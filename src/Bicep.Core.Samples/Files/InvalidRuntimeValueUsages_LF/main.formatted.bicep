resource foo 'Microsoft.Storage/storageAccounts@2022-09-01' = {
  name: 'foo'
  location: deployment().location
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
}
resource foos 'Microsoft.Storage/storageAccounts@2022-09-01' = [for i in range(0, 2): {
  name: 'foo-${i}'
  location: deployment().location
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
}]
param strParam string = 'id'
var idAccessor = 'id'

var varForBodyOk1 = [for i in range(0, 2): foo.id]
var varForBodyOk2 = [for i in range(0, 2): foos[0].id]
var varForBodyOk3 = [for i in range(0, 2): foos[i].id]
var varForBodyOk4 = [for i in range(0, 2): foo[idAccessor]]
var varForBodyBad1 = [for i in range(0, 2): foo.properties]
var varForBodyBad2 = [for i in range(0, 2): foos[0].properties]
var varForBodyBad3 = [for i in range(0, 2): {
  prop: foos[0].properties
}]
var varForBodyBad4 = [for i in range(0, 2): foos[i].properties.accessTier]
var varForBodyBad5 = [for i in range(0, 2): foo[strParam]]
