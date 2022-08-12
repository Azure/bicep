param location string = 'westus'
param timestamp string = utcNow()
param dsName string = 'ds${uniqueString(resourceGroup().name)}'

resource script 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
  kind: 'AzurePowerShell'
  name: dsName
  location: location
  // identity property no longer required
  properties: {
    azPowerShellVersion: '3.0'
    scriptContent: '''
$DeploymentScriptOutputs["test"] = "test this output"
'''
    forceUpdateTag: timestamp // script will run every time
    retentionInterval: 'PT4H' // deploymentScript resource will delete itself in 4 hours
  }
}

output scriptOutput string = script.properties.outputs.test
