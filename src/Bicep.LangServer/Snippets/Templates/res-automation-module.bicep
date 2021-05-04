// Automation Module
resource automationAccount 'Microsoft.Automation/automationAccounts@2015-10-31' = {
  name: ${1:'name'}
}

resource ${2:automationAccountVariable} 'Microsoft.Automation/automationAccounts/modules@2015-10-31' = {
  parent: automationAccount
  name: ${3:'name'}
  properties: {
    contentLink: {
      uri: ${4:'https://content-url.nupkg'}
    }
  }
}