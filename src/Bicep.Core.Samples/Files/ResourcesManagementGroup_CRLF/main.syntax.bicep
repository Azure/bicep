targetScope = 'managementGroup'
//@[000:1517) ProgramSyntax
//@[000:0031) ├─TargetScopeSyntax
//@[000:0011) | ├─Token(Identifier) |targetScope|
//@[012:0013) | ├─Token(Assignment) |=|
//@[014:0031) | └─StringSyntax
//@[014:0031) |   └─Token(StringComplete) |'managementGroup'|
//@[031:0035) ├─Token(NewLine) |\r\n\r\n|

param ownerPrincipalId string
//@[000:0029) ├─ParameterDeclarationSyntax
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0022) | ├─IdentifierSyntax
//@[006:0022) | | └─Token(Identifier) |ownerPrincipalId|
//@[023:0029) | └─SimpleTypeSyntax
//@[023:0029) |   └─Token(Identifier) |string|
//@[029:0033) ├─Token(NewLine) |\r\n\r\n|

param contributorPrincipals array
//@[000:0033) ├─ParameterDeclarationSyntax
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0027) | ├─IdentifierSyntax
//@[006:0027) | | └─Token(Identifier) |contributorPrincipals|
//@[028:0033) | └─SimpleTypeSyntax
//@[028:0033) |   └─Token(Identifier) |array|
//@[033:0035) ├─Token(NewLine) |\r\n|
param readerPrincipals array
//@[000:0028) ├─ParameterDeclarationSyntax
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0022) | ├─IdentifierSyntax
//@[006:0022) | | └─Token(Identifier) |readerPrincipals|
//@[023:0028) | └─SimpleTypeSyntax
//@[023:0028) |   └─Token(Identifier) |array|
//@[028:0032) ├─Token(NewLine) |\r\n\r\n|

