? /* TODO: User defined functions are not supported and have not been decompiled */
//@[0:01) [BCP007 (Error)] This declaration type is not recognized. Specify a metadata, parameter, variable, resource, or output declaration. (bicep https://aka.ms/bicep/core-diagnostics#BCP007) |?|

@maxLength(11)
param storageNamePrefix string

resource storageNamePrefix_resource 'Microsoft.Storage/storageAccounts@2019-04-01' = {
  name: contoso.uniqueName(storageNamePrefix)
//@[8:15) [BCP057 (Error)] The name "contoso" does not exist in the current context. (bicep https://aka.ms/bicep/core-diagnostics#BCP057) |contoso|
  location: 'South Central US'
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
  properties: {
    supportsHttpsTrafficOnly: true
  }
}

