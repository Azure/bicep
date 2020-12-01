param vpnsitename string
param location string = resourceGroup().location
param addressprefix string {
  metadata: {
    description: 'Specifices the VPN Sites local IP Addresses'
  }
}
param bgppeeringpddress string {
  metadata: {
    description: 'Specifices the VPN Sites BGP Peering IP Addresses'
  }
}
param ipaddress string {
  metadata: {
    description: 'Specifices the VPN Sites VPN Device IP Address'
  }
}
param wanid string {
  metadata: {
    description: 'Specifices the resource ID of the Virtual WAN where the VPN Site should be created'
  }
}
param remotesiteasn int {
  metadata: {
    description: 'BGP AS-Number used by the remote site'
  }
}

resource vpnsite 'Microsoft.Network/vpnSites@2020-06-01' = {
  name: vpnsitename
  location: location
  properties: {
    addressSpace: {
      addressPrefixes: [
        addressprefix
      ]
    }
    bgpProperties: {
      asn: remotesiteasn
      bgpPeeringAddress: bgppeeringpddress
      peerWeight: 0
    }
    deviceProperties: {
      linkSpeedInMbps: 0
    }
    ipAddress: ipaddress
    virtualWan: {
      id: wanid
    }
  }
}

output id string = vpnsite.id
