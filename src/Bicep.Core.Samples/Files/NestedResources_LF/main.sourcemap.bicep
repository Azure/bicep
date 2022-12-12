resource basicParent 'My.Rp/parentType@2020-12-01' = {
//@[line00->line108]     {
//@[line00->line109]       "type": "My.Rp/parentType",
//@[line00->line110]       "apiVersion": "2020-12-01",
//@[line00->line111]       "name": "basicParent",
//@[line00->line115]     },
  name: 'basicParent'
  properties: {
//@[line02->line112]       "properties": {
//@[line02->line114]       }
    size: 'large'
//@[line03->line113]         "size": "large"
  }

  resource basicChild 'childType' = {
//@[line06->line040]     {
//@[line06->line041]       "type": "My.Rp/parentType/childType",
//@[line06->line042]       "apiVersion": "2020-12-01",
//@[line06->line043]       "name": "[format('{0}/{1}', 'basicParent', 'basicChild')]",
//@[line06->line048]       "dependsOn": [
//@[line06->line049]         "[resourceId('My.Rp/parentType', 'basicParent')]"
//@[line06->line050]       ]
//@[line06->line051]     },
    name: 'basicChild'
    properties: {
//@[line08->line044]       "properties": {
//@[line08->line047]       },
      size: basicParent.properties.large
//@[line09->line045]         "size": "[reference(resourceId('My.Rp/parentType', 'basicParent'), '2020-12-01').large]",
      style: 'cool'
//@[line10->line046]         "style": "cool"
    }

    resource basicGrandchild 'grandchildType' = {
//@[line13->line028]     {
//@[line13->line029]       "type": "My.Rp/parentType/childType/grandchildType",
//@[line13->line030]       "apiVersion": "2020-12-01",
//@[line13->line031]       "name": "[format('{0}/{1}/{2}', 'basicParent', 'basicChild', 'basicGrandchild')]",
//@[line13->line036]       "dependsOn": [
//@[line13->line037]         "[resourceId('My.Rp/parentType/childType', 'basicParent', 'basicChild')]"
//@[line13->line038]       ]
//@[line13->line039]     },
      name: 'basicGrandchild'
      properties: {
//@[line15->line032]       "properties": {
//@[line15->line035]       },
        size: basicParent.properties.size
//@[line16->line033]         "size": "[reference(resourceId('My.Rp/parentType', 'basicParent'), '2020-12-01').size]",
        style: basicChild.properties.style
//@[line17->line034]         "style": "[reference(resourceId('My.Rp/parentType/childType', 'basicParent', 'basicChild'), '2020-12-01').style]"
      }
    }
  }

  resource basicSibling 'childType' = {
//@[line22->line052]     {
//@[line22->line053]       "type": "My.Rp/parentType/childType",
//@[line22->line054]       "apiVersion": "2020-12-01",
//@[line22->line055]       "name": "[format('{0}/{1}', 'basicParent', 'basicSibling')]",
//@[line22->line060]       "dependsOn": [
//@[line22->line061]         "[resourceId('My.Rp/parentType/childType/grandchildType', 'basicParent', 'basicChild', 'basicGrandchild')]",
//@[line22->line062]         "[resourceId('My.Rp/parentType', 'basicParent')]"
//@[line22->line063]       ]
//@[line22->line064]     },
    name: 'basicSibling'
    properties: {
//@[line24->line056]       "properties": {
//@[line24->line059]       },
      size: basicParent.properties.size
//@[line25->line057]         "size": "[reference(resourceId('My.Rp/parentType', 'basicParent'), '2020-12-01').size]",
      style: basicChild::basicGrandchild.properties.style
//@[line26->line058]         "style": "[reference(resourceId('My.Rp/parentType/childType/grandchildType', 'basicParent', 'basicChild', 'basicGrandchild'), '2020-12-01').style]"
    }
  }
}
// #completionTest(50) -> childResources
output referenceBasicChild string = basicParent::basicChild.properties.size
//@[line31->line129]     "referenceBasicChild": {
//@[line31->line130]       "type": "string",
//@[line31->line131]       "value": "[reference(resourceId('My.Rp/parentType/childType', 'basicParent', 'basicChild'), '2020-12-01').size]"
//@[line31->line132]     },
// #completionTest(67) -> grandChildResources
output referenceBasicGrandchild string = basicParent::basicChild::basicGrandchild.properties.style
//@[line33->line133]     "referenceBasicGrandchild": {
//@[line33->line134]       "type": "string",
//@[line33->line135]       "value": "[reference(resourceId('My.Rp/parentType/childType/grandchildType', 'basicParent', 'basicChild', 'basicGrandchild'), '2020-12-01').style]"
//@[line33->line136]     },

