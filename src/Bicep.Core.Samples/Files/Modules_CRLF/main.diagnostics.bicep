
@sys.description('this is deployTimeSuffix param')
param deployTimeSuffix string = newGuid()

@sys.description('this module a')
module modATest './modulea.bicep' = {
  name: 'modATest'
  params: {
//@[02:155) [explicit-values-for-loc-params (Warning)] Parameter 'stringParamA' of module 'modATest' isn't assigned an explicit value, and its default value may not give the intended behavior for a location-related parameter. You should assign an explicit value to the parameter. (CodeDescription: bicep core(https://aka.ms/bicep/linter/explicit-values-for-loc-params)) |params: {\r\n    stringParamB: 'hello!'\r\n    objParam: {\r\n      a: 'b'\r\n    }\r\n    arrayParam: [\r\n      {\r\n        a: 'b'\r\n      }\r\n      'abc'\r\n    ]\r\n  }|
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


@sys.description('this module b')
module modB './child/moduleb.bicep' = {
  name: 'modB'
  params: {
    location: 'West US'
  }
}

@sys.description('this is just module b with a condition')
module modBWithCondition './child/moduleb.bicep' = if (1 + 1 == 2) {
  name: 'modBWithCondition'
  params: {
    location: 'East US'
  }
}

module modC './child/modulec.json' = {
  name: 'modC'
  params: {
    location: 'West US'
  }
}

module modCWithCondition './child/modulec.json' = if (2 - 1 == 1) {
  name: 'modCWithCondition'
  params: {
    location: 'East US'
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
  name: 'optionalWithNoParams3'
  params: {
    optionalString: 'abc'
    optionalInt: 42
    optionalObj: { }
    optionalArray: [ ]
  }
}

resource resWithDependencies 'Mock.Rp/mockResource@2020-01-01' = {
//@[29:062) [BCP081 (Warning)] Resource type "Mock.Rp/mockResource@2020-01-01" does not have types available. (CodeDescription: none) |'Mock.Rp/mockResource@2020-01-01'|
  name: 'harry'
  properties: {
    modADep: modATest.outputs.stringOutputA
    modBDep: modB.outputs.myResourceId
    modCDep: modC.outputs.myResourceId
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
//@[20:097) [prefer-interpolation (Warning)] Use string interpolation instead of the concat function. (CodeDescription: bicep core(https://aka.ms/bicep/linter/prefer-interpolation)) |concat(resWithDependencies.id, optionalWithAllParamsAndManualDependency.name)|
    optionalInt: 42
    optionalObj: { }
    optionalArray: [ ]
  }
}

module moduleWithCalculatedName './child/optionalParams.bicep'= {
  name: '${optionalWithAllParamsAndManualDependency.name}${deployTimeSuffix}'
  params: {
    optionalString: concat(resWithDependencies.id, optionalWithAllParamsAndManualDependency.name)
//@[20:097) [prefer-interpolation (Warning)] Use string interpolation instead of the concat function. (CodeDescription: bicep core(https://aka.ms/bicep/linter/prefer-interpolation)) |concat(resWithDependencies.id, optionalWithAllParamsAndManualDependency.name)|
    optionalInt: 42
    optionalObj: { }
    optionalArray: [ ]
  }
}

resource resWithCalculatedNameDependencies 'Mock.Rp/mockResource@2020-01-01' = {
//@[43:076) [BCP081 (Warning)] Resource type "Mock.Rp/mockResource@2020-01-01" does not have types available. (CodeDescription: none) |'Mock.Rp/mockResource@2020-01-01'|
  name: '${optionalWithAllParamsAndManualDependency.name}${deployTimeSuffix}'
//@[08:077) [use-stable-resource-identifiers (Warning)] Resource identifiers should be reproducible outside of their initial deployment context. Resource resWithCalculatedNameDependencies's 'name' identifier is potentially nondeterministic due to its use of the 'newGuid' function (resWithCalculatedNameDependencies.name -> deployTimeSuffix (default value) -> newGuid()). (CodeDescription: bicep core(https://aka.ms/bicep/linter/use-stable-resource-identifiers)) |'${optionalWithAllParamsAndManualDependency.name}${deployTimeSuffix}'|
  properties: {
    modADep: moduleWithCalculatedName.outputs.outputObj
  }
}

output stringOutputA string = modATest.outputs.stringOutputA
output stringOutputB string = modATest.outputs.stringOutputB
output objOutput object = modATest.outputs.objOutput
output arrayOutput array = modATest.outputs.arrayOutput
output modCalculatedNameOutput object = moduleWithCalculatedName.outputs.outputObj

/*
  valid loop cases
*/ 

@sys.description('this is myModules')
var myModules = [
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

// simple module loop
module storageResources 'modulea.bicep' = [for module in myModules: {
  name: module.name
  params: {
//@[02:093) [explicit-values-for-loc-params (Warning)] Parameter 'stringParamA' of module 'storageResources' isn't assigned an explicit value, and its default value may not give the intended behavior for a location-related parameter. You should assign an explicit value to the parameter. (CodeDescription: bicep core(https://aka.ms/bicep/linter/explicit-values-for-loc-params)) |params: {\r\n    arrayParam: []\r\n    objParam: module\r\n    stringParamB: module.location\r\n  }|
    arrayParam: []
    objParam: module
    stringParamB: module.location
  }
}]

// simple indexed module loop
module storageResourcesWithIndex 'modulea.bicep' = [for (module, i) in myModules: {
  name: module.name
  params: {
    arrayParam: [
      i + 1
    ]
    objParam: module
    stringParamB: module.location
    stringParamA: concat('a', i)
//@[18:032) [prefer-interpolation (Warning)] Use string interpolation instead of the concat function. (CodeDescription: bicep core(https://aka.ms/bicep/linter/prefer-interpolation)) |concat('a', i)|
  }
}]

// nested module loop
module nestedModuleLoop 'modulea.bicep' = [for module in myModules: {
  name: module.name
  params: {
//@[02:150) [explicit-values-for-loc-params (Warning)] Parameter 'stringParamA' of module 'nestedModuleLoop' isn't assigned an explicit value, and its default value may not give the intended behavior for a location-related parameter. You should assign an explicit value to the parameter. (CodeDescription: bicep core(https://aka.ms/bicep/linter/explicit-values-for-loc-params)) |params: {\r\n    arrayParam: [for i in range(0,3): concat('test-', i, '-', module.name)]\r\n    objParam: module\r\n    stringParamB: module.location\r\n  }|
    arrayParam: [for i in range(0,3): concat('test-', i, '-', module.name)]
//@[38:074) [prefer-interpolation (Warning)] Use string interpolation instead of the concat function. (CodeDescription: bicep core(https://aka.ms/bicep/linter/prefer-interpolation)) |concat('test-', i, '-', module.name)|
    objParam: module
    stringParamB: module.location
  }
}]

// duplicate identifiers across scopes are allowed (inner hides the outer)
module duplicateIdentifiersWithinLoop 'modulea.bicep' = [for x in emptyArray:{
  name: 'hello-${x}'
  params: {
    objParam: {}
    stringParamA: 'test'
//@[18:024) [no-hardcoded-location (Warning)] Parameter 'stringParamA' may be used as a resource location in the module and should not be assigned a hard-coded string or variable value. Please use a parameter value, an expression, or the string 'global'. Found: 'test' (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-hardcoded-location)) |'test'|
    stringParamB: 'test'
    arrayParam: [for x in emptyArray: x]
  }
}]

// duplicate identifiers across scopes are allowed (inner hides the outer)
var duplicateAcrossScopes = 'hello'
//@[04:025) [no-unused-vars (Warning)] Variable "duplicateAcrossScopes" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |duplicateAcrossScopes|
module duplicateInGlobalAndOneLoop 'modulea.bicep' = [for duplicateAcrossScopes in []: {
  name: 'hello-${duplicateAcrossScopes}'
  params: {
    objParam: {}
    stringParamA: 'test'
//@[18:024) [no-hardcoded-location (Warning)] Parameter 'stringParamA' may be used as a resource location in the module and should not be assigned a hard-coded string or variable value. Please use a parameter value, an expression, or the string 'global'. Found: 'test' (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-hardcoded-location)) |'test'|
    stringParamB: 'test'
    arrayParam: [for x in emptyArray: x]
  }
}]

var someDuplicate = true
//@[04:017) [no-unused-vars (Warning)] Variable "someDuplicate" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |someDuplicate|
var otherDuplicate = false
//@[04:018) [no-unused-vars (Warning)] Variable "otherDuplicate" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |otherDuplicate|
module duplicatesEverywhere 'modulea.bicep' = [for someDuplicate in []: {
  name: 'hello-${someDuplicate}'
  params: {
//@[02:150) [explicit-values-for-loc-params (Warning)] Parameter 'stringParamA' of module 'duplicatesEverywhere' isn't assigned an explicit value, and its default value may not give the intended behavior for a location-related parameter. You should assign an explicit value to the parameter. (CodeDescription: bicep core(https://aka.ms/bicep/linter/explicit-values-for-loc-params)) |params: {\r\n    objParam: {}\r\n    stringParamB: 'test'\r\n    arrayParam: [for otherDuplicate in emptyArray: '${someDuplicate}-${otherDuplicate}']\r\n  }|
    objParam: {}
    stringParamB: 'test'
    arrayParam: [for otherDuplicate in emptyArray: '${someDuplicate}-${otherDuplicate}']
  }
}]

module propertyLoopInsideParameterValue 'modulea.bicep' = {
  name: 'propertyLoopInsideParameterValue'
  params: {
//@[02:330) [explicit-values-for-loc-params (Warning)] Parameter 'stringParamA' of module 'propertyLoopInsideParameterValue' isn't assigned an explicit value, and its default value may not give the intended behavior for a location-related parameter. You should assign an explicit value to the parameter. (CodeDescription: bicep core(https://aka.ms/bicep/linter/explicit-values-for-loc-params)) |params: {\r\n    objParam: {\r\n      a: [for i in range(0,10): i]\r\n      b: [for i in range(1,2): i]\r\n      c: {\r\n        d: [for j in range(2,3): j]\r\n      }\r\n      e: [for k in range(4,4): {\r\n        f: k\r\n      }]\r\n    }\r\n    stringParamB: ''\r\n    arrayParam: [\r\n      {\r\n        e: [for j in range(7,7): j]\r\n      }\r\n    ]\r\n  }|
    objParam: {
      a: [for i in range(0,10): i]
      b: [for i in range(1,2): i]
      c: {
        d: [for j in range(2,3): j]
      }
      e: [for k in range(4,4): {
        f: k
      }]
    }
    stringParamB: ''
    arrayParam: [
      {
        e: [for j in range(7,7): j]
      }
    ]
  }
}

module propertyLoopInsideParameterValueWithIndexes 'modulea.bicep' = {
  name: 'propertyLoopInsideParameterValueWithIndexes'
  params: {
//@[02:384) [explicit-values-for-loc-params (Warning)] Parameter 'stringParamA' of module 'propertyLoopInsideParameterValueWithIndexes' isn't assigned an explicit value, and its default value may not give the intended behavior for a location-related parameter. You should assign an explicit value to the parameter. (CodeDescription: bicep core(https://aka.ms/bicep/linter/explicit-values-for-loc-params)) |params: {\r\n    objParam: {\r\n      a: [for (i, i2) in range(0,10): i + i2]\r\n      b: [for (i, i2) in range(1,2): i / i2]\r\n      c: {\r\n        d: [for (j, j2) in range(2,3): j * j2]\r\n      }\r\n      e: [for (k, k2) in range(4,4): {\r\n        f: k\r\n        g: k2\r\n      }]\r\n    }\r\n    stringParamB: ''\r\n    arrayParam: [\r\n      {\r\n        e: [for j in range(7,7): j]\r\n      }\r\n    ]\r\n  }|
    objParam: {
      a: [for (i, i2) in range(0,10): i + i2]
      b: [for (i, i2) in range(1,2): i / i2]
      c: {
        d: [for (j, j2) in range(2,3): j * j2]
      }
      e: [for (k, k2) in range(4,4): {
        f: k
        g: k2
      }]
    }
    stringParamB: ''
    arrayParam: [
      {
        e: [for j in range(7,7): j]
      }
    ]
  }
}

module propertyLoopInsideParameterValueInsideModuleLoop 'modulea.bicep' = [for thing in range(0,1): {
//@[07:055) [BCP179 (Warning)] Unique resource or deployment name is required when looping. The loop item variable "thing" must be referenced in at least one of the value expressions of the following properties: "name", "scope" (CodeDescription: none) |propertyLoopInsideParameterValueInsideModuleLoop|
  name: 'propertyLoopInsideParameterValueInsideModuleLoop'
  params: {
//@[02:362) [explicit-values-for-loc-params (Warning)] Parameter 'stringParamA' of module 'propertyLoopInsideParameterValueInsideModuleLoop' isn't assigned an explicit value, and its default value may not give the intended behavior for a location-related parameter. You should assign an explicit value to the parameter. (CodeDescription: bicep core(https://aka.ms/bicep/linter/explicit-values-for-loc-params)) |params: {\r\n    objParam: {\r\n      a: [for i in range(0,10): i + thing]\r\n      b: [for i in range(1,2): i * thing]\r\n      c: {\r\n        d: [for j in range(2,3): j]\r\n      }\r\n      e: [for k in range(4,4): {\r\n        f: k - thing\r\n      }]\r\n    }\r\n    stringParamB: ''\r\n    arrayParam: [\r\n      {\r\n        e: [for j in range(7,7): j % thing]\r\n      }\r\n    ]\r\n  }|
    objParam: {
      a: [for i in range(0,10): i + thing]
      b: [for i in range(1,2): i * thing]
      c: {
        d: [for j in range(2,3): j]
      }
      e: [for k in range(4,4): {
        f: k - thing
      }]
    }
    stringParamB: ''
    arrayParam: [
      {
        e: [for j in range(7,7): j % thing]
      }
    ]
  }
}]


// BEGIN: Key Vault Secret Reference

resource kv 'Microsoft.KeyVault/vaults@2019-09-01' existing = {
  name: 'testkeyvault'
}

module secureModule1 'child/secureParams.bicep' = {
  name: 'secureModule1'
  params: {
    secureStringParam1: kv.getSecret('mySecret')
    secureStringParam2: kv.getSecret('mySecret','secretVersion')
  }
}

resource scopedKv 'Microsoft.KeyVault/vaults@2019-09-01' existing = {
  name: 'testkeyvault'
  scope: resourceGroup('otherGroup')
}

module secureModule2 'child/secureParams.bicep' = {
  name: 'secureModule2'
  params: {
    secureStringParam1: scopedKv.getSecret('mySecret')
    secureStringParam2: scopedKv.getSecret('mySecret','secretVersion')
  }
}

//looped module with looped existing resource (Issue #2862)
var vaults = [
  {
    vaultName: 'test-1-kv'
    vaultRG: 'test-1-rg'
    vaultSub: 'abcd-efgh'
  }
  {
    vaultName: 'test-2-kv'
    vaultRG: 'test-2-rg'
    vaultSub: 'ijkl-1adg1'
  }
]
var secrets = [
  {
    name: 'secret01'
    version: 'versionA'
  }
  {
    name: 'secret02'
    version: 'versionB'
  }
]

resource loopedKv 'Microsoft.KeyVault/vaults@2019-09-01' existing = [for vault in vaults: {
  name: vault.vaultName
  scope: resourceGroup(vault.vaultSub, vault.vaultRG)
}]

module secureModuleLooped 'child/secureParams.bicep' = [for (secret, i) in secrets: {
  name: 'secureModuleLooped-${i}'
  params: {
    secureStringParam1: loopedKv[i].getSecret(secret.name)
    secureStringParam2: loopedKv[i].getSecret(secret.name, secret.version)
  }
}]


// END: Key Vault Secret Reference

module withSpace 'module with space.bicep' = {
  name: 'withSpace'
}

module folderWithSpace 'child/folder with space/child with space.bicep' = {
  name: 'childWithSpace'
}

