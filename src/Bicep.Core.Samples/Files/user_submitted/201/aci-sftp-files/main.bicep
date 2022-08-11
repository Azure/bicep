param storageAccountType string = 'Standard_LRS'
param fileShareName string = 'sftpfileshare'
param sftpUser string

@secure()
param sftpPassword string

param location string = resourceGroup().location

var scriptName = 'createFileShare'
var identityName = 'scratch'
var roleDefinitionId = resourceId('Microsoft.Authorization/roleDefinitions', 'b24988ac-6180-42a0-ab88-20f7382dd24c')
var roleAssignmentName = guid(identityName, roleDefinitionId)
var sftpContainerName = 'sftp'
var sftpContainerGroupName = 'sftp-group'
var sftpContainerImage = 'atmoz/sftp:latest'
var sftpEnvVariable = '${sftpUser}:${sftpPassword}:1001'
var storageAccountName = 'sftpstg${uniqueString(resourceGroup().id)}'

resource managedIdentity 'Microsoft.ManagedIdentity/userAssignedIdentities@2018-11-30' = {
  name: identityName
  location: location
}

resource roleAssignment 'Microsoft.Authorization/roleAssignments@2020-04-01-preview' = {
  name: roleAssignmentName
  properties: {
    roleDefinitionId: roleDefinitionId
    principalId: managedIdentity.properties.principalId
    principalType: 'ServicePrincipal'
  }
}

resource storageAccount 'Microsoft.Storage/storageAccounts@2020-08-01-preview' = {
  name: storageAccountName
  location: location
  sku: {
    name: storageAccountType
  }
  kind: 'StorageV2'
  dependsOn: [
    roleAssignment // need to create a slight delay for the roleAssignment to replicate before the deployment script can run
  ]
}

resource deploymentScript 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
  name: scriptName
  location: location
  kind: 'AzurePowerShell'
  identity: {
    type: 'UserAssigned'
    userAssignedIdentities: {
      '${managedIdentity.id}': {}
    }
  }
  properties: {
    forceUpdateTag: '1'
    azPowerShellVersion: '3.0'
    arguments: ' -storageAccountName ${storageAccount.name} -fileShareName ${fileShareName} -resourceGroupName ${resourceGroup().name}'
    scriptContent: 'param([string] $storageAccountName, [string] $fileShareName, [string] $resourceGroupName) Get-AzStorageAccount -StorageAccountName $storageAccountName -ResourceGroupName $resourceGroupName | New-AzStorageShare -Name $fileShareName'
    timeout: 'PT5M'
    cleanupPreference: 'OnSuccess'
    retentionInterval: 'P1D'
  }
}

resource containerGroup 'Microsoft.ContainerInstance/containerGroups@2019-12-01' = {
  name: sftpContainerGroupName
  location: location
  dependsOn: [
    deploymentScript // Need to create the fileShare before creating the container.
  ]
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
              name: 'sftpvolume'
              mountPath: '/home/${sftpUser}/upload'
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
          protocol: 'TCP'
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
          storageAccountName: storageAccount.name
          storageAccountKey: storageAccount.listKeys().keys[0].value
        }
      }
    ]
  }
}

output containerIpv4Address string = containerGroup.properties.ipAddress.ip
