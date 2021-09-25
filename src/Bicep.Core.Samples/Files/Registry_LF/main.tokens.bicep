targetScope = 'subscription'
//@[0:11) Identifier |targetScope|
//@[12:13) Assignment |=|
//@[14:28) StringComplete |'subscription'|
//@[28:30) NewLine |\n\n|

resource rg 'Microsoft.Resources/resourceGroups@2020-06-01' = {
//@[0:8) Identifier |resource|
//@[9:11) Identifier |rg|
//@[12:59) StringComplete |'Microsoft.Resources/resourceGroups@2020-06-01'|
//@[60:61) Assignment |=|
//@[62:63) LeftBrace |{|
//@[63:64) NewLine |\n|
  name: 'adotfrank-rg'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:22) StringComplete |'adotfrank-rg'|
//@[22:23) NewLine |\n|
  location: deployment().location
//@[2:10) Identifier |location|
//@[10:11) Colon |:|
//@[12:22) Identifier |deployment|
//@[22:23) LeftParen |(|
//@[23:24) RightParen |)|
//@[24:25) Dot |.|
//@[25:33) Identifier |location|
//@[33:34) NewLine |\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

module appPlanDeploy 'br:mock-registry-one.invalid/demo/plan:v2' = {
//@[0:6) Identifier |module|
//@[7:20) Identifier |appPlanDeploy|
//@[21:64) StringComplete |'br:mock-registry-one.invalid/demo/plan:v2'|
//@[65:66) Assignment |=|
//@[67:68) LeftBrace |{|
//@[68:69) NewLine |\n|
  name: 'planDeploy'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:20) StringComplete |'planDeploy'|
//@[20:21) NewLine |\n|
  scope: rg
//@[2:7) Identifier |scope|
//@[7:8) Colon |:|
//@[9:11) Identifier |rg|
//@[11:12) NewLine |\n|
  params: {
//@[2:8) Identifier |params|
//@[8:9) Colon |:|
//@[10:11) LeftBrace |{|
//@[11:12) NewLine |\n|
    namePrefix: 'hello'
//@[4:14) Identifier |namePrefix|
//@[14:15) Colon |:|
//@[16:23) StringComplete |'hello'|
//@[23:24) NewLine |\n|
  }
//@[2:3) RightBrace |}|
//@[3:4) NewLine |\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

var websites = [
//@[0:3) Identifier |var|
//@[4:12) Identifier |websites|
//@[13:14) Assignment |=|
//@[15:16) LeftSquare |[|
//@[16:17) NewLine |\n|
  {
//@[2:3) LeftBrace |{|
//@[3:4) NewLine |\n|
    name: 'fancy'
//@[4:8) Identifier |name|
//@[8:9) Colon |:|
//@[10:17) StringComplete |'fancy'|
//@[17:18) NewLine |\n|
    tag: 'latest'
//@[4:7) Identifier |tag|
//@[7:8) Colon |:|
//@[9:17) StringComplete |'latest'|
//@[17:18) NewLine |\n|
  }
//@[2:3) RightBrace |}|
//@[3:4) NewLine |\n|
  {
//@[2:3) LeftBrace |{|
//@[3:4) NewLine |\n|
    name: 'plain'
//@[4:8) Identifier |name|
//@[8:9) Colon |:|
//@[10:17) StringComplete |'plain'|
//@[17:18) NewLine |\n|
    tag: 'plain-text'
//@[4:7) Identifier |tag|
//@[7:8) Colon |:|
//@[9:21) StringComplete |'plain-text'|
//@[21:22) NewLine |\n|
  }
//@[2:3) RightBrace |}|
//@[3:4) NewLine |\n|
]
//@[0:1) RightSquare |]|
//@[1:3) NewLine |\n\n|

