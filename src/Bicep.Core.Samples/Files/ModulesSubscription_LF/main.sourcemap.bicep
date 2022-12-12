targetScope = 'subscription'

param prefix string = 'majastrz'
//@[line02->line011]     "prefix": {
//@[line02->line012]       "type": "string",
//@[line02->line013]       "defaultValue": "majastrz"
//@[line02->line014]     }
var groups = [
//@[line03->line017]     "groups": [
//@[line03->line022]     ],
  'bicep1'
//@[line04->line018]       "bicep1",
  'bicep2'
//@[line05->line019]       "bicep2",
  'bicep3'
//@[line06->line020]       "bicep3",
  'bicep4'
//@[line07->line021]       "bicep4"
]

var scripts = take(groups, 2)
//@[line10->line023]     "scripts": "[take(variables('groups'), 2)]"

resource resourceGroups 'Microsoft.Resources/resourceGroups@2020-06-01' = [for name in groups: {
//@[line12->line026]     {
//@[line12->line027]       "copy": {
//@[line12->line028]         "name": "resourceGroups",
//@[line12->line029]         "count": "[length(variables('groups'))]"
//@[line12->line030]       },
//@[line12->line031]       "type": "Microsoft.Resources/resourceGroups",
//@[line12->line032]       "apiVersion": "2020-06-01",
//@[line12->line033]       "name": "[format('{0}-{1}', parameters('prefix'), variables('groups')[copyIndex()])]",
//@[line12->line035]     },
  name: '${prefix}-${name}'
  location: 'westus'
//@[line14->line034]       "location": "westus"
}]

module scopedToSymbolicName 'hello.bicep' = [for (name, i) in scripts: {
//@[line17->line036]     {
//@[line17->line037]       "copy": {
//@[line17->line038]         "name": "scopedToSymbolicName",
//@[line17->line039]         "count": "[length(variables('scripts'))]"
//@[line17->line040]       },
//@[line17->line041]       "type": "Microsoft.Resources/deployments",
//@[line17->line042]       "apiVersion": "2020-10-01",
//@[line17->line044]       "resourceGroup": "[format('{0}-{1}', parameters('prefix'), variables('groups')[copyIndex()])]",
//@[line17->line045]       "properties": {
//@[line17->line046]         "expressionEvaluationOptions": {
//@[line17->line047]           "scope": "inner"
//@[line17->line048]         },
//@[line17->line049]         "mode": "Incremental",
//@[line17->line050]         "parameters": {
//@[line17->line051]           "scriptName": {
//@[line17->line053]           }
//@[line17->line054]         },
//@[line17->line055]         "template": {
//@[line17->line056]           "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@[line17->line057]           "contentVersion": "1.0.0.0",
//@[line17->line058]           "metadata": {
//@[line17->line059]             "_generator": {
//@[line17->line060]               "name": "bicep",
//@[line17->line061]               "version": "dev",
//@[line17->line062]               "templateHash": "9518912405470532169"
//@[line17->line063]             }
//@[line17->line064]           },
//@[line17->line065]           "parameters": {
//@[line17->line066]             "scriptName": {
//@[line17->line067]               "type": "string"
//@[line17->line068]             }
//@[line17->line069]           },
//@[line17->line070]           "resources": [
//@[line17->line071]             {
//@[line17->line072]               "type": "Microsoft.Resources/deploymentScripts",
//@[line17->line073]               "apiVersion": "2020-10-01",
//@[line17->line074]               "name": "[parameters('scriptName')]",
//@[line17->line075]               "kind": "AzurePowerShell",
//@[line17->line076]               "location": "[resourceGroup().location]",
//@[line17->line077]               "properties": {
//@[line17->line078]                 "azPowerShellVersion": "3.0",
//@[line17->line079]                 "retentionInterval": "PT6H",
//@[line17->line080]                 "scriptContent": "      Write-Output 'Hello World!'\n"
//@[line17->line081]               }
//@[line17->line082]             }
//@[line17->line083]           ],
//@[line17->line084]           "outputs": {
//@[line17->line085]             "myOutput": {
//@[line17->line086]               "type": "string",
//@[line17->line087]               "value": "[parameters('scriptName')]"
//@[line17->line088]             }
//@[line17->line089]           }
//@[line17->line090]         }
//@[line17->line091]       },
//@[line17->line092]       "dependsOn": [
//@[line17->line093]         "[subscriptionResourceId('Microsoft.Resources/resourceGroups', format('{0}-{1}', parameters('prefix'), variables('groups')[copyIndex()]))]"
//@[line17->line094]       ]
//@[line17->line095]     },
  name: '${prefix}-dep-${i}'
//@[line18->line043]       "name": "[format('{0}-dep-{1}', parameters('prefix'), copyIndex())]",
  params: {
    scriptName: 'test-${name}-${i}'
//@[line20->line052]             "value": "[format('test-{0}-{1}', variables('scripts')[copyIndex()], copyIndex())]"
  }
  scope: resourceGroups[i]
}]

