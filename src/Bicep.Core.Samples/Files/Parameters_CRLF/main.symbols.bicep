/* 
  This is a block comment.
*/

// parameters without default value
param myString string
//@[6:14) Parameter myString. Type: string. Declaration start char: 0, length: 21
param myInt int
//@[6:11) Parameter myInt. Type: int. Declaration start char: 0, length: 15
param myBool bool
//@[6:12) Parameter myBool. Type: bool. Declaration start char: 0, length: 17

// parameters with default value
param myString2 string = 'string value'
//@[6:15) Parameter myString2. Type: string. Declaration start char: 0, length: 39
param myInt2 int = 42
//@[6:12) Parameter myInt2. Type: int. Declaration start char: 0, length: 21
param myTruth bool = true
//@[6:13) Parameter myTruth. Type: bool. Declaration start char: 0, length: 25
param myFalsehood bool = false
//@[6:17) Parameter myFalsehood. Type: bool. Declaration start char: 0, length: 30
param myEscapedString string = 'First line\r\nSecond\ttabbed\tline'
//@[6:21) Parameter myEscapedString. Type: string. Declaration start char: 0, length: 67

// object default value
param foo object = {
//@[6:9) Parameter foo. Type: object. Declaration start char: 0, length: 253
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
//@[6:18) Parameter myArrayParam. Type: array. Declaration start char: 0, length: 52
  'a'
  'b'
  'c'
]

// alternative array parameter
param myAlternativeArrayParam array {
//@[6:29) Parameter myAlternativeArrayParam. Type: array. Declaration start char: 0, length: 86
  default: [
    'a'
    'b'
    'c'
  ]
}

// secure string
param password string {
//@[6:14) Parameter password. Type: string. Declaration start char: 0, length: 42
  secure: true
}

@secure()
param passwordWithDecorator string
//@[6:27) Parameter passwordWithDecorator. Type: string. Declaration start char: 0, length: 45

// non-secure string
param nonSecure string {
//@[6:15) Parameter nonSecure. Type: string. Declaration start char: 0, length: 44
  secure: false
}

// secure object
param secretObject object {
//@[6:18) Parameter secretObject. Type: object. Declaration start char: 0, length: 46
  secure: true
}

@secure()
param secretObjectWithDecorator object
//@[6:31) Parameter secretObjectWithDecorator. Type: object. Declaration start char: 0, length: 49

// enum parameter
param storageSku string {
//@[6:16) Parameter storageSku. Type: 'Standard_GRS' | 'Standard_LRS'. Declaration start char: 0, length: 87
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
//@[6:29) Parameter storageSkuWithDecorator. Type: 'Standard_GRS' | 'Standard_LRS'. Declaration start char: 0, length: 88

// length constraint on a string
param storageName string {
//@[6:17) Parameter storageName. Type: string. Declaration start char: 0, length: 62
  minLength: 3
  maxLength: 24
}

@minLength(3)
@maxLength(24)
param storageNameWithDecorator string
//@[6:30) Parameter storageNameWithDecorator. Type: string. Declaration start char: 0, length: 68

// length constraint on an array
param someArray array {
//@[6:15) Parameter someArray. Type: array. Declaration start char: 0, length: 59
  minLength: 3
  maxLength: 24
}

@minLength(3)
@maxLength(24)
param someArrayWithDecorator array
//@[6:28) Parameter someArrayWithDecorator. Type: array. Declaration start char: 0, length: 65

// empty metadata
param emptyMetadata string {
//@[6:19) Parameter emptyMetadata. Type: string. Declaration start char: 0, length: 51
  metadata: {
  }
}

@metadata({})
param emptyMetadataWithDecorator string
//@[6:32) Parameter emptyMetadataWithDecorator. Type: string. Declaration start char: 0, length: 54

// description
param description string {
//@[6:17) Parameter description. Type: string. Declaration start char: 0, length: 84
  metadata: {
    description: 'my description'
  }
}

@metadata({
  description: 'my description'
})
param descriptionWithDecorator string
//@[6:30) Parameter descriptionWithDecorator. Type: string. Declaration start char: 0, length: 87

@sys.description('my description')
param descriptionWithDecorator2 string
//@[6:31) Parameter descriptionWithDecorator2. Type: string. Declaration start char: 0, length: 74

// random extra metadata
param additionalMetadata string {
//@[6:24) Parameter additionalMetadata. Type: string. Declaration start char: 0, length: 167
  metadata: {
    description: 'my description'
    a: 1
    b: true
    c: [
    ]
    d: {
      test: 'abc'
    }
  }
}

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
param additionalMetadataWithDecorator string
//@[6:37) Parameter additionalMetadataWithDecorator. Type: string. Declaration start char: 0, length: 156

// all modifiers together
param someParameter string {
//@[6:19) Parameter someParameter. Type: 'one' | 'three' | 'two'. Declaration start char: 0, length: 220
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
//@[6:32) Parameter someParameterWithDecorator. Type: 'one' | 'three' | 'two'. Declaration start char: 0, length: 189

param defaultValueExpression int {
//@[6:28) Parameter defaultValueExpression. Type: int. Declaration start char: 0, length: 68
  default: true ? 4 + 2*3 : 0
}

param defaultExpression bool = 18 != (true || false)
//@[6:23) Parameter defaultExpression. Type: bool. Declaration start char: 0, length: 52

param stringLiteral string {
//@[6:19) Parameter stringLiteral. Type: 'abc' | 'def'. Declaration start char: 0, length: 72
  allowed: [
    'abc'
    'def'
  ]
}

param stringLiteralWithAllowedValuesSuperset string {
//@[6:44) Parameter stringLiteralWithAllowedValuesSuperset. Type: 'abc' | 'def' | 'ghi'. Declaration start char: 0, length: 134
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
//@[6:32) Parameter stringLiteralWithDecorator. Type: 'abc' | 'def'. Declaration start char: 0, length: 73

@allowed([
  'abc'
  'def'
  'ghi'
])
param stringLiteralWithDecoratorWithAllowedValuesSuperset string = stringLiteralWithDecorator
//@[6:57) Parameter stringLiteralWithDecoratorWithAllowedValuesSuperset. Type: 'abc' | 'def' | 'ghi'. Declaration start char: 0, length: 136

@secure()
@minLength(2)
  @maxLength(10)
@allowed([
  'Apple'
  'Banana'
])
param decoratedString string
//@[6:21) Parameter decoratedString. Type: 'Apple' | 'Banana'. Declaration start char: 0, length: 111

@minValue(200)
param decoratedInt int = 123
//@[6:18) Parameter decoratedInt. Type: int. Declaration start char: 0, length: 44

// negative integer literals are allowed as decorator values
@minValue(-10)
@maxValue(-3)
param negativeValues int
//@[6:20) Parameter negativeValues. Type: int. Declaration start char: 0, length: 55

// negative integer literals in modifiers
param negativeModifiers int {
//@[6:23) Parameter negativeModifiers. Type: int. Declaration start char: 0, length: 67
  minValue: -100
  maxValue: -33
}

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
//@[6:19) Parameter decoratedBool. Type: bool. Declaration start char: 0, length: 229

@secure()
@secure()
@secure()
param decoratedObject object = {
//@[6:21) Parameter decoratedObject. Type: object. Declaration start char: 0, length: 298
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
@maxLength(10)
@maxLength(5)
@sys.description('I will be overrode.')
param decoratedArray array = [
//@[6:20) Parameter decoratedArray. Type: array. Declaration start char: 0, length: 205
    utcNow()
    newGuid()
]

