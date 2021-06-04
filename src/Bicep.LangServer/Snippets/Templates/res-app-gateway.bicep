// Application Gateway
resource ${1:applicationGateway} 'Microsoft.Network/applicationGateways@2020-11-01' = {
  name: ${2:'name'}
  location: resourceGroup().location
  properties: {
    sku: {
      name: '${3|Standard_Small,Standard_Medium,Standard_Large,WAF_Medium,WAF_Large,Standard_v2,WAF_v2|}'
      tier: '${4|Standard,WAF,Standard_v2,WAF_v2|}'
      capacity: ${5:'capacity'}
    }
    gatewayIPConfigurations: [
      {
        name: ${6:'name'}
        properties: {
          subnet: {
            id: ${7:'id'}
          }
        }
      }
    ]
    frontendIPConfigurations: [
      {
        name: ${8:'name'}
        properties: {
          publicIPAddress: {
            id: ${9:'id'}
          }
        }
      }
    ]
    frontendPorts: [
      {
        name: ${10:'name'}
        properties: {
          port: ${11:'port'}
        }
      }
    ]
    backendAddressPools: [
      {
        name: ${12:'name'}
      }
    ]
    backendHttpSettingsCollection: [
      {
        name: ${13:'name'}
        properties: {
          port: ${14:'port'}
          protocol: '${15|Http,Https|}'
          cookieBasedAffinity: 'Disabled'
        }
      }
    ]
    httpListeners: [
      {
        name: ${16:'name'}
        properties: {
          frontendIPConfiguration: {
            id: ${17:'id'}
          }
          frontendPort: {
            id: ${18:'id'}
          }
          protocol: '${19|Http,Https|}'
          sslCertificate: null
        }
      }
    ]
    requestRoutingRules: [
      {
        name: ${20:'name'}
        properties: {
          ruleType: '${21|Basic,PathBasedRouting|}'
          httpListener: {
            id: ${22:'id'}
          }
          backendAddressPool: {
            id: ${23:'id'}
          }
          backendHttpSettings: {
            id: ${24:'id'}
          }
        }
      }
    ]
  }
}
