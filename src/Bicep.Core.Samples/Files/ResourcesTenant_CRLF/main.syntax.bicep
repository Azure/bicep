targetScope = 'tenant'
//@[0:22) TargetScopeSyntax
//@[0:11)  Identifier |targetScope|
//@[12:13)  Assignment |=|
//@[14:22)  StringSyntax
//@[14:22)   StringComplete |'tenant'|
//@[22:26) NewLine |\r\n\r\n|

var managementGroups = [
//@[0:142) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:20)  IdentifierSyntax
//@[4:20)   Identifier |managementGroups|
//@[21:22)  Assignment |=|
//@[23:142)  ArraySyntax
//@[23:24)   LeftSquare |[|
//@[24:26)   NewLine |\r\n|
  {
//@[2:55)   ArrayItemSyntax
//@[2:55)    ObjectSyntax
//@[2:3)     LeftBrace |{|
//@[3:5)     NewLine |\r\n|
    name: 'one'
//@[4:15)     ObjectPropertySyntax
//@[4:8)      IdentifierSyntax
//@[4:8)       Identifier |name|
//@[8:9)      Colon |:|
//@[10:15)      StringSyntax
//@[10:15)       StringComplete |'one'|
//@[15:17)     NewLine |\r\n|
    displayName: 'The first'
//@[4:28)     ObjectPropertySyntax
//@[4:15)      IdentifierSyntax
//@[4:15)       Identifier |displayName|
//@[15:16)      Colon |:|
//@[17:28)      StringSyntax
//@[17:28)       StringComplete |'The first'|
//@[28:30)     NewLine |\r\n|
  }
//@[2:3)     RightBrace |}|
//@[3:5)   NewLine |\r\n|
  {
//@[2:56)   ArrayItemSyntax
//@[2:56)    ObjectSyntax
//@[2:3)     LeftBrace |{|
//@[3:5)     NewLine |\r\n|
    name: 'two'
//@[4:15)     ObjectPropertySyntax
//@[4:8)      IdentifierSyntax
//@[4:8)       Identifier |name|
//@[8:9)      Colon |:|
//@[10:15)      StringSyntax
//@[10:15)       StringComplete |'two'|
//@[15:17)     NewLine |\r\n|
    displayName: 'The second'
//@[4:29)     ObjectPropertySyntax
//@[4:15)      IdentifierSyntax
//@[4:15)       Identifier |displayName|
//@[15:16)      Colon |:|
//@[17:29)      StringSyntax
//@[17:29)       StringComplete |'The second'|
//@[29:31)     NewLine |\r\n|
  }
//@[2:3)     RightBrace |}|
//@[3:5)   NewLine |\r\n|
]
//@[0:1)   RightSquare |]|
//@[1:5) NewLine |\r\n\r\n|

