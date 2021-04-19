// Automation Account
resource ${1:'automationAccount'} 'Microsoft.Automation/automationAccounts@2019-06-01' = {
  name: 'name'
  location: resourceGroup().location
  properties: {
    sku: {
      name: '${2|Free,Basic|}'
    }
  }
}
