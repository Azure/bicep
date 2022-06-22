resource basicParent 'My.Rp/parentType@2020-12-01' = {
//@[108:115]       "type": "My.Rp/parentType",
  name: 'basicParent'
  properties: {
    size: 'large'
  }

  resource basicChild 'childType' = {
//@[40:51]       "type": "My.Rp/parentType/childType",
    name: 'basicChild'
    properties: {
      size: basicParent.properties.large
      style: 'cool'
    }

    resource basicGrandchild 'grandchildType' = {
//@[28:39]       "type": "My.Rp/parentType/childType/grandchildType",
      name: 'basicGrandchild'
      properties: {
        size: basicParent.properties.size
        style: basicChild.properties.style
      }
    }
  }

  resource basicSibling 'childType' = {
//@[52:64]       "type": "My.Rp/parentType/childType",
    name: 'basicSibling'
    properties: {
      size: basicParent.properties.size
      style: basicChild::basicGrandchild.properties.style
    }
  }
}
// #completionTest(50) -> childResources
output referenceBasicChild string = basicParent::basicChild.properties.size
//@[129:132]     "referenceBasicChild": {
// #completionTest(67) -> grandChildResources
output referenceBasicGrandchild string = basicParent::basicChild::basicGrandchild.properties.style
//@[133:136]     "referenceBasicGrandchild": {

resource existingParent 'My.Rp/parentType@2020-12-01' existing = {
  name: 'existingParent'

  resource existingChild 'childType' existing = {
    name: 'existingChild'

    resource existingGrandchild 'grandchildType' = {
//@[65:73]       "type": "My.Rp/parentType/childType/grandchildType",
      name: 'existingGrandchild'
      properties: {
        size: existingParent.properties.size
        style: existingChild.properties.style
      }
    }
  }
}

param createParent bool
//@[11:13]     "createParent": {
param createChild bool
//@[14:16]     "createChild": {
param createGrandchild bool
//@[17:19]     "createGrandchild": {
resource conditionParent 'My.Rp/parentType@2020-12-01' = if (createParent) {
//@[116:121]       "condition": "[parameters('createParent')]",
  name: 'conditionParent'

  resource conditionChild 'childType' = if (createChild) {
//@[87:95]       "condition": "[and(parameters('createParent'), parameters('createChild'))]",
    name: 'conditionChild'

    resource conditionGrandchild 'grandchildType' = if (createGrandchild) {
//@[74:86]       "condition": "[and(and(parameters('createParent'), parameters('createChild')), parameters('createGrandchild'))]",
      name: 'conditionGrandchild'
      properties: {
        size: conditionParent.properties.size
        style: conditionChild.properties.style
      }
    }
  }
}

var items = [
//@[22:25]     "items": [
  'a'
//@[23:23]       "a",
  'b'
//@[24:24]       "b"
]
resource loopParent 'My.Rp/parentType@2020-12-01' = {
//@[122:126]       "type": "My.Rp/parentType",
  name: 'loopParent'

  resource loopChild 'childType' = [for item in items: {
//@[96:107]       "copy": {
    name: 'loopChild'
  }]
}

output loopChildOutput string = loopParent::loopChild[0].name
//@[137:140]     "loopChildOutput": {
