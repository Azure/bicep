module stg './storage.bicep' = {
  name: 'storageDeploy'
  scope: resourceGroup('another-rg') // this will target another resource group in the same subscription
  params: {
    namePrefix: 'contoso'
  }
}

output storageName string = stg.outputs.computedStorageName