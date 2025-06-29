param name string
//@[000:12019) ProgramExpression
//@[000:00017) ├─DeclaredParameterExpression { Name = name }
//@[011:00017) | └─AmbientTypeReferenceExpression { Name = string }
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
//@[000:00000) | └─ResourceDependencyExpression [UNPARENTED]
//@[000:00000) |   └─ModuleReferenceExpression [UNPARENTED]
//@[000:00000) | ├─ResourceDependencyExpression [UNPARENTED]
//@[000:00000) | | └─ModuleReferenceExpression [UNPARENTED]
//@[000:00000) | └─ResourceDependencyExpression [UNPARENTED]
//@[000:00000) |   └─ModuleReferenceExpression [UNPARENTED]
//@[000:00000) | └─ResourceDependencyExpression [UNPARENTED]
//@[000:00000) |   └─ResourceReferenceExpression [UNPARENTED]
//@[000:00000) | ├─ResourceDependencyExpression [UNPARENTED]
//@[000:00000) | | └─ResourceReferenceExpression [UNPARENTED]
//@[000:00000) | └─ResourceDependencyExpression [UNPARENTED]
//@[000:00000) |   └─ResourceReferenceExpression [UNPARENTED]
//@[000:00000) | ├─ResourceDependencyExpression [UNPARENTED]
//@[000:00000) | | └─ModuleReferenceExpression [UNPARENTED]
//@[000:00000) | └─ResourceDependencyExpression [UNPARENTED]
//@[000:00000) |   └─ResourceReferenceExpression [UNPARENTED]
//@[000:00000) | ├─ResourceDependencyExpression [UNPARENTED]
//@[000:00000) | | └─ModuleReferenceExpression [UNPARENTED]
//@[000:00000) | └─ResourceDependencyExpression [UNPARENTED]
//@[000:00000) |   └─ResourceReferenceExpression [UNPARENTED]
//@[000:00000) | ├─ResourceDependencyExpression [UNPARENTED]
//@[000:00000) | | └─ModuleReferenceExpression [UNPARENTED]
//@[000:00000) | ├─ResourceDependencyExpression [UNPARENTED]
//@[000:00000) | | └─ResourceReferenceExpression [UNPARENTED]
//@[000:00000) | └─ResourceDependencyExpression [UNPARENTED]
//@[000:00000) |   └─ResourceReferenceExpression [UNPARENTED]
//@[000:00000) | ├─ResourceDependencyExpression [UNPARENTED]
//@[000:00000) | | └─ModuleReferenceExpression [UNPARENTED]
//@[000:00000) | ├─ResourceDependencyExpression [UNPARENTED]
//@[000:00000) | | └─ResourceReferenceExpression [UNPARENTED]
//@[000:00000) | └─ResourceDependencyExpression [UNPARENTED]
//@[000:00000) |   └─ResourceReferenceExpression [UNPARENTED]
param accounts array
//@[000:00020) ├─DeclaredParameterExpression { Name = accounts }
//@[015:00020) | └─AmbientTypeReferenceExpression { Name = array }
param index int
//@[000:00015) ├─DeclaredParameterExpression { Name = index }
//@[012:00015) | └─AmbientTypeReferenceExpression { Name = int }

// single resource
resource singleResource 'Microsoft.Storage/storageAccounts@2019-06-01' = {
//@[000:00209) ├─DeclaredResourceExpression
//@[073:00209) | └─ObjectExpression
  name: '${name}single-resource-name'
  location: resourceGroup().location
//@[002:00036) |   ├─ObjectPropertyExpression
//@[002:00010) |   | ├─StringLiteralExpression { Value = location }
//@[012:00036) |   | └─PropertyAccessExpression { PropertyName = location }
//@[012:00027) |   |   └─FunctionCallExpression { Name = resourceGroup }
  kind: 'StorageV2'
//@[002:00019) |   ├─ObjectPropertyExpression
//@[002:00006) |   | ├─StringLiteralExpression { Value = kind }
//@[008:00019) |   | └─StringLiteralExpression { Value = StorageV2 }
  sku: {
//@[002:00037) |   └─ObjectPropertyExpression
//@[002:00005) |     ├─StringLiteralExpression { Value = sku }
//@[007:00037) |     └─ObjectExpression
    name: 'Standard_LRS'
//@[004:00024) |       └─ObjectPropertyExpression
//@[004:00008) |         ├─StringLiteralExpression { Value = name }
//@[010:00024) |         └─StringLiteralExpression { Value = Standard_LRS }
  }
}

// extension of single resource
resource singleResourceExtension 'Microsoft.Authorization/locks@2016-09-01' = {
//@[000:00182) ├─DeclaredResourceExpression
//@[078:00182) | ├─ObjectExpression
  scope: singleResource
  name: 'single-resource-lock'
  properties: {
//@[002:00045) | | └─ObjectPropertyExpression
//@[002:00012) | |   ├─StringLiteralExpression { Value = properties }
//@[014:00045) | |   └─ObjectExpression
    level: 'CanNotDelete'
//@[004:00025) | |     └─ObjectPropertyExpression
//@[004:00009) | |       ├─StringLiteralExpression { Value = level }
//@[011:00025) | |       └─StringLiteralExpression { Value = CanNotDelete }
  }
}

// single resource cascade extension
resource singleResourceCascadeExtension 'Microsoft.Authorization/locks@2016-09-01' = {
//@[000:00211) ├─DeclaredResourceExpression
//@[085:00211) | ├─ObjectExpression
  scope: singleResourceExtension
  name: 'single-resource-cascade-extension'
  properties: {
//@[002:00045) | | └─ObjectPropertyExpression
//@[002:00012) | |   ├─StringLiteralExpression { Value = properties }
//@[014:00045) | |   └─ObjectExpression
    level: 'CanNotDelete'
//@[004:00025) | |     └─ObjectPropertyExpression
//@[004:00009) | |       ├─StringLiteralExpression { Value = level }
//@[011:00025) | |       └─StringLiteralExpression { Value = CanNotDelete }
  }
}

// resource collection
@batchSize(42)
//@[000:00289) ├─DeclaredResourceExpression
resource storageAccounts 'Microsoft.Storage/storageAccounts@2019-06-01' = [for account in accounts: {
//@[074:00274) | ├─ForLoopExpression
//@[090:00098) | | ├─ParametersReferenceExpression { Parameter = accounts }
//@[100:00273) | | └─ObjectExpression
//@[090:00098) | |   |     └─ParametersReferenceExpression { Parameter = accounts }
  name: '${name}-collection-${account.name}'
  location: account.location
//@[002:00028) | |   ├─ObjectPropertyExpression
//@[002:00010) | |   | ├─StringLiteralExpression { Value = location }
//@[012:00028) | |   | └─PropertyAccessExpression { PropertyName = location }
//@[012:00019) | |   |   └─ArrayAccessExpression
//@[012:00019) | |   |     ├─CopyIndexExpression
  kind: 'StorageV2'
//@[002:00019) | |   ├─ObjectPropertyExpression
//@[002:00006) | |   | ├─StringLiteralExpression { Value = kind }
//@[008:00019) | |   | └─StringLiteralExpression { Value = StorageV2 }
  sku: {
//@[002:00037) | |   └─ObjectPropertyExpression
//@[002:00005) | |     ├─StringLiteralExpression { Value = sku }
//@[007:00037) | |     └─ObjectExpression
    name: 'Standard_LRS'
//@[004:00024) | |       └─ObjectPropertyExpression
//@[004:00008) | |         ├─StringLiteralExpression { Value = name }
//@[010:00024) | |         └─StringLiteralExpression { Value = Standard_LRS }
  }
  dependsOn: [
    singleResource
  ]
}]

// extension of a single resource in a collection
resource extendSingleResourceInCollection 'Microsoft.Authorization/locks@2016-09-01' = {
//@[000:00212) ├─DeclaredResourceExpression
//@[087:00212) | ├─ObjectExpression
  name: 'one-resource-collection-item-lock'
  properties: {
//@[002:00041) | | └─ObjectPropertyExpression
//@[002:00012) | |   ├─StringLiteralExpression { Value = properties }
//@[014:00041) | |   └─ObjectExpression
    level: 'ReadOnly'
//@[004:00021) | |     └─ObjectPropertyExpression
//@[004:00009) | |       ├─StringLiteralExpression { Value = level }
//@[011:00021) | |       └─StringLiteralExpression { Value = ReadOnly }
  }
  scope: storageAccounts[index % 2]
}

