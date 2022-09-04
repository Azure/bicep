
//@[00:5538) ProgramSyntax
//@[00:0001) ├─Token(NewLine) |\n|
// unknown declaration
//@[22:0023) ├─Token(NewLine) |\n|
bad
//@[00:0003) ├─SkippedTriviaSyntax
//@[00:0003) | └─Token(Identifier) |bad|
//@[03:0005) ├─Token(NewLine) |\n\n|

// incomplete variable declaration #completionTest(0,1,2) -> declarations
//@[73:0074) ├─Token(NewLine) |\n|
var
//@[00:0003) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[03:0003) | ├─IdentifierSyntax
//@[03:0003) | | └─SkippedTriviaSyntax
//@[03:0003) | ├─SkippedTriviaSyntax
//@[03:0003) | └─SkippedTriviaSyntax
//@[03:0005) ├─Token(NewLine) |\n\n|

// missing identifier #completionTest(4) -> empty
//@[49:0050) ├─Token(NewLine) |\n|
var 
//@[00:0004) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0004) | ├─IdentifierSyntax
//@[04:0004) | | └─SkippedTriviaSyntax
//@[04:0004) | ├─SkippedTriviaSyntax
//@[04:0004) | └─SkippedTriviaSyntax
//@[04:0006) ├─Token(NewLine) |\n\n|

// incomplete keyword
//@[21:0022) ├─Token(NewLine) |\n|
// #completionTest(0,1) -> declarations
//@[39:0040) ├─Token(NewLine) |\n|
v
//@[00:0001) ├─SkippedTriviaSyntax
//@[00:0001) | └─Token(Identifier) |v|
//@[01:0002) ├─Token(NewLine) |\n|
// #completionTest(0,1,2) -> declarations
//@[41:0042) ├─Token(NewLine) |\n|
va
//@[00:0002) ├─SkippedTriviaSyntax
//@[00:0002) | └─Token(Identifier) |va|
//@[02:0004) ├─Token(NewLine) |\n\n|

// unassigned variable
//@[22:0023) ├─Token(NewLine) |\n|
var foo
//@[00:0007) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0007) | ├─IdentifierSyntax
//@[04:0007) | | └─Token(Identifier) |foo|
//@[07:0007) | ├─SkippedTriviaSyntax
//@[07:0007) | └─SkippedTriviaSyntax
//@[07:0009) ├─Token(NewLine) |\n\n|

// #completionTest(18,19) -> symbols
//@[36:0037) ├─Token(NewLine) |\n|
var missingValue = 
//@[00:0019) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0016) | ├─IdentifierSyntax
//@[04:0016) | | └─Token(Identifier) |missingValue|
//@[17:0018) | ├─Token(Assignment) |=|
//@[19:0019) | └─SkippedTriviaSyntax
//@[19:0021) ├─Token(NewLine) |\n\n|

// malformed identifier
//@[23:0024) ├─Token(NewLine) |\n|
var 2 
//@[00:0006) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0005) | ├─IdentifierSyntax
//@[04:0005) | | └─SkippedTriviaSyntax
//@[04:0005) | |   └─Token(Integer) |2|
//@[06:0006) | ├─SkippedTriviaSyntax
//@[06:0006) | └─SkippedTriviaSyntax
//@[06:0007) ├─Token(NewLine) |\n|
var $ = 23
//@[00:0010) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0005) | ├─IdentifierSyntax
//@[04:0005) | | └─SkippedTriviaSyntax
//@[04:0005) | |   └─Token(Unrecognized) |$|
//@[06:0007) | ├─Token(Assignment) |=|
//@[08:0010) | └─IntegerLiteralSyntax
//@[08:0010) |   └─Token(Integer) |23|
//@[10:0011) ├─Token(NewLine) |\n|
var # 33 = 43
//@[00:0013) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0008) | ├─IdentifierSyntax
//@[04:0008) | | └─SkippedTriviaSyntax
//@[04:0005) | |   ├─Token(Unrecognized) |#|
//@[06:0008) | |   └─Token(Integer) |33|
//@[09:0010) | ├─Token(Assignment) |=|
//@[11:0013) | └─IntegerLiteralSyntax
//@[11:0013) |   └─Token(Integer) |43|
//@[13:0015) ├─Token(NewLine) |\n\n|

// no value assigned
//@[20:0021) ├─Token(NewLine) |\n|
var foo =
//@[00:0009) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0007) | ├─IdentifierSyntax
//@[04:0007) | | └─Token(Identifier) |foo|
//@[08:0009) | ├─Token(Assignment) |=|
//@[09:0009) | └─SkippedTriviaSyntax
//@[09:0011) ├─Token(NewLine) |\n\n|

// bad =
//@[08:0009) ├─Token(NewLine) |\n|
var badEquals 2
//@[00:0015) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0013) | ├─IdentifierSyntax
//@[04:0013) | | └─Token(Identifier) |badEquals|
//@[14:0015) | ├─SkippedTriviaSyntax
//@[14:0015) | | └─Token(Integer) |2|
//@[15:0015) | └─SkippedTriviaSyntax
//@[15:0016) ├─Token(NewLine) |\n|
var badEquals2 3 true
//@[00:0021) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0014) | ├─IdentifierSyntax
//@[04:0014) | | └─Token(Identifier) |badEquals2|
//@[15:0021) | ├─SkippedTriviaSyntax
//@[15:0016) | | ├─Token(Integer) |3|
//@[17:0021) | | └─Token(TrueKeyword) |true|
//@[21:0021) | └─SkippedTriviaSyntax
//@[21:0023) ├─Token(NewLine) |\n\n|

// malformed identifier but type check should happen regardless
//@[63:0064) ├─Token(NewLine) |\n|
var 2 = x
//@[00:0009) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0005) | ├─IdentifierSyntax
//@[04:0005) | | └─SkippedTriviaSyntax
//@[04:0005) | |   └─Token(Integer) |2|
//@[06:0007) | ├─Token(Assignment) |=|
//@[08:0009) | └─VariableAccessSyntax
//@[08:0009) |   └─IdentifierSyntax
//@[08:0009) |     └─Token(Identifier) |x|
//@[09:0011) ├─Token(NewLine) |\n\n|

// bad token value
//@[18:0019) ├─Token(NewLine) |\n|
var foo = &
//@[00:0011) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0007) | ├─IdentifierSyntax
//@[04:0007) | | └─Token(Identifier) |foo|
//@[08:0009) | ├─Token(Assignment) |=|
//@[10:0011) | └─SkippedTriviaSyntax
//@[10:0011) |   └─Token(Unrecognized) |&|
//@[11:0013) ├─Token(NewLine) |\n\n|

// bad value
//@[12:0013) ├─Token(NewLine) |\n|
var foo = *
//@[00:0011) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0007) | ├─IdentifierSyntax
//@[04:0007) | | └─Token(Identifier) |foo|
//@[08:0009) | ├─Token(Assignment) |=|
//@[10:0011) | └─SkippedTriviaSyntax
//@[10:0011) |   └─Token(Asterisk) |*|
//@[11:0013) ├─Token(NewLine) |\n\n|

// expressions
//@[14:0015) ├─Token(NewLine) |\n|
var bar = x
//@[00:0011) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0007) | ├─IdentifierSyntax
//@[04:0007) | | └─Token(Identifier) |bar|
//@[08:0009) | ├─Token(Assignment) |=|
//@[10:0011) | └─VariableAccessSyntax
//@[10:0011) |   └─IdentifierSyntax
//@[10:0011) |     └─Token(Identifier) |x|
//@[11:0012) ├─Token(NewLine) |\n|
var bar = foo()
//@[00:0015) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0007) | ├─IdentifierSyntax
//@[04:0007) | | └─Token(Identifier) |bar|
//@[08:0009) | ├─Token(Assignment) |=|
//@[10:0015) | └─FunctionCallSyntax
//@[10:0013) |   ├─IdentifierSyntax
//@[10:0013) |   | └─Token(Identifier) |foo|
//@[13:0014) |   ├─Token(LeftParen) |(|
//@[14:0015) |   └─Token(RightParen) |)|
//@[15:0016) ├─Token(NewLine) |\n|
var x = 2 + !3
//@[00:0014) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0005) | ├─IdentifierSyntax
//@[04:0005) | | └─Token(Identifier) |x|
//@[06:0007) | ├─Token(Assignment) |=|
//@[08:0014) | └─BinaryOperationSyntax
//@[08:0009) |   ├─IntegerLiteralSyntax
//@[08:0009) |   | └─Token(Integer) |2|
//@[10:0011) |   ├─Token(Plus) |+|
//@[12:0014) |   └─UnaryOperationSyntax
//@[12:0013) |     ├─Token(Exclamation) |!|
//@[13:0014) |     └─IntegerLiteralSyntax
//@[13:0014) |       └─Token(Integer) |3|
//@[14:0015) ├─Token(NewLine) |\n|
var y = false ? true + 1 : !4
//@[00:0029) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0005) | ├─IdentifierSyntax
//@[04:0005) | | └─Token(Identifier) |y|
//@[06:0007) | ├─Token(Assignment) |=|
//@[08:0029) | └─TernaryOperationSyntax
//@[08:0013) |   ├─BooleanLiteralSyntax
//@[08:0013) |   | └─Token(FalseKeyword) |false|
//@[14:0015) |   ├─Token(Question) |?|
//@[16:0024) |   ├─BinaryOperationSyntax
//@[16:0020) |   | ├─BooleanLiteralSyntax
//@[16:0020) |   | | └─Token(TrueKeyword) |true|
//@[21:0022) |   | ├─Token(Plus) |+|
//@[23:0024) |   | └─IntegerLiteralSyntax
//@[23:0024) |   |   └─Token(Integer) |1|
//@[25:0026) |   ├─Token(Colon) |:|
//@[27:0029) |   └─UnaryOperationSyntax
//@[27:0028) |     ├─Token(Exclamation) |!|
//@[28:0029) |     └─IntegerLiteralSyntax
//@[28:0029) |       └─Token(Integer) |4|
//@[29:0031) ├─Token(NewLine) |\n\n|

// test for array item recovery
//@[31:0032) ├─Token(NewLine) |\n|
var x = [
//@[00:0031) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0005) | ├─IdentifierSyntax
//@[04:0005) | | └─Token(Identifier) |x|
//@[06:0007) | ├─Token(Assignment) |=|
//@[08:0031) | └─ArraySyntax
//@[08:0009) |   ├─Token(LeftSquare) |[|
//@[09:0010) |   ├─Token(NewLine) |\n|
  3 + 4
//@[02:0007) |   ├─ArrayItemSyntax
//@[02:0007) |   | └─BinaryOperationSyntax
//@[02:0003) |   |   ├─IntegerLiteralSyntax
//@[02:0003) |   |   | └─Token(Integer) |3|
//@[04:0005) |   |   ├─Token(Plus) |+|
//@[06:0007) |   |   └─IntegerLiteralSyntax
//@[06:0007) |   |     └─Token(Integer) |4|
//@[07:0008) |   ├─Token(NewLine) |\n|
  =
//@[02:0003) |   ├─SkippedTriviaSyntax
//@[02:0003) |   | └─Token(Assignment) |=|
//@[03:0004) |   ├─Token(NewLine) |\n|
  !null
//@[02:0007) |   ├─ArrayItemSyntax
//@[02:0007) |   | └─UnaryOperationSyntax
//@[02:0003) |   |   ├─Token(Exclamation) |!|
//@[03:0007) |   |   └─NullLiteralSyntax
//@[03:0007) |   |     └─Token(NullKeyword) |null|
//@[07:0008) |   ├─Token(NewLine) |\n|
]
//@[00:0001) |   └─Token(RightSquare) |]|
//@[01:0003) ├─Token(NewLine) |\n\n|

// test for object property recovery
//@[36:0037) ├─Token(NewLine) |\n|
var y = {
//@[00:0025) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0005) | ├─IdentifierSyntax
//@[04:0005) | | └─Token(Identifier) |y|
//@[06:0007) | ├─Token(Assignment) |=|
//@[08:0025) | └─ObjectSyntax
//@[08:0009) |   ├─Token(LeftBrace) |{|
//@[09:0010) |   ├─Token(NewLine) |\n|
  =
//@[02:0003) |   ├─ObjectPropertySyntax
//@[02:0003) |   | ├─SkippedTriviaSyntax
//@[02:0003) |   | | └─Token(Assignment) |=|
//@[03:0003) |   | ├─SkippedTriviaSyntax
//@[03:0003) |   | └─SkippedTriviaSyntax
//@[03:0004) |   ├─Token(NewLine) |\n|
  foo: !2
//@[02:0009) |   ├─ObjectPropertySyntax
//@[02:0005) |   | ├─IdentifierSyntax
//@[02:0005) |   | | └─Token(Identifier) |foo|
//@[05:0006) |   | ├─Token(Colon) |:|
//@[07:0009) |   | └─UnaryOperationSyntax
//@[07:0008) |   |   ├─Token(Exclamation) |!|
//@[08:0009) |   |   └─IntegerLiteralSyntax
//@[08:0009) |   |     └─Token(Integer) |2|
//@[09:0010) |   ├─Token(NewLine) |\n|
}
//@[00:0001) |   └─Token(RightBrace) |}|
//@[01:0003) ├─Token(NewLine) |\n\n|

