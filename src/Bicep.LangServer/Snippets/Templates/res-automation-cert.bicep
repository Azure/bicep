// Automation Certificate
resource automationAccount 'Microsoft.Automation/automationAccounts@2019-06-01' = {
  name: /*${1:'name'}*/'name'
}

resource /*${2:automationCertificate}*/automationCertificate 'Microsoft.Automation/automationAccounts/certificates@2019-06-01' = {
  parent: automationAccount
  name: /*${3:'name'}*/'name'
  properties: {
    base64Value: /*${4:'base64Value'}*/'base64Value'
    description: /*${5:'description'}*/'description'
    thumbprint: /*${6:'thumbprint'}*/'thumbprint'
    isExportable: /*${7|true,false|}*/true
  }
}
