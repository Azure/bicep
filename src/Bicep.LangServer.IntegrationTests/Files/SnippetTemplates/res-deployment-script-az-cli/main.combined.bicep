// $1 = script
// $2 = 'inlineCLI'
// $3 = location
// $4 = '2.52.0'
// $5 = 'echo "Hello world"'
// $6 = 'PT1H'

param location string

resource script 'Microsoft.Resources/deploymentScripts@2023-08-01' = {
  name: 'inlineCLI'
  location: location
  kind: 'AzureCLI'
  properties: {
    azCliVersion: '2.52.0'
    scriptContent: 'echo "Hello world"'
    retentionInterval: 'PT1H'
  }
}


