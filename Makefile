.PHONY: build run restore db-update db-reset

PROJECT = Server

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
