// Application Security Group
resource applicationSecurityGroup 'Microsoft.Network/applicationSecurityGroups@2019-11-01' = {
  name: '${1:applicationSecurityGroup}'
  location: resourceGroup().location
}