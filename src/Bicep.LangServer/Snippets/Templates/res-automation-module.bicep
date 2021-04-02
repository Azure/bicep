// Automation Module
resource automationAccount_automationVariable 'Microsoft.Automation/automationAccounts/modules@2015-10-31' = {
  name: '${automationAccount.name}/automationVariable'
  properties: {
    contentLink: {
      uri: 'https://content-url.nupkg'
    }
  }
}

resource automationAccount 'Microsoft.Automation/automationAccounts@2015-10-31' = {
  name: 'automationAccount'
}