/* 
  This is a block comment.
*/
//@[2:4) NewLine |\n\n|

// parameters without default value
//@[35:36) NewLine |\n|
@sys.description('''
//@[0:1) At |@|
//@[1:4) Identifier |sys|
//@[4:5) Dot |.|
//@[5:16) Identifier |description|
//@[16:17) LeftParen |(|
//@[17:75) MultilineString |'''\nthis is my multi line \ndescription for my myString\n'''|
this is my multi line 
description for my myString
''')
//@[3:4) RightParen |)|
//@[4:5) NewLine |\n|
param myString string
//@[0:5) Identifier |param|
//@[6:14) Identifier |myString|
//@[15:21) Identifier |string|
//@[21:22) NewLine |\n|
param myInt int
//@[0:5) Identifier |param|
//@[6:11) Identifier |myInt|
//@[12:15) Identifier |int|
//@[15:16) NewLine |\n|
param myBool bool
//@[0:5) Identifier |param|
//@[6:12) Identifier |myBool|
//@[13:17) Identifier |bool|
//@[17:19) NewLine |\n\n|

// parameters with default value
//@[32:33) NewLine |\n|
@sys.description('this is myString2')
//@[0:1) At |@|
//@[1:4) Identifier |sys|
//@[4:5) Dot |.|
//@[5:16) Identifier |description|
//@[16:17) LeftParen |(|
//@[17:36) StringComplete |'this is myString2'|
//@[36:37) RightParen |)|
//@[37:38) NewLine |\n|
@metadata({
//@[0:1) At |@|
//@[1:9) Identifier |metadata|
//@[9:10) LeftParen |(|
//@[10:11) LeftBrace |{|
//@[11:12) NewLine |\n|
  description: 'overwrite but still valid'
//@[2:13) Identifier |description|
//@[13:14) Colon |:|
//@[15:42) StringComplete |'overwrite but still valid'|
//@[42:43) NewLine |\n|
})
//@[0:1) RightBrace |}|
//@[1:2) RightParen |)|
//@[2:3) NewLine |\n|
param myString2 string = 'string value'
//@[0:5) Identifier |param|
//@[6:15) Identifier |myString2|
//@[16:22) Identifier |string|
//@[23:24) Assignment |=|
//@[25:39) StringComplete |'string value'|
//@[39:40) NewLine |\n|
param myInt2 int = 42
//@[0:5) Identifier |param|
//@[6:12) Identifier |myInt2|
//@[13:16) Identifier |int|
//@[17:18) Assignment |=|
//@[19:21) Integer |42|
//@[21:22) NewLine |\n|
param myTruth bool = true
//@[0:5) Identifier |param|
//@[6:13) Identifier |myTruth|
//@[14:18) Identifier |bool|
//@[19:20) Assignment |=|
//@[21:25) TrueKeyword |true|
//@[25:26) NewLine |\n|
param myFalsehood bool = false
//@[0:5) Identifier |param|
//@[6:17) Identifier |myFalsehood|
//@[18:22) Identifier |bool|
//@[23:24) Assignment |=|
//@[25:30) FalseKeyword |false|
//@[30:31) NewLine |\n|
param myEscapedString string = 'First line\r\nSecond\ttabbed\tline'
//@[0:5) Identifier |param|
//@[6:21) Identifier |myEscapedString|
//@[22:28) Identifier |string|
//@[29:30) Assignment |=|
//@[31:67) StringComplete |'First line\r\nSecond\ttabbed\tline'|
//@[67:69) NewLine |\n\n|

