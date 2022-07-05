targetScope = 'managementGroup'
//@[00:1039) ProgramSyntax
//@[00:0031) ├─TargetScopeSyntax
//@[00:0011) | ├─Token(Identifier) |targetScope|
//@[12:0013) | ├─Token(Assignment) |=|
//@[14:0031) | └─StringSyntax
//@[14:0031) |   └─Token(StringComplete) |'managementGroup'|
//@[31:0033) ├─Token(NewLine) |\n\n|

module managementGroupModuleDuplicateName1 'modules/managementGroup.bicep' = {
//@[00:0152) ├─ModuleDeclarationSyntax
//@[00:0006) | ├─Token(Identifier) |module|
//@[07:0042) | ├─IdentifierSyntax
//@[07:0042) | | └─Token(Identifier) |managementGroupModuleDuplicateName1|
//@[43:0074) | ├─StringSyntax
//@[43:0074) | | └─Token(StringComplete) |'modules/managementGroup.bicep'|
//@[75:0076) | ├─Token(Assignment) |=|
//@[77:0152) | └─ObjectSyntax
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
  scope: managementGroup()
//@[02:0026) |   ├─ObjectPropertySyntax
//@[02:0007) |   | ├─IdentifierSyntax
//@[02:0007) |   | | └─Token(Identifier) |scope|
//@[07:0008) |   | ├─Token(Colon) |:|
//@[09:0026) |   | └─FunctionCallSyntax
//@[09:0024) |   |   ├─IdentifierSyntax
//@[09:0024) |   |   | └─Token(Identifier) |managementGroup|
//@[24:0025) |   |   ├─Token(LeftParen) |(|
//@[25:0026) |   |   └─Token(RightParen) |)|
//@[26:0027) |   ├─Token(NewLine) |\n|
}
//@[00:0001) |   └─Token(RightBrace) |}|
//@[01:0003) ├─Token(NewLine) |\n\n|

module managementGroupModuleDuplicateName2 'modules/managementGroup.bicep' = {
//@[00:0152) ├─ModuleDeclarationSyntax
//@[00:0006) | ├─Token(Identifier) |module|
//@[07:0042) | ├─IdentifierSyntax
//@[07:0042) | | └─Token(Identifier) |managementGroupModuleDuplicateName2|
//@[43:0074) | ├─StringSyntax
//@[43:0074) | | └─Token(StringComplete) |'modules/managementGroup.bicep'|
//@[75:0076) | ├─Token(Assignment) |=|
//@[77:0152) | └─ObjectSyntax
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
  scope: managementGroup()
//@[02:0026) |   ├─ObjectPropertySyntax
//@[02:0007) |   | ├─IdentifierSyntax
//@[02:0007) |   | | └─Token(Identifier) |scope|
//@[07:0008) |   | ├─Token(Colon) |:|
//@[09:0026) |   | └─FunctionCallSyntax
//@[09:0024) |   |   ├─IdentifierSyntax
//@[09:0024) |   |   | └─Token(Identifier) |managementGroup|
//@[24:0025) |   |   ├─Token(LeftParen) |(|
//@[25:0026) |   |   └─Token(RightParen) |)|
//@[26:0027) |   ├─Token(NewLine) |\n|
}
//@[00:0001) |   └─Token(RightBrace) |}|
//@[01:0003) ├─Token(NewLine) |\n\n|

module managementGroupModuleDuplicateName3 'modules/managementGroup.bicep' = {
//@[00:0125) ├─ModuleDeclarationSyntax
//@[00:0006) | ├─Token(Identifier) |module|
//@[07:0042) | ├─IdentifierSyntax
//@[07:0042) | | └─Token(Identifier) |managementGroupModuleDuplicateName3|
//@[43:0074) | ├─StringSyntax
//@[43:0074) | | └─Token(StringComplete) |'modules/managementGroup.bicep'|
//@[75:0076) | ├─Token(Assignment) |=|
//@[77:0125) | └─ObjectSyntax
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
}
//@[00:0001) |   └─Token(RightBrace) |}|
//@[01:0003) ├─Token(NewLine) |\n\n|

module subscriptionModuleDuplicateName1 'modules/subscription.bicep' = {
//@[00:0172) ├─ModuleDeclarationSyntax
//@[00:0006) | ├─Token(Identifier) |module|
//@[07:0039) | ├─IdentifierSyntax
//@[07:0039) | | └─Token(Identifier) |subscriptionModuleDuplicateName1|
//@[40:0068) | ├─StringSyntax
//@[40:0068) | | └─Token(StringComplete) |'modules/subscription.bicep'|
//@[69:0070) | ├─Token(Assignment) |=|
//@[71:0172) | └─ObjectSyntax
//@[71:0072) |   ├─Token(LeftBrace) |{|
//@[72:0073) |   ├─Token(NewLine) |\n|
  name: 'subscriptionDuplicateName'
//@[02:0035) |   ├─ObjectPropertySyntax
//@[02:0006) |   | ├─IdentifierSyntax
//@[02:0006) |   | | └─Token(Identifier) |name|
//@[06:0007) |   | ├─Token(Colon) |:|
//@[08:0035) |   | └─StringSyntax
//@[08:0035) |   |   └─Token(StringComplete) |'subscriptionDuplicateName'|
//@[35:0036) |   ├─Token(NewLine) |\n|
  scope: subscription('c56ffff6-0806-4a98-83fb-17aed775d6e4')
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
//@[22:0060) |   |   |   └─Token(StringComplete) |'c56ffff6-0806-4a98-83fb-17aed775d6e4'|
//@[60:0061) |   |   └─Token(RightParen) |)|
//@[61:0062) |   ├─Token(NewLine) |\n|
}
//@[00:0001) |   └─Token(RightBrace) |}|
//@[01:0003) ├─Token(NewLine) |\n\n|

