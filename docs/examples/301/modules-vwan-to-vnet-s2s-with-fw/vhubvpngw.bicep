param location string {
  default: resourceGroup().location
  metadata: {
    description: 'Specifies the Azure location where the resource should be created.'
  }
}
param hubvpngwname string {
  metadata: {
    description: 'Specifies the name to use for the Virtual Hub VPN Gateway resource.'
  }
}
param hubid string {
  metadata: {
    description: 'Virtual WAN ID'
  }
}
param asn int {
  metadata: {
    description: 'BGP ASN'
  }
}

resource hubvpngw 'Microsoft.Network/vpnGateways@2020-06-01' = {
  name: hubvpngwname
  location: location   
  properties: {        
      virtualHub: {
          id: hubid
      }
      bgpSettings: {
          asn: asn
      }     
  }     
}

output id string = hubvpngw.id
output name string = hubvpngw.name
output gwpublicip string = hubvpngw.properties.ipConfigurations[0].publicIpAddress
output gwprivateip string = hubvpngw.properties.ipConfigurations[0].privateIpAddress
output bgpasn int = hubvpngw.properties.bgpSettings.asn