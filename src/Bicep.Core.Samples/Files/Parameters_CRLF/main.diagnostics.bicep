/* 
  This is a block comment.
*/

// parameters without default value
param myString string
//@[6:14) [no-unused-params (Warning)] Declared parameter must be referenced within the document scope.\r\n[See : https://aka.ms/bicep/linter/no-unused-params] |myString|
param myInt int
//@[6:11) [no-unused-params (Warning)] Declared parameter must be referenced within the document scope.\r\n[See : https://aka.ms/bicep/linter/no-unused-params] |myInt|
param myBool bool
//@[6:12) [no-unused-params (Warning)] Declared parameter must be referenced within the document scope.\r\n[See : https://aka.ms/bicep/linter/no-unused-params] |myBool|

// parameters with default value
param myString2 string = 'string value'
//@[6:15) [no-unused-params (Warning)] Declared parameter must be referenced within the document scope.\r\n[See : https://aka.ms/bicep/linter/no-unused-params] |myString2|
param myInt2 int = 42
//@[6:12) [no-unused-params (Warning)] Declared parameter must be referenced within the document scope.\r\n[See : https://aka.ms/bicep/linter/no-unused-params] |myInt2|
param myTruth bool = true
//@[6:13) [no-unused-params (Warning)] Declared parameter must be referenced within the document scope.\r\n[See : https://aka.ms/bicep/linter/no-unused-params] |myTruth|
param myFalsehood bool = false
//@[6:17) [no-unused-params (Warning)] Declared parameter must be referenced within the document scope.\r\n[See : https://aka.ms/bicep/linter/no-unused-params] |myFalsehood|
param myEscapedString string = 'First line\r\nSecond\ttabbed\tline'
//@[6:21) [no-unused-params (Warning)] Declared parameter must be referenced within the document scope.\r\n[See : https://aka.ms/bicep/linter/no-unused-params] |myEscapedString|

// object default value
param foo object = {
//@[6:9) [no-unused-params (Warning)] Declared parameter must be referenced within the document scope.\r\n[See : https://aka.ms/bicep/linter/no-unused-params] |foo|
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
//@[6:18) [no-unused-params (Warning)] Declared parameter must be referenced within the document scope.\r\n[See : https://aka.ms/bicep/linter/no-unused-params] |myArrayParam|
  'a'
  'b'
  'c'
]

// alternative array parameter
param myAlternativeArrayParam array {
//@[6:29) [no-unused-params (Warning)] Declared parameter must be referenced within the document scope.\r\n[See : https://aka.ms/bicep/linter/no-unused-params] |myAlternativeArrayParam|
//@[36:86) [BCP161 (Info)] Parameter modifiers are deprecated and will be removed in a future release. Use decorators instead (see https://aka.ms/BicepSpecParams for examples). |{\r\n  default: [\r\n    'a'\r\n    'b'\r\n    'c'\r\n  ]\r\n}|
  default: [
    'a'
    'b'
    'c'
  ]
}

// secure string
param password string {
//@[6:14) [no-unused-params (Warning)] Declared parameter must be referenced within the document scope.\r\n[See : https://aka.ms/bicep/linter/no-unused-params] |password|
//@[22:42) [BCP161 (Info)] Parameter modifiers are deprecated and will be removed in a future release. Use decorators instead (see https://aka.ms/BicepSpecParams for examples). |{\r\n  secure: true\r\n}|
  secure: true
}

@secure()
param passwordWithDecorator string
//@[6:27) [no-unused-params (Warning)] Declared parameter must be referenced within the document scope.\r\n[See : https://aka.ms/bicep/linter/no-unused-params] |passwordWithDecorator|

