param location string = resourceGroup().location
param virtualNetworkName string = 'Vnet'
param accountName string

@allowed([
  'Enabled'
  'Disabled'
])
param publicNetworkAccess string = 'Enabled'

param privateEndpointName string

resource virtualNetwork 'Microsoft.Network/virtualNetworks@2020-06-01' = {
  name: virtualNetworkName
  location: location
  properties: {
    addressSpace: {
      addressPrefixes: [
        '172.20.0.0/16'
      ]
    }
  }
}

resource subNet 'Microsoft.Network/virtualNetworks/subnets@2020-06-01' = {
  name: '${virtualNetwork.name}/default'
  properties: {
    addressPrefix: '172.20.0.0/24'
    privateEndpointNetworkPolicies: 'Disabled'
  }
}

resource databaseAccount 'Microsoft.DocumentDB/databaseAccounts@2020-06-01-preview' = {
  name: accountName
  location: location
  kind: 'GlobalDocumentDB'
  properties: any({
    createMode: 'Default'
    consistencyPolicy: {
      defaultConsistencyLevel: 'Session'
    }
    locations: [
      {
        locationName: location
        failoverPriority: 0
        isZoneRedundant: false
      }
    ]
    databaseAccountOfferType: 'Standard'
    enableAutomaticFailover: false
    enableMultipleWriteLocations: false
    publicNetworkAccess: publicNetworkAccess
  })
}

resource privateEndpoint 'Microsoft.Network/privateEndpoints@2020-06-01' = {
  name: privateEndpointName
  location: location
  properties: {
    subnet: {
      id: subNet.id
    }
    privateLinkServiceConnections: [
      {
        name: 'MyConnection'
        properties: {
          privateLinkServiceId: databaseAccount.id
          groupIds: [
            'Sql'
          ]
        }
      }
    ]
  }
}
