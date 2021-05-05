resource automationAccount 'Microsoft.Automation/automationAccounts@2015-10-31' = {
  name: 'name'
}

resource automationVariable 'Microsoft.Automation/automationAccounts/variables@2015-10-31' = {
  parent: automationAccount
  name: 'name'
  properties: {
    value: 'value'
    description: 'description'
    isEncrypted: true
  }
}

