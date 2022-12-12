param deployTimeParam string = 'steve'
//@[line00->line011]     "deployTimeParam": {
//@[line00->line012]       "type": "string",
//@[line00->line013]       "defaultValue": "steve"
//@[line00->line014]     }
var deployTimeVar = 'nigel'
//@[line01->line017]     "deployTimeVar": "nigel",
var dependentVar = {
//@[line02->line018]     "dependentVar": {
//@[line02->line023]     },
  dependencies: [
//@[line03->line019]       "dependencies": [
//@[line03->line022]       ]
    deployTimeVar
//@[line04->line020]         "[variables('deployTimeVar')]",
    deployTimeParam
//@[line05->line021]         "[parameters('deployTimeParam')]"
  ]
}

var resourceDependency = {
  dependenciesA: [
//@[line10->line045]           "dependenciesA": [
//@[line10->line051]           ]
    resA.id
//@[line11->line046]             "[resourceId('My.Rp/myResourceType', 'resA')]",
    resA.name
//@[line12->line047]             "resA",
    resA.type
//@[line13->line048]             "My.Rp/myResourceType",
    resA.properties.deployTime
//@[line14->line049]             "[reference(resourceId('My.Rp/myResourceType', 'resA'), '2020-01-01').deployTime]",
    resA.properties.eTag
//@[line15->line050]             "[reference(resourceId('My.Rp/myResourceType', 'resA'), '2020-01-01').eTag]"
  ]
}

output resourceAType string = resA.type
//@[line19->line092]     "resourceAType": {
//@[line19->line093]       "type": "string",
//@[line19->line094]       "value": "My.Rp/myResourceType"
//@[line19->line095]     },
resource resA 'My.Rp/myResourceType@2020-01-01' = {
//@[line20->line030]     {
//@[line20->line031]       "type": "My.Rp/myResourceType",
//@[line20->line032]       "apiVersion": "2020-01-01",
//@[line20->line033]       "name": "resA",
//@[line20->line038]     },
  name: 'resA'
  properties: {
//@[line22->line034]       "properties": {
//@[line22->line037]       }
    deployTime: dependentVar
//@[line23->line035]         "deployTime": "[variables('dependentVar')]",
    eTag: '1234'
//@[line24->line036]         "eTag": "1234"
  }
}

output resourceBId string = resB.id
//@[line28->line096]     "resourceBId": {
//@[line28->line097]       "type": "string",
//@[line28->line098]       "value": "[resourceId('My.Rp/myResourceType', 'resB')]"
//@[line28->line099]     },
resource resB 'My.Rp/myResourceType@2020-01-01' = {
//@[line29->line039]     {
//@[line29->line040]       "type": "My.Rp/myResourceType",
//@[line29->line041]       "apiVersion": "2020-01-01",
//@[line29->line042]       "name": "resB",
//@[line29->line054]       "dependsOn": [
//@[line29->line055]         "[resourceId('My.Rp/myResourceType', 'resA')]"
//@[line29->line056]       ]
//@[line29->line057]     },
  name: 'resB'
  properties: {
//@[line31->line043]       "properties": {
//@[line31->line053]       },
    dependencies: resourceDependency
//@[line32->line044]         "dependencies": {
//@[line32->line052]         }
  }
}

var resourceIds = {
//@[line36->line024]     "resourceIds": {
//@[line36->line027]     }
  a: resA.id
//@[line37->line025]       "a": "[resourceId('My.Rp/myResourceType', 'resA')]",
  b: resB.id
//@[line38->line026]       "b": "[resourceId('My.Rp/myResourceType', 'resB')]"
}

resource resC 'My.Rp/myResourceType@2020-01-01' = {
//@[line41->line058]     {
//@[line41->line059]       "type": "My.Rp/myResourceType",
//@[line41->line060]       "apiVersion": "2020-01-01",
//@[line41->line061]       "name": "resC",
//@[line41->line065]       "dependsOn": [
//@[line41->line066]         "[resourceId('My.Rp/myResourceType', 'resA')]",
//@[line41->line067]         "[resourceId('My.Rp/myResourceType', 'resB')]"
//@[line41->line068]       ]
//@[line41->line069]     },
  name: 'resC'
  properties: {
//@[line43->line062]       "properties": {
//@[line43->line064]       },
    resourceIds: resourceIds
//@[line44->line063]         "resourceIds": "[variables('resourceIds')]"
  }
}

resource resD 'My.Rp/myResourceType/childType@2020-01-01' = {
//@[line48->line070]     {
//@[line48->line071]       "type": "My.Rp/myResourceType/childType",
//@[line48->line072]       "apiVersion": "2020-01-01",
//@[line48->line073]       "name": "[format('{0}/resD', 'resC')]",
//@[line48->line075]       "dependsOn": [
//@[line48->line076]         "[resourceId('My.Rp/myResourceType', 'resC')]"
//@[line48->line077]       ]
//@[line48->line078]     },
  name: '${resC.name}/resD'
  properties: {
//@[line50->line074]       "properties": {},
  }
}

resource resE 'My.Rp/myResourceType/childType@2020-01-01' = {
//@[line54->line079]     {
//@[line54->line080]       "type": "My.Rp/myResourceType/childType",
//@[line54->line081]       "apiVersion": "2020-01-01",
//@[line54->line082]       "name": "resC/resD",
//@[line54->line086]       "dependsOn": [
//@[line54->line087]         "[resourceId('My.Rp/myResourceType/childType', split(format('{0}/resD', 'resC'), '/')[0], split(format('{0}/resD', 'resC'), '/')[1])]"
//@[line54->line088]       ]
//@[line54->line089]     }
  name: 'resC/resD'
  properties: {
//@[line56->line083]       "properties": {
//@[line56->line085]       },
    resDRef: resD.id
//@[line57->line084]         "resDRef": "[resourceId('My.Rp/myResourceType/childType', split(format('{0}/resD', 'resC'), '/')[0], split(format('{0}/resD', 'resC'), '/')[1])]"
  }
}

output resourceCProperties object = resC.properties
//@[line61->line100]     "resourceCProperties": {
//@[line61->line101]       "type": "object",
//@[line61->line102]       "value": "[reference(resourceId('My.Rp/myResourceType', 'resC'), '2020-01-01')]"
//@[line61->line103]     }

