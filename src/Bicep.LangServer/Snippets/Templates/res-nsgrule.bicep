// Network Security Group Rule
resource networkSecurityGroup 'Microsoft.Network/networkSecurityGroups@2021-03-01' existing = {
  name: /*${1:'name'}*/'name'
}

resource /*${2:networkSecurityGroupSecurityRule}*/networkSecurityGroupSecurityRule 'Microsoft.Network/networkSecurityGroups/securityRules@2021-03-01' = {
  parent: networkSecurityGroup
  name: /*${3:'name'}*/'name'
  properties: {
    description: /*${4:'description'}*/'description'
    protocol: /*${5|'*','Ah','Esp','Icmp','Tcp','Udb'|}*/'*'
    sourcePortRange: /*${6:'sourcePortRange'}*/'sourcePortRange'
    destinationPortRange: /*${7:'destinationPortRange'}*/'destinationPortRange'
    sourceAddressPrefix: /*${8:'sourceAddressPrefix'}*/'sourceAddressPrefix'
    destinationAddressPrefix: /*${9:'destinationAddressPrefix'}*/'destinationAddressPrefix'
    access: /*${10|'Allow','Deny'|}*/'Allow'
    priority: /*${11:100}*/100
    direction: /*${12|'Inbound','Outbound'|}*/'Inbound'
  }
}
