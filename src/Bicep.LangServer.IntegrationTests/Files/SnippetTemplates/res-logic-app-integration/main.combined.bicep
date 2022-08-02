// $1 = logicApp
// $2 = 'name'
// $3 = location

param location string

resource logicApp 'Microsoft.Logic/integrationAccounts@2016-06-01' = {
  name: 'name'
  location: location
}
// Insert snippet here

