// $1 = applicationSecurityGroup
// $2 = 'name'
// $3 = location

param location string

resource applicationSecurityGroup 'Microsoft.Network/applicationSecurityGroups@2020-11-01' = {
  name: 'name'
  location: location
}
// Insert snippet here

