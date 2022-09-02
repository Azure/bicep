/* 
//@[00:2930) ProgramSyntax
  This is a block comment.
*/
//@[02:0006) ├─Token(NewLine) |\r\n\r\n|

// parameters without default value
//@[35:0037) ├─Token(NewLine) |\r\n|
param myString string
//@[00:0021) ├─ParameterDeclarationSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0014) | ├─IdentifierSyntax
//@[06:0014) | | └─Token(Identifier) |myString|
//@[15:0021) | └─SimpleTypeSyntax
//@[15:0021) |   └─Token(Identifier) |string|
//@[21:0023) ├─Token(NewLine) |\r\n|
param myInt int
//@[00:0015) ├─ParameterDeclarationSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0011) | ├─IdentifierSyntax
//@[06:0011) | | └─Token(Identifier) |myInt|
//@[12:0015) | └─SimpleTypeSyntax
//@[12:0015) |   └─Token(Identifier) |int|
//@[15:0017) ├─Token(NewLine) |\r\n|
param myBool bool
//@[00:0017) ├─ParameterDeclarationSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0012) | ├─IdentifierSyntax
//@[06:0012) | | └─Token(Identifier) |myBool|
//@[13:0017) | └─SimpleTypeSyntax
//@[13:0017) |   └─Token(Identifier) |bool|
//@[17:0021) ├─Token(NewLine) |\r\n\r\n|

// parameters with default value
//@[32:0034) ├─Token(NewLine) |\r\n|
param myString2 string = 'string value'
//@[00:0039) ├─ParameterDeclarationSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0015) | ├─IdentifierSyntax
//@[06:0015) | | └─Token(Identifier) |myString2|
//@[16:0022) | ├─SimpleTypeSyntax
//@[16:0022) | | └─Token(Identifier) |string|
//@[23:0039) | └─ParameterDefaultValueSyntax
//@[23:0024) |   ├─Token(Assignment) |=|
//@[25:0039) |   └─StringSyntax
//@[25:0039) |     └─Token(StringComplete) |'string value'|
//@[39:0041) ├─Token(NewLine) |\r\n|
param myInt2 int = 42
//@[00:0021) ├─ParameterDeclarationSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0012) | ├─IdentifierSyntax
//@[06:0012) | | └─Token(Identifier) |myInt2|
//@[13:0016) | ├─SimpleTypeSyntax
//@[13:0016) | | └─Token(Identifier) |int|
//@[17:0021) | └─ParameterDefaultValueSyntax
//@[17:0018) |   ├─Token(Assignment) |=|
//@[19:0021) |   └─IntegerLiteralSyntax
//@[19:0021) |     └─Token(Integer) |42|
//@[21:0023) ├─Token(NewLine) |\r\n|
param myTruth bool = true
//@[00:0025) ├─ParameterDeclarationSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0013) | ├─IdentifierSyntax
//@[06:0013) | | └─Token(Identifier) |myTruth|
//@[14:0018) | ├─SimpleTypeSyntax
//@[14:0018) | | └─Token(Identifier) |bool|
//@[19:0025) | └─ParameterDefaultValueSyntax
//@[19:0020) |   ├─Token(Assignment) |=|
//@[21:0025) |   └─BooleanLiteralSyntax
//@[21:0025) |     └─Token(TrueKeyword) |true|
//@[25:0027) ├─Token(NewLine) |\r\n|
param myFalsehood bool = false
//@[00:0030) ├─ParameterDeclarationSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0017) | ├─IdentifierSyntax
//@[06:0017) | | └─Token(Identifier) |myFalsehood|
//@[18:0022) | ├─SimpleTypeSyntax
//@[18:0022) | | └─Token(Identifier) |bool|
//@[23:0030) | └─ParameterDefaultValueSyntax
//@[23:0024) |   ├─Token(Assignment) |=|
//@[25:0030) |   └─BooleanLiteralSyntax
//@[25:0030) |     └─Token(FalseKeyword) |false|
//@[30:0032) ├─Token(NewLine) |\r\n|
param myEscapedString string = 'First line\r\nSecond\ttabbed\tline'
//@[00:0067) ├─ParameterDeclarationSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0021) | ├─IdentifierSyntax
//@[06:0021) | | └─Token(Identifier) |myEscapedString|
//@[22:0028) | ├─SimpleTypeSyntax
//@[22:0028) | | └─Token(Identifier) |string|
//@[29:0067) | └─ParameterDefaultValueSyntax
//@[29:0030) |   ├─Token(Assignment) |=|
//@[31:0067) |   └─StringSyntax
//@[31:0067) |     └─Token(StringComplete) |'First line\r\nSecond\ttabbed\tline'|
//@[67:0071) ├─Token(NewLine) |\r\n\r\n|

// object default value
//@[23:0025) ├─Token(NewLine) |\r\n|
param foo object = {
//@[00:0253) ├─ParameterDeclarationSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0009) | ├─IdentifierSyntax
//@[06:0009) | | └─Token(Identifier) |foo|
//@[10:0016) | ├─SimpleTypeSyntax
//@[10:0016) | | └─Token(Identifier) |object|
//@[17:0253) | └─ParameterDefaultValueSyntax
//@[17:0018) |   ├─Token(Assignment) |=|
//@[19:0253) |   └─ObjectSyntax
//@[19:0020) |     ├─Token(LeftBrace) |{|
//@[20:0022) |     ├─Token(NewLine) |\r\n|
  enabled: true
//@[02:0015) |     ├─ObjectPropertySyntax
//@[02:0009) |     | ├─IdentifierSyntax
//@[02:0009) |     | | └─Token(Identifier) |enabled|
//@[09:0010) |     | ├─Token(Colon) |:|
//@[11:0015) |     | └─BooleanLiteralSyntax
//@[11:0015) |     |   └─Token(TrueKeyword) |true|
//@[15:0017) |     ├─Token(NewLine) |\r\n|
  name: 'this is my object'
//@[02:0027) |     ├─ObjectPropertySyntax
//@[02:0006) |     | ├─IdentifierSyntax
//@[02:0006) |     | | └─Token(Identifier) |name|
//@[06:0007) |     | ├─Token(Colon) |:|
//@[08:0027) |     | └─StringSyntax
//@[08:0027) |     |   └─Token(StringComplete) |'this is my object'|
//@[27:0029) |     ├─Token(NewLine) |\r\n|
  priority: 3
//@[02:0013) |     ├─ObjectPropertySyntax
//@[02:0010) |     | ├─IdentifierSyntax
//@[02:0010) |     | | └─Token(Identifier) |priority|
//@[10:0011) |     | ├─Token(Colon) |:|
//@[12:0013) |     | └─IntegerLiteralSyntax
//@[12:0013) |     |   └─Token(Integer) |3|
//@[13:0015) |     ├─Token(NewLine) |\r\n|
  info: {
//@[02:0026) |     ├─ObjectPropertySyntax
//@[02:0006) |     | ├─IdentifierSyntax
//@[02:0006) |     | | └─Token(Identifier) |info|
//@[06:0007) |     | ├─Token(Colon) |:|
//@[08:0026) |     | └─ObjectSyntax
//@[08:0009) |     |   ├─Token(LeftBrace) |{|
//@[09:0011) |     |   ├─Token(NewLine) |\r\n|
    a: 'b'
//@[04:0010) |     |   ├─ObjectPropertySyntax
//@[04:0005) |     |   | ├─IdentifierSyntax
//@[04:0005) |     |   | | └─Token(Identifier) |a|
//@[05:0006) |     |   | ├─Token(Colon) |:|
//@[07:0010) |     |   | └─StringSyntax
//@[07:0010) |     |   |   └─Token(StringComplete) |'b'|
//@[10:0012) |     |   ├─Token(NewLine) |\r\n|
  }
//@[02:0003) |     |   └─Token(RightBrace) |}|
//@[03:0005) |     ├─Token(NewLine) |\r\n|
  empty: {
//@[02:0015) |     ├─ObjectPropertySyntax
//@[02:0007) |     | ├─IdentifierSyntax
//@[02:0007) |     | | └─Token(Identifier) |empty|
//@[07:0008) |     | ├─Token(Colon) |:|
//@[09:0015) |     | └─ObjectSyntax
//@[09:0010) |     |   ├─Token(LeftBrace) |{|
//@[10:0012) |     |   ├─Token(NewLine) |\r\n|
  }
//@[02:0003) |     |   └─Token(RightBrace) |}|
//@[03:0005) |     ├─Token(NewLine) |\r\n|
  array: [
//@[02:0122) |     ├─ObjectPropertySyntax
//@[02:0007) |     | ├─IdentifierSyntax
//@[02:0007) |     | | └─Token(Identifier) |array|
//@[07:0008) |     | ├─Token(Colon) |:|
//@[09:0122) |     | └─ArraySyntax
//@[09:0010) |     |   ├─Token(LeftSquare) |[|
//@[10:0012) |     |   ├─Token(NewLine) |\r\n|
    'string item'
//@[04:0017) |     |   ├─ArrayItemSyntax
//@[04:0017) |     |   | └─StringSyntax
//@[04:0017) |     |   |   └─Token(StringComplete) |'string item'|
//@[17:0019) |     |   ├─Token(NewLine) |\r\n|
    12
//@[04:0006) |     |   ├─ArrayItemSyntax
//@[04:0006) |     |   | └─IntegerLiteralSyntax
//@[04:0006) |     |   |   └─Token(Integer) |12|
//@[06:0008) |     |   ├─Token(NewLine) |\r\n|
    true
//@[04:0008) |     |   ├─ArrayItemSyntax
//@[04:0008) |     |   | └─BooleanLiteralSyntax
//@[04:0008) |     |   |   └─Token(TrueKeyword) |true|
//@[08:0010) |     |   ├─Token(NewLine) |\r\n|
    [
//@[04:0040) |     |   ├─ArrayItemSyntax
//@[04:0040) |     |   | └─ArraySyntax
//@[04:0005) |     |   |   ├─Token(LeftSquare) |[|
//@[05:0007) |     |   |   ├─Token(NewLine) |\r\n|
      'inner'
//@[06:0013) |     |   |   ├─ArrayItemSyntax
//@[06:0013) |     |   |   | └─StringSyntax
//@[06:0013) |     |   |   |   └─Token(StringComplete) |'inner'|
//@[13:0015) |     |   |   ├─Token(NewLine) |\r\n|
      false
//@[06:0011) |     |   |   ├─ArrayItemSyntax
//@[06:0011) |     |   |   | └─BooleanLiteralSyntax
//@[06:0011) |     |   |   |   └─Token(FalseKeyword) |false|
//@[11:0013) |     |   |   ├─Token(NewLine) |\r\n|
    ]
//@[04:0005) |     |   |   └─Token(RightSquare) |]|
//@[05:0007) |     |   ├─Token(NewLine) |\r\n|
    {
//@[04:0026) |     |   ├─ArrayItemSyntax
//@[04:0026) |     |   | └─ObjectSyntax
//@[04:0005) |     |   |   ├─Token(LeftBrace) |{|
//@[05:0007) |     |   |   ├─Token(NewLine) |\r\n|
      a: 'b'
//@[06:0012) |     |   |   ├─ObjectPropertySyntax
//@[06:0007) |     |   |   | ├─IdentifierSyntax
//@[06:0007) |     |   |   | | └─Token(Identifier) |a|
//@[07:0008) |     |   |   | ├─Token(Colon) |:|
//@[09:0012) |     |   |   | └─StringSyntax
//@[09:0012) |     |   |   |   └─Token(StringComplete) |'b'|
//@[12:0014) |     |   |   ├─Token(NewLine) |\r\n|
    }
//@[04:0005) |     |   |   └─Token(RightBrace) |}|
//@[05:0007) |     |   ├─Token(NewLine) |\r\n|
  ]
//@[02:0003) |     |   └─Token(RightSquare) |]|
//@[03:0005) |     ├─Token(NewLine) |\r\n|
}
//@[00:0001) |     └─Token(RightBrace) |}|
//@[01:0005) ├─Token(NewLine) |\r\n\r\n|

