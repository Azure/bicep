/* 
  This is a block comment.
*/

// parameters without default value
@sys.description('''
this is my multi line 
//@[11:16]     "myString": {
description for my myString
''')
param myString string
param myInt int
//@[17:19]     "myInt": {
param myBool bool
//@[20:22]     "myBool": {

// parameters with default value
@sys.description('this is myString2')
@metadata({
  description: 'overwrite but still valid'
//@[23:29]     "myString2": {
})
param myString2 string = 'string value'
param myInt2 int = 42
//@[30:33]     "myInt2": {
param myTruth bool = true
//@[34:37]     "myTruth": {
param myFalsehood bool = false
//@[38:41]     "myFalsehood": {
param myEscapedString string = 'First line\r\nSecond\ttabbed\tline'
//@[42:45]     "myEscapedString": {

// object default value
@sys.description('this is foo')
@metadata({
  description: 'overwrite but still valid'
//@[46:73]     "foo": {
  another: 'just for fun'
//@[71:71]         "another": "just for fun"
})
param foo object = {
  enabled: true
//@[49:49]         "enabled": true,
  name: 'this is my object'
//@[50:50]         "name": "this is my object",
  priority: 3
//@[51:51]         "priority": 3,
  info: {
//@[52:54]         "info": {
    a: 'b'
//@[53:53]           "a": "b"
  }
  empty: {
//@[55:55]         "empty": {},
  }
  array: [
//@[56:67]         "array": [
    'string item'
//@[57:57]           "string item",
    12
//@[58:58]           12,
    true
//@[59:59]           true,
    [
      'inner'
//@[61:61]             "inner",
      false
//@[62:62]             false
    ]
    {
      a: 'b'
//@[65:65]             "a": "b"
    }
  ]
}

// array default value
param myArrayParam array = [
//@[74:81]     "myArrayParam": {
  'a'
//@[77:77]         "a",
  'b'
//@[78:78]         "b",
  'c'
//@[79:79]         "c"
]

// secure string
@secure()
param password string
//@[82:84]     "password": {

// secure object
@secure()
param secretObject object
//@[85:87]     "secretObject": {

// enum parameter
@allowed([
  'Standard_LRS'
//@[88:94]     "storageSku": {
  'Standard_GRS'
//@[92:92]         "Standard_GRS"
])
param storageSku string

// length constraint on a string
@minLength(3)
@maxLength(24)
param storageName string
//@[95:99]     "storageName": {

// length constraint on an array
@minLength(3)
@maxLength(24)
param someArray array
//@[100:104]     "someArray": {

// empty metadata
@metadata({})
param emptyMetadata string
//@[105:108]     "emptyMetadata": {

// description
@metadata({
  description: 'my description'
//@[109:114]     "description": {
})
param description string

@sys.description('my description')
param description2 string
//@[115:120]     "description2": {

// random extra metadata
@metadata({
  description: 'my description'
//@[121:132]     "additionalMetadata": {
  a: 1
//@[125:125]         "a": 1,
  b: true
//@[126:126]         "b": true,
  c: [
//@[127:127]         "c": [],
  ]
  d: {
//@[128:130]         "d": {
    test: 'abc'
//@[129:129]           "test": "abc"
  }
})
param additionalMetadata string

// all modifiers together
@secure()
@minLength(3)
@maxLength(24)
@allowed([
  'one'
//@[139:139]         "one",
  'two'
//@[133:145]     "someParameter": {
  'three'
//@[141:141]         "three"
])
@metadata({
  description: 'Name of the storage account'
//@[136:136]         "description": "Name of the storage account"
})
param someParameter string

param defaultExpression bool = 18 != (true || false)
//@[146:149]     "defaultExpression": {

@allowed([
  'abc'
//@[150:156]     "stringLiteral": {
  'def'
//@[154:154]         "def"
])
param stringLiteral string

@allowed([
  'abc'
//@[157:165]     "stringLiteralWithAllowedValuesSuperset": {
  'def'
//@[162:162]         "def",
  'ghi'
//@[163:163]         "ghi"
])
param stringLiteralWithAllowedValuesSuperset string = stringLiteral

@secure()
@minLength(2)
  @maxLength(10)
@allowed([
  'Apple'
//@[166:174]     "decoratedString": {
  'Banana'
//@[170:170]         "Banana"
])
param decoratedString string

@minValue(200)
param decoratedInt int = 123
//@[175:179]     "decoratedInt": {

// negative integer literals are allowed as decorator values
@minValue(-10)
@maxValue(-3)
param negativeValues int
//@[180:184]     "negativeValues": {

@sys.description('A boolean.')
@metadata({
    description: 'I will be overrode.'
//@[185:197]     "decoratedBool": {
    foo: 'something'
//@[190:190]         "foo": "something",
    bar: [
//@[191:195]         "bar": [
        {          }
        true
//@[193:193]           true,
        123
//@[194:194]           123
    ]
})
param decoratedBool bool = (true && false) != true

@secure()
param decoratedObject object = {
//@[198:221]     "decoratedObject": {
  enabled: true
//@[201:201]         "enabled": true,
  name: 'this is my object'
//@[202:202]         "name": "this is my object",
  priority: 3
//@[203:203]         "priority": 3,
  info: {
//@[204:206]         "info": {
    a: 'b'
//@[205:205]           "a": "b"
  }
  empty: {
//@[207:207]         "empty": {},
  }
  array: [
//@[208:219]         "array": [
    'string item'
//@[209:209]           "string item",
    12
//@[210:210]           12,
    true
//@[211:211]           true,
    [
      'inner'
//@[213:213]             "inner",
      false
//@[214:214]             false
    ]
    {
      a: 'b'
//@[217:217]             "a": "b"
    }
  ]
}

@sys.metadata({
    description: 'An array.'
})
@sys.maxLength(20)
//@[222:232]     "decoratedArray": {
@sys.description('I will be overrode.')
param decoratedArray array = [
    utcNow()
//@[225:225]         "[utcNow()]",
    newGuid()
//@[226:226]         "[newGuid()]"
]

