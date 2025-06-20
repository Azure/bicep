param storageCount int = 10
param storagePrefix string

@batchSize(3)
resource storagePrefix_id 'Microsoft.Storage/storageAccounts@2019-04-01' = [
  for i in range(0, storageCount): {
    name: toLower(concat(i, storagePrefix, uniqueString(resourceGroup().id)))
//@[18:76) [prefer-interpolation (Warning)] Use string interpolation instead of the concat function. (bicep core linter https://aka.ms/bicep/linter-diagnostics#prefer-interpolation) |concat(i, storagePrefix, uniqueString(resourceGroup().id))|
    location: resourceGroup().location
    sku: {
      name: 'Standard_LRS'
    }
    kind: 'Storage'
    properties: {}
  }
]

@batchSize(1)
resource one_id 'Microsoft.Storage/storageAccounts@2019-04-01' = [
  for i in range(0, storageCount): {
    name: toLower('${i}one${uniqueString(resourceGroup().id)}')
//@[10:63) [BCP335 (Warning)] The provided value can have a length as large as 35 and may be too long to assign to a target with a configured maximum length of 24. (bicep https://aka.ms/bicep/core-diagnostics#BCP335) |toLower('${i}one${uniqueString(resourceGroup().id)}')|
    location: resourceGroup().location
    sku: {
      name: 'Standard_LRS'
    }
    kind: 'Storage'
    properties: {}
  }
]

resource two_id 'Microsoft.Storage/storageAccounts@2019-04-01' = [
  for i in range(0, storageCount): {
    name: toLower('${i}two${uniqueString(resourceGroup().id)}')
//@[10:63) [BCP335 (Warning)] The provided value can have a length as large as 35 and may be too long to assign to a target with a configured maximum length of 24. (bicep https://aka.ms/bicep/core-diagnostics#BCP335) |toLower('${i}two${uniqueString(resourceGroup().id)}')|
    location: resourceGroup().location
    sku: {
      name: 'Standard_LRS'
    }
    kind: 'Storage'
    properties: {}
  }
]

