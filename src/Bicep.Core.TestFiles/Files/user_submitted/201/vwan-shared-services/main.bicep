// based on this routing scenario: https://docs.microsoft.com/en-us/azure/virtual-wan/scenario-shared-services-vnet
param location string = resourceGroup().location

var vwan_cfg = {
  name: 'vwan'
  type: 'Standard'
}

var virtual_hub_cfg = {
  name: 'virtual_hub'
  addressSpacePrefix: '192.168.0.0/24'
}

var vnet_shared_services_cfg = {
  name: 'vnet_shared_services'
  addressSpacePrefix: '10.0.0.0/24'
  subnetName: 'subnet1'
  subnetPrefix: '10.0.0.0/24'
}

var vnet_isolated_1_cfg = {
  name: 'vnet_isolated_1'
  addressSpacePrefix: '10.0.10.0/24'
  subnetName: 'subnet1'
  subnetPrefix: '10.0.10.0/24'
}

var vnet_isolated_2_cfg = {
  name: 'vnet_isolated_2'
  addressSpacePrefix: '10.0.20.0/24'
  subnetName: 'subnet1'
  subnetPrefix: '10.0.20.0/24'
}

resource vwan 'Microsoft.Network/virtualWans@2020-05-01' = {
  name: vwan_cfg.name
  location: location
  properties: {
    allowVnetToVnetTraffic: true
    allowBranchToBranchTraffic: true
    type: vwan_cfg.type
  }
}

resource virtual_hub 'Microsoft.Network/virtualHubs@2020-05-01' = {
  name: virtual_hub_cfg.name
  location: location
  properties: {
    addressPrefix: virtual_hub_cfg.addressSpacePrefix
    virtualWan: {
      id: vwan.id
    }
  }
}

resource rt_shared 'Microsoft.Network/virtualHubs/hubRouteTables@2020-05-01' = {
  name: '${virtual_hub_cfg.name}/RT_SHARED'
  properties: {
    routes: [
      {
        name: 'route_to_shared_services'
        destinationType: 'CIDR'
        destinations: [
          vnet_shared_services_cfg.addressSpacePrefix
        ]
        nextHopType: 'ResourceId'
        nextHop: '${virtual_hub.id}/hubVirtualNetworkConnections/${vnet_shared_services_cfg.name}_connection'
      }
    ]
  }
}

resource vnet_shared_services 'Microsoft.Network/virtualNetworks@2020-05-01' = {
  name: vnet_shared_services_cfg.name
  location: location
  properties: {
    addressSpace: {
      addressPrefixes: [
        vnet_shared_services_cfg.addressSpacePrefix
      ]
    }
    subnets: [
      {
        name: vnet_shared_services_cfg.subnetName
        properties: {
          addressPrefix: vnet_shared_services_cfg.subnetPrefix
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

resource vnet_isolated_1 'Microsoft.Network/virtualNetworks@2020-05-01' = {
  name: vnet_isolated_1_cfg.name
  location: location
  properties: {
    addressSpace: {
      addressPrefixes: [
        vnet_isolated_1_cfg.addressSpacePrefix
      ]
    }
    subnets: [
      {
        name: vnet_isolated_1_cfg.subnetName
        properties: {
          addressPrefix: vnet_isolated_1_cfg.subnetPrefix
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

resource vnet_isolated_2 'Microsoft.Network/virtualNetworks@2020-05-01' = {
  name: vnet_isolated_2_cfg.name
  location: location
  properties: {
    addressSpace: {
      addressPrefixes: [
        vnet_isolated_2_cfg.addressSpacePrefix
      ]
    }
    subnets: [
      {
        name: vnet_isolated_2_cfg.subnetName
        properties: {
          addressPrefix: vnet_isolated_2_cfg.subnetPrefix
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

resource vnet_shared_services_connection 'Microsoft.Network/virtualHubs/hubVirtualNetworkConnections@2020-05-01' = {
  name: '${virtual_hub_cfg.name}/${vnet_shared_services_cfg.name}_connection'
  properties: {
    remoteVirtualNetwork: {
      id: vnet_shared_services.id
    }
    routingConfiguration: {
      associatedRouteTable: {
        id: '${virtual_hub.id}/hubRouteTables/defaultRouteTable'
      }
      propagatedRouteTables: {
        ids: [
          {
            id: '${virtual_hub.id}/hubRouteTables/defaultRouteTable'
          }
          {
            id: rt_shared.id
          }
        ]
      }
    }
    allowHubToRemoteVnetTransit: true
    allowRemoteVnetToUseHubVnetGateways: true
    enableInternetSecurity: true
  }
}

resource vnet_isolated_1_connection 'Microsoft.Network/virtualHubs/hubVirtualNetworkConnections@2020-05-01' = {
  name: '${virtual_hub_cfg.name}/${vnet_isolated_1_cfg.name}_connection'
  properties: {
    remoteVirtualNetwork: {
      id: vnet_isolated_1.id
    }
    routingConfiguration: {
      associatedRouteTable: {
        id: rt_shared.id
      }
      propagatedRouteTables: {
        ids: [
          {
            id: '${virtual_hub.id}/hubRouteTables/defaultRouteTable'
          }
        ]
      }
    }
    allowHubToRemoteVnetTransit: true
    allowRemoteVnetToUseHubVnetGateways: true
    enableInternetSecurity: true
  }
  dependsOn: [
    vnet_shared_services_connection
  ]
}

resource vnet_isolated_2_connection 'Microsoft.Network/virtualHubs/hubVirtualNetworkConnections@2020-05-01' = {
  name: '${virtual_hub_cfg.name}/${vnet_isolated_2_cfg.name}_connection'
  properties: {
    remoteVirtualNetwork: {
      id: vnet_isolated_2.id
    }
    routingConfiguration: {
      associatedRouteTable: {
        id: rt_shared.id
      }
      propagatedRouteTables: {
        ids: [
          {
            id: '${virtual_hub.id}/hubRouteTables/defaultRouteTable'
          }
        ]
      }
    }
    allowHubToRemoteVnetTransit: true
    allowRemoteVnetToUseHubVnetGateways: true
    enableInternetSecurity: true
  }
  dependsOn: [
    vnet_isolated_1_connection
  ]
}
