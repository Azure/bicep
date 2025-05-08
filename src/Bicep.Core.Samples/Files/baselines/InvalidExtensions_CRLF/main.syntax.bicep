// BEGIN: Parameters
//@[000:2822) ProgramSyntax
//@[020:0024) ├─Token(NewLine) |\r\n\r\n|

param boolParam1 bool
//@[000:0021) ├─ParameterDeclarationSyntax
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0016) | ├─IdentifierSyntax
//@[006:0016) | | └─Token(Identifier) |boolParam1|
//@[017:0021) | └─TypeVariableAccessSyntax
//@[017:0021) |   └─IdentifierSyntax
//@[017:0021) |     └─Token(Identifier) |bool|
//@[021:0025) ├─Token(NewLine) |\r\n\r\n|

// END: Parameters
//@[018:0022) ├─Token(NewLine) |\r\n\r\n|

// BEGIN: Valid Extension declarations
//@[038:0042) ├─Token(NewLine) |\r\n\r\n|

extension az
//@[000:0012) ├─ExtensionDeclarationSyntax
//@[000:0009) | ├─Token(Identifier) |extension|
//@[010:0012) | ├─IdentifierSyntax
//@[010:0012) | | └─Token(Identifier) |az|
//@[012:0012) | ├─SkippedTriviaSyntax
//@[012:0012) | └─SkippedTriviaSyntax
//@[012:0014) ├─Token(NewLine) |\r\n|
extension kubernetes with {
//@[000:0084) ├─ExtensionDeclarationSyntax
//@[000:0009) | ├─Token(Identifier) |extension|
//@[010:0020) | ├─IdentifierSyntax
//@[010:0020) | | └─Token(Identifier) |kubernetes|
//@[021:0077) | ├─ExtensionWithClauseSyntax
//@[021:0025) | | ├─Token(Identifier) |with|
//@[026:0077) | | └─ObjectSyntax
//@[026:0027) | |   ├─Token(LeftBrace) |{|
//@[027:0029) | |   ├─Token(NewLine) |\r\n|
  kubeConfig: 'DELETE'
//@[002:0022) | |   ├─ObjectPropertySyntax
//@[002:0012) | |   | ├─IdentifierSyntax
//@[002:0012) | |   | | └─Token(Identifier) |kubeConfig|
//@[012:0013) | |   | ├─Token(Colon) |:|
//@[014:0022) | |   | └─StringSyntax
//@[014:0022) | |   |   └─Token(StringComplete) |'DELETE'|
//@[022:0024) | |   ├─Token(NewLine) |\r\n|
  namespace: 'DELETE'
//@[002:0021) | |   ├─ObjectPropertySyntax
//@[002:0011) | |   | ├─IdentifierSyntax
//@[002:0011) | |   | | └─Token(Identifier) |namespace|
//@[011:0012) | |   | ├─Token(Colon) |:|
//@[013:0021) | |   | └─StringSyntax
//@[013:0021) | |   |   └─Token(StringComplete) |'DELETE'|
//@[021:0023) | |   ├─Token(NewLine) |\r\n|
} as k8s
//@[000:0001) | |   └─Token(RightBrace) |}|
//@[002:0008) | └─AliasAsClauseSyntax
//@[002:0004) |   ├─Token(Identifier) |as|
//@[005:0008) |   └─IdentifierSyntax
//@[005:0008) |     └─Token(Identifier) |k8s|
//@[008:0012) ├─Token(NewLine) |\r\n\r\n|

//extension 'br:mcr.microsoft.com/bicep/extensions/microsoftgraph/v1.0:0.1.8-preview' as graph
//@[094:0098) ├─Token(NewLine) |\r\n\r\n|

// END: Valid Extension declarations
//@[036:0040) ├─Token(NewLine) |\r\n\r\n|

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
//@[000:0203) ├─ModuleDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |module|
//@[007:0041) | ├─IdentifierSyntax
//@[007:0041) | | └─Token(Identifier) |moduleWithExtsUsingFullInheritance|
//@[042:0090) | ├─StringSyntax
//@[042:0090) | | └─Token(StringComplete) |'child/hasConfigurableExtensionsWithAlias.bicep'|
//@[091:0092) | ├─Token(Assignment) |=|
//@[093:0203) | └─ObjectSyntax
//@[093:0094) |   ├─Token(LeftBrace) |{|
//@[094:0096) |   ├─Token(NewLine) |\r\n|
  name: 'moduleWithExtsFullInheritance'
//@[002:0039) |   ├─ObjectPropertySyntax
//@[002:0006) |   | ├─IdentifierSyntax
//@[002:0006) |   | | └─Token(Identifier) |name|
//@[006:0007) |   | ├─Token(Colon) |:|
//@[008:0039) |   | └─StringSyntax
//@[008:0039) |   |   └─Token(StringComplete) |'moduleWithExtsFullInheritance'|
//@[039:0041) |   ├─Token(NewLine) |\r\n|
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
//@[000:0280) ├─ModuleDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |module|
//@[007:0034) | ├─IdentifierSyntax
//@[007:0034) | | └─Token(Identifier) |moduleInvalidPropertyAccess|
//@[035:0083) | ├─StringSyntax
//@[035:0083) | | └─Token(StringComplete) |'child/hasConfigurableExtensionsWithAlias.bicep'|
//@[084:0085) | ├─Token(Assignment) |=|
//@[086:0280) | └─ObjectSyntax
//@[086:0087) |   ├─Token(LeftBrace) |{|
//@[087:0089) |   ├─Token(NewLine) |\r\n|
  name: 'moduleInvalidPropertyAccess'
