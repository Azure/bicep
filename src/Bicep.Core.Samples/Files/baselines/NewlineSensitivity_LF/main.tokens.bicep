@allowed(['abc', 'def', 'ghi'])
//@[00:01) At |@|
//@[01:08) Identifier |allowed|
//@[08:09) LeftParen |(|
//@[09:10) LeftSquare |[|
//@[10:15) StringComplete |'abc'|
//@[15:16) Comma |,|
//@[17:22) StringComplete |'def'|
//@[22:23) Comma |,|
//@[24:29) StringComplete |'ghi'|
//@[29:30) RightSquare |]|
//@[30:31) RightParen |)|
//@[31:32) NewLine |\n|
param foo string
//@[00:05) Identifier |param|
//@[06:09) Identifier |foo|
//@[10:16) Identifier |string|
//@[16:18) NewLine |\n\n|

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

var multiLineFunctionUnusualFormatting = concat(
//@[00:03) Identifier |var|
//@[04:38) Identifier |multiLineFunctionUnusualFormatting|
//@[39:40) Assignment |=|
//@[41:47) Identifier |concat|
//@[47:48) LeftParen |(|
//@[48:49) NewLine |\n|
              'abc',          any(['hello']),
//@[14:19) StringComplete |'abc'|
//@[19:20) Comma |,|
//@[30:33) Identifier |any|
//@[33:34) LeftParen |(|
//@[34:35) LeftSquare |[|
//@[35:42) StringComplete |'hello'|
//@[42:43) RightSquare |]|
//@[43:44) RightParen |)|
//@[44:45) Comma |,|
//@[45:46) NewLine |\n|
'def')
//@[00:05) StringComplete |'def'|
//@[05:06) RightParen |)|
//@[06:08) NewLine |\n\n|

var nestedTest = concat(
//@[00:03) Identifier |var|
//@[04:14) Identifier |nestedTest|
//@[15:16) Assignment |=|
//@[17:23) Identifier |concat|
//@[23:24) LeftParen |(|
//@[24:25) NewLine |\n|
concat(
//@[00:06) Identifier |concat|
//@[06:07) LeftParen |(|
//@[07:08) NewLine |\n|
concat(
//@[00:06) Identifier |concat|
//@[06:07) LeftParen |(|
//@[07:08) NewLine |\n|
concat(
//@[00:06) Identifier |concat|
//@[06:07) LeftParen |(|
//@[07:08) NewLine |\n|
concat(
//@[00:06) Identifier |concat|
//@[06:07) LeftParen |(|
//@[07:08) NewLine |\n|
'level',
//@[00:07) StringComplete |'level'|
//@[07:08) Comma |,|
//@[08:09) NewLine |\n|
'one'),
//@[00:05) StringComplete |'one'|
//@[05:06) RightParen |)|
//@[06:07) Comma |,|
//@[07:08) NewLine |\n|
'two'),
//@[00:05) StringComplete |'two'|
//@[05:06) RightParen |)|
//@[06:07) Comma |,|
//@[07:08) NewLine |\n|
'three'),
//@[00:07) StringComplete |'three'|
//@[07:08) RightParen |)|
//@[08:09) Comma |,|
//@[09:10) NewLine |\n|
'four'),
//@[00:06) StringComplete |'four'|
//@[06:07) RightParen |)|
//@[07:08) Comma |,|
//@[08:09) NewLine |\n|
'five')
//@[00:06) StringComplete |'five'|
//@[06:07) RightParen |)|
//@[07:09) NewLine |\n\n|

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
'ghi', 'jkl'
//@[00:05) StringComplete |'ghi'|
//@[05:06) Comma |,|
//@[07:12) StringComplete |'jkl'|
//@[12:13) NewLine |\n|
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
ghi: 'ghi', jkl: 'jkl'
//@[00:03) Identifier |ghi|
//@[03:04) Colon |:|
//@[05:10) StringComplete |'ghi'|
//@[10:11) Comma |,|
//@[12:15) Identifier |jkl|
//@[15:16) Colon |:|
//@[17:22) StringComplete |'jkl'|
//@[22:23) NewLine |\n|
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
    'bar', 'blah'
//@[04:09) StringComplete |'bar'|
//@[09:10) Comma |,|
//@[11:17) StringComplete |'blah'|
//@[17:18) NewLine |\n|
  ] }
//@[02:03) RightSquare |]|
//@[04:05) RightBrace |}|
//@[05:06) NewLine |\n|
}
//@[00:01) RightBrace |}|
//@[01:03) NewLine |\n\n|

var brokenFormatting = [      /*foo */ 'bar'   /*
//@[00:03) Identifier |var|
//@[04:20) Identifier |brokenFormatting|
//@[21:22) Assignment |=|
//@[23:24) LeftSquare |[|
//@[39:44) StringComplete |'bar'|

hello

*/,        'asdfdsf',             12324,       /*   asdf*/ '',     '''
//@[02:03) Comma |,|
//@[11:20) StringComplete |'asdfdsf'|
//@[20:21) Comma |,|
//@[34:39) Integer |12324|
//@[39:40) Comma |,|
//@[59:61) StringComplete |''|
//@[61:62) Comma |,|
//@[67:76) StringComplete |'''\n\n\n'''|


'''
//@[03:04) NewLine |\n|
123,      233535
//@[00:03) Integer |123|
//@[03:04) Comma |,|
//@[10:16) Integer |233535|
//@[16:17) NewLine |\n|
true
//@[00:04) TrueKeyword |true|
//@[04:05) NewLine |\n|
              ]
//@[14:15) RightSquare |]|
//@[15:16) NewLine |\n|

//@[00:00) EndOfFile ||
