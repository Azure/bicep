// $1 = script
// $2 = 'inlinePS'
// $3 = location
// $4 = '10.0'
// $5 = 'Write-Output "Hello world"'
// $6 = 'PT1H'

param location string

resource script 'Microsoft.Resources/deploymentScripts@2023-08-01' = {
//@[0:268) [use-recent-az-powershell-version (Warning)] Deployment script is using AzPowerShell version '10.0' which is below the recommended minimum version '11.0'. Consider upgrading to version 11.0 or higher to avoid EOL Ubuntu 20.04 LTS. (bicep core linter https://aka.ms/bicep/linter-diagnostics#use-recent-az-powershell-version) |resource script 'Microsoft.Resources/deploymentScripts@2023-08-01' = {\n  name: 'inlinePS'\n  location: location\n  kind: 'AzurePowerShell'\n  properties: {\n    azPowerShellVersion: '10.0'\n    scriptContent: 'Write-Output "Hello world"'\n    retentionInterval: 'PT1H'\n  }\n}|
  name: 'inlinePS'
  location: location
  kind: 'AzurePowerShell'
  properties: {
    azPowerShellVersion: '10.0'
    scriptContent: 'Write-Output "Hello world"'
    retentionInterval: 'PT1H'
  }
}


