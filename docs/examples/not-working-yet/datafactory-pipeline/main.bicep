param factoryName string {
  metadata: {
    description: 'Data Factory Name'
  }
}
param globalCosmosDbAccountUri string {
  metadata: {
    description: 'Global CosmosDb account URI'
  }
}
param globalCosmosDbDatabaseName string {
  metadata: {
    description: 'Global CosmosDb database name'
  }
}
param globalCosmosDbReadOnlyKey string {
  metadata: {
    description: 'Global CosmosDb read-only connection key'
  }
  secure: true
}
param localCosmosDbAccountUri string {
  metadata: {
    description: 'Local CosmosDb account URI'
  }
}
param localCosmosDbDatabaseName string {
  metadata: {
    description: 'Local CosmosDb database name'
  }
}
param localCosmosDbReadOnlyKey string {
  metadata: {
    description: 'Local CosmosDb read-only connection key'
  }
  secure: true
}
param azureEnvironment string {
  metadata: {
    description: 'Azure Environment'
  }
}
param cosmosWriterSecret string {
  metadata: {
    description: 'Client secret to authenticate with AAD and write to Cosmos'
  }
  secure: true
}

var exportFolder = 'local/CosmosDb.DataExport/${azureEnvironment}/@{formatDateTime(dataset().startDate, \'yyyy/MM/dd\')}/00'
resource factory 'Microsoft.DataFactory/factories@2018-06-01' = {
  name: factoryName
  location: resourceGroup().location
  properties: {
  }
  identity: {
    type: 'SystemAssigned'
  }
}

resource cosmosDbGlobalService 'Microsoft.DataFactory/factories/linkedServices@2018-06-01' = {
  name: '${factory.name}/CosmosDbGlobal'
  properties: {
    annotations: [
    ]
    type: 'CosmosDb'
    typeProperties: {
      connectionString: 'AccountEndpoint=${globalCosmosDbAccountUri};Database=${globalCosmosDbDatabaseName};AccountKey=${globalCosmosDbReadOnlyKey};'
    }
    connectVia: {
      referenceName: 'WestUsIntegrationRuntime'
      type: 'IntegrationRuntimeReference'
    }
  }
  dependsOn: [
    integrationRuntime
  ]
}

resource cosmosDbLocalService 'Microsoft.DataFactory/factories/linkedServices@2018-06-01' = {
  name: '${factory.name}/CosmosDbLocal'
  properties: {
    annotations: [
    ]
    type: 'CosmosDb'
    typeProperties: {
      connectionString: 'AccountEndpoint=${localCosmosDbAccountUri};Database=${localCosmosDbDatabaseName};AccountKey=${localCosmosDbReadOnlyKey};'
    }
    connectVia: {
      referenceName: 'WestUsIntegrationRuntime'
      type: 'IntegrationRuntimeReference'
    }
  }
  dependsOn: [
    integrationRuntime
  ]
}

resource factory_Cosmos 'Microsoft.DataFactory/factories/linkedServices@2018-06-01' = {
  name: '${factory.name}/Cosmos'
  properties: {
    annotations: [
    ]
    type: 'AzureDataLakeStore'
    typeProperties: {
      dataLakeStoreUri: 'adl://azureanalytics-arm-c14.azuredatalakestore.net'
      tenant: '72f988bf-86f1-41af-91ab-2d7cd011db47'
      subscriptionId: '04b6c4a0-bc75-4984-a699-9b9c55ae8d7c'
      resourceGroupName: 'conv'
      servicePrincipalId: 'fbd555e1-1b29-41e9-ae91-5e88f77728ca'
      servicePrincipalKey: {
        type: 'SecureString'
        value: cosmosWriterSecret
      }
      connectVia: {
        referenceName: 'WestUsIntegrationRuntime'
        type: 'IntegrationRuntimeReference'
      }
    }
  }
  dependsOn: [
    integrationRuntime
  ]
}

