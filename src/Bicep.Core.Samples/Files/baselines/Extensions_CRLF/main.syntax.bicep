// BEGIN: Parameters
//@[00:3059) ProgramSyntax
//@[20:0024) ├─Token(NewLine) |\r\n\r\n|

param strParam1 string
//@[00:0022) ├─ParameterDeclarationSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0015) | ├─IdentifierSyntax
//@[06:0015) | | └─Token(Identifier) |strParam1|
//@[16:0022) | └─TypeVariableAccessSyntax
//@[16:0022) |   └─IdentifierSyntax
//@[16:0022) |     └─Token(Identifier) |string|
//@[22:0026) ├─Token(NewLine) |\r\n\r\n|

@secure()
//@[00:0039) ├─ParameterDeclarationSyntax
//@[00:0009) | ├─DecoratorSyntax
//@[00:0001) | | ├─Token(At) |@|
//@[01:0009) | | └─FunctionCallSyntax
//@[01:0007) | |   ├─IdentifierSyntax
//@[01:0007) | |   | └─Token(Identifier) |secure|
//@[07:0008) | |   ├─Token(LeftParen) |(|
//@[08:0009) | |   └─Token(RightParen) |)|
//@[09:0011) | ├─Token(NewLine) |\r\n|
param secureStrParam1 string
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0021) | ├─IdentifierSyntax
//@[06:0021) | | └─Token(Identifier) |secureStrParam1|
//@[22:0028) | └─TypeVariableAccessSyntax
//@[22:0028) |   └─IdentifierSyntax
//@[22:0028) |     └─Token(Identifier) |string|
//@[28:0032) ├─Token(NewLine) |\r\n\r\n|

param boolParam1 bool
//@[00:0021) ├─ParameterDeclarationSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0016) | ├─IdentifierSyntax
//@[06:0016) | | └─Token(Identifier) |boolParam1|
//@[17:0021) | └─TypeVariableAccessSyntax
//@[17:0021) |   └─IdentifierSyntax
//@[17:0021) |     └─Token(Identifier) |bool|
//@[21:0025) ├─Token(NewLine) |\r\n\r\n|

// END: Parameters
//@[18:0022) ├─Token(NewLine) |\r\n\r\n|

// BEGIN: Extension declarations
//@[32:0036) ├─Token(NewLine) |\r\n\r\n|

extension kubernetes with {
//@[00:0084) ├─ExtensionDeclarationSyntax
//@[00:0009) | ├─Token(Identifier) |extension|
//@[10:0020) | ├─IdentifierSyntax
//@[10:0020) | | └─Token(Identifier) |kubernetes|
//@[21:0077) | ├─ExtensionWithClauseSyntax
//@[21:0025) | | ├─Token(Identifier) |with|
//@[26:0077) | | └─ObjectSyntax
//@[26:0027) | |   ├─Token(LeftBrace) |{|
//@[27:0029) | |   ├─Token(NewLine) |\r\n|
  kubeConfig: 'DELETE'
//@[02:0022) | |   ├─ObjectPropertySyntax
//@[02:0012) | |   | ├─IdentifierSyntax
//@[02:0012) | |   | | └─Token(Identifier) |kubeConfig|
//@[12:0013) | |   | ├─Token(Colon) |:|
//@[14:0022) | |   | └─StringSyntax
//@[14:0022) | |   |   └─Token(StringComplete) |'DELETE'|
//@[22:0024) | |   ├─Token(NewLine) |\r\n|
  namespace: 'DELETE'
//@[02:0021) | |   ├─ObjectPropertySyntax
//@[02:0011) | |   | ├─IdentifierSyntax
//@[02:0011) | |   | | └─Token(Identifier) |namespace|
//@[11:0012) | |   | ├─Token(Colon) |:|
//@[13:0021) | |   | └─StringSyntax
//@[13:0021) | |   |   └─Token(StringComplete) |'DELETE'|
//@[21:0023) | |   ├─Token(NewLine) |\r\n|
} as k8s
//@[00:0001) | |   └─Token(RightBrace) |}|
//@[02:0008) | └─AliasAsClauseSyntax
//@[02:0004) |   ├─Token(Identifier) |as|
//@[05:0008) |   └─IdentifierSyntax
//@[05:0008) |     └─Token(Identifier) |k8s|
//@[08:0012) ├─Token(NewLine) |\r\n\r\n|

//extension 'br:mcr.microsoft.com/bicep/extensions/microsoftgraph/v1:1.2.3' as graph
//@[84:0088) ├─Token(NewLine) |\r\n\r\n|

// END: Extension declarations
//@[30:0034) ├─Token(NewLine) |\r\n\r\n|

// BEGIN: Key vaults
//@[20:0024) ├─Token(NewLine) |\r\n\r\n|

