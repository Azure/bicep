/*
  This is a block comment.
*/

// parameters without default value
param myString string
//@[06:014) [no-unused-params (Warning)] Parameter "myString" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-params) |myString|
param myInt int
//@[06:011) [no-unused-params (Warning)] Parameter "myInt" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-params) |myInt|
param myBool bool
//@[06:012) [no-unused-params (Warning)] Parameter "myBool" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-params) |myBool|

// parameters with default value
param myString2 string = 'string value'
//@[06:015) [no-unused-params (Warning)] Parameter "myString2" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-params) |myString2|
param myInt2 int = 42
//@[06:012) [no-unused-params (Warning)] Parameter "myInt2" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-params) |myInt2|
param myTruth bool = true
//@[06:013) [no-unused-params (Warning)] Parameter "myTruth" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-params) |myTruth|
param myFalsehood bool = false
//@[06:017) [no-unused-params (Warning)] Parameter "myFalsehood" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-params) |myFalsehood|
param myEscapedString string = 'First line\r\nSecond\ttabbed\tline'
//@[06:021) [no-unused-params (Warning)] Parameter "myEscapedString" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-params) |myEscapedString|

// object default value
param foo object = {
//@[06:009) [no-unused-params (Warning)] Parameter "foo" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-params) |foo|
//@[10:016) [use-user-defined-types (Warning)] Use user-defined types instead of 'object' or 'array'. (bicep core linter https://aka.ms/bicep/linter-diagnostics#use-user-defined-types) |object|
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
//@[06:018) [no-unused-params (Warning)] Parameter "myArrayParam" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-params) |myArrayParam|
//@[19:024) [use-user-defined-types (Warning)] Use user-defined types instead of 'object' or 'array'. (bicep core linter https://aka.ms/bicep/linter-diagnostics#use-user-defined-types) |array|
  'a'
  'b'
  'c'
]

// secure string
@secure()
param password string
//@[06:014) [no-unused-params (Warning)] Parameter "password" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-params) |password|

// secure object
@secure()
param secretObject object
//@[06:018) [no-unused-params (Warning)] Parameter "secretObject" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-params) |secretObject|
//@[19:025) [use-user-defined-types (Warning)] Use user-defined types instead of 'object' or 'array'. (bicep core linter https://aka.ms/bicep/linter-diagnostics#use-user-defined-types) |object|

// enum parameter
@allowed([
  'Standard_LRS'
  'Standard_GRS'
])
param storageSku string
//@[06:016) [no-unused-params (Warning)] Parameter "storageSku" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-params) |storageSku|

// length constraint on a string
@minLength(3)
@maxLength(24)
param storageName string
//@[06:017) [no-unused-params (Warning)] Parameter "storageName" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-params) |storageName|

// length constraint on an array
@minLength(3)
@maxLength(24)
param someArray array
//@[06:015) [no-unused-params (Warning)] Parameter "someArray" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-params) |someArray|
//@[16:021) [use-user-defined-types (Warning)] Use user-defined types instead of 'object' or 'array'. (bicep core linter https://aka.ms/bicep/linter-diagnostics#use-user-defined-types) |array|

// empty metadata
@metadata({})
param emptyMetadata string
//@[06:019) [no-unused-params (Warning)] Parameter "emptyMetadata" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-params) |emptyMetadata|

// description
@metadata({
  description: 'my description'
})
param description string
//@[06:017) [no-unused-params (Warning)] Parameter "description" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-params) |description|

@sys.description('my description')
param description2 string
//@[06:018) [no-unused-params (Warning)] Parameter "description2" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-params) |description2|

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
//@[06:024) [no-unused-params (Warning)] Parameter "additionalMetadata" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-params) |additionalMetadata|

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
//@[06:019) [no-unused-params (Warning)] Parameter "someParameter" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-params) |someParameter|

param defaultExpression bool = 18 != (true || false)
//@[06:023) [no-unused-params (Warning)] Parameter "defaultExpression" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-params) |defaultExpression|

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
//@[06:044) [no-unused-params (Warning)] Parameter "stringLiteralWithAllowedValuesSuperset" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-params) |stringLiteralWithAllowedValuesSuperset|

@secure()
@minLength(2)
  @maxLength(10)
@allowed([
  'Apple'
  'Banana'
])
param decoratedString string
//@[06:021) [no-unused-params (Warning)] Parameter "decoratedString" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-params) |decoratedString|

@minValue(100)
param decoratedInt int = 123
//@[06:018) [no-unused-params (Warning)] Parameter "decoratedInt" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-params) |decoratedInt|

// negative integer literals are allowed as decorator values
@minValue(-10)
@maxValue(-3)
param negativeValues int
//@[06:020) [no-unused-params (Warning)] Parameter "negativeValues" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-params) |negativeValues|

@sys.description('A boolean.')
@metadata({
    description: 'I will be overrode.'
//@[04:015) [no-conflicting-metadata (Warning)] The "description" metadata property conflicts with the "description" decorator and will be overwritten. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-conflicting-metadata) |description|
    foo: 'something'
    bar: [
        {          }
        true
        123
    ]
})
param decoratedBool bool = (true && false) != true
//@[06:019) [no-unused-params (Warning)] Parameter "decoratedBool" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-params) |decoratedBool|

@secure()
param decoratedObject object = {
//@[06:021) [no-unused-params (Warning)] Parameter "decoratedObject" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-params) |decoratedObject|
//@[22:028) [use-user-defined-types (Warning)] Use user-defined types instead of 'object' or 'array'. (bicep core linter https://aka.ms/bicep/linter-diagnostics#use-user-defined-types) |object|
//@[29:265) [secure-parameter-default (Warning)] Secure parameters should not have hardcoded defaults (except for empty or newGuid()). (bicep core linter https://aka.ms/bicep/linter-diagnostics#secure-parameter-default) |= {\r\n  enabled: true\r\n  name: 'this is my object'\r\n  priority: 3\r\n  info: {\r\n    a: 'b'\r\n  }\r\n  empty: {\r\n  }\r\n  array: [\r\n    'string item'\r\n    12\r\n    true\r\n    [\r\n      'inner'\r\n      false\r\n    ]\r\n    {\r\n      a: 'b'\r\n    }\r\n  ]\r\n}|
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
//@[04:015) [no-conflicting-metadata (Warning)] The "description" metadata property conflicts with the "description" decorator and will be overwritten. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-conflicting-metadata) |description|
})
@sys.maxLength(20)
@sys.description('An array.')
param decoratedArray array = [
//@[06:020) [no-unused-params (Warning)] Parameter "decoratedArray" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-params) |decoratedArray|
//@[21:026) [use-user-defined-types (Warning)] Use user-defined types instead of 'object' or 'array'. (bicep core linter https://aka.ms/bicep/linter-diagnostics#use-user-defined-types) |array|
    utcNow()
    newGuid()
]

