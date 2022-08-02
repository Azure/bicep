// $1 = 'name'
// $2 = automationAccountVariable
// $3 = 'name'
// $4 = 'https://test-content-url.nupkg'

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
}// Insert snippet here

