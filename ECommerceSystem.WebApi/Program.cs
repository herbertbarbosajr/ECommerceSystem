using ECommerceSystem.Application.Mappings;
using ECommerceSystem.Core.Entities;
using ECommerceSystem.Core.Interfaces;
using ECommerceSystem.WebApi.HealthChecks;
using ECommerceSystem.Infrastructure;
using ECommerceSystem.Infrastructure.Data;
using ECommerceSystem.WebApi.Controllers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using System.Text.Json;
using Microsoft.Extensions.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Configure health checks
builder.Services.AddHealthChecks()
    .AddDbContextCheck<ECommerceDbContext>("database", tags: new[] { "ready" })
    .AddNpgSql(
        builder.Configuration.GetConnectionString("DefaultConnection")!,
        name: "postgres",
        tags: new[] { "ready" },
        timeout: TimeSpan.FromSeconds(3))
    .AddCheck<MigrationsHealthCheck>("migrations", tags: new[] { "ready" });

// Add migrations health check as a service
builder.Services.AddScoped<MigrationsHealthCheck>();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { 
        Title = "ECommerce API", 
        Version = "v1",
        Description = @"## Health Check Endpoints
- GET /health/live - Liveness probe: verifies if the application is running
- GET /health/ready - Readiness probe: verifies if the application is ready to accept requests (database, migrations)"
    });

    // Add JWT Bearer token support
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Database
builder.Services.AddDbContext<ECommerceDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        npgsql => npgsql.MigrationsAssembly(typeof(ECommerceSystem.Infrastructure.Data.ECommerceDbContext).Assembly.FullName))
           .ConfigureWarnings(warnings => warnings.Ignore(RelationalEventId.PendingModelChangesWarning)));

// Identity
builder.Services.AddIdentity<ECommerceSystem.Core.Entities.User, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<ECommerceDbContext>()
.AddDefaultTokenProviders();

// JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
    };
});

// Authorization policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("CustomerOnly", policy => policy.RequireRole("Customer"));
});

// Application services
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ECommerceSystem.Application.Handlers.CreateProductHandler).Assembly));

// CORS configuration for frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
                "http://localhost:3000",
                "http://localhost:80",
                "http://127.0.0.1:3000",
                "http://ecommerce_web:3000"
            )
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

var app = builder.Build();

// Apply migrations (with logging of pending/applied migrations for debugging)
using (var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    var db = scope.ServiceProvider.GetRequiredService<ECommerceDbContext>();

    try
    {
        var applied = db.Database.GetAppliedMigrations().ToList();
        var pending = db.Database.GetPendingMigrations().ToList();
        logger.LogInformation("Applied migrations count: {count}", applied.Count);
        logger.LogInformation("Pending migrations count: {count}", pending.Count);

        if (pending.Any())
        {
            logger.LogInformation("Applying {count} pending migrations...", pending.Count);
            db.Database.Migrate();
            logger.LogInformation("Migrations applied successfully.");
        }
        else
        {
            logger.LogInformation("No pending migrations to apply.");
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while applying migrations.");
        throw;
    }

    // Ensure Identity roles exist (idempotent)
    try
    {
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var roles = new[] { "Admin", "Customer" };

        foreach (var roleName in roles)
        {
            var exists = roleManager.RoleExistsAsync(roleName).GetAwaiter().GetResult();
            if (!exists)
            {
                var createResult = roleManager.CreateAsync(new IdentityRole(roleName)).GetAwaiter().GetResult();
                if (!createResult.Succeeded)
                {
                    logger.LogWarning("Failed to create role {role}: {errors}", roleName, string.Join(", ", createResult.Errors.Select(e => e.Description)));
                }
                else
                {
                    logger.LogInformation("Created role '{role}'", roleName);
                }
            }
            else
            {
                logger.LogInformation("Role '{role}' already exists", roleName);
            }
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while seeding roles.");
        throw;
    }

    // Optionally create an initial admin user in Development when configured
    try
    {
        var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var env = scope.ServiceProvider.GetRequiredService<Microsoft.AspNetCore.Hosting.IWebHostEnvironment>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ECommerceSystem.Core.Entities.User>>();

        if (env.IsDevelopment())
        {
            var enabled = config.GetValue<bool>("InitialAdmin:Enabled");
            var email = config["InitialAdmin:Email"];
            var password = config["InitialAdmin:Password"];

            if (enabled && !string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password))
            {
                var admins = userManager.GetUsersInRoleAsync("Admin").GetAwaiter().GetResult();
                if (!admins.Any())
                {
                    var adminUser = new ECommerceSystem.Core.Entities.User
                    {
                        UserName = email,
                        Email = email,
                        FirstName = "Admin",
                        LastName = "User",
                        IsAdmin = true
                    };

                    var createResult = userManager.CreateAsync(adminUser, password).GetAwaiter().GetResult();
                    if (createResult.Succeeded)
                    {
                        var addRoleResult = userManager.AddToRoleAsync(adminUser, "Admin").GetAwaiter().GetResult();
                        if (addRoleResult.Succeeded)
                        {
                            logger.LogInformation("Created initial admin user '{email}'", email);
                        }
                        else
                        {
                            logger.LogWarning("Failed to add initial admin to role: {errors}", string.Join(", ", addRoleResult.Errors.Select(e => e.Description)));
                        }
                    }
                    else
                    {
                        logger.LogWarning("Failed to create initial admin user: {errors}", string.Join(", ", createResult.Errors.Select(e => e.Description)));
                    }
                }
                else
                {
                    logger.LogInformation("Initial admin not created because an admin user already exists.");
                }
            }
            else
            {
                logger.LogInformation("Initial admin creation disabled or credentials not provided (InitialAdmin:Enabled, InitialAdmin:Email, InitialAdmin:Password).");
            }
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while creating the initial admin.");
        throw;
    }
}

// Configure the HTTP request pipeline.
app.UseRouting();

// Enable CORS as early as possible (before other middleware)
app.UseCors("AllowFrontend");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Add health check endpoints
app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = _ => true, // All checks for liveness
    ResponseWriter = async (context, report) =>
    {
        var result = JsonSerializer.Serialize(new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(entry => new
            {
                name = entry.Key,
                status = entry.Value.Status.ToString(),
                duration = entry.Value.Duration.ToString(),
                description = entry.Value.Description
            })
        });

        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(result);
    }
});

app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = (check) => check.Tags.Contains("ready"),
    ResponseWriter = async (context, report) =>
    {
        var result = JsonSerializer.Serialize(new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(entry => new
            {
                name = entry.Key,
                status = entry.Value.Status.ToString(),
                duration = entry.Value.Duration.ToString(),
                description = entry.Value.Description
            })
        });

        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(result);
    }
});

app.Run();