// object default value
//@[23:24) NewLine |\n|
@sys.description('this is foo')
//@[0:1) At |@|
//@[1:4) Identifier |sys|
//@[4:5) Dot |.|
//@[5:16) Identifier |description|
//@[16:17) LeftParen |(|
//@[17:30) StringComplete |'this is foo'|
//@[30:31) RightParen |)|
//@[31:32) NewLine |\n|
@metadata({
//@[0:1) At |@|
//@[1:9) Identifier |metadata|
//@[9:10) LeftParen |(|
//@[10:11) LeftBrace |{|
//@[11:12) NewLine |\n|
  description: 'overwrite but still valid'
//@[2:13) Identifier |description|
//@[13:14) Colon |:|
//@[15:42) StringComplete |'overwrite but still valid'|
//@[42:43) NewLine |\n|
  another: 'just for fun'
//@[2:9) Identifier |another|
//@[9:10) Colon |:|
//@[11:25) StringComplete |'just for fun'|
//@[25:26) NewLine |\n|
})
//@[0:1) RightBrace |}|
//@[1:2) RightParen |)|
//@[2:3) NewLine |\n|
param foo object = {
//@[0:5) Identifier |param|
//@[6:9) Identifier |foo|
//@[10:16) Identifier |object|
//@[17:18) Assignment |=|
//@[19:20) LeftBrace |{|
//@[20:21) NewLine |\n|
  enabled: true
//@[2:9) Identifier |enabled|
//@[9:10) Colon |:|
//@[11:15) TrueKeyword |true|
//@[15:16) NewLine |\n|
  name: 'this is my object'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:27) StringComplete |'this is my object'|
//@[27:28) NewLine |\n|
  priority: 3
//@[2:10) Identifier |priority|
//@[10:11) Colon |:|
//@[12:13) Integer |3|
//@[13:14) NewLine |\n|
  info: {
//@[2:6) Identifier |info|
//@[6:7) Colon |:|
//@[8:9) LeftBrace |{|
//@[9:10) NewLine |\n|
    a: 'b'
//@[4:5) Identifier |a|
//@[5:6) Colon |:|
//@[7:10) StringComplete |'b'|
//@[10:11) NewLine |\n|
  }
//@[2:3) RightBrace |}|
//@[3:4) NewLine |\n|
  empty: {
//@[2:7) Identifier |empty|
//@[7:8) Colon |:|
//@[9:10) LeftBrace |{|
//@[10:11) NewLine |\n|
  }
//@[2:3) RightBrace |}|
//@[3:4) NewLine |\n|
  array: [
//@[2:7) Identifier |array|
//@[7:8) Colon |:|
//@[9:10) LeftSquare |[|
//@[10:11) NewLine |\n|
    'string item'
//@[4:17) StringComplete |'string item'|
//@[17:18) NewLine |\n|
    12
//@[4:6) Integer |12|
//@[6:7) NewLine |\n|
    true
//@[4:8) TrueKeyword |true|
//@[8:9) NewLine |\n|
    [
//@[4:5) LeftSquare |[|
//@[5:6) NewLine |\n|
      'inner'
//@[6:13) StringComplete |'inner'|
//@[13:14) NewLine |\n|
      false
//@[6:11) FalseKeyword |false|
//@[11:12) NewLine |\n|
    ]
//@[4:5) RightSquare |]|
//@[5:6) NewLine |\n|
    {
//@[4:5) LeftBrace |{|
//@[5:6) NewLine |\n|
      a: 'b'
//@[6:7) Identifier |a|
//@[7:8) Colon |:|
//@[9:12) StringComplete |'b'|
//@[12:13) NewLine |\n|
    }
//@[4:5) RightBrace |}|
//@[5:6) NewLine |\n|
  ]
//@[2:3) RightSquare |]|
//@[3:4) NewLine |\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

// array default value
//@[22:23) NewLine |\n|
param myArrayParam array = [
//@[0:5) Identifier |param|
//@[6:18) Identifier |myArrayParam|
//@[19:24) Identifier |array|
//@[25:26) Assignment |=|
//@[27:28) LeftSquare |[|
//@[28:29) NewLine |\n|
  'a'
//@[2:5) StringComplete |'a'|
//@[5:6) NewLine |\n|
  'b'
//@[2:5) StringComplete |'b'|
//@[5:6) NewLine |\n|
  'c'
//@[2:5) StringComplete |'c'|
//@[5:6) NewLine |\n|
]
//@[0:1) RightSquare |]|
//@[1:3) NewLine |\n\n|

