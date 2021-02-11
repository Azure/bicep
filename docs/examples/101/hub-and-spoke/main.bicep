targetScope = 'subscription'

param region string = 'westeurope'

resource hubrg 'Microsoft.Resources/resourceGroups@2020-06-01' = {
  name: 'hub-rg'
  location: region
}

resource spokerg 'Microsoft.Resources/resourceGroups@2020-06-01' = {
  name: 'spoke-rg'
  location: region
}

module hubVNET 'modules/vnet.bicep' = {
  name: 'hub-vnet'
  scope: hubrg
  params: {
    prefix: 'hub'
    addressSpaces: [
      '192.168.0.0/24'
    ]
    subnets: [
      {
        name: 'AzureFirewallSubnet'
        properties: {
          addressPrefix: '192.168.0.0/25'
        }
      }
    ]
  }
}

module spokeVNET 'modules/vnet.bicep' = {
  name: 'spoke-vnet'
  scope: spokerg
  params: {
    prefix: 'spoke'
    addressSpaces: [
      '192.168.1.0/24'
      '10.0.0.0/23'
    ]
    subnets: [
      {
        name: 'spoke-vnet'
        properties: {
          addressPrefix: '10.0.0.0/24'
          routeTable: {
            id: route.outputs.id
          }
        }
      }
    ]
  }
}

module Hubfwl 'modules/fwl.bicep' = {
  name: 'hub-fwl'
  scope: hubrg
  params: {
    prefix: 'hub'
    hubId: hubVNET.outputs.id
  }
}

module HubToSpokePeering 'modules/peering.bicep' = {
  name: 'hub-to-spoke-peering'
  scope: hubrg
  params: {
    localVnetName: hubVNET.outputs.name
    remoteVnetName: 'spoke'
    remoteVnetId: spokeVNET.outputs.id
  }
}

module SpokeToHubPeering 'modules/peering.bicep' = {
  name: 'spoke-to-hub-peering'
  scope: spokerg
  params: {
    localVnetName: spokeVNET.outputs.name
    remoteVnetName: 'hub'
    remoteVnetId: hubVNET.outputs.id
  }
}

module route 'modules/rot.bicep' = {
  name: 'spoke-rot'
  scope: spokerg
  params: {
    prefix: 'spoke'
    azFwlIp: Hubfwl.outputs.privateIp
  }
}
