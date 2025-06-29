resource basicParent 'My.Rp/parentType@2020-12-01' = {
//@[00:2073) ProgramExpression
//@[00:0000) | ├─ResourceDependencyExpression [UNPARENTED]
//@[00:0000) | | └─ResourceReferenceExpression [UNPARENTED]
//@[00:0000) | └─ResourceDependencyExpression [UNPARENTED]
//@[00:0000) |   └─ResourceReferenceExpression [UNPARENTED]
//@[00:0000) | └─ResourceDependencyExpression [UNPARENTED]
//@[00:0000) |   └─ResourceReferenceExpression [UNPARENTED]
//@[00:0000) | ├─ResourceDependencyExpression [UNPARENTED]
//@[00:0000) | | └─ResourceReferenceExpression [UNPARENTED]
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
//@[00:0659) ├─DeclaredResourceExpression
//@[53:0659) | └─ObjectExpression
  name: 'basicParent'
  properties: {
//@[02:0037) |   └─ObjectPropertyExpression
//@[02:0012) |     ├─StringLiteralExpression { Value = properties }
//@[14:0037) |     └─ObjectExpression
    size: 'large'
//@[04:0017) |       └─ObjectPropertyExpression
//@[04:0008) |         ├─StringLiteralExpression { Value = size }
//@[10:0017) |         └─StringLiteralExpression { Value = large }
  }

  resource basicChild 'childType' = {
//@[02:0349) ├─DeclaredResourceExpression
//@[36:0349) | ├─ObjectExpression
    name: 'basicChild'
    properties: {
//@[04:0084) | | └─ObjectPropertyExpression
//@[04:0014) | |   ├─StringLiteralExpression { Value = properties }
//@[16:0084) | |   └─ObjectExpression
      size: basicParent.properties.large
//@[06:0040) | |     ├─ObjectPropertyExpression
//@[06:0010) | |     | ├─StringLiteralExpression { Value = size }
//@[12:0040) | |     | └─PropertyAccessExpression { PropertyName = large }
//@[12:0034) | |     |   └─PropertyAccessExpression { PropertyName = properties }
//@[12:0023) | |     |     └─ResourceReferenceExpression
      style: 'cool'
//@[06:0019) | |     └─ObjectPropertyExpression
//@[06:0011) | |       ├─StringLiteralExpression { Value = style }
//@[13:0019) | |       └─StringLiteralExpression { Value = cool }
    }

    resource basicGrandchild 'grandchildType' = {
//@[04:0198) ├─DeclaredResourceExpression
//@[48:0198) | ├─ObjectExpression
      name: 'basicGrandchild'
      properties: {
//@[06:0112) | | └─ObjectPropertyExpression
//@[06:0016) | |   ├─StringLiteralExpression { Value = properties }
//@[18:0112) | |   └─ObjectExpression
        size: basicParent.properties.size
//@[08:0041) | |     ├─ObjectPropertyExpression
//@[08:0012) | |     | ├─StringLiteralExpression { Value = size }
//@[14:0041) | |     | └─PropertyAccessExpression { PropertyName = size }
//@[14:0036) | |     |   └─PropertyAccessExpression { PropertyName = properties }
//@[14:0025) | |     |     └─ResourceReferenceExpression
        style: basicChild.properties.style
//@[08:0042) | |     └─ObjectPropertyExpression
//@[08:0013) | |       ├─StringLiteralExpression { Value = style }
//@[15:0042) | |       └─PropertyAccessExpression { PropertyName = style }
//@[15:0036) | |         └─PropertyAccessExpression { PropertyName = properties }
//@[15:0025) | |           └─ResourceReferenceExpression
      }
    }
  }

  resource basicSibling 'childType' = {
//@[02:0190) ├─DeclaredResourceExpression
//@[38:0190) | ├─ObjectExpression
    name: 'basicSibling'
    properties: {
//@[04:0121) | | └─ObjectPropertyExpression
//@[04:0014) | |   ├─StringLiteralExpression { Value = properties }
//@[16:0121) | |   └─ObjectExpression
      size: basicParent.properties.size
//@[06:0039) | |     ├─ObjectPropertyExpression
//@[06:0010) | |     | ├─StringLiteralExpression { Value = size }
//@[12:0039) | |     | └─PropertyAccessExpression { PropertyName = size }
//@[12:0034) | |     |   └─PropertyAccessExpression { PropertyName = properties }
//@[12:0023) | |     |     └─ResourceReferenceExpression
      style: basicChild::basicGrandchild.properties.style
//@[06:0057) | |     └─ObjectPropertyExpression
//@[06:0011) | |       ├─StringLiteralExpression { Value = style }
//@[13:0057) | |       └─PropertyAccessExpression { PropertyName = style }
//@[13:0051) | |         └─PropertyAccessExpression { PropertyName = properties }
//@[13:0040) | |           └─ResourceReferenceExpression
    }
  }
}
// #completionTest(50) -> childResources
output referenceBasicChild string = basicParent::basicChild.properties.size
//@[00:0075) ├─DeclaredOutputExpression { Name = referenceBasicChild }
//@[27:0033) | ├─AmbientTypeReferenceExpression { Name = string }
//@[36:0075) | └─PropertyAccessExpression { PropertyName = size }
//@[36:0070) |   └─PropertyAccessExpression { PropertyName = properties }
//@[36:0059) |     └─ResourceReferenceExpression
// #completionTest(67) -> grandChildResources
output referenceBasicGrandchild string = basicParent::basicChild::basicGrandchild.properties.style
//@[00:0098) ├─DeclaredOutputExpression { Name = referenceBasicGrandchild }
//@[32:0038) | ├─AmbientTypeReferenceExpression { Name = string }
//@[41:0098) | └─PropertyAccessExpression { PropertyName = style }
//@[41:0092) |   └─PropertyAccessExpression { PropertyName = properties }
//@[41:0081) |     └─ResourceReferenceExpression

