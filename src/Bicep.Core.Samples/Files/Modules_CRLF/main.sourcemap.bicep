
@sys.description('this is deployTimeSuffix param')
param deployTimeSuffix string = newGuid()
//@[11:17]     "deployTimeSuffix": {

@sys.description('this module a')
module modATest './modulea.bicep' = {
//@[85:177]       "type": "Microsoft.Resources/deployments",
  name: 'modATest'
//@[88:88]       "name": "modATest",
  params: {
    stringParamB: 'hello!'
    objParam: {
      a: 'b'
//@[100:100]               "a": "b"
    }
    arrayParam: [
      {
        a: 'b'
//@[106:106]                 "a": "b"
      }
      'abc'
//@[108:108]               "abc"
    ]
  }
}


@sys.description('this module b')
module modB './child/moduleb.bicep' = {
//@[178:226]       "type": "Microsoft.Resources/deployments",
  name: 'modB'
//@[181:181]       "name": "modB",
  params: {
    location: 'West US'
  }
}

@sys.description('this is just module b with a condition')
module modBWithCondition './child/moduleb.bicep' = if (1 + 1 == 2) {
//@[227:276]       "condition": "[equals(add(1, 1), 2)]",
  name: 'modBWithCondition'
//@[231:231]       "name": "modBWithCondition",
  params: {
    location: 'East US'
  }
}

module modC './child/modulec.json' = {
//@[277:316]       "type": "Microsoft.Resources/deployments",
  name: 'modC'
//@[280:280]       "name": "modC",
  params: {
    location: 'West US'
  }
}

module modCWithCondition './child/modulec.json' = if (2 - 1 == 1) {
//@[317:357]       "condition": "[equals(sub(2, 1), 1)]",
  name: 'modCWithCondition'
//@[321:321]       "name": "modCWithCondition",
  params: {
    location: 'East US'
  }
}

module optionalWithNoParams1 './child/optionalParams.bicep'= {
//@[358:415]       "type": "Microsoft.Resources/deployments",
  name: 'optionalWithNoParams1'
//@[361:361]       "name": "optionalWithNoParams1",
}

module optionalWithNoParams2 './child/optionalParams.bicep'= {
//@[416:474]       "type": "Microsoft.Resources/deployments",
  name: 'optionalWithNoParams2'
//@[419:419]       "name": "optionalWithNoParams2",
  params: {
  }
}

module optionalWithAllParams './child/optionalParams.bicep'= {
//@[475:546]       "type": "Microsoft.Resources/deployments",
  name: 'optionalWithNoParams3'
//@[478:478]       "name": "optionalWithNoParams3",
  params: {
    optionalString: 'abc'
    optionalInt: 42
    optionalObj: { }
    optionalArray: [ ]
  }
}

resource resWithDependencies 'Mock.Rp/mockResource@2020-01-01' = {
//@[58:72]       "type": "Mock.Rp/mockResource",
  name: 'harry'
  properties: {
//@[62:66]       "properties": {
    modADep: modATest.outputs.stringOutputA
//@[63:63]         "modADep": "[reference(resourceId('Microsoft.Resources/deployments', 'modATest')).outputs.stringOutputA.value]",
    modBDep: modB.outputs.myResourceId
//@[64:64]         "modBDep": "[reference(resourceId('Microsoft.Resources/deployments', 'modB')).outputs.myResourceId.value]",
    modCDep: modC.outputs.myResourceId
//@[65:65]         "modCDep": "[reference(resourceId('Microsoft.Resources/deployments', 'modC')).outputs.myResourceId.value]"
  }
}

module optionalWithAllParamsAndManualDependency './child/optionalParams.bicep'= {
//@[547:622]       "type": "Microsoft.Resources/deployments",
  name: 'optionalWithAllParamsAndManualDependency'
//@[550:550]       "name": "optionalWithAllParamsAndManualDependency",
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
//@[623:698]       "type": "Microsoft.Resources/deployments",
  name: 'optionalWithImplicitDependency'
//@[626:626]       "name": "optionalWithImplicitDependency",
  params: {
    optionalString: concat(resWithDependencies.id, optionalWithAllParamsAndManualDependency.name)
    optionalInt: 42
    optionalObj: { }
    optionalArray: [ ]
  }
}