// array default value
//@[22:0024) ├─Token(NewLine) |\r\n|
param myArrayParam array = [
//@[00:0052) ├─ParameterDeclarationSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0018) | ├─IdentifierSyntax
//@[06:0018) | | └─Token(Identifier) |myArrayParam|
//@[19:0024) | ├─SimpleTypeSyntax
//@[19:0024) | | └─Token(Identifier) |array|
//@[25:0052) | └─ParameterDefaultValueSyntax
//@[25:0026) |   ├─Token(Assignment) |=|
//@[27:0052) |   └─ArraySyntax
//@[27:0028) |     ├─Token(LeftSquare) |[|
//@[28:0030) |     ├─Token(NewLine) |\r\n|
  'a'
//@[02:0005) |     ├─ArrayItemSyntax
//@[02:0005) |     | └─StringSyntax
//@[02:0005) |     |   └─Token(StringComplete) |'a'|
//@[05:0007) |     ├─Token(NewLine) |\r\n|
  'b'
//@[02:0005) |     ├─ArrayItemSyntax
//@[02:0005) |     | └─StringSyntax
//@[02:0005) |     |   └─Token(StringComplete) |'b'|
//@[05:0007) |     ├─Token(NewLine) |\r\n|
  'c'
//@[02:0005) |     ├─ArrayItemSyntax
//@[02:0005) |     | └─StringSyntax
//@[02:0005) |     |   └─Token(StringComplete) |'c'|
//@[05:0007) |     ├─Token(NewLine) |\r\n|
]
//@[00:0001) |     └─Token(RightSquare) |]|
//@[01:0005) ├─Token(NewLine) |\r\n\r\n|

// secure string
//@[16:0018) ├─Token(NewLine) |\r\n|
@secure()
//@[00:0032) ├─ParameterDeclarationSyntax
//@[00:0009) | ├─DecoratorSyntax
//@[00:0001) | | ├─Token(At) |@|
//@[01:0009) | | └─FunctionCallSyntax
//@[01:0007) | |   ├─IdentifierSyntax
//@[01:0007) | |   | └─Token(Identifier) |secure|
//@[07:0008) | |   ├─Token(LeftParen) |(|
//@[08:0009) | |   └─Token(RightParen) |)|
//@[09:0011) | ├─Token(NewLine) |\r\n|
param password string
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0014) | ├─IdentifierSyntax
//@[06:0014) | | └─Token(Identifier) |password|
//@[15:0021) | └─SimpleTypeSyntax
//@[15:0021) |   └─Token(Identifier) |string|
//@[21:0025) ├─Token(NewLine) |\r\n\r\n|

// secure object
//@[16:0018) ├─Token(NewLine) |\r\n|
@secure()
//@[00:0036) ├─ParameterDeclarationSyntax
//@[00:0009) | ├─DecoratorSyntax
//@[00:0001) | | ├─Token(At) |@|
//@[01:0009) | | └─FunctionCallSyntax
//@[01:0007) | |   ├─IdentifierSyntax
//@[01:0007) | |   | └─Token(Identifier) |secure|
//@[07:0008) | |   ├─Token(LeftParen) |(|
//@[08:0009) | |   └─Token(RightParen) |)|
//@[09:0011) | ├─Token(NewLine) |\r\n|
param secretObject object
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0018) | ├─IdentifierSyntax
//@[06:0018) | | └─Token(Identifier) |secretObject|
//@[19:0025) | └─SimpleTypeSyntax
//@[19:0025) |   └─Token(Identifier) |object|
//@[25:0029) ├─Token(NewLine) |\r\n\r\n|

// enum parameter
//@[17:0019) ├─Token(NewLine) |\r\n|
@allowed([
//@[00:0075) ├─ParameterDeclarationSyntax
//@[00:0050) | ├─DecoratorSyntax
//@[00:0001) | | ├─Token(At) |@|
//@[01:0050) | | └─FunctionCallSyntax
//@[01:0008) | |   ├─IdentifierSyntax
//@[01:0008) | |   | └─Token(Identifier) |allowed|
//@[08:0009) | |   ├─Token(LeftParen) |(|
//@[09:0049) | |   ├─FunctionArgumentSyntax
//@[09:0049) | |   | └─ArraySyntax
//@[09:0010) | |   |   ├─Token(LeftSquare) |[|
//@[10:0012) | |   |   ├─Token(NewLine) |\r\n|
  'Standard_LRS'
//@[02:0016) | |   |   ├─ArrayItemSyntax
//@[02:0016) | |   |   | └─StringSyntax
//@[02:0016) | |   |   |   └─Token(StringComplete) |'Standard_LRS'|
//@[16:0018) | |   |   ├─Token(NewLine) |\r\n|
  'Standard_GRS'
//@[02:0016) | |   |   ├─ArrayItemSyntax
//@[02:0016) | |   |   | └─StringSyntax
//@[02:0016) | |   |   |   └─Token(StringComplete) |'Standard_GRS'|
//@[16:0018) | |   |   ├─Token(NewLine) |\r\n|
])
//@[00:0001) | |   |   └─Token(RightSquare) |]|
//@[01:0002) | |   └─Token(RightParen) |)|
//@[02:0004) | ├─Token(NewLine) |\r\n|
param storageSku string
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0016) | ├─IdentifierSyntax
//@[06:0016) | | └─Token(Identifier) |storageSku|
//@[17:0023) | └─SimpleTypeSyntax
//@[17:0023) |   └─Token(Identifier) |string|
//@[23:0027) ├─Token(NewLine) |\r\n\r\n|

