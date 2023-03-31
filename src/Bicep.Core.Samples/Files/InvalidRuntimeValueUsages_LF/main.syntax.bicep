resource foo 'Microsoft.Storage/storageAccounts@2022-09-01' = {
//@[00:981) ProgramSyntax
//@[00:171) ├─ResourceDeclarationSyntax
//@[00:008) | ├─Token(Identifier) |resource|
//@[09:012) | ├─IdentifierSyntax
//@[09:012) | | └─Token(Identifier) |foo|
//@[13:059) | ├─StringSyntax
//@[13:059) | | └─Token(StringComplete) |'Microsoft.Storage/storageAccounts@2022-09-01'|
//@[60:061) | ├─Token(Assignment) |=|
//@[62:171) | └─ObjectSyntax
//@[62:063) |   ├─Token(LeftBrace) |{|
//@[63:064) |   ├─Token(NewLine) |\n|
  name: 'foo'
//@[02:013) |   ├─ObjectPropertySyntax
//@[02:006) |   | ├─IdentifierSyntax
//@[02:006) |   | | └─Token(Identifier) |name|
//@[06:007) |   | ├─Token(Colon) |:|
//@[08:013) |   | └─StringSyntax
//@[08:013) |   |   └─Token(StringComplete) |'foo'|
//@[13:014) |   ├─Token(NewLine) |\n|
  location: deployment().location
//@[02:033) |   ├─ObjectPropertySyntax
//@[02:010) |   | ├─IdentifierSyntax
//@[02:010) |   | | └─Token(Identifier) |location|
//@[10:011) |   | ├─Token(Colon) |:|
//@[12:033) |   | └─PropertyAccessSyntax
//@[12:024) |   |   ├─FunctionCallSyntax
//@[12:022) |   |   | ├─IdentifierSyntax
//@[12:022) |   |   | | └─Token(Identifier) |deployment|
//@[22:023) |   |   | ├─Token(LeftParen) |(|
//@[23:024) |   |   | └─Token(RightParen) |)|
//@[24:025) |   |   ├─Token(Dot) |.|
//@[25:033) |   |   └─IdentifierSyntax
//@[25:033) |   |     └─Token(Identifier) |location|
//@[33:034) |   ├─Token(NewLine) |\n|
  sku: {
//@[02:037) |   ├─ObjectPropertySyntax
//@[02:005) |   | ├─IdentifierSyntax
//@[02:005) |   | | └─Token(Identifier) |sku|
//@[05:006) |   | ├─Token(Colon) |:|
//@[07:037) |   | └─ObjectSyntax
//@[07:008) |   |   ├─Token(LeftBrace) |{|
//@[08:009) |   |   ├─Token(NewLine) |\n|
    name: 'Standard_LRS'
//@[04:024) |   |   ├─ObjectPropertySyntax
//@[04:008) |   |   | ├─IdentifierSyntax
//@[04:008) |   |   | | └─Token(Identifier) |name|
//@[08:009) |   |   | ├─Token(Colon) |:|
//@[10:024) |   |   | └─StringSyntax
//@[10:024) |   |   |   └─Token(StringComplete) |'Standard_LRS'|
//@[24:025) |   |   ├─Token(NewLine) |\n|
  }
//@[02:003) |   |   └─Token(RightBrace) |}|
//@[03:004) |   ├─Token(NewLine) |\n|
  kind: 'StorageV2'
//@[02:019) |   ├─ObjectPropertySyntax
//@[02:006) |   | ├─IdentifierSyntax
//@[02:006) |   | | └─Token(Identifier) |kind|
//@[06:007) |   | ├─Token(Colon) |:|
//@[08:019) |   | └─StringSyntax
//@[08:019) |   |   └─Token(StringComplete) |'StorageV2'|
//@[19:020) |   ├─Token(NewLine) |\n|
}
//@[00:001) |   └─Token(RightBrace) |}|
//@[01:002) ├─Token(NewLine) |\n|
resource foos 'Microsoft.Storage/storageAccounts@2022-09-01' = [for i in range(0, 2): {
//@[00:201) ├─ResourceDeclarationSyntax
//@[00:008) | ├─Token(Identifier) |resource|
//@[09:013) | ├─IdentifierSyntax
//@[09:013) | | └─Token(Identifier) |foos|
//@[14:060) | ├─StringSyntax
//@[14:060) | | └─Token(StringComplete) |'Microsoft.Storage/storageAccounts@2022-09-01'|
//@[61:062) | ├─Token(Assignment) |=|
//@[63:201) | └─ForSyntax
//@[63:064) |   ├─Token(LeftSquare) |[|
//@[64:067) |   ├─Token(Identifier) |for|
//@[68:069) |   ├─LocalVariableSyntax
//@[68:069) |   | └─IdentifierSyntax
//@[68:069) |   |   └─Token(Identifier) |i|
//@[70:072) |   ├─Token(Identifier) |in|
//@[73:084) |   ├─FunctionCallSyntax
//@[73:078) |   | ├─IdentifierSyntax
//@[73:078) |   | | └─Token(Identifier) |range|
//@[78:079) |   | ├─Token(LeftParen) |(|
//@[79:080) |   | ├─FunctionArgumentSyntax
//@[79:080) |   | | └─IntegerLiteralSyntax
//@[79:080) |   | |   └─Token(Integer) |0|
//@[80:081) |   | ├─Token(Comma) |,|
//@[82:083) |   | ├─FunctionArgumentSyntax
//@[82:083) |   | | └─IntegerLiteralSyntax
//@[82:083) |   | |   └─Token(Integer) |2|
//@[83:084) |   | └─Token(RightParen) |)|
//@[84:085) |   ├─Token(Colon) |:|
//@[86:200) |   ├─ObjectSyntax
//@[86:087) |   | ├─Token(LeftBrace) |{|
//@[87:088) |   | ├─Token(NewLine) |\n|
  name: 'foo-${i}'
//@[02:018) |   | ├─ObjectPropertySyntax
//@[02:006) |   | | ├─IdentifierSyntax
//@[02:006) |   | | | └─Token(Identifier) |name|
//@[06:007) |   | | ├─Token(Colon) |:|
//@[08:018) |   | | └─StringSyntax
//@[08:015) |   | |   ├─Token(StringLeftPiece) |'foo-${|
//@[15:016) |   | |   ├─VariableAccessSyntax
//@[15:016) |   | |   | └─IdentifierSyntax
//@[15:016) |   | |   |   └─Token(Identifier) |i|
//@[16:018) |   | |   └─Token(StringRightPiece) |}'|
//@[18:019) |   | ├─Token(NewLine) |\n|
  location: deployment().location
//@[02:033) |   | ├─ObjectPropertySyntax
//@[02:010) |   | | ├─IdentifierSyntax
//@[02:010) |   | | | └─Token(Identifier) |location|
//@[10:011) |   | | ├─Token(Colon) |:|
//@[12:033) |   | | └─PropertyAccessSyntax
//@[12:024) |   | |   ├─FunctionCallSyntax
//@[12:022) |   | |   | ├─IdentifierSyntax
//@[12:022) |   | |   | | └─Token(Identifier) |deployment|
//@[22:023) |   | |   | ├─Token(LeftParen) |(|
//@[23:024) |   | |   | └─Token(RightParen) |)|
//@[24:025) |   | |   ├─Token(Dot) |.|
//@[25:033) |   | |   └─IdentifierSyntax
//@[25:033) |   | |     └─Token(Identifier) |location|
//@[33:034) |   | ├─Token(NewLine) |\n|
  sku: {
//@[02:037) |   | ├─ObjectPropertySyntax
//@[02:005) |   | | ├─IdentifierSyntax
//@[02:005) |   | | | └─Token(Identifier) |sku|
//@[05:006) |   | | ├─Token(Colon) |:|
//@[07:037) |   | | └─ObjectSyntax
//@[07:008) |   | |   ├─Token(LeftBrace) |{|
//@[08:009) |   | |   ├─Token(NewLine) |\n|
    name: 'Standard_LRS'
//@[04:024) |   | |   ├─ObjectPropertySyntax
//@[04:008) |   | |   | ├─IdentifierSyntax
//@[04:008) |   | |   | | └─Token(Identifier) |name|
//@[08:009) |   | |   | ├─Token(Colon) |:|
//@[10:024) |   | |   | └─StringSyntax
//@[10:024) |   | |   |   └─Token(StringComplete) |'Standard_LRS'|
//@[24:025) |   | |   ├─Token(NewLine) |\n|
  }
//@[02:003) |   | |   └─Token(RightBrace) |}|
//@[03:004) |   | ├─Token(NewLine) |\n|
  kind: 'StorageV2'
//@[02:019) |   | ├─ObjectPropertySyntax
//@[02:006) |   | | ├─IdentifierSyntax
//@[02:006) |   | | | └─Token(Identifier) |kind|
//@[06:007) |   | | ├─Token(Colon) |:|
//@[08:019) |   | | └─StringSyntax
//@[08:019) |   | |   └─Token(StringComplete) |'StorageV2'|
//@[19:020) |   | ├─Token(NewLine) |\n|
}]
//@[00:001) |   | └─Token(RightBrace) |}|
//@[01:002) |   └─Token(RightSquare) |]|
//@[02:003) ├─Token(NewLine) |\n|
param strParam string = 'id'
//@[00:028) ├─ParameterDeclarationSyntax
//@[00:005) | ├─Token(Identifier) |param|
//@[06:014) | ├─IdentifierSyntax
//@[06:014) | | └─Token(Identifier) |strParam|
//@[15:021) | ├─VariableAccessSyntax
//@[15:021) | | └─IdentifierSyntax
//@[15:021) | |   └─Token(Identifier) |string|
//@[22:028) | └─ParameterDefaultValueSyntax
//@[22:023) |   ├─Token(Assignment) |=|
//@[24:028) |   └─StringSyntax
//@[24:028) |     └─Token(StringComplete) |'id'|
//@[28:029) ├─Token(NewLine) |\n|
var idAccessor = 'id'
//@[00:021) ├─VariableDeclarationSyntax
//@[00:003) | ├─Token(Identifier) |var|
//@[04:014) | ├─IdentifierSyntax
//@[04:014) | | └─Token(Identifier) |idAccessor|
//@[15:016) | ├─Token(Assignment) |=|
//@[17:021) | └─StringSyntax
//@[17:021) |   └─Token(StringComplete) |'id'|
//@[21:023) ├─Token(NewLine) |\n\n|

