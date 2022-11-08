
@sys.description('this is basicStorage')
//@[67:67]         "description": "this is basicStorage"
resource basicStorage 'Microsoft.Storage/storageAccounts@2019-06-01' = {
//@[57:69]       "type": "Microsoft.Storage/storageAccounts",
  name: 'basicblobs'
  location: 'westus'
//@[61:61]       "location": "westus",
  kind: 'BlobStorage'
//@[62:62]       "kind": "BlobStorage",
  sku: {
//@[63:65]       "sku": {
    name: 'Standard_GRS'
//@[64:64]         "name": "Standard_GRS"
  }
}

@sys.description('this is dnsZone')
//@[76:76]         "description": "this is dnsZone"
resource dnsZone 'Microsoft.Network/dnszones@2018-05-01' = {
//@[70:78]       "type": "Microsoft.Network/dnsZones",
  name: 'myZone'
  location: 'global'
//@[74:74]       "location": "global",
}

resource myStorageAccount 'Microsoft.Storage/storageAccounts@2017-10-01' = {
//@[79:103]       "type": "Microsoft.Storage/storageAccounts",
  name: 'myencryptedone'
  location: 'eastus2'
//@[83:83]       "location": "eastus2",
  properties: {
//@[84:98]       "properties": {
    supportsHttpsTrafficOnly: true
//@[85:85]         "supportsHttpsTrafficOnly": true,
    accessTier: 'Hot'
//@[86:86]         "accessTier": "Hot",
    encryption: {
//@[87:97]         "encryption": {
      keySource: 'Microsoft.Storage'
//@[88:88]           "keySource": "Microsoft.Storage",
      services: {
//@[89:96]           "services": {
        blob: {
//@[90:92]             "blob": {
          enabled: true
//@[91:91]               "enabled": true
        }
        file: {
//@[93:95]             "file": {
          enabled: true
//@[94:94]               "enabled": true
        }
      }
    }
  }
  kind: 'StorageV2'
//@[99:99]       "kind": "StorageV2",
  sku: {
//@[100:102]       "sku": {
    name: 'Standard_LRS'
//@[101:101]         "name": "Standard_LRS"
  }
}

resource withExpressions 'Microsoft.Storage/storageAccounts@2017-10-01' = {
//@[104:131]       "type": "Microsoft.Storage/storageAccounts",
  name: 'myencryptedone2'
  location: 'eastus2'
//@[108:108]       "location": "eastus2",
  properties: {
//@[109:123]       "properties": {
    supportsHttpsTrafficOnly: !false
//@[110:110]         "supportsHttpsTrafficOnly": "[not(false())]",
    accessTier: true ? 'Hot' : 'Cold'
//@[111:111]         "accessTier": "[if(true(), 'Hot', 'Cold')]",
    encryption: {
//@[112:122]         "encryption": {
      keySource: 'Microsoft.Storage'
//@[113:113]           "keySource": "Microsoft.Storage",
      services: {
//@[114:121]           "services": {
        blob: {
//@[115:117]             "blob": {
          enabled: true || false
//@[116:116]               "enabled": "[or(true(), false())]"
        }
        file: {
//@[118:120]             "file": {
          enabled: true
//@[119:119]               "enabled": true
        }
      }
    }
  }
  kind: 'StorageV2'
//@[124:124]       "kind": "StorageV2",
  sku: {
//@[125:127]       "sku": {
    name: 'Standard_LRS'
//@[126:126]         "name": "Standard_LRS"
  }
  dependsOn: [
    myStorageAccount
  ]
}

param applicationName string = 'to-do-app${uniqueString(resourceGroup().id)}'
//@[11:14]     "applicationName": {
var hostingPlanName = applicationName // why not just use the param directly?
//@[33:33]     "hostingPlanName": "[parameters('applicationName')]",

param appServicePlanTier string
//@[15:17]     "appServicePlanTier": {
param appServicePlanInstances int
//@[18:20]     "appServicePlanInstances": {

