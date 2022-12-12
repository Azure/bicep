targetScope = 'subscription'

resource rg 'Microsoft.Resources/resourceGroups@2020-06-01' = {
//@[line002->line033]     {
//@[line002->line034]       "type": "Microsoft.Resources/resourceGroups",
//@[line002->line035]       "apiVersion": "2020-06-01",
//@[line002->line036]       "name": "adotfrank-rg",
//@[line002->line038]     },
  name: 'adotfrank-rg'
  location: deployment().location
//@[line004->line037]       "location": "[deployment().location]"
}

module appPlanDeploy 'br:mock-registry-one.invalid/demo/plan:v2' = {
//@[line007->line039]     {
//@[line007->line040]       "type": "Microsoft.Resources/deployments",
//@[line007->line041]       "apiVersion": "2020-10-01",
//@[line007->line043]       "resourceGroup": "adotfrank-rg",
//@[line007->line044]       "properties": {
//@[line007->line045]         "expressionEvaluationOptions": {
//@[line007->line046]           "scope": "inner"
//@[line007->line047]         },
//@[line007->line048]         "mode": "Incremental",
//@[line007->line049]         "parameters": {
//@[line007->line050]           "namePrefix": {
//@[line007->line052]           }
//@[line007->line053]         },
//@[line007->line054]         "template": {
//@[line007->line055]           "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@[line007->line056]           "contentVersion": "1.0.0.0",
//@[line007->line057]           "metadata": {
//@[line007->line058]             "_generator": {
//@[line007->line059]               "name": "bicep",
//@[line007->line060]               "version": "dev",
//@[line007->line061]               "templateHash": "15019246960605065046"
//@[line007->line062]             }
//@[line007->line063]           },
//@[line007->line064]           "parameters": {
//@[line007->line065]             "namePrefix": {
//@[line007->line066]               "type": "string"
//@[line007->line067]             },
//@[line007->line068]             "sku": {
//@[line007->line069]               "type": "string",
//@[line007->line070]               "defaultValue": "B1"
//@[line007->line071]             }
//@[line007->line072]           },
//@[line007->line073]           "resources": [
//@[line007->line074]             {
//@[line007->line075]               "type": "Microsoft.Web/serverfarms",
//@[line007->line076]               "apiVersion": "2020-06-01",
//@[line007->line077]               "name": "[format('{0}appPlan', parameters('namePrefix'))]",
//@[line007->line078]               "location": "[resourceGroup().location]",
//@[line007->line079]               "kind": "linux",
//@[line007->line080]               "sku": {
//@[line007->line081]                 "name": "[parameters('sku')]"
//@[line007->line082]               },
//@[line007->line083]               "properties": {
//@[line007->line084]                 "reserved": true
//@[line007->line085]               }
//@[line007->line086]             }
//@[line007->line087]           ],
//@[line007->line088]           "outputs": {
//@[line007->line089]             "planId": {
//@[line007->line090]               "type": "string",
//@[line007->line091]               "value": "[resourceId('Microsoft.Web/serverfarms', format('{0}appPlan', parameters('namePrefix')))]"
//@[line007->line092]             }
//@[line007->line093]           }
//@[line007->line094]         }
//@[line007->line095]       },
//@[line007->line096]       "dependsOn": [
//@[line007->line097]         "[subscriptionResourceId('Microsoft.Resources/resourceGroups', 'adotfrank-rg')]"
//@[line007->line098]       ]
//@[line007->line099]     },
  name: 'planDeploy'
//@[line008->line042]       "name": "planDeploy",
  scope: rg
  params: {
    namePrefix: 'hello'
//@[line011->line051]             "value": "hello"
  }
}

