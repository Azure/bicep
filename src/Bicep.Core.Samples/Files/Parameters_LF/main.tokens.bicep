/*
  This is a block comment.
*/
//@[02:04) NewLine |\n\n|

// parameters without default value
//@[35:36) NewLine |\n|
@sys.description('''
//@[00:01) At |@|
//@[01:04) Identifier |sys|
//@[04:05) Dot |.|
//@[05:16) Identifier |description|
//@[16:17) LeftParen |(|
//@[17:74) MultilineString |'''\nthis is my multi line\ndescription for my myString\n'''|
this is my multi line
description for my myString
''')
//@[03:04) RightParen |)|
//@[04:05) NewLine |\n|
param myString string
//@[00:05) Identifier |param|
//@[06:14) Identifier |myString|
//@[15:21) Identifier |string|
//@[21:22) NewLine |\n|
param myInt int
//@[00:05) Identifier |param|
//@[06:11) Identifier |myInt|
//@[12:15) Identifier |int|
//@[15:16) NewLine |\n|
param myBool bool
//@[00:05) Identifier |param|
//@[06:12) Identifier |myBool|
//@[13:17) Identifier |bool|
//@[17:19) NewLine |\n\n|

// parameters with default value
//@[32:33) NewLine |\n|
@sys.description('this is myString2')
//@[00:01) At |@|
//@[01:04) Identifier |sys|
//@[04:05) Dot |.|
//@[05:16) Identifier |description|
//@[16:17) LeftParen |(|
//@[17:36) StringComplete |'this is myString2'|
//@[36:37) RightParen |)|
//@[37:38) NewLine |\n|
@metadata({
//@[00:01) At |@|
//@[01:09) Identifier |metadata|
//@[09:10) LeftParen |(|
//@[10:11) LeftBrace |{|
//@[11:12) NewLine |\n|
  description: 'overwrite but still valid'
//@[02:13) Identifier |description|
//@[13:14) Colon |:|
//@[15:42) StringComplete |'overwrite but still valid'|
//@[42:43) NewLine |\n|
})
//@[00:01) RightBrace |}|
//@[01:02) RightParen |)|
//@[02:03) NewLine |\n|
param myString2 string = 'string value'
//@[00:05) Identifier |param|
//@[06:15) Identifier |myString2|
//@[16:22) Identifier |string|
//@[23:24) Assignment |=|
//@[25:39) StringComplete |'string value'|
//@[39:40) NewLine |\n|
param myInt2 int = 42
//@[00:05) Identifier |param|
//@[06:12) Identifier |myInt2|
//@[13:16) Identifier |int|
//@[17:18) Assignment |=|
//@[19:21) Integer |42|
//@[21:22) NewLine |\n|
param myTruth bool = true
//@[00:05) Identifier |param|
//@[06:13) Identifier |myTruth|
//@[14:18) Identifier |bool|
//@[19:20) Assignment |=|
//@[21:25) TrueKeyword |true|
//@[25:26) NewLine |\n|
param myFalsehood bool = false
//@[00:05) Identifier |param|
//@[06:17) Identifier |myFalsehood|
//@[18:22) Identifier |bool|
//@[23:24) Assignment |=|
//@[25:30) FalseKeyword |false|
//@[30:31) NewLine |\n|
param myEscapedString string = 'First line\r\nSecond\ttabbed\tline'
//@[00:05) Identifier |param|
//@[06:21) Identifier |myEscapedString|
//@[22:28) Identifier |string|
//@[29:30) Assignment |=|
//@[31:67) StringComplete |'First line\r\nSecond\ttabbed\tline'|
//@[67:69) NewLine |\n\n|

