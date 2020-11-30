param location string = resourceGroup().location

var storageAccountName = format('{0}{1}', 'storage', uniqueString(resourceGroup().id))
var endPointName = format('{0}{1}', 'endpoint-', uniqueString(resourceGroup().id))
var profileName = 'CdnProfile1'
var storageAccountHostName = replace(replace(reference(storageAccountName).primaryEndpoints.blob, 'https://', ''), '/', '')

resource storageAccount 'Microsoft.Storage/storageAccounts@2020-08-01-preview' = {
  location: location
  name: storageAccountName
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
  }
  properties: {}
}

resource cdnProfile 'Microsoft.Cdn/profiles@2020-04-15' = {
  location: location
  name: profileName
  sku: {
    name: 'Standard_Akamai'
  }
  properties: {}
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

output hostName string = reference(endPointName).hostName
output originHostHeader string = reference(endPointName).originHostHeader
