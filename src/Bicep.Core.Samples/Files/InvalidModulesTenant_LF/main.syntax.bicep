targetScope = 'tenant'
//@[00:1613) ProgramSyntax
//@[00:0022) ├─TargetScopeSyntax
//@[00:0011) | ├─Token(Identifier) |targetScope|
//@[12:0013) | ├─Token(Assignment) |=|
//@[14:0022) | └─StringSyntax
//@[14:0022) |   └─Token(StringComplete) |'tenant'|
//@[22:0024) ├─Token(NewLine) |\n\n|

module tenantModuleDuplicateName1 'modules/tenant.bicep' = {
//@[00:0116) ├─ModuleDeclarationSyntax
//@[00:0006) | ├─Token(Identifier) |module|
//@[07:0033) | ├─IdentifierSyntax
//@[07:0033) | | └─Token(Identifier) |tenantModuleDuplicateName1|
//@[34:0056) | ├─StringSyntax
//@[34:0056) | | └─Token(StringComplete) |'modules/tenant.bicep'|
//@[57:0058) | ├─Token(Assignment) |=|
//@[59:0116) | └─ObjectSyntax
//@[59:0060) |   ├─Token(LeftBrace) |{|
//@[60:0061) |   ├─Token(NewLine) |\n|
  name: 'tenantModuleDuplicateName'
//@[02:0035) |   ├─ObjectPropertySyntax
//@[02:0006) |   | ├─IdentifierSyntax
//@[02:0006) |   | | └─Token(Identifier) |name|
//@[06:0007) |   | ├─Token(Colon) |:|
//@[08:0035) |   | └─StringSyntax
//@[08:0035) |   |   └─Token(StringComplete) |'tenantModuleDuplicateName'|
//@[35:0036) |   ├─Token(NewLine) |\n|
  scope: tenant()
//@[02:0017) |   ├─ObjectPropertySyntax
//@[02:0007) |   | ├─IdentifierSyntax
//@[02:0007) |   | | └─Token(Identifier) |scope|
//@[07:0008) |   | ├─Token(Colon) |:|
//@[09:0017) |   | └─FunctionCallSyntax
//@[09:0015) |   |   ├─IdentifierSyntax
//@[09:0015) |   |   | └─Token(Identifier) |tenant|
//@[15:0016) |   |   ├─Token(LeftParen) |(|
//@[16:0017) |   |   └─Token(RightParen) |)|
//@[17:0018) |   ├─Token(NewLine) |\n|
}
//@[00:0001) |   └─Token(RightBrace) |}|
//@[01:0003) ├─Token(NewLine) |\n\n|

module tenantModuleDuplicateName2 'modules/tenant.bicep' = {
//@[00:0116) ├─ModuleDeclarationSyntax
//@[00:0006) | ├─Token(Identifier) |module|
//@[07:0033) | ├─IdentifierSyntax
//@[07:0033) | | └─Token(Identifier) |tenantModuleDuplicateName2|
//@[34:0056) | ├─StringSyntax
//@[34:0056) | | └─Token(StringComplete) |'modules/tenant.bicep'|
//@[57:0058) | ├─Token(Assignment) |=|
//@[59:0116) | └─ObjectSyntax
//@[59:0060) |   ├─Token(LeftBrace) |{|
//@[60:0061) |   ├─Token(NewLine) |\n|
  name: 'tenantModuleDuplicateName'
//@[02:0035) |   ├─ObjectPropertySyntax
//@[02:0006) |   | ├─IdentifierSyntax
//@[02:0006) |   | | └─Token(Identifier) |name|
//@[06:0007) |   | ├─Token(Colon) |:|
//@[08:0035) |   | └─StringSyntax
//@[08:0035) |   |   └─Token(StringComplete) |'tenantModuleDuplicateName'|
//@[35:0036) |   ├─Token(NewLine) |\n|
  scope: tenant()
//@[02:0017) |   ├─ObjectPropertySyntax
//@[02:0007) |   | ├─IdentifierSyntax
//@[02:0007) |   | | └─Token(Identifier) |scope|
//@[07:0008) |   | ├─Token(Colon) |:|
//@[09:0017) |   | └─FunctionCallSyntax
//@[09:0015) |   |   ├─IdentifierSyntax
//@[09:0015) |   |   | └─Token(Identifier) |tenant|
//@[15:0016) |   |   ├─Token(LeftParen) |(|
//@[16:0017) |   |   └─Token(RightParen) |)|
//@[17:0018) |   ├─Token(NewLine) |\n|
}
//@[00:0001) |   └─Token(RightBrace) |}|
//@[01:0003) ├─Token(NewLine) |\n\n|

