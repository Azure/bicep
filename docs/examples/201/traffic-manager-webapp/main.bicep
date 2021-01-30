param uniqueDnsName string
param uniqueDnsNameForWebApp string
param serverFarmName string
param trafficManagerName string
param location string = resourceGroup().location

resource serverFarm 'Microsoft.Web/serverfarms@2020-06-01' = {
  name: serverFarmName
  location: location
  sku: {
    name: 'S1'
    tier: 'Standard'
  }
}

resource webSite 'Microsoft.Web/sites@2020-06-01' = {
  name: uniqueDnsNameForWebApp
  location: location
  properties: {
    serverFarmId: serverFarm.id
  }
}

resource trafficManagerProfile 'Microsoft.Network/trafficmanagerprofiles@2018-04-01' = {
  name: trafficManagerName
  location: 'global'
  properties: {
    profileStatus: 'Enabled'
    trafficRoutingMethod: 'Priority'
    dnsConfig: {
      relativeName: uniqueDnsName
      ttl: 30
    }
    monitorConfig: {
      protocol: 'HTTPS'
      port: 443
      path: '/'
    }
  }
}

resource azureEndPoint 'Microsoft.Network/trafficManagerProfiles/azureEndpoints@2018-08-01' = {
  name: '${trafficManagerProfile.name}/${uniqueDnsNameForWebApp}'
  location: 'global'
  properties: {
    targetResourceId: webSite.id
    endpointStatus: 'Enabled'
  }
}
