param serviceBusNamespaceName string
param virtualNetworkName string
param subnetName string
param location string = resourceGroup().location

resource serviceBusNamespace 'Microsoft.ServiceBus/namespaces@2018-01-01-preview' = {
  name: serviceBusNamespaceName
  location: location
  sku: {
    name: 'Premium'
    tier: 'Premium'
  }
  properties: {}
}

resource virtualNetwork 'Microsoft.Network/virtualNetworks@2017-09-01' = {
  name: virtualNetworkName
  location: location
  properties: {
    addressSpace: {
      addressPrefixes: [
        '10.0.0.0/23'
      ]
    }
    subnets: [
      {
        name: subnetName
        properties: {
          addressPrefix: '10.0.0.0/23'
          serviceEndpoints: [
            {
              service: 'Microsoft.ServiceBus'
            }
          ]
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

resource namespaceVirtualNetworkRule 'Microsoft.ServiceBus/namespaces/VirtualNetworkRules@2018-01-01-preview' = {
  name: '${serviceBusNamespace.name}/${virtualNetwork.name}'
  properties: {
    virtualNetworkSubnetId: resourceId('Microsoft.Network/virtualNetworks/subnets/', virtualNetwork.name, subnetName)
  }
}
