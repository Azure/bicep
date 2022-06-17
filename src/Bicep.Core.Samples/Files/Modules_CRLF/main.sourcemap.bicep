
@sys.description('this is deployTimeSuffix param')
param deployTimeSuffix string = newGuid()
//@[12:18]     "deployTimeSuffix": {

@sys.description('this module a')
module modATest './modulea.bicep' = {
//@[86:178]       "type": "Microsoft.Resources/deployments",
  name: 'modATest'
//@[89:89]       "name": "modATest",
  params: {
    stringParamB: 'hello!'
    objParam: {
      a: 'b'
//@[101:101]               "a": "b"
    }
    arrayParam: [
      {
        a: 'b'
//@[107:107]                 "a": "b"
      }
      'abc'
//@[109:109]               "abc"
    ]
  }
}


@sys.description('this module b')
module modB './child/moduleb.bicep' = {
//@[179:227]       "type": "Microsoft.Resources/deployments",
  name: 'modB'
//@[182:182]       "name": "modB",
  params: {
    location: 'West US'
  }
}

@sys.description('this is just module b with a condition')
module modBWithCondition './child/moduleb.bicep' = if (1 + 1 == 2) {
//@[228:277]       "condition": "[equals(add(1, 1), 2)]",
  name: 'modBWithCondition'
//@[232:232]       "name": "modBWithCondition",
  params: {
    location: 'East US'
  }
}

module modC './child/modulec.json' = {
//@[278:317]       "type": "Microsoft.Resources/deployments",
  name: 'modC'
//@[281:281]       "name": "modC",
  params: {
    location: 'West US'
  }
}

module modCWithCondition './child/modulec.json' = if (2 - 1 == 1) {
//@[318:358]       "condition": "[equals(sub(2, 1), 1)]",
  name: 'modCWithCondition'
//@[322:322]       "name": "modCWithCondition",
  params: {
    location: 'East US'
  }
}

module optionalWithNoParams1 './child/optionalParams.bicep'= {
//@[359:416]       "type": "Microsoft.Resources/deployments",
  name: 'optionalWithNoParams1'
//@[362:362]       "name": "optionalWithNoParams1",
}

module optionalWithNoParams2 './child/optionalParams.bicep'= {
//@[417:475]       "type": "Microsoft.Resources/deployments",
  name: 'optionalWithNoParams2'
//@[420:420]       "name": "optionalWithNoParams2",
  params: {
  }
}

module optionalWithAllParams './child/optionalParams.bicep'= {
//@[476:547]       "type": "Microsoft.Resources/deployments",
  name: 'optionalWithNoParams3'
//@[479:479]       "name": "optionalWithNoParams3",
  params: {
    optionalString: 'abc'
    optionalInt: 42
    optionalObj: { }
    optionalArray: [ ]
  }
}

resource resWithDependencies 'Mock.Rp/mockResource@2020-01-01' = {
//@[59:73]       "type": "Mock.Rp/mockResource",
  name: 'harry'
  properties: {
//@[63:67]       "properties": {
    modADep: modATest.outputs.stringOutputA
//@[64:64]         "modADep": "[reference(resourceId('Microsoft.Resources/deployments', 'modATest')).outputs.stringOutputA.value]",
    modBDep: modB.outputs.myResourceId
//@[65:65]         "modBDep": "[reference(resourceId('Microsoft.Resources/deployments', 'modB')).outputs.myResourceId.value]",
    modCDep: modC.outputs.myResourceId
//@[66:66]         "modCDep": "[reference(resourceId('Microsoft.Resources/deployments', 'modC')).outputs.myResourceId.value]"
  }
}

module optionalWithAllParamsAndManualDependency './child/optionalParams.bicep'= {
//@[548:623]       "type": "Microsoft.Resources/deployments",
  name: 'optionalWithAllParamsAndManualDependency'
//@[551:551]       "name": "optionalWithAllParamsAndManualDependency",
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
//@[624:699]       "type": "Microsoft.Resources/deployments",
  name: 'optionalWithImplicitDependency'
//@[627:627]       "name": "optionalWithImplicitDependency",
  params: {
    optionalString: concat(resWithDependencies.id, optionalWithAllParamsAndManualDependency.name)
    optionalInt: 42
    optionalObj: { }
    optionalArray: [ ]
  }
}

