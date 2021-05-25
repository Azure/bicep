resource automationAccount 'Microsoft.Automation/automationAccounts@2019-06-01' = {
  name: 'name'
}

resource automationVariable 'Microsoft.Automation/automationAccounts/variables@2019-06-01' = {
  parent: automationAccount
  name: 'name'
  properties: {
    value: 'value'
    description: 'description'
    isEncrypted: true
  }
}

