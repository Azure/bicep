// Public IP Address
resource ${1:publicIPAddress} 'Microsoft.Network/publicIPAddresses@2019-11-01' = {
  name: ${2:'name'}
  location: resourceGroup().location
  properties: {
    publicIPAllocationMethod: 'Dynamic'
    dnsSettings: {
      domainNameLabel: ${3:'dnsname'}
    }
  }
}
