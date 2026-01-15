// BEGIN: Parameters
//@[000:4730) ProgramSyntax
//@[020:0024) ├─Token(NewLine) |\r\n\r\n|

param boolParam1 bool
//@[000:0021) ├─ParameterDeclarationSyntax
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0016) | ├─IdentifierSyntax
//@[006:0016) | | └─Token(Identifier) |boolParam1|
//@[017:0021) | └─TypeVariableAccessSyntax
//@[017:0021) |   └─IdentifierSyntax
//@[017:0021) |     └─Token(Identifier) |bool|
//@[021:0023) ├─Token(NewLine) |\r\n|
param strParam1 string
//@[000:0022) ├─ParameterDeclarationSyntax
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0015) | ├─IdentifierSyntax
//@[006:0015) | | └─Token(Identifier) |strParam1|
//@[016:0022) | └─TypeVariableAccessSyntax
//@[016:0022) |   └─IdentifierSyntax
//@[016:0022) |     └─Token(Identifier) |string|
//@[022:0024) ├─Token(NewLine) |\r\n|
param objParam1 object
//@[000:0022) ├─ParameterDeclarationSyntax
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0015) | ├─IdentifierSyntax
//@[006:0015) | | └─Token(Identifier) |objParam1|
//@[016:0022) | └─TypeVariableAccessSyntax
//@[016:0022) |   └─IdentifierSyntax
//@[016:0022) |     └─Token(Identifier) |object|
//@[022:0024) ├─Token(NewLine) |\r\n|
param invalidParamAssignment1 string = k8s.config.namespace
//@[000:0059) ├─ParameterDeclarationSyntax
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0029) | ├─IdentifierSyntax
//@[006:0029) | | └─Token(Identifier) |invalidParamAssignment1|
//@[030:0036) | ├─TypeVariableAccessSyntax
//@[030:0036) | | └─IdentifierSyntax
//@[030:0036) | |   └─Token(Identifier) |string|
//@[037:0059) | └─ParameterDefaultValueSyntax
//@[037:0038) |   ├─Token(Assignment) |=|
//@[039:0059) |   └─PropertyAccessSyntax
//@[039:0049) |     ├─PropertyAccessSyntax
//@[039:0042) |     | ├─VariableAccessSyntax
//@[039:0042) |     | | └─IdentifierSyntax
//@[039:0042) |     | |   └─Token(Identifier) |k8s|
//@[042:0043) |     | ├─Token(Dot) |.|
//@[043:0049) |     | └─IdentifierSyntax
//@[043:0049) |     |   └─Token(Identifier) |config|
//@[049:0050) |     ├─Token(Dot) |.|
//@[050:0059) |     └─IdentifierSyntax
//@[050:0059) |       └─Token(Identifier) |namespace|
//@[059:0063) ├─Token(NewLine) |\r\n\r\n|

// END: Parameters
//@[018:0022) ├─Token(NewLine) |\r\n\r\n|

// BEGIN: Valid extension declarations
//@[038:0042) ├─Token(NewLine) |\r\n\r\n|

extension az
//@[000:0012) ├─ExtensionDeclarationSyntax
//@[000:0009) | ├─Token(Identifier) |extension|
//@[010:0012) | ├─IdentifierSyntax
//@[010:0012) | | └─Token(Identifier) |az|
//@[012:0012) | ├─SkippedTriviaSyntax
//@[012:0012) | └─SkippedTriviaSyntax
//@[012:0014) ├─Token(NewLine) |\r\n|
extension kubernetes as k8s
//@[000:0027) ├─ExtensionDeclarationSyntax
//@[000:0009) | ├─Token(Identifier) |extension|
//@[010:0020) | ├─IdentifierSyntax
//@[010:0020) | | └─Token(Identifier) |kubernetes|
//@[021:0021) | ├─SkippedTriviaSyntax
//@[021:0027) | └─AliasAsClauseSyntax
//@[021:0023) |   ├─Token(Identifier) |as|
//@[024:0027) |   └─IdentifierSyntax
//@[024:0027) |     └─Token(Identifier) |k8s|
//@[027:0029) ├─Token(NewLine) |\r\n|
extension 'br:mcr.microsoft.com/bicep/extensions/hasoptionalconfig/v1:1.2.3' as extWithOptionalConfig1
//@[000:0102) ├─ExtensionDeclarationSyntax
//@[000:0009) | ├─Token(Identifier) |extension|
//@[010:0076) | ├─StringSyntax
//@[010:0076) | | └─Token(StringComplete) |'br:mcr.microsoft.com/bicep/extensions/hasoptionalconfig/v1:1.2.3'|
//@[077:0077) | ├─SkippedTriviaSyntax
//@[077:0102) | └─AliasAsClauseSyntax
//@[077:0079) |   ├─Token(Identifier) |as|
//@[080:0102) |   └─IdentifierSyntax
//@[080:0102) |     └─Token(Identifier) |extWithOptionalConfig1|
//@[102:0106) ├─Token(NewLine) |\r\n\r\n|

// END: Valid extension declarations
//@[036:0040) ├─Token(NewLine) |\r\n\r\n|

// BEGIN: Invalid extension declarations
//@[040:0044) ├─Token(NewLine) |\r\n\r\n|

extension 'br:mcr.microsoft.com/bicep/extensions/hasoptionalconfig/v1:1.2.3' with {
//@[000:0213) ├─ExtensionDeclarationSyntax
//@[000:0009) | ├─Token(Identifier) |extension|
//@[010:0076) | ├─StringSyntax
//@[010:0076) | | └─Token(StringComplete) |'br:mcr.microsoft.com/bicep/extensions/hasoptionalconfig/v1:1.2.3'|
//@[077:0194) | ├─ExtensionWithClauseSyntax
//@[077:0081) | | ├─Token(Identifier) |with|
//@[082:0194) | | └─ObjectSyntax
//@[082:0083) | |   ├─Token(LeftBrace) |{|
//@[083:0085) | |   ├─Token(NewLine) |\r\n|
  optionalString: testResource1.properties.ns // no reference calls, use module extension configs instead.
//@[002:0045) | |   ├─ObjectPropertySyntax
//@[002:0016) | |   | ├─IdentifierSyntax
//@[002:0016) | |   | | └─Token(Identifier) |optionalString|
//@[016:0017) | |   | ├─Token(Colon) |:|
//@[018:0045) | |   | └─PropertyAccessSyntax
//@[018:0042) | |   |   ├─PropertyAccessSyntax
//@[018:0031) | |   |   | ├─VariableAccessSyntax
//@[018:0031) | |   |   | | └─IdentifierSyntax
//@[018:0031) | |   |   | |   └─Token(Identifier) |testResource1|
//@[031:0032) | |   |   | ├─Token(Dot) |.|
//@[032:0042) | |   |   | └─IdentifierSyntax
//@[032:0042) | |   |   |   └─Token(Identifier) |properties|
//@[042:0043) | |   |   ├─Token(Dot) |.|
//@[043:0045) | |   |   └─IdentifierSyntax
//@[043:0045) | |   |     └─Token(Identifier) |ns|
//@[106:0108) | |   ├─Token(NewLine) |\r\n|
} as invalidExtDecl1
//@[000:0001) | |   └─Token(RightBrace) |}|
//@[002:0020) | └─AliasAsClauseSyntax
//@[002:0004) |   ├─Token(Identifier) |as|
//@[005:0020) |   └─IdentifierSyntax
//@[005:0020) |     └─Token(Identifier) |invalidExtDecl1|
//@[020:0024) ├─Token(NewLine) |\r\n\r\n|

extension 'br:mcr.microsoft.com/bicep/extensions/hasoptionalconfig/v1:1.2.3' with {
//@[000:0134) ├─ExtensionDeclarationSyntax
//@[000:0009) | ├─Token(Identifier) |extension|
//@[010:0076) | ├─StringSyntax
//@[010:0076) | | └─Token(StringComplete) |'br:mcr.microsoft.com/bicep/extensions/hasoptionalconfig/v1:1.2.3'|
//@[077:0115) | ├─ExtensionWithClauseSyntax
//@[077:0081) | | ├─Token(Identifier) |with|
//@[082:0115) | | └─ObjectSyntax
//@[082:0083) | |   ├─Token(LeftBrace) |{|
//@[083:0085) | |   ├─Token(NewLine) |\r\n|
  optionalString: newGuid()
//@[002:0027) | |   ├─ObjectPropertySyntax
//@[002:0016) | |   | ├─IdentifierSyntax
//@[002:0016) | |   | | └─Token(Identifier) |optionalString|
//@[016:0017) | |   | ├─Token(Colon) |:|
//@[018:0027) | |   | └─FunctionCallSyntax
//@[018:0025) | |   |   ├─IdentifierSyntax
//@[018:0025) | |   |   | └─Token(Identifier) |newGuid|
//@[025:0026) | |   |   ├─Token(LeftParen) |(|
//@[026:0027) | |   |   └─Token(RightParen) |)|
//@[027:0029) | |   ├─Token(NewLine) |\r\n|
} as invalidExtDecl2
//@[000:0001) | |   └─Token(RightBrace) |}|
//@[002:0020) | └─AliasAsClauseSyntax
//@[002:0004) |   ├─Token(Identifier) |as|
//@[005:0020) |   └─IdentifierSyntax
//@[005:0020) |     └─Token(Identifier) |invalidExtDecl2|
//@[020:0024) ├─Token(NewLine) |\r\n\r\n|

