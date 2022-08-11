// Input parameters must be specified by the module consumer
param publicIpResourceName string
param publicIpDnsLabel string = '${publicIpResourceName}-${newGuid()}'
param location string = resourceGroup().location
param dynamicAllocation bool

resource publicIp 'Microsoft.Network/publicIPAddresses@2020-06-01' = {
  name: publicIpResourceName
  location: location
  properties: {
    publicIPAllocationMethod: dynamicAllocation ? 'Dynamic' : 'Static'
    dnsSettings: {
      domainNameLabel: publicIpDnsLabel
    }
  }
}

// Set an output which can be accessed by the module consumer
output ipFqdn string = publicIp.properties.dnsSettings.fqdn
