resource pip 'Microsoft.Network/publicIpAddresses@2020-05-01' = {
  name: 'examplePip'
  location: 'eastus'
  properties: {
    dnsSettings: {
      domainNameLabel: 'dnslbl'
    }
    idleTimeoutInMinutes: 10
    publicIPAddressVersion: 'IPv4'
    publicIPAllocationMethod: 'Static'
  }
  sku: {
    name: 'Standard'
  }
  tags: {
    TagA: 'Value A'
    TagB: 'Value B'
  }
  zones: [
    '1'
  ]
}
