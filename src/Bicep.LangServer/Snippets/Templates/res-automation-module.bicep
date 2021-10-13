// Automation Module
resource automationAccount 'Microsoft.Automation/automationAccounts@2019-06-01' = {
  name: /*${1:'name'}*/'name'
}

resource /*${2:automationAccountVariable}*/automationAccountVariable 'Microsoft.Automation/automationAccounts/modules@2019-06-01' = {
  parent: automationAccount
  name: /*${3:'name'}*/'name'
  properties: {
    contentLink: {
      uri: /*${4:'https://content-url.nupkg'}*/'https://content-url.nupkg'
    }
  }
}