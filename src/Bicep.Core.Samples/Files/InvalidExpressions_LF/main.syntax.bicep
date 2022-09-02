/*
//@[00:5984) ProgramSyntax
  This tests the various cases of invalid expressions.
*/
//@[02:0004) ├─Token(NewLine) |\n\n|

// bad expressions
//@[18:0019) ├─Token(NewLine) |\n|
var bad = a+
//@[00:0012) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0007) | ├─IdentifierSyntax
//@[04:0007) | | └─Token(Identifier) |bad|
//@[08:0009) | ├─Token(Assignment) |=|
//@[10:0012) | └─BinaryOperationSyntax
//@[10:0011) |   ├─VariableAccessSyntax
//@[10:0011) |   | └─IdentifierSyntax
//@[10:0011) |   |   └─Token(Identifier) |a|
//@[11:0012) |   ├─Token(Plus) |+|
//@[12:0012) |   └─SkippedTriviaSyntax
//@[12:0013) ├─Token(NewLine) |\n|
var bad = *
//@[00:0011) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0007) | ├─IdentifierSyntax
//@[04:0007) | | └─Token(Identifier) |bad|
//@[08:0009) | ├─Token(Assignment) |=|
//@[10:0011) | └─SkippedTriviaSyntax
//@[10:0011) |   └─Token(Asterisk) |*|
//@[11:0012) ├─Token(NewLine) |\n|
var bad = /
//@[00:0011) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0007) | ├─IdentifierSyntax
//@[04:0007) | | └─Token(Identifier) |bad|
//@[08:0009) | ├─Token(Assignment) |=|
//@[10:0011) | └─SkippedTriviaSyntax
//@[10:0011) |   └─Token(Slash) |/|
//@[11:0012) ├─Token(NewLine) |\n|
var bad = %
//@[00:0011) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0007) | ├─IdentifierSyntax
//@[04:0007) | | └─Token(Identifier) |bad|
//@[08:0009) | ├─Token(Assignment) |=|
//@[10:0011) | └─SkippedTriviaSyntax
//@[10:0011) |   └─Token(Modulo) |%|
//@[11:0012) ├─Token(NewLine) |\n|
var bad = 33-
//@[00:0013) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0007) | ├─IdentifierSyntax
//@[04:0007) | | └─Token(Identifier) |bad|
//@[08:0009) | ├─Token(Assignment) |=|
//@[10:0013) | └─BinaryOperationSyntax
//@[10:0012) |   ├─IntegerLiteralSyntax
//@[10:0012) |   | └─Token(Integer) |33|
//@[12:0013) |   ├─Token(Minus) |-|
//@[13:0013) |   └─SkippedTriviaSyntax
//@[13:0014) ├─Token(NewLine) |\n|
var bad = --33
//@[00:0014) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0007) | ├─IdentifierSyntax
//@[04:0007) | | └─Token(Identifier) |bad|
//@[08:0009) | ├─Token(Assignment) |=|
//@[10:0014) | └─UnaryOperationSyntax
//@[10:0011) |   ├─Token(Minus) |-|
//@[11:0014) |   └─SkippedTriviaSyntax
//@[11:0012) |     ├─Token(Minus) |-|
//@[12:0014) |     └─Token(Integer) |33|
//@[14:0015) ├─Token(NewLine) |\n|
var bad = 3 * 4 /
//@[00:0017) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0007) | ├─IdentifierSyntax
//@[04:0007) | | └─Token(Identifier) |bad|
//@[08:0009) | ├─Token(Assignment) |=|
//@[10:0017) | └─BinaryOperationSyntax
//@[10:0015) |   ├─BinaryOperationSyntax
//@[10:0011) |   | ├─IntegerLiteralSyntax
//@[10:0011) |   | | └─Token(Integer) |3|
//@[12:0013) |   | ├─Token(Asterisk) |*|
//@[14:0015) |   | └─IntegerLiteralSyntax
//@[14:0015) |   |   └─Token(Integer) |4|
//@[16:0017) |   ├─Token(Slash) |/|
//@[17:0017) |   └─SkippedTriviaSyntax
//@[17:0018) ├─Token(NewLine) |\n|
var bad = 222222222222222222222222222222222222222222 * 4
//@[00:0056) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0007) | ├─IdentifierSyntax
//@[04:0007) | | └─Token(Identifier) |bad|
//@[08:0009) | ├─Token(Assignment) |=|
//@[10:0056) | └─SkippedTriviaSyntax
//@[10:0052) |   ├─Token(Integer) |222222222222222222222222222222222222222222|
//@[53:0054) |   ├─Token(Asterisk) |*|
//@[55:0056) |   └─Token(Integer) |4|
//@[56:0057) ├─Token(NewLine) |\n|
var bad = (null) ?
//@[00:0018) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0007) | ├─IdentifierSyntax
//@[04:0007) | | └─Token(Identifier) |bad|
//@[08:0009) | ├─Token(Assignment) |=|
//@[10:0018) | └─TernaryOperationSyntax
//@[10:0016) |   ├─ParenthesizedExpressionSyntax
//@[10:0011) |   | ├─Token(LeftParen) |(|
//@[11:0015) |   | ├─NullLiteralSyntax
//@[11:0015) |   | | └─Token(NullKeyword) |null|
//@[15:0016) |   | └─Token(RightParen) |)|
//@[17:0018) |   ├─Token(Question) |?|
//@[18:0018) |   ├─SkippedTriviaSyntax
//@[18:0018) |   ├─SkippedTriviaSyntax
//@[18:0018) |   └─SkippedTriviaSyntax
//@[18:0019) ├─Token(NewLine) |\n|
var bad = (null) ? :
//@[00:0020) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0007) | ├─IdentifierSyntax
//@[04:0007) | | └─Token(Identifier) |bad|
//@[08:0009) | ├─Token(Assignment) |=|
//@[10:0020) | └─TernaryOperationSyntax
//@[10:0016) |   ├─ParenthesizedExpressionSyntax
//@[10:0011) |   | ├─Token(LeftParen) |(|
//@[11:0015) |   | ├─NullLiteralSyntax
//@[11:0015) |   | | └─Token(NullKeyword) |null|
//@[15:0016) |   | └─Token(RightParen) |)|
//@[17:0018) |   ├─Token(Question) |?|
//@[19:0019) |   ├─SkippedTriviaSyntax
//@[19:0020) |   ├─Token(Colon) |:|
//@[20:0020) |   └─SkippedTriviaSyntax
//@[20:0021) ├─Token(NewLine) |\n|
var bad = (null) ? !
//@[00:0020) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0007) | ├─IdentifierSyntax
//@[04:0007) | | └─Token(Identifier) |bad|
//@[08:0009) | ├─Token(Assignment) |=|
//@[10:0020) | └─TernaryOperationSyntax
//@[10:0016) |   ├─ParenthesizedExpressionSyntax
//@[10:0011) |   | ├─Token(LeftParen) |(|
//@[11:0015) |   | ├─NullLiteralSyntax
//@[11:0015) |   | | └─Token(NullKeyword) |null|
//@[15:0016) |   | └─Token(RightParen) |)|
//@[17:0018) |   ├─Token(Question) |?|
//@[19:0020) |   ├─UnaryOperationSyntax
//@[19:0020) |   | ├─Token(Exclamation) |!|
//@[20:0020) |   | └─SkippedTriviaSyntax
//@[20:0020) |   ├─SkippedTriviaSyntax
//@[20:0020) |   └─SkippedTriviaSyntax
//@[20:0021) ├─Token(NewLine) |\n|
var bad = (null)!
//@[00:0016) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0007) | ├─IdentifierSyntax
//@[04:0007) | | └─Token(Identifier) |bad|
//@[08:0009) | ├─Token(Assignment) |=|
//@[10:0016) | └─ParenthesizedExpressionSyntax
//@[10:0011) |   ├─Token(LeftParen) |(|
//@[11:0015) |   ├─NullLiteralSyntax
//@[11:0015) |   | └─Token(NullKeyword) |null|
//@[15:0016) |   └─Token(RightParen) |)|
//@[16:0018) ├─SkippedTriviaSyntax
//@[16:0017) | ├─Token(Exclamation) |!|
//@[17:0018) | └─Token(NewLine) |\n|
var bad = (null)[0]
//@[00:0019) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0007) | ├─IdentifierSyntax
//@[04:0007) | | └─Token(Identifier) |bad|
//@[08:0009) | ├─Token(Assignment) |=|
//@[10:0019) | └─ArrayAccessSyntax
//@[10:0016) |   ├─ParenthesizedExpressionSyntax
//@[10:0011) |   | ├─Token(LeftParen) |(|
//@[11:0015) |   | ├─NullLiteralSyntax
//@[11:0015) |   | | └─Token(NullKeyword) |null|
//@[15:0016) |   | └─Token(RightParen) |)|
//@[16:0017) |   ├─Token(LeftSquare) |[|
//@[17:0018) |   ├─IntegerLiteralSyntax
//@[17:0018) |   | └─Token(Integer) |0|
//@[18:0019) |   └─Token(RightSquare) |]|
//@[19:0020) ├─Token(NewLine) |\n|
var bad = ()
//@[00:0012) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0007) | ├─IdentifierSyntax
//@[04:0007) | | └─Token(Identifier) |bad|
//@[08:0009) | ├─Token(Assignment) |=|
//@[10:0012) | └─ParenthesizedExpressionSyntax
//@[10:0011) |   ├─Token(LeftParen) |(|
//@[11:0011) |   ├─SkippedTriviaSyntax
//@[11:0012) |   └─Token(RightParen) |)|
//@[12:0013) ├─Token(NewLine) |\n|
var bad = 
//@[00:0010) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0007) | ├─IdentifierSyntax
//@[04:0007) | | └─Token(Identifier) |bad|
//@[08:0009) | ├─Token(Assignment) |=|
//@[10:0010) | └─SkippedTriviaSyntax
//@[10:0012) ├─Token(NewLine) |\n\n|

// variables not supported
//@[26:0027) ├─Token(NewLine) |\n|
var x = a + 2
//@[00:0013) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0005) | ├─IdentifierSyntax
//@[04:0005) | | └─Token(Identifier) |x|
//@[06:0007) | ├─Token(Assignment) |=|
//@[08:0013) | └─BinaryOperationSyntax
//@[08:0009) |   ├─VariableAccessSyntax
//@[08:0009) |   | └─IdentifierSyntax
//@[08:0009) |   |   └─Token(Identifier) |a|
//@[10:0011) |   ├─Token(Plus) |+|
//@[12:0013) |   └─IntegerLiteralSyntax
//@[12:0013) |     └─Token(Integer) |2|
//@[13:0015) ├─Token(NewLine) |\n\n|

// unary NOT
//@[12:0013) ├─Token(NewLine) |\n|
var not = !null
//@[00:0015) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0007) | ├─IdentifierSyntax
//@[04:0007) | | └─Token(Identifier) |not|
//@[08:0009) | ├─Token(Assignment) |=|
//@[10:0015) | └─UnaryOperationSyntax
//@[10:0011) |   ├─Token(Exclamation) |!|
//@[11:0015) |   └─NullLiteralSyntax
//@[11:0015) |     └─Token(NullKeyword) |null|
//@[15:0016) ├─Token(NewLine) |\n|
var not = !4
//@[00:0012) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0007) | ├─IdentifierSyntax
//@[04:0007) | | └─Token(Identifier) |not|
//@[08:0009) | ├─Token(Assignment) |=|
//@[10:0012) | └─UnaryOperationSyntax
//@[10:0011) |   ├─Token(Exclamation) |!|
//@[11:0012) |   └─IntegerLiteralSyntax
//@[11:0012) |     └─Token(Integer) |4|
//@[12:0013) ├─Token(NewLine) |\n|
var not = !'s'
//@[00:0014) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0007) | ├─IdentifierSyntax
//@[04:0007) | | └─Token(Identifier) |not|
//@[08:0009) | ├─Token(Assignment) |=|
//@[10:0014) | └─UnaryOperationSyntax
//@[10:0011) |   ├─Token(Exclamation) |!|
//@[11:0014) |   └─StringSyntax
//@[11:0014) |     └─Token(StringComplete) |'s'|
//@[14:0015) ├─Token(NewLine) |\n|
var not = ![
//@[00:0014) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0007) | ├─IdentifierSyntax
//@[04:0007) | | └─Token(Identifier) |not|
//@[08:0009) | ├─Token(Assignment) |=|
//@[10:0014) | └─UnaryOperationSyntax
//@[10:0011) |   ├─Token(Exclamation) |!|
//@[11:0014) |   └─ArraySyntax
//@[11:0012) |     ├─Token(LeftSquare) |[|
//@[12:0013) |     ├─Token(NewLine) |\n|
]
//@[00:0001) |     └─Token(RightSquare) |]|
//@[01:0002) ├─Token(NewLine) |\n|
var not = !{
//@[00:0014) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0007) | ├─IdentifierSyntax
//@[04:0007) | | └─Token(Identifier) |not|
//@[08:0009) | ├─Token(Assignment) |=|
//@[10:0014) | └─UnaryOperationSyntax
//@[10:0011) |   ├─Token(Exclamation) |!|
//@[11:0014) |   └─ObjectSyntax
//@[11:0012) |     ├─Token(LeftBrace) |{|
//@[12:0013) |     ├─Token(NewLine) |\n|
}
//@[00:0001) |     └─Token(RightBrace) |}|
//@[01:0003) ├─Token(NewLine) |\n\n|

// unary not chaining will be added in the future
//@[49:0050) ├─Token(NewLine) |\n|
var not = !!!!!!!true
//@[00:0021) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0007) | ├─IdentifierSyntax
//@[04:0007) | | └─Token(Identifier) |not|
//@[08:0009) | ├─Token(Assignment) |=|
//@[10:0021) | └─UnaryOperationSyntax
//@[10:0011) |   ├─Token(Exclamation) |!|
//@[11:0021) |   └─SkippedTriviaSyntax
//@[11:0012) |     ├─Token(Exclamation) |!|
//@[12:0013) |     ├─Token(Exclamation) |!|
//@[13:0014) |     ├─Token(Exclamation) |!|
//@[14:0015) |     ├─Token(Exclamation) |!|
//@[15:0016) |     ├─Token(Exclamation) |!|
//@[16:0017) |     ├─Token(Exclamation) |!|
//@[17:0021) |     └─Token(TrueKeyword) |true|
//@[21:0023) ├─Token(NewLine) |\n\n|

// unary minus chaining will not be supported (to reserve -- in case we need it)
//@[80:0081) ├─Token(NewLine) |\n|
var minus = ------12
//@[00:0020) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0009) | ├─IdentifierSyntax
//@[04:0009) | | └─Token(Identifier) |minus|
//@[10:0011) | ├─Token(Assignment) |=|
//@[12:0020) | └─UnaryOperationSyntax
//@[12:0013) |   ├─Token(Minus) |-|
//@[13:0020) |   └─SkippedTriviaSyntax
//@[13:0014) |     ├─Token(Minus) |-|
//@[14:0015) |     ├─Token(Minus) |-|
//@[15:0016) |     ├─Token(Minus) |-|
//@[16:0017) |     ├─Token(Minus) |-|
//@[17:0018) |     ├─Token(Minus) |-|
//@[18:0020) |     └─Token(Integer) |12|
//@[20:0022) ├─Token(NewLine) |\n\n|

// unary minus
//@[14:0015) ├─Token(NewLine) |\n|
var minus = -true
//@[00:0017) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0009) | ├─IdentifierSyntax
//@[04:0009) | | └─Token(Identifier) |minus|
//@[10:0011) | ├─Token(Assignment) |=|
//@[12:0017) | └─UnaryOperationSyntax
//@[12:0013) |   ├─Token(Minus) |-|
//@[13:0017) |   └─BooleanLiteralSyntax
//@[13:0017) |     └─Token(TrueKeyword) |true|
//@[17:0018) ├─Token(NewLine) |\n|
var minus = -null
//@[00:0017) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0009) | ├─IdentifierSyntax
//@[04:0009) | | └─Token(Identifier) |minus|
//@[10:0011) | ├─Token(Assignment) |=|
//@[12:0017) | └─UnaryOperationSyntax
//@[12:0013) |   ├─Token(Minus) |-|
//@[13:0017) |   └─NullLiteralSyntax
//@[13:0017) |     └─Token(NullKeyword) |null|
//@[17:0018) ├─Token(NewLine) |\n|
var minus = -'s'
//@[00:0016) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0009) | ├─IdentifierSyntax
//@[04:0009) | | └─Token(Identifier) |minus|
//@[10:0011) | ├─Token(Assignment) |=|
//@[12:0016) | └─UnaryOperationSyntax
//@[12:0013) |   ├─Token(Minus) |-|
//@[13:0016) |   └─StringSyntax
//@[13:0016) |     └─Token(StringComplete) |'s'|
//@[16:0017) ├─Token(NewLine) |\n|
var minus = -[
//@[00:0016) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0009) | ├─IdentifierSyntax
//@[04:0009) | | └─Token(Identifier) |minus|
//@[10:0011) | ├─Token(Assignment) |=|
//@[12:0016) | └─UnaryOperationSyntax
//@[12:0013) |   ├─Token(Minus) |-|
//@[13:0016) |   └─ArraySyntax
//@[13:0014) |     ├─Token(LeftSquare) |[|
//@[14:0015) |     ├─Token(NewLine) |\n|
]
//@[00:0001) |     └─Token(RightSquare) |]|
//@[01:0002) ├─Token(NewLine) |\n|
var minus = -{
//@[00:0016) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0009) | ├─IdentifierSyntax
//@[04:0009) | | └─Token(Identifier) |minus|
//@[10:0011) | ├─Token(Assignment) |=|
//@[12:0016) | └─UnaryOperationSyntax
//@[12:0013) |   ├─Token(Minus) |-|
//@[13:0016) |   └─ObjectSyntax
//@[13:0014) |     ├─Token(LeftBrace) |{|
//@[14:0015) |     ├─Token(NewLine) |\n|
}
//@[00:0001) |     └─Token(RightBrace) |}|
//@[01:0003) ├─Token(NewLine) |\n\n|