// collection of extensions
resource extensionCollection 'Microsoft.Authorization/locks@2016-09-01' = [for i in range(0,1): {
//@[000:00212) ├─DeclaredResourceExpression
//@[074:00212) | ├─ForLoopExpression
//@[084:00094) | | ├─FunctionCallExpression { Name = range }
//@[090:00091) | | | ├─IntegerLiteralExpression { Value = 0 }
//@[092:00093) | | | └─IntegerLiteralExpression { Value = 1 }
//@[096:00211) | | └─ObjectExpression
//@[084:00094) | |           | | └─FunctionCallExpression { Name = range }
//@[090:00091) | |           | |   ├─IntegerLiteralExpression { Value = 0 }
//@[092:00093) | |           | |   └─IntegerLiteralExpression { Value = 1 }
  name: 'lock-${i}'
  properties: {
//@[002:00067) | |   └─ObjectPropertyExpression
//@[002:00012) | |     ├─StringLiteralExpression { Value = properties }
//@[014:00067) | |     └─ObjectExpression
    level: i == 0 ? 'CanNotDelete' : 'ReadOnly'
//@[004:00047) | |       └─ObjectPropertyExpression
//@[004:00009) | |         ├─StringLiteralExpression { Value = level }
//@[011:00047) | |         └─TernaryExpression
//@[011:00017) | |           ├─BinaryExpression { Operator = Equals }
//@[011:00012) | |           | ├─ArrayAccessExpression
//@[011:00012) | |           | | ├─CopyIndexExpression
//@[016:00017) | |           | └─IntegerLiteralExpression { Value = 0 }
//@[020:00034) | |           ├─StringLiteralExpression { Value = CanNotDelete }
//@[037:00047) | |           └─StringLiteralExpression { Value = ReadOnly }
  }
  scope: singleResource
}]

// cascade extend the extension
@batchSize(1)
//@[000:00236) ├─DeclaredResourceExpression
resource lockTheLocks 'Microsoft.Authorization/locks@2016-09-01' = [for i in range(0,1): {
//@[067:00222) | ├─ForLoopExpression
//@[077:00087) | | ├─FunctionCallExpression { Name = range }
//@[083:00084) | | | ├─IntegerLiteralExpression { Value = 0 }
//@[085:00086) | | | └─IntegerLiteralExpression { Value = 1 }
//@[089:00221) | | └─ObjectExpression
//@[077:00087) | |           | | └─FunctionCallExpression { Name = range }
//@[083:00084) | |           | |   ├─IntegerLiteralExpression { Value = 0 }
//@[085:00086) | |           | |   └─IntegerLiteralExpression { Value = 1 }
  name: 'lock-the-lock-${i}'
  properties: {
//@[002:00067) | |   └─ObjectPropertyExpression
//@[002:00012) | |     ├─StringLiteralExpression { Value = properties }
//@[014:00067) | |     └─ObjectExpression
    level: i == 0 ? 'CanNotDelete' : 'ReadOnly'
//@[004:00047) | |       └─ObjectPropertyExpression
//@[004:00009) | |         ├─StringLiteralExpression { Value = level }
//@[011:00047) | |         └─TernaryExpression
//@[011:00017) | |           ├─BinaryExpression { Operator = Equals }
//@[011:00012) | |           | ├─ArrayAccessExpression
//@[011:00012) | |           | | ├─CopyIndexExpression
//@[016:00017) | |           | └─IntegerLiteralExpression { Value = 0 }
//@[020:00034) | |           ├─StringLiteralExpression { Value = CanNotDelete }
//@[037:00047) | |           └─StringLiteralExpression { Value = ReadOnly }
  }
  scope: extensionCollection[i]
}]

// special case property access
output indexedCollectionBlobEndpoint string = storageAccounts[index].properties.primaryEndpoints.blob
//@[000:00101) ├─DeclaredOutputExpression { Name = indexedCollectionBlobEndpoint }
//@[037:00043) | ├─AmbientTypeReferenceExpression { Name = string }
//@[046:00101) | └─PropertyAccessExpression { PropertyName = blob }
//@[046:00096) |   └─PropertyAccessExpression { PropertyName = primaryEndpoints }
//@[046:00079) |     └─PropertyAccessExpression { PropertyName = properties }
//@[046:00068) |       └─ResourceReferenceExpression
output indexedCollectionName string = storageAccounts[index].name
//@[000:00065) ├─DeclaredOutputExpression { Name = indexedCollectionName }
//@[029:00035) | ├─AmbientTypeReferenceExpression { Name = string }
//@[038:00065) | └─PropertyAccessExpression { PropertyName = name }
//@[038:00060) |   └─ResourceReferenceExpression
output indexedCollectionId string = storageAccounts[index].id
//@[000:00061) ├─DeclaredOutputExpression { Name = indexedCollectionId }
//@[027:00033) | ├─AmbientTypeReferenceExpression { Name = string }
//@[036:00061) | └─PropertyAccessExpression { PropertyName = id }
//@[036:00058) |   └─ResourceReferenceExpression
output indexedCollectionType string = storageAccounts[index].type
//@[000:00065) ├─DeclaredOutputExpression { Name = indexedCollectionType }
//@[029:00035) | ├─AmbientTypeReferenceExpression { Name = string }
//@[038:00065) | └─PropertyAccessExpression { PropertyName = type }
//@[038:00060) |   └─ResourceReferenceExpression
output indexedCollectionVersion string = storageAccounts[index].apiVersion
//@[000:00074) ├─DeclaredOutputExpression { Name = indexedCollectionVersion }
//@[032:00038) | ├─AmbientTypeReferenceExpression { Name = string }
//@[041:00074) | └─PropertyAccessExpression { PropertyName = apiVersion }
//@[041:00063) |   └─ResourceReferenceExpression

// general case property access
output indexedCollectionIdentity object = storageAccounts[index].identity
//@[000:00073) ├─DeclaredOutputExpression { Name = indexedCollectionIdentity }
//@[033:00039) | ├─AmbientTypeReferenceExpression { Name = object }
//@[042:00073) | └─PropertyAccessExpression { PropertyName = identity }
//@[042:00064) |   └─ResourceReferenceExpression

// indexed access of two properties
output indexedEndpointPair object = {
//@[000:00181) ├─DeclaredOutputExpression { Name = indexedEndpointPair }
//@[027:00033) | ├─AmbientTypeReferenceExpression { Name = object }
//@[036:00181) | └─ObjectExpression
  primary: storageAccounts[index].properties.primaryEndpoints.blob
//@[002:00066) |   ├─ObjectPropertyExpression
//@[002:00009) |   | ├─StringLiteralExpression { Value = primary }
//@[011:00066) |   | └─PropertyAccessExpression { PropertyName = blob }
//@[011:00061) |   |   └─PropertyAccessExpression { PropertyName = primaryEndpoints }
//@[011:00044) |   |     └─PropertyAccessExpression { PropertyName = properties }
//@[011:00033) |   |       └─ResourceReferenceExpression
  secondary: storageAccounts[index + 1].properties.secondaryEndpoints.blob
//@[002:00074) |   └─ObjectPropertyExpression
//@[002:00011) |     ├─StringLiteralExpression { Value = secondary }
//@[013:00074) |     └─PropertyAccessExpression { PropertyName = blob }
//@[013:00069) |       └─PropertyAccessExpression { PropertyName = secondaryEndpoints }
//@[013:00050) |         └─PropertyAccessExpression { PropertyName = properties }
//@[013:00039) |           └─ResourceReferenceExpression
}

// nested indexer?
output indexViaReference string = storageAccounts[int(storageAccounts[index].properties.creationTime)].properties.accessTier
//@[000:00124) ├─DeclaredOutputExpression { Name = indexViaReference }
//@[025:00031) | ├─AmbientTypeReferenceExpression { Name = string }
//@[034:00124) | └─PropertyAccessExpression { PropertyName = accessTier }
//@[034:00113) |   └─PropertyAccessExpression { PropertyName = properties }
//@[034:00102) |     └─ResourceReferenceExpression