resource owner 'Microsoft.Authorization/roleAssignments@2020-04-01-preview' = {
//@[000:0242) ├─ResourceDeclarationSyntax
//@[000:0008) | ├─Token(Identifier) |resource|
//@[009:0014) | ├─IdentifierSyntax
//@[009:0014) | | └─Token(Identifier) |owner|
//@[015:0075) | ├─StringSyntax
//@[015:0075) | | └─Token(StringComplete) |'Microsoft.Authorization/roleAssignments@2020-04-01-preview'|
//@[076:0077) | ├─Token(Assignment) |=|
//@[078:0242) | └─ObjectSyntax
//@[078:0079) |   ├─Token(LeftBrace) |{|
//@[079:0081) |   ├─Token(NewLine) |\r\n|
  name: guid('owner', ownerPrincipalId)
//@[002:0039) |   ├─ObjectPropertySyntax
//@[002:0006) |   | ├─IdentifierSyntax
//@[002:0006) |   | | └─Token(Identifier) |name|
//@[006:0007) |   | ├─Token(Colon) |:|
//@[008:0039) |   | └─FunctionCallSyntax
//@[008:0012) |   |   ├─IdentifierSyntax
//@[008:0012) |   |   | └─Token(Identifier) |guid|
//@[012:0013) |   |   ├─Token(LeftParen) |(|
//@[013:0020) |   |   ├─FunctionArgumentSyntax
//@[013:0020) |   |   | └─StringSyntax
//@[013:0020) |   |   |   └─Token(StringComplete) |'owner'|
//@[020:0021) |   |   ├─Token(Comma) |,|
//@[022:0038) |   |   ├─FunctionArgumentSyntax
//@[022:0038) |   |   | └─VariableAccessSyntax
//@[022:0038) |   |   |   └─IdentifierSyntax
//@[022:0038) |   |   |     └─Token(Identifier) |ownerPrincipalId|
//@[038:0039) |   |   └─Token(RightParen) |)|
//@[039:0041) |   ├─Token(NewLine) |\r\n|
  properties: {
//@[002:0117) |   ├─ObjectPropertySyntax
//@[002:0012) |   | ├─IdentifierSyntax
//@[002:0012) |   | | └─Token(Identifier) |properties|
//@[012:0013) |   | ├─Token(Colon) |:|
//@[014:0117) |   | └─ObjectSyntax
//@[014:0015) |   |   ├─Token(LeftBrace) |{|
//@[015:0017) |   |   ├─Token(NewLine) |\r\n|
    principalId: ownerPrincipalId
//@[004:0033) |   |   ├─ObjectPropertySyntax
//@[004:0015) |   |   | ├─IdentifierSyntax
//@[004:0015) |   |   | | └─Token(Identifier) |principalId|
//@[015:0016) |   |   | ├─Token(Colon) |:|
//@[017:0033) |   |   | └─VariableAccessSyntax
//@[017:0033) |   |   |   └─IdentifierSyntax
//@[017:0033) |   |   |     └─Token(Identifier) |ownerPrincipalId|
//@[033:0035) |   |   ├─Token(NewLine) |\r\n|
    roleDefinitionId: '8e3af657-a8ff-443c-a75c-2fe8c4bcb635'
//@[004:0060) |   |   ├─ObjectPropertySyntax
//@[004:0020) |   |   | ├─IdentifierSyntax
//@[004:0020) |   |   | | └─Token(Identifier) |roleDefinitionId|
//@[020:0021) |   |   | ├─Token(Colon) |:|
//@[022:0060) |   |   | └─StringSyntax
//@[022:0060) |   |   |   └─Token(StringComplete) |'8e3af657-a8ff-443c-a75c-2fe8c4bcb635'|
//@[060:0062) |   |   ├─Token(NewLine) |\r\n|
  }
//@[002:0003) |   |   └─Token(RightBrace) |}|
//@[003:0005) |   ├─Token(NewLine) |\r\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0005) ├─Token(NewLine) |\r\n\r\n|

resource contributors 'Microsoft.Authorization/roleAssignments@2020-04-01-preview' = [for contributor in contributorPrincipals: {
//@[000:0321) ├─ResourceDeclarationSyntax
//@[000:0008) | ├─Token(Identifier) |resource|
//@[009:0021) | ├─IdentifierSyntax
//@[009:0021) | | └─Token(Identifier) |contributors|
//@[022:0082) | ├─StringSyntax
//@[022:0082) | | └─Token(StringComplete) |'Microsoft.Authorization/roleAssignments@2020-04-01-preview'|
//@[083:0084) | ├─Token(Assignment) |=|
//@[085:0321) | └─ForSyntax
//@[085:0086) |   ├─Token(LeftSquare) |[|
//@[086:0089) |   ├─Token(Identifier) |for|
//@[090:0101) |   ├─LocalVariableSyntax
//@[090:0101) |   | └─IdentifierSyntax
//@[090:0101) |   |   └─Token(Identifier) |contributor|
//@[102:0104) |   ├─Token(Identifier) |in|
//@[105:0126) |   ├─VariableAccessSyntax
//@[105:0126) |   | └─IdentifierSyntax
//@[105:0126) |   |   └─Token(Identifier) |contributorPrincipals|
//@[126:0127) |   ├─Token(Colon) |:|
//@[128:0320) |   ├─ObjectSyntax
//@[128:0129) |   | ├─Token(LeftBrace) |{|
//@[129:0131) |   | ├─Token(NewLine) |\r\n|
  name: guid('contributor', contributor)
//@[002:0040) |   | ├─ObjectPropertySyntax
//@[002:0006) |   | | ├─IdentifierSyntax
//@[002:0006) |   | | | └─Token(Identifier) |name|
//@[006:0007) |   | | ├─Token(Colon) |:|
//@[008:0040) |   | | └─FunctionCallSyntax
//@[008:0012) |   | |   ├─IdentifierSyntax
//@[008:0012) |   | |   | └─Token(Identifier) |guid|
//@[012:0013) |   | |   ├─Token(LeftParen) |(|
//@[013:0026) |   | |   ├─FunctionArgumentSyntax
//@[013:0026) |   | |   | └─StringSyntax
//@[013:0026) |   | |   |   └─Token(StringComplete) |'contributor'|
//@[026:0027) |   | |   ├─Token(Comma) |,|
//@[028:0039) |   | |   ├─FunctionArgumentSyntax
//@[028:0039) |   | |   | └─VariableAccessSyntax
//@[028:0039) |   | |   |   └─IdentifierSyntax
//@[028:0039) |   | |   |     └─Token(Identifier) |contributor|
//@[039:0040) |   | |   └─Token(RightParen) |)|
//@[040:0042) |   | ├─Token(NewLine) |\r\n|
  properties: {
//@[002:0112) |   | ├─ObjectPropertySyntax
//@[002:0012) |   | | ├─IdentifierSyntax
//@[002:0012) |   | | | └─Token(Identifier) |properties|
//@[012:0013) |   | | ├─Token(Colon) |:|
//@[014:0112) |   | | └─ObjectSyntax
//@[014:0015) |   | |   ├─Token(LeftBrace) |{|
//@[015:0017) |   | |   ├─Token(NewLine) |\r\n|
    principalId: contributor
//@[004:0028) |   | |   ├─ObjectPropertySyntax
//@[004:0015) |   | |   | ├─IdentifierSyntax
//@[004:0015) |   | |   | | └─Token(Identifier) |principalId|
//@[015:0016) |   | |   | ├─Token(Colon) |:|
//@[017:0028) |   | |   | └─VariableAccessSyntax
//@[017:0028) |   | |   |   └─IdentifierSyntax
//@[017:0028) |   | |   |     └─Token(Identifier) |contributor|
//@[028:0030) |   | |   ├─Token(NewLine) |\r\n|
    roleDefinitionId: 'b24988ac-6180-42a0-ab88-20f7382dd24c'
//@[004:0060) |   | |   ├─ObjectPropertySyntax
//@[004:0020) |   | |   | ├─IdentifierSyntax
//@[004:0020) |   | |   | | └─Token(Identifier) |roleDefinitionId|
//@[020:0021) |   | |   | ├─Token(Colon) |:|
//@[022:0060) |   | |   | └─StringSyntax
//@[022:0060) |   | |   |   └─Token(StringComplete) |'b24988ac-6180-42a0-ab88-20f7382dd24c'|
//@[060:0062) |   | |   ├─Token(NewLine) |\r\n|
  }
//@[002:0003) |   | |   └─Token(RightBrace) |}|
//@[003:0005) |   | ├─Token(NewLine) |\r\n|
  dependsOn: [
//@[002:0030) |   | ├─ObjectPropertySyntax
//@[002:0011) |   | | ├─IdentifierSyntax
//@[002:0011) |   | | | └─Token(Identifier) |dependsOn|
//@[011:0012) |   | | ├─Token(Colon) |:|
//@[013:0030) |   | | └─ArraySyntax
//@[013:0014) |   | |   ├─Token(LeftSquare) |[|
//@[014:0016) |   | |   ├─Token(NewLine) |\r\n|
    owner
//@[004:0009) |   | |   ├─ArrayItemSyntax
//@[004:0009) |   | |   | └─VariableAccessSyntax
//@[004:0009) |   | |   |   └─IdentifierSyntax
//@[004:0009) |   | |   |     └─Token(Identifier) |owner|
//@[009:0011) |   | |   ├─Token(NewLine) |\r\n|
  ]
//@[002:0003) |   | |   └─Token(RightSquare) |]|
//@[003:0005) |   | ├─Token(NewLine) |\r\n|
}]
//@[000:0001) |   | └─Token(RightBrace) |}|
//@[001:0002) |   └─Token(RightSquare) |]|
//@[002:0006) ├─Token(NewLine) |\r\n\r\n|

resource readers 'Microsoft.Authorization/roleAssignments@2020-04-01-preview' = [for reader in readerPrincipals: {
//@[000:0312) ├─ResourceDeclarationSyntax
//@[000:0008) | ├─Token(Identifier) |resource|
//@[009:0016) | ├─IdentifierSyntax
//@[009:0016) | | └─Token(Identifier) |readers|
//@[017:0077) | ├─StringSyntax
//@[017:0077) | | └─Token(StringComplete) |'Microsoft.Authorization/roleAssignments@2020-04-01-preview'|
//@[078:0079) | ├─Token(Assignment) |=|
//@[080:0312) | └─ForSyntax
//@[080:0081) |   ├─Token(LeftSquare) |[|
//@[081:0084) |   ├─Token(Identifier) |for|
//@[085:0091) |   ├─LocalVariableSyntax
//@[085:0091) |   | └─IdentifierSyntax
//@[085:0091) |   |   └─Token(Identifier) |reader|
//@[092:0094) |   ├─Token(Identifier) |in|
//@[095:0111) |   ├─VariableAccessSyntax
//@[095:0111) |   | └─IdentifierSyntax
//@[095:0111) |   |   └─Token(Identifier) |readerPrincipals|
//@[111:0112) |   ├─Token(Colon) |:|
//@[113:0311) |   ├─ObjectSyntax
//@[113:0114) |   | ├─Token(LeftBrace) |{|
//@[114:0116) |   | ├─Token(NewLine) |\r\n|
  name: guid('reader', reader)
//@[002:0030) |   | ├─ObjectPropertySyntax
//@[002:0006) |   | | ├─IdentifierSyntax
//@[002:0006) |   | | | └─Token(Identifier) |name|
//@[006:0007) |   | | ├─Token(Colon) |:|
//@[008:0030) |   | | └─FunctionCallSyntax
//@[008:0012) |   | |   ├─IdentifierSyntax
//@[008:0012) |   | |   | └─Token(Identifier) |guid|
//@[012:0013) |   | |   ├─Token(LeftParen) |(|
//@[013:0021) |   | |   ├─FunctionArgumentSyntax
//@[013:0021) |   | |   | └─StringSyntax
//@[013:0021) |   | |   |   └─Token(StringComplete) |'reader'|
//@[021:0022) |   | |   ├─Token(Comma) |,|
//@[023:0029) |   | |   ├─FunctionArgumentSyntax
//@[023:0029) |   | |   | └─VariableAccessSyntax
//@[023:0029) |   | |   |   └─IdentifierSyntax
//@[023:0029) |   | |   |     └─Token(Identifier) |reader|
//@[029:0030) |   | |   └─Token(RightParen) |)|
//@[030:0032) |   | ├─Token(NewLine) |\r\n|
  properties: {
//@[002:0107) |   | ├─ObjectPropertySyntax
//@[002:0012) |   | | ├─IdentifierSyntax
//@[002:0012) |   | | | └─Token(Identifier) |properties|
//@[012:0013) |   | | ├─Token(Colon) |:|
//@[014:0107) |   | | └─ObjectSyntax
//@[014:0015) |   | |   ├─Token(LeftBrace) |{|
//@[015:0017) |   | |   ├─Token(NewLine) |\r\n|
    principalId: reader
//@[004:0023) |   | |   ├─ObjectPropertySyntax
//@[004:0015) |   | |   | ├─IdentifierSyntax
//@[004:0015) |   | |   | | └─Token(Identifier) |principalId|
//@[015:0016) |   | |   | ├─Token(Colon) |:|
//@[017:0023) |   | |   | └─VariableAccessSyntax
//@[017:0023) |   | |   |   └─IdentifierSyntax
//@[017:0023) |   | |   |     └─Token(Identifier) |reader|
//@[023:0025) |   | |   ├─Token(NewLine) |\r\n|
    roleDefinitionId: 'b24988ac-6180-42a0-ab88-20f7382dd24c'
//@[004:0060) |   | |   ├─ObjectPropertySyntax
//@[004:0020) |   | |   | ├─IdentifierSyntax
//@[004:0020) |   | |   | | └─Token(Identifier) |roleDefinitionId|
//@[020:0021) |   | |   | ├─Token(Colon) |:|
//@[022:0060) |   | |   | └─StringSyntax
//@[022:0060) |   | |   |   └─Token(StringComplete) |'b24988ac-6180-42a0-ab88-20f7382dd24c'|
//@[060:0062) |   | |   ├─Token(NewLine) |\r\n|
  }
//@[002:0003) |   | |   └─Token(RightBrace) |}|
//@[003:0005) |   | ├─Token(NewLine) |\r\n|
  dependsOn: [
//@[002:0051) |   | ├─ObjectPropertySyntax
//@[002:0011) |   | | ├─IdentifierSyntax
//@[002:0011) |   | | | └─Token(Identifier) |dependsOn|
//@[011:0012) |   | | ├─Token(Colon) |:|
//@[013:0051) |   | | └─ArraySyntax
//@[013:0014) |   | |   ├─Token(LeftSquare) |[|
//@[014:0016) |   | |   ├─Token(NewLine) |\r\n|
    owner
//@[004:0009) |   | |   ├─ArrayItemSyntax
//@[004:0009) |   | |   | └─VariableAccessSyntax
//@[004:0009) |   | |   |   └─IdentifierSyntax
//@[004:0009) |   | |   |     └─Token(Identifier) |owner|
//@[009:0011) |   | |   ├─Token(NewLine) |\r\n|
    contributors[0]
//@[004:0019) |   | |   ├─ArrayItemSyntax
//@[004:0019) |   | |   | └─ArrayAccessSyntax
//@[004:0016) |   | |   |   ├─VariableAccessSyntax
//@[004:0016) |   | |   |   | └─IdentifierSyntax
//@[004:0016) |   | |   |   |   └─Token(Identifier) |contributors|
//@[016:0017) |   | |   |   ├─Token(LeftSquare) |[|
//@[017:0018) |   | |   |   ├─IntegerLiteralSyntax
//@[017:0018) |   | |   |   | └─Token(Integer) |0|
//@[018:0019) |   | |   |   └─Token(RightSquare) |]|
//@[019:0021) |   | |   ├─Token(NewLine) |\r\n|
  ]
//@[002:0003) |   | |   └─Token(RightSquare) |]|
//@[003:0005) |   | ├─Token(NewLine) |\r\n|
}]
//@[000:0001) |   | └─Token(RightBrace) |}|
//@[001:0002) |   └─Token(RightSquare) |]|
//@[002:0006) ├─Token(NewLine) |\r\n\r\n|

resource single_mg 'Microsoft.Management/managementGroups@2020-05-01' = {
//@[000:0113) ├─ResourceDeclarationSyntax
//@[000:0008) | ├─Token(Identifier) |resource|
//@[009:0018) | ├─IdentifierSyntax
//@[009:0018) | | └─Token(Identifier) |single_mg|
//@[019:0069) | ├─StringSyntax
//@[019:0069) | | └─Token(StringComplete) |'Microsoft.Management/managementGroups@2020-05-01'|
//@[070:0071) | ├─Token(Assignment) |=|
//@[072:0113) | └─ObjectSyntax
//@[072:0073) |   ├─Token(LeftBrace) |{|
//@[073:0075) |   ├─Token(NewLine) |\r\n|
  scope: tenant()
//@[002:0017) |   ├─ObjectPropertySyntax
//@[002:0007) |   | ├─IdentifierSyntax
//@[002:0007) |   | | └─Token(Identifier) |scope|
//@[007:0008) |   | ├─Token(Colon) |:|
//@[009:0017) |   | └─FunctionCallSyntax
//@[009:0015) |   |   ├─IdentifierSyntax
//@[009:0015) |   |   | └─Token(Identifier) |tenant|
//@[015:0016) |   |   ├─Token(LeftParen) |(|
//@[016:0017) |   |   └─Token(RightParen) |)|
//@[017:0019) |   ├─Token(NewLine) |\r\n|
  name: 'one-mg'
//@[002:0016) |   ├─ObjectPropertySyntax
//@[002:0006) |   | ├─IdentifierSyntax
//@[002:0006) |   | | └─Token(Identifier) |name|
//@[006:0007) |   | ├─Token(Colon) |:|
//@[008:0016) |   | └─StringSyntax
//@[008:0016) |   |   └─Token(StringComplete) |'one-mg'|
//@[016:0018) |   ├─Token(NewLine) |\r\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0005) ├─Token(NewLine) |\r\n\r\n|

// Blueprints are read-only at tenant Scope, but it's a convenient example to use to validate this.
//@[099:0101) ├─Token(NewLine) |\r\n|
resource tenant_blueprint 'Microsoft.Blueprint/blueprints@2018-11-01-preview' = {
//@[000:0149) ├─ResourceDeclarationSyntax
//@[000:0008) | ├─Token(Identifier) |resource|
//@[009:0025) | ├─IdentifierSyntax
//@[009:0025) | | └─Token(Identifier) |tenant_blueprint|
//@[026:0077) | ├─StringSyntax
//@[026:0077) | | └─Token(StringComplete) |'Microsoft.Blueprint/blueprints@2018-11-01-preview'|
//@[078:0079) | ├─Token(Assignment) |=|
//@[080:0149) | └─ObjectSyntax
//@[080:0081) |   ├─Token(LeftBrace) |{|
//@[081:0083) |   ├─Token(NewLine) |\r\n|
  name: 'tenant-blueprint'
//@[002:0026) |   ├─ObjectPropertySyntax
//@[002:0006) |   | ├─IdentifierSyntax
//@[002:0006) |   | | └─Token(Identifier) |name|
//@[006:0007) |   | ├─Token(Colon) |:|
//@[008:0026) |   | └─StringSyntax
//@[008:0026) |   |   └─Token(StringComplete) |'tenant-blueprint'|
//@[026:0028) |   ├─Token(NewLine) |\r\n|
  properties: {}
//@[002:0016) |   ├─ObjectPropertySyntax
//@[002:0012) |   | ├─IdentifierSyntax
//@[002:0012) |   | | └─Token(Identifier) |properties|
//@[012:0013) |   | ├─Token(Colon) |:|
//@[014:0016) |   | └─ObjectSyntax
//@[014:0015) |   |   ├─Token(LeftBrace) |{|
//@[015:0016) |   |   └─Token(RightBrace) |}|
//@[016:0018) |   ├─Token(NewLine) |\r\n|
  scope: tenant()
//@[002:0017) |   ├─ObjectPropertySyntax
//@[002:0007) |   | ├─IdentifierSyntax
//@[002:0007) |   | | └─Token(Identifier) |scope|
//@[007:0008) |   | ├─Token(Colon) |:|
//@[009:0017) |   | └─FunctionCallSyntax
//@[009:0015) |   |   ├─IdentifierSyntax
//@[009:0015) |   |   | └─Token(Identifier) |tenant|
//@[015:0016) |   |   ├─Token(LeftParen) |(|
//@[016:0017) |   |   └─Token(RightParen) |)|
//@[017:0019) |   ├─Token(NewLine) |\r\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0005) ├─Token(NewLine) |\r\n\r\n|

resource mg_blueprint 'Microsoft.Blueprint/blueprints@2018-11-01-preview' = {
//@[000:0122) ├─ResourceDeclarationSyntax
//@[000:0008) | ├─Token(Identifier) |resource|
//@[009:0021) | ├─IdentifierSyntax
//@[009:0021) | | └─Token(Identifier) |mg_blueprint|
//@[022:0073) | ├─StringSyntax
//@[022:0073) | | └─Token(StringComplete) |'Microsoft.Blueprint/blueprints@2018-11-01-preview'|
//@[074:0075) | ├─Token(Assignment) |=|
//@[076:0122) | └─ObjectSyntax
//@[076:0077) |   ├─Token(LeftBrace) |{|
//@[077:0079) |   ├─Token(NewLine) |\r\n|
  name: 'mg-blueprint'
//@[002:0022) |   ├─ObjectPropertySyntax
//@[002:0006) |   | ├─IdentifierSyntax
//@[002:0006) |   | | └─Token(Identifier) |name|
//@[006:0007) |   | ├─Token(Colon) |:|
//@[008:0022) |   | └─StringSyntax
//@[008:0022) |   |   └─Token(StringComplete) |'mg-blueprint'|
//@[022:0024) |   ├─Token(NewLine) |\r\n|
  properties: {}
//@[002:0016) |   ├─ObjectPropertySyntax
//@[002:0012) |   | ├─IdentifierSyntax
//@[002:0012) |   | | └─Token(Identifier) |properties|
//@[012:0013) |   | ├─Token(Colon) |:|
//@[014:0016) |   | └─ObjectSyntax
//@[014:0015) |   |   ├─Token(LeftBrace) |{|
//@[015:0016) |   |   └─Token(RightBrace) |}|
//@[016:0018) |   ├─Token(NewLine) |\r\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0003) ├─Token(NewLine) |\r\n|

//@[000:0000) └─Token(EndOfFile) ||
