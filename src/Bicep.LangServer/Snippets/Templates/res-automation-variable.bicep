// Automation Variable
resource automationAccount 'Microsoft.Automation/automationAccounts@2019-06-01' = {
  name: ${1:'name'}
}

resource ${2:automationVariable} 'Microsoft.Automation/automationAccounts/variables@2019-06-01' = {
  parent: automationAccount
  name: ${3:'name'}
  properties: {
    value: ${4:'value'}
    description: ${5:'description'}
    isEncrypted: ${6|true,false|}
  }
}
