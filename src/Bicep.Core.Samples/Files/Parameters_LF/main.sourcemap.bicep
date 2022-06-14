/* 
  This is a block comment.
*/

// parameters without default value
@sys.description('''
this is my multi line 
//@[11:16]     "myString": {\r
description for my myString
''')
param myString string
param myInt int
//@[17:19]     "myInt": {\r
param myBool bool
//@[20:22]     "myBool": {\r

// parameters with default value
@sys.description('this is myString2')
@metadata({
  description: 'overwrite but still valid'
//@[23:29]     "myString2": {\r
})
param myString2 string = 'string value'
param myInt2 int = 42
//@[30:33]     "myInt2": {\r
param myTruth bool = true
//@[34:37]     "myTruth": {\r
param myFalsehood bool = false
//@[38:41]     "myFalsehood": {\r
param myEscapedString string = 'First line\r\nSecond\ttabbed\tline'
//@[42:45]     "myEscapedString": {\r

// object default value
@sys.description('this is foo')
@metadata({
  description: 'overwrite but still valid'
//@[46:73]     "foo": {\r
  another: 'just for fun'
//@[71:71]         "another": "just for fun"\r
})
param foo object = {
  enabled: true
//@[49:49]         "enabled": true,\r
  name: 'this is my object'
//@[50:50]         "name": "this is my object",\r
  priority: 3
//@[51:51]         "priority": 3,\r
  info: {
//@[52:54]         "info": {\r
    a: 'b'
//@[53:53]           "a": "b"\r
  }
  empty: {
//@[55:55]         "empty": {},\r
  }
  array: [
//@[56:67]         "array": [\r
    'string item'
//@[57:57]           "string item",\r
    12
//@[58:58]           12,\r
    true
//@[59:59]           true,\r
    [
      'inner'
//@[61:61]             "inner",\r
      false
//@[62:62]             false\r
    ]
    {
      a: 'b'
//@[65:65]             "a": "b"\r
    }
  ]
}

// array default value
param myArrayParam array = [
//@[74:81]     "myArrayParam": {\r
  'a'
//@[77:77]         "a",\r
  'b'
//@[78:78]         "b",\r
  'c'
//@[79:79]         "c"\r
]

// secure string
@secure()
param password string
//@[82:84]     "password": {\r

// secure object
@secure()
param secretObject object
//@[85:87]     "secretObject": {\r

// enum parameter
@allowed([
  'Standard_LRS'
//@[88:94]     "storageSku": {\r
  'Standard_GRS'
//@[92:92]         "Standard_GRS"\r
])
param storageSku string

// length constraint on a string
@minLength(3)
@maxLength(24)
param storageName string
//@[95:99]     "storageName": {\r

// length constraint on an array
@minLength(3)
@maxLength(24)
param someArray array
//@[100:104]     "someArray": {\r

// empty metadata
@metadata({})
param emptyMetadata string
//@[105:108]     "emptyMetadata": {\r

// description
@metadata({
  description: 'my description'
//@[109:114]     "description": {\r
})
param description string

@sys.description('my description')
param description2 string
//@[115:120]     "description2": {\r

// random extra metadata
@metadata({
  description: 'my description'
//@[121:132]     "additionalMetadata": {\r
  a: 1
//@[125:125]         "a": 1,\r
  b: true
//@[126:126]         "b": true,\r
  c: [
//@[127:127]         "c": [],\r
  ]
  d: {
//@[128:130]         "d": {\r
    test: 'abc'
//@[129:129]           "test": "abc"\r
  }
})
param additionalMetadata string

// all modifiers together
@secure()
@minLength(3)
@maxLength(24)
@allowed([
  'one'
//@[139:139]         "one",\r
  'two'
//@[133:145]     "someParameter": {\r
  'three'
//@[141:141]         "three"\r
])
@metadata({
  description: 'Name of the storage account'
//@[136:136]         "description": "Name of the storage account"\r
})
param someParameter string

param defaultExpression bool = 18 != (true || false)
//@[146:149]     "defaultExpression": {\r

@allowed([
  'abc'
//@[150:156]     "stringLiteral": {\r
  'def'
//@[154:154]         "def"\r
])
param stringLiteral string

@allowed([
  'abc'
//@[157:165]     "stringLiteralWithAllowedValuesSuperset": {\r
  'def'
//@[162:162]         "def",\r
  'ghi'
//@[163:163]         "ghi"\r
])
param stringLiteralWithAllowedValuesSuperset string = stringLiteral

@secure()
@minLength(2)
  @maxLength(10)
@allowed([
  'Apple'
//@[166:174]     "decoratedString": {\r
  'Banana'
//@[170:170]         "Banana"\r
])
param decoratedString string

@minValue(200)
param decoratedInt int = 123
//@[175:179]     "decoratedInt": {\r

// negative integer literals are allowed as decorator values
@minValue(-10)
@maxValue(-3)
param negativeValues int
//@[180:184]     "negativeValues": {\r

@sys.description('A boolean.')
@metadata({
    description: 'I will be overrode.'
//@[185:197]     "decoratedBool": {\r
    foo: 'something'
//@[190:190]         "foo": "something",\r
    bar: [
//@[191:195]         "bar": [\r
        {          }
        true
//@[193:193]           true,\r
        123
//@[194:194]           123\r
    ]
})
param decoratedBool bool = (true && false) != true

@secure()
param decoratedObject object = {
//@[198:221]     "decoratedObject": {\r
  enabled: true
//@[201:201]         "enabled": true,\r
  name: 'this is my object'
//@[202:202]         "name": "this is my object",\r
  priority: 3
//@[203:203]         "priority": 3,\r
  info: {
//@[204:206]         "info": {\r
    a: 'b'
//@[205:205]           "a": "b"\r
  }
  empty: {
//@[207:207]         "empty": {},\r
  }
  array: [
//@[208:219]         "array": [\r
    'string item'
//@[209:209]           "string item",\r
    12
//@[210:210]           12,\r
    true
//@[211:211]           true,\r
    [
      'inner'
//@[213:213]             "inner",\r
      false
//@[214:214]             false\r
    ]
    {
      a: 'b'
//@[217:217]             "a": "b"\r
    }
  ]
}

@sys.metadata({
    description: 'An array.'
})
@sys.maxLength(20)
//@[222:232]     "decoratedArray": {\r
@sys.description('I will be overrode.')
param decoratedArray array = [
    utcNow()
//@[225:225]         "[utcNow()]",\r
    newGuid()
//@[226:226]         "[newGuid()]"\r
]