// multiplicative
//@[17:0018) ├─Token(NewLine) |\n|
var mod = 's' % true
//@[00:0020) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0007) | ├─IdentifierSyntax
//@[04:0007) | | └─Token(Identifier) |mod|
//@[08:0009) | ├─Token(Assignment) |=|
//@[10:0020) | └─BinaryOperationSyntax
//@[10:0013) |   ├─StringSyntax
//@[10:0013) |   | └─Token(StringComplete) |'s'|
//@[14:0015) |   ├─Token(Modulo) |%|
//@[16:0020) |   └─BooleanLiteralSyntax
//@[16:0020) |     └─Token(TrueKeyword) |true|
//@[20:0021) ├─Token(NewLine) |\n|
var mul = true * null
//@[00:0021) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0007) | ├─IdentifierSyntax
//@[04:0007) | | └─Token(Identifier) |mul|
//@[08:0009) | ├─Token(Assignment) |=|
//@[10:0021) | └─BinaryOperationSyntax
//@[10:0014) |   ├─BooleanLiteralSyntax
//@[10:0014) |   | └─Token(TrueKeyword) |true|
//@[15:0016) |   ├─Token(Asterisk) |*|
//@[17:0021) |   └─NullLiteralSyntax
//@[17:0021) |     └─Token(NullKeyword) |null|
//@[21:0022) ├─Token(NewLine) |\n|
var div = {
//@[00:0019) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0007) | ├─IdentifierSyntax
//@[04:0007) | | └─Token(Identifier) |div|
//@[08:0009) | ├─Token(Assignment) |=|
//@[10:0019) | └─BinaryOperationSyntax
//@[10:0013) |   ├─ObjectSyntax
//@[10:0011) |   | ├─Token(LeftBrace) |{|
//@[11:0012) |   | ├─Token(NewLine) |\n|
} / [
//@[00:0001) |   | └─Token(RightBrace) |}|
//@[02:0003) |   ├─Token(Slash) |/|
//@[04:0007) |   └─ArraySyntax
//@[04:0005) |     ├─Token(LeftSquare) |[|
//@[05:0006) |     ├─Token(NewLine) |\n|
]
//@[00:0001) |     └─Token(RightSquare) |]|
//@[01:0003) ├─Token(NewLine) |\n\n|

// additive
//@[11:0012) ├─Token(NewLine) |\n|
var add = null + 's'
//@[00:0020) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0007) | ├─IdentifierSyntax
//@[04:0007) | | └─Token(Identifier) |add|
//@[08:0009) | ├─Token(Assignment) |=|
//@[10:0020) | └─BinaryOperationSyntax
//@[10:0014) |   ├─NullLiteralSyntax
//@[10:0014) |   | └─Token(NullKeyword) |null|
//@[15:0016) |   ├─Token(Plus) |+|
//@[17:0020) |   └─StringSyntax
//@[17:0020) |     └─Token(StringComplete) |'s'|
//@[20:0021) ├─Token(NewLine) |\n|
var sub = true - false
//@[00:0022) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0007) | ├─IdentifierSyntax
//@[04:0007) | | └─Token(Identifier) |sub|
//@[08:0009) | ├─Token(Assignment) |=|
//@[10:0022) | └─BinaryOperationSyntax
//@[10:0014) |   ├─BooleanLiteralSyntax
//@[10:0014) |   | └─Token(TrueKeyword) |true|
//@[15:0016) |   ├─Token(Minus) |-|
//@[17:0022) |   └─BooleanLiteralSyntax
//@[17:0022) |     └─Token(FalseKeyword) |false|
//@[22:0023) ├─Token(NewLine) |\n|
var add = 'bad' + 'str'
//@[00:0023) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0007) | ├─IdentifierSyntax
//@[04:0007) | | └─Token(Identifier) |add|
//@[08:0009) | ├─Token(Assignment) |=|
//@[10:0023) | └─BinaryOperationSyntax
//@[10:0015) |   ├─StringSyntax
//@[10:0015) |   | └─Token(StringComplete) |'bad'|
//@[16:0017) |   ├─Token(Plus) |+|
//@[18:0023) |   └─StringSyntax
//@[18:0023) |     └─Token(StringComplete) |'str'|
//@[23:0025) ├─Token(NewLine) |\n\n|

// equality (== and != can't have a type error because they work on "any" type)
//@[79:0080) ├─Token(NewLine) |\n|
var eq = true =~ null
//@[00:0021) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0006) | ├─IdentifierSyntax
//@[04:0006) | | └─Token(Identifier) |eq|
//@[07:0008) | ├─Token(Assignment) |=|
//@[09:0021) | └─BinaryOperationSyntax
//@[09:0013) |   ├─BooleanLiteralSyntax
//@[09:0013) |   | └─Token(TrueKeyword) |true|
//@[14:0016) |   ├─Token(EqualsInsensitive) |=~|
//@[17:0021) |   └─NullLiteralSyntax
//@[17:0021) |     └─Token(NullKeyword) |null|
//@[21:0022) ├─Token(NewLine) |\n|
var ne = 15 !~ [
//@[00:0018) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0006) | ├─IdentifierSyntax
//@[04:0006) | | └─Token(Identifier) |ne|
//@[07:0008) | ├─Token(Assignment) |=|
//@[09:0018) | └─BinaryOperationSyntax
//@[09:0011) |   ├─IntegerLiteralSyntax
//@[09:0011) |   | └─Token(Integer) |15|
//@[12:0014) |   ├─Token(NotEqualsInsensitive) |!~|
//@[15:0018) |   └─ArraySyntax
//@[15:0016) |     ├─Token(LeftSquare) |[|
//@[16:0017) |     ├─Token(NewLine) |\n|
]
//@[00:0001) |     └─Token(RightSquare) |]|
//@[01:0003) ├─Token(NewLine) |\n\n|

// relational
//@[13:0014) ├─Token(NewLine) |\n|
var lt = 4 < 's'
//@[00:0016) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0006) | ├─IdentifierSyntax
//@[04:0006) | | └─Token(Identifier) |lt|
//@[07:0008) | ├─Token(Assignment) |=|
//@[09:0016) | └─BinaryOperationSyntax
//@[09:0010) |   ├─IntegerLiteralSyntax
//@[09:0010) |   | └─Token(Integer) |4|
//@[11:0012) |   ├─Token(LessThan) |<|
//@[13:0016) |   └─StringSyntax
//@[13:0016) |     └─Token(StringComplete) |'s'|
//@[16:0017) ├─Token(NewLine) |\n|
var lteq = null <= 10
//@[00:0021) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0008) | ├─IdentifierSyntax
//@[04:0008) | | └─Token(Identifier) |lteq|
//@[09:0010) | ├─Token(Assignment) |=|
//@[11:0021) | └─BinaryOperationSyntax
//@[11:0015) |   ├─NullLiteralSyntax
//@[11:0015) |   | └─Token(NullKeyword) |null|
//@[16:0018) |   ├─Token(LessThanOrEqual) |<=|
//@[19:0021) |   └─IntegerLiteralSyntax
//@[19:0021) |     └─Token(Integer) |10|
//@[21:0022) ├─Token(NewLine) |\n|
var gt = false>[
//@[00:0018) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0006) | ├─IdentifierSyntax
//@[04:0006) | | └─Token(Identifier) |gt|
//@[07:0008) | ├─Token(Assignment) |=|
//@[09:0018) | └─BinaryOperationSyntax
//@[09:0014) |   ├─BooleanLiteralSyntax
//@[09:0014) |   | └─Token(FalseKeyword) |false|
//@[14:0015) |   ├─Token(GreaterThan) |>|
//@[15:0018) |   └─ArraySyntax
//@[15:0016) |     ├─Token(LeftSquare) |[|
//@[16:0017) |     ├─Token(NewLine) |\n|
]
//@[00:0001) |     └─Token(RightSquare) |]|
//@[01:0002) ├─Token(NewLine) |\n|
var gteq = {
//@[00:0023) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0008) | ├─IdentifierSyntax
//@[04:0008) | | └─Token(Identifier) |gteq|
//@[09:0010) | ├─Token(Assignment) |=|
//@[11:0023) | └─BinaryOperationSyntax
//@[11:0014) |   ├─ObjectSyntax
//@[11:0012) |   | ├─Token(LeftBrace) |{|
//@[12:0013) |   | ├─Token(NewLine) |\n|
} >= false
//@[00:0001) |   | └─Token(RightBrace) |}|
//@[02:0004) |   ├─Token(GreaterThanOrEqual) |>=|
//@[05:0010) |   └─BooleanLiteralSyntax
//@[05:0010) |     └─Token(FalseKeyword) |false|
//@[10:0012) ├─Token(NewLine) |\n\n|

// logical
//@[10:0011) ├─Token(NewLine) |\n|
var and = null && 'a'
//@[00:0021) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0007) | ├─IdentifierSyntax
//@[04:0007) | | └─Token(Identifier) |and|
//@[08:0009) | ├─Token(Assignment) |=|
//@[10:0021) | └─BinaryOperationSyntax
//@[10:0014) |   ├─NullLiteralSyntax
//@[10:0014) |   | └─Token(NullKeyword) |null|
//@[15:0017) |   ├─Token(LogicalAnd) |&&|
//@[18:0021) |   └─StringSyntax
//@[18:0021) |     └─Token(StringComplete) |'a'|
//@[21:0022) ├─Token(NewLine) |\n|
var or = 10 || 4
//@[00:0016) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0006) | ├─IdentifierSyntax
//@[04:0006) | | └─Token(Identifier) |or|
//@[07:0008) | ├─Token(Assignment) |=|
//@[09:0016) | └─BinaryOperationSyntax
//@[09:0011) |   ├─IntegerLiteralSyntax
//@[09:0011) |   | └─Token(Integer) |10|
//@[12:0014) |   ├─Token(LogicalOr) ||||
//@[15:0016) |   └─IntegerLiteralSyntax
//@[15:0016) |     └─Token(Integer) |4|
//@[16:0018) ├─Token(NewLine) |\n\n|

// conditional
//@[14:0015) ├─Token(NewLine) |\n|
var ternary = null ? 4 : false
//@[00:0030) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0011) | ├─IdentifierSyntax
//@[04:0011) | | └─Token(Identifier) |ternary|
//@[12:0013) | ├─Token(Assignment) |=|
//@[14:0030) | └─TernaryOperationSyntax
//@[14:0018) |   ├─NullLiteralSyntax
//@[14:0018) |   | └─Token(NullKeyword) |null|
//@[19:0020) |   ├─Token(Question) |?|
//@[21:0022) |   ├─IntegerLiteralSyntax
//@[21:0022) |   | └─Token(Integer) |4|
//@[23:0024) |   ├─Token(Colon) |:|
//@[25:0030) |   └─BooleanLiteralSyntax
//@[25:0030) |     └─Token(FalseKeyword) |false|
//@[30:0032) ├─Token(NewLine) |\n\n|

// complex expressions
//@[22:0023) ├─Token(NewLine) |\n|
var complex = test(2 + 3*4, true || false && null)
//@[00:0050) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0011) | ├─IdentifierSyntax
//@[04:0011) | | └─Token(Identifier) |complex|
//@[12:0013) | ├─Token(Assignment) |=|
//@[14:0050) | └─FunctionCallSyntax
//@[14:0018) |   ├─IdentifierSyntax
//@[14:0018) |   | └─Token(Identifier) |test|
//@[18:0019) |   ├─Token(LeftParen) |(|
//@[19:0026) |   ├─FunctionArgumentSyntax
//@[19:0026) |   | └─BinaryOperationSyntax
//@[19:0020) |   |   ├─IntegerLiteralSyntax
//@[19:0020) |   |   | └─Token(Integer) |2|
//@[21:0022) |   |   ├─Token(Plus) |+|
//@[23:0026) |   |   └─BinaryOperationSyntax
//@[23:0024) |   |     ├─IntegerLiteralSyntax
//@[23:0024) |   |     | └─Token(Integer) |3|
//@[24:0025) |   |     ├─Token(Asterisk) |*|
//@[25:0026) |   |     └─IntegerLiteralSyntax
//@[25:0026) |   |       └─Token(Integer) |4|
//@[26:0027) |   ├─Token(Comma) |,|
//@[28:0049) |   ├─FunctionArgumentSyntax
//@[28:0049) |   | └─BinaryOperationSyntax
//@[28:0032) |   |   ├─BooleanLiteralSyntax
//@[28:0032) |   |   | └─Token(TrueKeyword) |true|
//@[33:0035) |   |   ├─Token(LogicalOr) ||||
//@[36:0049) |   |   └─BinaryOperationSyntax
//@[36:0041) |   |     ├─BooleanLiteralSyntax
//@[36:0041) |   |     | └─Token(FalseKeyword) |false|
//@[42:0044) |   |     ├─Token(LogicalAnd) |&&|
//@[45:0049) |   |     └─NullLiteralSyntax
//@[45:0049) |   |       └─Token(NullKeyword) |null|
//@[49:0050) |   └─Token(RightParen) |)|
//@[50:0051) ├─Token(NewLine) |\n|
var complex = -2 && 3 && !4 && 5
//@[00:0032) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0011) | ├─IdentifierSyntax
//@[04:0011) | | └─Token(Identifier) |complex|
//@[12:0013) | ├─Token(Assignment) |=|
//@[14:0032) | └─BinaryOperationSyntax
//@[14:0027) |   ├─BinaryOperationSyntax
//@[14:0021) |   | ├─BinaryOperationSyntax
//@[14:0016) |   | | ├─UnaryOperationSyntax
//@[14:0015) |   | | | ├─Token(Minus) |-|
//@[15:0016) |   | | | └─IntegerLiteralSyntax
//@[15:0016) |   | | |   └─Token(Integer) |2|
//@[17:0019) |   | | ├─Token(LogicalAnd) |&&|
//@[20:0021) |   | | └─IntegerLiteralSyntax
//@[20:0021) |   | |   └─Token(Integer) |3|
//@[22:0024) |   | ├─Token(LogicalAnd) |&&|
//@[25:0027) |   | └─UnaryOperationSyntax
//@[25:0026) |   |   ├─Token(Exclamation) |!|
//@[26:0027) |   |   └─IntegerLiteralSyntax
//@[26:0027) |   |     └─Token(Integer) |4|
//@[28:0030) |   ├─Token(LogicalAnd) |&&|
//@[31:0032) |   └─IntegerLiteralSyntax
//@[31:0032) |     └─Token(Integer) |5|
//@[32:0033) ├─Token(NewLine) |\n|
var complex = null ? !4: false
//@[00:0030) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0011) | ├─IdentifierSyntax
//@[04:0011) | | └─Token(Identifier) |complex|
//@[12:0013) | ├─Token(Assignment) |=|
//@[14:0030) | └─TernaryOperationSyntax
//@[14:0018) |   ├─NullLiteralSyntax
//@[14:0018) |   | └─Token(NullKeyword) |null|
//@[19:0020) |   ├─Token(Question) |?|
//@[21:0023) |   ├─UnaryOperationSyntax
//@[21:0022) |   | ├─Token(Exclamation) |!|
//@[22:0023) |   | └─IntegerLiteralSyntax
//@[22:0023) |   |   └─Token(Integer) |4|
//@[23:0024) |   ├─Token(Colon) |:|
//@[25:0030) |   └─BooleanLiteralSyntax
//@[25:0030) |     └─Token(FalseKeyword) |false|
//@[30:0031) ├─Token(NewLine) |\n|
var complex = true == false != null == 4 != 'a' ? -2 && 3 && !4 && 5 : true || false && null
//@[00:0092) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0011) | ├─IdentifierSyntax
//@[04:0011) | | └─Token(Identifier) |complex|
//@[12:0013) | ├─Token(Assignment) |=|
//@[14:0092) | └─TernaryOperationSyntax
//@[14:0047) |   ├─BinaryOperationSyntax
//@[14:0040) |   | ├─BinaryOperationSyntax
//@[14:0035) |   | | ├─BinaryOperationSyntax
//@[14:0027) |   | | | ├─BinaryOperationSyntax
//@[14:0018) |   | | | | ├─BooleanLiteralSyntax
//@[14:0018) |   | | | | | └─Token(TrueKeyword) |true|
//@[19:0021) |   | | | | ├─Token(Equals) |==|
//@[22:0027) |   | | | | └─BooleanLiteralSyntax
//@[22:0027) |   | | | |   └─Token(FalseKeyword) |false|
//@[28:0030) |   | | | ├─Token(NotEquals) |!=|
//@[31:0035) |   | | | └─NullLiteralSyntax
//@[31:0035) |   | | |   └─Token(NullKeyword) |null|
//@[36:0038) |   | | ├─Token(Equals) |==|
//@[39:0040) |   | | └─IntegerLiteralSyntax
//@[39:0040) |   | |   └─Token(Integer) |4|
//@[41:0043) |   | ├─Token(NotEquals) |!=|
//@[44:0047) |   | └─StringSyntax
//@[44:0047) |   |   └─Token(StringComplete) |'a'|
//@[48:0049) |   ├─Token(Question) |?|
//@[50:0068) |   ├─BinaryOperationSyntax
//@[50:0063) |   | ├─BinaryOperationSyntax
//@[50:0057) |   | | ├─BinaryOperationSyntax
//@[50:0052) |   | | | ├─UnaryOperationSyntax
//@[50:0051) |   | | | | ├─Token(Minus) |-|
//@[51:0052) |   | | | | └─IntegerLiteralSyntax
//@[51:0052) |   | | | |   └─Token(Integer) |2|
//@[53:0055) |   | | | ├─Token(LogicalAnd) |&&|
//@[56:0057) |   | | | └─IntegerLiteralSyntax
//@[56:0057) |   | | |   └─Token(Integer) |3|
//@[58:0060) |   | | ├─Token(LogicalAnd) |&&|
//@[61:0063) |   | | └─UnaryOperationSyntax
//@[61:0062) |   | |   ├─Token(Exclamation) |!|
//@[62:0063) |   | |   └─IntegerLiteralSyntax
//@[62:0063) |   | |     └─Token(Integer) |4|
//@[64:0066) |   | ├─Token(LogicalAnd) |&&|
//@[67:0068) |   | └─IntegerLiteralSyntax
//@[67:0068) |   |   └─Token(Integer) |5|
//@[69:0070) |   ├─Token(Colon) |:|
//@[71:0092) |   └─BinaryOperationSyntax
//@[71:0075) |     ├─BooleanLiteralSyntax
//@[71:0075) |     | └─Token(TrueKeyword) |true|
//@[76:0078) |     ├─Token(LogicalOr) ||||
//@[79:0092) |     └─BinaryOperationSyntax
//@[79:0084) |       ├─BooleanLiteralSyntax
//@[79:0084) |       | └─Token(FalseKeyword) |false|
//@[85:0087) |       ├─Token(LogicalAnd) |&&|
//@[88:0092) |       └─NullLiteralSyntax
//@[88:0092) |         └─Token(NullKeyword) |null|
//@[92:0094) ├─Token(NewLine) |\n\n|

