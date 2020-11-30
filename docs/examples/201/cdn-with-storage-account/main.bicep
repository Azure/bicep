param location string = resourceGroup().location

var storageAccountName = 'storage${uniqueString(resourceGroup().id)}'
var endPointName = 'endpoint-${uniqueString(resourceGroup().id)}'
var profileName = 'CdnProfile1'
var storageAccountHostName = replace(replace(storageAccount.properties.primaryEndpoints.blob, 'https://', ''), '/', '')

resource storageAccount 'Microsoft.Storage/storageAccounts@2020-08-01-preview' = {
  location: location
  name: storageAccountName
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
  }
}

resource cdnProfile 'Microsoft.Cdn/profiles@2020-04-15' = {
  location: location
  name: profileName
  sku: {
    name: 'Standard_Akamai'
  }
}

resource endpoint 'Microsoft.Cdn/profiles/endpoints@2020-04-15' = {
  location: location
  name: endPointName
  properties: {
    originHostHeader: storageAccountHostName
    isHttpAllowed: true
    isHttpsAllowed: true
    queryStringCachingBehavior: 'IgnoreQueryString'
    contentTypesToCompress: [
      'text/plain'
      'text/html'
      'text/css'
      'application/x-javascript'
      'text/javascript'
    ]
    isCompressionEnabled: true
    origins: [
      {
        name: 'origin1'
        properties: {
          hostName: storageAccountHostName
        }
      }
    ]
  }
}

output hostName string = endpoint.properties.hostname
output originHostHeader string = endpoint.properties.originHostHeader
