param name string = 'site001'
param location string = resourceGroup().location

param acrName string = 'myAcr'
param dockerUsername string = 'adminUser'
param dockerImageAndTag string = 'app/frontend:latest'

// external ACR info
resource containerRegistry 'Microsoft.ContainerRegistry/registries@2019-05-01' existing = {
  name: acrName
}

var websiteName = '${name}-site'

resource site 'microsoft.web/sites@2020-06-01' = {
  name: websiteName
  location: location
  properties: {
    siteConfig: {
      appSettings: [
        {
          name: 'DOCKER_REGISTRY_SERVER_URL'
          value: 'https://${acrName}.azurecr.io'
        }
        {
          name: 'DOCKER_REGISTRY_SERVER_USERNAME'
          value: dockerUsername
        }
        {
          name: 'DOCKER_REGISTRY_SERVER_PASSWORD'
          value: containerRegistry.listCredentials().passwords[0].value
        }
        {
          name: 'WEBSITES_ENABLE_APP_SERVICE_STORAGE'
          value: 'false'
        }
      ]
      linuxFxVersion: 'DOCKER|${acrName}.azurecr.io/${dockerImageAndTag}'
    }
    serverFarmId: farm.id
  }
}

var farmName = '${name}-farm'

resource farm 'microsoft.web/serverFarms@2020-06-01' = {
  name: farmName
  location: location
  sku: {
    name: 'B1'
    tier: 'Basic'
  }
  kind: 'linux'
  properties: {
    targetWorkerSizeId: 0
    targetWorkerCount: 1
    reserved: true
  }
}

output publicUrl string = site.properties.defaultHostName
output ftpUser string = any(site.properties).ftpUsername // TODO: workaround for missing property definition
