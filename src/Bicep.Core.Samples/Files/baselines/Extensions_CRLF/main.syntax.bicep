// BEGIN: Extension declarations
//@[00:872) ProgramSyntax
//@[32:036) ├─Token(NewLine) |\r\n\r\n|

// extension kubernetes with {
//@[30:032) ├─Token(NewLine) |\r\n|
//   kubeConfig: 'DELETE'
//@[25:027) ├─Token(NewLine) |\r\n|
//   namespace: 'DELETE'
//@[24:026) ├─Token(NewLine) |\r\n|
// } as k8s
//@[11:015) ├─Token(NewLine) |\r\n\r\n|

//extension 'br:mcr.microsoft.com/bicep/extensions/microsoftgraph/v1.0:0.1.8-preview' as graph
//@[94:098) ├─Token(NewLine) |\r\n\r\n|

// END: Extension declarations
//@[30:034) ├─Token(NewLine) |\r\n\r\n|

// BEGIN: Extension configs for modules
//@[39:043) ├─Token(NewLine) |\r\n\r\n|

module moduleWithExtsWithAliases 'child/hasConfigurableExtensionsWithAlias.bicep' = {
//@[00:249) ├─ModuleDeclarationSyntax
//@[00:006) | ├─Token(Identifier) |module|
//@[07:032) | ├─IdentifierSyntax
//@[07:032) | | └─Token(Identifier) |moduleWithExtsWithAliases|
//@[33:081) | ├─StringSyntax
//@[33:081) | | └─Token(StringComplete) |'child/hasConfigurableExtensionsWithAlias.bicep'|
//@[82:083) | ├─Token(Assignment) |=|
//@[84:249) | └─ObjectSyntax
//@[84:085) |   ├─Token(LeftBrace) |{|
//@[85:087) |   ├─Token(NewLine) |\r\n|
  name: 'moduleWithExtsWithAliases'
//@[02:035) |   ├─ObjectPropertySyntax
//@[02:006) |   | ├─IdentifierSyntax
//@[02:006) |   | | └─Token(Identifier) |name|
//@[06:007) |   | ├─Token(Colon) |:|
//@[08:035) |   | └─StringSyntax
//@[08:035) |   |   └─Token(StringComplete) |'moduleWithExtsWithAliases'|
//@[35:037) |   ├─Token(NewLine) |\r\n|
  extensionConfigs: {
//@[02:122) |   ├─ObjectPropertySyntax
//@[02:018) |   | ├─IdentifierSyntax
//@[02:018) |   | | └─Token(Identifier) |extensionConfigs|
//@[18:019) |   | ├─Token(Colon) |:|
//@[20:122) |   | └─ObjectSyntax
//@[20:021) |   |   ├─Token(LeftBrace) |{|
//@[21:023) |   |   ├─Token(NewLine) |\r\n|
    k8s: {
//@[04:094) |   |   ├─ObjectPropertySyntax
//@[04:007) |   |   | ├─IdentifierSyntax
//@[04:007) |   |   | | └─Token(Identifier) |k8s|
//@[07:008) |   |   | ├─Token(Colon) |:|
//@[09:094) |   |   | └─ObjectSyntax
//@[09:010) |   |   |   ├─Token(LeftBrace) |{|
//@[10:012) |   |   |   ├─Token(NewLine) |\r\n|
      kubeConfig: 'kubeConfig2FromModule'
//@[06:041) |   |   |   ├─ObjectPropertySyntax
//@[06:016) |   |   |   | ├─IdentifierSyntax
//@[06:016) |   |   |   | | └─Token(Identifier) |kubeConfig|
//@[16:017) |   |   |   | ├─Token(Colon) |:|
//@[18:041) |   |   |   | └─StringSyntax
//@[18:041) |   |   |   |   └─Token(StringComplete) |'kubeConfig2FromModule'|
//@[41:043) |   |   |   ├─Token(NewLine) |\r\n|
      namespace: 'ns2FromModule'
//@[06:032) |   |   |   ├─ObjectPropertySyntax
//@[06:015) |   |   |   | ├─IdentifierSyntax
//@[06:015) |   |   |   | | └─Token(Identifier) |namespace|
//@[15:016) |   |   |   | ├─Token(Colon) |:|
//@[17:032) |   |   |   | └─StringSyntax
//@[17:032) |   |   |   |   └─Token(StringComplete) |'ns2FromModule'|
//@[32:034) |   |   |   ├─Token(NewLine) |\r\n|
    }
//@[04:005) |   |   |   └─Token(RightBrace) |}|
//@[05:007) |   |   ├─Token(NewLine) |\r\n|
  }
//@[02:003) |   |   └─Token(RightBrace) |}|
//@[03:005) |   ├─Token(NewLine) |\r\n|
}
//@[00:001) |   └─Token(RightBrace) |}|
//@[01:005) ├─Token(NewLine) |\r\n\r\n|