// utcNow and newGuid used outside a param default value
//@[56:0057) ├─Token(NewLine) |\n|
var test = utcNow('u')
//@[00:0022) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0008) | ├─IdentifierSyntax
//@[04:0008) | | └─Token(Identifier) |test|
//@[09:0010) | ├─Token(Assignment) |=|
//@[11:0022) | └─FunctionCallSyntax
//@[11:0017) |   ├─IdentifierSyntax
//@[11:0017) |   | └─Token(Identifier) |utcNow|
//@[17:0018) |   ├─Token(LeftParen) |(|
//@[18:0021) |   ├─FunctionArgumentSyntax
//@[18:0021) |   | └─StringSyntax
//@[18:0021) |   |   └─Token(StringComplete) |'u'|
//@[21:0022) |   └─Token(RightParen) |)|
//@[22:0023) ├─Token(NewLine) |\n|
var test2 = newGuid()
//@[00:0021) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0009) | ├─IdentifierSyntax
//@[04:0009) | | └─Token(Identifier) |test2|
//@[10:0011) | ├─Token(Assignment) |=|
//@[12:0021) | └─FunctionCallSyntax
//@[12:0019) |   ├─IdentifierSyntax
//@[12:0019) |   | └─Token(Identifier) |newGuid|
//@[19:0020) |   ├─Token(LeftParen) |(|
//@[20:0021) |   └─Token(RightParen) |)|
//@[21:0023) ├─Token(NewLine) |\n\n|

// bad string escape sequence in object key
//@[43:0044) ├─Token(NewLine) |\n|
var test3 = {
//@[00:0036) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0009) | ├─IdentifierSyntax
//@[04:0009) | | └─Token(Identifier) |test3|
//@[10:0011) | ├─Token(Assignment) |=|
//@[12:0036) | └─ObjectSyntax
//@[12:0013) |   ├─Token(LeftBrace) |{|
//@[13:0014) |   ├─Token(NewLine) |\n|
  'bad\escape': true
//@[02:0020) |   ├─ObjectPropertySyntax
//@[02:0014) |   | ├─SkippedTriviaSyntax
//@[02:0014) |   | | └─Token(StringComplete) |'bad\escape'|
//@[14:0015) |   | ├─Token(Colon) |:|
//@[16:0020) |   | └─BooleanLiteralSyntax
//@[16:0020) |   |   └─Token(TrueKeyword) |true|
//@[20:0021) |   ├─Token(NewLine) |\n|
}
//@[00:0001) |   └─Token(RightBrace) |}|
//@[01:0003) ├─Token(NewLine) |\n\n|

// duplicate properties
//@[23:0024) ├─Token(NewLine) |\n|
var testDupe = {
//@[00:0056) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0012) | ├─IdentifierSyntax
//@[04:0012) | | └─Token(Identifier) |testDupe|
//@[13:0014) | ├─Token(Assignment) |=|
//@[15:0056) | └─ObjectSyntax
//@[15:0016) |   ├─Token(LeftBrace) |{|
//@[16:0017) |   ├─Token(NewLine) |\n|
  'duplicate': true
//@[02:0019) |   ├─ObjectPropertySyntax
//@[02:0013) |   | ├─StringSyntax
//@[02:0013) |   | | └─Token(StringComplete) |'duplicate'|
//@[13:0014) |   | ├─Token(Colon) |:|
//@[15:0019) |   | └─BooleanLiteralSyntax
//@[15:0019) |   |   └─Token(TrueKeyword) |true|
//@[19:0020) |   ├─Token(NewLine) |\n|
  duplicate: true
//@[02:0017) |   ├─ObjectPropertySyntax
//@[02:0011) |   | ├─IdentifierSyntax
//@[02:0011) |   | | └─Token(Identifier) |duplicate|
//@[11:0012) |   | ├─Token(Colon) |:|
//@[13:0017) |   | └─BooleanLiteralSyntax
//@[13:0017) |   |   └─Token(TrueKeyword) |true|
//@[17:0018) |   ├─Token(NewLine) |\n|
}
//@[00:0001) |   └─Token(RightBrace) |}|
//@[01:0003) ├─Token(NewLine) |\n\n|

// interpolation with type errors in key
//@[40:0041) ├─Token(NewLine) |\n|
var objWithInterp = {
//@[00:0062) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0017) | ├─IdentifierSyntax
//@[04:0017) | | └─Token(Identifier) |objWithInterp|
//@[18:0019) | ├─Token(Assignment) |=|
//@[20:0062) | └─ObjectSyntax
//@[20:0021) |   ├─Token(LeftBrace) |{|
//@[21:0022) |   ├─Token(NewLine) |\n|
  'ab${nonExistentIdentifier}cd': true
//@[02:0038) |   ├─ObjectPropertySyntax
//@[02:0032) |   | ├─StringSyntax
//@[02:0007) |   | | ├─Token(StringLeftPiece) |'ab${|
//@[07:0028) |   | | ├─VariableAccessSyntax
//@[07:0028) |   | | | └─IdentifierSyntax
//@[07:0028) |   | | |   └─Token(Identifier) |nonExistentIdentifier|
//@[28:0032) |   | | └─Token(StringRightPiece) |}cd'|
//@[32:0033) |   | ├─Token(Colon) |:|
//@[34:0038) |   | └─BooleanLiteralSyntax
//@[34:0038) |   |   └─Token(TrueKeyword) |true|
//@[38:0039) |   ├─Token(NewLine) |\n|
}
//@[00:0001) |   └─Token(RightBrace) |}|
//@[01:0003) ├─Token(NewLine) |\n\n|

// invalid fully qualified function access
//@[42:0043) ├─Token(NewLine) |\n|
var mySum = az.add(1,2)
//@[00:0023) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0009) | ├─IdentifierSyntax
//@[04:0009) | | └─Token(Identifier) |mySum|
//@[10:0011) | ├─Token(Assignment) |=|
//@[12:0023) | └─InstanceFunctionCallSyntax
//@[12:0014) |   ├─VariableAccessSyntax
//@[12:0014) |   | └─IdentifierSyntax
//@[12:0014) |   |   └─Token(Identifier) |az|
//@[14:0015) |   ├─Token(Dot) |.|
//@[15:0018) |   ├─IdentifierSyntax
//@[15:0018) |   | └─Token(Identifier) |add|
//@[18:0019) |   ├─Token(LeftParen) |(|
//@[19:0020) |   ├─FunctionArgumentSyntax
//@[19:0020) |   | └─IntegerLiteralSyntax
//@[19:0020) |   |   └─Token(Integer) |1|
//@[20:0021) |   ├─Token(Comma) |,|
//@[21:0022) |   ├─FunctionArgumentSyntax
//@[21:0022) |   | └─IntegerLiteralSyntax
//@[21:0022) |   |   └─Token(Integer) |2|
//@[22:0023) |   └─Token(RightParen) |)|
//@[23:0024) ├─Token(NewLine) |\n|
var myConcat = sys.concat('a', az.concat('b', 'c'))
//@[00:0051) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0012) | ├─IdentifierSyntax
//@[04:0012) | | └─Token(Identifier) |myConcat|
//@[13:0014) | ├─Token(Assignment) |=|
//@[15:0051) | └─InstanceFunctionCallSyntax
//@[15:0018) |   ├─VariableAccessSyntax
//@[15:0018) |   | └─IdentifierSyntax
//@[15:0018) |   |   └─Token(Identifier) |sys|
//@[18:0019) |   ├─Token(Dot) |.|
//@[19:0025) |   ├─IdentifierSyntax
//@[19:0025) |   | └─Token(Identifier) |concat|
//@[25:0026) |   ├─Token(LeftParen) |(|
//@[26:0029) |   ├─FunctionArgumentSyntax
//@[26:0029) |   | └─StringSyntax
//@[26:0029) |   |   └─Token(StringComplete) |'a'|
//@[29:0030) |   ├─Token(Comma) |,|
//@[31:0050) |   ├─FunctionArgumentSyntax
//@[31:0050) |   | └─InstanceFunctionCallSyntax
//@[31:0033) |   |   ├─VariableAccessSyntax
//@[31:0033) |   |   | └─IdentifierSyntax
//@[31:0033) |   |   |   └─Token(Identifier) |az|
//@[33:0034) |   |   ├─Token(Dot) |.|
//@[34:0040) |   |   ├─IdentifierSyntax
//@[34:0040) |   |   | └─Token(Identifier) |concat|
//@[40:0041) |   |   ├─Token(LeftParen) |(|
//@[41:0044) |   |   ├─FunctionArgumentSyntax
//@[41:0044) |   |   | └─StringSyntax
//@[41:0044) |   |   |   └─Token(StringComplete) |'b'|
//@[44:0045) |   |   ├─Token(Comma) |,|
//@[46:0049) |   |   ├─FunctionArgumentSyntax
//@[46:0049) |   |   | └─StringSyntax
//@[46:0049) |   |   |   └─Token(StringComplete) |'c'|
//@[49:0050) |   |   └─Token(RightParen) |)|
//@[50:0051) |   └─Token(RightParen) |)|
//@[51:0053) ├─Token(NewLine) |\n\n|

// invalid string using double quotes
//@[37:0038) ├─Token(NewLine) |\n|
var doubleString = "bad string"
//@[00:0031) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0016) | ├─IdentifierSyntax
//@[04:0016) | | └─Token(Identifier) |doubleString|
//@[17:0018) | ├─Token(Assignment) |=|
//@[19:0031) | └─SkippedTriviaSyntax
//@[19:0020) |   ├─Token(Unrecognized) |"|
//@[20:0023) |   ├─Token(Identifier) |bad|
//@[24:0030) |   ├─Token(Identifier) |string|
//@[30:0031) |   └─Token(Unrecognized) |"|
//@[31:0033) ├─Token(NewLine) |\n\n|

var resourceGroup = ''
//@[00:0022) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0017) | ├─IdentifierSyntax
//@[04:0017) | | └─Token(Identifier) |resourceGroup|
//@[18:0019) | ├─Token(Assignment) |=|
//@[20:0022) | └─StringSyntax
//@[20:0022) |   └─Token(StringComplete) |''|
//@[22:0023) ├─Token(NewLine) |\n|
var rgName = resourceGroup().name
//@[00:0033) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0010) | ├─IdentifierSyntax
//@[04:0010) | | └─Token(Identifier) |rgName|
//@[11:0012) | ├─Token(Assignment) |=|
//@[13:0033) | └─PropertyAccessSyntax
//@[13:0028) |   ├─FunctionCallSyntax
//@[13:0026) |   | ├─IdentifierSyntax
//@[13:0026) |   | | └─Token(Identifier) |resourceGroup|
//@[26:0027) |   | ├─Token(LeftParen) |(|
//@[27:0028) |   | └─Token(RightParen) |)|
//@[28:0029) |   ├─Token(Dot) |.|
//@[29:0033) |   └─IdentifierSyntax
//@[29:0033) |     └─Token(Identifier) |name|
//@[33:0035) ├─Token(NewLine) |\n\n|

var subscription = ''
//@[00:0021) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0016) | ├─IdentifierSyntax
//@[04:0016) | | └─Token(Identifier) |subscription|
//@[17:0018) | ├─Token(Assignment) |=|
//@[19:0021) | └─StringSyntax
//@[19:0021) |   └─Token(StringComplete) |''|
//@[21:0022) ├─Token(NewLine) |\n|
var subName = subscription().name
//@[00:0033) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0011) | ├─IdentifierSyntax
//@[04:0011) | | └─Token(Identifier) |subName|
//@[12:0013) | ├─Token(Assignment) |=|
//@[14:0033) | └─PropertyAccessSyntax
//@[14:0028) |   ├─FunctionCallSyntax
//@[14:0026) |   | ├─IdentifierSyntax
//@[14:0026) |   | | └─Token(Identifier) |subscription|
//@[26:0027) |   | ├─Token(LeftParen) |(|
//@[27:0028) |   | └─Token(RightParen) |)|
//@[28:0029) |   ├─Token(Dot) |.|
//@[29:0033) |   └─IdentifierSyntax
//@[29:0033) |     └─Token(Identifier) |name|
//@[33:0035) ├─Token(NewLine) |\n\n|

