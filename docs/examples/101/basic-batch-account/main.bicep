param BatchaccountName string
param allocationMode string {
  default: 'BatchService'
  allowed: [
    'BatchService'
    'Usersubscription'
  ]
}

param location string = resourceGroup().location

resource batchaccount 'Microsoft.Batch/batchAccounts@2020-09-01' = {
  name: BatchaccountName
  location: location
  properties: {
    poolAllocationMode: allocationMode
  }
}

output batchaccountFQDN string = batchaccount.properties.accountEndpoint