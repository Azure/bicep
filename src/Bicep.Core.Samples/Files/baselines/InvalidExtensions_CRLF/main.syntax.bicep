// BEGIN: Valid Extension declarations
//@[00:557) ProgramSyntax
//@[38:042) ├─Token(NewLine) |\r\n\r\n|

extension kubernetes with {
//@[00:084) ├─ExtensionDeclarationSyntax
//@[00:009) | ├─Token(Identifier) |extension|
//@[10:020) | ├─IdentifierSyntax
//@[10:020) | | └─Token(Identifier) |kubernetes|
//@[21:077) | ├─ExtensionWithClauseSyntax
//@[21:025) | | ├─Token(Identifier) |with|
//@[26:077) | | └─ObjectSyntax
//@[26:027) | |   ├─Token(LeftBrace) |{|
//@[27:029) | |   ├─Token(NewLine) |\r\n|
  kubeConfig: 'DELETE'
//@[02:022) | |   ├─ObjectPropertySyntax
//@[02:012) | |   | ├─IdentifierSyntax
//@[02:012) | |   | | └─Token(Identifier) |kubeConfig|
//@[12:013) | |   | ├─Token(Colon) |:|
//@[14:022) | |   | └─StringSyntax
//@[14:022) | |   |   └─Token(StringComplete) |'DELETE'|
//@[22:024) | |   ├─Token(NewLine) |\r\n|
  namespace: 'DELETE'
//@[02:021) | |   ├─ObjectPropertySyntax
//@[02:011) | |   | ├─IdentifierSyntax
//@[02:011) | |   | | └─Token(Identifier) |namespace|
//@[11:012) | |   | ├─Token(Colon) |:|
//@[13:021) | |   | └─StringSyntax
//@[13:021) | |   |   └─Token(StringComplete) |'DELETE'|
//@[21:023) | |   ├─Token(NewLine) |\r\n|
} as k8s
//@[00:001) | |   └─Token(RightBrace) |}|
//@[02:008) | └─AliasAsClauseSyntax
//@[02:004) |   ├─Token(Identifier) |as|
//@[05:008) |   └─IdentifierSyntax
//@[05:008) |     └─Token(Identifier) |k8s|
//@[08:012) ├─Token(NewLine) |\r\n\r\n|

//extension 'br:mcr.microsoft.com/bicep/extensions/microsoftgraph/v1.0:0.1.8-preview' as graph
//@[94:098) ├─Token(NewLine) |\r\n\r\n|

// END: Valid Extension declarations
//@[36:040) ├─Token(NewLine) |\r\n\r\n|

// BEGIN: Extension configs for modules
//@[39:043) ├─Token(NewLine) |\r\n\r\n|

module moduleWithExtsUsingFullInheritance 'child/hasConfigurableExtensionsWithAlias.bicep' = {
//@[00:203) ├─ModuleDeclarationSyntax
//@[00:006) | ├─Token(Identifier) |module|
//@[07:041) | ├─IdentifierSyntax
//@[07:041) | | └─Token(Identifier) |moduleWithExtsUsingFullInheritance|
//@[42:090) | ├─StringSyntax
//@[42:090) | | └─Token(StringComplete) |'child/hasConfigurableExtensionsWithAlias.bicep'|
//@[91:092) | ├─Token(Assignment) |=|
//@[93:203) | └─ObjectSyntax
//@[93:094) |   ├─Token(LeftBrace) |{|
//@[94:096) |   ├─Token(NewLine) |\r\n|
  name: 'moduleWithExtsFullInheritance'
//@[02:039) |   ├─ObjectPropertySyntax
//@[02:006) |   | ├─IdentifierSyntax
//@[02:006) |   | | └─Token(Identifier) |name|
//@[06:007) |   | ├─Token(Colon) |:|
//@[08:039) |   | └─StringSyntax
//@[08:039) |   |   └─Token(StringComplete) |'moduleWithExtsFullInheritance'|
//@[39:041) |   ├─Token(NewLine) |\r\n|
  extensionConfigs: {
//@[02:063) |   ├─ObjectPropertySyntax
//@[02:018) |   | ├─IdentifierSyntax
//@[02:018) |   | | └─Token(Identifier) |extensionConfigs|
//@[18:019) |   | ├─Token(Colon) |:|
//@[20:063) |   | └─ObjectSyntax
//@[20:021) |   |   ├─Token(LeftBrace) |{|
//@[21:023) |   |   ├─Token(NewLine) |\r\n|
    k8s: k8s // must use k8s.config
//@[04:012) |   |   ├─ObjectPropertySyntax
//@[04:007) |   |   | ├─IdentifierSyntax
//@[04:007) |   |   | | └─Token(Identifier) |k8s|
//@[07:008) |   |   | ├─Token(Colon) |:|
//@[09:012) |   |   | └─VariableAccessSyntax
//@[09:012) |   |   |   └─IdentifierSyntax
//@[09:012) |   |   |     └─Token(Identifier) |k8s|
//@[35:037) |   |   ├─Token(NewLine) |\r\n|
  }
//@[02:003) |   |   └─Token(RightBrace) |}|
//@[03:005) |   ├─Token(NewLine) |\r\n|
}
//@[00:001) |   └─Token(RightBrace) |}|
//@[01:005) ├─Token(NewLine) |\r\n\r\n|

// END: Extension configs for modules
//@[37:039) ├─Token(NewLine) |\r\n|

//@[00:000) └─Token(EndOfFile) ||
