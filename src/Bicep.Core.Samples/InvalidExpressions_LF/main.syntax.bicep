/*
  This tests the various cases of invalid expressions.
*/
//@[2:4) NewLine |\n\n|

// bad expressions
//@[18:19) NewLine |\n|
var bad = a+
//@[0:12) SkippedTriviaSyntax
//@[0:3)  Identifier |var|
//@[4:7)  Identifier |bad|
//@[8:9)  Assignment |=|
//@[10:11)  Identifier |a|
//@[11:12)  Plus |+|
//@[12:13) NewLine |\n|
var bad = *
//@[0:11) SkippedTriviaSyntax
//@[0:3)  Identifier |var|
//@[4:7)  Identifier |bad|
//@[8:9)  Assignment |=|
//@[10:11)  Asterisk |*|
//@[11:12) NewLine |\n|
var bad = /
//@[0:11) SkippedTriviaSyntax
//@[0:3)  Identifier |var|
//@[4:7)  Identifier |bad|
//@[8:9)  Assignment |=|
//@[10:11)  Slash |/|
//@[11:12) NewLine |\n|
var bad = %
//@[0:11) SkippedTriviaSyntax
//@[0:3)  Identifier |var|
//@[4:7)  Identifier |bad|
//@[8:9)  Assignment |=|
//@[10:11)  Modulo |%|
//@[11:12) NewLine |\n|
var bad = 33-
//@[0:13) SkippedTriviaSyntax
//@[0:3)  Identifier |var|
//@[4:7)  Identifier |bad|
//@[8:9)  Assignment |=|
//@[10:12)  Number |33|
//@[12:13)  Minus |-|
//@[13:14) NewLine |\n|
var bad = --33
//@[0:14) SkippedTriviaSyntax
//@[0:3)  Identifier |var|
//@[4:7)  Identifier |bad|
//@[8:9)  Assignment |=|
//@[10:11)  Minus |-|
//@[11:12)  Minus |-|
//@[12:14)  Number |33|
//@[14:15) NewLine |\n|
var bad = 3 * 4 /
//@[0:17) SkippedTriviaSyntax
//@[0:3)  Identifier |var|
//@[4:7)  Identifier |bad|
//@[8:9)  Assignment |=|
//@[10:11)  Number |3|
//@[12:13)  Asterisk |*|
//@[14:15)  Number |4|
//@[16:17)  Slash |/|
//@[17:18) NewLine |\n|
var bad = 222222222222222222222222222222222222222222 * 4
//@[0:56) SkippedTriviaSyntax
//@[0:3)  Identifier |var|
//@[4:7)  Identifier |bad|
//@[8:9)  Assignment |=|
//@[10:52)  Number |222222222222222222222222222222222222222222|
//@[53:54)  Asterisk |*|
//@[55:56)  Number |4|
//@[56:57) NewLine |\n|
var bad = (null) ?
//@[0:18) SkippedTriviaSyntax
//@[0:3)  Identifier |var|
//@[4:7)  Identifier |bad|
//@[8:9)  Assignment |=|
//@[10:11)  LeftParen |(|
//@[11:15)  NullKeyword |null|
//@[15:16)  RightParen |)|
//@[17:18)  Question |?|
//@[18:19) NewLine |\n|
var bad = (null) ? :
//@[0:20) SkippedTriviaSyntax
//@[0:3)  Identifier |var|
//@[4:7)  Identifier |bad|
//@[8:9)  Assignment |=|
//@[10:11)  LeftParen |(|
//@[11:15)  NullKeyword |null|
//@[15:16)  RightParen |)|
//@[17:18)  Question |?|
//@[19:20)  Colon |:|
//@[20:21) NewLine |\n|
var bad = (null) ? !
//@[0:20) SkippedTriviaSyntax
//@[0:3)  Identifier |var|
//@[4:7)  Identifier |bad|
//@[8:9)  Assignment |=|
//@[10:11)  LeftParen |(|
//@[11:15)  NullKeyword |null|
//@[15:16)  RightParen |)|
//@[17:18)  Question |?|
//@[19:20)  Exclamation |!|
//@[20:21) NewLine |\n|
var bad = (null)!
//@[0:16) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:7)  IdentifierSyntax
//@[4:7)   Identifier |bad|
//@[8:9)  Assignment |=|
//@[10:16)  ParenthesizedExpressionSyntax
//@[10:11)   LeftParen |(|
//@[11:15)   NullLiteralSyntax
//@[11:15)    NullKeyword |null|
//@[15:16)   RightParen |)|
//@[16:18) SkippedTriviaSyntax
//@[16:17)  Exclamation |!|
//@[17:18)  NewLine |\n|
var bad = (null)[0]
//@[0:19) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:7)  IdentifierSyntax
//@[4:7)   Identifier |bad|
//@[8:9)  Assignment |=|
//@[10:19)  ArrayAccessSyntax
//@[10:16)   ParenthesizedExpressionSyntax
//@[10:11)    LeftParen |(|
//@[11:15)    NullLiteralSyntax
//@[11:15)     NullKeyword |null|
//@[15:16)    RightParen |)|
//@[16:17)   LeftSquare |[|
//@[17:18)   NumericLiteralSyntax
//@[17:18)    Number |0|
//@[18:19)   RightSquare |]|
//@[19:20) NewLine |\n|
var bad = ()
//@[0:12) SkippedTriviaSyntax
//@[0:3)  Identifier |var|
//@[4:7)  Identifier |bad|
//@[8:9)  Assignment |=|
//@[10:11)  LeftParen |(|
//@[11:12)  RightParen |)|
//@[12:13) NewLine |\n|
var bad = 
//@[0:9) SkippedTriviaSyntax
//@[0:3)  Identifier |var|
//@[4:7)  Identifier |bad|
//@[8:9)  Assignment |=|
//@[10:12) NewLine |\n\n|

// variables not supported
//@[26:27) NewLine |\n|
var x = a + 2
//@[0:13) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:5)  IdentifierSyntax
//@[4:5)   Identifier |x|
//@[6:7)  Assignment |=|
//@[8:13)  BinaryOperationSyntax
//@[8:9)   VariableAccessSyntax
//@[8:9)    IdentifierSyntax
//@[8:9)     Identifier |a|
//@[10:11)   Plus |+|
//@[12:13)   NumericLiteralSyntax
//@[12:13)    Number |2|
//@[13:15) NewLine |\n\n|

// unary NOT
//@[12:13) NewLine |\n|
var not = !null
//@[0:15) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:7)  IdentifierSyntax
//@[4:7)   Identifier |not|
//@[8:9)  Assignment |=|
//@[10:15)  UnaryOperationSyntax
//@[10:11)   Exclamation |!|
//@[11:15)   NullLiteralSyntax
//@[11:15)    NullKeyword |null|
//@[15:16) NewLine |\n|
var not = !4
//@[0:12) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:7)  IdentifierSyntax
//@[4:7)   Identifier |not|
//@[8:9)  Assignment |=|
//@[10:12)  UnaryOperationSyntax
//@[10:11)   Exclamation |!|
//@[11:12)   NumericLiteralSyntax
//@[11:12)    Number |4|
//@[12:13) NewLine |\n|
var not = !'s'
//@[0:14) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:7)  IdentifierSyntax
//@[4:7)   Identifier |not|
//@[8:9)  Assignment |=|
//@[10:14)  UnaryOperationSyntax
//@[10:11)   Exclamation |!|
//@[11:14)   StringSyntax
//@[11:14)    StringComplete |'s'|
//@[14:15) NewLine |\n|
var not = ![
//@[0:14) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:7)  IdentifierSyntax
//@[4:7)   Identifier |not|
//@[8:9)  Assignment |=|
//@[10:14)  UnaryOperationSyntax
//@[10:11)   Exclamation |!|
//@[11:14)   ArraySyntax
//@[11:12)    LeftSquare |[|
//@[12:13)    NewLine |\n|
]
//@[0:1)    RightSquare |]|
//@[1:2) NewLine |\n|
var not = !{
//@[0:14) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:7)  IdentifierSyntax
//@[4:7)   Identifier |not|
//@[8:9)  Assignment |=|
//@[10:14)  UnaryOperationSyntax
//@[10:11)   Exclamation |!|
//@[11:14)   ObjectSyntax
//@[11:12)    LeftBrace |{|
//@[12:13)    NewLine |\n|
}
//@[0:1)    RightBrace |}|
//@[1:3) NewLine |\n\n|

// unary not chaining will be added in the future
//@[49:50) NewLine |\n|
var not = !!!!!!!true
//@[0:21) SkippedTriviaSyntax
//@[0:3)  Identifier |var|
//@[4:7)  Identifier |not|
//@[8:9)  Assignment |=|
//@[10:11)  Exclamation |!|
//@[11:12)  Exclamation |!|
//@[12:13)  Exclamation |!|
//@[13:14)  Exclamation |!|
//@[14:15)  Exclamation |!|
//@[15:16)  Exclamation |!|
//@[16:17)  Exclamation |!|
//@[17:21)  TrueKeyword |true|
//@[21:23) NewLine |\n\n|

