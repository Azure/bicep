// $1 = managedIdentity
// $2 = 'name'

param location string

resource managedIdentity 'Microsoft.ManagedIdentity/userAssignedIdentities@2018-11-30' = {
  name: 'name'
  location: location
}
// Insert snippet here

