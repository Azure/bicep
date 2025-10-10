/*
//@[00:2981) ProgramExpression
  This is a block comment.
*/

// parameters without default value
param myString string
//@[00:0021) ├─DeclaredParameterExpression { Name = myString }
//@[15:0021) | └─AmbientTypeReferenceExpression { Name = string }
param myInt int
//@[00:0015) ├─DeclaredParameterExpression { Name = myInt }
//@[12:0015) | └─AmbientTypeReferenceExpression { Name = int }
param myBool bool
//@[00:0017) ├─DeclaredParameterExpression { Name = myBool }
//@[13:0017) | └─AmbientTypeReferenceExpression { Name = bool }
param myAny any
//@[00:0015) ├─DeclaredParameterExpression { Name = myAny }
//@[12:0015) | └─AmbientTypeReferenceExpression { Name = any }

// parameters with default value
param myString2 string = 'string value'
//@[00:0039) ├─DeclaredParameterExpression { Name = myString2 }
//@[16:0022) | ├─AmbientTypeReferenceExpression { Name = string }
//@[25:0039) | └─StringLiteralExpression { Value = string value }
param myInt2 int = 42
//@[00:0021) ├─DeclaredParameterExpression { Name = myInt2 }
//@[13:0016) | ├─AmbientTypeReferenceExpression { Name = int }
//@[19:0021) | └─IntegerLiteralExpression { Value = 42 }
param myTruth bool = true
//@[00:0025) ├─DeclaredParameterExpression { Name = myTruth }
//@[14:0018) | ├─AmbientTypeReferenceExpression { Name = bool }
//@[21:0025) | └─BooleanLiteralExpression { Value = True }
param myFalsehood bool = false
//@[00:0030) ├─DeclaredParameterExpression { Name = myFalsehood }
//@[18:0022) | ├─AmbientTypeReferenceExpression { Name = bool }
//@[25:0030) | └─BooleanLiteralExpression { Value = False }
param myEscapedString string = 'First line\r\nSecond\ttabbed\tline'
//@[00:0067) ├─DeclaredParameterExpression { Name = myEscapedString }
//@[22:0028) | ├─AmbientTypeReferenceExpression { Name = string }
//@[31:0067) | └─StringLiteralExpression { Value = First line\r\nSecond\ttabbed\tline }
param myAny2 any = myAny.property
//@[00:0033) ├─DeclaredParameterExpression { Name = myAny2 }
//@[13:0016) | ├─AmbientTypeReferenceExpression { Name = any }
//@[19:0033) | └─PropertyAccessExpression { PropertyName = property }
//@[19:0024) |   └─ParametersReferenceExpression { Parameter = myAny }

// object default value
param foo object = {
//@[00:0253) ├─DeclaredParameterExpression { Name = foo }
//@[10:0016) | ├─AmbientTypeReferenceExpression { Name = object }
//@[19:0253) | └─ObjectExpression
  enabled: true
//@[02:0015) |   ├─ObjectPropertyExpression
//@[02:0009) |   | ├─StringLiteralExpression { Value = enabled }
//@[11:0015) |   | └─BooleanLiteralExpression { Value = True }
  name: 'this is my object'
//@[02:0027) |   ├─ObjectPropertyExpression
//@[02:0006) |   | ├─StringLiteralExpression { Value = name }
//@[08:0027) |   | └─StringLiteralExpression { Value = this is my object }
  priority: 3
//@[02:0013) |   ├─ObjectPropertyExpression
//@[02:0010) |   | ├─StringLiteralExpression { Value = priority }
//@[12:0013) |   | └─IntegerLiteralExpression { Value = 3 }
  info: {
//@[02:0026) |   ├─ObjectPropertyExpression
//@[02:0006) |   | ├─StringLiteralExpression { Value = info }
//@[08:0026) |   | └─ObjectExpression
    a: 'b'
//@[04:0010) |   |   └─ObjectPropertyExpression
//@[04:0005) |   |     ├─StringLiteralExpression { Value = a }
//@[07:0010) |   |     └─StringLiteralExpression { Value = b }
  }
  empty: {
//@[02:0015) |   ├─ObjectPropertyExpression
//@[02:0007) |   | ├─StringLiteralExpression { Value = empty }
//@[09:0015) |   | └─ObjectExpression
  }
  array: [
//@[02:0122) |   └─ObjectPropertyExpression
//@[02:0007) |     ├─StringLiteralExpression { Value = array }
//@[09:0122) |     └─ArrayExpression
    'string item'
//@[04:0017) |       ├─StringLiteralExpression { Value = string item }
    12
//@[04:0006) |       ├─IntegerLiteralExpression { Value = 12 }
    true
//@[04:0008) |       ├─BooleanLiteralExpression { Value = True }
    [
//@[04:0040) |       ├─ArrayExpression
      'inner'
//@[06:0013) |       | ├─StringLiteralExpression { Value = inner }
      false
//@[06:0011) |       | └─BooleanLiteralExpression { Value = False }
    ]
    {
//@[04:0026) |       └─ObjectExpression
      a: 'b'
//@[06:0012) |         └─ObjectPropertyExpression
//@[06:0007) |           ├─StringLiteralExpression { Value = a }
//@[09:0012) |           └─StringLiteralExpression { Value = b }
    }
  ]
}

