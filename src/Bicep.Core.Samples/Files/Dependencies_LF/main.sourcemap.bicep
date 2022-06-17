param deployTimeParam string = 'steve'
//@[12:15]     "deployTimeParam": {
var deployTimeVar = 'nigel'
//@[18:18]     "deployTimeVar": "nigel",
var dependentVar = {
//@[19:24]     "dependentVar": {
  dependencies: [
//@[20:23]       "dependencies": [
    deployTimeVar
//@[21:21]         "[variables('deployTimeVar')]",
    deployTimeParam
//@[22:22]         "[parameters('deployTimeParam')]"
  ]
}

var resourceDependency = {
  dependenciesA: [
//@[46:52]           "dependenciesA": [
    resA.id
//@[47:47]             "[resourceId('My.Rp/myResourceType', 'resA')]",
    resA.name
//@[48:48]             "resA",
    resA.type
//@[49:49]             "My.Rp/myResourceType",
    resA.properties.deployTime
//@[50:50]             "[reference(resourceId('My.Rp/myResourceType', 'resA')).deployTime]",
    resA.properties.eTag
//@[51:51]             "[reference(resourceId('My.Rp/myResourceType', 'resA')).eTag]"
  ]
}

output resourceAType string = resA.type
//@[93:96]     "resourceAType": {
resource resA 'My.Rp/myResourceType@2020-01-01' = {
//@[31:39]       "type": "My.Rp/myResourceType",
  name: 'resA'
  properties: {
//@[35:38]       "properties": {
    deployTime: dependentVar
//@[36:36]         "deployTime": "[variables('dependentVar')]",
    eTag: '1234'
//@[37:37]         "eTag": "1234"
  }
}

output resourceBId string = resB.id
//@[97:100]     "resourceBId": {
resource resB 'My.Rp/myResourceType@2020-01-01' = {
//@[40:58]       "type": "My.Rp/myResourceType",
  name: 'resB'
  properties: {
//@[44:54]       "properties": {
    dependencies: resourceDependency
//@[45:53]         "dependencies": {
  }
}

var resourceIds = {
//@[25:28]     "resourceIds": {
  a: resA.id
//@[26:26]       "a": "[resourceId('My.Rp/myResourceType', 'resA')]",
  b: resB.id
//@[27:27]       "b": "[resourceId('My.Rp/myResourceType', 'resB')]"
}

resource resC 'My.Rp/myResourceType@2020-01-01' = {
//@[59:70]       "type": "My.Rp/myResourceType",
  name: 'resC'
  properties: {
//@[63:65]       "properties": {
    resourceIds: resourceIds
//@[64:64]         "resourceIds": "[variables('resourceIds')]"
  }
}

resource resD 'My.Rp/myResourceType/childType@2020-01-01' = {
//@[71:79]       "type": "My.Rp/myResourceType/childType",
  name: '${resC.name}/resD'
  properties: {
//@[75:75]       "properties": {},
  }
}

resource resE 'My.Rp/myResourceType/childType@2020-01-01' = {
//@[80:90]       "type": "My.Rp/myResourceType/childType",
  name: 'resC/resD'
  properties: {
//@[84:86]       "properties": {
    resDRef: resD.id
//@[85:85]         "resDRef": "[resourceId('My.Rp/myResourceType/childType', split(format('{0}/resD', 'resC'), '/')[0], split(format('{0}/resD', 'resC'), '/')[1])]"
  }
}

output resourceCProperties object = resC.properties
//@[101:104]     "resourceCProperties": {