resource singleGroup 'Microsoft.Management/managementGroups@2020-05-01' = {
//@[0:154) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:20)  IdentifierSyntax
//@[9:20)   Identifier |singleGroup|
//@[21:71)  StringSyntax
//@[21:71)   StringComplete |'Microsoft.Management/managementGroups@2020-05-01'|
//@[72:73)  Assignment |=|
//@[74:154)  ObjectSyntax
//@[74:75)   LeftBrace |{|
//@[75:77)   NewLine |\r\n|
  name: 'myMG'
//@[2:14)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:14)    StringSyntax
//@[8:14)     StringComplete |'myMG'|
//@[14:16)   NewLine |\r\n|
  properties: {
//@[2:58)   ObjectPropertySyntax
//@[2:12)    IdentifierSyntax
//@[2:12)     Identifier |properties|
//@[12:13)    Colon |:|
//@[14:58)    ObjectSyntax
//@[14:15)     LeftBrace |{|
//@[15:17)     NewLine |\r\n|
    displayName: 'This one is mine!'
//@[4:36)     ObjectPropertySyntax
//@[4:15)      IdentifierSyntax
//@[4:15)       Identifier |displayName|
//@[15:16)      Colon |:|
//@[17:36)      StringSyntax
//@[17:36)       StringComplete |'This one is mine!'|
//@[36:38)     NewLine |\r\n|
  }
//@[2:3)     RightBrace |}|
//@[3:5)   NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource manyGroups 'Microsoft.Management/managementGroups@2020-05-01' = [for mg in managementGroups: {
//@[0:224) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:19)  IdentifierSyntax
//@[9:19)   Identifier |manyGroups|
//@[20:70)  StringSyntax
//@[20:70)   StringComplete |'Microsoft.Management/managementGroups@2020-05-01'|
//@[71:72)  Assignment |=|
//@[73:224)  ForSyntax
//@[73:74)   LeftSquare |[|
//@[74:77)   Identifier |for|
//@[78:80)   LocalVariableSyntax
//@[78:80)    IdentifierSyntax
//@[78:80)     Identifier |mg|
//@[81:83)   Identifier |in|
//@[84:100)   VariableAccessSyntax
//@[84:100)    IdentifierSyntax
//@[84:100)     Identifier |managementGroups|
//@[100:101)   Colon |:|
//@[102:223)   ObjectSyntax
//@[102:103)    LeftBrace |{|
//@[103:105)    NewLine |\r\n|
  name: mg.name
//@[2:15)    ObjectPropertySyntax
//@[2:6)     IdentifierSyntax
//@[2:6)      Identifier |name|
//@[6:7)     Colon |:|
//@[8:15)     PropertyAccessSyntax
//@[8:10)      VariableAccessSyntax
//@[8:10)       IdentifierSyntax
//@[8:10)        Identifier |mg|
//@[10:11)      Dot |.|
//@[11:15)      IdentifierSyntax
//@[11:15)       Identifier |name|
//@[15:17)    NewLine |\r\n|
  properties: {
//@[2:98)    ObjectPropertySyntax
//@[2:12)     IdentifierSyntax
//@[2:12)      Identifier |properties|
//@[12:13)     Colon |:|
//@[14:98)     ObjectSyntax
//@[14:15)      LeftBrace |{|
//@[15:17)      NewLine |\r\n|
    displayName: '${mg.displayName} (${singleGroup.properties.displayName})'
//@[4:76)      ObjectPropertySyntax
//@[4:15)       IdentifierSyntax
//@[4:15)        Identifier |displayName|
//@[15:16)       Colon |:|
//@[17:76)       StringSyntax
//@[17:20)        StringLeftPiece |'${|
//@[20:34)        PropertyAccessSyntax
//@[20:22)         VariableAccessSyntax
//@[20:22)          IdentifierSyntax
//@[20:22)           Identifier |mg|
//@[22:23)         Dot |.|
//@[23:34)         IdentifierSyntax
//@[23:34)          Identifier |displayName|
//@[34:39)        StringMiddlePiece |} (${|
//@[39:73)        PropertyAccessSyntax
//@[39:61)         PropertyAccessSyntax
//@[39:50)          VariableAccessSyntax
//@[39:50)           IdentifierSyntax
//@[39:50)            Identifier |singleGroup|
//@[50:51)          Dot |.|
//@[51:61)          IdentifierSyntax
//@[51:61)           Identifier |properties|
//@[61:62)         Dot |.|
//@[62:73)         IdentifierSyntax
//@[62:73)          Identifier |displayName|
//@[73:76)        StringRightPiece |})'|
//@[76:78)      NewLine |\r\n|
  }
//@[2:3)      RightBrace |}|
//@[3:5)    NewLine |\r\n|
}]
//@[0:1)    RightBrace |}|
//@[1:2)   RightSquare |]|
//@[2:6) NewLine |\r\n\r\n|

