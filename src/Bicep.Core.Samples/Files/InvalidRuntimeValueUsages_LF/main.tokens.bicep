param location string = resourceGroup().location
//@[00:05) Identifier |param|
//@[06:14) Identifier |location|
//@[15:21) Identifier |string|
//@[22:23) Assignment |=|
//@[24:37) Identifier |resourceGroup|
//@[37:38) LeftParen |(|
//@[38:39) RightParen |)|
//@[39:40) Dot |.|
//@[40:48) Identifier |location|
//@[48:50) NewLine |\n\n|

resource foo 'Microsoft.Storage/storageAccounts@2022-09-01' = {
//@[00:08) Identifier |resource|
//@[09:12) Identifier |foo|
//@[13:59) StringComplete |'Microsoft.Storage/storageAccounts@2022-09-01'|
//@[60:61) Assignment |=|
//@[62:63) LeftBrace |{|
//@[63:64) NewLine |\n|
  name: 'foo'
//@[02:06) Identifier |name|
//@[06:07) Colon |:|
//@[08:13) StringComplete |'foo'|
//@[13:14) NewLine |\n|
  location: location
//@[02:10) Identifier |location|
//@[10:11) Colon |:|
//@[12:20) Identifier |location|
//@[20:21) NewLine |\n|
  sku: {
//@[02:05) Identifier |sku|
//@[05:06) Colon |:|
//@[07:08) LeftBrace |{|
//@[08:09) NewLine |\n|
    name: 'Standard_LRS'
//@[04:08) Identifier |name|
//@[08:09) Colon |:|
//@[10:24) StringComplete |'Standard_LRS'|
//@[24:25) NewLine |\n|
  }
//@[02:03) RightBrace |}|
//@[03:04) NewLine |\n|
  kind: 'StorageV2'
//@[02:06) Identifier |kind|
//@[06:07) Colon |:|
//@[08:19) StringComplete |'StorageV2'|
//@[19:20) NewLine |\n|
}
//@[00:01) RightBrace |}|
//@[01:02) NewLine |\n|
resource foos 'Microsoft.Storage/storageAccounts@2022-09-01' = [for i in range(0, 2): {
//@[00:08) Identifier |resource|
//@[09:13) Identifier |foos|
//@[14:60) StringComplete |'Microsoft.Storage/storageAccounts@2022-09-01'|
//@[61:62) Assignment |=|
//@[63:64) LeftSquare |[|
//@[64:67) Identifier |for|
//@[68:69) Identifier |i|
//@[70:72) Identifier |in|
//@[73:78) Identifier |range|
//@[78:79) LeftParen |(|
//@[79:80) Integer |0|
//@[80:81) Comma |,|
//@[82:83) Integer |2|
//@[83:84) RightParen |)|
//@[84:85) Colon |:|
//@[86:87) LeftBrace |{|
//@[87:88) NewLine |\n|
  name: 'foo-${i}'
//@[02:06) Identifier |name|
//@[06:07) Colon |:|
//@[08:15) StringLeftPiece |'foo-${|
//@[15:16) Identifier |i|
//@[16:18) StringRightPiece |}'|
//@[18:19) NewLine |\n|
  location: location
//@[02:10) Identifier |location|
//@[10:11) Colon |:|
//@[12:20) Identifier |location|
//@[20:21) NewLine |\n|
  sku: {
//@[02:05) Identifier |sku|
//@[05:06) Colon |:|
//@[07:08) LeftBrace |{|
//@[08:09) NewLine |\n|
    name: 'Standard_LRS'
//@[04:08) Identifier |name|
//@[08:09) Colon |:|
//@[10:24) StringComplete |'Standard_LRS'|
//@[24:25) NewLine |\n|
  }
//@[02:03) RightBrace |}|
//@[03:04) NewLine |\n|
  kind: 'StorageV2'
//@[02:06) Identifier |kind|
//@[06:07) Colon |:|
//@[08:19) StringComplete |'StorageV2'|
//@[19:20) NewLine |\n|
}]
//@[00:01) RightBrace |}|
//@[01:02) RightSquare |]|
//@[02:03) NewLine |\n|
param strParam string = 'id'
//@[00:05) Identifier |param|
//@[06:14) Identifier |strParam|
//@[15:21) Identifier |string|
//@[22:23) Assignment |=|
//@[24:28) StringComplete |'id'|
//@[28:29) NewLine |\n|
var idAccessor = 'id'
//@[00:03) Identifier |var|
//@[04:14) Identifier |idAccessor|
//@[15:16) Assignment |=|
//@[17:21) StringComplete |'id'|
//@[21:23) NewLine |\n\n|

