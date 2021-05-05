// Automation Credential
resource automationAccount 'Microsoft.Automation/automationAccounts@2015-10-31' = {
  name: ${1:'name'}
}

resource ${2:automationCredential} 'Microsoft.Automation/automationAccounts/credentials@2015-10-31' = {
  parent: automationAccount
  name: ${3:'name'}
  properties: {
    userName: ${4:'userName'}
    password: ${5:'password'}
    description: ${6:'description'}
  }
}
