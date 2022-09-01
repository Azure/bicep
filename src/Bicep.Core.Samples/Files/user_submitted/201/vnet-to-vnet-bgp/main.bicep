// converted from https://github.com/Azure/azure-quickstart-templates/tree/master/201-vnet-to-vnet-bgp
@secure()
param sharedKey string

@allowed([
  'Standard'
  'HighPerformance'
  'VpnGw1'
  'VpnGw2'
  'VpnGw3'
])
param gatewaySku string = 'VpnGw1'

param location string = resourceGroup().location

var vnet1cfg = {
  name: 'vnet1-${location}'
  addressSpacePrefix: '10.0.0.0/23'
  subnetName: 'subnet1'
  subnetPrefix: '10.0.0.0/24'
  gatewayName: 'vpngw1'
  gatewaySubnetPrefix: '10.0.1.224/27'
  gatewayPublicIPName: 'gw1pip'
  connectionName: 'vnet1-to-vnet2'
  asn: 65010
}

var vnet2cfg = {
  name: 'vnet2-${location}'
  addressSpacePrefix: '10.0.2.0/23'
  subnetName: 'subnet1'
  subnetPrefix: '10.0.2.0/24'
  gatewayName: 'vpngw2'
  gatewaySubnetPrefix: '10.0.3.224/27'
  gatewayPublicIPName: 'gw2pip'
  connectionName: 'vnet2-to-vnet1'
  asn: 65020
}

resource vnet1 'Microsoft.Network/virtualNetworks@2020-06-01' = {
  name: vnet1cfg.name
  location: location
  properties: {
    addressSpace: {
      addressPrefixes: [
        vnet1cfg.addressSpacePrefix
      ]
    }
    subnets: [
      {
        name: vnet1cfg.subnetName
        properties: {
          addressPrefix: vnet1cfg.subnetPrefix
          networkSecurityGroup: {
            properties: {
              securityRules: [
                {
                  properties: {
                    direction: 'Inbound'
                    protocol: '*'
                    access: 'Allow'
                  }
                }
                {
                  properties: {
                    direction: 'Outbound'
                    protocol: '*'
                    access: 'Allow'
                  }
                }
              ]
            }
          }
        }
      }
      {
        name: 'GatewaySubnet'
        properties: {
          addressPrefix: vnet1cfg.gatewaySubnetPrefix
          networkSecurityGroup: {
            properties: {
              securityRules: [
                {
                  properties: {
                    direction: 'Inbound'
                    protocol: '*'
                    access: 'Allow'
                  }
                }
                {
                  properties: {
                    direction: 'Outbound'
                    protocol: '*'
                    access: 'Allow'
                  }
                }
              ]
            }
          }
        }
      }
    ]
  }
}

resource vnet2 'Microsoft.Network/virtualNetworks@2020-06-01' = {
  name: vnet2cfg.name
  location: location
  properties: {
    addressSpace: {
      addressPrefixes: [
        vnet2cfg.addressSpacePrefix
      ]
    }
    subnets: [
      {
        name: vnet2cfg.subnetName
        properties: {
          addressPrefix: vnet2cfg.subnetPrefix
          networkSecurityGroup: {
            properties: {
              securityRules: [
                {
                  properties: {
                    direction: 'Inbound'
                    protocol: '*'
                    access: 'Allow'
                  }
                }
                {
                  properties: {
                    direction: 'Outbound'
                    protocol: '*'
                    access: 'Allow'
                  }
                }
              ]
            }
          }
        }
      }
      {
        name: 'GatewaySubnet'
        properties: {
          addressPrefix: vnet2cfg.gatewaySubnetPrefix
          networkSecurityGroup: {
            properties: {
              securityRules: [
                {
                  properties: {
                    direction: 'Inbound'
                    protocol: '*'
                    access: 'Allow'
                  }
                }
                {
                  properties: {
                    direction: 'Outbound'
                    protocol: '*'
                    access: 'Allow'
                  }
                }
              ]
            }
          }
        }
      }
    ]
  }
}

resource gw1pip 'Microsoft.Network/publicIPAddresses@2020-06-01' = {
  name: vnet1cfg.gatewayPublicIPName
  location: location
  properties: {
    publicIPAllocationMethod: 'Dynamic'
  }
}

resource gw2pip 'Microsoft.Network/publicIPAddresses@2020-06-01' = {
  name: vnet2cfg.gatewayPublicIPName
  location: location
  properties: {
    publicIPAllocationMethod: 'Dynamic'
  }
}

resource vpngw1 'Microsoft.Network/virtualNetworkGateways@2020-06-01' = {
  name: vnet1cfg.gatewayName
  location: location
  properties: {
    ipConfigurations: [
      {
        name: 'vnet1gwipconfig'
        properties: {
          privateIPAllocationMethod: 'Dynamic'
          subnet: {
            id: '${vnet1.id}/subnets/GatewaySubnet'
          }
          publicIPAddress: {
            id: gw1pip.id
          }
        }
      }
    ]
    gatewayType: 'Vpn'
    sku: {
      name: gatewaySku
      tier: gatewaySku
    }
    vpnType: 'RouteBased'
    enableBgp: true
    bgpSettings: {
      asn: vnet1cfg.asn
    }
  }
}

resource vpngw2 'Microsoft.Network/virtualNetworkGateways@2020-06-01' = {
  name: vnet2cfg.gatewayName
  location: location
  properties: {
    ipConfigurations: [
      {
        name: 'vnet2gwipconfig'
        properties: {
          privateIPAllocationMethod: 'Dynamic'
          subnet: {
            id: '${vnet2.id}/subnets/GatewaySubnet'
          }
          publicIPAddress: {
            id: gw2pip.id
          }
        }
      }
    ]
    gatewayType: 'Vpn'
    sku: {
      name: gatewaySku
      tier: gatewaySku
    }
    vpnType: 'RouteBased'
    enableBgp: true
    bgpSettings: {
      asn: vnet2cfg.asn
    }
  }
}

resource vpn1to2 'Microsoft.Network/connections@2020-06-01' = {
  name: vnet1cfg.connectionName
  location: location
  properties: {
    virtualNetworkGateway1: {
      id: vpngw1.id
      properties: {}
    }
    virtualNetworkGateway2: {
      id: vpngw2.id
      properties: {}
    }
    connectionType: 'Vnet2Vnet'
    routingWeight: 3
    sharedKey: sharedKey
    enableBgp: true
  }
}

resource vpn2to1 'Microsoft.Network/connections@2020-06-01' = {
  name: vnet2cfg.connectionName
  location: location
  properties: {
    virtualNetworkGateway1: {
      id: vpngw2.id
      properties: {}
    }
    virtualNetworkGateway2: {
      id: vpngw1.id
      properties: {}
    }
    connectionType: 'Vnet2Vnet'
    routingWeight: 3
    sharedKey: sharedKey
    enableBgp: true
  }
}
