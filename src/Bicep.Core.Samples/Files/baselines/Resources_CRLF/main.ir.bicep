
//@[000:14067) ProgramExpression
//@[000:00000) | └─ResourceDependencyExpression [UNPARENTED]
//@[000:00000) |   └─ResourceReferenceExpression [UNPARENTED]
//@[000:00000) | └─ResourceDependencyExpression [UNPARENTED]
//@[000:00000) |   └─ResourceReferenceExpression [UNPARENTED]
//@[000:00000) | └─ResourceDependencyExpression [UNPARENTED]
//@[000:00000) |   └─ResourceReferenceExpression [UNPARENTED]
//@[000:00000) | └─ResourceDependencyExpression [UNPARENTED]
//@[000:00000) |   └─ResourceReferenceExpression [UNPARENTED]
//@[000:00000) | └─ResourceDependencyExpression [UNPARENTED]
//@[000:00000) |   └─ResourceReferenceExpression [UNPARENTED]
//@[000:00000) | └─ResourceDependencyExpression [UNPARENTED]
//@[000:00000) |   └─ResourceReferenceExpression [UNPARENTED]
//@[000:00000) | ├─ResourceDependencyExpression [UNPARENTED]
//@[000:00000) | | └─ResourceReferenceExpression [UNPARENTED]
//@[000:00000) | └─ResourceDependencyExpression [UNPARENTED]
//@[000:00000) |   └─ResourceReferenceExpression [UNPARENTED]
//@[000:00000) | ├─ResourceDependencyExpression [UNPARENTED]
//@[000:00000) | | └─ResourceReferenceExpression [UNPARENTED]
//@[000:00000) | ├─ResourceDependencyExpression [UNPARENTED]
//@[000:00000) | | └─ResourceReferenceExpression [UNPARENTED]
//@[000:00000) | └─ResourceDependencyExpression [UNPARENTED]
//@[000:00000) |   └─ResourceReferenceExpression [UNPARENTED]
//@[000:00000) | └─ResourceDependencyExpression [UNPARENTED]
//@[000:00000) |   └─ResourceReferenceExpression [UNPARENTED]
//@[000:00000) | └─ResourceDependencyExpression [UNPARENTED]
//@[000:00000) |   └─ResourceReferenceExpression [UNPARENTED]
//@[000:00000) | ├─ResourceDependencyExpression [UNPARENTED]
//@[000:00000) | | └─ResourceReferenceExpression [UNPARENTED]
//@[000:00000) | ├─ResourceDependencyExpression [UNPARENTED]
//@[000:00000) | | └─ResourceReferenceExpression [UNPARENTED]
//@[000:00000) | └─ResourceDependencyExpression [UNPARENTED]
//@[000:00000) |   └─ResourceReferenceExpression [UNPARENTED]
//@[000:00000) | └─ResourceDependencyExpression [UNPARENTED]
//@[000:00000) |   └─ResourceReferenceExpression [UNPARENTED]
//@[000:00000) | └─ResourceDependencyExpression [UNPARENTED]
//@[000:00000) |   └─ResourceReferenceExpression [UNPARENTED]
//@[000:00000) | └─ResourceDependencyExpression [UNPARENTED]
//@[000:00000) |   └─ResourceReferenceExpression [UNPARENTED]
//@[000:00000) | └─ResourceDependencyExpression [UNPARENTED]
//@[000:00000) |   └─ResourceReferenceExpression [UNPARENTED]
//@[000:00000) | └─ResourceDependencyExpression [UNPARENTED]
//@[000:00000) |   └─ResourceReferenceExpression [UNPARENTED]
//@[000:00000) | └─ResourceDependencyExpression [UNPARENTED]
//@[000:00000) |   └─ResourceReferenceExpression [UNPARENTED]
//@[000:00000) | └─ResourceDependencyExpression [UNPARENTED]
//@[000:00000) |   └─ResourceReferenceExpression [UNPARENTED]
//@[000:00000) | └─ResourceDependencyExpression [UNPARENTED]
//@[000:00000) |   └─ResourceReferenceExpression [UNPARENTED]
//@[000:00000) | └─ResourceDependencyExpression [UNPARENTED]
//@[000:00000) |   └─ResourceReferenceExpression [UNPARENTED]
//@[000:00000) | └─ResourceDependencyExpression [UNPARENTED]
//@[000:00000) |   └─ResourceReferenceExpression [UNPARENTED]
//@[000:00000) | └─ResourceDependencyExpression [UNPARENTED]
//@[000:00000) |   └─ResourceReferenceExpression [UNPARENTED]
@sys.description('this is basicStorage')
//@[000:00225) ├─DeclaredResourceExpression
//@[017:00039) | ├─StringLiteralExpression { Value = this is basicStorage }
resource basicStorage 'Microsoft.Storage/storageAccounts@2019-06-01' = {
//@[071:00183) | └─ObjectExpression
  name: 'basicblobs'
  location: 'westus'
//@[002:00020) |   ├─ObjectPropertyExpression
//@[002:00010) |   | ├─StringLiteralExpression { Value = location }
//@[012:00020) |   | └─StringLiteralExpression { Value = westus }
  kind: 'BlobStorage'
//@[002:00021) |   ├─ObjectPropertyExpression
//@[002:00006) |   | ├─StringLiteralExpression { Value = kind }
//@[008:00021) |   | └─StringLiteralExpression { Value = BlobStorage }
  sku: {
//@[002:00039) |   └─ObjectPropertyExpression
//@[002:00005) |     ├─StringLiteralExpression { Value = sku }
//@[007:00039) |     └─ObjectExpression
    name: 'Standard_GRS'
//@[004:00024) |       └─ObjectPropertyExpression
//@[004:00008) |         ├─StringLiteralExpression { Value = name }
//@[010:00024) |         └─StringLiteralExpression { Value = Standard_GRS }
  }
}

@sys.description('this is dnsZone')
//@[000:00140) ├─DeclaredResourceExpression
//@[017:00034) | ├─StringLiteralExpression { Value = this is dnsZone }
resource dnsZone 'Microsoft.Network/dnszones@2018-05-01' = {
//@[059:00103) | └─ObjectExpression
  name: 'myZone'
  location: 'global'
//@[002:00020) |   └─ObjectPropertyExpression
//@[002:00010) |     ├─StringLiteralExpression { Value = location }
//@[012:00020) |     └─StringLiteralExpression { Value = global }
}

resource myStorageAccount 'Microsoft.Storage/storageAccounts@2017-10-01' = {
//@[000:00469) ├─DeclaredResourceExpression
//@[075:00469) | └─ObjectExpression
  name: 'myencryptedone'
  location: 'eastus2'
//@[002:00021) |   ├─ObjectPropertyExpression
//@[002:00010) |   | ├─StringLiteralExpression { Value = location }
//@[012:00021) |   | └─StringLiteralExpression { Value = eastus2 }
  properties: {
//@[002:00277) |   ├─ObjectPropertyExpression
//@[002:00012) |   | ├─StringLiteralExpression { Value = properties }
//@[014:00277) |   | └─ObjectExpression
    supportsHttpsTrafficOnly: true
//@[004:00034) |   |   ├─ObjectPropertyExpression
//@[004:00028) |   |   | ├─StringLiteralExpression { Value = supportsHttpsTrafficOnly }
//@[030:00034) |   |   | └─BooleanLiteralExpression { Value = True }
    accessTier: 'Hot'
//@[004:00021) |   |   ├─ObjectPropertyExpression
//@[004:00014) |   |   | ├─StringLiteralExpression { Value = accessTier }
//@[016:00021) |   |   | └─StringLiteralExpression { Value = Hot }
    encryption: {
//@[004:00196) |   |   └─ObjectPropertyExpression
//@[004:00014) |   |     ├─StringLiteralExpression { Value = encryption }
//@[016:00196) |   |     └─ObjectExpression
      keySource: 'Microsoft.Storage'
//@[006:00036) |   |       ├─ObjectPropertyExpression
//@[006:00015) |   |       | ├─StringLiteralExpression { Value = keySource }
//@[017:00036) |   |       | └─StringLiteralExpression { Value = Microsoft.Storage }
      services: {
//@[006:00132) |   |       └─ObjectPropertyExpression
//@[006:00014) |   |         ├─StringLiteralExpression { Value = services }
//@[016:00132) |   |         └─ObjectExpression
        blob: {
//@[008:00051) |   |           ├─ObjectPropertyExpression
//@[008:00012) |   |           | ├─StringLiteralExpression { Value = blob }
//@[014:00051) |   |           | └─ObjectExpression
          enabled: true
//@[010:00023) |   |           |   └─ObjectPropertyExpression
//@[010:00017) |   |           |     ├─StringLiteralExpression { Value = enabled }
//@[019:00023) |   |           |     └─BooleanLiteralExpression { Value = True }
        }
        file: {
//@[008:00051) |   |           └─ObjectPropertyExpression
//@[008:00012) |   |             ├─StringLiteralExpression { Value = file }
//@[014:00051) |   |             └─ObjectExpression
          enabled: true
//@[010:00023) |   |               └─ObjectPropertyExpression
//@[010:00017) |   |                 ├─StringLiteralExpression { Value = enabled }
//@[019:00023) |   |                 └─BooleanLiteralExpression { Value = True }
        }
      }
    }
  }
  kind: 'StorageV2'
//@[002:00019) |   ├─ObjectPropertyExpression
//@[002:00006) |   | ├─StringLiteralExpression { Value = kind }
//@[008:00019) |   | └─StringLiteralExpression { Value = StorageV2 }
  sku: {
//@[002:00039) |   └─ObjectPropertyExpression
//@[002:00005) |     ├─StringLiteralExpression { Value = sku }
//@[007:00039) |     └─ObjectExpression
    name: 'Standard_LRS'
//@[004:00024) |       └─ObjectPropertyExpression
//@[004:00008) |         ├─StringLiteralExpression { Value = name }
//@[010:00024) |         └─StringLiteralExpression { Value = Standard_LRS }
  }
}

