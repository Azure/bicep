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

module moduleWithDuplicateName1 './empty.bicep' = {
//@[7:31) Module moduleWithDuplicateName1. Type: module. Declaration start char: 0, length: 112
  name: 'moduleWithDuplicateName'
  scope: resourceGroup()
}

module moduleWithDuplicateName2 './empty.bicep' = {
//@[7:31) Module moduleWithDuplicateName2. Type: module. Declaration start char: 0, length: 87
  name: 'moduleWithDuplicateName'
}

