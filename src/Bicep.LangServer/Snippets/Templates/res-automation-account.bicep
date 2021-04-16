// Automation Account
resource automationAccount 'Microsoft.Automation/automationAccounts@2019-06-01' = {
  name: ${1:automationAccount}
  location: resourceGroup().location
  properties: {
    sku: {
      name: ${2|Free,Basic|}
    }
  }
}
