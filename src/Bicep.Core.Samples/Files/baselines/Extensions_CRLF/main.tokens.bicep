// BEGIN: Parameters
//@[020:024) NewLine |\r\n\r\n|

param strParam1 string
//@[000:005) Identifier |param|
//@[006:015) Identifier |strParam1|
//@[016:022) Identifier |string|
//@[022:026) NewLine |\r\n\r\n|

@secure()
//@[000:001) At |@|
//@[001:007) Identifier |secure|
//@[007:008) LeftParen |(|
//@[008:009) RightParen |)|
//@[009:011) NewLine |\r\n|
param secureStrParam1 string
//@[000:005) Identifier |param|
//@[006:021) Identifier |secureStrParam1|
//@[022:028) Identifier |string|
//@[028:032) NewLine |\r\n\r\n|

param boolParam1 bool
//@[000:005) Identifier |param|
//@[006:016) Identifier |boolParam1|
//@[017:021) Identifier |bool|
//@[021:025) NewLine |\r\n\r\n|

// END: Parameters
//@[018:022) NewLine |\r\n\r\n|

// BEGIN: Variables
//@[019:023) NewLine |\r\n\r\n|

var strVar1 = 'strVar1Value'
//@[000:003) Identifier |var|
//@[004:011) Identifier |strVar1|
//@[012:013) Assignment |=|
//@[014:028) StringComplete |'strVar1Value'|
//@[028:030) NewLine |\r\n|
var strParamVar1 = strParam1
//@[000:003) Identifier |var|
//@[004:016) Identifier |strParamVar1|
//@[017:018) Assignment |=|
//@[019:028) Identifier |strParam1|
//@[028:032) NewLine |\r\n\r\n|

// END: Variables
//@[017:021) NewLine |\r\n\r\n|

// BEGIN: Extension declarations
//@[032:036) NewLine |\r\n\r\n|