module moduleWithCalculatedName './child/optionalParams.bicep'= {
//@[699:774]       "type": "Microsoft.Resources/deployments",
  name: '${optionalWithAllParamsAndManualDependency.name}${deployTimeSuffix}'
//@[702:702]       "name": "[format('{0}{1}', 'optionalWithAllParamsAndManualDependency', parameters('deployTimeSuffix'))]",
  params: {
    optionalString: concat(resWithDependencies.id, optionalWithAllParamsAndManualDependency.name)
    optionalInt: 42
    optionalObj: { }
    optionalArray: [ ]
  }
}

resource resWithCalculatedNameDependencies 'Mock.Rp/mockResource@2020-01-01' = {
//@[73:84]       "type": "Mock.Rp/mockResource",
  name: '${optionalWithAllParamsAndManualDependency.name}${deployTimeSuffix}'
  properties: {
//@[77:79]       "properties": {
    modADep: moduleWithCalculatedName.outputs.outputObj
//@[78:78]         "modADep": "[reference(resourceId('Microsoft.Resources/deployments', format('{0}{1}', 'optionalWithAllParamsAndManualDependency', parameters('deployTimeSuffix')))).outputs.outputObj.value]"
  }
}

output stringOutputA string = modATest.outputs.stringOutputA
//@[1950:1953]     "stringOutputA": {
output stringOutputB string = modATest.outputs.stringOutputB
//@[1954:1957]     "stringOutputB": {
output objOutput object = modATest.outputs.objOutput
//@[1958:1961]     "objOutput": {
output arrayOutput array = modATest.outputs.arrayOutput
//@[1962:1965]     "arrayOutput": {
output modCalculatedNameOutput object = moduleWithCalculatedName.outputs.outputObj
//@[1966:1969]     "modCalculatedNameOutput": {

/*
  valid loop cases
*/ 

@sys.description('this is myModules')
var myModules = [
//@[20:29]     "myModules": [
  {
    name: 'one'
//@[22:22]         "name": "one",
    location: 'eastus2'
//@[23:23]         "location": "eastus2"
  }
  {
    name: 'two'
//@[26:26]         "name": "two",
    location: 'westus'
//@[27:27]         "location": "westus"
  }
]

var emptyArray = []
//@[30:30]     "emptyArray": [],

// simple module loop
module storageResources 'modulea.bicep' = [for module in myModules: {
//@[775:861]       "copy": {
  name: module.name
//@[782:782]       "name": "[variables('myModules')[copyIndex()].name]",
  params: {
    arrayParam: []
    objParam: module
    stringParamB: module.location
  }
}]

// simple indexed module loop
module storageResourcesWithIndex 'modulea.bicep' = [for (module, i) in myModules: {
//@[862:953]       "copy": {
  name: module.name
//@[869:869]       "name": "[variables('myModules')[copyIndex()].name]",
  params: {
    arrayParam: [
      i + 1
//@[878:878]               "[add(copyIndex(), 1)]"
    ]
    objParam: module
    stringParamB: module.location
    stringParamA: concat('a', i)
  }
}]

// nested module loop
module nestedModuleLoop 'modulea.bicep' = [for module in myModules: {
//@[954:1046]       "copy": {
  name: module.name
//@[961:961]       "name": "[variables('myModules')[copyIndex()].name]",
  params: {
    arrayParam: [for i in range(0,3): concat('test-', i, '-', module.name)]
    objParam: module
    stringParamB: module.location
  }
}]

