param deployTimeParam string = 'steve'
//@[6:21) Parameter deployTimeParam. Declaration start char: 0, length: 39
var deployTimeVar = 'nigel'
//@[4:17) Variable deployTimeVar. Declaration start char: 0, length: 28
var dependentVar = {
//@[4:16) Variable dependentVar. Declaration start char: 0, length: 84
  dependencies: [
    deployTimeVar
    deployTimeParam
  ]
}

var resourceDependency = {
//@[4:22) Variable resourceDependency. Declaration start char: 0, length: 138
  dependenciesA: [
    resA.id
    resA.name
    resA.type
    resA.properties.deployTime
    resA.eTag
  ]
}

output resourceAType string = resA.type
//@[7:20) Output resourceAType. Declaration start char: 0, length: 40
resource resA 'My.Rp/myResourceType@2020-01-01' = {
//@[9:13) Resource resA. Declaration start char: 0, length: 134
  name: 'resA'
  properties: {
    deployTime: dependentVar
  }
  eTag: '1234'
}

output resourceBId string = resB.id
//@[7:18) Output resourceBId. Declaration start char: 0, length: 36
resource resB 'My.Rp/myResourceType@2020-01-01' = {
//@[9:13) Resource resB. Declaration start char: 0, length: 127
  name: 'resB'
  properties: {
    dependencies: resourceDependency
  }
}

var resourceIds = {
//@[4:15) Variable resourceIds. Declaration start char: 0, length: 49
  a: resA.id
  b: resB.id
}

resource resC 'My.Rp/myResourceType@2020-01-01' = {
//@[9:13) Resource resC. Declaration start char: 0, length: 119
  name: 'resC'
  properties: {
    resourceIds: resourceIds
  }
}

resource resD 'My.Rp/myResourceType/childType@2020-01-01' = {
//@[9:13) Resource resD. Declaration start char: 0, length: 113
  name: '${resC.name}/resD'
  properties: {
  }
}

resource resE 'My.Rp/myResourceType/childType@2020-01-01' = {
//@[9:13) Resource resE. Declaration start char: 0, length: 126
  name: 'resC/resD'
  properties: {
    resDRef: resD.id
  }
}

output resourceCProperties object = resC.properties
//@[7:26) Output resourceCProperties. Declaration start char: 0, length: 51