module appPlanDeploy2 'br/mock-registry-one:demo/plan:v2' = {
//@[line015->line100]     {
//@[line015->line101]       "type": "Microsoft.Resources/deployments",
//@[line015->line102]       "apiVersion": "2020-10-01",
//@[line015->line104]       "resourceGroup": "adotfrank-rg",
//@[line015->line105]       "properties": {
//@[line015->line106]         "expressionEvaluationOptions": {
//@[line015->line107]           "scope": "inner"
//@[line015->line108]         },
//@[line015->line109]         "mode": "Incremental",
//@[line015->line110]         "parameters": {
//@[line015->line111]           "namePrefix": {
//@[line015->line113]           }
//@[line015->line114]         },
//@[line015->line115]         "template": {
//@[line015->line116]           "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@[line015->line117]           "contentVersion": "1.0.0.0",
//@[line015->line118]           "metadata": {
//@[line015->line119]             "_generator": {
//@[line015->line120]               "name": "bicep",
//@[line015->line121]               "version": "dev",
//@[line015->line122]               "templateHash": "15019246960605065046"
//@[line015->line123]             }
//@[line015->line124]           },
//@[line015->line125]           "parameters": {
//@[line015->line126]             "namePrefix": {
//@[line015->line127]               "type": "string"
//@[line015->line128]             },
//@[line015->line129]             "sku": {
//@[line015->line130]               "type": "string",
//@[line015->line131]               "defaultValue": "B1"
//@[line015->line132]             }
//@[line015->line133]           },
//@[line015->line134]           "resources": [
//@[line015->line135]             {
//@[line015->line136]               "type": "Microsoft.Web/serverfarms",
//@[line015->line137]               "apiVersion": "2020-06-01",
//@[line015->line138]               "name": "[format('{0}appPlan', parameters('namePrefix'))]",
//@[line015->line139]               "location": "[resourceGroup().location]",
//@[line015->line140]               "kind": "linux",
//@[line015->line141]               "sku": {
//@[line015->line142]                 "name": "[parameters('sku')]"
//@[line015->line143]               },
//@[line015->line144]               "properties": {
//@[line015->line145]                 "reserved": true
//@[line015->line146]               }
//@[line015->line147]             }
//@[line015->line148]           ],
//@[line015->line149]           "outputs": {
//@[line015->line150]             "planId": {
//@[line015->line151]               "type": "string",
//@[line015->line152]               "value": "[resourceId('Microsoft.Web/serverfarms', format('{0}appPlan', parameters('namePrefix')))]"
//@[line015->line153]             }
//@[line015->line154]           }
//@[line015->line155]         }
//@[line015->line156]       },
//@[line015->line157]       "dependsOn": [
//@[line015->line158]         "[subscriptionResourceId('Microsoft.Resources/resourceGroups', 'adotfrank-rg')]"
//@[line015->line159]       ]
//@[line015->line160]     },
  name: 'planDeploy2'
//@[line016->line103]       "name": "planDeploy2",
  scope: rg
  params: {
    namePrefix: 'hello'
//@[line019->line112]             "value": "hello"
  }
}

var websites = [
//@[line023->line011]     "websites": [
//@[line023->line020]     ],
  {
//@[line024->line012]       {
//@[line024->line015]       },
    name: 'fancy'
//@[line025->line013]         "name": "fancy",
    tag: 'latest'
//@[line026->line014]         "tag": "latest"
  }
  {
//@[line028->line016]       {
//@[line028->line019]       }
    name: 'plain'
//@[line029->line017]         "name": "plain",
    tag: 'plain-text'
//@[line030->line018]         "tag": "plain-text"
  }
]

