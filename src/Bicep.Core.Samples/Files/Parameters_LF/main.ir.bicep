/*
//@[000:3137) ProgramExpression
  This is a block comment.
*/

// parameters without default value
@sys.description('''
//@[000:0097) ├─DeclaredParameterExpression { Name = myString }
this is my multi line
description for my myString
''')
param myString string
param myInt int
//@[000:0015) ├─DeclaredParameterExpression { Name = myInt }
param myBool bool
//@[000:0017) ├─DeclaredParameterExpression { Name = myBool }

// parameters with default value
@sys.description('this is myString2')
//@[000:0135) ├─DeclaredParameterExpression { Name = myString2 }
@metadata({
  description: 'overwrite but still valid'
})
param myString2 string = 'string value'
//@[025:0039) | └─StringLiteralExpression { Value = string value }
param myInt2 int = 42
//@[000:0021) ├─DeclaredParameterExpression { Name = myInt2 }
//@[019:0021) | └─IntegerLiteralExpression { Value = 42 }
param myTruth bool = true
//@[000:0025) ├─DeclaredParameterExpression { Name = myTruth }
//@[021:0025) | └─BooleanLiteralExpression { Value = True }
param myFalsehood bool = false
//@[000:0030) ├─DeclaredParameterExpression { Name = myFalsehood }
//@[025:0030) | └─BooleanLiteralExpression { Value = False }
param myEscapedString string = 'First line\r\nSecond\ttabbed\tline'
//@[000:0067) ├─DeclaredParameterExpression { Name = myEscapedString }
//@[031:0067) | └─StringLiteralExpression { Value = First line\r\nSecond\ttabbed\tline }

// object default value
@sys.description('this is foo')
//@[000:0348) ├─DeclaredParameterExpression { Name = foo }
@metadata({
  description: 'overwrite but still valid'
  another: 'just for fun'
})
param foo object = {
//@[019:0232) | └─ObjectExpression
  enabled: true
//@[002:0015) |   ├─ObjectPropertyExpression
//@[002:0009) |   | ├─StringLiteralExpression { Value = enabled }
//@[011:0015) |   | └─BooleanLiteralExpression { Value = True }
  name: 'this is my object'
//@[002:0027) |   ├─ObjectPropertyExpression
//@[002:0006) |   | ├─StringLiteralExpression { Value = name }
//@[008:0027) |   | └─StringLiteralExpression { Value = this is my object }
  priority: 3
//@[002:0013) |   ├─ObjectPropertyExpression
//@[002:0010) |   | ├─StringLiteralExpression { Value = priority }
//@[012:0013) |   | └─IntegerLiteralExpression { Value = 3 }
  info: {
//@[002:0024) |   ├─ObjectPropertyExpression
//@[002:0006) |   | ├─StringLiteralExpression { Value = info }
//@[008:0024) |   | └─ObjectExpression
    a: 'b'
//@[004:0010) |   |   └─ObjectPropertyExpression
//@[004:0005) |   |     ├─StringLiteralExpression { Value = a }
//@[007:0010) |   |     └─StringLiteralExpression { Value = b }
  }
  empty: {
//@[002:0014) |   ├─ObjectPropertyExpression
//@[002:0007) |   | ├─StringLiteralExpression { Value = empty }
//@[009:0014) |   | └─ObjectExpression
  }
  array: [
//@[002:0111) |   └─ObjectPropertyExpression
//@[002:0007) |     ├─StringLiteralExpression { Value = array }
//@[009:0111) |     └─ArrayExpression
    'string item'
//@[004:0017) |       ├─StringLiteralExpression { Value = string item }
    12
//@[004:0006) |       ├─IntegerLiteralExpression { Value = 12 }
    true
//@[004:0008) |       ├─BooleanLiteralExpression { Value = True }
    [
//@[004:0037) |       ├─ArrayExpression
      'inner'
//@[006:0013) |       | ├─StringLiteralExpression { Value = inner }
      false
//@[006:0011) |       | └─BooleanLiteralExpression { Value = False }
    ]
    {
//@[004:0024) |       └─ObjectExpression
      a: 'b'
//@[006:0012) |         └─ObjectPropertyExpression
//@[006:0007) |           ├─StringLiteralExpression { Value = a }
//@[009:0012) |           └─StringLiteralExpression { Value = b }
    }
  ]
}

