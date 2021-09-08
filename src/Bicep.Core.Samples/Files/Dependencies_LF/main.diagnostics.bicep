param deployTimeParam string = 'steve'
var deployTimeVar = 'nigel'
var dependentVar = {
  dependencies: [
    deployTimeVar
    deployTimeParam
  ]
}

var resourceDependency = {
  dependenciesA: [
    resA.id
    resA.name
    resA.type
    resA.properties.deployTime
    resA.properties.eTag
  ]
}

output resourceAType string = resA.type
resource resA 'My.Rp/myResourceType@2020-01-01' = {
//@[14:47) [BCP081 (Warning)] Resource type "My.Rp/myResourceType@2020-01-01" does not have types available. (CodeDescription: none) |'My.Rp/myResourceType@2020-01-01'|
  name: 'resA'
  properties: {
    deployTime: dependentVar
    eTag: '1234'
  }
}

output resourceBId string = resB.id
resource resB 'My.Rp/myResourceType@2020-01-01' = {
//@[14:47) [BCP081 (Warning)] Resource type "My.Rp/myResourceType@2020-01-01" does not have types available. (CodeDescription: none) |'My.Rp/myResourceType@2020-01-01'|
  name: 'resB'
  properties: {
    dependencies: resourceDependency
  }
}

var resourceIds = {
  a: resA.id
  b: resB.id
}

resource resC 'My.Rp/myResourceType@2020-01-01' = {
//@[14:47) [BCP081 (Warning)] Resource type "My.Rp/myResourceType@2020-01-01" does not have types available. (CodeDescription: none) |'My.Rp/myResourceType@2020-01-01'|
  name: 'resC'
  properties: {
    resourceIds: resourceIds
  }
}

resource resD 'My.Rp/myResourceType/childType@2020-01-01' = {
//@[14:57) [BCP081 (Warning)] Resource type "My.Rp/myResourceType/childType@2020-01-01" does not have types available. (CodeDescription: none) |'My.Rp/myResourceType/childType@2020-01-01'|
  name: '${resC.name}/resD'
  properties: {
  }
}

resource resE 'My.Rp/myResourceType/childType@2020-01-01' = {
//@[14:57) [BCP081 (Warning)] Resource type "My.Rp/myResourceType/childType@2020-01-01" does not have types available. (CodeDescription: none) |'My.Rp/myResourceType/childType@2020-01-01'|
  name: 'resC/resD'
  properties: {
    resDRef: resD.id
  }
}

output resourceCProperties object = resC.properties

