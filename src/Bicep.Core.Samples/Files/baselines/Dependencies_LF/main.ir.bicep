param deployTimeParam string = 'steve'
//@[00:1100) ProgramExpression
//@[00:0038) ├─DeclaredParameterExpression { Name = deployTimeParam }
//@[22:0028) | ├─AmbientTypeReferenceExpression { Name = string }
//@[31:0038) | └─StringLiteralExpression { Value = steve }
//@[00:0000) | └─ResourceDependencyExpression [UNPARENTED]
//@[00:0000) |   └─ResourceReferenceExpression [UNPARENTED]
//@[00:0000) | ├─ResourceDependencyExpression [UNPARENTED]
//@[00:0000) | | └─ResourceReferenceExpression [UNPARENTED]
//@[00:0000) | └─ResourceDependencyExpression [UNPARENTED]
//@[00:0000) |   └─ResourceReferenceExpression [UNPARENTED]
//@[00:0000) | └─ResourceDependencyExpression [UNPARENTED]
//@[00:0000) |   └─ResourceReferenceExpression [UNPARENTED]
//@[00:0000) | └─ResourceDependencyExpression [UNPARENTED]
//@[00:0000) |   └─ResourceReferenceExpression [UNPARENTED]
var deployTimeVar = 'nigel'
//@[00:0027) ├─DeclaredVariableExpression { Name = deployTimeVar }
//@[20:0027) | └─StringLiteralExpression { Value = nigel }
var dependentVar = {
//@[00:0082) ├─DeclaredVariableExpression { Name = dependentVar }
//@[19:0082) | └─ObjectExpression
  dependencies: [
//@[02:0059) |   └─ObjectPropertyExpression
//@[02:0014) |     ├─StringLiteralExpression { Value = dependencies }
//@[16:0059) |     └─ArrayExpression
    deployTimeVar
//@[04:0017) |       ├─VariableReferenceExpression { Variable = deployTimeVar }
    deployTimeParam
//@[04:0019) |       └─ParametersReferenceExpression { Parameter = deployTimeParam }
  ]
}

var resourceDependency = {
//@[25:0147) | |       └─ObjectExpression
  dependenciesA: [
//@[02:0118) | |         └─ObjectPropertyExpression
//@[02:0015) | |           ├─StringLiteralExpression { Value = dependenciesA }
//@[17:0118) | |           └─ArrayExpression
    resA.id
//@[04:0011) | |             ├─PropertyAccessExpression { PropertyName = id }
//@[04:0008) | |             | └─ResourceReferenceExpression
    resA.name
//@[04:0013) | |             ├─PropertyAccessExpression { PropertyName = name }
//@[04:0008) | |             | └─ResourceReferenceExpression
    resA.type
//@[04:0013) | |             ├─PropertyAccessExpression { PropertyName = type }
//@[04:0008) | |             | └─ResourceReferenceExpression
    resA.properties.deployTime
//@[04:0030) | |             ├─PropertyAccessExpression { PropertyName = deployTime }
//@[04:0019) | |             | └─PropertyAccessExpression { PropertyName = properties }
//@[04:0008) | |             |   └─ResourceReferenceExpression
    resA.properties.eTag
//@[04:0024) | |             └─PropertyAccessExpression { PropertyName = eTag }
//@[04:0019) | |               └─PropertyAccessExpression { PropertyName = properties }
//@[04:0008) | |                 └─ResourceReferenceExpression
  ]
}

output resourceAType string = resA.type
//@[00:0039) ├─DeclaredOutputExpression { Name = resourceAType }
//@[21:0027) | ├─AmbientTypeReferenceExpression { Name = string }
//@[30:0039) | └─PropertyAccessExpression { PropertyName = type }
//@[30:0034) |   └─ResourceReferenceExpression
resource resA 'My.Rp/myResourceType@2020-01-01' = {
//@[00:0134) ├─DeclaredResourceExpression
//@[50:0134) | └─ObjectExpression
  name: 'resA'
  properties: {
//@[02:0065) |   └─ObjectPropertyExpression
//@[02:0012) |     ├─StringLiteralExpression { Value = properties }
//@[14:0065) |     └─ObjectExpression
    deployTime: dependentVar
//@[04:0028) |       ├─ObjectPropertyExpression
//@[04:0014) |       | ├─StringLiteralExpression { Value = deployTime }
//@[16:0028) |       | └─VariableReferenceExpression { Variable = dependentVar }
    eTag: '1234'
//@[04:0016) |       └─ObjectPropertyExpression
//@[04:0008) |         ├─StringLiteralExpression { Value = eTag }
//@[10:0016) |         └─StringLiteralExpression { Value = 1234 }
  }
}

