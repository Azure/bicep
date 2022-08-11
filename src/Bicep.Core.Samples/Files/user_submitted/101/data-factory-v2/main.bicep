param name string = 'myv2datafactory'

param location string = resourceGroup().location

resource dataFactory 'Microsoft.DataFactory/factories@2018-06-01' = {
  name: name
  location: location
  identity: {
    type: 'SystemAssigned'
  }
  properties: {}
}