module moduleWithCalculatedName './child/optionalParams.bicep'= {
//@[700:775]       "type": "Microsoft.Resources/deployments",
  name: '${optionalWithAllParamsAndManualDependency.name}${deployTimeSuffix}'
//@[703:703]       "name": "[format('{0}{1}', 'optionalWithAllParamsAndManualDependency', parameters('deployTimeSuffix'))]",
  params: {
    optionalString: concat(resWithDependencies.id, optionalWithAllParamsAndManualDependency.name)
    optionalInt: 42
    optionalObj: { }
    optionalArray: [ ]
  }
}

resource resWithCalculatedNameDependencies 'Mock.Rp/mockResource@2020-01-01' = {
//@[74:85]       "type": "Mock.Rp/mockResource",
  name: '${optionalWithAllParamsAndManualDependency.name}${deployTimeSuffix}'
  properties: {
//@[78:80]       "properties": {
    modADep: moduleWithCalculatedName.outputs.outputObj
//@[79:79]         "modADep": "[reference(resourceId('Microsoft.Resources/deployments', format('{0}{1}', 'optionalWithAllParamsAndManualDependency', parameters('deployTimeSuffix')))).outputs.outputObj.value]"
  }
}

output stringOutputA string = modATest.outputs.stringOutputA
//@[1951:1954]     "stringOutputA": {
output stringOutputB string = modATest.outputs.stringOutputB
//@[1955:1958]     "stringOutputB": {
output objOutput object = modATest.outputs.objOutput
//@[1959:1962]     "objOutput": {
output arrayOutput array = modATest.outputs.arrayOutput
//@[1963:1966]     "arrayOutput": {
output modCalculatedNameOutput object = moduleWithCalculatedName.outputs.outputObj
//@[1967:1970]     "modCalculatedNameOutput": {

/*
  valid loop cases
*/ 

@sys.description('this is myModules')
var myModules = [
//@[21:30]     "myModules": [
  {
    name: 'one'
//@[23:23]         "name": "one",
    location: 'eastus2'
//@[24:24]         "location": "eastus2"
  }
  {
    name: 'two'
//@[27:27]         "name": "two",
    location: 'westus'
//@[28:28]         "location": "westus"
  }
]

var emptyArray = []
//@[31:31]     "emptyArray": [],

// simple module loop
module storageResources 'modulea.bicep' = [for module in myModules: {
//@[776:862]       "copy": {
  name: module.name
//@[783:783]       "name": "[variables('myModules')[copyIndex()].name]",
  params: {
    arrayParam: []
    objParam: module
    stringParamB: module.location
  }
}]

// simple indexed module loop
module storageResourcesWithIndex 'modulea.bicep' = [for (module, i) in myModules: {
//@[863:954]       "copy": {
  name: module.name
//@[870:870]       "name": "[variables('myModules')[copyIndex()].name]",
  params: {
    arrayParam: [
      i + 1
//@[879:879]               "[add(copyIndex(), 1)]"
    ]
    objParam: module
    stringParamB: module.location
    stringParamA: concat('a', i)
  }
}]

// nested module loop
module nestedModuleLoop 'modulea.bicep' = [for module in myModules: {
//@[955:1047]       "copy": {
  name: module.name
//@[962:962]       "name": "[variables('myModules')[copyIndex()].name]",
  params: {
    arrayParam: [for i in range(0,3): concat('test-', i, '-', module.name)]
    objParam: module
    stringParamB: module.location
  }
}]

