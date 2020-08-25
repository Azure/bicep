param storageAccountType string {
    default: 'Standard_LRS'
    allowed: [
      'Standard_LRS'
      'Standard_GRS'
      'Standard_ZRS'
    ]
}

param storageAccountName string = uniqueString(resourceGroup().id)
param siteName string = storageAccountName

param mySqlPassword string {
    secure: true
}

param location string = resourceGroup().location

var cpuCores = '0.5'
var memoryInGb = '0.7'
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
        scope: resourceGroup().id
        principalType: 'ServicePrincipal'
    }
}

var uamiId = resourceId(mi.type, mi.name)

resource stg 'microsoft.storage/storageAccounts@2019-06-01' = {
    name: storageAccountName
    location: location
    tags: {
      // todo: switch to dependsOn
      // in quickstart template, they set a manual dependsOn to give time for the role assignment
      fakeDependsOn: miRoleAssign.id
    }
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
            // todo - add expression once properties can be set as strings
            replaceWithId: {
            }
        }
    }
    properties: {
        azPowerShellVersion: '3.0'
        storageAccountSettings: {
            storageAccountName: stg.name
            storageAccountKey: listKeys(stg.id, stg.apiVersion).keys[0].value
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
            // todo - add expression once properties can be set as strings
            replaceWithId: {
            }
        }
    }
    properties: {
        azPowerShellVersion: '3.0'
        storageAccountSettings: {
            storageAccountName: stg.name
            storageAccountKey: listKeys(stg.id, stg.apiVersion).keys[0].value
        }
        scriptContent: sqlScriptToExecute
        cleanupPreference: 'OnSuccess'
        retentionInterval: 'P1D'
        timeout: 'PT5M'
        // forceUpdateTag: currentTime // ensures script will run every time
    }
}

resource wpAci 'microsoft.containerInstance/containerGroups@2019-12-01' = {
  name: 'wordpress-containerinstance'
  location: location
  // todo - replace tags with dependsOn
  tags: {
    fakeDependsOn1: dScriptSql.id
    fakeDependsOn2: dScriptWp.id
  }
  properties: {
    containers: [
      {
        name: 'wordpress'
        properties: {
          image: 'wordpress:4.9-apache'
          ports: [
            {
              protocol: 'Tcp'
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
              memoryInGb: memoryInGb
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
              protocol: 'Tcp'
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
              memoryInGb: memoryInGb
            }
          }
        }
      }
    ]
    volumes: [
      {
        azureFile: {
          shareName: wpShareName
          storageAccountKey: listKeys(stg.name, stg.apiVersion).keys[0].value
          storageAccountName: stg.name
        }
        name: 'wordpressfile'
      }
      {
        azureFile: {
          shareName: sqlShareName
          storageAccountKey: listKeys(stg.name, stg.apiVersion).keys[0].value
          storageAccountName: stg.name
        }
        name: 'mysqlfile'
      }
    ]
    ipAddress: {
      ports: [
        {
          protocol: 'Tcp'
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