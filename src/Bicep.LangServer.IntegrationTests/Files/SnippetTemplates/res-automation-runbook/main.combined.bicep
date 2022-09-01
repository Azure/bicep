// $1 = 'name'
// $2 = automationRunbook
// $3 = 'name'
// $4 = location
// $5 = true
// $6 = true
// $7 = Script
// $8 = 'uri'
// $9 = '1.0.0.0'
// $10 = 'description'

param location string

resource automationAccount 'Microsoft.Automation/automationAccounts@2019-06-01' = {
  name: 'name'
}

resource automationRunbook 'Microsoft.Automation/automationAccounts/runbooks@2019-06-01' = {
  parent: automationAccount
  name: 'name'
  location: location
  properties: {
    logVerbose: true
    logProgress: true
    runbookType: 'Script'
    publishContentLink: {
      uri: 'uri'
      version: '1.0.0.0'
    }
    description: 'description'
  }
}
// Insert snippet here

