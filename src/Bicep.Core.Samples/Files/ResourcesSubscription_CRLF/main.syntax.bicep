targetScope = 'subscription'
//@[0:28) TargetScopeSyntax
//@[0:11)  Identifier |targetScope|
//@[12:13)  Assignment |=|
//@[14:28)  StringSyntax
//@[14:28)   StringComplete |'subscription'|
//@[28:32) NewLine |\r\n\r\n|

param ownerPrincipalId string
//@[0:29) ParameterDeclarationSyntax
//@[0:5)  Identifier |param|
//@[6:22)  IdentifierSyntax
//@[6:22)   Identifier |ownerPrincipalId|
//@[23:29)  TypeSyntax
//@[23:29)   Identifier |string|
//@[29:33) NewLine |\r\n\r\n|

param contributorPrincipals array
//@[0:33) ParameterDeclarationSyntax
//@[0:5)  Identifier |param|
//@[6:27)  IdentifierSyntax
//@[6:27)   Identifier |contributorPrincipals|
//@[28:33)  TypeSyntax
//@[28:33)   Identifier |array|
//@[33:35) NewLine |\r\n|
param readerPrincipals array
//@[0:28) ParameterDeclarationSyntax
//@[0:5)  Identifier |param|
//@[6:22)  IdentifierSyntax
//@[6:22)   Identifier |readerPrincipals|
//@[23:28)  TypeSyntax
//@[23:28)   Identifier |array|
//@[28:32) NewLine |\r\n\r\n|

