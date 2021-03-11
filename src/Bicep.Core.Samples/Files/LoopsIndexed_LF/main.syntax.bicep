param name string
//@[0:17) ParameterDeclarationSyntax
//@[0:5)  Identifier |param|
//@[6:10)  IdentifierSyntax
//@[6:10)   Identifier |name|
//@[11:17)  TypeSyntax
//@[11:17)   Identifier |string|
//@[17:18) NewLine |\n|
param accounts array
//@[0:20) ParameterDeclarationSyntax
//@[0:5)  Identifier |param|
//@[6:14)  IdentifierSyntax
//@[6:14)   Identifier |accounts|
//@[15:20)  TypeSyntax
//@[15:20)   Identifier |array|
//@[20:21) NewLine |\n|
param index int
//@[0:15) ParameterDeclarationSyntax
//@[0:5)  Identifier |param|
//@[6:11)  IdentifierSyntax
//@[6:11)   Identifier |index|
//@[12:15)  TypeSyntax
//@[12:15)   Identifier |int|
//@[15:17) NewLine |\n\n|

// single resource
//@[18:19) NewLine |\n|
resource singleResource 'Microsoft.Storage/storageAccounts@2019-06-01' = {
//@[0:209) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:23)  IdentifierSyntax
//@[9:23)   Identifier |singleResource|
//@[24:70)  StringSyntax
//@[24:70)   StringComplete |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[71:72)  Assignment |=|
//@[73:209)  ObjectSyntax
//@[73:74)   LeftBrace |{|
//@[74:75)   NewLine |\n|
  name: '${name}single-resource-name'
//@[2:37)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:37)    StringSyntax
//@[8:11)     StringLeftPiece |'${|
//@[11:15)     VariableAccessSyntax
//@[11:15)      IdentifierSyntax
//@[11:15)       Identifier |name|
//@[15:37)     StringRightPiece |}single-resource-name'|
//@[37:38)   NewLine |\n|
  location: resourceGroup().location
//@[2:36)   ObjectPropertySyntax
//@[2:10)    IdentifierSyntax
//@[2:10)     Identifier |location|
//@[10:11)    Colon |:|
//@[12:36)    PropertyAccessSyntax
//@[12:27)     FunctionCallSyntax
//@[12:25)      IdentifierSyntax
//@[12:25)       Identifier |resourceGroup|
//@[25:26)      LeftParen |(|
//@[26:27)      RightParen |)|
//@[27:28)     Dot |.|
//@[28:36)     IdentifierSyntax
//@[28:36)      Identifier |location|
//@[36:37)   NewLine |\n|
  kind: 'StorageV2'
//@[2:19)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |kind|
//@[6:7)    Colon |:|
//@[8:19)    StringSyntax
//@[8:19)     StringComplete |'StorageV2'|
//@[19:20)   NewLine |\n|
  sku: {
//@[2:37)   ObjectPropertySyntax
//@[2:5)    IdentifierSyntax
//@[2:5)     Identifier |sku|
//@[5:6)    Colon |:|
//@[7:37)    ObjectSyntax
//@[7:8)     LeftBrace |{|
//@[8:9)     NewLine |\n|
    name: 'Standard_LRS'
//@[4:24)     ObjectPropertySyntax
//@[4:8)      IdentifierSyntax
//@[4:8)       Identifier |name|
//@[8:9)      Colon |:|
//@[10:24)      StringSyntax
//@[10:24)       StringComplete |'Standard_LRS'|
//@[24:25)     NewLine |\n|
  }
//@[2:3)     RightBrace |}|
//@[3:4)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

// extension of single resource
//@[31:32) NewLine |\n|
resource singleResourceExtension 'Microsoft.Authorization/locks@2016-09-01' = {
//@[0:182) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:32)  IdentifierSyntax
//@[9:32)   Identifier |singleResourceExtension|
//@[33:75)  StringSyntax
//@[33:75)   StringComplete |'Microsoft.Authorization/locks@2016-09-01'|
//@[76:77)  Assignment |=|
//@[78:182)  ObjectSyntax
//@[78:79)   LeftBrace |{|
//@[79:80)   NewLine |\n|
  scope: singleResource
//@[2:23)   ObjectPropertySyntax
//@[2:7)    IdentifierSyntax
//@[2:7)     Identifier |scope|
//@[7:8)    Colon |:|
//@[9:23)    VariableAccessSyntax
//@[9:23)     IdentifierSyntax
//@[9:23)      Identifier |singleResource|
//@[23:24)   NewLine |\n|
  name: 'single-resource-lock'
//@[2:30)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:30)    StringSyntax
//@[8:30)     StringComplete |'single-resource-lock'|
//@[30:31)   NewLine |\n|
  properties: {
//@[2:45)   ObjectPropertySyntax
//@[2:12)    IdentifierSyntax
//@[2:12)     Identifier |properties|
//@[12:13)    Colon |:|
//@[14:45)    ObjectSyntax
//@[14:15)     LeftBrace |{|
//@[15:16)     NewLine |\n|
    level: 'CanNotDelete'
//@[4:25)     ObjectPropertySyntax
//@[4:9)      IdentifierSyntax
//@[4:9)       Identifier |level|
//@[9:10)      Colon |:|
//@[11:25)      StringSyntax
//@[11:25)       StringComplete |'CanNotDelete'|
//@[25:26)     NewLine |\n|
  }
//@[2:3)     RightBrace |}|
//@[3:4)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

// single resource cascade extension
//@[36:37) NewLine |\n|
resource singleResourceCascadeExtension 'Microsoft.Authorization/locks@2016-09-01' = {
//@[0:211) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:39)  IdentifierSyntax
//@[9:39)   Identifier |singleResourceCascadeExtension|
//@[40:82)  StringSyntax
//@[40:82)   StringComplete |'Microsoft.Authorization/locks@2016-09-01'|
//@[83:84)  Assignment |=|
//@[85:211)  ObjectSyntax
//@[85:86)   LeftBrace |{|
//@[86:87)   NewLine |\n|
  scope: singleResourceExtension
//@[2:32)   ObjectPropertySyntax
//@[2:7)    IdentifierSyntax
//@[2:7)     Identifier |scope|
//@[7:8)    Colon |:|
//@[9:32)    VariableAccessSyntax
//@[9:32)     IdentifierSyntax
//@[9:32)      Identifier |singleResourceExtension|
//@[32:33)   NewLine |\n|
  name: 'single-resource-cascade-extension'
//@[2:43)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:43)    StringSyntax
//@[8:43)     StringComplete |'single-resource-cascade-extension'|
//@[43:44)   NewLine |\n|
  properties: {
//@[2:45)   ObjectPropertySyntax
//@[2:12)    IdentifierSyntax
//@[2:12)     Identifier |properties|
//@[12:13)    Colon |:|
//@[14:45)    ObjectSyntax
//@[14:15)     LeftBrace |{|
//@[15:16)     NewLine |\n|
    level: 'CanNotDelete'
//@[4:25)     ObjectPropertySyntax
//@[4:9)      IdentifierSyntax
//@[4:9)       Identifier |level|
//@[9:10)      Colon |:|
//@[11:25)      StringSyntax
//@[11:25)       StringComplete |'CanNotDelete'|
//@[25:26)     NewLine |\n|
  }
//@[2:3)     RightBrace |}|
//@[3:4)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

// resource collection
//@[22:23) NewLine |\n|
@batchSize(42)
//@[0:307) ResourceDeclarationSyntax
//@[0:14)  DecoratorSyntax
//@[0:1)   At |@|
//@[1:14)   FunctionCallSyntax
//@[1:10)    IdentifierSyntax
//@[1:10)     Identifier |batchSize|
//@[10:11)    LeftParen |(|
//@[11:13)    FunctionArgumentSyntax
//@[11:13)     IntegerLiteralSyntax
//@[11:13)      Integer |42|
//@[13:14)    RightParen |)|
//@[14:15)  NewLine |\n|
resource storageAccounts 'Microsoft.Storage/storageAccounts@2019-06-01' = [for (account, index) in accounts: {
//@[0:8)  Identifier |resource|
//@[9:24)  IdentifierSyntax
//@[9:24)   Identifier |storageAccounts|
//@[25:71)  StringSyntax
//@[25:71)   StringComplete |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[72:73)  Assignment |=|
//@[74:292)  ForSyntax
//@[74:75)   LeftSquare |[|
//@[75:78)   Identifier |for|
//@[79:95)   ForVariableBlockSyntax
//@[79:80)    LeftParen |(|
//@[80:87)    LocalVariableSyntax
//@[80:87)     IdentifierSyntax
//@[80:87)      Identifier |account|
//@[87:88)    Comma |,|
//@[89:94)    LocalVariableSyntax
//@[89:94)     IdentifierSyntax
//@[89:94)      Identifier |index|
//@[94:95)    RightParen |)|
//@[96:98)   Identifier |in|
//@[99:107)   VariableAccessSyntax
//@[99:107)    IdentifierSyntax
//@[99:107)     Identifier |accounts|
//@[107:108)   Colon |:|
//@[109:291)   ObjectSyntax
//@[109:110)    LeftBrace |{|
//@[110:111)    NewLine |\n|
  name: '${name}-collection-${account.name}-${index}'
//@[2:53)    ObjectPropertySyntax
//@[2:6)     IdentifierSyntax
//@[2:6)      Identifier |name|
//@[6:7)     Colon |:|
//@[8:53)     StringSyntax
//@[8:11)      StringLeftPiece |'${|
//@[11:15)      VariableAccessSyntax
//@[11:15)       IdentifierSyntax
//@[11:15)        Identifier |name|
//@[15:30)      StringMiddlePiece |}-collection-${|
//@[30:42)      PropertyAccessSyntax
//@[30:37)       VariableAccessSyntax
//@[30:37)        IdentifierSyntax
//@[30:37)         Identifier |account|
//@[37:38)       Dot |.|
//@[38:42)       IdentifierSyntax
//@[38:42)        Identifier |name|
//@[42:46)      StringMiddlePiece |}-${|
//@[46:51)      VariableAccessSyntax
//@[46:51)       IdentifierSyntax
//@[46:51)        Identifier |index|
//@[51:53)      StringRightPiece |}'|
//@[53:54)    NewLine |\n|
  location: account.location
//@[2:28)    ObjectPropertySyntax
//@[2:10)     IdentifierSyntax
//@[2:10)      Identifier |location|
//@[10:11)     Colon |:|
//@[12:28)     PropertyAccessSyntax
//@[12:19)      VariableAccessSyntax
//@[12:19)       IdentifierSyntax
//@[12:19)        Identifier |account|
//@[19:20)      Dot |.|
//@[20:28)      IdentifierSyntax
//@[20:28)       Identifier |location|
//@[28:29)    NewLine |\n|
  kind: 'StorageV2'
//@[2:19)    ObjectPropertySyntax
//@[2:6)     IdentifierSyntax
//@[2:6)      Identifier |kind|
//@[6:7)     Colon |:|
//@[8:19)     StringSyntax
//@[8:19)      StringComplete |'StorageV2'|
//@[19:20)    NewLine |\n|
  sku: {
//@[2:37)    ObjectPropertySyntax
//@[2:5)     IdentifierSyntax
//@[2:5)      Identifier |sku|
//@[5:6)     Colon |:|
//@[7:37)     ObjectSyntax
//@[7:8)      LeftBrace |{|
//@[8:9)      NewLine |\n|
    name: 'Standard_LRS'
//@[4:24)      ObjectPropertySyntax
//@[4:8)       IdentifierSyntax
//@[4:8)        Identifier |name|
//@[8:9)       Colon |:|
//@[10:24)       StringSyntax
//@[10:24)        StringComplete |'Standard_LRS'|
//@[24:25)      NewLine |\n|
  }
//@[2:3)      RightBrace |}|
//@[3:4)    NewLine |\n|
  dependsOn: [
//@[2:37)    ObjectPropertySyntax
//@[2:11)     IdentifierSyntax
//@[2:11)      Identifier |dependsOn|
//@[11:12)     Colon |:|
//@[13:37)     ArraySyntax
//@[13:14)      LeftSquare |[|
//@[14:15)      NewLine |\n|
    singleResource
//@[4:18)      ArrayItemSyntax
//@[4:18)       VariableAccessSyntax
//@[4:18)        IdentifierSyntax
//@[4:18)         Identifier |singleResource|
//@[18:19)      NewLine |\n|
  ]
//@[2:3)      RightSquare |]|
//@[3:4)    NewLine |\n|
}]
//@[0:1)    RightBrace |}|
//@[1:2)   RightSquare |]|
//@[2:4) NewLine |\n\n|

// extension of a single resource in a collection
//@[49:50) NewLine |\n|
resource extendSingleResourceInCollection 'Microsoft.Authorization/locks@2016-09-01' = {
//@[0:212) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:41)  IdentifierSyntax
//@[9:41)   Identifier |extendSingleResourceInCollection|
//@[42:84)  StringSyntax
//@[42:84)   StringComplete |'Microsoft.Authorization/locks@2016-09-01'|
//@[85:86)  Assignment |=|
//@[87:212)  ObjectSyntax
//@[87:88)   LeftBrace |{|
//@[88:89)   NewLine |\n|
  name: 'one-resource-collection-item-lock'
//@[2:43)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:43)    StringSyntax
//@[8:43)     StringComplete |'one-resource-collection-item-lock'|
//@[43:44)   NewLine |\n|
  properties: {
//@[2:41)   ObjectPropertySyntax
//@[2:12)    IdentifierSyntax
//@[2:12)     Identifier |properties|
//@[12:13)    Colon |:|
//@[14:41)    ObjectSyntax
//@[14:15)     LeftBrace |{|
//@[15:16)     NewLine |\n|
    level: 'ReadOnly'
//@[4:21)     ObjectPropertySyntax
//@[4:9)      IdentifierSyntax
//@[4:9)       Identifier |level|
//@[9:10)      Colon |:|
//@[11:21)      StringSyntax
//@[11:21)       StringComplete |'ReadOnly'|
//@[21:22)     NewLine |\n|
  }
//@[2:3)     RightBrace |}|
//@[3:4)   NewLine |\n|
  scope: storageAccounts[index % 2]
//@[2:35)   ObjectPropertySyntax
//@[2:7)    IdentifierSyntax
//@[2:7)     Identifier |scope|
//@[7:8)    Colon |:|
//@[9:35)    ArrayAccessSyntax
//@[9:24)     VariableAccessSyntax
//@[9:24)      IdentifierSyntax
//@[9:24)       Identifier |storageAccounts|
//@[24:25)     LeftSquare |[|
//@[25:34)     BinaryOperationSyntax
//@[25:30)      VariableAccessSyntax
//@[25:30)       IdentifierSyntax
//@[25:30)        Identifier |index|
//@[31:32)      Modulo |%|
//@[33:34)      IntegerLiteralSyntax
//@[33:34)       Integer |2|
//@[34:35)     RightSquare |]|
//@[35:36)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

// collection of extensions
//@[27:28) NewLine |\n|
resource extensionCollection 'Microsoft.Authorization/locks@2016-09-01' = [for (i, i2) in range(0,1): {
//@[0:235) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:28)  IdentifierSyntax
//@[9:28)   Identifier |extensionCollection|
//@[29:71)  StringSyntax
//@[29:71)   StringComplete |'Microsoft.Authorization/locks@2016-09-01'|
//@[72:73)  Assignment |=|
//@[74:235)  ForSyntax
//@[74:75)   LeftSquare |[|
//@[75:78)   Identifier |for|
//@[79:86)   ForVariableBlockSyntax
//@[79:80)    LeftParen |(|
//@[80:81)    LocalVariableSyntax
//@[80:81)     IdentifierSyntax
//@[80:81)      Identifier |i|
//@[81:82)    Comma |,|
//@[83:85)    LocalVariableSyntax
//@[83:85)     IdentifierSyntax
//@[83:85)      Identifier |i2|
//@[85:86)    RightParen |)|
//@[87:89)   Identifier |in|
//@[90:100)   FunctionCallSyntax
//@[90:95)    IdentifierSyntax
//@[90:95)     Identifier |range|
//@[95:96)    LeftParen |(|
//@[96:98)    FunctionArgumentSyntax
//@[96:97)     IntegerLiteralSyntax
//@[96:97)      Integer |0|
//@[97:98)     Comma |,|
//@[98:99)    FunctionArgumentSyntax
//@[98:99)     IntegerLiteralSyntax
//@[98:99)      Integer |1|
//@[99:100)    RightParen |)|
//@[100:101)   Colon |:|
//@[102:234)   ObjectSyntax
//@[102:103)    LeftBrace |{|
//@[103:104)    NewLine |\n|
  name: 'lock-${i}-${i2}'
//@[2:25)    ObjectPropertySyntax
//@[2:6)     IdentifierSyntax
//@[2:6)      Identifier |name|
//@[6:7)     Colon |:|
//@[8:25)     StringSyntax
//@[8:16)      StringLeftPiece |'lock-${|
//@[16:17)      VariableAccessSyntax
//@[16:17)       IdentifierSyntax
//@[16:17)        Identifier |i|
//@[17:21)      StringMiddlePiece |}-${|
//@[21:23)      VariableAccessSyntax
//@[21:23)       IdentifierSyntax
//@[21:23)        Identifier |i2|
//@[23:25)      StringRightPiece |}'|
//@[25:26)    NewLine |\n|
  properties: {
//@[2:78)    ObjectPropertySyntax
//@[2:12)     IdentifierSyntax
//@[2:12)      Identifier |properties|
//@[12:13)     Colon |:|
//@[14:78)     ObjectSyntax
//@[14:15)      LeftBrace |{|
//@[15:16)      NewLine |\n|
    level: i == 0 && i2 == 0 ? 'CanNotDelete' : 'ReadOnly'
//@[4:58)      ObjectPropertySyntax
//@[4:9)       IdentifierSyntax
//@[4:9)        Identifier |level|
//@[9:10)       Colon |:|
//@[11:58)       TernaryOperationSyntax
//@[11:28)        BinaryOperationSyntax
//@[11:17)         BinaryOperationSyntax
//@[11:12)          VariableAccessSyntax
//@[11:12)           IdentifierSyntax
//@[11:12)            Identifier |i|
//@[13:15)          Equals |==|
//@[16:17)          IntegerLiteralSyntax
//@[16:17)           Integer |0|
//@[18:20)         LogicalAnd |&&|
//@[21:28)         BinaryOperationSyntax
//@[21:23)          VariableAccessSyntax
//@[21:23)           IdentifierSyntax
//@[21:23)            Identifier |i2|
//@[24:26)          Equals |==|
//@[27:28)          IntegerLiteralSyntax
//@[27:28)           Integer |0|
//@[29:30)        Question |?|
//@[31:45)        StringSyntax
//@[31:45)         StringComplete |'CanNotDelete'|
//@[46:47)        Colon |:|
//@[48:58)        StringSyntax
//@[48:58)         StringComplete |'ReadOnly'|
//@[58:59)      NewLine |\n|
  }
//@[2:3)      RightBrace |}|
//@[3:4)    NewLine |\n|
  scope: singleResource
//@[2:23)    ObjectPropertySyntax
//@[2:7)     IdentifierSyntax
//@[2:7)      Identifier |scope|
//@[7:8)     Colon |:|
//@[9:23)     VariableAccessSyntax
//@[9:23)      IdentifierSyntax
//@[9:23)       Identifier |singleResource|
//@[23:24)    NewLine |\n|
}]
//@[0:1)    RightBrace |}|
//@[1:2)   RightSquare |]|
//@[2:4) NewLine |\n\n|

// cascade extend the extension
//@[31:32) NewLine |\n|
@batchSize(1)
//@[0:260) ResourceDeclarationSyntax
//@[0:13)  DecoratorSyntax
//@[0:1)   At |@|
//@[1:13)   FunctionCallSyntax
//@[1:10)    IdentifierSyntax
//@[1:10)     Identifier |batchSize|
//@[10:11)    LeftParen |(|
//@[11:12)    FunctionArgumentSyntax
//@[11:12)     IntegerLiteralSyntax
//@[11:12)      Integer |1|
//@[12:13)    RightParen |)|
//@[13:14)  NewLine |\n|
resource lockTheLocks 'Microsoft.Authorization/locks@2016-09-01' = [for (i, i2) in range(0,1): {
//@[0:8)  Identifier |resource|
//@[9:21)  IdentifierSyntax
//@[9:21)   Identifier |lockTheLocks|
//@[22:64)  StringSyntax
//@[22:64)   StringComplete |'Microsoft.Authorization/locks@2016-09-01'|
//@[65:66)  Assignment |=|
//@[67:246)  ForSyntax
//@[67:68)   LeftSquare |[|
//@[68:71)   Identifier |for|
//@[72:79)   ForVariableBlockSyntax
//@[72:73)    LeftParen |(|
//@[73:74)    LocalVariableSyntax
//@[73:74)     IdentifierSyntax
//@[73:74)      Identifier |i|
//@[74:75)    Comma |,|
//@[76:78)    LocalVariableSyntax
//@[76:78)     IdentifierSyntax
//@[76:78)      Identifier |i2|
//@[78:79)    RightParen |)|
//@[80:82)   Identifier |in|
//@[83:93)   FunctionCallSyntax
//@[83:88)    IdentifierSyntax
//@[83:88)     Identifier |range|
//@[88:89)    LeftParen |(|
//@[89:91)    FunctionArgumentSyntax
//@[89:90)     IntegerLiteralSyntax
//@[89:90)      Integer |0|
//@[90:91)     Comma |,|
//@[91:92)    FunctionArgumentSyntax
//@[91:92)     IntegerLiteralSyntax
//@[91:92)      Integer |1|
//@[92:93)    RightParen |)|
//@[93:94)   Colon |:|
//@[95:245)   ObjectSyntax
//@[95:96)    LeftBrace |{|
//@[96:97)    NewLine |\n|
  name: 'lock-the-lock-${i}-${i2}'
//@[2:34)    ObjectPropertySyntax
//@[2:6)     IdentifierSyntax
//@[2:6)      Identifier |name|
//@[6:7)     Colon |:|
//@[8:34)     StringSyntax
//@[8:25)      StringLeftPiece |'lock-the-lock-${|
//@[25:26)      VariableAccessSyntax
//@[25:26)       IdentifierSyntax
//@[25:26)        Identifier |i|
//@[26:30)      StringMiddlePiece |}-${|
//@[30:32)      VariableAccessSyntax
//@[30:32)       IdentifierSyntax
//@[30:32)        Identifier |i2|
//@[32:34)      StringRightPiece |}'|
//@[34:35)    NewLine |\n|
  properties: {
//@[2:78)    ObjectPropertySyntax
//@[2:12)     IdentifierSyntax
//@[2:12)      Identifier |properties|
//@[12:13)     Colon |:|
//@[14:78)     ObjectSyntax
//@[14:15)      LeftBrace |{|
//@[15:16)      NewLine |\n|
    level: i == 0 && i2 == 0 ? 'CanNotDelete' : 'ReadOnly'
//@[4:58)      ObjectPropertySyntax
//@[4:9)       IdentifierSyntax
//@[4:9)        Identifier |level|
//@[9:10)       Colon |:|
//@[11:58)       TernaryOperationSyntax
//@[11:28)        BinaryOperationSyntax
//@[11:17)         BinaryOperationSyntax
//@[11:12)          VariableAccessSyntax
//@[11:12)           IdentifierSyntax
//@[11:12)            Identifier |i|
//@[13:15)          Equals |==|
//@[16:17)          IntegerLiteralSyntax
//@[16:17)           Integer |0|
//@[18:20)         LogicalAnd |&&|
//@[21:28)         BinaryOperationSyntax
//@[21:23)          VariableAccessSyntax
//@[21:23)           IdentifierSyntax
//@[21:23)            Identifier |i2|
//@[24:26)          Equals |==|
//@[27:28)          IntegerLiteralSyntax
//@[27:28)           Integer |0|
//@[29:30)        Question |?|
//@[31:45)        StringSyntax
//@[31:45)         StringComplete |'CanNotDelete'|
//@[46:47)        Colon |:|
//@[48:58)        StringSyntax
//@[48:58)         StringComplete |'ReadOnly'|
//@[58:59)      NewLine |\n|
  }
//@[2:3)      RightBrace |}|
//@[3:4)    NewLine |\n|
  scope: extensionCollection[i2]
//@[2:32)    ObjectPropertySyntax
//@[2:7)     IdentifierSyntax
//@[2:7)      Identifier |scope|
//@[7:8)     Colon |:|
//@[9:32)     ArrayAccessSyntax
//@[9:28)      VariableAccessSyntax
//@[9:28)       IdentifierSyntax
//@[9:28)        Identifier |extensionCollection|
//@[28:29)      LeftSquare |[|
//@[29:31)      VariableAccessSyntax
//@[29:31)       IdentifierSyntax
//@[29:31)        Identifier |i2|
//@[31:32)      RightSquare |]|
//@[32:33)    NewLine |\n|
}]
//@[0:1)    RightBrace |}|
//@[1:2)   RightSquare |]|
//@[2:4) NewLine |\n\n|

// special case property access
//@[31:32) NewLine |\n|
output indexedCollectionBlobEndpoint string = storageAccounts[index].properties.primaryEndpoints.blob
//@[0:101) OutputDeclarationSyntax
//@[0:6)  Identifier |output|
//@[7:36)  IdentifierSyntax
//@[7:36)   Identifier |indexedCollectionBlobEndpoint|
//@[37:43)  TypeSyntax
//@[37:43)   Identifier |string|
//@[44:45)  Assignment |=|
//@[46:101)  PropertyAccessSyntax
//@[46:96)   PropertyAccessSyntax
//@[46:79)    PropertyAccessSyntax
//@[46:68)     ArrayAccessSyntax
//@[46:61)      VariableAccessSyntax
//@[46:61)       IdentifierSyntax
//@[46:61)        Identifier |storageAccounts|
//@[61:62)      LeftSquare |[|
//@[62:67)      VariableAccessSyntax
//@[62:67)       IdentifierSyntax
//@[62:67)        Identifier |index|
//@[67:68)      RightSquare |]|
//@[68:69)     Dot |.|
//@[69:79)     IdentifierSyntax
//@[69:79)      Identifier |properties|
//@[79:80)    Dot |.|
//@[80:96)    IdentifierSyntax
//@[80:96)     Identifier |primaryEndpoints|
//@[96:97)   Dot |.|
//@[97:101)   IdentifierSyntax
//@[97:101)    Identifier |blob|
//@[101:102) NewLine |\n|
output indexedCollectionName string = storageAccounts[index].name
//@[0:65) OutputDeclarationSyntax
//@[0:6)  Identifier |output|
//@[7:28)  IdentifierSyntax
//@[7:28)   Identifier |indexedCollectionName|
//@[29:35)  TypeSyntax
//@[29:35)   Identifier |string|
//@[36:37)  Assignment |=|
//@[38:65)  PropertyAccessSyntax
//@[38:60)   ArrayAccessSyntax
//@[38:53)    VariableAccessSyntax
//@[38:53)     IdentifierSyntax
//@[38:53)      Identifier |storageAccounts|
//@[53:54)    LeftSquare |[|
//@[54:59)    VariableAccessSyntax
//@[54:59)     IdentifierSyntax
//@[54:59)      Identifier |index|
//@[59:60)    RightSquare |]|
//@[60:61)   Dot |.|
//@[61:65)   IdentifierSyntax
//@[61:65)    Identifier |name|
//@[65:66) NewLine |\n|
output indexedCollectionId string = storageAccounts[index].id
//@[0:61) OutputDeclarationSyntax
//@[0:6)  Identifier |output|
//@[7:26)  IdentifierSyntax
//@[7:26)   Identifier |indexedCollectionId|
//@[27:33)  TypeSyntax
//@[27:33)   Identifier |string|
//@[34:35)  Assignment |=|
//@[36:61)  PropertyAccessSyntax
//@[36:58)   ArrayAccessSyntax
//@[36:51)    VariableAccessSyntax
//@[36:51)     IdentifierSyntax
//@[36:51)      Identifier |storageAccounts|
//@[51:52)    LeftSquare |[|
//@[52:57)    VariableAccessSyntax
//@[52:57)     IdentifierSyntax
//@[52:57)      Identifier |index|
//@[57:58)    RightSquare |]|
//@[58:59)   Dot |.|
//@[59:61)   IdentifierSyntax
//@[59:61)    Identifier |id|
//@[61:62) NewLine |\n|
output indexedCollectionType string = storageAccounts[index].type
//@[0:65) OutputDeclarationSyntax
//@[0:6)  Identifier |output|
//@[7:28)  IdentifierSyntax
//@[7:28)   Identifier |indexedCollectionType|
//@[29:35)  TypeSyntax
//@[29:35)   Identifier |string|
//@[36:37)  Assignment |=|
//@[38:65)  PropertyAccessSyntax
//@[38:60)   ArrayAccessSyntax
//@[38:53)    VariableAccessSyntax
//@[38:53)     IdentifierSyntax
//@[38:53)      Identifier |storageAccounts|
//@[53:54)    LeftSquare |[|
//@[54:59)    VariableAccessSyntax
//@[54:59)     IdentifierSyntax
//@[54:59)      Identifier |index|
//@[59:60)    RightSquare |]|
//@[60:61)   Dot |.|
//@[61:65)   IdentifierSyntax
//@[61:65)    Identifier |type|
//@[65:66) NewLine |\n|
output indexedCollectionVersion string = storageAccounts[index].apiVersion
//@[0:74) OutputDeclarationSyntax
//@[0:6)  Identifier |output|
//@[7:31)  IdentifierSyntax
//@[7:31)   Identifier |indexedCollectionVersion|
//@[32:38)  TypeSyntax
//@[32:38)   Identifier |string|
//@[39:40)  Assignment |=|
//@[41:74)  PropertyAccessSyntax
//@[41:63)   ArrayAccessSyntax
//@[41:56)    VariableAccessSyntax
//@[41:56)     IdentifierSyntax
//@[41:56)      Identifier |storageAccounts|
//@[56:57)    LeftSquare |[|
//@[57:62)    VariableAccessSyntax
//@[57:62)     IdentifierSyntax
//@[57:62)      Identifier |index|
//@[62:63)    RightSquare |]|
//@[63:64)   Dot |.|
//@[64:74)   IdentifierSyntax
//@[64:74)    Identifier |apiVersion|
//@[74:76) NewLine |\n\n|

// general case property access
//@[31:32) NewLine |\n|
output indexedCollectionIdentity object = storageAccounts[index].identity
//@[0:73) OutputDeclarationSyntax
//@[0:6)  Identifier |output|
//@[7:32)  IdentifierSyntax
//@[7:32)   Identifier |indexedCollectionIdentity|
//@[33:39)  TypeSyntax
//@[33:39)   Identifier |object|
//@[40:41)  Assignment |=|
//@[42:73)  PropertyAccessSyntax
//@[42:64)   ArrayAccessSyntax
//@[42:57)    VariableAccessSyntax
//@[42:57)     IdentifierSyntax
//@[42:57)      Identifier |storageAccounts|
//@[57:58)    LeftSquare |[|
//@[58:63)    VariableAccessSyntax
//@[58:63)     IdentifierSyntax
//@[58:63)      Identifier |index|
//@[63:64)    RightSquare |]|
//@[64:65)   Dot |.|
//@[65:73)   IdentifierSyntax
//@[65:73)    Identifier |identity|
//@[73:75) NewLine |\n\n|

// indexed access of two properties
//@[35:36) NewLine |\n|
output indexedEndpointPair object = {
//@[0:181) OutputDeclarationSyntax
//@[0:6)  Identifier |output|
//@[7:26)  IdentifierSyntax
//@[7:26)   Identifier |indexedEndpointPair|
//@[27:33)  TypeSyntax
//@[27:33)   Identifier |object|
//@[34:35)  Assignment |=|
//@[36:181)  ObjectSyntax
//@[36:37)   LeftBrace |{|
//@[37:38)   NewLine |\n|
  primary: storageAccounts[index].properties.primaryEndpoints.blob
//@[2:66)   ObjectPropertySyntax
//@[2:9)    IdentifierSyntax
//@[2:9)     Identifier |primary|
//@[9:10)    Colon |:|
//@[11:66)    PropertyAccessSyntax
//@[11:61)     PropertyAccessSyntax
//@[11:44)      PropertyAccessSyntax
//@[11:33)       ArrayAccessSyntax
//@[11:26)        VariableAccessSyntax
//@[11:26)         IdentifierSyntax
//@[11:26)          Identifier |storageAccounts|
//@[26:27)        LeftSquare |[|
//@[27:32)        VariableAccessSyntax
//@[27:32)         IdentifierSyntax
//@[27:32)          Identifier |index|
//@[32:33)        RightSquare |]|
//@[33:34)       Dot |.|
//@[34:44)       IdentifierSyntax
//@[34:44)        Identifier |properties|
//@[44:45)      Dot |.|
//@[45:61)      IdentifierSyntax
//@[45:61)       Identifier |primaryEndpoints|
//@[61:62)     Dot |.|
//@[62:66)     IdentifierSyntax
//@[62:66)      Identifier |blob|
//@[66:67)   NewLine |\n|
  secondary: storageAccounts[index + 1].properties.secondaryEndpoints.blob
//@[2:74)   ObjectPropertySyntax
//@[2:11)    IdentifierSyntax
//@[2:11)     Identifier |secondary|
//@[11:12)    Colon |:|
//@[13:74)    PropertyAccessSyntax
//@[13:69)     PropertyAccessSyntax
//@[13:50)      PropertyAccessSyntax
//@[13:39)       ArrayAccessSyntax
//@[13:28)        VariableAccessSyntax
//@[13:28)         IdentifierSyntax
//@[13:28)          Identifier |storageAccounts|
//@[28:29)        LeftSquare |[|
//@[29:38)        BinaryOperationSyntax
//@[29:34)         VariableAccessSyntax
//@[29:34)          IdentifierSyntax
//@[29:34)           Identifier |index|
//@[35:36)         Plus |+|
//@[37:38)         IntegerLiteralSyntax
//@[37:38)          Integer |1|
//@[38:39)        RightSquare |]|
//@[39:40)       Dot |.|
//@[40:50)       IdentifierSyntax
//@[40:50)        Identifier |properties|
//@[50:51)      Dot |.|
//@[51:69)      IdentifierSyntax
//@[51:69)       Identifier |secondaryEndpoints|
//@[69:70)     Dot |.|
//@[70:74)     IdentifierSyntax
//@[70:74)      Identifier |blob|
//@[74:75)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

// nested indexer?
//@[18:19) NewLine |\n|
output indexViaReference string = storageAccounts[int(storageAccounts[index].properties.creationTime)].properties.accessTier
//@[0:124) OutputDeclarationSyntax
//@[0:6)  Identifier |output|
//@[7:24)  IdentifierSyntax
//@[7:24)   Identifier |indexViaReference|
//@[25:31)  TypeSyntax
//@[25:31)   Identifier |string|
//@[32:33)  Assignment |=|
//@[34:124)  PropertyAccessSyntax
//@[34:113)   PropertyAccessSyntax
//@[34:102)    ArrayAccessSyntax
//@[34:49)     VariableAccessSyntax
//@[34:49)      IdentifierSyntax
//@[34:49)       Identifier |storageAccounts|
//@[49:50)     LeftSquare |[|
//@[50:101)     FunctionCallSyntax
//@[50:53)      IdentifierSyntax
//@[50:53)       Identifier |int|
//@[53:54)      LeftParen |(|
//@[54:100)      FunctionArgumentSyntax
//@[54:100)       PropertyAccessSyntax
//@[54:87)        PropertyAccessSyntax
//@[54:76)         ArrayAccessSyntax
//@[54:69)          VariableAccessSyntax
//@[54:69)           IdentifierSyntax
//@[54:69)            Identifier |storageAccounts|
//@[69:70)          LeftSquare |[|
//@[70:75)          VariableAccessSyntax
//@[70:75)           IdentifierSyntax
//@[70:75)            Identifier |index|
//@[75:76)          RightSquare |]|
//@[76:77)         Dot |.|
//@[77:87)         IdentifierSyntax
//@[77:87)          Identifier |properties|
//@[87:88)        Dot |.|
//@[88:100)        IdentifierSyntax
//@[88:100)         Identifier |creationTime|
//@[100:101)      RightParen |)|
//@[101:102)     RightSquare |]|
//@[102:103)    Dot |.|
//@[103:113)    IdentifierSyntax
//@[103:113)     Identifier |properties|
//@[113:114)   Dot |.|
//@[114:124)   IdentifierSyntax
//@[114:124)    Identifier |accessTier|
//@[124:126) NewLine |\n\n|

// dependency on a resource collection
//@[38:39) NewLine |\n|
resource storageAccounts2 'Microsoft.Storage/storageAccounts@2019-06-01' = [for (account, idx) in accounts: {
//@[0:290) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:25)  IdentifierSyntax
//@[9:25)   Identifier |storageAccounts2|
//@[26:72)  StringSyntax
//@[26:72)   StringComplete |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[73:74)  Assignment |=|
//@[75:290)  ForSyntax
//@[75:76)   LeftSquare |[|
//@[76:79)   Identifier |for|
//@[80:94)   ForVariableBlockSyntax
//@[80:81)    LeftParen |(|
//@[81:88)    LocalVariableSyntax
//@[81:88)     IdentifierSyntax
//@[81:88)      Identifier |account|
//@[88:89)    Comma |,|
//@[90:93)    LocalVariableSyntax
//@[90:93)     IdentifierSyntax
//@[90:93)      Identifier |idx|
//@[93:94)    RightParen |)|
//@[95:97)   Identifier |in|
//@[98:106)   VariableAccessSyntax
//@[98:106)    IdentifierSyntax
//@[98:106)     Identifier |accounts|
//@[106:107)   Colon |:|
//@[108:289)   ObjectSyntax
//@[108:109)    LeftBrace |{|
//@[109:110)    NewLine |\n|
  name: '${name}-collection-${account.name}-${idx}'
//@[2:51)    ObjectPropertySyntax
//@[2:6)     IdentifierSyntax
//@[2:6)      Identifier |name|
//@[6:7)     Colon |:|
//@[8:51)     StringSyntax
//@[8:11)      StringLeftPiece |'${|
//@[11:15)      VariableAccessSyntax
//@[11:15)       IdentifierSyntax
//@[11:15)        Identifier |name|
//@[15:30)      StringMiddlePiece |}-collection-${|
//@[30:42)      PropertyAccessSyntax
//@[30:37)       VariableAccessSyntax
//@[30:37)        IdentifierSyntax
//@[30:37)         Identifier |account|
//@[37:38)       Dot |.|
//@[38:42)       IdentifierSyntax
//@[38:42)        Identifier |name|
//@[42:46)      StringMiddlePiece |}-${|
//@[46:49)      VariableAccessSyntax
//@[46:49)       IdentifierSyntax
//@[46:49)        Identifier |idx|
//@[49:51)      StringRightPiece |}'|
//@[51:52)    NewLine |\n|
  location: account.location
//@[2:28)    ObjectPropertySyntax
//@[2:10)     IdentifierSyntax
//@[2:10)      Identifier |location|
//@[10:11)     Colon |:|
//@[12:28)     PropertyAccessSyntax
//@[12:19)      VariableAccessSyntax
//@[12:19)       IdentifierSyntax
//@[12:19)        Identifier |account|
//@[19:20)      Dot |.|
//@[20:28)      IdentifierSyntax
//@[20:28)       Identifier |location|
//@[28:29)    NewLine |\n|
  kind: 'StorageV2'
//@[2:19)    ObjectPropertySyntax
//@[2:6)     IdentifierSyntax
//@[2:6)      Identifier |kind|
//@[6:7)     Colon |:|
//@[8:19)     StringSyntax
//@[8:19)      StringComplete |'StorageV2'|
//@[19:20)    NewLine |\n|
  sku: {
//@[2:37)    ObjectPropertySyntax
//@[2:5)     IdentifierSyntax
//@[2:5)      Identifier |sku|
//@[5:6)     Colon |:|
//@[7:37)     ObjectSyntax
//@[7:8)      LeftBrace |{|
//@[8:9)      NewLine |\n|
    name: 'Standard_LRS'
//@[4:24)      ObjectPropertySyntax
//@[4:8)       IdentifierSyntax
//@[4:8)        Identifier |name|
//@[8:9)       Colon |:|
//@[10:24)       StringSyntax
//@[10:24)        StringComplete |'Standard_LRS'|
//@[24:25)      NewLine |\n|
  }
//@[2:3)      RightBrace |}|
//@[3:4)    NewLine |\n|
  dependsOn: [
//@[2:38)    ObjectPropertySyntax
//@[2:11)     IdentifierSyntax
//@[2:11)      Identifier |dependsOn|
//@[11:12)     Colon |:|
//@[13:38)     ArraySyntax
//@[13:14)      LeftSquare |[|
//@[14:15)      NewLine |\n|
    storageAccounts
//@[4:19)      ArrayItemSyntax
//@[4:19)       VariableAccessSyntax
//@[4:19)        IdentifierSyntax
//@[4:19)         Identifier |storageAccounts|
//@[19:20)      NewLine |\n|
  ]
//@[2:3)      RightSquare |]|
//@[3:4)    NewLine |\n|
}]
//@[0:1)    RightBrace |}|
//@[1:2)   RightSquare |]|
//@[2:4) NewLine |\n\n|

// one-to-one paired dependencies
//@[33:34) NewLine |\n|
resource firstSet 'Microsoft.Storage/storageAccounts@2019-06-01' = [for (i,ii) in range(0, length(accounts)): {
//@[0:243) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:17)  IdentifierSyntax
//@[9:17)   Identifier |firstSet|
//@[18:64)  StringSyntax
//@[18:64)   StringComplete |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[65:66)  Assignment |=|
//@[67:243)  ForSyntax
//@[67:68)   LeftSquare |[|
//@[68:71)   Identifier |for|
//@[72:78)   ForVariableBlockSyntax
//@[72:73)    LeftParen |(|
//@[73:74)    LocalVariableSyntax
//@[73:74)     IdentifierSyntax
//@[73:74)      Identifier |i|
//@[74:75)    Comma |,|
//@[75:77)    LocalVariableSyntax
//@[75:77)     IdentifierSyntax
//@[75:77)      Identifier |ii|
//@[77:78)    RightParen |)|
//@[79:81)   Identifier |in|
//@[82:108)   FunctionCallSyntax
//@[82:87)    IdentifierSyntax
//@[82:87)     Identifier |range|
//@[87:88)    LeftParen |(|
//@[88:90)    FunctionArgumentSyntax
//@[88:89)     IntegerLiteralSyntax
//@[88:89)      Integer |0|
//@[89:90)     Comma |,|
//@[91:107)    FunctionArgumentSyntax
//@[91:107)     FunctionCallSyntax
//@[91:97)      IdentifierSyntax
//@[91:97)       Identifier |length|
//@[97:98)      LeftParen |(|
//@[98:106)      FunctionArgumentSyntax
//@[98:106)       VariableAccessSyntax
//@[98:106)        IdentifierSyntax
//@[98:106)         Identifier |accounts|
//@[106:107)      RightParen |)|
//@[107:108)    RightParen |)|
//@[108:109)   Colon |:|
//@[110:242)   ObjectSyntax
//@[110:111)    LeftBrace |{|
//@[111:112)    NewLine |\n|
  name: '${name}-set1-${i}-${ii}'
//@[2:33)    ObjectPropertySyntax
//@[2:6)     IdentifierSyntax
//@[2:6)      Identifier |name|
//@[6:7)     Colon |:|
//@[8:33)     StringSyntax
//@[8:11)      StringLeftPiece |'${|
//@[11:15)      VariableAccessSyntax
//@[11:15)       IdentifierSyntax
//@[11:15)        Identifier |name|
//@[15:24)      StringMiddlePiece |}-set1-${|
//@[24:25)      VariableAccessSyntax
//@[24:25)       IdentifierSyntax
//@[24:25)        Identifier |i|
//@[25:29)      StringMiddlePiece |}-${|
//@[29:31)      VariableAccessSyntax
//@[29:31)       IdentifierSyntax
//@[29:31)        Identifier |ii|
//@[31:33)      StringRightPiece |}'|
//@[33:34)    NewLine |\n|
  location: resourceGroup().location
//@[2:36)    ObjectPropertySyntax
//@[2:10)     IdentifierSyntax
//@[2:10)      Identifier |location|
//@[10:11)     Colon |:|
//@[12:36)     PropertyAccessSyntax
//@[12:27)      FunctionCallSyntax
//@[12:25)       IdentifierSyntax
//@[12:25)        Identifier |resourceGroup|
//@[25:26)       LeftParen |(|
//@[26:27)       RightParen |)|
//@[27:28)      Dot |.|
//@[28:36)      IdentifierSyntax
//@[28:36)       Identifier |location|
//@[36:37)    NewLine |\n|
  kind: 'StorageV2'
//@[2:19)    ObjectPropertySyntax
//@[2:6)     IdentifierSyntax
//@[2:6)      Identifier |kind|
//@[6:7)     Colon |:|
//@[8:19)     StringSyntax
//@[8:19)      StringComplete |'StorageV2'|
//@[19:20)    NewLine |\n|
  sku: {
//@[2:37)    ObjectPropertySyntax
//@[2:5)     IdentifierSyntax
//@[2:5)      Identifier |sku|
//@[5:6)     Colon |:|
//@[7:37)     ObjectSyntax
//@[7:8)      LeftBrace |{|
//@[8:9)      NewLine |\n|
    name: 'Standard_LRS'
//@[4:24)      ObjectPropertySyntax
//@[4:8)       IdentifierSyntax
//@[4:8)        Identifier |name|
//@[8:9)       Colon |:|
//@[10:24)       StringSyntax
//@[10:24)        StringComplete |'Standard_LRS'|
//@[24:25)      NewLine |\n|
  }
//@[2:3)      RightBrace |}|
//@[3:4)    NewLine |\n|
}]
//@[0:1)    RightBrace |}|
//@[1:2)   RightSquare |]|
//@[2:4) NewLine |\n\n|

resource secondSet 'Microsoft.Storage/storageAccounts@2019-06-01' = [for (i,iii) in range(0, length(accounts)): {
//@[0:283) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:18)  IdentifierSyntax
//@[9:18)   Identifier |secondSet|
//@[19:65)  StringSyntax
//@[19:65)   StringComplete |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[66:67)  Assignment |=|
//@[68:283)  ForSyntax
//@[68:69)   LeftSquare |[|
//@[69:72)   Identifier |for|
//@[73:80)   ForVariableBlockSyntax
//@[73:74)    LeftParen |(|
//@[74:75)    LocalVariableSyntax
//@[74:75)     IdentifierSyntax
//@[74:75)      Identifier |i|
//@[75:76)    Comma |,|
//@[76:79)    LocalVariableSyntax
//@[76:79)     IdentifierSyntax
//@[76:79)      Identifier |iii|
//@[79:80)    RightParen |)|
//@[81:83)   Identifier |in|
//@[84:110)   FunctionCallSyntax
//@[84:89)    IdentifierSyntax
//@[84:89)     Identifier |range|
//@[89:90)    LeftParen |(|
//@[90:92)    FunctionArgumentSyntax
//@[90:91)     IntegerLiteralSyntax
//@[90:91)      Integer |0|
//@[91:92)     Comma |,|
//@[93:109)    FunctionArgumentSyntax
//@[93:109)     FunctionCallSyntax
//@[93:99)      IdentifierSyntax
//@[93:99)       Identifier |length|
//@[99:100)      LeftParen |(|
//@[100:108)      FunctionArgumentSyntax
//@[100:108)       VariableAccessSyntax
//@[100:108)        IdentifierSyntax
//@[100:108)         Identifier |accounts|
//@[108:109)      RightParen |)|
//@[109:110)    RightParen |)|
//@[110:111)   Colon |:|
//@[112:282)   ObjectSyntax
//@[112:113)    LeftBrace |{|
//@[113:114)    NewLine |\n|
  name: '${name}-set2-${i}-${iii}'
//@[2:34)    ObjectPropertySyntax
//@[2:6)     IdentifierSyntax
//@[2:6)      Identifier |name|
//@[6:7)     Colon |:|
//@[8:34)     StringSyntax
//@[8:11)      StringLeftPiece |'${|
//@[11:15)      VariableAccessSyntax
//@[11:15)       IdentifierSyntax
//@[11:15)        Identifier |name|
//@[15:24)      StringMiddlePiece |}-set2-${|
//@[24:25)      VariableAccessSyntax
//@[24:25)       IdentifierSyntax
//@[24:25)        Identifier |i|
//@[25:29)      StringMiddlePiece |}-${|
//@[29:32)      VariableAccessSyntax
//@[29:32)       IdentifierSyntax
//@[29:32)        Identifier |iii|
//@[32:34)      StringRightPiece |}'|
//@[34:35)    NewLine |\n|
  location: resourceGroup().location
//@[2:36)    ObjectPropertySyntax
//@[2:10)     IdentifierSyntax
//@[2:10)      Identifier |location|
//@[10:11)     Colon |:|
//@[12:36)     PropertyAccessSyntax
//@[12:27)      FunctionCallSyntax
//@[12:25)       IdentifierSyntax
//@[12:25)        Identifier |resourceGroup|
//@[25:26)       LeftParen |(|
//@[26:27)       RightParen |)|
//@[27:28)      Dot |.|
//@[28:36)      IdentifierSyntax
//@[28:36)       Identifier |location|
//@[36:37)    NewLine |\n|
  kind: 'StorageV2'
//@[2:19)    ObjectPropertySyntax
//@[2:6)     IdentifierSyntax
//@[2:6)      Identifier |kind|
//@[6:7)     Colon |:|
//@[8:19)     StringSyntax
//@[8:19)      StringComplete |'StorageV2'|
//@[19:20)    NewLine |\n|
  sku: {
//@[2:37)    ObjectPropertySyntax
//@[2:5)     IdentifierSyntax
//@[2:5)      Identifier |sku|
//@[5:6)     Colon |:|
//@[7:37)     ObjectSyntax
//@[7:8)      LeftBrace |{|
//@[8:9)      NewLine |\n|
    name: 'Standard_LRS'
//@[4:24)      ObjectPropertySyntax
//@[4:8)       IdentifierSyntax
//@[4:8)        Identifier |name|
//@[8:9)       Colon |:|
//@[10:24)       StringSyntax
//@[10:24)        StringComplete |'Standard_LRS'|
//@[24:25)      NewLine |\n|
  }
//@[2:3)      RightBrace |}|
//@[3:4)    NewLine |\n|
  dependsOn: [
//@[2:36)    ObjectPropertySyntax
//@[2:11)     IdentifierSyntax
//@[2:11)      Identifier |dependsOn|
//@[11:12)     Colon |:|
//@[13:36)     ArraySyntax
//@[13:14)      LeftSquare |[|
//@[14:15)      NewLine |\n|
    firstSet[iii]
//@[4:17)      ArrayItemSyntax
//@[4:17)       ArrayAccessSyntax
//@[4:12)        VariableAccessSyntax
//@[4:12)         IdentifierSyntax
//@[4:12)          Identifier |firstSet|
//@[12:13)        LeftSquare |[|
//@[13:16)        VariableAccessSyntax
//@[13:16)         IdentifierSyntax
//@[13:16)          Identifier |iii|
//@[16:17)        RightSquare |]|
//@[17:18)      NewLine |\n|
  ]
//@[2:3)      RightSquare |]|
//@[3:4)    NewLine |\n|
}]
//@[0:1)    RightBrace |}|
//@[1:2)   RightSquare |]|
//@[2:4) NewLine |\n\n|

// depending on collection and one resource in the collection optimizes the latter part away
//@[92:93) NewLine |\n|
resource anotherSingleResource 'Microsoft.Storage/storageAccounts@2019-06-01' = {
//@[0:266) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:30)  IdentifierSyntax
//@[9:30)   Identifier |anotherSingleResource|
//@[31:77)  StringSyntax
//@[31:77)   StringComplete |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[78:79)  Assignment |=|
//@[80:266)  ObjectSyntax
//@[80:81)   LeftBrace |{|
//@[81:82)   NewLine |\n|
  name: '${name}single-resource-name'
//@[2:37)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:37)    StringSyntax
//@[8:11)     StringLeftPiece |'${|
//@[11:15)     VariableAccessSyntax
//@[11:15)      IdentifierSyntax
//@[11:15)       Identifier |name|
//@[15:37)     StringRightPiece |}single-resource-name'|
//@[37:38)   NewLine |\n|
  location: resourceGroup().location
//@[2:36)   ObjectPropertySyntax
//@[2:10)    IdentifierSyntax
//@[2:10)     Identifier |location|
//@[10:11)    Colon |:|
//@[12:36)    PropertyAccessSyntax
//@[12:27)     FunctionCallSyntax
//@[12:25)      IdentifierSyntax
//@[12:25)       Identifier |resourceGroup|
//@[25:26)      LeftParen |(|
//@[26:27)      RightParen |)|
//@[27:28)     Dot |.|
//@[28:36)     IdentifierSyntax
//@[28:36)      Identifier |location|
//@[36:37)   NewLine |\n|
  kind: 'StorageV2'
//@[2:19)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |kind|
//@[6:7)    Colon |:|
//@[8:19)    StringSyntax
//@[8:19)     StringComplete |'StorageV2'|
//@[19:20)   NewLine |\n|
  sku: {
//@[2:37)   ObjectPropertySyntax
//@[2:5)    IdentifierSyntax
//@[2:5)     Identifier |sku|
//@[5:6)    Colon |:|
//@[7:37)    ObjectSyntax
//@[7:8)     LeftBrace |{|
//@[8:9)     NewLine |\n|
    name: 'Standard_LRS'
//@[4:24)     ObjectPropertySyntax
//@[4:8)      IdentifierSyntax
//@[4:8)       Identifier |name|
//@[8:9)      Colon |:|
//@[10:24)      StringSyntax
//@[10:24)       StringComplete |'Standard_LRS'|
//@[24:25)     NewLine |\n|
  }
//@[2:3)     RightBrace |}|
//@[3:4)   NewLine |\n|
  dependsOn: [
//@[2:49)   ObjectPropertySyntax
//@[2:11)    IdentifierSyntax
//@[2:11)     Identifier |dependsOn|
//@[11:12)    Colon |:|
//@[13:49)    ArraySyntax
//@[13:14)     LeftSquare |[|
//@[14:15)     NewLine |\n|
    secondSet
//@[4:13)     ArrayItemSyntax
//@[4:13)      VariableAccessSyntax
//@[4:13)       IdentifierSyntax
//@[4:13)        Identifier |secondSet|
//@[13:14)     NewLine |\n|
    secondSet[0]
//@[4:16)     ArrayItemSyntax
//@[4:16)      ArrayAccessSyntax
//@[4:13)       VariableAccessSyntax
//@[4:13)        IdentifierSyntax
//@[4:13)         Identifier |secondSet|
//@[13:14)       LeftSquare |[|
//@[14:15)       IntegerLiteralSyntax
//@[14:15)        Integer |0|
//@[15:16)       RightSquare |]|
//@[16:17)     NewLine |\n|
  ]
//@[2:3)     RightSquare |]|
//@[3:4)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

// vnets
//@[8:9) NewLine |\n|
var vnetConfigurations = [
//@[0:138) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:22)  IdentifierSyntax
//@[4:22)   Identifier |vnetConfigurations|
//@[23:24)  Assignment |=|
//@[25:138)  ArraySyntax
//@[25:26)   LeftSquare |[|
//@[26:27)   NewLine |\n|
  {
//@[2:62)   ArrayItemSyntax
//@[2:62)    ObjectSyntax
//@[2:3)     LeftBrace |{|
//@[3:4)     NewLine |\n|
    name: 'one'
//@[4:15)     ObjectPropertySyntax
//@[4:8)      IdentifierSyntax
//@[4:8)       Identifier |name|
//@[8:9)      Colon |:|
//@[10:15)      StringSyntax
//@[10:15)       StringComplete |'one'|
//@[15:16)     NewLine |\n|
    location: resourceGroup().location
//@[4:38)     ObjectPropertySyntax
//@[4:12)      IdentifierSyntax
//@[4:12)       Identifier |location|
//@[12:13)      Colon |:|
//@[14:38)      PropertyAccessSyntax
//@[14:29)       FunctionCallSyntax
//@[14:27)        IdentifierSyntax
//@[14:27)         Identifier |resourceGroup|
//@[27:28)        LeftParen |(|
//@[28:29)        RightParen |)|
//@[29:30)       Dot |.|
//@[30:38)       IdentifierSyntax
//@[30:38)        Identifier |location|
//@[38:39)     NewLine |\n|
  }
//@[2:3)     RightBrace |}|
//@[3:4)   NewLine |\n|
  {
//@[2:46)   ArrayItemSyntax
//@[2:46)    ObjectSyntax
//@[2:3)     LeftBrace |{|
//@[3:4)     NewLine |\n|
    name: 'two'
//@[4:15)     ObjectPropertySyntax
//@[4:8)      IdentifierSyntax
//@[4:8)       Identifier |name|
//@[8:9)      Colon |:|
//@[10:15)      StringSyntax
//@[10:15)       StringComplete |'two'|
//@[15:16)     NewLine |\n|
    location: 'westus'
//@[4:22)     ObjectPropertySyntax
//@[4:12)      IdentifierSyntax
//@[4:12)       Identifier |location|
//@[12:13)      Colon |:|
//@[14:22)      StringSyntax
//@[14:22)       StringComplete |'westus'|
//@[22:23)     NewLine |\n|
  }
//@[2:3)     RightBrace |}|
//@[3:4)   NewLine |\n|
]
//@[0:1)   RightSquare |]|
//@[1:3) NewLine |\n\n|

resource vnets 'Microsoft.Network/virtualNetworks@2020-06-01' = [for (vnetConfig, index) in vnetConfigurations: {
//@[0:186) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:14)  IdentifierSyntax
//@[9:14)   Identifier |vnets|
//@[15:61)  StringSyntax
//@[15:61)   StringComplete |'Microsoft.Network/virtualNetworks@2020-06-01'|
//@[62:63)  Assignment |=|
//@[64:186)  ForSyntax
//@[64:65)   LeftSquare |[|
//@[65:68)   Identifier |for|
//@[69:88)   ForVariableBlockSyntax
//@[69:70)    LeftParen |(|
//@[70:80)    LocalVariableSyntax
//@[70:80)     IdentifierSyntax
//@[70:80)      Identifier |vnetConfig|
//@[80:81)    Comma |,|
//@[82:87)    LocalVariableSyntax
//@[82:87)     IdentifierSyntax
//@[82:87)      Identifier |index|
//@[87:88)    RightParen |)|
//@[89:91)   Identifier |in|
//@[92:110)   VariableAccessSyntax
//@[92:110)    IdentifierSyntax
//@[92:110)     Identifier |vnetConfigurations|
//@[110:111)   Colon |:|
//@[112:185)   ObjectSyntax
//@[112:113)    LeftBrace |{|
//@[113:114)    NewLine |\n|
  name: '${vnetConfig.name}-${index}'
//@[2:37)    ObjectPropertySyntax
//@[2:6)     IdentifierSyntax
//@[2:6)      Identifier |name|
//@[6:7)     Colon |:|
//@[8:37)     StringSyntax
//@[8:11)      StringLeftPiece |'${|
//@[11:26)      PropertyAccessSyntax
//@[11:21)       VariableAccessSyntax
//@[11:21)        IdentifierSyntax
//@[11:21)         Identifier |vnetConfig|
//@[21:22)       Dot |.|
//@[22:26)       IdentifierSyntax
//@[22:26)        Identifier |name|
//@[26:30)      StringMiddlePiece |}-${|
//@[30:35)      VariableAccessSyntax
//@[30:35)       IdentifierSyntax
//@[30:35)        Identifier |index|
//@[35:37)      StringRightPiece |}'|
//@[37:38)    NewLine |\n|
  location: vnetConfig.location
//@[2:31)    ObjectPropertySyntax
//@[2:10)     IdentifierSyntax
//@[2:10)      Identifier |location|
//@[10:11)     Colon |:|
//@[12:31)     PropertyAccessSyntax
//@[12:22)      VariableAccessSyntax
//@[12:22)       IdentifierSyntax
//@[12:22)        Identifier |vnetConfig|
//@[22:23)      Dot |.|
//@[23:31)      IdentifierSyntax
//@[23:31)       Identifier |location|
//@[31:32)    NewLine |\n|
}]
//@[0:1)    RightBrace |}|
//@[1:2)   RightSquare |]|
//@[2:4) NewLine |\n\n|

// implicit dependency on single resource from a resource collection
//@[68:69) NewLine |\n|
resource implicitDependencyOnSingleResourceByIndex 'Microsoft.Network/dnsZones@2018-05-01' = {
//@[0:237) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:50)  IdentifierSyntax
//@[9:50)   Identifier |implicitDependencyOnSingleResourceByIndex|
//@[51:90)  StringSyntax
//@[51:90)   StringComplete |'Microsoft.Network/dnsZones@2018-05-01'|
//@[91:92)  Assignment |=|
//@[93:237)  ObjectSyntax
//@[93:94)   LeftBrace |{|
//@[94:95)   NewLine |\n|
  name: 'test'
//@[2:14)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:14)    StringSyntax
//@[8:14)     StringComplete |'test'|
//@[14:15)   NewLine |\n|
  location: 'global'
//@[2:20)   ObjectPropertySyntax
//@[2:10)    IdentifierSyntax
//@[2:10)     Identifier |location|
//@[10:11)    Colon |:|
//@[12:20)    StringSyntax
//@[12:20)     StringComplete |'global'|
//@[20:21)   NewLine |\n|
  properties: {
//@[2:104)   ObjectPropertySyntax
//@[2:12)    IdentifierSyntax
//@[2:12)     Identifier |properties|
//@[12:13)    Colon |:|
//@[14:104)    ObjectSyntax
//@[14:15)     LeftBrace |{|
//@[15:16)     NewLine |\n|
    resolutionVirtualNetworks: [
//@[4:84)     ObjectPropertySyntax
//@[4:29)      IdentifierSyntax
//@[4:29)       Identifier |resolutionVirtualNetworks|
//@[29:30)      Colon |:|
//@[31:84)      ArraySyntax
//@[31:32)       LeftSquare |[|
//@[32:33)       NewLine |\n|
      {
//@[6:45)       ArrayItemSyntax
//@[6:45)        ObjectSyntax
//@[6:7)         LeftBrace |{|
//@[7:8)         NewLine |\n|
        id: vnets[index+1].id
//@[8:29)         ObjectPropertySyntax
//@[8:10)          IdentifierSyntax
//@[8:10)           Identifier |id|
//@[10:11)          Colon |:|
//@[12:29)          PropertyAccessSyntax
//@[12:26)           ArrayAccessSyntax
//@[12:17)            VariableAccessSyntax
//@[12:17)             IdentifierSyntax
//@[12:17)              Identifier |vnets|
//@[17:18)            LeftSquare |[|
//@[18:25)            BinaryOperationSyntax
//@[18:23)             VariableAccessSyntax
//@[18:23)              IdentifierSyntax
//@[18:23)               Identifier |index|
//@[23:24)             Plus |+|
//@[24:25)             IntegerLiteralSyntax
//@[24:25)              Integer |1|
//@[25:26)            RightSquare |]|
//@[26:27)           Dot |.|
//@[27:29)           IdentifierSyntax
//@[27:29)            Identifier |id|
//@[29:30)         NewLine |\n|
      }
//@[6:7)         RightBrace |}|
//@[7:8)       NewLine |\n|
    ]
//@[4:5)       RightSquare |]|
//@[5:6)     NewLine |\n|
  }
//@[2:3)     RightBrace |}|
//@[3:4)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

// implicit and explicit dependency combined
//@[44:45) NewLine |\n|
resource combinedDependencies 'Microsoft.Network/dnsZones@2018-05-01' = {
//@[0:294) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:29)  IdentifierSyntax
//@[9:29)   Identifier |combinedDependencies|
//@[30:69)  StringSyntax
//@[30:69)   StringComplete |'Microsoft.Network/dnsZones@2018-05-01'|
//@[70:71)  Assignment |=|
//@[72:294)  ObjectSyntax
//@[72:73)   LeftBrace |{|
//@[73:74)   NewLine |\n|
  name: 'test2'
//@[2:15)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:15)    StringSyntax
//@[8:15)     StringComplete |'test2'|
//@[15:16)   NewLine |\n|
  location: 'global'
//@[2:20)   ObjectPropertySyntax
//@[2:10)    IdentifierSyntax
//@[2:10)     Identifier |location|
//@[10:11)    Colon |:|
//@[12:20)    StringSyntax
//@[12:20)     StringComplete |'global'|
//@[20:21)   NewLine |\n|
  properties: {
//@[2:152)   ObjectPropertySyntax
//@[2:12)    IdentifierSyntax
//@[2:12)     Identifier |properties|
//@[12:13)    Colon |:|
//@[14:152)    ObjectSyntax
//@[14:15)     LeftBrace |{|
//@[15:16)     NewLine |\n|
    resolutionVirtualNetworks: [
//@[4:132)     ObjectPropertySyntax
//@[4:29)      IdentifierSyntax
//@[4:29)       Identifier |resolutionVirtualNetworks|
//@[29:30)      Colon |:|
//@[31:132)      ArraySyntax
//@[31:32)       LeftSquare |[|
//@[32:33)       NewLine |\n|
      {
//@[6:45)       ArrayItemSyntax
//@[6:45)        ObjectSyntax
//@[6:7)         LeftBrace |{|
//@[7:8)         NewLine |\n|
        id: vnets[index-1].id
//@[8:29)         ObjectPropertySyntax
//@[8:10)          IdentifierSyntax
//@[8:10)           Identifier |id|
//@[10:11)          Colon |:|
//@[12:29)          PropertyAccessSyntax
//@[12:26)           ArrayAccessSyntax
//@[12:17)            VariableAccessSyntax
//@[12:17)             IdentifierSyntax
//@[12:17)              Identifier |vnets|
//@[17:18)            LeftSquare |[|
//@[18:25)            BinaryOperationSyntax
//@[18:23)             VariableAccessSyntax
//@[18:23)              IdentifierSyntax
//@[18:23)               Identifier |index|
//@[23:24)             Minus |-|
//@[24:25)             IntegerLiteralSyntax
//@[24:25)              Integer |1|
//@[25:26)            RightSquare |]|
//@[26:27)           Dot |.|
//@[27:29)           IdentifierSyntax
//@[27:29)            Identifier |id|
//@[29:30)         NewLine |\n|
      }
//@[6:7)         RightBrace |}|
//@[7:8)       NewLine |\n|
      {
//@[6:47)       ArrayItemSyntax
//@[6:47)        ObjectSyntax
//@[6:7)         LeftBrace |{|
//@[7:8)         NewLine |\n|
        id: vnets[index * 2].id
//@[8:31)         ObjectPropertySyntax
//@[8:10)          IdentifierSyntax
//@[8:10)           Identifier |id|
//@[10:11)          Colon |:|
//@[12:31)          PropertyAccessSyntax
//@[12:28)           ArrayAccessSyntax
//@[12:17)            VariableAccessSyntax
//@[12:17)             IdentifierSyntax
//@[12:17)              Identifier |vnets|
//@[17:18)            LeftSquare |[|
//@[18:27)            BinaryOperationSyntax
//@[18:23)             VariableAccessSyntax
//@[18:23)              IdentifierSyntax
//@[18:23)               Identifier |index|
//@[24:25)             Asterisk |*|
//@[26:27)             IntegerLiteralSyntax
//@[26:27)              Integer |2|
//@[27:28)            RightSquare |]|
//@[28:29)           Dot |.|
//@[29:31)           IdentifierSyntax
//@[29:31)            Identifier |id|
//@[31:32)         NewLine |\n|
      }
//@[6:7)         RightBrace |}|
//@[7:8)       NewLine |\n|
    ]
//@[4:5)       RightSquare |]|
//@[5:6)     NewLine |\n|
  }
//@[2:3)     RightBrace |}|
//@[3:4)   NewLine |\n|
  dependsOn: [
//@[2:28)   ObjectPropertySyntax
//@[2:11)    IdentifierSyntax
//@[2:11)     Identifier |dependsOn|
//@[11:12)    Colon |:|
//@[13:28)    ArraySyntax
//@[13:14)     LeftSquare |[|
//@[14:15)     NewLine |\n|
    vnets
//@[4:9)     ArrayItemSyntax
//@[4:9)      VariableAccessSyntax
//@[4:9)       IdentifierSyntax
//@[4:9)        Identifier |vnets|
//@[9:10)     NewLine |\n|
  ]
//@[2:3)     RightSquare |]|
//@[3:4)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

// single module
//@[16:17) NewLine |\n|
module singleModule 'passthrough.bicep' = {
//@[0:97) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:19)  IdentifierSyntax
//@[7:19)   Identifier |singleModule|
//@[20:39)  StringSyntax
//@[20:39)   StringComplete |'passthrough.bicep'|
//@[40:41)  Assignment |=|
//@[42:97)  ObjectSyntax
//@[42:43)   LeftBrace |{|
//@[43:44)   NewLine |\n|
  name: 'test'
//@[2:14)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:14)    StringSyntax
//@[8:14)     StringComplete |'test'|
//@[14:15)   NewLine |\n|
  params: {
//@[2:36)   ObjectPropertySyntax
//@[2:8)    IdentifierSyntax
//@[2:8)     Identifier |params|
//@[8:9)    Colon |:|
//@[10:36)    ObjectSyntax
//@[10:11)     LeftBrace |{|
//@[11:12)     NewLine |\n|
    myInput: 'hello'
//@[4:20)     ObjectPropertySyntax
//@[4:11)      IdentifierSyntax
//@[4:11)       Identifier |myInput|
//@[11:12)      Colon |:|
//@[13:20)      StringSyntax
//@[13:20)       StringComplete |'hello'|
//@[20:21)     NewLine |\n|
  }
//@[2:3)     RightBrace |}|
//@[3:4)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

var moduleSetup = [
//@[0:47) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:15)  IdentifierSyntax
//@[4:15)   Identifier |moduleSetup|
//@[16:17)  Assignment |=|
//@[18:47)  ArraySyntax
//@[18:19)   LeftSquare |[|
//@[19:20)   NewLine |\n|
  'one'
//@[2:7)   ArrayItemSyntax
//@[2:7)    StringSyntax
//@[2:7)     StringComplete |'one'|
//@[7:8)   NewLine |\n|
  'two'
//@[2:7)   ArrayItemSyntax
//@[2:7)    StringSyntax
//@[2:7)     StringComplete |'two'|
//@[7:8)   NewLine |\n|
  'three'
//@[2:9)   ArrayItemSyntax
//@[2:9)    StringSyntax
//@[2:9)     StringComplete |'three'|
//@[9:10)   NewLine |\n|
]
//@[0:1)   RightSquare |]|
//@[1:3) NewLine |\n\n|

// module collection plus explicit dependency on single module
//@[62:63) NewLine |\n|
@sys.batchSize(3)
//@[0:293) ModuleDeclarationSyntax
//@[0:17)  DecoratorSyntax
//@[0:1)   At |@|
//@[1:17)   InstanceFunctionCallSyntax
//@[1:4)    VariableAccessSyntax
//@[1:4)     IdentifierSyntax
//@[1:4)      Identifier |sys|
//@[4:5)    Dot |.|
//@[5:14)    IdentifierSyntax
//@[5:14)     Identifier |batchSize|
//@[14:15)    LeftParen |(|
//@[15:16)    FunctionArgumentSyntax
//@[15:16)     IntegerLiteralSyntax
//@[15:16)      Integer |3|
//@[16:17)    RightParen |)|
//@[17:18)  NewLine |\n|
module moduleCollectionWithSingleDependency 'passthrough.bicep' = [for (moduleName, moduleIndex) in moduleSetup: {
//@[0:6)  Identifier |module|
//@[7:43)  IdentifierSyntax
//@[7:43)   Identifier |moduleCollectionWithSingleDependency|
//@[44:63)  StringSyntax
//@[44:63)   StringComplete |'passthrough.bicep'|
//@[64:65)  Assignment |=|
//@[66:275)  ForSyntax
//@[66:67)   LeftSquare |[|
//@[67:70)   Identifier |for|
//@[71:96)   ForVariableBlockSyntax
//@[71:72)    LeftParen |(|
//@[72:82)    LocalVariableSyntax
//@[72:82)     IdentifierSyntax
//@[72:82)      Identifier |moduleName|
//@[82:83)    Comma |,|
//@[84:95)    LocalVariableSyntax
//@[84:95)     IdentifierSyntax
//@[84:95)      Identifier |moduleIndex|
//@[95:96)    RightParen |)|
//@[97:99)   Identifier |in|
//@[100:111)   VariableAccessSyntax
//@[100:111)    IdentifierSyntax
//@[100:111)     Identifier |moduleSetup|
//@[111:112)   Colon |:|
//@[113:274)   ObjectSyntax
//@[113:114)    LeftBrace |{|
//@[114:115)    NewLine |\n|
  name: concat(moduleName, moduleIndex)
//@[2:39)    ObjectPropertySyntax
//@[2:6)     IdentifierSyntax
//@[2:6)      Identifier |name|
//@[6:7)     Colon |:|
//@[8:39)     FunctionCallSyntax
//@[8:14)      IdentifierSyntax
//@[8:14)       Identifier |concat|
//@[14:15)      LeftParen |(|
//@[15:26)      FunctionArgumentSyntax
//@[15:25)       VariableAccessSyntax
//@[15:25)        IdentifierSyntax
//@[15:25)         Identifier |moduleName|
//@[25:26)       Comma |,|
//@[27:38)      FunctionArgumentSyntax
//@[27:38)       VariableAccessSyntax
//@[27:38)        IdentifierSyntax
//@[27:38)         Identifier |moduleIndex|
//@[38:39)      RightParen |)|
//@[39:40)    NewLine |\n|
  params: {
//@[2:62)    ObjectPropertySyntax
//@[2:8)     IdentifierSyntax
//@[2:8)      Identifier |params|
//@[8:9)     Colon |:|
//@[10:62)     ObjectSyntax
//@[10:11)      LeftBrace |{|
//@[11:12)      NewLine |\n|
    myInput: 'in-${moduleName}-${moduleIndex}'
//@[4:46)      ObjectPropertySyntax
//@[4:11)       IdentifierSyntax
//@[4:11)        Identifier |myInput|
//@[11:12)       Colon |:|
//@[13:46)       StringSyntax
//@[13:19)        StringLeftPiece |'in-${|
//@[19:29)        VariableAccessSyntax
//@[19:29)         IdentifierSyntax
//@[19:29)          Identifier |moduleName|
//@[29:33)        StringMiddlePiece |}-${|
//@[33:44)        VariableAccessSyntax
//@[33:44)         IdentifierSyntax
//@[33:44)          Identifier |moduleIndex|
//@[44:46)        StringRightPiece |}'|
//@[46:47)      NewLine |\n|
  }
//@[2:3)      RightBrace |}|
//@[3:4)    NewLine |\n|
  dependsOn: [
//@[2:54)    ObjectPropertySyntax
//@[2:11)     IdentifierSyntax
//@[2:11)      Identifier |dependsOn|
//@[11:12)     Colon |:|
//@[13:54)     ArraySyntax
//@[13:14)      LeftSquare |[|
//@[14:15)      NewLine |\n|
    singleModule
//@[4:16)      ArrayItemSyntax
//@[4:16)       VariableAccessSyntax
//@[4:16)        IdentifierSyntax
//@[4:16)         Identifier |singleModule|
//@[16:17)      NewLine |\n|
    singleResource
//@[4:18)      ArrayItemSyntax
//@[4:18)       VariableAccessSyntax
//@[4:18)        IdentifierSyntax
//@[4:18)         Identifier |singleResource|
//@[18:19)      NewLine |\n|
  ]
//@[2:3)      RightSquare |]|
//@[3:4)    NewLine |\n|
}]
//@[0:1)    RightBrace |}|
//@[1:2)   RightSquare |]|
//@[2:4) NewLine |\n\n|

// another module collection with dependency on another module collection
//@[73:74) NewLine |\n|
module moduleCollectionWithCollectionDependencies 'passthrough.bicep' = [for (moduleName, moduleIndex) in moduleSetup: {
//@[0:306) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:49)  IdentifierSyntax
//@[7:49)   Identifier |moduleCollectionWithCollectionDependencies|
//@[50:69)  StringSyntax
//@[50:69)   StringComplete |'passthrough.bicep'|
//@[70:71)  Assignment |=|
//@[72:306)  ForSyntax
//@[72:73)   LeftSquare |[|
//@[73:76)   Identifier |for|
//@[77:102)   ForVariableBlockSyntax
//@[77:78)    LeftParen |(|
//@[78:88)    LocalVariableSyntax
//@[78:88)     IdentifierSyntax
//@[78:88)      Identifier |moduleName|
//@[88:89)    Comma |,|
//@[90:101)    LocalVariableSyntax
//@[90:101)     IdentifierSyntax
//@[90:101)      Identifier |moduleIndex|
//@[101:102)    RightParen |)|
//@[103:105)   Identifier |in|
//@[106:117)   VariableAccessSyntax
//@[106:117)    IdentifierSyntax
//@[106:117)     Identifier |moduleSetup|
//@[117:118)   Colon |:|
//@[119:305)   ObjectSyntax
//@[119:120)    LeftBrace |{|
//@[120:121)    NewLine |\n|
  name: concat(moduleName, moduleIndex)
//@[2:39)    ObjectPropertySyntax
//@[2:6)     IdentifierSyntax
//@[2:6)      Identifier |name|
//@[6:7)     Colon |:|
//@[8:39)     FunctionCallSyntax
//@[8:14)      IdentifierSyntax
//@[8:14)       Identifier |concat|
//@[14:15)      LeftParen |(|
//@[15:26)      FunctionArgumentSyntax
//@[15:25)       VariableAccessSyntax
//@[15:25)        IdentifierSyntax
//@[15:25)         Identifier |moduleName|
//@[25:26)       Comma |,|
//@[27:38)      FunctionArgumentSyntax
//@[27:38)       VariableAccessSyntax
//@[27:38)        IdentifierSyntax
//@[27:38)         Identifier |moduleIndex|
//@[38:39)      RightParen |)|
//@[39:40)    NewLine |\n|
  params: {
//@[2:62)    ObjectPropertySyntax
//@[2:8)     IdentifierSyntax
//@[2:8)      Identifier |params|
//@[8:9)     Colon |:|
//@[10:62)     ObjectSyntax
//@[10:11)      LeftBrace |{|
//@[11:12)      NewLine |\n|
    myInput: 'in-${moduleName}-${moduleIndex}'
//@[4:46)      ObjectPropertySyntax
//@[4:11)       IdentifierSyntax
//@[4:11)        Identifier |myInput|
//@[11:12)       Colon |:|
//@[13:46)       StringSyntax
//@[13:19)        StringLeftPiece |'in-${|
//@[19:29)        VariableAccessSyntax
//@[19:29)         IdentifierSyntax
//@[19:29)          Identifier |moduleName|
//@[29:33)        StringMiddlePiece |}-${|
//@[33:44)        VariableAccessSyntax
//@[33:44)         IdentifierSyntax
//@[33:44)          Identifier |moduleIndex|
//@[44:46)        StringRightPiece |}'|
//@[46:47)      NewLine |\n|
  }
//@[2:3)      RightBrace |}|
//@[3:4)    NewLine |\n|
  dependsOn: [
//@[2:79)    ObjectPropertySyntax
//@[2:11)     IdentifierSyntax
//@[2:11)      Identifier |dependsOn|
//@[11:12)     Colon |:|
//@[13:79)     ArraySyntax
//@[13:14)      LeftSquare |[|
//@[14:15)      NewLine |\n|
    storageAccounts
//@[4:19)      ArrayItemSyntax
//@[4:19)       VariableAccessSyntax
//@[4:19)        IdentifierSyntax
//@[4:19)         Identifier |storageAccounts|
//@[19:20)      NewLine |\n|
    moduleCollectionWithSingleDependency
//@[4:40)      ArrayItemSyntax
//@[4:40)       VariableAccessSyntax
//@[4:40)        IdentifierSyntax
//@[4:40)         Identifier |moduleCollectionWithSingleDependency|
//@[40:41)      NewLine |\n|
  ]
//@[2:3)      RightSquare |]|
//@[3:4)    NewLine |\n|
}]
//@[0:1)    RightBrace |}|
//@[1:2)   RightSquare |]|
//@[2:4) NewLine |\n\n|

module singleModuleWithIndexedDependencies 'passthrough.bicep' = {
//@[0:290) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:42)  IdentifierSyntax
//@[7:42)   Identifier |singleModuleWithIndexedDependencies|
//@[43:62)  StringSyntax
//@[43:62)   StringComplete |'passthrough.bicep'|
//@[63:64)  Assignment |=|
//@[65:290)  ObjectSyntax
//@[65:66)   LeftBrace |{|
//@[66:67)   NewLine |\n|
  name: 'hello'
//@[2:15)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:15)    StringSyntax
//@[8:15)     StringComplete |'hello'|
//@[15:16)   NewLine |\n|
  params: {
//@[2:153)   ObjectPropertySyntax
//@[2:8)    IdentifierSyntax
//@[2:8)     Identifier |params|
//@[8:9)    Colon |:|
//@[10:153)    ObjectSyntax
//@[10:11)     LeftBrace |{|
//@[11:12)     NewLine |\n|
    myInput: concat(moduleCollectionWithCollectionDependencies[index].outputs.myOutput, storageAccounts[index * 3].properties.accessTier)
//@[4:137)     ObjectPropertySyntax
//@[4:11)      IdentifierSyntax
//@[4:11)       Identifier |myInput|
//@[11:12)      Colon |:|
//@[13:137)      FunctionCallSyntax
//@[13:19)       IdentifierSyntax
//@[13:19)        Identifier |concat|
//@[19:20)       LeftParen |(|
//@[20:87)       FunctionArgumentSyntax
//@[20:86)        PropertyAccessSyntax
//@[20:77)         PropertyAccessSyntax
//@[20:69)          ArrayAccessSyntax
//@[20:62)           VariableAccessSyntax
//@[20:62)            IdentifierSyntax
//@[20:62)             Identifier |moduleCollectionWithCollectionDependencies|
//@[62:63)           LeftSquare |[|
//@[63:68)           VariableAccessSyntax
//@[63:68)            IdentifierSyntax
//@[63:68)             Identifier |index|
//@[68:69)           RightSquare |]|
//@[69:70)          Dot |.|
//@[70:77)          IdentifierSyntax
//@[70:77)           Identifier |outputs|
//@[77:78)         Dot |.|
//@[78:86)         IdentifierSyntax
//@[78:86)          Identifier |myOutput|
//@[86:87)        Comma |,|
//@[88:136)       FunctionArgumentSyntax
//@[88:136)        PropertyAccessSyntax
//@[88:125)         PropertyAccessSyntax
//@[88:114)          ArrayAccessSyntax
//@[88:103)           VariableAccessSyntax
//@[88:103)            IdentifierSyntax
//@[88:103)             Identifier |storageAccounts|
//@[103:104)           LeftSquare |[|
//@[104:113)           BinaryOperationSyntax
//@[104:109)            VariableAccessSyntax
//@[104:109)             IdentifierSyntax
//@[104:109)              Identifier |index|
//@[110:111)            Asterisk |*|
//@[112:113)            IntegerLiteralSyntax
//@[112:113)             Integer |3|
//@[113:114)           RightSquare |]|
//@[114:115)          Dot |.|
//@[115:125)          IdentifierSyntax
//@[115:125)           Identifier |properties|
//@[125:126)         Dot |.|
//@[126:136)         IdentifierSyntax
//@[126:136)          Identifier |accessTier|
//@[136:137)       RightParen |)|
//@[137:138)     NewLine |\n|
  }
//@[2:3)     RightBrace |}|
//@[3:4)   NewLine |\n|
  dependsOn: [
//@[2:51)   ObjectPropertySyntax
//@[2:11)    IdentifierSyntax
//@[2:11)     Identifier |dependsOn|
//@[11:12)    Colon |:|
//@[13:51)    ArraySyntax
//@[13:14)     LeftSquare |[|
//@[14:15)     NewLine |\n|
    storageAccounts2[index - 10]
//@[4:32)     ArrayItemSyntax
//@[4:32)      ArrayAccessSyntax
//@[4:20)       VariableAccessSyntax
//@[4:20)        IdentifierSyntax
//@[4:20)         Identifier |storageAccounts2|
//@[20:21)       LeftSquare |[|
//@[21:31)       BinaryOperationSyntax
//@[21:26)        VariableAccessSyntax
//@[21:26)         IdentifierSyntax
//@[21:26)          Identifier |index|
//@[27:28)        Minus |-|
//@[29:31)        IntegerLiteralSyntax
//@[29:31)         Integer |10|
//@[31:32)       RightSquare |]|
//@[32:33)     NewLine |\n|
  ]
//@[2:3)     RightSquare |]|
//@[3:4)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

module moduleCollectionWithIndexedDependencies 'passthrough.bicep' = [for (moduleName, moduleIndex) in moduleSetup: {
//@[0:399) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:46)  IdentifierSyntax
//@[7:46)   Identifier |moduleCollectionWithIndexedDependencies|
//@[47:66)  StringSyntax
//@[47:66)   StringComplete |'passthrough.bicep'|
//@[67:68)  Assignment |=|
//@[69:399)  ForSyntax
//@[69:70)   LeftSquare |[|
//@[70:73)   Identifier |for|
//@[74:99)   ForVariableBlockSyntax
//@[74:75)    LeftParen |(|
//@[75:85)    LocalVariableSyntax
//@[75:85)     IdentifierSyntax
//@[75:85)      Identifier |moduleName|
//@[85:86)    Comma |,|
//@[87:98)    LocalVariableSyntax
//@[87:98)     IdentifierSyntax
//@[87:98)      Identifier |moduleIndex|
//@[98:99)    RightParen |)|
//@[100:102)   Identifier |in|
//@[103:114)   VariableAccessSyntax
//@[103:114)    IdentifierSyntax
//@[103:114)     Identifier |moduleSetup|
//@[114:115)   Colon |:|
//@[116:398)   ObjectSyntax
//@[116:117)    LeftBrace |{|
//@[117:118)    NewLine |\n|
  name: concat(moduleName, moduleIndex)
//@[2:39)    ObjectPropertySyntax
//@[2:6)     IdentifierSyntax
//@[2:6)      Identifier |name|
//@[6:7)     Colon |:|
//@[8:39)     FunctionCallSyntax
//@[8:14)      IdentifierSyntax
//@[8:14)       Identifier |concat|
//@[14:15)      LeftParen |(|
//@[15:26)      FunctionArgumentSyntax
//@[15:25)       VariableAccessSyntax
//@[15:25)        IdentifierSyntax
//@[15:25)         Identifier |moduleName|
//@[25:26)       Comma |,|
//@[27:38)      FunctionArgumentSyntax
//@[27:38)       VariableAccessSyntax
//@[27:38)        IdentifierSyntax
//@[27:38)         Identifier |moduleIndex|
//@[38:39)      RightParen |)|
//@[39:40)    NewLine |\n|
  params: {
//@[2:187)    ObjectPropertySyntax
//@[2:8)     IdentifierSyntax
//@[2:8)      Identifier |params|
//@[8:9)     Colon |:|
//@[10:187)     ObjectSyntax
//@[10:11)      LeftBrace |{|
//@[11:12)      NewLine |\n|
    myInput: '${moduleCollectionWithCollectionDependencies[index].outputs.myOutput} - ${storageAccounts[index * 3].properties.accessTier} - ${moduleName} - ${moduleIndex}'
//@[4:171)      ObjectPropertySyntax
//@[4:11)       IdentifierSyntax
//@[4:11)        Identifier |myInput|
//@[11:12)       Colon |:|
//@[13:171)       StringSyntax
//@[13:16)        StringLeftPiece |'${|
//@[16:82)        PropertyAccessSyntax
//@[16:73)         PropertyAccessSyntax
//@[16:65)          ArrayAccessSyntax
//@[16:58)           VariableAccessSyntax
//@[16:58)            IdentifierSyntax
//@[16:58)             Identifier |moduleCollectionWithCollectionDependencies|
//@[58:59)           LeftSquare |[|
//@[59:64)           VariableAccessSyntax
//@[59:64)            IdentifierSyntax
//@[59:64)             Identifier |index|
//@[64:65)           RightSquare |]|
//@[65:66)          Dot |.|
//@[66:73)          IdentifierSyntax
//@[66:73)           Identifier |outputs|
//@[73:74)         Dot |.|
//@[74:82)         IdentifierSyntax
//@[74:82)          Identifier |myOutput|
//@[82:88)        StringMiddlePiece |} - ${|
//@[88:136)        PropertyAccessSyntax
//@[88:125)         PropertyAccessSyntax
//@[88:114)          ArrayAccessSyntax
//@[88:103)           VariableAccessSyntax
//@[88:103)            IdentifierSyntax
//@[88:103)             Identifier |storageAccounts|
//@[103:104)           LeftSquare |[|
//@[104:113)           BinaryOperationSyntax
//@[104:109)            VariableAccessSyntax
//@[104:109)             IdentifierSyntax
//@[104:109)              Identifier |index|
//@[110:111)            Asterisk |*|
//@[112:113)            IntegerLiteralSyntax
//@[112:113)             Integer |3|
//@[113:114)           RightSquare |]|
//@[114:115)          Dot |.|
//@[115:125)          IdentifierSyntax
//@[115:125)           Identifier |properties|
//@[125:126)         Dot |.|
//@[126:136)         IdentifierSyntax
//@[126:136)          Identifier |accessTier|
//@[136:142)        StringMiddlePiece |} - ${|
//@[142:152)        VariableAccessSyntax
//@[142:152)         IdentifierSyntax
//@[142:152)          Identifier |moduleName|
//@[152:158)        StringMiddlePiece |} - ${|
//@[158:169)        VariableAccessSyntax
//@[158:169)         IdentifierSyntax
//@[158:169)          Identifier |moduleIndex|
//@[169:171)        StringRightPiece |}'|
//@[171:172)      NewLine |\n|
  }
//@[2:3)      RightBrace |}|
//@[3:4)    NewLine |\n|
  dependsOn: [
//@[2:50)    ObjectPropertySyntax
//@[2:11)     IdentifierSyntax
//@[2:11)      Identifier |dependsOn|
//@[11:12)     Colon |:|
//@[13:50)     ArraySyntax
//@[13:14)      LeftSquare |[|
//@[14:15)      NewLine |\n|
    storageAccounts2[index - 9]
//@[4:31)      ArrayItemSyntax
//@[4:31)       ArrayAccessSyntax
//@[4:20)        VariableAccessSyntax
//@[4:20)         IdentifierSyntax
//@[4:20)          Identifier |storageAccounts2|
//@[20:21)        LeftSquare |[|
//@[21:30)        BinaryOperationSyntax
//@[21:26)         VariableAccessSyntax
//@[21:26)          IdentifierSyntax
//@[21:26)           Identifier |index|
//@[27:28)         Minus |-|
//@[29:30)         IntegerLiteralSyntax
//@[29:30)          Integer |9|
//@[30:31)        RightSquare |]|
//@[31:32)      NewLine |\n|
  ]
//@[2:3)      RightSquare |]|
//@[3:4)    NewLine |\n|
}]
//@[0:1)    RightBrace |}|
//@[1:2)   RightSquare |]|
//@[2:4) NewLine |\n\n|

output indexedModulesName string = moduleCollectionWithSingleDependency[index].name
//@[0:83) OutputDeclarationSyntax
//@[0:6)  Identifier |output|
//@[7:25)  IdentifierSyntax
//@[7:25)   Identifier |indexedModulesName|
//@[26:32)  TypeSyntax
//@[26:32)   Identifier |string|
//@[33:34)  Assignment |=|
//@[35:83)  PropertyAccessSyntax
//@[35:78)   ArrayAccessSyntax
//@[35:71)    VariableAccessSyntax
//@[35:71)     IdentifierSyntax
//@[35:71)      Identifier |moduleCollectionWithSingleDependency|
//@[71:72)    LeftSquare |[|
//@[72:77)    VariableAccessSyntax
//@[72:77)     IdentifierSyntax
//@[72:77)      Identifier |index|
//@[77:78)    RightSquare |]|
//@[78:79)   Dot |.|
//@[79:83)   IdentifierSyntax
//@[79:83)    Identifier |name|
//@[83:84) NewLine |\n|
output indexedModuleOutput string = moduleCollectionWithSingleDependency[index * 1].outputs.myOutput
//@[0:100) OutputDeclarationSyntax
//@[0:6)  Identifier |output|
//@[7:26)  IdentifierSyntax
//@[7:26)   Identifier |indexedModuleOutput|
//@[27:33)  TypeSyntax
//@[27:33)   Identifier |string|
//@[34:35)  Assignment |=|
//@[36:100)  PropertyAccessSyntax
//@[36:91)   PropertyAccessSyntax
//@[36:83)    ArrayAccessSyntax
//@[36:72)     VariableAccessSyntax
//@[36:72)      IdentifierSyntax
//@[36:72)       Identifier |moduleCollectionWithSingleDependency|
//@[72:73)     LeftSquare |[|
//@[73:82)     BinaryOperationSyntax
//@[73:78)      VariableAccessSyntax
//@[73:78)       IdentifierSyntax
//@[73:78)        Identifier |index|
//@[79:80)      Asterisk |*|
//@[81:82)      IntegerLiteralSyntax
//@[81:82)       Integer |1|
//@[82:83)     RightSquare |]|
//@[83:84)    Dot |.|
//@[84:91)    IdentifierSyntax
//@[84:91)     Identifier |outputs|
//@[91:92)   Dot |.|
//@[92:100)   IdentifierSyntax
//@[92:100)    Identifier |myOutput|
//@[100:102) NewLine |\n\n|

// resource collection
//@[22:23) NewLine |\n|
resource existingStorageAccounts 'Microsoft.Storage/storageAccounts@2019-06-01' existing = [for (account, i) in accounts: {
//@[0:174) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:32)  IdentifierSyntax
//@[9:32)   Identifier |existingStorageAccounts|
//@[33:79)  StringSyntax
//@[33:79)   StringComplete |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[80:88)  Identifier |existing|
//@[89:90)  Assignment |=|
//@[91:174)  ForSyntax
//@[91:92)   LeftSquare |[|
//@[92:95)   Identifier |for|
//@[96:108)   ForVariableBlockSyntax
//@[96:97)    LeftParen |(|
//@[97:104)    LocalVariableSyntax
//@[97:104)     IdentifierSyntax
//@[97:104)      Identifier |account|
//@[104:105)    Comma |,|
//@[106:107)    LocalVariableSyntax
//@[106:107)     IdentifierSyntax
//@[106:107)      Identifier |i|
//@[107:108)    RightParen |)|
//@[109:111)   Identifier |in|
//@[112:120)   VariableAccessSyntax
//@[112:120)    IdentifierSyntax
//@[112:120)     Identifier |accounts|
//@[120:121)   Colon |:|
//@[122:173)   ObjectSyntax
//@[122:123)    LeftBrace |{|
//@[123:124)    NewLine |\n|
  name: '${name}-existing-${account.name}-${i}'
//@[2:47)    ObjectPropertySyntax
//@[2:6)     IdentifierSyntax
//@[2:6)      Identifier |name|
//@[6:7)     Colon |:|
//@[8:47)     StringSyntax
//@[8:11)      StringLeftPiece |'${|
//@[11:15)      VariableAccessSyntax
//@[11:15)       IdentifierSyntax
//@[11:15)        Identifier |name|
//@[15:28)      StringMiddlePiece |}-existing-${|
//@[28:40)      PropertyAccessSyntax
//@[28:35)       VariableAccessSyntax
//@[28:35)        IdentifierSyntax
//@[28:35)         Identifier |account|
//@[35:36)       Dot |.|
//@[36:40)       IdentifierSyntax
//@[36:40)        Identifier |name|
//@[40:44)      StringMiddlePiece |}-${|
//@[44:45)      VariableAccessSyntax
//@[44:45)       IdentifierSyntax
//@[44:45)        Identifier |i|
//@[45:47)      StringRightPiece |}'|
//@[47:48)    NewLine |\n|
}]
//@[0:1)    RightBrace |}|
//@[1:2)   RightSquare |]|
//@[2:4) NewLine |\n\n|

output existingIndexedResourceName string = existingStorageAccounts[index * 0].name
//@[0:83) OutputDeclarationSyntax
//@[0:6)  Identifier |output|
//@[7:34)  IdentifierSyntax
//@[7:34)   Identifier |existingIndexedResourceName|
//@[35:41)  TypeSyntax
//@[35:41)   Identifier |string|
//@[42:43)  Assignment |=|
//@[44:83)  PropertyAccessSyntax
//@[44:78)   ArrayAccessSyntax
//@[44:67)    VariableAccessSyntax
//@[44:67)     IdentifierSyntax
//@[44:67)      Identifier |existingStorageAccounts|
//@[67:68)    LeftSquare |[|
//@[68:77)    BinaryOperationSyntax
//@[68:73)     VariableAccessSyntax
//@[68:73)      IdentifierSyntax
//@[68:73)       Identifier |index|
//@[74:75)     Asterisk |*|
//@[76:77)     IntegerLiteralSyntax
//@[76:77)      Integer |0|
//@[77:78)    RightSquare |]|
//@[78:79)   Dot |.|
//@[79:83)   IdentifierSyntax
//@[79:83)    Identifier |name|
//@[83:84) NewLine |\n|
output existingIndexedResourceId string = existingStorageAccounts[index * 1].id
//@[0:79) OutputDeclarationSyntax
//@[0:6)  Identifier |output|
//@[7:32)  IdentifierSyntax
//@[7:32)   Identifier |existingIndexedResourceId|
//@[33:39)  TypeSyntax
//@[33:39)   Identifier |string|
//@[40:41)  Assignment |=|
//@[42:79)  PropertyAccessSyntax
//@[42:76)   ArrayAccessSyntax
//@[42:65)    VariableAccessSyntax
//@[42:65)     IdentifierSyntax
//@[42:65)      Identifier |existingStorageAccounts|
//@[65:66)    LeftSquare |[|
//@[66:75)    BinaryOperationSyntax
//@[66:71)     VariableAccessSyntax
//@[66:71)      IdentifierSyntax
//@[66:71)       Identifier |index|
//@[72:73)     Asterisk |*|
//@[74:75)     IntegerLiteralSyntax
//@[74:75)      Integer |1|
//@[75:76)    RightSquare |]|
//@[76:77)   Dot |.|
//@[77:79)   IdentifierSyntax
//@[77:79)    Identifier |id|
//@[79:80) NewLine |\n|
output existingIndexedResourceType string = existingStorageAccounts[index+2].type
//@[0:81) OutputDeclarationSyntax
//@[0:6)  Identifier |output|
//@[7:34)  IdentifierSyntax
//@[7:34)   Identifier |existingIndexedResourceType|
//@[35:41)  TypeSyntax
//@[35:41)   Identifier |string|
//@[42:43)  Assignment |=|
//@[44:81)  PropertyAccessSyntax
//@[44:76)   ArrayAccessSyntax
//@[44:67)    VariableAccessSyntax
//@[44:67)     IdentifierSyntax
//@[44:67)      Identifier |existingStorageAccounts|
//@[67:68)    LeftSquare |[|
//@[68:75)    BinaryOperationSyntax
//@[68:73)     VariableAccessSyntax
//@[68:73)      IdentifierSyntax
//@[68:73)       Identifier |index|
//@[73:74)     Plus |+|
//@[74:75)     IntegerLiteralSyntax
//@[74:75)      Integer |2|
//@[75:76)    RightSquare |]|
//@[76:77)   Dot |.|
//@[77:81)   IdentifierSyntax
//@[77:81)    Identifier |type|
//@[81:82) NewLine |\n|
output existingIndexedResourceApiVersion string = existingStorageAccounts[index-7].apiVersion
//@[0:93) OutputDeclarationSyntax
//@[0:6)  Identifier |output|
//@[7:40)  IdentifierSyntax
//@[7:40)   Identifier |existingIndexedResourceApiVersion|
//@[41:47)  TypeSyntax
//@[41:47)   Identifier |string|
//@[48:49)  Assignment |=|
//@[50:93)  PropertyAccessSyntax
//@[50:82)   ArrayAccessSyntax
//@[50:73)    VariableAccessSyntax
//@[50:73)     IdentifierSyntax
//@[50:73)      Identifier |existingStorageAccounts|
//@[73:74)    LeftSquare |[|
//@[74:81)    BinaryOperationSyntax
//@[74:79)     VariableAccessSyntax
//@[74:79)      IdentifierSyntax
//@[74:79)       Identifier |index|
//@[79:80)     Minus |-|
//@[80:81)     IntegerLiteralSyntax
//@[80:81)      Integer |7|
//@[81:82)    RightSquare |]|
//@[82:83)   Dot |.|
//@[83:93)   IdentifierSyntax
//@[83:93)    Identifier |apiVersion|
//@[93:94) NewLine |\n|
output existingIndexedResourceLocation string = existingStorageAccounts[index/2].location
//@[0:89) OutputDeclarationSyntax
//@[0:6)  Identifier |output|
//@[7:38)  IdentifierSyntax
//@[7:38)   Identifier |existingIndexedResourceLocation|
//@[39:45)  TypeSyntax
//@[39:45)   Identifier |string|
//@[46:47)  Assignment |=|
//@[48:89)  PropertyAccessSyntax
//@[48:80)   ArrayAccessSyntax
//@[48:71)    VariableAccessSyntax
//@[48:71)     IdentifierSyntax
//@[48:71)      Identifier |existingStorageAccounts|
//@[71:72)    LeftSquare |[|
//@[72:79)    BinaryOperationSyntax
//@[72:77)     VariableAccessSyntax
//@[72:77)      IdentifierSyntax
//@[72:77)       Identifier |index|
//@[77:78)     Slash |/|
//@[78:79)     IntegerLiteralSyntax
//@[78:79)      Integer |2|
//@[79:80)    RightSquare |]|
//@[80:81)   Dot |.|
//@[81:89)   IdentifierSyntax
//@[81:89)    Identifier |location|
//@[89:90) NewLine |\n|
output existingIndexedResourceAccessTier string = existingStorageAccounts[index%3].properties.accessTier
//@[0:104) OutputDeclarationSyntax
//@[0:6)  Identifier |output|
//@[7:40)  IdentifierSyntax
//@[7:40)   Identifier |existingIndexedResourceAccessTier|
//@[41:47)  TypeSyntax
//@[41:47)   Identifier |string|
//@[48:49)  Assignment |=|
//@[50:104)  PropertyAccessSyntax
//@[50:93)   PropertyAccessSyntax
//@[50:82)    ArrayAccessSyntax
//@[50:73)     VariableAccessSyntax
//@[50:73)      IdentifierSyntax
//@[50:73)       Identifier |existingStorageAccounts|
//@[73:74)     LeftSquare |[|
//@[74:81)     BinaryOperationSyntax
//@[74:79)      VariableAccessSyntax
//@[74:79)       IdentifierSyntax
//@[74:79)        Identifier |index|
//@[79:80)      Modulo |%|
//@[80:81)      IntegerLiteralSyntax
//@[80:81)       Integer |3|
//@[81:82)     RightSquare |]|
//@[82:83)    Dot |.|
//@[83:93)    IdentifierSyntax
//@[83:93)     Identifier |properties|
//@[93:94)   Dot |.|
//@[94:104)   IdentifierSyntax
//@[94:104)    Identifier |accessTier|
//@[104:106) NewLine |\n\n|

resource duplicatedNames 'Microsoft.Network/dnsZones@2018-05-01' = [for (zone,i) in []: {
//@[0:140) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:24)  IdentifierSyntax
//@[9:24)   Identifier |duplicatedNames|
//@[25:64)  StringSyntax
//@[25:64)   StringComplete |'Microsoft.Network/dnsZones@2018-05-01'|
//@[65:66)  Assignment |=|
//@[67:140)  ForSyntax
//@[67:68)   LeftSquare |[|
//@[68:71)   Identifier |for|
//@[72:80)   ForVariableBlockSyntax
//@[72:73)    LeftParen |(|
//@[73:77)    LocalVariableSyntax
//@[73:77)     IdentifierSyntax
//@[73:77)      Identifier |zone|
//@[77:78)    Comma |,|
//@[78:79)    LocalVariableSyntax
//@[78:79)     IdentifierSyntax
//@[78:79)      Identifier |i|
//@[79:80)    RightParen |)|
//@[81:83)   Identifier |in|
//@[84:86)   ArraySyntax
//@[84:85)    LeftSquare |[|
//@[85:86)    RightSquare |]|
//@[86:87)   Colon |:|
//@[88:139)   ObjectSyntax
//@[88:89)    LeftBrace |{|
//@[89:90)    NewLine |\n|
  name: 'no loop variable'
//@[2:26)    ObjectPropertySyntax
//@[2:6)     IdentifierSyntax
//@[2:6)      Identifier |name|
//@[6:7)     Colon |:|
//@[8:26)     StringSyntax
//@[8:26)      StringComplete |'no loop variable'|
//@[26:27)    NewLine |\n|
  location: 'eastus'
//@[2:20)    ObjectPropertySyntax
//@[2:10)     IdentifierSyntax
//@[2:10)      Identifier |location|
//@[10:11)     Colon |:|
//@[12:20)     StringSyntax
//@[12:20)      StringComplete |'eastus'|
//@[20:21)    NewLine |\n|
}]
//@[0:1)    RightBrace |}|
//@[1:2)   RightSquare |]|
//@[2:4) NewLine |\n\n|

// reference to a resource collection whose name expression does not reference any loop variables
//@[97:98) NewLine |\n|
resource referenceToDuplicateNames 'Microsoft.Network/dnsZones@2018-05-01' = [for (zone,i) in []: {
//@[0:196) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:34)  IdentifierSyntax
//@[9:34)   Identifier |referenceToDuplicateNames|
//@[35:74)  StringSyntax
//@[35:74)   StringComplete |'Microsoft.Network/dnsZones@2018-05-01'|
//@[75:76)  Assignment |=|
//@[77:196)  ForSyntax
//@[77:78)   LeftSquare |[|
//@[78:81)   Identifier |for|
//@[82:90)   ForVariableBlockSyntax
//@[82:83)    LeftParen |(|
//@[83:87)    LocalVariableSyntax
//@[83:87)     IdentifierSyntax
//@[83:87)      Identifier |zone|
//@[87:88)    Comma |,|
//@[88:89)    LocalVariableSyntax
//@[88:89)     IdentifierSyntax
//@[88:89)      Identifier |i|
//@[89:90)    RightParen |)|
//@[91:93)   Identifier |in|
//@[94:96)   ArraySyntax
//@[94:95)    LeftSquare |[|
//@[95:96)    RightSquare |]|
//@[96:97)   Colon |:|
//@[98:195)   ObjectSyntax
//@[98:99)    LeftBrace |{|
//@[99:100)    NewLine |\n|
  name: 'no loop variable'
//@[2:26)    ObjectPropertySyntax
//@[2:6)     IdentifierSyntax
//@[2:6)      Identifier |name|
//@[6:7)     Colon |:|
//@[8:26)     StringSyntax
//@[8:26)      StringComplete |'no loop variable'|
//@[26:27)    NewLine |\n|
  location: 'eastus'
//@[2:20)    ObjectPropertySyntax
//@[2:10)     IdentifierSyntax
//@[2:10)      Identifier |location|
//@[10:11)     Colon |:|
//@[12:20)     StringSyntax
//@[12:20)      StringComplete |'eastus'|
//@[20:21)    NewLine |\n|
  dependsOn: [
//@[2:45)    ObjectPropertySyntax
//@[2:11)     IdentifierSyntax
//@[2:11)      Identifier |dependsOn|
//@[11:12)     Colon |:|
//@[13:45)     ArraySyntax
//@[13:14)      LeftSquare |[|
//@[14:15)      NewLine |\n|
    duplicatedNames[index]
//@[4:26)      ArrayItemSyntax
//@[4:26)       ArrayAccessSyntax
//@[4:19)        VariableAccessSyntax
//@[4:19)         IdentifierSyntax
//@[4:19)          Identifier |duplicatedNames|
//@[19:20)        LeftSquare |[|
//@[20:25)        VariableAccessSyntax
//@[20:25)         IdentifierSyntax
//@[20:25)          Identifier |index|
//@[25:26)        RightSquare |]|
//@[26:27)      NewLine |\n|
  ]
//@[2:3)      RightSquare |]|
//@[3:4)    NewLine |\n|
}]
//@[0:1)    RightBrace |}|
//@[1:2)   RightSquare |]|
//@[2:4) NewLine |\n\n|

var regions = [
//@[0:39) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:11)  IdentifierSyntax
//@[4:11)   Identifier |regions|
//@[12:13)  Assignment |=|
//@[14:39)  ArraySyntax
//@[14:15)   LeftSquare |[|
//@[15:16)   NewLine |\n|
  'eastus'
//@[2:10)   ArrayItemSyntax
//@[2:10)    StringSyntax
//@[2:10)     StringComplete |'eastus'|
//@[10:11)   NewLine |\n|
  'westus'
//@[2:10)   ArrayItemSyntax
//@[2:10)    StringSyntax
//@[2:10)     StringComplete |'westus'|
//@[10:11)   NewLine |\n|
]
//@[0:1)   RightSquare |]|
//@[1:3) NewLine |\n\n|

module apim 'passthrough.bicep' = [for (region, i) in regions: {
//@[0:141) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:11)  IdentifierSyntax
//@[7:11)   Identifier |apim|
//@[12:31)  StringSyntax
//@[12:31)   StringComplete |'passthrough.bicep'|
//@[32:33)  Assignment |=|
//@[34:141)  ForSyntax
//@[34:35)   LeftSquare |[|
//@[35:38)   Identifier |for|
//@[39:50)   ForVariableBlockSyntax
//@[39:40)    LeftParen |(|
//@[40:46)    LocalVariableSyntax
//@[40:46)     IdentifierSyntax
//@[40:46)      Identifier |region|
//@[46:47)    Comma |,|
//@[48:49)    LocalVariableSyntax
//@[48:49)     IdentifierSyntax
//@[48:49)      Identifier |i|
//@[49:50)    RightParen |)|
//@[51:53)   Identifier |in|
//@[54:61)   VariableAccessSyntax
//@[54:61)    IdentifierSyntax
//@[54:61)     Identifier |regions|
//@[61:62)   Colon |:|
//@[63:140)   ObjectSyntax
//@[63:64)    LeftBrace |{|
//@[64:65)    NewLine |\n|
  name: 'apim-${region}-${name}-${i}'
//@[2:37)    ObjectPropertySyntax
//@[2:6)     IdentifierSyntax
//@[2:6)      Identifier |name|
//@[6:7)     Colon |:|
//@[8:37)     StringSyntax
//@[8:16)      StringLeftPiece |'apim-${|
//@[16:22)      VariableAccessSyntax
//@[16:22)       IdentifierSyntax
//@[16:22)        Identifier |region|
//@[22:26)      StringMiddlePiece |}-${|
//@[26:30)      VariableAccessSyntax
//@[26:30)       IdentifierSyntax
//@[26:30)        Identifier |name|
//@[30:34)      StringMiddlePiece |}-${|
//@[34:35)      VariableAccessSyntax
//@[34:35)       IdentifierSyntax
//@[34:35)        Identifier |i|
//@[35:37)      StringRightPiece |}'|
//@[37:38)    NewLine |\n|
  params: {
//@[2:35)    ObjectPropertySyntax
//@[2:8)     IdentifierSyntax
//@[2:8)      Identifier |params|
//@[8:9)     Colon |:|
//@[10:35)     ObjectSyntax
//@[10:11)      LeftBrace |{|
//@[11:12)      NewLine |\n|
    myInput: region
//@[4:19)      ObjectPropertySyntax
//@[4:11)       IdentifierSyntax
//@[4:11)        Identifier |myInput|
//@[11:12)       Colon |:|
//@[13:19)       VariableAccessSyntax
//@[13:19)        IdentifierSyntax
//@[13:19)         Identifier |region|
//@[19:20)      NewLine |\n|
  }
//@[2:3)      RightBrace |}|
//@[3:4)    NewLine |\n|
}]
//@[0:1)    RightBrace |}|
//@[1:2)   RightSquare |]|
//@[2:4) NewLine |\n\n|

resource propertyLoopDependencyOnModuleCollection 'Microsoft.Network/frontDoors@2020-05-01' = {
//@[0:792) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:49)  IdentifierSyntax
//@[9:49)   Identifier |propertyLoopDependencyOnModuleCollection|
//@[50:91)  StringSyntax
//@[50:91)   StringComplete |'Microsoft.Network/frontDoors@2020-05-01'|
//@[92:93)  Assignment |=|
//@[94:792)  ObjectSyntax
//@[94:95)   LeftBrace |{|
//@[95:96)   NewLine |\n|
  name: name
//@[2:12)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:12)    VariableAccessSyntax
//@[8:12)     IdentifierSyntax
//@[8:12)      Identifier |name|
//@[12:13)   NewLine |\n|
  location: 'Global'
//@[2:20)   ObjectPropertySyntax
//@[2:10)    IdentifierSyntax
//@[2:10)     Identifier |location|
//@[10:11)    Colon |:|
//@[12:20)    StringSyntax
//@[12:20)     StringComplete |'Global'|
//@[20:21)   NewLine |\n|
  properties: {
//@[2:660)   ObjectPropertySyntax
//@[2:12)    IdentifierSyntax
//@[2:12)     Identifier |properties|
//@[12:13)    Colon |:|
//@[14:660)    ObjectSyntax
//@[14:15)     LeftBrace |{|
//@[15:16)     NewLine |\n|
    backendPools: [
//@[4:640)     ObjectPropertySyntax
//@[4:16)      IdentifierSyntax
//@[4:16)       Identifier |backendPools|
//@[16:17)      Colon |:|
//@[18:640)      ArraySyntax
//@[18:19)       LeftSquare |[|
//@[19:20)       NewLine |\n|
      {
//@[6:614)       ArrayItemSyntax
//@[6:614)        ObjectSyntax
//@[6:7)         LeftBrace |{|
//@[7:8)         NewLine |\n|
        name: 'BackendAPIMs'
//@[8:28)         ObjectPropertySyntax
//@[8:12)          IdentifierSyntax
//@[8:12)           Identifier |name|
//@[12:13)          Colon |:|
//@[14:28)          StringSyntax
//@[14:28)           StringComplete |'BackendAPIMs'|
//@[28:29)         NewLine |\n|
        properties: {
//@[8:569)         ObjectPropertySyntax
//@[8:18)          IdentifierSyntax
//@[8:18)           Identifier |properties|
//@[18:19)          Colon |:|
//@[20:569)          ObjectSyntax
//@[20:21)           LeftBrace |{|
//@[21:22)           NewLine |\n|
          backends: [for (index,i) in range(0, length(regions)): {
//@[10:537)           ObjectPropertySyntax
//@[10:18)            IdentifierSyntax
//@[10:18)             Identifier |backends|
//@[18:19)            Colon |:|
//@[20:537)            ForSyntax
//@[20:21)             LeftSquare |[|
//@[21:24)             Identifier |for|
//@[25:34)             ForVariableBlockSyntax
//@[25:26)              LeftParen |(|
//@[26:31)              LocalVariableSyntax
//@[26:31)               IdentifierSyntax
//@[26:31)                Identifier |index|
//@[31:32)              Comma |,|
//@[32:33)              LocalVariableSyntax
//@[32:33)               IdentifierSyntax
//@[32:33)                Identifier |i|
//@[33:34)              RightParen |)|
//@[35:37)             Identifier |in|
//@[38:63)             FunctionCallSyntax
//@[38:43)              IdentifierSyntax
//@[38:43)               Identifier |range|
//@[43:44)              LeftParen |(|
//@[44:46)              FunctionArgumentSyntax
//@[44:45)               IntegerLiteralSyntax
//@[44:45)                Integer |0|
//@[45:46)               Comma |,|
//@[47:62)              FunctionArgumentSyntax
//@[47:62)               FunctionCallSyntax
//@[47:53)                IdentifierSyntax
//@[47:53)                 Identifier |length|
//@[53:54)                LeftParen |(|
//@[54:61)                FunctionArgumentSyntax
//@[54:61)                 VariableAccessSyntax
//@[54:61)                  IdentifierSyntax
//@[54:61)                   Identifier |regions|
//@[61:62)                RightParen |)|
//@[62:63)              RightParen |)|
//@[63:64)             Colon |:|
//@[65:536)             ObjectSyntax
//@[65:66)              LeftBrace |{|
//@[66:67)              NewLine |\n|
            // we cannot codegen index correctly because the generated dependsOn property
//@[89:90)              NewLine |\n|
            // would be outside of the scope of the property loop
//@[65:66)              NewLine |\n|
            // as a result, this will generate a dependency on the entire collection
//@[84:85)              NewLine |\n|
            address: apim[index + i].outputs.myOutput
//@[12:53)              ObjectPropertySyntax
//@[12:19)               IdentifierSyntax
//@[12:19)                Identifier |address|
//@[19:20)               Colon |:|
//@[21:53)               PropertyAccessSyntax
//@[21:44)                PropertyAccessSyntax
//@[21:36)                 ArrayAccessSyntax
//@[21:25)                  VariableAccessSyntax
//@[21:25)                   IdentifierSyntax
//@[21:25)                    Identifier |apim|
//@[25:26)                  LeftSquare |[|
//@[26:35)                  BinaryOperationSyntax
//@[26:31)                   VariableAccessSyntax
//@[26:31)                    IdentifierSyntax
//@[26:31)                     Identifier |index|
//@[32:33)                   Plus |+|
//@[34:35)                   VariableAccessSyntax
//@[34:35)                    IdentifierSyntax
//@[34:35)                     Identifier |i|
//@[35:36)                  RightSquare |]|
//@[36:37)                 Dot |.|
//@[37:44)                 IdentifierSyntax
//@[37:44)                  Identifier |outputs|
//@[44:45)                Dot |.|
//@[45:53)                IdentifierSyntax
//@[45:53)                 Identifier |myOutput|
//@[53:54)              NewLine |\n|
            backendHostHeader: apim[index + i].outputs.myOutput
//@[12:63)              ObjectPropertySyntax
//@[12:29)               IdentifierSyntax
//@[12:29)                Identifier |backendHostHeader|
//@[29:30)               Colon |:|
//@[31:63)               PropertyAccessSyntax
//@[31:54)                PropertyAccessSyntax
//@[31:46)                 ArrayAccessSyntax
//@[31:35)                  VariableAccessSyntax
//@[31:35)                   IdentifierSyntax
//@[31:35)                    Identifier |apim|
//@[35:36)                  LeftSquare |[|
//@[36:45)                  BinaryOperationSyntax
//@[36:41)                   VariableAccessSyntax
//@[36:41)                    IdentifierSyntax
//@[36:41)                     Identifier |index|
//@[42:43)                   Plus |+|
//@[44:45)                   VariableAccessSyntax
//@[44:45)                    IdentifierSyntax
//@[44:45)                     Identifier |i|
//@[45:46)                  RightSquare |]|
//@[46:47)                 Dot |.|
//@[47:54)                 IdentifierSyntax
//@[47:54)                  Identifier |outputs|
//@[54:55)                Dot |.|
//@[55:63)                IdentifierSyntax
//@[55:63)                 Identifier |myOutput|
//@[63:64)              NewLine |\n|
            httpPort: 80
//@[12:24)              ObjectPropertySyntax
//@[12:20)               IdentifierSyntax
//@[12:20)                Identifier |httpPort|
//@[20:21)               Colon |:|
//@[22:24)               IntegerLiteralSyntax
//@[22:24)                Integer |80|
//@[24:25)              NewLine |\n|
            httpsPort: 443
//@[12:26)              ObjectPropertySyntax
//@[12:21)               IdentifierSyntax
//@[12:21)                Identifier |httpsPort|
//@[21:22)               Colon |:|
//@[23:26)               IntegerLiteralSyntax
//@[23:26)                Integer |443|
//@[26:27)              NewLine |\n|
            priority: 1
//@[12:23)              ObjectPropertySyntax
//@[12:20)               IdentifierSyntax
//@[12:20)                Identifier |priority|
//@[20:21)               Colon |:|
//@[22:23)               IntegerLiteralSyntax
//@[22:23)                Integer |1|
//@[23:24)              NewLine |\n|
            weight: 50
//@[12:22)              ObjectPropertySyntax
//@[12:18)               IdentifierSyntax
//@[12:18)                Identifier |weight|
//@[18:19)               Colon |:|
//@[20:22)               IntegerLiteralSyntax
//@[20:22)                Integer |50|
//@[22:23)              NewLine |\n|
          }]
//@[10:11)              RightBrace |}|
//@[11:12)             RightSquare |]|
//@[12:13)           NewLine |\n|
        }
//@[8:9)           RightBrace |}|
//@[9:10)         NewLine |\n|
      }
//@[6:7)         RightBrace |}|
//@[7:8)       NewLine |\n|
    ]
//@[4:5)       RightSquare |]|
//@[5:6)     NewLine |\n|
  }
//@[2:3)     RightBrace |}|
//@[3:4)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

resource indexedModuleCollectionDependency 'Microsoft.Network/frontDoors@2020-05-01' = [for (index, i) in range(0, length(regions)): {
//@[0:771) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:42)  IdentifierSyntax
//@[9:42)   Identifier |indexedModuleCollectionDependency|
//@[43:84)  StringSyntax
//@[43:84)   StringComplete |'Microsoft.Network/frontDoors@2020-05-01'|
//@[85:86)  Assignment |=|
//@[87:771)  ForSyntax
//@[87:88)   LeftSquare |[|
//@[88:91)   Identifier |for|
//@[92:102)   ForVariableBlockSyntax
//@[92:93)    LeftParen |(|
//@[93:98)    LocalVariableSyntax
//@[93:98)     IdentifierSyntax
//@[93:98)      Identifier |index|
//@[98:99)    Comma |,|
//@[100:101)    LocalVariableSyntax
//@[100:101)     IdentifierSyntax
//@[100:101)      Identifier |i|
//@[101:102)    RightParen |)|
//@[103:105)   Identifier |in|
//@[106:131)   FunctionCallSyntax
//@[106:111)    IdentifierSyntax
//@[106:111)     Identifier |range|
//@[111:112)    LeftParen |(|
//@[112:114)    FunctionArgumentSyntax
//@[112:113)     IntegerLiteralSyntax
//@[112:113)      Integer |0|
//@[113:114)     Comma |,|
//@[115:130)    FunctionArgumentSyntax
//@[115:130)     FunctionCallSyntax
//@[115:121)      IdentifierSyntax
//@[115:121)       Identifier |length|
//@[121:122)      LeftParen |(|
//@[122:129)      FunctionArgumentSyntax
//@[122:129)       VariableAccessSyntax
//@[122:129)        IdentifierSyntax
//@[122:129)         Identifier |regions|
//@[129:130)      RightParen |)|
//@[130:131)    RightParen |)|
//@[131:132)   Colon |:|
//@[133:770)   ObjectSyntax
//@[133:134)    LeftBrace |{|
//@[134:135)    NewLine |\n|
  name: '${name}-${index}-${i}'
//@[2:31)    ObjectPropertySyntax
//@[2:6)     IdentifierSyntax
//@[2:6)      Identifier |name|
//@[6:7)     Colon |:|
//@[8:31)     StringSyntax
//@[8:11)      StringLeftPiece |'${|
//@[11:15)      VariableAccessSyntax
//@[11:15)       IdentifierSyntax
//@[11:15)        Identifier |name|
//@[15:19)      StringMiddlePiece |}-${|
//@[19:24)      VariableAccessSyntax
//@[19:24)       IdentifierSyntax
//@[19:24)        Identifier |index|
//@[24:28)      StringMiddlePiece |}-${|
//@[28:29)      VariableAccessSyntax
//@[28:29)       IdentifierSyntax
//@[28:29)        Identifier |i|
//@[29:31)      StringRightPiece |}'|
//@[31:32)    NewLine |\n|
  location: 'Global'
//@[2:20)    ObjectPropertySyntax
//@[2:10)     IdentifierSyntax
//@[2:10)      Identifier |location|
//@[10:11)     Colon |:|
//@[12:20)     StringSyntax
//@[12:20)      StringComplete |'Global'|
//@[20:21)    NewLine |\n|
  properties: {
//@[2:580)    ObjectPropertySyntax
//@[2:12)     IdentifierSyntax
//@[2:12)      Identifier |properties|
//@[12:13)     Colon |:|
//@[14:580)     ObjectSyntax
//@[14:15)      LeftBrace |{|
//@[15:16)      NewLine |\n|
    backendPools: [
//@[4:560)      ObjectPropertySyntax
//@[4:16)       IdentifierSyntax
//@[4:16)        Identifier |backendPools|
//@[16:17)       Colon |:|
//@[18:560)       ArraySyntax
//@[18:19)        LeftSquare |[|
//@[19:20)        NewLine |\n|
      {
//@[6:534)        ArrayItemSyntax
//@[6:534)         ObjectSyntax
//@[6:7)          LeftBrace |{|
//@[7:8)          NewLine |\n|
        name: 'BackendAPIMs'
//@[8:28)          ObjectPropertySyntax
//@[8:12)           IdentifierSyntax
//@[8:12)            Identifier |name|
//@[12:13)           Colon |:|
//@[14:28)           StringSyntax
//@[14:28)            StringComplete |'BackendAPIMs'|
//@[28:29)          NewLine |\n|
        properties: {
//@[8:489)          ObjectPropertySyntax
//@[8:18)           IdentifierSyntax
//@[8:18)            Identifier |properties|
//@[18:19)           Colon |:|
//@[20:489)           ObjectSyntax
//@[20:21)            LeftBrace |{|
//@[21:22)            NewLine |\n|
          backends: [
//@[10:457)            ObjectPropertySyntax
//@[10:18)             IdentifierSyntax
//@[10:18)              Identifier |backends|
//@[18:19)             Colon |:|
//@[20:457)             ArraySyntax
//@[20:21)              LeftSquare |[|
//@[21:22)              NewLine |\n|
            {
//@[12:423)              ArrayItemSyntax
//@[12:423)               ObjectSyntax
//@[12:13)                LeftBrace |{|
//@[13:14)                NewLine |\n|
              // this indexed dependency on a module collection will be generated correctly because
//@[99:100)                NewLine |\n|
              // copyIndex() can be invoked in the generated dependsOn
//@[70:71)                NewLine |\n|
              address: apim[index+i].outputs.myOutput
//@[14:53)                ObjectPropertySyntax
//@[14:21)                 IdentifierSyntax
//@[14:21)                  Identifier |address|
//@[21:22)                 Colon |:|
//@[23:53)                 PropertyAccessSyntax
//@[23:44)                  PropertyAccessSyntax
//@[23:36)                   ArrayAccessSyntax
//@[23:27)                    VariableAccessSyntax
//@[23:27)                     IdentifierSyntax
//@[23:27)                      Identifier |apim|
//@[27:28)                    LeftSquare |[|
//@[28:35)                    BinaryOperationSyntax
//@[28:33)                     VariableAccessSyntax
//@[28:33)                      IdentifierSyntax
//@[28:33)                       Identifier |index|
//@[33:34)                     Plus |+|
//@[34:35)                     VariableAccessSyntax
//@[34:35)                      IdentifierSyntax
//@[34:35)                       Identifier |i|
//@[35:36)                    RightSquare |]|
//@[36:37)                   Dot |.|
//@[37:44)                   IdentifierSyntax
//@[37:44)                    Identifier |outputs|
//@[44:45)                  Dot |.|
//@[45:53)                  IdentifierSyntax
//@[45:53)                   Identifier |myOutput|
//@[53:54)                NewLine |\n|
              backendHostHeader: apim[index+i].outputs.myOutput
//@[14:63)                ObjectPropertySyntax
//@[14:31)                 IdentifierSyntax
//@[14:31)                  Identifier |backendHostHeader|
//@[31:32)                 Colon |:|
//@[33:63)                 PropertyAccessSyntax
//@[33:54)                  PropertyAccessSyntax
//@[33:46)                   ArrayAccessSyntax
//@[33:37)                    VariableAccessSyntax
//@[33:37)                     IdentifierSyntax
//@[33:37)                      Identifier |apim|
//@[37:38)                    LeftSquare |[|
//@[38:45)                    BinaryOperationSyntax
//@[38:43)                     VariableAccessSyntax
//@[38:43)                      IdentifierSyntax
//@[38:43)                       Identifier |index|
//@[43:44)                     Plus |+|
//@[44:45)                     VariableAccessSyntax
//@[44:45)                      IdentifierSyntax
//@[44:45)                       Identifier |i|
//@[45:46)                    RightSquare |]|
//@[46:47)                   Dot |.|
//@[47:54)                   IdentifierSyntax
//@[47:54)                    Identifier |outputs|
//@[54:55)                  Dot |.|
//@[55:63)                  IdentifierSyntax
//@[55:63)                   Identifier |myOutput|
//@[63:64)                NewLine |\n|
              httpPort: 80
//@[14:26)                ObjectPropertySyntax
//@[14:22)                 IdentifierSyntax
//@[14:22)                  Identifier |httpPort|
//@[22:23)                 Colon |:|
//@[24:26)                 IntegerLiteralSyntax
//@[24:26)                  Integer |80|
//@[26:27)                NewLine |\n|
              httpsPort: 443
//@[14:28)                ObjectPropertySyntax
//@[14:23)                 IdentifierSyntax
//@[14:23)                  Identifier |httpsPort|
//@[23:24)                 Colon |:|
//@[25:28)                 IntegerLiteralSyntax
//@[25:28)                  Integer |443|
//@[28:29)                NewLine |\n|
              priority: 1
//@[14:25)                ObjectPropertySyntax
//@[14:22)                 IdentifierSyntax
//@[14:22)                  Identifier |priority|
//@[22:23)                 Colon |:|
//@[24:25)                 IntegerLiteralSyntax
//@[24:25)                  Integer |1|
//@[25:26)                NewLine |\n|
              weight: 50
//@[14:24)                ObjectPropertySyntax
//@[14:20)                 IdentifierSyntax
//@[14:20)                  Identifier |weight|
//@[20:21)                 Colon |:|
//@[22:24)                 IntegerLiteralSyntax
//@[22:24)                  Integer |50|
//@[24:25)                NewLine |\n|
            }
//@[12:13)                RightBrace |}|
//@[13:14)              NewLine |\n|
          ]
//@[10:11)              RightSquare |]|
//@[11:12)            NewLine |\n|
        }
//@[8:9)            RightBrace |}|
//@[9:10)          NewLine |\n|
      }
//@[6:7)          RightBrace |}|
//@[7:8)        NewLine |\n|
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

resource propertyLoopDependencyOnResourceCollection 'Microsoft.Network/frontDoors@2020-05-01' = {
//@[0:871) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:51)  IdentifierSyntax
//@[9:51)   Identifier |propertyLoopDependencyOnResourceCollection|
//@[52:93)  StringSyntax
//@[52:93)   StringComplete |'Microsoft.Network/frontDoors@2020-05-01'|
//@[94:95)  Assignment |=|
//@[96:871)  ObjectSyntax
//@[96:97)   LeftBrace |{|
//@[97:98)   NewLine |\n|
  name: name
//@[2:12)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:12)    VariableAccessSyntax
//@[8:12)     IdentifierSyntax
//@[8:12)      Identifier |name|
//@[12:13)   NewLine |\n|
  location: 'Global'
//@[2:20)   ObjectPropertySyntax
//@[2:10)    IdentifierSyntax
//@[2:10)     Identifier |location|
//@[10:11)    Colon |:|
//@[12:20)    StringSyntax
//@[12:20)     StringComplete |'Global'|
//@[20:21)   NewLine |\n|
  properties: {
//@[2:737)   ObjectPropertySyntax
//@[2:12)    IdentifierSyntax
//@[2:12)     Identifier |properties|
//@[12:13)    Colon |:|
//@[14:737)    ObjectSyntax
//@[14:15)     LeftBrace |{|
//@[15:16)     NewLine |\n|
    backendPools: [
//@[4:717)     ObjectPropertySyntax
//@[4:16)      IdentifierSyntax
//@[4:16)       Identifier |backendPools|
//@[16:17)      Colon |:|
//@[18:717)      ArraySyntax
//@[18:19)       LeftSquare |[|
//@[19:20)       NewLine |\n|
      {
//@[6:691)       ArrayItemSyntax
//@[6:691)        ObjectSyntax
//@[6:7)         LeftBrace |{|
//@[7:8)         NewLine |\n|
        name: 'BackendAPIMs'
//@[8:28)         ObjectPropertySyntax
//@[8:12)          IdentifierSyntax
//@[8:12)           Identifier |name|
//@[12:13)          Colon |:|
//@[14:28)          StringSyntax
//@[14:28)           StringComplete |'BackendAPIMs'|
//@[28:29)         NewLine |\n|
        properties: {
//@[8:646)         ObjectPropertySyntax
//@[8:18)          IdentifierSyntax
//@[8:18)           Identifier |properties|
//@[18:19)          Colon |:|
//@[20:646)          ObjectSyntax
//@[20:21)           LeftBrace |{|
//@[21:22)           NewLine |\n|
          backends: [for index in range(0, length(accounts)): {
//@[10:614)           ObjectPropertySyntax
//@[10:18)            IdentifierSyntax
//@[10:18)             Identifier |backends|
//@[18:19)            Colon |:|
//@[20:614)            ForSyntax
//@[20:21)             LeftSquare |[|
//@[21:24)             Identifier |for|
//@[25:30)             LocalVariableSyntax
//@[25:30)              IdentifierSyntax
//@[25:30)               Identifier |index|
//@[31:33)             Identifier |in|
//@[34:60)             FunctionCallSyntax
//@[34:39)              IdentifierSyntax
//@[34:39)               Identifier |range|
//@[39:40)              LeftParen |(|
//@[40:42)              FunctionArgumentSyntax
//@[40:41)               IntegerLiteralSyntax
//@[40:41)                Integer |0|
//@[41:42)               Comma |,|
//@[43:59)              FunctionArgumentSyntax
//@[43:59)               FunctionCallSyntax
//@[43:49)                IdentifierSyntax
//@[43:49)                 Identifier |length|
//@[49:50)                LeftParen |(|
//@[50:58)                FunctionArgumentSyntax
//@[50:58)                 VariableAccessSyntax
//@[50:58)                  IdentifierSyntax
//@[50:58)                   Identifier |accounts|
//@[58:59)                RightParen |)|
//@[59:60)              RightParen |)|
//@[60:61)             Colon |:|
//@[62:613)             ObjectSyntax
//@[62:63)              LeftBrace |{|
//@[63:64)              NewLine |\n|
            // we cannot codegen index correctly because the generated dependsOn property
//@[89:90)              NewLine |\n|
            // would be outside of the scope of the property loop
//@[65:66)              NewLine |\n|
            // as a result, this will generate a dependency on the entire collection
//@[84:85)              NewLine |\n|
            address: storageAccounts[index].properties.primaryEndpoints.internetEndpoints.web
//@[12:93)              ObjectPropertySyntax
//@[12:19)               IdentifierSyntax
//@[12:19)                Identifier |address|
//@[19:20)               Colon |:|
//@[21:93)               PropertyAccessSyntax
//@[21:89)                PropertyAccessSyntax
//@[21:71)                 PropertyAccessSyntax
//@[21:54)                  PropertyAccessSyntax
//@[21:43)                   ArrayAccessSyntax
//@[21:36)                    VariableAccessSyntax
//@[21:36)                     IdentifierSyntax
//@[21:36)                      Identifier |storageAccounts|
//@[36:37)                    LeftSquare |[|
//@[37:42)                    VariableAccessSyntax
//@[37:42)                     IdentifierSyntax
//@[37:42)                      Identifier |index|
//@[42:43)                    RightSquare |]|
//@[43:44)                   Dot |.|
//@[44:54)                   IdentifierSyntax
//@[44:54)                    Identifier |properties|
//@[54:55)                  Dot |.|
//@[55:71)                  IdentifierSyntax
//@[55:71)                   Identifier |primaryEndpoints|
//@[71:72)                 Dot |.|
//@[72:89)                 IdentifierSyntax
//@[72:89)                  Identifier |internetEndpoints|
//@[89:90)                Dot |.|
//@[90:93)                IdentifierSyntax
//@[90:93)                 Identifier |web|
//@[93:94)              NewLine |\n|
            backendHostHeader: storageAccounts[index].properties.primaryEndpoints.internetEndpoints.web
//@[12:103)              ObjectPropertySyntax
//@[12:29)               IdentifierSyntax
//@[12:29)                Identifier |backendHostHeader|
//@[29:30)               Colon |:|
//@[31:103)               PropertyAccessSyntax
//@[31:99)                PropertyAccessSyntax
//@[31:81)                 PropertyAccessSyntax
//@[31:64)                  PropertyAccessSyntax
//@[31:53)                   ArrayAccessSyntax
//@[31:46)                    VariableAccessSyntax
//@[31:46)                     IdentifierSyntax
//@[31:46)                      Identifier |storageAccounts|
//@[46:47)                    LeftSquare |[|
//@[47:52)                    VariableAccessSyntax
//@[47:52)                     IdentifierSyntax
//@[47:52)                      Identifier |index|
//@[52:53)                    RightSquare |]|
//@[53:54)                   Dot |.|
//@[54:64)                   IdentifierSyntax
//@[54:64)                    Identifier |properties|
//@[64:65)                  Dot |.|
//@[65:81)                  IdentifierSyntax
//@[65:81)                   Identifier |primaryEndpoints|
//@[81:82)                 Dot |.|
//@[82:99)                 IdentifierSyntax
//@[82:99)                  Identifier |internetEndpoints|
//@[99:100)                Dot |.|
//@[100:103)                IdentifierSyntax
//@[100:103)                 Identifier |web|
//@[103:104)              NewLine |\n|
            httpPort: 80
//@[12:24)              ObjectPropertySyntax
//@[12:20)               IdentifierSyntax
//@[12:20)                Identifier |httpPort|
//@[20:21)               Colon |:|
//@[22:24)               IntegerLiteralSyntax
//@[22:24)                Integer |80|
//@[24:25)              NewLine |\n|
            httpsPort: 443
//@[12:26)              ObjectPropertySyntax
//@[12:21)               IdentifierSyntax
//@[12:21)                Identifier |httpsPort|
//@[21:22)               Colon |:|
//@[23:26)               IntegerLiteralSyntax
//@[23:26)                Integer |443|
//@[26:27)              NewLine |\n|
            priority: 1
//@[12:23)              ObjectPropertySyntax
//@[12:20)               IdentifierSyntax
//@[12:20)                Identifier |priority|
//@[20:21)               Colon |:|
//@[22:23)               IntegerLiteralSyntax
//@[22:23)                Integer |1|
//@[23:24)              NewLine |\n|
            weight: 50
//@[12:22)              ObjectPropertySyntax
//@[12:18)               IdentifierSyntax
//@[12:18)                Identifier |weight|
//@[18:19)               Colon |:|
//@[20:22)               IntegerLiteralSyntax
//@[20:22)                Integer |50|
//@[22:23)              NewLine |\n|
          }]
//@[10:11)              RightBrace |}|
//@[11:12)             RightSquare |]|
//@[12:13)           NewLine |\n|
        }
//@[8:9)           RightBrace |}|
//@[9:10)         NewLine |\n|
      }
//@[6:7)         RightBrace |}|
//@[7:8)       NewLine |\n|
    ]
//@[4:5)       RightSquare |]|
//@[5:6)     NewLine |\n|
  }
//@[2:3)     RightBrace |}|
//@[3:4)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

resource indexedResourceCollectionDependency 'Microsoft.Network/frontDoors@2020-05-01' = [for (index,i) in range(0, length(accounts)): {
//@[0:861) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:44)  IdentifierSyntax
//@[9:44)   Identifier |indexedResourceCollectionDependency|
//@[45:86)  StringSyntax
//@[45:86)   StringComplete |'Microsoft.Network/frontDoors@2020-05-01'|
//@[87:88)  Assignment |=|
//@[89:861)  ForSyntax
//@[89:90)   LeftSquare |[|
//@[90:93)   Identifier |for|
//@[94:103)   ForVariableBlockSyntax
//@[94:95)    LeftParen |(|
//@[95:100)    LocalVariableSyntax
//@[95:100)     IdentifierSyntax
//@[95:100)      Identifier |index|
//@[100:101)    Comma |,|
//@[101:102)    LocalVariableSyntax
//@[101:102)     IdentifierSyntax
//@[101:102)      Identifier |i|
//@[102:103)    RightParen |)|
//@[104:106)   Identifier |in|
//@[107:133)   FunctionCallSyntax
//@[107:112)    IdentifierSyntax
//@[107:112)     Identifier |range|
//@[112:113)    LeftParen |(|
//@[113:115)    FunctionArgumentSyntax
//@[113:114)     IntegerLiteralSyntax
//@[113:114)      Integer |0|
//@[114:115)     Comma |,|
//@[116:132)    FunctionArgumentSyntax
//@[116:132)     FunctionCallSyntax
//@[116:122)      IdentifierSyntax
//@[116:122)       Identifier |length|
//@[122:123)      LeftParen |(|
//@[123:131)      FunctionArgumentSyntax
//@[123:131)       VariableAccessSyntax
//@[123:131)        IdentifierSyntax
//@[123:131)         Identifier |accounts|
//@[131:132)      RightParen |)|
//@[132:133)    RightParen |)|
//@[133:134)   Colon |:|
//@[135:860)   ObjectSyntax
//@[135:136)    LeftBrace |{|
//@[136:137)    NewLine |\n|
  name: '${name}-${index}-${i}'
//@[2:31)    ObjectPropertySyntax
//@[2:6)     IdentifierSyntax
//@[2:6)      Identifier |name|
//@[6:7)     Colon |:|
//@[8:31)     StringSyntax
//@[8:11)      StringLeftPiece |'${|
//@[11:15)      VariableAccessSyntax
//@[11:15)       IdentifierSyntax
//@[11:15)        Identifier |name|
//@[15:19)      StringMiddlePiece |}-${|
//@[19:24)      VariableAccessSyntax
//@[19:24)       IdentifierSyntax
//@[19:24)        Identifier |index|
//@[24:28)      StringMiddlePiece |}-${|
//@[28:29)      VariableAccessSyntax
//@[28:29)       IdentifierSyntax
//@[28:29)        Identifier |i|
//@[29:31)      StringRightPiece |}'|
//@[31:32)    NewLine |\n|
  location: 'Global'
//@[2:20)    ObjectPropertySyntax
//@[2:10)     IdentifierSyntax
//@[2:10)      Identifier |location|
//@[10:11)     Colon |:|
//@[12:20)     StringSyntax
//@[12:20)      StringComplete |'Global'|
//@[20:21)    NewLine |\n|
  properties: {
//@[2:668)    ObjectPropertySyntax
//@[2:12)     IdentifierSyntax
//@[2:12)      Identifier |properties|
//@[12:13)     Colon |:|
//@[14:668)     ObjectSyntax
//@[14:15)      LeftBrace |{|
//@[15:16)      NewLine |\n|
    backendPools: [
//@[4:648)      ObjectPropertySyntax
//@[4:16)       IdentifierSyntax
//@[4:16)        Identifier |backendPools|
//@[16:17)       Colon |:|
//@[18:648)       ArraySyntax
//@[18:19)        LeftSquare |[|
//@[19:20)        NewLine |\n|
      {
//@[6:622)        ArrayItemSyntax
//@[6:622)         ObjectSyntax
//@[6:7)          LeftBrace |{|
//@[7:8)          NewLine |\n|
        name: 'BackendAPIMs'
//@[8:28)          ObjectPropertySyntax
//@[8:12)           IdentifierSyntax
//@[8:12)            Identifier |name|
//@[12:13)           Colon |:|
//@[14:28)           StringSyntax
//@[14:28)            StringComplete |'BackendAPIMs'|
//@[28:29)          NewLine |\n|
        properties: {
//@[8:577)          ObjectPropertySyntax
//@[8:18)           IdentifierSyntax
//@[8:18)            Identifier |properties|
//@[18:19)           Colon |:|
//@[20:577)           ObjectSyntax
//@[20:21)            LeftBrace |{|
//@[21:22)            NewLine |\n|
          backends: [
//@[10:545)            ObjectPropertySyntax
//@[10:18)             IdentifierSyntax
//@[10:18)              Identifier |backends|
//@[18:19)             Colon |:|
//@[20:545)             ArraySyntax
//@[20:21)              LeftSquare |[|
//@[21:22)              NewLine |\n|
            {
//@[12:511)              ArrayItemSyntax
//@[12:511)               ObjectSyntax
//@[12:13)                LeftBrace |{|
//@[13:14)                NewLine |\n|
              // this indexed dependency on a module collection will be generated correctly because
//@[99:100)                NewLine |\n|
              // copyIndex() can be invoked in the generated dependsOn
//@[70:71)                NewLine |\n|
              address: storageAccounts[index+i].properties.primaryEndpoints.internetEndpoints.web
//@[14:97)                ObjectPropertySyntax
//@[14:21)                 IdentifierSyntax
//@[14:21)                  Identifier |address|
//@[21:22)                 Colon |:|
//@[23:97)                 PropertyAccessSyntax
//@[23:93)                  PropertyAccessSyntax
//@[23:75)                   PropertyAccessSyntax
//@[23:58)                    PropertyAccessSyntax
//@[23:47)                     ArrayAccessSyntax
//@[23:38)                      VariableAccessSyntax
//@[23:38)                       IdentifierSyntax
//@[23:38)                        Identifier |storageAccounts|
//@[38:39)                      LeftSquare |[|
//@[39:46)                      BinaryOperationSyntax
//@[39:44)                       VariableAccessSyntax
//@[39:44)                        IdentifierSyntax
//@[39:44)                         Identifier |index|
//@[44:45)                       Plus |+|
//@[45:46)                       VariableAccessSyntax
//@[45:46)                        IdentifierSyntax
//@[45:46)                         Identifier |i|
//@[46:47)                      RightSquare |]|
//@[47:48)                     Dot |.|
//@[48:58)                     IdentifierSyntax
//@[48:58)                      Identifier |properties|
//@[58:59)                    Dot |.|
//@[59:75)                    IdentifierSyntax
//@[59:75)                     Identifier |primaryEndpoints|
//@[75:76)                   Dot |.|
//@[76:93)                   IdentifierSyntax
//@[76:93)                    Identifier |internetEndpoints|
//@[93:94)                  Dot |.|
//@[94:97)                  IdentifierSyntax
//@[94:97)                   Identifier |web|
//@[97:98)                NewLine |\n|
              backendHostHeader: storageAccounts[index+i].properties.primaryEndpoints.internetEndpoints.web
//@[14:107)                ObjectPropertySyntax
//@[14:31)                 IdentifierSyntax
//@[14:31)                  Identifier |backendHostHeader|
//@[31:32)                 Colon |:|
//@[33:107)                 PropertyAccessSyntax
//@[33:103)                  PropertyAccessSyntax
//@[33:85)                   PropertyAccessSyntax
//@[33:68)                    PropertyAccessSyntax
//@[33:57)                     ArrayAccessSyntax
//@[33:48)                      VariableAccessSyntax
//@[33:48)                       IdentifierSyntax
//@[33:48)                        Identifier |storageAccounts|
//@[48:49)                      LeftSquare |[|
//@[49:56)                      BinaryOperationSyntax
//@[49:54)                       VariableAccessSyntax
//@[49:54)                        IdentifierSyntax
//@[49:54)                         Identifier |index|
//@[54:55)                       Plus |+|
//@[55:56)                       VariableAccessSyntax
//@[55:56)                        IdentifierSyntax
//@[55:56)                         Identifier |i|
//@[56:57)                      RightSquare |]|
//@[57:58)                     Dot |.|
//@[58:68)                     IdentifierSyntax
//@[58:68)                      Identifier |properties|
//@[68:69)                    Dot |.|
//@[69:85)                    IdentifierSyntax
//@[69:85)                     Identifier |primaryEndpoints|
//@[85:86)                   Dot |.|
//@[86:103)                   IdentifierSyntax
//@[86:103)                    Identifier |internetEndpoints|
//@[103:104)                  Dot |.|
//@[104:107)                  IdentifierSyntax
//@[104:107)                   Identifier |web|
//@[107:108)                NewLine |\n|
              httpPort: 80
//@[14:26)                ObjectPropertySyntax
//@[14:22)                 IdentifierSyntax
//@[14:22)                  Identifier |httpPort|
//@[22:23)                 Colon |:|
//@[24:26)                 IntegerLiteralSyntax
//@[24:26)                  Integer |80|
//@[26:27)                NewLine |\n|
              httpsPort: 443
//@[14:28)                ObjectPropertySyntax
//@[14:23)                 IdentifierSyntax
//@[14:23)                  Identifier |httpsPort|
//@[23:24)                 Colon |:|
//@[25:28)                 IntegerLiteralSyntax
//@[25:28)                  Integer |443|
//@[28:29)                NewLine |\n|
              priority: 1
//@[14:25)                ObjectPropertySyntax
//@[14:22)                 IdentifierSyntax
//@[14:22)                  Identifier |priority|
//@[22:23)                 Colon |:|
//@[24:25)                 IntegerLiteralSyntax
//@[24:25)                  Integer |1|
//@[25:26)                NewLine |\n|
              weight: 50
//@[14:24)                ObjectPropertySyntax
//@[14:20)                 IdentifierSyntax
//@[14:20)                  Identifier |weight|
//@[20:21)                 Colon |:|
//@[22:24)                 IntegerLiteralSyntax
//@[22:24)                  Integer |50|
//@[24:25)                NewLine |\n|
            }
//@[12:13)                RightBrace |}|
//@[13:14)              NewLine |\n|
          ]
//@[10:11)              RightSquare |]|
//@[11:12)            NewLine |\n|
        }
//@[8:9)            RightBrace |}|
//@[9:10)          NewLine |\n|
      }
//@[6:7)          RightBrace |}|
//@[7:8)        NewLine |\n|
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
