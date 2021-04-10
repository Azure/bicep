// Public IP Address
resource publicIPAddress 'Microsoft.Network/publicIPAddresses@2019-11-01' = {
  name: '${1:publicIPAddress}'
  location: resourceGroup().location
  properties: {
    publicIPAllocationMethod: 'Dynamic'
    dnsSettings: {
      domainNameLabel: '${2:dnsname}'
    }
  }
}