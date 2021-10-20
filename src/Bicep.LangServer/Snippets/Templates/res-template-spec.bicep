// Template spec
resource /*${1:templateSpec}*/templateSpec 'Microsoft.Resources/templateSpecs@2019-06-01-preview' = {
  name: /*${2:'name'}*/'name'
  location: location
  properties: {
    description: /*${3:'description'}*/'description'
    displayName: /*${4:'displayName'}*/'displayName'
  }
}
