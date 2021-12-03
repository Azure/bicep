// $1 = networkSecurityGroup
// $2 = 'name'
// $3 = 'nsgRule'
// $4 = 'description'
// $5 = 'Tcp'
// $6 = '5'
// $7 = '*'
// $8 = '*'
// $9 = '*'
// $10 = 'Allow'
// $11 = 100
// $12 = 'Inbound'

param location string

resource networkSecurityGroup 'Microsoft.Network/networkSecurityGroups@2019-11-01' = {
  name: 'name'
  location: location
  properties: {
    securityRules: [
      {
        name: 'nsgRule'
        properties: {
          description: 'description'
          protocol: 'Tcp'
          sourcePortRange: '5'
          destinationPortRange: '*'
          sourceAddressPrefix: '*'
          destinationAddressPrefix: '*'
          access: 'Allow'
          priority: 100
          direction: 'Inbound'
        }
      }
    ]
  }
}
// Insert snippet here

