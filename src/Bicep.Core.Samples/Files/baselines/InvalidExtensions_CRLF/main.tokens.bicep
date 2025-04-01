// BEGIN: Parameters
//@[020:024) NewLine |\r\n\r\n|

param boolParam1 bool
//@[000:005) Identifier |param|
//@[006:016) Identifier |boolParam1|
//@[017:021) Identifier |bool|
//@[021:025) NewLine |\r\n\r\n|

// END: Parameters
//@[018:022) NewLine |\r\n\r\n|

// BEGIN: Valid Extension declarations
//@[038:042) NewLine |\r\n\r\n|

extension kubernetes with {
//@[000:009) Identifier |extension|
//@[010:020) Identifier |kubernetes|
//@[021:025) Identifier |with|
//@[026:027) LeftBrace |{|
//@[027:029) NewLine |\r\n|
  kubeConfig: 'DELETE'
//@[002:012) Identifier |kubeConfig|
//@[012:013) Colon |:|
//@[014:022) StringComplete |'DELETE'|
//@[022:024) NewLine |\r\n|
  namespace: 'DELETE'
//@[002:011) Identifier |namespace|
//@[011:012) Colon |:|
//@[013:021) StringComplete |'DELETE'|
//@[021:023) NewLine |\r\n|
} as k8s
//@[000:001) RightBrace |}|
//@[002:004) Identifier |as|
//@[005:008) Identifier |k8s|
//@[008:012) NewLine |\r\n\r\n|

//extension 'br:mcr.microsoft.com/bicep/extensions/microsoftgraph/v1.0:0.1.8-preview' as graph
//@[094:098) NewLine |\r\n\r\n|

// END: Valid Extension declarations
//@[036:040) NewLine |\r\n\r\n|

// BEGIN: Key vaults
//@[020:024) NewLine |\r\n\r\n|

resource kv1 'Microsoft.KeyVault/vaults@2019-09-01' existing = {
//@[000:008) Identifier |resource|
//@[009:012) Identifier |kv1|
//@[013:051) StringComplete |'Microsoft.KeyVault/vaults@2019-09-01'|
//@[052:060) Identifier |existing|
//@[061:062) Assignment |=|
//@[063:064) LeftBrace |{|
//@[064:066) NewLine |\r\n|
  name: 'kv1'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:013) StringComplete |'kv1'|
//@[013:015) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

resource scopedKv1 'Microsoft.KeyVault/vaults@2019-09-01' existing = {
//@[000:008) Identifier |resource|
//@[009:018) Identifier |scopedKv1|
//@[019:057) StringComplete |'Microsoft.KeyVault/vaults@2019-09-01'|
//@[058:066) Identifier |existing|
//@[067:068) Assignment |=|
//@[069:070) LeftBrace |{|
//@[070:072) NewLine |\r\n|
  name: 'scopedKv1'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:019) StringComplete |'scopedKv1'|
//@[019:021) NewLine |\r\n|
  scope: resourceGroup('otherGroup')
//@[002:007) Identifier |scope|
//@[007:008) Colon |:|
//@[009:022) Identifier |resourceGroup|
//@[022:023) LeftParen |(|
//@[023:035) StringComplete |'otherGroup'|
//@[035:036) RightParen |)|
//@[036:038) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

// END: Key Vaults
//@[018:022) NewLine |\r\n\r\n|

// BEGIN: Extension configs for modules
//@[039:043) NewLine |\r\n\r\n|

module moduleWithExtsUsingFullInheritance 'child/hasConfigurableExtensionsWithAlias.bicep' = {
//@[000:006) Identifier |module|
//@[007:041) Identifier |moduleWithExtsUsingFullInheritance|
//@[042:090) StringComplete |'child/hasConfigurableExtensionsWithAlias.bicep'|
//@[091:092) Assignment |=|
//@[093:094) LeftBrace |{|
//@[094:096) NewLine |\r\n|
  name: 'moduleWithExtsFullInheritance'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:039) StringComplete |'moduleWithExtsFullInheritance'|
