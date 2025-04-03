using none
//@[00:05) Identifier |using|
//@[06:10) Identifier |none|
//@[10:14) NewLine |\r\n\r\n|

var myVar = 1 + 2
//@[00:03) Identifier |var|
//@[04:09) Identifier |myVar|
//@[10:11) Assignment |=|
//@[12:13) Integer |1|
//@[14:15) Plus |+|
//@[16:17) Integer |2|
//@[17:19) NewLine |\r\n|
param p = externalInput('sys.envVar', myVar)
//@[00:05) Identifier |param|
//@[06:07) Identifier |p|
//@[08:09) Assignment |=|
//@[10:23) Identifier |externalInput|
//@[23:24) LeftParen |(|
//@[24:36) StringComplete |'sys.envVar'|
//@[36:37) Comma |,|
//@[38:43) Identifier |myVar|
//@[43:44) RightParen |)|
//@[44:48) NewLine |\r\n\r\n|

var x = 42
//@[00:03) Identifier |var|
//@[04:05) Identifier |x|
//@[06:07) Assignment |=|
//@[08:10) Integer |42|
//@[10:12) NewLine |\r\n|
var myVar2 = 'abcd-${x}'
//@[00:03) Identifier |var|
//@[04:10) Identifier |myVar2|
//@[11:12) Assignment |=|
//@[13:21) StringLeftPiece |'abcd-${|
//@[21:22) Identifier |x|
//@[22:24) StringRightPiece |}'|
//@[24:26) NewLine |\r\n|
param p2 = externalInput('sys.envVar', myVar2)
//@[00:05) Identifier |param|
//@[06:08) Identifier |p2|
//@[09:10) Assignment |=|
//@[11:24) Identifier |externalInput|
//@[24:25) LeftParen |(|
//@[25:37) StringComplete |'sys.envVar'|
//@[37:38) Comma |,|
//@[39:45) Identifier |myVar2|
//@[45:46) RightParen |)|
//@[46:50) NewLine |\r\n\r\n|

var myVar3 = 'test'
//@[00:03) Identifier |var|
//@[04:10) Identifier |myVar3|
//@[11:12) Assignment |=|
//@[13:19) StringComplete |'test'|
//@[19:21) NewLine |\r\n|
param p3 = externalInput(myVar3, myVar3)
//@[00:05) Identifier |param|
//@[06:08) Identifier |p3|
//@[09:10) Assignment |=|
//@[11:24) Identifier |externalInput|
//@[24:25) LeftParen |(|
//@[25:31) Identifier |myVar3|
//@[31:32) Comma |,|
//@[33:39) Identifier |myVar3|
//@[39:40) RightParen |)|
//@[40:44) NewLine |\r\n\r\n|

var myVar4 = {
//@[00:03) Identifier |var|
//@[04:10) Identifier |myVar4|
//@[11:12) Assignment |=|
//@[13:14) LeftBrace |{|
//@[14:16) NewLine |\r\n|
  name: 'test'
//@[02:06) Identifier |name|
//@[06:07) Colon |:|
//@[08:14) StringComplete |'test'|
//@[14:16) NewLine |\r\n|
}
//@[00:01) RightBrace |}|
//@[01:03) NewLine |\r\n|
param p4 = externalInput('sys.cli', myVar4)
//@[00:05) Identifier |param|
//@[06:08) Identifier |p4|
//@[09:10) Assignment |=|
//@[11:24) Identifier |externalInput|
//@[24:25) LeftParen |(|
//@[25:34) StringComplete |'sys.cli'|
//@[34:35) Comma |,|
//@[36:42) Identifier |myVar4|
//@[42:43) RightParen |)|
//@[43:47) NewLine |\r\n\r\n|

var test = 'test'
//@[00:03) Identifier |var|
//@[04:08) Identifier |test|
//@[09:10) Assignment |=|
//@[11:17) StringComplete |'test'|
//@[17:19) NewLine |\r\n|
var myVar5 = {
//@[00:03) Identifier |var|
//@[04:10) Identifier |myVar5|
//@[11:12) Assignment |=|
//@[13:14) LeftBrace |{|
//@[14:16) NewLine |\r\n|
  name: test
//@[02:06) Identifier |name|
//@[06:07) Colon |:|
//@[08:12) Identifier |test|
//@[12:14) NewLine |\r\n|
}
//@[00:01) RightBrace |}|
//@[01:03) NewLine |\r\n|
param p5 = externalInput('sys.cli', {
//@[00:05) Identifier |param|
//@[06:08) Identifier |p5|
//@[09:10) Assignment |=|
//@[11:24) Identifier |externalInput|
//@[24:25) LeftParen |(|
//@[25:34) StringComplete |'sys.cli'|
//@[34:35) Comma |,|
//@[36:37) LeftBrace |{|
//@[37:39) NewLine |\r\n|
  name: myVar5
//@[02:06) Identifier |name|
//@[06:07) Colon |:|
//@[08:14) Identifier |myVar5|
//@[14:16) NewLine |\r\n|
})
//@[00:01) RightBrace |}|
//@[01:02) RightParen |)|
//@[02:06) NewLine |\r\n\r\n|

param p6 = externalInput('custom', 'test')
//@[00:05) Identifier |param|
//@[06:08) Identifier |p6|
//@[09:10) Assignment |=|
//@[11:24) Identifier |externalInput|
//@[24:25) LeftParen |(|
//@[25:33) StringComplete |'custom'|
//@[33:34) Comma |,|
//@[35:41) StringComplete |'test'|
//@[41:42) RightParen |)|
//@[42:44) NewLine |\r\n|
param p7 = externalInput(p6)
//@[00:05) Identifier |param|
//@[06:08) Identifier |p7|
//@[09:10) Assignment |=|
//@[11:24) Identifier |externalInput|
//@[24:25) LeftParen |(|
//@[25:27) Identifier |p6|
//@[27:28) RightParen |)|
//@[28:32) NewLine |\r\n\r\n|

param p8 = externalInput('custom', externalInput('custom', 'foo'))
//@[00:05) Identifier |param|
//@[06:08) Identifier |p8|
//@[09:10) Assignment |=|
//@[11:24) Identifier |externalInput|
//@[24:25) LeftParen |(|
//@[25:33) StringComplete |'custom'|
//@[33:34) Comma |,|
//@[35:48) Identifier |externalInput|
//@[48:49) LeftParen |(|
//@[49:57) StringComplete |'custom'|
//@[57:58) Comma |,|
//@[59:64) StringComplete |'foo'|
//@[64:65) RightParen |)|
//@[65:66) RightParen |)|
//@[66:68) NewLine |\r\n|

//@[00:00) EndOfFile ||
