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
            id: 'id'
          }
        }
      }
    ]
    frontendIPConfigurations: [
      {
        name: 'name'
        properties: {
          publicIPAddress: {
            id: 'id'
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
            id: 'id'
          }
          frontendPort: {
            id: 'id'
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
            id: 'id'
          }
          backendAddressPool: {
            id: 'id'
          }
          backendHttpSettings: {
            id: 'id'
          }
        }
      }
    ]
  }
}

