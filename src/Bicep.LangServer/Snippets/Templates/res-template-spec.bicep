// Template spec
resource templateSpec 'Microsoft.Resources/templateSpecs@2019-06-01-preview' = {
  name: ${1:'templateSpec'}
  location: resourceGroup().location
  properties: {
    description: ${2:'description'}
    displayName: ${3:'displayName'}
  }
}
