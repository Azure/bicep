resource foo 'Microsoft.Storage/storageAccounts@2022-09-01' = {
  name: 'foo'
  location: 'westus'
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'

  resource fooChild 'fileServices' = {
    name: 'default'
  }
}
resource foos 'Microsoft.Storage/storageAccounts@2022-09-01' = [
  for i in range(0, 2): {
    name: 'foo-${i}'
    location: 'westus'
    sku: {
      name: 'Standard_LRS'
    }
    kind: 'StorageV2'
  }
]
resource existingFoo 'Microsoft.Storage/storageAccounts@2022-09-01' = {
  name: 'existingFoo'
}

param cond bool = false

var zeroIndex = 0
var otherIndex = zeroIndex + 2
var idAccessor = 'id'
var dStr = 'd'
var idAccessor2 = idAccessor
var idAccessorInterpolated = '${idAccessor}'
var idAccessorMixed = 'i${dStr}'
var strArray = ['id', 'properties']

var varForBodyOkDeployTimeUsages = [
  for i in range(0, 2): {
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
    case89: foos[cond ? i : zeroIndex]['id']
    case90: foos[any(0)].id
    case91: foos[cond ? any(0) : i - 1]['id']
  }
]
var varForBodyOkDeployTimeUsageExpression = [for i in range(0, 2): foo.id]
var varForBodyOkDeployTimeUsageInterpolatedKey = [
  for i in range(0, 2): {
    '${foo.id}': 'id'
  }
]
