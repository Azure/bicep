using none
//@[00:05) Identifier |using|
//@[06:10) Identifier |none|
//@[10:14) NewLine |\r\n\r\n|

// single parameter
//@[19:21) NewLine |\r\n|
param singleParam = externalInput('sys.cli', 'foo')
//@[00:05) Identifier |param|
//@[06:17) Identifier |singleParam|
//@[18:19) Assignment |=|
//@[20:33) Identifier |externalInput|
//@[33:34) LeftParen |(|
//@[34:43) StringComplete |'sys.cli'|
//@[43:44) Comma |,|
//@[45:50) StringComplete |'foo'|
//@[50:51) RightParen |)|
//@[51:55) NewLine |\r\n\r\n|

// single parameter with casting expression
//@[43:45) NewLine |\r\n|
param singleParamCast = bool(externalInput('sys.cli', 'foo'))
//@[00:05) Identifier |param|
//@[06:21) Identifier |singleParamCast|
//@[22:23) Assignment |=|
//@[24:28) Identifier |bool|
//@[28:29) LeftParen |(|
//@[29:42) Identifier |externalInput|
//@[42:43) LeftParen |(|
//@[43:52) StringComplete |'sys.cli'|
//@[52:53) Comma |,|
//@[54:59) StringComplete |'foo'|
//@[59:60) RightParen |)|
//@[60:61) RightParen |)|
//@[61:65) NewLine |\r\n\r\n|

// multiple parameters with different syntax
//@[44:46) NewLine |\r\n|
param foo = externalInput('sys.cli', 'foo')
//@[00:05) Identifier |param|
//@[06:09) Identifier |foo|
//@[10:11) Assignment |=|
//@[12:25) Identifier |externalInput|
//@[25:26) LeftParen |(|
//@[26:35) StringComplete |'sys.cli'|
//@[35:36) Comma |,|
//@[37:42) StringComplete |'foo'|
//@[42:43) RightParen |)|
//@[43:45) NewLine |\r\n|
param bar = externalInput('sys.envVar', 'bar')
//@[00:05) Identifier |param|
//@[06:09) Identifier |bar|
//@[10:11) Assignment |=|
//@[12:25) Identifier |externalInput|
//@[25:26) LeftParen |(|
//@[26:38) StringComplete |'sys.envVar'|
//@[38:39) Comma |,|
//@[40:45) StringComplete |'bar'|
//@[45:46) RightParen |)|
//@[46:48) NewLine |\r\n|
param baz = externalInput('custom.binding', '__BINDING__')
//@[00:05) Identifier |param|
//@[06:09) Identifier |baz|
//@[10:11) Assignment |=|
//@[12:25) Identifier |externalInput|
//@[25:26) LeftParen |(|
//@[26:42) StringComplete |'custom.binding'|
//@[42:43) Comma |,|
//@[44:57) StringComplete |'__BINDING__'|
//@[57:58) RightParen |)|
//@[58:62) NewLine |\r\n\r\n|

// single param with variable reference
//@[39:41) NewLine |\r\n|
var myVar = bool(externalInput('sys.cli', 'myVar'))
//@[00:03) Identifier |var|
//@[04:09) Identifier |myVar|
//@[10:11) Assignment |=|
//@[12:16) Identifier |bool|
//@[16:17) LeftParen |(|
//@[17:30) Identifier |externalInput|
//@[30:31) LeftParen |(|
//@[31:40) StringComplete |'sys.cli'|
//@[40:41) Comma |,|
//@[42:49) StringComplete |'myVar'|
//@[49:50) RightParen |)|
//@[50:51) RightParen |)|
//@[51:53) NewLine |\r\n|
param varRef = myVar
//@[00:05) Identifier |param|
//@[06:12) Identifier |varRef|
//@[13:14) Assignment |=|
//@[15:20) Identifier |myVar|
//@[20:24) NewLine |\r\n\r\n|

// object config
//@[16:18) NewLine |\r\n|
param objectConfig = externalInput('custom.tool', {
//@[00:05) Identifier |param|
//@[06:18) Identifier |objectConfig|
//@[19:20) Assignment |=|
//@[21:34) Identifier |externalInput|
//@[34:35) LeftParen |(|
//@[35:48) StringComplete |'custom.tool'|
//@[48:49) Comma |,|
//@[50:51) LeftBrace |{|
//@[51:53) NewLine |\r\n|
  path: '/path/to/file'
//@[02:06) Identifier |path|
//@[06:07) Colon |:|
//@[08:23) StringComplete |'/path/to/file'|
//@[23:25) NewLine |\r\n|
  isSecure: true
//@[02:10) Identifier |isSecure|
//@[10:11) Colon |:|
//@[12:16) TrueKeyword |true|
//@[16:18) NewLine |\r\n|
})
//@[00:01) RightBrace |}|
//@[01:02) RightParen |)|
//@[02:06) NewLine |\r\n\r\n|

// variable reference chain
//@[27:29) NewLine |\r\n|
var a = externalInput('sys.cli', 'a')
//@[00:03) Identifier |var|
//@[04:05) Identifier |a|
//@[06:07) Assignment |=|
//@[08:21) Identifier |externalInput|
//@[21:22) LeftParen |(|
//@[22:31) StringComplete |'sys.cli'|
//@[31:32) Comma |,|
//@[33:36) StringComplete |'a'|
//@[36:37) RightParen |)|
//@[37:39) NewLine |\r\n|
var b = a
//@[00:03) Identifier |var|
//@[04:05) Identifier |b|
//@[06:07) Assignment |=|
//@[08:09) Identifier |a|
//@[09:11) NewLine |\r\n|
param c = b
//@[00:05) Identifier |param|
//@[06:07) Identifier |c|
//@[08:09) Assignment |=|
//@[10:11) Identifier |b|
//@[11:15) NewLine |\r\n\r\n|

