// $1 = automationAccount
// $2 = 'name'
// $3 = location
// $4 = 'Basic'

param location string

resource automationAccount 'Microsoft.Automation/automationAccounts@2019-06-01' = {
  name: 'name'
  location: location
  properties: {
    sku: {
      name: 'Basic'
    }
  }
}
// Insert snippet here

