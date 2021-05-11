resource automationAccount 'Microsoft.Automation/automationAccounts@2019-06-01' = {
  name: 'name'
}

resource automationRunbook 'Microsoft.Automation/automationAccounts/runbooks@2019-06-01' = {
  parent: automationAccount
  name: 'name'
  location: resourceGroup().location
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

