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

module moduleWithSelfCycle './main.bicep' = {

}

module './main.bicep' = {

}

module modANoName './modulea.bicep' = {

}

module modANoInputs './modulea.bicep' = {
  name: 'modANoInputs'
}

module modAEmptyInputs './modulea.bicep' = {
  name: 'modANoInputs'
  params: {

  }
}

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