// secure string
//@[16:17) NewLine |\n|
@secure()
//@[0:1) At |@|
//@[1:7) Identifier |secure|
//@[7:8) LeftParen |(|
//@[8:9) RightParen |)|
//@[9:10) NewLine |\n|
param password string
//@[0:5) Identifier |param|
//@[6:14) Identifier |password|
//@[15:21) Identifier |string|
//@[21:23) NewLine |\n\n|

// secure object
//@[16:17) NewLine |\n|
@secure()
//@[0:1) At |@|
//@[1:7) Identifier |secure|
//@[7:8) LeftParen |(|
//@[8:9) RightParen |)|
//@[9:10) NewLine |\n|
param secretObject object
//@[0:5) Identifier |param|
//@[6:18) Identifier |secretObject|
//@[19:25) Identifier |object|
//@[25:27) NewLine |\n\n|

// enum parameter
//@[17:18) NewLine |\n|
@allowed([
//@[0:1) At |@|
//@[1:8) Identifier |allowed|
//@[8:9) LeftParen |(|
//@[9:10) LeftSquare |[|
//@[10:11) NewLine |\n|
  'Standard_LRS'
//@[2:16) StringComplete |'Standard_LRS'|
//@[16:17) NewLine |\n|
  'Standard_GRS'
//@[2:16) StringComplete |'Standard_GRS'|
//@[16:17) NewLine |\n|
])
//@[0:1) RightSquare |]|
//@[1:2) RightParen |)|
//@[2:3) NewLine |\n|
param storageSku string
//@[0:5) Identifier |param|
//@[6:16) Identifier |storageSku|
//@[17:23) Identifier |string|
//@[23:25) NewLine |\n\n|

// length constraint on a string
//@[32:33) NewLine |\n|
@minLength(3)
//@[0:1) At |@|
//@[1:10) Identifier |minLength|
//@[10:11) LeftParen |(|
//@[11:12) Integer |3|
//@[12:13) RightParen |)|
//@[13:14) NewLine |\n|
@maxLength(24)
//@[0:1) At |@|
//@[1:10) Identifier |maxLength|
//@[10:11) LeftParen |(|
//@[11:13) Integer |24|
//@[13:14) RightParen |)|
//@[14:15) NewLine |\n|
param storageName string
//@[0:5) Identifier |param|
//@[6:17) Identifier |storageName|
//@[18:24) Identifier |string|
//@[24:26) NewLine |\n\n|

// length constraint on an array
//@[32:33) NewLine |\n|
@minLength(3)
//@[0:1) At |@|
//@[1:10) Identifier |minLength|
//@[10:11) LeftParen |(|
//@[11:12) Integer |3|
//@[12:13) RightParen |)|
//@[13:14) NewLine |\n|
@maxLength(24)
//@[0:1) At |@|
//@[1:10) Identifier |maxLength|
//@[10:11) LeftParen |(|
//@[11:13) Integer |24|
//@[13:14) RightParen |)|
//@[14:15) NewLine |\n|
param someArray array
//@[0:5) Identifier |param|
//@[6:15) Identifier |someArray|
//@[16:21) Identifier |array|
//@[21:23) NewLine |\n\n|

// empty metadata
//@[17:18) NewLine |\n|
@metadata({})
//@[0:1) At |@|
//@[1:9) Identifier |metadata|
//@[9:10) LeftParen |(|
//@[10:11) LeftBrace |{|
//@[11:12) RightBrace |}|
//@[12:13) RightParen |)|
//@[13:14) NewLine |\n|
param emptyMetadata string
//@[0:5) Identifier |param|
//@[6:19) Identifier |emptyMetadata|
//@[20:26) Identifier |string|
//@[26:28) NewLine |\n\n|

// description
//@[14:15) NewLine |\n|
@metadata({
//@[0:1) At |@|
//@[1:9) Identifier |metadata|
//@[9:10) LeftParen |(|
//@[10:11) LeftBrace |{|
//@[11:12) NewLine |\n|
  description: 'my description'
//@[2:13) Identifier |description|
//@[13:14) Colon |:|
//@[15:31) StringComplete |'my description'|
//@[31:32) NewLine |\n|
})
//@[0:1) RightBrace |}|
//@[1:2) RightParen |)|
//@[2:3) NewLine |\n|
param description string
//@[0:5) Identifier |param|
//@[6:17) Identifier |description|
//@[18:24) Identifier |string|
//@[24:26) NewLine |\n\n|

