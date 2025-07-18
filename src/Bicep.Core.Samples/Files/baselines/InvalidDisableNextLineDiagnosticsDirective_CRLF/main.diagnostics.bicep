#disable-next-line
//@[18:18) [BCP226 (Error)] Expected at least one diagnostic code at this location. Valid format is "#disable-next-line diagnosticCode1 diagnosticCode2 ..." (bicep https://aka.ms/bicep/core-diagnostics#BCP226) ||
param storageAccount1 string = 'testStorageAccount'
//@[06:21) [no-unused-params (Warning)] Parameter "storageAccount1" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-params) |storageAccount1|

#  disable-next-line  no-unused-params
//@[00:01) [BCP001 (Error)] The following token is not recognized: "#". (bicep https://aka.ms/bicep/core-diagnostics#BCP001) |#|
//@[00:01) [BCP007 (Error)] This declaration type is not recognized. Specify a metadata, parameter, variable, resource, or output declaration. (bicep https://aka.ms/bicep/core-diagnostics#BCP007) |#|
param storageAccount2 string = 'testStorageAccount'
//@[06:21) [no-unused-params (Warning)] Parameter "storageAccount2" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-params) |storageAccount2|

/* comment before */#disable-next-line no-unused-params
//@[20:21) [BCP001 (Error)] The following token is not recognized: "#". (bicep https://aka.ms/bicep/core-diagnostics#BCP001) |#|
//@[20:21) [BCP007 (Error)] This declaration type is not recognized. Specify a metadata, parameter, variable, resource, or output declaration. (bicep https://aka.ms/bicep/core-diagnostics#BCP007) |#|
param storageAccount3 string = 'testStorageAccount'
//@[06:21) [no-unused-params (Warning)] Parameter "storageAccount3" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-params) |storageAccount3|

#disable-next-line/* comment between */ no-unused-params
//@[18:18) [BCP226 (Error)] Expected at least one diagnostic code at this location. Valid format is "#disable-next-line diagnosticCode1 diagnosticCode2 ..." (bicep https://aka.ms/bicep/core-diagnostics#BCP226) ||
//@[40:42) [BCP007 (Error)] This declaration type is not recognized. Specify a metadata, parameter, variable, resource, or output declaration. (bicep https://aka.ms/bicep/core-diagnostics#BCP007) |no|
param storageAccount4 string = 'testStorageAccount'
//@[06:21) [no-unused-params (Warning)] Parameter "storageAccount4" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-params) |storageAccount4|

//#disable-next-line no-unused-params
param storageAccount5 string = 'testStorageAccount'
//@[06:21) [no-unused-params (Warning)] Parameter "storageAccount5" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-params) |storageAccount5|

#disable-next-line 
//@[19:19) [BCP226 (Error)] Expected at least one diagnostic code at this location. Valid format is "#disable-next-line diagnosticCode1 diagnosticCode2 ..." (bicep https://aka.ms/bicep/core-diagnostics#BCP226) ||
no-unused-params
//@[00:02) [BCP007 (Error)] This declaration type is not recognized. Specify a metadata, parameter, variable, resource, or output declaration. (bicep https://aka.ms/bicep/core-diagnostics#BCP007) |no|
param storageAccount6 string = 'testStorageAccount'
//@[06:21) [no-unused-params (Warning)] Parameter "storageAccount6" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-params) |storageAccount6|

#madeup-directive
//@[00:01) [BCP001 (Error)] The following token is not recognized: "#". (bicep https://aka.ms/bicep/core-diagnostics#BCP001) |#|
//@[00:01) [BCP007 (Error)] This declaration type is not recognized. Specify a metadata, parameter, variable, resource, or output declaration. (bicep https://aka.ms/bicep/core-diagnostics#BCP007) |#|
param storageAccount7 string = 'testStorageAccount'
//@[06:21) [no-unused-params (Warning)] Parameter "storageAccount7" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-params) |storageAccount7|

var terminatedWithDirective = 'foo' #disable-next-line no-unused-params
//@[04:27) [no-unused-vars (Warning)] Variable "terminatedWithDirective" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |terminatedWithDirective|
//@[36:37) [BCP019 (Error)] Expected a new line character at this location. (bicep https://aka.ms/bicep/core-diagnostics#BCP019) |#|
//@[36:37) [BCP001 (Error)] The following token is not recognized: "#". (bicep https://aka.ms/bicep/core-diagnostics#BCP001) |#|
param storageAccount8 string = 'testStorageAccount'
//@[06:21) [no-unused-params (Warning)] Parameter "storageAccount8" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-params) |storageAccount8|
