// Azure Firewall Standard SKU
resource /*${1:firewall}*/firewall 'Microsoft.Network/azureFirewalls@2021-05-01' = {
  name: /*${2:'name'}*/'name'
  location: /*${3:location}*/'location'
  properties: {
    sku: {
      name: 'AZFW_VNet'
      tier: 'Standard'
    }    
    firewallPolicy: {
      id: /*${4:'firewallPolicy.id'}*/'firewallPolicy.id'
    }
    ipConfigurations: [
      {
        name: /*${5:'name'}*/'name'
        properties: {
          subnet: {
            id: /*${6:'subnet.id'}*/'subnet.id'
          }
          publicIPAddress: {
            id: /*${7:'publicIPAddress.id'}*/'publicIPAddress.id'
          }
        }
      }
    ]
  }
}