@sys.description('my description')
//@[0:1) At |@|
//@[1:4) Identifier |sys|
//@[4:5) Dot |.|
//@[5:16) Identifier |description|
//@[16:17) LeftParen |(|
//@[17:33) StringComplete |'my description'|
//@[33:34) RightParen |)|
//@[34:35) NewLine |\n|
param description2 string
//@[0:5) Identifier |param|
//@[6:18) Identifier |description2|
//@[19:25) Identifier |string|
//@[25:27) NewLine |\n\n|

// random extra metadata
//@[24:25) NewLine |\n|
@metadata({
//@[0:1) At |@|
//@[1:9) Identifier |metadata|
//@[9:10) LeftParen |(|
//@[10:11) LeftBrace |{|
//@[11:12) NewLine |\n|
  description: 'my description'
//@[2:13) Identifier |description|
//@[13:14) Colon |:|
//@[15:31) StringComplete |'my description'|
//@[31:32) NewLine |\n|
  a: 1
//@[2:3) Identifier |a|
//@[3:4) Colon |:|
//@[5:6) Integer |1|
//@[6:7) NewLine |\n|
  b: true
//@[2:3) Identifier |b|
//@[3:4) Colon |:|
//@[5:9) TrueKeyword |true|
//@[9:10) NewLine |\n|
  c: [
//@[2:3) Identifier |c|
//@[3:4) Colon |:|
//@[5:6) LeftSquare |[|
//@[6:7) NewLine |\n|
  ]
//@[2:3) RightSquare |]|
//@[3:4) NewLine |\n|
  d: {
//@[2:3) Identifier |d|
//@[3:4) Colon |:|
//@[5:6) LeftBrace |{|
//@[6:7) NewLine |\n|
    test: 'abc'
//@[4:8) Identifier |test|
//@[8:9) Colon |:|
//@[10:15) StringComplete |'abc'|
//@[15:16) NewLine |\n|
  }
//@[2:3) RightBrace |}|
//@[3:4) NewLine |\n|
})
//@[0:1) RightBrace |}|
//@[1:2) RightParen |)|
//@[2:3) NewLine |\n|
param additionalMetadata string
//@[0:5) Identifier |param|
//@[6:24) Identifier |additionalMetadata|
//@[25:31) Identifier |string|
//@[31:33) NewLine |\n\n|

// all modifiers together
//@[25:26) NewLine |\n|
@secure()
//@[0:1) At |@|
//@[1:7) Identifier |secure|
//@[7:8) LeftParen |(|
//@[8:9) RightParen |)|
//@[9:10) NewLine |\n|
@minLength(3)
//@[0:1) At |@|
//@[1:10) Identifier |minLength|
//@[10:11) LeftParen |(|
//@[11:12) Integer |3|
//@[12:13) RightParen |)|
//@[13:14) NewLine |\n|
@maxLength(24)
//@[0:1) At |@|
//@[1:10) Identifier |maxLength|
//@[10:11) LeftParen |(|
//@[11:13) Integer |24|
//@[13:14) RightParen |)|
//@[14:15) NewLine |\n|
@allowed([
//@[0:1) At |@|
//@[1:8) Identifier |allowed|
//@[8:9) LeftParen |(|
//@[9:10) LeftSquare |[|
//@[10:11) NewLine |\n|
  'one'
//@[2:7) StringComplete |'one'|
//@[7:8) NewLine |\n|
  'two'
//@[2:7) StringComplete |'two'|
//@[7:8) NewLine |\n|
  'three'
//@[2:9) StringComplete |'three'|
//@[9:10) NewLine |\n|
])
//@[0:1) RightSquare |]|
//@[1:2) RightParen |)|
//@[2:3) NewLine |\n|
@metadata({
//@[0:1) At |@|
//@[1:9) Identifier |metadata|
//@[9:10) LeftParen |(|
//@[10:11) LeftBrace |{|
//@[11:12) NewLine |\n|
  description: 'Name of the storage account'
//@[2:13) Identifier |description|
//@[13:14) Colon |:|
//@[15:44) StringComplete |'Name of the storage account'|
//@[44:45) NewLine |\n|
})
//@[0:1) RightBrace |}|
//@[1:2) RightParen |)|
//@[2:3) NewLine |\n|
param someParameter string
//@[0:5) Identifier |param|
//@[6:19) Identifier |someParameter|
//@[20:26) Identifier |string|
//@[26:28) NewLine |\n\n|

