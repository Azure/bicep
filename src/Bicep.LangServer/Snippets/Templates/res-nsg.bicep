// Network Security Group
resource networkSecurityGroup 'Microsoft.Network/networkSecurityGroups@2019-11-01' = {
  name: ${1:'networkSecurityGroup'}
  location: resourceGroup().location
  properties: {
    securityRules: [
      {
        name: ${2:'nsgRule'}
        properties: {
          description: ${3:'description'}
          protocol: '${4|Tcp,Udp,*|}'
          sourcePortRange: ${5:'*'}
          destinationPortRange: ${6:'*'}
          sourceAddressPrefix: ${7:'*'}
          destinationAddressPrefix: ${8:'*'}
          access: '${9|Allow,Deny|}'
          priority: ${10:100}
          direction: '${11|Inbound,Outbound|}'
        }
      }
    ]
  }
}