module siteDeploy 'br:mock-registry-two.invalid/demo/site:v3' = [for site in websites: {
//@[0:6) Identifier |module|
//@[7:17) Identifier |siteDeploy|
//@[18:61) StringComplete |'br:mock-registry-two.invalid/demo/site:v3'|
//@[62:63) Assignment |=|
//@[64:65) LeftSquare |[|
//@[65:68) Identifier |for|
//@[69:73) Identifier |site|
//@[74:76) Identifier |in|
//@[77:85) Identifier |websites|
//@[85:86) Colon |:|
//@[87:88) LeftBrace |{|
//@[88:89) NewLine |\n|
  name: '${site.name}siteDeploy'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:11) StringLeftPiece |'${|
//@[11:15) Identifier |site|
//@[15:16) Dot |.|
//@[16:20) Identifier |name|
//@[20:32) StringRightPiece |}siteDeploy'|
//@[32:33) NewLine |\n|
  scope: rg
//@[2:7) Identifier |scope|
//@[7:8) Colon |:|
//@[9:11) Identifier |rg|
//@[11:12) NewLine |\n|
  params: {
//@[2:8) Identifier |params|
//@[8:9) Colon |:|
//@[10:11) LeftBrace |{|
//@[11:12) NewLine |\n|
    appPlanId: appPlanDeploy.outputs.planId
//@[4:13) Identifier |appPlanId|
//@[13:14) Colon |:|
//@[15:28) Identifier |appPlanDeploy|
//@[28:29) Dot |.|
//@[29:36) Identifier |outputs|
//@[36:37) Dot |.|
//@[37:43) Identifier |planId|
//@[43:44) NewLine |\n|
    namePrefix: site.name
//@[4:14) Identifier |namePrefix|
//@[14:15) Colon |:|
//@[16:20) Identifier |site|
//@[20:21) Dot |.|
//@[21:25) Identifier |name|
//@[25:26) NewLine |\n|
    dockerImage: 'nginxdemos/hello'
//@[4:15) Identifier |dockerImage|
//@[15:16) Colon |:|
//@[17:35) StringComplete |'nginxdemos/hello'|
//@[35:36) NewLine |\n|
    dockerImageTag: site.tag
//@[4:18) Identifier |dockerImageTag|
//@[18:19) Colon |:|
//@[20:24) Identifier |site|
//@[24:25) Dot |.|
//@[25:28) Identifier |tag|
//@[28:29) NewLine |\n|
  }
//@[2:3) RightBrace |}|
//@[3:4) NewLine |\n|
}]
//@[0:1) RightBrace |}|
//@[1:2) RightSquare |]|
//@[2:4) NewLine |\n\n|

module storageDeploy 'ts:00000000-0000-0000-0000-000000000000/test-rg/storage-spec:1.0' = {
//@[0:6) Identifier |module|
//@[7:20) Identifier |storageDeploy|
//@[21:87) StringComplete |'ts:00000000-0000-0000-0000-000000000000/test-rg/storage-spec:1.0'|
//@[88:89) Assignment |=|
//@[90:91) LeftBrace |{|
//@[91:92) NewLine |\n|
  name: 'storageDeploy'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:23) StringComplete |'storageDeploy'|
//@[23:24) NewLine |\n|
  scope: rg
//@[2:7) Identifier |scope|
//@[7:8) Colon |:|
//@[9:11) Identifier |rg|
//@[11:12) NewLine |\n|
  params: {
//@[2:8) Identifier |params|
//@[8:9) Colon |:|
//@[10:11) LeftBrace |{|
//@[11:12) NewLine |\n|
    location: 'eastus'
//@[4:12) Identifier |location|
//@[12:13) Colon |:|
//@[14:22) StringComplete |'eastus'|
//@[22:23) NewLine |\n|
  }
//@[2:3) RightBrace |}|
//@[3:4) NewLine |\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

var vnets = [
//@[0:3) Identifier |var|
//@[4:9) Identifier |vnets|
//@[10:11) Assignment |=|
//@[12:13) LeftSquare |[|
//@[13:14) NewLine |\n|
  {
//@[2:3) LeftBrace |{|
//@[3:4) NewLine |\n|
    name: 'vnet1'
//@[4:8) Identifier |name|
//@[8:9) Colon |:|
//@[10:17) StringComplete |'vnet1'|
//@[17:18) NewLine |\n|
    subnetName: 'subnet1.1'
//@[4:14) Identifier |subnetName|
//@[14:15) Colon |:|
//@[16:27) StringComplete |'subnet1.1'|
//@[27:28) NewLine |\n|
  }
//@[2:3) RightBrace |}|
//@[3:4) NewLine |\n|
  {
//@[2:3) LeftBrace |{|
//@[3:4) NewLine |\n|
    name: 'vnet2'
//@[4:8) Identifier |name|
//@[8:9) Colon |:|
//@[10:17) StringComplete |'vnet2'|
//@[17:18) NewLine |\n|
    subnetName: 'subnet2.1'
//@[4:14) Identifier |subnetName|
//@[14:15) Colon |:|
//@[16:27) StringComplete |'subnet2.1'|
//@[27:28) NewLine |\n|
  }
//@[2:3) RightBrace |}|
//@[3:4) NewLine |\n|
]
//@[0:1) RightSquare |]|
//@[1:3) NewLine |\n\n|

