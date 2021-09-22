targetScope = 'subscription'
//@[0:28) TargetScopeSyntax
//@[0:11)  Identifier |targetScope|
//@[12:13)  Assignment |=|
//@[14:28)  StringSyntax
//@[14:28)   StringComplete |'subscription'|
//@[28:30) NewLine |\n\n|

resource rg 'Microsoft.Resources/resourceGroups@2020-06-01' = {
//@[0:122) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:11)  IdentifierSyntax
//@[9:11)   Identifier |rg|
//@[12:59)  StringSyntax
//@[12:59)   StringComplete |'Microsoft.Resources/resourceGroups@2020-06-01'|
//@[60:61)  Assignment |=|
//@[62:122)  ObjectSyntax
//@[62:63)   LeftBrace |{|
//@[63:64)   NewLine |\n|
  name: 'adotfrank-rg'
//@[2:22)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:22)    StringSyntax
//@[8:22)     StringComplete |'adotfrank-rg'|
//@[22:23)   NewLine |\n|
  location: deployment().location
//@[2:33)   ObjectPropertySyntax
//@[2:10)    IdentifierSyntax
//@[2:10)     Identifier |location|
//@[10:11)    Colon |:|
//@[12:33)    PropertyAccessSyntax
//@[12:24)     FunctionCallSyntax
//@[12:22)      IdentifierSyntax
//@[12:22)       Identifier |deployment|
//@[22:23)      LeftParen |(|
//@[23:24)      RightParen |)|
//@[24:25)     Dot |.|
//@[25:33)     IdentifierSyntax
//@[25:33)      Identifier |location|
//@[33:34)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

module appPlanDeploy 'br:mock-registry-one.invalid/demo/plan:v2' = {
//@[0:143) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:20)  IdentifierSyntax
//@[7:20)   Identifier |appPlanDeploy|
//@[21:64)  StringSyntax
//@[21:64)   StringComplete |'br:mock-registry-one.invalid/demo/plan:v2'|
//@[65:66)  Assignment |=|
//@[67:143)  ObjectSyntax
//@[67:68)   LeftBrace |{|
//@[68:69)   NewLine |\n|
  name: 'planDeploy'
//@[2:20)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:20)    StringSyntax
//@[8:20)     StringComplete |'planDeploy'|
//@[20:21)   NewLine |\n|
  scope: rg
//@[2:11)   ObjectPropertySyntax
//@[2:7)    IdentifierSyntax
//@[2:7)     Identifier |scope|
//@[7:8)    Colon |:|
//@[9:11)    VariableAccessSyntax
//@[9:11)     IdentifierSyntax
//@[9:11)      Identifier |rg|
//@[11:12)   NewLine |\n|
  params: {
//@[2:39)   ObjectPropertySyntax
//@[2:8)    IdentifierSyntax
//@[2:8)     Identifier |params|
//@[8:9)    Colon |:|
//@[10:39)    ObjectSyntax
//@[10:11)     LeftBrace |{|
//@[11:12)     NewLine |\n|
    namePrefix: 'hello'
//@[4:23)     ObjectPropertySyntax
//@[4:14)      IdentifierSyntax
//@[4:14)       Identifier |namePrefix|
//@[14:15)      Colon |:|
//@[16:23)      StringSyntax
//@[16:23)       StringComplete |'hello'|
//@[23:24)     NewLine |\n|
  }
//@[2:3)     RightBrace |}|
//@[3:4)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

var websites = [
//@[0:110) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:12)  IdentifierSyntax
//@[4:12)   Identifier |websites|
//@[13:14)  Assignment |=|
//@[15:110)  ArraySyntax
//@[15:16)   LeftSquare |[|
//@[16:17)   NewLine |\n|
  {
//@[2:43)   ArrayItemSyntax
//@[2:43)    ObjectSyntax
//@[2:3)     LeftBrace |{|
//@[3:4)     NewLine |\n|
    name: 'fancy'
//@[4:17)     ObjectPropertySyntax
//@[4:8)      IdentifierSyntax
//@[4:8)       Identifier |name|
//@[8:9)      Colon |:|
//@[10:17)      StringSyntax
//@[10:17)       StringComplete |'fancy'|
//@[17:18)     NewLine |\n|
    tag: 'latest'
//@[4:17)     ObjectPropertySyntax
//@[4:7)      IdentifierSyntax
//@[4:7)       Identifier |tag|
//@[7:8)      Colon |:|
//@[9:17)      StringSyntax
//@[9:17)       StringComplete |'latest'|
//@[17:18)     NewLine |\n|
  }
//@[2:3)     RightBrace |}|
//@[3:4)   NewLine |\n|
  {
//@[2:47)   ArrayItemSyntax
//@[2:47)    ObjectSyntax
//@[2:3)     LeftBrace |{|
//@[3:4)     NewLine |\n|
    name: 'plain'
//@[4:17)     ObjectPropertySyntax
//@[4:8)      IdentifierSyntax
//@[4:8)       Identifier |name|
//@[8:9)      Colon |:|
//@[10:17)      StringSyntax
//@[10:17)       StringComplete |'plain'|
//@[17:18)     NewLine |\n|
    tag: 'plain-text'
//@[4:21)     ObjectPropertySyntax
//@[4:7)      IdentifierSyntax
//@[4:7)       Identifier |tag|
//@[7:8)      Colon |:|
//@[9:21)      StringSyntax
//@[9:21)       StringComplete |'plain-text'|
//@[21:22)     NewLine |\n|
  }
//@[2:3)     RightBrace |}|
//@[3:4)   NewLine |\n|
]
//@[0:1)   RightSquare |]|
//@[1:3) NewLine |\n\n|

module siteDeploy 'br:mock-registry-two.invalid/demo/site:v3' = [for site in websites: {
//@[0:287) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:17)  IdentifierSyntax
//@[7:17)   Identifier |siteDeploy|
//@[18:61)  StringSyntax
//@[18:61)   StringComplete |'br:mock-registry-two.invalid/demo/site:v3'|
//@[62:63)  Assignment |=|
//@[64:287)  ForSyntax
//@[64:65)   LeftSquare |[|
//@[65:68)   Identifier |for|
//@[69:73)   LocalVariableSyntax
//@[69:73)    IdentifierSyntax
//@[69:73)     Identifier |site|
//@[74:76)   Identifier |in|
//@[77:85)   VariableAccessSyntax
//@[77:85)    IdentifierSyntax
//@[77:85)     Identifier |websites|
//@[85:86)   Colon |:|
//@[87:286)   ObjectSyntax
//@[87:88)    LeftBrace |{|
//@[88:89)    NewLine |\n|
  name: '${site.name}siteDeploy'
//@[2:32)    ObjectPropertySyntax
//@[2:6)     IdentifierSyntax
//@[2:6)      Identifier |name|
//@[6:7)     Colon |:|
//@[8:32)     StringSyntax
//@[8:11)      StringLeftPiece |'${|
//@[11:20)      PropertyAccessSyntax
//@[11:15)       VariableAccessSyntax
//@[11:15)        IdentifierSyntax
//@[11:15)         Identifier |site|
//@[15:16)       Dot |.|
//@[16:20)       IdentifierSyntax
//@[16:20)        Identifier |name|
//@[20:32)      StringRightPiece |}siteDeploy'|
//@[32:33)    NewLine |\n|
  scope: rg
//@[2:11)    ObjectPropertySyntax
//@[2:7)     IdentifierSyntax
//@[2:7)      Identifier |scope|
//@[7:8)     Colon |:|
//@[9:11)     VariableAccessSyntax
//@[9:11)      IdentifierSyntax
//@[9:11)       Identifier |rg|
//@[11:12)    NewLine |\n|
  params: {
//@[2:150)    ObjectPropertySyntax
//@[2:8)     IdentifierSyntax
//@[2:8)      Identifier |params|
//@[8:9)     Colon |:|
//@[10:150)     ObjectSyntax
//@[10:11)      LeftBrace |{|
//@[11:12)      NewLine |\n|
    appPlanId: appPlanDeploy.outputs.planId
//@[4:43)      ObjectPropertySyntax
//@[4:13)       IdentifierSyntax
//@[4:13)        Identifier |appPlanId|
//@[13:14)       Colon |:|
//@[15:43)       PropertyAccessSyntax
//@[15:36)        PropertyAccessSyntax
//@[15:28)         VariableAccessSyntax
//@[15:28)          IdentifierSyntax
//@[15:28)           Identifier |appPlanDeploy|
//@[28:29)         Dot |.|
//@[29:36)         IdentifierSyntax
//@[29:36)          Identifier |outputs|
//@[36:37)        Dot |.|
//@[37:43)        IdentifierSyntax
//@[37:43)         Identifier |planId|
//@[43:44)      NewLine |\n|
    namePrefix: site.name
//@[4:25)      ObjectPropertySyntax
//@[4:14)       IdentifierSyntax
//@[4:14)        Identifier |namePrefix|
//@[14:15)       Colon |:|
//@[16:25)       PropertyAccessSyntax
//@[16:20)        VariableAccessSyntax
//@[16:20)         IdentifierSyntax
//@[16:20)          Identifier |site|
//@[20:21)        Dot |.|
//@[21:25)        IdentifierSyntax
//@[21:25)         Identifier |name|
//@[25:26)      NewLine |\n|
    dockerImage: 'nginxdemos/hello'
//@[4:35)      ObjectPropertySyntax
//@[4:15)       IdentifierSyntax
//@[4:15)        Identifier |dockerImage|
//@[15:16)       Colon |:|
//@[17:35)       StringSyntax
//@[17:35)        StringComplete |'nginxdemos/hello'|
//@[35:36)      NewLine |\n|
    dockerImageTag: site.tag
//@[4:28)      ObjectPropertySyntax
//@[4:18)       IdentifierSyntax
//@[4:18)        Identifier |dockerImageTag|
//@[18:19)       Colon |:|
//@[20:28)       PropertyAccessSyntax
//@[20:24)        VariableAccessSyntax
//@[20:24)         IdentifierSyntax
//@[20:24)          Identifier |site|
//@[24:25)        Dot |.|
//@[25:28)        IdentifierSyntax
//@[25:28)         Identifier |tag|
//@[28:29)      NewLine |\n|
  }
//@[2:3)      RightBrace |}|
//@[3:4)    NewLine |\n|
}]
//@[0:1)    RightBrace |}|
//@[1:2)   RightSquare |]|
//@[2:4) NewLine |\n\n|

module storageDeploy 'ts:00000000-0000-0000-0000-000000000000/test-rg/storage-spec:1.0' = {
//@[0:168) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:20)  IdentifierSyntax
//@[7:20)   Identifier |storageDeploy|
//@[21:87)  StringSyntax
//@[21:87)   StringComplete |'ts:00000000-0000-0000-0000-000000000000/test-rg/storage-spec:1.0'|
//@[88:89)  Assignment |=|
//@[90:168)  ObjectSyntax
//@[90:91)   LeftBrace |{|
//@[91:92)   NewLine |\n|
  name: 'storageDeploy'
//@[2:23)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:23)    StringSyntax
//@[8:23)     StringComplete |'storageDeploy'|
//@[23:24)   NewLine |\n|
  scope: rg
//@[2:11)   ObjectPropertySyntax
//@[2:7)    IdentifierSyntax
//@[2:7)     Identifier |scope|
//@[7:8)    Colon |:|
//@[9:11)    VariableAccessSyntax
//@[9:11)     IdentifierSyntax
//@[9:11)      Identifier |rg|
//@[11:12)   NewLine |\n|
  params: {
//@[2:38)   ObjectPropertySyntax
//@[2:8)    IdentifierSyntax
//@[2:8)     Identifier |params|
//@[8:9)    Colon |:|
//@[10:38)    ObjectSyntax
//@[10:11)     LeftBrace |{|
//@[11:12)     NewLine |\n|
    location: 'eastus'
//@[4:22)     ObjectPropertySyntax
//@[4:12)      IdentifierSyntax
//@[4:12)       Identifier |location|
//@[12:13)      Colon |:|
//@[14:22)      StringSyntax
//@[14:22)       StringComplete |'eastus'|
//@[22:23)     NewLine |\n|
  }
//@[2:3)     RightBrace |}|
//@[3:4)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

var vnets = [
//@[0:123) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:9)  IdentifierSyntax
//@[4:9)   Identifier |vnets|
//@[10:11)  Assignment |=|
//@[12:123)  ArraySyntax
//@[12:13)   LeftSquare |[|
//@[13:14)   NewLine |\n|
  {
//@[2:53)   ArrayItemSyntax
//@[2:53)    ObjectSyntax
//@[2:3)     LeftBrace |{|
//@[3:4)     NewLine |\n|
    name: 'vnet1'
//@[4:17)     ObjectPropertySyntax
//@[4:8)      IdentifierSyntax
//@[4:8)       Identifier |name|
//@[8:9)      Colon |:|
//@[10:17)      StringSyntax
//@[10:17)       StringComplete |'vnet1'|
//@[17:18)     NewLine |\n|
    subnetName: 'subnet1.1'
//@[4:27)     ObjectPropertySyntax
//@[4:14)      IdentifierSyntax
//@[4:14)       Identifier |subnetName|
//@[14:15)      Colon |:|
//@[16:27)      StringSyntax
//@[16:27)       StringComplete |'subnet1.1'|
//@[27:28)     NewLine |\n|
  }
//@[2:3)     RightBrace |}|
//@[3:4)   NewLine |\n|
  {
//@[2:53)   ArrayItemSyntax
//@[2:53)    ObjectSyntax
//@[2:3)     LeftBrace |{|
//@[3:4)     NewLine |\n|
    name: 'vnet2'
//@[4:17)     ObjectPropertySyntax
//@[4:8)      IdentifierSyntax
//@[4:8)       Identifier |name|
//@[8:9)      Colon |:|
//@[10:17)      StringSyntax
//@[10:17)       StringComplete |'vnet2'|
//@[17:18)     NewLine |\n|
    subnetName: 'subnet2.1'
//@[4:27)     ObjectPropertySyntax
//@[4:14)      IdentifierSyntax
//@[4:14)       Identifier |subnetName|
//@[14:15)      Colon |:|
//@[16:27)      StringSyntax
//@[16:27)       StringComplete |'subnet2.1'|
//@[27:28)     NewLine |\n|
  }
//@[2:3)     RightBrace |}|
//@[3:4)   NewLine |\n|
]
//@[0:1)   RightSquare |]|
//@[1:3) NewLine |\n\n|

module vnetDeploy 'ts:management.azure.com/11111111-1111-1111-1111-111111111111/prod-rg/vnet-spec:v2' = [for vnet in vnets: {
//@[0:241) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:17)  IdentifierSyntax
//@[7:17)   Identifier |vnetDeploy|
//@[18:101)  StringSyntax
//@[18:101)   StringComplete |'ts:management.azure.com/11111111-1111-1111-1111-111111111111/prod-rg/vnet-spec:v2'|
//@[102:103)  Assignment |=|
//@[104:241)  ForSyntax
//@[104:105)   LeftSquare |[|
//@[105:108)   Identifier |for|
//@[109:113)   LocalVariableSyntax
//@[109:113)    IdentifierSyntax
//@[109:113)     Identifier |vnet|
//@[114:116)   Identifier |in|
//@[117:122)   VariableAccessSyntax
//@[117:122)    IdentifierSyntax
//@[117:122)     Identifier |vnets|
//@[122:123)   Colon |:|
//@[124:240)   ObjectSyntax
//@[124:125)    LeftBrace |{|
//@[125:126)    NewLine |\n|
  name: '${vnet.name}Deploy'
//@[2:28)    ObjectPropertySyntax
//@[2:6)     IdentifierSyntax
//@[2:6)      Identifier |name|
//@[6:7)     Colon |:|
//@[8:28)     StringSyntax
//@[8:11)      StringLeftPiece |'${|
//@[11:20)      PropertyAccessSyntax
//@[11:15)       VariableAccessSyntax
//@[11:15)        IdentifierSyntax
//@[11:15)         Identifier |vnet|
//@[15:16)       Dot |.|
//@[16:20)       IdentifierSyntax
//@[16:20)        Identifier |name|
//@[20:28)      StringRightPiece |}Deploy'|
//@[28:29)    NewLine |\n|
  scope: rg
//@[2:11)    ObjectPropertySyntax
//@[2:7)     IdentifierSyntax
//@[2:7)      Identifier |scope|
//@[7:8)     Colon |:|
//@[9:11)     VariableAccessSyntax
//@[9:11)      IdentifierSyntax
//@[9:11)       Identifier |rg|
//@[11:12)    NewLine |\n|
  params: {
//@[2:71)    ObjectPropertySyntax
//@[2:8)     IdentifierSyntax
//@[2:8)      Identifier |params|
//@[8:9)     Colon |:|
//@[10:71)     ObjectSyntax
//@[10:11)      LeftBrace |{|
//@[11:12)      NewLine |\n|
    vnetName: vnet.name
//@[4:23)      ObjectPropertySyntax
//@[4:12)       IdentifierSyntax
//@[4:12)        Identifier |vnetName|
//@[12:13)       Colon |:|
//@[14:23)       PropertyAccessSyntax
//@[14:18)        VariableAccessSyntax
//@[14:18)         IdentifierSyntax
//@[14:18)          Identifier |vnet|
//@[18:19)        Dot |.|
//@[19:23)        IdentifierSyntax
//@[19:23)         Identifier |name|
//@[23:24)      NewLine |\n|
    subnetName: vnet.subnetName
//@[4:31)      ObjectPropertySyntax
//@[4:14)       IdentifierSyntax
//@[4:14)        Identifier |subnetName|
//@[14:15)       Colon |:|
//@[16:31)       PropertyAccessSyntax
//@[16:20)        VariableAccessSyntax
//@[16:20)         IdentifierSyntax
//@[16:20)          Identifier |vnet|
//@[20:21)        Dot |.|
//@[21:31)        IdentifierSyntax
//@[21:31)         Identifier |subnetName|
//@[31:32)      NewLine |\n|
  }
//@[2:3)      RightBrace |}|
//@[3:4)    NewLine |\n|
}]
//@[0:1)    RightBrace |}|
//@[1:2)   RightSquare |]|
//@[2:4) NewLine |\n\n|

output siteUrls array = [for (site, i) in websites: siteDeploy[i].outputs.siteUrl]
//@[0:82) OutputDeclarationSyntax
//@[0:6)  Identifier |output|
//@[7:15)  IdentifierSyntax
//@[7:15)   Identifier |siteUrls|
//@[16:21)  TypeSyntax
//@[16:21)   Identifier |array|
//@[22:23)  Assignment |=|
//@[24:82)  ForSyntax
//@[24:25)   LeftSquare |[|
//@[25:28)   Identifier |for|
//@[29:38)   ForVariableBlockSyntax
//@[29:30)    LeftParen |(|
//@[30:34)    LocalVariableSyntax
//@[30:34)     IdentifierSyntax
//@[30:34)      Identifier |site|
//@[34:35)    Comma |,|
//@[36:37)    LocalVariableSyntax
//@[36:37)     IdentifierSyntax
//@[36:37)      Identifier |i|
//@[37:38)    RightParen |)|
//@[39:41)   Identifier |in|
//@[42:50)   VariableAccessSyntax
//@[42:50)    IdentifierSyntax
//@[42:50)     Identifier |websites|
//@[50:51)   Colon |:|
//@[52:81)   PropertyAccessSyntax
//@[52:73)    PropertyAccessSyntax
//@[52:65)     ArrayAccessSyntax
//@[52:62)      VariableAccessSyntax
//@[52:62)       IdentifierSyntax
//@[52:62)        Identifier |siteDeploy|
//@[62:63)      LeftSquare |[|
//@[63:64)      VariableAccessSyntax
//@[63:64)       IdentifierSyntax
//@[63:64)        Identifier |i|
//@[64:65)      RightSquare |]|
//@[65:66)     Dot |.|
//@[66:73)     IdentifierSyntax
//@[66:73)      Identifier |outputs|
//@[73:74)    Dot |.|
//@[74:81)    IdentifierSyntax
//@[74:81)     Identifier |siteUrl|
//@[81:82)   RightSquare |]|
//@[82:84) NewLine |\n\n|

module passthroughPort 'br:localhost:5000/passthrough/port:v1' = {
//@[0:128) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:22)  IdentifierSyntax
//@[7:22)   Identifier |passthroughPort|
//@[23:62)  StringSyntax
//@[23:62)   StringComplete |'br:localhost:5000/passthrough/port:v1'|
//@[63:64)  Assignment |=|
//@[65:128)  ObjectSyntax
//@[65:66)   LeftBrace |{|
//@[66:67)   NewLine |\n|
  scope: rg
//@[2:11)   ObjectPropertySyntax
//@[2:7)    IdentifierSyntax
//@[2:7)     Identifier |scope|
//@[7:8)    Colon |:|
//@[9:11)    VariableAccessSyntax
//@[9:11)     IdentifierSyntax
//@[9:11)      Identifier |rg|
//@[11:12)   NewLine |\n|
  name: 'port'
//@[2:14)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:14)    StringSyntax
//@[8:14)     StringComplete |'port'|
//@[14:15)   NewLine |\n|
  params: {
//@[2:32)   ObjectPropertySyntax
//@[2:8)    IdentifierSyntax
//@[2:8)     Identifier |params|
//@[8:9)    Colon |:|
//@[10:32)    ObjectSyntax
//@[10:11)     LeftBrace |{|
//@[11:12)     NewLine |\n|
    port: 'test'
//@[4:16)     ObjectPropertySyntax
//@[4:8)      IdentifierSyntax
//@[4:8)       Identifier |port|
//@[8:9)      Colon |:|
//@[10:16)      StringSyntax
//@[10:16)       StringComplete |'test'|
//@[16:17)     NewLine |\n|
  }
//@[2:3)     RightBrace |}|
//@[3:4)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

module ipv4 'br:127.0.0.1/passthrough/ipv4:v1' = {
//@[0:112) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:11)  IdentifierSyntax
//@[7:11)   Identifier |ipv4|
//@[12:46)  StringSyntax
//@[12:46)   StringComplete |'br:127.0.0.1/passthrough/ipv4:v1'|
//@[47:48)  Assignment |=|
//@[49:112)  ObjectSyntax
//@[49:50)   LeftBrace |{|
//@[50:51)   NewLine |\n|
  scope: rg
//@[2:11)   ObjectPropertySyntax
//@[2:7)    IdentifierSyntax
//@[2:7)     Identifier |scope|
//@[7:8)    Colon |:|
//@[9:11)    VariableAccessSyntax
//@[9:11)     IdentifierSyntax
//@[9:11)      Identifier |rg|
//@[11:12)   NewLine |\n|
  name: 'ipv4'
//@[2:14)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:14)    StringSyntax
//@[8:14)     StringComplete |'ipv4'|
//@[14:15)   NewLine |\n|
  params: {
//@[2:32)   ObjectPropertySyntax
//@[2:8)    IdentifierSyntax
//@[2:8)     Identifier |params|
//@[8:9)    Colon |:|
//@[10:32)    ObjectSyntax
//@[10:11)     LeftBrace |{|
//@[11:12)     NewLine |\n|
    ipv4: 'test'
//@[4:16)     ObjectPropertySyntax
//@[4:8)      IdentifierSyntax
//@[4:8)       Identifier |ipv4|
//@[8:9)      Colon |:|
//@[10:16)      StringSyntax
//@[10:16)       StringComplete |'test'|
//@[16:17)     NewLine |\n|
  }
//@[2:3)     RightBrace |}|
//@[3:4)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

module ipv4port 'br:127.0.0.1:5000/passthrough/ipv4port:v1' = {
//@[0:133) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:15)  IdentifierSyntax
//@[7:15)   Identifier |ipv4port|
//@[16:59)  StringSyntax
//@[16:59)   StringComplete |'br:127.0.0.1:5000/passthrough/ipv4port:v1'|
//@[60:61)  Assignment |=|
//@[62:133)  ObjectSyntax
//@[62:63)   LeftBrace |{|
//@[63:64)   NewLine |\n|
  scope: rg
//@[2:11)   ObjectPropertySyntax
//@[2:7)    IdentifierSyntax
//@[2:7)     Identifier |scope|
//@[7:8)    Colon |:|
//@[9:11)    VariableAccessSyntax
//@[9:11)     IdentifierSyntax
//@[9:11)      Identifier |rg|
//@[11:12)   NewLine |\n|
  name: 'ipv4port'
//@[2:18)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:18)    StringSyntax
//@[8:18)     StringComplete |'ipv4port'|
//@[18:19)   NewLine |\n|
  params: {
//@[2:36)   ObjectPropertySyntax
//@[2:8)    IdentifierSyntax
//@[2:8)     Identifier |params|
//@[8:9)    Colon |:|
//@[10:36)    ObjectSyntax
//@[10:11)     LeftBrace |{|
//@[11:12)     NewLine |\n|
    ipv4port: 'test'
//@[4:20)     ObjectPropertySyntax
//@[4:12)      IdentifierSyntax
//@[4:12)       Identifier |ipv4port|
//@[12:13)      Colon |:|
//@[14:20)      StringSyntax
//@[14:20)       StringComplete |'test'|
//@[20:21)     NewLine |\n|
  }
//@[2:3)     RightBrace |}|
//@[3:4)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

module ipv6 'br:[::1]/passthrough/ipv6:v1' = {
//@[0:108) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:11)  IdentifierSyntax
//@[7:11)   Identifier |ipv6|
//@[12:42)  StringSyntax
//@[12:42)   StringComplete |'br:[::1]/passthrough/ipv6:v1'|
//@[43:44)  Assignment |=|
//@[45:108)  ObjectSyntax
//@[45:46)   LeftBrace |{|
//@[46:47)   NewLine |\n|
  scope: rg
//@[2:11)   ObjectPropertySyntax
//@[2:7)    IdentifierSyntax
//@[2:7)     Identifier |scope|
//@[7:8)    Colon |:|
//@[9:11)    VariableAccessSyntax
//@[9:11)     IdentifierSyntax
//@[9:11)      Identifier |rg|
//@[11:12)   NewLine |\n|
  name: 'ipv6'
//@[2:14)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:14)    StringSyntax
//@[8:14)     StringComplete |'ipv6'|
//@[14:15)   NewLine |\n|
  params: {
//@[2:32)   ObjectPropertySyntax
//@[2:8)    IdentifierSyntax
//@[2:8)     Identifier |params|
//@[8:9)    Colon |:|
//@[10:32)    ObjectSyntax
//@[10:11)     LeftBrace |{|
//@[11:12)     NewLine |\n|
    ipv6: 'test'
//@[4:16)     ObjectPropertySyntax
//@[4:8)      IdentifierSyntax
//@[4:8)       Identifier |ipv6|
//@[8:9)      Colon |:|
//@[10:16)      StringSyntax
//@[10:16)       StringComplete |'test'|
//@[16:17)     NewLine |\n|
  }
//@[2:3)     RightBrace |}|
//@[3:4)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

module ipv6port 'br:[::1]:5000/passthrough/ipv6port:v1' = {
//@[0:129) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:15)  IdentifierSyntax
//@[7:15)   Identifier |ipv6port|
//@[16:55)  StringSyntax
//@[16:55)   StringComplete |'br:[::1]:5000/passthrough/ipv6port:v1'|
//@[56:57)  Assignment |=|
//@[58:129)  ObjectSyntax
//@[58:59)   LeftBrace |{|
//@[59:60)   NewLine |\n|
  scope: rg
//@[2:11)   ObjectPropertySyntax
//@[2:7)    IdentifierSyntax
//@[2:7)     Identifier |scope|
//@[7:8)    Colon |:|
//@[9:11)    VariableAccessSyntax
//@[9:11)     IdentifierSyntax
//@[9:11)      Identifier |rg|
//@[11:12)   NewLine |\n|
  name: 'ipv6port'
//@[2:18)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:18)    StringSyntax
//@[8:18)     StringComplete |'ipv6port'|
//@[18:19)   NewLine |\n|
  params: {
//@[2:36)   ObjectPropertySyntax
//@[2:8)    IdentifierSyntax
//@[2:8)     Identifier |params|
//@[8:9)    Colon |:|
//@[10:36)    ObjectSyntax
//@[10:11)     LeftBrace |{|
//@[11:12)     NewLine |\n|
    ipv6port: 'test'
//@[4:20)     ObjectPropertySyntax
//@[4:12)      IdentifierSyntax
//@[4:12)       Identifier |ipv6port|
//@[12:13)      Colon |:|
//@[14:20)      StringSyntax
//@[14:20)       StringComplete |'test'|
//@[20:21)     NewLine |\n|
  }
//@[2:3)     RightBrace |}|
//@[3:4)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:1) EndOfFile ||
