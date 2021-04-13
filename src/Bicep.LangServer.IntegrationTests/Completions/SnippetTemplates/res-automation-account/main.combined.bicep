resource automationAccount 'Microsoft.Automation/automationAccounts@2015-10-31' = {
  name: 'testAutomationAccount'
  location: resourceGroup().location
  properties: {
    sku: {
      name: 'Basic'
    }
  }
}
