resource basicParent 'My.Rp/parentType@2020-12-01' = {
//@[21:50) [BCP081 (Warning)] Resource type "My.Rp/parentType@2020-12-01" does not have types available. (CodeDescription: none) |'My.Rp/parentType@2020-12-01'|
  name: 'basicParent'
  properties: {
    size: 'large'
  }

  resource basicChild 'childType' = {
//@[22:33) [BCP081 (Warning)] Resource type "My.Rp/parentType/childType@2020-12-01" does not have types available. (CodeDescription: none) |'childType'|
    name: 'basicChild'
    properties: {
      size: basicParent.properties.large
      style: 'cool'
    }

    resource basicGrandchild 'grandchildType' = {
//@[29:45) [BCP081 (Warning)] Resource type "My.Rp/parentType/childType/grandchildType@2020-12-01" does not have types available. (CodeDescription: none) |'grandchildType'|
      name: 'basicGrandchild'
      properties: {
        size: basicParent.properties.size
        style: basicChild.properties.style
      }
    }
  }

  resource basicSibling 'childType' = {
//@[24:35) [BCP081 (Warning)] Resource type "My.Rp/parentType/childType@2020-12-01" does not have types available. (CodeDescription: none) |'childType'|
    name: 'basicSibling'
    properties: {
      size: basicParent.properties.size
      style: basicChild::basicGrandchild.properties.style
    }
  }
}
// #completionTest(50) -> childResources
output referenceBasicChild string = basicParent::basicChild.properties.size
// #completionTest(67) -> grandChildResources
output referenceBasicGrandchild string = basicParent::basicChild::basicGrandchild.properties.style

resource existingParent 'My.Rp/parentType@2020-12-01' existing = {
//@[24:53) [BCP081 (Warning)] Resource type "My.Rp/parentType@2020-12-01" does not have types available. (CodeDescription: none) |'My.Rp/parentType@2020-12-01'|
  name: 'existingParent'

  resource existingChild 'childType' existing = {
//@[25:36) [BCP081 (Warning)] Resource type "My.Rp/parentType/childType@2020-12-01" does not have types available. (CodeDescription: none) |'childType'|
    name: 'existingChild'

    resource existingGrandchild 'grandchildType' = {
//@[32:48) [BCP081 (Warning)] Resource type "My.Rp/parentType/childType/grandchildType@2020-12-01" does not have types available. (CodeDescription: none) |'grandchildType'|
      name: 'existingGrandchild'
      properties: {
        size: existingParent.properties.size
        style: existingChild.properties.style
      }
    }
  }
}

param createParent bool
param createChild bool
param createGrandchild bool
resource conditionParent 'My.Rp/parentType@2020-12-01' = if (createParent) {
//@[25:54) [BCP081 (Warning)] Resource type "My.Rp/parentType@2020-12-01" does not have types available. (CodeDescription: none) |'My.Rp/parentType@2020-12-01'|
  name: 'conditionParent'

  resource conditionChild 'childType' = if (createChild) {
//@[26:37) [BCP081 (Warning)] Resource type "My.Rp/parentType/childType@2020-12-01" does not have types available. (CodeDescription: none) |'childType'|
    name: 'conditionChild'

    resource conditionGrandchild 'grandchildType' = if (createGrandchild) {
//@[33:49) [BCP081 (Warning)] Resource type "My.Rp/parentType/childType/grandchildType@2020-12-01" does not have types available. (CodeDescription: none) |'grandchildType'|
      name: 'conditionGrandchild'
      properties: {
        size: conditionParent.properties.size
        style: conditionChild.properties.style
      }
    }
  }
}

var items = [
  'a'
  'b'
]
resource loopParent 'My.Rp/parentType@2020-12-01' = {
//@[20:49) [BCP081 (Warning)] Resource type "My.Rp/parentType@2020-12-01" does not have types available. (CodeDescription: none) |'My.Rp/parentType@2020-12-01'|
  name: 'loopParent'

  resource loopChild 'childType' = [for item in items: {
//@[11:20) [BCP179 (Warning)] The loop item variable "item" must be referenced in at least one of the value expressions of the following properties: "name", "scope" (CodeDescription: none) |loopChild|
//@[21:32) [BCP081 (Warning)] Resource type "My.Rp/parentType/childType@2020-12-01" does not have types available. (CodeDescription: none) |'childType'|
    name: 'loopChild'
  }]
}

output loopChildOutput string = loopParent::loopChild[0].name