param defaultExpression bool = 18 != (true || false)
//@[0:5) Identifier |param|
//@[6:23) Identifier |defaultExpression|
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
//@[0:1) At |@|
//@[1:8) Identifier |allowed|
//@[8:9) LeftParen |(|
//@[9:10) LeftSquare |[|
//@[10:11) NewLine |\n|
  'abc'
//@[2:7) StringComplete |'abc'|
//@[7:8) NewLine |\n|
  'def'
//@[2:7) StringComplete |'def'|
//@[7:8) NewLine |\n|
])
//@[0:1) RightSquare |]|
//@[1:2) RightParen |)|
//@[2:3) NewLine |\n|
param stringLiteral string
//@[0:5) Identifier |param|
//@[6:19) Identifier |stringLiteral|
//@[20:26) Identifier |string|
//@[26:28) NewLine |\n\n|

@allowed([
//@[0:1) At |@|
//@[1:8) Identifier |allowed|
//@[8:9) LeftParen |(|
//@[9:10) LeftSquare |[|
//@[10:11) NewLine |\n|
  'abc'
//@[2:7) StringComplete |'abc'|
//@[7:8) NewLine |\n|
  'def'
//@[2:7) StringComplete |'def'|
//@[7:8) NewLine |\n|
  'ghi'
//@[2:7) StringComplete |'ghi'|
//@[7:8) NewLine |\n|
])
//@[0:1) RightSquare |]|
//@[1:2) RightParen |)|
//@[2:3) NewLine |\n|
param stringLiteralWithAllowedValuesSuperset string = stringLiteral
//@[0:5) Identifier |param|
//@[6:44) Identifier |stringLiteralWithAllowedValuesSuperset|
//@[45:51) Identifier |string|
//@[52:53) Assignment |=|
//@[54:67) Identifier |stringLiteral|
//@[67:69) NewLine |\n\n|

@secure()
//@[0:1) At |@|
//@[1:7) Identifier |secure|
//@[7:8) LeftParen |(|
//@[8:9) RightParen |)|
//@[9:10) NewLine |\n|
@minLength(2)
//@[0:1) At |@|
//@[1:10) Identifier |minLength|
//@[10:11) LeftParen |(|
//@[11:12) Integer |2|
//@[12:13) RightParen |)|
//@[13:14) NewLine |\n|
  @maxLength(10)
//@[2:3) At |@|
//@[3:12) Identifier |maxLength|
//@[12:13) LeftParen |(|
//@[13:15) Integer |10|
//@[15:16) RightParen |)|
//@[16:17) NewLine |\n|
@allowed([
//@[0:1) At |@|
//@[1:8) Identifier |allowed|
//@[8:9) LeftParen |(|
//@[9:10) LeftSquare |[|
//@[10:11) NewLine |\n|
  'Apple'
//@[2:9) StringComplete |'Apple'|
//@[9:10) NewLine |\n|
  'Banana'
//@[2:10) StringComplete |'Banana'|
//@[10:11) NewLine |\n|
])
//@[0:1) RightSquare |]|
//@[1:2) RightParen |)|
//@[2:3) NewLine |\n|
param decoratedString string
//@[0:5) Identifier |param|
//@[6:21) Identifier |decoratedString|
//@[22:28) Identifier |string|
//@[28:30) NewLine |\n\n|