module tenantModuleDuplicateName3 'modules/tenant.bicep' = {
//@[00:0098) ├─ModuleDeclarationSyntax
//@[00:0006) | ├─Token(Identifier) |module|
//@[07:0033) | ├─IdentifierSyntax
//@[07:0033) | | └─Token(Identifier) |tenantModuleDuplicateName3|
//@[34:0056) | ├─StringSyntax
//@[34:0056) | | └─Token(StringComplete) |'modules/tenant.bicep'|
//@[57:0058) | ├─Token(Assignment) |=|
//@[59:0098) | └─ObjectSyntax
//@[59:0060) |   ├─Token(LeftBrace) |{|
//@[60:0061) |   ├─Token(NewLine) |\n|
  name: 'tenantModuleDuplicateName'
//@[02:0035) |   ├─ObjectPropertySyntax
//@[02:0006) |   | ├─IdentifierSyntax
//@[02:0006) |   | | └─Token(Identifier) |name|
//@[06:0007) |   | ├─Token(Colon) |:|
//@[08:0035) |   | └─StringSyntax
//@[08:0035) |   |   └─Token(StringComplete) |'tenantModuleDuplicateName'|
//@[35:0036) |   ├─Token(NewLine) |\n|
}
//@[00:0001) |   └─Token(RightBrace) |}|
//@[01:0003) ├─Token(NewLine) |\n\n|

module managementGroupModuleDuplicateName1 'modules/managementGroup.bicep' = {
//@[00:0156) ├─ModuleDeclarationSyntax
//@[00:0006) | ├─Token(Identifier) |module|
//@[07:0042) | ├─IdentifierSyntax
//@[07:0042) | | └─Token(Identifier) |managementGroupModuleDuplicateName1|
//@[43:0074) | ├─StringSyntax
//@[43:0074) | | └─Token(StringComplete) |'modules/managementGroup.bicep'|
//@[75:0076) | ├─Token(Assignment) |=|
//@[77:0156) | └─ObjectSyntax
//@[77:0078) |   ├─Token(LeftBrace) |{|
//@[78:0079) |   ├─Token(NewLine) |\n|
  name: 'managementGroupModuleDuplicateName'
//@[02:0044) |   ├─ObjectPropertySyntax
//@[02:0006) |   | ├─IdentifierSyntax
//@[02:0006) |   | | └─Token(Identifier) |name|
//@[06:0007) |   | ├─Token(Colon) |:|
//@[08:0044) |   | └─StringSyntax
//@[08:0044) |   |   └─Token(StringComplete) |'managementGroupModuleDuplicateName'|
//@[44:0045) |   ├─Token(NewLine) |\n|
  scope: managementGroup('MG')
//@[02:0030) |   ├─ObjectPropertySyntax
//@[02:0007) |   | ├─IdentifierSyntax
//@[02:0007) |   | | └─Token(Identifier) |scope|
//@[07:0008) |   | ├─Token(Colon) |:|
//@[09:0030) |   | └─FunctionCallSyntax
//@[09:0024) |   |   ├─IdentifierSyntax
//@[09:0024) |   |   | └─Token(Identifier) |managementGroup|
//@[24:0025) |   |   ├─Token(LeftParen) |(|
//@[25:0029) |   |   ├─FunctionArgumentSyntax
//@[25:0029) |   |   | └─StringSyntax
//@[25:0029) |   |   |   └─Token(StringComplete) |'MG'|
//@[29:0030) |   |   └─Token(RightParen) |)|
//@[30:0031) |   ├─Token(NewLine) |\n|
}
//@[00:0001) |   └─Token(RightBrace) |}|
//@[01:0003) ├─Token(NewLine) |\n\n|

