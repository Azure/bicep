param input string

resource storageAccount 'Microsoft.Storage/storageAccounts@2022-09-01' = {
    name: input
    location: 'eastus'
    sku: {
        name: 'Standard_LRS'
    }
    kind: 'StorageV2'
    properties: {
        accessTier: 'Hot'
    }
}

output storageAccountName string = storageAccount.name
