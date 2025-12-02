// BEGIN: Parameters
//@[000:4364) ProgramSyntax
//@[020:0024) ├─Token(NewLine) |\r\n\r\n|

param strParam1 string
//@[000:0022) ├─ParameterDeclarationSyntax
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0015) | ├─IdentifierSyntax
//@[006:0015) | | └─Token(Identifier) |strParam1|
//@[016:0022) | └─TypeVariableAccessSyntax
//@[016:0022) |   └─IdentifierSyntax
//@[016:0022) |     └─Token(Identifier) |string|
//@[022:0026) ├─Token(NewLine) |\r\n\r\n|

@secure()
//@[000:0039) ├─ParameterDeclarationSyntax
//@[000:0009) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0009) | | └─FunctionCallSyntax
//@[001:0007) | |   ├─IdentifierSyntax
//@[001:0007) | |   | └─Token(Identifier) |secure|
//@[007:0008) | |   ├─Token(LeftParen) |(|
//@[008:0009) | |   └─Token(RightParen) |)|
//@[009:0011) | ├─Token(NewLine) |\r\n|
param secureStrParam1 string
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0021) | ├─IdentifierSyntax
//@[006:0021) | | └─Token(Identifier) |secureStrParam1|
//@[022:0028) | └─TypeVariableAccessSyntax
//@[022:0028) |   └─IdentifierSyntax
//@[022:0028) |     └─Token(Identifier) |string|
//@[028:0032) ├─Token(NewLine) |\r\n\r\n|

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

// BEGIN: Variables
//@[019:0023) ├─Token(NewLine) |\r\n\r\n|

var strVar1 = 'strVar1Value'
//@[000:0028) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0011) | ├─IdentifierSyntax
//@[004:0011) | | └─Token(Identifier) |strVar1|
//@[012:0013) | ├─Token(Assignment) |=|
//@[014:0028) | └─StringSyntax
//@[014:0028) |   └─Token(StringComplete) |'strVar1Value'|
//@[028:0030) ├─Token(NewLine) |\r\n|
var strParamVar1 = strParam1
//@[000:0028) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0016) | ├─IdentifierSyntax
//@[004:0016) | | └─Token(Identifier) |strParamVar1|
//@[017:0018) | ├─Token(Assignment) |=|
//@[019:0028) | └─VariableAccessSyntax
//@[019:0028) |   └─IdentifierSyntax
//@[019:0028) |     └─Token(Identifier) |strParam1|
//@[028:0032) ├─Token(NewLine) |\r\n\r\n|

// END: Variables
//@[017:0021) ├─Token(NewLine) |\r\n\r\n|

// BEGIN: Extension declarations
//@[032:0036) ├─Token(NewLine) |\r\n\r\n|

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
//@[102:0104) ├─Token(NewLine) |\r\n|
extension 'br:mcr.microsoft.com/bicep/extensions/hasoptionalconfig/v1:1.2.3' as extWithOptionalConfig2
//@[000:0102) ├─ExtensionDeclarationSyntax
//@[000:0009) | ├─Token(Identifier) |extension|
//@[010:0076) | ├─StringSyntax
//@[010:0076) | | └─Token(StringComplete) |'br:mcr.microsoft.com/bicep/extensions/hasoptionalconfig/v1:1.2.3'|
//@[077:0077) | ├─SkippedTriviaSyntax
//@[077:0102) | └─AliasAsClauseSyntax
//@[077:0079) |   ├─Token(Identifier) |as|
//@[080:0102) |   └─IdentifierSyntax
//@[080:0102) |     └─Token(Identifier) |extWithOptionalConfig2|
//@[102:0104) ├─Token(NewLine) |\r\n|
extension 'br:mcr.microsoft.com/bicep/extensions/hasoptionalconfig/v1:1.2.3' with {
//@[000:0141) ├─ExtensionDeclarationSyntax
//@[000:0009) | ├─Token(Identifier) |extension|
//@[010:0076) | ├─StringSyntax
//@[010:0076) | | └─Token(StringComplete) |'br:mcr.microsoft.com/bicep/extensions/hasoptionalconfig/v1:1.2.3'|
//@[077:0115) | ├─ExtensionWithClauseSyntax
//@[077:0081) | | ├─Token(Identifier) |with|
//@[082:0115) | | └─ObjectSyntax
//@[082:0083) | |   ├─Token(LeftBrace) |{|
//@[083:0085) | |   ├─Token(NewLine) |\r\n|
  optionalString: strParam1
//@[002:0027) | |   ├─ObjectPropertySyntax
//@[002:0016) | |   | ├─IdentifierSyntax
//@[002:0016) | |   | | └─Token(Identifier) |optionalString|
//@[016:0017) | |   | ├─Token(Colon) |:|
//@[018:0027) | |   | └─VariableAccessSyntax
//@[018:0027) | |   |   └─IdentifierSyntax
//@[018:0027) | |   |     └─Token(Identifier) |strParam1|
//@[027:0029) | |   ├─Token(NewLine) |\r\n|
} as extWithOptionalConfig3
//@[000:0001) | |   └─Token(RightBrace) |}|
//@[002:0027) | └─AliasAsClauseSyntax
//@[002:0004) |   ├─Token(Identifier) |as|
//@[005:0027) |   └─IdentifierSyntax
//@[005:0027) |     └─Token(Identifier) |extWithOptionalConfig3|
//@[027:0029) ├─Token(NewLine) |\r\n|
extension 'br:mcr.microsoft.com/bicep/extensions/hassecureconfig/v1:1.2.3' with {
//@[000:0146) ├─ExtensionDeclarationSyntax
//@[000:0009) | ├─Token(Identifier) |extension|
//@[010:0074) | ├─StringSyntax
//@[010:0074) | | └─Token(StringComplete) |'br:mcr.microsoft.com/bicep/extensions/hassecureconfig/v1:1.2.3'|
//@[075:0125) | ├─ExtensionWithClauseSyntax
//@[075:0079) | | ├─Token(Identifier) |with|
//@[080:0125) | | └─ObjectSyntax
//@[080:0081) | |   ├─Token(LeftBrace) |{|
//@[081:0083) | |   ├─Token(NewLine) |\r\n|
  requiredSecureString: secureStrParam1
//@[002:0039) | |   ├─ObjectPropertySyntax
//@[002:0022) | |   | ├─IdentifierSyntax
//@[002:0022) | |   | | └─Token(Identifier) |requiredSecureString|
//@[022:0023) | |   | ├─Token(Colon) |:|
//@[024:0039) | |   | └─VariableAccessSyntax
//@[024:0039) | |   |   └─IdentifierSyntax
//@[024:0039) | |   |     └─Token(Identifier) |secureStrParam1|
//@[039:0041) | |   ├─Token(NewLine) |\r\n|
} as extWithSecureStr1
//@[000:0001) | |   └─Token(RightBrace) |}|
//@[002:0022) | └─AliasAsClauseSyntax
//@[002:0004) |   ├─Token(Identifier) |as|
//@[005:0022) |   └─IdentifierSyntax
//@[005:0022) |     └─Token(Identifier) |extWithSecureStr1|
//@[022:0024) ├─Token(NewLine) |\r\n|
extension 'br:mcr.microsoft.com/bicep/extensions/hasconfig/v1:1.2.3' with {
//@[000:0132) ├─ExtensionDeclarationSyntax
//@[000:0009) | ├─Token(Identifier) |extension|
//@[010:0068) | ├─StringSyntax
//@[010:0068) | | └─Token(StringComplete) |'br:mcr.microsoft.com/bicep/extensions/hasconfig/v1:1.2.3'|
//@[069:0114) | ├─ExtensionWithClauseSyntax
//@[069:0073) | | ├─Token(Identifier) |with|
//@[074:0114) | | └─ObjectSyntax
//@[074:0075) | |   ├─Token(LeftBrace) |{|
//@[075:0077) | |   ├─Token(NewLine) |\r\n|
  requiredString: testResource1.id
//@[002:0034) | |   ├─ObjectPropertySyntax
//@[002:0016) | |   | ├─IdentifierSyntax
//@[002:0016) | |   | | └─Token(Identifier) |requiredString|
//@[016:0017) | |   | ├─Token(Colon) |:|
//@[018:0034) | |   | └─PropertyAccessSyntax
//@[018:0031) | |   |   ├─VariableAccessSyntax
//@[018:0031) | |   |   | └─IdentifierSyntax
//@[018:0031) | |   |   |   └─Token(Identifier) |testResource1|
//@[031:0032) | |   |   ├─Token(Dot) |.|
//@[032:0034) | |   |   └─IdentifierSyntax
//@[032:0034) | |   |     └─Token(Identifier) |id|
//@[034:0036) | |   ├─Token(NewLine) |\r\n|
} as extWithConfig1
//@[000:0001) | |   └─Token(RightBrace) |}|
//@[002:0019) | └─AliasAsClauseSyntax
//@[002:0004) |   ├─Token(Identifier) |as|
//@[005:0019) |   └─IdentifierSyntax
//@[005:0019) |     └─Token(Identifier) |extWithConfig1|
//@[019:0021) ├─Token(NewLine) |\r\n|
extension 'br:mcr.microsoft.com/bicep/extensions/hasconfig/v1:1.2.3' with {
//@[000:0153) ├─ExtensionDeclarationSyntax
//@[000:0009) | ├─Token(Identifier) |extension|
//@[010:0068) | ├─StringSyntax
//@[010:0068) | | └─Token(StringComplete) |'br:mcr.microsoft.com/bicep/extensions/hasconfig/v1:1.2.3'|
//@[069:0135) | ├─ExtensionWithClauseSyntax
//@[069:0073) | | ├─Token(Identifier) |with|
//@[074:0135) | | └─ObjectSyntax
//@[074:0075) | |   ├─Token(LeftBrace) |{|
//@[075:0077) | |   ├─Token(NewLine) |\r\n|
  requiredString: boolParam1 ? strParamVar1 : strParam1
//@[002:0055) | |   ├─ObjectPropertySyntax
//@[002:0016) | |   | ├─IdentifierSyntax
//@[002:0016) | |   | | └─Token(Identifier) |requiredString|
//@[016:0017) | |   | ├─Token(Colon) |:|
//@[018:0055) | |   | └─TernaryOperationSyntax
//@[018:0028) | |   |   ├─VariableAccessSyntax
//@[018:0028) | |   |   | └─IdentifierSyntax
//@[018:0028) | |   |   |   └─Token(Identifier) |boolParam1|
//@[029:0030) | |   |   ├─Token(Question) |?|
//@[031:0043) | |   |   ├─VariableAccessSyntax
//@[031:0043) | |   |   | └─IdentifierSyntax
//@[031:0043) | |   |   |   └─Token(Identifier) |strParamVar1|
//@[044:0045) | |   |   ├─Token(Colon) |:|
//@[046:0055) | |   |   └─VariableAccessSyntax
//@[046:0055) | |   |     └─IdentifierSyntax
//@[046:0055) | |   |       └─Token(Identifier) |strParam1|
//@[055:0057) | |   ├─Token(NewLine) |\r\n|
} as extWithConfig2
//@[000:0001) | |   └─Token(RightBrace) |}|
//@[002:0019) | └─AliasAsClauseSyntax
//@[002:0004) |   ├─Token(Identifier) |as|
//@[005:0019) |   └─IdentifierSyntax
//@[005:0019) |     └─Token(Identifier) |extWithConfig2|
//@[019:0023) ├─Token(NewLine) |\r\n\r\n|

