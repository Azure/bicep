param deployTimeParam string = 'steve'
//@[11:14]     "deployTimeParam": {
var deployTimeVar = 'nigel'
//@[17:17]     "deployTimeVar": "nigel",
var dependentVar = {
//@[18:23]     "dependentVar": {
  dependencies: [
//@[19:22]       "dependencies": [
    deployTimeVar
//@[20:20]         "[variables('deployTimeVar')]",
    deployTimeParam
//@[21:21]         "[parameters('deployTimeParam')]"
  ]
}

var resourceDependency = {
  dependenciesA: [
//@[45:51]           "dependenciesA": [
    resA.id
//@[46:46]             "[resourceId('My.Rp/myResourceType', 'resA')]",
    resA.name
//@[47:47]             "resA",
    resA.type
//@[48:48]             "My.Rp/myResourceType",
    resA.properties.deployTime
//@[49:49]             "[reference(resourceId('My.Rp/myResourceType', 'resA'), '2020-01-01').deployTime]",
    resA.properties.eTag
//@[50:50]             "[reference(resourceId('My.Rp/myResourceType', 'resA'), '2020-01-01').eTag]"
  ]
}

output resourceAType string = resA.type
//@[92:95]     "resourceAType": {
resource resA 'My.Rp/myResourceType@2020-01-01' = {
//@[30:38]       "type": "My.Rp/myResourceType",
  name: 'resA'
  properties: {
//@[34:37]       "properties": {
    deployTime: dependentVar
//@[35:35]         "deployTime": "[variables('dependentVar')]",
    eTag: '1234'
//@[36:36]         "eTag": "1234"
  }
}

output resourceBId string = resB.id
//@[96:99]     "resourceBId": {
resource resB 'My.Rp/myResourceType@2020-01-01' = {
//@[39:57]       "type": "My.Rp/myResourceType",
  name: 'resB'
  properties: {
//@[43:53]       "properties": {
    dependencies: resourceDependency
//@[44:52]         "dependencies": {
  }
}

var resourceIds = {
//@[24:27]     "resourceIds": {
  a: resA.id
//@[25:25]       "a": "[resourceId('My.Rp/myResourceType', 'resA')]",
  b: resB.id
//@[26:26]       "b": "[resourceId('My.Rp/myResourceType', 'resB')]"
}

resource resC 'My.Rp/myResourceType@2020-01-01' = {
//@[58:69]       "type": "My.Rp/myResourceType",
  name: 'resC'
  properties: {
//@[62:64]       "properties": {
    resourceIds: resourceIds
//@[63:63]         "resourceIds": "[variables('resourceIds')]"
  }
}

resource resD 'My.Rp/myResourceType/childType@2020-01-01' = {
//@[70:78]       "type": "My.Rp/myResourceType/childType",
  name: '${resC.name}/resD'
  properties: {
//@[74:74]       "properties": {},
  }
}

resource resE 'My.Rp/myResourceType/childType@2020-01-01' = {
//@[79:89]       "type": "My.Rp/myResourceType/childType",
  name: 'resC/resD'
  properties: {
//@[83:85]       "properties": {
    resDRef: resD.id
//@[84:84]         "resDRef": "[resourceId('My.Rp/myResourceType/childType', split(format('{0}/resD', 'resC'), '/')[0], split(format('{0}/resD', 'resC'), '/')[1])]"
  }
}

output resourceCProperties object = resC.properties
//@[100:103]     "resourceCProperties": {