// length constraint on a string
//@[32:0034) ├─Token(NewLine) |\r\n|
@minLength(3)
//@[00:0055) ├─ParameterDeclarationSyntax
//@[00:0013) | ├─DecoratorSyntax
//@[00:0001) | | ├─Token(At) |@|
//@[01:0013) | | └─FunctionCallSyntax
//@[01:0010) | |   ├─IdentifierSyntax
//@[01:0010) | |   | └─Token(Identifier) |minLength|
//@[10:0011) | |   ├─Token(LeftParen) |(|
//@[11:0012) | |   ├─FunctionArgumentSyntax
//@[11:0012) | |   | └─IntegerLiteralSyntax
//@[11:0012) | |   |   └─Token(Integer) |3|
//@[12:0013) | |   └─Token(RightParen) |)|
//@[13:0015) | ├─Token(NewLine) |\r\n|
@maxLength(24)
//@[00:0014) | ├─DecoratorSyntax
//@[00:0001) | | ├─Token(At) |@|
//@[01:0014) | | └─FunctionCallSyntax
//@[01:0010) | |   ├─IdentifierSyntax
//@[01:0010) | |   | └─Token(Identifier) |maxLength|
//@[10:0011) | |   ├─Token(LeftParen) |(|
//@[11:0013) | |   ├─FunctionArgumentSyntax
//@[11:0013) | |   | └─IntegerLiteralSyntax
//@[11:0013) | |   |   └─Token(Integer) |24|
//@[13:0014) | |   └─Token(RightParen) |)|
//@[14:0016) | ├─Token(NewLine) |\r\n|
param storageName string
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0017) | ├─IdentifierSyntax
//@[06:0017) | | └─Token(Identifier) |storageName|
//@[18:0024) | └─SimpleTypeSyntax
//@[18:0024) |   └─Token(Identifier) |string|
//@[24:0028) ├─Token(NewLine) |\r\n\r\n|

// length constraint on an array
//@[32:0034) ├─Token(NewLine) |\r\n|
@minLength(3)
//@[00:0052) ├─ParameterDeclarationSyntax
//@[00:0013) | ├─DecoratorSyntax
//@[00:0001) | | ├─Token(At) |@|
//@[01:0013) | | └─FunctionCallSyntax
//@[01:0010) | |   ├─IdentifierSyntax
//@[01:0010) | |   | └─Token(Identifier) |minLength|
//@[10:0011) | |   ├─Token(LeftParen) |(|
//@[11:0012) | |   ├─FunctionArgumentSyntax
//@[11:0012) | |   | └─IntegerLiteralSyntax
//@[11:0012) | |   |   └─Token(Integer) |3|
//@[12:0013) | |   └─Token(RightParen) |)|
//@[13:0015) | ├─Token(NewLine) |\r\n|
@maxLength(24)
//@[00:0014) | ├─DecoratorSyntax
//@[00:0001) | | ├─Token(At) |@|
//@[01:0014) | | └─FunctionCallSyntax
//@[01:0010) | |   ├─IdentifierSyntax
//@[01:0010) | |   | └─Token(Identifier) |maxLength|
//@[10:0011) | |   ├─Token(LeftParen) |(|
//@[11:0013) | |   ├─FunctionArgumentSyntax
//@[11:0013) | |   | └─IntegerLiteralSyntax
//@[11:0013) | |   |   └─Token(Integer) |24|
//@[13:0014) | |   └─Token(RightParen) |)|
//@[14:0016) | ├─Token(NewLine) |\r\n|
param someArray array
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0015) | ├─IdentifierSyntax
//@[06:0015) | | └─Token(Identifier) |someArray|
//@[16:0021) | └─SimpleTypeSyntax
//@[16:0021) |   └─Token(Identifier) |array|
//@[21:0025) ├─Token(NewLine) |\r\n\r\n|

// empty metadata
//@[17:0019) ├─Token(NewLine) |\r\n|
@metadata({})
//@[00:0041) ├─ParameterDeclarationSyntax
//@[00:0013) | ├─DecoratorSyntax
//@[00:0001) | | ├─Token(At) |@|
//@[01:0013) | | └─FunctionCallSyntax
//@[01:0009) | |   ├─IdentifierSyntax
//@[01:0009) | |   | └─Token(Identifier) |metadata|
//@[09:0010) | |   ├─Token(LeftParen) |(|
//@[10:0012) | |   ├─FunctionArgumentSyntax
//@[10:0012) | |   | └─ObjectSyntax
//@[10:0011) | |   |   ├─Token(LeftBrace) |{|
//@[11:0012) | |   |   └─Token(RightBrace) |}|
//@[12:0013) | |   └─Token(RightParen) |)|
//@[13:0015) | ├─Token(NewLine) |\r\n|
param emptyMetadata string
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0019) | ├─IdentifierSyntax
//@[06:0019) | | └─Token(Identifier) |emptyMetadata|
//@[20:0026) | └─SimpleTypeSyntax
//@[20:0026) |   └─Token(Identifier) |string|
//@[26:0030) ├─Token(NewLine) |\r\n\r\n|

// description
//@[14:0016) ├─Token(NewLine) |\r\n|
@metadata({
//@[00:0074) ├─ParameterDeclarationSyntax
//@[00:0048) | ├─DecoratorSyntax
//@[00:0001) | | ├─Token(At) |@|
//@[01:0048) | | └─FunctionCallSyntax
//@[01:0009) | |   ├─IdentifierSyntax
//@[01:0009) | |   | └─Token(Identifier) |metadata|
//@[09:0010) | |   ├─Token(LeftParen) |(|
//@[10:0047) | |   ├─FunctionArgumentSyntax
//@[10:0047) | |   | └─ObjectSyntax
//@[10:0011) | |   |   ├─Token(LeftBrace) |{|
//@[11:0013) | |   |   ├─Token(NewLine) |\r\n|
  description: 'my description'
//@[02:0031) | |   |   ├─ObjectPropertySyntax
//@[02:0013) | |   |   | ├─IdentifierSyntax
//@[02:0013) | |   |   | | └─Token(Identifier) |description|
//@[13:0014) | |   |   | ├─Token(Colon) |:|
//@[15:0031) | |   |   | └─StringSyntax
//@[15:0031) | |   |   |   └─Token(StringComplete) |'my description'|
//@[31:0033) | |   |   ├─Token(NewLine) |\r\n|
})
//@[00:0001) | |   |   └─Token(RightBrace) |}|
//@[01:0002) | |   └─Token(RightParen) |)|
//@[02:0004) | ├─Token(NewLine) |\r\n|
param description string
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0017) | ├─IdentifierSyntax
//@[06:0017) | | └─Token(Identifier) |description|
//@[18:0024) | └─SimpleTypeSyntax
//@[18:0024) |   └─Token(Identifier) |string|
//@[24:0028) ├─Token(NewLine) |\r\n\r\n|

@sys.description('my description')
//@[00:0061) ├─ParameterDeclarationSyntax
//@[00:0034) | ├─DecoratorSyntax
//@[00:0001) | | ├─Token(At) |@|
//@[01:0034) | | └─InstanceFunctionCallSyntax
//@[01:0004) | |   ├─VariableAccessSyntax
//@[01:0004) | |   | └─IdentifierSyntax
//@[01:0004) | |   |   └─Token(Identifier) |sys|
//@[04:0005) | |   ├─Token(Dot) |.|
//@[05:0016) | |   ├─IdentifierSyntax
//@[05:0016) | |   | └─Token(Identifier) |description|
//@[16:0017) | |   ├─Token(LeftParen) |(|
//@[17:0033) | |   ├─FunctionArgumentSyntax
//@[17:0033) | |   | └─StringSyntax
//@[17:0033) | |   |   └─Token(StringComplete) |'my description'|
//@[33:0034) | |   └─Token(RightParen) |)|
//@[34:0036) | ├─Token(NewLine) |\r\n|
param description2 string
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0018) | ├─IdentifierSyntax
//@[06:0018) | | └─Token(Identifier) |description2|
//@[19:0025) | └─SimpleTypeSyntax
//@[19:0025) |   └─Token(Identifier) |string|
//@[25:0029) ├─Token(NewLine) |\r\n\r\n|

