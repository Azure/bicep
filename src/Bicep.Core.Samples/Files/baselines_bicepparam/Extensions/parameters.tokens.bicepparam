using 'main.bicep'
//@[00:05) Identifier |using|
//@[06:18) StringComplete |'main.bicep'|
//@[18:20) NewLine |\n\n|

extension k8s with {
//@[00:09) Identifier |extension|
//@[10:13) Identifier |k8s|
//@[14:18) Identifier |with|
//@[19:20) LeftBrace |{|
//@[20:21) NewLine |\n|
  kubeConfig: 'configFromParamFile'
//@[02:12) Identifier |kubeConfig|
//@[12:13) Colon |:|
//@[14:35) StringComplete |'configFromParamFile'|
//@[35:36) NewLine |\n|
  namespace: 'nsFromParamFile'
//@[02:11) Identifier |namespace|
//@[11:12) Colon |:|
//@[13:30) StringComplete |'nsFromParamFile'|
//@[30:31) NewLine |\n|
}
//@[00:01) RightBrace |}|
//@[01:02) NewLine |\n|

//@[00:00) EndOfFile ||