resource withExpressions 'Microsoft.Storage/storageAccounts@2017-10-01' = {
//@[000:00539) ├─DeclaredResourceExpression
//@[074:00539) | ├─ObjectExpression
  name: 'myencryptedone2'
  location: 'eastus2'
//@[002:00021) | | ├─ObjectPropertyExpression
//@[002:00010) | | | ├─StringLiteralExpression { Value = location }
//@[012:00021) | | | └─StringLiteralExpression { Value = eastus2 }
  properties: {
//@[002:00304) | | ├─ObjectPropertyExpression
//@[002:00012) | | | ├─StringLiteralExpression { Value = properties }
//@[014:00304) | | | └─ObjectExpression
    supportsHttpsTrafficOnly: !false
//@[004:00036) | | |   ├─ObjectPropertyExpression
//@[004:00028) | | |   | ├─StringLiteralExpression { Value = supportsHttpsTrafficOnly }
//@[030:00036) | | |   | └─UnaryExpression { Operator = Not }
//@[031:00036) | | |   |   └─BooleanLiteralExpression { Value = False }
    accessTier: true ? 'Hot' : 'Cold'
//@[004:00037) | | |   ├─ObjectPropertyExpression
//@[004:00014) | | |   | ├─StringLiteralExpression { Value = accessTier }
//@[016:00037) | | |   | └─TernaryExpression
//@[016:00020) | | |   |   ├─BooleanLiteralExpression { Value = True }
//@[023:00028) | | |   |   ├─StringLiteralExpression { Value = Hot }
//@[031:00037) | | |   |   └─StringLiteralExpression { Value = Cold }
    encryption: {
//@[004:00205) | | |   └─ObjectPropertyExpression
//@[004:00014) | | |     ├─StringLiteralExpression { Value = encryption }
//@[016:00205) | | |     └─ObjectExpression
      keySource: 'Microsoft.Storage'
//@[006:00036) | | |       ├─ObjectPropertyExpression
//@[006:00015) | | |       | ├─StringLiteralExpression { Value = keySource }
//@[017:00036) | | |       | └─StringLiteralExpression { Value = Microsoft.Storage }
      services: {
//@[006:00141) | | |       └─ObjectPropertyExpression
//@[006:00014) | | |         ├─StringLiteralExpression { Value = services }
//@[016:00141) | | |         └─ObjectExpression
        blob: {
//@[008:00060) | | |           ├─ObjectPropertyExpression
//@[008:00012) | | |           | ├─StringLiteralExpression { Value = blob }
//@[014:00060) | | |           | └─ObjectExpression
          enabled: true || false
//@[010:00032) | | |           |   └─ObjectPropertyExpression
//@[010:00017) | | |           |     ├─StringLiteralExpression { Value = enabled }
//@[019:00032) | | |           |     └─BinaryExpression { Operator = LogicalOr }
//@[019:00023) | | |           |       ├─BooleanLiteralExpression { Value = True }
//@[027:00032) | | |           |       └─BooleanLiteralExpression { Value = False }
        }
        file: {
//@[008:00051) | | |           └─ObjectPropertyExpression
//@[008:00012) | | |             ├─StringLiteralExpression { Value = file }
//@[014:00051) | | |             └─ObjectExpression
          enabled: true
//@[010:00023) | | |               └─ObjectPropertyExpression
//@[010:00017) | | |                 ├─StringLiteralExpression { Value = enabled }
//@[019:00023) | | |                 └─BooleanLiteralExpression { Value = True }
        }
      }
    }
  }
  kind: 'StorageV2'
//@[002:00019) | | ├─ObjectPropertyExpression
//@[002:00006) | | | ├─StringLiteralExpression { Value = kind }
//@[008:00019) | | | └─StringLiteralExpression { Value = StorageV2 }
  sku: {
//@[002:00039) | | └─ObjectPropertyExpression
//@[002:00005) | |   ├─StringLiteralExpression { Value = sku }
//@[007:00039) | |   └─ObjectExpression
    name: 'Standard_LRS'
//@[004:00024) | |     └─ObjectPropertyExpression
//@[004:00008) | |       ├─StringLiteralExpression { Value = name }
//@[010:00024) | |       └─StringLiteralExpression { Value = Standard_LRS }
  }
  dependsOn: [
    myStorageAccount
  ]
}

param applicationName string = 'to-do-app${uniqueString(resourceGroup().id)}'
//@[000:00077) ├─DeclaredParameterExpression { Name = applicationName }
//@[022:00028) | ├─AmbientTypeReferenceExpression { Name = string }
//@[031:00077) | └─InterpolatedStringExpression
//@[043:00075) |   └─FunctionCallExpression { Name = uniqueString }
//@[056:00074) |     └─PropertyAccessExpression { PropertyName = id }
//@[056:00071) |       └─FunctionCallExpression { Name = resourceGroup }
var hostingPlanName = applicationName // why not just use the param directly?
//@[000:00037) ├─DeclaredVariableExpression { Name = hostingPlanName }
//@[022:00037) | └─ParametersReferenceExpression { Parameter = applicationName }

param appServicePlanTier string
//@[000:00031) ├─DeclaredParameterExpression { Name = appServicePlanTier }
//@[025:00031) | └─AmbientTypeReferenceExpression { Name = string }
param appServicePlanInstances int
//@[000:00033) ├─DeclaredParameterExpression { Name = appServicePlanInstances }
//@[030:00033) | └─AmbientTypeReferenceExpression { Name = int }

var location = resourceGroup().location
//@[000:00039) ├─DeclaredVariableExpression { Name = location }
//@[015:00039) | └─PropertyAccessExpression { PropertyName = location }
//@[015:00030) |   └─FunctionCallExpression { Name = resourceGroup }

resource farm 'Microsoft.Web/serverFarms@2019-08-01' = {
//@[000:00371) ├─DeclaredResourceExpression
//@[055:00371) | └─ObjectExpression
  // dependsOn: resourceId('Microsoft.DocumentDB/databaseAccounts', cosmosAccountName)
  name: hostingPlanName
  location: location
//@[002:00020) |   ├─ObjectPropertyExpression
//@[002:00010) |   | ├─StringLiteralExpression { Value = location }
//@[012:00020) |   | └─VariableReferenceExpression { Variable = location }
  sku: {
//@[002:00082) |   ├─ObjectPropertyExpression
//@[002:00005) |   | ├─StringLiteralExpression { Value = sku }
//@[007:00082) |   | └─ObjectExpression
    name: appServicePlanTier
//@[004:00028) |   |   ├─ObjectPropertyExpression
//@[004:00008) |   |   | ├─StringLiteralExpression { Value = name }
//@[010:00028) |   |   | └─ParametersReferenceExpression { Parameter = appServicePlanTier }
    capacity: appServicePlanInstances
//@[004:00037) |   |   └─ObjectPropertyExpression
//@[004:00012) |   |     ├─StringLiteralExpression { Value = capacity }
//@[014:00037) |   |     └─ParametersReferenceExpression { Parameter = appServicePlanInstances }
  }
  properties: {
//@[002:00091) |   └─ObjectPropertyExpression
//@[002:00012) |     ├─StringLiteralExpression { Value = properties }
//@[014:00091) |     └─ObjectExpression
    name: hostingPlanName // just hostingPlanName results in an error
//@[004:00025) |       └─ObjectPropertyExpression
//@[004:00008) |         ├─StringLiteralExpression { Value = name }
//@[010:00025) |         └─VariableReferenceExpression { Variable = hostingPlanName }
  }
}

var cosmosDbResourceId = resourceId('Microsoft.DocumentDB/databaseAccounts',
//@[000:00107) ├─DeclaredVariableExpression { Name = cosmosDbResourceId }
//@[025:00107) | └─FunctionCallExpression { Name = resourceId }
//@[036:00075) |   ├─StringLiteralExpression { Value = Microsoft.DocumentDB/databaseAccounts }
// comment
cosmosDb.account)
//@[000:00016) |   └─PropertyAccessExpression { PropertyName = account }
//@[000:00008) |     └─ParametersReferenceExpression { Parameter = cosmosDb }
var cosmosDbRef = reference(cosmosDbResourceId).documentEndpoint

// this variable is not accessed anywhere in this template and depends on a run-time reference
// it should not be present at all in the template output as there is nowhere logical to put it
var cosmosDbEndpoint = cosmosDbRef.documentEndpoint

