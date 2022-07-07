resource basicParent 'My.Rp/parentType@2020-12-01' = {
//@[09:20) Resource basicParent. Type: My.Rp/parentType@2020-12-01. Declaration start char: 0, length: 659
  name: 'basicParent'
  properties: {
    size: 'large'
  }

  resource basicChild 'childType' = {
//@[11:21) Resource basicChild. Type: My.Rp/parentType/childType@2020-12-01. Declaration start char: 2, length: 347
    name: 'basicChild'
    properties: {
      size: basicParent.properties.large
      style: 'cool'
    }

    resource basicGrandchild 'grandchildType' = {
//@[13:28) Resource basicGrandchild. Type: My.Rp/parentType/childType/grandchildType@2020-12-01. Declaration start char: 4, length: 194
      name: 'basicGrandchild'
      properties: {
        size: basicParent.properties.size
        style: basicChild.properties.style
      }
    }
  }

  resource basicSibling 'childType' = {
//@[11:23) Resource basicSibling. Type: My.Rp/parentType/childType@2020-12-01. Declaration start char: 2, length: 188
    name: 'basicSibling'
    properties: {
      size: basicParent.properties.size
      style: basicChild::basicGrandchild.properties.style
    }
  }
}
// #completionTest(50) -> childResources
output referenceBasicChild string = basicParent::basicChild.properties.size
//@[07:26) Output referenceBasicChild. Type: string. Declaration start char: 0, length: 75
// #completionTest(67) -> grandChildResources
output referenceBasicGrandchild string = basicParent::basicChild::basicGrandchild.properties.style
//@[07:31) Output referenceBasicGrandchild. Type: string. Declaration start char: 0, length: 98

resource existingParent 'My.Rp/parentType@2020-12-01' existing = {
//@[09:23) Resource existingParent. Type: My.Rp/parentType@2020-12-01. Declaration start char: 0, length: 386
  name: 'existingParent'

  resource existingChild 'childType' existing = {
//@[11:24) Resource existingChild. Type: My.Rp/parentType/childType@2020-12-01. Declaration start char: 2, length: 289
    name: 'existingChild'

    resource existingGrandchild 'grandchildType' = {
//@[13:31) Resource existingGrandchild. Type: My.Rp/parentType/childType/grandchildType@2020-12-01. Declaration start char: 4, length: 206
      name: 'existingGrandchild'
      properties: {
        size: existingParent.properties.size
        style: existingChild.properties.style
      }
    }
  }
}

param createParent bool
//@[06:18) Parameter createParent. Type: bool. Declaration start char: 0, length: 23
param createChild bool
//@[06:17) Parameter createChild. Type: bool. Declaration start char: 0, length: 22
param createGrandchild bool
//@[06:22) Parameter createGrandchild. Type: bool. Declaration start char: 0, length: 27
resource conditionParent 'My.Rp/parentType@2020-12-01' = if (createParent) {
//@[09:24) Resource conditionParent. Type: My.Rp/parentType@2020-12-01. Declaration start char: 0, length: 433
  name: 'conditionParent'

  resource conditionChild 'childType' = if (createChild) {
//@[11:25) Resource conditionChild. Type: My.Rp/parentType/childType@2020-12-01. Declaration start char: 2, length: 325
    name: 'conditionChild'

    resource conditionGrandchild 'grandchildType' = if (createGrandchild) {
//@[13:32) Resource conditionGrandchild. Type: My.Rp/parentType/childType/grandchildType@2020-12-01. Declaration start char: 4, length: 232
      name: 'conditionGrandchild'
      properties: {
        size: conditionParent.properties.size
        style: conditionChild.properties.style
      }
    }
  }
}

var items = [
//@[04:09) Variable items. Type: ('a' | 'b')[]. Declaration start char: 0, length: 27
  'a'
  'b'
]
resource loopParent 'My.Rp/parentType@2020-12-01' = {
//@[09:19) Resource loopParent. Type: My.Rp/parentType@2020-12-01. Declaration start char: 0, length: 161
  name: 'loopParent'

  resource loopChild 'childType' = [for item in items: {
//@[40:44) Local item. Type: 'a' | 'b'. Declaration start char: 40, length: 4
//@[11:20) Resource loopChild. Type: My.Rp/parentType/childType@2020-12-01[]. Declaration start char: 2, length: 81
    name: 'loopChild'
  }]
}

output loopChildOutput string = loopParent::loopChild[0].name
//@[07:22) Output loopChildOutput. Type: string. Declaration start char: 0, length: 61
