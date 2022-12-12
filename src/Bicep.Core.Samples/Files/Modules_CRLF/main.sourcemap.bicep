
@sys.description('this is deployTimeSuffix param')
//@[line001->line0015]         "description": "this is deployTimeSuffix param"
param deployTimeSuffix string = newGuid()
//@[line002->line0011]     "deployTimeSuffix": {
//@[line002->line0012]       "type": "string",
//@[line002->line0013]       "defaultValue": "[newGuid()]",
//@[line002->line0014]       "metadata": {
//@[line002->line0016]       }
//@[line002->line0017]     }

@sys.description('this module a')
//@[line004->line0175]         "description": "this module a"
module modATest './modulea.bicep' = {
//@[line005->line0085]     {
//@[line005->line0086]       "type": "Microsoft.Resources/deployments",
//@[line005->line0087]       "apiVersion": "2020-10-01",
//@[line005->line0089]       "properties": {
//@[line005->line0090]         "expressionEvaluationOptions": {
//@[line005->line0091]           "scope": "inner"
//@[line005->line0092]         },
//@[line005->line0093]         "mode": "Incremental",
//@[line005->line0094]         "parameters": {
//@[line005->line0095]           "stringParamB": {
//@[line005->line0097]           },
//@[line005->line0098]           "objParam": {
//@[line005->line0102]           },
//@[line005->line0103]           "arrayParam": {
//@[line005->line0110]           }
//@[line005->line0111]         },
//@[line005->line0112]         "template": {
//@[line005->line0113]           "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@[line005->line0114]           "contentVersion": "1.0.0.0",
//@[line005->line0115]           "metadata": {
//@[line005->line0116]             "_generator": {
//@[line005->line0117]               "name": "bicep",
//@[line005->line0118]               "version": "dev",
//@[line005->line0119]               "templateHash": "8300391961099598421"
//@[line005->line0120]             }
//@[line005->line0121]           },
//@[line005->line0122]           "parameters": {
//@[line005->line0123]             "stringParamA": {
//@[line005->line0124]               "type": "string",
//@[line005->line0125]               "defaultValue": "test"
//@[line005->line0126]             },
//@[line005->line0127]             "stringParamB": {
//@[line005->line0128]               "type": "string"
//@[line005->line0129]             },
//@[line005->line0130]             "objParam": {
//@[line005->line0131]               "type": "object"
//@[line005->line0132]             },
//@[line005->line0133]             "arrayParam": {
//@[line005->line0134]               "type": "array"
//@[line005->line0135]             }
//@[line005->line0136]           },
//@[line005->line0137]           "resources": [
//@[line005->line0138]             {
//@[line005->line0139]               "type": "Mock.Rp/mockResource",
//@[line005->line0140]               "apiVersion": "2020-01-01",
//@[line005->line0141]               "name": "basicblobs",
//@[line005->line0142]               "location": "[parameters('stringParamA')]"
//@[line005->line0143]             },
//@[line005->line0144]             {
//@[line005->line0145]               "type": "Mock.Rp/mockResource",
//@[line005->line0146]               "apiVersion": "2020-01-01",
//@[line005->line0147]               "name": "myZone",
//@[line005->line0148]               "location": "[parameters('stringParamB')]"
//@[line005->line0149]             }
//@[line005->line0150]           ],
//@[line005->line0151]           "outputs": {
//@[line005->line0152]             "stringOutputA": {
//@[line005->line0153]               "type": "string",
//@[line005->line0154]               "value": "[parameters('stringParamA')]"
//@[line005->line0155]             },
//@[line005->line0156]             "stringOutputB": {
//@[line005->line0157]               "type": "string",
//@[line005->line0158]               "value": "[parameters('stringParamB')]"
//@[line005->line0159]             },
//@[line005->line0160]             "objOutput": {
//@[line005->line0161]               "type": "object",
//@[line005->line0162]               "value": "[reference(resourceId('Mock.Rp/mockResource', 'basicblobs'), '2020-01-01')]"
//@[line005->line0163]             },
//@[line005->line0164]             "arrayOutput": {
//@[line005->line0165]               "type": "array",
//@[line005->line0166]               "value": [
//@[line005->line0167]                 "[resourceId('Mock.Rp/mockResource', 'basicblobs')]",
//@[line005->line0168]                 "[resourceId('Mock.Rp/mockResource', 'myZone')]"
//@[line005->line0169]               ]
//@[line005->line0170]             }
//@[line005->line0171]           }
//@[line005->line0172]         }
//@[line005->line0173]       },
//@[line005->line0174]       "metadata": {
//@[line005->line0176]       }
//@[line005->line0177]     },
  name: 'modATest'
//@[line006->line0088]       "name": "modATest",
  params: {
    stringParamB: 'hello!'
//@[line008->line0096]             "value": "hello!"
    objParam: {
//@[line009->line0099]             "value": {
//@[line009->line0101]             }
      a: 'b'
//@[line010->line0100]               "a": "b"
    }
    arrayParam: [
//@[line012->line0104]             "value": [
//@[line012->line0109]             ]
      {
//@[line013->line0105]               {
//@[line013->line0107]               },
        a: 'b'
//@[line014->line0106]                 "a": "b"
      }
      'abc'
//@[line016->line0108]               "abc"
    ]
  }
}


@sys.description('this module b')
//@[line022->line0224]         "description": "this module b"
module modB './child/moduleb.bicep' = {
//@[line023->line0178]     {
//@[line023->line0179]       "type": "Microsoft.Resources/deployments",
//@[line023->line0180]       "apiVersion": "2020-10-01",
//@[line023->line0182]       "properties": {
//@[line023->line0183]         "expressionEvaluationOptions": {
//@[line023->line0184]           "scope": "inner"
//@[line023->line0185]         },
//@[line023->line0186]         "mode": "Incremental",
//@[line023->line0187]         "parameters": {
//@[line023->line0188]           "location": {
//@[line023->line0190]           }
//@[line023->line0191]         },
//@[line023->line0192]         "template": {
//@[line023->line0193]           "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@[line023->line0194]           "contentVersion": "1.0.0.0",
//@[line023->line0195]           "metadata": {
//@[line023->line0196]             "_generator": {
//@[line023->line0197]               "name": "bicep",
//@[line023->line0198]               "version": "dev",
//@[line023->line0199]               "templateHash": "13693869390953445824"
//@[line023->line0200]             }
//@[line023->line0201]           },
//@[line023->line0202]           "parameters": {
//@[line023->line0203]             "location": {
//@[line023->line0204]               "type": "string"
//@[line023->line0205]             }
//@[line023->line0206]           },
//@[line023->line0207]           "resources": [
//@[line023->line0208]             {
//@[line023->line0209]               "type": "Mock.Rp/mockResource",
//@[line023->line0210]               "apiVersion": "2020-01-01",
//@[line023->line0211]               "name": "mockResource",
//@[line023->line0212]               "location": "[parameters('location')]"
//@[line023->line0213]             }
//@[line023->line0214]           ],
//@[line023->line0215]           "outputs": {
//@[line023->line0216]             "myResourceId": {
//@[line023->line0217]               "type": "string",
//@[line023->line0218]               "value": "[resourceId('Mock.Rp/mockResource', 'mockResource')]"
//@[line023->line0219]             }
//@[line023->line0220]           }
//@[line023->line0221]         }
//@[line023->line0222]       },
//@[line023->line0223]       "metadata": {
//@[line023->line0225]       }
//@[line023->line0226]     },
  name: 'modB'
//@[line024->line0181]       "name": "modB",
  params: {
    location: 'West US'
//@[line026->line0189]             "value": "West US"
  }
}

@sys.description('this is just module b with a condition')
//@[line030->line0274]         "description": "this is just module b with a condition"
module modBWithCondition './child/moduleb.bicep' = if (1 + 1 == 2) {
//@[line031->line0227]     {
//@[line031->line0228]       "condition": "[equals(add(1, 1), 2)]",
//@[line031->line0229]       "type": "Microsoft.Resources/deployments",
//@[line031->line0230]       "apiVersion": "2020-10-01",
//@[line031->line0232]       "properties": {
//@[line031->line0233]         "expressionEvaluationOptions": {
//@[line031->line0234]           "scope": "inner"
//@[line031->line0235]         },
//@[line031->line0236]         "mode": "Incremental",
//@[line031->line0237]         "parameters": {
//@[line031->line0238]           "location": {
//@[line031->line0240]           }
//@[line031->line0241]         },
//@[line031->line0242]         "template": {
//@[line031->line0243]           "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@[line031->line0244]           "contentVersion": "1.0.0.0",
//@[line031->line0245]           "metadata": {
//@[line031->line0246]             "_generator": {
//@[line031->line0247]               "name": "bicep",
//@[line031->line0248]               "version": "dev",
//@[line031->line0249]               "templateHash": "13693869390953445824"
//@[line031->line0250]             }
//@[line031->line0251]           },
//@[line031->line0252]           "parameters": {
//@[line031->line0253]             "location": {
//@[line031->line0254]               "type": "string"
//@[line031->line0255]             }
//@[line031->line0256]           },
//@[line031->line0257]           "resources": [
//@[line031->line0258]             {
//@[line031->line0259]               "type": "Mock.Rp/mockResource",
//@[line031->line0260]               "apiVersion": "2020-01-01",
//@[line031->line0261]               "name": "mockResource",
//@[line031->line0262]               "location": "[parameters('location')]"
//@[line031->line0263]             }
//@[line031->line0264]           ],
//@[line031->line0265]           "outputs": {
//@[line031->line0266]             "myResourceId": {
//@[line031->line0267]               "type": "string",
//@[line031->line0268]               "value": "[resourceId('Mock.Rp/mockResource', 'mockResource')]"
//@[line031->line0269]             }
//@[line031->line0270]           }
//@[line031->line0271]         }
//@[line031->line0272]       },
//@[line031->line0273]       "metadata": {
//@[line031->line0275]       }
//@[line031->line0276]     },
  name: 'modBWithCondition'
//@[line032->line0231]       "name": "modBWithCondition",
  params: {
    location: 'East US'
//@[line034->line0239]             "value": "East US"
  }
}

module modC './child/modulec.json' = {
//@[line038->line0277]     {
//@[line038->line0278]       "type": "Microsoft.Resources/deployments",
//@[line038->line0279]       "apiVersion": "2020-10-01",
//@[line038->line0281]       "properties": {
//@[line038->line0282]         "expressionEvaluationOptions": {
//@[line038->line0283]           "scope": "inner"
//@[line038->line0284]         },
//@[line038->line0285]         "mode": "Incremental",
//@[line038->line0286]         "parameters": {
//@[line038->line0287]           "location": {
//@[line038->line0289]           }
//@[line038->line0290]         },
//@[line038->line0291]         "template": {
//@[line038->line0292]           "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@[line038->line0293]           "contentVersion": "1.0.0.0",
//@[line038->line0294]           "parameters": {
//@[line038->line0295]             "location": {
//@[line038->line0296]               "type": "string"
//@[line038->line0297]             }
//@[line038->line0298]           },
//@[line038->line0299]           "variables": {},
//@[line038->line0300]           "resources": [
//@[line038->line0301]             {
//@[line038->line0302]               "name": "myResource",
//@[line038->line0303]               "type": "Mock.Rp/mockResource",
//@[line038->line0304]               "apiVersion": "2020-01-01",
//@[line038->line0305]               "location": "[parameters('location')]"
//@[line038->line0306]             }
//@[line038->line0307]           ],
//@[line038->line0308]           "outputs": {
//@[line038->line0309]             "myResourceId": {
//@[line038->line0310]               "type": "string",
//@[line038->line0311]               "value": "[resourceId('Mock.Rp/mockResource', 'myResource')]"
//@[line038->line0312]             }
//@[line038->line0313]           }
//@[line038->line0314]         }
//@[line038->line0315]       }
//@[line038->line0316]     },
  name: 'modC'
//@[line039->line0280]       "name": "modC",
  params: {
    location: 'West US'
//@[line041->line0288]             "value": "West US"
  }
}

module modCWithCondition './child/modulec.json' = if (2 - 1 == 1) {
//@[line045->line0317]     {
//@[line045->line0318]       "condition": "[equals(sub(2, 1), 1)]",
//@[line045->line0319]       "type": "Microsoft.Resources/deployments",
//@[line045->line0320]       "apiVersion": "2020-10-01",
//@[line045->line0322]       "properties": {
//@[line045->line0323]         "expressionEvaluationOptions": {
//@[line045->line0324]           "scope": "inner"
//@[line045->line0325]         },
//@[line045->line0326]         "mode": "Incremental",
//@[line045->line0327]         "parameters": {
//@[line045->line0328]           "location": {
//@[line045->line0330]           }
//@[line045->line0331]         },
//@[line045->line0332]         "template": {
//@[line045->line0333]           "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@[line045->line0334]           "contentVersion": "1.0.0.0",
//@[line045->line0335]           "parameters": {
//@[line045->line0336]             "location": {
//@[line045->line0337]               "type": "string"
//@[line045->line0338]             }
//@[line045->line0339]           },
//@[line045->line0340]           "variables": {},
//@[line045->line0341]           "resources": [
//@[line045->line0342]             {
//@[line045->line0343]               "name": "myResource",
//@[line045->line0344]               "type": "Mock.Rp/mockResource",
//@[line045->line0345]               "apiVersion": "2020-01-01",
//@[line045->line0346]               "location": "[parameters('location')]"
//@[line045->line0347]             }
//@[line045->line0348]           ],
//@[line045->line0349]           "outputs": {
//@[line045->line0350]             "myResourceId": {
//@[line045->line0351]               "type": "string",
//@[line045->line0352]               "value": "[resourceId('Mock.Rp/mockResource', 'myResource')]"
//@[line045->line0353]             }
//@[line045->line0354]           }
//@[line045->line0355]         }
//@[line045->line0356]       }
//@[line045->line0357]     },
  name: 'modCWithCondition'
//@[line046->line0321]       "name": "modCWithCondition",
  params: {
    location: 'East US'
//@[line048->line0329]             "value": "East US"
  }
}

