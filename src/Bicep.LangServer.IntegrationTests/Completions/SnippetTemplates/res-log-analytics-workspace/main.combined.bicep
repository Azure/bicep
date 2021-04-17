resource logAnalyticsWorkspace 'Microsoft.OperationalInsights/workspaces@2020-10-01' = {
  name: 'logAnalyticsWorkspace'
  location: resourceGroup().location
  properties: {
    sku: {
      name: 'Free'
    }
  }
}

