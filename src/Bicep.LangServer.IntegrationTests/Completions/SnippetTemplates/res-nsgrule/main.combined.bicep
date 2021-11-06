// $1 = 'networkSecurityGroup'
// $2 = networkSecurityGroupSecurityRule
// $3 = 'name'
// $4 = 'description'
// $5 = 'Tcp'
// $6 = '1026'
// $7 = '1067'
// $8 = '0.11.26.162'
// $9 = '248.233.26.131'
// $10 = 'Allow'
// $11 = 100
// $12 = 'Inbound'

resource networkSecurityGroup 'Microsoft.Network/networkSecurityGroups@2021-03-01' existing = {
  name: 'networkSecurityGroup'
}

resource networkSecurityGroupSecurityRule 'Microsoft.Network/networkSecurityGroups/securityRules@2021-03-01' = {
  parent: networkSecurityGroup
  name: 'name'
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