resource existingParent 'My.Rp/parentType@2020-12-01' existing = {
  name: 'existingParent'

  resource existingChild 'childType' existing = {
    name: 'existingChild'

    resource existingGrandchild 'grandchildType' = {
//@[line41->line065]     {
//@[line41->line066]       "type": "My.Rp/parentType/childType/grandchildType",
//@[line41->line067]       "apiVersion": "2020-12-01",
//@[line41->line068]       "name": "[format('{0}/{1}/{2}', 'existingParent', 'existingChild', 'existingGrandchild')]",
//@[line41->line073]     },
      name: 'existingGrandchild'
      properties: {
//@[line43->line069]       "properties": {
//@[line43->line072]       }
        size: existingParent.properties.size
//@[line44->line070]         "size": "[reference(resourceId('My.Rp/parentType', 'existingParent'), '2020-12-01').size]",
        style: existingChild.properties.style
//@[line45->line071]         "style": "[reference(resourceId('My.Rp/parentType/childType', 'existingParent', 'existingChild'), '2020-12-01').style]"
      }
    }
  }
}

param createParent bool
//@[line51->line011]     "createParent": {
//@[line51->line012]       "type": "bool"
//@[line51->line013]     },
param createChild bool
//@[line52->line014]     "createChild": {
//@[line52->line015]       "type": "bool"
//@[line52->line016]     },
param createGrandchild bool
//@[line53->line017]     "createGrandchild": {
//@[line53->line018]       "type": "bool"
//@[line53->line019]     }
resource conditionParent 'My.Rp/parentType@2020-12-01' = if (createParent) {
//@[line54->line075]       "condition": "[and(and(parameters('createParent'), parameters('createChild')), parameters('createGrandchild'))]",
//@[line54->line088]       "condition": "[and(parameters('createParent'), parameters('createChild'))]",
//@[line54->line116]     {
//@[line54->line117]       "condition": "[parameters('createParent')]",
//@[line54->line118]       "type": "My.Rp/parentType",
//@[line54->line119]       "apiVersion": "2020-12-01",
//@[line54->line120]       "name": "conditionParent"
//@[line54->line121]     },
  name: 'conditionParent'

  resource conditionChild 'childType' = if (createChild) {
//@[line57->line087]     {
//@[line57->line089]       "type": "My.Rp/parentType/childType",
//@[line57->line090]       "apiVersion": "2020-12-01",
//@[line57->line091]       "name": "[format('{0}/{1}', 'conditionParent', 'conditionChild')]",
//@[line57->line092]       "dependsOn": [
//@[line57->line093]         "[resourceId('My.Rp/parentType', 'conditionParent')]"
//@[line57->line094]       ]
//@[line57->line095]     },
    name: 'conditionChild'

    resource conditionGrandchild 'grandchildType' = if (createGrandchild) {
//@[line60->line074]     {
//@[line60->line076]       "type": "My.Rp/parentType/childType/grandchildType",
//@[line60->line077]       "apiVersion": "2020-12-01",
//@[line60->line078]       "name": "[format('{0}/{1}/{2}', 'conditionParent', 'conditionChild', 'conditionGrandchild')]",
//@[line60->line083]       "dependsOn": [
//@[line60->line084]         "[resourceId('My.Rp/parentType/childType', 'conditionParent', 'conditionChild')]"
//@[line60->line085]       ]
//@[line60->line086]     },
      name: 'conditionGrandchild'
      properties: {
//@[line62->line079]       "properties": {
//@[line62->line082]       },
        size: conditionParent.properties.size
//@[line63->line080]         "size": "[reference(resourceId('My.Rp/parentType', 'conditionParent'), '2020-12-01').size]",
        style: conditionChild.properties.style
//@[line64->line081]         "style": "[reference(resourceId('My.Rp/parentType/childType', 'conditionParent', 'conditionChild'), '2020-12-01').style]"
      }
    }
  }
}

var items = [
//@[line70->line022]     "items": [
//@[line70->line025]     ]
  'a'
//@[line71->line023]       "a",
  'b'
//@[line72->line024]       "b"
]
resource loopParent 'My.Rp/parentType@2020-12-01' = {
//@[line74->line122]     {
//@[line74->line123]       "type": "My.Rp/parentType",
//@[line74->line124]       "apiVersion": "2020-12-01",
//@[line74->line125]       "name": "loopParent"
//@[line74->line126]     }
  name: 'loopParent'

  resource loopChild 'childType' = [for item in items: {
//@[line77->line096]     {
//@[line77->line097]       "copy": {
//@[line77->line098]         "name": "loopChild",
//@[line77->line099]         "count": "[length(variables('items'))]"
//@[line77->line100]       },
//@[line77->line101]       "type": "My.Rp/parentType/childType",
//@[line77->line102]       "apiVersion": "2020-12-01",
//@[line77->line103]       "name": "[format('{0}/{1}', 'loopParent', 'loopChild')]",
//@[line77->line104]       "dependsOn": [
//@[line77->line105]         "[resourceId('My.Rp/parentType', 'loopParent')]"
//@[line77->line106]       ]
//@[line77->line107]     },
    name: 'loopChild'
  }]
}

output loopChildOutput string = loopParent::loopChild[0].name
//@[line82->line137]     "loopChildOutput": {
//@[line82->line138]       "type": "string",
//@[line82->line139]       "value": "loopChild"
//@[line82->line140]     }