var location = resourceGroup().location
//@[34:34]     "location": "[resourceGroup().location]",

resource farm 'Microsoft.Web/serverFarms@2019-08-01' = {
//@[132:144]       "type": "Microsoft.Web/serverfarms",
  // dependsOn: resourceId('Microsoft.DocumentDB/databaseAccounts', cosmosAccountName)
  name: hostingPlanName
  location: location
//@[136:136]       "location": "[variables('location')]",
  sku: {
//@[137:140]       "sku": {
    name: appServicePlanTier
//@[138:138]         "name": "[parameters('appServicePlanTier')]",
    capacity: appServicePlanInstances
//@[139:139]         "capacity": "[parameters('appServicePlanInstances')]"
  }
  properties: {
//@[141:143]       "properties": {
    name: hostingPlanName // just hostingPlanName results in an error
//@[142:142]         "name": "[variables('hostingPlanName')]"
  }
}

var cosmosDbResourceId = resourceId('Microsoft.DocumentDB/databaseAccounts', cosmosDb.account)
//@[35:35]     "cosmosDbResourceId": "[resourceId('Microsoft.DocumentDB/databaseAccounts', parameters('cosmosDb').account)]",
var cosmosDbRef = reference(cosmosDbResourceId).documentEndpoint

// this variable is not accessed anywhere in this template and depends on a run-time reference
// it should not be present at all in the template output as there is nowhere logical to put it
var cosmosDbEndpoint = cosmosDbRef.documentEndpoint

param webSiteName string
//@[21:23]     "webSiteName": {
param cosmosDb object
//@[24:26]     "cosmosDb": {
resource site 'Microsoft.Web/sites@2019-08-01' = {
//@[145:172]       "type": "Microsoft.Web/sites",
  name: webSiteName
  location: location
//@[149:149]       "location": "[variables('location')]",
  properties: {
//@[150:171]       "properties": {
    // not yet supported // serverFarmId: farm.id
    siteConfig: {
//@[151:170]         "siteConfig": {
      appSettings: [
//@[152:169]           "appSettings": [
        {
          name: 'CosmosDb:Account'
//@[154:154]               "name": "CosmosDb:Account",
          value: reference(cosmosDbResourceId).documentEndpoint
//@[155:155]               "value": "[reference(variables('cosmosDbResourceId')).documentEndpoint]"
        }
        {
          name: 'CosmosDb:Key'
//@[158:158]               "name": "CosmosDb:Key",
          value: listKeys(cosmosDbResourceId, '2020-04-01').primaryMasterKey
//@[159:159]               "value": "[listKeys(variables('cosmosDbResourceId'), '2020-04-01').primaryMasterKey]"
        }
        {
          name: 'CosmosDb:DatabaseName'
//@[162:162]               "name": "CosmosDb:DatabaseName",
          value: cosmosDb.databaseName
//@[163:163]               "value": "[parameters('cosmosDb').databaseName]"
        }
        {
          name: 'CosmosDb:ContainerName'
//@[166:166]               "name": "CosmosDb:ContainerName",
          value: cosmosDb.containerName
//@[167:167]               "value": "[parameters('cosmosDb').containerName]"
        }
      ]
    }
  }
}

var _siteApiVersion = site.apiVersion
//@[36:36]     "_siteApiVersion": "2019-08-01",
var _siteType = site.type
//@[37:37]     "_siteType": "Microsoft.Web/sites",

output siteApiVersion string = site.apiVersion
//@[576:579]     "siteApiVersion": {
output siteType string = site.type
//@[580:583]     "siteType": {

resource nested 'Microsoft.Resources/deployments@2019-10-01' = {
//@[173:185]       "type": "Microsoft.Resources/deployments",
  name: 'nestedTemplate1'
  properties: {
//@[177:184]       "properties": {
    mode: 'Incremental'
//@[178:178]         "mode": "Incremental",
    template: {
//@[179:183]         "template": {
      // string key value
      '$schema': 'https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#'
//@[180:180]           "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
      contentVersion: '1.0.0.0'
//@[181:181]           "contentVersion": "1.0.0.0",
      resources: [
//@[182:182]           "resources": []
      ]
    }
  }
}

