resource automationAccount1 'Microsoft.Automation/automationAccounts@2015-10-31' = {
  name: '${1:automationAccount1}'
  location: resourceGroup().location
  tags: {}
  properties: {
    sku: {
      name: '${2|Free,Basic|}'
    }
  }
}