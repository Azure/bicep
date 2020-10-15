module moduleWithMissingPath './nonExistent.bicep' = {

}

module moduleWithoutPath = {

}

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

module moduleWithBackslash 'child\\file.bicep' = {
  
}

module modCycle './cycle.bicep' = {
  
}