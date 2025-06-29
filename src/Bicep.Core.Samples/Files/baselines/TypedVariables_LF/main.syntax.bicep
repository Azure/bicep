@export()
//@[00:304) ProgramSyntax
//@[00:043) ├─VariableDeclarationSyntax
//@[00:009) | ├─DecoratorSyntax
//@[00:001) | | ├─Token(At) |@|
//@[01:009) | | └─FunctionCallSyntax
//@[01:007) | |   ├─IdentifierSyntax
//@[01:007) | |   | └─Token(Identifier) |export|
//@[07:008) | |   ├─Token(LeftParen) |(|
//@[08:009) | |   └─Token(RightParen) |)|
//@[09:010) | ├─Token(NewLine) |\n|
var exportedString string = 'foo'
//@[00:003) | ├─Token(Identifier) |var|
//@[04:018) | ├─IdentifierSyntax
//@[04:018) | | └─Token(Identifier) |exportedString|
//@[19:025) | ├─TypeVariableAccessSyntax
//@[19:025) | | └─IdentifierSyntax
//@[19:025) | |   └─Token(Identifier) |string|
//@[26:027) | ├─Token(Assignment) |=|
//@[28:033) | └─StringSyntax
//@[28:033) |   └─Token(StringComplete) |'foo'|
//@[33:035) ├─Token(NewLine) |\n\n|

@export()
//@[00:090) ├─VariableDeclarationSyntax
//@[00:009) | ├─DecoratorSyntax
//@[00:001) | | ├─Token(At) |@|
//@[01:009) | | └─FunctionCallSyntax
//@[01:007) | |   ├─IdentifierSyntax
//@[01:007) | |   | └─Token(Identifier) |export|
//@[07:008) | |   ├─Token(LeftParen) |(|
//@[08:009) | |   └─Token(RightParen) |)|
//@[09:010) | ├─Token(NewLine) |\n|
var exporteInlineType {
//@[00:003) | ├─Token(Identifier) |var|
//@[04:021) | ├─IdentifierSyntax
//@[04:021) | | └─Token(Identifier) |exporteInlineType|
//@[22:050) | ├─ObjectTypeSyntax
//@[22:023) | | ├─Token(LeftBrace) |{|
//@[23:024) | | ├─Token(NewLine) |\n|
  foo: string
//@[02:013) | | ├─ObjectTypePropertySyntax
//@[02:005) | | | ├─IdentifierSyntax
//@[02:005) | | | | └─Token(Identifier) |foo|
//@[05:006) | | | ├─Token(Colon) |:|
//@[07:013) | | | └─TypeVariableAccessSyntax
//@[07:013) | | |   └─IdentifierSyntax
//@[07:013) | | |     └─Token(Identifier) |string|
//@[13:014) | | ├─Token(NewLine) |\n|
  bar: int
//@[02:010) | | ├─ObjectTypePropertySyntax
//@[02:005) | | | ├─IdentifierSyntax
//@[02:005) | | | | └─Token(Identifier) |bar|
//@[05:006) | | | ├─Token(Colon) |:|
//@[07:010) | | | └─TypeVariableAccessSyntax
//@[07:010) | | |   └─IdentifierSyntax
//@[07:010) | | |     └─Token(Identifier) |int|
//@[10:011) | | ├─Token(NewLine) |\n|
} = {
//@[00:001) | | └─Token(RightBrace) |}|
//@[02:003) | ├─Token(Assignment) |=|
//@[04:031) | └─ObjectSyntax
//@[04:005) |   ├─Token(LeftBrace) |{|
//@[05:006) |   ├─Token(NewLine) |\n|
  foo: 'abc'
//@[02:012) |   ├─ObjectPropertySyntax
//@[02:005) |   | ├─IdentifierSyntax
//@[02:005) |   | | └─Token(Identifier) |foo|
//@[05:006) |   | ├─Token(Colon) |:|
//@[07:012) |   | └─StringSyntax
//@[07:012) |   |   └─Token(StringComplete) |'abc'|
//@[12:013) |   ├─Token(NewLine) |\n|
  bar: 123
//@[02:010) |   ├─ObjectPropertySyntax
//@[02:005) |   | ├─IdentifierSyntax
//@[02:005) |   | | └─Token(Identifier) |bar|
//@[05:006) |   | ├─Token(Colon) |:|
//@[07:010) |   | └─IntegerLiteralSyntax
//@[07:010) |   |   └─Token(Integer) |123|
//@[10:011) |   ├─Token(NewLine) |\n|
}
//@[00:001) |   └─Token(RightBrace) |}|
//@[01:003) ├─Token(NewLine) |\n\n|

