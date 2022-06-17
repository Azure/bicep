
@sys.description('this is basicStorage')
resource basicStorage 'Microsoft.Storage/storageAccounts@2019-06-01' = {
//@[58:70]       "type": "Microsoft.Storage/storageAccounts",
  name: 'basicblobs'
  location: 'westus'
//@[62:62]       "location": "westus",
  kind: 'BlobStorage'
//@[63:63]       "kind": "BlobStorage",
  sku: {
//@[64:66]       "sku": {
    name: 'Standard_GRS'
//@[65:65]         "name": "Standard_GRS"
  }
}

@sys.description('this is dnsZone')
resource dnsZone 'Microsoft.Network/dnszones@2018-05-01' = {
//@[71:79]       "type": "Microsoft.Network/dnsZones",
  name: 'myZone'
  location: 'global'
//@[75:75]       "location": "global",
}

resource myStorageAccount 'Microsoft.Storage/storageAccounts@2017-10-01' = {
//@[80:104]       "type": "Microsoft.Storage/storageAccounts",
  name: 'myencryptedone'
  location: 'eastus2'
//@[84:84]       "location": "eastus2",
  properties: {
//@[85:99]       "properties": {
    supportsHttpsTrafficOnly: true
//@[86:86]         "supportsHttpsTrafficOnly": true,
    accessTier: 'Hot'
//@[87:87]         "accessTier": "Hot",
    encryption: {
//@[88:98]         "encryption": {
      keySource: 'Microsoft.Storage'
//@[89:89]           "keySource": "Microsoft.Storage",
      services: {
//@[90:97]           "services": {
        blob: {
//@[91:93]             "blob": {
          enabled: true
//@[92:92]               "enabled": true
        }
        file: {
//@[94:96]             "file": {
          enabled: true
//@[95:95]               "enabled": true
        }
      }
    }
  }
  kind: 'StorageV2'
//@[100:100]       "kind": "StorageV2",
  sku: {
//@[101:103]       "sku": {
    name: 'Standard_LRS'
//@[102:102]         "name": "Standard_LRS"
  }
}

resource withExpressions 'Microsoft.Storage/storageAccounts@2017-10-01' = {
//@[105:132]       "type": "Microsoft.Storage/storageAccounts",
  name: 'myencryptedone2'
  location: 'eastus2'
//@[109:109]       "location": "eastus2",
  properties: {
//@[110:124]       "properties": {
    supportsHttpsTrafficOnly: !false
//@[111:111]         "supportsHttpsTrafficOnly": "[not(false())]",
    accessTier: true ? 'Hot' : 'Cold'
//@[112:112]         "accessTier": "[if(true(), 'Hot', 'Cold')]",
    encryption: {
//@[113:123]         "encryption": {
      keySource: 'Microsoft.Storage'
//@[114:114]           "keySource": "Microsoft.Storage",
      services: {
//@[115:122]           "services": {
        blob: {
//@[116:118]             "blob": {
          enabled: true || false
//@[117:117]               "enabled": "[or(true(), false())]"
        }
        file: {
//@[119:121]             "file": {
          enabled: true
//@[120:120]               "enabled": true
        }
      }
    }
  }
  kind: 'StorageV2'
//@[125:125]       "kind": "StorageV2",
  sku: {
//@[126:128]       "sku": {
    name: 'Standard_LRS'
//@[127:127]         "name": "Standard_LRS"
  }
  dependsOn: [
    myStorageAccount
  ]
}

param applicationName string = 'to-do-app${uniqueString(resourceGroup().id)}'
//@[12:15]     "applicationName": {
var hostingPlanName = applicationName // why not just use the param directly?
//@[34:34]     "hostingPlanName": "[parameters('applicationName')]",

param appServicePlanTier string
//@[16:18]     "appServicePlanTier": {
param appServicePlanInstances int
//@[19:21]     "appServicePlanInstances": {

var location = resourceGroup().location
//@[35:35]     "location": "[resourceGroup().location]",

resource farm 'Microsoft.Web/serverFarms@2019-08-01' = {
//@[133:145]       "type": "Microsoft.Web/serverfarms",
  // dependsOn: resourceId('Microsoft.DocumentDB/databaseAccounts', cosmosAccountName)
  name: hostingPlanName
  location: location
//@[137:137]       "location": "[variables('location')]",
  sku: {
//@[138:141]       "sku": {
    name: appServicePlanTier
//@[139:139]         "name": "[parameters('appServicePlanTier')]",
    capacity: appServicePlanInstances
//@[140:140]         "capacity": "[parameters('appServicePlanInstances')]"
  }
  properties: {
//@[142:144]       "properties": {
    name: hostingPlanName // just hostingPlanName results in an error
//@[143:143]         "name": "[variables('hostingPlanName')]"
  }
}

