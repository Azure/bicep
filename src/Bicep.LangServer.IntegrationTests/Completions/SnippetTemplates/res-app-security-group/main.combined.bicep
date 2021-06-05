// $1 = applicationSecurityGroup
// $2 = 'name'

resource applicationSecurityGroup 'Microsoft.Network/applicationSecurityGroups@2020-11-01' = {
  name: 'name'
  location: resourceGroup().location
}
// Insert snippet here