// object default value
//@[23:24) NewLine |\n|
@sys.description('this is foo')
//@[00:01) At |@|
//@[01:04) Identifier |sys|
//@[04:05) Dot |.|
//@[05:16) Identifier |description|
//@[16:17) LeftParen |(|
//@[17:30) StringComplete |'this is foo'|
//@[30:31) RightParen |)|
//@[31:32) NewLine |\n|
@metadata({
//@[00:01) At |@|
//@[01:09) Identifier |metadata|
//@[09:10) LeftParen |(|
//@[10:11) LeftBrace |{|
//@[11:12) NewLine |\n|
  description: 'overwrite but still valid'
//@[02:13) Identifier |description|
//@[13:14) Colon |:|
//@[15:42) StringComplete |'overwrite but still valid'|
//@[42:43) NewLine |\n|
  another: 'just for fun'
//@[02:09) Identifier |another|
//@[09:10) Colon |:|
//@[11:25) StringComplete |'just for fun'|
//@[25:26) NewLine |\n|
})
//@[00:01) RightBrace |}|
//@[01:02) RightParen |)|
//@[02:03) NewLine |\n|
param foo object = {
//@[00:05) Identifier |param|
//@[06:09) Identifier |foo|
//@[10:16) Identifier |object|
//@[17:18) Assignment |=|
//@[19:20) LeftBrace |{|
//@[20:21) NewLine |\n|
  enabled: true
//@[02:09) Identifier |enabled|
//@[09:10) Colon |:|
//@[11:15) TrueKeyword |true|
//@[15:16) NewLine |\n|
  name: 'this is my object'
//@[02:06) Identifier |name|
//@[06:07) Colon |:|
//@[08:27) StringComplete |'this is my object'|
//@[27:28) NewLine |\n|
  priority: 3
//@[02:10) Identifier |priority|
//@[10:11) Colon |:|
//@[12:13) Integer |3|
//@[13:14) NewLine |\n|
  info: {
//@[02:06) Identifier |info|
//@[06:07) Colon |:|
//@[08:09) LeftBrace |{|
//@[09:10) NewLine |\n|
    a: 'b'
//@[04:05) Identifier |a|
//@[05:06) Colon |:|
//@[07:10) StringComplete |'b'|
//@[10:11) NewLine |\n|
  }
//@[02:03) RightBrace |}|
//@[03:04) NewLine |\n|
  empty: {
//@[02:07) Identifier |empty|
//@[07:08) Colon |:|
//@[09:10) LeftBrace |{|
//@[10:11) NewLine |\n|
  }
//@[02:03) RightBrace |}|
//@[03:04) NewLine |\n|
  array: [
//@[02:07) Identifier |array|
//@[07:08) Colon |:|
//@[09:10) LeftSquare |[|
//@[10:11) NewLine |\n|
    'string item'
//@[04:17) StringComplete |'string item'|
//@[17:18) NewLine |\n|
    12
//@[04:06) Integer |12|
//@[06:07) NewLine |\n|
    true
//@[04:08) TrueKeyword |true|
//@[08:09) NewLine |\n|
    [
//@[04:05) LeftSquare |[|
//@[05:06) NewLine |\n|
      'inner'
//@[06:13) StringComplete |'inner'|
//@[13:14) NewLine |\n|
      false
//@[06:11) FalseKeyword |false|
//@[11:12) NewLine |\n|
    ]
//@[04:05) RightSquare |]|
//@[05:06) NewLine |\n|
    {
//@[04:05) LeftBrace |{|
//@[05:06) NewLine |\n|
      a: 'b'
//@[06:07) Identifier |a|
//@[07:08) Colon |:|
//@[09:12) StringComplete |'b'|
//@[12:13) NewLine |\n|
    }
//@[04:05) RightBrace |}|
//@[05:06) NewLine |\n|
  ]
//@[02:03) RightSquare |]|
//@[03:04) NewLine |\n|
}
//@[00:01) RightBrace |}|
//@[01:03) NewLine |\n\n|

// array default value
//@[22:23) NewLine |\n|
param myArrayParam array = [
//@[00:05) Identifier |param|
//@[06:18) Identifier |myArrayParam|
//@[19:24) Identifier |array|
//@[25:26) Assignment |=|
//@[27:28) LeftSquare |[|
//@[28:29) NewLine |\n|
  'a'
//@[02:05) StringComplete |'a'|
//@[05:06) NewLine |\n|
  'b'
//@[02:05) StringComplete |'b'|
//@[05:06) NewLine |\n|
  'c'
//@[02:05) StringComplete |'c'|
//@[05:06) NewLine |\n|
]
//@[00:01) RightSquare |]|
//@[01:03) NewLine |\n\n|

// secure string
//@[16:17) NewLine |\n|
@secure()
//@[00:01) At |@|
//@[01:07) Identifier |secure|
//@[07:08) LeftParen |(|
//@[08:09) RightParen |)|
//@[09:10) NewLine |\n|
param password string
//@[00:05) Identifier |param|
//@[06:14) Identifier |password|
//@[15:21) Identifier |string|
//@[21:23) NewLine |\n\n|

