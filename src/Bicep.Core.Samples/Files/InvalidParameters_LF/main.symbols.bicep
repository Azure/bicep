/* 
  Valid and invalid code is mixed together to validate recovery logic. It can even contain ** * *** **.
*/

param myString string
//@[6:14) Parameter myString. Type: string. Declaration start char: 0, length: 21
wrong

param myInt int
//@[6:11) Parameter myInt. Type: int. Declaration start char: 0, length: 15
param
//@[5:5) Parameter <missing>. Type: any. Declaration start char: 0, length: 5

param 3
//@[6:7) Parameter <error>. Type: any. Declaration start char: 0, length: 7
param % string
//@[6:7) Parameter <error>. Type: string. Declaration start char: 0, length: 14
param % string 3 = 's'
//@[6:7) Parameter <error>. Type: string. Declaration start char: 0, length: 22

param myBool bool
//@[6:12) Parameter myBool. Type: bool. Declaration start char: 0, length: 17

param missingType
//@[6:17) Parameter missingType. Type: any. Declaration start char: 0, length: 17

// space after identifier #completionTest(32) -> paramTypes
param missingTypeWithSpaceAfter 
//@[6:31) Parameter missingTypeWithSpaceAfter. Type: any. Declaration start char: 0, length: 32

// tab after identifier #completionTest(30) -> paramTypes
param missingTypeWithTabAfter	
//@[6:29) Parameter missingTypeWithTabAfter. Type: any. Declaration start char: 0, length: 30

// #completionTest(20) -> paramTypes
param trailingSpace  
//@[6:19) Parameter trailingSpace. Type: any. Declaration start char: 0, length: 21

// partial type #completionTest(18, 19, 20, 21) -> paramTypes
param partialType str
//@[6:17) Parameter partialType. Type: error. Declaration start char: 0, length: 21

param malformedType 44
//@[6:19) Parameter malformedType. Type: any. Declaration start char: 0, length: 22

// malformed type but type check should still happen
param malformedType2 44 = f
//@[6:20) Parameter malformedType2. Type: any. Declaration start char: 0, length: 27

// malformed type but type check should still happen
@secure('s')
param malformedModifier 44
//@[6:23) Parameter malformedModifier. Type: any. Declaration start char: 0, length: 39

param myString2 string = 'string value'
//@[6:15) Parameter myString2. Type: string. Declaration start char: 0, length: 39

param wrongDefaultValue string = 42
//@[6:23) Parameter wrongDefaultValue. Type: string. Declaration start char: 0, length: 35

param myInt2 int = 42
//@[6:12) Parameter myInt2. Type: int. Declaration start char: 0, length: 21
param noValueAfterColon int =   
//@[6:23) Parameter noValueAfterColon. Type: int. Declaration start char: 0, length: 32

param myTruth bool = 'not a boolean'
//@[6:13) Parameter myTruth. Type: bool. Declaration start char: 0, length: 36
param myFalsehood bool = 'false'
//@[6:17) Parameter myFalsehood. Type: bool. Declaration start char: 0, length: 32

param wrongAssignmentToken string: 'hello'
//@[6:26) Parameter wrongAssignmentToken. Type: string. Declaration start char: 0, length: 42

param WhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLong string = 'why not?'
//@[6:267) Parameter WhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLong. Type: string. Declaration start char: 0, length: 287

// #completionTest(28,29) -> boolPlusSymbols
param boolCompletions bool = 
//@[6:21) Parameter boolCompletions. Type: bool. Declaration start char: 0, length: 29

// #completionTest(30,31) -> arrayPlusSymbols
param arrayCompletions array = 
//@[6:22) Parameter arrayCompletions. Type: array. Declaration start char: 0, length: 31

// #completionTest(32,33) -> objectPlusSymbols
param objectCompletions object = 
//@[6:23) Parameter objectCompletions. Type: object. Declaration start char: 0, length: 33

// badly escaped string
param wrongType fluffyBunny = 'what's up doc?'
//@[6:15) Parameter wrongType. Type: error. Declaration start char: 0, length: 36

// invalid escape
param wrongType fluffyBunny = 'what\s up doc?'
//@[6:15) Parameter wrongType. Type: error. Declaration start char: 0, length: 46

// unterminated string 
param wrongType fluffyBunny = 'what\'s up doc?
//@[6:15) Parameter wrongType. Type: error. Declaration start char: 0, length: 46

// unterminated interpolated string
param wrongType fluffyBunny = 'what\'s ${
//@[6:15) Parameter wrongType. Type: error. Declaration start char: 0, length: 41
param wrongType fluffyBunny = 'what\'s ${up
//@[6:15) Parameter wrongType. Type: error. Declaration start char: 0, length: 43
param wrongType fluffyBunny = 'what\'s ${up}
//@[6:15) Parameter wrongType. Type: error. Declaration start char: 0, length: 44
param wrongType fluffyBunny = 'what\'s ${'up
//@[6:15) Parameter wrongType. Type: error. Declaration start char: 0, length: 44

