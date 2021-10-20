// Azure Firewall
resource /*${1:firewall}*/firewall 'Microsoft.Network/azureFirewalls@2020-11-01' = {
  name: /*${2:'name'}*/'name'
  location: location
  properties: {
    applicationRuleCollections: [
      {
        name: /*${3:'name'}*/'name'
        properties: {
          priority: /*${4:'priority'}*/'priority'
          action: {
            type: /*'${5|Allow,Deny|}'*/'Allow'
          }
          rules: [
            {
              name: /*${6:'name'}*/'name'
              description: /*${7:'description'}*/'description'
              sourceAddresses: [
                /*${8:'sourceAddress'}*/'sourceAddress'
              ]
              protocols: [
                {
                  protocolType: /*'${9|Http,Https,Mssql|}'*/'Http'
                  port: /*${10|80,443,1433|}*/80
                }
              ]
              targetFqdns: [
                /*${11:'www.microsoft.com'}*/'www.microsoft.com'
              ]
            }
          ]
        }
      }
    ]
    natRuleCollections: [
      {
        name: /*${12:'name'}*/'name'
        properties: {
          priority: /*${13:'priority'}*/'priority'
          action: {
            type: /*'${14|Dnat, Snat|}'*/'Dnat'
          }
          rules: [
            {
              name: /*${15:'name'}*/'name'
              description: /*${16:'description'}*/'description'
              sourceAddresses: [
                /*${17:'sourceAddress'}*/'sourceAddress'
              ]
              destinationAddresses: [
                /*${18:'destinationAddress'}*/'destinationAddress'
              ]
              destinationPorts: [
                /*${19:'port'}*/'port'
              ]
              protocols: [
                /*'${20|TCP,UDP,Any,ICMP|}'*/'TCP'
              ]
              translatedAddress: /*${21:'translatedAddress'}*/'translatedAddress'
              translatedPort: /*${22:'translatedPort'}*/'translatedPort'
            }
          ]
        }
      }
    ]
    networkRuleCollections: [
      {
        name: /*${23:'name'}*/'name'
        properties: {
          priority: /*${24:'priority'}*/'priority'
          action: {
            type: /*'${25|Deny,Allow|}'*/'Deny'
          }
          rules: [
            {
              name: /*${26:'name'}*/'name'
              description: /*${27:'description'}*/'description'
              sourceAddresses: [
                /*${28:'sourceAddress'}*/'sourceAddress'
              ]
              destinationAddresses: [
                /*${29:'destinationAddress'}*/'destinationAddress'
              ]
              destinationPorts: [
                /*${30:'destinationPort'}*/'destinationPort'
              ]
              protocols: [
                /*'${31|TCP,UDP,Any,ICMP|}'*/'TCP'
              ]
            }
          ]
        }
      }
    ]
    ipConfigurations: [
      {
        name: /*${32:'name'}*/'name'
        properties: {
          subnet: {
            id: /*${33:'id'}*/'id'
          }
          publicIPAddress: {
            id: /*${34:'id'}*/'id'
          }
        }
      }
    ]
  }
}
