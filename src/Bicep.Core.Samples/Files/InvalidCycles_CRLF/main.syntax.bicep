
//@[00:963) ProgramSyntax
//@[00:002) ├─Token(NewLine) |\r\n|
//self-cycle
//@[12:014) ├─Token(NewLine) |\r\n|
var x = x
//@[00:009) ├─VariableDeclarationSyntax
//@[00:003) | ├─Token(Identifier) |var|
//@[04:005) | ├─IdentifierSyntax
//@[04:005) | | └─Token(Identifier) |x|
//@[06:007) | ├─Token(Assignment) |=|
//@[08:009) | └─VariableAccessSyntax
//@[08:009) |   └─IdentifierSyntax
//@[08:009) |     └─Token(Identifier) |x|
//@[09:011) ├─Token(NewLine) |\r\n|
var q = base64(q, !q)
//@[00:021) ├─VariableDeclarationSyntax
//@[00:003) | ├─Token(Identifier) |var|
//@[04:005) | ├─IdentifierSyntax
//@[04:005) | | └─Token(Identifier) |q|
//@[06:007) | ├─Token(Assignment) |=|
//@[08:021) | └─FunctionCallSyntax
//@[08:014) |   ├─IdentifierSyntax
//@[08:014) |   | └─Token(Identifier) |base64|
//@[14:015) |   ├─Token(LeftParen) |(|
//@[15:016) |   ├─FunctionArgumentSyntax
//@[15:016) |   | └─VariableAccessSyntax
//@[15:016) |   |   └─IdentifierSyntax
//@[15:016) |   |     └─Token(Identifier) |q|
//@[16:017) |   ├─Token(Comma) |,|
//@[18:020) |   ├─FunctionArgumentSyntax
//@[18:020) |   | └─UnaryOperationSyntax
//@[18:019) |   |   ├─Token(Exclamation) |!|
//@[19:020) |   |   └─VariableAccessSyntax
//@[19:020) |   |     └─IdentifierSyntax
//@[19:020) |   |       └─Token(Identifier) |q|
//@[20:021) |   └─Token(RightParen) |)|
//@[21:025) ├─Token(NewLine) |\r\n\r\n|

//2-cycle
//@[09:011) ├─Token(NewLine) |\r\n|
var a = b
//@[00:009) ├─VariableDeclarationSyntax
//@[00:003) | ├─Token(Identifier) |var|
//@[04:005) | ├─IdentifierSyntax
//@[04:005) | | └─Token(Identifier) |a|
//@[06:007) | ├─Token(Assignment) |=|
//@[08:009) | └─VariableAccessSyntax
//@[08:009) |   └─IdentifierSyntax
//@[08:009) |     └─Token(Identifier) |b|
//@[09:011) ├─Token(NewLine) |\r\n|
var b = max(a,1)
//@[00:016) ├─VariableDeclarationSyntax
//@[00:003) | ├─Token(Identifier) |var|
//@[04:005) | ├─IdentifierSyntax
//@[04:005) | | └─Token(Identifier) |b|
//@[06:007) | ├─Token(Assignment) |=|
//@[08:016) | └─FunctionCallSyntax
//@[08:011) |   ├─IdentifierSyntax
//@[08:011) |   | └─Token(Identifier) |max|
//@[11:012) |   ├─Token(LeftParen) |(|
//@[12:013) |   ├─FunctionArgumentSyntax
//@[12:013) |   | └─VariableAccessSyntax
//@[12:013) |   |   └─IdentifierSyntax
//@[12:013) |   |     └─Token(Identifier) |a|
//@[13:014) |   ├─Token(Comma) |,|
//@[14:015) |   ├─FunctionArgumentSyntax
//@[14:015) |   | └─IntegerLiteralSyntax
//@[14:015) |   |   └─Token(Integer) |1|
//@[15:016) |   └─Token(RightParen) |)|
//@[16:020) ├─Token(NewLine) |\r\n\r\n|