param webSiteName string
//@[000:00024) ├─DeclaredParameterExpression { Name = webSiteName }
//@[018:00024) | └─AmbientTypeReferenceExpression { Name = string }
param cosmosDb object
//@[000:00021) ├─DeclaredParameterExpression { Name = cosmosDb }
//@[015:00021) | └─AmbientTypeReferenceExpression { Name = object }
resource site 'Microsoft.Web/sites@2019-08-01' = {
//@[000:00689) ├─DeclaredResourceExpression
//@[049:00689) | └─ObjectExpression
  name: webSiteName
  location: location
//@[002:00020) |   ├─ObjectPropertyExpression
//@[002:00010) |   | ├─StringLiteralExpression { Value = location }
//@[012:00020) |   | └─VariableReferenceExpression { Variable = location }
  properties: {
//@[002:00591) |   └─ObjectPropertyExpression
//@[002:00012) |     ├─StringLiteralExpression { Value = properties }
//@[014:00591) |     └─ObjectExpression
    // not yet supported // serverFarmId: farm.id
    siteConfig: {
//@[004:00518) |       └─ObjectPropertyExpression
//@[004:00014) |         ├─StringLiteralExpression { Value = siteConfig }
//@[016:00518) |         └─ObjectExpression
      appSettings: [
//@[006:00492) |           └─ObjectPropertyExpression
//@[006:00017) |             ├─StringLiteralExpression { Value = appSettings }
//@[019:00492) |             └─ArrayExpression
        {
//@[008:00121) |               ├─ObjectExpression
          name: 'CosmosDb:Account'
//@[010:00034) |               | ├─ObjectPropertyExpression
//@[010:00014) |               | | ├─StringLiteralExpression { Value = name }
//@[016:00034) |               | | └─StringLiteralExpression { Value = CosmosDb:Account }
          value: reference(cosmosDbResourceId).documentEndpoint
//@[010:00063) |               | └─ObjectPropertyExpression
//@[010:00015) |               |   ├─StringLiteralExpression { Value = value }
//@[017:00063) |               |   └─PropertyAccessExpression { PropertyName = documentEndpoint }
//@[017:00046) |               |     └─FunctionCallExpression { Name = reference }
//@[027:00045) |               |       └─VariableReferenceExpression { Variable = cosmosDbResourceId }
        }
        {
//@[008:00130) |               ├─ObjectExpression
          name: 'CosmosDb:Key'
//@[010:00030) |               | ├─ObjectPropertyExpression
//@[010:00014) |               | | ├─StringLiteralExpression { Value = name }
//@[016:00030) |               | | └─StringLiteralExpression { Value = CosmosDb:Key }
          value: listKeys(cosmosDbResourceId, '2020-04-01').primaryMasterKey
//@[010:00076) |               | └─ObjectPropertyExpression
//@[010:00015) |               |   ├─StringLiteralExpression { Value = value }
//@[017:00076) |               |   └─PropertyAccessExpression { PropertyName = primaryMasterKey }
//@[017:00059) |               |     └─FunctionCallExpression { Name = listKeys }
//@[026:00044) |               |       ├─VariableReferenceExpression { Variable = cosmosDbResourceId }
//@[046:00058) |               |       └─StringLiteralExpression { Value = 2020-04-01 }
        }
        {
//@[008:00101) |               ├─ObjectExpression
          name: 'CosmosDb:DatabaseName'
//@[010:00039) |               | ├─ObjectPropertyExpression
//@[010:00014) |               | | ├─StringLiteralExpression { Value = name }
//@[016:00039) |               | | └─StringLiteralExpression { Value = CosmosDb:DatabaseName }
          value: cosmosDb.databaseName
//@[010:00038) |               | └─ObjectPropertyExpression
//@[010:00015) |               |   ├─StringLiteralExpression { Value = value }
//@[017:00038) |               |   └─PropertyAccessExpression { PropertyName = databaseName }
//@[017:00025) |               |     └─ParametersReferenceExpression { Parameter = cosmosDb }
        }
        {
//@[008:00103) |               └─ObjectExpression
          name: 'CosmosDb:ContainerName'
//@[010:00040) |                 ├─ObjectPropertyExpression
//@[010:00014) |                 | ├─StringLiteralExpression { Value = name }
//@[016:00040) |                 | └─StringLiteralExpression { Value = CosmosDb:ContainerName }
          value: cosmosDb.containerName
//@[010:00039) |                 └─ObjectPropertyExpression
//@[010:00015) |                   ├─StringLiteralExpression { Value = value }
//@[017:00039) |                   └─PropertyAccessExpression { PropertyName = containerName }
//@[017:00025) |                     └─ParametersReferenceExpression { Parameter = cosmosDb }
        }
      ]
    }
  }
}

var _siteApiVersion = site.apiVersion
//@[000:00037) ├─DeclaredVariableExpression { Name = _siteApiVersion }
//@[022:00037) | └─PropertyAccessExpression { PropertyName = apiVersion }
//@[022:00026) |   └─ResourceReferenceExpression
var _siteType = site.type
//@[000:00025) ├─DeclaredVariableExpression { Name = _siteType }
//@[016:00025) | └─PropertyAccessExpression { PropertyName = type }
//@[016:00020) |   └─ResourceReferenceExpression

output siteApiVersion string = site.apiVersion
//@[000:00046) ├─DeclaredOutputExpression { Name = siteApiVersion }
//@[022:00028) | ├─AmbientTypeReferenceExpression { Name = string }
//@[031:00046) | └─PropertyAccessExpression { PropertyName = apiVersion }
//@[031:00035) |   └─ResourceReferenceExpression
output siteType string = site.type
//@[000:00034) ├─DeclaredOutputExpression { Name = siteType }
//@[016:00022) | ├─AmbientTypeReferenceExpression { Name = string }
//@[025:00034) | └─PropertyAccessExpression { PropertyName = type }
//@[025:00029) |   └─ResourceReferenceExpression

resource nested 'Microsoft.Resources/deployments@2019-10-01' = {
//@[000:00354) ├─DeclaredResourceExpression
//@[063:00354) | └─ObjectExpression
  name: 'nestedTemplate1'
  properties: {
//@[002:00258) |   └─ObjectPropertyExpression
//@[002:00012) |     ├─StringLiteralExpression { Value = properties }
//@[014:00258) |     └─ObjectExpression
    mode: 'Incremental'
//@[004:00023) |       ├─ObjectPropertyExpression
//@[004:00008) |       | ├─StringLiteralExpression { Value = mode }
//@[010:00023) |       | └─StringLiteralExpression { Value = Incremental }
    template: {
//@[004:00211) |       └─ObjectPropertyExpression
//@[004:00012) |         ├─StringLiteralExpression { Value = template }
//@[014:00211) |         └─ObjectExpression
      // string key value
      '$schema': 'https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#'
//@[006:00098) |           ├─ObjectPropertyExpression
//@[006:00015) |           | ├─StringLiteralExpression { Value = $schema }
//@[017:00098) |           | └─StringLiteralExpression { Value = https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json# }
      contentVersion: '1.0.0.0'
//@[006:00031) |           ├─ObjectPropertyExpression
//@[006:00020) |           | ├─StringLiteralExpression { Value = contentVersion }
//@[022:00031) |           | └─StringLiteralExpression { Value = 1.0.0.0 }
      resources: [
//@[006:00027) |           └─ObjectPropertyExpression
//@[006:00015) |             ├─StringLiteralExpression { Value = resources }
//@[017:00027) |             └─ArrayExpression
      ]
    }
  }
}

// should be able to access the read only properties
resource accessingReadOnlyProperties 'Microsoft.Foo/foos@2019-10-01' = {
//@[000:00284) ├─DeclaredResourceExpression
//@[071:00284) | ├─ObjectExpression
  name: 'nestedTemplate1'
  properties: {
//@[002:00180) | | └─ObjectPropertyExpression
//@[002:00012) | |   ├─StringLiteralExpression { Value = properties }
//@[014:00180) | |   └─ObjectExpression
    otherId: nested.id
//@[004:00022) | |     ├─ObjectPropertyExpression
//@[004:00011) | |     | ├─StringLiteralExpression { Value = otherId }
//@[013:00022) | |     | └─PropertyAccessExpression { PropertyName = id }
//@[013:00019) | |     |   └─ResourceReferenceExpression
    otherName: nested.name
//@[004:00026) | |     ├─ObjectPropertyExpression
//@[004:00013) | |     | ├─StringLiteralExpression { Value = otherName }
//@[015:00026) | |     | └─PropertyAccessExpression { PropertyName = name }
//@[015:00021) | |     |   └─ResourceReferenceExpression
    otherVersion: nested.apiVersion
//@[004:00035) | |     ├─ObjectPropertyExpression
//@[004:00016) | |     | ├─StringLiteralExpression { Value = otherVersion }
//@[018:00035) | |     | └─PropertyAccessExpression { PropertyName = apiVersion }
//@[018:00024) | |     |   └─ResourceReferenceExpression
    otherType: nested.type
//@[004:00026) | |     ├─ObjectPropertyExpression
//@[004:00013) | |     | ├─StringLiteralExpression { Value = otherType }
//@[015:00026) | |     | └─PropertyAccessExpression { PropertyName = type }
//@[015:00021) | |     |   └─ResourceReferenceExpression

    otherThings: nested.properties.mode
//@[004:00039) | |     └─ObjectPropertyExpression
//@[004:00015) | |       ├─StringLiteralExpression { Value = otherThings }
//@[017:00039) | |       └─PropertyAccessExpression { PropertyName = mode }
//@[017:00034) | |         └─PropertyAccessExpression { PropertyName = properties }
//@[017:00023) | |           └─ResourceReferenceExpression
  }
}

resource resourceA 'My.Rp/typeA@2020-01-01' = {
//@[000:00071) ├─DeclaredResourceExpression
//@[046:00071) | └─ObjectExpression
  name: 'resourceA'
}

resource resourceB 'My.Rp/typeA/typeB@2020-01-01' = {
//@[000:00095) ├─DeclaredResourceExpression
//@[052:00095) | ├─ObjectExpression
  name: '${resourceA.name}/resourceB'
}

resource resourceC 'My.Rp/typeA/typeB@2020-01-01' = {
//@[000:00272) ├─DeclaredResourceExpression
//@[052:00272) | ├─ObjectExpression
  name: '${resourceA.name}/resourceC'
  properties: {
//@[002:00175) | | └─ObjectPropertyExpression
//@[002:00012) | |   ├─StringLiteralExpression { Value = properties }
//@[014:00175) | |   └─ObjectExpression
    aId: resourceA.id
//@[004:00021) | |     ├─ObjectPropertyExpression
//@[004:00007) | |     | ├─StringLiteralExpression { Value = aId }
//@[009:00021) | |     | └─PropertyAccessExpression { PropertyName = id }
//@[009:00018) | |     |   └─ResourceReferenceExpression
    aType: resourceA.type
//@[004:00025) | |     ├─ObjectPropertyExpression
//@[004:00009) | |     | ├─StringLiteralExpression { Value = aType }
//@[011:00025) | |     | └─PropertyAccessExpression { PropertyName = type }
//@[011:00020) | |     |   └─ResourceReferenceExpression
    aName: resourceA.name
//@[004:00025) | |     ├─ObjectPropertyExpression
//@[004:00009) | |     | ├─StringLiteralExpression { Value = aName }
//@[011:00025) | |     | └─PropertyAccessExpression { PropertyName = name }
//@[011:00020) | |     |   └─ResourceReferenceExpression
    aApiVersion: resourceA.apiVersion
//@[004:00037) | |     ├─ObjectPropertyExpression
//@[004:00015) | |     | ├─StringLiteralExpression { Value = aApiVersion }
//@[017:00037) | |     | └─PropertyAccessExpression { PropertyName = apiVersion }
//@[017:00026) | |     |   └─ResourceReferenceExpression
    bProperties: resourceB.properties
//@[004:00037) | |     └─ObjectPropertyExpression
//@[004:00015) | |       ├─StringLiteralExpression { Value = bProperties }
//@[017:00037) | |       └─PropertyAccessExpression { PropertyName = properties }
//@[017:00026) | |         └─ResourceReferenceExpression
  }
}

