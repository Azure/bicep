// Logic App
resource /*${1:logicApp}*/logicApp 'Microsoft.Logic/workflows@2019-05-01' = {
  name: /*${2:'name'}*/'name'
  location: location
  properties: {
    definition: {
      '$schema': 'https://schema.management.azure.com/schemas/2016-06-01/Microsoft.Logic.json'
      contentVersion: '1.0.0.0'
    }
  }
}
