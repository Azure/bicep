// $1 = 'sqlDatabase/import'
// $2 = sqlDatabaseImport
// $3 = StorageAccessKey
// $4 = 'storageKey'
// $5 = 'storageUri'
// $6 = 'administratorLogin'
// $7 = 'administratorLoginPassword'

param location string
//@[6:14) [no-unused-params (Warning)] Parameter "location" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |location|

resource sqlServerDatabase 'Microsoft.Sql/servers/databases@2014-04-01' = {
  name: 'sqlDatabase/import' 'sqlDatabase/Import'
//@[29:49) [BCP019 (Error)] Expected a new line character at this location. (CodeDescription: none) |'sqlDatabase/Import'|
}

resource StorageAccessKey sqlDatabaseImport 'Microsoft.Sql/servers/databases/extensions@2014-04-01' = {
//@[26:43) [BCP068 (Error)] Expected a resource type string. Specify a valid resource type of format "<types>@<apiVersion>". (CodeDescription: none) |sqlDatabaseImport|
//@[26:103) [BCP029 (Error)] The resource type is not valid. Specify a valid resource type of format "<types>@<apiVersion>". (CodeDescription: none) |sqlDatabaseImport 'Microsoft.Sql/servers/databases/extensions@2014-04-01' = {|
//@[103:103) [BCP018 (Error)] Expected the "=" character at this location. (CodeDescription: none) ||
  parent: sqlServerDatabase
//@[2:8) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |parent|
  name: 'import'
//@[2:6) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |name|
  properties: {
//@[2:12) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |properties|
    storageKeyType: ''storageKey'' 'StorageAccessKey'
//@[4:18) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |storageKeyType|
    storageKey: 'storageUri' 'storageKey'
//@[4:14) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |storageKey|
    storageUri: 'administratorLogin' 'storageUri'
//@[4:14) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |storageUri|
    administratorLogin: 'administratorLoginPassword' 'administratorLogin'
//@[4:22) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |administratorLogin|
    administratorLoginPassword:  'administratorLoginPassword'
//@[4:30) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |administratorLoginPassword|
    operationMode: 'Import'
//@[4:17) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |operationMode|
  }
//@[2:3) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |}|
}
//@[0:1) [BCP007 (Error)] This declaration type is not recognized. Specify a parameter, variable, resource, or output declaration. (CodeDescription: none) |}|
// Insert snippet here

