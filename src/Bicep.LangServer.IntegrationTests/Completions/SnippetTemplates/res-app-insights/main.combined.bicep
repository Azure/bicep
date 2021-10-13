// $1 = appInsightsComponents
// $2 = 'name'
// $3 = web

resource appInsightsComponents 'Microsoft.Insights/components@2020-02-02' = {
  name: 'name'
  location: resourceGroup().location
  kind: 'web'
  properties: {
    Application_Type: 'web'
  }
}
// Insert snippet here

