param scriptName string

resource script 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
  name: scriptName
  kind: 'AzurePowerShell'
  location: resourceGroup().location
  properties: {
    azPowerShellVersion: '3.0'
    retentionInterval: 'PT6H'
    scriptContent: '''
      Write-Output 'Hello World!'
'''
  }
}

output myOutput string = scriptName
