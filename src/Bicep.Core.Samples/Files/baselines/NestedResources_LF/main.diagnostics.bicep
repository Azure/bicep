resource basicParent 'My.Rp/parentType@2020-12-01' = {
//@[21:50) [BCP081 (Warning)] Resource type "My.Rp/parentType@2020-12-01" does not have types available. Bicep is unable to validate resource properties prior to deployment, but this will not block the resource from being deployed. (bicep https://aka.ms/bicep/core-diagnostics#BCP081) |'My.Rp/parentType@2020-12-01'|
  name: 'basicParent'
  properties: {
    size: 'large'
  }

  resource basicChild 'childType' = {
//@[22:33) [BCP081 (Warning)] Resource type "My.Rp/parentType/childType@2020-12-01" does not have types available. Bicep is unable to validate resource properties prior to deployment, but this will not block the resource from being deployed. (bicep https://aka.ms/bicep/core-diagnostics#BCP081) |'childType'|
    name: 'basicChild'
    properties: {
      size: basicParent.properties.large
      style: 'cool'
    }

    resource basicGrandchild 'grandchildType' = {
//@[29:45) [BCP081 (Warning)] Resource type "My.Rp/parentType/childType/grandchildType@2020-12-01" does not have types available. Bicep is unable to validate resource properties prior to deployment, but this will not block the resource from being deployed. (bicep https://aka.ms/bicep/core-diagnostics#BCP081) |'grandchildType'|
      name: 'basicGrandchild'
      properties: {
        size: basicParent.properties.size
        style: basicChild.properties.style
      }
    }
  }

  resource basicSibling 'childType' = {
//@[24:35) [BCP081 (Warning)] Resource type "My.Rp/parentType/childType@2020-12-01" does not have types available. Bicep is unable to validate resource properties prior to deployment, but this will not block the resource from being deployed. (bicep https://aka.ms/bicep/core-diagnostics#BCP081) |'childType'|
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
//@[24:53) [BCP081 (Warning)] Resource type "My.Rp/parentType@2020-12-01" does not have types available. Bicep is unable to validate resource properties prior to deployment, but this will not block the resource from being deployed. (bicep https://aka.ms/bicep/core-diagnostics#BCP081) |'My.Rp/parentType@2020-12-01'|
  name: 'existingParent'

  resource existingChild 'childType' existing = {
//@[25:36) [BCP081 (Warning)] Resource type "My.Rp/parentType/childType@2020-12-01" does not have types available. Bicep is unable to validate resource properties prior to deployment, but this will not block the resource from being deployed. (bicep https://aka.ms/bicep/core-diagnostics#BCP081) |'childType'|
    name: 'existingChild'

    resource existingGrandchild 'grandchildType' = {
//@[32:48) [BCP081 (Warning)] Resource type "My.Rp/parentType/childType/grandchildType@2020-12-01" does not have types available. Bicep is unable to validate resource properties prior to deployment, but this will not block the resource from being deployed. (bicep https://aka.ms/bicep/core-diagnostics#BCP081) |'grandchildType'|
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
//@[25:54) [BCP081 (Warning)] Resource type "My.Rp/parentType@2020-12-01" does not have types available. Bicep is unable to validate resource properties prior to deployment, but this will not block the resource from being deployed. (bicep https://aka.ms/bicep/core-diagnostics#BCP081) |'My.Rp/parentType@2020-12-01'|
  name: 'conditionParent'

  resource conditionChild 'childType' = if (createChild) {
//@[26:37) [BCP081 (Warning)] Resource type "My.Rp/parentType/childType@2020-12-01" does not have types available. Bicep is unable to validate resource properties prior to deployment, but this will not block the resource from being deployed. (bicep https://aka.ms/bicep/core-diagnostics#BCP081) |'childType'|
    name: 'conditionChild'

    resource conditionGrandchild 'grandchildType' = if (createGrandchild) {
//@[33:49) [BCP081 (Warning)] Resource type "My.Rp/parentType/childType/grandchildType@2020-12-01" does not have types available. Bicep is unable to validate resource properties prior to deployment, but this will not block the resource from being deployed. (bicep https://aka.ms/bicep/core-diagnostics#BCP081) |'grandchildType'|
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
//@[20:49) [BCP081 (Warning)] Resource type "My.Rp/parentType@2020-12-01" does not have types available. Bicep is unable to validate resource properties prior to deployment, but this will not block the resource from being deployed. (bicep https://aka.ms/bicep/core-diagnostics#BCP081) |'My.Rp/parentType@2020-12-01'|
  name: 'loopParent'

  resource loopChild 'childType' = [for item in items: {
//@[11:20) [BCP179 (Warning)] Unique resource or deployment name is required when looping. The loop item variable "item" must be referenced in at least one of the value expressions of the following properties: "name", "scope" (bicep https://aka.ms/bicep/core-diagnostics#BCP179) |loopChild|
//@[21:32) [BCP081 (Warning)] Resource type "My.Rp/parentType/childType@2020-12-01" does not have types available. Bicep is unable to validate resource properties prior to deployment, but this will not block the resource from being deployed. (bicep https://aka.ms/bicep/core-diagnostics#BCP081) |'childType'|
    name: 'loopChild'
  }]
}

output loopChildOutput string = loopParent::loopChild[0].name
