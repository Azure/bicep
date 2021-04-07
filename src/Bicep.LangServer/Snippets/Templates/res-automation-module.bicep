// Automation Module
resource automationAccount_automationVariable 'Microsoft.Automation/automationAccounts/modules@2015-10-31' = {
  name: '${automationAccount.name}/${2:automationVariable}'
  properties: {
    contentLink: {
      uri: '${3:https://content-url.nupkg}'
    }
  }
}

resource automationAccount 'Microsoft.Automation/automationAccounts@2015-10-31' = {
  name: '${1:automationAccount}'
}