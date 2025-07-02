using 'main.bicep'
//@[00:005) Identifier |using|
//@[06:018) StringComplete |'main.bicep'|
//@[18:020) NewLine |\n\n|

extensionConfig k8s with {
//@[00:015) Identifier |extensionConfig|
//@[16:019) Identifier |k8s|
//@[20:024) Identifier |with|
//@[25:026) LeftBrace |{|
//@[26:027) NewLine |\n|
  kubeConfig: az.getSecret('00000000-0000-0000-0000-000000000001', 'mock-rg', 'kv', 'myKubeConfig')
//@[02:012) Identifier |kubeConfig|
//@[12:013) Colon |:|
//@[14:016) Identifier |az|
//@[16:017) Dot |.|
//@[17:026) Identifier |getSecret|
//@[26:027) LeftParen |(|
//@[27:065) StringComplete |'00000000-0000-0000-0000-000000000001'|
//@[65:066) Comma |,|
//@[67:076) StringComplete |'mock-rg'|
//@[76:077) Comma |,|
//@[78:082) StringComplete |'kv'|
//@[82:083) Comma |,|
//@[84:098) StringComplete |'myKubeConfig'|
//@[98:099) RightParen |)|
//@[99:100) NewLine |\n|
  namespace: 'nsFromParamFile'
//@[02:011) Identifier |namespace|
//@[11:012) Colon |:|
//@[13:030) StringComplete |'nsFromParamFile'|
//@[30:031) NewLine |\n|
}
//@[00:001) RightBrace |}|
//@[01:002) NewLine |\n|

//@[00:000) EndOfFile ||
