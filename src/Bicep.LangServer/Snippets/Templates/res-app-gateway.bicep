// Application Gateway
resource /*${1:applicationGateway}*/applicationGateway 'Microsoft.Network/applicationGateways@2020-11-01' = {
  name: /*${2:'name'}*/'name'
  location: location
  properties: {
    sku: {
      name: /*'${3|Standard_Small,Standard_Medium,Standard_Large,WAF_Medium,WAF_Large,Standard_v2,WAF_v2|}'*/'Standard_Small'
      tier: /*'${4|Standard,WAF,Standard_v2,WAF_v2|}'*/'Standard'
      capacity: /*${5:'capacity'}*/'capacity'
    }
    gatewayIPConfigurations: [
      {
        name: /*${6:'name'}*/'name'
        properties: {
          subnet: {
            id: /*${7:'id'}*/'id'
          }
        }
      }
    ]
    frontendIPConfigurations: [
      {
        name: /*${8:'name'}*/'name'
        properties: {
          publicIPAddress: {
            id: /*${9:'id'}*/'id'
          }
        }
      }
    ]
    frontendPorts: [
      {
        name: /*${10:'name'}*/'name'
        properties: {
          port: /*${11:'port'}*/'port'
        }
      }
    ]
    backendAddressPools: [
      {
        name: /*${12:'name'}*/'name'
      }
    ]
    backendHttpSettingsCollection: [
      {
        name: /*${13:'name'}*/'name'
        properties: {
          port: /*${14:'port'}*/'port'
          protocol: /*'${15|Http,Https|}'*/'Http'
          cookieBasedAffinity: 'Disabled'
        }
      }
    ]
    httpListeners: [
      {
        name: /*${16:'name'}*/'name'
        properties: {
          frontendIPConfiguration: {
            id: /*${17:'id'}*/'id'
          }
          frontendPort: {
            id: /*${18:'id'}*/'id'
          }
          protocol: /*'${19|Http,Https|}'*/'Http'
          sslCertificate: null
        }
      }
    ]
    requestRoutingRules: [
      {
        name: /*${20:'name'}*/'name'
        properties: {
          ruleType: /*'${21|Basic,PathBasedRouting|}'*/'Basic'
          httpListener: {
            id: /*${22:'id'}*/'id'
          }
          backendAddressPool: {
            id: /*${23:'id'}*/'id'
          }
          backendHttpSettings: {
            id: /*${24:'id'}*/'id'
          }
        }
      }
    ]
  }
}