// array default value
param myArrayParam array = [
//@[00:0052) ├─DeclaredParameterExpression { Name = myArrayParam }
//@[19:0024) | ├─AmbientTypeReferenceExpression { Name = array }
//@[27:0052) | └─ArrayExpression
  'a'
//@[02:0005) |   ├─StringLiteralExpression { Value = a }
  'b'
//@[02:0005) |   ├─StringLiteralExpression { Value = b }
  'c'
//@[02:0005) |   └─StringLiteralExpression { Value = c }
]

// secure string
@secure()
//@[00:0032) ├─DeclaredParameterExpression { Name = password }
//@[01:0009) | ├─FunctionCallExpression { Name = secure }
param password string
//@[15:0021) | └─AmbientTypeReferenceExpression { Name = string }

// secure object
@secure()
//@[00:0036) ├─DeclaredParameterExpression { Name = secretObject }
//@[01:0009) | ├─FunctionCallExpression { Name = secure }
param secretObject object
//@[19:0025) | └─AmbientTypeReferenceExpression { Name = object }

// enum parameter
@allowed([
//@[00:0075) ├─DeclaredParameterExpression { Name = storageSku }
  'Standard_LRS'
  'Standard_GRS'
])
param storageSku string
//@[17:0023) | └─AmbientTypeReferenceExpression { Name = string }

// length constraint on a string
@minLength(3)
//@[00:0055) ├─DeclaredParameterExpression { Name = storageName }
//@[11:0012) | ├─IntegerLiteralExpression { Value = 3 }
@maxLength(24)
//@[11:0013) | ├─IntegerLiteralExpression { Value = 24 }
param storageName string
//@[18:0024) | └─AmbientTypeReferenceExpression { Name = string }

// length constraint on an array
@minLength(3)
//@[00:0052) ├─DeclaredParameterExpression { Name = someArray }
//@[11:0012) | ├─IntegerLiteralExpression { Value = 3 }
@maxLength(24)
//@[11:0013) | ├─IntegerLiteralExpression { Value = 24 }
param someArray array
//@[16:0021) | └─AmbientTypeReferenceExpression { Name = array }

// empty metadata
@metadata({})
//@[00:0041) ├─DeclaredParameterExpression { Name = emptyMetadata }
//@[10:0012) | ├─ObjectExpression
param emptyMetadata string
//@[20:0026) | └─AmbientTypeReferenceExpression { Name = string }

// description
@metadata({
//@[00:0074) ├─DeclaredParameterExpression { Name = description }
//@[10:0047) | ├─ObjectExpression
  description: 'my description'
//@[02:0031) | | └─ObjectPropertyExpression
//@[02:0013) | |   ├─StringLiteralExpression { Value = description }
//@[15:0031) | |   └─StringLiteralExpression { Value = my description }
})
param description string
//@[18:0024) | └─AmbientTypeReferenceExpression { Name = string }

@sys.description('my description')
//@[00:0061) ├─DeclaredParameterExpression { Name = description2 }
//@[17:0033) | ├─StringLiteralExpression { Value = my description }
param description2 string
//@[19:0025) | └─AmbientTypeReferenceExpression { Name = string }

// random extra metadata
@metadata({
//@[00:0143) ├─DeclaredParameterExpression { Name = additionalMetadata }
//@[10:0109) | ├─ObjectExpression
  description: 'my description'
//@[02:0031) | | ├─ObjectPropertyExpression
//@[02:0013) | | | ├─StringLiteralExpression { Value = description }
//@[15:0031) | | | └─StringLiteralExpression { Value = my description }
  a: 1
//@[02:0006) | | ├─ObjectPropertyExpression
//@[02:0003) | | | ├─StringLiteralExpression { Value = a }
//@[05:0006) | | | └─IntegerLiteralExpression { Value = 1 }
  b: true
//@[02:0009) | | ├─ObjectPropertyExpression
//@[02:0003) | | | ├─StringLiteralExpression { Value = b }
//@[05:0009) | | | └─BooleanLiteralExpression { Value = True }
  c: [
//@[02:0011) | | ├─ObjectPropertyExpression
//@[02:0003) | | | ├─StringLiteralExpression { Value = c }
//@[05:0011) | | | └─ArrayExpression
  ]
  d: {
//@[02:0028) | | └─ObjectPropertyExpression
//@[02:0003) | |   ├─StringLiteralExpression { Value = d }
//@[05:0028) | |   └─ObjectExpression
    test: 'abc'
//@[04:0015) | |     └─ObjectPropertyExpression
//@[04:0008) | |       ├─StringLiteralExpression { Value = test }
//@[10:0015) | |       └─StringLiteralExpression { Value = abc }
  }
})
param additionalMetadata string
//@[25:0031) | └─AmbientTypeReferenceExpression { Name = string }

