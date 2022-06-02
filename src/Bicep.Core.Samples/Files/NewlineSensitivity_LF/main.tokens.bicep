var singleLineFunction = concat('abc', 'def')
//@[00:03) Identifier |var|
//@[04:22) Identifier |singleLineFunction|
//@[23:24) Assignment |=|
//@[25:31) Identifier |concat|
//@[31:32) LeftParen |(|
//@[32:37) StringComplete |'abc'|
//@[37:38) Comma |,|
//@[39:44) StringComplete |'def'|
//@[44:45) RightParen |)|
//@[45:47) NewLine |\n\n|

var multiLineFunction = concat(
//@[00:03) Identifier |var|
//@[04:21) Identifier |multiLineFunction|
//@[22:23) Assignment |=|
//@[24:30) Identifier |concat|
//@[30:31) LeftParen |(|
//@[31:32) NewLine |\n|
  'abc',
//@[02:07) StringComplete |'abc'|
//@[07:08) Comma |,|
//@[08:09) NewLine |\n|
  'def'
//@[02:07) StringComplete |'def'|
//@[07:08) NewLine |\n|
)
//@[00:01) RightParen |)|
//@[01:03) NewLine |\n\n|

var singleLineArray = ['abc', 'def']
//@[00:03) Identifier |var|
//@[04:19) Identifier |singleLineArray|
//@[20:21) Assignment |=|
//@[22:23) LeftSquare |[|
//@[23:28) StringComplete |'abc'|
//@[28:29) Comma |,|
//@[30:35) StringComplete |'def'|
//@[35:36) RightSquare |]|
//@[36:37) NewLine |\n|
var singleLineArrayTrailingCommas = ['abc', 'def',]
//@[00:03) Identifier |var|
//@[04:33) Identifier |singleLineArrayTrailingCommas|
//@[34:35) Assignment |=|
//@[36:37) LeftSquare |[|
//@[37:42) StringComplete |'abc'|
//@[42:43) Comma |,|
//@[44:49) StringComplete |'def'|
//@[49:50) Comma |,|
//@[50:51) RightSquare |]|
//@[51:53) NewLine |\n\n|

var multiLineArray = [
//@[00:03) Identifier |var|
//@[04:18) Identifier |multiLineArray|
//@[19:20) Assignment |=|
//@[21:22) LeftSquare |[|
//@[22:23) NewLine |\n|
  'abc'
//@[02:07) StringComplete |'abc'|
//@[07:08) NewLine |\n|
  'def'
//@[02:07) StringComplete |'def'|
//@[07:08) NewLine |\n|
]
//@[00:01) RightSquare |]|
//@[01:02) NewLine |\n|
var multiLineArrayCommas = [
//@[00:03) Identifier |var|
//@[04:24) Identifier |multiLineArrayCommas|
//@[25:26) Assignment |=|
//@[27:28) LeftSquare |[|
//@[28:29) NewLine |\n|
  'abc',
//@[02:07) StringComplete |'abc'|
//@[07:08) Comma |,|
//@[08:09) NewLine |\n|
  'def',
//@[02:07) StringComplete |'def'|
//@[07:08) Comma |,|
//@[08:09) NewLine |\n|
]
//@[00:01) RightSquare |]|
//@[01:03) NewLine |\n\n|

var mixedArray = ['abc', 'def'
//@[00:03) Identifier |var|
//@[04:14) Identifier |mixedArray|
//@[15:16) Assignment |=|
//@[17:18) LeftSquare |[|
//@[18:23) StringComplete |'abc'|
//@[23:24) Comma |,|
//@[25:30) StringComplete |'def'|
//@[30:31) NewLine |\n|
'ghi', 'jkl',
//@[00:05) StringComplete |'ghi'|
//@[05:06) Comma |,|
//@[07:12) StringComplete |'jkl'|
//@[12:13) Comma |,|
//@[13:14) NewLine |\n|
'lmn']
//@[00:05) StringComplete |'lmn'|
//@[05:06) RightSquare |]|
//@[06:08) NewLine |\n\n|

