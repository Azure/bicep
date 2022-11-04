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

// allowed constraint on arrays
@allowed(['fizz'])
//@[115:117]       "allowedValues": [
param fizzArray array
//@[113:118]     "fizzArray": {

@allowed([true])
//@[121:123]       "allowedValues": [
param trueArray array
//@[119:124]     "trueArray": {

@allowed([1])
//@[127:129]       "allowedValues": [
param oneArray array
//@[125:130]     "oneArray": {

@allowed(['fizz', 'buzz', 'pop'])
//@[133:137]       "allowedValues": [
param permittedSubsetArray array
//@[131:138]     "permittedSubsetArray": {

// empty metadata
@metadata({})
//@[141:141]       "metadata": {}
param emptyMetadata string
//@[139:142]     "emptyMetadata": {

// description
@metadata({
//@[145:147]       "metadata": {
  description: 'my description'
//@[146:146]         "description": "my description"
})
param description string
//@[143:148]     "description": {

@sys.description('my description')
//@[152:152]         "description": "my description"
param description2 string
//@[149:154]     "description2": {

// random extra metadata
@metadata({
//@[157:165]       "metadata": {
  description: 'my description'
//@[158:158]         "description": "my description",
  a: 1
//@[159:159]         "a": 1,
  b: true
//@[160:160]         "b": true,
  c: [
//@[161:161]         "c": [],
  ]
  d: {
//@[162:164]         "d": {
    test: 'abc'
//@[163:163]           "test": "abc"
  }
})
param additionalMetadata string
//@[155:166]     "additionalMetadata": {

// all modifiers together
@secure()
@minLength(3)
//@[178:178]       "minLength": 3
@maxLength(24)
//@[177:177]       "maxLength": 24,
@allowed([
//@[172:176]       "allowedValues": [
  'one'
//@[173:173]         "one",
  'two'
//@[174:174]         "two",
  'three'
//@[175:175]         "three"
])
@metadata({
//@[169:171]       "metadata": {
  description: 'Name of the storage account'
//@[170:170]         "description": "Name of the storage account"
})
param someParameter string
//@[167:179]     "someParameter": {

param defaultExpression bool = 18 != (true || false)
//@[180:183]     "defaultExpression": {

@allowed([
//@[186:189]       "allowedValues": [
  'abc'
//@[187:187]         "abc",
  'def'
//@[188:188]         "def"
])
param stringLiteral string
//@[184:190]     "stringLiteral": {

@allowed([
//@[194:198]       "allowedValues": [
  'abc'
//@[195:195]         "abc",
  'def'
//@[196:196]         "def",
  'ghi'
//@[197:197]         "ghi"
])
param stringLiteralWithAllowedValuesSuperset string = stringLiteral
//@[191:199]     "stringLiteralWithAllowedValuesSuperset": {

@secure()
@minLength(2)
//@[207:207]       "minLength": 2
  @maxLength(10)
//@[206:206]       "maxLength": 10,
@allowed([
//@[202:205]       "allowedValues": [
  'Apple'
//@[203:203]         "Apple",
  'Banana'
//@[204:204]         "Banana"
])
param decoratedString string
//@[200:208]     "decoratedString": {

@minValue(200)
//@[212:212]       "minValue": 200
param decoratedInt int = 123
//@[209:213]     "decoratedInt": {

// negative integer literals are allowed as decorator values
@minValue(-10)
//@[217:217]       "minValue": -10
@maxValue(-3)
//@[216:216]       "maxValue": -3,
param negativeValues int
//@[214:218]     "negativeValues": {

@sys.description('A boolean.')
//@[223:223]         "description": "A boolean.",
@metadata({
    description: 'I will be overrode.'
    foo: 'something'
//@[224:224]         "foo": "something",
    bar: [
//@[225:229]         "bar": [
        {          }
        true
//@[227:227]           true,
        123
//@[228:228]           123
    ]
})
param decoratedBool bool = (true && false) != true
//@[219:231]     "decoratedBool": {

@secure()
param decoratedObject object = {
//@[232:255]     "decoratedObject": {
  enabled: true
//@[235:235]         "enabled": true,
  name: 'this is my object'
//@[236:236]         "name": "this is my object",
  priority: 3
//@[237:237]         "priority": 3,
  info: {
//@[238:240]         "info": {
    a: 'b'
//@[239:239]           "a": "b"
  }
  empty: {
//@[241:241]         "empty": {},
  }
  array: [
//@[242:253]         "array": [
    'string item'
//@[243:243]           "string item",
    12
//@[244:244]           12,
    true
//@[245:245]           true,
    [
      'inner'
//@[247:247]             "inner",
      false
//@[248:248]             false
    ]
    {
      a: 'b'
//@[251:251]             "a": "b"
    }
  ]
}

@sys.metadata({
    description: 'An array.'
//@[263:263]         "description": "An array."
})
@sys.maxLength(20)
//@[265:265]       "maxLength": 20
@sys.description('I will be overrode.')
param decoratedArray array = [
//@[256:266]     "decoratedArray": {
    utcNow()
//@[259:259]         "[utcNow()]",
    newGuid()
//@[260:260]         "[newGuid()]"
]

