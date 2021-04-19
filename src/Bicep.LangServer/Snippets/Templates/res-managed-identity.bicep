// Managed Identity (User Assigned)
resource ${1:'managedIdentity'} 'Microsoft.ManagedIdentity/userAssignedIdentities@2018-11-30' = {
  name: 'name'
  location: resourceGroup().location
}
