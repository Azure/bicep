resource automationAccount 'Microsoft.Automation/automationAccounts@2019-06-01' = {
  name: 'testAutomationAccount'
  location: resourceGroup().location
  properties: {
    sku: {
      name: 'Basic'
    }
  }
}

