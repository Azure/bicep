/* 
  This is a block comment.
*/

// parameters without default value
@sys.description('''
this is my multi line 
description for my myString
''')
param myString string
//@[12:17]     "myString": {
param myInt int
//@[18:20]     "myInt": {
param myBool bool
//@[21:23]     "myBool": {

// parameters with default value
@sys.description('this is myString2')
@metadata({
  description: 'overwrite but still valid'
})
param myString2 string = 'string value'
//@[24:30]     "myString2": {
param myInt2 int = 42
//@[31:34]     "myInt2": {
param myTruth bool = true
//@[35:38]     "myTruth": {
param myFalsehood bool = false
//@[39:42]     "myFalsehood": {
param myEscapedString string = 'First line\r\nSecond\ttabbed\tline'
//@[43:46]     "myEscapedString": {

// object default value
@sys.description('this is foo')
@metadata({
  description: 'overwrite but still valid'
  another: 'just for fun'
//@[72:72]         "another": "just for fun"
})
param foo object = {
//@[47:74]     "foo": {
  enabled: true
//@[50:50]         "enabled": true,
  name: 'this is my object'
//@[51:51]         "name": "this is my object",
  priority: 3
//@[52:52]         "priority": 3,
  info: {
//@[53:55]         "info": {
    a: 'b'
//@[54:54]           "a": "b"
  }
  empty: {
//@[56:56]         "empty": {},
  }
  array: [
//@[57:68]         "array": [
    'string item'
//@[58:58]           "string item",
    12
//@[59:59]           12,
    true
//@[60:60]           true,
    [
      'inner'
//@[62:62]             "inner",
      false
//@[63:63]             false
    ]
    {
      a: 'b'
//@[66:66]             "a": "b"
    }
  ]
}

// array default value
param myArrayParam array = [
//@[75:82]     "myArrayParam": {
  'a'
//@[78:78]         "a",
  'b'
//@[79:79]         "b",
  'c'
//@[80:80]         "c"
]

// secure string
@secure()
param password string
//@[83:85]     "password": {

// secure object
@secure()
param secretObject object
//@[86:88]     "secretObject": {

// enum parameter
@allowed([
  'Standard_LRS'
//@[92:92]         "Standard_LRS",
  'Standard_GRS'
//@[93:93]         "Standard_GRS"
])
param storageSku string
//@[89:95]     "storageSku": {

// length constraint on a string
@minLength(3)
@maxLength(24)
param storageName string
//@[96:100]     "storageName": {

// length constraint on an array
@minLength(3)
@maxLength(24)
param someArray array
//@[101:105]     "someArray": {

// empty metadata
@metadata({})
param emptyMetadata string
//@[106:109]     "emptyMetadata": {

// description
@metadata({
  description: 'my description'
//@[113:113]         "description": "my description"
})
param description string
//@[110:115]     "description": {

@sys.description('my description')
param description2 string
//@[116:121]     "description2": {

// random extra metadata
@metadata({
  description: 'my description'
//@[125:125]         "description": "my description",
  a: 1
//@[126:126]         "a": 1,
  b: true
//@[127:127]         "b": true,
  c: [
//@[128:128]         "c": [],
  ]
  d: {
//@[129:131]         "d": {
    test: 'abc'
//@[130:130]           "test": "abc"
  }
})
param additionalMetadata string
//@[122:133]     "additionalMetadata": {

// all modifiers together
@secure()
@minLength(3)
@maxLength(24)
@allowed([
  'one'
//@[140:140]         "one",
  'two'
//@[141:141]         "two",
  'three'
//@[142:142]         "three"
])
@metadata({
  description: 'Name of the storage account'
//@[137:137]         "description": "Name of the storage account"
})
param someParameter string
//@[134:146]     "someParameter": {

param defaultExpression bool = 18 != (true || false)
//@[147:150]     "defaultExpression": {

@allowed([
  'abc'
//@[154:154]         "abc",
  'def'
//@[155:155]         "def"
])
param stringLiteral string
//@[151:157]     "stringLiteral": {

@allowed([
  'abc'
//@[162:162]         "abc",
  'def'
//@[163:163]         "def",
  'ghi'
//@[164:164]         "ghi"
])
param stringLiteralWithAllowedValuesSuperset string = stringLiteral
//@[158:166]     "stringLiteralWithAllowedValuesSuperset": {

@secure()
@minLength(2)
  @maxLength(10)
@allowed([
  'Apple'
//@[170:170]         "Apple",
  'Banana'
//@[171:171]         "Banana"
])
param decoratedString string
//@[167:175]     "decoratedString": {

@minValue(200)
param decoratedInt int = 123
//@[176:180]     "decoratedInt": {

// negative integer literals are allowed as decorator values
@minValue(-10)
@maxValue(-3)
param negativeValues int
//@[181:185]     "negativeValues": {

@sys.description('A boolean.')
@metadata({
    description: 'I will be overrode.'
    foo: 'something'
//@[191:191]         "foo": "something",
    bar: [
//@[192:196]         "bar": [
        {          }
        true
//@[194:194]           true,
        123
//@[195:195]           123
    ]
})
param decoratedBool bool = (true && false) != true
//@[186:198]     "decoratedBool": {

@secure()
param decoratedObject object = {
//@[199:222]     "decoratedObject": {
  enabled: true
//@[202:202]         "enabled": true,
  name: 'this is my object'
//@[203:203]         "name": "this is my object",
  priority: 3
//@[204:204]         "priority": 3,
  info: {
//@[205:207]         "info": {
    a: 'b'
//@[206:206]           "a": "b"
  }
  empty: {
//@[208:208]         "empty": {},
  }
  array: [
//@[209:220]         "array": [
    'string item'
//@[210:210]           "string item",
    12
//@[211:211]           12,
    true
//@[212:212]           true,
    [
      'inner'
//@[214:214]             "inner",
      false
//@[215:215]             false
    ]
    {
      a: 'b'
//@[218:218]             "a": "b"
    }
  ]
}

@sys.metadata({
    description: 'An array.'
})
@sys.maxLength(20)
@sys.description('I will be overrode.')
param decoratedArray array = [
//@[223:233]     "decoratedArray": {
    utcNow()
//@[226:226]         "[utcNow()]",
    newGuid()
//@[227:227]         "[newGuid()]"
]