// secure object
//@[16:17) NewLine |\n|
@secure()
//@[00:01) At |@|
//@[01:07) Identifier |secure|
//@[07:08) LeftParen |(|
//@[08:09) RightParen |)|
//@[09:10) NewLine |\n|
param secretObject object
//@[00:05) Identifier |param|
//@[06:18) Identifier |secretObject|
//@[19:25) Identifier |object|
//@[25:27) NewLine |\n\n|

// enum parameter
//@[17:18) NewLine |\n|
@allowed([
//@[00:01) At |@|
//@[01:08) Identifier |allowed|
//@[08:09) LeftParen |(|
//@[09:10) LeftSquare |[|
//@[10:11) NewLine |\n|
  'Standard_LRS'
//@[02:16) StringComplete |'Standard_LRS'|
//@[16:17) NewLine |\n|
  'Standard_GRS'
//@[02:16) StringComplete |'Standard_GRS'|
//@[16:17) NewLine |\n|
])
//@[00:01) RightSquare |]|
//@[01:02) RightParen |)|
//@[02:03) NewLine |\n|
param storageSku string
//@[00:05) Identifier |param|
//@[06:16) Identifier |storageSku|
//@[17:23) Identifier |string|
//@[23:25) NewLine |\n\n|

@allowed([
//@[00:01) At |@|
//@[01:08) Identifier |allowed|
//@[08:09) LeftParen |(|
//@[09:10) LeftSquare |[|
//@[10:11) NewLine |\n|
  1
//@[02:03) Integer |1|
//@[03:04) NewLine |\n|
  2
//@[02:03) Integer |2|
//@[03:04) NewLine |\n|
  3
//@[02:03) Integer |3|
//@[03:04) NewLine |\n|
])
//@[00:01) RightSquare |]|
//@[01:02) RightParen |)|
//@[02:03) NewLine |\n|
param intEnum int
//@[00:05) Identifier |param|
//@[06:13) Identifier |intEnum|
//@[14:17) Identifier |int|
//@[17:19) NewLine |\n\n|

// length constraint on a string
//@[32:33) NewLine |\n|
@minLength(3)
//@[00:01) At |@|
//@[01:10) Identifier |minLength|
//@[10:11) LeftParen |(|
//@[11:12) Integer |3|
//@[12:13) RightParen |)|
//@[13:14) NewLine |\n|
@maxLength(24)
//@[00:01) At |@|
//@[01:10) Identifier |maxLength|
//@[10:11) LeftParen |(|
//@[11:13) Integer |24|
//@[13:14) RightParen |)|
//@[14:15) NewLine |\n|
param storageName string
//@[00:05) Identifier |param|
//@[06:17) Identifier |storageName|
//@[18:24) Identifier |string|
//@[24:26) NewLine |\n\n|

// length constraint on an array
//@[32:33) NewLine |\n|
@minLength(3)
//@[00:01) At |@|
//@[01:10) Identifier |minLength|
//@[10:11) LeftParen |(|
//@[11:12) Integer |3|
//@[12:13) RightParen |)|
//@[13:14) NewLine |\n|
@maxLength(24)
//@[00:01) At |@|
//@[01:10) Identifier |maxLength|
//@[10:11) LeftParen |(|
//@[11:13) Integer |24|
//@[13:14) RightParen |)|
//@[14:15) NewLine |\n|
param someArray array
//@[00:05) Identifier |param|
//@[06:15) Identifier |someArray|
//@[16:21) Identifier |array|
//@[21:23) NewLine |\n\n|

// empty metadata
//@[17:18) NewLine |\n|
@metadata({})
//@[00:01) At |@|
//@[01:09) Identifier |metadata|
//@[09:10) LeftParen |(|
//@[10:11) LeftBrace |{|
//@[11:12) RightBrace |}|
//@[12:13) RightParen |)|
//@[13:14) NewLine |\n|
param emptyMetadata string
//@[00:05) Identifier |param|
//@[06:19) Identifier |emptyMetadata|
//@[20:26) Identifier |string|
//@[26:28) NewLine |\n\n|

