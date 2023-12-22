targetScope = 'subscription'
//@[000:011) Identifier |targetScope|
//@[012:013) Assignment |=|
//@[014:028) StringComplete |'subscription'|
//@[028:030) NewLine |\n\n|

resource rg 'Microsoft.Resources/resourceGroups@2020-06-01' = {
//@[000:008) Identifier |resource|
//@[009:011) Identifier |rg|
//@[012:059) StringComplete |'Microsoft.Resources/resourceGroups@2020-06-01'|
//@[060:061) Assignment |=|
//@[062:063) LeftBrace |{|
//@[063:064) NewLine |\n|
  name: 'adotfrank-rg'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:022) StringComplete |'adotfrank-rg'|
//@[022:023) NewLine |\n|
  location: deployment().location
//@[002:010) Identifier |location|
//@[010:011) Colon |:|
//@[012:022) Identifier |deployment|
//@[022:023) LeftParen |(|
//@[023:024) RightParen |)|
//@[024:025) Dot |.|
//@[025:033) Identifier |location|
//@[033:034) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

module appPlanDeploy 'br:mock-registry-one.invalid/demo/plan:v2' = {
//@[000:006) Identifier |module|
//@[007:020) Identifier |appPlanDeploy|
//@[021:064) StringComplete |'br:mock-registry-one.invalid/demo/plan:v2'|
//@[065:066) Assignment |=|
//@[067:068) LeftBrace |{|
//@[068:069) NewLine |\n|
  name: 'planDeploy'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:020) StringComplete |'planDeploy'|
//@[020:021) NewLine |\n|
  scope: rg
//@[002:007) Identifier |scope|
//@[007:008) Colon |:|
//@[009:011) Identifier |rg|
//@[011:012) NewLine |\n|
  params: {
//@[002:008) Identifier |params|
//@[008:009) Colon |:|
//@[010:011) LeftBrace |{|
//@[011:012) NewLine |\n|
    namePrefix: 'hello'
//@[004:014) Identifier |namePrefix|
//@[014:015) Colon |:|
//@[016:023) StringComplete |'hello'|
//@[023:024) NewLine |\n|
  }
//@[002:003) RightBrace |}|
//@[003:004) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

module appPlanDeploy2 'br/mock-registry-one:demo/plan:v2' = {
//@[000:006) Identifier |module|
//@[007:021) Identifier |appPlanDeploy2|
//@[022:057) StringComplete |'br/mock-registry-one:demo/plan:v2'|
//@[058:059) Assignment |=|
//@[060:061) LeftBrace |{|
//@[061:062) NewLine |\n|
  name: 'planDeploy2'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:021) StringComplete |'planDeploy2'|
//@[021:022) NewLine |\n|
  scope: rg
//@[002:007) Identifier |scope|
//@[007:008) Colon |:|
//@[009:011) Identifier |rg|
//@[011:012) NewLine |\n|
  params: {
//@[002:008) Identifier |params|
//@[008:009) Colon |:|
//@[010:011) LeftBrace |{|
//@[011:012) NewLine |\n|
    namePrefix: 'hello'
//@[004:014) Identifier |namePrefix|
//@[014:015) Colon |:|
//@[016:023) StringComplete |'hello'|
//@[023:024) NewLine |\n|
  }
//@[002:003) RightBrace |}|
//@[003:004) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

var websites = [
//@[000:003) Identifier |var|
//@[004:012) Identifier |websites|
//@[013:014) Assignment |=|
//@[015:016) LeftSquare |[|
//@[016:017) NewLine |\n|
  {
//@[002:003) LeftBrace |{|
//@[003:004) NewLine |\n|
    name: 'fancy'
//@[004:008) Identifier |name|
//@[008:009) Colon |:|
//@[010:017) StringComplete |'fancy'|
//@[017:018) NewLine |\n|
    tag: 'latest'
//@[004:007) Identifier |tag|
//@[007:008) Colon |:|
//@[009:017) StringComplete |'latest'|
//@[017:018) NewLine |\n|
  }
//@[002:003) RightBrace |}|
//@[003:004) NewLine |\n|
  {
//@[002:003) LeftBrace |{|
//@[003:004) NewLine |\n|
    name: 'plain'
//@[004:008) Identifier |name|
//@[008:009) Colon |:|
//@[010:017) StringComplete |'plain'|
//@[017:018) NewLine |\n|
    tag: 'plain-text'
//@[004:007) Identifier |tag|
//@[007:008) Colon |:|
//@[009:021) StringComplete |'plain-text'|
//@[021:022) NewLine |\n|
  }
//@[002:003) RightBrace |}|
//@[003:004) NewLine |\n|
]
//@[000:001) RightSquare |]|
//@[001:003) NewLine |\n\n|

