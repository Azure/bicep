resource networkSecurityGroup 'Microsoft.Network/networkSecurityGroups@2019-11-01' = {
  name: 'testNetworkSecurityGroup'
  location: resourceGroup().location
  properties: {
    securityRules: [
      {
        name: 'nsgRule'
        properties: {
          description: 'testDescription'
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