// random extra metadata
//@[24:0026) ├─Token(NewLine) |\r\n|
@metadata({
//@[00:0143) ├─ParameterDeclarationSyntax
//@[00:0110) | ├─DecoratorSyntax
//@[00:0001) | | ├─Token(At) |@|
//@[01:0110) | | └─FunctionCallSyntax
//@[01:0009) | |   ├─IdentifierSyntax
//@[01:0009) | |   | └─Token(Identifier) |metadata|
//@[09:0010) | |   ├─Token(LeftParen) |(|
//@[10:0109) | |   ├─FunctionArgumentSyntax
//@[10:0109) | |   | └─ObjectSyntax
//@[10:0011) | |   |   ├─Token(LeftBrace) |{|
//@[11:0013) | |   |   ├─Token(NewLine) |\r\n|
  description: 'my description'
//@[02:0031) | |   |   ├─ObjectPropertySyntax
//@[02:0013) | |   |   | ├─IdentifierSyntax
//@[02:0013) | |   |   | | └─Token(Identifier) |description|
//@[13:0014) | |   |   | ├─Token(Colon) |:|
//@[15:0031) | |   |   | └─StringSyntax
//@[15:0031) | |   |   |   └─Token(StringComplete) |'my description'|
//@[31:0033) | |   |   ├─Token(NewLine) |\r\n|
  a: 1
//@[02:0006) | |   |   ├─ObjectPropertySyntax
//@[02:0003) | |   |   | ├─IdentifierSyntax
//@[02:0003) | |   |   | | └─Token(Identifier) |a|
//@[03:0004) | |   |   | ├─Token(Colon) |:|
//@[05:0006) | |   |   | └─IntegerLiteralSyntax
//@[05:0006) | |   |   |   └─Token(Integer) |1|
//@[06:0008) | |   |   ├─Token(NewLine) |\r\n|
  b: true
//@[02:0009) | |   |   ├─ObjectPropertySyntax
//@[02:0003) | |   |   | ├─IdentifierSyntax
//@[02:0003) | |   |   | | └─Token(Identifier) |b|
//@[03:0004) | |   |   | ├─Token(Colon) |:|
//@[05:0009) | |   |   | └─BooleanLiteralSyntax
//@[05:0009) | |   |   |   └─Token(TrueKeyword) |true|
//@[09:0011) | |   |   ├─Token(NewLine) |\r\n|
  c: [
//@[02:0011) | |   |   ├─ObjectPropertySyntax
//@[02:0003) | |   |   | ├─IdentifierSyntax
//@[02:0003) | |   |   | | └─Token(Identifier) |c|
//@[03:0004) | |   |   | ├─Token(Colon) |:|
//@[05:0011) | |   |   | └─ArraySyntax
//@[05:0006) | |   |   |   ├─Token(LeftSquare) |[|
//@[06:0008) | |   |   |   ├─Token(NewLine) |\r\n|
  ]
//@[02:0003) | |   |   |   └─Token(RightSquare) |]|
//@[03:0005) | |   |   ├─Token(NewLine) |\r\n|
  d: {
//@[02:0028) | |   |   ├─ObjectPropertySyntax
//@[02:0003) | |   |   | ├─IdentifierSyntax
//@[02:0003) | |   |   | | └─Token(Identifier) |d|
//@[03:0004) | |   |   | ├─Token(Colon) |:|
//@[05:0028) | |   |   | └─ObjectSyntax
//@[05:0006) | |   |   |   ├─Token(LeftBrace) |{|
//@[06:0008) | |   |   |   ├─Token(NewLine) |\r\n|
    test: 'abc'
//@[04:0015) | |   |   |   ├─ObjectPropertySyntax
//@[04:0008) | |   |   |   | ├─IdentifierSyntax
//@[04:0008) | |   |   |   | | └─Token(Identifier) |test|
//@[08:0009) | |   |   |   | ├─Token(Colon) |:|
//@[10:0015) | |   |   |   | └─StringSyntax
//@[10:0015) | |   |   |   |   └─Token(StringComplete) |'abc'|
//@[15:0017) | |   |   |   ├─Token(NewLine) |\r\n|
  }
//@[02:0003) | |   |   |   └─Token(RightBrace) |}|
//@[03:0005) | |   |   ├─Token(NewLine) |\r\n|
})
//@[00:0001) | |   |   └─Token(RightBrace) |}|
//@[01:0002) | |   └─Token(RightParen) |)|
//@[02:0004) | ├─Token(NewLine) |\r\n|
param additionalMetadata string
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0024) | ├─IdentifierSyntax
//@[06:0024) | | └─Token(Identifier) |additionalMetadata|
//@[25:0031) | └─SimpleTypeSyntax
//@[25:0031) |   └─Token(Identifier) |string|
//@[31:0035) ├─Token(NewLine) |\r\n\r\n|

// all modifiers together
//@[25:0027) ├─Token(NewLine) |\r\n|
@secure()
//@[00:0176) ├─ParameterDeclarationSyntax
//@[00:0009) | ├─DecoratorSyntax
//@[00:0001) | | ├─Token(At) |@|
//@[01:0009) | | └─FunctionCallSyntax
//@[01:0007) | |   ├─IdentifierSyntax
//@[01:0007) | |   | └─Token(Identifier) |secure|
//@[07:0008) | |   ├─Token(LeftParen) |(|
//@[08:0009) | |   └─Token(RightParen) |)|
//@[09:0011) | ├─Token(NewLine) |\r\n|
@minLength(3)
//@[00:0013) | ├─DecoratorSyntax
//@[00:0001) | | ├─Token(At) |@|
//@[01:0013) | | └─FunctionCallSyntax
//@[01:0010) | |   ├─IdentifierSyntax
//@[01:0010) | |   | └─Token(Identifier) |minLength|
//@[10:0011) | |   ├─Token(LeftParen) |(|
//@[11:0012) | |   ├─FunctionArgumentSyntax
//@[11:0012) | |   | └─IntegerLiteralSyntax
//@[11:0012) | |   |   └─Token(Integer) |3|
//@[12:0013) | |   └─Token(RightParen) |)|
//@[13:0015) | ├─Token(NewLine) |\r\n|
@maxLength(24)
//@[00:0014) | ├─DecoratorSyntax
//@[00:0001) | | ├─Token(At) |@|
//@[01:0014) | | └─FunctionCallSyntax
//@[01:0010) | |   ├─IdentifierSyntax
//@[01:0010) | |   | └─Token(Identifier) |maxLength|
//@[10:0011) | |   ├─Token(LeftParen) |(|
//@[11:0013) | |   ├─FunctionArgumentSyntax
//@[11:0013) | |   | └─IntegerLiteralSyntax
//@[11:0013) | |   |   └─Token(Integer) |24|
//@[13:0014) | |   └─Token(RightParen) |)|
//@[14:0016) | ├─Token(NewLine) |\r\n|
@allowed([
//@[00:0043) | ├─DecoratorSyntax
//@[00:0001) | | ├─Token(At) |@|
//@[01:0043) | | └─FunctionCallSyntax
//@[01:0008) | |   ├─IdentifierSyntax
//@[01:0008) | |   | └─Token(Identifier) |allowed|
//@[08:0009) | |   ├─Token(LeftParen) |(|
//@[09:0042) | |   ├─FunctionArgumentSyntax
//@[09:0042) | |   | └─ArraySyntax
//@[09:0010) | |   |   ├─Token(LeftSquare) |[|
//@[10:0012) | |   |   ├─Token(NewLine) |\r\n|
  'one'
//@[02:0007) | |   |   ├─ArrayItemSyntax
//@[02:0007) | |   |   | └─StringSyntax
//@[02:0007) | |   |   |   └─Token(StringComplete) |'one'|
//@[07:0009) | |   |   ├─Token(NewLine) |\r\n|
  'two'
//@[02:0007) | |   |   ├─ArrayItemSyntax
//@[02:0007) | |   |   | └─StringSyntax
//@[02:0007) | |   |   |   └─Token(StringComplete) |'two'|
//@[07:0009) | |   |   ├─Token(NewLine) |\r\n|
  'three'
//@[02:0009) | |   |   ├─ArrayItemSyntax
//@[02:0009) | |   |   | └─StringSyntax
//@[02:0009) | |   |   |   └─Token(StringComplete) |'three'|
//@[09:0011) | |   |   ├─Token(NewLine) |\r\n|
])
//@[00:0001) | |   |   └─Token(RightSquare) |]|
//@[01:0002) | |   └─Token(RightParen) |)|
//@[02:0004) | ├─Token(NewLine) |\r\n|
@metadata({
//@[00:0061) | ├─DecoratorSyntax
//@[00:0001) | | ├─Token(At) |@|
//@[01:0061) | | └─FunctionCallSyntax
//@[01:0009) | |   ├─IdentifierSyntax
//@[01:0009) | |   | └─Token(Identifier) |metadata|
//@[09:0010) | |   ├─Token(LeftParen) |(|
//@[10:0060) | |   ├─FunctionArgumentSyntax
//@[10:0060) | |   | └─ObjectSyntax
//@[10:0011) | |   |   ├─Token(LeftBrace) |{|
//@[11:0013) | |   |   ├─Token(NewLine) |\r\n|
  description: 'Name of the storage account'
//@[02:0044) | |   |   ├─ObjectPropertySyntax
//@[02:0013) | |   |   | ├─IdentifierSyntax
//@[02:0013) | |   |   | | └─Token(Identifier) |description|
//@[13:0014) | |   |   | ├─Token(Colon) |:|
//@[15:0044) | |   |   | └─StringSyntax
//@[15:0044) | |   |   |   └─Token(StringComplete) |'Name of the storage account'|
//@[44:0046) | |   |   ├─Token(NewLine) |\r\n|
})
//@[00:0001) | |   |   └─Token(RightBrace) |}|
//@[01:0002) | |   └─Token(RightParen) |)|
//@[02:0004) | ├─Token(NewLine) |\r\n|
param someParameter string
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0019) | ├─IdentifierSyntax
//@[06:0019) | | └─Token(Identifier) |someParameter|
//@[20:0026) | └─SimpleTypeSyntax
//@[20:0026) |   └─Token(Identifier) |string|
//@[26:0030) ├─Token(NewLine) |\r\n\r\n|