// should be able to access the read only properties
resource accessingReadOnlyProperties 'Microsoft.Foo/foos@2019-10-01' = {
//@[186:200]       "type": "Microsoft.Foo/foos",
  name: 'nestedTemplate1'
  properties: {
//@[190:196]       "properties": {
    otherId: nested.id
//@[191:191]         "otherId": "[resourceId('Microsoft.Resources/deployments', 'nestedTemplate1')]",
    otherName: nested.name
//@[192:192]         "otherName": "nestedTemplate1",
    otherVersion: nested.apiVersion
//@[193:193]         "otherVersion": "2019-10-01",
    otherType: nested.type
//@[194:194]         "otherType": "Microsoft.Resources/deployments",

    otherThings: nested.properties.mode
//@[195:195]         "otherThings": "[reference(resourceId('Microsoft.Resources/deployments', 'nestedTemplate1'), '2019-10-01').mode]"
  }
}

resource resourceA 'My.Rp/typeA@2020-01-01' = {
//@[201:205]       "type": "My.Rp/typeA",
  name: 'resourceA'
}

resource resourceB 'My.Rp/typeA/typeB@2020-01-01' = {
//@[206:213]       "type": "My.Rp/typeA/typeB",
  name: '${resourceA.name}/myName'
}

resource resourceC 'My.Rp/typeA/typeB@2020-01-01' = {
//@[214:229]       "type": "My.Rp/typeA/typeB",
  name: '${resourceA.name}/myName'
  properties: {
//@[218:224]       "properties": {
    aId: resourceA.id
//@[219:219]         "aId": "[resourceId('My.Rp/typeA', 'resourceA')]",
    aType: resourceA.type
//@[220:220]         "aType": "My.Rp/typeA",
    aName: resourceA.name
//@[221:221]         "aName": "resourceA",
    aApiVersion: resourceA.apiVersion
//@[222:222]         "aApiVersion": "2020-01-01",
    bProperties: resourceB.properties
//@[223:223]         "bProperties": "[reference(resourceId('My.Rp/typeA/typeB', split(format('{0}/myName', 'resourceA'), '/')[0], split(format('{0}/myName', 'resourceA'), '/')[1]), '2020-01-01')]"
  }
}

var varARuntime = {
  bId: resourceB.id
//@[237:237]             "bId": "[resourceId('My.Rp/typeA/typeB', split(format('{0}/myName', 'resourceA'), '/')[0], split(format('{0}/myName', 'resourceA'), '/')[1])]",
  bType: resourceB.type
//@[238:238]             "bType": "My.Rp/typeA/typeB",
  bName: resourceB.name
//@[239:239]             "bName": "[format('{0}/myName', 'resourceA')]",
  bApiVersion: resourceB.apiVersion
//@[240:240]             "bApiVersion": "2020-01-01",
  aKind: resourceA.kind
//@[241:241]             "aKind": "[reference(resourceId('My.Rp/typeA', 'resourceA'), '2020-01-01', 'full').kind]"
}

var varBRuntime = [
  varARuntime
//@[236:242]             "bId": "[resourceId('My.Rp/typeA/typeB', split(format('{0}/myName', 'resourceA'), '/')[0], split(format('{0}/myName', 'resourceA'), '/')[1])]",
]

var resourceCRef = {
//@[38:40]     "resourceCRef": {
  id: resourceC.id
//@[39:39]       "id": "[resourceId('My.Rp/typeA/typeB', split(format('{0}/myName', 'resourceA'), '/')[0], split(format('{0}/myName', 'resourceA'), '/')[1])]"
}
var setResourceCRef = true
//@[41:41]     "setResourceCRef": true,

