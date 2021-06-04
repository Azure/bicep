// $1 = logicApp
// $2 = 'name'

resource logicApp 'Microsoft.Logic/integrationAccounts@2016-06-01' = {
  name: 'name'
  location: resourceGroup().location
  properties: {
    definition: {
      '$schema': 'https://schema.management.azure.com/schemas/2016-06-01/Microsoft.Logic.json'
//@[17:94) [no-hardcoded-env-urls (Warning)] Environment URLs should not be hardcoded. Use the environment() function to ensure compatibility across clouds. Found this disallowed host: "management.azure.com" (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-hardcoded-env-urls)) |'https://schema.management.azure.com/schemas/2016-06-01/Microsoft.Logic.json'|
      contentVersion: '1.0.0.0'
    }
  }
}
// Insert snippet here