var varForBodyOk1 = [for i in range(0, 2): foo.id]
//@[00:050) ├─VariableDeclarationSyntax
//@[00:003) | ├─Token(Identifier) |var|
//@[04:017) | ├─IdentifierSyntax
//@[04:017) | | └─Token(Identifier) |varForBodyOk1|
//@[18:019) | ├─Token(Assignment) |=|
//@[20:050) | └─ForSyntax
//@[20:021) |   ├─Token(LeftSquare) |[|
//@[21:024) |   ├─Token(Identifier) |for|
//@[25:026) |   ├─LocalVariableSyntax
//@[25:026) |   | └─IdentifierSyntax
//@[25:026) |   |   └─Token(Identifier) |i|
//@[27:029) |   ├─Token(Identifier) |in|
//@[30:041) |   ├─FunctionCallSyntax
//@[30:035) |   | ├─IdentifierSyntax
//@[30:035) |   | | └─Token(Identifier) |range|
//@[35:036) |   | ├─Token(LeftParen) |(|
//@[36:037) |   | ├─FunctionArgumentSyntax
//@[36:037) |   | | └─IntegerLiteralSyntax
//@[36:037) |   | |   └─Token(Integer) |0|
//@[37:038) |   | ├─Token(Comma) |,|
//@[39:040) |   | ├─FunctionArgumentSyntax
//@[39:040) |   | | └─IntegerLiteralSyntax
//@[39:040) |   | |   └─Token(Integer) |2|
//@[40:041) |   | └─Token(RightParen) |)|
//@[41:042) |   ├─Token(Colon) |:|
//@[43:049) |   ├─PropertyAccessSyntax
//@[43:046) |   | ├─VariableAccessSyntax
//@[43:046) |   | | └─IdentifierSyntax
//@[43:046) |   | |   └─Token(Identifier) |foo|
//@[46:047) |   | ├─Token(Dot) |.|
//@[47:049) |   | └─IdentifierSyntax
//@[47:049) |   |   └─Token(Identifier) |id|
//@[49:050) |   └─Token(RightSquare) |]|
//@[50:051) ├─Token(NewLine) |\n|
var varForBodyOk2 = [for i in range(0, 2): foos[0].id]
//@[00:054) ├─VariableDeclarationSyntax
//@[00:003) | ├─Token(Identifier) |var|
//@[04:017) | ├─IdentifierSyntax
//@[04:017) | | └─Token(Identifier) |varForBodyOk2|
//@[18:019) | ├─Token(Assignment) |=|
//@[20:054) | └─ForSyntax
//@[20:021) |   ├─Token(LeftSquare) |[|
//@[21:024) |   ├─Token(Identifier) |for|
//@[25:026) |   ├─LocalVariableSyntax
//@[25:026) |   | └─IdentifierSyntax
//@[25:026) |   |   └─Token(Identifier) |i|
//@[27:029) |   ├─Token(Identifier) |in|
//@[30:041) |   ├─FunctionCallSyntax
//@[30:035) |   | ├─IdentifierSyntax
//@[30:035) |   | | └─Token(Identifier) |range|
//@[35:036) |   | ├─Token(LeftParen) |(|
//@[36:037) |   | ├─FunctionArgumentSyntax
//@[36:037) |   | | └─IntegerLiteralSyntax
//@[36:037) |   | |   └─Token(Integer) |0|
//@[37:038) |   | ├─Token(Comma) |,|
//@[39:040) |   | ├─FunctionArgumentSyntax
//@[39:040) |   | | └─IntegerLiteralSyntax
//@[39:040) |   | |   └─Token(Integer) |2|
//@[40:041) |   | └─Token(RightParen) |)|
//@[41:042) |   ├─Token(Colon) |:|
//@[43:053) |   ├─PropertyAccessSyntax
//@[43:050) |   | ├─ArrayAccessSyntax
//@[43:047) |   | | ├─VariableAccessSyntax
//@[43:047) |   | | | └─IdentifierSyntax
//@[43:047) |   | | |   └─Token(Identifier) |foos|
//@[47:048) |   | | ├─Token(LeftSquare) |[|
//@[48:049) |   | | ├─IntegerLiteralSyntax
//@[48:049) |   | | | └─Token(Integer) |0|
//@[49:050) |   | | └─Token(RightSquare) |]|
//@[50:051) |   | ├─Token(Dot) |.|
//@[51:053) |   | └─IdentifierSyntax
//@[51:053) |   |   └─Token(Identifier) |id|
//@[53:054) |   └─Token(RightSquare) |]|
//@[54:055) ├─Token(NewLine) |\n|
var varForBodyOk3 = [for i in range(0, 2): foos[i].id]
//@[00:054) ├─VariableDeclarationSyntax
//@[00:003) | ├─Token(Identifier) |var|
//@[04:017) | ├─IdentifierSyntax
//@[04:017) | | └─Token(Identifier) |varForBodyOk3|
//@[18:019) | ├─Token(Assignment) |=|
//@[20:054) | └─ForSyntax
//@[20:021) |   ├─Token(LeftSquare) |[|
//@[21:024) |   ├─Token(Identifier) |for|
//@[25:026) |   ├─LocalVariableSyntax
//@[25:026) |   | └─IdentifierSyntax
//@[25:026) |   |   └─Token(Identifier) |i|
//@[27:029) |   ├─Token(Identifier) |in|
//@[30:041) |   ├─FunctionCallSyntax
//@[30:035) |   | ├─IdentifierSyntax
//@[30:035) |   | | └─Token(Identifier) |range|
//@[35:036) |   | ├─Token(LeftParen) |(|
//@[36:037) |   | ├─FunctionArgumentSyntax
//@[36:037) |   | | └─IntegerLiteralSyntax
//@[36:037) |   | |   └─Token(Integer) |0|
//@[37:038) |   | ├─Token(Comma) |,|
//@[39:040) |   | ├─FunctionArgumentSyntax
//@[39:040) |   | | └─IntegerLiteralSyntax
//@[39:040) |   | |   └─Token(Integer) |2|
//@[40:041) |   | └─Token(RightParen) |)|
//@[41:042) |   ├─Token(Colon) |:|
//@[43:053) |   ├─PropertyAccessSyntax
//@[43:050) |   | ├─ArrayAccessSyntax
//@[43:047) |   | | ├─VariableAccessSyntax
//@[43:047) |   | | | └─IdentifierSyntax
//@[43:047) |   | | |   └─Token(Identifier) |foos|
//@[47:048) |   | | ├─Token(LeftSquare) |[|
//@[48:049) |   | | ├─VariableAccessSyntax
//@[48:049) |   | | | └─IdentifierSyntax
//@[48:049) |   | | |   └─Token(Identifier) |i|
//@[49:050) |   | | └─Token(RightSquare) |]|
//@[50:051) |   | ├─Token(Dot) |.|
//@[51:053) |   | └─IdentifierSyntax
//@[51:053) |   |   └─Token(Identifier) |id|
//@[53:054) |   └─Token(RightSquare) |]|
//@[54:055) ├─Token(NewLine) |\n|
var varForBodyOk4 = [for i in range(0, 2): foo[idAccessor]]
//@[00:059) ├─VariableDeclarationSyntax
//@[00:003) | ├─Token(Identifier) |var|
//@[04:017) | ├─IdentifierSyntax
//@[04:017) | | └─Token(Identifier) |varForBodyOk4|
//@[18:019) | ├─Token(Assignment) |=|
//@[20:059) | └─ForSyntax
//@[20:021) |   ├─Token(LeftSquare) |[|
//@[21:024) |   ├─Token(Identifier) |for|
//@[25:026) |   ├─LocalVariableSyntax
//@[25:026) |   | └─IdentifierSyntax
//@[25:026) |   |   └─Token(Identifier) |i|
//@[27:029) |   ├─Token(Identifier) |in|
//@[30:041) |   ├─FunctionCallSyntax
//@[30:035) |   | ├─IdentifierSyntax
//@[30:035) |   | | └─Token(Identifier) |range|
//@[35:036) |   | ├─Token(LeftParen) |(|
//@[36:037) |   | ├─FunctionArgumentSyntax
//@[36:037) |   | | └─IntegerLiteralSyntax
//@[36:037) |   | |   └─Token(Integer) |0|
//@[37:038) |   | ├─Token(Comma) |,|
//@[39:040) |   | ├─FunctionArgumentSyntax
//@[39:040) |   | | └─IntegerLiteralSyntax
//@[39:040) |   | |   └─Token(Integer) |2|
//@[40:041) |   | └─Token(RightParen) |)|
//@[41:042) |   ├─Token(Colon) |:|
//@[43:058) |   ├─ArrayAccessSyntax
//@[43:046) |   | ├─VariableAccessSyntax
//@[43:046) |   | | └─IdentifierSyntax
//@[43:046) |   | |   └─Token(Identifier) |foo|
//@[46:047) |   | ├─Token(LeftSquare) |[|
//@[47:057) |   | ├─VariableAccessSyntax
//@[47:057) |   | | └─IdentifierSyntax
//@[47:057) |   | |   └─Token(Identifier) |idAccessor|
//@[57:058) |   | └─Token(RightSquare) |]|
//@[58:059) |   └─Token(RightSquare) |]|
//@[59:060) ├─Token(NewLine) |\n|
var varForBodyBad1 = [for i in range(0, 2): foo.properties]
//@[00:059) ├─VariableDeclarationSyntax
//@[00:003) | ├─Token(Identifier) |var|
//@[04:018) | ├─IdentifierSyntax
//@[04:018) | | └─Token(Identifier) |varForBodyBad1|
//@[19:020) | ├─Token(Assignment) |=|
//@[21:059) | └─ForSyntax
//@[21:022) |   ├─Token(LeftSquare) |[|
//@[22:025) |   ├─Token(Identifier) |for|
//@[26:027) |   ├─LocalVariableSyntax
//@[26:027) |   | └─IdentifierSyntax
//@[26:027) |   |   └─Token(Identifier) |i|
//@[28:030) |   ├─Token(Identifier) |in|
//@[31:042) |   ├─FunctionCallSyntax
//@[31:036) |   | ├─IdentifierSyntax
//@[31:036) |   | | └─Token(Identifier) |range|
//@[36:037) |   | ├─Token(LeftParen) |(|
//@[37:038) |   | ├─FunctionArgumentSyntax
//@[37:038) |   | | └─IntegerLiteralSyntax
//@[37:038) |   | |   └─Token(Integer) |0|
//@[38:039) |   | ├─Token(Comma) |,|
//@[40:041) |   | ├─FunctionArgumentSyntax
//@[40:041) |   | | └─IntegerLiteralSyntax
//@[40:041) |   | |   └─Token(Integer) |2|
//@[41:042) |   | └─Token(RightParen) |)|
//@[42:043) |   ├─Token(Colon) |:|
//@[44:058) |   ├─PropertyAccessSyntax
//@[44:047) |   | ├─VariableAccessSyntax
//@[44:047) |   | | └─IdentifierSyntax
//@[44:047) |   | |   └─Token(Identifier) |foo|
//@[47:048) |   | ├─Token(Dot) |.|
//@[48:058) |   | └─IdentifierSyntax
//@[48:058) |   |   └─Token(Identifier) |properties|
//@[58:059) |   └─Token(RightSquare) |]|
//@[59:060) ├─Token(NewLine) |\n|
var varForBodyBad2 = [for i in range(0, 2): foos[0].properties]
//@[00:063) ├─VariableDeclarationSyntax
//@[00:003) | ├─Token(Identifier) |var|
//@[04:018) | ├─IdentifierSyntax
//@[04:018) | | └─Token(Identifier) |varForBodyBad2|
//@[19:020) | ├─Token(Assignment) |=|
//@[21:063) | └─ForSyntax
//@[21:022) |   ├─Token(LeftSquare) |[|
//@[22:025) |   ├─Token(Identifier) |for|
//@[26:027) |   ├─LocalVariableSyntax
//@[26:027) |   | └─IdentifierSyntax
//@[26:027) |   |   └─Token(Identifier) |i|
//@[28:030) |   ├─Token(Identifier) |in|
//@[31:042) |   ├─FunctionCallSyntax
//@[31:036) |   | ├─IdentifierSyntax
//@[31:036) |   | | └─Token(Identifier) |range|
//@[36:037) |   | ├─Token(LeftParen) |(|
//@[37:038) |   | ├─FunctionArgumentSyntax
//@[37:038) |   | | └─IntegerLiteralSyntax
//@[37:038) |   | |   └─Token(Integer) |0|
//@[38:039) |   | ├─Token(Comma) |,|
//@[40:041) |   | ├─FunctionArgumentSyntax
//@[40:041) |   | | └─IntegerLiteralSyntax
//@[40:041) |   | |   └─Token(Integer) |2|
//@[41:042) |   | └─Token(RightParen) |)|
//@[42:043) |   ├─Token(Colon) |:|
//@[44:062) |   ├─PropertyAccessSyntax
//@[44:051) |   | ├─ArrayAccessSyntax
//@[44:048) |   | | ├─VariableAccessSyntax
//@[44:048) |   | | | └─IdentifierSyntax
//@[44:048) |   | | |   └─Token(Identifier) |foos|
//@[48:049) |   | | ├─Token(LeftSquare) |[|
//@[49:050) |   | | ├─IntegerLiteralSyntax
//@[49:050) |   | | | └─Token(Integer) |0|
//@[50:051) |   | | └─Token(RightSquare) |]|
//@[51:052) |   | ├─Token(Dot) |.|
//@[52:062) |   | └─IdentifierSyntax
//@[52:062) |   |   └─Token(Identifier) |properties|
//@[62:063) |   └─Token(RightSquare) |]|
//@[63:064) ├─Token(NewLine) |\n|
var varForBodyBad3 = [for i in range(0, 2): {
//@[00:075) ├─VariableDeclarationSyntax
//@[00:003) | ├─Token(Identifier) |var|
//@[04:018) | ├─IdentifierSyntax
//@[04:018) | | └─Token(Identifier) |varForBodyBad3|
//@[19:020) | ├─Token(Assignment) |=|
//@[21:075) | └─ForSyntax
//@[21:022) |   ├─Token(LeftSquare) |[|
//@[22:025) |   ├─Token(Identifier) |for|
//@[26:027) |   ├─LocalVariableSyntax
//@[26:027) |   | └─IdentifierSyntax
//@[26:027) |   |   └─Token(Identifier) |i|
//@[28:030) |   ├─Token(Identifier) |in|
//@[31:042) |   ├─FunctionCallSyntax
//@[31:036) |   | ├─IdentifierSyntax
//@[31:036) |   | | └─Token(Identifier) |range|
//@[36:037) |   | ├─Token(LeftParen) |(|
//@[37:038) |   | ├─FunctionArgumentSyntax
//@[37:038) |   | | └─IntegerLiteralSyntax
//@[37:038) |   | |   └─Token(Integer) |0|
//@[38:039) |   | ├─Token(Comma) |,|
//@[40:041) |   | ├─FunctionArgumentSyntax
//@[40:041) |   | | └─IntegerLiteralSyntax
//@[40:041) |   | |   └─Token(Integer) |2|
//@[41:042) |   | └─Token(RightParen) |)|
//@[42:043) |   ├─Token(Colon) |:|
//@[44:074) |   ├─ObjectSyntax
//@[44:045) |   | ├─Token(LeftBrace) |{|
//@[45:046) |   | ├─Token(NewLine) |\n|
  prop: foos[0].properties
//@[02:026) |   | ├─ObjectPropertySyntax
//@[02:006) |   | | ├─IdentifierSyntax
//@[02:006) |   | | | └─Token(Identifier) |prop|
//@[06:007) |   | | ├─Token(Colon) |:|
//@[08:026) |   | | └─PropertyAccessSyntax
//@[08:015) |   | |   ├─ArrayAccessSyntax
//@[08:012) |   | |   | ├─VariableAccessSyntax
//@[08:012) |   | |   | | └─IdentifierSyntax
//@[08:012) |   | |   | |   └─Token(Identifier) |foos|
//@[12:013) |   | |   | ├─Token(LeftSquare) |[|
//@[13:014) |   | |   | ├─IntegerLiteralSyntax
//@[13:014) |   | |   | | └─Token(Integer) |0|
//@[14:015) |   | |   | └─Token(RightSquare) |]|
//@[15:016) |   | |   ├─Token(Dot) |.|
//@[16:026) |   | |   └─IdentifierSyntax
//@[16:026) |   | |     └─Token(Identifier) |properties|
//@[26:027) |   | ├─Token(NewLine) |\n|
}]
//@[00:001) |   | └─Token(RightBrace) |}|
//@[01:002) |   └─Token(RightSquare) |]|
//@[02:003) ├─Token(NewLine) |\n|
var varForBodyBad4 = [for i in range(0, 2): foos[i].properties.accessTier]
//@[00:074) ├─VariableDeclarationSyntax
//@[00:003) | ├─Token(Identifier) |var|
//@[04:018) | ├─IdentifierSyntax
//@[04:018) | | └─Token(Identifier) |varForBodyBad4|
//@[19:020) | ├─Token(Assignment) |=|
//@[21:074) | └─ForSyntax
//@[21:022) |   ├─Token(LeftSquare) |[|
//@[22:025) |   ├─Token(Identifier) |for|
//@[26:027) |   ├─LocalVariableSyntax
//@[26:027) |   | └─IdentifierSyntax
//@[26:027) |   |   └─Token(Identifier) |i|
//@[28:030) |   ├─Token(Identifier) |in|
//@[31:042) |   ├─FunctionCallSyntax
//@[31:036) |   | ├─IdentifierSyntax
//@[31:036) |   | | └─Token(Identifier) |range|
//@[36:037) |   | ├─Token(LeftParen) |(|
//@[37:038) |   | ├─FunctionArgumentSyntax
//@[37:038) |   | | └─IntegerLiteralSyntax
//@[37:038) |   | |   └─Token(Integer) |0|
//@[38:039) |   | ├─Token(Comma) |,|
//@[40:041) |   | ├─FunctionArgumentSyntax
//@[40:041) |   | | └─IntegerLiteralSyntax
//@[40:041) |   | |   └─Token(Integer) |2|
//@[41:042) |   | └─Token(RightParen) |)|
//@[42:043) |   ├─Token(Colon) |:|
//@[44:073) |   ├─PropertyAccessSyntax
//@[44:062) |   | ├─PropertyAccessSyntax
//@[44:051) |   | | ├─ArrayAccessSyntax
//@[44:048) |   | | | ├─VariableAccessSyntax
//@[44:048) |   | | | | └─IdentifierSyntax
//@[44:048) |   | | | |   └─Token(Identifier) |foos|
//@[48:049) |   | | | ├─Token(LeftSquare) |[|
//@[49:050) |   | | | ├─VariableAccessSyntax
//@[49:050) |   | | | | └─IdentifierSyntax
//@[49:050) |   | | | |   └─Token(Identifier) |i|
//@[50:051) |   | | | └─Token(RightSquare) |]|
//@[51:052) |   | | ├─Token(Dot) |.|
//@[52:062) |   | | └─IdentifierSyntax
//@[52:062) |   | |   └─Token(Identifier) |properties|
//@[62:063) |   | ├─Token(Dot) |.|
//@[63:073) |   | └─IdentifierSyntax
//@[63:073) |   |   └─Token(Identifier) |accessTier|
//@[73:074) |   └─Token(RightSquare) |]|
//@[74:075) ├─Token(NewLine) |\n|
var varForBodyBad5 = [for i in range(0, 2): foo[strParam]]
//@[00:058) ├─VariableDeclarationSyntax
//@[00:003) | ├─Token(Identifier) |var|
//@[04:018) | ├─IdentifierSyntax
//@[04:018) | | └─Token(Identifier) |varForBodyBad5|
//@[19:020) | ├─Token(Assignment) |=|
//@[21:058) | └─ForSyntax
//@[21:022) |   ├─Token(LeftSquare) |[|
//@[22:025) |   ├─Token(Identifier) |for|
//@[26:027) |   ├─LocalVariableSyntax
//@[26:027) |   | └─IdentifierSyntax
//@[26:027) |   |   └─Token(Identifier) |i|
//@[28:030) |   ├─Token(Identifier) |in|
//@[31:042) |   ├─FunctionCallSyntax
//@[31:036) |   | ├─IdentifierSyntax
//@[31:036) |   | | └─Token(Identifier) |range|
//@[36:037) |   | ├─Token(LeftParen) |(|
//@[37:038) |   | ├─FunctionArgumentSyntax
//@[37:038) |   | | └─IntegerLiteralSyntax
//@[37:038) |   | |   └─Token(Integer) |0|
//@[38:039) |   | ├─Token(Comma) |,|
//@[40:041) |   | ├─FunctionArgumentSyntax
//@[40:041) |   | | └─IntegerLiteralSyntax
//@[40:041) |   | |   └─Token(Integer) |2|
//@[41:042) |   | └─Token(RightParen) |)|
//@[42:043) |   ├─Token(Colon) |:|
//@[44:057) |   ├─ArrayAccessSyntax
//@[44:047) |   | ├─VariableAccessSyntax
//@[44:047) |   | | └─IdentifierSyntax
//@[44:047) |   | |   └─Token(Identifier) |foo|
//@[47:048) |   | ├─Token(LeftSquare) |[|
//@[48:056) |   | ├─VariableAccessSyntax
//@[48:056) |   | | └─IdentifierSyntax
//@[48:056) |   | |   └─Token(Identifier) |strParam|
//@[56:057) |   | └─Token(RightSquare) |]|
//@[57:058) |   └─Token(RightSquare) |]|
//@[58:059) ├─Token(NewLine) |\n|

//@[00:000) └─Token(EndOfFile) ||