resource kv1 'Microsoft.KeyVault/vaults@2019-09-01' existing = {
//@[00:0082) ├─ResourceDeclarationSyntax
//@[00:0008) | ├─Token(Identifier) |resource|
//@[09:0012) | ├─IdentifierSyntax
//@[09:0012) | | └─Token(Identifier) |kv1|
//@[13:0051) | ├─StringSyntax
//@[13:0051) | | └─Token(StringComplete) |'Microsoft.KeyVault/vaults@2019-09-01'|
//@[52:0060) | ├─Token(Identifier) |existing|
//@[61:0062) | ├─Token(Assignment) |=|
//@[63:0082) | └─ObjectSyntax
//@[63:0064) |   ├─Token(LeftBrace) |{|
//@[64:0066) |   ├─Token(NewLine) |\r\n|
  name: 'kv1'
//@[02:0013) |   ├─ObjectPropertySyntax
//@[02:0006) |   | ├─IdentifierSyntax
//@[02:0006) |   | | └─Token(Identifier) |name|
//@[06:0007) |   | ├─Token(Colon) |:|
//@[08:0013) |   | └─StringSyntax
//@[08:0013) |   |   └─Token(StringComplete) |'kv1'|
//@[13:0015) |   ├─Token(NewLine) |\r\n|
}
//@[00:0001) |   └─Token(RightBrace) |}|
//@[01:0005) ├─Token(NewLine) |\r\n\r\n|

resource scopedKv1 'Microsoft.KeyVault/vaults@2019-09-01' existing = {
//@[00:0132) ├─ResourceDeclarationSyntax
//@[00:0008) | ├─Token(Identifier) |resource|
//@[09:0018) | ├─IdentifierSyntax
//@[09:0018) | | └─Token(Identifier) |scopedKv1|
//@[19:0057) | ├─StringSyntax
//@[19:0057) | | └─Token(StringComplete) |'Microsoft.KeyVault/vaults@2019-09-01'|
//@[58:0066) | ├─Token(Identifier) |existing|
//@[67:0068) | ├─Token(Assignment) |=|
//@[69:0132) | └─ObjectSyntax
//@[69:0070) |   ├─Token(LeftBrace) |{|
//@[70:0072) |   ├─Token(NewLine) |\r\n|
  name: 'scopedKv1'
//@[02:0019) |   ├─ObjectPropertySyntax
//@[02:0006) |   | ├─IdentifierSyntax
//@[02:0006) |   | | └─Token(Identifier) |name|
//@[06:0007) |   | ├─Token(Colon) |:|
//@[08:0019) |   | └─StringSyntax
//@[08:0019) |   |   └─Token(StringComplete) |'scopedKv1'|
//@[19:0021) |   ├─Token(NewLine) |\r\n|
  scope: resourceGroup('otherGroup')
//@[02:0036) |   ├─ObjectPropertySyntax
//@[02:0007) |   | ├─IdentifierSyntax
//@[02:0007) |   | | └─Token(Identifier) |scope|
//@[07:0008) |   | ├─Token(Colon) |:|
//@[09:0036) |   | └─FunctionCallSyntax
//@[09:0022) |   |   ├─IdentifierSyntax
//@[09:0022) |   |   | └─Token(Identifier) |resourceGroup|
//@[22:0023) |   |   ├─Token(LeftParen) |(|
//@[23:0035) |   |   ├─FunctionArgumentSyntax
//@[23:0035) |   |   | └─StringSyntax
//@[23:0035) |   |   |   └─Token(StringComplete) |'otherGroup'|
//@[35:0036) |   |   └─Token(RightParen) |)|
//@[36:0038) |   ├─Token(NewLine) |\r\n|
}
//@[00:0001) |   └─Token(RightBrace) |}|
//@[01:0005) ├─Token(NewLine) |\r\n\r\n|