var nestedTernary = null ? 1 : 2 ? true ? 'a': 'b' : false ? 'd' : 15
//@[00:0069) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0017) | ├─IdentifierSyntax
//@[04:0017) | | └─Token(Identifier) |nestedTernary|
//@[18:0019) | ├─Token(Assignment) |=|
//@[20:0069) | └─TernaryOperationSyntax
//@[20:0024) |   ├─NullLiteralSyntax
//@[20:0024) |   | └─Token(NullKeyword) |null|
//@[25:0026) |   ├─Token(Question) |?|
//@[27:0028) |   ├─IntegerLiteralSyntax
//@[27:0028) |   | └─Token(Integer) |1|
//@[29:0030) |   ├─Token(Colon) |:|
//@[31:0069) |   └─TernaryOperationSyntax
//@[31:0032) |     ├─IntegerLiteralSyntax
//@[31:0032) |     | └─Token(Integer) |2|
//@[33:0034) |     ├─Token(Question) |?|
//@[35:0050) |     ├─TernaryOperationSyntax
//@[35:0039) |     | ├─BooleanLiteralSyntax
//@[35:0039) |     | | └─Token(TrueKeyword) |true|
//@[40:0041) |     | ├─Token(Question) |?|
//@[42:0045) |     | ├─StringSyntax
//@[42:0045) |     | | └─Token(StringComplete) |'a'|
//@[45:0046) |     | ├─Token(Colon) |:|
//@[47:0050) |     | └─StringSyntax
//@[47:0050) |     |   └─Token(StringComplete) |'b'|
//@[51:0052) |     ├─Token(Colon) |:|
//@[53:0069) |     └─TernaryOperationSyntax
//@[53:0058) |       ├─BooleanLiteralSyntax
//@[53:0058) |       | └─Token(FalseKeyword) |false|
//@[59:0060) |       ├─Token(Question) |?|
//@[61:0064) |       ├─StringSyntax
//@[61:0064) |       | └─Token(StringComplete) |'d'|
//@[65:0066) |       ├─Token(Colon) |:|
//@[67:0069) |       └─IntegerLiteralSyntax
//@[67:0069) |         └─Token(Integer) |15|
//@[69:0070) ├─Token(NewLine) |\n|
var nestedTernary = (null ? 1 : 2) ? (true ? 'a': 'b') : (false ? 'd' : 15)
//@[00:0075) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0017) | ├─IdentifierSyntax
//@[04:0017) | | └─Token(Identifier) |nestedTernary|
//@[18:0019) | ├─Token(Assignment) |=|
//@[20:0075) | └─TernaryOperationSyntax
//@[20:0034) |   ├─ParenthesizedExpressionSyntax
//@[20:0021) |   | ├─Token(LeftParen) |(|
//@[21:0033) |   | ├─TernaryOperationSyntax
//@[21:0025) |   | | ├─NullLiteralSyntax
//@[21:0025) |   | | | └─Token(NullKeyword) |null|
//@[26:0027) |   | | ├─Token(Question) |?|
//@[28:0029) |   | | ├─IntegerLiteralSyntax
//@[28:0029) |   | | | └─Token(Integer) |1|
//@[30:0031) |   | | ├─Token(Colon) |:|
//@[32:0033) |   | | └─IntegerLiteralSyntax
//@[32:0033) |   | |   └─Token(Integer) |2|
//@[33:0034) |   | └─Token(RightParen) |)|
//@[35:0036) |   ├─Token(Question) |?|
//@[37:0054) |   ├─ParenthesizedExpressionSyntax
//@[37:0038) |   | ├─Token(LeftParen) |(|
//@[38:0053) |   | ├─TernaryOperationSyntax
//@[38:0042) |   | | ├─BooleanLiteralSyntax
//@[38:0042) |   | | | └─Token(TrueKeyword) |true|
//@[43:0044) |   | | ├─Token(Question) |?|
//@[45:0048) |   | | ├─StringSyntax
//@[45:0048) |   | | | └─Token(StringComplete) |'a'|
//@[48:0049) |   | | ├─Token(Colon) |:|
//@[50:0053) |   | | └─StringSyntax
//@[50:0053) |   | |   └─Token(StringComplete) |'b'|
//@[53:0054) |   | └─Token(RightParen) |)|
//@[55:0056) |   ├─Token(Colon) |:|
//@[57:0075) |   └─ParenthesizedExpressionSyntax
//@[57:0058) |     ├─Token(LeftParen) |(|
//@[58:0074) |     ├─TernaryOperationSyntax
//@[58:0063) |     | ├─BooleanLiteralSyntax
//@[58:0063) |     | | └─Token(FalseKeyword) |false|
//@[64:0065) |     | ├─Token(Question) |?|
//@[66:0069) |     | ├─StringSyntax
//@[66:0069) |     | | └─Token(StringComplete) |'d'|
//@[70:0071) |     | ├─Token(Colon) |:|
//@[72:0074) |     | └─IntegerLiteralSyntax
//@[72:0074) |     |   └─Token(Integer) |15|
//@[74:0075) |     └─Token(RightParen) |)|
//@[75:0077) ├─Token(NewLine) |\n\n|

// bad array access
//@[19:0020) ├─Token(NewLine) |\n|
var errorInsideArrayAccess = [
//@[00:0044) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0026) | ├─IdentifierSyntax
//@[04:0026) | | └─Token(Identifier) |errorInsideArrayAccess|
//@[27:0028) | ├─Token(Assignment) |=|
//@[29:0044) | └─ArrayAccessSyntax
//@[29:0040) |   ├─ArraySyntax
//@[29:0030) |   | ├─Token(LeftSquare) |[|
//@[30:0031) |   | ├─Token(NewLine) |\n|
  !null
//@[02:0007) |   | ├─ArrayItemSyntax
//@[02:0007) |   | | └─UnaryOperationSyntax
//@[02:0003) |   | |   ├─Token(Exclamation) |!|
//@[03:0007) |   | |   └─NullLiteralSyntax
//@[03:0007) |   | |     └─Token(NullKeyword) |null|
//@[07:0008) |   | ├─Token(NewLine) |\n|
][!0]
//@[00:0001) |   | └─Token(RightSquare) |]|
//@[01:0002) |   ├─Token(LeftSquare) |[|
//@[02:0004) |   ├─UnaryOperationSyntax
//@[02:0003) |   | ├─Token(Exclamation) |!|
//@[03:0004) |   | └─IntegerLiteralSyntax
//@[03:0004) |   |   └─Token(Integer) |0|
//@[04:0005) |   └─Token(RightSquare) |]|
//@[05:0006) ├─Token(NewLine) |\n|
var integerIndexOnNonArray = (null)[0]
//@[00:0038) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0026) | ├─IdentifierSyntax
//@[04:0026) | | └─Token(Identifier) |integerIndexOnNonArray|
//@[27:0028) | ├─Token(Assignment) |=|
//@[29:0038) | └─ArrayAccessSyntax
//@[29:0035) |   ├─ParenthesizedExpressionSyntax
//@[29:0030) |   | ├─Token(LeftParen) |(|
//@[30:0034) |   | ├─NullLiteralSyntax
//@[30:0034) |   | | └─Token(NullKeyword) |null|
//@[34:0035) |   | └─Token(RightParen) |)|
//@[35:0036) |   ├─Token(LeftSquare) |[|
//@[36:0037) |   ├─IntegerLiteralSyntax
//@[36:0037) |   | └─Token(Integer) |0|
//@[37:0038) |   └─Token(RightSquare) |]|
//@[38:0039) ├─Token(NewLine) |\n|
var stringIndexOnNonObject = 'test'['test']
//@[00:0043) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0026) | ├─IdentifierSyntax
//@[04:0026) | | └─Token(Identifier) |stringIndexOnNonObject|
//@[27:0028) | ├─Token(Assignment) |=|
//@[29:0043) | └─ArrayAccessSyntax
//@[29:0035) |   ├─StringSyntax
//@[29:0035) |   | └─Token(StringComplete) |'test'|
//@[35:0036) |   ├─Token(LeftSquare) |[|
//@[36:0042) |   ├─StringSyntax
//@[36:0042) |   | └─Token(StringComplete) |'test'|
//@[42:0043) |   └─Token(RightSquare) |]|
//@[43:0044) ├─Token(NewLine) |\n|
var malformedStringIndex = {
//@[00:0040) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0024) | ├─IdentifierSyntax
//@[04:0024) | | └─Token(Identifier) |malformedStringIndex|
//@[25:0026) | ├─Token(Assignment) |=|
//@[27:0040) | └─ArrayAccessSyntax
//@[27:0030) |   ├─ObjectSyntax
//@[27:0028) |   | ├─Token(LeftBrace) |{|
//@[28:0029) |   | ├─Token(NewLine) |\n|
}['test\e']
//@[00:0001) |   | └─Token(RightBrace) |}|
//@[01:0002) |   ├─Token(LeftSquare) |[|
//@[02:0010) |   ├─SkippedTriviaSyntax
//@[02:0010) |   | └─Token(StringComplete) |'test\e'|
//@[10:0011) |   └─Token(RightSquare) |]|
//@[11:0012) ├─Token(NewLine) |\n|
var invalidIndexTypeOverAny = any(true)[true]
//@[00:0045) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0027) | ├─IdentifierSyntax
//@[04:0027) | | └─Token(Identifier) |invalidIndexTypeOverAny|
//@[28:0029) | ├─Token(Assignment) |=|
//@[30:0045) | └─ArrayAccessSyntax
//@[30:0039) |   ├─FunctionCallSyntax
//@[30:0033) |   | ├─IdentifierSyntax
//@[30:0033) |   | | └─Token(Identifier) |any|
//@[33:0034) |   | ├─Token(LeftParen) |(|
//@[34:0038) |   | ├─FunctionArgumentSyntax
//@[34:0038) |   | | └─BooleanLiteralSyntax
//@[34:0038) |   | |   └─Token(TrueKeyword) |true|
//@[38:0039) |   | └─Token(RightParen) |)|
//@[39:0040) |   ├─Token(LeftSquare) |[|
//@[40:0044) |   ├─BooleanLiteralSyntax
//@[40:0044) |   | └─Token(TrueKeyword) |true|
//@[44:0045) |   └─Token(RightSquare) |]|
//@[45:0046) ├─Token(NewLine) |\n|
var badIndexOverArray = [][null]
//@[00:0032) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0021) | ├─IdentifierSyntax
//@[04:0021) | | └─Token(Identifier) |badIndexOverArray|
//@[22:0023) | ├─Token(Assignment) |=|
//@[24:0032) | └─ArrayAccessSyntax
//@[24:0026) |   ├─ArraySyntax
//@[24:0025) |   | ├─Token(LeftSquare) |[|
//@[25:0026) |   | └─Token(RightSquare) |]|
//@[26:0027) |   ├─Token(LeftSquare) |[|
//@[27:0031) |   ├─NullLiteralSyntax
//@[27:0031) |   | └─Token(NullKeyword) |null|
//@[31:0032) |   └─Token(RightSquare) |]|
//@[32:0033) ├─Token(NewLine) |\n|
var badIndexOverArray2 = []['s']
//@[00:0032) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0022) | ├─IdentifierSyntax
//@[04:0022) | | └─Token(Identifier) |badIndexOverArray2|
//@[23:0024) | ├─Token(Assignment) |=|
//@[25:0032) | └─ArrayAccessSyntax
//@[25:0027) |   ├─ArraySyntax
//@[25:0026) |   | ├─Token(LeftSquare) |[|
//@[26:0027) |   | └─Token(RightSquare) |]|
//@[27:0028) |   ├─Token(LeftSquare) |[|
//@[28:0031) |   ├─StringSyntax
//@[28:0031) |   | └─Token(StringComplete) |'s'|
//@[31:0032) |   └─Token(RightSquare) |]|
//@[32:0033) ├─Token(NewLine) |\n|
var badIndexOverObj = {}[true]
//@[00:0030) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0019) | ├─IdentifierSyntax
//@[04:0019) | | └─Token(Identifier) |badIndexOverObj|
//@[20:0021) | ├─Token(Assignment) |=|
//@[22:0030) | └─ArrayAccessSyntax
//@[22:0024) |   ├─ObjectSyntax
//@[22:0023) |   | ├─Token(LeftBrace) |{|
//@[23:0024) |   | └─Token(RightBrace) |}|
//@[24:0025) |   ├─Token(LeftSquare) |[|
//@[25:0029) |   ├─BooleanLiteralSyntax
//@[25:0029) |   | └─Token(TrueKeyword) |true|
//@[29:0030) |   └─Token(RightSquare) |]|
//@[30:0031) ├─Token(NewLine) |\n|
var badIndexOverObj2 = {}[0]
//@[00:0028) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0020) | ├─IdentifierSyntax
//@[04:0020) | | └─Token(Identifier) |badIndexOverObj2|
//@[21:0022) | ├─Token(Assignment) |=|
//@[23:0028) | └─ArrayAccessSyntax
//@[23:0025) |   ├─ObjectSyntax
//@[23:0024) |   | ├─Token(LeftBrace) |{|
//@[24:0025) |   | └─Token(RightBrace) |}|
//@[25:0026) |   ├─Token(LeftSquare) |[|
//@[26:0027) |   ├─IntegerLiteralSyntax
//@[26:0027) |   | └─Token(Integer) |0|
//@[27:0028) |   └─Token(RightSquare) |]|
//@[28:0029) ├─Token(NewLine) |\n|
var badExpressionIndexer = {}[base64('a')]
//@[00:0042) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0024) | ├─IdentifierSyntax
//@[04:0024) | | └─Token(Identifier) |badExpressionIndexer|
//@[25:0026) | ├─Token(Assignment) |=|
//@[27:0042) | └─ArrayAccessSyntax
//@[27:0029) |   ├─ObjectSyntax
//@[27:0028) |   | ├─Token(LeftBrace) |{|
//@[28:0029) |   | └─Token(RightBrace) |}|
//@[29:0030) |   ├─Token(LeftSquare) |[|
//@[30:0041) |   ├─FunctionCallSyntax
//@[30:0036) |   | ├─IdentifierSyntax
//@[30:0036) |   | | └─Token(Identifier) |base64|
//@[36:0037) |   | ├─Token(LeftParen) |(|
//@[37:0040) |   | ├─FunctionArgumentSyntax
//@[37:0040) |   | | └─StringSyntax
//@[37:0040) |   | |   └─Token(StringComplete) |'a'|
//@[40:0041) |   | └─Token(RightParen) |)|
//@[41:0042) |   └─Token(RightSquare) |]|
//@[42:0044) ├─Token(NewLine) |\n\n|

// bad propertyAccess
//@[21:0022) ├─Token(NewLine) |\n|
var dotAccessOnNonObject = true.foo
//@[00:0035) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0024) | ├─IdentifierSyntax
//@[04:0024) | | └─Token(Identifier) |dotAccessOnNonObject|
//@[25:0026) | ├─Token(Assignment) |=|
//@[27:0035) | └─PropertyAccessSyntax
//@[27:0031) |   ├─BooleanLiteralSyntax
//@[27:0031) |   | └─Token(TrueKeyword) |true|
//@[31:0032) |   ├─Token(Dot) |.|
//@[32:0035) |   └─IdentifierSyntax
//@[32:0035) |     └─Token(Identifier) |foo|
//@[35:0036) ├─Token(NewLine) |\n|
var badExpressionInPropertyAccess = resourceGroup()[!'location']
//@[00:0064) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0033) | ├─IdentifierSyntax
//@[04:0033) | | └─Token(Identifier) |badExpressionInPropertyAccess|
//@[34:0035) | ├─Token(Assignment) |=|
//@[36:0064) | └─ArrayAccessSyntax
//@[36:0051) |   ├─FunctionCallSyntax
//@[36:0049) |   | ├─IdentifierSyntax
//@[36:0049) |   | | └─Token(Identifier) |resourceGroup|
//@[49:0050) |   | ├─Token(LeftParen) |(|
//@[50:0051) |   | └─Token(RightParen) |)|
//@[51:0052) |   ├─Token(LeftSquare) |[|
//@[52:0063) |   ├─UnaryOperationSyntax
//@[52:0053) |   | ├─Token(Exclamation) |!|
//@[53:0063) |   | └─StringSyntax
//@[53:0063) |   |   └─Token(StringComplete) |'location'|
//@[63:0064) |   └─Token(RightSquare) |]|
//@[64:0066) ├─Token(NewLine) |\n\n|

