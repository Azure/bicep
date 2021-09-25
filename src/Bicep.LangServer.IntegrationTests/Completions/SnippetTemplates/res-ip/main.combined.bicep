// $1 = publicIPAddress
// $2 = 'name'
// $3 = 'dnsName'

resource publicIPAddress 'Microsoft.Network/publicIPAddresses@2019-11-01' = {
  name: 'name'
  location: resourceGroup().location
  properties: {
    publicIPAllocationMethod: 'Dynamic'
    dnsSettings: {
      domainNameLabel: 'dnsName'
    }
  }
}
// Insert snippet here