resource testResource1 'az:My.Rp/TestType@2020-01-01' = {
//@[00:0147) ├─ResourceDeclarationSyntax
//@[00:0008) | ├─Token(Identifier) |resource|
//@[09:0022) | ├─IdentifierSyntax
//@[09:0022) | | └─Token(Identifier) |testResource1|
//@[23:0053) | ├─StringSyntax
//@[23:0053) | | └─Token(StringComplete) |'az:My.Rp/TestType@2020-01-01'|
//@[54:0055) | ├─Token(Assignment) |=|
//@[56:0147) | └─ObjectSyntax
//@[56:0057) |   ├─Token(LeftBrace) |{|
//@[57:0059) |   ├─Token(NewLine) |\r\n|
  name: k8s.config.namespace
//@[02:0028) |   ├─ObjectPropertySyntax
//@[02:0006) |   | ├─IdentifierSyntax
//@[02:0006) |   | | └─Token(Identifier) |name|
//@[06:0007) |   | ├─Token(Colon) |:|
//@[08:0028) |   | └─PropertyAccessSyntax
//@[08:0018) |   |   ├─PropertyAccessSyntax
//@[08:0011) |   |   | ├─VariableAccessSyntax
//@[08:0011) |   |   | | └─IdentifierSyntax
//@[08:0011) |   |   | |   └─Token(Identifier) |k8s|
//@[11:0012) |   |   | ├─Token(Dot) |.|
//@[12:0018) |   |   | └─IdentifierSyntax
//@[12:0018) |   |   |   └─Token(Identifier) |config|
//@[18:0019) |   |   ├─Token(Dot) |.|
//@[19:0028) |   |   └─IdentifierSyntax
//@[19:0028) |   |     └─Token(Identifier) |namespace|
//@[28:0030) |   ├─Token(NewLine) |\r\n|
  properties: {
//@[02:0055) |   ├─ObjectPropertySyntax
//@[02:0012) |   | ├─IdentifierSyntax
//@[02:0012) |   | | └─Token(Identifier) |properties|
//@[12:0013) |   | ├─Token(Colon) |:|
//@[14:0055) |   | └─ObjectSyntax
//@[14:0015) |   |   ├─Token(LeftBrace) |{|
//@[15:0017) |   |   ├─Token(NewLine) |\r\n|
    secret: k8s.config.kubeConfig
//@[04:0033) |   |   ├─ObjectPropertySyntax
//@[04:0010) |   |   | ├─IdentifierSyntax
//@[04:0010) |   |   | | └─Token(Identifier) |secret|
//@[10:0011) |   |   | ├─Token(Colon) |:|
//@[12:0033) |   |   | └─PropertyAccessSyntax
//@[12:0022) |   |   |   ├─PropertyAccessSyntax
//@[12:0015) |   |   |   | ├─VariableAccessSyntax
//@[12:0015) |   |   |   | | └─IdentifierSyntax
//@[12:0015) |   |   |   | |   └─Token(Identifier) |k8s|
//@[15:0016) |   |   |   | ├─Token(Dot) |.|
//@[16:0022) |   |   |   | └─IdentifierSyntax
//@[16:0022) |   |   |   |   └─Token(Identifier) |config|
//@[22:0023) |   |   |   ├─Token(Dot) |.|
//@[23:0033) |   |   |   └─IdentifierSyntax
//@[23:0033) |   |   |     └─Token(Identifier) |kubeConfig|
//@[33:0035) |   |   ├─Token(NewLine) |\r\n|
  }
//@[02:0003) |   |   └─Token(RightBrace) |}|
//@[03:0005) |   ├─Token(NewLine) |\r\n|
}
//@[00:0001) |   └─Token(RightBrace) |}|
//@[01:0005) ├─Token(NewLine) |\r\n\r\n|

// END: Key vaults
//@[18:0022) ├─Token(NewLine) |\r\n\r\n|

// BEGIN: Extension configs for modules
//@[39:0043) ├─Token(NewLine) |\r\n\r\n|

module moduleWithExtsWithAliases 'child/hasConfigurableExtensionsWithAlias.bicep' = {
//@[00:0249) ├─ModuleDeclarationSyntax
//@[00:0006) | ├─Token(Identifier) |module|
//@[07:0032) | ├─IdentifierSyntax
//@[07:0032) | | └─Token(Identifier) |moduleWithExtsWithAliases|
//@[33:0081) | ├─StringSyntax
//@[33:0081) | | └─Token(StringComplete) |'child/hasConfigurableExtensionsWithAlias.bicep'|
//@[82:0083) | ├─Token(Assignment) |=|
//@[84:0249) | └─ObjectSyntax
//@[84:0085) |   ├─Token(LeftBrace) |{|
//@[85:0087) |   ├─Token(NewLine) |\r\n|
  name: 'moduleWithExtsWithAliases'
//@[02:0035) |   ├─ObjectPropertySyntax
//@[02:0006) |   | ├─IdentifierSyntax
//@[02:0006) |   | | └─Token(Identifier) |name|
//@[06:0007) |   | ├─Token(Colon) |:|
//@[08:0035) |   | └─StringSyntax
//@[08:0035) |   |   └─Token(StringComplete) |'moduleWithExtsWithAliases'|
//@[35:0037) |   ├─Token(NewLine) |\r\n|
  extensionConfigs: {
//@[02:0122) |   ├─ObjectPropertySyntax
//@[02:0018) |   | ├─IdentifierSyntax
//@[02:0018) |   | | └─Token(Identifier) |extensionConfigs|
//@[18:0019) |   | ├─Token(Colon) |:|
//@[20:0122) |   | └─ObjectSyntax
//@[20:0021) |   |   ├─Token(LeftBrace) |{|
//@[21:0023) |   |   ├─Token(NewLine) |\r\n|
    k8s: {
//@[04:0094) |   |   ├─ObjectPropertySyntax
//@[04:0007) |   |   | ├─IdentifierSyntax
//@[04:0007) |   |   | | └─Token(Identifier) |k8s|
//@[07:0008) |   |   | ├─Token(Colon) |:|
//@[09:0094) |   |   | └─ObjectSyntax
//@[09:0010) |   |   |   ├─Token(LeftBrace) |{|
//@[10:0012) |   |   |   ├─Token(NewLine) |\r\n|
      kubeConfig: 'kubeConfig2FromModule'
//@[06:0041) |   |   |   ├─ObjectPropertySyntax
//@[06:0016) |   |   |   | ├─IdentifierSyntax
//@[06:0016) |   |   |   | | └─Token(Identifier) |kubeConfig|
//@[16:0017) |   |   |   | ├─Token(Colon) |:|
//@[18:0041) |   |   |   | └─StringSyntax
//@[18:0041) |   |   |   |   └─Token(StringComplete) |'kubeConfig2FromModule'|
//@[41:0043) |   |   |   ├─Token(NewLine) |\r\n|
      namespace: 'ns2FromModule'
//@[06:0032) |   |   |   ├─ObjectPropertySyntax
//@[06:0015) |   |   |   | ├─IdentifierSyntax
//@[06:0015) |   |   |   | | └─Token(Identifier) |namespace|
//@[15:0016) |   |   |   | ├─Token(Colon) |:|
//@[17:0032) |   |   |   | └─StringSyntax
//@[17:0032) |   |   |   |   └─Token(StringComplete) |'ns2FromModule'|
//@[32:0034) |   |   |   ├─Token(NewLine) |\r\n|
    }
//@[04:0005) |   |   |   └─Token(RightBrace) |}|
//@[05:0007) |   |   ├─Token(NewLine) |\r\n|
  }
//@[02:0003) |   |   └─Token(RightBrace) |}|
//@[03:0005) |   ├─Token(NewLine) |\r\n|
}
//@[00:0001) |   └─Token(RightBrace) |}|
//@[01:0005) ├─Token(NewLine) |\r\n\r\n|

