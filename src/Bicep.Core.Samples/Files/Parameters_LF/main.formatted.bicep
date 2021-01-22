/* 
  This is a block comment.
*/

// parameters without default value
param myString string
param myInt int
param myBool bool

// parameters with default value
param myString2 string = 'strin${2}g value'
param myInt2 int = 42
param myTruth bool = true
param myFalsehood bool = false
param myEscapedString string = 'First line\nSecond\ttabbed\tline'
param myNewGuid string = newGuid()
param myUtcTime string = utcNow()

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
  test: {
    time: utcNow('u')
    guid: newGuid()
  }
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
    newGuid()
    utcNow()
  ]
}

// secure string
param password string {
  secure: true
}

// non-secure string
param nonSecure string {
  secure: false
}

// secure object
param secretObject object {
  secure: true
}

// enum parameter
param storageSku string {
  allowed: [
    'Standard_LRS'
    'Standard_GRS'
  ]
}

// length constraint on a string
param storageName string {
  minLength: 3
  maxLength: 24
}

// length constraint on an array
param someArray array {
  minLength: 3
  maxLength: 24
}

// empty metadata
param emptyMetadata string {
  metadata: {}
}

// description
param description string {
  metadata: {
    description: 'my description'
  }
}

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

param defaultValueExpression int {
  default: true ? 4 + 2 * 3 : 0
}

param defaultExpression bool = 18 != (true || false)

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

@az.description('A boolean.')
@metadata({
  description: 'I will be overrode.'
  foo: 'something'
  bar: [
    {}
    true
    123
  ]
})
param decoratedBool bool

@secure()
@secure()
@secure()
param decoratedObject object = {
  location: 'westus'
}

@metadata({
  description: 'An array.'
})
@maxLength(20)
@maxLength(10)
@maxLength(5)
@az.description('I will be overrode.')
param decoratedArray array