module siteDeploy 'br:mock-registry-two.invalid/demo/site:v3' = [for site in websites: {
//@[line034->line161]     {
//@[line034->line162]       "copy": {
//@[line034->line163]         "name": "siteDeploy",
//@[line034->line164]         "count": "[length(variables('websites'))]"
//@[line034->line165]       },
//@[line034->line166]       "type": "Microsoft.Resources/deployments",
//@[line034->line167]       "apiVersion": "2020-10-01",
//@[line034->line169]       "resourceGroup": "adotfrank-rg",
//@[line034->line170]       "properties": {
//@[line034->line171]         "expressionEvaluationOptions": {
//@[line034->line172]           "scope": "inner"
//@[line034->line173]         },
//@[line034->line174]         "mode": "Incremental",
//@[line034->line175]         "parameters": {
//@[line034->line176]           "appPlanId": {
//@[line034->line178]           },
//@[line034->line179]           "namePrefix": {
//@[line034->line181]           },
//@[line034->line182]           "dockerImage": {
//@[line034->line184]           },
//@[line034->line185]           "dockerImageTag": {
//@[line034->line187]           }
//@[line034->line188]         },
//@[line034->line189]         "template": {
//@[line034->line190]           "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@[line034->line191]           "contentVersion": "1.0.0.0",
//@[line034->line192]           "metadata": {
//@[line034->line193]             "_generator": {
//@[line034->line194]               "name": "bicep",
//@[line034->line195]               "version": "dev",
//@[line034->line196]               "templateHash": "15188988612540889945"
//@[line034->line197]             }
//@[line034->line198]           },
//@[line034->line199]           "parameters": {
//@[line034->line200]             "namePrefix": {
//@[line034->line201]               "type": "string"
//@[line034->line202]             },
//@[line034->line203]             "location": {
//@[line034->line204]               "type": "string",
//@[line034->line205]               "defaultValue": "[resourceGroup().location]"
//@[line034->line206]             },
//@[line034->line207]             "dockerImage": {
//@[line034->line208]               "type": "string"
//@[line034->line209]             },
//@[line034->line210]             "dockerImageTag": {
//@[line034->line211]               "type": "string"
//@[line034->line212]             },
//@[line034->line213]             "appPlanId": {
//@[line034->line214]               "type": "string"
//@[line034->line215]             }
//@[line034->line216]           },
//@[line034->line217]           "resources": [
//@[line034->line218]             {
//@[line034->line219]               "type": "Microsoft.Web/sites",
//@[line034->line220]               "apiVersion": "2020-06-01",
//@[line034->line221]               "name": "[format('{0}site', parameters('namePrefix'))]",
//@[line034->line222]               "location": "[parameters('location')]",
//@[line034->line223]               "properties": {
//@[line034->line224]                 "siteConfig": {
//@[line034->line225]                   "appSettings": [
//@[line034->line226]                     {
//@[line034->line227]                       "name": "DOCKER_REGISTRY_SERVER_URL",
//@[line034->line228]                       "value": "https://index.docker.io"
//@[line034->line229]                     },
//@[line034->line230]                     {
//@[line034->line231]                       "name": "DOCKER_REGISTRY_SERVER_USERNAME",
//@[line034->line232]                       "value": ""
//@[line034->line233]                     },
//@[line034->line234]                     {
//@[line034->line235]                       "name": "DOCKER_REGISTRY_SERVER_PASSWORD",
//@[line034->line236]                       "value": ""
//@[line034->line237]                     },
//@[line034->line238]                     {
//@[line034->line239]                       "name": "WEBSITES_ENABLE_APP_SERVICE_STORAGE",
//@[line034->line240]                       "value": "false"
//@[line034->line241]                     }
//@[line034->line242]                   ],
//@[line034->line243]                   "linuxFxVersion": "[format('DOCKER|{0}:{1}', parameters('dockerImage'), parameters('dockerImageTag'))]"
//@[line034->line244]                 },
//@[line034->line245]                 "serverFarmId": "[parameters('appPlanId')]"
//@[line034->line246]               }
//@[line034->line247]             }
//@[line034->line248]           ],
//@[line034->line249]           "outputs": {
//@[line034->line250]             "siteUrl": {
//@[line034->line251]               "type": "string",
//@[line034->line252]               "value": "[reference(resourceId('Microsoft.Web/sites', format('{0}site', parameters('namePrefix'))), '2020-06-01').hostNames[0]]"
//@[line034->line253]             }
//@[line034->line254]           }
//@[line034->line255]         }
//@[line034->line256]       },
//@[line034->line257]       "dependsOn": [
//@[line034->line258]         "[extensionResourceId(format('/subscriptions/{0}/resourceGroups/{1}', subscription().subscriptionId, 'adotfrank-rg'), 'Microsoft.Resources/deployments', 'planDeploy')]",
//@[line034->line259]         "[subscriptionResourceId('Microsoft.Resources/resourceGroups', 'adotfrank-rg')]"
//@[line034->line260]       ]
//@[line034->line261]     },
  name: '${site.name}siteDeploy'
//@[line035->line168]       "name": "[format('{0}siteDeploy', variables('websites')[copyIndex()].name)]",
  scope: rg
  params: {
    appPlanId: appPlanDeploy.outputs.planId
//@[line038->line177]             "value": "[reference(extensionResourceId(format('/subscriptions/{0}/resourceGroups/{1}', subscription().subscriptionId, 'adotfrank-rg'), 'Microsoft.Resources/deployments', 'planDeploy'), '2020-10-01').outputs.planId.value]"
    namePrefix: site.name
//@[line039->line180]             "value": "[variables('websites')[copyIndex()].name]"
    dockerImage: 'nginxdemos/hello'
//@[line040->line183]             "value": "nginxdemos/hello"
    dockerImageTag: site.tag
//@[line041->line186]             "value": "[variables('websites')[copyIndex()].tag]"
  }
}]

