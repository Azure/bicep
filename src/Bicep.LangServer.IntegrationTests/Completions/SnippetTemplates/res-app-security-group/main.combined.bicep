resource applicationSecurityGroup 'Microsoft.Network/applicationSecurityGroups@2020-11-01' = {
  name: 'testApplicationSecurityGroup'
  location: resourceGroup().location
}
