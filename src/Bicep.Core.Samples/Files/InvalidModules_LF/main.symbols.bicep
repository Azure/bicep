module nonExistentFileRef './nonExistent.bicep' = {
//@[07:25) Module nonExistentFileRef. Type: error. Declaration start char: 0, length: 54

}

// we should only look this file up once, but should still return the same failure
module nonExistentFileRefDuplicate './nonExistent.bicep' = {
//@[07:34) Module nonExistentFileRefDuplicate. Type: error. Declaration start char: 0, length: 63

}

// we should only look this file up once, but should still return the same failure
module nonExistentFileRefEquivalentPath 'abc/def/../../nonExistent.bicep' = {
//@[07:39) Module nonExistentFileRefEquivalentPath. Type: error. Declaration start char: 0, length: 80

}

module moduleWithoutPath = {
//@[07:24) Module moduleWithoutPath. Type: error. Declaration start char: 0, length: 28

}

// #completionTest(41) -> moduleBodyCompletions
module moduleWithPath './moduleb.bicep' =
//@[07:21) Module moduleWithPath. Type: module. Declaration start char: 0, length: 41

// missing identifier #completionTest(7) -> empty
module 
//@[07:07) Module <missing>. Type: error. Declaration start char: 0, length: 7

// #completionTest(24,25) -> moduleObject
module missingValue '' = 
//@[07:19) Module missingValue. Type: error. Declaration start char: 0, length: 25

var interp = 'hello'
//@[04:10) Variable interp. Type: 'hello'. Declaration start char: 0, length: 20
module moduleWithInterpPath './${interp}.bicep' = {
//@[07:27) Module moduleWithInterpPath. Type: error. Declaration start char: 0, length: 54

}

module moduleWithConditionAndInterpPath './${interp}.bicep' = if (true) {
//@[07:39) Module moduleWithConditionAndInterpPath. Type: error. Declaration start char: 0, length: 76

}

module moduleWithSelfCycle './main.bicep' = {
//@[07:26) Module moduleWithSelfCycle. Type: error. Declaration start char: 0, length: 48

}

module moduleWithConditionAndSelfCycle './main.bicep' = if ('foo' == 'bar') {
//@[07:38) Module moduleWithConditionAndSelfCycle. Type: error. Declaration start char: 0, length: 80

}

module './main.bicep' = {
//@[07:07) Module <missing>. Type: error. Declaration start char: 0, length: 28

}

module './main.bicep' = if (1 + 2 == 3) {
//@[07:07) Module <missing>. Type: error. Declaration start char: 0, length: 44

}

module './main.bicep' = if
//@[07:07) Module <missing>. Type: error. Declaration start char: 0, length: 26

