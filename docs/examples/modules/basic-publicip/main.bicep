module publicIp './publicIpAddress.bicep' = {
  name: 'publicIp'
  params: {
    publicIpResourceName: 'myPublicIp'
    dynamicAllocation: true
    // Parameters with default values may be omitted.
  }
}

// To reference module outputs
output assignedIp string = publicIp.outputs.ipAddress