// Managed Identity (User Assigned)
resource /*${1:managedIdentity}*/managedIdentity 'Microsoft.ManagedIdentity/userAssignedIdentities@2018-11-30' = {
  name: /*${2:'name'}*/'name'
  location: location
}