resource anotherSet 'Microsoft.Management/managementGroups@2020-05-01' = [for mg in managementGroups: {
//@[0:285) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:19)  IdentifierSyntax
//@[9:19)   Identifier |anotherSet|
//@[20:70)  StringSyntax
//@[20:70)   StringComplete |'Microsoft.Management/managementGroups@2020-05-01'|
//@[71:72)  Assignment |=|
//@[73:285)  ForSyntax
//@[73:74)   LeftSquare |[|
//@[74:77)   Identifier |for|
//@[78:80)   LocalVariableSyntax
//@[78:80)    IdentifierSyntax
//@[78:80)     Identifier |mg|
//@[81:83)   Identifier |in|
//@[84:100)   VariableAccessSyntax
//@[84:100)    IdentifierSyntax
//@[84:100)     Identifier |managementGroups|
//@[100:101)   Colon |:|
//@[102:284)   ObjectSyntax
//@[102:103)    LeftBrace |{|
//@[103:105)    NewLine |\r\n|
  name: concat(mg.name, '-one')
//@[2:31)    ObjectPropertySyntax
//@[2:6)     IdentifierSyntax
//@[2:6)      Identifier |name|
//@[6:7)     Colon |:|
//@[8:31)     FunctionCallSyntax
//@[8:14)      IdentifierSyntax
//@[8:14)       Identifier |concat|
//@[14:15)      LeftParen |(|
//@[15:23)      FunctionArgumentSyntax
//@[15:22)       PropertyAccessSyntax
//@[15:17)        VariableAccessSyntax
//@[15:17)         IdentifierSyntax
//@[15:17)          Identifier |mg|
//@[17:18)        Dot |.|
//@[18:22)        IdentifierSyntax
//@[18:22)         Identifier |name|
//@[22:23)       Comma |,|
//@[24:30)      FunctionArgumentSyntax
//@[24:30)       StringSyntax
//@[24:30)        StringComplete |'-one'|
//@[30:31)      RightParen |)|
//@[31:33)    NewLine |\r\n|
  properties: {
//@[2:106)    ObjectPropertySyntax
//@[2:12)     IdentifierSyntax
//@[2:12)      Identifier |properties|
//@[12:13)     Colon |:|
//@[14:106)     ObjectSyntax
//@[14:15)      LeftBrace |{|
//@[15:17)      NewLine |\r\n|
    displayName: '${mg.displayName} (${singleGroup.properties.displayName}) (set 1)'
//@[4:84)      ObjectPropertySyntax
//@[4:15)       IdentifierSyntax
//@[4:15)        Identifier |displayName|
//@[15:16)       Colon |:|
//@[17:84)       StringSyntax
//@[17:20)        StringLeftPiece |'${|
//@[20:34)        PropertyAccessSyntax
//@[20:22)         VariableAccessSyntax
//@[20:22)          IdentifierSyntax
//@[20:22)           Identifier |mg|
//@[22:23)         Dot |.|
//@[23:34)         IdentifierSyntax
//@[23:34)          Identifier |displayName|
//@[34:39)        StringMiddlePiece |} (${|
//@[39:73)        PropertyAccessSyntax
//@[39:61)         PropertyAccessSyntax
//@[39:50)          VariableAccessSyntax
//@[39:50)           IdentifierSyntax
//@[39:50)            Identifier |singleGroup|
//@[50:51)          Dot |.|
//@[51:61)          IdentifierSyntax
//@[51:61)           Identifier |properties|
//@[61:62)         Dot |.|
//@[62:73)         IdentifierSyntax
//@[62:73)          Identifier |displayName|
//@[73:84)        StringRightPiece |}) (set 1)'|
//@[84:86)      NewLine |\r\n|
  }
//@[2:3)      RightBrace |}|
//@[3:5)    NewLine |\r\n|
  dependsOn: [
//@[2:35)    ObjectPropertySyntax
//@[2:11)     IdentifierSyntax
//@[2:11)      Identifier |dependsOn|
//@[11:12)     Colon |:|
//@[13:35)     ArraySyntax
//@[13:14)      LeftSquare |[|
//@[14:16)      NewLine |\r\n|
    manyGroups
//@[4:14)      ArrayItemSyntax
//@[4:14)       VariableAccessSyntax
//@[4:14)        IdentifierSyntax
//@[4:14)         Identifier |manyGroups|
//@[14:16)      NewLine |\r\n|
  ]
//@[2:3)      RightSquare |]|
//@[3:5)    NewLine |\r\n|
}]
//@[0:1)    RightBrace |}|
//@[1:2)   RightSquare |]|
//@[2:6) NewLine |\r\n\r\n|