module optionalWithNoParams1 './child/optionalParams.bicep'= {
//@[line052->line0358]     {
//@[line052->line0359]       "type": "Microsoft.Resources/deployments",
//@[line052->line0360]       "apiVersion": "2020-10-01",
//@[line052->line0362]       "properties": {
//@[line052->line0363]         "expressionEvaluationOptions": {
//@[line052->line0364]           "scope": "inner"
//@[line052->line0365]         },
//@[line052->line0366]         "mode": "Incremental",
//@[line052->line0367]         "template": {
//@[line052->line0368]           "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@[line052->line0369]           "contentVersion": "1.0.0.0",
//@[line052->line0370]           "metadata": {
//@[line052->line0371]             "_generator": {
//@[line052->line0372]               "name": "bicep",
//@[line052->line0373]               "version": "dev",
//@[line052->line0374]               "templateHash": "4191259681487754679"
//@[line052->line0375]             }
//@[line052->line0376]           },
//@[line052->line0377]           "parameters": {
//@[line052->line0378]             "optionalString": {
//@[line052->line0379]               "type": "string",
//@[line052->line0380]               "defaultValue": "abc"
//@[line052->line0381]             },
//@[line052->line0382]             "optionalInt": {
//@[line052->line0383]               "type": "int",
//@[line052->line0384]               "defaultValue": 42
//@[line052->line0385]             },
//@[line052->line0386]             "optionalObj": {
//@[line052->line0387]               "type": "object",
//@[line052->line0388]               "defaultValue": {
//@[line052->line0389]                 "a": "b"
//@[line052->line0390]               }
//@[line052->line0391]             },
//@[line052->line0392]             "optionalArray": {
//@[line052->line0393]               "type": "array",
//@[line052->line0394]               "defaultValue": [
//@[line052->line0395]                 1,
//@[line052->line0396]                 2,
//@[line052->line0397]                 3
//@[line052->line0398]               ]
//@[line052->line0399]             }
//@[line052->line0400]           },
//@[line052->line0401]           "resources": [],
//@[line052->line0402]           "outputs": {
//@[line052->line0403]             "outputObj": {
//@[line052->line0404]               "type": "object",
//@[line052->line0405]               "value": {
//@[line052->line0406]                 "optionalString": "[parameters('optionalString')]",
//@[line052->line0407]                 "optionalInt": "[parameters('optionalInt')]",
//@[line052->line0408]                 "optionalObj": "[parameters('optionalObj')]",
//@[line052->line0409]                 "optionalArray": "[parameters('optionalArray')]"
//@[line052->line0410]               }
//@[line052->line0411]             }
//@[line052->line0412]           }
//@[line052->line0413]         }
//@[line052->line0414]       }
//@[line052->line0415]     },
  name: 'optionalWithNoParams1'
//@[line053->line0361]       "name": "optionalWithNoParams1",
}

module optionalWithNoParams2 './child/optionalParams.bicep'= {
//@[line056->line0416]     {
//@[line056->line0417]       "type": "Microsoft.Resources/deployments",
//@[line056->line0418]       "apiVersion": "2020-10-01",
//@[line056->line0420]       "properties": {
//@[line056->line0421]         "expressionEvaluationOptions": {
//@[line056->line0422]           "scope": "inner"
//@[line056->line0423]         },
//@[line056->line0424]         "mode": "Incremental",
//@[line056->line0425]         "parameters": {},
//@[line056->line0426]         "template": {
//@[line056->line0427]           "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@[line056->line0428]           "contentVersion": "1.0.0.0",
//@[line056->line0429]           "metadata": {
//@[line056->line0430]             "_generator": {
//@[line056->line0431]               "name": "bicep",
//@[line056->line0432]               "version": "dev",
//@[line056->line0433]               "templateHash": "4191259681487754679"
//@[line056->line0434]             }
//@[line056->line0435]           },
//@[line056->line0436]           "parameters": {
//@[line056->line0437]             "optionalString": {
//@[line056->line0438]               "type": "string",
//@[line056->line0439]               "defaultValue": "abc"
//@[line056->line0440]             },
//@[line056->line0441]             "optionalInt": {
//@[line056->line0442]               "type": "int",
//@[line056->line0443]               "defaultValue": 42
//@[line056->line0444]             },
//@[line056->line0445]             "optionalObj": {
//@[line056->line0446]               "type": "object",
//@[line056->line0447]               "defaultValue": {
//@[line056->line0448]                 "a": "b"
//@[line056->line0449]               }
//@[line056->line0450]             },
//@[line056->line0451]             "optionalArray": {
//@[line056->line0452]               "type": "array",
//@[line056->line0453]               "defaultValue": [
//@[line056->line0454]                 1,
//@[line056->line0455]                 2,
//@[line056->line0456]                 3
//@[line056->line0457]               ]
//@[line056->line0458]             }
//@[line056->line0459]           },
//@[line056->line0460]           "resources": [],
//@[line056->line0461]           "outputs": {
//@[line056->line0462]             "outputObj": {
//@[line056->line0463]               "type": "object",
//@[line056->line0464]               "value": {
//@[line056->line0465]                 "optionalString": "[parameters('optionalString')]",
//@[line056->line0466]                 "optionalInt": "[parameters('optionalInt')]",
//@[line056->line0467]                 "optionalObj": "[parameters('optionalObj')]",
//@[line056->line0468]                 "optionalArray": "[parameters('optionalArray')]"
//@[line056->line0469]               }
//@[line056->line0470]             }
//@[line056->line0471]           }
//@[line056->line0472]         }
//@[line056->line0473]       }
//@[line056->line0474]     },
  name: 'optionalWithNoParams2'
//@[line057->line0419]       "name": "optionalWithNoParams2",
  params: {
  }
}

module optionalWithAllParams './child/optionalParams.bicep'= {
//@[line062->line0475]     {
//@[line062->line0476]       "type": "Microsoft.Resources/deployments",
//@[line062->line0477]       "apiVersion": "2020-10-01",
//@[line062->line0479]       "properties": {
//@[line062->line0480]         "expressionEvaluationOptions": {
//@[line062->line0481]           "scope": "inner"
//@[line062->line0482]         },
//@[line062->line0483]         "mode": "Incremental",
//@[line062->line0484]         "parameters": {
//@[line062->line0485]           "optionalString": {
//@[line062->line0487]           },
//@[line062->line0488]           "optionalInt": {
//@[line062->line0490]           },
//@[line062->line0491]           "optionalObj": {
//@[line062->line0493]           },
//@[line062->line0494]           "optionalArray": {
//@[line062->line0496]           }
//@[line062->line0497]         },
//@[line062->line0498]         "template": {
//@[line062->line0499]           "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@[line062->line0500]           "contentVersion": "1.0.0.0",
//@[line062->line0501]           "metadata": {
//@[line062->line0502]             "_generator": {
//@[line062->line0503]               "name": "bicep",
//@[line062->line0504]               "version": "dev",
//@[line062->line0505]               "templateHash": "4191259681487754679"
//@[line062->line0506]             }
//@[line062->line0507]           },
//@[line062->line0508]           "parameters": {
//@[line062->line0509]             "optionalString": {
//@[line062->line0510]               "type": "string",
//@[line062->line0511]               "defaultValue": "abc"
//@[line062->line0512]             },
//@[line062->line0513]             "optionalInt": {
//@[line062->line0514]               "type": "int",
//@[line062->line0515]               "defaultValue": 42
//@[line062->line0516]             },
//@[line062->line0517]             "optionalObj": {
//@[line062->line0518]               "type": "object",
//@[line062->line0519]               "defaultValue": {
//@[line062->line0520]                 "a": "b"
//@[line062->line0521]               }
//@[line062->line0522]             },
//@[line062->line0523]             "optionalArray": {
//@[line062->line0524]               "type": "array",
//@[line062->line0525]               "defaultValue": [
//@[line062->line0526]                 1,
//@[line062->line0527]                 2,
//@[line062->line0528]                 3
//@[line062->line0529]               ]
//@[line062->line0530]             }
//@[line062->line0531]           },
//@[line062->line0532]           "resources": [],
//@[line062->line0533]           "outputs": {
//@[line062->line0534]             "outputObj": {
//@[line062->line0535]               "type": "object",
//@[line062->line0536]               "value": {
//@[line062->line0537]                 "optionalString": "[parameters('optionalString')]",
//@[line062->line0538]                 "optionalInt": "[parameters('optionalInt')]",
//@[line062->line0539]                 "optionalObj": "[parameters('optionalObj')]",
//@[line062->line0540]                 "optionalArray": "[parameters('optionalArray')]"
//@[line062->line0541]               }
//@[line062->line0542]             }
//@[line062->line0543]           }
//@[line062->line0544]         }
//@[line062->line0545]       }
//@[line062->line0546]     },
  name: 'optionalWithNoParams3'
//@[line063->line0478]       "name": "optionalWithNoParams3",
  params: {
    optionalString: 'abc'
//@[line065->line0486]             "value": "abc"
    optionalInt: 42
//@[line066->line0489]             "value": 42
    optionalObj: { }
//@[line067->line0492]             "value": {}
    optionalArray: [ ]
//@[line068->line0495]             "value": []
  }
}

resource resWithDependencies 'Mock.Rp/mockResource@2020-01-01' = {
//@[line072->line0058]     {
//@[line072->line0059]       "type": "Mock.Rp/mockResource",
//@[line072->line0060]       "apiVersion": "2020-01-01",
//@[line072->line0061]       "name": "harry",
//@[line072->line0067]       "dependsOn": [
//@[line072->line0068]         "[resourceId('Microsoft.Resources/deployments', 'modATest')]",
//@[line072->line0069]         "[resourceId('Microsoft.Resources/deployments', 'modB')]",
//@[line072->line0070]         "[resourceId('Microsoft.Resources/deployments', 'modC')]"
//@[line072->line0071]       ]
//@[line072->line0072]     },
  name: 'harry'
  properties: {
//@[line074->line0062]       "properties": {
//@[line074->line0066]       },
    modADep: modATest.outputs.stringOutputA
//@[line075->line0063]         "modADep": "[reference(resourceId('Microsoft.Resources/deployments', 'modATest'), '2020-10-01').outputs.stringOutputA.value]",
    modBDep: modB.outputs.myResourceId
//@[line076->line0064]         "modBDep": "[reference(resourceId('Microsoft.Resources/deployments', 'modB'), '2020-10-01').outputs.myResourceId.value]",
    modCDep: modC.outputs.myResourceId
//@[line077->line0065]         "modCDep": "[reference(resourceId('Microsoft.Resources/deployments', 'modC'), '2020-10-01').outputs.myResourceId.value]"
  }
}

module optionalWithAllParamsAndManualDependency './child/optionalParams.bicep'= {
//@[line081->line0547]     {
//@[line081->line0548]       "type": "Microsoft.Resources/deployments",
//@[line081->line0549]       "apiVersion": "2020-10-01",
//@[line081->line0551]       "properties": {
//@[line081->line0552]         "expressionEvaluationOptions": {
//@[line081->line0553]           "scope": "inner"
//@[line081->line0554]         },
//@[line081->line0555]         "mode": "Incremental",
//@[line081->line0556]         "parameters": {
//@[line081->line0557]           "optionalString": {
//@[line081->line0559]           },
//@[line081->line0560]           "optionalInt": {
//@[line081->line0562]           },
//@[line081->line0563]           "optionalObj": {
//@[line081->line0565]           },
//@[line081->line0566]           "optionalArray": {
//@[line081->line0568]           }
//@[line081->line0569]         },
//@[line081->line0570]         "template": {
//@[line081->line0571]           "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@[line081->line0572]           "contentVersion": "1.0.0.0",
//@[line081->line0573]           "metadata": {
//@[line081->line0574]             "_generator": {
//@[line081->line0575]               "name": "bicep",
//@[line081->line0576]               "version": "dev",
//@[line081->line0577]               "templateHash": "4191259681487754679"
//@[line081->line0578]             }
//@[line081->line0579]           },
//@[line081->line0580]           "parameters": {
//@[line081->line0581]             "optionalString": {
//@[line081->line0582]               "type": "string",
//@[line081->line0583]               "defaultValue": "abc"
//@[line081->line0584]             },
//@[line081->line0585]             "optionalInt": {
//@[line081->line0586]               "type": "int",
//@[line081->line0587]               "defaultValue": 42
//@[line081->line0588]             },
//@[line081->line0589]             "optionalObj": {
//@[line081->line0590]               "type": "object",
//@[line081->line0591]               "defaultValue": {
//@[line081->line0592]                 "a": "b"
//@[line081->line0593]               }
//@[line081->line0594]             },
//@[line081->line0595]             "optionalArray": {
//@[line081->line0596]               "type": "array",
//@[line081->line0597]               "defaultValue": [
//@[line081->line0598]                 1,
//@[line081->line0599]                 2,
//@[line081->line0600]                 3
//@[line081->line0601]               ]
//@[line081->line0602]             }
//@[line081->line0603]           },
//@[line081->line0604]           "resources": [],
//@[line081->line0605]           "outputs": {
//@[line081->line0606]             "outputObj": {
//@[line081->line0607]               "type": "object",
//@[line081->line0608]               "value": {
//@[line081->line0609]                 "optionalString": "[parameters('optionalString')]",
//@[line081->line0610]                 "optionalInt": "[parameters('optionalInt')]",
//@[line081->line0611]                 "optionalObj": "[parameters('optionalObj')]",
//@[line081->line0612]                 "optionalArray": "[parameters('optionalArray')]"
//@[line081->line0613]               }
//@[line081->line0614]             }
//@[line081->line0615]           }
//@[line081->line0616]         }
//@[line081->line0617]       },
//@[line081->line0618]       "dependsOn": [
//@[line081->line0619]         "[resourceId('Microsoft.Resources/deployments', 'optionalWithNoParams3')]",
//@[line081->line0620]         "[resourceId('Mock.Rp/mockResource', 'harry')]"
//@[line081->line0621]       ]
//@[line081->line0622]     },
  name: 'optionalWithAllParamsAndManualDependency'
//@[line082->line0550]       "name": "optionalWithAllParamsAndManualDependency",
  params: {
    optionalString: 'abc'
//@[line084->line0558]             "value": "abc"
    optionalInt: 42
//@[line085->line0561]             "value": 42
    optionalObj: { }
//@[line086->line0564]             "value": {}
    optionalArray: [ ]
//@[line087->line0567]             "value": []
  }
  dependsOn: [
    resWithDependencies
    optionalWithAllParams
  ]
}