var varARuntime = {
//@[018:00155) | |     |   └─ObjectExpression
  bId: resourceB.id
//@[002:00019) | |     |     ├─ObjectPropertyExpression
//@[002:00005) | |     |     | ├─StringLiteralExpression { Value = bId }
//@[007:00019) | |     |     | └─PropertyAccessExpression { PropertyName = id }
//@[007:00016) | |     |     |   └─ResourceReferenceExpression
  bType: resourceB.type
//@[002:00023) | |     |     ├─ObjectPropertyExpression
//@[002:00007) | |     |     | ├─StringLiteralExpression { Value = bType }
//@[009:00023) | |     |     | └─PropertyAccessExpression { PropertyName = type }
//@[009:00018) | |     |     |   └─ResourceReferenceExpression
  bName: resourceB.name
//@[002:00023) | |     |     ├─ObjectPropertyExpression
//@[002:00007) | |     |     | ├─StringLiteralExpression { Value = bName }
//@[009:00023) | |     |     | └─PropertyAccessExpression { PropertyName = name }
//@[009:00018) | |     |     |   └─ResourceReferenceExpression
  bApiVersion: resourceB.apiVersion
//@[002:00035) | |     |     ├─ObjectPropertyExpression
//@[002:00013) | |     |     | ├─StringLiteralExpression { Value = bApiVersion }
//@[015:00035) | |     |     | └─PropertyAccessExpression { PropertyName = apiVersion }
//@[015:00024) | |     |     |   └─ResourceReferenceExpression
  aKind: resourceA.kind
//@[002:00023) | |     |     └─ObjectPropertyExpression
//@[002:00007) | |     |       ├─StringLiteralExpression { Value = aKind }
//@[009:00023) | |     |       └─PropertyAccessExpression { PropertyName = kind }
//@[009:00018) | |     |         └─ResourceReferenceExpression
}

var varBRuntime = [
//@[018:00037) | |     | └─ArrayExpression
  varARuntime
]

var resourceCRef = {
//@[000:00043) ├─DeclaredVariableExpression { Name = resourceCRef }
//@[019:00043) | └─ObjectExpression
  id: resourceC.id
//@[002:00018) |   └─ObjectPropertyExpression
//@[002:00004) |     ├─StringLiteralExpression { Value = id }
//@[006:00018) |     └─PropertyAccessExpression { PropertyName = id }
//@[006:00015) |       └─ResourceReferenceExpression
}
var setResourceCRef = true
//@[000:00026) ├─DeclaredVariableExpression { Name = setResourceCRef }
//@[022:00026) | └─BooleanLiteralExpression { Value = True }

resource resourceD 'My.Rp/typeD@2020-01-01' = {
//@[000:00231) ├─DeclaredResourceExpression
//@[046:00231) | ├─ObjectExpression
  name: 'constant'
  properties: {
//@[002:00159) | | └─ObjectPropertyExpression
//@[002:00012) | |   ├─StringLiteralExpression { Value = properties }
//@[014:00159) | |   └─ObjectExpression
    runtime: varBRuntime
//@[004:00024) | |     ├─ObjectPropertyExpression
//@[004:00011) | |     | ├─StringLiteralExpression { Value = runtime }
    // repro for https://github.com/Azure/bicep/issues/316
    repro316: setResourceCRef ? resourceCRef : null
//@[004:00051) | |     └─ObjectPropertyExpression
//@[004:00012) | |       ├─StringLiteralExpression { Value = repro316 }
//@[014:00051) | |       └─TernaryExpression
//@[014:00029) | |         ├─VariableReferenceExpression { Variable = setResourceCRef }
//@[032:00044) | |         ├─VariableReferenceExpression { Variable = resourceCRef }
//@[047:00051) | |         └─NullLiteralExpression
  }
}

var myInterpKey = 'abc'
//@[000:00023) ├─DeclaredVariableExpression { Name = myInterpKey }
//@[018:00023) | └─StringLiteralExpression { Value = abc }
resource resourceWithInterp 'My.Rp/interp@2020-01-01' = {
//@[000:00202) ├─DeclaredResourceExpression
//@[056:00202) | └─ObjectExpression
  name: 'interpTest'
  properties: {
//@[002:00118) |   └─ObjectPropertyExpression
//@[002:00012) |     ├─StringLiteralExpression { Value = properties }
//@[014:00118) |     └─ObjectExpression
    '${myInterpKey}': 1
//@[004:00023) |       ├─ObjectPropertyExpression
//@[004:00020) |       | ├─InterpolatedStringExpression
//@[007:00018) |       | | └─VariableReferenceExpression { Variable = myInterpKey }
//@[022:00023) |       | └─IntegerLiteralExpression { Value = 1 }
    'abc${myInterpKey}def': 2
//@[004:00029) |       ├─ObjectPropertyExpression
//@[004:00026) |       | ├─InterpolatedStringExpression
//@[010:00021) |       | | └─VariableReferenceExpression { Variable = myInterpKey }
//@[028:00029) |       | └─IntegerLiteralExpression { Value = 2 }
    '${myInterpKey}abc${myInterpKey}': 3
//@[004:00040) |       └─ObjectPropertyExpression
//@[004:00037) |         ├─InterpolatedStringExpression
//@[007:00018) |         | ├─VariableReferenceExpression { Variable = myInterpKey }
//@[024:00035) |         | └─VariableReferenceExpression { Variable = myInterpKey }
//@[039:00040) |         └─IntegerLiteralExpression { Value = 3 }
  }
}

resource resourceWithEscaping 'My.Rp/mockResource@2020-01-01' = {
//@[000:00234) ├─DeclaredResourceExpression
//@[064:00234) | └─ObjectExpression
  name: 'test'
  properties: {
//@[002:00148) |   └─ObjectPropertyExpression
//@[002:00012) |     ├─StringLiteralExpression { Value = properties }
//@[014:00148) |     └─ObjectExpression
    // both key and value should be escaped in template output
    '[resourceGroup().location]': '[resourceGroup().location]'
//@[004:00062) |       └─ObjectPropertyExpression
//@[004:00032) |         ├─StringLiteralExpression { Value = [resourceGroup().location] }
//@[034:00062) |         └─StringLiteralExpression { Value = [resourceGroup().location] }
  }
}

param shouldDeployVm bool = true
//@[000:00032) ├─DeclaredParameterExpression { Name = shouldDeployVm }
//@[021:00025) | ├─AmbientTypeReferenceExpression { Name = bool }
//@[028:00032) | └─BooleanLiteralExpression { Value = True }

@sys.description('this is vmWithCondition')
//@[000:00308) ├─DeclaredResourceExpression
//@[017:00042) | ├─StringLiteralExpression { Value = this is vmWithCondition }
resource vmWithCondition 'Microsoft.Compute/virtualMachines@2020-06-01' = if (shouldDeployVm) {
//@[078:00092) | └─ConditionExpression
//@[078:00092) |   ├─ParametersReferenceExpression { Parameter = shouldDeployVm }
//@[094:00263) |   └─ObjectExpression
  name: 'vmName'
  location: 'westus'
//@[002:00020) |     ├─ObjectPropertyExpression
//@[002:00010) |     | ├─StringLiteralExpression { Value = location }
//@[012:00020) |     | └─StringLiteralExpression { Value = westus }
  properties: {
//@[002:00123) |     └─ObjectPropertyExpression
//@[002:00012) |       ├─StringLiteralExpression { Value = properties }
//@[014:00123) |       └─ObjectExpression
    osProfile: {
//@[004:00101) |         └─ObjectPropertyExpression
//@[004:00013) |           ├─StringLiteralExpression { Value = osProfile }
//@[015:00101) |           └─ObjectExpression
      windowsConfiguration: {
//@[006:00076) |             └─ObjectPropertyExpression
//@[006:00026) |               ├─StringLiteralExpression { Value = windowsConfiguration }
//@[028:00076) |               └─ObjectExpression
        enableAutomaticUpdates: true
//@[008:00036) |                 └─ObjectPropertyExpression
//@[008:00030) |                   ├─StringLiteralExpression { Value = enableAutomaticUpdates }
//@[032:00036) |                   └─BooleanLiteralExpression { Value = True }
      }
    }
  }
}

