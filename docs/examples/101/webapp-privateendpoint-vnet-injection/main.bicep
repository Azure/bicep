param virtualNetworkName string = 'Vnet1'
param serverFarmName string = 'serverfarm'
param site1_Name string = 'webapp1-${uniqueString(resourceGroup().id)}'
param site2_Name string = 'webapp2-${uniqueString(resourceGroup().id)}'
param virtualNetwork_CIDR string = '10.200.0.0/16'
param subnet1Name string = 'Subnet1'
param subnet2Name string = 'Subnet2'
param subnet1_CIDR string = '10.200.1.0/24'
param subnet2_CIDR string = '10.200.2.0/24'
param location string = resourceGroup().location
param skuName string {
  default: 'P1v2'
  allowed: [
    'P1v2'
    'P2v2'
    'P3v2'
  ]
}

var webapp_dns_name = '.azurewebsites.net'
var privateDNSZoneName = 'privatelink.azurewebsites.net'
var SKU_tier = 'PremiumV2'

resource virtualNetwork 'Microsoft.Network/virtualNetworks@2020-06-01' = {
  name: virtualNetworkName
  location: location
  properties: {
    addressSpace: {
      addressPrefixes: [
        virtualNetwork_CIDR
      ]
    }
  }
}

resource subnet1 'Microsoft.Network/virtualNetworks/subnets@2020-06-01' = {
  name: '${virtualNetwork.name}/${subnet1Name}'
  properties: {
    addressPrefix: subnet1_CIDR
    privateEndpointNetworkPolicies: 'Disabled'
  }
}

resource subnet2 'Microsoft.Network/virtualNetworks/subnets@2020-06-01' = {
  name: '${virtualNetwork.name}/${subnet2Name}'
  properties: {
    addressPrefix: subnet2_CIDR
    delegations: [
      {
        name: 'delegation'
        properties: {
          serviceName: 'Microsoft.Web/serverfarms'
        }
      }
    ]
    privateEndpointNetworkPolicies: 'Enabled'
  }
  dependsOn: [
    subnet1
  ]
}

resource serverFarm 'Microsoft.Web/serverfarms@2020-06-01' = {
  name: serverFarmName
  location: location
  sku: {
    name: skuName
    tier: SKU_tier
    size: skuName
    family: skuName
    capacity: 1
  }
  kind: 'app'
}

resource webApp1 'Microsoft.Web/sites@2020-06-01' = {
  name: site1_Name
  location: location
  kind: 'app'
  properties: {
    serverFarmId: serverFarm.id
  }
}

resource webApp2 'Microsoft.Web/sites@2020-06-01' = {
  name: site2_Name
  location: location
  kind: 'app'
  properties: {
    serverFarmId: serverFarm.id
  }
}
resource webApp2AppSettings 'Microsoft.Web/sites/config@2018-11-01' = {
  name: '${webApp2.name}/appsettings'
  properties: {
    'WEBSITE_DNS_SERVER': '168.63.129.16'
    'WEBSITE_VNET_ROUTE_ALL': '1'
  }
}

resource webApp1Config 'Microsoft.Web/sites/config@2019-08-01' = {
  name: '${webApp1.name}/web'
  properties: {
    ftpsState: 'AllAllowed'
  }
}

resource webApp2Config 'Microsoft.Web/sites/config@2019-08-01' = {
  name: '${webApp2.name}/web'
  properties: {
    ftpsState: 'AllAllowed'
  }
}

resource webApp1Binding 'Microsoft.Web/sites/hostNameBindings@2019-08-01' = {
  name: '${webApp1.name}/${webApp1.name}${webapp_dns_name}'
  properties: {
    siteName: webApp1.name
    hostNameType: 'Verified'
  }
}

resource webApp2Binding 'Microsoft.Web/sites/hostNameBindings@2019-08-01' = {
  name: '${webApp2.name}/${webApp2.name}${webapp_dns_name}'
  properties: {
    siteName: webApp2.name
    hostNameType: 'Verified'
  }
}

resource webApp2NetworkConfig 'Microsoft.Web/sites/networkConfig@2020-06-01' = {
  name: '${webApp2.name}/VirtualNetwork'
  properties: {
    subnetResourceId: resourceId('Microsoft.Network/virtualNetworks/subnets', virtualNetwork.name, subnet2Name)
  }
}

resource privateEndpoint 'Microsoft.Network/privateEndpoints@2020-06-01' = {
  name: 'PrivateEndpoint1'
  location: location
  properties: {
    subnet: {
      id: resourceId('Microsoft.Network/virtualNetworks/subnets', virtualNetwork.name, subnet1Name)
    }
    privateLinkServiceConnections: [
      {
        name: 'PrivateEndpointLink1'
        properties: {
          privateLinkServiceId: webApp1.id
          groupIds: [
            'sites'
          ]
        }
      }
    ]
  }
}

resource privateDnsZones 'Microsoft.Network/privateDnsZones@2018-09-01' = {
  name: privateDNSZoneName
  location: 'global'
  properties: {}
}

resource privateDnsZoneLink 'Microsoft.Network/privateDnsZones/virtualNetworkLinks@2018-09-01' = {
  name: '${privateDnsZones.name}/${privateDnsZones.name}-link'
  location: 'global'
  properties: {
    registrationEnabled: false
    virtualNetwork: {
      id: virtualNetwork.id
    }
  }
}
resource privateDnsZoneGroup 'Microsoft.Network/privateEndpoints/privateDnsZoneGroups@2020-06-01' = {
  name: '${privateEndpoint.name}/dnsgroupname'
  properties: {
    privateDnsZoneConfigs: [
      {
        name: 'config1'
        properties: {
          privateDnsZoneId: privateDnsZones.id
        }
      }
    ]
  }
}
