parameter deployTimeParam string = 'steve'
variable deployTimeVar = 'nigel'
variable dependentVar = {
  dependencies: [
    deployTimeVar
    deployTimeParam
  ]
}

variable resourceDependency = {
  dependenciesA: [
    resA.id
    resA.name
    resA.type
    resA.properties.deployTime
    resA.eTag
  ]
}

output resourceAType string = resA.type
resource resA 'My.Rp/myResourceType@2020-01-01' = {
  name: 'resA'
  properties: {
    deployTime: dependentVar
  }
  eTag: '1234'
}

output resourceBId string = resB.id
resource resB 'My.Rp/myResourceType@2020-01-01' = {
  name: 'resB'
  properties: {
    dependencies: resourceDependency
  }
}

variable resourceIds = {
  a: resA.id
  b: resB.id
}

resource resC 'My.Rp/myResourceType@2020-01-01' = {
  name: 'resC'
  properties: {
    resourceIds: resourceIds
  }
}

output resourceCProperties object = resC.properties