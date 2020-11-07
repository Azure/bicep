targetScope = 'tenant'
//@[0:22) TargetScopeSyntax
//@[0:11)  Identifier |targetScope|
//@[12:13)  Assignment |=|
//@[14:22)  StringSyntax
//@[14:22)   StringComplete |'tenant'|
//@[22:24) NewLine |\n\n|

module myManagementGroupMod 'modules/managementgroup.bicep' = {
//@[0:142) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:27)  IdentifierSyntax
//@[7:27)   Identifier |myManagementGroupMod|
//@[28:59)  StringSyntax
//@[28:59)   StringComplete |'modules/managementgroup.bicep'|
//@[60:61)  Assignment |=|
//@[62:142)  ObjectSyntax
//@[62:63)   LeftBrace |{|
//@[63:64)   NewLine |\n|
  name: 'myManagementGroupMod'
//@[2:30)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:30)    StringSyntax
//@[8:30)     StringComplete |'myManagementGroupMod'|
//@[30:31)   NewLine |\n|
  scope: managementGroup('myManagementGroup')
//@[2:45)   ObjectPropertySyntax
//@[2:7)    IdentifierSyntax
//@[2:7)     Identifier |scope|
//@[7:8)    Colon |:|
//@[9:45)    FunctionCallSyntax
//@[9:24)     IdentifierSyntax
//@[9:24)      Identifier |managementGroup|
//@[24:25)     LeftParen |(|
//@[25:44)     FunctionArgumentSyntax
//@[25:44)      StringSyntax
//@[25:44)       StringComplete |'myManagementGroup'|
//@[44:45)     RightParen |)|
//@[45:46)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

module mySubscriptionMod 'modules/subscription.bicep' = {
//@[0:149) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:24)  IdentifierSyntax
//@[7:24)   Identifier |mySubscriptionMod|
//@[25:53)  StringSyntax
//@[25:53)   StringComplete |'modules/subscription.bicep'|
//@[54:55)  Assignment |=|
//@[56:149)  ObjectSyntax
//@[56:57)   LeftBrace |{|
//@[57:58)   NewLine |\n|
  name: 'mySubscriptionMod'
//@[2:27)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:27)    StringSyntax
//@[8:27)     StringComplete |'mySubscriptionMod'|
//@[27:28)   NewLine |\n|
  scope: subscription('ee44cd78-68c6-43d9-874e-e684ec8d1191')
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
//@[22:60)       StringComplete |'ee44cd78-68c6-43d9-874e-e684ec8d1191'|
//@[60:61)     RightParen |)|
//@[61:62)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

output myManagementGroupOutput string = myManagementGroupMod.outputs.myOutput
//@[0:77) OutputDeclarationSyntax
//@[0:6)  Identifier |output|
//@[7:30)  IdentifierSyntax
//@[7:30)   Identifier |myManagementGroupOutput|
//@[31:37)  TypeSyntax
//@[31:37)   Identifier |string|
//@[38:39)  Assignment |=|
//@[40:77)  PropertyAccessSyntax
//@[40:68)   PropertyAccessSyntax
//@[40:60)    VariableAccessSyntax
//@[40:60)     IdentifierSyntax
//@[40:60)      Identifier |myManagementGroupMod|
//@[60:61)    Dot |.|
//@[61:68)    IdentifierSyntax
//@[61:68)     Identifier |outputs|
//@[68:69)   Dot |.|
//@[69:77)   IdentifierSyntax
//@[69:77)    Identifier |myOutput|
//@[77:78) NewLine |\n|
output mySubscriptionOutput string = mySubscriptionMod.outputs.myOutput
//@[0:71) OutputDeclarationSyntax
//@[0:6)  Identifier |output|
//@[7:27)  IdentifierSyntax
//@[7:27)   Identifier |mySubscriptionOutput|
//@[28:34)  TypeSyntax
//@[28:34)   Identifier |string|
//@[35:36)  Assignment |=|
//@[37:71)  PropertyAccessSyntax
//@[37:62)   PropertyAccessSyntax
//@[37:54)    VariableAccessSyntax
//@[37:54)     IdentifierSyntax
//@[37:54)      Identifier |mySubscriptionMod|
//@[54:55)    Dot |.|
//@[55:62)    IdentifierSyntax
//@[55:62)     Identifier |outputs|
//@[62:63)   Dot |.|
//@[63:71)   IdentifierSyntax
//@[63:71)    Identifier |myOutput|
//@[71:71) EndOfFile ||
