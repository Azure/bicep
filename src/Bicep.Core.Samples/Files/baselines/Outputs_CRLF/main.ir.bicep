
//@[00:1589) ProgramExpression
@sys.description('string output description')
//@[00:0076) ├─DeclaredOutputExpression { Name = myStr }
//@[17:0044) | ├─StringLiteralExpression { Value = string output description }
output myStr string = 'hello'
//@[13:0019) | ├─AmbientTypeReferenceExpression { Name = string }
//@[22:0029) | └─StringLiteralExpression { Value = hello }

@sys.description('int output description')
//@[00:0064) ├─DeclaredOutputExpression { Name = myInt }
//@[17:0041) | ├─StringLiteralExpression { Value = int output description }
output myInt int = 7
//@[13:0016) | ├─AmbientTypeReferenceExpression { Name = int }
//@[19:0020) | └─IntegerLiteralExpression { Value = 7 }
output myOtherInt int = 20 / 13 + 80 % -4
//@[00:0041) ├─DeclaredOutputExpression { Name = myOtherInt }
//@[18:0021) | ├─AmbientTypeReferenceExpression { Name = int }
//@[24:0041) | └─BinaryExpression { Operator = Add }
//@[24:0031) |   ├─BinaryExpression { Operator = Divide }
//@[24:0026) |   | ├─IntegerLiteralExpression { Value = 20 }
//@[29:0031) |   | └─IntegerLiteralExpression { Value = 13 }
//@[34:0041) |   └─BinaryExpression { Operator = Modulo }
//@[34:0036) |     ├─IntegerLiteralExpression { Value = 80 }
//@[39:0041) |     └─IntegerLiteralExpression { Value = -4 }

@sys.description('bool output description')
//@[00:0072) ├─DeclaredOutputExpression { Name = myBool }
//@[17:0042) | ├─StringLiteralExpression { Value = bool output description }
output myBool bool = !false
//@[14:0018) | ├─AmbientTypeReferenceExpression { Name = bool }
//@[21:0027) | └─UnaryExpression { Operator = Not }
//@[22:0027) |   └─BooleanLiteralExpression { Value = False }
output myOtherBool bool = true
//@[00:0030) ├─DeclaredOutputExpression { Name = myOtherBool }
//@[19:0023) | ├─AmbientTypeReferenceExpression { Name = bool }
//@[26:0030) | └─BooleanLiteralExpression { Value = True }

@sys.description('object array description')
//@[00:0075) ├─DeclaredOutputExpression { Name = suchEmpty }
//@[17:0043) | ├─StringLiteralExpression { Value = object array description }
output suchEmpty array = [
//@[17:0022) | ├─AmbientTypeReferenceExpression { Name = array }
//@[25:0029) | └─ArrayExpression
]

output suchEmpty2 object = {
//@[00:0031) ├─DeclaredOutputExpression { Name = suchEmpty2 }
//@[18:0024) | ├─AmbientTypeReferenceExpression { Name = object }
//@[27:0031) | └─ObjectExpression
}

