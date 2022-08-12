targetScope = 'subscription'
//@[00:705) ProgramSyntax
//@[00:028) ├─TargetScopeSyntax
//@[00:011) | ├─Token(Identifier) |targetScope|
//@[12:013) | ├─Token(Assignment) |=|
//@[14:028) | └─StringSyntax
//@[14:028) |   └─Token(StringComplete) |'subscription'|
//@[28:030) ├─Token(NewLine) |\n\n|

param prefix string = 'majastrz'
//@[00:032) ├─ParameterDeclarationSyntax
//@[00:005) | ├─Token(Identifier) |param|
//@[06:012) | ├─IdentifierSyntax
//@[06:012) | | └─Token(Identifier) |prefix|
//@[13:019) | ├─SimpleTypeSyntax
//@[13:019) | | └─Token(Identifier) |string|
//@[20:032) | └─ParameterDefaultValueSyntax
//@[20:021) |   ├─Token(Assignment) |=|
//@[22:032) |   └─StringSyntax
//@[22:032) |     └─Token(StringComplete) |'majastrz'|
//@[32:033) ├─Token(NewLine) |\n|
var groups = [
//@[00:060) ├─VariableDeclarationSyntax
//@[00:003) | ├─Token(Identifier) |var|
//@[04:010) | ├─IdentifierSyntax
//@[04:010) | | └─Token(Identifier) |groups|
//@[11:012) | ├─Token(Assignment) |=|
//@[13:060) | └─ArraySyntax
//@[13:014) |   ├─Token(LeftSquare) |[|
//@[14:015) |   ├─Token(NewLine) |\n|
  'bicep1'
//@[02:010) |   ├─ArrayItemSyntax
//@[02:010) |   | └─StringSyntax
//@[02:010) |   |   └─Token(StringComplete) |'bicep1'|
//@[10:011) |   ├─Token(NewLine) |\n|
  'bicep2'
//@[02:010) |   ├─ArrayItemSyntax
//@[02:010) |   | └─StringSyntax
//@[02:010) |   |   └─Token(StringComplete) |'bicep2'|
//@[10:011) |   ├─Token(NewLine) |\n|
  'bicep3'
//@[02:010) |   ├─ArrayItemSyntax
//@[02:010) |   | └─StringSyntax
//@[02:010) |   |   └─Token(StringComplete) |'bicep3'|
//@[10:011) |   ├─Token(NewLine) |\n|
  'bicep4'
//@[02:010) |   ├─ArrayItemSyntax
//@[02:010) |   | └─StringSyntax
//@[02:010) |   |   └─Token(StringComplete) |'bicep4'|
//@[10:011) |   ├─Token(NewLine) |\n|
]
//@[00:001) |   └─Token(RightSquare) |]|
//@[01:003) ├─Token(NewLine) |\n\n|

var scripts = take(groups, 2)
//@[00:029) ├─VariableDeclarationSyntax
//@[00:003) | ├─Token(Identifier) |var|
//@[04:011) | ├─IdentifierSyntax
//@[04:011) | | └─Token(Identifier) |scripts|
//@[12:013) | ├─Token(Assignment) |=|
//@[14:029) | └─FunctionCallSyntax
//@[14:018) |   ├─IdentifierSyntax
//@[14:018) |   | └─Token(Identifier) |take|
//@[18:019) |   ├─Token(LeftParen) |(|
//@[19:025) |   ├─FunctionArgumentSyntax
//@[19:025) |   | └─VariableAccessSyntax
//@[19:025) |   |   └─IdentifierSyntax
//@[19:025) |   |     └─Token(Identifier) |groups|
//@[25:026) |   ├─Token(Comma) |,|
//@[27:028) |   ├─FunctionArgumentSyntax
//@[27:028) |   | └─IntegerLiteralSyntax
//@[27:028) |   |   └─Token(Integer) |2|
//@[28:029) |   └─Token(RightParen) |)|
//@[29:031) ├─Token(NewLine) |\n\n|