module siteDeploy2 'br/demo-two:site:v3' = [for site in websites: {
//@[line045->line262]     {
//@[line045->line263]       "copy": {
//@[line045->line264]         "name": "siteDeploy2",
//@[line045->line265]         "count": "[length(variables('websites'))]"
//@[line045->line266]       },
//@[line045->line267]       "type": "Microsoft.Resources/deployments",
//@[line045->line268]       "apiVersion": "2020-10-01",
//@[line045->line270]       "resourceGroup": "adotfrank-rg",
//@[line045->line271]       "properties": {
//@[line045->line272]         "expressionEvaluationOptions": {
//@[line045->line273]           "scope": "inner"
//@[line045->line274]         },
//@[line045->line275]         "mode": "Incremental",
//@[line045->line276]         "parameters": {
//@[line045->line277]           "appPlanId": {
//@[line045->line279]           },
//@[line045->line280]           "namePrefix": {
//@[line045->line282]           },
//@[line045->line283]           "dockerImage": {
//@[line045->line285]           },
//@[line045->line286]           "dockerImageTag": {
//@[line045->line288]           }
//@[line045->line289]         },
//@[line045->line290]         "template": {
//@[line045->line291]           "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@[line045->line292]           "contentVersion": "1.0.0.0",
//@[line045->line293]           "metadata": {
//@[line045->line294]             "_generator": {
//@[line045->line295]               "name": "bicep",
//@[line045->line296]               "version": "dev",
//@[line045->line297]               "templateHash": "15188988612540889945"
//@[line045->line298]             }
//@[line045->line299]           },
//@[line045->line300]           "parameters": {
//@[line045->line301]             "namePrefix": {
//@[line045->line302]               "type": "string"
//@[line045->line303]             },
//@[line045->line304]             "location": {
//@[line045->line305]               "type": "string",
//@[line045->line306]               "defaultValue": "[resourceGroup().location]"
//@[line045->line307]             },
//@[line045->line308]             "dockerImage": {
//@[line045->line309]               "type": "string"
//@[line045->line310]             },
//@[line045->line311]             "dockerImageTag": {
//@[line045->line312]               "type": "string"
//@[line045->line313]             },
//@[line045->line314]             "appPlanId": {
//@[line045->line315]               "type": "string"
//@[line045->line316]             }
//@[line045->line317]           },
//@[line045->line318]           "resources": [
//@[line045->line319]             {
//@[line045->line320]               "type": "Microsoft.Web/sites",
//@[line045->line321]               "apiVersion": "2020-06-01",
//@[line045->line322]               "name": "[format('{0}site', parameters('namePrefix'))]",
//@[line045->line323]               "location": "[parameters('location')]",
//@[line045->line324]               "properties": {
//@[line045->line325]                 "siteConfig": {
//@[line045->line326]                   "appSettings": [
//@[line045->line327]                     {
//@[line045->line328]                       "name": "DOCKER_REGISTRY_SERVER_URL",
//@[line045->line329]                       "value": "https://index.docker.io"
//@[line045->line330]                     },
//@[line045->line331]                     {
//@[line045->line332]                       "name": "DOCKER_REGISTRY_SERVER_USERNAME",
//@[line045->line333]                       "value": ""
//@[line045->line334]                     },
//@[line045->line335]                     {
//@[line045->line336]                       "name": "DOCKER_REGISTRY_SERVER_PASSWORD",
//@[line045->line337]                       "value": ""
//@[line045->line338]                     },
//@[line045->line339]                     {
//@[line045->line340]                       "name": "WEBSITES_ENABLE_APP_SERVICE_STORAGE",
//@[line045->line341]                       "value": "false"
//@[line045->line342]                     }
//@[line045->line343]                   ],
//@[line045->line344]                   "linuxFxVersion": "[format('DOCKER|{0}:{1}', parameters('dockerImage'), parameters('dockerImageTag'))]"
//@[line045->line345]                 },
//@[line045->line346]                 "serverFarmId": "[parameters('appPlanId')]"
//@[line045->line347]               }
//@[line045->line348]             }
//@[line045->line349]           ],
//@[line045->line350]           "outputs": {
//@[line045->line351]             "siteUrl": {
//@[line045->line352]               "type": "string",
//@[line045->line353]               "value": "[reference(resourceId('Microsoft.Web/sites', format('{0}site', parameters('namePrefix'))), '2020-06-01').hostNames[0]]"
//@[line045->line354]             }
//@[line045->line355]           }
//@[line045->line356]         }
//@[line045->line357]       },
//@[line045->line358]       "dependsOn": [
//@[line045->line359]         "[extensionResourceId(format('/subscriptions/{0}/resourceGroups/{1}', subscription().subscriptionId, 'adotfrank-rg'), 'Microsoft.Resources/deployments', 'planDeploy')]",
//@[line045->line360]         "[subscriptionResourceId('Microsoft.Resources/resourceGroups', 'adotfrank-rg')]"
//@[line045->line361]       ]
//@[line045->line362]     },
  name: '${site.name}siteDeploy2'
//@[line046->line269]       "name": "[format('{0}siteDeploy2', variables('websites')[copyIndex()].name)]",
  scope: rg
  params: {
    appPlanId: appPlanDeploy.outputs.planId
//@[line049->line278]             "value": "[reference(extensionResourceId(format('/subscriptions/{0}/resourceGroups/{1}', subscription().subscriptionId, 'adotfrank-rg'), 'Microsoft.Resources/deployments', 'planDeploy'), '2020-10-01').outputs.planId.value]"
    namePrefix: site.name
//@[line050->line281]             "value": "[variables('websites')[copyIndex()].name]"
    dockerImage: 'nginxdemos/hello'
//@[line051->line284]             "value": "nginxdemos/hello"
    dockerImageTag: site.tag
//@[line052->line287]             "value": "[variables('websites')[copyIndex()].tag]"
  }
}]

