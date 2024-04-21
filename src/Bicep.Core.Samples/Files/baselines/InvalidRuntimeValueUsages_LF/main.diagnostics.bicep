resource foo 'Microsoft.Storage/storageAccounts@2022-09-01' = {
  name: 'foo'
  location: 'westus'
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'

  resource fooChild 'fileServices' = {
    name: 'default'
  }
}
resource foos 'Microsoft.Storage/storageAccounts@2022-09-01' = [for i in range(0, 2): {
  name: 'foo-${i}'
  location: 'westus'
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
}]
resource existingFoo 'Microsoft.Storage/storageAccounts@2022-09-01' existing = {
  name: 'existingFoo'
}

param intParam int = 0
param strParam string = 'id'
param strParam2 string = 'd'
param cond bool = false

var zeroIndex = 0
var otherIndex = zeroIndex + 2
var idAccessor = 'id'
var dStr = 'd'
var idAccessor2 = idAccessor
//@[04:15) [no-unused-vars (Warning)] Variable "idAccessor2" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |idAccessor2|
var idAccessorInterpolated = '${idAccessor}'
//@[04:26) [no-unused-vars (Warning)] Variable "idAccessorInterpolated" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |idAccessorInterpolated|
//@[29:44) [simplify-interpolation (Warning)] Remove unnecessary string interpolation. (CodeDescription: bicep core(https://aka.ms/bicep/linter/simplify-interpolation)) |'${idAccessor}'|
var idAccessorMixed = 'i${dStr}'
//@[04:19) [no-unused-vars (Warning)] Variable "idAccessorMixed" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |idAccessorMixed|
var propertiesAccessor = 'properties'
var accessTierAccessor = 'accessTier'
var strArray = ['id', 'properties']
var intArray = [0, 1]