//@[039:041) NewLine |\r\n|
  extensionConfigs: {
//@[002:018) Identifier |extensionConfigs|
//@[018:019) Colon |:|
//@[020:021) LeftBrace |{|
//@[021:023) NewLine |\r\n|
    k8s: k8s // must use k8s.config
//@[004:007) Identifier |k8s|
//@[007:008) Colon |:|
//@[009:012) Identifier |k8s|
//@[035:037) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

module moduleInvalidPropertyAccess 'child/hasConfigurableExtensionsWithAlias.bicep' = {
//@[000:006) Identifier |module|
//@[007:034) Identifier |moduleInvalidPropertyAccess|
//@[035:083) StringComplete |'child/hasConfigurableExtensionsWithAlias.bicep'|
//@[084:085) Assignment |=|
//@[086:087) LeftBrace |{|
//@[087:089) NewLine |\r\n|
  name: 'moduleInvalidPropertyAccess'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:037) StringComplete |'moduleInvalidPropertyAccess'|
//@[037:039) NewLine |\r\n|
  extensionConfigs: {
//@[002:018) Identifier |extensionConfigs|
//@[018:019) Colon |:|
//@[020:021) LeftBrace |{|
//@[021:023) NewLine |\r\n|
    k8s: {
//@[004:007) Identifier |k8s|
//@[007:008) Colon |:|
//@[009:010) LeftBrace |{|
//@[010:012) NewLine |\r\n|
      kubeConfig: k8s.config.kubeConfig.keyVaultReference
//@[006:016) Identifier |kubeConfig|
//@[016:017) Colon |:|
//@[018:021) Identifier |k8s|
//@[021:022) Dot |.|
//@[022:028) Identifier |config|
//@[028:029) Dot |.|
//@[029:039) Identifier |kubeConfig|
//@[039:040) Dot |.|
//@[040:057) Identifier |keyVaultReference|
//@[057:059) NewLine |\r\n|
      namespace: k8s.config.namespace.value
//@[006:015) Identifier |namespace|
//@[015:016) Colon |:|
//@[017:020) Identifier |k8s|
//@[020:021) Dot |.|
//@[021:027) Identifier |config|
//@[027:028) Dot |.|
//@[028:037) Identifier |namespace|
//@[037:038) Dot |.|
//@[038:043) Identifier |value|
//@[043:045) NewLine |\r\n|
    }
//@[004:005) RightBrace |}|
//@[005:007) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

module moduleComplexKeyVaultReference 'child/hasConfigurableExtensionsWithAlias.bicep' = {
//@[000:006) Identifier |module|
//@[007:037) Identifier |moduleComplexKeyVaultReference|
//@[038:086) StringComplete |'child/hasConfigurableExtensionsWithAlias.bicep'|
//@[087:088) Assignment |=|
//@[089:090) LeftBrace |{|
//@[090:092) NewLine |\r\n|
  name: 'moduleComplexKeyVaultReference'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:040) StringComplete |'moduleComplexKeyVaultReference'|
//@[040:042) NewLine |\r\n|
  extensionConfigs: {
//@[002:018) Identifier |extensionConfigs|
//@[018:019) Colon |:|
//@[020:021) LeftBrace |{|
//@[021:023) NewLine |\r\n|
    k8s: {
//@[004:007) Identifier |k8s|
//@[007:008) Colon |:|
//@[009:010) LeftBrace |{|
//@[010:012) NewLine |\r\n|
      kubeConfig: boolParam1 ? kv1.getSecret('myKubeConfig') : scopedKv1.getSecret('myOtherKubeConfig')
//@[006:016) Identifier |kubeConfig|
//@[016:017) Colon |:|
//@[018:028) Identifier |boolParam1|
//@[029:030) Question |?|
//@[031:034) Identifier |kv1|
//@[034:035) Dot |.|
//@[035:044) Identifier |getSecret|
//@[044:045) LeftParen |(|
//@[045:059) StringComplete |'myKubeConfig'|
//@[059:060) RightParen |)|
//@[061:062) Colon |:|
//@[063:072) Identifier |scopedKv1|
//@[072:073) Dot |.|
//@[073:082) Identifier |getSecret|
//@[082:083) LeftParen |(|
//@[083:102) StringComplete |'myOtherKubeConfig'|
//@[102:103) RightParen |)|
//@[103:105) NewLine |\r\n|
      namespace: boolParam1 ? kv1.getSecret('myKubeConfig') : kv1.getSecret('myOtherKubeConfig')
//@[006:015) Identifier |namespace|
//@[015:016) Colon |:|
//@[017:027) Identifier |boolParam1|
//@[028:029) Question |?|
//@[030:033) Identifier |kv1|
//@[033:034) Dot |.|
//@[034:043) Identifier |getSecret|
//@[043:044) LeftParen |(|
//@[044:058) StringComplete |'myKubeConfig'|
//@[058:059) RightParen |)|
//@[060:061) Colon |:|
//@[062:065) Identifier |kv1|
//@[065:066) Dot |.|
//@[066:075) Identifier |getSecret|
//@[075:076) LeftParen |(|
//@[076:095) StringComplete |'myOtherKubeConfig'|
//@[095:096) RightParen |)|
//@[096:098) NewLine |\r\n|
    }
//@[004:005) RightBrace |}|
//@[005:007) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

// END: Extension configs for modules
//@[037:041) NewLine |\r\n\r\n|

// BEGIN: Outputs
//@[017:021) NewLine |\r\n\r\n|

output k8sNamespace object = k8s // This is a namespace type
//@[000:006) Identifier |output|
//@[007:019) Identifier |k8sNamespace|
//@[020:026) Identifier |object|
//@[027:028) Assignment |=|
//@[029:032) Identifier |k8s|
//@[060:064) NewLine |\r\n\r\n|

// END: Outputs
//@[015:017) NewLine |\r\n|

//@[000:000) EndOfFile ||
