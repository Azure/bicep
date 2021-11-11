#disable-next-line
//@[18:18) [BCP226 (Error)] Expected at least one diagnostic code at this location. Valid format is "#disable-next-line diagnosticCode1 diagnosticCode2 ..." (CodeDescription: none) ||
param storageAccount1 string = 'testStorageAccount'
//@[6:21) [no-unused-params (Warning)] Parameter "storageAccount1" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |storageAccount1|
#  disable-next-line  no-unused-params
//@[0:1) [BCP001 (Error)] The following token is not recognized: "#". (CodeDescription: none) |#|
//@[0:1) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |#|
param storageAccount2 string = 'testStorageAccount'
//@[6:21) [no-unused-params (Warning)] Parameter "storageAccount2" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |storageAccount2|