param defaultExpression bool = 18 != (true || false)
//@[00:0052) ├─ParameterDeclarationSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0023) | ├─IdentifierSyntax
//@[06:0023) | | └─Token(Identifier) |defaultExpression|
//@[24:0028) | ├─SimpleTypeSyntax
//@[24:0028) | | └─Token(Identifier) |bool|
//@[29:0052) | └─ParameterDefaultValueSyntax
//@[29:0030) |   ├─Token(Assignment) |=|
//@[31:0052) |   └─BinaryOperationSyntax
//@[31:0033) |     ├─IntegerLiteralSyntax
//@[31:0033) |     | └─Token(Integer) |18|
//@[34:0036) |     ├─Token(NotEquals) |!=|
//@[37:0052) |     └─ParenthesizedExpressionSyntax
//@[37:0038) |       ├─Token(LeftParen) |(|
//@[38:0051) |       ├─BinaryOperationSyntax
//@[38:0042) |       | ├─BooleanLiteralSyntax
//@[38:0042) |       | | └─Token(TrueKeyword) |true|
//@[43:0045) |       | ├─Token(LogicalOr) ||||
//@[46:0051) |       | └─BooleanLiteralSyntax
//@[46:0051) |       |   └─Token(FalseKeyword) |false|
//@[51:0052) |       └─Token(RightParen) |)|
//@[52:0056) ├─Token(NewLine) |\r\n\r\n|

@allowed([
//@[00:0060) ├─ParameterDeclarationSyntax
//@[00:0032) | ├─DecoratorSyntax
//@[00:0001) | | ├─Token(At) |@|
//@[01:0032) | | └─FunctionCallSyntax
//@[01:0008) | |   ├─IdentifierSyntax
//@[01:0008) | |   | └─Token(Identifier) |allowed|
//@[08:0009) | |   ├─Token(LeftParen) |(|
//@[09:0031) | |   ├─FunctionArgumentSyntax
//@[09:0031) | |   | └─ArraySyntax
//@[09:0010) | |   |   ├─Token(LeftSquare) |[|
//@[10:0012) | |   |   ├─Token(NewLine) |\r\n|
  'abc'
//@[02:0007) | |   |   ├─ArrayItemSyntax
//@[02:0007) | |   |   | └─StringSyntax
//@[02:0007) | |   |   |   └─Token(StringComplete) |'abc'|
//@[07:0009) | |   |   ├─Token(NewLine) |\r\n|
  'def'
//@[02:0007) | |   |   ├─ArrayItemSyntax
//@[02:0007) | |   |   | └─StringSyntax
//@[02:0007) | |   |   |   └─Token(StringComplete) |'def'|
//@[07:0009) | |   |   ├─Token(NewLine) |\r\n|
])
//@[00:0001) | |   |   └─Token(RightSquare) |]|
//@[01:0002) | |   └─Token(RightParen) |)|
//@[02:0004) | ├─Token(NewLine) |\r\n|
param stringLiteral string
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0019) | ├─IdentifierSyntax
//@[06:0019) | | └─Token(Identifier) |stringLiteral|
//@[20:0026) | └─SimpleTypeSyntax
//@[20:0026) |   └─Token(Identifier) |string|
//@[26:0030) ├─Token(NewLine) |\r\n\r\n|

@allowed([
//@[00:0110) ├─ParameterDeclarationSyntax
//@[00:0041) | ├─DecoratorSyntax
//@[00:0001) | | ├─Token(At) |@|
//@[01:0041) | | └─FunctionCallSyntax
//@[01:0008) | |   ├─IdentifierSyntax
//@[01:0008) | |   | └─Token(Identifier) |allowed|
//@[08:0009) | |   ├─Token(LeftParen) |(|
//@[09:0040) | |   ├─FunctionArgumentSyntax
//@[09:0040) | |   | └─ArraySyntax
//@[09:0010) | |   |   ├─Token(LeftSquare) |[|
//@[10:0012) | |   |   ├─Token(NewLine) |\r\n|
  'abc'
//@[02:0007) | |   |   ├─ArrayItemSyntax
//@[02:0007) | |   |   | └─StringSyntax
//@[02:0007) | |   |   |   └─Token(StringComplete) |'abc'|
//@[07:0009) | |   |   ├─Token(NewLine) |\r\n|
  'def'
//@[02:0007) | |   |   ├─ArrayItemSyntax
//@[02:0007) | |   |   | └─StringSyntax
//@[02:0007) | |   |   |   └─Token(StringComplete) |'def'|
//@[07:0009) | |   |   ├─Token(NewLine) |\r\n|
  'ghi'
//@[02:0007) | |   |   ├─ArrayItemSyntax
//@[02:0007) | |   |   | └─StringSyntax
//@[02:0007) | |   |   |   └─Token(StringComplete) |'ghi'|
//@[07:0009) | |   |   ├─Token(NewLine) |\r\n|
])
//@[00:0001) | |   |   └─Token(RightSquare) |]|
//@[01:0002) | |   └─Token(RightParen) |)|
//@[02:0004) | ├─Token(NewLine) |\r\n|
param stringLiteralWithAllowedValuesSuperset string = stringLiteral
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0044) | ├─IdentifierSyntax
//@[06:0044) | | └─Token(Identifier) |stringLiteralWithAllowedValuesSuperset|
//@[45:0051) | ├─SimpleTypeSyntax
//@[45:0051) | | └─Token(Identifier) |string|
//@[52:0067) | └─ParameterDefaultValueSyntax
//@[52:0053) |   ├─Token(Assignment) |=|
//@[54:0067) |   └─VariableAccessSyntax
//@[54:0067) |     └─IdentifierSyntax
//@[54:0067) |       └─Token(Identifier) |stringLiteral|
//@[67:0071) ├─Token(NewLine) |\r\n\r\n|

@secure()
//@[00:0111) ├─ParameterDeclarationSyntax
//@[00:0009) | ├─DecoratorSyntax
//@[00:0001) | | ├─Token(At) |@|
//@[01:0009) | | └─FunctionCallSyntax
//@[01:0007) | |   ├─IdentifierSyntax
//@[01:0007) | |   | └─Token(Identifier) |secure|
//@[07:0008) | |   ├─Token(LeftParen) |(|
//@[08:0009) | |   └─Token(RightParen) |)|
//@[09:0011) | ├─Token(NewLine) |\r\n|
@minLength(2)
//@[00:0013) | ├─DecoratorSyntax
//@[00:0001) | | ├─Token(At) |@|
//@[01:0013) | | └─FunctionCallSyntax
//@[01:0010) | |   ├─IdentifierSyntax
//@[01:0010) | |   | └─Token(Identifier) |minLength|
//@[10:0011) | |   ├─Token(LeftParen) |(|
//@[11:0012) | |   ├─FunctionArgumentSyntax
//@[11:0012) | |   | └─IntegerLiteralSyntax
//@[11:0012) | |   |   └─Token(Integer) |2|
//@[12:0013) | |   └─Token(RightParen) |)|
//@[13:0015) | ├─Token(NewLine) |\r\n|
  @maxLength(10)