// description
//@[14:15) NewLine |\n|
@metadata({
//@[00:01) At |@|
//@[01:09) Identifier |metadata|
//@[09:10) LeftParen |(|
//@[10:11) LeftBrace |{|
//@[11:12) NewLine |\n|
  description: 'my description'
//@[02:13) Identifier |description|
//@[13:14) Colon |:|
//@[15:31) StringComplete |'my description'|
//@[31:32) NewLine |\n|
})
//@[00:01) RightBrace |}|
//@[01:02) RightParen |)|
//@[02:03) NewLine |\n|
param description string
//@[00:05) Identifier |param|
//@[06:17) Identifier |description|
//@[18:24) Identifier |string|
//@[24:26) NewLine |\n\n|

@sys.description('my description')
//@[00:01) At |@|
//@[01:04) Identifier |sys|
//@[04:05) Dot |.|
//@[05:16) Identifier |description|
//@[16:17) LeftParen |(|
//@[17:33) StringComplete |'my description'|
//@[33:34) RightParen |)|
//@[34:35) NewLine |\n|
param description2 string
//@[00:05) Identifier |param|
//@[06:18) Identifier |description2|
//@[19:25) Identifier |string|
//@[25:27) NewLine |\n\n|

// random extra metadata
//@[24:25) NewLine |\n|
@metadata({
//@[00:01) At |@|
//@[01:09) Identifier |metadata|
//@[09:10) LeftParen |(|
//@[10:11) LeftBrace |{|
//@[11:12) NewLine |\n|
  description: 'my description'
//@[02:13) Identifier |description|
//@[13:14) Colon |:|
//@[15:31) StringComplete |'my description'|
//@[31:32) NewLine |\n|
  a: 1
//@[02:03) Identifier |a|
//@[03:04) Colon |:|
//@[05:06) Integer |1|
//@[06:07) NewLine |\n|
  b: true
//@[02:03) Identifier |b|
//@[03:04) Colon |:|
//@[05:09) TrueKeyword |true|
//@[09:10) NewLine |\n|
  c: [
//@[02:03) Identifier |c|
//@[03:04) Colon |:|
//@[05:06) LeftSquare |[|
//@[06:07) NewLine |\n|
  ]
//@[02:03) RightSquare |]|
//@[03:04) NewLine |\n|
  d: {
//@[02:03) Identifier |d|
//@[03:04) Colon |:|
//@[05:06) LeftBrace |{|
//@[06:07) NewLine |\n|
    test: 'abc'
//@[04:08) Identifier |test|
//@[08:09) Colon |:|
//@[10:15) StringComplete |'abc'|
//@[15:16) NewLine |\n|
  }
//@[02:03) RightBrace |}|
//@[03:04) NewLine |\n|
})
//@[00:01) RightBrace |}|
//@[01:02) RightParen |)|
//@[02:03) NewLine |\n|
param additionalMetadata string
//@[00:05) Identifier |param|
//@[06:24) Identifier |additionalMetadata|
//@[25:31) Identifier |string|
//@[31:33) NewLine |\n\n|

// all modifiers together
//@[25:26) NewLine |\n|
@secure()
//@[00:01) At |@|
//@[01:07) Identifier |secure|
//@[07:08) LeftParen |(|
//@[08:09) RightParen |)|
//@[09:10) NewLine |\n|
@minLength(3)
//@[00:01) At |@|
//@[01:10) Identifier |minLength|
//@[10:11) LeftParen |(|
//@[11:12) Integer |3|
//@[12:13) RightParen |)|
//@[13:14) NewLine |\n|
@maxLength(24)
//@[00:01) At |@|
//@[01:10) Identifier |maxLength|
//@[10:11) LeftParen |(|
//@[11:13) Integer |24|
//@[13:14) RightParen |)|
//@[14:15) NewLine |\n|
@allowed([
//@[00:01) At |@|
//@[01:08) Identifier |allowed|
//@[08:09) LeftParen |(|
//@[09:10) LeftSquare |[|
//@[10:11) NewLine |\n|
  'one'
//@[02:07) StringComplete |'one'|
//@[07:08) NewLine |\n|
  'two'
//@[02:07) StringComplete |'two'|
//@[07:08) NewLine |\n|
  'three'
//@[02:09) StringComplete |'three'|
//@[09:10) NewLine |\n|
])
//@[00:01) RightSquare |]|
//@[01:02) RightParen |)|
//@[02:03) NewLine |\n|
@metadata({
//@[00:01) At |@|
//@[01:09) Identifier |metadata|
//@[09:10) LeftParen |(|
//@[10:11) LeftBrace |{|
//@[11:12) NewLine |\n|
  description: 'Name of the storage account'
//@[02:13) Identifier |description|
//@[13:14) Colon |:|
//@[15:44) StringComplete |'Name of the storage account'|
//@[44:45) NewLine |\n|
})
//@[00:01) RightBrace |}|
//@[01:02) RightParen |)|
//@[02:03) NewLine |\n|
param someParameter string
//@[00:05) Identifier |param|
//@[06:19) Identifier |someParameter|
//@[20:26) Identifier |string|
//@[26:28) NewLine |\n\n|

