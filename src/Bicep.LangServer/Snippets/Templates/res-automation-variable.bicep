// Automation Variable
resource automationAccount 'Microsoft.Automation/automationAccounts@2019-06-01' = {
  name: /*${1:'name'}*/'name'
}

resource /*${2:automationVariable}*/automationVariable 'Microsoft.Automation/automationAccounts/variables@2019-06-01' = {
  parent: automationAccount
  name: /*${3:'name'}*/'name'
  properties: {
    value: /*${4:'value'}*/'value'
    description: /*${5:'description'}*/'description'
    isEncrypted: /*${6|true,false|}*/true
  }
}
