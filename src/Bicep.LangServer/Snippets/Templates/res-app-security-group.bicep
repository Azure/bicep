// Application Security Group
resource /*${1:applicationSecurityGroup}*/applicationSecurityGroup 'Microsoft.Network/applicationSecurityGroups@2020-11-01' = {
  name: /*${2:'name'}*/'name'
  location: location
}
