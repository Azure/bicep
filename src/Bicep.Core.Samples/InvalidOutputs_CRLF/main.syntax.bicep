
//@[0:2) NewLine |\r\n|
// wrong declaration
//@[20:22) NewLine |\r\n|
bad
//@[0:3) SkippedTriviaSyntax
//@[0:3)  Identifier |bad|
//@[3:7) NewLine |\r\n\r\n|

// incomplete
//@[13:15) NewLine |\r\n|
output 
//@[0:6) SkippedTriviaSyntax
//@[0:6)  Identifier |output|
//@[7:11) NewLine |\r\n\r\n|

// missing type
//@[15:17) NewLine |\r\n|
output foo
//@[0:10) SkippedTriviaSyntax
//@[0:6)  Identifier |output|
//@[7:10)  Identifier |foo|
//@[10:14) NewLine |\r\n\r\n|

// wrong type + missing value
//@[29:31) NewLine |\r\n|
output foo fluffy
//@[0:17) SkippedTriviaSyntax
//@[0:6)  Identifier |output|
//@[7:10)  Identifier |foo|
//@[11:17)  Identifier |fluffy|
//@[17:21) NewLine |\r\n\r\n|

// missing value
//@[16:18) NewLine |\r\n|
output foo string
//@[0:17) SkippedTriviaSyntax
//@[0:6)  Identifier |output|
//@[7:10)  Identifier |foo|
//@[11:17)  Identifier |string|
//@[17:21) NewLine |\r\n\r\n|

// missing value
//@[16:18) NewLine |\r\n|
output foo string =
//@[0:19) SkippedTriviaSyntax
//@[0:6)  Identifier |output|
//@[7:10)  Identifier |foo|
//@[11:17)  Identifier |string|
//@[18:19)  Assignment |=|
//@[19:23) NewLine |\r\n\r\n|

// wrong string output values
//@[29:31) NewLine |\r\n|
output str string = true
//@[0:24) OutputDeclarationSyntax
//@[0:6)  Identifier |output|
//@[7:10)  IdentifierSyntax
//@[7:10)   Identifier |str|
//@[11:17)  TypeSyntax
//@[11:17)   Identifier |string|
//@[18:19)  Assignment |=|
//@[20:24)  BooleanLiteralSyntax
//@[20:24)   TrueKeyword |true|
//@[24:26) NewLine |\r\n|
output str string = false
//@[0:25) OutputDeclarationSyntax
//@[0:6)  Identifier |output|
//@[7:10)  IdentifierSyntax
//@[7:10)   Identifier |str|
//@[11:17)  TypeSyntax
//@[11:17)   Identifier |string|
//@[18:19)  Assignment |=|
//@[20:25)  BooleanLiteralSyntax
//@[20:25)   FalseKeyword |false|
//@[25:27) NewLine |\r\n|
output str string = [
//@[0:24) OutputDeclarationSyntax
//@[0:6)  Identifier |output|
//@[7:10)  IdentifierSyntax
//@[7:10)   Identifier |str|
//@[11:17)  TypeSyntax
//@[11:17)   Identifier |string|
//@[18:19)  Assignment |=|
//@[20:24)  ArraySyntax
//@[20:21)   LeftSquare |[|
//@[21:23)   NewLine |\r\n|
]
//@[0:1)   RightSquare |]|
//@[1:3) NewLine |\r\n|
output str string = {
//@[0:24) OutputDeclarationSyntax
//@[0:6)  Identifier |output|
//@[7:10)  IdentifierSyntax
//@[7:10)   Identifier |str|
//@[11:17)  TypeSyntax
//@[11:17)   Identifier |string|
//@[18:19)  Assignment |=|
//@[20:24)  ObjectSyntax
//@[20:21)   LeftBrace |{|
//@[21:23)   NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\r\n|
output str string = 52
//@[0:22) OutputDeclarationSyntax
//@[0:6)  Identifier |output|
//@[7:10)  IdentifierSyntax
//@[7:10)   Identifier |str|
//@[11:17)  TypeSyntax
//@[11:17)   Identifier |string|
//@[18:19)  Assignment |=|
//@[20:22)  NumericLiteralSyntax
//@[20:22)   Number |52|
//@[22:26) NewLine |\r\n\r\n|

// wrong int output values
//@[26:28) NewLine |\r\n|
output i int = true
//@[0:19) OutputDeclarationSyntax
//@[0:6)  Identifier |output|
//@[7:8)  IdentifierSyntax
//@[7:8)   Identifier |i|
//@[9:12)  TypeSyntax
//@[9:12)   Identifier |int|
//@[13:14)  Assignment |=|
//@[15:19)  BooleanLiteralSyntax
//@[15:19)   TrueKeyword |true|
//@[19:21) NewLine |\r\n|
output i int = false
//@[0:20) OutputDeclarationSyntax
//@[0:6)  Identifier |output|
//@[7:8)  IdentifierSyntax
//@[7:8)   Identifier |i|
//@[9:12)  TypeSyntax
//@[9:12)   Identifier |int|
//@[13:14)  Assignment |=|
//@[15:20)  BooleanLiteralSyntax
//@[15:20)   FalseKeyword |false|
//@[20:22) NewLine |\r\n|
output i int = [
//@[0:19) OutputDeclarationSyntax
//@[0:6)  Identifier |output|
//@[7:8)  IdentifierSyntax
//@[7:8)   Identifier |i|
//@[9:12)  TypeSyntax
//@[9:12)   Identifier |int|
//@[13:14)  Assignment |=|
//@[15:19)  ArraySyntax
//@[15:16)   LeftSquare |[|
//@[16:18)   NewLine |\r\n|
]
//@[0:1)   RightSquare |]|
//@[1:3) NewLine |\r\n|
output i int = }
//@[0:16) SkippedTriviaSyntax
//@[0:6)  Identifier |output|
//@[7:8)  Identifier |i|
//@[9:12)  Identifier |int|
//@[13:14)  Assignment |=|
//@[15:16)  RightBrace |}|
//@[16:18) NewLine |\r\n|
}
//@[0:1) SkippedTriviaSyntax
//@[0:1)  RightBrace |}|
//@[1:3) NewLine |\r\n|
output i int = 'test'
//@[0:21) OutputDeclarationSyntax
//@[0:6)  Identifier |output|
//@[7:8)  IdentifierSyntax
//@[7:8)   Identifier |i|
//@[9:12)  TypeSyntax
//@[9:12)   Identifier |int|
//@[13:14)  Assignment |=|
//@[15:21)  StringSyntax
//@[15:21)   StringComplete |'test'|
//@[21:25) NewLine |\r\n\r\n|

