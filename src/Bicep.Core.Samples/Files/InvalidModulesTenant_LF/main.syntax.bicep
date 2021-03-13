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
//@[1:3) NewLine |\n\n|

module managementGroupModules 'modules/managementGroup.bicep' = [for (mg, i) in []: {
//@[0:137) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:29)  IdentifierSyntax
//@[7:29)   Identifier |managementGroupModules|
//@[30:61)  StringSyntax
//@[30:61)   StringComplete |'modules/managementGroup.bicep'|
//@[62:63)  Assignment |=|
//@[64:137)  ForSyntax
//@[64:65)   LeftSquare |[|
//@[65:68)   Identifier |for|
//@[69:76)   ForVariableBlockSyntax
//@[69:70)    LeftParen |(|
//@[70:72)    LocalVariableSyntax
//@[70:72)     IdentifierSyntax
//@[70:72)      Identifier |mg|
//@[72:73)    Comma |,|
//@[74:75)    LocalVariableSyntax
//@[74:75)     IdentifierSyntax
//@[74:75)      Identifier |i|
//@[75:76)    RightParen |)|
//@[77:79)   Identifier |in|
//@[80:82)   ArraySyntax
//@[80:81)    LeftSquare |[|
//@[81:82)    RightSquare |]|
//@[82:83)   Colon |:|
//@[84:136)   ObjectSyntax
//@[84:85)    LeftBrace |{|
//@[85:86)    NewLine |\n|
  name: 'dep-${mg}'
//@[2:19)    ObjectPropertySyntax
//@[2:6)     IdentifierSyntax
//@[2:6)      Identifier |name|
//@[6:7)     Colon |:|
//@[8:19)     StringSyntax
//@[8:15)      StringLeftPiece |'dep-${|
//@[15:17)      VariableAccessSyntax
//@[15:17)       IdentifierSyntax
//@[15:17)        Identifier |mg|
//@[17:19)      StringRightPiece |}'|
//@[19:20)    NewLine |\n|
  scope: managementGroup(mg)
//@[2:28)    ObjectPropertySyntax
//@[2:7)     IdentifierSyntax
//@[2:7)      Identifier |scope|
//@[7:8)     Colon |:|
//@[9:28)     FunctionCallSyntax
//@[9:24)      IdentifierSyntax
//@[9:24)       Identifier |managementGroup|
//@[24:25)      LeftParen |(|
//@[25:27)      FunctionArgumentSyntax
//@[25:27)       VariableAccessSyntax
//@[25:27)        IdentifierSyntax
//@[25:27)         Identifier |mg|
//@[27:28)      RightParen |)|
//@[28:29)    NewLine |\n|
}]
//@[0:1)    RightBrace |}|
//@[1:2)   RightSquare |]|
//@[2:4) NewLine |\n\n|

module cannotUseModuleCollectionAsScope 'modules/managementGroup.bicep' = [for (mg, i) in []: {
//@[0:150) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:39)  IdentifierSyntax
//@[7:39)   Identifier |cannotUseModuleCollectionAsScope|
//@[40:71)  StringSyntax
//@[40:71)   StringComplete |'modules/managementGroup.bicep'|
//@[72:73)  Assignment |=|
//@[74:150)  ForSyntax
//@[74:75)   LeftSquare |[|
//@[75:78)   Identifier |for|
//@[79:86)   ForVariableBlockSyntax
//@[79:80)    LeftParen |(|
//@[80:82)    LocalVariableSyntax
//@[80:82)     IdentifierSyntax
//@[80:82)      Identifier |mg|
//@[82:83)    Comma |,|
//@[84:85)    LocalVariableSyntax
//@[84:85)     IdentifierSyntax
//@[84:85)      Identifier |i|
//@[85:86)    RightParen |)|
//@[87:89)   Identifier |in|
//@[90:92)   ArraySyntax
//@[90:91)    LeftSquare |[|
//@[91:92)    RightSquare |]|
//@[92:93)   Colon |:|
//@[94:149)   ObjectSyntax
//@[94:95)    LeftBrace |{|
//@[95:96)    NewLine |\n|
  name: 'dep-${mg}'
//@[2:19)    ObjectPropertySyntax
//@[2:6)     IdentifierSyntax
//@[2:6)      Identifier |name|
//@[6:7)     Colon |:|
//@[8:19)     StringSyntax
//@[8:15)      StringLeftPiece |'dep-${|
//@[15:17)      VariableAccessSyntax
//@[15:17)       IdentifierSyntax
//@[15:17)        Identifier |mg|
//@[17:19)      StringRightPiece |}'|
//@[19:20)    NewLine |\n|
  scope: managementGroupModules
//@[2:31)    ObjectPropertySyntax
//@[2:7)     IdentifierSyntax
//@[2:7)      Identifier |scope|
//@[7:8)     Colon |:|
//@[9:31)     VariableAccessSyntax
//@[9:31)      IdentifierSyntax
//@[9:31)       Identifier |managementGroupModules|
//@[31:32)    NewLine |\n|
}]
//@[0:1)    RightBrace |}|
//@[1:2)   RightSquare |]|
//@[2:4) NewLine |\n\n|

