@description('Location for all resources')
param location string = 'australiaeast'

@description('SQL admin username')
param sqlAdmin string = 'sqladminuser'

@secure()
@description('SQL admin password')
param sqlPassword string

@description('App Service name')
param appServiceName string = 'bigpurplebank-api'

@description('Azure SQL DB name')
param dbName string = 'bankingdb'

@description('Key Vault name')
param keyVaultName string = 'bigpurplebank-kv'

resource appServicePlan 'Microsoft.Web/serverfarms@2022-03-01' = {
  name: '${appServiceName}-plan'
  location: location
  sku: {
    name: 'B1'
    tier: 'Basic'
  }
  kind: 'linux'
  properties: {
    reserved: true
  }
}

resource webApp 'Microsoft.Web/sites@2022-03-01' = {
  name: appServiceName
  location: location
  kind: 'app'
  properties: {
    serverFarmId: appServicePlan.id
    siteConfig: {
      linuxFxVersion: 'DOTNET|8.0'
    }
  }
}

resource sqlServer 'Microsoft.Sql/servers@2022-05-01-preview' = {
  name: '${appServiceName}-sql'
  location: location
  properties: {
    administratorLogin: sqlAdmin
    administratorLoginPassword: sqlPassword
  }
}

resource sqlDb 'Microsoft.Sql/servers/databases@2022-05-01-preview' = {
  parent: sqlServer
  name: dbName
  location: location
  sku: {
    name: 'Basic'
    tier: 'Basic'
  }
}

resource keyVault 'Microsoft.KeyVault/vaults@2022-07-01' = {
  name: keyVaultName
  location: location
  properties: {
    tenantId: subscription().tenantId
    sku: {
      name: 'standard'
      family: 'A'
    }
    accessPolicies: []
    enableSoftDelete: true
    enabledForDeployment: true
    enabledForTemplateDeployment: true
    enabledForDiskEncryption: true
  }
}

// Set secret: SQL Connection String
resource sqlSecret 'Microsoft.KeyVault/vaults/secrets@2022-07-01' = {
  name: '${keyVault.name}/BankingDB'
  properties: {
    value: 'Server=tcp:${sqlServer.name}.database.windows.net,1433;Initial Catalog=${dbName};Persist Security Info=False;User ID=${sqlAdmin};Password=${sqlPassword};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;'
  }
  dependsOn: [
    sqlDb
    keyVault
  ]
}

// Output values
output appServiceUrl string = 'https://${webApp.name}.azurewebsites.net'
output sqlServerName string = sqlServer.name
output keyVaultName string = keyVault.name