var singleLineObject = { abc: 'def', ghi: 'jkl'}
//@[00:03) Identifier |var|
//@[04:20) Identifier |singleLineObject|
//@[21:22) Assignment |=|
//@[23:24) LeftBrace |{|
//@[25:28) Identifier |abc|
//@[28:29) Colon |:|
//@[30:35) StringComplete |'def'|
//@[35:36) Comma |,|
//@[37:40) Identifier |ghi|
//@[40:41) Colon |:|
//@[42:47) StringComplete |'jkl'|
//@[47:48) RightBrace |}|
//@[48:49) NewLine |\n|
var singleLineObjectTrailingCommas = { abc: 'def', ghi: 'jkl',}
//@[00:03) Identifier |var|
//@[04:34) Identifier |singleLineObjectTrailingCommas|
//@[35:36) Assignment |=|
//@[37:38) LeftBrace |{|
//@[39:42) Identifier |abc|
//@[42:43) Colon |:|
//@[44:49) StringComplete |'def'|
//@[49:50) Comma |,|
//@[51:54) Identifier |ghi|
//@[54:55) Colon |:|
//@[56:61) StringComplete |'jkl'|
//@[61:62) Comma |,|
//@[62:63) RightBrace |}|
//@[63:64) NewLine |\n|
var multiLineObject = {
//@[00:03) Identifier |var|
//@[04:19) Identifier |multiLineObject|
//@[20:21) Assignment |=|
//@[22:23) LeftBrace |{|
//@[23:24) NewLine |\n|
  abc: 'def'
//@[02:05) Identifier |abc|
//@[05:06) Colon |:|
//@[07:12) StringComplete |'def'|
//@[12:13) NewLine |\n|
  ghi: 'jkl'
//@[02:05) Identifier |ghi|
//@[05:06) Colon |:|
//@[07:12) StringComplete |'jkl'|
//@[12:13) NewLine |\n|
}
//@[00:01) RightBrace |}|
//@[01:02) NewLine |\n|
var multiLineObjectCommas = {
//@[00:03) Identifier |var|
//@[04:25) Identifier |multiLineObjectCommas|
//@[26:27) Assignment |=|
//@[28:29) LeftBrace |{|
//@[29:30) NewLine |\n|
  abc: 'def',
//@[02:05) Identifier |abc|
//@[05:06) Colon |:|
//@[07:12) StringComplete |'def'|
//@[12:13) Comma |,|
//@[13:14) NewLine |\n|
  ghi: 'jkl',
//@[02:05) Identifier |ghi|
//@[05:06) Colon |:|
//@[07:12) StringComplete |'jkl'|
//@[12:13) Comma |,|
//@[13:14) NewLine |\n|
}
//@[00:01) RightBrace |}|
//@[01:02) NewLine |\n|
var mixedObject = { abc: 'abc', def: 'def'
//@[00:03) Identifier |var|
//@[04:15) Identifier |mixedObject|
//@[16:17) Assignment |=|
//@[18:19) LeftBrace |{|
//@[20:23) Identifier |abc|
//@[23:24) Colon |:|
//@[25:30) StringComplete |'abc'|
//@[30:31) Comma |,|
//@[32:35) Identifier |def|
//@[35:36) Colon |:|
//@[37:42) StringComplete |'def'|
//@[42:43) NewLine |\n|
ghi: 'ghi', jkl: 'jkl',
//@[00:03) Identifier |ghi|
//@[03:04) Colon |:|
//@[05:10) StringComplete |'ghi'|
//@[10:11) Comma |,|
//@[12:15) Identifier |jkl|
//@[15:16) Colon |:|
//@[17:22) StringComplete |'jkl'|
//@[22:23) Comma |,|
//@[23:24) NewLine |\n|
lmn: 'lmn' }
//@[00:03) Identifier |lmn|
//@[03:04) Colon |:|
//@[05:10) StringComplete |'lmn'|
//@[11:12) RightBrace |}|
//@[12:14) NewLine |\n\n|

var nestedMixed = {
//@[00:03) Identifier |var|
//@[04:15) Identifier |nestedMixed|
//@[16:17) Assignment |=|
//@[18:19) LeftBrace |{|
//@[19:20) NewLine |\n|
  abc: { 'def': 'ghi', abc: 'def', foo: [
//@[02:05) Identifier |abc|
//@[05:06) Colon |:|
//@[07:08) LeftBrace |{|
//@[09:14) StringComplete |'def'|
//@[14:15) Colon |:|
//@[16:21) StringComplete |'ghi'|
//@[21:22) Comma |,|
//@[23:26) Identifier |abc|
//@[26:27) Colon |:|
//@[28:33) StringComplete |'def'|
//@[33:34) Comma |,|
//@[35:38) Identifier |foo|
//@[38:39) Colon |:|
//@[40:41) LeftSquare |[|
//@[41:42) NewLine |\n|
    'bar', 'blah',
//@[04:09) StringComplete |'bar'|
//@[09:10) Comma |,|
//@[11:17) StringComplete |'blah'|
//@[17:18) Comma |,|
//@[18:19) NewLine |\n|
  ] }
//@[02:03) RightSquare |]|
//@[04:05) RightBrace |}|
//@[05:06) NewLine |\n|
}
//@[00:01) RightBrace |}|
//@[01:02) NewLine |\n|

//@[00:00) EndOfFile ||