resource yetAnotherSet 'Microsoft.Management/managementGroups@2020-05-01' = [for mg in managementGroups: {
//@[0:291) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:22)  IdentifierSyntax
//@[9:22)   Identifier |yetAnotherSet|
//@[23:73)  StringSyntax
//@[23:73)   StringComplete |'Microsoft.Management/managementGroups@2020-05-01'|
//@[74:75)  Assignment |=|
//@[76:291)  ForSyntax
//@[76:77)   LeftSquare |[|
//@[77:80)   Identifier |for|
//@[81:83)   LocalVariableSyntax
//@[81:83)    IdentifierSyntax
//@[81:83)     Identifier |mg|
//@[84:86)   Identifier |in|
//@[87:103)   VariableAccessSyntax
//@[87:103)    IdentifierSyntax
//@[87:103)     Identifier |managementGroups|
//@[103:104)   Colon |:|
//@[105:290)   ObjectSyntax
//@[105:106)    LeftBrace |{|
//@[106:108)    NewLine |\r\n|
  name: concat(mg.name, '-two')
//@[2:31)    ObjectPropertySyntax
//@[2:6)     IdentifierSyntax
//@[2:6)      Identifier |name|
//@[6:7)     Colon |:|
//@[8:31)     FunctionCallSyntax
//@[8:14)      IdentifierSyntax
//@[8:14)       Identifier |concat|
//@[14:15)      LeftParen |(|
//@[15:23)      FunctionArgumentSyntax
//@[15:22)       PropertyAccessSyntax
//@[15:17)        VariableAccessSyntax
//@[15:17)         IdentifierSyntax
//@[15:17)          Identifier |mg|
//@[17:18)        Dot |.|
//@[18:22)        IdentifierSyntax
//@[18:22)         Identifier |name|
//@[22:23)       Comma |,|
//@[24:30)      FunctionArgumentSyntax
//@[24:30)       StringSyntax
//@[24:30)        StringComplete |'-two'|
//@[30:31)      RightParen |)|
//@[31:33)    NewLine |\r\n|
  properties: {
//@[2:106)    ObjectPropertySyntax
//@[2:12)     IdentifierSyntax
//@[2:12)      Identifier |properties|
//@[12:13)     Colon |:|
//@[14:106)     ObjectSyntax
//@[14:15)      LeftBrace |{|
//@[15:17)      NewLine |\r\n|
    displayName: '${mg.displayName} (${singleGroup.properties.displayName}) (set 2)'
//@[4:84)      ObjectPropertySyntax
//@[4:15)       IdentifierSyntax
//@[4:15)        Identifier |displayName|
//@[15:16)       Colon |:|
//@[17:84)       StringSyntax
//@[17:20)        StringLeftPiece |'${|
//@[20:34)        PropertyAccessSyntax
//@[20:22)         VariableAccessSyntax
//@[20:22)          IdentifierSyntax
//@[20:22)           Identifier |mg|
//@[22:23)         Dot |.|
//@[23:34)         IdentifierSyntax
//@[23:34)          Identifier |displayName|
//@[34:39)        StringMiddlePiece |} (${|
//@[39:73)        PropertyAccessSyntax
//@[39:61)         PropertyAccessSyntax
//@[39:50)          VariableAccessSyntax
//@[39:50)           IdentifierSyntax
//@[39:50)            Identifier |singleGroup|
//@[50:51)          Dot |.|
//@[51:61)          IdentifierSyntax
//@[51:61)           Identifier |properties|
//@[61:62)         Dot |.|
//@[62:73)         IdentifierSyntax
//@[62:73)          Identifier |displayName|
//@[73:84)        StringRightPiece |}) (set 2)'|
//@[84:86)      NewLine |\r\n|
  }
//@[2:3)      RightBrace |}|
//@[3:5)    NewLine |\r\n|
  dependsOn: [
//@[2:38)    ObjectPropertySyntax
//@[2:11)     IdentifierSyntax
//@[2:11)      Identifier |dependsOn|
//@[11:12)     Colon |:|
//@[13:38)     ArraySyntax
//@[13:14)      LeftSquare |[|
//@[14:16)      NewLine |\r\n|
    anotherSet[0]
//@[4:17)      ArrayItemSyntax
//@[4:17)       ArrayAccessSyntax
//@[4:14)        VariableAccessSyntax
//@[4:14)         IdentifierSyntax
//@[4:14)          Identifier |anotherSet|
//@[14:15)        LeftSquare |[|
//@[15:16)        IntegerLiteralSyntax
//@[15:16)         Integer |0|
//@[16:17)        RightSquare |]|
//@[17:19)      NewLine |\r\n|
  ]
//@[2:3)      RightSquare |]|
//@[3:5)    NewLine |\r\n|
}]
//@[0:1)    RightBrace |}|
//@[1:2)   RightSquare |]|
//@[2:2) EndOfFile ||