resource owner 'Microsoft.Authorization/roleAssignments@2020-04-01-preview' = {
//@[0:242) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:14)  IdentifierSyntax
//@[9:14)   Identifier |owner|
//@[15:75)  StringSyntax
//@[15:75)   StringComplete |'Microsoft.Authorization/roleAssignments@2020-04-01-preview'|
//@[76:77)  Assignment |=|
//@[78:242)  ObjectSyntax
//@[78:79)   LeftBrace |{|
//@[79:81)   NewLine |\r\n|
  name: guid('owner', ownerPrincipalId)
//@[2:39)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:39)    FunctionCallSyntax
//@[8:12)     IdentifierSyntax
//@[8:12)      Identifier |guid|
//@[12:13)     LeftParen |(|
//@[13:21)     FunctionArgumentSyntax
//@[13:20)      StringSyntax
//@[13:20)       StringComplete |'owner'|
//@[20:21)      Comma |,|
//@[22:38)     FunctionArgumentSyntax
//@[22:38)      VariableAccessSyntax
//@[22:38)       IdentifierSyntax
//@[22:38)        Identifier |ownerPrincipalId|
//@[38:39)     RightParen |)|
//@[39:41)   NewLine |\r\n|
  properties: {
//@[2:117)   ObjectPropertySyntax
//@[2:12)    IdentifierSyntax
//@[2:12)     Identifier |properties|
//@[12:13)    Colon |:|
//@[14:117)    ObjectSyntax
//@[14:15)     LeftBrace |{|
//@[15:17)     NewLine |\r\n|
    principalId: ownerPrincipalId
//@[4:33)     ObjectPropertySyntax
//@[4:15)      IdentifierSyntax
//@[4:15)       Identifier |principalId|
//@[15:16)      Colon |:|
//@[17:33)      VariableAccessSyntax
//@[17:33)       IdentifierSyntax
//@[17:33)        Identifier |ownerPrincipalId|
//@[33:35)     NewLine |\r\n|
    roleDefinitionId: '8e3af657-a8ff-443c-a75c-2fe8c4bcb635'
//@[4:60)     ObjectPropertySyntax
//@[4:20)      IdentifierSyntax
//@[4:20)       Identifier |roleDefinitionId|
//@[20:21)      Colon |:|
//@[22:60)      StringSyntax
//@[22:60)       StringComplete |'8e3af657-a8ff-443c-a75c-2fe8c4bcb635'|
//@[60:62)     NewLine |\r\n|
  }
//@[2:3)     RightBrace |}|
//@[3:5)   NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource contributors 'Microsoft.Authorization/roleAssignments@2020-04-01-preview' = [for contributor in contributorPrincipals: {
//@[0:321) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:21)  IdentifierSyntax
//@[9:21)   Identifier |contributors|
//@[22:82)  StringSyntax
//@[22:82)   StringComplete |'Microsoft.Authorization/roleAssignments@2020-04-01-preview'|
//@[83:84)  Assignment |=|
//@[85:321)  ForSyntax
//@[85:86)   LeftSquare |[|
//@[86:89)   Identifier |for|
//@[90:101)   LocalVariableSyntax
//@[90:101)    IdentifierSyntax
//@[90:101)     Identifier |contributor|
//@[102:104)   Identifier |in|
//@[105:126)   VariableAccessSyntax
//@[105:126)    IdentifierSyntax
//@[105:126)     Identifier |contributorPrincipals|
//@[126:127)   Colon |:|
//@[128:320)   ObjectSyntax
//@[128:129)    LeftBrace |{|
//@[129:131)    NewLine |\r\n|
  name: guid('contributor', contributor)
//@[2:40)    ObjectPropertySyntax
//@[2:6)     IdentifierSyntax
//@[2:6)      Identifier |name|
//@[6:7)     Colon |:|
//@[8:40)     FunctionCallSyntax
//@[8:12)      IdentifierSyntax
//@[8:12)       Identifier |guid|
//@[12:13)      LeftParen |(|
//@[13:27)      FunctionArgumentSyntax
//@[13:26)       StringSyntax
//@[13:26)        StringComplete |'contributor'|
//@[26:27)       Comma |,|
//@[28:39)      FunctionArgumentSyntax
//@[28:39)       VariableAccessSyntax
//@[28:39)        IdentifierSyntax
//@[28:39)         Identifier |contributor|
//@[39:40)      RightParen |)|
//@[40:42)    NewLine |\r\n|
  properties: {
//@[2:112)    ObjectPropertySyntax
//@[2:12)     IdentifierSyntax
//@[2:12)      Identifier |properties|
//@[12:13)     Colon |:|
//@[14:112)     ObjectSyntax
//@[14:15)      LeftBrace |{|
//@[15:17)      NewLine |\r\n|
    principalId: contributor
//@[4:28)      ObjectPropertySyntax
//@[4:15)       IdentifierSyntax
//@[4:15)        Identifier |principalId|
//@[15:16)       Colon |:|
//@[17:28)       VariableAccessSyntax
//@[17:28)        IdentifierSyntax
//@[17:28)         Identifier |contributor|
//@[28:30)      NewLine |\r\n|
    roleDefinitionId: 'b24988ac-6180-42a0-ab88-20f7382dd24c'
//@[4:60)      ObjectPropertySyntax
//@[4:20)       IdentifierSyntax
//@[4:20)        Identifier |roleDefinitionId|
//@[20:21)       Colon |:|
//@[22:60)       StringSyntax
//@[22:60)        StringComplete |'b24988ac-6180-42a0-ab88-20f7382dd24c'|
//@[60:62)      NewLine |\r\n|
  }
//@[2:3)      RightBrace |}|
//@[3:5)    NewLine |\r\n|
  dependsOn: [
//@[2:30)    ObjectPropertySyntax
//@[2:11)     IdentifierSyntax
//@[2:11)      Identifier |dependsOn|
//@[11:12)     Colon |:|
//@[13:30)     ArraySyntax
//@[13:14)      LeftSquare |[|
//@[14:16)      NewLine |\r\n|
    owner
//@[4:9)      ArrayItemSyntax
//@[4:9)       VariableAccessSyntax
//@[4:9)        IdentifierSyntax
//@[4:9)         Identifier |owner|
//@[9:11)      NewLine |\r\n|
  ]
//@[2:3)      RightSquare |]|
//@[3:5)    NewLine |\r\n|
}]
//@[0:1)    RightBrace |}|
//@[1:2)   RightSquare |]|
//@[2:6) NewLine |\r\n\r\n|