module moduleWithExtsWithoutAliases 'child/hasConfigurableExtensionsWithoutAlias.bicep' = {
//@[00:0265) ├─ModuleDeclarationSyntax
//@[00:0006) | ├─Token(Identifier) |module|
//@[07:0035) | ├─IdentifierSyntax
//@[07:0035) | | └─Token(Identifier) |moduleWithExtsWithoutAliases|
//@[36:0087) | ├─StringSyntax
//@[36:0087) | | └─Token(StringComplete) |'child/hasConfigurableExtensionsWithoutAlias.bicep'|
//@[88:0089) | ├─Token(Assignment) |=|
//@[90:0265) | └─ObjectSyntax
//@[90:0091) |   ├─Token(LeftBrace) |{|
//@[91:0093) |   ├─Token(NewLine) |\r\n|
  name: 'moduleWithExtsWithoutAliases'
//@[02:0038) |   ├─ObjectPropertySyntax
//@[02:0006) |   | ├─IdentifierSyntax
//@[02:0006) |   | | └─Token(Identifier) |name|
//@[06:0007) |   | ├─Token(Colon) |:|
//@[08:0038) |   | └─StringSyntax
//@[08:0038) |   |   └─Token(StringComplete) |'moduleWithExtsWithoutAliases'|
//@[38:0040) |   ├─Token(NewLine) |\r\n|
  extensionConfigs: {
//@[02:0129) |   ├─ObjectPropertySyntax
//@[02:0018) |   | ├─IdentifierSyntax
//@[02:0018) |   | | └─Token(Identifier) |extensionConfigs|
//@[18:0019) |   | ├─Token(Colon) |:|
//@[20:0129) |   | └─ObjectSyntax
//@[20:0021) |   |   ├─Token(LeftBrace) |{|
//@[21:0023) |   |   ├─Token(NewLine) |\r\n|
    kubernetes: {
//@[04:0101) |   |   ├─ObjectPropertySyntax
//@[04:0014) |   |   | ├─IdentifierSyntax
//@[04:0014) |   |   | | └─Token(Identifier) |kubernetes|
//@[14:0015) |   |   | ├─Token(Colon) |:|
//@[16:0101) |   |   | └─ObjectSyntax
//@[16:0017) |   |   |   ├─Token(LeftBrace) |{|
//@[17:0019) |   |   |   ├─Token(NewLine) |\r\n|
      kubeConfig: 'kubeConfig2FromModule'
//@[06:0041) |   |   |   ├─ObjectPropertySyntax
//@[06:0016) |   |   |   | ├─IdentifierSyntax
//@[06:0016) |   |   |   | | └─Token(Identifier) |kubeConfig|
//@[16:0017) |   |   |   | ├─Token(Colon) |:|
//@[18:0041) |   |   |   | └─StringSyntax
//@[18:0041) |   |   |   |   └─Token(StringComplete) |'kubeConfig2FromModule'|
//@[41:0043) |   |   |   ├─Token(NewLine) |\r\n|
      namespace: 'ns2FromModule'
//@[06:0032) |   |   |   ├─ObjectPropertySyntax
//@[06:0015) |   |   |   | ├─IdentifierSyntax
//@[06:0015) |   |   |   | | └─Token(Identifier) |namespace|
//@[15:0016) |   |   |   | ├─Token(Colon) |:|
//@[17:0032) |   |   |   | └─StringSyntax
//@[17:0032) |   |   |   |   └─Token(StringComplete) |'ns2FromModule'|
//@[32:0034) |   |   |   ├─Token(NewLine) |\r\n|
    }
//@[04:0005) |   |   |   └─Token(RightBrace) |}|
//@[05:0007) |   |   ├─Token(NewLine) |\r\n|
  }
//@[02:0003) |   |   └─Token(RightBrace) |}|
//@[03:0005) |   ├─Token(NewLine) |\r\n|
}
//@[00:0001) |   └─Token(RightBrace) |}|
//@[01:0005) ├─Token(NewLine) |\r\n\r\n|

