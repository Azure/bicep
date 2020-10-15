module moduleWithMissingPath './nonExistent.bicep' = {
//@[7:28) Module moduleWithMissingPath. Type: error. Declaration start char: 0, length: 57

}

module moduleWithoutPath = {
//@[7:24) Module moduleWithoutPath. Type: error. Declaration start char: 0, length: 28

}

var interp = 'hello'
//@[4:10) Variable interp. Type: 'hello'. Declaration start char: 0, length: 20
module moduleWithInterpPath './${interp}.bicep' = {
//@[7:27) Module moduleWithInterpPath. Type: error. Declaration start char: 0, length: 54

}

module moduleWithSelfCycle './main.bicep' = {
//@[7:26) Module moduleWithSelfCycle. Type: error. Declaration start char: 0, length: 48

}

module './main.bicep' = {
//@[7:7) Module <missing>. Type: error. Declaration start char: 0, length: 28

}

module modANoInputs './modulea.bicep' = {
//@[7:19) Module modANoInputs. Type: modANoInputs. Declaration start char: 0, length: 46
  
}

module modAUnspecifiedInputs './modulea.bicep' = {
//@[7:28) Module modAUnspecifiedInputs. Type: modAUnspecifiedInputs. Declaration start char: 0, length: 124
  stringParamB: ''
  objParam: {}
  objArray: []
  unspecifiedInput: ''
}

var unspecifiedOutput = modAUnspecifiedInputs.test
//@[4:21) Variable unspecifiedOutput. Type: error. Declaration start char: 0, length: 50

module moduleWithBackslash 'child\\file.bicep' = {
//@[7:26) Module moduleWithBackslash. Type: error. Declaration start char: 0, length: 55
  
}

module modCycle './cycle.bicep' = {
//@[7:15) Module modCycle. Type: error. Declaration start char: 0, length: 40
  
}
