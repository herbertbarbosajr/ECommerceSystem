# Use the official .NET 9 SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy the solution file and restore dependencies
COPY ECommerceSystem.sln ./
COPY ECommerceSystem.Core/ECommerceSystem.Core.csproj ECommerceSystem.Core/
COPY ECommerceSystem.Application/ECommerceSystem.Application.csproj ECommerceSystem.Application/
COPY ECommerceSystem.Infrastructure/ECommerceSystem.Infrastructure.csproj ECommerceSystem.Infrastructure/
COPY ECommerceSystem.WebApi/ECommerceSystem.WebApi.csproj ECommerceSystem.WebApi/
COPY ECommerceSystem.Tests/ECommerceSystem.Tests.csproj ECommerceSystem.Tests/

RUN dotnet restore

# Copy the entire source code and build the application
COPY . .
WORKDIR /src/ECommerceSystem.WebApi
RUN dotnet build -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

# Use the official .NET 9 runtime image to run the application
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

EXPOSE 80
EXPOSE 443

ENTRYPOINT ["dotnet", "ECommerceSystem.WebApi.dll"]
