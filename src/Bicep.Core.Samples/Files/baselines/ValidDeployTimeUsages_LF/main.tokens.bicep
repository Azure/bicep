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
  location: 'westus'
//@[02:10) Identifier |location|
//@[10:11) Colon |:|
//@[12:20) StringComplete |'westus'|
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
//@[19:21) NewLine |\n\n|

  resource fooChild 'fileServices' = {
//@[02:10) Identifier |resource|
//@[11:19) Identifier |fooChild|
//@[20:34) StringComplete |'fileServices'|
//@[35:36) Assignment |=|
//@[37:38) LeftBrace |{|
//@[38:39) NewLine |\n|
    name: 'default'
//@[04:08) Identifier |name|
//@[08:09) Colon |:|
//@[10:19) StringComplete |'default'|
//@[19:20) NewLine |\n|
  }
//@[02:03) RightBrace |}|
//@[03:04) NewLine |\n|
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
  location: 'westus'
//@[02:10) Identifier |location|
//@[10:11) Colon |:|
//@[12:20) StringComplete |'westus'|
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
resource existingFoo 'Microsoft.Storage/storageAccounts@2022-09-01' existing = {
//@[00:08) Identifier |resource|
//@[09:20) Identifier |existingFoo|
//@[21:67) StringComplete |'Microsoft.Storage/storageAccounts@2022-09-01'|
//@[68:76) Identifier |existing|
//@[77:78) Assignment |=|
//@[79:80) LeftBrace |{|
//@[80:81) NewLine |\n|
  name: 'existingFoo'
//@[02:06) Identifier |name|
//@[06:07) Colon |:|
//@[08:21) StringComplete |'existingFoo'|
//@[21:22) NewLine |\n|
}
//@[00:01) RightBrace |}|
//@[01:03) NewLine |\n\n|

param cond bool = false
//@[00:05) Identifier |param|
//@[06:10) Identifier |cond|
//@[11:15) Identifier |bool|
//@[16:17) Assignment |=|
//@[18:23) FalseKeyword |false|
//@[23:25) NewLine |\n\n|

var zeroIndex = 0
//@[00:03) Identifier |var|
//@[04:13) Identifier |zeroIndex|
//@[14:15) Assignment |=|
//@[16:17) Integer |0|
//@[17:18) NewLine |\n|
var otherIndex = zeroIndex + 2
//@[00:03) Identifier |var|
//@[04:14) Identifier |otherIndex|
//@[15:16) Assignment |=|
//@[17:26) Identifier |zeroIndex|
//@[27:28) Plus |+|
//@[29:30) Integer |2|
//@[30:31) NewLine |\n|
var idAccessor = 'id'
//@[00:03) Identifier |var|
//@[04:14) Identifier |idAccessor|
//@[15:16) Assignment |=|
//@[17:21) StringComplete |'id'|
//@[21:22) NewLine |\n|
var dStr = 'd'
//@[00:03) Identifier |var|
//@[04:08) Identifier |dStr|
//@[09:10) Assignment |=|
//@[11:14) StringComplete |'d'|
//@[14:15) NewLine |\n|
var idAccessor2 = idAccessor
//@[00:03) Identifier |var|
//@[04:15) Identifier |idAccessor2|
//@[16:17) Assignment |=|
//@[18:28) Identifier |idAccessor|
//@[28:29) NewLine |\n|
var idAccessorInterpolated = '${idAccessor}'
//@[00:03) Identifier |var|
//@[04:26) Identifier |idAccessorInterpolated|
//@[27:28) Assignment |=|
//@[29:32) StringLeftPiece |'${|
//@[32:42) Identifier |idAccessor|
//@[42:44) StringRightPiece |}'|
//@[44:45) NewLine |\n|
var idAccessorMixed = 'i${dStr}'
//@[00:03) Identifier |var|
//@[04:19) Identifier |idAccessorMixed|
//@[20:21) Assignment |=|
//@[22:26) StringLeftPiece |'i${|
//@[26:30) Identifier |dStr|
//@[30:32) StringRightPiece |}'|
//@[32:33) NewLine |\n|
var strArray = ['id', 'properties']
//@[00:03) Identifier |var|
//@[04:12) Identifier |strArray|
//@[13:14) Assignment |=|
//@[15:16) LeftSquare |[|
//@[16:20) StringComplete |'id'|
//@[20:21) Comma |,|
//@[22:34) StringComplete |'properties'|
//@[34:35) RightSquare |]|
//@[35:37) NewLine |\n\n|

