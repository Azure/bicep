param name string

@allowed([
  'Standard_LRS'
  'Standard_GRS'
  'Standard_RAGRS'
  'Standard_ZRS'
  'Premium_LRS'
  'Premium_ZRS'
  'Standard_GZRS'
  'Standard_RAGZRS'
])
param sku string = 'Standard_LRS'

@allowed([
  'Storage'
  'StorageV2'
  'BlobStorage'
  'FileStorage'
  'BlockBlobStorage'
])
param kind string = 'StorageV2'

@allowed([
  'Hot'
  'Cool'
])
param accessTier string = 'Hot'
param fileShareName string = 'deployscript'
param location string = resourceGroup().id

resource storage 'Microsoft.Storage/storageAccounts@2019-06-01' = {
  name: name
  location: location
  sku: {
    name: sku
  }
  kind: kind
  properties: {
    accessTier: accessTier
  }
}

resource fileshare 'Microsoft.Storage/storageAccounts/fileServices/shares@2019-06-01' = {
  name: '${storage.name}/default/${fileShareName}'
}

output resourceId string = storage.id
output storageName string = storage.name
output fileShareName string = fileShareName
