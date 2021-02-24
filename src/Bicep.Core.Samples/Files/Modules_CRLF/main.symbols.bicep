param deployTimeSuffix string = newGuid()
//@[6:22) Parameter deployTimeSuffix. Type: string. Declaration start char: 0, length: 41

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

module modBWithCondition './child/moduleb.bicep' = if (1 + 1 == 2) {
//@[7:24) Module modBWithCondition. Type: module. Declaration start char: 0, length: 143
  name: 'modBWithCondition'
  params: {
    location: 'East US'
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
  name: 'optionalWithNoParams3'
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

module optionalWithAllParamsAndManualDependency './child/optionalParams.bicep'= {
//@[7:47) Module optionalWithAllParamsAndManualDependency. Type: module. Declaration start char: 0, length: 321
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
//@[7:37) Module optionalWithImplicitDependency. Type: module. Declaration start char: 0, length: 300
  name: 'optionalWithImplicitDependency'
  params: {
    optionalString: concat(resWithDependencies.id, optionalWithAllParamsAndManualDependency.name)
    optionalInt: 42
    optionalObj: { }
    optionalArray: [ ]
  }
}

module moduleWithCalculatedName './child/optionalParams.bicep'= {
//@[7:31) Module moduleWithCalculatedName. Type: module. Declaration start char: 0, length: 331
  name: '${optionalWithAllParamsAndManualDependency.name}${deployTimeSuffix}'
  params: {
    optionalString: concat(resWithDependencies.id, optionalWithAllParamsAndManualDependency.name)
    optionalInt: 42
    optionalObj: { }
    optionalArray: [ ]
  }
}

resource resWithCalculatedNameDependencies 'Mock.Rp/mockResource@2020-01-01' = {
//@[9:42) Resource resWithCalculatedNameDependencies. Type: Mock.Rp/mockResource@2020-01-01. Declaration start char: 0, length: 241
  name: '${optionalWithAllParamsAndManualDependency.name}${deployTimeSuffix}'
  properties: {
    modADep: moduleWithCalculatedName.outputs.outputObj
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
output modCalculatedNameOutput object = moduleWithCalculatedName.outputs.outputObj
//@[7:30) Output modCalculatedNameOutput. Type: object. Declaration start char: 0, length: 82

/*
  valid loop cases
*/ 
var myModules = [
//@[4:13) Variable myModules. Type: array. Declaration start char: 0, length: 123
  {
    name: 'one'
    location: 'eastus2'
  }
  {
    name: 'two'
    location: 'westus'
  }
]

var emptyArray = []
//@[4:14) Variable emptyArray. Type: array. Declaration start char: 0, length: 19

// simple module loop
module storageResources 'modulea.bicep' = [for module in myModules: {
//@[47:53) Local module. Type: any. Declaration start char: 47, length: 6
//@[7:23) Module storageResources. Type: module[]. Declaration start char: 0, length: 189
  name: module.name
  params: {
    arrayParam: []
    objParam: module
    stringParamB: module.location
  }
}]

// nested module loop
module nestedModuleLoop 'modulea.bicep' = [for module in myModules: {
//@[47:53) Local module. Type: any. Declaration start char: 47, length: 6
//@[7:23) Module nestedModuleLoop. Type: module[]. Declaration start char: 0, length: 246
  name: module.name
  params: {
    arrayParam: [for i in range(0,3): concat('test-', i, '-', module.name)]
//@[21:22) Local i. Type: int. Declaration start char: 21, length: 1
    objParam: module
    stringParamB: module.location
  }
}]

// duplicate identifiers across scopes are allowed (inner hides the outer)
module duplicateIdentifiersWithinLoop 'modulea.bicep' = [for x in emptyArray:{
//@[61:62) Local x. Type: any. Declaration start char: 61, length: 1
//@[7:37) Module duplicateIdentifiersWithinLoop. Type: module[]. Declaration start char: 0, length: 234
  name: 'hello-${x}'
  params: {
    objParam: {}
    stringParamA: 'test'
    stringParamB: 'test'
    arrayParam: [for x in emptyArray: x]
//@[21:22) Local x. Type: any. Declaration start char: 21, length: 1
  }
}]

// duplicate identifiers across scopes are allowed (inner hides the outer)
var duplicateAcrossScopes = 'hello'
//@[4:25) Variable duplicateAcrossScopes. Type: 'hello'. Declaration start char: 0, length: 35
module duplicateInGlobalAndOneLoop 'modulea.bicep' = [for duplicateAcrossScopes in []: {
//@[58:79) Local duplicateAcrossScopes. Type: any. Declaration start char: 58, length: 21
//@[7:34) Module duplicateInGlobalAndOneLoop. Type: module[]. Declaration start char: 0, length: 264
  name: 'hello-${duplicateAcrossScopes}'
  params: {
    objParam: {}
    stringParamA: 'test'
    stringParamB: 'test'
    arrayParam: [for x in emptyArray: x]
//@[21:22) Local x. Type: any. Declaration start char: 21, length: 1
  }
}]

var someDuplicate = true
//@[4:17) Variable someDuplicate. Type: bool. Declaration start char: 0, length: 24
var otherDuplicate = false
//@[4:18) Variable otherDuplicate. Type: bool. Declaration start char: 0, length: 26
module duplicatesEverywhere 'modulea.bicep' = [for someDuplicate in []: {
//@[51:64) Local someDuplicate. Type: any. Declaration start char: 51, length: 13
//@[7:27) Module duplicatesEverywhere. Type: module[]. Declaration start char: 0, length: 263
  name: 'hello-${someDuplicate}'
  params: {
    objParam: {}
    stringParamB: 'test'
    arrayParam: [for otherDuplicate in emptyArray: '${someDuplicate}-${otherDuplicate}']
//@[21:35) Local otherDuplicate. Type: any. Declaration start char: 21, length: 14
  }
}]