var propertyAccessOnVariable = x.foo
//@[00:0036) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0028) | ├─IdentifierSyntax
//@[04:0028) | | └─Token(Identifier) |propertyAccessOnVariable|
//@[29:0030) | ├─Token(Assignment) |=|
//@[31:0036) | └─PropertyAccessSyntax
//@[31:0032) |   ├─VariableAccessSyntax
//@[31:0032) |   | └─IdentifierSyntax
//@[31:0032) |   |   └─Token(Identifier) |x|
//@[32:0033) |   ├─Token(Dot) |.|
//@[33:0036) |   └─IdentifierSyntax
//@[33:0036) |     └─Token(Identifier) |foo|
//@[36:0038) ├─Token(NewLine) |\n\n|

// missing property in property access
//@[38:0039) ├─Token(NewLine) |\n|
var oneValidDeclaration = {}
//@[00:0028) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0023) | ├─IdentifierSyntax
//@[04:0023) | | └─Token(Identifier) |oneValidDeclaration|
//@[24:0025) | ├─Token(Assignment) |=|
//@[26:0028) | └─ObjectSyntax
//@[26:0027) |   ├─Token(LeftBrace) |{|
//@[27:0028) |   └─Token(RightBrace) |}|
//@[28:0029) ├─Token(NewLine) |\n|
var missingPropertyName = oneValidDeclaration.
//@[00:0046) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0023) | ├─IdentifierSyntax
//@[04:0023) | | └─Token(Identifier) |missingPropertyName|
//@[24:0025) | ├─Token(Assignment) |=|
//@[26:0046) | └─PropertyAccessSyntax
//@[26:0045) |   ├─VariableAccessSyntax
//@[26:0045) |   | └─IdentifierSyntax
//@[26:0045) |   |   └─Token(Identifier) |oneValidDeclaration|
//@[45:0046) |   ├─Token(Dot) |.|
//@[46:0046) |   └─IdentifierSyntax
//@[46:0046) |     └─SkippedTriviaSyntax
//@[46:0047) ├─Token(NewLine) |\n|
var missingPropertyInsideAnExpression = oneValidDeclaration. + oneValidDeclaration.
//@[00:0083) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0037) | ├─IdentifierSyntax
//@[04:0037) | | └─Token(Identifier) |missingPropertyInsideAnExpression|
//@[38:0039) | ├─Token(Assignment) |=|
//@[40:0083) | └─BinaryOperationSyntax
//@[40:0061) |   ├─PropertyAccessSyntax
//@[40:0059) |   | ├─VariableAccessSyntax
//@[40:0059) |   | | └─IdentifierSyntax
//@[40:0059) |   | |   └─Token(Identifier) |oneValidDeclaration|
//@[59:0060) |   | ├─Token(Dot) |.|
//@[61:0061) |   | └─IdentifierSyntax
//@[61:0061) |   |   └─SkippedTriviaSyntax
//@[61:0062) |   ├─Token(Plus) |+|
//@[63:0083) |   └─PropertyAccessSyntax
//@[63:0082) |     ├─VariableAccessSyntax
//@[63:0082) |     | └─IdentifierSyntax
//@[63:0082) |     |   └─Token(Identifier) |oneValidDeclaration|
//@[82:0083) |     ├─Token(Dot) |.|
//@[83:0083) |     └─IdentifierSyntax
//@[83:0083) |       └─SkippedTriviaSyntax
//@[83:0085) ├─Token(NewLine) |\n\n|

// function used like a variable
//@[32:0033) ├─Token(NewLine) |\n|
var funcvarvar = concat + base64 || !uniqueString
//@[00:0049) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0014) | ├─IdentifierSyntax
//@[04:0014) | | └─Token(Identifier) |funcvarvar|
//@[15:0016) | ├─Token(Assignment) |=|
//@[17:0049) | └─BinaryOperationSyntax
//@[17:0032) |   ├─BinaryOperationSyntax
//@[17:0023) |   | ├─VariableAccessSyntax
//@[17:0023) |   | | └─IdentifierSyntax
//@[17:0023) |   | |   └─Token(Identifier) |concat|
//@[24:0025) |   | ├─Token(Plus) |+|
//@[26:0032) |   | └─VariableAccessSyntax
//@[26:0032) |   |   └─IdentifierSyntax
//@[26:0032) |   |     └─Token(Identifier) |base64|
//@[33:0035) |   ├─Token(LogicalOr) ||||
//@[36:0049) |   └─UnaryOperationSyntax
//@[36:0037) |     ├─Token(Exclamation) |!|
//@[37:0049) |     └─VariableAccessSyntax
//@[37:0049) |       └─IdentifierSyntax
//@[37:0049) |         └─Token(Identifier) |uniqueString|
//@[49:0050) ├─Token(NewLine) |\n|
param funcvarparam bool = concat
//@[00:0032) ├─ParameterDeclarationSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0018) | ├─IdentifierSyntax
//@[06:0018) | | └─Token(Identifier) |funcvarparam|
//@[19:0023) | ├─SimpleTypeSyntax
//@[19:0023) | | └─Token(Identifier) |bool|
//@[24:0032) | └─ParameterDefaultValueSyntax
//@[24:0025) |   ├─Token(Assignment) |=|
//@[26:0032) |   └─VariableAccessSyntax
//@[26:0032) |     └─IdentifierSyntax
//@[26:0032) |       └─Token(Identifier) |concat|
//@[32:0033) ├─Token(NewLine) |\n|
output funcvarout array = padLeft
//@[00:0033) ├─OutputDeclarationSyntax
//@[00:0006) | ├─Token(Identifier) |output|
//@[07:0017) | ├─IdentifierSyntax
//@[07:0017) | | └─Token(Identifier) |funcvarout|
//@[18:0023) | ├─SimpleTypeSyntax
//@[18:0023) | | └─Token(Identifier) |array|
//@[24:0025) | ├─Token(Assignment) |=|
//@[26:0033) | └─VariableAccessSyntax
//@[26:0033) |   └─IdentifierSyntax
//@[26:0033) |     └─Token(Identifier) |padLeft|
//@[33:0035) ├─Token(NewLine) |\n\n|

// non-existent function
//@[24:0025) ├─Token(NewLine) |\n|
var fakeFunc = red() + green() * orange()
//@[00:0041) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0012) | ├─IdentifierSyntax
//@[04:0012) | | └─Token(Identifier) |fakeFunc|
//@[13:0014) | ├─Token(Assignment) |=|
//@[15:0041) | └─BinaryOperationSyntax
//@[15:0020) |   ├─FunctionCallSyntax
//@[15:0018) |   | ├─IdentifierSyntax
//@[15:0018) |   | | └─Token(Identifier) |red|
//@[18:0019) |   | ├─Token(LeftParen) |(|
//@[19:0020) |   | └─Token(RightParen) |)|
//@[21:0022) |   ├─Token(Plus) |+|
//@[23:0041) |   └─BinaryOperationSyntax
//@[23:0030) |     ├─FunctionCallSyntax
//@[23:0028) |     | ├─IdentifierSyntax
//@[23:0028) |     | | └─Token(Identifier) |green|
//@[28:0029) |     | ├─Token(LeftParen) |(|
//@[29:0030) |     | └─Token(RightParen) |)|
//@[31:0032) |     ├─Token(Asterisk) |*|
//@[33:0041) |     └─FunctionCallSyntax
//@[33:0039) |       ├─IdentifierSyntax
//@[33:0039) |       | └─Token(Identifier) |orange|
//@[39:0040) |       ├─Token(LeftParen) |(|
//@[40:0041) |       └─Token(RightParen) |)|
//@[41:0042) ├─Token(NewLine) |\n|
param fakeFuncP string = blue()
//@[00:0031) ├─ParameterDeclarationSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0015) | ├─IdentifierSyntax
//@[06:0015) | | └─Token(Identifier) |fakeFuncP|
//@[16:0022) | ├─SimpleTypeSyntax
//@[16:0022) | | └─Token(Identifier) |string|
//@[23:0031) | └─ParameterDefaultValueSyntax
//@[23:0024) |   ├─Token(Assignment) |=|
//@[25:0031) |   └─FunctionCallSyntax
//@[25:0029) |     ├─IdentifierSyntax
//@[25:0029) |     | └─Token(Identifier) |blue|
//@[29:0030) |     ├─Token(LeftParen) |(|
//@[30:0031) |     └─Token(RightParen) |)|
//@[31:0033) ├─Token(NewLine) |\n\n|

// non-existent variable
//@[24:0025) ├─Token(NewLine) |\n|
var fakeVar = concat(totallyFakeVar, 's')
//@[00:0041) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0011) | ├─IdentifierSyntax
//@[04:0011) | | └─Token(Identifier) |fakeVar|
//@[12:0013) | ├─Token(Assignment) |=|
//@[14:0041) | └─FunctionCallSyntax
//@[14:0020) |   ├─IdentifierSyntax
//@[14:0020) |   | └─Token(Identifier) |concat|
//@[20:0021) |   ├─Token(LeftParen) |(|
//@[21:0035) |   ├─FunctionArgumentSyntax
//@[21:0035) |   | └─VariableAccessSyntax
//@[21:0035) |   |   └─IdentifierSyntax
//@[21:0035) |   |     └─Token(Identifier) |totallyFakeVar|
//@[35:0036) |   ├─Token(Comma) |,|
//@[37:0040) |   ├─FunctionArgumentSyntax
//@[37:0040) |   | └─StringSyntax
//@[37:0040) |   |   └─Token(StringComplete) |'s'|
//@[40:0041) |   └─Token(RightParen) |)|
//@[41:0043) ├─Token(NewLine) |\n\n|

// bad functions arguments
//@[26:0027) ├─Token(NewLine) |\n|
var concatNotEnough = concat()
//@[00:0030) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0019) | ├─IdentifierSyntax
//@[04:0019) | | └─Token(Identifier) |concatNotEnough|
//@[20:0021) | ├─Token(Assignment) |=|
//@[22:0030) | └─FunctionCallSyntax
//@[22:0028) |   ├─IdentifierSyntax
//@[22:0028) |   | └─Token(Identifier) |concat|
//@[28:0029) |   ├─Token(LeftParen) |(|
//@[29:0030) |   └─Token(RightParen) |)|
//@[30:0031) ├─Token(NewLine) |\n|
var padLeftNotEnough = padLeft('s')
//@[00:0035) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0020) | ├─IdentifierSyntax
//@[04:0020) | | └─Token(Identifier) |padLeftNotEnough|
//@[21:0022) | ├─Token(Assignment) |=|
//@[23:0035) | └─FunctionCallSyntax
//@[23:0030) |   ├─IdentifierSyntax
//@[23:0030) |   | └─Token(Identifier) |padLeft|
//@[30:0031) |   ├─Token(LeftParen) |(|
//@[31:0034) |   ├─FunctionArgumentSyntax
//@[31:0034) |   | └─StringSyntax
//@[31:0034) |   |   └─Token(StringComplete) |'s'|
//@[34:0035) |   └─Token(RightParen) |)|
//@[35:0036) ├─Token(NewLine) |\n|
var takeTooMany = take([
//@[00:0035) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0015) | ├─IdentifierSyntax
//@[04:0015) | | └─Token(Identifier) |takeTooMany|
//@[16:0017) | ├─Token(Assignment) |=|
//@[18:0035) | └─FunctionCallSyntax
//@[18:0022) |   ├─IdentifierSyntax
//@[18:0022) |   | └─Token(Identifier) |take|
//@[22:0023) |   ├─Token(LeftParen) |(|
//@[23:0026) |   ├─FunctionArgumentSyntax
//@[23:0026) |   | └─ArraySyntax
//@[23:0024) |   |   ├─Token(LeftSquare) |[|
//@[24:0025) |   |   ├─Token(NewLine) |\n|
],1,2,'s')
//@[00:0001) |   |   └─Token(RightSquare) |]|
//@[01:0002) |   ├─Token(Comma) |,|
//@[02:0003) |   ├─FunctionArgumentSyntax
//@[02:0003) |   | └─IntegerLiteralSyntax
//@[02:0003) |   |   └─Token(Integer) |1|
//@[03:0004) |   ├─Token(Comma) |,|
//@[04:0005) |   ├─FunctionArgumentSyntax
//@[04:0005) |   | └─IntegerLiteralSyntax
//@[04:0005) |   |   └─Token(Integer) |2|
//@[05:0006) |   ├─Token(Comma) |,|
//@[06:0009) |   ├─FunctionArgumentSyntax
//@[06:0009) |   | └─StringSyntax
//@[06:0009) |   |   └─Token(StringComplete) |'s'|
//@[09:0010) |   └─Token(RightParen) |)|
//@[10:0012) ├─Token(NewLine) |\n\n|

// missing arguments
//@[20:0021) ├─Token(NewLine) |\n|
var trailingArgumentComma = format('s',)
//@[00:0040) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0025) | ├─IdentifierSyntax
//@[04:0025) | | └─Token(Identifier) |trailingArgumentComma|
//@[26:0027) | ├─Token(Assignment) |=|
//@[28:0040) | └─FunctionCallSyntax
//@[28:0034) |   ├─IdentifierSyntax
//@[28:0034) |   | └─Token(Identifier) |format|
//@[34:0035) |   ├─Token(LeftParen) |(|
//@[35:0038) |   ├─FunctionArgumentSyntax
//@[35:0038) |   | └─StringSyntax
//@[35:0038) |   |   └─Token(StringComplete) |'s'|
//@[38:0039) |   ├─Token(Comma) |,|
//@[39:0039) |   ├─FunctionArgumentSyntax
//@[39:0039) |   | └─SkippedTriviaSyntax
//@[39:0040) |   └─Token(RightParen) |)|
//@[40:0041) ├─Token(NewLine) |\n|
var onlyArgumentComma = concat(,)
//@[00:0033) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0021) | ├─IdentifierSyntax
//@[04:0021) | | └─Token(Identifier) |onlyArgumentComma|
//@[22:0023) | ├─Token(Assignment) |=|
//@[24:0033) | └─FunctionCallSyntax
//@[24:0030) |   ├─IdentifierSyntax
//@[24:0030) |   | └─Token(Identifier) |concat|
//@[30:0031) |   ├─Token(LeftParen) |(|
//@[31:0031) |   ├─FunctionArgumentSyntax
//@[31:0031) |   | └─SkippedTriviaSyntax
//@[31:0032) |   ├─Token(Comma) |,|
//@[32:0032) |   ├─FunctionArgumentSyntax
//@[32:0032) |   | └─SkippedTriviaSyntax
//@[32:0033) |   └─Token(RightParen) |)|
//@[33:0034) ├─Token(NewLine) |\n|
var multipleArgumentCommas = concat(,,,,,)
//@[00:0042) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0026) | ├─IdentifierSyntax
//@[04:0026) | | └─Token(Identifier) |multipleArgumentCommas|
//@[27:0028) | ├─Token(Assignment) |=|
//@[29:0042) | └─FunctionCallSyntax
//@[29:0035) |   ├─IdentifierSyntax
//@[29:0035) |   | └─Token(Identifier) |concat|
//@[35:0036) |   ├─Token(LeftParen) |(|
//@[36:0036) |   ├─FunctionArgumentSyntax
//@[36:0036) |   | └─SkippedTriviaSyntax
//@[36:0037) |   ├─Token(Comma) |,|
//@[37:0037) |   ├─FunctionArgumentSyntax
//@[37:0037) |   | └─SkippedTriviaSyntax
//@[37:0038) |   ├─Token(Comma) |,|
//@[38:0038) |   ├─FunctionArgumentSyntax
//@[38:0038) |   | └─SkippedTriviaSyntax
//@[38:0039) |   ├─Token(Comma) |,|
//@[39:0039) |   ├─FunctionArgumentSyntax
//@[39:0039) |   | └─SkippedTriviaSyntax
//@[39:0040) |   ├─Token(Comma) |,|
//@[40:0040) |   ├─FunctionArgumentSyntax
//@[40:0040) |   | └─SkippedTriviaSyntax
//@[40:0041) |   ├─Token(Comma) |,|
//@[41:0041) |   ├─FunctionArgumentSyntax
//@[41:0041) |   | └─SkippedTriviaSyntax
//@[41:0042) |   └─Token(RightParen) |)|
//@[42:0043) ├─Token(NewLine) |\n|
var emptyArgInBetween = concat(true,,false)
//@[00:0043) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0021) | ├─IdentifierSyntax
//@[04:0021) | | └─Token(Identifier) |emptyArgInBetween|
//@[22:0023) | ├─Token(Assignment) |=|
//@[24:0043) | └─FunctionCallSyntax
//@[24:0030) |   ├─IdentifierSyntax
//@[24:0030) |   | └─Token(Identifier) |concat|
//@[30:0031) |   ├─Token(LeftParen) |(|
//@[31:0035) |   ├─FunctionArgumentSyntax
//@[31:0035) |   | └─BooleanLiteralSyntax
//@[31:0035) |   |   └─Token(TrueKeyword) |true|
//@[35:0036) |   ├─Token(Comma) |,|
//@[36:0036) |   ├─FunctionArgumentSyntax
//@[36:0036) |   | └─SkippedTriviaSyntax
//@[36:0037) |   ├─Token(Comma) |,|
//@[37:0042) |   ├─FunctionArgumentSyntax
//@[37:0042) |   | └─BooleanLiteralSyntax
//@[37:0042) |   |   └─Token(FalseKeyword) |false|
//@[42:0043) |   └─Token(RightParen) |)|
//@[43:0044) ├─Token(NewLine) |\n|
var leadingEmptyArg = concat(,[])
//@[00:0033) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0019) | ├─IdentifierSyntax
//@[04:0019) | | └─Token(Identifier) |leadingEmptyArg|
//@[20:0021) | ├─Token(Assignment) |=|
//@[22:0033) | └─FunctionCallSyntax
//@[22:0028) |   ├─IdentifierSyntax
//@[22:0028) |   | └─Token(Identifier) |concat|
//@[28:0029) |   ├─Token(LeftParen) |(|
//@[29:0029) |   ├─FunctionArgumentSyntax
//@[29:0029) |   | └─SkippedTriviaSyntax
//@[29:0030) |   ├─Token(Comma) |,|
//@[30:0032) |   ├─FunctionArgumentSyntax
//@[30:0032) |   | └─ArraySyntax
//@[30:0031) |   |   ├─Token(LeftSquare) |[|
//@[31:0032) |   |   └─Token(RightSquare) |]|
//@[32:0033) |   └─Token(RightParen) |)|
//@[33:0034) ├─Token(NewLine) |\n|
var leadingAndTrailingEmptyArg = concat(,'s',)
//@[00:0046) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0030) | ├─IdentifierSyntax
//@[04:0030) | | └─Token(Identifier) |leadingAndTrailingEmptyArg|
//@[31:0032) | ├─Token(Assignment) |=|
//@[33:0046) | └─FunctionCallSyntax
//@[33:0039) |   ├─IdentifierSyntax
//@[33:0039) |   | └─Token(Identifier) |concat|
//@[39:0040) |   ├─Token(LeftParen) |(|
//@[40:0040) |   ├─FunctionArgumentSyntax
//@[40:0040) |   | └─SkippedTriviaSyntax
//@[40:0041) |   ├─Token(Comma) |,|
//@[41:0044) |   ├─FunctionArgumentSyntax
//@[41:0044) |   | └─StringSyntax
//@[41:0044) |   |   └─Token(StringComplete) |'s'|
//@[44:0045) |   ├─Token(Comma) |,|
//@[45:0045) |   ├─FunctionArgumentSyntax
//@[45:0045) |   | └─SkippedTriviaSyntax
//@[45:0046) |   └─Token(RightParen) |)|
//@[46:0048) ├─Token(NewLine) |\n\n|