resource existingParent 'My.Rp/parentType@2020-12-01' existing = {
//@[00:0386) ├─DeclaredResourceExpression
//@[65:0386) | └─ObjectExpression
  name: 'existingParent'

  resource existingChild 'childType' existing = {
//@[02:0291) ├─DeclaredResourceExpression
//@[48:0291) | └─ObjectExpression
    name: 'existingChild'

    resource existingGrandchild 'grandchildType' = {
//@[04:0210) ├─DeclaredResourceExpression
//@[51:0210) | └─ObjectExpression
      name: 'existingGrandchild'
      properties: {
//@[06:0118) |   └─ObjectPropertyExpression
//@[06:0016) |     ├─StringLiteralExpression { Value = properties }
//@[18:0118) |     └─ObjectExpression
        size: existingParent.properties.size
//@[08:0044) |       ├─ObjectPropertyExpression
//@[08:0012) |       | ├─StringLiteralExpression { Value = size }
//@[14:0044) |       | └─PropertyAccessExpression { PropertyName = size }
//@[14:0039) |       |   └─PropertyAccessExpression { PropertyName = properties }
//@[14:0028) |       |     └─ResourceReferenceExpression
        style: existingChild.properties.style
//@[08:0045) |       └─ObjectPropertyExpression
//@[08:0013) |         ├─StringLiteralExpression { Value = style }
//@[15:0045) |         └─PropertyAccessExpression { PropertyName = style }
//@[15:0039) |           └─PropertyAccessExpression { PropertyName = properties }
//@[15:0028) |             └─ResourceReferenceExpression
      }
    }
  }
}

