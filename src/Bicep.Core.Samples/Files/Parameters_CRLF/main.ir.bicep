/*
//@[00:2929) ProgramExpression
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
param password string
//@[15:0021) | └─AmbientTypeReferenceExpression { Name = string }

// secure object
@secure()
//@[00:0036) ├─DeclaredParameterExpression { Name = secretObject }
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
@maxLength(24)
param storageName string
//@[18:0024) | └─AmbientTypeReferenceExpression { Name = string }

// length constraint on an array
@minLength(3)
//@[00:0052) ├─DeclaredParameterExpression { Name = someArray }
@maxLength(24)
param someArray array
//@[16:0021) | └─AmbientTypeReferenceExpression { Name = array }

// empty metadata
@metadata({})
//@[00:0041) ├─DeclaredParameterExpression { Name = emptyMetadata }
param emptyMetadata string
//@[20:0026) | └─AmbientTypeReferenceExpression { Name = string }

// description
@metadata({
//@[00:0074) ├─DeclaredParameterExpression { Name = description }
  description: 'my description'
})
param description string
//@[18:0024) | └─AmbientTypeReferenceExpression { Name = string }

@sys.description('my description')
//@[00:0061) ├─DeclaredParameterExpression { Name = description2 }
param description2 string
//@[19:0025) | └─AmbientTypeReferenceExpression { Name = string }

// random extra metadata
@metadata({
//@[00:0143) ├─DeclaredParameterExpression { Name = additionalMetadata }
  description: 'my description'
  a: 1
  b: true
  c: [
  ]
  d: {
    test: 'abc'
  }
})
param additionalMetadata string
//@[25:0031) | └─AmbientTypeReferenceExpression { Name = string }

// all modifiers together
@secure()
//@[00:0176) ├─DeclaredParameterExpression { Name = someParameter }
@minLength(3)
@maxLength(24)
@allowed([
  'one'
  'two'
  'three'
])
@metadata({
  description: 'Name of the storage account'
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
@minLength(2)
  @maxLength(10)
@allowed([
  'Apple'
  'Banana'
])
param decoratedString string
//@[22:0028) | └─AmbientTypeReferenceExpression { Name = string }

@minValue(100)
//@[00:0044) ├─DeclaredParameterExpression { Name = decoratedInt }
param decoratedInt int = 123
//@[19:0022) | ├─AmbientTypeReferenceExpression { Name = int }
//@[25:0028) | └─IntegerLiteralExpression { Value = 123 }

// negative integer literals are allowed as decorator values
@minValue(-10)
//@[00:0055) ├─DeclaredParameterExpression { Name = negativeValues }
@maxValue(-3)
param negativeValues int
//@[21:0024) | └─AmbientTypeReferenceExpression { Name = int }

@sys.description('A boolean.')
//@[00:0229) ├─DeclaredParameterExpression { Name = decoratedBool }
@metadata({
    description: 'I will be overrode.'
    foo: 'something'
    bar: [
        {          }
        true
        123
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
    description: 'An array.'
})
@sys.maxLength(20)
@sys.description('I will be overrode.')
param decoratedArray array = [
//@[21:0026)   ├─AmbientTypeReferenceExpression { Name = array }
//@[29:0062)   └─ArrayExpression
    utcNow()
//@[04:0012)     ├─FunctionCallExpression { Name = utcNow }
    newGuid()
//@[04:0013)     └─FunctionCallExpression { Name = newGuid }
]

