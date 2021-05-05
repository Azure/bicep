// Automation Runbook
resource automationAccount 'Microsoft.Automation/automationAccounts@2015-10-31' = {
  name: ${1:'name'}
}

resource ${2:automationRunbook} 'Microsoft.Automation/automationAccounts/runbooks@2018-06-30' = {
  parent: automationAccount
  name: ${3:'name'}
  location: resourceGroup().location
  properties: {
    logVerbose: ${4|true,false|}
    logProgress: ${5|true,false|}
    runbookType: '${6|Script,Graph,PowerShellWorkflow,PowerShell,GraphPowerShellWorkflow,GraphPowerShell|}'
    publishContentLink: {
      uri: ${7:'REQUIRED'}
      version: ${8:'1.0.0.0'}
    }
    description: ${9:'description'}
  }
}
