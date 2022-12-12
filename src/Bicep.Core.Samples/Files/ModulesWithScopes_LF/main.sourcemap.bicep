targetScope = 'tenant'

module myManagementGroupMod 'modules/managementgroup.bicep' = {
//@[line02->line0011]     {
//@[line02->line0012]       "type": "Microsoft.Resources/deployments",
//@[line02->line0013]       "apiVersion": "2020-10-01",
//@[line02->line0015]       "scope": "[format('Microsoft.Management/managementGroups/{0}', 'myManagementGroup')]",
//@[line02->line0016]       "location": "[deployment().location]",
//@[line02->line0017]       "properties": {
//@[line02->line0018]         "expressionEvaluationOptions": {
//@[line02->line0019]           "scope": "inner"
//@[line02->line0020]         },
//@[line02->line0021]         "mode": "Incremental",
//@[line02->line0022]         "template": {
//@[line02->line0023]           "$schema": "https://schema.management.azure.com/schemas/2019-08-01/managementGroupDeploymentTemplate.json#",
//@[line02->line0024]           "contentVersion": "1.0.0.0",
//@[line02->line0025]           "metadata": {
//@[line02->line0026]             "_generator": {
//@[line02->line0027]               "name": "bicep",
//@[line02->line0028]               "version": "dev",
//@[line02->line0029]               "templateHash": "6066236114936137763"
//@[line02->line0030]             }
//@[line02->line0031]           },
//@[line02->line0032]           "resources": [
//@[line02->line0033]             {
//@[line02->line0034]               "type": "Microsoft.Resources/deployments",
//@[line02->line0035]               "apiVersion": "2020-10-01",
//@[line02->line0036]               "name": "myTenantMod",
//@[line02->line0037]               "scope": "/",
//@[line02->line0038]               "location": "[deployment().location]",
//@[line02->line0039]               "properties": {
//@[line02->line0040]                 "expressionEvaluationOptions": {
//@[line02->line0041]                   "scope": "inner"
//@[line02->line0042]                 },
//@[line02->line0043]                 "mode": "Incremental",
//@[line02->line0044]                 "template": {
//@[line02->line0045]                   "$schema": "https://schema.management.azure.com/schemas/2019-08-01/tenantDeploymentTemplate.json#",
//@[line02->line0046]                   "contentVersion": "1.0.0.0",
//@[line02->line0047]                   "metadata": {
//@[line02->line0048]                     "_generator": {
//@[line02->line0049]                       "name": "bicep",
//@[line02->line0050]                       "version": "dev",
//@[line02->line0051]                       "templateHash": "15729984543815100695"
//@[line02->line0052]                     }
//@[line02->line0053]                   },
//@[line02->line0054]                   "resources": [],
//@[line02->line0055]                   "outputs": {
//@[line02->line0056]                     "myOutput": {
//@[line02->line0057]                       "type": "string",
//@[line02->line0058]                       "value": "hello!"
//@[line02->line0059]                     }
//@[line02->line0060]                   }
//@[line02->line0061]                 }
//@[line02->line0062]               }
//@[line02->line0063]             },
//@[line02->line0064]             {
//@[line02->line0065]               "type": "Microsoft.Resources/deployments",
//@[line02->line0066]               "apiVersion": "2020-10-01",
//@[line02->line0067]               "name": "myManagementGroupMod",
//@[line02->line0068]               "scope": "[format('Microsoft.Management/managementGroups/{0}', 'myManagementGroup2')]",
//@[line02->line0069]               "location": "[deployment().location]",
//@[line02->line0070]               "properties": {
//@[line02->line0071]                 "expressionEvaluationOptions": {
//@[line02->line0072]                   "scope": "inner"
//@[line02->line0073]                 },
//@[line02->line0074]                 "mode": "Incremental",
//@[line02->line0075]                 "template": {
//@[line02->line0076]                   "$schema": "https://schema.management.azure.com/schemas/2019-08-01/managementGroupDeploymentTemplate.json#",
//@[line02->line0077]                   "contentVersion": "1.0.0.0",
//@[line02->line0078]                   "metadata": {
//@[line02->line0079]                     "_generator": {
//@[line02->line0080]                       "name": "bicep",
//@[line02->line0081]                       "version": "dev",
//@[line02->line0082]                       "templateHash": "2139830058681583241"
//@[line02->line0083]                     }
//@[line02->line0084]                   },
//@[line02->line0085]                   "resources": []
//@[line02->line0086]                 }
//@[line02->line0087]               }
//@[line02->line0088]             },
//@[line02->line0089]             {
//@[line02->line0090]               "type": "Microsoft.Resources/deployments",
//@[line02->line0091]               "apiVersion": "2020-10-01",
//@[line02->line0092]               "name": "mySubscriptionMod",
//@[line02->line0093]               "subscriptionId": "1ad827ac-2669-4c2f-9970-282b93c3c550",
//@[line02->line0094]               "location": "[deployment().location]",
//@[line02->line0095]               "properties": {
//@[line02->line0096]                 "expressionEvaluationOptions": {
//@[line02->line0097]                   "scope": "inner"
//@[line02->line0098]                 },
//@[line02->line0099]                 "mode": "Incremental",
//@[line02->line0100]                 "template": {
//@[line02->line0101]                   "$schema": "https://schema.management.azure.com/schemas/2018-05-01/subscriptionDeploymentTemplate.json#",
//@[line02->line0102]                   "contentVersion": "1.0.0.0",
//@[line02->line0103]                   "metadata": {
//@[line02->line0104]                     "_generator": {
//@[line02->line0105]                       "name": "bicep",
//@[line02->line0106]                       "version": "dev",
//@[line02->line0107]                       "templateHash": "1395382333336833730"
//@[line02->line0108]                     }
//@[line02->line0109]                   },
//@[line02->line0110]                   "resources": []
//@[line02->line0111]                 }
//@[line02->line0112]               }
//@[line02->line0113]             }
//@[line02->line0114]           ],
//@[line02->line0115]           "outputs": {
//@[line02->line0116]             "myOutput": {
//@[line02->line0117]               "type": "string",
//@[line02->line0118]               "value": "[reference(tenantResourceId('Microsoft.Resources/deployments', 'myTenantMod'), '2020-10-01').outputs.myOutput.value]"
//@[line02->line0119]             }
//@[line02->line0120]           }
//@[line02->line0121]         }
//@[line02->line0122]       }
//@[line02->line0123]     },
  name: 'myManagementGroupMod'
//@[line03->line0014]       "name": "myManagementGroupMod",
  scope: managementGroup('myManagementGroup')
}
module myManagementGroupModWithDuplicatedNameButDifferentScope 'modules/managementgroup_empty.bicep' = {
//@[line06->line0124]     {
//@[line06->line0125]       "type": "Microsoft.Resources/deployments",
//@[line06->line0126]       "apiVersion": "2020-10-01",
//@[line06->line0128]       "scope": "[format('Microsoft.Management/managementGroups/{0}', 'myManagementGroup2')]",
//@[line06->line0129]       "location": "[deployment().location]",
//@[line06->line0130]       "properties": {
//@[line06->line0131]         "expressionEvaluationOptions": {
//@[line06->line0132]           "scope": "inner"
//@[line06->line0133]         },
//@[line06->line0134]         "mode": "Incremental",
//@[line06->line0135]         "template": {
//@[line06->line0136]           "$schema": "https://schema.management.azure.com/schemas/2019-08-01/managementGroupDeploymentTemplate.json#",
//@[line06->line0137]           "contentVersion": "1.0.0.0",
//@[line06->line0138]           "metadata": {
//@[line06->line0139]             "_generator": {
//@[line06->line0140]               "name": "bicep",
//@[line06->line0141]               "version": "dev",
//@[line06->line0142]               "templateHash": "2139830058681583241"
//@[line06->line0143]             }
//@[line06->line0144]           },
//@[line06->line0145]           "resources": []
//@[line06->line0146]         }
//@[line06->line0147]       }
//@[line06->line0148]     },
  name: 'myManagementGroupMod'
//@[line07->line0127]       "name": "myManagementGroupMod",
  scope: managementGroup('myManagementGroup2')
}
module mySubscriptionMod 'modules/subscription.bicep' = {
//@[line10->line0149]     {
//@[line10->line0150]       "type": "Microsoft.Resources/deployments",
//@[line10->line0151]       "apiVersion": "2020-10-01",
//@[line10->line0154]       "location": "[deployment().location]",
//@[line10->line0155]       "properties": {
//@[line10->line0156]         "expressionEvaluationOptions": {
//@[line10->line0157]           "scope": "inner"
//@[line10->line0158]         },
//@[line10->line0159]         "mode": "Incremental",
//@[line10->line0160]         "template": {
//@[line10->line0161]           "$schema": "https://schema.management.azure.com/schemas/2018-05-01/subscriptionDeploymentTemplate.json#",
//@[line10->line0162]           "contentVersion": "1.0.0.0",
//@[line10->line0163]           "metadata": {
//@[line10->line0164]             "_generator": {
//@[line10->line0165]               "name": "bicep",
//@[line10->line0166]               "version": "dev",
//@[line10->line0167]               "templateHash": "12301588538908463385"
//@[line10->line0168]             }
//@[line10->line0169]           },
//@[line10->line0170]           "resources": [
//@[line10->line0171]             {
//@[line10->line0172]               "type": "Microsoft.Resources/deployments",
//@[line10->line0173]               "apiVersion": "2020-10-01",
//@[line10->line0174]               "name": "myResourceGroupMod",
//@[line10->line0175]               "resourceGroup": "myRg",
//@[line10->line0176]               "properties": {
//@[line10->line0177]                 "expressionEvaluationOptions": {
//@[line10->line0178]                   "scope": "inner"
//@[line10->line0179]                 },
//@[line10->line0180]                 "mode": "Incremental",
//@[line10->line0181]                 "template": {
//@[line10->line0182]                   "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@[line10->line0183]                   "contentVersion": "1.0.0.0",
//@[line10->line0184]                   "metadata": {
//@[line10->line0185]                     "_generator": {
//@[line10->line0186]                       "name": "bicep",
//@[line10->line0187]                       "version": "dev",
//@[line10->line0188]                       "templateHash": "3773512362905547979"
//@[line10->line0189]                     }
//@[line10->line0190]                   },
//@[line10->line0191]                   "resources": [
//@[line10->line0192]                     {
//@[line10->line0193]                       "type": "Microsoft.Resources/deployments",
//@[line10->line0194]                       "apiVersion": "2020-10-01",
//@[line10->line0195]                       "name": "myTenantMod",
//@[line10->line0196]                       "scope": "/",
//@[line10->line0197]                       "location": "[resourceGroup().location]",
//@[line10->line0198]                       "properties": {
//@[line10->line0199]                         "expressionEvaluationOptions": {
//@[line10->line0200]                           "scope": "inner"
//@[line10->line0201]                         },
//@[line10->line0202]                         "mode": "Incremental",
//@[line10->line0203]                         "template": {
//@[line10->line0204]                           "$schema": "https://schema.management.azure.com/schemas/2019-08-01/tenantDeploymentTemplate.json#",
//@[line10->line0205]                           "contentVersion": "1.0.0.0",
//@[line10->line0206]                           "metadata": {
//@[line10->line0207]                             "_generator": {
//@[line10->line0208]                               "name": "bicep",
//@[line10->line0209]                               "version": "dev",
//@[line10->line0210]                               "templateHash": "15729984543815100695"
//@[line10->line0211]                             }
//@[line10->line0212]                           },
//@[line10->line0213]                           "resources": [],
//@[line10->line0214]                           "outputs": {
//@[line10->line0215]                             "myOutput": {
//@[line10->line0216]                               "type": "string",
//@[line10->line0217]                               "value": "hello!"
//@[line10->line0218]                             }
//@[line10->line0219]                           }
//@[line10->line0220]                         }
//@[line10->line0221]                       }
//@[line10->line0222]                     },
//@[line10->line0223]                     {
//@[line10->line0224]                       "type": "Microsoft.Resources/deployments",
//@[line10->line0225]                       "apiVersion": "2020-10-01",
//@[line10->line0226]                       "name": "myOtherResourceGroup",
//@[line10->line0227]                       "subscriptionId": "db90cfef-a146-4f67-b32f-b263518bd216",
//@[line10->line0228]                       "resourceGroup": "myOtherRg",
//@[line10->line0229]                       "properties": {
//@[line10->line0230]                         "expressionEvaluationOptions": {
//@[line10->line0231]                           "scope": "inner"
//@[line10->line0232]                         },
//@[line10->line0233]                         "mode": "Incremental",
//@[line10->line0234]                         "template": {
//@[line10->line0235]                           "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@[line10->line0236]                           "contentVersion": "1.0.0.0",
//@[line10->line0237]                           "metadata": {
//@[line10->line0238]                             "_generator": {
//@[line10->line0239]                               "name": "bicep",
//@[line10->line0240]                               "version": "dev",
//@[line10->line0241]                               "templateHash": "7469434526073292388"
//@[line10->line0242]                             }
//@[line10->line0243]                           },
//@[line10->line0244]                           "resources": [],
//@[line10->line0245]                           "outputs": {
//@[line10->line0246]                             "myOutput": {
//@[line10->line0247]                               "type": "string",
//@[line10->line0248]                               "value": "hello!"
//@[line10->line0249]                             }
//@[line10->line0250]                           }
//@[line10->line0251]                         }
//@[line10->line0252]                       }
//@[line10->line0253]                     },
//@[line10->line0254]                     {
//@[line10->line0255]                       "type": "Microsoft.Resources/deployments",
//@[line10->line0256]                       "apiVersion": "2020-10-01",
//@[line10->line0257]                       "name": "mySubscription",
//@[line10->line0258]                       "subscriptionId": "[subscription().subscriptionId]",
//@[line10->line0259]                       "location": "[resourceGroup().location]",
//@[line10->line0260]                       "properties": {
//@[line10->line0261]                         "expressionEvaluationOptions": {
//@[line10->line0262]                           "scope": "inner"
//@[line10->line0263]                         },
//@[line10->line0264]                         "mode": "Incremental",
//@[line10->line0265]                         "template": {
//@[line10->line0266]                           "$schema": "https://schema.management.azure.com/schemas/2018-05-01/subscriptionDeploymentTemplate.json#",
//@[line10->line0267]                           "contentVersion": "1.0.0.0",
//@[line10->line0268]                           "metadata": {
//@[line10->line0269]                             "_generator": {
//@[line10->line0270]                               "name": "bicep",
//@[line10->line0271]                               "version": "dev",
//@[line10->line0272]                               "templateHash": "1395382333336833730"
//@[line10->line0273]                             }
//@[line10->line0274]                           },
//@[line10->line0275]                           "resources": []
//@[line10->line0276]                         }
//@[line10->line0277]                       }
//@[line10->line0278]                     },
//@[line10->line0279]                     {
//@[line10->line0280]                       "type": "Microsoft.Resources/deployments",
//@[line10->line0281]                       "apiVersion": "2020-10-01",
//@[line10->line0282]                       "name": "otherSubscription",
//@[line10->line0283]                       "subscriptionId": "cd780357-07f5-49cc-b945-a3fe15863860",
//@[line10->line0284]                       "location": "[resourceGroup().location]",
//@[line10->line0285]                       "properties": {
//@[line10->line0286]                         "expressionEvaluationOptions": {
//@[line10->line0287]                           "scope": "inner"
//@[line10->line0288]                         },
//@[line10->line0289]                         "mode": "Incremental",
//@[line10->line0290]                         "template": {
//@[line10->line0291]                           "$schema": "https://schema.management.azure.com/schemas/2018-05-01/subscriptionDeploymentTemplate.json#",
//@[line10->line0292]                           "contentVersion": "1.0.0.0",
//@[line10->line0293]                           "metadata": {
//@[line10->line0294]                             "_generator": {
//@[line10->line0295]                               "name": "bicep",
//@[line10->line0296]                               "version": "dev",
//@[line10->line0297]                               "templateHash": "1395382333336833730"
//@[line10->line0298]                             }
//@[line10->line0299]                           },
//@[line10->line0300]                           "resources": []
//@[line10->line0301]                         }
//@[line10->line0302]                       }
//@[line10->line0303]                     }
//@[line10->line0304]                   ],
//@[line10->line0305]                   "outputs": {
//@[line10->line0306]                     "myOutput": {
//@[line10->line0307]                       "type": "string",
//@[line10->line0308]                       "value": "[reference(tenantResourceId('Microsoft.Resources/deployments', 'myTenantMod'), '2020-10-01').outputs.myOutput.value]"
//@[line10->line0309]                     },
//@[line10->line0310]                     "myOutputResourceGroup": {
//@[line10->line0311]                       "type": "string",
//@[line10->line0312]                       "value": "[reference(extensionResourceId(format('/subscriptions/{0}/resourceGroups/{1}', 'db90cfef-a146-4f67-b32f-b263518bd216', 'myOtherRg'), 'Microsoft.Resources/deployments', 'myOtherResourceGroup'), '2020-10-01').outputs.myOutput.value]"
//@[line10->line0313]                     }
//@[line10->line0314]                   }
//@[line10->line0315]                 }
//@[line10->line0316]               }
//@[line10->line0317]             },
//@[line10->line0318]             {
//@[line10->line0319]               "type": "Microsoft.Resources/deployments",
//@[line10->line0320]               "apiVersion": "2020-10-01",
//@[line10->line0321]               "name": "myResourceGroupMod2",
//@[line10->line0322]               "resourceGroup": "myRg",
//@[line10->line0323]               "properties": {
//@[line10->line0324]                 "expressionEvaluationOptions": {
//@[line10->line0325]                   "scope": "inner"
//@[line10->line0326]                 },
//@[line10->line0327]                 "mode": "Incremental",
//@[line10->line0328]                 "template": {
//@[line10->line0329]                   "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@[line10->line0330]                   "contentVersion": "1.0.0.0",
//@[line10->line0331]                   "metadata": {
//@[line10->line0332]                     "_generator": {
//@[line10->line0333]                       "name": "bicep",
//@[line10->line0334]                       "version": "dev",
//@[line10->line0335]                       "templateHash": "3773512362905547979"
//@[line10->line0336]                     }
//@[line10->line0337]                   },
//@[line10->line0338]                   "resources": [
//@[line10->line0339]                     {
//@[line10->line0340]                       "type": "Microsoft.Resources/deployments",
//@[line10->line0341]                       "apiVersion": "2020-10-01",
//@[line10->line0342]                       "name": "myTenantMod",
//@[line10->line0343]                       "scope": "/",
//@[line10->line0344]                       "location": "[resourceGroup().location]",
//@[line10->line0345]                       "properties": {
//@[line10->line0346]                         "expressionEvaluationOptions": {
//@[line10->line0347]                           "scope": "inner"
//@[line10->line0348]                         },
//@[line10->line0349]                         "mode": "Incremental",
//@[line10->line0350]                         "template": {
//@[line10->line0351]                           "$schema": "https://schema.management.azure.com/schemas/2019-08-01/tenantDeploymentTemplate.json#",
//@[line10->line0352]                           "contentVersion": "1.0.0.0",
//@[line10->line0353]                           "metadata": {
//@[line10->line0354]                             "_generator": {
//@[line10->line0355]                               "name": "bicep",
//@[line10->line0356]                               "version": "dev",
//@[line10->line0357]                               "templateHash": "15729984543815100695"
//@[line10->line0358]                             }
//@[line10->line0359]                           },
//@[line10->line0360]                           "resources": [],
//@[line10->line0361]                           "outputs": {
//@[line10->line0362]                             "myOutput": {
//@[line10->line0363]                               "type": "string",
//@[line10->line0364]                               "value": "hello!"
//@[line10->line0365]                             }
//@[line10->line0366]                           }
//@[line10->line0367]                         }
//@[line10->line0368]                       }
//@[line10->line0369]                     },
//@[line10->line0370]                     {
//@[line10->line0371]                       "type": "Microsoft.Resources/deployments",
//@[line10->line0372]                       "apiVersion": "2020-10-01",
//@[line10->line0373]                       "name": "myOtherResourceGroup",
//@[line10->line0374]                       "subscriptionId": "db90cfef-a146-4f67-b32f-b263518bd216",
//@[line10->line0375]                       "resourceGroup": "myOtherRg",
//@[line10->line0376]                       "properties": {
//@[line10->line0377]                         "expressionEvaluationOptions": {
//@[line10->line0378]                           "scope": "inner"
//@[line10->line0379]                         },
//@[line10->line0380]                         "mode": "Incremental",
//@[line10->line0381]                         "template": {
//@[line10->line0382]                           "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@[line10->line0383]                           "contentVersion": "1.0.0.0",
//@[line10->line0384]                           "metadata": {
//@[line10->line0385]                             "_generator": {
//@[line10->line0386]                               "name": "bicep",
//@[line10->line0387]                               "version": "dev",
//@[line10->line0388]                               "templateHash": "7469434526073292388"
//@[line10->line0389]                             }
//@[line10->line0390]                           },
//@[line10->line0391]                           "resources": [],
//@[line10->line0392]                           "outputs": {
//@[line10->line0393]                             "myOutput": {
//@[line10->line0394]                               "type": "string",
//@[line10->line0395]                               "value": "hello!"
//@[line10->line0396]                             }
//@[line10->line0397]                           }
//@[line10->line0398]                         }
//@[line10->line0399]                       }
//@[line10->line0400]                     },
//@[line10->line0401]                     {
//@[line10->line0402]                       "type": "Microsoft.Resources/deployments",
//@[line10->line0403]                       "apiVersion": "2020-10-01",
//@[line10->line0404]                       "name": "mySubscription",
//@[line10->line0405]                       "subscriptionId": "[subscription().subscriptionId]",
//@[line10->line0406]                       "location": "[resourceGroup().location]",
//@[line10->line0407]                       "properties": {
//@[line10->line0408]                         "expressionEvaluationOptions": {
//@[line10->line0409]                           "scope": "inner"
//@[line10->line0410]                         },
//@[line10->line0411]                         "mode": "Incremental",
//@[line10->line0412]                         "template": {
//@[line10->line0413]                           "$schema": "https://schema.management.azure.com/schemas/2018-05-01/subscriptionDeploymentTemplate.json#",
//@[line10->line0414]                           "contentVersion": "1.0.0.0",
//@[line10->line0415]                           "metadata": {
//@[line10->line0416]                             "_generator": {
//@[line10->line0417]                               "name": "bicep",
//@[line10->line0418]                               "version": "dev",
//@[line10->line0419]                               "templateHash": "1395382333336833730"
//@[line10->line0420]                             }
//@[line10->line0421]                           },
//@[line10->line0422]                           "resources": []
//@[line10->line0423]                         }
//@[line10->line0424]                       }
//@[line10->line0425]                     },
//@[line10->line0426]                     {
//@[line10->line0427]                       "type": "Microsoft.Resources/deployments",
//@[line10->line0428]                       "apiVersion": "2020-10-01",
//@[line10->line0429]                       "name": "otherSubscription",
//@[line10->line0430]                       "subscriptionId": "cd780357-07f5-49cc-b945-a3fe15863860",
//@[line10->line0431]                       "location": "[resourceGroup().location]",
//@[line10->line0432]                       "properties": {
//@[line10->line0433]                         "expressionEvaluationOptions": {
//@[line10->line0434]                           "scope": "inner"
//@[line10->line0435]                         },
//@[line10->line0436]                         "mode": "Incremental",
//@[line10->line0437]                         "template": {
//@[line10->line0438]                           "$schema": "https://schema.management.azure.com/schemas/2018-05-01/subscriptionDeploymentTemplate.json#",
//@[line10->line0439]                           "contentVersion": "1.0.0.0",
//@[line10->line0440]                           "metadata": {
//@[line10->line0441]                             "_generator": {
//@[line10->line0442]                               "name": "bicep",
//@[line10->line0443]                               "version": "dev",
//@[line10->line0444]                               "templateHash": "1395382333336833730"
//@[line10->line0445]                             }
//@[line10->line0446]                           },
//@[line10->line0447]                           "resources": []
//@[line10->line0448]                         }
//@[line10->line0449]                       }
//@[line10->line0450]                     }
//@[line10->line0451]                   ],
//@[line10->line0452]                   "outputs": {
//@[line10->line0453]                     "myOutput": {
//@[line10->line0454]                       "type": "string",
//@[line10->line0455]                       "value": "[reference(tenantResourceId('Microsoft.Resources/deployments', 'myTenantMod'), '2020-10-01').outputs.myOutput.value]"
//@[line10->line0456]                     },
//@[line10->line0457]                     "myOutputResourceGroup": {
//@[line10->line0458]                       "type": "string",
//@[line10->line0459]                       "value": "[reference(extensionResourceId(format('/subscriptions/{0}/resourceGroups/{1}', 'db90cfef-a146-4f67-b32f-b263518bd216', 'myOtherRg'), 'Microsoft.Resources/deployments', 'myOtherResourceGroup'), '2020-10-01').outputs.myOutput.value]"
//@[line10->line0460]                     }
//@[line10->line0461]                   }
//@[line10->line0462]                 }
//@[line10->line0463]               }
//@[line10->line0464]             },
//@[line10->line0465]             {
//@[line10->line0466]               "type": "Microsoft.Resources/deployments",
//@[line10->line0467]               "apiVersion": "2020-10-01",
//@[line10->line0468]               "name": "myResourceGroupMod3",
//@[line10->line0469]               "subscriptionId": "subId",
//@[line10->line0470]               "resourceGroup": "myRg",
//@[line10->line0471]               "properties": {
//@[line10->line0472]                 "expressionEvaluationOptions": {
//@[line10->line0473]                   "scope": "inner"
//@[line10->line0474]                 },
//@[line10->line0475]                 "mode": "Incremental",
//@[line10->line0476]                 "template": {
//@[line10->line0477]                   "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@[line10->line0478]                   "contentVersion": "1.0.0.0",
//@[line10->line0479]                   "metadata": {
//@[line10->line0480]                     "_generator": {
//@[line10->line0481]                       "name": "bicep",
//@[line10->line0482]                       "version": "dev",
//@[line10->line0483]                       "templateHash": "3773512362905547979"
//@[line10->line0484]                     }
//@[line10->line0485]                   },
//@[line10->line0486]                   "resources": [
//@[line10->line0487]                     {
//@[line10->line0488]                       "type": "Microsoft.Resources/deployments",
//@[line10->line0489]                       "apiVersion": "2020-10-01",
//@[line10->line0490]                       "name": "myTenantMod",
//@[line10->line0491]                       "scope": "/",
//@[line10->line0492]                       "location": "[resourceGroup().location]",
//@[line10->line0493]                       "properties": {
//@[line10->line0494]                         "expressionEvaluationOptions": {
//@[line10->line0495]                           "scope": "inner"
//@[line10->line0496]                         },
//@[line10->line0497]                         "mode": "Incremental",
//@[line10->line0498]                         "template": {
//@[line10->line0499]                           "$schema": "https://schema.management.azure.com/schemas/2019-08-01/tenantDeploymentTemplate.json#",
//@[line10->line0500]                           "contentVersion": "1.0.0.0",
//@[line10->line0501]                           "metadata": {
//@[line10->line0502]                             "_generator": {
//@[line10->line0503]                               "name": "bicep",
//@[line10->line0504]                               "version": "dev",
//@[line10->line0505]                               "templateHash": "15729984543815100695"
//@[line10->line0506]                             }
//@[line10->line0507]                           },
//@[line10->line0508]                           "resources": [],
//@[line10->line0509]                           "outputs": {
//@[line10->line0510]                             "myOutput": {
//@[line10->line0511]                               "type": "string",
//@[line10->line0512]                               "value": "hello!"
//@[line10->line0513]                             }
//@[line10->line0514]                           }
//@[line10->line0515]                         }
//@[line10->line0516]                       }
//@[line10->line0517]                     },
//@[line10->line0518]                     {
//@[line10->line0519]                       "type": "Microsoft.Resources/deployments",
//@[line10->line0520]                       "apiVersion": "2020-10-01",
//@[line10->line0521]                       "name": "myOtherResourceGroup",
//@[line10->line0522]                       "subscriptionId": "db90cfef-a146-4f67-b32f-b263518bd216",
//@[line10->line0523]                       "resourceGroup": "myOtherRg",
//@[line10->line0524]                       "properties": {
//@[line10->line0525]                         "expressionEvaluationOptions": {
//@[line10->line0526]                           "scope": "inner"
//@[line10->line0527]                         },
//@[line10->line0528]                         "mode": "Incremental",
//@[line10->line0529]                         "template": {
//@[line10->line0530]                           "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@[line10->line0531]                           "contentVersion": "1.0.0.0",
//@[line10->line0532]                           "metadata": {
//@[line10->line0533]                             "_generator": {
//@[line10->line0534]                               "name": "bicep",
//@[line10->line0535]                               "version": "dev",
//@[line10->line0536]                               "templateHash": "7469434526073292388"
//@[line10->line0537]                             }
//@[line10->line0538]                           },
//@[line10->line0539]                           "resources": [],
//@[line10->line0540]                           "outputs": {
//@[line10->line0541]                             "myOutput": {
//@[line10->line0542]                               "type": "string",
//@[line10->line0543]                               "value": "hello!"
//@[line10->line0544]                             }
//@[line10->line0545]                           }
//@[line10->line0546]                         }
//@[line10->line0547]                       }
//@[line10->line0548]                     },
//@[line10->line0549]                     {
//@[line10->line0550]                       "type": "Microsoft.Resources/deployments",
//@[line10->line0551]                       "apiVersion": "2020-10-01",
//@[line10->line0552]                       "name": "mySubscription",
//@[line10->line0553]                       "subscriptionId": "[subscription().subscriptionId]",
//@[line10->line0554]                       "location": "[resourceGroup().location]",
//@[line10->line0555]                       "properties": {
//@[line10->line0556]                         "expressionEvaluationOptions": {
//@[line10->line0557]                           "scope": "inner"
//@[line10->line0558]                         },
//@[line10->line0559]                         "mode": "Incremental",
//@[line10->line0560]                         "template": {
//@[line10->line0561]                           "$schema": "https://schema.management.azure.com/schemas/2018-05-01/subscriptionDeploymentTemplate.json#",
//@[line10->line0562]                           "contentVersion": "1.0.0.0",
//@[line10->line0563]                           "metadata": {
//@[line10->line0564]                             "_generator": {
//@[line10->line0565]                               "name": "bicep",
//@[line10->line0566]                               "version": "dev",
//@[line10->line0567]                               "templateHash": "1395382333336833730"
//@[line10->line0568]                             }
//@[line10->line0569]                           },
//@[line10->line0570]                           "resources": []
//@[line10->line0571]                         }
//@[line10->line0572]                       }
//@[line10->line0573]                     },
//@[line10->line0574]                     {
//@[line10->line0575]                       "type": "Microsoft.Resources/deployments",
//@[line10->line0576]                       "apiVersion": "2020-10-01",
//@[line10->line0577]                       "name": "otherSubscription",
//@[line10->line0578]                       "subscriptionId": "cd780357-07f5-49cc-b945-a3fe15863860",
//@[line10->line0579]                       "location": "[resourceGroup().location]",
//@[line10->line0580]                       "properties": {
//@[line10->line0581]                         "expressionEvaluationOptions": {
//@[line10->line0582]                           "scope": "inner"
//@[line10->line0583]                         },
//@[line10->line0584]                         "mode": "Incremental",
//@[line10->line0585]                         "template": {
//@[line10->line0586]                           "$schema": "https://schema.management.azure.com/schemas/2018-05-01/subscriptionDeploymentTemplate.json#",
//@[line10->line0587]                           "contentVersion": "1.0.0.0",
//@[line10->line0588]                           "metadata": {
//@[line10->line0589]                             "_generator": {
//@[line10->line0590]                               "name": "bicep",
//@[line10->line0591]                               "version": "dev",
//@[line10->line0592]                               "templateHash": "1395382333336833730"
//@[line10->line0593]                             }
//@[line10->line0594]                           },
//@[line10->line0595]                           "resources": []
//@[line10->line0596]                         }
//@[line10->line0597]                       }
//@[line10->line0598]                     }
//@[line10->line0599]                   ],
//@[line10->line0600]                   "outputs": {
//@[line10->line0601]                     "myOutput": {
//@[line10->line0602]                       "type": "string",
//@[line10->line0603]                       "value": "[reference(tenantResourceId('Microsoft.Resources/deployments', 'myTenantMod'), '2020-10-01').outputs.myOutput.value]"
//@[line10->line0604]                     },
//@[line10->line0605]                     "myOutputResourceGroup": {
//@[line10->line0606]                       "type": "string",
//@[line10->line0607]                       "value": "[reference(extensionResourceId(format('/subscriptions/{0}/resourceGroups/{1}', 'db90cfef-a146-4f67-b32f-b263518bd216', 'myOtherRg'), 'Microsoft.Resources/deployments', 'myOtherResourceGroup'), '2020-10-01').outputs.myOutput.value]"
//@[line10->line0608]                     }
//@[line10->line0609]                   }
//@[line10->line0610]                 }
//@[line10->line0611]               }
//@[line10->line0612]             },
//@[line10->line0613]             {
//@[line10->line0614]               "type": "Microsoft.Resources/deployments",
//@[line10->line0615]               "apiVersion": "2020-10-01",
//@[line10->line0616]               "name": "myTenantMod",
//@[line10->line0617]               "scope": "/",
//@[line10->line0618]               "location": "[deployment().location]",
//@[line10->line0619]               "properties": {
//@[line10->line0620]                 "expressionEvaluationOptions": {
//@[line10->line0621]                   "scope": "inner"
//@[line10->line0622]                 },
//@[line10->line0623]                 "mode": "Incremental",
//@[line10->line0624]                 "template": {
//@[line10->line0625]                   "$schema": "https://schema.management.azure.com/schemas/2019-08-01/tenantDeploymentTemplate.json#",
//@[line10->line0626]                   "contentVersion": "1.0.0.0",
//@[line10->line0627]                   "metadata": {
//@[line10->line0628]                     "_generator": {
//@[line10->line0629]                       "name": "bicep",
//@[line10->line0630]                       "version": "dev",
//@[line10->line0631]                       "templateHash": "15729984543815100695"
//@[line10->line0632]                     }
//@[line10->line0633]                   },
//@[line10->line0634]                   "resources": [],
//@[line10->line0635]                   "outputs": {
//@[line10->line0636]                     "myOutput": {
//@[line10->line0637]                       "type": "string",
//@[line10->line0638]                       "value": "hello!"
//@[line10->line0639]                     }
//@[line10->line0640]                   }
//@[line10->line0641]                 }
//@[line10->line0642]               }
//@[line10->line0643]             }
//@[line10->line0644]           ],
//@[line10->line0645]           "outputs": {
//@[line10->line0646]             "myOutput": {
//@[line10->line0647]               "type": "string",
//@[line10->line0648]               "value": "[reference(tenantResourceId('Microsoft.Resources/deployments', 'myTenantMod'), '2020-10-01').outputs.myOutput.value]"
//@[line10->line0649]             },
//@[line10->line0650]             "myOutputRgMod": {
//@[line10->line0651]               "type": "string",
//@[line10->line0652]               "value": "[reference(extensionResourceId(format('/subscriptions/{0}/resourceGroups/{1}', subscription().subscriptionId, 'myRg'), 'Microsoft.Resources/deployments', 'myResourceGroupMod'), '2020-10-01').outputs.myOutput.value]"
//@[line10->line0653]             },
//@[line10->line0654]             "myOutputRgMod2": {
//@[line10->line0655]               "type": "string",
//@[line10->line0656]               "value": "[reference(extensionResourceId(format('/subscriptions/{0}/resourceGroups/{1}', subscription().subscriptionId, 'myRg'), 'Microsoft.Resources/deployments', 'myResourceGroupMod2'), '2020-10-01').outputs.myOutput.value]"
//@[line10->line0657]             }
//@[line10->line0658]           }
//@[line10->line0659]         }
//@[line10->line0660]       }
//@[line10->line0661]     },
  name: 'mySubscriptionMod'
//@[line11->line0152]       "name": "mySubscriptionMod",
  scope: subscription('ee44cd78-68c6-43d9-874e-e684ec8d1191')
//@[line12->line0153]       "subscriptionId": "ee44cd78-68c6-43d9-874e-e684ec8d1191",
}

