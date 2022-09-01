// $1 = resourceGroup
// $2 = 'name'
// $3 = location

targetScope = 'subscription'
param location string

resource resourceGroup 'Microsoft.Resources/resourceGroups@2021-04-01' = {
  name: 'name'
  location: location
}
// Insert snippet here