resource factory_Copy_SubscriptionAdmins 'Microsoft.DataFactory/factories/pipelines@2018-06-01' = {
  name: '${factory.name}/Copy SubscriptionAdmins'
  properties: {
    activities: [
      {
        name: 'Copy Data'
        type: 'Copy'
        dependsOn: [
        ]
        policy: {
          timeout: '0.12:00:00'
          retry: 5
          retryIntervalInSeconds: 30
          secureOutput: true
          secureInput: true
        }
        userProperties: [
        ]
        typeProperties: {
          source: {
            type: 'DocumentDbCollectionSource'
            query: 'select c.partitionKey, c.id, c.CreatedTime, c.ChangedTime, c.DeletedTime, c.AdminState, c.AdminType, c.SubscriptionId, c.AdminPuid from c'
            nestingSeparator: '.'
          }
          sink: {
            type: 'AzureDataLakeStoreSink'
          }
          enableStaging: false
          dataIntegrationUnits: 0
        }
        inputs: [
          {
            referenceName: 'CosmosDb_subscriptionadmins'
            type: 'DatasetReference'
            parameters: {
            }
          }
        ]
        outputs: [
          {
            referenceName: 'Cosmos_subscriptionadmins'
            type: 'DatasetReference'
            parameters: {
              startDate: {
                value: '@pipeline().parameters.startDate'
                type: 'Expression'
              }
            }
          }
        ]
      }
    ]
    parameters: {
      startDate: {
        type: 'String'
      }
    }
    annotations: [
    ]
  }
  dependsOn: [
    factory_CosmosDb_subscriptionadmins
    factory_Cosmos_subscriptionadmins
  ]
}

resource factory_Copy_Subscriptions 'Microsoft.DataFactory/factories/pipelines@2018-06-01' = {
  name: '${factory.name}/Copy Subscriptions'
  properties: {
    activities: [
      {
        name: 'Copy Data'
        type: 'Copy'
        dependsOn: [
        ]
        policy: {
          timeout: '0.12:00:00'
          retry: 5
          retryIntervalInSeconds: 30
          secureOutput: true
          secureInput: true
        }
        userProperties: [
        ]
        typeProperties: {
          source: {
            type: 'DocumentDbCollectionSource'
            query: 'select c.partitionKey, c.id, c.CreatedTime, c.ChangedTime, c.DeletedTime, c.DisplayName, c.OfferType, c.State, c.SubscriptionId, c.SubscriptionPolicies, c.TenantId, c.OfferCategory from c'
            nestingSeparator: '.'
          }
          sink: {
            type: 'AzureDataLakeStoreSink'
          }
          enableStaging: false
          dataIntegrationUnits: 0
        }
        inputs: [
          {
            referenceName: 'CosmosDb_subscriptions'
            type: 'DatasetReference'
            parameters: {
            }
          }
        ]
        outputs: [
          {
            referenceName: 'Cosmos_subscriptions'
            type: 'DatasetReference'
            parameters: {
              startDate: {
                value: '@pipeline().parameters.startDate'
                type: 'Expression'
              }
            }
          }
        ]
      }
    ]
    parameters: {
      startDate: {
        type: 'String'
      }
    }
    annotations: [
    ]
  }
  dependsOn: [
    factory_CosmosDb_subscriptions
    factory_Cosmos_subscriptions
  ]
}

resource factory_Copy_SubscriptionRegistrations 'Microsoft.DataFactory/factories/pipelines@2018-06-01' = {
  name: '${factory.name}/Copy SubscriptionRegistrations'
  properties: {
    activities: [
      {
        name: 'Copy Data'
        type: 'Copy'
        dependsOn: [
        ]
        policy: {
          timeout: '0.12:00:00'
          retry: 5
          retryIntervalInSeconds: 30
          secureOutput: true
          secureInput: true
        }
        userProperties: [
        ]
        typeProperties: {
          source: {
            type: 'DocumentDbCollectionSource'
            query: 'select c.partitionKey, c.id, c.CreatedTime, c.ChangedTime, c.DeletedTime, c.SubscriptionId, c.ResourceProviderNamespace, c.RegistrationDate, c.RegistrationState from c'
            nestingSeparator: '.'
          }
          sink: {
            type: 'AzureDataLakeStoreSink'
          }
          enableStaging: false
          dataIntegrationUnits: 0
        }
        inputs: [
          {
            referenceName: 'CosmosDb_subscriptionregistrations'
            type: 'DatasetReference'
            parameters: {
            }
          }
        ]
        outputs: [
          {
            referenceName: 'Cosmos_subscriptionregistrations'
            type: 'DatasetReference'
            parameters: {
              startDate: {
                value: '@pipeline().parameters.startDate'
                type: 'Expression'
              }
            }
          }
        ]
      }
    ]
    parameters: {
      startDate: {
        type: 'String'
      }
    }
    annotations: [
    ]
  }
  dependsOn: [
    factory_CosmosDb_subscriptionregistrations
    factory_Cosmos_subscriptionregistrations
  ]
}

