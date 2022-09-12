/* 
//@[00:883) ProgramSyntax
  This is a block comment.
*/
//@[02:006) ├─Token(NewLine) |\r\n\r\n|

// metadata with value
//@[22:024) ├─Token(NewLine) |\r\n|
metadata myString2 = 'string value'
//@[00:035) ├─MetadataDeclarationSyntax
//@[00:008) | ├─Token(Identifier) |metadata|
//@[09:018) | ├─IdentifierSyntax
//@[09:018) | | └─Token(Identifier) |myString2|
//@[19:020) | ├─Token(Assignment) |=|
//@[21:035) | └─StringSyntax
//@[21:035) |   └─Token(StringComplete) |'string value'|
//@[35:037) ├─Token(NewLine) |\r\n|
metadata myInt2 = 42
//@[00:020) ├─MetadataDeclarationSyntax
//@[00:008) | ├─Token(Identifier) |metadata|
//@[09:015) | ├─IdentifierSyntax
//@[09:015) | | └─Token(Identifier) |myInt2|
//@[16:017) | ├─Token(Assignment) |=|
//@[18:020) | └─IntegerLiteralSyntax
//@[18:020) |   └─Token(Integer) |42|
//@[20:022) ├─Token(NewLine) |\r\n|
metadata myTruth = true
//@[00:023) ├─MetadataDeclarationSyntax
//@[00:008) | ├─Token(Identifier) |metadata|
//@[09:016) | ├─IdentifierSyntax
//@[09:016) | | └─Token(Identifier) |myTruth|
//@[17:018) | ├─Token(Assignment) |=|
//@[19:023) | └─BooleanLiteralSyntax
//@[19:023) |   └─Token(TrueKeyword) |true|
//@[23:025) ├─Token(NewLine) |\r\n|
metadata myFalsehood = false
//@[00:028) ├─MetadataDeclarationSyntax
//@[00:008) | ├─Token(Identifier) |metadata|
//@[09:020) | ├─IdentifierSyntax
//@[09:020) | | └─Token(Identifier) |myFalsehood|
//@[21:022) | ├─Token(Assignment) |=|
//@[23:028) | └─BooleanLiteralSyntax
//@[23:028) |   └─Token(FalseKeyword) |false|
//@[28:030) ├─Token(NewLine) |\r\n|
metadata myEscapedString = 'First line\r\nSecond\ttabbed\tline'
//@[00:063) ├─MetadataDeclarationSyntax
//@[00:008) | ├─Token(Identifier) |metadata|
//@[09:024) | ├─IdentifierSyntax
//@[09:024) | | └─Token(Identifier) |myEscapedString|
//@[25:026) | ├─Token(Assignment) |=|
//@[27:063) | └─StringSyntax
//@[27:063) |   └─Token(StringComplete) |'First line\r\nSecond\ttabbed\tline'|
//@[63:065) ├─Token(NewLine) |\r\n|
metadata myMultiLineString = '''
//@[00:142) ├─MetadataDeclarationSyntax
//@[00:008) | ├─Token(Identifier) |metadata|
//@[09:026) | ├─IdentifierSyntax
//@[09:026) | | └─Token(Identifier) |myMultiLineString|
//@[27:028) | ├─Token(Assignment) |=|
//@[29:142) | └─StringSyntax
//@[29:142) |   └─Token(MultilineString) |'''\r\n  This is a multi line string // with comments,\r\n  blocked ${interpolation},\r\n  and a /* newline.\r\n  */\r\n'''|
  This is a multi line string // with comments,
  blocked ${interpolation},
  and a /* newline.
  */
