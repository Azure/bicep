resource foo 'Microsoft.Storage/storageAccounts@2022-09-01' = {
//@[00:4116) ProgramExpression
//@[00:0000) | └─ResourceDependencyExpression [UNPARENTED]
//@[00:0000) |   └─ResourceReferenceExpression [UNPARENTED]
//@[00:0222) ├─DeclaredResourceExpression
//@[62:0222) | └─ObjectExpression
  name: 'foo'
  location: 'westus'
//@[02:0020) |   ├─ObjectPropertyExpression
//@[02:0010) |   | ├─StringLiteralExpression { Value = location }
//@[12:0020) |   | └─StringLiteralExpression { Value = westus }
  sku: {
//@[02:0037) |   ├─ObjectPropertyExpression
//@[02:0005) |   | ├─StringLiteralExpression { Value = sku }
//@[07:0037) |   | └─ObjectExpression
    name: 'Standard_LRS'
//@[04:0024) |   |   └─ObjectPropertyExpression
//@[04:0008) |   |     ├─StringLiteralExpression { Value = name }
//@[10:0024) |   |     └─StringLiteralExpression { Value = Standard_LRS }
  }
  kind: 'StorageV2'
//@[02:0019) |   └─ObjectPropertyExpression
//@[02:0006) |     ├─StringLiteralExpression { Value = kind }
//@[08:0019) |     └─StringLiteralExpression { Value = StorageV2 }

  resource fooChild 'fileServices' = {
//@[02:0062) ├─DeclaredResourceExpression
//@[37:0062) | ├─ObjectExpression
    name: 'default'
  }
}
resource foos 'Microsoft.Storage/storageAccounts@2022-09-01' = [for i in range(0, 2): {
//@[00:0188) ├─DeclaredResourceExpression
//@[63:0188) | └─ForLoopExpression
//@[73:0084) |   ├─FunctionCallExpression { Name = range }
//@[79:0080) |   | ├─IntegerLiteralExpression { Value = 0 }
//@[82:0083) |   | └─IntegerLiteralExpression { Value = 2 }
//@[86:0187) |   └─ObjectExpression
  name: 'foo-${i}'
  location: 'westus'
//@[02:0020) |     ├─ObjectPropertyExpression
//@[02:0010) |     | ├─StringLiteralExpression { Value = location }
//@[12:0020) |     | └─StringLiteralExpression { Value = westus }
  sku: {
//@[02:0037) |     ├─ObjectPropertyExpression
//@[02:0005) |     | ├─StringLiteralExpression { Value = sku }
//@[07:0037) |     | └─ObjectExpression
    name: 'Standard_LRS'
//@[04:0024) |     |   └─ObjectPropertyExpression
//@[04:0008) |     |     ├─StringLiteralExpression { Value = name }
//@[10:0024) |     |     └─StringLiteralExpression { Value = Standard_LRS }
  }
  kind: 'StorageV2'
//@[02:0019) |     └─ObjectPropertyExpression
//@[02:0006) |       ├─StringLiteralExpression { Value = kind }
//@[08:0019) |       └─StringLiteralExpression { Value = StorageV2 }
}]
resource existingFoo 'Microsoft.Storage/storageAccounts@2022-09-01' existing = {
//@[00:0104) └─DeclaredResourceExpression
//@[79:0104)   └─ObjectExpression
  name: 'existingFoo'
}

param cond bool = false
//@[00:0023) ├─DeclaredParameterExpression { Name = cond }
//@[18:0023) | └─BooleanLiteralExpression { Value = False }

var zeroIndex = 0
//@[00:0017) ├─DeclaredVariableExpression { Name = zeroIndex }
//@[16:0017) | └─IntegerLiteralExpression { Value = 0 }
var otherIndex = zeroIndex + 2
//@[00:0030) ├─DeclaredVariableExpression { Name = otherIndex }
//@[17:0030) | └─BinaryExpression { Operator = Add }
//@[17:0026) |   ├─VariableReferenceExpression { Variable = zeroIndex }
//@[29:0030) |   └─IntegerLiteralExpression { Value = 2 }
var idAccessor = 'id'
//@[00:0021) ├─DeclaredVariableExpression { Name = idAccessor }
//@[17:0021) | └─StringLiteralExpression { Value = id }
var dStr = 'd'
//@[00:0014) ├─DeclaredVariableExpression { Name = dStr }
//@[11:0014) | └─StringLiteralExpression { Value = d }
var idAccessor2 = idAccessor
//@[00:0028) ├─DeclaredVariableExpression { Name = idAccessor2 }
//@[18:0028) | └─VariableReferenceExpression { Variable = idAccessor }
var idAccessorInterpolated = '${idAccessor}'
//@[00:0044) ├─DeclaredVariableExpression { Name = idAccessorInterpolated }
//@[29:0044) | └─InterpolatedStringExpression
//@[32:0042) |   └─VariableReferenceExpression { Variable = idAccessor }
var idAccessorMixed = 'i${dStr}'
//@[00:0032) ├─DeclaredVariableExpression { Name = idAccessorMixed }
//@[22:0032) | └─InterpolatedStringExpression
//@[26:0030) |   └─VariableReferenceExpression { Variable = dStr }
var strArray = ['id', 'properties']
//@[00:0035) ├─DeclaredVariableExpression { Name = strArray }
//@[15:0035) | └─ArrayExpression
//@[16:0020) |   ├─StringLiteralExpression { Value = id }
//@[22:0034) |   └─StringLiteralExpression { Value = properties }

