// $1 = applicationGateway
// $2 = 'name'
// $3 = location
// $4 = Standard_Small
// $5 = Standard
// $6 = 2
// $7 = 'name'
// $8 = 'id'
// $9 = 'name'
// $10 = 'id'
// $11 = 'name'
// $12 = 80
// $13 = 'name'
// $14 = 'name'
// $15 = 80
// $16 = Http
// $17 = 'name'
// $18 = 'id'
// $19 = 'id'
// $20 = Http
// $21 = 'name'
// $22 = Basic
// $23 = 'id'
// $24 = 'id'
// $25 = 'id'

param location string

resource applicationGateway 'Microsoft.Network/applicationGateways@2020-11-01' = {
  name: 'name'
  location: location
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
// Insert snippet here

