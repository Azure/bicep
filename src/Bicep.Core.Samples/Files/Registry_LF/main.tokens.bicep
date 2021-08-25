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

module appPlanDeploy 'oci:mock-registry-one.invalid/demo/plan:v2' = {
//@[0:6) Identifier |module|
//@[7:20) Identifier |appPlanDeploy|
//@[21:65) StringComplete |'oci:mock-registry-one.invalid/demo/plan:v2'|
//@[66:67) Assignment |=|
//@[68:69) LeftBrace |{|
//@[69:70) NewLine |\n|
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

module siteDeploy 'oci:mock-registry-two.invalid/demo/site:v3' = [for site in websites: {
//@[0:6) Identifier |module|
//@[7:17) Identifier |siteDeploy|
//@[18:62) StringComplete |'oci:mock-registry-two.invalid/demo/site:v3'|
//@[63:64) Assignment |=|
//@[65:66) LeftSquare |[|
//@[66:69) Identifier |for|
//@[70:74) Identifier |site|
//@[75:77) Identifier |in|
//@[78:86) Identifier |websites|
//@[86:87) Colon |:|
//@[88:89) LeftBrace |{|
//@[89:90) NewLine |\n|
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
//@[82:83) NewLine |\n|

//@[0:0) EndOfFile ||
