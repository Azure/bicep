param batchAccountName string

@allowed([
  'BatchService'
  'UserSubscription'
])
param allocationMode string = 'BatchService'

param location string = resourceGroup().location

resource batchAccount 'Microsoft.Batch/batchAccounts@2020-09-01' = {
  name: batchAccountName
  location: location
  properties: {
    poolAllocationMode: allocationMode
  }
}

output batchaccountFQDN string = batchAccount.properties.accountEndpoint