module optionalWithImplicitDependency './child/optionalParams.bicep'= {
//@[line095->line0623]     {
//@[line095->line0624]       "type": "Microsoft.Resources/deployments",
//@[line095->line0625]       "apiVersion": "2020-10-01",
//@[line095->line0627]       "properties": {
//@[line095->line0628]         "expressionEvaluationOptions": {
//@[line095->line0629]           "scope": "inner"
//@[line095->line0630]         },
//@[line095->line0631]         "mode": "Incremental",
//@[line095->line0632]         "parameters": {
//@[line095->line0633]           "optionalString": {
//@[line095->line0635]           },
//@[line095->line0636]           "optionalInt": {
//@[line095->line0638]           },
//@[line095->line0639]           "optionalObj": {
//@[line095->line0641]           },
//@[line095->line0642]           "optionalArray": {
//@[line095->line0644]           }
//@[line095->line0645]         },
//@[line095->line0646]         "template": {
//@[line095->line0647]           "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@[line095->line0648]           "contentVersion": "1.0.0.0",
//@[line095->line0649]           "metadata": {
//@[line095->line0650]             "_generator": {
//@[line095->line0651]               "name": "bicep",
//@[line095->line0652]               "version": "dev",
//@[line095->line0653]               "templateHash": "4191259681487754679"
//@[line095->line0654]             }
//@[line095->line0655]           },
//@[line095->line0656]           "parameters": {
//@[line095->line0657]             "optionalString": {
//@[line095->line0658]               "type": "string",
//@[line095->line0659]               "defaultValue": "abc"
//@[line095->line0660]             },
//@[line095->line0661]             "optionalInt": {
//@[line095->line0662]               "type": "int",
//@[line095->line0663]               "defaultValue": 42
//@[line095->line0664]             },
//@[line095->line0665]             "optionalObj": {
//@[line095->line0666]               "type": "object",
//@[line095->line0667]               "defaultValue": {
//@[line095->line0668]                 "a": "b"
//@[line095->line0669]               }
//@[line095->line0670]             },
//@[line095->line0671]             "optionalArray": {
//@[line095->line0672]               "type": "array",
//@[line095->line0673]               "defaultValue": [
//@[line095->line0674]                 1,
//@[line095->line0675]                 2,
//@[line095->line0676]                 3
//@[line095->line0677]               ]
//@[line095->line0678]             }
//@[line095->line0679]           },
//@[line095->line0680]           "resources": [],
//@[line095->line0681]           "outputs": {
//@[line095->line0682]             "outputObj": {
//@[line095->line0683]               "type": "object",
//@[line095->line0684]               "value": {
//@[line095->line0685]                 "optionalString": "[parameters('optionalString')]",
//@[line095->line0686]                 "optionalInt": "[parameters('optionalInt')]",
//@[line095->line0687]                 "optionalObj": "[parameters('optionalObj')]",
//@[line095->line0688]                 "optionalArray": "[parameters('optionalArray')]"
//@[line095->line0689]               }
//@[line095->line0690]             }
//@[line095->line0691]           }
//@[line095->line0692]         }
//@[line095->line0693]       },
//@[line095->line0694]       "dependsOn": [
//@[line095->line0695]         "[resourceId('Microsoft.Resources/deployments', 'optionalWithAllParamsAndManualDependency')]",
//@[line095->line0696]         "[resourceId('Mock.Rp/mockResource', 'harry')]"
//@[line095->line0697]       ]
//@[line095->line0698]     },
  name: 'optionalWithImplicitDependency'
//@[line096->line0626]       "name": "optionalWithImplicitDependency",
  params: {
    optionalString: concat(resWithDependencies.id, optionalWithAllParamsAndManualDependency.name)
//@[line098->line0634]             "value": "[concat(resourceId('Mock.Rp/mockResource', 'harry'), 'optionalWithAllParamsAndManualDependency')]"
    optionalInt: 42
//@[line099->line0637]             "value": 42
    optionalObj: { }
//@[line100->line0640]             "value": {}
    optionalArray: [ ]
//@[line101->line0643]             "value": []
  }
}

module moduleWithCalculatedName './child/optionalParams.bicep'= {
//@[line105->line0699]     {
//@[line105->line0700]       "type": "Microsoft.Resources/deployments",
//@[line105->line0701]       "apiVersion": "2020-10-01",
//@[line105->line0703]       "properties": {
//@[line105->line0704]         "expressionEvaluationOptions": {
//@[line105->line0705]           "scope": "inner"
//@[line105->line0706]         },
//@[line105->line0707]         "mode": "Incremental",
//@[line105->line0708]         "parameters": {
//@[line105->line0709]           "optionalString": {
//@[line105->line0711]           },
//@[line105->line0712]           "optionalInt": {
//@[line105->line0714]           },
//@[line105->line0715]           "optionalObj": {
//@[line105->line0717]           },
//@[line105->line0718]           "optionalArray": {
//@[line105->line0720]           }
//@[line105->line0721]         },
//@[line105->line0722]         "template": {
//@[line105->line0723]           "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@[line105->line0724]           "contentVersion": "1.0.0.0",
//@[line105->line0725]           "metadata": {
//@[line105->line0726]             "_generator": {
//@[line105->line0727]               "name": "bicep",
//@[line105->line0728]               "version": "dev",
//@[line105->line0729]               "templateHash": "4191259681487754679"
//@[line105->line0730]             }
//@[line105->line0731]           },
//@[line105->line0732]           "parameters": {
//@[line105->line0733]             "optionalString": {
//@[line105->line0734]               "type": "string",
//@[line105->line0735]               "defaultValue": "abc"
//@[line105->line0736]             },
//@[line105->line0737]             "optionalInt": {
//@[line105->line0738]               "type": "int",
//@[line105->line0739]               "defaultValue": 42
//@[line105->line0740]             },
//@[line105->line0741]             "optionalObj": {
//@[line105->line0742]               "type": "object",
//@[line105->line0743]               "defaultValue": {
//@[line105->line0744]                 "a": "b"
//@[line105->line0745]               }
//@[line105->line0746]             },
//@[line105->line0747]             "optionalArray": {
//@[line105->line0748]               "type": "array",
//@[line105->line0749]               "defaultValue": [
//@[line105->line0750]                 1,
//@[line105->line0751]                 2,
//@[line105->line0752]                 3
//@[line105->line0753]               ]
//@[line105->line0754]             }
//@[line105->line0755]           },
//@[line105->line0756]           "resources": [],
//@[line105->line0757]           "outputs": {
//@[line105->line0758]             "outputObj": {
//@[line105->line0759]               "type": "object",
//@[line105->line0760]               "value": {
//@[line105->line0761]                 "optionalString": "[parameters('optionalString')]",
//@[line105->line0762]                 "optionalInt": "[parameters('optionalInt')]",
//@[line105->line0763]                 "optionalObj": "[parameters('optionalObj')]",
//@[line105->line0764]                 "optionalArray": "[parameters('optionalArray')]"
//@[line105->line0765]               }
//@[line105->line0766]             }
//@[line105->line0767]           }
//@[line105->line0768]         }
//@[line105->line0769]       },
//@[line105->line0770]       "dependsOn": [
//@[line105->line0771]         "[resourceId('Microsoft.Resources/deployments', 'optionalWithAllParamsAndManualDependency')]",
//@[line105->line0772]         "[resourceId('Mock.Rp/mockResource', 'harry')]"
//@[line105->line0773]       ]
//@[line105->line0774]     },
  name: '${optionalWithAllParamsAndManualDependency.name}${deployTimeSuffix}'
//@[line106->line0702]       "name": "[format('{0}{1}', 'optionalWithAllParamsAndManualDependency', parameters('deployTimeSuffix'))]",
  params: {
    optionalString: concat(resWithDependencies.id, optionalWithAllParamsAndManualDependency.name)
//@[line108->line0710]             "value": "[concat(resourceId('Mock.Rp/mockResource', 'harry'), 'optionalWithAllParamsAndManualDependency')]"
    optionalInt: 42
//@[line109->line0713]             "value": 42
    optionalObj: { }
//@[line110->line0716]             "value": {}
    optionalArray: [ ]
//@[line111->line0719]             "value": []
  }
}

resource resWithCalculatedNameDependencies 'Mock.Rp/mockResource@2020-01-01' = {
//@[line115->line0073]     {
//@[line115->line0074]       "type": "Mock.Rp/mockResource",
//@[line115->line0075]       "apiVersion": "2020-01-01",
//@[line115->line0076]       "name": "[format('{0}{1}', 'optionalWithAllParamsAndManualDependency', parameters('deployTimeSuffix'))]",
//@[line115->line0080]       "dependsOn": [
//@[line115->line0081]         "[resourceId('Microsoft.Resources/deployments', format('{0}{1}', 'optionalWithAllParamsAndManualDependency', parameters('deployTimeSuffix')))]",
//@[line115->line0082]         "[resourceId('Microsoft.Resources/deployments', 'optionalWithAllParamsAndManualDependency')]"
//@[line115->line0083]       ]
//@[line115->line0084]     },
  name: '${optionalWithAllParamsAndManualDependency.name}${deployTimeSuffix}'
  properties: {
//@[line117->line0077]       "properties": {
//@[line117->line0079]       },
    modADep: moduleWithCalculatedName.outputs.outputObj
//@[line118->line0078]         "modADep": "[reference(resourceId('Microsoft.Resources/deployments', format('{0}{1}', 'optionalWithAllParamsAndManualDependency', parameters('deployTimeSuffix'))), '2020-10-01').outputs.outputObj.value]"
  }
}

output stringOutputA string = modATest.outputs.stringOutputA
//@[line122->line2027]     "stringOutputA": {
//@[line122->line2028]       "type": "string",
//@[line122->line2029]       "value": "[reference(resourceId('Microsoft.Resources/deployments', 'modATest'), '2020-10-01').outputs.stringOutputA.value]"
//@[line122->line2030]     },
output stringOutputB string = modATest.outputs.stringOutputB
//@[line123->line2031]     "stringOutputB": {
//@[line123->line2032]       "type": "string",
//@[line123->line2033]       "value": "[reference(resourceId('Microsoft.Resources/deployments', 'modATest'), '2020-10-01').outputs.stringOutputB.value]"
//@[line123->line2034]     },
output objOutput object = modATest.outputs.objOutput
//@[line124->line2035]     "objOutput": {
//@[line124->line2036]       "type": "object",
//@[line124->line2037]       "value": "[reference(resourceId('Microsoft.Resources/deployments', 'modATest'), '2020-10-01').outputs.objOutput.value]"
//@[line124->line2038]     },
output arrayOutput array = modATest.outputs.arrayOutput
//@[line125->line2039]     "arrayOutput": {
//@[line125->line2040]       "type": "array",
//@[line125->line2041]       "value": "[reference(resourceId('Microsoft.Resources/deployments', 'modATest'), '2020-10-01').outputs.arrayOutput.value]"
//@[line125->line2042]     },
output modCalculatedNameOutput object = moduleWithCalculatedName.outputs.outputObj
//@[line126->line2043]     "modCalculatedNameOutput": {
//@[line126->line2044]       "type": "object",
//@[line126->line2045]       "value": "[reference(resourceId('Microsoft.Resources/deployments', format('{0}{1}', 'optionalWithAllParamsAndManualDependency', parameters('deployTimeSuffix'))), '2020-10-01').outputs.outputObj.value]"
//@[line126->line2046]     }

/*
  valid loop cases
*/

@sys.description('this is myModules')
var myModules = [
//@[line133->line0020]     "myModules": [
//@[line133->line0029]     ],
  {
//@[line134->line0021]       {
//@[line134->line0024]       },
    name: 'one'
//@[line135->line0022]         "name": "one",
    location: 'eastus2'
//@[line136->line0023]         "location": "eastus2"
  }
  {
//@[line138->line0025]       {
//@[line138->line0028]       }
    name: 'two'
//@[line139->line0026]         "name": "two",
    location: 'westus'
//@[line140->line0027]         "location": "westus"
  }
]

var emptyArray = []
//@[line144->line0030]     "emptyArray": [],

