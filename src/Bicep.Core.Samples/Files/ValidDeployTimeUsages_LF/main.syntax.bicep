resource foo 'Microsoft.Storage/storageAccounts@2022-09-01' = {
//@[00:4116) ProgramSyntax
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

var varForBodyOkDeployTimeUsages = [for i in range(0, 2): {
//@[00:3342) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0032) | ├─IdentifierSyntax
//@[04:0032) | | └─Token(Identifier) |varForBodyOkDeployTimeUsages|
//@[33:0034) | ├─Token(Assignment) |=|
//@[35:3342) | └─ForSyntax
//@[35:0036) |   ├─Token(LeftSquare) |[|
//@[36:0039) |   ├─Token(Identifier) |for|
//@[40:0041) |   ├─LocalVariableSyntax
//@[40:0041) |   | └─IdentifierSyntax
//@[40:0041) |   |   └─Token(Identifier) |i|
//@[42:0044) |   ├─Token(Identifier) |in|
//@[45:0056) |   ├─FunctionCallSyntax
//@[45:0050) |   | ├─IdentifierSyntax
//@[45:0050) |   | | └─Token(Identifier) |range|
//@[50:0051) |   | ├─Token(LeftParen) |(|
//@[51:0052) |   | ├─FunctionArgumentSyntax
//@[51:0052) |   | | └─IntegerLiteralSyntax
//@[51:0052) |   | |   └─Token(Integer) |0|
//@[52:0053) |   | ├─Token(Comma) |,|
//@[54:0055) |   | ├─FunctionArgumentSyntax
//@[54:0055) |   | | └─IntegerLiteralSyntax
//@[54:0055) |   | |   └─Token(Integer) |2|
//@[55:0056) |   | └─Token(RightParen) |)|
//@[56:0057) |   ├─Token(Colon) |:|
//@[58:3341) |   ├─ObjectSyntax
//@[58:0059) |   | ├─Token(LeftBrace) |{|
//@[59:0060) |   | ├─Token(NewLine) |\n|
  case1: foo.id
//@[02:0015) |   | ├─ObjectPropertySyntax
//@[02:0007) |   | | ├─IdentifierSyntax
//@[02:0007) |   | | | └─Token(Identifier) |case1|
//@[07:0008) |   | | ├─Token(Colon) |:|
//@[09:0015) |   | | └─PropertyAccessSyntax
//@[09:0012) |   | |   ├─VariableAccessSyntax
//@[09:0012) |   | |   | └─IdentifierSyntax
//@[09:0012) |   | |   |   └─Token(Identifier) |foo|
//@[12:0013) |   | |   ├─Token(Dot) |.|
//@[13:0015) |   | |   └─IdentifierSyntax
//@[13:0015) |   | |     └─Token(Identifier) |id|
//@[15:0016) |   | ├─Token(NewLine) |\n|
  case2: existingFoo.id
//@[02:0023) |   | ├─ObjectPropertySyntax
//@[02:0007) |   | | ├─IdentifierSyntax
//@[02:0007) |   | | | └─Token(Identifier) |case2|
//@[07:0008) |   | | ├─Token(Colon) |:|
//@[09:0023) |   | | └─PropertyAccessSyntax
//@[09:0020) |   | |   ├─VariableAccessSyntax
//@[09:0020) |   | |   | └─IdentifierSyntax
//@[09:0020) |   | |   |   └─Token(Identifier) |existingFoo|
//@[20:0021) |   | |   ├─Token(Dot) |.|
//@[21:0023) |   | |   └─IdentifierSyntax
//@[21:0023) |   | |     └─Token(Identifier) |id|
//@[23:0024) |   | ├─Token(NewLine) |\n|
  case3: foo::fooChild.id
//@[02:0025) |   | ├─ObjectPropertySyntax
//@[02:0007) |   | | ├─IdentifierSyntax
//@[02:0007) |   | | | └─Token(Identifier) |case3|
//@[07:0008) |   | | ├─Token(Colon) |:|
//@[09:0025) |   | | └─PropertyAccessSyntax
//@[09:0022) |   | |   ├─ResourceAccessSyntax
//@[09:0012) |   | |   | ├─VariableAccessSyntax
//@[09:0012) |   | |   | | └─IdentifierSyntax
//@[09:0012) |   | |   | |   └─Token(Identifier) |foo|
//@[12:0014) |   | |   | ├─Token(DoubleColon) |::|
//@[14:0022) |   | |   | └─IdentifierSyntax
//@[14:0022) |   | |   |   └─Token(Identifier) |fooChild|
//@[22:0023) |   | |   ├─Token(Dot) |.|
//@[23:0025) |   | |   └─IdentifierSyntax
//@[23:0025) |   | |     └─Token(Identifier) |id|
//@[25:0026) |   | ├─Token(NewLine) |\n|
  case4: foos[0].id
//@[02:0019) |   | ├─ObjectPropertySyntax
//@[02:0007) |   | | ├─IdentifierSyntax
//@[02:0007) |   | | | └─Token(Identifier) |case4|
//@[07:0008) |   | | ├─Token(Colon) |:|
//@[09:0019) |   | | └─PropertyAccessSyntax
//@[09:0016) |   | |   ├─ArrayAccessSyntax
//@[09:0013) |   | |   | ├─VariableAccessSyntax
//@[09:0013) |   | |   | | └─IdentifierSyntax
//@[09:0013) |   | |   | |   └─Token(Identifier) |foos|
//@[13:0014) |   | |   | ├─Token(LeftSquare) |[|
//@[14:0015) |   | |   | ├─IntegerLiteralSyntax
//@[14:0015) |   | |   | | └─Token(Integer) |0|
//@[15:0016) |   | |   | └─Token(RightSquare) |]|
//@[16:0017) |   | |   ├─Token(Dot) |.|
//@[17:0019) |   | |   └─IdentifierSyntax
//@[17:0019) |   | |     └─Token(Identifier) |id|
//@[19:0020) |   | ├─Token(NewLine) |\n|
  case5: foos[i].id
//@[02:0019) |   | ├─ObjectPropertySyntax
//@[02:0007) |   | | ├─IdentifierSyntax
//@[02:0007) |   | | | └─Token(Identifier) |case5|
//@[07:0008) |   | | ├─Token(Colon) |:|
//@[09:0019) |   | | └─PropertyAccessSyntax
//@[09:0016) |   | |   ├─ArrayAccessSyntax
//@[09:0013) |   | |   | ├─VariableAccessSyntax
//@[09:0013) |   | |   | | └─IdentifierSyntax
//@[09:0013) |   | |   | |   └─Token(Identifier) |foos|
//@[13:0014) |   | |   | ├─Token(LeftSquare) |[|
//@[14:0015) |   | |   | ├─VariableAccessSyntax
//@[14:0015) |   | |   | | └─IdentifierSyntax
//@[14:0015) |   | |   | |   └─Token(Identifier) |i|
//@[15:0016) |   | |   | └─Token(RightSquare) |]|
//@[16:0017) |   | |   ├─Token(Dot) |.|
//@[17:0019) |   | |   └─IdentifierSyntax
//@[17:0019) |   | |     └─Token(Identifier) |id|
//@[19:0020) |   | ├─Token(NewLine) |\n|
  case6: foos[i + 2].id
//@[02:0023) |   | ├─ObjectPropertySyntax
//@[02:0007) |   | | ├─IdentifierSyntax
//@[02:0007) |   | | | └─Token(Identifier) |case6|
//@[07:0008) |   | | ├─Token(Colon) |:|
//@[09:0023) |   | | └─PropertyAccessSyntax
//@[09:0020) |   | |   ├─ArrayAccessSyntax
//@[09:0013) |   | |   | ├─VariableAccessSyntax
//@[09:0013) |   | |   | | └─IdentifierSyntax
//@[09:0013) |   | |   | |   └─Token(Identifier) |foos|
//@[13:0014) |   | |   | ├─Token(LeftSquare) |[|
//@[14:0019) |   | |   | ├─BinaryOperationSyntax
//@[14:0015) |   | |   | | ├─VariableAccessSyntax
//@[14:0015) |   | |   | | | └─IdentifierSyntax
//@[14:0015) |   | |   | | |   └─Token(Identifier) |i|
//@[16:0017) |   | |   | | ├─Token(Plus) |+|
//@[18:0019) |   | |   | | └─IntegerLiteralSyntax
//@[18:0019) |   | |   | |   └─Token(Integer) |2|
//@[19:0020) |   | |   | └─Token(RightSquare) |]|
//@[20:0021) |   | |   ├─Token(Dot) |.|
//@[21:0023) |   | |   └─IdentifierSyntax
//@[21:0023) |   | |     └─Token(Identifier) |id|
//@[23:0024) |   | ├─Token(NewLine) |\n|
  case7: foos[zeroIndex].id
//@[02:0027) |   | ├─ObjectPropertySyntax
//@[02:0007) |   | | ├─IdentifierSyntax
//@[02:0007) |   | | | └─Token(Identifier) |case7|
//@[07:0008) |   | | ├─Token(Colon) |:|
//@[09:0027) |   | | └─PropertyAccessSyntax
//@[09:0024) |   | |   ├─ArrayAccessSyntax
//@[09:0013) |   | |   | ├─VariableAccessSyntax
//@[09:0013) |   | |   | | └─IdentifierSyntax
//@[09:0013) |   | |   | |   └─Token(Identifier) |foos|
//@[13:0014) |   | |   | ├─Token(LeftSquare) |[|
//@[14:0023) |   | |   | ├─VariableAccessSyntax
//@[14:0023) |   | |   | | └─IdentifierSyntax
//@[14:0023) |   | |   | |   └─Token(Identifier) |zeroIndex|
//@[23:0024) |   | |   | └─Token(RightSquare) |]|
//@[24:0025) |   | |   ├─Token(Dot) |.|
//@[25:0027) |   | |   └─IdentifierSyntax
//@[25:0027) |   | |     └─Token(Identifier) |id|
//@[27:0028) |   | ├─Token(NewLine) |\n|
  case8: foos[otherIndex].id
//@[02:0028) |   | ├─ObjectPropertySyntax
//@[02:0007) |   | | ├─IdentifierSyntax
//@[02:0007) |   | | | └─Token(Identifier) |case8|
//@[07:0008) |   | | ├─Token(Colon) |:|
//@[09:0028) |   | | └─PropertyAccessSyntax
//@[09:0025) |   | |   ├─ArrayAccessSyntax
//@[09:0013) |   | |   | ├─VariableAccessSyntax
//@[09:0013) |   | |   | | └─IdentifierSyntax
//@[09:0013) |   | |   | |   └─Token(Identifier) |foos|
//@[13:0014) |   | |   | ├─Token(LeftSquare) |[|
//@[14:0024) |   | |   | ├─VariableAccessSyntax
//@[14:0024) |   | |   | | └─IdentifierSyntax
//@[14:0024) |   | |   | |   └─Token(Identifier) |otherIndex|
//@[24:0025) |   | |   | └─Token(RightSquare) |]|
//@[25:0026) |   | |   ├─Token(Dot) |.|
//@[26:0028) |   | |   └─IdentifierSyntax
//@[26:0028) |   | |     └─Token(Identifier) |id|
//@[28:0029) |   | ├─Token(NewLine) |\n|
  case9: foo['id']
//@[02:0018) |   | ├─ObjectPropertySyntax
//@[02:0007) |   | | ├─IdentifierSyntax
//@[02:0007) |   | | | └─Token(Identifier) |case9|
//@[07:0008) |   | | ├─Token(Colon) |:|
//@[09:0018) |   | | └─ArrayAccessSyntax
//@[09:0012) |   | |   ├─VariableAccessSyntax
//@[09:0012) |   | |   | └─IdentifierSyntax
//@[09:0012) |   | |   |   └─Token(Identifier) |foo|
//@[12:0013) |   | |   ├─Token(LeftSquare) |[|
//@[13:0017) |   | |   ├─StringSyntax
//@[13:0017) |   | |   | └─Token(StringComplete) |'id'|
//@[17:0018) |   | |   └─Token(RightSquare) |]|
//@[18:0019) |   | ├─Token(NewLine) |\n|
  case10: existingFoo['id']
//@[02:0027) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case10|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0027) |   | | └─ArrayAccessSyntax
//@[10:0021) |   | |   ├─VariableAccessSyntax
//@[10:0021) |   | |   | └─IdentifierSyntax
//@[10:0021) |   | |   |   └─Token(Identifier) |existingFoo|
//@[21:0022) |   | |   ├─Token(LeftSquare) |[|
//@[22:0026) |   | |   ├─StringSyntax
//@[22:0026) |   | |   | └─Token(StringComplete) |'id'|
//@[26:0027) |   | |   └─Token(RightSquare) |]|
//@[27:0028) |   | ├─Token(NewLine) |\n|
  case11: foo::fooChild['id']
//@[02:0029) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case11|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0029) |   | | └─ArrayAccessSyntax
//@[10:0023) |   | |   ├─ResourceAccessSyntax
//@[10:0013) |   | |   | ├─VariableAccessSyntax
//@[10:0013) |   | |   | | └─IdentifierSyntax
//@[10:0013) |   | |   | |   └─Token(Identifier) |foo|
//@[13:0015) |   | |   | ├─Token(DoubleColon) |::|
//@[15:0023) |   | |   | └─IdentifierSyntax
//@[15:0023) |   | |   |   └─Token(Identifier) |fooChild|
//@[23:0024) |   | |   ├─Token(LeftSquare) |[|
//@[24:0028) |   | |   ├─StringSyntax
//@[24:0028) |   | |   | └─Token(StringComplete) |'id'|
//@[28:0029) |   | |   └─Token(RightSquare) |]|
//@[29:0030) |   | ├─Token(NewLine) |\n|
  case12: foos[0]['id']
//@[02:0023) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case12|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0023) |   | | └─ArrayAccessSyntax
//@[10:0017) |   | |   ├─ArrayAccessSyntax
//@[10:0014) |   | |   | ├─VariableAccessSyntax
//@[10:0014) |   | |   | | └─IdentifierSyntax
//@[10:0014) |   | |   | |   └─Token(Identifier) |foos|
//@[14:0015) |   | |   | ├─Token(LeftSquare) |[|
//@[15:0016) |   | |   | ├─IntegerLiteralSyntax
//@[15:0016) |   | |   | | └─Token(Integer) |0|
//@[16:0017) |   | |   | └─Token(RightSquare) |]|
//@[17:0018) |   | |   ├─Token(LeftSquare) |[|
//@[18:0022) |   | |   ├─StringSyntax
//@[18:0022) |   | |   | └─Token(StringComplete) |'id'|
//@[22:0023) |   | |   └─Token(RightSquare) |]|
//@[23:0024) |   | ├─Token(NewLine) |\n|
  case13: foos[i]['id']
//@[02:0023) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case13|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0023) |   | | └─ArrayAccessSyntax
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
//@[18:0022) |   | |   ├─StringSyntax
//@[18:0022) |   | |   | └─Token(StringComplete) |'id'|
//@[22:0023) |   | |   └─Token(RightSquare) |]|
//@[23:0024) |   | ├─Token(NewLine) |\n|
  case14: foos[i + 2]['id']
//@[02:0027) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case14|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0027) |   | | └─ArrayAccessSyntax
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
//@[22:0026) |   | |   ├─StringSyntax
//@[22:0026) |   | |   | └─Token(StringComplete) |'id'|
//@[26:0027) |   | |   └─Token(RightSquare) |]|
//@[27:0028) |   | ├─Token(NewLine) |\n|
  case15: foos[zeroIndex]['id']
//@[02:0031) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case15|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0031) |   | | └─ArrayAccessSyntax
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
//@[26:0030) |   | |   ├─StringSyntax
//@[26:0030) |   | |   | └─Token(StringComplete) |'id'|
//@[30:0031) |   | |   └─Token(RightSquare) |]|
//@[31:0032) |   | ├─Token(NewLine) |\n|
  case16: foos[otherIndex]['id']
//@[02:0032) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case16|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0032) |   | | └─ArrayAccessSyntax
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
//@[27:0031) |   | |   ├─StringSyntax
//@[27:0031) |   | |   | └─Token(StringComplete) |'id'|
//@[31:0032) |   | |   └─Token(RightSquare) |]|
//@[32:0033) |   | ├─Token(NewLine) |\n|
  case17: foo[idAccessor]
//@[02:0025) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case17|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0025) |   | | └─ArrayAccessSyntax
//@[10:0013) |   | |   ├─VariableAccessSyntax
//@[10:0013) |   | |   | └─IdentifierSyntax
//@[10:0013) |   | |   |   └─Token(Identifier) |foo|
//@[13:0014) |   | |   ├─Token(LeftSquare) |[|
//@[14:0024) |   | |   ├─VariableAccessSyntax
//@[14:0024) |   | |   | └─IdentifierSyntax
//@[14:0024) |   | |   |   └─Token(Identifier) |idAccessor|
//@[24:0025) |   | |   └─Token(RightSquare) |]|
//@[25:0026) |   | ├─Token(NewLine) |\n|
  case18: existingFoo[idAccessor]
//@[02:0033) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case18|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0033) |   | | └─ArrayAccessSyntax
//@[10:0021) |   | |   ├─VariableAccessSyntax
//@[10:0021) |   | |   | └─IdentifierSyntax
//@[10:0021) |   | |   |   └─Token(Identifier) |existingFoo|
//@[21:0022) |   | |   ├─Token(LeftSquare) |[|
//@[22:0032) |   | |   ├─VariableAccessSyntax
//@[22:0032) |   | |   | └─IdentifierSyntax
//@[22:0032) |   | |   |   └─Token(Identifier) |idAccessor|
//@[32:0033) |   | |   └─Token(RightSquare) |]|
//@[33:0034) |   | ├─Token(NewLine) |\n|
  case19: foo::fooChild[idAccessor]
//@[02:0035) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case19|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0035) |   | | └─ArrayAccessSyntax
//@[10:0023) |   | |   ├─ResourceAccessSyntax
//@[10:0013) |   | |   | ├─VariableAccessSyntax
//@[10:0013) |   | |   | | └─IdentifierSyntax
//@[10:0013) |   | |   | |   └─Token(Identifier) |foo|
//@[13:0015) |   | |   | ├─Token(DoubleColon) |::|
//@[15:0023) |   | |   | └─IdentifierSyntax
//@[15:0023) |   | |   |   └─Token(Identifier) |fooChild|
//@[23:0024) |   | |   ├─Token(LeftSquare) |[|
//@[24:0034) |   | |   ├─VariableAccessSyntax
//@[24:0034) |   | |   | └─IdentifierSyntax
//@[24:0034) |   | |   |   └─Token(Identifier) |idAccessor|
//@[34:0035) |   | |   └─Token(RightSquare) |]|
//@[35:0036) |   | ├─Token(NewLine) |\n|
  case20: foos[0][idAccessor]
//@[02:0029) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case20|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0029) |   | | └─ArrayAccessSyntax
//@[10:0017) |   | |   ├─ArrayAccessSyntax
//@[10:0014) |   | |   | ├─VariableAccessSyntax
//@[10:0014) |   | |   | | └─IdentifierSyntax
//@[10:0014) |   | |   | |   └─Token(Identifier) |foos|
//@[14:0015) |   | |   | ├─Token(LeftSquare) |[|
//@[15:0016) |   | |   | ├─IntegerLiteralSyntax
//@[15:0016) |   | |   | | └─Token(Integer) |0|
//@[16:0017) |   | |   | └─Token(RightSquare) |]|
//@[17:0018) |   | |   ├─Token(LeftSquare) |[|
//@[18:0028) |   | |   ├─VariableAccessSyntax
//@[18:0028) |   | |   | └─IdentifierSyntax
//@[18:0028) |   | |   |   └─Token(Identifier) |idAccessor|
//@[28:0029) |   | |   └─Token(RightSquare) |]|
//@[29:0030) |   | ├─Token(NewLine) |\n|
  case21: foos[i][idAccessor]
//@[02:0029) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case21|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0029) |   | | └─ArrayAccessSyntax
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
//@[18:0028) |   | |   ├─VariableAccessSyntax
//@[18:0028) |   | |   | └─IdentifierSyntax
//@[18:0028) |   | |   |   └─Token(Identifier) |idAccessor|
//@[28:0029) |   | |   └─Token(RightSquare) |]|
//@[29:0030) |   | ├─Token(NewLine) |\n|
  case22: foos[i + 2][idAccessor]
//@[02:0033) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case22|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0033) |   | | └─ArrayAccessSyntax
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
//@[22:0032) |   | |   ├─VariableAccessSyntax
//@[22:0032) |   | |   | └─IdentifierSyntax
//@[22:0032) |   | |   |   └─Token(Identifier) |idAccessor|
//@[32:0033) |   | |   └─Token(RightSquare) |]|
//@[33:0034) |   | ├─Token(NewLine) |\n|
  case23: foos[zeroIndex][idAccessor]
//@[02:0037) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case23|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0037) |   | | └─ArrayAccessSyntax
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
//@[26:0036) |   | |   ├─VariableAccessSyntax
//@[26:0036) |   | |   | └─IdentifierSyntax
//@[26:0036) |   | |   |   └─Token(Identifier) |idAccessor|
//@[36:0037) |   | |   └─Token(RightSquare) |]|
//@[37:0038) |   | ├─Token(NewLine) |\n|
  case24: foos[otherIndex][idAccessor]
//@[02:0038) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case24|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0038) |   | | └─ArrayAccessSyntax
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
//@[27:0037) |   | |   ├─VariableAccessSyntax
//@[27:0037) |   | |   | └─IdentifierSyntax
//@[27:0037) |   | |   |   └─Token(Identifier) |idAccessor|
//@[37:0038) |   | |   └─Token(RightSquare) |]|
//@[38:0039) |   | ├─Token(NewLine) |\n|
  case25: foo[idAccessor2]
//@[02:0026) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case25|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0026) |   | | └─ArrayAccessSyntax
//@[10:0013) |   | |   ├─VariableAccessSyntax
//@[10:0013) |   | |   | └─IdentifierSyntax
//@[10:0013) |   | |   |   └─Token(Identifier) |foo|
//@[13:0014) |   | |   ├─Token(LeftSquare) |[|
//@[14:0025) |   | |   ├─VariableAccessSyntax
//@[14:0025) |   | |   | └─IdentifierSyntax
//@[14:0025) |   | |   |   └─Token(Identifier) |idAccessor2|
//@[25:0026) |   | |   └─Token(RightSquare) |]|
//@[26:0027) |   | ├─Token(NewLine) |\n|
  case26: existingFoo[idAccessor2]
//@[02:0034) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case26|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0034) |   | | └─ArrayAccessSyntax
//@[10:0021) |   | |   ├─VariableAccessSyntax
//@[10:0021) |   | |   | └─IdentifierSyntax
//@[10:0021) |   | |   |   └─Token(Identifier) |existingFoo|
//@[21:0022) |   | |   ├─Token(LeftSquare) |[|
//@[22:0033) |   | |   ├─VariableAccessSyntax
//@[22:0033) |   | |   | └─IdentifierSyntax
//@[22:0033) |   | |   |   └─Token(Identifier) |idAccessor2|
//@[33:0034) |   | |   └─Token(RightSquare) |]|
//@[34:0035) |   | ├─Token(NewLine) |\n|
  case27: foo::fooChild[idAccessor2]
//@[02:0036) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case27|
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
//@[24:0035) |   | |   ├─VariableAccessSyntax
//@[24:0035) |   | |   | └─IdentifierSyntax
//@[24:0035) |   | |   |   └─Token(Identifier) |idAccessor2|
//@[35:0036) |   | |   └─Token(RightSquare) |]|
//@[36:0037) |   | ├─Token(NewLine) |\n|
  case28: foos[0][idAccessor2]
//@[02:0030) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case28|
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
//@[18:0029) |   | |   ├─VariableAccessSyntax
//@[18:0029) |   | |   | └─IdentifierSyntax
//@[18:0029) |   | |   |   └─Token(Identifier) |idAccessor2|
//@[29:0030) |   | |   └─Token(RightSquare) |]|
//@[30:0031) |   | ├─Token(NewLine) |\n|
  case29: foos[i][idAccessor2]
//@[02:0030) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case29|
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
//@[18:0029) |   | |   ├─VariableAccessSyntax
//@[18:0029) |   | |   | └─IdentifierSyntax
//@[18:0029) |   | |   |   └─Token(Identifier) |idAccessor2|
//@[29:0030) |   | |   └─Token(RightSquare) |]|
//@[30:0031) |   | ├─Token(NewLine) |\n|
  case30: foos[i + 2][idAccessor2]
//@[02:0034) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case30|
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
//@[22:0033) |   | |   ├─VariableAccessSyntax
//@[22:0033) |   | |   | └─IdentifierSyntax
//@[22:0033) |   | |   |   └─Token(Identifier) |idAccessor2|
//@[33:0034) |   | |   └─Token(RightSquare) |]|
//@[34:0035) |   | ├─Token(NewLine) |\n|
  case31: foos[zeroIndex][idAccessor2]
//@[02:0038) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case31|
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
//@[26:0037) |   | |   ├─VariableAccessSyntax
//@[26:0037) |   | |   | └─IdentifierSyntax
//@[26:0037) |   | |   |   └─Token(Identifier) |idAccessor2|
//@[37:0038) |   | |   └─Token(RightSquare) |]|
//@[38:0039) |   | ├─Token(NewLine) |\n|
  case32: foos[otherIndex][idAccessor2]
//@[02:0039) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case32|
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
//@[27:0038) |   | |   ├─VariableAccessSyntax
//@[27:0038) |   | |   | └─IdentifierSyntax
//@[27:0038) |   | |   |   └─Token(Identifier) |idAccessor2|
//@[38:0039) |   | |   └─Token(RightSquare) |]|
//@[39:0040) |   | ├─Token(NewLine) |\n|
  case33: foo['${'id'}']
//@[02:0024) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case33|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0024) |   | | └─ArrayAccessSyntax
//@[10:0013) |   | |   ├─VariableAccessSyntax
//@[10:0013) |   | |   | └─IdentifierSyntax
//@[10:0013) |   | |   |   └─Token(Identifier) |foo|
//@[13:0014) |   | |   ├─Token(LeftSquare) |[|
//@[14:0023) |   | |   ├─StringSyntax
//@[14:0017) |   | |   | ├─Token(StringLeftPiece) |'${|
//@[17:0021) |   | |   | ├─StringSyntax
//@[17:0021) |   | |   | | └─Token(StringComplete) |'id'|
//@[21:0023) |   | |   | └─Token(StringRightPiece) |}'|
//@[23:0024) |   | |   └─Token(RightSquare) |]|
//@[24:0025) |   | ├─Token(NewLine) |\n|
  case34: existingFoo['${'id'}']
//@[02:0032) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case34|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0032) |   | | └─ArrayAccessSyntax
//@[10:0021) |   | |   ├─VariableAccessSyntax
//@[10:0021) |   | |   | └─IdentifierSyntax
//@[10:0021) |   | |   |   └─Token(Identifier) |existingFoo|
//@[21:0022) |   | |   ├─Token(LeftSquare) |[|
//@[22:0031) |   | |   ├─StringSyntax
//@[22:0025) |   | |   | ├─Token(StringLeftPiece) |'${|
//@[25:0029) |   | |   | ├─StringSyntax
//@[25:0029) |   | |   | | └─Token(StringComplete) |'id'|
//@[29:0031) |   | |   | └─Token(StringRightPiece) |}'|
//@[31:0032) |   | |   └─Token(RightSquare) |]|
//@[32:0033) |   | ├─Token(NewLine) |\n|
  case35: foo::fooChild['${'id'}']
//@[02:0034) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case35|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0034) |   | | └─ArrayAccessSyntax
//@[10:0023) |   | |   ├─ResourceAccessSyntax
//@[10:0013) |   | |   | ├─VariableAccessSyntax
//@[10:0013) |   | |   | | └─IdentifierSyntax
//@[10:0013) |   | |   | |   └─Token(Identifier) |foo|
//@[13:0015) |   | |   | ├─Token(DoubleColon) |::|
//@[15:0023) |   | |   | └─IdentifierSyntax
//@[15:0023) |   | |   |   └─Token(Identifier) |fooChild|
//@[23:0024) |   | |   ├─Token(LeftSquare) |[|
//@[24:0033) |   | |   ├─StringSyntax
//@[24:0027) |   | |   | ├─Token(StringLeftPiece) |'${|
//@[27:0031) |   | |   | ├─StringSyntax
//@[27:0031) |   | |   | | └─Token(StringComplete) |'id'|
//@[31:0033) |   | |   | └─Token(StringRightPiece) |}'|
//@[33:0034) |   | |   └─Token(RightSquare) |]|
//@[34:0035) |   | ├─Token(NewLine) |\n|
  case36: foos[0]['${'id'}']
//@[02:0028) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case36|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0028) |   | | └─ArrayAccessSyntax
//@[10:0017) |   | |   ├─ArrayAccessSyntax
//@[10:0014) |   | |   | ├─VariableAccessSyntax
//@[10:0014) |   | |   | | └─IdentifierSyntax
//@[10:0014) |   | |   | |   └─Token(Identifier) |foos|
//@[14:0015) |   | |   | ├─Token(LeftSquare) |[|
//@[15:0016) |   | |   | ├─IntegerLiteralSyntax
//@[15:0016) |   | |   | | └─Token(Integer) |0|
//@[16:0017) |   | |   | └─Token(RightSquare) |]|
//@[17:0018) |   | |   ├─Token(LeftSquare) |[|
//@[18:0027) |   | |   ├─StringSyntax
//@[18:0021) |   | |   | ├─Token(StringLeftPiece) |'${|
//@[21:0025) |   | |   | ├─StringSyntax
//@[21:0025) |   | |   | | └─Token(StringComplete) |'id'|
//@[25:0027) |   | |   | └─Token(StringRightPiece) |}'|
//@[27:0028) |   | |   └─Token(RightSquare) |]|
//@[28:0029) |   | ├─Token(NewLine) |\n|
  case37: foos[i]['${'id'}']
//@[02:0028) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case37|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0028) |   | | └─ArrayAccessSyntax
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
//@[18:0027) |   | |   ├─StringSyntax
//@[18:0021) |   | |   | ├─Token(StringLeftPiece) |'${|
//@[21:0025) |   | |   | ├─StringSyntax
//@[21:0025) |   | |   | | └─Token(StringComplete) |'id'|
//@[25:0027) |   | |   | └─Token(StringRightPiece) |}'|
//@[27:0028) |   | |   └─Token(RightSquare) |]|
//@[28:0029) |   | ├─Token(NewLine) |\n|
  case38: foos[i + 2]['${'id'}']
//@[02:0032) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case38|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0032) |   | | └─ArrayAccessSyntax
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
//@[22:0031) |   | |   ├─StringSyntax
//@[22:0025) |   | |   | ├─Token(StringLeftPiece) |'${|
//@[25:0029) |   | |   | ├─StringSyntax
//@[25:0029) |   | |   | | └─Token(StringComplete) |'id'|
//@[29:0031) |   | |   | └─Token(StringRightPiece) |}'|
//@[31:0032) |   | |   └─Token(RightSquare) |]|
//@[32:0033) |   | ├─Token(NewLine) |\n|
  case39: foos[zeroIndex]['${'id'}']
//@[02:0036) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case39|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0036) |   | | └─ArrayAccessSyntax
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
//@[26:0035) |   | |   ├─StringSyntax
//@[26:0029) |   | |   | ├─Token(StringLeftPiece) |'${|
//@[29:0033) |   | |   | ├─StringSyntax
//@[29:0033) |   | |   | | └─Token(StringComplete) |'id'|
//@[33:0035) |   | |   | └─Token(StringRightPiece) |}'|
//@[35:0036) |   | |   └─Token(RightSquare) |]|
//@[36:0037) |   | ├─Token(NewLine) |\n|
  case40: foos[otherIndex]['${'id'}']
//@[02:0037) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case40|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0037) |   | | └─ArrayAccessSyntax
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
//@[27:0036) |   | |   ├─StringSyntax
//@[27:0030) |   | |   | ├─Token(StringLeftPiece) |'${|
//@[30:0034) |   | |   | ├─StringSyntax
//@[30:0034) |   | |   | | └─Token(StringComplete) |'id'|
//@[34:0036) |   | |   | └─Token(StringRightPiece) |}'|
//@[36:0037) |   | |   └─Token(RightSquare) |]|
//@[37:0038) |   | ├─Token(NewLine) |\n|
  case41: foo[idAccessorInterpolated]
//@[02:0037) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case41|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0037) |   | | └─ArrayAccessSyntax
//@[10:0013) |   | |   ├─VariableAccessSyntax
//@[10:0013) |   | |   | └─IdentifierSyntax
//@[10:0013) |   | |   |   └─Token(Identifier) |foo|
//@[13:0014) |   | |   ├─Token(LeftSquare) |[|
//@[14:0036) |   | |   ├─VariableAccessSyntax
//@[14:0036) |   | |   | └─IdentifierSyntax
//@[14:0036) |   | |   |   └─Token(Identifier) |idAccessorInterpolated|
//@[36:0037) |   | |   └─Token(RightSquare) |]|
//@[37:0038) |   | ├─Token(NewLine) |\n|
  case42: existingFoo[idAccessorInterpolated]
//@[02:0045) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case42|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0045) |   | | └─ArrayAccessSyntax
//@[10:0021) |   | |   ├─VariableAccessSyntax
//@[10:0021) |   | |   | └─IdentifierSyntax
//@[10:0021) |   | |   |   └─Token(Identifier) |existingFoo|
//@[21:0022) |   | |   ├─Token(LeftSquare) |[|
//@[22:0044) |   | |   ├─VariableAccessSyntax
//@[22:0044) |   | |   | └─IdentifierSyntax
//@[22:0044) |   | |   |   └─Token(Identifier) |idAccessorInterpolated|
//@[44:0045) |   | |   └─Token(RightSquare) |]|
//@[45:0046) |   | ├─Token(NewLine) |\n|
  case43: foo::fooChild[idAccessorInterpolated]
//@[02:0047) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case43|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0047) |   | | └─ArrayAccessSyntax
//@[10:0023) |   | |   ├─ResourceAccessSyntax
//@[10:0013) |   | |   | ├─VariableAccessSyntax
//@[10:0013) |   | |   | | └─IdentifierSyntax
//@[10:0013) |   | |   | |   └─Token(Identifier) |foo|
//@[13:0015) |   | |   | ├─Token(DoubleColon) |::|
//@[15:0023) |   | |   | └─IdentifierSyntax
//@[15:0023) |   | |   |   └─Token(Identifier) |fooChild|
//@[23:0024) |   | |   ├─Token(LeftSquare) |[|
//@[24:0046) |   | |   ├─VariableAccessSyntax
//@[24:0046) |   | |   | └─IdentifierSyntax
//@[24:0046) |   | |   |   └─Token(Identifier) |idAccessorInterpolated|
//@[46:0047) |   | |   └─Token(RightSquare) |]|
//@[47:0048) |   | ├─Token(NewLine) |\n|
  case44: foos[0][idAccessorInterpolated]
//@[02:0041) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case44|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0041) |   | | └─ArrayAccessSyntax
//@[10:0017) |   | |   ├─ArrayAccessSyntax
//@[10:0014) |   | |   | ├─VariableAccessSyntax
//@[10:0014) |   | |   | | └─IdentifierSyntax
//@[10:0014) |   | |   | |   └─Token(Identifier) |foos|
//@[14:0015) |   | |   | ├─Token(LeftSquare) |[|
//@[15:0016) |   | |   | ├─IntegerLiteralSyntax
//@[15:0016) |   | |   | | └─Token(Integer) |0|
//@[16:0017) |   | |   | └─Token(RightSquare) |]|
//@[17:0018) |   | |   ├─Token(LeftSquare) |[|
//@[18:0040) |   | |   ├─VariableAccessSyntax
//@[18:0040) |   | |   | └─IdentifierSyntax
//@[18:0040) |   | |   |   └─Token(Identifier) |idAccessorInterpolated|
//@[40:0041) |   | |   └─Token(RightSquare) |]|
//@[41:0042) |   | ├─Token(NewLine) |\n|
  case45: foos[i][idAccessorInterpolated]
//@[02:0041) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case45|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0041) |   | | └─ArrayAccessSyntax
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
//@[18:0040) |   | |   ├─VariableAccessSyntax
//@[18:0040) |   | |   | └─IdentifierSyntax
//@[18:0040) |   | |   |   └─Token(Identifier) |idAccessorInterpolated|
//@[40:0041) |   | |   └─Token(RightSquare) |]|
//@[41:0042) |   | ├─Token(NewLine) |\n|
  case46: foos[i + 2][idAccessorInterpolated]
//@[02:0045) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case46|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0045) |   | | └─ArrayAccessSyntax
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
//@[22:0044) |   | |   ├─VariableAccessSyntax
//@[22:0044) |   | |   | └─IdentifierSyntax
//@[22:0044) |   | |   |   └─Token(Identifier) |idAccessorInterpolated|
//@[44:0045) |   | |   └─Token(RightSquare) |]|
//@[45:0046) |   | ├─Token(NewLine) |\n|
  case47: foos[zeroIndex][idAccessorInterpolated]
//@[02:0049) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case47|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0049) |   | | └─ArrayAccessSyntax
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
//@[26:0048) |   | |   ├─VariableAccessSyntax
//@[26:0048) |   | |   | └─IdentifierSyntax
//@[26:0048) |   | |   |   └─Token(Identifier) |idAccessorInterpolated|
//@[48:0049) |   | |   └─Token(RightSquare) |]|
//@[49:0050) |   | ├─Token(NewLine) |\n|
  case48: foos[otherIndex][idAccessorInterpolated]
//@[02:0050) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case48|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0050) |   | | └─ArrayAccessSyntax
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
//@[27:0049) |   | |   ├─VariableAccessSyntax
//@[27:0049) |   | |   | └─IdentifierSyntax
//@[27:0049) |   | |   |   └─Token(Identifier) |idAccessorInterpolated|
//@[49:0050) |   | |   └─Token(RightSquare) |]|
//@[50:0051) |   | ├─Token(NewLine) |\n|
  case49: foo[idAccessorMixed]
//@[02:0030) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case49|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0030) |   | | └─ArrayAccessSyntax
//@[10:0013) |   | |   ├─VariableAccessSyntax
//@[10:0013) |   | |   | └─IdentifierSyntax
//@[10:0013) |   | |   |   └─Token(Identifier) |foo|
//@[13:0014) |   | |   ├─Token(LeftSquare) |[|
//@[14:0029) |   | |   ├─VariableAccessSyntax
//@[14:0029) |   | |   | └─IdentifierSyntax
//@[14:0029) |   | |   |   └─Token(Identifier) |idAccessorMixed|
//@[29:0030) |   | |   └─Token(RightSquare) |]|
//@[30:0031) |   | ├─Token(NewLine) |\n|
  case50: existingFoo[idAccessorMixed]
//@[02:0038) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case50|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0038) |   | | └─ArrayAccessSyntax
//@[10:0021) |   | |   ├─VariableAccessSyntax
//@[10:0021) |   | |   | └─IdentifierSyntax
//@[10:0021) |   | |   |   └─Token(Identifier) |existingFoo|
//@[21:0022) |   | |   ├─Token(LeftSquare) |[|
//@[22:0037) |   | |   ├─VariableAccessSyntax
//@[22:0037) |   | |   | └─IdentifierSyntax
//@[22:0037) |   | |   |   └─Token(Identifier) |idAccessorMixed|
//@[37:0038) |   | |   └─Token(RightSquare) |]|
//@[38:0039) |   | ├─Token(NewLine) |\n|
  case51: foo::fooChild[idAccessorMixed]
//@[02:0040) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case51|
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
//@[24:0039) |   | |   ├─VariableAccessSyntax
//@[24:0039) |   | |   | └─IdentifierSyntax
//@[24:0039) |   | |   |   └─Token(Identifier) |idAccessorMixed|
//@[39:0040) |   | |   └─Token(RightSquare) |]|
//@[40:0041) |   | ├─Token(NewLine) |\n|
  case52: foos[0][idAccessorMixed]
//@[02:0034) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case52|
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
//@[18:0033) |   | |   ├─VariableAccessSyntax
//@[18:0033) |   | |   | └─IdentifierSyntax
//@[18:0033) |   | |   |   └─Token(Identifier) |idAccessorMixed|
//@[33:0034) |   | |   └─Token(RightSquare) |]|
//@[34:0035) |   | ├─Token(NewLine) |\n|
  case53: foos[i][idAccessorMixed]
//@[02:0034) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case53|
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
//@[18:0033) |   | |   ├─VariableAccessSyntax
//@[18:0033) |   | |   | └─IdentifierSyntax
//@[18:0033) |   | |   |   └─Token(Identifier) |idAccessorMixed|
//@[33:0034) |   | |   └─Token(RightSquare) |]|
//@[34:0035) |   | ├─Token(NewLine) |\n|
  case54: foos[i + 2][idAccessorMixed]
//@[02:0038) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case54|
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
//@[22:0037) |   | |   ├─VariableAccessSyntax
//@[22:0037) |   | |   | └─IdentifierSyntax
//@[22:0037) |   | |   |   └─Token(Identifier) |idAccessorMixed|
//@[37:0038) |   | |   └─Token(RightSquare) |]|
//@[38:0039) |   | ├─Token(NewLine) |\n|
  case55: foos[zeroIndex][idAccessorMixed]
//@[02:0042) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case55|
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
//@[26:0041) |   | |   ├─VariableAccessSyntax
//@[26:0041) |   | |   | └─IdentifierSyntax
//@[26:0041) |   | |   |   └─Token(Identifier) |idAccessorMixed|
//@[41:0042) |   | |   └─Token(RightSquare) |]|
//@[42:0043) |   | ├─Token(NewLine) |\n|
  case56: foos[otherIndex][idAccessorMixed]
//@[02:0043) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case56|
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
//@[27:0042) |   | |   ├─VariableAccessSyntax
//@[27:0042) |   | |   | └─IdentifierSyntax
//@[27:0042) |   | |   |   └─Token(Identifier) |idAccessorMixed|
//@[42:0043) |   | |   └─Token(RightSquare) |]|
//@[43:0044) |   | ├─Token(NewLine) |\n|
  case57: foo[strArray[0]]
//@[02:0026) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case57|
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
//@[23:0024) |   | |   | | └─Token(Integer) |0|
//@[24:0025) |   | |   | └─Token(RightSquare) |]|
//@[25:0026) |   | |   └─Token(RightSquare) |]|
//@[26:0027) |   | ├─Token(NewLine) |\n|
  case58: existingFoo[strArray[0]]
//@[02:0034) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case58|
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
//@[31:0032) |   | |   | | └─Token(Integer) |0|
//@[32:0033) |   | |   | └─Token(RightSquare) |]|
//@[33:0034) |   | |   └─Token(RightSquare) |]|
//@[34:0035) |   | ├─Token(NewLine) |\n|
  case59: foo::fooChild[strArray[0]]
//@[02:0036) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case59|
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
//@[33:0034) |   | |   | | └─Token(Integer) |0|
//@[34:0035) |   | |   | └─Token(RightSquare) |]|
//@[35:0036) |   | |   └─Token(RightSquare) |]|
//@[36:0037) |   | ├─Token(NewLine) |\n|
  case60: foos[0][strArray[0]]
//@[02:0030) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case60|
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
//@[27:0028) |   | |   | | └─Token(Integer) |0|
//@[28:0029) |   | |   | └─Token(RightSquare) |]|
//@[29:0030) |   | |   └─Token(RightSquare) |]|
//@[30:0031) |   | ├─Token(NewLine) |\n|
  case61: foos[i][strArray[0]]
//@[02:0030) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case61|
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
//@[27:0028) |   | |   | | └─Token(Integer) |0|
//@[28:0029) |   | |   | └─Token(RightSquare) |]|
//@[29:0030) |   | |   └─Token(RightSquare) |]|
//@[30:0031) |   | ├─Token(NewLine) |\n|
  case62: foos[i + 2][strArray[0]]
//@[02:0034) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case62|
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
//@[31:0032) |   | |   | | └─Token(Integer) |0|
//@[32:0033) |   | |   | └─Token(RightSquare) |]|
//@[33:0034) |   | |   └─Token(RightSquare) |]|
//@[34:0035) |   | ├─Token(NewLine) |\n|
  case63: foos[zeroIndex][strArray[0]]
//@[02:0038) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case63|
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
//@[35:0036) |   | |   | | └─Token(Integer) |0|
//@[36:0037) |   | |   | └─Token(RightSquare) |]|
//@[37:0038) |   | |   └─Token(RightSquare) |]|
//@[38:0039) |   | ├─Token(NewLine) |\n|
  case64: foos[otherIndex][strArray[0]]
//@[02:0039) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case64|
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
//@[36:0037) |   | |   | | └─Token(Integer) |0|
//@[37:0038) |   | |   | └─Token(RightSquare) |]|
//@[38:0039) |   | |   └─Token(RightSquare) |]|
//@[39:0040) |   | ├─Token(NewLine) |\n|
  case65: foo[first(strArray)]
//@[02:0030) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case65|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0030) |   | | └─ArrayAccessSyntax
//@[10:0013) |   | |   ├─VariableAccessSyntax
//@[10:0013) |   | |   | └─IdentifierSyntax
//@[10:0013) |   | |   |   └─Token(Identifier) |foo|
//@[13:0014) |   | |   ├─Token(LeftSquare) |[|
//@[14:0029) |   | |   ├─FunctionCallSyntax
//@[14:0019) |   | |   | ├─IdentifierSyntax
//@[14:0019) |   | |   | | └─Token(Identifier) |first|
//@[19:0020) |   | |   | ├─Token(LeftParen) |(|
//@[20:0028) |   | |   | ├─FunctionArgumentSyntax
//@[20:0028) |   | |   | | └─VariableAccessSyntax
//@[20:0028) |   | |   | |   └─IdentifierSyntax
//@[20:0028) |   | |   | |     └─Token(Identifier) |strArray|
//@[28:0029) |   | |   | └─Token(RightParen) |)|
//@[29:0030) |   | |   └─Token(RightSquare) |]|
//@[30:0031) |   | ├─Token(NewLine) |\n|
  case66: existingFoo[first(strArray)]
//@[02:0038) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case66|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0038) |   | | └─ArrayAccessSyntax
//@[10:0021) |   | |   ├─VariableAccessSyntax
//@[10:0021) |   | |   | └─IdentifierSyntax
//@[10:0021) |   | |   |   └─Token(Identifier) |existingFoo|
//@[21:0022) |   | |   ├─Token(LeftSquare) |[|
//@[22:0037) |   | |   ├─FunctionCallSyntax
//@[22:0027) |   | |   | ├─IdentifierSyntax
//@[22:0027) |   | |   | | └─Token(Identifier) |first|
//@[27:0028) |   | |   | ├─Token(LeftParen) |(|
//@[28:0036) |   | |   | ├─FunctionArgumentSyntax
//@[28:0036) |   | |   | | └─VariableAccessSyntax
//@[28:0036) |   | |   | |   └─IdentifierSyntax
//@[28:0036) |   | |   | |     └─Token(Identifier) |strArray|
//@[36:0037) |   | |   | └─Token(RightParen) |)|
//@[37:0038) |   | |   └─Token(RightSquare) |]|
//@[38:0039) |   | ├─Token(NewLine) |\n|
  case67: foo::fooChild[first(strArray)]
//@[02:0040) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case67|
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
//@[24:0039) |   | |   ├─FunctionCallSyntax
//@[24:0029) |   | |   | ├─IdentifierSyntax
//@[24:0029) |   | |   | | └─Token(Identifier) |first|
//@[29:0030) |   | |   | ├─Token(LeftParen) |(|
//@[30:0038) |   | |   | ├─FunctionArgumentSyntax
//@[30:0038) |   | |   | | └─VariableAccessSyntax
//@[30:0038) |   | |   | |   └─IdentifierSyntax
//@[30:0038) |   | |   | |     └─Token(Identifier) |strArray|
//@[38:0039) |   | |   | └─Token(RightParen) |)|
//@[39:0040) |   | |   └─Token(RightSquare) |]|
//@[40:0041) |   | ├─Token(NewLine) |\n|
  case68: foos[0][first(strArray)]
//@[02:0034) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case68|
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
//@[18:0033) |   | |   ├─FunctionCallSyntax
//@[18:0023) |   | |   | ├─IdentifierSyntax
//@[18:0023) |   | |   | | └─Token(Identifier) |first|
//@[23:0024) |   | |   | ├─Token(LeftParen) |(|
//@[24:0032) |   | |   | ├─FunctionArgumentSyntax
//@[24:0032) |   | |   | | └─VariableAccessSyntax
//@[24:0032) |   | |   | |   └─IdentifierSyntax
//@[24:0032) |   | |   | |     └─Token(Identifier) |strArray|
//@[32:0033) |   | |   | └─Token(RightParen) |)|
//@[33:0034) |   | |   └─Token(RightSquare) |]|
//@[34:0035) |   | ├─Token(NewLine) |\n|
  case69: foos[i][first(strArray)]
//@[02:0034) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case69|
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
//@[18:0033) |   | |   ├─FunctionCallSyntax
//@[18:0023) |   | |   | ├─IdentifierSyntax
//@[18:0023) |   | |   | | └─Token(Identifier) |first|
//@[23:0024) |   | |   | ├─Token(LeftParen) |(|
//@[24:0032) |   | |   | ├─FunctionArgumentSyntax
//@[24:0032) |   | |   | | └─VariableAccessSyntax
//@[24:0032) |   | |   | |   └─IdentifierSyntax
//@[24:0032) |   | |   | |     └─Token(Identifier) |strArray|
//@[32:0033) |   | |   | └─Token(RightParen) |)|
//@[33:0034) |   | |   └─Token(RightSquare) |]|
//@[34:0035) |   | ├─Token(NewLine) |\n|
  case70: foos[i + 2][first(strArray)]
//@[02:0038) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case70|
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
//@[22:0037) |   | |   ├─FunctionCallSyntax
//@[22:0027) |   | |   | ├─IdentifierSyntax
//@[22:0027) |   | |   | | └─Token(Identifier) |first|
//@[27:0028) |   | |   | ├─Token(LeftParen) |(|
//@[28:0036) |   | |   | ├─FunctionArgumentSyntax
//@[28:0036) |   | |   | | └─VariableAccessSyntax
//@[28:0036) |   | |   | |   └─IdentifierSyntax
//@[28:0036) |   | |   | |     └─Token(Identifier) |strArray|
//@[36:0037) |   | |   | └─Token(RightParen) |)|
//@[37:0038) |   | |   └─Token(RightSquare) |]|
//@[38:0039) |   | ├─Token(NewLine) |\n|
  case71: foos[zeroIndex][first(strArray)]
//@[02:0042) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case71|
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
//@[26:0041) |   | |   ├─FunctionCallSyntax
//@[26:0031) |   | |   | ├─IdentifierSyntax
//@[26:0031) |   | |   | | └─Token(Identifier) |first|
//@[31:0032) |   | |   | ├─Token(LeftParen) |(|
//@[32:0040) |   | |   | ├─FunctionArgumentSyntax
//@[32:0040) |   | |   | | └─VariableAccessSyntax
//@[32:0040) |   | |   | |   └─IdentifierSyntax
//@[32:0040) |   | |   | |     └─Token(Identifier) |strArray|
//@[40:0041) |   | |   | └─Token(RightParen) |)|
//@[41:0042) |   | |   └─Token(RightSquare) |]|
//@[42:0043) |   | ├─Token(NewLine) |\n|
  case72: foos[otherIndex][first(strArray)]
//@[02:0043) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case72|
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
//@[27:0042) |   | |   ├─FunctionCallSyntax
//@[27:0032) |   | |   | ├─IdentifierSyntax
//@[27:0032) |   | |   | | └─Token(Identifier) |first|
//@[32:0033) |   | |   | ├─Token(LeftParen) |(|
//@[33:0041) |   | |   | ├─FunctionArgumentSyntax
//@[33:0041) |   | |   | | └─VariableAccessSyntax
//@[33:0041) |   | |   | |   └─IdentifierSyntax
//@[33:0041) |   | |   | |     └─Token(Identifier) |strArray|
//@[41:0042) |   | |   | └─Token(RightParen) |)|
//@[42:0043) |   | |   └─Token(RightSquare) |]|
//@[43:0044) |   | ├─Token(NewLine) |\n|
  case73: foo[cond ? 'id' : 'name']
//@[02:0035) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case73|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0035) |   | | └─ArrayAccessSyntax
//@[10:0013) |   | |   ├─VariableAccessSyntax
//@[10:0013) |   | |   | └─IdentifierSyntax
//@[10:0013) |   | |   |   └─Token(Identifier) |foo|
//@[13:0014) |   | |   ├─Token(LeftSquare) |[|
//@[14:0034) |   | |   ├─TernaryOperationSyntax
//@[14:0018) |   | |   | ├─VariableAccessSyntax
//@[14:0018) |   | |   | | └─IdentifierSyntax
//@[14:0018) |   | |   | |   └─Token(Identifier) |cond|
//@[19:0020) |   | |   | ├─Token(Question) |?|
//@[21:0025) |   | |   | ├─StringSyntax
//@[21:0025) |   | |   | | └─Token(StringComplete) |'id'|
//@[26:0027) |   | |   | ├─Token(Colon) |:|
//@[28:0034) |   | |   | └─StringSyntax
//@[28:0034) |   | |   |   └─Token(StringComplete) |'name'|
//@[34:0035) |   | |   └─Token(RightSquare) |]|
//@[35:0036) |   | ├─Token(NewLine) |\n|
  case74: existingFoo[cond ? 'id' : 'name']
//@[02:0043) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case74|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0043) |   | | └─ArrayAccessSyntax
//@[10:0021) |   | |   ├─VariableAccessSyntax
//@[10:0021) |   | |   | └─IdentifierSyntax
//@[10:0021) |   | |   |   └─Token(Identifier) |existingFoo|
//@[21:0022) |   | |   ├─Token(LeftSquare) |[|
//@[22:0042) |   | |   ├─TernaryOperationSyntax
//@[22:0026) |   | |   | ├─VariableAccessSyntax
//@[22:0026) |   | |   | | └─IdentifierSyntax
//@[22:0026) |   | |   | |   └─Token(Identifier) |cond|
//@[27:0028) |   | |   | ├─Token(Question) |?|
//@[29:0033) |   | |   | ├─StringSyntax
//@[29:0033) |   | |   | | └─Token(StringComplete) |'id'|
//@[34:0035) |   | |   | ├─Token(Colon) |:|
//@[36:0042) |   | |   | └─StringSyntax
//@[36:0042) |   | |   |   └─Token(StringComplete) |'name'|
//@[42:0043) |   | |   └─Token(RightSquare) |]|
//@[43:0044) |   | ├─Token(NewLine) |\n|
  case75: foo::fooChild[cond ? 'id' : 'name']
//@[02:0045) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case75|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0045) |   | | └─ArrayAccessSyntax
//@[10:0023) |   | |   ├─ResourceAccessSyntax
//@[10:0013) |   | |   | ├─VariableAccessSyntax
//@[10:0013) |   | |   | | └─IdentifierSyntax
//@[10:0013) |   | |   | |   └─Token(Identifier) |foo|
//@[13:0015) |   | |   | ├─Token(DoubleColon) |::|
//@[15:0023) |   | |   | └─IdentifierSyntax
//@[15:0023) |   | |   |   └─Token(Identifier) |fooChild|
//@[23:0024) |   | |   ├─Token(LeftSquare) |[|
//@[24:0044) |   | |   ├─TernaryOperationSyntax
//@[24:0028) |   | |   | ├─VariableAccessSyntax
//@[24:0028) |   | |   | | └─IdentifierSyntax
//@[24:0028) |   | |   | |   └─Token(Identifier) |cond|
//@[29:0030) |   | |   | ├─Token(Question) |?|
//@[31:0035) |   | |   | ├─StringSyntax
//@[31:0035) |   | |   | | └─Token(StringComplete) |'id'|
//@[36:0037) |   | |   | ├─Token(Colon) |:|
//@[38:0044) |   | |   | └─StringSyntax
//@[38:0044) |   | |   |   └─Token(StringComplete) |'name'|
//@[44:0045) |   | |   └─Token(RightSquare) |]|
//@[45:0046) |   | ├─Token(NewLine) |\n|
  case76: foos[0][cond ? 'id' : 'name']
//@[02:0039) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case76|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0039) |   | | └─ArrayAccessSyntax
//@[10:0017) |   | |   ├─ArrayAccessSyntax
//@[10:0014) |   | |   | ├─VariableAccessSyntax
//@[10:0014) |   | |   | | └─IdentifierSyntax
//@[10:0014) |   | |   | |   └─Token(Identifier) |foos|
//@[14:0015) |   | |   | ├─Token(LeftSquare) |[|
//@[15:0016) |   | |   | ├─IntegerLiteralSyntax
//@[15:0016) |   | |   | | └─Token(Integer) |0|
//@[16:0017) |   | |   | └─Token(RightSquare) |]|
//@[17:0018) |   | |   ├─Token(LeftSquare) |[|
//@[18:0038) |   | |   ├─TernaryOperationSyntax
//@[18:0022) |   | |   | ├─VariableAccessSyntax
//@[18:0022) |   | |   | | └─IdentifierSyntax
//@[18:0022) |   | |   | |   └─Token(Identifier) |cond|
//@[23:0024) |   | |   | ├─Token(Question) |?|
//@[25:0029) |   | |   | ├─StringSyntax
//@[25:0029) |   | |   | | └─Token(StringComplete) |'id'|
//@[30:0031) |   | |   | ├─Token(Colon) |:|
//@[32:0038) |   | |   | └─StringSyntax
//@[32:0038) |   | |   |   └─Token(StringComplete) |'name'|
//@[38:0039) |   | |   └─Token(RightSquare) |]|
//@[39:0040) |   | ├─Token(NewLine) |\n|
  case77: foos[i][cond ? 'id' : 'name']
//@[02:0039) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case77|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0039) |   | | └─ArrayAccessSyntax
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
//@[18:0038) |   | |   ├─TernaryOperationSyntax
//@[18:0022) |   | |   | ├─VariableAccessSyntax
//@[18:0022) |   | |   | | └─IdentifierSyntax
//@[18:0022) |   | |   | |   └─Token(Identifier) |cond|
//@[23:0024) |   | |   | ├─Token(Question) |?|
//@[25:0029) |   | |   | ├─StringSyntax
//@[25:0029) |   | |   | | └─Token(StringComplete) |'id'|
//@[30:0031) |   | |   | ├─Token(Colon) |:|
//@[32:0038) |   | |   | └─StringSyntax
//@[32:0038) |   | |   |   └─Token(StringComplete) |'name'|
//@[38:0039) |   | |   └─Token(RightSquare) |]|
//@[39:0040) |   | ├─Token(NewLine) |\n|
  case78: foos[i + 2][cond ? 'id' : 'name']
//@[02:0043) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case78|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0043) |   | | └─ArrayAccessSyntax
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
//@[22:0042) |   | |   ├─TernaryOperationSyntax
//@[22:0026) |   | |   | ├─VariableAccessSyntax
//@[22:0026) |   | |   | | └─IdentifierSyntax
//@[22:0026) |   | |   | |   └─Token(Identifier) |cond|
//@[27:0028) |   | |   | ├─Token(Question) |?|
//@[29:0033) |   | |   | ├─StringSyntax
//@[29:0033) |   | |   | | └─Token(StringComplete) |'id'|
//@[34:0035) |   | |   | ├─Token(Colon) |:|
//@[36:0042) |   | |   | └─StringSyntax
//@[36:0042) |   | |   |   └─Token(StringComplete) |'name'|
//@[42:0043) |   | |   └─Token(RightSquare) |]|
//@[43:0044) |   | ├─Token(NewLine) |\n|
  case79: foos[zeroIndex][cond ? 'id' : 'name']
//@[02:0047) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case79|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0047) |   | | └─ArrayAccessSyntax
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
//@[26:0046) |   | |   ├─TernaryOperationSyntax
//@[26:0030) |   | |   | ├─VariableAccessSyntax
//@[26:0030) |   | |   | | └─IdentifierSyntax
//@[26:0030) |   | |   | |   └─Token(Identifier) |cond|
//@[31:0032) |   | |   | ├─Token(Question) |?|
//@[33:0037) |   | |   | ├─StringSyntax
//@[33:0037) |   | |   | | └─Token(StringComplete) |'id'|
//@[38:0039) |   | |   | ├─Token(Colon) |:|
//@[40:0046) |   | |   | └─StringSyntax
//@[40:0046) |   | |   |   └─Token(StringComplete) |'name'|
//@[46:0047) |   | |   └─Token(RightSquare) |]|
//@[47:0048) |   | ├─Token(NewLine) |\n|
  case80: foos[otherIndex][cond ? 'id' : 'name']
//@[02:0048) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case80|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0048) |   | | └─ArrayAccessSyntax
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
//@[27:0047) |   | |   ├─TernaryOperationSyntax
//@[27:0031) |   | |   | ├─VariableAccessSyntax
//@[27:0031) |   | |   | | └─IdentifierSyntax
//@[27:0031) |   | |   | |   └─Token(Identifier) |cond|
//@[32:0033) |   | |   | ├─Token(Question) |?|
//@[34:0038) |   | |   | ├─StringSyntax
//@[34:0038) |   | |   | | └─Token(StringComplete) |'id'|
//@[39:0040) |   | |   | ├─Token(Colon) |:|
//@[41:0047) |   | |   | └─StringSyntax
//@[41:0047) |   | |   |   └─Token(StringComplete) |'name'|
//@[47:0048) |   | |   └─Token(RightSquare) |]|
//@[48:0049) |   | ├─Token(NewLine) |\n|
  case81: foo[cond ? first(strArray) : strArray[0]]
//@[02:0051) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case81|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0051) |   | | └─ArrayAccessSyntax
//@[10:0013) |   | |   ├─VariableAccessSyntax
//@[10:0013) |   | |   | └─IdentifierSyntax
//@[10:0013) |   | |   |   └─Token(Identifier) |foo|
//@[13:0014) |   | |   ├─Token(LeftSquare) |[|
//@[14:0050) |   | |   ├─TernaryOperationSyntax
//@[14:0018) |   | |   | ├─VariableAccessSyntax
//@[14:0018) |   | |   | | └─IdentifierSyntax
//@[14:0018) |   | |   | |   └─Token(Identifier) |cond|
//@[19:0020) |   | |   | ├─Token(Question) |?|
//@[21:0036) |   | |   | ├─FunctionCallSyntax
//@[21:0026) |   | |   | | ├─IdentifierSyntax
//@[21:0026) |   | |   | | | └─Token(Identifier) |first|
//@[26:0027) |   | |   | | ├─Token(LeftParen) |(|
//@[27:0035) |   | |   | | ├─FunctionArgumentSyntax
//@[27:0035) |   | |   | | | └─VariableAccessSyntax
//@[27:0035) |   | |   | | |   └─IdentifierSyntax
//@[27:0035) |   | |   | | |     └─Token(Identifier) |strArray|
//@[35:0036) |   | |   | | └─Token(RightParen) |)|
//@[37:0038) |   | |   | ├─Token(Colon) |:|
//@[39:0050) |   | |   | └─ArrayAccessSyntax
//@[39:0047) |   | |   |   ├─VariableAccessSyntax
//@[39:0047) |   | |   |   | └─IdentifierSyntax
//@[39:0047) |   | |   |   |   └─Token(Identifier) |strArray|
//@[47:0048) |   | |   |   ├─Token(LeftSquare) |[|
//@[48:0049) |   | |   |   ├─IntegerLiteralSyntax
//@[48:0049) |   | |   |   | └─Token(Integer) |0|
//@[49:0050) |   | |   |   └─Token(RightSquare) |]|
//@[50:0051) |   | |   └─Token(RightSquare) |]|
//@[51:0052) |   | ├─Token(NewLine) |\n|
  case82: existingFoo[cond ? first(strArray) : strArray[0]]
//@[02:0059) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case82|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0059) |   | | └─ArrayAccessSyntax
//@[10:0021) |   | |   ├─VariableAccessSyntax
//@[10:0021) |   | |   | └─IdentifierSyntax
//@[10:0021) |   | |   |   └─Token(Identifier) |existingFoo|
//@[21:0022) |   | |   ├─Token(LeftSquare) |[|
//@[22:0058) |   | |   ├─TernaryOperationSyntax
//@[22:0026) |   | |   | ├─VariableAccessSyntax
//@[22:0026) |   | |   | | └─IdentifierSyntax
//@[22:0026) |   | |   | |   └─Token(Identifier) |cond|
//@[27:0028) |   | |   | ├─Token(Question) |?|
//@[29:0044) |   | |   | ├─FunctionCallSyntax
//@[29:0034) |   | |   | | ├─IdentifierSyntax
//@[29:0034) |   | |   | | | └─Token(Identifier) |first|
//@[34:0035) |   | |   | | ├─Token(LeftParen) |(|
//@[35:0043) |   | |   | | ├─FunctionArgumentSyntax
//@[35:0043) |   | |   | | | └─VariableAccessSyntax
//@[35:0043) |   | |   | | |   └─IdentifierSyntax
//@[35:0043) |   | |   | | |     └─Token(Identifier) |strArray|
//@[43:0044) |   | |   | | └─Token(RightParen) |)|
//@[45:0046) |   | |   | ├─Token(Colon) |:|
//@[47:0058) |   | |   | └─ArrayAccessSyntax
//@[47:0055) |   | |   |   ├─VariableAccessSyntax
//@[47:0055) |   | |   |   | └─IdentifierSyntax
//@[47:0055) |   | |   |   |   └─Token(Identifier) |strArray|
//@[55:0056) |   | |   |   ├─Token(LeftSquare) |[|
//@[56:0057) |   | |   |   ├─IntegerLiteralSyntax
//@[56:0057) |   | |   |   | └─Token(Integer) |0|
//@[57:0058) |   | |   |   └─Token(RightSquare) |]|
//@[58:0059) |   | |   └─Token(RightSquare) |]|
//@[59:0060) |   | ├─Token(NewLine) |\n|
  case83: foo::fooChild[cond ? first(strArray) : strArray[0]]
//@[02:0061) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case83|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0061) |   | | └─ArrayAccessSyntax
//@[10:0023) |   | |   ├─ResourceAccessSyntax
//@[10:0013) |   | |   | ├─VariableAccessSyntax
//@[10:0013) |   | |   | | └─IdentifierSyntax
//@[10:0013) |   | |   | |   └─Token(Identifier) |foo|
//@[13:0015) |   | |   | ├─Token(DoubleColon) |::|
//@[15:0023) |   | |   | └─IdentifierSyntax
//@[15:0023) |   | |   |   └─Token(Identifier) |fooChild|
//@[23:0024) |   | |   ├─Token(LeftSquare) |[|
//@[24:0060) |   | |   ├─TernaryOperationSyntax
//@[24:0028) |   | |   | ├─VariableAccessSyntax
//@[24:0028) |   | |   | | └─IdentifierSyntax
//@[24:0028) |   | |   | |   └─Token(Identifier) |cond|
//@[29:0030) |   | |   | ├─Token(Question) |?|
//@[31:0046) |   | |   | ├─FunctionCallSyntax
//@[31:0036) |   | |   | | ├─IdentifierSyntax
//@[31:0036) |   | |   | | | └─Token(Identifier) |first|
//@[36:0037) |   | |   | | ├─Token(LeftParen) |(|
//@[37:0045) |   | |   | | ├─FunctionArgumentSyntax
//@[37:0045) |   | |   | | | └─VariableAccessSyntax
//@[37:0045) |   | |   | | |   └─IdentifierSyntax
//@[37:0045) |   | |   | | |     └─Token(Identifier) |strArray|
//@[45:0046) |   | |   | | └─Token(RightParen) |)|
//@[47:0048) |   | |   | ├─Token(Colon) |:|
//@[49:0060) |   | |   | └─ArrayAccessSyntax
//@[49:0057) |   | |   |   ├─VariableAccessSyntax
//@[49:0057) |   | |   |   | └─IdentifierSyntax
//@[49:0057) |   | |   |   |   └─Token(Identifier) |strArray|
//@[57:0058) |   | |   |   ├─Token(LeftSquare) |[|
//@[58:0059) |   | |   |   ├─IntegerLiteralSyntax
//@[58:0059) |   | |   |   | └─Token(Integer) |0|
//@[59:0060) |   | |   |   └─Token(RightSquare) |]|
//@[60:0061) |   | |   └─Token(RightSquare) |]|
//@[61:0062) |   | ├─Token(NewLine) |\n|
  case84: foos[0][cond ? first(strArray) : strArray[0]]
//@[02:0055) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case84|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0055) |   | | └─ArrayAccessSyntax
//@[10:0017) |   | |   ├─ArrayAccessSyntax
//@[10:0014) |   | |   | ├─VariableAccessSyntax
//@[10:0014) |   | |   | | └─IdentifierSyntax
//@[10:0014) |   | |   | |   └─Token(Identifier) |foos|
//@[14:0015) |   | |   | ├─Token(LeftSquare) |[|
//@[15:0016) |   | |   | ├─IntegerLiteralSyntax
//@[15:0016) |   | |   | | └─Token(Integer) |0|
//@[16:0017) |   | |   | └─Token(RightSquare) |]|
//@[17:0018) |   | |   ├─Token(LeftSquare) |[|
//@[18:0054) |   | |   ├─TernaryOperationSyntax
//@[18:0022) |   | |   | ├─VariableAccessSyntax
//@[18:0022) |   | |   | | └─IdentifierSyntax
//@[18:0022) |   | |   | |   └─Token(Identifier) |cond|
//@[23:0024) |   | |   | ├─Token(Question) |?|
//@[25:0040) |   | |   | ├─FunctionCallSyntax
//@[25:0030) |   | |   | | ├─IdentifierSyntax
//@[25:0030) |   | |   | | | └─Token(Identifier) |first|
//@[30:0031) |   | |   | | ├─Token(LeftParen) |(|
//@[31:0039) |   | |   | | ├─FunctionArgumentSyntax
//@[31:0039) |   | |   | | | └─VariableAccessSyntax
//@[31:0039) |   | |   | | |   └─IdentifierSyntax
//@[31:0039) |   | |   | | |     └─Token(Identifier) |strArray|
//@[39:0040) |   | |   | | └─Token(RightParen) |)|
//@[41:0042) |   | |   | ├─Token(Colon) |:|
//@[43:0054) |   | |   | └─ArrayAccessSyntax
//@[43:0051) |   | |   |   ├─VariableAccessSyntax
//@[43:0051) |   | |   |   | └─IdentifierSyntax
//@[43:0051) |   | |   |   |   └─Token(Identifier) |strArray|
//@[51:0052) |   | |   |   ├─Token(LeftSquare) |[|
//@[52:0053) |   | |   |   ├─IntegerLiteralSyntax
//@[52:0053) |   | |   |   | └─Token(Integer) |0|
//@[53:0054) |   | |   |   └─Token(RightSquare) |]|
//@[54:0055) |   | |   └─Token(RightSquare) |]|
//@[55:0056) |   | ├─Token(NewLine) |\n|
  case85: foos[i][cond ? first(strArray) : strArray[0]]
//@[02:0055) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case85|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0055) |   | | └─ArrayAccessSyntax
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
//@[18:0054) |   | |   ├─TernaryOperationSyntax
//@[18:0022) |   | |   | ├─VariableAccessSyntax
//@[18:0022) |   | |   | | └─IdentifierSyntax
//@[18:0022) |   | |   | |   └─Token(Identifier) |cond|
//@[23:0024) |   | |   | ├─Token(Question) |?|
//@[25:0040) |   | |   | ├─FunctionCallSyntax
//@[25:0030) |   | |   | | ├─IdentifierSyntax
//@[25:0030) |   | |   | | | └─Token(Identifier) |first|
//@[30:0031) |   | |   | | ├─Token(LeftParen) |(|
//@[31:0039) |   | |   | | ├─FunctionArgumentSyntax
//@[31:0039) |   | |   | | | └─VariableAccessSyntax
//@[31:0039) |   | |   | | |   └─IdentifierSyntax
//@[31:0039) |   | |   | | |     └─Token(Identifier) |strArray|
//@[39:0040) |   | |   | | └─Token(RightParen) |)|
//@[41:0042) |   | |   | ├─Token(Colon) |:|
//@[43:0054) |   | |   | └─ArrayAccessSyntax
//@[43:0051) |   | |   |   ├─VariableAccessSyntax
//@[43:0051) |   | |   |   | └─IdentifierSyntax
//@[43:0051) |   | |   |   |   └─Token(Identifier) |strArray|
//@[51:0052) |   | |   |   ├─Token(LeftSquare) |[|
//@[52:0053) |   | |   |   ├─IntegerLiteralSyntax
//@[52:0053) |   | |   |   | └─Token(Integer) |0|
//@[53:0054) |   | |   |   └─Token(RightSquare) |]|
//@[54:0055) |   | |   └─Token(RightSquare) |]|
//@[55:0056) |   | ├─Token(NewLine) |\n|
  case86: foos[i + 2][cond ? first(strArray) : strArray[0]]
//@[02:0059) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case86|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0059) |   | | └─ArrayAccessSyntax
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
//@[22:0058) |   | |   ├─TernaryOperationSyntax
//@[22:0026) |   | |   | ├─VariableAccessSyntax
//@[22:0026) |   | |   | | └─IdentifierSyntax
//@[22:0026) |   | |   | |   └─Token(Identifier) |cond|
//@[27:0028) |   | |   | ├─Token(Question) |?|
//@[29:0044) |   | |   | ├─FunctionCallSyntax
//@[29:0034) |   | |   | | ├─IdentifierSyntax
//@[29:0034) |   | |   | | | └─Token(Identifier) |first|
//@[34:0035) |   | |   | | ├─Token(LeftParen) |(|
//@[35:0043) |   | |   | | ├─FunctionArgumentSyntax
//@[35:0043) |   | |   | | | └─VariableAccessSyntax
//@[35:0043) |   | |   | | |   └─IdentifierSyntax
//@[35:0043) |   | |   | | |     └─Token(Identifier) |strArray|
//@[43:0044) |   | |   | | └─Token(RightParen) |)|
//@[45:0046) |   | |   | ├─Token(Colon) |:|
//@[47:0058) |   | |   | └─ArrayAccessSyntax
//@[47:0055) |   | |   |   ├─VariableAccessSyntax
//@[47:0055) |   | |   |   | └─IdentifierSyntax
//@[47:0055) |   | |   |   |   └─Token(Identifier) |strArray|
//@[55:0056) |   | |   |   ├─Token(LeftSquare) |[|
//@[56:0057) |   | |   |   ├─IntegerLiteralSyntax
//@[56:0057) |   | |   |   | └─Token(Integer) |0|
//@[57:0058) |   | |   |   └─Token(RightSquare) |]|
//@[58:0059) |   | |   └─Token(RightSquare) |]|
//@[59:0060) |   | ├─Token(NewLine) |\n|
  case87: foos[zeroIndex][cond ? first(strArray) : strArray[0]]
//@[02:0063) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case87|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0063) |   | | └─ArrayAccessSyntax
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
//@[26:0062) |   | |   ├─TernaryOperationSyntax
//@[26:0030) |   | |   | ├─VariableAccessSyntax
//@[26:0030) |   | |   | | └─IdentifierSyntax
//@[26:0030) |   | |   | |   └─Token(Identifier) |cond|
//@[31:0032) |   | |   | ├─Token(Question) |?|
//@[33:0048) |   | |   | ├─FunctionCallSyntax
//@[33:0038) |   | |   | | ├─IdentifierSyntax
//@[33:0038) |   | |   | | | └─Token(Identifier) |first|
//@[38:0039) |   | |   | | ├─Token(LeftParen) |(|
//@[39:0047) |   | |   | | ├─FunctionArgumentSyntax
//@[39:0047) |   | |   | | | └─VariableAccessSyntax
//@[39:0047) |   | |   | | |   └─IdentifierSyntax
//@[39:0047) |   | |   | | |     └─Token(Identifier) |strArray|
//@[47:0048) |   | |   | | └─Token(RightParen) |)|
//@[49:0050) |   | |   | ├─Token(Colon) |:|
//@[51:0062) |   | |   | └─ArrayAccessSyntax
//@[51:0059) |   | |   |   ├─VariableAccessSyntax
//@[51:0059) |   | |   |   | └─IdentifierSyntax
//@[51:0059) |   | |   |   |   └─Token(Identifier) |strArray|
//@[59:0060) |   | |   |   ├─Token(LeftSquare) |[|
//@[60:0061) |   | |   |   ├─IntegerLiteralSyntax
//@[60:0061) |   | |   |   | └─Token(Integer) |0|
//@[61:0062) |   | |   |   └─Token(RightSquare) |]|
//@[62:0063) |   | |   └─Token(RightSquare) |]|
//@[63:0064) |   | ├─Token(NewLine) |\n|
  case88: foos[otherIndex][cond ? first(strArray) : strArray[0]]
//@[02:0064) |   | ├─ObjectPropertySyntax
//@[02:0008) |   | | ├─IdentifierSyntax
//@[02:0008) |   | | | └─Token(Identifier) |case88|
//@[08:0009) |   | | ├─Token(Colon) |:|
//@[10:0064) |   | | └─ArrayAccessSyntax
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
//@[27:0063) |   | |   ├─TernaryOperationSyntax
//@[27:0031) |   | |   | ├─VariableAccessSyntax
//@[27:0031) |   | |   | | └─IdentifierSyntax
//@[27:0031) |   | |   | |   └─Token(Identifier) |cond|
//@[32:0033) |   | |   | ├─Token(Question) |?|
//@[34:0049) |   | |   | ├─FunctionCallSyntax
//@[34:0039) |   | |   | | ├─IdentifierSyntax
//@[34:0039) |   | |   | | | └─Token(Identifier) |first|
//@[39:0040) |   | |   | | ├─Token(LeftParen) |(|
//@[40:0048) |   | |   | | ├─FunctionArgumentSyntax
//@[40:0048) |   | |   | | | └─VariableAccessSyntax
//@[40:0048) |   | |   | | |   └─IdentifierSyntax
//@[40:0048) |   | |   | | |     └─Token(Identifier) |strArray|
//@[48:0049) |   | |   | | └─Token(RightParen) |)|
//@[50:0051) |   | |   | ├─Token(Colon) |:|
//@[52:0063) |   | |   | └─ArrayAccessSyntax
//@[52:0060) |   | |   |   ├─VariableAccessSyntax
//@[52:0060) |   | |   |   | └─IdentifierSyntax
//@[52:0060) |   | |   |   |   └─Token(Identifier) |strArray|
//@[60:0061) |   | |   |   ├─Token(LeftSquare) |[|
//@[61:0062) |   | |   |   ├─IntegerLiteralSyntax
//@[61:0062) |   | |   |   | └─Token(Integer) |0|
//@[62:0063) |   | |   |   └─Token(RightSquare) |]|
//@[63:0064) |   | |   └─Token(RightSquare) |]|
//@[64:0065) |   | ├─Token(NewLine) |\n|
}]
//@[00:0001) |   | └─Token(RightBrace) |}|
//@[01:0002) |   └─Token(RightSquare) |]|
//@[02:0003) ├─Token(NewLine) |\n|

//@[00:0000) └─Token(EndOfFile) ||