resource factory_Copy_SubscriptionUserProvisioningRequests 'Microsoft.DataFactory/factories/pipelines@2018-06-01' = {
  name: '${factory.name}/Copy SubscriptionUserProvisioningRequests'
  properties: {
    activities: [
      {
        name: 'Copy Data'
        type: 'Copy'
        dependsOn: [
        ]
        policy: {
          timeout: '0.12:00:00'
          retry: 5
          retryIntervalInSeconds: 30
          secureOutput: true
          secureInput: true
        }
        userProperties: [
        ]
        typeProperties: {
          source: {
            type: 'DocumentDbCollectionSource'
            query: 'select c.partitionKey, c.id, c.CreatedTime, c.ChangedTime, c.DeletedTime, c.SubscriptionId from c'
            nestingSeparator: '.'
          }
          sink: {
            type: 'AzureDataLakeStoreSink'
          }
          enableStaging: false
          dataIntegrationUnits: 0
        }
        inputs: [
          {
            referenceName: 'CosmosDb_subscriptionuserprovisioningrequests'
            type: 'DatasetReference'
            parameters: {
            }
          }
        ]
        outputs: [
          {
            referenceName: 'Cosmos_subscriptionuserprovisioningrequests'
            type: 'DatasetReference'
            parameters: {
              startDate: {
                value: '@pipeline().parameters.startDate'
                type: 'Expression'
              }
            }
          }
        ]
      }
    ]
    parameters: {
      startDate: {
        type: 'String'
      }
    }
    annotations: [
    ]
  }
  dependsOn: [
    factory_CosmosDb_subscriptionuserprovisioningrequests
    factory_Cosmos_subscriptionuserprovisioningrequests
  ]
}

resource factory_Copy_SubscriptionMappings 'Microsoft.DataFactory/factories/pipelines@2018-06-01' = {
  name: '${factory.name}/Copy SubscriptionMappings'
  properties: {
    activities: [
      {
        name: 'Copy Data'
        type: 'Copy'
        dependsOn: [
        ]
        policy: {
          timeout: '0.12:00:00'
          retry: 5
          retryIntervalInSeconds: 30
          secureOutput: true
          secureInput: true
        }
        userProperties: [
        ]
        typeProperties: {
          source: {
            type: 'DocumentDbCollectionSource'
            query: 'select c.partitionKey, c.id, c.CreatedTime, c.ChangedTime, c.DeletedTime, c.AvailabilityZones, c.SubscriptionId, c.ManagedByTenantIds from c'
            nestingSeparator: '.'
          }
          sink: {
            type: 'AzureDataLakeStoreSink'
          }
          enableStaging: false
          dataIntegrationUnits: 0
        }
        inputs: [
          {
            referenceName: 'CosmosDb_subscriptionmappings'
            type: 'DatasetReference'
            parameters: {
            }
          }
        ]
        outputs: [
          {
            referenceName: 'Cosmos_subscriptionmappings'
            type: 'DatasetReference'
            parameters: {
              startDate: {
                value: '@pipeline().parameters.startDate'
                type: 'Expression'
              }
            }
          }
        ]
      }
    ]
    parameters: {
      startDate: {
        type: 'String'
      }
    }
    annotations: [
    ]
  }
  dependsOn: [
    factory_CosmosDb_subscriptionmappings
    factory_Cosmos_subscriptionmappings
  ]
}

