param virtualNetworkName string = 'vnet-01'
param virtualNetworkPrefix string
param subnetName string
param subnetPrefix string
param gatewaySubnetPrefix string
param gatewayPublicIPName string
param gatewayName string
param connectionName string
param circuitName string
param location string = resourceGroup().location

resource virtualNetwork 'Microsoft.Network/virtualNetworks@2020-06-01' = {
  name: virtualNetworkName
  location: location
  properties: {
    addressSpace: {
      addressPrefixes: [
        virtualNetworkPrefix
      ]
    }
  }
}
resource subnet 'Microsoft.Network/virtualNetworks/subnets@2020-06-01' = {
  name: '${virtualNetwork.name}/${subnetName}'
  properties: {
    addressPrefix: subnetPrefix
  }
}
resource gatewaySubnet 'Microsoft.Network/virtualNetworks/subnets@2020-06-01' = {
  name: '${virtualNetwork.name}/GatewaySubnet'
  properties: {
    addressPrefix: gatewaySubnetPrefix
  }
}
resource publicIP 'Microsoft.Network/publicIPAddresses@2020-06-01' = {
  name: gatewayPublicIPName
  location: location
  properties: {
    publicIPAllocationMethod: 'Dynamic'
  }
}
resource virtualNetworkGateway 'Microsoft.Network/virtualNetworkGateways@2020-06-01' = {
  name: gatewayName
  location: location
  properties: {
    ipConfigurations: [
      {
        name: 'vnetGatewayConfig'
        properties: {
          privateIPAllocationMethod: 'Dynamic'
          subnet: {
            id: gatewaySubnet.id
          }
          publicIPAddress: {
            id: publicIP.id
          }
        }
      }
    ]
    gatewayType: 'ExpressRoute'
  }
}
resource connection 'Microsoft.Network/connections@2020-06-01' = {
  name: connectionName
  location: location
  properties: {
    virtualNetworkGateway1: {
      id: virtualNetworkGateway.id
      properties: {}
    }
    peer: {
      id: resourceId('Microsoft.Network/expressRouteCircuits', circuitName)
    }
    connectionType: 'ExpressRoute'
    routingWeight: 3
  }
}