resource resourceGroups 'Microsoft.Resources/resourceGroups@2020-06-01' = [for name in groups: {
//@[00:148) ├─ResourceDeclarationSyntax
//@[00:008) | ├─Token(Identifier) |resource|
//@[09:023) | ├─IdentifierSyntax
//@[09:023) | | └─Token(Identifier) |resourceGroups|
//@[24:071) | ├─StringSyntax
//@[24:071) | | └─Token(StringComplete) |'Microsoft.Resources/resourceGroups@2020-06-01'|
//@[72:073) | ├─Token(Assignment) |=|
//@[74:148) | └─ForSyntax
//@[74:075) |   ├─Token(LeftSquare) |[|
//@[75:078) |   ├─Token(Identifier) |for|
//@[79:083) |   ├─LocalVariableSyntax
//@[79:083) |   | └─IdentifierSyntax
//@[79:083) |   |   └─Token(Identifier) |name|
//@[84:086) |   ├─Token(Identifier) |in|
//@[87:093) |   ├─VariableAccessSyntax
//@[87:093) |   | └─IdentifierSyntax
//@[87:093) |   |   └─Token(Identifier) |groups|
//@[93:094) |   ├─Token(Colon) |:|
//@[95:147) |   ├─ObjectSyntax
//@[95:096) |   | ├─Token(LeftBrace) |{|
//@[96:097) |   | ├─Token(NewLine) |\n|
  name: '${prefix}-${name}'
//@[02:027) |   | ├─ObjectPropertySyntax
//@[02:006) |   | | ├─IdentifierSyntax
//@[02:006) |   | | | └─Token(Identifier) |name|
//@[06:007) |   | | ├─Token(Colon) |:|
//@[08:027) |   | | └─StringSyntax
//@[08:011) |   | |   ├─Token(StringLeftPiece) |'${|
//@[11:017) |   | |   ├─VariableAccessSyntax
//@[11:017) |   | |   | └─IdentifierSyntax
//@[11:017) |   | |   |   └─Token(Identifier) |prefix|
//@[17:021) |   | |   ├─Token(StringMiddlePiece) |}-${|
//@[21:025) |   | |   ├─VariableAccessSyntax
//@[21:025) |   | |   | └─IdentifierSyntax
//@[21:025) |   | |   |   └─Token(Identifier) |name|
//@[25:027) |   | |   └─Token(StringRightPiece) |}'|
//@[27:028) |   | ├─Token(NewLine) |\n|
  location: 'westus'
//@[02:020) |   | ├─ObjectPropertySyntax
//@[02:010) |   | | ├─IdentifierSyntax
//@[02:010) |   | | | └─Token(Identifier) |location|
//@[10:011) |   | | ├─Token(Colon) |:|
//@[12:020) |   | | └─StringSyntax
//@[12:020) |   | |   └─Token(StringComplete) |'westus'|
//@[20:021) |   | ├─Token(NewLine) |\n|
}]
//@[00:001) |   | └─Token(RightBrace) |}|
//@[01:002) |   └─Token(RightSquare) |]|
//@[02:004) ├─Token(NewLine) |\n\n|