var varForBodyOk1 = [for i in range(0, 2): foo.id]
//@[00:03) Identifier |var|
//@[04:17) Identifier |varForBodyOk1|
//@[18:19) Assignment |=|
//@[20:21) LeftSquare |[|
//@[21:24) Identifier |for|
//@[25:26) Identifier |i|
//@[27:29) Identifier |in|
//@[30:35) Identifier |range|
//@[35:36) LeftParen |(|
//@[36:37) Integer |0|
//@[37:38) Comma |,|
//@[39:40) Integer |2|
//@[40:41) RightParen |)|
//@[41:42) Colon |:|
//@[43:46) Identifier |foo|
//@[46:47) Dot |.|
//@[47:49) Identifier |id|
//@[49:50) RightSquare |]|
//@[50:51) NewLine |\n|
var varForBodyOk2 = [for i in range(0, 2): foos[0].id]
//@[00:03) Identifier |var|
//@[04:17) Identifier |varForBodyOk2|
//@[18:19) Assignment |=|
//@[20:21) LeftSquare |[|
//@[21:24) Identifier |for|
//@[25:26) Identifier |i|
//@[27:29) Identifier |in|
//@[30:35) Identifier |range|
//@[35:36) LeftParen |(|
//@[36:37) Integer |0|
//@[37:38) Comma |,|
//@[39:40) Integer |2|
//@[40:41) RightParen |)|
//@[41:42) Colon |:|
//@[43:47) Identifier |foos|
//@[47:48) LeftSquare |[|
//@[48:49) Integer |0|
//@[49:50) RightSquare |]|
//@[50:51) Dot |.|
//@[51:53) Identifier |id|
//@[53:54) RightSquare |]|
//@[54:55) NewLine |\n|
var varForBodyOk3 = [for i in range(0, 2): foos[i].id]
//@[00:03) Identifier |var|
//@[04:17) Identifier |varForBodyOk3|
//@[18:19) Assignment |=|
//@[20:21) LeftSquare |[|
//@[21:24) Identifier |for|
//@[25:26) Identifier |i|
//@[27:29) Identifier |in|
//@[30:35) Identifier |range|
//@[35:36) LeftParen |(|
//@[36:37) Integer |0|
//@[37:38) Comma |,|
//@[39:40) Integer |2|
//@[40:41) RightParen |)|
//@[41:42) Colon |:|
//@[43:47) Identifier |foos|
//@[47:48) LeftSquare |[|
//@[48:49) Identifier |i|
//@[49:50) RightSquare |]|
//@[50:51) Dot |.|
//@[51:53) Identifier |id|
//@[53:54) RightSquare |]|
//@[54:55) NewLine |\n|
var varForBodyOk4 = [for i in range(0, 2): foo[idAccessor]]
//@[00:03) Identifier |var|
//@[04:17) Identifier |varForBodyOk4|
//@[18:19) Assignment |=|
//@[20:21) LeftSquare |[|
//@[21:24) Identifier |for|
//@[25:26) Identifier |i|
//@[27:29) Identifier |in|
//@[30:35) Identifier |range|
//@[35:36) LeftParen |(|
//@[36:37) Integer |0|
//@[37:38) Comma |,|
//@[39:40) Integer |2|
//@[40:41) RightParen |)|
//@[41:42) Colon |:|
//@[43:46) Identifier |foo|
//@[46:47) LeftSquare |[|
//@[47:57) Identifier |idAccessor|
//@[57:58) RightSquare |]|
//@[58:59) RightSquare |]|
//@[59:60) NewLine |\n|
var varForBodyBad1 = [for i in range(0, 2): foo.properties]
//@[00:03) Identifier |var|
//@[04:18) Identifier |varForBodyBad1|
//@[19:20) Assignment |=|
//@[21:22) LeftSquare |[|
//@[22:25) Identifier |for|
//@[26:27) Identifier |i|
//@[28:30) Identifier |in|
//@[31:36) Identifier |range|
//@[36:37) LeftParen |(|
//@[37:38) Integer |0|
//@[38:39) Comma |,|
//@[40:41) Integer |2|
//@[41:42) RightParen |)|
//@[42:43) Colon |:|
//@[44:47) Identifier |foo|
//@[47:48) Dot |.|
//@[48:58) Identifier |properties|
//@[58:59) RightSquare |]|
//@[59:60) NewLine |\n|
var varForBodyBad2 = [for i in range(0, 2): foos[0].properties]
//@[00:03) Identifier |var|
//@[04:18) Identifier |varForBodyBad2|
//@[19:20) Assignment |=|
//@[21:22) LeftSquare |[|
//@[22:25) Identifier |for|
//@[26:27) Identifier |i|
//@[28:30) Identifier |in|
//@[31:36) Identifier |range|
//@[36:37) LeftParen |(|
//@[37:38) Integer |0|
//@[38:39) Comma |,|
//@[40:41) Integer |2|
//@[41:42) RightParen |)|
//@[42:43) Colon |:|
//@[44:48) Identifier |foos|
//@[48:49) LeftSquare |[|
//@[49:50) Integer |0|
//@[50:51) RightSquare |]|
//@[51:52) Dot |.|
//@[52:62) Identifier |properties|
//@[62:63) RightSquare |]|
//@[63:64) NewLine |\n|
var varForBodyBad3 = [for i in range(0, 2): {
//@[00:03) Identifier |var|
//@[04:18) Identifier |varForBodyBad3|
//@[19:20) Assignment |=|
//@[21:22) LeftSquare |[|
//@[22:25) Identifier |for|
//@[26:27) Identifier |i|
//@[28:30) Identifier |in|
//@[31:36) Identifier |range|
//@[36:37) LeftParen |(|
//@[37:38) Integer |0|
//@[38:39) Comma |,|
//@[40:41) Integer |2|
//@[41:42) RightParen |)|
//@[42:43) Colon |:|
//@[44:45) LeftBrace |{|
//@[45:46) NewLine |\n|
  prop: foos[0].properties
//@[02:06) Identifier |prop|
//@[06:07) Colon |:|
//@[08:12) Identifier |foos|
//@[12:13) LeftSquare |[|
//@[13:14) Integer |0|
//@[14:15) RightSquare |]|
//@[15:16) Dot |.|
//@[16:26) Identifier |properties|
//@[26:27) NewLine |\n|
}]
//@[00:01) RightBrace |}|
//@[01:02) RightSquare |]|
//@[02:03) NewLine |\n|
var varForBodyBad4 = [for i in range(0, 2): foos[i].properties.accessTier]
//@[00:03) Identifier |var|
//@[04:18) Identifier |varForBodyBad4|
//@[19:20) Assignment |=|
//@[21:22) LeftSquare |[|
//@[22:25) Identifier |for|
//@[26:27) Identifier |i|
//@[28:30) Identifier |in|
//@[31:36) Identifier |range|
//@[36:37) LeftParen |(|
//@[37:38) Integer |0|
//@[38:39) Comma |,|
//@[40:41) Integer |2|
//@[41:42) RightParen |)|
//@[42:43) Colon |:|
//@[44:48) Identifier |foos|
//@[48:49) LeftSquare |[|
//@[49:50) Identifier |i|
//@[50:51) RightSquare |]|
//@[51:52) Dot |.|
//@[52:62) Identifier |properties|
//@[62:63) Dot |.|
//@[63:73) Identifier |accessTier|
//@[73:74) RightSquare |]|
//@[74:75) NewLine |\n|
var varForBodyBad5 = [for i in range(0, 2): foo[strParam]]
//@[00:03) Identifier |var|
//@[04:18) Identifier |varForBodyBad5|
//@[19:20) Assignment |=|
//@[21:22) LeftSquare |[|
//@[22:25) Identifier |for|
//@[26:27) Identifier |i|
//@[28:30) Identifier |in|
//@[31:36) Identifier |range|
//@[36:37) LeftParen |(|
//@[37:38) Integer |0|
//@[38:39) Comma |,|
//@[40:41) Integer |2|
//@[41:42) RightParen |)|
//@[42:43) Colon |:|
//@[44:47) Identifier |foo|
//@[47:48) LeftSquare |[|
//@[48:56) Identifier |strParam|
//@[56:57) RightSquare |]|
//@[57:58) RightSquare |]|
//@[58:59) NewLine |\n|

//@[00:00) EndOfFile ||