module managementGroupModuleDuplicateName2 'modules/managementGroup.bicep' = {
//@[00:0156) ├─ModuleDeclarationSyntax
//@[00:0006) | ├─Token(Identifier) |module|
//@[07:0042) | ├─IdentifierSyntax
//@[07:0042) | | └─Token(Identifier) |managementGroupModuleDuplicateName2|
//@[43:0074) | ├─StringSyntax
//@[43:0074) | | └─Token(StringComplete) |'modules/managementGroup.bicep'|
//@[75:0076) | ├─Token(Assignment) |=|
//@[77:0156) | └─ObjectSyntax
//@[77:0078) |   ├─Token(LeftBrace) |{|
//@[78:0079) |   ├─Token(NewLine) |\n|
  name: 'managementGroupModuleDuplicateName'
//@[02:0044) |   ├─ObjectPropertySyntax
//@[02:0006) |   | ├─IdentifierSyntax
//@[02:0006) |   | | └─Token(Identifier) |name|
//@[06:0007) |   | ├─Token(Colon) |:|
//@[08:0044) |   | └─StringSyntax
//@[08:0044) |   |   └─Token(StringComplete) |'managementGroupModuleDuplicateName'|
//@[44:0045) |   ├─Token(NewLine) |\n|
  scope: managementGroup('MG')
//@[02:0030) |   ├─ObjectPropertySyntax
//@[02:0007) |   | ├─IdentifierSyntax
//@[02:0007) |   | | └─Token(Identifier) |scope|
//@[07:0008) |   | ├─Token(Colon) |:|
//@[09:0030) |   | └─FunctionCallSyntax
//@[09:0024) |   |   ├─IdentifierSyntax
//@[09:0024) |   |   | └─Token(Identifier) |managementGroup|
//@[24:0025) |   |   ├─Token(LeftParen) |(|
//@[25:0029) |   |   ├─FunctionArgumentSyntax
//@[25:0029) |   |   | └─StringSyntax
//@[25:0029) |   |   |   └─Token(StringComplete) |'MG'|
//@[29:0030) |   |   └─Token(RightParen) |)|
//@[30:0031) |   ├─Token(NewLine) |\n|
}
//@[00:0001) |   └─Token(RightBrace) |}|
//@[01:0003) ├─Token(NewLine) |\n\n|

module subscriptionModuleDuplicateName1 'modules/subscription.bicep' = {
//@[00:0178) ├─ModuleDeclarationSyntax
//@[00:0006) | ├─Token(Identifier) |module|
//@[07:0039) | ├─IdentifierSyntax
//@[07:0039) | | └─Token(Identifier) |subscriptionModuleDuplicateName1|
//@[40:0068) | ├─StringSyntax
//@[40:0068) | | └─Token(StringComplete) |'modules/subscription.bicep'|
//@[69:0070) | ├─Token(Assignment) |=|
//@[71:0178) | └─ObjectSyntax
//@[71:0072) |   ├─Token(LeftBrace) |{|
//@[72:0073) |   ├─Token(NewLine) |\n|
  name: 'subscriptionModuleDuplicateName'
//@[02:0041) |   ├─ObjectPropertySyntax
//@[02:0006) |   | ├─IdentifierSyntax
//@[02:0006) |   | | └─Token(Identifier) |name|
//@[06:0007) |   | ├─Token(Colon) |:|
//@[08:0041) |   | └─StringSyntax
//@[08:0041) |   |   └─Token(StringComplete) |'subscriptionModuleDuplicateName'|
//@[41:0042) |   ├─Token(NewLine) |\n|
  scope: subscription('1ad827ac-2669-4c2f-9970-282b93c3c550')
//@[02:0061) |   ├─ObjectPropertySyntax
//@[02:0007) |   | ├─IdentifierSyntax
//@[02:0007) |   | | └─Token(Identifier) |scope|
//@[07:0008) |   | ├─Token(Colon) |:|
//@[09:0061) |   | └─FunctionCallSyntax
//@[09:0021) |   |   ├─IdentifierSyntax
//@[09:0021) |   |   | └─Token(Identifier) |subscription|
//@[21:0022) |   |   ├─Token(LeftParen) |(|
//@[22:0060) |   |   ├─FunctionArgumentSyntax
//@[22:0060) |   |   | └─StringSyntax
//@[22:0060) |   |   |   └─Token(StringComplete) |'1ad827ac-2669-4c2f-9970-282b93c3c550'|
//@[60:0061) |   |   └─Token(RightParen) |)|
//@[61:0062) |   ├─Token(NewLine) |\n|
}
//@[00:0001) |   └─Token(RightBrace) |}|
//@[01:0003) ├─Token(NewLine) |\n\n|

