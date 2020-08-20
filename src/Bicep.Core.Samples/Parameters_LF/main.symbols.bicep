/* 
  This is a block comment.
*/

// parameters without default value
param myString string
//@[6:14] Parameter myString
param myInt int
//@[6:11] Parameter myInt
param myBool bool
//@[6:12] Parameter myBool

// parameters with default value
param myString2 string = 'strin${2}g value'
//@[6:15] Parameter myString2
param myInt2 int = 42
//@[6:12] Parameter myInt2
param myTruth bool = true
//@[6:13] Parameter myTruth
param myFalsehood bool = false
//@[6:17] Parameter myFalsehood
param myEscapedString string = 'First line\nSecond\ttabbed\tline'
//@[6:21] Parameter myEscapedString
param myNewGuid string = newGuid()
//@[6:15] Parameter myNewGuid
param myUtcTime string = utcNow()
//@[6:15] Parameter myUtcTime

// object default value
param foo object = {
//@[6:9] Parameter foo
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
  test: {
    time: utcNow('u')
    guid: newGuid()
  }
}

// array default value
param myArrayParam array = [
//@[6:18] Parameter myArrayParam
  'a'
  'b'
  'c'
]

// alternative array parameter
param myAlternativeArrayParam array {
//@[6:29] Parameter myAlternativeArrayParam
  default: [
    'a'
    'b'
    'c'
    newGuid()
    utcNow()
  ]
}

// secure string
param password string {
//@[6:14] Parameter password
  secure: true
}

// non-secure string
param nonSecure string {
//@[6:15] Parameter nonSecure
  secure: false
}

// secure object
param secretObject object {
//@[6:18] Parameter secretObject
  secure: true
}

// enum parameter
param storageSku string {
//@[6:16] Parameter storageSku
  allowedValues: [
    'Standard_LRS'
    'Standard_GRS'
  ]
}

// length constraint on a string
param storageName string {
//@[6:17] Parameter storageName
  minLength: 3
  maxLength: 24
}

// length constraint on an array
param someArray array {
//@[6:15] Parameter someArray
  minLength: 3
  maxLength: 24
}

// empty metadata
param emptyMetadata string {
//@[6:19] Parameter emptyMetadata
  metadata: {
  }
}

// description
param description string {
//@[6:17] Parameter description
  metadata: {
    description: 'my description'
  }
}

// random extra metadata
param additionalMetadata string {
//@[6:24] Parameter additionalMetadata
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
//@[6:19] Parameter someParameter
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
//@[6:28] Parameter defaultValueExpression
  default: true ? 4 + 2*3 : 0
}

param defaultExpression bool = 18 != (true || false)
//@[6:23] Parameter defaultExpression