// this does not work at the resource group scope
//@[49:0050) ├─Token(NewLine) |\n|
var invalidLocationVar = deployment().location
//@[00:0046) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0022) | ├─IdentifierSyntax
//@[04:0022) | | └─Token(Identifier) |invalidLocationVar|
//@[23:0024) | ├─Token(Assignment) |=|
//@[25:0046) | └─PropertyAccessSyntax
//@[25:0037) |   ├─FunctionCallSyntax
//@[25:0035) |   | ├─IdentifierSyntax
//@[25:0035) |   | | └─Token(Identifier) |deployment|
//@[35:0036) |   | ├─Token(LeftParen) |(|
//@[36:0037) |   | └─Token(RightParen) |)|
//@[37:0038) |   ├─Token(Dot) |.|
//@[38:0046) |   └─IdentifierSyntax
//@[38:0046) |     └─Token(Identifier) |location|
//@[46:0048) ├─Token(NewLine) |\n\n|

var invalidEnvironmentVar = environment().aosdufhsad
//@[00:0052) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0025) | ├─IdentifierSyntax
//@[04:0025) | | └─Token(Identifier) |invalidEnvironmentVar|
//@[26:0027) | ├─Token(Assignment) |=|
//@[28:0052) | └─PropertyAccessSyntax
//@[28:0041) |   ├─FunctionCallSyntax
//@[28:0039) |   | ├─IdentifierSyntax
//@[28:0039) |   | | └─Token(Identifier) |environment|
//@[39:0040) |   | ├─Token(LeftParen) |(|
//@[40:0041) |   | └─Token(RightParen) |)|
//@[41:0042) |   ├─Token(Dot) |.|
//@[42:0052) |   └─IdentifierSyntax
//@[42:0052) |     └─Token(Identifier) |aosdufhsad|
//@[52:0053) ├─Token(NewLine) |\n|
var invalidEnvAuthVar = environment().authentication.asdgdsag
//@[00:0061) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0021) | ├─IdentifierSyntax
//@[04:0021) | | └─Token(Identifier) |invalidEnvAuthVar|
//@[22:0023) | ├─Token(Assignment) |=|
//@[24:0061) | └─PropertyAccessSyntax
//@[24:0052) |   ├─PropertyAccessSyntax
//@[24:0037) |   | ├─FunctionCallSyntax
//@[24:0035) |   | | ├─IdentifierSyntax
//@[24:0035) |   | | | └─Token(Identifier) |environment|
//@[35:0036) |   | | ├─Token(LeftParen) |(|
//@[36:0037) |   | | └─Token(RightParen) |)|
//@[37:0038) |   | ├─Token(Dot) |.|
//@[38:0052) |   | └─IdentifierSyntax
//@[38:0052) |   |   └─Token(Identifier) |authentication|
//@[52:0053) |   ├─Token(Dot) |.|
//@[53:0061) |   └─IdentifierSyntax
//@[53:0061) |     └─Token(Identifier) |asdgdsag|
//@[61:0063) ├─Token(NewLine) |\n\n|

// invalid use of reserved namespace
//@[36:0037) ├─Token(NewLine) |\n|
var az = 1
//@[00:0010) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0006) | ├─IdentifierSyntax
//@[04:0006) | | └─Token(Identifier) |az|
//@[07:0008) | ├─Token(Assignment) |=|
//@[09:0010) | └─IntegerLiteralSyntax
//@[09:0010) |   └─Token(Integer) |1|
//@[10:0012) ├─Token(NewLine) |\n\n|

// cannot assign a variable to a namespace
//@[42:0043) ├─Token(NewLine) |\n|
var invalidNamespaceAssignment = az
//@[00:0035) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0030) | ├─IdentifierSyntax
//@[04:0030) | | └─Token(Identifier) |invalidNamespaceAssignment|
//@[31:0032) | ├─Token(Assignment) |=|
//@[33:0035) | └─VariableAccessSyntax
//@[33:0035) |   └─IdentifierSyntax
//@[33:0035) |     └─Token(Identifier) |az|
//@[35:0037) ├─Token(NewLine) |\n\n|

var objectLiteralType = {
//@[00:0199) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0021) | ├─IdentifierSyntax
//@[04:0021) | | └─Token(Identifier) |objectLiteralType|
//@[22:0023) | ├─Token(Assignment) |=|
//@[24:0199) | └─ObjectSyntax
//@[24:0025) |   ├─Token(LeftBrace) |{|
//@[25:0026) |   ├─Token(NewLine) |\n|
  first: true
//@[02:0013) |   ├─ObjectPropertySyntax
//@[02:0007) |   | ├─IdentifierSyntax
//@[02:0007) |   | | └─Token(Identifier) |first|
//@[07:0008) |   | ├─Token(Colon) |:|
//@[09:0013) |   | └─BooleanLiteralSyntax
//@[09:0013) |   |   └─Token(TrueKeyword) |true|
//@[13:0014) |   ├─Token(NewLine) |\n|
  second: false
//@[02:0015) |   ├─ObjectPropertySyntax
//@[02:0008) |   | ├─IdentifierSyntax
//@[02:0008) |   | | └─Token(Identifier) |second|
//@[08:0009) |   | ├─Token(Colon) |:|
//@[10:0015) |   | └─BooleanLiteralSyntax
//@[10:0015) |   |   └─Token(FalseKeyword) |false|
//@[15:0016) |   ├─Token(NewLine) |\n|
  third: 42
//@[02:0011) |   ├─ObjectPropertySyntax
//@[02:0007) |   | ├─IdentifierSyntax
//@[02:0007) |   | | └─Token(Identifier) |third|
//@[07:0008) |   | ├─Token(Colon) |:|
//@[09:0011) |   | └─IntegerLiteralSyntax
//@[09:0011) |   |   └─Token(Integer) |42|
//@[11:0012) |   ├─Token(NewLine) |\n|
  fourth: 'test'
//@[02:0016) |   ├─ObjectPropertySyntax
//@[02:0008) |   | ├─IdentifierSyntax
//@[02:0008) |   | | └─Token(Identifier) |fourth|
//@[08:0009) |   | ├─Token(Colon) |:|
//@[10:0016) |   | └─StringSyntax
//@[10:0016) |   |   └─Token(StringComplete) |'test'|
//@[16:0017) |   ├─Token(NewLine) |\n|
  fifth: [
//@[02:0071) |   ├─ObjectPropertySyntax
//@[02:0007) |   | ├─IdentifierSyntax
//@[02:0007) |   | | └─Token(Identifier) |fifth|
//@[07:0008) |   | ├─Token(Colon) |:|
//@[09:0071) |   | └─ArraySyntax
//@[09:0010) |   |   ├─Token(LeftSquare) |[|
//@[10:0011) |   |   ├─Token(NewLine) |\n|
    {
//@[04:0027) |   |   ├─ArrayItemSyntax
//@[04:0027) |   |   | └─ObjectSyntax
//@[04:0005) |   |   |   ├─Token(LeftBrace) |{|
//@[05:0006) |   |   |   ├─Token(NewLine) |\n|
      one: true
//@[06:0015) |   |   |   ├─ObjectPropertySyntax
//@[06:0009) |   |   |   | ├─IdentifierSyntax
//@[06:0009) |   |   |   | | └─Token(Identifier) |one|
//@[09:0010) |   |   |   | ├─Token(Colon) |:|
//@[11:0015) |   |   |   | └─BooleanLiteralSyntax
//@[11:0015) |   |   |   |   └─Token(TrueKeyword) |true|
//@[15:0016) |   |   |   ├─Token(NewLine) |\n|
    }
//@[04:0005) |   |   |   └─Token(RightBrace) |}|
//@[05:0006) |   |   ├─Token(NewLine) |\n|
    {
//@[04:0028) |   |   ├─ArrayItemSyntax
//@[04:0028) |   |   | └─ObjectSyntax
//@[04:0005) |   |   |   ├─Token(LeftBrace) |{|
//@[05:0006) |   |   |   ├─Token(NewLine) |\n|
      one: false
//@[06:0016) |   |   |   ├─ObjectPropertySyntax
//@[06:0009) |   |   |   | ├─IdentifierSyntax
//@[06:0009) |   |   |   | | └─Token(Identifier) |one|
//@[09:0010) |   |   |   | ├─Token(Colon) |:|
//@[11:0016) |   |   |   | └─BooleanLiteralSyntax
//@[11:0016) |   |   |   |   └─Token(FalseKeyword) |false|
//@[16:0017) |   |   |   ├─Token(NewLine) |\n|
    }
//@[04:0005) |   |   |   └─Token(RightBrace) |}|
//@[05:0006) |   |   ├─Token(NewLine) |\n|
  ]
//@[02:0003) |   |   └─Token(RightSquare) |]|
//@[03:0004) |   ├─Token(NewLine) |\n|
  sixth: [
//@[02:0040) |   ├─ObjectPropertySyntax
//@[02:0007) |   | ├─IdentifierSyntax
//@[02:0007) |   | | └─Token(Identifier) |sixth|
//@[07:0008) |   | ├─Token(Colon) |:|
//@[09:0040) |   | └─ArraySyntax
//@[09:0010) |   |   ├─Token(LeftSquare) |[|
//@[10:0011) |   |   ├─Token(NewLine) |\n|
    {
//@[04:0025) |   |   ├─ArrayItemSyntax
//@[04:0025) |   |   | └─ObjectSyntax
//@[04:0005) |   |   |   ├─Token(LeftBrace) |{|
//@[05:0006) |   |   |   ├─Token(NewLine) |\n|
      two: 44
//@[06:0013) |   |   |   ├─ObjectPropertySyntax
//@[06:0009) |   |   |   | ├─IdentifierSyntax
//@[06:0009) |   |   |   | | └─Token(Identifier) |two|
//@[09:0010) |   |   |   | ├─Token(Colon) |:|
//@[11:0013) |   |   |   | └─IntegerLiteralSyntax
//@[11:0013) |   |   |   |   └─Token(Integer) |44|
//@[13:0014) |   |   |   ├─Token(NewLine) |\n|
    }
//@[04:0005) |   |   |   └─Token(RightBrace) |}|
//@[05:0006) |   |   ├─Token(NewLine) |\n|
  ]
//@[02:0003) |   |   └─Token(RightSquare) |]|
//@[03:0004) |   ├─Token(NewLine) |\n|
}
//@[00:0001) |   └─Token(RightBrace) |}|
//@[01:0003) ├─Token(NewLine) |\n\n|

// #completionTest(54) -> objectVarTopLevel
//@[43:0044) ├─Token(NewLine) |\n|
var objectVarTopLevelCompletions = objectLiteralType.f
//@[00:0054) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0032) | ├─IdentifierSyntax
//@[04:0032) | | └─Token(Identifier) |objectVarTopLevelCompletions|
//@[33:0034) | ├─Token(Assignment) |=|
//@[35:0054) | └─PropertyAccessSyntax
//@[35:0052) |   ├─VariableAccessSyntax
//@[35:0052) |   | └─IdentifierSyntax
//@[35:0052) |   |   └─Token(Identifier) |objectLiteralType|
//@[52:0053) |   ├─Token(Dot) |.|
//@[53:0054) |   └─IdentifierSyntax
//@[53:0054) |     └─Token(Identifier) |f|
//@[54:0055) ├─Token(NewLine) |\n|
// #completionTest(54) -> objectVarTopLevel
//@[43:0044) ├─Token(NewLine) |\n|
var objectVarTopLevelCompletions2 = objectLiteralType.
//@[00:0054) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0033) | ├─IdentifierSyntax
//@[04:0033) | | └─Token(Identifier) |objectVarTopLevelCompletions2|
//@[34:0035) | ├─Token(Assignment) |=|
//@[36:0054) | └─PropertyAccessSyntax
//@[36:0053) |   ├─VariableAccessSyntax
//@[36:0053) |   | └─IdentifierSyntax
//@[36:0053) |   |   └─Token(Identifier) |objectLiteralType|
//@[53:0054) |   ├─Token(Dot) |.|
//@[54:0054) |   └─IdentifierSyntax
//@[54:0054) |     └─SkippedTriviaSyntax
//@[54:0056) ├─Token(NewLine) |\n\n|

