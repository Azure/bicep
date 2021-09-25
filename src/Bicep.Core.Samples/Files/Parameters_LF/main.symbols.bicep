/* 
  This is a block comment.
*/

// parameters without default value
@sys.description('''
this is my multi line 
description for my myString
''')
param myString string
//@[6:14) Parameter myString. Type: string. Declaration start char: 0, length: 98
param myInt int
//@[6:11) Parameter myInt. Type: int. Declaration start char: 0, length: 15
param myBool bool
//@[6:12) Parameter myBool. Type: bool. Declaration start char: 0, length: 17

// parameters with default value
@sys.description('this is myString2')
@metadata({
  description: 'overwrite but still valid'
})
param myString2 string = 'string value'
//@[6:15) Parameter myString2. Type: string. Declaration start char: 0, length: 135
param myInt2 int = 42
//@[6:12) Parameter myInt2. Type: int. Declaration start char: 0, length: 21
param myTruth bool = true
//@[6:13) Parameter myTruth. Type: bool. Declaration start char: 0, length: 25
param myFalsehood bool = false
//@[6:17) Parameter myFalsehood. Type: bool. Declaration start char: 0, length: 30
param myEscapedString string = 'First line\r\nSecond\ttabbed\tline'
//@[6:21) Parameter myEscapedString. Type: string. Declaration start char: 0, length: 67

// object default value
@sys.description('this is foo')
@metadata({
  description: 'overwrite but still valid'
  another: 'just for fun'
})
param foo object = {
//@[6:9) Parameter foo. Type: object. Declaration start char: 0, length: 348
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
//@[6:18) Parameter myArrayParam. Type: array. Declaration start char: 0, length: 48
  'a'
  'b'
  'c'
]

// secure string
@secure()
param password string
//@[6:14) Parameter password. Type: string. Declaration start char: 0, length: 31

// secure object
@secure()
param secretObject object
//@[6:18) Parameter secretObject. Type: object. Declaration start char: 0, length: 35

// enum parameter
@allowed([
  'Standard_LRS'
  'Standard_GRS'
])
param storageSku string
//@[6:16) Parameter storageSku. Type: 'Standard_GRS' | 'Standard_LRS'. Declaration start char: 0, length: 71

// length constraint on a string
@minLength(3)
@maxLength(24)
param storageName string
//@[6:17) Parameter storageName. Type: string. Declaration start char: 0, length: 53

// length constraint on an array
@minLength(3)
@maxLength(24)
param someArray array
//@[6:15) Parameter someArray. Type: array. Declaration start char: 0, length: 50

// empty metadata
@metadata({})
param emptyMetadata string
//@[6:19) Parameter emptyMetadata. Type: string. Declaration start char: 0, length: 40

// description
@metadata({
  description: 'my description'
})
param description string
//@[6:17) Parameter description. Type: string. Declaration start char: 0, length: 71

@sys.description('my description')
param description2 string
//@[6:18) Parameter description2. Type: string. Declaration start char: 0, length: 60

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
//@[6:24) Parameter additionalMetadata. Type: string. Declaration start char: 0, length: 133

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
//@[6:19) Parameter someParameter. Type: 'one' | 'three' | 'two'. Declaration start char: 0, length: 165

param defaultExpression bool = 18 != (true || false)
//@[6:23) Parameter defaultExpression. Type: bool. Declaration start char: 0, length: 52

@allowed([
  'abc'
  'def'
])
param stringLiteral string
//@[6:19) Parameter stringLiteral. Type: 'abc' | 'def'. Declaration start char: 0, length: 56

@allowed([
  'abc'
  'def'
  'ghi'
])
param stringLiteralWithAllowedValuesSuperset string = stringLiteral
//@[6:44) Parameter stringLiteralWithAllowedValuesSuperset. Type: 'abc' | 'def' | 'ghi'. Declaration start char: 0, length: 105

@secure()
@minLength(2)
  @maxLength(10)
@allowed([
  'Apple'
  'Banana'
])
param decoratedString string
//@[6:21) Parameter decoratedString. Type: 'Apple' | 'Banana'. Declaration start char: 0, length: 104

@minValue(200)
param decoratedInt int = 123
//@[6:18) Parameter decoratedInt. Type: int. Declaration start char: 0, length: 43

// negative integer literals are allowed as decorator values
@minValue(-10)
@maxValue(-3)
param negativeValues int
//@[6:20) Parameter negativeValues. Type: int. Declaration start char: 0, length: 53

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
param decoratedBool bool = (true && false) != true
//@[6:19) Parameter decoratedBool. Type: bool. Declaration start char: 0, length: 219

@secure()
param decoratedObject object = {
//@[6:21) Parameter decoratedObject. Type: object. Declaration start char: 0, length: 254
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
    description: 'An array.'
})
@sys.maxLength(20)
@sys.description('I will be overrode.')
param decoratedArray array = [
//@[6:20) Parameter decoratedArray. Type: array. Declaration start char: 0, length: 166
    utcNow()
    newGuid()
]

