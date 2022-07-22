// $1 = 'name'
// $2 = automationAccountVariable
// $3 = 'name'
// $4 = 'https://test-content-url.nupkg'

resource automationAccount 'Microsoft.Automation/automationAccounts@2019-06-01' = {
  name: 'name'
}

resource automationAccountVariable 'Microsoft.Automation/automationAccounts/modules@2019-06-01' = {
  parent: automationAccount
  name: 'name'
  properties: {
    contentLink: {
      uri: 'https://test-content-url.nupkg'
    }
  }
}// Insert snippet here

