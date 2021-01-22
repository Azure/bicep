module nonExistentFileRef './nonExistent.bicep' = {

}

// we should only look this file up once, but should still return the same failure
module nonExistentFileRefDuplicate './nonExistent.bicep' = {

}

// we should only look this file up once, but should still return the same failure
module nonExistentFileRefEquivalentPath 'abc/def/../../nonExistent.bicep' = {

}

module moduleWithoutPath = {

}

// missing identifier #completionTest(7) -> empty
module 

// #completionTest(24,25) -> object
module missingValue '' = 

var interp = 'hello'
module moduleWithInterpPath './${interp}.bicep' = {

}

module moduleWithConditionAndInterpPath './${interp}.bicep' = if (true) {

}

module moduleWithSelfCycle './main.bicep' = {

}

module moduleWithConditionAndSelfCycle './main.bicep' = if ('foo' == 'bar') {

}

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

module modWithReferenceInCondition './main.bicep' = if (reference('Micorosft.Management/managementGroups/MG', '2020-05-01').name == 'something') {

}

module modWithListKeysInCondition './main.bicep' = if (listKeys('foo', '2020-05-01').bar == true) {

}


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

module modCycle './cycle.bicep' = {
  
}

module moduleWithEmptyPath '' = {
}

module moduleWithAbsolutePath '/abc/def.bicep' = {
}

module moduleWithBackslash 'child\\file.bicep' = {
}

module moduleWithInvalidChar 'child/fi|le.bicep' = {
}

module moduleWithInvalidTerminatorChar 'child/test.' = {
}

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

module moduleWithDuplicateName1 './empty.bicep' = {
  name: 'moduleWithDuplicateName'
  scope: resourceGroup()
}

module moduleWithDuplicateName2 './empty.bicep' = {
  name: 'moduleWithDuplicateName'
}

//@az.secure()
@minValue()
module moduleWithNotAttachableDecorators './empty.bicep' = {
  name: 'moduleWithNotAttachableDecorators'
}