// all modifiers together
@secure()
//@[00:0176) ├─DeclaredParameterExpression { Name = someParameter }
//@[01:0009) | ├─FunctionCallExpression { Name = secure }
@minLength(3)
//@[11:0012) | ├─IntegerLiteralExpression { Value = 3 }
@maxLength(24)
//@[11:0013) | ├─IntegerLiteralExpression { Value = 24 }
@allowed([
  'one'
  'two'
  'three'
])
@metadata({
//@[10:0060) | ├─ObjectExpression
  description: 'Name of the storage account'
//@[02:0044) | | └─ObjectPropertyExpression
//@[02:0013) | |   ├─StringLiteralExpression { Value = description }
//@[15:0044) | |   └─StringLiteralExpression { Value = Name of the storage account }
})
param someParameter string
//@[20:0026) | └─AmbientTypeReferenceExpression { Name = string }

param defaultExpression bool = 18 != (true || false)
//@[00:0052) ├─DeclaredParameterExpression { Name = defaultExpression }
//@[24:0028) | ├─AmbientTypeReferenceExpression { Name = bool }
//@[31:0052) | └─BinaryExpression { Operator = NotEquals }
//@[31:0033) |   ├─IntegerLiteralExpression { Value = 18 }
//@[38:0051) |   └─BinaryExpression { Operator = LogicalOr }
//@[38:0042) |     ├─BooleanLiteralExpression { Value = True }
//@[46:0051) |     └─BooleanLiteralExpression { Value = False }

@allowed([
//@[00:0060) ├─DeclaredParameterExpression { Name = stringLiteral }
  'abc'
  'def'
])
param stringLiteral string
//@[20:0026) | └─AmbientTypeReferenceExpression { Name = string }

@allowed([
//@[00:0110) ├─DeclaredParameterExpression { Name = stringLiteralWithAllowedValuesSuperset }
  'abc'
  'def'
  'ghi'
])
param stringLiteralWithAllowedValuesSuperset string = stringLiteral
//@[45:0051) | ├─AmbientTypeReferenceExpression { Name = string }
//@[54:0067) | └─ParametersReferenceExpression { Parameter = stringLiteral }

@secure()
//@[00:0111) ├─DeclaredParameterExpression { Name = decoratedString }
//@[01:0009) | ├─FunctionCallExpression { Name = secure }
@minLength(2)
//@[11:0012) | ├─IntegerLiteralExpression { Value = 2 }
  @maxLength(10)
//@[13:0015) | ├─IntegerLiteralExpression { Value = 10 }
@allowed([
  'Apple'
  'Banana'
])
param decoratedString string
//@[22:0028) | └─AmbientTypeReferenceExpression { Name = string }

@minValue(100)
//@[00:0044) ├─DeclaredParameterExpression { Name = decoratedInt }
//@[10:0013) | ├─IntegerLiteralExpression { Value = 100 }
param decoratedInt int = 123
//@[19:0022) | ├─AmbientTypeReferenceExpression { Name = int }
//@[25:0028) | └─IntegerLiteralExpression { Value = 123 }

// negative integer literals are allowed as decorator values
@minValue(-10)
//@[00:0055) ├─DeclaredParameterExpression { Name = negativeValues }
//@[10:0013) | ├─IntegerLiteralExpression { Value = -10 }
@maxValue(-3)
//@[10:0012) | ├─IntegerLiteralExpression { Value = -3 }
param negativeValues int
//@[21:0024) | └─AmbientTypeReferenceExpression { Name = int }

