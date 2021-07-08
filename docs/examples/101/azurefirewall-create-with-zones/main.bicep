param virtualNetworkName string = 'test-vnet'
param vnetAddressPrefix string = '10.0.0.0/16'
param azureFirewallSubnetAddressPrefix string = '10.0.1.0/24'
param firewallName string = 'firewall1'
param location string = resourceGroup().location
param availabilityZones array = [
  '1'
  '2'
  '3'
]

var azureFirewallSubnetName = 'AzureFirewallSubnet'

resource virtualNetwork 'Microsoft.Network/virtualNetworks@2020-06-01' = {
  name: virtualNetworkName
  location: location
  properties: {
    addressSpace: {
      addressPrefixes: [
        vnetAddressPrefix
      ]
    }
  }
}

resource subnet 'Microsoft.Network/virtualNetworks/subnets@2020-06-01' = {
  name: '${virtualNetwork.name}/${azureFirewallSubnetName}'
  properties: {
    addressPrefix: azureFirewallSubnetAddressPrefix
  }
}

resource publicIP 'Microsoft.Network/publicIPAddresses@2020-06-01' = {
  name: 'publicIp1'
  location: location
  sku: {
    name: 'Standard'
  }
  properties: {
    publicIPAllocationMethod: 'Static'
    publicIPAddressVersion: 'IPv4'
  }
}

resource firewall 'Microsoft.Network/azureFirewalls@2020-06-01' = {
  name: firewallName
  location: location
  zones: length(availabilityZones) == 0 ? null : availabilityZones
  properties: {
    ipConfigurations: [
      {
        name: 'IpConf1'
        properties: {
          subnet: {
            id: subnet.id
          }
          publicIPAddress: {
            id: publicIP.id
          }
        }
      }
    ]
    applicationRuleCollections: [
      {
        name: 'appRc1'
        properties: {
          priority: 101
          action: {
            type: 'Allow'
          }
          rules: [
            {
              name: 'appRule1'
              protocols: [
                {
                  port: 80
                  protocolType: 'Http'
                }
              ]
              targetFqdns: [
                'www.microsoft.com'
              ]
              sourceAddresses: [
                '10.0.0.0/24'
              ]
            }
          ]
        }
      }
    ]
    networkRuleCollections: [
      {
        name: 'netRc1'
        properties: {
          priority: 200
          action: {
            type: 'Allow'
          }
          rules: [
            {
              name: 'netRule1'
              protocols: [
                'TCP'
              ]
              sourceAddresses: [
                '10.0.0.0/24'
              ]
              destinationAddresses: [
                '*'
              ]
              destinationPorts: [
                '8000-8999'
              ]
            }
          ]
        }
      }
    ]
  }
}