module scopedToResourceGroupFunction 'hello.bicep' = [for (name, i) in scripts: {
//@[line25->line096]     {
//@[line25->line097]       "copy": {
//@[line25->line098]         "name": "scopedToResourceGroupFunction",
//@[line25->line099]         "count": "[length(variables('scripts'))]"
//@[line25->line100]       },
//@[line25->line101]       "type": "Microsoft.Resources/deployments",
//@[line25->line102]       "apiVersion": "2020-10-01",
//@[line25->line104]       "resourceGroup": "[concat(variables('scripts')[copyIndex()], '-extra')]",
//@[line25->line105]       "properties": {
//@[line25->line106]         "expressionEvaluationOptions": {
//@[line25->line107]           "scope": "inner"
//@[line25->line108]         },
//@[line25->line109]         "mode": "Incremental",
//@[line25->line110]         "parameters": {
//@[line25->line111]           "scriptName": {
//@[line25->line113]           }
//@[line25->line114]         },
//@[line25->line115]         "template": {
//@[line25->line116]           "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@[line25->line117]           "contentVersion": "1.0.0.0",
//@[line25->line118]           "metadata": {
//@[line25->line119]             "_generator": {
//@[line25->line120]               "name": "bicep",
//@[line25->line121]               "version": "dev",
//@[line25->line122]               "templateHash": "9518912405470532169"
//@[line25->line123]             }
//@[line25->line124]           },
//@[line25->line125]           "parameters": {
//@[line25->line126]             "scriptName": {
//@[line25->line127]               "type": "string"
//@[line25->line128]             }
//@[line25->line129]           },
//@[line25->line130]           "resources": [
//@[line25->line131]             {
//@[line25->line132]               "type": "Microsoft.Resources/deploymentScripts",
//@[line25->line133]               "apiVersion": "2020-10-01",
//@[line25->line134]               "name": "[parameters('scriptName')]",
//@[line25->line135]               "kind": "AzurePowerShell",
//@[line25->line136]               "location": "[resourceGroup().location]",
//@[line25->line137]               "properties": {
//@[line25->line138]                 "azPowerShellVersion": "3.0",
//@[line25->line139]                 "retentionInterval": "PT6H",
//@[line25->line140]                 "scriptContent": "      Write-Output 'Hello World!'\n"
//@[line25->line141]               }
//@[line25->line142]             }
//@[line25->line143]           ],
//@[line25->line144]           "outputs": {
//@[line25->line145]             "myOutput": {
//@[line25->line146]               "type": "string",
//@[line25->line147]               "value": "[parameters('scriptName')]"
//@[line25->line148]             }
//@[line25->line149]           }
//@[line25->line150]         }
//@[line25->line151]       }
//@[line25->line152]     }
  name: '${prefix}-dep-${i}'
//@[line26->line103]       "name": "[format('{0}-dep-{1}', parameters('prefix'), copyIndex())]",
  params: {
    scriptName: 'test-${name}-${i}'
//@[line28->line112]             "value": "[format('test-{0}-{1}', variables('scripts')[copyIndex()], copyIndex())]"
  }
  scope: resourceGroup(concat(name, '-extra'))
}]


