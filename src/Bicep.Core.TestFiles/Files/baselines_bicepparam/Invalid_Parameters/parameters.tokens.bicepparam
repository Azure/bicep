using './main.bicep'
//@[00:05) Identifier |using|
//@[06:20) StringComplete |'./main.bicep'|
//@[20:24) NewLine |\r\n\r\n|

param para1 = 'value
//@[00:05) Identifier |param|
//@[06:11) Identifier |para1|
//@[12:13) Assignment |=|
//@[14:20) StringComplete |'value|
//@[20:24) NewLine |\r\n\r\n|

para
//@[00:04) Identifier |para|
//@[04:08) NewLine |\r\n\r\n|

para2
//@[00:05) Identifier |para2|
//@[05:09) NewLine |\r\n\r\n|

param expr = 1 + 2
//@[00:05) Identifier |param|
//@[06:10) Identifier |expr|
//@[11:12) Assignment |=|
//@[13:14) Integer |1|
//@[15:16) Plus |+|
//@[17:18) Integer |2|
//@[18:22) NewLine |\r\n\r\n|

param interp = 'abc${123}def'
//@[00:05) Identifier |param|
//@[06:12) Identifier |interp|
//@[13:14) Assignment |=|
//@[15:21) StringLeftPiece |'abc${|
//@[21:24) Integer |123|
//@[24:29) StringRightPiece |}def'|
//@[29:33) NewLine |\r\n\r\n|

param doubleinterp = 'abc${interp + 'blah'}def'
//@[00:05) Identifier |param|
//@[06:18) Identifier |doubleinterp|
//@[19:20) Assignment |=|
//@[21:27) StringLeftPiece |'abc${|
//@[27:33) Identifier |interp|
//@[34:35) Plus |+|
//@[36:42) StringComplete |'blah'|
//@[42:47) StringRightPiece |}def'|
//@[47:51) NewLine |\r\n\r\n|

param objWithExpressions = {
//@[00:05) Identifier |param|
//@[06:24) Identifier |objWithExpressions|
//@[25:26) Assignment |=|
//@[27:28) LeftBrace |{|
//@[28:30) NewLine |\r\n|
  foo: 1 + 2
//@[02:05) Identifier |foo|
//@[05:06) Colon |:|
//@[07:08) Integer |1|
//@[09:10) Plus |+|
//@[11:12) Integer |2|
//@[12:14) NewLine |\r\n|
  bar: {
//@[02:05) Identifier |bar|
//@[05:06) Colon |:|
//@[07:08) LeftBrace |{|
//@[08:10) NewLine |\r\n|
    baz: concat('abc', 'def')
//@[04:07) Identifier |baz|
//@[07:08) Colon |:|
//@[09:15) Identifier |concat|
//@[15:16) LeftParen |(|
//@[16:21) StringComplete |'abc'|
//@[21:22) Comma |,|
//@[23:28) StringComplete |'def'|
//@[28:29) RightParen |)|
//@[29:31) NewLine |\r\n|
  }
//@[02:03) RightBrace |}|
//@[03:05) NewLine |\r\n|
}
//@[00:01) RightBrace |}|
//@[01:05) NewLine |\r\n\r\n|

param arrayWithExpressions = [1 + 1, 'ok']
//@[00:05) Identifier |param|
//@[06:26) Identifier |arrayWithExpressions|
//@[27:28) Assignment |=|
//@[29:30) LeftSquare |[|
//@[30:31) Integer |1|
//@[32:33) Plus |+|
//@[34:35) Integer |1|
//@[35:36) Comma |,|
//@[37:41) StringComplete |'ok'|
//@[41:42) RightSquare |]|
//@[42:42) EndOfFile ||