//@[02:0016) | ├─DecoratorSyntax
//@[02:0003) | | ├─Token(At) |@|
//@[03:0016) | | └─FunctionCallSyntax
//@[03:0012) | |   ├─IdentifierSyntax
//@[03:0012) | |   | └─Token(Identifier) |maxLength|
//@[12:0013) | |   ├─Token(LeftParen) |(|
//@[13:0015) | |   ├─FunctionArgumentSyntax
//@[13:0015) | |   | └─IntegerLiteralSyntax
//@[13:0015) | |   |   └─Token(Integer) |10|
//@[15:0016) | |   └─Token(RightParen) |)|
//@[16:0018) | ├─Token(NewLine) |\r\n|
@allowed([
//@[00:0037) | ├─DecoratorSyntax
//@[00:0001) | | ├─Token(At) |@|
//@[01:0037) | | └─FunctionCallSyntax
//@[01:0008) | |   ├─IdentifierSyntax
//@[01:0008) | |   | └─Token(Identifier) |allowed|
//@[08:0009) | |   ├─Token(LeftParen) |(|
//@[09:0036) | |   ├─FunctionArgumentSyntax
//@[09:0036) | |   | └─ArraySyntax
//@[09:0010) | |   |   ├─Token(LeftSquare) |[|
//@[10:0012) | |   |   ├─Token(NewLine) |\r\n|
  'Apple'
//@[02:0009) | |   |   ├─ArrayItemSyntax
//@[02:0009) | |   |   | └─StringSyntax
//@[02:0009) | |   |   |   └─Token(StringComplete) |'Apple'|
//@[09:0011) | |   |   ├─Token(NewLine) |\r\n|
  'Banana'
//@[02:0010) | |   |   ├─ArrayItemSyntax
//@[02:0010) | |   |   | └─StringSyntax
//@[02:0010) | |   |   |   └─Token(StringComplete) |'Banana'|
//@[10:0012) | |   |   ├─Token(NewLine) |\r\n|
])
//@[00:0001) | |   |   └─Token(RightSquare) |]|
//@[01:0002) | |   └─Token(RightParen) |)|
//@[02:0004) | ├─Token(NewLine) |\r\n|
param decoratedString string
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0021) | ├─IdentifierSyntax
//@[06:0021) | | └─Token(Identifier) |decoratedString|
//@[22:0028) | └─SimpleTypeSyntax
//@[22:0028) |   └─Token(Identifier) |string|
//@[28:0032) ├─Token(NewLine) |\r\n\r\n|

@minValue(200)
//@[00:0044) ├─ParameterDeclarationSyntax
//@[00:0014) | ├─DecoratorSyntax
//@[00:0001) | | ├─Token(At) |@|
//@[01:0014) | | └─FunctionCallSyntax
//@[01:0009) | |   ├─IdentifierSyntax
//@[01:0009) | |   | └─Token(Identifier) |minValue|
//@[09:0010) | |   ├─Token(LeftParen) |(|
//@[10:0013) | |   ├─FunctionArgumentSyntax
//@[10:0013) | |   | └─IntegerLiteralSyntax
//@[10:0013) | |   |   └─Token(Integer) |200|
//@[13:0014) | |   └─Token(RightParen) |)|
//@[14:0016) | ├─Token(NewLine) |\r\n|
param decoratedInt int = 123
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0018) | ├─IdentifierSyntax
//@[06:0018) | | └─Token(Identifier) |decoratedInt|
//@[19:0022) | ├─SimpleTypeSyntax
//@[19:0022) | | └─Token(Identifier) |int|
//@[23:0028) | └─ParameterDefaultValueSyntax
//@[23:0024) |   ├─Token(Assignment) |=|
//@[25:0028) |   └─IntegerLiteralSyntax
//@[25:0028) |     └─Token(Integer) |123|
//@[28:0032) ├─Token(NewLine) |\r\n\r\n|

// negative integer literals are allowed as decorator values
//@[60:0062) ├─Token(NewLine) |\r\n|
@minValue(-10)
//@[00:0055) ├─ParameterDeclarationSyntax
//@[00:0014) | ├─DecoratorSyntax
//@[00:0001) | | ├─Token(At) |@|
//@[01:0014) | | └─FunctionCallSyntax
//@[01:0009) | |   ├─IdentifierSyntax
//@[01:0009) | |   | └─Token(Identifier) |minValue|
//@[09:0010) | |   ├─Token(LeftParen) |(|
//@[10:0013) | |   ├─FunctionArgumentSyntax
//@[10:0013) | |   | └─UnaryOperationSyntax
//@[10:0011) | |   |   ├─Token(Minus) |-|
//@[11:0013) | |   |   └─IntegerLiteralSyntax
//@[11:0013) | |   |     └─Token(Integer) |10|
//@[13:0014) | |   └─Token(RightParen) |)|
//@[14:0016) | ├─Token(NewLine) |\r\n|
@maxValue(-3)
//@[00:0013) | ├─DecoratorSyntax
//@[00:0001) | | ├─Token(At) |@|
//@[01:0013) | | └─FunctionCallSyntax
//@[01:0009) | |   ├─IdentifierSyntax
//@[01:0009) | |   | └─Token(Identifier) |maxValue|
//@[09:0010) | |   ├─Token(LeftParen) |(|
//@[10:0012) | |   ├─FunctionArgumentSyntax
//@[10:0012) | |   | └─UnaryOperationSyntax
//@[10:0011) | |   |   ├─Token(Minus) |-|
//@[11:0012) | |   |   └─IntegerLiteralSyntax
//@[11:0012) | |   |     └─Token(Integer) |3|
//@[12:0013) | |   └─Token(RightParen) |)|
//@[13:0015) | ├─Token(NewLine) |\r\n|
param negativeValues int
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0020) | ├─IdentifierSyntax
//@[06:0020) | | └─Token(Identifier) |negativeValues|
//@[21:0024) | └─SimpleTypeSyntax
//@[21:0024) |   └─Token(Identifier) |int|
//@[24:0028) ├─Token(NewLine) |\r\n\r\n|