// param reference chain
//@[24:26) NewLine |\r\n|
param a1 = externalInput('sys.cli', 'a')
//@[00:05) Identifier |param|
//@[06:08) Identifier |a1|
//@[09:10) Assignment |=|
//@[11:24) Identifier |externalInput|
//@[24:25) LeftParen |(|
//@[25:34) StringComplete |'sys.cli'|
//@[34:35) Comma |,|
//@[36:39) StringComplete |'a'|
//@[39:40) RightParen |)|
//@[40:42) NewLine |\r\n|
param b1 = a1
//@[00:05) Identifier |param|
//@[06:08) Identifier |b1|
//@[09:10) Assignment |=|
//@[11:13) Identifier |a1|
//@[13:15) NewLine |\r\n|
param c1 = b1
//@[00:05) Identifier |param|
//@[06:08) Identifier |c1|
//@[09:10) Assignment |=|
//@[11:13) Identifier |b1|
//@[13:17) NewLine |\r\n\r\n|

// string interpolation
//@[23:25) NewLine |\r\n|
param first = int(externalInput('custom.binding', '__BINDING__'))
//@[00:05) Identifier |param|
//@[06:11) Identifier |first|
//@[12:13) Assignment |=|
//@[14:17) Identifier |int|
//@[17:18) LeftParen |(|
//@[18:31) Identifier |externalInput|
//@[31:32) LeftParen |(|
//@[32:48) StringComplete |'custom.binding'|
//@[48:49) Comma |,|
//@[50:63) StringComplete |'__BINDING__'|
//@[63:64) RightParen |)|
//@[64:65) RightParen |)|
//@[65:67) NewLine |\r\n|
param second = externalInput('custom.binding', {
//@[00:05) Identifier |param|
//@[06:12) Identifier |second|
//@[13:14) Assignment |=|
//@[15:28) Identifier |externalInput|
//@[28:29) LeftParen |(|
//@[29:45) StringComplete |'custom.binding'|
//@[45:46) Comma |,|
//@[47:48) LeftBrace |{|
//@[48:50) NewLine |\r\n|
    path: '/path/to/file'
//@[04:08) Identifier |path|
//@[08:09) Colon |:|
//@[10:25) StringComplete |'/path/to/file'|
//@[25:27) NewLine |\r\n|
    isSecure: true
//@[04:12) Identifier |isSecure|
//@[12:13) Colon |:|
//@[14:18) TrueKeyword |true|
//@[18:20) NewLine |\r\n|
})
//@[00:01) RightBrace |}|
//@[01:02) RightParen |)|
//@[02:04) NewLine |\r\n|
param result = '${first} combined with ${second}'
//@[00:05) Identifier |param|
//@[06:12) Identifier |result|
//@[13:14) Assignment |=|
//@[15:18) StringLeftPiece |'${|
//@[18:23) Identifier |first|
//@[23:41) StringMiddlePiece |} combined with ${|
//@[41:47) Identifier |second|
//@[47:49) StringRightPiece |}'|
//@[49:53) NewLine |\r\n\r\n|

// instance function call
//@[25:27) NewLine |\r\n|
param myParam = sys.externalInput('sys.cli', 'myParam')
//@[00:05) Identifier |param|
//@[06:13) Identifier |myParam|
//@[14:15) Assignment |=|
//@[16:19) Identifier |sys|
//@[19:20) Dot |.|
//@[20:33) Identifier |externalInput|
//@[33:34) LeftParen |(|
//@[34:43) StringComplete |'sys.cli'|
//@[43:44) Comma |,|
//@[45:54) StringComplete |'myParam'|
//@[54:55) RightParen |)|
//@[55:59) NewLine |\r\n\r\n|

// check sanitized externaInputDefinition
//@[41:43) NewLine |\r\n|
param coolParam = externalInput('sys&sons.cool#param provider')
//@[00:05) Identifier |param|
//@[06:15) Identifier |coolParam|
//@[16:17) Assignment |=|
//@[18:31) Identifier |externalInput|
//@[31:32) LeftParen |(|
//@[32:62) StringComplete |'sys&sons.cool#param provider'|
//@[62:63) RightParen |)|
//@[63:67) NewLine |\r\n\r\n|

param objectBody = {
//@[00:05) Identifier |param|
//@[06:16) Identifier |objectBody|
//@[17:18) Assignment |=|
//@[19:20) LeftBrace |{|
//@[20:22) NewLine |\r\n|
  foo: externalInput('custom.binding', 'foo')
//@[02:05) Identifier |foo|
//@[05:06) Colon |:|
//@[07:20) Identifier |externalInput|
//@[20:21) LeftParen |(|
//@[21:37) StringComplete |'custom.binding'|
//@[37:38) Comma |,|
//@[39:44) StringComplete |'foo'|
//@[44:45) RightParen |)|
//@[45:47) NewLine |\r\n|
  bar: externalInput('custom.binding', 'bar')
//@[02:05) Identifier |bar|
//@[05:06) Colon |:|
//@[07:20) Identifier |externalInput|
//@[20:21) LeftParen |(|
//@[21:37) StringComplete |'custom.binding'|
//@[37:38) Comma |,|
//@[39:44) StringComplete |'bar'|
//@[44:45) RightParen |)|
//@[45:47) NewLine |\r\n|
  baz: 'blah'
//@[02:05) Identifier |baz|
//@[05:06) Colon |:|
//@[07:13) StringComplete |'blah'|
//@[13:15) NewLine |\r\n|
}
//@[00:01) RightBrace |}|
//@[01:03) NewLine |\r\n|

//@[00:00) EndOfFile ||
