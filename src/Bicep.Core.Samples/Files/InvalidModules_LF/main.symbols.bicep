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

module modANoName './modulea.bicep' = {
//@[7:17) Module modANoName. Type: module. Declaration start char: 0, length: 42

}

module modANoInputs './modulea.bicep' = {
//@[7:19) Module modANoInputs. Type: module. Declaration start char: 0, length: 66
  name: 'modANoInputs'
}

module modAEmptyInputs './modulea.bicep' = {
//@[7:22) Module modAEmptyInputs. Type: module. Declaration start char: 0, length: 86
  name: 'modANoInputs'
  params: {

  }
}

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

module moduleWithBackslash 'child\\file.bicep' = {
//@[7:26) Module moduleWithBackslash. Type: error. Declaration start char: 0, length: 55
  
}

module modCycle './cycle.bicep' = {
//@[7:15) Module modCycle. Type: error. Declaration start char: 0, length: 40
  
}
