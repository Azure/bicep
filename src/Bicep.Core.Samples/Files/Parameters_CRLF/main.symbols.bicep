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

// enum parameter
param storageSku string {
//@[6:16) Parameter storageSku. Type: 'Standard_GRS' | 'Standard_LRS'. Declaration start char: 0, length: 87
  allowed: [
    'Standard_LRS'
    'Standard_GRS'
  ]
}

// length constraint on a string
param storageName string {
//@[6:17) Parameter storageName. Type: string. Declaration start char: 0, length: 62
  minLength: 3
  maxLength: 24
}

// length constraint on an array
param someArray array {
//@[6:15) Parameter someArray. Type: array. Declaration start char: 0, length: 59
  minLength: 3
  maxLength: 24
}

// empty metadata
param emptyMetadata string {
//@[6:19) Parameter emptyMetadata. Type: string. Declaration start char: 0, length: 51
  metadata: {
  }
}

// description
param description string {
//@[6:17) Parameter description. Type: string. Declaration start char: 0, length: 84
  metadata: {
    description: 'my description'
  }
}

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

