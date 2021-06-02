/* 
  Valid and invalid code is mixed together to validate recovery logic. It can even contain ** * *** **.
*/

param myString string
wrong

param myInt int
param

param 3
param % string
param % string 3 = 's'

param myBool bool

param missingType

// space after identifier #completionTest(32) -> paramTypes
param missingTypeWithSpaceAfter 

// tab after identifier #completionTest(30) -> paramTypes
param missingTypeWithTabAfter	

// #completionTest(20) -> paramTypes
param trailingSpace  

// partial type #completionTest(18, 19, 20, 21) -> paramTypes
param partialType str

param malformedType 44

// malformed type but type check should still happen
param malformedType2 44 = f

// malformed type but type check should still happen
@secure('s')
param malformedModifier 44

param myString2 string = 'string value'

param wrongDefaultValue string = 42

param myInt2 int = 42
param noValueAfterColon int =   

param myTruth bool = 'not a boolean'
param myFalsehood bool = 'false'

param wrongAssignmentToken string: 'hello'

param WhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLong string = 'why not?'

// #completionTest(28,29) -> boolPlusSymbols
param boolCompletions bool = 

// #completionTest(30,31) -> arrayPlusSymbols
param arrayCompletions array = 

// #completionTest(32,33) -> objectPlusSymbols
param objectCompletions object = 

// badly escaped string
param wrongType fluffyBunny = 'what' s up doc?'

// invalid escape
param wrongType fluffyBunny =   'what\s up doc?'

// unterminated string 
param wrongType fluffyBunny =   'what\'s up doc?

// unterminated interpolated string
param wrongType fluffyBunny = 'what\'s ${
param wrongType fluffyBunny =   'what\'s ${ up 
param wrongType fluffyBunny =   'what\'s ${ up }
param wrongType fluffyBunny =   'what\'s ${   'up 

// unterminated nested interpolated string
param wrongType fluffyBunny = 'what\'s ${'up${
param wrongType fluffyBunny = 'what\'s ${'up${
param wrongType fluffyBunny =   'what\'s ${   'up${ doc 
param wrongType fluffyBunny =   'what\'s ${   'up${ doc } 
param wrongType fluffyBunny =   'what\'s ${ 'up${doc}' 
param wrongType fluffyBunny =   'what\'s ${ 'up${doc}' }?

// object literal inside interpolated string
param wrongType fluffyBunny = '${{this: doesnt}.work}'

// bad interpolated string format
param badInterpolatedString string = 'hello ${}!'
param badInterpolatedString2 string = 'hello ${a b c}!'

param wrongType fluffyBunny = 'what\'s up doc?'

// modifier on an invalid type
@minLength(3)
@maxLength(24)
param someArray arra

@secure()
@minLength(3)
@maxLength(123)
param secureInt int

// wrong modifier value types
@allowed([
  'test'
  true
])
@minValue({})
@maxValue([])
@metadata('wrong')
param wrongIntModifier int = true

@metadata(any([]))
@allowed(any(2))
param fatalErrorInIssue1713

// wrong metadata schema
@metadata({
  description: true
})
param wrongMetadataSchema string

// expression in modifier
@maxLength(a + 2)
@minLength(foo())
@allowed([
  i
])
param expressionInModifier string = 2 + 3

@maxLength(2 + 3)
@minLength(length([]))
@allowed([
  resourceGroup().id
])
param nonCompileTimeConstant string

@allowed([])
param emptyAllowedString string

@allowed([])
param emptyAllowedInt int

// 1-cycle in params
param paramDefaultOneCycle string = paramDefaultOneCycle

// 2-cycle in params
param paramDefaultTwoCycle1 string = paramDefaultTwoCycle2
param paramDefaultTwoCycle2 string = paramDefaultTwoCycle1

@allowed([
  paramModifierSelfCycle
])
param paramModifierSelfCycle string

// wrong types of "variable"/identifier access
var sampleVar = 'sample'
resource sampleResource 'Microsoft.Foo/foos@2020-02-02' = {
  name: 'foo'
}
output sampleOutput string = 'hello'

param paramAccessingVar string = concat(sampleVar, 's')

param paramAccessingResource string = sampleResource

param paramAccessingOutput string = sampleOutput

// #completionTest(6) -> empty
param 

// #completionTest(46,47) -> justSymbols
param defaultValueOneLinerCompletions string = 

// invalid comma separator (array)
@metadata({
  description: 'Name of Virtual Machine'
})
@allowed([
  'abc',
  'def'
])
param commaOne string

@secure
@
@&& xxx
@sys
@paramAccessingVar
param incompleteDecorators string

@concat(1, 2)
@sys.concat('a', 'b')
@secure()
// wrong target type
@minValue(20)
param someString string

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

@allowed([], [], 2)
// #completionTest(4) -> empty
@az.
param tooManyArguments1 int = 20

@metadata({}, {}, true)
// #completionTest(2) -> stringParameterDecoratorsPlusNamespace
@m
// #completionTest(1, 2, 3) -> stringParameterDecoratorsPlusNamespace
@   
// #completionTest(5) -> stringParameterDecorators
@sys.
param tooManyArguments2 string

@description(sys.concat(2))
@allowed([for thing in []: 's'])
param nonConstantInDecorator string

@minValue(-length('s'))
@metadata({
  bool: !true
})
param unaryMinusOnFunction int

@minLength(1)
@minLength(2)
@secure()
@maxLength(3)
@maxLength(4)
param duplicateDecorators string

@minLength(-1)
@maxLength(-100)
param invalidLength string

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

// unterminated multi-line comment
/*    

