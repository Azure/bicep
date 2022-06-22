param deployTimeParam string = 'steve'
//@[11:14]     "deployTimeParam": {
var deployTimeVar = 'nigel'
//@[17:17]     "deployTimeVar": "nigel",
var dependentVar = {
//@[18:23]     "dependentVar": {
  dependencies: [
    deployTimeVar
//@[20:20]         "[variables('deployTimeVar')]",
    deployTimeParam
//@[21:21]         "[parameters('deployTimeParam')]"
  ]
}

var resourceDependency = {
  dependenciesA: [
    resA.id
//@[46:46]             "[resourceId('My.Rp/myResourceType', 'resA')]",
    resA.name
//@[47:47]             "resA",
    resA.type
//@[48:48]             "My.Rp/myResourceType",
    resA.properties.deployTime
//@[49:49]             "[reference(resourceId('My.Rp/myResourceType', 'resA')).deployTime]",
    resA.properties.eTag
//@[50:50]             "[reference(resourceId('My.Rp/myResourceType', 'resA')).eTag]"
  ]
}

output resourceAType string = resA.type
//@[92:95]     "resourceAType": {
resource resA 'My.Rp/myResourceType@2020-01-01' = {
//@[30:38]       "type": "My.Rp/myResourceType",
  name: 'resA'
  properties: {
    deployTime: dependentVar
    eTag: '1234'
  }
}

output resourceBId string = resB.id
//@[96:99]     "resourceBId": {
resource resB 'My.Rp/myResourceType@2020-01-01' = {
//@[39:57]       "type": "My.Rp/myResourceType",
  name: 'resB'
  properties: {
    dependencies: resourceDependency
  }
}

var resourceIds = {
//@[24:27]     "resourceIds": {
  a: resA.id
  b: resB.id
}

resource resC 'My.Rp/myResourceType@2020-01-01' = {
//@[58:69]       "type": "My.Rp/myResourceType",
  name: 'resC'
  properties: {
    resourceIds: resourceIds
  }
}

resource resD 'My.Rp/myResourceType/childType@2020-01-01' = {
//@[70:78]       "type": "My.Rp/myResourceType/childType",
  name: '${resC.name}/resD'
  properties: {
  }
}

resource resE 'My.Rp/myResourceType/childType@2020-01-01' = {
//@[79:89]       "type": "My.Rp/myResourceType/childType",
  name: 'resC/resD'
  properties: {
    resDRef: resD.id
  }
}

output resourceCProperties object = resC.properties
//@[100:103]     "resourceCProperties": {

