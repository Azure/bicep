// Logic App
resource /*${1:logicApp}*/logicApp 'Microsoft.Logic/workflows@2019-05-01' = {
  name: /*${2:'name'}*/'name'
  location: location
  properties: {
    definition: json(loadTextContent(/*${3:'REQUIRED'}*/'REQUIRED')).definition
  }
}