module moduleExtConfigsFromParams 'child/hasConfigurableExtensionsWithAlias.bicep' = {
//@[00:0289) ├─ModuleDeclarationSyntax
//@[00:0006) | ├─Token(Identifier) |module|
//@[07:0033) | ├─IdentifierSyntax
//@[07:0033) | | └─Token(Identifier) |moduleExtConfigsFromParams|
//@[34:0082) | ├─StringSyntax
//@[34:0082) | | └─Token(StringComplete) |'child/hasConfigurableExtensionsWithAlias.bicep'|
//@[83:0084) | ├─Token(Assignment) |=|
//@[85:0289) | └─ObjectSyntax
//@[85:0086) |   ├─Token(LeftBrace) |{|
//@[86:0088) |   ├─Token(NewLine) |\r\n|
  name: 'moduleExtConfigsFromParams'
//@[02:0036) |   ├─ObjectPropertySyntax
//@[02:0006) |   | ├─IdentifierSyntax
//@[02:0006) |   | | └─Token(Identifier) |name|
//@[06:0007) |   | ├─Token(Colon) |:|
//@[08:0036) |   | └─StringSyntax
//@[08:0036) |   |   └─Token(StringComplete) |'moduleExtConfigsFromParams'|
//@[36:0038) |   ├─Token(NewLine) |\r\n|
  extensionConfigs: {
//@[02:0160) |   ├─ObjectPropertySyntax
//@[02:0018) |   | ├─IdentifierSyntax
//@[02:0018) |   | | └─Token(Identifier) |extensionConfigs|
//@[18:0019) |   | ├─Token(Colon) |:|
//@[20:0160) |   | └─ObjectSyntax
//@[20:0021) |   |   ├─Token(LeftBrace) |{|
//@[21:0023) |   |   ├─Token(NewLine) |\r\n|
    k8s: {
//@[04:0132) |   |   ├─ObjectPropertySyntax
//@[04:0007) |   |   | ├─IdentifierSyntax
//@[04:0007) |   |   | | └─Token(Identifier) |k8s|
//@[07:0008) |   |   | ├─Token(Colon) |:|
//@[09:0132) |   |   | └─ObjectSyntax
//@[09:0010) |   |   |   ├─Token(LeftBrace) |{|
//@[10:0012) |   |   |   ├─Token(NewLine) |\r\n|
      kubeConfig: boolParam1 ? secureStrParam1 : strParam1
//@[06:0058) |   |   |   ├─ObjectPropertySyntax
//@[06:0016) |   |   |   | ├─IdentifierSyntax
//@[06:0016) |   |   |   | | └─Token(Identifier) |kubeConfig|
//@[16:0017) |   |   |   | ├─Token(Colon) |:|
//@[18:0058) |   |   |   | └─TernaryOperationSyntax
//@[18:0028) |   |   |   |   ├─VariableAccessSyntax
//@[18:0028) |   |   |   |   | └─IdentifierSyntax
//@[18:0028) |   |   |   |   |   └─Token(Identifier) |boolParam1|
//@[29:0030) |   |   |   |   ├─Token(Question) |?|
//@[31:0046) |   |   |   |   ├─VariableAccessSyntax
//@[31:0046) |   |   |   |   | └─IdentifierSyntax
//@[31:0046) |   |   |   |   |   └─Token(Identifier) |secureStrParam1|
//@[47:0048) |   |   |   |   ├─Token(Colon) |:|
//@[49:0058) |   |   |   |   └─VariableAccessSyntax
//@[49:0058) |   |   |   |     └─IdentifierSyntax
//@[49:0058) |   |   |   |       └─Token(Identifier) |strParam1|
//@[58:0060) |   |   |   ├─Token(NewLine) |\r\n|
      namespace: boolParam1 ? strParam1 : 'falseCond'
//@[06:0053) |   |   |   ├─ObjectPropertySyntax
//@[06:0015) |   |   |   | ├─IdentifierSyntax
//@[06:0015) |   |   |   | | └─Token(Identifier) |namespace|
//@[15:0016) |   |   |   | ├─Token(Colon) |:|
//@[17:0053) |   |   |   | └─TernaryOperationSyntax
//@[17:0027) |   |   |   |   ├─VariableAccessSyntax
//@[17:0027) |   |   |   |   | └─IdentifierSyntax
//@[17:0027) |   |   |   |   |   └─Token(Identifier) |boolParam1|
//@[28:0029) |   |   |   |   ├─Token(Question) |?|
//@[30:0039) |   |   |   |   ├─VariableAccessSyntax
//@[30:0039) |   |   |   |   | └─IdentifierSyntax
//@[30:0039) |   |   |   |   |   └─Token(Identifier) |strParam1|
//@[40:0041) |   |   |   |   ├─Token(Colon) |:|
//@[42:0053) |   |   |   |   └─StringSyntax
//@[42:0053) |   |   |   |     └─Token(StringComplete) |'falseCond'|
//@[53:0055) |   |   |   ├─Token(NewLine) |\r\n|
    }
//@[04:0005) |   |   |   └─Token(RightBrace) |}|
//@[05:0007) |   |   ├─Token(NewLine) |\r\n|
  }
//@[02:0003) |   |   └─Token(RightBrace) |}|
//@[03:0005) |   ├─Token(NewLine) |\r\n|
}
//@[00:0001) |   └─Token(RightBrace) |}|
//@[01:0005) ├─Token(NewLine) |\r\n\r\n|