//3-cycle
//@[09:011) ├─Token(NewLine) |\r\n|
var e = f
//@[00:009) ├─VariableDeclarationSyntax
//@[00:003) | ├─Token(Identifier) |var|
//@[04:005) | ├─IdentifierSyntax
//@[04:005) | | └─Token(Identifier) |e|
//@[06:007) | ├─Token(Assignment) |=|
//@[08:009) | └─VariableAccessSyntax
//@[08:009) |   └─IdentifierSyntax
//@[08:009) |     └─Token(Identifier) |f|
//@[09:011) ├─Token(NewLine) |\r\n|
var f = g && true
//@[00:017) ├─VariableDeclarationSyntax
//@[00:003) | ├─Token(Identifier) |var|
//@[04:005) | ├─IdentifierSyntax
//@[04:005) | | └─Token(Identifier) |f|
//@[06:007) | ├─Token(Assignment) |=|
//@[08:017) | └─BinaryOperationSyntax
//@[08:009) |   ├─VariableAccessSyntax
//@[08:009) |   | └─IdentifierSyntax
//@[08:009) |   |   └─Token(Identifier) |g|
//@[10:012) |   ├─Token(LogicalAnd) |&&|
//@[13:017) |   └─BooleanLiteralSyntax
//@[13:017) |     └─Token(TrueKeyword) |true|
//@[17:019) ├─Token(NewLine) |\r\n|
var g = e ? e : e
//@[00:017) ├─VariableDeclarationSyntax
//@[00:003) | ├─Token(Identifier) |var|
//@[04:005) | ├─IdentifierSyntax
//@[04:005) | | └─Token(Identifier) |g|
//@[06:007) | ├─Token(Assignment) |=|
//@[08:017) | └─TernaryOperationSyntax
//@[08:009) |   ├─VariableAccessSyntax
//@[08:009) |   | └─IdentifierSyntax
//@[08:009) |   |   └─Token(Identifier) |e|
//@[10:011) |   ├─Token(Question) |?|
//@[12:013) |   ├─VariableAccessSyntax
//@[12:013) |   | └─IdentifierSyntax
//@[12:013) |   |   └─Token(Identifier) |e|
//@[14:015) |   ├─Token(Colon) |:|
//@[16:017) |   └─VariableAccessSyntax
//@[16:017) |     └─IdentifierSyntax
//@[16:017) |       └─Token(Identifier) |e|
//@[17:021) ├─Token(NewLine) |\r\n\r\n|

