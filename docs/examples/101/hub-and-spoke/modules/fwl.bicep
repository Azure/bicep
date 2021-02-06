param prefix string
param hubId string

resource publicIp 'Microsoft.Network/publicIPAddresses@2020-06-01' = {
  name: '${prefix}-fwl-ip'
  location: resourceGroup().location
  properties: {
    publicIPAddressVersion: 'IPv4'
    publicIPAllocationMethod: 'Static'
  }
  sku: {
    name: 'Standard'
  }
}

resource fwl 'Microsoft.Network/azureFirewalls@2020-06-01' = {
  name: '${prefix}-fwl'
  location: resourceGroup().location
  properties: {
    ipConfigurations: [
      {
        name: '${prefix}-fwl-ipconf'
        properties: {
          subnet: {
            id: '${hubId}/subnets/AzureFirewallSubnet'
          }
          publicIPAddress: {
            id: publicIp.id
          }
        }
      }
    ]
  }
}

output privateIp string = fwl.properties.ipConfigurations[0].properties.privateIPAddress
