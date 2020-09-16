
output myStr string = 'hello'
//@[7:12) Output myStr. Type: string. Declaration start char: 0, length: 33

output myInt int = 7
//@[7:12) Output myInt. Type: int. Declaration start char: 0, length: 22
output myOtherInt int = 20 / 13 + 80 % -4
//@[7:17) Output myOtherInt. Type: int. Declaration start char: 0, length: 45

output myBool bool = !false
//@[7:13) Output myBool. Type: bool. Declaration start char: 0, length: 29
output myOtherBool bool = true
//@[7:18) Output myOtherBool. Type: bool. Declaration start char: 0, length: 34

output suchEmpty array = [
//@[7:16) Output suchEmpty. Type: array. Declaration start char: 0, length: 33
]

output suchEmpty2 object = {
//@[7:17) Output suchEmpty2. Type: object. Declaration start char: 0, length: 35
}

output obj object = {
//@[7:10) Output obj. Type: object. Declaration start char: 0, length: 182
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
//@[7:12) Output myArr. Type: array. Declaration start char: 0, length: 78
  'pirates'
  'say'
   false ? 'arr2' : 'arr'
]

output rgLocation string = resourceGroup().location
//@[7:17) Output rgLocation. Type: string. Declaration start char: 0, length: 55

output crossRegion bool = resourceGroup().location == deployment().location ? false : true
//@[7:18) Output crossRegion. Type: bool. Declaration start char: 0, length: 94

output expressionBasedIndexer string = {
//@[7:29) Output expressionBasedIndexer. Type: string. Declaration start char: 0, length: 144
  eastus: {
    foo: true
  }
  westus: {
    foo: false
  }
}[resourceGroup().location].foo

var secondaryKeyIntermediateVar = listKeys(resourceId('Mock.RP/type', 'steve'), '2020-01-01').secondaryKey
//@[4:31) Variable secondaryKeyIntermediateVar. Type: any. Declaration start char: 0, length: 110

output primaryKey string = listKeys(resourceId('Mock.RP/type', 'nigel'), '2020-01-01').primaryKey
//@[7:17) Output primaryKey. Type: string. Declaration start char: 0, length: 99
output secondaryKey string = secondaryKeyIntermediateVar
//@[7:19) Output secondaryKey. Type: string. Declaration start char: 0, length: 56
