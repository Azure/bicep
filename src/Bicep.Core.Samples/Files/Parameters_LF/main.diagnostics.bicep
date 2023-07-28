/*
  This is a block comment.
*/

// parameters without default value
@sys.description('''
this is my multi line
description for my myString
''')
param myString string
//@[06:014) [no-unused-params (Warning)] Parameter "myString" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |myString|
param myInt int
//@[06:011) [no-unused-params (Warning)] Parameter "myInt" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |myInt|
param myBool bool
//@[06:012) [no-unused-params (Warning)] Parameter "myBool" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |myBool|

// parameters with default value
@sys.description('this is myString2')
@metadata({
  description: 'overwrite but still valid'
})
param myString2 string = 'string value'
//@[06:015) [no-unused-params (Warning)] Parameter "myString2" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |myString2|
param myInt2 int = 42
//@[06:012) [no-unused-params (Warning)] Parameter "myInt2" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |myInt2|
param myTruth bool = true
//@[06:013) [no-unused-params (Warning)] Parameter "myTruth" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |myTruth|
param myFalsehood bool = false
//@[06:017) [no-unused-params (Warning)] Parameter "myFalsehood" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |myFalsehood|
param myEscapedString string = 'First line\r\nSecond\ttabbed\tline'
//@[06:021) [no-unused-params (Warning)] Parameter "myEscapedString" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |myEscapedString|

// object default value
@sys.description('this is foo')
@metadata({
  description: 'overwrite but still valid'
  another: 'just for fun'
})
param foo object = {
//@[06:009) [no-unused-params (Warning)] Parameter "foo" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |foo|
  enabled: true
  name: 'this is my object'
  priority: 3
  info: {
    a: 'b'
  }
  empty: {
  }
  array: [
    'string item'
    12
    true
    [
      'inner'
      false
    ]
    {
      a: 'b'
    }
  ]
}

// array default value
param myArrayParam array = [
//@[06:018) [no-unused-params (Warning)] Parameter "myArrayParam" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |myArrayParam|
  'a'
  'b'
  'c'
]

// secure string
@secure()
param password string
//@[06:014) [no-unused-params (Warning)] Parameter "password" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |password|

// secure object
@secure()
param secretObject object
//@[06:018) [no-unused-params (Warning)] Parameter "secretObject" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |secretObject|

// enum parameter
@allowed([
  'Standard_LRS'
  'Standard_GRS'
])
param storageSku string
//@[06:016) [no-unused-params (Warning)] Parameter "storageSku" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |storageSku|

@allowed([
  1
  2
  3
])
param intEnum int
//@[06:013) [no-unused-params (Warning)] Parameter "intEnum" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |intEnum|

// length constraint on a string
@minLength(3)
@maxLength(24)
param storageName string
//@[06:017) [no-unused-params (Warning)] Parameter "storageName" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |storageName|

// length constraint on an array
@minLength(3)
@maxLength(24)
param someArray array
//@[06:015) [no-unused-params (Warning)] Parameter "someArray" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |someArray|

// empty metadata
@metadata({})
param emptyMetadata string
//@[06:019) [no-unused-params (Warning)] Parameter "emptyMetadata" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |emptyMetadata|

// description
@metadata({
  description: 'my description'
})
param description string
//@[06:017) [no-unused-params (Warning)] Parameter "description" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |description|

@sys.description('my description')
param description2 string
//@[06:018) [no-unused-params (Warning)] Parameter "description2" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |description2|

// random extra metadata
@metadata({
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
//@[06:024) [no-unused-params (Warning)] Parameter "additionalMetadata" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |additionalMetadata|

// all modifiers together
@secure()
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
//@[06:019) [no-unused-params (Warning)] Parameter "someParameter" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |someParameter|

param defaultExpression bool = 18 != (true || false)
//@[06:023) [no-unused-params (Warning)] Parameter "defaultExpression" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |defaultExpression|

@allowed([
  'abc'
  'def'
])
param stringLiteral string

@allowed([
  'abc'
  'def'
  'ghi'
])
param stringLiteralWithAllowedValuesSuperset string = stringLiteral
//@[06:044) [no-unused-params (Warning)] Parameter "stringLiteralWithAllowedValuesSuperset" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |stringLiteralWithAllowedValuesSuperset|

@secure()
@minLength(2)
  @maxLength(10)
@allowed([
  'Apple'
  'Banana'
])
param decoratedString string
//@[06:021) [no-unused-params (Warning)] Parameter "decoratedString" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |decoratedString|

@minValue(100)
param decoratedInt int = 123
//@[06:018) [no-unused-params (Warning)] Parameter "decoratedInt" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |decoratedInt|

// negative integer literals are allowed as decorator values
@minValue(-10)
@maxValue(-3)
param negativeValues int
//@[06:020) [no-unused-params (Warning)] Parameter "negativeValues" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |negativeValues|

@sys.description('A boolean.')
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
//@[06:019) [no-unused-params (Warning)] Parameter "decoratedBool" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |decoratedBool|

@secure()
param decoratedObject object = {
//@[06:021) [no-unused-params (Warning)] Parameter "decoratedObject" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |decoratedObject|
//@[29:244) [secure-parameter-default (Warning)] Secure parameters should not have hardcoded defaults (except for empty or newGuid()). (CodeDescription: bicep core(https://aka.ms/bicep/linter/secure-parameter-default)) |= {\n  enabled: true\n  name: 'this is my object'\n  priority: 3\n  info: {\n    a: 'b'\n  }\n  empty: {\n  }\n  array: [\n    'string item'\n    12\n    true\n    [\n      'inner'\n      false\n    ]\n    {\n      a: 'b'\n    }\n  ]\n}|
  enabled: true
  name: 'this is my object'
  priority: 3
  info: {
    a: 'b'
  }
  empty: {
  }
  array: [
    'string item'
    12
    true
    [
      'inner'
      false
    ]
    {
      a: 'b'
    }
  ]
}

@sys.metadata({
    description: 'I will be overrode.'
})
@sys.maxLength(20)
@sys.description('An array.')
param decoratedArray array = [
//@[06:020) [no-unused-params (Warning)] Parameter "decoratedArray" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-params)) |decoratedArray|
    utcNow()
    newGuid()
]