module mySubscriptionModWithCondition 'modules/subscription.bicep' = if (length('foo') == 3) {
//@[line15->line0662]     {
//@[line15->line0663]       "condition": "[equals(length('foo'), 3)]",
//@[line15->line0664]       "type": "Microsoft.Resources/deployments",
//@[line15->line0665]       "apiVersion": "2020-10-01",
//@[line15->line0668]       "location": "[deployment().location]",
//@[line15->line0669]       "properties": {
//@[line15->line0670]         "expressionEvaluationOptions": {
//@[line15->line0671]           "scope": "inner"
//@[line15->line0672]         },
//@[line15->line0673]         "mode": "Incremental",
//@[line15->line0674]         "template": {
//@[line15->line0675]           "$schema": "https://schema.management.azure.com/schemas/2018-05-01/subscriptionDeploymentTemplate.json#",
//@[line15->line0676]           "contentVersion": "1.0.0.0",
//@[line15->line0677]           "metadata": {
//@[line15->line0678]             "_generator": {
//@[line15->line0679]               "name": "bicep",
//@[line15->line0680]               "version": "dev",
//@[line15->line0681]               "templateHash": "12301588538908463385"
//@[line15->line0682]             }
//@[line15->line0683]           },
//@[line15->line0684]           "resources": [
//@[line15->line0685]             {
//@[line15->line0686]               "type": "Microsoft.Resources/deployments",
//@[line15->line0687]               "apiVersion": "2020-10-01",
//@[line15->line0688]               "name": "myResourceGroupMod",
//@[line15->line0689]               "resourceGroup": "myRg",
//@[line15->line0690]               "properties": {
//@[line15->line0691]                 "expressionEvaluationOptions": {
//@[line15->line0692]                   "scope": "inner"
//@[line15->line0693]                 },
//@[line15->line0694]                 "mode": "Incremental",
//@[line15->line0695]                 "template": {
//@[line15->line0696]                   "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@[line15->line0697]                   "contentVersion": "1.0.0.0",
//@[line15->line0698]                   "metadata": {
//@[line15->line0699]                     "_generator": {
//@[line15->line0700]                       "name": "bicep",
//@[line15->line0701]                       "version": "dev",
//@[line15->line0702]                       "templateHash": "3773512362905547979"
//@[line15->line0703]                     }
//@[line15->line0704]                   },
//@[line15->line0705]                   "resources": [
//@[line15->line0706]                     {
//@[line15->line0707]                       "type": "Microsoft.Resources/deployments",
//@[line15->line0708]                       "apiVersion": "2020-10-01",
//@[line15->line0709]                       "name": "myTenantMod",
//@[line15->line0710]                       "scope": "/",
//@[line15->line0711]                       "location": "[resourceGroup().location]",
//@[line15->line0712]                       "properties": {
//@[line15->line0713]                         "expressionEvaluationOptions": {
//@[line15->line0714]                           "scope": "inner"
//@[line15->line0715]                         },
//@[line15->line0716]                         "mode": "Incremental",
//@[line15->line0717]                         "template": {
//@[line15->line0718]                           "$schema": "https://schema.management.azure.com/schemas/2019-08-01/tenantDeploymentTemplate.json#",
//@[line15->line0719]                           "contentVersion": "1.0.0.0",
//@[line15->line0720]                           "metadata": {
//@[line15->line0721]                             "_generator": {
//@[line15->line0722]                               "name": "bicep",
//@[line15->line0723]                               "version": "dev",
//@[line15->line0724]                               "templateHash": "15729984543815100695"
//@[line15->line0725]                             }
//@[line15->line0726]                           },
//@[line15->line0727]                           "resources": [],
//@[line15->line0728]                           "outputs": {
//@[line15->line0729]                             "myOutput": {
//@[line15->line0730]                               "type": "string",
//@[line15->line0731]                               "value": "hello!"
//@[line15->line0732]                             }
//@[line15->line0733]                           }
//@[line15->line0734]                         }
//@[line15->line0735]                       }
//@[line15->line0736]                     },
//@[line15->line0737]                     {
//@[line15->line0738]                       "type": "Microsoft.Resources/deployments",
//@[line15->line0739]                       "apiVersion": "2020-10-01",
//@[line15->line0740]                       "name": "myOtherResourceGroup",
//@[line15->line0741]                       "subscriptionId": "db90cfef-a146-4f67-b32f-b263518bd216",
//@[line15->line0742]                       "resourceGroup": "myOtherRg",
//@[line15->line0743]                       "properties": {
//@[line15->line0744]                         "expressionEvaluationOptions": {
//@[line15->line0745]                           "scope": "inner"
//@[line15->line0746]                         },
//@[line15->line0747]                         "mode": "Incremental",
//@[line15->line0748]                         "template": {
//@[line15->line0749]                           "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@[line15->line0750]                           "contentVersion": "1.0.0.0",
//@[line15->line0751]                           "metadata": {
//@[line15->line0752]                             "_generator": {
//@[line15->line0753]                               "name": "bicep",
//@[line15->line0754]                               "version": "dev",
//@[line15->line0755]                               "templateHash": "7469434526073292388"
//@[line15->line0756]                             }
//@[line15->line0757]                           },
//@[line15->line0758]                           "resources": [],
//@[line15->line0759]                           "outputs": {
//@[line15->line0760]                             "myOutput": {
//@[line15->line0761]                               "type": "string",
//@[line15->line0762]                               "value": "hello!"
//@[line15->line0763]                             }
//@[line15->line0764]                           }
//@[line15->line0765]                         }
//@[line15->line0766]                       }
//@[line15->line0767]                     },
//@[line15->line0768]                     {
//@[line15->line0769]                       "type": "Microsoft.Resources/deployments",
//@[line15->line0770]                       "apiVersion": "2020-10-01",
//@[line15->line0771]                       "name": "mySubscription",
//@[line15->line0772]                       "subscriptionId": "[subscription().subscriptionId]",
//@[line15->line0773]                       "location": "[resourceGroup().location]",
//@[line15->line0774]                       "properties": {
//@[line15->line0775]                         "expressionEvaluationOptions": {
//@[line15->line0776]                           "scope": "inner"
//@[line15->line0777]                         },
//@[line15->line0778]                         "mode": "Incremental",
//@[line15->line0779]                         "template": {
//@[line15->line0780]                           "$schema": "https://schema.management.azure.com/schemas/2018-05-01/subscriptionDeploymentTemplate.json#",
//@[line15->line0781]                           "contentVersion": "1.0.0.0",
//@[line15->line0782]                           "metadata": {
//@[line15->line0783]                             "_generator": {
//@[line15->line0784]                               "name": "bicep",
//@[line15->line0785]                               "version": "dev",
//@[line15->line0786]                               "templateHash": "1395382333336833730"
//@[line15->line0787]                             }
//@[line15->line0788]                           },
//@[line15->line0789]                           "resources": []
//@[line15->line0790]                         }
//@[line15->line0791]                       }
//@[line15->line0792]                     },
//@[line15->line0793]                     {
//@[line15->line0794]                       "type": "Microsoft.Resources/deployments",
//@[line15->line0795]                       "apiVersion": "2020-10-01",
//@[line15->line0796]                       "name": "otherSubscription",
//@[line15->line0797]                       "subscriptionId": "cd780357-07f5-49cc-b945-a3fe15863860",
//@[line15->line0798]                       "location": "[resourceGroup().location]",
//@[line15->line0799]                       "properties": {
//@[line15->line0800]                         "expressionEvaluationOptions": {
//@[line15->line0801]                           "scope": "inner"
//@[line15->line0802]                         },
//@[line15->line0803]                         "mode": "Incremental",
//@[line15->line0804]                         "template": {
//@[line15->line0805]                           "$schema": "https://schema.management.azure.com/schemas/2018-05-01/subscriptionDeploymentTemplate.json#",
//@[line15->line0806]                           "contentVersion": "1.0.0.0",
//@[line15->line0807]                           "metadata": {
//@[line15->line0808]                             "_generator": {
//@[line15->line0809]                               "name": "bicep",
//@[line15->line0810]                               "version": "dev",
//@[line15->line0811]                               "templateHash": "1395382333336833730"
//@[line15->line0812]                             }
//@[line15->line0813]                           },
//@[line15->line0814]                           "resources": []
//@[line15->line0815]                         }
//@[line15->line0816]                       }
//@[line15->line0817]                     }
//@[line15->line0818]                   ],
//@[line15->line0819]                   "outputs": {
//@[line15->line0820]                     "myOutput": {
//@[line15->line0821]                       "type": "string",
//@[line15->line0822]                       "value": "[reference(tenantResourceId('Microsoft.Resources/deployments', 'myTenantMod'), '2020-10-01').outputs.myOutput.value]"
//@[line15->line0823]                     },
//@[line15->line0824]                     "myOutputResourceGroup": {
//@[line15->line0825]                       "type": "string",
//@[line15->line0826]                       "value": "[reference(extensionResourceId(format('/subscriptions/{0}/resourceGroups/{1}', 'db90cfef-a146-4f67-b32f-b263518bd216', 'myOtherRg'), 'Microsoft.Resources/deployments', 'myOtherResourceGroup'), '2020-10-01').outputs.myOutput.value]"
//@[line15->line0827]                     }
//@[line15->line0828]                   }
//@[line15->line0829]                 }
//@[line15->line0830]               }
//@[line15->line0831]             },
//@[line15->line0832]             {
//@[line15->line0833]               "type": "Microsoft.Resources/deployments",
//@[line15->line0834]               "apiVersion": "2020-10-01",
//@[line15->line0835]               "name": "myResourceGroupMod2",
//@[line15->line0836]               "resourceGroup": "myRg",
//@[line15->line0837]               "properties": {
//@[line15->line0838]                 "expressionEvaluationOptions": {
//@[line15->line0839]                   "scope": "inner"
//@[line15->line0840]                 },
//@[line15->line0841]                 "mode": "Incremental",
//@[line15->line0842]                 "template": {
//@[line15->line0843]                   "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@[line15->line0844]                   "contentVersion": "1.0.0.0",
//@[line15->line0845]                   "metadata": {
//@[line15->line0846]                     "_generator": {
//@[line15->line0847]                       "name": "bicep",
//@[line15->line0848]                       "version": "dev",
//@[line15->line0849]                       "templateHash": "3773512362905547979"
//@[line15->line0850]                     }
//@[line15->line0851]                   },
//@[line15->line0852]                   "resources": [
//@[line15->line0853]                     {
//@[line15->line0854]                       "type": "Microsoft.Resources/deployments",
//@[line15->line0855]                       "apiVersion": "2020-10-01",
//@[line15->line0856]                       "name": "myTenantMod",
//@[line15->line0857]                       "scope": "/",
//@[line15->line0858]                       "location": "[resourceGroup().location]",
//@[line15->line0859]                       "properties": {
//@[line15->line0860]                         "expressionEvaluationOptions": {
//@[line15->line0861]                           "scope": "inner"
//@[line15->line0862]                         },
//@[line15->line0863]                         "mode": "Incremental",
//@[line15->line0864]                         "template": {
//@[line15->line0865]                           "$schema": "https://schema.management.azure.com/schemas/2019-08-01/tenantDeploymentTemplate.json#",
//@[line15->line0866]                           "contentVersion": "1.0.0.0",
//@[line15->line0867]                           "metadata": {
//@[line15->line0868]                             "_generator": {
//@[line15->line0869]                               "name": "bicep",
//@[line15->line0870]                               "version": "dev",
//@[line15->line0871]                               "templateHash": "15729984543815100695"
//@[line15->line0872]                             }
//@[line15->line0873]                           },
//@[line15->line0874]                           "resources": [],
//@[line15->line0875]                           "outputs": {
//@[line15->line0876]                             "myOutput": {
//@[line15->line0877]                               "type": "string",
//@[line15->line0878]                               "value": "hello!"
//@[line15->line0879]                             }
//@[line15->line0880]                           }
//@[line15->line0881]                         }
//@[line15->line0882]                       }
//@[line15->line0883]                     },
//@[line15->line0884]                     {
//@[line15->line0885]                       "type": "Microsoft.Resources/deployments",
//@[line15->line0886]                       "apiVersion": "2020-10-01",
//@[line15->line0887]                       "name": "myOtherResourceGroup",
//@[line15->line0888]                       "subscriptionId": "db90cfef-a146-4f67-b32f-b263518bd216",
//@[line15->line0889]                       "resourceGroup": "myOtherRg",
//@[line15->line0890]                       "properties": {
//@[line15->line0891]                         "expressionEvaluationOptions": {
//@[line15->line0892]                           "scope": "inner"
//@[line15->line0893]                         },
//@[line15->line0894]                         "mode": "Incremental",
//@[line15->line0895]                         "template": {
//@[line15->line0896]                           "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@[line15->line0897]                           "contentVersion": "1.0.0.0",
//@[line15->line0898]                           "metadata": {
//@[line15->line0899]                             "_generator": {
//@[line15->line0900]                               "name": "bicep",
//@[line15->line0901]                               "version": "dev",
//@[line15->line0902]                               "templateHash": "7469434526073292388"
//@[line15->line0903]                             }
//@[line15->line0904]                           },
//@[line15->line0905]                           "resources": [],
//@[line15->line0906]                           "outputs": {
//@[line15->line0907]                             "myOutput": {
//@[line15->line0908]                               "type": "string",
//@[line15->line0909]                               "value": "hello!"
//@[line15->line0910]                             }
//@[line15->line0911]                           }
//@[line15->line0912]                         }
//@[line15->line0913]                       }
//@[line15->line0914]                     },
//@[line15->line0915]                     {
//@[line15->line0916]                       "type": "Microsoft.Resources/deployments",
//@[line15->line0917]                       "apiVersion": "2020-10-01",
//@[line15->line0918]                       "name": "mySubscription",
//@[line15->line0919]                       "subscriptionId": "[subscription().subscriptionId]",
//@[line15->line0920]                       "location": "[resourceGroup().location]",
//@[line15->line0921]                       "properties": {
//@[line15->line0922]                         "expressionEvaluationOptions": {
//@[line15->line0923]                           "scope": "inner"
//@[line15->line0924]                         },
//@[line15->line0925]                         "mode": "Incremental",
//@[line15->line0926]                         "template": {
//@[line15->line0927]                           "$schema": "https://schema.management.azure.com/schemas/2018-05-01/subscriptionDeploymentTemplate.json#",
//@[line15->line0928]                           "contentVersion": "1.0.0.0",
//@[line15->line0929]                           "metadata": {
//@[line15->line0930]                             "_generator": {
//@[line15->line0931]                               "name": "bicep",
//@[line15->line0932]                               "version": "dev",
//@[line15->line0933]                               "templateHash": "1395382333336833730"
//@[line15->line0934]                             }
//@[line15->line0935]                           },
//@[line15->line0936]                           "resources": []
//@[line15->line0937]                         }
//@[line15->line0938]                       }
//@[line15->line0939]                     },
//@[line15->line0940]                     {
//@[line15->line0941]                       "type": "Microsoft.Resources/deployments",
//@[line15->line0942]                       "apiVersion": "2020-10-01",
//@[line15->line0943]                       "name": "otherSubscription",
//@[line15->line0944]                       "subscriptionId": "cd780357-07f5-49cc-b945-a3fe15863860",
//@[line15->line0945]                       "location": "[resourceGroup().location]",
//@[line15->line0946]                       "properties": {
//@[line15->line0947]                         "expressionEvaluationOptions": {
//@[line15->line0948]                           "scope": "inner"
//@[line15->line0949]                         },
//@[line15->line0950]                         "mode": "Incremental",
//@[line15->line0951]                         "template": {
//@[line15->line0952]                           "$schema": "https://schema.management.azure.com/schemas/2018-05-01/subscriptionDeploymentTemplate.json#",
//@[line15->line0953]                           "contentVersion": "1.0.0.0",
//@[line15->line0954]                           "metadata": {
//@[line15->line0955]                             "_generator": {
//@[line15->line0956]                               "name": "bicep",
//@[line15->line0957]                               "version": "dev",
//@[line15->line0958]                               "templateHash": "1395382333336833730"
//@[line15->line0959]                             }
//@[line15->line0960]                           },
//@[line15->line0961]                           "resources": []
//@[line15->line0962]                         }
//@[line15->line0963]                       }
//@[line15->line0964]                     }
//@[line15->line0965]                   ],
//@[line15->line0966]                   "outputs": {
//@[line15->line0967]                     "myOutput": {
//@[line15->line0968]                       "type": "string",
//@[line15->line0969]                       "value": "[reference(tenantResourceId('Microsoft.Resources/deployments', 'myTenantMod'), '2020-10-01').outputs.myOutput.value]"
//@[line15->line0970]                     },
//@[line15->line0971]                     "myOutputResourceGroup": {
//@[line15->line0972]                       "type": "string",
//@[line15->line0973]                       "value": "[reference(extensionResourceId(format('/subscriptions/{0}/resourceGroups/{1}', 'db90cfef-a146-4f67-b32f-b263518bd216', 'myOtherRg'), 'Microsoft.Resources/deployments', 'myOtherResourceGroup'), '2020-10-01').outputs.myOutput.value]"
//@[line15->line0974]                     }
//@[line15->line0975]                   }
//@[line15->line0976]                 }
//@[line15->line0977]               }
//@[line15->line0978]             },
//@[line15->line0979]             {
//@[line15->line0980]               "type": "Microsoft.Resources/deployments",
//@[line15->line0981]               "apiVersion": "2020-10-01",
//@[line15->line0982]               "name": "myResourceGroupMod3",
//@[line15->line0983]               "subscriptionId": "subId",
//@[line15->line0984]               "resourceGroup": "myRg",
//@[line15->line0985]               "properties": {
//@[line15->line0986]                 "expressionEvaluationOptions": {
//@[line15->line0987]                   "scope": "inner"
//@[line15->line0988]                 },
//@[line15->line0989]                 "mode": "Incremental",
//@[line15->line0990]                 "template": {
//@[line15->line0991]                   "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@[line15->line0992]                   "contentVersion": "1.0.0.0",
//@[line15->line0993]                   "metadata": {
//@[line15->line0994]                     "_generator": {
//@[line15->line0995]                       "name": "bicep",
//@[line15->line0996]                       "version": "dev",
//@[line15->line0997]                       "templateHash": "3773512362905547979"
//@[line15->line0998]                     }
//@[line15->line0999]                   },
//@[line15->line1000]                   "resources": [
//@[line15->line1001]                     {
//@[line15->line1002]                       "type": "Microsoft.Resources/deployments",
//@[line15->line1003]                       "apiVersion": "2020-10-01",
//@[line15->line1004]                       "name": "myTenantMod",
//@[line15->line1005]                       "scope": "/",
//@[line15->line1006]                       "location": "[resourceGroup().location]",
//@[line15->line1007]                       "properties": {
//@[line15->line1008]                         "expressionEvaluationOptions": {
//@[line15->line1009]                           "scope": "inner"
//@[line15->line1010]                         },
//@[line15->line1011]                         "mode": "Incremental",
//@[line15->line1012]                         "template": {
//@[line15->line1013]                           "$schema": "https://schema.management.azure.com/schemas/2019-08-01/tenantDeploymentTemplate.json#",
//@[line15->line1014]                           "contentVersion": "1.0.0.0",
//@[line15->line1015]                           "metadata": {
//@[line15->line1016]                             "_generator": {
//@[line15->line1017]                               "name": "bicep",
//@[line15->line1018]                               "version": "dev",
//@[line15->line1019]                               "templateHash": "15729984543815100695"
//@[line15->line1020]                             }
//@[line15->line1021]                           },
//@[line15->line1022]                           "resources": [],
//@[line15->line1023]                           "outputs": {
//@[line15->line1024]                             "myOutput": {
//@[line15->line1025]                               "type": "string",
//@[line15->line1026]                               "value": "hello!"
//@[line15->line1027]                             }
//@[line15->line1028]                           }
//@[line15->line1029]                         }
//@[line15->line1030]                       }
//@[line15->line1031]                     },
//@[line15->line1032]                     {
//@[line15->line1033]                       "type": "Microsoft.Resources/deployments",
//@[line15->line1034]                       "apiVersion": "2020-10-01",
//@[line15->line1035]                       "name": "myOtherResourceGroup",
//@[line15->line1036]                       "subscriptionId": "db90cfef-a146-4f67-b32f-b263518bd216",
//@[line15->line1037]                       "resourceGroup": "myOtherRg",
//@[line15->line1038]                       "properties": {
//@[line15->line1039]                         "expressionEvaluationOptions": {
//@[line15->line1040]                           "scope": "inner"
//@[line15->line1041]                         },
//@[line15->line1042]                         "mode": "Incremental",
//@[line15->line1043]                         "template": {
//@[line15->line1044]                           "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@[line15->line1045]                           "contentVersion": "1.0.0.0",
//@[line15->line1046]                           "metadata": {
//@[line15->line1047]                             "_generator": {
//@[line15->line1048]                               "name": "bicep",
//@[line15->line1049]                               "version": "dev",
//@[line15->line1050]                               "templateHash": "7469434526073292388"
//@[line15->line1051]                             }
//@[line15->line1052]                           },
//@[line15->line1053]                           "resources": [],
//@[line15->line1054]                           "outputs": {
//@[line15->line1055]                             "myOutput": {
//@[line15->line1056]                               "type": "string",
//@[line15->line1057]                               "value": "hello!"
//@[line15->line1058]                             }
//@[line15->line1059]                           }
//@[line15->line1060]                         }
//@[line15->line1061]                       }
//@[line15->line1062]                     },
//@[line15->line1063]                     {
//@[line15->line1064]                       "type": "Microsoft.Resources/deployments",
//@[line15->line1065]                       "apiVersion": "2020-10-01",
//@[line15->line1066]                       "name": "mySubscription",
//@[line15->line1067]                       "subscriptionId": "[subscription().subscriptionId]",
//@[line15->line1068]                       "location": "[resourceGroup().location]",
//@[line15->line1069]                       "properties": {
//@[line15->line1070]                         "expressionEvaluationOptions": {
//@[line15->line1071]                           "scope": "inner"
//@[line15->line1072]                         },
//@[line15->line1073]                         "mode": "Incremental",
//@[line15->line1074]                         "template": {
//@[line15->line1075]                           "$schema": "https://schema.management.azure.com/schemas/2018-05-01/subscriptionDeploymentTemplate.json#",
//@[line15->line1076]                           "contentVersion": "1.0.0.0",
//@[line15->line1077]                           "metadata": {
//@[line15->line1078]                             "_generator": {
//@[line15->line1079]                               "name": "bicep",
//@[line15->line1080]                               "version": "dev",
//@[line15->line1081]                               "templateHash": "1395382333336833730"
//@[line15->line1082]                             }
//@[line15->line1083]                           },
//@[line15->line1084]                           "resources": []
//@[line15->line1085]                         }
//@[line15->line1086]                       }
//@[line15->line1087]                     },
//@[line15->line1088]                     {
//@[line15->line1089]                       "type": "Microsoft.Resources/deployments",
//@[line15->line1090]                       "apiVersion": "2020-10-01",
//@[line15->line1091]                       "name": "otherSubscription",
//@[line15->line1092]                       "subscriptionId": "cd780357-07f5-49cc-b945-a3fe15863860",
//@[line15->line1093]                       "location": "[resourceGroup().location]",
//@[line15->line1094]                       "properties": {
//@[line15->line1095]                         "expressionEvaluationOptions": {
//@[line15->line1096]                           "scope": "inner"
//@[line15->line1097]                         },
//@[line15->line1098]                         "mode": "Incremental",
//@[line15->line1099]                         "template": {
//@[line15->line1100]                           "$schema": "https://schema.management.azure.com/schemas/2018-05-01/subscriptionDeploymentTemplate.json#",
//@[line15->line1101]                           "contentVersion": "1.0.0.0",
//@[line15->line1102]                           "metadata": {
//@[line15->line1103]                             "_generator": {
//@[line15->line1104]                               "name": "bicep",
//@[line15->line1105]                               "version": "dev",
//@[line15->line1106]                               "templateHash": "1395382333336833730"
//@[line15->line1107]                             }
//@[line15->line1108]                           },
//@[line15->line1109]                           "resources": []
//@[line15->line1110]                         }
//@[line15->line1111]                       }
//@[line15->line1112]                     }
//@[line15->line1113]                   ],
//@[line15->line1114]                   "outputs": {
//@[line15->line1115]                     "myOutput": {
//@[line15->line1116]                       "type": "string",
//@[line15->line1117]                       "value": "[reference(tenantResourceId('Microsoft.Resources/deployments', 'myTenantMod'), '2020-10-01').outputs.myOutput.value]"
//@[line15->line1118]                     },
//@[line15->line1119]                     "myOutputResourceGroup": {
//@[line15->line1120]                       "type": "string",
//@[line15->line1121]                       "value": "[reference(extensionResourceId(format('/subscriptions/{0}/resourceGroups/{1}', 'db90cfef-a146-4f67-b32f-b263518bd216', 'myOtherRg'), 'Microsoft.Resources/deployments', 'myOtherResourceGroup'), '2020-10-01').outputs.myOutput.value]"
//@[line15->line1122]                     }
//@[line15->line1123]                   }
//@[line15->line1124]                 }
//@[line15->line1125]               }
//@[line15->line1126]             },
//@[line15->line1127]             {
//@[line15->line1128]               "type": "Microsoft.Resources/deployments",
//@[line15->line1129]               "apiVersion": "2020-10-01",
//@[line15->line1130]               "name": "myTenantMod",
//@[line15->line1131]               "scope": "/",
//@[line15->line1132]               "location": "[deployment().location]",
//@[line15->line1133]               "properties": {
//@[line15->line1134]                 "expressionEvaluationOptions": {
//@[line15->line1135]                   "scope": "inner"
//@[line15->line1136]                 },
//@[line15->line1137]                 "mode": "Incremental",
//@[line15->line1138]                 "template": {
//@[line15->line1139]                   "$schema": "https://schema.management.azure.com/schemas/2019-08-01/tenantDeploymentTemplate.json#",
//@[line15->line1140]                   "contentVersion": "1.0.0.0",
//@[line15->line1141]                   "metadata": {
//@[line15->line1142]                     "_generator": {
//@[line15->line1143]                       "name": "bicep",
//@[line15->line1144]                       "version": "dev",
//@[line15->line1145]                       "templateHash": "15729984543815100695"
//@[line15->line1146]                     }
//@[line15->line1147]                   },
//@[line15->line1148]                   "resources": [],
//@[line15->line1149]                   "outputs": {
//@[line15->line1150]                     "myOutput": {
//@[line15->line1151]                       "type": "string",
//@[line15->line1152]                       "value": "hello!"
//@[line15->line1153]                     }
//@[line15->line1154]                   }
//@[line15->line1155]                 }
//@[line15->line1156]               }
//@[line15->line1157]             }
//@[line15->line1158]           ],
//@[line15->line1159]           "outputs": {
//@[line15->line1160]             "myOutput": {
//@[line15->line1161]               "type": "string",
//@[line15->line1162]               "value": "[reference(tenantResourceId('Microsoft.Resources/deployments', 'myTenantMod'), '2020-10-01').outputs.myOutput.value]"
//@[line15->line1163]             },
//@[line15->line1164]             "myOutputRgMod": {
//@[line15->line1165]               "type": "string",
//@[line15->line1166]               "value": "[reference(extensionResourceId(format('/subscriptions/{0}/resourceGroups/{1}', subscription().subscriptionId, 'myRg'), 'Microsoft.Resources/deployments', 'myResourceGroupMod'), '2020-10-01').outputs.myOutput.value]"
//@[line15->line1167]             },
//@[line15->line1168]             "myOutputRgMod2": {
//@[line15->line1169]               "type": "string",
//@[line15->line1170]               "value": "[reference(extensionResourceId(format('/subscriptions/{0}/resourceGroups/{1}', subscription().subscriptionId, 'myRg'), 'Microsoft.Resources/deployments', 'myResourceGroupMod2'), '2020-10-01').outputs.myOutput.value]"
//@[line15->line1171]             }
//@[line15->line1172]           }
//@[line15->line1173]         }
//@[line15->line1174]       }
//@[line15->line1175]     },
  name: 'mySubscriptionModWithCondition'
//@[line16->line0666]       "name": "mySubscriptionModWithCondition",
  scope: subscription('ee44cd78-68c6-43d9-874e-e684ec8d1191')
//@[line17->line0667]       "subscriptionId": "ee44cd78-68c6-43d9-874e-e684ec8d1191",
}