// unary minus chaining will not be supported (to reserve -- in case we need it)
//@[80:81) NewLine |\n|
var minus = ------12
//@[0:20) SkippedTriviaSyntax
//@[0:3)  Identifier |var|
//@[4:9)  Identifier |minus|
//@[10:11)  Assignment |=|
//@[12:13)  Minus |-|
//@[13:14)  Minus |-|
//@[14:15)  Minus |-|
//@[15:16)  Minus |-|
//@[16:17)  Minus |-|
//@[17:18)  Minus |-|
//@[18:20)  Number |12|
//@[20:22) NewLine |\n\n|

// unary minus
//@[14:15) NewLine |\n|
var minus = -true
//@[0:17) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:9)  IdentifierSyntax
//@[4:9)   Identifier |minus|
//@[10:11)  Assignment |=|
//@[12:17)  UnaryOperationSyntax
//@[12:13)   Minus |-|
//@[13:17)   BooleanLiteralSyntax
//@[13:17)    TrueKeyword |true|
//@[17:18) NewLine |\n|
var minus = -null
//@[0:17) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:9)  IdentifierSyntax
//@[4:9)   Identifier |minus|
//@[10:11)  Assignment |=|
//@[12:17)  UnaryOperationSyntax
//@[12:13)   Minus |-|
//@[13:17)   NullLiteralSyntax
//@[13:17)    NullKeyword |null|
//@[17:18) NewLine |\n|
var minus = -'s'
//@[0:16) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:9)  IdentifierSyntax
//@[4:9)   Identifier |minus|
//@[10:11)  Assignment |=|
//@[12:16)  UnaryOperationSyntax
//@[12:13)   Minus |-|
//@[13:16)   StringSyntax
//@[13:16)    StringComplete |'s'|
//@[16:17) NewLine |\n|
var minus = -[
//@[0:16) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:9)  IdentifierSyntax
//@[4:9)   Identifier |minus|
//@[10:11)  Assignment |=|
//@[12:16)  UnaryOperationSyntax
//@[12:13)   Minus |-|
//@[13:16)   ArraySyntax
//@[13:14)    LeftSquare |[|
//@[14:15)    NewLine |\n|
]
//@[0:1)    RightSquare |]|
//@[1:2) NewLine |\n|
var minus = -{
//@[0:16) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:9)  IdentifierSyntax
//@[4:9)   Identifier |minus|
//@[10:11)  Assignment |=|
//@[12:16)  UnaryOperationSyntax
//@[12:13)   Minus |-|
//@[13:16)   ObjectSyntax
//@[13:14)    LeftBrace |{|
//@[14:15)    NewLine |\n|
}
//@[0:1)    RightBrace |}|
//@[1:3) NewLine |\n\n|

// multiplicative
//@[17:18) NewLine |\n|
var mod = 's' % true
//@[0:20) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:7)  IdentifierSyntax
//@[4:7)   Identifier |mod|
//@[8:9)  Assignment |=|
//@[10:20)  BinaryOperationSyntax
//@[10:13)   StringSyntax
//@[10:13)    StringComplete |'s'|
//@[14:15)   Modulo |%|
//@[16:20)   BooleanLiteralSyntax
//@[16:20)    TrueKeyword |true|
//@[20:21) NewLine |\n|
var mul = true * null
//@[0:21) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:7)  IdentifierSyntax
//@[4:7)   Identifier |mul|
//@[8:9)  Assignment |=|
//@[10:21)  BinaryOperationSyntax
//@[10:14)   BooleanLiteralSyntax
//@[10:14)    TrueKeyword |true|
//@[15:16)   Asterisk |*|
//@[17:21)   NullLiteralSyntax
//@[17:21)    NullKeyword |null|
//@[21:22) NewLine |\n|
var div = {
//@[0:19) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:7)  IdentifierSyntax
//@[4:7)   Identifier |div|
//@[8:9)  Assignment |=|
//@[10:19)  BinaryOperationSyntax
//@[10:13)   ObjectSyntax
//@[10:11)    LeftBrace |{|
//@[11:12)    NewLine |\n|
} / [
//@[0:1)    RightBrace |}|
//@[2:3)   Slash |/|
//@[4:7)   ArraySyntax
//@[4:5)    LeftSquare |[|
//@[5:6)    NewLine |\n|
]
//@[0:1)    RightSquare |]|
//@[1:3) NewLine |\n\n|

// additive
//@[11:12) NewLine |\n|
var add = null + 's'
//@[0:20) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:7)  IdentifierSyntax
//@[4:7)   Identifier |add|
//@[8:9)  Assignment |=|
//@[10:20)  BinaryOperationSyntax
//@[10:14)   NullLiteralSyntax
//@[10:14)    NullKeyword |null|
//@[15:16)   Plus |+|
//@[17:20)   StringSyntax
//@[17:20)    StringComplete |'s'|
//@[20:21) NewLine |\n|
var sub = true - false
//@[0:22) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:7)  IdentifierSyntax
//@[4:7)   Identifier |sub|
//@[8:9)  Assignment |=|
//@[10:22)  BinaryOperationSyntax
//@[10:14)   BooleanLiteralSyntax
//@[10:14)    TrueKeyword |true|
//@[15:16)   Minus |-|
//@[17:22)   BooleanLiteralSyntax
//@[17:22)    FalseKeyword |false|
//@[22:24) NewLine |\n\n|

// equality (== and != can't have a type error because they work on "any" type)
//@[79:80) NewLine |\n|
var eq = true =~ null
//@[0:21) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:6)  IdentifierSyntax
//@[4:6)   Identifier |eq|
//@[7:8)  Assignment |=|
//@[9:21)  BinaryOperationSyntax
//@[9:13)   BooleanLiteralSyntax
//@[9:13)    TrueKeyword |true|
//@[14:16)   EqualsInsensitive |=~|
//@[17:21)   NullLiteralSyntax
//@[17:21)    NullKeyword |null|
//@[21:22) NewLine |\n|
var ne = 15 !~ [
//@[0:18) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:6)  IdentifierSyntax
//@[4:6)   Identifier |ne|
//@[7:8)  Assignment |=|
//@[9:18)  BinaryOperationSyntax
//@[9:11)   NumericLiteralSyntax
//@[9:11)    Number |15|
//@[12:14)   NotEqualsInsensitive |!~|
//@[15:18)   ArraySyntax
//@[15:16)    LeftSquare |[|
//@[16:17)    NewLine |\n|
]
//@[0:1)    RightSquare |]|
//@[1:3) NewLine |\n\n|

// relational
//@[13:14) NewLine |\n|
var lt = 4 < 's'
//@[0:16) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:6)  IdentifierSyntax
//@[4:6)   Identifier |lt|
//@[7:8)  Assignment |=|
//@[9:16)  BinaryOperationSyntax
//@[9:10)   NumericLiteralSyntax
//@[9:10)    Number |4|
//@[11:12)   LessThan |<|
//@[13:16)   StringSyntax
//@[13:16)    StringComplete |'s'|
//@[16:17) NewLine |\n|
var lteq = null <= 10
//@[0:21) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:8)  IdentifierSyntax
//@[4:8)   Identifier |lteq|
//@[9:10)  Assignment |=|
//@[11:21)  BinaryOperationSyntax
//@[11:15)   NullLiteralSyntax
//@[11:15)    NullKeyword |null|
//@[16:18)   LessThanOrEqual |<=|
//@[19:21)   NumericLiteralSyntax
//@[19:21)    Number |10|
//@[21:22) NewLine |\n|
var gt = false>[
//@[0:18) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:6)  IdentifierSyntax
//@[4:6)   Identifier |gt|
//@[7:8)  Assignment |=|
//@[9:18)  BinaryOperationSyntax
//@[9:14)   BooleanLiteralSyntax
//@[9:14)    FalseKeyword |false|
//@[14:15)   GreaterThan |>|
//@[15:18)   ArraySyntax
//@[15:16)    LeftSquare |[|
//@[16:17)    NewLine |\n|
]
//@[0:1)    RightSquare |]|
//@[1:2) NewLine |\n|
var gteq = {
//@[0:23) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:8)  IdentifierSyntax
//@[4:8)   Identifier |gteq|
//@[9:10)  Assignment |=|
//@[11:23)  BinaryOperationSyntax
//@[11:14)   ObjectSyntax
//@[11:12)    LeftBrace |{|
//@[12:13)    NewLine |\n|
} >= false
//@[0:1)    RightBrace |}|
//@[2:4)   GreaterThanOrEqual |>=|
//@[5:10)   BooleanLiteralSyntax
//@[5:10)    FalseKeyword |false|
//@[10:12) NewLine |\n\n|

// logical
//@[10:11) NewLine |\n|
var and = null && 'a'
//@[0:21) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:7)  IdentifierSyntax
//@[4:7)   Identifier |and|
//@[8:9)  Assignment |=|
//@[10:21)  BinaryOperationSyntax
//@[10:14)   NullLiteralSyntax
//@[10:14)    NullKeyword |null|
//@[15:17)   LogicalAnd |&&|
//@[18:21)   StringSyntax
//@[18:21)    StringComplete |'a'|
//@[21:22) NewLine |\n|
var or = 10 || 4
//@[0:16) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:6)  IdentifierSyntax
//@[4:6)   Identifier |or|
//@[7:8)  Assignment |=|
//@[9:16)  BinaryOperationSyntax
//@[9:11)   NumericLiteralSyntax
//@[9:11)    Number |10|
//@[12:14)   LogicalOr ||||
//@[15:16)   NumericLiteralSyntax
//@[15:16)    Number |4|
//@[16:18) NewLine |\n\n|

