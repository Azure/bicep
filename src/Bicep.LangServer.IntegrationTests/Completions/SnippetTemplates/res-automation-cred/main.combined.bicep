resource automationAccount 'Microsoft.Automation/automationAccounts@2019-06-01' = {
  name: 'name'
}

resource automationCredential 'Microsoft.Automation/automationAccounts/credentials@2019-06-01' = {
  parent: automationAccount
  name: 'name'
  properties: {
    userName: 'userName'
    password: 'password'
    description: 'description'
  }
}