module siteDeploy 'br:mock-registry-two.invalid/demo/site:v3' = [for site in websites: {
//@[000:006) Identifier |module|
//@[007:017) Identifier |siteDeploy|
//@[018:061) StringComplete |'br:mock-registry-two.invalid/demo/site:v3'|
//@[062:063) Assignment |=|
//@[064:065) LeftSquare |[|
//@[065:068) Identifier |for|
//@[069:073) Identifier |site|
//@[074:076) Identifier |in|
//@[077:085) Identifier |websites|
//@[085:086) Colon |:|
//@[087:088) LeftBrace |{|
//@[088:089) NewLine |\n|
  name: '${site.name}siteDeploy'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:011) StringLeftPiece |'${|
//@[011:015) Identifier |site|
//@[015:016) Dot |.|
//@[016:020) Identifier |name|
//@[020:032) StringRightPiece |}siteDeploy'|
//@[032:033) NewLine |\n|
  scope: rg
//@[002:007) Identifier |scope|
//@[007:008) Colon |:|
//@[009:011) Identifier |rg|
//@[011:012) NewLine |\n|
  params: {
//@[002:008) Identifier |params|
//@[008:009) Colon |:|
//@[010:011) LeftBrace |{|
//@[011:012) NewLine |\n|
    appPlanId: appPlanDeploy.outputs.planId
//@[004:013) Identifier |appPlanId|
//@[013:014) Colon |:|
//@[015:028) Identifier |appPlanDeploy|
//@[028:029) Dot |.|
//@[029:036) Identifier |outputs|
//@[036:037) Dot |.|
//@[037:043) Identifier |planId|
//@[043:044) NewLine |\n|
    namePrefix: site.name
//@[004:014) Identifier |namePrefix|
//@[014:015) Colon |:|
//@[016:020) Identifier |site|
//@[020:021) Dot |.|
//@[021:025) Identifier |name|
//@[025:026) NewLine |\n|
    dockerImage: 'nginxdemos/hello'
//@[004:015) Identifier |dockerImage|
//@[015:016) Colon |:|
//@[017:035) StringComplete |'nginxdemos/hello'|
//@[035:036) NewLine |\n|
    dockerImageTag: site.tag
//@[004:018) Identifier |dockerImageTag|
//@[018:019) Colon |:|
//@[020:024) Identifier |site|
//@[024:025) Dot |.|
//@[025:028) Identifier |tag|
//@[028:029) NewLine |\n|
  }
//@[002:003) RightBrace |}|
//@[003:004) NewLine |\n|
}]
//@[000:001) RightBrace |}|
//@[001:002) RightSquare |]|
//@[002:004) NewLine |\n\n|

module siteDeploy2 'br/demo-two:site:v3' = [for site in websites: {
//@[000:006) Identifier |module|
//@[007:018) Identifier |siteDeploy2|
//@[019:040) StringComplete |'br/demo-two:site:v3'|
//@[041:042) Assignment |=|
//@[043:044) LeftSquare |[|
//@[044:047) Identifier |for|
//@[048:052) Identifier |site|
//@[053:055) Identifier |in|
//@[056:064) Identifier |websites|
//@[064:065) Colon |:|
//@[066:067) LeftBrace |{|
//@[067:068) NewLine |\n|
  name: '${site.name}siteDeploy2'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:011) StringLeftPiece |'${|
//@[011:015) Identifier |site|
//@[015:016) Dot |.|
//@[016:020) Identifier |name|
//@[020:033) StringRightPiece |}siteDeploy2'|
//@[033:034) NewLine |\n|
  scope: rg
//@[002:007) Identifier |scope|
//@[007:008) Colon |:|
//@[009:011) Identifier |rg|
//@[011:012) NewLine |\n|
  params: {
//@[002:008) Identifier |params|
//@[008:009) Colon |:|
//@[010:011) LeftBrace |{|
//@[011:012) NewLine |\n|
    appPlanId: appPlanDeploy.outputs.planId
//@[004:013) Identifier |appPlanId|
//@[013:014) Colon |:|
//@[015:028) Identifier |appPlanDeploy|
//@[028:029) Dot |.|
//@[029:036) Identifier |outputs|
//@[036:037) Dot |.|
//@[037:043) Identifier |planId|
//@[043:044) NewLine |\n|
    namePrefix: site.name
//@[004:014) Identifier |namePrefix|
//@[014:015) Colon |:|
//@[016:020) Identifier |site|
//@[020:021) Dot |.|
//@[021:025) Identifier |name|
//@[025:026) NewLine |\n|
    dockerImage: 'nginxdemos/hello'
//@[004:015) Identifier |dockerImage|
//@[015:016) Colon |:|
//@[017:035) StringComplete |'nginxdemos/hello'|
//@[035:036) NewLine |\n|
    dockerImageTag: site.tag
//@[004:018) Identifier |dockerImageTag|
//@[018:019) Colon |:|
//@[020:024) Identifier |site|
//@[024:025) Dot |.|
//@[025:028) Identifier |tag|
//@[028:029) NewLine |\n|
  }
//@[002:003) RightBrace |}|
//@[003:004) NewLine |\n|
}]
//@[000:001) RightBrace |}|
//@[001:002) RightSquare |]|
//@[002:004) NewLine |\n\n|

