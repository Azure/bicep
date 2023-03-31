param location string = resourceGroup().location
//@[00:1005) ProgramSyntax
//@[00:0048) ├─ParameterDeclarationSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0014) | ├─IdentifierSyntax
//@[06:0014) | | └─Token(Identifier) |location|
//@[15:0021) | ├─VariableAccessSyntax
//@[15:0021) | | └─IdentifierSyntax
//@[15:0021) | |   └─Token(Identifier) |string|
//@[22:0048) | └─ParameterDefaultValueSyntax
//@[22:0023) |   ├─Token(Assignment) |=|
//@[24:0048) |   └─PropertyAccessSyntax
//@[24:0039) |     ├─FunctionCallSyntax
//@[24:0037) |     | ├─IdentifierSyntax
//@[24:0037) |     | | └─Token(Identifier) |resourceGroup|
//@[37:0038) |     | ├─Token(LeftParen) |(|
//@[38:0039) |     | └─Token(RightParen) |)|
//@[39:0040) |     ├─Token(Dot) |.|
//@[40:0048) |     └─IdentifierSyntax
//@[40:0048) |       └─Token(Identifier) |location|
//@[48:0050) ├─Token(NewLine) |\n\n|

resource foo 'Microsoft.Storage/storageAccounts@2022-09-01' = {
//@[00:0158) ├─ResourceDeclarationSyntax
//@[00:0008) | ├─Token(Identifier) |resource|
//@[09:0012) | ├─IdentifierSyntax
//@[09:0012) | | └─Token(Identifier) |foo|
//@[13:0059) | ├─StringSyntax
//@[13:0059) | | └─Token(StringComplete) |'Microsoft.Storage/storageAccounts@2022-09-01'|
//@[60:0061) | ├─Token(Assignment) |=|
//@[62:0158) | └─ObjectSyntax
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
  location: location
//@[02:0020) |   ├─ObjectPropertySyntax
//@[02:0010) |   | ├─IdentifierSyntax
//@[02:0010) |   | | └─Token(Identifier) |location|
//@[10:0011) |   | ├─Token(Colon) |:|
//@[12:0020) |   | └─VariableAccessSyntax
//@[12:0020) |   |   └─IdentifierSyntax
//@[12:0020) |   |     └─Token(Identifier) |location|
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
//@[19:0020) |   ├─Token(NewLine) |\n|
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
  location: location
//@[02:0020) |   | ├─ObjectPropertySyntax
//@[02:0010) |   | | ├─IdentifierSyntax
//@[02:0010) |   | | | └─Token(Identifier) |location|
//@[10:0011) |   | | ├─Token(Colon) |:|
//@[12:0020) |   | | └─VariableAccessSyntax
//@[12:0020) |   | |   └─IdentifierSyntax
//@[12:0020) |   | |     └─Token(Identifier) |location|
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
var idAccessor = 'id'
//@[00:0021) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0014) | ├─IdentifierSyntax
//@[04:0014) | | └─Token(Identifier) |idAccessor|
//@[15:0016) | ├─Token(Assignment) |=|
//@[17:0021) | └─StringSyntax
//@[17:0021) |   └─Token(StringComplete) |'id'|
//@[21:0023) ├─Token(NewLine) |\n\n|

var varForBodyOk1 = [for i in range(0, 2): foo.id]
//@[00:0050) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0017) | ├─IdentifierSyntax
//@[04:0017) | | └─Token(Identifier) |varForBodyOk1|
//@[18:0019) | ├─Token(Assignment) |=|
//@[20:0050) | └─ForSyntax
//@[20:0021) |   ├─Token(LeftSquare) |[|
//@[21:0024) |   ├─Token(Identifier) |for|
//@[25:0026) |   ├─LocalVariableSyntax
//@[25:0026) |   | └─IdentifierSyntax
//@[25:0026) |   |   └─Token(Identifier) |i|
//@[27:0029) |   ├─Token(Identifier) |in|
//@[30:0041) |   ├─FunctionCallSyntax
//@[30:0035) |   | ├─IdentifierSyntax
//@[30:0035) |   | | └─Token(Identifier) |range|
//@[35:0036) |   | ├─Token(LeftParen) |(|
//@[36:0037) |   | ├─FunctionArgumentSyntax
//@[36:0037) |   | | └─IntegerLiteralSyntax
//@[36:0037) |   | |   └─Token(Integer) |0|
//@[37:0038) |   | ├─Token(Comma) |,|
//@[39:0040) |   | ├─FunctionArgumentSyntax
//@[39:0040) |   | | └─IntegerLiteralSyntax
//@[39:0040) |   | |   └─Token(Integer) |2|
//@[40:0041) |   | └─Token(RightParen) |)|
//@[41:0042) |   ├─Token(Colon) |:|
//@[43:0049) |   ├─PropertyAccessSyntax
//@[43:0046) |   | ├─VariableAccessSyntax
//@[43:0046) |   | | └─IdentifierSyntax
//@[43:0046) |   | |   └─Token(Identifier) |foo|
//@[46:0047) |   | ├─Token(Dot) |.|
//@[47:0049) |   | └─IdentifierSyntax
//@[47:0049) |   |   └─Token(Identifier) |id|
//@[49:0050) |   └─Token(RightSquare) |]|
//@[50:0051) ├─Token(NewLine) |\n|
var varForBodyOk2 = [for i in range(0, 2): foos[0].id]
//@[00:0054) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0017) | ├─IdentifierSyntax
//@[04:0017) | | └─Token(Identifier) |varForBodyOk2|
//@[18:0019) | ├─Token(Assignment) |=|
//@[20:0054) | └─ForSyntax
//@[20:0021) |   ├─Token(LeftSquare) |[|
//@[21:0024) |   ├─Token(Identifier) |for|
//@[25:0026) |   ├─LocalVariableSyntax
//@[25:0026) |   | └─IdentifierSyntax
//@[25:0026) |   |   └─Token(Identifier) |i|
//@[27:0029) |   ├─Token(Identifier) |in|
//@[30:0041) |   ├─FunctionCallSyntax
//@[30:0035) |   | ├─IdentifierSyntax
//@[30:0035) |   | | └─Token(Identifier) |range|
//@[35:0036) |   | ├─Token(LeftParen) |(|
//@[36:0037) |   | ├─FunctionArgumentSyntax
//@[36:0037) |   | | └─IntegerLiteralSyntax
//@[36:0037) |   | |   └─Token(Integer) |0|
//@[37:0038) |   | ├─Token(Comma) |,|
//@[39:0040) |   | ├─FunctionArgumentSyntax
//@[39:0040) |   | | └─IntegerLiteralSyntax
//@[39:0040) |   | |   └─Token(Integer) |2|
//@[40:0041) |   | └─Token(RightParen) |)|
//@[41:0042) |   ├─Token(Colon) |:|
//@[43:0053) |   ├─PropertyAccessSyntax
//@[43:0050) |   | ├─ArrayAccessSyntax
//@[43:0047) |   | | ├─VariableAccessSyntax
//@[43:0047) |   | | | └─IdentifierSyntax
//@[43:0047) |   | | |   └─Token(Identifier) |foos|
//@[47:0048) |   | | ├─Token(LeftSquare) |[|
//@[48:0049) |   | | ├─IntegerLiteralSyntax
//@[48:0049) |   | | | └─Token(Integer) |0|
//@[49:0050) |   | | └─Token(RightSquare) |]|
//@[50:0051) |   | ├─Token(Dot) |.|
//@[51:0053) |   | └─IdentifierSyntax
//@[51:0053) |   |   └─Token(Identifier) |id|
//@[53:0054) |   └─Token(RightSquare) |]|
//@[54:0055) ├─Token(NewLine) |\n|
var varForBodyOk3 = [for i in range(0, 2): foos[i].id]
//@[00:0054) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0017) | ├─IdentifierSyntax
//@[04:0017) | | └─Token(Identifier) |varForBodyOk3|
//@[18:0019) | ├─Token(Assignment) |=|
//@[20:0054) | └─ForSyntax
//@[20:0021) |   ├─Token(LeftSquare) |[|
//@[21:0024) |   ├─Token(Identifier) |for|
//@[25:0026) |   ├─LocalVariableSyntax
//@[25:0026) |   | └─IdentifierSyntax
//@[25:0026) |   |   └─Token(Identifier) |i|
//@[27:0029) |   ├─Token(Identifier) |in|
//@[30:0041) |   ├─FunctionCallSyntax
//@[30:0035) |   | ├─IdentifierSyntax
//@[30:0035) |   | | └─Token(Identifier) |range|
//@[35:0036) |   | ├─Token(LeftParen) |(|
//@[36:0037) |   | ├─FunctionArgumentSyntax
//@[36:0037) |   | | └─IntegerLiteralSyntax
//@[36:0037) |   | |   └─Token(Integer) |0|
//@[37:0038) |   | ├─Token(Comma) |,|
//@[39:0040) |   | ├─FunctionArgumentSyntax
//@[39:0040) |   | | └─IntegerLiteralSyntax
//@[39:0040) |   | |   └─Token(Integer) |2|
//@[40:0041) |   | └─Token(RightParen) |)|
//@[41:0042) |   ├─Token(Colon) |:|
//@[43:0053) |   ├─PropertyAccessSyntax
//@[43:0050) |   | ├─ArrayAccessSyntax
//@[43:0047) |   | | ├─VariableAccessSyntax
//@[43:0047) |   | | | └─IdentifierSyntax
//@[43:0047) |   | | |   └─Token(Identifier) |foos|
//@[47:0048) |   | | ├─Token(LeftSquare) |[|
//@[48:0049) |   | | ├─VariableAccessSyntax
//@[48:0049) |   | | | └─IdentifierSyntax
//@[48:0049) |   | | |   └─Token(Identifier) |i|
//@[49:0050) |   | | └─Token(RightSquare) |]|
//@[50:0051) |   | ├─Token(Dot) |.|
//@[51:0053) |   | └─IdentifierSyntax
//@[51:0053) |   |   └─Token(Identifier) |id|
//@[53:0054) |   └─Token(RightSquare) |]|
//@[54:0055) ├─Token(NewLine) |\n|
var varForBodyOk4 = [for i in range(0, 2): foo[idAccessor]]
//@[00:0059) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0017) | ├─IdentifierSyntax
//@[04:0017) | | └─Token(Identifier) |varForBodyOk4|
//@[18:0019) | ├─Token(Assignment) |=|
//@[20:0059) | └─ForSyntax
//@[20:0021) |   ├─Token(LeftSquare) |[|
//@[21:0024) |   ├─Token(Identifier) |for|
//@[25:0026) |   ├─LocalVariableSyntax
//@[25:0026) |   | └─IdentifierSyntax
//@[25:0026) |   |   └─Token(Identifier) |i|
//@[27:0029) |   ├─Token(Identifier) |in|
//@[30:0041) |   ├─FunctionCallSyntax
//@[30:0035) |   | ├─IdentifierSyntax
//@[30:0035) |   | | └─Token(Identifier) |range|
//@[35:0036) |   | ├─Token(LeftParen) |(|
//@[36:0037) |   | ├─FunctionArgumentSyntax
//@[36:0037) |   | | └─IntegerLiteralSyntax
//@[36:0037) |   | |   └─Token(Integer) |0|
//@[37:0038) |   | ├─Token(Comma) |,|
//@[39:0040) |   | ├─FunctionArgumentSyntax
//@[39:0040) |   | | └─IntegerLiteralSyntax
//@[39:0040) |   | |   └─Token(Integer) |2|
//@[40:0041) |   | └─Token(RightParen) |)|
//@[41:0042) |   ├─Token(Colon) |:|
//@[43:0058) |   ├─ArrayAccessSyntax
//@[43:0046) |   | ├─VariableAccessSyntax
//@[43:0046) |   | | └─IdentifierSyntax
//@[43:0046) |   | |   └─Token(Identifier) |foo|
//@[46:0047) |   | ├─Token(LeftSquare) |[|
//@[47:0057) |   | ├─VariableAccessSyntax
//@[47:0057) |   | | └─IdentifierSyntax
//@[47:0057) |   | |   └─Token(Identifier) |idAccessor|
//@[57:0058) |   | └─Token(RightSquare) |]|
//@[58:0059) |   └─Token(RightSquare) |]|
//@[59:0060) ├─Token(NewLine) |\n|
var varForBodyBad1 = [for i in range(0, 2): foo.properties]
//@[00:0059) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0018) | ├─IdentifierSyntax
//@[04:0018) | | └─Token(Identifier) |varForBodyBad1|
//@[19:0020) | ├─Token(Assignment) |=|
//@[21:0059) | └─ForSyntax
//@[21:0022) |   ├─Token(LeftSquare) |[|
//@[22:0025) |   ├─Token(Identifier) |for|
//@[26:0027) |   ├─LocalVariableSyntax
//@[26:0027) |   | └─IdentifierSyntax
//@[26:0027) |   |   └─Token(Identifier) |i|
//@[28:0030) |   ├─Token(Identifier) |in|
//@[31:0042) |   ├─FunctionCallSyntax
//@[31:0036) |   | ├─IdentifierSyntax
//@[31:0036) |   | | └─Token(Identifier) |range|
//@[36:0037) |   | ├─Token(LeftParen) |(|
//@[37:0038) |   | ├─FunctionArgumentSyntax
//@[37:0038) |   | | └─IntegerLiteralSyntax
//@[37:0038) |   | |   └─Token(Integer) |0|
//@[38:0039) |   | ├─Token(Comma) |,|
//@[40:0041) |   | ├─FunctionArgumentSyntax
//@[40:0041) |   | | └─IntegerLiteralSyntax
//@[40:0041) |   | |   └─Token(Integer) |2|
//@[41:0042) |   | └─Token(RightParen) |)|
//@[42:0043) |   ├─Token(Colon) |:|
//@[44:0058) |   ├─PropertyAccessSyntax
//@[44:0047) |   | ├─VariableAccessSyntax
//@[44:0047) |   | | └─IdentifierSyntax
//@[44:0047) |   | |   └─Token(Identifier) |foo|
//@[47:0048) |   | ├─Token(Dot) |.|
//@[48:0058) |   | └─IdentifierSyntax
//@[48:0058) |   |   └─Token(Identifier) |properties|
//@[58:0059) |   └─Token(RightSquare) |]|
//@[59:0060) ├─Token(NewLine) |\n|
var varForBodyBad2 = [for i in range(0, 2): foos[0].properties]
//@[00:0063) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0018) | ├─IdentifierSyntax
//@[04:0018) | | └─Token(Identifier) |varForBodyBad2|
//@[19:0020) | ├─Token(Assignment) |=|
//@[21:0063) | └─ForSyntax
//@[21:0022) |   ├─Token(LeftSquare) |[|
//@[22:0025) |   ├─Token(Identifier) |for|
//@[26:0027) |   ├─LocalVariableSyntax
//@[26:0027) |   | └─IdentifierSyntax
//@[26:0027) |   |   └─Token(Identifier) |i|
//@[28:0030) |   ├─Token(Identifier) |in|
//@[31:0042) |   ├─FunctionCallSyntax
//@[31:0036) |   | ├─IdentifierSyntax
//@[31:0036) |   | | └─Token(Identifier) |range|
//@[36:0037) |   | ├─Token(LeftParen) |(|
//@[37:0038) |   | ├─FunctionArgumentSyntax
//@[37:0038) |   | | └─IntegerLiteralSyntax
//@[37:0038) |   | |   └─Token(Integer) |0|
//@[38:0039) |   | ├─Token(Comma) |,|
//@[40:0041) |   | ├─FunctionArgumentSyntax
//@[40:0041) |   | | └─IntegerLiteralSyntax
//@[40:0041) |   | |   └─Token(Integer) |2|
//@[41:0042) |   | └─Token(RightParen) |)|
//@[42:0043) |   ├─Token(Colon) |:|
//@[44:0062) |   ├─PropertyAccessSyntax
//@[44:0051) |   | ├─ArrayAccessSyntax
//@[44:0048) |   | | ├─VariableAccessSyntax
//@[44:0048) |   | | | └─IdentifierSyntax
//@[44:0048) |   | | |   └─Token(Identifier) |foos|
//@[48:0049) |   | | ├─Token(LeftSquare) |[|
//@[49:0050) |   | | ├─IntegerLiteralSyntax
//@[49:0050) |   | | | └─Token(Integer) |0|
//@[50:0051) |   | | └─Token(RightSquare) |]|
//@[51:0052) |   | ├─Token(Dot) |.|
//@[52:0062) |   | └─IdentifierSyntax
//@[52:0062) |   |   └─Token(Identifier) |properties|
//@[62:0063) |   └─Token(RightSquare) |]|
//@[63:0064) ├─Token(NewLine) |\n|
var varForBodyBad3 = [for i in range(0, 2): {
//@[00:0075) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0018) | ├─IdentifierSyntax
//@[04:0018) | | └─Token(Identifier) |varForBodyBad3|
//@[19:0020) | ├─Token(Assignment) |=|
//@[21:0075) | └─ForSyntax
//@[21:0022) |   ├─Token(LeftSquare) |[|
//@[22:0025) |   ├─Token(Identifier) |for|
//@[26:0027) |   ├─LocalVariableSyntax
//@[26:0027) |   | └─IdentifierSyntax
//@[26:0027) |   |   └─Token(Identifier) |i|
//@[28:0030) |   ├─Token(Identifier) |in|
//@[31:0042) |   ├─FunctionCallSyntax
//@[31:0036) |   | ├─IdentifierSyntax
//@[31:0036) |   | | └─Token(Identifier) |range|
//@[36:0037) |   | ├─Token(LeftParen) |(|
//@[37:0038) |   | ├─FunctionArgumentSyntax
//@[37:0038) |   | | └─IntegerLiteralSyntax
//@[37:0038) |   | |   └─Token(Integer) |0|
//@[38:0039) |   | ├─Token(Comma) |,|
//@[40:0041) |   | ├─FunctionArgumentSyntax
//@[40:0041) |   | | └─IntegerLiteralSyntax
//@[40:0041) |   | |   └─Token(Integer) |2|
//@[41:0042) |   | └─Token(RightParen) |)|
//@[42:0043) |   ├─Token(Colon) |:|
//@[44:0074) |   ├─ObjectSyntax
//@[44:0045) |   | ├─Token(LeftBrace) |{|
//@[45:0046) |   | ├─Token(NewLine) |\n|
  prop: foos[0].properties
//@[02:0026) |   | ├─ObjectPropertySyntax
//@[02:0006) |   | | ├─IdentifierSyntax
//@[02:0006) |   | | | └─Token(Identifier) |prop|
//@[06:0007) |   | | ├─Token(Colon) |:|
//@[08:0026) |   | | └─PropertyAccessSyntax
//@[08:0015) |   | |   ├─ArrayAccessSyntax
//@[08:0012) |   | |   | ├─VariableAccessSyntax
//@[08:0012) |   | |   | | └─IdentifierSyntax
//@[08:0012) |   | |   | |   └─Token(Identifier) |foos|
//@[12:0013) |   | |   | ├─Token(LeftSquare) |[|
//@[13:0014) |   | |   | ├─IntegerLiteralSyntax
//@[13:0014) |   | |   | | └─Token(Integer) |0|
//@[14:0015) |   | |   | └─Token(RightSquare) |]|
//@[15:0016) |   | |   ├─Token(Dot) |.|
//@[16:0026) |   | |   └─IdentifierSyntax
//@[16:0026) |   | |     └─Token(Identifier) |properties|
//@[26:0027) |   | ├─Token(NewLine) |\n|
}]
//@[00:0001) |   | └─Token(RightBrace) |}|
//@[01:0002) |   └─Token(RightSquare) |]|
//@[02:0003) ├─Token(NewLine) |\n|
var varForBodyBad4 = [for i in range(0, 2): foos[i].properties.accessTier]
//@[00:0074) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0018) | ├─IdentifierSyntax
//@[04:0018) | | └─Token(Identifier) |varForBodyBad4|
//@[19:0020) | ├─Token(Assignment) |=|
//@[21:0074) | └─ForSyntax
//@[21:0022) |   ├─Token(LeftSquare) |[|
//@[22:0025) |   ├─Token(Identifier) |for|
//@[26:0027) |   ├─LocalVariableSyntax
//@[26:0027) |   | └─IdentifierSyntax
//@[26:0027) |   |   └─Token(Identifier) |i|
//@[28:0030) |   ├─Token(Identifier) |in|
//@[31:0042) |   ├─FunctionCallSyntax
//@[31:0036) |   | ├─IdentifierSyntax
//@[31:0036) |   | | └─Token(Identifier) |range|
//@[36:0037) |   | ├─Token(LeftParen) |(|
//@[37:0038) |   | ├─FunctionArgumentSyntax
//@[37:0038) |   | | └─IntegerLiteralSyntax
//@[37:0038) |   | |   └─Token(Integer) |0|
//@[38:0039) |   | ├─Token(Comma) |,|
//@[40:0041) |   | ├─FunctionArgumentSyntax
//@[40:0041) |   | | └─IntegerLiteralSyntax
//@[40:0041) |   | |   └─Token(Integer) |2|
//@[41:0042) |   | └─Token(RightParen) |)|
//@[42:0043) |   ├─Token(Colon) |:|
//@[44:0073) |   ├─PropertyAccessSyntax
//@[44:0062) |   | ├─PropertyAccessSyntax
//@[44:0051) |   | | ├─ArrayAccessSyntax
//@[44:0048) |   | | | ├─VariableAccessSyntax
//@[44:0048) |   | | | | └─IdentifierSyntax
//@[44:0048) |   | | | |   └─Token(Identifier) |foos|
//@[48:0049) |   | | | ├─Token(LeftSquare) |[|
//@[49:0050) |   | | | ├─VariableAccessSyntax
//@[49:0050) |   | | | | └─IdentifierSyntax
//@[49:0050) |   | | | |   └─Token(Identifier) |i|
//@[50:0051) |   | | | └─Token(RightSquare) |]|
//@[51:0052) |   | | ├─Token(Dot) |.|
//@[52:0062) |   | | └─IdentifierSyntax
//@[52:0062) |   | |   └─Token(Identifier) |properties|
//@[62:0063) |   | ├─Token(Dot) |.|
//@[63:0073) |   | └─IdentifierSyntax
//@[63:0073) |   |   └─Token(Identifier) |accessTier|
//@[73:0074) |   └─Token(RightSquare) |]|
//@[74:0075) ├─Token(NewLine) |\n|
var varForBodyBad5 = [for i in range(0, 2): foo[strParam]]
//@[00:0058) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0018) | ├─IdentifierSyntax
//@[04:0018) | | └─Token(Identifier) |varForBodyBad5|
//@[19:0020) | ├─Token(Assignment) |=|
//@[21:0058) | └─ForSyntax
//@[21:0022) |   ├─Token(LeftSquare) |[|
//@[22:0025) |   ├─Token(Identifier) |for|
//@[26:0027) |   ├─LocalVariableSyntax
//@[26:0027) |   | └─IdentifierSyntax
//@[26:0027) |   |   └─Token(Identifier) |i|
//@[28:0030) |   ├─Token(Identifier) |in|
//@[31:0042) |   ├─FunctionCallSyntax
//@[31:0036) |   | ├─IdentifierSyntax
//@[31:0036) |   | | └─Token(Identifier) |range|
//@[36:0037) |   | ├─Token(LeftParen) |(|
//@[37:0038) |   | ├─FunctionArgumentSyntax
//@[37:0038) |   | | └─IntegerLiteralSyntax
//@[37:0038) |   | |   └─Token(Integer) |0|
//@[38:0039) |   | ├─Token(Comma) |,|
//@[40:0041) |   | ├─FunctionArgumentSyntax
//@[40:0041) |   | | └─IntegerLiteralSyntax
//@[40:0041) |   | |   └─Token(Integer) |2|
//@[41:0042) |   | └─Token(RightParen) |)|
//@[42:0043) |   ├─Token(Colon) |:|
//@[44:0057) |   ├─ArrayAccessSyntax
//@[44:0047) |   | ├─VariableAccessSyntax
//@[44:0047) |   | | └─IdentifierSyntax
//@[44:0047) |   | |   └─Token(Identifier) |foo|
//@[47:0048) |   | ├─Token(LeftSquare) |[|
//@[48:0056) |   | ├─VariableAccessSyntax
//@[48:0056) |   | | └─IdentifierSyntax
//@[48:0056) |   | |   └─Token(Identifier) |strParam|
//@[56:0057) |   | └─Token(RightSquare) |]|
//@[57:0058) |   └─Token(RightSquare) |]|
//@[58:0059) ├─Token(NewLine) |\n|

//@[00:0000) └─Token(EndOfFile) ||
