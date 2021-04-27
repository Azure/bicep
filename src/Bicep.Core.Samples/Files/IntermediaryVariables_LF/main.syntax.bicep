var boolVal = true
//@[0:18) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:11)  IdentifierSyntax
//@[4:11)   Identifier |boolVal|
//@[12:13)  Assignment |=|
//@[14:18)  BooleanLiteralSyntax
//@[14:18)   TrueKeyword |true|
//@[18:20) NewLine |\n\n|

var vmProperties = {
//@[0:173) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:16)  IdentifierSyntax
//@[4:16)   Identifier |vmProperties|
//@[17:18)  Assignment |=|
//@[19:173)  ObjectSyntax
//@[19:20)   LeftBrace |{|
//@[20:21)   NewLine |\n|
  diagnosticsProfile: {
//@[2:124)   ObjectPropertySyntax
//@[2:20)    IdentifierSyntax
//@[2:20)     Identifier |diagnosticsProfile|
//@[20:21)    Colon |:|
//@[22:124)    ObjectSyntax
//@[22:23)     LeftBrace |{|
//@[23:24)     NewLine |\n|
    bootDiagnostics: {
//@[4:96)     ObjectPropertySyntax
//@[4:19)      IdentifierSyntax
//@[4:19)       Identifier |bootDiagnostics|
//@[19:20)      Colon |:|
//@[21:96)      ObjectSyntax
//@[21:22)       LeftBrace |{|
//@[22:23)       NewLine |\n|
      enabled: 123
//@[6:18)       ObjectPropertySyntax
//@[6:13)        IdentifierSyntax
//@[6:13)         Identifier |enabled|
//@[13:14)        Colon |:|
//@[15:18)        IntegerLiteralSyntax
//@[15:18)         Integer |123|
//@[18:19)       NewLine |\n|
      storageUri: true
//@[6:22)       ObjectPropertySyntax
//@[6:16)        IdentifierSyntax
//@[6:16)         Identifier |storageUri|
//@[16:17)        Colon |:|
//@[18:22)        BooleanLiteralSyntax
//@[18:22)         TrueKeyword |true|
//@[22:23)       NewLine |\n|
      unknownProp: 'asdf'
//@[6:25)       ObjectPropertySyntax
//@[6:17)        IdentifierSyntax
//@[6:17)         Identifier |unknownProp|
//@[17:18)        Colon |:|
//@[19:25)        StringSyntax
//@[19:25)         StringComplete |'asdf'|
//@[25:26)       NewLine |\n|
    }
//@[4:5)       RightBrace |}|
//@[5:6)     NewLine |\n|
  }
//@[2:3)     RightBrace |}|
//@[3:4)   NewLine |\n|
  evictionPolicy: boolVal
//@[2:25)   ObjectPropertySyntax
//@[2:16)    IdentifierSyntax
//@[2:16)     Identifier |evictionPolicy|
//@[16:17)    Colon |:|
//@[18:25)    VariableAccessSyntax
//@[18:25)     IdentifierSyntax
//@[18:25)      Identifier |boolVal|
//@[25:26)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

resource vm 'Microsoft.Compute/virtualMachines@2020-12-01' = {
//@[0:126) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:11)  IdentifierSyntax
//@[9:11)   Identifier |vm|
//@[12:58)  StringSyntax
//@[12:58)   StringComplete |'Microsoft.Compute/virtualMachines@2020-12-01'|
//@[59:60)  Assignment |=|
//@[61:126)  ObjectSyntax
//@[61:62)   LeftBrace |{|
//@[62:63)   NewLine |\n|
  name: 'vm'
//@[2:12)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:12)    StringSyntax
//@[8:12)     StringComplete |'vm'|
//@[12:13)   NewLine |\n|
  location: 'West US'
//@[2:21)   ObjectPropertySyntax
//@[2:10)    IdentifierSyntax
//@[2:10)     Identifier |location|
//@[10:11)    Colon |:|
//@[12:21)    StringSyntax
//@[12:21)     StringComplete |'West US'|
//@[21:22)   NewLine |\n|
  properties: vmProperties
//@[2:26)   ObjectPropertySyntax
//@[2:12)    IdentifierSyntax
//@[2:12)     Identifier |properties|
//@[12:13)    Colon |:|
//@[14:26)    VariableAccessSyntax
//@[14:26)     IdentifierSyntax
//@[14:26)      Identifier |vmProperties|
//@[26:27)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

var ipConfigurations = [for i in range(0, 2): {
//@[0:148) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:20)  IdentifierSyntax
//@[4:20)   Identifier |ipConfigurations|
//@[21:22)  Assignment |=|
//@[23:148)  ForSyntax
//@[23:24)   LeftSquare |[|
//@[24:27)   Identifier |for|
//@[28:29)   LocalVariableSyntax
//@[28:29)    IdentifierSyntax
//@[28:29)     Identifier |i|
//@[30:32)   Identifier |in|
//@[33:44)   FunctionCallSyntax
//@[33:38)    IdentifierSyntax
//@[33:38)     Identifier |range|
//@[38:39)    LeftParen |(|
//@[39:41)    FunctionArgumentSyntax
//@[39:40)     IntegerLiteralSyntax
//@[39:40)      Integer |0|
//@[40:41)     Comma |,|
//@[42:43)    FunctionArgumentSyntax
//@[42:43)     IntegerLiteralSyntax
//@[42:43)      Integer |2|
//@[43:44)    RightParen |)|
//@[44:45)   Colon |:|
//@[46:147)   ObjectSyntax
//@[46:47)    LeftBrace |{|
//@[47:48)    NewLine |\n|
  id: true
//@[2:10)    ObjectPropertySyntax
//@[2:4)     IdentifierSyntax
//@[2:4)      Identifier |id|
//@[4:5)     Colon |:|
//@[6:10)     BooleanLiteralSyntax
//@[6:10)      TrueKeyword |true|
//@[10:11)    NewLine |\n|
  name: 'asdf${i}'
//@[2:18)    ObjectPropertySyntax
//@[2:6)     IdentifierSyntax
//@[2:6)      Identifier |name|
//@[6:7)     Colon |:|
//@[8:18)     StringSyntax
//@[8:15)      StringLeftPiece |'asdf${|
//@[15:16)      VariableAccessSyntax
//@[15:16)       IdentifierSyntax
//@[15:16)        Identifier |i|
//@[16:18)      StringRightPiece |}'|
//@[18:19)    NewLine |\n|
  properties: {
//@[2:67)    ObjectPropertySyntax
//@[2:12)     IdentifierSyntax
//@[2:12)      Identifier |properties|
//@[12:13)     Colon |:|
//@[14:67)     ObjectSyntax
//@[14:15)      LeftBrace |{|
//@[15:16)      NewLine |\n|
    madeUpProperty: boolVal
//@[4:27)      ObjectPropertySyntax
//@[4:18)       IdentifierSyntax
//@[4:18)        Identifier |madeUpProperty|
//@[18:19)       Colon |:|
//@[20:27)       VariableAccessSyntax
//@[20:27)        IdentifierSyntax
//@[20:27)         Identifier |boolVal|
//@[27:28)      NewLine |\n|
    subnet: 'hello'
//@[4:19)      ObjectPropertySyntax
//@[4:10)       IdentifierSyntax
//@[4:10)        Identifier |subnet|
//@[10:11)       Colon |:|
//@[12:19)       StringSyntax
//@[12:19)        StringComplete |'hello'|
//@[19:20)      NewLine |\n|
  }
//@[2:3)      RightBrace |}|
//@[3:4)    NewLine |\n|
}]
//@[0:1)    RightBrace |}|
//@[1:2)   RightSquare |]|
//@[2:4) NewLine |\n\n|

resource nic 'Microsoft.Network/networkInterfaces@2020-11-01' = {
//@[0:140) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:12)  IdentifierSyntax
//@[9:12)   Identifier |nic|
//@[13:61)  StringSyntax
//@[13:61)   StringComplete |'Microsoft.Network/networkInterfaces@2020-11-01'|
//@[62:63)  Assignment |=|
//@[64:140)  ObjectSyntax
//@[64:65)   LeftBrace |{|
//@[65:66)   NewLine |\n|
  name: 'abc'
//@[2:13)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:13)    StringSyntax
//@[8:13)     StringComplete |'abc'|
//@[13:14)   NewLine |\n|
  properties: {
//@[2:58)   ObjectPropertySyntax
//@[2:12)    IdentifierSyntax
//@[2:12)     Identifier |properties|
//@[12:13)    Colon |:|
//@[14:58)    ObjectSyntax
//@[14:15)     LeftBrace |{|
//@[15:16)     NewLine |\n|
    ipConfigurations: ipConfigurations
//@[4:38)     ObjectPropertySyntax
//@[4:20)      IdentifierSyntax
//@[4:20)       Identifier |ipConfigurations|
//@[20:21)      Colon |:|
//@[22:38)      VariableAccessSyntax
//@[22:38)       IdentifierSyntax
//@[22:38)        Identifier |ipConfigurations|
//@[38:39)     NewLine |\n|
  }
//@[2:3)     RightBrace |}|
//@[3:4)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

resource nicLoop 'Microsoft.Network/networkInterfaces@2020-11-01' = [for i in range(0, 2): {
//@[0:213) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:16)  IdentifierSyntax
//@[9:16)   Identifier |nicLoop|
//@[17:65)  StringSyntax
//@[17:65)   StringComplete |'Microsoft.Network/networkInterfaces@2020-11-01'|
//@[66:67)  Assignment |=|
//@[68:213)  ForSyntax
//@[68:69)   LeftSquare |[|
//@[69:72)   Identifier |for|
//@[73:74)   LocalVariableSyntax
//@[73:74)    IdentifierSyntax
//@[73:74)     Identifier |i|
//@[75:77)   Identifier |in|
//@[78:89)   FunctionCallSyntax
//@[78:83)    IdentifierSyntax
//@[78:83)     Identifier |range|
//@[83:84)    LeftParen |(|
//@[84:86)    FunctionArgumentSyntax
//@[84:85)     IntegerLiteralSyntax
//@[84:85)      Integer |0|
//@[85:86)     Comma |,|
//@[87:88)    FunctionArgumentSyntax
//@[87:88)     IntegerLiteralSyntax
//@[87:88)      Integer |2|
//@[88:89)    RightParen |)|
//@[89:90)   Colon |:|
//@[91:212)   ObjectSyntax
//@[91:92)    LeftBrace |{|
//@[92:93)    NewLine |\n|
  name: 'abc${i}'
//@[2:17)    ObjectPropertySyntax
//@[2:6)     IdentifierSyntax
//@[2:6)      Identifier |name|
//@[6:7)     Colon |:|
//@[8:17)     StringSyntax
//@[8:14)      StringLeftPiece |'abc${|
//@[14:15)      VariableAccessSyntax
//@[14:15)       IdentifierSyntax
//@[14:15)        Identifier |i|
//@[15:17)      StringRightPiece |}'|
//@[17:18)    NewLine |\n|
  properties: {
//@[2:99)    ObjectPropertySyntax
//@[2:12)     IdentifierSyntax
//@[2:12)      Identifier |properties|
//@[12:13)     Colon |:|
//@[14:99)     ObjectSyntax
//@[14:15)      LeftBrace |{|
//@[15:16)      NewLine |\n|
    ipConfigurations: [
//@[4:79)      ObjectPropertySyntax
//@[4:20)       IdentifierSyntax
//@[4:20)        Identifier |ipConfigurations|
//@[20:21)       Colon |:|
//@[22:79)       ArraySyntax
//@[22:23)        LeftSquare |[|
//@[23:24)        NewLine |\n|
      // TODO: fix this
//@[23:24)        NewLine |\n|
      ipConfigurations[i]
//@[6:25)        ArrayItemSyntax
//@[6:25)         ArrayAccessSyntax
//@[6:22)          VariableAccessSyntax
//@[6:22)           IdentifierSyntax
//@[6:22)            Identifier |ipConfigurations|
//@[22:23)          LeftSquare |[|
//@[23:24)          VariableAccessSyntax
//@[23:24)           IdentifierSyntax
//@[23:24)            Identifier |i|
//@[24:25)          RightSquare |]|
//@[25:26)        NewLine |\n|
    ]
//@[4:5)        RightSquare |]|
//@[5:6)      NewLine |\n|
  }
//@[2:3)      RightBrace |}|
//@[3:4)    NewLine |\n|
}]
//@[0:1)    RightBrace |}|
//@[1:2)   RightSquare |]|
//@[2:4) NewLine |\n\n|

resource nicLoop2 'Microsoft.Network/networkInterfaces@2020-11-01' = [for ipConfig in ipConfigurations: {
//@[0:227) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:17)  IdentifierSyntax
//@[9:17)   Identifier |nicLoop2|
//@[18:66)  StringSyntax
//@[18:66)   StringComplete |'Microsoft.Network/networkInterfaces@2020-11-01'|
//@[67:68)  Assignment |=|
//@[69:227)  ForSyntax
//@[69:70)   LeftSquare |[|
//@[70:73)   Identifier |for|
//@[74:82)   LocalVariableSyntax
//@[74:82)    IdentifierSyntax
//@[74:82)     Identifier |ipConfig|
//@[83:85)   Identifier |in|
//@[86:102)   VariableAccessSyntax
//@[86:102)    IdentifierSyntax
//@[86:102)     Identifier |ipConfigurations|
//@[102:103)   Colon |:|
//@[104:226)   ObjectSyntax
//@[104:105)    LeftBrace |{|
//@[105:106)    NewLine |\n|
  name: 'abc${ipConfig.name}'
//@[2:29)    ObjectPropertySyntax
//@[2:6)     IdentifierSyntax
//@[2:6)      Identifier |name|
//@[6:7)     Colon |:|
//@[8:29)     StringSyntax
//@[8:14)      StringLeftPiece |'abc${|
//@[14:27)      PropertyAccessSyntax
//@[14:22)       VariableAccessSyntax
//@[14:22)        IdentifierSyntax
//@[14:22)         Identifier |ipConfig|
//@[22:23)       Dot |.|
//@[23:27)       IdentifierSyntax
//@[23:27)        Identifier |name|
//@[27:29)      StringRightPiece |}'|
//@[29:30)    NewLine |\n|
  properties: {
//@[2:88)    ObjectPropertySyntax
//@[2:12)     IdentifierSyntax
//@[2:12)      Identifier |properties|
//@[12:13)     Colon |:|
//@[14:88)     ObjectSyntax
//@[14:15)      LeftBrace |{|
//@[15:16)      NewLine |\n|
    ipConfigurations: [
//@[4:68)      ObjectPropertySyntax
//@[4:20)       IdentifierSyntax
//@[4:20)        Identifier |ipConfigurations|
//@[20:21)       Colon |:|
//@[22:68)       ArraySyntax
//@[22:23)        LeftSquare |[|
//@[23:24)        NewLine |\n|
      // TODO: fix this
//@[23:24)        NewLine |\n|
      ipConfig
//@[6:14)        ArrayItemSyntax
//@[6:14)         VariableAccessSyntax
//@[6:14)          IdentifierSyntax
//@[6:14)           Identifier |ipConfig|
//@[14:15)        NewLine |\n|
    ]
//@[4:5)        RightSquare |]|
//@[5:6)      NewLine |\n|
  }
//@[2:3)      RightBrace |}|
//@[3:4)    NewLine |\n|
}]
//@[0:1)    RightBrace |}|
//@[1:2)   RightSquare |]|
//@[2:3) NewLine |\n|

//@[0:0) EndOfFile ||