@sys.description('this is another vmWithCondition')
//@[000:00339) ├─DeclaredResourceExpression
//@[017:00050) | ├─StringLiteralExpression { Value = this is another vmWithCondition }
resource vmWithCondition2 'Microsoft.Compute/virtualMachines@2020-06-01' =
                    if (shouldDeployVm) {
//@[024:00038) | └─ConditionExpression
//@[024:00038) |   ├─ParametersReferenceExpression { Parameter = shouldDeployVm }
//@[040:00210) |   └─ObjectExpression
  name: 'vmName2'
  location: 'westus'
//@[002:00020) |     ├─ObjectPropertyExpression
//@[002:00010) |     | ├─StringLiteralExpression { Value = location }
//@[012:00020) |     | └─StringLiteralExpression { Value = westus }
  properties: {
//@[002:00123) |     └─ObjectPropertyExpression
//@[002:00012) |       ├─StringLiteralExpression { Value = properties }
//@[014:00123) |       └─ObjectExpression
    osProfile: {
//@[004:00101) |         └─ObjectPropertyExpression
//@[004:00013) |           ├─StringLiteralExpression { Value = osProfile }
//@[015:00101) |           └─ObjectExpression
      windowsConfiguration: {
//@[006:00076) |             └─ObjectPropertyExpression
//@[006:00026) |               ├─StringLiteralExpression { Value = windowsConfiguration }
//@[028:00076) |               └─ObjectExpression
        enableAutomaticUpdates: true
//@[008:00036) |                 └─ObjectPropertyExpression
//@[008:00030) |                   ├─StringLiteralExpression { Value = enableAutomaticUpdates }
//@[032:00036) |                   └─BooleanLiteralExpression { Value = True }
      }
    }
  }
}

resource extension1 'My.Rp/extensionResource@2020-12-01' = {
//@[000:00110) ├─DeclaredResourceExpression
//@[059:00110) | ├─ObjectExpression
  name: 'extension'
  scope: vmWithCondition
}

resource extension2 'My.Rp/extensionResource@2020-12-01' = {
//@[000:00105) ├─DeclaredResourceExpression
//@[059:00105) | ├─ObjectExpression
  name: 'extension'
  scope: extension1
}

resource extensionDependencies 'My.Rp/mockResource@2020-01-01' = {
//@[000:00359) ├─DeclaredResourceExpression
//@[065:00359) | ├─ObjectExpression
  name: 'extensionDependencies'
  properties: {
//@[002:00255) | | └─ObjectPropertyExpression
//@[002:00012) | |   ├─StringLiteralExpression { Value = properties }
//@[014:00255) | |   └─ObjectExpression
    res1: vmWithCondition.id
//@[004:00028) | |     ├─ObjectPropertyExpression
//@[004:00008) | |     | ├─StringLiteralExpression { Value = res1 }
//@[010:00028) | |     | └─PropertyAccessExpression { PropertyName = id }
//@[010:00025) | |     |   └─ResourceReferenceExpression
    res1runtime: vmWithCondition.properties.something
//@[004:00053) | |     ├─ObjectPropertyExpression
//@[004:00015) | |     | ├─StringLiteralExpression { Value = res1runtime }
//@[017:00053) | |     | └─PropertyAccessExpression { PropertyName = something }
//@[017:00043) | |     |   └─PropertyAccessExpression { PropertyName = properties }
//@[017:00032) | |     |     └─ResourceReferenceExpression
    res2: extension1.id
//@[004:00023) | |     ├─ObjectPropertyExpression
//@[004:00008) | |     | ├─StringLiteralExpression { Value = res2 }
//@[010:00023) | |     | └─PropertyAccessExpression { PropertyName = id }
//@[010:00020) | |     |   └─ResourceReferenceExpression
    res2runtime: extension1.properties.something
//@[004:00048) | |     ├─ObjectPropertyExpression
//@[004:00015) | |     | ├─StringLiteralExpression { Value = res2runtime }
//@[017:00048) | |     | └─PropertyAccessExpression { PropertyName = something }
//@[017:00038) | |     |   └─PropertyAccessExpression { PropertyName = properties }
//@[017:00027) | |     |     └─ResourceReferenceExpression
    res3: extension2.id
//@[004:00023) | |     ├─ObjectPropertyExpression
//@[004:00008) | |     | ├─StringLiteralExpression { Value = res3 }
//@[010:00023) | |     | └─PropertyAccessExpression { PropertyName = id }
//@[010:00020) | |     |   └─ResourceReferenceExpression
    res3runtime: extension2.properties.something
//@[004:00048) | |     └─ObjectPropertyExpression
//@[004:00015) | |       ├─StringLiteralExpression { Value = res3runtime }
//@[017:00048) | |       └─PropertyAccessExpression { PropertyName = something }
//@[017:00038) | |         └─PropertyAccessExpression { PropertyName = properties }
//@[017:00027) | |           └─ResourceReferenceExpression
  }
}

@sys.description('this is existing1')
//@[000:00162) ├─DeclaredResourceExpression
//@[017:00036) | ├─StringLiteralExpression { Value = this is existing1 }
resource existing1 'Mock.Rp/existingExtensionResource@2020-01-01' existing = {
//@[077:00123) | ├─ObjectExpression
  name: 'existing1'
  scope: extension1
}

resource existing2 'Mock.Rp/existingExtensionResource@2020-01-01' existing = {
//@[000:00122) ├─DeclaredResourceExpression
//@[077:00122) | ├─ObjectExpression
  name: 'existing2'
  scope: existing1
}

resource extension3 'My.Rp/extensionResource@2020-12-01' = {
//@[000:00105) ├─DeclaredResourceExpression
//@[059:00105) | ├─ObjectExpression
  name: 'extension3'
  scope: existing1
}

/*
  valid loop cases
*/
var storageAccounts = [
//@[000:00129) ├─DeclaredVariableExpression { Name = storageAccounts }
//@[022:00129) | └─ArrayExpression
  {
//@[002:00050) |   ├─ObjectExpression
    name: 'one'
//@[004:00015) |   | ├─ObjectPropertyExpression
//@[004:00008) |   | | ├─StringLiteralExpression { Value = name }
//@[010:00015) |   | | └─StringLiteralExpression { Value = one }
    location: 'eastus2'
//@[004:00023) |   | └─ObjectPropertyExpression
//@[004:00012) |   |   ├─StringLiteralExpression { Value = location }
//@[014:00023) |   |   └─StringLiteralExpression { Value = eastus2 }
  }
  {
//@[002:00049) |   └─ObjectExpression
    name: 'two'
//@[004:00015) |     ├─ObjectPropertyExpression
//@[004:00008) |     | ├─StringLiteralExpression { Value = name }
//@[010:00015) |     | └─StringLiteralExpression { Value = two }
    location: 'westus'
//@[004:00022) |     └─ObjectPropertyExpression
//@[004:00012) |       ├─StringLiteralExpression { Value = location }
//@[014:00022) |       └─StringLiteralExpression { Value = westus }
  }
]

// just a storage account loop
@sys.description('this is just a storage account loop')
//@[000:00284) ├─DeclaredResourceExpression
//@[017:00054) | ├─StringLiteralExpression { Value = this is just a storage account loop }
resource storageResources 'Microsoft.Storage/storageAccounts@2019-06-01' = [for account in storageAccounts: {
//@[075:00227) | └─ForLoopExpression
//@[091:00106) |   ├─VariableReferenceExpression { Variable = storageAccounts }
//@[108:00226) |   └─ObjectExpression
//@[091:00106) |     |     └─VariableReferenceExpression { Variable = storageAccounts }
  name: account.name
  location: account.location
//@[002:00028) |     ├─ObjectPropertyExpression
//@[002:00010) |     | ├─StringLiteralExpression { Value = location }
//@[012:00028) |     | └─PropertyAccessExpression { PropertyName = location }
//@[012:00019) |     |   └─ArrayAccessExpression
//@[012:00019) |     |     ├─CopyIndexExpression
  sku: {
//@[002:00039) |     ├─ObjectPropertyExpression
//@[002:00005) |     | ├─StringLiteralExpression { Value = sku }
//@[007:00039) |     | └─ObjectExpression
    name: 'Standard_LRS'
//@[004:00024) |     |   └─ObjectPropertyExpression
//@[004:00008) |     |     ├─StringLiteralExpression { Value = name }
//@[010:00024) |     |     └─StringLiteralExpression { Value = Standard_LRS }
  }
  kind: 'StorageV2'
//@[002:00019) |     └─ObjectPropertyExpression
//@[002:00006) |       ├─StringLiteralExpression { Value = kind }
//@[008:00019) |       └─StringLiteralExpression { Value = StorageV2 }
}]

// storage account loop with index
@sys.description('this is just a storage account loop with index')
//@[000:00318) ├─DeclaredResourceExpression
//@[017:00065) | ├─StringLiteralExpression { Value = this is just a storage account loop with index }
resource storageResourcesWithIndex 'Microsoft.Storage/storageAccounts@2019-06-01' = [for (account, i) in storageAccounts: {
//@[084:00250) | └─ForLoopExpression
//@[105:00120) |   ├─VariableReferenceExpression { Variable = storageAccounts }
//@[122:00249) |   └─ObjectExpression
//@[105:00120) |     |     └─VariableReferenceExpression { Variable = storageAccounts }
  name: '${account.name}${i}'
  location: account.location
//@[002:00028) |     ├─ObjectPropertyExpression
//@[002:00010) |     | ├─StringLiteralExpression { Value = location }
//@[012:00028) |     | └─PropertyAccessExpression { PropertyName = location }
//@[012:00019) |     |   └─ArrayAccessExpression
//@[012:00019) |     |     ├─CopyIndexExpression
  sku: {
//@[002:00039) |     ├─ObjectPropertyExpression
//@[002:00005) |     | ├─StringLiteralExpression { Value = sku }
//@[007:00039) |     | └─ObjectExpression
    name: 'Standard_LRS'
//@[004:00024) |     |   └─ObjectPropertyExpression
//@[004:00008) |     |     ├─StringLiteralExpression { Value = name }
//@[010:00024) |     |     └─StringLiteralExpression { Value = Standard_LRS }
  }
  kind: 'StorageV2'
//@[002:00019) |     └─ObjectPropertyExpression
//@[002:00006) |       ├─StringLiteralExpression { Value = kind }
//@[008:00019) |       └─StringLiteralExpression { Value = StorageV2 }
}]

