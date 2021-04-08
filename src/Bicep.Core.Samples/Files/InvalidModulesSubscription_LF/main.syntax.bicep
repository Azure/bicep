targetScope = 'subscription'
//@[0:28) TargetScopeSyntax
//@[0:11)  Identifier |targetScope|
//@[12:13)  Assignment |=|
//@[14:28)  StringSyntax
//@[14:28)   StringComplete |'subscription'|
//@[28:30) NewLine |\n\n|

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
  scope: subscription('ced92236-c4d9-46ab-a299-a59c387fd1ee')
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
//@[22:60)       StringComplete |'ced92236-c4d9-46ab-a299-a59c387fd1ee'|
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
  scope: subscription('ced92236-c4d9-46ab-a299-a59c387fd1ee')
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
//@[22:60)       StringComplete |'ced92236-c4d9-46ab-a299-a59c387fd1ee'|
//@[60:61)     RightParen |)|
//@[61:62)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

module subscriptionModuleDuplicateName3 'modules/subscription.bicep' = {
//@[0:140) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:39)  IdentifierSyntax
//@[7:39)   Identifier |subscriptionModuleDuplicateName3|
//@[40:68)  StringSyntax
//@[40:68)   StringComplete |'modules/subscription.bicep'|
//@[69:70)  Assignment |=|
//@[71:140)  ObjectSyntax
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
  scope: subscription()
//@[2:23)   ObjectPropertySyntax
//@[2:7)    IdentifierSyntax
//@[2:7)     Identifier |scope|
//@[7:8)    Colon |:|
//@[9:23)    FunctionCallSyntax
//@[9:21)     IdentifierSyntax
//@[9:21)      Identifier |subscription|
//@[21:22)     LeftParen |(|
//@[22:23)     RightParen |)|
//@[23:24)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:2) NewLine |\n|
module subscriptionModuleDuplicateName4 'modules/subscription.bicep' = {
//@[0:140) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:39)  IdentifierSyntax
//@[7:39)   Identifier |subscriptionModuleDuplicateName4|
//@[40:68)  StringSyntax
//@[40:68)   StringComplete |'modules/subscription.bicep'|
//@[69:70)  Assignment |=|
//@[71:140)  ObjectSyntax
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
  scope: subscription()
//@[2:23)   ObjectPropertySyntax
//@[2:7)    IdentifierSyntax
//@[2:7)     Identifier |scope|
//@[7:8)    Colon |:|
//@[9:23)    FunctionCallSyntax
//@[9:21)     IdentifierSyntax
//@[9:21)      Identifier |subscription|
//@[21:22)     LeftParen |(|
//@[22:23)     RightParen |)|
//@[23:24)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:2) NewLine |\n|
module subscriptionModuleDuplicateName5 'modules/subscription.bicep' = {
//@[0:116) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:39)  IdentifierSyntax
//@[7:39)   Identifier |subscriptionModuleDuplicateName5|
//@[40:68)  StringSyntax
//@[40:68)   StringComplete |'modules/subscription.bicep'|
//@[69:70)  Assignment |=|
//@[71:116)  ObjectSyntax
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
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

module resourceGroupModuleDuplicateName1 'modules/resourceGroup.bicep' = {
//@[0:148) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:40)  IdentifierSyntax
//@[7:40)   Identifier |resourceGroupModuleDuplicateName1|
//@[41:70)  StringSyntax
//@[41:70)   StringComplete |'modules/resourceGroup.bicep'|
//@[71:72)  Assignment |=|
//@[73:148)  ObjectSyntax
//@[73:74)   LeftBrace |{|
//@[74:75)   NewLine |\n|
  name: 'resourceGroupModuleDuplicateName'
//@[2:42)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:42)    StringSyntax
//@[8:42)     StringComplete |'resourceGroupModuleDuplicateName'|
//@[42:43)   NewLine |\n|
  scope: resourceGroup('RG')
//@[2:28)   ObjectPropertySyntax
//@[2:7)    IdentifierSyntax
//@[2:7)     Identifier |scope|
//@[7:8)    Colon |:|
//@[9:28)    FunctionCallSyntax
//@[9:22)     IdentifierSyntax
//@[9:22)      Identifier |resourceGroup|
//@[22:23)     LeftParen |(|
//@[23:27)     FunctionArgumentSyntax
//@[23:27)      StringSyntax
//@[23:27)       StringComplete |'RG'|
//@[27:28)     RightParen |)|
//@[28:29)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:2) NewLine |\n|
module resourceGroupModuleDuplicateName2 'modules/resourceGroup.bicep' = {
//@[0:148) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:40)  IdentifierSyntax
//@[7:40)   Identifier |resourceGroupModuleDuplicateName2|
//@[41:70)  StringSyntax
//@[41:70)   StringComplete |'modules/resourceGroup.bicep'|
//@[71:72)  Assignment |=|
//@[73:148)  ObjectSyntax
//@[73:74)   LeftBrace |{|
//@[74:75)   NewLine |\n|
  name: 'resourceGroupModuleDuplicateName'
//@[2:42)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:42)    StringSyntax
//@[8:42)     StringComplete |'resourceGroupModuleDuplicateName'|
//@[42:43)   NewLine |\n|
  scope: resourceGroup('RG')
//@[2:28)   ObjectPropertySyntax
//@[2:7)    IdentifierSyntax
//@[2:7)     Identifier |scope|
//@[7:8)    Colon |:|
//@[9:28)    FunctionCallSyntax
//@[9:22)     IdentifierSyntax
//@[9:22)      Identifier |resourceGroup|
//@[22:23)     LeftParen |(|
//@[23:27)     FunctionArgumentSyntax
//@[23:27)      StringSyntax
//@[23:27)       StringComplete |'RG'|
//@[27:28)     RightParen |)|
//@[28:29)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

module unsupportedScopeManagementGroup 'modules/managementGroup.bicep' = {
//@[0:149) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:38)  IdentifierSyntax
//@[7:38)   Identifier |unsupportedScopeManagementGroup|
//@[39:70)  StringSyntax
//@[39:70)   StringComplete |'modules/managementGroup.bicep'|
//@[71:72)  Assignment |=|
//@[73:149)  ObjectSyntax
//@[73:74)   LeftBrace |{|
//@[74:75)   NewLine |\n|
  name: 'unsupportedScopeManagementGroup'
//@[2:41)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:41)    StringSyntax
//@[8:41)     StringComplete |'unsupportedScopeManagementGroup'|
//@[41:42)   NewLine |\n|
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

module singleRgModule 'modules/passthrough.bicep' = {
//@[0:143) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:21)  IdentifierSyntax
//@[7:21)   Identifier |singleRgModule|
//@[22:49)  StringSyntax
//@[22:49)   StringComplete |'modules/passthrough.bicep'|
//@[50:51)  Assignment |=|
//@[52:143)  ObjectSyntax
//@[52:53)   LeftBrace |{|
//@[53:54)   NewLine |\n|
  name: 'single-rg'
//@[2:19)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:19)    StringSyntax
//@[8:19)     StringComplete |'single-rg'|
//@[19:20)   NewLine |\n|
  params: {
//@[2:36)   ObjectPropertySyntax
//@[2:8)    IdentifierSyntax
//@[2:8)     Identifier |params|
//@[8:9)    Colon |:|
//@[10:36)    ObjectSyntax
//@[10:11)     LeftBrace |{|
//@[11:12)     NewLine |\n|
    myInput: 'stuff'
//@[4:20)     ObjectPropertySyntax
//@[4:11)      IdentifierSyntax
//@[4:11)       Identifier |myInput|
//@[11:12)      Colon |:|
//@[13:20)      StringSyntax
//@[13:20)       StringComplete |'stuff'|
//@[20:21)     NewLine |\n|
  }
//@[2:3)     RightBrace |}|
//@[3:4)   NewLine |\n|
  scope: resourceGroup('test')
//@[2:30)   ObjectPropertySyntax
//@[2:7)    IdentifierSyntax
//@[2:7)     Identifier |scope|
//@[7:8)    Colon |:|
//@[9:30)    FunctionCallSyntax
//@[9:22)     IdentifierSyntax
//@[9:22)      Identifier |resourceGroup|
//@[22:23)     LeftParen |(|
//@[23:29)     FunctionArgumentSyntax
//@[23:29)      StringSyntax
//@[23:29)       StringComplete |'test'|
//@[29:30)     RightParen |)|
//@[30:31)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

module singleRgModule2 'modules/passthrough.bicep' = {
//@[0:138) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:22)  IdentifierSyntax
//@[7:22)   Identifier |singleRgModule2|
//@[23:50)  StringSyntax
//@[23:50)   StringComplete |'modules/passthrough.bicep'|
//@[51:52)  Assignment |=|
//@[53:138)  ObjectSyntax
//@[53:54)   LeftBrace |{|
//@[54:55)   NewLine |\n|
  name: 'single-rg2'
//@[2:20)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:20)    StringSyntax
//@[8:20)     StringComplete |'single-rg2'|
//@[20:21)   NewLine |\n|
  params: {
//@[2:36)   ObjectPropertySyntax
//@[2:8)    IdentifierSyntax
//@[2:8)     Identifier |params|
//@[8:9)    Colon |:|
//@[10:36)    ObjectSyntax
//@[10:11)     LeftBrace |{|
//@[11:12)     NewLine |\n|
    myInput: 'stuff'
//@[4:20)     ObjectPropertySyntax
//@[4:11)      IdentifierSyntax
//@[4:11)       Identifier |myInput|
//@[11:12)      Colon |:|
//@[13:20)      StringSyntax
//@[13:20)       StringComplete |'stuff'|
//@[20:21)     NewLine |\n|
  }
//@[2:3)     RightBrace |}|
//@[3:4)   NewLine |\n|
  scope: singleRgModule
//@[2:23)   ObjectPropertySyntax
//@[2:7)    IdentifierSyntax
//@[2:7)     Identifier |scope|
//@[7:8)    Colon |:|
//@[9:23)    VariableAccessSyntax
//@[9:23)     IdentifierSyntax
//@[9:23)      Identifier |singleRgModule|
//@[23:24)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:2) NewLine |\n|

//@[0:0) EndOfFile ||
