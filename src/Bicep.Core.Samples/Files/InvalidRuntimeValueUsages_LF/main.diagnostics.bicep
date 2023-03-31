param location string = resourceGroup().location

resource foo 'Microsoft.Storage/storageAccounts@2022-09-01' = {
  name: 'foo'
  location: location
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
}
resource foos 'Microsoft.Storage/storageAccounts@2022-09-01' = [for i in range(0, 2): {
  name: 'foo-${i}'
  location: location
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
}]
param strParam string = 'id'
var idAccessor = 'id'

var varForBodyOk1 = [for i in range(0, 2): foo.id]
//@[04:17) [no-unused-vars (Warning)] Variable "varForBodyOk1" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |varForBodyOk1|
var varForBodyOk2 = [for i in range(0, 2): foos[0].id]
//@[04:17) [no-unused-vars (Warning)] Variable "varForBodyOk2" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |varForBodyOk2|
var varForBodyOk3 = [for i in range(0, 2): foos[i].id]
//@[04:17) [no-unused-vars (Warning)] Variable "varForBodyOk3" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |varForBodyOk3|
var varForBodyOk4 = [for i in range(0, 2): foo[idAccessor]]
//@[04:17) [no-unused-vars (Warning)] Variable "varForBodyOk4" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |varForBodyOk4|
var varForBodyBad1 = [for i in range(0, 2): foo.properties]
//@[04:18) [no-unused-vars (Warning)] Variable "varForBodyBad1" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |varForBodyBad1|
//@[44:58) [BCP182 (Error)] This expression is being used in the for-body of the variable "varForBodyBad1", which requires values that can be calculated at the start of the deployment. Properties of foo which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |foo.properties|
var varForBodyBad2 = [for i in range(0, 2): foos[0].properties]
//@[04:18) [no-unused-vars (Warning)] Variable "varForBodyBad2" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |varForBodyBad2|
//@[44:62) [BCP182 (Error)] This expression is being used in the for-body of the variable "varForBodyBad2", which requires values that can be calculated at the start of the deployment. Properties of foos which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |foos[0].properties|
var varForBodyBad3 = [for i in range(0, 2): {
//@[04:18) [no-unused-vars (Warning)] Variable "varForBodyBad3" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |varForBodyBad3|
  prop: foos[0].properties
//@[08:26) [BCP182 (Error)] This expression is being used in the for-body of the variable "varForBodyBad3", which requires values that can be calculated at the start of the deployment. Properties of foos which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |foos[0].properties|
}]
var varForBodyBad4 = [for i in range(0, 2): foos[i].properties.accessTier]
//@[04:18) [no-unused-vars (Warning)] Variable "varForBodyBad4" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |varForBodyBad4|
//@[44:62) [BCP182 (Error)] This expression is being used in the for-body of the variable "varForBodyBad4", which requires values that can be calculated at the start of the deployment. Properties of foos which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |foos[i].properties|
var varForBodyBad5 = [for i in range(0, 2): foo[strParam]]
//@[04:18) [no-unused-vars (Warning)] Variable "varForBodyBad5" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |varForBodyBad5|
//@[44:57) [BCP182 (Error)] This expression is being used in the for-body of the variable "varForBodyBad5", which requires values that can be calculated at the start of the deployment. Properties of foo which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |foo[strParam]|

