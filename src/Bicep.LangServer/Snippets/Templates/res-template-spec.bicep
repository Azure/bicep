// Template spec
resource ${1:'templateSpec'} 'Microsoft.Resources/templateSpecs@2019-06-01-preview' = {
  name: ${2:'name'}
  location: resourceGroup().location
  properties: {
    description: ${3:'description'}
    displayName: ${4:'displayName'}
  }
}
