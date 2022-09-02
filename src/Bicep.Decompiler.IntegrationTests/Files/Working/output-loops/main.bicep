param storageCount int = 2

var baseName_var = 'storage${uniqueString(resourceGroup().id)}'

resource baseName 'Microsoft.Storage/storageAccounts@2019-04-01' = [for i in range(0, storageCount): {
  name: concat(i, baseName_var)
//@[008:031) [prefer-interpolation (Warning)] Use string interpolation instead of the concat function. (CodeDescription: bicep core(https://aka.ms/bicep/linter/prefer-interpolation)) |concat(i, baseName_var)|
  location: resourceGroup().location
//@[012:036) [no-loc-expr-outside-params (Warning)] Use a parameter here instead of 'resourceGroup().location'. 'resourceGroup().location' and 'deployment().location' should only be used as a default value for parameters. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-loc-expr-outside-params)) |resourceGroup().location|
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'Storage'
  properties: {
  }
}]

output storageEndpoints array = [for i in range(0, storageCount): reference(resourceId('Microsoft.Storage/storageAccounts', concat(i, baseName_var))).primaryEndpoints.blob]
//@[124:147) [prefer-interpolation (Warning)] Use string interpolation instead of the concat function. (CodeDescription: bicep core(https://aka.ms/bicep/linter/prefer-interpolation)) |concat(i, baseName_var)|
output copyIndex array = [for i in range(0, storageCount): i]
output copyIndexWithInt array = [for i in range(0, storageCount): (i + 123)]
