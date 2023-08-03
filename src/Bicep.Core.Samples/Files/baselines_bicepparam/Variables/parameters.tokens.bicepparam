using 'main.bicep'
//@[00:05) Identifier |using|
//@[06:18) StringComplete |'main.bicep'|
//@[18:20) NewLine |\n\n|

@description('blah blah')
//@[00:01) At |@|
//@[01:12) Identifier |description|
//@[12:13) LeftParen |(|
//@[13:24) StringComplete |'blah blah'|
//@[24:25) RightParen |)|
//@[25:26) NewLine |\n|
var blah = 'blah'
//@[00:03) Identifier |var|
//@[04:08) Identifier |blah|
//@[09:10) Assignment |=|
//@[11:17) StringComplete |'blah'|
//@[17:19) NewLine |\n\n|

param foo = blah
//@[00:05) Identifier |param|
//@[06:09) Identifier |foo|
//@[10:11) Assignment |=|
//@[12:16) Identifier |blah|
//@[16:18) NewLine |\n\n|

var abc = 'abc'
//@[00:03) Identifier |var|
//@[04:07) Identifier |abc|
//@[08:09) Assignment |=|
//@[10:15) StringComplete |'abc'|
//@[15:16) NewLine |\n|
var def = {
//@[00:03) Identifier |var|
//@[04:07) Identifier |def|
//@[08:09) Assignment |=|
//@[10:11) LeftBrace |{|
//@[11:12) NewLine |\n|
  abc: toUpper(abc)
//@[02:05) Identifier |abc|
//@[05:06) Colon |:|
//@[07:14) Identifier |toUpper|
//@[14:15) LeftParen |(|
//@[15:18) Identifier |abc|
//@[18:19) RightParen |)|
//@[19:20) NewLine |\n|
}
//@[00:01) RightBrace |}|
//@[01:03) NewLine |\n\n|

param fooObj = {
//@[00:05) Identifier |param|
//@[06:12) Identifier |fooObj|
//@[13:14) Assignment |=|
//@[15:16) LeftBrace |{|
//@[16:17) NewLine |\n|
  def: def
//@[02:05) Identifier |def|
//@[05:06) Colon |:|
//@[07:10) Identifier |def|
//@[10:11) NewLine |\n|
  ghi: 'ghi'
//@[02:05) Identifier |ghi|
//@[05:06) Colon |:|
//@[07:12) StringComplete |'ghi'|
//@[12:13) NewLine |\n|
}
//@[00:01) RightBrace |}|
//@[01:02) NewLine |\n|
var list = 'FOO,BAR,BAZ'
//@[00:03) Identifier |var|
//@[04:08) Identifier |list|
//@[09:10) Assignment |=|
//@[11:24) StringComplete |'FOO,BAR,BAZ'|
//@[24:25) NewLine |\n|
param bar = join(map(range(0, 3), i => split(list, ',')[2 - i]), ',')
//@[00:05) Identifier |param|
//@[06:09) Identifier |bar|
//@[10:11) Assignment |=|
//@[12:16) Identifier |join|
//@[16:17) LeftParen |(|
//@[17:20) Identifier |map|
//@[20:21) LeftParen |(|
//@[21:26) Identifier |range|
//@[26:27) LeftParen |(|
//@[27:28) Integer |0|
//@[28:29) Comma |,|
//@[30:31) Integer |3|
//@[31:32) RightParen |)|
//@[32:33) Comma |,|
//@[34:35) Identifier |i|
//@[36:38) Arrow |=>|
//@[39:44) Identifier |split|
//@[44:45) LeftParen |(|
//@[45:49) Identifier |list|
//@[49:50) Comma |,|
//@[51:54) StringComplete |','|
//@[54:55) RightParen |)|
//@[55:56) LeftSquare |[|
//@[56:57) Integer |2|
//@[58:59) Minus |-|
//@[60:61) Identifier |i|
//@[61:62) RightSquare |]|
//@[62:63) RightParen |)|
//@[63:64) Comma |,|
//@[65:68) StringComplete |','|
//@[68:69) RightParen |)|
//@[69:70) NewLine |\n|

//@[00:00) EndOfFile ||
