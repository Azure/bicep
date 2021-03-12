resource publicIPAddress1 'Microsoft.Network/publicIPAddresses@2019-11-01' = {
  name: '${1:publicIPAddress1}'
  location: resourceGroup().location
  tags: {
    displayName: '${1:publicIPAddress1}'
  }
  properties: {
    publicIPAllocationMethod: 'Dynamic'
    dnsSettings: {
      domainNameLabel: '${2:dnsname1}'
    }
  }
}