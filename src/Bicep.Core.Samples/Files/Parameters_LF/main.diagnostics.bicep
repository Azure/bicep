/* 
  This is a block comment.
*/

// parameters without default value
param myString string
//@[6:14) [no-unused-params (Warning)] Declared parameter must be referenced within the document scope.\n[See : https://aka.ms/bicep/linter/no-unused-params] |myString|
param myInt int
//@[6:11) [no-unused-params (Warning)] Declared parameter must be referenced within the document scope.\n[See : https://aka.ms/bicep/linter/no-unused-params] |myInt|
param myBool bool
//@[6:12) [no-unused-params (Warning)] Declared parameter must be referenced within the document scope.\n[See : https://aka.ms/bicep/linter/no-unused-params] |myBool|

// parameters with default value
param myString2 string = 'strin${2}g value'
//@[6:15) [no-unused-params (Warning)] Declared parameter must be referenced within the document scope.\n[See : https://aka.ms/bicep/linter/no-unused-params] |myString2|
param myInt2 int = 42
//@[6:12) [no-unused-params (Warning)] Declared parameter must be referenced within the document scope.\n[See : https://aka.ms/bicep/linter/no-unused-params] |myInt2|
param myTruth bool = true
//@[6:13) [no-unused-params (Warning)] Declared parameter must be referenced within the document scope.\n[See : https://aka.ms/bicep/linter/no-unused-params] |myTruth|
param myFalsehood bool = false
//@[6:17) [no-unused-params (Warning)] Declared parameter must be referenced within the document scope.\n[See : https://aka.ms/bicep/linter/no-unused-params] |myFalsehood|
param myEscapedString string = 'First line\nSecond\ttabbed\tline'
//@[6:21) [no-unused-params (Warning)] Declared parameter must be referenced within the document scope.\n[See : https://aka.ms/bicep/linter/no-unused-params] |myEscapedString|
param myNewGuid string = newGuid()
//@[6:15) [no-unused-params (Warning)] Declared parameter must be referenced within the document scope.\n[See : https://aka.ms/bicep/linter/no-unused-params] |myNewGuid|
param myUtcTime string = utcNow()
//@[6:15) [no-unused-params (Warning)] Declared parameter must be referenced within the document scope.\n[See : https://aka.ms/bicep/linter/no-unused-params] |myUtcTime|

// object default value
param foo object = {
//@[6:9) [no-unused-params (Warning)] Declared parameter must be referenced within the document scope.\n[See : https://aka.ms/bicep/linter/no-unused-params] |foo|
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
//@[6:18) [no-unused-params (Warning)] Declared parameter must be referenced within the document scope.\n[See : https://aka.ms/bicep/linter/no-unused-params] |myArrayParam|
  'a'
  'b'
  'c'
]

// alternative array parameter
param myAlternativeArrayParam array {
//@[6:29) [no-unused-params (Warning)] Declared parameter must be referenced within the document scope.\n[See : https://aka.ms/bicep/linter/no-unused-params] |myAlternativeArrayParam|
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
//@[6:14) [no-unused-params (Warning)] Declared parameter must be referenced within the document scope.\n[See : https://aka.ms/bicep/linter/no-unused-params] |password|
//@[22:40) [BCP161 (Info)] Parameter modifiers are deprecated and will be removed in a future release. Use decorators instead (see https://aka.ms/BicepSpecParams for examples). |{\n  secure: true\n}|
  secure: true
}

@secure()
param passwordWithDecorator string
//@[6:27) [no-unused-params (Warning)] Declared parameter must be referenced within the document scope.\n[See : https://aka.ms/bicep/linter/no-unused-params] |passwordWithDecorator|

// non-secure string
param nonSecure string {
//@[6:15) [no-unused-params (Warning)] Declared parameter must be referenced within the document scope.\n[See : https://aka.ms/bicep/linter/no-unused-params] |nonSecure|
//@[23:42) [BCP161 (Info)] Parameter modifiers are deprecated and will be removed in a future release. Use decorators instead (see https://aka.ms/BicepSpecParams for examples). |{\n  secure: false\n}|
  secure: false
}

// secure object
param secretObject object {
//@[6:18) [no-unused-params (Warning)] Declared parameter must be referenced within the document scope.\n[See : https://aka.ms/bicep/linter/no-unused-params] |secretObject|
//@[26:44) [BCP161 (Info)] Parameter modifiers are deprecated and will be removed in a future release. Use decorators instead (see https://aka.ms/BicepSpecParams for examples). |{\n  secure: true\n}|
  secure: true
}

