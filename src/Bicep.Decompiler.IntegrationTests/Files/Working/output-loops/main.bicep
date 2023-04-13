param storageCount int = 2

var baseName = 'storage${uniqueString(resourceGroup().id)}'

resource base 'Microsoft.Storage/storageAccounts@2019-04-01' = [for i in range(0, storageCount): {
  name: concat(i, baseName)
//@[008:027) [prefer-interpolation (Warning)] Use string interpolation instead of the concat function. (CodeDescription: bicep core(https://aka.ms/bicep/linter/prefer-interpolation)) |concat(i, baseName)|
  location: resourceGroup().location
//@[012:036) [no-loc-expr-outside-params (Warning)] Use a parameter here instead of 'resourceGroup().location'. 'resourceGroup().location' and 'deployment().location' should only be used as a default value for parameters. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-loc-expr-outside-params)) |resourceGroup().location|
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'Storage'
  properties: {}
}]

output storageEndpoints array = [for i in range(0, storageCount): reference(resourceId('Microsoft.Storage/storageAccounts', concat(i, baseName))).primaryEndpoints.blob]
//@[066:145) [use-resource-symbol-reference (Warning)] Use a resource reference instead of invoking function "reference". This simplifies the syntax and allows Bicep to better understand your deployment dependency graph. (CodeDescription: bicep core(https://aka.ms/bicep/linter/use-resource-symbol-reference)) |reference(resourceId('Microsoft.Storage/storageAccounts', concat(i, baseName)))|
//@[124:143) [prefer-interpolation (Warning)] Use string interpolation instead of the concat function. (CodeDescription: bicep core(https://aka.ms/bicep/linter/prefer-interpolation)) |concat(i, baseName)|
output copyIndex array = [for i in range(0, storageCount): i]
output copyIndexWithInt array = [for i in range(0, storageCount): (i + 123)]
