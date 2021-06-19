// $1 = logAnalyticsWorkspace
// $2 = 'name'
// $3 = 'Free'

resource logAnalyticsWorkspace 'Microsoft.OperationalInsights/workspaces@2020-10-01' = {
  name: 'name'
  location: resourceGroup().location
  properties: {
    sku: {
      name: 'Free'
    }
  }
}
// Insert snippet here

