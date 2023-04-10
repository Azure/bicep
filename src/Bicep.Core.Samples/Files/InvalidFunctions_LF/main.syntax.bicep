func useRuntimeFunction = () => reference('foo').bar
//@[00:195) ProgramSyntax
//@[00:052) ├─FunctionDeclarationSyntax
//@[00:004) | ├─Token(Identifier) |func|
//@[05:023) | ├─IdentifierSyntax
//@[05:023) | | └─Token(Identifier) |useRuntimeFunction|
//@[24:025) | ├─Token(Assignment) |=|
//@[26:052) | └─TypedLambdaSyntax
//@[26:028) |   ├─TypedVariableBlockSyntax
//@[26:027) |   | ├─Token(LeftParen) |(|
//@[27:028) |   | └─Token(RightParen) |)|
//@[29:031) |   ├─Token(Arrow) |=>|
//@[32:052) |   └─PropertyAccessSyntax
//@[32:048) |     ├─FunctionCallSyntax
//@[32:041) |     | ├─IdentifierSyntax
//@[32:041) |     | | └─Token(Identifier) |reference|
//@[41:042) |     | ├─Token(LeftParen) |(|
//@[42:047) |     | ├─FunctionArgumentSyntax
//@[42:047) |     | | └─StringSyntax
//@[42:047) |     | |   └─Token(StringComplete) |'foo'|
//@[47:048) |     | └─Token(RightParen) |)|
//@[48:049) |     ├─Token(Dot) |.|
//@[49:052) |     └─IdentifierSyntax
//@[49:052) |       └─Token(Identifier) |bar|
//@[52:054) ├─Token(NewLine) |\n\n|

func funcA = () => 'A'
//@[00:022) ├─FunctionDeclarationSyntax
//@[00:004) | ├─Token(Identifier) |func|
//@[05:010) | ├─IdentifierSyntax
//@[05:010) | | └─Token(Identifier) |funcA|
//@[11:012) | ├─Token(Assignment) |=|
//@[13:022) | └─TypedLambdaSyntax
//@[13:015) |   ├─TypedVariableBlockSyntax
//@[13:014) |   | ├─Token(LeftParen) |(|
//@[14:015) |   | └─Token(RightParen) |)|
//@[16:018) |   ├─Token(Arrow) |=>|
//@[19:022) |   └─StringSyntax
//@[19:022) |     └─Token(StringComplete) |'A'|
//@[22:023) ├─Token(NewLine) |\n|
func funcB = () => funcA()
//@[00:026) ├─FunctionDeclarationSyntax
//@[00:004) | ├─Token(Identifier) |func|
//@[05:010) | ├─IdentifierSyntax
//@[05:010) | | └─Token(Identifier) |funcB|
//@[11:012) | ├─Token(Assignment) |=|
//@[13:026) | └─TypedLambdaSyntax
//@[13:015) |   ├─TypedVariableBlockSyntax
//@[13:014) |   | ├─Token(LeftParen) |(|
//@[14:015) |   | └─Token(RightParen) |)|
//@[16:018) |   ├─Token(Arrow) |=>|
//@[19:026) |   └─FunctionCallSyntax
//@[19:024) |     ├─IdentifierSyntax
//@[19:024) |     | └─Token(Identifier) |funcA|
//@[24:025) |     ├─Token(LeftParen) |(|
//@[25:026) |     └─Token(RightParen) |)|
//@[26:028) ├─Token(NewLine) |\n\n|

func invalidType = (string input) => input
//@[00:042) ├─FunctionDeclarationSyntax
//@[00:004) | ├─Token(Identifier) |func|
//@[05:016) | ├─IdentifierSyntax
//@[05:016) | | └─Token(Identifier) |invalidType|
//@[17:018) | ├─Token(Assignment) |=|
//@[19:042) | └─TypedLambdaSyntax
//@[19:033) |   ├─TypedVariableBlockSyntax
//@[19:020) |   | ├─Token(LeftParen) |(|
//@[20:032) |   | ├─TypedLocalVariableSyntax
//@[27:032) |   | | ├─IdentifierSyntax
//@[27:032) |   | | | └─Token(Identifier) |input|
//@[20:026) |   | | └─VariableAccessSyntax
//@[20:026) |   | |   └─IdentifierSyntax
//@[20:026) |   | |     └─Token(Identifier) |string|
//@[32:033) |   | └─Token(RightParen) |)|
//@[34:036) |   ├─Token(Arrow) |=>|
//@[37:042) |   └─VariableAccessSyntax
//@[37:042) |     └─IdentifierSyntax
//@[37:042) |       └─Token(Identifier) |input|
//@[42:044) ├─Token(NewLine) |\n\n|

output invalidType string = invalidType(true)
//@[00:045) ├─OutputDeclarationSyntax
//@[00:006) | ├─Token(Identifier) |output|
//@[07:018) | ├─IdentifierSyntax
//@[07:018) | | └─Token(Identifier) |invalidType|
//@[19:025) | ├─VariableAccessSyntax
//@[19:025) | | └─IdentifierSyntax
//@[19:025) | |   └─Token(Identifier) |string|
//@[26:027) | ├─Token(Assignment) |=|
//@[28:045) | └─FunctionCallSyntax
//@[28:039) |   ├─IdentifierSyntax
//@[28:039) |   | └─Token(Identifier) |invalidType|
//@[39:040) |   ├─Token(LeftParen) |(|
//@[40:044) |   ├─FunctionArgumentSyntax
//@[40:044) |   | └─BooleanLiteralSyntax
//@[40:044) |   |   └─Token(TrueKeyword) |true|
//@[44:045) |   └─Token(RightParen) |)|
//@[45:046) ├─Token(NewLine) |\n|

//@[00:000) └─Token(EndOfFile) ||
