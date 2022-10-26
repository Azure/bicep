resource /*${1:dnsResolvers}*/dnsResolvers 'Microsoft.Network/dnsResolvers@2022-07-01' = {
  name: /*${2:'name'}*/'name'
  location: /*${3:location}*/'location'
  properties: {
     virtualNetwork: {
      id: /*${4:'virtualNetworkId'}*/'virtualNetworkId'
     }
  }

  resource /*${5:inboundEndpoints}*/inboundEndpoints 'inboundEndpoints' = {
    name: /*${6:'name'}*/'name'
    location: /*${7:location}*/'location'
    properties: {
      ipConfigurations: [
        {
          privateIpAllocationMethod: /*${8|'Static','Dynamic'|}*/'Dynamic'
          subnet: {
            id: /*${9:'subnetId'}*/'subnetId'
          }
        }
      ]
    }
  }

  resource /*${10:outboundEndpoints}*/outboundEndpoints 'outboundEndpoints' = {
    name: /*${11:'name'}*/'name'
    location: /*${12:location}*/'location'
    properties: {
      subnet: {
        id: /*${13:'subnetId'}*/'subnetId'
      }
    }
  }
}
