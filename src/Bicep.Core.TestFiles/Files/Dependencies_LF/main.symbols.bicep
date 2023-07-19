param deployTimeParam string = 'steve'
//@[6:21) Parameter deployTimeParam. Type: string. Declaration start char: 0, length: 38
var deployTimeVar = 'nigel'
//@[4:17) Variable deployTimeVar. Type: 'nigel'. Declaration start char: 0, length: 27
var dependentVar = {
//@[4:16) Variable dependentVar. Type: object. Declaration start char: 0, length: 82
  dependencies: [
    deployTimeVar
    deployTimeParam
  ]
}

var resourceDependency = {
//@[4:22) Variable resourceDependency. Type: object. Declaration start char: 0, length: 147
  dependenciesA: [
    resA.id
    resA.name
    resA.type
    resA.properties.deployTime
    resA.properties.eTag
  ]
}

output resourceAType string = resA.type
//@[7:20) Output resourceAType. Type: string. Declaration start char: 0, length: 39
resource resA 'My.Rp/myResourceType@2020-01-01' = {
//@[9:13) Resource resA. Type: My.Rp/myResourceType@2020-01-01. Declaration start char: 0, length: 134
  name: 'resA'
  properties: {
    deployTime: dependentVar
    eTag: '1234'
  }
}

output resourceBId string = resB.id
//@[7:18) Output resourceBId. Type: string. Declaration start char: 0, length: 35
resource resB 'My.Rp/myResourceType@2020-01-01' = {
//@[9:13) Resource resB. Type: My.Rp/myResourceType@2020-01-01. Declaration start char: 0, length: 125
  name: 'resB'
  properties: {
    dependencies: resourceDependency
  }
}

var resourceIds = {
//@[4:15) Variable resourceIds. Type: object. Declaration start char: 0, length: 47
  a: resA.id
  b: resB.id
}

resource resC 'My.Rp/myResourceType@2020-01-01' = {
//@[9:13) Resource resC. Type: My.Rp/myResourceType@2020-01-01. Declaration start char: 0, length: 117
  name: 'resC'
  properties: {
    resourceIds: resourceIds
  }
}

resource resD 'My.Rp/myResourceType/childType@2020-01-01' = {
//@[9:13) Resource resD. Type: My.Rp/myResourceType/childType@2020-01-01. Declaration start char: 0, length: 111
  name: '${resC.name}/resD'
  properties: {
  }
}

resource resE 'My.Rp/myResourceType/childType@2020-01-01' = {
//@[9:13) Resource resE. Type: My.Rp/myResourceType/childType@2020-01-01. Declaration start char: 0, length: 124
  name: 'resC/resD'
  properties: {
    resDRef: resD.id
  }
}

output resourceCProperties object = resC.properties
//@[7:26) Output resourceCProperties. Type: object. Declaration start char: 0, length: 51

