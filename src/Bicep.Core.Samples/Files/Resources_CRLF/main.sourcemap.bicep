
@sys.description('this is basicStorage')
//@[line001->line067]         "description": "this is basicStorage"
resource basicStorage 'Microsoft.Storage/storageAccounts@2019-06-01' = {
//@[line002->line057]     {
//@[line002->line058]       "type": "Microsoft.Storage/storageAccounts",
//@[line002->line059]       "apiVersion": "2019-06-01",
//@[line002->line060]       "name": "basicblobs",
//@[line002->line066]       "metadata": {
//@[line002->line068]       }
//@[line002->line069]     },
  name: 'basicblobs'
  location: 'westus'
//@[line004->line061]       "location": "westus",
  kind: 'BlobStorage'
//@[line005->line062]       "kind": "BlobStorage",
  sku: {
//@[line006->line063]       "sku": {
//@[line006->line065]       },
    name: 'Standard_GRS'
//@[line007->line064]         "name": "Standard_GRS"
  }
}

@sys.description('this is dnsZone')
//@[line011->line076]         "description": "this is dnsZone"
resource dnsZone 'Microsoft.Network/dnszones@2018-05-01' = {
//@[line012->line070]     {
//@[line012->line071]       "type": "Microsoft.Network/dnsZones",
//@[line012->line072]       "apiVersion": "2018-05-01",
//@[line012->line073]       "name": "myZone",
//@[line012->line075]       "metadata": {
//@[line012->line077]       }
//@[line012->line078]     },
  name: 'myZone'
  location: 'global'
//@[line014->line074]       "location": "global",
}

resource myStorageAccount 'Microsoft.Storage/storageAccounts@2017-10-01' = {
//@[line017->line079]     {
//@[line017->line080]       "type": "Microsoft.Storage/storageAccounts",
//@[line017->line081]       "apiVersion": "2017-10-01",
//@[line017->line082]       "name": "myencryptedone",
//@[line017->line103]     },
  name: 'myencryptedone'
  location: 'eastus2'
//@[line019->line083]       "location": "eastus2",
  properties: {
//@[line020->line084]       "properties": {
//@[line020->line098]       },
    supportsHttpsTrafficOnly: true
//@[line021->line085]         "supportsHttpsTrafficOnly": true,
    accessTier: 'Hot'
//@[line022->line086]         "accessTier": "Hot",
    encryption: {
//@[line023->line087]         "encryption": {
//@[line023->line097]         }
      keySource: 'Microsoft.Storage'
//@[line024->line088]           "keySource": "Microsoft.Storage",
      services: {
//@[line025->line089]           "services": {
//@[line025->line096]           }
        blob: {
//@[line026->line090]             "blob": {
//@[line026->line092]             },
          enabled: true
//@[line027->line091]               "enabled": true
        }
        file: {
//@[line029->line093]             "file": {
//@[line029->line095]             }
          enabled: true
//@[line030->line094]               "enabled": true
        }
      }
    }
  }
  kind: 'StorageV2'
//@[line035->line099]       "kind": "StorageV2",
  sku: {
//@[line036->line100]       "sku": {
//@[line036->line102]       }
    name: 'Standard_LRS'
//@[line037->line101]         "name": "Standard_LRS"
  }
}

resource withExpressions 'Microsoft.Storage/storageAccounts@2017-10-01' = {
//@[line041->line104]     {
//@[line041->line105]       "type": "Microsoft.Storage/storageAccounts",
//@[line041->line106]       "apiVersion": "2017-10-01",
//@[line041->line107]       "name": "myencryptedone2",
//@[line041->line128]       "dependsOn": [
//@[line041->line129]         "[resourceId('Microsoft.Storage/storageAccounts', 'myencryptedone')]"
//@[line041->line130]       ]
//@[line041->line131]     },
  name: 'myencryptedone2'
  location: 'eastus2'
//@[line043->line108]       "location": "eastus2",
  properties: {
//@[line044->line109]       "properties": {
//@[line044->line123]       },
    supportsHttpsTrafficOnly: !false
//@[line045->line110]         "supportsHttpsTrafficOnly": "[not(false())]",
    accessTier: true ? 'Hot' : 'Cold'
//@[line046->line111]         "accessTier": "[if(true(), 'Hot', 'Cold')]",
    encryption: {
//@[line047->line112]         "encryption": {
//@[line047->line122]         }
      keySource: 'Microsoft.Storage'
//@[line048->line113]           "keySource": "Microsoft.Storage",
      services: {
//@[line049->line114]           "services": {
//@[line049->line121]           }
        blob: {
//@[line050->line115]             "blob": {
//@[line050->line117]             },
          enabled: true || false
//@[line051->line116]               "enabled": "[or(true(), false())]"
        }
        file: {
//@[line053->line118]             "file": {
//@[line053->line120]             }
          enabled: true
//@[line054->line119]               "enabled": true
        }
      }
    }
  }
  kind: 'StorageV2'
//@[line059->line124]       "kind": "StorageV2",
  sku: {
//@[line060->line125]       "sku": {
//@[line060->line127]       },
    name: 'Standard_LRS'
//@[line061->line126]         "name": "Standard_LRS"
  }
  dependsOn: [
    myStorageAccount
  ]
}

param applicationName string = 'to-do-app${uniqueString(resourceGroup().id)}'
//@[line068->line011]     "applicationName": {
//@[line068->line012]       "type": "string",
//@[line068->line013]       "defaultValue": "[format('to-do-app{0}', uniqueString(resourceGroup().id))]"
//@[line068->line014]     },
var hostingPlanName = applicationName // why not just use the param directly?
//@[line069->line033]     "hostingPlanName": "[parameters('applicationName')]",

param appServicePlanTier string
//@[line071->line015]     "appServicePlanTier": {
//@[line071->line016]       "type": "string"
//@[line071->line017]     },
param appServicePlanInstances int
//@[line072->line018]     "appServicePlanInstances": {
//@[line072->line019]       "type": "int"
//@[line072->line020]     },

var location = resourceGroup().location
//@[line074->line034]     "location": "[resourceGroup().location]",

