# README.md

# Company Management System

## Overview
The Company Management System is a .NET application designed to manage purchase orders, including functionalities for creating, retrieving, and deleting orders. It also handles notifications based on order dates.

## Project Structure
- **src/Controllers**: Contains the `PurchaseOrdersController` class for managing purchase order operations.
- **src/Models**: Contains model classes representing data structures such as `PurchaseOrder`, `LineItem`, and `Company`.
- **src/Data**: Contains the database context class for managing database connections and entity sets.
- **src/Program.cs**: The entry point for the application.
- **tests/Controllers**: Contains unit tests for the `PurchaseOrdersController`.

## Deployment
Deployment scripts are provided in the `scripts` directory:
- **deploy.ps1**: PowerShell script for deployment.
- **deploy.sh**: Shell script for deployment in Unix-like environments.

## Configuration
Configuration settings are stored in:
- **appsettings.json**: General application settings.
- **appsettings.Development.json**: Development-specific settings.

## GitHub Actions
A GitHub Actions workflow for deployment is defined in `.github/workflows/deploy.yml`.

## Getting Started
1. Clone the repository.
2. Run `dotnet restore` to restore dependencies.
3. Run `dotnet run` to start the application.

## License
This project is licensed under the MIT License.