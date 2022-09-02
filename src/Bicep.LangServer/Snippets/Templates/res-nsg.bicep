// Network Security Group
resource /*${1:networkSecurityGroup}*/networkSecurityGroup 'Microsoft.Network/networkSecurityGroups@2019-11-01' = {
  name: /*${2:'name'}*/'name'
  location: /*${3:location}*/'location'
  properties: {
    securityRules: [
      {
        name: /*${4:'nsgRule'}*/'nsgRule'
        properties: {
          description: /*${5:'description'}*/'description'
          protocol: /*${6|'Tcp','Udp','*'|}*/'Tcp'
          sourcePortRange: /*${7:'*'}*/'*'
          destinationPortRange: /*${8:'*'}*/'*'
          sourceAddressPrefix: /*${9:'*'}*/'*'
          destinationAddressPrefix: /*${10:'*'}*/'*'
          access: /*${11|'Allow','Deny'|}*/'Allow'
          priority: /*${12:100}*/100
          direction: /*${13|'Inbound','Outbound'|}*/'Inbound'
        }
      }
    ]
  }
}