//4-cycle
//@[09:011) ├─Token(NewLine) |\r\n|
var aa = {
//@[00:023) ├─VariableDeclarationSyntax
//@[00:003) | ├─Token(Identifier) |var|
//@[04:006) | ├─IdentifierSyntax
//@[04:006) | | └─Token(Identifier) |aa|
//@[07:008) | ├─Token(Assignment) |=|
//@[09:023) | └─ObjectSyntax
//@[09:010) |   ├─Token(LeftBrace) |{|
//@[10:012) |   ├─Token(NewLine) |\r\n|
  bb: bb
//@[02:008) |   ├─ObjectPropertySyntax
//@[02:004) |   | ├─IdentifierSyntax
//@[02:004) |   | | └─Token(Identifier) |bb|
//@[04:005) |   | ├─Token(Colon) |:|
//@[06:008) |   | └─VariableAccessSyntax
//@[06:008) |   |   └─IdentifierSyntax
//@[06:008) |   |     └─Token(Identifier) |bb|
//@[08:010) |   ├─Token(NewLine) |\r\n|
}
//@[00:001) |   └─Token(RightBrace) |}|
//@[01:003) ├─Token(NewLine) |\r\n|
var bb = {
//@[00:023) ├─VariableDeclarationSyntax
//@[00:003) | ├─Token(Identifier) |var|
//@[04:006) | ├─IdentifierSyntax
//@[04:006) | | └─Token(Identifier) |bb|
//@[07:008) | ├─Token(Assignment) |=|
//@[09:023) | └─ObjectSyntax
//@[09:010) |   ├─Token(LeftBrace) |{|
//@[10:012) |   ├─Token(NewLine) |\r\n|
  cc: cc
//@[02:008) |   ├─ObjectPropertySyntax
//@[02:004) |   | ├─IdentifierSyntax
//@[02:004) |   | | └─Token(Identifier) |cc|
//@[04:005) |   | ├─Token(Colon) |:|
//@[06:008) |   | └─VariableAccessSyntax
//@[06:008) |   |   └─IdentifierSyntax
//@[06:008) |   |     └─Token(Identifier) |cc|
//@[08:010) |   ├─Token(NewLine) |\r\n|
}
//@[00:001) |   └─Token(RightBrace) |}|
//@[01:003) ├─Token(NewLine) |\r\n|
var cc = {
//@[00:023) ├─VariableDeclarationSyntax
//@[00:003) | ├─Token(Identifier) |var|
//@[04:006) | ├─IdentifierSyntax
//@[04:006) | | └─Token(Identifier) |cc|
//@[07:008) | ├─Token(Assignment) |=|
//@[09:023) | └─ObjectSyntax
//@[09:010) |   ├─Token(LeftBrace) |{|
//@[10:012) |   ├─Token(NewLine) |\r\n|
  dd: dd
//@[02:008) |   ├─ObjectPropertySyntax
//@[02:004) |   | ├─IdentifierSyntax
//@[02:004) |   | | └─Token(Identifier) |dd|
//@[04:005) |   | ├─Token(Colon) |:|
//@[06:008) |   | └─VariableAccessSyntax
//@[06:008) |   |   └─IdentifierSyntax
//@[06:008) |   |     └─Token(Identifier) |dd|
//@[08:010) |   ├─Token(NewLine) |\r\n|
}
//@[00:001) |   └─Token(RightBrace) |}|
//@[01:003) ├─Token(NewLine) |\r\n|
var dd = {
//@[00:023) ├─VariableDeclarationSyntax
//@[00:003) | ├─Token(Identifier) |var|
//@[04:006) | ├─IdentifierSyntax
//@[04:006) | | └─Token(Identifier) |dd|
//@[07:008) | ├─Token(Assignment) |=|
//@[09:023) | └─ObjectSyntax
//@[09:010) |   ├─Token(LeftBrace) |{|
//@[10:012) |   ├─Token(NewLine) |\r\n|
  aa: aa
//@[02:008) |   ├─ObjectPropertySyntax
//@[02:004) |   | ├─IdentifierSyntax
//@[02:004) |   | | └─Token(Identifier) |aa|
//@[04:005) |   | ├─Token(Colon) |:|
//@[06:008) |   | └─VariableAccessSyntax
//@[06:008) |   |   └─IdentifierSyntax
//@[06:008) |   |     └─Token(Identifier) |aa|
//@[08:010) |   ├─Token(NewLine) |\r\n|
}
//@[00:001) |   └─Token(RightBrace) |}|
//@[01:005) ├─Token(NewLine) |\r\n\r\n|