module storageDeploy 'ts:00000000-0000-0000-0000-000000000000/test-rg/storage-spec:1.0' = {
//@[000:006) Identifier |module|
//@[007:020) Identifier |storageDeploy|
//@[021:087) StringComplete |'ts:00000000-0000-0000-0000-000000000000/test-rg/storage-spec:1.0'|
//@[088:089) Assignment |=|
//@[090:091) LeftBrace |{|
//@[091:092) NewLine |\n|
  name: 'storageDeploy'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:023) StringComplete |'storageDeploy'|
//@[023:024) NewLine |\n|
  scope: rg
//@[002:007) Identifier |scope|
//@[007:008) Colon |:|
//@[009:011) Identifier |rg|
//@[011:012) NewLine |\n|
  params: {
//@[002:008) Identifier |params|
//@[008:009) Colon |:|
//@[010:011) LeftBrace |{|
//@[011:012) NewLine |\n|
    location: 'eastus'
//@[004:012) Identifier |location|
//@[012:013) Colon |:|
//@[014:022) StringComplete |'eastus'|
//@[022:023) NewLine |\n|
  }
//@[002:003) RightBrace |}|
//@[003:004) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

module storageDeploy2 'ts/mySpecRG:storage-spec:1.0' = {
//@[000:006) Identifier |module|
//@[007:021) Identifier |storageDeploy2|
//@[022:052) StringComplete |'ts/mySpecRG:storage-spec:1.0'|
//@[053:054) Assignment |=|
//@[055:056) LeftBrace |{|
//@[056:057) NewLine |\n|
  name: 'storageDeploy2'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:024) StringComplete |'storageDeploy2'|
//@[024:025) NewLine |\n|
  scope: rg
//@[002:007) Identifier |scope|
//@[007:008) Colon |:|
//@[009:011) Identifier |rg|
//@[011:012) NewLine |\n|
  params: {
//@[002:008) Identifier |params|
//@[008:009) Colon |:|
//@[010:011) LeftBrace |{|
//@[011:012) NewLine |\n|
    location: 'eastus'
//@[004:012) Identifier |location|
//@[012:013) Colon |:|
//@[014:022) StringComplete |'eastus'|
//@[022:023) NewLine |\n|
  }
//@[002:003) RightBrace |}|
//@[003:004) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

var vnets = [
//@[000:003) Identifier |var|
//@[004:009) Identifier |vnets|
//@[010:011) Assignment |=|
//@[012:013) LeftSquare |[|
//@[013:014) NewLine |\n|
  {
//@[002:003) LeftBrace |{|
//@[003:004) NewLine |\n|
    name: 'vnet1'
//@[004:008) Identifier |name|
//@[008:009) Colon |:|
//@[010:017) StringComplete |'vnet1'|
//@[017:018) NewLine |\n|
    subnetName: 'subnet1.1'
//@[004:014) Identifier |subnetName|
//@[014:015) Colon |:|
//@[016:027) StringComplete |'subnet1.1'|
//@[027:028) NewLine |\n|
  }
//@[002:003) RightBrace |}|
//@[003:004) NewLine |\n|
  {
//@[002:003) LeftBrace |{|
//@[003:004) NewLine |\n|
    name: 'vnet2'
//@[004:008) Identifier |name|
//@[008:009) Colon |:|
//@[010:017) StringComplete |'vnet2'|
//@[017:018) NewLine |\n|
    subnetName: 'subnet2.1'
//@[004:014) Identifier |subnetName|
//@[014:015) Colon |:|
//@[016:027) StringComplete |'subnet2.1'|
//@[027:028) NewLine |\n|
  }
//@[002:003) RightBrace |}|
//@[003:004) NewLine |\n|
]
//@[000:001) RightSquare |]|
//@[001:003) NewLine |\n\n|

