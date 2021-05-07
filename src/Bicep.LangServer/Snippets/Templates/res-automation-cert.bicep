// Automation Certificate
resource automationAccount 'Microsoft.Automation/automationAccounts@2019-06-01' = {
  name: ${1:'name'}
}

resource ${2:automationCertificate} 'Microsoft.Automation/automationAccounts/certificates@2019-06-01' = {
  parent: automationAccount
  name: ${3:'name'}
  properties: {
    base64Value: ${4:'base64Value'}
    description: ${5:'description'}
    thumbprint: ${6:'thumbprint'}
    isExportable: ${7|true,false|}
  }
}