module subscriptionModuleDuplicateName2 'modules/subscription.bicep' = {
//@[00:0178) ├─ModuleDeclarationSyntax
//@[00:0006) | ├─Token(Identifier) |module|
//@[07:0039) | ├─IdentifierSyntax
//@[07:0039) | | └─Token(Identifier) |subscriptionModuleDuplicateName2|
//@[40:0068) | ├─StringSyntax
//@[40:0068) | | └─Token(StringComplete) |'modules/subscription.bicep'|
//@[69:0070) | ├─Token(Assignment) |=|
//@[71:0178) | └─ObjectSyntax
//@[71:0072) |   ├─Token(LeftBrace) |{|
//@[72:0073) |   ├─Token(NewLine) |\n|
  name: 'subscriptionModuleDuplicateName'
//@[02:0041) |   ├─ObjectPropertySyntax
//@[02:0006) |   | ├─IdentifierSyntax
//@[02:0006) |   | | └─Token(Identifier) |name|
//@[06:0007) |   | ├─Token(Colon) |:|
//@[08:0041) |   | └─StringSyntax
//@[08:0041) |   |   └─Token(StringComplete) |'subscriptionModuleDuplicateName'|
//@[41:0042) |   ├─Token(NewLine) |\n|
  scope: subscription('1ad827ac-2669-4c2f-9970-282b93c3c550')
//@[02:0061) |   ├─ObjectPropertySyntax
//@[02:0007) |   | ├─IdentifierSyntax
//@[02:0007) |   | | └─Token(Identifier) |scope|
//@[07:0008) |   | ├─Token(Colon) |:|
//@[09:0061) |   | └─FunctionCallSyntax
//@[09:0021) |   |   ├─IdentifierSyntax
//@[09:0021) |   |   | └─Token(Identifier) |subscription|
//@[21:0022) |   |   ├─Token(LeftParen) |(|
//@[22:0060) |   |   ├─FunctionArgumentSyntax
//@[22:0060) |   |   | └─StringSyntax
//@[22:0060) |   |   |   └─Token(StringComplete) |'1ad827ac-2669-4c2f-9970-282b93c3c550'|
//@[60:0061) |   |   └─Token(RightParen) |)|
//@[61:0062) |   ├─Token(NewLine) |\n|
}
//@[00:0001) |   └─Token(RightBrace) |}|
//@[01:0003) ├─Token(NewLine) |\n\n|