resource farm 'Microsoft.Web/serverFarms@2019-08-01' = {
//@[line076->line132]     {
//@[line076->line133]       "type": "Microsoft.Web/serverfarms",
//@[line076->line134]       "apiVersion": "2019-08-01",
//@[line076->line135]       "name": "[variables('hostingPlanName')]",
//@[line076->line144]     },
  // dependsOn: resourceId('Microsoft.DocumentDB/databaseAccounts', cosmosAccountName)
  name: hostingPlanName
  location: location
//@[line079->line136]       "location": "[variables('location')]",
  sku: {
//@[line080->line137]       "sku": {
//@[line080->line140]       },
    name: appServicePlanTier
//@[line081->line138]         "name": "[parameters('appServicePlanTier')]",
    capacity: appServicePlanInstances
//@[line082->line139]         "capacity": "[parameters('appServicePlanInstances')]"
  }
  properties: {
//@[line084->line141]       "properties": {
//@[line084->line143]       }
    name: hostingPlanName // just hostingPlanName results in an error
//@[line085->line142]         "name": "[variables('hostingPlanName')]"
  }
}

var cosmosDbResourceId = resourceId('Microsoft.DocumentDB/databaseAccounts', cosmosDb.account)
//@[line089->line035]     "cosmosDbResourceId": "[resourceId('Microsoft.DocumentDB/databaseAccounts', parameters('cosmosDb').account)]",
var cosmosDbRef = reference(cosmosDbResourceId).documentEndpoint

// this variable is not accessed anywhere in this template and depends on a run-time reference
// it should not be present at all in the template output as there is nowhere logical to put it
var cosmosDbEndpoint = cosmosDbRef.documentEndpoint

param webSiteName string
//@[line096->line021]     "webSiteName": {
//@[line096->line022]       "type": "string"
//@[line096->line023]     },
param cosmosDb object
//@[line097->line024]     "cosmosDb": {
//@[line097->line025]       "type": "object"
//@[line097->line026]     },
resource site 'Microsoft.Web/sites@2019-08-01' = {
//@[line098->line145]     {
//@[line098->line146]       "type": "Microsoft.Web/sites",
//@[line098->line147]       "apiVersion": "2019-08-01",
//@[line098->line148]       "name": "[parameters('webSiteName')]",
//@[line098->line172]     },
  name: webSiteName
  location: location
//@[line100->line149]       "location": "[variables('location')]",
  properties: {
//@[line101->line150]       "properties": {
//@[line101->line171]       }
    // not yet supported // serverFarmId: farm.id
    siteConfig: {
//@[line103->line151]         "siteConfig": {
//@[line103->line170]         }
      appSettings: [
//@[line104->line152]           "appSettings": [
//@[line104->line169]           ]
        {
//@[line105->line153]             {
//@[line105->line156]             },
          name: 'CosmosDb:Account'
//@[line106->line154]               "name": "CosmosDb:Account",
          value: reference(cosmosDbResourceId).documentEndpoint
//@[line107->line155]               "value": "[reference(variables('cosmosDbResourceId')).documentEndpoint]"
        }
        {
//@[line109->line157]             {
//@[line109->line160]             },
          name: 'CosmosDb:Key'
//@[line110->line158]               "name": "CosmosDb:Key",
          value: listKeys(cosmosDbResourceId, '2020-04-01').primaryMasterKey
//@[line111->line159]               "value": "[listKeys(variables('cosmosDbResourceId'), '2020-04-01').primaryMasterKey]"
        }
        {
//@[line113->line161]             {
//@[line113->line164]             },
          name: 'CosmosDb:DatabaseName'
//@[line114->line162]               "name": "CosmosDb:DatabaseName",
          value: cosmosDb.databaseName
//@[line115->line163]               "value": "[parameters('cosmosDb').databaseName]"
        }
        {
//@[line117->line165]             {
//@[line117->line168]             }
          name: 'CosmosDb:ContainerName'
//@[line118->line166]               "name": "CosmosDb:ContainerName",
          value: cosmosDb.containerName
//@[line119->line167]               "value": "[parameters('cosmosDb').containerName]"
        }
      ]
    }
  }
}

var _siteApiVersion = site.apiVersion
//@[line126->line036]     "_siteApiVersion": "2019-08-01",
var _siteType = site.type
//@[line127->line037]     "_siteType": "Microsoft.Web/sites",

output siteApiVersion string = site.apiVersion
//@[line129->line576]     "siteApiVersion": {
//@[line129->line577]       "type": "string",
//@[line129->line578]       "value": "2019-08-01"
//@[line129->line579]     },
output siteType string = site.type
//@[line130->line580]     "siteType": {
//@[line130->line581]       "type": "string",
//@[line130->line582]       "value": "Microsoft.Web/sites"
//@[line130->line583]     },

resource nested 'Microsoft.Resources/deployments@2019-10-01' = {
//@[line132->line173]     {
//@[line132->line174]       "type": "Microsoft.Resources/deployments",
//@[line132->line175]       "apiVersion": "2019-10-01",
//@[line132->line176]       "name": "nestedTemplate1",
//@[line132->line185]     },
  name: 'nestedTemplate1'
  properties: {
//@[line134->line177]       "properties": {
//@[line134->line184]       }
    mode: 'Incremental'
//@[line135->line178]         "mode": "Incremental",
    template: {
//@[line136->line179]         "template": {
//@[line136->line183]         }
      // string key value
      '$schema': 'https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#'
//@[line138->line180]           "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
      contentVersion: '1.0.0.0'
//@[line139->line181]           "contentVersion": "1.0.0.0",
      resources: [
//@[line140->line182]           "resources": []
      ]
    }
  }
}

// should be able to access the read only properties
resource accessingReadOnlyProperties 'Microsoft.Foo/foos@2019-10-01' = {
//@[line147->line186]     {
//@[line147->line187]       "type": "Microsoft.Foo/foos",
//@[line147->line188]       "apiVersion": "2019-10-01",
//@[line147->line189]       "name": "nestedTemplate1",
//@[line147->line197]       "dependsOn": [
//@[line147->line198]         "[resourceId('Microsoft.Resources/deployments', 'nestedTemplate1')]"
//@[line147->line199]       ]
//@[line147->line200]     },
  name: 'nestedTemplate1'
  properties: {
//@[line149->line190]       "properties": {
//@[line149->line196]       },
    otherId: nested.id
//@[line150->line191]         "otherId": "[resourceId('Microsoft.Resources/deployments', 'nestedTemplate1')]",
    otherName: nested.name
//@[line151->line192]         "otherName": "nestedTemplate1",
    otherVersion: nested.apiVersion
//@[line152->line193]         "otherVersion": "2019-10-01",
    otherType: nested.type
//@[line153->line194]         "otherType": "Microsoft.Resources/deployments",

    otherThings: nested.properties.mode
//@[line155->line195]         "otherThings": "[reference(resourceId('Microsoft.Resources/deployments', 'nestedTemplate1'), '2019-10-01').mode]"
  }
}

