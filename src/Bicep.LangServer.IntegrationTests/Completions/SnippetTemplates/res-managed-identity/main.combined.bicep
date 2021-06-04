// $1 = managedIdentity
// $2 = 'name'

resource managedIdentity 'Microsoft.ManagedIdentity/userAssignedIdentities@2018-11-30' = {
  name: 'name'
  location: resourceGroup().location
}
// Insert snippet here

