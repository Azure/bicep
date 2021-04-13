// Log Analytics Workspace
resource logAnalyticsWorkspace 'Microsoft.OperationalInsights/workspaces@2015-11-01-preview' = {
  name: ${1:logAnalyticsWorkspace}
  location: resourceGroup().location
  properties: {
    sku: {
      name: ${2|Free,Standard,Premium,Unlimited,PerNode,PerGB2018,Standalone|}
    }
  }
}