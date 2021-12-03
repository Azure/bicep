// $1 = publicIPAddress
// $2 = 'name'
// $3 = 'dnsName'

param location string

resource publicIPAddress 'Microsoft.Network/publicIPAddresses@2019-11-01' = {
  name: 'name'
  location: location
  properties: {
    publicIPAllocationMethod: 'Dynamic'
    dnsSettings: {
      domainNameLabel: 'dnsName'
    }
  }
}
// Insert snippet here

