resource foo 'Microsoft.Storage/storageAccounts@2022-09-01' = {
//@[09:12) Resource foo. Type: Microsoft.Storage/storageAccounts@2022-09-01. Declaration start char: 0, length: 171
  name: 'foo'
  location: deployment().location
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
}
resource foos 'Microsoft.Storage/storageAccounts@2022-09-01' = [for i in range(0, 2): {
//@[68:69) Local i. Type: int. Declaration start char: 68, length: 1
//@[09:13) Resource foos. Type: Microsoft.Storage/storageAccounts@2022-09-01[]. Declaration start char: 0, length: 201
  name: 'foo-${i}'
  location: deployment().location
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
}]
param strParam string = 'id'
//@[06:14) Parameter strParam. Type: string. Declaration start char: 0, length: 28
var idAccessor = 'id'
//@[04:14) Variable idAccessor. Type: 'id'. Declaration start char: 0, length: 21

var varForBodyOk1 = [for i in range(0, 2): foo.id]
//@[25:26) Local i. Type: int. Declaration start char: 25, length: 1
//@[04:17) Variable varForBodyOk1. Type: string[]. Declaration start char: 0, length: 50
var varForBodyOk2 = [for i in range(0, 2): foos[0].id]
//@[25:26) Local i. Type: int. Declaration start char: 25, length: 1
//@[04:17) Variable varForBodyOk2. Type: string[]. Declaration start char: 0, length: 54
var varForBodyOk3 = [for i in range(0, 2): foos[i].id]
//@[25:26) Local i. Type: int. Declaration start char: 25, length: 1
//@[04:17) Variable varForBodyOk3. Type: string[]. Declaration start char: 0, length: 54
var varForBodyOk4 = [for i in range(0, 2): foo[idAccessor]]
//@[25:26) Local i. Type: int. Declaration start char: 25, length: 1
//@[04:17) Variable varForBodyOk4. Type: array. Declaration start char: 0, length: 59
var varForBodyBad1 = [for i in range(0, 2): foo.properties]
//@[26:27) Local i. Type: int. Declaration start char: 26, length: 1
//@[04:18) Variable varForBodyBad1. Type: StorageAccountPropertiesCreateParametersOrStorageAccountProperties[]. Declaration start char: 0, length: 59
var varForBodyBad2 = [for i in range(0, 2): foos[0].properties]
//@[26:27) Local i. Type: int. Declaration start char: 26, length: 1
//@[04:18) Variable varForBodyBad2. Type: StorageAccountPropertiesCreateParametersOrStorageAccountProperties[]. Declaration start char: 0, length: 63
var varForBodyBad3 = [for i in range(0, 2): {
//@[26:27) Local i. Type: int. Declaration start char: 26, length: 1
//@[04:18) Variable varForBodyBad3. Type: object[]. Declaration start char: 0, length: 75
  prop: foos[0].properties
}]
var varForBodyBad4 = [for i in range(0, 2): foos[i].properties.accessTier]
//@[26:27) Local i. Type: int. Declaration start char: 26, length: 1
//@[04:18) Variable varForBodyBad4. Type: ('Cool' | 'Hot' | 'Premium')[]. Declaration start char: 0, length: 74
var varForBodyBad5 = [for i in range(0, 2): foo[strParam]]
//@[26:27) Local i. Type: int. Declaration start char: 26, length: 1
//@[04:18) Variable varForBodyBad5. Type: array. Declaration start char: 0, length: 58

