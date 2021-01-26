/* 
  This is a block comment.
*/
//@[2:6) NewLine |\r\n\r\n|

// parameters without default value
//@[35:37) NewLine |\r\n|
param myString string
//@[0:5) Identifier |param|
//@[6:14) Identifier |myString|
//@[15:21) Identifier |string|
//@[21:23) NewLine |\r\n|
param myInt int
//@[0:5) Identifier |param|
//@[6:11) Identifier |myInt|
//@[12:15) Identifier |int|
//@[15:17) NewLine |\r\n|
param myBool bool
//@[0:5) Identifier |param|
//@[6:12) Identifier |myBool|
//@[13:17) Identifier |bool|
//@[17:21) NewLine |\r\n\r\n|

// parameters with default value
//@[32:34) NewLine |\r\n|
param myString2 string = 'string value'
//@[0:5) Identifier |param|
//@[6:15) Identifier |myString2|
//@[16:22) Identifier |string|
//@[23:24) Assignment |=|
//@[25:39) StringComplete |'string value'|
//@[39:41) NewLine |\r\n|
param myInt2 int = 42
//@[0:5) Identifier |param|
//@[6:12) Identifier |myInt2|
//@[13:16) Identifier |int|
//@[17:18) Assignment |=|
//@[19:21) Number |42|
//@[21:23) NewLine |\r\n|
param myTruth bool = true
//@[0:5) Identifier |param|
//@[6:13) Identifier |myTruth|
//@[14:18) Identifier |bool|
//@[19:20) Assignment |=|
//@[21:25) TrueKeyword |true|
//@[25:27) NewLine |\r\n|
param myFalsehood bool = false
//@[0:5) Identifier |param|
//@[6:17) Identifier |myFalsehood|
//@[18:22) Identifier |bool|
//@[23:24) Assignment |=|
//@[25:30) FalseKeyword |false|
//@[30:32) NewLine |\r\n|
param myEscapedString string = 'First line\r\nSecond\ttabbed\tline'
//@[0:5) Identifier |param|
//@[6:21) Identifier |myEscapedString|
//@[22:28) Identifier |string|
//@[29:30) Assignment |=|
//@[31:67) StringComplete |'First line\r\nSecond\ttabbed\tline'|
//@[67:71) NewLine |\r\n\r\n|

// object default value
//@[23:25) NewLine |\r\n|
param foo object = {
//@[0:5) Identifier |param|
//@[6:9) Identifier |foo|
//@[10:16) Identifier |object|
//@[17:18) Assignment |=|
//@[19:20) LeftBrace |{|
//@[20:22) NewLine |\r\n|
  enabled: true
//@[2:9) Identifier |enabled|
//@[9:10) Colon |:|
//@[11:15) TrueKeyword |true|
//@[15:17) NewLine |\r\n|
  name: 'this is my object'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:27) StringComplete |'this is my object'|
//@[27:29) NewLine |\r\n|
  priority: 3
//@[2:10) Identifier |priority|
//@[10:11) Colon |:|
//@[12:13) Number |3|
//@[13:15) NewLine |\r\n|
  info: {
//@[2:6) Identifier |info|
//@[6:7) Colon |:|
//@[8:9) LeftBrace |{|
//@[9:11) NewLine |\r\n|
    a: 'b'
//@[4:5) Identifier |a|
//@[5:6) Colon |:|
//@[7:10) StringComplete |'b'|
//@[10:12) NewLine |\r\n|
  }
//@[2:3) RightBrace |}|
//@[3:5) NewLine |\r\n|
  empty: {
//@[2:7) Identifier |empty|
//@[7:8) Colon |:|
//@[9:10) LeftBrace |{|
//@[10:12) NewLine |\r\n|
  }
