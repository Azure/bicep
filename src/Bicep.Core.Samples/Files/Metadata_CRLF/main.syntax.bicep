/* 
//@[00:633) ProgramSyntax
  This is a block comment.
*/
//@[02:006) ├─Token(NewLine) |\r\n\r\n|

// metadata with value
//@[22:024) ├─Token(NewLine) |\r\n|
meta myString2 = 'string value'
//@[00:031) ├─MetadataDeclarationSyntax
//@[00:004) | ├─Token(Identifier) |meta|
//@[05:014) | ├─IdentifierSyntax
//@[05:014) | | └─Token(Identifier) |myString2|
//@[15:016) | ├─Token(Assignment) |=|
//@[17:031) | └─StringSyntax
//@[17:031) | | └─Token(StringComplete) |'string value'|
//@[31:033) ├─Token(NewLine) |\r\n|
meta myInt2 = 42
//@[00:016) ├─MetadataDeclarationSyntax
//@[00:004) | ├─Token(Identifier) |meta|
//@[05:011) | ├─IdentifierSyntax
//@[05:011) | | └─Token(Identifier) |myInt2|
//@[12:013) | ├─Token(Assignment) |=|
//@[14:016) | └─IntegerLiteralSyntax
//@[14:016) | | └─Token(Integer) |42|
//@[16:018) ├─Token(NewLine) |\r\n|
meta myTruth = true
//@[00:019) ├─MetadataDeclarationSyntax
//@[00:004) | ├─Token(Identifier) |meta|
//@[05:012) | ├─IdentifierSyntax
//@[05:012) | | └─Token(Identifier) |myTruth|
//@[13:014) | ├─Token(Assignment) |=|
//@[15:019) | └─BooleanLiteralSyntax
//@[15:019) | | └─Token(TrueKeyword) |true|
//@[19:021) ├─Token(NewLine) |\r\n|
meta myFalsehood = false
//@[00:024) ├─MetadataDeclarationSyntax
//@[00:004) | ├─Token(Identifier) |meta|
//@[05:016) | ├─IdentifierSyntax
//@[05:016) | | └─Token(Identifier) |myFalsehood|
//@[17:018) | ├─Token(Assignment) |=|
//@[19:024) | └─BooleanLiteralSyntax
//@[19:024) | | └─Token(FalseKeyword) |false|
//@[24:026) ├─Token(NewLine) |\r\n|
meta myEscapedString = 'First line\r\nSecond\ttabbed\tline'
//@[00:059) ├─MetadataDeclarationSyntax
//@[00:004) | ├─Token(Identifier) |meta|
//@[05:020) | ├─IdentifierSyntax
//@[05:020) | | └─Token(Identifier) |myEscapedString|
//@[21:022) | ├─Token(Assignment) |=|
//@[23:059) | └─StringSyntax
//@[23:059) | | └─Token(StringComplete) |'First line\r\nSecond\ttabbed\tline'|
//@[59:063) ├─Token(NewLine) |\r\n\r\n|