module storageDeploy 'ts:00000000-0000-0000-0000-000000000000/test-rg/storage-spec:1.0' = {
//@[line056->line363]     {
//@[line056->line364]       "type": "Microsoft.Resources/deployments",
//@[line056->line365]       "apiVersion": "2020-10-01",
//@[line056->line367]       "resourceGroup": "adotfrank-rg",
//@[line056->line368]       "properties": {
//@[line056->line369]         "expressionEvaluationOptions": {
//@[line056->line370]           "scope": "inner"
//@[line056->line371]         },
//@[line056->line372]         "mode": "Incremental",
//@[line056->line373]         "parameters": {
//@[line056->line374]           "location": {
//@[line056->line376]           }
//@[line056->line377]         },
//@[line056->line378]         "templateLink": {
//@[line056->line379]           "id": "/subscriptions/00000000-0000-0000-0000-000000000000/resourceGroups/test-rg/providers/Microsoft.Resources/templateSpecs/storage-spec/versions/1.0"
//@[line056->line380]         }
//@[line056->line381]       },
//@[line056->line382]       "dependsOn": [
//@[line056->line383]         "[subscriptionResourceId('Microsoft.Resources/resourceGroups', 'adotfrank-rg')]"
//@[line056->line384]       ]
//@[line056->line385]     },
  name: 'storageDeploy'
//@[line057->line366]       "name": "storageDeploy",
  scope: rg
  params: {
    location: 'eastus'
//@[line060->line375]             "value": "eastus"
  }
}

module storageDeploy2 'ts/mySpecRG:storage-spec:1.0' = {
//@[line064->line386]     {
//@[line064->line387]       "type": "Microsoft.Resources/deployments",
//@[line064->line388]       "apiVersion": "2020-10-01",
//@[line064->line390]       "resourceGroup": "adotfrank-rg",
//@[line064->line391]       "properties": {
//@[line064->line392]         "expressionEvaluationOptions": {
//@[line064->line393]           "scope": "inner"
//@[line064->line394]         },
//@[line064->line395]         "mode": "Incremental",
//@[line064->line396]         "parameters": {
//@[line064->line397]           "location": {
//@[line064->line399]           }
//@[line064->line400]         },
//@[line064->line401]         "templateLink": {
//@[line064->line402]           "id": "/subscriptions/00000000-0000-0000-0000-000000000000/resourceGroups/test-rg/providers/Microsoft.Resources/templateSpecs/storage-spec/versions/1.0"
//@[line064->line403]         }
//@[line064->line404]       },
//@[line064->line405]       "dependsOn": [
//@[line064->line406]         "[subscriptionResourceId('Microsoft.Resources/resourceGroups', 'adotfrank-rg')]"
//@[line064->line407]       ]
//@[line064->line408]     },
  name: 'storageDeploy2'
//@[line065->line389]       "name": "storageDeploy2",
  scope: rg
  params: {
    location: 'eastus'
//@[line068->line398]             "value": "eastus"
  }
}

var vnets = [
//@[line072->line021]     "vnets": [
//@[line072->line030]     ]
  {
//@[line073->line022]       {
//@[line073->line025]       },
    name: 'vnet1'
//@[line074->line023]         "name": "vnet1",
    subnetName: 'subnet1.1'
//@[line075->line024]         "subnetName": "subnet1.1"
  }
  {
//@[line077->line026]       {
//@[line077->line029]       }
    name: 'vnet2'
//@[line078->line027]         "name": "vnet2",
    subnetName: 'subnet2.1'
//@[line079->line028]         "subnetName": "subnet2.1"
  }
]

