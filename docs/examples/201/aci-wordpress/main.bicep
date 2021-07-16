@allowed([
  'Standard_LRS'
  'Standard_GRS'
  'Standard_ZRS'
])
param storageAccountType string = 'Standard_LRS'

param storageAccountName string = uniqueString(resourceGroup().id)
param siteName string = storageAccountName

@secure()
param mySqlPassword string

param location string = resourceGroup().location

var cpuCores = any('0.5') // TODO: workaround for https://github.com/Azure/bicep/issues/486
var memoryInGb = any('0.7') // TODO: workaround for https://github.com/Azure/bicep/issues/486
var scriptName = 'createFileShare'
var wpShareName = 'wordpress-share'
var sqlShareName = 'mysql-share'

resource mi 'microsoft.managedIdentity/userAssignedIdentities@2018-11-30' = {
  name: 'scratch'
  location: location
}

var roleDefinitionId = resourceId('microsoft.authorization/roleDefinitions', 'b24988ac-6180-42a0-ab88-20f7382dd24c')
var roleAssignmentName = guid(mi.name, roleDefinitionId, resourceGroup().id)

resource miRoleAssign 'microsoft.authorization/roleAssignments@2020-04-01-preview' = {
  name: roleAssignmentName
  properties: {
    roleDefinitionId: roleDefinitionId
    principalId: mi.properties.principalId
    principalType: 'ServicePrincipal'
  }
}

var uamiId = resourceId(mi.type, mi.name)

resource stg 'microsoft.storage/storageAccounts@2019-06-01' = {
  dependsOn: [
    miRoleAssign
  ]
  name: storageAccountName
  location: location
  sku: {
    name: storageAccountType
  }
  kind: 'StorageV2'
}

// create file share for wordpress
var wpScriptToExecute = 'Get-AzStorageAccount -StorageAccountName ${storageAccountName} -ResourceGroupName ${resourceGroup().name} | New-AzStorageShare -Name ${wpShareName}'
resource dScriptWp 'Microsoft.Resources/deploymentScripts@2019-10-01-preview' = {
  name: '${scriptName}-${wpShareName}'
  location: location
  kind: 'AzurePowerShell'
  identity: {
    type: 'UserAssigned'
    userAssignedIdentities: {
      '${uamiId}': {}
    }
  }
  properties: {
    azPowerShellVersion: '3.0'
    storageAccountSettings: {
      storageAccountName: stg.name
      storageAccountKey: stg.listKeys().keys[0].value
    }
    scriptContent: wpScriptToExecute
    cleanupPreference: 'OnSuccess'
    retentionInterval: 'P1D'
    timeout: 'PT5M'
  }
}

// create second file share for sql
var sqlScriptToExecute = 'Get-AzStorageAccount -StorageAccountName ${storageAccountName} -ResourceGroupName ${resourceGroup().name} | New-AzStorageShare -Name ${sqlShareName}'
resource dScriptSql 'Microsoft.Resources/deploymentScripts@2019-10-01-preview' = {
  name: '${scriptName}-${sqlShareName}'
  location: location
  kind: 'AzurePowerShell'
  identity: {
    type: 'UserAssigned'
    userAssignedIdentities: {
      '${uamiId}': {}
    }
  }
  properties: {
    azPowerShellVersion: '3.0'
    storageAccountSettings: {
      storageAccountName: stg.name
      storageAccountKey: stg.listKeys().keys[0].value
    }
    scriptContent: sqlScriptToExecute
    cleanupPreference: 'OnSuccess'
    retentionInterval: 'P1D'
    timeout: 'PT5M'
    // forceUpdateTag: currentTime // ensures script will run every time
  }
}

resource wpAci 'microsoft.containerInstance/containerGroups@2019-12-01' = {
  dependsOn: [
    dScriptSql
    dScriptWp
  ]
  name: 'wordpress-containerinstance'
  location: location
  properties: {
    containers: [
      {
        name: 'wordpress'
        properties: {
          image: 'wordpress:4.9-apache'
          ports: [
            {
              protocol: 'TCP'
              port: 80
            }
          ]
          environmentVariables: [
            {
              name: 'WORDPRESS_DB_HOST'
              value: '127.0.0.1:3306'
            }
            {
              name: 'WORDPRESS_DB_PASSWORD'
              secureValue: mySqlPassword // changed this from 'value' => 'secureValue'
            }
          ]
          volumeMounts: [
            {
              mountPath: '/var/www/html'
              name: 'wordpressfile'
            }
          ]
          resources: {
            requests: {
              cpu: cpuCores
              memoryInGB: memoryInGb
            }
          }
        }
      }
      {
        name: 'mysql'
        properties: {
          image: 'mysql:5.6'
          ports: [
            {
              protocol: 'TCP'
              port: 3306
            }
          ]
          environmentVariables: [
            {
              name: 'MYSQL_ROOT_PASSWORD'
              value: mySqlPassword
            }
          ]
          volumeMounts: [
            {
              mountPath: '/var/lib/mysql'
              name: 'mysqlfile'
            }
          ]
          resources: {
            requests: {
              cpu: cpuCores
              memoryInGB: memoryInGb
            }
          }
        }
      }
    ]
    volumes: [
      {
        azureFile: {
          shareName: wpShareName
          storageAccountKey: stg.listKeys().keys[0].value
          storageAccountName: stg.name
        }
        name: 'wordpressfile'
      }
      {
        azureFile: {
          shareName: sqlShareName
          storageAccountKey: stg.listKeys().keys[0].value
          storageAccountName: stg.name
        }
        name: 'mysqlfile'
      }
    ]
    ipAddress: {
      ports: [
        {
          protocol: 'TCP'
          port: 80
        }
      ]
      type: 'Public'
      dnsNameLabel: siteName
    }
    osType: 'Linux'
  }
}

output siteFQDN string = wpAci.properties.ipAddress.fqdn
