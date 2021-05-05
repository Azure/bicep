resource automationAccount 'Microsoft.Automation/automationAccounts@2019-06-01' = {
  name: 'name'
}

resource automationCertificate 'Microsoft.Automation/automationAccounts/certificates@2019-06-01' = {
  parent: automationAccount
  name: 'name'
  properties: {
    base64Value: 'base64Value'
    description: 'description'
    thumbprint: 'thumbprint'
    isExportable: true
  }
}

