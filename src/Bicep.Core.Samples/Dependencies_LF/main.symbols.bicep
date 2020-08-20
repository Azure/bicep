param deployTimeParam string = 'steve'
//@[6:21] Parameter deployTimeParam
var deployTimeVar = 'nigel'
//@[4:17] Variable deployTimeVar
var dependentVar = {
//@[4:16] Variable dependentVar
  dependencies: [
    deployTimeVar
    deployTimeParam
  ]
}

var resourceDependency = {
//@[4:22] Variable resourceDependency
  dependenciesA: [
    resA.id
    resA.name
    resA.type
    resA.properties.deployTime
    resA.eTag
  ]
}

output resourceAType string = resA.type
//@[7:20] Output resourceAType
resource resA 'My.Rp/myResourceType@2020-01-01' = {
//@[9:13] Resource resA
  name: 'resA'
  properties: {
    deployTime: dependentVar
  }
  eTag: '1234'
}

output resourceBId string = resB.id
//@[7:18] Output resourceBId
resource resB 'My.Rp/myResourceType@2020-01-01' = {
//@[9:13] Resource resB
  name: 'resB'
  properties: {
    dependencies: resourceDependency
  }
}

var resourceIds = {
//@[4:15] Variable resourceIds
  a: resA.id
  b: resB.id
}

resource resC 'My.Rp/myResourceType@2020-01-01' = {
//@[9:13] Resource resC
  name: 'resC'
  properties: {
    resourceIds: resourceIds
  }
}

resource resD 'My.Rp/myResourceType/childType@2020-01-01' = {
//@[9:13] Resource resD
  name: '${resC.name}/resD'
  properties: {
  }
}

resource resE 'My.Rp/myResourceType/childType@2020-01-01' = {
//@[9:13] Resource resE
  name: 'resC/resD'
  properties: {
    resDRef: resD.id
  }
}

output resourceCProperties object = resC.properties
//@[7:26] Output resourceCProperties
