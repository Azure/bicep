param location string 

resource storageAccount 'Microsoft.Storage/storageAccounts@2022-09-01' = {
  name: 'toylaunchstorage'
  location: location
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
  properties: {
    accessTier: 'Hot'
  }
}


assert storageAccountNameIsCorrect = storageAccount.name == 'toylaunchstorage2'
assert storageAccountLocationIsCorrect = storageAccount.location == 'westus3'