// TODO(kylealbert): Allow key vault references in extension configs
//@[68:0070) ├─Token(NewLine) |\r\n|
// module moduleExtConfigFromKeyVaultReference 'child/hasConfigurableExtensionsWithAlias.bicep' = {
//@[99:0101) ├─Token(NewLine) |\r\n|
//   name: 'moduleExtConfigKeyVaultReference'
//@[45:0047) ├─Token(NewLine) |\r\n|
//   extensionConfigs: {
//@[24:0026) ├─Token(NewLine) |\r\n|
//     k8s: {
//@[13:0015) ├─Token(NewLine) |\r\n|
//       kubeConfig: kv1.getSecret('myKubeConfig'),
//@[51:0053) ├─Token(NewLine) |\r\n|
//       namespace: scopedKv1.getSecret('myNamespace')
//@[54:0056) ├─Token(NewLine) |\r\n|
//     }
//@[08:0010) ├─Token(NewLine) |\r\n|
//   }
//@[06:0008) ├─Token(NewLine) |\r\n|
// }
//@[04:0008) ├─Token(NewLine) |\r\n\r\n|

module moduleWithExtsUsingFullInheritance 'child/hasConfigurableExtensionsWithAlias.bicep' = {
//@[00:0187) ├─ModuleDeclarationSyntax
//@[00:0006) | ├─Token(Identifier) |module|
//@[07:0041) | ├─IdentifierSyntax
//@[07:0041) | | └─Token(Identifier) |moduleWithExtsUsingFullInheritance|
//@[42:0090) | ├─StringSyntax
//@[42:0090) | | └─Token(StringComplete) |'child/hasConfigurableExtensionsWithAlias.bicep'|
//@[91:0092) | ├─Token(Assignment) |=|
//@[93:0187) | └─ObjectSyntax
//@[93:0094) |   ├─Token(LeftBrace) |{|
//@[94:0096) |   ├─Token(NewLine) |\r\n|
  name: 'moduleWithExtsFullInheritance'
//@[02:0039) |   ├─ObjectPropertySyntax
//@[02:0006) |   | ├─IdentifierSyntax
//@[02:0006) |   | | └─Token(Identifier) |name|
//@[06:0007) |   | ├─Token(Colon) |:|
//@[08:0039) |   | └─StringSyntax
//@[08:0039) |   |   └─Token(StringComplete) |'moduleWithExtsFullInheritance'|
//@[39:0041) |   ├─Token(NewLine) |\r\n|
  extensionConfigs: {
//@[02:0047) |   ├─ObjectPropertySyntax
//@[02:0018) |   | ├─IdentifierSyntax
//@[02:0018) |   | | └─Token(Identifier) |extensionConfigs|
//@[18:0019) |   | ├─Token(Colon) |:|
//@[20:0047) |   | └─ObjectSyntax
//@[20:0021) |   |   ├─Token(LeftBrace) |{|
//@[21:0023) |   |   ├─Token(NewLine) |\r\n|
    k8s: k8s.config
//@[04:0019) |   |   ├─ObjectPropertySyntax
//@[04:0007) |   |   | ├─IdentifierSyntax
//@[04:0007) |   |   | | └─Token(Identifier) |k8s|
//@[07:0008) |   |   | ├─Token(Colon) |:|
//@[09:0019) |   |   | └─PropertyAccessSyntax
//@[09:0012) |   |   |   ├─VariableAccessSyntax
//@[09:0012) |   |   |   | └─IdentifierSyntax
//@[09:0012) |   |   |   |   └─Token(Identifier) |k8s|
//@[12:0013) |   |   |   ├─Token(Dot) |.|
//@[13:0019) |   |   |   └─IdentifierSyntax
//@[13:0019) |   |   |     └─Token(Identifier) |config|
//@[19:0021) |   |   ├─Token(NewLine) |\r\n|
  }
//@[02:0003) |   |   └─Token(RightBrace) |}|
//@[03:0005) |   ├─Token(NewLine) |\r\n|
}
//@[00:0001) |   └─Token(RightBrace) |}|
//@[01:0005) ├─Token(NewLine) |\r\n\r\n|

