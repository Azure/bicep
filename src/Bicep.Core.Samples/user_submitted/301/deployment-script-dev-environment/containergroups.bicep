param name string

@description('Specify which type of dev environment to deploy')
@allowed([
  'AzureCLI'
  'AzurePowerShell'
])
param type string = 'AzureCLI'

@description('Use to overide the version to use for Azure CLI or AzurePowerShell')
param toolVersion string = ''

@description('This is the path in the container instance where it\'s mounted to the file share.')
param mountPath string = '/mnt/azscripts/azscriptinput'

@description('Time in second before the container instance is suspended')
param sessionTime string = '1800'

param fileShareName string
param storageName string
param storageId string
param location string = resourceGroup().location

// Specifies which version to use if no specific toolVersion is provided (Azure CLI latest or Azure PowerShell 5.6)
var version = (type == 'AzureCLI' && toolVersion == '' ? 'latest' : type == 'AzurePowerShell' && toolVersion == '' ? '5.6' : toolVersion)

var azcliImage = 'mcr.microsoft.com/azure-cli:${version}'
var azpwshImage = 'mcr.microsoft.com/azuredeploymentscripts-powershell:az${version}'

var azpwshCommand = [
  '/bin/sh'
  '-c'
  'pwsh -c \'Start-Sleep -Seconds ${sessionTime}\''
]

var azcliCommand = [
  '/bin/bash'
  '-c'
  'echo hello; sleep ${sessionTime}'
]

resource containerGroupName 'Microsoft.ContainerInstance/containerGroups@2019-12-01' = {
  name: name
  location: location
  properties: {
    containers: [
      {
        name: '${name}cg'
        properties: {
          image: type == 'AzureCLI' ? azcliImage : type == 'AzurePowerShell' ? azpwshImage : ''
          resources: {
            requests: {
              cpu: 1
              memoryInGB: 2
            }
          }
          ports: [
            {
              protocol: 'TCP'
              port: 80
            }
          ]
          volumeMounts: [
            {
              name: 'filesharevolume'
              mountPath: mountPath
            }
          ]
          command: type == 'AzureCLI' ? azcliCommand : type == 'AzurePowerShell' ? azpwshCommand : null
        }
      }
    ]
    osType: 'Linux'
    volumes: [
      {
        name: 'filesharevolume'
        azureFile: {
          readOnly: false
          shareName: fileShareName
          storageAccountName: storageName
          storageAccountKey: listKeys(storageId, '2019-06-01').keys[0].value
        }
      }
    ]
  }
}
