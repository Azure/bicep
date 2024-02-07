param storageCount int = 2
param storagePrefix string

resource storagePrefix_id 'Microsoft.Storage/storageAccounts@2019-04-01' = [
  for i in range(0, storageCount): if (i == 1) {
    name: toLower(concat(i, storagePrefix, uniqueString(resourceGroup().id)))
//@[18:76) [prefer-interpolation (Warning)] Use string interpolation instead of the concat function. (CodeDescription: bicep core(https://aka.ms/bicep/linter/prefer-interpolation)) |concat(i, storagePrefix, uniqueString(resourceGroup().id))|
    location: resourceGroup().location
//@[14:38) [no-loc-expr-outside-params (Warning)] Use a parameter here instead of 'resourceGroup().location'. 'resourceGroup().location' and 'deployment().location' should only be used as a default value for parameters. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-loc-expr-outside-params)) |resourceGroup().location|
    sku: {
      name: 'Standard_LRS'
    }
    kind: 'Storage'
    properties: {}
  }
]

