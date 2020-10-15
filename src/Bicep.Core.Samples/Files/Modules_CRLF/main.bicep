module modATest './modulea.bicep' = {
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
  name: 'modB'
  params: {
    location: 'West US'
  }
}

resource resWithDependencies 'Mock.Rp/mockResource@2020-01-01' = {
  name: 'harry'
  properties: {
    modADep: modATest.outputs.stringOutputA
    modBDep: modB.outputs.myResourceId
  }
}

output stringOutputA string = modATest.outputs.stringOutputA
output stringOutputB string = modATest.outputs.stringOutputB
output objOutput object = modATest.outputs.objOutput
output arrayOutput array = modATest.outputs.arrayOutput