module vnetDeploy 'ts:11111111-1111-1111-1111-111111111111/prod-rg/vnet-spec:v2' = [for vnet in vnets: {
//@[line083->line409]     {
//@[line083->line410]       "copy": {
//@[line083->line411]         "name": "vnetDeploy",
//@[line083->line412]         "count": "[length(variables('vnets'))]"
//@[line083->line413]       },
//@[line083->line414]       "type": "Microsoft.Resources/deployments",
//@[line083->line415]       "apiVersion": "2020-10-01",
//@[line083->line417]       "resourceGroup": "adotfrank-rg",
//@[line083->line418]       "properties": {
//@[line083->line419]         "expressionEvaluationOptions": {
//@[line083->line420]           "scope": "inner"
//@[line083->line421]         },
//@[line083->line422]         "mode": "Incremental",
//@[line083->line423]         "parameters": {
//@[line083->line424]           "vnetName": {
//@[line083->line426]           },
//@[line083->line427]           "subnetName": {
//@[line083->line429]           }
//@[line083->line430]         },
//@[line083->line431]         "templateLink": {
//@[line083->line432]           "id": "/subscriptions/11111111-1111-1111-1111-111111111111/resourceGroups/prod-rg/providers/Microsoft.Resources/templateSpecs/vnet-spec/versions/v2"
//@[line083->line433]         }
//@[line083->line434]       },
//@[line083->line435]       "dependsOn": [
//@[line083->line436]         "[subscriptionResourceId('Microsoft.Resources/resourceGroups', 'adotfrank-rg')]"
//@[line083->line437]       ]
//@[line083->line438]     },
  name: '${vnet.name}Deploy'
//@[line084->line416]       "name": "[format('{0}Deploy', variables('vnets')[copyIndex()].name)]",
  scope: rg
  params: {
    vnetName: vnet.name
//@[line087->line425]             "value": "[variables('vnets')[copyIndex()].name]"
    subnetName: vnet.subnetName
//@[line088->line428]             "value": "[variables('vnets')[copyIndex()].subnetName]"
  }
}]

output siteUrls array = [for (site, i) in websites: siteDeploy[i].outputs.siteUrl]
//@[line092->line656]     "siteUrls": {
//@[line092->line657]       "type": "array",
//@[line092->line658]       "copy": {
//@[line092->line659]         "count": "[length(variables('websites'))]",
//@[line092->line660]         "input": "[reference(extensionResourceId(format('/subscriptions/{0}/resourceGroups/{1}', subscription().subscriptionId, 'adotfrank-rg'), 'Microsoft.Resources/deployments', format('{0}siteDeploy', variables('websites')[copyIndex()].name)), '2020-10-01').outputs.siteUrl.value]"
//@[line092->line661]       }
//@[line092->line662]     }

module passthroughPort 'br:localhost:5000/passthrough/port:v1' = {
//@[line094->line439]     {
//@[line094->line440]       "type": "Microsoft.Resources/deployments",
//@[line094->line441]       "apiVersion": "2020-10-01",
//@[line094->line443]       "resourceGroup": "adotfrank-rg",
//@[line094->line444]       "properties": {
//@[line094->line445]         "expressionEvaluationOptions": {
//@[line094->line446]           "scope": "inner"
//@[line094->line447]         },
//@[line094->line448]         "mode": "Incremental",
//@[line094->line449]         "parameters": {
//@[line094->line450]           "port": {
//@[line094->line452]           }
//@[line094->line453]         },
//@[line094->line454]         "template": {
//@[line094->line455]           "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@[line094->line456]           "contentVersion": "1.0.0.0",
//@[line094->line457]           "metadata": {
//@[line094->line458]             "_generator": {
//@[line094->line459]               "name": "bicep",
//@[line094->line460]               "version": "dev",
//@[line094->line461]               "templateHash": "2505217721104922110"
//@[line094->line462]             }
//@[line094->line463]           },
//@[line094->line464]           "parameters": {
//@[line094->line465]             "port": {
//@[line094->line466]               "type": "string"
//@[line094->line467]             }
//@[line094->line468]           },
//@[line094->line469]           "resources": [],
//@[line094->line470]           "outputs": {
//@[line094->line471]             "port": {
//@[line094->line472]               "type": "string",
//@[line094->line473]               "value": "[parameters('port')]"
//@[line094->line474]             }
//@[line094->line475]           }
//@[line094->line476]         }
//@[line094->line477]       },
//@[line094->line478]       "dependsOn": [
//@[line094->line479]         "[subscriptionResourceId('Microsoft.Resources/resourceGroups', 'adotfrank-rg')]"
//@[line094->line480]       ]
//@[line094->line481]     },
  scope: rg
  name: 'port'
//@[line096->line442]       "name": "port",
  params: {
    port: 'test'
//@[line098->line451]             "value": "test"
  }
}