// conditional
//@[14:15) NewLine |\n|
var ternary = null ? 4 : false
//@[0:30) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:11)  IdentifierSyntax
//@[4:11)   Identifier |ternary|
//@[12:13)  Assignment |=|
//@[14:30)  TernaryOperationSyntax
//@[14:18)   NullLiteralSyntax
//@[14:18)    NullKeyword |null|
//@[19:20)   Question |?|
//@[21:22)   NumericLiteralSyntax
//@[21:22)    Number |4|
//@[23:24)   Colon |:|
//@[25:30)   BooleanLiteralSyntax
//@[25:30)    FalseKeyword |false|
//@[30:32) NewLine |\n\n|

// complex expressions
//@[22:23) NewLine |\n|
var complex = test(2 + 3*4, true || false && null)
//@[0:50) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:11)  IdentifierSyntax
//@[4:11)   Identifier |complex|
//@[12:13)  Assignment |=|
//@[14:50)  FunctionCallSyntax
//@[14:18)   IdentifierSyntax
//@[14:18)    Identifier |test|
//@[18:19)   LeftParen |(|
//@[19:27)   FunctionArgumentSyntax
//@[19:26)    BinaryOperationSyntax
//@[19:20)     NumericLiteralSyntax
//@[19:20)      Number |2|
//@[21:22)     Plus |+|
//@[23:26)     BinaryOperationSyntax
//@[23:24)      NumericLiteralSyntax
//@[23:24)       Number |3|
//@[24:25)      Asterisk |*|
//@[25:26)      NumericLiteralSyntax
//@[25:26)       Number |4|
//@[26:27)    Comma |,|
//@[28:49)   FunctionArgumentSyntax
//@[28:49)    BinaryOperationSyntax
//@[28:32)     BooleanLiteralSyntax
//@[28:32)      TrueKeyword |true|
//@[33:35)     LogicalOr ||||
//@[36:49)     BinaryOperationSyntax
//@[36:41)      BooleanLiteralSyntax
//@[36:41)       FalseKeyword |false|
//@[42:44)      LogicalAnd |&&|
//@[45:49)      NullLiteralSyntax
//@[45:49)       NullKeyword |null|
//@[49:50)   RightParen |)|
//@[50:51) NewLine |\n|
var complex = -2 && 3 && !4 && 5
//@[0:32) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:11)  IdentifierSyntax
//@[4:11)   Identifier |complex|
//@[12:13)  Assignment |=|
//@[14:32)  BinaryOperationSyntax
//@[14:27)   BinaryOperationSyntax
//@[14:21)    BinaryOperationSyntax
//@[14:16)     UnaryOperationSyntax
//@[14:15)      Minus |-|
//@[15:16)      NumericLiteralSyntax
//@[15:16)       Number |2|
//@[17:19)     LogicalAnd |&&|
//@[20:21)     NumericLiteralSyntax
//@[20:21)      Number |3|
//@[22:24)    LogicalAnd |&&|
//@[25:27)    UnaryOperationSyntax
//@[25:26)     Exclamation |!|
//@[26:27)     NumericLiteralSyntax
//@[26:27)      Number |4|
//@[28:30)   LogicalAnd |&&|
//@[31:32)   NumericLiteralSyntax
//@[31:32)    Number |5|
//@[32:33) NewLine |\n|
var complex = null ? !4: false
//@[0:30) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:11)  IdentifierSyntax
//@[4:11)   Identifier |complex|
//@[12:13)  Assignment |=|
//@[14:30)  TernaryOperationSyntax
//@[14:18)   NullLiteralSyntax
//@[14:18)    NullKeyword |null|
//@[19:20)   Question |?|
//@[21:23)   UnaryOperationSyntax
//@[21:22)    Exclamation |!|
//@[22:23)    NumericLiteralSyntax
//@[22:23)     Number |4|
//@[23:24)   Colon |:|
//@[25:30)   BooleanLiteralSyntax
//@[25:30)    FalseKeyword |false|
//@[30:31) NewLine |\n|
var complex = true == false != null == 4 != 'a' ? -2 && 3 && !4 && 5 : true || false && null
//@[0:92) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:11)  IdentifierSyntax
//@[4:11)   Identifier |complex|
//@[12:13)  Assignment |=|
//@[14:92)  TernaryOperationSyntax
//@[14:47)   BinaryOperationSyntax
//@[14:40)    BinaryOperationSyntax
//@[14:35)     BinaryOperationSyntax
//@[14:27)      BinaryOperationSyntax
//@[14:18)       BooleanLiteralSyntax
//@[14:18)        TrueKeyword |true|
//@[19:21)       Equals |==|
//@[22:27)       BooleanLiteralSyntax
//@[22:27)        FalseKeyword |false|
//@[28:30)      NotEquals |!=|
//@[31:35)      NullLiteralSyntax
//@[31:35)       NullKeyword |null|
//@[36:38)     Equals |==|
//@[39:40)     NumericLiteralSyntax
//@[39:40)      Number |4|
//@[41:43)    NotEquals |!=|
//@[44:47)    StringSyntax
//@[44:47)     StringComplete |'a'|
//@[48:49)   Question |?|
//@[50:68)   BinaryOperationSyntax
//@[50:63)    BinaryOperationSyntax
//@[50:57)     BinaryOperationSyntax
//@[50:52)      UnaryOperationSyntax
//@[50:51)       Minus |-|
//@[51:52)       NumericLiteralSyntax
//@[51:52)        Number |2|
//@[53:55)      LogicalAnd |&&|
//@[56:57)      NumericLiteralSyntax
//@[56:57)       Number |3|
//@[58:60)     LogicalAnd |&&|
//@[61:63)     UnaryOperationSyntax
//@[61:62)      Exclamation |!|
//@[62:63)      NumericLiteralSyntax
//@[62:63)       Number |4|
//@[64:66)    LogicalAnd |&&|
//@[67:68)    NumericLiteralSyntax
//@[67:68)     Number |5|
//@[69:70)   Colon |:|
//@[71:92)   BinaryOperationSyntax
//@[71:75)    BooleanLiteralSyntax
//@[71:75)     TrueKeyword |true|
//@[76:78)    LogicalOr ||||
//@[79:92)    BinaryOperationSyntax
//@[79:84)     BooleanLiteralSyntax
//@[79:84)      FalseKeyword |false|
//@[85:87)     LogicalAnd |&&|
//@[88:92)     NullLiteralSyntax
//@[88:92)      NullKeyword |null|
//@[92:94) NewLine |\n\n|

var nestedTernary = null ? 1 : 2 ? true ? 'a': 'b' : false ? 'd' : 15
//@[0:69) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:17)  IdentifierSyntax
//@[4:17)   Identifier |nestedTernary|
//@[18:19)  Assignment |=|
//@[20:69)  TernaryOperationSyntax
//@[20:24)   NullLiteralSyntax
//@[20:24)    NullKeyword |null|
//@[25:26)   Question |?|
//@[27:28)   NumericLiteralSyntax
//@[27:28)    Number |1|
//@[29:30)   Colon |:|
//@[31:69)   TernaryOperationSyntax
//@[31:32)    NumericLiteralSyntax
//@[31:32)     Number |2|
//@[33:34)    Question |?|
//@[35:50)    TernaryOperationSyntax
//@[35:39)     BooleanLiteralSyntax
//@[35:39)      TrueKeyword |true|
//@[40:41)     Question |?|
//@[42:45)     StringSyntax
//@[42:45)      StringComplete |'a'|
//@[45:46)     Colon |:|
//@[47:50)     StringSyntax
//@[47:50)      StringComplete |'b'|
//@[51:52)    Colon |:|
//@[53:69)    TernaryOperationSyntax
//@[53:58)     BooleanLiteralSyntax
//@[53:58)      FalseKeyword |false|
//@[59:60)     Question |?|
//@[61:64)     StringSyntax
//@[61:64)      StringComplete |'d'|
//@[65:66)     Colon |:|
//@[67:69)     NumericLiteralSyntax
//@[67:69)      Number |15|
//@[69:70) NewLine |\n|
var nestedTernary = (null ? 1 : 2) ? (true ? 'a': 'b') : (false ? 'd' : 15)
//@[0:75) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:17)  IdentifierSyntax
//@[4:17)   Identifier |nestedTernary|
//@[18:19)  Assignment |=|
//@[20:75)  TernaryOperationSyntax
//@[20:34)   ParenthesizedExpressionSyntax
//@[20:21)    LeftParen |(|
//@[21:33)    TernaryOperationSyntax
//@[21:25)     NullLiteralSyntax
//@[21:25)      NullKeyword |null|
//@[26:27)     Question |?|
//@[28:29)     NumericLiteralSyntax
//@[28:29)      Number |1|
//@[30:31)     Colon |:|
//@[32:33)     NumericLiteralSyntax
//@[32:33)      Number |2|
//@[33:34)    RightParen |)|
//@[35:36)   Question |?|
//@[37:54)   ParenthesizedExpressionSyntax
//@[37:38)    LeftParen |(|
//@[38:53)    TernaryOperationSyntax
//@[38:42)     BooleanLiteralSyntax
//@[38:42)      TrueKeyword |true|
//@[43:44)     Question |?|
//@[45:48)     StringSyntax
//@[45:48)      StringComplete |'a'|
//@[48:49)     Colon |:|
//@[50:53)     StringSyntax
//@[50:53)      StringComplete |'b'|
//@[53:54)    RightParen |)|
//@[55:56)   Colon |:|
//@[57:75)   ParenthesizedExpressionSyntax
//@[57:58)    LeftParen |(|
//@[58:74)    TernaryOperationSyntax
//@[58:63)     BooleanLiteralSyntax
//@[58:63)      FalseKeyword |false|
//@[64:65)     Question |?|
//@[66:69)     StringSyntax
//@[66:69)      StringComplete |'d'|
//@[70:71)     Colon |:|
//@[72:74)     NumericLiteralSyntax
//@[72:74)      Number |15|
//@[74:75)    RightParen |)|
//@[75:77) NewLine |\n\n|