module moduleWithExtsUsingPiecemealInheritance 'child/hasConfigurableExtensionsWithAlias.bicep' = {
//@[00:0275) ├─ModuleDeclarationSyntax
//@[00:0006) | ├─Token(Identifier) |module|
//@[07:0046) | ├─IdentifierSyntax
//@[07:0046) | | └─Token(Identifier) |moduleWithExtsUsingPiecemealInheritance|
//@[47:0095) | ├─StringSyntax
//@[47:0095) | | └─Token(StringComplete) |'child/hasConfigurableExtensionsWithAlias.bicep'|
//@[96:0097) | ├─Token(Assignment) |=|
//@[98:0275) | └─ObjectSyntax
//@[98:0099) |   ├─Token(LeftBrace) |{|
//@[99:0101) |   ├─Token(NewLine) |\r\n|
  name: 'moduleWithExtsPiecemealInheritance'
//@[02:0044) |   ├─ObjectPropertySyntax
//@[02:0006) |   | ├─IdentifierSyntax
//@[02:0006) |   | | └─Token(Identifier) |name|
//@[06:0007) |   | ├─Token(Colon) |:|
//@[08:0044) |   | └─StringSyntax
//@[08:0044) |   |   └─Token(StringComplete) |'moduleWithExtsPiecemealInheritance'|
//@[44:0046) |   ├─Token(NewLine) |\r\n|
  extensionConfigs: {
//@[02:0125) |   ├─ObjectPropertySyntax
//@[02:0018) |   | ├─IdentifierSyntax
//@[02:0018) |   | | └─Token(Identifier) |extensionConfigs|
//@[18:0019) |   | ├─Token(Colon) |:|
//@[20:0125) |   | └─ObjectSyntax
//@[20:0021) |   |   ├─Token(LeftBrace) |{|
//@[21:0023) |   |   ├─Token(NewLine) |\r\n|
    k8s: {
//@[04:0097) |   |   ├─ObjectPropertySyntax
//@[04:0007) |   |   | ├─IdentifierSyntax
//@[04:0007) |   |   | | └─Token(Identifier) |k8s|
//@[07:0008) |   |   | ├─Token(Colon) |:|
//@[09:0097) |   |   | └─ObjectSyntax
//@[09:0010) |   |   |   ├─Token(LeftBrace) |{|
//@[10:0012) |   |   |   ├─Token(NewLine) |\r\n|
      kubeConfig: k8s.config.kubeConfig
//@[06:0039) |   |   |   ├─ObjectPropertySyntax
//@[06:0016) |   |   |   | ├─IdentifierSyntax
//@[06:0016) |   |   |   | | └─Token(Identifier) |kubeConfig|
//@[16:0017) |   |   |   | ├─Token(Colon) |:|
//@[18:0039) |   |   |   | └─PropertyAccessSyntax
//@[18:0028) |   |   |   |   ├─PropertyAccessSyntax
//@[18:0021) |   |   |   |   | ├─VariableAccessSyntax
//@[18:0021) |   |   |   |   | | └─IdentifierSyntax
//@[18:0021) |   |   |   |   | |   └─Token(Identifier) |k8s|
//@[21:0022) |   |   |   |   | ├─Token(Dot) |.|
//@[22:0028) |   |   |   |   | └─IdentifierSyntax
//@[22:0028) |   |   |   |   |   └─Token(Identifier) |config|
//@[28:0029) |   |   |   |   ├─Token(Dot) |.|
//@[29:0039) |   |   |   |   └─IdentifierSyntax
//@[29:0039) |   |   |   |     └─Token(Identifier) |kubeConfig|
//@[39:0041) |   |   |   ├─Token(NewLine) |\r\n|
      namespace: k8s.config.namespace
//@[06:0037) |   |   |   ├─ObjectPropertySyntax
//@[06:0015) |   |   |   | ├─IdentifierSyntax
//@[06:0015) |   |   |   | | └─Token(Identifier) |namespace|
//@[15:0016) |   |   |   | ├─Token(Colon) |:|
//@[17:0037) |   |   |   | └─PropertyAccessSyntax
//@[17:0027) |   |   |   |   ├─PropertyAccessSyntax
//@[17:0020) |   |   |   |   | ├─VariableAccessSyntax
//@[17:0020) |   |   |   |   | | └─IdentifierSyntax
//@[17:0020) |   |   |   |   | |   └─Token(Identifier) |k8s|
//@[20:0021) |   |   |   |   | ├─Token(Dot) |.|
//@[21:0027) |   |   |   |   | └─IdentifierSyntax
//@[21:0027) |   |   |   |   |   └─Token(Identifier) |config|
//@[27:0028) |   |   |   |   ├─Token(Dot) |.|
//@[28:0037) |   |   |   |   └─IdentifierSyntax
//@[28:0037) |   |   |   |     └─Token(Identifier) |namespace|
//@[37:0039) |   |   |   ├─Token(NewLine) |\r\n|
    }
//@[04:0005) |   |   |   └─Token(RightBrace) |}|
//@[05:0007) |   |   ├─Token(NewLine) |\r\n|
  }
//@[02:0003) |   |   └─Token(RightBrace) |}|
//@[03:0005) |   ├─Token(NewLine) |\r\n|
}
//@[00:0001) |   └─Token(RightBrace) |}|
//@[01:0005) ├─Token(NewLine) |\r\n\r\n|

