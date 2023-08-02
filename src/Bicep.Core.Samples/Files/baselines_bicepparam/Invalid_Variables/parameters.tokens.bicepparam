using 'main.bicep'
//@[00:05) Identifier |using|
//@[06:18) StringComplete |'main.bicep'|
//@[18:22) NewLine |\r\n\r\n|

var foo
//@[00:03) Identifier |var|
//@[04:07) Identifier |foo|
//@[07:11) NewLine |\r\n\r\n|

var foo2 =
//@[00:03) Identifier |var|
//@[04:08) Identifier |foo2|
//@[09:10) Assignment |=|
//@[10:14) NewLine |\r\n\r\n|

var foo3 = asdf
//@[00:03) Identifier |var|
//@[04:08) Identifier |foo3|
//@[09:10) Assignment |=|
//@[11:15) Identifier |asdf|
//@[15:19) NewLine |\r\n\r\n|

var foo4 = utcNow()
//@[00:03) Identifier |var|
//@[04:08) Identifier |foo4|
//@[09:10) Assignment |=|
//@[11:17) Identifier |utcNow|
//@[17:18) LeftParen |(|
//@[18:19) RightParen |)|
//@[19:21) NewLine |\r\n|

//@[00:00) EndOfFile ||
