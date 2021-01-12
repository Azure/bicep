targetScope = 'managementGroup'
//@[0:31) TargetScopeSyntax
//@[0:11)  Identifier |targetScope|
//@[12:13)  Assignment |=|
//@[14:31)  StringSyntax
//@[14:31)   StringComplete |'managementGroup'|
//@[31:33) NewLine |\n\n|

module managementGroupModuleDuplicateName1 'modules/managementGroup.bicep' = {
//@[0:152) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:42)  IdentifierSyntax
//@[7:42)   Identifier |managementGroupModuleDuplicateName1|
//@[43:74)  StringSyntax
//@[43:74)   StringComplete |'modules/managementGroup.bicep'|
//@[75:76)  Assignment |=|
//@[77:152)  ObjectSyntax
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
  scope: managementGroup()
//@[2:26)   ObjectPropertySyntax
//@[2:7)    IdentifierSyntax
//@[2:7)     Identifier |scope|
//@[7:8)    Colon |:|
//@[9:26)    FunctionCallSyntax
//@[9:24)     IdentifierSyntax
//@[9:24)      Identifier |managementGroup|
//@[24:25)     LeftParen |(|
//@[25:26)     RightParen |)|
//@[26:27)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

module managementGroupModuleDuplicateName2 'modules/managementGroup.bicep' = {
//@[0:152) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:42)  IdentifierSyntax
//@[7:42)   Identifier |managementGroupModuleDuplicateName2|
//@[43:74)  StringSyntax
//@[43:74)   StringComplete |'modules/managementGroup.bicep'|
//@[75:76)  Assignment |=|
//@[77:152)  ObjectSyntax
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
  scope: managementGroup()
//@[2:26)   ObjectPropertySyntax
//@[2:7)    IdentifierSyntax
//@[2:7)     Identifier |scope|
//@[7:8)    Colon |:|
//@[9:26)    FunctionCallSyntax
//@[9:24)     IdentifierSyntax
//@[9:24)      Identifier |managementGroup|
//@[24:25)     LeftParen |(|
//@[25:26)     RightParen |)|
//@[26:27)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

module managementGroupModuleDuplicateName3 'modules/managementGroup.bicep' = {
//@[0:125) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:42)  IdentifierSyntax
//@[7:42)   Identifier |managementGroupModuleDuplicateName3|
//@[43:74)  StringSyntax
//@[43:74)   StringComplete |'modules/managementGroup.bicep'|
//@[75:76)  Assignment |=|
//@[77:125)  ObjectSyntax
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
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

module subscriptionModuleDuplicateName1 'modules/subscription.bicep' = {
//@[0:172) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:39)  IdentifierSyntax
//@[7:39)   Identifier |subscriptionModuleDuplicateName1|
//@[40:68)  StringSyntax
//@[40:68)   StringComplete |'modules/subscription.bicep'|
//@[69:70)  Assignment |=|
//@[71:172)  ObjectSyntax
//@[71:72)   LeftBrace |{|
//@[72:73)   NewLine |\n|
  name: 'subscriptionDuplicateName'
//@[2:35)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:35)    StringSyntax
//@[8:35)     StringComplete |'subscriptionDuplicateName'|
//@[35:36)   NewLine |\n|
  scope: subscription('c56ffff6-0806-4a98-83fb-17aed775d6e4')
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
//@[22:60)       StringComplete |'c56ffff6-0806-4a98-83fb-17aed775d6e4'|
//@[60:61)     RightParen |)|
//@[61:62)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

module subscriptionModuleDuplicateName2 'modules/subscription.bicep' = {
//@[0:172) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:39)  IdentifierSyntax
//@[7:39)   Identifier |subscriptionModuleDuplicateName2|
//@[40:68)  StringSyntax
//@[40:68)   StringComplete |'modules/subscription.bicep'|
//@[69:70)  Assignment |=|
//@[71:172)  ObjectSyntax
//@[71:72)   LeftBrace |{|
//@[72:73)   NewLine |\n|
  name: 'subscriptionDuplicateName'
//@[2:35)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:35)    StringSyntax
//@[8:35)     StringComplete |'subscriptionDuplicateName'|
//@[35:36)   NewLine |\n|
  scope: subscription('c56ffff6-0806-4a98-83fb-17aed775d6e4')
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
//@[22:60)       StringComplete |'c56ffff6-0806-4a98-83fb-17aed775d6e4'|
//@[60:61)     RightParen |)|
//@[61:62)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

module tenantModuleDuplicateName1 'modules/tenant.bicep' = {
//@[0:110) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:33)  IdentifierSyntax
//@[7:33)   Identifier |tenantModuleDuplicateName1|
//@[34:56)  StringSyntax
//@[34:56)   StringComplete |'modules/tenant.bicep'|
//@[57:58)  Assignment |=|
//@[59:110)  ObjectSyntax
//@[59:60)   LeftBrace |{|
//@[60:61)   NewLine |\n|
  name: 'tenantDuplicateName'
//@[2:29)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:29)    StringSyntax
//@[8:29)     StringComplete |'tenantDuplicateName'|
//@[29:30)   NewLine |\n|
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
//@[0:110) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:33)  IdentifierSyntax
//@[7:33)   Identifier |tenantModuleDuplicateName2|
//@[34:56)  StringSyntax
//@[34:56)   StringComplete |'modules/tenant.bicep'|
//@[57:58)  Assignment |=|
//@[59:110)  ObjectSyntax
//@[59:60)   LeftBrace |{|
//@[60:61)   NewLine |\n|
  name: 'tenantDuplicateName'
//@[2:29)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:29)    StringSyntax
//@[8:29)     StringComplete |'tenantDuplicateName'|
//@[29:30)   NewLine |\n|
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
//@[1:2) NewLine |\n|

//@[0:0) EndOfFile ||
