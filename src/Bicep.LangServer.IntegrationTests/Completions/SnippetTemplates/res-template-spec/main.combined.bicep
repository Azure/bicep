resource templateSpec 'Microsoft.Resources/templateSpecs@2019-06-01-preview' = {
  name: 'name'
  location: resourceGroup().location
  properties: {
    description: 'description'
    displayName: 'displayName'
  }
}