resource factory_CosmosDb_subscriptionadmins 'Microsoft.DataFactory/factories/datasets@2018-06-01' = {
  name: '${factory.name}/CosmosDb_subscriptionadmins'
  properties: {
    linkedServiceName: {
      referenceName: 'CosmosDbGlobal'
      type: 'LinkedServiceReference'
    }
    folder: {
      name: 'CosmosDb'
    }
    annotations: [
    ]
    type: 'DocumentDbCollection'
    typeProperties: {
      collectionName: 'subscriptionadmins'
    }
  }
  dependsOn: [
    cosmosDbGlobalService
  ]
}

resource factory_CosmosDb_subscriptionregistrations 'Microsoft.DataFactory/factories/datasets@2018-06-01' = {
  name: '${factory.name}/CosmosDb_subscriptionregistrations'
  properties: {
    linkedServiceName: {
      referenceName: 'CosmosDbLocal'
      type: 'LinkedServiceReference'
    }
    folder: {
      name: 'CosmosDb'
    }
    annotations: [
    ]
    type: 'DocumentDbCollection'
    typeProperties: {
      collectionName: 'subscriptionregistrations'
    }
  }
  dependsOn: [
    cosmosDbLocalService
  ]
}

resource factory_CosmosDb_subscriptions 'Microsoft.DataFactory/factories/datasets@2018-06-01' = {
  name: '${factory.name}/CosmosDb_subscriptions'
  properties: {
    linkedServiceName: {
      referenceName: 'CosmosDbGlobal'
      type: 'LinkedServiceReference'
    }
    folder: {
      name: 'CosmosDb'
    }
    annotations: [
    ]
    type: 'DocumentDbCollection'
    typeProperties: {
      collectionName: 'subscriptions'
    }
  }
  dependsOn: [
    cosmosDbGlobalService
  ]
}

resource factory_CosmosDb_subscriptionuserprovisioningrequests 'Microsoft.DataFactory/factories/datasets@2018-06-01' = {
  name: '${factory.name}/CosmosDb_subscriptionuserprovisioningrequests'
  properties: {
    linkedServiceName: {
      referenceName: 'CosmosDbGlobal'
      type: 'LinkedServiceReference'
    }
    folder: {
      name: 'CosmosDb'
    }
    annotations: [
    ]
    type: 'DocumentDbCollection'
    typeProperties: {
      collectionName: 'subscriptionuserprovisioningrequests'
    }
  }
  dependsOn: [
    cosmosDbGlobalService
  ]
}

resource factory_CosmosDb_subscriptionmappings 'Microsoft.DataFactory/factories/datasets@2018-06-01' = {
  name: '${factory.name}/CosmosDb_subscriptionmappings'
  properties: {
    linkedServiceName: {
      referenceName: 'CosmosDbGlobal'
      type: 'LinkedServiceReference'
    }
    folder: {
      name: 'CosmosDb'
    }
    annotations: [
    ]
    type: 'DocumentDbCollection'
    typeProperties: {
      collectionName: 'subscriptionmappings'
    }
  }
  dependsOn: [
    cosmosDbGlobalService
  ]
}

resource factory_Cosmos_subscriptionadmins 'Microsoft.DataFactory/factories/datasets@2018-06-01' = {
  name: '${factory.name}/Cosmos_subscriptionadmins'
  properties: {
    linkedServiceName: {
      referenceName: 'Cosmos'
      type: 'LinkedServiceReference'
    }
    parameters: {
      startDate: {
        type: 'String'
      }
    }
    folder: {
      name: 'Cosmos'
    }
    annotations: [
    ]
    type: 'AzureDataLakeStoreFile'
    typeProperties: {
      format: {
        type: 'JsonFormat'
        filePattern: 'setOfObjects'
      }
      fileName: 'SubscriptionAdmins.json'
      folderPath: {
        value: exportFolder
        type: 'Expression'
      }
    }
  }
  dependsOn: [
    factory_Cosmos
  ]
}

resource factory_Cosmos_subscriptionuserprovisioningrequests 'Microsoft.DataFactory/factories/datasets@2018-06-01' = {
  name: '${factory.name}/Cosmos_subscriptionuserprovisioningrequests'
  properties: {
    linkedServiceName: {
      referenceName: 'Cosmos'
      type: 'LinkedServiceReference'
    }
    parameters: {
      startDate: {
        type: 'String'
      }
    }
    folder: {
      name: 'Cosmos'
    }
    annotations: [
    ]
    type: 'AzureDataLakeStoreFile'
    typeProperties: {
      format: {
        type: 'JsonFormat'
        filePattern: 'setOfObjects'
      }
      fileName: 'SubscriptionUserProvisioningRequests.json'
      folderPath: {
        value: exportFolder
        type: 'Expression'
      }
    }
  }
  dependsOn: [
    factory_Cosmos
  ]
}