resource resourceD 'My.Rp/typeD@2020-01-01' = {
//@[230:251]       "type": "My.Rp/typeD",
  name: 'constant'
  properties: {
//@[234:245]       "properties": {
    runtime: varBRuntime
//@[235:243]         "runtime": [
    // repro for https://github.com/Azure/bicep/issues/316
    repro316: setResourceCRef ? resourceCRef : null
//@[244:244]         "repro316": "[if(variables('setResourceCRef'), variables('resourceCRef'), null())]"
  }
}

var myInterpKey = 'abc'
//@[42:42]     "myInterpKey": "abc",
resource resourceWithInterp 'My.Rp/interp@2020-01-01' = {
//@[252:261]       "type": "My.Rp/interp",
  name: 'interpTest'
  properties: {
//@[256:260]       "properties": {
    '${myInterpKey}': 1
//@[257:257]         "[format('{0}', variables('myInterpKey'))]": 1,
    'abc${myInterpKey}def': 2
//@[258:258]         "[format('abc{0}def', variables('myInterpKey'))]": 2,
    '${myInterpKey}abc${myInterpKey}': 3
//@[259:259]         "[format('{0}abc{1}', variables('myInterpKey'), variables('myInterpKey'))]": 3
  }
}

resource resourceWithEscaping 'My.Rp/mockResource@2020-01-01' = {
//@[262:269]       "type": "My.Rp/mockResource",
  name: 'test'
  properties: {
//@[266:268]       "properties": {
    // both key and value should be escaped in template output
    '[resourceGroup().location]': '[resourceGroup().location]'
//@[267:267]         "[[resourceGroup().location]": "[[resourceGroup().location]"
  }
}

param shouldDeployVm bool = true
//@[27:30]     "shouldDeployVm": {

@sys.description('this is vmWithCondition')
//@[284:284]         "description": "this is vmWithCondition"
resource vmWithCondition 'Microsoft.Compute/virtualMachines@2020-06-01' = if (shouldDeployVm) {
//@[270:286]       "condition": "[parameters('shouldDeployVm')]",
  name: 'vmName'
  location: 'westus'
//@[275:275]       "location": "westus",
  properties: {
//@[276:282]       "properties": {
    osProfile: {
//@[277:281]         "osProfile": {
      windowsConfiguration: {
//@[278:280]           "windowsConfiguration": {
        enableAutomaticUpdates: true
//@[279:279]             "enableAutomaticUpdates": true
      }
    }
  }
}

resource extension1 'My.Rp/extensionResource@2020-12-01' = {
//@[287:295]       "type": "My.Rp/extensionResource",
  name: 'extension'
  scope: vmWithCondition
}

resource extension2 'My.Rp/extensionResource@2020-12-01' = {
//@[296:304]       "type": "My.Rp/extensionResource",
  name: 'extension'
  scope: extension1
}

resource extensionDependencies 'My.Rp/mockResource@2020-01-01' = {
//@[305:322]       "type": "My.Rp/mockResource",
  name: 'extensionDependencies'
  properties: {
//@[309:316]       "properties": {
    res1: vmWithCondition.id
//@[310:310]         "res1": "[resourceId('Microsoft.Compute/virtualMachines', 'vmName')]",
    res1runtime: vmWithCondition.properties.something
//@[311:311]         "res1runtime": "[reference(resourceId('Microsoft.Compute/virtualMachines', 'vmName'), '2020-06-01').something]",
    res2: extension1.id
//@[312:312]         "res2": "[extensionResourceId(resourceId('Microsoft.Compute/virtualMachines', 'vmName'), 'My.Rp/extensionResource', 'extension')]",
    res2runtime: extension1.properties.something
//@[313:313]         "res2runtime": "[reference(extensionResourceId(resourceId('Microsoft.Compute/virtualMachines', 'vmName'), 'My.Rp/extensionResource', 'extension'), '2020-12-01').something]",
    res3: extension2.id
//@[314:314]         "res3": "[extensionResourceId(extensionResourceId(resourceId('Microsoft.Compute/virtualMachines', 'vmName'), 'My.Rp/extensionResource', 'extension'), 'My.Rp/extensionResource', 'extension')]",
    res3runtime: extension2.properties.something
//@[315:315]         "res3runtime": "[reference(extensionResourceId(extensionResourceId(resourceId('Microsoft.Compute/virtualMachines', 'vmName'), 'My.Rp/extensionResource', 'extension'), 'My.Rp/extensionResource', 'extension'), '2020-12-01').something]"
  }
}

