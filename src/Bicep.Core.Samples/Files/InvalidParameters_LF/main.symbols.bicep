/* 
  Valid and invalid code is mixed together to validate recovery logic. It can even contain ** * *** **.
*/

param myString string
//@[06:014) Parameter myString. Type: string. Declaration start char: 0, length: 21
wrong

param myInt int
//@[06:011) Parameter myInt. Type: int. Declaration start char: 0, length: 15
param
//@[05:005) Parameter <missing>. Type: any. Declaration start char: 0, length: 5

param 3
//@[06:007) Parameter <error>. Type: any. Declaration start char: 0, length: 7
param % string
//@[06:007) Parameter <error>. Type: string. Declaration start char: 0, length: 14
param % string 3 = 's'
//@[06:007) Parameter <error>. Type: string. Declaration start char: 0, length: 22

param myBool bool
//@[06:012) Parameter myBool. Type: bool. Declaration start char: 0, length: 17

param missingType
//@[06:017) Parameter missingType. Type: any. Declaration start char: 0, length: 17

// space after identifier #completionTest(32) -> paramTypes
param missingTypeWithSpaceAfter 
//@[06:031) Parameter missingTypeWithSpaceAfter. Type: any. Declaration start char: 0, length: 32

// tab after identifier #completionTest(30) -> paramTypes
param missingTypeWithTabAfter	
//@[06:029) Parameter missingTypeWithTabAfter. Type: any. Declaration start char: 0, length: 30

// #completionTest(20) -> paramTypes
param trailingSpace  
//@[06:019) Parameter trailingSpace. Type: any. Declaration start char: 0, length: 21

// partial type #completionTest(18, 19, 20, 21) -> paramTypes
param partialType str
//@[06:017) Parameter partialType. Type: error. Declaration start char: 0, length: 21

param malformedType 44
//@[06:019) Parameter malformedType. Type: error. Declaration start char: 0, length: 22

// malformed type but type check should still happen
param malformedType2 44 = f
//@[06:020) Parameter malformedType2. Type: error. Declaration start char: 0, length: 27

// malformed type but type check should still happen
@secure('s')
param malformedModifier 44
//@[06:023) Parameter malformedModifier. Type: error. Declaration start char: 0, length: 39

param myString2 string = 'string value'
//@[06:015) Parameter myString2. Type: string. Declaration start char: 0, length: 39

param wrongDefaultValue string = 42
//@[06:023) Parameter wrongDefaultValue. Type: string. Declaration start char: 0, length: 35

param myInt2 int = 42
//@[06:012) Parameter myInt2. Type: int. Declaration start char: 0, length: 21
param noValueAfterColon int =   
//@[06:023) Parameter noValueAfterColon. Type: int. Declaration start char: 0, length: 32

param myTruth bool = 'not a boolean'
//@[06:013) Parameter myTruth. Type: bool. Declaration start char: 0, length: 36
param myFalsehood bool = 'false'
//@[06:017) Parameter myFalsehood. Type: bool. Declaration start char: 0, length: 32

param wrongAssignmentToken string: 'hello'
//@[06:026) Parameter wrongAssignmentToken. Type: string. Declaration start char: 0, length: 42

param WhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLong string = 'why not?'
//@[06:267) Parameter WhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLong. Type: string. Declaration start char: 0, length: 287

// #completionTest(28,29) -> boolPlusSymbols
param boolCompletions bool = 
//@[06:021) Parameter boolCompletions. Type: bool. Declaration start char: 0, length: 29

// #completionTest(30,31) -> arrayPlusSymbols
param arrayCompletions array = 
//@[06:022) Parameter arrayCompletions. Type: array. Declaration start char: 0, length: 31

// #completionTest(32,33) -> objectPlusSymbols
param objectCompletions object = 
//@[06:023) Parameter objectCompletions. Type: object. Declaration start char: 0, length: 33

// badly escaped string
param wrongType fluffyBunny = 'what's up doc?'
//@[06:015) Parameter wrongType. Type: error. Declaration start char: 0, length: 36

// invalid escape
param wrongType fluffyBunny = 'what\s up doc?'
//@[06:015) Parameter wrongType. Type: error. Declaration start char: 0, length: 46

// unterminated string 
param wrongType fluffyBunny = 'what\'s up doc?
//@[06:015) Parameter wrongType. Type: error. Declaration start char: 0, length: 46

