var subscriptionId = readCliArg('subscription-id')
//@[00:03) Identifier |var|
//@[04:18) Identifier |subscriptionId|
//@[19:20) Assignment |=|
//@[21:31) Identifier |readCliArg|
//@[31:32) LeftParen |(|
//@[32:49) StringComplete |'subscription-id'|
//@[49:50) RightParen |)|
//@[50:52) NewLine |\r\n|
var resourceGroup = readCliArg('resource-group')
//@[00:03) Identifier |var|
//@[04:17) Identifier |resourceGroup|
//@[18:19) Assignment |=|
//@[20:30) Identifier |readCliArg|
//@[30:31) LeftParen |(|
//@[31:47) StringComplete |'resource-group'|
//@[47:48) RightParen |)|
//@[48:52) NewLine |\r\n\r\n|

using 'main.bicep' with {
//@[00:05) Identifier |using|
//@[06:18) StringComplete |'main.bicep'|
//@[19:23) Identifier |with|
//@[24:25) LeftBrace |{|
//@[25:27) NewLine |\r\n|
  scope: '/subscriptions/${subscriptionId}/resourceGroups/${resourceGroup}'
//@[02:07) Identifier |scope|
//@[07:08) Colon |:|
//@[09:27) StringLeftPiece |'/subscriptions/${|
//@[27:41) Identifier |subscriptionId|
//@[41:60) StringMiddlePiece |}/resourceGroups/${|
//@[60:73) Identifier |resourceGroup|
//@[73:75) StringRightPiece |}'|
//@[75:77) NewLine |\r\n|
  actionOnUnmanage: {
//@[02:18) Identifier |actionOnUnmanage|
//@[18:19) Colon |:|
//@[20:21) LeftBrace |{|
//@[21:23) NewLine |\r\n|
    resources: 'delete'
//@[04:13) Identifier |resources|
//@[13:14) Colon |:|
//@[15:23) StringComplete |'delete'|
//@[23:25) NewLine |\r\n|
  }
//@[02:03) RightBrace |}|
//@[03:05) NewLine |\r\n|
  denySettings: {
//@[02:14) Identifier |denySettings|
//@[14:15) Colon |:|
//@[16:17) LeftBrace |{|
//@[17:19) NewLine |\r\n|
    mode: 'denyDelete'
//@[04:08) Identifier |mode|
//@[08:09) Colon |:|
//@[10:22) StringComplete |'denyDelete'|
//@[22:24) NewLine |\r\n|
  }
//@[02:03) RightBrace |}|
//@[03:05) NewLine |\r\n|
  mode: 'stack'
//@[02:06) Identifier |mode|
//@[06:07) Colon |:|
//@[08:15) StringComplete |'stack'|
//@[15:17) NewLine |\r\n|
}
//@[00:01) RightBrace |}|
//@[01:03) NewLine |\r\n|

//@[00:00) EndOfFile ||
