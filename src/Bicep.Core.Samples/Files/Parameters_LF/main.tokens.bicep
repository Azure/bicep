/* 
  This is a block comment.
*/
//@[2:4) NewLine |\n\n|

// parameters without default value
//@[35:36) NewLine |\n|
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
param myString2 string = 'strin${2}g value'
//@[0:5) Identifier |param|
//@[6:15) Identifier |myString2|
//@[16:22) Identifier |string|
//@[23:24) Assignment |=|
//@[25:33) StringLeftPiece |'strin${|
//@[33:34) Integer |2|
//@[34:43) StringRightPiece |}g value'|
//@[43:44) NewLine |\n|
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
param myEscapedString string = 'First line\nSecond\ttabbed\tline'
//@[0:5) Identifier |param|
//@[6:21) Identifier |myEscapedString|
//@[22:28) Identifier |string|
//@[29:30) Assignment |=|
//@[31:65) StringComplete |'First line\nSecond\ttabbed\tline'|
//@[65:66) NewLine |\n|
param myNewGuid string = newGuid()
//@[0:5) Identifier |param|
//@[6:15) Identifier |myNewGuid|
//@[16:22) Identifier |string|
//@[23:24) Assignment |=|
//@[25:32) Identifier |newGuid|
//@[32:33) LeftParen |(|
//@[33:34) RightParen |)|
//@[34:35) NewLine |\n|
param myUtcTime string = utcNow()
//@[0:5) Identifier |param|
//@[6:15) Identifier |myUtcTime|
//@[16:22) Identifier |string|
//@[23:24) Assignment |=|
//@[25:31) Identifier |utcNow|
//@[31:32) LeftParen |(|
//@[32:33) RightParen |)|
//@[33:35) NewLine |\n\n|

// object default value
//@[23:24) NewLine |\n|
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
  test: {
//@[2:6) Identifier |test|
//@[6:7) Colon |:|
//@[8:9) LeftBrace |{|
//@[9:10) NewLine |\n|
    time: utcNow('u')
//@[4:8) Identifier |time|
//@[8:9) Colon |:|
//@[10:16) Identifier |utcNow|
//@[16:17) LeftParen |(|
//@[17:20) StringComplete |'u'|
//@[20:21) RightParen |)|
//@[21:22) NewLine |\n|
    guid: newGuid()
//@[4:8) Identifier |guid|
//@[8:9) Colon |:|
//@[10:17) Identifier |newGuid|
//@[17:18) LeftParen |(|
//@[18:19) RightParen |)|
//@[19:20) NewLine |\n|
  }
//@[2:3) RightBrace |}|
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

