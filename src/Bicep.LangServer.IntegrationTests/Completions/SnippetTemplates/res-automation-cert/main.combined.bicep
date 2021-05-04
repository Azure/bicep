resource automationAccount 'Microsoft.Automation/automationAccounts@2015-10-31' = {
  name: 'name'
}

resource automationCertificate 'Microsoft.Automation/automationAccounts/certificates@2015-10-31' = {
  parent: automationAccount
  name: 'name'
  properties: {
    base64Value: 'base64Value'
    description: 'description'
    thumbprint: 'thumbprint''
    isExportable: 'true'
  }
}

