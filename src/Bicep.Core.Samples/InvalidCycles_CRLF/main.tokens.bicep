
//@[0:2) NewLine |\r\n|
//self-cycle
//@[12:14) NewLine |\r\n|
var x = x
//@[0:3) Identifier |var|
//@[4:5) Identifier |x|
//@[6:7) Assignment |=|
//@[8:9) Identifier |x|
//@[9:11) NewLine |\r\n|
var q = base64(q, !q)
//@[0:3) Identifier |var|
//@[4:5) Identifier |q|
//@[6:7) Assignment |=|
//@[8:14) Identifier |base64|
//@[14:15) LeftParen |(|
//@[15:16) Identifier |q|
//@[16:17) Comma |,|
//@[18:19) Exclamation |!|
//@[19:20) Identifier |q|
//@[20:21) RightParen |)|
//@[21:25) NewLine |\r\n\r\n|

//2-cycle
//@[9:11) NewLine |\r\n|
var a = b
//@[0:3) Identifier |var|
//@[4:5) Identifier |a|
//@[6:7) Assignment |=|
//@[8:9) Identifier |b|
//@[9:11) NewLine |\r\n|
var b = add(a,1)
//@[0:3) Identifier |var|
//@[4:5) Identifier |b|
//@[6:7) Assignment |=|
//@[8:11) Identifier |add|
//@[11:12) LeftParen |(|
//@[12:13) Identifier |a|
//@[13:14) Comma |,|
//@[14:15) Number |1|
//@[15:16) RightParen |)|
//@[16:20) NewLine |\r\n\r\n|

//3-cycle
//@[9:11) NewLine |\r\n|
var e = f
//@[0:3) Identifier |var|
//@[4:5) Identifier |e|
//@[6:7) Assignment |=|
//@[8:9) Identifier |f|
//@[9:11) NewLine |\r\n|
var f = g && true
//@[0:3) Identifier |var|
//@[4:5) Identifier |f|
//@[6:7) Assignment |=|
//@[8:9) Identifier |g|
//@[10:12) LogicalAnd |&&|
//@[13:17) TrueKeyword |true|
//@[17:19) NewLine |\r\n|
var g = e ? e : e
//@[0:3) Identifier |var|
//@[4:5) Identifier |g|
//@[6:7) Assignment |=|
//@[8:9) Identifier |e|
//@[10:11) Question |?|
//@[12:13) Identifier |e|
//@[14:15) Colon |:|
//@[16:17) Identifier |e|
//@[17:17) EndOfFile ||
