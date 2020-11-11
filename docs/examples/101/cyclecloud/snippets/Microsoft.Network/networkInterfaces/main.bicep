resource nic 'Microsoft.Network/networkInterfaces@2020-05-01' = {
  name: 'exampleNic'
  location: 'eastus'
  properties: {
    dnsSettings: {}
    enableAcceleratedNetworking: false
    enableIPForwarding: false
    ipConfigurations: [
      {
        name: 'ipconfig'
        properties: {
          privateIPAllocationMethod: 'Dynamic'
          subnet: {
            id: 'Subnet Resource Id'
          }
          publicIPAddress: {
            id: 'PublicIPAddress Resource Id'
          }
        }
      }
    ]
    networkSecurityGroup: {
      id: 'NetworkSecurityGroup Resource Id'
    }
  }
  tags: {
    TagA: 'Value A'
    TagB: 'Value B'
  }
}
