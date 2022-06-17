
@sys.description('string output description')
output myStr string = 'hello'
//@[21:27]     "myStr": {

@sys.description('int output description')
output myInt int = 7
//@[28:34]     "myInt": {
output myOtherInt int = 20 / 13 + 80 % -4
//@[35:38]     "myOtherInt": {

@sys.description('bool output description')
output myBool bool = !false
//@[39:45]     "myBool": {
output myOtherBool bool = true
//@[46:49]     "myOtherBool": {

@sys.description('object array description')
output suchEmpty array = [
//@[50:56]     "suchEmpty": {
]

output suchEmpty2 object = {
//@[57:60]     "suchEmpty2": {
}

@sys.description('object output description')
output obj object = {
//@[61:84]     "obj": {
  a: 'a'
//@[64:64]         "a": "a",
  b: 12
//@[65:65]         "b": 12,
  c: true
//@[66:66]         "c": true,
  d: null
//@[67:67]         "d": null,
  list: [
//@[68:74]         "list": [
    1
//@[69:69]           1,
    2
//@[70:70]           2,
    3
//@[71:71]           3,
    null
//@[72:72]           null,
    {
    }
  ]
  obj: {
//@[75:79]         "obj": {
    nested: [
//@[76:78]           "nested": [
      'hello'
//@[77:77]             "hello"
    ]
  }
}

output myArr array = [
//@[85:92]     "myArr": {
  'pirates'
//@[88:88]         "pirates",
  'say'
//@[89:89]         "say",
   false ? 'arr2' : 'arr'
//@[90:90]         "[if(false(), 'arr2', 'arr')]"
]

output rgLocation string = resourceGroup().location
//@[93:96]     "rgLocation": {

output isWestUs bool = resourceGroup().location != 'westus' ? false : true
//@[97:100]     "isWestUs": {

output expressionBasedIndexer string = {
//@[101:104]     "expressionBasedIndexer": {
  eastus: {
    foo: true
  }
  westus: {
    foo: false
  }
}[resourceGroup().location].foo

var secondaryKeyIntermediateVar = listKeys(resourceId('Mock.RP/type', 'steve'), '2020-01-01').secondaryKey

output primaryKey string = listKeys(resourceId('Mock.RP/type', 'nigel'), '2020-01-01').primaryKey
//@[105:108]     "primaryKey": {
output secondaryKey string = secondaryKeyIntermediateVar
//@[109:112]     "secondaryKey": {

var varWithOverlappingOutput = 'hello'
//@[17:17]     "varWithOverlappingOutput": "hello"
param paramWithOverlappingOutput string
//@[12:14]     "paramWithOverlappingOutput": {

output varWithOverlappingOutput string = varWithOverlappingOutput
//@[113:116]     "varWithOverlappingOutput": {
output paramWithOverlappingOutput string = paramWithOverlappingOutput
//@[117:120]     "paramWithOverlappingOutput": {

// top-level output loops are supported
output generatedArray array = [for i in range(0,10): i]
//@[121:127]     "generatedArray": {

