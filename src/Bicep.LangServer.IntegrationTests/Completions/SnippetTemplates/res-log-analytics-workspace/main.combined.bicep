// $1 = logAnalyticsWorkspace
// $2 = 'name'
// $3 = 'Free'

param location string

resource logAnalyticsWorkspace 'Microsoft.OperationalInsights/workspaces@2020-10-01' = {
  name: 'name'
  location: location
  properties: {
    sku: {
      name: 'Free'
    }
  }
}
// Insert snippet here

