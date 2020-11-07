resource storage 'Microsoft.Storage/storageAccounts@2019-06-01' = {
    name: 'myuniquesaname'
    location: 'eastus'
    kind: 'Storage'
    sku: {
        name: 'Standard_LRS'
    }
  }
  