@sys.description('this is existing1')
resource existing1 'Mock.Rp/existingExtensionResource@2020-01-01' existing = {
  name: 'existing1'
  scope: extension1
}

resource existing2 'Mock.Rp/existingExtensionResource@2020-01-01' existing = {
  name: 'existing2'
  scope: existing1
}

resource extension3 'My.Rp/extensionResource@2020-12-01' = {
//@[323:331]       "type": "My.Rp/extensionResource",
  name: 'extension3'
  scope: existing1
}

/*
  valid loop cases
*/ 
var storageAccounts = [
//@[43:52]     "storageAccounts": [
  {
    name: 'one'
//@[45:45]         "name": "one",
    location: 'eastus2'
//@[46:46]         "location": "eastus2"
  }
  {
    name: 'two'
//@[49:49]         "name": "two",
    location: 'westus'
//@[50:50]         "location": "westus"
  }
]

// just a storage account loop
@sys.description('this is just a storage account loop')
//@[346:346]         "description": "this is just a storage account loop"
resource storageResources 'Microsoft.Storage/storageAccounts@2019-06-01' = [for account in storageAccounts: {
//@[332:348]       "copy": {
  name: account.name
  location: account.location
//@[340:340]       "location": "[variables('storageAccounts')[copyIndex()].location]",
  sku: {
//@[341:343]       "sku": {
    name: 'Standard_LRS'
//@[342:342]         "name": "Standard_LRS"
  }
  kind: 'StorageV2'
//@[344:344]       "kind": "StorageV2",
}]

// storage account loop with index
@sys.description('this is just a storage account loop with index')
//@[363:363]         "description": "this is just a storage account loop with index"
resource storageResourcesWithIndex 'Microsoft.Storage/storageAccounts@2019-06-01' = [for (account, i) in storageAccounts: {
//@[349:365]       "copy": {
  name: '${account.name}${i}'
  location: account.location
//@[357:357]       "location": "[variables('storageAccounts')[copyIndex()].location]",
  sku: {
//@[358:360]       "sku": {
    name: 'Standard_LRS'
//@[359:359]         "name": "Standard_LRS"
  }
  kind: 'StorageV2'
//@[361:361]       "kind": "StorageV2",
}]

// basic nested loop
@sys.description('this is just a basic nested loop')
//@[386:386]         "description": "this is just a basic nested loop"
resource vnet 'Microsoft.Network/virtualNetworks@2020-06-01' = [for i in range(0, 3): {
//@[366:388]       "copy": {
  name: 'vnet-${i}'
  properties: {
//@[374:384]       "properties": {
    subnets: [for j in range(0, 4): {
//@[376:382]             "name": "subnets",
      // #completionTest(0,1,2,3,4,5) -> subnetIdAndProperties
     
      // #completionTest(6) -> subnetIdAndPropertiesNoColon
      name: 'subnet-${i}-${j}'
//@[380:380]               "name": "[format('subnet-{0}-{1}', range(0, 3)[copyIndex()], range(0, 4)[copyIndex('subnets')])]"
    }]
  }
}]

// duplicate identifiers within the loop are allowed
resource duplicateIdentifiersWithinLoop 'Microsoft.Network/virtualNetworks@2020-06-01' = [for i in range(0, 3): {
//@[389:408]       "copy": {
  name: 'vnet-${i}'
  properties: {
//@[397:407]       "properties": {
    subnets: [for i in range(0, 4): {
//@[399:405]             "name": "subnets",
      name: 'subnet-${i}-${i}'
//@[403:403]               "name": "[format('subnet-{0}-{1}', range(0, 4)[copyIndex('subnets')], range(0, 4)[copyIndex('subnets')])]"
    }]
  }
}]

