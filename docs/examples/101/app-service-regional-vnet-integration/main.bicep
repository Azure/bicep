param location string
param nameprefix string

param vnetAddressPrefix string = '10.0.0.0/16'
param subnetAddressPrefix string = '10.0.0.0/24'

resource appsvc 'Microsoft.Web/serverfarms@2020-06-01' = {
  name: '${nameprefix}asp'
  location: location
  sku: {
    name: 'S1'
  }
}

resource vnet 'Microsoft.Network/virtualNetworks@2020-06-01' = {
  name: '${nameprefix}vnet'
  location: location
  properties: {
    addressSpace: {
      addressPrefixes: [
        vnetAddressPrefix
      ]
    }
    subnets: [
      {
        name: '${nameprefix}sn'
        properties: {
          addressPrefix: subnetAddressPrefix
          delegations: [
            {
              name: 'delegation'
              properties: {
                serviceName: 'Microsoft.Web/serverFarms'
              }
            }
          ]
        }
      }
    ]
  }
}

resource webapp1 'Microsoft.Web/sites@2020-06-01' = {
  name: '${nameprefix}wa'
  location: location
  kind: 'app'
  dependsOn: [
    appsvc
    vnet
  ]
  properties: {
    serverFarmId: appsvc.id
  }
}

resource webapp1vnet 'Microsoft.Web/sites/networkConfig@2020-06-01' = {
  name: '${webapp1.name}/virtualNetwork'
  properties: {
    subnetResourceId: resourceId('Microsoft.Network/virtualNetworks/subnets', vnet.name, vnet.properties.subnets[0].name)
  }
}