// dependency on a resource collection
resource storageAccounts2 'Microsoft.Storage/storageAccounts@2019-06-01' = [for account in accounts: {
//@[000:00276) ├─DeclaredResourceExpression
//@[075:00276) | ├─ForLoopExpression
//@[091:00099) | | ├─ParametersReferenceExpression { Parameter = accounts }
//@[101:00275) | | └─ObjectExpression
//@[091:00099) | |   |     └─ParametersReferenceExpression { Parameter = accounts }
  name: '${name}-collection-${account.name}'
  location: account.location
//@[002:00028) | |   ├─ObjectPropertyExpression
//@[002:00010) | |   | ├─StringLiteralExpression { Value = location }
//@[012:00028) | |   | └─PropertyAccessExpression { PropertyName = location }
//@[012:00019) | |   |   └─ArrayAccessExpression
//@[012:00019) | |   |     ├─CopyIndexExpression
  kind: 'StorageV2'
//@[002:00019) | |   ├─ObjectPropertyExpression
//@[002:00006) | |   | ├─StringLiteralExpression { Value = kind }
//@[008:00019) | |   | └─StringLiteralExpression { Value = StorageV2 }
  sku: {
//@[002:00037) | |   └─ObjectPropertyExpression
//@[002:00005) | |     ├─StringLiteralExpression { Value = sku }
//@[007:00037) | |     └─ObjectExpression
    name: 'Standard_LRS'
//@[004:00024) | |       └─ObjectPropertyExpression
//@[004:00008) | |         ├─StringLiteralExpression { Value = name }
//@[010:00024) | |         └─StringLiteralExpression { Value = Standard_LRS }
  }
  dependsOn: [
    storageAccounts
  ]
}]

// one-to-one paired dependencies
resource firstSet 'Microsoft.Storage/storageAccounts@2019-06-01' = [for i in range(0, length(accounts)): {
//@[000:00232) ├─DeclaredResourceExpression
//@[067:00232) | └─ForLoopExpression
//@[077:00103) |   ├─FunctionCallExpression { Name = range }
//@[083:00084) |   | ├─IntegerLiteralExpression { Value = 0 }
//@[086:00102) |   | └─FunctionCallExpression { Name = length }
//@[093:00101) |   |   └─ParametersReferenceExpression { Parameter = accounts }
//@[105:00231) |   └─ObjectExpression
  name: '${name}-set1-${i}'
  location: resourceGroup().location
//@[002:00036) |     ├─ObjectPropertyExpression
//@[002:00010) |     | ├─StringLiteralExpression { Value = location }
//@[012:00036) |     | └─PropertyAccessExpression { PropertyName = location }
//@[012:00027) |     |   └─FunctionCallExpression { Name = resourceGroup }
  kind: 'StorageV2'
//@[002:00019) |     ├─ObjectPropertyExpression
//@[002:00006) |     | ├─StringLiteralExpression { Value = kind }
//@[008:00019) |     | └─StringLiteralExpression { Value = StorageV2 }
  sku: {
//@[002:00037) |     └─ObjectPropertyExpression
//@[002:00005) |       ├─StringLiteralExpression { Value = sku }
//@[007:00037) |       └─ObjectExpression
    name: 'Standard_LRS'
//@[004:00024) |         └─ObjectPropertyExpression
//@[004:00008) |           ├─StringLiteralExpression { Value = name }
//@[010:00024) |           └─StringLiteralExpression { Value = Standard_LRS }
  }
}]

resource secondSet 'Microsoft.Storage/storageAccounts@2019-06-01' = [for i in range(0, length(accounts)): {
//@[000:00268) ├─DeclaredResourceExpression
//@[068:00268) | ├─ForLoopExpression
//@[078:00104) | | ├─FunctionCallExpression { Name = range }
//@[084:00085) | | | ├─IntegerLiteralExpression { Value = 0 }
//@[087:00103) | | | └─FunctionCallExpression { Name = length }
//@[094:00102) | | |   └─ParametersReferenceExpression { Parameter = accounts }
//@[106:00267) | | └─ObjectExpression
  name: '${name}-set2-${i}'
  location: resourceGroup().location
//@[002:00036) | |   ├─ObjectPropertyExpression
//@[002:00010) | |   | ├─StringLiteralExpression { Value = location }
//@[012:00036) | |   | └─PropertyAccessExpression { PropertyName = location }
//@[012:00027) | |   |   └─FunctionCallExpression { Name = resourceGroup }
  kind: 'StorageV2'
//@[002:00019) | |   ├─ObjectPropertyExpression
//@[002:00006) | |   | ├─StringLiteralExpression { Value = kind }
//@[008:00019) | |   | └─StringLiteralExpression { Value = StorageV2 }
  sku: {
//@[002:00037) | |   └─ObjectPropertyExpression
//@[002:00005) | |     ├─StringLiteralExpression { Value = sku }
//@[007:00037) | |     └─ObjectExpression
    name: 'Standard_LRS'
//@[004:00024) | |       └─ObjectPropertyExpression
//@[004:00008) | |         ├─StringLiteralExpression { Value = name }
//@[010:00024) | |         └─StringLiteralExpression { Value = Standard_LRS }
  }
  dependsOn: [
    firstSet[i]
  ]
}]

// depending on collection and one resource in the collection optimizes the latter part away
resource anotherSingleResource 'Microsoft.Storage/storageAccounts@2019-06-01' = {
//@[000:00266) ├─DeclaredResourceExpression
//@[080:00266) | ├─ObjectExpression
  name: '${name}single-resource-name'
  location: resourceGroup().location
//@[002:00036) | | ├─ObjectPropertyExpression
//@[002:00010) | | | ├─StringLiteralExpression { Value = location }
//@[012:00036) | | | └─PropertyAccessExpression { PropertyName = location }
//@[012:00027) | | |   └─FunctionCallExpression { Name = resourceGroup }
  kind: 'StorageV2'
//@[002:00019) | | ├─ObjectPropertyExpression
//@[002:00006) | | | ├─StringLiteralExpression { Value = kind }
//@[008:00019) | | | └─StringLiteralExpression { Value = StorageV2 }
  sku: {
//@[002:00037) | | └─ObjectPropertyExpression
//@[002:00005) | |   ├─StringLiteralExpression { Value = sku }
//@[007:00037) | |   └─ObjectExpression
    name: 'Standard_LRS'
//@[004:00024) | |     └─ObjectPropertyExpression
//@[004:00008) | |       ├─StringLiteralExpression { Value = name }
//@[010:00024) | |       └─StringLiteralExpression { Value = Standard_LRS }
  }
  dependsOn: [
    secondSet
    secondSet[0]
  ]
}

