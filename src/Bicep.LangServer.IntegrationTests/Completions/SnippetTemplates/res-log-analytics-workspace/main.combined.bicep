resource logAnalyticsWorkspace 'Microsoft.OperationalInsights/workspaces@2015-11-01-preview' = {
  name: 'testLogAnalyticsWorkspace'
  location: resourceGroup().location
  properties: {
    sku: {
      name: 'Free'
    }
  }
}
