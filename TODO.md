# TODO: Switch to PostgreSQL with Docker Setup

## Completed Tasks
- [x] Update ECommerceSystem.WebApi.csproj: Replace SqlServer package with Npgsql.EntityFrameworkCore.PostgreSQL
- [x] Modify Program.cs: Change UseSqlServer to UseNpgsql
- [x] Update appsettings.json: Replace connection string with PostgreSQL format
- [x] Create Dockerfile: Multi-stage build for the WebApi project
- [x] Create docker-compose.yml: Define PostgreSQL service, app service, network, and volume
- [x] Update README.md: Add Docker setup section with instructions

## Followup Steps
- [x] Run EF migrations to ensure compatibility (attempted, but Docker not running)
- [x] Test Docker setup by building and running containers (attempted, Docker not available)
- [ ] Verify database connectivity and API functionality (pending Docker availability)
