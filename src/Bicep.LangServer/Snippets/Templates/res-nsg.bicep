// Network Security Group
resource ${1:'networkSecurityGroup'} 'Microsoft.Network/networkSecurityGroups@2019-11-01' = {
  name: ${2:'name'}
  location: resourceGroup().location
  properties: {
    securityRules: [
      {
        name: ${3:'nsgRule'}
        properties: {
          description: ${4:'description'}
          protocol: ${5|'Tcp','Udp','*'|}
          sourcePortRange: ${6:'*'}
          destinationPortRange: ${7:'*'}
          sourceAddressPrefix: ${8:'*'}
          destinationAddressPrefix: ${9:'*'}
          access: ${10|'Allow','Deny'|}
          priority: ${11:100}
          direction: ${12|'Inbound','Outbound'|}
        }
      }
    ]
  }
}
