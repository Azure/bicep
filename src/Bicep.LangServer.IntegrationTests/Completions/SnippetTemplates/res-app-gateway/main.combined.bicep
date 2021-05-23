resource applicationGateway 'Microsoft.Network/applicationGateways@2020-11-01' = {
  name: 'name'
  location: resourceGroup().location
  properties: {
    sku: {
      name: 'Standard_Small'
      tier: 'Standard'
      capacity: 2
    }
    gatewayIPConfigurations: [
      {
        name: 'name'
        properties: {
          subnet: {
            id: resourceId('Microsoft.Network/virtualNetworks/subnets', 'virtualNetwork', 'Subnet-1')
          }
        }
      }
    ]
    frontendIPConfigurations: [
      {
        name: 'name'
        properties: {
          publicIPAddress: {
            id: resourceId('Microsoft.Network/publicIPAddresses', 'publicIPAddress')
          }
        }
      }
    ]
    frontendPorts: [
      {
        name: 'name'
        properties: {
          port: 80
        }
      }
    ]
    backendAddressPools: [
      {
        name: 'name'
      }
    ]
    backendHttpSettingsCollection: [
      {
        name: 'name'
        properties: {
          port: 80
          protocol: 'Http'
          cookieBasedAffinity: 'Disabled'
        }
      }
    ]
    httpListeners: [
      {
        name: 'name'
        properties: {
          frontendIPConfiguration: {
            id: resourceId('Microsoft.Network/applicationGateways/frontendIPConfigurations', 'name', 'name')
          }
          frontendPort: {
            id: resourceId('Microsoft.Network/applicationGateways/frontendPorts', 'name', 'name')
          }
          protocol: 'Http'
          sslCertificate: null
        }
      }
    ]
    requestRoutingRules: [
      {
        name: 'name'
        properties: {
          ruleType: 'Basic'
          httpListener: {
            id: resourceId('Microsoft.Network/applicationGateways/httpListeners', 'name', 'name')
          }
          backendAddressPool: {
            id: resourceId('Microsoft.Network/applicationGateways/backendAddressPools', 'name', 'name')
          }
          backendHttpSettings: {
            id: resourceId('Microsoft.Network/applicationGateways/backendHttpSettingsCollection', 'name', 'name')
          }
        }
      }
    ]
  }
}