// simple module loop
module storageResources 'modulea.bicep' = [for module in myModules: {
//@[line147->line0775]     {
//@[line147->line0776]       "copy": {
//@[line147->line0777]         "name": "storageResources",
//@[line147->line0778]         "count": "[length(variables('myModules'))]"
//@[line147->line0779]       },
//@[line147->line0780]       "type": "Microsoft.Resources/deployments",
//@[line147->line0781]       "apiVersion": "2020-10-01",
//@[line147->line0783]       "properties": {
//@[line147->line0784]         "expressionEvaluationOptions": {
//@[line147->line0785]           "scope": "inner"
//@[line147->line0786]         },
//@[line147->line0787]         "mode": "Incremental",
//@[line147->line0788]         "parameters": {
//@[line147->line0789]           "arrayParam": {
//@[line147->line0791]           },
//@[line147->line0792]           "objParam": {
//@[line147->line0794]           },
//@[line147->line0795]           "stringParamB": {
//@[line147->line0797]           }
//@[line147->line0798]         },
//@[line147->line0799]         "template": {
//@[line147->line0800]           "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@[line147->line0801]           "contentVersion": "1.0.0.0",
//@[line147->line0802]           "metadata": {
//@[line147->line0803]             "_generator": {
//@[line147->line0804]               "name": "bicep",
//@[line147->line0805]               "version": "dev",
//@[line147->line0806]               "templateHash": "8300391961099598421"
//@[line147->line0807]             }
//@[line147->line0808]           },
//@[line147->line0809]           "parameters": {
//@[line147->line0810]             "stringParamA": {
//@[line147->line0811]               "type": "string",
//@[line147->line0812]               "defaultValue": "test"
//@[line147->line0813]             },
//@[line147->line0814]             "stringParamB": {
//@[line147->line0815]               "type": "string"
//@[line147->line0816]             },
//@[line147->line0817]             "objParam": {
//@[line147->line0818]               "type": "object"
//@[line147->line0819]             },
//@[line147->line0820]             "arrayParam": {
//@[line147->line0821]               "type": "array"
//@[line147->line0822]             }
//@[line147->line0823]           },
//@[line147->line0824]           "resources": [
//@[line147->line0825]             {
//@[line147->line0826]               "type": "Mock.Rp/mockResource",
//@[line147->line0827]               "apiVersion": "2020-01-01",
//@[line147->line0828]               "name": "basicblobs",
//@[line147->line0829]               "location": "[parameters('stringParamA')]"
//@[line147->line0830]             },
//@[line147->line0831]             {
//@[line147->line0832]               "type": "Mock.Rp/mockResource",
//@[line147->line0833]               "apiVersion": "2020-01-01",
//@[line147->line0834]               "name": "myZone",
//@[line147->line0835]               "location": "[parameters('stringParamB')]"
//@[line147->line0836]             }
//@[line147->line0837]           ],
//@[line147->line0838]           "outputs": {
//@[line147->line0839]             "stringOutputA": {
//@[line147->line0840]               "type": "string",
//@[line147->line0841]               "value": "[parameters('stringParamA')]"
//@[line147->line0842]             },
//@[line147->line0843]             "stringOutputB": {
//@[line147->line0844]               "type": "string",
//@[line147->line0845]               "value": "[parameters('stringParamB')]"
//@[line147->line0846]             },
//@[line147->line0847]             "objOutput": {
//@[line147->line0848]               "type": "object",
//@[line147->line0849]               "value": "[reference(resourceId('Mock.Rp/mockResource', 'basicblobs'), '2020-01-01')]"
//@[line147->line0850]             },
//@[line147->line0851]             "arrayOutput": {
//@[line147->line0852]               "type": "array",
//@[line147->line0853]               "value": [
//@[line147->line0854]                 "[resourceId('Mock.Rp/mockResource', 'basicblobs')]",
//@[line147->line0855]                 "[resourceId('Mock.Rp/mockResource', 'myZone')]"
//@[line147->line0856]               ]
//@[line147->line0857]             }
//@[line147->line0858]           }
//@[line147->line0859]         }
//@[line147->line0860]       }
//@[line147->line0861]     },
  name: module.name
//@[line148->line0782]       "name": "[variables('myModules')[copyIndex()].name]",
  params: {
    arrayParam: []
//@[line150->line0790]             "value": []
    objParam: module
//@[line151->line0793]             "value": "[variables('myModules')[copyIndex()]]"
    stringParamB: module.location
//@[line152->line0796]             "value": "[variables('myModules')[copyIndex()].location]"
  }
}]

// simple indexed module loop
module storageResourcesWithIndex 'modulea.bicep' = [for (module, i) in myModules: {
//@[line157->line0862]     {
//@[line157->line0863]       "copy": {
//@[line157->line0864]         "name": "storageResourcesWithIndex",
//@[line157->line0865]         "count": "[length(variables('myModules'))]"
//@[line157->line0866]       },
//@[line157->line0867]       "type": "Microsoft.Resources/deployments",
//@[line157->line0868]       "apiVersion": "2020-10-01",
//@[line157->line0870]       "properties": {
//@[line157->line0871]         "expressionEvaluationOptions": {
//@[line157->line0872]           "scope": "inner"
//@[line157->line0873]         },
//@[line157->line0874]         "mode": "Incremental",
//@[line157->line0875]         "parameters": {
//@[line157->line0876]           "arrayParam": {
//@[line157->line0880]           },
//@[line157->line0881]           "objParam": {
//@[line157->line0883]           },
//@[line157->line0884]           "stringParamB": {
//@[line157->line0886]           },
//@[line157->line0887]           "stringParamA": {
//@[line157->line0889]           }
//@[line157->line0890]         },
//@[line157->line0891]         "template": {
//@[line157->line0892]           "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@[line157->line0893]           "contentVersion": "1.0.0.0",
//@[line157->line0894]           "metadata": {
//@[line157->line0895]             "_generator": {
//@[line157->line0896]               "name": "bicep",
//@[line157->line0897]               "version": "dev",
//@[line157->line0898]               "templateHash": "8300391961099598421"
//@[line157->line0899]             }
//@[line157->line0900]           },
//@[line157->line0901]           "parameters": {
//@[line157->line0902]             "stringParamA": {
//@[line157->line0903]               "type": "string",
//@[line157->line0904]               "defaultValue": "test"
//@[line157->line0905]             },
//@[line157->line0906]             "stringParamB": {
//@[line157->line0907]               "type": "string"
//@[line157->line0908]             },
//@[line157->line0909]             "objParam": {
//@[line157->line0910]               "type": "object"
//@[line157->line0911]             },
//@[line157->line0912]             "arrayParam": {
//@[line157->line0913]               "type": "array"
//@[line157->line0914]             }
//@[line157->line0915]           },
//@[line157->line0916]           "resources": [
//@[line157->line0917]             {
//@[line157->line0918]               "type": "Mock.Rp/mockResource",
//@[line157->line0919]               "apiVersion": "2020-01-01",
//@[line157->line0920]               "name": "basicblobs",
//@[line157->line0921]               "location": "[parameters('stringParamA')]"
//@[line157->line0922]             },
//@[line157->line0923]             {
//@[line157->line0924]               "type": "Mock.Rp/mockResource",
//@[line157->line0925]               "apiVersion": "2020-01-01",
//@[line157->line0926]               "name": "myZone",
//@[line157->line0927]               "location": "[parameters('stringParamB')]"
//@[line157->line0928]             }
//@[line157->line0929]           ],
//@[line157->line0930]           "outputs": {
//@[line157->line0931]             "stringOutputA": {
//@[line157->line0932]               "type": "string",
//@[line157->line0933]               "value": "[parameters('stringParamA')]"
//@[line157->line0934]             },
//@[line157->line0935]             "stringOutputB": {
//@[line157->line0936]               "type": "string",
//@[line157->line0937]               "value": "[parameters('stringParamB')]"
//@[line157->line0938]             },
//@[line157->line0939]             "objOutput": {
//@[line157->line0940]               "type": "object",
//@[line157->line0941]               "value": "[reference(resourceId('Mock.Rp/mockResource', 'basicblobs'), '2020-01-01')]"
//@[line157->line0942]             },
//@[line157->line0943]             "arrayOutput": {
//@[line157->line0944]               "type": "array",
//@[line157->line0945]               "value": [
//@[line157->line0946]                 "[resourceId('Mock.Rp/mockResource', 'basicblobs')]",
//@[line157->line0947]                 "[resourceId('Mock.Rp/mockResource', 'myZone')]"
//@[line157->line0948]               ]
//@[line157->line0949]             }
//@[line157->line0950]           }
//@[line157->line0951]         }
//@[line157->line0952]       }
//@[line157->line0953]     },
  name: module.name
//@[line158->line0869]       "name": "[variables('myModules')[copyIndex()].name]",
  params: {
    arrayParam: [
//@[line160->line0877]             "value": [
//@[line160->line0879]             ]
      i + 1
//@[line161->line0878]               "[add(copyIndex(), 1)]"
    ]
    objParam: module
//@[line163->line0882]             "value": "[variables('myModules')[copyIndex()]]"
    stringParamB: module.location
//@[line164->line0885]             "value": "[variables('myModules')[copyIndex()].location]"
    stringParamA: concat('a', i)
//@[line165->line0888]             "value": "[concat('a', copyIndex())]"
  }
}]

// nested module loop
module nestedModuleLoop 'modulea.bicep' = [for module in myModules: {
//@[line170->line0954]     {
//@[line170->line0955]       "copy": {
//@[line170->line0956]         "name": "nestedModuleLoop",
//@[line170->line0957]         "count": "[length(variables('myModules'))]"
//@[line170->line0958]       },
//@[line170->line0959]       "type": "Microsoft.Resources/deployments",
//@[line170->line0960]       "apiVersion": "2020-10-01",
//@[line170->line0962]       "properties": {
//@[line170->line0963]         "expressionEvaluationOptions": {
//@[line170->line0964]           "scope": "inner"
//@[line170->line0965]         },
//@[line170->line0966]         "mode": "Incremental",
//@[line170->line0967]         "parameters": {
//@[line170->line0968]           "arrayParam": {
//@[line170->line0969]             "copy": [
//@[line170->line0975]             ]
//@[line170->line0976]           },
//@[line170->line0977]           "objParam": {
//@[line170->line0979]           },
//@[line170->line0980]           "stringParamB": {
//@[line170->line0982]           }
//@[line170->line0983]         },
//@[line170->line0984]         "template": {
//@[line170->line0985]           "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@[line170->line0986]           "contentVersion": "1.0.0.0",
//@[line170->line0987]           "metadata": {
//@[line170->line0988]             "_generator": {
//@[line170->line0989]               "name": "bicep",
//@[line170->line0990]               "version": "dev",
//@[line170->line0991]               "templateHash": "8300391961099598421"
//@[line170->line0992]             }
//@[line170->line0993]           },
//@[line170->line0994]           "parameters": {
//@[line170->line0995]             "stringParamA": {
//@[line170->line0996]               "type": "string",
//@[line170->line0997]               "defaultValue": "test"
//@[line170->line0998]             },
//@[line170->line0999]             "stringParamB": {
//@[line170->line1000]               "type": "string"
//@[line170->line1001]             },
//@[line170->line1002]             "objParam": {
//@[line170->line1003]               "type": "object"
//@[line170->line1004]             },
//@[line170->line1005]             "arrayParam": {
//@[line170->line1006]               "type": "array"
//@[line170->line1007]             }
//@[line170->line1008]           },
//@[line170->line1009]           "resources": [
//@[line170->line1010]             {
//@[line170->line1011]               "type": "Mock.Rp/mockResource",
//@[line170->line1012]               "apiVersion": "2020-01-01",
//@[line170->line1013]               "name": "basicblobs",
//@[line170->line1014]               "location": "[parameters('stringParamA')]"
//@[line170->line1015]             },
//@[line170->line1016]             {
//@[line170->line1017]               "type": "Mock.Rp/mockResource",
//@[line170->line1018]               "apiVersion": "2020-01-01",
//@[line170->line1019]               "name": "myZone",
//@[line170->line1020]               "location": "[parameters('stringParamB')]"
//@[line170->line1021]             }
//@[line170->line1022]           ],
//@[line170->line1023]           "outputs": {
//@[line170->line1024]             "stringOutputA": {
//@[line170->line1025]               "type": "string",
//@[line170->line1026]               "value": "[parameters('stringParamA')]"
//@[line170->line1027]             },
//@[line170->line1028]             "stringOutputB": {
//@[line170->line1029]               "type": "string",
//@[line170->line1030]               "value": "[parameters('stringParamB')]"
//@[line170->line1031]             },
//@[line170->line1032]             "objOutput": {
//@[line170->line1033]               "type": "object",
//@[line170->line1034]               "value": "[reference(resourceId('Mock.Rp/mockResource', 'basicblobs'), '2020-01-01')]"
//@[line170->line1035]             },
//@[line170->line1036]             "arrayOutput": {
//@[line170->line1037]               "type": "array",
//@[line170->line1038]               "value": [
//@[line170->line1039]                 "[resourceId('Mock.Rp/mockResource', 'basicblobs')]",
//@[line170->line1040]                 "[resourceId('Mock.Rp/mockResource', 'myZone')]"
//@[line170->line1041]               ]
//@[line170->line1042]             }
//@[line170->line1043]           }
//@[line170->line1044]         }
//@[line170->line1045]       }
//@[line170->line1046]     },
  name: module.name
//@[line171->line0961]       "name": "[variables('myModules')[copyIndex()].name]",
  params: {
    arrayParam: [for i in range(0,3): concat('test-', i, '-', module.name)]
//@[line173->line0970]               {
//@[line173->line0971]                 "name": "value",
//@[line173->line0972]                 "count": "[length(range(0, 3))]",
//@[line173->line0973]                 "input": "[concat('test-', range(0, 3)[copyIndex('value')], '-', variables('myModules')[copyIndex()].name)]"
//@[line173->line0974]               }
    objParam: module
//@[line174->line0978]             "value": "[variables('myModules')[copyIndex()]]"
    stringParamB: module.location
//@[line175->line0981]             "value": "[variables('myModules')[copyIndex()].location]"
  }
}]