extension az
//@[000:009) Identifier |extension|
//@[010:012) Identifier |az|
//@[012:014) NewLine |\r\n|
extension kubernetes as k8s
//@[000:009) Identifier |extension|
//@[010:020) Identifier |kubernetes|
//@[021:023) Identifier |as|
//@[024:027) Identifier |k8s|
//@[027:029) NewLine |\r\n|
extension 'br:mcr.microsoft.com/bicep/extensions/hasoptionalconfig/v1:1.2.3' as extWithOptionalConfig1
//@[000:009) Identifier |extension|
//@[010:076) StringComplete |'br:mcr.microsoft.com/bicep/extensions/hasoptionalconfig/v1:1.2.3'|
//@[077:079) Identifier |as|
//@[080:102) Identifier |extWithOptionalConfig1|
//@[102:104) NewLine |\r\n|
extension 'br:mcr.microsoft.com/bicep/extensions/hasoptionalconfig/v1:1.2.3' as extWithOptionalConfig2
//@[000:009) Identifier |extension|
//@[010:076) StringComplete |'br:mcr.microsoft.com/bicep/extensions/hasoptionalconfig/v1:1.2.3'|
//@[077:079) Identifier |as|
//@[080:102) Identifier |extWithOptionalConfig2|
//@[102:104) NewLine |\r\n|
extension 'br:mcr.microsoft.com/bicep/extensions/hasoptionalconfig/v1:1.2.3' with {
//@[000:009) Identifier |extension|
//@[010:076) StringComplete |'br:mcr.microsoft.com/bicep/extensions/hasoptionalconfig/v1:1.2.3'|
//@[077:081) Identifier |with|
//@[082:083) LeftBrace |{|
//@[083:085) NewLine |\r\n|
  optionalString: strParam1
//@[002:016) Identifier |optionalString|
//@[016:017) Colon |:|
//@[018:027) Identifier |strParam1|
//@[027:029) NewLine |\r\n|
} as extWithOptionalConfig3
//@[000:001) RightBrace |}|
//@[002:004) Identifier |as|
//@[005:027) Identifier |extWithOptionalConfig3|
//@[027:029) NewLine |\r\n|
extension 'br:mcr.microsoft.com/bicep/extensions/hassecureconfig/v1:1.2.3' with {
//@[000:009) Identifier |extension|
//@[010:074) StringComplete |'br:mcr.microsoft.com/bicep/extensions/hassecureconfig/v1:1.2.3'|
//@[075:079) Identifier |with|
//@[080:081) LeftBrace |{|
//@[081:083) NewLine |\r\n|
  requiredSecureString: secureStrParam1
//@[002:022) Identifier |requiredSecureString|
//@[022:023) Colon |:|
//@[024:039) Identifier |secureStrParam1|
//@[039:041) NewLine |\r\n|
} as extWithSecureStr1
//@[000:001) RightBrace |}|
//@[002:004) Identifier |as|
//@[005:022) Identifier |extWithSecureStr1|
//@[022:024) NewLine |\r\n|
extension 'br:mcr.microsoft.com/bicep/extensions/hasconfig/v1:1.2.3' with {
//@[000:009) Identifier |extension|
//@[010:068) StringComplete |'br:mcr.microsoft.com/bicep/extensions/hasconfig/v1:1.2.3'|
//@[069:073) Identifier |with|
//@[074:075) LeftBrace |{|
//@[075:077) NewLine |\r\n|
  requiredString: testResource1.id
//@[002:016) Identifier |requiredString|
//@[016:017) Colon |:|
//@[018:031) Identifier |testResource1|
//@[031:032) Dot |.|
//@[032:034) Identifier |id|
//@[034:036) NewLine |\r\n|
} as extWithConfig1
//@[000:001) RightBrace |}|
//@[002:004) Identifier |as|
//@[005:019) Identifier |extWithConfig1|
//@[019:021) NewLine |\r\n|
extension 'br:mcr.microsoft.com/bicep/extensions/hasconfig/v1:1.2.3' with {
//@[000:009) Identifier |extension|
//@[010:068) StringComplete |'br:mcr.microsoft.com/bicep/extensions/hasconfig/v1:1.2.3'|
//@[069:073) Identifier |with|
//@[074:075) LeftBrace |{|
//@[075:077) NewLine |\r\n|
  requiredString: boolParam1 ? strParamVar1 : strParam1
//@[002:016) Identifier |requiredString|
//@[016:017) Colon |:|
//@[018:028) Identifier |boolParam1|
//@[029:030) Question |?|
//@[031:043) Identifier |strParamVar1|
//@[044:045) Colon |:|
//@[046:055) Identifier |strParam1|
//@[055:057) NewLine |\r\n|
} as extWithConfig2
//@[000:001) RightBrace |}|
//@[002:004) Identifier |as|
//@[005:019) Identifier |extWithConfig2|
//@[019:023) NewLine |\r\n\r\n|

// END: Extension declarations
//@[030:034) NewLine |\r\n\r\n|

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
  scope: az.resourceGroup('otherGroup')
//@[002:007) Identifier |scope|
//@[007:008) Colon |:|
//@[009:011) Identifier |az|
//@[011:012) Dot |.|
//@[012:025) Identifier |resourceGroup|
//@[025:026) LeftParen |(|
//@[026:038) StringComplete |'otherGroup'|
//@[038:039) RightParen |)|
//@[039:041) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

// END: Key vaults
//@[018:022) NewLine |\r\n\r\n|

// BEGIN: Test resources
//@[024:028) NewLine |\r\n\r\n|

resource testResource1 'az:My.Rp/TestType@2020-01-01' = {
//@[000:008) Identifier |resource|
//@[009:022) Identifier |testResource1|
//@[023:053) StringComplete |'az:My.Rp/TestType@2020-01-01'|
//@[054:055) Assignment |=|
//@[056:057) LeftBrace |{|
//@[057:059) NewLine |\r\n|
  name: 'testResource1'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:023) StringComplete |'testResource1'|
//@[023:025) NewLine |\r\n|
  properties: {}
