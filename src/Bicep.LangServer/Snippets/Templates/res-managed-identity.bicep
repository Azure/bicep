// Managed Identity (User Assigned)
resource managedIdentity 'Microsoft.ManagedIdentity/userAssignedIdentities@2018-11-30' = {
  name: ${1:managedIdentity}
  location: resourceGroup().location
}