// bad array access
//@[19:20) NewLine |\n|
var errorInsideArrayAccess = [
//@[0:44) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:26)  IdentifierSyntax
//@[4:26)   Identifier |errorInsideArrayAccess|
//@[27:28)  Assignment |=|
//@[29:44)  ArrayAccessSyntax
//@[29:40)   ArraySyntax
//@[29:30)    LeftSquare |[|
//@[30:31)    NewLine |\n|
  !null
//@[2:8)    ArrayItemSyntax
//@[2:7)     UnaryOperationSyntax
//@[2:3)      Exclamation |!|
//@[3:7)      NullLiteralSyntax
//@[3:7)       NullKeyword |null|
//@[7:8)     NewLine |\n|
][!0]
//@[0:1)    RightSquare |]|
//@[1:2)   LeftSquare |[|
//@[2:4)   UnaryOperationSyntax
//@[2:3)    Exclamation |!|
//@[3:4)    NumericLiteralSyntax
//@[3:4)     Number |0|
//@[4:5)   RightSquare |]|
//@[5:6) NewLine |\n|
var integerIndexOnNonArray = (null)[0]
//@[0:38) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:26)  IdentifierSyntax
//@[4:26)   Identifier |integerIndexOnNonArray|
//@[27:28)  Assignment |=|
//@[29:38)  ArrayAccessSyntax
//@[29:35)   ParenthesizedExpressionSyntax
//@[29:30)    LeftParen |(|
//@[30:34)    NullLiteralSyntax
//@[30:34)     NullKeyword |null|
//@[34:35)    RightParen |)|
//@[35:36)   LeftSquare |[|
//@[36:37)   NumericLiteralSyntax
//@[36:37)    Number |0|
//@[37:38)   RightSquare |]|
//@[38:39) NewLine |\n|
var stringIndexOnNonObject = 'test'['test']
//@[0:43) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:26)  IdentifierSyntax
//@[4:26)   Identifier |stringIndexOnNonObject|
//@[27:28)  Assignment |=|
//@[29:43)  ArrayAccessSyntax
//@[29:35)   StringSyntax
//@[29:35)    StringComplete |'test'|
//@[35:36)   LeftSquare |[|
//@[36:42)   StringSyntax
//@[36:42)    StringComplete |'test'|
//@[42:43)   RightSquare |]|
//@[43:44) NewLine |\n|
var malformedStringIndex = {
//@[0:40) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:24)  IdentifierSyntax
//@[4:24)   Identifier |malformedStringIndex|
//@[25:26)  Assignment |=|
//@[27:40)  ArrayAccessSyntax
//@[27:30)   ObjectSyntax
//@[27:28)    LeftBrace |{|
//@[28:29)    NewLine |\n|
}['test\e']
//@[0:1)    RightBrace |}|
//@[1:2)   LeftSquare |[|
//@[2:10)   SkippedTriviaSyntax
//@[2:10)    StringComplete |'test\e'|
//@[10:11)   RightSquare |]|
//@[11:12) NewLine |\n|
var invalidIndexTypeOverAny = any(true)[true]
//@[0:45) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:27)  IdentifierSyntax
//@[4:27)   Identifier |invalidIndexTypeOverAny|
//@[28:29)  Assignment |=|
//@[30:45)  ArrayAccessSyntax
//@[30:39)   FunctionCallSyntax
//@[30:33)    IdentifierSyntax
//@[30:33)     Identifier |any|
//@[33:34)    LeftParen |(|
//@[34:38)    FunctionArgumentSyntax
//@[34:38)     BooleanLiteralSyntax
//@[34:38)      TrueKeyword |true|
//@[38:39)    RightParen |)|
//@[39:40)   LeftSquare |[|
//@[40:44)   BooleanLiteralSyntax
//@[40:44)    TrueKeyword |true|
//@[44:45)   RightSquare |]|
//@[45:46) NewLine |\n|
var badIndexOverArray = [][null]
//@[0:32) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:21)  IdentifierSyntax
//@[4:21)   Identifier |badIndexOverArray|
//@[22:23)  Assignment |=|
//@[24:32)  ArrayAccessSyntax
//@[24:26)   ArraySyntax
//@[24:25)    LeftSquare |[|
//@[25:26)    RightSquare |]|
//@[26:27)   LeftSquare |[|
//@[27:31)   NullLiteralSyntax
//@[27:31)    NullKeyword |null|
//@[31:32)   RightSquare |]|
//@[32:33) NewLine |\n|
var badIndexOverArray2 = []['s']
//@[0:32) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:22)  IdentifierSyntax
//@[4:22)   Identifier |badIndexOverArray2|
//@[23:24)  Assignment |=|
//@[25:32)  ArrayAccessSyntax
//@[25:27)   ArraySyntax
//@[25:26)    LeftSquare |[|
//@[26:27)    RightSquare |]|
//@[27:28)   LeftSquare |[|
//@[28:31)   StringSyntax
//@[28:31)    StringComplete |'s'|
//@[31:32)   RightSquare |]|
//@[32:33) NewLine |\n|
var badIndexOverObj = {}[true]
//@[0:30) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:19)  IdentifierSyntax
//@[4:19)   Identifier |badIndexOverObj|
//@[20:21)  Assignment |=|
//@[22:30)  ArrayAccessSyntax
//@[22:24)   ObjectSyntax
//@[22:23)    LeftBrace |{|
//@[23:24)    RightBrace |}|
//@[24:25)   LeftSquare |[|
//@[25:29)   BooleanLiteralSyntax
//@[25:29)    TrueKeyword |true|
//@[29:30)   RightSquare |]|
//@[30:31) NewLine |\n|
var badIndexOverObj2 = {}[0]
//@[0:28) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:20)  IdentifierSyntax
//@[4:20)   Identifier |badIndexOverObj2|
//@[21:22)  Assignment |=|
//@[23:28)  ArrayAccessSyntax
//@[23:25)   ObjectSyntax
//@[23:24)    LeftBrace |{|
//@[24:25)    RightBrace |}|
//@[25:26)   LeftSquare |[|
//@[26:27)   NumericLiteralSyntax
//@[26:27)    Number |0|
//@[27:28)   RightSquare |]|
//@[28:29) NewLine |\n|
var badExpressionIndexer = {}[base64('a')]
//@[0:42) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:24)  IdentifierSyntax
//@[4:24)   Identifier |badExpressionIndexer|
//@[25:26)  Assignment |=|
//@[27:42)  ArrayAccessSyntax
//@[27:29)   ObjectSyntax
//@[27:28)    LeftBrace |{|
//@[28:29)    RightBrace |}|
//@[29:30)   LeftSquare |[|
//@[30:41)   FunctionCallSyntax
//@[30:36)    IdentifierSyntax
//@[30:36)     Identifier |base64|
//@[36:37)    LeftParen |(|
//@[37:40)    FunctionArgumentSyntax
//@[37:40)     StringSyntax
//@[37:40)      StringComplete |'a'|
//@[40:41)    RightParen |)|
//@[41:42)   RightSquare |]|
//@[42:44) NewLine |\n\n|

// bad propertyAccess
//@[21:22) NewLine |\n|
var dotAccessOnNonObject = true.foo
//@[0:35) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:24)  IdentifierSyntax
//@[4:24)   Identifier |dotAccessOnNonObject|
//@[25:26)  Assignment |=|
//@[27:35)  PropertyAccessSyntax
//@[27:31)   BooleanLiteralSyntax
//@[27:31)    TrueKeyword |true|
//@[31:32)   Dot |.|
//@[32:35)   IdentifierSyntax
//@[32:35)    Identifier |foo|
//@[35:36) NewLine |\n|
var badExpressionInPropertyAccess = resourceGroup()[!'location']
//@[0:64) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:33)  IdentifierSyntax
//@[4:33)   Identifier |badExpressionInPropertyAccess|
//@[34:35)  Assignment |=|
//@[36:64)  ArrayAccessSyntax
//@[36:51)   FunctionCallSyntax
//@[36:49)    IdentifierSyntax
//@[36:49)     Identifier |resourceGroup|
//@[49:50)    LeftParen |(|
//@[50:51)    RightParen |)|
//@[51:52)   LeftSquare |[|
//@[52:63)   UnaryOperationSyntax
//@[52:53)    Exclamation |!|
//@[53:63)    StringSyntax
//@[53:63)     StringComplete |'location'|
//@[63:64)   RightSquare |]|
//@[64:66) NewLine |\n\n|

