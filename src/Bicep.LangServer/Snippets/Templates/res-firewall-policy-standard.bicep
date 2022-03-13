// Azure Firewall Policy Standard SKU
resource /*${1:firewallPolicy}*/firewallPolicy 'Microsoft.Network/firewallPolicies@2021-05-01' = {
  name: /*${2:'name'}*/'name'
  location: /*${3:location}*/'location'
  properties: {
    sku: {
      tier: 'Standard'
    }
    dnsSettings: {
      enableProxy: /*${4|true,false|}*/true
    }
    threatIntelMode: /*'${5|Alert,Deny,Off|}'*/'Alert'
  }
}