// wrong argument types
//@[23:0024) ├─Token(NewLine) |\n|
var concatWrongTypes = concat({
//@[00:0034) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0020) | ├─IdentifierSyntax
//@[04:0020) | | └─Token(Identifier) |concatWrongTypes|
//@[21:0022) | ├─Token(Assignment) |=|
//@[23:0034) | └─FunctionCallSyntax
//@[23:0029) |   ├─IdentifierSyntax
//@[23:0029) |   | └─Token(Identifier) |concat|
//@[29:0030) |   ├─Token(LeftParen) |(|
//@[30:0033) |   ├─FunctionArgumentSyntax
//@[30:0033) |   | └─ObjectSyntax
//@[30:0031) |   |   ├─Token(LeftBrace) |{|
//@[31:0032) |   |   ├─Token(NewLine) |\n|
})
//@[00:0001) |   |   └─Token(RightBrace) |}|
//@[01:0002) |   └─Token(RightParen) |)|
//@[02:0003) ├─Token(NewLine) |\n|
var concatWrongTypesContradiction = concat('s', [
//@[00:0052) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0033) | ├─IdentifierSyntax
//@[04:0033) | | └─Token(Identifier) |concatWrongTypesContradiction|
//@[34:0035) | ├─Token(Assignment) |=|
//@[36:0052) | └─FunctionCallSyntax
//@[36:0042) |   ├─IdentifierSyntax
//@[36:0042) |   | └─Token(Identifier) |concat|
//@[42:0043) |   ├─Token(LeftParen) |(|
//@[43:0046) |   ├─FunctionArgumentSyntax
//@[43:0046) |   | └─StringSyntax
//@[43:0046) |   |   └─Token(StringComplete) |'s'|
//@[46:0047) |   ├─Token(Comma) |,|
//@[48:0051) |   ├─FunctionArgumentSyntax
//@[48:0051) |   | └─ArraySyntax
//@[48:0049) |   |   ├─Token(LeftSquare) |[|
//@[49:0050) |   |   ├─Token(NewLine) |\n|
])
//@[00:0001) |   |   └─Token(RightSquare) |]|
//@[01:0002) |   └─Token(RightParen) |)|
//@[02:0003) ├─Token(NewLine) |\n|
var indexOfWrongTypes = indexOf(1,1)
//@[00:0036) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0021) | ├─IdentifierSyntax
//@[04:0021) | | └─Token(Identifier) |indexOfWrongTypes|
//@[22:0023) | ├─Token(Assignment) |=|
//@[24:0036) | └─FunctionCallSyntax
//@[24:0031) |   ├─IdentifierSyntax
//@[24:0031) |   | └─Token(Identifier) |indexOf|
//@[31:0032) |   ├─Token(LeftParen) |(|
//@[32:0033) |   ├─FunctionArgumentSyntax
//@[32:0033) |   | └─IntegerLiteralSyntax
//@[32:0033) |   |   └─Token(Integer) |1|
//@[33:0034) |   ├─Token(Comma) |,|
//@[34:0035) |   ├─FunctionArgumentSyntax
//@[34:0035) |   | └─IntegerLiteralSyntax
//@[34:0035) |   |   └─Token(Integer) |1|
//@[35:0036) |   └─Token(RightParen) |)|
//@[36:0038) ├─Token(NewLine) |\n\n|

// not enough params
//@[20:0021) ├─Token(NewLine) |\n|
var test1 = listKeys('abcd')
//@[00:0028) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0009) | ├─IdentifierSyntax
//@[04:0009) | | └─Token(Identifier) |test1|
//@[10:0011) | ├─Token(Assignment) |=|
//@[12:0028) | └─FunctionCallSyntax
//@[12:0020) |   ├─IdentifierSyntax
//@[12:0020) |   | └─Token(Identifier) |listKeys|
//@[20:0021) |   ├─Token(LeftParen) |(|
//@[21:0027) |   ├─FunctionArgumentSyntax
//@[21:0027) |   | └─StringSyntax
//@[21:0027) |   |   └─Token(StringComplete) |'abcd'|
//@[27:0028) |   └─Token(RightParen) |)|
//@[28:0030) ├─Token(NewLine) |\n\n|

// list spelled wrong 
//@[22:0023) ├─Token(NewLine) |\n|
var test2 = lsitKeys('abcd', '2020-01-01')
//@[00:0042) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0009) | ├─IdentifierSyntax
//@[04:0009) | | └─Token(Identifier) |test2|
//@[10:0011) | ├─Token(Assignment) |=|
//@[12:0042) | └─FunctionCallSyntax
//@[12:0020) |   ├─IdentifierSyntax
//@[12:0020) |   | └─Token(Identifier) |lsitKeys|
//@[20:0021) |   ├─Token(LeftParen) |(|
//@[21:0027) |   ├─FunctionArgumentSyntax
//@[21:0027) |   | └─StringSyntax
//@[21:0027) |   |   └─Token(StringComplete) |'abcd'|
//@[27:0028) |   ├─Token(Comma) |,|
//@[29:0041) |   ├─FunctionArgumentSyntax
//@[29:0041) |   | └─StringSyntax
//@[29:0041) |   |   └─Token(StringComplete) |'2020-01-01'|
//@[41:0042) |   └─Token(RightParen) |)|
//@[42:0044) ├─Token(NewLine) |\n\n|

// just 'lis' instead of 'list'
//@[31:0032) ├─Token(NewLine) |\n|
var test3 = lis('abcd', '2020-01-01')
//@[00:0037) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0009) | ├─IdentifierSyntax
//@[04:0009) | | └─Token(Identifier) |test3|
//@[10:0011) | ├─Token(Assignment) |=|
//@[12:0037) | └─FunctionCallSyntax
//@[12:0015) |   ├─IdentifierSyntax
//@[12:0015) |   | └─Token(Identifier) |lis|
//@[15:0016) |   ├─Token(LeftParen) |(|
//@[16:0022) |   ├─FunctionArgumentSyntax
//@[16:0022) |   | └─StringSyntax
//@[16:0022) |   |   └─Token(StringComplete) |'abcd'|
//@[22:0023) |   ├─Token(Comma) |,|
//@[24:0036) |   ├─FunctionArgumentSyntax
//@[24:0036) |   | └─StringSyntax
//@[24:0036) |   |   └─Token(StringComplete) |'2020-01-01'|
//@[36:0037) |   └─Token(RightParen) |)|
//@[37:0039) ├─Token(NewLine) |\n\n|

var sampleObject = {
//@[00:0190) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0016) | ├─IdentifierSyntax
//@[04:0016) | | └─Token(Identifier) |sampleObject|
//@[17:0018) | ├─Token(Assignment) |=|
//@[19:0190) | └─ObjectSyntax
//@[19:0020) |   ├─Token(LeftBrace) |{|
//@[20:0021) |   ├─Token(NewLine) |\n|
  myInt: 42
//@[02:0011) |   ├─ObjectPropertySyntax
//@[02:0007) |   | ├─IdentifierSyntax
//@[02:0007) |   | | └─Token(Identifier) |myInt|
//@[07:0008) |   | ├─Token(Colon) |:|
//@[09:0011) |   | └─IntegerLiteralSyntax
//@[09:0011) |   |   └─Token(Integer) |42|
//@[11:0012) |   ├─Token(NewLine) |\n|
  myStr: 's'
//@[02:0012) |   ├─ObjectPropertySyntax
//@[02:0007) |   | ├─IdentifierSyntax
//@[02:0007) |   | | └─Token(Identifier) |myStr|
//@[07:0008) |   | ├─Token(Colon) |:|
//@[09:0012) |   | └─StringSyntax
//@[09:0012) |   |   └─Token(StringComplete) |'s'|
//@[12:0013) |   ├─Token(NewLine) |\n|
  myBool: false
//@[02:0015) |   ├─ObjectPropertySyntax
//@[02:0008) |   | ├─IdentifierSyntax
//@[02:0008) |   | | └─Token(Identifier) |myBool|
//@[08:0009) |   | ├─Token(Colon) |:|
//@[10:0015) |   | └─BooleanLiteralSyntax
//@[10:0015) |   |   └─Token(FalseKeyword) |false|
//@[15:0016) |   ├─Token(NewLine) |\n|
  myNull: null
//@[02:0014) |   ├─ObjectPropertySyntax
//@[02:0008) |   | ├─IdentifierSyntax
//@[02:0008) |   | | └─Token(Identifier) |myNull|
//@[08:0009) |   | ├─Token(Colon) |:|
//@[10:0014) |   | └─NullLiteralSyntax
//@[10:0014) |   |   └─Token(NullKeyword) |null|
//@[14:0015) |   ├─Token(NewLine) |\n|
  myInner: {
//@[02:0078) |   ├─ObjectPropertySyntax
//@[02:0009) |   | ├─IdentifierSyntax
//@[02:0009) |   | | └─Token(Identifier) |myInner|
//@[09:0010) |   | ├─Token(Colon) |:|
//@[11:0078) |   | └─ObjectSyntax
//@[11:0012) |   |   ├─Token(LeftBrace) |{|
//@[12:0013) |   |   ├─Token(NewLine) |\n|
    anotherStr: 'a'
//@[04:0019) |   |   ├─ObjectPropertySyntax
//@[04:0014) |   |   | ├─IdentifierSyntax
//@[04:0014) |   |   | | └─Token(Identifier) |anotherStr|
//@[14:0015) |   |   | ├─Token(Colon) |:|
//@[16:0019) |   |   | └─StringSyntax
//@[16:0019) |   |   |   └─Token(StringComplete) |'a'|
//@[19:0020) |   |   ├─Token(NewLine) |\n|
    otherArr: [
//@[04:0041) |   |   ├─ObjectPropertySyntax
//@[04:0012) |   |   | ├─IdentifierSyntax
//@[04:0012) |   |   | | └─Token(Identifier) |otherArr|
//@[12:0013) |   |   | ├─Token(Colon) |:|
//@[14:0041) |   |   | └─ArraySyntax
//@[14:0015) |   |   |   ├─Token(LeftSquare) |[|
//@[15:0016) |   |   |   ├─Token(NewLine) |\n|
      's'
//@[06:0009) |   |   |   ├─ArrayItemSyntax
//@[06:0009) |   |   |   | └─StringSyntax
//@[06:0009) |   |   |   |   └─Token(StringComplete) |'s'|
//@[09:0010) |   |   |   ├─Token(NewLine) |\n|
      'a'
//@[06:0009) |   |   |   ├─ArrayItemSyntax
//@[06:0009) |   |   |   | └─StringSyntax
//@[06:0009) |   |   |   |   └─Token(StringComplete) |'a'|
//@[09:0010) |   |   |   ├─Token(NewLine) |\n|
    ]
//@[04:0005) |   |   |   └─Token(RightSquare) |]|
//@[05:0006) |   |   ├─Token(NewLine) |\n|
  }
//@[02:0003) |   |   └─Token(RightBrace) |}|
//@[03:0004) |   ├─Token(NewLine) |\n|
  myArr: [
//@[02:0032) |   ├─ObjectPropertySyntax
//@[02:0007) |   | ├─IdentifierSyntax
//@[02:0007) |   | | └─Token(Identifier) |myArr|
//@[07:0008) |   | ├─Token(Colon) |:|
//@[09:0032) |   | └─ArraySyntax
//@[09:0010) |   |   ├─Token(LeftSquare) |[|
//@[10:0011) |   |   ├─Token(NewLine) |\n|
    1
//@[04:0005) |   |   ├─ArrayItemSyntax
//@[04:0005) |   |   | └─IntegerLiteralSyntax
//@[04:0005) |   |   |   └─Token(Integer) |1|
//@[05:0006) |   |   ├─Token(NewLine) |\n|
    2
//@[04:0005) |   |   ├─ArrayItemSyntax
//@[04:0005) |   |   | └─IntegerLiteralSyntax
//@[04:0005) |   |   |   └─Token(Integer) |2|
//@[05:0006) |   |   ├─Token(NewLine) |\n|
    3
//@[04:0005) |   |   ├─ArrayItemSyntax
//@[04:0005) |   |   | └─IntegerLiteralSyntax
//@[04:0005) |   |   |   └─Token(Integer) |3|
//@[05:0006) |   |   ├─Token(NewLine) |\n|
  ]
//@[02:0003) |   |   └─Token(RightSquare) |]|
//@[03:0004) |   ├─Token(NewLine) |\n|
}
//@[00:0001) |   └─Token(RightBrace) |}|
//@[01:0003) ├─Token(NewLine) |\n\n|