module scopedToSymbolicName 'hello.bicep' = [for (name, i) in scripts: {
//@[00:183) ├─ModuleDeclarationSyntax
//@[00:006) | ├─Token(Identifier) |module|
//@[07:027) | ├─IdentifierSyntax
//@[07:027) | | └─Token(Identifier) |scopedToSymbolicName|
//@[28:041) | ├─StringSyntax
//@[28:041) | | └─Token(StringComplete) |'hello.bicep'|
//@[42:043) | ├─Token(Assignment) |=|
//@[44:183) | └─ForSyntax
//@[44:045) |   ├─Token(LeftSquare) |[|
//@[45:048) |   ├─Token(Identifier) |for|
//@[49:058) |   ├─VariableBlockSyntax
//@[49:050) |   | ├─Token(LeftParen) |(|
//@[50:054) |   | ├─LocalVariableSyntax
//@[50:054) |   | | └─IdentifierSyntax
//@[50:054) |   | |   └─Token(Identifier) |name|
//@[54:055) |   | ├─Token(Comma) |,|
//@[56:057) |   | ├─LocalVariableSyntax
//@[56:057) |   | | └─IdentifierSyntax
//@[56:057) |   | |   └─Token(Identifier) |i|
//@[57:058) |   | └─Token(RightParen) |)|
//@[59:061) |   ├─Token(Identifier) |in|
//@[62:069) |   ├─VariableAccessSyntax
//@[62:069) |   | └─IdentifierSyntax
//@[62:069) |   |   └─Token(Identifier) |scripts|
//@[69:070) |   ├─Token(Colon) |:|
//@[71:182) |   ├─ObjectSyntax
//@[71:072) |   | ├─Token(LeftBrace) |{|
//@[72:073) |   | ├─Token(NewLine) |\n|
  name: '${prefix}-dep-${i}'
//@[02:028) |   | ├─ObjectPropertySyntax
//@[02:006) |   | | ├─IdentifierSyntax
//@[02:006) |   | | | └─Token(Identifier) |name|
//@[06:007) |   | | ├─Token(Colon) |:|
//@[08:028) |   | | └─StringSyntax
//@[08:011) |   | |   ├─Token(StringLeftPiece) |'${|
//@[11:017) |   | |   ├─VariableAccessSyntax
//@[11:017) |   | |   | └─IdentifierSyntax
//@[11:017) |   | |   |   └─Token(Identifier) |prefix|
//@[17:025) |   | |   ├─Token(StringMiddlePiece) |}-dep-${|
//@[25:026) |   | |   ├─VariableAccessSyntax
//@[25:026) |   | |   | └─IdentifierSyntax
//@[25:026) |   | |   |   └─Token(Identifier) |i|
//@[26:028) |   | |   └─Token(StringRightPiece) |}'|
//@[28:029) |   | ├─Token(NewLine) |\n|
  params: {
//@[02:051) |   | ├─ObjectPropertySyntax
//@[02:008) |   | | ├─IdentifierSyntax
//@[02:008) |   | | | └─Token(Identifier) |params|
//@[08:009) |   | | ├─Token(Colon) |:|
//@[10:051) |   | | └─ObjectSyntax
//@[10:011) |   | |   ├─Token(LeftBrace) |{|
//@[11:012) |   | |   ├─Token(NewLine) |\n|
    scriptName: 'test-${name}-${i}'
//@[04:035) |   | |   ├─ObjectPropertySyntax
//@[04:014) |   | |   | ├─IdentifierSyntax
//@[04:014) |   | |   | | └─Token(Identifier) |scriptName|
//@[14:015) |   | |   | ├─Token(Colon) |:|
//@[16:035) |   | |   | └─StringSyntax
//@[16:024) |   | |   |   ├─Token(StringLeftPiece) |'test-${|
//@[24:028) |   | |   |   ├─VariableAccessSyntax
//@[24:028) |   | |   |   | └─IdentifierSyntax
//@[24:028) |   | |   |   |   └─Token(Identifier) |name|
//@[28:032) |   | |   |   ├─Token(StringMiddlePiece) |}-${|
//@[32:033) |   | |   |   ├─VariableAccessSyntax
//@[32:033) |   | |   |   | └─IdentifierSyntax
//@[32:033) |   | |   |   |   └─Token(Identifier) |i|
//@[33:035) |   | |   |   └─Token(StringRightPiece) |}'|
//@[35:036) |   | |   ├─Token(NewLine) |\n|
  }
//@[02:003) |   | |   └─Token(RightBrace) |}|
//@[03:004) |   | ├─Token(NewLine) |\n|
  scope: resourceGroups[i]
//@[02:026) |   | ├─ObjectPropertySyntax
//@[02:007) |   | | ├─IdentifierSyntax
//@[02:007) |   | | | └─Token(Identifier) |scope|
//@[07:008) |   | | ├─Token(Colon) |:|
//@[09:026) |   | | └─ArrayAccessSyntax
//@[09:023) |   | |   ├─VariableAccessSyntax
//@[09:023) |   | |   | └─IdentifierSyntax
//@[09:023) |   | |   |   └─Token(Identifier) |resourceGroups|
//@[23:024) |   | |   ├─Token(LeftSquare) |[|
//@[24:025) |   | |   ├─VariableAccessSyntax
//@[24:025) |   | |   | └─IdentifierSyntax
//@[24:025) |   | |   |   └─Token(Identifier) |i|
//@[25:026) |   | |   └─Token(RightSquare) |]|
//@[26:027) |   | ├─Token(NewLine) |\n|
}]
//@[00:001) |   | └─Token(RightBrace) |}|
//@[01:002) |   └─Token(RightSquare) |]|
//@[02:004) ├─Token(NewLine) |\n\n|

