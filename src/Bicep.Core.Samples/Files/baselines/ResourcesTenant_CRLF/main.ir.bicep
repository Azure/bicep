targetScope = 'tenant'
//@[000:1350) ProgramExpression
//@[000:0000) | └─ResourceDependencyExpression [UNPARENTED]
//@[000:0000) |   └─ResourceReferenceExpression [UNPARENTED]
//@[000:0000) | ├─ResourceDependencyExpression [UNPARENTED]
//@[000:0000) | | └─ResourceReferenceExpression [UNPARENTED]
//@[000:0000) | └─ResourceDependencyExpression [UNPARENTED]
//@[000:0000) |   └─ResourceReferenceExpression [UNPARENTED]
//@[000:0000) | ├─ResourceDependencyExpression [UNPARENTED]
//@[000:0000) | | └─ResourceReferenceExpression [UNPARENTED]
//@[000:0000) | └─ResourceDependencyExpression [UNPARENTED]
//@[000:0000) |   └─ResourceReferenceExpression [UNPARENTED]

var managementGroups = [
//@[000:0142) ├─DeclaredVariableExpression { Name = managementGroups }
//@[023:0142) | └─ArrayExpression
  {
//@[002:0055) |   ├─ObjectExpression
    name: 'one'
//@[004:0015) |   | ├─ObjectPropertyExpression
//@[004:0008) |   | | ├─StringLiteralExpression { Value = name }
//@[010:0015) |   | | └─StringLiteralExpression { Value = one }
    displayName: 'The first'
//@[004:0028) |   | └─ObjectPropertyExpression
//@[004:0015) |   |   ├─StringLiteralExpression { Value = displayName }
//@[017:0028) |   |   └─StringLiteralExpression { Value = The first }
  }
  {
//@[002:0056) |   └─ObjectExpression
    name: 'two'
//@[004:0015) |     ├─ObjectPropertyExpression
//@[004:0008) |     | ├─StringLiteralExpression { Value = name }
//@[010:0015) |     | └─StringLiteralExpression { Value = two }
    displayName: 'The second'
//@[004:0029) |     └─ObjectPropertyExpression
//@[004:0015) |       ├─StringLiteralExpression { Value = displayName }
//@[017:0029) |       └─StringLiteralExpression { Value = The second }
  }
]

resource singleGroup 'Microsoft.Management/managementGroups@2020-05-01' = {
//@[000:0154) ├─DeclaredResourceExpression
//@[074:0154) | └─ObjectExpression
  name: 'myMG'
  properties: {
//@[002:0058) |   └─ObjectPropertyExpression
//@[002:0012) |     ├─StringLiteralExpression { Value = properties }
//@[014:0058) |     └─ObjectExpression
    displayName: 'This one is mine!'
//@[004:0036) |       └─ObjectPropertyExpression
//@[004:0015) |         ├─StringLiteralExpression { Value = displayName }
//@[017:0036) |         └─StringLiteralExpression { Value = This one is mine! }
  }
}

resource manyGroups 'Microsoft.Management/managementGroups@2020-05-01' = [for mg in managementGroups: {
//@[000:0224) ├─DeclaredResourceExpression
//@[073:0224) | ├─ForLoopExpression
//@[084:0100) | | ├─VariableReferenceExpression { Variable = managementGroups }
//@[102:0223) | | └─ObjectExpression
//@[084:0100) | |           |   └─VariableReferenceExpression { Variable = managementGroups }
  name: mg.name
  properties: {
//@[002:0098) | |   └─ObjectPropertyExpression
//@[002:0012) | |     ├─StringLiteralExpression { Value = properties }
//@[014:0098) | |     └─ObjectExpression
    displayName: '${mg.displayName} (${singleGroup.properties.displayName})'
//@[004:0076) | |       └─ObjectPropertyExpression
//@[004:0015) | |         ├─StringLiteralExpression { Value = displayName }
//@[017:0076) | |         └─InterpolatedStringExpression
//@[020:0034) | |           ├─PropertyAccessExpression { PropertyName = displayName }
//@[020:0022) | |           | └─ArrayAccessExpression
//@[020:0022) | |           |   ├─CopyIndexExpression
//@[039:0073) | |           └─PropertyAccessExpression { PropertyName = displayName }
//@[039:0061) | |             └─PropertyAccessExpression { PropertyName = properties }
//@[039:0050) | |               └─ResourceReferenceExpression
  }
}]

