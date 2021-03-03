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
  'a'
  'b'
  'c'
]

// alternative array parameter
param myAlternativeArrayParam array {
//@[36:107) [BCP153 (Warning)] Parameter modifiers are deprecated and will be removed in a future release. Use decorators instead. |{\n  default: [\n    'a'\n    'b'\n    'c'\n    newGuid()\n    utcNow()\n  ]\n}|
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
//@[22:40) [BCP153 (Warning)] Parameter modifiers are deprecated and will be removed in a future release. Use decorators instead. |{\n  secure: true\n}|
  secure: true
}

@secure()
param passwordWithDecorator string

// non-secure string
param nonSecure string {
//@[23:42) [BCP153 (Warning)] Parameter modifiers are deprecated and will be removed in a future release. Use decorators instead. |{\n  secure: false\n}|
  secure: false
}

// secure object
param secretObject object {
//@[26:44) [BCP153 (Warning)] Parameter modifiers are deprecated and will be removed in a future release. Use decorators instead. |{\n  secure: true\n}|
  secure: true
}

@secure()
param secureObjectWithDecorator object

// enum parameter
param storageSku string {
//@[24:82) [BCP153 (Warning)] Parameter modifiers are deprecated and will be removed in a future release. Use decorators instead. |{\n  allowed: [\n    'Standard_LRS'\n    'Standard_GRS'\n  ]\n}|
  allowed: [
    'Standard_LRS'
    'Standard_GRS'
  ]
}

@  allowed([
'Standard_LRS'
'Standard_GRS'
])
param storageSkuWithDecorator string

// length constraint on a string
param storageName string {
//@[25:59) [BCP153 (Warning)] Parameter modifiers are deprecated and will be removed in a future release. Use decorators instead. |{\n  minLength: 3\n  maxLength: 24\n}|
  minLength: 3
  maxLength: 24
}

@minLength(3)
@maxLength(24)
param storageNameWithDecorator string

// length constraint on an array
param someArray array {
//@[22:56) [BCP153 (Warning)] Parameter modifiers are deprecated and will be removed in a future release. Use decorators instead. |{\n  minLength: 3\n  maxLength: 24\n}|
  minLength: 3
  maxLength: 24
}

@minLength(3)
@maxLength(24)
param someArrayWithDecorator array

// empty metadata
param emptyMetadata string {
//@[27:48) [BCP153 (Warning)] Parameter modifiers are deprecated and will be removed in a future release. Use decorators instead. |{\n  metadata: {\n  }\n}|
  metadata: {
  }
}

@metadata({})
param emptyMetadataWithDecorator string

// description
param description string {
//@[25:80) [BCP153 (Warning)] Parameter modifiers are deprecated and will be removed in a future release. Use decorators instead. |{\n  metadata: {\n    description: 'my description'\n  }\n}|
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
//@[32:156) [BCP153 (Warning)] Parameter modifiers are deprecated and will be removed in a future release. Use decorators instead. |{\n  metadata: {\n    description: 'my description'\n    a: 1\n    b: true\n    c: [\n    ]\n    d: {\n      test: 'abc'\n    }\n  }\n}|
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
//@[27:207) [BCP153 (Warning)] Parameter modifiers are deprecated and will be removed in a future release. Use decorators instead. |{\n  secure: true\n  minLength: 3\n  maxLength: 24\n  default: 'one'\n  allowed: [\n    'one'\n    'two'\n    'three'\n  ]\n  metadata: {\n    description: 'Name of the storage account'\n  }\n}|
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
param someParameterWithDecorator string = 'one'

param defaultValueExpression int {
//@[33:66) [BCP153 (Warning)] Parameter modifiers are deprecated and will be removed in a future release. Use decorators instead. |{\n  default: true ? 4 + 2*3 : 0\n}|
  default: true ? 4 + 2*3 : 0
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
@sys.description('I will be overrode.')
param decoratedArray array