// duplicate identifiers across scopes are allowed (inner hides the outer)
module duplicateIdentifiersWithinLoop 'modulea.bicep' = [for x in emptyArray:{
//@[line180->line1047]     {
//@[line180->line1048]       "copy": {
//@[line180->line1049]         "name": "duplicateIdentifiersWithinLoop",
//@[line180->line1050]         "count": "[length(variables('emptyArray'))]"
//@[line180->line1051]       },
//@[line180->line1052]       "type": "Microsoft.Resources/deployments",
//@[line180->line1053]       "apiVersion": "2020-10-01",
//@[line180->line1055]       "properties": {
//@[line180->line1056]         "expressionEvaluationOptions": {
//@[line180->line1057]           "scope": "inner"
//@[line180->line1058]         },
//@[line180->line1059]         "mode": "Incremental",
//@[line180->line1060]         "parameters": {
//@[line180->line1061]           "objParam": {
//@[line180->line1063]           },
//@[line180->line1064]           "stringParamA": {
//@[line180->line1066]           },
//@[line180->line1067]           "stringParamB": {
//@[line180->line1069]           },
//@[line180->line1070]           "arrayParam": {
//@[line180->line1071]             "copy": [
//@[line180->line1077]             ]
//@[line180->line1078]           }
//@[line180->line1079]         },
//@[line180->line1080]         "template": {
//@[line180->line1081]           "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@[line180->line1082]           "contentVersion": "1.0.0.0",
//@[line180->line1083]           "metadata": {
//@[line180->line1084]             "_generator": {
//@[line180->line1085]               "name": "bicep",
//@[line180->line1086]               "version": "dev",
//@[line180->line1087]               "templateHash": "8300391961099598421"
//@[line180->line1088]             }
//@[line180->line1089]           },
//@[line180->line1090]           "parameters": {
//@[line180->line1091]             "stringParamA": {
//@[line180->line1092]               "type": "string",
//@[line180->line1093]               "defaultValue": "test"
//@[line180->line1094]             },
//@[line180->line1095]             "stringParamB": {
//@[line180->line1096]               "type": "string"
//@[line180->line1097]             },
//@[line180->line1098]             "objParam": {
//@[line180->line1099]               "type": "object"
//@[line180->line1100]             },
//@[line180->line1101]             "arrayParam": {
//@[line180->line1102]               "type": "array"
//@[line180->line1103]             }
//@[line180->line1104]           },
//@[line180->line1105]           "resources": [
//@[line180->line1106]             {
//@[line180->line1107]               "type": "Mock.Rp/mockResource",
//@[line180->line1108]               "apiVersion": "2020-01-01",
//@[line180->line1109]               "name": "basicblobs",
//@[line180->line1110]               "location": "[parameters('stringParamA')]"
//@[line180->line1111]             },
//@[line180->line1112]             {
//@[line180->line1113]               "type": "Mock.Rp/mockResource",
//@[line180->line1114]               "apiVersion": "2020-01-01",
//@[line180->line1115]               "name": "myZone",
//@[line180->line1116]               "location": "[parameters('stringParamB')]"
//@[line180->line1117]             }
//@[line180->line1118]           ],
//@[line180->line1119]           "outputs": {
//@[line180->line1120]             "stringOutputA": {
//@[line180->line1121]               "type": "string",
//@[line180->line1122]               "value": "[parameters('stringParamA')]"
//@[line180->line1123]             },
//@[line180->line1124]             "stringOutputB": {
//@[line180->line1125]               "type": "string",
//@[line180->line1126]               "value": "[parameters('stringParamB')]"
//@[line180->line1127]             },
//@[line180->line1128]             "objOutput": {
//@[line180->line1129]               "type": "object",
//@[line180->line1130]               "value": "[reference(resourceId('Mock.Rp/mockResource', 'basicblobs'), '2020-01-01')]"
//@[line180->line1131]             },
//@[line180->line1132]             "arrayOutput": {
//@[line180->line1133]               "type": "array",
//@[line180->line1134]               "value": [
//@[line180->line1135]                 "[resourceId('Mock.Rp/mockResource', 'basicblobs')]",
//@[line180->line1136]                 "[resourceId('Mock.Rp/mockResource', 'myZone')]"
//@[line180->line1137]               ]
//@[line180->line1138]             }
//@[line180->line1139]           }
//@[line180->line1140]         }
//@[line180->line1141]       }
//@[line180->line1142]     },
  name: 'hello-${x}'
//@[line181->line1054]       "name": "[format('hello-{0}', variables('emptyArray')[copyIndex()])]",
  params: {
    objParam: {}
//@[line183->line1062]             "value": {}
    stringParamA: 'test'
//@[line184->line1065]             "value": "test"
    stringParamB: 'test'
//@[line185->line1068]             "value": "test"
    arrayParam: [for x in emptyArray: x]
//@[line186->line1072]               {
//@[line186->line1073]                 "name": "value",
//@[line186->line1074]                 "count": "[length(variables('emptyArray'))]",
//@[line186->line1075]                 "input": "[variables('emptyArray')[copyIndex('value')]]"
//@[line186->line1076]               }
  }
}]

// duplicate identifiers across scopes are allowed (inner hides the outer)
var duplicateAcrossScopes = 'hello'
//@[line191->line0031]     "duplicateAcrossScopes": "hello",
module duplicateInGlobalAndOneLoop 'modulea.bicep' = [for duplicateAcrossScopes in []: {
//@[line192->line1143]     {
//@[line192->line1144]       "copy": {
//@[line192->line1145]         "name": "duplicateInGlobalAndOneLoop",
//@[line192->line1146]         "count": "[length(createArray())]"
//@[line192->line1147]       },
//@[line192->line1148]       "type": "Microsoft.Resources/deployments",
//@[line192->line1149]       "apiVersion": "2020-10-01",
//@[line192->line1151]       "properties": {
//@[line192->line1152]         "expressionEvaluationOptions": {
//@[line192->line1153]           "scope": "inner"
//@[line192->line1154]         },
//@[line192->line1155]         "mode": "Incremental",
//@[line192->line1156]         "parameters": {
//@[line192->line1157]           "objParam": {
//@[line192->line1159]           },
//@[line192->line1160]           "stringParamA": {
//@[line192->line1162]           },
//@[line192->line1163]           "stringParamB": {
//@[line192->line1165]           },
//@[line192->line1166]           "arrayParam": {
//@[line192->line1167]             "copy": [
//@[line192->line1173]             ]
//@[line192->line1174]           }
//@[line192->line1175]         },
//@[line192->line1176]         "template": {
//@[line192->line1177]           "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@[line192->line1178]           "contentVersion": "1.0.0.0",
//@[line192->line1179]           "metadata": {
//@[line192->line1180]             "_generator": {
//@[line192->line1181]               "name": "bicep",
//@[line192->line1182]               "version": "dev",
//@[line192->line1183]               "templateHash": "8300391961099598421"
//@[line192->line1184]             }
//@[line192->line1185]           },
//@[line192->line1186]           "parameters": {
//@[line192->line1187]             "stringParamA": {
//@[line192->line1188]               "type": "string",
//@[line192->line1189]               "defaultValue": "test"
//@[line192->line1190]             },
//@[line192->line1191]             "stringParamB": {
//@[line192->line1192]               "type": "string"
//@[line192->line1193]             },
//@[line192->line1194]             "objParam": {
//@[line192->line1195]               "type": "object"
//@[line192->line1196]             },
//@[line192->line1197]             "arrayParam": {
//@[line192->line1198]               "type": "array"
//@[line192->line1199]             }
//@[line192->line1200]           },
//@[line192->line1201]           "resources": [
//@[line192->line1202]             {
//@[line192->line1203]               "type": "Mock.Rp/mockResource",
//@[line192->line1204]               "apiVersion": "2020-01-01",
//@[line192->line1205]               "name": "basicblobs",
//@[line192->line1206]               "location": "[parameters('stringParamA')]"
//@[line192->line1207]             },
//@[line192->line1208]             {
//@[line192->line1209]               "type": "Mock.Rp/mockResource",
//@[line192->line1210]               "apiVersion": "2020-01-01",
//@[line192->line1211]               "name": "myZone",
//@[line192->line1212]               "location": "[parameters('stringParamB')]"
//@[line192->line1213]             }
//@[line192->line1214]           ],
//@[line192->line1215]           "outputs": {
//@[line192->line1216]             "stringOutputA": {
//@[line192->line1217]               "type": "string",
//@[line192->line1218]               "value": "[parameters('stringParamA')]"
//@[line192->line1219]             },
//@[line192->line1220]             "stringOutputB": {
//@[line192->line1221]               "type": "string",
//@[line192->line1222]               "value": "[parameters('stringParamB')]"
//@[line192->line1223]             },
//@[line192->line1224]             "objOutput": {
//@[line192->line1225]               "type": "object",
//@[line192->line1226]               "value": "[reference(resourceId('Mock.Rp/mockResource', 'basicblobs'), '2020-01-01')]"
//@[line192->line1227]             },
//@[line192->line1228]             "arrayOutput": {
//@[line192->line1229]               "type": "array",
//@[line192->line1230]               "value": [
//@[line192->line1231]                 "[resourceId('Mock.Rp/mockResource', 'basicblobs')]",
//@[line192->line1232]                 "[resourceId('Mock.Rp/mockResource', 'myZone')]"
//@[line192->line1233]               ]
//@[line192->line1234]             }
//@[line192->line1235]           }
//@[line192->line1236]         }
//@[line192->line1237]       }
//@[line192->line1238]     },
  name: 'hello-${duplicateAcrossScopes}'
//@[line193->line1150]       "name": "[format('hello-{0}', createArray()[copyIndex()])]",
  params: {
    objParam: {}
//@[line195->line1158]             "value": {}
    stringParamA: 'test'
//@[line196->line1161]             "value": "test"
    stringParamB: 'test'
//@[line197->line1164]             "value": "test"
    arrayParam: [for x in emptyArray: x]
//@[line198->line1168]               {
//@[line198->line1169]                 "name": "value",
//@[line198->line1170]                 "count": "[length(variables('emptyArray'))]",
//@[line198->line1171]                 "input": "[variables('emptyArray')[copyIndex('value')]]"
//@[line198->line1172]               }
  }
}]

var someDuplicate = true
//@[line202->line0032]     "someDuplicate": true,
var otherDuplicate = false
//@[line203->line0033]     "otherDuplicate": false,
module duplicatesEverywhere 'modulea.bicep' = [for someDuplicate in []: {
//@[line204->line1239]     {
//@[line204->line1240]       "copy": {
//@[line204->line1241]         "name": "duplicatesEverywhere",
//@[line204->line1242]         "count": "[length(createArray())]"
//@[line204->line1243]       },
//@[line204->line1244]       "type": "Microsoft.Resources/deployments",
//@[line204->line1245]       "apiVersion": "2020-10-01",
//@[line204->line1247]       "properties": {
//@[line204->line1248]         "expressionEvaluationOptions": {
//@[line204->line1249]           "scope": "inner"
//@[line204->line1250]         },
//@[line204->line1251]         "mode": "Incremental",
//@[line204->line1252]         "parameters": {
//@[line204->line1253]           "objParam": {
//@[line204->line1255]           },
//@[line204->line1256]           "stringParamB": {
//@[line204->line1258]           },
//@[line204->line1259]           "arrayParam": {
//@[line204->line1260]             "copy": [
//@[line204->line1266]             ]
//@[line204->line1267]           }
//@[line204->line1268]         },
//@[line204->line1269]         "template": {
//@[line204->line1270]           "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@[line204->line1271]           "contentVersion": "1.0.0.0",
//@[line204->line1272]           "metadata": {
//@[line204->line1273]             "_generator": {
//@[line204->line1274]               "name": "bicep",
//@[line204->line1275]               "version": "dev",
//@[line204->line1276]               "templateHash": "8300391961099598421"
//@[line204->line1277]             }
//@[line204->line1278]           },
//@[line204->line1279]           "parameters": {
//@[line204->line1280]             "stringParamA": {
//@[line204->line1281]               "type": "string",
//@[line204->line1282]               "defaultValue": "test"
//@[line204->line1283]             },
//@[line204->line1284]             "stringParamB": {
//@[line204->line1285]               "type": "string"
//@[line204->line1286]             },
//@[line204->line1287]             "objParam": {
//@[line204->line1288]               "type": "object"
//@[line204->line1289]             },
//@[line204->line1290]             "arrayParam": {
//@[line204->line1291]               "type": "array"
//@[line204->line1292]             }
//@[line204->line1293]           },
//@[line204->line1294]           "resources": [
//@[line204->line1295]             {
//@[line204->line1296]               "type": "Mock.Rp/mockResource",
//@[line204->line1297]               "apiVersion": "2020-01-01",
//@[line204->line1298]               "name": "basicblobs",
//@[line204->line1299]               "location": "[parameters('stringParamA')]"
//@[line204->line1300]             },
//@[line204->line1301]             {
//@[line204->line1302]               "type": "Mock.Rp/mockResource",
//@[line204->line1303]               "apiVersion": "2020-01-01",
//@[line204->line1304]               "name": "myZone",
//@[line204->line1305]               "location": "[parameters('stringParamB')]"
//@[line204->line1306]             }
//@[line204->line1307]           ],
//@[line204->line1308]           "outputs": {
//@[line204->line1309]             "stringOutputA": {
//@[line204->line1310]               "type": "string",
//@[line204->line1311]               "value": "[parameters('stringParamA')]"
//@[line204->line1312]             },
//@[line204->line1313]             "stringOutputB": {
//@[line204->line1314]               "type": "string",
//@[line204->line1315]               "value": "[parameters('stringParamB')]"
//@[line204->line1316]             },
//@[line204->line1317]             "objOutput": {
//@[line204->line1318]               "type": "object",
//@[line204->line1319]               "value": "[reference(resourceId('Mock.Rp/mockResource', 'basicblobs'), '2020-01-01')]"
//@[line204->line1320]             },
//@[line204->line1321]             "arrayOutput": {
//@[line204->line1322]               "type": "array",
//@[line204->line1323]               "value": [
//@[line204->line1324]                 "[resourceId('Mock.Rp/mockResource', 'basicblobs')]",
//@[line204->line1325]                 "[resourceId('Mock.Rp/mockResource', 'myZone')]"
//@[line204->line1326]               ]
//@[line204->line1327]             }
//@[line204->line1328]           }
//@[line204->line1329]         }
//@[line204->line1330]       }
//@[line204->line1331]     },
  name: 'hello-${someDuplicate}'
//@[line205->line1246]       "name": "[format('hello-{0}', createArray()[copyIndex()])]",
  params: {
    objParam: {}
//@[line207->line1254]             "value": {}
    stringParamB: 'test'
//@[line208->line1257]             "value": "test"
    arrayParam: [for otherDuplicate in emptyArray: '${someDuplicate}-${otherDuplicate}']
//@[line209->line1261]               {
//@[line209->line1262]                 "name": "value",
//@[line209->line1263]                 "count": "[length(variables('emptyArray'))]",
//@[line209->line1264]                 "input": "[format('{0}-{1}', createArray()[copyIndex()], variables('emptyArray')[copyIndex('value')])]"
//@[line209->line1265]               }
  }
}]

