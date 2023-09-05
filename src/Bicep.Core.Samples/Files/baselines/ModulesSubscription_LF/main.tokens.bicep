targetScope = 'subscription'
//@[00:11) Identifier |targetScope|
//@[12:13) Assignment |=|
//@[14:28) StringComplete |'subscription'|
//@[28:30) NewLine |\n\n|

param prefix string = 'majastrz'
//@[00:05) Identifier |param|
//@[06:12) Identifier |prefix|
//@[13:19) Identifier |string|
//@[20:21) Assignment |=|
//@[22:32) StringComplete |'majastrz'|
//@[32:33) NewLine |\n|
var groups = [
//@[00:03) Identifier |var|
//@[04:10) Identifier |groups|
//@[11:12) Assignment |=|
//@[13:14) LeftSquare |[|
//@[14:15) NewLine |\n|
  'bicep1'
//@[02:10) StringComplete |'bicep1'|
//@[10:11) NewLine |\n|
  'bicep2'
//@[02:10) StringComplete |'bicep2'|
//@[10:11) NewLine |\n|
  'bicep3'
//@[02:10) StringComplete |'bicep3'|
//@[10:11) NewLine |\n|
  'bicep4'
//@[02:10) StringComplete |'bicep4'|
//@[10:11) NewLine |\n|
]
//@[00:01) RightSquare |]|
//@[01:03) NewLine |\n\n|

var scripts = take(groups, 2)
//@[00:03) Identifier |var|
//@[04:11) Identifier |scripts|
//@[12:13) Assignment |=|
//@[14:18) Identifier |take|
//@[18:19) LeftParen |(|
//@[19:25) Identifier |groups|
//@[25:26) Comma |,|
//@[27:28) Integer |2|
//@[28:29) RightParen |)|
//@[29:31) NewLine |\n\n|

resource resourceGroups 'Microsoft.Resources/resourceGroups@2020-06-01' = [for name in groups: {
//@[00:08) Identifier |resource|
//@[09:23) Identifier |resourceGroups|
//@[24:71) StringComplete |'Microsoft.Resources/resourceGroups@2020-06-01'|
//@[72:73) Assignment |=|
//@[74:75) LeftSquare |[|
//@[75:78) Identifier |for|
//@[79:83) Identifier |name|
//@[84:86) Identifier |in|
//@[87:93) Identifier |groups|
//@[93:94) Colon |:|
//@[95:96) LeftBrace |{|
//@[96:97) NewLine |\n|
  name: '${prefix}-${name}'
//@[02:06) Identifier |name|
//@[06:07) Colon |:|
//@[08:11) StringLeftPiece |'${|
//@[11:17) Identifier |prefix|
//@[17:21) StringMiddlePiece |}-${|
//@[21:25) Identifier |name|
//@[25:27) StringRightPiece |}'|
//@[27:28) NewLine |\n|
  location: 'westus'
//@[02:10) Identifier |location|
//@[10:11) Colon |:|
//@[12:20) StringComplete |'westus'|
//@[20:21) NewLine |\n|
}]
//@[00:01) RightBrace |}|
//@[01:02) RightSquare |]|
//@[02:04) NewLine |\n\n|

