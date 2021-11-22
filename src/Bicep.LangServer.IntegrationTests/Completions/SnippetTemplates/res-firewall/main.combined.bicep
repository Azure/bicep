// $1 = firewall
// $2 = 'name'
// $3 = location
// $4 = 'name'
// $5 = 100
// $6 = Allow
// $7 = 'name'
// $8 = 'description'
// $9 = 'sourceAddress'
// $10 = Http
// $11 = 80
// $12 = 'www.microsoft.com'
// $13 = 'name'
// $14 = 100
// $15 = Dnat
// $16 = 'name'
// $17 = 'description'
// $18 = 'sourceAddress'
// $19 = 'destinationAddress'
// $20 = '80'
// $21 = TCP
// $22 = 'translatedAddress'
// $23 = '80'
// $24 = 'name'
// $25 = 100
// $26 = Deny
// $27 = 'name'
// $28 = 'description'
// $29 = 'sourceAddress'
// $30 = 'destinationAddress'
// $31 = '80'
// $32 = TCP
// $33 = 'name'
// $34 = 'id'
// $35 = 'id'

param location string

resource firewall 'Microsoft.Network/azureFirewalls@2020-11-01' = {
  name: 'name'
  location: location
  properties: {
    applicationRuleCollections: [
      {
        name: 'name'
        properties: {
          priority: 100
          action: {
            type: 'Allow'
          }
          rules: [
            {
              name: 'name'
              description: 'description'
              sourceAddresses: [
                'sourceAddress'
              ]
              protocols: [
                {
                  protocolType: 'Http'
                  port: 80
                }
              ]
              targetFqdns: [
                'www.microsoft.com'
              ]
            }
          ]
        }
      }
    ]
    natRuleCollections: [
      {
        name: 'name'
        properties: {
          priority: 100
          action: {
            type: 'Dnat'
          }
          rules: [
            {
              name: 'name'
              description: 'description'
              sourceAddresses: [
                'sourceAddress'
              ]
              destinationAddresses: [
                'destinationAddress'
              ]
              destinationPorts: [
                '80'
              ]
              protocols: [
                'TCP'
              ]
              translatedAddress: 'translatedAddress'
              translatedPort: '80'
            }
          ]
        }
      }
    ]
    networkRuleCollections: [
      {
        name: '80'
        properties: {
          priority: 'name'
          action: {
            type: '100'
          }
          rules: [
            {
              name: Deny
//@[20:24) [BCP057 (Error)] The name "Deny" does not exist in the current context. (CodeDescription: none) |Deny|
              description: 'name'
              sourceAddresses: [
                'description'
              ]
              destinationAddresses: [
                'sourceAddress'
              ]
              destinationPorts: [
                'destinationAddress'
              ]
              protocols: [
                ''80''
//@[18:20) [BCP019 (Error)] Expected a new line character at this location. (CodeDescription: none) |80|
              ]
            }
          ]
        }
      }
    ]
    ipConfigurations: [
      {
        name: TCP
//@[14:17) [BCP057 (Error)] The name "TCP" does not exist in the current context. (CodeDescription: none) |TCP|
        properties: {
          subnet: {
            id: 'name'
          }
          publicIPAddress: {
            id: 'id'
          }
        }
      }
    ]
  }
}
// Insert snippet here

