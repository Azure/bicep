// Logic App
resource /*${1:logicApp}*/logicApp 'Microsoft.Logic/workflows@2019-05-01' = {
  name: /*${2:'name'}*/'name'
  location: /*${3:location}*/'location'
  properties: {
    definition: json(loadTextContent(/*${4:'REQUIRED'}*/'REQUIRED')).definition
  }
}
