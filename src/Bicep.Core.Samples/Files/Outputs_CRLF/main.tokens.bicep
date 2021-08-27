
//@[0:2) NewLine |\r\n|
@sys.description('string output description')
//@[0:1) At |@|
//@[1:4) Identifier |sys|
//@[4:5) Dot |.|
//@[5:16) Identifier |description|
//@[16:17) LeftParen |(|
//@[17:44) StringComplete |'string output description'|
//@[44:45) RightParen |)|
//@[45:47) NewLine |\r\n|
output myStr string = 'hello'
//@[0:6) Identifier |output|
//@[7:12) Identifier |myStr|
//@[13:19) Identifier |string|
//@[20:21) Assignment |=|
//@[22:29) StringComplete |'hello'|
//@[29:33) NewLine |\r\n\r\n|

@sys.description('int output description')
//@[0:1) At |@|
//@[1:4) Identifier |sys|
//@[4:5) Dot |.|
//@[5:16) Identifier |description|
//@[16:17) LeftParen |(|
//@[17:41) StringComplete |'int output description'|
//@[41:42) RightParen |)|
//@[42:44) NewLine |\r\n|
output myInt int = 7
//@[0:6) Identifier |output|
//@[7:12) Identifier |myInt|
//@[13:16) Identifier |int|
//@[17:18) Assignment |=|
//@[19:20) Integer |7|
//@[20:22) NewLine |\r\n|
output myOtherInt int = 20 / 13 + 80 % -4
//@[0:6) Identifier |output|
//@[7:17) Identifier |myOtherInt|
//@[18:21) Identifier |int|
//@[22:23) Assignment |=|
//@[24:26) Integer |20|
//@[27:28) Slash |/|
//@[29:31) Integer |13|
//@[32:33) Plus |+|
//@[34:36) Integer |80|
//@[37:38) Modulo |%|
//@[39:40) Minus |-|
//@[40:41) Integer |4|
//@[41:45) NewLine |\r\n\r\n|

@sys.description('bool output description')
//@[0:1) At |@|
//@[1:4) Identifier |sys|
//@[4:5) Dot |.|
//@[5:16) Identifier |description|
//@[16:17) LeftParen |(|
//@[17:42) StringComplete |'bool output description'|
//@[42:43) RightParen |)|
//@[43:45) NewLine |\r\n|
output myBool bool = !false
//@[0:6) Identifier |output|
//@[7:13) Identifier |myBool|
//@[14:18) Identifier |bool|
//@[19:20) Assignment |=|
//@[21:22) Exclamation |!|
//@[22:27) FalseKeyword |false|
//@[27:29) NewLine |\r\n|
output myOtherBool bool = true
//@[0:6) Identifier |output|
//@[7:18) Identifier |myOtherBool|
//@[19:23) Identifier |bool|
//@[24:25) Assignment |=|
//@[26:30) TrueKeyword |true|
//@[30:34) NewLine |\r\n\r\n|

@sys.description('object array description')
//@[0:1) At |@|
//@[1:4) Identifier |sys|
//@[4:5) Dot |.|
//@[5:16) Identifier |description|
//@[16:17) LeftParen |(|
//@[17:43) StringComplete |'object array description'|
//@[43:44) RightParen |)|
//@[44:46) NewLine |\r\n|
output suchEmpty array = [
//@[0:6) Identifier |output|
//@[7:16) Identifier |suchEmpty|
//@[17:22) Identifier |array|
//@[23:24) Assignment |=|
//@[25:26) LeftSquare |[|
//@[26:28) NewLine |\r\n|
]
//@[0:1) RightSquare |]|
//@[1:5) NewLine |\r\n\r\n|

