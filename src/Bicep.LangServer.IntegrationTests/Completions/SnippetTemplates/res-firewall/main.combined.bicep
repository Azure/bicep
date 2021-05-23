resource firewall 'Microsoft.Network/azureFirewalls@2020-11-01' = {
  name: 'name'
  location: resourceGroup().location
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
            id: resourceId('virtualNetworkResourceGroup', 'Microsoft.Network/virtualNetworks/subnets', 'virtualNetworkName', 'AzureFirewallSubnet')
          }
          publicIPAddress: {
            id: 'id'
          }
        }
      }
    ]
  }
}

