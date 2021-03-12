resource applicationSecurityGroup1 'Microsoft.Network/applicationSecurityGroups@2019-11-01' = {
  name: '${1:applicationSecurityGroup1}'
  location: resourceGroup().location
  tags: {}
  properties: {}
}