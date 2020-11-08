param deployTimeSuffix string = newGuid()

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

module optionalWithNoParams1 './child/optionalParams.bicep'= {
  name: 'optionalWithNoParams1'
}

module optionalWithNoParams2 './child/optionalParams.bicep'= {
  name: 'optionalWithNoParams2'
  params: {
  }
}

module optionalWithAllParams './child/optionalParams.bicep'= {
  name: 'optionalWithNoParams2'
  params: {
    optionalString: 'abc'
    optionalInt: 42
    optionalObj: { }
    optionalArray: [ ]
  }
}

resource resWithDependencies 'Mock.Rp/mockResource@2020-01-01' = {
  name: 'harry'
  properties: {
    modADep: modATest.outputs.stringOutputA
    modBDep: modB.outputs.myResourceId
  }
}

module optionalWithAllParamsAndManualDependency './child/optionalParams.bicep'= {
  name: 'optionalWithAllParamsAndManualDependency'
  params: {
    optionalString: 'abc'
    optionalInt: 42
    optionalObj: { }
    optionalArray: [ ]
  }
  dependsOn: [
    resWithDependencies
    optionalWithAllParams
  ]
}

module optionalWithImplicitDependency './child/optionalParams.bicep'= {
  name: 'optionalWithImplicitDependency'
  params: {
    optionalString: concat(resWithDependencies.id, optionalWithAllParamsAndManualDependency.name)
    optionalInt: 42
    optionalObj: { }
    optionalArray: [ ]
  }
}

module moduleWithCalculatedName './child/optionalParams.bicep'= {
  name: '${optionalWithAllParamsAndManualDependency.name}${deployTimeSuffix}'
  params: {
    optionalString: concat(resWithDependencies.id, optionalWithAllParamsAndManualDependency.name)
    optionalInt: 42
    optionalObj: { }
    optionalArray: [ ]
  }
}

resource resWithCalculatedNameDependencies 'Mock.Rp/mockResource@2020-01-01' = {
  name: '${optionalWithAllParamsAndManualDependency.name}${deployTimeSuffix}'
  properties: {
    modADep: moduleWithCalculatedName.outputs.outputObj
  }
}

output stringOutputA string = modATest.outputs.stringOutputA
output stringOutputB string = modATest.outputs.stringOutputB
output objOutput object = modATest.outputs.objOutput
output arrayOutput array = modATest.outputs.arrayOutput
output modCalculatedNameOutput object = moduleWithCalculatedName.outputs.outputObj
