// Log Analytics Workspace
resource /*${1:logAnalyticsWorkspace}*/logAnalyticsWorkspace 'Microsoft.OperationalInsights/workspaces@2020-10-01' = {
  name: /*${2:'name'}*/'name'
  location: location
  properties: {
    sku: {
      name: /*${3|'Free','Standard','Premium','Unlimited','PerNode','PerGB2018','Standalone'|}*/'Free'
    }
  }
}
