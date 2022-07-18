// $1 = appInsightsComponents
// $2 = 'name'
// $3 = location
// $4 = web

param location string

resource appInsightsComponents 'Microsoft.Insights/components@2020-02-02' = {
  name: 'name'
  location: location
  kind: 'web'
  properties: {
    Application_Type: 'web'
  }
}
// Insert snippet here

