// Azure Firewall
resource ${1:firewall} 'Microsoft.Network/azureFirewalls@2020-11-01' = {
  name: ${2:'name'}
  location: resourceGroup().location
  properties: {
    applicationRuleCollections: [
      {
        name: ${3:'name'}
        properties: {
          priority: ${4:'priority'}
          action: {
            type: '${5|Allow,Deny|}'
          }
          rules: [
            {
              name: ${6:'name'}
              description: ${7:'description'}
              sourceAddresses: [
                ${8:'sourceAddress'}
              ]
              protocols: [
                {
                  protocolType: '${9|Http,Https,Mssql|}'
                  port: ${10|80,443,1433|}
                }
              ]
              targetFqdns: [
                ${11:'www.microsoft.com'}
              ]
            }
          ]
        }
      }
    ]
    natRuleCollections: [
      {
        name: ${12:'name'}
        properties: {
          priority: ${13:'priority'}
          action: {
            type: '${14|Dnat, Snat|}'
          }
          rules: [
            {
              name: ${15:'name'}
              description: ${16:'description'}
              sourceAddresses: [
                ${17:'sourceAddress'}
              ]
              destinationAddresses: [
                ${18:'destinationAddress'}
              ]
              destinationPorts: [
                ${19:'port'}
              ]
              protocols: [
                '${20|TCP,UDP,Any,ICMP|}'
              ]
              translatedAddress: ${21:'translatedAddress'}
              translatedPort: ${22:'translatedPort'}
            }
          ]
        }
      }
    ]
    networkRuleCollections: [
      {
        name: ${23:'name'}
        properties: {
          priority: ${24:'priority'}
          action: {
            type: '${25|Deny,Allow|}'
          }
          rules: [
            {
              name: ${26:'name'}
              description: ${27:'description'}
              sourceAddresses: [
                ${28:'sourceAddress'}
              ]
              destinationAddresses: [
                ${29:'destinationAddress'}
              ]
              destinationPorts: [
                ${30:'destinationPort'}
              ]
              protocols: [
                '${31|TCP,UDP,Any,ICMP|}'
              ]
            }
          ]
        }
      }
    ]
    ipConfigurations: [
      {
        name: ${32:'name'}
        properties: {
          subnet: {
            id: ${33:'id'}
          }
          publicIPAddress: {
            id: ${34:'id'}
          }
        }
      }
    ]
  }
}