// duplicate identifiers across scopes are allowed (inner hides the outer)
module duplicateIdentifiersWithinLoop 'modulea.bicep' = [for x in emptyArray:{
//@[1047:1142]       "copy": {
  name: 'hello-${x}'
//@[1054:1054]       "name": "[format('hello-{0}', variables('emptyArray')[copyIndex()])]",
  params: {
    objParam: {}
    stringParamA: 'test'
    stringParamB: 'test'
    arrayParam: [for x in emptyArray: x]
  }
}]

// duplicate identifiers across scopes are allowed (inner hides the outer)
var duplicateAcrossScopes = 'hello'
//@[31:31]     "duplicateAcrossScopes": "hello",
module duplicateInGlobalAndOneLoop 'modulea.bicep' = [for duplicateAcrossScopes in []: {
//@[1143:1238]       "copy": {
  name: 'hello-${duplicateAcrossScopes}'
//@[1150:1150]       "name": "[format('hello-{0}', createArray()[copyIndex()])]",
  params: {
    objParam: {}
    stringParamA: 'test'
    stringParamB: 'test'
    arrayParam: [for x in emptyArray: x]
  }
}]

var someDuplicate = true
//@[32:32]     "someDuplicate": true,
var otherDuplicate = false
//@[33:33]     "otherDuplicate": false,
module duplicatesEverywhere 'modulea.bicep' = [for someDuplicate in []: {
//@[1239:1331]       "copy": {
  name: 'hello-${someDuplicate}'
//@[1246:1246]       "name": "[format('hello-{0}', createArray()[copyIndex()])]",
  params: {
    objParam: {}
    stringParamB: 'test'
    arrayParam: [for otherDuplicate in emptyArray: '${someDuplicate}-${otherDuplicate}']
  }
}]

module propertyLoopInsideParameterValue 'modulea.bicep' = {
//@[1332:1453]       "type": "Microsoft.Resources/deployments",
  name: 'propertyLoopInsideParameterValue'
//@[1335:1335]       "name": "propertyLoopInsideParameterValue",
  params: {
    objParam: {
      a: [for i in range(0,10): i]
//@[1345:1349]                   "name": "a",
      b: [for i in range(1,2): i]
//@[1350:1354]                   "name": "b",
      c: {
//@[1363:1371]               "c": {
        d: [for j in range(2,3): j]
//@[1365:1369]                     "name": "d",
      }
      e: [for k in range(4,4): {
//@[1355:1361]                   "name": "e",
        f: k
//@[1359:1359]                     "f": "[range(4, 4)[copyIndex('e')]]"
      }]
    }
    stringParamB: ''
    arrayParam: [
      {
        e: [for j in range(7,7): j]
//@[1381:1385]                     "name": "e",
      }
    ]
  }
}

module propertyLoopInsideParameterValueWithIndexes 'modulea.bicep' = {
//@[1454:1576]       "type": "Microsoft.Resources/deployments",
  name: 'propertyLoopInsideParameterValueWithIndexes'
//@[1457:1457]       "name": "propertyLoopInsideParameterValueWithIndexes",
  params: {
    objParam: {
      a: [for (i, i2) in range(0,10): i + i2]
//@[1467:1471]                   "name": "a",
      b: [for (i, i2) in range(1,2): i / i2]
//@[1472:1476]                   "name": "b",
      c: {
//@[1486:1494]               "c": {
        d: [for (j, j2) in range(2,3): j * j2]
//@[1488:1492]                     "name": "d",
      }
      e: [for (k, k2) in range(4,4): {
//@[1477:1484]                   "name": "e",
        f: k
//@[1481:1481]                     "f": "[range(4, 4)[copyIndex('e')]]",
        g: k2
//@[1482:1482]                     "g": "[copyIndex('e')]"
      }]
    }
    stringParamB: ''
    arrayParam: [
      {
        e: [for j in range(7,7): j]
//@[1504:1508]                     "name": "e",
      }
    ]
  }
}