// unterminated interpolated string
param wrongType fluffyBunny = 'what\'s ${
//@[06:015) Parameter wrongType. Type: error. Declaration start char: 0, length: 41
param wrongType fluffyBunny = 'what\'s ${up
//@[06:015) Parameter wrongType. Type: error. Declaration start char: 0, length: 43
param wrongType fluffyBunny = 'what\'s ${up}
//@[06:015) Parameter wrongType. Type: error. Declaration start char: 0, length: 44
param wrongType fluffyBunny = 'what\'s ${'up
//@[06:015) Parameter wrongType. Type: error. Declaration start char: 0, length: 44

// unterminated nested interpolated string
param wrongType fluffyBunny = 'what\'s ${'up${
//@[06:015) Parameter wrongType. Type: error. Declaration start char: 0, length: 46
param wrongType fluffyBunny = 'what\'s ${'up${
//@[06:015) Parameter wrongType. Type: error. Declaration start char: 0, length: 46
param wrongType fluffyBunny = 'what\'s ${'up${doc
//@[06:015) Parameter wrongType. Type: error. Declaration start char: 0, length: 49
param wrongType fluffyBunny = 'what\'s ${'up${doc}
//@[06:015) Parameter wrongType. Type: error. Declaration start char: 0, length: 50
param wrongType fluffyBunny = 'what\'s ${'up${doc}'
//@[06:015) Parameter wrongType. Type: error. Declaration start char: 0, length: 51
param wrongType fluffyBunny = 'what\'s ${'up${doc}'}?
//@[06:015) Parameter wrongType. Type: error. Declaration start char: 0, length: 53

// object literal inside interpolated string
param wrongType fluffyBunny = '${{this: doesnt}.work}'
//@[06:015) Parameter wrongType. Type: error. Declaration start char: 0, length: 54

// bad interpolated string format
param badInterpolatedString string = 'hello ${}!'
//@[06:027) Parameter badInterpolatedString. Type: string. Declaration start char: 0, length: 49
param badInterpolatedString2 string = 'hello ${a b c}!'
//@[06:028) Parameter badInterpolatedString2. Type: string. Declaration start char: 0, length: 55

param wrongType fluffyBunny = 'what\'s up doc?'
//@[06:015) Parameter wrongType. Type: error. Declaration start char: 0, length: 47

// modifier on an invalid type
@minLength(3)
@maxLength(24)
param someArray arra
//@[06:015) Parameter someArray. Type: error. Declaration start char: 0, length: 49

@secure()
@minLength(3)
@maxLength(123)
param secureInt int
//@[06:015) Parameter secureInt. Type: int. Declaration start char: 0, length: 59

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
//@[06:022) Parameter wrongIntModifier. Type: int. Declaration start char: 0, length: 112

@metadata(any([]))
@allowed(any(2))
param fatalErrorInIssue1713
//@[06:027) Parameter fatalErrorInIssue1713. Type: any. Declaration start char: 0, length: 63

// wrong metadata schema
@metadata({
  description: true
})
param wrongMetadataSchema string
//@[06:025) Parameter wrongMetadataSchema. Type: string. Declaration start char: 0, length: 67

// expression in modifier
@maxLength(a + 2)
@minLength(foo())
@allowed([
  i
])
param expressionInModifier string = 2 + 3
//@[06:026) Parameter expressionInModifier. Type: string. Declaration start char: 0, length: 95

@maxLength(2 + 3)
@minLength(length([]))
@allowed([
  resourceGroup().id
])
param nonCompileTimeConstant string
//@[06:028) Parameter nonCompileTimeConstant. Type: string. Declaration start char: 0, length: 111


@allowed([])
param emptyAllowedString string
//@[06:024) Parameter emptyAllowedString. Type: error. Declaration start char: 0, length: 44

@allowed([])
param emptyAllowedInt int
//@[06:021) Parameter emptyAllowedInt. Type: error. Declaration start char: 0, length: 38

// 1-cycle in params
param paramDefaultOneCycle string = paramDefaultOneCycle
//@[06:026) Parameter paramDefaultOneCycle. Type: string. Declaration start char: 0, length: 56

// 2-cycle in params
param paramDefaultTwoCycle1 string = paramDefaultTwoCycle2
//@[06:027) Parameter paramDefaultTwoCycle1. Type: string. Declaration start char: 0, length: 58
param paramDefaultTwoCycle2 string = paramDefaultTwoCycle1
//@[06:027) Parameter paramDefaultTwoCycle2. Type: string. Declaration start char: 0, length: 58

@allowed([
  paramModifierSelfCycle
])
param paramModifierSelfCycle string
//@[06:028) Parameter paramModifierSelfCycle. Type: string. Declaration start char: 0, length: 74

// wrong types of "variable"/identifier access
var sampleVar = 'sample'
//@[04:013) Variable sampleVar. Type: 'sample'. Declaration start char: 0, length: 24
resource sampleResource 'Microsoft.Foo/foos@2020-02-02' = {
//@[09:023) Resource sampleResource. Type: Microsoft.Foo/foos@2020-02-02. Declaration start char: 0, length: 75
  name: 'foo'
}
output sampleOutput string = 'hello'
//@[07:019) Output sampleOutput. Type: string. Declaration start char: 0, length: 36

param paramAccessingVar string = concat(sampleVar, 's')
//@[06:023) Parameter paramAccessingVar. Type: string. Declaration start char: 0, length: 55

param paramAccessingResource string = sampleResource
//@[06:028) Parameter paramAccessingResource. Type: string. Declaration start char: 0, length: 52

param paramAccessingOutput string = sampleOutput
//@[06:026) Parameter paramAccessingOutput. Type: string. Declaration start char: 0, length: 48

// #completionTest(6) -> empty
param 
//@[06:006) Parameter <missing>. Type: any. Declaration start char: 0, length: 6

// #completionTest(46,47) -> justSymbols
param defaultValueOneLinerCompletions string = 
//@[06:037) Parameter defaultValueOneLinerCompletions. Type: string. Declaration start char: 0, length: 47

// invalid comma separator (array)
@metadata({
  description: 'Name of Virtual Machine'
})
@allowed([
  'abc',
  'def'
])
param commaOne string
//@[06:014) Parameter commaOne. Type: 'abc' | 'def'. Declaration start char: 0, length: 108

@secure
@
@&& xxx
@sys
@paramAccessingVar
param incompleteDecorators string
//@[06:026) Parameter incompleteDecorators. Type: string. Declaration start char: 0, length: 75

@concat(1, 2)
@sys.concat('a', 'b')
@secure()
// wrong target type
@minValue(20)
param someString string
//@[06:016) Parameter someString. Type: string. Declaration start char: 0, length: 104

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
//@[06:017) Parameter someInteger. Type: int. Declaration start char: 0, length: 207

@allowed([], [], 2)
// #completionTest(4) -> empty
@az.
param tooManyArguments1 int = 20
//@[06:023) Parameter tooManyArguments1. Type: int. Declaration start char: 0, length: 88

@metadata({}, {}, true)
// #completionTest(2) -> stringParameterDecoratorsPlusNamespace
@m
// #completionTest(1, 2, 3) -> stringParameterDecoratorsPlusNamespace
@   
// #completionTest(5) -> stringParameterDecorators
@sys.
param tooManyArguments2 string
//@[06:023) Parameter tooManyArguments2. Type: string. Declaration start char: 0, length: 253

@description(sys.concat(2))
@allowed([for thing in []: 's'])
//@[14:019) Local thing. Type: never. Declaration start char: 14, length: 5
param nonConstantInDecorator string
//@[06:028) Parameter nonConstantInDecorator. Type: string. Declaration start char: 0, length: 96

@minValue(-length('s'))
@metadata({
  bool: !true
})
param unaryMinusOnFunction int
//@[06:026) Parameter unaryMinusOnFunction. Type: int. Declaration start char: 0, length: 83

@minLength(1)
@minLength(2)
@secure()
@maxLength(3)
@maxLength(4)
param duplicateDecorators string
//@[06:025) Parameter duplicateDecorators. Type: string. Declaration start char: 0, length: 98

@minLength(-1)
@maxLength(-100)
param invalidLength string
//@[06:019) Parameter invalidLength. Type: string. Declaration start char: 0, length: 58

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
//@[06:024) Parameter invalidPermutation. Type: ('Microsoft.AnalysisServices/servers' | 'Microsoft.ApiManagement/service' | 'Microsoft.Automation/automationAccounts' | 'Microsoft.ContainerInstance/containerGroups' | 'Microsoft.ContainerRegistry/registries' | 'Microsoft.ContainerService/managedClusters' | 'Microsoft.Network/applicationGateways')[]. Declaration start char: 0, length: 366
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
//@[06:045) Parameter invalidDefaultWithAllowedArrayDecorator. Type: (['Microsoft.AnalysisServices/servers', 'Microsoft.ApiManagement/service'] | ['Microsoft.Network/applicationGateways', 'Microsoft.Automation/automationAccounts'])[]. Declaration start char: 0, length: 245

// unterminated multi-line comment
/*    

