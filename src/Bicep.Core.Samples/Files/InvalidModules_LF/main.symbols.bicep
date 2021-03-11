module nonExistentFileRef './nonExistent.bicep' = {
//@[7:25) Module nonExistentFileRef. Type: error. Declaration start char: 0, length: 54

}

// we should only look this file up once, but should still return the same failure
module nonExistentFileRefDuplicate './nonExistent.bicep' = {
//@[7:34) Module nonExistentFileRefDuplicate. Type: error. Declaration start char: 0, length: 63

}

// we should only look this file up once, but should still return the same failure
module nonExistentFileRefEquivalentPath 'abc/def/../../nonExistent.bicep' = {
//@[7:39) Module nonExistentFileRefEquivalentPath. Type: error. Declaration start char: 0, length: 80

}

module moduleWithoutPath = {
//@[7:24) Module moduleWithoutPath. Type: error. Declaration start char: 0, length: 28

}

// missing identifier #completionTest(7) -> empty
module 
//@[7:7) Module <missing>. Type: error. Declaration start char: 0, length: 7

// #completionTest(24,25) -> object
module missingValue '' = 
//@[7:19) Module missingValue. Type: error. Declaration start char: 0, length: 25

var interp = 'hello'
//@[4:10) Variable interp. Type: 'hello'. Declaration start char: 0, length: 20
module moduleWithInterpPath './${interp}.bicep' = {
//@[7:27) Module moduleWithInterpPath. Type: error. Declaration start char: 0, length: 54

}

module moduleWithConditionAndInterpPath './${interp}.bicep' = if (true) {
//@[7:39) Module moduleWithConditionAndInterpPath. Type: error. Declaration start char: 0, length: 76

}

module moduleWithSelfCycle './main.bicep' = {
//@[7:26) Module moduleWithSelfCycle. Type: error. Declaration start char: 0, length: 48

}

module moduleWithConditionAndSelfCycle './main.bicep' = if ('foo' == 'bar') {
//@[7:38) Module moduleWithConditionAndSelfCycle. Type: error. Declaration start char: 0, length: 80

}

module './main.bicep' = {
//@[7:7) Module <missing>. Type: error. Declaration start char: 0, length: 28

}

module './main.bicep' = if (1 + 2 == 3) {
//@[7:7) Module <missing>. Type: error. Declaration start char: 0, length: 44

}

module './main.bicep' = if
//@[7:7) Module <missing>. Type: error. Declaration start char: 0, length: 26

