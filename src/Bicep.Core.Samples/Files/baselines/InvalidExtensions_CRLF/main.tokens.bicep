// BEGIN: Parameters
//@[020:024) NewLine |\r\n\r\n|

param boolParam1 bool
//@[000:005) Identifier |param|
//@[006:016) Identifier |boolParam1|
//@[017:021) Identifier |bool|
//@[021:023) NewLine |\r\n|
param strParam1 string
//@[000:005) Identifier |param|
//@[006:015) Identifier |strParam1|
//@[016:022) Identifier |string|
//@[022:026) NewLine |\r\n\r\n|

// END: Parameters
//@[018:022) NewLine |\r\n\r\n|

// BEGIN: Valid Extension declarations
//@[038:042) NewLine |\r\n\r\n|

extension az
//@[000:009) Identifier |extension|
//@[010:012) Identifier |az|
//@[012:014) NewLine |\r\n|
extension kubernetes as k8s
//@[000:009) Identifier |extension|
//@[010:020) Identifier |kubernetes|
//@[021:023) Identifier |as|
//@[024:027) Identifier |k8s|
//@[027:031) NewLine |\r\n\r\n|

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

// BEGIN: Resources
//@[019:023) NewLine |\r\n\r\n|

var configProp = 'config'
//@[000:003) Identifier |var|
//@[004:014) Identifier |configProp|
//@[015:016) Assignment |=|
//@[017:025) StringComplete |'config'|
//@[025:029) NewLine |\r\n\r\n|

