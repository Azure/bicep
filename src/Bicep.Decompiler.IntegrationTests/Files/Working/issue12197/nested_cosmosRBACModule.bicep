param databaseAccountName string
param databaseAccountResourceGroup string
param roleDefinitionId string = '00000000-0000-0000-0000-000000000002'
param principalId string

@description('Data actions permitted by the Role Definition')
param dataActions array = [
//@[6:17) [no-unused-params (Warning)] Parameter "dataActions" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |dataActions|
  'Microsoft.DocumentDB/databaseAccounts/readMetadata'
  'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers/items/*'
]

@description('Friendly name for the SQL Role Definition')
param roleDefinitionName string = 'My Read Write Role'
//@[6:24) [no-unused-params (Warning)] Parameter "roleDefinitionName" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |roleDefinitionName|

var roleAssignmentId = guid(
  roleDefinitionId,
  principalId,
  resourceId('Microsoft.DocumentDB/databaseAccounts', databaseAccountName)
)
var roleDefinition = guid(
//@[4:18) [no-unused-vars (Warning)] Variable "roleDefinition" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |roleDefinition|
  'sql-role-definition-',
  principalId,
  resourceId('Microsoft.DocumentDB/databaseAccounts', databaseAccountName)
)

resource databaseAccountName_roleAssignmentId 'Microsoft.DocumentDB/databaseAccounts/sqlRoleAssignments@2023-04-15' = {
  name: '${databaseAccountName}/${roleAssignmentId}'
  properties: {
    principalId: principalId
    roleDefinitionId: '/${subscription().id}/resourceGroups/${databaseAccountResourceGroup}/providers/Microsoft.DocumentDB/databaseAccounts/${databaseAccountName}/sqlRoleDefinitions/${roleDefinitionId}'
//@[4:20) [use-resource-id-functions (Warning)] If property "roleDefinitionId" represents a resource ID, it must use a symbolic resource reference, be a parameter or start with one of these functions: extensionResourceId, guid, if, managementGroupResourceId, reference, resourceId, subscription, subscriptionResourceId, tenantResourceId. (CodeDescription: bicep core(https://aka.ms/bicep/linter/use-resource-id-functions)) |roleDefinitionId|
    scope: resourceId('Microsoft.DocumentDB/databaseAccounts', databaseAccountName)
  }
}

