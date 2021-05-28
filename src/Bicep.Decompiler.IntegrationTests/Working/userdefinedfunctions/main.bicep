? /* TODO: User defined functions are not supported and have not been decompiled */
//@[0:1) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |?|

@maxLength(11)
param storageNamePrefix string

resource storageNamePrefix_resource 'Microsoft.Storage/storageAccounts@2019-04-01' = {
  name: contoso.uniqueName(storageNamePrefix)
//@[8:15) [BCP057 (Error)] The name "contoso" does not exist in the current context. (CodeDescription: none) |contoso|
  location: 'South Central US'
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
  properties: {
    supportsHttpsTrafficOnly: true
  }
}
