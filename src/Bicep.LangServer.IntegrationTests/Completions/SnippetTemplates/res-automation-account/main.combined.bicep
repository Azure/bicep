resource automationAccount 'Microsoft.Automation/automationAccounts@2019-06-01' = {
  name: 'automationAccount'
  location: resourceGroup().location
  properties: {
    sku: {
      name: 'Basic'
    }
  }
}