@sys.description('A boolean.')
//@[00:0229) ├─ParameterDeclarationSyntax
//@[00:0030) | ├─DecoratorSyntax
//@[00:0001) | | ├─Token(At) |@|
//@[01:0030) | | └─InstanceFunctionCallSyntax
//@[01:0004) | |   ├─VariableAccessSyntax
//@[01:0004) | |   | └─IdentifierSyntax
//@[01:0004) | |   |   └─Token(Identifier) |sys|
//@[04:0005) | |   ├─Token(Dot) |.|
//@[05:0016) | |   ├─IdentifierSyntax
//@[05:0016) | |   | └─Token(Identifier) |description|
//@[16:0017) | |   ├─Token(LeftParen) |(|
//@[17:0029) | |   ├─FunctionArgumentSyntax
//@[17:0029) | |   | └─StringSyntax
//@[17:0029) | |   |   └─Token(StringComplete) |'A boolean.'|
//@[29:0030) | |   └─Token(RightParen) |)|
//@[30:0032) | ├─Token(NewLine) |\r\n|
@metadata({
//@[00:0145) | ├─DecoratorSyntax
//@[00:0001) | | ├─Token(At) |@|
//@[01:0145) | | └─FunctionCallSyntax
//@[01:0009) | |   ├─IdentifierSyntax
//@[01:0009) | |   | └─Token(Identifier) |metadata|
//@[09:0010) | |   ├─Token(LeftParen) |(|
//@[10:0144) | |   ├─FunctionArgumentSyntax
//@[10:0144) | |   | └─ObjectSyntax
//@[10:0011) | |   |   ├─Token(LeftBrace) |{|
//@[11:0013) | |   |   ├─Token(NewLine) |\r\n|
    description: 'I will be overrode.'
//@[04:0038) | |   |   ├─ObjectPropertySyntax
//@[04:0015) | |   |   | ├─IdentifierSyntax
//@[04:0015) | |   |   | | └─Token(Identifier) |description|
//@[15:0016) | |   |   | ├─Token(Colon) |:|
//@[17:0038) | |   |   | └─StringSyntax
//@[17:0038) | |   |   |   └─Token(StringComplete) |'I will be overrode.'|
//@[38:0040) | |   |   ├─Token(NewLine) |\r\n|
    foo: 'something'
//@[04:0020) | |   |   ├─ObjectPropertySyntax
//@[04:0007) | |   |   | ├─IdentifierSyntax
//@[04:0007) | |   |   | | └─Token(Identifier) |foo|
//@[07:0008) | |   |   | ├─Token(Colon) |:|
//@[09:0020) | |   |   | └─StringSyntax
//@[09:0020) | |   |   |   └─Token(StringComplete) |'something'|
//@[20:0022) | |   |   ├─Token(NewLine) |\r\n|
    bar: [
//@[04:0066) | |   |   ├─ObjectPropertySyntax
//@[04:0007) | |   |   | ├─IdentifierSyntax
//@[04:0007) | |   |   | | └─Token(Identifier) |bar|
//@[07:0008) | |   |   | ├─Token(Colon) |:|
//@[09:0066) | |   |   | └─ArraySyntax
//@[09:0010) | |   |   |   ├─Token(LeftSquare) |[|
//@[10:0012) | |   |   |   ├─Token(NewLine) |\r\n|
        {          }
//@[08:0020) | |   |   |   ├─ArrayItemSyntax
//@[08:0020) | |   |   |   | └─ObjectSyntax
//@[08:0009) | |   |   |   |   ├─Token(LeftBrace) |{|
//@[19:0020) | |   |   |   |   └─Token(RightBrace) |}|
//@[20:0022) | |   |   |   ├─Token(NewLine) |\r\n|
        true
//@[08:0012) | |   |   |   ├─ArrayItemSyntax
//@[08:0012) | |   |   |   | └─BooleanLiteralSyntax
//@[08:0012) | |   |   |   |   └─Token(TrueKeyword) |true|
//@[12:0014) | |   |   |   ├─Token(NewLine) |\r\n|
        123
//@[08:0011) | |   |   |   ├─ArrayItemSyntax
//@[08:0011) | |   |   |   | └─IntegerLiteralSyntax
//@[08:0011) | |   |   |   |   └─Token(Integer) |123|
//@[11:0013) | |   |   |   ├─Token(NewLine) |\r\n|
    ]
//@[04:0005) | |   |   |   └─Token(RightSquare) |]|
//@[05:0007) | |   |   ├─Token(NewLine) |\r\n|
})
//@[00:0001) | |   |   └─Token(RightBrace) |}|
//@[01:0002) | |   └─Token(RightParen) |)|
//@[02:0004) | ├─Token(NewLine) |\r\n|
param decoratedBool bool = (true && false) != true
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0019) | ├─IdentifierSyntax
//@[06:0019) | | └─Token(Identifier) |decoratedBool|
//@[20:0024) | ├─SimpleTypeSyntax
//@[20:0024) | | └─Token(Identifier) |bool|
//@[25:0050) | └─ParameterDefaultValueSyntax
//@[25:0026) |   ├─Token(Assignment) |=|
//@[27:0050) |   └─BinaryOperationSyntax
//@[27:0042) |     ├─ParenthesizedExpressionSyntax
//@[27:0028) |     | ├─Token(LeftParen) |(|
//@[28:0041) |     | ├─BinaryOperationSyntax
//@[28:0032) |     | | ├─BooleanLiteralSyntax
//@[28:0032) |     | | | └─Token(TrueKeyword) |true|
//@[33:0035) |     | | ├─Token(LogicalAnd) |&&|
//@[36:0041) |     | | └─BooleanLiteralSyntax
//@[36:0041) |     | |   └─Token(FalseKeyword) |false|
//@[41:0042) |     | └─Token(RightParen) |)|
//@[43:0045) |     ├─Token(NotEquals) |!=|
//@[46:0050) |     └─BooleanLiteralSyntax
//@[46:0050) |       └─Token(TrueKeyword) |true|
//@[50:0054) ├─Token(NewLine) |\r\n\r\n|

@secure()
//@[00:0276) ├─ParameterDeclarationSyntax
//@[00:0009) | ├─DecoratorSyntax
//@[00:0001) | | ├─Token(At) |@|
//@[01:0009) | | └─FunctionCallSyntax
//@[01:0007) | |   ├─IdentifierSyntax
//@[01:0007) | |   | └─Token(Identifier) |secure|
//@[07:0008) | |   ├─Token(LeftParen) |(|
//@[08:0009) | |   └─Token(RightParen) |)|
//@[09:0011) | ├─Token(NewLine) |\r\n|
param decoratedObject object = {
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0021) | ├─IdentifierSyntax
//@[06:0021) | | └─Token(Identifier) |decoratedObject|
//@[22:0028) | ├─SimpleTypeSyntax
//@[22:0028) | | └─Token(Identifier) |object|
//@[29:0265) | └─ParameterDefaultValueSyntax
//@[29:0030) |   ├─Token(Assignment) |=|
//@[31:0265) |   └─ObjectSyntax
//@[31:0032) |     ├─Token(LeftBrace) |{|
//@[32:0034) |     ├─Token(NewLine) |\r\n|
  enabled: true
//@[02:0015) |     ├─ObjectPropertySyntax
//@[02:0009) |     | ├─IdentifierSyntax
//@[02:0009) |     | | └─Token(Identifier) |enabled|
//@[09:0010) |     | ├─Token(Colon) |:|
//@[11:0015) |     | └─BooleanLiteralSyntax
//@[11:0015) |     |   └─Token(TrueKeyword) |true|
//@[15:0017) |     ├─Token(NewLine) |\r\n|
  name: 'this is my object'
//@[02:0027) |     ├─ObjectPropertySyntax
//@[02:0006) |     | ├─IdentifierSyntax
//@[02:0006) |     | | └─Token(Identifier) |name|
//@[06:0007) |     | ├─Token(Colon) |:|
//@[08:0027) |     | └─StringSyntax
//@[08:0027) |     |   └─Token(StringComplete) |'this is my object'|
//@[27:0029) |     ├─Token(NewLine) |\r\n|
  priority: 3
//@[02:0013) |     ├─ObjectPropertySyntax
//@[02:0010) |     | ├─IdentifierSyntax
//@[02:0010) |     | | └─Token(Identifier) |priority|
//@[10:0011) |     | ├─Token(Colon) |:|
//@[12:0013) |     | └─IntegerLiteralSyntax
//@[12:0013) |     |   └─Token(Integer) |3|
//@[13:0015) |     ├─Token(NewLine) |\r\n|
  info: {
//@[02:0026) |     ├─ObjectPropertySyntax
//@[02:0006) |     | ├─IdentifierSyntax
//@[02:0006) |     | | └─Token(Identifier) |info|
//@[06:0007) |     | ├─Token(Colon) |:|
//@[08:0026) |     | └─ObjectSyntax
//@[08:0009) |     |   ├─Token(LeftBrace) |{|
//@[09:0011) |     |   ├─Token(NewLine) |\r\n|
    a: 'b'
//@[04:0010) |     |   ├─ObjectPropertySyntax
//@[04:0005) |     |   | ├─IdentifierSyntax
//@[04:0005) |     |   | | └─Token(Identifier) |a|
//@[05:0006) |     |   | ├─Token(Colon) |:|
//@[07:0010) |     |   | └─StringSyntax
//@[07:0010) |     |   |   └─Token(StringComplete) |'b'|
//@[10:0012) |     |   ├─Token(NewLine) |\r\n|
  }
//@[02:0003) |     |   └─Token(RightBrace) |}|
//@[03:0005) |     ├─Token(NewLine) |\r\n|
  empty: {
//@[02:0015) |     ├─ObjectPropertySyntax
//@[02:0007) |     | ├─IdentifierSyntax
//@[02:0007) |     | | └─Token(Identifier) |empty|
//@[07:0008) |     | ├─Token(Colon) |:|
//@[09:0015) |     | └─ObjectSyntax
//@[09:0010) |     |   ├─Token(LeftBrace) |{|
//@[10:0012) |     |   ├─Token(NewLine) |\r\n|
  }
