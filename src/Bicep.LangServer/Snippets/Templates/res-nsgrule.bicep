// Network Security Group Rule
resource ${1:'networkSecurityGroupSecurityRule'} 'Microsoft.Network/networkSecurityGroups/securityRules@2019-11-01' = {
  name: 'networkSecurityGroup/name'
  properties: {
    description: ${2:'description'}
    protocol: '${3|*,Ah,Esp,Icmp,Tcp,Udb|}'
    sourcePortRange: ${4:'sourcePortRange'}
    destinationPortRange: ${5:'destinationPortRange'}
    sourceAddressPrefix: ${6:'sourceAddressPrefix'}
    destinationAddressPrefix: ${7:'destinationAddressPrefix'}
    access: '${8|Allow,Deny|}'
    priority: ${9:100}
    direction: '${10|Inbound,Outbound|}'
  }
}