param defaultExpression bool = 18 != (true || false)
//@[00:05) Identifier |param|
//@[06:23) Identifier |defaultExpression|
//@[24:28) Identifier |bool|
//@[29:30) Assignment |=|
//@[31:33) Integer |18|
//@[34:36) NotEquals |!=|
//@[37:38) LeftParen |(|
//@[38:42) TrueKeyword |true|
//@[43:45) LogicalOr ||||
//@[46:51) FalseKeyword |false|
//@[51:52) RightParen |)|
//@[52:54) NewLine |\n\n|

@allowed([
//@[00:01) At |@|
//@[01:08) Identifier |allowed|
//@[08:09) LeftParen |(|
//@[09:10) LeftSquare |[|
//@[10:11) NewLine |\n|
  'abc'
//@[02:07) StringComplete |'abc'|
//@[07:08) NewLine |\n|
  'def'
//@[02:07) StringComplete |'def'|
//@[07:08) NewLine |\n|
])
//@[00:01) RightSquare |]|
//@[01:02) RightParen |)|
//@[02:03) NewLine |\n|
param stringLiteral string
//@[00:05) Identifier |param|
//@[06:19) Identifier |stringLiteral|
//@[20:26) Identifier |string|
//@[26:28) NewLine |\n\n|

@allowed([
//@[00:01) At |@|
//@[01:08) Identifier |allowed|
//@[08:09) LeftParen |(|
//@[09:10) LeftSquare |[|
//@[10:11) NewLine |\n|
  'abc'
//@[02:07) StringComplete |'abc'|
//@[07:08) NewLine |\n|
  'def'
//@[02:07) StringComplete |'def'|
//@[07:08) NewLine |\n|
  'ghi'
//@[02:07) StringComplete |'ghi'|
//@[07:08) NewLine |\n|
])
//@[00:01) RightSquare |]|
//@[01:02) RightParen |)|
//@[02:03) NewLine |\n|
param stringLiteralWithAllowedValuesSuperset string = stringLiteral
//@[00:05) Identifier |param|
//@[06:44) Identifier |stringLiteralWithAllowedValuesSuperset|
//@[45:51) Identifier |string|
//@[52:53) Assignment |=|
//@[54:67) Identifier |stringLiteral|
//@[67:69) NewLine |\n\n|

@secure()
//@[00:01) At |@|
//@[01:07) Identifier |secure|
//@[07:08) LeftParen |(|
//@[08:09) RightParen |)|
//@[09:10) NewLine |\n|
@minLength(2)
//@[00:01) At |@|
//@[01:10) Identifier |minLength|
//@[10:11) LeftParen |(|
//@[11:12) Integer |2|
//@[12:13) RightParen |)|
//@[13:14) NewLine |\n|
  @maxLength(10)
//@[02:03) At |@|
//@[03:12) Identifier |maxLength|
//@[12:13) LeftParen |(|
//@[13:15) Integer |10|
//@[15:16) RightParen |)|
//@[16:17) NewLine |\n|
@allowed([
//@[00:01) At |@|
//@[01:08) Identifier |allowed|
//@[08:09) LeftParen |(|
//@[09:10) LeftSquare |[|
//@[10:11) NewLine |\n|
  'Apple'
//@[02:09) StringComplete |'Apple'|
//@[09:10) NewLine |\n|
  'Banana'
//@[02:10) StringComplete |'Banana'|
//@[10:11) NewLine |\n|
])
//@[00:01) RightSquare |]|
//@[01:02) RightParen |)|
//@[02:03) NewLine |\n|
param decoratedString string
//@[00:05) Identifier |param|
//@[06:21) Identifier |decoratedString|
//@[22:28) Identifier |string|
//@[28:30) NewLine |\n\n|

