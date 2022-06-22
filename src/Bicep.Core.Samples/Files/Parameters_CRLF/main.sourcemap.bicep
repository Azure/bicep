/* 
  This is a block comment.
*/

// parameters without default value
param myString string
//@[11:13]     "myString": {
param myInt int
//@[14:16]     "myInt": {
param myBool bool
//@[17:19]     "myBool": {

// parameters with default value
param myString2 string = 'string value'
//@[20:23]     "myString2": {
param myInt2 int = 42
//@[24:27]     "myInt2": {
param myTruth bool = true
//@[28:31]     "myTruth": {
param myFalsehood bool = false
//@[32:35]     "myFalsehood": {
param myEscapedString string = 'First line\r\nSecond\ttabbed\tline'
//@[36:39]     "myEscapedString": {

// object default value
param foo object = {
//@[40:63]     "foo": {
  enabled: true
  name: 'this is my object'
  priority: 3
  info: {
    a: 'b'
  }
  empty: {
  }
  array: [
    'string item'
//@[51:51]           "string item",
    12
//@[52:52]           12,
    true
//@[53:53]           true,
    [
      'inner'
//@[55:55]             "inner",
      false
//@[56:56]             false
    ]
    {
      a: 'b'
    }
  ]
}

// array default value
param myArrayParam array = [
//@[64:71]     "myArrayParam": {
  'a'
//@[67:67]         "a",
  'b'
//@[68:68]         "b",
  'c'
//@[69:69]         "c"
]

// secure string
@secure()
param password string
//@[72:74]     "password": {

// secure object
@secure()
param secretObject object
//@[75:77]     "secretObject": {

// enum parameter
@allowed([
  'Standard_LRS'
//@[81:81]         "Standard_LRS",
  'Standard_GRS'
//@[82:82]         "Standard_GRS"
])
param storageSku string
//@[78:84]     "storageSku": {

// length constraint on a string
@minLength(3)
@maxLength(24)
param storageName string
//@[85:89]     "storageName": {

// length constraint on an array
@minLength(3)
@maxLength(24)
param someArray array
//@[90:94]     "someArray": {

// empty metadata
@metadata({})
param emptyMetadata string
//@[95:98]     "emptyMetadata": {

// description
@metadata({
  description: 'my description'
})
param description string
//@[99:104]     "description": {

@sys.description('my description')
param description2 string
//@[105:110]     "description2": {

// random extra metadata
@metadata({
  description: 'my description'
  a: 1
  b: true
  c: [
  ]
  d: {
    test: 'abc'
  }
})
param additionalMetadata string
//@[111:122]     "additionalMetadata": {

// all modifiers together
@secure()
@minLength(3)
@maxLength(24)
@allowed([
  'one'
//@[129:129]         "one",
  'two'
//@[130:130]         "two",
  'three'
//@[131:131]         "three"
])
@metadata({
  description: 'Name of the storage account'
})
param someParameter string
//@[123:135]     "someParameter": {

param defaultExpression bool = 18 != (true || false)
//@[136:139]     "defaultExpression": {

@allowed([
  'abc'
//@[143:143]         "abc",
  'def'
//@[144:144]         "def"
])
param stringLiteral string
//@[140:146]     "stringLiteral": {

@allowed([
  'abc'
//@[151:151]         "abc",
  'def'
//@[152:152]         "def",
  'ghi'
//@[153:153]         "ghi"
])
param stringLiteralWithAllowedValuesSuperset string = stringLiteral
//@[147:155]     "stringLiteralWithAllowedValuesSuperset": {

@secure()
@minLength(2)
  @maxLength(10)
@allowed([
  'Apple'
//@[159:159]         "Apple",
  'Banana'
//@[160:160]         "Banana"
])
param decoratedString string
//@[156:164]     "decoratedString": {

@minValue(200)
param decoratedInt int = 123
//@[165:169]     "decoratedInt": {

// negative integer literals are allowed as decorator values
@minValue(-10)
@maxValue(-3)
param negativeValues int
//@[170:174]     "negativeValues": {

@sys.description('A boolean.')
@metadata({
    description: 'I will be overrode.'
    foo: 'something'
    bar: [
        {          }
        true
//@[183:183]           true,
        123
//@[184:184]           123
    ]
})
param decoratedBool bool = (true && false) != true
//@[175:187]     "decoratedBool": {

@secure()
param decoratedObject object = {
//@[188:211]     "decoratedObject": {
  enabled: true
  name: 'this is my object'
  priority: 3
  info: {
    a: 'b'
  }
  empty: {
  }
  array: [
    'string item'
//@[199:199]           "string item",
    12
//@[200:200]           12,
    true
//@[201:201]           true,
    [
      'inner'
//@[203:203]             "inner",
      false
//@[204:204]             false
    ]
    {
      a: 'b'
    }
  ]
}

@sys.metadata({
    description: 'An array.'
})
@sys.maxLength(20)
@sys.description('I will be overrode.')
param decoratedArray array = [
//@[212:222]     "decoratedArray": {
    utcNow()
//@[215:215]         "[utcNow()]",
    newGuid()
//@[216:216]         "[newGuid()]"
]

