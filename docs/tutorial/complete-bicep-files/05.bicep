param storageAccountName string // need to be provided since it is existing

param containerNames array = [
  'dogs'
  'cats'
  'fish'
]

resource stg 'Microsoft.Storage/storageAccounts@2019-06-01' existing = {
  name: storageAccountName
}

resource blob 'Microsoft.Storage/storageAccounts/blobServices/containers@2019-06-01' = [for name in containerNames: {
  name: '${stg.name}/default/${name}'
  // dependsOn will be added when the template is compiled
}]

output storageId string = stg.id // replacement for resourceId(...)
output primaryEndpoint string = stg.properties.primaryEndpoints.blob // replacement for reference(...).*
output containerProps array = [for i in range(0, length(containerNames)): blob[i].id]