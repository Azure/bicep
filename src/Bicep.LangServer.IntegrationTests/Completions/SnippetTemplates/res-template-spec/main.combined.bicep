// $1 = templateSpec
// $2 = 'name'
// $3 = 'description'
// $4 = 'displayName'

param location string

resource templateSpec 'Microsoft.Resources/templateSpecs@2019-06-01-preview' = {
  name: 'name'
  location: location
  properties: {
    description: 'description'
    displayName: 'displayName'
  }
}
// Insert snippet here