@minValue(200)
//@[00:01) At |@|
//@[01:09) Identifier |minValue|
//@[09:10) LeftParen |(|
//@[10:13) Integer |200|
//@[13:14) RightParen |)|
//@[14:15) NewLine |\n|
param decoratedInt int = 123
//@[00:05) Identifier |param|
//@[06:18) Identifier |decoratedInt|
//@[19:22) Identifier |int|
//@[23:24) Assignment |=|
//@[25:28) Integer |123|
//@[28:30) NewLine |\n\n|

// negative integer literals are allowed as decorator values
//@[60:61) NewLine |\n|
@minValue(-10)
//@[00:01) At |@|
//@[01:09) Identifier |minValue|
//@[09:10) LeftParen |(|
//@[10:11) Minus |-|
//@[11:13) Integer |10|
//@[13:14) RightParen |)|
//@[14:15) NewLine |\n|
@maxValue(-3)
//@[00:01) At |@|
//@[01:09) Identifier |maxValue|
//@[09:10) LeftParen |(|
//@[10:11) Minus |-|
//@[11:12) Integer |3|
//@[12:13) RightParen |)|
//@[13:14) NewLine |\n|
param negativeValues int
//@[00:05) Identifier |param|
//@[06:20) Identifier |negativeValues|
//@[21:24) Identifier |int|
//@[24:26) NewLine |\n\n|

@sys.description('A boolean.')
//@[00:01) At |@|
//@[01:04) Identifier |sys|
//@[04:05) Dot |.|
//@[05:16) Identifier |description|
//@[16:17) LeftParen |(|
//@[17:29) StringComplete |'A boolean.'|
//@[29:30) RightParen |)|
//@[30:31) NewLine |\n|
@metadata({
//@[00:01) At |@|
//@[01:09) Identifier |metadata|
//@[09:10) LeftParen |(|
//@[10:11) LeftBrace |{|
//@[11:12) NewLine |\n|
    description: 'I will be overrode.'
//@[04:15) Identifier |description|
//@[15:16) Colon |:|
//@[17:38) StringComplete |'I will be overrode.'|
//@[38:39) NewLine |\n|
    foo: 'something'
//@[04:07) Identifier |foo|
//@[07:08) Colon |:|
//@[09:20) StringComplete |'something'|
//@[20:21) NewLine |\n|
    bar: [
//@[04:07) Identifier |bar|
//@[07:08) Colon |:|
//@[09:10) LeftSquare |[|
//@[10:11) NewLine |\n|
        {          }
//@[08:09) LeftBrace |{|
//@[19:20) RightBrace |}|
//@[20:21) NewLine |\n|
        true
//@[08:12) TrueKeyword |true|
//@[12:13) NewLine |\n|
        123
//@[08:11) Integer |123|
//@[11:12) NewLine |\n|
    ]
//@[04:05) RightSquare |]|
//@[05:06) NewLine |\n|
})
//@[00:01) RightBrace |}|
//@[01:02) RightParen |)|
//@[02:03) NewLine |\n|
param decoratedBool bool = (true && false) != true
//@[00:05) Identifier |param|
//@[06:19) Identifier |decoratedBool|
//@[20:24) Identifier |bool|
//@[25:26) Assignment |=|
//@[27:28) LeftParen |(|
//@[28:32) TrueKeyword |true|
//@[33:35) LogicalAnd |&&|
//@[36:41) FalseKeyword |false|
//@[41:42) RightParen |)|
//@[43:45) NotEquals |!=|
//@[46:50) TrueKeyword |true|
//@[50:52) NewLine |\n\n|

