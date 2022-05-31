targetScope
//@[00:11) Identifier |targetScope|
//@[11:13) NewLine |\n\n|

// #completionTest(12) -> empty
//@[31:32) NewLine |\n|
targetScope 
//@[00:11) Identifier |targetScope|
//@[12:14) NewLine |\n\n|

// #completionTest(13,14) -> targetScopes
//@[41:42) NewLine |\n|
targetScope = 
//@[00:11) Identifier |targetScope|
//@[12:13) Assignment |=|
//@[14:17) NewLine |\n\n\n|


targetScope = 'asdfds'
//@[00:11) Identifier |targetScope|
//@[12:13) Assignment |=|
//@[14:22) StringComplete |'asdfds'|
//@[22:24) NewLine |\n\n|

targetScope = { }
//@[00:11) Identifier |targetScope|
//@[12:13) Assignment |=|
//@[14:15) LeftBrace |{|
//@[16:17) RightBrace |}|
//@[17:19) NewLine |\n\n|

targetScope = true
//@[00:11) Identifier |targetScope|
//@[12:13) Assignment |=|
//@[14:18) TrueKeyword |true|
//@[18:18) EndOfFile ||