extension 'br:mcr.microsoft.com/bicep/extensions/hassecureconfig/v1:1.2.3' with {
//@[000:0149) ├─ExtensionDeclarationSyntax
//@[000:0009) | ├─Token(Identifier) |extension|
//@[010:0074) | ├─StringSyntax
//@[010:0074) | | └─Token(StringComplete) |'br:mcr.microsoft.com/bicep/extensions/hassecureconfig/v1:1.2.3'|
//@[075:0130) | ├─ExtensionWithClauseSyntax
//@[075:0079) | | ├─Token(Identifier) |with|
//@[080:0130) | | └─ObjectSyntax
//@[080:0081) | |   ├─Token(LeftBrace) |{|
//@[081:0083) | |   ├─Token(NewLine) |\r\n|
  requiredSecureString: kv1.getSecret('abc')
//@[002:0044) | |   ├─ObjectPropertySyntax
//@[002:0022) | |   | ├─IdentifierSyntax
//@[002:0022) | |   | | └─Token(Identifier) |requiredSecureString|
//@[022:0023) | |   | ├─Token(Colon) |:|
//@[024:0044) | |   | └─InstanceFunctionCallSyntax
//@[024:0027) | |   |   ├─VariableAccessSyntax
//@[024:0027) | |   |   | └─IdentifierSyntax
//@[024:0027) | |   |   |   └─Token(Identifier) |kv1|
//@[027:0028) | |   |   ├─Token(Dot) |.|
//@[028:0037) | |   |   ├─IdentifierSyntax
//@[028:0037) | |   |   | └─Token(Identifier) |getSecret|
//@[037:0038) | |   |   ├─Token(LeftParen) |(|
//@[038:0043) | |   |   ├─FunctionArgumentSyntax
//@[038:0043) | |   |   | └─StringSyntax
//@[038:0043) | |   |   |   └─Token(StringComplete) |'abc'|
//@[043:0044) | |   |   └─Token(RightParen) |)|
//@[044:0046) | |   ├─Token(NewLine) |\r\n|
} as invalidExtDecl3
//@[000:0001) | |   └─Token(RightBrace) |}|
//@[002:0020) | └─AliasAsClauseSyntax
//@[002:0004) |   ├─Token(Identifier) |as|
//@[005:0020) |   └─IdentifierSyntax
//@[005:0020) |     └─Token(Identifier) |invalidExtDecl3|
//@[020:0024) ├─Token(NewLine) |\r\n\r\n|

// END: Invalid extension declarations
//@[038:0042) ├─Token(NewLine) |\r\n\r\n|

// BEGIN: Key vaults
//@[020:0024) ├─Token(NewLine) |\r\n\r\n|