module mySubscriptionModWithDuplicatedNameButDifferentScope 'modules/subscription_empty.bicep' = {
//@[line20->line1176]     {
//@[line20->line1177]       "type": "Microsoft.Resources/deployments",
//@[line20->line1178]       "apiVersion": "2020-10-01",
//@[line20->line1181]       "location": "[deployment().location]",
//@[line20->line1182]       "properties": {
//@[line20->line1183]         "expressionEvaluationOptions": {
//@[line20->line1184]           "scope": "inner"
//@[line20->line1185]         },
//@[line20->line1186]         "mode": "Incremental",
//@[line20->line1187]         "template": {
//@[line20->line1188]           "$schema": "https://schema.management.azure.com/schemas/2018-05-01/subscriptionDeploymentTemplate.json#",
//@[line20->line1189]           "contentVersion": "1.0.0.0",
//@[line20->line1190]           "metadata": {
//@[line20->line1191]             "_generator": {
//@[line20->line1192]               "name": "bicep",
//@[line20->line1193]               "version": "dev",
//@[line20->line1194]               "templateHash": "1395382333336833730"
//@[line20->line1195]             }
//@[line20->line1196]           },
//@[line20->line1197]           "resources": []
//@[line20->line1198]         }
//@[line20->line1199]       }
//@[line20->line1200]     }
  name: 'mySubscriptionMod'
//@[line21->line1179]       "name": "mySubscriptionMod",
  scope: subscription('1ad827ac-2669-4c2f-9970-282b93c3c550')
//@[line22->line1180]       "subscriptionId": "1ad827ac-2669-4c2f-9970-282b93c3c550",
}


output myManagementGroupOutput string = myManagementGroupMod.outputs.myOutput
//@[line26->line1203]     "myManagementGroupOutput": {
//@[line26->line1204]       "type": "string",
//@[line26->line1205]       "value": "[reference(extensionResourceId(tenantResourceId('Microsoft.Management/managementGroups', 'myManagementGroup'), 'Microsoft.Resources/deployments', 'myManagementGroupMod'), '2020-10-01').outputs.myOutput.value]"
//@[line26->line1206]     },
output mySubscriptionOutput string = mySubscriptionMod.outputs.myOutput
//@[line27->line1207]     "mySubscriptionOutput": {
//@[line27->line1208]       "type": "string",
//@[line27->line1209]       "value": "[reference(subscriptionResourceId('ee44cd78-68c6-43d9-874e-e684ec8d1191', 'Microsoft.Resources/deployments', 'mySubscriptionMod'), '2020-10-01').outputs.myOutput.value]"
//@[line27->line1210]     }