//@[02:0003) |     |   └─Token(RightBrace) |}|
//@[03:0005) |     ├─Token(NewLine) |\r\n|
  array: [
//@[02:0122) |     ├─ObjectPropertySyntax
//@[02:0007) |     | ├─IdentifierSyntax
//@[02:0007) |     | | └─Token(Identifier) |array|
//@[07:0008) |     | ├─Token(Colon) |:|
//@[09:0122) |     | └─ArraySyntax
//@[09:0010) |     |   ├─Token(LeftSquare) |[|
//@[10:0012) |     |   ├─Token(NewLine) |\r\n|
    'string item'
//@[04:0017) |     |   ├─ArrayItemSyntax
//@[04:0017) |     |   | └─StringSyntax
//@[04:0017) |     |   |   └─Token(StringComplete) |'string item'|
//@[17:0019) |     |   ├─Token(NewLine) |\r\n|
    12
//@[04:0006) |     |   ├─ArrayItemSyntax
//@[04:0006) |     |   | └─IntegerLiteralSyntax
//@[04:0006) |     |   |   └─Token(Integer) |12|
//@[06:0008) |     |   ├─Token(NewLine) |\r\n|
    true
//@[04:0008) |     |   ├─ArrayItemSyntax
//@[04:0008) |     |   | └─BooleanLiteralSyntax
//@[04:0008) |     |   |   └─Token(TrueKeyword) |true|
//@[08:0010) |     |   ├─Token(NewLine) |\r\n|
    [
//@[04:0040) |     |   ├─ArrayItemSyntax
//@[04:0040) |     |   | └─ArraySyntax
//@[04:0005) |     |   |   ├─Token(LeftSquare) |[|
//@[05:0007) |     |   |   ├─Token(NewLine) |\r\n|
      'inner'
//@[06:0013) |     |   |   ├─ArrayItemSyntax
//@[06:0013) |     |   |   | └─StringSyntax
//@[06:0013) |     |   |   |   └─Token(StringComplete) |'inner'|
//@[13:0015) |     |   |   ├─Token(NewLine) |\r\n|
      false
//@[06:0011) |     |   |   ├─ArrayItemSyntax
//@[06:0011) |     |   |   | └─BooleanLiteralSyntax
//@[06:0011) |     |   |   |   └─Token(FalseKeyword) |false|
//@[11:0013) |     |   |   ├─Token(NewLine) |\r\n|
    ]
//@[04:0005) |     |   |   └─Token(RightSquare) |]|
//@[05:0007) |     |   ├─Token(NewLine) |\r\n|
    {
//@[04:0026) |     |   ├─ArrayItemSyntax
//@[04:0026) |     |   | └─ObjectSyntax
//@[04:0005) |     |   |   ├─Token(LeftBrace) |{|
//@[05:0007) |     |   |   ├─Token(NewLine) |\r\n|
      a: 'b'
//@[06:0012) |     |   |   ├─ObjectPropertySyntax
//@[06:0007) |     |   |   | ├─IdentifierSyntax
//@[06:0007) |     |   |   | | └─Token(Identifier) |a|
//@[07:0008) |     |   |   | ├─Token(Colon) |:|
//@[09:0012) |     |   |   | └─StringSyntax
//@[09:0012) |     |   |   |   └─Token(StringComplete) |'b'|
//@[12:0014) |     |   |   ├─Token(NewLine) |\r\n|
    }
//@[04:0005) |     |   |   └─Token(RightBrace) |}|
//@[05:0007) |     |   ├─Token(NewLine) |\r\n|
  ]
//@[02:0003) |     |   └─Token(RightSquare) |]|
//@[03:0005) |     ├─Token(NewLine) |\r\n|
}
//@[00:0001) |     └─Token(RightBrace) |}|
//@[01:0005) ├─Token(NewLine) |\r\n\r\n|

@sys.metadata({
//@[00:0174) ├─ParameterDeclarationSyntax
//@[00:0049) | ├─DecoratorSyntax
//@[00:0001) | | ├─Token(At) |@|
//@[01:0049) | | └─InstanceFunctionCallSyntax
//@[01:0004) | |   ├─VariableAccessSyntax
//@[01:0004) | |   | └─IdentifierSyntax
//@[01:0004) | |   |   └─Token(Identifier) |sys|
//@[04:0005) | |   ├─Token(Dot) |.|
//@[05:0013) | |   ├─IdentifierSyntax
//@[05:0013) | |   | └─Token(Identifier) |metadata|
//@[13:0014) | |   ├─Token(LeftParen) |(|
//@[14:0048) | |   ├─FunctionArgumentSyntax
//@[14:0048) | |   | └─ObjectSyntax
//@[14:0015) | |   |   ├─Token(LeftBrace) |{|
//@[15:0017) | |   |   ├─Token(NewLine) |\r\n|
    description: 'An array.'
//@[04:0028) | |   |   ├─ObjectPropertySyntax
//@[04:0015) | |   |   | ├─IdentifierSyntax
//@[04:0015) | |   |   | | └─Token(Identifier) |description|
//@[15:0016) | |   |   | ├─Token(Colon) |:|
//@[17:0028) | |   |   | └─StringSyntax
//@[17:0028) | |   |   |   └─Token(StringComplete) |'An array.'|
//@[28:0030) | |   |   ├─Token(NewLine) |\r\n|
})
//@[00:0001) | |   |   └─Token(RightBrace) |}|
//@[01:0002) | |   └─Token(RightParen) |)|
//@[02:0004) | ├─Token(NewLine) |\r\n|
@sys.maxLength(20)
//@[00:0018) | ├─DecoratorSyntax
//@[00:0001) | | ├─Token(At) |@|
//@[01:0018) | | └─InstanceFunctionCallSyntax
//@[01:0004) | |   ├─VariableAccessSyntax
//@[01:0004) | |   | └─IdentifierSyntax
//@[01:0004) | |   |   └─Token(Identifier) |sys|
//@[04:0005) | |   ├─Token(Dot) |.|
//@[05:0014) | |   ├─IdentifierSyntax
//@[05:0014) | |   | └─Token(Identifier) |maxLength|
//@[14:0015) | |   ├─Token(LeftParen) |(|
//@[15:0017) | |   ├─FunctionArgumentSyntax
//@[15:0017) | |   | └─IntegerLiteralSyntax
//@[15:0017) | |   |   └─Token(Integer) |20|
//@[17:0018) | |   └─Token(RightParen) |)|
//@[18:0020) | ├─Token(NewLine) |\r\n|
@sys.description('I will be overrode.')
//@[00:0039) | ├─DecoratorSyntax
//@[00:0001) | | ├─Token(At) |@|
//@[01:0039) | | └─InstanceFunctionCallSyntax
//@[01:0004) | |   ├─VariableAccessSyntax
//@[01:0004) | |   | └─IdentifierSyntax
//@[01:0004) | |   |   └─Token(Identifier) |sys|
//@[04:0005) | |   ├─Token(Dot) |.|
//@[05:0016) | |   ├─IdentifierSyntax
//@[05:0016) | |   | └─Token(Identifier) |description|
//@[16:0017) | |   ├─Token(LeftParen) |(|
//@[17:0038) | |   ├─FunctionArgumentSyntax
//@[17:0038) | |   | └─StringSyntax
//@[17:0038) | |   |   └─Token(StringComplete) |'I will be overrode.'|
//@[38:0039) | |   └─Token(RightParen) |)|
//@[39:0041) | ├─Token(NewLine) |\r\n|
param decoratedArray array = [
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0020) | ├─IdentifierSyntax
//@[06:0020) | | └─Token(Identifier) |decoratedArray|
//@[21:0026) | ├─SimpleTypeSyntax
//@[21:0026) | | └─Token(Identifier) |array|
//@[27:0062) | └─ParameterDefaultValueSyntax
//@[27:0028) |   ├─Token(Assignment) |=|
//@[29:0062) |   └─ArraySyntax
//@[29:0030) |     ├─Token(LeftSquare) |[|
//@[30:0032) |     ├─Token(NewLine) |\r\n|
    utcNow()
//@[04:0012) |     ├─ArrayItemSyntax
//@[04:0012) |     | └─FunctionCallSyntax
//@[04:0010) |     |   ├─IdentifierSyntax
//@[04:0010) |     |   | └─Token(Identifier) |utcNow|
//@[10:0011) |     |   ├─Token(LeftParen) |(|
//@[11:0012) |     |   └─Token(RightParen) |)|
//@[12:0014) |     ├─Token(NewLine) |\r\n|
    newGuid()
//@[04:0013) |     ├─ArrayItemSyntax
//@[04:0013) |     | └─FunctionCallSyntax
//@[04:0011) |     |   ├─IdentifierSyntax
//@[04:0011) |     |   | └─Token(Identifier) |newGuid|
//@[11:0012) |     |   ├─Token(LeftParen) |(|
//@[12:0013) |     |   └─Token(RightParen) |)|
//@[13:0015) |     ├─Token(NewLine) |\r\n|
]
//@[00:0001) |     └─Token(RightSquare) |]|
//@[01:0003) ├─Token(NewLine) |\r\n|

//@[00:0000) └─Token(EndOfFile) ||