// this does not produce any completions because mixed array items are of type "any"
//@[84:0085) ├─Token(NewLine) |\n|
// #completionTest(60) -> mixedArrayProperties
//@[46:0047) ├─Token(NewLine) |\n|
var mixedArrayTypeCompletions = objectLiteralType.fifth[0].o
//@[00:0060) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0029) | ├─IdentifierSyntax
//@[04:0029) | | └─Token(Identifier) |mixedArrayTypeCompletions|
//@[30:0031) | ├─Token(Assignment) |=|
//@[32:0060) | └─PropertyAccessSyntax
//@[32:0058) |   ├─ArrayAccessSyntax
//@[32:0055) |   | ├─PropertyAccessSyntax
//@[32:0049) |   | | ├─VariableAccessSyntax
//@[32:0049) |   | | | └─IdentifierSyntax
//@[32:0049) |   | | |   └─Token(Identifier) |objectLiteralType|
//@[49:0050) |   | | ├─Token(Dot) |.|
//@[50:0055) |   | | └─IdentifierSyntax
//@[50:0055) |   | |   └─Token(Identifier) |fifth|
//@[55:0056) |   | ├─Token(LeftSquare) |[|
//@[56:0057) |   | ├─IntegerLiteralSyntax
//@[56:0057) |   | | └─Token(Integer) |0|
//@[57:0058) |   | └─Token(RightSquare) |]|
//@[58:0059) |   ├─Token(Dot) |.|
//@[59:0060) |   └─IdentifierSyntax
//@[59:0060) |     └─Token(Identifier) |o|
//@[60:0061) ├─Token(NewLine) |\n|
// #completionTest(60) -> mixedArrayProperties
//@[46:0047) ├─Token(NewLine) |\n|
var mixedArrayTypeCompletions2 = objectLiteralType.fifth[0].
//@[00:0060) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0030) | ├─IdentifierSyntax
//@[04:0030) | | └─Token(Identifier) |mixedArrayTypeCompletions2|
//@[31:0032) | ├─Token(Assignment) |=|
//@[33:0060) | └─PropertyAccessSyntax
//@[33:0059) |   ├─ArrayAccessSyntax
//@[33:0056) |   | ├─PropertyAccessSyntax
//@[33:0050) |   | | ├─VariableAccessSyntax
//@[33:0050) |   | | | └─IdentifierSyntax
//@[33:0050) |   | | |   └─Token(Identifier) |objectLiteralType|
//@[50:0051) |   | | ├─Token(Dot) |.|
//@[51:0056) |   | | └─IdentifierSyntax
//@[51:0056) |   | |   └─Token(Identifier) |fifth|
//@[56:0057) |   | ├─Token(LeftSquare) |[|
//@[57:0058) |   | ├─IntegerLiteralSyntax
//@[57:0058) |   | | └─Token(Integer) |0|
//@[58:0059) |   | └─Token(RightSquare) |]|
//@[59:0060) |   ├─Token(Dot) |.|
//@[60:0060) |   └─IdentifierSyntax
//@[60:0060) |     └─SkippedTriviaSyntax
//@[60:0062) ├─Token(NewLine) |\n\n|

// #completionTest(58) -> oneArrayItemProperties
//@[48:0049) ├─Token(NewLine) |\n|
var oneArrayItemCompletions = objectLiteralType.sixth[0].t
//@[00:0058) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0027) | ├─IdentifierSyntax
//@[04:0027) | | └─Token(Identifier) |oneArrayItemCompletions|
//@[28:0029) | ├─Token(Assignment) |=|
//@[30:0058) | └─PropertyAccessSyntax
//@[30:0056) |   ├─ArrayAccessSyntax
//@[30:0053) |   | ├─PropertyAccessSyntax
//@[30:0047) |   | | ├─VariableAccessSyntax
//@[30:0047) |   | | | └─IdentifierSyntax
//@[30:0047) |   | | |   └─Token(Identifier) |objectLiteralType|
//@[47:0048) |   | | ├─Token(Dot) |.|
//@[48:0053) |   | | └─IdentifierSyntax
//@[48:0053) |   | |   └─Token(Identifier) |sixth|
//@[53:0054) |   | ├─Token(LeftSquare) |[|
//@[54:0055) |   | ├─IntegerLiteralSyntax
//@[54:0055) |   | | └─Token(Integer) |0|
//@[55:0056) |   | └─Token(RightSquare) |]|
//@[56:0057) |   ├─Token(Dot) |.|
//@[57:0058) |   └─IdentifierSyntax
//@[57:0058) |     └─Token(Identifier) |t|
//@[58:0059) ├─Token(NewLine) |\n|
// #completionTest(58) -> oneArrayItemProperties
//@[48:0049) ├─Token(NewLine) |\n|
var oneArrayItemCompletions2 = objectLiteralType.sixth[0].
//@[00:0058) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0028) | ├─IdentifierSyntax
//@[04:0028) | | └─Token(Identifier) |oneArrayItemCompletions2|
//@[29:0030) | ├─Token(Assignment) |=|
//@[31:0058) | └─PropertyAccessSyntax
//@[31:0057) |   ├─ArrayAccessSyntax
//@[31:0054) |   | ├─PropertyAccessSyntax
//@[31:0048) |   | | ├─VariableAccessSyntax
//@[31:0048) |   | | | └─IdentifierSyntax
//@[31:0048) |   | | |   └─Token(Identifier) |objectLiteralType|
//@[48:0049) |   | | ├─Token(Dot) |.|
//@[49:0054) |   | | └─IdentifierSyntax
//@[49:0054) |   | |   └─Token(Identifier) |sixth|
//@[54:0055) |   | ├─Token(LeftSquare) |[|
//@[55:0056) |   | ├─IntegerLiteralSyntax
//@[55:0056) |   | | └─Token(Integer) |0|
//@[56:0057) |   | └─Token(RightSquare) |]|
//@[57:0058) |   ├─Token(Dot) |.|
//@[58:0058) |   └─IdentifierSyntax
//@[58:0058) |     └─SkippedTriviaSyntax
//@[58:0060) ├─Token(NewLine) |\n\n|

// #completionTest(65) -> objectVarTopLevelIndexes
//@[50:0051) ├─Token(NewLine) |\n|
var objectVarTopLevelArrayIndexCompletions = objectLiteralType[f]
//@[00:0065) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0042) | ├─IdentifierSyntax
//@[04:0042) | | └─Token(Identifier) |objectVarTopLevelArrayIndexCompletions|
//@[43:0044) | ├─Token(Assignment) |=|
//@[45:0065) | └─ArrayAccessSyntax
//@[45:0062) |   ├─VariableAccessSyntax
//@[45:0062) |   | └─IdentifierSyntax
//@[45:0062) |   |   └─Token(Identifier) |objectLiteralType|
//@[62:0063) |   ├─Token(LeftSquare) |[|
//@[63:0064) |   ├─VariableAccessSyntax
//@[63:0064) |   | └─IdentifierSyntax
//@[63:0064) |   |   └─Token(Identifier) |f|
//@[64:0065) |   └─Token(RightSquare) |]|
//@[65:0067) ├─Token(NewLine) |\n\n|

// #completionTest(58) -> twoIndexPlusSymbols
//@[45:0046) ├─Token(NewLine) |\n|
var oneArrayIndexCompletions = objectLiteralType.sixth[0][]
//@[00:0059) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0028) | ├─IdentifierSyntax
//@[04:0028) | | └─Token(Identifier) |oneArrayIndexCompletions|
//@[29:0030) | ├─Token(Assignment) |=|
//@[31:0059) | └─ArrayAccessSyntax
//@[31:0057) |   ├─ArrayAccessSyntax
//@[31:0054) |   | ├─PropertyAccessSyntax
//@[31:0048) |   | | ├─VariableAccessSyntax
//@[31:0048) |   | | | └─IdentifierSyntax
//@[31:0048) |   | | |   └─Token(Identifier) |objectLiteralType|
//@[48:0049) |   | | ├─Token(Dot) |.|
//@[49:0054) |   | | └─IdentifierSyntax
//@[49:0054) |   | |   └─Token(Identifier) |sixth|
//@[54:0055) |   | ├─Token(LeftSquare) |[|
//@[55:0056) |   | ├─IntegerLiteralSyntax
//@[55:0056) |   | | └─Token(Integer) |0|
//@[56:0057) |   | └─Token(RightSquare) |]|
//@[57:0058) |   ├─Token(LeftSquare) |[|
//@[58:0058) |   ├─SkippedTriviaSyntax
//@[58:0059) |   └─Token(RightSquare) |]|
//@[59:0061) ├─Token(NewLine) |\n\n|

// Issue 486
//@[12:0013) ├─Token(NewLine) |\n|
var myFloat = 3.14
//@[00:0016) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0011) | ├─IdentifierSyntax
//@[04:0011) | | └─Token(Identifier) |myFloat|
//@[12:0013) | ├─Token(Assignment) |=|
//@[14:0016) | └─PropertyAccessSyntax
//@[14:0015) |   ├─IntegerLiteralSyntax
//@[14:0015) |   | └─Token(Integer) |3|
//@[15:0016) |   ├─Token(Dot) |.|
//@[16:0016) |   └─IdentifierSyntax
//@[16:0016) |     └─SkippedTriviaSyntax
//@[16:0020) ├─SkippedTriviaSyntax
//@[16:0018) | ├─Token(Integer) |14|
//@[18:0020) | └─Token(NewLine) |\n\n|

// secure cannot be used as a variable decorator
//@[48:0049) ├─Token(NewLine) |\n|
@sys.secure()
//@[00:0031) ├─VariableDeclarationSyntax
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
//@[13:0014) | ├─Token(NewLine) |\n|
var something = 1
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0013) | ├─IdentifierSyntax
//@[04:0013) | | └─Token(Identifier) |something|
//@[14:0015) | ├─Token(Assignment) |=|
//@[16:0017) | └─IntegerLiteralSyntax
//@[16:0017) |   └─Token(Integer) |1|
//@[17:0019) ├─Token(NewLine) |\n\n|

// #completionTest(1) -> sysAndDescription
//@[42:0043) ├─Token(NewLine) |\n|
@
//@[00:0068) ├─VariableDeclarationSyntax
//@[00:0001) | ├─DecoratorSyntax
//@[00:0001) | | ├─Token(At) |@|
//@[01:0001) | | └─SkippedTriviaSyntax
//@[01:0002) | ├─Token(NewLine) |\n|
// #completionTest(5) -> description
//@[36:0037) | ├─Token(NewLine) |\n|
@sys.
//@[00:0005) | ├─DecoratorSyntax
//@[00:0001) | | ├─Token(At) |@|
//@[01:0005) | | └─PropertyAccessSyntax
//@[01:0004) | |   ├─VariableAccessSyntax
//@[01:0004) | |   | └─IdentifierSyntax
//@[01:0004) | |   |   └─Token(Identifier) |sys|
//@[04:0005) | |   ├─Token(Dot) |.|
//@[05:0005) | |   └─IdentifierSyntax
//@[05:0005) | |     └─SkippedTriviaSyntax
//@[05:0006) | ├─Token(NewLine) |\n|
var anotherThing = true
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0016) | ├─IdentifierSyntax
//@[04:0016) | | └─Token(Identifier) |anotherThing|
//@[17:0018) | ├─Token(Assignment) |=|
//@[19:0023) | └─BooleanLiteralSyntax
//@[19:0023) |   └─Token(TrueKeyword) |true|
//@[23:0025) ├─Token(NewLine) |\n\n|

// invalid identifier character classes
//@[39:0040) ├─Token(NewLine) |\n|
var ☕ = true
//@[00:0012) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0005) | ├─IdentifierSyntax
//@[04:0005) | | └─SkippedTriviaSyntax
//@[04:0005) | |   └─Token(Unrecognized) |☕|
//@[06:0007) | ├─Token(Assignment) |=|
//@[08:0012) | └─BooleanLiteralSyntax
//@[08:0012) |   └─Token(TrueKeyword) |true|
//@[12:0013) ├─Token(NewLine) |\n|
var a☕ = true
//@[00:0013) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0005) | ├─IdentifierSyntax
//@[04:0005) | | └─Token(Identifier) |a|
//@[05:0013) | ├─SkippedTriviaSyntax
//@[05:0006) | | ├─Token(Unrecognized) |☕|
//@[07:0008) | | ├─Token(Assignment) |=|
//@[09:0013) | | └─Token(TrueKeyword) |true|
//@[13:0013) | └─SkippedTriviaSyntax
//@[13:0015) ├─Token(NewLine) |\n\n|

var missingArrayVariable = [for thing in stuff: 4]
//@[00:0050) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0024) | ├─IdentifierSyntax
//@[04:0024) | | └─Token(Identifier) |missingArrayVariable|
//@[25:0026) | ├─Token(Assignment) |=|
//@[27:0050) | └─ForSyntax
//@[27:0028) |   ├─Token(LeftSquare) |[|
//@[28:0031) |   ├─Token(Identifier) |for|
//@[32:0037) |   ├─LocalVariableSyntax
//@[32:0037) |   | └─IdentifierSyntax
//@[32:0037) |   |   └─Token(Identifier) |thing|
//@[38:0040) |   ├─Token(Identifier) |in|
//@[41:0046) |   ├─VariableAccessSyntax
//@[41:0046) |   | └─IdentifierSyntax
//@[41:0046) |   |   └─Token(Identifier) |stuff|
//@[46:0047) |   ├─Token(Colon) |:|
//@[48:0049) |   ├─IntegerLiteralSyntax
//@[48:0049) |   | └─Token(Integer) |4|
//@[49:0050) |   └─Token(RightSquare) |]|
//@[50:0052) ├─Token(NewLine) |\n\n|