var cosmosDbResourceId = resourceId('Microsoft.DocumentDB/databaseAccounts', cosmosDb.account)
//@[36:36]     "cosmosDbResourceId": "[resourceId('Microsoft.DocumentDB/databaseAccounts', parameters('cosmosDb').account)]",
var cosmosDbRef = reference(cosmosDbResourceId).documentEndpoint

// this variable is not accessed anywhere in this template and depends on a run-time reference
// it should not be present at all in the template output as there is nowhere logical to put it
var cosmosDbEndpoint = cosmosDbRef.documentEndpoint

param webSiteName string
//@[22:24]     "webSiteName": {
param cosmosDb object
//@[25:27]     "cosmosDb": {
resource site 'Microsoft.Web/sites@2019-08-01' = {
//@[146:173]       "type": "Microsoft.Web/sites",
  name: webSiteName
  location: location
//@[150:150]       "location": "[variables('location')]",
  properties: {
//@[151:172]       "properties": {
    // not yet supported // serverFarmId: farm.id
    siteConfig: {
//@[152:171]         "siteConfig": {
      appSettings: [
//@[153:170]           "appSettings": [
        {
          name: 'CosmosDb:Account'
//@[155:155]               "name": "CosmosDb:Account",
          value: reference(cosmosDbResourceId).documentEndpoint
//@[156:156]               "value": "[reference(variables('cosmosDbResourceId')).documentEndpoint]"
        }
        {
          name: 'CosmosDb:Key'
//@[159:159]               "name": "CosmosDb:Key",
          value: listKeys(cosmosDbResourceId, '2020-04-01').primaryMasterKey
//@[160:160]               "value": "[listKeys(variables('cosmosDbResourceId'), '2020-04-01').primaryMasterKey]"
        }
        {
          name: 'CosmosDb:DatabaseName'
//@[163:163]               "name": "CosmosDb:DatabaseName",
          value: cosmosDb.databaseName
//@[164:164]               "value": "[parameters('cosmosDb').databaseName]"
        }
        {
          name: 'CosmosDb:ContainerName'
//@[167:167]               "name": "CosmosDb:ContainerName",
          value: cosmosDb.containerName
//@[168:168]               "value": "[parameters('cosmosDb').containerName]"
        }
      ]
    }
  }
}

var _siteApiVersion = site.apiVersion
//@[37:37]     "_siteApiVersion": "2019-08-01",
var _siteType = site.type
//@[38:38]     "_siteType": "Microsoft.Web/sites",

output siteApiVersion string = site.apiVersion
//@[577:580]     "siteApiVersion": {
output siteType string = site.type
//@[581:584]     "siteType": {

resource nested 'Microsoft.Resources/deployments@2019-10-01' = {
//@[174:186]       "type": "Microsoft.Resources/deployments",
  name: 'nestedTemplate1'
  properties: {
//@[178:185]       "properties": {
    mode: 'Incremental'
//@[179:179]         "mode": "Incremental",
    template: {
//@[180:184]         "template": {
      // string key value
      '$schema': 'https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#'
//@[181:181]           "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
      contentVersion: '1.0.0.0'
//@[182:182]           "contentVersion": "1.0.0.0",
      resources: [
//@[183:183]           "resources": []
      ]
    }
  }
}

// should be able to access the read only properties
resource accessingReadOnlyProperties 'Microsoft.Foo/foos@2019-10-01' = {
//@[187:201]       "type": "Microsoft.Foo/foos",
  name: 'nestedTemplate1'
  properties: {
//@[191:197]       "properties": {
    otherId: nested.id
//@[192:192]         "otherId": "[resourceId('Microsoft.Resources/deployments', 'nestedTemplate1')]",
    otherName: nested.name
//@[193:193]         "otherName": "nestedTemplate1",
    otherVersion: nested.apiVersion
//@[194:194]         "otherVersion": "2019-10-01",
    otherType: nested.type
//@[195:195]         "otherType": "Microsoft.Resources/deployments",

    otherThings: nested.properties.mode
//@[196:196]         "otherThings": "[reference(resourceId('Microsoft.Resources/deployments', 'nestedTemplate1')).mode]"
  }
}