module './main.bicep' = if (
//@[07:07) Module <missing>. Type: error. Declaration start char: 0, length: 28

module './main.bicep' = if (true
//@[07:07) Module <missing>. Type: error. Declaration start char: 0, length: 32

module './main.bicep' = if (true)
//@[07:07) Module <missing>. Type: error. Declaration start char: 0, length: 33

module './main.bicep' = if {
//@[07:07) Module <missing>. Type: error. Declaration start char: 0, length: 31

}

module './main.bicep' = if () {
//@[07:07) Module <missing>. Type: error. Declaration start char: 0, length: 34

}

module './main.bicep' = if ('true') {
//@[07:07) Module <missing>. Type: error. Declaration start char: 0, length: 40

}

module modANoName './modulea.bicep' = {
//@[07:17) Module modANoName. Type: module. Declaration start char: 0, length: 93
// #completionTest(0) -> moduleATopLevelProperties

}

module modANoNameWithCondition './modulea.bicep' = if (true) {
//@[07:30) Module modANoNameWithCondition. Type: module. Declaration start char: 0, length: 129
// #completionTest(0) -> moduleAWithConditionTopLevelProperties

}

module modWithReferenceInCondition './main.bicep' = if (reference('Micorosft.Management/managementGroups/MG', '2020-05-01').name == 'something') {
//@[07:34) Module modWithReferenceInCondition. Type: error. Declaration start char: 0, length: 149

}

module modWithListKeysInCondition './main.bicep' = if (listKeys('foo', '2020-05-01').bar == true) {
//@[07:33) Module modWithListKeysInCondition. Type: error. Declaration start char: 0, length: 102

}


module modANoName './modulea.bicep' = if ({ 'a': b }.a == true) {
//@[07:17) Module modANoName. Type: error. Declaration start char: 0, length: 68

}

module modANoInputs './modulea.bicep' = {
//@[07:19) Module modANoInputs. Type: module. Declaration start char: 0, length: 135
  name: 'modANoInputs'
  // #completionTest(0,1,2) -> moduleATopLevelPropertiesMinusName
  
}

module modANoInputsWithCondition './modulea.bicep' = if (length([
//@[07:32) Module modANoInputsWithCondition. Type: module. Declaration start char: 0, length: 191
  'foo'
]) == 1) {
  name: 'modANoInputs'
  // #completionTest(0,1,2) -> moduleAWithConditionTopLevelPropertiesMinusName
  
}

module modAEmptyInputs './modulea.bicep' = {
//@[07:22) Module modAEmptyInputs. Type: module. Declaration start char: 0, length: 141
  name: 'modANoInputs'
  params: {
    // #completionTest(0,1,2,3,4) -> moduleAParams
    
  }
}

module modAEmptyInputsWithCondition './modulea.bicep' = if (1 + 2 == 2) {
//@[07:35) Module modAEmptyInputsWithCondition. Type: module. Declaration start char: 0, length: 183
  name: 'modANoInputs'
  params: {
    // #completionTest(0,1,2,3,4) -> moduleAWithConditionParams
    
  }
}

// #completionTest(55) -> moduleATopLevelPropertyAccess
var modulePropertyAccessCompletions = modAEmptyInputs.o
//@[04:35) Variable modulePropertyAccessCompletions. Type: error. Declaration start char: 0, length: 55

// #completionTest(81) -> moduleAWithConditionTopLevelPropertyAccess
var moduleWithConditionPropertyAccessCompletions = modAEmptyInputsWithCondition.o
//@[04:48) Variable moduleWithConditionPropertyAccessCompletions. Type: error. Declaration start char: 0, length: 81

// #completionTest(56) -> moduleAOutputs
var moduleOutputsCompletions = modAEmptyInputs.outputs.s
//@[04:28) Variable moduleOutputsCompletions. Type: error. Declaration start char: 0, length: 56

// #completionTest(82) -> moduleAWithConditionOutputs
var moduleWithConditionOutputsCompletions = modAEmptyInputsWithCondition.outputs.s
//@[04:41) Variable moduleWithConditionOutputsCompletions. Type: error. Declaration start char: 0, length: 82

module modAUnspecifiedInputs './modulea.bicep' = {
//@[07:28) Module modAUnspecifiedInputs. Type: module. Declaration start char: 0, length: 180
  name: 'modAUnspecifiedInputs'
  params: {
    stringParamB: ''
    objParam: {}
    objArray: []
    unspecifiedInput: ''
  }
}

var unspecifiedOutput = modAUnspecifiedInputs.outputs.test
//@[04:21) Variable unspecifiedOutput. Type: error. Declaration start char: 0, length: 58

module modCycle './cycle.bicep' = {
//@[07:15) Module modCycle. Type: error. Declaration start char: 0, length: 40
  
}

module moduleWithEmptyPath '' = {
//@[07:26) Module moduleWithEmptyPath. Type: error. Declaration start char: 0, length: 35
}

module moduleWithAbsolutePath '/abc/def.bicep' = {
//@[07:29) Module moduleWithAbsolutePath. Type: error. Declaration start char: 0, length: 52
}

module moduleWithBackslash 'child\\file.bicep' = {
//@[07:26) Module moduleWithBackslash. Type: error. Declaration start char: 0, length: 52
}

module moduleWithInvalidChar 'child/fi|le.bicep' = {
//@[07:28) Module moduleWithInvalidChar. Type: error. Declaration start char: 0, length: 54
}

module moduleWithInvalidTerminatorChar 'child/test.' = {
//@[07:38) Module moduleWithInvalidTerminatorChar. Type: error. Declaration start char: 0, length: 58
}

module moduleWithValidScope './empty.bicep' = {
//@[07:27) Module moduleWithValidScope. Type: module. Declaration start char: 0, length: 80
  name: 'moduleWithValidScope'
}

module moduleWithInvalidScope './empty.bicep' = {
//@[07:29) Module moduleWithInvalidScope. Type: module. Declaration start char: 0, length: 114
  name: 'moduleWithInvalidScope'
  scope: moduleWithValidScope
}

module moduleWithMissingRequiredScope './subscription_empty.bicep' = {
//@[07:37) Module moduleWithMissingRequiredScope. Type: module. Declaration start char: 0, length: 113
  name: 'moduleWithMissingRequiredScope'
}

module moduleWithInvalidScope2 './empty.bicep' = {
//@[07:30) Module moduleWithInvalidScope2. Type: module. Declaration start char: 0, length: 113
  name: 'moduleWithInvalidScope2'
  scope: managementGroup()
}

module moduleWithUnsupprtedScope1 './mg_empty.bicep' = {
//@[07:33) Module moduleWithUnsupprtedScope1. Type: module. Declaration start char: 0, length: 122
  name: 'moduleWithUnsupprtedScope1'
  scope: managementGroup()
}

module moduleWithUnsupprtedScope2 './mg_empty.bicep' = {
//@[07:33) Module moduleWithUnsupprtedScope2. Type: module. Declaration start char: 0, length: 126
  name: 'moduleWithUnsupprtedScope2'
  scope: managementGroup('MG')
}

module moduleWithBadScope './empty.bicep' = {
//@[07:25) Module moduleWithBadScope. Type: module. Declaration start char: 0, length: 99
  name: 'moduleWithBadScope'
  scope: 'stringScope'
}

resource runtimeValidRes1 'Microsoft.Storage/storageAccounts@2019-06-01' = {
//@[09:25) Resource runtimeValidRes1. Type: Microsoft.Storage/storageAccounts@2019-06-01. Declaration start char: 0, length: 190
  name: 'runtimeValidRes1Name'
  location: 'westeurope'
  kind: 'Storage'
  sku: {
    name: 'Standard_GRS'
  }
}

module runtimeValidModule1 'empty.bicep' = {
//@[07:26) Module runtimeValidModule1. Type: module. Declaration start char: 0, length: 136
  name: concat(concat(runtimeValidRes1.id, runtimeValidRes1.name), runtimeValidRes1.type)
}

module runtimeInvalidModule1 'empty.bicep' = {
//@[07:28) Module runtimeInvalidModule1. Type: module. Declaration start char: 0, length: 82
  name: runtimeValidRes1.location
}

module runtimeInvalidModule2 'empty.bicep' = {
//@[07:28) Module runtimeInvalidModule2. Type: module. Declaration start char: 0, length: 85
  name: runtimeValidRes1['location']
}

module runtimeInvalidModule3 'empty.bicep' = {
//@[07:28) Module runtimeInvalidModule3. Type: module. Declaration start char: 0, length: 82
  name: runtimeValidRes1.sku.name
}

module runtimeInvalidModule4 'empty.bicep' = {
//@[07:28) Module runtimeInvalidModule4. Type: module. Declaration start char: 0, length: 85
  name: runtimeValidRes1.sku['name']
}

module runtimeInvalidModule5 'empty.bicep' = {
//@[07:28) Module runtimeInvalidModule5. Type: module. Declaration start char: 0, length: 88
  name: runtimeValidRes1['sku']['name']
}

module runtimeInvalidModule6 'empty.bicep' = {
//@[07:28) Module runtimeInvalidModule6. Type: module. Declaration start char: 0, length: 85
  name: runtimeValidRes1['sku'].name
}

module singleModuleForRuntimeCheck 'modulea.bicep' = {
//@[07:34) Module singleModuleForRuntimeCheck. Type: module. Declaration start char: 0, length: 71
  name: 'test'
}

var moduleRuntimeCheck = singleModuleForRuntimeCheck.outputs.stringOutputA
//@[04:22) Variable moduleRuntimeCheck. Type: string. Declaration start char: 0, length: 74
var moduleRuntimeCheck2 = moduleRuntimeCheck
//@[04:23) Variable moduleRuntimeCheck2. Type: string. Declaration start char: 0, length: 44

module moduleLoopForRuntimeCheck 'modulea.bicep' = [for thing in []: {
//@[56:61) Local thing. Type: any. Declaration start char: 56, length: 5
//@[07:32) Module moduleLoopForRuntimeCheck. Type: module[]. Declaration start char: 0, length: 101
  name: moduleRuntimeCheck2
}]

var moduleRuntimeCheck3 = moduleLoopForRuntimeCheck[1].outputs.stringOutputB
//@[04:23) Variable moduleRuntimeCheck3. Type: string. Declaration start char: 0, length: 76
var moduleRuntimeCheck4 = moduleRuntimeCheck3
//@[04:23) Variable moduleRuntimeCheck4. Type: string. Declaration start char: 0, length: 45
module moduleLoopForRuntimeCheck2 'modulea.bicep' = [for thing in []: {
//@[57:62) Local thing. Type: any. Declaration start char: 57, length: 5
//@[07:33) Module moduleLoopForRuntimeCheck2. Type: module[]. Declaration start char: 0, length: 102
  name: moduleRuntimeCheck4
}]

module moduleLoopForRuntimeCheck3 'modulea.bicep' = [for thing in []: {
//@[57:62) Local thing. Type: any. Declaration start char: 57, length: 5
//@[07:33) Module moduleLoopForRuntimeCheck3. Type: module[]. Declaration start char: 0, length: 194
  name: concat(moduleLoopForRuntimeCheck[1].outputs.stringOutputB, moduleLoopForRuntimeCheck[1].outputs.stringOutputA )
}]

module moduleWithDuplicateName1 './empty.bicep' = {
//@[07:31) Module moduleWithDuplicateName1. Type: module. Declaration start char: 0, length: 112
  name: 'moduleWithDuplicateName'
  scope: resourceGroup()
}

module moduleWithDuplicateName2 './empty.bicep' = {
//@[07:31) Module moduleWithDuplicateName2. Type: module. Declaration start char: 0, length: 87
  name: 'moduleWithDuplicateName'
}

// #completionTest(19, 20, 21) -> cwdFileCompletions
module completionB ''
//@[07:18) Module completionB. Type: error. Declaration start char: 0, length: 21

// #completionTest(19, 20, 21) -> cwdFileCompletions
module completionC '' =
//@[07:18) Module completionC. Type: error. Declaration start char: 0, length: 23

// #completionTest(19, 20, 21) -> cwdFileCompletions
module completionD '' = {}
//@[07:18) Module completionD. Type: error. Declaration start char: 0, length: 26

// #completionTest(19, 20, 21) -> cwdFileCompletions
module completionE '' = {
//@[07:18) Module completionE. Type: error. Declaration start char: 0, length: 43
  name: 'hello'
}

// #completionTest(29) -> cwdDotFileCompletions
module cwdFileCompletionA './m'
//@[07:25) Module cwdFileCompletionA. Type: error. Declaration start char: 0, length: 31

// #completionTest(26, 27) -> cwdFileCompletions
module cwdFileCompletionB m
//@[07:25) Module cwdFileCompletionB. Type: error. Declaration start char: 0, length: 27

// #completionTest(26, 27, 28, 29) -> cwdFileCompletions
module cwdFileCompletionC 'm'
//@[07:25) Module cwdFileCompletionC. Type: error. Declaration start char: 0, length: 29

// #completionTest(24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39) -> childFileCompletions
module childCompletionA 'ChildModules/'
//@[07:23) Module childCompletionA. Type: error. Declaration start char: 0, length: 39

// #completionTest(24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39) -> childDotFileCompletions
module childCompletionB './ChildModules/'
//@[07:23) Module childCompletionB. Type: error. Declaration start char: 0, length: 41

// #completionTest(24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40) -> childDotFileCompletions
module childCompletionC './ChildModules/m'
//@[07:23) Module childCompletionC. Type: error. Declaration start char: 0, length: 42

// #completionTest(24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40) -> childFileCompletions
module childCompletionD 'ChildModules/e'
//@[07:23) Module childCompletionD. Type: error. Declaration start char: 0, length: 40

@minValue()
module moduleWithNotAttachableDecorators './empty.bicep' = {
//@[07:40) Module moduleWithNotAttachableDecorators. Type: module. Declaration start char: 0, length: 118
  name: 'moduleWithNotAttachableDecorators'
}

// loop parsing cases
module expectedForKeyword 'modulea.bicep' = []
//@[07:25) Module expectedForKeyword. Type: module. Declaration start char: 0, length: 46

module expectedForKeyword2 'modulea.bicep' = [f]
//@[07:26) Module expectedForKeyword2. Type: module. Declaration start char: 0, length: 48

module expectedLoopVar 'modulea.bicep' = [for]
//@[07:22) Module expectedLoopVar. Type: module[]. Declaration start char: 0, length: 46

module expectedInKeyword 'modulea.bicep' = [for x]
//@[48:49) Local x. Type: any. Declaration start char: 48, length: 1
//@[07:24) Module expectedInKeyword. Type: module[]. Declaration start char: 0, length: 50

module expectedInKeyword2 'modulea.bicep' = [for x b]
//@[49:50) Local x. Type: any. Declaration start char: 49, length: 1
//@[07:25) Module expectedInKeyword2. Type: module[]. Declaration start char: 0, length: 53

module expectedArrayExpression 'modulea.bicep' = [for x in]
//@[54:55) Local x. Type: any. Declaration start char: 54, length: 1
//@[07:30) Module expectedArrayExpression. Type: module[]. Declaration start char: 0, length: 59

module expectedColon 'modulea.bicep' = [for x in y]
//@[44:45) Local x. Type: any. Declaration start char: 44, length: 1
//@[07:20) Module expectedColon. Type: module[]. Declaration start char: 0, length: 51

module expectedLoopBody 'modulea.bicep' = [for x in y:]
//@[47:48) Local x. Type: any. Declaration start char: 47, length: 1
//@[07:23) Module expectedLoopBody. Type: module[]. Declaration start char: 0, length: 55

// indexed loop parsing cases
module expectedItemVarName 'modulea.bicep' = [for ()]
//@[07:26) Module expectedItemVarName. Type: module[]. Declaration start char: 0, length: 53

module expectedComma 'modulea.bicep' = [for (x)]
//@[07:20) Module expectedComma. Type: module[]. Declaration start char: 0, length: 48

module expectedIndexVarName 'modulea.bicep' = [for (x,)]
//@[07:27) Module expectedIndexVarName. Type: module[]. Declaration start char: 0, length: 56

module expectedInKeyword3 'modulea.bicep' = [for (x,y)]
//@[50:51) Local x. Type: any. Declaration start char: 50, length: 1
//@[52:53) Local y. Type: int. Declaration start char: 52, length: 1
//@[07:25) Module expectedInKeyword3. Type: module[]. Declaration start char: 0, length: 55

module expectedArrayExpression2 'modulea.bicep' = [for (x,y) in ]
//@[56:57) Local x. Type: any. Declaration start char: 56, length: 1
//@[58:59) Local y. Type: int. Declaration start char: 58, length: 1
//@[07:31) Module expectedArrayExpression2. Type: module[]. Declaration start char: 0, length: 65

module expectedColon2 'modulea.bicep' = [for (x,y) in z]
//@[46:47) Local x. Type: any. Declaration start char: 46, length: 1
//@[48:49) Local y. Type: int. Declaration start char: 48, length: 1
//@[07:21) Module expectedColon2. Type: module[]. Declaration start char: 0, length: 56

module expectedLoopBody2 'modulea.bicep' = [for (x,y) in z:]
//@[49:50) Local x. Type: any. Declaration start char: 49, length: 1
//@[51:52) Local y. Type: int. Declaration start char: 51, length: 1
//@[07:24) Module expectedLoopBody2. Type: module[]. Declaration start char: 0, length: 60

// loop filter parsing cases
module expectedLoopFilterOpenParen 'modulea.bicep' = [for x in y: if]
//@[58:59) Local x. Type: any. Declaration start char: 58, length: 1
//@[07:34) Module expectedLoopFilterOpenParen. Type: module[]. Declaration start char: 0, length: 69
module expectedLoopFilterOpenParen2 'modulea.bicep' = [for (x,y) in z: if]
//@[60:61) Local x. Type: any. Declaration start char: 60, length: 1
//@[62:63) Local y. Type: int. Declaration start char: 62, length: 1
//@[07:35) Module expectedLoopFilterOpenParen2. Type: module[]. Declaration start char: 0, length: 74

module expectedLoopFilterPredicateAndBody 'modulea.bicep' = [for x in y: if()]
//@[65:66) Local x. Type: any. Declaration start char: 65, length: 1
//@[07:41) Module expectedLoopFilterPredicateAndBody. Type: module[]. Declaration start char: 0, length: 78
module expectedLoopFilterPredicateAndBody2 'modulea.bicep' = [for (x,y) in z: if()]
//@[67:68) Local x. Type: any. Declaration start char: 67, length: 1
//@[69:70) Local y. Type: int. Declaration start char: 69, length: 1
//@[07:42) Module expectedLoopFilterPredicateAndBody2. Type: module[]. Declaration start char: 0, length: 83

// wrong loop body type
var emptyArray = []
//@[04:14) Variable emptyArray. Type: array. Declaration start char: 0, length: 19
module wrongLoopBodyType 'modulea.bicep' = [for x in emptyArray:4]
//@[48:49) Local x. Type: any. Declaration start char: 48, length: 1
//@[07:24) Module wrongLoopBodyType. Type: module[]. Declaration start char: 0, length: 66
module wrongLoopBodyType2 'modulea.bicep' = [for (x,i) in emptyArray:4]
//@[50:51) Local x. Type: any. Declaration start char: 50, length: 1
//@[52:53) Local i. Type: int. Declaration start char: 52, length: 1
//@[07:25) Module wrongLoopBodyType2. Type: module[]. Declaration start char: 0, length: 71

// missing loop body properties
module missingLoopBodyProperties 'modulea.bicep' = [for x in emptyArray:{
//@[56:57) Local x. Type: any. Declaration start char: 56, length: 1
//@[07:32) Module missingLoopBodyProperties. Type: module[]. Declaration start char: 0, length: 76
}]
module missingLoopBodyProperties2 'modulea.bicep' = [for (x,i) in emptyArray:{
//@[58:59) Local x. Type: any. Declaration start char: 58, length: 1
//@[60:61) Local i. Type: int. Declaration start char: 60, length: 1
//@[07:33) Module missingLoopBodyProperties2. Type: module[]. Declaration start char: 0, length: 81
}]

// wrong array type
var notAnArray = true
//@[04:14) Variable notAnArray. Type: bool. Declaration start char: 0, length: 21
module wrongArrayType 'modulea.bicep' = [for x in notAnArray:{
//@[45:46) Local x. Type: any. Declaration start char: 45, length: 1
//@[07:21) Module wrongArrayType. Type: module[]. Declaration start char: 0, length: 65
}]

// missing fewer properties
module missingFewerLoopBodyProperties 'modulea.bicep' = [for x in emptyArray:{
//@[61:62) Local x. Type: any. Declaration start char: 61, length: 1
//@[07:37) Module missingFewerLoopBodyProperties. Type: module[]. Declaration start char: 0, length: 119
  name: 'hello-${x}'
  params: {

  }
}]

// wrong parameter in the module loop
module wrongModuleParameterInLoop 'modulea.bicep' = [for x in emptyArray:{
//@[57:58) Local x. Type: any. Declaration start char: 57, length: 1
//@[07:33) Module wrongModuleParameterInLoop. Type: module[]. Declaration start char: 0, length: 263
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
module wrongModuleParameterInFilteredLoop 'modulea.bicep' = [for x in emptyArray: if(true) {
//@[65:66) Local x. Type: any. Declaration start char: 65, length: 1
//@[07:41) Module wrongModuleParameterInFilteredLoop. Type: module[]. Declaration start char: 0, length: 284
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
module wrongModuleParameterInLoop2 'modulea.bicep' = [for (x,i) in emptyArray:{
//@[59:60) Local x. Type: any. Declaration start char: 59, length: 1
//@[61:62) Local i. Type: int. Declaration start char: 61, length: 1
//@[07:34) Module wrongModuleParameterInLoop2. Type: module[]. Declaration start char: 0, length: 240
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

module paramNameCompletionsInFilteredLoops 'modulea.bicep' = [for (x,i) in emptyArray: if(true) {
//@[67:68) Local x. Type: any. Declaration start char: 67, length: 1
//@[69:70) Local i. Type: int. Declaration start char: 69, length: 1
//@[07:42) Module paramNameCompletionsInFilteredLoops. Type: module[]. Declaration start char: 0, length: 187
  name: 'hello-${x}'
  params: {
    // #completionTest(0,1,2) -> moduleAParams
  
  }
}]

// #completionTest(100) -> moduleAOutputs
var propertyAccessCompletionsForFilteredModuleLoop = paramNameCompletionsInFilteredLoops[0].outputs.
//@[04:50) Variable propertyAccessCompletionsForFilteredModuleLoop. Type: error. Declaration start char: 0, length: 100

// nonexistent arrays and loop variables
var evenMoreDuplicates = 'there'
//@[04:22) Variable evenMoreDuplicates. Type: 'there'. Declaration start char: 0, length: 32
module nonexistentArrays 'modulea.bicep' = [for evenMoreDuplicates in alsoDoesNotExist: {
//@[48:66) Local evenMoreDuplicates. Type: any. Declaration start char: 48, length: 18
//@[07:24) Module nonexistentArrays. Type: module[]. Declaration start char: 0, length: 278
  name: 'hello-${whyChooseRealVariablesWhenWeCanPretend}'
  params: {
    objParam: {}
    stringParamB: 'test'
    arrayParam: [for evenMoreDuplicates in totallyFake: doesNotExist]
//@[21:39) Local evenMoreDuplicates. Type: any. Declaration start char: 21, length: 18
  }
}]

output directRefToCollectionViaOutput array = nonexistentArrays
//@[07:37) Output directRefToCollectionViaOutput. Type: array. Declaration start char: 0, length: 63

module directRefToCollectionViaSingleBody 'modulea.bicep' = {
//@[07:41) Module directRefToCollectionViaSingleBody. Type: module. Declaration start char: 0, length: 203
  name: 'hello'
  params: {
    arrayParam: concat(wrongModuleParameterInLoop, nonexistentArrays)
    objParam: {}
    stringParamB: ''
  }
}

module directRefToCollectionViaSingleConditionalBody 'modulea.bicep' = if(true) {
//@[07:52) Module directRefToCollectionViaSingleConditionalBody. Type: module. Declaration start char: 0, length: 224
  name: 'hello2'
  params: {
    arrayParam: concat(wrongModuleParameterInLoop, nonexistentArrays)
    objParam: {}
    stringParamB: ''
  }
}

module directRefToCollectionViaLoopBody 'modulea.bicep' = [for test in []: {
//@[63:67) Local test. Type: any. Declaration start char: 63, length: 4
//@[07:39) Module directRefToCollectionViaLoopBody. Type: module[]. Declaration start char: 0, length: 220
  name: 'hello3'
  params: {
    arrayParam: concat(wrongModuleParameterInLoop, nonexistentArrays)
    objParam: {}
    stringParamB: ''
  }
}]

module directRefToCollectionViaLoopBodyWithExtraDependsOn 'modulea.bicep' = [for test in []: {
//@[81:85) Local test. Type: any. Declaration start char: 81, length: 4
//@[07:57) Module directRefToCollectionViaLoopBodyWithExtraDependsOn. Type: module[]. Declaration start char: 0, length: 309
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
//@[07:26) Module nonObjectModuleBody. Type: module[]. Declaration start char: 0, length: 71
module nonObjectModuleBody2 'modulea.bicep' = [for thing in []: concat()]
//@[51:56) Local thing. Type: any. Declaration start char: 51, length: 5
//@[07:27) Module nonObjectModuleBody2. Type: module[]. Declaration start char: 0, length: 73
module nonObjectModuleBody3 'modulea.bicep' = [for (thing,i) in []: 'hello']
//@[52:57) Local thing. Type: any. Declaration start char: 52, length: 5
//@[58:59) Local i. Type: int. Declaration start char: 58, length: 1
//@[07:27) Module nonObjectModuleBody3. Type: module[]. Declaration start char: 0, length: 76
module nonObjectModuleBody4 'modulea.bicep' = [for (thing,i) in []: concat()]
//@[52:57) Local thing. Type: any. Declaration start char: 52, length: 5
//@[58:59) Local i. Type: int. Declaration start char: 58, length: 1
//@[07:27) Module nonObjectModuleBody4. Type: module[]. Declaration start char: 0, length: 77

module anyTypeInScope 'empty.bicep' = {
//@[07:21) Module anyTypeInScope. Type: module. Declaration start char: 0, length: 91
  dependsOn: [
    any('s')
  ]

  scope: any(42)
}

module anyTypeInScopeConditional 'empty.bicep' = if(false) {
//@[07:32) Module anyTypeInScopeConditional. Type: module. Declaration start char: 0, length: 112
  dependsOn: [
    any('s')
  ]

  scope: any(42)
}

module anyTypeInScopeLoop 'empty.bicep' = [for thing in []: {
//@[47:52) Local thing. Type: any. Declaration start char: 47, length: 5
//@[07:25) Module anyTypeInScopeLoop. Type: module[]. Declaration start char: 0, length: 114
  dependsOn: [
    any('s')
  ]

  scope: any(42)
}]

// Key Vault Secret Reference

resource kv 'Microsoft.KeyVault/vaults@2019-09-01' existing = {
//@[09:11) Resource kv. Type: Microsoft.KeyVault/vaults@2019-09-01. Declaration start char: 0, length: 88
  name: 'testkeyvault'
}

module secureModule1 'moduleb.bicep' = {
//@[07:20) Module secureModule1. Type: module. Declaration start char: 0, length: 464
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
//@[07:20) Module secureModule2. Type: error. Declaration start char: 0, length: 134
  name: 'secureModule2'
  params: {       
    secret: kv.getSecret('mySecret')
  }
}

module issue3000 'empty.bicep' = {
//@[07:16) Module issue3000. Type: module. Declaration start char: 0, length: 305
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

module invalidJsonMod 'modulec.json' = {
//@[07:21) Module invalidJsonMod. Type: module. Declaration start char: 0, length: 42
}

module jsonModMissingParam 'moduled.json' = {
//@[07:26) Module jsonModMissingParam. Type: module. Declaration start char: 0, length: 119
  name: 'jsonModMissingParam'
  params: {
    foo: 123
    baz: 'C'
  }
}

module assignToOutput 'empty.bicep' = {
//@[07:21) Module assignToOutput. Type: module. Declaration start char: 0, length: 80
  name: 'assignToOutput'
  outputs: {}
}