resource factory_Cosmos_subscriptionregistrations 'Microsoft.DataFactory/factories/datasets@2018-06-01' = {
  name: '${factory.name}/Cosmos_subscriptionregistrations'
  properties: {
    linkedServiceName: {
      referenceName: 'Cosmos'
      type: 'LinkedServiceReference'
    }
    parameters: {
      startDate: {
        type: 'String'
      }
    }
    folder: {
      name: 'Cosmos'
    }
    annotations: [
    ]
    type: 'AzureDataLakeStoreFile'
    typeProperties: {
      format: {
        type: 'JsonFormat'
        filePattern: 'setOfObjects'
      }
      fileName: 'SubscriptionRegistrations.json'
      folderPath: {
        value: exportFolder
        type: 'Expression'
      }
    }
  }
  dependsOn: [
    factory_Cosmos
  ]
}

resource factory_Cosmos_subscriptionmappings 'Microsoft.DataFactory/factories/datasets@2018-06-01' = {
  name: '${factory.name}/Cosmos_subscriptionmappings'
  properties: {
    linkedServiceName: {
      referenceName: 'Cosmos'
      type: 'LinkedServiceReference'
    }
    parameters: {
      startDate: {
        type: 'String'
      }
    }
    folder: {
      name: 'Cosmos'
    }
    annotations: [
    ]
    type: 'AzureDataLakeStoreFile'
    typeProperties: {
      format: {
        type: 'JsonFormat'
        filePattern: 'setOfObjects'
      }
      fileName: 'SubscriptionMappings.json'
      folderPath: {
        value: exportFolder
        type: 'Expression'
      }
    }
  }
  dependsOn: [
    factory_Cosmos
  ]
}

resource factory_Cosmos_subscriptions 'Microsoft.DataFactory/factories/datasets@2018-06-01' = {
  name: '${factory.name}/Cosmos_subscriptions'
  properties: {
    linkedServiceName: {
      referenceName: 'Cosmos'
      type: 'LinkedServiceReference'
    }
    parameters: {
      startDate: {
        type: 'String'
      }
    }
    folder: {
      name: 'Cosmos'
    }
    annotations: [
    ]
    type: 'AzureDataLakeStoreFile'
    typeProperties: {
      format: {
        type: 'JsonFormat'
        filePattern: 'setOfObjects'
      }
      fileName: 'Subscriptions.json'
      folderPath: {
        value: exportFolder
        type: 'Expression'
      }
    }
  }
  dependsOn: [
    factory_Cosmos
  ]
}

resource Microsoft_DataFactory_factories_triggers__factory_Copy_SubscriptionRegistrations 'Microsoft.DataFactory/factories/triggers@2018-06-01' = {
  name: '${factory.name}/Copy SubscriptionRegistrations'
  properties: {
    annotations: [
    ]
    pipelines: [
      {
        pipelineReference: {
          referenceName: 'Copy SubscriptionRegistrations'
          type: 'PipelineReference'
        }
        parameters: {
          startDate: '@{formatDateTime(addhours(trigger().scheduledTime, -18), \'yyyy/MM/dd\')}'
        }
      }
    ]
    type: 'ScheduleTrigger'
    typeProperties: {
      recurrence: {
        frequency: 'Day'
        interval: 1
        startTime: '11/27/2018 12:00:00 PM'
        timeZone: 'UTC'
        schedule: {
          minutes: [
            0
          ]
          hours: [
            18
          ]
        }
      }
    }
  }
  dependsOn: [
    factory_Copy_SubscriptionRegistrations
  ]
}