type FooType = {
//@[00:043) ├─TypeDeclarationSyntax
//@[00:004) | ├─Token(Identifier) |type|
//@[05:012) | ├─IdentifierSyntax
//@[05:012) | | └─Token(Identifier) |FooType|
//@[13:014) | ├─Token(Assignment) |=|
//@[15:043) | └─ObjectTypeSyntax
//@[15:016) |   ├─Token(LeftBrace) |{|
//@[16:017) |   ├─Token(NewLine) |\n|
  foo: string
//@[02:013) |   ├─ObjectTypePropertySyntax
//@[02:005) |   | ├─IdentifierSyntax
//@[02:005) |   | | └─Token(Identifier) |foo|
//@[05:006) |   | ├─Token(Colon) |:|
//@[07:013) |   | └─TypeVariableAccessSyntax
//@[07:013) |   |   └─IdentifierSyntax
//@[07:013) |   |     └─Token(Identifier) |string|
//@[13:014) |   ├─Token(NewLine) |\n|
  bar: int
//@[02:010) |   ├─ObjectTypePropertySyntax
//@[02:005) |   | ├─IdentifierSyntax
//@[02:005) |   | | └─Token(Identifier) |bar|
//@[05:006) |   | ├─Token(Colon) |:|
//@[07:010) |   | └─TypeVariableAccessSyntax
//@[07:010) |   |   └─IdentifierSyntax
//@[07:010) |   |     └─Token(Identifier) |int|
//@[10:011) |   ├─Token(NewLine) |\n|
}
//@[00:001) |   └─Token(RightBrace) |}|
//@[01:003) ├─Token(NewLine) |\n\n|

@export()
//@[00:067) ├─VariableDeclarationSyntax
//@[00:009) | ├─DecoratorSyntax
//@[00:001) | | ├─Token(At) |@|
//@[01:009) | | └─FunctionCallSyntax
//@[01:007) | |   ├─IdentifierSyntax
//@[01:007) | |   | └─Token(Identifier) |export|
//@[07:008) | |   ├─Token(LeftParen) |(|
//@[08:009) | |   └─Token(RightParen) |)|
//@[09:010) | ├─Token(NewLine) |\n|
var exportedTypeRef FooType = {
//@[00:003) | ├─Token(Identifier) |var|
//@[04:019) | ├─IdentifierSyntax
//@[04:019) | | └─Token(Identifier) |exportedTypeRef|
//@[20:027) | ├─TypeVariableAccessSyntax
//@[20:027) | | └─IdentifierSyntax
//@[20:027) | |   └─Token(Identifier) |FooType|
//@[28:029) | ├─Token(Assignment) |=|
//@[30:057) | └─ObjectSyntax
//@[30:031) |   ├─Token(LeftBrace) |{|
//@[31:032) |   ├─Token(NewLine) |\n|
  foo: 'abc'
//@[02:012) |   ├─ObjectPropertySyntax
//@[02:005) |   | ├─IdentifierSyntax
//@[02:005) |   | | └─Token(Identifier) |foo|
//@[05:006) |   | ├─Token(Colon) |:|
//@[07:012) |   | └─StringSyntax
//@[07:012) |   |   └─Token(StringComplete) |'abc'|
//@[12:013) |   ├─Token(NewLine) |\n|
  bar: 123
//@[02:010) |   ├─ObjectPropertySyntax
//@[02:005) |   | ├─IdentifierSyntax
//@[02:005) |   | | └─Token(Identifier) |bar|
//@[05:006) |   | ├─Token(Colon) |:|
//@[07:010) |   | └─IntegerLiteralSyntax
//@[07:010) |   |   └─Token(Integer) |123|
//@[10:011) |   ├─Token(NewLine) |\n|
}
//@[00:001) |   └─Token(RightBrace) |}|
//@[01:003) ├─Token(NewLine) |\n\n|

var unExported FooType = {
//@[00:052) ├─VariableDeclarationSyntax
//@[00:003) | ├─Token(Identifier) |var|
//@[04:014) | ├─IdentifierSyntax
//@[04:014) | | └─Token(Identifier) |unExported|
//@[15:022) | ├─TypeVariableAccessSyntax
//@[15:022) | | └─IdentifierSyntax
//@[15:022) | |   └─Token(Identifier) |FooType|
//@[23:024) | ├─Token(Assignment) |=|
//@[25:052) | └─ObjectSyntax
//@[25:026) |   ├─Token(LeftBrace) |{|
//@[26:027) |   ├─Token(NewLine) |\n|
  foo: 'abc'
//@[02:012) |   ├─ObjectPropertySyntax
//@[02:005) |   | ├─IdentifierSyntax
//@[02:005) |   | | └─Token(Identifier) |foo|
//@[05:006) |   | ├─Token(Colon) |:|
//@[07:012) |   | └─StringSyntax
//@[07:012) |   |   └─Token(StringComplete) |'abc'|
//@[12:013) |   ├─Token(NewLine) |\n|
  bar: 123
//@[02:010) |   ├─ObjectPropertySyntax
//@[02:005) |   | ├─IdentifierSyntax
//@[02:005) |   | | └─Token(Identifier) |bar|
//@[05:006) |   | ├─Token(Colon) |:|
//@[07:010) |   | └─IntegerLiteralSyntax
//@[07:010) |   |   └─Token(Integer) |123|
//@[10:011) |   ├─Token(NewLine) |\n|
}
//@[00:001) |   └─Token(RightBrace) |}|
//@[01:002) ├─Token(NewLine) |\n|

//@[00:000) └─Token(EndOfFile) ||