// variable completion cycles
//@[29:031) ├─Token(NewLine) |\r\n|
var one = {
//@[00:028) ├─VariableDeclarationSyntax
//@[00:003) | ├─Token(Identifier) |var|
//@[04:007) | ├─IdentifierSyntax
//@[04:007) | | └─Token(Identifier) |one|
//@[08:009) | ├─Token(Assignment) |=|
//@[10:028) | └─ObjectSyntax
//@[10:011) |   ├─Token(LeftBrace) |{|
//@[11:013) |   ├─Token(NewLine) |\r\n|
  first: two
//@[02:012) |   ├─ObjectPropertySyntax
//@[02:007) |   | ├─IdentifierSyntax
//@[02:007) |   | | └─Token(Identifier) |first|
//@[07:008) |   | ├─Token(Colon) |:|
//@[09:012) |   | └─VariableAccessSyntax
//@[09:012) |   |   └─IdentifierSyntax
//@[09:012) |   |     └─Token(Identifier) |two|
//@[12:014) |   ├─Token(NewLine) |\r\n|
}
//@[00:001) |   └─Token(RightBrace) |}|
//@[01:003) ├─Token(NewLine) |\r\n|
// #completionTest(15) -> empty
//@[31:033) ├─Token(NewLine) |\r\n|
var two = one.f
//@[00:015) ├─VariableDeclarationSyntax
//@[00:003) | ├─Token(Identifier) |var|
//@[04:007) | ├─IdentifierSyntax
//@[04:007) | | └─Token(Identifier) |two|
//@[08:009) | ├─Token(Assignment) |=|
//@[10:015) | └─PropertyAccessSyntax
//@[10:013) |   ├─VariableAccessSyntax
//@[10:013) |   | └─IdentifierSyntax
//@[10:013) |   |   └─Token(Identifier) |one|
//@[13:014) |   ├─Token(Dot) |.|
//@[14:015) |   └─IdentifierSyntax
//@[14:015) |     └─Token(Identifier) |f|
//@[15:017) ├─Token(NewLine) |\r\n|
// #completionTest(17) -> empty
//@[31:033) ├─Token(NewLine) |\r\n|
var twotwo = one.
//@[00:017) ├─VariableDeclarationSyntax
//@[00:003) | ├─Token(Identifier) |var|
//@[04:010) | ├─IdentifierSyntax
//@[04:010) | | └─Token(Identifier) |twotwo|
//@[11:012) | ├─Token(Assignment) |=|
//@[13:017) | └─PropertyAccessSyntax
//@[13:016) |   ├─VariableAccessSyntax
//@[13:016) |   | └─IdentifierSyntax
//@[13:016) |   |   └─Token(Identifier) |one|
//@[16:017) |   ├─Token(Dot) |.|
//@[17:017) |   └─IdentifierSyntax
//@[17:017) |     └─SkippedTriviaSyntax
//@[17:021) ├─Token(NewLine) |\r\n\r\n|

