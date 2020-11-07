resource pip 'Microsoft.Network/publicIpAddresses@2020-05-01' =  {
  name: 'myvm-pip'
  location: 'eastus'
  properties: {
    publicIPAllocationMethod: 'Dynamic'
  }
}
  