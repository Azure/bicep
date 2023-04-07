resource foo 'Microsoft.Storage/storageAccounts@2022-09-01' = {
//@[00:5648) ProgramSyntax
//@[00:0222) ├─ResourceDeclarationSyntax
//@[00:0008) | ├─Token(Identifier) |resource|
//@[09:0012) | ├─IdentifierSyntax
//@[09:0012) | | └─Token(Identifier) |foo|
//@[13:0059) | ├─StringSyntax
//@[13:0059) | | └─Token(StringComplete) |'Microsoft.Storage/storageAccounts@2022-09-01'|
//@[60:0061) | ├─Token(Assignment) |=|
//@[62:0222) | └─ObjectSyntax
//@[62:0063) |   ├─Token(LeftBrace) |{|
//@[63:0064) |   ├─Token(NewLine) |\n|
  name: 'foo'
//@[02:0013) |   ├─ObjectPropertySyntax
//@[02:0006) |   | ├─IdentifierSyntax
//@[02:0006) |   | | └─Token(Identifier) |name|
//@[06:0007) |   | ├─Token(Colon) |:|
//@[08:0013) |   | └─StringSyntax
//@[08:0013) |   |   └─Token(StringComplete) |'foo'|
//@[13:0014) |   ├─Token(NewLine) |\n|
  location: 'westus'
//@[02:0020) |   ├─ObjectPropertySyntax
//@[02:0010) |   | ├─IdentifierSyntax
//@[02:0010) |   | | └─Token(Identifier) |location|
//@[10:0011) |   | ├─Token(Colon) |:|
//@[12:0020) |   | └─StringSyntax
//@[12:0020) |   |   └─Token(StringComplete) |'westus'|
//@[20:0021) |   ├─Token(NewLine) |\n|
  sku: {
//@[02:0037) |   ├─ObjectPropertySyntax
//@[02:0005) |   | ├─IdentifierSyntax
//@[02:0005) |   | | └─Token(Identifier) |sku|
//@[05:0006) |   | ├─Token(Colon) |:|
//@[07:0037) |   | └─ObjectSyntax
//@[07:0008) |   |   ├─Token(LeftBrace) |{|
//@[08:0009) |   |   ├─Token(NewLine) |\n|
    name: 'Standard_LRS'
//@[04:0024) |   |   ├─ObjectPropertySyntax
//@[04:0008) |   |   | ├─IdentifierSyntax
//@[04:0008) |   |   | | └─Token(Identifier) |name|
//@[08:0009) |   |   | ├─Token(Colon) |:|
//@[10:0024) |   |   | └─StringSyntax
//@[10:0024) |   |   |   └─Token(StringComplete) |'Standard_LRS'|
//@[24:0025) |   |   ├─Token(NewLine) |\n|
  }
//@[02:0003) |   |   └─Token(RightBrace) |}|
//@[03:0004) |   ├─Token(NewLine) |\n|
  kind: 'StorageV2'
//@[02:0019) |   ├─ObjectPropertySyntax
//@[02:0006) |   | ├─IdentifierSyntax
//@[02:0006) |   | | └─Token(Identifier) |kind|
//@[06:0007) |   | ├─Token(Colon) |:|
//@[08:0019) |   | └─StringSyntax
//@[08:0019) |   |   └─Token(StringComplete) |'StorageV2'|
//@[19:0021) |   ├─Token(NewLine) |\n\n|

  resource fooChild 'fileServices' = {
//@[02:0062) |   ├─ResourceDeclarationSyntax
//@[02:0010) |   | ├─Token(Identifier) |resource|
//@[11:0019) |   | ├─IdentifierSyntax
//@[11:0019) |   | | └─Token(Identifier) |fooChild|
//@[20:0034) |   | ├─StringSyntax
//@[20:0034) |   | | └─Token(StringComplete) |'fileServices'|
//@[35:0036) |   | ├─Token(Assignment) |=|
//@[37:0062) |   | └─ObjectSyntax
//@[37:0038) |   |   ├─Token(LeftBrace) |{|
//@[38:0039) |   |   ├─Token(NewLine) |\n|
    name: 'default'
//@[04:0019) |   |   ├─ObjectPropertySyntax
//@[04:0008) |   |   | ├─IdentifierSyntax
//@[04:0008) |   |   | | └─Token(Identifier) |name|
//@[08:0009) |   |   | ├─Token(Colon) |:|
//@[10:0019) |   |   | └─StringSyntax
//@[10:0019) |   |   |   └─Token(StringComplete) |'default'|
//@[19:0020) |   |   ├─Token(NewLine) |\n|
  }
//@[02:0003) |   |   └─Token(RightBrace) |}|
//@[03:0004) |   ├─Token(NewLine) |\n|
}
//@[00:0001) |   └─Token(RightBrace) |}|
//@[01:0002) ├─Token(NewLine) |\n|
resource foos 'Microsoft.Storage/storageAccounts@2022-09-01' = [for i in range(0, 2): {
//@[00:0188) ├─ResourceDeclarationSyntax
//@[00:0008) | ├─Token(Identifier) |resource|
//@[09:0013) | ├─IdentifierSyntax
//@[09:0013) | | └─Token(Identifier) |foos|
//@[14:0060) | ├─StringSyntax
//@[14:0060) | | └─Token(StringComplete) |'Microsoft.Storage/storageAccounts@2022-09-01'|
//@[61:0062) | ├─Token(Assignment) |=|
//@[63:0188) | └─ForSyntax
//@[63:0064) |   ├─Token(LeftSquare) |[|
//@[64:0067) |   ├─Token(Identifier) |for|
//@[68:0069) |   ├─LocalVariableSyntax
//@[68:0069) |   | └─IdentifierSyntax
//@[68:0069) |   |   └─Token(Identifier) |i|
//@[70:0072) |   ├─Token(Identifier) |in|
//@[73:0084) |   ├─FunctionCallSyntax
//@[73:0078) |   | ├─IdentifierSyntax
//@[73:0078) |   | | └─Token(Identifier) |range|
//@[78:0079) |   | ├─Token(LeftParen) |(|
//@[79:0080) |   | ├─FunctionArgumentSyntax
//@[79:0080) |   | | └─IntegerLiteralSyntax
//@[79:0080) |   | |   └─Token(Integer) |0|
//@[80:0081) |   | ├─Token(Comma) |,|
//@[82:0083) |   | ├─FunctionArgumentSyntax
//@[82:0083) |   | | └─IntegerLiteralSyntax
//@[82:0083) |   | |   └─Token(Integer) |2|
//@[83:0084) |   | └─Token(RightParen) |)|
//@[84:0085) |   ├─Token(Colon) |:|
//@[86:0187) |   ├─ObjectSyntax
//@[86:0087) |   | ├─Token(LeftBrace) |{|
//@[87:0088) |   | ├─Token(NewLine) |\n|
  name: 'foo-${i}'
//@[02:0018) |   | ├─ObjectPropertySyntax
//@[02:0006) |   | | ├─IdentifierSyntax
//@[02:0006) |   | | | └─Token(Identifier) |name|
//@[06:0007) |   | | ├─Token(Colon) |:|
//@[08:0018) |   | | └─StringSyntax
//@[08:0015) |   | |   ├─Token(StringLeftPiece) |'foo-${|
//@[15:0016) |   | |   ├─VariableAccessSyntax
//@[15:0016) |   | |   | └─IdentifierSyntax
//@[15:0016) |   | |   |   └─Token(Identifier) |i|
//@[16:0018) |   | |   └─Token(StringRightPiece) |}'|
//@[18:0019) |   | ├─Token(NewLine) |\n|
  location: 'westus'
//@[02:0020) |   | ├─ObjectPropertySyntax
//@[02:0010) |   | | ├─IdentifierSyntax
//@[02:0010) |   | | | └─Token(Identifier) |location|
//@[10:0011) |   | | ├─Token(Colon) |:|
//@[12:0020) |   | | └─StringSyntax
//@[12:0020) |   | |   └─Token(StringComplete) |'westus'|
//@[20:0021) |   | ├─Token(NewLine) |\n|
  sku: {
//@[02:0037) |   | ├─ObjectPropertySyntax
//@[02:0005) |   | | ├─IdentifierSyntax
//@[02:0005) |   | | | └─Token(Identifier) |sku|
//@[05:0006) |   | | ├─Token(Colon) |:|
//@[07:0037) |   | | └─ObjectSyntax
//@[07:0008) |   | |   ├─Token(LeftBrace) |{|
//@[08:0009) |   | |   ├─Token(NewLine) |\n|
    name: 'Standard_LRS'
//@[04:0024) |   | |   ├─ObjectPropertySyntax
//@[04:0008) |   | |   | ├─IdentifierSyntax
//@[04:0008) |   | |   | | └─Token(Identifier) |name|
//@[08:0009) |   | |   | ├─Token(Colon) |:|
//@[10:0024) |   | |   | └─StringSyntax
//@[10:0024) |   | |   |   └─Token(StringComplete) |'Standard_LRS'|
//@[24:0025) |   | |   ├─Token(NewLine) |\n|
  }
//@[02:0003) |   | |   └─Token(RightBrace) |}|
//@[03:0004) |   | ├─Token(NewLine) |\n|
  kind: 'StorageV2'
//@[02:0019) |   | ├─ObjectPropertySyntax
//@[02:0006) |   | | ├─IdentifierSyntax
//@[02:0006) |   | | | └─Token(Identifier) |kind|
//@[06:0007) |   | | ├─Token(Colon) |:|
//@[08:0019) |   | | └─StringSyntax
//@[08:0019) |   | |   └─Token(StringComplete) |'StorageV2'|
//@[19:0020) |   | ├─Token(NewLine) |\n|
}]
//@[00:0001) |   | └─Token(RightBrace) |}|
//@[01:0002) |   └─Token(RightSquare) |]|
//@[02:0003) ├─Token(NewLine) |\n|
resource existingFoo 'Microsoft.Storage/storageAccounts@2022-09-01' existing = {
//@[00:0104) ├─ResourceDeclarationSyntax
//@[00:0008) | ├─Token(Identifier) |resource|
//@[09:0020) | ├─IdentifierSyntax
//@[09:0020) | | └─Token(Identifier) |existingFoo|
//@[21:0067) | ├─StringSyntax
//@[21:0067) | | └─Token(StringComplete) |'Microsoft.Storage/storageAccounts@2022-09-01'|
//@[68:0076) | ├─Token(Identifier) |existing|
//@[77:0078) | ├─Token(Assignment) |=|
//@[79:0104) | └─ObjectSyntax
//@[79:0080) |   ├─Token(LeftBrace) |{|
//@[80:0081) |   ├─Token(NewLine) |\n|
  name: 'existingFoo'
//@[02:0021) |   ├─ObjectPropertySyntax
//@[02:0006) |   | ├─IdentifierSyntax
//@[02:0006) |   | | └─Token(Identifier) |name|
//@[06:0007) |   | ├─Token(Colon) |:|
//@[08:0021) |   | └─StringSyntax
//@[08:0021) |   |   └─Token(StringComplete) |'existingFoo'|
//@[21:0022) |   ├─Token(NewLine) |\n|
}
//@[00:0001) |   └─Token(RightBrace) |}|
//@[01:0003) ├─Token(NewLine) |\n\n|

param strParam string = 'id'
//@[00:0028) ├─ParameterDeclarationSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0014) | ├─IdentifierSyntax
//@[06:0014) | | └─Token(Identifier) |strParam|
//@[15:0021) | ├─VariableAccessSyntax
//@[15:0021) | | └─IdentifierSyntax
//@[15:0021) | |   └─Token(Identifier) |string|
//@[22:0028) | └─ParameterDefaultValueSyntax
//@[22:0023) |   ├─Token(Assignment) |=|
//@[24:0028) |   └─StringSyntax
//@[24:0028) |     └─Token(StringComplete) |'id'|
//@[28:0029) ├─Token(NewLine) |\n|
param strParam2 string = 'd'
//@[00:0028) ├─ParameterDeclarationSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0015) | ├─IdentifierSyntax
//@[06:0015) | | └─Token(Identifier) |strParam2|
//@[16:0022) | ├─VariableAccessSyntax
//@[16:0022) | | └─IdentifierSyntax
//@[16:0022) | |   └─Token(Identifier) |string|
//@[23:0028) | └─ParameterDefaultValueSyntax
//@[23:0024) |   ├─Token(Assignment) |=|
//@[25:0028) |   └─StringSyntax
//@[25:0028) |     └─Token(StringComplete) |'d'|
//@[28:0029) ├─Token(NewLine) |\n|
param cond bool = false
//@[00:0023) ├─ParameterDeclarationSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0010) | ├─IdentifierSyntax
//@[06:0010) | | └─Token(Identifier) |cond|
//@[11:0015) | ├─VariableAccessSyntax
//@[11:0015) | | └─IdentifierSyntax
//@[11:0015) | |   └─Token(Identifier) |bool|
//@[16:0023) | └─ParameterDefaultValueSyntax
//@[16:0017) |   ├─Token(Assignment) |=|
//@[18:0023) |   └─BooleanLiteralSyntax
//@[18:0023) |     └─Token(FalseKeyword) |false|
//@[23:0025) ├─Token(NewLine) |\n\n|

var zeroIndex = 0
//@[00:0017) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0013) | ├─IdentifierSyntax
//@[04:0013) | | └─Token(Identifier) |zeroIndex|
//@[14:0015) | ├─Token(Assignment) |=|
//@[16:0017) | └─IntegerLiteralSyntax
//@[16:0017) |   └─Token(Integer) |0|
//@[17:0018) ├─Token(NewLine) |\n|
var otherIndex = zeroIndex + 2
//@[00:0030) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0014) | ├─IdentifierSyntax
//@[04:0014) | | └─Token(Identifier) |otherIndex|
//@[15:0016) | ├─Token(Assignment) |=|
//@[17:0030) | └─BinaryOperationSyntax
//@[17:0026) |   ├─VariableAccessSyntax
//@[17:0026) |   | └─IdentifierSyntax
//@[17:0026) |   |   └─Token(Identifier) |zeroIndex|
//@[27:0028) |   ├─Token(Plus) |+|
//@[29:0030) |   └─IntegerLiteralSyntax
//@[29:0030) |     └─Token(Integer) |2|
//@[30:0031) ├─Token(NewLine) |\n|
var idAccessor = 'id'
//@[00:0021) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0014) | ├─IdentifierSyntax
//@[04:0014) | | └─Token(Identifier) |idAccessor|
//@[15:0016) | ├─Token(Assignment) |=|
//@[17:0021) | └─StringSyntax
//@[17:0021) |   └─Token(StringComplete) |'id'|
//@[21:0022) ├─Token(NewLine) |\n|
var dStr = 'd'
//@[00:0014) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0008) | ├─IdentifierSyntax
//@[04:0008) | | └─Token(Identifier) |dStr|
//@[09:0010) | ├─Token(Assignment) |=|
//@[11:0014) | └─StringSyntax
//@[11:0014) |   └─Token(StringComplete) |'d'|
//@[14:0015) ├─Token(NewLine) |\n|
var idAccessor2 = idAccessor
//@[00:0028) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0015) | ├─IdentifierSyntax
//@[04:0015) | | └─Token(Identifier) |idAccessor2|
//@[16:0017) | ├─Token(Assignment) |=|
//@[18:0028) | └─VariableAccessSyntax
//@[18:0028) |   └─IdentifierSyntax
//@[18:0028) |     └─Token(Identifier) |idAccessor|
//@[28:0029) ├─Token(NewLine) |\n|
var idAccessorInterpolated = '${idAccessor}'
//@[00:0044) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0026) | ├─IdentifierSyntax
//@[04:0026) | | └─Token(Identifier) |idAccessorInterpolated|
//@[27:0028) | ├─Token(Assignment) |=|
//@[29:0044) | └─StringSyntax
//@[29:0032) |   ├─Token(StringLeftPiece) |'${|
//@[32:0042) |   ├─VariableAccessSyntax
//@[32:0042) |   | └─IdentifierSyntax
//@[32:0042) |   |   └─Token(Identifier) |idAccessor|
//@[42:0044) |   └─Token(StringRightPiece) |}'|
//@[44:0045) ├─Token(NewLine) |\n|
var idAccessorMixed = 'i${dStr}'
//@[00:0032) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0019) | ├─IdentifierSyntax
//@[04:0019) | | └─Token(Identifier) |idAccessorMixed|
//@[20:0021) | ├─Token(Assignment) |=|
//@[22:0032) | └─StringSyntax
//@[22:0026) |   ├─Token(StringLeftPiece) |'i${|
//@[26:0030) |   ├─VariableAccessSyntax
//@[26:0030) |   | └─IdentifierSyntax
//@[26:0030) |   |   └─Token(Identifier) |dStr|
//@[30:0032) |   └─Token(StringRightPiece) |}'|
//@[32:0033) ├─Token(NewLine) |\n|
var propertiesAccessor = 'properties'
//@[00:0037) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0022) | ├─IdentifierSyntax
//@[04:0022) | | └─Token(Identifier) |propertiesAccessor|
//@[23:0024) | ├─Token(Assignment) |=|
//@[25:0037) | └─StringSyntax
//@[25:0037) |   └─Token(StringComplete) |'properties'|
//@[37:0038) ├─Token(NewLine) |\n|
var accessTierAccessor = 'accessTier'
//@[00:0037) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0022) | ├─IdentifierSyntax
//@[04:0022) | | └─Token(Identifier) |accessTierAccessor|
//@[23:0024) | ├─Token(Assignment) |=|
//@[25:0037) | └─StringSyntax
//@[25:0037) |   └─Token(StringComplete) |'accessTier'|
//@[37:0038) ├─Token(NewLine) |\n|
var strArray = ['id', 'properties']
//@[00:0035) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0012) | ├─IdentifierSyntax
//@[04:0012) | | └─Token(Identifier) |strArray|
//@[13:0014) | ├─Token(Assignment) |=|
//@[15:0035) | └─ArraySyntax
//@[15:0016) |   ├─Token(LeftSquare) |[|
//@[16:0020) |   ├─ArrayItemSyntax
//@[16:0020) |   | └─StringSyntax
//@[16:0020) |   |   └─Token(StringComplete) |'id'|
//@[20:0021) |   ├─Token(Comma) |,|
//@[22:0034) |   ├─ArrayItemSyntax
//@[22:0034) |   | └─StringSyntax
//@[22:0034) |   |   └─Token(StringComplete) |'properties'|
//@[34:0035) |   └─Token(RightSquare) |]|
//@[35:0037) ├─Token(NewLine) |\n\n|