var varForBodyOkDeployTimeUsages = [for i in range(0, 2): {
//@[00:03) Identifier |var|
//@[04:32) Identifier |varForBodyOkDeployTimeUsages|
//@[33:34) Assignment |=|
//@[35:36) LeftSquare |[|
//@[36:39) Identifier |for|
//@[40:41) Identifier |i|
//@[42:44) Identifier |in|
//@[45:50) Identifier |range|
//@[50:51) LeftParen |(|
//@[51:52) Integer |0|
//@[52:53) Comma |,|
//@[54:55) Integer |2|
//@[55:56) RightParen |)|
//@[56:57) Colon |:|
//@[58:59) LeftBrace |{|
//@[59:60) NewLine |\n|
  case1: foo.id
//@[02:07) Identifier |case1|
//@[07:08) Colon |:|
//@[09:12) Identifier |foo|
//@[12:13) Dot |.|
//@[13:15) Identifier |id|
//@[15:16) NewLine |\n|
  case2: existingFoo.id
//@[02:07) Identifier |case2|
//@[07:08) Colon |:|
//@[09:20) Identifier |existingFoo|
//@[20:21) Dot |.|
//@[21:23) Identifier |id|
//@[23:24) NewLine |\n|
  case3: foo::fooChild.id
//@[02:07) Identifier |case3|
//@[07:08) Colon |:|
//@[09:12) Identifier |foo|
//@[12:14) DoubleColon |::|
//@[14:22) Identifier |fooChild|
//@[22:23) Dot |.|
//@[23:25) Identifier |id|
//@[25:26) NewLine |\n|
  case4: foos[0].id
//@[02:07) Identifier |case4|
//@[07:08) Colon |:|
//@[09:13) Identifier |foos|
//@[13:14) LeftSquare |[|
//@[14:15) Integer |0|
//@[15:16) RightSquare |]|
//@[16:17) Dot |.|
//@[17:19) Identifier |id|
//@[19:20) NewLine |\n|
  case5: foos[i].id
//@[02:07) Identifier |case5|
//@[07:08) Colon |:|
//@[09:13) Identifier |foos|
//@[13:14) LeftSquare |[|
//@[14:15) Identifier |i|
//@[15:16) RightSquare |]|
//@[16:17) Dot |.|
//@[17:19) Identifier |id|
//@[19:20) NewLine |\n|
  case6: foos[i + 2].id
//@[02:07) Identifier |case6|
//@[07:08) Colon |:|
//@[09:13) Identifier |foos|
//@[13:14) LeftSquare |[|
//@[14:15) Identifier |i|
//@[16:17) Plus |+|
//@[18:19) Integer |2|
//@[19:20) RightSquare |]|
//@[20:21) Dot |.|
//@[21:23) Identifier |id|
//@[23:24) NewLine |\n|
  case7: foos[zeroIndex].id
//@[02:07) Identifier |case7|
//@[07:08) Colon |:|
//@[09:13) Identifier |foos|
//@[13:14) LeftSquare |[|
//@[14:23) Identifier |zeroIndex|
//@[23:24) RightSquare |]|
//@[24:25) Dot |.|
//@[25:27) Identifier |id|
//@[27:28) NewLine |\n|
  case8: foos[otherIndex].id
//@[02:07) Identifier |case8|
//@[07:08) Colon |:|
//@[09:13) Identifier |foos|
//@[13:14) LeftSquare |[|
//@[14:24) Identifier |otherIndex|
//@[24:25) RightSquare |]|
//@[25:26) Dot |.|
//@[26:28) Identifier |id|
//@[28:29) NewLine |\n|
  case9: foo['id']
//@[02:07) Identifier |case9|
//@[07:08) Colon |:|
//@[09:12) Identifier |foo|
//@[12:13) LeftSquare |[|
//@[13:17) StringComplete |'id'|
//@[17:18) RightSquare |]|
//@[18:19) NewLine |\n|
  case10: existingFoo['id']
//@[02:08) Identifier |case10|
//@[08:09) Colon |:|
//@[10:21) Identifier |existingFoo|
//@[21:22) LeftSquare |[|
//@[22:26) StringComplete |'id'|
//@[26:27) RightSquare |]|
//@[27:28) NewLine |\n|
  case11: foo::fooChild['id']
//@[02:08) Identifier |case11|
//@[08:09) Colon |:|
//@[10:13) Identifier |foo|
//@[13:15) DoubleColon |::|
//@[15:23) Identifier |fooChild|
//@[23:24) LeftSquare |[|
//@[24:28) StringComplete |'id'|
//@[28:29) RightSquare |]|
//@[29:30) NewLine |\n|
  case12: foos[0]['id']
//@[02:08) Identifier |case12|
//@[08:09) Colon |:|
//@[10:14) Identifier |foos|
//@[14:15) LeftSquare |[|
//@[15:16) Integer |0|
//@[16:17) RightSquare |]|
//@[17:18) LeftSquare |[|
//@[18:22) StringComplete |'id'|
//@[22:23) RightSquare |]|
//@[23:24) NewLine |\n|
  case13: foos[i]['id']
//@[02:08) Identifier |case13|
//@[08:09) Colon |:|
//@[10:14) Identifier |foos|
//@[14:15) LeftSquare |[|
//@[15:16) Identifier |i|
//@[16:17) RightSquare |]|
//@[17:18) LeftSquare |[|
//@[18:22) StringComplete |'id'|
//@[22:23) RightSquare |]|
//@[23:24) NewLine |\n|
  case14: foos[i + 2]['id']
//@[02:08) Identifier |case14|
//@[08:09) Colon |:|
//@[10:14) Identifier |foos|
//@[14:15) LeftSquare |[|
//@[15:16) Identifier |i|
//@[17:18) Plus |+|
//@[19:20) Integer |2|
//@[20:21) RightSquare |]|
//@[21:22) LeftSquare |[|
//@[22:26) StringComplete |'id'|
//@[26:27) RightSquare |]|
//@[27:28) NewLine |\n|
  case15: foos[zeroIndex]['id']
//@[02:08) Identifier |case15|
//@[08:09) Colon |:|
//@[10:14) Identifier |foos|
//@[14:15) LeftSquare |[|
//@[15:24) Identifier |zeroIndex|
//@[24:25) RightSquare |]|
//@[25:26) LeftSquare |[|
//@[26:30) StringComplete |'id'|
//@[30:31) RightSquare |]|
//@[31:32) NewLine |\n|
  case16: foos[otherIndex]['id']
//@[02:08) Identifier |case16|
//@[08:09) Colon |:|
//@[10:14) Identifier |foos|
//@[14:15) LeftSquare |[|
//@[15:25) Identifier |otherIndex|
//@[25:26) RightSquare |]|
//@[26:27) LeftSquare |[|
//@[27:31) StringComplete |'id'|
//@[31:32) RightSquare |]|
//@[32:33) NewLine |\n|
  case17: foo[idAccessor]
//@[02:08) Identifier |case17|
//@[08:09) Colon |:|
//@[10:13) Identifier |foo|
//@[13:14) LeftSquare |[|
//@[14:24) Identifier |idAccessor|
//@[24:25) RightSquare |]|
//@[25:26) NewLine |\n|
  case18: existingFoo[idAccessor]
//@[02:08) Identifier |case18|
//@[08:09) Colon |:|
//@[10:21) Identifier |existingFoo|
//@[21:22) LeftSquare |[|
//@[22:32) Identifier |idAccessor|
//@[32:33) RightSquare |]|
//@[33:34) NewLine |\n|
  case19: foo::fooChild[idAccessor]
//@[02:08) Identifier |case19|
//@[08:09) Colon |:|
//@[10:13) Identifier |foo|
//@[13:15) DoubleColon |::|
//@[15:23) Identifier |fooChild|
//@[23:24) LeftSquare |[|
//@[24:34) Identifier |idAccessor|
//@[34:35) RightSquare |]|
//@[35:36) NewLine |\n|
  case20: foos[0][idAccessor]
//@[02:08) Identifier |case20|
//@[08:09) Colon |:|
//@[10:14) Identifier |foos|
//@[14:15) LeftSquare |[|
//@[15:16) Integer |0|
//@[16:17) RightSquare |]|
//@[17:18) LeftSquare |[|
//@[18:28) Identifier |idAccessor|
//@[28:29) RightSquare |]|
//@[29:30) NewLine |\n|
  case21: foos[i][idAccessor]
//@[02:08) Identifier |case21|
//@[08:09) Colon |:|
//@[10:14) Identifier |foos|
//@[14:15) LeftSquare |[|
//@[15:16) Identifier |i|
//@[16:17) RightSquare |]|
//@[17:18) LeftSquare |[|
//@[18:28) Identifier |idAccessor|
//@[28:29) RightSquare |]|
//@[29:30) NewLine |\n|
  case22: foos[i + 2][idAccessor]
//@[02:08) Identifier |case22|
//@[08:09) Colon |:|
//@[10:14) Identifier |foos|
//@[14:15) LeftSquare |[|
//@[15:16) Identifier |i|
//@[17:18) Plus |+|
//@[19:20) Integer |2|
//@[20:21) RightSquare |]|
//@[21:22) LeftSquare |[|
//@[22:32) Identifier |idAccessor|
//@[32:33) RightSquare |]|
//@[33:34) NewLine |\n|
  case23: foos[zeroIndex][idAccessor]
//@[02:08) Identifier |case23|
//@[08:09) Colon |:|
//@[10:14) Identifier |foos|
//@[14:15) LeftSquare |[|
//@[15:24) Identifier |zeroIndex|
//@[24:25) RightSquare |]|
//@[25:26) LeftSquare |[|
//@[26:36) Identifier |idAccessor|
//@[36:37) RightSquare |]|
//@[37:38) NewLine |\n|
  case24: foos[otherIndex][idAccessor]
//@[02:08) Identifier |case24|
//@[08:09) Colon |:|
//@[10:14) Identifier |foos|
//@[14:15) LeftSquare |[|
//@[15:25) Identifier |otherIndex|
//@[25:26) RightSquare |]|
//@[26:27) LeftSquare |[|
//@[27:37) Identifier |idAccessor|
//@[37:38) RightSquare |]|
//@[38:39) NewLine |\n|
  case25: foo[idAccessor2]
//@[02:08) Identifier |case25|
//@[08:09) Colon |:|
//@[10:13) Identifier |foo|
//@[13:14) LeftSquare |[|
//@[14:25) Identifier |idAccessor2|
//@[25:26) RightSquare |]|
//@[26:27) NewLine |\n|
  case26: existingFoo[idAccessor2]
//@[02:08) Identifier |case26|
//@[08:09) Colon |:|
//@[10:21) Identifier |existingFoo|
//@[21:22) LeftSquare |[|
//@[22:33) Identifier |idAccessor2|
//@[33:34) RightSquare |]|
//@[34:35) NewLine |\n|
  case27: foo::fooChild[idAccessor2]
//@[02:08) Identifier |case27|
//@[08:09) Colon |:|
//@[10:13) Identifier |foo|
//@[13:15) DoubleColon |::|
//@[15:23) Identifier |fooChild|
//@[23:24) LeftSquare |[|
//@[24:35) Identifier |idAccessor2|
//@[35:36) RightSquare |]|
//@[36:37) NewLine |\n|
  case28: foos[0][idAccessor2]
//@[02:08) Identifier |case28|
//@[08:09) Colon |:|
//@[10:14) Identifier |foos|
//@[14:15) LeftSquare |[|
//@[15:16) Integer |0|
//@[16:17) RightSquare |]|
//@[17:18) LeftSquare |[|
//@[18:29) Identifier |idAccessor2|
//@[29:30) RightSquare |]|
//@[30:31) NewLine |\n|
  case29: foos[i][idAccessor2]
//@[02:08) Identifier |case29|
//@[08:09) Colon |:|
//@[10:14) Identifier |foos|
//@[14:15) LeftSquare |[|
//@[15:16) Identifier |i|
//@[16:17) RightSquare |]|
//@[17:18) LeftSquare |[|
//@[18:29) Identifier |idAccessor2|
//@[29:30) RightSquare |]|
//@[30:31) NewLine |\n|
  case30: foos[i + 2][idAccessor2]
//@[02:08) Identifier |case30|
//@[08:09) Colon |:|
//@[10:14) Identifier |foos|
//@[14:15) LeftSquare |[|
//@[15:16) Identifier |i|
//@[17:18) Plus |+|
//@[19:20) Integer |2|
//@[20:21) RightSquare |]|
//@[21:22) LeftSquare |[|
//@[22:33) Identifier |idAccessor2|
//@[33:34) RightSquare |]|
//@[34:35) NewLine |\n|
  case31: foos[zeroIndex][idAccessor2]
//@[02:08) Identifier |case31|
//@[08:09) Colon |:|
//@[10:14) Identifier |foos|
//@[14:15) LeftSquare |[|
//@[15:24) Identifier |zeroIndex|
//@[24:25) RightSquare |]|
//@[25:26) LeftSquare |[|
//@[26:37) Identifier |idAccessor2|
//@[37:38) RightSquare |]|
//@[38:39) NewLine |\n|
  case32: foos[otherIndex][idAccessor2]
//@[02:08) Identifier |case32|
//@[08:09) Colon |:|
//@[10:14) Identifier |foos|
//@[14:15) LeftSquare |[|
//@[15:25) Identifier |otherIndex|
//@[25:26) RightSquare |]|
//@[26:27) LeftSquare |[|
//@[27:38) Identifier |idAccessor2|
//@[38:39) RightSquare |]|
//@[39:40) NewLine |\n|
  case33: foo['${'id'}']
//@[02:08) Identifier |case33|
//@[08:09) Colon |:|
//@[10:13) Identifier |foo|
//@[13:14) LeftSquare |[|
//@[14:17) StringLeftPiece |'${|
//@[17:21) StringComplete |'id'|
//@[21:23) StringRightPiece |}'|
//@[23:24) RightSquare |]|
//@[24:25) NewLine |\n|
  case34: existingFoo['${'id'}']
//@[02:08) Identifier |case34|
//@[08:09) Colon |:|
//@[10:21) Identifier |existingFoo|
//@[21:22) LeftSquare |[|
//@[22:25) StringLeftPiece |'${|
//@[25:29) StringComplete |'id'|
//@[29:31) StringRightPiece |}'|
//@[31:32) RightSquare |]|
//@[32:33) NewLine |\n|
  case35: foo::fooChild['${'id'}']
//@[02:08) Identifier |case35|
//@[08:09) Colon |:|
//@[10:13) Identifier |foo|
//@[13:15) DoubleColon |::|
//@[15:23) Identifier |fooChild|
//@[23:24) LeftSquare |[|
//@[24:27) StringLeftPiece |'${|
//@[27:31) StringComplete |'id'|
//@[31:33) StringRightPiece |}'|
//@[33:34) RightSquare |]|
//@[34:35) NewLine |\n|
  case36: foos[0]['${'id'}']
//@[02:08) Identifier |case36|
//@[08:09) Colon |:|
//@[10:14) Identifier |foos|
//@[14:15) LeftSquare |[|
//@[15:16) Integer |0|
//@[16:17) RightSquare |]|
//@[17:18) LeftSquare |[|
//@[18:21) StringLeftPiece |'${|
//@[21:25) StringComplete |'id'|
//@[25:27) StringRightPiece |}'|
//@[27:28) RightSquare |]|
//@[28:29) NewLine |\n|
  case37: foos[i]['${'id'}']
//@[02:08) Identifier |case37|
//@[08:09) Colon |:|
//@[10:14) Identifier |foos|
//@[14:15) LeftSquare |[|
//@[15:16) Identifier |i|
//@[16:17) RightSquare |]|
//@[17:18) LeftSquare |[|
//@[18:21) StringLeftPiece |'${|
//@[21:25) StringComplete |'id'|
//@[25:27) StringRightPiece |}'|
//@[27:28) RightSquare |]|
//@[28:29) NewLine |\n|
  case38: foos[i + 2]['${'id'}']
//@[02:08) Identifier |case38|
//@[08:09) Colon |:|
//@[10:14) Identifier |foos|
//@[14:15) LeftSquare |[|
//@[15:16) Identifier |i|
//@[17:18) Plus |+|
//@[19:20) Integer |2|
//@[20:21) RightSquare |]|
//@[21:22) LeftSquare |[|
//@[22:25) StringLeftPiece |'${|
//@[25:29) StringComplete |'id'|
//@[29:31) StringRightPiece |}'|
//@[31:32) RightSquare |]|
//@[32:33) NewLine |\n|
  case39: foos[zeroIndex]['${'id'}']
//@[02:08) Identifier |case39|
//@[08:09) Colon |:|
//@[10:14) Identifier |foos|
//@[14:15) LeftSquare |[|
//@[15:24) Identifier |zeroIndex|
//@[24:25) RightSquare |]|
//@[25:26) LeftSquare |[|
//@[26:29) StringLeftPiece |'${|
//@[29:33) StringComplete |'id'|
//@[33:35) StringRightPiece |}'|
//@[35:36) RightSquare |]|
//@[36:37) NewLine |\n|
  case40: foos[otherIndex]['${'id'}']
//@[02:08) Identifier |case40|
//@[08:09) Colon |:|
//@[10:14) Identifier |foos|
//@[14:15) LeftSquare |[|
//@[15:25) Identifier |otherIndex|
//@[25:26) RightSquare |]|
//@[26:27) LeftSquare |[|
//@[27:30) StringLeftPiece |'${|
//@[30:34) StringComplete |'id'|
//@[34:36) StringRightPiece |}'|
//@[36:37) RightSquare |]|
//@[37:38) NewLine |\n|
  case41: foo[idAccessorInterpolated]
//@[02:08) Identifier |case41|
//@[08:09) Colon |:|
//@[10:13) Identifier |foo|
//@[13:14) LeftSquare |[|
//@[14:36) Identifier |idAccessorInterpolated|
//@[36:37) RightSquare |]|
//@[37:38) NewLine |\n|
  case42: existingFoo[idAccessorInterpolated]
//@[02:08) Identifier |case42|
//@[08:09) Colon |:|
//@[10:21) Identifier |existingFoo|
//@[21:22) LeftSquare |[|
//@[22:44) Identifier |idAccessorInterpolated|
//@[44:45) RightSquare |]|
//@[45:46) NewLine |\n|
  case43: foo::fooChild[idAccessorInterpolated]
//@[02:08) Identifier |case43|
//@[08:09) Colon |:|
//@[10:13) Identifier |foo|
//@[13:15) DoubleColon |::|
//@[15:23) Identifier |fooChild|
//@[23:24) LeftSquare |[|
//@[24:46) Identifier |idAccessorInterpolated|
//@[46:47) RightSquare |]|
//@[47:48) NewLine |\n|
  case44: foos[0][idAccessorInterpolated]
//@[02:08) Identifier |case44|
//@[08:09) Colon |:|
//@[10:14) Identifier |foos|
//@[14:15) LeftSquare |[|
//@[15:16) Integer |0|
//@[16:17) RightSquare |]|
//@[17:18) LeftSquare |[|
//@[18:40) Identifier |idAccessorInterpolated|
//@[40:41) RightSquare |]|
//@[41:42) NewLine |\n|
  case45: foos[i][idAccessorInterpolated]
//@[02:08) Identifier |case45|
//@[08:09) Colon |:|
//@[10:14) Identifier |foos|
//@[14:15) LeftSquare |[|
//@[15:16) Identifier |i|
//@[16:17) RightSquare |]|
//@[17:18) LeftSquare |[|
//@[18:40) Identifier |idAccessorInterpolated|
//@[40:41) RightSquare |]|
//@[41:42) NewLine |\n|
  case46: foos[i + 2][idAccessorInterpolated]
//@[02:08) Identifier |case46|
//@[08:09) Colon |:|
//@[10:14) Identifier |foos|
//@[14:15) LeftSquare |[|
//@[15:16) Identifier |i|
//@[17:18) Plus |+|
//@[19:20) Integer |2|
//@[20:21) RightSquare |]|
//@[21:22) LeftSquare |[|
//@[22:44) Identifier |idAccessorInterpolated|
//@[44:45) RightSquare |]|
//@[45:46) NewLine |\n|
  case47: foos[zeroIndex][idAccessorInterpolated]
//@[02:08) Identifier |case47|
//@[08:09) Colon |:|
//@[10:14) Identifier |foos|
//@[14:15) LeftSquare |[|
//@[15:24) Identifier |zeroIndex|
//@[24:25) RightSquare |]|
//@[25:26) LeftSquare |[|
//@[26:48) Identifier |idAccessorInterpolated|
//@[48:49) RightSquare |]|
//@[49:50) NewLine |\n|
  case48: foos[otherIndex][idAccessorInterpolated]
//@[02:08) Identifier |case48|
//@[08:09) Colon |:|
//@[10:14) Identifier |foos|
//@[14:15) LeftSquare |[|
//@[15:25) Identifier |otherIndex|
//@[25:26) RightSquare |]|
//@[26:27) LeftSquare |[|
//@[27:49) Identifier |idAccessorInterpolated|
//@[49:50) RightSquare |]|
//@[50:51) NewLine |\n|
  case49: foo[idAccessorMixed]
//@[02:08) Identifier |case49|
//@[08:09) Colon |:|
//@[10:13) Identifier |foo|
//@[13:14) LeftSquare |[|
//@[14:29) Identifier |idAccessorMixed|
//@[29:30) RightSquare |]|
//@[30:31) NewLine |\n|
  case50: existingFoo[idAccessorMixed]
//@[02:08) Identifier |case50|
//@[08:09) Colon |:|
//@[10:21) Identifier |existingFoo|
//@[21:22) LeftSquare |[|
//@[22:37) Identifier |idAccessorMixed|
//@[37:38) RightSquare |]|
//@[38:39) NewLine |\n|
  case51: foo::fooChild[idAccessorMixed]
//@[02:08) Identifier |case51|
//@[08:09) Colon |:|
//@[10:13) Identifier |foo|
//@[13:15) DoubleColon |::|
//@[15:23) Identifier |fooChild|
//@[23:24) LeftSquare |[|
//@[24:39) Identifier |idAccessorMixed|
//@[39:40) RightSquare |]|
//@[40:41) NewLine |\n|
  case52: foos[0][idAccessorMixed]
//@[02:08) Identifier |case52|
//@[08:09) Colon |:|
//@[10:14) Identifier |foos|
//@[14:15) LeftSquare |[|
//@[15:16) Integer |0|
//@[16:17) RightSquare |]|
//@[17:18) LeftSquare |[|
//@[18:33) Identifier |idAccessorMixed|
//@[33:34) RightSquare |]|
//@[34:35) NewLine |\n|
  case53: foos[i][idAccessorMixed]
//@[02:08) Identifier |case53|
//@[08:09) Colon |:|
//@[10:14) Identifier |foos|
//@[14:15) LeftSquare |[|
//@[15:16) Identifier |i|
//@[16:17) RightSquare |]|
//@[17:18) LeftSquare |[|
//@[18:33) Identifier |idAccessorMixed|
//@[33:34) RightSquare |]|
//@[34:35) NewLine |\n|
  case54: foos[i + 2][idAccessorMixed]
//@[02:08) Identifier |case54|
//@[08:09) Colon |:|
//@[10:14) Identifier |foos|
//@[14:15) LeftSquare |[|
//@[15:16) Identifier |i|
//@[17:18) Plus |+|
//@[19:20) Integer |2|
//@[20:21) RightSquare |]|
//@[21:22) LeftSquare |[|
//@[22:37) Identifier |idAccessorMixed|
//@[37:38) RightSquare |]|
//@[38:39) NewLine |\n|
  case55: foos[zeroIndex][idAccessorMixed]
//@[02:08) Identifier |case55|
//@[08:09) Colon |:|
//@[10:14) Identifier |foos|
//@[14:15) LeftSquare |[|
//@[15:24) Identifier |zeroIndex|
//@[24:25) RightSquare |]|
//@[25:26) LeftSquare |[|
//@[26:41) Identifier |idAccessorMixed|
//@[41:42) RightSquare |]|
//@[42:43) NewLine |\n|
  case56: foos[otherIndex][idAccessorMixed]
//@[02:08) Identifier |case56|
//@[08:09) Colon |:|
//@[10:14) Identifier |foos|
//@[14:15) LeftSquare |[|
//@[15:25) Identifier |otherIndex|
//@[25:26) RightSquare |]|
//@[26:27) LeftSquare |[|
//@[27:42) Identifier |idAccessorMixed|
//@[42:43) RightSquare |]|
//@[43:44) NewLine |\n|
  case57: foo[strArray[0]]
//@[02:08) Identifier |case57|
//@[08:09) Colon |:|
//@[10:13) Identifier |foo|
//@[13:14) LeftSquare |[|
//@[14:22) Identifier |strArray|
//@[22:23) LeftSquare |[|
//@[23:24) Integer |0|
//@[24:25) RightSquare |]|
//@[25:26) RightSquare |]|
//@[26:27) NewLine |\n|
  case58: existingFoo[strArray[0]]
//@[02:08) Identifier |case58|
//@[08:09) Colon |:|
//@[10:21) Identifier |existingFoo|
//@[21:22) LeftSquare |[|
//@[22:30) Identifier |strArray|
//@[30:31) LeftSquare |[|
//@[31:32) Integer |0|
//@[32:33) RightSquare |]|
//@[33:34) RightSquare |]|
//@[34:35) NewLine |\n|
  case59: foo::fooChild[strArray[0]]
//@[02:08) Identifier |case59|
//@[08:09) Colon |:|
//@[10:13) Identifier |foo|
//@[13:15) DoubleColon |::|
//@[15:23) Identifier |fooChild|
//@[23:24) LeftSquare |[|
//@[24:32) Identifier |strArray|
//@[32:33) LeftSquare |[|
//@[33:34) Integer |0|
//@[34:35) RightSquare |]|
//@[35:36) RightSquare |]|
//@[36:37) NewLine |\n|
  case60: foos[0][strArray[0]]
//@[02:08) Identifier |case60|
//@[08:09) Colon |:|
//@[10:14) Identifier |foos|
//@[14:15) LeftSquare |[|
//@[15:16) Integer |0|
//@[16:17) RightSquare |]|
//@[17:18) LeftSquare |[|
//@[18:26) Identifier |strArray|
//@[26:27) LeftSquare |[|
//@[27:28) Integer |0|
//@[28:29) RightSquare |]|
//@[29:30) RightSquare |]|
//@[30:31) NewLine |\n|
  case61: foos[i][strArray[0]]
//@[02:08) Identifier |case61|
//@[08:09) Colon |:|
//@[10:14) Identifier |foos|
//@[14:15) LeftSquare |[|
//@[15:16) Identifier |i|
//@[16:17) RightSquare |]|
//@[17:18) LeftSquare |[|
//@[18:26) Identifier |strArray|
//@[26:27) LeftSquare |[|
//@[27:28) Integer |0|
//@[28:29) RightSquare |]|
//@[29:30) RightSquare |]|
//@[30:31) NewLine |\n|
  case62: foos[i + 2][strArray[0]]
//@[02:08) Identifier |case62|
//@[08:09) Colon |:|
//@[10:14) Identifier |foos|
//@[14:15) LeftSquare |[|
//@[15:16) Identifier |i|
//@[17:18) Plus |+|
//@[19:20) Integer |2|
//@[20:21) RightSquare |]|
//@[21:22) LeftSquare |[|
//@[22:30) Identifier |strArray|
//@[30:31) LeftSquare |[|
//@[31:32) Integer |0|
//@[32:33) RightSquare |]|
//@[33:34) RightSquare |]|
//@[34:35) NewLine |\n|
  case63: foos[zeroIndex][strArray[0]]
//@[02:08) Identifier |case63|
//@[08:09) Colon |:|
//@[10:14) Identifier |foos|
//@[14:15) LeftSquare |[|
//@[15:24) Identifier |zeroIndex|
//@[24:25) RightSquare |]|
//@[25:26) LeftSquare |[|
//@[26:34) Identifier |strArray|
//@[34:35) LeftSquare |[|
//@[35:36) Integer |0|
//@[36:37) RightSquare |]|
//@[37:38) RightSquare |]|
//@[38:39) NewLine |\n|
  case64: foos[otherIndex][strArray[0]]
//@[02:08) Identifier |case64|
//@[08:09) Colon |:|
//@[10:14) Identifier |foos|
//@[14:15) LeftSquare |[|
//@[15:25) Identifier |otherIndex|
//@[25:26) RightSquare |]|
//@[26:27) LeftSquare |[|
//@[27:35) Identifier |strArray|
//@[35:36) LeftSquare |[|
//@[36:37) Integer |0|
//@[37:38) RightSquare |]|
//@[38:39) RightSquare |]|
//@[39:40) NewLine |\n|
  case65: foo[first(strArray)]
//@[02:08) Identifier |case65|
//@[08:09) Colon |:|
//@[10:13) Identifier |foo|
//@[13:14) LeftSquare |[|
//@[14:19) Identifier |first|
//@[19:20) LeftParen |(|
//@[20:28) Identifier |strArray|
//@[28:29) RightParen |)|
//@[29:30) RightSquare |]|
//@[30:31) NewLine |\n|
  case66: existingFoo[first(strArray)]
//@[02:08) Identifier |case66|
//@[08:09) Colon |:|
//@[10:21) Identifier |existingFoo|
//@[21:22) LeftSquare |[|
//@[22:27) Identifier |first|
//@[27:28) LeftParen |(|
//@[28:36) Identifier |strArray|
//@[36:37) RightParen |)|
//@[37:38) RightSquare |]|
//@[38:39) NewLine |\n|
  case67: foo::fooChild[first(strArray)]
//@[02:08) Identifier |case67|
//@[08:09) Colon |:|
//@[10:13) Identifier |foo|
//@[13:15) DoubleColon |::|
//@[15:23) Identifier |fooChild|
//@[23:24) LeftSquare |[|
//@[24:29) Identifier |first|
//@[29:30) LeftParen |(|
//@[30:38) Identifier |strArray|
//@[38:39) RightParen |)|
//@[39:40) RightSquare |]|
//@[40:41) NewLine |\n|
  case68: foos[0][first(strArray)]
//@[02:08) Identifier |case68|
//@[08:09) Colon |:|
//@[10:14) Identifier |foos|
//@[14:15) LeftSquare |[|
//@[15:16) Integer |0|
//@[16:17) RightSquare |]|
//@[17:18) LeftSquare |[|
//@[18:23) Identifier |first|
//@[23:24) LeftParen |(|
//@[24:32) Identifier |strArray|
//@[32:33) RightParen |)|
//@[33:34) RightSquare |]|
//@[34:35) NewLine |\n|
  case69: foos[i][first(strArray)]
//@[02:08) Identifier |case69|
//@[08:09) Colon |:|
//@[10:14) Identifier |foos|
//@[14:15) LeftSquare |[|
//@[15:16) Identifier |i|
//@[16:17) RightSquare |]|
//@[17:18) LeftSquare |[|
//@[18:23) Identifier |first|
//@[23:24) LeftParen |(|
//@[24:32) Identifier |strArray|
//@[32:33) RightParen |)|
//@[33:34) RightSquare |]|
//@[34:35) NewLine |\n|
  case70: foos[i + 2][first(strArray)]
//@[02:08) Identifier |case70|
//@[08:09) Colon |:|
//@[10:14) Identifier |foos|
//@[14:15) LeftSquare |[|
//@[15:16) Identifier |i|
//@[17:18) Plus |+|
//@[19:20) Integer |2|
//@[20:21) RightSquare |]|
//@[21:22) LeftSquare |[|
//@[22:27) Identifier |first|
//@[27:28) LeftParen |(|
//@[28:36) Identifier |strArray|
//@[36:37) RightParen |)|
//@[37:38) RightSquare |]|
//@[38:39) NewLine |\n|
  case71: foos[zeroIndex][first(strArray)]
//@[02:08) Identifier |case71|
//@[08:09) Colon |:|
//@[10:14) Identifier |foos|
//@[14:15) LeftSquare |[|
//@[15:24) Identifier |zeroIndex|
//@[24:25) RightSquare |]|
//@[25:26) LeftSquare |[|
//@[26:31) Identifier |first|
//@[31:32) LeftParen |(|
//@[32:40) Identifier |strArray|
//@[40:41) RightParen |)|
//@[41:42) RightSquare |]|
//@[42:43) NewLine |\n|
  case72: foos[otherIndex][first(strArray)]
//@[02:08) Identifier |case72|
//@[08:09) Colon |:|
//@[10:14) Identifier |foos|
//@[14:15) LeftSquare |[|
//@[15:25) Identifier |otherIndex|
//@[25:26) RightSquare |]|
//@[26:27) LeftSquare |[|
//@[27:32) Identifier |first|
//@[32:33) LeftParen |(|
//@[33:41) Identifier |strArray|
//@[41:42) RightParen |)|
//@[42:43) RightSquare |]|
//@[43:44) NewLine |\n|
  case73: foo[cond ? 'id' : 'name']
//@[02:08) Identifier |case73|
//@[08:09) Colon |:|
//@[10:13) Identifier |foo|
//@[13:14) LeftSquare |[|
//@[14:18) Identifier |cond|
//@[19:20) Question |?|
//@[21:25) StringComplete |'id'|
//@[26:27) Colon |:|
//@[28:34) StringComplete |'name'|
//@[34:35) RightSquare |]|
//@[35:36) NewLine |\n|
  case74: existingFoo[cond ? 'id' : 'name']
//@[02:08) Identifier |case74|
//@[08:09) Colon |:|
//@[10:21) Identifier |existingFoo|
//@[21:22) LeftSquare |[|
//@[22:26) Identifier |cond|
//@[27:28) Question |?|
//@[29:33) StringComplete |'id'|
//@[34:35) Colon |:|
//@[36:42) StringComplete |'name'|
//@[42:43) RightSquare |]|
//@[43:44) NewLine |\n|
  case75: foo::fooChild[cond ? 'id' : 'name']
//@[02:08) Identifier |case75|
//@[08:09) Colon |:|
//@[10:13) Identifier |foo|
//@[13:15) DoubleColon |::|
//@[15:23) Identifier |fooChild|
//@[23:24) LeftSquare |[|
//@[24:28) Identifier |cond|
//@[29:30) Question |?|
//@[31:35) StringComplete |'id'|
//@[36:37) Colon |:|
//@[38:44) StringComplete |'name'|
//@[44:45) RightSquare |]|
//@[45:46) NewLine |\n|
  case76: foos[0][cond ? 'id' : 'name']
//@[02:08) Identifier |case76|
//@[08:09) Colon |:|
//@[10:14) Identifier |foos|
//@[14:15) LeftSquare |[|
//@[15:16) Integer |0|
//@[16:17) RightSquare |]|
//@[17:18) LeftSquare |[|
//@[18:22) Identifier |cond|
//@[23:24) Question |?|
//@[25:29) StringComplete |'id'|
//@[30:31) Colon |:|
//@[32:38) StringComplete |'name'|
//@[38:39) RightSquare |]|
//@[39:40) NewLine |\n|
  case77: foos[i][cond ? 'id' : 'name']
//@[02:08) Identifier |case77|
//@[08:09) Colon |:|
//@[10:14) Identifier |foos|
//@[14:15) LeftSquare |[|
//@[15:16) Identifier |i|
//@[16:17) RightSquare |]|
//@[17:18) LeftSquare |[|
//@[18:22) Identifier |cond|
//@[23:24) Question |?|
//@[25:29) StringComplete |'id'|
//@[30:31) Colon |:|
//@[32:38) StringComplete |'name'|
//@[38:39) RightSquare |]|
//@[39:40) NewLine |\n|
  case78: foos[i + 2][cond ? 'id' : 'name']
//@[02:08) Identifier |case78|
//@[08:09) Colon |:|
//@[10:14) Identifier |foos|
//@[14:15) LeftSquare |[|
//@[15:16) Identifier |i|
//@[17:18) Plus |+|
//@[19:20) Integer |2|
//@[20:21) RightSquare |]|
//@[21:22) LeftSquare |[|
//@[22:26) Identifier |cond|
//@[27:28) Question |?|
//@[29:33) StringComplete |'id'|
//@[34:35) Colon |:|
//@[36:42) StringComplete |'name'|
//@[42:43) RightSquare |]|
//@[43:44) NewLine |\n|
  case79: foos[zeroIndex][cond ? 'id' : 'name']
//@[02:08) Identifier |case79|
//@[08:09) Colon |:|
//@[10:14) Identifier |foos|
//@[14:15) LeftSquare |[|
//@[15:24) Identifier |zeroIndex|
//@[24:25) RightSquare |]|
//@[25:26) LeftSquare |[|
//@[26:30) Identifier |cond|
//@[31:32) Question |?|
//@[33:37) StringComplete |'id'|
//@[38:39) Colon |:|
//@[40:46) StringComplete |'name'|
//@[46:47) RightSquare |]|
//@[47:48) NewLine |\n|
  case80: foos[otherIndex][cond ? 'id' : 'name']
//@[02:08) Identifier |case80|
//@[08:09) Colon |:|
//@[10:14) Identifier |foos|
//@[14:15) LeftSquare |[|
//@[15:25) Identifier |otherIndex|
//@[25:26) RightSquare |]|
//@[26:27) LeftSquare |[|
//@[27:31) Identifier |cond|
//@[32:33) Question |?|
//@[34:38) StringComplete |'id'|
//@[39:40) Colon |:|
//@[41:47) StringComplete |'name'|
//@[47:48) RightSquare |]|
//@[48:49) NewLine |\n|
  case81: foo[cond ? first(strArray) : strArray[0]]
//@[02:08) Identifier |case81|
//@[08:09) Colon |:|
//@[10:13) Identifier |foo|
//@[13:14) LeftSquare |[|
//@[14:18) Identifier |cond|
//@[19:20) Question |?|
//@[21:26) Identifier |first|
//@[26:27) LeftParen |(|
//@[27:35) Identifier |strArray|
//@[35:36) RightParen |)|
//@[37:38) Colon |:|
//@[39:47) Identifier |strArray|
//@[47:48) LeftSquare |[|
//@[48:49) Integer |0|
//@[49:50) RightSquare |]|
//@[50:51) RightSquare |]|
//@[51:52) NewLine |\n|
  case82: existingFoo[cond ? first(strArray) : strArray[0]]
//@[02:08) Identifier |case82|
//@[08:09) Colon |:|
//@[10:21) Identifier |existingFoo|
//@[21:22) LeftSquare |[|
//@[22:26) Identifier |cond|
//@[27:28) Question |?|
//@[29:34) Identifier |first|
//@[34:35) LeftParen |(|
//@[35:43) Identifier |strArray|
//@[43:44) RightParen |)|
//@[45:46) Colon |:|
//@[47:55) Identifier |strArray|
//@[55:56) LeftSquare |[|
//@[56:57) Integer |0|
//@[57:58) RightSquare |]|
//@[58:59) RightSquare |]|
//@[59:60) NewLine |\n|
  case83: foo::fooChild[cond ? first(strArray) : strArray[0]]
//@[02:08) Identifier |case83|
//@[08:09) Colon |:|
//@[10:13) Identifier |foo|
//@[13:15) DoubleColon |::|
//@[15:23) Identifier |fooChild|
//@[23:24) LeftSquare |[|
//@[24:28) Identifier |cond|
//@[29:30) Question |?|
//@[31:36) Identifier |first|
//@[36:37) LeftParen |(|
//@[37:45) Identifier |strArray|
//@[45:46) RightParen |)|
//@[47:48) Colon |:|
//@[49:57) Identifier |strArray|
//@[57:58) LeftSquare |[|
//@[58:59) Integer |0|
//@[59:60) RightSquare |]|
//@[60:61) RightSquare |]|
//@[61:62) NewLine |\n|
  case84: foos[0][cond ? first(strArray) : strArray[0]]
//@[02:08) Identifier |case84|
//@[08:09) Colon |:|
//@[10:14) Identifier |foos|
//@[14:15) LeftSquare |[|
//@[15:16) Integer |0|
//@[16:17) RightSquare |]|
//@[17:18) LeftSquare |[|
//@[18:22) Identifier |cond|
//@[23:24) Question |?|
//@[25:30) Identifier |first|
//@[30:31) LeftParen |(|
//@[31:39) Identifier |strArray|
//@[39:40) RightParen |)|
//@[41:42) Colon |:|
//@[43:51) Identifier |strArray|
//@[51:52) LeftSquare |[|
//@[52:53) Integer |0|
//@[53:54) RightSquare |]|
//@[54:55) RightSquare |]|
//@[55:56) NewLine |\n|
  case85: foos[i][cond ? first(strArray) : strArray[0]]
//@[02:08) Identifier |case85|
//@[08:09) Colon |:|
//@[10:14) Identifier |foos|
//@[14:15) LeftSquare |[|
//@[15:16) Identifier |i|
//@[16:17) RightSquare |]|
//@[17:18) LeftSquare |[|
//@[18:22) Identifier |cond|
//@[23:24) Question |?|
//@[25:30) Identifier |first|
//@[30:31) LeftParen |(|
//@[31:39) Identifier |strArray|
//@[39:40) RightParen |)|
//@[41:42) Colon |:|
//@[43:51) Identifier |strArray|
//@[51:52) LeftSquare |[|
//@[52:53) Integer |0|
//@[53:54) RightSquare |]|
//@[54:55) RightSquare |]|
//@[55:56) NewLine |\n|
  case86: foos[i + 2][cond ? first(strArray) : strArray[0]]
//@[02:08) Identifier |case86|
//@[08:09) Colon |:|
//@[10:14) Identifier |foos|
//@[14:15) LeftSquare |[|
//@[15:16) Identifier |i|
//@[17:18) Plus |+|
//@[19:20) Integer |2|
//@[20:21) RightSquare |]|
//@[21:22) LeftSquare |[|
//@[22:26) Identifier |cond|
//@[27:28) Question |?|
//@[29:34) Identifier |first|
//@[34:35) LeftParen |(|
//@[35:43) Identifier |strArray|
//@[43:44) RightParen |)|
//@[45:46) Colon |:|
//@[47:55) Identifier |strArray|
//@[55:56) LeftSquare |[|
//@[56:57) Integer |0|
//@[57:58) RightSquare |]|
//@[58:59) RightSquare |]|
//@[59:60) NewLine |\n|
  case87: foos[zeroIndex][cond ? first(strArray) : strArray[0]]
//@[02:08) Identifier |case87|
//@[08:09) Colon |:|
//@[10:14) Identifier |foos|
//@[14:15) LeftSquare |[|
//@[15:24) Identifier |zeroIndex|
//@[24:25) RightSquare |]|
//@[25:26) LeftSquare |[|
//@[26:30) Identifier |cond|
//@[31:32) Question |?|
//@[33:38) Identifier |first|
//@[38:39) LeftParen |(|
//@[39:47) Identifier |strArray|
//@[47:48) RightParen |)|
//@[49:50) Colon |:|
//@[51:59) Identifier |strArray|
//@[59:60) LeftSquare |[|
//@[60:61) Integer |0|
//@[61:62) RightSquare |]|
//@[62:63) RightSquare |]|
//@[63:64) NewLine |\n|
  case88: foos[otherIndex][cond ? first(strArray) : strArray[0]]
//@[02:08) Identifier |case88|
//@[08:09) Colon |:|
//@[10:14) Identifier |foos|
//@[14:15) LeftSquare |[|
//@[15:25) Identifier |otherIndex|
//@[25:26) RightSquare |]|
//@[26:27) LeftSquare |[|
//@[27:31) Identifier |cond|
//@[32:33) Question |?|
//@[34:39) Identifier |first|
//@[39:40) LeftParen |(|
//@[40:48) Identifier |strArray|
//@[48:49) RightParen |)|
//@[50:51) Colon |:|
//@[52:60) Identifier |strArray|
//@[60:61) LeftSquare |[|
//@[61:62) Integer |0|
//@[62:63) RightSquare |]|
//@[63:64) RightSquare |]|
//@[64:65) NewLine |\n|
  case89: foos[cond ? i : zeroIndex]['id']
//@[02:08) Identifier |case89|
//@[08:09) Colon |:|
//@[10:14) Identifier |foos|
//@[14:15) LeftSquare |[|
//@[15:19) Identifier |cond|
//@[20:21) Question |?|
//@[22:23) Identifier |i|
//@[24:25) Colon |:|
//@[26:35) Identifier |zeroIndex|
//@[35:36) RightSquare |]|
//@[36:37) LeftSquare |[|
//@[37:41) StringComplete |'id'|
//@[41:42) RightSquare |]|
//@[42:43) NewLine |\n|
  case90: foos[any(0)].id
//@[02:08) Identifier |case90|
//@[08:09) Colon |:|
//@[10:14) Identifier |foos|
//@[14:15) LeftSquare |[|
//@[15:18) Identifier |any|
//@[18:19) LeftParen |(|
//@[19:20) Integer |0|
//@[20:21) RightParen |)|
//@[21:22) RightSquare |]|
//@[22:23) Dot |.|
//@[23:25) Identifier |id|
//@[25:26) NewLine |\n|
  case91: foos[cond ? any(0) : i - 1]['id']
//@[02:08) Identifier |case91|
//@[08:09) Colon |:|
//@[10:14) Identifier |foos|
//@[14:15) LeftSquare |[|
//@[15:19) Identifier |cond|
//@[20:21) Question |?|
//@[22:25) Identifier |any|
//@[25:26) LeftParen |(|
//@[26:27) Integer |0|
//@[27:28) RightParen |)|
//@[29:30) Colon |:|
//@[31:32) Identifier |i|
//@[33:34) Minus |-|
//@[35:36) Integer |1|
//@[36:37) RightSquare |]|
//@[37:38) LeftSquare |[|
//@[38:42) StringComplete |'id'|
//@[42:43) RightSquare |]|
//@[43:44) NewLine |\n|
}]
//@[00:01) RightBrace |}|
//@[01:02) RightSquare |]|
//@[02:03) NewLine |\n|
var varForBodyOkDeployTimeUsageExpression = [for i in range(0, 2): foo.id]
//@[00:03) Identifier |var|
//@[04:41) Identifier |varForBodyOkDeployTimeUsageExpression|
//@[42:43) Assignment |=|
//@[44:45) LeftSquare |[|
//@[45:48) Identifier |for|
//@[49:50) Identifier |i|
//@[51:53) Identifier |in|
//@[54:59) Identifier |range|
//@[59:60) LeftParen |(|
//@[60:61) Integer |0|
//@[61:62) Comma |,|
//@[63:64) Integer |2|
//@[64:65) RightParen |)|
//@[65:66) Colon |:|
//@[67:70) Identifier |foo|
//@[70:71) Dot |.|
//@[71:73) Identifier |id|
//@[73:74) RightSquare |]|
//@[74:75) NewLine |\n|
var varForBodyOkDeployTimeUsageInterpolatedKey = [for i in range(0, 2): {
//@[00:03) Identifier |var|
//@[04:46) Identifier |varForBodyOkDeployTimeUsageInterpolatedKey|
//@[47:48) Assignment |=|
//@[49:50) LeftSquare |[|
//@[50:53) Identifier |for|
//@[54:55) Identifier |i|
//@[56:58) Identifier |in|
//@[59:64) Identifier |range|
//@[64:65) LeftParen |(|
//@[65:66) Integer |0|
//@[66:67) Comma |,|
//@[68:69) Integer |2|
//@[69:70) RightParen |)|
//@[70:71) Colon |:|
//@[72:73) LeftBrace |{|
//@[73:74) NewLine |\n|
  '${foo.id}': 'id'
//@[02:05) StringLeftPiece |'${|
//@[05:08) Identifier |foo|
//@[08:09) Dot |.|
//@[09:11) Identifier |id|
//@[11:13) StringRightPiece |}'|
//@[13:14) Colon |:|
//@[15:19) StringComplete |'id'|
//@[19:20) NewLine |\n|
}]
//@[00:01) RightBrace |}|
//@[01:02) RightSquare |]|
//@[02:02) EndOfFile ||
