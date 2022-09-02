// Template spec
resource /*${1:templateSpec}*/templateSpec 'Microsoft.Resources/templateSpecs@2021-05-01' = {
  name: /*${2:'name'}*/'name'
  location: /*${3:location}*/'location'
  properties: {
    description: /*${4:'description'}*/'description'
    displayName: /*${5:'displayName'}*/'displayName'
  }
}
