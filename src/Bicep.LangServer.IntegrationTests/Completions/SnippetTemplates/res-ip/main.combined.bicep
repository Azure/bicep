resource publicIPAddress 'Microsoft.Network/publicIPAddresses@2019-11-01' = {
  name: 'testPublicIPAddress'
  location: resourceGroup().location
  properties: {
    publicIPAllocationMethod: 'Dynamic'
    dnsSettings: {
      domainNameLabel: 'testDnsName'
    }
  }
}
