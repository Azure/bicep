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
resource existingFoo 'Microsoft.Storage/storageAccounts@2022-09-01' existing = {
  name: 'existingFoo'
}

param intParam int = 0
param strParam string = 'id'
param strParam2 string = 'd'
param cond bool = false

var zeroIndex = 0
var otherIndex = zeroIndex + 2
var idAccessor = 'id'
var dStr = 'd'
var idAccessor2 = idAccessor
var idAccessorInterpolated = '${idAccessor}'
var idAccessorMixed = 'i${dStr}'
var propertiesAccessor = 'properties'
var accessTierAccessor = 'accessTier'
var strArray = ['id', 'properties']
var intArray = [0, 1]

var varForBodyInvalidRuntimeUsages = [
  for i in range(0, 2): {
    case1: foo
    case2: existingFoo
    case3: foo::fooChild
    case4: foos[0]
    case5: foos[i]
    case6: foos[i + 2]
    case7: foos[zeroIndex]
    case8: foos[otherIndex]
    case9: foo.properties
    case10: existingFoo.properties
    case11: foo::fooChild.properties
    case12: foos[0].properties
    case13: foos[i].properties
    case14: foos[i + 2].properties
    case15: foos[zeroIndex].properties
    case16: foos[otherIndex].properties
    case17: foo.properties.accessTier
    case18: existingFoo.properties.accessTier
    case19: foo::fooChild.properties.accessTier
    case20: foos[0].properties.accessTier
    case21: foos[i].properties.accessTier
    case22: foos[i + 2].properties.accessTier
    case23: foos[zeroIndex].properties.accessTier
    case24: foos[otherIndex].properties.accessTier
    case25: foo['properties']
    case26: existingFoo['properties']
    case27: foo::fooChild['properties']
    case28: foos[0]['properties']
    case29: foos[i]['properties']
    case30: foos[i + 2]['properties']
    case31: foos[zeroIndex]['properties']
    case32: foos[otherIndex]['properties']
    case33: foo['properties']['accessTier']
    case34: existingFoo['properties']['accessTier']
    case35: foo::fooChild['properties']['accessTier']
    case36: foos[0]['properties']['accessTier']
    case37: foos[i]['properties']['accessTier']
    case38: foos[i + 2]['properties']['accessTier']
    case39: foos[zeroIndex]['properties']['accessTier']
    case40: foos[otherIndex]['properties']['accessTier']
    case41: foo[propertiesAccessor]
    case42: existingFoo[propertiesAccessor]
    case43: foo::fooChild[propertiesAccessor]
    case44: foos[0][propertiesAccessor]
    case45: foos[i][propertiesAccessor]
    case46: foos[i + 2][propertiesAccessor]
    case47: foos[zeroIndex][propertiesAccessor]
    case48: foos[otherIndex][propertiesAccessor]
    case49: foo[propertiesAccessor][accessTierAccessor]
    case50: existingFoo[propertiesAccessor][accessTierAccessor]
    case51: foo::fooChild[propertiesAccessor][accessTierAccessor]
    case52: foos[0][propertiesAccessor][accessTierAccessor]
    case53: foos[i][propertiesAccessor][accessTierAccessor]
    case54: foos[i + 2][propertiesAccessor][accessTierAccessor]
    case55: foos[zeroIndex][propertiesAccessor][accessTierAccessor]
    case56: foos[otherIndex][propertiesAccessor][accessTierAccessor]
    case57: foo[strParam]
    case58: existingFoo[strParam]
    case59: foo::fooChild[strParam]
    case60: foos[0][strParam]
    case61: foos[i][strParam]
    case62: foos[i + 2][strParam]
    case63: foos[zeroIndex][strParam]
    case64: foos[otherIndex][strParam]
    case65: foo['${strParam}']
    case66: existingFoo['${strParam}']
    case67: foo::fooChild['${strParam}']
    case68: foos[0]['${strParam}']
    case69: foos[i]['${strParam}']
    case70: foos[i + 2]['${strParam}']
    case71: foos[zeroIndex]['${strParam}']
    case72: foos[otherIndex]['${strParam}']
    case73: foo['i${strParam2}']
    case74: existingFoo['i${strParam2}']
    case75: foo::fooChild['i${strParam2}']
    case76: foos[0]['i${strParam2}']
    case77: foos[i]['i${strParam2}']
    case78: foos[i + 2]['i${strParam2}']
    case79: foos[zeroIndex]['i${strParam2}']
    case80: foos[otherIndex]['i${strParam2}']
    case81: foo[strArray[1]]
    case82: existingFoo[strArray[1]]
    case83: foo::fooChild[strArray[1]]
    case84: foos[0][strArray[1]]
    case85: foos[i][strArray[1]]
    case86: foos[i + 2][strArray[1]]
    case87: foos[zeroIndex][strArray[1]]
    case88: foos[otherIndex][strArray[1]]
    case89: foo[last(strArray)]
    case90: existingFoo[last(strArray)]
    case91: foo::fooChild[last(strArray)]
    case92: foos[0][last(strArray)]
    case93: foos[i][last(strArray)]
    case94: foos[i + 2][last(strArray)]
    case95: foos[zeroIndex][last(strArray)]
    case96: foos[otherIndex][last(strArray)]
    case97: foo[cond ? 'id' : 'properties']
    case98: existingFoo[cond ? 'id' : 'properties']
    case99: foo::fooChild[cond ? 'id' : 'properties']
    case100: foos[0][cond ? 'id' : 'properties']
    case101: foos[i][cond ? 'id' : 'properties']
    case102: foos[i + 2][cond ? 'id' : 'properties']
    case103: foos[zeroIndex][cond ? 'id' : 'properties']
    case104: foos[otherIndex][cond ? 'id' : 'properties']
    case105: foo[cond ? 'id' : strParam]
    case106: existingFoo[cond ? 'id' : strParam]
    case107: foo::fooChild[cond ? 'id' : strParam]
    case108: foos[0][cond ? 'id' : strParam]
    case109: foos[i][cond ? 'id' : strParam]
    case110: foos[i + 2][cond ? 'id' : strParam]
    case111: foos[zeroIndex][cond ? 'id' : strParam]
    case112: foos[otherIndex][cond ? 'id' : strParam]
    case113: foos[cond ? 0 : 1].properties
    case114: foo[any('id')]
    case115: foos[any(0)]
    case116: foos[cond ? i : 0]
    case117: foos[cond ? i : i - 1]
    case118: foos[cond ? i + 1 : i - 1]
    case119: foos[cond ? any(0) : i]
    case120: foos[cond ? i : first(intArray)]
    case121: foos[intParam]
  }
]
var varForBodyInvalidRuntimeUsageExpression = [for i in range(0, 2): foo.properties]
var varForBodyInvalidRuntimeUsageInterpolatedKey = [
  for i in range(0, 2): {
    '${foos[i].properties.accessTier}': 'accessTier'
  }
]
