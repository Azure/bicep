/* 
  This is a block comment.
*/

// parameters without default value
parameter myString string
parameter myInt int
parameter myBool bool

// parameters with default value
parameter myString2 string = 'strin${2}g value'
parameter myInt2 int = 42
parameter myTruth bool = true
parameter myFalsehood bool = false
parameter myEscapedString string = 'First line\nSecond\ttabbed\tline'

// object default value
parameter foo object = {
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
parameter myArrayParam array = [
  'a'
  'b'
  'c'
]

// alternative array parameter
parameter myAlternativeArrayParam array {
  defaultValue: [
    'a'
    'b'
    'c'
  ]
}

// secure string
parameter password string {
  secure: true
}

// non-secure string
parameter nonSecure string {
  secure: false
}

// secure object
parameter secretObject object {
  secure: true
}

// enum parameter
parameter storageSku string {
  allowedValues: [
    'Standard_LRS'
    'Standard_GRS'
  ]
}

// length constraint on a string
parameter storageName string {
  minLength: 3
  maxLength: 24
}

// length constraint on an array
parameter someArray array {
  minLength: 3
  maxLength: 24
}

// empty metadata
parameter emptyMetadata string {
  metadata: {
  }
}

// description
parameter description string {
  metadata: {
    description: 'my description'
  }
}

// random extra metadata
parameter additionalMetadata string {
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
parameter someParameter string {
  secure: true
  minLength: 3
  maxLength: 24
  defaultValue: 'one'
  allowedValues: [
    'one'
    'two'
    'three'
  ]
  metadata: {
    description: 'Name of the storage account'
  }
}

parameter defaultValueExpression int {
  defaultValue: true ? 4 + 2*3 : 0
}

parameter defaultExpression bool = 18 != (true || false)