@minValue(200)
//@[0:1) At |@|
//@[1:9) Identifier |minValue|
//@[9:10) LeftParen |(|
//@[10:13) Integer |200|
//@[13:14) RightParen |)|
//@[14:15) NewLine |\n|
param decoratedInt int = 123
//@[0:5) Identifier |param|
//@[6:18) Identifier |decoratedInt|
//@[19:22) Identifier |int|
//@[23:24) Assignment |=|
//@[25:28) Integer |123|
//@[28:30) NewLine |\n\n|

// negative integer literals are allowed as decorator values
//@[60:61) NewLine |\n|
@minValue(-10)
//@[0:1) At |@|
//@[1:9) Identifier |minValue|
//@[9:10) LeftParen |(|
//@[10:11) Minus |-|
//@[11:13) Integer |10|
//@[13:14) RightParen |)|
//@[14:15) NewLine |\n|
@maxValue(-3)
//@[0:1) At |@|
//@[1:9) Identifier |maxValue|
//@[9:10) LeftParen |(|
//@[10:11) Minus |-|
//@[11:12) Integer |3|
//@[12:13) RightParen |)|
//@[13:14) NewLine |\n|
param negativeValues int
//@[0:5) Identifier |param|
//@[6:20) Identifier |negativeValues|
//@[21:24) Identifier |int|
//@[24:26) NewLine |\n\n|

@sys.description('A boolean.')
//@[0:1) At |@|
//@[1:4) Identifier |sys|
//@[4:5) Dot |.|
//@[5:16) Identifier |description|
//@[16:17) LeftParen |(|
//@[17:29) StringComplete |'A boolean.'|
//@[29:30) RightParen |)|
//@[30:31) NewLine |\n|
@metadata({
//@[0:1) At |@|
//@[1:9) Identifier |metadata|
//@[9:10) LeftParen |(|
//@[10:11) LeftBrace |{|
//@[11:12) NewLine |\n|
    description: 'I will be overrode.'
//@[4:15) Identifier |description|
//@[15:16) Colon |:|
//@[17:38) StringComplete |'I will be overrode.'|
//@[38:39) NewLine |\n|
    foo: 'something'
//@[4:7) Identifier |foo|
//@[7:8) Colon |:|
//@[9:20) StringComplete |'something'|
//@[20:21) NewLine |\n|
    bar: [
//@[4:7) Identifier |bar|
//@[7:8) Colon |:|
//@[9:10) LeftSquare |[|
//@[10:11) NewLine |\n|
        {          }
//@[8:9) LeftBrace |{|
//@[19:20) RightBrace |}|
//@[20:21) NewLine |\n|
        true
//@[8:12) TrueKeyword |true|
//@[12:13) NewLine |\n|
        123
//@[8:11) Integer |123|
//@[11:12) NewLine |\n|
    ]
//@[4:5) RightSquare |]|
//@[5:6) NewLine |\n|
})
//@[0:1) RightBrace |}|
//@[1:2) RightParen |)|
//@[2:3) NewLine |\n|
param decoratedBool bool = (true && false) != true
//@[0:5) Identifier |param|
//@[6:19) Identifier |decoratedBool|
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
//@[0:1) At |@|
//@[1:7) Identifier |secure|
//@[7:8) LeftParen |(|
//@[8:9) RightParen |)|
//@[9:10) NewLine |\n|
param decoratedObject object = {
//@[0:5) Identifier |param|
//@[6:21) Identifier |decoratedObject|
//@[22:28) Identifier |object|
//@[29:30) Assignment |=|
//@[31:32) LeftBrace |{|
//@[32:33) NewLine |\n|
  enabled: true
//@[2:9) Identifier |enabled|
//@[9:10) Colon |:|
//@[11:15) TrueKeyword |true|
//@[15:16) NewLine |\n|
  name: 'this is my object'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:27) StringComplete |'this is my object'|
//@[27:28) NewLine |\n|
  priority: 3
