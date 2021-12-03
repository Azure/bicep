// Automation Runbook
resource automationAccount 'Microsoft.Automation/automationAccounts@2019-06-01' = {
  name: /*${1:'name'}*/'name'
}

resource /*${2:automationRunbook}*/automationRunbook 'Microsoft.Automation/automationAccounts/runbooks@2019-06-01' = {
  parent: automationAccount
  name: /*${3:'name'}*/'name'
  location: location
  properties: {
    logVerbose: /*${4|true,false|}*/true
    logProgress: /*${5|true,false|}*/true
    runbookType: /*'${6|Script,Graph,PowerShellWorkflow,PowerShell,GraphPowerShellWorkflow,GraphPowerShell|}'*/'Script'
    publishContentLink: {
      uri: /*${7:'uri'}*/'uri'
      version: /*${8:'1.0.0.0'}*/'1.0.0.0'
    }
    description: /*${9:'description'}*/'description'
  }
}
