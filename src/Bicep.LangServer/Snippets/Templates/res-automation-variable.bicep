// Automation Variable
resource automationAccount 'Microsoft.Automation/automationAccounts@2015-10-31' = {
  name: ${1:'name'}
}

resource ${2:automationVariable} 'Microsoft.Automation/automationAccounts/variables@2015-10-31' = {
  parent: automationAccount
  name: ${3:'name'}
  properties: {
    value: ${4:'value'}
    description: ${5:'description'}
    isEncrypted: ${6|true,false|}
  }
}
