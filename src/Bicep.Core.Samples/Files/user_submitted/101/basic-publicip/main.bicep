param publicIpName string = 'mypublicip'

module publicIp './publicIpAddress.bicep' = {
  name: 'publicIp'
  params: {
    publicIpResourceName: publicIpName
    dynamicAllocation: true
    // Parameters with default values may be omitted.
  }
}

// To reference module outputs
output ipFqdn string = publicIp.outputs.ipFqdn