module vnetDeploy 'ts:management.azure.com/11111111-1111-1111-1111-111111111111/prod-rg/vnet-spec:v2' = [for vnet in vnets: {
//@[0:6) Identifier |module|
//@[7:17) Identifier |vnetDeploy|
//@[18:101) StringComplete |'ts:management.azure.com/11111111-1111-1111-1111-111111111111/prod-rg/vnet-spec:v2'|
//@[102:103) Assignment |=|
//@[104:105) LeftSquare |[|
//@[105:108) Identifier |for|
//@[109:113) Identifier |vnet|
//@[114:116) Identifier |in|
//@[117:122) Identifier |vnets|
//@[122:123) Colon |:|
//@[124:125) LeftBrace |{|
//@[125:126) NewLine |\n|
  name: '${vnet.name}Deploy'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:11) StringLeftPiece |'${|
//@[11:15) Identifier |vnet|
//@[15:16) Dot |.|
//@[16:20) Identifier |name|
//@[20:28) StringRightPiece |}Deploy'|
//@[28:29) NewLine |\n|
  scope: rg
//@[2:7) Identifier |scope|
//@[7:8) Colon |:|
//@[9:11) Identifier |rg|
//@[11:12) NewLine |\n|
  params: {
//@[2:8) Identifier |params|
//@[8:9) Colon |:|
//@[10:11) LeftBrace |{|
//@[11:12) NewLine |\n|
    vnetName: vnet.name
//@[4:12) Identifier |vnetName|
//@[12:13) Colon |:|
//@[14:18) Identifier |vnet|
//@[18:19) Dot |.|
//@[19:23) Identifier |name|
//@[23:24) NewLine |\n|
    subnetName: vnet.subnetName
//@[4:14) Identifier |subnetName|
//@[14:15) Colon |:|
//@[16:20) Identifier |vnet|
//@[20:21) Dot |.|
//@[21:31) Identifier |subnetName|
//@[31:32) NewLine |\n|
  }
//@[2:3) RightBrace |}|
//@[3:4) NewLine |\n|
}]
//@[0:1) RightBrace |}|
//@[1:2) RightSquare |]|
//@[2:4) NewLine |\n\n|

output siteUrls array = [for (site, i) in websites: siteDeploy[i].outputs.siteUrl]
//@[0:6) Identifier |output|
//@[7:15) Identifier |siteUrls|
//@[16:21) Identifier |array|
//@[22:23) Assignment |=|
//@[24:25) LeftSquare |[|
//@[25:28) Identifier |for|
//@[29:30) LeftParen |(|
//@[30:34) Identifier |site|
//@[34:35) Comma |,|
//@[36:37) Identifier |i|
//@[37:38) RightParen |)|
//@[39:41) Identifier |in|
//@[42:50) Identifier |websites|
//@[50:51) Colon |:|
//@[52:62) Identifier |siteDeploy|
//@[62:63) LeftSquare |[|
//@[63:64) Identifier |i|
//@[64:65) RightSquare |]|
//@[65:66) Dot |.|
//@[66:73) Identifier |outputs|
//@[73:74) Dot |.|
//@[74:81) Identifier |siteUrl|
//@[81:82) RightSquare |]|
//@[82:84) NewLine |\n\n|

module passthroughPort 'br:localhost:5000/passthrough/port:v1' = {
//@[0:6) Identifier |module|
//@[7:22) Identifier |passthroughPort|
//@[23:62) StringComplete |'br:localhost:5000/passthrough/port:v1'|
//@[63:64) Assignment |=|
//@[65:66) LeftBrace |{|
//@[66:67) NewLine |\n|
  scope: rg
//@[2:7) Identifier |scope|
//@[7:8) Colon |:|
//@[9:11) Identifier |rg|
//@[11:12) NewLine |\n|
  name: 'port'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:14) StringComplete |'port'|
//@[14:15) NewLine |\n|
  params: {
//@[2:8) Identifier |params|
//@[8:9) Colon |:|
//@[10:11) LeftBrace |{|
//@[11:12) NewLine |\n|
    port: 'test'
//@[4:8) Identifier |port|
//@[8:9) Colon |:|
//@[10:16) StringComplete |'test'|
//@[16:17) NewLine |\n|
  }
//@[2:3) RightBrace |}|
//@[3:4) NewLine |\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