@secure()
//@[00:01) At |@|
//@[01:07) Identifier |secure|
//@[07:08) LeftParen |(|
//@[08:09) RightParen |)|
//@[09:10) NewLine |\n|
param decoratedObject object = {
//@[00:05) Identifier |param|
//@[06:21) Identifier |decoratedObject|
//@[22:28) Identifier |object|
//@[29:30) Assignment |=|
//@[31:32) LeftBrace |{|
//@[32:33) NewLine |\n|
  enabled: true
//@[02:09) Identifier |enabled|
//@[09:10) Colon |:|
//@[11:15) TrueKeyword |true|
//@[15:16) NewLine |\n|
  name: 'this is my object'
//@[02:06) Identifier |name|
//@[06:07) Colon |:|
//@[08:27) StringComplete |'this is my object'|
//@[27:28) NewLine |\n|
  priority: 3
//@[02:10) Identifier |priority|
//@[10:11) Colon |:|
//@[12:13) Integer |3|
//@[13:14) NewLine |\n|
  info: {
//@[02:06) Identifier |info|
//@[06:07) Colon |:|
//@[08:09) LeftBrace |{|
//@[09:10) NewLine |\n|
    a: 'b'
//@[04:05) Identifier |a|
//@[05:06) Colon |:|
//@[07:10) StringComplete |'b'|
//@[10:11) NewLine |\n|
  }
//@[02:03) RightBrace |}|
//@[03:04) NewLine |\n|
  empty: {
//@[02:07) Identifier |empty|
//@[07:08) Colon |:|
//@[09:10) LeftBrace |{|
//@[10:11) NewLine |\n|
  }
//@[02:03) RightBrace |}|
//@[03:04) NewLine |\n|
  array: [
//@[02:07) Identifier |array|
//@[07:08) Colon |:|
//@[09:10) LeftSquare |[|
//@[10:11) NewLine |\n|
    'string item'
//@[04:17) StringComplete |'string item'|
//@[17:18) NewLine |\n|
    12
//@[04:06) Integer |12|
//@[06:07) NewLine |\n|
    true
//@[04:08) TrueKeyword |true|
//@[08:09) NewLine |\n|
    [
//@[04:05) LeftSquare |[|
//@[05:06) NewLine |\n|
      'inner'
//@[06:13) StringComplete |'inner'|
//@[13:14) NewLine |\n|
      false
//@[06:11) FalseKeyword |false|
//@[11:12) NewLine |\n|
    ]
//@[04:05) RightSquare |]|
//@[05:06) NewLine |\n|
    {
//@[04:05) LeftBrace |{|
//@[05:06) NewLine |\n|
      a: 'b'
//@[06:07) Identifier |a|
//@[07:08) Colon |:|
//@[09:12) StringComplete |'b'|
//@[12:13) NewLine |\n|
    }
//@[04:05) RightBrace |}|
//@[05:06) NewLine |\n|
  ]
//@[02:03) RightSquare |]|
//@[03:04) NewLine |\n|
}
//@[00:01) RightBrace |}|
//@[01:03) NewLine |\n\n|

@sys.metadata({
//@[00:01) At |@|
//@[01:04) Identifier |sys|
//@[04:05) Dot |.|
//@[05:13) Identifier |metadata|
//@[13:14) LeftParen |(|
//@[14:15) LeftBrace |{|
//@[15:16) NewLine |\n|
    description: 'An array.'
//@[04:15) Identifier |description|
//@[15:16) Colon |:|
//@[17:28) StringComplete |'An array.'|
//@[28:29) NewLine |\n|
})
//@[00:01) RightBrace |}|
//@[01:02) RightParen |)|
//@[02:03) NewLine |\n|
@sys.maxLength(20)
//@[00:01) At |@|
//@[01:04) Identifier |sys|
//@[04:05) Dot |.|
//@[05:14) Identifier |maxLength|
//@[14:15) LeftParen |(|
//@[15:17) Integer |20|
//@[17:18) RightParen |)|
//@[18:19) NewLine |\n|
@sys.description('I will be overrode.')
//@[00:01) At |@|
//@[01:04) Identifier |sys|
//@[04:05) Dot |.|
//@[05:16) Identifier |description|
//@[16:17) LeftParen |(|
//@[17:38) StringComplete |'I will be overrode.'|
//@[38:39) RightParen |)|
//@[39:40) NewLine |\n|
param decoratedArray array = [
//@[00:05) Identifier |param|
//@[06:20) Identifier |decoratedArray|
//@[21:26) Identifier |array|
//@[27:28) Assignment |=|
//@[29:30) LeftSquare |[|
//@[30:31) NewLine |\n|
    utcNow()
//@[04:10) Identifier |utcNow|
//@[10:11) LeftParen |(|
//@[11:12) RightParen |)|
//@[12:13) NewLine |\n|
    newGuid()
//@[04:11) Identifier |newGuid|
//@[11:12) LeftParen |(|
//@[12:13) RightParen |)|
//@[13:14) NewLine |\n|
]
//@[00:01) RightSquare |]|
//@[01:02) NewLine |\n|

//@[00:00) EndOfFile ||
