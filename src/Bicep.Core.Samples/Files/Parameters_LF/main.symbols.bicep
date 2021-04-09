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
param myString2 string = 'strin${2}g value'
//@[6:15) Parameter myString2. Type: string. Declaration start char: 0, length: 43
param myInt2 int = 42
//@[6:12) Parameter myInt2. Type: int. Declaration start char: 0, length: 21
param myTruth bool = true
//@[6:13) Parameter myTruth. Type: bool. Declaration start char: 0, length: 25
param myFalsehood bool = false
//@[6:17) Parameter myFalsehood. Type: bool. Declaration start char: 0, length: 30
param myEscapedString string = 'First line\nSecond\ttabbed\tline'
//@[6:21) Parameter myEscapedString. Type: string. Declaration start char: 0, length: 65
param myNewGuid string = newGuid()
//@[6:15) Parameter myNewGuid. Type: string. Declaration start char: 0, length: 34
param myUtcTime string = utcNow()
//@[6:15) Parameter myUtcTime. Type: string. Declaration start char: 0, length: 33

// object default value
param foo object = {
//@[6:9) Parameter foo. Type: object. Declaration start char: 0, length: 288
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
//@[6:18) Parameter myArrayParam. Type: array. Declaration start char: 0, length: 48
  'a'
  'b'
  'c'
]

// alternative array parameter
param myAlternativeArrayParam array {
//@[6:29) Parameter myAlternativeArrayParam. Type: array. Declaration start char: 0, length: 107
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
//@[6:14) Parameter password. Type: string. Declaration start char: 0, length: 40
  secure: true
}

@secure()
param passwordWithDecorator string
//@[6:27) Parameter passwordWithDecorator. Type: string. Declaration start char: 0, length: 44

// non-secure string
param nonSecure string {
//@[6:15) Parameter nonSecure. Type: string. Declaration start char: 0, length: 42
  secure: false
}

// secure object
param secretObject object {
//@[6:18) Parameter secretObject. Type: object. Declaration start char: 0, length: 44
  secure: true
}

@secure()
param secureObjectWithDecorator object
//@[6:31) Parameter secureObjectWithDecorator. Type: object. Declaration start char: 0, length: 48