// array default value
param myArrayParam array = [
//@[000:0048) ├─DeclaredParameterExpression { Name = myArrayParam }
//@[027:0048) | └─ArrayExpression
  'a'
//@[002:0005) |   ├─StringLiteralExpression { Value = a }
  'b'
//@[002:0005) |   ├─StringLiteralExpression { Value = b }
  'c'
//@[002:0005) |   └─StringLiteralExpression { Value = c }
]

// secure string
@secure()
//@[000:0031) ├─DeclaredParameterExpression { Name = password }
param password string

// secure object
@secure()
//@[000:0035) ├─DeclaredParameterExpression { Name = secretObject }
param secretObject object

// enum parameter
@allowed([
//@[000:0071) ├─DeclaredParameterExpression { Name = storageSku }
  'Standard_LRS'
  'Standard_GRS'
])
param storageSku string

@allowed([
//@[000:0043) ├─DeclaredParameterExpression { Name = intEnum }
  1
  2
  3
])
param intEnum int

// length constraint on a string
@minLength(3)
//@[000:0053) ├─DeclaredParameterExpression { Name = storageName }
@maxLength(24)
param storageName string

// length constraint on an array
@minLength(3)
//@[000:0050) ├─DeclaredParameterExpression { Name = someArray }
@maxLength(24)
param someArray array

// empty metadata
@metadata({})
//@[000:0040) ├─DeclaredParameterExpression { Name = emptyMetadata }
param emptyMetadata string

// description
@metadata({
//@[000:0071) ├─DeclaredParameterExpression { Name = description }
  description: 'my description'
})
param description string

@sys.description('my description')
//@[000:0060) ├─DeclaredParameterExpression { Name = description2 }
param description2 string

