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
            id: resourceId('Microsoft.Network/virtualNetworks/subnets', ${7:'virtualNetwork'}, ${8:'Subnet-1'})
          }
        }
      }
    ]
    frontendIPConfigurations: [
      {
        name: ${9:'name'}
        properties: {
          publicIPAddress: {
            id: resourceId('Microsoft.Network/publicIPAddresses', ${10:'publicIPAddress'})
          }
        }
      }
    ]
    frontendPorts: [
      {
        name: ${11:'name'}
        properties: {
          port: ${12:'port'}
        }
      }
    ]
    backendAddressPools: [
      {
        name: ${13:'name'}
      }
    ]
    backendHttpSettingsCollection: [
      {
        name: ${14:'name'}
        properties: {
          port: ${15:'port'}
          protocol: '${16|Http,Https|}'
          cookieBasedAffinity: 'Disabled'
        }
      }
    ]
    httpListeners: [
      {
        name: ${17:'name'}
        properties: {
          frontendIPConfiguration: {
            id: resourceId('Microsoft.Network/applicationGateways/frontendIPConfigurations', ${2:'name'}, ${9:'name'})
          }
          frontendPort: {
            id: resourceId('Microsoft.Network/applicationGateways/frontendPorts', ${2:'name'}, ${11:'name'})
          }
          protocol: '${18|Http,Https|}'
          sslCertificate: null
        }
      }
    ]
    requestRoutingRules: [
      {
        name: ${19:'name'}
        properties: {
          ruleType: '${20|Basic,PathBasedRouting|}'
          httpListener: {
            id: resourceId('Microsoft.Network/applicationGateways/httpListeners', ${2:'name'}, ${17:'name'})
          }
          backendAddressPool: {
            id: resourceId('Microsoft.Network/applicationGateways/backendAddressPools', ${2:'name'}, ${13:'name'})
          }
          backendHttpSettings: {
            id: resourceId('Microsoft.Network/applicationGateways/backendHttpSettingsCollection', ${2:'name'}, ${14:'name'})
          }
        }
      }
    ]
  }
}
