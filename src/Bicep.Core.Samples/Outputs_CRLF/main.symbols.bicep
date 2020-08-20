
output myStr string = 'hello'
//@[7:12] Output myStr

output myInt int = 7
//@[7:12] Output myInt
output myOtherInt int = 20 / 13 + 80 % -4
//@[7:17] Output myOtherInt

output myBool bool = !false
//@[7:13] Output myBool
output myOtherBool bool = true
//@[7:18] Output myOtherBool

output suchEmpty array = [
//@[7:16] Output suchEmpty
]

output suchEmpty2 object = {
//@[7:17] Output suchEmpty2
}

output obj object = {
//@[7:10] Output obj
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
//@[7:12] Output myArr
  'pirates'
  'say'
   false ? 'arr2' : 'arr'
]

output rgLocation string = resourceGroup().location
//@[7:17] Output rgLocation

output crossRegion bool = resourceGroup().location == deployment().location ? false : true
//@[7:18] Output crossRegion

output expressionBasedIndexer string = {
//@[7:29] Output expressionBasedIndexer
  eastus: {
    foo: true
  }
  westus: {
    foo: false
  }
}[resourceGroup().location].foo

var secondaryKeyIntermediateVar = listKeys(resourceId('Mock.RP/type', 'steve'), '2020-01-01').secondaryKey
//@[4:31] Variable secondaryKeyIntermediateVar

output primaryKey string = listKeys(resourceId('Mock.RP/type', 'nigel'), '2020-01-01').primaryKey
//@[7:17] Output primaryKey
output secondaryKey string = secondaryKeyIntermediateVar
//@[7:19] Output secondaryKey