// TODO(kylealbert): Figure out if this is allowable
//@[52:0054) ├─Token(NewLine) |\r\n|
// var k8sConfigDeployTime = {
//@[30:0032) ├─Token(NewLine) |\r\n|
//   kubeConfig: k8s.config.kubeConfig
//@[38:0040) ├─Token(NewLine) |\r\n|
//   namespace: strParam1
//@[25:0027) ├─Token(NewLine) |\r\n|
// }
//@[04:0008) ├─Token(NewLine) |\r\n\r\n|

// module moduleWithExtsUsingVar 'child/hasConfigurableExtensionsWithAlias.bicep' = {
//@[85:0087) ├─Token(NewLine) |\r\n|
//   name: 'moduleWithExtsUsingVar'
//@[35:0037) ├─Token(NewLine) |\r\n|
//   extensionConfigs: {
//@[24:0026) ├─Token(NewLine) |\r\n|
//     k8s: k8sConfigDeployTime
//@[31:0033) ├─Token(NewLine) |\r\n|
//   }
//@[06:0008) ├─Token(NewLine) |\r\n|
// }
//@[04:0008) ├─Token(NewLine) |\r\n\r\n|

// END: Extension configs for modules
//@[37:0041) ├─Token(NewLine) |\r\n\r\n|

// BEGIN: Outputs
//@[17:0021) ├─Token(NewLine) |\r\n\r\n|

output k8sConfig object = k8s.config
//@[00:0036) ├─OutputDeclarationSyntax
//@[00:0006) | ├─Token(Identifier) |output|
//@[07:0016) | ├─IdentifierSyntax
//@[07:0016) | | └─Token(Identifier) |k8sConfig|
//@[17:0023) | ├─TypeVariableAccessSyntax
//@[17:0023) | | └─IdentifierSyntax
//@[17:0023) | |   └─Token(Identifier) |object|
//@[24:0025) | ├─Token(Assignment) |=|
//@[26:0036) | └─PropertyAccessSyntax
//@[26:0029) |   ├─VariableAccessSyntax
//@[26:0029) |   | └─IdentifierSyntax
//@[26:0029) |   |   └─Token(Identifier) |k8s|
//@[29:0030) |   ├─Token(Dot) |.|
//@[30:0036) |   └─IdentifierSyntax
//@[30:0036) |     └─Token(Identifier) |config|
//@[36:0040) ├─Token(NewLine) |\r\n\r\n|

output k8sNamespace string = k8s.config.namespace
//@[00:0049) ├─OutputDeclarationSyntax
//@[00:0006) | ├─Token(Identifier) |output|
//@[07:0019) | ├─IdentifierSyntax
//@[07:0019) | | └─Token(Identifier) |k8sNamespace|
//@[20:0026) | ├─TypeVariableAccessSyntax
//@[20:0026) | | └─IdentifierSyntax
//@[20:0026) | |   └─Token(Identifier) |string|
//@[27:0028) | ├─Token(Assignment) |=|
//@[29:0049) | └─PropertyAccessSyntax
//@[29:0039) |   ├─PropertyAccessSyntax
//@[29:0032) |   | ├─VariableAccessSyntax
//@[29:0032) |   | | └─IdentifierSyntax
//@[29:0032) |   | |   └─Token(Identifier) |k8s|
//@[32:0033) |   | ├─Token(Dot) |.|
//@[33:0039) |   | └─IdentifierSyntax
//@[33:0039) |   |   └─Token(Identifier) |config|
//@[39:0040) |   ├─Token(Dot) |.|
//@[40:0049) |   └─IdentifierSyntax
//@[40:0049) |     └─Token(Identifier) |namespace|
//@[49:0053) ├─Token(NewLine) |\r\n\r\n|

// END: Outputs
//@[15:0017) ├─Token(NewLine) |\r\n|

//@[00:0000) └─Token(EndOfFile) ||
