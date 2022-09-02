// $1 = firewall
// $2 = 'name'
// $3 = location
// $4 = 'firewallPolicy.id'
// $5 = 'virtualHub.id'
// $6 = 2


param location string

resource firewall 'Microsoft.Network/azureFirewalls@2021-05-01' = {
  name: 'name'
  location: location
  properties: {
    sku: {
      name: 'AZFW_Hub'
      tier: 'Standard'
    }    
    firewallPolicy: {
      id: 'firewallPolicy.id'
    }
    virtualHub: {
      id: 'virtualHub.id'
    }
    hubIPAddresses: {
       publicIPs: {
         count: 2
       }
    }
  }
}
// Insert snippet here