module ipv4 'br:127.0.0.1/passthrough/ipv4:v1' = {
//@[line102->line482]     {
//@[line102->line483]       "type": "Microsoft.Resources/deployments",
//@[line102->line484]       "apiVersion": "2020-10-01",
//@[line102->line486]       "resourceGroup": "adotfrank-rg",
//@[line102->line487]       "properties": {
//@[line102->line488]         "expressionEvaluationOptions": {
//@[line102->line489]           "scope": "inner"
//@[line102->line490]         },
//@[line102->line491]         "mode": "Incremental",
//@[line102->line492]         "parameters": {
//@[line102->line493]           "ipv4": {
//@[line102->line495]           }
//@[line102->line496]         },
//@[line102->line497]         "template": {
//@[line102->line498]           "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@[line102->line499]           "contentVersion": "1.0.0.0",
//@[line102->line500]           "metadata": {
//@[line102->line501]             "_generator": {
//@[line102->line502]               "name": "bicep",
//@[line102->line503]               "version": "dev",
//@[line102->line504]               "templateHash": "12121705673191614943"
//@[line102->line505]             }
//@[line102->line506]           },
//@[line102->line507]           "parameters": {
//@[line102->line508]             "ipv4": {
//@[line102->line509]               "type": "string"
//@[line102->line510]             }
//@[line102->line511]           },
//@[line102->line512]           "resources": [],
//@[line102->line513]           "outputs": {
//@[line102->line514]             "ipv4": {
//@[line102->line515]               "type": "string",
//@[line102->line516]               "value": "[parameters('ipv4')]"
//@[line102->line517]             }
//@[line102->line518]           }
//@[line102->line519]         }
//@[line102->line520]       },
//@[line102->line521]       "dependsOn": [
//@[line102->line522]         "[subscriptionResourceId('Microsoft.Resources/resourceGroups', 'adotfrank-rg')]"
//@[line102->line523]       ]
//@[line102->line524]     },
  scope: rg
  name: 'ipv4'
//@[line104->line485]       "name": "ipv4",
  params: {
    ipv4: 'test'
//@[line106->line494]             "value": "test"
  }
}

module ipv4port 'br:127.0.0.1:5000/passthrough/ipv4port:v1' = {
//@[line110->line525]     {
//@[line110->line526]       "type": "Microsoft.Resources/deployments",
//@[line110->line527]       "apiVersion": "2020-10-01",
//@[line110->line529]       "resourceGroup": "adotfrank-rg",
//@[line110->line530]       "properties": {
//@[line110->line531]         "expressionEvaluationOptions": {
//@[line110->line532]           "scope": "inner"
//@[line110->line533]         },
//@[line110->line534]         "mode": "Incremental",
//@[line110->line535]         "parameters": {
//@[line110->line536]           "ipv4port": {
//@[line110->line538]           }
//@[line110->line539]         },
//@[line110->line540]         "template": {
//@[line110->line541]           "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@[line110->line542]           "contentVersion": "1.0.0.0",
//@[line110->line543]           "metadata": {
//@[line110->line544]             "_generator": {
//@[line110->line545]               "name": "bicep",
//@[line110->line546]               "version": "dev",
//@[line110->line547]               "templateHash": "7823936554124086035"
//@[line110->line548]             }
//@[line110->line549]           },
//@[line110->line550]           "parameters": {
//@[line110->line551]             "ipv4port": {
//@[line110->line552]               "type": "string"
//@[line110->line553]             }
//@[line110->line554]           },
//@[line110->line555]           "resources": [],
//@[line110->line556]           "outputs": {
//@[line110->line557]             "ipv4port": {
//@[line110->line558]               "type": "string",
//@[line110->line559]               "value": "[parameters('ipv4port')]"
//@[line110->line560]             }
//@[line110->line561]           }
//@[line110->line562]         }
//@[line110->line563]       },
//@[line110->line564]       "dependsOn": [
//@[line110->line565]         "[subscriptionResourceId('Microsoft.Resources/resourceGroups', 'adotfrank-rg')]"
//@[line110->line566]       ]
//@[line110->line567]     },
  scope: rg
  name: 'ipv4port'
//@[line112->line528]       "name": "ipv4port",
  params: {
    ipv4port: 'test'
//@[line114->line537]             "value": "test"
  }
}