var badProperty = sampleObject.myFake
//@[00:0037) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0015) | ├─IdentifierSyntax
//@[04:0015) | | └─Token(Identifier) |badProperty|
//@[16:0017) | ├─Token(Assignment) |=|
//@[18:0037) | └─PropertyAccessSyntax
//@[18:0030) |   ├─VariableAccessSyntax
//@[18:0030) |   | └─IdentifierSyntax
//@[18:0030) |   |   └─Token(Identifier) |sampleObject|
//@[30:0031) |   ├─Token(Dot) |.|
//@[31:0037) |   └─IdentifierSyntax
//@[31:0037) |     └─Token(Identifier) |myFake|
//@[37:0038) ├─Token(NewLine) |\n|
var badSpelling = sampleObject.myNul
//@[00:0036) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0015) | ├─IdentifierSyntax
//@[04:0015) | | └─Token(Identifier) |badSpelling|
//@[16:0017) | ├─Token(Assignment) |=|
//@[18:0036) | └─PropertyAccessSyntax
//@[18:0030) |   ├─VariableAccessSyntax
//@[18:0030) |   | └─IdentifierSyntax
//@[18:0030) |   |   └─Token(Identifier) |sampleObject|
//@[30:0031) |   ├─Token(Dot) |.|
//@[31:0036) |   └─IdentifierSyntax
//@[31:0036) |     └─Token(Identifier) |myNul|
//@[36:0037) ├─Token(NewLine) |\n|
var badPropertyIndexer = sampleObject['fake']
//@[00:0045) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0022) | ├─IdentifierSyntax
//@[04:0022) | | └─Token(Identifier) |badPropertyIndexer|
//@[23:0024) | ├─Token(Assignment) |=|
//@[25:0045) | └─ArrayAccessSyntax
//@[25:0037) |   ├─VariableAccessSyntax
//@[25:0037) |   | └─IdentifierSyntax
//@[25:0037) |   |   └─Token(Identifier) |sampleObject|
//@[37:0038) |   ├─Token(LeftSquare) |[|
//@[38:0044) |   ├─StringSyntax
//@[38:0044) |   | └─Token(StringComplete) |'fake'|
//@[44:0045) |   └─Token(RightSquare) |]|
//@[45:0046) ├─Token(NewLine) |\n|
var badType = sampleObject.myStr / 32
//@[00:0037) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0011) | ├─IdentifierSyntax
//@[04:0011) | | └─Token(Identifier) |badType|
//@[12:0013) | ├─Token(Assignment) |=|
//@[14:0037) | └─BinaryOperationSyntax
//@[14:0032) |   ├─PropertyAccessSyntax
//@[14:0026) |   | ├─VariableAccessSyntax
//@[14:0026) |   | | └─IdentifierSyntax
//@[14:0026) |   | |   └─Token(Identifier) |sampleObject|
//@[26:0027) |   | ├─Token(Dot) |.|
//@[27:0032) |   | └─IdentifierSyntax
//@[27:0032) |   |   └─Token(Identifier) |myStr|
//@[33:0034) |   ├─Token(Slash) |/|
//@[35:0037) |   └─IntegerLiteralSyntax
//@[35:0037) |     └─Token(Integer) |32|
//@[37:0038) ├─Token(NewLine) |\n|
var badInnerProperty = sampleObject.myInner.fake
//@[00:0048) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0020) | ├─IdentifierSyntax
//@[04:0020) | | └─Token(Identifier) |badInnerProperty|
//@[21:0022) | ├─Token(Assignment) |=|
//@[23:0048) | └─PropertyAccessSyntax
//@[23:0043) |   ├─PropertyAccessSyntax
//@[23:0035) |   | ├─VariableAccessSyntax
//@[23:0035) |   | | └─IdentifierSyntax
//@[23:0035) |   | |   └─Token(Identifier) |sampleObject|
//@[35:0036) |   | ├─Token(Dot) |.|
//@[36:0043) |   | └─IdentifierSyntax
//@[36:0043) |   |   └─Token(Identifier) |myInner|
//@[43:0044) |   ├─Token(Dot) |.|
//@[44:0048) |   └─IdentifierSyntax
//@[44:0048) |     └─Token(Identifier) |fake|
//@[48:0049) ├─Token(NewLine) |\n|
var badInnerType = sampleObject.myInner.anotherStr + 2
//@[00:0054) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0016) | ├─IdentifierSyntax
//@[04:0016) | | └─Token(Identifier) |badInnerType|
//@[17:0018) | ├─Token(Assignment) |=|
//@[19:0054) | └─BinaryOperationSyntax
//@[19:0050) |   ├─PropertyAccessSyntax
//@[19:0039) |   | ├─PropertyAccessSyntax
//@[19:0031) |   | | ├─VariableAccessSyntax
//@[19:0031) |   | | | └─IdentifierSyntax
//@[19:0031) |   | | |   └─Token(Identifier) |sampleObject|
//@[31:0032) |   | | ├─Token(Dot) |.|
//@[32:0039) |   | | └─IdentifierSyntax
//@[32:0039) |   | |   └─Token(Identifier) |myInner|
//@[39:0040) |   | ├─Token(Dot) |.|
//@[40:0050) |   | └─IdentifierSyntax
//@[40:0050) |   |   └─Token(Identifier) |anotherStr|
//@[51:0052) |   ├─Token(Plus) |+|
//@[53:0054) |   └─IntegerLiteralSyntax
//@[53:0054) |     └─Token(Integer) |2|
//@[54:0055) ├─Token(NewLine) |\n|
var badArrayIndexer = sampleObject.myArr['s']
//@[00:0045) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0019) | ├─IdentifierSyntax
//@[04:0019) | | └─Token(Identifier) |badArrayIndexer|
//@[20:0021) | ├─Token(Assignment) |=|
//@[22:0045) | └─ArrayAccessSyntax
//@[22:0040) |   ├─PropertyAccessSyntax
//@[22:0034) |   | ├─VariableAccessSyntax
//@[22:0034) |   | | └─IdentifierSyntax
//@[22:0034) |   | |   └─Token(Identifier) |sampleObject|
//@[34:0035) |   | ├─Token(Dot) |.|
//@[35:0040) |   | └─IdentifierSyntax
//@[35:0040) |   |   └─Token(Identifier) |myArr|
//@[40:0041) |   ├─Token(LeftSquare) |[|
//@[41:0044) |   ├─StringSyntax
//@[41:0044) |   | └─Token(StringComplete) |'s'|
//@[44:0045) |   └─Token(RightSquare) |]|
//@[45:0046) ├─Token(NewLine) |\n|
var badInnerArrayIndexer = sampleObject.myInner.otherArr['s']
//@[00:0061) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0024) | ├─IdentifierSyntax
//@[04:0024) | | └─Token(Identifier) |badInnerArrayIndexer|
//@[25:0026) | ├─Token(Assignment) |=|
//@[27:0061) | └─ArrayAccessSyntax
//@[27:0056) |   ├─PropertyAccessSyntax
//@[27:0047) |   | ├─PropertyAccessSyntax
//@[27:0039) |   | | ├─VariableAccessSyntax
//@[27:0039) |   | | | └─IdentifierSyntax
//@[27:0039) |   | | |   └─Token(Identifier) |sampleObject|
//@[39:0040) |   | | ├─Token(Dot) |.|
//@[40:0047) |   | | └─IdentifierSyntax
//@[40:0047) |   | |   └─Token(Identifier) |myInner|
//@[47:0048) |   | ├─Token(Dot) |.|
//@[48:0056) |   | └─IdentifierSyntax
//@[48:0056) |   |   └─Token(Identifier) |otherArr|
//@[56:0057) |   ├─Token(LeftSquare) |[|
//@[57:0060) |   ├─StringSyntax
//@[57:0060) |   | └─Token(StringComplete) |'s'|
//@[60:0061) |   └─Token(RightSquare) |]|
//@[61:0062) ├─Token(NewLine) |\n|
var badIndexer = sampleObject.myStr['s']
//@[00:0040) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0014) | ├─IdentifierSyntax
//@[04:0014) | | └─Token(Identifier) |badIndexer|
//@[15:0016) | ├─Token(Assignment) |=|
//@[17:0040) | └─ArrayAccessSyntax
//@[17:0035) |   ├─PropertyAccessSyntax
//@[17:0029) |   | ├─VariableAccessSyntax
//@[17:0029) |   | | └─IdentifierSyntax
//@[17:0029) |   | |   └─Token(Identifier) |sampleObject|
//@[29:0030) |   | ├─Token(Dot) |.|
//@[30:0035) |   | └─IdentifierSyntax
//@[30:0035) |   |   └─Token(Identifier) |myStr|
//@[35:0036) |   ├─Token(LeftSquare) |[|
//@[36:0039) |   ├─StringSyntax
//@[36:0039) |   | └─Token(StringComplete) |'s'|
//@[39:0040) |   └─Token(RightSquare) |]|
//@[40:0041) ├─Token(NewLine) |\n|
var badInnerArray = sampleObject.myInner.fakeArr['s']
//@[00:0053) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0017) | ├─IdentifierSyntax
//@[04:0017) | | └─Token(Identifier) |badInnerArray|
//@[18:0019) | ├─Token(Assignment) |=|
//@[20:0053) | └─ArrayAccessSyntax
//@[20:0048) |   ├─PropertyAccessSyntax
//@[20:0040) |   | ├─PropertyAccessSyntax
//@[20:0032) |   | | ├─VariableAccessSyntax
//@[20:0032) |   | | | └─IdentifierSyntax
//@[20:0032) |   | | |   └─Token(Identifier) |sampleObject|
//@[32:0033) |   | | ├─Token(Dot) |.|
//@[33:0040) |   | | └─IdentifierSyntax
//@[33:0040) |   | |   └─Token(Identifier) |myInner|
//@[40:0041) |   | ├─Token(Dot) |.|
//@[41:0048) |   | └─IdentifierSyntax
//@[41:0048) |   |   └─Token(Identifier) |fakeArr|
//@[48:0049) |   ├─Token(LeftSquare) |[|
//@[49:0052) |   ├─StringSyntax
//@[49:0052) |   | └─Token(StringComplete) |'s'|
//@[52:0053) |   └─Token(RightSquare) |]|
//@[53:0054) ├─Token(NewLine) |\n|
var invalidPropertyCallOnInstanceFunctionAccess = a.b.c.bar().baz
//@[00:0065) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0047) | ├─IdentifierSyntax
//@[04:0047) | | └─Token(Identifier) |invalidPropertyCallOnInstanceFunctionAccess|
//@[48:0049) | ├─Token(Assignment) |=|
//@[50:0065) | └─PropertyAccessSyntax
//@[50:0061) |   ├─InstanceFunctionCallSyntax
//@[50:0055) |   | ├─PropertyAccessSyntax
//@[50:0053) |   | | ├─PropertyAccessSyntax
//@[50:0051) |   | | | ├─VariableAccessSyntax
//@[50:0051) |   | | | | └─IdentifierSyntax
//@[50:0051) |   | | | |   └─Token(Identifier) |a|
//@[51:0052) |   | | | ├─Token(Dot) |.|
//@[52:0053) |   | | | └─IdentifierSyntax
//@[52:0053) |   | | |   └─Token(Identifier) |b|
//@[53:0054) |   | | ├─Token(Dot) |.|
//@[54:0055) |   | | └─IdentifierSyntax
//@[54:0055) |   | |   └─Token(Identifier) |c|
//@[55:0056) |   | ├─Token(Dot) |.|
//@[56:0059) |   | ├─IdentifierSyntax
//@[56:0059) |   | | └─Token(Identifier) |bar|
//@[59:0060) |   | ├─Token(LeftParen) |(|
//@[60:0061) |   | └─Token(RightParen) |)|
//@[61:0062) |   ├─Token(Dot) |.|
//@[62:0065) |   └─IdentifierSyntax
//@[62:0065) |     └─Token(Identifier) |baz|
//@[65:0066) ├─Token(NewLine) |\n|
var invalidInstanceFunctionAccess = a.b.c.bar()
//@[00:0047) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0033) | ├─IdentifierSyntax
//@[04:0033) | | └─Token(Identifier) |invalidInstanceFunctionAccess|
//@[34:0035) | ├─Token(Assignment) |=|
//@[36:0047) | └─InstanceFunctionCallSyntax
//@[36:0041) |   ├─PropertyAccessSyntax
//@[36:0039) |   | ├─PropertyAccessSyntax
//@[36:0037) |   | | ├─VariableAccessSyntax
//@[36:0037) |   | | | └─IdentifierSyntax
//@[36:0037) |   | | |   └─Token(Identifier) |a|
//@[37:0038) |   | | ├─Token(Dot) |.|
//@[38:0039) |   | | └─IdentifierSyntax
//@[38:0039) |   | |   └─Token(Identifier) |b|
//@[39:0040) |   | ├─Token(Dot) |.|
//@[40:0041) |   | └─IdentifierSyntax
//@[40:0041) |   |   └─Token(Identifier) |c|
//@[41:0042) |   ├─Token(Dot) |.|
//@[42:0045) |   ├─IdentifierSyntax
//@[42:0045) |   | └─Token(Identifier) |bar|
//@[45:0046) |   ├─Token(LeftParen) |(|
//@[46:0047) |   └─Token(RightParen) |)|
//@[47:0048) ├─Token(NewLine) |\n|
var invalidInstanceFunctionCall = az.az()
//@[00:0041) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0031) | ├─IdentifierSyntax
//@[04:0031) | | └─Token(Identifier) |invalidInstanceFunctionCall|
//@[32:0033) | ├─Token(Assignment) |=|
//@[34:0041) | └─InstanceFunctionCallSyntax
//@[34:0036) |   ├─VariableAccessSyntax
//@[34:0036) |   | └─IdentifierSyntax
//@[34:0036) |   |   └─Token(Identifier) |az|
//@[36:0037) |   ├─Token(Dot) |.|
//@[37:0039) |   ├─IdentifierSyntax
//@[37:0039) |   | └─Token(Identifier) |az|
//@[39:0040) |   ├─Token(LeftParen) |(|
//@[40:0041) |   └─Token(RightParen) |)|
//@[41:0042) ├─Token(NewLine) |\n|
var invalidPropertyAccessOnAzNamespace = az.az
//@[00:0046) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0038) | ├─IdentifierSyntax
//@[04:0038) | | └─Token(Identifier) |invalidPropertyAccessOnAzNamespace|
//@[39:0040) | ├─Token(Assignment) |=|
//@[41:0046) | └─PropertyAccessSyntax
//@[41:0043) |   ├─VariableAccessSyntax
//@[41:0043) |   | └─IdentifierSyntax
//@[41:0043) |   |   └─Token(Identifier) |az|
//@[43:0044) |   ├─Token(Dot) |.|
//@[44:0046) |   └─IdentifierSyntax
//@[44:0046) |     └─Token(Identifier) |az|
//@[46:0047) ├─Token(NewLine) |\n|
var invalidPropertyAccessOnSysNamespace = sys.az
//@[00:0048) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0039) | ├─IdentifierSyntax
//@[04:0039) | | └─Token(Identifier) |invalidPropertyAccessOnSysNamespace|
//@[40:0041) | ├─Token(Assignment) |=|
//@[42:0048) | └─PropertyAccessSyntax
//@[42:0045) |   ├─VariableAccessSyntax
//@[42:0045) |   | └─IdentifierSyntax
//@[42:0045) |   |   └─Token(Identifier) |sys|
//@[45:0046) |   ├─Token(Dot) |.|
//@[46:0048) |   └─IdentifierSyntax
//@[46:0048) |     └─Token(Identifier) |az|
//@[48:0049) ├─Token(NewLine) |\n|
var invalidOperands = 1 + az
//@[00:0028) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0019) | ├─IdentifierSyntax
//@[04:0019) | | └─Token(Identifier) |invalidOperands|
//@[20:0021) | ├─Token(Assignment) |=|
//@[22:0028) | └─BinaryOperationSyntax
//@[22:0023) |   ├─IntegerLiteralSyntax
//@[22:0023) |   | └─Token(Integer) |1|
//@[24:0025) |   ├─Token(Plus) |+|
//@[26:0028) |   └─VariableAccessSyntax
//@[26:0028) |     └─IdentifierSyntax
//@[26:0028) |       └─Token(Identifier) |az|
//@[28:0029) ├─Token(NewLine) |\n|
var invalidStringAddition = 'hello' + sampleObject.myStr
//@[00:0056) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0025) | ├─IdentifierSyntax
//@[04:0025) | | └─Token(Identifier) |invalidStringAddition|
//@[26:0027) | ├─Token(Assignment) |=|
//@[28:0056) | └─BinaryOperationSyntax
//@[28:0035) |   ├─StringSyntax
//@[28:0035) |   | └─Token(StringComplete) |'hello'|
//@[36:0037) |   ├─Token(Plus) |+|
//@[38:0056) |   └─PropertyAccessSyntax
//@[38:0050) |     ├─VariableAccessSyntax
//@[38:0050) |     | └─IdentifierSyntax
//@[38:0050) |     |   └─Token(Identifier) |sampleObject|
//@[50:0051) |     ├─Token(Dot) |.|
//@[51:0056) |     └─IdentifierSyntax
//@[51:0056) |       └─Token(Identifier) |myStr|
//@[56:0058) ├─Token(NewLine) |\n\n|