module vnetDeploy 'ts:11111111-1111-1111-1111-111111111111/prod-rg/vnet-spec:v2' = [for vnet in vnets: {
//@[000:006) Identifier |module|
//@[007:017) Identifier |vnetDeploy|
//@[018:080) StringComplete |'ts:11111111-1111-1111-1111-111111111111/prod-rg/vnet-spec:v2'|
//@[081:082) Assignment |=|
//@[083:084) LeftSquare |[|
//@[084:087) Identifier |for|
//@[088:092) Identifier |vnet|
//@[093:095) Identifier |in|
//@[096:101) Identifier |vnets|
//@[101:102) Colon |:|
//@[103:104) LeftBrace |{|
//@[104:105) NewLine |\n|
  name: '${vnet.name}Deploy'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:011) StringLeftPiece |'${|
//@[011:015) Identifier |vnet|
//@[015:016) Dot |.|
//@[016:020) Identifier |name|
//@[020:028) StringRightPiece |}Deploy'|
//@[028:029) NewLine |\n|
  scope: rg
//@[002:007) Identifier |scope|
//@[007:008) Colon |:|
//@[009:011) Identifier |rg|
//@[011:012) NewLine |\n|
  params: {
//@[002:008) Identifier |params|
//@[008:009) Colon |:|
//@[010:011) LeftBrace |{|
//@[011:012) NewLine |\n|
    vnetName: vnet.name
//@[004:012) Identifier |vnetName|
//@[012:013) Colon |:|
//@[014:018) Identifier |vnet|
//@[018:019) Dot |.|
//@[019:023) Identifier |name|
//@[023:024) NewLine |\n|
    subnetName: vnet.subnetName
//@[004:014) Identifier |subnetName|
//@[014:015) Colon |:|
//@[016:020) Identifier |vnet|
//@[020:021) Dot |.|
//@[021:031) Identifier |subnetName|
//@[031:032) NewLine |\n|
  }
//@[002:003) RightBrace |}|
//@[003:004) NewLine |\n|
}]
//@[000:001) RightBrace |}|
//@[001:002) RightSquare |]|
//@[002:004) NewLine |\n\n|

output siteUrls array = [for (site, i) in websites: siteDeploy[i].outputs.siteUrl]
//@[000:006) Identifier |output|
//@[007:015) Identifier |siteUrls|
//@[016:021) Identifier |array|
//@[022:023) Assignment |=|
//@[024:025) LeftSquare |[|
//@[025:028) Identifier |for|
//@[029:030) LeftParen |(|
//@[030:034) Identifier |site|
//@[034:035) Comma |,|
//@[036:037) Identifier |i|
//@[037:038) RightParen |)|
//@[039:041) Identifier |in|
//@[042:050) Identifier |websites|
//@[050:051) Colon |:|
//@[052:062) Identifier |siteDeploy|
//@[062:063) LeftSquare |[|
//@[063:064) Identifier |i|
//@[064:065) RightSquare |]|
//@[065:066) Dot |.|
//@[066:073) Identifier |outputs|
//@[073:074) Dot |.|
//@[074:081) Identifier |siteUrl|
//@[081:082) RightSquare |]|
//@[082:084) NewLine |\n\n|

module passthroughPort 'br:localhost:5000/passthrough/port:v1' = {
//@[000:006) Identifier |module|
//@[007:022) Identifier |passthroughPort|
//@[023:062) StringComplete |'br:localhost:5000/passthrough/port:v1'|
//@[063:064) Assignment |=|
//@[065:066) LeftBrace |{|
//@[066:067) NewLine |\n|
  scope: rg
//@[002:007) Identifier |scope|
//@[007:008) Colon |:|
//@[009:011) Identifier |rg|
//@[011:012) NewLine |\n|
  name: 'port'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:014) StringComplete |'port'|
//@[014:015) NewLine |\n|
  params: {
//@[002:008) Identifier |params|
//@[008:009) Colon |:|
//@[010:011) LeftBrace |{|
//@[011:012) NewLine |\n|
    port: 'test'
//@[004:008) Identifier |port|
//@[008:009) Colon |:|
//@[010:016) StringComplete |'test'|
//@[016:017) NewLine |\n|
  }
