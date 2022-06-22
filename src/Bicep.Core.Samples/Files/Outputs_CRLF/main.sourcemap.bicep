
@sys.description('string output description')
output myStr string = 'hello'
//@[20:26]     "myStr": {

@sys.description('int output description')
output myInt int = 7
//@[27:33]     "myInt": {
output myOtherInt int = 20 / 13 + 80 % -4
//@[34:37]     "myOtherInt": {

@sys.description('bool output description')
output myBool bool = !false
//@[38:44]     "myBool": {
output myOtherBool bool = true
//@[45:48]     "myOtherBool": {

@sys.description('object array description')
output suchEmpty array = [
//@[49:55]     "suchEmpty": {
]

output suchEmpty2 object = {
//@[56:59]     "suchEmpty2": {
}

@sys.description('object output description')
output obj object = {
//@[60:83]     "obj": {
  a: 'a'
  b: 12
  c: true
  d: null
  list: [
    1
//@[68:68]           1,
    2
//@[69:69]           2,
    3
//@[70:70]           3,
    null
//@[71:71]           null,
    {
    }
  ]
  obj: {
    nested: [
      'hello'
//@[76:76]             "hello"
    ]
  }
}

output myArr array = [
//@[84:91]     "myArr": {
  'pirates'
//@[87:87]         "pirates",
  'say'
//@[88:88]         "say",
   false ? 'arr2' : 'arr'
//@[89:89]         "[if(false(), 'arr2', 'arr')]"
]

output rgLocation string = resourceGroup().location
//@[92:95]     "rgLocation": {

output isWestUs bool = resourceGroup().location != 'westus' ? false : true
//@[96:99]     "isWestUs": {

output expressionBasedIndexer string = {
//@[100:103]     "expressionBasedIndexer": {
  eastus: {
    foo: true
  }
  westus: {
    foo: false
  }
}[resourceGroup().location].foo

var secondaryKeyIntermediateVar = listKeys(resourceId('Mock.RP/type', 'steve'), '2020-01-01').secondaryKey

output primaryKey string = listKeys(resourceId('Mock.RP/type', 'nigel'), '2020-01-01').primaryKey
//@[104:107]     "primaryKey": {
output secondaryKey string = secondaryKeyIntermediateVar
//@[108:111]     "secondaryKey": {

var varWithOverlappingOutput = 'hello'
//@[16:16]     "varWithOverlappingOutput": "hello"
param paramWithOverlappingOutput string
//@[11:13]     "paramWithOverlappingOutput": {

output varWithOverlappingOutput string = varWithOverlappingOutput
//@[112:115]     "varWithOverlappingOutput": {
output paramWithOverlappingOutput string = paramWithOverlappingOutput
//@[116:119]     "paramWithOverlappingOutput": {

// top-level output loops are supported
output generatedArray array = [for i in range(0,10): i]
//@[120:126]     "generatedArray": {