module ipv6 'br:[::1]/passthrough/ipv6:v1' = {
//@[line118->line568]     {
//@[line118->line569]       "type": "Microsoft.Resources/deployments",
//@[line118->line570]       "apiVersion": "2020-10-01",
//@[line118->line572]       "resourceGroup": "adotfrank-rg",
//@[line118->line573]       "properties": {
//@[line118->line574]         "expressionEvaluationOptions": {
//@[line118->line575]           "scope": "inner"
//@[line118->line576]         },
//@[line118->line577]         "mode": "Incremental",
//@[line118->line578]         "parameters": {
//@[line118->line579]           "ipv6": {
//@[line118->line581]           }
//@[line118->line582]         },
//@[line118->line583]         "template": {
//@[line118->line584]           "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@[line118->line585]           "contentVersion": "1.0.0.0",
//@[line118->line586]           "metadata": {
//@[line118->line587]             "_generator": {
//@[line118->line588]               "name": "bicep",
//@[line118->line589]               "version": "dev",
//@[line118->line590]               "templateHash": "5093140793351809054"
//@[line118->line591]             }
//@[line118->line592]           },
//@[line118->line593]           "parameters": {
//@[line118->line594]             "ipv6": {
//@[line118->line595]               "type": "string"
//@[line118->line596]             }
//@[line118->line597]           },
//@[line118->line598]           "resources": [],
//@[line118->line599]           "outputs": {
//@[line118->line600]             "ipv6": {
//@[line118->line601]               "type": "string",
//@[line118->line602]               "value": "[parameters('ipv6')]"
//@[line118->line603]             }
//@[line118->line604]           }
//@[line118->line605]         }
//@[line118->line606]       },
//@[line118->line607]       "dependsOn": [
//@[line118->line608]         "[subscriptionResourceId('Microsoft.Resources/resourceGroups', 'adotfrank-rg')]"
//@[line118->line609]       ]
//@[line118->line610]     },
  scope: rg
  name: 'ipv6'
//@[line120->line571]       "name": "ipv6",
  params: {
    ipv6: 'test'
//@[line122->line580]             "value": "test"
  }
}

module ipv6port 'br:[::1]:5000/passthrough/ipv6port:v1' = {
//@[line126->line611]     {
//@[line126->line612]       "type": "Microsoft.Resources/deployments",
//@[line126->line613]       "apiVersion": "2020-10-01",
//@[line126->line615]       "resourceGroup": "adotfrank-rg",
//@[line126->line616]       "properties": {
//@[line126->line617]         "expressionEvaluationOptions": {
//@[line126->line618]           "scope": "inner"
//@[line126->line619]         },
//@[line126->line620]         "mode": "Incremental",
//@[line126->line621]         "parameters": {
//@[line126->line622]           "ipv6port": {
//@[line126->line624]           }
//@[line126->line625]         },
//@[line126->line626]         "template": {
//@[line126->line627]           "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@[line126->line628]           "contentVersion": "1.0.0.0",
//@[line126->line629]           "metadata": {
//@[line126->line630]             "_generator": {
//@[line126->line631]               "name": "bicep",
//@[line126->line632]               "version": "dev",
//@[line126->line633]               "templateHash": "969477156678070948"
//@[line126->line634]             }
//@[line126->line635]           },
//@[line126->line636]           "parameters": {
//@[line126->line637]             "ipv6port": {
//@[line126->line638]               "type": "string"
//@[line126->line639]             }
//@[line126->line640]           },
//@[line126->line641]           "resources": [],
//@[line126->line642]           "outputs": {
//@[line126->line643]             "ipv6port": {
//@[line126->line644]               "type": "string",
//@[line126->line645]               "value": "[parameters('ipv6port')]"
//@[line126->line646]             }
//@[line126->line647]           }
//@[line126->line648]         }
//@[line126->line649]       },
//@[line126->line650]       "dependsOn": [
//@[line126->line651]         "[subscriptionResourceId('Microsoft.Resources/resourceGroups', 'adotfrank-rg')]"
//@[line126->line652]       ]
//@[line126->line653]     }
  scope: rg
  name: 'ipv6port'
//@[line128->line614]       "name": "ipv6port",
  params: {
    ipv6port: 'test'
//@[line130->line623]             "value": "test"
  }
}
