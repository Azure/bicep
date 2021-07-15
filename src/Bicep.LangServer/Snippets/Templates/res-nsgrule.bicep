// Network Security Group Rule
resource /*${1:networkSecurityGroupSecurityRule}*/networkSecurityGroupSecurityRule 'Microsoft.Network/networkSecurityGroups/securityRules@2019-11-01' = {
  name: /*${2:'networkSecurityGroup/name'}*/'networkSecurityGroup/name'
  properties: {
    description: /*${3:'description'}*/'description'
    protocol: /*${4|'*','Ah','Esp','Icmp','Tcp','Udb'|}*/'*'
    sourcePortRange: /*${5:'sourcePortRange'}*/'sourcePortRange'
    destinationPortRange: /*${6:'destinationPortRange'}*/'destinationPortRange'
    sourceAddressPrefix: /*${7:'sourceAddressPrefix'}*/'sourceAddressPrefix'
    destinationAddressPrefix: /*${8:'destinationAddressPrefix'}*/'destinationAddressPrefix'
    access: /*${9|'Allow','Deny'|}*/'Allow'
    priority: /*${10:100}*/100
    direction: /*${11|'Inbound','Outbound'|}*/'Inbound'
  }
}
