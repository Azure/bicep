// $1 = 'name'
// $2 = automationCredential
// $3 = 'name'
// $4 = 'userName'
// $5 = 'password'
// $6 = 'description'

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
// Insert snippet here

