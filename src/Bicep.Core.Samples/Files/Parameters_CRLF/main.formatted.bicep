/* 
  This is a block comment.
*/

// parameters without default value
param myString string
param myInt int
param myBool bool

// parameters with default value
param myString2 string = 'string value'
param myInt2 int = 42
param myTruth bool = true
param myFalsehood bool = false
param myEscapedString string = 'First line\r\nSecond\ttabbed\tline'

// object default value
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

// alternative array parameter
param myAlternativeArrayParam array {
  default: [
    'a'
    'b'
    'c'
  ]
}

// secure string
param password string {
  secure: true
}

@secure()
param passwordWithDecorator string

// non-secure string
param nonSecure string {
  secure: false
}

// secure object
param secretObject object {
  secure: true
}

@secure()
param secretObjectWithDecorator object

// enum parameter
param storageSku string {
  allowed: [
    'Standard_LRS'
    'Standard_GRS'
  ]
}

@allowed([
  'Standard_LRS'
  'Standard_GRS'
])
param storageSkuWithDecorator string

// length constraint on a string
param storageName string {
  minLength: 3
  maxLength: 24
}

@minLength(3)
@maxLength(24)
param storageNameWithDecorator string

// length constraint on an array
param someArray array {
  minLength: 3
  maxLength: 24
}

@minLength(3)
@maxLength(24)
param someArrayWithDecorator array

// empty metadata
param emptyMetadata string {
  metadata: {}
}

@metadata({})
param emptyMetadataWithDecorator string

// description
param description string {
  metadata: {
    description: 'my description'
  }
}

@metadata({
  description: 'my description'
})
param descriptionWithDecorator string

@sys.description('my description')
param descriptionWithDecorator2 string

// random extra metadata
param additionalMetadata string {
  metadata: {
    description: 'my description'
    a: 1
    b: true
    c: []
    d: {
      test: 'abc'
    }
  }
}

@metadata({
  description: 'my description'
  a: 1
  b: true
  c: []
  d: {
    test: 'abc'
  }
})
param additionalMetadataWithDecorator string

// all modifiers together
param someParameter string {
  secure: true
  minLength: 3
  maxLength: 24
  default: 'one'
  allowed: [
    'one'
    'two'
    'three'
  ]
  metadata: {
    description: 'Name of the storage account'
  }
}

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
param someParameterWithDecorator string

param defaultValueExpression int {
  default: true ? 4 + 2 * 3 : 0
}

param defaultExpression bool = 18 != (true || false)

param stringLiteral string {
  allowed: [
    'abc'
    'def'
  ]
}

param stringLiteralWithAllowedValuesSuperset string {
  allowed: [
    'abc'
    'def'
    'ghi'
  ]
  default: stringLiteral
}

@allowed([
  'abc'
  'def'
])
param stringLiteralWithDecorator string

@allowed([
  'abc'
  'def'
  'ghi'
])
param stringLiteralWithDecoratorWithAllowedValuesSuperset string = stringLiteralWithDecorator

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

// negative integer literals in modifiers
param negativeModifiers int {
  minValue: -100
  maxValue: -33
}

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
@secure()
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
@maxLength(10)
@maxLength(5)
@sys.description('I will be overrode.')
param decoratedArray array = [
  utcNow()
  newGuid()
]
