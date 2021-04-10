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
param malformedModifier 44 {
  secure: 's'
}

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
param someArray arra {
  minLength: 3
  maxLength: 24
}

@minLength(3)
@maxLength(24)
param someArrayWithDecorator arra

// duplicate modifier property
param duplicatedModifierProperty string {
  minLength: 3
  minLength: 24
}

// non-existent modifiers
param secureInt int {
  secure: true
  minLength: 3
  maxLength: 123
}

@secure()
@minLength(3)
@maxLength(123)
param secureIntWithDecorator int

// wrong modifier value types
param wrongIntModifier int {
  default: true
  allowed: [
    'test'
    true
  ]
  minValue: {}
  maxValue: []
  metadata: 'wrong'
}

@allowed([
  'test'
  true
])
@minValue({})
@maxValue([])
@metadata('wrong')
param wrongIntModifierWithDecorator int = true

@metadata(any([]))
@allowed(any(2))
param fatalErrorInIssue1713

// wrong metadata schema
param wrongMetadataSchema string {
  metadata: {
    description: true
  }
}

@metadata({
  description: true
})
param wrongMetadataSchemaWithDecorator string

// expression in modifier
param expressionInModifier string {
  // #completionTest(10,11) -> symbolsPlusParamDefaultFunctions
  default: 2 + 3
  maxLength: a + 2
  minLength: foo()
  allowed: [
    i
  ]
}

@maxLength(a + 2)
@minLength(foo())
@allowed([
  i
])
param expressionInModifierWithDecorator string = 2 + 3

param nonCompileTimeConstant string {
  maxLength: 2 + 3
  minLength: length([])
  allowed: [
    resourceGroup().id
  ]
}

@maxLength(2 + 3)
@minLength(length([]))
@allowed([
  resourceGroup().id
])
param nonCompileTimeConstantWithDecorator string

param emptyAllowedString string {
  allowed: []
}

@allowed([])
param emptyAllowedStringWithDecorator string

param emptyAllowedInt int {
  allowed: []
}

@allowed([])
param emptyAllowedIntWithDecorator int

// 1-cycle in params
param paramDefaultOneCycle string = paramDefaultOneCycle

// 2-cycle in params
param paramDefaultTwoCycle1 string = paramDefaultTwoCycle2
param paramDefaultTwoCycle2 string = paramDefaultTwoCycle1

// 1-cycle in modifier params
param paramModifierOneCycle string {
  default: paramModifierOneCycle
}

// 1-cycle in modifier with non-default property
param paramModifierSelfCycle string {
  allowed: [
    paramModifierSelfCycle
  ]
}

@allowed([
  paramModifierSelfCycleWithDecorator
])
param paramModifierSelfCycleWithDecorator string

// 2-cycle in modifier params
param paramModifierTwoCycle1 string {
  default: paramModifierTwoCycle2
}
param paramModifierTwoCycle2 string {
  default: paramModifierTwoCycle1
}

// 2-cycle mixed param syntaxes
param paramMixedTwoCycle1 string = paramMixedTwoCycle2
param paramMixedTwoCycle2 string {
  default: paramMixedTwoCycle1
}

// wrong types of "variable"/identifier access
var sampleVar = 'sample'
resource sampleResource 'Microsoft.Foo/foos@2020-02-02' = {
  name: 'foo'
}
output sampleOutput string = 'hello'

param paramAccessingVar string = concat(sampleVar, 's')
param paramAccessingVar2 string {
  default: 'foo ${sampleVar} foo'
}

param paramAccessingResource string = sampleResource
param paramAccessingResource2 string {
  default: base64(sampleResource.properties.foo)
}

param paramAccessingOutput string = sampleOutput
param paramAccessingOutput2 string {
  default: sampleOutput
}

param stringLiteral string {
  allowed: [
    'def'
  ]
}

param stringLiteral2 string {
  allowed: [
    'abc'
    'def'
  ]
  default: stringLiteral
}

param stringLiteral3 string {
  allowed: [
    'abc'
  ]
  default: stringLiteral2
}

// #completionTest(6) -> empty
param 

param stringModifierCompletions string {
  // #completionTest(0,1,2) -> stringModifierProperties
}

param intModifierCompletions int {
  // #completionTest(0,1,2) -> intModifierProperties
}

// #completionTest(46,47) -> justSymbols
param defaultValueOneLinerCompletions string = 

param defaultValueCompletions string {
  allowed: [
    'one'
    'two'
    'three'
    // #completionTest(0,1,2,3,4) -> oneTwoThree
    
  ]
  // #completionTest(10,11) -> oneTwoThreePlusSymbols
  default: 
  
  // #completionTest(9,10) -> booleanValues
  secure: 

  metadata: {
    // #completionTest(0,1,2,3) -> description
    
  }
  // #completionTest(0,1,2) -> stringLengthConstraints
  
}

// invalid comma separator (array)
param commaOne string {
    metadata: {
      description: 'Name of Virtual Machine'
    }
    secure: true
    allowed: [
      'abc',
      'def'
    ]
    default: 'abc'
}

@metadata({
  description: 'Name of Virtual Machine'
})
@allowed([
  'abc',
  'def'
])
param commaOneWithDecorator string

// invalid comma separator (object)
param commaTwo string {
    metadata: {
      description: 'Name of Virtual Machine'
    },
    secure: true
    allowed: [
      'abc'
      'def'
    ]
    default: 'abc'
}

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
param someString string {
  // using decorators and modifier at the same time
  secure: true
}

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

param invalidPermutation array {
  default: [
    'foobar'
    true
    100
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
param invalidPermutationWithDecorator array = [
  'foobar'
  true
  100
]

param invalidDefaultWithAllowedArray array {
  default: true
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
param invalidDefaultWithAllowedArrayDecorator array = true

// unterminated multi-line comment
/*    