@secure()
param secureObjectWithDecorator object
//@[6:31) [no-unused-params (Warning)] Declared parameter must be referenced within the document scope.\n[See : https://aka.ms/bicep/linter/no-unused-params] |secureObjectWithDecorator|

// enum parameter
param storageSku string {
//@[6:16) [no-unused-params (Warning)] Declared parameter must be referenced within the document scope.\n[See : https://aka.ms/bicep/linter/no-unused-params] |storageSku|
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
//@[6:29) [no-unused-params (Warning)] Declared parameter must be referenced within the document scope.\n[See : https://aka.ms/bicep/linter/no-unused-params] |storageSkuWithDecorator|

// length constraint on a string
param storageName string {
//@[6:17) [no-unused-params (Warning)] Declared parameter must be referenced within the document scope.\n[See : https://aka.ms/bicep/linter/no-unused-params] |storageName|
//@[25:59) [BCP161 (Info)] Parameter modifiers are deprecated and will be removed in a future release. Use decorators instead (see https://aka.ms/BicepSpecParams for examples). |{\n  minLength: 3\n  maxLength: 24\n}|
  minLength: 3
  maxLength: 24
}

@minLength(3)
@maxLength(24)
param storageNameWithDecorator string
//@[6:30) [no-unused-params (Warning)] Declared parameter must be referenced within the document scope.\n[See : https://aka.ms/bicep/linter/no-unused-params] |storageNameWithDecorator|

// length constraint on an array
param someArray array {
//@[6:15) [no-unused-params (Warning)] Declared parameter must be referenced within the document scope.\n[See : https://aka.ms/bicep/linter/no-unused-params] |someArray|
//@[22:56) [BCP161 (Info)] Parameter modifiers are deprecated and will be removed in a future release. Use decorators instead (see https://aka.ms/BicepSpecParams for examples). |{\n  minLength: 3\n  maxLength: 24\n}|
  minLength: 3
  maxLength: 24
}

@minLength(3)
@maxLength(24)
param someArrayWithDecorator array
//@[6:28) [no-unused-params (Warning)] Declared parameter must be referenced within the document scope.\n[See : https://aka.ms/bicep/linter/no-unused-params] |someArrayWithDecorator|

// empty metadata
param emptyMetadata string {
//@[6:19) [no-unused-params (Warning)] Declared parameter must be referenced within the document scope.\n[See : https://aka.ms/bicep/linter/no-unused-params] |emptyMetadata|
//@[27:48) [BCP161 (Info)] Parameter modifiers are deprecated and will be removed in a future release. Use decorators instead (see https://aka.ms/BicepSpecParams for examples). |{\n  metadata: {\n  }\n}|
  metadata: {
  }
}

@metadata({})
param emptyMetadataWithDecorator string
//@[6:32) [no-unused-params (Warning)] Declared parameter must be referenced within the document scope.\n[See : https://aka.ms/bicep/linter/no-unused-params] |emptyMetadataWithDecorator|

// description
param description string {
//@[6:17) [no-unused-params (Warning)] Declared parameter must be referenced within the document scope.\n[See : https://aka.ms/bicep/linter/no-unused-params] |description|
//@[25:80) [BCP161 (Info)] Parameter modifiers are deprecated and will be removed in a future release. Use decorators instead (see https://aka.ms/BicepSpecParams for examples). |{\n  metadata: {\n    description: 'my description'\n  }\n}|
  metadata: {
    description: 'my description'
  }
}

@metadata({
  description: 'my description'
})
param descriptionWithDecorator string
//@[6:30) [no-unused-params (Warning)] Declared parameter must be referenced within the document scope.\n[See : https://aka.ms/bicep/linter/no-unused-params] |descriptionWithDecorator|

@sys.description('my description')
param descriptionWithDecorator2 string
//@[6:31) [no-unused-params (Warning)] Declared parameter must be referenced within the document scope.\n[See : https://aka.ms/bicep/linter/no-unused-params] |descriptionWithDecorator2|

// random extra metadata
param additionalMetadata string {
//@[6:24) [no-unused-params (Warning)] Declared parameter must be referenced within the document scope.\n[See : https://aka.ms/bicep/linter/no-unused-params] |additionalMetadata|
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
//@[6:37) [no-unused-params (Warning)] Declared parameter must be referenced within the document scope.\n[See : https://aka.ms/bicep/linter/no-unused-params] |additionalMetadataWithDecorator|

// all modifiers together
param someParameter string {
//@[6:19) [no-unused-params (Warning)] Declared parameter must be referenced within the document scope.\n[See : https://aka.ms/bicep/linter/no-unused-params] |someParameter|
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
//@[6:32) [no-unused-params (Warning)] Declared parameter must be referenced within the document scope.\n[See : https://aka.ms/bicep/linter/no-unused-params] |someParameterWithDecorator|
//@[40:47) [secure-parameter-default (Warning)] Secure parameters can't have hardcoded default. This prevents storage of sensitive data in the Bicep declaration.\n[See : https://aka.ms/bicep/linter/secure-parameter-default] |= 'one'|

param defaultValueExpression int {
//@[6:28) [no-unused-params (Warning)] Declared parameter must be referenced within the document scope.\n[See : https://aka.ms/bicep/linter/no-unused-params] |defaultValueExpression|
//@[33:66) [BCP161 (Info)] Parameter modifiers are deprecated and will be removed in a future release. Use decorators instead (see https://aka.ms/BicepSpecParams for examples). |{\n  default: true ? 4 + 2*3 : 0\n}|
  default: true ? 4 + 2*3 : 0
}

param defaultExpression bool = 18 != (true || false)
//@[6:23) [no-unused-params (Warning)] Declared parameter must be referenced within the document scope.\n[See : https://aka.ms/bicep/linter/no-unused-params] |defaultExpression|

@secure()
@minLength(2)
  @maxLength(10)
@allowed([
  'Apple'
  'Banana'
])
param decoratedString string
//@[6:21) [no-unused-params (Warning)] Declared parameter must be referenced within the document scope.\n[See : https://aka.ms/bicep/linter/no-unused-params] |decoratedString|

@minValue(200)
param decoratedInt int = 123
//@[6:18) [no-unused-params (Warning)] Declared parameter must be referenced within the document scope.\n[See : https://aka.ms/bicep/linter/no-unused-params] |decoratedInt|

// negative integer literals are allowed as decorator values
@minValue(-10)
@maxValue(-3)
param negativeValues int
//@[6:20) [no-unused-params (Warning)] Declared parameter must be referenced within the document scope.\n[See : https://aka.ms/bicep/linter/no-unused-params] |negativeValues|

// negative zeros are valid lengths
@minLength(-0)
@maxLength(-0)
param negativeZeros string
//@[6:19) [no-unused-params (Warning)] Declared parameter must be referenced within the document scope.\n[See : https://aka.ms/bicep/linter/no-unused-params] |negativeZeros|

// negative integer literals in modifiers
param negativeModifiers int {
//@[6:23) [no-unused-params (Warning)] Declared parameter must be referenced within the document scope.\n[See : https://aka.ms/bicep/linter/no-unused-params] |negativeModifiers|
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
//@[6:19) [no-unused-params (Warning)] Declared parameter must be referenced within the document scope.\n[See : https://aka.ms/bicep/linter/no-unused-params] |decoratedBool|

@secure()
param decoratedObject object = {
//@[6:21) [no-unused-params (Warning)] Declared parameter must be referenced within the document scope.\n[See : https://aka.ms/bicep/linter/no-unused-params] |decoratedObject|
//@[29:55) [secure-parameter-default (Warning)] Secure parameters can't have hardcoded default. This prevents storage of sensitive data in the Bicep declaration.\n[See : https://aka.ms/bicep/linter/secure-parameter-default] |= {\n  location: 'westus'\n}|
  location: 'westus'
}


@metadata({
    description: 'An array.'
})
@maxLength(20)
@sys.description('I will be overrode.')
param decoratedArray array
//@[6:20) [no-unused-params (Warning)] Declared parameter must be referenced within the document scope.\n[See : https://aka.ms/bicep/linter/no-unused-params] |decoratedArray|

param allowedPermutation array {
//@[6:24) [no-unused-params (Warning)] Declared parameter must be referenced within the document scope.\n[See : https://aka.ms/bicep/linter/no-unused-params] |allowedPermutation|
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
//@[6:37) [no-unused-params (Warning)] Declared parameter must be referenced within the document scope.\n[See : https://aka.ms/bicep/linter/no-unused-params] |allowedPermutationWithDecorator|
	'Microsoft.AnalysisServices/servers'
	'Microsoft.ContainerRegistry/registries'
]

param allowedArray array {
//@[6:18) [no-unused-params (Warning)] Declared parameter must be referenced within the document scope.\n[See : https://aka.ms/bicep/linter/no-unused-params] |allowedArray|
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
//@[6:31) [no-unused-params (Warning)] Declared parameter must be referenced within the document scope.\n[See : https://aka.ms/bicep/linter/no-unused-params] |allowedArrayWithDecorator|
	'Microsoft.AnalysisServices/servers'
	'Microsoft.ApiManagement/service'
]

