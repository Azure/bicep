// $1 = logicApp
// $2 = 'name'

resource logicApp 'Microsoft.Logic/integrationAccounts@2016-06-01' = {
  name: 'name'
  location: resourceGroup().location
}
// Insert snippet here