var bannedFunctions = {
//@[00:0393) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0019) | ├─IdentifierSyntax
//@[04:0019) | | └─Token(Identifier) |bannedFunctions|
//@[20:0021) | ├─Token(Assignment) |=|
//@[22:0393) | └─ObjectSyntax
//@[22:0023) |   ├─Token(LeftBrace) |{|
//@[23:0024) |   ├─Token(NewLine) |\n|
  var: variables()
//@[02:0018) |   ├─ObjectPropertySyntax
//@[02:0005) |   | ├─IdentifierSyntax
//@[02:0005) |   | | └─Token(Identifier) |var|
//@[05:0006) |   | ├─Token(Colon) |:|
//@[07:0018) |   | └─FunctionCallSyntax
//@[07:0016) |   |   ├─IdentifierSyntax
//@[07:0016) |   |   | └─Token(Identifier) |variables|
//@[16:0017) |   |   ├─Token(LeftParen) |(|
//@[17:0018) |   |   └─Token(RightParen) |)|
//@[18:0019) |   ├─Token(NewLine) |\n|
  param: parameters() + 2
//@[02:0025) |   ├─ObjectPropertySyntax
//@[02:0007) |   | ├─IdentifierSyntax
//@[02:0007) |   | | └─Token(Identifier) |param|
//@[07:0008) |   | ├─Token(Colon) |:|
//@[09:0025) |   | └─BinaryOperationSyntax
//@[09:0021) |   |   ├─FunctionCallSyntax
//@[09:0019) |   |   | ├─IdentifierSyntax
//@[09:0019) |   |   | | └─Token(Identifier) |parameters|
//@[19:0020) |   |   | ├─Token(LeftParen) |(|
//@[20:0021) |   |   | └─Token(RightParen) |)|
//@[22:0023) |   |   ├─Token(Plus) |+|
//@[24:0025) |   |   └─IntegerLiteralSyntax
//@[24:0025) |   |     └─Token(Integer) |2|
//@[25:0026) |   ├─Token(NewLine) |\n|
  if: sys.if(null,null)
//@[02:0023) |   ├─ObjectPropertySyntax
//@[02:0004) |   | ├─IdentifierSyntax
//@[02:0004) |   | | └─Token(Identifier) |if|
//@[04:0005) |   | ├─Token(Colon) |:|
//@[06:0023) |   | └─InstanceFunctionCallSyntax
//@[06:0009) |   |   ├─VariableAccessSyntax
//@[06:0009) |   |   | └─IdentifierSyntax
//@[06:0009) |   |   |   └─Token(Identifier) |sys|
//@[09:0010) |   |   ├─Token(Dot) |.|
//@[10:0012) |   |   ├─IdentifierSyntax
//@[10:0012) |   |   | └─Token(Identifier) |if|
//@[12:0013) |   |   ├─Token(LeftParen) |(|
//@[13:0017) |   |   ├─FunctionArgumentSyntax
//@[13:0017) |   |   | └─NullLiteralSyntax
//@[13:0017) |   |   |   └─Token(NullKeyword) |null|
//@[17:0018) |   |   ├─Token(Comma) |,|
//@[18:0022) |   |   ├─FunctionArgumentSyntax
//@[18:0022) |   |   | └─NullLiteralSyntax
//@[18:0022) |   |   |   └─Token(NullKeyword) |null|
//@[22:0023) |   |   └─Token(RightParen) |)|
//@[23:0024) |   ├─Token(NewLine) |\n|
  obj: sys.createArray()
//@[02:0024) |   ├─ObjectPropertySyntax
//@[02:0005) |   | ├─IdentifierSyntax
//@[02:0005) |   | | └─Token(Identifier) |obj|
//@[05:0006) |   | ├─Token(Colon) |:|
//@[07:0024) |   | └─InstanceFunctionCallSyntax
//@[07:0010) |   |   ├─VariableAccessSyntax
//@[07:0010) |   |   | └─IdentifierSyntax
//@[07:0010) |   |   |   └─Token(Identifier) |sys|
//@[10:0011) |   |   ├─Token(Dot) |.|
//@[11:0022) |   |   ├─IdentifierSyntax
//@[11:0022) |   |   | └─Token(Identifier) |createArray|
//@[22:0023) |   |   ├─Token(LeftParen) |(|
//@[23:0024) |   |   └─Token(RightParen) |)|
//@[24:0025) |   ├─Token(NewLine) |\n|
  arr: sys.createObject()
//@[02:0025) |   ├─ObjectPropertySyntax
//@[02:0005) |   | ├─IdentifierSyntax
//@[02:0005) |   | | └─Token(Identifier) |arr|
//@[05:0006) |   | ├─Token(Colon) |:|
//@[07:0025) |   | └─InstanceFunctionCallSyntax
//@[07:0010) |   |   ├─VariableAccessSyntax
//@[07:0010) |   |   | └─IdentifierSyntax
//@[07:0010) |   |   |   └─Token(Identifier) |sys|
//@[10:0011) |   |   ├─Token(Dot) |.|
//@[11:0023) |   |   ├─IdentifierSyntax
//@[11:0023) |   |   | └─Token(Identifier) |createObject|
//@[23:0024) |   |   ├─Token(LeftParen) |(|
//@[24:0025) |   |   └─Token(RightParen) |)|
//@[25:0026) |   ├─Token(NewLine) |\n|
  numeric: sys.add(1) + sys.sub(2,3) + sys.mul(8,'s') + sys.div(true) + sys.mod(null, false)
//@[02:0092) |   ├─ObjectPropertySyntax
//@[02:0009) |   | ├─IdentifierSyntax
//@[02:0009) |   | | └─Token(Identifier) |numeric|
//@[09:0010) |   | ├─Token(Colon) |:|
//@[11:0092) |   | └─BinaryOperationSyntax
//@[11:0069) |   |   ├─BinaryOperationSyntax
//@[11:0053) |   |   | ├─BinaryOperationSyntax
//@[11:0036) |   |   | | ├─BinaryOperationSyntax
//@[11:0021) |   |   | | | ├─InstanceFunctionCallSyntax
//@[11:0014) |   |   | | | | ├─VariableAccessSyntax
//@[11:0014) |   |   | | | | | └─IdentifierSyntax
//@[11:0014) |   |   | | | | |   └─Token(Identifier) |sys|
//@[14:0015) |   |   | | | | ├─Token(Dot) |.|
//@[15:0018) |   |   | | | | ├─IdentifierSyntax
//@[15:0018) |   |   | | | | | └─Token(Identifier) |add|
//@[18:0019) |   |   | | | | ├─Token(LeftParen) |(|
//@[19:0020) |   |   | | | | ├─FunctionArgumentSyntax
//@[19:0020) |   |   | | | | | └─IntegerLiteralSyntax
//@[19:0020) |   |   | | | | |   └─Token(Integer) |1|
//@[20:0021) |   |   | | | | └─Token(RightParen) |)|
//@[22:0023) |   |   | | | ├─Token(Plus) |+|
//@[24:0036) |   |   | | | └─InstanceFunctionCallSyntax
//@[24:0027) |   |   | | |   ├─VariableAccessSyntax
//@[24:0027) |   |   | | |   | └─IdentifierSyntax
//@[24:0027) |   |   | | |   |   └─Token(Identifier) |sys|
//@[27:0028) |   |   | | |   ├─Token(Dot) |.|
//@[28:0031) |   |   | | |   ├─IdentifierSyntax
//@[28:0031) |   |   | | |   | └─Token(Identifier) |sub|
//@[31:0032) |   |   | | |   ├─Token(LeftParen) |(|
//@[32:0033) |   |   | | |   ├─FunctionArgumentSyntax
//@[32:0033) |   |   | | |   | └─IntegerLiteralSyntax
//@[32:0033) |   |   | | |   |   └─Token(Integer) |2|
//@[33:0034) |   |   | | |   ├─Token(Comma) |,|
//@[34:0035) |   |   | | |   ├─FunctionArgumentSyntax
//@[34:0035) |   |   | | |   | └─IntegerLiteralSyntax
//@[34:0035) |   |   | | |   |   └─Token(Integer) |3|
//@[35:0036) |   |   | | |   └─Token(RightParen) |)|
//@[37:0038) |   |   | | ├─Token(Plus) |+|
//@[39:0053) |   |   | | └─InstanceFunctionCallSyntax
//@[39:0042) |   |   | |   ├─VariableAccessSyntax
//@[39:0042) |   |   | |   | └─IdentifierSyntax
//@[39:0042) |   |   | |   |   └─Token(Identifier) |sys|
//@[42:0043) |   |   | |   ├─Token(Dot) |.|
//@[43:0046) |   |   | |   ├─IdentifierSyntax
//@[43:0046) |   |   | |   | └─Token(Identifier) |mul|
//@[46:0047) |   |   | |   ├─Token(LeftParen) |(|
//@[47:0048) |   |   | |   ├─FunctionArgumentSyntax
//@[47:0048) |   |   | |   | └─IntegerLiteralSyntax
//@[47:0048) |   |   | |   |   └─Token(Integer) |8|
//@[48:0049) |   |   | |   ├─Token(Comma) |,|
//@[49:0052) |   |   | |   ├─FunctionArgumentSyntax
//@[49:0052) |   |   | |   | └─StringSyntax
//@[49:0052) |   |   | |   |   └─Token(StringComplete) |'s'|
//@[52:0053) |   |   | |   └─Token(RightParen) |)|
//@[54:0055) |   |   | ├─Token(Plus) |+|
//@[56:0069) |   |   | └─InstanceFunctionCallSyntax
//@[56:0059) |   |   |   ├─VariableAccessSyntax
//@[56:0059) |   |   |   | └─IdentifierSyntax
//@[56:0059) |   |   |   |   └─Token(Identifier) |sys|
//@[59:0060) |   |   |   ├─Token(Dot) |.|
//@[60:0063) |   |   |   ├─IdentifierSyntax
//@[60:0063) |   |   |   | └─Token(Identifier) |div|
//@[63:0064) |   |   |   ├─Token(LeftParen) |(|
//@[64:0068) |   |   |   ├─FunctionArgumentSyntax
//@[64:0068) |   |   |   | └─BooleanLiteralSyntax
//@[64:0068) |   |   |   |   └─Token(TrueKeyword) |true|
//@[68:0069) |   |   |   └─Token(RightParen) |)|
//@[70:0071) |   |   ├─Token(Plus) |+|
//@[72:0092) |   |   └─InstanceFunctionCallSyntax
//@[72:0075) |   |     ├─VariableAccessSyntax
//@[72:0075) |   |     | └─IdentifierSyntax
//@[72:0075) |   |     |   └─Token(Identifier) |sys|
//@[75:0076) |   |     ├─Token(Dot) |.|
//@[76:0079) |   |     ├─IdentifierSyntax
//@[76:0079) |   |     | └─Token(Identifier) |mod|
//@[79:0080) |   |     ├─Token(LeftParen) |(|
//@[80:0084) |   |     ├─FunctionArgumentSyntax
//@[80:0084) |   |     | └─NullLiteralSyntax
//@[80:0084) |   |     |   └─Token(NullKeyword) |null|
//@[84:0085) |   |     ├─Token(Comma) |,|
//@[86:0091) |   |     ├─FunctionArgumentSyntax
//@[86:0091) |   |     | └─BooleanLiteralSyntax
//@[86:0091) |   |     |   └─Token(FalseKeyword) |false|
//@[91:0092) |   |     └─Token(RightParen) |)|
//@[92:0093) |   ├─Token(NewLine) |\n|
  relational: sys.less() && sys.lessOrEquals() && sys.greater() && sys.greaterOrEquals()
//@[02:0088) |   ├─ObjectPropertySyntax
//@[02:0012) |   | ├─IdentifierSyntax
//@[02:0012) |   | | └─Token(Identifier) |relational|
//@[12:0013) |   | ├─Token(Colon) |:|
//@[14:0088) |   | └─BinaryOperationSyntax
//@[14:0063) |   |   ├─BinaryOperationSyntax
//@[14:0046) |   |   | ├─BinaryOperationSyntax
//@[14:0024) |   |   | | ├─InstanceFunctionCallSyntax
//@[14:0017) |   |   | | | ├─VariableAccessSyntax
//@[14:0017) |   |   | | | | └─IdentifierSyntax
//@[14:0017) |   |   | | | |   └─Token(Identifier) |sys|
//@[17:0018) |   |   | | | ├─Token(Dot) |.|
//@[18:0022) |   |   | | | ├─IdentifierSyntax
//@[18:0022) |   |   | | | | └─Token(Identifier) |less|
//@[22:0023) |   |   | | | ├─Token(LeftParen) |(|
//@[23:0024) |   |   | | | └─Token(RightParen) |)|
//@[25:0027) |   |   | | ├─Token(LogicalAnd) |&&|
//@[28:0046) |   |   | | └─InstanceFunctionCallSyntax
//@[28:0031) |   |   | |   ├─VariableAccessSyntax
//@[28:0031) |   |   | |   | └─IdentifierSyntax
//@[28:0031) |   |   | |   |   └─Token(Identifier) |sys|
//@[31:0032) |   |   | |   ├─Token(Dot) |.|
//@[32:0044) |   |   | |   ├─IdentifierSyntax
//@[32:0044) |   |   | |   | └─Token(Identifier) |lessOrEquals|
//@[44:0045) |   |   | |   ├─Token(LeftParen) |(|
//@[45:0046) |   |   | |   └─Token(RightParen) |)|
//@[47:0049) |   |   | ├─Token(LogicalAnd) |&&|
//@[50:0063) |   |   | └─InstanceFunctionCallSyntax
//@[50:0053) |   |   |   ├─VariableAccessSyntax
//@[50:0053) |   |   |   | └─IdentifierSyntax
//@[50:0053) |   |   |   |   └─Token(Identifier) |sys|
//@[53:0054) |   |   |   ├─Token(Dot) |.|
//@[54:0061) |   |   |   ├─IdentifierSyntax
//@[54:0061) |   |   |   | └─Token(Identifier) |greater|
//@[61:0062) |   |   |   ├─Token(LeftParen) |(|
//@[62:0063) |   |   |   └─Token(RightParen) |)|
//@[64:0066) |   |   ├─Token(LogicalAnd) |&&|
//@[67:0088) |   |   └─InstanceFunctionCallSyntax
//@[67:0070) |   |     ├─VariableAccessSyntax
//@[67:0070) |   |     | └─IdentifierSyntax
//@[67:0070) |   |     |   └─Token(Identifier) |sys|
//@[70:0071) |   |     ├─Token(Dot) |.|
//@[71:0086) |   |     ├─IdentifierSyntax
//@[71:0086) |   |     | └─Token(Identifier) |greaterOrEquals|
//@[86:0087) |   |     ├─Token(LeftParen) |(|
//@[87:0088) |   |     └─Token(RightParen) |)|
//@[88:0089) |   ├─Token(NewLine) |\n|
  equals: sys.equals()
//@[02:0022) |   ├─ObjectPropertySyntax
//@[02:0008) |   | ├─IdentifierSyntax
//@[02:0008) |   | | └─Token(Identifier) |equals|
//@[08:0009) |   | ├─Token(Colon) |:|
//@[10:0022) |   | └─InstanceFunctionCallSyntax
//@[10:0013) |   |   ├─VariableAccessSyntax
//@[10:0013) |   |   | └─IdentifierSyntax
//@[10:0013) |   |   |   └─Token(Identifier) |sys|
//@[13:0014) |   |   ├─Token(Dot) |.|
//@[14:0020) |   |   ├─IdentifierSyntax
//@[14:0020) |   |   | └─Token(Identifier) |equals|
//@[20:0021) |   |   ├─Token(LeftParen) |(|
//@[21:0022) |   |   └─Token(RightParen) |)|
//@[22:0023) |   ├─Token(NewLine) |\n|
  bool: sys.not() || sys.and() || sys.or()
//@[02:0042) |   ├─ObjectPropertySyntax
//@[02:0006) |   | ├─IdentifierSyntax
//@[02:0006) |   | | └─Token(Identifier) |bool|
//@[06:0007) |   | ├─Token(Colon) |:|
//@[08:0042) |   | └─BinaryOperationSyntax
//@[08:0030) |   |   ├─BinaryOperationSyntax
//@[08:0017) |   |   | ├─InstanceFunctionCallSyntax
//@[08:0011) |   |   | | ├─VariableAccessSyntax
//@[08:0011) |   |   | | | └─IdentifierSyntax
//@[08:0011) |   |   | | |   └─Token(Identifier) |sys|
//@[11:0012) |   |   | | ├─Token(Dot) |.|
//@[12:0015) |   |   | | ├─IdentifierSyntax
//@[12:0015) |   |   | | | └─Token(Identifier) |not|
//@[15:0016) |   |   | | ├─Token(LeftParen) |(|
//@[16:0017) |   |   | | └─Token(RightParen) |)|
//@[18:0020) |   |   | ├─Token(LogicalOr) ||||
//@[21:0030) |   |   | └─InstanceFunctionCallSyntax
//@[21:0024) |   |   |   ├─VariableAccessSyntax
//@[21:0024) |   |   |   | └─IdentifierSyntax
//@[21:0024) |   |   |   |   └─Token(Identifier) |sys|
//@[24:0025) |   |   |   ├─Token(Dot) |.|
//@[25:0028) |   |   |   ├─IdentifierSyntax
//@[25:0028) |   |   |   | └─Token(Identifier) |and|
//@[28:0029) |   |   |   ├─Token(LeftParen) |(|
//@[29:0030) |   |   |   └─Token(RightParen) |)|
//@[31:0033) |   |   ├─Token(LogicalOr) ||||
//@[34:0042) |   |   └─InstanceFunctionCallSyntax
//@[34:0037) |   |     ├─VariableAccessSyntax
//@[34:0037) |   |     | └─IdentifierSyntax
//@[34:0037) |   |     |   └─Token(Identifier) |sys|
//@[37:0038) |   |     ├─Token(Dot) |.|
//@[38:0040) |   |     ├─IdentifierSyntax
//@[38:0040) |   |     | └─Token(Identifier) |or|
//@[40:0041) |   |     ├─Token(LeftParen) |(|
//@[41:0042) |   |     └─Token(RightParen) |)|
//@[42:0043) |   ├─Token(NewLine) |\n|
}
//@[00:0001) |   └─Token(RightBrace) |}|
//@[01:0003) ├─Token(NewLine) |\n\n|

// we can get function completions from namespaces
//@[50:0051) ├─Token(NewLine) |\n|
// #completionTest(22) -> azFunctions
//@[37:0038) ├─Token(NewLine) |\n|
var azFunctions = az.a
//@[00:0022) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0015) | ├─IdentifierSyntax
//@[04:0015) | | └─Token(Identifier) |azFunctions|
//@[16:0017) | ├─Token(Assignment) |=|
//@[18:0022) | └─PropertyAccessSyntax
//@[18:0020) |   ├─VariableAccessSyntax
//@[18:0020) |   | └─IdentifierSyntax
//@[18:0020) |   |   └─Token(Identifier) |az|
//@[20:0021) |   ├─Token(Dot) |.|
//@[21:0022) |   └─IdentifierSyntax
//@[21:0022) |     └─Token(Identifier) |a|
//@[22:0023) ├─Token(NewLine) |\n|
// #completionTest(24) -> sysFunctions
//@[38:0039) ├─Token(NewLine) |\n|
var sysFunctions = sys.a
//@[00:0024) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0016) | ├─IdentifierSyntax
//@[04:0016) | | └─Token(Identifier) |sysFunctions|
//@[17:0018) | ├─Token(Assignment) |=|
//@[19:0024) | └─PropertyAccessSyntax
//@[19:0022) |   ├─VariableAccessSyntax
//@[19:0022) |   | └─IdentifierSyntax
//@[19:0022) |   |   └─Token(Identifier) |sys|
//@[22:0023) |   ├─Token(Dot) |.|
//@[23:0024) |   └─IdentifierSyntax
//@[23:0024) |     └─Token(Identifier) |a|
//@[24:0026) ├─Token(NewLine) |\n\n|

// #completionTest(33) -> sysFunctions
//@[38:0039) ├─Token(NewLine) |\n|
var sysFunctionsInParens = (sys.a)
//@[00:0034) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0024) | ├─IdentifierSyntax
//@[04:0024) | | └─Token(Identifier) |sysFunctionsInParens|
//@[25:0026) | ├─Token(Assignment) |=|
//@[27:0034) | └─ParenthesizedExpressionSyntax
//@[27:0028) |   ├─Token(LeftParen) |(|
//@[28:0033) |   ├─PropertyAccessSyntax
//@[28:0031) |   | ├─VariableAccessSyntax
//@[28:0031) |   | | └─IdentifierSyntax
//@[28:0031) |   | |   └─Token(Identifier) |sys|
//@[31:0032) |   | ├─Token(Dot) |.|
//@[32:0033) |   | └─IdentifierSyntax
//@[32:0033) |   |   └─Token(Identifier) |a|
//@[33:0034) |   └─Token(RightParen) |)|
//@[34:0036) ├─Token(NewLine) |\n\n|

// missing method name
//@[22:0023) ├─Token(NewLine) |\n|
var missingMethodName = az.()
//@[00:0029) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0021) | ├─IdentifierSyntax
//@[04:0021) | | └─Token(Identifier) |missingMethodName|
//@[22:0023) | ├─Token(Assignment) |=|
//@[24:0029) | └─InstanceFunctionCallSyntax
//@[24:0026) |   ├─VariableAccessSyntax
//@[24:0026) |   | └─IdentifierSyntax
//@[24:0026) |   |   └─Token(Identifier) |az|
//@[26:0027) |   ├─Token(Dot) |.|
//@[27:0027) |   ├─IdentifierSyntax
//@[27:0027) |   | └─SkippedTriviaSyntax
//@[27:0028) |   ├─Token(LeftParen) |(|
//@[28:0029) |   └─Token(RightParen) |)|
//@[29:0031) ├─Token(NewLine) |\n\n|