// loops are only allowed at the top level
//@[42:0043) ├─Token(NewLine) |\n|
var nonTopLevelLoop = {
//@[00:0062) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0019) | ├─IdentifierSyntax
//@[04:0019) | | └─Token(Identifier) |nonTopLevelLoop|
//@[20:0021) | ├─Token(Assignment) |=|
//@[22:0062) | └─ObjectSyntax
//@[22:0023) |   ├─Token(LeftBrace) |{|
//@[23:0024) |   ├─Token(NewLine) |\n|
  notOkHere: [for thing in stuff: 4]
//@[02:0036) |   ├─ObjectPropertySyntax
//@[02:0011) |   | ├─IdentifierSyntax
//@[02:0011) |   | | └─Token(Identifier) |notOkHere|
//@[11:0012) |   | ├─Token(Colon) |:|
//@[13:0036) |   | └─ForSyntax
//@[13:0014) |   |   ├─Token(LeftSquare) |[|
//@[14:0017) |   |   ├─Token(Identifier) |for|
//@[18:0023) |   |   ├─LocalVariableSyntax
//@[18:0023) |   |   | └─IdentifierSyntax
//@[18:0023) |   |   |   └─Token(Identifier) |thing|
//@[24:0026) |   |   ├─Token(Identifier) |in|
//@[27:0032) |   |   ├─VariableAccessSyntax
//@[27:0032) |   |   | └─IdentifierSyntax
//@[27:0032) |   |   |   └─Token(Identifier) |stuff|
//@[32:0033) |   |   ├─Token(Colon) |:|
//@[34:0035) |   |   ├─IntegerLiteralSyntax
//@[34:0035) |   |   | └─Token(Integer) |4|
//@[35:0036) |   |   └─Token(RightSquare) |]|
//@[36:0037) |   ├─Token(NewLine) |\n|
}
//@[00:0001) |   └─Token(RightBrace) |}|
//@[01:0003) ├─Token(NewLine) |\n\n|

// loops with conditions won't even parse
//@[41:0042) ├─Token(NewLine) |\n|
var noFilteredLoopsInVariables = [for thing in stuff: if]
//@[00:0057) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0030) | ├─IdentifierSyntax
//@[04:0030) | | └─Token(Identifier) |noFilteredLoopsInVariables|
//@[31:0032) | ├─Token(Assignment) |=|
//@[33:0057) | └─ForSyntax
//@[33:0034) |   ├─Token(LeftSquare) |[|
//@[34:0037) |   ├─Token(Identifier) |for|
//@[38:0043) |   ├─LocalVariableSyntax
//@[38:0043) |   | └─IdentifierSyntax
//@[38:0043) |   |   └─Token(Identifier) |thing|
//@[44:0046) |   ├─Token(Identifier) |in|
//@[47:0052) |   ├─VariableAccessSyntax
//@[47:0052) |   | └─IdentifierSyntax
//@[47:0052) |   |   └─Token(Identifier) |stuff|
//@[52:0053) |   ├─Token(Colon) |:|
//@[54:0056) |   ├─VariableAccessSyntax
//@[54:0056) |   | └─IdentifierSyntax
//@[54:0056) |   |   └─Token(Identifier) |if|
//@[56:0057) |   └─Token(RightSquare) |]|
//@[57:0059) ├─Token(NewLine) |\n\n|

// nested loops are also not allowed
//@[36:0037) ├─Token(NewLine) |\n|
var noNestedVariableLoopsEither = [for thing in stuff: {
//@[00:0089) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0031) | ├─IdentifierSyntax
//@[04:0031) | | └─Token(Identifier) |noNestedVariableLoopsEither|
//@[32:0033) | ├─Token(Assignment) |=|
//@[34:0089) | └─ForSyntax
//@[34:0035) |   ├─Token(LeftSquare) |[|
//@[35:0038) |   ├─Token(Identifier) |for|
//@[39:0044) |   ├─LocalVariableSyntax
//@[39:0044) |   | └─IdentifierSyntax
//@[39:0044) |   |   └─Token(Identifier) |thing|
//@[45:0047) |   ├─Token(Identifier) |in|
//@[48:0053) |   ├─VariableAccessSyntax
//@[48:0053) |   | └─IdentifierSyntax
//@[48:0053) |   |   └─Token(Identifier) |stuff|
//@[53:0054) |   ├─Token(Colon) |:|
//@[55:0088) |   ├─ObjectSyntax
//@[55:0056) |   | ├─Token(LeftBrace) |{|
//@[56:0057) |   | ├─Token(NewLine) |\n|
  hello: [for thing in []: 4]
//@[02:0029) |   | ├─ObjectPropertySyntax
//@[02:0007) |   | | ├─IdentifierSyntax
//@[02:0007) |   | | | └─Token(Identifier) |hello|
//@[07:0008) |   | | ├─Token(Colon) |:|
//@[09:0029) |   | | └─ForSyntax
//@[09:0010) |   | |   ├─Token(LeftSquare) |[|
//@[10:0013) |   | |   ├─Token(Identifier) |for|
//@[14:0019) |   | |   ├─LocalVariableSyntax
//@[14:0019) |   | |   | └─IdentifierSyntax
//@[14:0019) |   | |   |   └─Token(Identifier) |thing|
//@[20:0022) |   | |   ├─Token(Identifier) |in|
//@[23:0025) |   | |   ├─ArraySyntax
//@[23:0024) |   | |   | ├─Token(LeftSquare) |[|
//@[24:0025) |   | |   | └─Token(RightSquare) |]|
//@[25:0026) |   | |   ├─Token(Colon) |:|
//@[27:0028) |   | |   ├─IntegerLiteralSyntax
//@[27:0028) |   | |   | └─Token(Integer) |4|
//@[28:0029) |   | |   └─Token(RightSquare) |]|
//@[29:0030) |   | ├─Token(NewLine) |\n|
}]
//@[00:0001) |   | └─Token(RightBrace) |}|
//@[01:0002) |   └─Token(RightSquare) |]|
//@[02:0004) ├─Token(NewLine) |\n\n|

// loops in inner properties of a variable are also not supported
//@[65:0066) ├─Token(NewLine) |\n|
var innerPropertyLoop = {
//@[00:0058) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0021) | ├─IdentifierSyntax
//@[04:0021) | | └─Token(Identifier) |innerPropertyLoop|
//@[22:0023) | ├─Token(Assignment) |=|
//@[24:0058) | └─ObjectSyntax
//@[24:0025) |   ├─Token(LeftBrace) |{|
//@[25:0026) |   ├─Token(NewLine) |\n|
  a: [for i in range(0,10): i]
//@[02:0030) |   ├─ObjectPropertySyntax
//@[02:0003) |   | ├─IdentifierSyntax
//@[02:0003) |   | | └─Token(Identifier) |a|
//@[03:0004) |   | ├─Token(Colon) |:|
//@[05:0030) |   | └─ForSyntax
//@[05:0006) |   |   ├─Token(LeftSquare) |[|
//@[06:0009) |   |   ├─Token(Identifier) |for|
//@[10:0011) |   |   ├─LocalVariableSyntax
//@[10:0011) |   |   | └─IdentifierSyntax
//@[10:0011) |   |   |   └─Token(Identifier) |i|
//@[12:0014) |   |   ├─Token(Identifier) |in|
//@[15:0026) |   |   ├─FunctionCallSyntax
//@[15:0020) |   |   | ├─IdentifierSyntax
//@[15:0020) |   |   | | └─Token(Identifier) |range|
//@[20:0021) |   |   | ├─Token(LeftParen) |(|
//@[21:0022) |   |   | ├─FunctionArgumentSyntax
//@[21:0022) |   |   | | └─IntegerLiteralSyntax
//@[21:0022) |   |   | |   └─Token(Integer) |0|
//@[22:0023) |   |   | ├─Token(Comma) |,|
//@[23:0025) |   |   | ├─FunctionArgumentSyntax
//@[23:0025) |   |   | | └─IntegerLiteralSyntax
//@[23:0025) |   |   | |   └─Token(Integer) |10|
//@[25:0026) |   |   | └─Token(RightParen) |)|
//@[26:0027) |   |   ├─Token(Colon) |:|
//@[28:0029) |   |   ├─VariableAccessSyntax
//@[28:0029) |   |   | └─IdentifierSyntax
//@[28:0029) |   |   |   └─Token(Identifier) |i|
//@[29:0030) |   |   └─Token(RightSquare) |]|
//@[30:0031) |   ├─Token(NewLine) |\n|
}
//@[00:0001) |   └─Token(RightBrace) |}|
//@[01:0002) ├─Token(NewLine) |\n|
var innerPropertyLoop2 = {
//@[00:0072) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0022) | ├─IdentifierSyntax
//@[04:0022) | | └─Token(Identifier) |innerPropertyLoop2|
//@[23:0024) | ├─Token(Assignment) |=|
//@[25:0072) | └─ObjectSyntax
//@[25:0026) |   ├─Token(LeftBrace) |{|
//@[26:0027) |   ├─Token(NewLine) |\n|
  b: {
//@[02:0043) |   ├─ObjectPropertySyntax
//@[02:0003) |   | ├─IdentifierSyntax
//@[02:0003) |   | | └─Token(Identifier) |b|
//@[03:0004) |   | ├─Token(Colon) |:|
//@[05:0043) |   | └─ObjectSyntax
//@[05:0006) |   |   ├─Token(LeftBrace) |{|
//@[06:0007) |   |   ├─Token(NewLine) |\n|
    a: [for i in range(0,10): i]
//@[04:0032) |   |   ├─ObjectPropertySyntax
//@[04:0005) |   |   | ├─IdentifierSyntax
//@[04:0005) |   |   | | └─Token(Identifier) |a|
//@[05:0006) |   |   | ├─Token(Colon) |:|
//@[07:0032) |   |   | └─ForSyntax
//@[07:0008) |   |   |   ├─Token(LeftSquare) |[|
//@[08:0011) |   |   |   ├─Token(Identifier) |for|
//@[12:0013) |   |   |   ├─LocalVariableSyntax
//@[12:0013) |   |   |   | └─IdentifierSyntax
//@[12:0013) |   |   |   |   └─Token(Identifier) |i|
//@[14:0016) |   |   |   ├─Token(Identifier) |in|
//@[17:0028) |   |   |   ├─FunctionCallSyntax
//@[17:0022) |   |   |   | ├─IdentifierSyntax
//@[17:0022) |   |   |   | | └─Token(Identifier) |range|
//@[22:0023) |   |   |   | ├─Token(LeftParen) |(|
//@[23:0024) |   |   |   | ├─FunctionArgumentSyntax
//@[23:0024) |   |   |   | | └─IntegerLiteralSyntax
//@[23:0024) |   |   |   | |   └─Token(Integer) |0|
//@[24:0025) |   |   |   | ├─Token(Comma) |,|
//@[25:0027) |   |   |   | ├─FunctionArgumentSyntax
//@[25:0027) |   |   |   | | └─IntegerLiteralSyntax
//@[25:0027) |   |   |   | |   └─Token(Integer) |10|
//@[27:0028) |   |   |   | └─Token(RightParen) |)|
//@[28:0029) |   |   |   ├─Token(Colon) |:|
//@[30:0031) |   |   |   ├─VariableAccessSyntax
//@[30:0031) |   |   |   | └─IdentifierSyntax
//@[30:0031) |   |   |   |   └─Token(Identifier) |i|
//@[31:0032) |   |   |   └─Token(RightSquare) |]|
//@[32:0033) |   |   ├─Token(NewLine) |\n|
  }
//@[02:0003) |   |   └─Token(RightBrace) |}|
//@[03:0004) |   ├─Token(NewLine) |\n|
}
//@[00:0001) |   └─Token(RightBrace) |}|
//@[01:0003) ├─Token(NewLine) |\n\n|

