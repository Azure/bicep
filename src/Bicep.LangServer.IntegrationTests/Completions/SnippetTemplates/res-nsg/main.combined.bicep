// $1 = networkSecurityGroup
// $2 = 'name'
// $3 = location
// $4 = 'nsgRule'
// $5 = 'description'
// $6 = 'Tcp'
// $7 = '5'
// $8 = '*'
// $9 = '*'
// $10 = '*'
// $11 = 'Allow'
// $12 = 100
// $13 = 'Inbound'

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

