resource storage 'Microsoft.Storage/storageAccounts@2019-06-01' = {
    name: 'exampleuniquesaname'
    location: 'eastus'
    kind: 'Storage'
    sku: {
        name: 'Standard_LRS'
    }
  }
  