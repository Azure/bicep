param storageCount int = 10
param storagePrefix string

@batchSize(3)
resource storagePrefix_id 'Microsoft.Storage/storageAccounts@2019-04-01' = [for i in range(0, storageCount): {
  name: toLower(concat(i, storagePrefix, uniqueString(resourceGroup().id)))
//@[16:74) [prefer-interpolation (Warning)] Use string interpolation instead of the concat function. (CodeDescription: bicep core(https://aka.ms/bicep/linter/prefer-interpolation)) |concat(i, storagePrefix, uniqueString(resourceGroup().id))|
  location: resourceGroup().location
//@[12:36) [no-loc-expr-outside-params (Warning)] Use a parameter here instead of 'resourceGroup().location'. 'resourceGroup().location' and 'deployment().location' should only be used as a default value for parameters. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-loc-expr-outside-params)) |resourceGroup().location|
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'Storage'
  properties: {
  }
}]

@batchSize(1)
resource one_id 'Microsoft.Storage/storageAccounts@2019-04-01' = [for i in range(0, storageCount): {
  name: toLower('${i}one${uniqueString(resourceGroup().id)}')
  location: resourceGroup().location
//@[12:36) [no-loc-expr-outside-params (Warning)] Use a parameter here instead of 'resourceGroup().location'. 'resourceGroup().location' and 'deployment().location' should only be used as a default value for parameters. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-loc-expr-outside-params)) |resourceGroup().location|
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'Storage'
  properties: {
  }
}]

resource two_id 'Microsoft.Storage/storageAccounts@2019-04-01' = [for i in range(0, storageCount): {
  name: toLower('${i}two${uniqueString(resourceGroup().id)}')
  location: resourceGroup().location
//@[12:36) [no-loc-expr-outside-params (Warning)] Use a parameter here instead of 'resourceGroup().location'. 'resourceGroup().location' and 'deployment().location' should only be used as a default value for parameters. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-loc-expr-outside-params)) |resourceGroup().location|
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'Storage'
  properties: {
  }
}]