// loops using expressions with a runtime dependency are also not allowed
//@[73:0074) ├─Token(NewLine) |\n|
var keys = listKeys('fake','fake')
//@[00:0034) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0008) | ├─IdentifierSyntax
//@[04:0008) | | └─Token(Identifier) |keys|
//@[09:0010) | ├─Token(Assignment) |=|
//@[11:0034) | └─FunctionCallSyntax
//@[11:0019) |   ├─IdentifierSyntax
//@[11:0019) |   | └─Token(Identifier) |listKeys|
//@[19:0020) |   ├─Token(LeftParen) |(|
//@[20:0026) |   ├─FunctionArgumentSyntax
//@[20:0026) |   | └─StringSyntax
//@[20:0026) |   |   └─Token(StringComplete) |'fake'|
//@[26:0027) |   ├─Token(Comma) |,|
//@[27:0033) |   ├─FunctionArgumentSyntax
//@[27:0033) |   | └─StringSyntax
//@[27:0033) |   |   └─Token(StringComplete) |'fake'|
//@[33:0034) |   └─Token(RightParen) |)|
//@[34:0035) ├─Token(NewLine) |\n|
var indirection = keys
//@[00:0022) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0015) | ├─IdentifierSyntax
//@[04:0015) | | └─Token(Identifier) |indirection|
//@[16:0017) | ├─Token(Assignment) |=|
//@[18:0022) | └─VariableAccessSyntax
//@[18:0022) |   └─IdentifierSyntax
//@[18:0022) |     └─Token(Identifier) |keys|
//@[22:0024) ├─Token(NewLine) |\n\n|

var runtimeLoop = [for (item, index) in []: indirection]
//@[00:0056) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0015) | ├─IdentifierSyntax
//@[04:0015) | | └─Token(Identifier) |runtimeLoop|
//@[16:0017) | ├─Token(Assignment) |=|
//@[18:0056) | └─ForSyntax
//@[18:0019) |   ├─Token(LeftSquare) |[|
//@[19:0022) |   ├─Token(Identifier) |for|
//@[23:0036) |   ├─VariableBlockSyntax
//@[23:0024) |   | ├─Token(LeftParen) |(|
//@[24:0028) |   | ├─LocalVariableSyntax
//@[24:0028) |   | | └─IdentifierSyntax
//@[24:0028) |   | |   └─Token(Identifier) |item|
//@[28:0029) |   | ├─Token(Comma) |,|
//@[30:0035) |   | ├─LocalVariableSyntax
//@[30:0035) |   | | └─IdentifierSyntax
//@[30:0035) |   | |   └─Token(Identifier) |index|
//@[35:0036) |   | └─Token(RightParen) |)|
//@[37:0039) |   ├─Token(Identifier) |in|
//@[40:0042) |   ├─ArraySyntax
//@[40:0041) |   | ├─Token(LeftSquare) |[|
//@[41:0042) |   | └─Token(RightSquare) |]|
//@[42:0043) |   ├─Token(Colon) |:|
//@[44:0055) |   ├─VariableAccessSyntax
//@[44:0055) |   | └─IdentifierSyntax
//@[44:0055) |   |   └─Token(Identifier) |indirection|
//@[55:0056) |   └─Token(RightSquare) |]|
//@[56:0057) ├─Token(NewLine) |\n|
var runtimeLoop2 = [for (item, index) in indirection.keys: 's']
//@[00:0063) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0016) | ├─IdentifierSyntax
//@[04:0016) | | └─Token(Identifier) |runtimeLoop2|
//@[17:0018) | ├─Token(Assignment) |=|
//@[19:0063) | └─ForSyntax
//@[19:0020) |   ├─Token(LeftSquare) |[|
//@[20:0023) |   ├─Token(Identifier) |for|
//@[24:0037) |   ├─VariableBlockSyntax
//@[24:0025) |   | ├─Token(LeftParen) |(|
//@[25:0029) |   | ├─LocalVariableSyntax
//@[25:0029) |   | | └─IdentifierSyntax
//@[25:0029) |   | |   └─Token(Identifier) |item|
//@[29:0030) |   | ├─Token(Comma) |,|
//@[31:0036) |   | ├─LocalVariableSyntax
//@[31:0036) |   | | └─IdentifierSyntax
//@[31:0036) |   | |   └─Token(Identifier) |index|
//@[36:0037) |   | └─Token(RightParen) |)|
//@[38:0040) |   ├─Token(Identifier) |in|
//@[41:0057) |   ├─PropertyAccessSyntax
//@[41:0052) |   | ├─VariableAccessSyntax
//@[41:0052) |   | | └─IdentifierSyntax
//@[41:0052) |   | |   └─Token(Identifier) |indirection|
//@[52:0053) |   | ├─Token(Dot) |.|
//@[53:0057) |   | └─IdentifierSyntax
//@[53:0057) |   |   └─Token(Identifier) |keys|
//@[57:0058) |   ├─Token(Colon) |:|
//@[59:0062) |   ├─StringSyntax
//@[59:0062) |   | └─Token(StringComplete) |'s'|
//@[62:0063) |   └─Token(RightSquare) |]|
//@[63:0065) ├─Token(NewLine) |\n\n|

var zoneInput = []
//@[00:0018) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0013) | ├─IdentifierSyntax
//@[04:0013) | | └─Token(Identifier) |zoneInput|
//@[14:0015) | ├─Token(Assignment) |=|
//@[16:0018) | └─ArraySyntax
//@[16:0017) |   ├─Token(LeftSquare) |[|
//@[17:0018) |   └─Token(RightSquare) |]|
//@[18:0019) ├─Token(NewLine) |\n|
resource zones 'Microsoft.Network/dnsZones@2018-05-01' = [for (zone, i) in zoneInput: {
//@[00:0143) ├─ResourceDeclarationSyntax
//@[00:0008) | ├─Token(Identifier) |resource|
//@[09:0014) | ├─IdentifierSyntax
//@[09:0014) | | └─Token(Identifier) |zones|
//@[15:0054) | ├─StringSyntax
//@[15:0054) | | └─Token(StringComplete) |'Microsoft.Network/dnsZones@2018-05-01'|
//@[55:0056) | ├─Token(Assignment) |=|
//@[57:0143) | └─ForSyntax
//@[57:0058) |   ├─Token(LeftSquare) |[|
//@[58:0061) |   ├─Token(Identifier) |for|
//@[62:0071) |   ├─VariableBlockSyntax
//@[62:0063) |   | ├─Token(LeftParen) |(|
//@[63:0067) |   | ├─LocalVariableSyntax
//@[63:0067) |   | | └─IdentifierSyntax
//@[63:0067) |   | |   └─Token(Identifier) |zone|
//@[67:0068) |   | ├─Token(Comma) |,|
//@[69:0070) |   | ├─LocalVariableSyntax
//@[69:0070) |   | | └─IdentifierSyntax
//@[69:0070) |   | |   └─Token(Identifier) |i|
//@[70:0071) |   | └─Token(RightParen) |)|
//@[72:0074) |   ├─Token(Identifier) |in|
//@[75:0084) |   ├─VariableAccessSyntax
//@[75:0084) |   | └─IdentifierSyntax
//@[75:0084) |   |   └─Token(Identifier) |zoneInput|
//@[84:0085) |   ├─Token(Colon) |:|
//@[86:0142) |   ├─ObjectSyntax
//@[86:0087) |   | ├─Token(LeftBrace) |{|
//@[87:0088) |   | ├─Token(NewLine) |\n|
  name: zone
//@[02:0012) |   | ├─ObjectPropertySyntax
//@[02:0006) |   | | ├─IdentifierSyntax
//@[02:0006) |   | | | └─Token(Identifier) |name|
//@[06:0007) |   | | ├─Token(Colon) |:|
//@[08:0012) |   | | └─VariableAccessSyntax
//@[08:0012) |   | |   └─IdentifierSyntax
//@[08:0012) |   | |     └─Token(Identifier) |zone|
//@[12:0013) |   | ├─Token(NewLine) |\n|
  location: az.resourceGroup().location
//@[02:0039) |   | ├─ObjectPropertySyntax
//@[02:0010) |   | | ├─IdentifierSyntax
//@[02:0010) |   | | | └─Token(Identifier) |location|
//@[10:0011) |   | | ├─Token(Colon) |:|
//@[12:0039) |   | | └─PropertyAccessSyntax
//@[12:0030) |   | |   ├─InstanceFunctionCallSyntax
//@[12:0014) |   | |   | ├─VariableAccessSyntax
//@[12:0014) |   | |   | | └─IdentifierSyntax
//@[12:0014) |   | |   | |   └─Token(Identifier) |az|
//@[14:0015) |   | |   | ├─Token(Dot) |.|
//@[15:0028) |   | |   | ├─IdentifierSyntax
//@[15:0028) |   | |   | | └─Token(Identifier) |resourceGroup|
//@[28:0029) |   | |   | ├─Token(LeftParen) |(|
//@[29:0030) |   | |   | └─Token(RightParen) |)|
//@[30:0031) |   | |   ├─Token(Dot) |.|
//@[31:0039) |   | |   └─IdentifierSyntax
//@[31:0039) |   | |     └─Token(Identifier) |location|
//@[39:0040) |   | ├─Token(NewLine) |\n|
}]
//@[00:0001) |   | └─Token(RightBrace) |}|
//@[01:0002) |   └─Token(RightSquare) |]|
//@[02:0003) ├─Token(NewLine) |\n|
var inlinedVariable = zones[0].properties.zoneType
//@[00:0050) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0019) | ├─IdentifierSyntax
//@[04:0019) | | └─Token(Identifier) |inlinedVariable|
//@[20:0021) | ├─Token(Assignment) |=|
//@[22:0050) | └─PropertyAccessSyntax
//@[22:0041) |   ├─PropertyAccessSyntax
//@[22:0030) |   | ├─ArrayAccessSyntax
//@[22:0027) |   | | ├─VariableAccessSyntax
//@[22:0027) |   | | | └─IdentifierSyntax
//@[22:0027) |   | | |   └─Token(Identifier) |zones|
//@[27:0028) |   | | ├─Token(LeftSquare) |[|
//@[28:0029) |   | | ├─IntegerLiteralSyntax
//@[28:0029) |   | | | └─Token(Integer) |0|
//@[29:0030) |   | | └─Token(RightSquare) |]|
//@[30:0031) |   | ├─Token(Dot) |.|
//@[31:0041) |   | └─IdentifierSyntax
//@[31:0041) |   |   └─Token(Identifier) |properties|
//@[41:0042) |   ├─Token(Dot) |.|
//@[42:0050) |   └─IdentifierSyntax
//@[42:0050) |     └─Token(Identifier) |zoneType|
//@[50:0052) ├─Token(NewLine) |\n\n|

var runtimeLoop3 = [for (zone, i) in zoneInput: {
//@[00:0073) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0016) | ├─IdentifierSyntax
//@[04:0016) | | └─Token(Identifier) |runtimeLoop3|
//@[17:0018) | ├─Token(Assignment) |=|
//@[19:0073) | └─ForSyntax
//@[19:0020) |   ├─Token(LeftSquare) |[|
//@[20:0023) |   ├─Token(Identifier) |for|
//@[24:0033) |   ├─VariableBlockSyntax
//@[24:0025) |   | ├─Token(LeftParen) |(|
//@[25:0029) |   | ├─LocalVariableSyntax
//@[25:0029) |   | | └─IdentifierSyntax
//@[25:0029) |   | |   └─Token(Identifier) |zone|
//@[29:0030) |   | ├─Token(Comma) |,|
//@[31:0032) |   | ├─LocalVariableSyntax
//@[31:0032) |   | | └─IdentifierSyntax
//@[31:0032) |   | |   └─Token(Identifier) |i|
//@[32:0033) |   | └─Token(RightParen) |)|
//@[34:0036) |   ├─Token(Identifier) |in|
//@[37:0046) |   ├─VariableAccessSyntax
//@[37:0046) |   | └─IdentifierSyntax
//@[37:0046) |   |   └─Token(Identifier) |zoneInput|
//@[46:0047) |   ├─Token(Colon) |:|
//@[48:0072) |   ├─ObjectSyntax
//@[48:0049) |   | ├─Token(LeftBrace) |{|
//@[49:0050) |   | ├─Token(NewLine) |\n|
  a: inlinedVariable
//@[02:0020) |   | ├─ObjectPropertySyntax
//@[02:0003) |   | | ├─IdentifierSyntax
//@[02:0003) |   | | | └─Token(Identifier) |a|
//@[03:0004) |   | | ├─Token(Colon) |:|
//@[05:0020) |   | | └─VariableAccessSyntax
//@[05:0020) |   | |   └─IdentifierSyntax
//@[05:0020) |   | |     └─Token(Identifier) |inlinedVariable|
//@[20:0021) |   | ├─Token(NewLine) |\n|
}]
//@[00:0001) |   | └─Token(RightBrace) |}|
//@[01:0002) |   └─Token(RightSquare) |]|
//@[02:0004) ├─Token(NewLine) |\n\n|

