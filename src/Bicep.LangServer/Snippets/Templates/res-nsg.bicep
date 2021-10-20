// Network Security Group
resource /*${1:networkSecurityGroup}*/networkSecurityGroup 'Microsoft.Network/networkSecurityGroups@2019-11-01' = {
  name: /*${2:'name'}*/'name'
  location: location
  properties: {
    securityRules: [
      {
        name: /*${3:'nsgRule'}*/'nsgRule'
        properties: {
          description: /*${4:'description'}*/'description'
          protocol: /*${5|'Tcp','Udp','*'|}*/'Tcp'
          sourcePortRange: /*${6:'*'}*/'*'
          destinationPortRange: /*${7:'*'}*/'*'
          sourceAddressPrefix: /*${8:'*'}*/'*'
          destinationAddressPrefix: /*${9:'*'}*/'*'
          access: /*${10|'Allow','Deny'|}*/'Allow'
          priority: /*${11:100}*/100
          direction: /*${12|'Inbound','Outbound'|}*/'Inbound'
        }
      }
    ]
  }
}
