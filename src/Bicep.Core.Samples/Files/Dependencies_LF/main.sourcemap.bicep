param deployTimeParam string = 'steve'
//@[11:14]     "deployTimeParam": {\r
var deployTimeVar = 'nigel'
//@[17:17]     "deployTimeVar": "nigel",\r
var dependentVar = {
//@[18:23]     "dependentVar": {\r
  dependencies: [
//@[19:22]       "dependencies": [\r
    deployTimeVar
//@[20:20]         "[variables('deployTimeVar')]",\r
    deployTimeParam
//@[21:21]         "[parameters('deployTimeParam')]"\r
  ]
}

var resourceDependency = {
  dependenciesA: [
//@[45:51]           "dependenciesA": [\r
    resA.id
//@[46:46]             "[resourceId('My.Rp/myResourceType', 'resA')]",\r
    resA.name
//@[47:47]             "resA",\r
    resA.type
//@[48:48]             "My.Rp/myResourceType",\r
    resA.properties.deployTime
//@[49:49]             "[reference(resourceId('My.Rp/myResourceType', 'resA')).deployTime]",\r
    resA.properties.eTag
//@[50:50]             "[reference(resourceId('My.Rp/myResourceType', 'resA')).eTag]"\r
  ]
}

output resourceAType string = resA.type
//@[92:95]     "resourceAType": {\r
resource resA 'My.Rp/myResourceType@2020-01-01' = {
//@[30:38]       "type": "My.Rp/myResourceType",\r
  name: 'resA'
  properties: {
//@[34:37]       "properties": {\r
    deployTime: dependentVar
//@[35:35]         "deployTime": "[variables('dependentVar')]",\r
    eTag: '1234'
//@[36:36]         "eTag": "1234"\r
  }
}

output resourceBId string = resB.id
//@[96:99]     "resourceBId": {\r
resource resB 'My.Rp/myResourceType@2020-01-01' = {
//@[39:57]       "type": "My.Rp/myResourceType",\r
  name: 'resB'
  properties: {
//@[43:53]       "properties": {\r
    dependencies: resourceDependency
//@[44:52]         "dependencies": {\r
  }
}

var resourceIds = {
//@[24:27]     "resourceIds": {\r
  a: resA.id
//@[25:25]       "a": "[resourceId('My.Rp/myResourceType', 'resA')]",\r
  b: resB.id
//@[26:26]       "b": "[resourceId('My.Rp/myResourceType', 'resB')]"\r
}

resource resC 'My.Rp/myResourceType@2020-01-01' = {
//@[58:69]       "type": "My.Rp/myResourceType",\r
  name: 'resC'
  properties: {
//@[62:64]       "properties": {\r
    resourceIds: resourceIds
//@[63:63]         "resourceIds": "[variables('resourceIds')]"\r
  }
}

resource resD 'My.Rp/myResourceType/childType@2020-01-01' = {
//@[70:78]       "type": "My.Rp/myResourceType/childType",\r
  name: '${resC.name}/resD'
  properties: {
//@[74:74]       "properties": {},\r
  }
}

resource resE 'My.Rp/myResourceType/childType@2020-01-01' = {
//@[79:89]       "type": "My.Rp/myResourceType/childType",\r
  name: 'resC/resD'
  properties: {
//@[83:85]       "properties": {\r
    resDRef: resD.id
//@[84:84]         "resDRef": "[resourceId('My.Rp/myResourceType/childType', split(format('{0}/resD', 'resC'), '/')[0], split(format('{0}/resD', 'resC'), '/')[1])]"\r
  }
}

output resourceCProperties object = resC.properties
//@[100:103]     "resourceCProperties": {\r

