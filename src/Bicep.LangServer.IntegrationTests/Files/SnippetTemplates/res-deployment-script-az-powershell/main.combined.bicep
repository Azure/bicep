// $1 = script
// $2 = 'inlinePS'
// $3 = location
// $4 = '10.0'
// $5 = 'Write-Output "Hello world"'
// $6 = 'PT1H'

param location string

resource script 'Microsoft.Resources/deploymentScripts@2023-08-01' = {
  name: 'inlinePS'
  location: location
  kind: 'AzurePowerShell'
  properties: {
    azPowerShellVersion: '10.0'
    scriptContent: 'Write-Output "Hello world"'
    retentionInterval: 'PT1H'
  }
}


