param serviceName string
param app1Name string = 'gateway'
param app2Name string = 'account-service'
param app3Name string = 'auth-service'
param location string = resourceGroup().location

#disable-next-line BCP081
resource service 'Microsoft.AppPlatform/Spring@2019-05-01-preview' = {
  name: serviceName
  location: location
  sku: {
    name: 'S0'
    tier: 'Standard'
  }
  properties: {
    configServerProperties: {
      configServer: {
        gitProperty: {
          uri: 'https://github.com/Azure-Samples/piggymetrics-config'
        }
      }
    }
  }
}

resource app1 'Microsoft.AppPlatform/Spring/apps@2020-07-01' = {
  name: '${service.name}/${app1Name}'
  properties: {
    public: true
  }
}

resource app1deployment 'Microsoft.AppPlatform/Spring/apps/deployments@2020-07-01' = {
  name: '${app1.name}/default'
  properties: {
    source: {
      relativePath: '<default>'
      type: 'Jar'
    }
  }
}
resource app2 'Microsoft.AppPlatform/Spring/apps@2020-07-01' = {
  name: '${service.name}/${app2Name}'
  properties: {
    public: false
  }
}

resource app2deployment 'Microsoft.AppPlatform/Spring/apps/deployments@2020-07-01' = {
  name: '${app2.name}/default'
  properties: {
    source: {
      relativePath: '<default>'
      type: 'Jar'
    }
  }
}

resource app3 'Microsoft.AppPlatform/Spring/apps@2020-07-01' = {
  name: '${service.name}/${app3Name}'
  properties: {
    public: false
  }
}

resource app3deployment 'Microsoft.AppPlatform/Spring/apps/deployments@2020-07-01' = {
  name: '${app3.name}/default'
  properties: {
    source: {
      relativePath: '<default>'
      type: 'Jar'
    }
  }
}

module activeDeployment './activedeployment.bicep' = {
  name: 'setActiveDeployment'
  params: {
    app1Name: app1.name
    app2Name: app2.name
    app3Name: app3.name
  }
  dependsOn: [
    app1deployment
    app2deployment
    app3deployment
  ]
}
