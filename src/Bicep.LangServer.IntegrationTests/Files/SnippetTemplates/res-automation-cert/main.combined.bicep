// $1 = 'name'
// $2 = automationCertificate
// $3 = 'name'
// $4 = 'base64Value'
// $5 = 'description'
// $6 = 'thumbprint'
// $7 = true

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
// Insert snippet here

