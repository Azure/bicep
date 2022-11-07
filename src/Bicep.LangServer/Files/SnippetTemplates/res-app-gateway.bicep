// Application Gateway
resource /*${1:applicationGateway}*/applicationGateway 'Microsoft.Network/applicationGateways@2020-11-01' = {
  name: /*${2:'name'}*/'name'
  location: /*${3:location}*/'location'
  properties: {
    sku: {
      name: /*'${4|Standard_Small,Standard_Medium,Standard_Large,WAF_Medium,WAF_Large,Standard_v2,WAF_v2|}'*/'Standard_Small'
      tier: /*'${5|Standard,WAF,Standard_v2,WAF_v2|}'*/'Standard'
      capacity: /*${6:'capacity'}*/'capacity'
    }
    gatewayIPConfigurations: [
      {
        name: /*${7:'name'}*/'name'
        properties: {
          subnet: {
            id: /*${8:'id'}*/'id'
          }
        }
      }
    ]
    frontendIPConfigurations: [
      {
        name: /*${9:'name'}*/'name'
        properties: {
          publicIPAddress: {
            id: /*${10:'id'}*/'id'
          }
        }
      }
    ]
    frontendPorts: [
      {
        name: /*${11:'name'}*/'name'
        properties: {
          port: /*${12:'port'}*/'port'
        }
      }
    ]
    backendAddressPools: [
      {
        name: /*${13:'name'}*/'name'
      }
    ]
    backendHttpSettingsCollection: [
      {
        name: /*${14:'name'}*/'name'
        properties: {
          port: /*${15:'port'}*/'port'
          protocol: /*'${16|Http,Https|}'*/'Http'
          cookieBasedAffinity: 'Disabled'
        }
      }
    ]
    httpListeners: [
      {
        name: /*${17:'name'}*/'name'
        properties: {
          frontendIPConfiguration: {
            id: /*${18:'id'}*/'id'
          }
          frontendPort: {
            id: /*${19:'id'}*/'id'
          }
          protocol: /*'${20|Http,Https|}'*/'Http'
          sslCertificate: null
        }
      }
    ]
    requestRoutingRules: [
      {
        name: /*${21:'name'}*/'name'
        properties: {
          ruleType: /*'${22|Basic,PathBasedRouting|}'*/'Basic'
          httpListener: {
            id: /*${23:'id'}*/'id'
          }
          backendAddressPool: {
            id: /*${24:'id'}*/'id'
          }
          backendHttpSettings: {
            id: /*${25:'id'}*/'id'
          }
        }
      }
    ]
  }
}
