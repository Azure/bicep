resource applicationSecurityGroup 'Microsoft.Network/applicationSecurityGroups@2019-11-01' = {
  name: 'testApplicationSecurityGroup'
  location: resourceGroup().location
}
