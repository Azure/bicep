resource nsg 'Microsoft.Network/networkSecurityGroups@2020-06-01' = {
  name: 'exampleNsg'
  location: 'eastus'
  properties: {
    securityRules: [
      {
        name: 'SSH'
        properties: {
          access: 'Allow'
          description: 'SSH'
          destinationAddressPrefix: '*'
          destinationPortRange: '22'
          direction: 'Inbound'
          priority: 1000
          protocol: 'Tcp'
          sourceAddressPrefix: '20.49.199.4'
          sourcePortRange: '*'
        }
      }
    ]
  }
  tags: {
    TagA: 'Value A'
    TagB: 'Value B'
  }
}
