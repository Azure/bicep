param appName string = uniqueString(resourceGroup().id)
param logging bool = false

module webappModule './webapp.bicep' = {
  name: 'webAppPlanDeploy'
  params: {
    appName: appName
  }
}

module loggingModule './logging.bicep' = if (logging) {
  name: 'loggingDeploy'
  params: {
    appName: webappModule.outputs.appServiceName
  }
}
