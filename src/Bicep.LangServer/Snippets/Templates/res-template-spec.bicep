// Template spec
resource /*${1:templateSpec}*/templateSpec 'Microsoft.Resources/templateSpecs@2021-05-01' = {
  name: /*${2:'name'}*/'name'
  location: resourceGroup().location
  properties: {
    description: /*${3:'description'}*/'description'
    displayName: /*${4:'displayName'}*/'displayName'
  }
}
