// Public IP Address
resource ${1:'publicIPAddress'} 'Microsoft.Network/publicIPAddresses@2019-11-01' = {
  name: 'name'
  location: resourceGroup().location
  properties: {
    publicIPAllocationMethod: 'Dynamic'
    dnsSettings: {
      domainNameLabel: ${2:'dnsname'}
    }
  }
}