// duplicate identifiers across scopes are allowed (inner hides the outer)
module duplicateIdentifiersWithinLoop 'modulea.bicep' = [for x in emptyArray:{
//@[1048:1143]       "copy": {
  name: 'hello-${x}'
//@[1055:1055]       "name": "[format('hello-{0}', variables('emptyArray')[copyIndex()])]",
  params: {
    objParam: {}
    stringParamA: 'test'
    stringParamB: 'test'
    arrayParam: [for x in emptyArray: x]
  }
}]

// duplicate identifiers across scopes are allowed (inner hides the outer)
var duplicateAcrossScopes = 'hello'
//@[32:32]     "duplicateAcrossScopes": "hello",
module duplicateInGlobalAndOneLoop 'modulea.bicep' = [for duplicateAcrossScopes in []: {
//@[1144:1239]       "copy": {
  name: 'hello-${duplicateAcrossScopes}'
//@[1151:1151]       "name": "[format('hello-{0}', createArray()[copyIndex()])]",
  params: {
    objParam: {}
    stringParamA: 'test'
    stringParamB: 'test'
    arrayParam: [for x in emptyArray: x]
  }
}]

var someDuplicate = true
//@[33:33]     "someDuplicate": true,
var otherDuplicate = false
//@[34:34]     "otherDuplicate": false,
module duplicatesEverywhere 'modulea.bicep' = [for someDuplicate in []: {
//@[1240:1332]       "copy": {
  name: 'hello-${someDuplicate}'
//@[1247:1247]       "name": "[format('hello-{0}', createArray()[copyIndex()])]",
  params: {
    objParam: {}
    stringParamB: 'test'
    arrayParam: [for otherDuplicate in emptyArray: '${someDuplicate}-${otherDuplicate}']
  }
}]

module propertyLoopInsideParameterValue 'modulea.bicep' = {
//@[1333:1454]       "type": "Microsoft.Resources/deployments",
  name: 'propertyLoopInsideParameterValue'
//@[1336:1336]       "name": "propertyLoopInsideParameterValue",
  params: {
    objParam: {
      a: [for i in range(0,10): i]
//@[1346:1350]                   "name": "a",
      b: [for i in range(1,2): i]
//@[1351:1355]                   "name": "b",
      c: {
//@[1364:1372]               "c": {
        d: [for j in range(2,3): j]
//@[1366:1370]                     "name": "d",
      }
      e: [for k in range(4,4): {
//@[1356:1362]                   "name": "e",
        f: k
//@[1360:1360]                     "f": "[range(4, 4)[copyIndex('e')]]"
      }]
    }
    stringParamB: ''
    arrayParam: [
      {
        e: [for j in range(7,7): j]
//@[1382:1386]                     "name": "e",
      }
    ]
  }
}

module propertyLoopInsideParameterValueWithIndexes 'modulea.bicep' = {
//@[1455:1577]       "type": "Microsoft.Resources/deployments",
  name: 'propertyLoopInsideParameterValueWithIndexes'
//@[1458:1458]       "name": "propertyLoopInsideParameterValueWithIndexes",
  params: {
    objParam: {
      a: [for (i, i2) in range(0,10): i + i2]
//@[1468:1472]                   "name": "a",
      b: [for (i, i2) in range(1,2): i / i2]
//@[1473:1477]                   "name": "b",
      c: {
//@[1487:1495]               "c": {
        d: [for (j, j2) in range(2,3): j * j2]
//@[1489:1493]                     "name": "d",
      }
      e: [for (k, k2) in range(4,4): {
//@[1478:1485]                   "name": "e",
        f: k
//@[1482:1482]                     "f": "[range(4, 4)[copyIndex('e')]]",
        g: k2
//@[1483:1483]                     "g": "[copyIndex('e')]"
      }]
    }
    stringParamB: ''
    arrayParam: [
      {
        e: [for j in range(7,7): j]
//@[1505:1509]                     "name": "e",
      }
    ]
  }
}