// vnets
var vnetConfigurations = [
//@[000:00138) ├─DeclaredVariableExpression { Name = vnetConfigurations }
//@[025:00138) | └─ArrayExpression
  {
//@[002:00062) |   ├─ObjectExpression
    name: 'one'
//@[004:00015) |   | ├─ObjectPropertyExpression
//@[004:00008) |   | | ├─StringLiteralExpression { Value = name }
//@[010:00015) |   | | └─StringLiteralExpression { Value = one }
    location: resourceGroup().location
//@[004:00038) |   | └─ObjectPropertyExpression
//@[004:00012) |   |   ├─StringLiteralExpression { Value = location }
//@[014:00038) |   |   └─PropertyAccessExpression { PropertyName = location }
//@[014:00029) |   |     └─FunctionCallExpression { Name = resourceGroup }
  }
  {
//@[002:00046) |   └─ObjectExpression
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

resource vnets 'Microsoft.Network/virtualNetworks@2020-06-01' = [for vnetConfig in vnetConfigurations: {
//@[000:00163) ├─DeclaredResourceExpression
//@[064:00163) | └─ForLoopExpression
//@[083:00101) |   ├─VariableReferenceExpression { Variable = vnetConfigurations }
//@[103:00162) |   └─ObjectExpression
//@[083:00101) |           └─VariableReferenceExpression { Variable = vnetConfigurations }
  name: vnetConfig.name
  location: vnetConfig.location
//@[002:00031) |     └─ObjectPropertyExpression
//@[002:00010) |       ├─StringLiteralExpression { Value = location }
//@[012:00031) |       └─PropertyAccessExpression { PropertyName = location }
//@[012:00022) |         └─ArrayAccessExpression
//@[012:00022) |           ├─CopyIndexExpression
}]

// implicit dependency on single resource from a resource collection
resource implicitDependencyOnSingleResourceByIndex 'Microsoft.Network/dnsZones@2018-05-01' = {
//@[000:00237) ├─DeclaredResourceExpression
//@[093:00237) | ├─ObjectExpression
  name: 'test'
  location: 'global'
//@[002:00020) | | ├─ObjectPropertyExpression
//@[002:00010) | | | ├─StringLiteralExpression { Value = location }
//@[012:00020) | | | └─StringLiteralExpression { Value = global }
  properties: {
//@[002:00104) | | └─ObjectPropertyExpression
//@[002:00012) | |   ├─StringLiteralExpression { Value = properties }
//@[014:00104) | |   └─ObjectExpression
    resolutionVirtualNetworks: [
//@[004:00084) | |     └─ObjectPropertyExpression
//@[004:00029) | |       ├─StringLiteralExpression { Value = resolutionVirtualNetworks }
//@[031:00084) | |       └─ArrayExpression
      {
//@[006:00045) | |         └─ObjectExpression
        id: vnets[index+1].id
//@[008:00029) | |           └─ObjectPropertyExpression
//@[008:00010) | |             ├─StringLiteralExpression { Value = id }
//@[012:00029) | |             └─PropertyAccessExpression { PropertyName = id }
//@[012:00026) | |               └─ResourceReferenceExpression
      }
    ]
  }
}

// implicit and explicit dependency combined
resource combinedDependencies 'Microsoft.Network/dnsZones@2018-05-01' = {
//@[000:00294) ├─DeclaredResourceExpression
//@[072:00294) | ├─ObjectExpression
  name: 'test2'
  location: 'global'
//@[002:00020) | | ├─ObjectPropertyExpression
//@[002:00010) | | | ├─StringLiteralExpression { Value = location }
//@[012:00020) | | | └─StringLiteralExpression { Value = global }
  properties: {
//@[002:00152) | | └─ObjectPropertyExpression
//@[002:00012) | |   ├─StringLiteralExpression { Value = properties }
//@[014:00152) | |   └─ObjectExpression
    resolutionVirtualNetworks: [
//@[004:00132) | |     └─ObjectPropertyExpression
//@[004:00029) | |       ├─StringLiteralExpression { Value = resolutionVirtualNetworks }
//@[031:00132) | |       └─ArrayExpression
      {
//@[006:00045) | |         ├─ObjectExpression
        id: vnets[index-1].id
//@[008:00029) | |         | └─ObjectPropertyExpression
//@[008:00010) | |         |   ├─StringLiteralExpression { Value = id }
//@[012:00029) | |         |   └─PropertyAccessExpression { PropertyName = id }
//@[012:00026) | |         |     └─ResourceReferenceExpression
      }
      {
//@[006:00047) | |         └─ObjectExpression
        id: vnets[index * 2].id
//@[008:00031) | |           └─ObjectPropertyExpression
//@[008:00010) | |             ├─StringLiteralExpression { Value = id }
//@[012:00031) | |             └─PropertyAccessExpression { PropertyName = id }
//@[012:00028) | |               └─ResourceReferenceExpression
      }
    ]
  }
  dependsOn: [
    vnets
  ]
}

// single module
module singleModule 'passthrough.bicep' = {
//@[000:00097) ├─DeclaredModuleExpression
//@[042:00097) | ├─ObjectExpression
  name: 'test'
//@[002:00014) | | └─ObjectPropertyExpression
//@[002:00006) | |   ├─StringLiteralExpression { Value = name }
//@[008:00014) | |   └─StringLiteralExpression { Value = test }
  params: {
//@[010:00036) | └─ObjectExpression
    myInput: 'hello'
//@[004:00020) |   └─ObjectPropertyExpression
//@[004:00011) |     ├─StringLiteralExpression { Value = myInput }
//@[013:00020) |     └─StringLiteralExpression { Value = hello }
  }
}

var moduleSetup = [
//@[000:00047) ├─DeclaredVariableExpression { Name = moduleSetup }
//@[018:00047) | └─ArrayExpression
  'one'
//@[002:00007) |   ├─StringLiteralExpression { Value = one }
  'two'
//@[002:00007) |   ├─StringLiteralExpression { Value = two }
  'three'
//@[002:00009) |   └─StringLiteralExpression { Value = three }
]

// module collection plus explicit dependency on single module
@sys.batchSize(3)
//@[000:00242) ├─DeclaredModuleExpression
module moduleCollectionWithSingleDependency 'passthrough.bicep' = [for moduleName in moduleSetup: {
//@[066:00224) | ├─ForLoopExpression
//@[085:00096) | | ├─VariableReferenceExpression { Variable = moduleSetup }
//@[098:00223) | | └─ObjectExpression
//@[085:00096) | |       └─VariableReferenceExpression { Variable = moduleSetup }
//@[085:00096) | |       └─VariableReferenceExpression { Variable = moduleSetup }
  name: moduleName
//@[002:00018) | |   └─ObjectPropertyExpression
//@[002:00006) | |     ├─StringLiteralExpression { Value = name }
//@[008:00018) | |     └─ArrayAccessExpression
//@[008:00018) | |       ├─CopyIndexExpression
  params: {
//@[010:00047) | ├─ObjectExpression
    myInput: 'in-${moduleName}'
//@[004:00031) | | └─ObjectPropertyExpression
//@[004:00011) | |   ├─StringLiteralExpression { Value = myInput }
//@[013:00031) | |   └─InterpolatedStringExpression
//@[019:00029) | |     └─ArrayAccessExpression
//@[019:00029) | |       ├─CopyIndexExpression
  }
  dependsOn: [
    singleModule
    singleResource
  ]
}]

// another module collection with dependency on another module collection
module moduleCollectionWithCollectionDependencies 'passthrough.bicep' = [for moduleName in moduleSetup: {
//@[000:00255) ├─DeclaredModuleExpression
//@[072:00255) | ├─ForLoopExpression
//@[091:00102) | | ├─VariableReferenceExpression { Variable = moduleSetup }
//@[104:00254) | | └─ObjectExpression
//@[091:00102) | |       └─VariableReferenceExpression { Variable = moduleSetup }
//@[091:00102) | |       └─VariableReferenceExpression { Variable = moduleSetup }
  name: moduleName
//@[002:00018) | |   └─ObjectPropertyExpression
//@[002:00006) | |     ├─StringLiteralExpression { Value = name }
//@[008:00018) | |     └─ArrayAccessExpression
//@[008:00018) | |       ├─CopyIndexExpression
  params: {
//@[010:00047) | ├─ObjectExpression
    myInput: 'in-${moduleName}'
//@[004:00031) | | └─ObjectPropertyExpression
//@[004:00011) | |   ├─StringLiteralExpression { Value = myInput }
//@[013:00031) | |   └─InterpolatedStringExpression
//@[019:00029) | |     └─ArrayAccessExpression
//@[019:00029) | |       ├─CopyIndexExpression
  }
  dependsOn: [
    storageAccounts
    moduleCollectionWithSingleDependency
  ]
}]

module singleModuleWithIndexedDependencies 'passthrough.bicep' = {
//@[000:00290) ├─DeclaredModuleExpression
//@[065:00290) | ├─ObjectExpression
  name: 'hello'
//@[002:00015) | | └─ObjectPropertyExpression
//@[002:00006) | |   ├─StringLiteralExpression { Value = name }
//@[008:00015) | |   └─StringLiteralExpression { Value = hello }
  params: {
//@[010:00153) | ├─ObjectExpression
    myInput: concat(moduleCollectionWithCollectionDependencies[index].outputs.myOutput, storageAccounts[index * 3].properties.accessTier)
//@[004:00137) | | └─ObjectPropertyExpression
//@[004:00011) | |   ├─StringLiteralExpression { Value = myInput }
//@[013:00137) | |   └─FunctionCallExpression { Name = concat }
//@[020:00086) | |     ├─ModuleOutputPropertyAccessExpression { PropertyName = myOutput }
//@[020:00077) | |     | └─PropertyAccessExpression { PropertyName = outputs }
//@[020:00069) | |     |   └─ModuleReferenceExpression
//@[088:00136) | |     └─PropertyAccessExpression { PropertyName = accessTier }
//@[088:00125) | |       └─PropertyAccessExpression { PropertyName = properties }
//@[088:00114) | |         └─ResourceReferenceExpression
  }
  dependsOn: [
    storageAccounts2[index - 10]
  ]
}

module moduleCollectionWithIndexedDependencies 'passthrough.bicep' = [for moduleName in moduleSetup: {
//@[000:00346) ├─DeclaredModuleExpression
//@[069:00346) | ├─ForLoopExpression
//@[088:00099) | | ├─VariableReferenceExpression { Variable = moduleSetup }
//@[101:00345) | | └─ObjectExpression
//@[088:00099) | |       └─VariableReferenceExpression { Variable = moduleSetup }
//@[088:00099) | |       └─VariableReferenceExpression { Variable = moduleSetup }
  name: moduleName
//@[002:00018) | |   └─ObjectPropertyExpression
//@[002:00006) | |     ├─StringLiteralExpression { Value = name }
//@[008:00018) | |     └─ArrayAccessExpression
//@[008:00018) | |       ├─CopyIndexExpression
  params: {
//@[010:00170) | ├─ObjectExpression
    myInput: '${moduleCollectionWithCollectionDependencies[index].outputs.myOutput} - ${storageAccounts[index * 3].properties.accessTier} - ${moduleName}'
//@[004:00154) | | └─ObjectPropertyExpression
//@[004:00011) | |   ├─StringLiteralExpression { Value = myInput }
//@[013:00154) | |   └─InterpolatedStringExpression
//@[016:00082) | |     ├─ModuleOutputPropertyAccessExpression { PropertyName = myOutput }
//@[016:00073) | |     | └─PropertyAccessExpression { PropertyName = outputs }
//@[016:00065) | |     |   └─ModuleReferenceExpression
//@[088:00136) | |     ├─PropertyAccessExpression { PropertyName = accessTier }
//@[088:00125) | |     | └─PropertyAccessExpression { PropertyName = properties }
//@[088:00114) | |     |   └─ResourceReferenceExpression
//@[142:00152) | |     └─ArrayAccessExpression
//@[142:00152) | |       ├─CopyIndexExpression
  }
  dependsOn: [
    storageAccounts2[index - 9]
  ]
}]

output indexedModulesName string = moduleCollectionWithSingleDependency[index].name
//@[000:00083) ├─DeclaredOutputExpression { Name = indexedModulesName }
//@[026:00032) | ├─AmbientTypeReferenceExpression { Name = string }
//@[035:00083) | └─PropertyAccessExpression { PropertyName = name }
//@[035:00078) |   └─ModuleReferenceExpression
output indexedModuleOutput string = moduleCollectionWithSingleDependency[index * 1].outputs.myOutput
//@[000:00100) ├─DeclaredOutputExpression { Name = indexedModuleOutput }
//@[027:00033) | ├─AmbientTypeReferenceExpression { Name = string }
//@[036:00100) | └─ModuleOutputPropertyAccessExpression { PropertyName = myOutput }
//@[036:00091) |   └─PropertyAccessExpression { PropertyName = outputs }
//@[036:00083) |     └─ModuleReferenceExpression

// resource collection
resource existingStorageAccounts 'Microsoft.Storage/storageAccounts@2019-06-01' existing = [for account in accounts: {
//@[000:00164) ├─DeclaredResourceExpression
//@[091:00164) | └─ForLoopExpression
//@[107:00115) |   ├─ParametersReferenceExpression { Parameter = accounts }
//@[117:00163) |   └─ObjectExpression
  name: '${name}-existing-${account.name}'
}]

output existingIndexedResourceName string = existingStorageAccounts[index * 0].name
//@[000:00083) ├─DeclaredOutputExpression { Name = existingIndexedResourceName }
//@[035:00041) | ├─AmbientTypeReferenceExpression { Name = string }
//@[044:00083) | └─PropertyAccessExpression { PropertyName = name }
//@[044:00078) |   └─ResourceReferenceExpression
output existingIndexedResourceId string = existingStorageAccounts[index * 1].id
//@[000:00079) ├─DeclaredOutputExpression { Name = existingIndexedResourceId }
//@[033:00039) | ├─AmbientTypeReferenceExpression { Name = string }
//@[042:00079) | └─PropertyAccessExpression { PropertyName = id }
//@[042:00076) |   └─ResourceReferenceExpression
output existingIndexedResourceType string = existingStorageAccounts[index+2].type
//@[000:00081) ├─DeclaredOutputExpression { Name = existingIndexedResourceType }
//@[035:00041) | ├─AmbientTypeReferenceExpression { Name = string }
//@[044:00081) | └─PropertyAccessExpression { PropertyName = type }
//@[044:00076) |   └─ResourceReferenceExpression
output existingIndexedResourceApiVersion string = existingStorageAccounts[index-7].apiVersion
//@[000:00093) ├─DeclaredOutputExpression { Name = existingIndexedResourceApiVersion }
//@[041:00047) | ├─AmbientTypeReferenceExpression { Name = string }
//@[050:00093) | └─PropertyAccessExpression { PropertyName = apiVersion }
//@[050:00082) |   └─ResourceReferenceExpression
output existingIndexedResourceLocation string = existingStorageAccounts[index/2].location
//@[000:00089) ├─DeclaredOutputExpression { Name = existingIndexedResourceLocation }
//@[039:00045) | ├─AmbientTypeReferenceExpression { Name = string }
//@[048:00089) | └─PropertyAccessExpression { PropertyName = location }
//@[048:00080) |   └─ResourceReferenceExpression
output existingIndexedResourceAccessTier string = existingStorageAccounts[index%3].properties.accessTier
//@[000:00104) ├─DeclaredOutputExpression { Name = existingIndexedResourceAccessTier }
//@[041:00047) | ├─AmbientTypeReferenceExpression { Name = string }
//@[050:00104) | └─PropertyAccessExpression { PropertyName = accessTier }
//@[050:00093) |   └─PropertyAccessExpression { PropertyName = properties }
//@[050:00082) |     └─ResourceReferenceExpression

resource duplicatedNames 'Microsoft.Network/dnsZones@2018-05-01' = [for zone in []: {
//@[000:00136) ├─DeclaredResourceExpression
//@[067:00136) | └─ForLoopExpression
//@[080:00082) |   ├─ArrayExpression
//@[084:00135) |   └─ObjectExpression
  name: 'no loop variable'
  location: 'eastus'
//@[002:00020) |     └─ObjectPropertyExpression
//@[002:00010) |       ├─StringLiteralExpression { Value = location }
//@[012:00020) |       └─StringLiteralExpression { Value = eastus }
}]

// reference to a resource collection whose name expression does not reference any loop variables
resource referenceToDuplicateNames 'Microsoft.Network/dnsZones@2018-05-01' = [for zone in []: {
//@[000:00194) ├─DeclaredResourceExpression
//@[077:00194) | ├─ForLoopExpression
//@[090:00092) | | ├─ArrayExpression
//@[094:00193) | | └─ObjectExpression
  name: 'no loop variable 2'
  location: 'eastus'
//@[002:00020) | |   └─ObjectPropertyExpression
//@[002:00010) | |     ├─StringLiteralExpression { Value = location }
//@[012:00020) | |     └─StringLiteralExpression { Value = eastus }
  dependsOn: [
    duplicatedNames[index]
  ]
}]

var regions = [
//@[000:00039) ├─DeclaredVariableExpression { Name = regions }
//@[014:00039) | └─ArrayExpression
  'eastus'
//@[002:00010) |   ├─StringLiteralExpression { Value = eastus }
  'westus'
//@[002:00010) |   └─StringLiteralExpression { Value = westus }
]

module apim 'passthrough.bicep' = [for region in regions: {
//@[000:00131) ├─DeclaredModuleExpression
//@[034:00131) | ├─ForLoopExpression
//@[049:00056) | | ├─VariableReferenceExpression { Variable = regions }
//@[058:00130) | | └─ObjectExpression
//@[049:00056) | |       | └─VariableReferenceExpression { Variable = regions }
//@[049:00056) |       └─VariableReferenceExpression { Variable = regions }
  name: 'apim-${region}-${name}'
//@[002:00032) | |   └─ObjectPropertyExpression
//@[002:00006) | |     ├─StringLiteralExpression { Value = name }
//@[008:00032) | |     └─InterpolatedStringExpression
//@[016:00022) | |       ├─ArrayAccessExpression
//@[016:00022) | |       | ├─CopyIndexExpression
//@[026:00030) | |       └─ParametersReferenceExpression { Parameter = name }
  params: {
//@[010:00035) | └─ObjectExpression
    myInput: region
//@[004:00019) |   └─ObjectPropertyExpression
//@[004:00011) |     ├─StringLiteralExpression { Value = myInput }
//@[013:00019) |     └─ArrayAccessExpression
//@[013:00019) |       ├─CopyIndexExpression
  }
}]

resource propertyLoopDependencyOnModuleCollection 'Microsoft.Network/frontDoors@2020-05-01' = {
//@[000:00780) ├─DeclaredResourceExpression
//@[094:00780) | ├─ObjectExpression
  name: name
  location: 'Global'
//@[002:00020) | | ├─ObjectPropertyExpression
//@[002:00010) | | | ├─StringLiteralExpression { Value = location }
//@[012:00020) | | | └─StringLiteralExpression { Value = Global }
  properties: {
//@[002:00648) | | └─ObjectPropertyExpression
//@[002:00012) | |   ├─StringLiteralExpression { Value = properties }
//@[014:00648) | |   └─ObjectExpression
    backendPools: [
//@[004:00628) | |     └─ObjectPropertyExpression
//@[004:00016) | |       ├─StringLiteralExpression { Value = backendPools }
//@[018:00628) | |       └─ArrayExpression
      {
//@[006:00602) | |         └─ObjectExpression
        name: 'BackendAPIMs'
//@[008:00028) | |           ├─ObjectPropertyExpression
//@[008:00012) | |           | ├─StringLiteralExpression { Value = name }
//@[014:00028) | |           | └─StringLiteralExpression { Value = BackendAPIMs }
        properties: {
//@[008:00557) | |           └─ObjectPropertyExpression
//@[008:00018) | |             ├─StringLiteralExpression { Value = properties }
//@[020:00557) | |             └─ObjectExpression
          backends: [for index in range(0, length(regions)): {
//@[010:00525) | |               └─ObjectPropertyExpression
//@[010:00018) | |                 ├─StringLiteralExpression { Value = backends }
//@[020:00525) | |                 └─ForLoopExpression
//@[034:00059) | |                   ├─FunctionCallExpression { Name = range }
//@[040:00041) | |                   | ├─IntegerLiteralExpression { Value = 0 }
//@[043:00058) | |                   | └─FunctionCallExpression { Name = length }
//@[050:00057) | |                   |   └─VariableReferenceExpression { Variable = regions }
//@[061:00524) | |                   └─ObjectExpression
            // we cannot codegen index correctly because the generated dependsOn property
            // would be outside of the scope of the property loop
            // as a result, this will generate a dependency on the entire collection
            address: apim[index].outputs.myOutput
//@[012:00049) | |                     ├─ObjectPropertyExpression
//@[012:00019) | |                     | ├─StringLiteralExpression { Value = address }
//@[021:00049) | |                     | └─ModuleOutputPropertyAccessExpression { PropertyName = myOutput }
//@[021:00040) | |                     |   └─PropertyAccessExpression { PropertyName = outputs }
//@[021:00032) | |                     |     └─ModuleReferenceExpression
            backendHostHeader: apim[index].outputs.myOutput
//@[012:00059) | |                     ├─ObjectPropertyExpression
//@[012:00029) | |                     | ├─StringLiteralExpression { Value = backendHostHeader }
//@[031:00059) | |                     | └─ModuleOutputPropertyAccessExpression { PropertyName = myOutput }
//@[031:00050) | |                     |   └─PropertyAccessExpression { PropertyName = outputs }
//@[031:00042) | |                     |     └─ModuleReferenceExpression
            httpPort: 80
//@[012:00024) | |                     ├─ObjectPropertyExpression
//@[012:00020) | |                     | ├─StringLiteralExpression { Value = httpPort }
//@[022:00024) | |                     | └─IntegerLiteralExpression { Value = 80 }
            httpsPort: 443
//@[012:00026) | |                     ├─ObjectPropertyExpression
//@[012:00021) | |                     | ├─StringLiteralExpression { Value = httpsPort }
//@[023:00026) | |                     | └─IntegerLiteralExpression { Value = 443 }
            priority: 1
//@[012:00023) | |                     ├─ObjectPropertyExpression
//@[012:00020) | |                     | ├─StringLiteralExpression { Value = priority }
//@[022:00023) | |                     | └─IntegerLiteralExpression { Value = 1 }
            weight: 50
//@[012:00022) | |                     └─ObjectPropertyExpression
//@[012:00018) | |                       ├─StringLiteralExpression { Value = weight }
//@[020:00022) | |                       └─IntegerLiteralExpression { Value = 50 }
          }]
        }
      }
    ]
  }
}

resource indexedModuleCollectionDependency 'Microsoft.Network/frontDoors@2020-05-01' = [for index in range(0, length(regions)): {
//@[000:00757) ├─DeclaredResourceExpression
//@[087:00757) | ├─ForLoopExpression
//@[101:00126) | | ├─FunctionCallExpression { Name = range }
//@[107:00108) | | | ├─IntegerLiteralExpression { Value = 0 }
//@[110:00125) | | | └─FunctionCallExpression { Name = length }
//@[117:00124) | | |   └─VariableReferenceExpression { Variable = regions }
//@[128:00756) | | └─ObjectExpression
  name: '${name}-${index}'
  location: 'Global'
//@[002:00020) | |   ├─ObjectPropertyExpression
//@[002:00010) | |   | ├─StringLiteralExpression { Value = location }
//@[012:00020) | |   | └─StringLiteralExpression { Value = Global }
  properties: {
//@[002:00576) | |   └─ObjectPropertyExpression
//@[002:00012) | |     ├─StringLiteralExpression { Value = properties }
//@[014:00576) | |     └─ObjectExpression
    backendPools: [
//@[004:00556) | |       └─ObjectPropertyExpression
//@[004:00016) | |         ├─StringLiteralExpression { Value = backendPools }
//@[018:00556) | |         └─ArrayExpression
      {
//@[006:00530) | |           └─ObjectExpression
        name: 'BackendAPIMs'
//@[008:00028) | |             ├─ObjectPropertyExpression
//@[008:00012) | |             | ├─StringLiteralExpression { Value = name }
//@[014:00028) | |             | └─StringLiteralExpression { Value = BackendAPIMs }
        properties: {
//@[008:00485) | |             └─ObjectPropertyExpression
//@[008:00018) | |               ├─StringLiteralExpression { Value = properties }
//@[020:00485) | |               └─ObjectExpression
          backends: [
//@[010:00453) | |                 └─ObjectPropertyExpression
//@[010:00018) | |                   ├─StringLiteralExpression { Value = backends }
//@[020:00453) | |                   └─ArrayExpression
            {
//@[012:00419) | |                     └─ObjectExpression
              // this indexed dependency on a module collection will be generated correctly because
              // copyIndex() can be invoked in the generated dependsOn
              address: apim[index].outputs.myOutput
//@[014:00051) | |                       ├─ObjectPropertyExpression
//@[014:00021) | |                       | ├─StringLiteralExpression { Value = address }
//@[023:00051) | |                       | └─ModuleOutputPropertyAccessExpression { PropertyName = myOutput }
//@[023:00042) | |                       |   └─PropertyAccessExpression { PropertyName = outputs }
//@[023:00034) | |                       |     └─ModuleReferenceExpression
              backendHostHeader: apim[index].outputs.myOutput
//@[014:00061) | |                       ├─ObjectPropertyExpression
//@[014:00031) | |                       | ├─StringLiteralExpression { Value = backendHostHeader }
//@[033:00061) | |                       | └─ModuleOutputPropertyAccessExpression { PropertyName = myOutput }
//@[033:00052) | |                       |   └─PropertyAccessExpression { PropertyName = outputs }
//@[033:00044) | |                       |     └─ModuleReferenceExpression
              httpPort: 80
//@[014:00026) | |                       ├─ObjectPropertyExpression
//@[014:00022) | |                       | ├─StringLiteralExpression { Value = httpPort }
//@[024:00026) | |                       | └─IntegerLiteralExpression { Value = 80 }
              httpsPort: 443
//@[014:00028) | |                       ├─ObjectPropertyExpression
//@[014:00023) | |                       | ├─StringLiteralExpression { Value = httpsPort }
//@[025:00028) | |                       | └─IntegerLiteralExpression { Value = 443 }
              priority: 1
//@[014:00025) | |                       ├─ObjectPropertyExpression
//@[014:00022) | |                       | ├─StringLiteralExpression { Value = priority }
//@[024:00025) | |                       | └─IntegerLiteralExpression { Value = 1 }
              weight: 50
//@[014:00024) | |                       └─ObjectPropertyExpression
//@[014:00020) | |                         ├─StringLiteralExpression { Value = weight }
//@[022:00024) | |                         └─IntegerLiteralExpression { Value = 50 }
            }
          ]
        }
      }
    ]
  }
}]

resource propertyLoopDependencyOnResourceCollection 'Microsoft.Network/frontDoors@2020-05-01' = {
//@[000:00871) ├─DeclaredResourceExpression
//@[096:00871) | ├─ObjectExpression
  name: name
  location: 'Global'
//@[002:00020) | | ├─ObjectPropertyExpression
//@[002:00010) | | | ├─StringLiteralExpression { Value = location }
//@[012:00020) | | | └─StringLiteralExpression { Value = Global }
  properties: {
//@[002:00737) | | └─ObjectPropertyExpression
//@[002:00012) | |   ├─StringLiteralExpression { Value = properties }
//@[014:00737) | |   └─ObjectExpression
    backendPools: [
//@[004:00717) | |     └─ObjectPropertyExpression
//@[004:00016) | |       ├─StringLiteralExpression { Value = backendPools }
//@[018:00717) | |       └─ArrayExpression
      {
//@[006:00691) | |         └─ObjectExpression
        name: 'BackendAPIMs'
//@[008:00028) | |           ├─ObjectPropertyExpression
//@[008:00012) | |           | ├─StringLiteralExpression { Value = name }
//@[014:00028) | |           | └─StringLiteralExpression { Value = BackendAPIMs }
        properties: {
//@[008:00646) | |           └─ObjectPropertyExpression
//@[008:00018) | |             ├─StringLiteralExpression { Value = properties }
//@[020:00646) | |             └─ObjectExpression
          backends: [for index in range(0, length(accounts)): {
//@[010:00614) | |               └─ObjectPropertyExpression
//@[010:00018) | |                 ├─StringLiteralExpression { Value = backends }
//@[020:00614) | |                 └─ForLoopExpression
//@[034:00060) | |                   ├─FunctionCallExpression { Name = range }
//@[040:00041) | |                   | ├─IntegerLiteralExpression { Value = 0 }
//@[043:00059) | |                   | └─FunctionCallExpression { Name = length }
//@[050:00058) | |                   |   └─ParametersReferenceExpression { Parameter = accounts }
//@[062:00613) | |                   └─ObjectExpression
            // we cannot codegen index correctly because the generated dependsOn property
            // would be outside of the scope of the property loop
            // as a result, this will generate a dependency on the entire collection
            address: storageAccounts[index].properties.primaryEndpoints.internetEndpoints.web
//@[012:00093) | |                     ├─ObjectPropertyExpression
//@[012:00019) | |                     | ├─StringLiteralExpression { Value = address }
//@[021:00093) | |                     | └─PropertyAccessExpression { PropertyName = web }
//@[021:00089) | |                     |   └─PropertyAccessExpression { PropertyName = internetEndpoints }
//@[021:00071) | |                     |     └─PropertyAccessExpression { PropertyName = primaryEndpoints }
//@[021:00054) | |                     |       └─PropertyAccessExpression { PropertyName = properties }
//@[021:00043) | |                     |         └─ResourceReferenceExpression
            backendHostHeader: storageAccounts[index].properties.primaryEndpoints.internetEndpoints.web
//@[012:00103) | |                     ├─ObjectPropertyExpression
//@[012:00029) | |                     | ├─StringLiteralExpression { Value = backendHostHeader }
//@[031:00103) | |                     | └─PropertyAccessExpression { PropertyName = web }
//@[031:00099) | |                     |   └─PropertyAccessExpression { PropertyName = internetEndpoints }
//@[031:00081) | |                     |     └─PropertyAccessExpression { PropertyName = primaryEndpoints }
//@[031:00064) | |                     |       └─PropertyAccessExpression { PropertyName = properties }
//@[031:00053) | |                     |         └─ResourceReferenceExpression
            httpPort: 80
//@[012:00024) | |                     ├─ObjectPropertyExpression
//@[012:00020) | |                     | ├─StringLiteralExpression { Value = httpPort }
//@[022:00024) | |                     | └─IntegerLiteralExpression { Value = 80 }
            httpsPort: 443
//@[012:00026) | |                     ├─ObjectPropertyExpression
//@[012:00021) | |                     | ├─StringLiteralExpression { Value = httpsPort }
//@[023:00026) | |                     | └─IntegerLiteralExpression { Value = 443 }
            priority: 1
//@[012:00023) | |                     ├─ObjectPropertyExpression
//@[012:00020) | |                     | ├─StringLiteralExpression { Value = priority }
//@[022:00023) | |                     | └─IntegerLiteralExpression { Value = 1 }
            weight: 50
//@[012:00022) | |                     └─ObjectPropertyExpression
//@[012:00018) | |                       ├─StringLiteralExpression { Value = weight }
//@[020:00022) | |                       └─IntegerLiteralExpression { Value = 50 }
          }]
        }
      }
    ]
  }
}

resource indexedResourceCollectionDependency 'Microsoft.Network/frontDoors@2020-05-01' = [for index in range(0, length(accounts)): {
//@[000:00848) ├─DeclaredResourceExpression
//@[089:00848) | ├─ForLoopExpression
//@[103:00129) | | ├─FunctionCallExpression { Name = range }
//@[109:00110) | | | ├─IntegerLiteralExpression { Value = 0 }
//@[112:00128) | | | └─FunctionCallExpression { Name = length }
//@[119:00127) | | |   └─ParametersReferenceExpression { Parameter = accounts }
//@[131:00847) | | └─ObjectExpression
  name: '${name}-${index}'
  location: 'Global'
//@[002:00020) | |   ├─ObjectPropertyExpression
//@[002:00010) | |   | ├─StringLiteralExpression { Value = location }
//@[012:00020) | |   | └─StringLiteralExpression { Value = Global }
  properties: {
//@[002:00664) | |   └─ObjectPropertyExpression
//@[002:00012) | |     ├─StringLiteralExpression { Value = properties }
//@[014:00664) | |     └─ObjectExpression
    backendPools: [
//@[004:00644) | |       └─ObjectPropertyExpression
//@[004:00016) | |         ├─StringLiteralExpression { Value = backendPools }
//@[018:00644) | |         └─ArrayExpression
      {
//@[006:00618) | |           └─ObjectExpression
        name: 'BackendAPIMs'
//@[008:00028) | |             ├─ObjectPropertyExpression
//@[008:00012) | |             | ├─StringLiteralExpression { Value = name }
//@[014:00028) | |             | └─StringLiteralExpression { Value = BackendAPIMs }
        properties: {
//@[008:00573) | |             └─ObjectPropertyExpression
//@[008:00018) | |               ├─StringLiteralExpression { Value = properties }
//@[020:00573) | |               └─ObjectExpression
          backends: [
//@[010:00541) | |                 └─ObjectPropertyExpression
//@[010:00018) | |                   ├─StringLiteralExpression { Value = backends }
//@[020:00541) | |                   └─ArrayExpression
            {
//@[012:00507) | |                     └─ObjectExpression
              // this indexed dependency on a module collection will be generated correctly because
              // copyIndex() can be invoked in the generated dependsOn
              address: storageAccounts[index].properties.primaryEndpoints.internetEndpoints.web
//@[014:00095) | |                       ├─ObjectPropertyExpression
//@[014:00021) | |                       | ├─StringLiteralExpression { Value = address }
//@[023:00095) | |                       | └─PropertyAccessExpression { PropertyName = web }
//@[023:00091) | |                       |   └─PropertyAccessExpression { PropertyName = internetEndpoints }
//@[023:00073) | |                       |     └─PropertyAccessExpression { PropertyName = primaryEndpoints }
//@[023:00056) | |                       |       └─PropertyAccessExpression { PropertyName = properties }
//@[023:00045) | |                       |         └─ResourceReferenceExpression
              backendHostHeader: storageAccounts[index].properties.primaryEndpoints.internetEndpoints.web
//@[014:00105) | |                       ├─ObjectPropertyExpression
//@[014:00031) | |                       | ├─StringLiteralExpression { Value = backendHostHeader }
//@[033:00105) | |                       | └─PropertyAccessExpression { PropertyName = web }
//@[033:00101) | |                       |   └─PropertyAccessExpression { PropertyName = internetEndpoints }
//@[033:00083) | |                       |     └─PropertyAccessExpression { PropertyName = primaryEndpoints }
//@[033:00066) | |                       |       └─PropertyAccessExpression { PropertyName = properties }
//@[033:00055) | |                       |         └─ResourceReferenceExpression
              httpPort: 80
//@[014:00026) | |                       ├─ObjectPropertyExpression
//@[014:00022) | |                       | ├─StringLiteralExpression { Value = httpPort }
//@[024:00026) | |                       | └─IntegerLiteralExpression { Value = 80 }
              httpsPort: 443
//@[014:00028) | |                       ├─ObjectPropertyExpression
//@[014:00023) | |                       | ├─StringLiteralExpression { Value = httpsPort }
//@[025:00028) | |                       | └─IntegerLiteralExpression { Value = 443 }
              priority: 1
//@[014:00025) | |                       ├─ObjectPropertyExpression
//@[014:00022) | |                       | ├─StringLiteralExpression { Value = priority }
//@[024:00025) | |                       | └─IntegerLiteralExpression { Value = 1 }
              weight: 50
//@[014:00024) | |                       └─ObjectPropertyExpression
//@[014:00020) | |                         ├─StringLiteralExpression { Value = weight }
//@[022:00024) | |                         └─IntegerLiteralExpression { Value = 50 }
            }
          ]
        }
      }
    ]
  }
}]

resource filteredZones 'Microsoft.Network/dnsZones@2018-05-01' = [for i in range(0,10): if(i % 3 == 0) {
//@[000:00163) ├─DeclaredResourceExpression
//@[065:00163) | └─ForLoopExpression
//@[075:00086) |   ├─FunctionCallExpression { Name = range }
//@[081:00082) |   | ├─IntegerLiteralExpression { Value = 0 }
//@[083:00085) |   | └─IntegerLiteralExpression { Value = 10 }
//@[091:00101) |   └─ConditionExpression
//@[091:00101) |     ├─BinaryExpression { Operator = Equals }
//@[091:00096) |     | ├─BinaryExpression { Operator = Modulo }
//@[091:00092) |     | | ├─ArrayAccessExpression
//@[091:00092) |     | | | ├─CopyIndexExpression
//@[075:00086) |     | | | └─FunctionCallExpression { Name = range }
//@[081:00082) |     | | |   ├─IntegerLiteralExpression { Value = 0 }
//@[083:00085) |     | | |   └─IntegerLiteralExpression { Value = 10 }
//@[095:00096) |     | | └─IntegerLiteralExpression { Value = 3 }
//@[100:00101) |     | └─IntegerLiteralExpression { Value = 0 }
//@[103:00162) |     └─ObjectExpression
  name: 'zone${i}'
  location: resourceGroup().location
//@[002:00036) |       └─ObjectPropertyExpression
//@[002:00010) |         ├─StringLiteralExpression { Value = location }
//@[012:00036) |         └─PropertyAccessExpression { PropertyName = location }
//@[012:00027) |           └─FunctionCallExpression { Name = resourceGroup }
}]

module filteredModules 'passthrough.bicep' = [for i in range(0,6): if(i % 2 == 0) {
//@[000:00149) ├─DeclaredModuleExpression
//@[045:00149) | ├─ForLoopExpression
//@[055:00065) | | ├─FunctionCallExpression { Name = range }
//@[061:00062) | | | ├─IntegerLiteralExpression { Value = 0 }
//@[063:00064) | | | └─IntegerLiteralExpression { Value = 6 }
//@[070:00080) | | └─ConditionExpression
//@[070:00080) | |   ├─BinaryExpression { Operator = Equals }
//@[070:00075) | |   | ├─BinaryExpression { Operator = Modulo }
//@[070:00071) | |   | | ├─ArrayAccessExpression
//@[070:00071) | |   | | | ├─CopyIndexExpression
//@[055:00065) | |   | | | └─FunctionCallExpression { Name = range }
//@[061:00062) | |   | | |   ├─IntegerLiteralExpression { Value = 0 }
//@[063:00064) | |   | | |   └─IntegerLiteralExpression { Value = 6 }
//@[074:00075) | |   | | └─IntegerLiteralExpression { Value = 2 }
//@[079:00080) | |   | └─IntegerLiteralExpression { Value = 0 }
//@[082:00148) | |   └─ObjectExpression
//@[055:00065) | |           └─FunctionCallExpression { Name = range }
//@[061:00062) | |             ├─IntegerLiteralExpression { Value = 0 }
//@[063:00064) | |             └─IntegerLiteralExpression { Value = 6 }
//@[055:00065) |         └─FunctionCallExpression { Name = range }
//@[061:00062) |           ├─IntegerLiteralExpression { Value = 0 }
//@[063:00064) |           └─IntegerLiteralExpression { Value = 6 }
  name: 'stuff${i}'
//@[002:00019) | |     └─ObjectPropertyExpression
//@[002:00006) | |       ├─StringLiteralExpression { Value = name }
//@[008:00019) | |       └─InterpolatedStringExpression
//@[016:00017) | |         └─ArrayAccessExpression
//@[016:00017) | |           ├─CopyIndexExpression
  params: {
//@[010:00042) | └─ObjectExpression
    myInput: 'script-${i}'
//@[004:00026) |   └─ObjectPropertyExpression
//@[004:00011) |     ├─StringLiteralExpression { Value = myInput }
//@[013:00026) |     └─InterpolatedStringExpression
//@[023:00024) |       └─ArrayAccessExpression
//@[023:00024) |         ├─CopyIndexExpression
  }
}]

resource filteredIndexedZones 'Microsoft.Network/dnsZones@2018-05-01' = [for (account, i) in accounts: if(account.enabled) {
//@[000:00199) ├─DeclaredResourceExpression
//@[072:00199) | └─ForLoopExpression
//@[093:00101) |   ├─ParametersReferenceExpression { Parameter = accounts }
//@[106:00121) |   └─ConditionExpression
//@[106:00121) |     ├─PropertyAccessExpression { PropertyName = enabled }
//@[106:00113) |     | └─ArrayAccessExpression
//@[106:00113) |     |   ├─CopyIndexExpression
//@[093:00101) |     |   └─ParametersReferenceExpression { Parameter = accounts }
//@[123:00198) |     └─ObjectExpression
//@[093:00101) |             └─ParametersReferenceExpression { Parameter = accounts }
  name: 'indexedZone-${account.name}-${i}'
  location: account.location
//@[002:00028) |       └─ObjectPropertyExpression
//@[002:00010) |         ├─StringLiteralExpression { Value = location }
//@[012:00028) |         └─PropertyAccessExpression { PropertyName = location }
//@[012:00019) |           └─ArrayAccessExpression
//@[012:00019) |             ├─CopyIndexExpression
}]

output lastNameServers array = filteredIndexedZones[length(accounts) - 1].properties.nameServers
//@[000:00096) ├─DeclaredOutputExpression { Name = lastNameServers }
//@[023:00028) | ├─AmbientTypeReferenceExpression { Name = array }
//@[031:00096) | └─PropertyAccessExpression { PropertyName = nameServers }
//@[031:00084) |   └─PropertyAccessExpression { PropertyName = properties }
//@[031:00073) |     └─ResourceReferenceExpression

module filteredIndexedModules 'passthrough.bicep' = [for (account, i) in accounts: if(account.enabled) {
//@[000:00187) ├─DeclaredModuleExpression
//@[052:00187) | ├─ForLoopExpression
//@[073:00081) | | ├─ParametersReferenceExpression { Parameter = accounts }
//@[086:00101) | | └─ConditionExpression
//@[086:00101) | |   ├─PropertyAccessExpression { PropertyName = enabled }
//@[086:00093) | |   | └─ArrayAccessExpression
//@[086:00093) | |   |   ├─CopyIndexExpression
//@[073:00081) | |   |   └─ParametersReferenceExpression { Parameter = accounts }
//@[103:00186) | |   └─ObjectExpression
//@[073:00081) |       |   └─ParametersReferenceExpression { Parameter = accounts }
  name: 'stuff-${i}'
//@[002:00020) | |     └─ObjectPropertyExpression
//@[002:00006) | |       ├─StringLiteralExpression { Value = name }
//@[008:00020) | |       └─InterpolatedStringExpression
//@[017:00018) | |         └─CopyIndexExpression
  params: {
//@[010:00058) | └─ObjectExpression
    myInput: 'script-${account.name}-${i}'
//@[004:00042) |   └─ObjectPropertyExpression
//@[004:00011) |     ├─StringLiteralExpression { Value = myInput }
//@[013:00042) |     └─InterpolatedStringExpression
//@[023:00035) |       ├─PropertyAccessExpression { PropertyName = name }
//@[023:00030) |       | └─ArrayAccessExpression
//@[023:00030) |       |   ├─CopyIndexExpression
//@[039:00040) |       └─CopyIndexExpression
  }
}]

output lastModuleOutput string = filteredIndexedModules[length(accounts) - 1].outputs.myOutput
//@[000:00094) └─DeclaredOutputExpression { Name = lastModuleOutput }
//@[024:00030)   ├─AmbientTypeReferenceExpression { Name = string }
//@[033:00094)   └─ModuleOutputPropertyAccessExpression { PropertyName = myOutput }
//@[033:00085)     └─PropertyAccessExpression { PropertyName = outputs }
//@[033:00077)       └─ModuleReferenceExpression