var propertyAccessOnVariable = x.foo
//@[0:36) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:28)  IdentifierSyntax
//@[4:28)   Identifier |propertyAccessOnVariable|
//@[29:30)  Assignment |=|
//@[31:36)  PropertyAccessSyntax
//@[31:32)   VariableAccessSyntax
//@[31:32)    IdentifierSyntax
//@[31:32)     Identifier |x|
//@[32:33)   Dot |.|
//@[33:36)   IdentifierSyntax
//@[33:36)    Identifier |foo|
//@[36:38) NewLine |\n\n|

// function used like a variable
//@[32:33) NewLine |\n|
var funcvarvar = concat + base64 || !uniqueString
//@[0:49) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:14)  IdentifierSyntax
//@[4:14)   Identifier |funcvarvar|
//@[15:16)  Assignment |=|
//@[17:49)  BinaryOperationSyntax
//@[17:32)   BinaryOperationSyntax
//@[17:23)    VariableAccessSyntax
//@[17:23)     IdentifierSyntax
//@[17:23)      Identifier |concat|
//@[24:25)    Plus |+|
//@[26:32)    VariableAccessSyntax
//@[26:32)     IdentifierSyntax
//@[26:32)      Identifier |base64|
//@[33:35)   LogicalOr ||||
//@[36:49)   UnaryOperationSyntax
//@[36:37)    Exclamation |!|
//@[37:49)    VariableAccessSyntax
//@[37:49)     IdentifierSyntax
//@[37:49)      Identifier |uniqueString|
//@[49:50) NewLine |\n|
param funcvarparam bool = concat
//@[0:32) ParameterDeclarationSyntax
//@[0:5)  Identifier |param|
//@[6:18)  IdentifierSyntax
//@[6:18)   Identifier |funcvarparam|
//@[19:23)  TypeSyntax
//@[19:23)   Identifier |bool|
//@[24:32)  ParameterDefaultValueSyntax
//@[24:25)   Assignment |=|
//@[26:32)   VariableAccessSyntax
//@[26:32)    IdentifierSyntax
//@[26:32)     Identifier |concat|
//@[32:33) NewLine |\n|
output funcvarout array = padLeft
//@[0:33) OutputDeclarationSyntax
//@[0:6)  Identifier |output|
//@[7:17)  IdentifierSyntax
//@[7:17)   Identifier |funcvarout|
//@[18:23)  TypeSyntax
//@[18:23)   Identifier |array|
//@[24:25)  Assignment |=|
//@[26:33)  VariableAccessSyntax
//@[26:33)   IdentifierSyntax
//@[26:33)    Identifier |padLeft|
//@[33:35) NewLine |\n\n|

