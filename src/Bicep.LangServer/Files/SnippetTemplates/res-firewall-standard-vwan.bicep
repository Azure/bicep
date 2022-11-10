// Azure Firewall Standard SKU for Virtual WAN Hub
resource /*${1:firewall}*/firewall 'Microsoft.Network/azureFirewalls@2021-05-01' = {
  name: /*${2:'name'}*/'name'
  location: /*${3:location}*/'location'
  properties: {
    sku: {
      name: 'AZFW_Hub'
      tier: 'Standard'
    }    
    firewallPolicy: {
      id: /*${4:'firewallPolicy.id'}*/'firewallPolicy.id'
    }
    virtualHub: {
      id: /*${5:'virtualHub.id'}*/'virtualHub.id'
    }
    hubIPAddresses: {
       publicIPs: {
         count: /*${6:2}*/2
       }
    }
  }
}
