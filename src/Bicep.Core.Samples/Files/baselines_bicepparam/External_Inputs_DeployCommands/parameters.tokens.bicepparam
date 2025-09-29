using none
//@[00:05) Identifier |using|
//@[06:10) Identifier |none|
//@[10:14) NewLine |\r\n\r\n|

// single parameter
//@[19:21) NewLine |\r\n|
param singleParam = readCliArg('foo')
//@[00:05) Identifier |param|
//@[06:17) Identifier |singleParam|
//@[18:19) Assignment |=|
//@[20:30) Identifier |readCliArg|
//@[30:31) LeftParen |(|
//@[31:36) StringComplete |'foo'|
//@[36:37) RightParen |)|
//@[37:41) NewLine |\r\n\r\n|

// single parameter with casting expression
//@[43:45) NewLine |\r\n|
param singleParamCast = bool(readCliArg('foo'))
//@[00:05) Identifier |param|
//@[06:21) Identifier |singleParamCast|
//@[22:23) Assignment |=|
//@[24:28) Identifier |bool|
//@[28:29) LeftParen |(|
//@[29:39) Identifier |readCliArg|
//@[39:40) LeftParen |(|
//@[40:45) StringComplete |'foo'|
//@[45:46) RightParen |)|
//@[46:47) RightParen |)|
//@[47:51) NewLine |\r\n\r\n|

// multiple parameters with different syntax
//@[44:46) NewLine |\r\n|
param foo = readCliArg('foo')
//@[00:05) Identifier |param|
//@[06:09) Identifier |foo|
//@[10:11) Assignment |=|
//@[12:22) Identifier |readCliArg|
//@[22:23) LeftParen |(|
//@[23:28) StringComplete |'foo'|
//@[28:29) RightParen |)|
//@[29:31) NewLine |\r\n|
param bar = readEnvVar('bar')
//@[00:05) Identifier |param|
//@[06:09) Identifier |bar|
//@[10:11) Assignment |=|
//@[12:22) Identifier |readEnvVar|
//@[22:23) LeftParen |(|
//@[23:28) StringComplete |'bar'|
//@[28:29) RightParen |)|
//@[29:31) NewLine |\r\n|
param baz = readEnvVar('FIRST_ENV_VAR')
//@[00:05) Identifier |param|
//@[06:09) Identifier |baz|
//@[10:11) Assignment |=|
//@[12:22) Identifier |readEnvVar|
//@[22:23) LeftParen |(|
//@[23:38) StringComplete |'FIRST_ENV_VAR'|
//@[38:39) RightParen |)|
//@[39:43) NewLine |\r\n\r\n|

// single param with variable reference
//@[39:41) NewLine |\r\n|
var myVar = bool(readCliArg('myVar'))
//@[00:03) Identifier |var|
//@[04:09) Identifier |myVar|
//@[10:11) Assignment |=|
//@[12:16) Identifier |bool|
//@[16:17) LeftParen |(|
//@[17:27) Identifier |readCliArg|
//@[27:28) LeftParen |(|
//@[28:35) StringComplete |'myVar'|
//@[35:36) RightParen |)|
//@[36:37) RightParen |)|
//@[37:39) NewLine |\r\n|
param varRef = myVar
//@[00:05) Identifier |param|
//@[06:12) Identifier |varRef|
//@[13:14) Assignment |=|
//@[15:20) Identifier |myVar|
//@[20:24) NewLine |\r\n\r\n|

// variable reference chain
//@[27:29) NewLine |\r\n|
var a = readCliArg('a')
//@[00:03) Identifier |var|
//@[04:05) Identifier |a|
//@[06:07) Assignment |=|
//@[08:18) Identifier |readCliArg|
//@[18:19) LeftParen |(|
//@[19:22) StringComplete |'a'|
//@[22:23) RightParen |)|
//@[23:25) NewLine |\r\n|
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
param a1 = readCliArg('a')
//@[00:05) Identifier |param|
//@[06:08) Identifier |a1|
//@[09:10) Assignment |=|
//@[11:21) Identifier |readCliArg|
//@[21:22) LeftParen |(|
//@[22:25) StringComplete |'a'|
//@[25:26) RightParen |)|
//@[26:28) NewLine |\r\n|
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
param first = int(readEnvVar('FIRST_ENV_VAR'))
//@[00:05) Identifier |param|
//@[06:11) Identifier |first|
//@[12:13) Assignment |=|
//@[14:17) Identifier |int|
//@[17:18) LeftParen |(|
//@[18:28) Identifier |readEnvVar|
//@[28:29) LeftParen |(|
//@[29:44) StringComplete |'FIRST_ENV_VAR'|
//@[44:45) RightParen |)|
//@[45:46) RightParen |)|
//@[46:48) NewLine |\r\n|
param second = readEnvVar('SECOND_ENV_VAR')
//@[00:05) Identifier |param|
//@[06:12) Identifier |second|
//@[13:14) Assignment |=|
//@[15:25) Identifier |readEnvVar|
//@[25:26) LeftParen |(|
//@[26:42) StringComplete |'SECOND_ENV_VAR'|
//@[42:43) RightParen |)|
//@[43:45) NewLine |\r\n|
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
param myParam = sys.readCliArg('myParam')
//@[00:05) Identifier |param|
//@[06:13) Identifier |myParam|
//@[14:15) Assignment |=|
//@[16:19) Identifier |sys|
//@[19:20) Dot |.|
//@[20:30) Identifier |readCliArg|
//@[30:31) LeftParen |(|
//@[31:40) StringComplete |'myParam'|
//@[40:41) RightParen |)|
//@[41:45) NewLine |\r\n\r\n|

// check sanitized externaInputDefinition
//@[41:43) NewLine |\r\n|
param coolParam = readCliArg('sys&sons.cool#param provider')
//@[00:05) Identifier |param|
//@[06:15) Identifier |coolParam|
//@[16:17) Assignment |=|
//@[18:28) Identifier |readCliArg|
//@[28:29) LeftParen |(|
//@[29:59) StringComplete |'sys&sons.cool#param provider'|
//@[59:60) RightParen |)|
//@[60:64) NewLine |\r\n\r\n|

param objectBody = {
//@[00:05) Identifier |param|
//@[06:16) Identifier |objectBody|
//@[17:18) Assignment |=|
//@[19:20) LeftBrace |{|
//@[20:22) NewLine |\r\n|
  foo: readEnvVar('foo')
//@[02:05) Identifier |foo|
//@[05:06) Colon |:|
//@[07:17) Identifier |readEnvVar|
//@[17:18) LeftParen |(|
//@[18:23) StringComplete |'foo'|
//@[23:24) RightParen |)|
//@[24:26) NewLine |\r\n|
  bar: readEnvVar('bar')
//@[02:05) Identifier |bar|
//@[05:06) Colon |:|
//@[07:17) Identifier |readEnvVar|
//@[17:18) LeftParen |(|
//@[18:23) StringComplete |'bar'|
//@[23:24) RightParen |)|
//@[24:26) NewLine |\r\n|
  baz: 'blah'
//@[02:05) Identifier |baz|
//@[05:06) Colon |:|
//@[07:13) StringComplete |'blah'|
//@[13:15) NewLine |\r\n|
}
//@[00:01) RightBrace |}|
//@[01:03) NewLine |\r\n|

//@[00:00) EndOfFile ||
