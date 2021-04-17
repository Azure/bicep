// Log Analytics Workspace
resource logAnalyticsWorkspace 'Microsoft.OperationalInsights/workspaces@2020-10-01' = {
  name: ${1:'logAnalyticsWorkspace'}
  location: resourceGroup().location
  properties: {
    sku: {
      name: '${2|Free,Standard,Premium,Unlimited,PerNode,PerGB2018,Standalone|}'
    }
  }
}
