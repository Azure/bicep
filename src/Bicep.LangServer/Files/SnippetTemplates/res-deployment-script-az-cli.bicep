// Deployment script AzureCLI
resource /*${1:deploymentScript}*/deploymentScript 'Microsoft.Resources/deploymentScripts@2023-08-01' = {
  name: /*${2:'name'}*/'name'
  location: /*${3:location}*/'location'
  kind: 'AzureCLI'
  properties: {
    azCliVersion: /*${4:'azCliVersion'}*/'azCliVersion'
    scriptContent: /*${5:'scriptContent'}*/'scriptContent'
    retentionInterval: /*${6:'retentionInterval'}*/'retentionInterval'
  }
}