// alternative array parameter
//@[30:31) NewLine |\n|
param myAlternativeArrayParam array {
//@[0:5) Identifier |param|
//@[6:29) Identifier |myAlternativeArrayParam|
//@[30:35) Identifier |array|
//@[36:37) LeftBrace |{|
//@[37:38) NewLine |\n|
  default: [
//@[2:9) Identifier |default|
//@[9:10) Colon |:|
//@[11:12) LeftSquare |[|
//@[12:13) NewLine |\n|
    'a'
//@[4:7) StringComplete |'a'|
//@[7:8) NewLine |\n|
    'b'
//@[4:7) StringComplete |'b'|
//@[7:8) NewLine |\n|
    'c'
//@[4:7) StringComplete |'c'|
//@[7:8) NewLine |\n|
    newGuid()
//@[4:11) Identifier |newGuid|
//@[11:12) LeftParen |(|
//@[12:13) RightParen |)|
//@[13:14) NewLine |\n|
    utcNow()
//@[4:10) Identifier |utcNow|
//@[10:11) LeftParen |(|
//@[11:12) RightParen |)|
//@[12:13) NewLine |\n|
  ]
//@[2:3) RightSquare |]|
//@[3:4) NewLine |\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

// secure string
//@[16:17) NewLine |\n|
param password string {
//@[0:5) Identifier |param|
//@[6:14) Identifier |password|
//@[15:21) Identifier |string|
//@[22:23) LeftBrace |{|
//@[23:24) NewLine |\n|
  secure: true
//@[2:8) Identifier |secure|
//@[8:9) Colon |:|
//@[10:14) TrueKeyword |true|
//@[14:15) NewLine |\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

@secure()
//@[0:1) At |@|
//@[1:7) Identifier |secure|
//@[7:8) LeftParen |(|
//@[8:9) RightParen |)|
//@[9:10) NewLine |\n|
param passwordWithDecorator string
//@[0:5) Identifier |param|
//@[6:27) Identifier |passwordWithDecorator|
//@[28:34) Identifier |string|
//@[34:36) NewLine |\n\n|

// non-secure string
//@[20:21) NewLine |\n|
param nonSecure string {
//@[0:5) Identifier |param|
//@[6:15) Identifier |nonSecure|
//@[16:22) Identifier |string|
//@[23:24) LeftBrace |{|
//@[24:25) NewLine |\n|
  secure: false
//@[2:8) Identifier |secure|
//@[8:9) Colon |:|
//@[10:15) FalseKeyword |false|
//@[15:16) NewLine |\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

// secure object
//@[16:17) NewLine |\n|
param secretObject object {
//@[0:5) Identifier |param|
//@[6:18) Identifier |secretObject|
//@[19:25) Identifier |object|
//@[26:27) LeftBrace |{|
//@[27:28) NewLine |\n|
  secure: true
//@[2:8) Identifier |secure|
//@[8:9) Colon |:|
//@[10:14) TrueKeyword |true|
//@[14:15) NewLine |\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

@secure()
//@[0:1) At |@|
//@[1:7) Identifier |secure|
//@[7:8) LeftParen |(|
//@[8:9) RightParen |)|
//@[9:10) NewLine |\n|
param secureObjectWithDecorator object
//@[0:5) Identifier |param|
//@[6:31) Identifier |secureObjectWithDecorator|
//@[32:38) Identifier |object|
//@[38:40) NewLine |\n\n|

// enum parameter
//@[17:18) NewLine |\n|
param storageSku string {
//@[0:5) Identifier |param|
//@[6:16) Identifier |storageSku|
//@[17:23) Identifier |string|
//@[24:25) LeftBrace |{|
//@[25:26) NewLine |\n|
  allowed: [
//@[2:9) Identifier |allowed|
//@[9:10) Colon |:|
//@[11:12) LeftSquare |[|
//@[12:13) NewLine |\n|
    'Standard_LRS'
//@[4:18) StringComplete |'Standard_LRS'|
//@[18:19) NewLine |\n|
    'Standard_GRS'
//@[4:18) StringComplete |'Standard_GRS'|
//@[18:19) NewLine |\n|
  ]
//@[2:3) RightSquare |]|
//@[3:4) NewLine |\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

@  allowed([
//@[0:1) At |@|
//@[3:10) Identifier |allowed|
//@[10:11) LeftParen |(|
//@[11:12) LeftSquare |[|
//@[12:13) NewLine |\n|
'Standard_LRS'
//@[0:14) StringComplete |'Standard_LRS'|
//@[14:15) NewLine |\n|
'Standard_GRS'
//@[0:14) StringComplete |'Standard_GRS'|
//@[14:15) NewLine |\n|
])
//@[0:1) RightSquare |]|
//@[1:2) RightParen |)|
//@[2:3) NewLine |\n|
param storageSkuWithDecorator string
//@[0:5) Identifier |param|
//@[6:29) Identifier |storageSkuWithDecorator|
//@[30:36) Identifier |string|
//@[36:38) NewLine |\n\n|

// length constraint on a string
//@[32:33) NewLine |\n|
param storageName string {
//@[0:5) Identifier |param|
//@[6:17) Identifier |storageName|
//@[18:24) Identifier |string|
//@[25:26) LeftBrace |{|
//@[26:27) NewLine |\n|
  minLength: 3
//@[2:11) Identifier |minLength|
//@[11:12) Colon |:|
//@[13:14) Integer |3|
//@[14:15) NewLine |\n|
  maxLength: 24
//@[2:11) Identifier |maxLength|
//@[11:12) Colon |:|
//@[13:15) Integer |24|
//@[15:16) NewLine |\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

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
param storageNameWithDecorator string
//@[0:5) Identifier |param|
//@[6:30) Identifier |storageNameWithDecorator|
//@[31:37) Identifier |string|
//@[37:39) NewLine |\n\n|

// length constraint on an array
//@[32:33) NewLine |\n|
param someArray array {
//@[0:5) Identifier |param|
//@[6:15) Identifier |someArray|
//@[16:21) Identifier |array|
//@[22:23) LeftBrace |{|
//@[23:24) NewLine |\n|
  minLength: 3
//@[2:11) Identifier |minLength|
//@[11:12) Colon |:|
//@[13:14) Integer |3|
//@[14:15) NewLine |\n|
  maxLength: 24
//@[2:11) Identifier |maxLength|
//@[11:12) Colon |:|
//@[13:15) Integer |24|
//@[15:16) NewLine |\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

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
param someArrayWithDecorator array
//@[0:5) Identifier |param|
//@[6:28) Identifier |someArrayWithDecorator|
//@[29:34) Identifier |array|
//@[34:36) NewLine |\n\n|

// empty metadata
//@[17:18) NewLine |\n|
param emptyMetadata string {
//@[0:5) Identifier |param|
//@[6:19) Identifier |emptyMetadata|
//@[20:26) Identifier |string|
//@[27:28) LeftBrace |{|
//@[28:29) NewLine |\n|
  metadata: {
//@[2:10) Identifier |metadata|
//@[10:11) Colon |:|
//@[12:13) LeftBrace |{|
//@[13:14) NewLine |\n|
  }
//@[2:3) RightBrace |}|
//@[3:4) NewLine |\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

@metadata({})
//@[0:1) At |@|
//@[1:9) Identifier |metadata|
//@[9:10) LeftParen |(|
//@[10:11) LeftBrace |{|
//@[11:12) RightBrace |}|
//@[12:13) RightParen |)|
//@[13:14) NewLine |\n|
param emptyMetadataWithDecorator string
//@[0:5) Identifier |param|
//@[6:32) Identifier |emptyMetadataWithDecorator|
//@[33:39) Identifier |string|
//@[39:41) NewLine |\n\n|

// description
//@[14:15) NewLine |\n|
param description string {
//@[0:5) Identifier |param|
//@[6:17) Identifier |description|
//@[18:24) Identifier |string|
//@[25:26) LeftBrace |{|
//@[26:27) NewLine |\n|
  metadata: {
//@[2:10) Identifier |metadata|
//@[10:11) Colon |:|
//@[12:13) LeftBrace |{|
//@[13:14) NewLine |\n|
    description: 'my description'
//@[4:15) Identifier |description|
//@[15:16) Colon |:|
//@[17:33) StringComplete |'my description'|
//@[33:34) NewLine |\n|
  }
//@[2:3) RightBrace |}|
//@[3:4) NewLine |\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

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
param descriptionWithDecorator string
//@[0:5) Identifier |param|
//@[6:30) Identifier |descriptionWithDecorator|
//@[31:37) Identifier |string|
//@[37:39) NewLine |\n\n|

@sys.description('my description')
//@[0:1) At |@|
//@[1:4) Identifier |sys|
//@[4:5) Dot |.|
//@[5:16) Identifier |description|
//@[16:17) LeftParen |(|
//@[17:33) StringComplete |'my description'|
//@[33:34) RightParen |)|
//@[34:35) NewLine |\n|
param descriptionWithDecorator2 string
//@[0:5) Identifier |param|
//@[6:31) Identifier |descriptionWithDecorator2|
//@[32:38) Identifier |string|
//@[38:40) NewLine |\n\n|

// random extra metadata
//@[24:25) NewLine |\n|
param additionalMetadata string {
//@[0:5) Identifier |param|
//@[6:24) Identifier |additionalMetadata|
//@[25:31) Identifier |string|
//@[32:33) LeftBrace |{|
//@[33:34) NewLine |\n|
  metadata: {
//@[2:10) Identifier |metadata|
//@[10:11) Colon |:|
//@[12:13) LeftBrace |{|
//@[13:14) NewLine |\n|
    description: 'my description'
//@[4:15) Identifier |description|
//@[15:16) Colon |:|
//@[17:33) StringComplete |'my description'|
//@[33:34) NewLine |\n|
    a: 1
//@[4:5) Identifier |a|
//@[5:6) Colon |:|
//@[7:8) Integer |1|
//@[8:9) NewLine |\n|
    b: true
//@[4:5) Identifier |b|
//@[5:6) Colon |:|
//@[7:11) TrueKeyword |true|
//@[11:12) NewLine |\n|
    c: [
//@[4:5) Identifier |c|
//@[5:6) Colon |:|
//@[7:8) LeftSquare |[|
//@[8:9) NewLine |\n|
    ]
//@[4:5) RightSquare |]|
//@[5:6) NewLine |\n|
    d: {
//@[4:5) Identifier |d|
//@[5:6) Colon |:|
//@[7:8) LeftBrace |{|
//@[8:9) NewLine |\n|
      test: 'abc'
//@[6:10) Identifier |test|
//@[10:11) Colon |:|
//@[12:17) StringComplete |'abc'|
//@[17:18) NewLine |\n|
    }
//@[4:5) RightBrace |}|
//@[5:6) NewLine |\n|
  }
//@[2:3) RightBrace |}|
//@[3:4) NewLine |\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

@metadata({
//@[0:1) At |@|
//@[1:9) Identifier |metadata|
//@[9:10) LeftParen |(|
//@[10:11) LeftBrace |{|
//@[11:12) NewLine |\n|
	description: 'my description'
//@[1:12) Identifier |description|
//@[12:13) Colon |:|
//@[14:30) StringComplete |'my description'|
//@[30:31) NewLine |\n|
	a: 1
//@[1:2) Identifier |a|
//@[2:3) Colon |:|
//@[4:5) Integer |1|
//@[5:6) NewLine |\n|
	b: true
//@[1:2) Identifier |b|
//@[2:3) Colon |:|
//@[4:8) TrueKeyword |true|
//@[8:9) NewLine |\n|
	c: [
//@[1:2) Identifier |c|
//@[2:3) Colon |:|
//@[4:5) LeftSquare |[|
//@[5:6) NewLine |\n|
	]
//@[1:2) RightSquare |]|
//@[2:3) NewLine |\n|
	d: {
//@[1:2) Identifier |d|
//@[2:3) Colon |:|
//@[4:5) LeftBrace |{|
//@[5:6) NewLine |\n|
	  test: 'abc'
//@[3:7) Identifier |test|
//@[7:8) Colon |:|
//@[9:14) StringComplete |'abc'|
//@[14:15) NewLine |\n|
	}
//@[1:2) RightBrace |}|
//@[2:3) NewLine |\n|
})
//@[0:1) RightBrace |}|
//@[1:2) RightParen |)|
//@[2:3) NewLine |\n|
param additionalMetadataWithDecorator string
//@[0:5) Identifier |param|
//@[6:37) Identifier |additionalMetadataWithDecorator|
//@[38:44) Identifier |string|
//@[44:46) NewLine |\n\n|

// all modifiers together
//@[25:26) NewLine |\n|
param someParameter string {
//@[0:5) Identifier |param|
//@[6:19) Identifier |someParameter|
//@[20:26) Identifier |string|
//@[27:28) LeftBrace |{|
//@[28:29) NewLine |\n|
  secure: true
//@[2:8) Identifier |secure|
//@[8:9) Colon |:|
//@[10:14) TrueKeyword |true|
//@[14:15) NewLine |\n|
  minLength: 3
//@[2:11) Identifier |minLength|
//@[11:12) Colon |:|
//@[13:14) Integer |3|
//@[14:15) NewLine |\n|
  maxLength: 24
//@[2:11) Identifier |maxLength|
//@[11:12) Colon |:|
//@[13:15) Integer |24|
//@[15:16) NewLine |\n|
  default: 'one'
//@[2:9) Identifier |default|
//@[9:10) Colon |:|
//@[11:16) StringComplete |'one'|
//@[16:17) NewLine |\n|
  allowed: [
//@[2:9) Identifier |allowed|
//@[9:10) Colon |:|
//@[11:12) LeftSquare |[|
//@[12:13) NewLine |\n|
    'one'
//@[4:9) StringComplete |'one'|
//@[9:10) NewLine |\n|
    'two'
//@[4:9) StringComplete |'two'|
//@[9:10) NewLine |\n|
    'three'
//@[4:11) StringComplete |'three'|
//@[11:12) NewLine |\n|
  ]
//@[2:3) RightSquare |]|
//@[3:4) NewLine |\n|
  metadata: {
//@[2:10) Identifier |metadata|
//@[10:11) Colon |:|
//@[12:13) LeftBrace |{|
//@[13:14) NewLine |\n|
    description: 'Name of the storage account'
//@[4:15) Identifier |description|
//@[15:16) Colon |:|
//@[17:46) StringComplete |'Name of the storage account'|
//@[46:47) NewLine |\n|
  }
//@[2:3) RightBrace |}|
//@[3:4) NewLine |\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

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
param someParameterWithDecorator string = 'one'
//@[0:5) Identifier |param|
//@[6:32) Identifier |someParameterWithDecorator|
//@[33:39) Identifier |string|
//@[40:41) Assignment |=|
//@[42:47) StringComplete |'one'|
//@[47:49) NewLine |\n\n|

param defaultValueExpression int {
//@[0:5) Identifier |param|
//@[6:28) Identifier |defaultValueExpression|
//@[29:32) Identifier |int|
//@[33:34) LeftBrace |{|
//@[34:35) NewLine |\n|
  default: true ? 4 + 2*3 : 0
//@[2:9) Identifier |default|
//@[9:10) Colon |:|
//@[11:15) TrueKeyword |true|
//@[16:17) Question |?|
//@[18:19) Integer |4|
//@[20:21) Plus |+|
//@[22:23) Integer |2|
//@[23:24) Asterisk |*|
//@[24:25) Integer |3|
//@[26:27) Colon |:|
//@[28:29) Integer |0|
//@[29:30) NewLine |\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

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

// negative zeros are valid lengths
//@[35:36) NewLine |\n|
@minLength(-0)
//@[0:1) At |@|
//@[1:10) Identifier |minLength|
//@[10:11) LeftParen |(|
//@[11:12) Minus |-|
//@[12:13) Integer |0|
//@[13:14) RightParen |)|
//@[14:15) NewLine |\n|
@maxLength(-0)
//@[0:1) At |@|
//@[1:10) Identifier |maxLength|
//@[10:11) LeftParen |(|
//@[11:12) Minus |-|
//@[12:13) Integer |0|
//@[13:14) RightParen |)|
//@[14:15) NewLine |\n|
param negativeZeros string
//@[0:5) Identifier |param|
//@[6:19) Identifier |negativeZeros|
//@[20:26) Identifier |string|
//@[26:28) NewLine |\n\n|

// negative integer literals in modifiers
//@[41:42) NewLine |\n|
param negativeModifiers int {
//@[0:5) Identifier |param|
//@[6:23) Identifier |negativeModifiers|
//@[24:27) Identifier |int|
//@[28:29) LeftBrace |{|
//@[29:30) NewLine |\n|
  minValue: -100
//@[2:10) Identifier |minValue|
//@[10:11) Colon |:|
//@[12:13) Minus |-|
//@[13:16) Integer |100|
//@[16:17) NewLine |\n|
  maxValue: -33
//@[2:10) Identifier |maxValue|
//@[10:11) Colon |:|
//@[12:13) Minus |-|
//@[13:15) Integer |33|
//@[15:16) NewLine |\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

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
param decoratedBool bool
//@[0:5) Identifier |param|
//@[6:19) Identifier |decoratedBool|
//@[20:24) Identifier |bool|
//@[24:26) NewLine |\n\n|

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
  location: 'westus'
//@[2:10) Identifier |location|
//@[10:11) Colon |:|
//@[12:20) StringComplete |'westus'|
//@[20:21) NewLine |\n|
}
//@[0:1) RightBrace |}|
//@[1:4) NewLine |\n\n\n|


@metadata({
//@[0:1) At |@|
//@[1:9) Identifier |metadata|
//@[9:10) LeftParen |(|
//@[10:11) LeftBrace |{|
//@[11:12) NewLine |\n|
    description: 'An array.'
//@[4:15) Identifier |description|
//@[15:16) Colon |:|
//@[17:28) StringComplete |'An array.'|
//@[28:29) NewLine |\n|
})
//@[0:1) RightBrace |}|
//@[1:2) RightParen |)|
//@[2:3) NewLine |\n|
@maxLength(20)
//@[0:1) At |@|
//@[1:10) Identifier |maxLength|
//@[10:11) LeftParen |(|
//@[11:13) Integer |20|
//@[13:14) RightParen |)|
//@[14:15) NewLine |\n|
@sys.description('I will be overrode.')
//@[0:1) At |@|
//@[1:4) Identifier |sys|
//@[4:5) Dot |.|
//@[5:16) Identifier |description|
//@[16:17) LeftParen |(|
//@[17:38) StringComplete |'I will be overrode.'|
//@[38:39) RightParen |)|
//@[39:40) NewLine |\n|
param decoratedArray array
//@[0:5) Identifier |param|
//@[6:20) Identifier |decoratedArray|
//@[21:26) Identifier |array|
//@[26:27) NewLine |\n|

//@[0:0) EndOfFile ||
