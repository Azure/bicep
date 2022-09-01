// $1 = logicApp
// $2 = 'name'
// $3 = location
// $4 = 'logicapp.json'

param location string

resource logicApp 'Microsoft.Logic/workflows@2019-05-01' = {
  name: 'name'
  location: location
  properties: {
    definition: json(loadTextContent('logicapp.json')).definition
  }
}
// Insert snippet here

