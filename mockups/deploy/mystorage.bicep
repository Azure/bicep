param location string
param storageAccountName string
param accountType string
param kind string
param accessTier string
param supportsHttpsTrafficOnly bool

resource storageAccount 'Microsoft.Storage/storageAccounts@2018-07-01' = {
  name: storageAccountName
  location: location
  properties: {
    accessTier: accessTier
    supportsHttpsTrafficOnly: supportsHttpsTrafficOnly
  }
  sku: {
    name: accountType
  }
  kind: kind
  dependsOn: []
}