resource readers 'Microsoft.Authorization/roleAssignments@2020-04-01-preview' = [for reader in readerPrincipals: {
//@[0:312) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:16)  IdentifierSyntax
//@[9:16)   Identifier |readers|
//@[17:77)  StringSyntax
//@[17:77)   StringComplete |'Microsoft.Authorization/roleAssignments@2020-04-01-preview'|
//@[78:79)  Assignment |=|
//@[80:312)  ForSyntax
//@[80:81)   LeftSquare |[|
//@[81:84)   Identifier |for|
//@[85:91)   LocalVariableSyntax
//@[85:91)    IdentifierSyntax
//@[85:91)     Identifier |reader|
//@[92:94)   Identifier |in|
//@[95:111)   VariableAccessSyntax
//@[95:111)    IdentifierSyntax
//@[95:111)     Identifier |readerPrincipals|
//@[111:112)   Colon |:|
//@[113:311)   ObjectSyntax
//@[113:114)    LeftBrace |{|
//@[114:116)    NewLine |\r\n|
  name: guid('reader', reader)
//@[2:30)    ObjectPropertySyntax
//@[2:6)     IdentifierSyntax
//@[2:6)      Identifier |name|
//@[6:7)     Colon |:|
//@[8:30)     FunctionCallSyntax
//@[8:12)      IdentifierSyntax
//@[8:12)       Identifier |guid|
//@[12:13)      LeftParen |(|
//@[13:22)      FunctionArgumentSyntax
//@[13:21)       StringSyntax
//@[13:21)        StringComplete |'reader'|
//@[21:22)       Comma |,|
//@[23:29)      FunctionArgumentSyntax
//@[23:29)       VariableAccessSyntax
//@[23:29)        IdentifierSyntax
//@[23:29)         Identifier |reader|
//@[29:30)      RightParen |)|
//@[30:32)    NewLine |\r\n|
  properties: {
//@[2:107)    ObjectPropertySyntax
//@[2:12)     IdentifierSyntax
//@[2:12)      Identifier |properties|
//@[12:13)     Colon |:|
//@[14:107)     ObjectSyntax
//@[14:15)      LeftBrace |{|
//@[15:17)      NewLine |\r\n|
    principalId: reader
//@[4:23)      ObjectPropertySyntax
//@[4:15)       IdentifierSyntax
//@[4:15)        Identifier |principalId|
//@[15:16)       Colon |:|
//@[17:23)       VariableAccessSyntax
//@[17:23)        IdentifierSyntax
//@[17:23)         Identifier |reader|
//@[23:25)      NewLine |\r\n|
    roleDefinitionId: 'b24988ac-6180-42a0-ab88-20f7382dd24c'
//@[4:60)      ObjectPropertySyntax
//@[4:20)       IdentifierSyntax
//@[4:20)        Identifier |roleDefinitionId|
//@[20:21)       Colon |:|
//@[22:60)       StringSyntax
//@[22:60)        StringComplete |'b24988ac-6180-42a0-ab88-20f7382dd24c'|
//@[60:62)      NewLine |\r\n|
  }
//@[2:3)      RightBrace |}|
//@[3:5)    NewLine |\r\n|
  dependsOn: [
//@[2:51)    ObjectPropertySyntax
//@[2:11)     IdentifierSyntax
//@[2:11)      Identifier |dependsOn|
//@[11:12)     Colon |:|
//@[13:51)     ArraySyntax
//@[13:14)      LeftSquare |[|
//@[14:16)      NewLine |\r\n|
    owner
//@[4:9)      ArrayItemSyntax
//@[4:9)       VariableAccessSyntax
//@[4:9)        IdentifierSyntax
//@[4:9)         Identifier |owner|
//@[9:11)      NewLine |\r\n|
    contributors[0]
//@[4:19)      ArrayItemSyntax
//@[4:19)       ArrayAccessSyntax
//@[4:16)        VariableAccessSyntax
//@[4:16)         IdentifierSyntax
//@[4:16)          Identifier |contributors|
//@[16:17)        LeftSquare |[|
//@[17:18)        IntegerLiteralSyntax
//@[17:18)         Integer |0|
//@[18:19)        RightSquare |]|
//@[19:21)      NewLine |\r\n|
  ]
//@[2:3)      RightSquare |]|
//@[3:5)    NewLine |\r\n|
}]
//@[0:1)    RightBrace |}|
//@[1:2)   RightSquare |]|
//@[2:4) NewLine |\r\n|

//@[0:0) EndOfFile ||
