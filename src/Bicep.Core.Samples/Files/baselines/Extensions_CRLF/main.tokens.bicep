// BEGIN: Extension declarations
//@[32:36) NewLine |\r\n\r\n|

// extension kubernetes with {
//@[30:32) NewLine |\r\n|
//   kubeConfig: 'DELETE'
//@[25:27) NewLine |\r\n|
//   namespace: 'DELETE'
//@[24:26) NewLine |\r\n|
// } as k8s
//@[11:15) NewLine |\r\n\r\n|

//extension 'br:mcr.microsoft.com/bicep/extensions/microsoftgraph/v1.0:0.1.8-preview' as graph
//@[94:98) NewLine |\r\n\r\n|

// END: Extension declarations
//@[30:34) NewLine |\r\n\r\n|

// BEGIN: Extension configs for modules
//@[39:43) NewLine |\r\n\r\n|

module moduleWithExtsWithAliases 'child/hasConfigurableExtensionsWithAlias.bicep' = {
//@[00:06) Identifier |module|
//@[07:32) Identifier |moduleWithExtsWithAliases|
//@[33:81) StringComplete |'child/hasConfigurableExtensionsWithAlias.bicep'|
//@[82:83) Assignment |=|
//@[84:85) LeftBrace |{|
//@[85:87) NewLine |\r\n|
  name: 'moduleWithExtsWithAliases'
//@[02:06) Identifier |name|
//@[06:07) Colon |:|
//@[08:35) StringComplete |'moduleWithExtsWithAliases'|
//@[35:37) NewLine |\r\n|
  extensionConfigs: {
//@[02:18) Identifier |extensionConfigs|
//@[18:19) Colon |:|
//@[20:21) LeftBrace |{|
//@[21:23) NewLine |\r\n|
    k8s: {
//@[04:07) Identifier |k8s|
//@[07:08) Colon |:|
//@[09:10) LeftBrace |{|
//@[10:12) NewLine |\r\n|
      kubeConfig: 'kubeConfig2FromModule'
//@[06:16) Identifier |kubeConfig|
//@[16:17) Colon |:|
//@[18:41) StringComplete |'kubeConfig2FromModule'|
//@[41:43) NewLine |\r\n|
      namespace: 'ns2FromModule'
//@[06:15) Identifier |namespace|
//@[15:16) Colon |:|
//@[17:32) StringComplete |'ns2FromModule'|
//@[32:34) NewLine |\r\n|
    }
//@[04:05) RightBrace |}|
//@[05:07) NewLine |\r\n|
  }
//@[02:03) RightBrace |}|
//@[03:05) NewLine |\r\n|
}
//@[00:01) RightBrace |}|
//@[01:05) NewLine |\r\n\r\n|

module moduleWithExtsWithoutAliases 'child/hasConfigurableExtensionsWithoutAlias.bicep' = {
//@[00:06) Identifier |module|
//@[07:35) Identifier |moduleWithExtsWithoutAliases|
//@[36:87) StringComplete |'child/hasConfigurableExtensionsWithoutAlias.bicep'|
//@[88:89) Assignment |=|
//@[90:91) LeftBrace |{|
//@[91:93) NewLine |\r\n|
  name: 'moduleWithExtsWithoutAlaises'
//@[02:06) Identifier |name|
//@[06:07) Colon |:|
//@[08:38) StringComplete |'moduleWithExtsWithoutAlaises'|
//@[38:40) NewLine |\r\n|
  extensionConfigs: {
//@[02:18) Identifier |extensionConfigs|
//@[18:19) Colon |:|
//@[20:21) LeftBrace |{|
//@[21:23) NewLine |\r\n|
    kubernetes: {
//@[04:14) Identifier |kubernetes|
//@[14:15) Colon |:|
//@[16:17) LeftBrace |{|
//@[17:19) NewLine |\r\n|
      kubeConfig: 'kubeConfig2FromModule'
//@[06:16) Identifier |kubeConfig|
//@[16:17) Colon |:|
//@[18:41) StringComplete |'kubeConfig2FromModule'|
//@[41:43) NewLine |\r\n|
      namespace: 'ns2FromModule'
//@[06:15) Identifier |namespace|
//@[15:16) Colon |:|
//@[17:32) StringComplete |'ns2FromModule'|
//@[32:34) NewLine |\r\n|
    }
//@[04:05) RightBrace |}|
//@[05:07) NewLine |\r\n|
  }
//@[02:03) RightBrace |}|
//@[03:05) NewLine |\r\n|
}
//@[00:01) RightBrace |}|
//@[01:05) NewLine |\r\n\r\n|

// END: Extension configs for modules
//@[37:39) NewLine |\r\n|

//@[00:00) EndOfFile ||
