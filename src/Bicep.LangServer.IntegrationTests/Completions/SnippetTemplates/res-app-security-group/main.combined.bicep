resource applicationSecurityGroup 'Microsoft.Network/applicationSecurityGroups@2020-11-01' = {
  name: 'applicationSecurityGroup'
  location: resourceGroup().location
}