var runtimeLoop4 = [for (zone, i) in zones[0].properties.registrationVirtualNetworks: {
//@[00:0097) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0016) | ├─IdentifierSyntax
//@[04:0016) | | └─Token(Identifier) |runtimeLoop4|
//@[17:0018) | ├─Token(Assignment) |=|
//@[19:0097) | └─ForSyntax
//@[19:0020) |   ├─Token(LeftSquare) |[|
//@[20:0023) |   ├─Token(Identifier) |for|
//@[24:0033) |   ├─VariableBlockSyntax
//@[24:0025) |   | ├─Token(LeftParen) |(|
//@[25:0029) |   | ├─LocalVariableSyntax
//@[25:0029) |   | | └─IdentifierSyntax
//@[25:0029) |   | |   └─Token(Identifier) |zone|
//@[29:0030) |   | ├─Token(Comma) |,|
//@[31:0032) |   | ├─LocalVariableSyntax
//@[31:0032) |   | | └─IdentifierSyntax
//@[31:0032) |   | |   └─Token(Identifier) |i|
//@[32:0033) |   | └─Token(RightParen) |)|
//@[34:0036) |   ├─Token(Identifier) |in|
//@[37:0084) |   ├─PropertyAccessSyntax
//@[37:0056) |   | ├─PropertyAccessSyntax
//@[37:0045) |   | | ├─ArrayAccessSyntax
//@[37:0042) |   | | | ├─VariableAccessSyntax
//@[37:0042) |   | | | | └─IdentifierSyntax
//@[37:0042) |   | | | |   └─Token(Identifier) |zones|
//@[42:0043) |   | | | ├─Token(LeftSquare) |[|
//@[43:0044) |   | | | ├─IntegerLiteralSyntax
//@[43:0044) |   | | | | └─Token(Integer) |0|
//@[44:0045) |   | | | └─Token(RightSquare) |]|
//@[45:0046) |   | | ├─Token(Dot) |.|
//@[46:0056) |   | | └─IdentifierSyntax
//@[46:0056) |   | |   └─Token(Identifier) |properties|
//@[56:0057) |   | ├─Token(Dot) |.|
//@[57:0084) |   | └─IdentifierSyntax
//@[57:0084) |   |   └─Token(Identifier) |registrationVirtualNetworks|
//@[84:0085) |   ├─Token(Colon) |:|
//@[86:0096) |   ├─ObjectSyntax
//@[86:0087) |   | ├─Token(LeftBrace) |{|
//@[87:0088) |   | ├─Token(NewLine) |\n|
  a: 0
//@[02:0006) |   | ├─ObjectPropertySyntax
//@[02:0003) |   | | ├─IdentifierSyntax
//@[02:0003) |   | | | └─Token(Identifier) |a|
//@[03:0004) |   | | ├─Token(Colon) |:|
//@[05:0006) |   | | └─IntegerLiteralSyntax
//@[05:0006) |   | |   └─Token(Integer) |0|
//@[06:0007) |   | ├─Token(NewLine) |\n|
}]
//@[00:0001) |   | └─Token(RightBrace) |}|
//@[01:0002) |   └─Token(RightSquare) |]|
//@[02:0004) ├─Token(NewLine) |\n\n|

var notRuntime = concat('a','b')
//@[00:0032) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0014) | ├─IdentifierSyntax
//@[04:0014) | | └─Token(Identifier) |notRuntime|
//@[15:0016) | ├─Token(Assignment) |=|
//@[17:0032) | └─FunctionCallSyntax
//@[17:0023) |   ├─IdentifierSyntax
//@[17:0023) |   | └─Token(Identifier) |concat|
//@[23:0024) |   ├─Token(LeftParen) |(|
//@[24:0027) |   ├─FunctionArgumentSyntax
//@[24:0027) |   | └─StringSyntax
//@[24:0027) |   |   └─Token(StringComplete) |'a'|
//@[27:0028) |   ├─Token(Comma) |,|
//@[28:0031) |   ├─FunctionArgumentSyntax
//@[28:0031) |   | └─StringSyntax
//@[28:0031) |   |   └─Token(StringComplete) |'b'|
//@[31:0032) |   └─Token(RightParen) |)|
//@[32:0033) ├─Token(NewLine) |\n|
var evenMoreIndirection = concat(notRuntime, string(moreIndirection))
//@[00:0069) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0023) | ├─IdentifierSyntax
//@[04:0023) | | └─Token(Identifier) |evenMoreIndirection|
//@[24:0025) | ├─Token(Assignment) |=|
//@[26:0069) | └─FunctionCallSyntax
//@[26:0032) |   ├─IdentifierSyntax
//@[26:0032) |   | └─Token(Identifier) |concat|
//@[32:0033) |   ├─Token(LeftParen) |(|
//@[33:0043) |   ├─FunctionArgumentSyntax
//@[33:0043) |   | └─VariableAccessSyntax
//@[33:0043) |   |   └─IdentifierSyntax
//@[33:0043) |   |     └─Token(Identifier) |notRuntime|
//@[43:0044) |   ├─Token(Comma) |,|
//@[45:0068) |   ├─FunctionArgumentSyntax
//@[45:0068) |   | └─FunctionCallSyntax
//@[45:0051) |   |   ├─IdentifierSyntax
//@[45:0051) |   |   | └─Token(Identifier) |string|
//@[51:0052) |   |   ├─Token(LeftParen) |(|
//@[52:0067) |   |   ├─FunctionArgumentSyntax
//@[52:0067) |   |   | └─VariableAccessSyntax
//@[52:0067) |   |   |   └─IdentifierSyntax
//@[52:0067) |   |   |     └─Token(Identifier) |moreIndirection|
//@[67:0068) |   |   └─Token(RightParen) |)|
//@[68:0069) |   └─Token(RightParen) |)|
//@[69:0070) ├─Token(NewLine) |\n|
var moreIndirection = reference('s','s', 'Full')
//@[00:0048) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0019) | ├─IdentifierSyntax
//@[04:0019) | | └─Token(Identifier) |moreIndirection|
//@[20:0021) | ├─Token(Assignment) |=|
//@[22:0048) | └─FunctionCallSyntax
//@[22:0031) |   ├─IdentifierSyntax
//@[22:0031) |   | └─Token(Identifier) |reference|
//@[31:0032) |   ├─Token(LeftParen) |(|
//@[32:0035) |   ├─FunctionArgumentSyntax
//@[32:0035) |   | └─StringSyntax
//@[32:0035) |   |   └─Token(StringComplete) |'s'|
//@[35:0036) |   ├─Token(Comma) |,|
//@[36:0039) |   ├─FunctionArgumentSyntax
//@[36:0039) |   | └─StringSyntax
//@[36:0039) |   |   └─Token(StringComplete) |'s'|
//@[39:0040) |   ├─Token(Comma) |,|
//@[41:0047) |   ├─FunctionArgumentSyntax
//@[41:0047) |   | └─StringSyntax
//@[41:0047) |   |   └─Token(StringComplete) |'Full'|
//@[47:0048) |   └─Token(RightParen) |)|
//@[48:0050) ├─Token(NewLine) |\n\n|

var myRef = [
//@[00:0037) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0009) | ├─IdentifierSyntax
//@[04:0009) | | └─Token(Identifier) |myRef|
//@[10:0011) | ├─Token(Assignment) |=|
//@[12:0037) | └─ArraySyntax
//@[12:0013) |   ├─Token(LeftSquare) |[|
//@[13:0014) |   ├─Token(NewLine) |\n|
  evenMoreIndirection
//@[02:0021) |   ├─ArrayItemSyntax
//@[02:0021) |   | └─VariableAccessSyntax
//@[02:0021) |   |   └─IdentifierSyntax
//@[02:0021) |   |     └─Token(Identifier) |evenMoreIndirection|
//@[21:0022) |   ├─Token(NewLine) |\n|
]
//@[00:0001) |   └─Token(RightSquare) |]|
//@[01:0002) ├─Token(NewLine) |\n|
var runtimeLoop5 = [for (item, index) in myRef: 's']
//@[00:0052) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0016) | ├─IdentifierSyntax
//@[04:0016) | | └─Token(Identifier) |runtimeLoop5|
//@[17:0018) | ├─Token(Assignment) |=|
//@[19:0052) | └─ForSyntax
//@[19:0020) |   ├─Token(LeftSquare) |[|
//@[20:0023) |   ├─Token(Identifier) |for|
//@[24:0037) |   ├─VariableBlockSyntax
//@[24:0025) |   | ├─Token(LeftParen) |(|
//@[25:0029) |   | ├─LocalVariableSyntax
//@[25:0029) |   | | └─IdentifierSyntax
//@[25:0029) |   | |   └─Token(Identifier) |item|
//@[29:0030) |   | ├─Token(Comma) |,|
//@[31:0036) |   | ├─LocalVariableSyntax
//@[31:0036) |   | | └─IdentifierSyntax
//@[31:0036) |   | |   └─Token(Identifier) |index|
//@[36:0037) |   | └─Token(RightParen) |)|
//@[38:0040) |   ├─Token(Identifier) |in|
//@[41:0046) |   ├─VariableAccessSyntax
//@[41:0046) |   | └─IdentifierSyntax
//@[41:0046) |   |   └─Token(Identifier) |myRef|
//@[46:0047) |   ├─Token(Colon) |:|
//@[48:0051) |   ├─StringSyntax
//@[48:0051) |   | └─Token(StringComplete) |'s'|
//@[51:0052) |   └─Token(RightSquare) |]|
//@[52:0054) ├─Token(NewLine) |\n\n|

// cannot use loops in expressions
//@[34:0035) ├─Token(NewLine) |\n|
var loopExpression = union([for thing in stuff: 4], [for thing in stuff: true])
//@[00:0079) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0018) | ├─IdentifierSyntax
//@[04:0018) | | └─Token(Identifier) |loopExpression|
//@[19:0020) | ├─Token(Assignment) |=|
//@[21:0079) | └─FunctionCallSyntax
//@[21:0026) |   ├─IdentifierSyntax
//@[21:0026) |   | └─Token(Identifier) |union|
//@[26:0027) |   ├─Token(LeftParen) |(|
//@[27:0050) |   ├─FunctionArgumentSyntax
//@[27:0050) |   | └─ForSyntax
//@[27:0028) |   |   ├─Token(LeftSquare) |[|
//@[28:0031) |   |   ├─Token(Identifier) |for|
//@[32:0037) |   |   ├─LocalVariableSyntax
//@[32:0037) |   |   | └─IdentifierSyntax
//@[32:0037) |   |   |   └─Token(Identifier) |thing|
//@[38:0040) |   |   ├─Token(Identifier) |in|
//@[41:0046) |   |   ├─VariableAccessSyntax
//@[41:0046) |   |   | └─IdentifierSyntax
//@[41:0046) |   |   |   └─Token(Identifier) |stuff|
//@[46:0047) |   |   ├─Token(Colon) |:|
//@[48:0049) |   |   ├─IntegerLiteralSyntax
//@[48:0049) |   |   | └─Token(Integer) |4|
//@[49:0050) |   |   └─Token(RightSquare) |]|
//@[50:0051) |   ├─Token(Comma) |,|
//@[52:0078) |   ├─FunctionArgumentSyntax
//@[52:0078) |   | └─ForSyntax
//@[52:0053) |   |   ├─Token(LeftSquare) |[|
//@[53:0056) |   |   ├─Token(Identifier) |for|
//@[57:0062) |   |   ├─LocalVariableSyntax
//@[57:0062) |   |   | └─IdentifierSyntax
//@[57:0062) |   |   |   └─Token(Identifier) |thing|
//@[63:0065) |   |   ├─Token(Identifier) |in|
//@[66:0071) |   |   ├─VariableAccessSyntax
//@[66:0071) |   |   | └─IdentifierSyntax
//@[66:0071) |   |   |   └─Token(Identifier) |stuff|
//@[71:0072) |   |   ├─Token(Colon) |:|
//@[73:0077) |   |   ├─BooleanLiteralSyntax
//@[73:0077) |   |   | └─Token(TrueKeyword) |true|
//@[77:0078) |   |   └─Token(RightSquare) |]|
//@[78:0079) |   └─Token(RightParen) |)|
//@[79:0081) ├─Token(NewLine) |\n\n|

@batchSize(1)
//@[00:0051) ├─VariableDeclarationSyntax
//@[00:0013) | ├─DecoratorSyntax
//@[00:0001) | | ├─Token(At) |@|
//@[01:0013) | | └─FunctionCallSyntax
//@[01:0010) | |   ├─IdentifierSyntax
//@[01:0010) | |   | └─Token(Identifier) |batchSize|
//@[10:0011) | |   ├─Token(LeftParen) |(|
//@[11:0012) | |   ├─FunctionArgumentSyntax
//@[11:0012) | |   | └─IntegerLiteralSyntax
//@[11:0012) | |   |   └─Token(Integer) |1|
//@[12:0013) | |   └─Token(RightParen) |)|
//@[13:0014) | ├─Token(NewLine) |\n|
var batchSizeMakesNoSenseHere = false
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0029) | ├─IdentifierSyntax
//@[04:0029) | | └─Token(Identifier) |batchSizeMakesNoSenseHere|
//@[30:0031) | ├─Token(Assignment) |=|
//@[32:0037) | └─BooleanLiteralSyntax
//@[32:0037) |   └─Token(FalseKeyword) |false|
//@[37:0040) ├─Token(NewLine) |\n\n\n|


