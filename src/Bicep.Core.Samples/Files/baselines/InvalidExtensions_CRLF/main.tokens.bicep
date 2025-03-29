// BEGIN: Valid Extension declarations
//@[38:42) NewLine |\r\n\r\n|

extension kubernetes with {
//@[00:09) Identifier |extension|
//@[10:20) Identifier |kubernetes|
//@[21:25) Identifier |with|
//@[26:27) LeftBrace |{|
//@[27:29) NewLine |\r\n|
  kubeConfig: 'DELETE'
//@[02:12) Identifier |kubeConfig|
//@[12:13) Colon |:|
//@[14:22) StringComplete |'DELETE'|
//@[22:24) NewLine |\r\n|
  namespace: 'DELETE'
//@[02:11) Identifier |namespace|
//@[11:12) Colon |:|
//@[13:21) StringComplete |'DELETE'|
//@[21:23) NewLine |\r\n|
} as k8s
//@[00:01) RightBrace |}|
//@[02:04) Identifier |as|
//@[05:08) Identifier |k8s|
//@[08:12) NewLine |\r\n\r\n|

//extension 'br:mcr.microsoft.com/bicep/extensions/microsoftgraph/v1.0:0.1.8-preview' as graph
//@[94:98) NewLine |\r\n\r\n|

// END: Valid Extension declarations
//@[36:40) NewLine |\r\n\r\n|

// BEGIN: Extension configs for modules
//@[39:43) NewLine |\r\n\r\n|

module moduleWithExtsUsingFullInheritance 'child/hasConfigurableExtensionsWithAlias.bicep' = {
//@[00:06) Identifier |module|
//@[07:41) Identifier |moduleWithExtsUsingFullInheritance|
//@[42:90) StringComplete |'child/hasConfigurableExtensionsWithAlias.bicep'|
//@[91:92) Assignment |=|
//@[93:94) LeftBrace |{|
//@[94:96) NewLine |\r\n|
  name: 'moduleWithExtsFullInheritance'
//@[02:06) Identifier |name|
//@[06:07) Colon |:|
//@[08:39) StringComplete |'moduleWithExtsFullInheritance'|
//@[39:41) NewLine |\r\n|
  extensionConfigs: {
//@[02:18) Identifier |extensionConfigs|
//@[18:19) Colon |:|
//@[20:21) LeftBrace |{|
//@[21:23) NewLine |\r\n|
    k8s: k8s // must use k8s.config
//@[04:07) Identifier |k8s|
//@[07:08) Colon |:|
//@[09:12) Identifier |k8s|
//@[35:37) NewLine |\r\n|
  }
//@[02:03) RightBrace |}|
//@[03:05) NewLine |\r\n|
}
//@[00:01) RightBrace |}|
//@[01:05) NewLine |\r\n\r\n|

// END: Extension configs for modules
//@[37:39) NewLine |\r\n|

//@[00:00) EndOfFile ||
