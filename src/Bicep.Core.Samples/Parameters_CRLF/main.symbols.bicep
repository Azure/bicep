/* 
  This is a block comment.
*/

// parameters without default value
param myString string
//@[6:14) Parameter myString. Declaration start char: 0, length: 23
param myInt int
//@[6:11) Parameter myInt. Declaration start char: 0, length: 17
param myBool bool
//@[6:12) Parameter myBool. Declaration start char: 0, length: 21

// parameters with default value
param myString2 string = 'string value'
//@[6:15) Parameter myString2. Declaration start char: 0, length: 41
param myInt2 int = 42
//@[6:12) Parameter myInt2. Declaration start char: 0, length: 23
param myTruth bool = true
//@[6:13) Parameter myTruth. Declaration start char: 0, length: 27
param myFalsehood bool = false
//@[6:17) Parameter myFalsehood. Declaration start char: 0, length: 32
param myEscapedString string = 'First line\r\nSecond\ttabbed\tline'
//@[6:21) Parameter myEscapedString. Declaration start char: 0, length: 71

// object default value
param foo object = {
//@[6:9) Parameter foo. Declaration start char: 0, length: 257
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
//@[6:18) Parameter myArrayParam. Declaration start char: 0, length: 56
  'a'
  'b'
  'c'
]

// alternative array parameter
param myAlternativeArrayParam array {
//@[6:29) Parameter myAlternativeArrayParam. Declaration start char: 0, length: 90
  default: [
    'a'
    'b'
    'c'
  ]
}

// secure string
param password string {
//@[6:14) Parameter password. Declaration start char: 0, length: 46
  secure: true
}

// non-secure string
param nonSecure string {
//@[6:15) Parameter nonSecure. Declaration start char: 0, length: 48
  secure: false
}

// secure object
param secretObject object {
//@[6:18) Parameter secretObject. Declaration start char: 0, length: 50
  secure: true
}

// enum parameter
param storageSku string {
//@[6:16) Parameter storageSku. Declaration start char: 0, length: 97
  allowedValues: [
    'Standard_LRS'
    'Standard_GRS'
  ]
}

// length constraint on a string
param storageName string {
//@[6:17) Parameter storageName. Declaration start char: 0, length: 66
  minLength: 3
  maxLength: 24
}

// length constraint on an array
param someArray array {
//@[6:15) Parameter someArray. Declaration start char: 0, length: 63
  minLength: 3
  maxLength: 24
}

// empty metadata
param emptyMetadata string {
//@[6:19) Parameter emptyMetadata. Declaration start char: 0, length: 55
  metadata: {
  }
}

// description
param description string {
//@[6:17) Parameter description. Declaration start char: 0, length: 88
  metadata: {
    description: 'my description'
  }
}

// random extra metadata
param additionalMetadata string {
//@[6:24) Parameter additionalMetadata. Declaration start char: 0, length: 171
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
//@[6:19) Parameter someParameter. Declaration start char: 0, length: 230
  secure: true
  minLength: 3
  maxLength: 24
  default: 'one'
  allowedValues: [
    'one'
    'two'
    'three'
  ]
  metadata: {
    description: 'Name of the storage account'
  }
}

param defaultValueExpression int {
//@[6:28) Parameter defaultValueExpression. Declaration start char: 0, length: 72
  default: true ? 4 + 2*3 : 0
}

param defaultExpression bool = 18 != (true || false)
//@[6:23) Parameter defaultExpression. Declaration start char: 0, length: 54