// wrong bool output values
//@[27:29) NewLine |\r\n|
output b bool = [
//@[0:20) OutputDeclarationSyntax
//@[0:6)  Identifier |output|
//@[7:8)  IdentifierSyntax
//@[7:8)   Identifier |b|
//@[9:13)  TypeSyntax
//@[9:13)   Identifier |bool|
//@[14:15)  Assignment |=|
//@[16:20)  ArraySyntax
//@[16:17)   LeftSquare |[|
//@[17:19)   NewLine |\r\n|
]
//@[0:1)   RightSquare |]|
//@[1:3) NewLine |\r\n|
output b bool = {
//@[0:20) OutputDeclarationSyntax
//@[0:6)  Identifier |output|
//@[7:8)  IdentifierSyntax
//@[7:8)   Identifier |b|
//@[9:13)  TypeSyntax
//@[9:13)   Identifier |bool|
//@[14:15)  Assignment |=|
//@[16:20)  ObjectSyntax
//@[16:17)   LeftBrace |{|
//@[17:19)   NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\r\n|
output b bool = 32
//@[0:18) OutputDeclarationSyntax
//@[0:6)  Identifier |output|
//@[7:8)  IdentifierSyntax
//@[7:8)   Identifier |b|
//@[9:13)  TypeSyntax
//@[9:13)   Identifier |bool|
//@[14:15)  Assignment |=|
//@[16:18)  NumericLiteralSyntax
//@[16:18)   Number |32|
//@[18:20) NewLine |\r\n|
output b bool = 'str'
//@[0:21) OutputDeclarationSyntax
//@[0:6)  Identifier |output|
//@[7:8)  IdentifierSyntax
//@[7:8)   Identifier |b|
//@[9:13)  TypeSyntax
//@[9:13)   Identifier |bool|
//@[14:15)  Assignment |=|
//@[16:21)  StringSyntax
//@[16:21)   StringComplete |'str'|
//@[21:25) NewLine |\r\n\r\n|

// wrong array output values
//@[28:30) NewLine |\r\n|
output arr array = 32
//@[0:21) OutputDeclarationSyntax
//@[0:6)  Identifier |output|
//@[7:10)  IdentifierSyntax
//@[7:10)   Identifier |arr|
//@[11:16)  TypeSyntax
//@[11:16)   Identifier |array|
//@[17:18)  Assignment |=|
//@[19:21)  NumericLiteralSyntax
//@[19:21)   Number |32|
//@[21:23) NewLine |\r\n|
output arr array = true
//@[0:23) OutputDeclarationSyntax
//@[0:6)  Identifier |output|
//@[7:10)  IdentifierSyntax
//@[7:10)   Identifier |arr|
//@[11:16)  TypeSyntax
//@[11:16)   Identifier |array|
//@[17:18)  Assignment |=|
//@[19:23)  BooleanLiteralSyntax
//@[19:23)   TrueKeyword |true|
//@[23:25) NewLine |\r\n|
output arr array = false
//@[0:24) OutputDeclarationSyntax
//@[0:6)  Identifier |output|
//@[7:10)  IdentifierSyntax
//@[7:10)   Identifier |arr|
//@[11:16)  TypeSyntax
//@[11:16)   Identifier |array|
//@[17:18)  Assignment |=|
//@[19:24)  BooleanLiteralSyntax
//@[19:24)   FalseKeyword |false|
//@[24:26) NewLine |\r\n|
output arr array = {
//@[0:23) OutputDeclarationSyntax
//@[0:6)  Identifier |output|
//@[7:10)  IdentifierSyntax
//@[7:10)   Identifier |arr|
//@[11:16)  TypeSyntax
//@[11:16)   Identifier |array|
//@[17:18)  Assignment |=|
//@[19:23)  ObjectSyntax
//@[19:20)   LeftBrace |{|
//@[20:22)   NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\r\n|
output arr array = 'str'
//@[0:24) OutputDeclarationSyntax
//@[0:6)  Identifier |output|
//@[7:10)  IdentifierSyntax
//@[7:10)   Identifier |arr|
//@[11:16)  TypeSyntax
//@[11:16)   Identifier |array|
//@[17:18)  Assignment |=|
//@[19:24)  StringSyntax
//@[19:24)   StringComplete |'str'|
//@[24:28) NewLine |\r\n\r\n|

// wrong object output values
//@[29:31) NewLine |\r\n|
output o object = 32
//@[0:20) OutputDeclarationSyntax
//@[0:6)  Identifier |output|
//@[7:8)  IdentifierSyntax
//@[7:8)   Identifier |o|
//@[9:15)  TypeSyntax
//@[9:15)   Identifier |object|
//@[16:17)  Assignment |=|
//@[18:20)  NumericLiteralSyntax
//@[18:20)   Number |32|
//@[20:22) NewLine |\r\n|
output o object = true
//@[0:22) OutputDeclarationSyntax
//@[0:6)  Identifier |output|
//@[7:8)  IdentifierSyntax
//@[7:8)   Identifier |o|
//@[9:15)  TypeSyntax
//@[9:15)   Identifier |object|
//@[16:17)  Assignment |=|
//@[18:22)  BooleanLiteralSyntax
//@[18:22)   TrueKeyword |true|
//@[22:24) NewLine |\r\n|
output o object = false
//@[0:23) OutputDeclarationSyntax
//@[0:6)  Identifier |output|
//@[7:8)  IdentifierSyntax
//@[7:8)   Identifier |o|
//@[9:15)  TypeSyntax
//@[9:15)   Identifier |object|
//@[16:17)  Assignment |=|
//@[18:23)  BooleanLiteralSyntax
//@[18:23)   FalseKeyword |false|
//@[23:25) NewLine |\r\n|
output o object = [
//@[0:22) OutputDeclarationSyntax
//@[0:6)  Identifier |output|
//@[7:8)  IdentifierSyntax
//@[7:8)   Identifier |o|
//@[9:15)  TypeSyntax
//@[9:15)   Identifier |object|
//@[16:17)  Assignment |=|
//@[18:22)  ArraySyntax
//@[18:19)   LeftSquare |[|
//@[19:21)   NewLine |\r\n|
]
//@[0:1)   RightSquare |]|
//@[1:3) NewLine |\r\n|
output o object = 'str'
//@[0:23) OutputDeclarationSyntax
//@[0:6)  Identifier |output|
//@[7:8)  IdentifierSyntax
//@[7:8)   Identifier |o|
//@[9:15)  TypeSyntax
//@[9:15)   Identifier |object|
//@[16:17)  Assignment |=|
//@[18:23)  StringSyntax
//@[18:23)   StringComplete |'str'|
//@[23:27) NewLine |\r\n\r\n|

// a few expression cases
//@[25:27) NewLine |\r\n|
output exp string = 2 + 3
//@[0:25) OutputDeclarationSyntax
//@[0:6)  Identifier |output|
//@[7:10)  IdentifierSyntax
//@[7:10)   Identifier |exp|
//@[11:17)  TypeSyntax
//@[11:17)   Identifier |string|
//@[18:19)  Assignment |=|
//@[20:25)  BinaryOperationSyntax
//@[20:21)   NumericLiteralSyntax
//@[20:21)    Number |2|
//@[22:23)   Plus |+|
//@[24:25)   NumericLiteralSyntax
//@[24:25)    Number |3|
//@[25:27) NewLine |\r\n|
output union string = true ? 's' : 1
//@[0:36) OutputDeclarationSyntax
//@[0:6)  Identifier |output|
//@[7:12)  IdentifierSyntax
//@[7:12)   Identifier |union|
//@[13:19)  TypeSyntax
//@[13:19)   Identifier |string|
//@[20:21)  Assignment |=|
//@[22:36)  TernaryOperationSyntax
//@[22:26)   BooleanLiteralSyntax
//@[22:26)    TrueKeyword |true|
//@[27:28)   Question |?|
//@[29:32)   StringSyntax
//@[29:32)    StringComplete |'s'|
//@[33:34)   Colon |:|
//@[35:36)   NumericLiteralSyntax
//@[35:36)    Number |1|
//@[36:38) NewLine |\r\n|
output bad int = true && !4
//@[0:27) OutputDeclarationSyntax
//@[0:6)  Identifier |output|
//@[7:10)  IdentifierSyntax
//@[7:10)   Identifier |bad|
//@[11:14)  TypeSyntax
//@[11:14)   Identifier |int|
//@[15:16)  Assignment |=|
//@[17:27)  BinaryOperationSyntax
//@[17:21)   BooleanLiteralSyntax
//@[17:21)    TrueKeyword |true|
//@[22:24)   LogicalAnd |&&|
//@[25:27)   UnaryOperationSyntax
//@[25:26)    Exclamation |!|
//@[26:27)    NumericLiteralSyntax
//@[26:27)     Number |4|
//@[27:29) NewLine |\r\n|
output deeper bool = true ? -true : (14 && 's') + 10
//@[0:52) OutputDeclarationSyntax
//@[0:6)  Identifier |output|
//@[7:13)  IdentifierSyntax
//@[7:13)   Identifier |deeper|
//@[14:18)  TypeSyntax
//@[14:18)   Identifier |bool|
//@[19:20)  Assignment |=|
//@[21:52)  TernaryOperationSyntax
//@[21:25)   BooleanLiteralSyntax
//@[21:25)    TrueKeyword |true|
//@[26:27)   Question |?|
//@[28:33)   UnaryOperationSyntax
//@[28:29)    Minus |-|
//@[29:33)    BooleanLiteralSyntax
//@[29:33)     TrueKeyword |true|
//@[34:35)   Colon |:|
//@[36:52)   BinaryOperationSyntax
//@[36:47)    ParenthesizedExpressionSyntax
//@[36:37)     LeftParen |(|
//@[37:46)     BinaryOperationSyntax
//@[37:39)      NumericLiteralSyntax
//@[37:39)       Number |14|
//@[40:42)      LogicalAnd |&&|
//@[43:46)      StringSyntax
//@[43:46)       StringComplete |'s'|
//@[46:47)     RightParen |)|
//@[48:49)    Plus |+|
//@[50:52)    NumericLiteralSyntax
//@[50:52)     Number |10|
//@[52:54) NewLine |\r\n|

//@[0:0) EndOfFile ||
