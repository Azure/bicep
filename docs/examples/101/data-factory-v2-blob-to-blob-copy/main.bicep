param systemName string = uniqueString(resourceGroup().id)
param location string = resourceGroup().location

var dataFactoryName = 'df${systemName}'
var storageAccountName = 'sa${systemName}'
var blobContainerName = 'blob${systemName}'
var pipelineName = 'pipe${systemName}'
var dataFactoryLinkedServiceName = 'ArmtemplateStorageLinkedService'
var DataFactoryDataSetInName = 'ArmtemplateTestDatasetIn'
var DataFactoryDataSetOutName = 'ArmtemplateTestDatasetOut'

resource storageAccount 'Microsoft.Storage/storageAccounts@2020-08-01-preview' = {
  name: storageAccountName
  location: location
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
}

resource blobContainer 'Microsoft.Storage/storageAccounts/blobServices/containers@2020-08-01-preview' = {
  name: '${storageAccount.name}/default/${blobContainerName}'
}

resource dataFactory 'Microsoft.DataFactory/factories@2018-06-01' = {
  name: dataFactoryName
  location: location
  identity: {
    type: 'SystemAssigned'
  }
}

resource dataFactoryLinkedService 'Microsoft.DataFactory/factories/linkedservices@2018-06-01' = {
  name: '${dataFactory.name}/${dataFactoryLinkedServiceName}'
  properties: {
    type: 'AzureBlobStorage'
    typeProperties: {
      connectionString: 'DefaultEndpointsProtocol=https;AccountName=${storageAccount.name};AccountKey=${listKeys(storageAccount.id, storageAccount.apiVersion).keys[0].value}'
    }
  }
}

resource dataFactoryDataSetIn 'Microsoft.DataFactory/factories/datasets@2018-06-01' = {
  name: '${dataFactory.name}/${DataFactoryDataSetInName}'
  properties: {
    linkedServiceName: {
      referenceName: dataFactoryLinkedServiceName
      type: 'LinkedServiceReference'
    }
    type: 'Binary'
    typeProperties: {
      location: {
        type: 'AzureBlobStorageLocation'
        container: blobContainer.name
        folderPath: 'input'
        fileName: 'emp.txt'
      }
    }
  }
}
resource dataFactoryDataSetOut 'Microsoft.DataFactory/factories/datasets@2018-06-01' = {
  name: '${dataFactory.name}/${DataFactoryDataSetOutName}'
  properties: {
    linkedServiceName: {
      referenceName: dataFactoryLinkedServiceName
      type: 'LinkedServiceReference'
    }
    type: 'Binary'
    typeProperties: {
      location: {
        type: 'AzureBlobStorageLocation'
        container: blobContainer.name
        folderPath: 'output'
      }
    }
  }
}

resource dataFactoryPipeline 'Microsoft.DataFactory/factories/pipelines@2018-06-01' = {
  name: '${dataFactory.name}/${pipelineName}'
  properties: {
    activities: [
      {
        name: 'MyCopyActivity'
        type: 'Copy'
        policy: {
          timeout: '7.00:00:00'
          retry: 0
          retryIntervalInSeconds: 30
          secureOutput: false
          secureInput: false
        }
        typeProperties: {
          source: {
            type: 'BinarySource'
            storeSettings: {
              type: 'AzureBlobStorageReadSettings'
              recursive: true
            }
          }
          sink: {
            type: 'BinarySink'
            storeSettings: {
              type: 'AzureBlobStorageWriterSettings'
            }
          }
          enableStaging: false
        }
        inputs: [
          {
            referenceName: DataFactoryDataSetInName
            type: 'DatasetReference'
            properties: {}
          }
        ]
        outputs: [
          {
            referenceName: DataFactoryDataSetOutName
            type: 'DatasetReference'
            properties: {}
          }
        ]
      }
    ]
  }
  dependsOn: [
    dataFactoryDataSetIn
    dataFactoryDataSetOut
  ]
}