resource resourceA 'My.Rp/typeA@2020-01-01' = {
//@[line159->line201]     {
//@[line159->line202]       "type": "My.Rp/typeA",
//@[line159->line203]       "apiVersion": "2020-01-01",
//@[line159->line204]       "name": "resourceA"
//@[line159->line205]     },
  name: 'resourceA'
}

resource resourceB 'My.Rp/typeA/typeB@2020-01-01' = {
//@[line163->line206]     {
//@[line163->line207]       "type": "My.Rp/typeA/typeB",
//@[line163->line208]       "apiVersion": "2020-01-01",
//@[line163->line209]       "name": "[format('{0}/myName', 'resourceA')]",
//@[line163->line210]       "dependsOn": [
//@[line163->line211]         "[resourceId('My.Rp/typeA', 'resourceA')]"
//@[line163->line212]       ]
//@[line163->line213]     },
  name: '${resourceA.name}/myName'
}

resource resourceC 'My.Rp/typeA/typeB@2020-01-01' = {
//@[line167->line214]     {
//@[line167->line215]       "type": "My.Rp/typeA/typeB",
//@[line167->line216]       "apiVersion": "2020-01-01",
//@[line167->line217]       "name": "[format('{0}/myName', 'resourceA')]",
//@[line167->line225]       "dependsOn": [
//@[line167->line226]         "[resourceId('My.Rp/typeA', 'resourceA')]",
//@[line167->line227]         "[resourceId('My.Rp/typeA/typeB', split(format('{0}/myName', 'resourceA'), '/')[0], split(format('{0}/myName', 'resourceA'), '/')[1])]"
//@[line167->line228]       ]
//@[line167->line229]     },
  name: '${resourceA.name}/myName'
  properties: {
//@[line169->line218]       "properties": {
//@[line169->line224]       },
    aId: resourceA.id
//@[line170->line219]         "aId": "[resourceId('My.Rp/typeA', 'resourceA')]",
    aType: resourceA.type
//@[line171->line220]         "aType": "My.Rp/typeA",
    aName: resourceA.name
//@[line172->line221]         "aName": "resourceA",
    aApiVersion: resourceA.apiVersion
//@[line173->line222]         "aApiVersion": "2020-01-01",
    bProperties: resourceB.properties
//@[line174->line223]         "bProperties": "[reference(resourceId('My.Rp/typeA/typeB', split(format('{0}/myName', 'resourceA'), '/')[0], split(format('{0}/myName', 'resourceA'), '/')[1]), '2020-01-01')]"
  }
}

var varARuntime = {
  bId: resourceB.id
//@[line179->line237]             "bId": "[resourceId('My.Rp/typeA/typeB', split(format('{0}/myName', 'resourceA'), '/')[0], split(format('{0}/myName', 'resourceA'), '/')[1])]",
  bType: resourceB.type
//@[line180->line238]             "bType": "My.Rp/typeA/typeB",
  bName: resourceB.name
//@[line181->line239]             "bName": "[format('{0}/myName', 'resourceA')]",
  bApiVersion: resourceB.apiVersion
//@[line182->line240]             "bApiVersion": "2020-01-01",
  aKind: resourceA.kind
//@[line183->line241]             "aKind": "[reference(resourceId('My.Rp/typeA', 'resourceA'), '2020-01-01', 'full').kind]"
}

var varBRuntime = [
  varARuntime
//@[line187->line236]           {
//@[line187->line242]           }
]

var resourceCRef = {
//@[line190->line038]     "resourceCRef": {
//@[line190->line040]     },
  id: resourceC.id
//@[line191->line039]       "id": "[resourceId('My.Rp/typeA/typeB', split(format('{0}/myName', 'resourceA'), '/')[0], split(format('{0}/myName', 'resourceA'), '/')[1])]"
}
var setResourceCRef = true
//@[line193->line041]     "setResourceCRef": true,

resource resourceD 'My.Rp/typeD@2020-01-01' = {
//@[line195->line230]     {
//@[line195->line231]       "type": "My.Rp/typeD",
//@[line195->line232]       "apiVersion": "2020-01-01",
//@[line195->line233]       "name": "constant",
//@[line195->line246]       "dependsOn": [
//@[line195->line247]         "[resourceId('My.Rp/typeA', 'resourceA')]",
//@[line195->line248]         "[resourceId('My.Rp/typeA/typeB', split(format('{0}/myName', 'resourceA'), '/')[0], split(format('{0}/myName', 'resourceA'), '/')[1])]",
//@[line195->line249]         "[resourceId('My.Rp/typeA/typeB', split(format('{0}/myName', 'resourceA'), '/')[0], split(format('{0}/myName', 'resourceA'), '/')[1])]"
//@[line195->line250]       ]
//@[line195->line251]     },
  name: 'constant'
  properties: {
//@[line197->line234]       "properties": {
//@[line197->line245]       },
    runtime: varBRuntime
//@[line198->line235]         "runtime": [
//@[line198->line243]         ],
    // repro for https://github.com/Azure/bicep/issues/316
    repro316: setResourceCRef ? resourceCRef : null
//@[line200->line244]         "repro316": "[if(variables('setResourceCRef'), variables('resourceCRef'), null())]"
  }
}

var myInterpKey = 'abc'
//@[line204->line042]     "myInterpKey": "abc",
resource resourceWithInterp 'My.Rp/interp@2020-01-01' = {
//@[line205->line252]     {
//@[line205->line253]       "type": "My.Rp/interp",
//@[line205->line254]       "apiVersion": "2020-01-01",
//@[line205->line255]       "name": "interpTest",
//@[line205->line261]     },
  name: 'interpTest'
  properties: {
//@[line207->line256]       "properties": {
//@[line207->line260]       }
    '${myInterpKey}': 1
//@[line208->line257]         "[format('{0}', variables('myInterpKey'))]": 1,
    'abc${myInterpKey}def': 2
//@[line209->line258]         "[format('abc{0}def', variables('myInterpKey'))]": 2,
    '${myInterpKey}abc${myInterpKey}': 3
//@[line210->line259]         "[format('{0}abc{1}', variables('myInterpKey'), variables('myInterpKey'))]": 3
  }
}