// Extension symbols are blocked in resources because each config property returns an object { value, keyVaultReference } and "value" is not available when a reference is provided.
//@[180:182) NewLine |\r\n|
// Users should use deployment parameters for this scenario.
//@[060:062) NewLine |\r\n|
resource testResource1 'az:My.Rp/TestType@2020-01-01' = {
//@[000:008) Identifier |resource|
//@[009:022) Identifier |testResource1|
//@[023:053) StringComplete |'az:My.Rp/TestType@2020-01-01'|
//@[054:055) Assignment |=|
//@[056:057) LeftBrace |{|
//@[057:059) NewLine |\r\n|
  name: k8s.config.namespace
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:011) Identifier |k8s|
//@[011:012) Dot |.|
//@[012:018) Identifier |config|
//@[018:019) Dot |.|
//@[019:028) Identifier |namespace|
//@[028:030) NewLine |\r\n|
  properties: {
//@[002:012) Identifier |properties|
//@[012:013) Colon |:|
//@[014:015) LeftBrace |{|
//@[015:017) NewLine |\r\n|
    secret: k8s.config.kubeConfig
//@[004:010) Identifier |secret|
//@[010:011) Colon |:|
//@[012:015) Identifier |k8s|
//@[015:016) Dot |.|
//@[016:022) Identifier |config|
//@[022:023) Dot |.|
//@[023:033) Identifier |kubeConfig|
//@[033:035) NewLine |\r\n|
    ns: k8s[configProp].namespace
//@[004:006) Identifier |ns|
//@[006:007) Colon |:|
//@[008:011) Identifier |k8s|
//@[011:012) LeftSquare |[|
//@[012:022) Identifier |configProp|
//@[022:023) RightSquare |]|
//@[023:024) Dot |.|
//@[024:033) Identifier |namespace|
//@[033:035) NewLine |\r\n|
    ref: k8s[kv1.properties.sku.name].namespace
//@[004:007) Identifier |ref|
//@[007:008) Colon |:|
//@[009:012) Identifier |k8s|
//@[012:013) LeftSquare |[|
//@[013:016) Identifier |kv1|
//@[016:017) Dot |.|
//@[017:027) Identifier |properties|
//@[027:028) Dot |.|
//@[028:031) Identifier |sku|
//@[031:032) Dot |.|
//@[032:036) Identifier |name|
//@[036:037) RightSquare |]|
//@[037:038) Dot |.|
//@[038:047) Identifier |namespace|
//@[047:049) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

// END: Resources
//@[017:021) NewLine |\r\n\r\n|

// BEGIN: Extension configs for modules
//@[039:043) NewLine |\r\n\r\n|

module moduleWithExtsUsingFullInheritance 'child/hasConfigurableExtensionsWithAlias.bicep' = {
//@[000:006) Identifier |module|
//@[007:041) Identifier |moduleWithExtsUsingFullInheritance|
//@[042:090) StringComplete |'child/hasConfigurableExtensionsWithAlias.bicep'|
//@[091:092) Assignment |=|
//@[093:094) LeftBrace |{|
//@[094:096) NewLine |\r\n|
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

var invalidVarAssignment1 = k8s.config.namespace
//@[000:003) Identifier |var|
//@[004:025) Identifier |invalidVarAssignment1|
//@[026:027) Assignment |=|
//@[028:031) Identifier |k8s|
//@[031:032) Dot |.|
//@[032:038) Identifier |config|
//@[038:039) Dot |.|
//@[039:048) Identifier |namespace|
//@[048:050) NewLine |\r\n|
var invalidVarAssignment2 = k8s.config.kubeConfig
//@[000:003) Identifier |var|
//@[004:025) Identifier |invalidVarAssignment2|
//@[026:027) Assignment |=|
//@[028:031) Identifier |k8s|
//@[031:032) Dot |.|
//@[032:038) Identifier |config|
//@[038:039) Dot |.|
//@[039:049) Identifier |kubeConfig|
//@[049:053) NewLine |\r\n\r\n|

var extensionConfigsVar = {
//@[000:003) Identifier |var|
//@[004:023) Identifier |extensionConfigsVar|
//@[024:025) Assignment |=|
//@[026:027) LeftBrace |{|
//@[027:029) NewLine |\r\n|
  k8s: {
//@[002:005) Identifier |k8s|
//@[005:006) Colon |:|
//@[007:008) LeftBrace |{|
//@[008:010) NewLine |\r\n|
    kubeConfig: 'inlined',
//@[004:014) Identifier |kubeConfig|
//@[014:015) Colon |:|
//@[016:025) StringComplete |'inlined'|
//@[025:026) Comma |,|
//@[026:028) NewLine |\r\n|
    namespace: 'inlined'
//@[004:013) Identifier |namespace|
//@[013:014) Colon |:|
//@[015:024) StringComplete |'inlined'|
//@[024:026) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

var k8sConfigDeployTime = {
//@[000:003) Identifier |var|
//@[004:023) Identifier |k8sConfigDeployTime|
//@[024:025) Assignment |=|
//@[026:027) LeftBrace |{|
//@[027:029) NewLine |\r\n|
  kubeConfig: strParam1
//@[002:012) Identifier |kubeConfig|
//@[012:013) Colon |:|
//@[014:023) Identifier |strParam1|
//@[023:025) NewLine |\r\n|
  namespace: strParam1
//@[002:011) Identifier |namespace|
//@[011:012) Colon |:|
//@[013:022) Identifier |strParam1|
//@[022:024) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

module moduleWithExtsUsingVar1 'child/hasConfigurableExtensionsWithAlias.bicep' = {
//@[000:006) Identifier |module|
//@[007:030) Identifier |moduleWithExtsUsingVar1|
//@[031:079) StringComplete |'child/hasConfigurableExtensionsWithAlias.bicep'|
//@[080:081) Assignment |=|
//@[082:083) LeftBrace |{|
//@[083:085) NewLine |\r\n|
  extensionConfigs: extensionConfigsVar
//@[002:018) Identifier |extensionConfigs|
//@[018:019) Colon |:|
//@[020:039) Identifier |extensionConfigsVar|
//@[039:041) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

module moduleWithExtsUsingVar2 'child/hasConfigurableExtensionsWithAlias.bicep' = {
//@[000:006) Identifier |module|
//@[007:030) Identifier |moduleWithExtsUsingVar2|
//@[031:079) StringComplete |'child/hasConfigurableExtensionsWithAlias.bicep'|
//@[080:081) Assignment |=|
//@[082:083) LeftBrace |{|
//@[083:085) NewLine |\r\n|
  extensionConfigs: {
//@[002:018) Identifier |extensionConfigs|
//@[018:019) Colon |:|
//@[020:021) LeftBrace |{|
//@[021:023) NewLine |\r\n|
    k8s: k8sConfigDeployTime
//@[004:007) Identifier |k8s|
//@[007:008) Colon |:|
//@[009:028) Identifier |k8sConfigDeployTime|
//@[028:030) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

module moduleInvalidSpread1 'child/hasConfigurableExtensionsWithAlias.bicep' = {
//@[000:006) Identifier |module|
//@[007:027) Identifier |moduleInvalidSpread1|
//@[028:076) StringComplete |'child/hasConfigurableExtensionsWithAlias.bicep'|
//@[077:078) Assignment |=|
//@[079:080) LeftBrace |{|
//@[080:082) NewLine |\r\n|
  extensionConfigs: {
//@[002:018) Identifier |extensionConfigs|
//@[018:019) Colon |:|
//@[020:021) LeftBrace |{|
//@[021:023) NewLine |\r\n|
    ...extensionConfigsVar
//@[004:007) Ellipsis |...|
//@[007:026) Identifier |extensionConfigsVar|
//@[026:028) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

module moduleInvalidSpread2 'child/hasConfigurableExtensionsWithAlias.bicep' = {
//@[000:006) Identifier |module|
//@[007:027) Identifier |moduleInvalidSpread2|
//@[028:076) StringComplete |'child/hasConfigurableExtensionsWithAlias.bicep'|
//@[077:078) Assignment |=|
//@[079:080) LeftBrace |{|
//@[080:082) NewLine |\r\n|
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
      ...k8sConfigDeployTime
//@[006:009) Ellipsis |...|
//@[009:028) Identifier |k8sConfigDeployTime|
//@[028:030) NewLine |\r\n|
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

// Extension symbols are blocked for outputs for now. Users should use deployment parameters for this scenario.
//@[111:115) NewLine |\r\n\r\n|

output k8sTheNamespace object = k8s // This is a namespace type
//@[000:006) Identifier |output|
//@[007:022) Identifier |k8sTheNamespace|
//@[023:029) Identifier |object|
//@[030:031) Assignment |=|
//@[032:035) Identifier |k8s|
//@[063:067) NewLine |\r\n\r\n|

output k8sConfig object = k8s.config
//@[000:006) Identifier |output|
//@[007:016) Identifier |k8sConfig|
//@[017:023) Identifier |object|
//@[024:025) Assignment |=|
//@[026:029) Identifier |k8s|
//@[029:030) Dot |.|
//@[030:036) Identifier |config|
//@[036:040) NewLine |\r\n\r\n|

output k8sNamespace string = k8s.config.namespace
//@[000:006) Identifier |output|
//@[007:019) Identifier |k8sNamespace|
//@[020:026) Identifier |string|
//@[027:028) Assignment |=|
//@[029:032) Identifier |k8s|
//@[032:033) Dot |.|
//@[033:039) Identifier |config|
//@[039:040) Dot |.|
//@[040:049) Identifier |namespace|
//@[049:053) NewLine |\r\n\r\n|

// END: Outputs
//@[015:017) NewLine |\r\n|

//@[000:000) EndOfFile ||
