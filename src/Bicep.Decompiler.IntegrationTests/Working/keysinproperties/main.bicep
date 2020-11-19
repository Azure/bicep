param storageAccountType string {
  allowed: [
    'Standard_LRS'
    'Standard_GRS'
  ]
  metadata: {
    description: 'Storage account type'
  }
  default: 'Standard_LRS'
}
param fileShareName string {
  metadata: {
    description: 'Name of file share to be created'
  }
  default: 'sftpfileshare'
}
param sftpUser string {
  metadata: {
    description: 'Username to use for SFTP access'
  }
}
param sftpPassword string {
  metadata: {
    description: 'Password to use for SFTP access'
  }
  secure: true
}
param location string {
  metadata: {
    description: 'Primary location for resources'
  }
  default: resourceGroup().location
}

var scriptName_var = 'createFileShare'
var identityName_var = 'scratch'
var roleDefinitionId = resourceId('Microsoft.Authorization/roleDefinitions', 'b24988ac-6180-42a0-ab88-20f7382dd24c')
var roleDefinitionName_var = guid(identityName_var, roleDefinitionId)
var sftpContainerName = 'sftp'
var sftpContainerGroupName_var = 'sftp-group'
var sftpContainerImage = 'atmoz/sftp:latest'
var sftpEnvVariable = '${sftpUser}:${sftpPassword}:1001'
var storageAccountName_var = 'sftpstg${uniqueString(resourceGroup().id)}'

resource identityName 'Microsoft.ManagedIdentity/userAssignedIdentities@2018-11-30' = {
  name: identityName_var
  location: location
}

resource roleDefinitionName 'Microsoft.Authorization/roleAssignments@2020-04-01-preview' = {
  name: roleDefinitionName_var
  properties: {
    roleDefinitionId: roleDefinitionId
    principalId: reference(identityName_var).principalId
    scope: resourceGroup().id
//@[4:9) [BCP073 (Warning)] The property "scope" is read-only. Expressions cannot be assigned to read-only properties. |scope|
    principalType: 'ServicePrincipal'
  }
  dependsOn: [
    identityName
  ]
}

resource storageAccountName 'Microsoft.Storage/storageAccounts@2019-06-01' = {
  name: storageAccountName_var
  location: location
  sku: {
    name: storageAccountType
  }
  kind: 'StorageV2'
  properties: {}
  dependsOn: [
    roleDefinitionName
  ]
}

resource scriptName 'Microsoft.Resources/deploymentScripts@2019-10-01-preview' = {
  name: scriptName_var
  location: location
  kind: 'AzurePowerShell'
  identity: {
    type: 'UserAssigned'
    userAssignedIdentities: {
      '${identityName.id}': {}
    }
  }
  properties: {
    forceUpdateTag: '1'
    azPowerShellVersion: '3.0'
    arguments: ' -storageAccountName ${storageAccountName_var} -fileShareName ${fileShareName} -resourceGroupName ${resourceGroup().name}'
    scriptContent: '\n                param(\n                    [string] $storageAccountName,\n                    [string] $fileShareName,\n                    [string] $resourceGroupName\n                )\n                Get-AzStorageAccount -StorageAccountName $storageAccountName -ResourceGroupName $resourceGroupName | New-AzStorageShare -Name $fileShareName\n                '
    timeout: 'PT5M'
    cleanupPreference: 'OnSuccess'
    retentionInterval: 'P1D'
  }
  dependsOn: [
    storageAccountName
  ]
}

resource sftpContainerGroupName 'Microsoft.ContainerInstance/containerGroups@2019-12-01' = {
  name: sftpContainerGroupName_var
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
          storageAccountName: storageAccountName_var
          storageAccountKey: listKeys(storageAccountName_var, '2018-02-01').keys[0].value
        }
      }
    ]
  }
  dependsOn: [
    scriptName
  ]
}

output containerIPv4Address string = sftpContainerGroupName.properties.ipAddress.ip