module propertyLoopInsideParameterValue 'modulea.bicep' = {
//@[line213->line1332]     {
//@[line213->line1333]       "type": "Microsoft.Resources/deployments",
//@[line213->line1334]       "apiVersion": "2020-10-01",
//@[line213->line1336]       "properties": {
//@[line213->line1337]         "expressionEvaluationOptions": {
//@[line213->line1338]           "scope": "inner"
//@[line213->line1339]         },
//@[line213->line1340]         "mode": "Incremental",
//@[line213->line1341]         "parameters": {
//@[line213->line1342]           "objParam": {
//@[line213->line1373]           },
//@[line213->line1374]           "stringParamB": {
//@[line213->line1376]           },
//@[line213->line1377]           "arrayParam": {
//@[line213->line1389]           }
//@[line213->line1390]         },
//@[line213->line1391]         "template": {
//@[line213->line1392]           "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@[line213->line1393]           "contentVersion": "1.0.0.0",
//@[line213->line1394]           "metadata": {
//@[line213->line1395]             "_generator": {
//@[line213->line1396]               "name": "bicep",
//@[line213->line1397]               "version": "dev",
//@[line213->line1398]               "templateHash": "8300391961099598421"
//@[line213->line1399]             }
//@[line213->line1400]           },
//@[line213->line1401]           "parameters": {
//@[line213->line1402]             "stringParamA": {
//@[line213->line1403]               "type": "string",
//@[line213->line1404]               "defaultValue": "test"
//@[line213->line1405]             },
//@[line213->line1406]             "stringParamB": {
//@[line213->line1407]               "type": "string"
//@[line213->line1408]             },
//@[line213->line1409]             "objParam": {
//@[line213->line1410]               "type": "object"
//@[line213->line1411]             },
//@[line213->line1412]             "arrayParam": {
//@[line213->line1413]               "type": "array"
//@[line213->line1414]             }
//@[line213->line1415]           },
//@[line213->line1416]           "resources": [
//@[line213->line1417]             {
//@[line213->line1418]               "type": "Mock.Rp/mockResource",
//@[line213->line1419]               "apiVersion": "2020-01-01",
//@[line213->line1420]               "name": "basicblobs",
//@[line213->line1421]               "location": "[parameters('stringParamA')]"
//@[line213->line1422]             },
//@[line213->line1423]             {
//@[line213->line1424]               "type": "Mock.Rp/mockResource",
//@[line213->line1425]               "apiVersion": "2020-01-01",
//@[line213->line1426]               "name": "myZone",
//@[line213->line1427]               "location": "[parameters('stringParamB')]"
//@[line213->line1428]             }
//@[line213->line1429]           ],
//@[line213->line1430]           "outputs": {
//@[line213->line1431]             "stringOutputA": {
//@[line213->line1432]               "type": "string",
//@[line213->line1433]               "value": "[parameters('stringParamA')]"
//@[line213->line1434]             },
//@[line213->line1435]             "stringOutputB": {
//@[line213->line1436]               "type": "string",
//@[line213->line1437]               "value": "[parameters('stringParamB')]"
//@[line213->line1438]             },
//@[line213->line1439]             "objOutput": {
//@[line213->line1440]               "type": "object",
//@[line213->line1441]               "value": "[reference(resourceId('Mock.Rp/mockResource', 'basicblobs'), '2020-01-01')]"
//@[line213->line1442]             },
//@[line213->line1443]             "arrayOutput": {
//@[line213->line1444]               "type": "array",
//@[line213->line1445]               "value": [
//@[line213->line1446]                 "[resourceId('Mock.Rp/mockResource', 'basicblobs')]",
//@[line213->line1447]                 "[resourceId('Mock.Rp/mockResource', 'myZone')]"
//@[line213->line1448]               ]
//@[line213->line1449]             }
//@[line213->line1450]           }
//@[line213->line1451]         }
//@[line213->line1452]       }
//@[line213->line1453]     },
  name: 'propertyLoopInsideParameterValue'
//@[line214->line1335]       "name": "propertyLoopInsideParameterValue",
  params: {
    objParam: {
//@[line216->line1343]             "value": {
//@[line216->line1344]               "copy": [
//@[line216->line1362]               ],
//@[line216->line1372]             }
      a: [for i in range(0,10): i]
//@[line217->line1345]                 {
//@[line217->line1346]                   "name": "a",
//@[line217->line1347]                   "count": "[length(range(0, 10))]",
//@[line217->line1348]                   "input": "[range(0, 10)[copyIndex('a')]]"
//@[line217->line1349]                 },
      b: [for i in range(1,2): i]
//@[line218->line1350]                 {
//@[line218->line1351]                   "name": "b",
//@[line218->line1352]                   "count": "[length(range(1, 2))]",
//@[line218->line1353]                   "input": "[range(1, 2)[copyIndex('b')]]"
//@[line218->line1354]                 },
      c: {
//@[line219->line1363]               "c": {
//@[line219->line1364]                 "copy": [
//@[line219->line1370]                 ]
//@[line219->line1371]               }
        d: [for j in range(2,3): j]
//@[line220->line1365]                   {
//@[line220->line1366]                     "name": "d",
//@[line220->line1367]                     "count": "[length(range(2, 3))]",
//@[line220->line1368]                     "input": "[range(2, 3)[copyIndex('d')]]"
//@[line220->line1369]                   }
      }
      e: [for k in range(4,4): {
//@[line222->line1355]                 {
//@[line222->line1356]                   "name": "e",
//@[line222->line1357]                   "count": "[length(range(4, 4))]",
//@[line222->line1358]                   "input": {
//@[line222->line1360]                   }
//@[line222->line1361]                 }
        f: k
//@[line223->line1359]                     "f": "[range(4, 4)[copyIndex('e')]]"
      }]
    }
    stringParamB: ''
//@[line226->line1375]             "value": ""
    arrayParam: [
//@[line227->line1378]             "value": [
//@[line227->line1388]             ]
      {
//@[line228->line1379]               {
//@[line228->line1380]                 "copy": [
//@[line228->line1386]                 ]
//@[line228->line1387]               }
        e: [for j in range(7,7): j]
//@[line229->line1381]                   {
//@[line229->line1382]                     "name": "e",
//@[line229->line1383]                     "count": "[length(range(7, 7))]",
//@[line229->line1384]                     "input": "[range(7, 7)[copyIndex('e')]]"
//@[line229->line1385]                   }
      }
    ]
  }
}

module propertyLoopInsideParameterValueWithIndexes 'modulea.bicep' = {
//@[line235->line1454]     {
//@[line235->line1455]       "type": "Microsoft.Resources/deployments",
//@[line235->line1456]       "apiVersion": "2020-10-01",
//@[line235->line1458]       "properties": {
//@[line235->line1459]         "expressionEvaluationOptions": {
//@[line235->line1460]           "scope": "inner"
//@[line235->line1461]         },
//@[line235->line1462]         "mode": "Incremental",
//@[line235->line1463]         "parameters": {
//@[line235->line1464]           "objParam": {
//@[line235->line1496]           },
//@[line235->line1497]           "stringParamB": {
//@[line235->line1499]           },
//@[line235->line1500]           "arrayParam": {
//@[line235->line1512]           }
//@[line235->line1513]         },
//@[line235->line1514]         "template": {
//@[line235->line1515]           "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@[line235->line1516]           "contentVersion": "1.0.0.0",
//@[line235->line1517]           "metadata": {
//@[line235->line1518]             "_generator": {
//@[line235->line1519]               "name": "bicep",
//@[line235->line1520]               "version": "dev",
//@[line235->line1521]               "templateHash": "8300391961099598421"
//@[line235->line1522]             }
//@[line235->line1523]           },
//@[line235->line1524]           "parameters": {
//@[line235->line1525]             "stringParamA": {
//@[line235->line1526]               "type": "string",
//@[line235->line1527]               "defaultValue": "test"
//@[line235->line1528]             },
//@[line235->line1529]             "stringParamB": {
//@[line235->line1530]               "type": "string"
//@[line235->line1531]             },
//@[line235->line1532]             "objParam": {
//@[line235->line1533]               "type": "object"
//@[line235->line1534]             },
//@[line235->line1535]             "arrayParam": {
//@[line235->line1536]               "type": "array"
//@[line235->line1537]             }
//@[line235->line1538]           },
//@[line235->line1539]           "resources": [
//@[line235->line1540]             {
//@[line235->line1541]               "type": "Mock.Rp/mockResource",
//@[line235->line1542]               "apiVersion": "2020-01-01",
//@[line235->line1543]               "name": "basicblobs",
//@[line235->line1544]               "location": "[parameters('stringParamA')]"
//@[line235->line1545]             },
//@[line235->line1546]             {
//@[line235->line1547]               "type": "Mock.Rp/mockResource",
//@[line235->line1548]               "apiVersion": "2020-01-01",
//@[line235->line1549]               "name": "myZone",
//@[line235->line1550]               "location": "[parameters('stringParamB')]"
//@[line235->line1551]             }
//@[line235->line1552]           ],
//@[line235->line1553]           "outputs": {
//@[line235->line1554]             "stringOutputA": {
//@[line235->line1555]               "type": "string",
//@[line235->line1556]               "value": "[parameters('stringParamA')]"
//@[line235->line1557]             },
//@[line235->line1558]             "stringOutputB": {
//@[line235->line1559]               "type": "string",
//@[line235->line1560]               "value": "[parameters('stringParamB')]"
//@[line235->line1561]             },
//@[line235->line1562]             "objOutput": {
//@[line235->line1563]               "type": "object",
//@[line235->line1564]               "value": "[reference(resourceId('Mock.Rp/mockResource', 'basicblobs'), '2020-01-01')]"
//@[line235->line1565]             },
//@[line235->line1566]             "arrayOutput": {
//@[line235->line1567]               "type": "array",
//@[line235->line1568]               "value": [
//@[line235->line1569]                 "[resourceId('Mock.Rp/mockResource', 'basicblobs')]",
//@[line235->line1570]                 "[resourceId('Mock.Rp/mockResource', 'myZone')]"
//@[line235->line1571]               ]
//@[line235->line1572]             }
//@[line235->line1573]           }
//@[line235->line1574]         }
//@[line235->line1575]       }
//@[line235->line1576]     },
  name: 'propertyLoopInsideParameterValueWithIndexes'
//@[line236->line1457]       "name": "propertyLoopInsideParameterValueWithIndexes",
  params: {
    objParam: {
//@[line238->line1465]             "value": {
//@[line238->line1466]               "copy": [
//@[line238->line1485]               ],
//@[line238->line1495]             }
      a: [for (i, i2) in range(0,10): i + i2]
//@[line239->line1467]                 {
//@[line239->line1468]                   "name": "a",
//@[line239->line1469]                   "count": "[length(range(0, 10))]",
//@[line239->line1470]                   "input": "[add(range(0, 10)[copyIndex('a')], copyIndex('a'))]"
//@[line239->line1471]                 },
      b: [for (i, i2) in range(1,2): i / i2]
//@[line240->line1472]                 {
//@[line240->line1473]                   "name": "b",
//@[line240->line1474]                   "count": "[length(range(1, 2))]",
//@[line240->line1475]                   "input": "[div(range(1, 2)[copyIndex('b')], copyIndex('b'))]"
//@[line240->line1476]                 },
      c: {
//@[line241->line1486]               "c": {
//@[line241->line1487]                 "copy": [
//@[line241->line1493]                 ]
//@[line241->line1494]               }
        d: [for (j, j2) in range(2,3): j * j2]
//@[line242->line1488]                   {
//@[line242->line1489]                     "name": "d",
//@[line242->line1490]                     "count": "[length(range(2, 3))]",
//@[line242->line1491]                     "input": "[mul(range(2, 3)[copyIndex('d')], copyIndex('d'))]"
//@[line242->line1492]                   }
      }
      e: [for (k, k2) in range(4,4): {
//@[line244->line1477]                 {
//@[line244->line1478]                   "name": "e",
//@[line244->line1479]                   "count": "[length(range(4, 4))]",
//@[line244->line1480]                   "input": {
//@[line244->line1483]                   }
//@[line244->line1484]                 }
        f: k
//@[line245->line1481]                     "f": "[range(4, 4)[copyIndex('e')]]",
        g: k2
//@[line246->line1482]                     "g": "[copyIndex('e')]"
      }]
    }
    stringParamB: ''
//@[line249->line1498]             "value": ""
    arrayParam: [
//@[line250->line1501]             "value": [
//@[line250->line1511]             ]
      {
//@[line251->line1502]               {
//@[line251->line1503]                 "copy": [
//@[line251->line1509]                 ]
//@[line251->line1510]               }
        e: [for j in range(7,7): j]
//@[line252->line1504]                   {
//@[line252->line1505]                     "name": "e",
//@[line252->line1506]                     "count": "[length(range(7, 7))]",
//@[line252->line1507]                     "input": "[range(7, 7)[copyIndex('e')]]"
//@[line252->line1508]                   }
      }
    ]
  }
}

