param name string = 'site001'
param location string = resourceGroup().location

param acrName string = 'myAcr'
param dockerUsername string = 'adminUser'
param dockerImageAndTag string = 'app/frontend:latest'

// external ACR info
var containerRegistryId = resourceId('Microsoft.ContainerRegistry/registries', acrName)
var acrApiVersion = '2019-05-01'

var websiteName = '${name}-site'

resource site 'microsoft.web/sites@2018-11-01' = {
  name: websiteName
  location: location
  properties: {
    name: websiteName
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
          value: listCredentials(containerRegistryId, acrApiVersion).passwords[0].value
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

resource farm 'microsoft.web/serverFarms@2018-11-01' = {
  name: farmName
  location: location
  sku: {
    name: 'B1'
    tier: 'Basic'
  }
  kind: 'linux'
  properties: {
    name: farmName
    workerSize: '0'
    workerSizeId: '0'
    numberOfWorkers: '1'
    reserved: true
  }
}

output publicUrl string = site.properties.defaultHostName
output ftpUser string = site.properties.ftpUsername