param createParent bool
//@[00:0023) ├─DeclaredParameterExpression { Name = createParent }
//@[19:0023) | └─AmbientTypeReferenceExpression { Name = bool }
param createChild bool
//@[00:0022) ├─DeclaredParameterExpression { Name = createChild }
//@[18:0022) | └─AmbientTypeReferenceExpression { Name = bool }
param createGrandchild bool
//@[00:0027) ├─DeclaredParameterExpression { Name = createGrandchild }
//@[23:0027) | └─AmbientTypeReferenceExpression { Name = bool }
resource conditionParent 'My.Rp/parentType@2020-12-01' = if (createParent) {
//@[61:0073) | | | | ├─ParametersReferenceExpression { Parameter = createParent }
//@[61:0073) | | | ├─ParametersReferenceExpression { Parameter = createParent }
//@[00:0433) ├─DeclaredResourceExpression
//@[61:0073) | └─ConditionExpression
//@[61:0073) |   ├─ParametersReferenceExpression { Parameter = createParent }
//@[75:0433) |   └─ObjectExpression
  name: 'conditionParent'

  resource conditionChild 'childType' = if (createChild) {
//@[44:0055) | | | ├─BinaryExpression { Operator = LogicalAnd }
//@[44:0055) | | | | └─ParametersReferenceExpression { Parameter = createChild }
//@[02:0327) ├─DeclaredResourceExpression
//@[44:0055) | ├─ConditionExpression
//@[44:0055) | | ├─BinaryExpression { Operator = LogicalAnd }
//@[44:0055) | | | └─ParametersReferenceExpression { Parameter = createChild }
//@[57:0327) | | └─ObjectExpression
    name: 'conditionChild'

    resource conditionGrandchild 'grandchildType' = if (createGrandchild) {
//@[04:0236) ├─DeclaredResourceExpression
//@[56:0072) | ├─ConditionExpression
//@[56:0072) | | ├─BinaryExpression { Operator = LogicalAnd }
//@[56:0072) | | | └─ParametersReferenceExpression { Parameter = createGrandchild }
//@[74:0236) | | └─ObjectExpression
      name: 'conditionGrandchild'
      properties: {
//@[06:0120) | |   └─ObjectPropertyExpression
//@[06:0016) | |     ├─StringLiteralExpression { Value = properties }
//@[18:0120) | |     └─ObjectExpression
        size: conditionParent.properties.size
//@[08:0045) | |       ├─ObjectPropertyExpression
//@[08:0012) | |       | ├─StringLiteralExpression { Value = size }
//@[14:0045) | |       | └─PropertyAccessExpression { PropertyName = size }
//@[14:0040) | |       |   └─PropertyAccessExpression { PropertyName = properties }
//@[14:0029) | |       |     └─ResourceReferenceExpression
        style: conditionChild.properties.style
//@[08:0046) | |       └─ObjectPropertyExpression
//@[08:0013) | |         ├─StringLiteralExpression { Value = style }
//@[15:0046) | |         └─PropertyAccessExpression { PropertyName = style }
//@[15:0040) | |           └─PropertyAccessExpression { PropertyName = properties }
//@[15:0029) | |             └─ResourceReferenceExpression
      }
    }
  }
}

var items = [
//@[00:0027) ├─DeclaredVariableExpression { Name = items }
//@[12:0027) | └─ArrayExpression
  'a'
//@[02:0005) |   ├─StringLiteralExpression { Value = a }
  'b'
//@[02:0005) |   └─StringLiteralExpression { Value = b }
]
resource loopParent 'My.Rp/parentType@2020-12-01' = {
//@[00:0161) ├─DeclaredResourceExpression
//@[52:0161) | └─ObjectExpression
  name: 'loopParent'

  resource loopChild 'childType' = [for item in items: {
//@[02:0083) ├─DeclaredResourceExpression
//@[35:0083) | ├─ForLoopExpression
//@[48:0053) | | ├─VariableReferenceExpression { Variable = items }
//@[55:0082) | | └─ObjectExpression
    name: 'loopChild'
  }]
}

output loopChildOutput string = loopParent::loopChild[0].name
//@[00:0061) └─DeclaredOutputExpression { Name = loopChildOutput }
//@[23:0029)   ├─AmbientTypeReferenceExpression { Name = string }
//@[32:0061)   └─PropertyAccessExpression { PropertyName = name }
//@[32:0056)     └─ResourceReferenceExpression
