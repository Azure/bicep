// Log Analytics Workspace
resource ${1:'logAnalyticsWorkspace'} 'Microsoft.OperationalInsights/workspaces@2020-10-01' = {
  name: 'name'
  location: resourceGroup().location
  properties: {
    sku: {
      name: '${2|Free,Standard,Premium,Unlimited,PerNode,PerGB2018,Standalone|}'
    }
  }
}