resource resourceWithEscaping 'My.Rp/mockResource@2020-01-01' = {
//@[line214->line262]     {
//@[line214->line263]       "type": "My.Rp/mockResource",
//@[line214->line264]       "apiVersion": "2020-01-01",
//@[line214->line265]       "name": "test",
//@[line214->line269]     },
  name: 'test'
  properties: {
//@[line216->line266]       "properties": {
//@[line216->line268]       }
    // both key and value should be escaped in template output
    '[resourceGroup().location]': '[resourceGroup().location]'
//@[line218->line267]         "[[resourceGroup().location]": "[[resourceGroup().location]"
  }
}

param shouldDeployVm bool = true
//@[line222->line027]     "shouldDeployVm": {
//@[line222->line028]       "type": "bool",
//@[line222->line029]       "defaultValue": true
//@[line222->line030]     }

@sys.description('this is vmWithCondition')
//@[line224->line284]         "description": "this is vmWithCondition"
resource vmWithCondition 'Microsoft.Compute/virtualMachines@2020-06-01' = if (shouldDeployVm) {
//@[line225->line270]     {
//@[line225->line271]       "condition": "[parameters('shouldDeployVm')]",
//@[line225->line272]       "type": "Microsoft.Compute/virtualMachines",
//@[line225->line273]       "apiVersion": "2020-06-01",
//@[line225->line274]       "name": "vmName",
//@[line225->line283]       "metadata": {
//@[line225->line285]       }
//@[line225->line286]     },
  name: 'vmName'
  location: 'westus'
//@[line227->line275]       "location": "westus",
  properties: {
//@[line228->line276]       "properties": {
//@[line228->line282]       },
    osProfile: {
//@[line229->line277]         "osProfile": {
//@[line229->line281]         }
      windowsConfiguration: {
//@[line230->line278]           "windowsConfiguration": {
//@[line230->line280]           }
        enableAutomaticUpdates: true
//@[line231->line279]             "enableAutomaticUpdates": true
      }
    }
  }
}

resource extension1 'My.Rp/extensionResource@2020-12-01' = {
//@[line237->line287]     {
//@[line237->line288]       "type": "My.Rp/extensionResource",
//@[line237->line289]       "apiVersion": "2020-12-01",
//@[line237->line290]       "scope": "[format('Microsoft.Compute/virtualMachines/{0}', 'vmName')]",
//@[line237->line291]       "name": "extension",
//@[line237->line292]       "dependsOn": [
//@[line237->line293]         "[resourceId('Microsoft.Compute/virtualMachines', 'vmName')]"
//@[line237->line294]       ]
//@[line237->line295]     },
  name: 'extension'
  scope: vmWithCondition
}

resource extension2 'My.Rp/extensionResource@2020-12-01' = {
//@[line242->line296]     {
//@[line242->line297]       "type": "My.Rp/extensionResource",
//@[line242->line298]       "apiVersion": "2020-12-01",
//@[line242->line299]       "scope": "[extensionResourceId(format('Microsoft.Compute/virtualMachines/{0}', 'vmName'), 'My.Rp/extensionResource', 'extension')]",
//@[line242->line300]       "name": "extension",
//@[line242->line301]       "dependsOn": [
//@[line242->line302]         "[extensionResourceId(resourceId('Microsoft.Compute/virtualMachines', 'vmName'), 'My.Rp/extensionResource', 'extension')]"
//@[line242->line303]       ]
//@[line242->line304]     },
  name: 'extension'
  scope: extension1
}

