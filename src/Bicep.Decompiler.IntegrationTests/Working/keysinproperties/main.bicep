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

var scriptName = 'createFileShare'
var identityName = 'scratch'
var roleDefinitionId = resourceId('Microsoft.Authorization/roleDefinitions', 'b24988ac-6180-42a0-ab88-20f7382dd24c')
var roleDefinitionName = guid(identityName, roleDefinitionId)
var sftpContainerName = 'sftp'
var sftpContainerGroupName = 'sftp-group'
var sftpContainerImage = 'atmoz/sftp:latest'
var sftpEnvVariable = '${sftpUser}:${sftpPassword}:1001'
var storageAccountName = 'sftpstg${uniqueString(resourceGroup().id)}'

resource identityName_resource 'Microsoft.ManagedIdentity/userAssignedIdentities@2018-11-30' = {
  name: identityName
  location: location
}

resource roleDefinitionName_resource 'Microsoft.Authorization/roleAssignments@2020-04-01-preview' = {
  name: roleDefinitionName
  properties: {
    roleDefinitionId: roleDefinitionId
    principalId: reference(identityName).principalId
    scope: resourceGroup().id
//@[4:9) [BCP073 (Warning)] The property "scope" is read-only. Expressions cannot be assigned to read-only properties. |scope|
    principalType: 'ServicePrincipal'
  }
  dependsOn: [
    identityName_resource
  ]
}

resource storageAccountName_resource 'Microsoft.Storage/storageAccounts@2019-06-01' = {
  name: storageAccountName
  location: location
  sku: {
    name: storageAccountType
  }
  kind: 'StorageV2'
  properties: {}
  dependsOn: [
    roleDefinitionName_resource
    ' need to create a slight delay for the roleAssignment to replicate before the deployment script can run'
//@[4:109) [BCP034 (Warning)] The enclosing array expected an item of type "resource | module", but the provided item was of type "' need to create a slight delay for the roleAssignment to replicate before the deployment script can run'". |' need to create a slight delay for the roleAssignment to replicate before the deployment script can run'|
  ]
}

resource scriptName_resource 'Microsoft.Resources/deploymentScripts@2019-10-01-preview' = {
  name: scriptName
  location: location
  kind: 'AzurePowerShell'
  identity: {
    type: 'userAssigned'
//@[10:24) [BCP088 (Warning)] The property "type" expected a value of type "'UserAssigned'" but the provided value is of type "'userAssigned'". Did you mean "'UserAssigned'"? |'userAssigned'|
    userAssignedIdentities: {
      '${identityName_resource.id}': {}
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
    storageAccountName_resource
  ]
}

resource sftpContainerGroupName_resource 'Microsoft.ContainerInstance/containerGroups@2019-12-01' = {
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
//@[20:25) [BCP088 (Warning)] The property "protocol" expected a value of type "'TCP' | 'UDP'" but the provided value is of type "'Tcp'". Did you mean "'TCP'"? |'Tcp'|
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
    scriptName_resource
  ]
}

output containerIPv4Address string = sftpContainerGroupName_resource.properties.ipAddress.ip