output resourceBId string = resB.id
//@[00:0035) ├─DeclaredOutputExpression { Name = resourceBId }
//@[19:0025) | ├─AmbientTypeReferenceExpression { Name = string }
//@[28:0035) | └─PropertyAccessExpression { PropertyName = id }
//@[28:0032) |   └─ResourceReferenceExpression
resource resB 'My.Rp/myResourceType@2020-01-01' = {
//@[00:0125) ├─DeclaredResourceExpression
//@[50:0125) | ├─ObjectExpression
  name: 'resB'
  properties: {
//@[02:0056) | | └─ObjectPropertyExpression
//@[02:0012) | |   ├─StringLiteralExpression { Value = properties }
//@[14:0056) | |   └─ObjectExpression
    dependencies: resourceDependency
//@[04:0036) | |     └─ObjectPropertyExpression
//@[04:0016) | |       ├─StringLiteralExpression { Value = dependencies }
  }
}

var resourceIds = {
//@[00:0047) ├─DeclaredVariableExpression { Name = resourceIds }
//@[18:0047) | └─ObjectExpression
  a: resA.id
//@[02:0012) |   ├─ObjectPropertyExpression
//@[02:0003) |   | ├─StringLiteralExpression { Value = a }
//@[05:0012) |   | └─PropertyAccessExpression { PropertyName = id }
//@[05:0009) |   |   └─ResourceReferenceExpression
  b: resB.id
//@[02:0012) |   └─ObjectPropertyExpression
//@[02:0003) |     ├─StringLiteralExpression { Value = b }
//@[05:0012) |     └─PropertyAccessExpression { PropertyName = id }
//@[05:0009) |       └─ResourceReferenceExpression
}

resource resC 'My.Rp/myResourceType@2020-01-01' = {
//@[00:0117) ├─DeclaredResourceExpression
//@[50:0117) | ├─ObjectExpression
  name: 'resC'
  properties: {
//@[02:0048) | | └─ObjectPropertyExpression
//@[02:0012) | |   ├─StringLiteralExpression { Value = properties }
//@[14:0048) | |   └─ObjectExpression
    resourceIds: resourceIds
//@[04:0028) | |     └─ObjectPropertyExpression
//@[04:0015) | |       ├─StringLiteralExpression { Value = resourceIds }
//@[17:0028) | |       └─VariableReferenceExpression { Variable = resourceIds }
  }
}

resource resD 'My.Rp/myResourceType/childType@2020-01-01' = {
//@[00:0111) ├─DeclaredResourceExpression
//@[60:0111) | ├─ObjectExpression
  name: '${resC.name}/resD'
  properties: {
//@[02:0019) | | └─ObjectPropertyExpression
//@[02:0012) | |   ├─StringLiteralExpression { Value = properties }
//@[14:0019) | |   └─ObjectExpression
  }
}

resource resE 'My.Rp/myResourceType/childType@2020-01-01' = {
//@[00:0126) ├─DeclaredResourceExpression
//@[60:0126) | ├─ObjectExpression
  name: 'resC/resD_2'
  properties: {
//@[02:0040) | | └─ObjectPropertyExpression
//@[02:0012) | |   ├─StringLiteralExpression { Value = properties }
//@[14:0040) | |   └─ObjectExpression
    resDRef: resD.id
//@[04:0020) | |     └─ObjectPropertyExpression
//@[04:0011) | |       ├─StringLiteralExpression { Value = resDRef }
//@[13:0020) | |       └─PropertyAccessExpression { PropertyName = id }
//@[13:0017) | |         └─ResourceReferenceExpression
  }
}

output resourceCProperties object = resC.properties
//@[00:0051) └─DeclaredOutputExpression { Name = resourceCProperties }
//@[27:0033)   ├─AmbientTypeReferenceExpression { Name = object }
//@[36:0051)   └─PropertyAccessExpression { PropertyName = properties }
//@[36:0040)     └─ResourceReferenceExpression

