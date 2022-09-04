? /* TODO: User defined functions are not supported and have not been decompiled */
//@[00:01) [BCP007 (Error)] This declaration type is not recognized. Specify a metadata, parameter, variable, resource, or output declaration. (CodeDescription: none) |?|

@maxLength(11)
param storageNamePrefix string

resource storageNamePrefix_resource 'Microsoft.Storage/storageAccounts@2019-04-01' = {
  name: contoso.uniqueName(storageNamePrefix)
//@[08:15) [BCP057 (Error)] The name "contoso" does not exist in the current context. (CodeDescription: none) |contoso|
  location: 'South Central US'
//@[12:30) [no-hardcoded-location (Warning)] A resource location should not use a hard-coded string or variable value. Please use a parameter value, an expression, or the string 'global'. Found: 'South Central US' (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-hardcoded-location)) |'South Central US'|
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
  properties: {
    supportsHttpsTrafficOnly: true
  }
}