module managementGroupModules 'modules/managementGroup.bicep' = [for (mg, i) in []: {
//@[00:0137) ├─ModuleDeclarationSyntax
//@[00:0006) | ├─Token(Identifier) |module|
//@[07:0029) | ├─IdentifierSyntax
//@[07:0029) | | └─Token(Identifier) |managementGroupModules|
//@[30:0061) | ├─StringSyntax
//@[30:0061) | | └─Token(StringComplete) |'modules/managementGroup.bicep'|
//@[62:0063) | ├─Token(Assignment) |=|
//@[64:0137) | └─ForSyntax
//@[64:0065) |   ├─Token(LeftSquare) |[|
//@[65:0068) |   ├─Token(Identifier) |for|
//@[69:0076) |   ├─VariableBlockSyntax
//@[69:0070) |   | ├─Token(LeftParen) |(|
//@[70:0072) |   | ├─LocalVariableSyntax
//@[70:0072) |   | | └─IdentifierSyntax
//@[70:0072) |   | |   └─Token(Identifier) |mg|
//@[72:0073) |   | ├─Token(Comma) |,|
//@[74:0075) |   | ├─LocalVariableSyntax
//@[74:0075) |   | | └─IdentifierSyntax
//@[74:0075) |   | |   └─Token(Identifier) |i|
//@[75:0076) |   | └─Token(RightParen) |)|
//@[77:0079) |   ├─Token(Identifier) |in|
//@[80:0082) |   ├─ArraySyntax
//@[80:0081) |   | ├─Token(LeftSquare) |[|
//@[81:0082) |   | └─Token(RightSquare) |]|
//@[82:0083) |   ├─Token(Colon) |:|
//@[84:0136) |   ├─ObjectSyntax
//@[84:0085) |   | ├─Token(LeftBrace) |{|
//@[85:0086) |   | ├─Token(NewLine) |\n|
  name: 'dep-${mg}'
//@[02:0019) |   | ├─ObjectPropertySyntax
//@[02:0006) |   | | ├─IdentifierSyntax
//@[02:0006) |   | | | └─Token(Identifier) |name|
//@[06:0007) |   | | ├─Token(Colon) |:|
//@[08:0019) |   | | └─StringSyntax
//@[08:0015) |   | |   ├─Token(StringLeftPiece) |'dep-${|
//@[15:0017) |   | |   ├─VariableAccessSyntax
//@[15:0017) |   | |   | └─IdentifierSyntax
//@[15:0017) |   | |   |   └─Token(Identifier) |mg|
//@[17:0019) |   | |   └─Token(StringRightPiece) |}'|
//@[19:0020) |   | ├─Token(NewLine) |\n|
  scope: managementGroup(mg)
//@[02:0028) |   | ├─ObjectPropertySyntax
//@[02:0007) |   | | ├─IdentifierSyntax
//@[02:0007) |   | | | └─Token(Identifier) |scope|
//@[07:0008) |   | | ├─Token(Colon) |:|
//@[09:0028) |   | | └─FunctionCallSyntax
//@[09:0024) |   | |   ├─IdentifierSyntax
//@[09:0024) |   | |   | └─Token(Identifier) |managementGroup|
//@[24:0025) |   | |   ├─Token(LeftParen) |(|
//@[25:0027) |   | |   ├─FunctionArgumentSyntax
//@[25:0027) |   | |   | └─VariableAccessSyntax
//@[25:0027) |   | |   |   └─IdentifierSyntax
//@[25:0027) |   | |   |     └─Token(Identifier) |mg|
//@[27:0028) |   | |   └─Token(RightParen) |)|
//@[28:0029) |   | ├─Token(NewLine) |\n|
}]
//@[00:0001) |   | └─Token(RightBrace) |}|
//@[01:0002) |   └─Token(RightSquare) |]|
//@[02:0004) ├─Token(NewLine) |\n\n|