@sys.description('A boolean.')
//@[00:0229) ├─DeclaredParameterExpression { Name = decoratedBool }
//@[17:0029) | ├─StringLiteralExpression { Value = A boolean. }
@metadata({
//@[10:0144) | ├─ObjectExpression
    description: 'I will be overrode.'
//@[04:0038) | | ├─ObjectPropertyExpression
//@[04:0015) | | | ├─StringLiteralExpression { Value = description }
//@[17:0038) | | | └─StringLiteralExpression { Value = I will be overrode. }
    foo: 'something'
//@[04:0020) | | ├─ObjectPropertyExpression
//@[04:0007) | | | ├─StringLiteralExpression { Value = foo }
//@[09:0020) | | | └─StringLiteralExpression { Value = something }
    bar: [
//@[04:0066) | | └─ObjectPropertyExpression
//@[04:0007) | |   ├─StringLiteralExpression { Value = bar }
//@[09:0066) | |   └─ArrayExpression
        {          }
//@[08:0020) | |     ├─ObjectExpression
        true
//@[08:0012) | |     ├─BooleanLiteralExpression { Value = True }
        123
//@[08:0011) | |     └─IntegerLiteralExpression { Value = 123 }
    ]
})
param decoratedBool bool = (true && false) != true
//@[20:0024) | ├─AmbientTypeReferenceExpression { Name = bool }
//@[27:0050) | └─BinaryExpression { Operator = NotEquals }
//@[28:0041) |   ├─BinaryExpression { Operator = LogicalAnd }
//@[28:0032) |   | ├─BooleanLiteralExpression { Value = True }
//@[36:0041) |   | └─BooleanLiteralExpression { Value = False }
//@[46:0050) |   └─BooleanLiteralExpression { Value = True }

@secure()
//@[00:0276) ├─DeclaredParameterExpression { Name = decoratedObject }
//@[01:0009) | ├─FunctionCallExpression { Name = secure }
param decoratedObject object = {
//@[22:0028) | ├─AmbientTypeReferenceExpression { Name = object }
//@[31:0265) | └─ObjectExpression
  enabled: true
//@[02:0015) |   ├─ObjectPropertyExpression
//@[02:0009) |   | ├─StringLiteralExpression { Value = enabled }
//@[11:0015) |   | └─BooleanLiteralExpression { Value = True }
  name: 'this is my object'
//@[02:0027) |   ├─ObjectPropertyExpression
//@[02:0006) |   | ├─StringLiteralExpression { Value = name }
//@[08:0027) |   | └─StringLiteralExpression { Value = this is my object }
  priority: 3
//@[02:0013) |   ├─ObjectPropertyExpression
//@[02:0010) |   | ├─StringLiteralExpression { Value = priority }
//@[12:0013) |   | └─IntegerLiteralExpression { Value = 3 }
  info: {
//@[02:0026) |   ├─ObjectPropertyExpression
//@[02:0006) |   | ├─StringLiteralExpression { Value = info }
//@[08:0026) |   | └─ObjectExpression
    a: 'b'
//@[04:0010) |   |   └─ObjectPropertyExpression
//@[04:0005) |   |     ├─StringLiteralExpression { Value = a }
//@[07:0010) |   |     └─StringLiteralExpression { Value = b }
  }
  empty: {
//@[02:0015) |   ├─ObjectPropertyExpression
//@[02:0007) |   | ├─StringLiteralExpression { Value = empty }
//@[09:0015) |   | └─ObjectExpression
  }
  array: [
//@[02:0122) |   └─ObjectPropertyExpression
//@[02:0007) |     ├─StringLiteralExpression { Value = array }
//@[09:0122) |     └─ArrayExpression
    'string item'
//@[04:0017) |       ├─StringLiteralExpression { Value = string item }
    12
//@[04:0006) |       ├─IntegerLiteralExpression { Value = 12 }
    true
//@[04:0008) |       ├─BooleanLiteralExpression { Value = True }
    [
//@[04:0040) |       ├─ArrayExpression
      'inner'
//@[06:0013) |       | ├─StringLiteralExpression { Value = inner }
      false
//@[06:0011) |       | └─BooleanLiteralExpression { Value = False }
    ]
    {
//@[04:0026) |       └─ObjectExpression
      a: 'b'
//@[06:0012) |         └─ObjectPropertyExpression
//@[06:0007) |           ├─StringLiteralExpression { Value = a }
//@[09:0012) |           └─StringLiteralExpression { Value = b }
    }
  ]
}

@sys.metadata({
//@[00:0174) └─DeclaredParameterExpression { Name = decoratedArray }
//@[14:0058)   ├─ObjectExpression
    description: 'I will be overrode.'
//@[04:0038)   | └─ObjectPropertyExpression
//@[04:0015)   |   ├─StringLiteralExpression { Value = description }
//@[17:0038)   |   └─StringLiteralExpression { Value = I will be overrode. }
})
@sys.maxLength(20)
//@[15:0017)   ├─IntegerLiteralExpression { Value = 20 }
@sys.description('An array.')
//@[17:0028)   ├─StringLiteralExpression { Value = An array. }
param decoratedArray array = [
//@[21:0026)   ├─AmbientTypeReferenceExpression { Name = array }
//@[29:0062)   └─ArrayExpression
    utcNow()
//@[04:0012)     ├─FunctionCallExpression { Name = utcNow }
    newGuid()
//@[04:0013)     └─FunctionCallExpression { Name = newGuid }
]