// object value
//@[15:017) ├─Token(NewLine) |\r\n|
meta foo = {
//@[00:245) ├─MetadataDeclarationSyntax
//@[00:004) | ├─Token(Identifier) |meta|
//@[05:008) | ├─IdentifierSyntax
//@[05:008) | | └─Token(Identifier) |foo|
//@[09:010) | ├─Token(Assignment) |=|
//@[11:245) | └─ObjectSyntax
//@[11:012) | | ├─Token(LeftBrace) |{|
//@[12:014) | | ├─Token(NewLine) |\r\n|
  enabled: true
//@[02:015) | | ├─ObjectPropertySyntax
//@[02:009) | | | ├─IdentifierSyntax
//@[02:009) | | | | └─Token(Identifier) |enabled|
//@[09:010) | | | ├─Token(Colon) |:|
//@[11:015) | | | └─BooleanLiteralSyntax
//@[11:015) | | | | └─Token(TrueKeyword) |true|
//@[15:017) | | ├─Token(NewLine) |\r\n|
  name: 'this is my object'
//@[02:027) | | ├─ObjectPropertySyntax
//@[02:006) | | | ├─IdentifierSyntax
//@[02:006) | | | | └─Token(Identifier) |name|
//@[06:007) | | | ├─Token(Colon) |:|
//@[08:027) | | | └─StringSyntax
//@[08:027) | | | | └─Token(StringComplete) |'this is my object'|
//@[27:029) | | ├─Token(NewLine) |\r\n|
  priority: 3
//@[02:013) | | ├─ObjectPropertySyntax
//@[02:010) | | | ├─IdentifierSyntax
//@[02:010) | | | | └─Token(Identifier) |priority|
//@[10:011) | | | ├─Token(Colon) |:|
//@[12:013) | | | └─IntegerLiteralSyntax
//@[12:013) | | | | └─Token(Integer) |3|
//@[13:015) | | ├─Token(NewLine) |\r\n|
  info: {
//@[02:026) | | ├─ObjectPropertySyntax
//@[02:006) | | | ├─IdentifierSyntax
//@[02:006) | | | | └─Token(Identifier) |info|
//@[06:007) | | | ├─Token(Colon) |:|
//@[08:026) | | | └─ObjectSyntax
//@[08:009) | | | | ├─Token(LeftBrace) |{|
//@[09:011) | | | | ├─Token(NewLine) |\r\n|
    a: 'b'
//@[04:010) | | | | ├─ObjectPropertySyntax
//@[04:005) | | | | | ├─IdentifierSyntax
//@[04:005) | | | | | | └─Token(Identifier) |a|
//@[05:006) | | | | | ├─Token(Colon) |:|
//@[07:010) | | | | | └─StringSyntax
//@[07:010) | | | | | | └─Token(StringComplete) |'b'|
//@[10:012) | | | | ├─Token(NewLine) |\r\n|
  }
//@[02:003) | | | | └─Token(RightBrace) |}|
//@[03:005) | | ├─Token(NewLine) |\r\n|
  empty: {
//@[02:015) | | ├─ObjectPropertySyntax
//@[02:007) | | | ├─IdentifierSyntax
//@[02:007) | | | | └─Token(Identifier) |empty|
//@[07:008) | | | ├─Token(Colon) |:|
//@[09:015) | | | └─ObjectSyntax
//@[09:010) | | | | ├─Token(LeftBrace) |{|
//@[10:012) | | | | ├─Token(NewLine) |\r\n|
  }
//@[02:003) | | | | └─Token(RightBrace) |}|
//@[03:005) | | ├─Token(NewLine) |\r\n|
  array: [
//@[02:122) | | ├─ObjectPropertySyntax
//@[02:007) | | | ├─IdentifierSyntax
//@[02:007) | | | | └─Token(Identifier) |array|
//@[07:008) | | | ├─Token(Colon) |:|
//@[09:122) | | | └─ArraySyntax
//@[09:010) | | | | ├─Token(LeftSquare) |[|
//@[10:012) | | | | ├─Token(NewLine) |\r\n|
    'string item'
//@[04:017) | | | | ├─ArrayItemSyntax
//@[04:017) | | | | | └─StringSyntax
//@[04:017) | | | | | | └─Token(StringComplete) |'string item'|
//@[17:019) | | | | ├─Token(NewLine) |\r\n|
    12
//@[04:006) | | | | ├─ArrayItemSyntax
//@[04:006) | | | | | └─IntegerLiteralSyntax
//@[04:006) | | | | | | └─Token(Integer) |12|
//@[06:008) | | | | ├─Token(NewLine) |\r\n|
    true
//@[04:008) | | | | ├─ArrayItemSyntax
//@[04:008) | | | | | └─BooleanLiteralSyntax
//@[04:008) | | | | | | └─Token(TrueKeyword) |true|
//@[08:010) | | | | ├─Token(NewLine) |\r\n|
    [
//@[04:040) | | | | ├─ArrayItemSyntax
//@[04:040) | | | | | └─ArraySyntax
//@[04:005) | | | | | | ├─Token(LeftSquare) |[|
//@[05:007) | | | | | | ├─Token(NewLine) |\r\n|
      'inner'
//@[06:013) | | | | | | ├─ArrayItemSyntax
//@[06:013) | | | | | | | └─StringSyntax
//@[06:013) | | | | | | | | └─Token(StringComplete) |'inner'|
//@[13:015) | | | | | | ├─Token(NewLine) |\r\n|
      false
//@[06:011) | | | | | | ├─ArrayItemSyntax
//@[06:011) | | | | | | | └─BooleanLiteralSyntax
//@[06:011) | | | | | | | | └─Token(FalseKeyword) |false|
//@[11:013) | | | | | | ├─Token(NewLine) |\r\n|
    ]
//@[04:005) | | | | | | └─Token(RightSquare) |]|
//@[05:007) | | | | ├─Token(NewLine) |\r\n|
    {
//@[04:026) | | | | ├─ArrayItemSyntax
//@[04:026) | | | | | └─ObjectSyntax
//@[04:005) | | | | | | ├─Token(LeftBrace) |{|
//@[05:007) | | | | | | ├─Token(NewLine) |\r\n|
      a: 'b'
//@[06:012) | | | | | | ├─ObjectPropertySyntax
//@[06:007) | | | | | | | ├─IdentifierSyntax
//@[06:007) | | | | | | | | └─Token(Identifier) |a|
//@[07:008) | | | | | | | ├─Token(Colon) |:|
//@[09:012) | | | | | | | └─StringSyntax
//@[09:012) | | | | | | | | └─Token(StringComplete) |'b'|
//@[12:014) | | | | | | ├─Token(NewLine) |\r\n|
    }
//@[04:005) | | | | | | └─Token(RightBrace) |}|
//@[05:007) | | | | ├─Token(NewLine) |\r\n|
  ]
//@[02:003) | | | | └─Token(RightSquare) |]|
//@[03:005) | | ├─Token(NewLine) |\r\n|
}
//@[00:001) | | └─Token(RightBrace) |}|
//@[01:005) ├─Token(NewLine) |\r\n\r\n|

