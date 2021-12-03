// Public IP Address
resource /*${1:publicIPAddress}*/publicIPAddress 'Microsoft.Network/publicIPAddresses@2019-11-01' = {
  name: /*${2:'name'}*/'name'
  location: location
  properties: {
    publicIPAllocationMethod: 'Dynamic'
    dnsSettings: {
      domainNameLabel: /*${3:'dnsname'}*/'dnsname'
    }
  }
}
