resource basicStorage 'Microsoft.Storage/storageAccounts@2019-06-01' = {
  name: 'basicblobs'
  location: 'westus'
  kind: 'BlobStorage'
  sku: {
    name: 'Standard_GRS'
  }
}

resource dnsZone 'Microsoft.Network/dnszones@2018-05-01' = {
  name: 'myZone'
  location: 'global'
}

resource myStorageAccount 'Microsoft.Storage/storageAccounts@2017-10-01' = {
  name: 'myencryptedone'
  location: 'eastus2'
  properties: {
    supportsHttpsTrafficOnly: true
    accessTier: 'Hot'
    encryption: {
      keySource: 'Microsoft.Storage'
      services: {
        blob: {
          enabled: true
        }
        file: {
          enabled: true
        }
      }
    }
  }
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
  }
}

resource withExpressions 'Microsoft.Storage/storageAccounts@2017-10-01' = {
  name: 'myencryptedone2'
  location: 'eastus2'
  properties: {
    supportsHttpsTrafficOnly: !false
    accessTier: true ? 'Hot' : 'Cold'
//@[16:37) [BCP036 (Warning)] The property "accessTier" expected a value of type "'Cool' | 'Hot'" but the provided value is of type "'Cold' | 'Hot'". |true ? 'Hot' : 'Cold'|
    encryption: {
      keySource: 'Microsoft.Storage'
      services: {
        blob: {
          enabled: true || false
        }
        file: {
          enabled: true
        }
      }
    }
  }
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
  }
  dependsOn: [
    myStorageAccount
  ]
}

param applicationName string = 'to-do-app${uniqueString(resourceGroup().id)}'
var hostingPlanName = applicationName // why not just use the param directly?

param appServicePlanTier string
param appServicePlanInstances int

var location = resourceGroup().location

resource farm 'Microsoft.Web/serverFarms@2019-08-01' = {
  // dependsOn: resourceId('Microsoft.DocumentDB/databaseAccounts', cosmosAccountName)
  name: hostingPlanName
  location: location
  sku: {
    name: appServicePlanTier
    capacity: appServicePlanInstances
  }
  properties: {
    name: hostingPlanName // just hostingPlanName results in an error
//@[4:8) [BCP038 (Warning)] The property "name" is not allowed on objects of type "schemas:6_properties". Permissible properties include "freeOfferExpirationTime", "hostingEnvironmentProfile", "hyperV", "isSpot", "isXenon", "maximumElasticWorkerCount", "perSiteScaling", "reserved", "spotExpirationTime", "targetWorkerCount", "targetWorkerSizeId", "workerTierName". |name|
  }
}

var cosmosDbResourceId = resourceId('Microsoft.DocumentDB/databaseAccounts', cosmosDb.account)
var cosmosDbRef = reference(cosmosDbResourceId).documentEndpoint

// this variable is not accessed anywhere in this template and depends on a run-time reference
// it should not be present at all in the template output as there is nowhere logical to put it
var cosmosDbEndpoint = cosmosDbRef.documentEndpoint

param webSiteName string
param cosmosDb object
resource site 'Microsoft.Web/sites@2019-08-01' = {
  name: webSiteName
  location: location
  properties: {
    // not yet supported // serverFarmId: farm.id
    siteConfig: {
      appSettings: [
        {
          name: 'CosmosDb:Account'
          value: reference(cosmosDbResourceId).documentEndpoint
        }
        {
          name: 'CosmosDb:Key'
          value: listKeys(cosmosDbResourceId, '2020-04-01').primaryMasterKey
        }
        {
          name: 'CosmosDb:DatabaseName'
          value: cosmosDb.databaseName
        }
        {
          name: 'CosmosDb:ContainerName'
          value: cosmosDb.containerName
        }
      ]
    }
  }
}

var _siteApiVersion = site.apiVersion
var _siteType = site.type

output siteApiVersion string = site.apiVersion
output siteType string = site.type

resource nested 'Microsoft.Resources/deployments@2019-10-01' = {
  name: 'nestedTemplate1'
  properties: {
    mode: 'Incremental'
    template: {
      // string key value
      '$schema': 'https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#'
      contentVersion: '1.0.0.0'
      resources: [
      ]
    }
  }
}

// should be able to access the read only properties
resource accessingReadOnlyProperties 'Microsoft.Foo/foos@2019-10-01' = {
//@[37:68) [BCP081 (Warning)] Resource type "Microsoft.Foo/foos@2019-10-01" does not have types available. |'Microsoft.Foo/foos@2019-10-01'|
  name: 'nestedTemplate1'
  properties: {
    otherId: nested.id
    otherName: nested.name
    otherVersion: nested.apiVersion
    otherType: nested.type

    otherThings: nested.properties.mode
  }
}

resource resourceA 'My.Rp/typeA@2020-01-01' = {
//@[19:43) [BCP081 (Warning)] Resource type "My.Rp/typeA@2020-01-01" does not have types available. |'My.Rp/typeA@2020-01-01'|
  name: 'resourceA'
}

resource resourceB 'My.Rp/typeA/typeB@2020-01-01' = {
//@[19:49) [BCP081 (Warning)] Resource type "My.Rp/typeA/typeB@2020-01-01" does not have types available. |'My.Rp/typeA/typeB@2020-01-01'|
  name: '${resourceA.name}/myName'
}

