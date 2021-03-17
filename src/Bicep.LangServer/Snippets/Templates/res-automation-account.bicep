resource automationAccount 'Microsoft.Automation/automationAccounts@2015-10-31' = {
  name: '${1:automationAccount}'
  location: resourceGroup().location
  tags: {
    displayName: '${1:automationAccount}'
  }
  properties: {
    sku: {
      name: '${2|Free,Basic|}'
    }
  }
}