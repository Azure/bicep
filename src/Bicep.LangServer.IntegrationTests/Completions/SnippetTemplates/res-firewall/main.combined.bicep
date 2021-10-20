// $1 = firewall
// $2 = 'name'
// $3 = 'name'
// $4 = 100
// $5 = Allow
// $6 = 'name'
// $7 = 'description'
// $8 = 'sourceAddress'
// $9 = Http
// $10 = 80
// $11 = 'www.microsoft.com'
// $12 = 'name'
// $13 = 100
// $14 = Dnat
// $15 = 'name'
// $16 = 'description'
// $17 = 'sourceAddress'
// $18 = 'destinationAddress'
// $19 = '80'
// $20 = TCP
// $21 = 'translatedAddress'
// $22 = '80'
// $23 = 'name'
// $24 = 100
// $25 = Deny
// $26 = 'name'
// $27 = 'description'
// $28 = 'sourceAddress'
// $29 = 'destinationAddress'
// $30 = '80'
// $31 = TCP
// $32 = 'name'
// $33 = 'id'
// $34 = 'id'

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
        name: 'name'
        properties: {
          priority: 100
          action: {
            type: 'Deny'
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
            }
          ]
        }
      }
    ]
    ipConfigurations: [
      {
        name: 'name'
        properties: {
          subnet: {
            id: 'id'
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