module propertyLoopInsideParameterValueInsideModuleLoop 'modulea.bicep' = [for thing in range(0,1): {
//@[1577:1702]       "copy": {
  name: 'propertyLoopInsideParameterValueInsideModuleLoop'
//@[1584:1584]       "name": "propertyLoopInsideParameterValueInsideModuleLoop",
  params: {
    objParam: {
      a: [for i in range(0,10): i + thing]
//@[1594:1598]                   "name": "a",
      b: [for i in range(1,2): i * thing]
//@[1599:1603]                   "name": "b",
      c: {
//@[1612:1620]               "c": {
        d: [for j in range(2,3): j]
//@[1614:1618]                     "name": "d",
      }
      e: [for k in range(4,4): {
//@[1604:1610]                   "name": "e",
        f: k - thing
//@[1608:1608]                     "f": "[sub(range(4, 4)[copyIndex('e')], range(0, 1)[copyIndex()])]"
      }]
    }
    stringParamB: ''
    arrayParam: [
      {
        e: [for j in range(7,7): j % thing]
//@[1630:1634]                     "name": "e",
      }
    ]
  }
}]


// BEGIN: Key Vault Secret Reference

resource kv 'Microsoft.KeyVault/vaults@2019-09-01' existing = {
  name: 'testkeyvault'
}

module secureModule1 'child/secureParams.bicep' = {
//@[1703:1759]       "type": "Microsoft.Resources/deployments",
  name: 'secureModule1'
//@[1706:1706]       "name": "secureModule1",
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
//@[1760:1816]       "type": "Microsoft.Resources/deployments",
  name: 'secureModule2'
//@[1763:1763]       "name": "secureModule2",
  params: {
    secureStringParam1: scopedKv.getSecret('mySecret')
    secureStringParam2: scopedKv.getSecret('mySecret','secretVersion')
  }
}

//looped module with looped existing resource (Issue #2862)
var vaults = [
//@[34:45]     "vaults": [
  {
    vaultName: 'test-1-kv'
//@[36:36]         "vaultName": "test-1-kv",
    vaultRG: 'test-1-rg'
//@[37:37]         "vaultRG": "test-1-rg",
    vaultSub: 'abcd-efgh'
//@[38:38]         "vaultSub": "abcd-efgh"
  }
  {
    vaultName: 'test-2-kv'
//@[41:41]         "vaultName": "test-2-kv",
    vaultRG: 'test-2-rg'
//@[42:42]         "vaultRG": "test-2-rg",
    vaultSub: 'ijkl-1adg1'
//@[43:43]         "vaultSub": "ijkl-1adg1"
  }
]
var secrets = [
//@[46:55]     "secrets": [
  {
    name: 'secret01'
//@[48:48]         "name": "secret01",
    version: 'versionA'
//@[49:49]         "version": "versionA"
  }
  {
    name: 'secret02'
//@[52:52]         "name": "secret02",
    version: 'versionB'
//@[53:53]         "version": "versionB"
  }
]

resource loopedKv 'Microsoft.KeyVault/vaults@2019-09-01' existing = [for vault in vaults: {
  name: vault.vaultName
  scope: resourceGroup(vault.vaultSub, vault.vaultRG)
}]

module secureModuleLooped 'child/secureParams.bicep' = [for (secret, i) in secrets: {
//@[1817:1877]       "copy": {
  name: 'secureModuleLooped-${i}'
//@[1824:1824]       "name": "[format('secureModuleLooped-{0}', copyIndex())]",
  params: {
    secureStringParam1: loopedKv[i].getSecret(secret.name)
    secureStringParam2: loopedKv[i].getSecret(secret.name, secret.version)
  }
}]


// END: Key Vault Secret Reference

module withSpace 'module with space.bicep' = {
//@[1878:1912]       "type": "Microsoft.Resources/deployments",
  name: 'withSpace'
//@[1881:1881]       "name": "withSpace",
}

module folderWithSpace 'child/folder with space/child with space.bicep' = {
//@[1913:1947]       "type": "Microsoft.Resources/deployments",
  name: 'childWithSpace'
//@[1916:1916]       "name": "childWithSpace",
}

