module nonExistentFileRef './nonExistent.bicep' = {}

// we should only look this file up once, but should still return the same failure
module nonExistentFileRefDuplicate './nonExistent.bicep' = {}

// we should only look this file up once, but should still return the same failure
module nonExistentFileRefEquivalentPath 'abc/def/../../nonExistent.bicep' = {}

module moduleWithoutPath = {

}

// #completionTest(41) -> moduleBodyCompletions
module moduleWithPath './moduleb.bicep' =

// missing identifier #completionTest(7) -> empty
module 

// #completionTest(24,25) -> moduleObject
module missingValue '' = 

var interp = 'hello'
module moduleWithInterpPath './${interp}.bicep' = {}

module moduleWithConditionAndInterpPath './${interp}.bicep' = if (true) {}

module moduleWithSelfCycle './main.bicep' = {}

module moduleWithConditionAndSelfCycle './main.bicep' = if ('foo' == 'bar') {}

module './main.bicep' = {

}

module './main.bicep' = if (1 + 2 == 3) {

}

module './main.bicep' = if

module './main.bicep' = if (

module './main.bicep' = if (true

module './main.bicep' = if (true)

module './main.bicep' = if {

}

module './main.bicep' = if () {

}

module './main.bicep' = if ('true') {

}

module modANoName './modulea.bicep' = {
  // #completionTest(0) -> moduleATopLevelProperties
}

module modANoNameWithCondition './modulea.bicep' = if (true) {
  // #completionTest(0) -> moduleAWithConditionTopLevelProperties
}

module modWithReferenceInCondition './main.bicep' = if (reference('Micorosft.Management/managementGroups/MG', '2020-05-01').name == 'something') {}

module modWithListKeysInCondition './main.bicep' = if (listKeys('foo', '2020-05-01').bar == true) {}

module modANoName './modulea.bicep' = if ({ 'a': b }.a == true) {

}

module modANoInputs './modulea.bicep' = {
  name: 'modANoInputs'
  // #completionTest(0,1,2) -> moduleATopLevelPropertiesMinusName
}

module modANoInputsWithCondition './modulea.bicep' = if (length([
  'foo'
]) == 1) {
  name: 'modANoInputs'
  // #completionTest(0,1,2) -> moduleAWithConditionTopLevelPropertiesMinusName
}

module modAEmptyInputs './modulea.bicep' = {
  name: 'modANoInputs'
  params: {
    // #completionTest(0,1,2,3,4) -> moduleAParams
  }
}

module modAEmptyInputsWithCondition './modulea.bicep' = if (1 + 2 == 2) {
  name: 'modANoInputs'
  params: {
    // #completionTest(0,1,2,3,4) -> moduleAWithConditionParams
  }
}

// #completionTest(55) -> moduleATopLevelPropertyAccess
var modulePropertyAccessCompletions = modAEmptyInputs.o

// #completionTest(81) -> moduleAWithConditionTopLevelPropertyAccess
var moduleWithConditionPropertyAccessCompletions = modAEmptyInputsWithCondition.o

// #completionTest(56) -> moduleAOutputs
var moduleOutputsCompletions = modAEmptyInputs.outputs.s

// #completionTest(82) -> moduleAWithConditionOutputs
var moduleWithConditionOutputsCompletions = modAEmptyInputsWithCondition.outputs.s

module modAUnspecifiedInputs './modulea.bicep' = {
  name: 'modAUnspecifiedInputs'
  params: {
    stringParamB: ''
    objParam: {}
    objArray: []
    unspecifiedInput: ''
  }
}

var unspecifiedOutput = modAUnspecifiedInputs.outputs.test

module modCycle './cycle.bicep' = {}

module moduleWithEmptyPath '' = {}

module moduleWithAbsolutePath '/abc/def.bicep' = {}

module moduleWithBackslash 'child\\file.bicep' = {}

module moduleWithInvalidChar 'child/fi|le.bicep' = {}

module moduleWithInvalidTerminatorChar 'child/test.' = {}

module moduleWithValidScope './empty.bicep' = {
  name: 'moduleWithValidScope'
}

module moduleWithInvalidScope './empty.bicep' = {
  name: 'moduleWithInvalidScope'
  scope: moduleWithValidScope
}

module moduleWithMissingRequiredScope './subscription_empty.bicep' = {
  name: 'moduleWithMissingRequiredScope'
}

module moduleWithInvalidScope2 './empty.bicep' = {
  name: 'moduleWithInvalidScope2'
  scope: managementGroup()
}

module moduleWithUnsupprtedScope1 './mg_empty.bicep' = {
  name: 'moduleWithUnsupprtedScope1'
  scope: managementGroup()
}

module moduleWithUnsupprtedScope2 './mg_empty.bicep' = {
  name: 'moduleWithUnsupprtedScope2'
  scope: managementGroup('MG')
}

module moduleWithBadScope './empty.bicep' = {
  name: 'moduleWithBadScope'
  scope: 'stringScope'
}

resource runtimeValidRes1 'Microsoft.Storage/storageAccounts@2019-06-01' = {
  name: 'runtimeValidRes1Name'
  location: 'westeurope'
  kind: 'Storage'
  sku: {
    name: 'Standard_GRS'
  }
}

module runtimeValidModule1 'empty.bicep' = {
  name: concat(concat(runtimeValidRes1.id, runtimeValidRes1.name), runtimeValidRes1.type)
}

module runtimeInvalidModule1 'empty.bicep' = {
  name: runtimeValidRes1.location
}

module runtimeInvalidModule2 'empty.bicep' = {
  name: runtimeValidRes1['location']
}

module runtimeInvalidModule3 'empty.bicep' = {
  name: runtimeValidRes1.sku.name
}

module runtimeInvalidModule4 'empty.bicep' = {
  name: runtimeValidRes1.sku['name']
}

module runtimeInvalidModule5 'empty.bicep' = {
  name: runtimeValidRes1['sku']['name']
}

module runtimeInvalidModule6 'empty.bicep' = {
  name: runtimeValidRes1['sku'].name
}

module singleModuleForRuntimeCheck 'modulea.bicep' = {
  name: 'test'
}

var moduleRuntimeCheck = singleModuleForRuntimeCheck.outputs.stringOutputA
var moduleRuntimeCheck2 = moduleRuntimeCheck

module moduleLoopForRuntimeCheck 'modulea.bicep' = [for thing in []: {
  name: moduleRuntimeCheck2
}]

var moduleRuntimeCheck3 = moduleLoopForRuntimeCheck[1].outputs.stringOutputB
var moduleRuntimeCheck4 = moduleRuntimeCheck3
module moduleLoopForRuntimeCheck2 'modulea.bicep' = [for thing in []: {
  name: moduleRuntimeCheck4
}]

module moduleLoopForRuntimeCheck3 'modulea.bicep' = [for thing in []: {
  name: concat(moduleLoopForRuntimeCheck[1].outputs.stringOutputB, moduleLoopForRuntimeCheck[1].outputs.stringOutputA)
}]

module moduleWithDuplicateName1 './empty.bicep' = {
  name: 'moduleWithDuplicateName'
  scope: resourceGroup()
}

module moduleWithDuplicateName2 './empty.bicep' = {
  name: 'moduleWithDuplicateName'
}

// #completionTest(19, 20, 21) -> cwdCompletions
module completionB ''

// #completionTest(19, 20, 21) -> cwdCompletions
module completionC '' =

// #completionTest(19, 20, 21) -> cwdCompletions
module completionD '' = {}

// #completionTest(19, 20, 21) -> cwdCompletions
module completionE '' = {
  name: 'hello'
}

// #completionTest(26, 27, 28, 29) -> cwdFileCompletions
module cwdFileCompletionA '.'

// #completionTest(26, 27) -> cwdMCompletions
module cwdFileCompletionB m

// #completionTest(26, 27, 28, 29) -> cwdMCompletions
module cwdFileCompletionC 'm'

// #completionTest(24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39) -> childCompletions
module childCompletionA 'ChildModules/'

// #completionTest(24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39) -> childDotCompletions
module childCompletionB './ChildModules/'

// #completionTest(24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40) -> childMCompletions
module childCompletionC './ChildModules/m'

// #completionTest(24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40) -> childECompletions
module childCompletionD 'ChildModules/e'

@minValue()
module moduleWithNotAttachableDecorators './empty.bicep' = {
  name: 'moduleWithNotAttachableDecorators'
}

// loop parsing cases
module expectedForKeyword 'modulea.bicep' = []

module expectedForKeyword2 'modulea.bicep' = [f]

module expectedLoopVar 'modulea.bicep' = [for]

module expectedInKeyword 'modulea.bicep' = [for x]

module expectedInKeyword2 'modulea.bicep' = [for x b]

module expectedArrayExpression 'modulea.bicep' = [for x in]

module expectedColon 'modulea.bicep' = [for x in y]

module expectedLoopBody 'modulea.bicep' = [for x in y:]

// indexed loop parsing cases
module expectedItemVarName 'modulea.bicep' = [for ()]

module expectedComma 'modulea.bicep' = [for (x)]

module expectedIndexVarName 'modulea.bicep' = [for (x,)]

module expectedInKeyword3 'modulea.bicep' = [for (x,y)]

module expectedArrayExpression2 'modulea.bicep' = [for (x,y) in ]

module expectedColon2 'modulea.bicep' = [for (x,y) in z]

module expectedLoopBody2 'modulea.bicep' = [for (x,y) in z:]

// loop filter parsing cases
module expectedLoopFilterOpenParen 'modulea.bicep' = [for x in y: if]
module expectedLoopFilterOpenParen2 'modulea.bicep' = [for (x,y) in z: if]

module expectedLoopFilterPredicateAndBody 'modulea.bicep' = [for x in y: if()]
module expectedLoopFilterPredicateAndBody2 'modulea.bicep' = [for (x,y) in z: if()]

// wrong loop body type
var emptyArray = []
module wrongLoopBodyType 'modulea.bicep' = [for x in emptyArray:4]
module wrongLoopBodyType2 'modulea.bicep' = [for (x,i) in emptyArray:4]

// missing loop body properties
module missingLoopBodyProperties 'modulea.bicep' = [for x in emptyArray: {}]
module missingLoopBodyProperties2 'modulea.bicep' = [for (x, i) in emptyArray: {}]

// wrong array type
var notAnArray = true
module wrongArrayType 'modulea.bicep' = [for x in notAnArray: {}]

// missing fewer properties
module missingFewerLoopBodyProperties 'modulea.bicep' = [for x in emptyArray: {
  name: 'hello-${x}'
  params: {}
}]

// wrong parameter in the module loop
module wrongModuleParameterInLoop 'modulea.bicep' = [for x in emptyArray: {
  // #completionTest(17) -> symbolsPlusX
  name: 'hello-${x}'
  params: {
    arrayParam: []
    objParam: {}
    stringParamA: 'test'
    stringParamB: 'test'
    notAThing: 'test'
  }
}]
module wrongModuleParameterInFilteredLoop 'modulea.bicep' = [for x in emptyArray: if (true) {
  // #completionTest(17) -> symbolsPlusX_if
  name: 'hello-${x}'
  params: {
    arrayParam: []
    objParam: {}
    stringParamA: 'test'
    stringParamB: 'test'
    notAThing: 'test'
  }
}]
module wrongModuleParameterInLoop2 'modulea.bicep' = [for (x, i) in emptyArray: {
  name: 'hello-${x}'
  params: {
    arrayParam: [
      i
    ]
    objParam: {}
    stringParamA: 'test'
    stringParamB: 'test'
    notAThing: 'test'
  }
}]

module paramNameCompletionsInFilteredLoops 'modulea.bicep' = [for (x, i) in emptyArray: if (true) {
  name: 'hello-${x}'
  params: {
    // #completionTest(0,1,2) -> moduleAParams
  }
}]

// #completionTest(100) -> moduleAOutputs
var propertyAccessCompletionsForFilteredModuleLoop = paramNameCompletionsInFilteredLoops[0].outputs.

// nonexistent arrays and loop variables
var evenMoreDuplicates = 'there'
module nonexistentArrays 'modulea.bicep' = [for evenMoreDuplicates in alsoDoesNotExist: {
  name: 'hello-${whyChooseRealVariablesWhenWeCanPretend}'
  params: {
    objParam: {}
    stringParamB: 'test'
    arrayParam: [for evenMoreDuplicates in totallyFake: doesNotExist]
  }
}]

output directRefToCollectionViaOutput array = nonexistentArrays

module directRefToCollectionViaSingleBody 'modulea.bicep' = {
  name: 'hello'
  params: {
    arrayParam: concat(wrongModuleParameterInLoop, nonexistentArrays)
    objParam: {}
    stringParamB: ''
  }
}

module directRefToCollectionViaSingleConditionalBody 'modulea.bicep' = if (true) {
  name: 'hello2'
  params: {
    arrayParam: concat(wrongModuleParameterInLoop, nonexistentArrays)
    objParam: {}
    stringParamB: ''
  }
}

module directRefToCollectionViaLoopBody 'modulea.bicep' = [for test in []: {
  name: 'hello3'
  params: {
    arrayParam: concat(wrongModuleParameterInLoop, nonexistentArrays)
    objParam: {}
    stringParamB: ''
  }
}]

module directRefToCollectionViaLoopBodyWithExtraDependsOn 'modulea.bicep' = [for test in []: {
  name: 'hello4'
  params: {
    arrayParam: concat(wrongModuleParameterInLoop, nonexistentArrays)
    objParam: {}
    stringParamB: ''
    dependsOn: [
      nonexistentArrays
    ]
  }
  dependsOn: []
}]

// module body that isn't an object
module nonObjectModuleBody 'modulea.bicep' = [for thing in []: 'hello']
module nonObjectModuleBody2 'modulea.bicep' = [for thing in []: concat()]
module nonObjectModuleBody3 'modulea.bicep' = [for (thing,i) in []: 'hello']
module nonObjectModuleBody4 'modulea.bicep' = [for (thing,i) in []: concat()]

module anyTypeInScope 'empty.bicep' = {
  dependsOn: [
    any('s')
  ]

  scope: any(42)
}

module anyTypeInScopeConditional 'empty.bicep' = if (false) {
  dependsOn: [
    any('s')
  ]

  scope: any(42)
}

module anyTypeInScopeLoop 'empty.bicep' = [for thing in []: {
  dependsOn: [
    any('s')
  ]

  scope: any(42)
}]

// Key Vault Secret Reference

resource kv 'Microsoft.KeyVault/vaults@2019-09-01' existing = {
  name: 'testkeyvault'
}

module secureModule1 'moduleb.bicep' = {
  name: 'secureModule1'
  params: {
    stringParamA: kv.getSecret('mySecret')
    stringParamB: '${kv.getSecret('mySecret')}'
    objParam: kv.getSecret('mySecret')
    arrayParam: kv.getSecret('mySecret')
    secureStringParam: '${kv.getSecret('mySecret')}'
    secureObjectParam: kv.getSecret('mySecret')
    secureStringParam2: '${kv.getSecret('mySecret')}'
    secureObjectParam2: kv.getSecret('mySecret')
  }
}

module secureModule2 'BAD_MODULE_PATH.bicep' = {
  name: 'secureModule2'
  params: {
    secret: kv.getSecret('mySecret')
  }
}

module issue3000 'empty.bicep' = {
  name: 'issue3000Module'
  params: {}
  identity: {
    type: 'SystemAssigned'
  }
  extendedLocation: {}
  sku: {}
  kind: 'V1'
  managedBy: 'string'
  mangedByExtended: [
    'str1'
    'str2'
  ]
  zones: [
    'str1'
    'str2'
  ]
  plan: {}
  eTag: ''
  scale: {}
}

module invalidJsonMod 'modulec.json' = {}

module jsonModMissingParam 'moduled.json' = {
  name: 'jsonModMissingParam'
  params: {
    foo: 123
    baz: 'C'
  }
}
