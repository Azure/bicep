
@sys.description('string output description')
output myStr string = 'hello'
//@[07:12) Output myStr. Type: string. Declaration start char: 0, length: 76

@sys.description('int output description')
output myInt int = 7
//@[07:12) Output myInt. Type: int. Declaration start char: 0, length: 64
output myOtherInt int = 20 / 13 + 80 % -4
//@[07:17) Output myOtherInt. Type: int. Declaration start char: 0, length: 41

@sys.description('bool output description')
output myBool bool = !false
//@[07:13) Output myBool. Type: bool. Declaration start char: 0, length: 72
output myOtherBool bool = true
//@[07:18) Output myOtherBool. Type: bool. Declaration start char: 0, length: 30

@sys.description('object array description')
output suchEmpty array = [
//@[07:16) Output suchEmpty. Type: array. Declaration start char: 0, length: 75
]

output suchEmpty2 object = {
//@[07:17) Output suchEmpty2. Type: object. Declaration start char: 0, length: 31
}

@sys.description('object output description')
output obj object = {
//@[07:10) Output obj. Type: object. Declaration start char: 0, length: 225
  a: 'a'
  b: 12
  c: true
  d: null
  list: [
    1
    2
    3
    null
    {
    }
  ]
  obj: {
    nested: [
      'hello'
    ]
  }
}

output myArr array = [
//@[07:12) Output myArr. Type: array. Declaration start char: 0, length: 74
  'pirates'
  'say'
   false ? 'arr2' : 'arr'
]

output rgLocation string = resourceGroup().location
//@[07:17) Output rgLocation. Type: string. Declaration start char: 0, length: 51

output isWestUs bool = resourceGroup().location != 'westus' ? false : true
//@[07:15) Output isWestUs. Type: bool. Declaration start char: 0, length: 74

output expressionBasedIndexer string = {
//@[07:29) Output expressionBasedIndexer. Type: string. Declaration start char: 0, length: 140
  eastus: {
    foo: true
  }
  westus: {
    foo: false
  }
}[resourceGroup().location].foo

var secondaryKeyIntermediateVar = listKeys(resourceId('Mock.RP/type', 'steve'), '2020-01-01').secondaryKey
//@[04:31) Variable secondaryKeyIntermediateVar. Type: any. Declaration start char: 0, length: 106

output primaryKey string = listKeys(resourceId('Mock.RP/type', 'nigel'), '2020-01-01').primaryKey
//@[07:17) Output primaryKey. Type: string. Declaration start char: 0, length: 97
output secondaryKey string = secondaryKeyIntermediateVar
//@[07:19) Output secondaryKey. Type: string. Declaration start char: 0, length: 56

var varWithOverlappingOutput = 'hello'
//@[04:28) Variable varWithOverlappingOutput. Type: 'hello'. Declaration start char: 0, length: 38
param paramWithOverlappingOutput string
//@[06:32) Parameter paramWithOverlappingOutput. Type: string. Declaration start char: 0, length: 39

output varWithOverlappingOutput string = varWithOverlappingOutput
//@[07:31) Output varWithOverlappingOutput. Type: string. Declaration start char: 0, length: 65
output paramWithOverlappingOutput string = paramWithOverlappingOutput
//@[07:33) Output paramWithOverlappingOutput. Type: string. Declaration start char: 0, length: 69

// top-level output loops are supported
output generatedArray array = [for i in range(0,10): i]
//@[35:36) Local i. Type: int. Declaration start char: 35, length: 1
//@[07:21) Output generatedArray. Type: array. Declaration start char: 0, length: 55

