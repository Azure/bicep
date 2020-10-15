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

module modANoInputs './modulea.bicep' = {
  
}

module modAUnspecifiedInputs './modulea.bicep' = {
  stringParamB: ''
  objParam: {}
  objArray: []
  unspecifiedInput: ''
}

var unspecifiedOutput = modAUnspecifiedInputs.test

module moduleWithBackslash 'child\\file.bicep' = {
  
}

module modCycle './cycle.bicep' = {
  
}