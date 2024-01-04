type foo = resource < 'Microsoft.Storage/storageAccounts@2023-01-01' >

param bar resource < 'Microsoft.Resources/tags@2022-09-01' > = {
  name: 'default'
  properties: {
    tags: {
      fizz: 'buzz'
      snap: 'crackle'
    }
  }
}

output baz resource < 'Microsoft.ManagedIdentity/userAssignedIdentities@2023-01-31' > = {
  name: 'myId'
  location: 'eastus'
}
