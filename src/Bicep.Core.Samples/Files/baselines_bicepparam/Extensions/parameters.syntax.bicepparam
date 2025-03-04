using 'main.bicep'
//@[00:110) ProgramSyntax
//@[00:018) ├─UsingDeclarationSyntax
//@[00:005) | ├─Token(Identifier) |using|
//@[06:018) | └─StringSyntax
//@[06:018) |   └─Token(StringComplete) |'main.bicep'|
//@[18:020) ├─Token(NewLine) |\n\n|

extension k8s with {
//@[00:089) ├─ExtensionConfigAssignmentSyntax
//@[00:009) | ├─Token(Identifier) |extension|
//@[10:013) | ├─IdentifierSyntax
//@[10:013) | | └─Token(Identifier) |k8s|
//@[14:089) | └─ExtensionWithClauseSyntax
//@[14:018) |   ├─Token(Identifier) |with|
//@[19:089) |   └─ObjectSyntax
//@[19:020) |     ├─Token(LeftBrace) |{|
//@[20:021) |     ├─Token(NewLine) |\n|
  kubeConfig: 'configFromParamFile'
//@[02:035) |     ├─ObjectPropertySyntax
//@[02:012) |     | ├─IdentifierSyntax
//@[02:012) |     | | └─Token(Identifier) |kubeConfig|
//@[12:013) |     | ├─Token(Colon) |:|
//@[14:035) |     | └─StringSyntax
//@[14:035) |     |   └─Token(StringComplete) |'configFromParamFile'|
//@[35:036) |     ├─Token(NewLine) |\n|
  namespace: 'nsFromParamFile'
//@[02:030) |     ├─ObjectPropertySyntax
//@[02:011) |     | ├─IdentifierSyntax
//@[02:011) |     | | └─Token(Identifier) |namespace|
//@[11:012) |     | ├─Token(Colon) |:|
//@[13:030) |     | └─StringSyntax
//@[13:030) |     |   └─Token(StringComplete) |'nsFromParamFile'|
//@[30:031) |     ├─Token(NewLine) |\n|
}
//@[00:001) |     └─Token(RightBrace) |}|
//@[01:002) ├─Token(NewLine) |\n|

//@[00:000) └─Token(EndOfFile) ||
