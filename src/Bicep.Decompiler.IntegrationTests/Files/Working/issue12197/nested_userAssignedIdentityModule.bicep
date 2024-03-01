@description('Name for the User Assigned Identity')
param userIdentityName string

@description('Location for resource.')
param location string

resource ui_userIdentity 'Microsoft.ManagedIdentity/userAssignedIdentities@2023-01-31' = {
  name: toLower('ui-${userIdentityName}')
  location: location
}

output userIdentityPrincipalOutput string = reference(ui_userIdentity.id, '2023-01-31').principalId
//@[44:87) [use-resource-symbol-reference (Warning)] Use a resource reference instead of invoking function "reference". This simplifies the syntax and allows Bicep to better understand your deployment dependency graph. (CodeDescription: bicep core(https://aka.ms/bicep/linter/use-resource-symbol-reference)) |reference(ui_userIdentity.id, '2023-01-31')|
output userIdentityNameOutput string = toLower('ui-${userIdentityName}')
output userIdentityResrouceId string = ui_userIdentity.id