// basic nested loop
@sys.description('this is just a basic nested loop')
//@[000:00394) ├─DeclaredResourceExpression
//@[017:00051) | ├─StringLiteralExpression { Value = this is just a basic nested loop }
resource vnet 'Microsoft.Network/virtualNetworks@2020-06-01' = [for i in range(0, 3): {
//@[063:00340) | └─ForLoopExpression
//@[073:00084) |   ├─FunctionCallExpression { Name = range }
//@[079:00080) |   | ├─IntegerLiteralExpression { Value = 0 }
//@[082:00083) |   | └─IntegerLiteralExpression { Value = 3 }
//@[086:00339) |   └─ObjectExpression
//@[073:00084) |                   | └─FunctionCallExpression { Name = range }
//@[079:00080) |                   |   ├─IntegerLiteralExpression { Value = 0 }
//@[082:00083) |                   |   └─IntegerLiteralExpression { Value = 3 }
  name: 'vnet-${i}'
  properties: {
//@[002:00226) |     └─ObjectPropertyExpression
//@[002:00012) |       ├─StringLiteralExpression { Value = properties }
//@[014:00226) |       └─ObjectExpression
    subnets: [for j in range(0, 4): {
//@[004:00204) |         └─ObjectPropertyExpression
//@[004:00011) |           ├─StringLiteralExpression { Value = subnets }
//@[013:00204) |           └─ForLoopExpression
//@[023:00034) |             ├─FunctionCallExpression { Name = range }
//@[029:00030) |             | ├─IntegerLiteralExpression { Value = 0 }
//@[032:00033) |             | └─IntegerLiteralExpression { Value = 4 }
//@[036:00203) |             └─ObjectExpression
//@[023:00034) |                     └─FunctionCallExpression { Name = range }
//@[029:00030) |                       ├─IntegerLiteralExpression { Value = 0 }
//@[032:00033) |                       └─IntegerLiteralExpression { Value = 4 }
      // #completionTest(0,1,2,3,4,5) -> subnetIdAndProperties

      // #completionTest(6) -> subnetIdAndPropertiesNoColon
      name: 'subnet-${i}-${j}'
//@[006:00030) |               └─ObjectPropertyExpression
//@[006:00010) |                 ├─StringLiteralExpression { Value = name }
//@[012:00030) |                 └─InterpolatedStringExpression
//@[022:00023) |                   ├─ArrayAccessExpression
//@[022:00023) |                   | ├─CopyIndexExpression
//@[027:00028) |                   └─ArrayAccessExpression
//@[027:00028) |                     ├─CopyIndexExpression
    }]
  }
}]

// duplicate identifiers within the loop are allowed
resource duplicateIdentifiersWithinLoop 'Microsoft.Network/virtualNetworks@2020-06-01' = [for i in range(0, 3): {
//@[000:00239) ├─DeclaredResourceExpression
//@[089:00239) | └─ForLoopExpression
//@[099:00110) |   ├─FunctionCallExpression { Name = range }
//@[105:00106) |   | ├─IntegerLiteralExpression { Value = 0 }
//@[108:00109) |   | └─IntegerLiteralExpression { Value = 3 }
//@[112:00238) |   └─ObjectExpression
  name: 'vnet-${i}'
  properties: {
//@[002:00099) |     └─ObjectPropertyExpression
//@[002:00012) |       ├─StringLiteralExpression { Value = properties }
//@[014:00099) |       └─ObjectExpression
    subnets: [for i in range(0, 4): {
//@[004:00077) |         └─ObjectPropertyExpression
//@[004:00011) |           ├─StringLiteralExpression { Value = subnets }
//@[013:00077) |           └─ForLoopExpression
//@[023:00034) |             ├─FunctionCallExpression { Name = range }
//@[029:00030) |             | ├─IntegerLiteralExpression { Value = 0 }
//@[032:00033) |             | └─IntegerLiteralExpression { Value = 4 }
//@[036:00076) |             └─ObjectExpression
//@[023:00034) |                   | └─FunctionCallExpression { Name = range }
//@[029:00030) |                   |   ├─IntegerLiteralExpression { Value = 0 }
//@[032:00033) |                   |   └─IntegerLiteralExpression { Value = 4 }
//@[023:00034) |                     └─FunctionCallExpression { Name = range }
//@[029:00030) |                       ├─IntegerLiteralExpression { Value = 0 }
//@[032:00033) |                       └─IntegerLiteralExpression { Value = 4 }
      name: 'subnet-${i}-${i}'
//@[006:00030) |               └─ObjectPropertyExpression
//@[006:00010) |                 ├─StringLiteralExpression { Value = name }
//@[012:00030) |                 └─InterpolatedStringExpression
//@[022:00023) |                   ├─ArrayAccessExpression
//@[022:00023) |                   | ├─CopyIndexExpression
//@[027:00028) |                   └─ArrayAccessExpression
//@[027:00028) |                     ├─CopyIndexExpression
    }]
  }
}]

// duplicate identifiers in global and single loop scope are allowed (inner variable hides the outer)
var canHaveDuplicatesAcrossScopes = 'hello'
//@[000:00043) ├─DeclaredVariableExpression { Name = canHaveDuplicatesAcrossScopes }
//@[036:00043) | └─StringLiteralExpression { Value = hello }
resource duplicateInGlobalAndOneLoop 'Microsoft.Network/virtualNetworks@2020-06-01' = [for canHaveDuplicatesAcrossScopes in range(0, 3): {
//@[000:00292) ├─DeclaredResourceExpression
//@[086:00292) | └─ForLoopExpression
//@[124:00135) |   ├─FunctionCallExpression { Name = range }
//@[130:00131) |   | ├─IntegerLiteralExpression { Value = 0 }
//@[133:00134) |   | └─IntegerLiteralExpression { Value = 3 }
//@[137:00291) |   └─ObjectExpression
  name: 'vnet-${canHaveDuplicatesAcrossScopes}'
  properties: {
//@[002:00099) |     └─ObjectPropertyExpression
//@[002:00012) |       ├─StringLiteralExpression { Value = properties }
//@[014:00099) |       └─ObjectExpression
    subnets: [for i in range(0, 4): {
//@[004:00077) |         └─ObjectPropertyExpression
//@[004:00011) |           ├─StringLiteralExpression { Value = subnets }
//@[013:00077) |           └─ForLoopExpression
//@[023:00034) |             ├─FunctionCallExpression { Name = range }
//@[029:00030) |             | ├─IntegerLiteralExpression { Value = 0 }
//@[032:00033) |             | └─IntegerLiteralExpression { Value = 4 }
//@[036:00076) |             └─ObjectExpression
//@[023:00034) |                   | └─FunctionCallExpression { Name = range }
//@[029:00030) |                   |   ├─IntegerLiteralExpression { Value = 0 }
//@[032:00033) |                   |   └─IntegerLiteralExpression { Value = 4 }
//@[023:00034) |                     └─FunctionCallExpression { Name = range }
//@[029:00030) |                       ├─IntegerLiteralExpression { Value = 0 }
//@[032:00033) |                       └─IntegerLiteralExpression { Value = 4 }
      name: 'subnet-${i}-${i}'
//@[006:00030) |               └─ObjectPropertyExpression
//@[006:00010) |                 ├─StringLiteralExpression { Value = name }
//@[012:00030) |                 └─InterpolatedStringExpression
//@[022:00023) |                   ├─ArrayAccessExpression
//@[022:00023) |                   | ├─CopyIndexExpression
//@[027:00028) |                   └─ArrayAccessExpression
//@[027:00028) |                     ├─CopyIndexExpression
    }]
  }
}]

// duplicate in global and multiple loop scopes are allowed (inner hides the outer)
var duplicatesEverywhere = 'hello'
//@[000:00034) ├─DeclaredVariableExpression { Name = duplicatesEverywhere }
//@[027:00034) | └─StringLiteralExpression { Value = hello }
resource duplicateInGlobalAndTwoLoops 'Microsoft.Network/virtualNetworks@2020-06-01' = [for duplicatesEverywhere in range(0, 3): {
//@[000:00308) ├─DeclaredResourceExpression
//@[087:00308) | └─ForLoopExpression
//@[116:00127) |   ├─FunctionCallExpression { Name = range }
//@[122:00123) |   | ├─IntegerLiteralExpression { Value = 0 }
//@[125:00126) |   | └─IntegerLiteralExpression { Value = 3 }
//@[129:00307) |   └─ObjectExpression
  name: 'vnet-${duplicatesEverywhere}'
  properties: {
//@[002:00132) |     └─ObjectPropertyExpression
//@[002:00012) |       ├─StringLiteralExpression { Value = properties }
//@[014:00132) |       └─ObjectExpression
    subnets: [for duplicatesEverywhere in range(0, 4): {
//@[004:00110) |         └─ObjectPropertyExpression
//@[004:00011) |           ├─StringLiteralExpression { Value = subnets }
//@[013:00110) |           └─ForLoopExpression
//@[042:00053) |             ├─FunctionCallExpression { Name = range }
//@[048:00049) |             | ├─IntegerLiteralExpression { Value = 0 }
//@[051:00052) |             | └─IntegerLiteralExpression { Value = 4 }
//@[055:00109) |             └─ObjectExpression
//@[042:00053) |                     └─FunctionCallExpression { Name = range }
//@[048:00049) |                       ├─IntegerLiteralExpression { Value = 0 }
//@[051:00052) |                       └─IntegerLiteralExpression { Value = 4 }
      name: 'subnet-${duplicatesEverywhere}'
//@[006:00044) |               └─ObjectPropertyExpression
//@[006:00010) |                 ├─StringLiteralExpression { Value = name }
//@[012:00044) |                 └─InterpolatedStringExpression
//@[022:00042) |                   └─ArrayAccessExpression
//@[022:00042) |                     ├─CopyIndexExpression
    }]
  }
}]