module ipv4 'br:127.0.0.1/passthrough/ipv4:v1' = {
//@[0:6) Identifier |module|
//@[7:11) Identifier |ipv4|
//@[12:46) StringComplete |'br:127.0.0.1/passthrough/ipv4:v1'|
//@[47:48) Assignment |=|
//@[49:50) LeftBrace |{|
//@[50:51) NewLine |\n|
  scope: rg
//@[2:7) Identifier |scope|
//@[7:8) Colon |:|
//@[9:11) Identifier |rg|
//@[11:12) NewLine |\n|
  name: 'ipv4'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:14) StringComplete |'ipv4'|
//@[14:15) NewLine |\n|
  params: {
//@[2:8) Identifier |params|
//@[8:9) Colon |:|
//@[10:11) LeftBrace |{|
//@[11:12) NewLine |\n|
    ipv4: 'test'
//@[4:8) Identifier |ipv4|
//@[8:9) Colon |:|
//@[10:16) StringComplete |'test'|
//@[16:17) NewLine |\n|
  }
//@[2:3) RightBrace |}|
//@[3:4) NewLine |\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

module ipv4port 'br:127.0.0.1:5000/passthrough/ipv4port:v1' = {
//@[0:6) Identifier |module|
//@[7:15) Identifier |ipv4port|
//@[16:59) StringComplete |'br:127.0.0.1:5000/passthrough/ipv4port:v1'|
//@[60:61) Assignment |=|
//@[62:63) LeftBrace |{|
//@[63:64) NewLine |\n|
  scope: rg
//@[2:7) Identifier |scope|
//@[7:8) Colon |:|
//@[9:11) Identifier |rg|
//@[11:12) NewLine |\n|
  name: 'ipv4port'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:18) StringComplete |'ipv4port'|
//@[18:19) NewLine |\n|
  params: {
//@[2:8) Identifier |params|
//@[8:9) Colon |:|
//@[10:11) LeftBrace |{|
//@[11:12) NewLine |\n|
    ipv4port: 'test'
//@[4:12) Identifier |ipv4port|
//@[12:13) Colon |:|
//@[14:20) StringComplete |'test'|
//@[20:21) NewLine |\n|
  }
//@[2:3) RightBrace |}|
//@[3:4) NewLine |\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

module ipv6 'br:[::1]/passthrough/ipv6:v1' = {
//@[0:6) Identifier |module|
//@[7:11) Identifier |ipv6|
//@[12:42) StringComplete |'br:[::1]/passthrough/ipv6:v1'|
//@[43:44) Assignment |=|
//@[45:46) LeftBrace |{|
//@[46:47) NewLine |\n|
  scope: rg
//@[2:7) Identifier |scope|
//@[7:8) Colon |:|
//@[9:11) Identifier |rg|
//@[11:12) NewLine |\n|
  name: 'ipv6'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:14) StringComplete |'ipv6'|
//@[14:15) NewLine |\n|
  params: {
//@[2:8) Identifier |params|
//@[8:9) Colon |:|
//@[10:11) LeftBrace |{|
//@[11:12) NewLine |\n|
    ipv6: 'test'
//@[4:8) Identifier |ipv6|
//@[8:9) Colon |:|
//@[10:16) StringComplete |'test'|
//@[16:17) NewLine |\n|
  }
//@[2:3) RightBrace |}|
//@[3:4) NewLine |\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

module ipv6port 'br:[::1]:5000/passthrough/ipv6port:v1' = {
//@[0:6) Identifier |module|
//@[7:15) Identifier |ipv6port|
//@[16:55) StringComplete |'br:[::1]:5000/passthrough/ipv6port:v1'|
//@[56:57) Assignment |=|
//@[58:59) LeftBrace |{|
//@[59:60) NewLine |\n|
  scope: rg
//@[2:7) Identifier |scope|
//@[7:8) Colon |:|
//@[9:11) Identifier |rg|
//@[11:12) NewLine |\n|
  name: 'ipv6port'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:18) StringComplete |'ipv6port'|
//@[18:19) NewLine |\n|
  params: {
//@[2:8) Identifier |params|
//@[8:9) Colon |:|
//@[10:11) LeftBrace |{|
//@[11:12) NewLine |\n|
    ipv6port: 'test'
//@[4:12) Identifier |ipv6port|
//@[12:13) Colon |:|
//@[14:20) StringComplete |'test'|
//@[20:21) NewLine |\n|
  }
//@[2:3) RightBrace |}|
//@[3:4) NewLine |\n|
}
//@[0:1) RightBrace |}|
//@[1:1) EndOfFile ||
