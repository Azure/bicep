// Log Analytics Workspace
resource /*${1:logAnalyticsWorkspace}*/logAnalyticsWorkspace 'Microsoft.OperationalInsights/workspaces@2020-10-01' = {
  name: /*${2:'name'}*/'name'
  location: /*${3:location}*/'location'
  properties: {
    sku: {
      name: /*${4|'Free','Standard','Premium','Unlimited','PerNode','PerGB2018','Standalone'|}*/'Free'
    }
  }
}