'''
//@[03:007) ├─Token(NewLine) |\r\n\r\n|

// object value
//@[15:017) ├─Token(NewLine) |\r\n|
metadata foo = {
//@[00:249) ├─MetadataDeclarationSyntax
//@[00:008) | ├─Token(Identifier) |metadata|
//@[09:012) | ├─IdentifierSyntax
//@[09:012) | | └─Token(Identifier) |foo|
//@[13:014) | ├─Token(Assignment) |=|
//@[15:249) | └─ObjectSyntax
//@[15:016) |   ├─Token(LeftBrace) |{|
//@[16:018) |   ├─Token(NewLine) |\r\n|
  enabled: true
//@[02:015) |   ├─ObjectPropertySyntax
//@[02:009) |   | ├─IdentifierSyntax
//@[02:009) |   | | └─Token(Identifier) |enabled|
//@[09:010) |   | ├─Token(Colon) |:|
//@[11:015) |   | └─BooleanLiteralSyntax
//@[11:015) |   |   └─Token(TrueKeyword) |true|
//@[15:017) |   ├─Token(NewLine) |\r\n|
  name: 'this is my object'
//@[02:027) |   ├─ObjectPropertySyntax
//@[02:006) |   | ├─IdentifierSyntax
//@[02:006) |   | | └─Token(Identifier) |name|
//@[06:007) |   | ├─Token(Colon) |:|
//@[08:027) |   | └─StringSyntax
//@[08:027) |   |   └─Token(StringComplete) |'this is my object'|
//@[27:029) |   ├─Token(NewLine) |\r\n|
  priority: 3
//@[02:013) |   ├─ObjectPropertySyntax
//@[02:010) |   | ├─IdentifierSyntax
//@[02:010) |   | | └─Token(Identifier) |priority|
//@[10:011) |   | ├─Token(Colon) |:|
//@[12:013) |   | └─IntegerLiteralSyntax
//@[12:013) |   |   └─Token(Integer) |3|
//@[13:015) |   ├─Token(NewLine) |\r\n|
  info: {
//@[02:026) |   ├─ObjectPropertySyntax
//@[02:006) |   | ├─IdentifierSyntax
//@[02:006) |   | | └─Token(Identifier) |info|
//@[06:007) |   | ├─Token(Colon) |:|
//@[08:026) |   | └─ObjectSyntax
//@[08:009) |   |   ├─Token(LeftBrace) |{|
//@[09:011) |   |   ├─Token(NewLine) |\r\n|
    a: 'b'
//@[04:010) |   |   ├─ObjectPropertySyntax
//@[04:005) |   |   | ├─IdentifierSyntax
//@[04:005) |   |   | | └─Token(Identifier) |a|
//@[05:006) |   |   | ├─Token(Colon) |:|
//@[07:010) |   |   | └─StringSyntax
//@[07:010) |   |   |   └─Token(StringComplete) |'b'|
//@[10:012) |   |   ├─Token(NewLine) |\r\n|
  }
//@[02:003) |   |   └─Token(RightBrace) |}|
//@[03:005) |   ├─Token(NewLine) |\r\n|
  empty: {
//@[02:015) |   ├─ObjectPropertySyntax
//@[02:007) |   | ├─IdentifierSyntax
//@[02:007) |   | | └─Token(Identifier) |empty|
//@[07:008) |   | ├─Token(Colon) |:|
//@[09:015) |   | └─ObjectSyntax
//@[09:010) |   |   ├─Token(LeftBrace) |{|
//@[10:012) |   |   ├─Token(NewLine) |\r\n|
  }
//@[02:003) |   |   └─Token(RightBrace) |}|
//@[03:005) |   ├─Token(NewLine) |\r\n|
  array: [
//@[02:122) |   ├─ObjectPropertySyntax
//@[02:007) |   | ├─IdentifierSyntax
//@[02:007) |   | | └─Token(Identifier) |array|
//@[07:008) |   | ├─Token(Colon) |:|
//@[09:122) |   | └─ArraySyntax
//@[09:010) |   |   ├─Token(LeftSquare) |[|
//@[10:012) |   |   ├─Token(NewLine) |\r\n|
    'string item'
//@[04:017) |   |   ├─ArrayItemSyntax
//@[04:017) |   |   | └─StringSyntax
//@[04:017) |   |   |   └─Token(StringComplete) |'string item'|
//@[17:019) |   |   ├─Token(NewLine) |\r\n|
    12
//@[04:006) |   |   ├─ArrayItemSyntax
//@[04:006) |   |   | └─IntegerLiteralSyntax
//@[04:006) |   |   |   └─Token(Integer) |12|
//@[06:008) |   |   ├─Token(NewLine) |\r\n|
    true
//@[04:008) |   |   ├─ArrayItemSyntax
//@[04:008) |   |   | └─BooleanLiteralSyntax
//@[04:008) |   |   |   └─Token(TrueKeyword) |true|
//@[08:010) |   |   ├─Token(NewLine) |\r\n|
    [
//@[04:040) |   |   ├─ArrayItemSyntax
//@[04:040) |   |   | └─ArraySyntax
//@[04:005) |   |   |   ├─Token(LeftSquare) |[|
//@[05:007) |   |   |   ├─Token(NewLine) |\r\n|
      'inner'
//@[06:013) |   |   |   ├─ArrayItemSyntax
//@[06:013) |   |   |   | └─StringSyntax
//@[06:013) |   |   |   |   └─Token(StringComplete) |'inner'|
//@[13:015) |   |   |   ├─Token(NewLine) |\r\n|
      false
//@[06:011) |   |   |   ├─ArrayItemSyntax
//@[06:011) |   |   |   | └─BooleanLiteralSyntax
//@[06:011) |   |   |   |   └─Token(FalseKeyword) |false|
//@[11:013) |   |   |   ├─Token(NewLine) |\r\n|
    ]
//@[04:005) |   |   |   └─Token(RightSquare) |]|
//@[05:007) |   |   ├─Token(NewLine) |\r\n|
    {
//@[04:026) |   |   ├─ArrayItemSyntax
//@[04:026) |   |   | └─ObjectSyntax
//@[04:005) |   |   |   ├─Token(LeftBrace) |{|
//@[05:007) |   |   |   ├─Token(NewLine) |\r\n|
      a: 'b'
//@[06:012) |   |   |   ├─ObjectPropertySyntax
//@[06:007) |   |   |   | ├─IdentifierSyntax
//@[06:007) |   |   |   | | └─Token(Identifier) |a|
//@[07:008) |   |   |   | ├─Token(Colon) |:|
//@[09:012) |   |   |   | └─StringSyntax
//@[09:012) |   |   |   |   └─Token(StringComplete) |'b'|
//@[12:014) |   |   |   ├─Token(NewLine) |\r\n|
    }
//@[04:005) |   |   |   └─Token(RightBrace) |}|
//@[05:007) |   |   ├─Token(NewLine) |\r\n|
  ]
//@[02:003) |   |   └─Token(RightSquare) |]|
//@[03:005) |   ├─Token(NewLine) |\r\n|
}
//@[00:001) |   └─Token(RightBrace) |}|
//@[01:005) ├─Token(NewLine) |\r\n\r\n|

// array value
//@[14:016) ├─Token(NewLine) |\r\n|
metadata myArrayMetadata = [
//@[00:052) ├─MetadataDeclarationSyntax
//@[00:008) | ├─Token(Identifier) |metadata|
//@[09:024) | ├─IdentifierSyntax
//@[09:024) | | └─Token(Identifier) |myArrayMetadata|
//@[25:026) | ├─Token(Assignment) |=|
//@[27:052) | └─ArraySyntax
//@[27:028) |   ├─Token(LeftSquare) |[|
//@[28:030) |   ├─Token(NewLine) |\r\n|
  'a'
//@[02:005) |   ├─ArrayItemSyntax
//@[02:005) |   | └─StringSyntax
//@[02:005) |   |   └─Token(StringComplete) |'a'|
//@[05:007) |   ├─Token(NewLine) |\r\n|
  'b'
//@[02:005) |   ├─ArrayItemSyntax
//@[02:005) |   | └─StringSyntax
//@[02:005) |   |   └─Token(StringComplete) |'b'|
//@[05:007) |   ├─Token(NewLine) |\r\n|
  'c'
//@[02:005) |   ├─ArrayItemSyntax
//@[02:005) |   | └─StringSyntax
//@[02:005) |   |   └─Token(StringComplete) |'c'|
//@[05:007) |   ├─Token(NewLine) |\r\n|
]
//@[00:001) |   └─Token(RightSquare) |]|
//@[01:005) ├─Token(NewLine) |\r\n\r\n|

// emtpy object and array
//@[25:027) ├─Token(NewLine) |\r\n|
metadata myEmptyObj = { }
//@[00:025) ├─MetadataDeclarationSyntax
//@[00:008) | ├─Token(Identifier) |metadata|
//@[09:019) | ├─IdentifierSyntax
//@[09:019) | | └─Token(Identifier) |myEmptyObj|
//@[20:021) | ├─Token(Assignment) |=|
//@[22:025) | └─ObjectSyntax
//@[22:023) |   ├─Token(LeftBrace) |{|
//@[24:025) |   └─Token(RightBrace) |}|
//@[25:027) ├─Token(NewLine) |\r\n|
metadata myEmptyArray = [ ]
//@[00:027) ├─MetadataDeclarationSyntax
//@[00:008) | ├─Token(Identifier) |metadata|
//@[09:021) | ├─IdentifierSyntax
//@[09:021) | | └─Token(Identifier) |myEmptyArray|
//@[22:023) | ├─Token(Assignment) |=|
//@[24:027) | └─ArraySyntax
//@[24:025) |   ├─Token(LeftSquare) |[|
//@[26:027) |   └─Token(RightSquare) |]|
//@[27:031) ├─Token(NewLine) |\r\n\r\n|

// param with same name as metadata is permitted
//@[48:050) ├─Token(NewLine) |\r\n|
param foo string
//@[00:016) ├─ParameterDeclarationSyntax
//@[00:005) | ├─Token(Identifier) |param|
//@[06:009) | ├─IdentifierSyntax
//@[06:009) | | └─Token(Identifier) |foo|
//@[10:016) | └─SimpleTypeSyntax
//@[10:016) |   └─Token(Identifier) |string|
//@[16:018) ├─Token(NewLine) |\r\n|

//@[00:000) └─Token(EndOfFile) ||