module cannotUseModuleCollectionAsScope 'modules/managementGroup.bicep' = [for (mg, i) in []: {
//@[00:0150) ├─ModuleDeclarationSyntax
//@[00:0006) | ├─Token(Identifier) |module|
//@[07:0039) | ├─IdentifierSyntax
//@[07:0039) | | └─Token(Identifier) |cannotUseModuleCollectionAsScope|
//@[40:0071) | ├─StringSyntax
//@[40:0071) | | └─Token(StringComplete) |'modules/managementGroup.bicep'|
//@[72:0073) | ├─Token(Assignment) |=|
//@[74:0150) | └─ForSyntax
//@[74:0075) |   ├─Token(LeftSquare) |[|
//@[75:0078) |   ├─Token(Identifier) |for|
//@[79:0086) |   ├─VariableBlockSyntax
//@[79:0080) |   | ├─Token(LeftParen) |(|
//@[80:0082) |   | ├─LocalVariableSyntax
//@[80:0082) |   | | └─IdentifierSyntax
//@[80:0082) |   | |   └─Token(Identifier) |mg|
//@[82:0083) |   | ├─Token(Comma) |,|
//@[84:0085) |   | ├─LocalVariableSyntax
//@[84:0085) |   | | └─IdentifierSyntax
//@[84:0085) |   | |   └─Token(Identifier) |i|
//@[85:0086) |   | └─Token(RightParen) |)|
//@[87:0089) |   ├─Token(Identifier) |in|
//@[90:0092) |   ├─ArraySyntax
//@[90:0091) |   | ├─Token(LeftSquare) |[|
//@[91:0092) |   | └─Token(RightSquare) |]|
//@[92:0093) |   ├─Token(Colon) |:|
//@[94:0149) |   ├─ObjectSyntax
//@[94:0095) |   | ├─Token(LeftBrace) |{|
//@[95:0096) |   | ├─Token(NewLine) |\n|
  name: 'dep-${mg}'
//@[02:0019) |   | ├─ObjectPropertySyntax
//@[02:0006) |   | | ├─IdentifierSyntax
//@[02:0006) |   | | | └─Token(Identifier) |name|
//@[06:0007) |   | | ├─Token(Colon) |:|
//@[08:0019) |   | | └─StringSyntax
//@[08:0015) |   | |   ├─Token(StringLeftPiece) |'dep-${|
//@[15:0017) |   | |   ├─VariableAccessSyntax
//@[15:0017) |   | |   | └─IdentifierSyntax
//@[15:0017) |   | |   |   └─Token(Identifier) |mg|
//@[17:0019) |   | |   └─Token(StringRightPiece) |}'|
//@[19:0020) |   | ├─Token(NewLine) |\n|
  scope: managementGroupModules
//@[02:0031) |   | ├─ObjectPropertySyntax
//@[02:0007) |   | | ├─IdentifierSyntax
//@[02:0007) |   | | | └─Token(Identifier) |scope|
//@[07:0008) |   | | ├─Token(Colon) |:|
//@[09:0031) |   | | └─VariableAccessSyntax
//@[09:0031) |   | |   └─IdentifierSyntax
//@[09:0031) |   | |     └─Token(Identifier) |managementGroupModules|
//@[31:0032) |   | ├─Token(NewLine) |\n|
}]
//@[00:0001) |   | └─Token(RightBrace) |}|
//@[01:0002) |   └─Token(RightSquare) |]|
//@[02:0004) ├─Token(NewLine) |\n\n|

