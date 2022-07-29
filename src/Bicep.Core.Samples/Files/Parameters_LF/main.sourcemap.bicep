/* 
  This is a block comment.
*/

// parameters without default value
@sys.description('''
//@[14:14]         "description": "this is my multi line \ndescription for my myString\n"
this is my multi line 
description for my myString
''')
param myString string
//@[11:16]     "myString": {
param myInt int
//@[17:19]     "myInt": {
param myBool bool
//@[20:22]     "myBool": {

// parameters with default value
@sys.description('this is myString2')
//@[27:27]         "description": "this is myString2"
@metadata({
  description: 'overwrite but still valid'
})
param myString2 string = 'string value'
//@[23:29]     "myString2": {
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
//@[70:70]         "description": "this is foo",
@metadata({
  description: 'overwrite but still valid'
  another: 'just for fun'
//@[71:71]         "another": "just for fun"
})
param foo object = {
//@[46:73]     "foo": {
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
//@[90:93]       "allowedValues": [
  'Standard_LRS'
//@[91:91]         "Standard_LRS",
  'Standard_GRS'
//@[92:92]         "Standard_GRS"
])
param storageSku string
//@[88:94]     "storageSku": {

// length constraint on a string
@minLength(3)
//@[98:98]       "minLength": 3
@maxLength(24)
//@[97:97]       "maxLength": 24,
param storageName string
//@[95:99]     "storageName": {

// length constraint on an array
@minLength(3)
//@[103:103]       "minLength": 3
@maxLength(24)
//@[102:102]       "maxLength": 24,
param someArray array
//@[100:104]     "someArray": {

// empty metadata
@metadata({})
//@[107:107]       "metadata": {}
param emptyMetadata string
//@[105:108]     "emptyMetadata": {

// description
@metadata({
//@[111:113]       "metadata": {
  description: 'my description'
//@[112:112]         "description": "my description"
})
param description string
//@[109:114]     "description": {

@sys.description('my description')
//@[118:118]         "description": "my description"
param description2 string
//@[115:120]     "description2": {

// random extra metadata
@metadata({
//@[123:131]       "metadata": {
  description: 'my description'
//@[124:124]         "description": "my description",
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
//@[121:132]     "additionalMetadata": {

// all modifiers together
@secure()
@minLength(3)
//@[144:144]       "minLength": 3
@maxLength(24)
//@[143:143]       "maxLength": 24,
@allowed([
//@[138:142]       "allowedValues": [
  'one'
//@[139:139]         "one",
  'two'
//@[140:140]         "two",
  'three'
//@[141:141]         "three"
])
@metadata({
//@[135:137]       "metadata": {
  description: 'Name of the storage account'
//@[136:136]         "description": "Name of the storage account"
})
param someParameter string
//@[133:145]     "someParameter": {

param defaultExpression bool = 18 != (true || false)
//@[146:149]     "defaultExpression": {

@allowed([
//@[152:155]       "allowedValues": [
  'abc'
//@[153:153]         "abc",
  'def'
//@[154:154]         "def"
])
param stringLiteral string
//@[150:156]     "stringLiteral": {

@allowed([
//@[160:164]       "allowedValues": [
  'abc'
//@[161:161]         "abc",
  'def'
//@[162:162]         "def",
  'ghi'
//@[163:163]         "ghi"
])
param stringLiteralWithAllowedValuesSuperset string = stringLiteral
//@[157:165]     "stringLiteralWithAllowedValuesSuperset": {

@secure()
@minLength(2)
//@[173:173]       "minLength": 2
  @maxLength(10)
//@[172:172]       "maxLength": 10,
@allowed([
//@[168:171]       "allowedValues": [
  'Apple'
//@[169:169]         "Apple",
  'Banana'
//@[170:170]         "Banana"
])
param decoratedString string
//@[166:174]     "decoratedString": {

@minValue(200)
//@[178:178]       "minValue": 200
param decoratedInt int = 123
//@[175:179]     "decoratedInt": {

// negative integer literals are allowed as decorator values
@minValue(-10)
//@[184:184]       "minValue": -10
@maxValue(-3)
//@[183:183]       "maxValue": -3,
param negativeValues int = -8
//@[180:185]     "negativeValues": {

@sys.description('A boolean.')
//@[190:190]         "description": "A boolean.",
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
//@[230:230]         "description": "An array."
})
@sys.maxLength(20)
//@[232:232]       "maxLength": 20
@sys.description('I will be overrode.')
param decoratedArray array = [
//@[223:233]     "decoratedArray": {
    utcNow()
//@[226:226]         "[utcNow()]",
    newGuid()
//@[227:227]         "[newGuid()]"
]

