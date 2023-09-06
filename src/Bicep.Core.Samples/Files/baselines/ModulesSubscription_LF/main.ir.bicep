targetScope = 'subscription'
//@[00:705) ProgramExpression
//@[00:000) | └─ResourceDependencyExpression [UNPARENTED]
//@[00:000) |   └─ResourceReferenceExpression [UNPARENTED]

param prefix string = 'majastrz'
//@[00:032) ├─DeclaredParameterExpression { Name = prefix }
//@[13:019) | ├─AmbientTypeReferenceExpression { Name = string }
//@[22:032) | └─StringLiteralExpression { Value = majastrz }
var groups = [
//@[00:060) ├─DeclaredVariableExpression { Name = groups }
//@[13:060) | └─ArrayExpression
  'bicep1'
//@[02:010) |   ├─StringLiteralExpression { Value = bicep1 }
  'bicep2'
//@[02:010) |   ├─StringLiteralExpression { Value = bicep2 }
  'bicep3'
//@[02:010) |   ├─StringLiteralExpression { Value = bicep3 }
  'bicep4'
//@[02:010) |   └─StringLiteralExpression { Value = bicep4 }
]

var scripts = take(groups, 2)
//@[00:029) ├─DeclaredVariableExpression { Name = scripts }
//@[14:029) | └─FunctionCallExpression { Name = take }
//@[19:025) |   ├─VariableReferenceExpression { Variable = groups }
//@[27:028) |   └─IntegerLiteralExpression { Value = 2 }

resource resourceGroups 'Microsoft.Resources/resourceGroups@2020-06-01' = [for name in groups: {
//@[00:148) ├─DeclaredResourceExpression
//@[74:148) | └─ForLoopExpression
//@[87:093) |   ├─VariableReferenceExpression { Variable = groups }
//@[95:147) |   └─ObjectExpression
  name: '${prefix}-${name}'
  location: 'westus'
//@[02:020) |     └─ObjectPropertyExpression
//@[02:010) |       ├─StringLiteralExpression { Value = location }
//@[12:020) |       └─StringLiteralExpression { Value = westus }
}]

module scopedToSymbolicName 'hello.bicep' = [for (name, i) in scripts: {
//@[00:183) ├─DeclaredModuleExpression
//@[44:183) | ├─ForLoopExpression
//@[62:069) | | ├─VariableReferenceExpression { Variable = scripts }
//@[71:182) | | └─ObjectExpression
//@[62:069) | |     | └─VariableReferenceExpression { Variable = scripts }
  name: '${prefix}-dep-${i}'
//@[02:028) | |   └─ObjectPropertyExpression
//@[02:006) | |     ├─StringLiteralExpression { Value = name }
//@[08:028) | |     └─InterpolatedStringExpression
//@[11:017) | |       ├─ParametersReferenceExpression { Parameter = prefix }
//@[25:026) | |       └─CopyIndexExpression
  params: {
//@[10:051) | ├─ObjectExpression
    scriptName: 'test-${name}-${i}'
//@[04:035) | | └─ObjectPropertyExpression
//@[04:014) | |   ├─StringLiteralExpression { Value = scriptName }
//@[16:035) | |   └─InterpolatedStringExpression
//@[24:028) | |     ├─ArrayAccessExpression
//@[24:028) | |     | ├─CopyIndexExpression
//@[32:033) | |     └─CopyIndexExpression
  }
  scope: resourceGroups[i]
}]

module scopedToResourceGroupFunction 'hello.bicep' = [for (name, i) in scripts: {
//@[00:212) └─DeclaredModuleExpression
//@[53:212)   ├─ForLoopExpression
//@[71:078)   | ├─VariableReferenceExpression { Variable = scripts }
//@[80:211)   | └─ObjectExpression
//@[71:078)         | └─VariableReferenceExpression { Variable = scripts }
  name: '${prefix}-dep-${i}'
//@[02:028)   |   └─ObjectPropertyExpression
//@[02:006)   |     ├─StringLiteralExpression { Value = name }
//@[08:028)   |     └─InterpolatedStringExpression
//@[11:017)   |       ├─ParametersReferenceExpression { Parameter = prefix }
//@[25:026)   |       └─CopyIndexExpression
  params: {
//@[10:051)   └─ObjectExpression
    scriptName: 'test-${name}-${i}'
//@[04:035)     └─ObjectPropertyExpression
//@[04:014)       ├─StringLiteralExpression { Value = scriptName }
//@[16:035)       └─InterpolatedStringExpression
//@[24:028)         ├─ArrayAccessExpression
//@[24:028)         | ├─CopyIndexExpression
//@[32:033)         └─CopyIndexExpression
  }
  scope: resourceGroup(concat(name, '-extra'))
}]