// missing indexer
//@[18:0019) ├─Token(NewLine) |\n|
var missingIndexerOnLiteralArray = [][][]
//@[00:0041) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0032) | ├─IdentifierSyntax
//@[04:0032) | | └─Token(Identifier) |missingIndexerOnLiteralArray|
//@[33:0034) | ├─Token(Assignment) |=|
//@[35:0041) | └─ArrayAccessSyntax
//@[35:0039) |   ├─ArrayAccessSyntax
//@[35:0037) |   | ├─ArraySyntax
//@[35:0036) |   | | ├─Token(LeftSquare) |[|
//@[36:0037) |   | | └─Token(RightSquare) |]|
//@[37:0038) |   | ├─Token(LeftSquare) |[|
//@[38:0038) |   | ├─SkippedTriviaSyntax
//@[38:0039) |   | └─Token(RightSquare) |]|
//@[39:0040) |   ├─Token(LeftSquare) |[|
//@[40:0040) |   ├─SkippedTriviaSyntax
//@[40:0041) |   └─Token(RightSquare) |]|
//@[41:0042) ├─Token(NewLine) |\n|
var missingIndexerOnIdentifier = nonExistentIdentifier[][1][]
//@[00:0061) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0030) | ├─IdentifierSyntax
//@[04:0030) | | └─Token(Identifier) |missingIndexerOnIdentifier|
//@[31:0032) | ├─Token(Assignment) |=|
//@[33:0061) | └─ArrayAccessSyntax
//@[33:0059) |   ├─ArrayAccessSyntax
//@[33:0056) |   | ├─ArrayAccessSyntax
//@[33:0054) |   | | ├─VariableAccessSyntax
//@[33:0054) |   | | | └─IdentifierSyntax
//@[33:0054) |   | | |   └─Token(Identifier) |nonExistentIdentifier|
//@[54:0055) |   | | ├─Token(LeftSquare) |[|
//@[55:0055) |   | | ├─SkippedTriviaSyntax
//@[55:0056) |   | | └─Token(RightSquare) |]|
//@[56:0057) |   | ├─Token(LeftSquare) |[|
//@[57:0058) |   | ├─IntegerLiteralSyntax
//@[57:0058) |   | | └─Token(Integer) |1|
//@[58:0059) |   | └─Token(RightSquare) |]|
//@[59:0060) |   ├─Token(LeftSquare) |[|
//@[60:0060) |   ├─SkippedTriviaSyntax
//@[60:0061) |   └─Token(RightSquare) |]|
//@[61:0063) ├─Token(NewLine) |\n\n|

// empty parens - should produce expected expression diagnostic
//@[63:0064) ├─Token(NewLine) |\n|
var emptyParens = ()
//@[00:0020) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0015) | ├─IdentifierSyntax
//@[04:0015) | | └─Token(Identifier) |emptyParens|
//@[16:0017) | ├─Token(Assignment) |=|
//@[18:0020) | └─ParenthesizedExpressionSyntax
//@[18:0019) |   ├─Token(LeftParen) |(|
//@[19:0019) |   ├─SkippedTriviaSyntax
//@[19:0020) |   └─Token(RightParen) |)|
//@[20:0022) ├─Token(NewLine) |\n\n|

// #completionTest(26) -> symbols
//@[33:0034) ├─Token(NewLine) |\n|
var anotherEmptyParens = ()
//@[00:0027) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0022) | ├─IdentifierSyntax
//@[04:0022) | | └─Token(Identifier) |anotherEmptyParens|
//@[23:0024) | ├─Token(Assignment) |=|
//@[25:0027) | └─ParenthesizedExpressionSyntax
//@[25:0026) |   ├─Token(LeftParen) |(|
//@[26:0026) |   ├─SkippedTriviaSyntax
//@[26:0027) |   └─Token(RightParen) |)|
//@[27:0029) ├─Token(NewLine) |\n\n|

// keywords can't be called like functions
//@[42:0043) ├─Token(NewLine) |\n|
var nullness = null()
//@[00:0019) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0012) | ├─IdentifierSyntax
//@[04:0012) | | └─Token(Identifier) |nullness|
//@[13:0014) | ├─Token(Assignment) |=|
//@[15:0019) | └─NullLiteralSyntax
//@[15:0019) |   └─Token(NullKeyword) |null|
//@[19:0022) ├─SkippedTriviaSyntax
//@[19:0020) | ├─Token(LeftParen) |(|
//@[20:0021) | ├─Token(RightParen) |)|
//@[21:0022) | └─Token(NewLine) |\n|
var truth = true()
//@[00:0016) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0009) | ├─IdentifierSyntax
//@[04:0009) | | └─Token(Identifier) |truth|
//@[10:0011) | ├─Token(Assignment) |=|
//@[12:0016) | └─BooleanLiteralSyntax
//@[12:0016) |   └─Token(TrueKeyword) |true|
//@[16:0019) ├─SkippedTriviaSyntax
//@[16:0017) | ├─Token(LeftParen) |(|
//@[17:0018) | ├─Token(RightParen) |)|
//@[18:0019) | └─Token(NewLine) |\n|
var falsehood = false()
//@[00:0021) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0013) | ├─IdentifierSyntax
//@[04:0013) | | └─Token(Identifier) |falsehood|
//@[14:0015) | ├─Token(Assignment) |=|
//@[16:0021) | └─BooleanLiteralSyntax
//@[16:0021) |   └─Token(FalseKeyword) |false|
//@[21:0025) ├─SkippedTriviaSyntax
//@[21:0022) | ├─Token(LeftParen) |(|
//@[22:0023) | ├─Token(RightParen) |)|
//@[23:0025) | └─Token(NewLine) |\n\n|

var partialObject = {
//@[00:0126) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0017) | ├─IdentifierSyntax
//@[04:0017) | | └─Token(Identifier) |partialObject|
//@[18:0019) | ├─Token(Assignment) |=|
//@[20:0126) | └─ObjectSyntax
//@[20:0021) |   ├─Token(LeftBrace) |{|
//@[21:0022) |   ├─Token(NewLine) |\n|
  2: true
//@[02:0009) |   ├─ObjectPropertySyntax
//@[02:0003) |   | ├─SkippedTriviaSyntax
//@[02:0003) |   | | └─Token(Integer) |2|
//@[03:0004) |   | ├─Token(Colon) |:|
//@[05:0009) |   | └─BooleanLiteralSyntax
//@[05:0009) |   |   └─Token(TrueKeyword) |true|
//@[09:0010) |   ├─Token(NewLine) |\n|
  +
//@[02:0003) |   ├─ObjectPropertySyntax
//@[02:0003) |   | ├─SkippedTriviaSyntax
//@[02:0003) |   | | └─Token(Plus) |+|
//@[03:0003) |   | ├─SkippedTriviaSyntax
//@[03:0003) |   | └─SkippedTriviaSyntax
//@[03:0004) |   ├─Token(NewLine) |\n|
  3 : concat('s')
//@[02:0017) |   ├─ObjectPropertySyntax
//@[02:0003) |   | ├─SkippedTriviaSyntax
//@[02:0003) |   | | └─Token(Integer) |3|
//@[04:0005) |   | ├─Token(Colon) |:|
//@[06:0017) |   | └─FunctionCallSyntax
//@[06:0012) |   |   ├─IdentifierSyntax
//@[06:0012) |   |   | └─Token(Identifier) |concat|
//@[12:0013) |   |   ├─Token(LeftParen) |(|
//@[13:0016) |   |   ├─FunctionArgumentSyntax
//@[13:0016) |   |   | └─StringSyntax
//@[13:0016) |   |   |   └─Token(StringComplete) |'s'|
//@[16:0017) |   |   └─Token(RightParen) |)|
//@[17:0018) |   ├─Token(NewLine) |\n|
  
//@[02:0003) |   ├─Token(NewLine) |\n|
  's' 
//@[02:0006) |   ├─ObjectPropertySyntax
//@[02:0005) |   | ├─StringSyntax
//@[02:0005) |   | | └─Token(StringComplete) |'s'|
//@[06:0006) |   | ├─SkippedTriviaSyntax
//@[06:0006) |   | └─SkippedTriviaSyntax
//@[06:0007) |   ├─Token(NewLine) |\n|
  's' \
//@[02:0007) |   ├─ObjectPropertySyntax
//@[02:0005) |   | ├─StringSyntax
//@[02:0005) |   | | └─Token(StringComplete) |'s'|
//@[06:0007) |   | ├─SkippedTriviaSyntax
//@[06:0007) |   | | └─Token(Unrecognized) |\|
//@[07:0007) |   | └─SkippedTriviaSyntax
//@[07:0008) |   ├─Token(NewLine) |\n|
  'e'   =
//@[02:0009) |   ├─ObjectPropertySyntax
//@[02:0005) |   | ├─StringSyntax
//@[02:0005) |   | | └─Token(StringComplete) |'e'|
//@[08:0009) |   | ├─SkippedTriviaSyntax
//@[08:0009) |   | | └─Token(Assignment) |=|
//@[09:0009) |   | └─SkippedTriviaSyntax
//@[09:0010) |   ├─Token(NewLine) |\n|
  's' :
//@[02:0007) |   ├─ObjectPropertySyntax
//@[02:0005) |   | ├─StringSyntax
//@[02:0005) |   | | └─Token(StringComplete) |'s'|
//@[06:0007) |   | ├─Token(Colon) |:|
//@[07:0007) |   | └─SkippedTriviaSyntax
//@[07:0009) |   ├─Token(NewLine) |\n\n|

  a
//@[02:0003) |   ├─ObjectPropertySyntax
//@[02:0003) |   | ├─IdentifierSyntax
//@[02:0003) |   | | └─Token(Identifier) |a|
//@[03:0003) |   | ├─SkippedTriviaSyntax
//@[03:0003) |   | └─SkippedTriviaSyntax
//@[03:0004) |   ├─Token(NewLine) |\n|
  b $
//@[02:0005) |   ├─ObjectPropertySyntax
//@[02:0003) |   | ├─IdentifierSyntax
//@[02:0003) |   | | └─Token(Identifier) |b|
//@[04:0005) |   | ├─SkippedTriviaSyntax
//@[04:0005) |   | | └─Token(Unrecognized) |$|
//@[05:0005) |   | └─SkippedTriviaSyntax
//@[05:0006) |   ├─Token(NewLine) |\n|
  a # 22
//@[02:0008) |   ├─ObjectPropertySyntax
//@[02:0003) |   | ├─IdentifierSyntax
//@[02:0003) |   | | └─Token(Identifier) |a|
//@[04:0008) |   | ├─SkippedTriviaSyntax
//@[04:0005) |   | | ├─Token(Unrecognized) |#|
//@[06:0008) |   | | └─Token(Integer) |22|
//@[08:0008) |   | └─SkippedTriviaSyntax
//@[08:0009) |   ├─Token(NewLine) |\n|
  c :
//@[02:0005) |   ├─ObjectPropertySyntax
//@[02:0003) |   | ├─IdentifierSyntax
//@[02:0003) |   | | └─Token(Identifier) |c|
//@[04:0005) |   | ├─Token(Colon) |:|
//@[05:0005) |   | └─SkippedTriviaSyntax
//@[05:0006) |   ├─Token(NewLine) |\n|
  d  : %
//@[02:0008) |   ├─ObjectPropertySyntax
//@[02:0003) |   | ├─IdentifierSyntax
//@[02:0003) |   | | └─Token(Identifier) |d|
//@[05:0006) |   | ├─Token(Colon) |:|
//@[07:0008) |   | └─SkippedTriviaSyntax
//@[07:0008) |   |   └─Token(Modulo) |%|
//@[08:0009) |   ├─Token(NewLine) |\n|
}
//@[00:0001) |   └─Token(RightBrace) |}|
//@[01:0003) ├─Token(NewLine) |\n\n|

// dangling decorators - to make sure the tests work, please do not add contents after this line
//@[96:0097) ├─Token(NewLine) |\n|
@concat()
//@[00:0024) ├─MissingDeclarationSyntax
//@[00:0009) | ├─DecoratorSyntax
//@[00:0001) | | ├─Token(At) |@|
//@[01:0009) | | └─FunctionCallSyntax
//@[01:0007) | |   ├─IdentifierSyntax
//@[01:0007) | |   | └─Token(Identifier) |concat|
//@[07:0008) | |   ├─Token(LeftParen) |(|
//@[08:0009) | |   └─Token(RightParen) |)|
//@[09:0010) | ├─Token(NewLine) |\n|
@sys.secure()
//@[00:0013) | ├─DecoratorSyntax
//@[00:0001) | | ├─Token(At) |@|
//@[01:0013) | | └─InstanceFunctionCallSyntax
//@[01:0004) | |   ├─VariableAccessSyntax
//@[01:0004) | |   | └─IdentifierSyntax
//@[01:0004) | |   |   └─Token(Identifier) |sys|
//@[04:0005) | |   ├─Token(Dot) |.|
//@[05:0011) | |   ├─IdentifierSyntax
//@[05:0011) | |   | └─Token(Identifier) |secure|
//@[11:0012) | |   ├─Token(LeftParen) |(|
//@[12:0013) | |   └─Token(RightParen) |)|
//@[13:0014) | └─Token(NewLine) |\n|
xxxxx
//@[00:0005) ├─SkippedTriviaSyntax
//@[00:0005) | └─Token(Identifier) |xxxxx|
//@[05:0008) ├─Token(NewLine) |\n\n\n|


var noElements = ()
//@[00:0019) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0014) | ├─IdentifierSyntax
//@[04:0014) | | └─Token(Identifier) |noElements|
//@[15:0016) | ├─Token(Assignment) |=|
//@[17:0019) | └─ParenthesizedExpressionSyntax
//@[17:0018) |   ├─Token(LeftParen) |(|
//@[18:0018) |   ├─SkippedTriviaSyntax
//@[18:0019) |   └─Token(RightParen) |)|
//@[19:0020) ├─Token(NewLine) |\n|
var justAComma = (,)
//@[00:0020) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0014) | ├─IdentifierSyntax
//@[04:0014) | | └─Token(Identifier) |justAComma|
//@[15:0016) | ├─Token(Assignment) |=|
//@[17:0020) | └─ParenthesizedExpressionSyntax
//@[17:0018) |   ├─Token(LeftParen) |(|
//@[18:0019) |   ├─SkippedTriviaSyntax
//@[18:0018) |   | ├─SkippedTriviaSyntax
//@[18:0019) |   | └─Token(Comma) |,|
//@[19:0020) |   └─Token(RightParen) |)|
//@[20:0021) ├─Token(NewLine) |\n|
var twoElements = (1, 2)
//@[00:0024) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0015) | ├─IdentifierSyntax
//@[04:0015) | | └─Token(Identifier) |twoElements|
//@[16:0017) | ├─Token(Assignment) |=|
//@[18:0024) | └─ParenthesizedExpressionSyntax
//@[18:0019) |   ├─Token(LeftParen) |(|
//@[19:0023) |   ├─SkippedTriviaSyntax
//@[19:0020) |   | ├─IntegerLiteralSyntax
//@[19:0020) |   | | └─Token(Integer) |1|
//@[20:0021) |   | ├─Token(Comma) |,|
//@[22:0023) |   | └─IntegerLiteralSyntax
//@[22:0023) |   |   └─Token(Integer) |2|
//@[23:0024) |   └─Token(RightParen) |)|
//@[24:0025) ├─Token(NewLine) |\n|
var threeElements = (1, 2, 3)
//@[00:0029) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0017) | ├─IdentifierSyntax
//@[04:0017) | | └─Token(Identifier) |threeElements|
//@[18:0019) | ├─Token(Assignment) |=|
//@[20:0029) | └─ParenthesizedExpressionSyntax
//@[20:0021) |   ├─Token(LeftParen) |(|
//@[21:0028) |   ├─SkippedTriviaSyntax
//@[21:0022) |   | ├─IntegerLiteralSyntax
//@[21:0022) |   | | └─Token(Integer) |1|
//@[22:0023) |   | ├─Token(Comma) |,|
//@[24:0025) |   | ├─IntegerLiteralSyntax
//@[24:0025) |   | | └─Token(Integer) |2|
//@[25:0026) |   | ├─Token(Comma) |,|
//@[27:0028) |   | └─IntegerLiteralSyntax
//@[27:0028) |   |   └─Token(Integer) |3|
//@[28:0029) |   └─Token(RightParen) |)|
//@[29:0030) ├─Token(NewLine) |\n|
var unterminated1 = (
//@[00:0021) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0017) | ├─IdentifierSyntax
//@[04:0017) | | └─Token(Identifier) |unterminated1|
//@[18:0019) | ├─Token(Assignment) |=|
//@[20:0021) | └─ParenthesizedExpressionSyntax
//@[20:0021) |   ├─Token(LeftParen) |(|
//@[21:0021) |   ├─SkippedTriviaSyntax
//@[21:0021) |   └─SkippedTriviaSyntax
//@[21:0022) ├─Token(NewLine) |\n|
var unterminated2 = (,
//@[00:0022) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0017) | ├─IdentifierSyntax
//@[04:0017) | | └─Token(Identifier) |unterminated2|
//@[18:0019) | ├─Token(Assignment) |=|
//@[20:0022) | └─ParenthesizedExpressionSyntax
//@[20:0021) |   ├─Token(LeftParen) |(|
//@[21:0022) |   ├─SkippedTriviaSyntax
//@[21:0021) |   | ├─SkippedTriviaSyntax
//@[21:0022) |   | ├─Token(Comma) |,|
//@[22:0022) |   | └─SkippedTriviaSyntax
//@[22:0022) |   └─SkippedTriviaSyntax
//@[22:0024) ├─Token(NewLine) |\n\n|

// trailing decorator with no declaration
//@[41:0042) ├─Token(NewLine) |\n|
@minLength()
//@[00:0016) ├─MissingDeclarationSyntax
//@[00:0012) | ├─DecoratorSyntax
//@[00:0001) | | ├─Token(At) |@|
//@[01:0012) | | └─FunctionCallSyntax
//@[01:0010) | |   ├─IdentifierSyntax
//@[01:0010) | |   | └─Token(Identifier) |minLength|
//@[10:0011) | |   ├─Token(LeftParen) |(|
//@[11:0012) | |   └─Token(RightParen) |)|
//@[12:0016) | └─Token(NewLine) |\n\n\n\n|




//@[00:0000) └─Token(EndOfFile) ||