module './main.bicep' = if (
//@[7:7) Module <missing>. Type: error. Declaration start char: 0, length: 28

module './main.bicep' = if (true
//@[7:7) Module <missing>. Type: error. Declaration start char: 0, length: 32

module './main.bicep' = if (true)
//@[7:7) Module <missing>. Type: error. Declaration start char: 0, length: 33

module './main.bicep' = if {
//@[7:7) Module <missing>. Type: error. Declaration start char: 0, length: 31

}

module './main.bicep' = if () {
//@[7:7) Module <missing>. Type: error. Declaration start char: 0, length: 34

}

module './main.bicep' = if ('true') {
//@[7:7) Module <missing>. Type: error. Declaration start char: 0, length: 40

}

module modANoName './modulea.bicep' = {
//@[7:17) Module modANoName. Type: module. Declaration start char: 0, length: 93
// #completionTest(0) -> moduleATopLevelProperties

}

module modANoNameWithCondition './modulea.bicep' = if (true) {
//@[7:30) Module modANoNameWithCondition. Type: module. Declaration start char: 0, length: 129
// #completionTest(0) -> moduleAWithConditionTopLevelProperties

}

module modWithReferenceInCondition './main.bicep' = if (reference('Micorosft.Management/managementGroups/MG', '2020-05-01').name == 'something') {
//@[7:34) Module modWithReferenceInCondition. Type: error. Declaration start char: 0, length: 149

}

module modWithListKeysInCondition './main.bicep' = if (listKeys('foo', '2020-05-01').bar == true) {
//@[7:33) Module modWithListKeysInCondition. Type: error. Declaration start char: 0, length: 102

}


module modANoName './modulea.bicep' = if ({ 'a': b }.a == true) {
//@[7:17) Module modANoName. Type: error. Declaration start char: 0, length: 68

}

module modANoInputs './modulea.bicep' = {
//@[7:19) Module modANoInputs. Type: module. Declaration start char: 0, length: 135
  name: 'modANoInputs'
  // #completionTest(0,1,2) -> moduleATopLevelPropertiesMinusName
  
}

module modANoInputsWithCondition './modulea.bicep' = if (length([
//@[7:32) Module modANoInputsWithCondition. Type: module. Declaration start char: 0, length: 191
  'foo'
]) == 1) {
  name: 'modANoInputs'
  // #completionTest(0,1,2) -> moduleAWithConditionTopLevelPropertiesMinusName
  
}

module modAEmptyInputs './modulea.bicep' = {
//@[7:22) Module modAEmptyInputs. Type: module. Declaration start char: 0, length: 141
  name: 'modANoInputs'
  params: {
    // #completionTest(0,1,2,3,4) -> moduleAParams
    
  }
}

module modAEmptyInputsWithCondition './modulea.bicep' = if (1 + 2 == 2) {
//@[7:35) Module modAEmptyInputsWithCondition. Type: module. Declaration start char: 0, length: 183
  name: 'modANoInputs'
  params: {
    // #completionTest(0,1,2,3,4) -> moduleAWithConditionParams
    
  }
}

// #completionTest(55) -> moduleATopLevelPropertyAccess
var modulePropertyAccessCompletions = modAEmptyInputs.o
//@[4:35) Variable modulePropertyAccessCompletions. Type: error. Declaration start char: 0, length: 55

// #completionTest(81) -> moduleAWithConditionTopLevelPropertyAccess
var moduleWithConditionPropertyAccessCompletions = modAEmptyInputsWithCondition.o
//@[4:48) Variable moduleWithConditionPropertyAccessCompletions. Type: error. Declaration start char: 0, length: 81

// #completionTest(56) -> moduleAOutputs
var moduleOutputsCompletions = modAEmptyInputs.outputs.s
//@[4:28) Variable moduleOutputsCompletions. Type: error. Declaration start char: 0, length: 56

// #completionTest(82) -> moduleAWithConditionOutputs
var moduleWithConditionOutputsCompletions = modAEmptyInputsWithCondition.outputs.s
//@[4:41) Variable moduleWithConditionOutputsCompletions. Type: error. Declaration start char: 0, length: 82

module modAUnspecifiedInputs './modulea.bicep' = {
//@[7:28) Module modAUnspecifiedInputs. Type: module. Declaration start char: 0, length: 180
  name: 'modAUnspecifiedInputs'
  params: {
    stringParamB: ''
    objParam: {}
    objArray: []
    unspecifiedInput: ''
  }
}

var unspecifiedOutput = modAUnspecifiedInputs.outputs.test
//@[4:21) Variable unspecifiedOutput. Type: error. Declaration start char: 0, length: 58

module modCycle './cycle.bicep' = {
//@[7:15) Module modCycle. Type: error. Declaration start char: 0, length: 40
  
}

module moduleWithEmptyPath '' = {
//@[7:26) Module moduleWithEmptyPath. Type: error. Declaration start char: 0, length: 35
}

module moduleWithAbsolutePath '/abc/def.bicep' = {
//@[7:29) Module moduleWithAbsolutePath. Type: error. Declaration start char: 0, length: 52
}

module moduleWithBackslash 'child\\file.bicep' = {
//@[7:26) Module moduleWithBackslash. Type: error. Declaration start char: 0, length: 52
}

module moduleWithInvalidChar 'child/fi|le.bicep' = {
//@[7:28) Module moduleWithInvalidChar. Type: error. Declaration start char: 0, length: 54
}

module moduleWithInvalidTerminatorChar 'child/test.' = {
//@[7:38) Module moduleWithInvalidTerminatorChar. Type: error. Declaration start char: 0, length: 58
}

module moduleWithValidScope './empty.bicep' = {
//@[7:27) Module moduleWithValidScope. Type: module. Declaration start char: 0, length: 80
  name: 'moduleWithValidScope'
}

module moduleWithInvalidScope './empty.bicep' = {
//@[7:29) Module moduleWithInvalidScope. Type: module. Declaration start char: 0, length: 114
  name: 'moduleWithInvalidScope'
  scope: moduleWithValidScope
}

module moduleWithMissingRequiredScope './subscription_empty.bicep' = {
//@[7:37) Module moduleWithMissingRequiredScope. Type: module. Declaration start char: 0, length: 113
  name: 'moduleWithMissingRequiredScope'
}

module moduleWithInvalidScope2 './empty.bicep' = {
//@[7:30) Module moduleWithInvalidScope2. Type: module. Declaration start char: 0, length: 113
  name: 'moduleWithInvalidScope2'
  scope: managementGroup()
}

module moduleWithUnsupprtedScope1 './mg_empty.bicep' = {
//@[7:33) Module moduleWithUnsupprtedScope1. Type: module. Declaration start char: 0, length: 122
  name: 'moduleWithUnsupprtedScope1'
  scope: managementGroup()
}

module moduleWithUnsupprtedScope2 './mg_empty.bicep' = {
//@[7:33) Module moduleWithUnsupprtedScope2. Type: module. Declaration start char: 0, length: 126
  name: 'moduleWithUnsupprtedScope2'
  scope: managementGroup('MG')
}

module moduleWithBadScope './empty.bicep' = {
//@[7:25) Module moduleWithBadScope. Type: module. Declaration start char: 0, length: 99
  name: 'moduleWithBadScope'
  scope: 'stringScope'
}

resource runtimeValidRes1 'Microsoft.Storage/storageAccounts@2019-06-01' = {
//@[9:25) Resource runtimeValidRes1. Type: Microsoft.Storage/storageAccounts@2019-06-01. Declaration start char: 0, length: 190
  name: 'runtimeValidRes1Name'
  location: 'westeurope'
  kind: 'Storage'
  sku: {
    name: 'Standard_GRS'
  }
}

module runtimeValidModule1 'empty.bicep' = {
//@[7:26) Module runtimeValidModule1. Type: module. Declaration start char: 0, length: 136
  name: concat(concat(runtimeValidRes1.id, runtimeValidRes1.name), runtimeValidRes1.type)
}

module runtimeInvalidModule1 'empty.bicep' = {
//@[7:28) Module runtimeInvalidModule1. Type: module. Declaration start char: 0, length: 82
  name: runtimeValidRes1.location
}

module runtimeInvalidModule2 'empty.bicep' = {
//@[7:28) Module runtimeInvalidModule2. Type: module. Declaration start char: 0, length: 85
  name: runtimeValidRes1['location']
}

module runtimeInvalidModule3 'empty.bicep' = {
//@[7:28) Module runtimeInvalidModule3. Type: module. Declaration start char: 0, length: 82
  name: runtimeValidRes1.sku.name
}

module runtimeInvalidModule4 'empty.bicep' = {
//@[7:28) Module runtimeInvalidModule4. Type: module. Declaration start char: 0, length: 85
  name: runtimeValidRes1.sku['name']
}

module runtimeInvalidModule5 'empty.bicep' = {
//@[7:28) Module runtimeInvalidModule5. Type: module. Declaration start char: 0, length: 88
  name: runtimeValidRes1['sku']['name']
}

module runtimeInvalidModule6 'empty.bicep' = {
//@[7:28) Module runtimeInvalidModule6. Type: module. Declaration start char: 0, length: 85
  name: runtimeValidRes1['sku'].name
}

module singleModuleForRuntimeCheck 'modulea.bicep' = {
//@[7:34) Module singleModuleForRuntimeCheck. Type: module. Declaration start char: 0, length: 71
  name: 'test'
}

var moduleRuntimeCheck = singleModuleForRuntimeCheck.outputs.stringOutputA
//@[4:22) Variable moduleRuntimeCheck. Type: string. Declaration start char: 0, length: 74
var moduleRuntimeCheck2 = moduleRuntimeCheck
//@[4:23) Variable moduleRuntimeCheck2. Type: string. Declaration start char: 0, length: 44

module moduleLoopForRuntimeCheck 'modulea.bicep' = [for thing in []: {
//@[56:61) Local thing. Type: any. Declaration start char: 56, length: 5
//@[7:32) Module moduleLoopForRuntimeCheck. Type: module[]. Declaration start char: 0, length: 101
  name: moduleRuntimeCheck2
}]

var moduleRuntimeCheck3 = moduleLoopForRuntimeCheck[1].outputs.stringOutputB
//@[4:23) Variable moduleRuntimeCheck3. Type: string. Declaration start char: 0, length: 76
var moduleRuntimeCheck4 = moduleRuntimeCheck3
//@[4:23) Variable moduleRuntimeCheck4. Type: string. Declaration start char: 0, length: 45
module moduleLoopForRuntimeCheck2 'modulea.bicep' = [for thing in []: {
//@[57:62) Local thing. Type: any. Declaration start char: 57, length: 5
//@[7:33) Module moduleLoopForRuntimeCheck2. Type: module[]. Declaration start char: 0, length: 102
  name: moduleRuntimeCheck4
}]

module moduleLoopForRuntimeCheck3 'modulea.bicep' = [for thing in []: {
//@[57:62) Local thing. Type: any. Declaration start char: 57, length: 5
//@[7:33) Module moduleLoopForRuntimeCheck3. Type: module[]. Declaration start char: 0, length: 194
  name: concat(moduleLoopForRuntimeCheck[1].outputs.stringOutputB, moduleLoopForRuntimeCheck[1].outputs.stringOutputA )
}]

module moduleWithDuplicateName1 './empty.bicep' = {
//@[7:31) Module moduleWithDuplicateName1. Type: module. Declaration start char: 0, length: 112
  name: 'moduleWithDuplicateName'
  scope: resourceGroup()
}

module moduleWithDuplicateName2 './empty.bicep' = {
//@[7:31) Module moduleWithDuplicateName2. Type: module. Declaration start char: 0, length: 87
  name: 'moduleWithDuplicateName'
}

// #completionTest(19, 20, 21) -> cwdCompletions
module completionB ''
//@[7:18) Module completionB. Type: error. Declaration start char: 0, length: 21

// #completionTest(19, 20, 21) -> cwdCompletions
module completionC '' =
//@[7:18) Module completionC. Type: error. Declaration start char: 0, length: 23

// #completionTest(19, 20, 21) -> cwdCompletions
module completionD '' = {}
//@[7:18) Module completionD. Type: error. Declaration start char: 0, length: 26

// #completionTest(19, 20, 21) -> cwdCompletions
module completionE '' = {
//@[7:18) Module completionE. Type: error. Declaration start char: 0, length: 43
  name: 'hello'
}

// #completionTest(26, 27, 28, 29) -> cwdFileCompletions
module cwdFileCompletionA '.'
//@[7:25) Module cwdFileCompletionA. Type: error. Declaration start char: 0, length: 29

// #completionTest(26, 27) -> cwdMCompletions
module cwdFileCompletionB m
//@[7:25) Module cwdFileCompletionB. Type: error. Declaration start char: 0, length: 27

// #completionTest(26, 27, 28, 29) -> cwdMCompletions
module cwdFileCompletionC 'm'
//@[7:25) Module cwdFileCompletionC. Type: error. Declaration start char: 0, length: 29

// #completionTest(24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39) -> childCompletions
module childCompletionA 'ChildModules/'
//@[7:23) Module childCompletionA. Type: error. Declaration start char: 0, length: 39

// #completionTest(24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39) -> childDotCompletions
module childCompletionB './ChildModules/'
//@[7:23) Module childCompletionB. Type: error. Declaration start char: 0, length: 41

// #completionTest(24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40) -> childMCompletions
module childCompletionC './ChildModules/m'
//@[7:23) Module childCompletionC. Type: error. Declaration start char: 0, length: 42

// #completionTest(24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40) -> childECompletions
module childCompletionD 'ChildModules/e'
//@[7:23) Module childCompletionD. Type: error. Declaration start char: 0, length: 40

@minValue()
module moduleWithNotAttachableDecorators './empty.bicep' = {
//@[7:40) Module moduleWithNotAttachableDecorators. Type: module. Declaration start char: 0, length: 118
  name: 'moduleWithNotAttachableDecorators'
}

// loop parsing cases
module expectedForKeyword 'modulea.bicep' = []
//@[7:25) Module expectedForKeyword. Type: module. Declaration start char: 0, length: 46

module expectedForKeyword2 'modulea.bicep' = [f]
//@[7:26) Module expectedForKeyword2. Type: module. Declaration start char: 0, length: 48

module expectedLoopVar 'modulea.bicep' = [for]
//@[7:22) Module expectedLoopVar. Type: module[]. Declaration start char: 0, length: 46

module expectedInKeyword 'modulea.bicep' = [for x]
//@[48:49) Local x. Type: any. Declaration start char: 48, length: 1
//@[7:24) Module expectedInKeyword. Type: module[]. Declaration start char: 0, length: 50

module expectedInKeyword2 'modulea.bicep' = [for x b]
//@[49:50) Local x. Type: any. Declaration start char: 49, length: 1
//@[7:25) Module expectedInKeyword2. Type: module[]. Declaration start char: 0, length: 53

module expectedArrayExpression 'modulea.bicep' = [for x in]
//@[54:55) Local x. Type: any. Declaration start char: 54, length: 1
//@[7:30) Module expectedArrayExpression. Type: module[]. Declaration start char: 0, length: 59

module expectedColon 'modulea.bicep' = [for x in y]
//@[44:45) Local x. Type: any. Declaration start char: 44, length: 1
//@[7:20) Module expectedColon. Type: module[]. Declaration start char: 0, length: 51

module expectedLoopBody 'modulea.bicep' = [for x in y:]
//@[47:48) Local x. Type: any. Declaration start char: 47, length: 1
//@[7:23) Module expectedLoopBody. Type: module[]. Declaration start char: 0, length: 55

// indexed loop parsing cases
module expectedItemVarName 'modulea.bicep' = [for ()]
//@[51:51) Local <missing>. Type: any. Declaration start char: 51, length: 0
//@[51:51) Local <missing>. Type: int. Declaration start char: 51, length: 0
//@[7:26) Module expectedItemVarName. Type: module[]. Declaration start char: 0, length: 53

module expectedComma 'modulea.bicep' = [for (x)]
//@[45:46) Local x. Type: any. Declaration start char: 45, length: 1
//@[46:46) Local <missing>. Type: int. Declaration start char: 46, length: 0
//@[7:20) Module expectedComma. Type: module[]. Declaration start char: 0, length: 48

module expectedIndexVarName 'modulea.bicep' = [for (x,)]
//@[52:53) Local x. Type: any. Declaration start char: 52, length: 1
//@[54:54) Local <missing>. Type: int. Declaration start char: 54, length: 0
//@[7:27) Module expectedIndexVarName. Type: module[]. Declaration start char: 0, length: 56

module expectedInKeyword3 'modulea.bicep' = [for (x,y)]
//@[50:51) Local x. Type: any. Declaration start char: 50, length: 1
//@[52:53) Local y. Type: int. Declaration start char: 52, length: 1
//@[7:25) Module expectedInKeyword3. Type: module[]. Declaration start char: 0, length: 55

module expectedArrayExpression2 'modulea.bicep' = [for (x,y) in ]
//@[56:57) Local x. Type: any. Declaration start char: 56, length: 1
//@[58:59) Local y. Type: int. Declaration start char: 58, length: 1
//@[7:31) Module expectedArrayExpression2. Type: module[]. Declaration start char: 0, length: 65

module expectedColon2 'modulea.bicep' = [for (x,y) in z]
//@[46:47) Local x. Type: any. Declaration start char: 46, length: 1
//@[48:49) Local y. Type: int. Declaration start char: 48, length: 1
//@[7:21) Module expectedColon2. Type: module[]. Declaration start char: 0, length: 56

module expectedLoopBody2 'modulea.bicep' = [for (x,y) in z:]
//@[49:50) Local x. Type: any. Declaration start char: 49, length: 1
//@[51:52) Local y. Type: int. Declaration start char: 51, length: 1
//@[7:24) Module expectedLoopBody2. Type: module[]. Declaration start char: 0, length: 60

// wrong loop body type
var emptyArray = []
//@[4:14) Variable emptyArray. Type: array. Declaration start char: 0, length: 19
module wrongLoopBodyType 'modulea.bicep' = [for x in emptyArray:4]
//@[48:49) Local x. Type: any. Declaration start char: 48, length: 1
//@[7:24) Module wrongLoopBodyType. Type: module[]. Declaration start char: 0, length: 66
module wrongLoopBodyType2 'modulea.bicep' = [for (x,i) in emptyArray:4]
//@[50:51) Local x. Type: any. Declaration start char: 50, length: 1
//@[52:53) Local i. Type: int. Declaration start char: 52, length: 1
//@[7:25) Module wrongLoopBodyType2. Type: module[]. Declaration start char: 0, length: 71

// missing loop body properties
module missingLoopBodyProperties 'modulea.bicep' = [for x in emptyArray:{
//@[56:57) Local x. Type: any. Declaration start char: 56, length: 1
//@[7:32) Module missingLoopBodyProperties. Type: module[]. Declaration start char: 0, length: 76
}]
module missingLoopBodyProperties2 'modulea.bicep' = [for (x,i) in emptyArray:{
//@[58:59) Local x. Type: any. Declaration start char: 58, length: 1
//@[60:61) Local i. Type: int. Declaration start char: 60, length: 1
//@[7:33) Module missingLoopBodyProperties2. Type: module[]. Declaration start char: 0, length: 81
}]

// wrong array type
var notAnArray = true
//@[4:14) Variable notAnArray. Type: bool. Declaration start char: 0, length: 21
module wrongArrayType 'modulea.bicep' = [for x in notAnArray:{
//@[45:46) Local x. Type: any. Declaration start char: 45, length: 1
//@[7:21) Module wrongArrayType. Type: module[]. Declaration start char: 0, length: 65
}]

// missing fewer properties
module missingFewerLoopBodyProperties 'modulea.bicep' = [for x in emptyArray:{
//@[61:62) Local x. Type: any. Declaration start char: 61, length: 1
//@[7:37) Module missingFewerLoopBodyProperties. Type: module[]. Declaration start char: 0, length: 119
  name: 'hello-${x}'
  params: {

  }
}]

// wrong parameter in the module loop
module wrongModuleParameterInLoop 'modulea.bicep' = [for x in emptyArray:{
//@[57:58) Local x. Type: any. Declaration start char: 57, length: 1
//@[7:33) Module wrongModuleParameterInLoop. Type: module[]. Declaration start char: 0, length: 263
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
module wrongModuleParameterInLoop2 'modulea.bicep' = [for (x,i) in emptyArray:{
//@[59:60) Local x. Type: any. Declaration start char: 59, length: 1
//@[61:62) Local i. Type: int. Declaration start char: 61, length: 1
//@[7:34) Module wrongModuleParameterInLoop2. Type: module[]. Declaration start char: 0, length: 240
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

// nonexistent arrays and loop variables
var evenMoreDuplicates = 'there'
//@[4:22) Variable evenMoreDuplicates. Type: 'there'. Declaration start char: 0, length: 32
module nonexistentArrays 'modulea.bicep' = [for evenMoreDuplicates in alsoDoesNotExist: {
//@[48:66) Local evenMoreDuplicates. Type: any. Declaration start char: 48, length: 18
//@[7:24) Module nonexistentArrays. Type: module[]. Declaration start char: 0, length: 278
  name: 'hello-${whyChooseRealVariablesWhenWeCanPretend}'
  params: {
    objParam: {}
    stringParamB: 'test'
    arrayParam: [for evenMoreDuplicates in totallyFake: doesNotExist]
//@[21:39) Local evenMoreDuplicates. Type: any. Declaration start char: 21, length: 18
  }
}]

output directRefToCollectionViaOutput array = nonexistentArrays
//@[7:37) Output directRefToCollectionViaOutput. Type: array. Declaration start char: 0, length: 63

module directRefToCollectionViaSingleBody 'modulea.bicep' = {
//@[7:41) Module directRefToCollectionViaSingleBody. Type: module. Declaration start char: 0, length: 203
  name: 'hello'
  params: {
    arrayParam: concat(wrongModuleParameterInLoop, nonexistentArrays)
    objParam: {}
    stringParamB: ''
  }
}

module directRefToCollectionViaSingleConditionalBody 'modulea.bicep' = if(true) {
//@[7:52) Module directRefToCollectionViaSingleConditionalBody. Type: module. Declaration start char: 0, length: 224
  name: 'hello2'
  params: {
    arrayParam: concat(wrongModuleParameterInLoop, nonexistentArrays)
    objParam: {}
    stringParamB: ''
  }
}

module directRefToCollectionViaLoopBody 'modulea.bicep' = [for test in []: {
//@[63:67) Local test. Type: any. Declaration start char: 63, length: 4
//@[7:39) Module directRefToCollectionViaLoopBody. Type: module[]. Declaration start char: 0, length: 220
  name: 'hello3'
  params: {
    arrayParam: concat(wrongModuleParameterInLoop, nonexistentArrays)
    objParam: {}
    stringParamB: ''
  }
}]

module directRefToCollectionViaLoopBodyWithExtraDependsOn 'modulea.bicep' = [for test in []: {
//@[81:85) Local test. Type: any. Declaration start char: 81, length: 4
//@[7:57) Module directRefToCollectionViaLoopBodyWithExtraDependsOn. Type: module[]. Declaration start char: 0, length: 309
  name: 'hello4'
  params: {
    arrayParam: concat(wrongModuleParameterInLoop, nonexistentArrays)
    objParam: {}
    stringParamB: ''
    dependsOn: [
      nonexistentArrays
    ]
  }
  dependsOn: [
    
  ]
}]


// module body that isn't an object
module nonObjectModuleBody 'modulea.bicep' = [for thing in []: 'hello']
//@[50:55) Local thing. Type: any. Declaration start char: 50, length: 5
//@[7:26) Module nonObjectModuleBody. Type: module[]. Declaration start char: 0, length: 71
module nonObjectModuleBody2 'modulea.bicep' = [for thing in []: concat()]
//@[51:56) Local thing. Type: any. Declaration start char: 51, length: 5
//@[7:27) Module nonObjectModuleBody2. Type: module[]. Declaration start char: 0, length: 73
module nonObjectModuleBody3 'modulea.bicep' = [for (thing,i) in []: 'hello']
//@[52:57) Local thing. Type: any. Declaration start char: 52, length: 5
//@[58:59) Local i. Type: int. Declaration start char: 58, length: 1
//@[7:27) Module nonObjectModuleBody3. Type: module[]. Declaration start char: 0, length: 76
module nonObjectModuleBody4 'modulea.bicep' = [for (thing,i) in []: concat()]
//@[52:57) Local thing. Type: any. Declaration start char: 52, length: 5
//@[58:59) Local i. Type: int. Declaration start char: 58, length: 1
//@[7:27) Module nonObjectModuleBody4. Type: module[]. Declaration start char: 0, length: 77