output suchEmpty2 object = {
//@[0:6) Identifier |output|
//@[7:17) Identifier |suchEmpty2|
//@[18:24) Identifier |object|
//@[25:26) Assignment |=|
//@[27:28) LeftBrace |{|
//@[28:30) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

@sys.description('object output description')
//@[0:1) At |@|
//@[1:4) Identifier |sys|
//@[4:5) Dot |.|
//@[5:16) Identifier |description|
//@[16:17) LeftParen |(|
//@[17:44) StringComplete |'object output description'|
//@[44:45) RightParen |)|
//@[45:47) NewLine |\r\n|
output obj object = {
//@[0:6) Identifier |output|
//@[7:10) Identifier |obj|
//@[11:17) Identifier |object|
//@[18:19) Assignment |=|
//@[20:21) LeftBrace |{|
//@[21:23) NewLine |\r\n|
  a: 'a'
//@[2:3) Identifier |a|
//@[3:4) Colon |:|
//@[5:8) StringComplete |'a'|
//@[8:10) NewLine |\r\n|
  b: 12
//@[2:3) Identifier |b|
//@[3:4) Colon |:|
//@[5:7) Integer |12|
//@[7:9) NewLine |\r\n|
  c: true
//@[2:3) Identifier |c|
//@[3:4) Colon |:|
//@[5:9) TrueKeyword |true|
//@[9:11) NewLine |\r\n|
  d: null
//@[2:3) Identifier |d|
//@[3:4) Colon |:|
//@[5:9) NullKeyword |null|
//@[9:11) NewLine |\r\n|
  list: [
//@[2:6) Identifier |list|
//@[6:7) Colon |:|
//@[8:9) LeftSquare |[|
//@[9:11) NewLine |\r\n|
    1
//@[4:5) Integer |1|
//@[5:7) NewLine |\r\n|
    2
//@[4:5) Integer |2|
//@[5:7) NewLine |\r\n|
    3
//@[4:5) Integer |3|
//@[5:7) NewLine |\r\n|
    null
//@[4:8) NullKeyword |null|
//@[8:10) NewLine |\r\n|
    {
//@[4:5) LeftBrace |{|
//@[5:7) NewLine |\r\n|
    }
//@[4:5) RightBrace |}|
//@[5:7) NewLine |\r\n|
  ]
//@[2:3) RightSquare |]|
//@[3:5) NewLine |\r\n|
  obj: {
//@[2:5) Identifier |obj|
//@[5:6) Colon |:|
//@[7:8) LeftBrace |{|
//@[8:10) NewLine |\r\n|
    nested: [
//@[4:10) Identifier |nested|
//@[10:11) Colon |:|
//@[12:13) LeftSquare |[|
//@[13:15) NewLine |\r\n|
      'hello'
//@[6:13) StringComplete |'hello'|
//@[13:15) NewLine |\r\n|
    ]
//@[4:5) RightSquare |]|
//@[5:7) NewLine |\r\n|
  }
//@[2:3) RightBrace |}|
//@[3:5) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

output myArr array = [
//@[0:6) Identifier |output|
//@[7:12) Identifier |myArr|
//@[13:18) Identifier |array|
//@[19:20) Assignment |=|
//@[21:22) LeftSquare |[|
//@[22:24) NewLine |\r\n|
  'pirates'
//@[2:11) StringComplete |'pirates'|
//@[11:13) NewLine |\r\n|
  'say'
//@[2:7) StringComplete |'say'|
//@[7:9) NewLine |\r\n|
   false ? 'arr2' : 'arr'
//@[3:8) FalseKeyword |false|
//@[9:10) Question |?|
//@[11:17) StringComplete |'arr2'|
//@[18:19) Colon |:|
//@[20:25) StringComplete |'arr'|
//@[25:27) NewLine |\r\n|
]
//@[0:1) RightSquare |]|
//@[1:5) NewLine |\r\n\r\n|

output rgLocation string = resourceGroup().location
//@[0:6) Identifier |output|
//@[7:17) Identifier |rgLocation|
//@[18:24) Identifier |string|
//@[25:26) Assignment |=|
//@[27:40) Identifier |resourceGroup|
//@[40:41) LeftParen |(|
//@[41:42) RightParen |)|
//@[42:43) Dot |.|
//@[43:51) Identifier |location|
//@[51:55) NewLine |\r\n\r\n|

output isWestUs bool = resourceGroup().location != 'westus' ? false : true
//@[0:6) Identifier |output|
//@[7:15) Identifier |isWestUs|
//@[16:20) Identifier |bool|
//@[21:22) Assignment |=|
//@[23:36) Identifier |resourceGroup|
//@[36:37) LeftParen |(|
//@[37:38) RightParen |)|
//@[38:39) Dot |.|
//@[39:47) Identifier |location|
//@[48:50) NotEquals |!=|
//@[51:59) StringComplete |'westus'|
//@[60:61) Question |?|
//@[62:67) FalseKeyword |false|
//@[68:69) Colon |:|
//@[70:74) TrueKeyword |true|
//@[74:78) NewLine |\r\n\r\n|

output expressionBasedIndexer string = {
//@[0:6) Identifier |output|
//@[7:29) Identifier |expressionBasedIndexer|
//@[30:36) Identifier |string|
//@[37:38) Assignment |=|
//@[39:40) LeftBrace |{|
//@[40:42) NewLine |\r\n|
  eastus: {
//@[2:8) Identifier |eastus|
//@[8:9) Colon |:|
//@[10:11) LeftBrace |{|
//@[11:13) NewLine |\r\n|
    foo: true
//@[4:7) Identifier |foo|
//@[7:8) Colon |:|
//@[9:13) TrueKeyword |true|
//@[13:15) NewLine |\r\n|
  }
//@[2:3) RightBrace |}|
//@[3:5) NewLine |\r\n|
  westus: {
//@[2:8) Identifier |westus|
//@[8:9) Colon |:|
//@[10:11) LeftBrace |{|
//@[11:13) NewLine |\r\n|
    foo: false
//@[4:7) Identifier |foo|
//@[7:8) Colon |:|
//@[9:14) FalseKeyword |false|
//@[14:16) NewLine |\r\n|
  }
//@[2:3) RightBrace |}|
//@[3:5) NewLine |\r\n|
}[resourceGroup().location].foo
//@[0:1) RightBrace |}|
//@[1:2) LeftSquare |[|
//@[2:15) Identifier |resourceGroup|
//@[15:16) LeftParen |(|
//@[16:17) RightParen |)|
//@[17:18) Dot |.|
//@[18:26) Identifier |location|
//@[26:27) RightSquare |]|
//@[27:28) Dot |.|
//@[28:31) Identifier |foo|
//@[31:35) NewLine |\r\n\r\n|

var secondaryKeyIntermediateVar = listKeys(resourceId('Mock.RP/type', 'steve'), '2020-01-01').secondaryKey
//@[0:3) Identifier |var|
//@[4:31) Identifier |secondaryKeyIntermediateVar|
//@[32:33) Assignment |=|
//@[34:42) Identifier |listKeys|
//@[42:43) LeftParen |(|
//@[43:53) Identifier |resourceId|
//@[53:54) LeftParen |(|
//@[54:68) StringComplete |'Mock.RP/type'|
//@[68:69) Comma |,|
//@[70:77) StringComplete |'steve'|
//@[77:78) RightParen |)|
//@[78:79) Comma |,|
//@[80:92) StringComplete |'2020-01-01'|
//@[92:93) RightParen |)|
//@[93:94) Dot |.|
//@[94:106) Identifier |secondaryKey|
//@[106:110) NewLine |\r\n\r\n|

output primaryKey string = listKeys(resourceId('Mock.RP/type', 'nigel'), '2020-01-01').primaryKey
//@[0:6) Identifier |output|
//@[7:17) Identifier |primaryKey|
//@[18:24) Identifier |string|
//@[25:26) Assignment |=|
//@[27:35) Identifier |listKeys|
//@[35:36) LeftParen |(|
//@[36:46) Identifier |resourceId|
//@[46:47) LeftParen |(|
//@[47:61) StringComplete |'Mock.RP/type'|
//@[61:62) Comma |,|
//@[63:70) StringComplete |'nigel'|
//@[70:71) RightParen |)|
//@[71:72) Comma |,|
//@[73:85) StringComplete |'2020-01-01'|
//@[85:86) RightParen |)|
//@[86:87) Dot |.|
//@[87:97) Identifier |primaryKey|
//@[97:99) NewLine |\r\n|
output secondaryKey string = secondaryKeyIntermediateVar
//@[0:6) Identifier |output|
//@[7:19) Identifier |secondaryKey|
//@[20:26) Identifier |string|
//@[27:28) Assignment |=|
//@[29:56) Identifier |secondaryKeyIntermediateVar|
//@[56:60) NewLine |\r\n\r\n|

var varWithOverlappingOutput = 'hello'
//@[0:3) Identifier |var|
//@[4:28) Identifier |varWithOverlappingOutput|
//@[29:30) Assignment |=|
//@[31:38) StringComplete |'hello'|
//@[38:40) NewLine |\r\n|
param paramWithOverlappingOutput string
//@[0:5) Identifier |param|
//@[6:32) Identifier |paramWithOverlappingOutput|
//@[33:39) Identifier |string|
//@[39:43) NewLine |\r\n\r\n|

output varWithOverlappingOutput string = varWithOverlappingOutput
//@[0:6) Identifier |output|
//@[7:31) Identifier |varWithOverlappingOutput|
//@[32:38) Identifier |string|
//@[39:40) Assignment |=|
//@[41:65) Identifier |varWithOverlappingOutput|
//@[65:67) NewLine |\r\n|
output paramWithOverlappingOutput string = paramWithOverlappingOutput
//@[0:6) Identifier |output|
//@[7:33) Identifier |paramWithOverlappingOutput|
//@[34:40) Identifier |string|
//@[41:42) Assignment |=|
//@[43:69) Identifier |paramWithOverlappingOutput|
//@[69:73) NewLine |\r\n\r\n|

// top-level output loops are supported
//@[39:41) NewLine |\r\n|
output generatedArray array = [for i in range(0,10): i]
//@[0:6) Identifier |output|
//@[7:21) Identifier |generatedArray|
//@[22:27) Identifier |array|
//@[28:29) Assignment |=|
//@[30:31) LeftSquare |[|
//@[31:34) Identifier |for|
//@[35:36) Identifier |i|
//@[37:39) Identifier |in|
//@[40:45) Identifier |range|
//@[45:46) LeftParen |(|
//@[46:47) Integer |0|
//@[47:48) Comma |,|
//@[48:50) Integer |10|
//@[50:51) RightParen |)|
//@[51:52) Colon |:|
//@[53:54) Identifier |i|
//@[54:55) RightSquare |]|
//@[55:57) NewLine |\r\n|

//@[0:0) EndOfFile ||
