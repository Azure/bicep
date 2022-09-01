using './main.bicep'
//@[00:05) Identifier |using|
//@[06:20) StringComplete |'./main.bicep'|
//@[20:22) NewLine |\n\n|

param string = 123
//@[00:05) Identifier |param|
//@[06:12) Identifier |string|
//@[13:14) Assignment |=|
//@[15:18) Integer |123|
//@[18:20) NewLine |\n\n|

param bool = 'hello'
//@[00:05) Identifier |param|
//@[06:10) Identifier |bool|
//@[11:12) Assignment |=|
//@[13:20) StringComplete |'hello'|
//@[20:22) NewLine |\n\n|

param int = false
//@[00:05) Identifier |param|
//@[06:09) Identifier |int|
//@[10:11) Assignment |=|
//@[12:17) FalseKeyword |false|
//@[17:19) NewLine |\n\n|

param object = ['abc', 'def']
//@[00:05) Identifier |param|
//@[06:12) Identifier |object|
//@[13:14) Assignment |=|
//@[15:16) LeftSquare |[|
//@[16:21) StringComplete |'abc'|
//@[21:22) Comma |,|
//@[23:28) StringComplete |'def'|
//@[28:29) RightSquare |]|
//@[29:31) NewLine |\n\n|

param array = {
//@[00:05) Identifier |param|
//@[06:11) Identifier |array|
//@[12:13) Assignment |=|
//@[14:15) LeftBrace |{|
//@[15:16) NewLine |\n|
  isThis: 'correct?'
//@[02:08) Identifier |isThis|
//@[08:09) Colon |:|
//@[10:20) StringComplete |'correct?'|
//@[20:21) NewLine |\n|
}
//@[00:01) RightBrace |}|
//@[01:03) NewLine |\n\n|

param stringAllowed = 'notTheAllowedValue'
//@[00:05) Identifier |param|
//@[06:19) Identifier |stringAllowed|
//@[20:21) Assignment |=|
//@[22:42) StringComplete |'notTheAllowedValue'|
//@[42:43) NewLine |\n|

//@[00:00) EndOfFile ||
