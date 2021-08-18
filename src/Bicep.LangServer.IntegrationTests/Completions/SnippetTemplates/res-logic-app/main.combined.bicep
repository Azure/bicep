// $1 = logicApp
// $2 = 'name'

resource logicApp 'Microsoft.Logic/workflows@2019-05-01' = {
  name: 'name'
  location: resourceGroup().location
  properties: {
    definition: {
      '$schema': 'https://schema.management.azure.com/schemas/2016-06-01/Microsoft.Logic.json'
      contentVersion: '1.0.0.0'
    }
  }
}
// Insert snippet here

