resource templateSpec 'Microsoft.Resources/templateSpecs@2019-06-01-preview' = {
  name: 'templateSpec'
  location: resourceGroup().location
  properties: {
    description: 'description'
    displayName: 'displayName'
  }
}

