// Template spec
resource ${1:'templateSpec'} 'Microsoft.Resources/templateSpecs@2019-06-01-preview' = {
  name: 'name'
  location: resourceGroup().location
  properties: {
    description: ${2:'description'}
    displayName: ${3:'displayName'}
  }
}
