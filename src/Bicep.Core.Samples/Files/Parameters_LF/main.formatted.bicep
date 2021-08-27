/* 
  This is a block comment.
*/

// parameters without default value
@sys.description('''
this is my multi line 
description for my myString
''')
param myString string
param myInt int
param myBool bool

// parameters with default value
@sys.description('this is myString2')
@metadata({
  description: 'overwrite but still valid'
})
param myString2 string = 'string value'
param myInt2 int = 42
param myTruth bool = true
param myFalsehood bool = false
param myEscapedString string = 'First line\r\nSecond\ttabbed\tline'

// object default value
@sys.description('this is foo')
@metadata({
  description: 'overwrite but still valid'
  another: 'just for fun'
})
param foo object = {
  enabled: true
  name: 'this is my object'
  priority: 3
  info: {
    a: 'b'
  }
  empty: {}
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
  'a'
  'b'
  'c'
]

// secure string
@secure()
param password string

// secure object
@secure()
param secretObject object

// enum parameter
@allowed([
  'Standard_LRS'
  'Standard_GRS'
])
param storageSku string

// length constraint on a string
@minLength(3)
@maxLength(24)
param storageName string

// length constraint on an array
@minLength(3)
@maxLength(24)
param someArray array

// empty metadata
@metadata({})
param emptyMetadata string

// description
@metadata({
  description: 'my description'
})
param description string

@sys.description('my description')
param description2 string

// random extra metadata
@metadata({
  description: 'my description'
  a: 1
  b: true
  c: []
  d: {
    test: 'abc'
  }
})
param additionalMetadata string

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

param defaultExpression bool = 18 != (true || false)

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

@secure()
@minLength(2)
@maxLength(10)
@allowed([
  'Apple'
  'Banana'
])
param decoratedString string

@minValue(200)
param decoratedInt int = 123

// negative integer literals are allowed as decorator values
@minValue(-10)
@maxValue(-3)
param negativeValues int

@sys.description('A boolean.')
@metadata({
  description: 'I will be overrode.'
  foo: 'something'
  bar: [
    {}
    true
    123
  ]
})
param decoratedBool bool = (true && false) != true

@secure()
param decoratedObject object = {
  enabled: true
  name: 'this is my object'
  priority: 3
  info: {
    a: 'b'
  }
  empty: {}
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
  description: 'An array.'
})
@sys.maxLength(20)
@sys.description('I will be overrode.')
param decoratedArray array = [
  utcNow()
  newGuid()
]