var varForBodyOkDeployTimeUsages = [for i in range(0, 2): {
  case1: foo.id
  case2: existingFoo.id
  case3: foo::fooChild.id
  case4: foos[0].id
  case5: foos[i].id
  case6: foos[i + 2].id
  case7: foos[zeroIndex].id
  case8: foos[otherIndex].id
  case9: foo['id']
  case10: existingFoo['id']
  case11: foo::fooChild['id']
  case12: foos[0]['id']
  case13: foos[i]['id']
  case14: foos[i + 2]['id']
  case15: foos[zeroIndex]['id']
  case16: foos[otherIndex]['id']
  case17: foo[idAccessor]
  case18: existingFoo[idAccessor]
  case19: foo::fooChild[idAccessor]
  case20: foos[0][idAccessor]
  case21: foos[i][idAccessor]
  case22: foos[i + 2][idAccessor]
  case23: foos[zeroIndex][idAccessor]
  case24: foos[otherIndex][idAccessor]
  case25: foo[idAccessor2]
  case26: existingFoo[idAccessor2]
  case27: foo::fooChild[idAccessor2]
  case28: foos[0][idAccessor2]
  case29: foos[i][idAccessor2]
  case30: foos[i + 2][idAccessor2]
  case31: foos[zeroIndex][idAccessor2]
  case32: foos[otherIndex][idAccessor2]
  case33: foo['${'id'}']
  case34: existingFoo['${'id'}']
  case35: foo::fooChild['${'id'}']
  case36: foos[0]['${'id'}']
  case37: foos[i]['${'id'}']
  case38: foos[i + 2]['${'id'}']
  case39: foos[zeroIndex]['${'id'}']
  case40: foos[otherIndex]['${'id'}']
  case41: foo[idAccessorInterpolated]
  case42: existingFoo[idAccessorInterpolated]
  case43: foo::fooChild[idAccessorInterpolated]
  case44: foos[0][idAccessorInterpolated]
  case45: foos[i][idAccessorInterpolated]
  case46: foos[i + 2][idAccessorInterpolated]
  case47: foos[zeroIndex][idAccessorInterpolated]
  case48: foos[otherIndex][idAccessorInterpolated]
  case49: foo[idAccessorMixed]
  case50: existingFoo[idAccessorMixed]
  case51: foo::fooChild[idAccessorMixed]
  case52: foos[0][idAccessorMixed]
  case53: foos[i][idAccessorMixed]
  case54: foos[i + 2][idAccessorMixed]
  case55: foos[zeroIndex][idAccessorMixed]
  case56: foos[otherIndex][idAccessorMixed]
  case57: foo[strArray[0]]
  case58: existingFoo[strArray[0]]
  case59: foo::fooChild[strArray[0]]
  case60: foos[0][strArray[0]]
  case61: foos[i][strArray[0]]
  case62: foos[i + 2][strArray[0]]
  case63: foos[zeroIndex][strArray[0]]
  case64: foos[otherIndex][strArray[0]]
  case65: foo[first(strArray)]
  case66: existingFoo[first(strArray)]
  case67: foo::fooChild[first(strArray)]
  case68: foos[0][first(strArray)]
  case69: foos[i][first(strArray)]
  case70: foos[i + 2][first(strArray)]
  case71: foos[zeroIndex][first(strArray)]
  case72: foos[otherIndex][first(strArray)]
  case73: foo[cond ? 'id' : 'name']
  case74: existingFoo[cond ? 'id' : 'name']
  case75: foo::fooChild[cond ? 'id' : 'name']
  case76: foos[0][cond ? 'id' : 'name']
  case77: foos[i][cond ? 'id' : 'name']
  case78: foos[i + 2][cond ? 'id' : 'name']
  case79: foos[zeroIndex][cond ? 'id' : 'name']
  case80: foos[otherIndex][cond ? 'id' : 'name']
  case81: foo[cond ? first(strArray) : strArray[0]]
  case82: existingFoo[cond ? first(strArray) : strArray[0]]
  case83: foo::fooChild[cond ? first(strArray) : strArray[0]]
  case84: foos[0][cond ? first(strArray) : strArray[0]]
  case85: foos[i][cond ? first(strArray) : strArray[0]]
  case86: foos[i + 2][cond ? first(strArray) : strArray[0]]
  case87: foos[zeroIndex][cond ? first(strArray) : strArray[0]]
  case88: foos[otherIndex][cond ? first(strArray) : strArray[0]]
}]

