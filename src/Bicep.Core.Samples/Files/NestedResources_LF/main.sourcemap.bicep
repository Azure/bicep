resource basicParent 'My.Rp/parentType@2020-12-01' = {
//@[109:116]       "type": "My.Rp/parentType",
  name: 'basicParent'
  properties: {
//@[113:115]       "properties": {
    size: 'large'
//@[114:114]         "size": "large"
  }

  resource basicChild 'childType' = {
//@[41:52]       "type": "My.Rp/parentType/childType",
    name: 'basicChild'
    properties: {
//@[45:48]       "properties": {
      size: basicParent.properties.large
//@[46:46]         "size": "[reference(resourceId('My.Rp/parentType', 'basicParent')).large]",
      style: 'cool'
//@[47:47]         "style": "cool"
    }

    resource basicGrandchild 'grandchildType' = {
//@[29:40]       "type": "My.Rp/parentType/childType/grandchildType",
      name: 'basicGrandchild'
      properties: {
//@[33:36]       "properties": {
        size: basicParent.properties.size
//@[34:34]         "size": "[reference(resourceId('My.Rp/parentType', 'basicParent')).size]",
        style: basicChild.properties.style
//@[35:35]         "style": "[reference(resourceId('My.Rp/parentType/childType', 'basicParent', 'basicChild')).style]"
      }
    }
  }

  resource basicSibling 'childType' = {
//@[53:65]       "type": "My.Rp/parentType/childType",
    name: 'basicSibling'
    properties: {
//@[57:60]       "properties": {
      size: basicParent.properties.size
//@[58:58]         "size": "[reference(resourceId('My.Rp/parentType', 'basicParent')).size]",
      style: basicChild::basicGrandchild.properties.style
//@[59:59]         "style": "[reference(resourceId('My.Rp/parentType/childType/grandchildType', 'basicParent', 'basicChild', 'basicGrandchild')).style]"
    }
  }
}
// #completionTest(50) -> childResources
output referenceBasicChild string = basicParent::basicChild.properties.size
//@[130:133]     "referenceBasicChild": {
// #completionTest(67) -> grandChildResources
output referenceBasicGrandchild string = basicParent::basicChild::basicGrandchild.properties.style
//@[134:137]     "referenceBasicGrandchild": {

resource existingParent 'My.Rp/parentType@2020-12-01' existing = {
  name: 'existingParent'

  resource existingChild 'childType' existing = {
    name: 'existingChild'

    resource existingGrandchild 'grandchildType' = {
//@[66:74]       "type": "My.Rp/parentType/childType/grandchildType",
      name: 'existingGrandchild'
      properties: {
//@[70:73]       "properties": {
        size: existingParent.properties.size
//@[71:71]         "size": "[reference(resourceId('My.Rp/parentType', 'existingParent'), '2020-12-01').size]",
        style: existingChild.properties.style
//@[72:72]         "style": "[reference(resourceId('My.Rp/parentType/childType', 'existingParent', 'existingChild'), '2020-12-01').style]"
      }
    }
  }
}

param createParent bool
//@[12:14]     "createParent": {
param createChild bool
//@[15:17]     "createChild": {
param createGrandchild bool
//@[18:20]     "createGrandchild": {
resource conditionParent 'My.Rp/parentType@2020-12-01' = if (createParent) {
//@[117:122]       "condition": "[parameters('createParent')]",
  name: 'conditionParent'

  resource conditionChild 'childType' = if (createChild) {
//@[88:96]       "condition": "[and(parameters('createParent'), parameters('createChild'))]",
    name: 'conditionChild'

    resource conditionGrandchild 'grandchildType' = if (createGrandchild) {
//@[75:87]       "condition": "[and(and(parameters('createParent'), parameters('createChild')), parameters('createGrandchild'))]",
      name: 'conditionGrandchild'
      properties: {
//@[80:83]       "properties": {
        size: conditionParent.properties.size
//@[81:81]         "size": "[reference(resourceId('My.Rp/parentType', 'conditionParent'), '2020-12-01').size]",
        style: conditionChild.properties.style
//@[82:82]         "style": "[reference(resourceId('My.Rp/parentType/childType', 'conditionParent', 'conditionChild'), '2020-12-01').style]"
      }
    }
  }
}

var items = [
//@[23:26]     "items": [
  'a'
//@[24:24]       "a",
  'b'
//@[25:25]       "b"
]
resource loopParent 'My.Rp/parentType@2020-12-01' = {
//@[123:127]       "type": "My.Rp/parentType",
  name: 'loopParent'

  resource loopChild 'childType' = [for item in items: {
//@[97:108]       "copy": {
    name: 'loopChild'
  }]
}

output loopChildOutput string = loopParent::loopChild[0].name
//@[138:141]     "loopChildOutput": {
