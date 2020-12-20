targetScope = 'tenant'
//@[0:22) TargetScopeSyntax
//@[0:11)  Identifier |targetScope|
//@[12:13)  Assignment |=|
//@[14:22)  StringSyntax
//@[14:22)   StringComplete |'tenant'|
//@[22:24) NewLine |\n\n|

module tenantModule 'modules/tenant.bicep' = {
//@[0:65) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:19)  IdentifierSyntax
//@[7:19)   Identifier |tenantModule|
//@[20:42)  StringSyntax
//@[20:42)   StringComplete |'modules/tenant.bicep'|
//@[43:44)  Assignment |=|
//@[45:65)  ObjectSyntax
//@[45:46)   LeftBrace |{|
//@[46:47)   NewLine |\n|
  name: 'tenant'
//@[2:16)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:16)    StringSyntax
//@[8:16)     StringComplete |'tenant'|
//@[16:17)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

module managementGroupModule 'modules/managementGroup.bicep' = {
//@[0:123) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:28)  IdentifierSyntax
//@[7:28)   Identifier |managementGroupModule|
//@[29:60)  StringSyntax
//@[29:60)   StringComplete |'modules/managementGroup.bicep'|
//@[61:62)  Assignment |=|
//@[63:123)  ObjectSyntax
//@[63:64)   LeftBrace |{|
//@[64:65)   NewLine |\n|
  name: 'managementGroup'
//@[2:25)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:25)    StringSyntax
//@[8:25)     StringComplete |'managementGroup'|
//@[25:26)   NewLine |\n|
  scope: managementGroup('MG')
//@[2:30)   ObjectPropertySyntax
//@[2:7)    IdentifierSyntax
//@[2:7)     Identifier |scope|
//@[7:8)    Colon |:|
//@[9:30)    FunctionCallSyntax
//@[9:24)     IdentifierSyntax
//@[9:24)      Identifier |managementGroup|
//@[24:25)     LeftParen |(|
//@[25:29)     FunctionArgumentSyntax
//@[25:29)      StringSyntax
//@[25:29)       StringComplete |'MG'|
//@[29:30)     RightParen |)|
//@[30:31)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

module subscriptionModule 'modules/subscription.bicep' = {
//@[0:112) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:25)  IdentifierSyntax
//@[7:25)   Identifier |subscriptionModule|
//@[26:54)  StringSyntax
//@[26:54)   StringComplete |'modules/subscription.bicep'|
//@[55:56)  Assignment |=|
//@[57:112)  ObjectSyntax
//@[57:58)   LeftBrace |{|
//@[58:59)   NewLine |\n|
  name: 'subscription'
//@[2:22)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:22)    StringSyntax
//@[8:22)     StringComplete |'subscription'|
//@[22:23)   NewLine |\n|
  scope: subscription('SUB')
//@[2:28)   ObjectPropertySyntax
//@[2:7)    IdentifierSyntax
//@[2:7)     Identifier |scope|
//@[7:8)    Colon |:|
//@[9:28)    FunctionCallSyntax
//@[9:21)     IdentifierSyntax
//@[9:21)      Identifier |subscription|
//@[21:22)     LeftParen |(|
//@[22:27)     FunctionArgumentSyntax
//@[22:27)      StringSyntax
//@[22:27)       StringComplete |'SUB'|
//@[27:28)     RightParen |)|
//@[28:29)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

resource errorMaker //Incomplete resource for Integration Tests stub - Valid test must pass without warnings, while Invalid ones must not create template and raise an error. Hence this incomplete declaration.
//@[0:208) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:19)  IdentifierSyntax
//@[9:19)   Identifier |errorMaker|
//@[208:208)  SkippedTriviaSyntax
//@[208:208)  SkippedTriviaSyntax
//@[208:208)  SkippedTriviaSyntax
//@[208:208)  SkippedTriviaSyntax
//@[208:208) EndOfFile ||
