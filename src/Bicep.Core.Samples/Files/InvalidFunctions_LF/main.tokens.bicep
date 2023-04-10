func useRuntimeFunction = () => reference('foo').bar
//@[00:04) Identifier |func|
//@[05:23) Identifier |useRuntimeFunction|
//@[24:25) Assignment |=|
//@[26:27) LeftParen |(|
//@[27:28) RightParen |)|
//@[29:31) Arrow |=>|
//@[32:41) Identifier |reference|
//@[41:42) LeftParen |(|
//@[42:47) StringComplete |'foo'|
//@[47:48) RightParen |)|
//@[48:49) Dot |.|
//@[49:52) Identifier |bar|
//@[52:54) NewLine |\n\n|

func funcA = () => 'A'
//@[00:04) Identifier |func|
//@[05:10) Identifier |funcA|
//@[11:12) Assignment |=|
//@[13:14) LeftParen |(|
//@[14:15) RightParen |)|
//@[16:18) Arrow |=>|
//@[19:22) StringComplete |'A'|
//@[22:23) NewLine |\n|
func funcB = () => funcA()
//@[00:04) Identifier |func|
//@[05:10) Identifier |funcB|
//@[11:12) Assignment |=|
//@[13:14) LeftParen |(|
//@[14:15) RightParen |)|
//@[16:18) Arrow |=>|
//@[19:24) Identifier |funcA|
//@[24:25) LeftParen |(|
//@[25:26) RightParen |)|
//@[26:28) NewLine |\n\n|

func invalidType = (string input) => input
//@[00:04) Identifier |func|
//@[05:16) Identifier |invalidType|
//@[17:18) Assignment |=|
//@[19:20) LeftParen |(|
//@[20:26) Identifier |string|
//@[27:32) Identifier |input|
//@[32:33) RightParen |)|
//@[34:36) Arrow |=>|
//@[37:42) Identifier |input|
//@[42:44) NewLine |\n\n|

output invalidType string = invalidType(true)
//@[00:06) Identifier |output|
//@[07:18) Identifier |invalidType|
//@[19:25) Identifier |string|
//@[26:27) Assignment |=|
//@[28:39) Identifier |invalidType|
//@[39:40) LeftParen |(|
//@[40:44) TrueKeyword |true|
//@[44:45) RightParen |)|
//@[45:46) NewLine |\n|

//@[00:00) EndOfFile ||
