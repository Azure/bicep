resource basicParent 'My.Rp/parentType@2020-12-01' = {
  name: 'basicParent'
  properties: {
    size: 'large'
  }

  resource basicChild 'childType' = {
    name: 'basicChild'
    properties: {
      size: basicParent.properties.large
      style: 'cool'
    }

    resource basicGrandchild 'grandchildType' = {
      name: 'basicGrandchild'
      properties: {
        size: basicParent.properties.size
        style: basicChild.properties.style
      }
    }
  }

  resource basicSibling 'childType' = {
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
  name: 'existingParent'

  resource existingChild 'childType' existing = {
    name: 'existingChild'

    resource existingGrandchild 'grandchildType' = {
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
  name: 'conditionParent'

  resource conditionChild 'childType' = if (createChild) {
    name: 'conditionChild'

    resource conditionGrandchild 'grandchildType' = if (createGrandchild) {
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
  name: 'loopParent'

  resource loopChild 'childType' = [for item in items: {
    name: 'loopChild'
  }]
}

output loopChildOutput string = loopParent::loopChild[0].name