module cannotUseSingleModuleAsScope 'modules/managementGroup.bicep' = [for (mg, i) in []: {
//@[0:149) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:35)  IdentifierSyntax
//@[7:35)   Identifier |cannotUseSingleModuleAsScope|
//@[36:67)  StringSyntax
//@[36:67)   StringComplete |'modules/managementGroup.bicep'|
//@[68:69)  Assignment |=|
//@[70:149)  ForSyntax
//@[70:71)   LeftSquare |[|
//@[71:74)   Identifier |for|
//@[75:82)   ForVariableBlockSyntax
//@[75:76)    LeftParen |(|
//@[76:78)    LocalVariableSyntax
//@[76:78)     IdentifierSyntax
//@[76:78)      Identifier |mg|
//@[78:79)    Comma |,|
//@[80:81)    LocalVariableSyntax
//@[80:81)     IdentifierSyntax
//@[80:81)      Identifier |i|
//@[81:82)    RightParen |)|
//@[83:85)   Identifier |in|
//@[86:88)   ArraySyntax
//@[86:87)    LeftSquare |[|
//@[87:88)    RightSquare |]|
//@[88:89)   Colon |:|
//@[90:148)   ObjectSyntax
//@[90:91)    LeftBrace |{|
//@[91:92)    NewLine |\n|
  name: 'dep-${mg}'
//@[2:19)    ObjectPropertySyntax
//@[2:6)     IdentifierSyntax
//@[2:6)      Identifier |name|
//@[6:7)     Colon |:|
//@[8:19)     StringSyntax
//@[8:15)      StringLeftPiece |'dep-${|
//@[15:17)      VariableAccessSyntax
//@[15:17)       IdentifierSyntax
//@[15:17)        Identifier |mg|
//@[17:19)      StringRightPiece |}'|
//@[19:20)    NewLine |\n|
  scope: managementGroupModules[i]
//@[2:34)    ObjectPropertySyntax
//@[2:7)     IdentifierSyntax
//@[2:7)      Identifier |scope|
//@[7:8)     Colon |:|
//@[9:34)     ArrayAccessSyntax
//@[9:31)      VariableAccessSyntax
//@[9:31)       IdentifierSyntax
//@[9:31)        Identifier |managementGroupModules|
//@[31:32)      LeftSquare |[|
//@[32:33)      VariableAccessSyntax
//@[32:33)       IdentifierSyntax
//@[32:33)        Identifier |i|
//@[33:34)      RightSquare |]|
//@[34:35)    NewLine |\n|
}]
//@[0:1)    RightBrace |}|
//@[1:2)   RightSquare |]|
//@[2:4) NewLine |\n\n|

module cannotUseSingleModuleAsScope2 'modules/managementGroup.bicep' = {
//@[0:134) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:36)  IdentifierSyntax
//@[7:36)   Identifier |cannotUseSingleModuleAsScope2|
//@[37:68)  StringSyntax
//@[37:68)   StringComplete |'modules/managementGroup.bicep'|
//@[69:70)  Assignment |=|
//@[71:134)  ObjectSyntax
//@[71:72)   LeftBrace |{|
//@[72:73)   NewLine |\n|
  name: 'test'
//@[2:14)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:14)    StringSyntax
//@[8:14)     StringComplete |'test'|
//@[14:15)   NewLine |\n|
  scope: managementGroupModuleDuplicateName1
//@[2:44)   ObjectPropertySyntax
//@[2:7)    IdentifierSyntax
//@[2:7)     Identifier |scope|
//@[7:8)    Colon |:|
//@[9:44)    VariableAccessSyntax
//@[9:44)     IdentifierSyntax
//@[9:44)      Identifier |managementGroupModuleDuplicateName1|
//@[44:45)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:2) NewLine |\n|

//@[0:0) EndOfFile ||