resource resourceA 'My.Rp/typeA@2020-01-01' = {
//@[202:206]       "type": "My.Rp/typeA",
  name: 'resourceA'
}

resource resourceB 'My.Rp/typeA/typeB@2020-01-01' = {
//@[207:214]       "type": "My.Rp/typeA/typeB",
  name: '${resourceA.name}/myName'
}

resource resourceC 'My.Rp/typeA/typeB@2020-01-01' = {
//@[215:230]       "type": "My.Rp/typeA/typeB",
  name: '${resourceA.name}/myName'
  properties: {
//@[219:225]       "properties": {
    aId: resourceA.id
//@[220:220]         "aId": "[resourceId('My.Rp/typeA', 'resourceA')]",
    aType: resourceA.type
//@[221:221]         "aType": "My.Rp/typeA",
    aName: resourceA.name
//@[222:222]         "aName": "resourceA",
    aApiVersion: resourceA.apiVersion
//@[223:223]         "aApiVersion": "2020-01-01",
    bProperties: resourceB.properties
//@[224:224]         "bProperties": "[reference(resourceId('My.Rp/typeA/typeB', split(format('{0}/myName', 'resourceA'), '/')[0], split(format('{0}/myName', 'resourceA'), '/')[1]))]"
  }
}

var varARuntime = {
  bId: resourceB.id
//@[238:238]             "bId": "[resourceId('My.Rp/typeA/typeB', split(format('{0}/myName', 'resourceA'), '/')[0], split(format('{0}/myName', 'resourceA'), '/')[1])]",
  bType: resourceB.type
//@[239:239]             "bType": "My.Rp/typeA/typeB",
  bName: resourceB.name
//@[240:240]             "bName": "[format('{0}/myName', 'resourceA')]",
  bApiVersion: resourceB.apiVersion
//@[241:241]             "bApiVersion": "2020-01-01",
  aKind: resourceA.kind
//@[242:242]             "aKind": "[reference(resourceId('My.Rp/typeA', 'resourceA'), '2020-01-01', 'full').kind]"
}

var varBRuntime = [
  varARuntime
//@[237:243]             "bId": "[resourceId('My.Rp/typeA/typeB', split(format('{0}/myName', 'resourceA'), '/')[0], split(format('{0}/myName', 'resourceA'), '/')[1])]",
]

var resourceCRef = {
//@[39:41]     "resourceCRef": {
  id: resourceC.id
//@[40:40]       "id": "[resourceId('My.Rp/typeA/typeB', split(format('{0}/myName', 'resourceA'), '/')[0], split(format('{0}/myName', 'resourceA'), '/')[1])]"
}
var setResourceCRef = true
//@[42:42]     "setResourceCRef": true,

resource resourceD 'My.Rp/typeD@2020-01-01' = {
//@[231:252]       "type": "My.Rp/typeD",
  name: 'constant'
  properties: {
//@[235:246]       "properties": {
    runtime: varBRuntime
//@[236:244]         "runtime": [
    // repro for https://github.com/Azure/bicep/issues/316
    repro316: setResourceCRef ? resourceCRef : null
//@[245:245]         "repro316": "[if(variables('setResourceCRef'), variables('resourceCRef'), null())]"
  }
}

var myInterpKey = 'abc'
//@[43:43]     "myInterpKey": "abc",
resource resourceWithInterp 'My.Rp/interp@2020-01-01' = {
//@[253:262]       "type": "My.Rp/interp",
  name: 'interpTest'
  properties: {
//@[257:261]       "properties": {
    '${myInterpKey}': 1
//@[258:258]         "[format('{0}', variables('myInterpKey'))]": 1,
    'abc${myInterpKey}def': 2
//@[259:259]         "[format('abc{0}def', variables('myInterpKey'))]": 2,
    '${myInterpKey}abc${myInterpKey}': 3
//@[260:260]         "[format('{0}abc{1}', variables('myInterpKey'), variables('myInterpKey'))]": 3
  }
}

resource resourceWithEscaping 'My.Rp/mockResource@2020-01-01' = {
//@[263:270]       "type": "My.Rp/mockResource",
  name: 'test'
  properties: {
//@[267:269]       "properties": {
    // both key and value should be escaped in template output
    '[resourceGroup().location]': '[resourceGroup().location]'
//@[268:268]         "[[resourceGroup().location]": "[[resourceGroup().location]"
  }
}

