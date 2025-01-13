param name string
//@[000:11469) ProgramExpression
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
//@[000:00307) ├─DeclaredResourceExpression
resource storageAccounts 'Microsoft.Storage/storageAccounts@2019-06-01' = [for (account, index) in accounts: {
//@[074:00292) | ├─ForLoopExpression
//@[099:00107) | | ├─ParametersReferenceExpression { Parameter = accounts }
//@[109:00291) | | └─ObjectExpression
//@[099:00107) | |   |     └─ParametersReferenceExpression { Parameter = accounts }
  name: '${name}-collection-${account.name}-${index}'
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
resource extensionCollection 'Microsoft.Authorization/locks@2016-09-01' = [for (i, i2) in range(0,1): {
//@[000:00235) ├─DeclaredResourceExpression
//@[074:00235) | ├─ForLoopExpression
//@[090:00100) | | ├─FunctionCallExpression { Name = range }
//@[096:00097) | | | ├─IntegerLiteralExpression { Value = 0 }
//@[098:00099) | | | └─IntegerLiteralExpression { Value = 1 }
//@[102:00234) | | └─ObjectExpression
//@[090:00100) | |           | | | └─FunctionCallExpression { Name = range }
//@[096:00097) | |           | | |   ├─IntegerLiteralExpression { Value = 0 }
//@[098:00099) | |           | | |   └─IntegerLiteralExpression { Value = 1 }
  name: 'lock-${i}-${i2}'
  properties: {
//@[002:00078) | |   └─ObjectPropertyExpression
//@[002:00012) | |     ├─StringLiteralExpression { Value = properties }
//@[014:00078) | |     └─ObjectExpression
    level: i == 0 && i2 == 0 ? 'CanNotDelete' : 'ReadOnly'
//@[004:00058) | |       └─ObjectPropertyExpression
//@[004:00009) | |         ├─StringLiteralExpression { Value = level }
//@[011:00058) | |         └─TernaryExpression
//@[011:00028) | |           ├─BinaryExpression { Operator = LogicalAnd }
//@[011:00017) | |           | ├─BinaryExpression { Operator = Equals }
//@[011:00012) | |           | | ├─ArrayAccessExpression
//@[011:00012) | |           | | | ├─CopyIndexExpression
//@[016:00017) | |           | | └─IntegerLiteralExpression { Value = 0 }
//@[021:00028) | |           | └─BinaryExpression { Operator = Equals }
//@[021:00023) | |           |   ├─CopyIndexExpression
//@[027:00028) | |           |   └─IntegerLiteralExpression { Value = 0 }
//@[031:00045) | |           ├─StringLiteralExpression { Value = CanNotDelete }
//@[048:00058) | |           └─StringLiteralExpression { Value = ReadOnly }
  }
  scope: singleResource
}]

// cascade extend the extension
@batchSize(1)
//@[000:00260) ├─DeclaredResourceExpression
resource lockTheLocks 'Microsoft.Authorization/locks@2016-09-01' = [for (i, i2) in range(0,1): {
//@[067:00246) | ├─ForLoopExpression
//@[083:00093) | | ├─FunctionCallExpression { Name = range }
//@[089:00090) | | | ├─IntegerLiteralExpression { Value = 0 }
//@[091:00092) | | | └─IntegerLiteralExpression { Value = 1 }
//@[095:00245) | | └─ObjectExpression
//@[083:00093) | |           | | | └─FunctionCallExpression { Name = range }
//@[089:00090) | |           | | |   ├─IntegerLiteralExpression { Value = 0 }
//@[091:00092) | |           | | |   └─IntegerLiteralExpression { Value = 1 }
  name: 'lock-the-lock-${i}-${i2}'
  properties: {
//@[002:00078) | |   └─ObjectPropertyExpression
//@[002:00012) | |     ├─StringLiteralExpression { Value = properties }
//@[014:00078) | |     └─ObjectExpression
    level: i == 0 && i2 == 0 ? 'CanNotDelete' : 'ReadOnly'
//@[004:00058) | |       └─ObjectPropertyExpression
//@[004:00009) | |         ├─StringLiteralExpression { Value = level }
//@[011:00058) | |         └─TernaryExpression
//@[011:00028) | |           ├─BinaryExpression { Operator = LogicalAnd }
//@[011:00017) | |           | ├─BinaryExpression { Operator = Equals }
//@[011:00012) | |           | | ├─ArrayAccessExpression
//@[011:00012) | |           | | | ├─CopyIndexExpression
//@[016:00017) | |           | | └─IntegerLiteralExpression { Value = 0 }
//@[021:00028) | |           | └─BinaryExpression { Operator = Equals }
//@[021:00023) | |           |   ├─CopyIndexExpression
//@[027:00028) | |           |   └─IntegerLiteralExpression { Value = 0 }
//@[031:00045) | |           ├─StringLiteralExpression { Value = CanNotDelete }
//@[048:00058) | |           └─StringLiteralExpression { Value = ReadOnly }
  }
  scope: extensionCollection[i2]
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
resource storageAccounts2 'Microsoft.Storage/storageAccounts@2019-06-01' = [for (account, idx) in accounts: {
//@[000:00290) ├─DeclaredResourceExpression
//@[075:00290) | ├─ForLoopExpression
//@[098:00106) | | ├─ParametersReferenceExpression { Parameter = accounts }
//@[108:00289) | | └─ObjectExpression
//@[098:00106) | |   |     └─ParametersReferenceExpression { Parameter = accounts }
  name: '${name}-collection-${account.name}-${idx}'
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
resource firstSet 'Microsoft.Storage/storageAccounts@2019-06-01' = [for (i,ii) in range(0, length(accounts)): {
//@[000:00243) ├─DeclaredResourceExpression
//@[067:00243) | └─ForLoopExpression
//@[082:00108) |   ├─FunctionCallExpression { Name = range }
//@[088:00089) |   | ├─IntegerLiteralExpression { Value = 0 }
//@[091:00107) |   | └─FunctionCallExpression { Name = length }
//@[098:00106) |   |   └─ParametersReferenceExpression { Parameter = accounts }
//@[110:00242) |   └─ObjectExpression
  name: '${name}-set1-${i}-${ii}'
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

resource secondSet 'Microsoft.Storage/storageAccounts@2019-06-01' = [for (i,iii) in range(0, length(accounts)): {
//@[000:00283) ├─DeclaredResourceExpression
//@[068:00283) | ├─ForLoopExpression
//@[084:00110) | | ├─FunctionCallExpression { Name = range }
//@[090:00091) | | | ├─IntegerLiteralExpression { Value = 0 }
//@[093:00109) | | | └─FunctionCallExpression { Name = length }
//@[100:00108) | | |   └─ParametersReferenceExpression { Parameter = accounts }
//@[112:00282) | | └─ObjectExpression
  name: '${name}-set2-${i}-${iii}'
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
    firstSet[iii]
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

resource vnets 'Microsoft.Network/virtualNetworks@2020-06-01' = [for (vnetConfig, index) in vnetConfigurations: {
//@[000:00186) ├─DeclaredResourceExpression
//@[064:00186) | └─ForLoopExpression
//@[092:00110) |   ├─VariableReferenceExpression { Variable = vnetConfigurations }
//@[112:00185) |   └─ObjectExpression
//@[092:00110) |           └─VariableReferenceExpression { Variable = vnetConfigurations }
  name: '${vnetConfig.name}-${index}'
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
//@[000:00293) ├─DeclaredModuleExpression
module moduleCollectionWithSingleDependency 'passthrough.bicep' = [for (moduleName, moduleIndex) in moduleSetup: {
//@[066:00275) | ├─ForLoopExpression
//@[100:00111) | | ├─VariableReferenceExpression { Variable = moduleSetup }
//@[113:00274) | | └─ObjectExpression
//@[100:00111) | |       | └─VariableReferenceExpression { Variable = moduleSetup }
//@[100:00111) | |     | └─VariableReferenceExpression { Variable = moduleSetup }
  name: concat(moduleName, moduleIndex)
//@[002:00039) | |   └─ObjectPropertyExpression
//@[002:00006) | |     ├─StringLiteralExpression { Value = name }
//@[008:00039) | |     └─FunctionCallExpression { Name = concat }
//@[015:00025) | |       ├─ArrayAccessExpression
//@[015:00025) | |       | ├─CopyIndexExpression
//@[027:00038) | |       └─CopyIndexExpression
  params: {
//@[010:00062) | ├─ObjectExpression
    myInput: 'in-${moduleName}-${moduleIndex}'
//@[004:00046) | | └─ObjectPropertyExpression
//@[004:00011) | |   ├─StringLiteralExpression { Value = myInput }
//@[013:00046) | |   └─InterpolatedStringExpression
//@[019:00029) | |     ├─ArrayAccessExpression
//@[019:00029) | |     | ├─CopyIndexExpression
//@[033:00044) | |     └─CopyIndexExpression
  }
  dependsOn: [
    singleModule
    singleResource
  ]
}]

// another module collection with dependency on another module collection
module moduleCollectionWithCollectionDependencies 'passthrough.bicep' = [for (moduleName, moduleIndex) in moduleSetup: {
//@[000:00306) ├─DeclaredModuleExpression
//@[072:00306) | ├─ForLoopExpression
//@[106:00117) | | ├─VariableReferenceExpression { Variable = moduleSetup }
//@[119:00305) | | └─ObjectExpression
//@[106:00117) | |       | └─VariableReferenceExpression { Variable = moduleSetup }
//@[106:00117) | |     | └─VariableReferenceExpression { Variable = moduleSetup }
  name: concat(moduleName, moduleIndex)
//@[002:00039) | |   └─ObjectPropertyExpression
//@[002:00006) | |     ├─StringLiteralExpression { Value = name }
//@[008:00039) | |     └─FunctionCallExpression { Name = concat }
//@[015:00025) | |       ├─ArrayAccessExpression
//@[015:00025) | |       | ├─CopyIndexExpression
//@[027:00038) | |       └─CopyIndexExpression
  params: {
//@[010:00062) | ├─ObjectExpression
    myInput: 'in-${moduleName}-${moduleIndex}'
//@[004:00046) | | └─ObjectPropertyExpression
//@[004:00011) | |   ├─StringLiteralExpression { Value = myInput }
//@[013:00046) | |   └─InterpolatedStringExpression
//@[019:00029) | |     ├─ArrayAccessExpression
//@[019:00029) | |     | ├─CopyIndexExpression
//@[033:00044) | |     └─CopyIndexExpression
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

module moduleCollectionWithIndexedDependencies 'passthrough.bicep' = [for (moduleName, moduleIndex) in moduleSetup: {
//@[000:00399) ├─DeclaredModuleExpression
//@[069:00399) | ├─ForLoopExpression
//@[103:00114) | | ├─VariableReferenceExpression { Variable = moduleSetup }
//@[116:00398) | | └─ObjectExpression
//@[103:00114) | |       | └─VariableReferenceExpression { Variable = moduleSetup }
//@[103:00114) | |     | └─VariableReferenceExpression { Variable = moduleSetup }
  name: concat(moduleName, moduleIndex)
//@[002:00039) | |   └─ObjectPropertyExpression
//@[002:00006) | |     ├─StringLiteralExpression { Value = name }
//@[008:00039) | |     └─FunctionCallExpression { Name = concat }
//@[015:00025) | |       ├─ArrayAccessExpression
//@[015:00025) | |       | ├─CopyIndexExpression
//@[027:00038) | |       └─CopyIndexExpression
  params: {
//@[010:00187) | ├─ObjectExpression
    myInput: '${moduleCollectionWithCollectionDependencies[index].outputs.myOutput} - ${storageAccounts[index * 3].properties.accessTier} - ${moduleName} - ${moduleIndex}'
//@[004:00171) | | └─ObjectPropertyExpression
//@[004:00011) | |   ├─StringLiteralExpression { Value = myInput }
//@[013:00171) | |   └─InterpolatedStringExpression
//@[016:00082) | |     ├─ModuleOutputPropertyAccessExpression { PropertyName = myOutput }
//@[016:00073) | |     | └─PropertyAccessExpression { PropertyName = outputs }
//@[016:00065) | |     |   └─ModuleReferenceExpression
//@[088:00136) | |     ├─PropertyAccessExpression { PropertyName = accessTier }
//@[088:00125) | |     | └─PropertyAccessExpression { PropertyName = properties }
//@[088:00114) | |     |   └─ResourceReferenceExpression
//@[142:00152) | |     ├─ArrayAccessExpression
//@[142:00152) | |     | ├─CopyIndexExpression
//@[158:00169) | |     └─CopyIndexExpression
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
resource existingStorageAccounts 'Microsoft.Storage/storageAccounts@2019-06-01' existing = [for (account, i) in accounts: {
//@[000:00174) ├─DeclaredResourceExpression
//@[091:00174) | └─ForLoopExpression
//@[112:00120) |   ├─ParametersReferenceExpression { Parameter = accounts }
//@[122:00173) |   └─ObjectExpression
  name: '${name}-existing-${account.name}-${i}'
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
//@[000:00104) └─DeclaredOutputExpression { Name = existingIndexedResourceAccessTier }
//@[041:00047)   ├─AmbientTypeReferenceExpression { Name = string }
//@[050:00104)   └─PropertyAccessExpression { PropertyName = accessTier }
//@[050:00093)     └─PropertyAccessExpression { PropertyName = properties }
//@[050:00082)       └─ResourceReferenceExpression

resource duplicatedNames 'Microsoft.Network/dnsZones@2018-05-01' = [for (zone,i) in []: {
//@[000:00140) ├─DeclaredResourceExpression
//@[067:00140) | └─ForLoopExpression
//@[084:00086) |   ├─ArrayExpression
//@[088:00139) |   └─ObjectExpression
  name: 'no loop variable'
  location: 'eastus'
//@[002:00020) |     └─ObjectPropertyExpression
//@[002:00010) |       ├─StringLiteralExpression { Value = location }
//@[012:00020) |       └─StringLiteralExpression { Value = eastus }
}]

// reference to a resource collection whose name expression does not reference any loop variables
resource referenceToDuplicateNames 'Microsoft.Network/dnsZones@2018-05-01' = [for (zone,i) in []: {
//@[000:00198) ├─DeclaredResourceExpression
//@[077:00198) | ├─ForLoopExpression
//@[094:00096) | | ├─ArrayExpression
//@[098:00197) | | └─ObjectExpression
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

module apim 'passthrough.bicep' = [for (region, i) in regions: {
//@[000:00141) ├─DeclaredModuleExpression
//@[034:00141) | ├─ForLoopExpression
//@[054:00061) | | ├─VariableReferenceExpression { Variable = regions }
//@[063:00140) | | └─ObjectExpression
//@[054:00061) | |       | └─VariableReferenceExpression { Variable = regions }
//@[054:00061) |       └─VariableReferenceExpression { Variable = regions }
  name: 'apim-${region}-${name}-${i}'
//@[002:00037) | |   └─ObjectPropertyExpression
//@[002:00006) | |     ├─StringLiteralExpression { Value = name }
//@[008:00037) | |     └─InterpolatedStringExpression
//@[016:00022) | |       ├─ArrayAccessExpression
//@[016:00022) | |       | ├─CopyIndexExpression
//@[026:00030) | |       ├─ParametersReferenceExpression { Parameter = name }
//@[034:00035) | |       └─CopyIndexExpression
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
//@[000:00792) ├─DeclaredResourceExpression
//@[094:00792) | ├─ObjectExpression
  name: name
  location: 'Global'
//@[002:00020) | | ├─ObjectPropertyExpression
//@[002:00010) | | | ├─StringLiteralExpression { Value = location }
//@[012:00020) | | | └─StringLiteralExpression { Value = Global }
  properties: {
//@[002:00660) | | └─ObjectPropertyExpression
//@[002:00012) | |   ├─StringLiteralExpression { Value = properties }
//@[014:00660) | |   └─ObjectExpression
    backendPools: [
//@[004:00640) | |     └─ObjectPropertyExpression
//@[004:00016) | |       ├─StringLiteralExpression { Value = backendPools }
//@[018:00640) | |       └─ArrayExpression
      {
//@[006:00614) | |         └─ObjectExpression
        name: 'BackendAPIMs'
//@[008:00028) | |           ├─ObjectPropertyExpression
//@[008:00012) | |           | ├─StringLiteralExpression { Value = name }
//@[014:00028) | |           | └─StringLiteralExpression { Value = BackendAPIMs }
        properties: {
//@[008:00569) | |           └─ObjectPropertyExpression
//@[008:00018) | |             ├─StringLiteralExpression { Value = properties }
//@[020:00569) | |             └─ObjectExpression
          backends: [for (index,i) in range(0, length(regions)): {
//@[010:00537) | |               └─ObjectPropertyExpression
//@[010:00018) | |                 ├─StringLiteralExpression { Value = backends }
//@[020:00537) | |                 └─ForLoopExpression
//@[038:00063) | |                   ├─FunctionCallExpression { Name = range }
//@[044:00045) | |                   | ├─IntegerLiteralExpression { Value = 0 }
//@[047:00062) | |                   | └─FunctionCallExpression { Name = length }
//@[054:00061) | |                   |   └─VariableReferenceExpression { Variable = regions }
//@[065:00536) | |                   └─ObjectExpression
            // we cannot codegen index correctly because the generated dependsOn property
            // would be outside of the scope of the property loop
            // as a result, this will generate a dependency on the entire collection
            address: apim[index + i].outputs.myOutput
//@[012:00053) | |                     ├─ObjectPropertyExpression
//@[012:00019) | |                     | ├─StringLiteralExpression { Value = address }
//@[021:00053) | |                     | └─ModuleOutputPropertyAccessExpression { PropertyName = myOutput }
//@[021:00044) | |                     |   └─PropertyAccessExpression { PropertyName = outputs }
//@[021:00036) | |                     |     └─ModuleReferenceExpression
            backendHostHeader: apim[index + i].outputs.myOutput
//@[012:00063) | |                     ├─ObjectPropertyExpression
//@[012:00029) | |                     | ├─StringLiteralExpression { Value = backendHostHeader }
//@[031:00063) | |                     | └─ModuleOutputPropertyAccessExpression { PropertyName = myOutput }
//@[031:00054) | |                     |   └─PropertyAccessExpression { PropertyName = outputs }
//@[031:00046) | |                     |     └─ModuleReferenceExpression
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

resource indexedModuleCollectionDependency 'Microsoft.Network/frontDoors@2020-05-01' = [for (index, i) in range(0, length(regions)): {
//@[000:00771) ├─DeclaredResourceExpression
//@[087:00771) | ├─ForLoopExpression
//@[106:00131) | | ├─FunctionCallExpression { Name = range }
//@[112:00113) | | | ├─IntegerLiteralExpression { Value = 0 }
//@[115:00130) | | | └─FunctionCallExpression { Name = length }
//@[122:00129) | | |   └─VariableReferenceExpression { Variable = regions }
//@[133:00770) | | └─ObjectExpression
  name: '${name}-${index}-${i}'
  location: 'Global'
//@[002:00020) | |   ├─ObjectPropertyExpression
//@[002:00010) | |   | ├─StringLiteralExpression { Value = location }
//@[012:00020) | |   | └─StringLiteralExpression { Value = Global }
  properties: {
//@[002:00580) | |   └─ObjectPropertyExpression
//@[002:00012) | |     ├─StringLiteralExpression { Value = properties }
//@[014:00580) | |     └─ObjectExpression
    backendPools: [
//@[004:00560) | |       └─ObjectPropertyExpression
//@[004:00016) | |         ├─StringLiteralExpression { Value = backendPools }
//@[018:00560) | |         └─ArrayExpression
      {
//@[006:00534) | |           └─ObjectExpression
        name: 'BackendAPIMs'
//@[008:00028) | |             ├─ObjectPropertyExpression
//@[008:00012) | |             | ├─StringLiteralExpression { Value = name }
//@[014:00028) | |             | └─StringLiteralExpression { Value = BackendAPIMs }
        properties: {
//@[008:00489) | |             └─ObjectPropertyExpression
//@[008:00018) | |               ├─StringLiteralExpression { Value = properties }
//@[020:00489) | |               └─ObjectExpression
          backends: [
//@[010:00457) | |                 └─ObjectPropertyExpression
//@[010:00018) | |                   ├─StringLiteralExpression { Value = backends }
//@[020:00457) | |                   └─ArrayExpression
            {
//@[012:00423) | |                     └─ObjectExpression
              // this indexed dependency on a module collection will be generated correctly because
              // copyIndex() can be invoked in the generated dependsOn
              address: apim[index+i].outputs.myOutput
//@[014:00053) | |                       ├─ObjectPropertyExpression
//@[014:00021) | |                       | ├─StringLiteralExpression { Value = address }
//@[023:00053) | |                       | └─ModuleOutputPropertyAccessExpression { PropertyName = myOutput }
//@[023:00044) | |                       |   └─PropertyAccessExpression { PropertyName = outputs }
//@[023:00036) | |                       |     └─ModuleReferenceExpression
              backendHostHeader: apim[index+i].outputs.myOutput
//@[014:00063) | |                       ├─ObjectPropertyExpression
//@[014:00031) | |                       | ├─StringLiteralExpression { Value = backendHostHeader }
//@[033:00063) | |                       | └─ModuleOutputPropertyAccessExpression { PropertyName = myOutput }
//@[033:00054) | |                       |   └─PropertyAccessExpression { PropertyName = outputs }
//@[033:00046) | |                       |     └─ModuleReferenceExpression
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

resource indexedResourceCollectionDependency 'Microsoft.Network/frontDoors@2020-05-01' = [for (index,i) in range(0, length(accounts)): {
//@[000:00861) ├─DeclaredResourceExpression
//@[089:00861) | ├─ForLoopExpression
//@[107:00133) | | ├─FunctionCallExpression { Name = range }
//@[113:00114) | | | ├─IntegerLiteralExpression { Value = 0 }
//@[116:00132) | | | └─FunctionCallExpression { Name = length }
//@[123:00131) | | |   └─ParametersReferenceExpression { Parameter = accounts }
//@[135:00860) | | └─ObjectExpression
  name: '${name}-${index}-${i}'
  location: 'Global'
//@[002:00020) | |   ├─ObjectPropertyExpression
//@[002:00010) | |   | ├─StringLiteralExpression { Value = location }
//@[012:00020) | |   | └─StringLiteralExpression { Value = Global }
  properties: {
//@[002:00668) | |   └─ObjectPropertyExpression
//@[002:00012) | |     ├─StringLiteralExpression { Value = properties }
//@[014:00668) | |     └─ObjectExpression
    backendPools: [
//@[004:00648) | |       └─ObjectPropertyExpression
//@[004:00016) | |         ├─StringLiteralExpression { Value = backendPools }
//@[018:00648) | |         └─ArrayExpression
      {
//@[006:00622) | |           └─ObjectExpression
        name: 'BackendAPIMs'
//@[008:00028) | |             ├─ObjectPropertyExpression
//@[008:00012) | |             | ├─StringLiteralExpression { Value = name }
//@[014:00028) | |             | └─StringLiteralExpression { Value = BackendAPIMs }
        properties: {
//@[008:00577) | |             └─ObjectPropertyExpression
//@[008:00018) | |               ├─StringLiteralExpression { Value = properties }
//@[020:00577) | |               └─ObjectExpression
          backends: [
//@[010:00545) | |                 └─ObjectPropertyExpression
//@[010:00018) | |                   ├─StringLiteralExpression { Value = backends }
//@[020:00545) | |                   └─ArrayExpression
            {
//@[012:00511) | |                     └─ObjectExpression
              // this indexed dependency on a module collection will be generated correctly because
              // copyIndex() can be invoked in the generated dependsOn
              address: storageAccounts[index+i].properties.primaryEndpoints.internetEndpoints.web
//@[014:00097) | |                       ├─ObjectPropertyExpression
//@[014:00021) | |                       | ├─StringLiteralExpression { Value = address }
//@[023:00097) | |                       | └─PropertyAccessExpression { PropertyName = web }
//@[023:00093) | |                       |   └─PropertyAccessExpression { PropertyName = internetEndpoints }
//@[023:00075) | |                       |     └─PropertyAccessExpression { PropertyName = primaryEndpoints }
//@[023:00058) | |                       |       └─PropertyAccessExpression { PropertyName = properties }
//@[023:00047) | |                       |         └─ResourceReferenceExpression
              backendHostHeader: storageAccounts[index+i].properties.primaryEndpoints.internetEndpoints.web
//@[014:00107) | |                       ├─ObjectPropertyExpression
//@[014:00031) | |                       | ├─StringLiteralExpression { Value = backendHostHeader }
//@[033:00107) | |                       | └─PropertyAccessExpression { PropertyName = web }
//@[033:00103) | |                       |   └─PropertyAccessExpression { PropertyName = internetEndpoints }
//@[033:00085) | |                       |     └─PropertyAccessExpression { PropertyName = primaryEndpoints }
//@[033:00068) | |                       |       └─PropertyAccessExpression { PropertyName = properties }
//@[033:00057) | |                       |         └─ResourceReferenceExpression
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

