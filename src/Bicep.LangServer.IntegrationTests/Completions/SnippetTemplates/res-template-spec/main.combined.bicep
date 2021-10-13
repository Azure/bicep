// $1 = templateSpec
// $2 = 'name'
// $3 = 'description'
// $4 = 'displayName'

resource templateSpec 'Microsoft.Resources/templateSpecs@2021-05-01' = {
  name: 'name'
  location: resourceGroup().location
  properties: {
    description: 'description'
    displayName: 'displayName'
  }
}
// Insert snippet here