param shouldDeployVm bool = true
//@[28:31]     "shouldDeployVm": {

@sys.description('this is vmWithCondition')
resource vmWithCondition 'Microsoft.Compute/virtualMachines@2020-06-01' = if (shouldDeployVm) {
//@[271:287]       "condition": "[parameters('shouldDeployVm')]",
  name: 'vmName'
  location: 'westus'
//@[276:276]       "location": "westus",
  properties: {
//@[277:283]       "properties": {
    osProfile: {
//@[278:282]         "osProfile": {
      windowsConfiguration: {
//@[279:281]           "windowsConfiguration": {
        enableAutomaticUpdates: true
//@[280:280]             "enableAutomaticUpdates": true
      }
    }
  }
}

resource extension1 'My.Rp/extensionResource@2020-12-01' = {
//@[288:296]       "type": "My.Rp/extensionResource",
  name: 'extension'
  scope: vmWithCondition
}

resource extension2 'My.Rp/extensionResource@2020-12-01' = {
//@[297:305]       "type": "My.Rp/extensionResource",
  name: 'extension'
  scope: extension1
}

resource extensionDependencies 'My.Rp/mockResource@2020-01-01' = {
//@[306:323]       "type": "My.Rp/mockResource",
  name: 'extensionDependencies'
  properties: {
//@[310:317]       "properties": {
    res1: vmWithCondition.id
//@[311:311]         "res1": "[resourceId('Microsoft.Compute/virtualMachines', 'vmName')]",
    res1runtime: vmWithCondition.properties.something
//@[312:312]         "res1runtime": "[reference(resourceId('Microsoft.Compute/virtualMachines', 'vmName'), '2020-06-01').something]",
    res2: extension1.id
//@[313:313]         "res2": "[extensionResourceId(resourceId('Microsoft.Compute/virtualMachines', 'vmName'), 'My.Rp/extensionResource', 'extension')]",
    res2runtime: extension1.properties.something
//@[314:314]         "res2runtime": "[reference(extensionResourceId(resourceId('Microsoft.Compute/virtualMachines', 'vmName'), 'My.Rp/extensionResource', 'extension')).something]",
    res3: extension2.id
//@[315:315]         "res3": "[extensionResourceId(extensionResourceId(resourceId('Microsoft.Compute/virtualMachines', 'vmName'), 'My.Rp/extensionResource', 'extension'), 'My.Rp/extensionResource', 'extension')]",
    res3runtime: extension2.properties.something
//@[316:316]         "res3runtime": "[reference(extensionResourceId(extensionResourceId(resourceId('Microsoft.Compute/virtualMachines', 'vmName'), 'My.Rp/extensionResource', 'extension'), 'My.Rp/extensionResource', 'extension')).something]"
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
//@[324:332]       "type": "My.Rp/extensionResource",
  name: 'extension3'
  scope: existing1
}

/*
  valid loop cases
*/ 
var storageAccounts = [
//@[44:53]     "storageAccounts": [
  {
    name: 'one'
//@[46:46]         "name": "one",
    location: 'eastus2'
//@[47:47]         "location": "eastus2"
  }
  {
    name: 'two'
//@[50:50]         "name": "two",
    location: 'westus'
//@[51:51]         "location": "westus"
  }
]

// just a storage account loop
@sys.description('this is just a storage account loop')
resource storageResources 'Microsoft.Storage/storageAccounts@2019-06-01' = [for account in storageAccounts: {
//@[333:349]       "copy": {
  name: account.name
  location: account.location
//@[341:341]       "location": "[variables('storageAccounts')[copyIndex()].location]",
  sku: {
//@[342:344]       "sku": {
    name: 'Standard_LRS'
//@[343:343]         "name": "Standard_LRS"
  }
  kind: 'StorageV2'
//@[345:345]       "kind": "StorageV2",
}]

// storage account loop with index
@sys.description('this is just a storage account loop with index')
resource storageResourcesWithIndex 'Microsoft.Storage/storageAccounts@2019-06-01' = [for (account, i) in storageAccounts: {
//@[350:366]       "copy": {
  name: '${account.name}${i}'
  location: account.location
//@[358:358]       "location": "[variables('storageAccounts')[copyIndex()].location]",
  sku: {
//@[359:361]       "sku": {
    name: 'Standard_LRS'
//@[360:360]         "name": "Standard_LRS"
  }
  kind: 'StorageV2'
//@[362:362]       "kind": "StorageV2",
}]

// basic nested loop
@sys.description('this is just a basic nested loop')
resource vnet 'Microsoft.Network/virtualNetworks@2020-06-01' = [for i in range(0, 3): {
//@[367:389]       "copy": {
  name: 'vnet-${i}'
  properties: {
//@[375:385]       "properties": {
    subnets: [for j in range(0, 4): {
//@[377:383]             "name": "subnets",
      // #completionTest(0,1,2,3,4,5) -> subnetIdAndProperties
     
      // #completionTest(6) -> subnetIdAndPropertiesNoColon
      name: 'subnet-${i}-${j}'
//@[381:381]               "name": "[format('subnet-{0}-{1}', range(0, 3)[copyIndex()], range(0, 4)[copyIndex('subnets')])]"
    }]
  }
}]

// duplicate identifiers within the loop are allowed
resource duplicateIdentifiersWithinLoop 'Microsoft.Network/virtualNetworks@2020-06-01' = [for i in range(0, 3): {
//@[390:409]       "copy": {
  name: 'vnet-${i}'
  properties: {
//@[398:408]       "properties": {
    subnets: [for i in range(0, 4): {
//@[400:406]             "name": "subnets",
      name: 'subnet-${i}-${i}'
//@[404:404]               "name": "[format('subnet-{0}-{1}', range(0, 4)[copyIndex('subnets')], range(0, 4)[copyIndex('subnets')])]"
    }]
  }
}]

// duplicate identifers in global and single loop scope are allowed (inner variable hides the outer)
var canHaveDuplicatesAcrossScopes = 'hello'
//@[54:54]     "canHaveDuplicatesAcrossScopes": "hello",
resource duplicateInGlobalAndOneLoop 'Microsoft.Network/virtualNetworks@2020-06-01' = [for canHaveDuplicatesAcrossScopes in range(0, 3): {
//@[410:429]       "copy": {
  name: 'vnet-${canHaveDuplicatesAcrossScopes}'
  properties: {
//@[418:428]       "properties": {
    subnets: [for i in range(0, 4): {
//@[420:426]             "name": "subnets",
      name: 'subnet-${i}-${i}'
//@[424:424]               "name": "[format('subnet-{0}-{1}', range(0, 4)[copyIndex('subnets')], range(0, 4)[copyIndex('subnets')])]"
    }]
  }
}]

// duplicate in global and multiple loop scopes are allowed (inner hides the outer)
var duplicatesEverywhere = 'hello'
//@[55:55]     "duplicatesEverywhere": "hello"
resource duplicateInGlobalAndTwoLoops 'Microsoft.Network/virtualNetworks@2020-06-01' = [for duplicatesEverywhere in range(0, 3): {
//@[430:449]       "copy": {
  name: 'vnet-${duplicatesEverywhere}'
  properties: {
//@[438:448]       "properties": {
    subnets: [for duplicatesEverywhere in range(0, 4): {
//@[440:446]             "name": "subnets",
      name: 'subnet-${duplicatesEverywhere}'
//@[444:444]               "name": "[format('subnet-{0}', range(0, 4)[copyIndex('subnets')])]"
    }]
  }
}]

/*
  Scope values created via array access on a resource collection
*/
resource dnsZones 'Microsoft.Network/dnsZones@2018-05-01' = [for zone in range(0,4): {
//@[450:459]       "copy": {
  name: 'zone${zone}'
  location: 'global'
//@[458:458]       "location": "global"
}]

resource locksOnZones 'Microsoft.Authorization/locks@2016-09-01' = [for lock in range(0,2): {
//@[460:475]       "copy": {
  name: 'lock${lock}'
  properties: {
//@[469:471]       "properties": {
    level: 'CanNotDelete'
//@[470:470]         "level": "CanNotDelete"
  }
  scope: dnsZones[lock]
}]

resource moreLocksOnZones 'Microsoft.Authorization/locks@2016-09-01' = [for (lock, i) in range(0,3): {
//@[476:491]       "copy": {
  name: 'another${i}'
  properties: {
//@[485:487]       "properties": {
    level: 'ReadOnly'
//@[486:486]         "level": "ReadOnly"
  }
  scope: dnsZones[i]
}]

resource singleLockOnFirstZone 'Microsoft.Authorization/locks@2016-09-01' = {
//@[492:503]       "type": "Microsoft.Authorization/locks",
  name: 'single-lock'
  properties: {
//@[497:499]       "properties": {
    level: 'ReadOnly'
//@[498:498]         "level": "ReadOnly"
  }
  scope: dnsZones[0]
}


resource p1_vnet 'Microsoft.Network/virtualNetworks@2020-06-01' = {
//@[504:516]       "type": "Microsoft.Network/virtualNetworks",
  location: resourceGroup().location
//@[508:508]       "location": "[resourceGroup().location]",
  name: 'myVnet'
  properties: {
//@[509:515]       "properties": {
    addressSpace: {
//@[510:514]         "addressSpace": {
      addressPrefixes: [
//@[511:513]           "addressPrefixes": [
        '10.0.0.0/20'
//@[512:512]             "10.0.0.0/20"
      ]
    }
  }
}

resource p1_subnet1 'Microsoft.Network/virtualNetworks/subnets@2020-06-01' = {
//@[517:527]       "type": "Microsoft.Network/virtualNetworks/subnets",
  parent: p1_vnet
  name: 'subnet1'
  properties: {
//@[521:523]       "properties": {
    addressPrefix: '10.0.0.0/24'
//@[522:522]         "addressPrefix": "10.0.0.0/24"
  }
}

resource p1_subnet2 'Microsoft.Network/virtualNetworks/subnets@2020-06-01' = {
//@[528:538]       "type": "Microsoft.Network/virtualNetworks/subnets",
  parent: p1_vnet
  name: 'subnet2'
  properties: {
//@[532:534]       "properties": {
    addressPrefix: '10.0.1.0/24'
//@[533:533]         "addressPrefix": "10.0.1.0/24"
  }
}

output p1_subnet1prefix string = p1_subnet1.properties.addressPrefix
//@[585:588]     "p1_subnet1prefix": {
output p1_subnet1name string = p1_subnet1.name
//@[589:592]     "p1_subnet1name": {
output p1_subnet1type string = p1_subnet1.type
//@[593:596]     "p1_subnet1type": {
output p1_subnet1id string = p1_subnet1.id
//@[597:600]     "p1_subnet1id": {

// parent property with extension resource
resource p2_res1 'Microsoft.Rp1/resource1@2020-06-01' = {
//@[539:543]       "type": "Microsoft.Rp1/resource1",
  name: 'res1'
}

resource p2_res1child 'Microsoft.Rp1/resource1/child1@2020-06-01' = {
//@[544:551]       "type": "Microsoft.Rp1/resource1/child1",
  parent: p2_res1
  name: 'child1'
}

resource p2_res2 'Microsoft.Rp2/resource2@2020-06-01' = {
//@[552:560]       "type": "Microsoft.Rp2/resource2",
  scope: p2_res1child
  name: 'res2'
}

resource p2_res2child 'Microsoft.Rp2/resource2/child2@2020-06-01' = {
//@[561:569]       "type": "Microsoft.Rp2/resource2/child2",
  parent: p2_res2
  name: 'child2'
}

output p2_res2childprop string = p2_res2child.properties.someProp
//@[601:604]     "p2_res2childprop": {
output p2_res2childname string = p2_res2child.name
//@[605:608]     "p2_res2childname": {
output p2_res2childtype string = p2_res2child.type
//@[609:612]     "p2_res2childtype": {
output p2_res2childid string = p2_res2child.id
//@[613:616]     "p2_res2childid": {

// parent property with 'existing' resource
resource p3_res1 'Microsoft.Rp1/resource1@2020-06-01' existing = {
  name: 'res1'
}

resource p3_child1 'Microsoft.Rp1/resource1/child1@2020-06-01' = {
//@[570:574]       "type": "Microsoft.Rp1/resource1/child1",
  parent: p3_res1
  name: 'child1'
}

output p3_res1childprop string = p3_child1.properties.someProp
//@[617:620]     "p3_res1childprop": {
output p3_res1childname string = p3_child1.name
//@[621:624]     "p3_res1childname": {
output p3_res1childtype string = p3_child1.type
//@[625:628]     "p3_res1childtype": {
output p3_res1childid string = p3_child1.id
//@[629:632]     "p3_res1childid": {

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
//@[633:636]     "p4_res1childprop": {
output p4_res1childname string = p4_child1.name
//@[637:640]     "p4_res1childname": {
output p4_res1childtype string = p4_child1.type
//@[641:644]     "p4_res1childtype": {
output p4_res1childid string = p4_child1.id
//@[645:648]     "p4_res1childid": {