/*
  Scope values created via array access on a resource collection
*/
resource dnsZones 'Microsoft.Network/dnsZones@2018-05-01' = [for zone in range(0,4): {
//@[000:00135) ├─DeclaredResourceExpression
//@[060:00135) | └─ForLoopExpression
//@[073:00083) |   ├─FunctionCallExpression { Name = range }
//@[079:00080) |   | ├─IntegerLiteralExpression { Value = 0 }
//@[081:00082) |   | └─IntegerLiteralExpression { Value = 4 }
//@[085:00134) |   └─ObjectExpression
  name: 'zone${zone}'
  location: 'global'
//@[002:00020) |     └─ObjectPropertyExpression
//@[002:00010) |       ├─StringLiteralExpression { Value = location }
//@[012:00020) |       └─StringLiteralExpression { Value = global }
}]

resource locksOnZones 'Microsoft.Authorization/locks@2016-09-01' = [for lock in range(0,2): {
//@[000:00194) ├─DeclaredResourceExpression
//@[067:00194) | ├─ForLoopExpression
//@[080:00090) | | ├─FunctionCallExpression { Name = range }
//@[086:00087) | | | ├─IntegerLiteralExpression { Value = 0 }
//@[088:00089) | | | └─IntegerLiteralExpression { Value = 2 }
//@[092:00193) | | └─ObjectExpression
  name: 'lock${lock}'
  properties: {
//@[002:00047) | |   └─ObjectPropertyExpression
//@[002:00012) | |     ├─StringLiteralExpression { Value = properties }
//@[014:00047) | |     └─ObjectExpression
    level: 'CanNotDelete'
//@[004:00025) | |       └─ObjectPropertyExpression
//@[004:00009) | |         ├─StringLiteralExpression { Value = level }
//@[011:00025) | |         └─StringLiteralExpression { Value = CanNotDelete }
  }
  scope: dnsZones[lock]
}]

resource moreLocksOnZones 'Microsoft.Authorization/locks@2016-09-01' = [for (lock, i) in range(0,3): {
//@[000:00196) ├─DeclaredResourceExpression
//@[071:00196) | ├─ForLoopExpression
//@[089:00099) | | ├─FunctionCallExpression { Name = range }
//@[095:00096) | | | ├─IntegerLiteralExpression { Value = 0 }
//@[097:00098) | | | └─IntegerLiteralExpression { Value = 3 }
//@[101:00195) | | └─ObjectExpression
  name: 'another${i}'
  properties: {
//@[002:00043) | |   └─ObjectPropertyExpression
//@[002:00012) | |     ├─StringLiteralExpression { Value = properties }
//@[014:00043) | |     └─ObjectExpression
    level: 'ReadOnly'
//@[004:00021) | |       └─ObjectPropertyExpression
//@[004:00009) | |         ├─StringLiteralExpression { Value = level }
//@[011:00021) | |         └─StringLiteralExpression { Value = ReadOnly }
  }
  scope: dnsZones[i]
}]

resource singleLockOnFirstZone 'Microsoft.Authorization/locks@2016-09-01' = {
//@[000:00170) ├─DeclaredResourceExpression
//@[076:00170) | ├─ObjectExpression
  name: 'single-lock'
  properties: {
//@[002:00043) | | └─ObjectPropertyExpression
//@[002:00012) | |   ├─StringLiteralExpression { Value = properties }
//@[014:00043) | |   └─ObjectExpression
    level: 'ReadOnly'
//@[004:00021) | |     └─ObjectPropertyExpression
//@[004:00009) | |       ├─StringLiteralExpression { Value = level }
//@[011:00021) | |       └─StringLiteralExpression { Value = ReadOnly }
  }
  scope: dnsZones[0]
}


resource p1_vnet 'Microsoft.Network/virtualNetworks@2020-06-01' = {
//@[000:00234) ├─DeclaredResourceExpression
//@[066:00234) | └─ObjectExpression
  location: resourceGroup().location
//@[002:00036) |   ├─ObjectPropertyExpression
//@[002:00010) |   | ├─StringLiteralExpression { Value = location }
//@[012:00036) |   | └─PropertyAccessExpression { PropertyName = location }
//@[012:00027) |   |   └─FunctionCallExpression { Name = resourceGroup }
  name: 'myVnet'
  properties: {
//@[002:00106) |   └─ObjectPropertyExpression
//@[002:00012) |     ├─StringLiteralExpression { Value = properties }
//@[014:00106) |     └─ObjectExpression
    addressSpace: {
//@[004:00084) |       └─ObjectPropertyExpression
//@[004:00016) |         ├─StringLiteralExpression { Value = addressSpace }
//@[018:00084) |         └─ObjectExpression
      addressPrefixes: [
//@[006:00056) |           └─ObjectPropertyExpression
//@[006:00021) |             ├─StringLiteralExpression { Value = addressPrefixes }
//@[023:00056) |             └─ArrayExpression
        '10.0.0.0/20'
//@[008:00021) |               └─StringLiteralExpression { Value = 10.0.0.0/20 }
      ]
    }
  }
}

resource p1_subnet1 'Microsoft.Network/virtualNetworks/subnets@2020-06-01' = {
//@[000:00175) ├─DeclaredResourceExpression
//@[077:00175) | ├─ObjectExpression
  parent: p1_vnet
  name: 'subnet1'
  properties: {
//@[002:00054) | | └─ObjectPropertyExpression
//@[002:00012) | |   ├─StringLiteralExpression { Value = properties }
//@[014:00054) | |   └─ObjectExpression
    addressPrefix: '10.0.0.0/24'
//@[004:00032) | |     └─ObjectPropertyExpression
//@[004:00017) | |       ├─StringLiteralExpression { Value = addressPrefix }
//@[019:00032) | |       └─StringLiteralExpression { Value = 10.0.0.0/24 }
  }
}

resource p1_subnet2 'Microsoft.Network/virtualNetworks/subnets@2020-06-01' = {
//@[000:00175) ├─DeclaredResourceExpression
//@[077:00175) | ├─ObjectExpression
  parent: p1_vnet
  name: 'subnet2'
  properties: {
//@[002:00054) | | └─ObjectPropertyExpression
//@[002:00012) | |   ├─StringLiteralExpression { Value = properties }
//@[014:00054) | |   └─ObjectExpression
    addressPrefix: '10.0.1.0/24'
//@[004:00032) | |     └─ObjectPropertyExpression
//@[004:00017) | |       ├─StringLiteralExpression { Value = addressPrefix }
//@[019:00032) | |       └─StringLiteralExpression { Value = 10.0.1.0/24 }
  }
}

output p1_subnet1prefix string = p1_subnet1.properties.addressPrefix
//@[000:00068) ├─DeclaredOutputExpression { Name = p1_subnet1prefix }
//@[024:00030) | ├─AmbientTypeReferenceExpression { Name = string }
//@[033:00068) | └─PropertyAccessExpression { PropertyName = addressPrefix }
//@[033:00054) |   └─PropertyAccessExpression { PropertyName = properties }
//@[033:00043) |     └─ResourceReferenceExpression
output p1_subnet1name string = p1_subnet1.name
//@[000:00046) ├─DeclaredOutputExpression { Name = p1_subnet1name }
//@[022:00028) | ├─AmbientTypeReferenceExpression { Name = string }
//@[031:00046) | └─PropertyAccessExpression { PropertyName = name }
//@[031:00041) |   └─ResourceReferenceExpression
output p1_subnet1type string = p1_subnet1.type
//@[000:00046) ├─DeclaredOutputExpression { Name = p1_subnet1type }
//@[022:00028) | ├─AmbientTypeReferenceExpression { Name = string }
//@[031:00046) | └─PropertyAccessExpression { PropertyName = type }
//@[031:00041) |   └─ResourceReferenceExpression
output p1_subnet1id string = p1_subnet1.id
//@[000:00042) ├─DeclaredOutputExpression { Name = p1_subnet1id }
//@[020:00026) | ├─AmbientTypeReferenceExpression { Name = string }
//@[029:00042) | └─PropertyAccessExpression { PropertyName = id }
//@[029:00039) |   └─ResourceReferenceExpression

// parent property with extension resource
resource p2_res1 'Microsoft.Rp1/resource1@2020-06-01' = {
//@[000:00078) ├─DeclaredResourceExpression
//@[056:00078) | └─ObjectExpression
  name: 'p2res1'
}

resource p2_res1child 'Microsoft.Rp1/resource1/child1@2020-06-01' = {
//@[000:00109) ├─DeclaredResourceExpression
//@[068:00109) | ├─ObjectExpression
  parent: p2_res1
  name: 'child1'
}

resource p2_res2 'Microsoft.Rp2/resource2@2020-06-01' = {
//@[000:00099) ├─DeclaredResourceExpression
//@[056:00099) | ├─ObjectExpression
  scope: p2_res1child
  name: 'res2'
}

resource p2_res2child 'Microsoft.Rp2/resource2/child2@2020-06-01' = {
//@[000:00109) ├─DeclaredResourceExpression
//@[068:00109) | ├─ObjectExpression
  parent: p2_res2
  name: 'child2'
}

output p2_res2childprop string = p2_res2child.properties.someProp
//@[000:00065) ├─DeclaredOutputExpression { Name = p2_res2childprop }
//@[024:00030) | ├─AmbientTypeReferenceExpression { Name = string }
//@[033:00065) | └─PropertyAccessExpression { PropertyName = someProp }
//@[033:00056) |   └─PropertyAccessExpression { PropertyName = properties }
//@[033:00045) |     └─ResourceReferenceExpression
output p2_res2childname string = p2_res2child.name
//@[000:00050) ├─DeclaredOutputExpression { Name = p2_res2childname }
//@[024:00030) | ├─AmbientTypeReferenceExpression { Name = string }
//@[033:00050) | └─PropertyAccessExpression { PropertyName = name }
//@[033:00045) |   └─ResourceReferenceExpression
output p2_res2childtype string = p2_res2child.type
//@[000:00050) ├─DeclaredOutputExpression { Name = p2_res2childtype }
//@[024:00030) | ├─AmbientTypeReferenceExpression { Name = string }
//@[033:00050) | └─PropertyAccessExpression { PropertyName = type }
//@[033:00045) |   └─ResourceReferenceExpression
output p2_res2childid string = p2_res2child.id
//@[000:00046) ├─DeclaredOutputExpression { Name = p2_res2childid }
//@[022:00028) | ├─AmbientTypeReferenceExpression { Name = string }
//@[031:00046) | └─PropertyAccessExpression { PropertyName = id }
//@[031:00043) |   └─ResourceReferenceExpression

