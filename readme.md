# Big Purple Bank API

## Overview
This project implements the Consumer Data Standards Banking API's Get Accounts endpoint for Big Purple Bank. It uses Azure PaaS, .NET 6, and a CI/CD pipeline with Azure DevOps.

## Architecture
- **Backend**: .NET 6 API with Entity Framework Core, hosted on Azure App Service.
- **Database**: Azure SQL Database, with connection string stored in Azure Key Vault.
- **Security**: API key authentication, error handling middleware.
- **Infrastructure**: Deployed via Bicep (IaC) for App Service, SQL Server, Key Vault.
- **CI/CD**: Azure DevOps pipeline for build, test, and deployment.
- **Tests**: Unit tests with xUnit, covering account retrieval scenarios.

## Setup
1. Clone the repository.
2. Restore dependencies: `dotnet restore`.
3. Set up Azure resources using `main.bicep` (see `az deployment` command).
4. Configure Azure DevOps pipeline with `azure-pipelines.yml`.
5. Run locally: `dotnet run --project BankingApi`.
6. Test endpoint: `curl -H "X-API-Key: <key>" https://<app-url>/banking/accounts`.

## Deployment
- Deploy infrastructure: `az deployment group create ...`.
- Deploy app: Configured in Azure DevOps or manually via `az webapp deployment`.
- Smoke test included in pipeline.

## Testing
- Run tests: `dotnet test BankingApi.Tests`.
- Tests cover account retrieval with multiple accounts, empty results, and various currencies.

## Notes
- Uses managed identity for Key Vault access.
- Follows Consumer Data Standards for API response format.