module propertyLoopInsideParameterValueInsideModuleLoop 'modulea.bicep' = [for thing in range(0,1): {
//@[line258->line1577]     {
//@[line258->line1578]       "copy": {
//@[line258->line1579]         "name": "propertyLoopInsideParameterValueInsideModuleLoop",
//@[line258->line1580]         "count": "[length(range(0, 1))]"
//@[line258->line1581]       },
//@[line258->line1582]       "type": "Microsoft.Resources/deployments",
//@[line258->line1583]       "apiVersion": "2020-10-01",
//@[line258->line1585]       "properties": {
//@[line258->line1586]         "expressionEvaluationOptions": {
//@[line258->line1587]           "scope": "inner"
//@[line258->line1588]         },
//@[line258->line1589]         "mode": "Incremental",
//@[line258->line1590]         "parameters": {
//@[line258->line1591]           "objParam": {
//@[line258->line1622]           },
//@[line258->line1623]           "stringParamB": {
//@[line258->line1625]           },
//@[line258->line1626]           "arrayParam": {
//@[line258->line1638]           }
//@[line258->line1639]         },
//@[line258->line1640]         "template": {
//@[line258->line1641]           "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@[line258->line1642]           "contentVersion": "1.0.0.0",
//@[line258->line1643]           "metadata": {
//@[line258->line1644]             "_generator": {
//@[line258->line1645]               "name": "bicep",
//@[line258->line1646]               "version": "dev",
//@[line258->line1647]               "templateHash": "8300391961099598421"
//@[line258->line1648]             }
//@[line258->line1649]           },
//@[line258->line1650]           "parameters": {
//@[line258->line1651]             "stringParamA": {
//@[line258->line1652]               "type": "string",
//@[line258->line1653]               "defaultValue": "test"
//@[line258->line1654]             },
//@[line258->line1655]             "stringParamB": {
//@[line258->line1656]               "type": "string"
//@[line258->line1657]             },
//@[line258->line1658]             "objParam": {
//@[line258->line1659]               "type": "object"
//@[line258->line1660]             },
//@[line258->line1661]             "arrayParam": {
//@[line258->line1662]               "type": "array"
//@[line258->line1663]             }
//@[line258->line1664]           },
//@[line258->line1665]           "resources": [
//@[line258->line1666]             {
//@[line258->line1667]               "type": "Mock.Rp/mockResource",
//@[line258->line1668]               "apiVersion": "2020-01-01",
//@[line258->line1669]               "name": "basicblobs",
//@[line258->line1670]               "location": "[parameters('stringParamA')]"
//@[line258->line1671]             },
//@[line258->line1672]             {
//@[line258->line1673]               "type": "Mock.Rp/mockResource",
//@[line258->line1674]               "apiVersion": "2020-01-01",
//@[line258->line1675]               "name": "myZone",
//@[line258->line1676]               "location": "[parameters('stringParamB')]"
//@[line258->line1677]             }
//@[line258->line1678]           ],
//@[line258->line1679]           "outputs": {
//@[line258->line1680]             "stringOutputA": {
//@[line258->line1681]               "type": "string",
//@[line258->line1682]               "value": "[parameters('stringParamA')]"
//@[line258->line1683]             },
//@[line258->line1684]             "stringOutputB": {
//@[line258->line1685]               "type": "string",
//@[line258->line1686]               "value": "[parameters('stringParamB')]"
//@[line258->line1687]             },
//@[line258->line1688]             "objOutput": {
//@[line258->line1689]               "type": "object",
//@[line258->line1690]               "value": "[reference(resourceId('Mock.Rp/mockResource', 'basicblobs'), '2020-01-01')]"
//@[line258->line1691]             },
//@[line258->line1692]             "arrayOutput": {
//@[line258->line1693]               "type": "array",
//@[line258->line1694]               "value": [
//@[line258->line1695]                 "[resourceId('Mock.Rp/mockResource', 'basicblobs')]",
//@[line258->line1696]                 "[resourceId('Mock.Rp/mockResource', 'myZone')]"
//@[line258->line1697]               ]
//@[line258->line1698]             }
//@[line258->line1699]           }
//@[line258->line1700]         }
//@[line258->line1701]       }
//@[line258->line1702]     },
  name: 'propertyLoopInsideParameterValueInsideModuleLoop'
//@[line259->line1584]       "name": "propertyLoopInsideParameterValueInsideModuleLoop",
  params: {
    objParam: {
//@[line261->line1592]             "value": {
//@[line261->line1593]               "copy": [
//@[line261->line1611]               ],
//@[line261->line1621]             }
      a: [for i in range(0,10): i + thing]
//@[line262->line1594]                 {
//@[line262->line1595]                   "name": "a",
//@[line262->line1596]                   "count": "[length(range(0, 10))]",
//@[line262->line1597]                   "input": "[add(range(0, 10)[copyIndex('a')], range(0, 1)[copyIndex()])]"
//@[line262->line1598]                 },
      b: [for i in range(1,2): i * thing]
//@[line263->line1599]                 {
//@[line263->line1600]                   "name": "b",
//@[line263->line1601]                   "count": "[length(range(1, 2))]",
//@[line263->line1602]                   "input": "[mul(range(1, 2)[copyIndex('b')], range(0, 1)[copyIndex()])]"
//@[line263->line1603]                 },
      c: {
//@[line264->line1612]               "c": {
//@[line264->line1613]                 "copy": [
//@[line264->line1619]                 ]
//@[line264->line1620]               }
        d: [for j in range(2,3): j]
//@[line265->line1614]                   {
//@[line265->line1615]                     "name": "d",
//@[line265->line1616]                     "count": "[length(range(2, 3))]",
//@[line265->line1617]                     "input": "[range(2, 3)[copyIndex('d')]]"
//@[line265->line1618]                   }
      }
      e: [for k in range(4,4): {
//@[line267->line1604]                 {
//@[line267->line1605]                   "name": "e",
//@[line267->line1606]                   "count": "[length(range(4, 4))]",
//@[line267->line1607]                   "input": {
//@[line267->line1609]                   }
//@[line267->line1610]                 }
        f: k - thing
//@[line268->line1608]                     "f": "[sub(range(4, 4)[copyIndex('e')], range(0, 1)[copyIndex()])]"
      }]
    }
    stringParamB: ''
//@[line271->line1624]             "value": ""
    arrayParam: [
//@[line272->line1627]             "value": [
//@[line272->line1637]             ]
      {
//@[line273->line1628]               {
//@[line273->line1629]                 "copy": [
//@[line273->line1635]                 ]
//@[line273->line1636]               }
        e: [for j in range(7,7): j % thing]
//@[line274->line1630]                   {
//@[line274->line1631]                     "name": "e",
//@[line274->line1632]                     "count": "[length(range(7, 7))]",
//@[line274->line1633]                     "input": "[mod(range(7, 7)[copyIndex('e')], range(0, 1)[copyIndex()])]"
//@[line274->line1634]                   }
      }
    ]
  }
}]


// BEGIN: Key Vault Secret Reference

resource kv 'Microsoft.KeyVault/vaults@2019-09-01' existing = {
  name: 'testkeyvault'
}

module secureModule1 'child/secureParams.bicep' = {
//@[line287->line1703]     {
//@[line287->line1704]       "type": "Microsoft.Resources/deployments",
//@[line287->line1705]       "apiVersion": "2020-10-01",
//@[line287->line1707]       "properties": {
//@[line287->line1708]         "expressionEvaluationOptions": {
//@[line287->line1709]           "scope": "inner"
//@[line287->line1710]         },
//@[line287->line1711]         "mode": "Incremental",
//@[line287->line1712]         "parameters": {
//@[line287->line1713]           "secureStringParam1": {
//@[line287->line1714]             "reference": {
//@[line287->line1715]               "keyVault": {
//@[line287->line1716]                 "id": "[resourceId('Microsoft.KeyVault/vaults', 'testkeyvault')]"
//@[line287->line1717]               },
//@[line287->line1718]               "secretName": "mySecret"
//@[line287->line1719]             }
//@[line287->line1720]           },
//@[line287->line1721]           "secureStringParam2": {
//@[line287->line1722]             "reference": {
//@[line287->line1723]               "keyVault": {
//@[line287->line1724]                 "id": "[resourceId('Microsoft.KeyVault/vaults', 'testkeyvault')]"
//@[line287->line1725]               },
//@[line287->line1726]               "secretName": "mySecret",
//@[line287->line1727]               "secretVersion": "secretVersion"
//@[line287->line1728]             }
//@[line287->line1729]           }
//@[line287->line1730]         },
//@[line287->line1731]         "template": {
//@[line287->line1732]           "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@[line287->line1733]           "contentVersion": "1.0.0.0",
//@[line287->line1734]           "metadata": {
//@[line287->line1735]             "_generator": {
//@[line287->line1736]               "name": "bicep",
//@[line287->line1737]               "version": "dev",
//@[line287->line1738]               "templateHash": "15522334618541518671"
//@[line287->line1739]             }
//@[line287->line1740]           },
//@[line287->line1741]           "parameters": {
//@[line287->line1742]             "secureStringParam1": {
//@[line287->line1743]               "type": "securestring"
//@[line287->line1744]             },
//@[line287->line1745]             "secureStringParam2": {
//@[line287->line1746]               "type": "securestring",
//@[line287->line1747]               "defaultValue": ""
//@[line287->line1748]             }
//@[line287->line1749]           },
//@[line287->line1750]           "resources": [],
//@[line287->line1751]           "outputs": {
//@[line287->line1752]             "exposedSecureString": {
//@[line287->line1753]               "type": "string",
//@[line287->line1754]               "value": "[parameters('secureStringParam1')]"
//@[line287->line1755]             }
//@[line287->line1756]           }
//@[line287->line1757]         }
//@[line287->line1758]       }
//@[line287->line1759]     },
  name: 'secureModule1'
//@[line288->line1706]       "name": "secureModule1",
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
//@[line300->line1760]     {
//@[line300->line1761]       "type": "Microsoft.Resources/deployments",
//@[line300->line1762]       "apiVersion": "2020-10-01",
//@[line300->line1764]       "properties": {
//@[line300->line1765]         "expressionEvaluationOptions": {
//@[line300->line1766]           "scope": "inner"
//@[line300->line1767]         },
//@[line300->line1768]         "mode": "Incremental",
//@[line300->line1769]         "parameters": {
//@[line300->line1770]           "secureStringParam1": {
//@[line300->line1771]             "reference": {
//@[line300->line1772]               "keyVault": {
//@[line300->line1773]                 "id": "[extensionResourceId(format('/subscriptions/{0}/resourceGroups/{1}', subscription().subscriptionId, 'otherGroup'), 'Microsoft.KeyVault/vaults', 'testkeyvault')]"
//@[line300->line1774]               },
//@[line300->line1775]               "secretName": "mySecret"
//@[line300->line1776]             }
//@[line300->line1777]           },
//@[line300->line1778]           "secureStringParam2": {
//@[line300->line1779]             "reference": {
//@[line300->line1780]               "keyVault": {
//@[line300->line1781]                 "id": "[extensionResourceId(format('/subscriptions/{0}/resourceGroups/{1}', subscription().subscriptionId, 'otherGroup'), 'Microsoft.KeyVault/vaults', 'testkeyvault')]"
//@[line300->line1782]               },
//@[line300->line1783]               "secretName": "mySecret",
//@[line300->line1784]               "secretVersion": "secretVersion"
//@[line300->line1785]             }
//@[line300->line1786]           }
//@[line300->line1787]         },
//@[line300->line1788]         "template": {
//@[line300->line1789]           "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@[line300->line1790]           "contentVersion": "1.0.0.0",
//@[line300->line1791]           "metadata": {
//@[line300->line1792]             "_generator": {
//@[line300->line1793]               "name": "bicep",
//@[line300->line1794]               "version": "dev",
//@[line300->line1795]               "templateHash": "15522334618541518671"
//@[line300->line1796]             }
//@[line300->line1797]           },
//@[line300->line1798]           "parameters": {
//@[line300->line1799]             "secureStringParam1": {
//@[line300->line1800]               "type": "securestring"
//@[line300->line1801]             },
//@[line300->line1802]             "secureStringParam2": {
//@[line300->line1803]               "type": "securestring",
//@[line300->line1804]               "defaultValue": ""
//@[line300->line1805]             }
//@[line300->line1806]           },
//@[line300->line1807]           "resources": [],
//@[line300->line1808]           "outputs": {
//@[line300->line1809]             "exposedSecureString": {
//@[line300->line1810]               "type": "string",
//@[line300->line1811]               "value": "[parameters('secureStringParam1')]"
//@[line300->line1812]             }
//@[line300->line1813]           }
//@[line300->line1814]         }
//@[line300->line1815]       }
//@[line300->line1816]     },
  name: 'secureModule2'
//@[line301->line1763]       "name": "secureModule2",
  params: {
    secureStringParam1: scopedKv.getSecret('mySecret')
    secureStringParam2: scopedKv.getSecret('mySecret','secretVersion')
  }
}

