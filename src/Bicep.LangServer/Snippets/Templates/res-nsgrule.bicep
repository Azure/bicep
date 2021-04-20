// Network Security Group Rule
resource ${1:'networkSecurityGroupSecurityRule'} 'Microsoft.Network/networkSecurityGroups/securityRules@2019-11-01' = {
  name: ${2:'networkSecurityGroup/name'}
  properties: {
    description: ${3:'description'}
    protocol: '${4|*,Ah,Esp,Icmp,Tcp,Udb|}'
    sourcePortRange: ${5:'sourcePortRange'}
    destinationPortRange: ${6:'destinationPortRange'}
    sourceAddressPrefix: ${7:'sourceAddressPrefix'}
    destinationAddressPrefix: ${8:'destinationAddressPrefix'}
    access: '${9|Allow,Deny|}'
    priority: ${10:100}
    direction: '${11|Inbound,Outbound|}'
  }
}