//@[2:3) RightBrace |}|
//@[3:5) NewLine |\r\n|
  array: [
//@[2:7) Identifier |array|
//@[7:8) Colon |:|
//@[9:10) LeftSquare |[|
//@[10:12) NewLine |\r\n|
    'string item'
//@[4:17) StringComplete |'string item'|
//@[17:19) NewLine |\r\n|
    12
//@[4:6) Number |12|
//@[6:8) NewLine |\r\n|
    true
//@[4:8) TrueKeyword |true|
//@[8:10) NewLine |\r\n|
    [
//@[4:5) LeftSquare |[|
//@[5:7) NewLine |\r\n|
      'inner'
//@[6:13) StringComplete |'inner'|
//@[13:15) NewLine |\r\n|
      false
//@[6:11) FalseKeyword |false|
//@[11:13) NewLine |\r\n|
    ]
//@[4:5) RightSquare |]|
//@[5:7) NewLine |\r\n|
    {
//@[4:5) LeftBrace |{|
//@[5:7) NewLine |\r\n|
      a: 'b'
//@[6:7) Identifier |a|
//@[7:8) Colon |:|
//@[9:12) StringComplete |'b'|
//@[12:14) NewLine |\r\n|
    }
//@[4:5) RightBrace |}|
//@[5:7) NewLine |\r\n|
  ]
//@[2:3) RightSquare |]|
//@[3:5) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

// array default value
//@[22:24) NewLine |\r\n|
param myArrayParam array = [
//@[0:5) Identifier |param|
//@[6:18) Identifier |myArrayParam|
//@[19:24) Identifier |array|
//@[25:26) Assignment |=|
//@[27:28) LeftSquare |[|
//@[28:30) NewLine |\r\n|
  'a'
//@[2:5) StringComplete |'a'|
//@[5:7) NewLine |\r\n|
  'b'
//@[2:5) StringComplete |'b'|
//@[5:7) NewLine |\r\n|
  'c'
//@[2:5) StringComplete |'c'|
//@[5:7) NewLine |\r\n|
]
//@[0:1) RightSquare |]|
//@[1:5) NewLine |\r\n\r\n|

