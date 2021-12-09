// Azure Firewall
resource /*${1:firewall}*/firewall 'Microsoft.Network/azureFirewalls@2020-11-01' = {
  name: /*${2:'name'}*/'name'
  location: /*${3:location}*/'location'
  properties: {
    applicationRuleCollections: [
      {
        name: /*${4:'name'}*/'name'
        properties: {
          priority: /*${5:'priority'}*/'priority'
          action: {
            type: /*'${6|Allow,Deny|}'*/'Allow'
          }
          rules: [
            {
              name: /*${7:'name'}*/'name'
              description: /*${8:'description'}*/'description'
              sourceAddresses: [
                /*${9:'sourceAddress'}*/'sourceAddress'
              ]
              protocols: [
                {
                  protocolType: /*'${10|Http,Https,Mssql|}'*/'Http'
                  port: /*${11|80,443,1433|}*/80
                }
              ]
              targetFqdns: [
                /*${12:'www.microsoft.com'}*/'www.microsoft.com'
              ]
            }
          ]
        }
      }
    ]
    natRuleCollections: [
      {
        name: /*${13:'name'}*/'name'
        properties: {
          priority: /*${14:'priority'}*/'priority'
          action: {
            type: /*'${15|Dnat, Snat|}'*/'Dnat'
          }
          rules: [
            {
              name: /*${16:'name'}*/'name'
              description: /*${17:'description'}*/'description'
              sourceAddresses: [
                /*${18:'sourceAddress'}*/'sourceAddress'
              ]
              destinationAddresses: [
                /*${19:'destinationAddress'}*/'destinationAddress'
              ]
              destinationPorts: [
                /*${20:'port'}*/'port'
              ]
              protocols: [
                /*'${21|TCP,UDP,Any,ICMP|}'*/'TCP'
              ]
              translatedAddress: /*${22:'translatedAddress'}*/'translatedAddress'
              translatedPort: /*${23:'translatedPort'}*/'translatedPort'
            }
          ]
        }
      }
    ]
    networkRuleCollections: [
      {
        name: /*${24:'name'}*/'name'
        properties: {
          priority: /*${25:'priority'}*/'priority'
          action: {
            type: /*'${26|Deny,Allow|}'*/'Deny'
          }
          rules: [
            {
              name: /*${27:'name'}*/'name'
              description: /*${28:'description'}*/'description'
              sourceAddresses: [
                /*${29:'sourceAddress'}*/'sourceAddress'
              ]
              destinationAddresses: [
                /*${30:'destinationAddress'}*/'destinationAddress'
              ]
              destinationPorts: [
                /*${31:'destinationPort'}*/'destinationPort'
              ]
              protocols: [
                /*'${32|TCP,UDP,Any,ICMP|}'*/'TCP'
              ]
            }
          ]
        }
      }
    ]
    ipConfigurations: [
      {
        name: /*${33:'name'}*/'name'
        properties: {
          subnet: {
            id: /*${34:'id'}*/'id'
          }
          publicIPAddress: {
            id: /*${35:'id'}*/'id'
          }
        }
      }
    ]
  }
}
