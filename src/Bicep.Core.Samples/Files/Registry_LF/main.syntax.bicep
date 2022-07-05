targetScope = 'subscription'
//@[000:2463) ProgramSyntax
//@[000:0028) ├─TargetScopeSyntax
//@[000:0011) | ├─Token(Identifier) |targetScope|
//@[012:0013) | ├─Token(Assignment) |=|
//@[014:0028) | └─StringSyntax
//@[014:0028) |   └─Token(StringComplete) |'subscription'|
//@[028:0030) ├─Token(NewLine) |\n\n|

resource rg 'Microsoft.Resources/resourceGroups@2020-06-01' = {
//@[000:0122) ├─ResourceDeclarationSyntax
//@[000:0008) | ├─Token(Identifier) |resource|
//@[009:0011) | ├─IdentifierSyntax
//@[009:0011) | | └─Token(Identifier) |rg|
//@[012:0059) | ├─StringSyntax
//@[012:0059) | | └─Token(StringComplete) |'Microsoft.Resources/resourceGroups@2020-06-01'|
//@[060:0061) | ├─Token(Assignment) |=|
//@[062:0122) | └─ObjectSyntax
//@[062:0063) |   ├─Token(LeftBrace) |{|
//@[063:0064) |   ├─Token(NewLine) |\n|
  name: 'adotfrank-rg'
//@[002:0022) |   ├─ObjectPropertySyntax
//@[002:0006) |   | ├─IdentifierSyntax
//@[002:0006) |   | | └─Token(Identifier) |name|
//@[006:0007) |   | ├─Token(Colon) |:|
//@[008:0022) |   | └─StringSyntax
//@[008:0022) |   |   └─Token(StringComplete) |'adotfrank-rg'|
//@[022:0023) |   ├─Token(NewLine) |\n|
  location: deployment().location
//@[002:0033) |   ├─ObjectPropertySyntax
//@[002:0010) |   | ├─IdentifierSyntax
//@[002:0010) |   | | └─Token(Identifier) |location|
//@[010:0011) |   | ├─Token(Colon) |:|
//@[012:0033) |   | └─PropertyAccessSyntax
//@[012:0024) |   |   ├─FunctionCallSyntax
//@[012:0022) |   |   | ├─IdentifierSyntax
//@[012:0022) |   |   | | └─Token(Identifier) |deployment|
//@[022:0023) |   |   | ├─Token(LeftParen) |(|
//@[023:0024) |   |   | └─Token(RightParen) |)|
//@[024:0025) |   |   ├─Token(Dot) |.|
//@[025:0033) |   |   └─IdentifierSyntax
//@[025:0033) |   |     └─Token(Identifier) |location|
//@[033:0034) |   ├─Token(NewLine) |\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0003) ├─Token(NewLine) |\n\n|

module appPlanDeploy 'br:mock-registry-one.invalid/demo/plan:v2' = {
//@[000:0143) ├─ModuleDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |module|
//@[007:0020) | ├─IdentifierSyntax
//@[007:0020) | | └─Token(Identifier) |appPlanDeploy|
//@[021:0064) | ├─StringSyntax
//@[021:0064) | | └─Token(StringComplete) |'br:mock-registry-one.invalid/demo/plan:v2'|
//@[065:0066) | ├─Token(Assignment) |=|
//@[067:0143) | └─ObjectSyntax
//@[067:0068) |   ├─Token(LeftBrace) |{|
//@[068:0069) |   ├─Token(NewLine) |\n|
  name: 'planDeploy'
//@[002:0020) |   ├─ObjectPropertySyntax
//@[002:0006) |   | ├─IdentifierSyntax
//@[002:0006) |   | | └─Token(Identifier) |name|
//@[006:0007) |   | ├─Token(Colon) |:|
//@[008:0020) |   | └─StringSyntax
//@[008:0020) |   |   └─Token(StringComplete) |'planDeploy'|
//@[020:0021) |   ├─Token(NewLine) |\n|
  scope: rg
//@[002:0011) |   ├─ObjectPropertySyntax
//@[002:0007) |   | ├─IdentifierSyntax
//@[002:0007) |   | | └─Token(Identifier) |scope|
//@[007:0008) |   | ├─Token(Colon) |:|
//@[009:0011) |   | └─VariableAccessSyntax
//@[009:0011) |   |   └─IdentifierSyntax
//@[009:0011) |   |     └─Token(Identifier) |rg|
//@[011:0012) |   ├─Token(NewLine) |\n|
  params: {
//@[002:0039) |   ├─ObjectPropertySyntax
//@[002:0008) |   | ├─IdentifierSyntax
//@[002:0008) |   | | └─Token(Identifier) |params|
//@[008:0009) |   | ├─Token(Colon) |:|
//@[010:0039) |   | └─ObjectSyntax
//@[010:0011) |   |   ├─Token(LeftBrace) |{|
//@[011:0012) |   |   ├─Token(NewLine) |\n|
    namePrefix: 'hello'
//@[004:0023) |   |   ├─ObjectPropertySyntax
//@[004:0014) |   |   | ├─IdentifierSyntax
//@[004:0014) |   |   | | └─Token(Identifier) |namePrefix|
//@[014:0015) |   |   | ├─Token(Colon) |:|
//@[016:0023) |   |   | └─StringSyntax
//@[016:0023) |   |   |   └─Token(StringComplete) |'hello'|
//@[023:0024) |   |   ├─Token(NewLine) |\n|
  }
//@[002:0003) |   |   └─Token(RightBrace) |}|
//@[003:0004) |   ├─Token(NewLine) |\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0003) ├─Token(NewLine) |\n\n|

module appPlanDeploy2 'br/mock-registry-one:demo/plan:v2' = {
//@[000:0137) ├─ModuleDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |module|
//@[007:0021) | ├─IdentifierSyntax
//@[007:0021) | | └─Token(Identifier) |appPlanDeploy2|
//@[022:0057) | ├─StringSyntax
//@[022:0057) | | └─Token(StringComplete) |'br/mock-registry-one:demo/plan:v2'|
//@[058:0059) | ├─Token(Assignment) |=|
//@[060:0137) | └─ObjectSyntax
//@[060:0061) |   ├─Token(LeftBrace) |{|
//@[061:0062) |   ├─Token(NewLine) |\n|
  name: 'planDeploy2'
//@[002:0021) |   ├─ObjectPropertySyntax
//@[002:0006) |   | ├─IdentifierSyntax
//@[002:0006) |   | | └─Token(Identifier) |name|
//@[006:0007) |   | ├─Token(Colon) |:|
//@[008:0021) |   | └─StringSyntax
//@[008:0021) |   |   └─Token(StringComplete) |'planDeploy2'|
//@[021:0022) |   ├─Token(NewLine) |\n|
  scope: rg
//@[002:0011) |   ├─ObjectPropertySyntax
//@[002:0007) |   | ├─IdentifierSyntax
//@[002:0007) |   | | └─Token(Identifier) |scope|
//@[007:0008) |   | ├─Token(Colon) |:|
//@[009:0011) |   | └─VariableAccessSyntax
//@[009:0011) |   |   └─IdentifierSyntax
//@[009:0011) |   |     └─Token(Identifier) |rg|
//@[011:0012) |   ├─Token(NewLine) |\n|
  params: {
//@[002:0039) |   ├─ObjectPropertySyntax
//@[002:0008) |   | ├─IdentifierSyntax
//@[002:0008) |   | | └─Token(Identifier) |params|
//@[008:0009) |   | ├─Token(Colon) |:|
//@[010:0039) |   | └─ObjectSyntax
//@[010:0011) |   |   ├─Token(LeftBrace) |{|
//@[011:0012) |   |   ├─Token(NewLine) |\n|
    namePrefix: 'hello'
//@[004:0023) |   |   ├─ObjectPropertySyntax
//@[004:0014) |   |   | ├─IdentifierSyntax
//@[004:0014) |   |   | | └─Token(Identifier) |namePrefix|
//@[014:0015) |   |   | ├─Token(Colon) |:|
//@[016:0023) |   |   | └─StringSyntax
//@[016:0023) |   |   |   └─Token(StringComplete) |'hello'|
//@[023:0024) |   |   ├─Token(NewLine) |\n|
  }
//@[002:0003) |   |   └─Token(RightBrace) |}|
//@[003:0004) |   ├─Token(NewLine) |\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0003) ├─Token(NewLine) |\n\n|

var websites = [
//@[000:0110) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0012) | ├─IdentifierSyntax
//@[004:0012) | | └─Token(Identifier) |websites|
//@[013:0014) | ├─Token(Assignment) |=|
//@[015:0110) | └─ArraySyntax
//@[015:0016) |   ├─Token(LeftSquare) |[|
//@[016:0017) |   ├─Token(NewLine) |\n|
  {
//@[002:0043) |   ├─ArrayItemSyntax
//@[002:0043) |   | └─ObjectSyntax
//@[002:0003) |   |   ├─Token(LeftBrace) |{|
//@[003:0004) |   |   ├─Token(NewLine) |\n|
    name: 'fancy'
//@[004:0017) |   |   ├─ObjectPropertySyntax
//@[004:0008) |   |   | ├─IdentifierSyntax
//@[004:0008) |   |   | | └─Token(Identifier) |name|
//@[008:0009) |   |   | ├─Token(Colon) |:|
//@[010:0017) |   |   | └─StringSyntax
//@[010:0017) |   |   |   └─Token(StringComplete) |'fancy'|
//@[017:0018) |   |   ├─Token(NewLine) |\n|
    tag: 'latest'
//@[004:0017) |   |   ├─ObjectPropertySyntax
//@[004:0007) |   |   | ├─IdentifierSyntax
//@[004:0007) |   |   | | └─Token(Identifier) |tag|
//@[007:0008) |   |   | ├─Token(Colon) |:|
//@[009:0017) |   |   | └─StringSyntax
//@[009:0017) |   |   |   └─Token(StringComplete) |'latest'|
//@[017:0018) |   |   ├─Token(NewLine) |\n|
  }
//@[002:0003) |   |   └─Token(RightBrace) |}|
//@[003:0004) |   ├─Token(NewLine) |\n|
  {
//@[002:0047) |   ├─ArrayItemSyntax
//@[002:0047) |   | └─ObjectSyntax
//@[002:0003) |   |   ├─Token(LeftBrace) |{|
//@[003:0004) |   |   ├─Token(NewLine) |\n|
    name: 'plain'
//@[004:0017) |   |   ├─ObjectPropertySyntax
//@[004:0008) |   |   | ├─IdentifierSyntax
//@[004:0008) |   |   | | └─Token(Identifier) |name|
//@[008:0009) |   |   | ├─Token(Colon) |:|
//@[010:0017) |   |   | └─StringSyntax
//@[010:0017) |   |   |   └─Token(StringComplete) |'plain'|
//@[017:0018) |   |   ├─Token(NewLine) |\n|
    tag: 'plain-text'
//@[004:0021) |   |   ├─ObjectPropertySyntax
//@[004:0007) |   |   | ├─IdentifierSyntax
//@[004:0007) |   |   | | └─Token(Identifier) |tag|
//@[007:0008) |   |   | ├─Token(Colon) |:|
//@[009:0021) |   |   | └─StringSyntax
//@[009:0021) |   |   |   └─Token(StringComplete) |'plain-text'|
//@[021:0022) |   |   ├─Token(NewLine) |\n|
  }
//@[002:0003) |   |   └─Token(RightBrace) |}|
//@[003:0004) |   ├─Token(NewLine) |\n|
]
//@[000:0001) |   └─Token(RightSquare) |]|
//@[001:0003) ├─Token(NewLine) |\n\n|

module siteDeploy 'br:mock-registry-two.invalid/demo/site:v3' = [for site in websites: {
//@[000:0287) ├─ModuleDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |module|
//@[007:0017) | ├─IdentifierSyntax
//@[007:0017) | | └─Token(Identifier) |siteDeploy|
//@[018:0061) | ├─StringSyntax
//@[018:0061) | | └─Token(StringComplete) |'br:mock-registry-two.invalid/demo/site:v3'|
//@[062:0063) | ├─Token(Assignment) |=|
//@[064:0287) | └─ForSyntax
//@[064:0065) |   ├─Token(LeftSquare) |[|
//@[065:0068) |   ├─Token(Identifier) |for|
//@[069:0073) |   ├─LocalVariableSyntax
//@[069:0073) |   | └─IdentifierSyntax
//@[069:0073) |   |   └─Token(Identifier) |site|
//@[074:0076) |   ├─Token(Identifier) |in|
//@[077:0085) |   ├─VariableAccessSyntax
//@[077:0085) |   | └─IdentifierSyntax
//@[077:0085) |   |   └─Token(Identifier) |websites|
//@[085:0086) |   ├─Token(Colon) |:|
//@[087:0286) |   ├─ObjectSyntax
//@[087:0088) |   | ├─Token(LeftBrace) |{|
//@[088:0089) |   | ├─Token(NewLine) |\n|
  name: '${site.name}siteDeploy'
//@[002:0032) |   | ├─ObjectPropertySyntax
//@[002:0006) |   | | ├─IdentifierSyntax
//@[002:0006) |   | | | └─Token(Identifier) |name|
//@[006:0007) |   | | ├─Token(Colon) |:|
//@[008:0032) |   | | └─StringSyntax
//@[008:0011) |   | |   ├─Token(StringLeftPiece) |'${|
//@[011:0020) |   | |   ├─PropertyAccessSyntax
//@[011:0015) |   | |   | ├─VariableAccessSyntax
//@[011:0015) |   | |   | | └─IdentifierSyntax
//@[011:0015) |   | |   | |   └─Token(Identifier) |site|
//@[015:0016) |   | |   | ├─Token(Dot) |.|
//@[016:0020) |   | |   | └─IdentifierSyntax
//@[016:0020) |   | |   |   └─Token(Identifier) |name|
//@[020:0032) |   | |   └─Token(StringRightPiece) |}siteDeploy'|
//@[032:0033) |   | ├─Token(NewLine) |\n|
  scope: rg
//@[002:0011) |   | ├─ObjectPropertySyntax
//@[002:0007) |   | | ├─IdentifierSyntax
//@[002:0007) |   | | | └─Token(Identifier) |scope|
//@[007:0008) |   | | ├─Token(Colon) |:|
//@[009:0011) |   | | └─VariableAccessSyntax
//@[009:0011) |   | |   └─IdentifierSyntax
//@[009:0011) |   | |     └─Token(Identifier) |rg|
//@[011:0012) |   | ├─Token(NewLine) |\n|
  params: {
//@[002:0150) |   | ├─ObjectPropertySyntax
//@[002:0008) |   | | ├─IdentifierSyntax
//@[002:0008) |   | | | └─Token(Identifier) |params|
//@[008:0009) |   | | ├─Token(Colon) |:|
//@[010:0150) |   | | └─ObjectSyntax
//@[010:0011) |   | |   ├─Token(LeftBrace) |{|
//@[011:0012) |   | |   ├─Token(NewLine) |\n|
    appPlanId: appPlanDeploy.outputs.planId
//@[004:0043) |   | |   ├─ObjectPropertySyntax
//@[004:0013) |   | |   | ├─IdentifierSyntax
//@[004:0013) |   | |   | | └─Token(Identifier) |appPlanId|
//@[013:0014) |   | |   | ├─Token(Colon) |:|
//@[015:0043) |   | |   | └─PropertyAccessSyntax
//@[015:0036) |   | |   |   ├─PropertyAccessSyntax
//@[015:0028) |   | |   |   | ├─VariableAccessSyntax
//@[015:0028) |   | |   |   | | └─IdentifierSyntax
//@[015:0028) |   | |   |   | |   └─Token(Identifier) |appPlanDeploy|
//@[028:0029) |   | |   |   | ├─Token(Dot) |.|
//@[029:0036) |   | |   |   | └─IdentifierSyntax
//@[029:0036) |   | |   |   |   └─Token(Identifier) |outputs|
//@[036:0037) |   | |   |   ├─Token(Dot) |.|
//@[037:0043) |   | |   |   └─IdentifierSyntax
//@[037:0043) |   | |   |     └─Token(Identifier) |planId|
//@[043:0044) |   | |   ├─Token(NewLine) |\n|
    namePrefix: site.name
//@[004:0025) |   | |   ├─ObjectPropertySyntax
//@[004:0014) |   | |   | ├─IdentifierSyntax
//@[004:0014) |   | |   | | └─Token(Identifier) |namePrefix|
//@[014:0015) |   | |   | ├─Token(Colon) |:|
//@[016:0025) |   | |   | └─PropertyAccessSyntax
//@[016:0020) |   | |   |   ├─VariableAccessSyntax
//@[016:0020) |   | |   |   | └─IdentifierSyntax
//@[016:0020) |   | |   |   |   └─Token(Identifier) |site|
//@[020:0021) |   | |   |   ├─Token(Dot) |.|
//@[021:0025) |   | |   |   └─IdentifierSyntax
//@[021:0025) |   | |   |     └─Token(Identifier) |name|
//@[025:0026) |   | |   ├─Token(NewLine) |\n|
    dockerImage: 'nginxdemos/hello'
//@[004:0035) |   | |   ├─ObjectPropertySyntax
//@[004:0015) |   | |   | ├─IdentifierSyntax
//@[004:0015) |   | |   | | └─Token(Identifier) |dockerImage|
//@[015:0016) |   | |   | ├─Token(Colon) |:|
//@[017:0035) |   | |   | └─StringSyntax
//@[017:0035) |   | |   |   └─Token(StringComplete) |'nginxdemos/hello'|
//@[035:0036) |   | |   ├─Token(NewLine) |\n|
    dockerImageTag: site.tag
//@[004:0028) |   | |   ├─ObjectPropertySyntax
//@[004:0018) |   | |   | ├─IdentifierSyntax
//@[004:0018) |   | |   | | └─Token(Identifier) |dockerImageTag|
//@[018:0019) |   | |   | ├─Token(Colon) |:|
//@[020:0028) |   | |   | └─PropertyAccessSyntax
//@[020:0024) |   | |   |   ├─VariableAccessSyntax
//@[020:0024) |   | |   |   | └─IdentifierSyntax
//@[020:0024) |   | |   |   |   └─Token(Identifier) |site|
//@[024:0025) |   | |   |   ├─Token(Dot) |.|
//@[025:0028) |   | |   |   └─IdentifierSyntax
//@[025:0028) |   | |   |     └─Token(Identifier) |tag|
//@[028:0029) |   | |   ├─Token(NewLine) |\n|
  }
//@[002:0003) |   | |   └─Token(RightBrace) |}|
//@[003:0004) |   | ├─Token(NewLine) |\n|
}]
//@[000:0001) |   | └─Token(RightBrace) |}|
//@[001:0002) |   └─Token(RightSquare) |]|
//@[002:0004) ├─Token(NewLine) |\n\n|

module siteDeploy2 'br/demo-two:site:v3' = [for site in websites: {
//@[000:0267) ├─ModuleDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |module|
//@[007:0018) | ├─IdentifierSyntax
//@[007:0018) | | └─Token(Identifier) |siteDeploy2|
//@[019:0040) | ├─StringSyntax
//@[019:0040) | | └─Token(StringComplete) |'br/demo-two:site:v3'|
//@[041:0042) | ├─Token(Assignment) |=|
//@[043:0267) | └─ForSyntax
//@[043:0044) |   ├─Token(LeftSquare) |[|
//@[044:0047) |   ├─Token(Identifier) |for|
//@[048:0052) |   ├─LocalVariableSyntax
//@[048:0052) |   | └─IdentifierSyntax
//@[048:0052) |   |   └─Token(Identifier) |site|
//@[053:0055) |   ├─Token(Identifier) |in|
//@[056:0064) |   ├─VariableAccessSyntax
//@[056:0064) |   | └─IdentifierSyntax
//@[056:0064) |   |   └─Token(Identifier) |websites|
//@[064:0065) |   ├─Token(Colon) |:|
//@[066:0266) |   ├─ObjectSyntax
//@[066:0067) |   | ├─Token(LeftBrace) |{|
//@[067:0068) |   | ├─Token(NewLine) |\n|
  name: '${site.name}siteDeploy2'
//@[002:0033) |   | ├─ObjectPropertySyntax
//@[002:0006) |   | | ├─IdentifierSyntax
//@[002:0006) |   | | | └─Token(Identifier) |name|
//@[006:0007) |   | | ├─Token(Colon) |:|
//@[008:0033) |   | | └─StringSyntax
//@[008:0011) |   | |   ├─Token(StringLeftPiece) |'${|
//@[011:0020) |   | |   ├─PropertyAccessSyntax
//@[011:0015) |   | |   | ├─VariableAccessSyntax
//@[011:0015) |   | |   | | └─IdentifierSyntax
//@[011:0015) |   | |   | |   └─Token(Identifier) |site|
//@[015:0016) |   | |   | ├─Token(Dot) |.|
//@[016:0020) |   | |   | └─IdentifierSyntax
//@[016:0020) |   | |   |   └─Token(Identifier) |name|
//@[020:0033) |   | |   └─Token(StringRightPiece) |}siteDeploy2'|
//@[033:0034) |   | ├─Token(NewLine) |\n|
  scope: rg
//@[002:0011) |   | ├─ObjectPropertySyntax
//@[002:0007) |   | | ├─IdentifierSyntax
//@[002:0007) |   | | | └─Token(Identifier) |scope|
//@[007:0008) |   | | ├─Token(Colon) |:|
//@[009:0011) |   | | └─VariableAccessSyntax
//@[009:0011) |   | |   └─IdentifierSyntax
//@[009:0011) |   | |     └─Token(Identifier) |rg|
//@[011:0012) |   | ├─Token(NewLine) |\n|
  params: {
//@[002:0150) |   | ├─ObjectPropertySyntax
//@[002:0008) |   | | ├─IdentifierSyntax
//@[002:0008) |   | | | └─Token(Identifier) |params|
//@[008:0009) |   | | ├─Token(Colon) |:|
//@[010:0150) |   | | └─ObjectSyntax
//@[010:0011) |   | |   ├─Token(LeftBrace) |{|
//@[011:0012) |   | |   ├─Token(NewLine) |\n|
    appPlanId: appPlanDeploy.outputs.planId
//@[004:0043) |   | |   ├─ObjectPropertySyntax
//@[004:0013) |   | |   | ├─IdentifierSyntax
//@[004:0013) |   | |   | | └─Token(Identifier) |appPlanId|
//@[013:0014) |   | |   | ├─Token(Colon) |:|
//@[015:0043) |   | |   | └─PropertyAccessSyntax
//@[015:0036) |   | |   |   ├─PropertyAccessSyntax
//@[015:0028) |   | |   |   | ├─VariableAccessSyntax
//@[015:0028) |   | |   |   | | └─IdentifierSyntax
//@[015:0028) |   | |   |   | |   └─Token(Identifier) |appPlanDeploy|
//@[028:0029) |   | |   |   | ├─Token(Dot) |.|
//@[029:0036) |   | |   |   | └─IdentifierSyntax
//@[029:0036) |   | |   |   |   └─Token(Identifier) |outputs|
//@[036:0037) |   | |   |   ├─Token(Dot) |.|
//@[037:0043) |   | |   |   └─IdentifierSyntax
//@[037:0043) |   | |   |     └─Token(Identifier) |planId|
//@[043:0044) |   | |   ├─Token(NewLine) |\n|
    namePrefix: site.name
//@[004:0025) |   | |   ├─ObjectPropertySyntax
//@[004:0014) |   | |   | ├─IdentifierSyntax
//@[004:0014) |   | |   | | └─Token(Identifier) |namePrefix|
//@[014:0015) |   | |   | ├─Token(Colon) |:|
//@[016:0025) |   | |   | └─PropertyAccessSyntax
//@[016:0020) |   | |   |   ├─VariableAccessSyntax
//@[016:0020) |   | |   |   | └─IdentifierSyntax
//@[016:0020) |   | |   |   |   └─Token(Identifier) |site|
//@[020:0021) |   | |   |   ├─Token(Dot) |.|
//@[021:0025) |   | |   |   └─IdentifierSyntax
//@[021:0025) |   | |   |     └─Token(Identifier) |name|
//@[025:0026) |   | |   ├─Token(NewLine) |\n|
    dockerImage: 'nginxdemos/hello'
//@[004:0035) |   | |   ├─ObjectPropertySyntax
//@[004:0015) |   | |   | ├─IdentifierSyntax
//@[004:0015) |   | |   | | └─Token(Identifier) |dockerImage|
//@[015:0016) |   | |   | ├─Token(Colon) |:|
//@[017:0035) |   | |   | └─StringSyntax
//@[017:0035) |   | |   |   └─Token(StringComplete) |'nginxdemos/hello'|
//@[035:0036) |   | |   ├─Token(NewLine) |\n|
    dockerImageTag: site.tag
//@[004:0028) |   | |   ├─ObjectPropertySyntax
//@[004:0018) |   | |   | ├─IdentifierSyntax
//@[004:0018) |   | |   | | └─Token(Identifier) |dockerImageTag|
//@[018:0019) |   | |   | ├─Token(Colon) |:|
//@[020:0028) |   | |   | └─PropertyAccessSyntax
//@[020:0024) |   | |   |   ├─VariableAccessSyntax
//@[020:0024) |   | |   |   | └─IdentifierSyntax
//@[020:0024) |   | |   |   |   └─Token(Identifier) |site|
//@[024:0025) |   | |   |   ├─Token(Dot) |.|
//@[025:0028) |   | |   |   └─IdentifierSyntax
//@[025:0028) |   | |   |     └─Token(Identifier) |tag|
//@[028:0029) |   | |   ├─Token(NewLine) |\n|
  }
//@[002:0003) |   | |   └─Token(RightBrace) |}|
//@[003:0004) |   | ├─Token(NewLine) |\n|
}]
//@[000:0001) |   | └─Token(RightBrace) |}|
//@[001:0002) |   └─Token(RightSquare) |]|
//@[002:0004) ├─Token(NewLine) |\n\n|

module storageDeploy 'ts:00000000-0000-0000-0000-000000000000/test-rg/storage-spec:1.0' = {
//@[000:0168) ├─ModuleDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |module|
//@[007:0020) | ├─IdentifierSyntax
//@[007:0020) | | └─Token(Identifier) |storageDeploy|
//@[021:0087) | ├─StringSyntax
//@[021:0087) | | └─Token(StringComplete) |'ts:00000000-0000-0000-0000-000000000000/test-rg/storage-spec:1.0'|
//@[088:0089) | ├─Token(Assignment) |=|
//@[090:0168) | └─ObjectSyntax
//@[090:0091) |   ├─Token(LeftBrace) |{|
//@[091:0092) |   ├─Token(NewLine) |\n|
  name: 'storageDeploy'
//@[002:0023) |   ├─ObjectPropertySyntax
//@[002:0006) |   | ├─IdentifierSyntax
//@[002:0006) |   | | └─Token(Identifier) |name|
//@[006:0007) |   | ├─Token(Colon) |:|
//@[008:0023) |   | └─StringSyntax
//@[008:0023) |   |   └─Token(StringComplete) |'storageDeploy'|
//@[023:0024) |   ├─Token(NewLine) |\n|
  scope: rg
//@[002:0011) |   ├─ObjectPropertySyntax
//@[002:0007) |   | ├─IdentifierSyntax
//@[002:0007) |   | | └─Token(Identifier) |scope|
//@[007:0008) |   | ├─Token(Colon) |:|
//@[009:0011) |   | └─VariableAccessSyntax
//@[009:0011) |   |   └─IdentifierSyntax
//@[009:0011) |   |     └─Token(Identifier) |rg|
//@[011:0012) |   ├─Token(NewLine) |\n|
  params: {
//@[002:0038) |   ├─ObjectPropertySyntax
//@[002:0008) |   | ├─IdentifierSyntax
//@[002:0008) |   | | └─Token(Identifier) |params|
//@[008:0009) |   | ├─Token(Colon) |:|
//@[010:0038) |   | └─ObjectSyntax
//@[010:0011) |   |   ├─Token(LeftBrace) |{|
//@[011:0012) |   |   ├─Token(NewLine) |\n|
    location: 'eastus'
//@[004:0022) |   |   ├─ObjectPropertySyntax
//@[004:0012) |   |   | ├─IdentifierSyntax
//@[004:0012) |   |   | | └─Token(Identifier) |location|
//@[012:0013) |   |   | ├─Token(Colon) |:|
//@[014:0022) |   |   | └─StringSyntax
//@[014:0022) |   |   |   └─Token(StringComplete) |'eastus'|
//@[022:0023) |   |   ├─Token(NewLine) |\n|
  }
//@[002:0003) |   |   └─Token(RightBrace) |}|
//@[003:0004) |   ├─Token(NewLine) |\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0003) ├─Token(NewLine) |\n\n|

module storageDeploy2 'ts/mySpecRG:storage-spec:1.0' = {
//@[000:0134) ├─ModuleDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |module|
//@[007:0021) | ├─IdentifierSyntax
//@[007:0021) | | └─Token(Identifier) |storageDeploy2|
//@[022:0052) | ├─StringSyntax
//@[022:0052) | | └─Token(StringComplete) |'ts/mySpecRG:storage-spec:1.0'|
//@[053:0054) | ├─Token(Assignment) |=|
//@[055:0134) | └─ObjectSyntax
//@[055:0056) |   ├─Token(LeftBrace) |{|
//@[056:0057) |   ├─Token(NewLine) |\n|
  name: 'storageDeploy2'
//@[002:0024) |   ├─ObjectPropertySyntax
//@[002:0006) |   | ├─IdentifierSyntax
//@[002:0006) |   | | └─Token(Identifier) |name|
//@[006:0007) |   | ├─Token(Colon) |:|
//@[008:0024) |   | └─StringSyntax
//@[008:0024) |   |   └─Token(StringComplete) |'storageDeploy2'|
//@[024:0025) |   ├─Token(NewLine) |\n|
  scope: rg
//@[002:0011) |   ├─ObjectPropertySyntax
//@[002:0007) |   | ├─IdentifierSyntax
//@[002:0007) |   | | └─Token(Identifier) |scope|
//@[007:0008) |   | ├─Token(Colon) |:|
//@[009:0011) |   | └─VariableAccessSyntax
//@[009:0011) |   |   └─IdentifierSyntax
//@[009:0011) |   |     └─Token(Identifier) |rg|
//@[011:0012) |   ├─Token(NewLine) |\n|
  params: {
//@[002:0038) |   ├─ObjectPropertySyntax
//@[002:0008) |   | ├─IdentifierSyntax
//@[002:0008) |   | | └─Token(Identifier) |params|
//@[008:0009) |   | ├─Token(Colon) |:|
//@[010:0038) |   | └─ObjectSyntax
//@[010:0011) |   |   ├─Token(LeftBrace) |{|
//@[011:0012) |   |   ├─Token(NewLine) |\n|
    location: 'eastus'
//@[004:0022) |   |   ├─ObjectPropertySyntax
//@[004:0012) |   |   | ├─IdentifierSyntax
//@[004:0012) |   |   | | └─Token(Identifier) |location|
//@[012:0013) |   |   | ├─Token(Colon) |:|
//@[014:0022) |   |   | └─StringSyntax
//@[014:0022) |   |   |   └─Token(StringComplete) |'eastus'|
//@[022:0023) |   |   ├─Token(NewLine) |\n|
  }
//@[002:0003) |   |   └─Token(RightBrace) |}|
//@[003:0004) |   ├─Token(NewLine) |\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0003) ├─Token(NewLine) |\n\n|

var vnets = [
//@[000:0123) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0009) | ├─IdentifierSyntax
//@[004:0009) | | └─Token(Identifier) |vnets|
//@[010:0011) | ├─Token(Assignment) |=|
//@[012:0123) | └─ArraySyntax
//@[012:0013) |   ├─Token(LeftSquare) |[|
//@[013:0014) |   ├─Token(NewLine) |\n|
  {
//@[002:0053) |   ├─ArrayItemSyntax
//@[002:0053) |   | └─ObjectSyntax
//@[002:0003) |   |   ├─Token(LeftBrace) |{|
//@[003:0004) |   |   ├─Token(NewLine) |\n|
    name: 'vnet1'
//@[004:0017) |   |   ├─ObjectPropertySyntax
//@[004:0008) |   |   | ├─IdentifierSyntax
//@[004:0008) |   |   | | └─Token(Identifier) |name|
//@[008:0009) |   |   | ├─Token(Colon) |:|
//@[010:0017) |   |   | └─StringSyntax
//@[010:0017) |   |   |   └─Token(StringComplete) |'vnet1'|
//@[017:0018) |   |   ├─Token(NewLine) |\n|
    subnetName: 'subnet1.1'
//@[004:0027) |   |   ├─ObjectPropertySyntax
//@[004:0014) |   |   | ├─IdentifierSyntax
//@[004:0014) |   |   | | └─Token(Identifier) |subnetName|
//@[014:0015) |   |   | ├─Token(Colon) |:|
//@[016:0027) |   |   | └─StringSyntax
//@[016:0027) |   |   |   └─Token(StringComplete) |'subnet1.1'|
//@[027:0028) |   |   ├─Token(NewLine) |\n|
  }
//@[002:0003) |   |   └─Token(RightBrace) |}|
//@[003:0004) |   ├─Token(NewLine) |\n|
  {
//@[002:0053) |   ├─ArrayItemSyntax
//@[002:0053) |   | └─ObjectSyntax
//@[002:0003) |   |   ├─Token(LeftBrace) |{|
//@[003:0004) |   |   ├─Token(NewLine) |\n|
    name: 'vnet2'
//@[004:0017) |   |   ├─ObjectPropertySyntax
//@[004:0008) |   |   | ├─IdentifierSyntax
//@[004:0008) |   |   | | └─Token(Identifier) |name|
//@[008:0009) |   |   | ├─Token(Colon) |:|
//@[010:0017) |   |   | └─StringSyntax
//@[010:0017) |   |   |   └─Token(StringComplete) |'vnet2'|
//@[017:0018) |   |   ├─Token(NewLine) |\n|
    subnetName: 'subnet2.1'
//@[004:0027) |   |   ├─ObjectPropertySyntax
//@[004:0014) |   |   | ├─IdentifierSyntax
//@[004:0014) |   |   | | └─Token(Identifier) |subnetName|
//@[014:0015) |   |   | ├─Token(Colon) |:|
//@[016:0027) |   |   | └─StringSyntax
//@[016:0027) |   |   |   └─Token(StringComplete) |'subnet2.1'|
//@[027:0028) |   |   ├─Token(NewLine) |\n|
  }
//@[002:0003) |   |   └─Token(RightBrace) |}|
//@[003:0004) |   ├─Token(NewLine) |\n|
]
//@[000:0001) |   └─Token(RightSquare) |]|
//@[001:0003) ├─Token(NewLine) |\n\n|

module vnetDeploy 'ts:11111111-1111-1111-1111-111111111111/prod-rg/vnet-spec:v2' = [for vnet in vnets: {
//@[000:0220) ├─ModuleDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |module|
//@[007:0017) | ├─IdentifierSyntax
//@[007:0017) | | └─Token(Identifier) |vnetDeploy|
//@[018:0080) | ├─StringSyntax
//@[018:0080) | | └─Token(StringComplete) |'ts:11111111-1111-1111-1111-111111111111/prod-rg/vnet-spec:v2'|
//@[081:0082) | ├─Token(Assignment) |=|
//@[083:0220) | └─ForSyntax
//@[083:0084) |   ├─Token(LeftSquare) |[|
//@[084:0087) |   ├─Token(Identifier) |for|
//@[088:0092) |   ├─LocalVariableSyntax
//@[088:0092) |   | └─IdentifierSyntax
//@[088:0092) |   |   └─Token(Identifier) |vnet|
//@[093:0095) |   ├─Token(Identifier) |in|
//@[096:0101) |   ├─VariableAccessSyntax
//@[096:0101) |   | └─IdentifierSyntax
//@[096:0101) |   |   └─Token(Identifier) |vnets|
//@[101:0102) |   ├─Token(Colon) |:|
//@[103:0219) |   ├─ObjectSyntax
//@[103:0104) |   | ├─Token(LeftBrace) |{|
//@[104:0105) |   | ├─Token(NewLine) |\n|
  name: '${vnet.name}Deploy'
//@[002:0028) |   | ├─ObjectPropertySyntax
//@[002:0006) |   | | ├─IdentifierSyntax
//@[002:0006) |   | | | └─Token(Identifier) |name|
//@[006:0007) |   | | ├─Token(Colon) |:|
//@[008:0028) |   | | └─StringSyntax
//@[008:0011) |   | |   ├─Token(StringLeftPiece) |'${|
//@[011:0020) |   | |   ├─PropertyAccessSyntax
//@[011:0015) |   | |   | ├─VariableAccessSyntax
//@[011:0015) |   | |   | | └─IdentifierSyntax
//@[011:0015) |   | |   | |   └─Token(Identifier) |vnet|
//@[015:0016) |   | |   | ├─Token(Dot) |.|
//@[016:0020) |   | |   | └─IdentifierSyntax
//@[016:0020) |   | |   |   └─Token(Identifier) |name|
//@[020:0028) |   | |   └─Token(StringRightPiece) |}Deploy'|
//@[028:0029) |   | ├─Token(NewLine) |\n|
  scope: rg
//@[002:0011) |   | ├─ObjectPropertySyntax
//@[002:0007) |   | | ├─IdentifierSyntax
//@[002:0007) |   | | | └─Token(Identifier) |scope|
//@[007:0008) |   | | ├─Token(Colon) |:|
//@[009:0011) |   | | └─VariableAccessSyntax
//@[009:0011) |   | |   └─IdentifierSyntax
//@[009:0011) |   | |     └─Token(Identifier) |rg|
//@[011:0012) |   | ├─Token(NewLine) |\n|
  params: {
//@[002:0071) |   | ├─ObjectPropertySyntax
//@[002:0008) |   | | ├─IdentifierSyntax
//@[002:0008) |   | | | └─Token(Identifier) |params|
//@[008:0009) |   | | ├─Token(Colon) |:|
//@[010:0071) |   | | └─ObjectSyntax
//@[010:0011) |   | |   ├─Token(LeftBrace) |{|
//@[011:0012) |   | |   ├─Token(NewLine) |\n|
    vnetName: vnet.name
//@[004:0023) |   | |   ├─ObjectPropertySyntax
//@[004:0012) |   | |   | ├─IdentifierSyntax
//@[004:0012) |   | |   | | └─Token(Identifier) |vnetName|
//@[012:0013) |   | |   | ├─Token(Colon) |:|
//@[014:0023) |   | |   | └─PropertyAccessSyntax
//@[014:0018) |   | |   |   ├─VariableAccessSyntax
//@[014:0018) |   | |   |   | └─IdentifierSyntax
//@[014:0018) |   | |   |   |   └─Token(Identifier) |vnet|
//@[018:0019) |   | |   |   ├─Token(Dot) |.|
//@[019:0023) |   | |   |   └─IdentifierSyntax
//@[019:0023) |   | |   |     └─Token(Identifier) |name|
//@[023:0024) |   | |   ├─Token(NewLine) |\n|
    subnetName: vnet.subnetName
//@[004:0031) |   | |   ├─ObjectPropertySyntax
//@[004:0014) |   | |   | ├─IdentifierSyntax
//@[004:0014) |   | |   | | └─Token(Identifier) |subnetName|
//@[014:0015) |   | |   | ├─Token(Colon) |:|
//@[016:0031) |   | |   | └─PropertyAccessSyntax
//@[016:0020) |   | |   |   ├─VariableAccessSyntax
//@[016:0020) |   | |   |   | └─IdentifierSyntax
//@[016:0020) |   | |   |   |   └─Token(Identifier) |vnet|
//@[020:0021) |   | |   |   ├─Token(Dot) |.|
//@[021:0031) |   | |   |   └─IdentifierSyntax
//@[021:0031) |   | |   |     └─Token(Identifier) |subnetName|
//@[031:0032) |   | |   ├─Token(NewLine) |\n|
  }
//@[002:0003) |   | |   └─Token(RightBrace) |}|
//@[003:0004) |   | ├─Token(NewLine) |\n|
}]
//@[000:0001) |   | └─Token(RightBrace) |}|
//@[001:0002) |   └─Token(RightSquare) |]|
//@[002:0004) ├─Token(NewLine) |\n\n|

output siteUrls array = [for (site, i) in websites: siteDeploy[i].outputs.siteUrl]
//@[000:0082) ├─OutputDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |output|
//@[007:0015) | ├─IdentifierSyntax
//@[007:0015) | | └─Token(Identifier) |siteUrls|
//@[016:0021) | ├─SimpleTypeSyntax
//@[016:0021) | | └─Token(Identifier) |array|
//@[022:0023) | ├─Token(Assignment) |=|
//@[024:0082) | └─ForSyntax
//@[024:0025) |   ├─Token(LeftSquare) |[|
//@[025:0028) |   ├─Token(Identifier) |for|
//@[029:0038) |   ├─VariableBlockSyntax
//@[029:0030) |   | ├─Token(LeftParen) |(|
//@[030:0034) |   | ├─LocalVariableSyntax
//@[030:0034) |   | | └─IdentifierSyntax
//@[030:0034) |   | |   └─Token(Identifier) |site|
//@[034:0035) |   | ├─Token(Comma) |,|
//@[036:0037) |   | ├─LocalVariableSyntax
//@[036:0037) |   | | └─IdentifierSyntax
//@[036:0037) |   | |   └─Token(Identifier) |i|
//@[037:0038) |   | └─Token(RightParen) |)|
//@[039:0041) |   ├─Token(Identifier) |in|
//@[042:0050) |   ├─VariableAccessSyntax
//@[042:0050) |   | └─IdentifierSyntax
//@[042:0050) |   |   └─Token(Identifier) |websites|
//@[050:0051) |   ├─Token(Colon) |:|
//@[052:0081) |   ├─PropertyAccessSyntax
//@[052:0073) |   | ├─PropertyAccessSyntax
//@[052:0065) |   | | ├─ArrayAccessSyntax
//@[052:0062) |   | | | ├─VariableAccessSyntax
//@[052:0062) |   | | | | └─IdentifierSyntax
//@[052:0062) |   | | | |   └─Token(Identifier) |siteDeploy|
//@[062:0063) |   | | | ├─Token(LeftSquare) |[|
//@[063:0064) |   | | | ├─VariableAccessSyntax
//@[063:0064) |   | | | | └─IdentifierSyntax
//@[063:0064) |   | | | |   └─Token(Identifier) |i|
//@[064:0065) |   | | | └─Token(RightSquare) |]|
//@[065:0066) |   | | ├─Token(Dot) |.|
//@[066:0073) |   | | └─IdentifierSyntax
//@[066:0073) |   | |   └─Token(Identifier) |outputs|
//@[073:0074) |   | ├─Token(Dot) |.|
//@[074:0081) |   | └─IdentifierSyntax
//@[074:0081) |   |   └─Token(Identifier) |siteUrl|
//@[081:0082) |   └─Token(RightSquare) |]|
//@[082:0084) ├─Token(NewLine) |\n\n|

module passthroughPort 'br:localhost:5000/passthrough/port:v1' = {
//@[000:0128) ├─ModuleDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |module|
//@[007:0022) | ├─IdentifierSyntax
//@[007:0022) | | └─Token(Identifier) |passthroughPort|
//@[023:0062) | ├─StringSyntax
//@[023:0062) | | └─Token(StringComplete) |'br:localhost:5000/passthrough/port:v1'|
//@[063:0064) | ├─Token(Assignment) |=|
//@[065:0128) | └─ObjectSyntax
//@[065:0066) |   ├─Token(LeftBrace) |{|
//@[066:0067) |   ├─Token(NewLine) |\n|
  scope: rg
//@[002:0011) |   ├─ObjectPropertySyntax
//@[002:0007) |   | ├─IdentifierSyntax
//@[002:0007) |   | | └─Token(Identifier) |scope|
//@[007:0008) |   | ├─Token(Colon) |:|
//@[009:0011) |   | └─VariableAccessSyntax
//@[009:0011) |   |   └─IdentifierSyntax
//@[009:0011) |   |     └─Token(Identifier) |rg|
//@[011:0012) |   ├─Token(NewLine) |\n|
  name: 'port'
//@[002:0014) |   ├─ObjectPropertySyntax
//@[002:0006) |   | ├─IdentifierSyntax
//@[002:0006) |   | | └─Token(Identifier) |name|
//@[006:0007) |   | ├─Token(Colon) |:|
//@[008:0014) |   | └─StringSyntax
//@[008:0014) |   |   └─Token(StringComplete) |'port'|
//@[014:0015) |   ├─Token(NewLine) |\n|
  params: {
//@[002:0032) |   ├─ObjectPropertySyntax
//@[002:0008) |   | ├─IdentifierSyntax
//@[002:0008) |   | | └─Token(Identifier) |params|
//@[008:0009) |   | ├─Token(Colon) |:|
//@[010:0032) |   | └─ObjectSyntax
//@[010:0011) |   |   ├─Token(LeftBrace) |{|
//@[011:0012) |   |   ├─Token(NewLine) |\n|
    port: 'test'
//@[004:0016) |   |   ├─ObjectPropertySyntax
//@[004:0008) |   |   | ├─IdentifierSyntax
//@[004:0008) |   |   | | └─Token(Identifier) |port|
//@[008:0009) |   |   | ├─Token(Colon) |:|
//@[010:0016) |   |   | └─StringSyntax
//@[010:0016) |   |   |   └─Token(StringComplete) |'test'|
//@[016:0017) |   |   ├─Token(NewLine) |\n|
  }
//@[002:0003) |   |   └─Token(RightBrace) |}|
//@[003:0004) |   ├─Token(NewLine) |\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0003) ├─Token(NewLine) |\n\n|

module ipv4 'br:127.0.0.1/passthrough/ipv4:v1' = {
//@[000:0112) ├─ModuleDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |module|
//@[007:0011) | ├─IdentifierSyntax
//@[007:0011) | | └─Token(Identifier) |ipv4|
//@[012:0046) | ├─StringSyntax
//@[012:0046) | | └─Token(StringComplete) |'br:127.0.0.1/passthrough/ipv4:v1'|
//@[047:0048) | ├─Token(Assignment) |=|
//@[049:0112) | └─ObjectSyntax
//@[049:0050) |   ├─Token(LeftBrace) |{|
//@[050:0051) |   ├─Token(NewLine) |\n|
  scope: rg
//@[002:0011) |   ├─ObjectPropertySyntax
//@[002:0007) |   | ├─IdentifierSyntax
//@[002:0007) |   | | └─Token(Identifier) |scope|
//@[007:0008) |   | ├─Token(Colon) |:|
//@[009:0011) |   | └─VariableAccessSyntax
//@[009:0011) |   |   └─IdentifierSyntax
//@[009:0011) |   |     └─Token(Identifier) |rg|
//@[011:0012) |   ├─Token(NewLine) |\n|
  name: 'ipv4'
//@[002:0014) |   ├─ObjectPropertySyntax
//@[002:0006) |   | ├─IdentifierSyntax
//@[002:0006) |   | | └─Token(Identifier) |name|
//@[006:0007) |   | ├─Token(Colon) |:|
//@[008:0014) |   | └─StringSyntax
//@[008:0014) |   |   └─Token(StringComplete) |'ipv4'|
//@[014:0015) |   ├─Token(NewLine) |\n|
  params: {
//@[002:0032) |   ├─ObjectPropertySyntax
//@[002:0008) |   | ├─IdentifierSyntax
//@[002:0008) |   | | └─Token(Identifier) |params|
//@[008:0009) |   | ├─Token(Colon) |:|
//@[010:0032) |   | └─ObjectSyntax
//@[010:0011) |   |   ├─Token(LeftBrace) |{|
//@[011:0012) |   |   ├─Token(NewLine) |\n|
    ipv4: 'test'
//@[004:0016) |   |   ├─ObjectPropertySyntax
//@[004:0008) |   |   | ├─IdentifierSyntax
//@[004:0008) |   |   | | └─Token(Identifier) |ipv4|
//@[008:0009) |   |   | ├─Token(Colon) |:|
//@[010:0016) |   |   | └─StringSyntax
//@[010:0016) |   |   |   └─Token(StringComplete) |'test'|
//@[016:0017) |   |   ├─Token(NewLine) |\n|
  }
//@[002:0003) |   |   └─Token(RightBrace) |}|
//@[003:0004) |   ├─Token(NewLine) |\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0003) ├─Token(NewLine) |\n\n|

module ipv4port 'br:127.0.0.1:5000/passthrough/ipv4port:v1' = {
//@[000:0133) ├─ModuleDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |module|
//@[007:0015) | ├─IdentifierSyntax
//@[007:0015) | | └─Token(Identifier) |ipv4port|
//@[016:0059) | ├─StringSyntax
//@[016:0059) | | └─Token(StringComplete) |'br:127.0.0.1:5000/passthrough/ipv4port:v1'|
//@[060:0061) | ├─Token(Assignment) |=|
//@[062:0133) | └─ObjectSyntax
//@[062:0063) |   ├─Token(LeftBrace) |{|
//@[063:0064) |   ├─Token(NewLine) |\n|
  scope: rg
//@[002:0011) |   ├─ObjectPropertySyntax
//@[002:0007) |   | ├─IdentifierSyntax
//@[002:0007) |   | | └─Token(Identifier) |scope|
//@[007:0008) |   | ├─Token(Colon) |:|
//@[009:0011) |   | └─VariableAccessSyntax
//@[009:0011) |   |   └─IdentifierSyntax
//@[009:0011) |   |     └─Token(Identifier) |rg|
//@[011:0012) |   ├─Token(NewLine) |\n|
  name: 'ipv4port'
//@[002:0018) |   ├─ObjectPropertySyntax
//@[002:0006) |   | ├─IdentifierSyntax
//@[002:0006) |   | | └─Token(Identifier) |name|
//@[006:0007) |   | ├─Token(Colon) |:|
//@[008:0018) |   | └─StringSyntax
//@[008:0018) |   |   └─Token(StringComplete) |'ipv4port'|
//@[018:0019) |   ├─Token(NewLine) |\n|
  params: {
//@[002:0036) |   ├─ObjectPropertySyntax
//@[002:0008) |   | ├─IdentifierSyntax
//@[002:0008) |   | | └─Token(Identifier) |params|
//@[008:0009) |   | ├─Token(Colon) |:|
//@[010:0036) |   | └─ObjectSyntax
//@[010:0011) |   |   ├─Token(LeftBrace) |{|
//@[011:0012) |   |   ├─Token(NewLine) |\n|
    ipv4port: 'test'
//@[004:0020) |   |   ├─ObjectPropertySyntax
//@[004:0012) |   |   | ├─IdentifierSyntax
//@[004:0012) |   |   | | └─Token(Identifier) |ipv4port|
//@[012:0013) |   |   | ├─Token(Colon) |:|
//@[014:0020) |   |   | └─StringSyntax
//@[014:0020) |   |   |   └─Token(StringComplete) |'test'|
//@[020:0021) |   |   ├─Token(NewLine) |\n|
  }
//@[002:0003) |   |   └─Token(RightBrace) |}|
//@[003:0004) |   ├─Token(NewLine) |\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0003) ├─Token(NewLine) |\n\n|

module ipv6 'br:[::1]/passthrough/ipv6:v1' = {
//@[000:0108) ├─ModuleDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |module|
//@[007:0011) | ├─IdentifierSyntax
//@[007:0011) | | └─Token(Identifier) |ipv6|
//@[012:0042) | ├─StringSyntax
//@[012:0042) | | └─Token(StringComplete) |'br:[::1]/passthrough/ipv6:v1'|
//@[043:0044) | ├─Token(Assignment) |=|
//@[045:0108) | └─ObjectSyntax
//@[045:0046) |   ├─Token(LeftBrace) |{|
//@[046:0047) |   ├─Token(NewLine) |\n|
  scope: rg
//@[002:0011) |   ├─ObjectPropertySyntax
//@[002:0007) |   | ├─IdentifierSyntax
//@[002:0007) |   | | └─Token(Identifier) |scope|
//@[007:0008) |   | ├─Token(Colon) |:|
//@[009:0011) |   | └─VariableAccessSyntax
//@[009:0011) |   |   └─IdentifierSyntax
//@[009:0011) |   |     └─Token(Identifier) |rg|
//@[011:0012) |   ├─Token(NewLine) |\n|
  name: 'ipv6'
//@[002:0014) |   ├─ObjectPropertySyntax
//@[002:0006) |   | ├─IdentifierSyntax
//@[002:0006) |   | | └─Token(Identifier) |name|
//@[006:0007) |   | ├─Token(Colon) |:|
//@[008:0014) |   | └─StringSyntax
//@[008:0014) |   |   └─Token(StringComplete) |'ipv6'|
//@[014:0015) |   ├─Token(NewLine) |\n|
  params: {
//@[002:0032) |   ├─ObjectPropertySyntax
//@[002:0008) |   | ├─IdentifierSyntax
//@[002:0008) |   | | └─Token(Identifier) |params|
//@[008:0009) |   | ├─Token(Colon) |:|
//@[010:0032) |   | └─ObjectSyntax
//@[010:0011) |   |   ├─Token(LeftBrace) |{|
//@[011:0012) |   |   ├─Token(NewLine) |\n|
    ipv6: 'test'
//@[004:0016) |   |   ├─ObjectPropertySyntax
//@[004:0008) |   |   | ├─IdentifierSyntax
//@[004:0008) |   |   | | └─Token(Identifier) |ipv6|
//@[008:0009) |   |   | ├─Token(Colon) |:|
//@[010:0016) |   |   | └─StringSyntax
//@[010:0016) |   |   |   └─Token(StringComplete) |'test'|
//@[016:0017) |   |   ├─Token(NewLine) |\n|
  }
//@[002:0003) |   |   └─Token(RightBrace) |}|
//@[003:0004) |   ├─Token(NewLine) |\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0003) ├─Token(NewLine) |\n\n|

module ipv6port 'br:[::1]:5000/passthrough/ipv6port:v1' = {
//@[000:0129) ├─ModuleDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |module|
//@[007:0015) | ├─IdentifierSyntax
//@[007:0015) | | └─Token(Identifier) |ipv6port|
//@[016:0055) | ├─StringSyntax
//@[016:0055) | | └─Token(StringComplete) |'br:[::1]:5000/passthrough/ipv6port:v1'|
//@[056:0057) | ├─Token(Assignment) |=|
//@[058:0129) | └─ObjectSyntax
//@[058:0059) |   ├─Token(LeftBrace) |{|
//@[059:0060) |   ├─Token(NewLine) |\n|
  scope: rg
//@[002:0011) |   ├─ObjectPropertySyntax
//@[002:0007) |   | ├─IdentifierSyntax
//@[002:0007) |   | | └─Token(Identifier) |scope|
//@[007:0008) |   | ├─Token(Colon) |:|
//@[009:0011) |   | └─VariableAccessSyntax
//@[009:0011) |   |   └─IdentifierSyntax
//@[009:0011) |   |     └─Token(Identifier) |rg|
//@[011:0012) |   ├─Token(NewLine) |\n|
  name: 'ipv6port'
//@[002:0018) |   ├─ObjectPropertySyntax
//@[002:0006) |   | ├─IdentifierSyntax
//@[002:0006) |   | | └─Token(Identifier) |name|
//@[006:0007) |   | ├─Token(Colon) |:|
//@[008:0018) |   | └─StringSyntax
//@[008:0018) |   |   └─Token(StringComplete) |'ipv6port'|
//@[018:0019) |   ├─Token(NewLine) |\n|
  params: {
//@[002:0036) |   ├─ObjectPropertySyntax
//@[002:0008) |   | ├─IdentifierSyntax
//@[002:0008) |   | | └─Token(Identifier) |params|
//@[008:0009) |   | ├─Token(Colon) |:|
//@[010:0036) |   | └─ObjectSyntax
//@[010:0011) |   |   ├─Token(LeftBrace) |{|
//@[011:0012) |   |   ├─Token(NewLine) |\n|
    ipv6port: 'test'
//@[004:0020) |   |   ├─ObjectPropertySyntax
//@[004:0012) |   |   | ├─IdentifierSyntax
//@[004:0012) |   |   | | └─Token(Identifier) |ipv6port|
//@[012:0013) |   |   | ├─Token(Colon) |:|
//@[014:0020) |   |   | └─StringSyntax
//@[014:0020) |   |   |   └─Token(StringComplete) |'test'|
//@[020:0021) |   |   ├─Token(NewLine) |\n|
  }
//@[002:0003) |   |   └─Token(RightBrace) |}|
//@[003:0004) |   ├─Token(NewLine) |\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0001) └─Token(EndOfFile) ||
