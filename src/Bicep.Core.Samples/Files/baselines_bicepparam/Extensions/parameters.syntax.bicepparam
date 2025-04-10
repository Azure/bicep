using 'main.bicep'
//@[00:174) ProgramSyntax
//@[00:018) ├─UsingDeclarationSyntax
//@[00:005) | ├─Token(Identifier) |using|
//@[06:018) | └─StringSyntax
//@[06:018) |   └─Token(StringComplete) |'main.bicep'|
//@[18:020) ├─Token(NewLine) |\n\n|

extension k8s with {
//@[00:153) ├─ExtensionConfigAssignmentSyntax
//@[00:009) | ├─Token(Identifier) |extension|
//@[10:013) | ├─IdentifierSyntax
//@[10:013) | | └─Token(Identifier) |k8s|
//@[14:153) | └─ExtensionWithClauseSyntax
//@[14:018) |   ├─Token(Identifier) |with|
//@[19:153) |   └─ObjectSyntax
//@[19:020) |     ├─Token(LeftBrace) |{|
//@[20:021) |     ├─Token(NewLine) |\n|
  kubeConfig: az.getSecret('00000000-0000-0000-0000-000000000001', 'mock-rg', 'kv', 'myKubeConfig')
//@[02:099) |     ├─ObjectPropertySyntax
//@[02:012) |     | ├─IdentifierSyntax
//@[02:012) |     | | └─Token(Identifier) |kubeConfig|
//@[12:013) |     | ├─Token(Colon) |:|
//@[14:099) |     | └─InstanceFunctionCallSyntax
//@[14:016) |     |   ├─VariableAccessSyntax
//@[14:016) |     |   | └─IdentifierSyntax
//@[14:016) |     |   |   └─Token(Identifier) |az|
//@[16:017) |     |   ├─Token(Dot) |.|
//@[17:026) |     |   ├─IdentifierSyntax
//@[17:026) |     |   | └─Token(Identifier) |getSecret|
//@[26:027) |     |   ├─Token(LeftParen) |(|
//@[27:065) |     |   ├─FunctionArgumentSyntax
//@[27:065) |     |   | └─StringSyntax
//@[27:065) |     |   |   └─Token(StringComplete) |'00000000-0000-0000-0000-000000000001'|
//@[65:066) |     |   ├─Token(Comma) |,|
//@[67:076) |     |   ├─FunctionArgumentSyntax
//@[67:076) |     |   | └─StringSyntax
//@[67:076) |     |   |   └─Token(StringComplete) |'mock-rg'|
//@[76:077) |     |   ├─Token(Comma) |,|
//@[78:082) |     |   ├─FunctionArgumentSyntax
//@[78:082) |     |   | └─StringSyntax
//@[78:082) |     |   |   └─Token(StringComplete) |'kv'|
//@[82:083) |     |   ├─Token(Comma) |,|
//@[84:098) |     |   ├─FunctionArgumentSyntax
//@[84:098) |     |   | └─StringSyntax
//@[84:098) |     |   |   └─Token(StringComplete) |'myKubeConfig'|
//@[98:099) |     |   └─Token(RightParen) |)|
//@[99:100) |     ├─Token(NewLine) |\n|
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