// parent property with 'existing' resource
resource p3_res1 'Microsoft.Rp1/resource1@2020-06-01' existing = {
//@[000:00087) ├─DeclaredResourceExpression
//@[065:00087) | └─ObjectExpression
  name: 'p3res1'
}

resource p3_child1 'Microsoft.Rp1/resource1/child1@2020-06-01' = {
//@[000:00106) ├─DeclaredResourceExpression
//@[065:00106) | └─ObjectExpression
  parent: p3_res1
  name: 'child1'
}

output p3_res1childprop string = p3_child1.properties.someProp
//@[000:00062) ├─DeclaredOutputExpression { Name = p3_res1childprop }
//@[024:00030) | ├─AmbientTypeReferenceExpression { Name = string }
//@[033:00062) | └─PropertyAccessExpression { PropertyName = someProp }
//@[033:00053) |   └─PropertyAccessExpression { PropertyName = properties }
//@[033:00042) |     └─ResourceReferenceExpression
output p3_res1childname string = p3_child1.name
//@[000:00047) ├─DeclaredOutputExpression { Name = p3_res1childname }
//@[024:00030) | ├─AmbientTypeReferenceExpression { Name = string }
//@[033:00047) | └─PropertyAccessExpression { PropertyName = name }
//@[033:00042) |   └─ResourceReferenceExpression
output p3_res1childtype string = p3_child1.type
//@[000:00047) ├─DeclaredOutputExpression { Name = p3_res1childtype }
//@[024:00030) | ├─AmbientTypeReferenceExpression { Name = string }
//@[033:00047) | └─PropertyAccessExpression { PropertyName = type }
//@[033:00042) |   └─ResourceReferenceExpression
output p3_res1childid string = p3_child1.id
//@[000:00043) ├─DeclaredOutputExpression { Name = p3_res1childid }
//@[022:00028) | ├─AmbientTypeReferenceExpression { Name = string }
//@[031:00043) | └─PropertyAccessExpression { PropertyName = id }
//@[031:00040) |   └─ResourceReferenceExpression

// parent & child with 'existing'
resource p4_res1 'Microsoft.Rp1/resource1@2020-06-01' existing = {
//@[000:00106) ├─DeclaredResourceExpression
//@[065:00106) | └─ObjectExpression
  scope: tenant()
  name: 'p4res1'
}

resource p4_child1 'Microsoft.Rp1/resource1/child1@2020-06-01' existing = {
//@[000:00115) ├─DeclaredResourceExpression
//@[074:00115) | └─ObjectExpression
  parent: p4_res1
  name: 'child1'
}

output p4_res1childprop string = p4_child1.properties.someProp
//@[000:00062) ├─DeclaredOutputExpression { Name = p4_res1childprop }
//@[024:00030) | ├─AmbientTypeReferenceExpression { Name = string }
//@[033:00062) | └─PropertyAccessExpression { PropertyName = someProp }
//@[033:00053) |   └─PropertyAccessExpression { PropertyName = properties }
//@[033:00042) |     └─ResourceReferenceExpression
output p4_res1childname string = p4_child1.name
//@[000:00047) ├─DeclaredOutputExpression { Name = p4_res1childname }
//@[024:00030) | ├─AmbientTypeReferenceExpression { Name = string }
//@[033:00047) | └─PropertyAccessExpression { PropertyName = name }
//@[033:00042) |   └─ResourceReferenceExpression
output p4_res1childtype string = p4_child1.type
//@[000:00047) ├─DeclaredOutputExpression { Name = p4_res1childtype }
//@[024:00030) | ├─AmbientTypeReferenceExpression { Name = string }
//@[033:00047) | └─PropertyAccessExpression { PropertyName = type }
//@[033:00042) |   └─ResourceReferenceExpression
output p4_res1childid string = p4_child1.id
//@[000:00043) ├─DeclaredOutputExpression { Name = p4_res1childid }
//@[022:00028) | ├─AmbientTypeReferenceExpression { Name = string }
//@[031:00043) | └─PropertyAccessExpression { PropertyName = id }
//@[031:00040) |   └─ResourceReferenceExpression

// parent & nested child with decorators https://github.com/Azure/bicep/issues/10970
var dbs = ['db1', 'db2','db3']
//@[000:00030) ├─DeclaredVariableExpression { Name = dbs }
//@[010:00030) | └─ArrayExpression
//@[011:00016) |   ├─StringLiteralExpression { Value = db1 }
//@[018:00023) |   ├─StringLiteralExpression { Value = db2 }
//@[024:00029) |   └─StringLiteralExpression { Value = db3 }
resource sqlServer 'Microsoft.Sql/servers@2021-11-01' = {
//@[000:00527) ├─DeclaredResourceExpression
//@[056:00527) | └─ObjectExpression
  name: 'sql-server-name'
  location: 'polandcentral'
//@[002:00027) |   └─ObjectPropertyExpression
//@[002:00010) |     ├─StringLiteralExpression { Value = location }
//@[012:00027) |     └─StringLiteralExpression { Value = polandcentral }

  @batchSize(1)
//@[002:00156) ├─DeclaredResourceExpression
  @description('Sql Databases')
//@[015:00030) | ├─StringLiteralExpression { Value = Sql Databases }
  resource sqlDatabases 'databases' = [for db in dbs: {
//@[038:00106) | ├─ForLoopExpression
//@[049:00052) | | ├─VariableReferenceExpression { Variable = dbs }
//@[054:00105) | | └─ObjectExpression
    name: db
    location: 'polandcentral'
//@[004:00029) | |   └─ObjectPropertyExpression
//@[004:00012) | |     ├─StringLiteralExpression { Value = location }
//@[014:00029) | |     └─StringLiteralExpression { Value = polandcentral }
  }]

  @description('Primary Sql Database')
//@[002:00247) ├─DeclaredResourceExpression
//@[015:00037) | ├─StringLiteralExpression { Value = Primary Sql Database }
  resource primaryDb 'databases' = {
//@[035:00207) | ├─ObjectExpression
    name: 'primary-db'
    location: 'polandcentral'
//@[004:00029) | | └─ObjectPropertyExpression
//@[004:00012) | |   ├─StringLiteralExpression { Value = location }
//@[014:00029) | |   └─StringLiteralExpression { Value = polandcentral }

    resource threatProtection 'advancedThreatProtectionSettings' existing = {
//@[004:00107) ├─DeclaredResourceExpression
//@[076:00107) | ├─ObjectExpression
      name: 'default'
    }
  }
}

//nameof
output nameof_sqlServer string = nameof(sqlServer)
//@[000:00050) ├─DeclaredOutputExpression { Name = nameof_sqlServer }
//@[024:00030) | ├─AmbientTypeReferenceExpression { Name = string }
//@[040:00049) | └─StringLiteralExpression { Value = sqlServer }
output nameof_location string = nameof(sqlServer.location)
//@[000:00058) ├─DeclaredOutputExpression { Name = nameof_location }
//@[023:00029) | ├─AmbientTypeReferenceExpression { Name = string }
//@[039:00057) | └─StringLiteralExpression { Value = location }
output nameof_minCapacity string = nameof(sqlServer::primaryDb.properties.minCapacity)
//@[000:00086) ├─DeclaredOutputExpression { Name = nameof_minCapacity }
//@[026:00032) | ├─AmbientTypeReferenceExpression { Name = string }
//@[042:00085) | └─StringLiteralExpression { Value = minCapacity }
output nameof_creationTime string = nameof(sqlServer::primaryDb::threatProtection.properties.creationTime)
//@[000:00106) ├─DeclaredOutputExpression { Name = nameof_creationTime }
//@[027:00033) | ├─AmbientTypeReferenceExpression { Name = string }
//@[043:00105) | └─StringLiteralExpression { Value = creationTime }
output nameof_id string = nameof(sqlServer::sqlDatabases[0].id)
//@[000:00063) └─DeclaredOutputExpression { Name = nameof_id }
//@[017:00023)   ├─AmbientTypeReferenceExpression { Name = string }
//@[033:00062)   └─StringLiteralExpression { Value = id }

var sqlConfig = {
//@[000:00055) ├─DeclaredVariableExpression { Name = sqlConfig }
//@[016:00055) | └─ObjectExpression
  westus: {}
//@[002:00012) |   ├─ObjectPropertyExpression
//@[002:00008) |   | ├─StringLiteralExpression { Value = westus }
//@[010:00012) |   | └─ObjectExpression
  'server-name': {}
//@[002:00019) |   └─ObjectPropertyExpression
//@[002:00015) |     ├─StringLiteralExpression { Value = server-name }
//@[017:00019) |     └─ObjectExpression
}

resource sqlServerWithNameof 'Microsoft.Sql/servers@2021-11-01' = {
//@[000:00173) ├─DeclaredResourceExpression
//@[066:00173) | └─ObjectExpression
  name: 'sql-server-nameof-${nameof(sqlConfig['server-name'])}'
  location: nameof(sqlConfig.westus)
//@[002:00036) |   └─ObjectPropertyExpression
//@[002:00010) |     ├─StringLiteralExpression { Value = location }
//@[019:00035) |     └─StringLiteralExpression { Value = westus }
}

