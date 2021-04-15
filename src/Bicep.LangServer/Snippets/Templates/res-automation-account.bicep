// Automation Account
resource automationAccount 'Microsoft.Automation/automationAccounts@2020-01-13-preview' = {
  name: ${1:automationAccount}
  location: resourceGroup().location
  properties: {
    sku: {
      name: ${2|Free,Basic|}
    }
  }
}