// END: Extension declarations
//@[030:0034) ├─Token(NewLine) |\r\n\r\n|

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
//@[000:0135) ├─ResourceDeclarationSyntax
//@[000:0008) | ├─Token(Identifier) |resource|
//@[009:0018) | ├─IdentifierSyntax
//@[009:0018) | | └─Token(Identifier) |scopedKv1|
//@[019:0057) | ├─StringSyntax
//@[019:0057) | | └─Token(StringComplete) |'Microsoft.KeyVault/vaults@2019-09-01'|
//@[058:0066) | ├─Token(Identifier) |existing|
//@[067:0068) | ├─Token(Assignment) |=|
//@[069:0135) | └─ObjectSyntax
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
  scope: az.resourceGroup('otherGroup')
//@[002:0039) |   ├─ObjectPropertySyntax
//@[002:0007) |   | ├─IdentifierSyntax
//@[002:0007) |   | | └─Token(Identifier) |scope|
//@[007:0008) |   | ├─Token(Colon) |:|
//@[009:0039) |   | └─InstanceFunctionCallSyntax
//@[009:0011) |   |   ├─VariableAccessSyntax
//@[009:0011) |   |   | └─IdentifierSyntax
//@[009:0011) |   |   |   └─Token(Identifier) |az|
//@[011:0012) |   |   ├─Token(Dot) |.|
//@[012:0025) |   |   ├─IdentifierSyntax
//@[012:0025) |   |   | └─Token(Identifier) |resourceGroup|
//@[025:0026) |   |   ├─Token(LeftParen) |(|
//@[026:0038) |   |   ├─FunctionArgumentSyntax
//@[026:0038) |   |   | └─StringSyntax
//@[026:0038) |   |   |   └─Token(StringComplete) |'otherGroup'|
//@[038:0039) |   |   └─Token(RightParen) |)|
//@[039:0041) |   ├─Token(NewLine) |\r\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0005) ├─Token(NewLine) |\r\n\r\n|

// END: Key vaults
//@[018:0022) ├─Token(NewLine) |\r\n\r\n|

// BEGIN: Test resources
//@[024:0028) ├─Token(NewLine) |\r\n\r\n|