//KeyVault Secret Reference
//@[27:0028) ├─Token(NewLine) |\n|
resource kv 'Microsoft.KeyVault/vaults@2019-09-01' existing = {
//@[00:0088) ├─ResourceDeclarationSyntax
//@[00:0008) | ├─Token(Identifier) |resource|
//@[09:0011) | ├─IdentifierSyntax
//@[09:0011) | | └─Token(Identifier) |kv|
//@[12:0050) | ├─StringSyntax
//@[12:0050) | | └─Token(StringComplete) |'Microsoft.KeyVault/vaults@2019-09-01'|
//@[51:0059) | ├─Token(Identifier) |existing|
//@[60:0061) | ├─Token(Assignment) |=|
//@[62:0088) | └─ObjectSyntax
//@[62:0063) |   ├─Token(LeftBrace) |{|
//@[63:0064) |   ├─Token(NewLine) |\n|
  name: 'testkeyvault'
//@[02:0022) |   ├─ObjectPropertySyntax
//@[02:0006) |   | ├─IdentifierSyntax
//@[02:0006) |   | | └─Token(Identifier) |name|
//@[06:0007) |   | ├─Token(Colon) |:|
//@[08:0022) |   | └─StringSyntax
//@[08:0022) |   |   └─Token(StringComplete) |'testkeyvault'|
//@[22:0023) |   ├─Token(NewLine) |\n|
}
//@[00:0001) |   └─Token(RightBrace) |}|
//@[01:0003) ├─Token(NewLine) |\n\n|

var keyVaultSecretVar = kv.getSecret('mySecret')
//@[00:0048) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0021) | ├─IdentifierSyntax
//@[04:0021) | | └─Token(Identifier) |keyVaultSecretVar|
//@[22:0023) | ├─Token(Assignment) |=|
//@[24:0048) | └─InstanceFunctionCallSyntax
//@[24:0026) |   ├─VariableAccessSyntax
//@[24:0026) |   | └─IdentifierSyntax
//@[24:0026) |   |   └─Token(Identifier) |kv|
//@[26:0027) |   ├─Token(Dot) |.|
//@[27:0036) |   ├─IdentifierSyntax
//@[27:0036) |   | └─Token(Identifier) |getSecret|
//@[36:0037) |   ├─Token(LeftParen) |(|
//@[37:0047) |   ├─FunctionArgumentSyntax
//@[37:0047) |   | └─StringSyntax
//@[37:0047) |   |   └─Token(StringComplete) |'mySecret'|
//@[47:0048) |   └─Token(RightParen) |)|
//@[48:0049) ├─Token(NewLine) |\n|
var keyVaultSecretInterpolatedVar = '${kv.getSecret('mySecret')}'
//@[00:0065) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0033) | ├─IdentifierSyntax
//@[04:0033) | | └─Token(Identifier) |keyVaultSecretInterpolatedVar|
//@[34:0035) | ├─Token(Assignment) |=|
//@[36:0065) | └─StringSyntax
//@[36:0039) |   ├─Token(StringLeftPiece) |'${|
//@[39:0063) |   ├─InstanceFunctionCallSyntax
//@[39:0041) |   | ├─VariableAccessSyntax
//@[39:0041) |   | | └─IdentifierSyntax
//@[39:0041) |   | |   └─Token(Identifier) |kv|
//@[41:0042) |   | ├─Token(Dot) |.|
//@[42:0051) |   | ├─IdentifierSyntax
//@[42:0051) |   | | └─Token(Identifier) |getSecret|
//@[51:0052) |   | ├─Token(LeftParen) |(|
//@[52:0062) |   | ├─FunctionArgumentSyntax
//@[52:0062) |   | | └─StringSyntax
//@[52:0062) |   | |   └─Token(StringComplete) |'mySecret'|
//@[62:0063) |   | └─Token(RightParen) |)|
//@[63:0065) |   └─Token(StringRightPiece) |}'|
//@[65:0066) ├─Token(NewLine) |\n|
var keyVaultSecretObjectVar = {
//@[00:0068) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0027) | ├─IdentifierSyntax
//@[04:0027) | | └─Token(Identifier) |keyVaultSecretObjectVar|
//@[28:0029) | ├─Token(Assignment) |=|
//@[30:0068) | └─ObjectSyntax
//@[30:0031) |   ├─Token(LeftBrace) |{|
//@[31:0032) |   ├─Token(NewLine) |\n|
  secret: kv.getSecret('mySecret')
//@[02:0034) |   ├─ObjectPropertySyntax
//@[02:0008) |   | ├─IdentifierSyntax
//@[02:0008) |   | | └─Token(Identifier) |secret|
//@[08:0009) |   | ├─Token(Colon) |:|
//@[10:0034) |   | └─InstanceFunctionCallSyntax
//@[10:0012) |   |   ├─VariableAccessSyntax
//@[10:0012) |   |   | └─IdentifierSyntax
//@[10:0012) |   |   |   └─Token(Identifier) |kv|
//@[12:0013) |   |   ├─Token(Dot) |.|
//@[13:0022) |   |   ├─IdentifierSyntax
//@[13:0022) |   |   | └─Token(Identifier) |getSecret|
//@[22:0023) |   |   ├─Token(LeftParen) |(|
//@[23:0033) |   |   ├─FunctionArgumentSyntax
//@[23:0033) |   |   | └─StringSyntax
//@[23:0033) |   |   |   └─Token(StringComplete) |'mySecret'|
//@[33:0034) |   |   └─Token(RightParen) |)|
//@[34:0035) |   ├─Token(NewLine) |\n|
}
//@[00:0001) |   └─Token(RightBrace) |}|
//@[01:0002) ├─Token(NewLine) |\n|
var keyVaultSecretArrayVar = [
//@[00:0059) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0026) | ├─IdentifierSyntax
//@[04:0026) | | └─Token(Identifier) |keyVaultSecretArrayVar|
//@[27:0028) | ├─Token(Assignment) |=|
//@[29:0059) | └─ArraySyntax
//@[29:0030) |   ├─Token(LeftSquare) |[|
//@[30:0031) |   ├─Token(NewLine) |\n|
  kv.getSecret('mySecret')
//@[02:0026) |   ├─ArrayItemSyntax
//@[02:0026) |   | └─InstanceFunctionCallSyntax
//@[02:0004) |   |   ├─VariableAccessSyntax
//@[02:0004) |   |   | └─IdentifierSyntax
//@[02:0004) |   |   |   └─Token(Identifier) |kv|
//@[04:0005) |   |   ├─Token(Dot) |.|
//@[05:0014) |   |   ├─IdentifierSyntax
//@[05:0014) |   |   | └─Token(Identifier) |getSecret|
//@[14:0015) |   |   ├─Token(LeftParen) |(|
//@[15:0025) |   |   ├─FunctionArgumentSyntax
//@[15:0025) |   |   | └─StringSyntax
//@[15:0025) |   |   |   └─Token(StringComplete) |'mySecret'|
//@[25:0026) |   |   └─Token(RightParen) |)|
//@[26:0027) |   ├─Token(NewLine) |\n|
]
//@[00:0001) |   └─Token(RightSquare) |]|
//@[01:0002) ├─Token(NewLine) |\n|
var keyVaultSecretArrayInterpolatedVar = [
//@[00:0076) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0038) | ├─IdentifierSyntax
//@[04:0038) | | └─Token(Identifier) |keyVaultSecretArrayInterpolatedVar|
//@[39:0040) | ├─Token(Assignment) |=|
//@[41:0076) | └─ArraySyntax
//@[41:0042) |   ├─Token(LeftSquare) |[|
//@[42:0043) |   ├─Token(NewLine) |\n|
  '${kv.getSecret('mySecret')}'
//@[02:0031) |   ├─ArrayItemSyntax
//@[02:0031) |   | └─StringSyntax
//@[02:0005) |   |   ├─Token(StringLeftPiece) |'${|
//@[05:0029) |   |   ├─InstanceFunctionCallSyntax
//@[05:0007) |   |   | ├─VariableAccessSyntax
//@[05:0007) |   |   | | └─IdentifierSyntax
//@[05:0007) |   |   | |   └─Token(Identifier) |kv|
//@[07:0008) |   |   | ├─Token(Dot) |.|
//@[08:0017) |   |   | ├─IdentifierSyntax
//@[08:0017) |   |   | | └─Token(Identifier) |getSecret|
//@[17:0018) |   |   | ├─Token(LeftParen) |(|
//@[18:0028) |   |   | ├─FunctionArgumentSyntax
//@[18:0028) |   |   | | └─StringSyntax
//@[18:0028) |   |   | |   └─Token(StringComplete) |'mySecret'|
//@[28:0029) |   |   | └─Token(RightParen) |)|
//@[29:0031) |   |   └─Token(StringRightPiece) |}'|
//@[31:0032) |   ├─Token(NewLine) |\n|
]
//@[00:0001) |   └─Token(RightSquare) |]|
//@[01:0003) ├─Token(NewLine) |\n\n|

var listSecrets= ''
//@[00:0019) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0015) | ├─IdentifierSyntax
//@[04:0015) | | └─Token(Identifier) |listSecrets|
//@[15:0016) | ├─Token(Assignment) |=|
//@[17:0019) | └─StringSyntax
//@[17:0019) |   └─Token(StringComplete) |''|
//@[19:0020) ├─Token(NewLine) |\n|
var listSecretsVar = listSecrets()
//@[00:0034) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0018) | ├─IdentifierSyntax
//@[04:0018) | | └─Token(Identifier) |listSecretsVar|
//@[19:0020) | ├─Token(Assignment) |=|
//@[21:0034) | └─FunctionCallSyntax
//@[21:0032) |   ├─IdentifierSyntax
//@[21:0032) |   | └─Token(Identifier) |listSecrets|
//@[32:0033) |   ├─Token(LeftParen) |(|
//@[33:0034) |   └─Token(RightParen) |)|
//@[34:0036) ├─Token(NewLine) |\n\n|

var copy = [
//@[00:0082) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0008) | ├─IdentifierSyntax
//@[04:0008) | | └─Token(Identifier) |copy|
//@[09:0010) | ├─Token(Assignment) |=|
//@[11:0082) | └─ArraySyntax
//@[11:0012) |   ├─Token(LeftSquare) |[|
//@[12:0013) |   ├─Token(NewLine) |\n|
  {
//@[02:0067) |   ├─ArrayItemSyntax
//@[02:0067) |   | └─ObjectSyntax
//@[02:0003) |   |   ├─Token(LeftBrace) |{|
//@[03:0004) |   |   ├─Token(NewLine) |\n|
    name: 'one'
//@[04:0015) |   |   ├─ObjectPropertySyntax
//@[04:0008) |   |   | ├─IdentifierSyntax
//@[04:0008) |   |   | | └─Token(Identifier) |name|
//@[08:0009) |   |   | ├─Token(Colon) |:|
//@[10:0015) |   |   | └─StringSyntax
//@[10:0015) |   |   |   └─Token(StringComplete) |'one'|
//@[15:0016) |   |   ├─Token(NewLine) |\n|
    count: '[notAFunction()]'
//@[04:0029) |   |   ├─ObjectPropertySyntax
//@[04:0009) |   |   | ├─IdentifierSyntax
//@[04:0009) |   |   | | └─Token(Identifier) |count|
//@[09:0010) |   |   | ├─Token(Colon) |:|
//@[11:0029) |   |   | └─StringSyntax
//@[11:0029) |   |   |   └─Token(StringComplete) |'[notAFunction()]'|
//@[29:0030) |   |   ├─Token(NewLine) |\n|
    input: {}
//@[04:0013) |   |   ├─ObjectPropertySyntax
//@[04:0009) |   |   | ├─IdentifierSyntax
//@[04:0009) |   |   | | └─Token(Identifier) |input|
//@[09:0010) |   |   | ├─Token(Colon) |:|
//@[11:0013) |   |   | └─ObjectSyntax
//@[11:0012) |   |   |   ├─Token(LeftBrace) |{|
//@[12:0013) |   |   |   └─Token(RightBrace) |}|
//@[13:0014) |   |   ├─Token(NewLine) |\n|
  }
//@[02:0003) |   |   └─Token(RightBrace) |}|
//@[03:0004) |   ├─Token(NewLine) |\n|
]
//@[00:0001) |   └─Token(RightSquare) |]|
//@[01:0002) ├─Token(NewLine) |\n|

//@[00:0000) └─Token(EndOfFile) ||
