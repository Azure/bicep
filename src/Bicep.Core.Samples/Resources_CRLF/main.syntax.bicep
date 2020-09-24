
//@[0:2) NewLine |\r\n|
resource basicStorage 'Microsoft.Storage/storageAccounts@2019-06-01' = {
//@[0:119) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:21)  IdentifierSyntax
//@[9:21)   Identifier |basicStorage|
//@[22:68)  StringSyntax
//@[22:68)   StringComplete |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[69:70)  Assignment |=|
//@[71:119)  ObjectSyntax
//@[71:72)   LeftBrace |{|
//@[72:74)   NewLine |\r\n|
  name: 'basicblobs'
//@[2:22)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:20)    StringSyntax
//@[8:20)     StringComplete |'basicblobs'|
//@[20:22)    NewLine |\r\n|
  location: 'westus'
//@[2:22)   ObjectPropertySyntax
//@[2:10)    IdentifierSyntax
//@[2:10)     Identifier |location|
//@[10:11)    Colon |:|
//@[12:20)    StringSyntax
//@[12:20)     StringComplete |'westus'|
//@[20:22)    NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource dnsZone 'Microsoft.Network/dnszones@2018-05-01' = {
//@[0:103) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:16)  IdentifierSyntax
//@[9:16)   Identifier |dnsZone|
//@[17:56)  StringSyntax
//@[17:56)   StringComplete |'Microsoft.Network/dnszones@2018-05-01'|
//@[57:58)  Assignment |=|
//@[59:103)  ObjectSyntax
//@[59:60)   LeftBrace |{|
//@[60:62)   NewLine |\r\n|
  name: 'myZone'
//@[2:18)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:16)    StringSyntax
//@[8:16)     StringComplete |'myZone'|
//@[16:18)    NewLine |\r\n|
  location: 'global'
//@[2:22)   ObjectPropertySyntax
//@[2:10)    IdentifierSyntax
//@[2:10)     Identifier |location|
//@[10:11)    Colon |:|
//@[12:20)    StringSyntax
//@[12:20)     StringComplete |'global'|
//@[20:22)    NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource myStorageAccount 'Microsoft.Storage/storageAccounts@2017-10-01' = {
//@[0:469) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:25)  IdentifierSyntax
//@[9:25)   Identifier |myStorageAccount|
//@[26:72)  StringSyntax
//@[26:72)   StringComplete |'Microsoft.Storage/storageAccounts@2017-10-01'|
//@[73:74)  Assignment |=|
//@[75:469)  ObjectSyntax
//@[75:76)   LeftBrace |{|
//@[76:78)   NewLine |\r\n|
  name: 'myencryptedone'
//@[2:26)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:24)    StringSyntax
//@[8:24)     StringComplete |'myencryptedone'|
//@[24:26)    NewLine |\r\n|
  location: 'eastus2'
//@[2:23)   ObjectPropertySyntax
//@[2:10)    IdentifierSyntax
//@[2:10)     Identifier |location|
//@[10:11)    Colon |:|
//@[12:21)    StringSyntax
//@[12:21)     StringComplete |'eastus2'|
//@[21:23)    NewLine |\r\n|
  properties: {
//@[2:279)   ObjectPropertySyntax
//@[2:12)    IdentifierSyntax
//@[2:12)     Identifier |properties|
//@[12:13)    Colon |:|
//@[14:277)    ObjectSyntax
//@[14:15)     LeftBrace |{|
//@[15:17)     NewLine |\r\n|
    supportsHttpsTrafficOnly: true
//@[4:36)     ObjectPropertySyntax
//@[4:28)      IdentifierSyntax
//@[4:28)       Identifier |supportsHttpsTrafficOnly|
//@[28:29)      Colon |:|
//@[30:34)      BooleanLiteralSyntax
//@[30:34)       TrueKeyword |true|
//@[34:36)      NewLine |\r\n|
    accessTier: 'Hot'
//@[4:23)     ObjectPropertySyntax
//@[4:14)      IdentifierSyntax
//@[4:14)       Identifier |accessTier|
//@[14:15)      Colon |:|
//@[16:21)      StringSyntax
//@[16:21)       StringComplete |'Hot'|
//@[21:23)      NewLine |\r\n|
    encryption: {
//@[4:198)     ObjectPropertySyntax
//@[4:14)      IdentifierSyntax
//@[4:14)       Identifier |encryption|
//@[14:15)      Colon |:|
//@[16:196)      ObjectSyntax
//@[16:17)       LeftBrace |{|
//@[17:19)       NewLine |\r\n|
      keySource: 'Microsoft.Storage'
//@[6:38)       ObjectPropertySyntax
//@[6:15)        IdentifierSyntax
//@[6:15)         Identifier |keySource|
//@[15:16)        Colon |:|
//@[17:36)        StringSyntax
//@[17:36)         StringComplete |'Microsoft.Storage'|
//@[36:38)        NewLine |\r\n|
      services: {
//@[6:134)       ObjectPropertySyntax
//@[6:14)        IdentifierSyntax
//@[6:14)         Identifier |services|
//@[14:15)        Colon |:|
//@[16:132)        ObjectSyntax
//@[16:17)         LeftBrace |{|
//@[17:19)         NewLine |\r\n|
        blob: {
//@[8:53)         ObjectPropertySyntax
//@[8:12)          IdentifierSyntax
//@[8:12)           Identifier |blob|
//@[12:13)          Colon |:|
//@[14:51)          ObjectSyntax
//@[14:15)           LeftBrace |{|
//@[15:17)           NewLine |\r\n|
          enabled: true
//@[10:25)           ObjectPropertySyntax
//@[10:17)            IdentifierSyntax
//@[10:17)             Identifier |enabled|
//@[17:18)            Colon |:|
//@[19:23)            BooleanLiteralSyntax
//@[19:23)             TrueKeyword |true|
//@[23:25)            NewLine |\r\n|
        }
//@[8:9)           RightBrace |}|
//@[9:11)          NewLine |\r\n|
        file: {
//@[8:53)         ObjectPropertySyntax
//@[8:12)          IdentifierSyntax
//@[8:12)           Identifier |file|
//@[12:13)          Colon |:|
//@[14:51)          ObjectSyntax
//@[14:15)           LeftBrace |{|
//@[15:17)           NewLine |\r\n|
          enabled: true
//@[10:25)           ObjectPropertySyntax
//@[10:17)            IdentifierSyntax
//@[10:17)             Identifier |enabled|
//@[17:18)            Colon |:|
//@[19:23)            BooleanLiteralSyntax
//@[19:23)             TrueKeyword |true|
//@[23:25)            NewLine |\r\n|
        }
//@[8:9)           RightBrace |}|
//@[9:11)          NewLine |\r\n|
      }
//@[6:7)         RightBrace |}|
//@[7:9)        NewLine |\r\n|
    }
//@[4:5)       RightBrace |}|
//@[5:7)      NewLine |\r\n|
  }
//@[2:3)     RightBrace |}|
//@[3:5)    NewLine |\r\n|
  kind: 'StorageV2'
//@[2:21)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |kind|
//@[6:7)    Colon |:|
//@[8:19)    StringSyntax
//@[8:19)     StringComplete |'StorageV2'|
//@[19:21)    NewLine |\r\n|
  sku: {
//@[2:41)   ObjectPropertySyntax
//@[2:5)    IdentifierSyntax
//@[2:5)     Identifier |sku|
//@[5:6)    Colon |:|
//@[7:39)    ObjectSyntax
//@[7:8)     LeftBrace |{|
//@[8:10)     NewLine |\r\n|
    name: 'Standard_LRS'
//@[4:26)     ObjectPropertySyntax
//@[4:8)      IdentifierSyntax
//@[4:8)       Identifier |name|
//@[8:9)      Colon |:|
//@[10:24)      StringSyntax
//@[10:24)       StringComplete |'Standard_LRS'|
//@[24:26)      NewLine |\r\n|
  }
//@[2:3)     RightBrace |}|
//@[3:5)    NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource withExpressions 'Microsoft.Storage/storageAccounts@2017-10-01' = {
//@[0:538) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:24)  IdentifierSyntax
//@[9:24)   Identifier |withExpressions|
//@[25:71)  StringSyntax
//@[25:71)   StringComplete |'Microsoft.Storage/storageAccounts@2017-10-01'|
//@[72:73)  Assignment |=|
//@[74:538)  ObjectSyntax
//@[74:75)   LeftBrace |{|
//@[75:77)   NewLine |\r\n|
  name: 'myencryptedone'
//@[2:26)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:24)    StringSyntax
//@[8:24)     StringComplete |'myencryptedone'|
//@[24:26)    NewLine |\r\n|
  location: 'eastus2'
//@[2:23)   ObjectPropertySyntax
//@[2:10)    IdentifierSyntax
//@[2:10)     Identifier |location|
//@[10:11)    Colon |:|
//@[12:21)    StringSyntax
//@[12:21)     StringComplete |'eastus2'|
//@[21:23)    NewLine |\r\n|
  properties: {
//@[2:306)   ObjectPropertySyntax
//@[2:12)    IdentifierSyntax
//@[2:12)     Identifier |properties|
//@[12:13)    Colon |:|
//@[14:304)    ObjectSyntax
//@[14:15)     LeftBrace |{|
//@[15:17)     NewLine |\r\n|
    supportsHttpsTrafficOnly: !false
//@[4:38)     ObjectPropertySyntax
//@[4:28)      IdentifierSyntax
//@[4:28)       Identifier |supportsHttpsTrafficOnly|
//@[28:29)      Colon |:|
//@[30:36)      UnaryOperationSyntax
//@[30:31)       Exclamation |!|
//@[31:36)       BooleanLiteralSyntax
//@[31:36)        FalseKeyword |false|
//@[36:38)      NewLine |\r\n|
    accessTier: true ? 'Hot' : 'Cold'
//@[4:39)     ObjectPropertySyntax
//@[4:14)      IdentifierSyntax
//@[4:14)       Identifier |accessTier|
//@[14:15)      Colon |:|
//@[16:37)      TernaryOperationSyntax
//@[16:20)       BooleanLiteralSyntax
//@[16:20)        TrueKeyword |true|
//@[21:22)       Question |?|
//@[23:28)       StringSyntax
//@[23:28)        StringComplete |'Hot'|
//@[29:30)       Colon |:|
//@[31:37)       StringSyntax
//@[31:37)        StringComplete |'Cold'|
//@[37:39)      NewLine |\r\n|
    encryption: {
//@[4:207)     ObjectPropertySyntax
//@[4:14)      IdentifierSyntax
//@[4:14)       Identifier |encryption|
//@[14:15)      Colon |:|
//@[16:205)      ObjectSyntax
//@[16:17)       LeftBrace |{|
//@[17:19)       NewLine |\r\n|
      keySource: 'Microsoft.Storage'
//@[6:38)       ObjectPropertySyntax
//@[6:15)        IdentifierSyntax
//@[6:15)         Identifier |keySource|
//@[15:16)        Colon |:|
//@[17:36)        StringSyntax
//@[17:36)         StringComplete |'Microsoft.Storage'|
//@[36:38)        NewLine |\r\n|
      services: {
//@[6:143)       ObjectPropertySyntax
//@[6:14)        IdentifierSyntax
//@[6:14)         Identifier |services|
//@[14:15)        Colon |:|
//@[16:141)        ObjectSyntax
//@[16:17)         LeftBrace |{|
//@[17:19)         NewLine |\r\n|
        blob: {
//@[8:62)         ObjectPropertySyntax
//@[8:12)          IdentifierSyntax
//@[8:12)           Identifier |blob|
//@[12:13)          Colon |:|
//@[14:60)          ObjectSyntax
//@[14:15)           LeftBrace |{|
//@[15:17)           NewLine |\r\n|
          enabled: true || false
//@[10:34)           ObjectPropertySyntax
//@[10:17)            IdentifierSyntax
//@[10:17)             Identifier |enabled|
//@[17:18)            Colon |:|
//@[19:32)            BinaryOperationSyntax
//@[19:23)             BooleanLiteralSyntax
//@[19:23)              TrueKeyword |true|
//@[24:26)             LogicalOr ||||
//@[27:32)             BooleanLiteralSyntax
//@[27:32)              FalseKeyword |false|
//@[32:34)            NewLine |\r\n|
        }
//@[8:9)           RightBrace |}|
//@[9:11)          NewLine |\r\n|
        file: {
//@[8:53)         ObjectPropertySyntax
//@[8:12)          IdentifierSyntax
//@[8:12)           Identifier |file|
//@[12:13)          Colon |:|
//@[14:51)          ObjectSyntax
//@[14:15)           LeftBrace |{|
//@[15:17)           NewLine |\r\n|
          enabled: true
//@[10:25)           ObjectPropertySyntax
//@[10:17)            IdentifierSyntax
//@[10:17)             Identifier |enabled|
//@[17:18)            Colon |:|
//@[19:23)            BooleanLiteralSyntax
//@[19:23)             TrueKeyword |true|
//@[23:25)            NewLine |\r\n|
        }
//@[8:9)           RightBrace |}|
//@[9:11)          NewLine |\r\n|
      }
//@[6:7)         RightBrace |}|
//@[7:9)        NewLine |\r\n|
    }
//@[4:5)       RightBrace |}|
//@[5:7)      NewLine |\r\n|
  }
//@[2:3)     RightBrace |}|
//@[3:5)    NewLine |\r\n|
  kind: 'StorageV2'
//@[2:21)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |kind|
//@[6:7)    Colon |:|
//@[8:19)    StringSyntax
//@[8:19)     StringComplete |'StorageV2'|
//@[19:21)    NewLine |\r\n|
  sku: {
//@[2:41)   ObjectPropertySyntax
//@[2:5)    IdentifierSyntax
//@[2:5)     Identifier |sku|
//@[5:6)    Colon |:|
//@[7:39)    ObjectSyntax
//@[7:8)     LeftBrace |{|
//@[8:10)     NewLine |\r\n|
    name: 'Standard_LRS'
//@[4:26)     ObjectPropertySyntax
//@[4:8)      IdentifierSyntax
//@[4:8)       Identifier |name|
//@[8:9)      Colon |:|
//@[10:24)      StringSyntax
//@[10:24)       StringComplete |'Standard_LRS'|
//@[24:26)      NewLine |\r\n|
  }
//@[2:3)     RightBrace |}|
//@[3:5)    NewLine |\r\n|
  dependsOn: [
//@[2:43)   ObjectPropertySyntax
//@[2:11)    IdentifierSyntax
//@[2:11)     Identifier |dependsOn|
//@[11:12)    Colon |:|
//@[13:41)    ArraySyntax
//@[13:14)     LeftSquare |[|
//@[14:16)     NewLine |\r\n|
    myStorageAccount
//@[4:22)     ArrayItemSyntax
//@[4:20)      VariableAccessSyntax
//@[4:20)       IdentifierSyntax
//@[4:20)        Identifier |myStorageAccount|
//@[20:22)      NewLine |\r\n|
  ]
//@[2:3)     RightSquare |]|
//@[3:5)    NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

param applicationName string = 'to-do-app${uniqueString(resourceGroup().id)}'
//@[0:77) ParameterDeclarationSyntax
//@[0:5)  Identifier |param|
//@[6:21)  IdentifierSyntax
//@[6:21)   Identifier |applicationName|
//@[22:28)  TypeSyntax
//@[22:28)   Identifier |string|
//@[29:77)  ParameterDefaultValueSyntax
//@[29:30)   Assignment |=|
//@[31:77)   StringSyntax
//@[31:43)    StringLeftPiece |'to-do-app${|
//@[43:75)    FunctionCallSyntax
//@[43:55)     IdentifierSyntax
//@[43:55)      Identifier |uniqueString|
//@[55:56)     LeftParen |(|
//@[56:74)     FunctionArgumentSyntax
//@[56:74)      PropertyAccessSyntax
//@[56:71)       FunctionCallSyntax
//@[56:69)        IdentifierSyntax
//@[56:69)         Identifier |resourceGroup|
//@[69:70)        LeftParen |(|
//@[70:71)        RightParen |)|
//@[71:72)       Dot |.|
//@[72:74)       IdentifierSyntax
//@[72:74)        Identifier |id|
//@[74:75)     RightParen |)|
//@[75:77)    StringRightPiece |}'|
//@[77:79) NewLine |\r\n|
var hostingPlanName = applicationName // why not just use the param directly?
//@[0:37) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:19)  IdentifierSyntax
//@[4:19)   Identifier |hostingPlanName|
//@[20:21)  Assignment |=|
//@[22:37)  VariableAccessSyntax
//@[22:37)   IdentifierSyntax
//@[22:37)    Identifier |applicationName|
//@[77:81) NewLine |\r\n\r\n|

param appServicePlanTier string
//@[0:31) ParameterDeclarationSyntax
//@[0:5)  Identifier |param|
//@[6:24)  IdentifierSyntax
//@[6:24)   Identifier |appServicePlanTier|
//@[25:31)  TypeSyntax
//@[25:31)   Identifier |string|
//@[31:33) NewLine |\r\n|
param appServicePlanInstances int
//@[0:33) ParameterDeclarationSyntax
//@[0:5)  Identifier |param|
//@[6:29)  IdentifierSyntax
//@[6:29)   Identifier |appServicePlanInstances|
//@[30:33)  TypeSyntax
//@[30:33)   Identifier |int|
//@[33:37) NewLine |\r\n\r\n|

var location = resourceGroup().location
//@[0:39) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:12)  IdentifierSyntax
//@[4:12)   Identifier |location|
//@[13:14)  Assignment |=|
//@[15:39)  PropertyAccessSyntax
//@[15:30)   FunctionCallSyntax
//@[15:28)    IdentifierSyntax
//@[15:28)     Identifier |resourceGroup|
//@[28:29)    LeftParen |(|
//@[29:30)    RightParen |)|
//@[30:31)   Dot |.|
//@[31:39)   IdentifierSyntax
//@[31:39)    Identifier |location|
//@[39:43) NewLine |\r\n\r\n|

resource farm 'Microsoft.Web/serverFarms@2019-08-01' = {
//@[0:371) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:13)  IdentifierSyntax
//@[9:13)   Identifier |farm|
//@[14:52)  StringSyntax
//@[14:52)   StringComplete |'Microsoft.Web/serverFarms@2019-08-01'|
//@[53:54)  Assignment |=|
//@[55:371)  ObjectSyntax
//@[55:56)   LeftBrace |{|
//@[56:58)   NewLine |\r\n|
  // dependsOn: resourceId('Microsoft.DocumentDB/databaseAccounts', cosmosAccountName)
//@[86:88)   NewLine |\r\n|
  name: hostingPlanName
//@[2:25)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:23)    VariableAccessSyntax
//@[8:23)     IdentifierSyntax
//@[8:23)      Identifier |hostingPlanName|
//@[23:25)    NewLine |\r\n|
  location: location
//@[2:22)   ObjectPropertySyntax
//@[2:10)    IdentifierSyntax
//@[2:10)     Identifier |location|
//@[10:11)    Colon |:|
//@[12:20)    VariableAccessSyntax
//@[12:20)     IdentifierSyntax
//@[12:20)      Identifier |location|
//@[20:22)    NewLine |\r\n|
  sku: {
//@[2:84)   ObjectPropertySyntax
//@[2:5)    IdentifierSyntax
//@[2:5)     Identifier |sku|
//@[5:6)    Colon |:|
//@[7:82)    ObjectSyntax
//@[7:8)     LeftBrace |{|
//@[8:10)     NewLine |\r\n|
    name: appServicePlanTier
//@[4:30)     ObjectPropertySyntax
//@[4:8)      IdentifierSyntax
//@[4:8)       Identifier |name|
//@[8:9)      Colon |:|
//@[10:28)      VariableAccessSyntax
//@[10:28)       IdentifierSyntax
//@[10:28)        Identifier |appServicePlanTier|
//@[28:30)      NewLine |\r\n|
    capacity: appServicePlanInstances
//@[4:39)     ObjectPropertySyntax
//@[4:12)      IdentifierSyntax
//@[4:12)       Identifier |capacity|
//@[12:13)      Colon |:|
//@[14:37)      VariableAccessSyntax
//@[14:37)       IdentifierSyntax
//@[14:37)        Identifier |appServicePlanInstances|
//@[37:39)      NewLine |\r\n|
  }
//@[2:3)     RightBrace |}|
//@[3:5)    NewLine |\r\n|
  properties: {
//@[2:93)   ObjectPropertySyntax
//@[2:12)    IdentifierSyntax
//@[2:12)     Identifier |properties|
//@[12:13)    Colon |:|
//@[14:91)    ObjectSyntax
//@[14:15)     LeftBrace |{|
//@[15:17)     NewLine |\r\n|
    name: hostingPlanName // just hostingPlanName results in an error
//@[4:71)     ObjectPropertySyntax
//@[4:8)      IdentifierSyntax
//@[4:8)       Identifier |name|
//@[8:9)      Colon |:|
//@[10:25)      VariableAccessSyntax
//@[10:25)       IdentifierSyntax
//@[10:25)        Identifier |hostingPlanName|
//@[69:71)      NewLine |\r\n|
  }
//@[2:3)     RightBrace |}|
//@[3:5)    NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

var cosmosDbResourceId = resourceId('Microsoft.DocumentDB/databaseAccounts', cosmosDb.account)
//@[0:94) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:22)  IdentifierSyntax
//@[4:22)   Identifier |cosmosDbResourceId|
//@[23:24)  Assignment |=|
//@[25:94)  FunctionCallSyntax
//@[25:35)   IdentifierSyntax
//@[25:35)    Identifier |resourceId|
//@[35:36)   LeftParen |(|
//@[36:76)   FunctionArgumentSyntax
//@[36:75)    StringSyntax
//@[36:75)     StringComplete |'Microsoft.DocumentDB/databaseAccounts'|
//@[75:76)    Comma |,|
//@[77:93)   FunctionArgumentSyntax
//@[77:93)    PropertyAccessSyntax
//@[77:85)     VariableAccessSyntax
//@[77:85)      IdentifierSyntax
//@[77:85)       Identifier |cosmosDb|
//@[85:86)     Dot |.|
//@[86:93)     IdentifierSyntax
//@[86:93)      Identifier |account|
//@[93:94)   RightParen |)|
//@[94:96) NewLine |\r\n|
var cosmosDbRef = reference(cosmosDbResourceId).documentEndpoint
//@[0:64) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:15)  IdentifierSyntax
//@[4:15)   Identifier |cosmosDbRef|
//@[16:17)  Assignment |=|
//@[18:64)  PropertyAccessSyntax
//@[18:47)   FunctionCallSyntax
//@[18:27)    IdentifierSyntax
//@[18:27)     Identifier |reference|
//@[27:28)    LeftParen |(|
//@[28:46)    FunctionArgumentSyntax
//@[28:46)     VariableAccessSyntax
//@[28:46)      IdentifierSyntax
//@[28:46)       Identifier |cosmosDbResourceId|
//@[46:47)    RightParen |)|
//@[47:48)   Dot |.|
//@[48:64)   IdentifierSyntax
//@[48:64)    Identifier |documentEndpoint|
//@[64:68) NewLine |\r\n\r\n|

// this variable is not accessed anywhere in this template and depends on a run-time reference
//@[94:96) NewLine |\r\n|
// it should not be present at all in the template output as there is nowhere logical to put it
//@[95:97) NewLine |\r\n|
var cosmosDbEndpoint = cosmosDbRef.documentEndpoint
//@[0:51) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:20)  IdentifierSyntax
//@[4:20)   Identifier |cosmosDbEndpoint|
//@[21:22)  Assignment |=|
//@[23:51)  PropertyAccessSyntax
//@[23:34)   VariableAccessSyntax
//@[23:34)    IdentifierSyntax
//@[23:34)     Identifier |cosmosDbRef|
//@[34:35)   Dot |.|
//@[35:51)   IdentifierSyntax
//@[35:51)    Identifier |documentEndpoint|
//@[51:55) NewLine |\r\n\r\n|

param webSiteName string
//@[0:24) ParameterDeclarationSyntax
//@[0:5)  Identifier |param|
//@[6:17)  IdentifierSyntax
//@[6:17)   Identifier |webSiteName|
//@[18:24)  TypeSyntax
//@[18:24)   Identifier |string|
//@[24:26) NewLine |\r\n|
param cosmosDb object
//@[0:21) ParameterDeclarationSyntax
//@[0:5)  Identifier |param|
//@[6:14)  IdentifierSyntax
//@[6:14)   Identifier |cosmosDb|
//@[15:21)  TypeSyntax
//@[15:21)   Identifier |object|
//@[21:23) NewLine |\r\n|
resource site 'Microsoft.Web/sites@2019-08-01' = {
//@[0:689) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:13)  IdentifierSyntax
//@[9:13)   Identifier |site|
//@[14:46)  StringSyntax
//@[14:46)   StringComplete |'Microsoft.Web/sites@2019-08-01'|
//@[47:48)  Assignment |=|
//@[49:689)  ObjectSyntax
//@[49:50)   LeftBrace |{|
//@[50:52)   NewLine |\r\n|
  name: webSiteName
//@[2:21)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:19)    VariableAccessSyntax
//@[8:19)     IdentifierSyntax
//@[8:19)      Identifier |webSiteName|
//@[19:21)    NewLine |\r\n|
  location: location
//@[2:22)   ObjectPropertySyntax
//@[2:10)    IdentifierSyntax
//@[2:10)     Identifier |location|
//@[10:11)    Colon |:|
//@[12:20)    VariableAccessSyntax
//@[12:20)     IdentifierSyntax
//@[12:20)      Identifier |location|
//@[20:22)    NewLine |\r\n|
  properties: {
//@[2:593)   ObjectPropertySyntax
//@[2:12)    IdentifierSyntax
//@[2:12)     Identifier |properties|
//@[12:13)    Colon |:|
//@[14:591)    ObjectSyntax
//@[14:15)     LeftBrace |{|
//@[15:17)     NewLine |\r\n|
    // not yet supported // serverFarmId: farm.id
//@[49:51)     NewLine |\r\n|
    siteConfig: {
//@[4:520)     ObjectPropertySyntax
//@[4:14)      IdentifierSyntax
//@[4:14)       Identifier |siteConfig|
//@[14:15)      Colon |:|
//@[16:518)      ObjectSyntax
//@[16:17)       LeftBrace |{|
//@[17:19)       NewLine |\r\n|
      appSettings: [
//@[6:494)       ObjectPropertySyntax
//@[6:17)        IdentifierSyntax
//@[6:17)         Identifier |appSettings|
//@[17:18)        Colon |:|
//@[19:492)        ArraySyntax
//@[19:20)         LeftSquare |[|
//@[20:22)         NewLine |\r\n|
        {
//@[8:123)         ArrayItemSyntax
//@[8:121)          ObjectSyntax
//@[8:9)           LeftBrace |{|
//@[9:11)           NewLine |\r\n|
          name: 'CosmosDb:Account'
//@[10:36)           ObjectPropertySyntax
//@[10:14)            IdentifierSyntax
//@[10:14)             Identifier |name|
//@[14:15)            Colon |:|
//@[16:34)            StringSyntax
//@[16:34)             StringComplete |'CosmosDb:Account'|
//@[34:36)            NewLine |\r\n|
          value: reference(cosmosDbResourceId).documentEndpoint
//@[10:65)           ObjectPropertySyntax
//@[10:15)            IdentifierSyntax
//@[10:15)             Identifier |value|
//@[15:16)            Colon |:|
//@[17:63)            PropertyAccessSyntax
//@[17:46)             FunctionCallSyntax
//@[17:26)              IdentifierSyntax
//@[17:26)               Identifier |reference|
//@[26:27)              LeftParen |(|
//@[27:45)              FunctionArgumentSyntax
//@[27:45)               VariableAccessSyntax
//@[27:45)                IdentifierSyntax
//@[27:45)                 Identifier |cosmosDbResourceId|
//@[45:46)              RightParen |)|
//@[46:47)             Dot |.|
//@[47:63)             IdentifierSyntax
//@[47:63)              Identifier |documentEndpoint|
//@[63:65)            NewLine |\r\n|
        }
//@[8:9)           RightBrace |}|
//@[9:11)          NewLine |\r\n|
        {
//@[8:132)         ArrayItemSyntax
//@[8:130)          ObjectSyntax
//@[8:9)           LeftBrace |{|
//@[9:11)           NewLine |\r\n|
          name: 'CosmosDb:Key'
//@[10:32)           ObjectPropertySyntax
//@[10:14)            IdentifierSyntax
//@[10:14)             Identifier |name|
//@[14:15)            Colon |:|
//@[16:30)            StringSyntax
//@[16:30)             StringComplete |'CosmosDb:Key'|
//@[30:32)            NewLine |\r\n|
          value: listKeys(cosmosDbResourceId, '2020-04-01').primaryMasterKey
//@[10:78)           ObjectPropertySyntax
//@[10:15)            IdentifierSyntax
//@[10:15)             Identifier |value|
//@[15:16)            Colon |:|
//@[17:76)            PropertyAccessSyntax
//@[17:59)             FunctionCallSyntax
//@[17:25)              IdentifierSyntax
//@[17:25)               Identifier |listKeys|
//@[25:26)              LeftParen |(|
//@[26:45)              FunctionArgumentSyntax
//@[26:44)               VariableAccessSyntax
//@[26:44)                IdentifierSyntax
//@[26:44)                 Identifier |cosmosDbResourceId|
//@[44:45)               Comma |,|
//@[46:58)              FunctionArgumentSyntax
//@[46:58)               StringSyntax
//@[46:58)                StringComplete |'2020-04-01'|
//@[58:59)              RightParen |)|
//@[59:60)             Dot |.|
//@[60:76)             IdentifierSyntax
//@[60:76)              Identifier |primaryMasterKey|
//@[76:78)            NewLine |\r\n|
        }
//@[8:9)           RightBrace |}|
//@[9:11)          NewLine |\r\n|
        {
//@[8:103)         ArrayItemSyntax
//@[8:101)          ObjectSyntax
//@[8:9)           LeftBrace |{|
//@[9:11)           NewLine |\r\n|
          name: 'CosmosDb:DatabaseName'
//@[10:41)           ObjectPropertySyntax
//@[10:14)            IdentifierSyntax
//@[10:14)             Identifier |name|
//@[14:15)            Colon |:|
//@[16:39)            StringSyntax
//@[16:39)             StringComplete |'CosmosDb:DatabaseName'|
//@[39:41)            NewLine |\r\n|
          value: cosmosDb.databaseName
//@[10:40)           ObjectPropertySyntax
//@[10:15)            IdentifierSyntax
//@[10:15)             Identifier |value|
//@[15:16)            Colon |:|
//@[17:38)            PropertyAccessSyntax
//@[17:25)             VariableAccessSyntax
//@[17:25)              IdentifierSyntax
//@[17:25)               Identifier |cosmosDb|
//@[25:26)             Dot |.|
//@[26:38)             IdentifierSyntax
//@[26:38)              Identifier |databaseName|
//@[38:40)            NewLine |\r\n|
        }
//@[8:9)           RightBrace |}|
//@[9:11)          NewLine |\r\n|
        {
//@[8:105)         ArrayItemSyntax
//@[8:103)          ObjectSyntax
//@[8:9)           LeftBrace |{|
//@[9:11)           NewLine |\r\n|
          name: 'CosmosDb:ContainerName'
//@[10:42)           ObjectPropertySyntax
//@[10:14)            IdentifierSyntax
//@[10:14)             Identifier |name|
//@[14:15)            Colon |:|
//@[16:40)            StringSyntax
//@[16:40)             StringComplete |'CosmosDb:ContainerName'|
//@[40:42)            NewLine |\r\n|
          value: cosmosDb.containerName
//@[10:41)           ObjectPropertySyntax
//@[10:15)            IdentifierSyntax
//@[10:15)             Identifier |value|
//@[15:16)            Colon |:|
//@[17:39)            PropertyAccessSyntax
//@[17:25)             VariableAccessSyntax
//@[17:25)              IdentifierSyntax
//@[17:25)               Identifier |cosmosDb|
//@[25:26)             Dot |.|
//@[26:39)             IdentifierSyntax
//@[26:39)              Identifier |containerName|
//@[39:41)            NewLine |\r\n|
        }
//@[8:9)           RightBrace |}|
//@[9:11)          NewLine |\r\n|
      ]
//@[6:7)         RightSquare |]|
//@[7:9)        NewLine |\r\n|
    }
//@[4:5)       RightBrace |}|
//@[5:7)      NewLine |\r\n|
  }
//@[2:3)     RightBrace |}|
//@[3:5)    NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

var _siteApiVersion = site.apiVersion
//@[0:37) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:19)  IdentifierSyntax
//@[4:19)   Identifier |_siteApiVersion|
//@[20:21)  Assignment |=|
//@[22:37)  PropertyAccessSyntax
//@[22:26)   VariableAccessSyntax
//@[22:26)    IdentifierSyntax
//@[22:26)     Identifier |site|
//@[26:27)   Dot |.|
//@[27:37)   IdentifierSyntax
//@[27:37)    Identifier |apiVersion|
//@[37:39) NewLine |\r\n|
var _siteType = site.type
//@[0:25) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:13)  IdentifierSyntax
//@[4:13)   Identifier |_siteType|
//@[14:15)  Assignment |=|
//@[16:25)  PropertyAccessSyntax
//@[16:20)   VariableAccessSyntax
//@[16:20)    IdentifierSyntax
//@[16:20)     Identifier |site|
//@[20:21)   Dot |.|
//@[21:25)   IdentifierSyntax
//@[21:25)    Identifier |type|
//@[25:29) NewLine |\r\n\r\n|

output siteApiVersion string = site.apiVersion
//@[0:46) OutputDeclarationSyntax
//@[0:6)  Identifier |output|
//@[7:21)  IdentifierSyntax
//@[7:21)   Identifier |siteApiVersion|
//@[22:28)  TypeSyntax
//@[22:28)   Identifier |string|
//@[29:30)  Assignment |=|
//@[31:46)  PropertyAccessSyntax
//@[31:35)   VariableAccessSyntax
//@[31:35)    IdentifierSyntax
//@[31:35)     Identifier |site|
//@[35:36)   Dot |.|
//@[36:46)   IdentifierSyntax
//@[36:46)    Identifier |apiVersion|
//@[46:48) NewLine |\r\n|
output siteType string = site.type
//@[0:34) OutputDeclarationSyntax
//@[0:6)  Identifier |output|
//@[7:15)  IdentifierSyntax
//@[7:15)   Identifier |siteType|
//@[16:22)  TypeSyntax
//@[16:22)   Identifier |string|
//@[23:24)  Assignment |=|
//@[25:34)  PropertyAccessSyntax
//@[25:29)   VariableAccessSyntax
//@[25:29)    IdentifierSyntax
//@[25:29)     Identifier |site|
//@[29:30)   Dot |.|
//@[30:34)   IdentifierSyntax
//@[30:34)    Identifier |type|
//@[34:38) NewLine |\r\n\r\n|

resource nested 'Microsoft.Resources/deployments@2019-10-01' = {
//@[0:354) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:15)  IdentifierSyntax
//@[9:15)   Identifier |nested|
//@[16:60)  StringSyntax
//@[16:60)   StringComplete |'Microsoft.Resources/deployments@2019-10-01'|
//@[61:62)  Assignment |=|
//@[63:354)  ObjectSyntax
//@[63:64)   LeftBrace |{|
//@[64:66)   NewLine |\r\n|
  name: 'nestedTemplate1'
//@[2:27)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:25)    StringSyntax
//@[8:25)     StringComplete |'nestedTemplate1'|
//@[25:27)    NewLine |\r\n|
  properties: {
//@[2:260)   ObjectPropertySyntax
//@[2:12)    IdentifierSyntax
//@[2:12)     Identifier |properties|
//@[12:13)    Colon |:|
//@[14:258)    ObjectSyntax
//@[14:15)     LeftBrace |{|
//@[15:17)     NewLine |\r\n|
    mode: 'Incremental'
//@[4:25)     ObjectPropertySyntax
//@[4:8)      IdentifierSyntax
//@[4:8)       Identifier |mode|
//@[8:9)      Colon |:|
//@[10:23)      StringSyntax
//@[10:23)       StringComplete |'Incremental'|
//@[23:25)      NewLine |\r\n|
    template: {
//@[4:213)     ObjectPropertySyntax
//@[4:12)      IdentifierSyntax
//@[4:12)       Identifier |template|
//@[12:13)      Colon |:|
//@[14:211)      ObjectSyntax
//@[14:15)       LeftBrace |{|
//@[15:17)       NewLine |\r\n|
      // string key value
//@[25:27)       NewLine |\r\n|
      '$schema': 'https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#'
//@[6:100)       ObjectPropertySyntax
//@[6:15)        StringSyntax
//@[6:15)         StringComplete |'$schema'|
//@[15:16)        Colon |:|
//@[17:98)        StringSyntax
//@[17:98)         StringComplete |'https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#'|
//@[98:100)        NewLine |\r\n|
      contentVersion: '1.0.0.0'
//@[6:33)       ObjectPropertySyntax
//@[6:20)        IdentifierSyntax
//@[6:20)         Identifier |contentVersion|
//@[20:21)        Colon |:|
//@[22:31)        StringSyntax
//@[22:31)         StringComplete |'1.0.0.0'|
//@[31:33)        NewLine |\r\n|
      resources: [
//@[6:29)       ObjectPropertySyntax
//@[6:15)        IdentifierSyntax
//@[6:15)         Identifier |resources|
//@[15:16)        Colon |:|
//@[17:27)        ArraySyntax
//@[17:18)         LeftSquare |[|
//@[18:20)         NewLine |\r\n|
      ]
//@[6:7)         RightSquare |]|
//@[7:9)        NewLine |\r\n|
    }
//@[4:5)       RightBrace |}|
//@[5:7)      NewLine |\r\n|
  }
//@[2:3)     RightBrace |}|
//@[3:5)    NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

// should be able to access the read only properties
//@[52:54) NewLine |\r\n|
resource accessingReadOnlyProperties 'Microsoft.Foo/foos@2019-10-01' = {
//@[0:284) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:36)  IdentifierSyntax
//@[9:36)   Identifier |accessingReadOnlyProperties|
//@[37:68)  StringSyntax
//@[37:68)   StringComplete |'Microsoft.Foo/foos@2019-10-01'|
//@[69:70)  Assignment |=|
//@[71:284)  ObjectSyntax
//@[71:72)   LeftBrace |{|
//@[72:74)   NewLine |\r\n|
  name: 'nestedTemplate1'
//@[2:27)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:25)    StringSyntax
//@[8:25)     StringComplete |'nestedTemplate1'|
//@[25:27)    NewLine |\r\n|
  properties: {
//@[2:182)   ObjectPropertySyntax
//@[2:12)    IdentifierSyntax
//@[2:12)     Identifier |properties|
//@[12:13)    Colon |:|
//@[14:180)    ObjectSyntax
//@[14:15)     LeftBrace |{|
//@[15:17)     NewLine |\r\n|
    otherId: nested.id
//@[4:24)     ObjectPropertySyntax
//@[4:11)      IdentifierSyntax
//@[4:11)       Identifier |otherId|
//@[11:12)      Colon |:|
//@[13:22)      PropertyAccessSyntax
//@[13:19)       VariableAccessSyntax
//@[13:19)        IdentifierSyntax
//@[13:19)         Identifier |nested|
//@[19:20)       Dot |.|
//@[20:22)       IdentifierSyntax
//@[20:22)        Identifier |id|
//@[22:24)      NewLine |\r\n|
    otherName: nested.name
//@[4:28)     ObjectPropertySyntax
//@[4:13)      IdentifierSyntax
//@[4:13)       Identifier |otherName|
//@[13:14)      Colon |:|
//@[15:26)      PropertyAccessSyntax
//@[15:21)       VariableAccessSyntax
//@[15:21)        IdentifierSyntax
//@[15:21)         Identifier |nested|
//@[21:22)       Dot |.|
//@[22:26)       IdentifierSyntax
//@[22:26)        Identifier |name|
//@[26:28)      NewLine |\r\n|
    otherVersion: nested.apiVersion
//@[4:37)     ObjectPropertySyntax
//@[4:16)      IdentifierSyntax
//@[4:16)       Identifier |otherVersion|
//@[16:17)      Colon |:|
//@[18:35)      PropertyAccessSyntax
//@[18:24)       VariableAccessSyntax
//@[18:24)        IdentifierSyntax
//@[18:24)         Identifier |nested|
//@[24:25)       Dot |.|
//@[25:35)       IdentifierSyntax
//@[25:35)        Identifier |apiVersion|
//@[35:37)      NewLine |\r\n|
    otherType: nested.type
//@[4:30)     ObjectPropertySyntax
//@[4:13)      IdentifierSyntax
//@[4:13)       Identifier |otherType|
//@[13:14)      Colon |:|
//@[15:26)      PropertyAccessSyntax
//@[15:21)       VariableAccessSyntax
//@[15:21)        IdentifierSyntax
//@[15:21)         Identifier |nested|
//@[21:22)       Dot |.|
//@[22:26)       IdentifierSyntax
//@[22:26)        Identifier |type|
//@[26:30)      NewLine |\r\n\r\n|

    otherThings: nested.properties.mode
//@[4:41)     ObjectPropertySyntax
//@[4:15)      IdentifierSyntax
//@[4:15)       Identifier |otherThings|
//@[15:16)      Colon |:|
//@[17:39)      PropertyAccessSyntax
//@[17:34)       PropertyAccessSyntax
//@[17:23)        VariableAccessSyntax
//@[17:23)         IdentifierSyntax
//@[17:23)          Identifier |nested|
//@[23:24)        Dot |.|
//@[24:34)        IdentifierSyntax
//@[24:34)         Identifier |properties|
//@[34:35)       Dot |.|
//@[35:39)       IdentifierSyntax
//@[35:39)        Identifier |mode|
//@[39:41)      NewLine |\r\n|
  }
//@[2:3)     RightBrace |}|
//@[3:5)    NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource resourceA 'My.Rp/typeA@2020-01-01' = {
//@[0:71) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:18)  IdentifierSyntax
//@[9:18)   Identifier |resourceA|
//@[19:43)  StringSyntax
//@[19:43)   StringComplete |'My.Rp/typeA@2020-01-01'|
//@[44:45)  Assignment |=|
//@[46:71)  ObjectSyntax
//@[46:47)   LeftBrace |{|
//@[47:49)   NewLine |\r\n|
  name: 'resourceA'
//@[2:21)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:19)    StringSyntax
//@[8:19)     StringComplete |'resourceA'|
//@[19:21)    NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource resourceB 'My.Rp/typeA/typeB@2020-01-01' = {
//@[0:92) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:18)  IdentifierSyntax
//@[9:18)   Identifier |resourceB|
//@[19:49)  StringSyntax
//@[19:49)   StringComplete |'My.Rp/typeA/typeB@2020-01-01'|
//@[50:51)  Assignment |=|
//@[52:92)  ObjectSyntax
//@[52:53)   LeftBrace |{|
//@[53:55)   NewLine |\r\n|
  name: '${resourceA.name}/myName'
//@[2:36)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:34)    StringSyntax
//@[8:11)     StringLeftPiece |'${|
//@[11:25)     PropertyAccessSyntax
//@[11:20)      VariableAccessSyntax
//@[11:20)       IdentifierSyntax
//@[11:20)        Identifier |resourceA|
//@[20:21)      Dot |.|
//@[21:25)      IdentifierSyntax
//@[21:25)       Identifier |name|
//@[25:34)     StringRightPiece |}/myName'|
//@[34:36)    NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource resourceC 'My.Rp/typeA/typeB@2020-01-01' = {
//@[0:269) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:18)  IdentifierSyntax
//@[9:18)   Identifier |resourceC|
//@[19:49)  StringSyntax
//@[19:49)   StringComplete |'My.Rp/typeA/typeB@2020-01-01'|
//@[50:51)  Assignment |=|
//@[52:269)  ObjectSyntax
//@[52:53)   LeftBrace |{|
//@[53:55)   NewLine |\r\n|
  name: '${resourceA.name}/myName'
//@[2:36)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:34)    StringSyntax
//@[8:11)     StringLeftPiece |'${|
//@[11:25)     PropertyAccessSyntax
//@[11:20)      VariableAccessSyntax
//@[11:20)       IdentifierSyntax
//@[11:20)        Identifier |resourceA|
//@[20:21)      Dot |.|
//@[21:25)      IdentifierSyntax
//@[21:25)       Identifier |name|
//@[25:34)     StringRightPiece |}/myName'|
//@[34:36)    NewLine |\r\n|
  properties: {
//@[2:177)   ObjectPropertySyntax
//@[2:12)    IdentifierSyntax
//@[2:12)     Identifier |properties|
//@[12:13)    Colon |:|
//@[14:175)    ObjectSyntax
//@[14:15)     LeftBrace |{|
//@[15:17)     NewLine |\r\n|
    aId: resourceA.id
//@[4:23)     ObjectPropertySyntax
//@[4:7)      IdentifierSyntax
//@[4:7)       Identifier |aId|
//@[7:8)      Colon |:|
//@[9:21)      PropertyAccessSyntax
//@[9:18)       VariableAccessSyntax
//@[9:18)        IdentifierSyntax
//@[9:18)         Identifier |resourceA|
//@[18:19)       Dot |.|
//@[19:21)       IdentifierSyntax
//@[19:21)        Identifier |id|
//@[21:23)      NewLine |\r\n|
    aType: resourceA.type
//@[4:27)     ObjectPropertySyntax
//@[4:9)      IdentifierSyntax
//@[4:9)       Identifier |aType|
//@[9:10)      Colon |:|
//@[11:25)      PropertyAccessSyntax
//@[11:20)       VariableAccessSyntax
//@[11:20)        IdentifierSyntax
//@[11:20)         Identifier |resourceA|
//@[20:21)       Dot |.|
//@[21:25)       IdentifierSyntax
//@[21:25)        Identifier |type|
//@[25:27)      NewLine |\r\n|
    aName: resourceA.name
//@[4:27)     ObjectPropertySyntax
//@[4:9)      IdentifierSyntax
//@[4:9)       Identifier |aName|
//@[9:10)      Colon |:|
//@[11:25)      PropertyAccessSyntax
//@[11:20)       VariableAccessSyntax
//@[11:20)        IdentifierSyntax
//@[11:20)         Identifier |resourceA|
//@[20:21)       Dot |.|
//@[21:25)       IdentifierSyntax
//@[21:25)        Identifier |name|
//@[25:27)      NewLine |\r\n|
    aApiVersion: resourceA.apiVersion
//@[4:39)     ObjectPropertySyntax
//@[4:15)      IdentifierSyntax
//@[4:15)       Identifier |aApiVersion|
//@[15:16)      Colon |:|
//@[17:37)      PropertyAccessSyntax
//@[17:26)       VariableAccessSyntax
//@[17:26)        IdentifierSyntax
//@[17:26)         Identifier |resourceA|
//@[26:27)       Dot |.|
//@[27:37)       IdentifierSyntax
//@[27:37)        Identifier |apiVersion|
//@[37:39)      NewLine |\r\n|
    bProperties: resourceB.properties
//@[4:39)     ObjectPropertySyntax
//@[4:15)      IdentifierSyntax
//@[4:15)       Identifier |bProperties|
//@[15:16)      Colon |:|
//@[17:37)      PropertyAccessSyntax
//@[17:26)       VariableAccessSyntax
//@[17:26)        IdentifierSyntax
//@[17:26)         Identifier |resourceB|
//@[26:27)       Dot |.|
//@[27:37)       IdentifierSyntax
//@[27:37)        Identifier |properties|
//@[37:39)      NewLine |\r\n|
  }
//@[2:3)     RightBrace |}|
//@[3:5)    NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

var varARuntime = {
//@[0:155) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:15)  IdentifierSyntax
//@[4:15)   Identifier |varARuntime|
//@[16:17)  Assignment |=|
//@[18:155)  ObjectSyntax
//@[18:19)   LeftBrace |{|
//@[19:21)   NewLine |\r\n|
  bId: resourceB.id
//@[2:21)   ObjectPropertySyntax
//@[2:5)    IdentifierSyntax
//@[2:5)     Identifier |bId|
//@[5:6)    Colon |:|
//@[7:19)    PropertyAccessSyntax
//@[7:16)     VariableAccessSyntax
//@[7:16)      IdentifierSyntax
//@[7:16)       Identifier |resourceB|
//@[16:17)     Dot |.|
//@[17:19)     IdentifierSyntax
//@[17:19)      Identifier |id|
//@[19:21)    NewLine |\r\n|
  bType: resourceB.type
//@[2:25)   ObjectPropertySyntax
//@[2:7)    IdentifierSyntax
//@[2:7)     Identifier |bType|
//@[7:8)    Colon |:|
//@[9:23)    PropertyAccessSyntax
//@[9:18)     VariableAccessSyntax
//@[9:18)      IdentifierSyntax
//@[9:18)       Identifier |resourceB|
//@[18:19)     Dot |.|
//@[19:23)     IdentifierSyntax
//@[19:23)      Identifier |type|
//@[23:25)    NewLine |\r\n|
  bName: resourceB.name
//@[2:25)   ObjectPropertySyntax
//@[2:7)    IdentifierSyntax
//@[2:7)     Identifier |bName|
//@[7:8)    Colon |:|
//@[9:23)    PropertyAccessSyntax
//@[9:18)     VariableAccessSyntax
//@[9:18)      IdentifierSyntax
//@[9:18)       Identifier |resourceB|
//@[18:19)     Dot |.|
//@[19:23)     IdentifierSyntax
//@[19:23)      Identifier |name|
//@[23:25)    NewLine |\r\n|
  bApiVersion: resourceB.apiVersion
//@[2:37)   ObjectPropertySyntax
//@[2:13)    IdentifierSyntax
//@[2:13)     Identifier |bApiVersion|
//@[13:14)    Colon |:|
//@[15:35)    PropertyAccessSyntax
//@[15:24)     VariableAccessSyntax
//@[15:24)      IdentifierSyntax
//@[15:24)       Identifier |resourceB|
//@[24:25)     Dot |.|
//@[25:35)     IdentifierSyntax
//@[25:35)      Identifier |apiVersion|
//@[35:37)    NewLine |\r\n|
  aKind: resourceA.kind
//@[2:25)   ObjectPropertySyntax
//@[2:7)    IdentifierSyntax
//@[2:7)     Identifier |aKind|
//@[7:8)    Colon |:|
//@[9:23)    PropertyAccessSyntax
//@[9:18)     VariableAccessSyntax
//@[9:18)      IdentifierSyntax
//@[9:18)       Identifier |resourceA|
//@[18:19)     Dot |.|
//@[19:23)     IdentifierSyntax
//@[19:23)      Identifier |kind|
//@[23:25)    NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

var varBRuntime = [
//@[0:37) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:15)  IdentifierSyntax
//@[4:15)   Identifier |varBRuntime|
//@[16:17)  Assignment |=|
//@[18:37)  ArraySyntax
//@[18:19)   LeftSquare |[|
//@[19:21)   NewLine |\r\n|
  varARuntime
//@[2:15)   ArrayItemSyntax
//@[2:13)    VariableAccessSyntax
//@[2:13)     IdentifierSyntax
//@[2:13)      Identifier |varARuntime|
//@[13:15)    NewLine |\r\n|
]
//@[0:1)   RightSquare |]|
//@[1:5) NewLine |\r\n\r\n|

var resourceCRef = {
//@[0:43) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:16)  IdentifierSyntax
//@[4:16)   Identifier |resourceCRef|
//@[17:18)  Assignment |=|
//@[19:43)  ObjectSyntax
//@[19:20)   LeftBrace |{|
//@[20:22)   NewLine |\r\n|
  id: resourceC.id
//@[2:20)   ObjectPropertySyntax
//@[2:4)    IdentifierSyntax
//@[2:4)     Identifier |id|
//@[4:5)    Colon |:|
//@[6:18)    PropertyAccessSyntax
//@[6:15)     VariableAccessSyntax
//@[6:15)      IdentifierSyntax
//@[6:15)       Identifier |resourceC|
//@[15:16)     Dot |.|
//@[16:18)     IdentifierSyntax
//@[16:18)      Identifier |id|
//@[18:20)    NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\r\n|
var setResourceCRef = true
//@[0:26) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:19)  IdentifierSyntax
//@[4:19)   Identifier |setResourceCRef|
//@[20:21)  Assignment |=|
//@[22:26)  BooleanLiteralSyntax
//@[22:26)   TrueKeyword |true|
//@[26:30) NewLine |\r\n\r\n|

resource resourceD 'My.Rp/typeD@2020-01-01' = {
//@[0:231) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:18)  IdentifierSyntax
//@[9:18)   Identifier |resourceD|
//@[19:43)  StringSyntax
//@[19:43)   StringComplete |'My.Rp/typeD@2020-01-01'|
//@[44:45)  Assignment |=|
//@[46:231)  ObjectSyntax
//@[46:47)   LeftBrace |{|
//@[47:49)   NewLine |\r\n|
  name: 'constant'
//@[2:20)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:18)    StringSyntax
//@[8:18)     StringComplete |'constant'|
//@[18:20)    NewLine |\r\n|
  properties: {
//@[2:161)   ObjectPropertySyntax
//@[2:12)    IdentifierSyntax
//@[2:12)     Identifier |properties|
//@[12:13)    Colon |:|
//@[14:159)    ObjectSyntax
//@[14:15)     LeftBrace |{|
//@[15:17)     NewLine |\r\n|
    runtime: varBRuntime
//@[4:86)     ObjectPropertySyntax
//@[4:11)      IdentifierSyntax
//@[4:11)       Identifier |runtime|
//@[11:12)      Colon |:|
//@[13:24)      VariableAccessSyntax
//@[13:24)       IdentifierSyntax
//@[13:24)        Identifier |varBRuntime|
//@[24:26)      NewLine |\r\n|
    // repro for https://github.com/Azure/bicep/issues/316
//@[58:60)      NewLine |\r\n|
    repro316: setResourceCRef ? resourceCRef : null
//@[4:53)     ObjectPropertySyntax
//@[4:12)      IdentifierSyntax
//@[4:12)       Identifier |repro316|
//@[12:13)      Colon |:|
//@[14:51)      TernaryOperationSyntax
//@[14:29)       VariableAccessSyntax
//@[14:29)        IdentifierSyntax
//@[14:29)         Identifier |setResourceCRef|
//@[30:31)       Question |?|
//@[32:44)       VariableAccessSyntax
//@[32:44)        IdentifierSyntax
//@[32:44)         Identifier |resourceCRef|
//@[45:46)       Colon |:|
//@[47:51)       NullLiteralSyntax
//@[47:51)        NullKeyword |null|
//@[51:53)      NewLine |\r\n|
  }
//@[2:3)     RightBrace |}|
//@[3:5)    NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:1) EndOfFile ||
