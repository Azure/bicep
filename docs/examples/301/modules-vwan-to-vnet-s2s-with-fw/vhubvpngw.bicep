param location string = resourceGroup().location
param hubvpngwname string
param hubid string {
  metadata: {
    description: 'Virtual Hub ID'
  }
}
param asn int {
  metadata: {
    description: 'BGP AS number for the VPN Gateway'
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
