/*
  This is a block comment.
*/

// parameters without default value
@sys.description('''
//@[14:14]         "description": "this is my multi line\ndescription for my myString\n"
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

@allowed([
//@[97:101]       "allowedValues": [
  1
//@[98:98]         1,
  2
//@[99:99]         2,
  3
//@[100:100]         3
])
param intEnum int
//@[95:102]     "intEnum": {

// length constraint on a string
@minLength(3)
//@[106:106]       "minLength": 3
@maxLength(24)
//@[105:105]       "maxLength": 24,
param storageName string
//@[103:107]     "storageName": {

// length constraint on an array
@minLength(3)
//@[111:111]       "minLength": 3
@maxLength(24)
//@[110:110]       "maxLength": 24,
param someArray array
//@[108:112]     "someArray": {

// empty metadata
@metadata({})
//@[115:115]       "metadata": {}
param emptyMetadata string
//@[113:116]     "emptyMetadata": {

// description
@metadata({
//@[119:121]       "metadata": {
  description: 'my description'
//@[120:120]         "description": "my description"
})
param description string
//@[117:122]     "description": {

@sys.description('my description')
//@[126:126]         "description": "my description"
param description2 string
//@[123:128]     "description2": {

// random extra metadata
@metadata({
//@[131:139]       "metadata": {
  description: 'my description'
//@[132:132]         "description": "my description",
  a: 1
//@[133:133]         "a": 1,
  b: true
//@[134:134]         "b": true,
  c: [
//@[135:135]         "c": [],
  ]
  d: {
//@[136:138]         "d": {
    test: 'abc'
//@[137:137]           "test": "abc"
  }
})
param additionalMetadata string
//@[129:140]     "additionalMetadata": {

// all modifiers together
@secure()
@minLength(3)
//@[152:152]       "minLength": 3
@maxLength(24)
//@[151:151]       "maxLength": 24,
@allowed([
//@[146:150]       "allowedValues": [
  'one'
//@[147:147]         "one",
  'two'
//@[148:148]         "two",
  'three'
//@[149:149]         "three"
])
@metadata({
//@[143:145]       "metadata": {
  description: 'Name of the storage account'
//@[144:144]         "description": "Name of the storage account"
})
param someParameter string
//@[141:153]     "someParameter": {

param defaultExpression bool = 18 != (true || false)
//@[154:157]     "defaultExpression": {

@allowed([
//@[160:163]       "allowedValues": [
  'abc'
//@[161:161]         "abc",
  'def'
//@[162:162]         "def"
])
param stringLiteral string
//@[158:164]     "stringLiteral": {

@allowed([
//@[168:172]       "allowedValues": [
  'abc'
//@[169:169]         "abc",
  'def'
//@[170:170]         "def",
  'ghi'
//@[171:171]         "ghi"
])
param stringLiteralWithAllowedValuesSuperset string = stringLiteral
//@[165:173]     "stringLiteralWithAllowedValuesSuperset": {

@secure()
@minLength(2)
//@[181:181]       "minLength": 2
  @maxLength(10)
//@[180:180]       "maxLength": 10,
@allowed([
//@[176:179]       "allowedValues": [
  'Apple'
//@[177:177]         "Apple",
  'Banana'
//@[178:178]         "Banana"
])
param decoratedString string
//@[174:182]     "decoratedString": {

@minValue(200)
//@[186:186]       "minValue": 200
param decoratedInt int = 123
//@[183:187]     "decoratedInt": {

// negative integer literals are allowed as decorator values
@minValue(-10)
//@[191:191]       "minValue": -10
@maxValue(-3)
//@[190:190]       "maxValue": -3,
param negativeValues int
//@[188:192]     "negativeValues": {

@sys.description('A boolean.')
//@[197:197]         "description": "A boolean.",
@metadata({
    description: 'I will be overrode.'
    foo: 'something'
//@[198:198]         "foo": "something",
    bar: [
//@[199:203]         "bar": [
        {          }
        true
//@[201:201]           true,
        123
//@[202:202]           123
    ]
})
param decoratedBool bool = /* comment1 */ /* comment2*/      /* comment3 */ /* comment4 */ (true && false) != true
//@[193:205]     "decoratedBool": {

@secure()
param decoratedObject object = {
//@[206:229]     "decoratedObject": {
  enabled: true
//@[209:209]         "enabled": true,
  name: 'this is my object'
//@[210:210]         "name": "this is my object",
  priority: 3
//@[211:211]         "priority": 3,
  info: {
//@[212:214]         "info": {
    a: 'b'
//@[213:213]           "a": "b"
  }
  empty: {
//@[215:215]         "empty": {},
  }
  array: [
//@[216:227]         "array": [
    'string item'
//@[217:217]           "string item",
    12
//@[218:218]           12,
    true
//@[219:219]           true,
    [
      'inner'
//@[221:221]             "inner",
      false
//@[222:222]             false
    ]
    {
      a: 'b'
//@[225:225]             "a": "b"
    }
  ]
}

@sys.metadata({
    description: 'An array.'
//@[237:237]         "description": "An array."
})
@sys.maxLength(20)
//@[239:239]       "maxLength": 20
@sys.description('I will be overrode.')
param decoratedArray array = [
//@[230:240]     "decoratedArray": {
    utcNow()
//@[233:233]         "[utcNow()]",
    newGuid()
//@[234:234]         "[newGuid()]"
]