// duplicate identifers in global and single loop scope are allowed (inner variable hides the outer)
var canHaveDuplicatesAcrossScopes = 'hello'
//@[53:53]     "canHaveDuplicatesAcrossScopes": "hello",
resource duplicateInGlobalAndOneLoop 'Microsoft.Network/virtualNetworks@2020-06-01' = [for canHaveDuplicatesAcrossScopes in range(0, 3): {
//@[409:428]       "copy": {
  name: 'vnet-${canHaveDuplicatesAcrossScopes}'
  properties: {
//@[417:427]       "properties": {
    subnets: [for i in range(0, 4): {
//@[419:425]             "name": "subnets",
      name: 'subnet-${i}-${i}'
//@[423:423]               "name": "[format('subnet-{0}-{1}', range(0, 4)[copyIndex('subnets')], range(0, 4)[copyIndex('subnets')])]"
    }]
  }
}]

// duplicate in global and multiple loop scopes are allowed (inner hides the outer)
var duplicatesEverywhere = 'hello'
//@[54:54]     "duplicatesEverywhere": "hello"
resource duplicateInGlobalAndTwoLoops 'Microsoft.Network/virtualNetworks@2020-06-01' = [for duplicatesEverywhere in range(0, 3): {
//@[429:448]       "copy": {
  name: 'vnet-${duplicatesEverywhere}'
  properties: {
//@[437:447]       "properties": {
    subnets: [for duplicatesEverywhere in range(0, 4): {
//@[439:445]             "name": "subnets",
      name: 'subnet-${duplicatesEverywhere}'
//@[443:443]               "name": "[format('subnet-{0}', range(0, 4)[copyIndex('subnets')])]"
    }]
  }
}]

/*
  Scope values created via array access on a resource collection
*/
resource dnsZones 'Microsoft.Network/dnsZones@2018-05-01' = [for zone in range(0,4): {
//@[449:458]       "copy": {
  name: 'zone${zone}'
  location: 'global'
//@[457:457]       "location": "global"
}]

resource locksOnZones 'Microsoft.Authorization/locks@2016-09-01' = [for lock in range(0,2): {
//@[459:474]       "copy": {
  name: 'lock${lock}'
  properties: {
//@[468:470]       "properties": {
    level: 'CanNotDelete'
//@[469:469]         "level": "CanNotDelete"
  }
  scope: dnsZones[lock]
}]

resource moreLocksOnZones 'Microsoft.Authorization/locks@2016-09-01' = [for (lock, i) in range(0,3): {
//@[475:490]       "copy": {
  name: 'another${i}'
  properties: {
//@[484:486]       "properties": {
    level: 'ReadOnly'
//@[485:485]         "level": "ReadOnly"
  }
  scope: dnsZones[i]
}]

resource singleLockOnFirstZone 'Microsoft.Authorization/locks@2016-09-01' = {
//@[491:502]       "type": "Microsoft.Authorization/locks",
  name: 'single-lock'
  properties: {
//@[496:498]       "properties": {
    level: 'ReadOnly'
//@[497:497]         "level": "ReadOnly"
  }
  scope: dnsZones[0]
}


resource p1_vnet 'Microsoft.Network/virtualNetworks@2020-06-01' = {
//@[503:515]       "type": "Microsoft.Network/virtualNetworks",
  location: resourceGroup().location
//@[507:507]       "location": "[resourceGroup().location]",
  name: 'myVnet'
  properties: {
//@[508:514]       "properties": {
    addressSpace: {
//@[509:513]         "addressSpace": {
      addressPrefixes: [
//@[510:512]           "addressPrefixes": [
        '10.0.0.0/20'
//@[511:511]             "10.0.0.0/20"
      ]
    }
  }
}

