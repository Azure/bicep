// Network Security Group Rule
resource networkSecurityGroupRuleName 'Microsoft.Network/networkSecurityGroups/securityRules@2019-11-01' = {
  name: ${1:'networkSecurityGroupRuleName'}
  properties: {
    description: ${2:'nsgRuleDescription'}
    protocol: '${3|*,Ah,Esp,Icmp,Tcp,Udb|}'
    sourcePortRange: ${4:'nsgRuleSourcePortRange'}
    destinationPortRange: ${5:'nsgRuleDestinationPortRange'}
    sourceAddressPrefix: ${6:'nsgRuleSourceAddressPrefix'}
    destinationAddressPrefix: ${7:'nsgRuleDestinationAddressRange'}
    access: '${8|Allow,Deny|}'
    priority: ${9:100}
    direction: '${10|Inbound,Outbound|}'
  }
}