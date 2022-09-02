using './main.bicep'
//@[00:197) ProgramSyntax
//@[00:020) ├─UsingDeclarationSyntax
//@[00:005) | ├─Token(Identifier) |using|
//@[06:020) | └─StringSyntax
//@[06:020) |   └─Token(StringComplete) |'./main.bicep'|
//@[20:022) ├─Token(NewLine) |\n\n|

param string = 123
//@[00:018) ├─ParameterAssignmentSyntax
//@[00:005) | ├─Token(Identifier) |param|
//@[06:012) | ├─IdentifierSyntax
//@[06:012) | | └─Token(Identifier) |string|
//@[13:014) | ├─Token(Assignment) |=|
//@[15:018) | └─IntegerLiteralSyntax
//@[15:018) |   └─Token(Integer) |123|
//@[18:020) ├─Token(NewLine) |\n\n|

param bool = 'hello'
//@[00:020) ├─ParameterAssignmentSyntax
//@[00:005) | ├─Token(Identifier) |param|
//@[06:010) | ├─IdentifierSyntax
//@[06:010) | | └─Token(Identifier) |bool|
//@[11:012) | ├─Token(Assignment) |=|
//@[13:020) | └─StringSyntax
//@[13:020) |   └─Token(StringComplete) |'hello'|
//@[20:022) ├─Token(NewLine) |\n\n|

param int = false
//@[00:017) ├─ParameterAssignmentSyntax
//@[00:005) | ├─Token(Identifier) |param|
//@[06:009) | ├─IdentifierSyntax
//@[06:009) | | └─Token(Identifier) |int|
//@[10:011) | ├─Token(Assignment) |=|
//@[12:017) | └─BooleanLiteralSyntax
//@[12:017) |   └─Token(FalseKeyword) |false|
//@[17:019) ├─Token(NewLine) |\n\n|

param object = ['abc', 'def']
//@[00:029) ├─ParameterAssignmentSyntax
//@[00:005) | ├─Token(Identifier) |param|
//@[06:012) | ├─IdentifierSyntax
//@[06:012) | | └─Token(Identifier) |object|
//@[13:014) | ├─Token(Assignment) |=|
//@[15:029) | └─ArraySyntax
//@[15:016) |   ├─Token(LeftSquare) |[|
//@[16:021) |   ├─ArrayItemSyntax
//@[16:021) |   | └─StringSyntax
//@[16:021) |   |   └─Token(StringComplete) |'abc'|
//@[21:022) |   ├─Token(Comma) |,|
//@[23:028) |   ├─ArrayItemSyntax
//@[23:028) |   | └─StringSyntax
//@[23:028) |   |   └─Token(StringComplete) |'def'|
//@[28:029) |   └─Token(RightSquare) |]|
//@[29:031) ├─Token(NewLine) |\n\n|

param array = {
//@[00:038) ├─ParameterAssignmentSyntax
//@[00:005) | ├─Token(Identifier) |param|
//@[06:011) | ├─IdentifierSyntax
//@[06:011) | | └─Token(Identifier) |array|
//@[12:013) | ├─Token(Assignment) |=|
//@[14:038) | └─ObjectSyntax
//@[14:015) |   ├─Token(LeftBrace) |{|
//@[15:016) |   ├─Token(NewLine) |\n|
  isThis: 'correct?'
//@[02:020) |   ├─ObjectPropertySyntax
//@[02:008) |   | ├─IdentifierSyntax
//@[02:008) |   | | └─Token(Identifier) |isThis|
//@[08:009) |   | ├─Token(Colon) |:|
//@[10:020) |   | └─StringSyntax
//@[10:020) |   |   └─Token(StringComplete) |'correct?'|
//@[20:021) |   ├─Token(NewLine) |\n|
}
//@[00:001) |   └─Token(RightBrace) |}|
//@[01:003) ├─Token(NewLine) |\n\n|

param stringAllowed = 'notTheAllowedValue'
//@[00:042) ├─ParameterAssignmentSyntax
//@[00:005) | ├─Token(Identifier) |param|
//@[06:019) | ├─IdentifierSyntax
//@[06:019) | | └─Token(Identifier) |stringAllowed|
//@[20:021) | ├─Token(Assignment) |=|
//@[22:042) | └─StringSyntax
//@[22:042) |   └─Token(StringComplete) |'notTheAllowedValue'|
//@[42:043) ├─Token(NewLine) |\n|

//@[00:000) └─Token(EndOfFile) ||
