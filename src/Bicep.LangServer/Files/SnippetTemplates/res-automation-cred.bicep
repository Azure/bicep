// Automation Credential
resource automationAccount 'Microsoft.Automation/automationAccounts@2019-06-01' = {
  name: /*${1:'name'}*/'name'
}

resource /*${2:automationCredential}*/automationCredential 'Microsoft.Automation/automationAccounts/credentials@2019-06-01' = {
  parent: automationAccount
  name: /*${3:'name'}*/'name'
  properties: {
    userName: /*${4:'userName'}*/'userName'
    password: /*${5:'password'}*/'password'
    description: /*${6:'description'}*/'description'
  }
}