// unterminated nested interpolated string
param wrongType fluffyBunny = 'what\'s ${'up${
//@[6:15) Parameter wrongType. Type: error. Declaration start char: 0, length: 46
param wrongType fluffyBunny = 'what\'s ${'up${
//@[6:15) Parameter wrongType. Type: error. Declaration start char: 0, length: 46
param wrongType fluffyBunny = 'what\'s ${'up${doc
//@[6:15) Parameter wrongType. Type: error. Declaration start char: 0, length: 49
param wrongType fluffyBunny = 'what\'s ${'up${doc}
//@[6:15) Parameter wrongType. Type: error. Declaration start char: 0, length: 50
param wrongType fluffyBunny = 'what\'s ${'up${doc}'
//@[6:15) Parameter wrongType. Type: error. Declaration start char: 0, length: 51
param wrongType fluffyBunny = 'what\'s ${'up${doc}'}?
//@[6:15) Parameter wrongType. Type: error. Declaration start char: 0, length: 53

// object literal inside interpolated string
param wrongType fluffyBunny = '${{this: doesnt}.work}'
//@[6:15) Parameter wrongType. Type: error. Declaration start char: 0, length: 54

// bad interpolated string format
param badInterpolatedString string = 'hello ${}!'
//@[6:27) Parameter badInterpolatedString. Type: string. Declaration start char: 0, length: 49
param badInterpolatedString2 string = 'hello ${a b c}!'
//@[6:28) Parameter badInterpolatedString2. Type: string. Declaration start char: 0, length: 55

param wrongType fluffyBunny = 'what\'s up doc?'
//@[6:15) Parameter wrongType. Type: error. Declaration start char: 0, length: 47

// modifier on an invalid type
@minLength(3)
@maxLength(24)
param someArray arra
//@[6:15) Parameter someArray. Type: error. Declaration start char: 0, length: 49

@secure()
@minLength(3)
@maxLength(123)
param secureInt int
//@[6:15) Parameter secureInt. Type: int. Declaration start char: 0, length: 59

// wrong modifier value types
@allowed([
  'test'
  true
])
@minValue({
})
@maxValue([
])
@metadata('wrong')
param wrongIntModifier int = true
//@[6:22) Parameter wrongIntModifier. Type: int. Declaration start char: 0, length: 112

@metadata(any([]))
@allowed(any(2))
param fatalErrorInIssue1713
//@[6:27) Parameter fatalErrorInIssue1713. Type: any. Declaration start char: 0, length: 63

// wrong metadata schema
@metadata({
  description: true
})
param wrongMetadataSchema string
//@[6:25) Parameter wrongMetadataSchema. Type: string. Declaration start char: 0, length: 67

// expression in modifier
@maxLength(a + 2)
@minLength(foo())
@allowed([
  i
])
param expressionInModifier string = 2 + 3
//@[6:26) Parameter expressionInModifier. Type: string. Declaration start char: 0, length: 95

@maxLength(2 + 3)
@minLength(length([]))
@allowed([
  resourceGroup().id
])
param nonCompileTimeConstant string
//@[6:28) Parameter nonCompileTimeConstant. Type: string. Declaration start char: 0, length: 111


@allowed([])
param emptyAllowedString string
//@[6:24) Parameter emptyAllowedString. Type: error. Declaration start char: 0, length: 44

@allowed([])
param emptyAllowedInt int
//@[6:21) Parameter emptyAllowedInt. Type: error. Declaration start char: 0, length: 38

// 1-cycle in params
param paramDefaultOneCycle string = paramDefaultOneCycle
//@[6:26) Parameter paramDefaultOneCycle. Type: string. Declaration start char: 0, length: 56

// 2-cycle in params
param paramDefaultTwoCycle1 string = paramDefaultTwoCycle2
//@[6:27) Parameter paramDefaultTwoCycle1. Type: string. Declaration start char: 0, length: 58
param paramDefaultTwoCycle2 string = paramDefaultTwoCycle1
//@[6:27) Parameter paramDefaultTwoCycle2. Type: string. Declaration start char: 0, length: 58

@allowed([
  paramModifierSelfCycle
])
param paramModifierSelfCycle string
//@[6:28) Parameter paramModifierSelfCycle. Type: string. Declaration start char: 0, length: 74

// wrong types of "variable"/identifier access
var sampleVar = 'sample'
//@[4:13) Variable sampleVar. Type: 'sample'. Declaration start char: 0, length: 24
resource sampleResource 'Microsoft.Foo/foos@2020-02-02' = {
//@[9:23) Resource sampleResource. Type: Microsoft.Foo/foos@2020-02-02. Declaration start char: 0, length: 75
  name: 'foo'
}
output sampleOutput string = 'hello'
//@[7:19) Output sampleOutput. Type: string. Declaration start char: 0, length: 36

param paramAccessingVar string = concat(sampleVar, 's')
//@[6:23) Parameter paramAccessingVar. Type: string. Declaration start char: 0, length: 55

param paramAccessingResource string = sampleResource
//@[6:28) Parameter paramAccessingResource. Type: string. Declaration start char: 0, length: 52

param paramAccessingOutput string = sampleOutput
//@[6:26) Parameter paramAccessingOutput. Type: string. Declaration start char: 0, length: 48

// #completionTest(6) -> empty
param 
//@[6:6) Parameter <missing>. Type: any. Declaration start char: 0, length: 6

// #completionTest(46,47) -> justSymbols
param defaultValueOneLinerCompletions string = 
//@[6:37) Parameter defaultValueOneLinerCompletions. Type: string. Declaration start char: 0, length: 47

// invalid comma separator (array)
@metadata({
  description: 'Name of Virtual Machine'
})
@allowed([
  'abc',
  'def'
])
param commaOne string
//@[6:14) Parameter commaOne. Type: 'abc' | 'def'. Declaration start char: 0, length: 108

@secure
@
@&& xxx
@sys
@paramAccessingVar
param incompleteDecorators string
//@[6:26) Parameter incompleteDecorators. Type: string. Declaration start char: 0, length: 75

@concat(1, 2)
@sys.concat('a', 'b')
@secure()
// wrong target type
@minValue(20)
param someString string
//@[6:16) Parameter someString. Type: string. Declaration start char: 0, length: 104

@allowed([
    true
    10
    'foo'
])
@secure()
// #completionTest(1, 2, 3) -> intParameterDecoratorsPlusNamespace
@  
// #completionTest(5, 6) -> intParameterDecorators
@sys.   
param someInteger int = 20
//@[6:17) Parameter someInteger. Type: int. Declaration start char: 0, length: 207

@allowed([], [], 2)
// #completionTest(4) -> empty
@az.
param tooManyArguments1 int = 20
//@[6:23) Parameter tooManyArguments1. Type: int. Declaration start char: 0, length: 88

@metadata({}, {}, true)
// #completionTest(2) -> stringParameterDecoratorsPlusNamespace
@m
// #completionTest(1, 2, 3) -> stringParameterDecoratorsPlusNamespace
@   
// #completionTest(5) -> stringParameterDecorators
@sys.
param tooManyArguments2 string
//@[6:23) Parameter tooManyArguments2. Type: string. Declaration start char: 0, length: 253

@description(sys.concat(2))
@allowed([for thing in []: 's'])
//@[14:19) Local thing. Type: any. Declaration start char: 14, length: 5
param nonConstantInDecorator string
//@[6:28) Parameter nonConstantInDecorator. Type: string. Declaration start char: 0, length: 96

@minValue(-length('s'))
@metadata({
  bool: !true
})
param unaryMinusOnFunction int
//@[6:26) Parameter unaryMinusOnFunction. Type: int. Declaration start char: 0, length: 83

@minLength(1)
@minLength(2)
@secure()
@maxLength(3)
@maxLength(4)
param duplicateDecorators string
//@[6:25) Parameter duplicateDecorators. Type: string. Declaration start char: 0, length: 98

@minLength(-1)
@maxLength(-100)
param invalidLength string
//@[6:19) Parameter invalidLength. Type: string. Declaration start char: 0, length: 58

@allowed([
	'Microsoft.AnalysisServices/servers'
	'Microsoft.ApiManagement/service'
	'Microsoft.Network/applicationGateways'
	'Microsoft.Automation/automationAccounts'
	'Microsoft.ContainerInstance/containerGroups'
	'Microsoft.ContainerRegistry/registries'
	'Microsoft.ContainerService/managedClusters'
])
param invalidPermutation array = [
//@[6:24) Parameter invalidPermutation. Type: ('Microsoft.AnalysisServices/servers' | 'Microsoft.ApiManagement/service' | 'Microsoft.Automation/automationAccounts' | 'Microsoft.ContainerInstance/containerGroups' | 'Microsoft.ContainerRegistry/registries' | 'Microsoft.ContainerService/managedClusters' | 'Microsoft.Network/applicationGateways')[]. Declaration start char: 0, length: 366
	'foobar'
	true
    100
]

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
param invalidDefaultWithAllowedArrayDecorator array = true
//@[6:45) Parameter invalidDefaultWithAllowedArrayDecorator. Type: array. Declaration start char: 0, length: 245

// unterminated multi-line comment
/*    