module propertyLoopInsideParameterValueInsideModuleLoop 'modulea.bicep' = [for thing in range(0,1): {
//@[1578:1703]       "copy": {
  name: 'propertyLoopInsideParameterValueInsideModuleLoop'
//@[1585:1585]       "name": "propertyLoopInsideParameterValueInsideModuleLoop",
  params: {
    objParam: {
      a: [for i in range(0,10): i + thing]
//@[1595:1599]                   "name": "a",
      b: [for i in range(1,2): i * thing]
//@[1600:1604]                   "name": "b",
      c: {
//@[1613:1621]               "c": {
        d: [for j in range(2,3): j]
//@[1615:1619]                     "name": "d",
      }
      e: [for k in range(4,4): {
//@[1605:1611]                   "name": "e",
        f: k - thing
//@[1609:1609]                     "f": "[sub(range(4, 4)[copyIndex('e')], range(0, 1)[copyIndex()])]"
      }]
    }
    stringParamB: ''
    arrayParam: [
      {
        e: [for j in range(7,7): j % thing]
//@[1631:1635]                     "name": "e",
      }
    ]
  }
}]


// BEGIN: Key Vault Secret Reference

resource kv 'Microsoft.KeyVault/vaults@2019-09-01' existing = {
  name: 'testkeyvault'
}

module secureModule1 'child/secureParams.bicep' = {
//@[1704:1760]       "type": "Microsoft.Resources/deployments",
  name: 'secureModule1'
//@[1707:1707]       "name": "secureModule1",
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
//@[1761:1817]       "type": "Microsoft.Resources/deployments",
  name: 'secureModule2'
//@[1764:1764]       "name": "secureModule2",
  params: {
    secureStringParam1: scopedKv.getSecret('mySecret')
    secureStringParam2: scopedKv.getSecret('mySecret','secretVersion')
  }
}

//looped module with looped existing resource (Issue #2862)
var vaults = [
//@[35:46]     "vaults": [
  {
    vaultName: 'test-1-kv'
//@[37:37]         "vaultName": "test-1-kv",
    vaultRG: 'test-1-rg'
//@[38:38]         "vaultRG": "test-1-rg",
    vaultSub: 'abcd-efgh'
//@[39:39]         "vaultSub": "abcd-efgh"
  }
  {
    vaultName: 'test-2-kv'
//@[42:42]         "vaultName": "test-2-kv",
    vaultRG: 'test-2-rg'
//@[43:43]         "vaultRG": "test-2-rg",
    vaultSub: 'ijkl-1adg1'
//@[44:44]         "vaultSub": "ijkl-1adg1"
  }
]
var secrets = [
//@[47:56]     "secrets": [
  {
    name: 'secret01'
//@[49:49]         "name": "secret01",
    version: 'versionA'
//@[50:50]         "version": "versionA"
  }
  {
    name: 'secret02'
//@[53:53]         "name": "secret02",
    version: 'versionB'
//@[54:54]         "version": "versionB"
  }
]

resource loopedKv 'Microsoft.KeyVault/vaults@2019-09-01' existing = [for vault in vaults: {
  name: vault.vaultName
  scope: resourceGroup(vault.vaultSub, vault.vaultRG)
}]

module secureModuleLooped 'child/secureParams.bicep' = [for (secret, i) in secrets: {
//@[1818:1878]       "copy": {
  name: 'secureModuleLooped-${i}'
//@[1825:1825]       "name": "[format('secureModuleLooped-{0}', copyIndex())]",
  params: {
    secureStringParam1: loopedKv[i].getSecret(secret.name)
    secureStringParam2: loopedKv[i].getSecret(secret.name, secret.version)
  }
}]


// END: Key Vault Secret Reference

module withSpace 'module with space.bicep' = {
//@[1879:1913]       "type": "Microsoft.Resources/deployments",
  name: 'withSpace'
//@[1882:1882]       "name": "withSpace",
}

module folderWithSpace 'child/folder with space/child with space.bicep' = {
//@[1914:1948]       "type": "Microsoft.Resources/deployments",
  name: 'childWithSpace'
//@[1917:1917]       "name": "childWithSpace",
}

