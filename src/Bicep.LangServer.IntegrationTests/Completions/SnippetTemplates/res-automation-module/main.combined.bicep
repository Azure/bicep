resource automationAccount 'Microsoft.Automation/automationAccounts@2015-10-31' = {
  name: 'name'
}

resource automationAccountVariable 'Microsoft.Automation/automationAccounts/modules@2015-10-31' = {
  parent: automationAccount
  name: 'name'
  properties: {
    contentLink: {
      uri: 'https://test-content-url.nupkg'
    }
  }
}