//@[002:012) Identifier |properties|
//@[012:013) Colon |:|
//@[014:015) LeftBrace |{|
//@[015:016) RightBrace |}|
//@[016:018) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

resource aks 'Microsoft.ContainerService/managedClusters@2024-02-01' = {
//@[000:008) Identifier |resource|
//@[009:012) Identifier |aks|
//@[013:068) StringComplete |'Microsoft.ContainerService/managedClusters@2024-02-01'|
//@[069:070) Assignment |=|
//@[071:072) LeftBrace |{|
//@[072:074) NewLine |\r\n|
  name: 'aksCluster'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:020) StringComplete |'aksCluster'|
//@[020:022) NewLine |\r\n|
  location: az.resourceGroup().location
//@[002:010) Identifier |location|
//@[010:011) Colon |:|
//@[012:014) Identifier |az|
//@[014:015) Dot |.|
//@[015:028) Identifier |resourceGroup|
//@[028:029) LeftParen |(|
//@[029:030) RightParen |)|
//@[030:031) Dot |.|
//@[031:039) Identifier |location|
//@[039:041) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

// END: Test resources
//@[022:026) NewLine |\r\n\r\n|

// BEGIN: Extension configs for modules
//@[039:043) NewLine |\r\n\r\n|

module moduleWithExtsWithAliases 'child/hasConfigurableExtensionsWithAlias.bicep' = {
//@[000:006) Identifier |module|
//@[007:032) Identifier |moduleWithExtsWithAliases|
//@[033:081) StringComplete |'child/hasConfigurableExtensionsWithAlias.bicep'|
//@[082:083) Assignment |=|
//@[084:085) LeftBrace |{|
//@[085:087) NewLine |\r\n|
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
      kubeConfig: 'kubeConfig2'
//@[006:016) Identifier |kubeConfig|
//@[016:017) Colon |:|
//@[018:031) StringComplete |'kubeConfig2'|
//@[031:033) NewLine |\r\n|
      namespace: 'ns2'
//@[006:015) Identifier |namespace|
//@[015:016) Colon |:|
//@[017:022) StringComplete |'ns2'|
//@[022:024) NewLine |\r\n|
    }
//@[004:005) RightBrace |}|
//@[005:007) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

