.PHONY: list build run restore db-update db-reset

PROJECT = Server

list:
	@echo "make build     - Compiles your project and checks for any build errors."
	@echo "make run       - Runs your application using the specified project."
	@echo "make restore   - Restores all NuGet packages required by your project."
	@echo "make db-update - Applies Entity Framework migrations to update your database schema."
	@echo "make db-reset  - Drops the database and recreates it with the latest migrations (force reset)."

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
