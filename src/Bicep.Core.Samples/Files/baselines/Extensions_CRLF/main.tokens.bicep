// BEGIN: Extension declarations
//@[32:036) NewLine |\r\n\r\n|

extension kubernetes with {
//@[00:009) Identifier |extension|
//@[10:020) Identifier |kubernetes|
//@[21:025) Identifier |with|
//@[26:027) LeftBrace |{|
//@[27:029) NewLine |\r\n|
  kubeConfig: 'DELETE'
//@[02:012) Identifier |kubeConfig|
//@[12:013) Colon |:|
//@[14:022) StringComplete |'DELETE'|
//@[22:024) NewLine |\r\n|
  namespace: 'DELETE'
//@[02:011) Identifier |namespace|
//@[11:012) Colon |:|
//@[13:021) StringComplete |'DELETE'|
//@[21:023) NewLine |\r\n|
} as k8s
//@[00:001) RightBrace |}|
//@[02:004) Identifier |as|
//@[05:008) Identifier |k8s|
//@[08:012) NewLine |\r\n\r\n|

//extension 'br:mcr.microsoft.com/bicep/extensions/microsoftgraph/v1.0:0.1.8-preview' as graph
//@[94:098) NewLine |\r\n\r\n|

// END: Extension declarations
//@[30:034) NewLine |\r\n\r\n|

// BEGIN: Extension configs for modules
//@[39:043) NewLine |\r\n\r\n|

module moduleWithExtsWithAliases 'child/hasConfigurableExtensionsWithAlias.bicep' = {
//@[00:006) Identifier |module|
//@[07:032) Identifier |moduleWithExtsWithAliases|
//@[33:081) StringComplete |'child/hasConfigurableExtensionsWithAlias.bicep'|
//@[82:083) Assignment |=|
//@[84:085) LeftBrace |{|
//@[85:087) NewLine |\r\n|
  name: 'moduleWithExtsWithAliases'
//@[02:006) Identifier |name|
//@[06:007) Colon |:|
//@[08:035) StringComplete |'moduleWithExtsWithAliases'|
//@[35:037) NewLine |\r\n|
  extensionConfigs: {
//@[02:018) Identifier |extensionConfigs|
//@[18:019) Colon |:|
//@[20:021) LeftBrace |{|
//@[21:023) NewLine |\r\n|
    k8s: {
//@[04:007) Identifier |k8s|
//@[07:008) Colon |:|
//@[09:010) LeftBrace |{|
//@[10:012) NewLine |\r\n|
      kubeConfig: 'kubeConfig2FromModule'
//@[06:016) Identifier |kubeConfig|
//@[16:017) Colon |:|
//@[18:041) StringComplete |'kubeConfig2FromModule'|
//@[41:043) NewLine |\r\n|
      namespace: 'ns2FromModule'
//@[06:015) Identifier |namespace|
//@[15:016) Colon |:|
//@[17:032) StringComplete |'ns2FromModule'|
//@[32:034) NewLine |\r\n|
    }
//@[04:005) RightBrace |}|
//@[05:007) NewLine |\r\n|
  }
//@[02:003) RightBrace |}|
//@[03:005) NewLine |\r\n|
}
//@[00:001) RightBrace |}|
//@[01:005) NewLine |\r\n\r\n|

module moduleWithExtsWithoutAliases 'child/hasConfigurableExtensionsWithoutAlias.bicep' = {
//@[00:006) Identifier |module|
//@[07:035) Identifier |moduleWithExtsWithoutAliases|
//@[36:087) StringComplete |'child/hasConfigurableExtensionsWithoutAlias.bicep'|
//@[88:089) Assignment |=|
//@[90:091) LeftBrace |{|
//@[91:093) NewLine |\r\n|
  name: 'moduleWithExtsWithoutAliases'
//@[02:006) Identifier |name|
//@[06:007) Colon |:|
//@[08:038) StringComplete |'moduleWithExtsWithoutAliases'|
//@[38:040) NewLine |\r\n|
  extensionConfigs: {
//@[02:018) Identifier |extensionConfigs|
//@[18:019) Colon |:|
//@[20:021) LeftBrace |{|
//@[21:023) NewLine |\r\n|
    kubernetes: {
//@[04:014) Identifier |kubernetes|
//@[14:015) Colon |:|
//@[16:017) LeftBrace |{|
//@[17:019) NewLine |\r\n|
      kubeConfig: 'kubeConfig2FromModule'
//@[06:016) Identifier |kubeConfig|
//@[16:017) Colon |:|
//@[18:041) StringComplete |'kubeConfig2FromModule'|
//@[41:043) NewLine |\r\n|
      namespace: 'ns2FromModule'
//@[06:015) Identifier |namespace|
//@[15:016) Colon |:|
//@[17:032) StringComplete |'ns2FromModule'|
//@[32:034) NewLine |\r\n|
    }
//@[04:005) RightBrace |}|
//@[05:007) NewLine |\r\n|
  }
//@[02:003) RightBrace |}|
//@[03:005) NewLine |\r\n|
}
//@[00:001) RightBrace |}|
//@[01:005) NewLine |\r\n\r\n|

