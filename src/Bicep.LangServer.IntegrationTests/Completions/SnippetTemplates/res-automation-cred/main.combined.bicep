resource automationAccount 'Microsoft.Automation/automationAccounts@2015-10-31' = {
  name: 'name'
}

resource automationCredential 'Microsoft.Automation/automationAccounts/credentials@2015-10-31' = {
  parent: automationAccount
  name: 'name'
  properties: {
    userName: 'userName'
    password: 'password'
    description: 'description'
  }
}

