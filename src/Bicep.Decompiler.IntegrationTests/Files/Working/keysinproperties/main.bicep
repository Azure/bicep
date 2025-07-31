@description('Storage account type')
@allowed([
  'Standard_LRS'
  'Standard_GRS'
])
param storageAccountType string = 'Standard_LRS'

@description('Name of file share to be created')
param fileShareName string = 'sftpfileshare'

@description('Username to use for SFTP access')
param sftpUser string

@description('Password to use for SFTP access')
@secure()
param sftpPassword string

@description('Primary location for resources')
param location string = resourceGroup().location

var scriptName = 'createFileShare'
var identityName = 'scratch'
var roleDefinitionId = resourceId('Microsoft.Authorization/roleDefinitions', 'b24988ac-6180-42a0-ab88-20f7382dd24c')
var roleDefinitionName = guid(identityName, roleDefinitionId)
var sftpContainerName = 'sftp'
var sftpContainerGroupName = 'sftp-group'
var sftpContainerImage = 'atmoz/sftp:latest'
var sftpEnvVariable = '${sftpUser}:${sftpPassword}:1001'
var storageAccountName = 'sftpstg${uniqueString(resourceGroup().id)}'

resource identity 'Microsoft.ManagedIdentity/userAssignedIdentities@2018-11-30' = {
  name: identityName
  location: location
}

resource roleDefinition 'Microsoft.Authorization/roleAssignments@2020-04-01-preview' = {
  name: roleDefinitionName
  properties: {
    roleDefinitionId: roleDefinitionId
    principalId: identity.properties.principalId
    scope: resourceGroup().id
//@[4:009) [BCP073 (Warning)] The property "scope" is read-only. Expressions cannot be assigned to read-only properties. If this is a resource type definition inaccuracy, report it using https://aka.ms/bicep-type-issues. (bicep https://aka.ms/bicep/core-diagnostics#BCP073) |scope|
    principalType: 'ServicePrincipal'
  }
}

resource storageAccount 'Microsoft.Storage/storageAccounts@2019-06-01' = {
  name: storageAccountName
  location: location
  sku: {
    name: storageAccountType
  }
  kind: 'StorageV2'
  properties: {}
  dependsOn: [
    roleDefinition
  ]
}

resource script 'Microsoft.Resources/deploymentScripts@2019-10-01-preview' = {
//@[0:971) [use-recent-az-powershell-version (Warning)] Deployment script is using AzPowerShell version '3.0' which is below the recommended minimum version '11.0'. Consider upgrading to version 11.0 or higher to avoid EOL Ubuntu 20.04 LTS. (bicep core linter https://aka.ms/bicep/linter-diagnostics#use-recent-az-powershell-version) |resource script 'Microsoft.Resources/deploymentScripts@2019-10-01-preview' = {\n  name: scriptName\n  location: location\n  kind: 'AzurePowerShell'\n  identity: {\n    type: 'userAssigned'\n    userAssignedIdentities: {\n      '${identity.id}': {}\n    }\n  }\n  properties: {\n    forceUpdateTag: '1'\n    azPowerShellVersion: '3.0'\n    arguments: ' -storageAccountName ${storageAccountName} -fileShareName ${fileShareName} -resourceGroupName ${resourceGroup().name}'\n    scriptContent: '\n                param(\n                    [string] $storageAccountName,\n                    [string] $fileShareName,\n                    [string] $resourceGroupName\n                )\n                Get-AzStorageAccount -StorageAccountName $storageAccountName -ResourceGroupName $resourceGroupName | New-AzStorageShare -Name $fileShareName\n                '\n    timeout: 'PT5M'\n    cleanupPreference: 'OnSuccess'\n    retentionInterval: 'P1D'\n  }\n  dependsOn: [\n    storageAccount\n  ]\n}|
  name: scriptName
  location: location
  kind: 'AzurePowerShell'
  identity: {
    type: 'userAssigned'
    userAssignedIdentities: {
      '${identity.id}': {}
    }
  }
  properties: {
    forceUpdateTag: '1'
    azPowerShellVersion: '3.0'
    arguments: ' -storageAccountName ${storageAccountName} -fileShareName ${fileShareName} -resourceGroupName ${resourceGroup().name}'
    scriptContent: '\n                param(\n                    [string] $storageAccountName,\n                    [string] $fileShareName,\n                    [string] $resourceGroupName\n                )\n                Get-AzStorageAccount -StorageAccountName $storageAccountName -ResourceGroupName $resourceGroupName | New-AzStorageShare -Name $fileShareName\n                '
    timeout: 'PT5M'
    cleanupPreference: 'OnSuccess'
    retentionInterval: 'P1D'
  }
  dependsOn: [
    storageAccount
  ]
}

resource sftpContainerGroup 'Microsoft.ContainerInstance/containerGroups@2019-12-01' = {
  name: sftpContainerGroupName
  location: location
  properties: {
    containers: [
      {
        name: sftpContainerName
        properties: {
          image: sftpContainerImage
          environmentVariables: [
            {
              name: 'SFTP_USERS'
              value: sftpEnvVariable
            }
          ]
          resources: {
            requests: {
              cpu: 2
              memoryInGB: 1
            }
          }
          ports: [
            {
              port: 22
            }
          ]
          volumeMounts: [
            {
              mountPath: '/home/${sftpUser}/upload'
              name: 'sftpvolume'
              readOnly: false
            }
          ]
        }
      }
    ]
    osType: 'Linux'
    ipAddress: {
      type: 'Public'
      ports: [
        {
          protocol: 'Tcp'
          port: 22
        }
      ]
    }
    restartPolicy: 'OnFailure'
    volumes: [
      {
        name: 'sftpvolume'
        azureFile: {
          readOnly: false
          shareName: fileShareName
          storageAccountName: storageAccountName
          storageAccountKey: listKeys(storageAccountName, '2018-02-01').keys[0].value
        }
      }
    ]
  }
  dependsOn: [
    script
  ]
}

output containerIPv4Address string = sftpContainerGroup.properties.ipAddress.ip

