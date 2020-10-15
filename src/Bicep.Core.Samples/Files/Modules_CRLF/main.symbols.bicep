module modATest './modulea.bicep' = {
//@[7:15) Module modATest. Type: module. Declaration start char: 0, length: 217
  name: 'modATest'
  params: {
    stringParamB: 'hello!'
    objParam: {
      a: 'b'
    }
    arrayParam: [
      {
        a: 'b'
      }
      'abc'
    ]
  }
}

module modB './child/moduleb.bicep' = {
//@[7:11) Module modB. Type: module. Declaration start char: 0, length: 101
  name: 'modB'
  params: {
    location: 'West US'
  }
}

module optionalWithNoParams1 './child/optionalParams.bicep'= {
//@[7:28) Module optionalWithNoParams1. Type: module. Declaration start char: 0, length: 98
  name: 'optionalWithNoParams1'
}

module optionalWithNoParams2 './child/optionalParams.bicep'= {
//@[7:28) Module optionalWithNoParams2. Type: module. Declaration start char: 0, length: 116
  name: 'optionalWithNoParams2'
  params: {
  }
}

module optionalWithAllParams './child/optionalParams.bicep'= {
//@[7:28) Module optionalWithAllParams. Type: module. Declaration start char: 0, length: 210
  name: 'optionalWithNoParams2'
  params: {
    optionalString: 'abc'
    optionalInt: 42
    optionalObj: { }
    optionalArray: [ ]
  }
}

resource resWithDependencies 'Mock.Rp/mockResource@2020-01-01' = {
//@[9:28) Resource resWithDependencies. Type: Mock.Rp/mockResource@2020-01-01. Declaration start char: 0, length: 193
  name: 'harry'
  properties: {
    modADep: modATest.outputs.stringOutputA
    modBDep: modB.outputs.myResourceId
  }
}

output stringOutputA string = modATest.outputs.stringOutputA
//@[7:20) Output stringOutputA. Type: string. Declaration start char: 0, length: 60
output stringOutputB string = modATest.outputs.stringOutputB
//@[7:20) Output stringOutputB. Type: string. Declaration start char: 0, length: 60
output objOutput object = modATest.outputs.objOutput
//@[7:16) Output objOutput. Type: object. Declaration start char: 0, length: 52
output arrayOutput array = modATest.outputs.arrayOutput
//@[7:18) Output arrayOutput. Type: array. Declaration start char: 0, length: 55
