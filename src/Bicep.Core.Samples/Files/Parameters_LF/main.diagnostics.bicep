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
//@[36:107) [BCP161 (Info)] Parameter modifiers are deprecated and will be removed in a future release. Use decorators instead (see https://aka.ms/BicepSpecParams for examples). |{\n  default: [\n    'a'\n    'b'\n    'c'\n    newGuid()\n    utcNow()\n  ]\n}|
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
//@[22:40) [BCP161 (Info)] Parameter modifiers are deprecated and will be removed in a future release. Use decorators instead (see https://aka.ms/BicepSpecParams for examples). |{\n  secure: true\n}|
  secure: true
}

@secure()
param passwordWithDecorator string

// non-secure string
param nonSecure string {
//@[23:42) [BCP161 (Info)] Parameter modifiers are deprecated and will be removed in a future release. Use decorators instead (see https://aka.ms/BicepSpecParams for examples). |{\n  secure: false\n}|
  secure: false
}

// secure object
param secretObject object {
//@[26:44) [BCP161 (Info)] Parameter modifiers are deprecated and will be removed in a future release. Use decorators instead (see https://aka.ms/BicepSpecParams for examples). |{\n  secure: true\n}|
  secure: true
}

@secure()
param secureObjectWithDecorator object

// enum parameter
param storageSku string {
//@[24:82) [BCP161 (Info)] Parameter modifiers are deprecated and will be removed in a future release. Use decorators instead (see https://aka.ms/BicepSpecParams for examples). |{\n  allowed: [\n    'Standard_LRS'\n    'Standard_GRS'\n  ]\n}|
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
//@[25:59) [BCP161 (Info)] Parameter modifiers are deprecated and will be removed in a future release. Use decorators instead (see https://aka.ms/BicepSpecParams for examples). |{\n  minLength: 3\n  maxLength: 24\n}|
  minLength: 3
  maxLength: 24
}

@minLength(3)
@maxLength(24)
param storageNameWithDecorator string

// length constraint on an array
param someArray array {
//@[22:56) [BCP161 (Info)] Parameter modifiers are deprecated and will be removed in a future release. Use decorators instead (see https://aka.ms/BicepSpecParams for examples). |{\n  minLength: 3\n  maxLength: 24\n}|
  minLength: 3
  maxLength: 24
}

@minLength(3)
@maxLength(24)
param someArrayWithDecorator array

// empty metadata
param emptyMetadata string {
//@[27:48) [BCP161 (Info)] Parameter modifiers are deprecated and will be removed in a future release. Use decorators instead (see https://aka.ms/BicepSpecParams for examples). |{\n  metadata: {\n  }\n}|
  metadata: {
  }
}

@metadata({})
param emptyMetadataWithDecorator string

// description
param description string {
//@[25:80) [BCP161 (Info)] Parameter modifiers are deprecated and will be removed in a future release. Use decorators instead (see https://aka.ms/BicepSpecParams for examples). |{\n  metadata: {\n    description: 'my description'\n  }\n}|
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
//@[32:156) [BCP161 (Info)] Parameter modifiers are deprecated and will be removed in a future release. Use decorators instead (see https://aka.ms/BicepSpecParams for examples). |{\n  metadata: {\n    description: 'my description'\n    a: 1\n    b: true\n    c: [\n    ]\n    d: {\n      test: 'abc'\n    }\n  }\n}|
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
//@[27:207) [BCP161 (Info)] Parameter modifiers are deprecated and will be removed in a future release. Use decorators instead (see https://aka.ms/BicepSpecParams for examples). |{\n  secure: true\n  minLength: 3\n  maxLength: 24\n  default: 'one'\n  allowed: [\n    'one'\n    'two'\n    'three'\n  ]\n  metadata: {\n    description: 'Name of the storage account'\n  }\n}|
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
//@[33:66) [BCP161 (Info)] Parameter modifiers are deprecated and will be removed in a future release. Use decorators instead (see https://aka.ms/BicepSpecParams for examples). |{\n  default: true ? 4 + 2*3 : 0\n}|
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

// negative integer literals are allowed as decorator values
@minValue(-10)
@maxValue(-3)
param negativeValues int

// negative zeros are valid lengths
@minLength(-0)
@maxLength(-0)
param negativeZeros string

// negative integer literals in modifiers
param negativeModifiers int {
//@[28:64) [BCP161 (Info)] Parameter modifiers are deprecated and will be removed in a future release. Use decorators instead (see https://aka.ms/BicepSpecParams for examples). |{\n  minValue: -100\n  maxValue: -33\n}|
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

@secure()
param decoratedObject object = {
  location: 'westus'
}


@metadata({
    description: 'An array.'
})
@maxLength(20)
@sys.description('I will be overrode.')
param decoratedArray array

param allowedPermutation array {
//@[31:454) [BCP161 (Info)] Parameter modifiers are deprecated and will be removed in a future release. Use decorators instead (see https://aka.ms/BicepSpecParams for examples). |{\n    default: [\n\t\t'Microsoft.AnalysisServices/servers'\n\t\t'Microsoft.ContainerRegistry/registries'\n\t]\n    allowed: [\n\t\t'Microsoft.AnalysisServices/servers'\n\t\t'Microsoft.ApiManagement/service'\n\t\t'Microsoft.Network/applicationGateways'\n\t\t'Microsoft.Automation/automationAccounts'\n\t\t'Microsoft.ContainerInstance/containerGroups'\n\t\t'Microsoft.ContainerRegistry/registries'\n\t\t'Microsoft.ContainerService/managedClusters'\n    ]\n}|
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
	'Microsoft.AnalysisServices/servers'
	'Microsoft.ContainerRegistry/registries'
]

param allowedArray array {
//@[25:323) [BCP161 (Info)] Parameter modifiers are deprecated and will be removed in a future release. Use decorators instead (see https://aka.ms/BicepSpecParams for examples). |{\n    default: [\n\t\t'Microsoft.AnalysisServices/servers'\n\t\t'Microsoft.ApiManagement/service'\n\t]\n    allowed: [\n\t\t[\n\t\t\t'Microsoft.AnalysisServices/servers'\n\t\t\t'Microsoft.ApiManagement/service'\n\t\t]\n\t\t[\n\t\t\t'Microsoft.Network/applicationGateways'\n\t\t\t'Microsoft.Automation/automationAccounts'\n\t\t]\n    ]\n}|
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
	'Microsoft.AnalysisServices/servers'
	'Microsoft.ApiManagement/service'
]

