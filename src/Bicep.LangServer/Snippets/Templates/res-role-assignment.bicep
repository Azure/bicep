resource /*${1:roleAssignment}*/roleAssignment 'Microsoft.Authorization/roleAssignments@2020-10-01-preview' = {
  name: /*${2:'name'}*/'name'
  properties: {
    roleDefinitionId: /*${3:'roleDefinitionId'}*/'roleDefinitionId'
    principalId: /*${4:'principalId'}*/'principalId'
    principalType: /*${5|'ServicePrincipal','Group','User'|}*/'ServicePrincipal'
  }
}