module moduleWithExtsWithoutAliases 'child/hasConfigurableExtensionsWithoutAlias.bicep' = {
//@[00:265) ├─ModuleDeclarationSyntax
//@[00:006) | ├─Token(Identifier) |module|
//@[07:035) | ├─IdentifierSyntax
//@[07:035) | | └─Token(Identifier) |moduleWithExtsWithoutAliases|
//@[36:087) | ├─StringSyntax
//@[36:087) | | └─Token(StringComplete) |'child/hasConfigurableExtensionsWithoutAlias.bicep'|
//@[88:089) | ├─Token(Assignment) |=|
//@[90:265) | └─ObjectSyntax
//@[90:091) |   ├─Token(LeftBrace) |{|
//@[91:093) |   ├─Token(NewLine) |\r\n|
  name: 'moduleWithExtsWithoutAlaises'
//@[02:038) |   ├─ObjectPropertySyntax
//@[02:006) |   | ├─IdentifierSyntax
//@[02:006) |   | | └─Token(Identifier) |name|
//@[06:007) |   | ├─Token(Colon) |:|
//@[08:038) |   | └─StringSyntax
//@[08:038) |   |   └─Token(StringComplete) |'moduleWithExtsWithoutAlaises'|
//@[38:040) |   ├─Token(NewLine) |\r\n|
  extensionConfigs: {
//@[02:129) |   ├─ObjectPropertySyntax
//@[02:018) |   | ├─IdentifierSyntax
//@[02:018) |   | | └─Token(Identifier) |extensionConfigs|
//@[18:019) |   | ├─Token(Colon) |:|
//@[20:129) |   | └─ObjectSyntax
//@[20:021) |   |   ├─Token(LeftBrace) |{|
//@[21:023) |   |   ├─Token(NewLine) |\r\n|
    kubernetes: {
//@[04:101) |   |   ├─ObjectPropertySyntax
//@[04:014) |   |   | ├─IdentifierSyntax
//@[04:014) |   |   | | └─Token(Identifier) |kubernetes|
//@[14:015) |   |   | ├─Token(Colon) |:|
//@[16:101) |   |   | └─ObjectSyntax
//@[16:017) |   |   |   ├─Token(LeftBrace) |{|
//@[17:019) |   |   |   ├─Token(NewLine) |\r\n|
      kubeConfig: 'kubeConfig2FromModule'
//@[06:041) |   |   |   ├─ObjectPropertySyntax
//@[06:016) |   |   |   | ├─IdentifierSyntax
//@[06:016) |   |   |   | | └─Token(Identifier) |kubeConfig|
//@[16:017) |   |   |   | ├─Token(Colon) |:|
//@[18:041) |   |   |   | └─StringSyntax
//@[18:041) |   |   |   |   └─Token(StringComplete) |'kubeConfig2FromModule'|
//@[41:043) |   |   |   ├─Token(NewLine) |\r\n|
      namespace: 'ns2FromModule'
//@[06:032) |   |   |   ├─ObjectPropertySyntax
//@[06:015) |   |   |   | ├─IdentifierSyntax
//@[06:015) |   |   |   | | └─Token(Identifier) |namespace|
//@[15:016) |   |   |   | ├─Token(Colon) |:|
//@[17:032) |   |   |   | └─StringSyntax
//@[17:032) |   |   |   |   └─Token(StringComplete) |'ns2FromModule'|
//@[32:034) |   |   |   ├─Token(NewLine) |\r\n|
    }
//@[04:005) |   |   |   └─Token(RightBrace) |}|
//@[05:007) |   |   ├─Token(NewLine) |\r\n|
  }
//@[02:003) |   |   └─Token(RightBrace) |}|
//@[03:005) |   ├─Token(NewLine) |\r\n|
}
//@[00:001) |   └─Token(RightBrace) |}|
//@[01:005) ├─Token(NewLine) |\r\n\r\n|

// END: Extension configs for modules
//@[37:039) ├─Token(NewLine) |\r\n|

//@[00:000) └─Token(EndOfFile) ||