//@[002:0037) |   ├─ObjectPropertySyntax
//@[002:0006) |   | ├─IdentifierSyntax
//@[002:0006) |   | | └─Token(Identifier) |name|
//@[006:0007) |   | ├─Token(Colon) |:|
//@[008:0037) |   | └─StringSyntax
//@[008:0037) |   |   └─Token(StringComplete) |'moduleInvalidPropertyAccess'|
//@[037:0039) |   ├─Token(NewLine) |\r\n|
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
//@[000:0385) ├─ModuleDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |module|
//@[007:0037) | ├─IdentifierSyntax
//@[007:0037) | | └─Token(Identifier) |moduleComplexKeyVaultReference|
//@[038:0086) | ├─StringSyntax
//@[038:0086) | | └─Token(StringComplete) |'child/hasConfigurableExtensionsWithAlias.bicep'|
//@[087:0088) | ├─Token(Assignment) |=|
//@[089:0385) | └─ObjectSyntax
//@[089:0090) |   ├─Token(LeftBrace) |{|
//@[090:0092) |   ├─Token(NewLine) |\r\n|
  name: 'moduleComplexKeyVaultReference'
//@[002:0040) |   ├─ObjectPropertySyntax
//@[002:0006) |   | ├─IdentifierSyntax
//@[002:0006) |   | | └─Token(Identifier) |name|
//@[006:0007) |   | ├─Token(Colon) |:|
//@[008:0040) |   | └─StringSyntax
//@[008:0040) |   |   └─Token(StringComplete) |'moduleComplexKeyVaultReference'|
//@[040:0042) |   ├─Token(NewLine) |\r\n|
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

// TODO(kylealbert): Figure out if this can be made allowable easily, potentially by inlining.
//@[094:0096) ├─Token(NewLine) |\r\n|
var k8sConfigDeployTime = {
//@[000:0091) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0023) | ├─IdentifierSyntax
//@[004:0023) | | └─Token(Identifier) |k8sConfigDeployTime|
//@[024:0025) | ├─Token(Assignment) |=|
//@[026:0091) | └─ObjectSyntax
//@[026:0027) |   ├─Token(LeftBrace) |{|
//@[027:0029) |   ├─Token(NewLine) |\r\n|
  kubeConfig: k8s.config.kubeConfig
//@[002:0035) |   ├─ObjectPropertySyntax
//@[002:0012) |   | ├─IdentifierSyntax
//@[002:0012) |   | | └─Token(Identifier) |kubeConfig|
//@[012:0013) |   | ├─Token(Colon) |:|
//@[014:0035) |   | └─PropertyAccessSyntax
//@[014:0024) |   |   ├─PropertyAccessSyntax
//@[014:0017) |   |   | ├─VariableAccessSyntax
//@[014:0017) |   |   | | └─IdentifierSyntax
//@[014:0017) |   |   | |   └─Token(Identifier) |k8s|
//@[017:0018) |   |   | ├─Token(Dot) |.|
//@[018:0024) |   |   | └─IdentifierSyntax
//@[018:0024) |   |   |   └─Token(Identifier) |config|
//@[024:0025) |   |   ├─Token(Dot) |.|
//@[025:0035) |   |   └─IdentifierSyntax
//@[025:0035) |   |     └─Token(Identifier) |kubeConfig|
//@[035:0037) |   ├─Token(NewLine) |\r\n|
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

module moduleWithExtsUsingVar 'child/hasConfigurableExtensionsWithAlias.bicep' = {
//@[000:0177) ├─ModuleDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |module|
//@[007:0029) | ├─IdentifierSyntax
//@[007:0029) | | └─Token(Identifier) |moduleWithExtsUsingVar|
//@[030:0078) | ├─StringSyntax
//@[030:0078) | | └─Token(StringComplete) |'child/hasConfigurableExtensionsWithAlias.bicep'|
//@[079:0080) | ├─Token(Assignment) |=|
//@[081:0177) | └─ObjectSyntax
//@[081:0082) |   ├─Token(LeftBrace) |{|
//@[082:0084) |   ├─Token(NewLine) |\r\n|
  name: 'moduleWithExtsUsingVar'
//@[002:0032) |   ├─ObjectPropertySyntax
//@[002:0006) |   | ├─IdentifierSyntax
//@[002:0006) |   | | └─Token(Identifier) |name|
//@[006:0007) |   | ├─Token(Colon) |:|
//@[008:0032) |   | └─StringSyntax
//@[008:0032) |   |   └─Token(StringComplete) |'moduleWithExtsUsingVar'|
//@[032:0034) |   ├─Token(NewLine) |\r\n|
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