@sys.description('object output description')
//@[00:0225) ├─DeclaredOutputExpression { Name = obj }
//@[17:0044) | ├─StringLiteralExpression { Value = object output description }
output obj object = {
//@[11:0017) | ├─AmbientTypeReferenceExpression { Name = object }
//@[20:0178) | └─ObjectExpression
  a: 'a'
//@[02:0008) |   ├─ObjectPropertyExpression
//@[02:0003) |   | ├─StringLiteralExpression { Value = a }
//@[05:0008) |   | └─StringLiteralExpression { Value = a }
  b: 12
//@[02:0007) |   ├─ObjectPropertyExpression
//@[02:0003) |   | ├─StringLiteralExpression { Value = b }
//@[05:0007) |   | └─IntegerLiteralExpression { Value = 12 }
  c: true
//@[02:0009) |   ├─ObjectPropertyExpression
//@[02:0003) |   | ├─StringLiteralExpression { Value = c }
//@[05:0009) |   | └─BooleanLiteralExpression { Value = True }
  d: null
//@[02:0009) |   ├─ObjectPropertyExpression
//@[02:0003) |   | ├─StringLiteralExpression { Value = d }
//@[05:0009) |   | └─NullLiteralExpression
  list: [
//@[02:0059) |   ├─ObjectPropertyExpression
//@[02:0006) |   | ├─StringLiteralExpression { Value = list }
//@[08:0059) |   | └─ArrayExpression
    1
//@[04:0005) |   |   ├─IntegerLiteralExpression { Value = 1 }
    2
//@[04:0005) |   |   ├─IntegerLiteralExpression { Value = 2 }
    3
//@[04:0005) |   |   ├─IntegerLiteralExpression { Value = 3 }
    null
//@[04:0008) |   |   ├─NullLiteralExpression
    {
//@[04:0012) |   |   └─ObjectExpression
    }
  ]
  obj: {
//@[02:0050) |   └─ObjectPropertyExpression
//@[02:0005) |     ├─StringLiteralExpression { Value = obj }
//@[07:0050) |     └─ObjectExpression
    nested: [
//@[04:0035) |       └─ObjectPropertyExpression
//@[04:0010) |         ├─StringLiteralExpression { Value = nested }
//@[12:0035) |         └─ArrayExpression
      'hello'
//@[06:0013) |           └─StringLiteralExpression { Value = hello }
    ]
  }
}

output myArr array = [
//@[00:0074) ├─DeclaredOutputExpression { Name = myArr }
//@[13:0018) | ├─AmbientTypeReferenceExpression { Name = array }
//@[21:0074) | └─ArrayExpression
  'pirates'
//@[02:0011) |   ├─StringLiteralExpression { Value = pirates }
  'say'
//@[02:0007) |   ├─StringLiteralExpression { Value = say }
   false ? 'arr2' : 'arr'
//@[03:0025) |   └─TernaryExpression
//@[03:0008) |     ├─BooleanLiteralExpression { Value = False }
//@[11:0017) |     ├─StringLiteralExpression { Value = arr2 }
//@[20:0025) |     └─StringLiteralExpression { Value = arr }
]

output rgLocation string = resourceGroup().location
//@[00:0051) ├─DeclaredOutputExpression { Name = rgLocation }
//@[18:0024) | ├─AmbientTypeReferenceExpression { Name = string }
//@[27:0051) | └─PropertyAccessExpression { PropertyName = location }
//@[27:0042) |   └─FunctionCallExpression { Name = resourceGroup }

output isWestUs bool = resourceGroup().location != 'westus' ? false : true
//@[00:0074) ├─DeclaredOutputExpression { Name = isWestUs }
//@[16:0020) | ├─AmbientTypeReferenceExpression { Name = bool }
//@[23:0074) | └─TernaryExpression
//@[23:0059) |   ├─BinaryExpression { Operator = NotEquals }
//@[23:0047) |   | ├─PropertyAccessExpression { PropertyName = location }
//@[23:0038) |   | | └─FunctionCallExpression { Name = resourceGroup }
//@[51:0059) |   | └─StringLiteralExpression { Value = westus }
//@[62:0067) |   ├─BooleanLiteralExpression { Value = False }
//@[70:0074) |   └─BooleanLiteralExpression { Value = True }

output expressionBasedIndexer string = {
//@[00:0140) ├─DeclaredOutputExpression { Name = expressionBasedIndexer }
//@[30:0036) | ├─AmbientTypeReferenceExpression { Name = string }
//@[39:0140) | └─PropertyAccessExpression { PropertyName = foo }
//@[39:0136) |   └─ArrayAccessExpression
//@[39:0110) |     └─ObjectExpression
  eastus: {
//@[02:0031) |       ├─ObjectPropertyExpression
//@[02:0008) |       | ├─StringLiteralExpression { Value = eastus }
//@[10:0031) |       | └─ObjectExpression
    foo: true
//@[04:0013) |       |   └─ObjectPropertyExpression
//@[04:0007) |       |     ├─StringLiteralExpression { Value = foo }
//@[09:0013) |       |     └─BooleanLiteralExpression { Value = True }
  }
  westus: {
//@[02:0032) |       └─ObjectPropertyExpression
//@[02:0008) |         ├─StringLiteralExpression { Value = westus }
//@[10:0032) |         └─ObjectExpression
    foo: false
//@[04:0014) |           └─ObjectPropertyExpression
//@[04:0007) |             ├─StringLiteralExpression { Value = foo }
//@[09:0014) |             └─BooleanLiteralExpression { Value = False }
  }
}[resourceGroup().location].foo
//@[02:0026) |     ├─PropertyAccessExpression { PropertyName = location }
//@[02:0017) |     | └─FunctionCallExpression { Name = resourceGroup }