module cannotUseSingleModuleAsScope 'modules/managementGroup.bicep' = [for (mg, i) in []: {
//@[00:0149) ├─ModuleDeclarationSyntax
//@[00:0006) | ├─Token(Identifier) |module|
//@[07:0035) | ├─IdentifierSyntax
//@[07:0035) | | └─Token(Identifier) |cannotUseSingleModuleAsScope|
//@[36:0067) | ├─StringSyntax
//@[36:0067) | | └─Token(StringComplete) |'modules/managementGroup.bicep'|
//@[68:0069) | ├─Token(Assignment) |=|
//@[70:0149) | └─ForSyntax
//@[70:0071) |   ├─Token(LeftSquare) |[|
//@[71:0074) |   ├─Token(Identifier) |for|
//@[75:0082) |   ├─VariableBlockSyntax
//@[75:0076) |   | ├─Token(LeftParen) |(|
//@[76:0078) |   | ├─LocalVariableSyntax
//@[76:0078) |   | | └─IdentifierSyntax
//@[76:0078) |   | |   └─Token(Identifier) |mg|
//@[78:0079) |   | ├─Token(Comma) |,|
//@[80:0081) |   | ├─LocalVariableSyntax
//@[80:0081) |   | | └─IdentifierSyntax
//@[80:0081) |   | |   └─Token(Identifier) |i|
//@[81:0082) |   | └─Token(RightParen) |)|
//@[83:0085) |   ├─Token(Identifier) |in|
//@[86:0088) |   ├─ArraySyntax
//@[86:0087) |   | ├─Token(LeftSquare) |[|
//@[87:0088) |   | └─Token(RightSquare) |]|
//@[88:0089) |   ├─Token(Colon) |:|
//@[90:0148) |   ├─ObjectSyntax
//@[90:0091) |   | ├─Token(LeftBrace) |{|
//@[91:0092) |   | ├─Token(NewLine) |\n|
  name: 'dep-${mg}'
//@[02:0019) |   | ├─ObjectPropertySyntax
//@[02:0006) |   | | ├─IdentifierSyntax
//@[02:0006) |   | | | └─Token(Identifier) |name|
//@[06:0007) |   | | ├─Token(Colon) |:|
//@[08:0019) |   | | └─StringSyntax
//@[08:0015) |   | |   ├─Token(StringLeftPiece) |'dep-${|
//@[15:0017) |   | |   ├─VariableAccessSyntax
//@[15:0017) |   | |   | └─IdentifierSyntax
//@[15:0017) |   | |   |   └─Token(Identifier) |mg|
//@[17:0019) |   | |   └─Token(StringRightPiece) |}'|
//@[19:0020) |   | ├─Token(NewLine) |\n|
  scope: managementGroupModules[i]
//@[02:0034) |   | ├─ObjectPropertySyntax
//@[02:0007) |   | | ├─IdentifierSyntax
//@[02:0007) |   | | | └─Token(Identifier) |scope|
//@[07:0008) |   | | ├─Token(Colon) |:|
//@[09:0034) |   | | └─ArrayAccessSyntax
//@[09:0031) |   | |   ├─VariableAccessSyntax
//@[09:0031) |   | |   | └─IdentifierSyntax
//@[09:0031) |   | |   |   └─Token(Identifier) |managementGroupModules|
//@[31:0032) |   | |   ├─Token(LeftSquare) |[|
//@[32:0033) |   | |   ├─VariableAccessSyntax
//@[32:0033) |   | |   | └─IdentifierSyntax
//@[32:0033) |   | |   |   └─Token(Identifier) |i|
//@[33:0034) |   | |   └─Token(RightSquare) |]|
//@[34:0035) |   | ├─Token(NewLine) |\n|
}]
//@[00:0001) |   | └─Token(RightBrace) |}|
//@[01:0002) |   └─Token(RightSquare) |]|
//@[02:0004) ├─Token(NewLine) |\n\n|

module cannotUseSingleModuleAsScope2 'modules/managementGroup.bicep' = {
//@[00:0134) ├─ModuleDeclarationSyntax
//@[00:0006) | ├─Token(Identifier) |module|
//@[07:0036) | ├─IdentifierSyntax
//@[07:0036) | | └─Token(Identifier) |cannotUseSingleModuleAsScope2|
//@[37:0068) | ├─StringSyntax
//@[37:0068) | | └─Token(StringComplete) |'modules/managementGroup.bicep'|
//@[69:0070) | ├─Token(Assignment) |=|
//@[71:0134) | └─ObjectSyntax
//@[71:0072) |   ├─Token(LeftBrace) |{|
//@[72:0073) |   ├─Token(NewLine) |\n|
  name: 'test'
//@[02:0014) |   ├─ObjectPropertySyntax
//@[02:0006) |   | ├─IdentifierSyntax
//@[02:0006) |   | | └─Token(Identifier) |name|
//@[06:0007) |   | ├─Token(Colon) |:|
//@[08:0014) |   | └─StringSyntax
//@[08:0014) |   |   └─Token(StringComplete) |'test'|
//@[14:0015) |   ├─Token(NewLine) |\n|
  scope: managementGroupModuleDuplicateName1
//@[02:0044) |   ├─ObjectPropertySyntax
//@[02:0007) |   | ├─IdentifierSyntax
//@[02:0007) |   | | └─Token(Identifier) |scope|
//@[07:0008) |   | ├─Token(Colon) |:|
//@[09:0044) |   | └─VariableAccessSyntax
//@[09:0044) |   |   └─IdentifierSyntax
//@[09:0044) |   |     └─Token(Identifier) |managementGroupModuleDuplicateName1|
//@[44:0045) |   ├─Token(NewLine) |\n|
}
//@[00:0001) |   └─Token(RightBrace) |}|
//@[01:0002) ├─Token(NewLine) |\n|

//@[00:0000) └─Token(EndOfFile) ||