//looped module with looped existing resource (Issue #2862)
var vaults = [
//@[line309->line0034]     "vaults": [
//@[line309->line0045]     ],
  {
//@[line310->line0035]       {
//@[line310->line0039]       },
    vaultName: 'test-1-kv'
//@[line311->line0036]         "vaultName": "test-1-kv",
    vaultRG: 'test-1-rg'
//@[line312->line0037]         "vaultRG": "test-1-rg",
    vaultSub: 'abcd-efgh'
//@[line313->line0038]         "vaultSub": "abcd-efgh"
  }
  {
//@[line315->line0040]       {
//@[line315->line0044]       }
    vaultName: 'test-2-kv'
//@[line316->line0041]         "vaultName": "test-2-kv",
    vaultRG: 'test-2-rg'
//@[line317->line0042]         "vaultRG": "test-2-rg",
    vaultSub: 'ijkl-1adg1'
//@[line318->line0043]         "vaultSub": "ijkl-1adg1"
  }
]
var secrets = [
//@[line321->line0046]     "secrets": [
//@[line321->line0055]     ]
  {
//@[line322->line0047]       {
//@[line322->line0050]       },
    name: 'secret01'
//@[line323->line0048]         "name": "secret01",
    version: 'versionA'
//@[line324->line0049]         "version": "versionA"
  }
  {
//@[line326->line0051]       {
//@[line326->line0054]       }
    name: 'secret02'
//@[line327->line0052]         "name": "secret02",
    version: 'versionB'
//@[line328->line0053]         "version": "versionB"
  }
]

resource loopedKv 'Microsoft.KeyVault/vaults@2019-09-01' existing = [for vault in vaults: {
  name: vault.vaultName
  scope: resourceGroup(vault.vaultSub, vault.vaultRG)
}]

module secureModuleLooped 'child/secureParams.bicep' = [for (secret, i) in secrets: {
//@[line337->line1817]     {
//@[line337->line1818]       "copy": {
//@[line337->line1819]         "name": "secureModuleLooped",
//@[line337->line1820]         "count": "[length(variables('secrets'))]"
//@[line337->line1821]       },
//@[line337->line1822]       "type": "Microsoft.Resources/deployments",
//@[line337->line1823]       "apiVersion": "2020-10-01",
//@[line337->line1825]       "properties": {
//@[line337->line1826]         "expressionEvaluationOptions": {
//@[line337->line1827]           "scope": "inner"
//@[line337->line1828]         },
//@[line337->line1829]         "mode": "Incremental",
//@[line337->line1830]         "parameters": {
//@[line337->line1831]           "secureStringParam1": {
//@[line337->line1832]             "reference": {
//@[line337->line1833]               "keyVault": {
//@[line337->line1834]                 "id": "[extensionResourceId(format('/subscriptions/{0}/resourceGroups/{1}', variables('vaults')[copyIndex()].vaultSub, variables('vaults')[copyIndex()].vaultRG), 'Microsoft.KeyVault/vaults', variables('vaults')[copyIndex()].vaultName)]"
//@[line337->line1835]               },
//@[line337->line1836]               "secretName": "[variables('secrets')[copyIndex()].name]"
//@[line337->line1837]             }
//@[line337->line1838]           },
//@[line337->line1839]           "secureStringParam2": {
//@[line337->line1840]             "reference": {
//@[line337->line1841]               "keyVault": {
//@[line337->line1842]                 "id": "[extensionResourceId(format('/subscriptions/{0}/resourceGroups/{1}', variables('vaults')[copyIndex()].vaultSub, variables('vaults')[copyIndex()].vaultRG), 'Microsoft.KeyVault/vaults', variables('vaults')[copyIndex()].vaultName)]"
//@[line337->line1843]               },
//@[line337->line1844]               "secretName": "[variables('secrets')[copyIndex()].name]",
//@[line337->line1845]               "secretVersion": "[variables('secrets')[copyIndex()].version]"
//@[line337->line1846]             }
//@[line337->line1847]           }
//@[line337->line1848]         },
//@[line337->line1849]         "template": {
//@[line337->line1850]           "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@[line337->line1851]           "contentVersion": "1.0.0.0",
//@[line337->line1852]           "metadata": {
//@[line337->line1853]             "_generator": {
//@[line337->line1854]               "name": "bicep",
//@[line337->line1855]               "version": "dev",
//@[line337->line1856]               "templateHash": "15522334618541518671"
//@[line337->line1857]             }
//@[line337->line1858]           },
//@[line337->line1859]           "parameters": {
//@[line337->line1860]             "secureStringParam1": {
//@[line337->line1861]               "type": "securestring"
//@[line337->line1862]             },
//@[line337->line1863]             "secureStringParam2": {
//@[line337->line1864]               "type": "securestring",
//@[line337->line1865]               "defaultValue": ""
//@[line337->line1866]             }
//@[line337->line1867]           },
//@[line337->line1868]           "resources": [],
//@[line337->line1869]           "outputs": {
//@[line337->line1870]             "exposedSecureString": {
//@[line337->line1871]               "type": "string",
//@[line337->line1872]               "value": "[parameters('secureStringParam1')]"
//@[line337->line1873]             }
//@[line337->line1874]           }
//@[line337->line1875]         }
//@[line337->line1876]       }
//@[line337->line1877]     },
  name: 'secureModuleLooped-${i}'
//@[line338->line1824]       "name": "[format('secureModuleLooped-{0}', copyIndex())]",
  params: {
    secureStringParam1: loopedKv[i].getSecret(secret.name)
    secureStringParam2: loopedKv[i].getSecret(secret.name, secret.version)
  }
}]

module secureModuleCondition 'child/secureParams.bicep' = {
//@[line345->line1878]     {
//@[line345->line1879]       "type": "Microsoft.Resources/deployments",
//@[line345->line1880]       "apiVersion": "2020-10-01",
//@[line345->line1882]       "properties": {
//@[line345->line1883]         "expressionEvaluationOptions": {
//@[line345->line1884]           "scope": "inner"
//@[line345->line1885]         },
//@[line345->line1886]         "mode": "Incremental",
//@[line345->line1887]         "parameters": {
//@[line345->line1888]           "secureStringParam1": "[if(true(), createObject('reference', createObject('keyVault', createObject('id', resourceId('Microsoft.KeyVault/vaults', 'testkeyvault')), 'secretName', 'mySecret')), createObject('value', 'notTrue'))]",
//@[line345->line1889]           "secureStringParam2": "[if(true(), if(false(), createObject('value', 'false'), createObject('reference', createObject('keyVault', createObject('id', resourceId('Microsoft.KeyVault/vaults', 'testkeyvault')), 'secretName', 'mySecret', 'secretVersion', 'secretVersion'))), createObject('value', 'notTrue'))]"
//@[line345->line1890]         },
//@[line345->line1891]         "template": {
//@[line345->line1892]           "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@[line345->line1893]           "contentVersion": "1.0.0.0",
//@[line345->line1894]           "metadata": {
//@[line345->line1895]             "_generator": {
//@[line345->line1896]               "name": "bicep",
//@[line345->line1897]               "version": "dev",
//@[line345->line1898]               "templateHash": "15522334618541518671"
//@[line345->line1899]             }
//@[line345->line1900]           },
//@[line345->line1901]           "parameters": {
//@[line345->line1902]             "secureStringParam1": {
//@[line345->line1903]               "type": "securestring"
//@[line345->line1904]             },
//@[line345->line1905]             "secureStringParam2": {
//@[line345->line1906]               "type": "securestring",
//@[line345->line1907]               "defaultValue": ""
//@[line345->line1908]             }
//@[line345->line1909]           },
//@[line345->line1910]           "resources": [],
//@[line345->line1911]           "outputs": {
//@[line345->line1912]             "exposedSecureString": {
//@[line345->line1913]               "type": "string",
//@[line345->line1914]               "value": "[parameters('secureStringParam1')]"
//@[line345->line1915]             }
//@[line345->line1916]           }
//@[line345->line1917]         }
//@[line345->line1918]       }
//@[line345->line1919]     },
  name: 'secureModuleCondition'
//@[line346->line1881]       "name": "secureModuleCondition",
  params: {
    secureStringParam1: true ? kv.getSecret('mySecret') : 'notTrue'
    secureStringParam2: true ? false ? 'false' : kv.getSecret('mySecret','secretVersion') : 'notTrue'
  }
}

// END: Key Vault Secret Reference

module withSpace 'module with space.bicep' = {
//@[line355->line1920]     {
//@[line355->line1921]       "type": "Microsoft.Resources/deployments",
//@[line355->line1922]       "apiVersion": "2020-10-01",
//@[line355->line1924]       "properties": {
//@[line355->line1925]         "expressionEvaluationOptions": {
//@[line355->line1926]           "scope": "inner"
//@[line355->line1927]         },
//@[line355->line1928]         "mode": "Incremental",
//@[line355->line1929]         "template": {
//@[line355->line1930]           "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@[line355->line1931]           "contentVersion": "1.0.0.0",
//@[line355->line1932]           "metadata": {
//@[line355->line1933]             "_generator": {
//@[line355->line1934]               "name": "bicep",
//@[line355->line1935]               "version": "dev",
//@[line355->line1936]               "templateHash": "1347091426241151379"
//@[line355->line1937]             }
//@[line355->line1938]           },
//@[line355->line1939]           "parameters": {
//@[line355->line1940]             "location": {
//@[line355->line1941]               "type": "string",
//@[line355->line1942]               "defaultValue": "westus"
//@[line355->line1943]             }
//@[line355->line1944]           },
//@[line355->line1945]           "resources": [],
//@[line355->line1946]           "outputs": {
//@[line355->line1947]             "loc": {
//@[line355->line1948]               "type": "string",
//@[line355->line1949]               "value": "[parameters('location')]"
//@[line355->line1950]             }
//@[line355->line1951]           }
//@[line355->line1952]         }
//@[line355->line1953]       }
//@[line355->line1954]     },
  name: 'withSpace'
//@[line356->line1923]       "name": "withSpace",
}

module folderWithSpace 'child/folder with space/child with space.bicep' = {
//@[line359->line1955]     {
//@[line359->line1956]       "type": "Microsoft.Resources/deployments",
//@[line359->line1957]       "apiVersion": "2020-10-01",
//@[line359->line1959]       "properties": {
//@[line359->line1960]         "expressionEvaluationOptions": {
//@[line359->line1961]           "scope": "inner"
//@[line359->line1962]         },
//@[line359->line1963]         "mode": "Incremental",
//@[line359->line1964]         "template": {
//@[line359->line1965]           "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@[line359->line1966]           "contentVersion": "1.0.0.0",
//@[line359->line1967]           "metadata": {
//@[line359->line1968]             "_generator": {
//@[line359->line1969]               "name": "bicep",
//@[line359->line1970]               "version": "dev",
//@[line359->line1971]               "templateHash": "1347091426241151379"
//@[line359->line1972]             }
//@[line359->line1973]           },
//@[line359->line1974]           "parameters": {
//@[line359->line1975]             "location": {
//@[line359->line1976]               "type": "string",
//@[line359->line1977]               "defaultValue": "westus"
//@[line359->line1978]             }
//@[line359->line1979]           },
//@[line359->line1980]           "resources": [],
//@[line359->line1981]           "outputs": {
//@[line359->line1982]             "loc": {
//@[line359->line1983]               "type": "string",
//@[line359->line1984]               "value": "[parameters('location')]"
//@[line359->line1985]             }
//@[line359->line1986]           }
//@[line359->line1987]         }
//@[line359->line1988]       }
//@[line359->line1989]     },
  name: 'childWithSpace'
//@[line360->line1958]       "name": "childWithSpace",
}

module withSeparateConfig './child/folder with separate config/moduleWithAzImport.bicep' = {
//@[line363->line1990]     {
//@[line363->line1991]       "type": "Microsoft.Resources/deployments",
//@[line363->line1992]       "apiVersion": "2020-10-01",
//@[line363->line1994]       "properties": {
//@[line363->line1995]         "expressionEvaluationOptions": {
//@[line363->line1996]           "scope": "inner"
//@[line363->line1997]         },
//@[line363->line1998]         "mode": "Incremental",
//@[line363->line1999]         "template": {
//@[line363->line2000]           "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@[line363->line2001]           "contentVersion": "1.0.0.0",
//@[line363->line2002]           "metadata": {
//@[line363->line2003]             "_generator": {
//@[line363->line2004]               "name": "bicep",
//@[line363->line2005]               "version": "dev",
//@[line363->line2006]               "templateHash": "16208829896531593582"
//@[line363->line2007]             }
//@[line363->line2008]           },
//@[line363->line2009]           "imports": {
//@[line363->line2010]             "az": {
//@[line363->line2011]               "provider": "AzureResourceManager",
//@[line363->line2012]               "version": "1.0.0"
//@[line363->line2013]             }
//@[line363->line2014]           },
//@[line363->line2015]           "resources": [],
//@[line363->line2016]           "outputs": {
//@[line363->line2017]             "str": {
//@[line363->line2018]               "type": "string",
//@[line363->line2019]               "value": "foo"
//@[line363->line2020]             }
//@[line363->line2021]           }
//@[line363->line2022]         }
//@[line363->line2023]       }
//@[line363->line2024]     }
  name: 'withSeparateConfig'
//@[line364->line1993]       "name": "withSeparateConfig",
}