var varForBodyInvalidRuntimeUsages = [for i in range(0, 2): {
//@[00:4526) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0034) | ├─IdentifierSyntax
//@[04:0034) | | └─Token(Identifier) |varForBodyInvalidRuntimeUsages|
//@[35:0036) | ├─Token(Assignment) |=|
//@[37:4526) | └─ForSyntax
//@[37:0038) |   ├─Token(LeftSquare) |[|
//@[38:0041) |   ├─Token(Identifier) |for|
//@[42:0043) |   ├─LocalVariableSyntax
//@[42:0043) |   | └─IdentifierSyntax
//@[42:0043) |   |   └─Token(Identifier) |i|
//@[44:0046) |   ├─Token(Identifier) |in|
//@[47:0058) |   ├─FunctionCallSyntax
//@[47:0052) |   | ├─IdentifierSyntax
//@[47:0052) |   | | └─Token(Identifier) |range|
//@[52:0053) |   | ├─Token(LeftParen) |(|
//@[53:0054) |   | ├─FunctionArgumentSyntax
//@[53:0054) |   | | └─IntegerLiteralSyntax
//@[53:0054) |   | |   └─Token(Integer) |0|
//@[54:0055) |   | ├─Token(Comma) |,|
//@[56:0057) |   | ├─FunctionArgumentSyntax
//@[56:0057) |   | | └─IntegerLiteralSyntax
//@[56:0057) |   | |   └─Token(Integer) |2|
//@[57:0058) |   | └─Token(RightParen) |)|
//@[58:0059) |   ├─Token(Colon) |:|
//@[60:4525) |   ├─ObjectSyntax
//@[60:0061) |   | ├─Token(LeftBrace) |{|
//@[61:0062) |   | ├─Token(NewLine) |\n|
  case1: foo
//@[02:0012) |   | ├─ObjectPropertySyntax
//@[02:0007) |   | | ├─IdentifierSyntax
//@[02:0007) |   | | | └─Token(Identifier) |case1|
//@[07:0008) |   | | ├─Token(Colon) |:|
//@[09:0012) |   | | └─VariableAccessSyntax
//@[09:0012) |   | |   └─IdentifierSyntax
//@[09:0012) |   | |     └─Token(Identifier) |foo|
//@[12:0013) |   | ├─Token(NewLine) |\n|
  case2: existingFoo
//@[02:0020) |   | ├─ObjectPropertySyntax
//@[02:0007) |   | | ├─IdentifierSyntax
//@[02:0007) |   | | | └─Token(Identifier) |case2|
//@[07:0008) |   | | ├─Token(Colon) |:|
//@[09:0020) |   | | └─VariableAccessSyntax
//@[09:0020) |   | |   └─IdentifierSyntax
//@[09:0020) |   | |     └─Token(Identifier) |existingFoo|
//@[20:0021) |   | ├─Token(NewLine) |\n|
  case3: foo::fooChild
//@[02:0022) |   | ├─ObjectPropertySyntax
//@[02:0007) |   | | ├─IdentifierSyntax
//@[02:0007) |   | | | └─Token(Identifier) |case3|
//@[07:0008) |   | | ├─Token(Colon) |:|
//@[09:0022) |   | | └─ResourceAccessSyntax
//@[09:0012) |   | |   ├─VariableAccessSyntax
//@[09:0012) |   | |   | └─IdentifierSyntax
//@[09:0012) |   | |   |   └─Token(Identifier) |foo|
//@[12:0014) |   | |   ├─Token(DoubleColon) |::|
//@[14:0022) |   | |   └─IdentifierSyntax
//@[14:0022) |   | |     └─Token(Identifier) |fooChild|
//@[22:0023) |   | ├─Token(NewLine) |\n|
  case4: foos[0]
//@[02:0016) |   | ├─ObjectPropertySyntax
//@[02:0007) |   | | ├─IdentifierSyntax
//@[02:0007) |   | | | └─Token(Identifier) |case4|
//@[07:0008) |   | | ├─Token(Colon) |:|
//@[09:0016) |   | | └─ArrayAccessSyntax
//@[09:0013) |   | |   ├─VariableAccessSyntax
//@[09:0013) |   | |   | └─IdentifierSyntax
//@[09:0013) |   | |   |   └─Token(Identifier) |foos|
//@[13:0014) |   | |   ├─Token(LeftSquare) |[|
//@[14:0015) |   | |   ├─IntegerLiteralSyntax
//@[14:0015) |   | |   | └─Token(Integer) |0|
//@[15:0016) |   | |   └─Token(RightSquare) |]|
//@[16:0017) |   | ├─Token(NewLine) |\n|
  case5: foos[i]
//@[02:0016) |   | ├─ObjectPropertySyntax
//@[02:0007) |   | | ├─IdentifierSyntax
//@[02:0007) |   | | | └─Token(Identifier) |case5|
//@[07:0008) |   | | ├─Token(Colon) |:|
//@[09:0016) |   | | └─ArrayAccessSyntax
//@[09:0013) |   | |   ├─VariableAccessSyntax
//@[09:0013) |   | |   | └─IdentifierSyntax
//@[09:0013) |   | |   |   └─Token(Identifier) |foos|
//@[13:0014) |   | |   ├─Token(LeftSquare) |[|
//@[14:0015) |   | |   ├─VariableAccessSyntax
//@[14:0015) |   | |   | └─IdentifierSyntax
//@[14:0015) |   | |   |   └─Token(Identifier) |i|
//@[15:0016) |   | |   └─Token(RightSquare) |]|
//@[16:0017) |   | ├─Token(NewLine) |\n|
  case6: foos[i + 2]
//@[02:0020) |   | ├─ObjectPropertySyntax
//@[02:0007) |   | | ├─IdentifierSyntax
//@[02:0007) |   | | | └─Token(Identifier) |case6|
//@[07:0008) |   | | ├─Token(Colon) |:|
//@[09:0020) |   | | └─ArrayAccessSyntax
//@[09:0013) |   | |   ├─VariableAccessSyntax
//@[09:0013) |   | |   | └─IdentifierSyntax
//@[09:0013) |   | |   |   └─Token(Identifier) |foos|
//@[13:0014) |   | |   ├─Token(LeftSquare) |[|
//@[14:0019) |   | |   ├─BinaryOperationSyntax
//@[14:0015) |   | |   | ├─VariableAccessSyntax
//@[14:0015) |   | |   | | └─IdentifierSyntax
//@[14:0015) |   | |   | |   └─Token(Identifier) |i|
//@[16:0017) |   | |   | ├─Token(Plus) |+|
//@[18:0019) |   | |   | └─IntegerLiteralSyntax
//@[18:0019) |   | |   |   └─Token(Integer) |2|
//@[19:0020) |   | |   └─Token(RightSquare) |]|
//@[20:0021) |   | ├─Token(NewLine) |\n|
  case7: foos[zeroIndex]
//@[02:0024) |   | ├─ObjectPropertySyntax
//@[02:0007) |   | | ├─IdentifierSyntax
//@[02:0007) |   | | | └─Token(Identifier) |case7|
//@[07:0008) |   | | ├─Token(Colon) |:|
//@[09:0024) |   | | └─ArrayAccessSyntax
//@[09:0013) |   | |   ├─VariableAccessSyntax
//@[09:0013) |   | |   | └─IdentifierSyntax
//@[09:0013) |   | |   |   └─Token(Identifier) |foos|
//@[13:0014) |   | |   ├─Token(LeftSquare) |[|
//@[14:0023) |   | |   ├─VariableAccessSyntax
//@[14:0023) |   | |   | └─IdentifierSyntax
//@[14:0023) |   | |   |   └─Token(Identifier) |zeroIndex|
//@[23:0024) |   | |   └─Token(RightSquare) |]|
//@[24:0025) |   | ├─Token(NewLine) |\n|
  case8: foos[otherIndex]
//@[02:0025) |   | ├─ObjectPropertySyntax
//@[02:0007) |   | | ├─IdentifierSyntax
//@[02:0007) |   | | | └─Token(Identifier) |case8|
//@[07:0008) |   | | ├─Token(Colon) |:|
//@[09:0025) |   | | └─ArrayAccessSyntax
//@[09:0013) |   | |   ├─VariableAccessSyntax
//@[09:0013) |   | |   | └─IdentifierSyntax
//@[09:0013) |   | |   |   └─Token(Identifier) |foos|
//@[13:0014) |   | |   ├─Token(LeftSquare) |[|
//@[14:0024) |   | |   ├─VariableAccessSyntax
//@[14:0024) |   | |   | └─IdentifierSyntax
//@[14:0024) |   | |   |   └─Token(Identifier) |otherIndex|
//@[24:0025) |   | |   └─Token(RightSquare) |]|
//@[25:0026) |   | ├─Token(NewLine) |\n|
  case9: foo.properties
//@[02:0023) |   | ├─ObjectPropertySyntax
//@[02:0007) |   | | ├─IdentifierSyntax
//@[02:0007) |   | | | └─Token(Identifier) |case9|
//@[07:0008) |   | | ├─Token(Colon) |:|
//@[09:0023) |   | | └─PropertyAccessSyntax
//@[09:0012) |   | |   ├─VariableAccessSyntax
//@[09:0012) |   | |   | └─IdentifierSyntax
//@[09:0012) |   | |   |   └─Token(Identifier) |foo|
//@[12:0013) |   | |   ├─Token(Dot) |.|
//@[13:0023) |   | |   └─IdentifierSyntax
//@[13:0023) |   | |     └─Token(Identifier) |properties|
//@[23:0024) |   | ├─Token(NewLine) |\n|
  case10: existingFoo.properties
//@[02:0032) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case10|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0032) |   | | └─PropertyAccessSyntax
//@[10:0021) |   | |   ├─VariableAccessSyntax
//@[10:0021) |   | |   | └─IdentifierSyntax
//@[10:0021) |   | |   |   └─Token(Identifier) |existingFoo|
//@[21:0022) |   | |   ├─Token(Dot) |.|
//@[22:0032) |   | |   └─IdentifierSyntax
//@[22:0032) |   | |     └─Token(Identifier) |properties|
//@[32:0033) |   | ├─Token(NewLine) |\n|
  case11: foo::fooChild.properties
//@[02:0034) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case11|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0034) |   | | └─PropertyAccessSyntax
//@[10:0023) |   | |   ├─ResourceAccessSyntax
//@[10:0013) |   | |   | ├─VariableAccessSyntax
//@[10:0013) |   | |   | | └─IdentifierSyntax
//@[10:0013) |   | |   | |   └─Token(Identifier) |foo|
//@[13:0015) |   | |   | ├─Token(DoubleColon) |::|
//@[15:0023) |   | |   | └─IdentifierSyntax
//@[15:0023) |   | |   |   └─Token(Identifier) |fooChild|
//@[23:0024) |   | |   ├─Token(Dot) |.|
//@[24:0034) |   | |   └─IdentifierSyntax
//@[24:0034) |   | |     └─Token(Identifier) |properties|
//@[34:0035) |   | ├─Token(NewLine) |\n|
  case12: foos[0].properties
//@[02:0028) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case12|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0028) |   | | └─PropertyAccessSyntax
//@[10:0017) |   | |   ├─ArrayAccessSyntax
//@[10:0014) |   | |   | ├─VariableAccessSyntax
//@[10:0014) |   | |   | | └─IdentifierSyntax
//@[10:0014) |   | |   | |   └─Token(Identifier) |foos|
//@[14:0015) |   | |   | ├─Token(LeftSquare) |[|
//@[15:0016) |   | |   | ├─IntegerLiteralSyntax
//@[15:0016) |   | |   | | └─Token(Integer) |0|
//@[16:0017) |   | |   | └─Token(RightSquare) |]|
//@[17:0018) |   | |   ├─Token(Dot) |.|
//@[18:0028) |   | |   └─IdentifierSyntax
//@[18:0028) |   | |     └─Token(Identifier) |properties|
//@[28:0029) |   | ├─Token(NewLine) |\n|
  case13: foos[i].properties
//@[02:0028) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case13|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0028) |   | | └─PropertyAccessSyntax
//@[10:0017) |   | |   ├─ArrayAccessSyntax
//@[10:0014) |   | |   | ├─VariableAccessSyntax
//@[10:0014) |   | |   | | └─IdentifierSyntax
//@[10:0014) |   | |   | |   └─Token(Identifier) |foos|
//@[14:0015) |   | |   | ├─Token(LeftSquare) |[|
//@[15:0016) |   | |   | ├─VariableAccessSyntax
//@[15:0016) |   | |   | | └─IdentifierSyntax
//@[15:0016) |   | |   | |   └─Token(Identifier) |i|
//@[16:0017) |   | |   | └─Token(RightSquare) |]|
//@[17:0018) |   | |   ├─Token(Dot) |.|
//@[18:0028) |   | |   └─IdentifierSyntax
//@[18:0028) |   | |     └─Token(Identifier) |properties|
//@[28:0029) |   | ├─Token(NewLine) |\n|
  case14: foos[i + 2].properties
//@[02:0032) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case14|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0032) |   | | └─PropertyAccessSyntax
//@[10:0021) |   | |   ├─ArrayAccessSyntax
//@[10:0014) |   | |   | ├─VariableAccessSyntax
//@[10:0014) |   | |   | | └─IdentifierSyntax
//@[10:0014) |   | |   | |   └─Token(Identifier) |foos|
//@[14:0015) |   | |   | ├─Token(LeftSquare) |[|
//@[15:0020) |   | |   | ├─BinaryOperationSyntax
//@[15:0016) |   | |   | | ├─VariableAccessSyntax
//@[15:0016) |   | |   | | | └─IdentifierSyntax
//@[15:0016) |   | |   | | |   └─Token(Identifier) |i|
//@[17:0018) |   | |   | | ├─Token(Plus) |+|
//@[19:0020) |   | |   | | └─IntegerLiteralSyntax
//@[19:0020) |   | |   | |   └─Token(Integer) |2|
//@[20:0021) |   | |   | └─Token(RightSquare) |]|
//@[21:0022) |   | |   ├─Token(Dot) |.|
//@[22:0032) |   | |   └─IdentifierSyntax
//@[22:0032) |   | |     └─Token(Identifier) |properties|
//@[32:0033) |   | ├─Token(NewLine) |\n|
  case15: foos[zeroIndex].properties
//@[02:0036) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case15|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0036) |   | | └─PropertyAccessSyntax
//@[10:0025) |   | |   ├─ArrayAccessSyntax
//@[10:0014) |   | |   | ├─VariableAccessSyntax
//@[10:0014) |   | |   | | └─IdentifierSyntax
//@[10:0014) |   | |   | |   └─Token(Identifier) |foos|
//@[14:0015) |   | |   | ├─Token(LeftSquare) |[|
//@[15:0024) |   | |   | ├─VariableAccessSyntax
//@[15:0024) |   | |   | | └─IdentifierSyntax
//@[15:0024) |   | |   | |   └─Token(Identifier) |zeroIndex|
//@[24:0025) |   | |   | └─Token(RightSquare) |]|
//@[25:0026) |   | |   ├─Token(Dot) |.|
//@[26:0036) |   | |   └─IdentifierSyntax
//@[26:0036) |   | |     └─Token(Identifier) |properties|
//@[36:0037) |   | ├─Token(NewLine) |\n|
  case16: foos[otherIndex].properties
//@[02:0037) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case16|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0037) |   | | └─PropertyAccessSyntax
//@[10:0026) |   | |   ├─ArrayAccessSyntax
//@[10:0014) |   | |   | ├─VariableAccessSyntax
//@[10:0014) |   | |   | | └─IdentifierSyntax
//@[10:0014) |   | |   | |   └─Token(Identifier) |foos|
//@[14:0015) |   | |   | ├─Token(LeftSquare) |[|
//@[15:0025) |   | |   | ├─VariableAccessSyntax
//@[15:0025) |   | |   | | └─IdentifierSyntax
//@[15:0025) |   | |   | |   └─Token(Identifier) |otherIndex|
//@[25:0026) |   | |   | └─Token(RightSquare) |]|
//@[26:0027) |   | |   ├─Token(Dot) |.|
//@[27:0037) |   | |   └─IdentifierSyntax
//@[27:0037) |   | |     └─Token(Identifier) |properties|
//@[37:0038) |   | ├─Token(NewLine) |\n|
  case17: foo.properties.accessTier
//@[02:0035) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case17|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0035) |   | | └─PropertyAccessSyntax
//@[10:0024) |   | |   ├─PropertyAccessSyntax
//@[10:0013) |   | |   | ├─VariableAccessSyntax
//@[10:0013) |   | |   | | └─IdentifierSyntax
//@[10:0013) |   | |   | |   └─Token(Identifier) |foo|
//@[13:0014) |   | |   | ├─Token(Dot) |.|
//@[14:0024) |   | |   | └─IdentifierSyntax
//@[14:0024) |   | |   |   └─Token(Identifier) |properties|
//@[24:0025) |   | |   ├─Token(Dot) |.|
//@[25:0035) |   | |   └─IdentifierSyntax
//@[25:0035) |   | |     └─Token(Identifier) |accessTier|
//@[35:0036) |   | ├─Token(NewLine) |\n|
  case18: existingFoo.properties.accessTier
//@[02:0043) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case18|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0043) |   | | └─PropertyAccessSyntax
//@[10:0032) |   | |   ├─PropertyAccessSyntax
//@[10:0021) |   | |   | ├─VariableAccessSyntax
//@[10:0021) |   | |   | | └─IdentifierSyntax
//@[10:0021) |   | |   | |   └─Token(Identifier) |existingFoo|
//@[21:0022) |   | |   | ├─Token(Dot) |.|
//@[22:0032) |   | |   | └─IdentifierSyntax
//@[22:0032) |   | |   |   └─Token(Identifier) |properties|
//@[32:0033) |   | |   ├─Token(Dot) |.|
//@[33:0043) |   | |   └─IdentifierSyntax
//@[33:0043) |   | |     └─Token(Identifier) |accessTier|
//@[43:0044) |   | ├─Token(NewLine) |\n|
  case19: foo::fooChild.properties.accessTier
//@[02:0045) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case19|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0045) |   | | └─PropertyAccessSyntax
//@[10:0034) |   | |   ├─PropertyAccessSyntax
//@[10:0023) |   | |   | ├─ResourceAccessSyntax
//@[10:0013) |   | |   | | ├─VariableAccessSyntax
//@[10:0013) |   | |   | | | └─IdentifierSyntax
//@[10:0013) |   | |   | | |   └─Token(Identifier) |foo|
//@[13:0015) |   | |   | | ├─Token(DoubleColon) |::|
//@[15:0023) |   | |   | | └─IdentifierSyntax
//@[15:0023) |   | |   | |   └─Token(Identifier) |fooChild|
//@[23:0024) |   | |   | ├─Token(Dot) |.|
//@[24:0034) |   | |   | └─IdentifierSyntax
//@[24:0034) |   | |   |   └─Token(Identifier) |properties|
//@[34:0035) |   | |   ├─Token(Dot) |.|
//@[35:0045) |   | |   └─IdentifierSyntax
//@[35:0045) |   | |     └─Token(Identifier) |accessTier|
//@[45:0046) |   | ├─Token(NewLine) |\n|
  case20: foos[0].properties.accessTier
//@[02:0039) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case20|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0039) |   | | └─PropertyAccessSyntax
//@[10:0028) |   | |   ├─PropertyAccessSyntax
//@[10:0017) |   | |   | ├─ArrayAccessSyntax
//@[10:0014) |   | |   | | ├─VariableAccessSyntax
//@[10:0014) |   | |   | | | └─IdentifierSyntax
//@[10:0014) |   | |   | | |   └─Token(Identifier) |foos|
//@[14:0015) |   | |   | | ├─Token(LeftSquare) |[|
//@[15:0016) |   | |   | | ├─IntegerLiteralSyntax
//@[15:0016) |   | |   | | | └─Token(Integer) |0|
//@[16:0017) |   | |   | | └─Token(RightSquare) |]|
//@[17:0018) |   | |   | ├─Token(Dot) |.|
//@[18:0028) |   | |   | └─IdentifierSyntax
//@[18:0028) |   | |   |   └─Token(Identifier) |properties|
//@[28:0029) |   | |   ├─Token(Dot) |.|
//@[29:0039) |   | |   └─IdentifierSyntax
//@[29:0039) |   | |     └─Token(Identifier) |accessTier|
//@[39:0040) |   | ├─Token(NewLine) |\n|
  case21: foos[i].properties.accessTier
//@[02:0039) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case21|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0039) |   | | └─PropertyAccessSyntax
//@[10:0028) |   | |   ├─PropertyAccessSyntax
//@[10:0017) |   | |   | ├─ArrayAccessSyntax
//@[10:0014) |   | |   | | ├─VariableAccessSyntax
//@[10:0014) |   | |   | | | └─IdentifierSyntax
//@[10:0014) |   | |   | | |   └─Token(Identifier) |foos|
//@[14:0015) |   | |   | | ├─Token(LeftSquare) |[|
//@[15:0016) |   | |   | | ├─VariableAccessSyntax
//@[15:0016) |   | |   | | | └─IdentifierSyntax
//@[15:0016) |   | |   | | |   └─Token(Identifier) |i|
//@[16:0017) |   | |   | | └─Token(RightSquare) |]|
//@[17:0018) |   | |   | ├─Token(Dot) |.|
//@[18:0028) |   | |   | └─IdentifierSyntax
//@[18:0028) |   | |   |   └─Token(Identifier) |properties|
//@[28:0029) |   | |   ├─Token(Dot) |.|
//@[29:0039) |   | |   └─IdentifierSyntax
//@[29:0039) |   | |     └─Token(Identifier) |accessTier|
//@[39:0040) |   | ├─Token(NewLine) |\n|
  case22: foos[i + 2].properties.accessTier
//@[02:0043) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case22|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0043) |   | | └─PropertyAccessSyntax
//@[10:0032) |   | |   ├─PropertyAccessSyntax
//@[10:0021) |   | |   | ├─ArrayAccessSyntax
//@[10:0014) |   | |   | | ├─VariableAccessSyntax
//@[10:0014) |   | |   | | | └─IdentifierSyntax
//@[10:0014) |   | |   | | |   └─Token(Identifier) |foos|
//@[14:0015) |   | |   | | ├─Token(LeftSquare) |[|
//@[15:0020) |   | |   | | ├─BinaryOperationSyntax
//@[15:0016) |   | |   | | | ├─VariableAccessSyntax
//@[15:0016) |   | |   | | | | └─IdentifierSyntax
//@[15:0016) |   | |   | | | |   └─Token(Identifier) |i|
//@[17:0018) |   | |   | | | ├─Token(Plus) |+|
//@[19:0020) |   | |   | | | └─IntegerLiteralSyntax
//@[19:0020) |   | |   | | |   └─Token(Integer) |2|
//@[20:0021) |   | |   | | └─Token(RightSquare) |]|
//@[21:0022) |   | |   | ├─Token(Dot) |.|
//@[22:0032) |   | |   | └─IdentifierSyntax
//@[22:0032) |   | |   |   └─Token(Identifier) |properties|
//@[32:0033) |   | |   ├─Token(Dot) |.|
//@[33:0043) |   | |   └─IdentifierSyntax
//@[33:0043) |   | |     └─Token(Identifier) |accessTier|
//@[43:0044) |   | ├─Token(NewLine) |\n|
  case23: foos[zeroIndex].properties.accessTier
//@[02:0047) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case23|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0047) |   | | └─PropertyAccessSyntax
//@[10:0036) |   | |   ├─PropertyAccessSyntax
//@[10:0025) |   | |   | ├─ArrayAccessSyntax
//@[10:0014) |   | |   | | ├─VariableAccessSyntax
//@[10:0014) |   | |   | | | └─IdentifierSyntax
//@[10:0014) |   | |   | | |   └─Token(Identifier) |foos|
//@[14:0015) |   | |   | | ├─Token(LeftSquare) |[|
//@[15:0024) |   | |   | | ├─VariableAccessSyntax
//@[15:0024) |   | |   | | | └─IdentifierSyntax
//@[15:0024) |   | |   | | |   └─Token(Identifier) |zeroIndex|
//@[24:0025) |   | |   | | └─Token(RightSquare) |]|
//@[25:0026) |   | |   | ├─Token(Dot) |.|
//@[26:0036) |   | |   | └─IdentifierSyntax
//@[26:0036) |   | |   |   └─Token(Identifier) |properties|
//@[36:0037) |   | |   ├─Token(Dot) |.|
//@[37:0047) |   | |   └─IdentifierSyntax
//@[37:0047) |   | |     └─Token(Identifier) |accessTier|
//@[47:0048) |   | ├─Token(NewLine) |\n|
  case24: foos[otherIndex].properties.accessTier
//@[02:0048) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case24|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0048) |   | | └─PropertyAccessSyntax
//@[10:0037) |   | |   ├─PropertyAccessSyntax
//@[10:0026) |   | |   | ├─ArrayAccessSyntax
//@[10:0014) |   | |   | | ├─VariableAccessSyntax
//@[10:0014) |   | |   | | | └─IdentifierSyntax
//@[10:0014) |   | |   | | |   └─Token(Identifier) |foos|
//@[14:0015) |   | |   | | ├─Token(LeftSquare) |[|
//@[15:0025) |   | |   | | ├─VariableAccessSyntax
//@[15:0025) |   | |   | | | └─IdentifierSyntax
//@[15:0025) |   | |   | | |   └─Token(Identifier) |otherIndex|
//@[25:0026) |   | |   | | └─Token(RightSquare) |]|
//@[26:0027) |   | |   | ├─Token(Dot) |.|
//@[27:0037) |   | |   | └─IdentifierSyntax
//@[27:0037) |   | |   |   └─Token(Identifier) |properties|
//@[37:0038) |   | |   ├─Token(Dot) |.|
//@[38:0048) |   | |   └─IdentifierSyntax
//@[38:0048) |   | |     └─Token(Identifier) |accessTier|
//@[48:0049) |   | ├─Token(NewLine) |\n|
  case25: foo['properties']
//@[02:0027) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case25|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0027) |   | | └─ArrayAccessSyntax
//@[10:0013) |   | |   ├─VariableAccessSyntax
//@[10:0013) |   | |   | └─IdentifierSyntax
//@[10:0013) |   | |   |   └─Token(Identifier) |foo|
//@[13:0014) |   | |   ├─Token(LeftSquare) |[|
//@[14:0026) |   | |   ├─StringSyntax
//@[14:0026) |   | |   | └─Token(StringComplete) |'properties'|
//@[26:0027) |   | |   └─Token(RightSquare) |]|
//@[27:0028) |   | ├─Token(NewLine) |\n|
  case26: existingFoo['properties']
//@[02:0035) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case26|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0035) |   | | └─ArrayAccessSyntax
//@[10:0021) |   | |   ├─VariableAccessSyntax
//@[10:0021) |   | |   | └─IdentifierSyntax
//@[10:0021) |   | |   |   └─Token(Identifier) |existingFoo|
//@[21:0022) |   | |   ├─Token(LeftSquare) |[|
//@[22:0034) |   | |   ├─StringSyntax
//@[22:0034) |   | |   | └─Token(StringComplete) |'properties'|
//@[34:0035) |   | |   └─Token(RightSquare) |]|
//@[35:0036) |   | ├─Token(NewLine) |\n|
  case27: foo::fooChild['properties']
//@[02:0037) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case27|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0037) |   | | └─ArrayAccessSyntax
//@[10:0023) |   | |   ├─ResourceAccessSyntax
//@[10:0013) |   | |   | ├─VariableAccessSyntax
//@[10:0013) |   | |   | | └─IdentifierSyntax
//@[10:0013) |   | |   | |   └─Token(Identifier) |foo|
//@[13:0015) |   | |   | ├─Token(DoubleColon) |::|
//@[15:0023) |   | |   | └─IdentifierSyntax
//@[15:0023) |   | |   |   └─Token(Identifier) |fooChild|
//@[23:0024) |   | |   ├─Token(LeftSquare) |[|
//@[24:0036) |   | |   ├─StringSyntax
//@[24:0036) |   | |   | └─Token(StringComplete) |'properties'|
//@[36:0037) |   | |   └─Token(RightSquare) |]|
//@[37:0038) |   | ├─Token(NewLine) |\n|
  case28: foos[0]['properties']
//@[02:0031) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case28|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0031) |   | | └─ArrayAccessSyntax
//@[10:0017) |   | |   ├─ArrayAccessSyntax
//@[10:0014) |   | |   | ├─VariableAccessSyntax
//@[10:0014) |   | |   | | └─IdentifierSyntax
//@[10:0014) |   | |   | |   └─Token(Identifier) |foos|
//@[14:0015) |   | |   | ├─Token(LeftSquare) |[|
//@[15:0016) |   | |   | ├─IntegerLiteralSyntax
//@[15:0016) |   | |   | | └─Token(Integer) |0|
//@[16:0017) |   | |   | └─Token(RightSquare) |]|
//@[17:0018) |   | |   ├─Token(LeftSquare) |[|
//@[18:0030) |   | |   ├─StringSyntax
//@[18:0030) |   | |   | └─Token(StringComplete) |'properties'|
//@[30:0031) |   | |   └─Token(RightSquare) |]|
//@[31:0032) |   | ├─Token(NewLine) |\n|
  case29: foos[i]['properties']
//@[02:0031) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case29|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0031) |   | | └─ArrayAccessSyntax
//@[10:0017) |   | |   ├─ArrayAccessSyntax
//@[10:0014) |   | |   | ├─VariableAccessSyntax
//@[10:0014) |   | |   | | └─IdentifierSyntax
//@[10:0014) |   | |   | |   └─Token(Identifier) |foos|
//@[14:0015) |   | |   | ├─Token(LeftSquare) |[|
//@[15:0016) |   | |   | ├─VariableAccessSyntax
//@[15:0016) |   | |   | | └─IdentifierSyntax
//@[15:0016) |   | |   | |   └─Token(Identifier) |i|
//@[16:0017) |   | |   | └─Token(RightSquare) |]|
//@[17:0018) |   | |   ├─Token(LeftSquare) |[|
//@[18:0030) |   | |   ├─StringSyntax
//@[18:0030) |   | |   | └─Token(StringComplete) |'properties'|
//@[30:0031) |   | |   └─Token(RightSquare) |]|
//@[31:0032) |   | ├─Token(NewLine) |\n|
  case30: foos[i + 2]['properties']
//@[02:0035) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case30|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0035) |   | | └─ArrayAccessSyntax
//@[10:0021) |   | |   ├─ArrayAccessSyntax
//@[10:0014) |   | |   | ├─VariableAccessSyntax
//@[10:0014) |   | |   | | └─IdentifierSyntax
//@[10:0014) |   | |   | |   └─Token(Identifier) |foos|
//@[14:0015) |   | |   | ├─Token(LeftSquare) |[|
//@[15:0020) |   | |   | ├─BinaryOperationSyntax
//@[15:0016) |   | |   | | ├─VariableAccessSyntax
//@[15:0016) |   | |   | | | └─IdentifierSyntax
//@[15:0016) |   | |   | | |   └─Token(Identifier) |i|
//@[17:0018) |   | |   | | ├─Token(Plus) |+|
//@[19:0020) |   | |   | | └─IntegerLiteralSyntax
//@[19:0020) |   | |   | |   └─Token(Integer) |2|
//@[20:0021) |   | |   | └─Token(RightSquare) |]|
//@[21:0022) |   | |   ├─Token(LeftSquare) |[|
//@[22:0034) |   | |   ├─StringSyntax
//@[22:0034) |   | |   | └─Token(StringComplete) |'properties'|
//@[34:0035) |   | |   └─Token(RightSquare) |]|
//@[35:0036) |   | ├─Token(NewLine) |\n|
  case31: foos[zeroIndex]['properties']
//@[02:0039) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case31|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0039) |   | | └─ArrayAccessSyntax
//@[10:0025) |   | |   ├─ArrayAccessSyntax
//@[10:0014) |   | |   | ├─VariableAccessSyntax
//@[10:0014) |   | |   | | └─IdentifierSyntax
//@[10:0014) |   | |   | |   └─Token(Identifier) |foos|
//@[14:0015) |   | |   | ├─Token(LeftSquare) |[|
//@[15:0024) |   | |   | ├─VariableAccessSyntax
//@[15:0024) |   | |   | | └─IdentifierSyntax
//@[15:0024) |   | |   | |   └─Token(Identifier) |zeroIndex|
//@[24:0025) |   | |   | └─Token(RightSquare) |]|
//@[25:0026) |   | |   ├─Token(LeftSquare) |[|
//@[26:0038) |   | |   ├─StringSyntax
//@[26:0038) |   | |   | └─Token(StringComplete) |'properties'|
//@[38:0039) |   | |   └─Token(RightSquare) |]|
//@[39:0040) |   | ├─Token(NewLine) |\n|
  case32: foos[otherIndex]['properties']
//@[02:0040) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case32|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0040) |   | | └─ArrayAccessSyntax
//@[10:0026) |   | |   ├─ArrayAccessSyntax
//@[10:0014) |   | |   | ├─VariableAccessSyntax
//@[10:0014) |   | |   | | └─IdentifierSyntax
//@[10:0014) |   | |   | |   └─Token(Identifier) |foos|
//@[14:0015) |   | |   | ├─Token(LeftSquare) |[|
//@[15:0025) |   | |   | ├─VariableAccessSyntax
//@[15:0025) |   | |   | | └─IdentifierSyntax
//@[15:0025) |   | |   | |   └─Token(Identifier) |otherIndex|
//@[25:0026) |   | |   | └─Token(RightSquare) |]|
//@[26:0027) |   | |   ├─Token(LeftSquare) |[|
//@[27:0039) |   | |   ├─StringSyntax
//@[27:0039) |   | |   | └─Token(StringComplete) |'properties'|
//@[39:0040) |   | |   └─Token(RightSquare) |]|
//@[40:0041) |   | ├─Token(NewLine) |\n|
  case33: foo['properties']['accessTier']
//@[02:0041) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case33|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0041) |   | | └─ArrayAccessSyntax
//@[10:0027) |   | |   ├─ArrayAccessSyntax
//@[10:0013) |   | |   | ├─VariableAccessSyntax
//@[10:0013) |   | |   | | └─IdentifierSyntax
//@[10:0013) |   | |   | |   └─Token(Identifier) |foo|
//@[13:0014) |   | |   | ├─Token(LeftSquare) |[|
//@[14:0026) |   | |   | ├─StringSyntax
//@[14:0026) |   | |   | | └─Token(StringComplete) |'properties'|
//@[26:0027) |   | |   | └─Token(RightSquare) |]|
//@[27:0028) |   | |   ├─Token(LeftSquare) |[|
//@[28:0040) |   | |   ├─StringSyntax
//@[28:0040) |   | |   | └─Token(StringComplete) |'accessTier'|
//@[40:0041) |   | |   └─Token(RightSquare) |]|
//@[41:0042) |   | ├─Token(NewLine) |\n|
  case34: existingFoo['properties']['accessTier']
//@[02:0049) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case34|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0049) |   | | └─ArrayAccessSyntax
//@[10:0035) |   | |   ├─ArrayAccessSyntax
//@[10:0021) |   | |   | ├─VariableAccessSyntax
//@[10:0021) |   | |   | | └─IdentifierSyntax
//@[10:0021) |   | |   | |   └─Token(Identifier) |existingFoo|
//@[21:0022) |   | |   | ├─Token(LeftSquare) |[|
//@[22:0034) |   | |   | ├─StringSyntax
//@[22:0034) |   | |   | | └─Token(StringComplete) |'properties'|
//@[34:0035) |   | |   | └─Token(RightSquare) |]|
//@[35:0036) |   | |   ├─Token(LeftSquare) |[|
//@[36:0048) |   | |   ├─StringSyntax
//@[36:0048) |   | |   | └─Token(StringComplete) |'accessTier'|
//@[48:0049) |   | |   └─Token(RightSquare) |]|
//@[49:0050) |   | ├─Token(NewLine) |\n|
  case35: foo::fooChild['properties']['accessTier']
//@[02:0051) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case35|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0051) |   | | └─ArrayAccessSyntax
//@[10:0037) |   | |   ├─ArrayAccessSyntax
//@[10:0023) |   | |   | ├─ResourceAccessSyntax
//@[10:0013) |   | |   | | ├─VariableAccessSyntax
//@[10:0013) |   | |   | | | └─IdentifierSyntax
//@[10:0013) |   | |   | | |   └─Token(Identifier) |foo|
//@[13:0015) |   | |   | | ├─Token(DoubleColon) |::|
//@[15:0023) |   | |   | | └─IdentifierSyntax
//@[15:0023) |   | |   | |   └─Token(Identifier) |fooChild|
//@[23:0024) |   | |   | ├─Token(LeftSquare) |[|
//@[24:0036) |   | |   | ├─StringSyntax
//@[24:0036) |   | |   | | └─Token(StringComplete) |'properties'|
//@[36:0037) |   | |   | └─Token(RightSquare) |]|
//@[37:0038) |   | |   ├─Token(LeftSquare) |[|
//@[38:0050) |   | |   ├─StringSyntax
//@[38:0050) |   | |   | └─Token(StringComplete) |'accessTier'|
//@[50:0051) |   | |   └─Token(RightSquare) |]|
//@[51:0052) |   | ├─Token(NewLine) |\n|
  case36: foos[0]['properties']['accessTier']
//@[02:0045) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case36|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0045) |   | | └─ArrayAccessSyntax
//@[10:0031) |   | |   ├─ArrayAccessSyntax
//@[10:0017) |   | |   | ├─ArrayAccessSyntax
//@[10:0014) |   | |   | | ├─VariableAccessSyntax
//@[10:0014) |   | |   | | | └─IdentifierSyntax
//@[10:0014) |   | |   | | |   └─Token(Identifier) |foos|
//@[14:0015) |   | |   | | ├─Token(LeftSquare) |[|
//@[15:0016) |   | |   | | ├─IntegerLiteralSyntax
//@[15:0016) |   | |   | | | └─Token(Integer) |0|
//@[16:0017) |   | |   | | └─Token(RightSquare) |]|
//@[17:0018) |   | |   | ├─Token(LeftSquare) |[|
//@[18:0030) |   | |   | ├─StringSyntax
//@[18:0030) |   | |   | | └─Token(StringComplete) |'properties'|
//@[30:0031) |   | |   | └─Token(RightSquare) |]|
//@[31:0032) |   | |   ├─Token(LeftSquare) |[|
//@[32:0044) |   | |   ├─StringSyntax
//@[32:0044) |   | |   | └─Token(StringComplete) |'accessTier'|
//@[44:0045) |   | |   └─Token(RightSquare) |]|
//@[45:0046) |   | ├─Token(NewLine) |\n|
  case37: foos[i]['properties']['accessTier']
//@[02:0045) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case37|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0045) |   | | └─ArrayAccessSyntax
//@[10:0031) |   | |   ├─ArrayAccessSyntax
//@[10:0017) |   | |   | ├─ArrayAccessSyntax
//@[10:0014) |   | |   | | ├─VariableAccessSyntax
//@[10:0014) |   | |   | | | └─IdentifierSyntax
//@[10:0014) |   | |   | | |   └─Token(Identifier) |foos|
//@[14:0015) |   | |   | | ├─Token(LeftSquare) |[|
//@[15:0016) |   | |   | | ├─VariableAccessSyntax
//@[15:0016) |   | |   | | | └─IdentifierSyntax
//@[15:0016) |   | |   | | |   └─Token(Identifier) |i|
//@[16:0017) |   | |   | | └─Token(RightSquare) |]|
//@[17:0018) |   | |   | ├─Token(LeftSquare) |[|
//@[18:0030) |   | |   | ├─StringSyntax
//@[18:0030) |   | |   | | └─Token(StringComplete) |'properties'|
//@[30:0031) |   | |   | └─Token(RightSquare) |]|
//@[31:0032) |   | |   ├─Token(LeftSquare) |[|
//@[32:0044) |   | |   ├─StringSyntax
//@[32:0044) |   | |   | └─Token(StringComplete) |'accessTier'|
//@[44:0045) |   | |   └─Token(RightSquare) |]|
//@[45:0046) |   | ├─Token(NewLine) |\n|
  case38: foos[i + 2]['properties']['accessTier']
//@[02:0049) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case38|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0049) |   | | └─ArrayAccessSyntax
//@[10:0035) |   | |   ├─ArrayAccessSyntax
//@[10:0021) |   | |   | ├─ArrayAccessSyntax
//@[10:0014) |   | |   | | ├─VariableAccessSyntax
//@[10:0014) |   | |   | | | └─IdentifierSyntax
//@[10:0014) |   | |   | | |   └─Token(Identifier) |foos|
//@[14:0015) |   | |   | | ├─Token(LeftSquare) |[|
//@[15:0020) |   | |   | | ├─BinaryOperationSyntax
//@[15:0016) |   | |   | | | ├─VariableAccessSyntax
//@[15:0016) |   | |   | | | | └─IdentifierSyntax
//@[15:0016) |   | |   | | | |   └─Token(Identifier) |i|
//@[17:0018) |   | |   | | | ├─Token(Plus) |+|
//@[19:0020) |   | |   | | | └─IntegerLiteralSyntax
//@[19:0020) |   | |   | | |   └─Token(Integer) |2|
//@[20:0021) |   | |   | | └─Token(RightSquare) |]|
//@[21:0022) |   | |   | ├─Token(LeftSquare) |[|
//@[22:0034) |   | |   | ├─StringSyntax
//@[22:0034) |   | |   | | └─Token(StringComplete) |'properties'|
//@[34:0035) |   | |   | └─Token(RightSquare) |]|
//@[35:0036) |   | |   ├─Token(LeftSquare) |[|
//@[36:0048) |   | |   ├─StringSyntax
//@[36:0048) |   | |   | └─Token(StringComplete) |'accessTier'|
//@[48:0049) |   | |   └─Token(RightSquare) |]|
//@[49:0050) |   | ├─Token(NewLine) |\n|
  case39: foos[zeroIndex]['properties']['accessTier']
//@[02:0053) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case39|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0053) |   | | └─ArrayAccessSyntax
//@[10:0039) |   | |   ├─ArrayAccessSyntax
//@[10:0025) |   | |   | ├─ArrayAccessSyntax
//@[10:0014) |   | |   | | ├─VariableAccessSyntax
//@[10:0014) |   | |   | | | └─IdentifierSyntax
//@[10:0014) |   | |   | | |   └─Token(Identifier) |foos|
//@[14:0015) |   | |   | | ├─Token(LeftSquare) |[|
//@[15:0024) |   | |   | | ├─VariableAccessSyntax
//@[15:0024) |   | |   | | | └─IdentifierSyntax
//@[15:0024) |   | |   | | |   └─Token(Identifier) |zeroIndex|
//@[24:0025) |   | |   | | └─Token(RightSquare) |]|
//@[25:0026) |   | |   | ├─Token(LeftSquare) |[|
//@[26:0038) |   | |   | ├─StringSyntax
//@[26:0038) |   | |   | | └─Token(StringComplete) |'properties'|
//@[38:0039) |   | |   | └─Token(RightSquare) |]|
//@[39:0040) |   | |   ├─Token(LeftSquare) |[|
//@[40:0052) |   | |   ├─StringSyntax
//@[40:0052) |   | |   | └─Token(StringComplete) |'accessTier'|
//@[52:0053) |   | |   └─Token(RightSquare) |]|
//@[53:0054) |   | ├─Token(NewLine) |\n|
  case40: foos[otherIndex]['properties']['accessTier']
//@[02:0054) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case40|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0054) |   | | └─ArrayAccessSyntax
//@[10:0040) |   | |   ├─ArrayAccessSyntax
//@[10:0026) |   | |   | ├─ArrayAccessSyntax
//@[10:0014) |   | |   | | ├─VariableAccessSyntax
//@[10:0014) |   | |   | | | └─IdentifierSyntax
//@[10:0014) |   | |   | | |   └─Token(Identifier) |foos|
//@[14:0015) |   | |   | | ├─Token(LeftSquare) |[|
//@[15:0025) |   | |   | | ├─VariableAccessSyntax
//@[15:0025) |   | |   | | | └─IdentifierSyntax
//@[15:0025) |   | |   | | |   └─Token(Identifier) |otherIndex|
//@[25:0026) |   | |   | | └─Token(RightSquare) |]|
//@[26:0027) |   | |   | ├─Token(LeftSquare) |[|
//@[27:0039) |   | |   | ├─StringSyntax
//@[27:0039) |   | |   | | └─Token(StringComplete) |'properties'|
//@[39:0040) |   | |   | └─Token(RightSquare) |]|
//@[40:0041) |   | |   ├─Token(LeftSquare) |[|
//@[41:0053) |   | |   ├─StringSyntax
//@[41:0053) |   | |   | └─Token(StringComplete) |'accessTier'|
//@[53:0054) |   | |   └─Token(RightSquare) |]|
//@[54:0055) |   | ├─Token(NewLine) |\n|
  case41: foo[propertiesAccessor]
//@[02:0033) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case41|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0033) |   | | └─ArrayAccessSyntax
//@[10:0013) |   | |   ├─VariableAccessSyntax
//@[10:0013) |   | |   | └─IdentifierSyntax
//@[10:0013) |   | |   |   └─Token(Identifier) |foo|
//@[13:0014) |   | |   ├─Token(LeftSquare) |[|
//@[14:0032) |   | |   ├─VariableAccessSyntax
//@[14:0032) |   | |   | └─IdentifierSyntax
//@[14:0032) |   | |   |   └─Token(Identifier) |propertiesAccessor|
//@[32:0033) |   | |   └─Token(RightSquare) |]|
//@[33:0034) |   | ├─Token(NewLine) |\n|
  case42: existingFoo[propertiesAccessor]
//@[02:0041) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case42|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0041) |   | | └─ArrayAccessSyntax
//@[10:0021) |   | |   ├─VariableAccessSyntax
//@[10:0021) |   | |   | └─IdentifierSyntax
//@[10:0021) |   | |   |   └─Token(Identifier) |existingFoo|
//@[21:0022) |   | |   ├─Token(LeftSquare) |[|
//@[22:0040) |   | |   ├─VariableAccessSyntax
//@[22:0040) |   | |   | └─IdentifierSyntax
//@[22:0040) |   | |   |   └─Token(Identifier) |propertiesAccessor|
//@[40:0041) |   | |   └─Token(RightSquare) |]|
//@[41:0042) |   | ├─Token(NewLine) |\n|
  case43: foo::fooChild[propertiesAccessor]
//@[02:0043) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case43|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0043) |   | | └─ArrayAccessSyntax
//@[10:0023) |   | |   ├─ResourceAccessSyntax
//@[10:0013) |   | |   | ├─VariableAccessSyntax
//@[10:0013) |   | |   | | └─IdentifierSyntax
//@[10:0013) |   | |   | |   └─Token(Identifier) |foo|
//@[13:0015) |   | |   | ├─Token(DoubleColon) |::|
//@[15:0023) |   | |   | └─IdentifierSyntax
//@[15:0023) |   | |   |   └─Token(Identifier) |fooChild|
//@[23:0024) |   | |   ├─Token(LeftSquare) |[|
//@[24:0042) |   | |   ├─VariableAccessSyntax
//@[24:0042) |   | |   | └─IdentifierSyntax
//@[24:0042) |   | |   |   └─Token(Identifier) |propertiesAccessor|
//@[42:0043) |   | |   └─Token(RightSquare) |]|
//@[43:0044) |   | ├─Token(NewLine) |\n|
  case44: foos[0][propertiesAccessor]
//@[02:0037) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case44|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0037) |   | | └─ArrayAccessSyntax
//@[10:0017) |   | |   ├─ArrayAccessSyntax
//@[10:0014) |   | |   | ├─VariableAccessSyntax
//@[10:0014) |   | |   | | └─IdentifierSyntax
//@[10:0014) |   | |   | |   └─Token(Identifier) |foos|
//@[14:0015) |   | |   | ├─Token(LeftSquare) |[|
//@[15:0016) |   | |   | ├─IntegerLiteralSyntax
//@[15:0016) |   | |   | | └─Token(Integer) |0|
//@[16:0017) |   | |   | └─Token(RightSquare) |]|
//@[17:0018) |   | |   ├─Token(LeftSquare) |[|
//@[18:0036) |   | |   ├─VariableAccessSyntax
//@[18:0036) |   | |   | └─IdentifierSyntax
//@[18:0036) |   | |   |   └─Token(Identifier) |propertiesAccessor|
//@[36:0037) |   | |   └─Token(RightSquare) |]|
//@[37:0038) |   | ├─Token(NewLine) |\n|
  case45: foos[i][propertiesAccessor]
//@[02:0037) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case45|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0037) |   | | └─ArrayAccessSyntax
//@[10:0017) |   | |   ├─ArrayAccessSyntax
//@[10:0014) |   | |   | ├─VariableAccessSyntax
//@[10:0014) |   | |   | | └─IdentifierSyntax
//@[10:0014) |   | |   | |   └─Token(Identifier) |foos|
//@[14:0015) |   | |   | ├─Token(LeftSquare) |[|
//@[15:0016) |   | |   | ├─VariableAccessSyntax
//@[15:0016) |   | |   | | └─IdentifierSyntax
//@[15:0016) |   | |   | |   └─Token(Identifier) |i|
//@[16:0017) |   | |   | └─Token(RightSquare) |]|
//@[17:0018) |   | |   ├─Token(LeftSquare) |[|
//@[18:0036) |   | |   ├─VariableAccessSyntax
//@[18:0036) |   | |   | └─IdentifierSyntax
//@[18:0036) |   | |   |   └─Token(Identifier) |propertiesAccessor|
//@[36:0037) |   | |   └─Token(RightSquare) |]|
//@[37:0038) |   | ├─Token(NewLine) |\n|
  case46: foos[i + 2][propertiesAccessor]
//@[02:0041) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case46|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0041) |   | | └─ArrayAccessSyntax
//@[10:0021) |   | |   ├─ArrayAccessSyntax
//@[10:0014) |   | |   | ├─VariableAccessSyntax
//@[10:0014) |   | |   | | └─IdentifierSyntax
//@[10:0014) |   | |   | |   └─Token(Identifier) |foos|
//@[14:0015) |   | |   | ├─Token(LeftSquare) |[|
//@[15:0020) |   | |   | ├─BinaryOperationSyntax
//@[15:0016) |   | |   | | ├─VariableAccessSyntax
//@[15:0016) |   | |   | | | └─IdentifierSyntax
//@[15:0016) |   | |   | | |   └─Token(Identifier) |i|
//@[17:0018) |   | |   | | ├─Token(Plus) |+|
//@[19:0020) |   | |   | | └─IntegerLiteralSyntax
//@[19:0020) |   | |   | |   └─Token(Integer) |2|
//@[20:0021) |   | |   | └─Token(RightSquare) |]|
//@[21:0022) |   | |   ├─Token(LeftSquare) |[|
//@[22:0040) |   | |   ├─VariableAccessSyntax
//@[22:0040) |   | |   | └─IdentifierSyntax
//@[22:0040) |   | |   |   └─Token(Identifier) |propertiesAccessor|
//@[40:0041) |   | |   └─Token(RightSquare) |]|
//@[41:0042) |   | ├─Token(NewLine) |\n|
  case47: foos[zeroIndex][propertiesAccessor]
//@[02:0045) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case47|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0045) |   | | └─ArrayAccessSyntax
//@[10:0025) |   | |   ├─ArrayAccessSyntax
//@[10:0014) |   | |   | ├─VariableAccessSyntax
//@[10:0014) |   | |   | | └─IdentifierSyntax
//@[10:0014) |   | |   | |   └─Token(Identifier) |foos|
//@[14:0015) |   | |   | ├─Token(LeftSquare) |[|
//@[15:0024) |   | |   | ├─VariableAccessSyntax
//@[15:0024) |   | |   | | └─IdentifierSyntax
//@[15:0024) |   | |   | |   └─Token(Identifier) |zeroIndex|
//@[24:0025) |   | |   | └─Token(RightSquare) |]|
//@[25:0026) |   | |   ├─Token(LeftSquare) |[|
//@[26:0044) |   | |   ├─VariableAccessSyntax
//@[26:0044) |   | |   | └─IdentifierSyntax
//@[26:0044) |   | |   |   └─Token(Identifier) |propertiesAccessor|
//@[44:0045) |   | |   └─Token(RightSquare) |]|
//@[45:0046) |   | ├─Token(NewLine) |\n|
  case48: foos[otherIndex][propertiesAccessor]
//@[02:0046) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case48|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0046) |   | | └─ArrayAccessSyntax
//@[10:0026) |   | |   ├─ArrayAccessSyntax
//@[10:0014) |   | |   | ├─VariableAccessSyntax
//@[10:0014) |   | |   | | └─IdentifierSyntax
//@[10:0014) |   | |   | |   └─Token(Identifier) |foos|
//@[14:0015) |   | |   | ├─Token(LeftSquare) |[|
//@[15:0025) |   | |   | ├─VariableAccessSyntax
//@[15:0025) |   | |   | | └─IdentifierSyntax
//@[15:0025) |   | |   | |   └─Token(Identifier) |otherIndex|
//@[25:0026) |   | |   | └─Token(RightSquare) |]|
//@[26:0027) |   | |   ├─Token(LeftSquare) |[|
//@[27:0045) |   | |   ├─VariableAccessSyntax
//@[27:0045) |   | |   | └─IdentifierSyntax
//@[27:0045) |   | |   |   └─Token(Identifier) |propertiesAccessor|
//@[45:0046) |   | |   └─Token(RightSquare) |]|
//@[46:0047) |   | ├─Token(NewLine) |\n|
  case49: foo[propertiesAccessor][accessTierAccessor]
//@[02:0053) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case49|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0053) |   | | └─ArrayAccessSyntax
//@[10:0033) |   | |   ├─ArrayAccessSyntax
//@[10:0013) |   | |   | ├─VariableAccessSyntax
//@[10:0013) |   | |   | | └─IdentifierSyntax
//@[10:0013) |   | |   | |   └─Token(Identifier) |foo|
//@[13:0014) |   | |   | ├─Token(LeftSquare) |[|
//@[14:0032) |   | |   | ├─VariableAccessSyntax
//@[14:0032) |   | |   | | └─IdentifierSyntax
//@[14:0032) |   | |   | |   └─Token(Identifier) |propertiesAccessor|
//@[32:0033) |   | |   | └─Token(RightSquare) |]|
//@[33:0034) |   | |   ├─Token(LeftSquare) |[|
//@[34:0052) |   | |   ├─VariableAccessSyntax
//@[34:0052) |   | |   | └─IdentifierSyntax
//@[34:0052) |   | |   |   └─Token(Identifier) |accessTierAccessor|
//@[52:0053) |   | |   └─Token(RightSquare) |]|
//@[53:0054) |   | ├─Token(NewLine) |\n|
  case50: existingFoo[propertiesAccessor][accessTierAccessor]
//@[02:0061) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case50|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0061) |   | | └─ArrayAccessSyntax
//@[10:0041) |   | |   ├─ArrayAccessSyntax
//@[10:0021) |   | |   | ├─VariableAccessSyntax
//@[10:0021) |   | |   | | └─IdentifierSyntax
//@[10:0021) |   | |   | |   └─Token(Identifier) |existingFoo|
//@[21:0022) |   | |   | ├─Token(LeftSquare) |[|
//@[22:0040) |   | |   | ├─VariableAccessSyntax
//@[22:0040) |   | |   | | └─IdentifierSyntax
//@[22:0040) |   | |   | |   └─Token(Identifier) |propertiesAccessor|
//@[40:0041) |   | |   | └─Token(RightSquare) |]|
//@[41:0042) |   | |   ├─Token(LeftSquare) |[|
//@[42:0060) |   | |   ├─VariableAccessSyntax
//@[42:0060) |   | |   | └─IdentifierSyntax
//@[42:0060) |   | |   |   └─Token(Identifier) |accessTierAccessor|
//@[60:0061) |   | |   └─Token(RightSquare) |]|
//@[61:0062) |   | ├─Token(NewLine) |\n|
  case51: foo::fooChild[propertiesAccessor][accessTierAccessor]
//@[02:0063) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case51|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0063) |   | | └─ArrayAccessSyntax
//@[10:0043) |   | |   ├─ArrayAccessSyntax
//@[10:0023) |   | |   | ├─ResourceAccessSyntax
//@[10:0013) |   | |   | | ├─VariableAccessSyntax
//@[10:0013) |   | |   | | | └─IdentifierSyntax
//@[10:0013) |   | |   | | |   └─Token(Identifier) |foo|
//@[13:0015) |   | |   | | ├─Token(DoubleColon) |::|
//@[15:0023) |   | |   | | └─IdentifierSyntax
//@[15:0023) |   | |   | |   └─Token(Identifier) |fooChild|
//@[23:0024) |   | |   | ├─Token(LeftSquare) |[|
//@[24:0042) |   | |   | ├─VariableAccessSyntax
//@[24:0042) |   | |   | | └─IdentifierSyntax
//@[24:0042) |   | |   | |   └─Token(Identifier) |propertiesAccessor|
//@[42:0043) |   | |   | └─Token(RightSquare) |]|
//@[43:0044) |   | |   ├─Token(LeftSquare) |[|
//@[44:0062) |   | |   ├─VariableAccessSyntax
//@[44:0062) |   | |   | └─IdentifierSyntax
//@[44:0062) |   | |   |   └─Token(Identifier) |accessTierAccessor|
//@[62:0063) |   | |   └─Token(RightSquare) |]|
//@[63:0064) |   | ├─Token(NewLine) |\n|
  case52: foos[0][propertiesAccessor][accessTierAccessor]
//@[02:0057) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case52|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0057) |   | | └─ArrayAccessSyntax
//@[10:0037) |   | |   ├─ArrayAccessSyntax
//@[10:0017) |   | |   | ├─ArrayAccessSyntax
//@[10:0014) |   | |   | | ├─VariableAccessSyntax
//@[10:0014) |   | |   | | | └─IdentifierSyntax
//@[10:0014) |   | |   | | |   └─Token(Identifier) |foos|
//@[14:0015) |   | |   | | ├─Token(LeftSquare) |[|
//@[15:0016) |   | |   | | ├─IntegerLiteralSyntax
//@[15:0016) |   | |   | | | └─Token(Integer) |0|
//@[16:0017) |   | |   | | └─Token(RightSquare) |]|
//@[17:0018) |   | |   | ├─Token(LeftSquare) |[|
//@[18:0036) |   | |   | ├─VariableAccessSyntax
//@[18:0036) |   | |   | | └─IdentifierSyntax
//@[18:0036) |   | |   | |   └─Token(Identifier) |propertiesAccessor|
//@[36:0037) |   | |   | └─Token(RightSquare) |]|
//@[37:0038) |   | |   ├─Token(LeftSquare) |[|
//@[38:0056) |   | |   ├─VariableAccessSyntax
//@[38:0056) |   | |   | └─IdentifierSyntax
//@[38:0056) |   | |   |   └─Token(Identifier) |accessTierAccessor|
//@[56:0057) |   | |   └─Token(RightSquare) |]|
//@[57:0058) |   | ├─Token(NewLine) |\n|
  case53: foos[i][propertiesAccessor][accessTierAccessor]
//@[02:0057) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case53|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0057) |   | | └─ArrayAccessSyntax
//@[10:0037) |   | |   ├─ArrayAccessSyntax
//@[10:0017) |   | |   | ├─ArrayAccessSyntax
//@[10:0014) |   | |   | | ├─VariableAccessSyntax
//@[10:0014) |   | |   | | | └─IdentifierSyntax
//@[10:0014) |   | |   | | |   └─Token(Identifier) |foos|
//@[14:0015) |   | |   | | ├─Token(LeftSquare) |[|
//@[15:0016) |   | |   | | ├─VariableAccessSyntax
//@[15:0016) |   | |   | | | └─IdentifierSyntax
//@[15:0016) |   | |   | | |   └─Token(Identifier) |i|
//@[16:0017) |   | |   | | └─Token(RightSquare) |]|
//@[17:0018) |   | |   | ├─Token(LeftSquare) |[|
//@[18:0036) |   | |   | ├─VariableAccessSyntax
//@[18:0036) |   | |   | | └─IdentifierSyntax
//@[18:0036) |   | |   | |   └─Token(Identifier) |propertiesAccessor|
//@[36:0037) |   | |   | └─Token(RightSquare) |]|
//@[37:0038) |   | |   ├─Token(LeftSquare) |[|
//@[38:0056) |   | |   ├─VariableAccessSyntax
//@[38:0056) |   | |   | └─IdentifierSyntax
//@[38:0056) |   | |   |   └─Token(Identifier) |accessTierAccessor|
//@[56:0057) |   | |   └─Token(RightSquare) |]|
//@[57:0058) |   | ├─Token(NewLine) |\n|
  case54: foos[i + 2][propertiesAccessor][accessTierAccessor]
//@[02:0061) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case54|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0061) |   | | └─ArrayAccessSyntax
//@[10:0041) |   | |   ├─ArrayAccessSyntax
//@[10:0021) |   | |   | ├─ArrayAccessSyntax
//@[10:0014) |   | |   | | ├─VariableAccessSyntax
//@[10:0014) |   | |   | | | └─IdentifierSyntax
//@[10:0014) |   | |   | | |   └─Token(Identifier) |foos|
//@[14:0015) |   | |   | | ├─Token(LeftSquare) |[|
//@[15:0020) |   | |   | | ├─BinaryOperationSyntax
//@[15:0016) |   | |   | | | ├─VariableAccessSyntax
//@[15:0016) |   | |   | | | | └─IdentifierSyntax
//@[15:0016) |   | |   | | | |   └─Token(Identifier) |i|
//@[17:0018) |   | |   | | | ├─Token(Plus) |+|
//@[19:0020) |   | |   | | | └─IntegerLiteralSyntax
//@[19:0020) |   | |   | | |   └─Token(Integer) |2|
//@[20:0021) |   | |   | | └─Token(RightSquare) |]|
//@[21:0022) |   | |   | ├─Token(LeftSquare) |[|
//@[22:0040) |   | |   | ├─VariableAccessSyntax
//@[22:0040) |   | |   | | └─IdentifierSyntax
//@[22:0040) |   | |   | |   └─Token(Identifier) |propertiesAccessor|
//@[40:0041) |   | |   | └─Token(RightSquare) |]|
//@[41:0042) |   | |   ├─Token(LeftSquare) |[|
//@[42:0060) |   | |   ├─VariableAccessSyntax
//@[42:0060) |   | |   | └─IdentifierSyntax
//@[42:0060) |   | |   |   └─Token(Identifier) |accessTierAccessor|
//@[60:0061) |   | |   └─Token(RightSquare) |]|
//@[61:0062) |   | ├─Token(NewLine) |\n|
  case55: foos[zeroIndex][propertiesAccessor][accessTierAccessor]
//@[02:0065) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case55|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0065) |   | | └─ArrayAccessSyntax
//@[10:0045) |   | |   ├─ArrayAccessSyntax
//@[10:0025) |   | |   | ├─ArrayAccessSyntax
//@[10:0014) |   | |   | | ├─VariableAccessSyntax
//@[10:0014) |   | |   | | | └─IdentifierSyntax
//@[10:0014) |   | |   | | |   └─Token(Identifier) |foos|
//@[14:0015) |   | |   | | ├─Token(LeftSquare) |[|
//@[15:0024) |   | |   | | ├─VariableAccessSyntax
//@[15:0024) |   | |   | | | └─IdentifierSyntax
//@[15:0024) |   | |   | | |   └─Token(Identifier) |zeroIndex|
//@[24:0025) |   | |   | | └─Token(RightSquare) |]|
//@[25:0026) |   | |   | ├─Token(LeftSquare) |[|
//@[26:0044) |   | |   | ├─VariableAccessSyntax
//@[26:0044) |   | |   | | └─IdentifierSyntax
//@[26:0044) |   | |   | |   └─Token(Identifier) |propertiesAccessor|
//@[44:0045) |   | |   | └─Token(RightSquare) |]|
//@[45:0046) |   | |   ├─Token(LeftSquare) |[|
//@[46:0064) |   | |   ├─VariableAccessSyntax
//@[46:0064) |   | |   | └─IdentifierSyntax
//@[46:0064) |   | |   |   └─Token(Identifier) |accessTierAccessor|
//@[64:0065) |   | |   └─Token(RightSquare) |]|
//@[65:0066) |   | ├─Token(NewLine) |\n|
  case56: foos[otherIndex][propertiesAccessor][accessTierAccessor]
//@[02:0066) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case56|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0066) |   | | └─ArrayAccessSyntax
//@[10:0046) |   | |   ├─ArrayAccessSyntax
//@[10:0026) |   | |   | ├─ArrayAccessSyntax
//@[10:0014) |   | |   | | ├─VariableAccessSyntax
//@[10:0014) |   | |   | | | └─IdentifierSyntax
//@[10:0014) |   | |   | | |   └─Token(Identifier) |foos|
//@[14:0015) |   | |   | | ├─Token(LeftSquare) |[|
//@[15:0025) |   | |   | | ├─VariableAccessSyntax
//@[15:0025) |   | |   | | | └─IdentifierSyntax
//@[15:0025) |   | |   | | |   └─Token(Identifier) |otherIndex|
//@[25:0026) |   | |   | | └─Token(RightSquare) |]|
//@[26:0027) |   | |   | ├─Token(LeftSquare) |[|
//@[27:0045) |   | |   | ├─VariableAccessSyntax
//@[27:0045) |   | |   | | └─IdentifierSyntax
//@[27:0045) |   | |   | |   └─Token(Identifier) |propertiesAccessor|
//@[45:0046) |   | |   | └─Token(RightSquare) |]|
//@[46:0047) |   | |   ├─Token(LeftSquare) |[|
//@[47:0065) |   | |   ├─VariableAccessSyntax
//@[47:0065) |   | |   | └─IdentifierSyntax
//@[47:0065) |   | |   |   └─Token(Identifier) |accessTierAccessor|
//@[65:0066) |   | |   └─Token(RightSquare) |]|
//@[66:0067) |   | ├─Token(NewLine) |\n|
  case57: foo[strParam]
//@[02:0023) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case57|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0023) |   | | └─ArrayAccessSyntax
//@[10:0013) |   | |   ├─VariableAccessSyntax
//@[10:0013) |   | |   | └─IdentifierSyntax
//@[10:0013) |   | |   |   └─Token(Identifier) |foo|
//@[13:0014) |   | |   ├─Token(LeftSquare) |[|
//@[14:0022) |   | |   ├─VariableAccessSyntax
//@[14:0022) |   | |   | └─IdentifierSyntax
//@[14:0022) |   | |   |   └─Token(Identifier) |strParam|
//@[22:0023) |   | |   └─Token(RightSquare) |]|
//@[23:0024) |   | ├─Token(NewLine) |\n|
  case58: existingFoo[strParam]
//@[02:0031) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case58|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0031) |   | | └─ArrayAccessSyntax
//@[10:0021) |   | |   ├─VariableAccessSyntax
//@[10:0021) |   | |   | └─IdentifierSyntax
//@[10:0021) |   | |   |   └─Token(Identifier) |existingFoo|
//@[21:0022) |   | |   ├─Token(LeftSquare) |[|
//@[22:0030) |   | |   ├─VariableAccessSyntax
//@[22:0030) |   | |   | └─IdentifierSyntax
//@[22:0030) |   | |   |   └─Token(Identifier) |strParam|
//@[30:0031) |   | |   └─Token(RightSquare) |]|
//@[31:0032) |   | ├─Token(NewLine) |\n|
  case59: foo::fooChild[strParam]
//@[02:0033) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case59|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0033) |   | | └─ArrayAccessSyntax
//@[10:0023) |   | |   ├─ResourceAccessSyntax
//@[10:0013) |   | |   | ├─VariableAccessSyntax
//@[10:0013) |   | |   | | └─IdentifierSyntax
//@[10:0013) |   | |   | |   └─Token(Identifier) |foo|
//@[13:0015) |   | |   | ├─Token(DoubleColon) |::|
//@[15:0023) |   | |   | └─IdentifierSyntax
//@[15:0023) |   | |   |   └─Token(Identifier) |fooChild|
//@[23:0024) |   | |   ├─Token(LeftSquare) |[|
//@[24:0032) |   | |   ├─VariableAccessSyntax
//@[24:0032) |   | |   | └─IdentifierSyntax
//@[24:0032) |   | |   |   └─Token(Identifier) |strParam|
//@[32:0033) |   | |   └─Token(RightSquare) |]|
//@[33:0034) |   | ├─Token(NewLine) |\n|
  case60: foos[0][strParam]
//@[02:0027) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case60|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0027) |   | | └─ArrayAccessSyntax
//@[10:0017) |   | |   ├─ArrayAccessSyntax
//@[10:0014) |   | |   | ├─VariableAccessSyntax
//@[10:0014) |   | |   | | └─IdentifierSyntax
//@[10:0014) |   | |   | |   └─Token(Identifier) |foos|
//@[14:0015) |   | |   | ├─Token(LeftSquare) |[|
//@[15:0016) |   | |   | ├─IntegerLiteralSyntax
//@[15:0016) |   | |   | | └─Token(Integer) |0|
//@[16:0017) |   | |   | └─Token(RightSquare) |]|
//@[17:0018) |   | |   ├─Token(LeftSquare) |[|
//@[18:0026) |   | |   ├─VariableAccessSyntax
//@[18:0026) |   | |   | └─IdentifierSyntax
//@[18:0026) |   | |   |   └─Token(Identifier) |strParam|
//@[26:0027) |   | |   └─Token(RightSquare) |]|
//@[27:0028) |   | ├─Token(NewLine) |\n|
  case61: foos[i][strParam]
//@[02:0027) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case61|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0027) |   | | └─ArrayAccessSyntax
//@[10:0017) |   | |   ├─ArrayAccessSyntax
//@[10:0014) |   | |   | ├─VariableAccessSyntax
//@[10:0014) |   | |   | | └─IdentifierSyntax
//@[10:0014) |   | |   | |   └─Token(Identifier) |foos|
//@[14:0015) |   | |   | ├─Token(LeftSquare) |[|
//@[15:0016) |   | |   | ├─VariableAccessSyntax
//@[15:0016) |   | |   | | └─IdentifierSyntax
//@[15:0016) |   | |   | |   └─Token(Identifier) |i|
//@[16:0017) |   | |   | └─Token(RightSquare) |]|
//@[17:0018) |   | |   ├─Token(LeftSquare) |[|
//@[18:0026) |   | |   ├─VariableAccessSyntax
//@[18:0026) |   | |   | └─IdentifierSyntax
//@[18:0026) |   | |   |   └─Token(Identifier) |strParam|
//@[26:0027) |   | |   └─Token(RightSquare) |]|
//@[27:0028) |   | ├─Token(NewLine) |\n|
  case62: foos[i + 2][strParam]
//@[02:0031) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case62|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0031) |   | | └─ArrayAccessSyntax
//@[10:0021) |   | |   ├─ArrayAccessSyntax
//@[10:0014) |   | |   | ├─VariableAccessSyntax
//@[10:0014) |   | |   | | └─IdentifierSyntax
//@[10:0014) |   | |   | |   └─Token(Identifier) |foos|
//@[14:0015) |   | |   | ├─Token(LeftSquare) |[|
//@[15:0020) |   | |   | ├─BinaryOperationSyntax
//@[15:0016) |   | |   | | ├─VariableAccessSyntax
//@[15:0016) |   | |   | | | └─IdentifierSyntax
//@[15:0016) |   | |   | | |   └─Token(Identifier) |i|
//@[17:0018) |   | |   | | ├─Token(Plus) |+|
//@[19:0020) |   | |   | | └─IntegerLiteralSyntax
//@[19:0020) |   | |   | |   └─Token(Integer) |2|
//@[20:0021) |   | |   | └─Token(RightSquare) |]|
//@[21:0022) |   | |   ├─Token(LeftSquare) |[|
//@[22:0030) |   | |   ├─VariableAccessSyntax
//@[22:0030) |   | |   | └─IdentifierSyntax
//@[22:0030) |   | |   |   └─Token(Identifier) |strParam|
//@[30:0031) |   | |   └─Token(RightSquare) |]|
//@[31:0032) |   | ├─Token(NewLine) |\n|
  case63: foos[zeroIndex][strParam]
//@[02:0035) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case63|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0035) |   | | └─ArrayAccessSyntax
//@[10:0025) |   | |   ├─ArrayAccessSyntax
//@[10:0014) |   | |   | ├─VariableAccessSyntax
//@[10:0014) |   | |   | | └─IdentifierSyntax
//@[10:0014) |   | |   | |   └─Token(Identifier) |foos|
//@[14:0015) |   | |   | ├─Token(LeftSquare) |[|
//@[15:0024) |   | |   | ├─VariableAccessSyntax
//@[15:0024) |   | |   | | └─IdentifierSyntax
//@[15:0024) |   | |   | |   └─Token(Identifier) |zeroIndex|
//@[24:0025) |   | |   | └─Token(RightSquare) |]|
//@[25:0026) |   | |   ├─Token(LeftSquare) |[|
//@[26:0034) |   | |   ├─VariableAccessSyntax
//@[26:0034) |   | |   | └─IdentifierSyntax
//@[26:0034) |   | |   |   └─Token(Identifier) |strParam|
//@[34:0035) |   | |   └─Token(RightSquare) |]|
//@[35:0036) |   | ├─Token(NewLine) |\n|
  case64: foos[otherIndex][strParam]
//@[02:0036) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case64|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0036) |   | | └─ArrayAccessSyntax
//@[10:0026) |   | |   ├─ArrayAccessSyntax
//@[10:0014) |   | |   | ├─VariableAccessSyntax
//@[10:0014) |   | |   | | └─IdentifierSyntax
//@[10:0014) |   | |   | |   └─Token(Identifier) |foos|
//@[14:0015) |   | |   | ├─Token(LeftSquare) |[|
//@[15:0025) |   | |   | ├─VariableAccessSyntax
//@[15:0025) |   | |   | | └─IdentifierSyntax
//@[15:0025) |   | |   | |   └─Token(Identifier) |otherIndex|
//@[25:0026) |   | |   | └─Token(RightSquare) |]|
//@[26:0027) |   | |   ├─Token(LeftSquare) |[|
//@[27:0035) |   | |   ├─VariableAccessSyntax
//@[27:0035) |   | |   | └─IdentifierSyntax
//@[27:0035) |   | |   |   └─Token(Identifier) |strParam|
//@[35:0036) |   | |   └─Token(RightSquare) |]|
//@[36:0037) |   | ├─Token(NewLine) |\n|
  case65: foo['${strParam}']
//@[02:0028) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case65|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0028) |   | | └─ArrayAccessSyntax
//@[10:0013) |   | |   ├─VariableAccessSyntax
//@[10:0013) |   | |   | └─IdentifierSyntax
//@[10:0013) |   | |   |   └─Token(Identifier) |foo|
//@[13:0014) |   | |   ├─Token(LeftSquare) |[|
//@[14:0027) |   | |   ├─StringSyntax
//@[14:0017) |   | |   | ├─Token(StringLeftPiece) |'${|
//@[17:0025) |   | |   | ├─VariableAccessSyntax
//@[17:0025) |   | |   | | └─IdentifierSyntax
//@[17:0025) |   | |   | |   └─Token(Identifier) |strParam|
//@[25:0027) |   | |   | └─Token(StringRightPiece) |}'|
//@[27:0028) |   | |   └─Token(RightSquare) |]|
//@[28:0029) |   | ├─Token(NewLine) |\n|
  case66: existingFoo['${strParam}']
//@[02:0036) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case66|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0036) |   | | └─ArrayAccessSyntax
//@[10:0021) |   | |   ├─VariableAccessSyntax
//@[10:0021) |   | |   | └─IdentifierSyntax
//@[10:0021) |   | |   |   └─Token(Identifier) |existingFoo|
//@[21:0022) |   | |   ├─Token(LeftSquare) |[|
//@[22:0035) |   | |   ├─StringSyntax
//@[22:0025) |   | |   | ├─Token(StringLeftPiece) |'${|
//@[25:0033) |   | |   | ├─VariableAccessSyntax
//@[25:0033) |   | |   | | └─IdentifierSyntax
//@[25:0033) |   | |   | |   └─Token(Identifier) |strParam|
//@[33:0035) |   | |   | └─Token(StringRightPiece) |}'|
//@[35:0036) |   | |   └─Token(RightSquare) |]|
//@[36:0037) |   | ├─Token(NewLine) |\n|
  case67: foo::fooChild['${strParam}']
//@[02:0038) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case67|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0038) |   | | └─ArrayAccessSyntax
//@[10:0023) |   | |   ├─ResourceAccessSyntax
//@[10:0013) |   | |   | ├─VariableAccessSyntax
//@[10:0013) |   | |   | | └─IdentifierSyntax
//@[10:0013) |   | |   | |   └─Token(Identifier) |foo|
//@[13:0015) |   | |   | ├─Token(DoubleColon) |::|
//@[15:0023) |   | |   | └─IdentifierSyntax
//@[15:0023) |   | |   |   └─Token(Identifier) |fooChild|
//@[23:0024) |   | |   ├─Token(LeftSquare) |[|
//@[24:0037) |   | |   ├─StringSyntax
//@[24:0027) |   | |   | ├─Token(StringLeftPiece) |'${|
//@[27:0035) |   | |   | ├─VariableAccessSyntax
//@[27:0035) |   | |   | | └─IdentifierSyntax
//@[27:0035) |   | |   | |   └─Token(Identifier) |strParam|
//@[35:0037) |   | |   | └─Token(StringRightPiece) |}'|
//@[37:0038) |   | |   └─Token(RightSquare) |]|
//@[38:0039) |   | ├─Token(NewLine) |\n|
  case68: foos[0]['${strParam}']
//@[02:0032) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case68|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0032) |   | | └─ArrayAccessSyntax
//@[10:0017) |   | |   ├─ArrayAccessSyntax
//@[10:0014) |   | |   | ├─VariableAccessSyntax
//@[10:0014) |   | |   | | └─IdentifierSyntax
//@[10:0014) |   | |   | |   └─Token(Identifier) |foos|
//@[14:0015) |   | |   | ├─Token(LeftSquare) |[|
//@[15:0016) |   | |   | ├─IntegerLiteralSyntax
//@[15:0016) |   | |   | | └─Token(Integer) |0|
//@[16:0017) |   | |   | └─Token(RightSquare) |]|
//@[17:0018) |   | |   ├─Token(LeftSquare) |[|
//@[18:0031) |   | |   ├─StringSyntax
//@[18:0021) |   | |   | ├─Token(StringLeftPiece) |'${|
//@[21:0029) |   | |   | ├─VariableAccessSyntax
//@[21:0029) |   | |   | | └─IdentifierSyntax
//@[21:0029) |   | |   | |   └─Token(Identifier) |strParam|
//@[29:0031) |   | |   | └─Token(StringRightPiece) |}'|
//@[31:0032) |   | |   └─Token(RightSquare) |]|
//@[32:0033) |   | ├─Token(NewLine) |\n|
  case69: foos[i]['${strParam}']
//@[02:0032) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case69|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0032) |   | | └─ArrayAccessSyntax
//@[10:0017) |   | |   ├─ArrayAccessSyntax
//@[10:0014) |   | |   | ├─VariableAccessSyntax
//@[10:0014) |   | |   | | └─IdentifierSyntax
//@[10:0014) |   | |   | |   └─Token(Identifier) |foos|
//@[14:0015) |   | |   | ├─Token(LeftSquare) |[|
//@[15:0016) |   | |   | ├─VariableAccessSyntax
//@[15:0016) |   | |   | | └─IdentifierSyntax
//@[15:0016) |   | |   | |   └─Token(Identifier) |i|
//@[16:0017) |   | |   | └─Token(RightSquare) |]|
//@[17:0018) |   | |   ├─Token(LeftSquare) |[|
//@[18:0031) |   | |   ├─StringSyntax
//@[18:0021) |   | |   | ├─Token(StringLeftPiece) |'${|
//@[21:0029) |   | |   | ├─VariableAccessSyntax
//@[21:0029) |   | |   | | └─IdentifierSyntax
//@[21:0029) |   | |   | |   └─Token(Identifier) |strParam|
//@[29:0031) |   | |   | └─Token(StringRightPiece) |}'|
//@[31:0032) |   | |   └─Token(RightSquare) |]|
//@[32:0033) |   | ├─Token(NewLine) |\n|
  case70: foos[i + 2]['${strParam}']
//@[02:0036) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case70|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0036) |   | | └─ArrayAccessSyntax
//@[10:0021) |   | |   ├─ArrayAccessSyntax
//@[10:0014) |   | |   | ├─VariableAccessSyntax
//@[10:0014) |   | |   | | └─IdentifierSyntax
//@[10:0014) |   | |   | |   └─Token(Identifier) |foos|
//@[14:0015) |   | |   | ├─Token(LeftSquare) |[|
//@[15:0020) |   | |   | ├─BinaryOperationSyntax
//@[15:0016) |   | |   | | ├─VariableAccessSyntax
//@[15:0016) |   | |   | | | └─IdentifierSyntax
//@[15:0016) |   | |   | | |   └─Token(Identifier) |i|
//@[17:0018) |   | |   | | ├─Token(Plus) |+|
//@[19:0020) |   | |   | | └─IntegerLiteralSyntax
//@[19:0020) |   | |   | |   └─Token(Integer) |2|
//@[20:0021) |   | |   | └─Token(RightSquare) |]|
//@[21:0022) |   | |   ├─Token(LeftSquare) |[|
//@[22:0035) |   | |   ├─StringSyntax
//@[22:0025) |   | |   | ├─Token(StringLeftPiece) |'${|
//@[25:0033) |   | |   | ├─VariableAccessSyntax
//@[25:0033) |   | |   | | └─IdentifierSyntax
//@[25:0033) |   | |   | |   └─Token(Identifier) |strParam|
//@[33:0035) |   | |   | └─Token(StringRightPiece) |}'|
//@[35:0036) |   | |   └─Token(RightSquare) |]|
//@[36:0037) |   | ├─Token(NewLine) |\n|
  case71: foos[zeroIndex]['${strParam}']
//@[02:0040) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case71|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0040) |   | | └─ArrayAccessSyntax
//@[10:0025) |   | |   ├─ArrayAccessSyntax
//@[10:0014) |   | |   | ├─VariableAccessSyntax
//@[10:0014) |   | |   | | └─IdentifierSyntax
//@[10:0014) |   | |   | |   └─Token(Identifier) |foos|
//@[14:0015) |   | |   | ├─Token(LeftSquare) |[|
//@[15:0024) |   | |   | ├─VariableAccessSyntax
//@[15:0024) |   | |   | | └─IdentifierSyntax
//@[15:0024) |   | |   | |   └─Token(Identifier) |zeroIndex|
//@[24:0025) |   | |   | └─Token(RightSquare) |]|
//@[25:0026) |   | |   ├─Token(LeftSquare) |[|
//@[26:0039) |   | |   ├─StringSyntax
//@[26:0029) |   | |   | ├─Token(StringLeftPiece) |'${|
//@[29:0037) |   | |   | ├─VariableAccessSyntax
//@[29:0037) |   | |   | | └─IdentifierSyntax
//@[29:0037) |   | |   | |   └─Token(Identifier) |strParam|
//@[37:0039) |   | |   | └─Token(StringRightPiece) |}'|
//@[39:0040) |   | |   └─Token(RightSquare) |]|
//@[40:0041) |   | ├─Token(NewLine) |\n|
  case72: foos[otherIndex]['${strParam}']
//@[02:0041) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case72|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0041) |   | | └─ArrayAccessSyntax
//@[10:0026) |   | |   ├─ArrayAccessSyntax
//@[10:0014) |   | |   | ├─VariableAccessSyntax
//@[10:0014) |   | |   | | └─IdentifierSyntax
//@[10:0014) |   | |   | |   └─Token(Identifier) |foos|
//@[14:0015) |   | |   | ├─Token(LeftSquare) |[|
//@[15:0025) |   | |   | ├─VariableAccessSyntax
//@[15:0025) |   | |   | | └─IdentifierSyntax
//@[15:0025) |   | |   | |   └─Token(Identifier) |otherIndex|
//@[25:0026) |   | |   | └─Token(RightSquare) |]|
//@[26:0027) |   | |   ├─Token(LeftSquare) |[|
//@[27:0040) |   | |   ├─StringSyntax
//@[27:0030) |   | |   | ├─Token(StringLeftPiece) |'${|
//@[30:0038) |   | |   | ├─VariableAccessSyntax
//@[30:0038) |   | |   | | └─IdentifierSyntax
//@[30:0038) |   | |   | |   └─Token(Identifier) |strParam|
//@[38:0040) |   | |   | └─Token(StringRightPiece) |}'|
//@[40:0041) |   | |   └─Token(RightSquare) |]|
//@[41:0042) |   | ├─Token(NewLine) |\n|
  case73: foo['i${strParam2}']
//@[02:0030) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case73|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0030) |   | | └─ArrayAccessSyntax
//@[10:0013) |   | |   ├─VariableAccessSyntax
//@[10:0013) |   | |   | └─IdentifierSyntax
//@[10:0013) |   | |   |   └─Token(Identifier) |foo|
//@[13:0014) |   | |   ├─Token(LeftSquare) |[|
//@[14:0029) |   | |   ├─StringSyntax
//@[14:0018) |   | |   | ├─Token(StringLeftPiece) |'i${|
//@[18:0027) |   | |   | ├─VariableAccessSyntax
//@[18:0027) |   | |   | | └─IdentifierSyntax
//@[18:0027) |   | |   | |   └─Token(Identifier) |strParam2|
//@[27:0029) |   | |   | └─Token(StringRightPiece) |}'|
//@[29:0030) |   | |   └─Token(RightSquare) |]|
//@[30:0031) |   | ├─Token(NewLine) |\n|
  case74: existingFoo['i${strParam2}']
//@[02:0038) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case74|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0038) |   | | └─ArrayAccessSyntax
//@[10:0021) |   | |   ├─VariableAccessSyntax
//@[10:0021) |   | |   | └─IdentifierSyntax
//@[10:0021) |   | |   |   └─Token(Identifier) |existingFoo|
//@[21:0022) |   | |   ├─Token(LeftSquare) |[|
//@[22:0037) |   | |   ├─StringSyntax
//@[22:0026) |   | |   | ├─Token(StringLeftPiece) |'i${|
//@[26:0035) |   | |   | ├─VariableAccessSyntax
//@[26:0035) |   | |   | | └─IdentifierSyntax
//@[26:0035) |   | |   | |   └─Token(Identifier) |strParam2|
//@[35:0037) |   | |   | └─Token(StringRightPiece) |}'|
//@[37:0038) |   | |   └─Token(RightSquare) |]|
//@[38:0039) |   | ├─Token(NewLine) |\n|
  case75: foo::fooChild['i${strParam2}']
//@[02:0040) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case75|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0040) |   | | └─ArrayAccessSyntax
//@[10:0023) |   | |   ├─ResourceAccessSyntax
//@[10:0013) |   | |   | ├─VariableAccessSyntax
//@[10:0013) |   | |   | | └─IdentifierSyntax
//@[10:0013) |   | |   | |   └─Token(Identifier) |foo|
//@[13:0015) |   | |   | ├─Token(DoubleColon) |::|
//@[15:0023) |   | |   | └─IdentifierSyntax
//@[15:0023) |   | |   |   └─Token(Identifier) |fooChild|
//@[23:0024) |   | |   ├─Token(LeftSquare) |[|
//@[24:0039) |   | |   ├─StringSyntax
//@[24:0028) |   | |   | ├─Token(StringLeftPiece) |'i${|
//@[28:0037) |   | |   | ├─VariableAccessSyntax
//@[28:0037) |   | |   | | └─IdentifierSyntax
//@[28:0037) |   | |   | |   └─Token(Identifier) |strParam2|
//@[37:0039) |   | |   | └─Token(StringRightPiece) |}'|
//@[39:0040) |   | |   └─Token(RightSquare) |]|
//@[40:0041) |   | ├─Token(NewLine) |\n|
  case76: foos[0]['i${strParam2}']
//@[02:0034) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case76|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0034) |   | | └─ArrayAccessSyntax
//@[10:0017) |   | |   ├─ArrayAccessSyntax
//@[10:0014) |   | |   | ├─VariableAccessSyntax
//@[10:0014) |   | |   | | └─IdentifierSyntax
//@[10:0014) |   | |   | |   └─Token(Identifier) |foos|
//@[14:0015) |   | |   | ├─Token(LeftSquare) |[|
//@[15:0016) |   | |   | ├─IntegerLiteralSyntax
//@[15:0016) |   | |   | | └─Token(Integer) |0|
//@[16:0017) |   | |   | └─Token(RightSquare) |]|
//@[17:0018) |   | |   ├─Token(LeftSquare) |[|
//@[18:0033) |   | |   ├─StringSyntax
//@[18:0022) |   | |   | ├─Token(StringLeftPiece) |'i${|
//@[22:0031) |   | |   | ├─VariableAccessSyntax
//@[22:0031) |   | |   | | └─IdentifierSyntax
//@[22:0031) |   | |   | |   └─Token(Identifier) |strParam2|
//@[31:0033) |   | |   | └─Token(StringRightPiece) |}'|
//@[33:0034) |   | |   └─Token(RightSquare) |]|
//@[34:0035) |   | ├─Token(NewLine) |\n|
  case77: foos[i]['i${strParam2}']
//@[02:0034) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case77|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0034) |   | | └─ArrayAccessSyntax
//@[10:0017) |   | |   ├─ArrayAccessSyntax
//@[10:0014) |   | |   | ├─VariableAccessSyntax
//@[10:0014) |   | |   | | └─IdentifierSyntax
//@[10:0014) |   | |   | |   └─Token(Identifier) |foos|
//@[14:0015) |   | |   | ├─Token(LeftSquare) |[|
//@[15:0016) |   | |   | ├─VariableAccessSyntax
//@[15:0016) |   | |   | | └─IdentifierSyntax
//@[15:0016) |   | |   | |   └─Token(Identifier) |i|
//@[16:0017) |   | |   | └─Token(RightSquare) |]|
//@[17:0018) |   | |   ├─Token(LeftSquare) |[|
//@[18:0033) |   | |   ├─StringSyntax
//@[18:0022) |   | |   | ├─Token(StringLeftPiece) |'i${|
//@[22:0031) |   | |   | ├─VariableAccessSyntax
//@[22:0031) |   | |   | | └─IdentifierSyntax
//@[22:0031) |   | |   | |   └─Token(Identifier) |strParam2|
//@[31:0033) |   | |   | └─Token(StringRightPiece) |}'|
//@[33:0034) |   | |   └─Token(RightSquare) |]|
//@[34:0035) |   | ├─Token(NewLine) |\n|
  case78: foos[i + 2]['i${strParam2}']
//@[02:0038) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case78|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0038) |   | | └─ArrayAccessSyntax
//@[10:0021) |   | |   ├─ArrayAccessSyntax
//@[10:0014) |   | |   | ├─VariableAccessSyntax
//@[10:0014) |   | |   | | └─IdentifierSyntax
//@[10:0014) |   | |   | |   └─Token(Identifier) |foos|
//@[14:0015) |   | |   | ├─Token(LeftSquare) |[|
//@[15:0020) |   | |   | ├─BinaryOperationSyntax
//@[15:0016) |   | |   | | ├─VariableAccessSyntax
//@[15:0016) |   | |   | | | └─IdentifierSyntax
//@[15:0016) |   | |   | | |   └─Token(Identifier) |i|
//@[17:0018) |   | |   | | ├─Token(Plus) |+|
//@[19:0020) |   | |   | | └─IntegerLiteralSyntax
//@[19:0020) |   | |   | |   └─Token(Integer) |2|
//@[20:0021) |   | |   | └─Token(RightSquare) |]|
//@[21:0022) |   | |   ├─Token(LeftSquare) |[|
//@[22:0037) |   | |   ├─StringSyntax
//@[22:0026) |   | |   | ├─Token(StringLeftPiece) |'i${|
//@[26:0035) |   | |   | ├─VariableAccessSyntax
//@[26:0035) |   | |   | | └─IdentifierSyntax
//@[26:0035) |   | |   | |   └─Token(Identifier) |strParam2|
//@[35:0037) |   | |   | └─Token(StringRightPiece) |}'|
//@[37:0038) |   | |   └─Token(RightSquare) |]|
//@[38:0039) |   | ├─Token(NewLine) |\n|
  case79: foos[zeroIndex]['i${strParam2}']
//@[02:0042) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case79|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0042) |   | | └─ArrayAccessSyntax
//@[10:0025) |   | |   ├─ArrayAccessSyntax
//@[10:0014) |   | |   | ├─VariableAccessSyntax
//@[10:0014) |   | |   | | └─IdentifierSyntax
//@[10:0014) |   | |   | |   └─Token(Identifier) |foos|
//@[14:0015) |   | |   | ├─Token(LeftSquare) |[|
//@[15:0024) |   | |   | ├─VariableAccessSyntax
//@[15:0024) |   | |   | | └─IdentifierSyntax
//@[15:0024) |   | |   | |   └─Token(Identifier) |zeroIndex|
//@[24:0025) |   | |   | └─Token(RightSquare) |]|
//@[25:0026) |   | |   ├─Token(LeftSquare) |[|
//@[26:0041) |   | |   ├─StringSyntax
//@[26:0030) |   | |   | ├─Token(StringLeftPiece) |'i${|
//@[30:0039) |   | |   | ├─VariableAccessSyntax
//@[30:0039) |   | |   | | └─IdentifierSyntax
//@[30:0039) |   | |   | |   └─Token(Identifier) |strParam2|
//@[39:0041) |   | |   | └─Token(StringRightPiece) |}'|
//@[41:0042) |   | |   └─Token(RightSquare) |]|
//@[42:0043) |   | ├─Token(NewLine) |\n|
  case80: foos[otherIndex]['i${strParam2}']
//@[02:0043) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case80|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0043) |   | | └─ArrayAccessSyntax
//@[10:0026) |   | |   ├─ArrayAccessSyntax
//@[10:0014) |   | |   | ├─VariableAccessSyntax
//@[10:0014) |   | |   | | └─IdentifierSyntax
//@[10:0014) |   | |   | |   └─Token(Identifier) |foos|
//@[14:0015) |   | |   | ├─Token(LeftSquare) |[|
//@[15:0025) |   | |   | ├─VariableAccessSyntax
//@[15:0025) |   | |   | | └─IdentifierSyntax
//@[15:0025) |   | |   | |   └─Token(Identifier) |otherIndex|
//@[25:0026) |   | |   | └─Token(RightSquare) |]|
//@[26:0027) |   | |   ├─Token(LeftSquare) |[|
//@[27:0042) |   | |   ├─StringSyntax
//@[27:0031) |   | |   | ├─Token(StringLeftPiece) |'i${|
//@[31:0040) |   | |   | ├─VariableAccessSyntax
//@[31:0040) |   | |   | | └─IdentifierSyntax
//@[31:0040) |   | |   | |   └─Token(Identifier) |strParam2|
//@[40:0042) |   | |   | └─Token(StringRightPiece) |}'|
//@[42:0043) |   | |   └─Token(RightSquare) |]|
//@[43:0044) |   | ├─Token(NewLine) |\n|
  case81: foo[strArray[1]]
//@[02:0026) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case81|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0026) |   | | └─ArrayAccessSyntax
//@[10:0013) |   | |   ├─VariableAccessSyntax
//@[10:0013) |   | |   | └─IdentifierSyntax
//@[10:0013) |   | |   |   └─Token(Identifier) |foo|
//@[13:0014) |   | |   ├─Token(LeftSquare) |[|
//@[14:0025) |   | |   ├─ArrayAccessSyntax
//@[14:0022) |   | |   | ├─VariableAccessSyntax
//@[14:0022) |   | |   | | └─IdentifierSyntax
//@[14:0022) |   | |   | |   └─Token(Identifier) |strArray|
//@[22:0023) |   | |   | ├─Token(LeftSquare) |[|
//@[23:0024) |   | |   | ├─IntegerLiteralSyntax
//@[23:0024) |   | |   | | └─Token(Integer) |1|
//@[24:0025) |   | |   | └─Token(RightSquare) |]|
//@[25:0026) |   | |   └─Token(RightSquare) |]|
//@[26:0027) |   | ├─Token(NewLine) |\n|
  case82: existingFoo[strArray[1]]
//@[02:0034) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case82|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0034) |   | | └─ArrayAccessSyntax
//@[10:0021) |   | |   ├─VariableAccessSyntax
//@[10:0021) |   | |   | └─IdentifierSyntax
//@[10:0021) |   | |   |   └─Token(Identifier) |existingFoo|
//@[21:0022) |   | |   ├─Token(LeftSquare) |[|
//@[22:0033) |   | |   ├─ArrayAccessSyntax
//@[22:0030) |   | |   | ├─VariableAccessSyntax
//@[22:0030) |   | |   | | └─IdentifierSyntax
//@[22:0030) |   | |   | |   └─Token(Identifier) |strArray|
//@[30:0031) |   | |   | ├─Token(LeftSquare) |[|
//@[31:0032) |   | |   | ├─IntegerLiteralSyntax
//@[31:0032) |   | |   | | └─Token(Integer) |1|
//@[32:0033) |   | |   | └─Token(RightSquare) |]|
//@[33:0034) |   | |   └─Token(RightSquare) |]|
//@[34:0035) |   | ├─Token(NewLine) |\n|
  case83: foo::fooChild[strArray[1]]
//@[02:0036) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case83|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0036) |   | | └─ArrayAccessSyntax
//@[10:0023) |   | |   ├─ResourceAccessSyntax
//@[10:0013) |   | |   | ├─VariableAccessSyntax
//@[10:0013) |   | |   | | └─IdentifierSyntax
//@[10:0013) |   | |   | |   └─Token(Identifier) |foo|
//@[13:0015) |   | |   | ├─Token(DoubleColon) |::|
//@[15:0023) |   | |   | └─IdentifierSyntax
//@[15:0023) |   | |   |   └─Token(Identifier) |fooChild|
//@[23:0024) |   | |   ├─Token(LeftSquare) |[|
//@[24:0035) |   | |   ├─ArrayAccessSyntax
//@[24:0032) |   | |   | ├─VariableAccessSyntax
//@[24:0032) |   | |   | | └─IdentifierSyntax
//@[24:0032) |   | |   | |   └─Token(Identifier) |strArray|
//@[32:0033) |   | |   | ├─Token(LeftSquare) |[|
//@[33:0034) |   | |   | ├─IntegerLiteralSyntax
//@[33:0034) |   | |   | | └─Token(Integer) |1|
//@[34:0035) |   | |   | └─Token(RightSquare) |]|
//@[35:0036) |   | |   └─Token(RightSquare) |]|
//@[36:0037) |   | ├─Token(NewLine) |\n|
  case84: foos[0][strArray[1]]
//@[02:0030) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case84|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0030) |   | | └─ArrayAccessSyntax
//@[10:0017) |   | |   ├─ArrayAccessSyntax
//@[10:0014) |   | |   | ├─VariableAccessSyntax
//@[10:0014) |   | |   | | └─IdentifierSyntax
//@[10:0014) |   | |   | |   └─Token(Identifier) |foos|
//@[14:0015) |   | |   | ├─Token(LeftSquare) |[|
//@[15:0016) |   | |   | ├─IntegerLiteralSyntax
//@[15:0016) |   | |   | | └─Token(Integer) |0|
//@[16:0017) |   | |   | └─Token(RightSquare) |]|
//@[17:0018) |   | |   ├─Token(LeftSquare) |[|
//@[18:0029) |   | |   ├─ArrayAccessSyntax
//@[18:0026) |   | |   | ├─VariableAccessSyntax
//@[18:0026) |   | |   | | └─IdentifierSyntax
//@[18:0026) |   | |   | |   └─Token(Identifier) |strArray|
//@[26:0027) |   | |   | ├─Token(LeftSquare) |[|
//@[27:0028) |   | |   | ├─IntegerLiteralSyntax
//@[27:0028) |   | |   | | └─Token(Integer) |1|
//@[28:0029) |   | |   | └─Token(RightSquare) |]|
//@[29:0030) |   | |   └─Token(RightSquare) |]|
//@[30:0031) |   | ├─Token(NewLine) |\n|
  case85: foos[i][strArray[1]]
//@[02:0030) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case85|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0030) |   | | └─ArrayAccessSyntax
//@[10:0017) |   | |   ├─ArrayAccessSyntax
//@[10:0014) |   | |   | ├─VariableAccessSyntax
//@[10:0014) |   | |   | | └─IdentifierSyntax
//@[10:0014) |   | |   | |   └─Token(Identifier) |foos|
//@[14:0015) |   | |   | ├─Token(LeftSquare) |[|
//@[15:0016) |   | |   | ├─VariableAccessSyntax
//@[15:0016) |   | |   | | └─IdentifierSyntax
//@[15:0016) |   | |   | |   └─Token(Identifier) |i|
//@[16:0017) |   | |   | └─Token(RightSquare) |]|
//@[17:0018) |   | |   ├─Token(LeftSquare) |[|
//@[18:0029) |   | |   ├─ArrayAccessSyntax
//@[18:0026) |   | |   | ├─VariableAccessSyntax
//@[18:0026) |   | |   | | └─IdentifierSyntax
//@[18:0026) |   | |   | |   └─Token(Identifier) |strArray|
//@[26:0027) |   | |   | ├─Token(LeftSquare) |[|
//@[27:0028) |   | |   | ├─IntegerLiteralSyntax
//@[27:0028) |   | |   | | └─Token(Integer) |1|
//@[28:0029) |   | |   | └─Token(RightSquare) |]|
//@[29:0030) |   | |   └─Token(RightSquare) |]|
//@[30:0031) |   | ├─Token(NewLine) |\n|
  case86: foos[i + 2][strArray[1]]
//@[02:0034) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case86|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0034) |   | | └─ArrayAccessSyntax
//@[10:0021) |   | |   ├─ArrayAccessSyntax
//@[10:0014) |   | |   | ├─VariableAccessSyntax
//@[10:0014) |   | |   | | └─IdentifierSyntax
//@[10:0014) |   | |   | |   └─Token(Identifier) |foos|
//@[14:0015) |   | |   | ├─Token(LeftSquare) |[|
//@[15:0020) |   | |   | ├─BinaryOperationSyntax
//@[15:0016) |   | |   | | ├─VariableAccessSyntax
//@[15:0016) |   | |   | | | └─IdentifierSyntax
//@[15:0016) |   | |   | | |   └─Token(Identifier) |i|
//@[17:0018) |   | |   | | ├─Token(Plus) |+|
//@[19:0020) |   | |   | | └─IntegerLiteralSyntax
//@[19:0020) |   | |   | |   └─Token(Integer) |2|
//@[20:0021) |   | |   | └─Token(RightSquare) |]|
//@[21:0022) |   | |   ├─Token(LeftSquare) |[|
//@[22:0033) |   | |   ├─ArrayAccessSyntax
//@[22:0030) |   | |   | ├─VariableAccessSyntax
//@[22:0030) |   | |   | | └─IdentifierSyntax
//@[22:0030) |   | |   | |   └─Token(Identifier) |strArray|
//@[30:0031) |   | |   | ├─Token(LeftSquare) |[|
//@[31:0032) |   | |   | ├─IntegerLiteralSyntax
//@[31:0032) |   | |   | | └─Token(Integer) |1|
//@[32:0033) |   | |   | └─Token(RightSquare) |]|
//@[33:0034) |   | |   └─Token(RightSquare) |]|
//@[34:0035) |   | ├─Token(NewLine) |\n|
  case87: foos[zeroIndex][strArray[1]]
//@[02:0038) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case87|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0038) |   | | └─ArrayAccessSyntax
//@[10:0025) |   | |   ├─ArrayAccessSyntax
//@[10:0014) |   | |   | ├─VariableAccessSyntax
//@[10:0014) |   | |   | | └─IdentifierSyntax
//@[10:0014) |   | |   | |   └─Token(Identifier) |foos|
//@[14:0015) |   | |   | ├─Token(LeftSquare) |[|
//@[15:0024) |   | |   | ├─VariableAccessSyntax
//@[15:0024) |   | |   | | └─IdentifierSyntax
//@[15:0024) |   | |   | |   └─Token(Identifier) |zeroIndex|
//@[24:0025) |   | |   | └─Token(RightSquare) |]|
//@[25:0026) |   | |   ├─Token(LeftSquare) |[|
//@[26:0037) |   | |   ├─ArrayAccessSyntax
//@[26:0034) |   | |   | ├─VariableAccessSyntax
//@[26:0034) |   | |   | | └─IdentifierSyntax
//@[26:0034) |   | |   | |   └─Token(Identifier) |strArray|
//@[34:0035) |   | |   | ├─Token(LeftSquare) |[|
//@[35:0036) |   | |   | ├─IntegerLiteralSyntax
//@[35:0036) |   | |   | | └─Token(Integer) |1|
//@[36:0037) |   | |   | └─Token(RightSquare) |]|
//@[37:0038) |   | |   └─Token(RightSquare) |]|
//@[38:0039) |   | ├─Token(NewLine) |\n|
  case88: foos[otherIndex][strArray[1]]
//@[02:0039) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case88|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0039) |   | | └─ArrayAccessSyntax
//@[10:0026) |   | |   ├─ArrayAccessSyntax
//@[10:0014) |   | |   | ├─VariableAccessSyntax
//@[10:0014) |   | |   | | └─IdentifierSyntax
//@[10:0014) |   | |   | |   └─Token(Identifier) |foos|
//@[14:0015) |   | |   | ├─Token(LeftSquare) |[|
//@[15:0025) |   | |   | ├─VariableAccessSyntax
//@[15:0025) |   | |   | | └─IdentifierSyntax
//@[15:0025) |   | |   | |   └─Token(Identifier) |otherIndex|
//@[25:0026) |   | |   | └─Token(RightSquare) |]|
//@[26:0027) |   | |   ├─Token(LeftSquare) |[|
//@[27:0038) |   | |   ├─ArrayAccessSyntax
//@[27:0035) |   | |   | ├─VariableAccessSyntax
//@[27:0035) |   | |   | | └─IdentifierSyntax
//@[27:0035) |   | |   | |   └─Token(Identifier) |strArray|
//@[35:0036) |   | |   | ├─Token(LeftSquare) |[|
//@[36:0037) |   | |   | ├─IntegerLiteralSyntax
//@[36:0037) |   | |   | | └─Token(Integer) |1|
//@[37:0038) |   | |   | └─Token(RightSquare) |]|
//@[38:0039) |   | |   └─Token(RightSquare) |]|
//@[39:0040) |   | ├─Token(NewLine) |\n|
  case89: foo[last(strArray)]
//@[02:0029) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case89|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0029) |   | | └─ArrayAccessSyntax
//@[10:0013) |   | |   ├─VariableAccessSyntax
//@[10:0013) |   | |   | └─IdentifierSyntax
//@[10:0013) |   | |   |   └─Token(Identifier) |foo|
//@[13:0014) |   | |   ├─Token(LeftSquare) |[|
//@[14:0028) |   | |   ├─FunctionCallSyntax
//@[14:0018) |   | |   | ├─IdentifierSyntax
//@[14:0018) |   | |   | | └─Token(Identifier) |last|
//@[18:0019) |   | |   | ├─Token(LeftParen) |(|
//@[19:0027) |   | |   | ├─FunctionArgumentSyntax
//@[19:0027) |   | |   | | └─VariableAccessSyntax
//@[19:0027) |   | |   | |   └─IdentifierSyntax
//@[19:0027) |   | |   | |     └─Token(Identifier) |strArray|
//@[27:0028) |   | |   | └─Token(RightParen) |)|
//@[28:0029) |   | |   └─Token(RightSquare) |]|
//@[29:0030) |   | ├─Token(NewLine) |\n|
  case90: existingFoo[last(strArray)]
//@[02:0037) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case90|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0037) |   | | └─ArrayAccessSyntax
//@[10:0021) |   | |   ├─VariableAccessSyntax
//@[10:0021) |   | |   | └─IdentifierSyntax
//@[10:0021) |   | |   |   └─Token(Identifier) |existingFoo|
//@[21:0022) |   | |   ├─Token(LeftSquare) |[|
//@[22:0036) |   | |   ├─FunctionCallSyntax
//@[22:0026) |   | |   | ├─IdentifierSyntax
//@[22:0026) |   | |   | | └─Token(Identifier) |last|
//@[26:0027) |   | |   | ├─Token(LeftParen) |(|
//@[27:0035) |   | |   | ├─FunctionArgumentSyntax
//@[27:0035) |   | |   | | └─VariableAccessSyntax
//@[27:0035) |   | |   | |   └─IdentifierSyntax
//@[27:0035) |   | |   | |     └─Token(Identifier) |strArray|
//@[35:0036) |   | |   | └─Token(RightParen) |)|
//@[36:0037) |   | |   └─Token(RightSquare) |]|
//@[37:0038) |   | ├─Token(NewLine) |\n|
  case91: foo::fooChild[last(strArray)]
//@[02:0039) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case91|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0039) |   | | └─ArrayAccessSyntax
//@[10:0023) |   | |   ├─ResourceAccessSyntax
//@[10:0013) |   | |   | ├─VariableAccessSyntax
//@[10:0013) |   | |   | | └─IdentifierSyntax
//@[10:0013) |   | |   | |   └─Token(Identifier) |foo|
//@[13:0015) |   | |   | ├─Token(DoubleColon) |::|
//@[15:0023) |   | |   | └─IdentifierSyntax
//@[15:0023) |   | |   |   └─Token(Identifier) |fooChild|
//@[23:0024) |   | |   ├─Token(LeftSquare) |[|
//@[24:0038) |   | |   ├─FunctionCallSyntax
//@[24:0028) |   | |   | ├─IdentifierSyntax
//@[24:0028) |   | |   | | └─Token(Identifier) |last|
//@[28:0029) |   | |   | ├─Token(LeftParen) |(|
//@[29:0037) |   | |   | ├─FunctionArgumentSyntax
//@[29:0037) |   | |   | | └─VariableAccessSyntax
//@[29:0037) |   | |   | |   └─IdentifierSyntax
//@[29:0037) |   | |   | |     └─Token(Identifier) |strArray|
//@[37:0038) |   | |   | └─Token(RightParen) |)|
//@[38:0039) |   | |   └─Token(RightSquare) |]|
//@[39:0040) |   | ├─Token(NewLine) |\n|
  case92: foos[0][last(strArray)]
//@[02:0033) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case92|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0033) |   | | └─ArrayAccessSyntax
//@[10:0017) |   | |   ├─ArrayAccessSyntax
//@[10:0014) |   | |   | ├─VariableAccessSyntax
//@[10:0014) |   | |   | | └─IdentifierSyntax
//@[10:0014) |   | |   | |   └─Token(Identifier) |foos|
//@[14:0015) |   | |   | ├─Token(LeftSquare) |[|
//@[15:0016) |   | |   | ├─IntegerLiteralSyntax
//@[15:0016) |   | |   | | └─Token(Integer) |0|
//@[16:0017) |   | |   | └─Token(RightSquare) |]|
//@[17:0018) |   | |   ├─Token(LeftSquare) |[|
//@[18:0032) |   | |   ├─FunctionCallSyntax
//@[18:0022) |   | |   | ├─IdentifierSyntax
//@[18:0022) |   | |   | | └─Token(Identifier) |last|
//@[22:0023) |   | |   | ├─Token(LeftParen) |(|
//@[23:0031) |   | |   | ├─FunctionArgumentSyntax
//@[23:0031) |   | |   | | └─VariableAccessSyntax
//@[23:0031) |   | |   | |   └─IdentifierSyntax
//@[23:0031) |   | |   | |     └─Token(Identifier) |strArray|
//@[31:0032) |   | |   | └─Token(RightParen) |)|
//@[32:0033) |   | |   └─Token(RightSquare) |]|
//@[33:0034) |   | ├─Token(NewLine) |\n|
  case93: foos[i][last(strArray)]
//@[02:0033) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case93|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0033) |   | | └─ArrayAccessSyntax
//@[10:0017) |   | |   ├─ArrayAccessSyntax
//@[10:0014) |   | |   | ├─VariableAccessSyntax
//@[10:0014) |   | |   | | └─IdentifierSyntax
//@[10:0014) |   | |   | |   └─Token(Identifier) |foos|
//@[14:0015) |   | |   | ├─Token(LeftSquare) |[|
//@[15:0016) |   | |   | ├─VariableAccessSyntax
//@[15:0016) |   | |   | | └─IdentifierSyntax
//@[15:0016) |   | |   | |   └─Token(Identifier) |i|
//@[16:0017) |   | |   | └─Token(RightSquare) |]|
//@[17:0018) |   | |   ├─Token(LeftSquare) |[|
//@[18:0032) |   | |   ├─FunctionCallSyntax
//@[18:0022) |   | |   | ├─IdentifierSyntax
//@[18:0022) |   | |   | | └─Token(Identifier) |last|
//@[22:0023) |   | |   | ├─Token(LeftParen) |(|
//@[23:0031) |   | |   | ├─FunctionArgumentSyntax
//@[23:0031) |   | |   | | └─VariableAccessSyntax
//@[23:0031) |   | |   | |   └─IdentifierSyntax
//@[23:0031) |   | |   | |     └─Token(Identifier) |strArray|
//@[31:0032) |   | |   | └─Token(RightParen) |)|
//@[32:0033) |   | |   └─Token(RightSquare) |]|
//@[33:0034) |   | ├─Token(NewLine) |\n|
  case94: foos[i + 2][last(strArray)]
//@[02:0037) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case94|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0037) |   | | └─ArrayAccessSyntax
//@[10:0021) |   | |   ├─ArrayAccessSyntax
//@[10:0014) |   | |   | ├─VariableAccessSyntax
//@[10:0014) |   | |   | | └─IdentifierSyntax
//@[10:0014) |   | |   | |   └─Token(Identifier) |foos|
//@[14:0015) |   | |   | ├─Token(LeftSquare) |[|
//@[15:0020) |   | |   | ├─BinaryOperationSyntax
//@[15:0016) |   | |   | | ├─VariableAccessSyntax
//@[15:0016) |   | |   | | | └─IdentifierSyntax
//@[15:0016) |   | |   | | |   └─Token(Identifier) |i|
//@[17:0018) |   | |   | | ├─Token(Plus) |+|
//@[19:0020) |   | |   | | └─IntegerLiteralSyntax
//@[19:0020) |   | |   | |   └─Token(Integer) |2|
//@[20:0021) |   | |   | └─Token(RightSquare) |]|
//@[21:0022) |   | |   ├─Token(LeftSquare) |[|
//@[22:0036) |   | |   ├─FunctionCallSyntax
//@[22:0026) |   | |   | ├─IdentifierSyntax
//@[22:0026) |   | |   | | └─Token(Identifier) |last|
//@[26:0027) |   | |   | ├─Token(LeftParen) |(|
//@[27:0035) |   | |   | ├─FunctionArgumentSyntax
//@[27:0035) |   | |   | | └─VariableAccessSyntax
//@[27:0035) |   | |   | |   └─IdentifierSyntax
//@[27:0035) |   | |   | |     └─Token(Identifier) |strArray|
//@[35:0036) |   | |   | └─Token(RightParen) |)|
//@[36:0037) |   | |   └─Token(RightSquare) |]|
//@[37:0038) |   | ├─Token(NewLine) |\n|
  case95: foos[zeroIndex][last(strArray)]
//@[02:0041) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case95|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0041) |   | | └─ArrayAccessSyntax
//@[10:0025) |   | |   ├─ArrayAccessSyntax
//@[10:0014) |   | |   | ├─VariableAccessSyntax
//@[10:0014) |   | |   | | └─IdentifierSyntax
//@[10:0014) |   | |   | |   └─Token(Identifier) |foos|
//@[14:0015) |   | |   | ├─Token(LeftSquare) |[|
//@[15:0024) |   | |   | ├─VariableAccessSyntax
//@[15:0024) |   | |   | | └─IdentifierSyntax
//@[15:0024) |   | |   | |   └─Token(Identifier) |zeroIndex|
//@[24:0025) |   | |   | └─Token(RightSquare) |]|
//@[25:0026) |   | |   ├─Token(LeftSquare) |[|
//@[26:0040) |   | |   ├─FunctionCallSyntax
//@[26:0030) |   | |   | ├─IdentifierSyntax
//@[26:0030) |   | |   | | └─Token(Identifier) |last|
//@[30:0031) |   | |   | ├─Token(LeftParen) |(|
//@[31:0039) |   | |   | ├─FunctionArgumentSyntax
//@[31:0039) |   | |   | | └─VariableAccessSyntax
//@[31:0039) |   | |   | |   └─IdentifierSyntax
//@[31:0039) |   | |   | |     └─Token(Identifier) |strArray|
//@[39:0040) |   | |   | └─Token(RightParen) |)|
//@[40:0041) |   | |   └─Token(RightSquare) |]|
//@[41:0042) |   | ├─Token(NewLine) |\n|
  case96: foos[otherIndex][last(strArray)]
//@[02:0042) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case96|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0042) |   | | └─ArrayAccessSyntax
//@[10:0026) |   | |   ├─ArrayAccessSyntax
//@[10:0014) |   | |   | ├─VariableAccessSyntax
//@[10:0014) |   | |   | | └─IdentifierSyntax
//@[10:0014) |   | |   | |   └─Token(Identifier) |foos|
//@[14:0015) |   | |   | ├─Token(LeftSquare) |[|
//@[15:0025) |   | |   | ├─VariableAccessSyntax
//@[15:0025) |   | |   | | └─IdentifierSyntax
//@[15:0025) |   | |   | |   └─Token(Identifier) |otherIndex|
//@[25:0026) |   | |   | └─Token(RightSquare) |]|
//@[26:0027) |   | |   ├─Token(LeftSquare) |[|
//@[27:0041) |   | |   ├─FunctionCallSyntax
//@[27:0031) |   | |   | ├─IdentifierSyntax
//@[27:0031) |   | |   | | └─Token(Identifier) |last|
//@[31:0032) |   | |   | ├─Token(LeftParen) |(|
//@[32:0040) |   | |   | ├─FunctionArgumentSyntax
//@[32:0040) |   | |   | | └─VariableAccessSyntax
//@[32:0040) |   | |   | |   └─IdentifierSyntax
//@[32:0040) |   | |   | |     └─Token(Identifier) |strArray|
//@[40:0041) |   | |   | └─Token(RightParen) |)|
//@[41:0042) |   | |   └─Token(RightSquare) |]|
//@[42:0043) |   | ├─Token(NewLine) |\n|
  case97: foo[cond ? 'id' : 'properties']
//@[02:0041) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case97|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0041) |   | | └─ArrayAccessSyntax
//@[10:0013) |   | |   ├─VariableAccessSyntax
//@[10:0013) |   | |   | └─IdentifierSyntax
//@[10:0013) |   | |   |   └─Token(Identifier) |foo|
//@[13:0014) |   | |   ├─Token(LeftSquare) |[|
//@[14:0040) |   | |   ├─TernaryOperationSyntax
//@[14:0018) |   | |   | ├─VariableAccessSyntax
//@[14:0018) |   | |   | | └─IdentifierSyntax
//@[14:0018) |   | |   | |   └─Token(Identifier) |cond|
//@[19:0020) |   | |   | ├─Token(Question) |?|
//@[21:0025) |   | |   | ├─StringSyntax
//@[21:0025) |   | |   | | └─Token(StringComplete) |'id'|
//@[26:0027) |   | |   | ├─Token(Colon) |:|
//@[28:0040) |   | |   | └─StringSyntax
//@[28:0040) |   | |   |   └─Token(StringComplete) |'properties'|
//@[40:0041) |   | |   └─Token(RightSquare) |]|
//@[41:0042) |   | ├─Token(NewLine) |\n|
  case98: existingFoo[cond ? 'id' : 'properties']
//@[02:0049) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case98|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0049) |   | | └─ArrayAccessSyntax
//@[10:0021) |   | |   ├─VariableAccessSyntax
//@[10:0021) |   | |   | └─IdentifierSyntax
//@[10:0021) |   | |   |   └─Token(Identifier) |existingFoo|
//@[21:0022) |   | |   ├─Token(LeftSquare) |[|
//@[22:0048) |   | |   ├─TernaryOperationSyntax
//@[22:0026) |   | |   | ├─VariableAccessSyntax
//@[22:0026) |   | |   | | └─IdentifierSyntax
//@[22:0026) |   | |   | |   └─Token(Identifier) |cond|
//@[27:0028) |   | |   | ├─Token(Question) |?|
//@[29:0033) |   | |   | ├─StringSyntax
//@[29:0033) |   | |   | | └─Token(StringComplete) |'id'|
//@[34:0035) |   | |   | ├─Token(Colon) |:|
//@[36:0048) |   | |   | └─StringSyntax
//@[36:0048) |   | |   |   └─Token(StringComplete) |'properties'|
//@[48:0049) |   | |   └─Token(RightSquare) |]|
//@[49:0050) |   | ├─Token(NewLine) |\n|
  case99: foo::fooChild[cond ? 'id' : 'properties']
//@[02:0051) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case99|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0051) |   | | └─ArrayAccessSyntax
//@[10:0023) |   | |   ├─ResourceAccessSyntax
//@[10:0013) |   | |   | ├─VariableAccessSyntax
//@[10:0013) |   | |   | | └─IdentifierSyntax
//@[10:0013) |   | |   | |   └─Token(Identifier) |foo|
//@[13:0015) |   | |   | ├─Token(DoubleColon) |::|
//@[15:0023) |   | |   | └─IdentifierSyntax
//@[15:0023) |   | |   |   └─Token(Identifier) |fooChild|
//@[23:0024) |   | |   ├─Token(LeftSquare) |[|
//@[24:0050) |   | |   ├─TernaryOperationSyntax
//@[24:0028) |   | |   | ├─VariableAccessSyntax
//@[24:0028) |   | |   | | └─IdentifierSyntax
//@[24:0028) |   | |   | |   └─Token(Identifier) |cond|
//@[29:0030) |   | |   | ├─Token(Question) |?|
//@[31:0035) |   | |   | ├─StringSyntax
//@[31:0035) |   | |   | | └─Token(StringComplete) |'id'|
//@[36:0037) |   | |   | ├─Token(Colon) |:|
//@[38:0050) |   | |   | └─StringSyntax
//@[38:0050) |   | |   |   └─Token(StringComplete) |'properties'|
//@[50:0051) |   | |   └─Token(RightSquare) |]|
//@[51:0052) |   | ├─Token(NewLine) |\n|
  case100: foos[0][cond ? 'id' : 'properties']
//@[02:0046) |   | ├─ObjectPropertySyntax
//@[02:0009) |   | | ├─IdentifierSyntax
//@[02:0009) |   | | | └─Token(Identifier) |case100|
//@[09:0010) |   | | ├─Token(Colon) |:|
//@[11:0046) |   | | └─ArrayAccessSyntax
//@[11:0018) |   | |   ├─ArrayAccessSyntax
//@[11:0015) |   | |   | ├─VariableAccessSyntax
//@[11:0015) |   | |   | | └─IdentifierSyntax
//@[11:0015) |   | |   | |   └─Token(Identifier) |foos|
//@[15:0016) |   | |   | ├─Token(LeftSquare) |[|
//@[16:0017) |   | |   | ├─IntegerLiteralSyntax
//@[16:0017) |   | |   | | └─Token(Integer) |0|
//@[17:0018) |   | |   | └─Token(RightSquare) |]|
//@[18:0019) |   | |   ├─Token(LeftSquare) |[|
//@[19:0045) |   | |   ├─TernaryOperationSyntax
//@[19:0023) |   | |   | ├─VariableAccessSyntax
//@[19:0023) |   | |   | | └─IdentifierSyntax
//@[19:0023) |   | |   | |   └─Token(Identifier) |cond|
//@[24:0025) |   | |   | ├─Token(Question) |?|
//@[26:0030) |   | |   | ├─StringSyntax
//@[26:0030) |   | |   | | └─Token(StringComplete) |'id'|
//@[31:0032) |   | |   | ├─Token(Colon) |:|
//@[33:0045) |   | |   | └─StringSyntax
//@[33:0045) |   | |   |   └─Token(StringComplete) |'properties'|
//@[45:0046) |   | |   └─Token(RightSquare) |]|
//@[46:0047) |   | ├─Token(NewLine) |\n|
  case101: foos[i][cond ? 'id' : 'properties']
//@[02:0046) |   | ├─ObjectPropertySyntax
//@[02:0009) |   | | ├─IdentifierSyntax
//@[02:0009) |   | | | └─Token(Identifier) |case101|
//@[09:0010) |   | | ├─Token(Colon) |:|
//@[11:0046) |   | | └─ArrayAccessSyntax
//@[11:0018) |   | |   ├─ArrayAccessSyntax
//@[11:0015) |   | |   | ├─VariableAccessSyntax
//@[11:0015) |   | |   | | └─IdentifierSyntax
//@[11:0015) |   | |   | |   └─Token(Identifier) |foos|
//@[15:0016) |   | |   | ├─Token(LeftSquare) |[|
//@[16:0017) |   | |   | ├─VariableAccessSyntax
//@[16:0017) |   | |   | | └─IdentifierSyntax
//@[16:0017) |   | |   | |   └─Token(Identifier) |i|
//@[17:0018) |   | |   | └─Token(RightSquare) |]|
//@[18:0019) |   | |   ├─Token(LeftSquare) |[|
//@[19:0045) |   | |   ├─TernaryOperationSyntax
//@[19:0023) |   | |   | ├─VariableAccessSyntax
//@[19:0023) |   | |   | | └─IdentifierSyntax
//@[19:0023) |   | |   | |   └─Token(Identifier) |cond|
//@[24:0025) |   | |   | ├─Token(Question) |?|
//@[26:0030) |   | |   | ├─StringSyntax
//@[26:0030) |   | |   | | └─Token(StringComplete) |'id'|
//@[31:0032) |   | |   | ├─Token(Colon) |:|
//@[33:0045) |   | |   | └─StringSyntax
//@[33:0045) |   | |   |   └─Token(StringComplete) |'properties'|
//@[45:0046) |   | |   └─Token(RightSquare) |]|
//@[46:0047) |   | ├─Token(NewLine) |\n|
  case102: foos[i + 2][cond ? 'id' : 'properties']
//@[02:0050) |   | ├─ObjectPropertySyntax
//@[02:0009) |   | | ├─IdentifierSyntax
//@[02:0009) |   | | | └─Token(Identifier) |case102|
//@[09:0010) |   | | ├─Token(Colon) |:|
//@[11:0050) |   | | └─ArrayAccessSyntax
//@[11:0022) |   | |   ├─ArrayAccessSyntax
//@[11:0015) |   | |   | ├─VariableAccessSyntax
//@[11:0015) |   | |   | | └─IdentifierSyntax
//@[11:0015) |   | |   | |   └─Token(Identifier) |foos|
//@[15:0016) |   | |   | ├─Token(LeftSquare) |[|
//@[16:0021) |   | |   | ├─BinaryOperationSyntax
//@[16:0017) |   | |   | | ├─VariableAccessSyntax
//@[16:0017) |   | |   | | | └─IdentifierSyntax
//@[16:0017) |   | |   | | |   └─Token(Identifier) |i|
//@[18:0019) |   | |   | | ├─Token(Plus) |+|
//@[20:0021) |   | |   | | └─IntegerLiteralSyntax
//@[20:0021) |   | |   | |   └─Token(Integer) |2|
//@[21:0022) |   | |   | └─Token(RightSquare) |]|
//@[22:0023) |   | |   ├─Token(LeftSquare) |[|
//@[23:0049) |   | |   ├─TernaryOperationSyntax
//@[23:0027) |   | |   | ├─VariableAccessSyntax
//@[23:0027) |   | |   | | └─IdentifierSyntax
//@[23:0027) |   | |   | |   └─Token(Identifier) |cond|
//@[28:0029) |   | |   | ├─Token(Question) |?|
//@[30:0034) |   | |   | ├─StringSyntax
//@[30:0034) |   | |   | | └─Token(StringComplete) |'id'|
//@[35:0036) |   | |   | ├─Token(Colon) |:|
//@[37:0049) |   | |   | └─StringSyntax
//@[37:0049) |   | |   |   └─Token(StringComplete) |'properties'|
//@[49:0050) |   | |   └─Token(RightSquare) |]|
//@[50:0051) |   | ├─Token(NewLine) |\n|
  case103: foos[zeroIndex][cond ? 'id' : 'properties']
//@[02:0054) |   | ├─ObjectPropertySyntax
//@[02:0009) |   | | ├─IdentifierSyntax
//@[02:0009) |   | | | └─Token(Identifier) |case103|
//@[09:0010) |   | | ├─Token(Colon) |:|
//@[11:0054) |   | | └─ArrayAccessSyntax
//@[11:0026) |   | |   ├─ArrayAccessSyntax
//@[11:0015) |   | |   | ├─VariableAccessSyntax
//@[11:0015) |   | |   | | └─IdentifierSyntax
//@[11:0015) |   | |   | |   └─Token(Identifier) |foos|
//@[15:0016) |   | |   | ├─Token(LeftSquare) |[|
//@[16:0025) |   | |   | ├─VariableAccessSyntax
//@[16:0025) |   | |   | | └─IdentifierSyntax
//@[16:0025) |   | |   | |   └─Token(Identifier) |zeroIndex|
//@[25:0026) |   | |   | └─Token(RightSquare) |]|
//@[26:0027) |   | |   ├─Token(LeftSquare) |[|
//@[27:0053) |   | |   ├─TernaryOperationSyntax
//@[27:0031) |   | |   | ├─VariableAccessSyntax
//@[27:0031) |   | |   | | └─IdentifierSyntax
//@[27:0031) |   | |   | |   └─Token(Identifier) |cond|
//@[32:0033) |   | |   | ├─Token(Question) |?|
//@[34:0038) |   | |   | ├─StringSyntax
//@[34:0038) |   | |   | | └─Token(StringComplete) |'id'|
//@[39:0040) |   | |   | ├─Token(Colon) |:|
//@[41:0053) |   | |   | └─StringSyntax
//@[41:0053) |   | |   |   └─Token(StringComplete) |'properties'|
//@[53:0054) |   | |   └─Token(RightSquare) |]|
//@[54:0055) |   | ├─Token(NewLine) |\n|
  case104: foos[otherIndex][cond ? 'id' : 'properties']
//@[02:0055) |   | ├─ObjectPropertySyntax
//@[02:0009) |   | | ├─IdentifierSyntax
//@[02:0009) |   | | | └─Token(Identifier) |case104|
//@[09:0010) |   | | ├─Token(Colon) |:|
//@[11:0055) |   | | └─ArrayAccessSyntax
//@[11:0027) |   | |   ├─ArrayAccessSyntax
//@[11:0015) |   | |   | ├─VariableAccessSyntax
//@[11:0015) |   | |   | | └─IdentifierSyntax
//@[11:0015) |   | |   | |   └─Token(Identifier) |foos|
//@[15:0016) |   | |   | ├─Token(LeftSquare) |[|
//@[16:0026) |   | |   | ├─VariableAccessSyntax
//@[16:0026) |   | |   | | └─IdentifierSyntax
//@[16:0026) |   | |   | |   └─Token(Identifier) |otherIndex|
//@[26:0027) |   | |   | └─Token(RightSquare) |]|
//@[27:0028) |   | |   ├─Token(LeftSquare) |[|
//@[28:0054) |   | |   ├─TernaryOperationSyntax
//@[28:0032) |   | |   | ├─VariableAccessSyntax
//@[28:0032) |   | |   | | └─IdentifierSyntax
//@[28:0032) |   | |   | |   └─Token(Identifier) |cond|
//@[33:0034) |   | |   | ├─Token(Question) |?|
//@[35:0039) |   | |   | ├─StringSyntax
//@[35:0039) |   | |   | | └─Token(StringComplete) |'id'|
//@[40:0041) |   | |   | ├─Token(Colon) |:|
//@[42:0054) |   | |   | └─StringSyntax
//@[42:0054) |   | |   |   └─Token(StringComplete) |'properties'|
//@[54:0055) |   | |   └─Token(RightSquare) |]|
//@[55:0056) |   | ├─Token(NewLine) |\n|
  case105: foo[cond ? 'id' : strParam]
//@[02:0038) |   | ├─ObjectPropertySyntax
//@[02:0009) |   | | ├─IdentifierSyntax
//@[02:0009) |   | | | └─Token(Identifier) |case105|
//@[09:0010) |   | | ├─Token(Colon) |:|
//@[11:0038) |   | | └─ArrayAccessSyntax
//@[11:0014) |   | |   ├─VariableAccessSyntax
//@[11:0014) |   | |   | └─IdentifierSyntax
//@[11:0014) |   | |   |   └─Token(Identifier) |foo|
//@[14:0015) |   | |   ├─Token(LeftSquare) |[|
//@[15:0037) |   | |   ├─TernaryOperationSyntax
//@[15:0019) |   | |   | ├─VariableAccessSyntax
//@[15:0019) |   | |   | | └─IdentifierSyntax
//@[15:0019) |   | |   | |   └─Token(Identifier) |cond|
//@[20:0021) |   | |   | ├─Token(Question) |?|
//@[22:0026) |   | |   | ├─StringSyntax
//@[22:0026) |   | |   | | └─Token(StringComplete) |'id'|
//@[27:0028) |   | |   | ├─Token(Colon) |:|
//@[29:0037) |   | |   | └─VariableAccessSyntax
//@[29:0037) |   | |   |   └─IdentifierSyntax
//@[29:0037) |   | |   |     └─Token(Identifier) |strParam|
//@[37:0038) |   | |   └─Token(RightSquare) |]|
//@[38:0039) |   | ├─Token(NewLine) |\n|
  case106: existingFoo[cond ? 'id' : strParam]
//@[02:0046) |   | ├─ObjectPropertySyntax
//@[02:0009) |   | | ├─IdentifierSyntax
//@[02:0009) |   | | | └─Token(Identifier) |case106|
//@[09:0010) |   | | ├─Token(Colon) |:|
//@[11:0046) |   | | └─ArrayAccessSyntax
//@[11:0022) |   | |   ├─VariableAccessSyntax
//@[11:0022) |   | |   | └─IdentifierSyntax
//@[11:0022) |   | |   |   └─Token(Identifier) |existingFoo|
//@[22:0023) |   | |   ├─Token(LeftSquare) |[|
//@[23:0045) |   | |   ├─TernaryOperationSyntax
//@[23:0027) |   | |   | ├─VariableAccessSyntax
//@[23:0027) |   | |   | | └─IdentifierSyntax
//@[23:0027) |   | |   | |   └─Token(Identifier) |cond|
//@[28:0029) |   | |   | ├─Token(Question) |?|
//@[30:0034) |   | |   | ├─StringSyntax
//@[30:0034) |   | |   | | └─Token(StringComplete) |'id'|
//@[35:0036) |   | |   | ├─Token(Colon) |:|
//@[37:0045) |   | |   | └─VariableAccessSyntax
//@[37:0045) |   | |   |   └─IdentifierSyntax
//@[37:0045) |   | |   |     └─Token(Identifier) |strParam|
//@[45:0046) |   | |   └─Token(RightSquare) |]|
//@[46:0047) |   | ├─Token(NewLine) |\n|
  case107: foo::fooChild[cond ? 'id' : strParam]
//@[02:0048) |   | ├─ObjectPropertySyntax
//@[02:0009) |   | | ├─IdentifierSyntax
//@[02:0009) |   | | | └─Token(Identifier) |case107|
//@[09:0010) |   | | ├─Token(Colon) |:|
//@[11:0048) |   | | └─ArrayAccessSyntax
//@[11:0024) |   | |   ├─ResourceAccessSyntax
//@[11:0014) |   | |   | ├─VariableAccessSyntax
//@[11:0014) |   | |   | | └─IdentifierSyntax
//@[11:0014) |   | |   | |   └─Token(Identifier) |foo|
//@[14:0016) |   | |   | ├─Token(DoubleColon) |::|
//@[16:0024) |   | |   | └─IdentifierSyntax
//@[16:0024) |   | |   |   └─Token(Identifier) |fooChild|
//@[24:0025) |   | |   ├─Token(LeftSquare) |[|
//@[25:0047) |   | |   ├─TernaryOperationSyntax
//@[25:0029) |   | |   | ├─VariableAccessSyntax
//@[25:0029) |   | |   | | └─IdentifierSyntax
//@[25:0029) |   | |   | |   └─Token(Identifier) |cond|
//@[30:0031) |   | |   | ├─Token(Question) |?|
//@[32:0036) |   | |   | ├─StringSyntax
//@[32:0036) |   | |   | | └─Token(StringComplete) |'id'|
//@[37:0038) |   | |   | ├─Token(Colon) |:|
//@[39:0047) |   | |   | └─VariableAccessSyntax
//@[39:0047) |   | |   |   └─IdentifierSyntax
//@[39:0047) |   | |   |     └─Token(Identifier) |strParam|
//@[47:0048) |   | |   └─Token(RightSquare) |]|
//@[48:0049) |   | ├─Token(NewLine) |\n|
  case108: foos[0][cond ? 'id' : strParam]
//@[02:0042) |   | ├─ObjectPropertySyntax
//@[02:0009) |   | | ├─IdentifierSyntax
//@[02:0009) |   | | | └─Token(Identifier) |case108|
//@[09:0010) |   | | ├─Token(Colon) |:|
//@[11:0042) |   | | └─ArrayAccessSyntax
//@[11:0018) |   | |   ├─ArrayAccessSyntax
//@[11:0015) |   | |   | ├─VariableAccessSyntax
//@[11:0015) |   | |   | | └─IdentifierSyntax
//@[11:0015) |   | |   | |   └─Token(Identifier) |foos|
//@[15:0016) |   | |   | ├─Token(LeftSquare) |[|
//@[16:0017) |   | |   | ├─IntegerLiteralSyntax
//@[16:0017) |   | |   | | └─Token(Integer) |0|
//@[17:0018) |   | |   | └─Token(RightSquare) |]|
//@[18:0019) |   | |   ├─Token(LeftSquare) |[|
//@[19:0041) |   | |   ├─TernaryOperationSyntax
//@[19:0023) |   | |   | ├─VariableAccessSyntax
//@[19:0023) |   | |   | | └─IdentifierSyntax
//@[19:0023) |   | |   | |   └─Token(Identifier) |cond|
//@[24:0025) |   | |   | ├─Token(Question) |?|
//@[26:0030) |   | |   | ├─StringSyntax
//@[26:0030) |   | |   | | └─Token(StringComplete) |'id'|
//@[31:0032) |   | |   | ├─Token(Colon) |:|
//@[33:0041) |   | |   | └─VariableAccessSyntax
//@[33:0041) |   | |   |   └─IdentifierSyntax
//@[33:0041) |   | |   |     └─Token(Identifier) |strParam|
//@[41:0042) |   | |   └─Token(RightSquare) |]|
//@[42:0043) |   | ├─Token(NewLine) |\n|
  case109: foos[i][cond ? 'id' : strParam]
//@[02:0042) |   | ├─ObjectPropertySyntax
//@[02:0009) |   | | ├─IdentifierSyntax
//@[02:0009) |   | | | └─Token(Identifier) |case109|
//@[09:0010) |   | | ├─Token(Colon) |:|
//@[11:0042) |   | | └─ArrayAccessSyntax
//@[11:0018) |   | |   ├─ArrayAccessSyntax
//@[11:0015) |   | |   | ├─VariableAccessSyntax
//@[11:0015) |   | |   | | └─IdentifierSyntax
//@[11:0015) |   | |   | |   └─Token(Identifier) |foos|
//@[15:0016) |   | |   | ├─Token(LeftSquare) |[|
//@[16:0017) |   | |   | ├─VariableAccessSyntax
//@[16:0017) |   | |   | | └─IdentifierSyntax
//@[16:0017) |   | |   | |   └─Token(Identifier) |i|
//@[17:0018) |   | |   | └─Token(RightSquare) |]|
//@[18:0019) |   | |   ├─Token(LeftSquare) |[|
//@[19:0041) |   | |   ├─TernaryOperationSyntax
//@[19:0023) |   | |   | ├─VariableAccessSyntax
//@[19:0023) |   | |   | | └─IdentifierSyntax
//@[19:0023) |   | |   | |   └─Token(Identifier) |cond|
//@[24:0025) |   | |   | ├─Token(Question) |?|
//@[26:0030) |   | |   | ├─StringSyntax
//@[26:0030) |   | |   | | └─Token(StringComplete) |'id'|
//@[31:0032) |   | |   | ├─Token(Colon) |:|
//@[33:0041) |   | |   | └─VariableAccessSyntax
//@[33:0041) |   | |   |   └─IdentifierSyntax
//@[33:0041) |   | |   |     └─Token(Identifier) |strParam|
//@[41:0042) |   | |   └─Token(RightSquare) |]|
//@[42:0043) |   | ├─Token(NewLine) |\n|
  case110: foos[i + 2][cond ? 'id' : strParam]
//@[02:0046) |   | ├─ObjectPropertySyntax
//@[02:0009) |   | | ├─IdentifierSyntax
//@[02:0009) |   | | | └─Token(Identifier) |case110|
//@[09:0010) |   | | ├─Token(Colon) |:|
//@[11:0046) |   | | └─ArrayAccessSyntax
//@[11:0022) |   | |   ├─ArrayAccessSyntax
//@[11:0015) |   | |   | ├─VariableAccessSyntax
//@[11:0015) |   | |   | | └─IdentifierSyntax
//@[11:0015) |   | |   | |   └─Token(Identifier) |foos|
//@[15:0016) |   | |   | ├─Token(LeftSquare) |[|
//@[16:0021) |   | |   | ├─BinaryOperationSyntax
//@[16:0017) |   | |   | | ├─VariableAccessSyntax
//@[16:0017) |   | |   | | | └─IdentifierSyntax
//@[16:0017) |   | |   | | |   └─Token(Identifier) |i|
//@[18:0019) |   | |   | | ├─Token(Plus) |+|
//@[20:0021) |   | |   | | └─IntegerLiteralSyntax
//@[20:0021) |   | |   | |   └─Token(Integer) |2|
//@[21:0022) |   | |   | └─Token(RightSquare) |]|
//@[22:0023) |   | |   ├─Token(LeftSquare) |[|
//@[23:0045) |   | |   ├─TernaryOperationSyntax
//@[23:0027) |   | |   | ├─VariableAccessSyntax
//@[23:0027) |   | |   | | └─IdentifierSyntax
//@[23:0027) |   | |   | |   └─Token(Identifier) |cond|
//@[28:0029) |   | |   | ├─Token(Question) |?|
//@[30:0034) |   | |   | ├─StringSyntax
//@[30:0034) |   | |   | | └─Token(StringComplete) |'id'|
//@[35:0036) |   | |   | ├─Token(Colon) |:|
//@[37:0045) |   | |   | └─VariableAccessSyntax
//@[37:0045) |   | |   |   └─IdentifierSyntax
//@[37:0045) |   | |   |     └─Token(Identifier) |strParam|
//@[45:0046) |   | |   └─Token(RightSquare) |]|
//@[46:0047) |   | ├─Token(NewLine) |\n|
  case111: foos[zeroIndex][cond ? 'id' : strParam]
//@[02:0050) |   | ├─ObjectPropertySyntax
//@[02:0009) |   | | ├─IdentifierSyntax
//@[02:0009) |   | | | └─Token(Identifier) |case111|
//@[09:0010) |   | | ├─Token(Colon) |:|
//@[11:0050) |   | | └─ArrayAccessSyntax
//@[11:0026) |   | |   ├─ArrayAccessSyntax
//@[11:0015) |   | |   | ├─VariableAccessSyntax
//@[11:0015) |   | |   | | └─IdentifierSyntax
//@[11:0015) |   | |   | |   └─Token(Identifier) |foos|
//@[15:0016) |   | |   | ├─Token(LeftSquare) |[|
//@[16:0025) |   | |   | ├─VariableAccessSyntax
//@[16:0025) |   | |   | | └─IdentifierSyntax
//@[16:0025) |   | |   | |   └─Token(Identifier) |zeroIndex|
//@[25:0026) |   | |   | └─Token(RightSquare) |]|
//@[26:0027) |   | |   ├─Token(LeftSquare) |[|
//@[27:0049) |   | |   ├─TernaryOperationSyntax
//@[27:0031) |   | |   | ├─VariableAccessSyntax
//@[27:0031) |   | |   | | └─IdentifierSyntax
//@[27:0031) |   | |   | |   └─Token(Identifier) |cond|
//@[32:0033) |   | |   | ├─Token(Question) |?|
//@[34:0038) |   | |   | ├─StringSyntax
//@[34:0038) |   | |   | | └─Token(StringComplete) |'id'|
//@[39:0040) |   | |   | ├─Token(Colon) |:|
//@[41:0049) |   | |   | └─VariableAccessSyntax
//@[41:0049) |   | |   |   └─IdentifierSyntax
//@[41:0049) |   | |   |     └─Token(Identifier) |strParam|
//@[49:0050) |   | |   └─Token(RightSquare) |]|
//@[50:0051) |   | ├─Token(NewLine) |\n|
  case112: foos[otherIndex][cond ? 'id' : strParam]
//@[02:0051) |   | ├─ObjectPropertySyntax
//@[02:0009) |   | | ├─IdentifierSyntax
//@[02:0009) |   | | | └─Token(Identifier) |case112|
//@[09:0010) |   | | ├─Token(Colon) |:|
//@[11:0051) |   | | └─ArrayAccessSyntax
//@[11:0027) |   | |   ├─ArrayAccessSyntax
//@[11:0015) |   | |   | ├─VariableAccessSyntax
//@[11:0015) |   | |   | | └─IdentifierSyntax
//@[11:0015) |   | |   | |   └─Token(Identifier) |foos|
//@[15:0016) |   | |   | ├─Token(LeftSquare) |[|
//@[16:0026) |   | |   | ├─VariableAccessSyntax
//@[16:0026) |   | |   | | └─IdentifierSyntax
//@[16:0026) |   | |   | |   └─Token(Identifier) |otherIndex|
//@[26:0027) |   | |   | └─Token(RightSquare) |]|
//@[27:0028) |   | |   ├─Token(LeftSquare) |[|
//@[28:0050) |   | |   ├─TernaryOperationSyntax
//@[28:0032) |   | |   | ├─VariableAccessSyntax
//@[28:0032) |   | |   | | └─IdentifierSyntax
//@[28:0032) |   | |   | |   └─Token(Identifier) |cond|
//@[33:0034) |   | |   | ├─Token(Question) |?|
//@[35:0039) |   | |   | ├─StringSyntax
//@[35:0039) |   | |   | | └─Token(StringComplete) |'id'|
//@[40:0041) |   | |   | ├─Token(Colon) |:|
//@[42:0050) |   | |   | └─VariableAccessSyntax
//@[42:0050) |   | |   |   └─IdentifierSyntax
//@[42:0050) |   | |   |     └─Token(Identifier) |strParam|
//@[50:0051) |   | |   └─Token(RightSquare) |]|
//@[51:0052) |   | ├─Token(NewLine) |\n|
}]
//@[00:0001) |   | └─Token(RightBrace) |}|
//@[01:0002) |   └─Token(RightSquare) |]|
//@[02:0003) ├─Token(NewLine) |\n|
var varForBodyInvalidRuntimeUsageExpression = [for i in range(0, 2): foo.properties]
//@[00:0084) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0043) | ├─IdentifierSyntax
//@[04:0043) | | └─Token(Identifier) |varForBodyInvalidRuntimeUsageExpression|
//@[44:0045) | ├─Token(Assignment) |=|
//@[46:0084) | └─ForSyntax
//@[46:0047) |   ├─Token(LeftSquare) |[|
//@[47:0050) |   ├─Token(Identifier) |for|
//@[51:0052) |   ├─LocalVariableSyntax
//@[51:0052) |   | └─IdentifierSyntax
//@[51:0052) |   |   └─Token(Identifier) |i|
//@[53:0055) |   ├─Token(Identifier) |in|
//@[56:0067) |   ├─FunctionCallSyntax
//@[56:0061) |   | ├─IdentifierSyntax
//@[56:0061) |   | | └─Token(Identifier) |range|
//@[61:0062) |   | ├─Token(LeftParen) |(|
//@[62:0063) |   | ├─FunctionArgumentSyntax
//@[62:0063) |   | | └─IntegerLiteralSyntax
//@[62:0063) |   | |   └─Token(Integer) |0|
//@[63:0064) |   | ├─Token(Comma) |,|
//@[65:0066) |   | ├─FunctionArgumentSyntax
//@[65:0066) |   | | └─IntegerLiteralSyntax
//@[65:0066) |   | |   └─Token(Integer) |2|
//@[66:0067) |   | └─Token(RightParen) |)|
//@[67:0068) |   ├─Token(Colon) |:|
//@[69:0083) |   ├─PropertyAccessSyntax
//@[69:0072) |   | ├─VariableAccessSyntax
//@[69:0072) |   | | └─IdentifierSyntax
//@[69:0072) |   | |   └─Token(Identifier) |foo|
//@[72:0073) |   | ├─Token(Dot) |.|
//@[73:0083) |   | └─IdentifierSyntax
//@[73:0083) |   |   └─Token(Identifier) |properties|
//@[83:0084) |   └─Token(RightSquare) |]|
//@[84:0085) ├─Token(NewLine) |\n|
var varForBodyInvalidRuntimeUsageInterpolatedKey = [for i in range(0, 2): {
//@[00:0129) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0048) | ├─IdentifierSyntax
//@[04:0048) | | └─Token(Identifier) |varForBodyInvalidRuntimeUsageInterpolatedKey|
//@[49:0050) | ├─Token(Assignment) |=|
//@[51:0129) | └─ForSyntax
//@[51:0052) |   ├─Token(LeftSquare) |[|
//@[52:0055) |   ├─Token(Identifier) |for|
//@[56:0057) |   ├─LocalVariableSyntax
//@[56:0057) |   | └─IdentifierSyntax
//@[56:0057) |   |   └─Token(Identifier) |i|
//@[58:0060) |   ├─Token(Identifier) |in|
//@[61:0072) |   ├─FunctionCallSyntax
//@[61:0066) |   | ├─IdentifierSyntax
//@[61:0066) |   | | └─Token(Identifier) |range|
//@[66:0067) |   | ├─Token(LeftParen) |(|
//@[67:0068) |   | ├─FunctionArgumentSyntax
//@[67:0068) |   | | └─IntegerLiteralSyntax
//@[67:0068) |   | |   └─Token(Integer) |0|
//@[68:0069) |   | ├─Token(Comma) |,|
//@[70:0071) |   | ├─FunctionArgumentSyntax
//@[70:0071) |   | | └─IntegerLiteralSyntax
//@[70:0071) |   | |   └─Token(Integer) |2|
//@[71:0072) |   | └─Token(RightParen) |)|
//@[72:0073) |   ├─Token(Colon) |:|
//@[74:0128) |   ├─ObjectSyntax
//@[74:0075) |   | ├─Token(LeftBrace) |{|
//@[75:0076) |   | ├─Token(NewLine) |\n|
  '${foos[i].properties.accessTier}': 'accessTier'
//@[02:0050) |   | ├─ObjectPropertySyntax
//@[02:0036) |   | | ├─StringSyntax
//@[02:0005) |   | | | ├─Token(StringLeftPiece) |'${|
//@[05:0034) |   | | | ├─PropertyAccessSyntax
//@[05:0023) |   | | | | ├─PropertyAccessSyntax
//@[05:0012) |   | | | | | ├─ArrayAccessSyntax
//@[05:0009) |   | | | | | | ├─VariableAccessSyntax
//@[05:0009) |   | | | | | | | └─IdentifierSyntax
//@[05:0009) |   | | | | | | |   └─Token(Identifier) |foos|
//@[09:0010) |   | | | | | | ├─Token(LeftSquare) |[|
//@[10:0011) |   | | | | | | ├─VariableAccessSyntax
//@[10:0011) |   | | | | | | | └─IdentifierSyntax
//@[10:0011) |   | | | | | | |   └─Token(Identifier) |i|
//@[11:0012) |   | | | | | | └─Token(RightSquare) |]|
//@[12:0013) |   | | | | | ├─Token(Dot) |.|
//@[13:0023) |   | | | | | └─IdentifierSyntax
//@[13:0023) |   | | | | |   └─Token(Identifier) |properties|
//@[23:0024) |   | | | | ├─Token(Dot) |.|
//@[24:0034) |   | | | | └─IdentifierSyntax
//@[24:0034) |   | | | |   └─Token(Identifier) |accessTier|
//@[34:0036) |   | | | └─Token(StringRightPiece) |}'|
//@[36:0037) |   | | ├─Token(Colon) |:|
//@[38:0050) |   | | └─StringSyntax
//@[38:0050) |   | |   └─Token(StringComplete) |'accessTier'|
//@[50:0051) |   | ├─Token(NewLine) |\n|
}]
//@[00:0001) |   | └─Token(RightBrace) |}|
//@[01:0002) |   └─Token(RightSquare) |]|
//@[02:0002) └─Token(EndOfFile) ||
