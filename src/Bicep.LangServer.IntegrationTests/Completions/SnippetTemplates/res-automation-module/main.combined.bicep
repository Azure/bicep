resource automationAccount_automationVariable 'Microsoft.Automation/automationAccounts/modules@2015-10-31' = {
  name: '${automationAccount.name}/testAutomationVariable'
  properties: {
    contentLink: {
      uri: 'https://test-content-url.nupkg'
    }
  }
}

resource automationAccount 'Microsoft.Automation/automationAccounts@2015-10-31' = {
  name: 'testAutomationModule'
}