resource testResource1 'az:My.Rp/TestType@2020-01-01' = {
//@[000:0103) ├─ResourceDeclarationSyntax
//@[000:0008) | ├─Token(Identifier) |resource|
//@[009:0022) | ├─IdentifierSyntax
//@[009:0022) | | └─Token(Identifier) |testResource1|
//@[023:0053) | ├─StringSyntax
//@[023:0053) | | └─Token(StringComplete) |'az:My.Rp/TestType@2020-01-01'|
//@[054:0055) | ├─Token(Assignment) |=|
//@[056:0103) | └─ObjectSyntax
//@[056:0057) |   ├─Token(LeftBrace) |{|
//@[057:0059) |   ├─Token(NewLine) |\r\n|
  name: 'testResource1'
//@[002:0023) |   ├─ObjectPropertySyntax
//@[002:0006) |   | ├─IdentifierSyntax
//@[002:0006) |   | | └─Token(Identifier) |name|
//@[006:0007) |   | ├─Token(Colon) |:|
//@[008:0023) |   | └─StringSyntax
//@[008:0023) |   |   └─Token(StringComplete) |'testResource1'|
//@[023:0025) |   ├─Token(NewLine) |\r\n|
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
//@[001:0005) ├─Token(NewLine) |\r\n\r\n|

resource aks 'Microsoft.ContainerService/managedClusters@2024-02-01' = {
//@[000:0138) ├─ResourceDeclarationSyntax
//@[000:0008) | ├─Token(Identifier) |resource|
//@[009:0012) | ├─IdentifierSyntax
//@[009:0012) | | └─Token(Identifier) |aks|
//@[013:0068) | ├─StringSyntax
//@[013:0068) | | └─Token(StringComplete) |'Microsoft.ContainerService/managedClusters@2024-02-01'|
//@[069:0070) | ├─Token(Assignment) |=|
//@[071:0138) | └─ObjectSyntax
//@[071:0072) |   ├─Token(LeftBrace) |{|
//@[072:0074) |   ├─Token(NewLine) |\r\n|
  name: 'aksCluster'
//@[002:0020) |   ├─ObjectPropertySyntax
//@[002:0006) |   | ├─IdentifierSyntax
//@[002:0006) |   | | └─Token(Identifier) |name|
//@[006:0007) |   | ├─Token(Colon) |:|
//@[008:0020) |   | └─StringSyntax
//@[008:0020) |   |   └─Token(StringComplete) |'aksCluster'|
//@[020:0022) |   ├─Token(NewLine) |\r\n|
  location: az.resourceGroup().location
//@[002:0039) |   ├─ObjectPropertySyntax
//@[002:0010) |   | ├─IdentifierSyntax
//@[002:0010) |   | | └─Token(Identifier) |location|
//@[010:0011) |   | ├─Token(Colon) |:|
//@[012:0039) |   | └─PropertyAccessSyntax
//@[012:0030) |   |   ├─InstanceFunctionCallSyntax
//@[012:0014) |   |   | ├─VariableAccessSyntax
//@[012:0014) |   |   | | └─IdentifierSyntax
//@[012:0014) |   |   | |   └─Token(Identifier) |az|
//@[014:0015) |   |   | ├─Token(Dot) |.|
//@[015:0028) |   |   | ├─IdentifierSyntax
//@[015:0028) |   |   | | └─Token(Identifier) |resourceGroup|
//@[028:0029) |   |   | ├─Token(LeftParen) |(|
//@[029:0030) |   |   | └─Token(RightParen) |)|
//@[030:0031) |   |   ├─Token(Dot) |.|
//@[031:0039) |   |   └─IdentifierSyntax
//@[031:0039) |   |     └─Token(Identifier) |location|
//@[039:0041) |   ├─Token(NewLine) |\r\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0005) ├─Token(NewLine) |\r\n\r\n|

// END: Test resources
//@[022:0026) ├─Token(NewLine) |\r\n\r\n|

// BEGIN: Extension configs for modules
//@[039:0043) ├─Token(NewLine) |\r\n\r\n|

module moduleWithExtsWithAliases 'child/hasConfigurableExtensionsWithAlias.bicep' = {
//@[000:0192) ├─ModuleDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |module|
//@[007:0032) | ├─IdentifierSyntax
//@[007:0032) | | └─Token(Identifier) |moduleWithExtsWithAliases|
//@[033:0081) | ├─StringSyntax
//@[033:0081) | | └─Token(StringComplete) |'child/hasConfigurableExtensionsWithAlias.bicep'|
//@[082:0083) | ├─Token(Assignment) |=|
//@[084:0192) | └─ObjectSyntax
//@[084:0085) |   ├─Token(LeftBrace) |{|
//@[085:0087) |   ├─Token(NewLine) |\r\n|
  extensionConfigs: {
//@[002:0102) |   ├─ObjectPropertySyntax
//@[002:0018) |   | ├─IdentifierSyntax
//@[002:0018) |   | | └─Token(Identifier) |extensionConfigs|
//@[018:0019) |   | ├─Token(Colon) |:|
//@[020:0102) |   | └─ObjectSyntax
//@[020:0021) |   |   ├─Token(LeftBrace) |{|
//@[021:0023) |   |   ├─Token(NewLine) |\r\n|
    k8s: {
//@[004:0074) |   |   ├─ObjectPropertySyntax
//@[004:0007) |   |   | ├─IdentifierSyntax
//@[004:0007) |   |   | | └─Token(Identifier) |k8s|
//@[007:0008) |   |   | ├─Token(Colon) |:|
//@[009:0074) |   |   | └─ObjectSyntax
//@[009:0010) |   |   |   ├─Token(LeftBrace) |{|
//@[010:0012) |   |   |   ├─Token(NewLine) |\r\n|
      kubeConfig: 'kubeConfig2'
//@[006:0031) |   |   |   ├─ObjectPropertySyntax
//@[006:0016) |   |   |   | ├─IdentifierSyntax
//@[006:0016) |   |   |   | | └─Token(Identifier) |kubeConfig|
//@[016:0017) |   |   |   | ├─Token(Colon) |:|
//@[018:0031) |   |   |   | └─StringSyntax
//@[018:0031) |   |   |   |   └─Token(StringComplete) |'kubeConfig2'|
//@[031:0033) |   |   |   ├─Token(NewLine) |\r\n|
      namespace: 'ns2'
//@[006:0022) |   |   |   ├─ObjectPropertySyntax
//@[006:0015) |   |   |   | ├─IdentifierSyntax
//@[006:0015) |   |   |   | | └─Token(Identifier) |namespace|
//@[015:0016) |   |   |   | ├─Token(Colon) |:|
//@[017:0022) |   |   |   | └─StringSyntax
//@[017:0022) |   |   |   |   └─Token(StringComplete) |'ns2'|
//@[022:0024) |   |   |   ├─Token(NewLine) |\r\n|
    }
//@[004:0005) |   |   |   └─Token(RightBrace) |}|
//@[005:0007) |   |   ├─Token(NewLine) |\r\n|
  }
//@[002:0003) |   |   └─Token(RightBrace) |}|
//@[003:0005) |   ├─Token(NewLine) |\r\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0005) ├─Token(NewLine) |\r\n\r\n|

module moduleWithExtsWithoutAliases 'child/hasConfigurableExtensionsWithoutAlias.bicep' = {
//@[000:0181) ├─ModuleDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |module|
//@[007:0035) | ├─IdentifierSyntax
//@[007:0035) | | └─Token(Identifier) |moduleWithExtsWithoutAliases|
//@[036:0087) | ├─StringSyntax
//@[036:0087) | | └─Token(StringComplete) |'child/hasConfigurableExtensionsWithoutAlias.bicep'|
//@[088:0089) | ├─Token(Assignment) |=|
//@[090:0181) | └─ObjectSyntax
//@[090:0091) |   ├─Token(LeftBrace) |{|
//@[091:0093) |   ├─Token(NewLine) |\r\n|
  extensionConfigs: {
//@[002:0085) |   ├─ObjectPropertySyntax
//@[002:0018) |   | ├─IdentifierSyntax
//@[002:0018) |   | | └─Token(Identifier) |extensionConfigs|
//@[018:0019) |   | ├─Token(Colon) |:|
//@[020:0085) |   | └─ObjectSyntax
//@[020:0021) |   |   ├─Token(LeftBrace) |{|
//@[021:0023) |   |   ├─Token(NewLine) |\r\n|
    kubernetes: {
//@[004:0057) |   |   ├─ObjectPropertySyntax
//@[004:0014) |   |   | ├─IdentifierSyntax
//@[004:0014) |   |   | | └─Token(Identifier) |kubernetes|
//@[014:0015) |   |   | ├─Token(Colon) |:|
//@[016:0057) |   |   | └─ObjectSyntax
//@[016:0017) |   |   |   ├─Token(LeftBrace) |{|
//@[017:0019) |   |   |   ├─Token(NewLine) |\r\n|
      kubeConfig: 'kubeConfig2'
//@[006:0031) |   |   |   ├─ObjectPropertySyntax
//@[006:0016) |   |   |   | ├─IdentifierSyntax
//@[006:0016) |   |   |   | | └─Token(Identifier) |kubeConfig|
//@[016:0017) |   |   |   | ├─Token(Colon) |:|
//@[018:0031) |   |   |   | └─StringSyntax
//@[018:0031) |   |   |   |   └─Token(StringComplete) |'kubeConfig2'|
//@[031:0033) |   |   |   ├─Token(NewLine) |\r\n|
    }
//@[004:0005) |   |   |   └─Token(RightBrace) |}|
//@[005:0007) |   |   ├─Token(NewLine) |\r\n|
  }
//@[002:0003) |   |   └─Token(RightBrace) |}|
//@[003:0005) |   ├─Token(NewLine) |\r\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0005) ├─Token(NewLine) |\r\n\r\n|

module moduleExtConfigsFromParams 'child/hasConfigurableExtensionsWithAlias.bicep' = {
//@[000:0251) ├─ModuleDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |module|
//@[007:0033) | ├─IdentifierSyntax
//@[007:0033) | | └─Token(Identifier) |moduleExtConfigsFromParams|
//@[034:0082) | ├─StringSyntax
//@[034:0082) | | └─Token(StringComplete) |'child/hasConfigurableExtensionsWithAlias.bicep'|
//@[083:0084) | ├─Token(Assignment) |=|
//@[085:0251) | └─ObjectSyntax
//@[085:0086) |   ├─Token(LeftBrace) |{|
//@[086:0088) |   ├─Token(NewLine) |\r\n|
  extensionConfigs: {
//@[002:0160) |   ├─ObjectPropertySyntax
//@[002:0018) |   | ├─IdentifierSyntax
//@[002:0018) |   | | └─Token(Identifier) |extensionConfigs|
//@[018:0019) |   | ├─Token(Colon) |:|
//@[020:0160) |   | └─ObjectSyntax
//@[020:0021) |   |   ├─Token(LeftBrace) |{|
//@[021:0023) |   |   ├─Token(NewLine) |\r\n|
    k8s: {
//@[004:0132) |   |   ├─ObjectPropertySyntax
//@[004:0007) |   |   | ├─IdentifierSyntax
//@[004:0007) |   |   | | └─Token(Identifier) |k8s|
//@[007:0008) |   |   | ├─Token(Colon) |:|
//@[009:0132) |   |   | └─ObjectSyntax
//@[009:0010) |   |   |   ├─Token(LeftBrace) |{|
//@[010:0012) |   |   |   ├─Token(NewLine) |\r\n|
      kubeConfig: boolParam1 ? secureStrParam1 : strParam1
//@[006:0058) |   |   |   ├─ObjectPropertySyntax
//@[006:0016) |   |   |   | ├─IdentifierSyntax
//@[006:0016) |   |   |   | | └─Token(Identifier) |kubeConfig|
//@[016:0017) |   |   |   | ├─Token(Colon) |:|
//@[018:0058) |   |   |   | └─TernaryOperationSyntax
//@[018:0028) |   |   |   |   ├─VariableAccessSyntax
//@[018:0028) |   |   |   |   | └─IdentifierSyntax
//@[018:0028) |   |   |   |   |   └─Token(Identifier) |boolParam1|
//@[029:0030) |   |   |   |   ├─Token(Question) |?|
//@[031:0046) |   |   |   |   ├─VariableAccessSyntax
//@[031:0046) |   |   |   |   | └─IdentifierSyntax
//@[031:0046) |   |   |   |   |   └─Token(Identifier) |secureStrParam1|
//@[047:0048) |   |   |   |   ├─Token(Colon) |:|
//@[049:0058) |   |   |   |   └─VariableAccessSyntax
//@[049:0058) |   |   |   |     └─IdentifierSyntax
//@[049:0058) |   |   |   |       └─Token(Identifier) |strParam1|
//@[058:0060) |   |   |   ├─Token(NewLine) |\r\n|
      namespace: boolParam1 ? strParam1 : 'falseCond'
//@[006:0053) |   |   |   ├─ObjectPropertySyntax
//@[006:0015) |   |   |   | ├─IdentifierSyntax
//@[006:0015) |   |   |   | | └─Token(Identifier) |namespace|
//@[015:0016) |   |   |   | ├─Token(Colon) |:|
//@[017:0053) |   |   |   | └─TernaryOperationSyntax
//@[017:0027) |   |   |   |   ├─VariableAccessSyntax
//@[017:0027) |   |   |   |   | └─IdentifierSyntax
//@[017:0027) |   |   |   |   |   └─Token(Identifier) |boolParam1|
//@[028:0029) |   |   |   |   ├─Token(Question) |?|
//@[030:0039) |   |   |   |   ├─VariableAccessSyntax
//@[030:0039) |   |   |   |   | └─IdentifierSyntax
//@[030:0039) |   |   |   |   |   └─Token(Identifier) |strParam1|
//@[040:0041) |   |   |   |   ├─Token(Colon) |:|
//@[042:0053) |   |   |   |   └─StringSyntax
//@[042:0053) |   |   |   |     └─Token(StringComplete) |'falseCond'|
//@[053:0055) |   |   |   ├─Token(NewLine) |\r\n|
    }
//@[004:0005) |   |   |   └─Token(RightBrace) |}|
//@[005:0007) |   |   ├─Token(NewLine) |\r\n|
  }
//@[002:0003) |   |   └─Token(RightBrace) |}|
//@[003:0005) |   ├─Token(NewLine) |\r\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0005) ├─Token(NewLine) |\r\n\r\n|

module moduleExtConfigFromKeyVaultReference 'child/hasConfigurableExtensionsWithAlias.bicep' = {
//@[000:0221) ├─ModuleDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |module|
//@[007:0043) | ├─IdentifierSyntax
//@[007:0043) | | └─Token(Identifier) |moduleExtConfigFromKeyVaultReference|
//@[044:0092) | ├─StringSyntax
//@[044:0092) | | └─Token(StringComplete) |'child/hasConfigurableExtensionsWithAlias.bicep'|
//@[093:0094) | ├─Token(Assignment) |=|
//@[095:0221) | └─ObjectSyntax
//@[095:0096) |   ├─Token(LeftBrace) |{|
//@[096:0098) |   ├─Token(NewLine) |\r\n|
  extensionConfigs: {
//@[002:0120) |   ├─ObjectPropertySyntax
//@[002:0018) |   | ├─IdentifierSyntax
//@[002:0018) |   | | └─Token(Identifier) |extensionConfigs|
//@[018:0019) |   | ├─Token(Colon) |:|
//@[020:0120) |   | └─ObjectSyntax
//@[020:0021) |   |   ├─Token(LeftBrace) |{|
//@[021:0023) |   |   ├─Token(NewLine) |\r\n|
    k8s: {
//@[004:0092) |   |   ├─ObjectPropertySyntax
//@[004:0007) |   |   | ├─IdentifierSyntax
//@[004:0007) |   |   | | └─Token(Identifier) |k8s|
//@[007:0008) |   |   | ├─Token(Colon) |:|
//@[009:0092) |   |   | └─ObjectSyntax
//@[009:0010) |   |   |   ├─Token(LeftBrace) |{|
//@[010:0012) |   |   |   ├─Token(NewLine) |\r\n|
      kubeConfig: kv1.getSecret('myKubeConfig')
//@[006:0047) |   |   |   ├─ObjectPropertySyntax
//@[006:0016) |   |   |   | ├─IdentifierSyntax
//@[006:0016) |   |   |   | | └─Token(Identifier) |kubeConfig|
//@[016:0017) |   |   |   | ├─Token(Colon) |:|
//@[018:0047) |   |   |   | └─InstanceFunctionCallSyntax
//@[018:0021) |   |   |   |   ├─VariableAccessSyntax
//@[018:0021) |   |   |   |   | └─IdentifierSyntax
//@[018:0021) |   |   |   |   |   └─Token(Identifier) |kv1|
//@[021:0022) |   |   |   |   ├─Token(Dot) |.|
//@[022:0031) |   |   |   |   ├─IdentifierSyntax
//@[022:0031) |   |   |   |   | └─Token(Identifier) |getSecret|
//@[031:0032) |   |   |   |   ├─Token(LeftParen) |(|
//@[032:0046) |   |   |   |   ├─FunctionArgumentSyntax
//@[032:0046) |   |   |   |   | └─StringSyntax
//@[032:0046) |   |   |   |   |   └─Token(StringComplete) |'myKubeConfig'|
//@[046:0047) |   |   |   |   └─Token(RightParen) |)|
//@[047:0049) |   |   |   ├─Token(NewLine) |\r\n|
      namespace: strVar1
//@[006:0024) |   |   |   ├─ObjectPropertySyntax
//@[006:0015) |   |   |   | ├─IdentifierSyntax
//@[006:0015) |   |   |   | | └─Token(Identifier) |namespace|
//@[015:0016) |   |   |   | ├─Token(Colon) |:|
//@[017:0024) |   |   |   | └─VariableAccessSyntax
//@[017:0024) |   |   |   |   └─IdentifierSyntax
//@[017:0024) |   |   |   |     └─Token(Identifier) |strVar1|
//@[024:0026) |   |   |   ├─Token(NewLine) |\r\n|
    }
//@[004:0005) |   |   |   └─Token(RightBrace) |}|
//@[005:0007) |   |   ├─Token(NewLine) |\r\n|
  }
//@[002:0003) |   |   └─Token(RightBrace) |}|
//@[003:0005) |   ├─Token(NewLine) |\r\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0005) ├─Token(NewLine) |\r\n\r\n|

module moduleExtConfigFromReferences 'child/hasConfigurableExtensionsWithAlias.bicep' = {
//@[000:0265) ├─ModuleDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |module|
//@[007:0036) | ├─IdentifierSyntax
//@[007:0036) | | └─Token(Identifier) |moduleExtConfigFromReferences|
//@[037:0085) | ├─StringSyntax
//@[037:0085) | | └─Token(StringComplete) |'child/hasConfigurableExtensionsWithAlias.bicep'|
//@[086:0087) | ├─Token(Assignment) |=|
//@[088:0265) | └─ObjectSyntax
//@[088:0089) |   ├─Token(LeftBrace) |{|
//@[089:0091) |   ├─Token(NewLine) |\r\n|
  extensionConfigs: {
//@[002:0171) |   ├─ObjectPropertySyntax
//@[002:0018) |   | ├─IdentifierSyntax
//@[002:0018) |   | | └─Token(Identifier) |extensionConfigs|
//@[018:0019) |   | ├─Token(Colon) |:|
//@[020:0171) |   | └─ObjectSyntax
//@[020:0021) |   |   ├─Token(LeftBrace) |{|
//@[021:0023) |   |   ├─Token(NewLine) |\r\n|
    k8s: {
//@[004:0143) |   |   ├─ObjectPropertySyntax
//@[004:0007) |   |   | ├─IdentifierSyntax
//@[004:0007) |   |   | | └─Token(Identifier) |k8s|
//@[007:0008) |   |   | ├─Token(Colon) |:|
//@[009:0143) |   |   | └─ObjectSyntax
//@[009:0010) |   |   |   ├─Token(LeftBrace) |{|
//@[010:0012) |   |   |   ├─Token(NewLine) |\r\n|
      kubeConfig: aks.listClusterAdminCredential().kubeconfigs[0].value
//@[006:0071) |   |   |   ├─ObjectPropertySyntax
//@[006:0016) |   |   |   | ├─IdentifierSyntax
//@[006:0016) |   |   |   | | └─Token(Identifier) |kubeConfig|
//@[016:0017) |   |   |   | ├─Token(Colon) |:|
//@[018:0071) |   |   |   | └─PropertyAccessSyntax
//@[018:0065) |   |   |   |   ├─ArrayAccessSyntax
//@[018:0062) |   |   |   |   | ├─PropertyAccessSyntax
//@[018:0050) |   |   |   |   | | ├─InstanceFunctionCallSyntax
//@[018:0021) |   |   |   |   | | | ├─VariableAccessSyntax
//@[018:0021) |   |   |   |   | | | | └─IdentifierSyntax
//@[018:0021) |   |   |   |   | | | |   └─Token(Identifier) |aks|
//@[021:0022) |   |   |   |   | | | ├─Token(Dot) |.|
//@[022:0048) |   |   |   |   | | | ├─IdentifierSyntax
//@[022:0048) |   |   |   |   | | | | └─Token(Identifier) |listClusterAdminCredential|
//@[048:0049) |   |   |   |   | | | ├─Token(LeftParen) |(|
//@[049:0050) |   |   |   |   | | | └─Token(RightParen) |)|
//@[050:0051) |   |   |   |   | | ├─Token(Dot) |.|
//@[051:0062) |   |   |   |   | | └─IdentifierSyntax
//@[051:0062) |   |   |   |   | |   └─Token(Identifier) |kubeconfigs|
//@[062:0063) |   |   |   |   | ├─Token(LeftSquare) |[|
//@[063:0064) |   |   |   |   | ├─IntegerLiteralSyntax
//@[063:0064) |   |   |   |   | | └─Token(Integer) |0|
//@[064:0065) |   |   |   |   | └─Token(RightSquare) |]|
//@[065:0066) |   |   |   |   ├─Token(Dot) |.|
//@[066:0071) |   |   |   |   └─IdentifierSyntax
//@[066:0071) |   |   |   |     └─Token(Identifier) |value|
//@[071:0073) |   |   |   ├─Token(NewLine) |\r\n|
      namespace: testResource1.properties.namespace
//@[006:0051) |   |   |   ├─ObjectPropertySyntax
//@[006:0015) |   |   |   | ├─IdentifierSyntax
//@[006:0015) |   |   |   | | └─Token(Identifier) |namespace|
//@[015:0016) |   |   |   | ├─Token(Colon) |:|
//@[017:0051) |   |   |   | └─PropertyAccessSyntax
//@[017:0041) |   |   |   |   ├─PropertyAccessSyntax
//@[017:0030) |   |   |   |   | ├─VariableAccessSyntax
//@[017:0030) |   |   |   |   | | └─IdentifierSyntax
//@[017:0030) |   |   |   |   | |   └─Token(Identifier) |testResource1|
//@[030:0031) |   |   |   |   | ├─Token(Dot) |.|
//@[031:0041) |   |   |   |   | └─IdentifierSyntax
//@[031:0041) |   |   |   |   |   └─Token(Identifier) |properties|
//@[041:0042) |   |   |   |   ├─Token(Dot) |.|
//@[042:0051) |   |   |   |   └─IdentifierSyntax
//@[042:0051) |   |   |   |     └─Token(Identifier) |namespace|
//@[051:0053) |   |   |   ├─Token(NewLine) |\r\n|
    }
//@[004:0005) |   |   |   └─Token(RightBrace) |}|
//@[005:0007) |   |   ├─Token(NewLine) |\r\n|
  }
//@[002:0003) |   |   └─Token(RightBrace) |}|
//@[003:0005) |   ├─Token(NewLine) |\r\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0005) ├─Token(NewLine) |\r\n\r\n|

module moduleWithExtsUsingFullInheritance 'child/hasConfigurableExtensionsWithAlias.bicep' = {
//@[000:0146) ├─ModuleDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |module|
//@[007:0041) | ├─IdentifierSyntax
//@[007:0041) | | └─Token(Identifier) |moduleWithExtsUsingFullInheritance|
//@[042:0090) | ├─StringSyntax
//@[042:0090) | | └─Token(StringComplete) |'child/hasConfigurableExtensionsWithAlias.bicep'|
//@[091:0092) | ├─Token(Assignment) |=|
//@[093:0146) | └─ObjectSyntax
//@[093:0094) |   ├─Token(LeftBrace) |{|
//@[094:0096) |   ├─Token(NewLine) |\r\n|
  extensionConfigs: {
//@[002:0047) |   ├─ObjectPropertySyntax
//@[002:0018) |   | ├─IdentifierSyntax
//@[002:0018) |   | | └─Token(Identifier) |extensionConfigs|
//@[018:0019) |   | ├─Token(Colon) |:|
//@[020:0047) |   | └─ObjectSyntax
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
  }
//@[002:0003) |   |   └─Token(RightBrace) |}|
//@[003:0005) |   ├─Token(NewLine) |\r\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0005) ├─Token(NewLine) |\r\n\r\n|

module moduleWithExtsUsingFullInheritanceTernary1 'child/hasConfigurableExtensionsWithAlias.bicep' = {
//@[000:0257) ├─ModuleDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |module|
//@[007:0049) | ├─IdentifierSyntax
//@[007:0049) | | └─Token(Identifier) |moduleWithExtsUsingFullInheritanceTernary1|
//@[050:0098) | ├─StringSyntax
//@[050:0098) | | └─Token(StringComplete) |'child/hasConfigurableExtensionsWithAlias.bicep'|
//@[099:0100) | ├─Token(Assignment) |=|
//@[101:0257) | └─ObjectSyntax
//@[101:0102) |   ├─Token(LeftBrace) |{|
//@[102:0104) |   ├─Token(NewLine) |\r\n|
  extensionConfigs: {
//@[002:0150) |   ├─ObjectPropertySyntax
//@[002:0018) |   | ├─IdentifierSyntax
//@[002:0018) |   | | └─Token(Identifier) |extensionConfigs|
//@[018:0019) |   | ├─Token(Colon) |:|
//@[020:0150) |   | └─ObjectSyntax
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
    extWithOptionalConfig: boolParam1 ? extWithOptionalConfig1.config : extWithOptionalConfig2.config
//@[004:0101) |   |   ├─ObjectPropertySyntax
//@[004:0025) |   |   | ├─IdentifierSyntax
//@[004:0025) |   |   | | └─Token(Identifier) |extWithOptionalConfig|
//@[025:0026) |   |   | ├─Token(Colon) |:|
//@[027:0101) |   |   | └─TernaryOperationSyntax
//@[027:0037) |   |   |   ├─VariableAccessSyntax
//@[027:0037) |   |   |   | └─IdentifierSyntax
//@[027:0037) |   |   |   |   └─Token(Identifier) |boolParam1|
//@[038:0039) |   |   |   ├─Token(Question) |?|
//@[040:0069) |   |   |   ├─PropertyAccessSyntax
//@[040:0062) |   |   |   | ├─VariableAccessSyntax
//@[040:0062) |   |   |   | | └─IdentifierSyntax
//@[040:0062) |   |   |   | |   └─Token(Identifier) |extWithOptionalConfig1|
//@[062:0063) |   |   |   | ├─Token(Dot) |.|
//@[063:0069) |   |   |   | └─IdentifierSyntax
//@[063:0069) |   |   |   |   └─Token(Identifier) |config|
//@[070:0071) |   |   |   ├─Token(Colon) |:|
//@[072:0101) |   |   |   └─PropertyAccessSyntax
//@[072:0094) |   |   |     ├─VariableAccessSyntax
//@[072:0094) |   |   |     | └─IdentifierSyntax
//@[072:0094) |   |   |     |   └─Token(Identifier) |extWithOptionalConfig2|
//@[094:0095) |   |   |     ├─Token(Dot) |.|
//@[095:0101) |   |   |     └─IdentifierSyntax
//@[095:0101) |   |   |       └─Token(Identifier) |config|
//@[101:0103) |   |   ├─Token(NewLine) |\r\n|
  }
//@[002:0003) |   |   └─Token(RightBrace) |}|
//@[003:0005) |   ├─Token(NewLine) |\r\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0005) ├─Token(NewLine) |\r\n\r\n|

module moduleWithExtsUsingPiecemealInheritance 'child/hasConfigurableExtensionsWithAlias.bicep' = {
//@[000:0229) ├─ModuleDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |module|
//@[007:0046) | ├─IdentifierSyntax
//@[007:0046) | | └─Token(Identifier) |moduleWithExtsUsingPiecemealInheritance|
//@[047:0095) | ├─StringSyntax
//@[047:0095) | | └─Token(StringComplete) |'child/hasConfigurableExtensionsWithAlias.bicep'|
//@[096:0097) | ├─Token(Assignment) |=|
//@[098:0229) | └─ObjectSyntax
//@[098:0099) |   ├─Token(LeftBrace) |{|
//@[099:0101) |   ├─Token(NewLine) |\r\n|
  extensionConfigs: {
//@[002:0125) |   ├─ObjectPropertySyntax
//@[002:0018) |   | ├─IdentifierSyntax
//@[002:0018) |   | | └─Token(Identifier) |extensionConfigs|
//@[018:0019) |   | ├─Token(Colon) |:|
//@[020:0125) |   | └─ObjectSyntax
//@[020:0021) |   |   ├─Token(LeftBrace) |{|
//@[021:0023) |   |   ├─Token(NewLine) |\r\n|
    k8s: {
//@[004:0097) |   |   ├─ObjectPropertySyntax
//@[004:0007) |   |   | ├─IdentifierSyntax
//@[004:0007) |   |   | | └─Token(Identifier) |k8s|
//@[007:0008) |   |   | ├─Token(Colon) |:|
//@[009:0097) |   |   | └─ObjectSyntax
//@[009:0010) |   |   |   ├─Token(LeftBrace) |{|
//@[010:0012) |   |   |   ├─Token(NewLine) |\r\n|
      kubeConfig: k8s.config.kubeConfig
//@[006:0039) |   |   |   ├─ObjectPropertySyntax
//@[006:0016) |   |   |   | ├─IdentifierSyntax
//@[006:0016) |   |   |   | | └─Token(Identifier) |kubeConfig|
//@[016:0017) |   |   |   | ├─Token(Colon) |:|
//@[018:0039) |   |   |   | └─PropertyAccessSyntax
//@[018:0028) |   |   |   |   ├─PropertyAccessSyntax
//@[018:0021) |   |   |   |   | ├─VariableAccessSyntax
//@[018:0021) |   |   |   |   | | └─IdentifierSyntax
//@[018:0021) |   |   |   |   | |   └─Token(Identifier) |k8s|
//@[021:0022) |   |   |   |   | ├─Token(Dot) |.|
//@[022:0028) |   |   |   |   | └─IdentifierSyntax
//@[022:0028) |   |   |   |   |   └─Token(Identifier) |config|
//@[028:0029) |   |   |   |   ├─Token(Dot) |.|
//@[029:0039) |   |   |   |   └─IdentifierSyntax
//@[029:0039) |   |   |   |     └─Token(Identifier) |kubeConfig|
//@[039:0041) |   |   |   ├─Token(NewLine) |\r\n|
      namespace: k8s.config.namespace
//@[006:0037) |   |   |   ├─ObjectPropertySyntax
//@[006:0015) |   |   |   | ├─IdentifierSyntax
//@[006:0015) |   |   |   | | └─Token(Identifier) |namespace|
//@[015:0016) |   |   |   | ├─Token(Colon) |:|
//@[017:0037) |   |   |   | └─PropertyAccessSyntax
//@[017:0027) |   |   |   |   ├─PropertyAccessSyntax
//@[017:0020) |   |   |   |   | ├─VariableAccessSyntax
//@[017:0020) |   |   |   |   | | └─IdentifierSyntax
//@[017:0020) |   |   |   |   | |   └─Token(Identifier) |k8s|
//@[020:0021) |   |   |   |   | ├─Token(Dot) |.|
//@[021:0027) |   |   |   |   | └─IdentifierSyntax
//@[021:0027) |   |   |   |   |   └─Token(Identifier) |config|
//@[027:0028) |   |   |   |   ├─Token(Dot) |.|
//@[028:0037) |   |   |   |   └─IdentifierSyntax
//@[028:0037) |   |   |   |     └─Token(Identifier) |namespace|
//@[037:0039) |   |   |   ├─Token(NewLine) |\r\n|
    }
//@[004:0005) |   |   |   └─Token(RightBrace) |}|
//@[005:0007) |   |   ├─Token(NewLine) |\r\n|
  }
//@[002:0003) |   |   └─Token(RightBrace) |}|
//@[003:0005) |   ├─Token(NewLine) |\r\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0005) ├─Token(NewLine) |\r\n\r\n|

module moduleWithExtsUsingPiecemealInheritanceLooped 'child/hasConfigurableExtensionsWithAlias.bicep' = [for i in range(0, 4): {
//@[000:0315) ├─ModuleDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |module|
//@[007:0052) | ├─IdentifierSyntax
//@[007:0052) | | └─Token(Identifier) |moduleWithExtsUsingPiecemealInheritanceLooped|
//@[053:0101) | ├─StringSyntax
//@[053:0101) | | └─Token(StringComplete) |'child/hasConfigurableExtensionsWithAlias.bicep'|
//@[102:0103) | ├─Token(Assignment) |=|
//@[104:0315) | └─ForSyntax
//@[104:0105) |   ├─Token(LeftSquare) |[|
//@[105:0108) |   ├─Token(Identifier) |for|
//@[109:0110) |   ├─LocalVariableSyntax
//@[109:0110) |   | └─IdentifierSyntax
//@[109:0110) |   |   └─Token(Identifier) |i|
//@[111:0113) |   ├─Token(Identifier) |in|
//@[114:0125) |   ├─FunctionCallSyntax
//@[114:0119) |   | ├─IdentifierSyntax
//@[114:0119) |   | | └─Token(Identifier) |range|
//@[119:0120) |   | ├─Token(LeftParen) |(|
//@[120:0121) |   | ├─FunctionArgumentSyntax
//@[120:0121) |   | | └─IntegerLiteralSyntax
//@[120:0121) |   | |   └─Token(Integer) |0|
//@[121:0122) |   | ├─Token(Comma) |,|
//@[123:0124) |   | ├─FunctionArgumentSyntax
//@[123:0124) |   | | └─IntegerLiteralSyntax
//@[123:0124) |   | |   └─Token(Integer) |4|
//@[124:0125) |   | └─Token(RightParen) |)|
//@[125:0126) |   ├─Token(Colon) |:|
//@[127:0314) |   ├─ObjectSyntax
//@[127:0128) |   | ├─Token(LeftBrace) |{|
//@[128:0130) |   | ├─Token(NewLine) |\r\n|
  name: 'moduleWithExtsPiecemealInheritanceLooped${i}'
//@[002:0054) |   | ├─ObjectPropertySyntax
//@[002:0006) |   | | ├─IdentifierSyntax
//@[002:0006) |   | | | └─Token(Identifier) |name|
//@[006:0007) |   | | ├─Token(Colon) |:|
//@[008:0054) |   | | └─StringSyntax
//@[008:0051) |   | |   ├─Token(StringLeftPiece) |'moduleWithExtsPiecemealInheritanceLooped${|
//@[051:0052) |   | |   ├─VariableAccessSyntax
//@[051:0052) |   | |   | └─IdentifierSyntax
//@[051:0052) |   | |   |   └─Token(Identifier) |i|
//@[052:0054) |   | |   └─Token(StringRightPiece) |}'|
//@[054:0056) |   | ├─Token(NewLine) |\r\n|
  extensionConfigs: {
//@[002:0125) |   | ├─ObjectPropertySyntax
//@[002:0018) |   | | ├─IdentifierSyntax
//@[002:0018) |   | | | └─Token(Identifier) |extensionConfigs|
//@[018:0019) |   | | ├─Token(Colon) |:|
//@[020:0125) |   | | └─ObjectSyntax
//@[020:0021) |   | |   ├─Token(LeftBrace) |{|
//@[021:0023) |   | |   ├─Token(NewLine) |\r\n|
    k8s: {
//@[004:0097) |   | |   ├─ObjectPropertySyntax
//@[004:0007) |   | |   | ├─IdentifierSyntax
//@[004:0007) |   | |   | | └─Token(Identifier) |k8s|
//@[007:0008) |   | |   | ├─Token(Colon) |:|
//@[009:0097) |   | |   | └─ObjectSyntax
//@[009:0010) |   | |   |   ├─Token(LeftBrace) |{|
//@[010:0012) |   | |   |   ├─Token(NewLine) |\r\n|
      kubeConfig: k8s.config.kubeConfig
//@[006:0039) |   | |   |   ├─ObjectPropertySyntax
//@[006:0016) |   | |   |   | ├─IdentifierSyntax
//@[006:0016) |   | |   |   | | └─Token(Identifier) |kubeConfig|
//@[016:0017) |   | |   |   | ├─Token(Colon) |:|
//@[018:0039) |   | |   |   | └─PropertyAccessSyntax
//@[018:0028) |   | |   |   |   ├─PropertyAccessSyntax
//@[018:0021) |   | |   |   |   | ├─VariableAccessSyntax
//@[018:0021) |   | |   |   |   | | └─IdentifierSyntax
//@[018:0021) |   | |   |   |   | |   └─Token(Identifier) |k8s|
//@[021:0022) |   | |   |   |   | ├─Token(Dot) |.|
//@[022:0028) |   | |   |   |   | └─IdentifierSyntax
//@[022:0028) |   | |   |   |   |   └─Token(Identifier) |config|
//@[028:0029) |   | |   |   |   ├─Token(Dot) |.|
//@[029:0039) |   | |   |   |   └─IdentifierSyntax
//@[029:0039) |   | |   |   |     └─Token(Identifier) |kubeConfig|
//@[039:0041) |   | |   |   ├─Token(NewLine) |\r\n|
      namespace: k8s.config.namespace
//@[006:0037) |   | |   |   ├─ObjectPropertySyntax
//@[006:0015) |   | |   |   | ├─IdentifierSyntax
//@[006:0015) |   | |   |   | | └─Token(Identifier) |namespace|
//@[015:0016) |   | |   |   | ├─Token(Colon) |:|
//@[017:0037) |   | |   |   | └─PropertyAccessSyntax
//@[017:0027) |   | |   |   |   ├─PropertyAccessSyntax
//@[017:0020) |   | |   |   |   | ├─VariableAccessSyntax
//@[017:0020) |   | |   |   |   | | └─IdentifierSyntax
//@[017:0020) |   | |   |   |   | |   └─Token(Identifier) |k8s|
//@[020:0021) |   | |   |   |   | ├─Token(Dot) |.|
//@[021:0027) |   | |   |   |   | └─IdentifierSyntax
//@[021:0027) |   | |   |   |   |   └─Token(Identifier) |config|
//@[027:0028) |   | |   |   |   ├─Token(Dot) |.|
//@[028:0037) |   | |   |   |   └─IdentifierSyntax
//@[028:0037) |   | |   |   |     └─Token(Identifier) |namespace|
//@[037:0039) |   | |   |   ├─Token(NewLine) |\r\n|
    }
//@[004:0005) |   | |   |   └─Token(RightBrace) |}|
//@[005:0007) |   | |   ├─Token(NewLine) |\r\n|
  }
//@[002:0003) |   | |   └─Token(RightBrace) |}|
//@[003:0005) |   | ├─Token(NewLine) |\r\n|
}]
//@[000:0001) |   | └─Token(RightBrace) |}|
//@[001:0002) |   └─Token(RightSquare) |]|
//@[002:0006) ├─Token(NewLine) |\r\n\r\n|

module moduleExtConfigsConditionalMixed 'child/hasConfigurableExtensionsWithAlias.bicep' = {
//@[000:0296) ├─ModuleDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |module|
//@[007:0039) | ├─IdentifierSyntax
//@[007:0039) | | └─Token(Identifier) |moduleExtConfigsConditionalMixed|
//@[040:0088) | ├─StringSyntax
//@[040:0088) | | └─Token(StringComplete) |'child/hasConfigurableExtensionsWithAlias.bicep'|
//@[089:0090) | ├─Token(Assignment) |=|
//@[091:0296) | └─ObjectSyntax
//@[091:0092) |   ├─Token(LeftBrace) |{|
//@[092:0094) |   ├─Token(NewLine) |\r\n|
  extensionConfigs: {
//@[002:0199) |   ├─ObjectPropertySyntax
//@[002:0018) |   | ├─IdentifierSyntax
//@[002:0018) |   | | └─Token(Identifier) |extensionConfigs|
//@[018:0019) |   | ├─Token(Colon) |:|
//@[020:0199) |   | └─ObjectSyntax
//@[020:0021) |   |   ├─Token(LeftBrace) |{|
//@[021:0023) |   |   ├─Token(NewLine) |\r\n|
    k8s: {
//@[004:0171) |   |   ├─ObjectPropertySyntax
//@[004:0007) |   |   | ├─IdentifierSyntax
//@[004:0007) |   |   | | └─Token(Identifier) |k8s|
//@[007:0008) |   |   | ├─Token(Colon) |:|
//@[009:0171) |   |   | └─ObjectSyntax
//@[009:0010) |   |   |   ├─Token(LeftBrace) |{|
//@[010:0012) |   |   |   ├─Token(NewLine) |\r\n|
      kubeConfig: boolParam1 ? secureStrParam1 : k8s.config.kubeConfig
//@[006:0070) |   |   |   ├─ObjectPropertySyntax
//@[006:0016) |   |   |   | ├─IdentifierSyntax
//@[006:0016) |   |   |   | | └─Token(Identifier) |kubeConfig|
//@[016:0017) |   |   |   | ├─Token(Colon) |:|
//@[018:0070) |   |   |   | └─TernaryOperationSyntax
//@[018:0028) |   |   |   |   ├─VariableAccessSyntax
//@[018:0028) |   |   |   |   | └─IdentifierSyntax
//@[018:0028) |   |   |   |   |   └─Token(Identifier) |boolParam1|
//@[029:0030) |   |   |   |   ├─Token(Question) |?|
//@[031:0046) |   |   |   |   ├─VariableAccessSyntax
//@[031:0046) |   |   |   |   | └─IdentifierSyntax
//@[031:0046) |   |   |   |   |   └─Token(Identifier) |secureStrParam1|
//@[047:0048) |   |   |   |   ├─Token(Colon) |:|
//@[049:0070) |   |   |   |   └─PropertyAccessSyntax
//@[049:0059) |   |   |   |     ├─PropertyAccessSyntax
//@[049:0052) |   |   |   |     | ├─VariableAccessSyntax
//@[049:0052) |   |   |   |     | | └─IdentifierSyntax
//@[049:0052) |   |   |   |     | |   └─Token(Identifier) |k8s|
//@[052:0053) |   |   |   |     | ├─Token(Dot) |.|
//@[053:0059) |   |   |   |     | └─IdentifierSyntax
//@[053:0059) |   |   |   |     |   └─Token(Identifier) |config|
//@[059:0060) |   |   |   |     ├─Token(Dot) |.|
//@[060:0070) |   |   |   |     └─IdentifierSyntax
//@[060:0070) |   |   |   |       └─Token(Identifier) |kubeConfig|
//@[070:0072) |   |   |   ├─Token(NewLine) |\r\n|
      namespace: boolParam1 ? az.resourceGroup().location : k8s.config.namespace
//@[006:0080) |   |   |   ├─ObjectPropertySyntax
//@[006:0015) |   |   |   | ├─IdentifierSyntax
//@[006:0015) |   |   |   | | └─Token(Identifier) |namespace|
//@[015:0016) |   |   |   | ├─Token(Colon) |:|
//@[017:0080) |   |   |   | └─TernaryOperationSyntax
//@[017:0027) |   |   |   |   ├─VariableAccessSyntax
//@[017:0027) |   |   |   |   | └─IdentifierSyntax
//@[017:0027) |   |   |   |   |   └─Token(Identifier) |boolParam1|
//@[028:0029) |   |   |   |   ├─Token(Question) |?|
//@[030:0057) |   |   |   |   ├─PropertyAccessSyntax
//@[030:0048) |   |   |   |   | ├─InstanceFunctionCallSyntax
//@[030:0032) |   |   |   |   | | ├─VariableAccessSyntax
//@[030:0032) |   |   |   |   | | | └─IdentifierSyntax
//@[030:0032) |   |   |   |   | | |   └─Token(Identifier) |az|
//@[032:0033) |   |   |   |   | | ├─Token(Dot) |.|
//@[033:0046) |   |   |   |   | | ├─IdentifierSyntax
//@[033:0046) |   |   |   |   | | | └─Token(Identifier) |resourceGroup|
//@[046:0047) |   |   |   |   | | ├─Token(LeftParen) |(|
//@[047:0048) |   |   |   |   | | └─Token(RightParen) |)|
//@[048:0049) |   |   |   |   | ├─Token(Dot) |.|
//@[049:0057) |   |   |   |   | └─IdentifierSyntax
//@[049:0057) |   |   |   |   |   └─Token(Identifier) |location|
//@[058:0059) |   |   |   |   ├─Token(Colon) |:|
//@[060:0080) |   |   |   |   └─PropertyAccessSyntax
//@[060:0070) |   |   |   |     ├─PropertyAccessSyntax
//@[060:0063) |   |   |   |     | ├─VariableAccessSyntax
//@[060:0063) |   |   |   |     | | └─IdentifierSyntax
//@[060:0063) |   |   |   |     | |   └─Token(Identifier) |k8s|
//@[063:0064) |   |   |   |     | ├─Token(Dot) |.|
//@[064:0070) |   |   |   |     | └─IdentifierSyntax
//@[064:0070) |   |   |   |     |   └─Token(Identifier) |config|
//@[070:0071) |   |   |   |     ├─Token(Dot) |.|
//@[071:0080) |   |   |   |     └─IdentifierSyntax
//@[071:0080) |   |   |   |       └─Token(Identifier) |namespace|
//@[080:0082) |   |   |   ├─Token(NewLine) |\r\n|
    }
//@[004:0005) |   |   |   └─Token(RightBrace) |}|
//@[005:0007) |   |   ├─Token(NewLine) |\r\n|
  }
//@[002:0003) |   |   └─Token(RightBrace) |}|
//@[003:0005) |   ├─Token(NewLine) |\r\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0005) ├─Token(NewLine) |\r\n\r\n|

module moduleWithExtsEmpty 'child/hasConfigurableExtensionsWithAlias.bicep' = {
//@[000:0162) ├─ModuleDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |module|
//@[007:0026) | ├─IdentifierSyntax
//@[007:0026) | | └─Token(Identifier) |moduleWithExtsEmpty|
//@[027:0075) | ├─StringSyntax
//@[027:0075) | | └─Token(StringComplete) |'child/hasConfigurableExtensionsWithAlias.bicep'|
//@[076:0077) | ├─Token(Assignment) |=|
//@[078:0162) | └─ObjectSyntax
//@[078:0079) |   ├─Token(LeftBrace) |{|
//@[079:0081) |   ├─Token(NewLine) |\r\n|
  extensionConfigs: {
//@[002:0078) |   ├─ObjectPropertySyntax
//@[002:0018) |   | ├─IdentifierSyntax
//@[002:0018) |   | | └─Token(Identifier) |extensionConfigs|
//@[018:0019) |   | ├─Token(Colon) |:|
//@[020:0078) |   | └─ObjectSyntax
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
    extWithOptionalConfig: {}
//@[004:0029) |   |   ├─ObjectPropertySyntax
//@[004:0025) |   |   | ├─IdentifierSyntax
//@[004:0025) |   |   | | └─Token(Identifier) |extWithOptionalConfig|
//@[025:0026) |   |   | ├─Token(Colon) |:|
//@[027:0029) |   |   | └─ObjectSyntax
//@[027:0028) |   |   |   ├─Token(LeftBrace) |{|
//@[028:0029) |   |   |   └─Token(RightBrace) |}|
//@[029:0031) |   |   ├─Token(NewLine) |\r\n|
  }
//@[002:0003) |   |   └─Token(RightBrace) |}|
//@[003:0005) |   ├─Token(NewLine) |\r\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0005) ├─Token(NewLine) |\r\n\r\n|

// END: Extension configs for modules
//@[037:0039) ├─Token(NewLine) |\r\n|

//@[000:0000) └─Token(EndOfFile) ||
