// from quickstart docs for IOT: https://docs.microsoft.com/en-us/azure/iot-hub/horizontal-arm-route-messages
@minLength(1)
@maxLength(11)
param projectName string = 'contoso'

param location string = resourceGroup().location
param skuName string = 'S1'
param skuUnits int = 1
param d2cPartitions int = 4 // partitions used for the event stream

var unique = uniqueString(resourceGroup().id)

var iotHubName = '${projectName}Hub${unique}'
var storageAccountName = '${toLower(projectName)}${unique}'
var storageEndpoint = '${projectName}StorageEndpoint'
var storageContainerName = '${toLower(projectName)}results'

resource stg 'microsoft.storage/storageAccounts@2019-06-01' = {
  name: storageAccountName
  location: location
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'Storage'
}

resource blob 'microsoft.storage/storageAccounts/blobServices/containers@2019-06-01' = {
  name: '${stg.name}/default/${storageContainerName}'
  properties: {
    publicAccess: 'None'
  }
}

resource iot 'microsoft.devices/iotHubs@2020-03-01' = {
  name: iotHubName
  location: location
  sku: {
    name: skuName
    capacity: skuUnits
  }
  properties: {
    eventHubEndpoints: {
      events: {
        retentionTimeInDays: 1
        partitionCount: d2cPartitions
      }
    }
    routing: {
      endpoints: {
        storageContainers: [
          {
            // really need a listConnectionString() function
            connectionString: 'DefaultEndpointsProtocol=https;AccountName=${storageAccountName};EndpointSuffix=${environment().suffixes.storage};AccountKey=${stg.listKeys().keys[0].value}'
            containerName: storageContainerName
            fileNameFormat: '{iothub}/{partition}/{YYYY}/{MM}/{DD}/{HH}/{mm}'
            batchFrequencyInSeconds: 100
            maxChunkSizeInBytes: 104857600
            encoding: 'JSON'
            name: storageEndpoint
          }
        ]
      }
      routes: [
        {
          name: 'ContosoStorageRoute'
          source: 'DeviceMessages'
          condition: 'level="storage"'
          endpointNames: [
            storageEndpoint
          ]
          isEnabled: true
        }
      ]
      fallbackRoute: {
        name: '$fallback'
        source: 'DeviceMessages'
        condition: 'true' // todo - does this need to be a string?
        endpointNames: [
          'events'
        ]
        isEnabled: true
      }
    }
    messagingEndpoints: {
      fileNotifications: {
        lockDurationAsIso8601: 'PT1M'
        ttlAsIso8601: 'PT1H'
        maxDeliveryCount: 10
      }
    }
    enableFileUploadNotifications: false
    cloudToDevice: {
      maxDeliveryCount: 10
      defaultTtlAsIso8601: 'PT1H'
      feedback: {
        lockDurationAsIso8601: 'PT1M'
        ttlAsIso8601: 'PT1H'
        maxDeliveryCount: 10
      }
    }
  }
}
