// $1 = firewall
// $2 = 'name'
// $3 = location
// $4 = 'firewallPolicy.id'
// $5 = 'name'
// $6 = 'subnet.id'
// $7 = 'publicIPAddress.id'

param location string

resource firewall 'Microsoft.Network/azureFirewalls@2021-05-01' = {
  name: 'name'
  location: location
  properties: {
    sku: {
      name: 'AZFW_VNet'
      tier: 'Standard'
    }    
    firewallPolicy: {
      id: 'firewallPolicy.id'
    }
    ipConfigurations: [
      {
        name: 'name'
        properties: {
          subnet: {
            id: 'subnet.id'
          }
          publicIPAddress: {
            id: 'publicIPAddress.id'
          }
        }
      }
    ]
  }
}
// Insert snippet here

