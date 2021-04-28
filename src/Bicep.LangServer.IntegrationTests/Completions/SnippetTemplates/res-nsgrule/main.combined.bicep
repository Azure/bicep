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