//@[2:10) Identifier |priority|
//@[10:11) Colon |:|
//@[12:13) Integer |3|
//@[13:14) NewLine |\n|
  info: {
//@[2:6) Identifier |info|
//@[6:7) Colon |:|
//@[8:9) LeftBrace |{|
//@[9:10) NewLine |\n|
    a: 'b'
//@[4:5) Identifier |a|
//@[5:6) Colon |:|
//@[7:10) StringComplete |'b'|
//@[10:11) NewLine |\n|
  }
//@[2:3) RightBrace |}|
//@[3:4) NewLine |\n|
  empty: {
//@[2:7) Identifier |empty|
//@[7:8) Colon |:|
//@[9:10) LeftBrace |{|
//@[10:11) NewLine |\n|
  }
//@[2:3) RightBrace |}|
//@[3:4) NewLine |\n|
  array: [
//@[2:7) Identifier |array|
//@[7:8) Colon |:|
//@[9:10) LeftSquare |[|
//@[10:11) NewLine |\n|
    'string item'
//@[4:17) StringComplete |'string item'|
//@[17:18) NewLine |\n|
    12
//@[4:6) Integer |12|
//@[6:7) NewLine |\n|
    true
//@[4:8) TrueKeyword |true|
//@[8:9) NewLine |\n|
    [
//@[4:5) LeftSquare |[|
//@[5:6) NewLine |\n|
      'inner'
//@[6:13) StringComplete |'inner'|
//@[13:14) NewLine |\n|
      false
//@[6:11) FalseKeyword |false|
//@[11:12) NewLine |\n|
    ]
//@[4:5) RightSquare |]|
//@[5:6) NewLine |\n|
    {
//@[4:5) LeftBrace |{|
//@[5:6) NewLine |\n|
      a: 'b'
//@[6:7) Identifier |a|
//@[7:8) Colon |:|
//@[9:12) StringComplete |'b'|
//@[12:13) NewLine |\n|
    }
//@[4:5) RightBrace |}|
//@[5:6) NewLine |\n|
  ]
//@[2:3) RightSquare |]|
//@[3:4) NewLine |\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

@sys.metadata({
//@[0:1) At |@|
//@[1:4) Identifier |sys|
//@[4:5) Dot |.|
//@[5:13) Identifier |metadata|
//@[13:14) LeftParen |(|
//@[14:15) LeftBrace |{|
//@[15:16) NewLine |\n|
    description: 'An array.'
//@[4:15) Identifier |description|
//@[15:16) Colon |:|
//@[17:28) StringComplete |'An array.'|
//@[28:29) NewLine |\n|
})
//@[0:1) RightBrace |}|
//@[1:2) RightParen |)|
//@[2:3) NewLine |\n|
@sys.maxLength(20)
//@[0:1) At |@|
//@[1:4) Identifier |sys|
//@[4:5) Dot |.|
//@[5:14) Identifier |maxLength|
//@[14:15) LeftParen |(|
//@[15:17) Integer |20|
//@[17:18) RightParen |)|
//@[18:19) NewLine |\n|
@sys.description('I will be overrode.')
//@[0:1) At |@|
//@[1:4) Identifier |sys|
//@[4:5) Dot |.|
//@[5:16) Identifier |description|
//@[16:17) LeftParen |(|
//@[17:38) StringComplete |'I will be overrode.'|
//@[38:39) RightParen |)|
//@[39:40) NewLine |\n|
param decoratedArray array = [
//@[0:5) Identifier |param|
//@[6:20) Identifier |decoratedArray|
//@[21:26) Identifier |array|
//@[27:28) Assignment |=|
//@[29:30) LeftSquare |[|
//@[30:31) NewLine |\n|
    utcNow()
//@[4:10) Identifier |utcNow|
//@[10:11) LeftParen |(|
//@[11:12) RightParen |)|
//@[12:13) NewLine |\n|
    newGuid()
//@[4:11) Identifier |newGuid|
//@[11:12) LeftParen |(|
//@[12:13) RightParen |)|
//@[13:14) NewLine |\n|
]
//@[0:1) RightSquare |]|
//@[1:2) NewLine |\n|

//@[0:0) EndOfFile ||
