// Application Security Group
resource ${1:'applicationSecurityGroup'} 'Microsoft.Network/applicationSecurityGroups@2020-11-01' = {
  name: 'name'
  location: resourceGroup().location
}