// non-existent function
//@[24:25) NewLine |\n|
var fakeFunc = red() + green() * orange()
//@[0:41) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:12)  IdentifierSyntax
//@[4:12)   Identifier |fakeFunc|
//@[13:14)  Assignment |=|
//@[15:41)  BinaryOperationSyntax
//@[15:20)   FunctionCallSyntax
//@[15:18)    IdentifierSyntax
//@[15:18)     Identifier |red|
//@[18:19)    LeftParen |(|
//@[19:20)    RightParen |)|
//@[21:22)   Plus |+|
//@[23:41)   BinaryOperationSyntax
//@[23:30)    FunctionCallSyntax
//@[23:28)     IdentifierSyntax
//@[23:28)      Identifier |green|
//@[28:29)     LeftParen |(|
//@[29:30)     RightParen |)|
//@[31:32)    Asterisk |*|
//@[33:41)    FunctionCallSyntax
//@[33:39)     IdentifierSyntax
//@[33:39)      Identifier |orange|
//@[39:40)     LeftParen |(|
//@[40:41)     RightParen |)|
//@[41:42) NewLine |\n|
param fakeFuncP string {
//@[0:44) ParameterDeclarationSyntax
//@[0:5)  Identifier |param|
//@[6:15)  IdentifierSyntax
//@[6:15)   Identifier |fakeFuncP|
//@[16:22)  TypeSyntax
//@[16:22)   Identifier |string|
//@[23:44)  ObjectSyntax
//@[23:24)   LeftBrace |{|
//@[24:25)   NewLine |\n|
  default: blue()
//@[2:18)   ObjectPropertySyntax
//@[2:9)    IdentifierSyntax
//@[2:9)     Identifier |default|
//@[9:10)    Colon |:|
//@[11:17)    FunctionCallSyntax
//@[11:15)     IdentifierSyntax
//@[11:15)      Identifier |blue|
//@[15:16)     LeftParen |(|
//@[16:17)     RightParen |)|
//@[17:18)    NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

// non-existent variable
//@[24:25) NewLine |\n|
var fakeVar = concat(totallyFakeVar, 's')
//@[0:41) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:11)  IdentifierSyntax
//@[4:11)   Identifier |fakeVar|
//@[12:13)  Assignment |=|
//@[14:41)  FunctionCallSyntax
//@[14:20)   IdentifierSyntax
//@[14:20)    Identifier |concat|
//@[20:21)   LeftParen |(|
//@[21:36)   FunctionArgumentSyntax
//@[21:35)    VariableAccessSyntax
//@[21:35)     IdentifierSyntax
//@[21:35)      Identifier |totallyFakeVar|
//@[35:36)    Comma |,|
//@[37:40)   FunctionArgumentSyntax
//@[37:40)    StringSyntax
//@[37:40)     StringComplete |'s'|
//@[40:41)   RightParen |)|
//@[41:43) NewLine |\n\n|

// bad functions arguments
//@[26:27) NewLine |\n|
var concatNotEnough = concat()
//@[0:30) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:19)  IdentifierSyntax
//@[4:19)   Identifier |concatNotEnough|
//@[20:21)  Assignment |=|
//@[22:30)  FunctionCallSyntax
//@[22:28)   IdentifierSyntax
//@[22:28)    Identifier |concat|
//@[28:29)   LeftParen |(|
//@[29:30)   RightParen |)|
//@[30:31) NewLine |\n|
var padLeftNotEnough = padLeft('s')
//@[0:35) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:20)  IdentifierSyntax
//@[4:20)   Identifier |padLeftNotEnough|
//@[21:22)  Assignment |=|
//@[23:35)  FunctionCallSyntax
//@[23:30)   IdentifierSyntax
//@[23:30)    Identifier |padLeft|
//@[30:31)   LeftParen |(|
//@[31:34)   FunctionArgumentSyntax
//@[31:34)    StringSyntax
//@[31:34)     StringComplete |'s'|
//@[34:35)   RightParen |)|
//@[35:36) NewLine |\n|
var takeTooMany = take([
//@[0:35) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:15)  IdentifierSyntax
//@[4:15)   Identifier |takeTooMany|
//@[16:17)  Assignment |=|
//@[18:35)  FunctionCallSyntax
//@[18:22)   IdentifierSyntax
//@[18:22)    Identifier |take|
//@[22:23)   LeftParen |(|
//@[23:27)   FunctionArgumentSyntax
//@[23:26)    ArraySyntax
//@[23:24)     LeftSquare |[|
//@[24:25)     NewLine |\n|
],1,2,'s')
//@[0:1)     RightSquare |]|
//@[1:2)    Comma |,|
//@[2:4)   FunctionArgumentSyntax
//@[2:3)    NumericLiteralSyntax
//@[2:3)     Number |1|
//@[3:4)    Comma |,|
//@[4:6)   FunctionArgumentSyntax
//@[4:5)    NumericLiteralSyntax
//@[4:5)     Number |2|
//@[5:6)    Comma |,|
//@[6:9)   FunctionArgumentSyntax
//@[6:9)    StringSyntax
//@[6:9)     StringComplete |'s'|
//@[9:10)   RightParen |)|
//@[10:12) NewLine |\n\n|

// wrong argument types
//@[23:24) NewLine |\n|
var concatWrongTypes = concat({
//@[0:34) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:20)  IdentifierSyntax
//@[4:20)   Identifier |concatWrongTypes|
//@[21:22)  Assignment |=|
//@[23:34)  FunctionCallSyntax
//@[23:29)   IdentifierSyntax
//@[23:29)    Identifier |concat|
//@[29:30)   LeftParen |(|
//@[30:33)   FunctionArgumentSyntax
//@[30:33)    ObjectSyntax
//@[30:31)     LeftBrace |{|
//@[31:32)     NewLine |\n|
})
//@[0:1)     RightBrace |}|
//@[1:2)   RightParen |)|
//@[2:3) NewLine |\n|
var concatWrongTypesContradiction = concat('s', [
//@[0:52) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:33)  IdentifierSyntax
//@[4:33)   Identifier |concatWrongTypesContradiction|
//@[34:35)  Assignment |=|
//@[36:52)  FunctionCallSyntax
//@[36:42)   IdentifierSyntax
//@[36:42)    Identifier |concat|
//@[42:43)   LeftParen |(|
//@[43:47)   FunctionArgumentSyntax
//@[43:46)    StringSyntax
//@[43:46)     StringComplete |'s'|
//@[46:47)    Comma |,|
//@[48:51)   FunctionArgumentSyntax
//@[48:51)    ArraySyntax
//@[48:49)     LeftSquare |[|
//@[49:50)     NewLine |\n|
])
//@[0:1)     RightSquare |]|
//@[1:2)   RightParen |)|
//@[2:3) NewLine |\n|
var indexOfWrongTypes = indexOf(1,1)
//@[0:36) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:21)  IdentifierSyntax
//@[4:21)   Identifier |indexOfWrongTypes|
//@[22:23)  Assignment |=|
//@[24:36)  FunctionCallSyntax
//@[24:31)   IdentifierSyntax
//@[24:31)    Identifier |indexOf|
//@[31:32)   LeftParen |(|
//@[32:34)   FunctionArgumentSyntax
//@[32:33)    NumericLiteralSyntax
//@[32:33)     Number |1|
//@[33:34)    Comma |,|
//@[34:35)   FunctionArgumentSyntax
//@[34:35)    NumericLiteralSyntax
//@[34:35)     Number |1|
//@[35:36)   RightParen |)|
//@[36:38) NewLine |\n\n|

// not enough params
//@[20:21) NewLine |\n|
var test1 = listKeys('abcd')
//@[0:28) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:9)  IdentifierSyntax
//@[4:9)   Identifier |test1|
//@[10:11)  Assignment |=|
//@[12:28)  FunctionCallSyntax
//@[12:20)   IdentifierSyntax
//@[12:20)    Identifier |listKeys|
//@[20:21)   LeftParen |(|
//@[21:27)   FunctionArgumentSyntax
//@[21:27)    StringSyntax
//@[21:27)     StringComplete |'abcd'|
//@[27:28)   RightParen |)|
//@[28:30) NewLine |\n\n|

// list spelled wrong 
//@[22:23) NewLine |\n|
var test2 = lsitKeys('abcd', '2020-01-01')
//@[0:42) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:9)  IdentifierSyntax
//@[4:9)   Identifier |test2|
//@[10:11)  Assignment |=|
//@[12:42)  FunctionCallSyntax
//@[12:20)   IdentifierSyntax
//@[12:20)    Identifier |lsitKeys|
//@[20:21)   LeftParen |(|
//@[21:28)   FunctionArgumentSyntax
//@[21:27)    StringSyntax
//@[21:27)     StringComplete |'abcd'|
//@[27:28)    Comma |,|
//@[29:41)   FunctionArgumentSyntax
//@[29:41)    StringSyntax
//@[29:41)     StringComplete |'2020-01-01'|
//@[41:42)   RightParen |)|
//@[42:44) NewLine |\n\n|

// just 'list' 
//@[15:16) NewLine |\n|
var test3 = list('abcd', '2020-01-01')
//@[0:38) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:9)  IdentifierSyntax
//@[4:9)   Identifier |test3|
//@[10:11)  Assignment |=|
//@[12:38)  FunctionCallSyntax
//@[12:16)   IdentifierSyntax
//@[12:16)    Identifier |list|
//@[16:17)   LeftParen |(|
//@[17:24)   FunctionArgumentSyntax
//@[17:23)    StringSyntax
//@[17:23)     StringComplete |'abcd'|
//@[23:24)    Comma |,|
//@[25:37)   FunctionArgumentSyntax
//@[25:37)    StringSyntax
//@[25:37)     StringComplete |'2020-01-01'|
//@[37:38)   RightParen |)|
//@[38:40) NewLine |\n\n|

// cannot compile an expression like this
//@[41:42) NewLine |\n|
var emitLimit = [
//@[0:315) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:13)  IdentifierSyntax
//@[4:13)   Identifier |emitLimit|
//@[14:15)  Assignment |=|
//@[16:315)  ArraySyntax
//@[16:17)   LeftSquare |[|
//@[17:18)   NewLine |\n|
  concat('s')
//@[2:14)   ArrayItemSyntax
//@[2:13)    FunctionCallSyntax
//@[2:8)     IdentifierSyntax
//@[2:8)      Identifier |concat|
//@[8:9)     LeftParen |(|
//@[9:12)     FunctionArgumentSyntax
//@[9:12)      StringSyntax
//@[9:12)       StringComplete |'s'|
//@[12:13)     RightParen |)|
//@[13:14)    NewLine |\n|
  '${4}'
//@[2:9)   ArrayItemSyntax
//@[2:8)    StringSyntax
//@[2:5)     StringLeftPiece |'${|
//@[5:6)     NumericLiteralSyntax
//@[5:6)      Number |4|
//@[6:8)     StringRightPiece |}'|
//@[8:9)    NewLine |\n|
  {
//@[2:273)   ArrayItemSyntax
//@[2:272)    ObjectSyntax
//@[2:3)     LeftBrace |{|
//@[3:4)     NewLine |\n|
    a: {
//@[4:265)     ObjectPropertySyntax
//@[4:5)      IdentifierSyntax
//@[4:5)       Identifier |a|
//@[5:6)      Colon |:|
//@[7:264)      ObjectSyntax
//@[7:8)       LeftBrace |{|
//@[8:9)       NewLine |\n|
      b: base64('s')
//@[6:21)       ObjectPropertySyntax
//@[6:7)        IdentifierSyntax
//@[6:7)         Identifier |b|
//@[7:8)        Colon |:|
//@[9:20)        FunctionCallSyntax
//@[9:15)         IdentifierSyntax
//@[9:15)          Identifier |base64|
//@[15:16)         LeftParen |(|
//@[16:19)         FunctionArgumentSyntax
//@[16:19)          StringSyntax
//@[16:19)           StringComplete |'s'|
//@[19:20)         RightParen |)|
//@[20:21)        NewLine |\n|
      c: union({
//@[6:91)       ObjectPropertySyntax
//@[6:7)        IdentifierSyntax
//@[6:7)         Identifier |c|
//@[7:8)        Colon |:|
//@[9:90)        FunctionCallSyntax
//@[9:14)         IdentifierSyntax
//@[9:14)          Identifier |union|
//@[14:15)         LeftParen |(|
//@[15:43)         FunctionArgumentSyntax
//@[15:42)          ObjectSyntax
//@[15:16)           LeftBrace |{|
//@[16:17)           NewLine |\n|
        a: 12 + 3
//@[8:18)           ObjectPropertySyntax
//@[8:9)            IdentifierSyntax
//@[8:9)             Identifier |a|
//@[9:10)            Colon |:|
//@[11:17)            BinaryOperationSyntax
//@[11:13)             NumericLiteralSyntax
//@[11:13)              Number |12|
//@[14:15)             Plus |+|
//@[16:17)             NumericLiteralSyntax
//@[16:17)              Number |3|
//@[17:18)            NewLine |\n|
      }, {
//@[6:7)           RightBrace |}|
//@[7:8)          Comma |,|
//@[9:54)         FunctionArgumentSyntax
//@[9:54)          ObjectSyntax
//@[9:10)           LeftBrace |{|
//@[10:11)           NewLine |\n|
        b: !true
//@[8:17)           ObjectPropertySyntax
//@[8:9)            IdentifierSyntax
//@[8:9)             Identifier |b|
//@[9:10)            Colon |:|
//@[11:16)            UnaryOperationSyntax
//@[11:12)             Exclamation |!|
//@[12:16)             BooleanLiteralSyntax
//@[12:16)              TrueKeyword |true|
//@[16:17)            NewLine |\n|
        c: 'hello'
//@[8:19)           ObjectPropertySyntax
//@[8:9)            IdentifierSyntax
//@[8:9)             Identifier |c|
//@[9:10)            Colon |:|
//@[11:18)            StringSyntax
//@[11:18)             StringComplete |'hello'|
//@[18:19)            NewLine |\n|
      })
//@[6:7)           RightBrace |}|
//@[7:8)         RightParen |)|
//@[8:9)        NewLine |\n|
      d: resourceGroup().location
//@[6:34)       ObjectPropertySyntax
//@[6:7)        IdentifierSyntax
//@[6:7)         Identifier |d|
//@[7:8)        Colon |:|
//@[9:33)        PropertyAccessSyntax
//@[9:24)         FunctionCallSyntax
//@[9:22)          IdentifierSyntax
//@[9:22)           Identifier |resourceGroup|
//@[22:23)          LeftParen |(|
//@[23:24)          RightParen |)|
//@[24:25)         Dot |.|
//@[25:33)         IdentifierSyntax
//@[25:33)          Identifier |location|
//@[33:34)        NewLine |\n|
      e: union({
//@[6:46)       ObjectPropertySyntax
//@[6:7)        IdentifierSyntax
//@[6:7)         Identifier |e|
//@[7:8)        Colon |:|
//@[9:45)        FunctionCallSyntax
//@[9:14)         IdentifierSyntax
//@[9:14)          Identifier |union|
//@[14:15)         LeftParen |(|
//@[15:41)         FunctionArgumentSyntax
//@[15:40)          ObjectSyntax
//@[15:16)           LeftBrace |{|
//@[16:17)           NewLine |\n|
        x: true
//@[8:16)           ObjectPropertySyntax
//@[8:9)            IdentifierSyntax
//@[8:9)             Identifier |x|
//@[9:10)            Colon |:|
//@[11:15)            BooleanLiteralSyntax
//@[11:15)             TrueKeyword |true|
//@[15:16)            NewLine |\n|
      }, {})
//@[6:7)           RightBrace |}|
//@[7:8)          Comma |,|
//@[9:11)         FunctionArgumentSyntax
//@[9:11)          ObjectSyntax
//@[9:10)           LeftBrace |{|
//@[10:11)           RightBrace |}|
//@[11:12)         RightParen |)|
//@[12:13)        NewLine |\n|
      f: intersection({
//@[6:58)       ObjectPropertySyntax
//@[6:7)        IdentifierSyntax
//@[6:7)         Identifier |f|
//@[7:8)        Colon |:|
//@[9:57)        FunctionCallSyntax
//@[9:21)         IdentifierSyntax
//@[9:21)          Identifier |intersection|
//@[21:22)         LeftParen |(|
//@[22:53)         FunctionArgumentSyntax
//@[22:52)          ObjectSyntax
//@[22:23)           LeftBrace |{|
//@[23:24)           NewLine |\n|
        q: 's' == 12
//@[8:21)           ObjectPropertySyntax
//@[8:9)            IdentifierSyntax
//@[8:9)             Identifier |q|
//@[9:10)            Colon |:|
//@[11:20)            BinaryOperationSyntax
//@[11:14)             StringSyntax
//@[11:14)              StringComplete |'s'|
//@[15:17)             Equals |==|
//@[18:20)             NumericLiteralSyntax
//@[18:20)              Number |12|
//@[20:21)            NewLine |\n|
      }, {})
//@[6:7)           RightBrace |}|
//@[7:8)          Comma |,|
//@[9:11)         FunctionArgumentSyntax
//@[9:11)          ObjectSyntax
//@[9:10)           LeftBrace |{|
//@[10:11)           RightBrace |}|
//@[11:12)         RightParen |)|
//@[12:13)        NewLine |\n|
    }
//@[4:5)       RightBrace |}|
//@[5:6)      NewLine |\n|
  }
//@[2:3)     RightBrace |}|
//@[3:4)    NewLine |\n|
]
//@[0:1)   RightSquare |]|
//@[1:3) NewLine |\n\n|

// cannot compile an expression like this
//@[41:42) NewLine |\n|
var emitLimit2 = {
//@[0:115) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:14)  IdentifierSyntax
//@[4:14)   Identifier |emitLimit2|
//@[15:16)  Assignment |=|
//@[17:115)  ObjectSyntax
//@[17:18)   LeftBrace |{|
//@[18:19)   NewLine |\n|
  a: {
//@[2:95)   ObjectPropertySyntax
//@[2:3)    IdentifierSyntax
//@[2:3)     Identifier |a|
//@[3:4)    Colon |:|
//@[5:94)    ObjectSyntax
//@[5:6)     LeftBrace |{|
//@[6:7)     NewLine |\n|
    b: {
//@[4:54)     ObjectPropertySyntax
//@[4:5)      IdentifierSyntax
//@[4:5)       Identifier |b|
//@[5:6)      Colon |:|
//@[7:53)      BinaryOperationSyntax
//@[7:48)       ObjectSyntax
//@[7:8)        LeftBrace |{|
//@[8:9)        NewLine |\n|
      a: resourceGroup().location
//@[6:34)        ObjectPropertySyntax
//@[6:7)         IdentifierSyntax
//@[6:7)          Identifier |a|
//@[7:8)         Colon |:|
//@[9:33)         PropertyAccessSyntax
//@[9:24)          FunctionCallSyntax
//@[9:22)           IdentifierSyntax
//@[9:22)            Identifier |resourceGroup|
//@[22:23)           LeftParen |(|
//@[23:24)           RightParen |)|
//@[24:25)          Dot |.|
//@[25:33)          IdentifierSyntax
//@[25:33)           Identifier |location|
//@[33:34)         NewLine |\n|
    } == 2
//@[4:5)        RightBrace |}|
//@[6:8)       Equals |==|
//@[9:10)       NumericLiteralSyntax
//@[9:10)        Number |2|
//@[10:11)      NewLine |\n|
    c: concat([
//@[4:30)     ObjectPropertySyntax
//@[4:5)      IdentifierSyntax
//@[4:5)       Identifier |c|
//@[5:6)      Colon |:|
//@[7:29)      FunctionCallSyntax
//@[7:13)       IdentifierSyntax
//@[7:13)        Identifier |concat|
//@[13:14)       LeftParen |(|
//@[14:23)       FunctionArgumentSyntax
//@[14:22)        ArraySyntax
//@[14:15)         LeftSquare |[|
//@[15:17)         NewLine |\n\n|

    ], true)
//@[4:5)         RightSquare |]|
//@[5:6)        Comma |,|
//@[7:11)       FunctionArgumentSyntax
//@[7:11)        BooleanLiteralSyntax
//@[7:11)         TrueKeyword |true|
//@[11:12)       RightParen |)|
//@[12:13)      NewLine |\n|
  }
//@[2:3)     RightBrace |}|
//@[3:4)    NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

var sampleObject = {
//@[0:190) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:16)  IdentifierSyntax
//@[4:16)   Identifier |sampleObject|
//@[17:18)  Assignment |=|
//@[19:190)  ObjectSyntax
//@[19:20)   LeftBrace |{|
//@[20:21)   NewLine |\n|
  myInt: 42
//@[2:12)   ObjectPropertySyntax
//@[2:7)    IdentifierSyntax
//@[2:7)     Identifier |myInt|
//@[7:8)    Colon |:|
//@[9:11)    NumericLiteralSyntax
//@[9:11)     Number |42|
//@[11:12)    NewLine |\n|
  myStr: 's'
//@[2:13)   ObjectPropertySyntax
//@[2:7)    IdentifierSyntax
//@[2:7)     Identifier |myStr|
//@[7:8)    Colon |:|
//@[9:12)    StringSyntax
//@[9:12)     StringComplete |'s'|
//@[12:13)    NewLine |\n|
  myBool: false
//@[2:16)   ObjectPropertySyntax
//@[2:8)    IdentifierSyntax
//@[2:8)     Identifier |myBool|
//@[8:9)    Colon |:|
//@[10:15)    BooleanLiteralSyntax
//@[10:15)     FalseKeyword |false|
//@[15:16)    NewLine |\n|
  myNull: null
//@[2:15)   ObjectPropertySyntax
//@[2:8)    IdentifierSyntax
//@[2:8)     Identifier |myNull|
//@[8:9)    Colon |:|
//@[10:14)    NullLiteralSyntax
//@[10:14)     NullKeyword |null|
//@[14:15)    NewLine |\n|
  myInner: {
//@[2:79)   ObjectPropertySyntax
//@[2:9)    IdentifierSyntax
//@[2:9)     Identifier |myInner|
//@[9:10)    Colon |:|
//@[11:78)    ObjectSyntax
//@[11:12)     LeftBrace |{|
//@[12:13)     NewLine |\n|
    anotherStr: 'a'
//@[4:20)     ObjectPropertySyntax
//@[4:14)      IdentifierSyntax
//@[4:14)       Identifier |anotherStr|
//@[14:15)      Colon |:|
//@[16:19)      StringSyntax
//@[16:19)       StringComplete |'a'|
//@[19:20)      NewLine |\n|
    otherArr: [
//@[4:42)     ObjectPropertySyntax
//@[4:12)      IdentifierSyntax
//@[4:12)       Identifier |otherArr|
//@[12:13)      Colon |:|
//@[14:41)      ArraySyntax
//@[14:15)       LeftSquare |[|
//@[15:16)       NewLine |\n|
      's'
//@[6:10)       ArrayItemSyntax
//@[6:9)        StringSyntax
//@[6:9)         StringComplete |'s'|
//@[9:10)        NewLine |\n|
      'a'
//@[6:10)       ArrayItemSyntax
//@[6:9)        StringSyntax
//@[6:9)         StringComplete |'a'|
//@[9:10)        NewLine |\n|
    ]
//@[4:5)       RightSquare |]|
//@[5:6)      NewLine |\n|
  }
//@[2:3)     RightBrace |}|
//@[3:4)    NewLine |\n|
  myArr: [
//@[2:33)   ObjectPropertySyntax
//@[2:7)    IdentifierSyntax
//@[2:7)     Identifier |myArr|
//@[7:8)    Colon |:|
//@[9:32)    ArraySyntax
//@[9:10)     LeftSquare |[|
//@[10:11)     NewLine |\n|
    1
//@[4:6)     ArrayItemSyntax
//@[4:5)      NumericLiteralSyntax
//@[4:5)       Number |1|
//@[5:6)      NewLine |\n|
    2
//@[4:6)     ArrayItemSyntax
//@[4:5)      NumericLiteralSyntax
//@[4:5)       Number |2|
//@[5:6)      NewLine |\n|
    3
//@[4:6)     ArrayItemSyntax
//@[4:5)      NumericLiteralSyntax
//@[4:5)       Number |3|
//@[5:6)      NewLine |\n|
  ]
//@[2:3)     RightSquare |]|
//@[3:4)    NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

var badProperty = sampleObject.myFake
//@[0:37) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:15)  IdentifierSyntax
//@[4:15)   Identifier |badProperty|
//@[16:17)  Assignment |=|
//@[18:37)  PropertyAccessSyntax
//@[18:30)   VariableAccessSyntax
//@[18:30)    IdentifierSyntax
//@[18:30)     Identifier |sampleObject|
//@[30:31)   Dot |.|
//@[31:37)   IdentifierSyntax
//@[31:37)    Identifier |myFake|
//@[37:38) NewLine |\n|
var badPropertyIndexer = sampleObject['fake']
//@[0:45) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:22)  IdentifierSyntax
//@[4:22)   Identifier |badPropertyIndexer|
//@[23:24)  Assignment |=|
//@[25:45)  ArrayAccessSyntax
//@[25:37)   VariableAccessSyntax
//@[25:37)    IdentifierSyntax
//@[25:37)     Identifier |sampleObject|
//@[37:38)   LeftSquare |[|
//@[38:44)   StringSyntax
//@[38:44)    StringComplete |'fake'|
//@[44:45)   RightSquare |]|
//@[45:46) NewLine |\n|
var badType = sampleObject.myStr / 32
//@[0:37) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:11)  IdentifierSyntax
//@[4:11)   Identifier |badType|
//@[12:13)  Assignment |=|
//@[14:37)  BinaryOperationSyntax
//@[14:32)   PropertyAccessSyntax
//@[14:26)    VariableAccessSyntax
//@[14:26)     IdentifierSyntax
//@[14:26)      Identifier |sampleObject|
//@[26:27)    Dot |.|
//@[27:32)    IdentifierSyntax
//@[27:32)     Identifier |myStr|
//@[33:34)   Slash |/|
//@[35:37)   NumericLiteralSyntax
//@[35:37)    Number |32|
//@[37:38) NewLine |\n|
var badInnerProperty = sampleObject.myInner.fake
//@[0:48) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:20)  IdentifierSyntax
//@[4:20)   Identifier |badInnerProperty|
//@[21:22)  Assignment |=|
//@[23:48)  PropertyAccessSyntax
//@[23:43)   PropertyAccessSyntax
//@[23:35)    VariableAccessSyntax
//@[23:35)     IdentifierSyntax
//@[23:35)      Identifier |sampleObject|
//@[35:36)    Dot |.|
//@[36:43)    IdentifierSyntax
//@[36:43)     Identifier |myInner|
//@[43:44)   Dot |.|
//@[44:48)   IdentifierSyntax
//@[44:48)    Identifier |fake|
//@[48:49) NewLine |\n|
var badInnerType = sampleObject.myInner.anotherStr + 2
//@[0:54) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:16)  IdentifierSyntax
//@[4:16)   Identifier |badInnerType|
//@[17:18)  Assignment |=|
//@[19:54)  BinaryOperationSyntax
//@[19:50)   PropertyAccessSyntax
//@[19:39)    PropertyAccessSyntax
//@[19:31)     VariableAccessSyntax
//@[19:31)      IdentifierSyntax
//@[19:31)       Identifier |sampleObject|
//@[31:32)     Dot |.|
//@[32:39)     IdentifierSyntax
//@[32:39)      Identifier |myInner|
//@[39:40)    Dot |.|
//@[40:50)    IdentifierSyntax
//@[40:50)     Identifier |anotherStr|
//@[51:52)   Plus |+|
//@[53:54)   NumericLiteralSyntax
//@[53:54)    Number |2|
//@[54:55) NewLine |\n|
var badArrayIndexer = sampleObject.myArr['s']
//@[0:45) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:19)  IdentifierSyntax
//@[4:19)   Identifier |badArrayIndexer|
//@[20:21)  Assignment |=|
//@[22:45)  ArrayAccessSyntax
//@[22:40)   PropertyAccessSyntax
//@[22:34)    VariableAccessSyntax
//@[22:34)     IdentifierSyntax
//@[22:34)      Identifier |sampleObject|
//@[34:35)    Dot |.|
//@[35:40)    IdentifierSyntax
//@[35:40)     Identifier |myArr|
//@[40:41)   LeftSquare |[|
//@[41:44)   StringSyntax
//@[41:44)    StringComplete |'s'|
//@[44:45)   RightSquare |]|
//@[45:46) NewLine |\n|
var badInnerArrayIndexer = sampleObject.myInner.otherArr['s']
//@[0:61) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:24)  IdentifierSyntax
//@[4:24)   Identifier |badInnerArrayIndexer|
//@[25:26)  Assignment |=|
//@[27:61)  ArrayAccessSyntax
//@[27:56)   PropertyAccessSyntax
//@[27:47)    PropertyAccessSyntax
//@[27:39)     VariableAccessSyntax
//@[27:39)      IdentifierSyntax
//@[27:39)       Identifier |sampleObject|
//@[39:40)     Dot |.|
//@[40:47)     IdentifierSyntax
//@[40:47)      Identifier |myInner|
//@[47:48)    Dot |.|
//@[48:56)    IdentifierSyntax
//@[48:56)     Identifier |otherArr|
//@[56:57)   LeftSquare |[|
//@[57:60)   StringSyntax
//@[57:60)    StringComplete |'s'|
//@[60:61)   RightSquare |]|
//@[61:62) NewLine |\n|
var badIndexer = sampleObject.myStr['s']
//@[0:40) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:14)  IdentifierSyntax
//@[4:14)   Identifier |badIndexer|
//@[15:16)  Assignment |=|
//@[17:40)  ArrayAccessSyntax
//@[17:35)   PropertyAccessSyntax
//@[17:29)    VariableAccessSyntax
//@[17:29)     IdentifierSyntax
//@[17:29)      Identifier |sampleObject|
//@[29:30)    Dot |.|
//@[30:35)    IdentifierSyntax
//@[30:35)     Identifier |myStr|
//@[35:36)   LeftSquare |[|
//@[36:39)   StringSyntax
//@[36:39)    StringComplete |'s'|
//@[39:40)   RightSquare |]|
//@[40:41) NewLine |\n|
var badInnerArray = sampleObject.myInner.fakeArr['s']
//@[0:53) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:17)  IdentifierSyntax
//@[4:17)   Identifier |badInnerArray|
//@[18:19)  Assignment |=|
//@[20:53)  ArrayAccessSyntax
//@[20:48)   PropertyAccessSyntax
//@[20:40)    PropertyAccessSyntax
//@[20:32)     VariableAccessSyntax
//@[20:32)      IdentifierSyntax
//@[20:32)       Identifier |sampleObject|
//@[32:33)     Dot |.|
//@[33:40)     IdentifierSyntax
//@[33:40)      Identifier |myInner|
//@[40:41)    Dot |.|
//@[41:48)    IdentifierSyntax
//@[41:48)     Identifier |fakeArr|
//@[48:49)   LeftSquare |[|
//@[49:52)   StringSyntax
//@[49:52)    StringComplete |'s'|
//@[52:53)   RightSquare |]|
//@[53:54) NewLine |\n|
var invalidPropertyCallOnInstanceFunctionAccess = a.b.c.bar().baz
//@[0:65) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:47)  IdentifierSyntax
//@[4:47)   Identifier |invalidPropertyCallOnInstanceFunctionAccess|
//@[48:49)  Assignment |=|
//@[50:65)  PropertyAccessSyntax
//@[50:61)   InstanceFunctionCallSyntax
//@[50:55)    PropertyAccessSyntax
//@[50:53)     PropertyAccessSyntax
//@[50:51)      VariableAccessSyntax
//@[50:51)       IdentifierSyntax
//@[50:51)        Identifier |a|
//@[51:52)      Dot |.|
//@[52:53)      IdentifierSyntax
//@[52:53)       Identifier |b|
//@[53:54)     Dot |.|
//@[54:55)     IdentifierSyntax
//@[54:55)      Identifier |c|
//@[55:56)    Dot |.|
//@[56:59)    IdentifierSyntax
//@[56:59)     Identifier |bar|
//@[59:60)    LeftParen |(|
//@[60:61)    RightParen |)|
//@[61:62)   Dot |.|
//@[62:65)   IdentifierSyntax
//@[62:65)    Identifier |baz|
//@[65:66) NewLine |\n|
var invalidInstanceFunctionAccess = a.b.c.bar()
//@[0:47) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:33)  IdentifierSyntax
//@[4:33)   Identifier |invalidInstanceFunctionAccess|
//@[34:35)  Assignment |=|
//@[36:47)  InstanceFunctionCallSyntax
//@[36:41)   PropertyAccessSyntax
//@[36:39)    PropertyAccessSyntax
//@[36:37)     VariableAccessSyntax
//@[36:37)      IdentifierSyntax
//@[36:37)       Identifier |a|
//@[37:38)     Dot |.|
//@[38:39)     IdentifierSyntax
//@[38:39)      Identifier |b|
//@[39:40)    Dot |.|
//@[40:41)    IdentifierSyntax
//@[40:41)     Identifier |c|
//@[41:42)   Dot |.|
//@[42:45)   IdentifierSyntax
//@[42:45)    Identifier |bar|
//@[45:46)   LeftParen |(|
//@[46:47)   RightParen |)|
//@[47:48) NewLine |\n|

//@[0:0) EndOfFile ||
