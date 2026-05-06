.PHONY: list build run restore db-update db-reset compose-up compose-down

PROJECT = Server

list:
	@echo "Available commands:"
	@echo "build     - Compiles your project and checks for any build errors."
	@echo "run       - Runs your application using the specified project."
	@echo "restore   - Restores all NuGet packages required by your project."
	@echo "db-update - Applies Entity Framework migrations to update your database schema."
	@echo "db-reset  - Drops the database and recreates it with the latest migrations (force reset)."
	@echo "compose-up - Starts the Docker compose services in watch mode."
	@echo "compose-down - Stops the Docker compose services."

build:
	dotnet build

run:
	dotnet run --project $(PROJECT).csproj

restore:
	dotnet restore

db-update:
	dotnet ef database update

db-reset:
	dotnet ef database drop --force
	dotnet ef database update

compose-up:
	docker compose up --watch

compose-down:
	docker compose down