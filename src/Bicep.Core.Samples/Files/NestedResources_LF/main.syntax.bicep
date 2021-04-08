resource basicParent 'My.Rp/parentType@2020-12-01' = {
//@[0:659) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:20)  IdentifierSyntax
//@[9:20)   Identifier |basicParent|
//@[21:50)  StringSyntax
//@[21:50)   StringComplete |'My.Rp/parentType@2020-12-01'|
//@[51:52)  Assignment |=|
//@[53:659)  ObjectSyntax
//@[53:54)   LeftBrace |{|
//@[54:55)   NewLine |\n|
  name: 'basicParent'
//@[2:21)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:21)    StringSyntax
//@[8:21)     StringComplete |'basicParent'|
//@[21:22)   NewLine |\n|
  properties: {
//@[2:37)   ObjectPropertySyntax
//@[2:12)    IdentifierSyntax
//@[2:12)     Identifier |properties|
//@[12:13)    Colon |:|
//@[14:37)    ObjectSyntax
//@[14:15)     LeftBrace |{|
//@[15:16)     NewLine |\n|
    size: 'large'
//@[4:17)     ObjectPropertySyntax
//@[4:8)      IdentifierSyntax
//@[4:8)       Identifier |size|
//@[8:9)      Colon |:|
//@[10:17)      StringSyntax
//@[10:17)       StringComplete |'large'|
//@[17:18)     NewLine |\n|
  }
//@[2:3)     RightBrace |}|
//@[3:5)   NewLine |\n\n|

  resource basicChild 'childType' = {
//@[2:349)   ResourceDeclarationSyntax
//@[2:10)    Identifier |resource|
//@[11:21)    IdentifierSyntax
//@[11:21)     Identifier |basicChild|
//@[22:33)    StringSyntax
//@[22:33)     StringComplete |'childType'|
//@[34:35)    Assignment |=|
//@[36:349)    ObjectSyntax
//@[36:37)     LeftBrace |{|
//@[37:38)     NewLine |\n|
    name: 'basicChild'
//@[4:22)     ObjectPropertySyntax
//@[4:8)      IdentifierSyntax
//@[4:8)       Identifier |name|
//@[8:9)      Colon |:|
//@[10:22)      StringSyntax
//@[10:22)       StringComplete |'basicChild'|
//@[22:23)     NewLine |\n|
    properties: {
//@[4:84)     ObjectPropertySyntax
//@[4:14)      IdentifierSyntax
//@[4:14)       Identifier |properties|
//@[14:15)      Colon |:|
//@[16:84)      ObjectSyntax
//@[16:17)       LeftBrace |{|
//@[17:18)       NewLine |\n|
      size: basicParent.properties.large
//@[6:40)       ObjectPropertySyntax
//@[6:10)        IdentifierSyntax
//@[6:10)         Identifier |size|
//@[10:11)        Colon |:|
//@[12:40)        PropertyAccessSyntax
//@[12:34)         PropertyAccessSyntax
//@[12:23)          VariableAccessSyntax
//@[12:23)           IdentifierSyntax
//@[12:23)            Identifier |basicParent|
//@[23:24)          Dot |.|
//@[24:34)          IdentifierSyntax
//@[24:34)           Identifier |properties|
//@[34:35)         Dot |.|
//@[35:40)         IdentifierSyntax
//@[35:40)          Identifier |large|
//@[40:41)       NewLine |\n|
      style: 'cool'
//@[6:19)       ObjectPropertySyntax
//@[6:11)        IdentifierSyntax
//@[6:11)         Identifier |style|
//@[11:12)        Colon |:|
//@[13:19)        StringSyntax
//@[13:19)         StringComplete |'cool'|
//@[19:20)       NewLine |\n|
    }
//@[4:5)       RightBrace |}|
//@[5:7)     NewLine |\n\n|

    resource basicGrandchild 'grandchildType' = {
//@[4:198)     ResourceDeclarationSyntax
//@[4:12)      Identifier |resource|
//@[13:28)      IdentifierSyntax
//@[13:28)       Identifier |basicGrandchild|
//@[29:45)      StringSyntax
//@[29:45)       StringComplete |'grandchildType'|
//@[46:47)      Assignment |=|
//@[48:198)      ObjectSyntax
//@[48:49)       LeftBrace |{|
//@[49:50)       NewLine |\n|
      name: 'basicGrandchild'
//@[6:29)       ObjectPropertySyntax
//@[6:10)        IdentifierSyntax
//@[6:10)         Identifier |name|
//@[10:11)        Colon |:|
//@[12:29)        StringSyntax
//@[12:29)         StringComplete |'basicGrandchild'|
//@[29:30)       NewLine |\n|
      properties: {
//@[6:112)       ObjectPropertySyntax
//@[6:16)        IdentifierSyntax
//@[6:16)         Identifier |properties|
//@[16:17)        Colon |:|
//@[18:112)        ObjectSyntax
//@[18:19)         LeftBrace |{|
//@[19:20)         NewLine |\n|
        size: basicParent.properties.size
//@[8:41)         ObjectPropertySyntax
//@[8:12)          IdentifierSyntax
//@[8:12)           Identifier |size|
//@[12:13)          Colon |:|
//@[14:41)          PropertyAccessSyntax
//@[14:36)           PropertyAccessSyntax
//@[14:25)            VariableAccessSyntax
//@[14:25)             IdentifierSyntax
//@[14:25)              Identifier |basicParent|
//@[25:26)            Dot |.|
//@[26:36)            IdentifierSyntax
//@[26:36)             Identifier |properties|
//@[36:37)           Dot |.|
//@[37:41)           IdentifierSyntax
//@[37:41)            Identifier |size|
//@[41:42)         NewLine |\n|
        style: basicChild.properties.style
//@[8:42)         ObjectPropertySyntax
//@[8:13)          IdentifierSyntax
//@[8:13)           Identifier |style|
//@[13:14)          Colon |:|
//@[15:42)          PropertyAccessSyntax
//@[15:36)           PropertyAccessSyntax
//@[15:25)            VariableAccessSyntax
//@[15:25)             IdentifierSyntax
//@[15:25)              Identifier |basicChild|
//@[25:26)            Dot |.|
//@[26:36)            IdentifierSyntax
//@[26:36)             Identifier |properties|
//@[36:37)           Dot |.|
//@[37:42)           IdentifierSyntax
//@[37:42)            Identifier |style|
//@[42:43)         NewLine |\n|
      }
//@[6:7)         RightBrace |}|
//@[7:8)       NewLine |\n|
    }
//@[4:5)       RightBrace |}|
//@[5:6)     NewLine |\n|
  }
//@[2:3)     RightBrace |}|
//@[3:5)   NewLine |\n\n|

  resource basicSibling 'childType' = {
//@[2:190)   ResourceDeclarationSyntax
//@[2:10)    Identifier |resource|
//@[11:23)    IdentifierSyntax
//@[11:23)     Identifier |basicSibling|
//@[24:35)    StringSyntax
//@[24:35)     StringComplete |'childType'|
//@[36:37)    Assignment |=|
//@[38:190)    ObjectSyntax
//@[38:39)     LeftBrace |{|
//@[39:40)     NewLine |\n|
    name: 'basicSibling'
//@[4:24)     ObjectPropertySyntax
//@[4:8)      IdentifierSyntax
//@[4:8)       Identifier |name|
//@[8:9)      Colon |:|
//@[10:24)      StringSyntax
//@[10:24)       StringComplete |'basicSibling'|
//@[24:25)     NewLine |\n|
    properties: {
//@[4:121)     ObjectPropertySyntax
//@[4:14)      IdentifierSyntax
//@[4:14)       Identifier |properties|
//@[14:15)      Colon |:|
//@[16:121)      ObjectSyntax
//@[16:17)       LeftBrace |{|
//@[17:18)       NewLine |\n|
      size: basicParent.properties.size
//@[6:39)       ObjectPropertySyntax
//@[6:10)        IdentifierSyntax
//@[6:10)         Identifier |size|
//@[10:11)        Colon |:|
//@[12:39)        PropertyAccessSyntax
//@[12:34)         PropertyAccessSyntax
//@[12:23)          VariableAccessSyntax
//@[12:23)           IdentifierSyntax
//@[12:23)            Identifier |basicParent|
//@[23:24)          Dot |.|
//@[24:34)          IdentifierSyntax
//@[24:34)           Identifier |properties|
//@[34:35)         Dot |.|
//@[35:39)         IdentifierSyntax
//@[35:39)          Identifier |size|
//@[39:40)       NewLine |\n|
      style: basicChild::basicGrandchild.properties.style
//@[6:57)       ObjectPropertySyntax
//@[6:11)        IdentifierSyntax
//@[6:11)         Identifier |style|
//@[11:12)        Colon |:|
//@[13:57)        PropertyAccessSyntax
//@[13:51)         PropertyAccessSyntax
//@[13:40)          ResourceAccessSyntax
//@[13:23)           VariableAccessSyntax
//@[13:23)            IdentifierSyntax
//@[13:23)             Identifier |basicChild|
//@[23:25)           DoubleColon |::|
//@[25:40)           IdentifierSyntax
//@[25:40)            Identifier |basicGrandchild|
//@[40:41)          Dot |.|
//@[41:51)          IdentifierSyntax
//@[41:51)           Identifier |properties|
//@[51:52)         Dot |.|
//@[52:57)         IdentifierSyntax
//@[52:57)          Identifier |style|
//@[57:58)       NewLine |\n|
    }
//@[4:5)       RightBrace |}|
//@[5:6)     NewLine |\n|
  }
//@[2:3)     RightBrace |}|
//@[3:4)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:2) NewLine |\n|
// #completionTest(50) -> childResources
//@[40:41) NewLine |\n|
output referenceBasicChild string = basicParent::basicChild.properties.size
//@[0:75) OutputDeclarationSyntax
//@[0:6)  Identifier |output|
//@[7:26)  IdentifierSyntax
//@[7:26)   Identifier |referenceBasicChild|
//@[27:33)  TypeSyntax
//@[27:33)   Identifier |string|
//@[34:35)  Assignment |=|
//@[36:75)  PropertyAccessSyntax
//@[36:70)   PropertyAccessSyntax
//@[36:59)    ResourceAccessSyntax
//@[36:47)     VariableAccessSyntax
//@[36:47)      IdentifierSyntax
//@[36:47)       Identifier |basicParent|
//@[47:49)     DoubleColon |::|
//@[49:59)     IdentifierSyntax
//@[49:59)      Identifier |basicChild|
//@[59:60)    Dot |.|
//@[60:70)    IdentifierSyntax
//@[60:70)     Identifier |properties|
//@[70:71)   Dot |.|
//@[71:75)   IdentifierSyntax
//@[71:75)    Identifier |size|
//@[75:76) NewLine |\n|
// #completionTest(67) -> grandChildResources
//@[45:46) NewLine |\n|
output referenceBasicGrandchild string = basicParent::basicChild::basicGrandchild.properties.style
//@[0:98) OutputDeclarationSyntax
//@[0:6)  Identifier |output|
//@[7:31)  IdentifierSyntax
//@[7:31)   Identifier |referenceBasicGrandchild|
//@[32:38)  TypeSyntax
//@[32:38)   Identifier |string|
//@[39:40)  Assignment |=|
//@[41:98)  PropertyAccessSyntax
//@[41:92)   PropertyAccessSyntax
//@[41:81)    ResourceAccessSyntax
//@[41:64)     ResourceAccessSyntax
//@[41:52)      VariableAccessSyntax
//@[41:52)       IdentifierSyntax
//@[41:52)        Identifier |basicParent|
//@[52:54)      DoubleColon |::|
//@[54:64)      IdentifierSyntax
//@[54:64)       Identifier |basicChild|
//@[64:66)     DoubleColon |::|
//@[66:81)     IdentifierSyntax
//@[66:81)      Identifier |basicGrandchild|
//@[81:82)    Dot |.|
//@[82:92)    IdentifierSyntax
//@[82:92)     Identifier |properties|
//@[92:93)   Dot |.|
//@[93:98)   IdentifierSyntax
//@[93:98)    Identifier |style|
//@[98:100) NewLine |\n\n|

resource existingParent 'My.Rp/parentType@2020-12-01' existing = {
//@[0:386) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:23)  IdentifierSyntax
//@[9:23)   Identifier |existingParent|
//@[24:53)  StringSyntax
//@[24:53)   StringComplete |'My.Rp/parentType@2020-12-01'|
//@[54:62)  Identifier |existing|
//@[63:64)  Assignment |=|
//@[65:386)  ObjectSyntax
//@[65:66)   LeftBrace |{|
//@[66:67)   NewLine |\n|
  name: 'existingParent'
//@[2:24)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:24)    StringSyntax
//@[8:24)     StringComplete |'existingParent'|
//@[24:26)   NewLine |\n\n|

  resource existingChild 'childType' existing = {
//@[2:291)   ResourceDeclarationSyntax
//@[2:10)    Identifier |resource|
//@[11:24)    IdentifierSyntax
//@[11:24)     Identifier |existingChild|
//@[25:36)    StringSyntax
//@[25:36)     StringComplete |'childType'|
//@[37:45)    Identifier |existing|
//@[46:47)    Assignment |=|
//@[48:291)    ObjectSyntax
//@[48:49)     LeftBrace |{|
//@[49:50)     NewLine |\n|
    name: 'existingChild'
//@[4:25)     ObjectPropertySyntax
//@[4:8)      IdentifierSyntax
//@[4:8)       Identifier |name|
//@[8:9)      Colon |:|
//@[10:25)      StringSyntax
//@[10:25)       StringComplete |'existingChild'|
//@[25:27)     NewLine |\n\n|

    resource existingGrandchild 'grandchildType' = {
//@[4:210)     ResourceDeclarationSyntax
//@[4:12)      Identifier |resource|
//@[13:31)      IdentifierSyntax
//@[13:31)       Identifier |existingGrandchild|
//@[32:48)      StringSyntax
//@[32:48)       StringComplete |'grandchildType'|
//@[49:50)      Assignment |=|
//@[51:210)      ObjectSyntax
//@[51:52)       LeftBrace |{|
//@[52:53)       NewLine |\n|
      name: 'existingGrandchild'
//@[6:32)       ObjectPropertySyntax
//@[6:10)        IdentifierSyntax
//@[6:10)         Identifier |name|
//@[10:11)        Colon |:|
//@[12:32)        StringSyntax
//@[12:32)         StringComplete |'existingGrandchild'|
//@[32:33)       NewLine |\n|
      properties: {
//@[6:118)       ObjectPropertySyntax
//@[6:16)        IdentifierSyntax
//@[6:16)         Identifier |properties|
//@[16:17)        Colon |:|
//@[18:118)        ObjectSyntax
//@[18:19)         LeftBrace |{|
//@[19:20)         NewLine |\n|
        size: existingParent.properties.size
//@[8:44)         ObjectPropertySyntax
//@[8:12)          IdentifierSyntax
//@[8:12)           Identifier |size|
//@[12:13)          Colon |:|
//@[14:44)          PropertyAccessSyntax
//@[14:39)           PropertyAccessSyntax
//@[14:28)            VariableAccessSyntax
//@[14:28)             IdentifierSyntax
//@[14:28)              Identifier |existingParent|
//@[28:29)            Dot |.|
//@[29:39)            IdentifierSyntax
//@[29:39)             Identifier |properties|
//@[39:40)           Dot |.|
//@[40:44)           IdentifierSyntax
//@[40:44)            Identifier |size|
//@[44:45)         NewLine |\n|
        style: existingChild.properties.style
//@[8:45)         ObjectPropertySyntax
//@[8:13)          IdentifierSyntax
//@[8:13)           Identifier |style|
//@[13:14)          Colon |:|
//@[15:45)          PropertyAccessSyntax
//@[15:39)           PropertyAccessSyntax
//@[15:28)            VariableAccessSyntax
//@[15:28)             IdentifierSyntax
//@[15:28)              Identifier |existingChild|
//@[28:29)            Dot |.|
//@[29:39)            IdentifierSyntax
//@[29:39)             Identifier |properties|
//@[39:40)           Dot |.|
//@[40:45)           IdentifierSyntax
//@[40:45)            Identifier |style|
//@[45:46)         NewLine |\n|
      }
//@[6:7)         RightBrace |}|
//@[7:8)       NewLine |\n|
    }
//@[4:5)       RightBrace |}|
//@[5:6)     NewLine |\n|
  }
//@[2:3)     RightBrace |}|
//@[3:4)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

param createParent bool
//@[0:23) ParameterDeclarationSyntax
//@[0:5)  Identifier |param|
//@[6:18)  IdentifierSyntax
//@[6:18)   Identifier |createParent|
//@[19:23)  TypeSyntax
//@[19:23)   Identifier |bool|
//@[23:24) NewLine |\n|
param createChild bool
//@[0:22) ParameterDeclarationSyntax
//@[0:5)  Identifier |param|
//@[6:17)  IdentifierSyntax
//@[6:17)   Identifier |createChild|
//@[18:22)  TypeSyntax
//@[18:22)   Identifier |bool|
//@[22:23) NewLine |\n|
param createGrandchild bool
//@[0:27) ParameterDeclarationSyntax
//@[0:5)  Identifier |param|
//@[6:22)  IdentifierSyntax
//@[6:22)   Identifier |createGrandchild|
//@[23:27)  TypeSyntax
//@[23:27)   Identifier |bool|
//@[27:28) NewLine |\n|
resource conditionParent 'My.Rp/parentType@2020-12-01' = if (createParent) {
//@[0:433) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:24)  IdentifierSyntax
//@[9:24)   Identifier |conditionParent|
//@[25:54)  StringSyntax
//@[25:54)   StringComplete |'My.Rp/parentType@2020-12-01'|
//@[55:56)  Assignment |=|
//@[57:433)  IfConditionSyntax
//@[57:59)   Identifier |if|
//@[60:74)   ParenthesizedExpressionSyntax
//@[60:61)    LeftParen |(|
//@[61:73)    VariableAccessSyntax
//@[61:73)     IdentifierSyntax
//@[61:73)      Identifier |createParent|
//@[73:74)    RightParen |)|
//@[75:433)   ObjectSyntax
//@[75:76)    LeftBrace |{|
//@[76:77)    NewLine |\n|
  name: 'conditionParent'
//@[2:25)    ObjectPropertySyntax
//@[2:6)     IdentifierSyntax
//@[2:6)      Identifier |name|
//@[6:7)     Colon |:|
//@[8:25)     StringSyntax
//@[8:25)      StringComplete |'conditionParent'|
//@[25:27)    NewLine |\n\n|

  resource conditionChild 'childType' = if (createChild) {
//@[2:327)    ResourceDeclarationSyntax
//@[2:10)     Identifier |resource|
//@[11:25)     IdentifierSyntax
//@[11:25)      Identifier |conditionChild|
//@[26:37)     StringSyntax
//@[26:37)      StringComplete |'childType'|
//@[38:39)     Assignment |=|
//@[40:327)     IfConditionSyntax
//@[40:42)      Identifier |if|
//@[43:56)      ParenthesizedExpressionSyntax
//@[43:44)       LeftParen |(|
//@[44:55)       VariableAccessSyntax
//@[44:55)        IdentifierSyntax
//@[44:55)         Identifier |createChild|
//@[55:56)       RightParen |)|
//@[57:327)      ObjectSyntax
//@[57:58)       LeftBrace |{|
//@[58:59)       NewLine |\n|
    name: 'conditionChild'
//@[4:26)       ObjectPropertySyntax
//@[4:8)        IdentifierSyntax
//@[4:8)         Identifier |name|
//@[8:9)        Colon |:|
//@[10:26)        StringSyntax
//@[10:26)         StringComplete |'conditionChild'|
//@[26:28)       NewLine |\n\n|

    resource conditionGrandchild 'grandchildType' = if (createGrandchild) {
//@[4:236)       ResourceDeclarationSyntax
//@[4:12)        Identifier |resource|
//@[13:32)        IdentifierSyntax
//@[13:32)         Identifier |conditionGrandchild|
//@[33:49)        StringSyntax
//@[33:49)         StringComplete |'grandchildType'|
//@[50:51)        Assignment |=|
//@[52:236)        IfConditionSyntax
//@[52:54)         Identifier |if|
//@[55:73)         ParenthesizedExpressionSyntax
//@[55:56)          LeftParen |(|
//@[56:72)          VariableAccessSyntax
//@[56:72)           IdentifierSyntax
//@[56:72)            Identifier |createGrandchild|
//@[72:73)          RightParen |)|
//@[74:236)         ObjectSyntax
//@[74:75)          LeftBrace |{|
//@[75:76)          NewLine |\n|
      name: 'conditionGrandchild'
//@[6:33)          ObjectPropertySyntax
//@[6:10)           IdentifierSyntax
//@[6:10)            Identifier |name|
//@[10:11)           Colon |:|
//@[12:33)           StringSyntax
//@[12:33)            StringComplete |'conditionGrandchild'|
//@[33:34)          NewLine |\n|
      properties: {
//@[6:120)          ObjectPropertySyntax
//@[6:16)           IdentifierSyntax
//@[6:16)            Identifier |properties|
//@[16:17)           Colon |:|
//@[18:120)           ObjectSyntax
//@[18:19)            LeftBrace |{|
//@[19:20)            NewLine |\n|
        size: conditionParent.properties.size
//@[8:45)            ObjectPropertySyntax
//@[8:12)             IdentifierSyntax
//@[8:12)              Identifier |size|
//@[12:13)             Colon |:|
//@[14:45)             PropertyAccessSyntax
//@[14:40)              PropertyAccessSyntax
//@[14:29)               VariableAccessSyntax
//@[14:29)                IdentifierSyntax
//@[14:29)                 Identifier |conditionParent|
//@[29:30)               Dot |.|
//@[30:40)               IdentifierSyntax
//@[30:40)                Identifier |properties|
//@[40:41)              Dot |.|
//@[41:45)              IdentifierSyntax
//@[41:45)               Identifier |size|
//@[45:46)            NewLine |\n|
        style: conditionChild.properties.style
//@[8:46)            ObjectPropertySyntax
//@[8:13)             IdentifierSyntax
//@[8:13)              Identifier |style|
//@[13:14)             Colon |:|
//@[15:46)             PropertyAccessSyntax
//@[15:40)              PropertyAccessSyntax
//@[15:29)               VariableAccessSyntax
//@[15:29)                IdentifierSyntax
//@[15:29)                 Identifier |conditionChild|
//@[29:30)               Dot |.|
//@[30:40)               IdentifierSyntax
//@[30:40)                Identifier |properties|
//@[40:41)              Dot |.|
//@[41:46)              IdentifierSyntax
//@[41:46)               Identifier |style|
//@[46:47)            NewLine |\n|
      }
//@[6:7)            RightBrace |}|
//@[7:8)          NewLine |\n|
    }
//@[4:5)          RightBrace |}|
//@[5:6)       NewLine |\n|
  }
//@[2:3)       RightBrace |}|
//@[3:4)    NewLine |\n|
}
//@[0:1)    RightBrace |}|
//@[1:3) NewLine |\n\n|

var items = [
//@[0:27) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:9)  IdentifierSyntax
//@[4:9)   Identifier |items|
//@[10:11)  Assignment |=|
//@[12:27)  ArraySyntax
//@[12:13)   LeftSquare |[|
//@[13:14)   NewLine |\n|
  'a'
//@[2:5)   ArrayItemSyntax
//@[2:5)    StringSyntax
//@[2:5)     StringComplete |'a'|
//@[5:6)   NewLine |\n|
  'b'
//@[2:5)   ArrayItemSyntax
//@[2:5)    StringSyntax
//@[2:5)     StringComplete |'b'|
//@[5:6)   NewLine |\n|
]
//@[0:1)   RightSquare |]|
//@[1:2) NewLine |\n|
resource loopParent 'My.Rp/parentType@2020-12-01' = {
//@[0:161) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:19)  IdentifierSyntax
//@[9:19)   Identifier |loopParent|
//@[20:49)  StringSyntax
//@[20:49)   StringComplete |'My.Rp/parentType@2020-12-01'|
//@[50:51)  Assignment |=|
//@[52:161)  ObjectSyntax
//@[52:53)   LeftBrace |{|
//@[53:54)   NewLine |\n|
  name: 'loopParent'
//@[2:20)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:20)    StringSyntax
//@[8:20)     StringComplete |'loopParent'|
//@[20:22)   NewLine |\n\n|

  resource loopChild 'childType' = [for item in items: {
//@[2:83)   ResourceDeclarationSyntax
//@[2:10)    Identifier |resource|
//@[11:20)    IdentifierSyntax
//@[11:20)     Identifier |loopChild|
//@[21:32)    StringSyntax
//@[21:32)     StringComplete |'childType'|
//@[33:34)    Assignment |=|
//@[35:83)    ForSyntax
//@[35:36)     LeftSquare |[|
//@[36:39)     Identifier |for|
//@[40:44)     LocalVariableSyntax
//@[40:44)      IdentifierSyntax
//@[40:44)       Identifier |item|
//@[45:47)     Identifier |in|
//@[48:53)     VariableAccessSyntax
//@[48:53)      IdentifierSyntax
//@[48:53)       Identifier |items|
//@[53:54)     Colon |:|
//@[55:82)     ObjectSyntax
//@[55:56)      LeftBrace |{|
//@[56:57)      NewLine |\n|
    name: 'loopChild'
//@[4:21)      ObjectPropertySyntax
//@[4:8)       IdentifierSyntax
//@[4:8)        Identifier |name|
//@[8:9)       Colon |:|
//@[10:21)       StringSyntax
//@[10:21)        StringComplete |'loopChild'|
//@[21:22)      NewLine |\n|
  }]
//@[2:3)      RightBrace |}|
//@[3:4)     RightSquare |]|
//@[4:5)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

output loopChildOutput string = loopParent::loopChild[0].name
//@[0:61) OutputDeclarationSyntax
//@[0:6)  Identifier |output|
//@[7:22)  IdentifierSyntax
//@[7:22)   Identifier |loopChildOutput|
//@[23:29)  TypeSyntax
//@[23:29)   Identifier |string|
//@[30:31)  Assignment |=|
//@[32:61)  PropertyAccessSyntax
//@[32:56)   ArrayAccessSyntax
//@[32:53)    ResourceAccessSyntax
//@[32:42)     VariableAccessSyntax
//@[32:42)      IdentifierSyntax
//@[32:42)       Identifier |loopParent|
//@[42:44)     DoubleColon |::|
//@[44:53)     IdentifierSyntax
//@[44:53)      Identifier |loopChild|
//@[53:54)    LeftSquare |[|
//@[54:55)    IntegerLiteralSyntax
//@[54:55)     Integer |0|
//@[55:56)    RightSquare |]|
//@[56:57)   Dot |.|
//@[57:61)   IdentifierSyntax
//@[57:61)    Identifier |name|
//@[61:61) EndOfFile ||
