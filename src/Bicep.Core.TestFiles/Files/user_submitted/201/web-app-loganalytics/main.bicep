param skuName string = 'S1'
param skuCapacity int = 1
param location string = resourceGroup().location
param appName string = uniqueString(resourceGroup().id)

var appServicePlanName = toLower('asp-${appName}')
var webSiteName = toLower('wapp-${appName}')
var appInsightName = toLower('appi-${appName}')
var logAnalyticsName = toLower('la-${appName}')

resource appServicePlan 'Microsoft.Web/serverfarms@2020-06-01' = {
  name: appServicePlanName // Globally unique storage account name
  location: location // Azure Region
  sku: {
    name: skuName
    capacity: skuCapacity
  }
  tags: {
    displayName: 'HostingPlan'
    ProjectName: appName
  }
}

resource appService 'Microsoft.Web/sites@2020-06-01' = {
  name: webSiteName
  location: location
  identity: {
    type: 'SystemAssigned'
  }
  tags: {
    displayName: 'Website'
    ProjectName: appName
  }
  properties: {
    serverFarmId: appServicePlan.id
    httpsOnly: true
    siteConfig: {
      minTlsVersion: '1.2'
    }
  }
}
resource appServiceLogging 'Microsoft.Web/sites/config@2020-06-01' = {
  name: '${appService.name}/logs'
  properties: {
    applicationLogs: {
      fileSystem: {
        level: 'Warning'
      }
    }
    httpLogs: {
      fileSystem: {
        retentionInMb: 40
        enabled: true
      }
    }
    failedRequestsTracing: {
      enabled: true
    }
    detailedErrorMessages: {
      enabled: true
    }
  }
}

resource appServiceAppSettings 'Microsoft.Web/sites/config@2020-06-01' = {
  name: '${appService.name}/appsettings'
  properties: {
    APPINSIGHTS_INSTRUMENTATIONKEY: appInsights.properties.InstrumentationKey
  }
  dependsOn: [
    appInsights
    appServiceSiteExtension
  ]
}
resource appServiceSiteExtension 'Microsoft.Web/sites/siteextensions@2020-06-01' = {
  name: '${appService.name}/Microsoft.ApplicationInsights.AzureWebsites'
  dependsOn: [
    appInsights
  ]
}
resource appInsights 'microsoft.insights/components@2020-02-02-preview' = {
  name: appInsightName
  location: location
  kind: 'string'
  tags: {
    displayName: 'AppInsight'
    ProjectName: appName
  }
  properties: {
    Application_Type: 'web'
    WorkspaceResourceId: logAnalyticsWorkspace.id
  }
}

resource logAnalyticsWorkspace 'Microsoft.OperationalInsights/workspaces@2020-03-01-preview' = {
  name: logAnalyticsName
  location: location
  tags: {
    displayName: 'Log Analytics'
    ProjectName: appName
  }
  properties: {
    sku: {
      name: 'PerGB2018'
    }
    retentionInDays: 120
  }
}
