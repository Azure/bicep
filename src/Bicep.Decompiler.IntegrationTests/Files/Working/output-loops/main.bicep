param storageCount int = 2

var baseName = 'storage${uniqueString(resourceGroup().id)}'

resource base 'Microsoft.Storage/storageAccounts@2019-04-01' = [
  for i in range(0, storageCount): {
    name: concat(i, baseName)
//@[10:029) [prefer-interpolation (Warning)] Use string interpolation instead of the concat function. (bicep core linter https://aka.ms/bicep/linter-diagnostics#prefer-interpolation) |concat(i, baseName)|
    location: resourceGroup().location
    sku: {
      name: 'Standard_LRS'
    }
    kind: 'Storage'
    properties: {}
  }
]

output storageEndpoints array = [
  for i in range(0, storageCount): reference(resourceId('Microsoft.Storage/storageAccounts', concat(i, baseName))).primaryEndpoints.blob
//@[35:114) [use-resource-symbol-reference (Warning)] Use a resource reference instead of invoking function "reference". This simplifies the syntax and allows Bicep to better understand your deployment dependency graph. (bicep core linter https://aka.ms/bicep/linter-diagnostics#use-resource-symbol-reference) |reference(resourceId('Microsoft.Storage/storageAccounts', concat(i, baseName)))|
//@[93:112) [prefer-interpolation (Warning)] Use string interpolation instead of the concat function. (bicep core linter https://aka.ms/bicep/linter-diagnostics#prefer-interpolation) |concat(i, baseName)|
]
output copyIndex array = [for i in range(0, storageCount): i]
output copyIndexWithInt array = [for i in range(0, storageCount): (i + 123)]