module moduleWithExtsUsingFullInheritance 'child/hasConfigurableExtensionsWithAlias.bicep' = {
//@[00:006) Identifier |module|
//@[07:041) Identifier |moduleWithExtsUsingFullInheritance|
//@[42:090) StringComplete |'child/hasConfigurableExtensionsWithAlias.bicep'|
//@[91:092) Assignment |=|
//@[93:094) LeftBrace |{|
//@[94:096) NewLine |\r\n|
  name: 'moduleWithExtsFullInheritance'
//@[02:006) Identifier |name|
//@[06:007) Colon |:|
//@[08:039) StringComplete |'moduleWithExtsFullInheritance'|
//@[39:041) NewLine |\r\n|
  extensionConfigs: {
//@[02:018) Identifier |extensionConfigs|
//@[18:019) Colon |:|
//@[20:021) LeftBrace |{|
//@[21:023) NewLine |\r\n|
    k8s: k8s.config
//@[04:007) Identifier |k8s|
//@[07:008) Colon |:|
//@[09:012) Identifier |k8s|
//@[12:013) Dot |.|
//@[13:019) Identifier |config|
//@[19:021) NewLine |\r\n|
  }
//@[02:003) RightBrace |}|
//@[03:005) NewLine |\r\n|
}
//@[00:001) RightBrace |}|
//@[01:005) NewLine |\r\n\r\n|

module moduleWithExtsUsingPiecemealInheritance 'child/hasConfigurableExtensionsWithAlias.bicep' = {
//@[00:006) Identifier |module|
//@[07:046) Identifier |moduleWithExtsUsingPiecemealInheritance|
//@[47:095) StringComplete |'child/hasConfigurableExtensionsWithAlias.bicep'|
//@[96:097) Assignment |=|
//@[98:099) LeftBrace |{|
//@[99:101) NewLine |\r\n|
  name: 'moduleWithExtsPiecemealInheritance'
//@[02:006) Identifier |name|
//@[06:007) Colon |:|
//@[08:044) StringComplete |'moduleWithExtsPiecemealInheritance'|
//@[44:046) NewLine |\r\n|
  extensionConfigs: {
//@[02:018) Identifier |extensionConfigs|
//@[18:019) Colon |:|
//@[20:021) LeftBrace |{|
//@[21:023) NewLine |\r\n|
    k8s: {
//@[04:007) Identifier |k8s|
//@[07:008) Colon |:|
//@[09:010) LeftBrace |{|
//@[10:012) NewLine |\r\n|
      kubeConfig: k8s.config.kubeConfig
//@[06:016) Identifier |kubeConfig|
//@[16:017) Colon |:|
//@[18:021) Identifier |k8s|
//@[21:022) Dot |.|
//@[22:028) Identifier |config|
//@[28:029) Dot |.|
//@[29:039) Identifier |kubeConfig|
//@[39:041) NewLine |\r\n|
      namespace: k8s.config.namespace
//@[06:015) Identifier |namespace|
//@[15:016) Colon |:|
//@[17:020) Identifier |k8s|
//@[20:021) Dot |.|
//@[21:027) Identifier |config|
//@[27:028) Dot |.|
//@[28:037) Identifier |namespace|
//@[37:039) NewLine |\r\n|
    }
//@[04:005) RightBrace |}|
//@[05:007) NewLine |\r\n|
  }
//@[02:003) RightBrace |}|
//@[03:005) NewLine |\r\n|
}
//@[00:001) RightBrace |}|
//@[01:005) NewLine |\r\n\r\n|

// END: Extension configs for modules
//@[37:039) NewLine |\r\n|

//@[00:000) EndOfFile ||
