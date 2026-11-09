# RaceDay

A race event management web application built with ASP.NET Core.

## Projects

- **RaceDay.API** - REST API (ASP.NET Core Web API, .NET 8)
- **RaceDay.MVC** - Web frontend (ASP.NET Core MVC, .NET 8)
- **RaceDay.Tests** - Unit tests (xUnit)

## Prerequisites

- .NET 8 SDK
- SQL Server Express
- Docker Desktop
- Azurite (Azure Storage Emulator)

## Running the API

1. Open `RaceDay.API/RaceDay.API.sln` in Visual Studio
2. Set connection string in `RaceDay.API/appsettings.json`
3. Run database migrations:

Update-Database

4. Start the API project (runs on https://localhost:7174)

## Running the MVC

1. Open terminal in the RaceDay root folder
2. Run:

dotnet run --project RaceDay.MVC/RaceDay.MVC.csproj


## Running with Docker

1. Make sure the API is running on port 7174
2. From the RaceDay root folder run:

docker-compose up --build

3. Open browser at http://localhost:8080

## Azure Blob Storage

This project uses Azurite for local blob storage emulation. Start Azurite before running the application:

azurite --silent --location c:\azurite --debug c:\azurite\debug.log


Images uploaded to events are stored in the `event-images` container.

## CI/CD

GitHub Actions pipeline runs on every push to main. See `.github/workflows/ci.yml`.