// enum parameter
param storageSku string {
//@[6:16) Parameter storageSku. Type: 'Standard_GRS' | 'Standard_LRS'. Declaration start char: 0, length: 82
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
//@[6:29) Parameter storageSkuWithDecorator. Type: 'Standard_GRS' | 'Standard_LRS'. Declaration start char: 0, length: 82

// length constraint on a string
param storageName string {
//@[6:17) Parameter storageName. Type: string. Declaration start char: 0, length: 59
  minLength: 3
  maxLength: 24
}

@minLength(3)
@maxLength(24)
param storageNameWithDecorator string
//@[6:30) Parameter storageNameWithDecorator. Type: string. Declaration start char: 0, length: 66

// length constraint on an array
param someArray array {
//@[6:15) Parameter someArray. Type: array. Declaration start char: 0, length: 56
  minLength: 3
  maxLength: 24
}

@minLength(3)
@maxLength(24)
param someArrayWithDecorator array
//@[6:28) Parameter someArrayWithDecorator. Type: array. Declaration start char: 0, length: 63

// empty metadata
param emptyMetadata string {
//@[6:19) Parameter emptyMetadata. Type: string. Declaration start char: 0, length: 48
  metadata: {
  }
}

@metadata({})
param emptyMetadataWithDecorator string
//@[6:32) Parameter emptyMetadataWithDecorator. Type: string. Declaration start char: 0, length: 53

// description
param description string {
//@[6:17) Parameter description. Type: string. Declaration start char: 0, length: 80
  metadata: {
    description: 'my description'
  }
}

@metadata({
  description: 'my description'
})
param descriptionWithDecorator string
//@[6:30) Parameter descriptionWithDecorator. Type: string. Declaration start char: 0, length: 84

@sys.description('my description')
param descriptionWithDecorator2 string
//@[6:31) Parameter descriptionWithDecorator2. Type: string. Declaration start char: 0, length: 73

// random extra metadata
param additionalMetadata string {
//@[6:24) Parameter additionalMetadata. Type: string. Declaration start char: 0, length: 156
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
//@[6:37) Parameter additionalMetadataWithDecorator. Type: string. Declaration start char: 0, length: 138

// all modifiers together
param someParameter string {
//@[6:19) Parameter someParameter. Type: 'one' | 'three' | 'two'. Declaration start char: 0, length: 207
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
//@[6:32) Parameter someParameterWithDecorator. Type: 'one' | 'three' | 'two'. Declaration start char: 0, length: 186

param defaultValueExpression int {
//@[6:28) Parameter defaultValueExpression. Type: int. Declaration start char: 0, length: 66
  default: true ? 4 + 2*3 : 0
}

param defaultExpression bool = 18 != (true || false)
//@[6:23) Parameter defaultExpression. Type: bool. Declaration start char: 0, length: 52

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

// negative zeros are valid lengths
@minLength(-0)
@maxLength(-0)
param negativeZeros string
//@[6:19) Parameter negativeZeros. Type: string. Declaration start char: 0, length: 56

// negative integer literals in modifiers
param negativeModifiers int {
//@[6:23) Parameter negativeModifiers. Type: int. Declaration start char: 0, length: 64
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
param decoratedBool bool
//@[6:19) Parameter decoratedBool. Type: bool. Declaration start char: 0, length: 193

@secure()
param decoratedObject object = {
//@[6:21) Parameter decoratedObject. Type: object. Declaration start char: 0, length: 65
  location: 'westus'
}


@metadata({
    description: 'An array.'
})
@maxLength(20)
@sys.description('I will be overrode.')
param decoratedArray array
//@[6:20) Parameter decoratedArray. Type: array. Declaration start char: 0, length: 125

param allowedPermutation array {
//@[6:24) Parameter allowedPermutation. Type: ('Microsoft.AnalysisServices/servers' | 'Microsoft.ApiManagement/service' | 'Microsoft.Automation/automationAccounts' | 'Microsoft.ContainerInstance/containerGroups' | 'Microsoft.ContainerRegistry/registries' | 'Microsoft.ContainerService/managedClusters' | 'Microsoft.Network/applicationGateways')[]. Declaration start char: 0, length: 454
    default: [
		'Microsoft.AnalysisServices/servers'
		'Microsoft.ContainerRegistry/registries'
	]
    allowed: [
		'Microsoft.AnalysisServices/servers'
		'Microsoft.ApiManagement/service'
		'Microsoft.Network/applicationGateways'
		'Microsoft.Automation/automationAccounts'
		'Microsoft.ContainerInstance/containerGroups'
		'Microsoft.ContainerRegistry/registries'
		'Microsoft.ContainerService/managedClusters'
    ]
}

@allowed([
	'Microsoft.AnalysisServices/servers'
	'Microsoft.ApiManagement/service'
	'Microsoft.Network/applicationGateways'
	'Microsoft.Automation/automationAccounts'
	'Microsoft.ContainerInstance/containerGroups'
	'Microsoft.ContainerRegistry/registries'
	'Microsoft.ContainerService/managedClusters'
])
param allowedPermutationWithDecorator array = [
//@[6:37) Parameter allowedPermutationWithDecorator. Type: ('Microsoft.AnalysisServices/servers' | 'Microsoft.ApiManagement/service' | 'Microsoft.Automation/automationAccounts' | 'Microsoft.ContainerInstance/containerGroups' | 'Microsoft.ContainerRegistry/registries' | 'Microsoft.ContainerService/managedClusters' | 'Microsoft.Network/applicationGateways')[]. Declaration start char: 0, length: 435
	'Microsoft.AnalysisServices/servers'
	'Microsoft.ContainerRegistry/registries'
]

param allowedArray array {
//@[6:18) Parameter allowedArray. Type: array. Declaration start char: 0, length: 323
    default: [
		'Microsoft.AnalysisServices/servers'
		'Microsoft.ApiManagement/service'
	]
    allowed: [
		[
			'Microsoft.AnalysisServices/servers'
			'Microsoft.ApiManagement/service'
		]
		[
			'Microsoft.Network/applicationGateways'
			'Microsoft.Automation/automationAccounts'
		]
    ]
}

@allowed([
	[
		'Microsoft.AnalysisServices/servers'
		'Microsoft.ApiManagement/service'
	]
	[
		'Microsoft.Network/applicationGateways'
		'Microsoft.Automation/automationAccounts'
	]
])
param allowedArrayWithDecorator array = [
//@[6:31) Parameter allowedArrayWithDecorator. Type: array. Declaration start char: 0, length: 303
	'Microsoft.AnalysisServices/servers'
	'Microsoft.ApiManagement/service'
]

