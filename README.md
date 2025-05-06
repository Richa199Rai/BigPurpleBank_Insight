BigPurpleBank - Cloud App Technical Assessment

This project is a technical assessment for Insight, showcasing cloud application development and deployment using .NET, Azure, and Infrastructure as Code.

--------------------------------------------------

Features Implemented
- .NET 8 Web API for `/banking/accounts`
- API Key Authentication via custom middleware
- Azure SQL Database integration
- Azure Key Vault for securely storing connection strings
- CI/CD Pipeline using GitHub Actions
- Infrastructure as Code using Bicep (partial deployment)

--------------------------------------------------

Local Development Setup

Prerequisites
- [.NET 8 SDK]
- [SQL Server]

Run Locally

dotnet restore
dotnet build
dotnet run --project BankingApi/BankingApi.csproj


Test API
Use PowerShell or Postman:
powershell
Invoke-RestMethod -Uri "http://localhost:5282/banking/accounts" -Headers @{"X-API-Key" = "TestApiKey123"}

> Update `appsettings.json` with your local SQL connection string.

--------------------------------------------------

API Security
This API uses custom middleware to validate requests using an `X-API-Key` header. The key is read from configuration or Key Vault in production.

--------------------------------------------------

Azure Resources
The project uses these Azure services:
- Azure App Service (Linux)
- Azure SQL Database
- Azure Key Vault
- Azure Resource Group: `bigpurplebank-rg`

--------------------------------------------------

Infrastructure as Code - Bicep
File: `main.bicep`

- Deploys App Service, SQL Server + DB, Key Vault
- Adds connection string secret to Key Vault

--------------------------------------------------

CI/CD Pipeline
File: `.github/workflows/deploy.yml`

This GitHub Actions workflow:
- Builds and tests the .NET app
- Publishes it to Azure App Service
- Uses a service principal (`AZURE_CREDENTIALS`) for authentication

Azure Secret Setup
Set the following secret in your GitHub repo:

- `AZURE_CREDENTIALS`: JSON output from `az ad sp create-for-rbac --sdk-auth`

--------------------------------------------------

Project Structure

BigPurpleBank/
├── BankingApi/           # Main Web API project
├── BankingApi.Tests/     # xUnit test project
├── infra/main.bicep      # Azure infra as code
└── .github/workflows/    # GitHub Actions CI/CD pipeline

Notes
- Swagger UI was intentionally disabled for this assessment.
- Connection string is securely fetched from Key Vault when deployed to Azure.

--------------------------------------------------

Richa Rai  
GitHub: [@Richa199Rai](https://github.com/Richa199Rai)
