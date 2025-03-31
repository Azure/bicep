// BEGIN: Parameters
//@[20:024) NewLine |\r\n\r\n|

param strParam1 string
//@[00:005) Identifier |param|
//@[06:015) Identifier |strParam1|
//@[16:022) Identifier |string|
//@[22:026) NewLine |\r\n\r\n|

@secure()
//@[00:001) At |@|
//@[01:007) Identifier |secure|
//@[07:008) LeftParen |(|
//@[08:009) RightParen |)|
//@[09:011) NewLine |\r\n|
param secureStrParam1 string
//@[00:005) Identifier |param|
//@[06:021) Identifier |secureStrParam1|
//@[22:028) Identifier |string|
//@[28:032) NewLine |\r\n\r\n|

param boolParam1 bool
//@[00:005) Identifier |param|
//@[06:016) Identifier |boolParam1|
//@[17:021) Identifier |bool|
//@[21:025) NewLine |\r\n\r\n|

// END: Parameters
//@[18:022) NewLine |\r\n\r\n|

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

//extension 'br:mcr.microsoft.com/bicep/extensions/microsoftgraph/v1:1.2.3' as graph
//@[84:088) NewLine |\r\n\r\n|

// END: Extension declarations
//@[30:034) NewLine |\r\n\r\n|

// BEGIN: Key vaults
//@[20:024) NewLine |\r\n\r\n|

resource kv1 'Microsoft.KeyVault/vaults@2019-09-01' existing = {
//@[00:008) Identifier |resource|
//@[09:012) Identifier |kv1|
//@[13:051) StringComplete |'Microsoft.KeyVault/vaults@2019-09-01'|
//@[52:060) Identifier |existing|
//@[61:062) Assignment |=|
//@[63:064) LeftBrace |{|
//@[64:066) NewLine |\r\n|
  name: 'kv1'
//@[02:006) Identifier |name|
//@[06:007) Colon |:|
//@[08:013) StringComplete |'kv1'|
//@[13:015) NewLine |\r\n|
}
//@[00:001) RightBrace |}|
//@[01:005) NewLine |\r\n\r\n|

resource scopedKv1 'Microsoft.KeyVault/vaults@2019-09-01' existing = {
//@[00:008) Identifier |resource|
//@[09:018) Identifier |scopedKv1|
//@[19:057) StringComplete |'Microsoft.KeyVault/vaults@2019-09-01'|
//@[58:066) Identifier |existing|
//@[67:068) Assignment |=|
//@[69:070) LeftBrace |{|
//@[70:072) NewLine |\r\n|
  name: 'scopedKv1'
//@[02:006) Identifier |name|
//@[06:007) Colon |:|
//@[08:019) StringComplete |'scopedKv1'|
//@[19:021) NewLine |\r\n|
  scope: resourceGroup('otherGroup')
//@[02:007) Identifier |scope|
//@[07:008) Colon |:|
//@[09:022) Identifier |resourceGroup|
//@[22:023) LeftParen |(|
//@[23:035) StringComplete |'otherGroup'|
//@[35:036) RightParen |)|
//@[36:038) NewLine |\r\n|
}
//@[00:001) RightBrace |}|
//@[01:005) NewLine |\r\n\r\n|

// END: Key vaults
//@[18:022) NewLine |\r\n\r\n|

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

module moduleExtConfigsFromParams 'child/hasConfigurableExtensionsWithAlias.bicep' = {
//@[00:006) Identifier |module|
//@[07:033) Identifier |moduleExtConfigsFromParams|
//@[34:082) StringComplete |'child/hasConfigurableExtensionsWithAlias.bicep'|
//@[83:084) Assignment |=|
//@[85:086) LeftBrace |{|
//@[86:088) NewLine |\r\n|
  name: 'moduleExtConfigsFromParams'
//@[02:006) Identifier |name|
//@[06:007) Colon |:|
//@[08:036) StringComplete |'moduleExtConfigsFromParams'|
//@[36:038) NewLine |\r\n|
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
      kubeConfig: boolParam1 ? secureStrParam1 : strParam1
//@[06:016) Identifier |kubeConfig|
//@[16:017) Colon |:|
//@[18:028) Identifier |boolParam1|
//@[29:030) Question |?|
//@[31:046) Identifier |secureStrParam1|
//@[47:048) Colon |:|
//@[49:058) Identifier |strParam1|
//@[58:060) NewLine |\r\n|
      namespace: boolParam1 ? strParam1 : 'falseCond'
//@[06:015) Identifier |namespace|
//@[15:016) Colon |:|
//@[17:027) Identifier |boolParam1|
//@[28:029) Question |?|
//@[30:039) Identifier |strParam1|
//@[40:041) Colon |:|
//@[42:053) StringComplete |'falseCond'|
//@[53:055) NewLine |\r\n|
    }
//@[04:005) RightBrace |}|
//@[05:007) NewLine |\r\n|
  }
//@[02:003) RightBrace |}|
//@[03:005) NewLine |\r\n|
}
//@[00:001) RightBrace |}|
//@[01:005) NewLine |\r\n\r\n|

// TODO(kylealbert): Allow key vault references in extension configs
//@[68:070) NewLine |\r\n|
// module moduleExtConfigFromKeyVaultReference 'child/hasConfigurableExtensionsWithAlias.bicep' = {
//@[99:101) NewLine |\r\n|
//   name: 'moduleExtConfigKeyVaultReference'
//@[45:047) NewLine |\r\n|
//   extensionConfigs: {
//@[24:026) NewLine |\r\n|
//     k8s: {
//@[13:015) NewLine |\r\n|
//       kubeConfig: kv1.getSecret('myKubeConfig'),
//@[51:053) NewLine |\r\n|
//       namespace: scopedKv1.getSecret('myNamespace')
//@[54:056) NewLine |\r\n|
//     }
//@[08:010) NewLine |\r\n|
//   }
//@[06:008) NewLine |\r\n|
// }
//@[04:008) NewLine |\r\n\r\n|

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

// TODO(kylealbert): Figure out if this is allowable
//@[52:054) NewLine |\r\n|
// var k8sConfigDeployTime = {
//@[30:032) NewLine |\r\n|
//   kubeConfig: k8s.config.kubeConfig
//@[38:040) NewLine |\r\n|
//   namespace: strParam1
//@[25:027) NewLine |\r\n|
// }
//@[04:008) NewLine |\r\n\r\n|

// module moduleWithExtsUsingVar 'child/hasConfigurableExtensionsWithAlias.bicep' = {
//@[85:087) NewLine |\r\n|
//   name: 'moduleWithExtsUsingVar'
//@[35:037) NewLine |\r\n|
//   extensionConfigs: {
//@[24:026) NewLine |\r\n|
//     k8s: k8sConfigDeployTime
//@[31:033) NewLine |\r\n|
//   }
//@[06:008) NewLine |\r\n|
// }
//@[04:008) NewLine |\r\n\r\n|

// END: Extension configs for modules
//@[37:039) NewLine |\r\n|

//@[00:000) EndOfFile ||
