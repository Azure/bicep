// $1 = logicApp
// $2 = 'name'
// $3 = 'logicapp.json'

resource logicApp 'Microsoft.Logic/workflows@2019-05-01' = {
  name: 'name'
  location: resourceGroup().location
  properties: {
    definition: json(loadTextContent('logicapp.json')).definition
  }
}
// Insert snippet here

