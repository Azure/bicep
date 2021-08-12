// $1 = networkSecurityGroupSecurityRule
// $2 = 'networkSecurityGroup/name'
// $3 = 'description'
// $4 = 'Tcp'
// $5 = '1026'
// $6 = '1067'
// $7 = '0.11.26.162'
// $8 = '248.233.26.131'
// $9 = 'Allow'
// $10 = 100
// $11 = 'Inbound'

resource networkSecurityGroupSecurityRule 'Microsoft.Network/networkSecurityGroups/securityRules@2019-11-01' = {
  name: 'networkSecurityGroup/name'
  properties: {
    description: 'description'
    protocol: 'Tcp'
    sourcePortRange: '1026'
    destinationPortRange: '1067'
    sourceAddressPrefix: '0.11.26.162'
    destinationAddressPrefix: '248.233.26.131'
    access: 'Allow'
    priority: 100
    direction: 'Inbound'
  }
}
// Insert snippet here

