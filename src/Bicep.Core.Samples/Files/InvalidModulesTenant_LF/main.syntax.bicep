targetScope = 'tenant'
//@[0:22) TargetScopeSyntax
//@[0:11)  Identifier |targetScope|
//@[12:13)  Assignment |=|
//@[14:22)  StringSyntax
//@[14:22)   StringComplete |'tenant'|
//@[22:24) NewLine |\n\n|

module tenantModuleDuplicateName1 'modules/tenant.bicep' = {
//@[0:116) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:33)  IdentifierSyntax
//@[7:33)   Identifier |tenantModuleDuplicateName1|
//@[34:56)  StringSyntax
//@[34:56)   StringComplete |'modules/tenant.bicep'|
//@[57:58)  Assignment |=|
//@[59:116)  ObjectSyntax
//@[59:60)   LeftBrace |{|
//@[60:61)   NewLine |\n|
  name: 'tenantModuleDuplicateName'
//@[2:35)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:35)    StringSyntax
//@[8:35)     StringComplete |'tenantModuleDuplicateName'|
//@[35:36)   NewLine |\n|
  scope: tenant()
//@[2:17)   ObjectPropertySyntax
//@[2:7)    IdentifierSyntax
//@[2:7)     Identifier |scope|
//@[7:8)    Colon |:|
//@[9:17)    FunctionCallSyntax
//@[9:15)     IdentifierSyntax
//@[9:15)      Identifier |tenant|
//@[15:16)     LeftParen |(|
//@[16:17)     RightParen |)|
//@[17:18)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

module tenantModuleDuplicateName2 'modules/tenant.bicep' = {
//@[0:116) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:33)  IdentifierSyntax
//@[7:33)   Identifier |tenantModuleDuplicateName2|
//@[34:56)  StringSyntax
//@[34:56)   StringComplete |'modules/tenant.bicep'|
//@[57:58)  Assignment |=|
//@[59:116)  ObjectSyntax
//@[59:60)   LeftBrace |{|
//@[60:61)   NewLine |\n|
  name: 'tenantModuleDuplicateName'
//@[2:35)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:35)    StringSyntax
//@[8:35)     StringComplete |'tenantModuleDuplicateName'|
//@[35:36)   NewLine |\n|
  scope: tenant()
//@[2:17)   ObjectPropertySyntax
//@[2:7)    IdentifierSyntax
//@[2:7)     Identifier |scope|
//@[7:8)    Colon |:|
//@[9:17)    FunctionCallSyntax
//@[9:15)     IdentifierSyntax
//@[9:15)      Identifier |tenant|
//@[15:16)     LeftParen |(|
//@[16:17)     RightParen |)|
//@[17:18)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

module tenantModuleDuplicateName3 'modules/tenant.bicep' = {
//@[0:98) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:33)  IdentifierSyntax
//@[7:33)   Identifier |tenantModuleDuplicateName3|
//@[34:56)  StringSyntax
//@[34:56)   StringComplete |'modules/tenant.bicep'|
//@[57:58)  Assignment |=|
//@[59:98)  ObjectSyntax
//@[59:60)   LeftBrace |{|
//@[60:61)   NewLine |\n|
  name: 'tenantModuleDuplicateName'
//@[2:35)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:35)    StringSyntax
//@[8:35)     StringComplete |'tenantModuleDuplicateName'|
//@[35:36)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

module managementGroupModuleDuplicateName1 'modules/managementGroup.bicep' = {
//@[0:156) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:42)  IdentifierSyntax
//@[7:42)   Identifier |managementGroupModuleDuplicateName1|
//@[43:74)  StringSyntax
//@[43:74)   StringComplete |'modules/managementGroup.bicep'|
//@[75:76)  Assignment |=|
//@[77:156)  ObjectSyntax
//@[77:78)   LeftBrace |{|
//@[78:79)   NewLine |\n|
  name: 'managementGroupModuleDuplicateName'
//@[2:44)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:44)    StringSyntax
//@[8:44)     StringComplete |'managementGroupModuleDuplicateName'|
//@[44:45)   NewLine |\n|
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

module managementGroupModuleDuplicateName2 'modules/managementGroup.bicep' = {
//@[0:156) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:42)  IdentifierSyntax
//@[7:42)   Identifier |managementGroupModuleDuplicateName2|
//@[43:74)  StringSyntax
//@[43:74)   StringComplete |'modules/managementGroup.bicep'|
//@[75:76)  Assignment |=|
//@[77:156)  ObjectSyntax
//@[77:78)   LeftBrace |{|
//@[78:79)   NewLine |\n|
  name: 'managementGroupModuleDuplicateName'
//@[2:44)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:44)    StringSyntax
//@[8:44)     StringComplete |'managementGroupModuleDuplicateName'|
//@[44:45)   NewLine |\n|
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

module subscriptionModuleDuplicateName1 'modules/subscription.bicep' = {
//@[0:178) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:39)  IdentifierSyntax
//@[7:39)   Identifier |subscriptionModuleDuplicateName1|
//@[40:68)  StringSyntax
//@[40:68)   StringComplete |'modules/subscription.bicep'|
//@[69:70)  Assignment |=|
//@[71:178)  ObjectSyntax
//@[71:72)   LeftBrace |{|
//@[72:73)   NewLine |\n|
  name: 'subscriptionModuleDuplicateName'
//@[2:41)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:41)    StringSyntax
//@[8:41)     StringComplete |'subscriptionModuleDuplicateName'|
//@[41:42)   NewLine |\n|
  scope: subscription('1ad827ac-2669-4c2f-9970-282b93c3c550')
//@[2:61)   ObjectPropertySyntax
//@[2:7)    IdentifierSyntax
//@[2:7)     Identifier |scope|
//@[7:8)    Colon |:|
//@[9:61)    FunctionCallSyntax
//@[9:21)     IdentifierSyntax
//@[9:21)      Identifier |subscription|
//@[21:22)     LeftParen |(|
//@[22:60)     FunctionArgumentSyntax
//@[22:60)      StringSyntax
//@[22:60)       StringComplete |'1ad827ac-2669-4c2f-9970-282b93c3c550'|
//@[60:61)     RightParen |)|
//@[61:62)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

module subscriptionModuleDuplicateName2 'modules/subscription.bicep' = {
//@[0:178) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:39)  IdentifierSyntax
//@[7:39)   Identifier |subscriptionModuleDuplicateName2|
//@[40:68)  StringSyntax
//@[40:68)   StringComplete |'modules/subscription.bicep'|
//@[69:70)  Assignment |=|
//@[71:178)  ObjectSyntax
//@[71:72)   LeftBrace |{|
//@[72:73)   NewLine |\n|
  name: 'subscriptionModuleDuplicateName'
//@[2:41)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:41)    StringSyntax
//@[8:41)     StringComplete |'subscriptionModuleDuplicateName'|
//@[41:42)   NewLine |\n|
  scope: subscription('1ad827ac-2669-4c2f-9970-282b93c3c550')
//@[2:61)   ObjectPropertySyntax
//@[2:7)    IdentifierSyntax
//@[2:7)     Identifier |scope|
//@[7:8)    Colon |:|
//@[9:61)    FunctionCallSyntax
//@[9:21)     IdentifierSyntax
//@[9:21)      Identifier |subscription|
//@[21:22)     LeftParen |(|
//@[22:60)     FunctionArgumentSyntax
//@[22:60)      StringSyntax
//@[22:60)       StringComplete |'1ad827ac-2669-4c2f-9970-282b93c3c550'|
//@[60:61)     RightParen |)|
//@[61:62)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:2) NewLine |\n|

//@[0:0) EndOfFile ||
