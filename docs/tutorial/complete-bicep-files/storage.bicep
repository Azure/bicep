param location string = resourceGroup().location
param namePrefix string = 'stg'

param globalRedundancy bool = true // defaults to true, but can be overridden

var storageAccountName = '${namePrefix}${uniqueString(resourceGroup().id)}'

resource stg 'Microsoft.Storage/storageAccounts@2019-06-01' = {
  name: storageAccountName
  location: location
  kind: 'Storage'
  sku: {
    name: globalRedundancy ? 'Standard_GRS' : 'Standard_LRS' // if true --> GRS, else --> LRS
  }
}

resource blob 'Microsoft.Storage/storageAccounts/blobServices/containers@2019-06-01' = {
  name: '${stg.name}/default/logs'
  // dependsOn will be added when the template is compiled
}

output storageId string = stg.id // replacement for resourceId(...)
output computedStorageName string = stg.name
output primaryEndpoint string = stg.properties.primaryEndpoints.blob // replacement for reference(...).*