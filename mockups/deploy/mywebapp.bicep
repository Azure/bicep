@description('The name of the web app that you wish to create.')
param siteName string

@description('The name of the App Service plan to use for hosting the web app.')
param hostingPlanName string

@description('The pricing tier for the hosting plan.')
param sku string

@description('The instance size of the hosting plan (small, medium, or large).')
param workerSize string

@description('The URL for the GitHub repository that contains the project to deploy.')
@secure()
param servicePackageLink string

var resourceLocation = resourceGroup().location

resource hostingPlan 'Microsoft.Web/serverfarms@2015-04-01' = {
  name: hostingPlanName
  location: resourceLocation
  properties: {
    name: hostingPlanName
    sku: sku
    workerSize: workerSize
    numberOfWorkers: 1
  }
}

resource site 'Microsoft.Web/sites@2015-04-01' = {
  name: siteName
  location: resourceLocation
  properties: {
    serverFarmId: hostingPlanName
  }
  dependsOn: [
    hostingPlan
  ]
}

resource siteName_MSDeploy 'Microsoft.Web/sites/Extensions@2014-06-01' = {
  parent: site
  name: 'MSDeploy'
  properties: {
    packageUri: servicePackageLink
  }
}