resource resourceC 'My.Rp/typeA/typeB@2020-01-01' = {
//@[19:49) [BCP081 (Warning)] Resource type "My.Rp/typeA/typeB@2020-01-01" does not have types available. |'My.Rp/typeA/typeB@2020-01-01'|
  name: '${resourceA.name}/myName'
  properties: {
    aId: resourceA.id
    aType: resourceA.type
    aName: resourceA.name
    aApiVersion: resourceA.apiVersion
    bProperties: resourceB.properties
  }
}

var varARuntime = {
  bId: resourceB.id
  bType: resourceB.type
  bName: resourceB.name
  bApiVersion: resourceB.apiVersion
  aKind: resourceA.kind
}

var varBRuntime = [
  varARuntime
]

var resourceCRef = {
  id: resourceC.id
}
var setResourceCRef = true

resource resourceD 'My.Rp/typeD@2020-01-01' = {
//@[19:43) [BCP081 (Warning)] Resource type "My.Rp/typeD@2020-01-01" does not have types available. |'My.Rp/typeD@2020-01-01'|
  name: 'constant'
  properties: {
    runtime: varBRuntime
    // repro for https://github.com/Azure/bicep/issues/316
    repro316: setResourceCRef ? resourceCRef : null
  }
}

var myInterpKey = 'abc'
resource resourceWithInterp 'My.Rp/interp@2020-01-01' = {
//@[28:53) [BCP081 (Warning)] Resource type "My.Rp/interp@2020-01-01" does not have types available. |'My.Rp/interp@2020-01-01'|
  name: 'interpTest'
  properties: {
    '${myInterpKey}': 1
    'abc${myInterpKey}def': 2
    '${myInterpKey}abc${myInterpKey}': 3
  }
}

resource resourceWithEscaping 'My.Rp/mockResource@2020-01-01' = {
//@[30:61) [BCP081 (Warning)] Resource type "My.Rp/mockResource@2020-01-01" does not have types available. |'My.Rp/mockResource@2020-01-01'|
  name: 'test'
  properties: {
    // both key and value should be escaped in template output
    '[resourceGroup().location]': '[resourceGroup().location]'
  }
}

param shouldDeployVm bool = true
resource vmWithCondition 'Microsoft.Compute/virtualMachines@2020-06-01' = if (shouldDeployVm) {
  name: 'vmName'
  location: 'westus'
  properties: {
    osProfile: {
      windowsConfiguration: {
        enableAutomaticUpdates: true
      }
    }
  }
}

resource extension1 'My.Rp/extensionResource@2020-12-01' = {
//@[20:56) [BCP081 (Warning)] Resource type "My.Rp/extensionResource@2020-12-01" does not have types available. |'My.Rp/extensionResource@2020-12-01'|
  name: 'extension'
  scope: vmWithCondition
}

resource extension2 'My.Rp/extensionResource@2020-12-01' = {
//@[20:56) [BCP081 (Warning)] Resource type "My.Rp/extensionResource@2020-12-01" does not have types available. |'My.Rp/extensionResource@2020-12-01'|
  name: 'extension'
  scope: extension1
}

resource extensionDependencies 'My.Rp/mockResource@2020-01-01' = {
//@[31:62) [BCP081 (Warning)] Resource type "My.Rp/mockResource@2020-01-01" does not have types available. |'My.Rp/mockResource@2020-01-01'|
  name: 'extensionDependencies'
  properties: {
    res1: vmWithCondition.id
    res1runtime: vmWithCondition.properties.something
//@[44:53) [BCP053 (Warning)] The type "VirtualMachineProperties" does not contain property "something". Available properties include "additionalCapabilities", "availabilitySet", "billingProfile", "diagnosticsProfile", "evictionPolicy", "extensionsTimeBudget", "hardwareProfile", "host", "hostGroup", "instanceView", "licenseType", "networkProfile", "osProfile", "priority", "provisioningState", "proximityPlacementGroup", "securityProfile", "storageProfile", "virtualMachineScaleSet", "vmId". |something|
    res2: extension1.id
    res2runtime: extension1.properties.something
    res3: extension2.id
    res3runtime: extension2.properties.something
  }
}

resource existing1 'Mock.Rp/existingExtensionResource@2020-01-01' existing = {
//@[19:65) [BCP081 (Warning)] Resource type "Mock.Rp/existingExtensionResource@2020-01-01" does not have types available. |'Mock.Rp/existingExtensionResource@2020-01-01'|
  name: 'existing1'
  scope: extension1
}

resource existing2 'Mock.Rp/existingExtensionResource@2020-01-01' existing = {
//@[19:65) [BCP081 (Warning)] Resource type "Mock.Rp/existingExtensionResource@2020-01-01" does not have types available. |'Mock.Rp/existingExtensionResource@2020-01-01'|
  name: 'existing2'
  scope: existing1
}

resource extension3 'My.Rp/extensionResource@2020-12-01' = {
//@[20:56) [BCP081 (Warning)] Resource type "My.Rp/extensionResource@2020-12-01" does not have types available. |'My.Rp/extensionResource@2020-12-01'|
  name: 'extension3'
  scope: existing1
}