var secondaryKeyIntermediateVar = listKeys(resourceId('Mock.RP/type', 'steve'), '2020-01-01').secondaryKey
//@[34:0106) | └─PropertyAccessExpression { PropertyName = secondaryKey }
//@[34:0093) |   └─FunctionCallExpression { Name = listKeys }
//@[43:0078) |     ├─FunctionCallExpression { Name = resourceId }
//@[54:0068) |     | ├─StringLiteralExpression { Value = Mock.RP/type }
//@[70:0077) |     | └─StringLiteralExpression { Value = steve }
//@[80:0092) |     └─StringLiteralExpression { Value = 2020-01-01 }

output primaryKey string = listKeys(resourceId('Mock.RP/type', 'nigel'), '2020-01-01').primaryKey
//@[00:0097) ├─DeclaredOutputExpression { Name = primaryKey }
//@[18:0024) | ├─AmbientTypeReferenceExpression { Name = string }
//@[27:0097) | └─PropertyAccessExpression { PropertyName = primaryKey }
//@[27:0086) |   └─FunctionCallExpression { Name = listKeys }
//@[36:0071) |     ├─FunctionCallExpression { Name = resourceId }
//@[47:0061) |     | ├─StringLiteralExpression { Value = Mock.RP/type }
//@[63:0070) |     | └─StringLiteralExpression { Value = nigel }
//@[73:0085) |     └─StringLiteralExpression { Value = 2020-01-01 }
output secondaryKey string = secondaryKeyIntermediateVar
//@[00:0056) ├─DeclaredOutputExpression { Name = secondaryKey }
//@[20:0026) | ├─AmbientTypeReferenceExpression { Name = string }

var varWithOverlappingOutput = 'hello'
//@[00:0038) ├─DeclaredVariableExpression { Name = varWithOverlappingOutput }
//@[31:0038) | └─StringLiteralExpression { Value = hello }
param paramWithOverlappingOutput string
//@[00:0039) ├─DeclaredParameterExpression { Name = paramWithOverlappingOutput }
//@[33:0039) | └─AmbientTypeReferenceExpression { Name = string }

output varWithOverlappingOutput string = varWithOverlappingOutput
//@[00:0065) ├─DeclaredOutputExpression { Name = varWithOverlappingOutput }
//@[32:0038) | ├─AmbientTypeReferenceExpression { Name = string }
//@[41:0065) | └─VariableReferenceExpression { Variable = varWithOverlappingOutput }
output paramWithOverlappingOutput string = paramWithOverlappingOutput
//@[00:0069) ├─DeclaredOutputExpression { Name = paramWithOverlappingOutput }
//@[34:0040) | ├─AmbientTypeReferenceExpression { Name = string }
//@[43:0069) | └─ParametersReferenceExpression { Parameter = paramWithOverlappingOutput }

// top-level output loops are supported
output generatedArray array = [for i in range(0,10): i]
//@[00:0055) └─DeclaredOutputExpression { Name = generatedArray }
//@[22:0027)   ├─AmbientTypeReferenceExpression { Name = array }
//@[30:0055)   └─ForLoopExpression
//@[40:0051)     ├─FunctionCallExpression { Name = range }
//@[46:0047)     | ├─IntegerLiteralExpression { Value = 0 }
//@[48:0050)     | └─IntegerLiteralExpression { Value = 10 }
//@[53:0054)     └─ArrayAccessExpression
//@[53:0054)       ├─CopyIndexExpression
//@[40:0051)       └─FunctionCallExpression { Name = range }
//@[46:0047)         ├─IntegerLiteralExpression { Value = 0 }
//@[48:0050)         └─IntegerLiteralExpression { Value = 10 }