module scopedToResourceGroupFunction 'hello.bicep' = [for (name, i) in scripts: {
//@[00:212) ├─ModuleDeclarationSyntax
//@[00:006) | ├─Token(Identifier) |module|
//@[07:036) | ├─IdentifierSyntax
//@[07:036) | | └─Token(Identifier) |scopedToResourceGroupFunction|
//@[37:050) | ├─StringSyntax
//@[37:050) | | └─Token(StringComplete) |'hello.bicep'|
//@[51:052) | ├─Token(Assignment) |=|
//@[53:212) | └─ForSyntax
//@[53:054) |   ├─Token(LeftSquare) |[|
//@[54:057) |   ├─Token(Identifier) |for|
//@[58:067) |   ├─VariableBlockSyntax
//@[58:059) |   | ├─Token(LeftParen) |(|
//@[59:063) |   | ├─LocalVariableSyntax
//@[59:063) |   | | └─IdentifierSyntax
//@[59:063) |   | |   └─Token(Identifier) |name|
//@[63:064) |   | ├─Token(Comma) |,|
//@[65:066) |   | ├─LocalVariableSyntax
//@[65:066) |   | | └─IdentifierSyntax
//@[65:066) |   | |   └─Token(Identifier) |i|
//@[66:067) |   | └─Token(RightParen) |)|
//@[68:070) |   ├─Token(Identifier) |in|
//@[71:078) |   ├─VariableAccessSyntax
//@[71:078) |   | └─IdentifierSyntax
//@[71:078) |   |   └─Token(Identifier) |scripts|
//@[78:079) |   ├─Token(Colon) |:|
//@[80:211) |   ├─ObjectSyntax
//@[80:081) |   | ├─Token(LeftBrace) |{|
//@[81:082) |   | ├─Token(NewLine) |\n|
  name: '${prefix}-dep-${i}'
//@[02:028) |   | ├─ObjectPropertySyntax
//@[02:006) |   | | ├─IdentifierSyntax
//@[02:006) |   | | | └─Token(Identifier) |name|
//@[06:007) |   | | ├─Token(Colon) |:|
//@[08:028) |   | | └─StringSyntax
//@[08:011) |   | |   ├─Token(StringLeftPiece) |'${|
//@[11:017) |   | |   ├─VariableAccessSyntax
//@[11:017) |   | |   | └─IdentifierSyntax
//@[11:017) |   | |   |   └─Token(Identifier) |prefix|
//@[17:025) |   | |   ├─Token(StringMiddlePiece) |}-dep-${|
//@[25:026) |   | |   ├─VariableAccessSyntax
//@[25:026) |   | |   | └─IdentifierSyntax
//@[25:026) |   | |   |   └─Token(Identifier) |i|
//@[26:028) |   | |   └─Token(StringRightPiece) |}'|
//@[28:029) |   | ├─Token(NewLine) |\n|
  params: {
//@[02:051) |   | ├─ObjectPropertySyntax
//@[02:008) |   | | ├─IdentifierSyntax
//@[02:008) |   | | | └─Token(Identifier) |params|
//@[08:009) |   | | ├─Token(Colon) |:|
//@[10:051) |   | | └─ObjectSyntax
//@[10:011) |   | |   ├─Token(LeftBrace) |{|
//@[11:012) |   | |   ├─Token(NewLine) |\n|
    scriptName: 'test-${name}-${i}'
//@[04:035) |   | |   ├─ObjectPropertySyntax
//@[04:014) |   | |   | ├─IdentifierSyntax
//@[04:014) |   | |   | | └─Token(Identifier) |scriptName|
//@[14:015) |   | |   | ├─Token(Colon) |:|
//@[16:035) |   | |   | └─StringSyntax
//@[16:024) |   | |   |   ├─Token(StringLeftPiece) |'test-${|
//@[24:028) |   | |   |   ├─VariableAccessSyntax
//@[24:028) |   | |   |   | └─IdentifierSyntax
//@[24:028) |   | |   |   |   └─Token(Identifier) |name|
//@[28:032) |   | |   |   ├─Token(StringMiddlePiece) |}-${|
//@[32:033) |   | |   |   ├─VariableAccessSyntax
//@[32:033) |   | |   |   | └─IdentifierSyntax
//@[32:033) |   | |   |   |   └─Token(Identifier) |i|
//@[33:035) |   | |   |   └─Token(StringRightPiece) |}'|
//@[35:036) |   | |   ├─Token(NewLine) |\n|
  }
//@[02:003) |   | |   └─Token(RightBrace) |}|
//@[03:004) |   | ├─Token(NewLine) |\n|
  scope: resourceGroup(concat(name, '-extra'))
//@[02:046) |   | ├─ObjectPropertySyntax
//@[02:007) |   | | ├─IdentifierSyntax
//@[02:007) |   | | | └─Token(Identifier) |scope|
//@[07:008) |   | | ├─Token(Colon) |:|
//@[09:046) |   | | └─FunctionCallSyntax
//@[09:022) |   | |   ├─IdentifierSyntax
//@[09:022) |   | |   | └─Token(Identifier) |resourceGroup|
//@[22:023) |   | |   ├─Token(LeftParen) |(|
//@[23:045) |   | |   ├─FunctionArgumentSyntax
//@[23:045) |   | |   | └─FunctionCallSyntax
//@[23:029) |   | |   |   ├─IdentifierSyntax
//@[23:029) |   | |   |   | └─Token(Identifier) |concat|
//@[29:030) |   | |   |   ├─Token(LeftParen) |(|
//@[30:034) |   | |   |   ├─FunctionArgumentSyntax
//@[30:034) |   | |   |   | └─VariableAccessSyntax
//@[30:034) |   | |   |   |   └─IdentifierSyntax
//@[30:034) |   | |   |   |     └─Token(Identifier) |name|
//@[34:035) |   | |   |   ├─Token(Comma) |,|
//@[36:044) |   | |   |   ├─FunctionArgumentSyntax
//@[36:044) |   | |   |   | └─StringSyntax
//@[36:044) |   | |   |   |   └─Token(StringComplete) |'-extra'|
//@[44:045) |   | |   |   └─Token(RightParen) |)|
//@[45:046) |   | |   └─Token(RightParen) |)|
//@[46:047) |   | ├─Token(NewLine) |\n|
}]
//@[00:001) |   | └─Token(RightBrace) |}|
//@[01:002) |   └─Token(RightSquare) |]|
//@[02:004) ├─Token(NewLine) |\n\n|


//@[00:000) └─Token(EndOfFile) ||