module scopedToSymbolicName 'hello.bicep' = [for (name, i) in scripts: {
//@[00:06) Identifier |module|
//@[07:27) Identifier |scopedToSymbolicName|
//@[28:41) StringComplete |'hello.bicep'|
//@[42:43) Assignment |=|
//@[44:45) LeftSquare |[|
//@[45:48) Identifier |for|
//@[49:50) LeftParen |(|
//@[50:54) Identifier |name|
//@[54:55) Comma |,|
//@[56:57) Identifier |i|
//@[57:58) RightParen |)|
//@[59:61) Identifier |in|
//@[62:69) Identifier |scripts|
//@[69:70) Colon |:|
//@[71:72) LeftBrace |{|
//@[72:73) NewLine |\n|
  name: '${prefix}-dep-${i}'
//@[02:06) Identifier |name|
//@[06:07) Colon |:|
//@[08:11) StringLeftPiece |'${|
//@[11:17) Identifier |prefix|
//@[17:25) StringMiddlePiece |}-dep-${|
//@[25:26) Identifier |i|
//@[26:28) StringRightPiece |}'|
//@[28:29) NewLine |\n|
  params: {
//@[02:08) Identifier |params|
//@[08:09) Colon |:|
//@[10:11) LeftBrace |{|
//@[11:12) NewLine |\n|
    scriptName: 'test-${name}-${i}'
//@[04:14) Identifier |scriptName|
//@[14:15) Colon |:|
//@[16:24) StringLeftPiece |'test-${|
//@[24:28) Identifier |name|
//@[28:32) StringMiddlePiece |}-${|
//@[32:33) Identifier |i|
//@[33:35) StringRightPiece |}'|
//@[35:36) NewLine |\n|
  }
//@[02:03) RightBrace |}|
//@[03:04) NewLine |\n|
  scope: resourceGroups[i]
//@[02:07) Identifier |scope|
//@[07:08) Colon |:|
//@[09:23) Identifier |resourceGroups|
//@[23:24) LeftSquare |[|
//@[24:25) Identifier |i|
//@[25:26) RightSquare |]|
//@[26:27) NewLine |\n|
}]
//@[00:01) RightBrace |}|
//@[01:02) RightSquare |]|
//@[02:04) NewLine |\n\n|

module scopedToResourceGroupFunction 'hello.bicep' = [for (name, i) in scripts: {
//@[00:06) Identifier |module|
//@[07:36) Identifier |scopedToResourceGroupFunction|
//@[37:50) StringComplete |'hello.bicep'|
//@[51:52) Assignment |=|
//@[53:54) LeftSquare |[|
//@[54:57) Identifier |for|
//@[58:59) LeftParen |(|
//@[59:63) Identifier |name|
//@[63:64) Comma |,|
//@[65:66) Identifier |i|
//@[66:67) RightParen |)|
//@[68:70) Identifier |in|
//@[71:78) Identifier |scripts|
//@[78:79) Colon |:|
//@[80:81) LeftBrace |{|
//@[81:82) NewLine |\n|
  name: '${prefix}-dep-${i}'
//@[02:06) Identifier |name|
//@[06:07) Colon |:|
//@[08:11) StringLeftPiece |'${|
//@[11:17) Identifier |prefix|
//@[17:25) StringMiddlePiece |}-dep-${|
//@[25:26) Identifier |i|
//@[26:28) StringRightPiece |}'|
//@[28:29) NewLine |\n|
  params: {
//@[02:08) Identifier |params|
//@[08:09) Colon |:|
//@[10:11) LeftBrace |{|
//@[11:12) NewLine |\n|
    scriptName: 'test-${name}-${i}'
//@[04:14) Identifier |scriptName|
//@[14:15) Colon |:|
//@[16:24) StringLeftPiece |'test-${|
//@[24:28) Identifier |name|
//@[28:32) StringMiddlePiece |}-${|
//@[32:33) Identifier |i|
//@[33:35) StringRightPiece |}'|
//@[35:36) NewLine |\n|
  }
//@[02:03) RightBrace |}|
//@[03:04) NewLine |\n|
  scope: resourceGroup(concat(name, '-extra'))
//@[02:07) Identifier |scope|
//@[07:08) Colon |:|
//@[09:22) Identifier |resourceGroup|
//@[22:23) LeftParen |(|
//@[23:29) Identifier |concat|
//@[29:30) LeftParen |(|
//@[30:34) Identifier |name|
//@[34:35) Comma |,|
//@[36:44) StringComplete |'-extra'|
//@[44:45) RightParen |)|
//@[45:46) RightParen |)|
//@[46:47) NewLine |\n|
}]
//@[00:01) RightBrace |}|
//@[01:02) RightSquare |]|
//@[02:04) NewLine |\n\n|


//@[00:00) EndOfFile ||
