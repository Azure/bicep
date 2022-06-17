/* 
  This is a block comment.
*/

// parameters without default value
param myString string
//@[12:14]     "myString": {
param myInt int
//@[15:17]     "myInt": {
param myBool bool
//@[18:20]     "myBool": {

// parameters with default value
param myString2 string = 'string value'
//@[21:24]     "myString2": {
param myInt2 int = 42
//@[25:28]     "myInt2": {
param myTruth bool = true
//@[29:32]     "myTruth": {
param myFalsehood bool = false
//@[33:36]     "myFalsehood": {
param myEscapedString string = 'First line\r\nSecond\ttabbed\tline'
//@[37:40]     "myEscapedString": {

// object default value
param foo object = {
//@[41:64]     "foo": {
  enabled: true
//@[44:44]         "enabled": true,
  name: 'this is my object'
//@[45:45]         "name": "this is my object",
  priority: 3
//@[46:46]         "priority": 3,
  info: {
//@[47:49]         "info": {
    a: 'b'
//@[48:48]           "a": "b"
  }
  empty: {
//@[50:50]         "empty": {},
  }
  array: [
//@[51:62]         "array": [
    'string item'
//@[52:52]           "string item",
    12
//@[53:53]           12,
    true
//@[54:54]           true,
    [
      'inner'
//@[56:56]             "inner",
      false
//@[57:57]             false
    ]
    {
      a: 'b'
//@[60:60]             "a": "b"
    }
  ]
}

// array default value
param myArrayParam array = [
//@[65:72]     "myArrayParam": {
  'a'
//@[68:68]         "a",
  'b'
//@[69:69]         "b",
  'c'
//@[70:70]         "c"
]

// secure string
@secure()
param password string
//@[73:75]     "password": {

// secure object
@secure()
param secretObject object
//@[76:78]     "secretObject": {

// enum parameter
@allowed([
  'Standard_LRS'
//@[82:82]         "Standard_LRS",
  'Standard_GRS'
//@[83:83]         "Standard_GRS"
])
param storageSku string
//@[79:85]     "storageSku": {

// length constraint on a string
@minLength(3)
@maxLength(24)
param storageName string
//@[86:90]     "storageName": {

// length constraint on an array
@minLength(3)
@maxLength(24)
param someArray array
//@[91:95]     "someArray": {

// empty metadata
@metadata({})
param emptyMetadata string
//@[96:99]     "emptyMetadata": {

// description
@metadata({
  description: 'my description'
//@[103:103]         "description": "my description"
})
param description string
//@[100:105]     "description": {

@sys.description('my description')
param description2 string
//@[106:111]     "description2": {

// random extra metadata
@metadata({
  description: 'my description'
//@[115:115]         "description": "my description",
  a: 1
//@[116:116]         "a": 1,
  b: true
//@[117:117]         "b": true,
  c: [
//@[118:118]         "c": [],
  ]
  d: {
//@[119:121]         "d": {
    test: 'abc'
//@[120:120]           "test": "abc"
  }
})
param additionalMetadata string
//@[112:123]     "additionalMetadata": {

// all modifiers together
@secure()
@minLength(3)
@maxLength(24)
@allowed([
  'one'
//@[130:130]         "one",
  'two'
//@[131:131]         "two",
  'three'
//@[132:132]         "three"
])
@metadata({
  description: 'Name of the storage account'
//@[127:127]         "description": "Name of the storage account"
})
param someParameter string
//@[124:136]     "someParameter": {

param defaultExpression bool = 18 != (true || false)
//@[137:140]     "defaultExpression": {

@allowed([
  'abc'
//@[144:144]         "abc",
  'def'
//@[145:145]         "def"
])
param stringLiteral string
//@[141:147]     "stringLiteral": {

@allowed([
  'abc'
//@[152:152]         "abc",
  'def'
//@[153:153]         "def",
  'ghi'
//@[154:154]         "ghi"
])
param stringLiteralWithAllowedValuesSuperset string = stringLiteral
//@[148:156]     "stringLiteralWithAllowedValuesSuperset": {

@secure()
@minLength(2)
  @maxLength(10)
@allowed([
  'Apple'
//@[160:160]         "Apple",
  'Banana'
//@[161:161]         "Banana"
])
param decoratedString string
//@[157:165]     "decoratedString": {

@minValue(200)
param decoratedInt int = 123
//@[166:170]     "decoratedInt": {

// negative integer literals are allowed as decorator values
@minValue(-10)
@maxValue(-3)
param negativeValues int
//@[171:175]     "negativeValues": {

@sys.description('A boolean.')
@metadata({
    description: 'I will be overrode.'
    foo: 'something'
//@[181:181]         "foo": "something",
    bar: [
//@[182:186]         "bar": [
        {          }
        true
//@[184:184]           true,
        123
//@[185:185]           123
    ]
})
param decoratedBool bool = (true && false) != true
//@[176:188]     "decoratedBool": {

@secure()
param decoratedObject object = {
//@[189:212]     "decoratedObject": {
  enabled: true
//@[192:192]         "enabled": true,
  name: 'this is my object'
//@[193:193]         "name": "this is my object",
  priority: 3
//@[194:194]         "priority": 3,
  info: {
//@[195:197]         "info": {
    a: 'b'
//@[196:196]           "a": "b"
  }
  empty: {
//@[198:198]         "empty": {},
  }
  array: [
//@[199:210]         "array": [
    'string item'
//@[200:200]           "string item",
    12
//@[201:201]           12,
    true
//@[202:202]           true,
    [
      'inner'
//@[204:204]             "inner",
      false
//@[205:205]             false
    ]
    {
      a: 'b'
//@[208:208]             "a": "b"
    }
  ]
}

@sys.metadata({
    description: 'An array.'
})
@sys.maxLength(20)
@sys.description('I will be overrode.')
param decoratedArray array = [
//@[213:223]     "decoratedArray": {
    utcNow()
//@[216:216]         "[utcNow()]",
    newGuid()
//@[217:217]         "[newGuid()]"
]

