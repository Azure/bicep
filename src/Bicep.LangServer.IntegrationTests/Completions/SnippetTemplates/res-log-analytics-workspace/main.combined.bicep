resource logAnalyticsWorkspace 'Microsoft.OperationalInsights/workspaces@2020-10-01' = {
  name: 'name'
  location: resourceGroup().location
  properties: {
    sku: {
      name: 'Free'
    }
  }
}