// non-secure string
param nonSecure string {
//@[6:15) [no-unused-params (Warning)] Declared parameter must be referenced within the document scope.\r\n[See : https://aka.ms/bicep/linter/no-unused-params] |nonSecure|
//@[23:44) [BCP161 (Info)] Parameter modifiers are deprecated and will be removed in a future release. Use decorators instead (see https://aka.ms/BicepSpecParams for examples). |{\r\n  secure: false\r\n}|
  secure: false
}

// secure object
param secretObject object {
//@[6:18) [no-unused-params (Warning)] Declared parameter must be referenced within the document scope.\r\n[See : https://aka.ms/bicep/linter/no-unused-params] |secretObject|
//@[26:46) [BCP161 (Info)] Parameter modifiers are deprecated and will be removed in a future release. Use decorators instead (see https://aka.ms/BicepSpecParams for examples). |{\r\n  secure: true\r\n}|
  secure: true
}

@secure()
param secretObjectWithDecorator object
//@[6:31) [no-unused-params (Warning)] Declared parameter must be referenced within the document scope.\r\n[See : https://aka.ms/bicep/linter/no-unused-params] |secretObjectWithDecorator|

// enum parameter
param storageSku string {
//@[6:16) [no-unused-params (Warning)] Declared parameter must be referenced within the document scope.\r\n[See : https://aka.ms/bicep/linter/no-unused-params] |storageSku|
//@[24:87) [BCP161 (Info)] Parameter modifiers are deprecated and will be removed in a future release. Use decorators instead (see https://aka.ms/BicepSpecParams for examples). |{\r\n  allowed: [\r\n    'Standard_LRS'\r\n    'Standard_GRS'\r\n  ]\r\n}|
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
//@[6:29) [no-unused-params (Warning)] Declared parameter must be referenced within the document scope.\r\n[See : https://aka.ms/bicep/linter/no-unused-params] |storageSkuWithDecorator|

// length constraint on a string
param storageName string {
//@[6:17) [no-unused-params (Warning)] Declared parameter must be referenced within the document scope.\r\n[See : https://aka.ms/bicep/linter/no-unused-params] |storageName|
//@[25:62) [BCP161 (Info)] Parameter modifiers are deprecated and will be removed in a future release. Use decorators instead (see https://aka.ms/BicepSpecParams for examples). |{\r\n  minLength: 3\r\n  maxLength: 24\r\n}|
  minLength: 3
  maxLength: 24
}

@minLength(3)
@maxLength(24)
param storageNameWithDecorator string
//@[6:30) [no-unused-params (Warning)] Declared parameter must be referenced within the document scope.\r\n[See : https://aka.ms/bicep/linter/no-unused-params] |storageNameWithDecorator|

// length constraint on an array
param someArray array {
//@[6:15) [no-unused-params (Warning)] Declared parameter must be referenced within the document scope.\r\n[See : https://aka.ms/bicep/linter/no-unused-params] |someArray|
//@[22:59) [BCP161 (Info)] Parameter modifiers are deprecated and will be removed in a future release. Use decorators instead (see https://aka.ms/BicepSpecParams for examples). |{\r\n  minLength: 3\r\n  maxLength: 24\r\n}|
  minLength: 3
  maxLength: 24
}

@minLength(3)
@maxLength(24)
param someArrayWithDecorator array
//@[6:28) [no-unused-params (Warning)] Declared parameter must be referenced within the document scope.\r\n[See : https://aka.ms/bicep/linter/no-unused-params] |someArrayWithDecorator|

// empty metadata
param emptyMetadata string {
//@[6:19) [no-unused-params (Warning)] Declared parameter must be referenced within the document scope.\r\n[See : https://aka.ms/bicep/linter/no-unused-params] |emptyMetadata|
//@[27:51) [BCP161 (Info)] Parameter modifiers are deprecated and will be removed in a future release. Use decorators instead (see https://aka.ms/BicepSpecParams for examples). |{\r\n  metadata: {\r\n  }\r\n}|
  metadata: {
  }
}

@metadata({})
param emptyMetadataWithDecorator string
//@[6:32) [no-unused-params (Warning)] Declared parameter must be referenced within the document scope.\r\n[See : https://aka.ms/bicep/linter/no-unused-params] |emptyMetadataWithDecorator|

// description
param description string {
//@[6:17) [no-unused-params (Warning)] Declared parameter must be referenced within the document scope.\r\n[See : https://aka.ms/bicep/linter/no-unused-params] |description|
//@[25:84) [BCP161 (Info)] Parameter modifiers are deprecated and will be removed in a future release. Use decorators instead (see https://aka.ms/BicepSpecParams for examples). |{\r\n  metadata: {\r\n    description: 'my description'\r\n  }\r\n}|
  metadata: {
    description: 'my description'
  }
}

@metadata({
  description: 'my description'
})
param descriptionWithDecorator string
//@[6:30) [no-unused-params (Warning)] Declared parameter must be referenced within the document scope.\r\n[See : https://aka.ms/bicep/linter/no-unused-params] |descriptionWithDecorator|

@sys.description('my description')
param descriptionWithDecorator2 string
//@[6:31) [no-unused-params (Warning)] Declared parameter must be referenced within the document scope.\r\n[See : https://aka.ms/bicep/linter/no-unused-params] |descriptionWithDecorator2|

// random extra metadata
param additionalMetadata string {
//@[6:24) [no-unused-params (Warning)] Declared parameter must be referenced within the document scope.\r\n[See : https://aka.ms/bicep/linter/no-unused-params] |additionalMetadata|
//@[32:167) [BCP161 (Info)] Parameter modifiers are deprecated and will be removed in a future release. Use decorators instead (see https://aka.ms/BicepSpecParams for examples). |{\r\n  metadata: {\r\n    description: 'my description'\r\n    a: 1\r\n    b: true\r\n    c: [\r\n    ]\r\n    d: {\r\n      test: 'abc'\r\n    }\r\n  }\r\n}|
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
//@[6:37) [no-unused-params (Warning)] Declared parameter must be referenced within the document scope.\r\n[See : https://aka.ms/bicep/linter/no-unused-params] |additionalMetadataWithDecorator|

// all modifiers together
param someParameter string {
//@[6:19) [no-unused-params (Warning)] Declared parameter must be referenced within the document scope.\r\n[See : https://aka.ms/bicep/linter/no-unused-params] |someParameter|
//@[27:220) [BCP161 (Info)] Parameter modifiers are deprecated and will be removed in a future release. Use decorators instead (see https://aka.ms/BicepSpecParams for examples). |{\r\n  secure: true\r\n  minLength: 3\r\n  maxLength: 24\r\n  default: 'one'\r\n  allowed: [\r\n    'one'\r\n    'two'\r\n    'three'\r\n  ]\r\n  metadata: {\r\n    description: 'Name of the storage account'\r\n  }\r\n}|
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
//@[6:32) [no-unused-params (Warning)] Declared parameter must be referenced within the document scope.\r\n[See : https://aka.ms/bicep/linter/no-unused-params] |someParameterWithDecorator|

param defaultValueExpression int {
//@[6:28) [no-unused-params (Warning)] Declared parameter must be referenced within the document scope.\r\n[See : https://aka.ms/bicep/linter/no-unused-params] |defaultValueExpression|
//@[33:68) [BCP161 (Info)] Parameter modifiers are deprecated and will be removed in a future release. Use decorators instead (see https://aka.ms/BicepSpecParams for examples). |{\r\n  default: true ? 4 + 2*3 : 0\r\n}|
  default: true ? 4 + 2*3 : 0
}

param defaultExpression bool = 18 != (true || false)
//@[6:23) [no-unused-params (Warning)] Declared parameter must be referenced within the document scope.\r\n[See : https://aka.ms/bicep/linter/no-unused-params] |defaultExpression|

param stringLiteral string {
//@[27:72) [BCP161 (Info)] Parameter modifiers are deprecated and will be removed in a future release. Use decorators instead (see https://aka.ms/BicepSpecParams for examples). |{\r\n  allowed: [\r\n    'abc'\r\n    'def'\r\n  ]\r\n}|
  allowed: [
    'abc'
    'def'
  ]
}

param stringLiteralWithAllowedValuesSuperset string {
//@[6:44) [no-unused-params (Warning)] Declared parameter must be referenced within the document scope.\r\n[See : https://aka.ms/bicep/linter/no-unused-params] |stringLiteralWithAllowedValuesSuperset|
//@[52:134) [BCP161 (Info)] Parameter modifiers are deprecated and will be removed in a future release. Use decorators instead (see https://aka.ms/BicepSpecParams for examples). |{\r\n  allowed: [\r\n    'abc'\r\n    'def'\r\n    'ghi'\r\n  ]\r\n  default: stringLiteral\r\n}|
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
//@[6:57) [no-unused-params (Warning)] Declared parameter must be referenced within the document scope.\r\n[See : https://aka.ms/bicep/linter/no-unused-params] |stringLiteralWithDecoratorWithAllowedValuesSuperset|

@secure()
@minLength(2)
  @maxLength(10)
@allowed([
  'Apple'
  'Banana'
])
param decoratedString string
//@[6:21) [no-unused-params (Warning)] Declared parameter must be referenced within the document scope.\r\n[See : https://aka.ms/bicep/linter/no-unused-params] |decoratedString|

@minValue(200)
param decoratedInt int = 123
//@[6:18) [no-unused-params (Warning)] Declared parameter must be referenced within the document scope.\r\n[See : https://aka.ms/bicep/linter/no-unused-params] |decoratedInt|

// negative integer literals are allowed as decorator values
@minValue(-10)
@maxValue(-3)
param negativeValues int
//@[6:20) [no-unused-params (Warning)] Declared parameter must be referenced within the document scope.\r\n[See : https://aka.ms/bicep/linter/no-unused-params] |negativeValues|

// negative integer literals in modifiers
param negativeModifiers int {
//@[6:23) [no-unused-params (Warning)] Declared parameter must be referenced within the document scope.\r\n[See : https://aka.ms/bicep/linter/no-unused-params] |negativeModifiers|
//@[28:67) [BCP161 (Info)] Parameter modifiers are deprecated and will be removed in a future release. Use decorators instead (see https://aka.ms/BicepSpecParams for examples). |{\r\n  minValue: -100\r\n  maxValue: -33\r\n}|
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
//@[6:19) [no-unused-params (Warning)] Declared parameter must be referenced within the document scope.\r\n[See : https://aka.ms/bicep/linter/no-unused-params] |decoratedBool|

@secure()
param decoratedObject object = {
//@[6:21) [no-unused-params (Warning)] Declared parameter must be referenced within the document scope.\r\n[See : https://aka.ms/bicep/linter/no-unused-params] |decoratedObject|
//@[29:265) [secure-paramenter-default (Warning)] Secure parameters can't have hardcoded default. This prevents storage of sensitive data in the Bicep declaration.\r\n[See : https://aka.ms/bicep/linter/secure-paramenter-default] |= {\r\n  enabled: true\r\n  name: 'this is my object'\r\n  priority: 3\r\n  info: {\r\n    a: 'b'\r\n  }\r\n  empty: {\r\n  }\r\n  array: [\r\n    'string item'\r\n    12\r\n    true\r\n    [\r\n      'inner'\r\n      false\r\n    ]\r\n    {\r\n      a: 'b'\r\n    }\r\n  ]\r\n}|
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
//@[6:20) [no-unused-params (Warning)] Declared parameter must be referenced within the document scope.\r\n[See : https://aka.ms/bicep/linter/no-unused-params] |decoratedArray|
    utcNow()
    newGuid()
]

