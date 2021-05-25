resource logicApp 'Microsoft.Logic/integrationAccounts@2016-06-01' = {
  name: 'name'
  location: resourceGroup().location
  properties: {
    definition: {
      '$schema': 'https://schema.management.azure.com/schemas/2016-06-01/Microsoft.Logic.json'
//@[17:94) [no-hardcoded-env-urls (Warning)] Environment URLs should not be hardcoded. Access URLs via the environment() function to keep references current. -- Found: 'management.azure.com'\n[See : https://aka.ms/bicep/linter/no-hardcoded-env-urls] |'https://schema.management.azure.com/schemas/2016-06-01/Microsoft.Logic.json'|
      contentVersion: '1.0.0.0'
    }
  }
}

