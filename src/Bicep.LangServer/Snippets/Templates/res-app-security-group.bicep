// Application Security Group
resource applicationSecurityGroup 'Microsoft.Network/applicationSecurityGroups@2020-11-01' = {
  name: ${1:'applicationSecurityGroup'}
  location: resourceGroup().location
}
