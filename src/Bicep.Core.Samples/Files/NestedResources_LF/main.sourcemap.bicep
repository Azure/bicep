resource basicParent 'My.Rp/parentType@2020-12-01' = {
//@[108:115]       "type": "My.Rp/parentType",\r
  name: 'basicParent'
  properties: {
//@[112:114]       "properties": {\r
    size: 'large'
//@[113:113]         "size": "large"\r
  }

  resource basicChild 'childType' = {
//@[40:51]       "type": "My.Rp/parentType/childType",\r
    name: 'basicChild'
    properties: {
//@[44:47]       "properties": {\r
      size: basicParent.properties.large
//@[45:45]         "size": "[reference(resourceId('My.Rp/parentType', 'basicParent')).large]",\r
      style: 'cool'
//@[46:46]         "style": "cool"\r
    }

    resource basicGrandchild 'grandchildType' = {
//@[28:39]       "type": "My.Rp/parentType/childType/grandchildType",\r
      name: 'basicGrandchild'
      properties: {
//@[32:35]       "properties": {\r
        size: basicParent.properties.size
//@[33:33]         "size": "[reference(resourceId('My.Rp/parentType', 'basicParent')).size]",\r
        style: basicChild.properties.style
//@[34:34]         "style": "[reference(resourceId('My.Rp/parentType/childType', 'basicParent', 'basicChild')).style]"\r
      }
    }
  }

  resource basicSibling 'childType' = {
//@[52:64]       "type": "My.Rp/parentType/childType",\r
    name: 'basicSibling'
    properties: {
//@[56:59]       "properties": {\r
      size: basicParent.properties.size
//@[57:57]         "size": "[reference(resourceId('My.Rp/parentType', 'basicParent')).size]",\r
      style: basicChild::basicGrandchild.properties.style
//@[58:58]         "style": "[reference(resourceId('My.Rp/parentType/childType/grandchildType', 'basicParent', 'basicChild', 'basicGrandchild')).style]"\r
    }
  }
}
// #completionTest(50) -> childResources
output referenceBasicChild string = basicParent::basicChild.properties.size
//@[129:132]     "referenceBasicChild": {\r
// #completionTest(67) -> grandChildResources
output referenceBasicGrandchild string = basicParent::basicChild::basicGrandchild.properties.style
//@[133:136]     "referenceBasicGrandchild": {\r

resource existingParent 'My.Rp/parentType@2020-12-01' existing = {
  name: 'existingParent'

  resource existingChild 'childType' existing = {
    name: 'existingChild'

    resource existingGrandchild 'grandchildType' = {
//@[65:73]       "type": "My.Rp/parentType/childType/grandchildType",\r
      name: 'existingGrandchild'
      properties: {
//@[69:72]       "properties": {\r
        size: existingParent.properties.size
//@[70:70]         "size": "[reference(resourceId('My.Rp/parentType', 'existingParent'), '2020-12-01').size]",\r
        style: existingChild.properties.style
//@[71:71]         "style": "[reference(resourceId('My.Rp/parentType/childType', 'existingParent', 'existingChild'), '2020-12-01').style]"\r
      }
    }
  }
}

param createParent bool
//@[11:13]     "createParent": {\r
param createChild bool
//@[14:16]     "createChild": {\r
param createGrandchild bool
//@[17:19]     "createGrandchild": {\r
resource conditionParent 'My.Rp/parentType@2020-12-01' = if (createParent) {
//@[116:121]       "condition": "[parameters('createParent')]",\r
  name: 'conditionParent'

  resource conditionChild 'childType' = if (createChild) {
//@[87:95]       "condition": "[and(parameters('createParent'), parameters('createChild'))]",\r
    name: 'conditionChild'

    resource conditionGrandchild 'grandchildType' = if (createGrandchild) {
//@[74:86]       "condition": "[and(and(parameters('createParent'), parameters('createChild')), parameters('createGrandchild'))]",\r
      name: 'conditionGrandchild'
      properties: {
//@[79:82]       "properties": {\r
        size: conditionParent.properties.size
//@[80:80]         "size": "[reference(resourceId('My.Rp/parentType', 'conditionParent'), '2020-12-01').size]",\r
        style: conditionChild.properties.style
//@[81:81]         "style": "[reference(resourceId('My.Rp/parentType/childType', 'conditionParent', 'conditionChild'), '2020-12-01').style]"\r
      }
    }
  }
}

var items = [
//@[22:25]     "items": [\r
  'a'
//@[23:23]       "a",\r
  'b'
//@[24:24]       "b"\r
]
resource loopParent 'My.Rp/parentType@2020-12-01' = {
//@[122:126]       "type": "My.Rp/parentType",\r
  name: 'loopParent'

  resource loopChild 'childType' = [for item in items: {
//@[96:107]       "copy": {\r
    name: 'loopChild'
  }]
}

output loopChildOutput string = loopParent::loopChild[0].name
//@[137:140]     "loopChildOutput": {\r
