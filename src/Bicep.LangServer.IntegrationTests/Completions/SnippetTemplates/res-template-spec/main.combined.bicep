// $1 = templateSpec
// $2 = 'name'
// $3 = 'description'
// $4 = 'displayName'

resource templateSpec 'Microsoft.Resources/templateSpecs@2019-06-01-preview' = {
  name: 'name'
  location: resourceGroup().location
  properties: {
    description: 'description'
    displayName: 'displayName'
  }
}
// Insert snippet here