resource anotherSet 'Microsoft.Management/managementGroups@2020-05-01' = [for (mg, index) in managementGroups: {
//@[000:0319) ├─DeclaredResourceExpression
//@[073:0319) | ├─ForLoopExpression
//@[093:0109) | | ├─VariableReferenceExpression { Variable = managementGroups }
//@[111:0318) | | └─ObjectExpression
//@[093:0109) | |           |   └─VariableReferenceExpression { Variable = managementGroups }
  name: concat(mg.name, '-one-', index)
  properties: {
//@[002:0123) | |   └─ObjectPropertyExpression
//@[002:0012) | |     ├─StringLiteralExpression { Value = properties }
//@[014:0123) | |     └─ObjectExpression
    displayName: '${mg.displayName} (${singleGroup.properties.displayName}) (set 1) (index ${index})'
//@[004:0101) | |       └─ObjectPropertyExpression
//@[004:0015) | |         ├─StringLiteralExpression { Value = displayName }
//@[017:0101) | |         └─InterpolatedStringExpression
//@[020:0034) | |           ├─PropertyAccessExpression { PropertyName = displayName }
//@[020:0022) | |           | └─ArrayAccessExpression
//@[020:0022) | |           |   ├─CopyIndexExpression
//@[039:0073) | |           ├─PropertyAccessExpression { PropertyName = displayName }
//@[039:0061) | |           | └─PropertyAccessExpression { PropertyName = properties }
//@[039:0050) | |           |   └─ResourceReferenceExpression
//@[093:0098) | |           └─CopyIndexExpression
  }
  dependsOn: [
    manyGroups
  ]
}]

resource yetAnotherSet 'Microsoft.Management/managementGroups@2020-05-01' = [for mg in managementGroups: {
//@[000:0291) ├─DeclaredResourceExpression
//@[076:0291) | ├─ForLoopExpression
//@[087:0103) | | ├─VariableReferenceExpression { Variable = managementGroups }
//@[105:0290) | | └─ObjectExpression
//@[087:0103) | |           |   └─VariableReferenceExpression { Variable = managementGroups }
  name: concat(mg.name, '-two')
  properties: {
//@[002:0106) | |   └─ObjectPropertyExpression
//@[002:0012) | |     ├─StringLiteralExpression { Value = properties }
//@[014:0106) | |     └─ObjectExpression
    displayName: '${mg.displayName} (${singleGroup.properties.displayName}) (set 2)'
//@[004:0084) | |       └─ObjectPropertyExpression
//@[004:0015) | |         ├─StringLiteralExpression { Value = displayName }
//@[017:0084) | |         └─InterpolatedStringExpression
//@[020:0034) | |           ├─PropertyAccessExpression { PropertyName = displayName }
//@[020:0022) | |           | └─ArrayAccessExpression
//@[020:0022) | |           |   ├─CopyIndexExpression
//@[039:0073) | |           └─PropertyAccessExpression { PropertyName = displayName }
//@[039:0061) | |             └─PropertyAccessExpression { PropertyName = properties }
//@[039:0050) | |               └─ResourceReferenceExpression
  }
  dependsOn: [
    anotherSet[0]
  ]
}]

output managementGroupIds array = [for i in range(0, length(managementGroups)): {
//@[000:0172) └─DeclaredOutputExpression { Name = managementGroupIds }
//@[026:0031)   ├─AmbientTypeReferenceExpression { Name = array }
//@[034:0172)   └─ForLoopExpression
//@[044:0078)     ├─FunctionCallExpression { Name = range }
//@[050:0051)     | ├─IntegerLiteralExpression { Value = 0 }
//@[053:0077)     | └─FunctionCallExpression { Name = length }
//@[060:0076)     |   └─VariableReferenceExpression { Variable = managementGroups }
//@[080:0171)     └─ObjectExpression
  name: yetAnotherSet[i].name
//@[002:0029)       ├─ObjectPropertyExpression
//@[002:0006)       | ├─StringLiteralExpression { Value = name }
//@[008:0029)       | └─PropertyAccessExpression { PropertyName = name }
//@[008:0024)       |   └─ResourceReferenceExpression
  displayName: yetAnotherSet[i].properties.displayName
//@[002:0054)       └─ObjectPropertyExpression
//@[002:0013)         ├─StringLiteralExpression { Value = displayName }
//@[015:0054)         └─PropertyAccessExpression { PropertyName = displayName }
//@[015:0042)           └─PropertyAccessExpression { PropertyName = properties }
//@[015:0031)             └─ResourceReferenceExpression
}]

