
//@[0:2) NewLine |\r\n|
//self-cycle
//@[12:14) NewLine |\r\n|
var x = x
//@[0:9) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:5)  IdentifierSyntax
//@[4:5)   Identifier |x|
//@[6:7)  Assignment |=|
//@[8:9)  VariableAccessSyntax
//@[8:9)   IdentifierSyntax
//@[8:9)    Identifier |x|
//@[9:11) NewLine |\r\n|
var q = base64(q, !q)
//@[0:21) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:5)  IdentifierSyntax
//@[4:5)   Identifier |q|
//@[6:7)  Assignment |=|
//@[8:21)  FunctionCallSyntax
//@[8:14)   IdentifierSyntax
//@[8:14)    Identifier |base64|
//@[14:15)   LeftParen |(|
//@[15:17)   FunctionArgumentSyntax
//@[15:16)    VariableAccessSyntax
//@[15:16)     IdentifierSyntax
//@[15:16)      Identifier |q|
//@[16:17)    Comma |,|
//@[18:20)   FunctionArgumentSyntax
//@[18:20)    UnaryOperationSyntax
//@[18:19)     Exclamation |!|
//@[19:20)     VariableAccessSyntax
//@[19:20)      IdentifierSyntax
//@[19:20)       Identifier |q|
//@[20:21)   RightParen |)|
//@[21:25) NewLine |\r\n\r\n|

//2-cycle
//@[9:11) NewLine |\r\n|
var a = b
//@[0:9) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:5)  IdentifierSyntax
//@[4:5)   Identifier |a|
//@[6:7)  Assignment |=|
//@[8:9)  VariableAccessSyntax
//@[8:9)   IdentifierSyntax
//@[8:9)    Identifier |b|
//@[9:11) NewLine |\r\n|
var b = add(a,1)
//@[0:16) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:5)  IdentifierSyntax
//@[4:5)   Identifier |b|
//@[6:7)  Assignment |=|
//@[8:16)  FunctionCallSyntax
//@[8:11)   IdentifierSyntax
//@[8:11)    Identifier |add|
//@[11:12)   LeftParen |(|
//@[12:14)   FunctionArgumentSyntax
//@[12:13)    VariableAccessSyntax
//@[12:13)     IdentifierSyntax
//@[12:13)      Identifier |a|
//@[13:14)    Comma |,|
//@[14:15)   FunctionArgumentSyntax
//@[14:15)    NumericLiteralSyntax
//@[14:15)     Number |1|
//@[15:16)   RightParen |)|
//@[16:20) NewLine |\r\n\r\n|

//3-cycle
//@[9:11) NewLine |\r\n|
var e = f
//@[0:9) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:5)  IdentifierSyntax
//@[4:5)   Identifier |e|
//@[6:7)  Assignment |=|
//@[8:9)  VariableAccessSyntax
//@[8:9)   IdentifierSyntax
//@[8:9)    Identifier |f|
//@[9:11) NewLine |\r\n|
var f = g && true
//@[0:17) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:5)  IdentifierSyntax
//@[4:5)   Identifier |f|
//@[6:7)  Assignment |=|
//@[8:17)  BinaryOperationSyntax
//@[8:9)   VariableAccessSyntax
//@[8:9)    IdentifierSyntax
//@[8:9)     Identifier |g|
//@[10:12)   LogicalAnd |&&|
//@[13:17)   BooleanLiteralSyntax
//@[13:17)    TrueKeyword |true|
//@[17:19) NewLine |\r\n|
var g = e ? e : e
//@[0:17) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:5)  IdentifierSyntax
//@[4:5)   Identifier |g|
//@[6:7)  Assignment |=|
//@[8:17)  TernaryOperationSyntax
//@[8:9)   VariableAccessSyntax
//@[8:9)    IdentifierSyntax
//@[8:9)     Identifier |e|
//@[10:11)   Question |?|
//@[12:13)   VariableAccessSyntax
//@[12:13)    IdentifierSyntax
//@[12:13)     Identifier |e|
//@[14:15)   Colon |:|
//@[16:17)   VariableAccessSyntax
//@[16:17)    IdentifierSyntax
//@[16:17)     Identifier |e|
//@[17:21) NewLine |\r\n\r\n|

//4-cycle
//@[9:11) NewLine |\r\n|
var aa = {
//@[0:23) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:6)  IdentifierSyntax
//@[4:6)   Identifier |aa|
//@[7:8)  Assignment |=|
//@[9:23)  ObjectSyntax
//@[9:10)   LeftBrace |{|
//@[10:12)   NewLine |\r\n|
  bb: bb
//@[2:10)   ObjectPropertySyntax
//@[2:4)    IdentifierSyntax
//@[2:4)     Identifier |bb|
//@[4:5)    Colon |:|
//@[6:8)    VariableAccessSyntax
//@[6:8)     IdentifierSyntax
//@[6:8)      Identifier |bb|
//@[8:10)    NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\r\n|
var bb = {
//@[0:23) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:6)  IdentifierSyntax
//@[4:6)   Identifier |bb|
//@[7:8)  Assignment |=|
//@[9:23)  ObjectSyntax
//@[9:10)   LeftBrace |{|
//@[10:12)   NewLine |\r\n|
  cc: cc
//@[2:10)   ObjectPropertySyntax
//@[2:4)    IdentifierSyntax
//@[2:4)     Identifier |cc|
//@[4:5)    Colon |:|
//@[6:8)    VariableAccessSyntax
//@[6:8)     IdentifierSyntax
//@[6:8)      Identifier |cc|
//@[8:10)    NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\r\n|
var cc = {
//@[0:23) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:6)  IdentifierSyntax
//@[4:6)   Identifier |cc|
//@[7:8)  Assignment |=|
//@[9:23)  ObjectSyntax
//@[9:10)   LeftBrace |{|
//@[10:12)   NewLine |\r\n|
  dd: dd
//@[2:10)   ObjectPropertySyntax
//@[2:4)    IdentifierSyntax
//@[2:4)     Identifier |dd|
//@[4:5)    Colon |:|
//@[6:8)    VariableAccessSyntax
//@[6:8)     IdentifierSyntax
//@[6:8)      Identifier |dd|
//@[8:10)    NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\r\n|
var dd = {
//@[0:23) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:6)  IdentifierSyntax
//@[4:6)   Identifier |dd|
//@[7:8)  Assignment |=|
//@[9:23)  ObjectSyntax
//@[9:10)   LeftBrace |{|
//@[10:12)   NewLine |\r\n|
  aa: aa
//@[2:10)   ObjectPropertySyntax
//@[2:4)    IdentifierSyntax
//@[2:4)     Identifier |aa|
//@[4:5)    Colon |:|
//@[6:8)    VariableAccessSyntax
//@[6:8)     IdentifierSyntax
//@[6:8)      Identifier |aa|
//@[8:10)    NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:1) EndOfFile ||