// alternative array parameter
//@[30:32) NewLine |\r\n|
param myAlternativeArrayParam array {
//@[0:5) Identifier |param|
//@[6:29) Identifier |myAlternativeArrayParam|
//@[30:35) Identifier |array|
//@[36:37) LeftBrace |{|
//@[37:39) NewLine |\r\n|
  default: [
//@[2:9) Identifier |default|
//@[9:10) Colon |:|
//@[11:12) LeftSquare |[|
//@[12:14) NewLine |\r\n|
    'a'
//@[4:7) StringComplete |'a'|
//@[7:9) NewLine |\r\n|
    'b'
//@[4:7) StringComplete |'b'|
//@[7:9) NewLine |\r\n|
    'c'
//@[4:7) StringComplete |'c'|
//@[7:9) NewLine |\r\n|
  ]
//@[2:3) RightSquare |]|
//@[3:5) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

// secure string
//@[16:18) NewLine |\r\n|
param password string {
//@[0:5) Identifier |param|
//@[6:14) Identifier |password|
//@[15:21) Identifier |string|
//@[22:23) LeftBrace |{|
//@[23:25) NewLine |\r\n|
  secure: true
//@[2:8) Identifier |secure|
//@[8:9) Colon |:|
//@[10:14) TrueKeyword |true|
//@[14:16) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

// non-secure string
//@[20:22) NewLine |\r\n|
param nonSecure string {
//@[0:5) Identifier |param|
//@[6:15) Identifier |nonSecure|
//@[16:22) Identifier |string|
//@[23:24) LeftBrace |{|
//@[24:26) NewLine |\r\n|
  secure: false
//@[2:8) Identifier |secure|
//@[8:9) Colon |:|
//@[10:15) FalseKeyword |false|
//@[15:17) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

// secure object
//@[16:18) NewLine |\r\n|
param secretObject object {
//@[0:5) Identifier |param|
//@[6:18) Identifier |secretObject|
//@[19:25) Identifier |object|
//@[26:27) LeftBrace |{|
//@[27:29) NewLine |\r\n|
  secure: true
//@[2:8) Identifier |secure|
//@[8:9) Colon |:|
//@[10:14) TrueKeyword |true|
//@[14:16) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

// enum parameter
//@[17:19) NewLine |\r\n|
param storageSku string {
//@[0:5) Identifier |param|
//@[6:16) Identifier |storageSku|
//@[17:23) Identifier |string|
//@[24:25) LeftBrace |{|
//@[25:27) NewLine |\r\n|
  allowed: [
//@[2:9) Identifier |allowed|
//@[9:10) Colon |:|
//@[11:12) LeftSquare |[|
//@[12:14) NewLine |\r\n|
    'Standard_LRS'
//@[4:18) StringComplete |'Standard_LRS'|
//@[18:20) NewLine |\r\n|
    'Standard_GRS'
//@[4:18) StringComplete |'Standard_GRS'|
//@[18:20) NewLine |\r\n|
  ]
//@[2:3) RightSquare |]|
//@[3:5) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

// length constraint on a string
//@[32:34) NewLine |\r\n|
param storageName string {
//@[0:5) Identifier |param|
//@[6:17) Identifier |storageName|
//@[18:24) Identifier |string|
//@[25:26) LeftBrace |{|
//@[26:28) NewLine |\r\n|
  minLength: 3
//@[2:11) Identifier |minLength|
//@[11:12) Colon |:|
//@[13:14) Number |3|
//@[14:16) NewLine |\r\n|
  maxLength: 24
//@[2:11) Identifier |maxLength|
//@[11:12) Colon |:|
//@[13:15) Number |24|
//@[15:17) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

// length constraint on an array
//@[32:34) NewLine |\r\n|
param someArray array {
//@[0:5) Identifier |param|
//@[6:15) Identifier |someArray|
//@[16:21) Identifier |array|
//@[22:23) LeftBrace |{|
//@[23:25) NewLine |\r\n|
  minLength: 3
//@[2:11) Identifier |minLength|
//@[11:12) Colon |:|
//@[13:14) Number |3|
//@[14:16) NewLine |\r\n|
  maxLength: 24
//@[2:11) Identifier |maxLength|
//@[11:12) Colon |:|
//@[13:15) Number |24|
//@[15:17) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

// empty metadata
//@[17:19) NewLine |\r\n|
param emptyMetadata string {
//@[0:5) Identifier |param|
//@[6:19) Identifier |emptyMetadata|
//@[20:26) Identifier |string|
//@[27:28) LeftBrace |{|
//@[28:30) NewLine |\r\n|
  metadata: {
//@[2:10) Identifier |metadata|
//@[10:11) Colon |:|
//@[12:13) LeftBrace |{|
//@[13:15) NewLine |\r\n|
  }
//@[2:3) RightBrace |}|
//@[3:5) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

// description
//@[14:16) NewLine |\r\n|
param description string {
//@[0:5) Identifier |param|
//@[6:17) Identifier |description|
//@[18:24) Identifier |string|
//@[25:26) LeftBrace |{|
//@[26:28) NewLine |\r\n|
  metadata: {
//@[2:10) Identifier |metadata|
//@[10:11) Colon |:|
//@[12:13) LeftBrace |{|
//@[13:15) NewLine |\r\n|
    description: 'my description'
//@[4:15) Identifier |description|
//@[15:16) Colon |:|
//@[17:33) StringComplete |'my description'|
//@[33:35) NewLine |\r\n|
  }
//@[2:3) RightBrace |}|
//@[3:5) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

// random extra metadata
//@[24:26) NewLine |\r\n|
param additionalMetadata string {
//@[0:5) Identifier |param|
//@[6:24) Identifier |additionalMetadata|
//@[25:31) Identifier |string|
//@[32:33) LeftBrace |{|
//@[33:35) NewLine |\r\n|
  metadata: {
//@[2:10) Identifier |metadata|
//@[10:11) Colon |:|
//@[12:13) LeftBrace |{|
//@[13:15) NewLine |\r\n|
    description: 'my description'
//@[4:15) Identifier |description|
//@[15:16) Colon |:|
//@[17:33) StringComplete |'my description'|
//@[33:35) NewLine |\r\n|
    a: 1
//@[4:5) Identifier |a|
//@[5:6) Colon |:|
//@[7:8) Number |1|
//@[8:10) NewLine |\r\n|
    b: true
//@[4:5) Identifier |b|
//@[5:6) Colon |:|
//@[7:11) TrueKeyword |true|
//@[11:13) NewLine |\r\n|
    c: [
//@[4:5) Identifier |c|
//@[5:6) Colon |:|
//@[7:8) LeftSquare |[|
//@[8:10) NewLine |\r\n|
    ]
//@[4:5) RightSquare |]|
//@[5:7) NewLine |\r\n|
    d: {
//@[4:5) Identifier |d|
//@[5:6) Colon |:|
//@[7:8) LeftBrace |{|
//@[8:10) NewLine |\r\n|
      test: 'abc'
//@[6:10) Identifier |test|
//@[10:11) Colon |:|
//@[12:17) StringComplete |'abc'|
//@[17:19) NewLine |\r\n|
    }
//@[4:5) RightBrace |}|
//@[5:7) NewLine |\r\n|
  }
//@[2:3) RightBrace |}|
//@[3:5) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

// all modifiers together
//@[25:27) NewLine |\r\n|
param someParameter string {
//@[0:5) Identifier |param|
//@[6:19) Identifier |someParameter|
//@[20:26) Identifier |string|
//@[27:28) LeftBrace |{|
//@[28:30) NewLine |\r\n|
  secure: true
//@[2:8) Identifier |secure|
//@[8:9) Colon |:|
//@[10:14) TrueKeyword |true|
//@[14:16) NewLine |\r\n|
  minLength: 3
//@[2:11) Identifier |minLength|
//@[11:12) Colon |:|
//@[13:14) Number |3|
//@[14:16) NewLine |\r\n|
  maxLength: 24
//@[2:11) Identifier |maxLength|
//@[11:12) Colon |:|
//@[13:15) Number |24|
//@[15:17) NewLine |\r\n|
  default: 'one'
//@[2:9) Identifier |default|
//@[9:10) Colon |:|
//@[11:16) StringComplete |'one'|
//@[16:18) NewLine |\r\n|
  allowed: [
//@[2:9) Identifier |allowed|
//@[9:10) Colon |:|
//@[11:12) LeftSquare |[|
//@[12:14) NewLine |\r\n|
    'one'
//@[4:9) StringComplete |'one'|
//@[9:11) NewLine |\r\n|
    'two'
//@[4:9) StringComplete |'two'|
//@[9:11) NewLine |\r\n|
    'three'
//@[4:11) StringComplete |'three'|
//@[11:13) NewLine |\r\n|
  ]
//@[2:3) RightSquare |]|
//@[3:5) NewLine |\r\n|
  metadata: {
//@[2:10) Identifier |metadata|
//@[10:11) Colon |:|
//@[12:13) LeftBrace |{|
//@[13:15) NewLine |\r\n|
    description: 'Name of the storage account'
//@[4:15) Identifier |description|
//@[15:16) Colon |:|
//@[17:46) StringComplete |'Name of the storage account'|
//@[46:48) NewLine |\r\n|
  }
//@[2:3) RightBrace |}|
//@[3:5) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

param defaultValueExpression int {
//@[0:5) Identifier |param|
//@[6:28) Identifier |defaultValueExpression|
//@[29:32) Identifier |int|
//@[33:34) LeftBrace |{|
//@[34:36) NewLine |\r\n|
  default: true ? 4 + 2*3 : 0
//@[2:9) Identifier |default|
//@[9:10) Colon |:|
//@[11:15) TrueKeyword |true|
//@[16:17) Question |?|
//@[18:19) Number |4|
//@[20:21) Plus |+|
//@[22:23) Number |2|
//@[23:24) Asterisk |*|
//@[24:25) Number |3|
//@[26:27) Colon |:|
//@[28:29) Number |0|
//@[29:31) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

param defaultExpression bool = 18 != (true || false)
//@[0:5) Identifier |param|
//@[6:23) Identifier |defaultExpression|
//@[24:28) Identifier |bool|
//@[29:30) Assignment |=|
//@[31:33) Number |18|
//@[34:36) NotEquals |!=|
//@[37:38) LeftParen |(|
//@[38:42) TrueKeyword |true|
//@[43:45) LogicalOr ||||
//@[46:51) FalseKeyword |false|
//@[51:52) RightParen |)|
//@[52:56) NewLine |\r\n\r\n|

param stringLiteral string {
//@[0:5) Identifier |param|
//@[6:19) Identifier |stringLiteral|
//@[20:26) Identifier |string|
//@[27:28) LeftBrace |{|
//@[28:30) NewLine |\r\n|
  allowed: [
//@[2:9) Identifier |allowed|
//@[9:10) Colon |:|
//@[11:12) LeftSquare |[|
//@[12:14) NewLine |\r\n|
    'abc'
//@[4:9) StringComplete |'abc'|
//@[9:11) NewLine |\r\n|
    'def'
//@[4:9) StringComplete |'def'|
//@[9:11) NewLine |\r\n|
  ]
//@[2:3) RightSquare |]|
//@[3:5) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

param stringLiteralWithAllowedValuesSuperset string {
//@[0:5) Identifier |param|
//@[6:44) Identifier |stringLiteralWithAllowedValuesSuperset|
//@[45:51) Identifier |string|
//@[52:53) LeftBrace |{|
//@[53:55) NewLine |\r\n|
  allowed: [
//@[2:9) Identifier |allowed|
//@[9:10) Colon |:|
//@[11:12) LeftSquare |[|
//@[12:14) NewLine |\r\n|
    'abc'
//@[4:9) StringComplete |'abc'|
//@[9:11) NewLine |\r\n|
    'def'
//@[4:9) StringComplete |'def'|
//@[9:11) NewLine |\r\n|
    'ghi'
//@[4:9) StringComplete |'ghi'|
//@[9:11) NewLine |\r\n|
  ]
//@[2:3) RightSquare |]|
//@[3:5) NewLine |\r\n|
  default: stringLiteral
//@[2:9) Identifier |default|
//@[9:10) Colon |:|
//@[11:24) Identifier |stringLiteral|
//@[24:26) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

@secure()
//@[0:1) At |@|
//@[1:7) Identifier |secure|
//@[7:8) LeftParen |(|
//@[8:9) RightParen |)|
//@[9:11) NewLine |\r\n|
@minLength(2)
//@[0:1) At |@|
//@[1:10) Identifier |minLength|
//@[10:11) LeftParen |(|
//@[11:12) Number |2|
//@[12:13) RightParen |)|
//@[13:15) NewLine |\r\n|
  @maxLength(10)
//@[2:3) At |@|
//@[3:12) Identifier |maxLength|
//@[12:13) LeftParen |(|
//@[13:15) Number |10|
//@[15:16) RightParen |)|
//@[16:18) NewLine |\r\n|
@allowed([
//@[0:1) At |@|
//@[1:8) Identifier |allowed|
//@[8:9) LeftParen |(|
//@[9:10) LeftSquare |[|
//@[10:12) NewLine |\r\n|
  'Apple'
//@[2:9) StringComplete |'Apple'|
//@[9:11) NewLine |\r\n|
  'Banana'
//@[2:10) StringComplete |'Banana'|
//@[10:12) NewLine |\r\n|
])
//@[0:1) RightSquare |]|
//@[1:2) RightParen |)|
//@[2:4) NewLine |\r\n|
param decoratedString string
//@[0:5) Identifier |param|
//@[6:21) Identifier |decoratedString|
//@[22:28) Identifier |string|
//@[28:32) NewLine |\r\n\r\n|

@minValue(200)
//@[0:1) At |@|
//@[1:9) Identifier |minValue|
//@[9:10) LeftParen |(|
//@[10:13) Number |200|
//@[13:14) RightParen |)|
//@[14:16) NewLine |\r\n|
param decoratedInt int = 123
//@[0:5) Identifier |param|
//@[6:18) Identifier |decoratedInt|
//@[19:22) Identifier |int|
//@[23:24) Assignment |=|
//@[25:28) Number |123|
//@[28:32) NewLine |\r\n\r\n|

@sys.description('A boolean.')
//@[0:1) At |@|
//@[1:4) Identifier |sys|
//@[4:5) Dot |.|
//@[5:16) Identifier |description|
//@[16:17) LeftParen |(|
//@[17:29) StringComplete |'A boolean.'|
//@[29:30) RightParen |)|
//@[30:32) NewLine |\r\n|
@metadata({
//@[0:1) At |@|
//@[1:9) Identifier |metadata|
//@[9:10) LeftParen |(|
//@[10:11) LeftBrace |{|
//@[11:13) NewLine |\r\n|
    description: 'I will be overrode.'
//@[4:15) Identifier |description|
//@[15:16) Colon |:|
//@[17:38) StringComplete |'I will be overrode.'|
//@[38:40) NewLine |\r\n|
    foo: 'something'
//@[4:7) Identifier |foo|
//@[7:8) Colon |:|
//@[9:20) StringComplete |'something'|
//@[20:22) NewLine |\r\n|
    bar: [
//@[4:7) Identifier |bar|
//@[7:8) Colon |:|
//@[9:10) LeftSquare |[|
//@[10:12) NewLine |\r\n|
        {          }
//@[8:9) LeftBrace |{|
//@[19:20) RightBrace |}|
//@[20:22) NewLine |\r\n|
        true
//@[8:12) TrueKeyword |true|
//@[12:14) NewLine |\r\n|
        123
//@[8:11) Number |123|
//@[11:13) NewLine |\r\n|
    ]
//@[4:5) RightSquare |]|
//@[5:7) NewLine |\r\n|
})
//@[0:1) RightBrace |}|
//@[1:2) RightParen |)|
//@[2:4) NewLine |\r\n|
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
//@[50:54) NewLine |\r\n\r\n|

@secure()
//@[0:1) At |@|
//@[1:7) Identifier |secure|
//@[7:8) LeftParen |(|
//@[8:9) RightParen |)|
//@[9:11) NewLine |\r\n|
@secure()
//@[0:1) At |@|
//@[1:7) Identifier |secure|
//@[7:8) LeftParen |(|
//@[8:9) RightParen |)|
//@[9:11) NewLine |\r\n|
@secure()
//@[0:1) At |@|
//@[1:7) Identifier |secure|
//@[7:8) LeftParen |(|
//@[8:9) RightParen |)|
//@[9:11) NewLine |\r\n|
param decoratedObject object = {
//@[0:5) Identifier |param|
//@[6:21) Identifier |decoratedObject|
//@[22:28) Identifier |object|
//@[29:30) Assignment |=|
//@[31:32) LeftBrace |{|
//@[32:34) NewLine |\r\n|
  enabled: true
//@[2:9) Identifier |enabled|
//@[9:10) Colon |:|
//@[11:15) TrueKeyword |true|
//@[15:17) NewLine |\r\n|
  name: 'this is my object'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:27) StringComplete |'this is my object'|
//@[27:29) NewLine |\r\n|
  priority: 3
//@[2:10) Identifier |priority|
//@[10:11) Colon |:|
//@[12:13) Number |3|
//@[13:15) NewLine |\r\n|
  info: {
//@[2:6) Identifier |info|
//@[6:7) Colon |:|
//@[8:9) LeftBrace |{|
//@[9:11) NewLine |\r\n|
    a: 'b'
//@[4:5) Identifier |a|
//@[5:6) Colon |:|
//@[7:10) StringComplete |'b'|
//@[10:12) NewLine |\r\n|
  }
//@[2:3) RightBrace |}|
//@[3:5) NewLine |\r\n|
  empty: {
//@[2:7) Identifier |empty|
//@[7:8) Colon |:|
//@[9:10) LeftBrace |{|
//@[10:12) NewLine |\r\n|
  }
//@[2:3) RightBrace |}|
//@[3:5) NewLine |\r\n|
  array: [
//@[2:7) Identifier |array|
//@[7:8) Colon |:|
//@[9:10) LeftSquare |[|
//@[10:12) NewLine |\r\n|
    'string item'
//@[4:17) StringComplete |'string item'|
//@[17:19) NewLine |\r\n|
    12
//@[4:6) Number |12|
//@[6:8) NewLine |\r\n|
    true
//@[4:8) TrueKeyword |true|
//@[8:10) NewLine |\r\n|
    [
//@[4:5) LeftSquare |[|
//@[5:7) NewLine |\r\n|
      'inner'
//@[6:13) StringComplete |'inner'|
//@[13:15) NewLine |\r\n|
      false
//@[6:11) FalseKeyword |false|
//@[11:13) NewLine |\r\n|
    ]
//@[4:5) RightSquare |]|
//@[5:7) NewLine |\r\n|
    {
//@[4:5) LeftBrace |{|
//@[5:7) NewLine |\r\n|
      a: 'b'
//@[6:7) Identifier |a|
//@[7:8) Colon |:|
//@[9:12) StringComplete |'b'|
//@[12:14) NewLine |\r\n|
    }
//@[4:5) RightBrace |}|
//@[5:7) NewLine |\r\n|
  ]
//@[2:3) RightSquare |]|
//@[3:5) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

@sys.metadata({
//@[0:1) At |@|
//@[1:4) Identifier |sys|
//@[4:5) Dot |.|
//@[5:13) Identifier |metadata|
//@[13:14) LeftParen |(|
//@[14:15) LeftBrace |{|
//@[15:17) NewLine |\r\n|
    description: 'An array.'
//@[4:15) Identifier |description|
//@[15:16) Colon |:|
//@[17:28) StringComplete |'An array.'|
//@[28:30) NewLine |\r\n|
})
//@[0:1) RightBrace |}|
//@[1:2) RightParen |)|
//@[2:4) NewLine |\r\n|
@sys.maxLength(20)
//@[0:1) At |@|
//@[1:4) Identifier |sys|
//@[4:5) Dot |.|
//@[5:14) Identifier |maxLength|
//@[14:15) LeftParen |(|
//@[15:17) Number |20|
//@[17:18) RightParen |)|
//@[18:20) NewLine |\r\n|
@maxLength(10)
//@[0:1) At |@|
//@[1:10) Identifier |maxLength|
//@[10:11) LeftParen |(|
//@[11:13) Number |10|
//@[13:14) RightParen |)|
//@[14:16) NewLine |\r\n|
@maxLength(5)
//@[0:1) At |@|
//@[1:10) Identifier |maxLength|
//@[10:11) LeftParen |(|
//@[11:12) Number |5|
//@[12:13) RightParen |)|
//@[13:15) NewLine |\r\n|
@sys.description('I will be overrode.')
//@[0:1) At |@|
//@[1:4) Identifier |sys|
//@[4:5) Dot |.|
//@[5:16) Identifier |description|
//@[16:17) LeftParen |(|
//@[17:38) StringComplete |'I will be overrode.'|
//@[38:39) RightParen |)|
//@[39:41) NewLine |\r\n|
param decoratedArray array = [
//@[0:5) Identifier |param|
//@[6:20) Identifier |decoratedArray|
//@[21:26) Identifier |array|
//@[27:28) Assignment |=|
//@[29:30) LeftSquare |[|
//@[30:32) NewLine |\r\n|
    utcNow()
//@[4:10) Identifier |utcNow|
//@[10:11) LeftParen |(|
//@[11:12) RightParen |)|
//@[12:14) NewLine |\r\n|
    newGuid()
//@[4:11) Identifier |newGuid|
//@[11:12) LeftParen |(|
//@[12:13) RightParen |)|
//@[13:15) NewLine |\r\n|
]
//@[0:1) RightSquare |]|
//@[1:3) NewLine |\r\n|

//@[0:0) EndOfFile ||
