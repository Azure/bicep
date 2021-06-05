// $1 = automationAccount
// $2 = 'name'
// $3 = 'Basic'

resource automationAccount 'Microsoft.Automation/automationAccounts@2019-06-01' = {
  name: 'name'
  location: resourceGroup().location
  properties: {
    sku: {
      name: 'Basic'
    }
  }
}
// Insert snippet here

