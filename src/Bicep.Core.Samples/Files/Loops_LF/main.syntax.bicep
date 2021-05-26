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
//@[0:289) ResourceDeclarationSyntax
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
resource storageAccounts 'Microsoft.Storage/storageAccounts@2019-06-01' = [for account in accounts: {
//@[0:8)  Identifier |resource|
//@[9:24)  IdentifierSyntax
//@[9:24)   Identifier |storageAccounts|
//@[25:71)  StringSyntax
//@[25:71)   StringComplete |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[72:73)  Assignment |=|
//@[74:274)  ForSyntax
//@[74:75)   LeftSquare |[|
//@[75:78)   Identifier |for|
//@[79:86)   LocalVariableSyntax
//@[79:86)    IdentifierSyntax
//@[79:86)     Identifier |account|
//@[87:89)   Identifier |in|
//@[90:98)   VariableAccessSyntax
//@[90:98)    IdentifierSyntax
//@[90:98)     Identifier |accounts|
//@[98:99)   Colon |:|
//@[100:273)   ObjectSyntax
//@[100:101)    LeftBrace |{|
//@[101:102)    NewLine |\n|
  name: '${name}-collection-${account.name}'
//@[2:44)    ObjectPropertySyntax
//@[2:6)     IdentifierSyntax
//@[2:6)      Identifier |name|
//@[6:7)     Colon |:|
//@[8:44)     StringSyntax
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
//@[42:44)      StringRightPiece |}'|
//@[44:45)    NewLine |\n|
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
resource extensionCollection 'Microsoft.Authorization/locks@2016-09-01' = [for i in range(0,1): {
//@[0:212) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:28)  IdentifierSyntax
//@[9:28)   Identifier |extensionCollection|
//@[29:71)  StringSyntax
//@[29:71)   StringComplete |'Microsoft.Authorization/locks@2016-09-01'|
//@[72:73)  Assignment |=|
//@[74:212)  ForSyntax
//@[74:75)   LeftSquare |[|
//@[75:78)   Identifier |for|
//@[79:80)   LocalVariableSyntax
//@[79:80)    IdentifierSyntax
//@[79:80)     Identifier |i|
//@[81:83)   Identifier |in|
//@[84:94)   FunctionCallSyntax
//@[84:89)    IdentifierSyntax
//@[84:89)     Identifier |range|
//@[89:90)    LeftParen |(|
//@[90:92)    FunctionArgumentSyntax
//@[90:91)     IntegerLiteralSyntax
//@[90:91)      Integer |0|
//@[91:92)     Comma |,|
//@[92:93)    FunctionArgumentSyntax
//@[92:93)     IntegerLiteralSyntax
//@[92:93)      Integer |1|
//@[93:94)    RightParen |)|
//@[94:95)   Colon |:|
//@[96:211)   ObjectSyntax
//@[96:97)    LeftBrace |{|
//@[97:98)    NewLine |\n|
  name: 'lock-${i}'
//@[2:19)    ObjectPropertySyntax
//@[2:6)     IdentifierSyntax
//@[2:6)      Identifier |name|
//@[6:7)     Colon |:|
//@[8:19)     StringSyntax
//@[8:16)      StringLeftPiece |'lock-${|
//@[16:17)      VariableAccessSyntax
//@[16:17)       IdentifierSyntax
//@[16:17)        Identifier |i|
//@[17:19)      StringRightPiece |}'|
//@[19:20)    NewLine |\n|
  properties: {
//@[2:67)    ObjectPropertySyntax
//@[2:12)     IdentifierSyntax
//@[2:12)      Identifier |properties|
//@[12:13)     Colon |:|
//@[14:67)     ObjectSyntax
//@[14:15)      LeftBrace |{|
//@[15:16)      NewLine |\n|
    level: i == 0 ? 'CanNotDelete' : 'ReadOnly'
//@[4:47)      ObjectPropertySyntax
//@[4:9)       IdentifierSyntax
//@[4:9)        Identifier |level|
//@[9:10)       Colon |:|
//@[11:47)       TernaryOperationSyntax
//@[11:17)        BinaryOperationSyntax
//@[11:12)         VariableAccessSyntax
//@[11:12)          IdentifierSyntax
//@[11:12)           Identifier |i|
//@[13:15)         Equals |==|
//@[16:17)         IntegerLiteralSyntax
//@[16:17)          Integer |0|
//@[18:19)        Question |?|
//@[20:34)        StringSyntax
//@[20:34)         StringComplete |'CanNotDelete'|
//@[35:36)        Colon |:|
//@[37:47)        StringSyntax
//@[37:47)         StringComplete |'ReadOnly'|
//@[47:48)      NewLine |\n|
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
//@[0:236) ResourceDeclarationSyntax
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
resource lockTheLocks 'Microsoft.Authorization/locks@2016-09-01' = [for i in range(0,1): {
//@[0:8)  Identifier |resource|
//@[9:21)  IdentifierSyntax
//@[9:21)   Identifier |lockTheLocks|
//@[22:64)  StringSyntax
//@[22:64)   StringComplete |'Microsoft.Authorization/locks@2016-09-01'|
//@[65:66)  Assignment |=|
//@[67:222)  ForSyntax
//@[67:68)   LeftSquare |[|
//@[68:71)   Identifier |for|
//@[72:73)   LocalVariableSyntax
//@[72:73)    IdentifierSyntax
//@[72:73)     Identifier |i|
//@[74:76)   Identifier |in|
//@[77:87)   FunctionCallSyntax
//@[77:82)    IdentifierSyntax
//@[77:82)     Identifier |range|
//@[82:83)    LeftParen |(|
//@[83:85)    FunctionArgumentSyntax
//@[83:84)     IntegerLiteralSyntax
//@[83:84)      Integer |0|
//@[84:85)     Comma |,|
//@[85:86)    FunctionArgumentSyntax
//@[85:86)     IntegerLiteralSyntax
//@[85:86)      Integer |1|
//@[86:87)    RightParen |)|
//@[87:88)   Colon |:|
//@[89:221)   ObjectSyntax
//@[89:90)    LeftBrace |{|
//@[90:91)    NewLine |\n|
  name: 'lock-the-lock-${i}'
//@[2:28)    ObjectPropertySyntax
//@[2:6)     IdentifierSyntax
//@[2:6)      Identifier |name|
//@[6:7)     Colon |:|
//@[8:28)     StringSyntax
//@[8:25)      StringLeftPiece |'lock-the-lock-${|
//@[25:26)      VariableAccessSyntax
//@[25:26)       IdentifierSyntax
//@[25:26)        Identifier |i|
//@[26:28)      StringRightPiece |}'|
//@[28:29)    NewLine |\n|
  properties: {
//@[2:67)    ObjectPropertySyntax
//@[2:12)     IdentifierSyntax
//@[2:12)      Identifier |properties|
//@[12:13)     Colon |:|
//@[14:67)     ObjectSyntax
//@[14:15)      LeftBrace |{|
//@[15:16)      NewLine |\n|
    level: i == 0 ? 'CanNotDelete' : 'ReadOnly'
//@[4:47)      ObjectPropertySyntax
//@[4:9)       IdentifierSyntax
//@[4:9)        Identifier |level|
//@[9:10)       Colon |:|
//@[11:47)       TernaryOperationSyntax
//@[11:17)        BinaryOperationSyntax
//@[11:12)         VariableAccessSyntax
//@[11:12)          IdentifierSyntax
//@[11:12)           Identifier |i|
//@[13:15)         Equals |==|
//@[16:17)         IntegerLiteralSyntax
//@[16:17)          Integer |0|
//@[18:19)        Question |?|
//@[20:34)        StringSyntax
//@[20:34)         StringComplete |'CanNotDelete'|
//@[35:36)        Colon |:|
//@[37:47)        StringSyntax
//@[37:47)         StringComplete |'ReadOnly'|
//@[47:48)      NewLine |\n|
  }
//@[2:3)      RightBrace |}|
//@[3:4)    NewLine |\n|
  scope: extensionCollection[i]
//@[2:31)    ObjectPropertySyntax
//@[2:7)     IdentifierSyntax
//@[2:7)      Identifier |scope|
//@[7:8)     Colon |:|
//@[9:31)     ArrayAccessSyntax
//@[9:28)      VariableAccessSyntax
//@[9:28)       IdentifierSyntax
//@[9:28)        Identifier |extensionCollection|
//@[28:29)      LeftSquare |[|
//@[29:30)      VariableAccessSyntax
//@[29:30)       IdentifierSyntax
//@[29:30)        Identifier |i|
//@[30:31)      RightSquare |]|
//@[31:32)    NewLine |\n|
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
resource storageAccounts2 'Microsoft.Storage/storageAccounts@2019-06-01' = [for account in accounts: {
//@[0:276) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:25)  IdentifierSyntax
//@[9:25)   Identifier |storageAccounts2|
//@[26:72)  StringSyntax
//@[26:72)   StringComplete |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[73:74)  Assignment |=|
//@[75:276)  ForSyntax
//@[75:76)   LeftSquare |[|
//@[76:79)   Identifier |for|
//@[80:87)   LocalVariableSyntax
//@[80:87)    IdentifierSyntax
//@[80:87)     Identifier |account|
//@[88:90)   Identifier |in|
//@[91:99)   VariableAccessSyntax
//@[91:99)    IdentifierSyntax
//@[91:99)     Identifier |accounts|
//@[99:100)   Colon |:|
//@[101:275)   ObjectSyntax
//@[101:102)    LeftBrace |{|
//@[102:103)    NewLine |\n|
  name: '${name}-collection-${account.name}'
//@[2:44)    ObjectPropertySyntax
//@[2:6)     IdentifierSyntax
//@[2:6)      Identifier |name|
//@[6:7)     Colon |:|
//@[8:44)     StringSyntax
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
//@[42:44)      StringRightPiece |}'|
//@[44:45)    NewLine |\n|
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
resource firstSet 'Microsoft.Storage/storageAccounts@2019-06-01' = [for i in range(0, length(accounts)): {
//@[0:232) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:17)  IdentifierSyntax
//@[9:17)   Identifier |firstSet|
//@[18:64)  StringSyntax
//@[18:64)   StringComplete |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[65:66)  Assignment |=|
//@[67:232)  ForSyntax
//@[67:68)   LeftSquare |[|
//@[68:71)   Identifier |for|
//@[72:73)   LocalVariableSyntax
//@[72:73)    IdentifierSyntax
//@[72:73)     Identifier |i|
//@[74:76)   Identifier |in|
//@[77:103)   FunctionCallSyntax
//@[77:82)    IdentifierSyntax
//@[77:82)     Identifier |range|
//@[82:83)    LeftParen |(|
//@[83:85)    FunctionArgumentSyntax
//@[83:84)     IntegerLiteralSyntax
//@[83:84)      Integer |0|
//@[84:85)     Comma |,|
//@[86:102)    FunctionArgumentSyntax
//@[86:102)     FunctionCallSyntax
//@[86:92)      IdentifierSyntax
//@[86:92)       Identifier |length|
//@[92:93)      LeftParen |(|
//@[93:101)      FunctionArgumentSyntax
//@[93:101)       VariableAccessSyntax
//@[93:101)        IdentifierSyntax
//@[93:101)         Identifier |accounts|
//@[101:102)      RightParen |)|
//@[102:103)    RightParen |)|
//@[103:104)   Colon |:|
//@[105:231)   ObjectSyntax
//@[105:106)    LeftBrace |{|
//@[106:107)    NewLine |\n|
  name: '${name}-set1-${i}'
//@[2:27)    ObjectPropertySyntax
//@[2:6)     IdentifierSyntax
//@[2:6)      Identifier |name|
//@[6:7)     Colon |:|
//@[8:27)     StringSyntax
//@[8:11)      StringLeftPiece |'${|
//@[11:15)      VariableAccessSyntax
//@[11:15)       IdentifierSyntax
//@[11:15)        Identifier |name|
//@[15:24)      StringMiddlePiece |}-set1-${|
//@[24:25)      VariableAccessSyntax
//@[24:25)       IdentifierSyntax
//@[24:25)        Identifier |i|
//@[25:27)      StringRightPiece |}'|
//@[27:28)    NewLine |\n|
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

resource secondSet 'Microsoft.Storage/storageAccounts@2019-06-01' = [for i in range(0, length(accounts)): {
//@[0:268) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:18)  IdentifierSyntax
//@[9:18)   Identifier |secondSet|
//@[19:65)  StringSyntax
//@[19:65)   StringComplete |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[66:67)  Assignment |=|
//@[68:268)  ForSyntax
//@[68:69)   LeftSquare |[|
//@[69:72)   Identifier |for|
//@[73:74)   LocalVariableSyntax
//@[73:74)    IdentifierSyntax
//@[73:74)     Identifier |i|
//@[75:77)   Identifier |in|
//@[78:104)   FunctionCallSyntax
//@[78:83)    IdentifierSyntax
//@[78:83)     Identifier |range|
//@[83:84)    LeftParen |(|
//@[84:86)    FunctionArgumentSyntax
//@[84:85)     IntegerLiteralSyntax
//@[84:85)      Integer |0|
//@[85:86)     Comma |,|
//@[87:103)    FunctionArgumentSyntax
//@[87:103)     FunctionCallSyntax
//@[87:93)      IdentifierSyntax
//@[87:93)       Identifier |length|
//@[93:94)      LeftParen |(|
//@[94:102)      FunctionArgumentSyntax
//@[94:102)       VariableAccessSyntax
//@[94:102)        IdentifierSyntax
//@[94:102)         Identifier |accounts|
//@[102:103)      RightParen |)|
//@[103:104)    RightParen |)|
//@[104:105)   Colon |:|
//@[106:267)   ObjectSyntax
//@[106:107)    LeftBrace |{|
//@[107:108)    NewLine |\n|
  name: '${name}-set2-${i}'
//@[2:27)    ObjectPropertySyntax
//@[2:6)     IdentifierSyntax
//@[2:6)      Identifier |name|
//@[6:7)     Colon |:|
//@[8:27)     StringSyntax
//@[8:11)      StringLeftPiece |'${|
//@[11:15)      VariableAccessSyntax
//@[11:15)       IdentifierSyntax
//@[11:15)        Identifier |name|
//@[15:24)      StringMiddlePiece |}-set2-${|
//@[24:25)      VariableAccessSyntax
//@[24:25)       IdentifierSyntax
//@[24:25)        Identifier |i|
//@[25:27)      StringRightPiece |}'|
//@[27:28)    NewLine |\n|
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
//@[2:34)    ObjectPropertySyntax
//@[2:11)     IdentifierSyntax
//@[2:11)      Identifier |dependsOn|
//@[11:12)     Colon |:|
//@[13:34)     ArraySyntax
//@[13:14)      LeftSquare |[|
//@[14:15)      NewLine |\n|
    firstSet[i]
//@[4:15)      ArrayItemSyntax
//@[4:15)       ArrayAccessSyntax
//@[4:12)        VariableAccessSyntax
//@[4:12)         IdentifierSyntax
//@[4:12)          Identifier |firstSet|
//@[12:13)        LeftSquare |[|
//@[13:14)        VariableAccessSyntax
//@[13:14)         IdentifierSyntax
//@[13:14)          Identifier |i|
//@[14:15)        RightSquare |]|
//@[15:16)      NewLine |\n|
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

resource vnets 'Microsoft.Network/virtualNetworks@2020-06-01' = [for vnetConfig in vnetConfigurations: {
//@[0:163) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:14)  IdentifierSyntax
//@[9:14)   Identifier |vnets|
//@[15:61)  StringSyntax
//@[15:61)   StringComplete |'Microsoft.Network/virtualNetworks@2020-06-01'|
//@[62:63)  Assignment |=|
//@[64:163)  ForSyntax
//@[64:65)   LeftSquare |[|
//@[65:68)   Identifier |for|
//@[69:79)   LocalVariableSyntax
//@[69:79)    IdentifierSyntax
//@[69:79)     Identifier |vnetConfig|
//@[80:82)   Identifier |in|
//@[83:101)   VariableAccessSyntax
//@[83:101)    IdentifierSyntax
//@[83:101)     Identifier |vnetConfigurations|
//@[101:102)   Colon |:|
//@[103:162)   ObjectSyntax
//@[103:104)    LeftBrace |{|
//@[104:105)    NewLine |\n|
  name: vnetConfig.name
//@[2:23)    ObjectPropertySyntax
//@[2:6)     IdentifierSyntax
//@[2:6)      Identifier |name|
//@[6:7)     Colon |:|
//@[8:23)     PropertyAccessSyntax
//@[8:18)      VariableAccessSyntax
//@[8:18)       IdentifierSyntax
//@[8:18)        Identifier |vnetConfig|
//@[18:19)      Dot |.|
//@[19:23)      IdentifierSyntax
//@[19:23)       Identifier |name|
//@[23:24)    NewLine |\n|
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
//@[0:242) ModuleDeclarationSyntax
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
module moduleCollectionWithSingleDependency 'passthrough.bicep' = [for moduleName in moduleSetup: {
//@[0:6)  Identifier |module|
//@[7:43)  IdentifierSyntax
//@[7:43)   Identifier |moduleCollectionWithSingleDependency|
//@[44:63)  StringSyntax
//@[44:63)   StringComplete |'passthrough.bicep'|
//@[64:65)  Assignment |=|
//@[66:224)  ForSyntax
//@[66:67)   LeftSquare |[|
//@[67:70)   Identifier |for|
//@[71:81)   LocalVariableSyntax
//@[71:81)    IdentifierSyntax
//@[71:81)     Identifier |moduleName|
//@[82:84)   Identifier |in|
//@[85:96)   VariableAccessSyntax
//@[85:96)    IdentifierSyntax
//@[85:96)     Identifier |moduleSetup|
//@[96:97)   Colon |:|
//@[98:223)   ObjectSyntax
//@[98:99)    LeftBrace |{|
//@[99:100)    NewLine |\n|
  name: moduleName
//@[2:18)    ObjectPropertySyntax
//@[2:6)     IdentifierSyntax
//@[2:6)      Identifier |name|
//@[6:7)     Colon |:|
//@[8:18)     VariableAccessSyntax
//@[8:18)      IdentifierSyntax
//@[8:18)       Identifier |moduleName|
//@[18:19)    NewLine |\n|
  params: {
//@[2:47)    ObjectPropertySyntax
//@[2:8)     IdentifierSyntax
//@[2:8)      Identifier |params|
//@[8:9)     Colon |:|
//@[10:47)     ObjectSyntax
//@[10:11)      LeftBrace |{|
//@[11:12)      NewLine |\n|
    myInput: 'in-${moduleName}'
//@[4:31)      ObjectPropertySyntax
//@[4:11)       IdentifierSyntax
//@[4:11)        Identifier |myInput|
//@[11:12)       Colon |:|
//@[13:31)       StringSyntax
//@[13:19)        StringLeftPiece |'in-${|
//@[19:29)        VariableAccessSyntax
//@[19:29)         IdentifierSyntax
//@[19:29)          Identifier |moduleName|
//@[29:31)        StringRightPiece |}'|
//@[31:32)      NewLine |\n|
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
module moduleCollectionWithCollectionDependencies 'passthrough.bicep' = [for moduleName in moduleSetup: {
//@[0:255) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:49)  IdentifierSyntax
//@[7:49)   Identifier |moduleCollectionWithCollectionDependencies|
//@[50:69)  StringSyntax
//@[50:69)   StringComplete |'passthrough.bicep'|
//@[70:71)  Assignment |=|
//@[72:255)  ForSyntax
//@[72:73)   LeftSquare |[|
//@[73:76)   Identifier |for|
//@[77:87)   LocalVariableSyntax
//@[77:87)    IdentifierSyntax
//@[77:87)     Identifier |moduleName|
//@[88:90)   Identifier |in|
//@[91:102)   VariableAccessSyntax
//@[91:102)    IdentifierSyntax
//@[91:102)     Identifier |moduleSetup|
//@[102:103)   Colon |:|
//@[104:254)   ObjectSyntax
//@[104:105)    LeftBrace |{|
//@[105:106)    NewLine |\n|
  name: moduleName
//@[2:18)    ObjectPropertySyntax
//@[2:6)     IdentifierSyntax
//@[2:6)      Identifier |name|
//@[6:7)     Colon |:|
//@[8:18)     VariableAccessSyntax
//@[8:18)      IdentifierSyntax
//@[8:18)       Identifier |moduleName|
//@[18:19)    NewLine |\n|
  params: {
//@[2:47)    ObjectPropertySyntax
//@[2:8)     IdentifierSyntax
//@[2:8)      Identifier |params|
//@[8:9)     Colon |:|
//@[10:47)     ObjectSyntax
//@[10:11)      LeftBrace |{|
//@[11:12)      NewLine |\n|
    myInput: 'in-${moduleName}'
//@[4:31)      ObjectPropertySyntax
//@[4:11)       IdentifierSyntax
//@[4:11)        Identifier |myInput|
//@[11:12)       Colon |:|
//@[13:31)       StringSyntax
//@[13:19)        StringLeftPiece |'in-${|
//@[19:29)        VariableAccessSyntax
//@[19:29)         IdentifierSyntax
//@[19:29)          Identifier |moduleName|
//@[29:31)        StringRightPiece |}'|
//@[31:32)      NewLine |\n|
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

module moduleCollectionWithIndexedDependencies 'passthrough.bicep' = [for moduleName in moduleSetup: {
//@[0:346) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:46)  IdentifierSyntax
//@[7:46)   Identifier |moduleCollectionWithIndexedDependencies|
//@[47:66)  StringSyntax
//@[47:66)   StringComplete |'passthrough.bicep'|
//@[67:68)  Assignment |=|
//@[69:346)  ForSyntax
//@[69:70)   LeftSquare |[|
//@[70:73)   Identifier |for|
//@[74:84)   LocalVariableSyntax
//@[74:84)    IdentifierSyntax
//@[74:84)     Identifier |moduleName|
//@[85:87)   Identifier |in|
//@[88:99)   VariableAccessSyntax
//@[88:99)    IdentifierSyntax
//@[88:99)     Identifier |moduleSetup|
//@[99:100)   Colon |:|
//@[101:345)   ObjectSyntax
//@[101:102)    LeftBrace |{|
//@[102:103)    NewLine |\n|
  name: moduleName
//@[2:18)    ObjectPropertySyntax
//@[2:6)     IdentifierSyntax
//@[2:6)      Identifier |name|
//@[6:7)     Colon |:|
//@[8:18)     VariableAccessSyntax
//@[8:18)      IdentifierSyntax
//@[8:18)       Identifier |moduleName|
//@[18:19)    NewLine |\n|
  params: {
//@[2:170)    ObjectPropertySyntax
//@[2:8)     IdentifierSyntax
//@[2:8)      Identifier |params|
//@[8:9)     Colon |:|
//@[10:170)     ObjectSyntax
//@[10:11)      LeftBrace |{|
//@[11:12)      NewLine |\n|
    myInput: '${moduleCollectionWithCollectionDependencies[index].outputs.myOutput} - ${storageAccounts[index * 3].properties.accessTier} - ${moduleName}'
//@[4:154)      ObjectPropertySyntax
//@[4:11)       IdentifierSyntax
//@[4:11)        Identifier |myInput|
//@[11:12)       Colon |:|
//@[13:154)       StringSyntax
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
//@[152:154)        StringRightPiece |}'|
//@[154:155)      NewLine |\n|
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
resource existingStorageAccounts 'Microsoft.Storage/storageAccounts@2019-06-01' existing = [for account in accounts: {
//@[0:164) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:32)  IdentifierSyntax
//@[9:32)   Identifier |existingStorageAccounts|
//@[33:79)  StringSyntax
//@[33:79)   StringComplete |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[80:88)  Identifier |existing|
//@[89:90)  Assignment |=|
//@[91:164)  ForSyntax
//@[91:92)   LeftSquare |[|
//@[92:95)   Identifier |for|
//@[96:103)   LocalVariableSyntax
//@[96:103)    IdentifierSyntax
//@[96:103)     Identifier |account|
//@[104:106)   Identifier |in|
//@[107:115)   VariableAccessSyntax
//@[107:115)    IdentifierSyntax
//@[107:115)     Identifier |accounts|
//@[115:116)   Colon |:|
//@[117:163)   ObjectSyntax
//@[117:118)    LeftBrace |{|
//@[118:119)    NewLine |\n|
  name: '${name}-existing-${account.name}'
//@[2:42)    ObjectPropertySyntax
//@[2:6)     IdentifierSyntax
//@[2:6)      Identifier |name|
//@[6:7)     Colon |:|
//@[8:42)     StringSyntax
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
//@[40:42)      StringRightPiece |}'|
//@[42:43)    NewLine |\n|
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

resource duplicatedNames 'Microsoft.Network/dnsZones@2018-05-01' = [for zone in []: {
//@[0:136) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:24)  IdentifierSyntax
//@[9:24)   Identifier |duplicatedNames|
//@[25:64)  StringSyntax
//@[25:64)   StringComplete |'Microsoft.Network/dnsZones@2018-05-01'|
//@[65:66)  Assignment |=|
//@[67:136)  ForSyntax
//@[67:68)   LeftSquare |[|
//@[68:71)   Identifier |for|
//@[72:76)   LocalVariableSyntax
//@[72:76)    IdentifierSyntax
//@[72:76)     Identifier |zone|
//@[77:79)   Identifier |in|
//@[80:82)   ArraySyntax
//@[80:81)    LeftSquare |[|
//@[81:82)    RightSquare |]|
//@[82:83)   Colon |:|
//@[84:135)   ObjectSyntax
//@[84:85)    LeftBrace |{|
//@[85:86)    NewLine |\n|
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
resource referenceToDuplicateNames 'Microsoft.Network/dnsZones@2018-05-01' = [for zone in []: {
//@[0:194) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:34)  IdentifierSyntax
//@[9:34)   Identifier |referenceToDuplicateNames|
//@[35:74)  StringSyntax
//@[35:74)   StringComplete |'Microsoft.Network/dnsZones@2018-05-01'|
//@[75:76)  Assignment |=|
//@[77:194)  ForSyntax
//@[77:78)   LeftSquare |[|
//@[78:81)   Identifier |for|
//@[82:86)   LocalVariableSyntax
//@[82:86)    IdentifierSyntax
//@[82:86)     Identifier |zone|
//@[87:89)   Identifier |in|
//@[90:92)   ArraySyntax
//@[90:91)    LeftSquare |[|
//@[91:92)    RightSquare |]|
//@[92:93)   Colon |:|
//@[94:193)   ObjectSyntax
//@[94:95)    LeftBrace |{|
//@[95:96)    NewLine |\n|
  name: 'no loop variable 2'
//@[2:28)    ObjectPropertySyntax
//@[2:6)     IdentifierSyntax
//@[2:6)      Identifier |name|
//@[6:7)     Colon |:|
//@[8:28)     StringSyntax
//@[8:28)      StringComplete |'no loop variable 2'|
//@[28:29)    NewLine |\n|
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

module apim 'passthrough.bicep' = [for region in regions: {
//@[0:131) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:11)  IdentifierSyntax
//@[7:11)   Identifier |apim|
//@[12:31)  StringSyntax
//@[12:31)   StringComplete |'passthrough.bicep'|
//@[32:33)  Assignment |=|
//@[34:131)  ForSyntax
//@[34:35)   LeftSquare |[|
//@[35:38)   Identifier |for|
//@[39:45)   LocalVariableSyntax
//@[39:45)    IdentifierSyntax
//@[39:45)     Identifier |region|
//@[46:48)   Identifier |in|
//@[49:56)   VariableAccessSyntax
//@[49:56)    IdentifierSyntax
//@[49:56)     Identifier |regions|
//@[56:57)   Colon |:|
//@[58:130)   ObjectSyntax
//@[58:59)    LeftBrace |{|
//@[59:60)    NewLine |\n|
  name: 'apim-${region}-${name}'
//@[2:32)    ObjectPropertySyntax
//@[2:6)     IdentifierSyntax
//@[2:6)      Identifier |name|
//@[6:7)     Colon |:|
//@[8:32)     StringSyntax
//@[8:16)      StringLeftPiece |'apim-${|
//@[16:22)      VariableAccessSyntax
//@[16:22)       IdentifierSyntax
//@[16:22)        Identifier |region|
//@[22:26)      StringMiddlePiece |}-${|
//@[26:30)      VariableAccessSyntax
//@[26:30)       IdentifierSyntax
//@[26:30)        Identifier |name|
//@[30:32)      StringRightPiece |}'|
//@[32:33)    NewLine |\n|
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
//@[0:780) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:49)  IdentifierSyntax
//@[9:49)   Identifier |propertyLoopDependencyOnModuleCollection|
//@[50:91)  StringSyntax
//@[50:91)   StringComplete |'Microsoft.Network/frontDoors@2020-05-01'|
//@[92:93)  Assignment |=|
//@[94:780)  ObjectSyntax
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
//@[2:648)   ObjectPropertySyntax
//@[2:12)    IdentifierSyntax
//@[2:12)     Identifier |properties|
//@[12:13)    Colon |:|
//@[14:648)    ObjectSyntax
//@[14:15)     LeftBrace |{|
//@[15:16)     NewLine |\n|
    backendPools: [
//@[4:628)     ObjectPropertySyntax
//@[4:16)      IdentifierSyntax
//@[4:16)       Identifier |backendPools|
//@[16:17)      Colon |:|
//@[18:628)      ArraySyntax
//@[18:19)       LeftSquare |[|
//@[19:20)       NewLine |\n|
      {
//@[6:602)       ArrayItemSyntax
//@[6:602)        ObjectSyntax
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
//@[8:557)         ObjectPropertySyntax
//@[8:18)          IdentifierSyntax
//@[8:18)           Identifier |properties|
//@[18:19)          Colon |:|
//@[20:557)          ObjectSyntax
//@[20:21)           LeftBrace |{|
//@[21:22)           NewLine |\n|
          backends: [for index in range(0, length(regions)): {
//@[10:525)           ObjectPropertySyntax
//@[10:18)            IdentifierSyntax
//@[10:18)             Identifier |backends|
//@[18:19)            Colon |:|
//@[20:525)            ForSyntax
//@[20:21)             LeftSquare |[|
//@[21:24)             Identifier |for|
//@[25:30)             LocalVariableSyntax
//@[25:30)              IdentifierSyntax
//@[25:30)               Identifier |index|
//@[31:33)             Identifier |in|
//@[34:59)             FunctionCallSyntax
//@[34:39)              IdentifierSyntax
//@[34:39)               Identifier |range|
//@[39:40)              LeftParen |(|
//@[40:42)              FunctionArgumentSyntax
//@[40:41)               IntegerLiteralSyntax
//@[40:41)                Integer |0|
//@[41:42)               Comma |,|
//@[43:58)              FunctionArgumentSyntax
//@[43:58)               FunctionCallSyntax
//@[43:49)                IdentifierSyntax
//@[43:49)                 Identifier |length|
//@[49:50)                LeftParen |(|
//@[50:57)                FunctionArgumentSyntax
//@[50:57)                 VariableAccessSyntax
//@[50:57)                  IdentifierSyntax
//@[50:57)                   Identifier |regions|
//@[57:58)                RightParen |)|
//@[58:59)              RightParen |)|
//@[59:60)             Colon |:|
//@[61:524)             ObjectSyntax
//@[61:62)              LeftBrace |{|
//@[62:63)              NewLine |\n|
            // we cannot codegen index correctly because the generated dependsOn property
//@[89:90)              NewLine |\n|
            // would be outside of the scope of the property loop
//@[65:66)              NewLine |\n|
            // as a result, this will generate a dependency on the entire collection
//@[84:85)              NewLine |\n|
            address: apim[index].outputs.myOutput
//@[12:49)              ObjectPropertySyntax
//@[12:19)               IdentifierSyntax
//@[12:19)                Identifier |address|
//@[19:20)               Colon |:|
//@[21:49)               PropertyAccessSyntax
//@[21:40)                PropertyAccessSyntax
//@[21:32)                 ArrayAccessSyntax
//@[21:25)                  VariableAccessSyntax
//@[21:25)                   IdentifierSyntax
//@[21:25)                    Identifier |apim|
//@[25:26)                  LeftSquare |[|
//@[26:31)                  VariableAccessSyntax
//@[26:31)                   IdentifierSyntax
//@[26:31)                    Identifier |index|
//@[31:32)                  RightSquare |]|
//@[32:33)                 Dot |.|
//@[33:40)                 IdentifierSyntax
//@[33:40)                  Identifier |outputs|
//@[40:41)                Dot |.|
//@[41:49)                IdentifierSyntax
//@[41:49)                 Identifier |myOutput|
//@[49:50)              NewLine |\n|
            backendHostHeader: apim[index].outputs.myOutput
//@[12:59)              ObjectPropertySyntax
//@[12:29)               IdentifierSyntax
//@[12:29)                Identifier |backendHostHeader|
//@[29:30)               Colon |:|
//@[31:59)               PropertyAccessSyntax
//@[31:50)                PropertyAccessSyntax
//@[31:42)                 ArrayAccessSyntax
//@[31:35)                  VariableAccessSyntax
//@[31:35)                   IdentifierSyntax
//@[31:35)                    Identifier |apim|
//@[35:36)                  LeftSquare |[|
//@[36:41)                  VariableAccessSyntax
//@[36:41)                   IdentifierSyntax
//@[36:41)                    Identifier |index|
//@[41:42)                  RightSquare |]|
//@[42:43)                 Dot |.|
//@[43:50)                 IdentifierSyntax
//@[43:50)                  Identifier |outputs|
//@[50:51)                Dot |.|
//@[51:59)                IdentifierSyntax
//@[51:59)                 Identifier |myOutput|
//@[59:60)              NewLine |\n|
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

resource indexedModuleCollectionDependency 'Microsoft.Network/frontDoors@2020-05-01' = [for index in range(0, length(regions)): {
//@[0:757) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:42)  IdentifierSyntax
//@[9:42)   Identifier |indexedModuleCollectionDependency|
//@[43:84)  StringSyntax
//@[43:84)   StringComplete |'Microsoft.Network/frontDoors@2020-05-01'|
//@[85:86)  Assignment |=|
//@[87:757)  ForSyntax
//@[87:88)   LeftSquare |[|
//@[88:91)   Identifier |for|
//@[92:97)   LocalVariableSyntax
//@[92:97)    IdentifierSyntax
//@[92:97)     Identifier |index|
//@[98:100)   Identifier |in|
//@[101:126)   FunctionCallSyntax
//@[101:106)    IdentifierSyntax
//@[101:106)     Identifier |range|
//@[106:107)    LeftParen |(|
//@[107:109)    FunctionArgumentSyntax
//@[107:108)     IntegerLiteralSyntax
//@[107:108)      Integer |0|
//@[108:109)     Comma |,|
//@[110:125)    FunctionArgumentSyntax
//@[110:125)     FunctionCallSyntax
//@[110:116)      IdentifierSyntax
//@[110:116)       Identifier |length|
//@[116:117)      LeftParen |(|
//@[117:124)      FunctionArgumentSyntax
//@[117:124)       VariableAccessSyntax
//@[117:124)        IdentifierSyntax
//@[117:124)         Identifier |regions|
//@[124:125)      RightParen |)|
//@[125:126)    RightParen |)|
//@[126:127)   Colon |:|
//@[128:756)   ObjectSyntax
//@[128:129)    LeftBrace |{|
//@[129:130)    NewLine |\n|
  name: '${name}-${index}'
//@[2:26)    ObjectPropertySyntax
//@[2:6)     IdentifierSyntax
//@[2:6)      Identifier |name|
//@[6:7)     Colon |:|
//@[8:26)     StringSyntax
//@[8:11)      StringLeftPiece |'${|
//@[11:15)      VariableAccessSyntax
//@[11:15)       IdentifierSyntax
//@[11:15)        Identifier |name|
//@[15:19)      StringMiddlePiece |}-${|
//@[19:24)      VariableAccessSyntax
//@[19:24)       IdentifierSyntax
//@[19:24)        Identifier |index|
//@[24:26)      StringRightPiece |}'|
//@[26:27)    NewLine |\n|
  location: 'Global'
//@[2:20)    ObjectPropertySyntax
//@[2:10)     IdentifierSyntax
//@[2:10)      Identifier |location|
//@[10:11)     Colon |:|
//@[12:20)     StringSyntax
//@[12:20)      StringComplete |'Global'|
//@[20:21)    NewLine |\n|
  properties: {
//@[2:576)    ObjectPropertySyntax
//@[2:12)     IdentifierSyntax
//@[2:12)      Identifier |properties|
//@[12:13)     Colon |:|
//@[14:576)     ObjectSyntax
//@[14:15)      LeftBrace |{|
//@[15:16)      NewLine |\n|
    backendPools: [
//@[4:556)      ObjectPropertySyntax
//@[4:16)       IdentifierSyntax
//@[4:16)        Identifier |backendPools|
//@[16:17)       Colon |:|
//@[18:556)       ArraySyntax
//@[18:19)        LeftSquare |[|
//@[19:20)        NewLine |\n|
      {
//@[6:530)        ArrayItemSyntax
//@[6:530)         ObjectSyntax
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
//@[8:485)          ObjectPropertySyntax
//@[8:18)           IdentifierSyntax
//@[8:18)            Identifier |properties|
//@[18:19)           Colon |:|
//@[20:485)           ObjectSyntax
//@[20:21)            LeftBrace |{|
//@[21:22)            NewLine |\n|
          backends: [
//@[10:453)            ObjectPropertySyntax
//@[10:18)             IdentifierSyntax
//@[10:18)              Identifier |backends|
//@[18:19)             Colon |:|
//@[20:453)             ArraySyntax
//@[20:21)              LeftSquare |[|
//@[21:22)              NewLine |\n|
            {
//@[12:419)              ArrayItemSyntax
//@[12:419)               ObjectSyntax
//@[12:13)                LeftBrace |{|
//@[13:14)                NewLine |\n|
              // this indexed dependency on a module collection will be generated correctly because
//@[99:100)                NewLine |\n|
              // copyIndex() can be invoked in the generated dependsOn
//@[70:71)                NewLine |\n|
              address: apim[index].outputs.myOutput
//@[14:51)                ObjectPropertySyntax
//@[14:21)                 IdentifierSyntax
//@[14:21)                  Identifier |address|
//@[21:22)                 Colon |:|
//@[23:51)                 PropertyAccessSyntax
//@[23:42)                  PropertyAccessSyntax
//@[23:34)                   ArrayAccessSyntax
//@[23:27)                    VariableAccessSyntax
//@[23:27)                     IdentifierSyntax
//@[23:27)                      Identifier |apim|
//@[27:28)                    LeftSquare |[|
//@[28:33)                    VariableAccessSyntax
//@[28:33)                     IdentifierSyntax
//@[28:33)                      Identifier |index|
//@[33:34)                    RightSquare |]|
//@[34:35)                   Dot |.|
//@[35:42)                   IdentifierSyntax
//@[35:42)                    Identifier |outputs|
//@[42:43)                  Dot |.|
//@[43:51)                  IdentifierSyntax
//@[43:51)                   Identifier |myOutput|
//@[51:52)                NewLine |\n|
              backendHostHeader: apim[index].outputs.myOutput
//@[14:61)                ObjectPropertySyntax
//@[14:31)                 IdentifierSyntax
//@[14:31)                  Identifier |backendHostHeader|
//@[31:32)                 Colon |:|
//@[33:61)                 PropertyAccessSyntax
//@[33:52)                  PropertyAccessSyntax
//@[33:44)                   ArrayAccessSyntax
//@[33:37)                    VariableAccessSyntax
//@[33:37)                     IdentifierSyntax
//@[33:37)                      Identifier |apim|
//@[37:38)                    LeftSquare |[|
//@[38:43)                    VariableAccessSyntax
//@[38:43)                     IdentifierSyntax
//@[38:43)                      Identifier |index|
//@[43:44)                    RightSquare |]|
//@[44:45)                   Dot |.|
//@[45:52)                   IdentifierSyntax
//@[45:52)                    Identifier |outputs|
//@[52:53)                  Dot |.|
//@[53:61)                  IdentifierSyntax
//@[53:61)                   Identifier |myOutput|
//@[61:62)                NewLine |\n|
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

resource indexedResourceCollectionDependency 'Microsoft.Network/frontDoors@2020-05-01' = [for index in range(0, length(accounts)): {
//@[0:848) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:44)  IdentifierSyntax
//@[9:44)   Identifier |indexedResourceCollectionDependency|
//@[45:86)  StringSyntax
//@[45:86)   StringComplete |'Microsoft.Network/frontDoors@2020-05-01'|
//@[87:88)  Assignment |=|
//@[89:848)  ForSyntax
//@[89:90)   LeftSquare |[|
//@[90:93)   Identifier |for|
//@[94:99)   LocalVariableSyntax
//@[94:99)    IdentifierSyntax
//@[94:99)     Identifier |index|
//@[100:102)   Identifier |in|
//@[103:129)   FunctionCallSyntax
//@[103:108)    IdentifierSyntax
//@[103:108)     Identifier |range|
//@[108:109)    LeftParen |(|
//@[109:111)    FunctionArgumentSyntax
//@[109:110)     IntegerLiteralSyntax
//@[109:110)      Integer |0|
//@[110:111)     Comma |,|
//@[112:128)    FunctionArgumentSyntax
//@[112:128)     FunctionCallSyntax
//@[112:118)      IdentifierSyntax
//@[112:118)       Identifier |length|
//@[118:119)      LeftParen |(|
//@[119:127)      FunctionArgumentSyntax
//@[119:127)       VariableAccessSyntax
//@[119:127)        IdentifierSyntax
//@[119:127)         Identifier |accounts|
//@[127:128)      RightParen |)|
//@[128:129)    RightParen |)|
//@[129:130)   Colon |:|
//@[131:847)   ObjectSyntax
//@[131:132)    LeftBrace |{|
//@[132:133)    NewLine |\n|
  name: '${name}-${index}'
//@[2:26)    ObjectPropertySyntax
//@[2:6)     IdentifierSyntax
//@[2:6)      Identifier |name|
//@[6:7)     Colon |:|
//@[8:26)     StringSyntax
//@[8:11)      StringLeftPiece |'${|
//@[11:15)      VariableAccessSyntax
//@[11:15)       IdentifierSyntax
//@[11:15)        Identifier |name|
//@[15:19)      StringMiddlePiece |}-${|
//@[19:24)      VariableAccessSyntax
//@[19:24)       IdentifierSyntax
//@[19:24)        Identifier |index|
//@[24:26)      StringRightPiece |}'|
//@[26:27)    NewLine |\n|
  location: 'Global'
//@[2:20)    ObjectPropertySyntax
//@[2:10)     IdentifierSyntax
//@[2:10)      Identifier |location|
//@[10:11)     Colon |:|
//@[12:20)     StringSyntax
//@[12:20)      StringComplete |'Global'|
//@[20:21)    NewLine |\n|
  properties: {
//@[2:664)    ObjectPropertySyntax
//@[2:12)     IdentifierSyntax
//@[2:12)      Identifier |properties|
//@[12:13)     Colon |:|
//@[14:664)     ObjectSyntax
//@[14:15)      LeftBrace |{|
//@[15:16)      NewLine |\n|
    backendPools: [
//@[4:644)      ObjectPropertySyntax
//@[4:16)       IdentifierSyntax
//@[4:16)        Identifier |backendPools|
//@[16:17)       Colon |:|
//@[18:644)       ArraySyntax
//@[18:19)        LeftSquare |[|
//@[19:20)        NewLine |\n|
      {
//@[6:618)        ArrayItemSyntax
//@[6:618)         ObjectSyntax
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
//@[8:573)          ObjectPropertySyntax
//@[8:18)           IdentifierSyntax
//@[8:18)            Identifier |properties|
//@[18:19)           Colon |:|
//@[20:573)           ObjectSyntax
//@[20:21)            LeftBrace |{|
//@[21:22)            NewLine |\n|
          backends: [
//@[10:541)            ObjectPropertySyntax
//@[10:18)             IdentifierSyntax
//@[10:18)              Identifier |backends|
//@[18:19)             Colon |:|
//@[20:541)             ArraySyntax
//@[20:21)              LeftSquare |[|
//@[21:22)              NewLine |\n|
            {
//@[12:507)              ArrayItemSyntax
//@[12:507)               ObjectSyntax
//@[12:13)                LeftBrace |{|
//@[13:14)                NewLine |\n|
              // this indexed dependency on a module collection will be generated correctly because
//@[99:100)                NewLine |\n|
              // copyIndex() can be invoked in the generated dependsOn
//@[70:71)                NewLine |\n|
              address: storageAccounts[index].properties.primaryEndpoints.internetEndpoints.web
//@[14:95)                ObjectPropertySyntax
//@[14:21)                 IdentifierSyntax
//@[14:21)                  Identifier |address|
//@[21:22)                 Colon |:|
//@[23:95)                 PropertyAccessSyntax
//@[23:91)                  PropertyAccessSyntax
//@[23:73)                   PropertyAccessSyntax
//@[23:56)                    PropertyAccessSyntax
//@[23:45)                     ArrayAccessSyntax
//@[23:38)                      VariableAccessSyntax
//@[23:38)                       IdentifierSyntax
//@[23:38)                        Identifier |storageAccounts|
//@[38:39)                      LeftSquare |[|
//@[39:44)                      VariableAccessSyntax
//@[39:44)                       IdentifierSyntax
//@[39:44)                        Identifier |index|
//@[44:45)                      RightSquare |]|
//@[45:46)                     Dot |.|
//@[46:56)                     IdentifierSyntax
//@[46:56)                      Identifier |properties|
//@[56:57)                    Dot |.|
//@[57:73)                    IdentifierSyntax
//@[57:73)                     Identifier |primaryEndpoints|
//@[73:74)                   Dot |.|
//@[74:91)                   IdentifierSyntax
//@[74:91)                    Identifier |internetEndpoints|
//@[91:92)                  Dot |.|
//@[92:95)                  IdentifierSyntax
//@[92:95)                   Identifier |web|
//@[95:96)                NewLine |\n|
              backendHostHeader: storageAccounts[index].properties.primaryEndpoints.internetEndpoints.web
//@[14:105)                ObjectPropertySyntax
//@[14:31)                 IdentifierSyntax
//@[14:31)                  Identifier |backendHostHeader|
//@[31:32)                 Colon |:|
//@[33:105)                 PropertyAccessSyntax
//@[33:101)                  PropertyAccessSyntax
//@[33:83)                   PropertyAccessSyntax
//@[33:66)                    PropertyAccessSyntax
//@[33:55)                     ArrayAccessSyntax
//@[33:48)                      VariableAccessSyntax
//@[33:48)                       IdentifierSyntax
//@[33:48)                        Identifier |storageAccounts|
//@[48:49)                      LeftSquare |[|
//@[49:54)                      VariableAccessSyntax
//@[49:54)                       IdentifierSyntax
//@[49:54)                        Identifier |index|
//@[54:55)                      RightSquare |]|
//@[55:56)                     Dot |.|
//@[56:66)                     IdentifierSyntax
//@[56:66)                      Identifier |properties|
//@[66:67)                    Dot |.|
//@[67:83)                    IdentifierSyntax
//@[67:83)                     Identifier |primaryEndpoints|
//@[83:84)                   Dot |.|
//@[84:101)                   IdentifierSyntax
//@[84:101)                    Identifier |internetEndpoints|
//@[101:102)                  Dot |.|
//@[102:105)                  IdentifierSyntax
//@[102:105)                   Identifier |web|
//@[105:106)                NewLine |\n|
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

resource filteredZones 'Microsoft.Network/dnsZones@2018-05-01' = [for i in range(0,10): if(i % 3 == 0) {
//@[0:163) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:22)  IdentifierSyntax
//@[9:22)   Identifier |filteredZones|
//@[23:62)  StringSyntax
//@[23:62)   StringComplete |'Microsoft.Network/dnsZones@2018-05-01'|
//@[63:64)  Assignment |=|
//@[65:163)  ForSyntax
//@[65:66)   LeftSquare |[|
//@[66:69)   Identifier |for|
//@[70:71)   LocalVariableSyntax
//@[70:71)    IdentifierSyntax
//@[70:71)     Identifier |i|
//@[72:74)   Identifier |in|
//@[75:86)   FunctionCallSyntax
//@[75:80)    IdentifierSyntax
//@[75:80)     Identifier |range|
//@[80:81)    LeftParen |(|
//@[81:83)    FunctionArgumentSyntax
//@[81:82)     IntegerLiteralSyntax
//@[81:82)      Integer |0|
//@[82:83)     Comma |,|
//@[83:85)    FunctionArgumentSyntax
//@[83:85)     IntegerLiteralSyntax
//@[83:85)      Integer |10|
//@[85:86)    RightParen |)|
//@[86:87)   Colon |:|
//@[88:162)   IfConditionSyntax
//@[88:90)    Identifier |if|
//@[90:102)    ParenthesizedExpressionSyntax
//@[90:91)     LeftParen |(|
//@[91:101)     BinaryOperationSyntax
//@[91:96)      BinaryOperationSyntax
//@[91:92)       VariableAccessSyntax
//@[91:92)        IdentifierSyntax
//@[91:92)         Identifier |i|
//@[93:94)       Modulo |%|
//@[95:96)       IntegerLiteralSyntax
//@[95:96)        Integer |3|
//@[97:99)      Equals |==|
//@[100:101)      IntegerLiteralSyntax
//@[100:101)       Integer |0|
//@[101:102)     RightParen |)|
//@[103:162)    ObjectSyntax
//@[103:104)     LeftBrace |{|
//@[104:105)     NewLine |\n|
  name: 'zone${i}'
//@[2:18)     ObjectPropertySyntax
//@[2:6)      IdentifierSyntax
//@[2:6)       Identifier |name|
//@[6:7)      Colon |:|
//@[8:18)      StringSyntax
//@[8:15)       StringLeftPiece |'zone${|
//@[15:16)       VariableAccessSyntax
//@[15:16)        IdentifierSyntax
//@[15:16)         Identifier |i|
//@[16:18)       StringRightPiece |}'|
//@[18:19)     NewLine |\n|
  location: resourceGroup().location
//@[2:36)     ObjectPropertySyntax
//@[2:10)      IdentifierSyntax
//@[2:10)       Identifier |location|
//@[10:11)      Colon |:|
//@[12:36)      PropertyAccessSyntax
//@[12:27)       FunctionCallSyntax
//@[12:25)        IdentifierSyntax
//@[12:25)         Identifier |resourceGroup|
//@[25:26)        LeftParen |(|
//@[26:27)        RightParen |)|
//@[27:28)       Dot |.|
//@[28:36)       IdentifierSyntax
//@[28:36)        Identifier |location|
//@[36:37)     NewLine |\n|
}]
//@[0:1)     RightBrace |}|
//@[1:2)   RightSquare |]|
//@[2:4) NewLine |\n\n|

module filteredModules 'passthrough.bicep' = [for i in range(0,6): if(i % 2 == 0) {
//@[0:149) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:22)  IdentifierSyntax
//@[7:22)   Identifier |filteredModules|
//@[23:42)  StringSyntax
//@[23:42)   StringComplete |'passthrough.bicep'|
//@[43:44)  Assignment |=|
//@[45:149)  ForSyntax
//@[45:46)   LeftSquare |[|
//@[46:49)   Identifier |for|
//@[50:51)   LocalVariableSyntax
//@[50:51)    IdentifierSyntax
//@[50:51)     Identifier |i|
//@[52:54)   Identifier |in|
//@[55:65)   FunctionCallSyntax
//@[55:60)    IdentifierSyntax
//@[55:60)     Identifier |range|
//@[60:61)    LeftParen |(|
//@[61:63)    FunctionArgumentSyntax
//@[61:62)     IntegerLiteralSyntax
//@[61:62)      Integer |0|
//@[62:63)     Comma |,|
//@[63:64)    FunctionArgumentSyntax
//@[63:64)     IntegerLiteralSyntax
//@[63:64)      Integer |6|
//@[64:65)    RightParen |)|
//@[65:66)   Colon |:|
//@[67:148)   IfConditionSyntax
//@[67:69)    Identifier |if|
//@[69:81)    ParenthesizedExpressionSyntax
//@[69:70)     LeftParen |(|
//@[70:80)     BinaryOperationSyntax
//@[70:75)      BinaryOperationSyntax
//@[70:71)       VariableAccessSyntax
//@[70:71)        IdentifierSyntax
//@[70:71)         Identifier |i|
//@[72:73)       Modulo |%|
//@[74:75)       IntegerLiteralSyntax
//@[74:75)        Integer |2|
//@[76:78)      Equals |==|
//@[79:80)      IntegerLiteralSyntax
//@[79:80)       Integer |0|
//@[80:81)     RightParen |)|
//@[82:148)    ObjectSyntax
//@[82:83)     LeftBrace |{|
//@[83:84)     NewLine |\n|
  name: 'stuff${i}'
//@[2:19)     ObjectPropertySyntax
//@[2:6)      IdentifierSyntax
//@[2:6)       Identifier |name|
//@[6:7)      Colon |:|
//@[8:19)      StringSyntax
//@[8:16)       StringLeftPiece |'stuff${|
//@[16:17)       VariableAccessSyntax
//@[16:17)        IdentifierSyntax
//@[16:17)         Identifier |i|
//@[17:19)       StringRightPiece |}'|
//@[19:20)     NewLine |\n|
  params: {
//@[2:42)     ObjectPropertySyntax
//@[2:8)      IdentifierSyntax
//@[2:8)       Identifier |params|
//@[8:9)      Colon |:|
//@[10:42)      ObjectSyntax
//@[10:11)       LeftBrace |{|
//@[11:12)       NewLine |\n|
    myInput: 'script-${i}'
//@[4:26)       ObjectPropertySyntax
//@[4:11)        IdentifierSyntax
//@[4:11)         Identifier |myInput|
//@[11:12)        Colon |:|
//@[13:26)        StringSyntax
//@[13:23)         StringLeftPiece |'script-${|
//@[23:24)         VariableAccessSyntax
//@[23:24)          IdentifierSyntax
//@[23:24)           Identifier |i|
//@[24:26)         StringRightPiece |}'|
//@[26:27)       NewLine |\n|
  }
//@[2:3)       RightBrace |}|
//@[3:4)     NewLine |\n|
}]
//@[0:1)     RightBrace |}|
//@[1:2)   RightSquare |]|
//@[2:4) NewLine |\n\n|

resource filteredIndexedZones 'Microsoft.Network/dnsZones@2018-05-01' = [for (account, i) in accounts: if(account.enabled) {
//@[0:199) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:29)  IdentifierSyntax
//@[9:29)   Identifier |filteredIndexedZones|
//@[30:69)  StringSyntax
//@[30:69)   StringComplete |'Microsoft.Network/dnsZones@2018-05-01'|
//@[70:71)  Assignment |=|
//@[72:199)  ForSyntax
//@[72:73)   LeftSquare |[|
//@[73:76)   Identifier |for|
//@[77:89)   ForVariableBlockSyntax
//@[77:78)    LeftParen |(|
//@[78:85)    LocalVariableSyntax
//@[78:85)     IdentifierSyntax
//@[78:85)      Identifier |account|
//@[85:86)    Comma |,|
//@[87:88)    LocalVariableSyntax
//@[87:88)     IdentifierSyntax
//@[87:88)      Identifier |i|
//@[88:89)    RightParen |)|
//@[90:92)   Identifier |in|
//@[93:101)   VariableAccessSyntax
//@[93:101)    IdentifierSyntax
//@[93:101)     Identifier |accounts|
//@[101:102)   Colon |:|
//@[103:198)   IfConditionSyntax
//@[103:105)    Identifier |if|
//@[105:122)    ParenthesizedExpressionSyntax
//@[105:106)     LeftParen |(|
//@[106:121)     PropertyAccessSyntax
//@[106:113)      VariableAccessSyntax
//@[106:113)       IdentifierSyntax
//@[106:113)        Identifier |account|
//@[113:114)      Dot |.|
//@[114:121)      IdentifierSyntax
//@[114:121)       Identifier |enabled|
//@[121:122)     RightParen |)|
//@[123:198)    ObjectSyntax
//@[123:124)     LeftBrace |{|
//@[124:125)     NewLine |\n|
  name: 'indexedZone-${account.name}-${i}'
//@[2:42)     ObjectPropertySyntax
//@[2:6)      IdentifierSyntax
//@[2:6)       Identifier |name|
//@[6:7)      Colon |:|
//@[8:42)      StringSyntax
//@[8:23)       StringLeftPiece |'indexedZone-${|
//@[23:35)       PropertyAccessSyntax
//@[23:30)        VariableAccessSyntax
//@[23:30)         IdentifierSyntax
//@[23:30)          Identifier |account|
//@[30:31)        Dot |.|
//@[31:35)        IdentifierSyntax
//@[31:35)         Identifier |name|
//@[35:39)       StringMiddlePiece |}-${|
//@[39:40)       VariableAccessSyntax
//@[39:40)        IdentifierSyntax
//@[39:40)         Identifier |i|
//@[40:42)       StringRightPiece |}'|
//@[42:43)     NewLine |\n|
  location: account.location
//@[2:28)     ObjectPropertySyntax
//@[2:10)      IdentifierSyntax
//@[2:10)       Identifier |location|
//@[10:11)      Colon |:|
//@[12:28)      PropertyAccessSyntax
//@[12:19)       VariableAccessSyntax
//@[12:19)        IdentifierSyntax
//@[12:19)         Identifier |account|
//@[19:20)       Dot |.|
//@[20:28)       IdentifierSyntax
//@[20:28)        Identifier |location|
//@[28:29)     NewLine |\n|
}]
//@[0:1)     RightBrace |}|
//@[1:2)   RightSquare |]|
//@[2:4) NewLine |\n\n|

output lastNameServers array = filteredIndexedZones[length(accounts) - 1].properties.nameServers
//@[0:96) OutputDeclarationSyntax
//@[0:6)  Identifier |output|
//@[7:22)  IdentifierSyntax
//@[7:22)   Identifier |lastNameServers|
//@[23:28)  TypeSyntax
//@[23:28)   Identifier |array|
//@[29:30)  Assignment |=|
//@[31:96)  PropertyAccessSyntax
//@[31:84)   PropertyAccessSyntax
//@[31:73)    ArrayAccessSyntax
//@[31:51)     VariableAccessSyntax
//@[31:51)      IdentifierSyntax
//@[31:51)       Identifier |filteredIndexedZones|
//@[51:52)     LeftSquare |[|
//@[52:72)     BinaryOperationSyntax
//@[52:68)      FunctionCallSyntax
//@[52:58)       IdentifierSyntax
//@[52:58)        Identifier |length|
//@[58:59)       LeftParen |(|
//@[59:67)       FunctionArgumentSyntax
//@[59:67)        VariableAccessSyntax
//@[59:67)         IdentifierSyntax
//@[59:67)          Identifier |accounts|
//@[67:68)       RightParen |)|
//@[69:70)      Minus |-|
//@[71:72)      IntegerLiteralSyntax
//@[71:72)       Integer |1|
//@[72:73)     RightSquare |]|
//@[73:74)    Dot |.|
//@[74:84)    IdentifierSyntax
//@[74:84)     Identifier |properties|
//@[84:85)   Dot |.|
//@[85:96)   IdentifierSyntax
//@[85:96)    Identifier |nameServers|
//@[96:98) NewLine |\n\n|

module filteredIndexedModules 'passthrough.bicep' = [for (account, i) in accounts: if(account.enabled) {
//@[0:187) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:29)  IdentifierSyntax
//@[7:29)   Identifier |filteredIndexedModules|
//@[30:49)  StringSyntax
//@[30:49)   StringComplete |'passthrough.bicep'|
//@[50:51)  Assignment |=|
//@[52:187)  ForSyntax
//@[52:53)   LeftSquare |[|
//@[53:56)   Identifier |for|
//@[57:69)   ForVariableBlockSyntax
//@[57:58)    LeftParen |(|
//@[58:65)    LocalVariableSyntax
//@[58:65)     IdentifierSyntax
//@[58:65)      Identifier |account|
//@[65:66)    Comma |,|
//@[67:68)    LocalVariableSyntax
//@[67:68)     IdentifierSyntax
//@[67:68)      Identifier |i|
//@[68:69)    RightParen |)|
//@[70:72)   Identifier |in|
//@[73:81)   VariableAccessSyntax
//@[73:81)    IdentifierSyntax
//@[73:81)     Identifier |accounts|
//@[81:82)   Colon |:|
//@[83:186)   IfConditionSyntax
//@[83:85)    Identifier |if|
//@[85:102)    ParenthesizedExpressionSyntax
//@[85:86)     LeftParen |(|
//@[86:101)     PropertyAccessSyntax
//@[86:93)      VariableAccessSyntax
//@[86:93)       IdentifierSyntax
//@[86:93)        Identifier |account|
//@[93:94)      Dot |.|
//@[94:101)      IdentifierSyntax
//@[94:101)       Identifier |enabled|
//@[101:102)     RightParen |)|
//@[103:186)    ObjectSyntax
//@[103:104)     LeftBrace |{|
//@[104:105)     NewLine |\n|
  name: 'stuff-${i}'
//@[2:20)     ObjectPropertySyntax
//@[2:6)      IdentifierSyntax
//@[2:6)       Identifier |name|
//@[6:7)      Colon |:|
//@[8:20)      StringSyntax
//@[8:17)       StringLeftPiece |'stuff-${|
//@[17:18)       VariableAccessSyntax
//@[17:18)        IdentifierSyntax
//@[17:18)         Identifier |i|
//@[18:20)       StringRightPiece |}'|
//@[20:21)     NewLine |\n|
  params: {
//@[2:58)     ObjectPropertySyntax
//@[2:8)      IdentifierSyntax
//@[2:8)       Identifier |params|
//@[8:9)      Colon |:|
//@[10:58)      ObjectSyntax
//@[10:11)       LeftBrace |{|
//@[11:12)       NewLine |\n|
    myInput: 'script-${account.name}-${i}'
//@[4:42)       ObjectPropertySyntax
//@[4:11)        IdentifierSyntax
//@[4:11)         Identifier |myInput|
//@[11:12)        Colon |:|
//@[13:42)        StringSyntax
//@[13:23)         StringLeftPiece |'script-${|
//@[23:35)         PropertyAccessSyntax
//@[23:30)          VariableAccessSyntax
//@[23:30)           IdentifierSyntax
//@[23:30)            Identifier |account|
//@[30:31)          Dot |.|
//@[31:35)          IdentifierSyntax
//@[31:35)           Identifier |name|
//@[35:39)         StringMiddlePiece |}-${|
//@[39:40)         VariableAccessSyntax
//@[39:40)          IdentifierSyntax
//@[39:40)           Identifier |i|
//@[40:42)         StringRightPiece |}'|
//@[42:43)       NewLine |\n|
  }
//@[2:3)       RightBrace |}|
//@[3:4)     NewLine |\n|
}]
//@[0:1)     RightBrace |}|
//@[1:2)   RightSquare |]|
//@[2:4) NewLine |\n\n|

output lastModuleOutput string = filteredIndexedModules[length(accounts) - 1].outputs.myOutput
//@[0:94) OutputDeclarationSyntax
//@[0:6)  Identifier |output|
//@[7:23)  IdentifierSyntax
//@[7:23)   Identifier |lastModuleOutput|
//@[24:30)  TypeSyntax
//@[24:30)   Identifier |string|
//@[31:32)  Assignment |=|
//@[33:94)  PropertyAccessSyntax
//@[33:85)   PropertyAccessSyntax
//@[33:77)    ArrayAccessSyntax
//@[33:55)     VariableAccessSyntax
//@[33:55)      IdentifierSyntax
//@[33:55)       Identifier |filteredIndexedModules|
//@[55:56)     LeftSquare |[|
//@[56:76)     BinaryOperationSyntax
//@[56:72)      FunctionCallSyntax
//@[56:62)       IdentifierSyntax
//@[56:62)        Identifier |length|
//@[62:63)       LeftParen |(|
//@[63:71)       FunctionArgumentSyntax
//@[63:71)        VariableAccessSyntax
//@[63:71)         IdentifierSyntax
//@[63:71)          Identifier |accounts|
//@[71:72)       RightParen |)|
//@[73:74)      Minus |-|
//@[75:76)      IntegerLiteralSyntax
//@[75:76)       Integer |1|
//@[76:77)     RightSquare |]|
//@[77:78)    Dot |.|
//@[78:85)    IdentifierSyntax
//@[78:85)     Identifier |outputs|
//@[85:86)   Dot |.|
//@[86:94)   IdentifierSyntax
//@[86:94)    Identifier |myOutput|
//@[94:95) NewLine |\n|

//@[0:0) EndOfFile ||