// random extra metadata
@metadata({
//@[000:0133) ├─DeclaredParameterExpression { Name = additionalMetadata }
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

// all modifiers together
@secure()
//@[000:0165) ├─DeclaredParameterExpression { Name = someParameter }
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

param defaultExpression bool = 18 != (true || false)
//@[000:0052) ├─DeclaredParameterExpression { Name = defaultExpression }
//@[031:0052) | └─BinaryExpression { Operator = NotEquals }
//@[031:0033) |   ├─IntegerLiteralExpression { Value = 18 }
//@[038:0051) |   └─BinaryExpression { Operator = LogicalOr }
//@[038:0042) |     ├─BooleanLiteralExpression { Value = True }
//@[046:0051) |     └─BooleanLiteralExpression { Value = False }

@allowed([
//@[000:0056) ├─DeclaredParameterExpression { Name = stringLiteral }
  'abc'
  'def'
])
param stringLiteral string

@allowed([
//@[000:0105) ├─DeclaredParameterExpression { Name = stringLiteralWithAllowedValuesSuperset }
  'abc'
  'def'
  'ghi'
])
param stringLiteralWithAllowedValuesSuperset string = stringLiteral
//@[054:0067) | └─ParametersReferenceExpression { Parameter = stringLiteral }

@secure()
//@[000:0104) ├─DeclaredParameterExpression { Name = decoratedString }
@minLength(2)
  @maxLength(10)
@allowed([
  'Apple'
  'Banana'
])
param decoratedString string

@minValue(200)
//@[000:0043) ├─DeclaredParameterExpression { Name = decoratedInt }
param decoratedInt int = 123
//@[025:0028) | └─IntegerLiteralExpression { Value = 123 }

// negative integer literals are allowed as decorator values
@minValue(-10)
//@[000:0053) ├─DeclaredParameterExpression { Name = negativeValues }
@maxValue(-3)
param negativeValues int

@sys.description('A boolean.')
//@[000:0283) ├─DeclaredParameterExpression { Name = decoratedBool }
@metadata({
    description: 'I will be overrode.'
    foo: 'something'
    bar: [
        {          }
        true
        123
    ]
})
param decoratedBool bool = /* comment1 */ /* comment2*/      /* comment3 */ /* comment4 */ (true && false) != true
//@[091:0114) | └─BinaryExpression { Operator = NotEquals }
//@[092:0105) |   ├─BinaryExpression { Operator = LogicalAnd }
//@[092:0096) |   | ├─BooleanLiteralExpression { Value = True }
//@[100:0105) |   | └─BooleanLiteralExpression { Value = False }
//@[110:0114) |   └─BooleanLiteralExpression { Value = True }

@secure()
//@[000:0254) ├─DeclaredParameterExpression { Name = decoratedObject }
param decoratedObject object = {
//@[031:0244) | └─ObjectExpression
  enabled: true
//@[002:0015) |   ├─ObjectPropertyExpression
//@[002:0009) |   | ├─StringLiteralExpression { Value = enabled }
//@[011:0015) |   | └─BooleanLiteralExpression { Value = True }
  name: 'this is my object'
//@[002:0027) |   ├─ObjectPropertyExpression
//@[002:0006) |   | ├─StringLiteralExpression { Value = name }
//@[008:0027) |   | └─StringLiteralExpression { Value = this is my object }
  priority: 3
//@[002:0013) |   ├─ObjectPropertyExpression
//@[002:0010) |   | ├─StringLiteralExpression { Value = priority }
//@[012:0013) |   | └─IntegerLiteralExpression { Value = 3 }
  info: {
//@[002:0024) |   ├─ObjectPropertyExpression
//@[002:0006) |   | ├─StringLiteralExpression { Value = info }
//@[008:0024) |   | └─ObjectExpression
    a: 'b'
//@[004:0010) |   |   └─ObjectPropertyExpression
//@[004:0005) |   |     ├─StringLiteralExpression { Value = a }
//@[007:0010) |   |     └─StringLiteralExpression { Value = b }
  }
  empty: {
//@[002:0014) |   ├─ObjectPropertyExpression
//@[002:0007) |   | ├─StringLiteralExpression { Value = empty }
//@[009:0014) |   | └─ObjectExpression
  }
  array: [
//@[002:0111) |   └─ObjectPropertyExpression
//@[002:0007) |     ├─StringLiteralExpression { Value = array }
//@[009:0111) |     └─ArrayExpression
    'string item'
//@[004:0017) |       ├─StringLiteralExpression { Value = string item }
    12
//@[004:0006) |       ├─IntegerLiteralExpression { Value = 12 }
    true
//@[004:0008) |       ├─BooleanLiteralExpression { Value = True }
    [
//@[004:0037) |       ├─ArrayExpression
      'inner'
//@[006:0013) |       | ├─StringLiteralExpression { Value = inner }
      false
//@[006:0011) |       | └─BooleanLiteralExpression { Value = False }
    ]
    {
//@[004:0024) |       └─ObjectExpression
      a: 'b'
//@[006:0012) |         └─ObjectPropertyExpression
//@[006:0007) |           ├─StringLiteralExpression { Value = a }
//@[009:0012) |           └─StringLiteralExpression { Value = b }
    }
  ]
}

@sys.metadata({
//@[000:0166) └─DeclaredParameterExpression { Name = decoratedArray }
    description: 'An array.'
})
@sys.maxLength(20)
@sys.description('I will be overrode.')
param decoratedArray array = [
//@[029:0059)   └─ArrayExpression
    utcNow()
//@[004:0012)     ├─FunctionCallExpression { Name = utcNow }
    newGuid()
//@[004:0013)     └─FunctionCallExpression { Name = newGuid }
]