// array value
//@[14:016) ├─Token(NewLine) |\r\n|
meta myArrayMetadata = [
//@[00:048) ├─MetadataDeclarationSyntax
//@[00:004) | ├─Token(Identifier) |meta|
//@[05:020) | ├─IdentifierSyntax
//@[05:020) | | └─Token(Identifier) |myArrayMetadata|
//@[21:022) | ├─Token(Assignment) |=|
//@[23:048) | └─ArraySyntax
//@[23:024) | | ├─Token(LeftSquare) |[|
//@[24:026) | | ├─Token(NewLine) |\r\n|
  'a'
//@[02:005) | | ├─ArrayItemSyntax
//@[02:005) | | | └─StringSyntax
//@[02:005) | | | | └─Token(StringComplete) |'a'|
//@[05:007) | | ├─Token(NewLine) |\r\n|
  'b'
//@[02:005) | | ├─ArrayItemSyntax
//@[02:005) | | | └─StringSyntax
//@[02:005) | | | | └─Token(StringComplete) |'b'|
//@[05:007) | | ├─Token(NewLine) |\r\n|
  'c'
//@[02:005) | | ├─ArrayItemSyntax
//@[02:005) | | | └─StringSyntax
//@[02:005) | | | | └─Token(StringComplete) |'c'|
//@[05:007) | | ├─Token(NewLine) |\r\n|
]
//@[00:001) | | └─Token(RightSquare) |]|
//@[01:005) ├─Token(NewLine) |\r\n\r\n|

// emtpy object and array
//@[25:027) ├─Token(NewLine) |\r\n|
meta myEmptyObj = { }
//@[00:021) ├─MetadataDeclarationSyntax
//@[00:004) | ├─Token(Identifier) |meta|
//@[05:015) | ├─IdentifierSyntax
//@[05:015) | | └─Token(Identifier) |myEmptyObj|
//@[16:017) | ├─Token(Assignment) |=|
//@[18:021) | └─ObjectSyntax
//@[18:019) | | ├─Token(LeftBrace) |{|
//@[20:021) | | └─Token(RightBrace) |}|
//@[21:023) ├─Token(NewLine) |\r\n|
meta myEmptyArray = [ ]
//@[00:023) ├─MetadataDeclarationSyntax
//@[00:004) | ├─Token(Identifier) |meta|
//@[05:017) | ├─IdentifierSyntax
//@[05:017) | | └─Token(Identifier) |myEmptyArray|
//@[18:019) | ├─Token(Assignment) |=|
//@[20:023) | └─ArraySyntax
//@[20:021) | | ├─Token(LeftSquare) |[|
//@[22:023) | | └─Token(RightSquare) |]|
//@[23:025) ├─Token(NewLine) |\r\n|

//@[00:000) └─Token(EndOfFile) ||
