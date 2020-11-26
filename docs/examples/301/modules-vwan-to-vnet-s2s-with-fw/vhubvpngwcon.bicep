param hubvpngwname string {
  metadata: {
    description: 'Specifies the name of the Virtual Hub where the Vpn Vonnection should be created.'
  }
}
param psk string {
  secure: true
  metadata: {
    description: 'Specifies the PSK to use for the VPN Connection'
  }
}
param vpnsiteid string {
  metadata: {
    description: 'Specifies the resource id to the VWAN Vpn Site to connect to'
  }
}

resource hubvpnconnection 'Microsoft.Network/vpnGateways/vpnConnections@2020-05-01' = {
  name: '${hubvpngwname}/HubToOnPremConnection'
  properties: {
      connectionBandwidth: 10
      enableBgp: true
      sharedKey: psk
      remoteVpnSite: {
          id: vpnsiteid
      }
  }    
}