resource Microsoft_DataFactory_factories_triggers__factory_Copy_SubscriptionMappings 'Microsoft.DataFactory/factories/triggers@2018-06-01' = {
  name: '${factory.name}/Copy SubscriptionMappings'
  properties: {
    annotations: [
    ]
    pipelines: [
      {
        pipelineReference: {
          referenceName: 'Copy SubscriptionMappings'
          type: 'PipelineReference'
        }
        parameters: {
          startDate: '@{formatDateTime(addhours(trigger().scheduledTime, -19), \'yyyy/MM/dd\')}'
        }
      }
    ]
    type: 'ScheduleTrigger'
    typeProperties: {
      recurrence: {
        frequency: 'Day'
        interval: 1
        startTime: '11/27/2018 12:00:00 PM'
        timeZone: 'UTC'
        schedule: {
          minutes: [
            0
          ]
          hours: [
            19
          ]
        }
      }
    }
  }
  dependsOn: [
    factory_Copy_SubscriptionMappings
  ]
}

resource Microsoft_DataFactory_factories_triggers__factory_Copy_Subscriptions 'Microsoft.DataFactory/factories/triggers@2018-06-01' = {
  name: '${factory.name}/Copy Subscriptions'
  properties: {
    annotations: [
    ]
    pipelines: [
      {
        pipelineReference: {
          referenceName: 'Copy Subscriptions'
          type: 'PipelineReference'
        }
        parameters: {
          startDate: '@{formatDateTime(addhours(trigger().scheduledTime, -20), \'yyyy/MM/dd\')}'
        }
      }
    ]
    type: 'ScheduleTrigger'
    typeProperties: {
      recurrence: {
        frequency: 'Day'
        interval: 1
        startTime: '11/27/2018 12:00:00 PM'
        timeZone: 'UTC'
        schedule: {
          minutes: [
            0
          ]
          hours: [
            20
          ]
        }
      }
    }
  }
  dependsOn: [
    factory_Copy_Subscriptions
  ]
}

resource Microsoft_DataFactory_factories_triggers__factory_Copy_SubscriptionAdmins 'Microsoft.DataFactory/factories/triggers@2018-06-01' = {
  name: '${factory.name}/Copy SubscriptionAdmins'
  properties: {
    annotations: [
    ]
    pipelines: [
      {
        pipelineReference: {
          referenceName: 'Copy SubscriptionAdmins'
          type: 'PipelineReference'
        }
        parameters: {
          startDate: '@{formatDateTime(addhours(trigger().scheduledTime, -21), \'yyyy/MM/dd\')}'
        }
      }
    ]
    type: 'ScheduleTrigger'
    typeProperties: {
      recurrence: {
        frequency: 'Day'
        interval: 1
        startTime: '11/27/2018 12:00:00 PM'
        timeZone: 'UTC'
        schedule: {
          minutes: [
            0
          ]
          hours: [
            21
          ]
        }
      }
    }
  }
  dependsOn: [
    factory_Copy_SubscriptionAdmins
  ]
}

resource Microsoft_DataFactory_factories_triggers__factory_Copy_SubscriptionUserProvisioningRequests 'Microsoft.DataFactory/factories/triggers@2018-06-01' = {
  name: '${factory.name}/Copy SubscriptionUserProvisioningRequests'
  properties: {
    annotations: [
    ]
    pipelines: [
      {
        pipelineReference: {
          referenceName: 'Copy SubscriptionUserProvisioningRequests'
          type: 'PipelineReference'
        }
        parameters: {
          startDate: '@{formatDateTime(addhours(trigger().scheduledTime, -22), \'yyyy/MM/dd\')}'
        }
      }
    ]
    type: 'ScheduleTrigger'
    typeProperties: {
      recurrence: {
        frequency: 'Day'
        interval: 1
        startTime: '11/27/2018 12:00:00 PM'
        timeZone: 'UTC'
        schedule: {
          minutes: [
            0
          ]
          hours: [
            22
          ]
        }
      }
    }
  }
  dependsOn: [
    factory_Copy_SubscriptionUserProvisioningRequests
  ]
}

resource integrationRuntime 'Microsoft.DataFactory/factories/integrationRuntimes@2018-06-01' = {
  name: '${factory.name}/WestUsIntegrationRuntime'
  properties: {
    type: 'Managed'
    typeProperties: {
      computeProperties: {
        location: 'West US'
      }
    }
  }
}