//@[002:003) RightBrace |}|
//@[003:004) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

module ipv4 'br:127.0.0.1/passthrough/ipv4:v1' = {
//@[000:006) Identifier |module|
//@[007:011) Identifier |ipv4|
//@[012:046) StringComplete |'br:127.0.0.1/passthrough/ipv4:v1'|
//@[047:048) Assignment |=|
//@[049:050) LeftBrace |{|
//@[050:051) NewLine |\n|
  scope: rg
//@[002:007) Identifier |scope|
//@[007:008) Colon |:|
//@[009:011) Identifier |rg|
//@[011:012) NewLine |\n|
  name: 'ipv4'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:014) StringComplete |'ipv4'|
//@[014:015) NewLine |\n|
  params: {
//@[002:008) Identifier |params|
//@[008:009) Colon |:|
//@[010:011) LeftBrace |{|
//@[011:012) NewLine |\n|
    ipv4: 'test'
//@[004:008) Identifier |ipv4|
//@[008:009) Colon |:|
//@[010:016) StringComplete |'test'|
//@[016:017) NewLine |\n|
  }
//@[002:003) RightBrace |}|
//@[003:004) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

module ipv4port 'br:127.0.0.1:5000/passthrough/ipv4port:v1' = {
//@[000:006) Identifier |module|
//@[007:015) Identifier |ipv4port|
//@[016:059) StringComplete |'br:127.0.0.1:5000/passthrough/ipv4port:v1'|
//@[060:061) Assignment |=|
//@[062:063) LeftBrace |{|
//@[063:064) NewLine |\n|
  scope: rg
//@[002:007) Identifier |scope|
//@[007:008) Colon |:|
//@[009:011) Identifier |rg|
//@[011:012) NewLine |\n|
  name: 'ipv4port'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:018) StringComplete |'ipv4port'|
//@[018:019) NewLine |\n|
  params: {
//@[002:008) Identifier |params|
//@[008:009) Colon |:|
//@[010:011) LeftBrace |{|
//@[011:012) NewLine |\n|
    ipv4port: 'test'
//@[004:012) Identifier |ipv4port|
//@[012:013) Colon |:|
//@[014:020) StringComplete |'test'|
//@[020:021) NewLine |\n|
  }
//@[002:003) RightBrace |}|
//@[003:004) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

module ipv6 'br:[::1]/passthrough/ipv6:v1' = {
//@[000:006) Identifier |module|
//@[007:011) Identifier |ipv6|
//@[012:042) StringComplete |'br:[::1]/passthrough/ipv6:v1'|
//@[043:044) Assignment |=|
//@[045:046) LeftBrace |{|
//@[046:047) NewLine |\n|
  scope: rg
//@[002:007) Identifier |scope|
//@[007:008) Colon |:|
//@[009:011) Identifier |rg|
//@[011:012) NewLine |\n|
  name: 'ipv6'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:014) StringComplete |'ipv6'|
//@[014:015) NewLine |\n|
  params: {
//@[002:008) Identifier |params|
//@[008:009) Colon |:|
//@[010:011) LeftBrace |{|
//@[011:012) NewLine |\n|
    ipv6: 'test'
//@[004:008) Identifier |ipv6|
//@[008:009) Colon |:|
//@[010:016) StringComplete |'test'|
//@[016:017) NewLine |\n|
  }
//@[002:003) RightBrace |}|
//@[003:004) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

module ipv6port 'br:[::1]:5000/passthrough/ipv6port:v1' = {
//@[000:006) Identifier |module|
//@[007:015) Identifier |ipv6port|
//@[016:055) StringComplete |'br:[::1]:5000/passthrough/ipv6port:v1'|
//@[056:057) Assignment |=|
//@[058:059) LeftBrace |{|
//@[059:060) NewLine |\n|
  scope: rg
//@[002:007) Identifier |scope|
//@[007:008) Colon |:|
//@[009:011) Identifier |rg|
//@[011:012) NewLine |\n|
  name: 'ipv6port'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:018) StringComplete |'ipv6port'|
//@[018:019) NewLine |\n|
  params: {
//@[002:008) Identifier |params|
//@[008:009) Colon |:|
//@[010:011) LeftBrace |{|
//@[011:012) NewLine |\n|
    ipv6port: 'test'
//@[004:012) Identifier |ipv6port|
//@[012:013) Colon |:|
//@[014:020) StringComplete |'test'|
//@[020:021) NewLine |\n|
  }
//@[002:003) RightBrace |}|
//@[003:004) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:001) EndOfFile ||
