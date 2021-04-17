// Template spec
resource templateSpec 'Microsoft.Resources/templateSpecs@2019-06-01-preview' = {
  name: ${1:'templateSpecName'}
  location: resourceGroup().location
  properties: {
    description: ${2:'templateDescription'}
    displayName: ${3:'templateDisplayName'}
  }
}
