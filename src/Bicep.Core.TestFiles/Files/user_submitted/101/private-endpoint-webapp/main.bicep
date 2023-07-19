param location string = resourceGroup().location
param virtualNetworkName string = 'vnet-01'
param virtualNetworkCIDR string = '10.200.0.0/16'
param subnetName string = 'subnet-01'
param subnetCIDR string = '10.200.1.0/24'
param serverFarmName string = 'ServerFarm1'
param websiteName string = 'website${uniqueString(resourceGroup().name)}'
param skuName string = 'P1v2'
param skuTier string = 'PremiumV2'
param skuSize string = 'P1v2'
param skuFamily string = 'P1v2'
param privateEndpointName string = 'privateEndpoint${uniqueString(resourceGroup().name)}'
param privateLinkConnectionName string = 'privateLink${uniqueString(resourceGroup().name)}'
param privateDNSZoneName string = 'privatelink.azurewebsites.net'
param websiteDNSName string = '.azurewebsites.net'

resource virtualNetwork 'Microsoft.Network/virtualNetworks@2020-06-01' = {
  name: virtualNetworkName
  location: location
  properties: {
    addressSpace: {
      addressPrefixes: [
        virtualNetworkCIDR
      ]
    }
  }
}
resource subnet 'Microsoft.Network/virtualNetworks/subnets@2020-06-01' = {
  name: '${virtualNetwork.name}/${subnetName}'
  properties: {
    addressPrefix: subnetCIDR
    privateEndpointNetworkPolicies: 'Disabled'
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

resource serverFarm 'Microsoft.Web/serverfarms@2020-06-01' = {
  name: serverFarmName
  location: location
  sku: {
    name: skuName
    tier: skuTier
    size: skuSize
    family: skuFamily
    capacity: 1
  }
  kind: 'app'
}

resource website 'Microsoft.Web/sites@2020-06-01' = {
  name: websiteName
  location: location
  kind: 'app'
  properties: {
    enabled: true
    hostNameSslStates: [
      {
        name: '${websiteName}${websiteDNSName}'
        sslState: 'Disabled'
        hostType: 'Standard'
      }
      {
        name: '${websiteName}.scm${websiteDNSName}'
        sslState: 'Disabled'
        hostType: 'Repository'
      }
    ]
    serverFarmId: serverFarm.id
  }
}

resource hostnameBinding 'Microsoft.Web/sites/hostNameBindings@2020-06-01' = {
  name: '${website.name}/${website.name}${websiteDNSName}'
  properties: {
    siteName: website.name
    hostNameType: 'Verified'
  }
}

resource privateEndpoint 'Microsoft.Network/privateEndpoints@2020-06-01' = {
  name: privateEndpointName
  location: location
  properties: {
    subnet: {
      id: subnet.id
    }
    privateLinkServiceConnections: [
      {
        name: privateLinkConnectionName
        properties: {
          privateLinkServiceId: website.id
          groupIds: [
            'sites'
          ]
        }
      }
    ]
  }
}

resource privateDNSZone 'Microsoft.Network/privateDnsZones@2020-06-01' = {
  name: privateDNSZoneName
  location: 'global'
}

resource virtualNetworkLink 'Microsoft.Network/privateDnsZones/virtualNetworkLinks@2020-06-01' = {
  name: '${privateDNSZone.name}/${privateDNSZone.name}-link'
  location: 'global'
  properties: {
    registrationEnabled: false
    virtualNetwork: {
      id: virtualNetwork.id
    }
  }
}

resource privateDNSZoneGroup 'Microsoft.Network/privateEndpoints/privateDnsZoneGroups@2020-06-01' = {
  name: '${privateEndpoint.name}/dnsgroupname'
  properties: {
    privateDnsZoneConfigs: [
      {
        name: 'config1'
        properties: {
          privateDnsZoneId: privateDNSZone.id
        }
      }
    ]
  }
}
