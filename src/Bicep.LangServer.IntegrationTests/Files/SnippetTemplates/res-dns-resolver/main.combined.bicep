// $1 = dnsResolvers
// $2 = 'name'
// $3 = location
// $4 = 'virtualNetworkId'
// $5 = inboundEndpoints
// $6 = 'name'
// $7 = location
// $8 = 'Dynamic'
// $9 = 'subnetId'
// $10 = outboundEndpoints
// $11 = 'name'
// $12 = location
// $13 = 'subnetId'

param location string

resource dnsResolvers 'Microsoft.Network/dnsResolvers@2022-07-01' = {
  name: 'name'
  location: location
  properties: {
     virtualNetwork: {
      id: 'virtualNetworkId'
     }
  }

  resource inboundEndpoints 'inboundEndpoints' = {
    name: 'name'
    location: location
    properties: {
      ipConfigurations: [
        {
          privateIpAllocationMethod: 'Dynamic'
          subnet: {
            id: 'subnetId'
          }
        }
      ]
    }
  }

  resource outboundEndpoints 'outboundEndpoints' = {
    name: 'name'
    location: location
    properties: {
      subnet: {
        id: 'subnetId'
      }
    }
  }
}