// resource completion cycles
//@[29:031) ├─Token(NewLine) |\r\n|
resource res1 'Microsoft.Storage/storageAccounts@2019-06-01' = {
//@[00:250) ├─ResourceDeclarationSyntax
//@[00:008) | ├─Token(Identifier) |resource|
//@[09:013) | ├─IdentifierSyntax
//@[09:013) | | └─Token(Identifier) |res1|
//@[14:060) | ├─StringSyntax
//@[14:060) | | └─Token(StringComplete) |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[61:062) | ├─Token(Assignment) |=|
//@[63:250) | └─ObjectSyntax
//@[63:064) |   ├─Token(LeftBrace) |{|
//@[64:066) |   ├─Token(NewLine) |\r\n|
  // #completionTest(14) -> empty
//@[33:035) |   ├─Token(NewLine) |\r\n|
  name: res2.n
//@[02:014) |   ├─ObjectPropertySyntax
//@[02:006) |   | ├─IdentifierSyntax
//@[02:006) |   | | └─Token(Identifier) |name|
//@[06:007) |   | ├─Token(Colon) |:|
//@[08:014) |   | └─PropertyAccessSyntax
//@[08:012) |   |   ├─VariableAccessSyntax
//@[08:012) |   |   | └─IdentifierSyntax
//@[08:012) |   |   |   └─Token(Identifier) |res2|
//@[12:013) |   |   ├─Token(Dot) |.|
//@[13:014) |   |   └─IdentifierSyntax
//@[13:014) |   |     └─Token(Identifier) |n|
//@[14:016) |   ├─Token(NewLine) |\r\n|
  location: 'l'
//@[02:015) |   ├─ObjectPropertySyntax
//@[02:010) |   | ├─IdentifierSyntax
//@[02:010) |   | | └─Token(Identifier) |location|
//@[10:011) |   | ├─Token(Colon) |:|
//@[12:015) |   | └─StringSyntax
//@[12:015) |   |   └─Token(StringComplete) |'l'|
//@[15:017) |   ├─Token(NewLine) |\r\n|
  sku: {
//@[02:092) |   ├─ObjectPropertySyntax
//@[02:005) |   | ├─IdentifierSyntax
//@[02:005) |   | | └─Token(Identifier) |sku|
//@[05:006) |   | ├─Token(Colon) |:|
//@[07:092) |   | └─ObjectSyntax
//@[07:008) |   |   ├─Token(LeftBrace) |{|
//@[08:010) |   |   ├─Token(NewLine) |\r\n|
    name: 'Premium_LRS'
//@[04:023) |   |   ├─ObjectPropertySyntax
//@[04:008) |   |   | ├─IdentifierSyntax
//@[04:008) |   |   | | └─Token(Identifier) |name|
//@[08:009) |   |   | ├─Token(Colon) |:|
//@[10:023) |   |   | └─StringSyntax
//@[10:023) |   |   |   └─Token(StringComplete) |'Premium_LRS'|
//@[23:025) |   |   ├─Token(NewLine) |\r\n|
    // #completionTest(15) -> empty
//@[35:037) |   |   ├─Token(NewLine) |\r\n|
    tier: res2.
//@[04:015) |   |   ├─ObjectPropertySyntax
//@[04:008) |   |   | ├─IdentifierSyntax
//@[04:008) |   |   | | └─Token(Identifier) |tier|
//@[08:009) |   |   | ├─Token(Colon) |:|
//@[10:015) |   |   | └─PropertyAccessSyntax
//@[10:014) |   |   |   ├─VariableAccessSyntax
//@[10:014) |   |   |   | └─IdentifierSyntax
//@[10:014) |   |   |   |   └─Token(Identifier) |res2|
//@[14:015) |   |   |   ├─Token(Dot) |.|
//@[15:015) |   |   |   └─IdentifierSyntax
//@[15:015) |   |   |     └─SkippedTriviaSyntax
//@[15:017) |   |   ├─Token(NewLine) |\r\n|
  }
//@[02:003) |   |   └─Token(RightBrace) |}|
//@[03:005) |   ├─Token(NewLine) |\r\n|
  kind: 'StorageV2'
//@[02:019) |   ├─ObjectPropertySyntax
//@[02:006) |   | ├─IdentifierSyntax
//@[02:006) |   | | └─Token(Identifier) |kind|
//@[06:007) |   | ├─Token(Colon) |:|
//@[08:019) |   | └─StringSyntax
//@[08:019) |   |   └─Token(StringComplete) |'StorageV2'|
//@[19:021) |   ├─Token(NewLine) |\r\n|
}
//@[00:001) |   └─Token(RightBrace) |}|
//@[01:003) ├─Token(NewLine) |\r\n|
resource res2 'Microsoft.Storage/storageAccounts@2019-06-01' = {
//@[00:246) ├─ResourceDeclarationSyntax
//@[00:008) | ├─Token(Identifier) |resource|
//@[09:013) | ├─IdentifierSyntax
//@[09:013) | | └─Token(Identifier) |res2|
//@[14:060) | ├─StringSyntax
//@[14:060) | | └─Token(StringComplete) |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[61:062) | ├─Token(Assignment) |=|
//@[63:246) | └─ObjectSyntax
//@[63:064) |   ├─Token(LeftBrace) |{|
//@[64:066) |   ├─Token(NewLine) |\r\n|
  name: res1.name
//@[02:017) |   ├─ObjectPropertySyntax
//@[02:006) |   | ├─IdentifierSyntax
//@[02:006) |   | | └─Token(Identifier) |name|
//@[06:007) |   | ├─Token(Colon) |:|
//@[08:017) |   | └─PropertyAccessSyntax
//@[08:012) |   |   ├─VariableAccessSyntax
//@[08:012) |   |   | └─IdentifierSyntax
//@[08:012) |   |   |   └─Token(Identifier) |res1|
//@[12:013) |   |   ├─Token(Dot) |.|
//@[13:017) |   |   └─IdentifierSyntax
//@[13:017) |   |     └─Token(Identifier) |name|
//@[17:019) |   ├─Token(NewLine) |\r\n|
  location: 'l'
//@[02:015) |   ├─ObjectPropertySyntax
//@[02:010) |   | ├─IdentifierSyntax
//@[02:010) |   | | └─Token(Identifier) |location|
//@[10:011) |   | ├─Token(Colon) |:|
//@[12:015) |   | └─StringSyntax
//@[12:015) |   |   └─Token(StringComplete) |'l'|
//@[15:017) |   ├─Token(NewLine) |\r\n|
  sku: {
//@[02:038) |   ├─ObjectPropertySyntax
//@[02:005) |   | ├─IdentifierSyntax
//@[02:005) |   | | └─Token(Identifier) |sku|
//@[05:006) |   | ├─Token(Colon) |:|
//@[07:038) |   | └─ObjectSyntax
//@[07:008) |   |   ├─Token(LeftBrace) |{|
//@[08:010) |   |   ├─Token(NewLine) |\r\n|
    name: 'Premium_LRS'
//@[04:023) |   |   ├─ObjectPropertySyntax
//@[04:008) |   |   | ├─IdentifierSyntax
//@[04:008) |   |   | | └─Token(Identifier) |name|
//@[08:009) |   |   | ├─Token(Colon) |:|
//@[10:023) |   |   | └─StringSyntax
//@[10:023) |   |   |   └─Token(StringComplete) |'Premium_LRS'|
//@[23:025) |   |   ├─Token(NewLine) |\r\n|
  }
//@[02:003) |   |   └─Token(RightBrace) |}|
//@[03:005) |   ├─Token(NewLine) |\r\n|
  properties: {
//@[02:080) |   ├─ObjectPropertySyntax
//@[02:012) |   | ├─IdentifierSyntax
//@[02:012) |   | | └─Token(Identifier) |properties|
//@[12:013) |   | ├─Token(Colon) |:|
//@[14:080) |   | └─ObjectSyntax
//@[14:015) |   |   ├─Token(LeftBrace) |{|
//@[15:017) |   |   ├─Token(NewLine) |\r\n|
    // #completionTest(21) -> empty
//@[35:037) |   |   ├─Token(NewLine) |\r\n|
    accessTier: res1.
//@[04:021) |   |   ├─ObjectPropertySyntax
//@[04:014) |   |   | ├─IdentifierSyntax
//@[04:014) |   |   | | └─Token(Identifier) |accessTier|
//@[14:015) |   |   | ├─Token(Colon) |:|
//@[16:021) |   |   | └─PropertyAccessSyntax
//@[16:020) |   |   |   ├─VariableAccessSyntax
//@[16:020) |   |   |   | └─IdentifierSyntax
//@[16:020) |   |   |   |   └─Token(Identifier) |res1|
//@[20:021) |   |   |   ├─Token(Dot) |.|
//@[21:021) |   |   |   └─IdentifierSyntax
//@[21:021) |   |   |     └─SkippedTriviaSyntax
//@[21:023) |   |   ├─Token(NewLine) |\r\n|
  }
//@[02:003) |   |   └─Token(RightBrace) |}|
//@[03:005) |   ├─Token(NewLine) |\r\n|
  kind: 'StorageV2'
//@[02:019) |   ├─ObjectPropertySyntax
//@[02:006) |   | ├─IdentifierSyntax
//@[02:006) |   | | └─Token(Identifier) |kind|
//@[06:007) |   | ├─Token(Colon) |:|
//@[08:019) |   | └─StringSyntax
//@[08:019) |   |   └─Token(StringComplete) |'StorageV2'|
//@[19:021) |   ├─Token(NewLine) |\r\n|
}
//@[00:001) |   └─Token(RightBrace) |}|
//@[01:001) └─Token(EndOfFile) ||