module moduleWithExtsWithoutAliases 'child/hasConfigurableExtensionsWithoutAlias.bicep' = {
//@[000:006) Identifier |module|
//@[007:035) Identifier |moduleWithExtsWithoutAliases|
//@[036:087) StringComplete |'child/hasConfigurableExtensionsWithoutAlias.bicep'|
//@[088:089) Assignment |=|
//@[090:091) LeftBrace |{|
//@[091:093) NewLine |\r\n|
  extensionConfigs: {
//@[002:018) Identifier |extensionConfigs|
//@[018:019) Colon |:|
//@[020:021) LeftBrace |{|
//@[021:023) NewLine |\r\n|
    kubernetes: {
//@[004:014) Identifier |kubernetes|
//@[014:015) Colon |:|
//@[016:017) LeftBrace |{|
//@[017:019) NewLine |\r\n|
      kubeConfig: 'kubeConfig2'
//@[006:016) Identifier |kubeConfig|
//@[016:017) Colon |:|
//@[018:031) StringComplete |'kubeConfig2'|
//@[031:033) NewLine |\r\n|
    }
//@[004:005) RightBrace |}|
//@[005:007) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

module moduleExtConfigsFromParams 'child/hasConfigurableExtensionsWithAlias.bicep' = {
//@[000:006) Identifier |module|
//@[007:033) Identifier |moduleExtConfigsFromParams|
//@[034:082) StringComplete |'child/hasConfigurableExtensionsWithAlias.bicep'|
//@[083:084) Assignment |=|
//@[085:086) LeftBrace |{|
//@[086:088) NewLine |\r\n|
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
      kubeConfig: boolParam1 ? secureStrParam1 : strParam1
//@[006:016) Identifier |kubeConfig|
//@[016:017) Colon |:|
//@[018:028) Identifier |boolParam1|
//@[029:030) Question |?|
//@[031:046) Identifier |secureStrParam1|
//@[047:048) Colon |:|
//@[049:058) Identifier |strParam1|
//@[058:060) NewLine |\r\n|
      namespace: boolParam1 ? strParam1 : 'falseCond'
//@[006:015) Identifier |namespace|
//@[015:016) Colon |:|
//@[017:027) Identifier |boolParam1|
//@[028:029) Question |?|
//@[030:039) Identifier |strParam1|
//@[040:041) Colon |:|
//@[042:053) StringComplete |'falseCond'|
//@[053:055) NewLine |\r\n|
    }
//@[004:005) RightBrace |}|
//@[005:007) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

module moduleExtConfigFromKeyVaultReference 'child/hasConfigurableExtensionsWithAlias.bicep' = {
//@[000:006) Identifier |module|
//@[007:043) Identifier |moduleExtConfigFromKeyVaultReference|
//@[044:092) StringComplete |'child/hasConfigurableExtensionsWithAlias.bicep'|
//@[093:094) Assignment |=|
//@[095:096) LeftBrace |{|
//@[096:098) NewLine |\r\n|
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
      kubeConfig: kv1.getSecret('myKubeConfig')
//@[006:016) Identifier |kubeConfig|
//@[016:017) Colon |:|
//@[018:021) Identifier |kv1|
//@[021:022) Dot |.|
//@[022:031) Identifier |getSecret|
//@[031:032) LeftParen |(|
//@[032:046) StringComplete |'myKubeConfig'|
//@[046:047) RightParen |)|
//@[047:049) NewLine |\r\n|
      namespace: strVar1
//@[006:015) Identifier |namespace|
//@[015:016) Colon |:|
//@[017:024) Identifier |strVar1|
//@[024:026) NewLine |\r\n|
    }
//@[004:005) RightBrace |}|
//@[005:007) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

module moduleExtConfigFromReferences 'child/hasConfigurableExtensionsWithAlias.bicep' = {
//@[000:006) Identifier |module|
//@[007:036) Identifier |moduleExtConfigFromReferences|
//@[037:085) StringComplete |'child/hasConfigurableExtensionsWithAlias.bicep'|
//@[086:087) Assignment |=|
//@[088:089) LeftBrace |{|
//@[089:091) NewLine |\r\n|
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
      kubeConfig: aks.listClusterAdminCredential().kubeconfigs[0].value
//@[006:016) Identifier |kubeConfig|
//@[016:017) Colon |:|
//@[018:021) Identifier |aks|
//@[021:022) Dot |.|
//@[022:048) Identifier |listClusterAdminCredential|
//@[048:049) LeftParen |(|
//@[049:050) RightParen |)|
//@[050:051) Dot |.|
//@[051:062) Identifier |kubeconfigs|
//@[062:063) LeftSquare |[|
//@[063:064) Integer |0|
//@[064:065) RightSquare |]|
//@[065:066) Dot |.|
//@[066:071) Identifier |value|
//@[071:073) NewLine |\r\n|
      namespace: testResource1.properties.namespace
//@[006:015) Identifier |namespace|
//@[015:016) Colon |:|
//@[017:030) Identifier |testResource1|
//@[030:031) Dot |.|
//@[031:041) Identifier |properties|
//@[041:042) Dot |.|
//@[042:051) Identifier |namespace|
//@[051:053) NewLine |\r\n|
    }
//@[004:005) RightBrace |}|
//@[005:007) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

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
    k8s: k8s.config
//@[004:007) Identifier |k8s|
//@[007:008) Colon |:|
//@[009:012) Identifier |k8s|
//@[012:013) Dot |.|
//@[013:019) Identifier |config|
//@[019:021) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

module moduleWithExtsUsingFullInheritanceTernary1 'child/hasConfigurableExtensionsWithAlias.bicep' = {
//@[000:006) Identifier |module|
//@[007:049) Identifier |moduleWithExtsUsingFullInheritanceTernary1|
//@[050:098) StringComplete |'child/hasConfigurableExtensionsWithAlias.bicep'|
//@[099:100) Assignment |=|
//@[101:102) LeftBrace |{|
//@[102:104) NewLine |\r\n|
  extensionConfigs: {
//@[002:018) Identifier |extensionConfigs|
//@[018:019) Colon |:|
//@[020:021) LeftBrace |{|
//@[021:023) NewLine |\r\n|
    k8s: k8s.config
//@[004:007) Identifier |k8s|
//@[007:008) Colon |:|
//@[009:012) Identifier |k8s|
//@[012:013) Dot |.|
//@[013:019) Identifier |config|
//@[019:021) NewLine |\r\n|
    extWithOptionalConfig: boolParam1 ? extWithOptionalConfig1.config : extWithOptionalConfig2.config
//@[004:025) Identifier |extWithOptionalConfig|
//@[025:026) Colon |:|
//@[027:037) Identifier |boolParam1|
//@[038:039) Question |?|
//@[040:062) Identifier |extWithOptionalConfig1|
//@[062:063) Dot |.|
//@[063:069) Identifier |config|
//@[070:071) Colon |:|
//@[072:094) Identifier |extWithOptionalConfig2|
//@[094:095) Dot |.|
//@[095:101) Identifier |config|
//@[101:103) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

module moduleWithExtsUsingPiecemealInheritance 'child/hasConfigurableExtensionsWithAlias.bicep' = {
//@[000:006) Identifier |module|
//@[007:046) Identifier |moduleWithExtsUsingPiecemealInheritance|
//@[047:095) StringComplete |'child/hasConfigurableExtensionsWithAlias.bicep'|
//@[096:097) Assignment |=|
//@[098:099) LeftBrace |{|
//@[099:101) NewLine |\r\n|
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
      kubeConfig: k8s.config.kubeConfig
//@[006:016) Identifier |kubeConfig|
//@[016:017) Colon |:|
//@[018:021) Identifier |k8s|
//@[021:022) Dot |.|
//@[022:028) Identifier |config|
//@[028:029) Dot |.|
//@[029:039) Identifier |kubeConfig|
//@[039:041) NewLine |\r\n|
      namespace: k8s.config.namespace
//@[006:015) Identifier |namespace|
//@[015:016) Colon |:|
//@[017:020) Identifier |k8s|
//@[020:021) Dot |.|
//@[021:027) Identifier |config|
//@[027:028) Dot |.|
//@[028:037) Identifier |namespace|
//@[037:039) NewLine |\r\n|
    }
//@[004:005) RightBrace |}|
//@[005:007) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

module moduleWithExtsUsingPiecemealInheritanceLooped 'child/hasConfigurableExtensionsWithAlias.bicep' = [for i in range(0, 4): {
//@[000:006) Identifier |module|
//@[007:052) Identifier |moduleWithExtsUsingPiecemealInheritanceLooped|
//@[053:101) StringComplete |'child/hasConfigurableExtensionsWithAlias.bicep'|
//@[102:103) Assignment |=|
//@[104:105) LeftSquare |[|
//@[105:108) Identifier |for|
//@[109:110) Identifier |i|
//@[111:113) Identifier |in|
//@[114:119) Identifier |range|
//@[119:120) LeftParen |(|
//@[120:121) Integer |0|
//@[121:122) Comma |,|
//@[123:124) Integer |4|
//@[124:125) RightParen |)|
//@[125:126) Colon |:|
//@[127:128) LeftBrace |{|
//@[128:130) NewLine |\r\n|
  name: 'moduleWithExtsPiecemealInheritanceLooped${i}'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:051) StringLeftPiece |'moduleWithExtsPiecemealInheritanceLooped${|
//@[051:052) Identifier |i|
//@[052:054) StringRightPiece |}'|
//@[054:056) NewLine |\r\n|
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
      kubeConfig: k8s.config.kubeConfig
//@[006:016) Identifier |kubeConfig|
//@[016:017) Colon |:|
//@[018:021) Identifier |k8s|
//@[021:022) Dot |.|
//@[022:028) Identifier |config|
//@[028:029) Dot |.|
//@[029:039) Identifier |kubeConfig|
//@[039:041) NewLine |\r\n|
      namespace: k8s.config.namespace
//@[006:015) Identifier |namespace|
//@[015:016) Colon |:|
//@[017:020) Identifier |k8s|
//@[020:021) Dot |.|
//@[021:027) Identifier |config|
//@[027:028) Dot |.|
//@[028:037) Identifier |namespace|
//@[037:039) NewLine |\r\n|
    }
//@[004:005) RightBrace |}|
//@[005:007) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
}]
//@[000:001) RightBrace |}|
//@[001:002) RightSquare |]|
//@[002:006) NewLine |\r\n\r\n|

module moduleExtConfigsConditionalMixed 'child/hasConfigurableExtensionsWithAlias.bicep' = {
//@[000:006) Identifier |module|
//@[007:039) Identifier |moduleExtConfigsConditionalMixed|
//@[040:088) StringComplete |'child/hasConfigurableExtensionsWithAlias.bicep'|
//@[089:090) Assignment |=|
//@[091:092) LeftBrace |{|
//@[092:094) NewLine |\r\n|
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
      kubeConfig: boolParam1 ? secureStrParam1 : k8s.config.kubeConfig
//@[006:016) Identifier |kubeConfig|
//@[016:017) Colon |:|
//@[018:028) Identifier |boolParam1|
//@[029:030) Question |?|
//@[031:046) Identifier |secureStrParam1|
//@[047:048) Colon |:|
//@[049:052) Identifier |k8s|
//@[052:053) Dot |.|
//@[053:059) Identifier |config|
//@[059:060) Dot |.|
//@[060:070) Identifier |kubeConfig|
//@[070:072) NewLine |\r\n|
      namespace: boolParam1 ? az.resourceGroup().location : k8s.config.namespace
//@[006:015) Identifier |namespace|
//@[015:016) Colon |:|
//@[017:027) Identifier |boolParam1|
//@[028:029) Question |?|
//@[030:032) Identifier |az|
//@[032:033) Dot |.|
//@[033:046) Identifier |resourceGroup|
//@[046:047) LeftParen |(|
//@[047:048) RightParen |)|
//@[048:049) Dot |.|
//@[049:057) Identifier |location|
//@[058:059) Colon |:|
//@[060:063) Identifier |k8s|
//@[063:064) Dot |.|
//@[064:070) Identifier |config|
//@[070:071) Dot |.|
//@[071:080) Identifier |namespace|
//@[080:082) NewLine |\r\n|
    }
//@[004:005) RightBrace |}|
//@[005:007) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

module moduleWithExtsEmpty 'child/hasConfigurableExtensionsWithAlias.bicep' = {
//@[000:006) Identifier |module|
//@[007:026) Identifier |moduleWithExtsEmpty|
//@[027:075) StringComplete |'child/hasConfigurableExtensionsWithAlias.bicep'|
//@[076:077) Assignment |=|
//@[078:079) LeftBrace |{|
//@[079:081) NewLine |\r\n|
  extensionConfigs: {
//@[002:018) Identifier |extensionConfigs|
//@[018:019) Colon |:|
//@[020:021) LeftBrace |{|
//@[021:023) NewLine |\r\n|
    k8s: k8s.config
//@[004:007) Identifier |k8s|
//@[007:008) Colon |:|
//@[009:012) Identifier |k8s|
//@[012:013) Dot |.|
//@[013:019) Identifier |config|
//@[019:021) NewLine |\r\n|
    extWithOptionalConfig: {}
//@[004:025) Identifier |extWithOptionalConfig|
//@[025:026) Colon |:|
//@[027:028) LeftBrace |{|
//@[028:029) RightBrace |}|
//@[029:031) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

// END: Extension configs for modules
//@[037:039) NewLine |\r\n|

//@[000:000) EndOfFile ||