var varForBodyInvalidRuntimeUsages = [for i in range(0, 2): {
//@[04:34) [no-unused-vars (Warning)] Variable "varForBodyInvalidRuntimeUsages" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |varForBodyInvalidRuntimeUsages|
  case1: foo
//@[09:12) [BCP182 (Error)] This expression is being used in the for-body of the variable "varForBodyInvalidRuntimeUsages", which requires values that can be calculated at the start of the deployment. Properties of foo which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |foo|
  case2: existingFoo
//@[09:20) [BCP182 (Error)] This expression is being used in the for-body of the variable "varForBodyInvalidRuntimeUsages", which requires values that can be calculated at the start of the deployment. Properties of existingFoo which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |existingFoo|
  case3: foo::fooChild
//@[09:22) [BCP182 (Error)] This expression is being used in the for-body of the variable "varForBodyInvalidRuntimeUsages", which requires values that can be calculated at the start of the deployment. Properties of fooChild which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |foo::fooChild|
  case4: foos[0]
//@[09:13) [BCP182 (Error)] This expression is being used in the for-body of the variable "varForBodyInvalidRuntimeUsages", which requires values that can be calculated at the start of the deployment. Properties of foos which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |foos|
  case5: foos[i]
//@[09:13) [BCP182 (Error)] This expression is being used in the for-body of the variable "varForBodyInvalidRuntimeUsages", which requires values that can be calculated at the start of the deployment. Properties of foos which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |foos|
  case6: foos[i + 2]
//@[09:13) [BCP182 (Error)] This expression is being used in the for-body of the variable "varForBodyInvalidRuntimeUsages", which requires values that can be calculated at the start of the deployment. Properties of foos which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |foos|
  case7: foos[zeroIndex]
//@[09:13) [BCP182 (Error)] This expression is being used in the for-body of the variable "varForBodyInvalidRuntimeUsages", which requires values that can be calculated at the start of the deployment. Properties of foos which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |foos|
  case8: foos[otherIndex]
//@[09:13) [BCP182 (Error)] This expression is being used in the for-body of the variable "varForBodyInvalidRuntimeUsages", which requires values that can be calculated at the start of the deployment. Properties of foos which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |foos|
  case9: foo.properties
//@[09:23) [BCP182 (Error)] This expression is being used in the for-body of the variable "varForBodyInvalidRuntimeUsages", which requires values that can be calculated at the start of the deployment. The property "properties" of foo cannot be calculated at the start. Properties of foo which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |foo.properties|
  case10: existingFoo.properties
//@[10:32) [BCP182 (Error)] This expression is being used in the for-body of the variable "varForBodyInvalidRuntimeUsages", which requires values that can be calculated at the start of the deployment. The property "properties" of existingFoo cannot be calculated at the start. Properties of existingFoo which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |existingFoo.properties|
  case11: foo::fooChild.properties
//@[10:34) [BCP182 (Error)] This expression is being used in the for-body of the variable "varForBodyInvalidRuntimeUsages", which requires values that can be calculated at the start of the deployment. The property "properties" of fooChild cannot be calculated at the start. Properties of fooChild which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |foo::fooChild.properties|
  case12: foos[0].properties
//@[10:28) [BCP182 (Error)] This expression is being used in the for-body of the variable "varForBodyInvalidRuntimeUsages", which requires values that can be calculated at the start of the deployment. The property "properties" of foos cannot be calculated at the start. Properties of foos which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |foos[0].properties|
  case13: foos[i].properties
//@[10:28) [BCP182 (Error)] This expression is being used in the for-body of the variable "varForBodyInvalidRuntimeUsages", which requires values that can be calculated at the start of the deployment. The property "properties" of foos cannot be calculated at the start. Properties of foos which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |foos[i].properties|
  case14: foos[i + 2].properties
//@[10:32) [BCP182 (Error)] This expression is being used in the for-body of the variable "varForBodyInvalidRuntimeUsages", which requires values that can be calculated at the start of the deployment. The property "properties" of foos cannot be calculated at the start. Properties of foos which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |foos[i + 2].properties|
  case15: foos[zeroIndex].properties
//@[10:36) [BCP182 (Error)] This expression is being used in the for-body of the variable "varForBodyInvalidRuntimeUsages", which requires values that can be calculated at the start of the deployment. The property "properties" of foos cannot be calculated at the start. Properties of foos which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |foos[zeroIndex].properties|
  case16: foos[otherIndex].properties
//@[10:37) [BCP182 (Error)] This expression is being used in the for-body of the variable "varForBodyInvalidRuntimeUsages", which requires values that can be calculated at the start of the deployment. The property "properties" of foos cannot be calculated at the start. Properties of foos which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |foos[otherIndex].properties|
  case17: foo.properties.accessTier
//@[10:24) [BCP182 (Error)] This expression is being used in the for-body of the variable "varForBodyInvalidRuntimeUsages", which requires values that can be calculated at the start of the deployment. The property "properties" of foo cannot be calculated at the start. Properties of foo which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |foo.properties|
  case18: existingFoo.properties.accessTier
//@[10:32) [BCP182 (Error)] This expression is being used in the for-body of the variable "varForBodyInvalidRuntimeUsages", which requires values that can be calculated at the start of the deployment. The property "properties" of existingFoo cannot be calculated at the start. Properties of existingFoo which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |existingFoo.properties|
  case19: foo::fooChild.properties.accessTier
//@[10:34) [BCP182 (Error)] This expression is being used in the for-body of the variable "varForBodyInvalidRuntimeUsages", which requires values that can be calculated at the start of the deployment. The property "properties" of fooChild cannot be calculated at the start. Properties of fooChild which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |foo::fooChild.properties|
//@[35:45) [BCP053 (Warning)] The type "FileServicePropertiesProperties" does not contain property "accessTier". Available properties include "cors", "protocolSettings", "shareDeleteRetentionPolicy". (CodeDescription: none) |accessTier|
  case20: foos[0].properties.accessTier
//@[10:28) [BCP182 (Error)] This expression is being used in the for-body of the variable "varForBodyInvalidRuntimeUsages", which requires values that can be calculated at the start of the deployment. The property "properties" of foos cannot be calculated at the start. Properties of foos which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |foos[0].properties|
  case21: foos[i].properties.accessTier
//@[10:28) [BCP182 (Error)] This expression is being used in the for-body of the variable "varForBodyInvalidRuntimeUsages", which requires values that can be calculated at the start of the deployment. The property "properties" of foos cannot be calculated at the start. Properties of foos which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |foos[i].properties|
  case22: foos[i + 2].properties.accessTier
//@[10:32) [BCP182 (Error)] This expression is being used in the for-body of the variable "varForBodyInvalidRuntimeUsages", which requires values that can be calculated at the start of the deployment. The property "properties" of foos cannot be calculated at the start. Properties of foos which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |foos[i + 2].properties|
  case23: foos[zeroIndex].properties.accessTier
//@[10:36) [BCP182 (Error)] This expression is being used in the for-body of the variable "varForBodyInvalidRuntimeUsages", which requires values that can be calculated at the start of the deployment. The property "properties" of foos cannot be calculated at the start. Properties of foos which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |foos[zeroIndex].properties|
  case24: foos[otherIndex].properties.accessTier
//@[10:37) [BCP182 (Error)] This expression is being used in the for-body of the variable "varForBodyInvalidRuntimeUsages", which requires values that can be calculated at the start of the deployment. The property "properties" of foos cannot be calculated at the start. Properties of foos which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |foos[otherIndex].properties|
  case25: foo['properties']
//@[10:27) [BCP182 (Error)] This expression is being used in the for-body of the variable "varForBodyInvalidRuntimeUsages", which requires values that can be calculated at the start of the deployment. The property "properties" of foo cannot be calculated at the start. Properties of foo which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |foo['properties']|
//@[13:27) [prefer-unquoted-property-names (Warning)] Property names that are valid identifiers should be declared without quotation marks and accessed using dot notation. (CodeDescription: bicep core(https://aka.ms/bicep/linter/prefer-unquoted-property-names)) |['properties']|
  case26: existingFoo['properties']
//@[10:35) [BCP182 (Error)] This expression is being used in the for-body of the variable "varForBodyInvalidRuntimeUsages", which requires values that can be calculated at the start of the deployment. The property "properties" of existingFoo cannot be calculated at the start. Properties of existingFoo which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |existingFoo['properties']|
//@[21:35) [prefer-unquoted-property-names (Warning)] Property names that are valid identifiers should be declared without quotation marks and accessed using dot notation. (CodeDescription: bicep core(https://aka.ms/bicep/linter/prefer-unquoted-property-names)) |['properties']|
  case27: foo::fooChild['properties']
//@[10:37) [BCP182 (Error)] This expression is being used in the for-body of the variable "varForBodyInvalidRuntimeUsages", which requires values that can be calculated at the start of the deployment. The property "properties" of fooChild cannot be calculated at the start. Properties of fooChild which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |foo::fooChild['properties']|
//@[23:37) [prefer-unquoted-property-names (Warning)] Property names that are valid identifiers should be declared without quotation marks and accessed using dot notation. (CodeDescription: bicep core(https://aka.ms/bicep/linter/prefer-unquoted-property-names)) |['properties']|
  case28: foos[0]['properties']
//@[10:31) [BCP182 (Error)] This expression is being used in the for-body of the variable "varForBodyInvalidRuntimeUsages", which requires values that can be calculated at the start of the deployment. The property "properties" of foos cannot be calculated at the start. Properties of foos which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |foos[0]['properties']|
//@[17:31) [prefer-unquoted-property-names (Warning)] Property names that are valid identifiers should be declared without quotation marks and accessed using dot notation. (CodeDescription: bicep core(https://aka.ms/bicep/linter/prefer-unquoted-property-names)) |['properties']|
  case29: foos[i]['properties']
//@[10:31) [BCP182 (Error)] This expression is being used in the for-body of the variable "varForBodyInvalidRuntimeUsages", which requires values that can be calculated at the start of the deployment. The property "properties" of foos cannot be calculated at the start. Properties of foos which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |foos[i]['properties']|
//@[17:31) [prefer-unquoted-property-names (Warning)] Property names that are valid identifiers should be declared without quotation marks and accessed using dot notation. (CodeDescription: bicep core(https://aka.ms/bicep/linter/prefer-unquoted-property-names)) |['properties']|
  case30: foos[i + 2]['properties']
//@[10:35) [BCP182 (Error)] This expression is being used in the for-body of the variable "varForBodyInvalidRuntimeUsages", which requires values that can be calculated at the start of the deployment. The property "properties" of foos cannot be calculated at the start. Properties of foos which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |foos[i + 2]['properties']|
//@[21:35) [prefer-unquoted-property-names (Warning)] Property names that are valid identifiers should be declared without quotation marks and accessed using dot notation. (CodeDescription: bicep core(https://aka.ms/bicep/linter/prefer-unquoted-property-names)) |['properties']|
  case31: foos[zeroIndex]['properties']
//@[10:39) [BCP182 (Error)] This expression is being used in the for-body of the variable "varForBodyInvalidRuntimeUsages", which requires values that can be calculated at the start of the deployment. The property "properties" of foos cannot be calculated at the start. Properties of foos which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |foos[zeroIndex]['properties']|
//@[25:39) [prefer-unquoted-property-names (Warning)] Property names that are valid identifiers should be declared without quotation marks and accessed using dot notation. (CodeDescription: bicep core(https://aka.ms/bicep/linter/prefer-unquoted-property-names)) |['properties']|
  case32: foos[otherIndex]['properties']
//@[10:40) [BCP182 (Error)] This expression is being used in the for-body of the variable "varForBodyInvalidRuntimeUsages", which requires values that can be calculated at the start of the deployment. The property "properties" of foos cannot be calculated at the start. Properties of foos which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |foos[otherIndex]['properties']|
//@[26:40) [prefer-unquoted-property-names (Warning)] Property names that are valid identifiers should be declared without quotation marks and accessed using dot notation. (CodeDescription: bicep core(https://aka.ms/bicep/linter/prefer-unquoted-property-names)) |['properties']|
  case33: foo['properties']['accessTier']
//@[10:27) [BCP182 (Error)] This expression is being used in the for-body of the variable "varForBodyInvalidRuntimeUsages", which requires values that can be calculated at the start of the deployment. The property "properties" of foo cannot be calculated at the start. Properties of foo which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |foo['properties']|
//@[13:27) [prefer-unquoted-property-names (Warning)] Property names that are valid identifiers should be declared without quotation marks and accessed using dot notation. (CodeDescription: bicep core(https://aka.ms/bicep/linter/prefer-unquoted-property-names)) |['properties']|
//@[27:41) [prefer-unquoted-property-names (Warning)] Property names that are valid identifiers should be declared without quotation marks and accessed using dot notation. (CodeDescription: bicep core(https://aka.ms/bicep/linter/prefer-unquoted-property-names)) |['accessTier']|
  case34: existingFoo['properties']['accessTier']
//@[10:35) [BCP182 (Error)] This expression is being used in the for-body of the variable "varForBodyInvalidRuntimeUsages", which requires values that can be calculated at the start of the deployment. The property "properties" of existingFoo cannot be calculated at the start. Properties of existingFoo which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |existingFoo['properties']|
//@[21:35) [prefer-unquoted-property-names (Warning)] Property names that are valid identifiers should be declared without quotation marks and accessed using dot notation. (CodeDescription: bicep core(https://aka.ms/bicep/linter/prefer-unquoted-property-names)) |['properties']|
//@[35:49) [prefer-unquoted-property-names (Warning)] Property names that are valid identifiers should be declared without quotation marks and accessed using dot notation. (CodeDescription: bicep core(https://aka.ms/bicep/linter/prefer-unquoted-property-names)) |['accessTier']|
  case35: foo::fooChild['properties']['accessTier']
//@[10:37) [BCP182 (Error)] This expression is being used in the for-body of the variable "varForBodyInvalidRuntimeUsages", which requires values that can be calculated at the start of the deployment. The property "properties" of fooChild cannot be calculated at the start. Properties of fooChild which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |foo::fooChild['properties']|
//@[23:37) [prefer-unquoted-property-names (Warning)] Property names that are valid identifiers should be declared without quotation marks and accessed using dot notation. (CodeDescription: bicep core(https://aka.ms/bicep/linter/prefer-unquoted-property-names)) |['properties']|
//@[37:51) [prefer-unquoted-property-names (Warning)] Property names that are valid identifiers should be declared without quotation marks and accessed using dot notation. (CodeDescription: bicep core(https://aka.ms/bicep/linter/prefer-unquoted-property-names)) |['accessTier']|
//@[38:50) [BCP053 (Warning)] The type "FileServicePropertiesProperties" does not contain property "accessTier". Available properties include "cors", "protocolSettings", "shareDeleteRetentionPolicy". (CodeDescription: none) |'accessTier'|
  case36: foos[0]['properties']['accessTier']
//@[10:31) [BCP182 (Error)] This expression is being used in the for-body of the variable "varForBodyInvalidRuntimeUsages", which requires values that can be calculated at the start of the deployment. The property "properties" of foos cannot be calculated at the start. Properties of foos which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |foos[0]['properties']|
//@[17:31) [prefer-unquoted-property-names (Warning)] Property names that are valid identifiers should be declared without quotation marks and accessed using dot notation. (CodeDescription: bicep core(https://aka.ms/bicep/linter/prefer-unquoted-property-names)) |['properties']|
//@[31:45) [prefer-unquoted-property-names (Warning)] Property names that are valid identifiers should be declared without quotation marks and accessed using dot notation. (CodeDescription: bicep core(https://aka.ms/bicep/linter/prefer-unquoted-property-names)) |['accessTier']|
  case37: foos[i]['properties']['accessTier']
//@[10:31) [BCP182 (Error)] This expression is being used in the for-body of the variable "varForBodyInvalidRuntimeUsages", which requires values that can be calculated at the start of the deployment. The property "properties" of foos cannot be calculated at the start. Properties of foos which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |foos[i]['properties']|
//@[17:31) [prefer-unquoted-property-names (Warning)] Property names that are valid identifiers should be declared without quotation marks and accessed using dot notation. (CodeDescription: bicep core(https://aka.ms/bicep/linter/prefer-unquoted-property-names)) |['properties']|
//@[31:45) [prefer-unquoted-property-names (Warning)] Property names that are valid identifiers should be declared without quotation marks and accessed using dot notation. (CodeDescription: bicep core(https://aka.ms/bicep/linter/prefer-unquoted-property-names)) |['accessTier']|
  case38: foos[i + 2]['properties']['accessTier']
//@[10:35) [BCP182 (Error)] This expression is being used in the for-body of the variable "varForBodyInvalidRuntimeUsages", which requires values that can be calculated at the start of the deployment. The property "properties" of foos cannot be calculated at the start. Properties of foos which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |foos[i + 2]['properties']|
//@[21:35) [prefer-unquoted-property-names (Warning)] Property names that are valid identifiers should be declared without quotation marks and accessed using dot notation. (CodeDescription: bicep core(https://aka.ms/bicep/linter/prefer-unquoted-property-names)) |['properties']|
//@[35:49) [prefer-unquoted-property-names (Warning)] Property names that are valid identifiers should be declared without quotation marks and accessed using dot notation. (CodeDescription: bicep core(https://aka.ms/bicep/linter/prefer-unquoted-property-names)) |['accessTier']|
  case39: foos[zeroIndex]['properties']['accessTier']
//@[10:39) [BCP182 (Error)] This expression is being used in the for-body of the variable "varForBodyInvalidRuntimeUsages", which requires values that can be calculated at the start of the deployment. The property "properties" of foos cannot be calculated at the start. Properties of foos which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |foos[zeroIndex]['properties']|
//@[25:39) [prefer-unquoted-property-names (Warning)] Property names that are valid identifiers should be declared without quotation marks and accessed using dot notation. (CodeDescription: bicep core(https://aka.ms/bicep/linter/prefer-unquoted-property-names)) |['properties']|
//@[39:53) [prefer-unquoted-property-names (Warning)] Property names that are valid identifiers should be declared without quotation marks and accessed using dot notation. (CodeDescription: bicep core(https://aka.ms/bicep/linter/prefer-unquoted-property-names)) |['accessTier']|
  case40: foos[otherIndex]['properties']['accessTier']
//@[10:40) [BCP182 (Error)] This expression is being used in the for-body of the variable "varForBodyInvalidRuntimeUsages", which requires values that can be calculated at the start of the deployment. The property "properties" of foos cannot be calculated at the start. Properties of foos which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |foos[otherIndex]['properties']|
//@[26:40) [prefer-unquoted-property-names (Warning)] Property names that are valid identifiers should be declared without quotation marks and accessed using dot notation. (CodeDescription: bicep core(https://aka.ms/bicep/linter/prefer-unquoted-property-names)) |['properties']|
//@[40:54) [prefer-unquoted-property-names (Warning)] Property names that are valid identifiers should be declared without quotation marks and accessed using dot notation. (CodeDescription: bicep core(https://aka.ms/bicep/linter/prefer-unquoted-property-names)) |['accessTier']|
  case41: foo[propertiesAccessor]
//@[10:33) [BCP182 (Error)] This expression is being used in the for-body of the variable "varForBodyInvalidRuntimeUsages", which requires values that can be calculated at the start of the deployment. The property "properties" of foo cannot be calculated at the start. Properties of foo which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |foo[propertiesAccessor]|
  case42: existingFoo[propertiesAccessor]
//@[10:41) [BCP182 (Error)] This expression is being used in the for-body of the variable "varForBodyInvalidRuntimeUsages", which requires values that can be calculated at the start of the deployment. The property "properties" of existingFoo cannot be calculated at the start. Properties of existingFoo which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |existingFoo[propertiesAccessor]|
  case43: foo::fooChild[propertiesAccessor]
//@[10:43) [BCP182 (Error)] This expression is being used in the for-body of the variable "varForBodyInvalidRuntimeUsages", which requires values that can be calculated at the start of the deployment. The property "properties" of fooChild cannot be calculated at the start. Properties of fooChild which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |foo::fooChild[propertiesAccessor]|
  case44: foos[0][propertiesAccessor]
//@[10:37) [BCP182 (Error)] This expression is being used in the for-body of the variable "varForBodyInvalidRuntimeUsages", which requires values that can be calculated at the start of the deployment. The property "properties" of foos cannot be calculated at the start. Properties of foos which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |foos[0][propertiesAccessor]|
  case45: foos[i][propertiesAccessor]
//@[10:37) [BCP182 (Error)] This expression is being used in the for-body of the variable "varForBodyInvalidRuntimeUsages", which requires values that can be calculated at the start of the deployment. The property "properties" of foos cannot be calculated at the start. Properties of foos which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |foos[i][propertiesAccessor]|
  case46: foos[i + 2][propertiesAccessor]
//@[10:41) [BCP182 (Error)] This expression is being used in the for-body of the variable "varForBodyInvalidRuntimeUsages", which requires values that can be calculated at the start of the deployment. The property "properties" of foos cannot be calculated at the start. Properties of foos which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |foos[i + 2][propertiesAccessor]|
  case47: foos[zeroIndex][propertiesAccessor]
//@[10:45) [BCP182 (Error)] This expression is being used in the for-body of the variable "varForBodyInvalidRuntimeUsages", which requires values that can be calculated at the start of the deployment. The property "properties" of foos cannot be calculated at the start. Properties of foos which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |foos[zeroIndex][propertiesAccessor]|
  case48: foos[otherIndex][propertiesAccessor]
//@[10:46) [BCP182 (Error)] This expression is being used in the for-body of the variable "varForBodyInvalidRuntimeUsages", which requires values that can be calculated at the start of the deployment. The property "properties" of foos cannot be calculated at the start. Properties of foos which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |foos[otherIndex][propertiesAccessor]|
  case49: foo[propertiesAccessor][accessTierAccessor]
//@[10:33) [BCP182 (Error)] This expression is being used in the for-body of the variable "varForBodyInvalidRuntimeUsages", which requires values that can be calculated at the start of the deployment. The property "properties" of foo cannot be calculated at the start. Properties of foo which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |foo[propertiesAccessor]|
  case50: existingFoo[propertiesAccessor][accessTierAccessor]
//@[10:41) [BCP182 (Error)] This expression is being used in the for-body of the variable "varForBodyInvalidRuntimeUsages", which requires values that can be calculated at the start of the deployment. The property "properties" of existingFoo cannot be calculated at the start. Properties of existingFoo which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |existingFoo[propertiesAccessor]|
  case51: foo::fooChild[propertiesAccessor][accessTierAccessor]
//@[10:43) [BCP182 (Error)] This expression is being used in the for-body of the variable "varForBodyInvalidRuntimeUsages", which requires values that can be calculated at the start of the deployment. The property "properties" of fooChild cannot be calculated at the start. Properties of fooChild which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |foo::fooChild[propertiesAccessor]|
//@[44:62) [BCP053 (Warning)] The type "FileServicePropertiesProperties" does not contain property "accessTier". Available properties include "cors", "protocolSettings", "shareDeleteRetentionPolicy". (CodeDescription: none) |accessTierAccessor|
  case52: foos[0][propertiesAccessor][accessTierAccessor]
//@[10:37) [BCP182 (Error)] This expression is being used in the for-body of the variable "varForBodyInvalidRuntimeUsages", which requires values that can be calculated at the start of the deployment. The property "properties" of foos cannot be calculated at the start. Properties of foos which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |foos[0][propertiesAccessor]|
  case53: foos[i][propertiesAccessor][accessTierAccessor]
//@[10:37) [BCP182 (Error)] This expression is being used in the for-body of the variable "varForBodyInvalidRuntimeUsages", which requires values that can be calculated at the start of the deployment. The property "properties" of foos cannot be calculated at the start. Properties of foos which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |foos[i][propertiesAccessor]|
  case54: foos[i + 2][propertiesAccessor][accessTierAccessor]
//@[10:41) [BCP182 (Error)] This expression is being used in the for-body of the variable "varForBodyInvalidRuntimeUsages", which requires values that can be calculated at the start of the deployment. The property "properties" of foos cannot be calculated at the start. Properties of foos which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |foos[i + 2][propertiesAccessor]|
  case55: foos[zeroIndex][propertiesAccessor][accessTierAccessor]
//@[10:45) [BCP182 (Error)] This expression is being used in the for-body of the variable "varForBodyInvalidRuntimeUsages", which requires values that can be calculated at the start of the deployment. The property "properties" of foos cannot be calculated at the start. Properties of foos which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |foos[zeroIndex][propertiesAccessor]|
  case56: foos[otherIndex][propertiesAccessor][accessTierAccessor]
//@[10:46) [BCP182 (Error)] This expression is being used in the for-body of the variable "varForBodyInvalidRuntimeUsages", which requires values that can be calculated at the start of the deployment. The property "properties" of foos cannot be calculated at the start. Properties of foos which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |foos[otherIndex][propertiesAccessor]|
  case57: foo[strParam]
//@[10:23) [BCP182 (Error)] This expression is being used in the for-body of the variable "varForBodyInvalidRuntimeUsages", which requires values that can be calculated at the start of the deployment. Properties of foo which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |foo[strParam]|
  case58: existingFoo[strParam]
//@[10:31) [BCP182 (Error)] This expression is being used in the for-body of the variable "varForBodyInvalidRuntimeUsages", which requires values that can be calculated at the start of the deployment. Properties of existingFoo which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |existingFoo[strParam]|
  case59: foo::fooChild[strParam]
//@[10:33) [BCP182 (Error)] This expression is being used in the for-body of the variable "varForBodyInvalidRuntimeUsages", which requires values that can be calculated at the start of the deployment. Properties of fooChild which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |foo::fooChild[strParam]|
  case60: foos[0][strParam]
//@[10:27) [BCP182 (Error)] This expression is being used in the for-body of the variable "varForBodyInvalidRuntimeUsages", which requires values that can be calculated at the start of the deployment. Properties of foos which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |foos[0][strParam]|
  case61: foos[i][strParam]
//@[10:27) [BCP182 (Error)] This expression is being used in the for-body of the variable "varForBodyInvalidRuntimeUsages", which requires values that can be calculated at the start of the deployment. Properties of foos which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |foos[i][strParam]|
  case62: foos[i + 2][strParam]
//@[10:31) [BCP182 (Error)] This expression is being used in the for-body of the variable "varForBodyInvalidRuntimeUsages", which requires values that can be calculated at the start of the deployment. Properties of foos which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |foos[i + 2][strParam]|
  case63: foos[zeroIndex][strParam]
//@[10:35) [BCP182 (Error)] This expression is being used in the for-body of the variable "varForBodyInvalidRuntimeUsages", which requires values that can be calculated at the start of the deployment. Properties of foos which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |foos[zeroIndex][strParam]|
  case64: foos[otherIndex][strParam]
//@[10:36) [BCP182 (Error)] This expression is being used in the for-body of the variable "varForBodyInvalidRuntimeUsages", which requires values that can be calculated at the start of the deployment. Properties of foos which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |foos[otherIndex][strParam]|
  case65: foo['${strParam}']
//@[10:28) [BCP182 (Error)] This expression is being used in the for-body of the variable "varForBodyInvalidRuntimeUsages", which requires values that can be calculated at the start of the deployment. Properties of foo which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |foo['${strParam}']|
  case66: existingFoo['${strParam}']
//@[10:36) [BCP182 (Error)] This expression is being used in the for-body of the variable "varForBodyInvalidRuntimeUsages", which requires values that can be calculated at the start of the deployment. Properties of existingFoo which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |existingFoo['${strParam}']|
  case67: foo::fooChild['${strParam}']
//@[10:38) [BCP182 (Error)] This expression is being used in the for-body of the variable "varForBodyInvalidRuntimeUsages", which requires values that can be calculated at the start of the deployment. Properties of fooChild which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |foo::fooChild['${strParam}']|
  case68: foos[0]['${strParam}']
//@[10:32) [BCP182 (Error)] This expression is being used in the for-body of the variable "varForBodyInvalidRuntimeUsages", which requires values that can be calculated at the start of the deployment. Properties of foos which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |foos[0]['${strParam}']|
  case69: foos[i]['${strParam}']
//@[10:32) [BCP182 (Error)] This expression is being used in the for-body of the variable "varForBodyInvalidRuntimeUsages", which requires values that can be calculated at the start of the deployment. Properties of foos which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |foos[i]['${strParam}']|
  case70: foos[i + 2]['${strParam}']
//@[10:36) [BCP182 (Error)] This expression is being used in the for-body of the variable "varForBodyInvalidRuntimeUsages", which requires values that can be calculated at the start of the deployment. Properties of foos which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |foos[i + 2]['${strParam}']|
  case71: foos[zeroIndex]['${strParam}']
//@[10:40) [BCP182 (Error)] This expression is being used in the for-body of the variable "varForBodyInvalidRuntimeUsages", which requires values that can be calculated at the start of the deployment. Properties of foos which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |foos[zeroIndex]['${strParam}']|
  case72: foos[otherIndex]['${strParam}']
//@[10:41) [BCP182 (Error)] This expression is being used in the for-body of the variable "varForBodyInvalidRuntimeUsages", which requires values that can be calculated at the start of the deployment. Properties of foos which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |foos[otherIndex]['${strParam}']|
  case73: foo['i${strParam2}']
//@[10:30) [BCP182 (Error)] This expression is being used in the for-body of the variable "varForBodyInvalidRuntimeUsages", which requires values that can be calculated at the start of the deployment. Properties of foo which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |foo['i${strParam2}']|
  case74: existingFoo['i${strParam2}']
//@[10:38) [BCP182 (Error)] This expression is being used in the for-body of the variable "varForBodyInvalidRuntimeUsages", which requires values that can be calculated at the start of the deployment. Properties of existingFoo which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |existingFoo['i${strParam2}']|
  case75: foo::fooChild['i${strParam2}']
//@[10:40) [BCP182 (Error)] This expression is being used in the for-body of the variable "varForBodyInvalidRuntimeUsages", which requires values that can be calculated at the start of the deployment. Properties of fooChild which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |foo::fooChild['i${strParam2}']|
  case76: foos[0]['i${strParam2}']
//@[10:34) [BCP182 (Error)] This expression is being used in the for-body of the variable "varForBodyInvalidRuntimeUsages", which requires values that can be calculated at the start of the deployment. Properties of foos which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |foos[0]['i${strParam2}']|
  case77: foos[i]['i${strParam2}']
//@[10:34) [BCP182 (Error)] This expression is being used in the for-body of the variable "varForBodyInvalidRuntimeUsages", which requires values that can be calculated at the start of the deployment. Properties of foos which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |foos[i]['i${strParam2}']|
  case78: foos[i + 2]['i${strParam2}']
//@[10:38) [BCP182 (Error)] This expression is being used in the for-body of the variable "varForBodyInvalidRuntimeUsages", which requires values that can be calculated at the start of the deployment. Properties of foos which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |foos[i + 2]['i${strParam2}']|
  case79: foos[zeroIndex]['i${strParam2}']
//@[10:42) [BCP182 (Error)] This expression is being used in the for-body of the variable "varForBodyInvalidRuntimeUsages", which requires values that can be calculated at the start of the deployment. Properties of foos which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |foos[zeroIndex]['i${strParam2}']|
  case80: foos[otherIndex]['i${strParam2}']
//@[10:43) [BCP182 (Error)] This expression is being used in the for-body of the variable "varForBodyInvalidRuntimeUsages", which requires values that can be calculated at the start of the deployment. Properties of foos which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |foos[otherIndex]['i${strParam2}']|
  case81: foo[strArray[1]]
//@[10:26) [BCP182 (Error)] This expression is being used in the for-body of the variable "varForBodyInvalidRuntimeUsages", which requires values that can be calculated at the start of the deployment. The property "properties" of foo cannot be calculated at the start. Properties of foo which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |foo[strArray[1]]|
  case82: existingFoo[strArray[1]]
//@[10:34) [BCP182 (Error)] This expression is being used in the for-body of the variable "varForBodyInvalidRuntimeUsages", which requires values that can be calculated at the start of the deployment. The property "properties" of existingFoo cannot be calculated at the start. Properties of existingFoo which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |existingFoo[strArray[1]]|
  case83: foo::fooChild[strArray[1]]
//@[10:36) [BCP182 (Error)] This expression is being used in the for-body of the variable "varForBodyInvalidRuntimeUsages", which requires values that can be calculated at the start of the deployment. The property "properties" of fooChild cannot be calculated at the start. Properties of fooChild which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |foo::fooChild[strArray[1]]|
  case84: foos[0][strArray[1]]
//@[10:30) [BCP182 (Error)] This expression is being used in the for-body of the variable "varForBodyInvalidRuntimeUsages", which requires values that can be calculated at the start of the deployment. The property "properties" of foos cannot be calculated at the start. Properties of foos which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |foos[0][strArray[1]]|
  case85: foos[i][strArray[1]]
//@[10:30) [BCP182 (Error)] This expression is being used in the for-body of the variable "varForBodyInvalidRuntimeUsages", which requires values that can be calculated at the start of the deployment. The property "properties" of foos cannot be calculated at the start. Properties of foos which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |foos[i][strArray[1]]|
  case86: foos[i + 2][strArray[1]]
//@[10:34) [BCP182 (Error)] This expression is being used in the for-body of the variable "varForBodyInvalidRuntimeUsages", which requires values that can be calculated at the start of the deployment. The property "properties" of foos cannot be calculated at the start. Properties of foos which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |foos[i + 2][strArray[1]]|
  case87: foos[zeroIndex][strArray[1]]
//@[10:38) [BCP182 (Error)] This expression is being used in the for-body of the variable "varForBodyInvalidRuntimeUsages", which requires values that can be calculated at the start of the deployment. The property "properties" of foos cannot be calculated at the start. Properties of foos which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |foos[zeroIndex][strArray[1]]|
  case88: foos[otherIndex][strArray[1]]
//@[10:39) [BCP182 (Error)] This expression is being used in the for-body of the variable "varForBodyInvalidRuntimeUsages", which requires values that can be calculated at the start of the deployment. The property "properties" of foos cannot be calculated at the start. Properties of foos which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |foos[otherIndex][strArray[1]]|
  case89: foo[last(strArray)]
//@[10:29) [BCP182 (Error)] This expression is being used in the for-body of the variable "varForBodyInvalidRuntimeUsages", which requires values that can be calculated at the start of the deployment. The property "properties" of foo cannot be calculated at the start. Properties of foo which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |foo[last(strArray)]|
  case90: existingFoo[last(strArray)]
//@[10:37) [BCP182 (Error)] This expression is being used in the for-body of the variable "varForBodyInvalidRuntimeUsages", which requires values that can be calculated at the start of the deployment. The property "properties" of existingFoo cannot be calculated at the start. Properties of existingFoo which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |existingFoo[last(strArray)]|
  case91: foo::fooChild[last(strArray)]
//@[10:39) [BCP182 (Error)] This expression is being used in the for-body of the variable "varForBodyInvalidRuntimeUsages", which requires values that can be calculated at the start of the deployment. The property "properties" of fooChild cannot be calculated at the start. Properties of fooChild which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |foo::fooChild[last(strArray)]|
  case92: foos[0][last(strArray)]
//@[10:33) [BCP182 (Error)] This expression is being used in the for-body of the variable "varForBodyInvalidRuntimeUsages", which requires values that can be calculated at the start of the deployment. The property "properties" of foos cannot be calculated at the start. Properties of foos which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |foos[0][last(strArray)]|
  case93: foos[i][last(strArray)]
//@[10:33) [BCP182 (Error)] This expression is being used in the for-body of the variable "varForBodyInvalidRuntimeUsages", which requires values that can be calculated at the start of the deployment. The property "properties" of foos cannot be calculated at the start. Properties of foos which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |foos[i][last(strArray)]|
  case94: foos[i + 2][last(strArray)]
//@[10:37) [BCP182 (Error)] This expression is being used in the for-body of the variable "varForBodyInvalidRuntimeUsages", which requires values that can be calculated at the start of the deployment. The property "properties" of foos cannot be calculated at the start. Properties of foos which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |foos[i + 2][last(strArray)]|
  case95: foos[zeroIndex][last(strArray)]
//@[10:41) [BCP182 (Error)] This expression is being used in the for-body of the variable "varForBodyInvalidRuntimeUsages", which requires values that can be calculated at the start of the deployment. The property "properties" of foos cannot be calculated at the start. Properties of foos which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |foos[zeroIndex][last(strArray)]|
  case96: foos[otherIndex][last(strArray)]
//@[10:42) [BCP182 (Error)] This expression is being used in the for-body of the variable "varForBodyInvalidRuntimeUsages", which requires values that can be calculated at the start of the deployment. The property "properties" of foos cannot be calculated at the start. Properties of foos which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |foos[otherIndex][last(strArray)]|
  case97: foo[cond ? 'id' : 'properties']
//@[10:41) [BCP182 (Error)] This expression is being used in the for-body of the variable "varForBodyInvalidRuntimeUsages", which requires values that can be calculated at the start of the deployment. The property "properties" of foo cannot be calculated at the start. Properties of foo which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |foo[cond ? 'id' : 'properties']|
  case98: existingFoo[cond ? 'id' : 'properties']
//@[10:49) [BCP182 (Error)] This expression is being used in the for-body of the variable "varForBodyInvalidRuntimeUsages", which requires values that can be calculated at the start of the deployment. The property "properties" of existingFoo cannot be calculated at the start. Properties of existingFoo which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |existingFoo[cond ? 'id' : 'properties']|
  case99: foo::fooChild[cond ? 'id' : 'properties']
//@[10:51) [BCP182 (Error)] This expression is being used in the for-body of the variable "varForBodyInvalidRuntimeUsages", which requires values that can be calculated at the start of the deployment. The property "properties" of fooChild cannot be calculated at the start. Properties of fooChild which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |foo::fooChild[cond ? 'id' : 'properties']|
  case100: foos[0][cond ? 'id' : 'properties']
//@[11:46) [BCP182 (Error)] This expression is being used in the for-body of the variable "varForBodyInvalidRuntimeUsages", which requires values that can be calculated at the start of the deployment. The property "properties" of foos cannot be calculated at the start. Properties of foos which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |foos[0][cond ? 'id' : 'properties']|
  case101: foos[i][cond ? 'id' : 'properties']
//@[11:46) [BCP182 (Error)] This expression is being used in the for-body of the variable "varForBodyInvalidRuntimeUsages", which requires values that can be calculated at the start of the deployment. The property "properties" of foos cannot be calculated at the start. Properties of foos which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |foos[i][cond ? 'id' : 'properties']|
  case102: foos[i + 2][cond ? 'id' : 'properties']
//@[11:50) [BCP182 (Error)] This expression is being used in the for-body of the variable "varForBodyInvalidRuntimeUsages", which requires values that can be calculated at the start of the deployment. The property "properties" of foos cannot be calculated at the start. Properties of foos which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |foos[i + 2][cond ? 'id' : 'properties']|
  case103: foos[zeroIndex][cond ? 'id' : 'properties']
//@[11:54) [BCP182 (Error)] This expression is being used in the for-body of the variable "varForBodyInvalidRuntimeUsages", which requires values that can be calculated at the start of the deployment. The property "properties" of foos cannot be calculated at the start. Properties of foos which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |foos[zeroIndex][cond ? 'id' : 'properties']|
  case104: foos[otherIndex][cond ? 'id' : 'properties']
//@[11:55) [BCP182 (Error)] This expression is being used in the for-body of the variable "varForBodyInvalidRuntimeUsages", which requires values that can be calculated at the start of the deployment. The property "properties" of foos cannot be calculated at the start. Properties of foos which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |foos[otherIndex][cond ? 'id' : 'properties']|
  case105: foo[cond ? 'id' : strParam]
//@[11:38) [BCP182 (Error)] This expression is being used in the for-body of the variable "varForBodyInvalidRuntimeUsages", which requires values that can be calculated at the start of the deployment. Properties of foo which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |foo[cond ? 'id' : strParam]|
  case106: existingFoo[cond ? 'id' : strParam]
//@[11:46) [BCP182 (Error)] This expression is being used in the for-body of the variable "varForBodyInvalidRuntimeUsages", which requires values that can be calculated at the start of the deployment. Properties of existingFoo which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |existingFoo[cond ? 'id' : strParam]|
  case107: foo::fooChild[cond ? 'id' : strParam]
//@[11:48) [BCP182 (Error)] This expression is being used in the for-body of the variable "varForBodyInvalidRuntimeUsages", which requires values that can be calculated at the start of the deployment. Properties of fooChild which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |foo::fooChild[cond ? 'id' : strParam]|
  case108: foos[0][cond ? 'id' : strParam]
//@[11:42) [BCP182 (Error)] This expression is being used in the for-body of the variable "varForBodyInvalidRuntimeUsages", which requires values that can be calculated at the start of the deployment. Properties of foos which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |foos[0][cond ? 'id' : strParam]|
  case109: foos[i][cond ? 'id' : strParam]
//@[11:42) [BCP182 (Error)] This expression is being used in the for-body of the variable "varForBodyInvalidRuntimeUsages", which requires values that can be calculated at the start of the deployment. Properties of foos which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |foos[i][cond ? 'id' : strParam]|
  case110: foos[i + 2][cond ? 'id' : strParam]
//@[11:46) [BCP182 (Error)] This expression is being used in the for-body of the variable "varForBodyInvalidRuntimeUsages", which requires values that can be calculated at the start of the deployment. Properties of foos which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |foos[i + 2][cond ? 'id' : strParam]|
  case111: foos[zeroIndex][cond ? 'id' : strParam]
//@[11:50) [BCP182 (Error)] This expression is being used in the for-body of the variable "varForBodyInvalidRuntimeUsages", which requires values that can be calculated at the start of the deployment. Properties of foos which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |foos[zeroIndex][cond ? 'id' : strParam]|
  case112: foos[otherIndex][cond ? 'id' : strParam]
//@[11:51) [BCP182 (Error)] This expression is being used in the for-body of the variable "varForBodyInvalidRuntimeUsages", which requires values that can be calculated at the start of the deployment. Properties of foos which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |foos[otherIndex][cond ? 'id' : strParam]|
  case113: foos[cond ? 0 : 1].properties
//@[11:40) [BCP182 (Error)] This expression is being used in the for-body of the variable "varForBodyInvalidRuntimeUsages", which requires values that can be calculated at the start of the deployment. The property "properties" of foos cannot be calculated at the start. Properties of foos which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |foos[cond ? 0 : 1].properties|
  case114: foo[any('id')]
//@[11:25) [BCP182 (Error)] This expression is being used in the for-body of the variable "varForBodyInvalidRuntimeUsages", which requires values that can be calculated at the start of the deployment. Properties of foo which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |foo[any('id')]|
  case115: foos[any(0)]
//@[11:15) [BCP182 (Error)] This expression is being used in the for-body of the variable "varForBodyInvalidRuntimeUsages", which requires values that can be calculated at the start of the deployment. Properties of foos which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |foos|
  case116: foos[cond ? i : 0]
//@[11:15) [BCP182 (Error)] This expression is being used in the for-body of the variable "varForBodyInvalidRuntimeUsages", which requires values that can be calculated at the start of the deployment. Properties of foos which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |foos|
  case117: foos[cond ? i : i - 1]
//@[11:15) [BCP182 (Error)] This expression is being used in the for-body of the variable "varForBodyInvalidRuntimeUsages", which requires values that can be calculated at the start of the deployment. Properties of foos which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |foos|
  case118: foos[cond ? i + 1 : i - 1]
//@[11:15) [BCP182 (Error)] This expression is being used in the for-body of the variable "varForBodyInvalidRuntimeUsages", which requires values that can be calculated at the start of the deployment. Properties of foos which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |foos|
  case119: foos[cond ? any(0) : i]
//@[11:15) [BCP182 (Error)] This expression is being used in the for-body of the variable "varForBodyInvalidRuntimeUsages", which requires values that can be calculated at the start of the deployment. Properties of foos which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |foos|
  case120: foos[cond ? i : first(intArray)]
//@[11:15) [BCP182 (Error)] This expression is being used in the for-body of the variable "varForBodyInvalidRuntimeUsages", which requires values that can be calculated at the start of the deployment. Properties of foos which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |foos|
  case121: foos[intParam]
//@[11:15) [BCP182 (Error)] This expression is being used in the for-body of the variable "varForBodyInvalidRuntimeUsages", which requires values that can be calculated at the start of the deployment. Properties of foos which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |foos|
}]
var varForBodyInvalidRuntimeUsageExpression = [for i in range(0, 2): foo.properties]
//@[04:43) [no-unused-vars (Warning)] Variable "varForBodyInvalidRuntimeUsageExpression" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |varForBodyInvalidRuntimeUsageExpression|
//@[69:83) [BCP182 (Error)] This expression is being used in the for-body of the variable "varForBodyInvalidRuntimeUsageExpression", which requires values that can be calculated at the start of the deployment. The property "properties" of foo cannot be calculated at the start. Properties of foo which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |foo.properties|
var varForBodyInvalidRuntimeUsageInterpolatedKey = [for i in range(0, 2): {
//@[04:48) [no-unused-vars (Warning)] Variable "varForBodyInvalidRuntimeUsageInterpolatedKey" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |varForBodyInvalidRuntimeUsageInterpolatedKey|
  '${foos[i].properties.accessTier}': 'accessTier'
//@[05:23) [BCP182 (Error)] This expression is being used in the for-body of the variable "varForBodyInvalidRuntimeUsageInterpolatedKey", which requires values that can be calculated at the start of the deployment. The property "properties" of foos cannot be calculated at the start. Properties of foos which can be calculated at the start include "apiVersion", "id", "name", "type". (CodeDescription: none) |foos[i].properties|
}]
