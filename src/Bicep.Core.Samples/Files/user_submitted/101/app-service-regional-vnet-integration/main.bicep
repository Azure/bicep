@description('The location in which all resources should be deployed.')
param location string = resourceGroup().location

@description('The name of the app to create.')
param appName string = uniqueString(resourceGroup().id)

var appServicePlanName = '${appName}${uniqueString(subscription().subscriptionId)}'
var vnetName = '${appName}vnet'
var vnetAddressPrefix = '10.0.0.0/16'
var subnetName = '${appName}sn'
var subnetAddressPrefix = '10.0.0.0/24'
var appServicePlanSku = 'S1'

resource appServicePlan 'Microsoft.Web/serverfarms@2020-06-01' = {
  name: appServicePlanName
  location: location
  sku: {
    name: appServicePlanSku
  }
  kind: 'app'
}

resource vnet 'Microsoft.Network/virtualNetworks@2020-06-01' = {
  name: vnetName
  location: location
  properties: {
    addressSpace: {
      addressPrefixes: [vnetAddressPrefix]
    }
    subnets: [
      {
        name: subnetName
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
          networkSecurityGroup: {
            properties: {
              securityRules: [
                {
                  properties: {
                    direction: 'Inbound'
                    protocol: '*'
                    access: 'Allow'
                  }
                }
                {
                  properties: {
                    direction: 'Outbound'
                    protocol: '*'
                    access: 'Allow'
                  }
                }
              ]
            }
          }
        }
      }
    ]
  }
}

resource webApp 'Microsoft.Web/sites@2020-06-01' = {
  name: appName
  location: location
  kind: 'app'
  properties: {
    serverFarmId: appServicePlan.id
  }
  dependsOn: [vnet]
}

resource webappVnet 'Microsoft.Web/sites/networkConfig@2020-06-01' = {
  parent: webApp
  name: 'virtualNetwork'
  properties: {
    subnetResourceId: vnet.properties.subnets[0].id
    swiftSupported: true
  }
}