resource p1_subnet1 'Microsoft.Network/virtualNetworks/subnets@2020-06-01' = {
//@[516:526]       "type": "Microsoft.Network/virtualNetworks/subnets",
  parent: p1_vnet
  name: 'subnet1'
  properties: {
//@[520:522]       "properties": {
    addressPrefix: '10.0.0.0/24'
//@[521:521]         "addressPrefix": "10.0.0.0/24"
  }
}

resource p1_subnet2 'Microsoft.Network/virtualNetworks/subnets@2020-06-01' = {
//@[527:537]       "type": "Microsoft.Network/virtualNetworks/subnets",
  parent: p1_vnet
  name: 'subnet2'
  properties: {
//@[531:533]       "properties": {
    addressPrefix: '10.0.1.0/24'
//@[532:532]         "addressPrefix": "10.0.1.0/24"
  }
}

output p1_subnet1prefix string = p1_subnet1.properties.addressPrefix
//@[584:587]     "p1_subnet1prefix": {
output p1_subnet1name string = p1_subnet1.name
//@[588:591]     "p1_subnet1name": {
output p1_subnet1type string = p1_subnet1.type
//@[592:595]     "p1_subnet1type": {
output p1_subnet1id string = p1_subnet1.id
//@[596:599]     "p1_subnet1id": {

// parent property with extension resource
resource p2_res1 'Microsoft.Rp1/resource1@2020-06-01' = {
//@[538:542]       "type": "Microsoft.Rp1/resource1",
  name: 'res1'
}

resource p2_res1child 'Microsoft.Rp1/resource1/child1@2020-06-01' = {
//@[543:550]       "type": "Microsoft.Rp1/resource1/child1",
  parent: p2_res1
  name: 'child1'
}

resource p2_res2 'Microsoft.Rp2/resource2@2020-06-01' = {
//@[551:559]       "type": "Microsoft.Rp2/resource2",
  scope: p2_res1child
  name: 'res2'
}

resource p2_res2child 'Microsoft.Rp2/resource2/child2@2020-06-01' = {
//@[560:568]       "type": "Microsoft.Rp2/resource2/child2",
  parent: p2_res2
  name: 'child2'
}

output p2_res2childprop string = p2_res2child.properties.someProp
//@[600:603]     "p2_res2childprop": {
output p2_res2childname string = p2_res2child.name
//@[604:607]     "p2_res2childname": {
output p2_res2childtype string = p2_res2child.type
//@[608:611]     "p2_res2childtype": {
output p2_res2childid string = p2_res2child.id
//@[612:615]     "p2_res2childid": {

// parent property with 'existing' resource
resource p3_res1 'Microsoft.Rp1/resource1@2020-06-01' existing = {
  name: 'res1'
}

resource p3_child1 'Microsoft.Rp1/resource1/child1@2020-06-01' = {
//@[569:573]       "type": "Microsoft.Rp1/resource1/child1",
  parent: p3_res1
  name: 'child1'
}

output p3_res1childprop string = p3_child1.properties.someProp
//@[616:619]     "p3_res1childprop": {
output p3_res1childname string = p3_child1.name
//@[620:623]     "p3_res1childname": {
output p3_res1childtype string = p3_child1.type
//@[624:627]     "p3_res1childtype": {
output p3_res1childid string = p3_child1.id
//@[628:631]     "p3_res1childid": {

// parent & child with 'existing'
resource p4_res1 'Microsoft.Rp1/resource1@2020-06-01' existing = {
  scope: tenant()
  name: 'res1'
}

resource p4_child1 'Microsoft.Rp1/resource1/child1@2020-06-01' existing = {
  parent: p4_res1
  name: 'child1'
}

output p4_res1childprop string = p4_child1.properties.someProp
//@[632:635]     "p4_res1childprop": {
output p4_res1childname string = p4_child1.name
//@[636:639]     "p4_res1childname": {
output p4_res1childtype string = p4_child1.type
//@[640:643]     "p4_res1childtype": {
output p4_res1childid string = p4_child1.id
//@[644:647]     "p4_res1childid": {