resource extensionDependencies 'My.Rp/mockResource@2020-01-01' = {
//@[line247->line305]     {
//@[line247->line306]       "type": "My.Rp/mockResource",
//@[line247->line307]       "apiVersion": "2020-01-01",
//@[line247->line308]       "name": "extensionDependencies",
//@[line247->line317]       "dependsOn": [
//@[line247->line318]         "[extensionResourceId(resourceId('Microsoft.Compute/virtualMachines', 'vmName'), 'My.Rp/extensionResource', 'extension')]",
//@[line247->line319]         "[extensionResourceId(extensionResourceId(resourceId('Microsoft.Compute/virtualMachines', 'vmName'), 'My.Rp/extensionResource', 'extension'), 'My.Rp/extensionResource', 'extension')]",
//@[line247->line320]         "[resourceId('Microsoft.Compute/virtualMachines', 'vmName')]"
//@[line247->line321]       ]
//@[line247->line322]     },
  name: 'extensionDependencies'
  properties: {
//@[line249->line309]       "properties": {
//@[line249->line316]       },
    res1: vmWithCondition.id
//@[line250->line310]         "res1": "[resourceId('Microsoft.Compute/virtualMachines', 'vmName')]",
    res1runtime: vmWithCondition.properties.something
//@[line251->line311]         "res1runtime": "[reference(resourceId('Microsoft.Compute/virtualMachines', 'vmName'), '2020-06-01').something]",
    res2: extension1.id
//@[line252->line312]         "res2": "[extensionResourceId(resourceId('Microsoft.Compute/virtualMachines', 'vmName'), 'My.Rp/extensionResource', 'extension')]",
    res2runtime: extension1.properties.something
//@[line253->line313]         "res2runtime": "[reference(extensionResourceId(resourceId('Microsoft.Compute/virtualMachines', 'vmName'), 'My.Rp/extensionResource', 'extension'), '2020-12-01').something]",
    res3: extension2.id
//@[line254->line314]         "res3": "[extensionResourceId(extensionResourceId(resourceId('Microsoft.Compute/virtualMachines', 'vmName'), 'My.Rp/extensionResource', 'extension'), 'My.Rp/extensionResource', 'extension')]",
    res3runtime: extension2.properties.something
//@[line255->line315]         "res3runtime": "[reference(extensionResourceId(extensionResourceId(resourceId('Microsoft.Compute/virtualMachines', 'vmName'), 'My.Rp/extensionResource', 'extension'), 'My.Rp/extensionResource', 'extension'), '2020-12-01').something]"
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
//@[line270->line323]     {
//@[line270->line324]       "type": "My.Rp/extensionResource",
//@[line270->line325]       "apiVersion": "2020-12-01",
//@[line270->line326]       "scope": "[extensionResourceId(extensionResourceId(format('Microsoft.Compute/virtualMachines/{0}', 'vmName'), 'My.Rp/extensionResource', 'extension'), 'Mock.Rp/existingExtensionResource', 'existing1')]",
//@[line270->line327]       "name": "extension3",
//@[line270->line328]       "dependsOn": [
//@[line270->line329]         "[extensionResourceId(resourceId('Microsoft.Compute/virtualMachines', 'vmName'), 'My.Rp/extensionResource', 'extension')]"
//@[line270->line330]       ]
//@[line270->line331]     },
  name: 'extension3'
  scope: existing1
}

/*
  valid loop cases
*/ 
var storageAccounts = [
//@[line278->line043]     "storageAccounts": [
//@[line278->line052]     ],
  {
//@[line279->line044]       {
//@[line279->line047]       },
    name: 'one'
//@[line280->line045]         "name": "one",
    location: 'eastus2'
//@[line281->line046]         "location": "eastus2"
  }
  {
//@[line283->line048]       {
//@[line283->line051]       }
    name: 'two'
//@[line284->line049]         "name": "two",
    location: 'westus'
//@[line285->line050]         "location": "westus"
  }
]

// just a storage account loop
@sys.description('this is just a storage account loop')
//@[line290->line346]         "description": "this is just a storage account loop"
resource storageResources 'Microsoft.Storage/storageAccounts@2019-06-01' = [for account in storageAccounts: {
//@[line291->line332]     {
//@[line291->line333]       "copy": {
//@[line291->line334]         "name": "storageResources",
//@[line291->line335]         "count": "[length(variables('storageAccounts'))]"
//@[line291->line336]       },
//@[line291->line337]       "type": "Microsoft.Storage/storageAccounts",
//@[line291->line338]       "apiVersion": "2019-06-01",
//@[line291->line339]       "name": "[variables('storageAccounts')[copyIndex()].name]",
//@[line291->line345]       "metadata": {
//@[line291->line347]       }
//@[line291->line348]     },
  name: account.name
  location: account.location
//@[line293->line340]       "location": "[variables('storageAccounts')[copyIndex()].location]",
  sku: {
//@[line294->line341]       "sku": {
//@[line294->line343]       },
    name: 'Standard_LRS'
//@[line295->line342]         "name": "Standard_LRS"
  }
  kind: 'StorageV2'
//@[line297->line344]       "kind": "StorageV2",
}]

// storage account loop with index
@sys.description('this is just a storage account loop with index')
//@[line301->line363]         "description": "this is just a storage account loop with index"
resource storageResourcesWithIndex 'Microsoft.Storage/storageAccounts@2019-06-01' = [for (account, i) in storageAccounts: {
//@[line302->line349]     {
//@[line302->line350]       "copy": {
//@[line302->line351]         "name": "storageResourcesWithIndex",
//@[line302->line352]         "count": "[length(variables('storageAccounts'))]"
//@[line302->line353]       },
//@[line302->line354]       "type": "Microsoft.Storage/storageAccounts",
//@[line302->line355]       "apiVersion": "2019-06-01",
//@[line302->line356]       "name": "[format('{0}{1}', variables('storageAccounts')[copyIndex()].name, copyIndex())]",
//@[line302->line362]       "metadata": {
//@[line302->line364]       }
//@[line302->line365]     },
  name: '${account.name}${i}'
  location: account.location
//@[line304->line357]       "location": "[variables('storageAccounts')[copyIndex()].location]",
  sku: {
//@[line305->line358]       "sku": {
//@[line305->line360]       },
    name: 'Standard_LRS'
//@[line306->line359]         "name": "Standard_LRS"
  }
  kind: 'StorageV2'
//@[line308->line361]       "kind": "StorageV2",
}]

// basic nested loop
@sys.description('this is just a basic nested loop')
//@[line312->line386]         "description": "this is just a basic nested loop"
resource vnet 'Microsoft.Network/virtualNetworks@2020-06-01' = [for i in range(0, 3): {
//@[line313->line366]     {
//@[line313->line367]       "copy": {
//@[line313->line368]         "name": "vnet",
//@[line313->line369]         "count": "[length(range(0, 3))]"
//@[line313->line370]       },
//@[line313->line371]       "type": "Microsoft.Network/virtualNetworks",
//@[line313->line372]       "apiVersion": "2020-06-01",
//@[line313->line373]       "name": "[format('vnet-{0}', range(0, 3)[copyIndex()])]",
//@[line313->line385]       "metadata": {
//@[line313->line387]       }
//@[line313->line388]     },
  name: 'vnet-${i}'
  properties: {
//@[line315->line374]       "properties": {
//@[line315->line375]         "copy": [
//@[line315->line383]         ]
//@[line315->line384]       },
    subnets: [for j in range(0, 4): {
//@[line316->line376]           {
//@[line316->line377]             "name": "subnets",
//@[line316->line378]             "count": "[length(range(0, 4))]",
//@[line316->line379]             "input": {
//@[line316->line381]             }
//@[line316->line382]           }
      // #completionTest(0,1,2,3,4,5) -> subnetIdAndProperties
     
      // #completionTest(6) -> subnetIdAndPropertiesNoColon
      name: 'subnet-${i}-${j}'
//@[line320->line380]               "name": "[format('subnet-{0}-{1}', range(0, 3)[copyIndex()], range(0, 4)[copyIndex('subnets')])]"
    }]
  }
}]

// duplicate identifiers within the loop are allowed
resource duplicateIdentifiersWithinLoop 'Microsoft.Network/virtualNetworks@2020-06-01' = [for i in range(0, 3): {
//@[line326->line389]     {
//@[line326->line390]       "copy": {
//@[line326->line391]         "name": "duplicateIdentifiersWithinLoop",
//@[line326->line392]         "count": "[length(range(0, 3))]"
//@[line326->line393]       },
//@[line326->line394]       "type": "Microsoft.Network/virtualNetworks",
//@[line326->line395]       "apiVersion": "2020-06-01",
//@[line326->line396]       "name": "[format('vnet-{0}', range(0, 3)[copyIndex()])]",
//@[line326->line408]     },
  name: 'vnet-${i}'
  properties: {
//@[line328->line397]       "properties": {
//@[line328->line398]         "copy": [
//@[line328->line406]         ]
//@[line328->line407]       }
    subnets: [for i in range(0, 4): {
//@[line329->line399]           {
//@[line329->line400]             "name": "subnets",
//@[line329->line401]             "count": "[length(range(0, 4))]",
//@[line329->line402]             "input": {
//@[line329->line404]             }
//@[line329->line405]           }
      name: 'subnet-${i}-${i}'
//@[line330->line403]               "name": "[format('subnet-{0}-{1}', range(0, 4)[copyIndex('subnets')], range(0, 4)[copyIndex('subnets')])]"
    }]
  }
}]

// duplicate identifers in global and single loop scope are allowed (inner variable hides the outer)
var canHaveDuplicatesAcrossScopes = 'hello'
//@[line336->line053]     "canHaveDuplicatesAcrossScopes": "hello",
resource duplicateInGlobalAndOneLoop 'Microsoft.Network/virtualNetworks@2020-06-01' = [for canHaveDuplicatesAcrossScopes in range(0, 3): {
//@[line337->line409]     {
//@[line337->line410]       "copy": {
//@[line337->line411]         "name": "duplicateInGlobalAndOneLoop",
//@[line337->line412]         "count": "[length(range(0, 3))]"
//@[line337->line413]       },
//@[line337->line414]       "type": "Microsoft.Network/virtualNetworks",
//@[line337->line415]       "apiVersion": "2020-06-01",
//@[line337->line416]       "name": "[format('vnet-{0}', range(0, 3)[copyIndex()])]",
//@[line337->line428]     },
  name: 'vnet-${canHaveDuplicatesAcrossScopes}'
  properties: {
//@[line339->line417]       "properties": {
//@[line339->line418]         "copy": [
//@[line339->line426]         ]
//@[line339->line427]       }
    subnets: [for i in range(0, 4): {
//@[line340->line419]           {
//@[line340->line420]             "name": "subnets",
//@[line340->line421]             "count": "[length(range(0, 4))]",
//@[line340->line422]             "input": {
//@[line340->line424]             }
//@[line340->line425]           }
      name: 'subnet-${i}-${i}'
//@[line341->line423]               "name": "[format('subnet-{0}-{1}', range(0, 4)[copyIndex('subnets')], range(0, 4)[copyIndex('subnets')])]"
    }]
  }
}]

// duplicate in global and multiple loop scopes are allowed (inner hides the outer)
var duplicatesEverywhere = 'hello'
//@[line347->line054]     "duplicatesEverywhere": "hello"
resource duplicateInGlobalAndTwoLoops 'Microsoft.Network/virtualNetworks@2020-06-01' = [for duplicatesEverywhere in range(0, 3): {
//@[line348->line429]     {
//@[line348->line430]       "copy": {
//@[line348->line431]         "name": "duplicateInGlobalAndTwoLoops",
//@[line348->line432]         "count": "[length(range(0, 3))]"
//@[line348->line433]       },
//@[line348->line434]       "type": "Microsoft.Network/virtualNetworks",
//@[line348->line435]       "apiVersion": "2020-06-01",
//@[line348->line436]       "name": "[format('vnet-{0}', range(0, 3)[copyIndex()])]",
//@[line348->line448]     },
  name: 'vnet-${duplicatesEverywhere}'
  properties: {
//@[line350->line437]       "properties": {
//@[line350->line438]         "copy": [
//@[line350->line446]         ]
//@[line350->line447]       }
    subnets: [for duplicatesEverywhere in range(0, 4): {
//@[line351->line439]           {
//@[line351->line440]             "name": "subnets",
//@[line351->line441]             "count": "[length(range(0, 4))]",
//@[line351->line442]             "input": {
//@[line351->line444]             }
//@[line351->line445]           }
      name: 'subnet-${duplicatesEverywhere}'
//@[line352->line443]               "name": "[format('subnet-{0}', range(0, 4)[copyIndex('subnets')])]"
    }]
  }
}]

/*
  Scope values created via array access on a resource collection
*/
resource dnsZones 'Microsoft.Network/dnsZones@2018-05-01' = [for zone in range(0,4): {
//@[line360->line449]     {
//@[line360->line450]       "copy": {
//@[line360->line451]         "name": "dnsZones",
//@[line360->line452]         "count": "[length(range(0, 4))]"
//@[line360->line453]       },
//@[line360->line454]       "type": "Microsoft.Network/dnsZones",
//@[line360->line455]       "apiVersion": "2018-05-01",
//@[line360->line456]       "name": "[format('zone{0}', range(0, 4)[copyIndex()])]",
//@[line360->line458]     },
  name: 'zone${zone}'
  location: 'global'
//@[line362->line457]       "location": "global"
}]

resource locksOnZones 'Microsoft.Authorization/locks@2016-09-01' = [for lock in range(0,2): {
//@[line365->line459]     {
//@[line365->line460]       "copy": {
//@[line365->line461]         "name": "locksOnZones",
//@[line365->line462]         "count": "[length(range(0, 2))]"
//@[line365->line463]       },
//@[line365->line464]       "type": "Microsoft.Authorization/locks",
//@[line365->line465]       "apiVersion": "2016-09-01",
//@[line365->line466]       "scope": "[format('Microsoft.Network/dnsZones/{0}', format('zone{0}', range(0, 4)[range(0, 2)[copyIndex()]]))]",
//@[line365->line467]       "name": "[format('lock{0}', range(0, 2)[copyIndex()])]",
//@[line365->line471]       "dependsOn": [
//@[line365->line472]         "[resourceId('Microsoft.Network/dnsZones', format('zone{0}', range(0, 4)[range(0, 2)[copyIndex()]]))]"
//@[line365->line473]       ]
//@[line365->line474]     },
  name: 'lock${lock}'
  properties: {
//@[line367->line468]       "properties": {
//@[line367->line470]       },
    level: 'CanNotDelete'
//@[line368->line469]         "level": "CanNotDelete"
  }
  scope: dnsZones[lock]
}]

resource moreLocksOnZones 'Microsoft.Authorization/locks@2016-09-01' = [for (lock, i) in range(0,3): {
//@[line373->line475]     {
//@[line373->line476]       "copy": {
//@[line373->line477]         "name": "moreLocksOnZones",
//@[line373->line478]         "count": "[length(range(0, 3))]"
//@[line373->line479]       },
//@[line373->line480]       "type": "Microsoft.Authorization/locks",
//@[line373->line481]       "apiVersion": "2016-09-01",
//@[line373->line482]       "scope": "[format('Microsoft.Network/dnsZones/{0}', format('zone{0}', range(0, 4)[copyIndex()]))]",
//@[line373->line483]       "name": "[format('another{0}', copyIndex())]",
//@[line373->line487]       "dependsOn": [
//@[line373->line488]         "[resourceId('Microsoft.Network/dnsZones', format('zone{0}', range(0, 4)[copyIndex()]))]"
//@[line373->line489]       ]
//@[line373->line490]     },
  name: 'another${i}'
  properties: {
//@[line375->line484]       "properties": {
//@[line375->line486]       },
    level: 'ReadOnly'
//@[line376->line485]         "level": "ReadOnly"
  }
  scope: dnsZones[i]
}]

resource singleLockOnFirstZone 'Microsoft.Authorization/locks@2016-09-01' = {
//@[line381->line491]     {
//@[line381->line492]       "type": "Microsoft.Authorization/locks",
//@[line381->line493]       "apiVersion": "2016-09-01",
//@[line381->line494]       "scope": "[format('Microsoft.Network/dnsZones/{0}', format('zone{0}', range(0, 4)[0]))]",
//@[line381->line495]       "name": "single-lock",
//@[line381->line499]       "dependsOn": [
//@[line381->line500]         "[resourceId('Microsoft.Network/dnsZones', format('zone{0}', range(0, 4)[0]))]"
//@[line381->line501]       ]
//@[line381->line502]     },
  name: 'single-lock'
  properties: {
//@[line383->line496]       "properties": {
//@[line383->line498]       },
    level: 'ReadOnly'
//@[line384->line497]         "level": "ReadOnly"
  }
  scope: dnsZones[0]
}


resource p1_vnet 'Microsoft.Network/virtualNetworks@2020-06-01' = {
//@[line390->line503]     {
//@[line390->line504]       "type": "Microsoft.Network/virtualNetworks",
//@[line390->line505]       "apiVersion": "2020-06-01",
//@[line390->line506]       "name": "myVnet",
//@[line390->line515]     },
  location: resourceGroup().location
//@[line391->line507]       "location": "[resourceGroup().location]",
  name: 'myVnet'
  properties: {
//@[line393->line508]       "properties": {
//@[line393->line514]       }
    addressSpace: {
//@[line394->line509]         "addressSpace": {
//@[line394->line513]         }
      addressPrefixes: [
//@[line395->line510]           "addressPrefixes": [
//@[line395->line512]           ]
        '10.0.0.0/20'
//@[line396->line511]             "10.0.0.0/20"
      ]
    }
  }
}

resource p1_subnet1 'Microsoft.Network/virtualNetworks/subnets@2020-06-01' = {
//@[line402->line516]     {
//@[line402->line517]       "type": "Microsoft.Network/virtualNetworks/subnets",
//@[line402->line518]       "apiVersion": "2020-06-01",
//@[line402->line519]       "name": "[format('{0}/{1}', 'myVnet', 'subnet1')]",
//@[line402->line523]       "dependsOn": [
//@[line402->line524]         "[resourceId('Microsoft.Network/virtualNetworks', 'myVnet')]"
//@[line402->line525]       ]
//@[line402->line526]     },
  parent: p1_vnet
  name: 'subnet1'
  properties: {
//@[line405->line520]       "properties": {
//@[line405->line522]       },
    addressPrefix: '10.0.0.0/24'
//@[line406->line521]         "addressPrefix": "10.0.0.0/24"
  }
}

resource p1_subnet2 'Microsoft.Network/virtualNetworks/subnets@2020-06-01' = {
//@[line410->line527]     {
//@[line410->line528]       "type": "Microsoft.Network/virtualNetworks/subnets",
//@[line410->line529]       "apiVersion": "2020-06-01",
//@[line410->line530]       "name": "[format('{0}/{1}', 'myVnet', 'subnet2')]",
//@[line410->line534]       "dependsOn": [
//@[line410->line535]         "[resourceId('Microsoft.Network/virtualNetworks', 'myVnet')]"
//@[line410->line536]       ]
//@[line410->line537]     },
  parent: p1_vnet
  name: 'subnet2'
  properties: {
//@[line413->line531]       "properties": {
//@[line413->line533]       },
    addressPrefix: '10.0.1.0/24'
//@[line414->line532]         "addressPrefix": "10.0.1.0/24"
  }
}

output p1_subnet1prefix string = p1_subnet1.properties.addressPrefix
//@[line418->line584]     "p1_subnet1prefix": {
//@[line418->line585]       "type": "string",
//@[line418->line586]       "value": "[reference(resourceId('Microsoft.Network/virtualNetworks/subnets', 'myVnet', 'subnet1'), '2020-06-01').addressPrefix]"
//@[line418->line587]     },
output p1_subnet1name string = p1_subnet1.name
//@[line419->line588]     "p1_subnet1name": {
//@[line419->line589]       "type": "string",
//@[line419->line590]       "value": "subnet1"
//@[line419->line591]     },
output p1_subnet1type string = p1_subnet1.type
//@[line420->line592]     "p1_subnet1type": {
//@[line420->line593]       "type": "string",
//@[line420->line594]       "value": "Microsoft.Network/virtualNetworks/subnets"
//@[line420->line595]     },
output p1_subnet1id string = p1_subnet1.id
//@[line421->line596]     "p1_subnet1id": {
//@[line421->line597]       "type": "string",
//@[line421->line598]       "value": "[resourceId('Microsoft.Network/virtualNetworks/subnets', 'myVnet', 'subnet1')]"
//@[line421->line599]     },

// parent property with extension resource
resource p2_res1 'Microsoft.Rp1/resource1@2020-06-01' = {
//@[line424->line538]     {
//@[line424->line539]       "type": "Microsoft.Rp1/resource1",
//@[line424->line540]       "apiVersion": "2020-06-01",
//@[line424->line541]       "name": "res1"
//@[line424->line542]     },
  name: 'res1'
}

resource p2_res1child 'Microsoft.Rp1/resource1/child1@2020-06-01' = {
//@[line428->line543]     {
//@[line428->line544]       "type": "Microsoft.Rp1/resource1/child1",
//@[line428->line545]       "apiVersion": "2020-06-01",
//@[line428->line546]       "name": "[format('{0}/{1}', 'res1', 'child1')]",
//@[line428->line547]       "dependsOn": [
//@[line428->line548]         "[resourceId('Microsoft.Rp1/resource1', 'res1')]"
//@[line428->line549]       ]
//@[line428->line550]     },
  parent: p2_res1
  name: 'child1'
}

resource p2_res2 'Microsoft.Rp2/resource2@2020-06-01' = {
//@[line433->line551]     {
//@[line433->line552]       "type": "Microsoft.Rp2/resource2",
//@[line433->line553]       "apiVersion": "2020-06-01",
//@[line433->line554]       "scope": "[format('Microsoft.Rp1/resource1/{0}/child1/{1}', 'res1', 'child1')]",
//@[line433->line555]       "name": "res2",
//@[line433->line556]       "dependsOn": [
//@[line433->line557]         "[resourceId('Microsoft.Rp1/resource1/child1', 'res1', 'child1')]"
//@[line433->line558]       ]
//@[line433->line559]     },
  scope: p2_res1child
  name: 'res2'
}

resource p2_res2child 'Microsoft.Rp2/resource2/child2@2020-06-01' = {
//@[line438->line560]     {
//@[line438->line561]       "type": "Microsoft.Rp2/resource2/child2",
//@[line438->line562]       "apiVersion": "2020-06-01",
//@[line438->line563]       "scope": "[format('Microsoft.Rp1/resource1/{0}/child1/{1}', 'res1', 'child1')]",
//@[line438->line564]       "name": "[format('{0}/{1}', 'res2', 'child2')]",
//@[line438->line565]       "dependsOn": [
//@[line438->line566]         "[extensionResourceId(resourceId('Microsoft.Rp1/resource1/child1', 'res1', 'child1'), 'Microsoft.Rp2/resource2', 'res2')]"
//@[line438->line567]       ]
//@[line438->line568]     },
  parent: p2_res2
  name: 'child2'
}

output p2_res2childprop string = p2_res2child.properties.someProp
//@[line443->line600]     "p2_res2childprop": {
//@[line443->line601]       "type": "string",
//@[line443->line602]       "value": "[reference(extensionResourceId(resourceId('Microsoft.Rp1/resource1/child1', 'res1', 'child1'), 'Microsoft.Rp2/resource2/child2', 'res2', 'child2'), '2020-06-01').someProp]"
//@[line443->line603]     },
output p2_res2childname string = p2_res2child.name
//@[line444->line604]     "p2_res2childname": {
//@[line444->line605]       "type": "string",
//@[line444->line606]       "value": "child2"
//@[line444->line607]     },
output p2_res2childtype string = p2_res2child.type
//@[line445->line608]     "p2_res2childtype": {
//@[line445->line609]       "type": "string",
//@[line445->line610]       "value": "Microsoft.Rp2/resource2/child2"
//@[line445->line611]     },
output p2_res2childid string = p2_res2child.id
//@[line446->line612]     "p2_res2childid": {
//@[line446->line613]       "type": "string",
//@[line446->line614]       "value": "[extensionResourceId(resourceId('Microsoft.Rp1/resource1/child1', 'res1', 'child1'), 'Microsoft.Rp2/resource2/child2', 'res2', 'child2')]"
//@[line446->line615]     },

// parent property with 'existing' resource
resource p3_res1 'Microsoft.Rp1/resource1@2020-06-01' existing = {
  name: 'res1'
}

resource p3_child1 'Microsoft.Rp1/resource1/child1@2020-06-01' = {
//@[line453->line569]     {
//@[line453->line570]       "type": "Microsoft.Rp1/resource1/child1",
//@[line453->line571]       "apiVersion": "2020-06-01",
//@[line453->line572]       "name": "[format('{0}/{1}', 'res1', 'child1')]"
//@[line453->line573]     }
  parent: p3_res1
  name: 'child1'
}

output p3_res1childprop string = p3_child1.properties.someProp
//@[line458->line616]     "p3_res1childprop": {
//@[line458->line617]       "type": "string",
//@[line458->line618]       "value": "[reference(resourceId('Microsoft.Rp1/resource1/child1', 'res1', 'child1'), '2020-06-01').someProp]"
//@[line458->line619]     },
output p3_res1childname string = p3_child1.name
//@[line459->line620]     "p3_res1childname": {
//@[line459->line621]       "type": "string",
//@[line459->line622]       "value": "child1"
//@[line459->line623]     },
output p3_res1childtype string = p3_child1.type
//@[line460->line624]     "p3_res1childtype": {
//@[line460->line625]       "type": "string",
//@[line460->line626]       "value": "Microsoft.Rp1/resource1/child1"
//@[line460->line627]     },
output p3_res1childid string = p3_child1.id
//@[line461->line628]     "p3_res1childid": {
//@[line461->line629]       "type": "string",
//@[line461->line630]       "value": "[resourceId('Microsoft.Rp1/resource1/child1', 'res1', 'child1')]"
//@[line461->line631]     },

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
//@[line474->line632]     "p4_res1childprop": {
//@[line474->line633]       "type": "string",
//@[line474->line634]       "value": "[reference(tenantResourceId('Microsoft.Rp1/resource1/child1', 'res1', 'child1'), '2020-06-01').someProp]"
//@[line474->line635]     },
output p4_res1childname string = p4_child1.name
//@[line475->line636]     "p4_res1childname": {
//@[line475->line637]       "type": "string",
//@[line475->line638]       "value": "child1"
//@[line475->line639]     },
output p4_res1childtype string = p4_child1.type
//@[line476->line640]     "p4_res1childtype": {
//@[line476->line641]       "type": "string",
//@[line476->line642]       "value": "Microsoft.Rp1/resource1/child1"
//@[line476->line643]     },
output p4_res1childid string = p4_child1.id
//@[line477->line644]     "p4_res1childid": {
//@[line477->line645]       "type": "string",
//@[line477->line646]       "value": "[tenantResourceId('Microsoft.Rp1/resource1/child1', 'res1', 'child1')]"
//@[line477->line647]     }

