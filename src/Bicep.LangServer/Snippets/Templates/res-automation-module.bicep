// Automation Module
resource ${1:automationAccountVariable} 'Microsoft.Automation/automationAccounts/modules@2015-10-31' = {
  name: '${automationAccount.name}/${2:name}'
  properties: {
    contentLink: {
      uri: ${3:'https://content-url.nupkg'}
    }
  }
}

resource automationAccount 'Microsoft.Automation/automationAccounts@2015-10-31' = {
  name: ${4:'name'}
}