param databaseAccountName string
param databaseAccountResourceGroup string
param roleDefinitionId string = '00000000-0000-0000-0000-000000000002'
param principalId string

@description('Data actions permitted by the Role Definition')
param dataActions array = [
//@[6:17) [no-unused-params (Warning)] Parameter "dataActions" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-params) |dataActions|
  'Microsoft.DocumentDB/databaseAccounts/readMetadata'
  'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers/items/*'
]

@description('Friendly name for the SQL Role Definition')
param roleDefinitionName string = 'My Read Write Role'
//@[6:24) [no-unused-params (Warning)] Parameter "roleDefinitionName" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-params) |roleDefinitionName|

var roleAssignmentId = guid(
  roleDefinitionId,
  principalId,
  resourceId('Microsoft.DocumentDB/databaseAccounts', databaseAccountName)
)
var roleDefinition = guid(
//@[4:18) [no-unused-vars (Warning)] Variable "roleDefinition" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |roleDefinition|
  'sql-role-definition-',
  principalId,
  resourceId('Microsoft.DocumentDB/databaseAccounts', databaseAccountName)
)

resource databaseAccountName_roleAssignmentId 'Microsoft.DocumentDB/databaseAccounts/sqlRoleAssignments@2023-04-15' = {
  name: '${databaseAccountName}/${roleAssignmentId}'
  properties: {
    principalId: principalId
    roleDefinitionId: '/${subscription().id}/resourceGroups/${databaseAccountResourceGroup}/providers/Microsoft.DocumentDB/databaseAccounts/${databaseAccountName}/sqlRoleDefinitions/${roleDefinitionId}'
    scope: resourceId('Microsoft.DocumentDB/databaseAccounts', databaseAccountName)
  }
}

