resource automationAccount 'Microsoft.Automation/automationAccounts@2020-01-13-preview' = {
  name: 'testAutomationAccount'
  location: resourceGroup().location
  properties: {
    sku: {
      name: 'Basic'
    }
  }
}
