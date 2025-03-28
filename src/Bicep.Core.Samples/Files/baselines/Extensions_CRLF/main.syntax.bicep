// BEGIN: Extension declarations
//@[00:1330) ProgramSyntax
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

//extension 'br:mcr.microsoft.com/bicep/extensions/microsoftgraph/v1.0:0.1.8-preview' as graph
//@[94:0098) ├─Token(NewLine) |\r\n\r\n|

// END: Extension declarations
//@[30:0034) ├─Token(NewLine) |\r\n\r\n|

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

// END: Extension configs for modules
//@[37:0039) ├─Token(NewLine) |\r\n|

//@[00:0000) └─Token(EndOfFile) ||
