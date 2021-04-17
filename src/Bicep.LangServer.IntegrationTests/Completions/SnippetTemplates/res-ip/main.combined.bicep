resource publicIPAddress 'Microsoft.Network/publicIPAddresses@2019-11-01' = {
  name: 'publicIPAddress'
  location: resourceGroup().location
  properties: {
    publicIPAllocationMethod: 'Dynamic'
    dnsSettings: {
      domainNameLabel: 'dnsName'
    }
  }
}