module subscriptionModuleDuplicateName2 'modules/subscription.bicep' = {
//@[00:0172) ├─ModuleDeclarationSyntax
//@[00:0006) | ├─Token(Identifier) |module|
//@[07:0039) | ├─IdentifierSyntax
//@[07:0039) | | └─Token(Identifier) |subscriptionModuleDuplicateName2|
//@[40:0068) | ├─StringSyntax
//@[40:0068) | | └─Token(StringComplete) |'modules/subscription.bicep'|
//@[69:0070) | ├─Token(Assignment) |=|
//@[71:0172) | └─ObjectSyntax
//@[71:0072) |   ├─Token(LeftBrace) |{|
//@[72:0073) |   ├─Token(NewLine) |\n|
  name: 'subscriptionDuplicateName'
//@[02:0035) |   ├─ObjectPropertySyntax
//@[02:0006) |   | ├─IdentifierSyntax
//@[02:0006) |   | | └─Token(Identifier) |name|
//@[06:0007) |   | ├─Token(Colon) |:|
//@[08:0035) |   | └─StringSyntax
//@[08:0035) |   |   └─Token(StringComplete) |'subscriptionDuplicateName'|
//@[35:0036) |   ├─Token(NewLine) |\n|
  scope: subscription('c56ffff6-0806-4a98-83fb-17aed775d6e4')
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
//@[22:0060) |   |   |   └─Token(StringComplete) |'c56ffff6-0806-4a98-83fb-17aed775d6e4'|
//@[60:0061) |   |   └─Token(RightParen) |)|
//@[61:0062) |   ├─Token(NewLine) |\n|
}
//@[00:0001) |   └─Token(RightBrace) |}|
//@[01:0003) ├─Token(NewLine) |\n\n|

module tenantModuleDuplicateName1 'modules/tenant.bicep' = {
//@[00:0110) ├─ModuleDeclarationSyntax
//@[00:0006) | ├─Token(Identifier) |module|
//@[07:0033) | ├─IdentifierSyntax
//@[07:0033) | | └─Token(Identifier) |tenantModuleDuplicateName1|
//@[34:0056) | ├─StringSyntax
//@[34:0056) | | └─Token(StringComplete) |'modules/tenant.bicep'|
//@[57:0058) | ├─Token(Assignment) |=|
//@[59:0110) | └─ObjectSyntax
//@[59:0060) |   ├─Token(LeftBrace) |{|
//@[60:0061) |   ├─Token(NewLine) |\n|
  name: 'tenantDuplicateName'
//@[02:0029) |   ├─ObjectPropertySyntax
//@[02:0006) |   | ├─IdentifierSyntax
//@[02:0006) |   | | └─Token(Identifier) |name|
//@[06:0007) |   | ├─Token(Colon) |:|
//@[08:0029) |   | └─StringSyntax
//@[08:0029) |   |   └─Token(StringComplete) |'tenantDuplicateName'|
//@[29:0030) |   ├─Token(NewLine) |\n|
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
//@[00:0110) ├─ModuleDeclarationSyntax
//@[00:0006) | ├─Token(Identifier) |module|
//@[07:0033) | ├─IdentifierSyntax
//@[07:0033) | | └─Token(Identifier) |tenantModuleDuplicateName2|
//@[34:0056) | ├─StringSyntax
//@[34:0056) | | └─Token(StringComplete) |'modules/tenant.bicep'|
//@[57:0058) | ├─Token(Assignment) |=|
//@[59:0110) | └─ObjectSyntax
//@[59:0060) |   ├─Token(LeftBrace) |{|
//@[60:0061) |   ├─Token(NewLine) |\n|
  name: 'tenantDuplicateName'
//@[02:0029) |   ├─ObjectPropertySyntax
//@[02:0006) |   | ├─IdentifierSyntax
//@[02:0006) |   | | └─Token(Identifier) |name|
//@[06:0007) |   | ├─Token(Colon) |:|
//@[08:0029) |   | └─StringSyntax
//@[08:0029) |   |   └─Token(StringComplete) |'tenantDuplicateName'|
//@[29:0030) |   ├─Token(NewLine) |\n|
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
//@[01:0002) ├─Token(NewLine) |\n|

//@[00:0000) └─Token(EndOfFile) ||