resource kv1 'Microsoft.KeyVault/vaults@2019-09-01' existing = {
//@[000:0082) ├─ResourceDeclarationSyntax
//@[000:0008) | ├─Token(Identifier) |resource|
//@[009:0012) | ├─IdentifierSyntax
//@[009:0012) | | └─Token(Identifier) |kv1|
//@[013:0051) | ├─StringSyntax
//@[013:0051) | | └─Token(StringComplete) |'Microsoft.KeyVault/vaults@2019-09-01'|
//@[052:0060) | ├─Token(Identifier) |existing|
//@[061:0062) | ├─Token(Assignment) |=|
//@[063:0082) | └─ObjectSyntax
//@[063:0064) |   ├─Token(LeftBrace) |{|
//@[064:0066) |   ├─Token(NewLine) |\r\n|
  name: 'kv1'
//@[002:0013) |   ├─ObjectPropertySyntax
//@[002:0006) |   | ├─IdentifierSyntax
//@[002:0006) |   | | └─Token(Identifier) |name|
//@[006:0007) |   | ├─Token(Colon) |:|
//@[008:0013) |   | └─StringSyntax
//@[008:0013) |   |   └─Token(StringComplete) |'kv1'|
//@[013:0015) |   ├─Token(NewLine) |\r\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0005) ├─Token(NewLine) |\r\n\r\n|

resource scopedKv1 'Microsoft.KeyVault/vaults@2019-09-01' existing = {
//@[000:0132) ├─ResourceDeclarationSyntax
//@[000:0008) | ├─Token(Identifier) |resource|
//@[009:0018) | ├─IdentifierSyntax
//@[009:0018) | | └─Token(Identifier) |scopedKv1|
//@[019:0057) | ├─StringSyntax
//@[019:0057) | | └─Token(StringComplete) |'Microsoft.KeyVault/vaults@2019-09-01'|
//@[058:0066) | ├─Token(Identifier) |existing|
//@[067:0068) | ├─Token(Assignment) |=|
//@[069:0132) | └─ObjectSyntax
//@[069:0070) |   ├─Token(LeftBrace) |{|
//@[070:0072) |   ├─Token(NewLine) |\r\n|
  name: 'scopedKv1'
//@[002:0019) |   ├─ObjectPropertySyntax
//@[002:0006) |   | ├─IdentifierSyntax
//@[002:0006) |   | | └─Token(Identifier) |name|
//@[006:0007) |   | ├─Token(Colon) |:|
//@[008:0019) |   | └─StringSyntax
//@[008:0019) |   |   └─Token(StringComplete) |'scopedKv1'|
//@[019:0021) |   ├─Token(NewLine) |\r\n|
  scope: resourceGroup('otherGroup')
//@[002:0036) |   ├─ObjectPropertySyntax
//@[002:0007) |   | ├─IdentifierSyntax
//@[002:0007) |   | | └─Token(Identifier) |scope|
//@[007:0008) |   | ├─Token(Colon) |:|
//@[009:0036) |   | └─FunctionCallSyntax
//@[009:0022) |   |   ├─IdentifierSyntax
//@[009:0022) |   |   | └─Token(Identifier) |resourceGroup|
//@[022:0023) |   |   ├─Token(LeftParen) |(|
//@[023:0035) |   |   ├─FunctionArgumentSyntax
//@[023:0035) |   |   | └─StringSyntax
//@[023:0035) |   |   |   └─Token(StringComplete) |'otherGroup'|
//@[035:0036) |   |   └─Token(RightParen) |)|
//@[036:0038) |   ├─Token(NewLine) |\r\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0005) ├─Token(NewLine) |\r\n\r\n|

// END: Key Vaults
//@[018:0022) ├─Token(NewLine) |\r\n\r\n|

// BEGIN: Resources
//@[019:0023) ├─Token(NewLine) |\r\n\r\n|

var configProp = 'config'
//@[000:0025) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0014) | ├─IdentifierSyntax
//@[004:0014) | | └─Token(Identifier) |configProp|
//@[015:0016) | ├─Token(Assignment) |=|
//@[017:0025) | └─StringSyntax
//@[017:0025) |   └─Token(StringComplete) |'config'|
//@[025:0029) ├─Token(NewLine) |\r\n\r\n|

// Extension symbols are blocked in resources because each config property returns an object { value, keyVaultReference } and "value" is not available when a reference is provided.
//@[180:0182) ├─Token(NewLine) |\r\n|
// Users should use deployment parameters for this scenario.
//@[060:0062) ├─Token(NewLine) |\r\n|
resource testResource1 'az:My.Rp/TestType@2020-01-01' = {
//@[000:0231) ├─ResourceDeclarationSyntax
//@[000:0008) | ├─Token(Identifier) |resource|
//@[009:0022) | ├─IdentifierSyntax
//@[009:0022) | | └─Token(Identifier) |testResource1|
//@[023:0053) | ├─StringSyntax
//@[023:0053) | | └─Token(StringComplete) |'az:My.Rp/TestType@2020-01-01'|
//@[054:0055) | ├─Token(Assignment) |=|
//@[056:0231) | └─ObjectSyntax
//@[056:0057) |   ├─Token(LeftBrace) |{|
//@[057:0059) |   ├─Token(NewLine) |\r\n|
  name: k8s.config.namespace
//@[002:0028) |   ├─ObjectPropertySyntax
//@[002:0006) |   | ├─IdentifierSyntax
//@[002:0006) |   | | └─Token(Identifier) |name|
//@[006:0007) |   | ├─Token(Colon) |:|
//@[008:0028) |   | └─PropertyAccessSyntax
//@[008:0018) |   |   ├─PropertyAccessSyntax
//@[008:0011) |   |   | ├─VariableAccessSyntax
//@[008:0011) |   |   | | └─IdentifierSyntax
//@[008:0011) |   |   | |   └─Token(Identifier) |k8s|
//@[011:0012) |   |   | ├─Token(Dot) |.|
//@[012:0018) |   |   | └─IdentifierSyntax
//@[012:0018) |   |   |   └─Token(Identifier) |config|
//@[018:0019) |   |   ├─Token(Dot) |.|
//@[019:0028) |   |   └─IdentifierSyntax
//@[019:0028) |   |     └─Token(Identifier) |namespace|
//@[028:0030) |   ├─Token(NewLine) |\r\n|
  properties: {
//@[002:0139) |   ├─ObjectPropertySyntax
//@[002:0012) |   | ├─IdentifierSyntax
//@[002:0012) |   | | └─Token(Identifier) |properties|
//@[012:0013) |   | ├─Token(Colon) |:|
//@[014:0139) |   | └─ObjectSyntax
//@[014:0015) |   |   ├─Token(LeftBrace) |{|
//@[015:0017) |   |   ├─Token(NewLine) |\r\n|
    secret: k8s.config.kubeConfig
//@[004:0033) |   |   ├─ObjectPropertySyntax
//@[004:0010) |   |   | ├─IdentifierSyntax
//@[004:0010) |   |   | | └─Token(Identifier) |secret|
//@[010:0011) |   |   | ├─Token(Colon) |:|
//@[012:0033) |   |   | └─PropertyAccessSyntax
//@[012:0022) |   |   |   ├─PropertyAccessSyntax
//@[012:0015) |   |   |   | ├─VariableAccessSyntax
//@[012:0015) |   |   |   | | └─IdentifierSyntax
//@[012:0015) |   |   |   | |   └─Token(Identifier) |k8s|
//@[015:0016) |   |   |   | ├─Token(Dot) |.|
//@[016:0022) |   |   |   | └─IdentifierSyntax
//@[016:0022) |   |   |   |   └─Token(Identifier) |config|
//@[022:0023) |   |   |   ├─Token(Dot) |.|
//@[023:0033) |   |   |   └─IdentifierSyntax
//@[023:0033) |   |   |     └─Token(Identifier) |kubeConfig|
//@[033:0035) |   |   ├─Token(NewLine) |\r\n|
    ns: k8s[configProp].namespace
//@[004:0033) |   |   ├─ObjectPropertySyntax
//@[004:0006) |   |   | ├─IdentifierSyntax
//@[004:0006) |   |   | | └─Token(Identifier) |ns|
//@[006:0007) |   |   | ├─Token(Colon) |:|
//@[008:0033) |   |   | └─PropertyAccessSyntax
//@[008:0023) |   |   |   ├─ArrayAccessSyntax
//@[008:0011) |   |   |   | ├─VariableAccessSyntax
//@[008:0011) |   |   |   | | └─IdentifierSyntax
//@[008:0011) |   |   |   | |   └─Token(Identifier) |k8s|
//@[011:0012) |   |   |   | ├─Token(LeftSquare) |[|
//@[012:0022) |   |   |   | ├─VariableAccessSyntax
//@[012:0022) |   |   |   | | └─IdentifierSyntax
//@[012:0022) |   |   |   | |   └─Token(Identifier) |configProp|
//@[022:0023) |   |   |   | └─Token(RightSquare) |]|
//@[023:0024) |   |   |   ├─Token(Dot) |.|
//@[024:0033) |   |   |   └─IdentifierSyntax
//@[024:0033) |   |   |     └─Token(Identifier) |namespace|
//@[033:0035) |   |   ├─Token(NewLine) |\r\n|
    ref: k8s[kv1.properties.sku.name].namespace
//@[004:0047) |   |   ├─ObjectPropertySyntax
//@[004:0007) |   |   | ├─IdentifierSyntax
//@[004:0007) |   |   | | └─Token(Identifier) |ref|
//@[007:0008) |   |   | ├─Token(Colon) |:|
//@[009:0047) |   |   | └─PropertyAccessSyntax
//@[009:0037) |   |   |   ├─ArrayAccessSyntax
//@[009:0012) |   |   |   | ├─VariableAccessSyntax
//@[009:0012) |   |   |   | | └─IdentifierSyntax
//@[009:0012) |   |   |   | |   └─Token(Identifier) |k8s|
//@[012:0013) |   |   |   | ├─Token(LeftSquare) |[|
//@[013:0036) |   |   |   | ├─PropertyAccessSyntax
//@[013:0031) |   |   |   | | ├─PropertyAccessSyntax
//@[013:0027) |   |   |   | | | ├─PropertyAccessSyntax
//@[013:0016) |   |   |   | | | | ├─VariableAccessSyntax
//@[013:0016) |   |   |   | | | | | └─IdentifierSyntax
//@[013:0016) |   |   |   | | | | |   └─Token(Identifier) |kv1|
//@[016:0017) |   |   |   | | | | ├─Token(Dot) |.|
//@[017:0027) |   |   |   | | | | └─IdentifierSyntax
//@[017:0027) |   |   |   | | | |   └─Token(Identifier) |properties|
//@[027:0028) |   |   |   | | | ├─Token(Dot) |.|
//@[028:0031) |   |   |   | | | └─IdentifierSyntax
//@[028:0031) |   |   |   | | |   └─Token(Identifier) |sku|
//@[031:0032) |   |   |   | | ├─Token(Dot) |.|
//@[032:0036) |   |   |   | | └─IdentifierSyntax
//@[032:0036) |   |   |   | |   └─Token(Identifier) |name|
//@[036:0037) |   |   |   | └─Token(RightSquare) |]|
//@[037:0038) |   |   |   ├─Token(Dot) |.|
//@[038:0047) |   |   |   └─IdentifierSyntax
//@[038:0047) |   |   |     └─Token(Identifier) |namespace|
//@[047:0049) |   |   ├─Token(NewLine) |\r\n|
  }
//@[002:0003) |   |   └─Token(RightBrace) |}|
//@[003:0005) |   ├─Token(NewLine) |\r\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0005) ├─Token(NewLine) |\r\n\r\n|

// END: Resources
//@[017:0021) ├─Token(NewLine) |\r\n\r\n|

// BEGIN: Extension configs for modules
//@[039:0043) ├─Token(NewLine) |\r\n\r\n|

module moduleWithExtsUsingFullInheritance 'child/hasConfigurableExtensionsWithAlias.bicep' = {
//@[000:0162) ├─ModuleDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |module|
//@[007:0041) | ├─IdentifierSyntax
//@[007:0041) | | └─Token(Identifier) |moduleWithExtsUsingFullInheritance|
//@[042:0090) | ├─StringSyntax
//@[042:0090) | | └─Token(StringComplete) |'child/hasConfigurableExtensionsWithAlias.bicep'|
//@[091:0092) | ├─Token(Assignment) |=|
//@[093:0162) | └─ObjectSyntax
//@[093:0094) |   ├─Token(LeftBrace) |{|
//@[094:0096) |   ├─Token(NewLine) |\r\n|
  extensionConfigs: {
//@[002:0063) |   ├─ObjectPropertySyntax
//@[002:0018) |   | ├─IdentifierSyntax
//@[002:0018) |   | | └─Token(Identifier) |extensionConfigs|
//@[018:0019) |   | ├─Token(Colon) |:|
//@[020:0063) |   | └─ObjectSyntax
//@[020:0021) |   |   ├─Token(LeftBrace) |{|
//@[021:0023) |   |   ├─Token(NewLine) |\r\n|
    k8s: k8s // must use k8s.config
//@[004:0012) |   |   ├─ObjectPropertySyntax
//@[004:0007) |   |   | ├─IdentifierSyntax
//@[004:0007) |   |   | | └─Token(Identifier) |k8s|
//@[007:0008) |   |   | ├─Token(Colon) |:|
//@[009:0012) |   |   | └─VariableAccessSyntax
//@[009:0012) |   |   |   └─IdentifierSyntax
//@[009:0012) |   |   |     └─Token(Identifier) |k8s|
//@[035:0037) |   |   ├─Token(NewLine) |\r\n|
  }
//@[002:0003) |   |   └─Token(RightBrace) |}|
//@[003:0005) |   ├─Token(NewLine) |\r\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0005) ├─Token(NewLine) |\r\n\r\n|

module moduleInvalidPropertyAccess 'child/hasConfigurableExtensionsWithAlias.bicep' = {
//@[000:0241) ├─ModuleDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |module|
//@[007:0034) | ├─IdentifierSyntax
//@[007:0034) | | └─Token(Identifier) |moduleInvalidPropertyAccess|
//@[035:0083) | ├─StringSyntax
//@[035:0083) | | └─Token(StringComplete) |'child/hasConfigurableExtensionsWithAlias.bicep'|
//@[084:0085) | ├─Token(Assignment) |=|
//@[086:0241) | └─ObjectSyntax
//@[086:0087) |   ├─Token(LeftBrace) |{|
//@[087:0089) |   ├─Token(NewLine) |\r\n|
  extensionConfigs: {
//@[002:0149) |   ├─ObjectPropertySyntax
//@[002:0018) |   | ├─IdentifierSyntax
//@[002:0018) |   | | └─Token(Identifier) |extensionConfigs|
//@[018:0019) |   | ├─Token(Colon) |:|
//@[020:0149) |   | └─ObjectSyntax
//@[020:0021) |   |   ├─Token(LeftBrace) |{|
//@[021:0023) |   |   ├─Token(NewLine) |\r\n|
    k8s: {
//@[004:0121) |   |   ├─ObjectPropertySyntax
//@[004:0007) |   |   | ├─IdentifierSyntax
//@[004:0007) |   |   | | └─Token(Identifier) |k8s|
//@[007:0008) |   |   | ├─Token(Colon) |:|
//@[009:0121) |   |   | └─ObjectSyntax
//@[009:0010) |   |   |   ├─Token(LeftBrace) |{|
//@[010:0012) |   |   |   ├─Token(NewLine) |\r\n|
      kubeConfig: k8s.config.kubeConfig.keyVaultReference
//@[006:0057) |   |   |   ├─ObjectPropertySyntax
//@[006:0016) |   |   |   | ├─IdentifierSyntax
//@[006:0016) |   |   |   | | └─Token(Identifier) |kubeConfig|
//@[016:0017) |   |   |   | ├─Token(Colon) |:|
//@[018:0057) |   |   |   | └─PropertyAccessSyntax
//@[018:0039) |   |   |   |   ├─PropertyAccessSyntax
//@[018:0028) |   |   |   |   | ├─PropertyAccessSyntax
//@[018:0021) |   |   |   |   | | ├─VariableAccessSyntax
//@[018:0021) |   |   |   |   | | | └─IdentifierSyntax
//@[018:0021) |   |   |   |   | | |   └─Token(Identifier) |k8s|
//@[021:0022) |   |   |   |   | | ├─Token(Dot) |.|
//@[022:0028) |   |   |   |   | | └─IdentifierSyntax
//@[022:0028) |   |   |   |   | |   └─Token(Identifier) |config|
//@[028:0029) |   |   |   |   | ├─Token(Dot) |.|
//@[029:0039) |   |   |   |   | └─IdentifierSyntax
//@[029:0039) |   |   |   |   |   └─Token(Identifier) |kubeConfig|
//@[039:0040) |   |   |   |   ├─Token(Dot) |.|
//@[040:0057) |   |   |   |   └─IdentifierSyntax
//@[040:0057) |   |   |   |     └─Token(Identifier) |keyVaultReference|
//@[057:0059) |   |   |   ├─Token(NewLine) |\r\n|
      namespace: k8s.config.namespace.value
//@[006:0043) |   |   |   ├─ObjectPropertySyntax
//@[006:0015) |   |   |   | ├─IdentifierSyntax
//@[006:0015) |   |   |   | | └─Token(Identifier) |namespace|
//@[015:0016) |   |   |   | ├─Token(Colon) |:|
//@[017:0043) |   |   |   | └─PropertyAccessSyntax
//@[017:0037) |   |   |   |   ├─PropertyAccessSyntax
//@[017:0027) |   |   |   |   | ├─PropertyAccessSyntax
//@[017:0020) |   |   |   |   | | ├─VariableAccessSyntax
//@[017:0020) |   |   |   |   | | | └─IdentifierSyntax
//@[017:0020) |   |   |   |   | | |   └─Token(Identifier) |k8s|
//@[020:0021) |   |   |   |   | | ├─Token(Dot) |.|
//@[021:0027) |   |   |   |   | | └─IdentifierSyntax
//@[021:0027) |   |   |   |   | |   └─Token(Identifier) |config|
//@[027:0028) |   |   |   |   | ├─Token(Dot) |.|
//@[028:0037) |   |   |   |   | └─IdentifierSyntax
//@[028:0037) |   |   |   |   |   └─Token(Identifier) |namespace|
//@[037:0038) |   |   |   |   ├─Token(Dot) |.|
//@[038:0043) |   |   |   |   └─IdentifierSyntax
//@[038:0043) |   |   |   |     └─Token(Identifier) |value|
//@[043:0045) |   |   |   ├─Token(NewLine) |\r\n|
    }
//@[004:0005) |   |   |   └─Token(RightBrace) |}|
//@[005:0007) |   |   ├─Token(NewLine) |\r\n|
  }
//@[002:0003) |   |   └─Token(RightBrace) |}|
//@[003:0005) |   ├─Token(NewLine) |\r\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0005) ├─Token(NewLine) |\r\n\r\n|

module moduleComplexKeyVaultReference 'child/hasConfigurableExtensionsWithAlias.bicep' = {
//@[000:0343) ├─ModuleDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |module|
//@[007:0037) | ├─IdentifierSyntax
//@[007:0037) | | └─Token(Identifier) |moduleComplexKeyVaultReference|
//@[038:0086) | ├─StringSyntax
//@[038:0086) | | └─Token(StringComplete) |'child/hasConfigurableExtensionsWithAlias.bicep'|
//@[087:0088) | ├─Token(Assignment) |=|
//@[089:0343) | └─ObjectSyntax
//@[089:0090) |   ├─Token(LeftBrace) |{|
//@[090:0092) |   ├─Token(NewLine) |\r\n|
  extensionConfigs: {
//@[002:0248) |   ├─ObjectPropertySyntax
//@[002:0018) |   | ├─IdentifierSyntax
//@[002:0018) |   | | └─Token(Identifier) |extensionConfigs|
//@[018:0019) |   | ├─Token(Colon) |:|
//@[020:0248) |   | └─ObjectSyntax
//@[020:0021) |   |   ├─Token(LeftBrace) |{|
//@[021:0023) |   |   ├─Token(NewLine) |\r\n|
    k8s: {
//@[004:0220) |   |   ├─ObjectPropertySyntax
//@[004:0007) |   |   | ├─IdentifierSyntax
//@[004:0007) |   |   | | └─Token(Identifier) |k8s|
//@[007:0008) |   |   | ├─Token(Colon) |:|
//@[009:0220) |   |   | └─ObjectSyntax
//@[009:0010) |   |   |   ├─Token(LeftBrace) |{|
//@[010:0012) |   |   |   ├─Token(NewLine) |\r\n|
      kubeConfig: boolParam1 ? kv1.getSecret('myKubeConfig') : scopedKv1.getSecret('myOtherKubeConfig')
//@[006:0103) |   |   |   ├─ObjectPropertySyntax
//@[006:0016) |   |   |   | ├─IdentifierSyntax
//@[006:0016) |   |   |   | | └─Token(Identifier) |kubeConfig|
//@[016:0017) |   |   |   | ├─Token(Colon) |:|
//@[018:0103) |   |   |   | └─TernaryOperationSyntax
//@[018:0028) |   |   |   |   ├─VariableAccessSyntax
//@[018:0028) |   |   |   |   | └─IdentifierSyntax
//@[018:0028) |   |   |   |   |   └─Token(Identifier) |boolParam1|
//@[029:0030) |   |   |   |   ├─Token(Question) |?|
//@[031:0060) |   |   |   |   ├─InstanceFunctionCallSyntax
//@[031:0034) |   |   |   |   | ├─VariableAccessSyntax
//@[031:0034) |   |   |   |   | | └─IdentifierSyntax
//@[031:0034) |   |   |   |   | |   └─Token(Identifier) |kv1|
//@[034:0035) |   |   |   |   | ├─Token(Dot) |.|
//@[035:0044) |   |   |   |   | ├─IdentifierSyntax
//@[035:0044) |   |   |   |   | | └─Token(Identifier) |getSecret|
//@[044:0045) |   |   |   |   | ├─Token(LeftParen) |(|
//@[045:0059) |   |   |   |   | ├─FunctionArgumentSyntax
//@[045:0059) |   |   |   |   | | └─StringSyntax
//@[045:0059) |   |   |   |   | |   └─Token(StringComplete) |'myKubeConfig'|
//@[059:0060) |   |   |   |   | └─Token(RightParen) |)|
//@[061:0062) |   |   |   |   ├─Token(Colon) |:|
//@[063:0103) |   |   |   |   └─InstanceFunctionCallSyntax
//@[063:0072) |   |   |   |     ├─VariableAccessSyntax
//@[063:0072) |   |   |   |     | └─IdentifierSyntax
//@[063:0072) |   |   |   |     |   └─Token(Identifier) |scopedKv1|
//@[072:0073) |   |   |   |     ├─Token(Dot) |.|
//@[073:0082) |   |   |   |     ├─IdentifierSyntax
//@[073:0082) |   |   |   |     | └─Token(Identifier) |getSecret|
//@[082:0083) |   |   |   |     ├─Token(LeftParen) |(|
//@[083:0102) |   |   |   |     ├─FunctionArgumentSyntax
//@[083:0102) |   |   |   |     | └─StringSyntax
//@[083:0102) |   |   |   |     |   └─Token(StringComplete) |'myOtherKubeConfig'|
//@[102:0103) |   |   |   |     └─Token(RightParen) |)|
//@[103:0105) |   |   |   ├─Token(NewLine) |\r\n|
      namespace: boolParam1 ? kv1.getSecret('myKubeConfig') : kv1.getSecret('myOtherKubeConfig')
//@[006:0096) |   |   |   ├─ObjectPropertySyntax
//@[006:0015) |   |   |   | ├─IdentifierSyntax
//@[006:0015) |   |   |   | | └─Token(Identifier) |namespace|
//@[015:0016) |   |   |   | ├─Token(Colon) |:|
//@[017:0096) |   |   |   | └─TernaryOperationSyntax
//@[017:0027) |   |   |   |   ├─VariableAccessSyntax
//@[017:0027) |   |   |   |   | └─IdentifierSyntax
//@[017:0027) |   |   |   |   |   └─Token(Identifier) |boolParam1|
//@[028:0029) |   |   |   |   ├─Token(Question) |?|
//@[030:0059) |   |   |   |   ├─InstanceFunctionCallSyntax
//@[030:0033) |   |   |   |   | ├─VariableAccessSyntax
//@[030:0033) |   |   |   |   | | └─IdentifierSyntax
//@[030:0033) |   |   |   |   | |   └─Token(Identifier) |kv1|
//@[033:0034) |   |   |   |   | ├─Token(Dot) |.|
//@[034:0043) |   |   |   |   | ├─IdentifierSyntax
//@[034:0043) |   |   |   |   | | └─Token(Identifier) |getSecret|
//@[043:0044) |   |   |   |   | ├─Token(LeftParen) |(|
//@[044:0058) |   |   |   |   | ├─FunctionArgumentSyntax
//@[044:0058) |   |   |   |   | | └─StringSyntax
//@[044:0058) |   |   |   |   | |   └─Token(StringComplete) |'myKubeConfig'|
//@[058:0059) |   |   |   |   | └─Token(RightParen) |)|
//@[060:0061) |   |   |   |   ├─Token(Colon) |:|
//@[062:0096) |   |   |   |   └─InstanceFunctionCallSyntax
//@[062:0065) |   |   |   |     ├─VariableAccessSyntax
//@[062:0065) |   |   |   |     | └─IdentifierSyntax
//@[062:0065) |   |   |   |     |   └─Token(Identifier) |kv1|
//@[065:0066) |   |   |   |     ├─Token(Dot) |.|
//@[066:0075) |   |   |   |     ├─IdentifierSyntax
//@[066:0075) |   |   |   |     | └─Token(Identifier) |getSecret|
//@[075:0076) |   |   |   |     ├─Token(LeftParen) |(|
//@[076:0095) |   |   |   |     ├─FunctionArgumentSyntax
//@[076:0095) |   |   |   |     | └─StringSyntax
//@[076:0095) |   |   |   |     |   └─Token(StringComplete) |'myOtherKubeConfig'|
//@[095:0096) |   |   |   |     └─Token(RightParen) |)|
//@[096:0098) |   |   |   ├─Token(NewLine) |\r\n|
    }
//@[004:0005) |   |   |   └─Token(RightBrace) |}|
//@[005:0007) |   |   ├─Token(NewLine) |\r\n|
  }
//@[002:0003) |   |   └─Token(RightBrace) |}|
//@[003:0005) |   ├─Token(NewLine) |\r\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0005) ├─Token(NewLine) |\r\n\r\n|

var invalidVarAssignment1 = k8s.config.namespace
//@[000:0048) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0025) | ├─IdentifierSyntax
//@[004:0025) | | └─Token(Identifier) |invalidVarAssignment1|
//@[026:0027) | ├─Token(Assignment) |=|
//@[028:0048) | └─PropertyAccessSyntax
//@[028:0038) |   ├─PropertyAccessSyntax
//@[028:0031) |   | ├─VariableAccessSyntax
//@[028:0031) |   | | └─IdentifierSyntax
//@[028:0031) |   | |   └─Token(Identifier) |k8s|
//@[031:0032) |   | ├─Token(Dot) |.|
//@[032:0038) |   | └─IdentifierSyntax
//@[032:0038) |   |   └─Token(Identifier) |config|
//@[038:0039) |   ├─Token(Dot) |.|
//@[039:0048) |   └─IdentifierSyntax
//@[039:0048) |     └─Token(Identifier) |namespace|
//@[048:0050) ├─Token(NewLine) |\r\n|
var invalidVarAssignment2 = k8s.config.kubeConfig
//@[000:0049) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0025) | ├─IdentifierSyntax
//@[004:0025) | | └─Token(Identifier) |invalidVarAssignment2|
//@[026:0027) | ├─Token(Assignment) |=|
//@[028:0049) | └─PropertyAccessSyntax
//@[028:0038) |   ├─PropertyAccessSyntax
//@[028:0031) |   | ├─VariableAccessSyntax
//@[028:0031) |   | | └─IdentifierSyntax
//@[028:0031) |   | |   └─Token(Identifier) |k8s|
//@[031:0032) |   | ├─Token(Dot) |.|
//@[032:0038) |   | └─IdentifierSyntax
//@[032:0038) |   |   └─Token(Identifier) |config|
//@[038:0039) |   ├─Token(Dot) |.|
//@[039:0049) |   └─IdentifierSyntax
//@[039:0049) |     └─Token(Identifier) |kubeConfig|
//@[049:0053) ├─Token(NewLine) |\r\n\r\n|

var extensionConfigsVar = {
//@[000:0099) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0023) | ├─IdentifierSyntax
//@[004:0023) | | └─Token(Identifier) |extensionConfigsVar|
//@[024:0025) | ├─Token(Assignment) |=|
//@[026:0099) | └─ObjectSyntax
//@[026:0027) |   ├─Token(LeftBrace) |{|
//@[027:0029) |   ├─Token(NewLine) |\r\n|
  k8s: {
//@[002:0067) |   ├─ObjectPropertySyntax
//@[002:0005) |   | ├─IdentifierSyntax
//@[002:0005) |   | | └─Token(Identifier) |k8s|
//@[005:0006) |   | ├─Token(Colon) |:|
//@[007:0067) |   | └─ObjectSyntax
//@[007:0008) |   |   ├─Token(LeftBrace) |{|
//@[008:0010) |   |   ├─Token(NewLine) |\r\n|
    kubeConfig: 'inlined',
//@[004:0025) |   |   ├─ObjectPropertySyntax
//@[004:0014) |   |   | ├─IdentifierSyntax
//@[004:0014) |   |   | | └─Token(Identifier) |kubeConfig|
//@[014:0015) |   |   | ├─Token(Colon) |:|
//@[016:0025) |   |   | └─StringSyntax
//@[016:0025) |   |   |   └─Token(StringComplete) |'inlined'|
//@[025:0026) |   |   ├─Token(Comma) |,|
//@[026:0026) |   |   ├─SkippedTriviaSyntax
//@[026:0028) |   |   ├─Token(NewLine) |\r\n|
    namespace: 'inlined'
//@[004:0024) |   |   ├─ObjectPropertySyntax
//@[004:0013) |   |   | ├─IdentifierSyntax
//@[004:0013) |   |   | | └─Token(Identifier) |namespace|
//@[013:0014) |   |   | ├─Token(Colon) |:|
//@[015:0024) |   |   | └─StringSyntax
//@[015:0024) |   |   |   └─Token(StringComplete) |'inlined'|
//@[024:0026) |   |   ├─Token(NewLine) |\r\n|
  }
//@[002:0003) |   |   └─Token(RightBrace) |}|
//@[003:0005) |   ├─Token(NewLine) |\r\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0005) ├─Token(NewLine) |\r\n\r\n|

var k8sConfigDeployTime = {
//@[000:0079) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0023) | ├─IdentifierSyntax
//@[004:0023) | | └─Token(Identifier) |k8sConfigDeployTime|
//@[024:0025) | ├─Token(Assignment) |=|
//@[026:0079) | └─ObjectSyntax
//@[026:0027) |   ├─Token(LeftBrace) |{|
//@[027:0029) |   ├─Token(NewLine) |\r\n|
  kubeConfig: strParam1
//@[002:0023) |   ├─ObjectPropertySyntax
//@[002:0012) |   | ├─IdentifierSyntax
//@[002:0012) |   | | └─Token(Identifier) |kubeConfig|
//@[012:0013) |   | ├─Token(Colon) |:|
//@[014:0023) |   | └─VariableAccessSyntax
//@[014:0023) |   |   └─IdentifierSyntax
//@[014:0023) |   |     └─Token(Identifier) |strParam1|
//@[023:0025) |   ├─Token(NewLine) |\r\n|
  namespace: strParam1
//@[002:0022) |   ├─ObjectPropertySyntax
//@[002:0011) |   | ├─IdentifierSyntax
//@[002:0011) |   | | └─Token(Identifier) |namespace|
//@[011:0012) |   | ├─Token(Colon) |:|
//@[013:0022) |   | └─VariableAccessSyntax
//@[013:0022) |   |   └─IdentifierSyntax
//@[013:0022) |   |     └─Token(Identifier) |strParam1|
//@[022:0024) |   ├─Token(NewLine) |\r\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0005) ├─Token(NewLine) |\r\n\r\n|

module moduleWithExtsUsingVar1 'child/hasConfigurableExtensionsWithAlias.bicep' = {
//@[000:0127) ├─ModuleDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |module|
//@[007:0030) | ├─IdentifierSyntax
//@[007:0030) | | └─Token(Identifier) |moduleWithExtsUsingVar1|
//@[031:0079) | ├─StringSyntax
//@[031:0079) | | └─Token(StringComplete) |'child/hasConfigurableExtensionsWithAlias.bicep'|
//@[080:0081) | ├─Token(Assignment) |=|
//@[082:0127) | └─ObjectSyntax
//@[082:0083) |   ├─Token(LeftBrace) |{|
//@[083:0085) |   ├─Token(NewLine) |\r\n|
  extensionConfigs: extensionConfigsVar
//@[002:0039) |   ├─ObjectPropertySyntax
//@[002:0018) |   | ├─IdentifierSyntax
//@[002:0018) |   | | └─Token(Identifier) |extensionConfigs|
//@[018:0019) |   | ├─Token(Colon) |:|
//@[020:0039) |   | └─VariableAccessSyntax
//@[020:0039) |   |   └─IdentifierSyntax
//@[020:0039) |   |     └─Token(Identifier) |extensionConfigsVar|
//@[039:0041) |   ├─Token(NewLine) |\r\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0005) ├─Token(NewLine) |\r\n\r\n|

module moduleWithExtsUsingVar2 'child/hasConfigurableExtensionsWithAlias.bicep' = {
//@[000:0144) ├─ModuleDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |module|
//@[007:0030) | ├─IdentifierSyntax
//@[007:0030) | | └─Token(Identifier) |moduleWithExtsUsingVar2|
//@[031:0079) | ├─StringSyntax
//@[031:0079) | | └─Token(StringComplete) |'child/hasConfigurableExtensionsWithAlias.bicep'|
//@[080:0081) | ├─Token(Assignment) |=|
//@[082:0144) | └─ObjectSyntax
//@[082:0083) |   ├─Token(LeftBrace) |{|
//@[083:0085) |   ├─Token(NewLine) |\r\n|
  extensionConfigs: {
//@[002:0056) |   ├─ObjectPropertySyntax
//@[002:0018) |   | ├─IdentifierSyntax
//@[002:0018) |   | | └─Token(Identifier) |extensionConfigs|
//@[018:0019) |   | ├─Token(Colon) |:|
//@[020:0056) |   | └─ObjectSyntax
//@[020:0021) |   |   ├─Token(LeftBrace) |{|
//@[021:0023) |   |   ├─Token(NewLine) |\r\n|
    k8s: k8sConfigDeployTime
//@[004:0028) |   |   ├─ObjectPropertySyntax
//@[004:0007) |   |   | ├─IdentifierSyntax
//@[004:0007) |   |   | | └─Token(Identifier) |k8s|
//@[007:0008) |   |   | ├─Token(Colon) |:|
//@[009:0028) |   |   | └─VariableAccessSyntax
//@[009:0028) |   |   |   └─IdentifierSyntax
//@[009:0028) |   |   |     └─Token(Identifier) |k8sConfigDeployTime|
//@[028:0030) |   |   ├─Token(NewLine) |\r\n|
  }
//@[002:0003) |   |   └─Token(RightBrace) |}|
//@[003:0005) |   ├─Token(NewLine) |\r\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0005) ├─Token(NewLine) |\r\n\r\n|

module moduleWithExtsUsingParam1 'child/hasConfigurableExtensionsWithAlias.bicep' = {
//@[000:0136) ├─ModuleDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |module|
//@[007:0032) | ├─IdentifierSyntax
//@[007:0032) | | └─Token(Identifier) |moduleWithExtsUsingParam1|
//@[033:0081) | ├─StringSyntax
//@[033:0081) | | └─Token(StringComplete) |'child/hasConfigurableExtensionsWithAlias.bicep'|
//@[082:0083) | ├─Token(Assignment) |=|
//@[084:0136) | └─ObjectSyntax
//@[084:0085) |   ├─Token(LeftBrace) |{|
//@[085:0087) |   ├─Token(NewLine) |\r\n|
  extensionConfigs: {
//@[002:0046) |   ├─ObjectPropertySyntax
//@[002:0018) |   | ├─IdentifierSyntax
//@[002:0018) |   | | └─Token(Identifier) |extensionConfigs|
//@[018:0019) |   | ├─Token(Colon) |:|
//@[020:0046) |   | └─ObjectSyntax
//@[020:0021) |   |   ├─Token(LeftBrace) |{|
//@[021:0023) |   |   ├─Token(NewLine) |\r\n|
    k8s: objParam1
//@[004:0018) |   |   ├─ObjectPropertySyntax
//@[004:0007) |   |   | ├─IdentifierSyntax
//@[004:0007) |   |   | | └─Token(Identifier) |k8s|
//@[007:0008) |   |   | ├─Token(Colon) |:|
//@[009:0018) |   |   | └─VariableAccessSyntax
//@[009:0018) |   |   |   └─IdentifierSyntax
//@[009:0018) |   |   |     └─Token(Identifier) |objParam1|
//@[018:0020) |   |   ├─Token(NewLine) |\r\n|
  }
//@[002:0003) |   |   └─Token(RightBrace) |}|
//@[003:0005) |   ├─Token(NewLine) |\r\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0005) ├─Token(NewLine) |\r\n\r\n|

module moduleWithExtsUsingReference1 'child/hasConfigurableExtensionsWithAlias.bicep' = {
//@[000:0155) ├─ModuleDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |module|
//@[007:0036) | ├─IdentifierSyntax
//@[007:0036) | | └─Token(Identifier) |moduleWithExtsUsingReference1|
//@[037:0085) | ├─StringSyntax
//@[037:0085) | | └─Token(StringComplete) |'child/hasConfigurableExtensionsWithAlias.bicep'|
//@[086:0087) | ├─Token(Assignment) |=|
//@[088:0155) | └─ObjectSyntax
//@[088:0089) |   ├─Token(LeftBrace) |{|
//@[089:0091) |   ├─Token(NewLine) |\r\n|
  extensionConfigs: {
//@[002:0061) |   ├─ObjectPropertySyntax
//@[002:0018) |   | ├─IdentifierSyntax
//@[002:0018) |   | | └─Token(Identifier) |extensionConfigs|
//@[018:0019) |   | ├─Token(Colon) |:|
//@[020:0061) |   | └─ObjectSyntax
//@[020:0021) |   |   ├─Token(LeftBrace) |{|
//@[021:0023) |   |   ├─Token(NewLine) |\r\n|
    k8s: testResource1.properties
//@[004:0033) |   |   ├─ObjectPropertySyntax
//@[004:0007) |   |   | ├─IdentifierSyntax
//@[004:0007) |   |   | | └─Token(Identifier) |k8s|
//@[007:0008) |   |   | ├─Token(Colon) |:|
//@[009:0033) |   |   | └─PropertyAccessSyntax
//@[009:0022) |   |   |   ├─VariableAccessSyntax
//@[009:0022) |   |   |   | └─IdentifierSyntax
//@[009:0022) |   |   |   |   └─Token(Identifier) |testResource1|
//@[022:0023) |   |   |   ├─Token(Dot) |.|
//@[023:0033) |   |   |   └─IdentifierSyntax
//@[023:0033) |   |   |     └─Token(Identifier) |properties|
//@[033:0035) |   |   ├─Token(NewLine) |\r\n|
  }
//@[002:0003) |   |   └─Token(RightBrace) |}|
//@[003:0005) |   ├─Token(NewLine) |\r\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0005) ├─Token(NewLine) |\r\n\r\n|

module moduleInvalidSpread1 'child/hasConfigurableExtensionsWithAlias.bicep' = {
//@[000:0139) ├─ModuleDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |module|
//@[007:0027) | ├─IdentifierSyntax
//@[007:0027) | | └─Token(Identifier) |moduleInvalidSpread1|
//@[028:0076) | ├─StringSyntax
//@[028:0076) | | └─Token(StringComplete) |'child/hasConfigurableExtensionsWithAlias.bicep'|
//@[077:0078) | ├─Token(Assignment) |=|
//@[079:0139) | └─ObjectSyntax
//@[079:0080) |   ├─Token(LeftBrace) |{|
//@[080:0082) |   ├─Token(NewLine) |\r\n|
  extensionConfigs: {
//@[002:0054) |   ├─ObjectPropertySyntax
//@[002:0018) |   | ├─IdentifierSyntax
//@[002:0018) |   | | └─Token(Identifier) |extensionConfigs|
//@[018:0019) |   | ├─Token(Colon) |:|
//@[020:0054) |   | └─ObjectSyntax
//@[020:0021) |   |   ├─Token(LeftBrace) |{|
//@[021:0023) |   |   ├─Token(NewLine) |\r\n|
    ...extensionConfigsVar
//@[004:0026) |   |   ├─SpreadExpressionSyntax
//@[004:0007) |   |   | ├─Token(Ellipsis) |...|
//@[007:0026) |   |   | └─VariableAccessSyntax
//@[007:0026) |   |   |   └─IdentifierSyntax
//@[007:0026) |   |   |     └─Token(Identifier) |extensionConfigsVar|
//@[026:0028) |   |   ├─Token(NewLine) |\r\n|
  }
//@[002:0003) |   |   └─Token(RightBrace) |}|
//@[003:0005) |   ├─Token(NewLine) |\r\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0005) ├─Token(NewLine) |\r\n\r\n|

module moduleInvalidSpread2 'child/hasConfigurableExtensionsWithAlias.bicep' = {
//@[000:0160) ├─ModuleDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |module|
//@[007:0027) | ├─IdentifierSyntax
//@[007:0027) | | └─Token(Identifier) |moduleInvalidSpread2|
//@[028:0076) | ├─StringSyntax
//@[028:0076) | | └─Token(StringComplete) |'child/hasConfigurableExtensionsWithAlias.bicep'|
//@[077:0078) | ├─Token(Assignment) |=|
//@[079:0160) | └─ObjectSyntax
//@[079:0080) |   ├─Token(LeftBrace) |{|
//@[080:0082) |   ├─Token(NewLine) |\r\n|
  extensionConfigs: {
//@[002:0075) |   ├─ObjectPropertySyntax
//@[002:0018) |   | ├─IdentifierSyntax
//@[002:0018) |   | | └─Token(Identifier) |extensionConfigs|
//@[018:0019) |   | ├─Token(Colon) |:|
//@[020:0075) |   | └─ObjectSyntax
//@[020:0021) |   |   ├─Token(LeftBrace) |{|
//@[021:0023) |   |   ├─Token(NewLine) |\r\n|
    k8s: {
//@[004:0047) |   |   ├─ObjectPropertySyntax
//@[004:0007) |   |   | ├─IdentifierSyntax
//@[004:0007) |   |   | | └─Token(Identifier) |k8s|
//@[007:0008) |   |   | ├─Token(Colon) |:|
//@[009:0047) |   |   | └─ObjectSyntax
//@[009:0010) |   |   |   ├─Token(LeftBrace) |{|
//@[010:0012) |   |   |   ├─Token(NewLine) |\r\n|
      ...k8sConfigDeployTime
//@[006:0028) |   |   |   ├─SpreadExpressionSyntax
//@[006:0009) |   |   |   | ├─Token(Ellipsis) |...|
//@[009:0028) |   |   |   | └─VariableAccessSyntax
//@[009:0028) |   |   |   |   └─IdentifierSyntax
//@[009:0028) |   |   |   |     └─Token(Identifier) |k8sConfigDeployTime|
//@[028:0030) |   |   |   ├─Token(NewLine) |\r\n|
    }
//@[004:0005) |   |   |   └─Token(RightBrace) |}|
//@[005:0007) |   |   ├─Token(NewLine) |\r\n|
  }
//@[002:0003) |   |   └─Token(RightBrace) |}|
//@[003:0005) |   ├─Token(NewLine) |\r\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0005) ├─Token(NewLine) |\r\n\r\n|

module moduleInvalidInheritanceTernary1 'child/hasConfigurableExtensionsWithAlias.bicep' = {
//@[000:0229) ├─ModuleDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |module|
//@[007:0039) | ├─IdentifierSyntax
//@[007:0039) | | └─Token(Identifier) |moduleInvalidInheritanceTernary1|
//@[040:0088) | ├─StringSyntax
//@[040:0088) | | └─Token(StringComplete) |'child/hasConfigurableExtensionsWithAlias.bicep'|
//@[089:0090) | ├─Token(Assignment) |=|
//@[091:0229) | └─ObjectSyntax
//@[091:0092) |   ├─Token(LeftBrace) |{|
//@[092:0094) |   ├─Token(NewLine) |\r\n|
  extensionConfigs: {
//@[002:0132) |   ├─ObjectPropertySyntax
//@[002:0018) |   | ├─IdentifierSyntax
//@[002:0018) |   | | └─Token(Identifier) |extensionConfigs|
//@[018:0019) |   | ├─Token(Colon) |:|
//@[020:0132) |   | └─ObjectSyntax
//@[020:0021) |   |   ├─Token(LeftBrace) |{|
//@[021:0023) |   |   ├─Token(NewLine) |\r\n|
    k8s: k8s.config
//@[004:0019) |   |   ├─ObjectPropertySyntax
//@[004:0007) |   |   | ├─IdentifierSyntax
//@[004:0007) |   |   | | └─Token(Identifier) |k8s|
//@[007:0008) |   |   | ├─Token(Colon) |:|
//@[009:0019) |   |   | └─PropertyAccessSyntax
//@[009:0012) |   |   |   ├─VariableAccessSyntax
//@[009:0012) |   |   |   | └─IdentifierSyntax
//@[009:0012) |   |   |   |   └─Token(Identifier) |k8s|
//@[012:0013) |   |   |   ├─Token(Dot) |.|
//@[013:0019) |   |   |   └─IdentifierSyntax
//@[013:0019) |   |   |     └─Token(Identifier) |config|
//@[019:0021) |   |   ├─Token(NewLine) |\r\n|
    extWithOptionalConfig1: boolParam1 ? extWithOptionalConfig1.config : k8s.config
//@[004:0083) |   |   ├─ObjectPropertySyntax
//@[004:0026) |   |   | ├─IdentifierSyntax
//@[004:0026) |   |   | | └─Token(Identifier) |extWithOptionalConfig1|
//@[026:0027) |   |   | ├─Token(Colon) |:|
//@[028:0083) |   |   | └─TernaryOperationSyntax
//@[028:0038) |   |   |   ├─VariableAccessSyntax
//@[028:0038) |   |   |   | └─IdentifierSyntax
//@[028:0038) |   |   |   |   └─Token(Identifier) |boolParam1|
//@[039:0040) |   |   |   ├─Token(Question) |?|
//@[041:0070) |   |   |   ├─PropertyAccessSyntax
//@[041:0063) |   |   |   | ├─VariableAccessSyntax
//@[041:0063) |   |   |   | | └─IdentifierSyntax
//@[041:0063) |   |   |   | |   └─Token(Identifier) |extWithOptionalConfig1|
//@[063:0064) |   |   |   | ├─Token(Dot) |.|
//@[064:0070) |   |   |   | └─IdentifierSyntax
//@[064:0070) |   |   |   |   └─Token(Identifier) |config|
//@[071:0072) |   |   |   ├─Token(Colon) |:|
//@[073:0083) |   |   |   └─PropertyAccessSyntax
//@[073:0076) |   |   |     ├─VariableAccessSyntax
//@[073:0076) |   |   |     | └─IdentifierSyntax
//@[073:0076) |   |   |     |   └─Token(Identifier) |k8s|
//@[076:0077) |   |   |     ├─Token(Dot) |.|
//@[077:0083) |   |   |     └─IdentifierSyntax
//@[077:0083) |   |   |       └─Token(Identifier) |config|
//@[083:0085) |   |   ├─Token(NewLine) |\r\n|
  }
//@[002:0003) |   |   └─Token(RightBrace) |}|
//@[003:0005) |   ├─Token(NewLine) |\r\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0005) ├─Token(NewLine) |\r\n\r\n|

module moduleInvalidInheritanceTernary2 'child/hasConfigurableExtensionsWithAlias.bicep' = {
//@[000:0339) ├─ModuleDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |module|
//@[007:0039) | ├─IdentifierSyntax
//@[007:0039) | | └─Token(Identifier) |moduleInvalidInheritanceTernary2|
//@[040:0088) | ├─StringSyntax
//@[040:0088) | | └─Token(StringComplete) |'child/hasConfigurableExtensionsWithAlias.bicep'|
//@[089:0090) | ├─Token(Assignment) |=|
//@[091:0339) | └─ObjectSyntax
//@[091:0092) |   ├─Token(LeftBrace) |{|
//@[092:0094) |   ├─Token(NewLine) |\r\n|
  extensionConfigs: {
//@[002:0242) |   ├─ObjectPropertySyntax
//@[002:0018) |   | ├─IdentifierSyntax
//@[002:0018) |   | | └─Token(Identifier) |extensionConfigs|
//@[018:0019) |   | ├─Token(Colon) |:|
//@[020:0242) |   | └─ObjectSyntax
//@[020:0021) |   |   ├─Token(LeftBrace) |{|
//@[021:0023) |   |   ├─Token(NewLine) |\r\n|
    k8s: k8s.config
//@[004:0019) |   |   ├─ObjectPropertySyntax
//@[004:0007) |   |   | ├─IdentifierSyntax
//@[004:0007) |   |   | | └─Token(Identifier) |k8s|
//@[007:0008) |   |   | ├─Token(Colon) |:|
//@[009:0019) |   |   | └─PropertyAccessSyntax
//@[009:0012) |   |   |   ├─VariableAccessSyntax
//@[009:0012) |   |   |   | └─IdentifierSyntax
//@[009:0012) |   |   |   |   └─Token(Identifier) |k8s|
//@[012:0013) |   |   |   ├─Token(Dot) |.|
//@[013:0019) |   |   |   └─IdentifierSyntax
//@[013:0019) |   |   |     └─Token(Identifier) |config|
//@[019:0021) |   |   ├─Token(NewLine) |\r\n|
    extWithOptionalConfig1: boolParam1 ? extWithOptionalConfig1.config : { optionalString: 'value' } // limitation: cannot mix these currently due to special code gen needed for object literals
//@[004:0100) |   |   ├─ObjectPropertySyntax
//@[004:0026) |   |   | ├─IdentifierSyntax
//@[004:0026) |   |   | | └─Token(Identifier) |extWithOptionalConfig1|
//@[026:0027) |   |   | ├─Token(Colon) |:|
//@[028:0100) |   |   | └─TernaryOperationSyntax
//@[028:0038) |   |   |   ├─VariableAccessSyntax
//@[028:0038) |   |   |   | └─IdentifierSyntax
//@[028:0038) |   |   |   |   └─Token(Identifier) |boolParam1|
//@[039:0040) |   |   |   ├─Token(Question) |?|
//@[041:0070) |   |   |   ├─PropertyAccessSyntax
//@[041:0063) |   |   |   | ├─VariableAccessSyntax
//@[041:0063) |   |   |   | | └─IdentifierSyntax
//@[041:0063) |   |   |   | |   └─Token(Identifier) |extWithOptionalConfig1|
//@[063:0064) |   |   |   | ├─Token(Dot) |.|
//@[064:0070) |   |   |   | └─IdentifierSyntax
//@[064:0070) |   |   |   |   └─Token(Identifier) |config|
//@[071:0072) |   |   |   ├─Token(Colon) |:|
//@[073:0100) |   |   |   └─ObjectSyntax
//@[073:0074) |   |   |     ├─Token(LeftBrace) |{|
//@[075:0098) |   |   |     ├─ObjectPropertySyntax
//@[075:0089) |   |   |     | ├─IdentifierSyntax
//@[075:0089) |   |   |     | | └─Token(Identifier) |optionalString|
//@[089:0090) |   |   |     | ├─Token(Colon) |:|
//@[091:0098) |   |   |     | └─StringSyntax
//@[091:0098) |   |   |     |   └─Token(StringComplete) |'value'|
//@[099:0100) |   |   |     └─Token(RightBrace) |}|
//@[193:0195) |   |   ├─Token(NewLine) |\r\n|
  }
//@[002:0003) |   |   └─Token(RightBrace) |}|
//@[003:0005) |   ├─Token(NewLine) |\r\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0005) ├─Token(NewLine) |\r\n\r\n|

// END: Extension configs for modules
//@[037:0041) ├─Token(NewLine) |\r\n\r\n|

// BEGIN: Outputs
//@[017:0021) ├─Token(NewLine) |\r\n\r\n|

// Extension symbols are blocked for outputs for now. Users should use deployment parameters for this scenario.
//@[111:0115) ├─Token(NewLine) |\r\n\r\n|

output k8sTheNamespace object = k8s // This is a namespace type
//@[000:0035) ├─OutputDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |output|
//@[007:0022) | ├─IdentifierSyntax
//@[007:0022) | | └─Token(Identifier) |k8sTheNamespace|
//@[023:0029) | ├─TypeVariableAccessSyntax
//@[023:0029) | | └─IdentifierSyntax
//@[023:0029) | |   └─Token(Identifier) |object|
//@[030:0031) | ├─Token(Assignment) |=|
//@[032:0035) | └─VariableAccessSyntax
//@[032:0035) |   └─IdentifierSyntax
//@[032:0035) |     └─Token(Identifier) |k8s|
//@[063:0067) ├─Token(NewLine) |\r\n\r\n|

output k8sConfig object = k8s.config
//@[000:0036) ├─OutputDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |output|
//@[007:0016) | ├─IdentifierSyntax
//@[007:0016) | | └─Token(Identifier) |k8sConfig|
//@[017:0023) | ├─TypeVariableAccessSyntax
//@[017:0023) | | └─IdentifierSyntax
//@[017:0023) | |   └─Token(Identifier) |object|
//@[024:0025) | ├─Token(Assignment) |=|
//@[026:0036) | └─PropertyAccessSyntax
//@[026:0029) |   ├─VariableAccessSyntax
//@[026:0029) |   | └─IdentifierSyntax
//@[026:0029) |   |   └─Token(Identifier) |k8s|
//@[029:0030) |   ├─Token(Dot) |.|
//@[030:0036) |   └─IdentifierSyntax
//@[030:0036) |     └─Token(Identifier) |config|
//@[036:0040) ├─Token(NewLine) |\r\n\r\n|

output k8sNamespace string = k8s.config.namespace
//@[000:0049) ├─OutputDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |output|
//@[007:0019) | ├─IdentifierSyntax
//@[007:0019) | | └─Token(Identifier) |k8sNamespace|
//@[020:0026) | ├─TypeVariableAccessSyntax
//@[020:0026) | | └─IdentifierSyntax
//@[020:0026) | |   └─Token(Identifier) |string|
//@[027:0028) | ├─Token(Assignment) |=|
//@[029:0049) | └─PropertyAccessSyntax
//@[029:0039) |   ├─PropertyAccessSyntax
//@[029:0032) |   | ├─VariableAccessSyntax
//@[029:0032) |   | | └─IdentifierSyntax
//@[029:0032) |   | |   └─Token(Identifier) |k8s|
//@[032:0033) |   | ├─Token(Dot) |.|
//@[033:0039) |   | └─IdentifierSyntax
//@[033:0039) |   |   └─Token(Identifier) |config|
//@[039:0040) |   ├─Token(Dot) |.|
//@[040:0049) |   └─IdentifierSyntax
//@[040:0049) |     └─Token(Identifier) |namespace|
//@[049:0053) ├─Token(NewLine) |\r\n\r\n|

// END: Outputs
//@[015:0017) ├─Token(NewLine) |\r\n|

//@[000:0000) └─Token(EndOfFile) ||
