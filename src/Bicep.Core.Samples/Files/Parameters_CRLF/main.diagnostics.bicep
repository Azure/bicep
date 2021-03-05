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
  'a'
  'b'
  'c'
]

// alternative array parameter
param myAlternativeArrayParam array {
//@[36:86) [BCP156 (Info)] Parameter modifiers are deprecated and will be removed in a future release. Use decorators instead (see https://aka.ms/BicepSpecParams for examples). |{\r\n  default: [\r\n    'a'\r\n    'b'\r\n    'c'\r\n  ]\r\n}|
  default: [
    'a'
    'b'
    'c'
  ]
}

// secure string
param password string {
//@[22:42) [BCP156 (Info)] Parameter modifiers are deprecated and will be removed in a future release. Use decorators instead (see https://aka.ms/BicepSpecParams for examples). |{\r\n  secure: true\r\n}|
  secure: true
}

@secure()
param passwordWithDecorator string

// non-secure string
param nonSecure string {
//@[23:44) [BCP156 (Info)] Parameter modifiers are deprecated and will be removed in a future release. Use decorators instead (see https://aka.ms/BicepSpecParams for examples). |{\r\n  secure: false\r\n}|
  secure: false
}

// secure object
param secretObject object {
//@[26:46) [BCP156 (Info)] Parameter modifiers are deprecated and will be removed in a future release. Use decorators instead (see https://aka.ms/BicepSpecParams for examples). |{\r\n  secure: true\r\n}|
  secure: true
}

@secure()
param secretObjectWithDecorator object

// enum parameter
param storageSku string {
//@[24:87) [BCP156 (Info)] Parameter modifiers are deprecated and will be removed in a future release. Use decorators instead (see https://aka.ms/BicepSpecParams for examples). |{\r\n  allowed: [\r\n    'Standard_LRS'\r\n    'Standard_GRS'\r\n  ]\r\n}|
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
//@[25:62) [BCP156 (Info)] Parameter modifiers are deprecated and will be removed in a future release. Use decorators instead (see https://aka.ms/BicepSpecParams for examples). |{\r\n  minLength: 3\r\n  maxLength: 24\r\n}|
  minLength: 3
  maxLength: 24
}

@minLength(3)
@maxLength(24)
param storageNameWithDecorator string

// length constraint on an array
param someArray array {
//@[22:59) [BCP156 (Info)] Parameter modifiers are deprecated and will be removed in a future release. Use decorators instead (see https://aka.ms/BicepSpecParams for examples). |{\r\n  minLength: 3\r\n  maxLength: 24\r\n}|
  minLength: 3
  maxLength: 24
}

@minLength(3)
@maxLength(24)
param someArrayWithDecorator array

// empty metadata
param emptyMetadata string {
//@[27:51) [BCP156 (Info)] Parameter modifiers are deprecated and will be removed in a future release. Use decorators instead (see https://aka.ms/BicepSpecParams for examples). |{\r\n  metadata: {\r\n  }\r\n}|
  metadata: {
  }
}

@metadata({})
param emptyMetadataWithDecorator string

// description
param description string {
//@[25:84) [BCP156 (Info)] Parameter modifiers are deprecated and will be removed in a future release. Use decorators instead (see https://aka.ms/BicepSpecParams for examples). |{\r\n  metadata: {\r\n    description: 'my description'\r\n  }\r\n}|
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
//@[32:167) [BCP156 (Info)] Parameter modifiers are deprecated and will be removed in a future release. Use decorators instead (see https://aka.ms/BicepSpecParams for examples). |{\r\n  metadata: {\r\n    description: 'my description'\r\n    a: 1\r\n    b: true\r\n    c: [\r\n    ]\r\n    d: {\r\n      test: 'abc'\r\n    }\r\n  }\r\n}|
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

// all modifiers together
param someParameter string {
//@[27:220) [BCP156 (Info)] Parameter modifiers are deprecated and will be removed in a future release. Use decorators instead (see https://aka.ms/BicepSpecParams for examples). |{\r\n  secure: true\r\n  minLength: 3\r\n  maxLength: 24\r\n  default: 'one'\r\n  allowed: [\r\n    'one'\r\n    'two'\r\n    'three'\r\n  ]\r\n  metadata: {\r\n    description: 'Name of the storage account'\r\n  }\r\n}|
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
//@[33:68) [BCP156 (Info)] Parameter modifiers are deprecated and will be removed in a future release. Use decorators instead (see https://aka.ms/BicepSpecParams for examples). |{\r\n  default: true ? 4 + 2*3 : 0\r\n}|
  default: true ? 4 + 2*3 : 0
}

param defaultExpression bool = 18 != (true || false)

param stringLiteral string {
//@[27:72) [BCP156 (Info)] Parameter modifiers are deprecated and will be removed in a future release. Use decorators instead (see https://aka.ms/BicepSpecParams for examples). |{\r\n  allowed: [\r\n    'abc'\r\n    'def'\r\n  ]\r\n}|
  allowed: [
    'abc'
    'def'
  ]
}

param stringLiteralWithAllowedValuesSuperset string {
//@[52:134) [BCP156 (Info)] Parameter modifiers are deprecated and will be removed in a future release. Use decorators instead (see https://aka.ms/BicepSpecParams for examples). |{\r\n  allowed: [\r\n    'abc'\r\n    'def'\r\n    'ghi'\r\n  ]\r\n  default: stringLiteral\r\n}|
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
    utcNow()
    newGuid()
]

