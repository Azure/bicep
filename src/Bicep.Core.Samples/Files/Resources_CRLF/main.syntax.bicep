
//@[0:2) NewLine |\r\n|
@sys.description('this is basicStorage')
//@[0:225) ResourceDeclarationSyntax
//@[0:40)  DecoratorSyntax
//@[0:1)   At |@|
//@[1:40)   InstanceFunctionCallSyntax
//@[1:4)    VariableAccessSyntax
//@[1:4)     IdentifierSyntax
//@[1:4)      Identifier |sys|
//@[4:5)    Dot |.|
//@[5:16)    IdentifierSyntax
//@[5:16)     Identifier |description|
//@[16:17)    LeftParen |(|
//@[17:39)    FunctionArgumentSyntax
//@[17:39)     StringSyntax
//@[17:39)      StringComplete |'this is basicStorage'|
//@[39:40)    RightParen |)|
//@[40:42)  NewLine |\r\n|
resource basicStorage 'Microsoft.Storage/storageAccounts@2019-06-01' = {
//@[0:8)  Identifier |resource|
//@[9:21)  IdentifierSyntax
//@[9:21)   Identifier |basicStorage|
//@[22:68)  StringSyntax
//@[22:68)   StringComplete |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[69:70)  Assignment |=|
//@[71:183)  ObjectSyntax
//@[71:72)   LeftBrace |{|
//@[72:74)   NewLine |\r\n|
  name: 'basicblobs'
//@[2:20)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:20)    StringSyntax
//@[8:20)     StringComplete |'basicblobs'|
//@[20:22)   NewLine |\r\n|
  location: 'westus'
//@[2:20)   ObjectPropertySyntax
//@[2:10)    IdentifierSyntax
//@[2:10)     Identifier |location|
//@[10:11)    Colon |:|
//@[12:20)    StringSyntax
//@[12:20)     StringComplete |'westus'|
//@[20:22)   NewLine |\r\n|
  kind: 'BlobStorage'
//@[2:21)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |kind|
//@[6:7)    Colon |:|
//@[8:21)    StringSyntax
//@[8:21)     StringComplete |'BlobStorage'|
//@[21:23)   NewLine |\r\n|
  sku: {
//@[2:39)   ObjectPropertySyntax
//@[2:5)    IdentifierSyntax
//@[2:5)     Identifier |sku|
//@[5:6)    Colon |:|
//@[7:39)    ObjectSyntax
//@[7:8)     LeftBrace |{|
//@[8:10)     NewLine |\r\n|
    name: 'Standard_GRS'
//@[4:24)     ObjectPropertySyntax
//@[4:8)      IdentifierSyntax
//@[4:8)       Identifier |name|
//@[8:9)      Colon |:|
//@[10:24)      StringSyntax
//@[10:24)       StringComplete |'Standard_GRS'|
//@[24:26)     NewLine |\r\n|
  }
//@[2:3)     RightBrace |}|
//@[3:5)   NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

@sys.description('this is dnsZone')
//@[0:140) ResourceDeclarationSyntax
//@[0:35)  DecoratorSyntax
//@[0:1)   At |@|
//@[1:35)   InstanceFunctionCallSyntax
//@[1:4)    VariableAccessSyntax
//@[1:4)     IdentifierSyntax
//@[1:4)      Identifier |sys|
//@[4:5)    Dot |.|
//@[5:16)    IdentifierSyntax
//@[5:16)     Identifier |description|
//@[16:17)    LeftParen |(|
//@[17:34)    FunctionArgumentSyntax
//@[17:34)     StringSyntax
//@[17:34)      StringComplete |'this is dnsZone'|
//@[34:35)    RightParen |)|
//@[35:37)  NewLine |\r\n|
resource dnsZone 'Microsoft.Network/dnszones@2018-05-01' = {
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
//@[2:16)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:16)    StringSyntax
//@[8:16)     StringComplete |'myZone'|
//@[16:18)   NewLine |\r\n|
  location: 'global'
//@[2:20)   ObjectPropertySyntax
//@[2:10)    IdentifierSyntax
//@[2:10)     Identifier |location|
//@[10:11)    Colon |:|
//@[12:20)    StringSyntax
//@[12:20)     StringComplete |'global'|
//@[20:22)   NewLine |\r\n|
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
//@[2:24)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:24)    StringSyntax
//@[8:24)     StringComplete |'myencryptedone'|
//@[24:26)   NewLine |\r\n|
  location: 'eastus2'
//@[2:21)   ObjectPropertySyntax
//@[2:10)    IdentifierSyntax
//@[2:10)     Identifier |location|
//@[10:11)    Colon |:|
//@[12:21)    StringSyntax
//@[12:21)     StringComplete |'eastus2'|
//@[21:23)   NewLine |\r\n|
  properties: {
//@[2:277)   ObjectPropertySyntax
//@[2:12)    IdentifierSyntax
//@[2:12)     Identifier |properties|
//@[12:13)    Colon |:|
//@[14:277)    ObjectSyntax
//@[14:15)     LeftBrace |{|
//@[15:17)     NewLine |\r\n|
    supportsHttpsTrafficOnly: true
//@[4:34)     ObjectPropertySyntax
//@[4:28)      IdentifierSyntax
//@[4:28)       Identifier |supportsHttpsTrafficOnly|
//@[28:29)      Colon |:|
//@[30:34)      BooleanLiteralSyntax
//@[30:34)       TrueKeyword |true|
//@[34:36)     NewLine |\r\n|
    accessTier: 'Hot'
//@[4:21)     ObjectPropertySyntax
//@[4:14)      IdentifierSyntax
//@[4:14)       Identifier |accessTier|
//@[14:15)      Colon |:|
//@[16:21)      StringSyntax
//@[16:21)       StringComplete |'Hot'|
//@[21:23)     NewLine |\r\n|
    encryption: {
//@[4:196)     ObjectPropertySyntax
//@[4:14)      IdentifierSyntax
//@[4:14)       Identifier |encryption|
//@[14:15)      Colon |:|
//@[16:196)      ObjectSyntax
//@[16:17)       LeftBrace |{|
//@[17:19)       NewLine |\r\n|
      keySource: 'Microsoft.Storage'
//@[6:36)       ObjectPropertySyntax
//@[6:15)        IdentifierSyntax
//@[6:15)         Identifier |keySource|
//@[15:16)        Colon |:|
//@[17:36)        StringSyntax
//@[17:36)         StringComplete |'Microsoft.Storage'|
//@[36:38)       NewLine |\r\n|
      services: {
//@[6:132)       ObjectPropertySyntax
//@[6:14)        IdentifierSyntax
//@[6:14)         Identifier |services|
//@[14:15)        Colon |:|
//@[16:132)        ObjectSyntax
//@[16:17)         LeftBrace |{|
//@[17:19)         NewLine |\r\n|
        blob: {
//@[8:51)         ObjectPropertySyntax
//@[8:12)          IdentifierSyntax
//@[8:12)           Identifier |blob|
//@[12:13)          Colon |:|
//@[14:51)          ObjectSyntax
//@[14:15)           LeftBrace |{|
//@[15:17)           NewLine |\r\n|
          enabled: true
//@[10:23)           ObjectPropertySyntax
//@[10:17)            IdentifierSyntax
//@[10:17)             Identifier |enabled|
//@[17:18)            Colon |:|
//@[19:23)            BooleanLiteralSyntax
//@[19:23)             TrueKeyword |true|
//@[23:25)           NewLine |\r\n|
        }
//@[8:9)           RightBrace |}|
//@[9:11)         NewLine |\r\n|
        file: {
//@[8:51)         ObjectPropertySyntax
//@[8:12)          IdentifierSyntax
//@[8:12)           Identifier |file|
//@[12:13)          Colon |:|
//@[14:51)          ObjectSyntax
//@[14:15)           LeftBrace |{|
//@[15:17)           NewLine |\r\n|
          enabled: true
//@[10:23)           ObjectPropertySyntax
//@[10:17)            IdentifierSyntax
//@[10:17)             Identifier |enabled|
//@[17:18)            Colon |:|
//@[19:23)            BooleanLiteralSyntax
//@[19:23)             TrueKeyword |true|
//@[23:25)           NewLine |\r\n|
        }
//@[8:9)           RightBrace |}|
//@[9:11)         NewLine |\r\n|
      }
//@[6:7)         RightBrace |}|
//@[7:9)       NewLine |\r\n|
    }
//@[4:5)       RightBrace |}|
//@[5:7)     NewLine |\r\n|
  }
//@[2:3)     RightBrace |}|
//@[3:5)   NewLine |\r\n|
  kind: 'StorageV2'
//@[2:19)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |kind|
//@[6:7)    Colon |:|
//@[8:19)    StringSyntax
//@[8:19)     StringComplete |'StorageV2'|
//@[19:21)   NewLine |\r\n|
  sku: {
//@[2:39)   ObjectPropertySyntax
//@[2:5)    IdentifierSyntax
//@[2:5)     Identifier |sku|
//@[5:6)    Colon |:|
//@[7:39)    ObjectSyntax
//@[7:8)     LeftBrace |{|
//@[8:10)     NewLine |\r\n|
    name: 'Standard_LRS'
//@[4:24)     ObjectPropertySyntax
//@[4:8)      IdentifierSyntax
//@[4:8)       Identifier |name|
//@[8:9)      Colon |:|
//@[10:24)      StringSyntax
//@[10:24)       StringComplete |'Standard_LRS'|
//@[24:26)     NewLine |\r\n|
  }
//@[2:3)     RightBrace |}|
//@[3:5)   NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource withExpressions 'Microsoft.Storage/storageAccounts@2017-10-01' = {
//@[0:539) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:24)  IdentifierSyntax
//@[9:24)   Identifier |withExpressions|
//@[25:71)  StringSyntax
//@[25:71)   StringComplete |'Microsoft.Storage/storageAccounts@2017-10-01'|
//@[72:73)  Assignment |=|
//@[74:539)  ObjectSyntax
//@[74:75)   LeftBrace |{|
//@[75:77)   NewLine |\r\n|
  name: 'myencryptedone2'
//@[2:25)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:25)    StringSyntax
//@[8:25)     StringComplete |'myencryptedone2'|
//@[25:27)   NewLine |\r\n|
  location: 'eastus2'
//@[2:21)   ObjectPropertySyntax
//@[2:10)    IdentifierSyntax
//@[2:10)     Identifier |location|
//@[10:11)    Colon |:|
//@[12:21)    StringSyntax
//@[12:21)     StringComplete |'eastus2'|
//@[21:23)   NewLine |\r\n|
  properties: {
//@[2:304)   ObjectPropertySyntax
//@[2:12)    IdentifierSyntax
//@[2:12)     Identifier |properties|
//@[12:13)    Colon |:|
//@[14:304)    ObjectSyntax
//@[14:15)     LeftBrace |{|
//@[15:17)     NewLine |\r\n|
    supportsHttpsTrafficOnly: !false
//@[4:36)     ObjectPropertySyntax
//@[4:28)      IdentifierSyntax
//@[4:28)       Identifier |supportsHttpsTrafficOnly|
//@[28:29)      Colon |:|
//@[30:36)      UnaryOperationSyntax
//@[30:31)       Exclamation |!|
//@[31:36)       BooleanLiteralSyntax
//@[31:36)        FalseKeyword |false|
//@[36:38)     NewLine |\r\n|
    accessTier: true ? 'Hot' : 'Cold'
//@[4:37)     ObjectPropertySyntax
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
//@[37:39)     NewLine |\r\n|
    encryption: {
//@[4:205)     ObjectPropertySyntax
//@[4:14)      IdentifierSyntax
//@[4:14)       Identifier |encryption|
//@[14:15)      Colon |:|
//@[16:205)      ObjectSyntax
//@[16:17)       LeftBrace |{|
//@[17:19)       NewLine |\r\n|
      keySource: 'Microsoft.Storage'
//@[6:36)       ObjectPropertySyntax
//@[6:15)        IdentifierSyntax
//@[6:15)         Identifier |keySource|
//@[15:16)        Colon |:|
//@[17:36)        StringSyntax
//@[17:36)         StringComplete |'Microsoft.Storage'|
//@[36:38)       NewLine |\r\n|
      services: {
//@[6:141)       ObjectPropertySyntax
//@[6:14)        IdentifierSyntax
//@[6:14)         Identifier |services|
//@[14:15)        Colon |:|
//@[16:141)        ObjectSyntax
//@[16:17)         LeftBrace |{|
//@[17:19)         NewLine |\r\n|
        blob: {
//@[8:60)         ObjectPropertySyntax
//@[8:12)          IdentifierSyntax
//@[8:12)           Identifier |blob|
//@[12:13)          Colon |:|
//@[14:60)          ObjectSyntax
//@[14:15)           LeftBrace |{|
//@[15:17)           NewLine |\r\n|
          enabled: true || false
//@[10:32)           ObjectPropertySyntax
//@[10:17)            IdentifierSyntax
//@[10:17)             Identifier |enabled|
//@[17:18)            Colon |:|
//@[19:32)            BinaryOperationSyntax
//@[19:23)             BooleanLiteralSyntax
//@[19:23)              TrueKeyword |true|
//@[24:26)             LogicalOr ||||
//@[27:32)             BooleanLiteralSyntax
//@[27:32)              FalseKeyword |false|
//@[32:34)           NewLine |\r\n|
        }
//@[8:9)           RightBrace |}|
//@[9:11)         NewLine |\r\n|
        file: {
//@[8:51)         ObjectPropertySyntax
//@[8:12)          IdentifierSyntax
//@[8:12)           Identifier |file|
//@[12:13)          Colon |:|
//@[14:51)          ObjectSyntax
//@[14:15)           LeftBrace |{|
//@[15:17)           NewLine |\r\n|
          enabled: true
//@[10:23)           ObjectPropertySyntax
//@[10:17)            IdentifierSyntax
//@[10:17)             Identifier |enabled|
//@[17:18)            Colon |:|
//@[19:23)            BooleanLiteralSyntax
//@[19:23)             TrueKeyword |true|
//@[23:25)           NewLine |\r\n|
        }
//@[8:9)           RightBrace |}|
//@[9:11)         NewLine |\r\n|
      }
//@[6:7)         RightBrace |}|
//@[7:9)       NewLine |\r\n|
    }
//@[4:5)       RightBrace |}|
//@[5:7)     NewLine |\r\n|
  }
//@[2:3)     RightBrace |}|
//@[3:5)   NewLine |\r\n|
  kind: 'StorageV2'
//@[2:19)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |kind|
//@[6:7)    Colon |:|
//@[8:19)    StringSyntax
//@[8:19)     StringComplete |'StorageV2'|
//@[19:21)   NewLine |\r\n|
  sku: {
//@[2:39)   ObjectPropertySyntax
//@[2:5)    IdentifierSyntax
//@[2:5)     Identifier |sku|
//@[5:6)    Colon |:|
//@[7:39)    ObjectSyntax
//@[7:8)     LeftBrace |{|
//@[8:10)     NewLine |\r\n|
    name: 'Standard_LRS'
//@[4:24)     ObjectPropertySyntax
//@[4:8)      IdentifierSyntax
//@[4:8)       Identifier |name|
//@[8:9)      Colon |:|
//@[10:24)      StringSyntax
//@[10:24)       StringComplete |'Standard_LRS'|
//@[24:26)     NewLine |\r\n|
  }
//@[2:3)     RightBrace |}|
//@[3:5)   NewLine |\r\n|
  dependsOn: [
//@[2:41)   ObjectPropertySyntax
//@[2:11)    IdentifierSyntax
//@[2:11)     Identifier |dependsOn|
//@[11:12)    Colon |:|
//@[13:41)    ArraySyntax
//@[13:14)     LeftSquare |[|
//@[14:16)     NewLine |\r\n|
    myStorageAccount
//@[4:20)     ArrayItemSyntax
//@[4:20)      VariableAccessSyntax
//@[4:20)       IdentifierSyntax
//@[4:20)        Identifier |myStorageAccount|
//@[20:22)     NewLine |\r\n|
  ]
//@[2:3)     RightSquare |]|
//@[3:5)   NewLine |\r\n|
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
//@[2:23)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:23)    VariableAccessSyntax
//@[8:23)     IdentifierSyntax
//@[8:23)      Identifier |hostingPlanName|
//@[23:25)   NewLine |\r\n|
  location: location
//@[2:20)   ObjectPropertySyntax
//@[2:10)    IdentifierSyntax
//@[2:10)     Identifier |location|
//@[10:11)    Colon |:|
//@[12:20)    VariableAccessSyntax
//@[12:20)     IdentifierSyntax
//@[12:20)      Identifier |location|
//@[20:22)   NewLine |\r\n|
  sku: {
//@[2:82)   ObjectPropertySyntax
//@[2:5)    IdentifierSyntax
//@[2:5)     Identifier |sku|
//@[5:6)    Colon |:|
//@[7:82)    ObjectSyntax
//@[7:8)     LeftBrace |{|
//@[8:10)     NewLine |\r\n|
    name: appServicePlanTier
//@[4:28)     ObjectPropertySyntax
//@[4:8)      IdentifierSyntax
//@[4:8)       Identifier |name|
//@[8:9)      Colon |:|
//@[10:28)      VariableAccessSyntax
//@[10:28)       IdentifierSyntax
//@[10:28)        Identifier |appServicePlanTier|
//@[28:30)     NewLine |\r\n|
    capacity: appServicePlanInstances
//@[4:37)     ObjectPropertySyntax
//@[4:12)      IdentifierSyntax
//@[4:12)       Identifier |capacity|
//@[12:13)      Colon |:|
//@[14:37)      VariableAccessSyntax
//@[14:37)       IdentifierSyntax
//@[14:37)        Identifier |appServicePlanInstances|
//@[37:39)     NewLine |\r\n|
  }
//@[2:3)     RightBrace |}|
//@[3:5)   NewLine |\r\n|
  properties: {
//@[2:91)   ObjectPropertySyntax
//@[2:12)    IdentifierSyntax
//@[2:12)     Identifier |properties|
//@[12:13)    Colon |:|
//@[14:91)    ObjectSyntax
//@[14:15)     LeftBrace |{|
//@[15:17)     NewLine |\r\n|
    name: hostingPlanName // just hostingPlanName results in an error
//@[4:25)     ObjectPropertySyntax
//@[4:8)      IdentifierSyntax
//@[4:8)       Identifier |name|
//@[8:9)      Colon |:|
//@[10:25)      VariableAccessSyntax
//@[10:25)       IdentifierSyntax
//@[10:25)        Identifier |hostingPlanName|
//@[69:71)     NewLine |\r\n|
  }
//@[2:3)     RightBrace |}|
//@[3:5)   NewLine |\r\n|
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
//@[2:19)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:19)    VariableAccessSyntax
//@[8:19)     IdentifierSyntax
//@[8:19)      Identifier |webSiteName|
//@[19:21)   NewLine |\r\n|
  location: location
//@[2:20)   ObjectPropertySyntax
//@[2:10)    IdentifierSyntax
//@[2:10)     Identifier |location|
//@[10:11)    Colon |:|
//@[12:20)    VariableAccessSyntax
//@[12:20)     IdentifierSyntax
//@[12:20)      Identifier |location|
//@[20:22)   NewLine |\r\n|
  properties: {
//@[2:591)   ObjectPropertySyntax
//@[2:12)    IdentifierSyntax
//@[2:12)     Identifier |properties|
//@[12:13)    Colon |:|
//@[14:591)    ObjectSyntax
//@[14:15)     LeftBrace |{|
//@[15:17)     NewLine |\r\n|
    // not yet supported // serverFarmId: farm.id
//@[49:51)     NewLine |\r\n|
    siteConfig: {
//@[4:518)     ObjectPropertySyntax
//@[4:14)      IdentifierSyntax
//@[4:14)       Identifier |siteConfig|
//@[14:15)      Colon |:|
//@[16:518)      ObjectSyntax
//@[16:17)       LeftBrace |{|
//@[17:19)       NewLine |\r\n|
      appSettings: [
//@[6:492)       ObjectPropertySyntax
//@[6:17)        IdentifierSyntax
//@[6:17)         Identifier |appSettings|
//@[17:18)        Colon |:|
//@[19:492)        ArraySyntax
//@[19:20)         LeftSquare |[|
//@[20:22)         NewLine |\r\n|
        {
//@[8:121)         ArrayItemSyntax
//@[8:121)          ObjectSyntax
//@[8:9)           LeftBrace |{|
//@[9:11)           NewLine |\r\n|
          name: 'CosmosDb:Account'
//@[10:34)           ObjectPropertySyntax
//@[10:14)            IdentifierSyntax
//@[10:14)             Identifier |name|
//@[14:15)            Colon |:|
//@[16:34)            StringSyntax
//@[16:34)             StringComplete |'CosmosDb:Account'|
//@[34:36)           NewLine |\r\n|
          value: reference(cosmosDbResourceId).documentEndpoint
//@[10:63)           ObjectPropertySyntax
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
//@[63:65)           NewLine |\r\n|
        }
//@[8:9)           RightBrace |}|
//@[9:11)         NewLine |\r\n|
        {
//@[8:130)         ArrayItemSyntax
//@[8:130)          ObjectSyntax
//@[8:9)           LeftBrace |{|
//@[9:11)           NewLine |\r\n|
          name: 'CosmosDb:Key'
//@[10:30)           ObjectPropertySyntax
//@[10:14)            IdentifierSyntax
//@[10:14)             Identifier |name|
//@[14:15)            Colon |:|
//@[16:30)            StringSyntax
//@[16:30)             StringComplete |'CosmosDb:Key'|
//@[30:32)           NewLine |\r\n|
          value: listKeys(cosmosDbResourceId, '2020-04-01').primaryMasterKey
//@[10:76)           ObjectPropertySyntax
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
//@[76:78)           NewLine |\r\n|
        }
//@[8:9)           RightBrace |}|
//@[9:11)         NewLine |\r\n|
        {
//@[8:101)         ArrayItemSyntax
//@[8:101)          ObjectSyntax
//@[8:9)           LeftBrace |{|
//@[9:11)           NewLine |\r\n|
          name: 'CosmosDb:DatabaseName'
//@[10:39)           ObjectPropertySyntax
//@[10:14)            IdentifierSyntax
//@[10:14)             Identifier |name|
//@[14:15)            Colon |:|
//@[16:39)            StringSyntax
//@[16:39)             StringComplete |'CosmosDb:DatabaseName'|
//@[39:41)           NewLine |\r\n|
          value: cosmosDb.databaseName
//@[10:38)           ObjectPropertySyntax
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
//@[38:40)           NewLine |\r\n|
        }
//@[8:9)           RightBrace |}|
//@[9:11)         NewLine |\r\n|
        {
//@[8:103)         ArrayItemSyntax
//@[8:103)          ObjectSyntax
//@[8:9)           LeftBrace |{|
//@[9:11)           NewLine |\r\n|
          name: 'CosmosDb:ContainerName'
//@[10:40)           ObjectPropertySyntax
//@[10:14)            IdentifierSyntax
//@[10:14)             Identifier |name|
//@[14:15)            Colon |:|
//@[16:40)            StringSyntax
//@[16:40)             StringComplete |'CosmosDb:ContainerName'|
//@[40:42)           NewLine |\r\n|
          value: cosmosDb.containerName
//@[10:39)           ObjectPropertySyntax
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
//@[39:41)           NewLine |\r\n|
        }
//@[8:9)           RightBrace |}|
//@[9:11)         NewLine |\r\n|
      ]
//@[6:7)         RightSquare |]|
//@[7:9)       NewLine |\r\n|
    }
//@[4:5)       RightBrace |}|
//@[5:7)     NewLine |\r\n|
  }
//@[2:3)     RightBrace |}|
//@[3:5)   NewLine |\r\n|
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
//@[2:25)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:25)    StringSyntax
//@[8:25)     StringComplete |'nestedTemplate1'|
//@[25:27)   NewLine |\r\n|
  properties: {
//@[2:258)   ObjectPropertySyntax
//@[2:12)    IdentifierSyntax
//@[2:12)     Identifier |properties|
//@[12:13)    Colon |:|
//@[14:258)    ObjectSyntax
//@[14:15)     LeftBrace |{|
//@[15:17)     NewLine |\r\n|
    mode: 'Incremental'
//@[4:23)     ObjectPropertySyntax
//@[4:8)      IdentifierSyntax
//@[4:8)       Identifier |mode|
//@[8:9)      Colon |:|
//@[10:23)      StringSyntax
//@[10:23)       StringComplete |'Incremental'|
//@[23:25)     NewLine |\r\n|
    template: {
//@[4:211)     ObjectPropertySyntax
//@[4:12)      IdentifierSyntax
//@[4:12)       Identifier |template|
//@[12:13)      Colon |:|
//@[14:211)      ObjectSyntax
//@[14:15)       LeftBrace |{|
//@[15:17)       NewLine |\r\n|
      // string key value
//@[25:27)       NewLine |\r\n|
      '$schema': 'https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#'
//@[6:98)       ObjectPropertySyntax
//@[6:15)        StringSyntax
//@[6:15)         StringComplete |'$schema'|
//@[15:16)        Colon |:|
//@[17:98)        StringSyntax
//@[17:98)         StringComplete |'https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#'|
//@[98:100)       NewLine |\r\n|
      contentVersion: '1.0.0.0'
//@[6:31)       ObjectPropertySyntax
//@[6:20)        IdentifierSyntax
//@[6:20)         Identifier |contentVersion|
//@[20:21)        Colon |:|
//@[22:31)        StringSyntax
//@[22:31)         StringComplete |'1.0.0.0'|
//@[31:33)       NewLine |\r\n|
      resources: [
//@[6:27)       ObjectPropertySyntax
//@[6:15)        IdentifierSyntax
//@[6:15)         Identifier |resources|
//@[15:16)        Colon |:|
//@[17:27)        ArraySyntax
//@[17:18)         LeftSquare |[|
//@[18:20)         NewLine |\r\n|
      ]
//@[6:7)         RightSquare |]|
//@[7:9)       NewLine |\r\n|
    }
//@[4:5)       RightBrace |}|
//@[5:7)     NewLine |\r\n|
  }
//@[2:3)     RightBrace |}|
//@[3:5)   NewLine |\r\n|
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
//@[2:25)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:25)    StringSyntax
//@[8:25)     StringComplete |'nestedTemplate1'|
//@[25:27)   NewLine |\r\n|
  properties: {
//@[2:180)   ObjectPropertySyntax
//@[2:12)    IdentifierSyntax
//@[2:12)     Identifier |properties|
//@[12:13)    Colon |:|
//@[14:180)    ObjectSyntax
//@[14:15)     LeftBrace |{|
//@[15:17)     NewLine |\r\n|
    otherId: nested.id
//@[4:22)     ObjectPropertySyntax
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
//@[22:24)     NewLine |\r\n|
    otherName: nested.name
//@[4:26)     ObjectPropertySyntax
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
//@[26:28)     NewLine |\r\n|
    otherVersion: nested.apiVersion
//@[4:35)     ObjectPropertySyntax
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
//@[35:37)     NewLine |\r\n|
    otherType: nested.type
//@[4:26)     ObjectPropertySyntax
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
//@[26:30)     NewLine |\r\n\r\n|

    otherThings: nested.properties.mode
//@[4:39)     ObjectPropertySyntax
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
//@[39:41)     NewLine |\r\n|
  }
//@[2:3)     RightBrace |}|
//@[3:5)   NewLine |\r\n|
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
//@[2:19)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:19)    StringSyntax
//@[8:19)     StringComplete |'resourceA'|
//@[19:21)   NewLine |\r\n|
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
//@[2:34)   ObjectPropertySyntax
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
//@[34:36)   NewLine |\r\n|
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
//@[2:34)   ObjectPropertySyntax
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
//@[34:36)   NewLine |\r\n|
  properties: {
//@[2:175)   ObjectPropertySyntax
//@[2:12)    IdentifierSyntax
//@[2:12)     Identifier |properties|
//@[12:13)    Colon |:|
//@[14:175)    ObjectSyntax
//@[14:15)     LeftBrace |{|
//@[15:17)     NewLine |\r\n|
    aId: resourceA.id
//@[4:21)     ObjectPropertySyntax
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
//@[21:23)     NewLine |\r\n|
    aType: resourceA.type
//@[4:25)     ObjectPropertySyntax
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
//@[25:27)     NewLine |\r\n|
    aName: resourceA.name
//@[4:25)     ObjectPropertySyntax
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
//@[25:27)     NewLine |\r\n|
    aApiVersion: resourceA.apiVersion
//@[4:37)     ObjectPropertySyntax
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
//@[37:39)     NewLine |\r\n|
    bProperties: resourceB.properties
//@[4:37)     ObjectPropertySyntax
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
//@[37:39)     NewLine |\r\n|
  }
//@[2:3)     RightBrace |}|
//@[3:5)   NewLine |\r\n|
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
//@[2:19)   ObjectPropertySyntax
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
//@[19:21)   NewLine |\r\n|
  bType: resourceB.type
//@[2:23)   ObjectPropertySyntax
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
//@[23:25)   NewLine |\r\n|
  bName: resourceB.name
//@[2:23)   ObjectPropertySyntax
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
//@[23:25)   NewLine |\r\n|
  bApiVersion: resourceB.apiVersion
//@[2:35)   ObjectPropertySyntax
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
//@[35:37)   NewLine |\r\n|
  aKind: resourceA.kind
//@[2:23)   ObjectPropertySyntax
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
//@[23:25)   NewLine |\r\n|
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
//@[2:13)   ArrayItemSyntax
//@[2:13)    VariableAccessSyntax
//@[2:13)     IdentifierSyntax
//@[2:13)      Identifier |varARuntime|
//@[13:15)   NewLine |\r\n|
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
//@[2:18)   ObjectPropertySyntax
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
//@[18:20)   NewLine |\r\n|
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
//@[2:18)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:18)    StringSyntax
//@[8:18)     StringComplete |'constant'|
//@[18:20)   NewLine |\r\n|
  properties: {
//@[2:159)   ObjectPropertySyntax
//@[2:12)    IdentifierSyntax
//@[2:12)     Identifier |properties|
//@[12:13)    Colon |:|
//@[14:159)    ObjectSyntax
//@[14:15)     LeftBrace |{|
//@[15:17)     NewLine |\r\n|
    runtime: varBRuntime
//@[4:24)     ObjectPropertySyntax
//@[4:11)      IdentifierSyntax
//@[4:11)       Identifier |runtime|
//@[11:12)      Colon |:|
//@[13:24)      VariableAccessSyntax
//@[13:24)       IdentifierSyntax
//@[13:24)        Identifier |varBRuntime|
//@[24:26)     NewLine |\r\n|
    // repro for https://github.com/Azure/bicep/issues/316
//@[58:60)     NewLine |\r\n|
    repro316: setResourceCRef ? resourceCRef : null
//@[4:51)     ObjectPropertySyntax
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
//@[51:53)     NewLine |\r\n|
  }
//@[2:3)     RightBrace |}|
//@[3:5)   NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

var myInterpKey = 'abc'
//@[0:23) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:15)  IdentifierSyntax
//@[4:15)   Identifier |myInterpKey|
//@[16:17)  Assignment |=|
//@[18:23)  StringSyntax
//@[18:23)   StringComplete |'abc'|
//@[23:25) NewLine |\r\n|
resource resourceWithInterp 'My.Rp/interp@2020-01-01' = {
//@[0:202) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:27)  IdentifierSyntax
//@[9:27)   Identifier |resourceWithInterp|
//@[28:53)  StringSyntax
//@[28:53)   StringComplete |'My.Rp/interp@2020-01-01'|
//@[54:55)  Assignment |=|
//@[56:202)  ObjectSyntax
//@[56:57)   LeftBrace |{|
//@[57:59)   NewLine |\r\n|
  name: 'interpTest'
//@[2:20)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:20)    StringSyntax
//@[8:20)     StringComplete |'interpTest'|
//@[20:22)   NewLine |\r\n|
  properties: {
//@[2:118)   ObjectPropertySyntax
//@[2:12)    IdentifierSyntax
//@[2:12)     Identifier |properties|
//@[12:13)    Colon |:|
//@[14:118)    ObjectSyntax
//@[14:15)     LeftBrace |{|
//@[15:17)     NewLine |\r\n|
    '${myInterpKey}': 1
//@[4:23)     ObjectPropertySyntax
//@[4:20)      StringSyntax
//@[4:7)       StringLeftPiece |'${|
//@[7:18)       VariableAccessSyntax
//@[7:18)        IdentifierSyntax
//@[7:18)         Identifier |myInterpKey|
//@[18:20)       StringRightPiece |}'|
//@[20:21)      Colon |:|
//@[22:23)      IntegerLiteralSyntax
//@[22:23)       Integer |1|
//@[23:25)     NewLine |\r\n|
    'abc${myInterpKey}def': 2
//@[4:29)     ObjectPropertySyntax
//@[4:26)      StringSyntax
//@[4:10)       StringLeftPiece |'abc${|
//@[10:21)       VariableAccessSyntax
//@[10:21)        IdentifierSyntax
//@[10:21)         Identifier |myInterpKey|
//@[21:26)       StringRightPiece |}def'|
//@[26:27)      Colon |:|
//@[28:29)      IntegerLiteralSyntax
//@[28:29)       Integer |2|
//@[29:31)     NewLine |\r\n|
    '${myInterpKey}abc${myInterpKey}': 3
//@[4:40)     ObjectPropertySyntax
//@[4:37)      StringSyntax
//@[4:7)       StringLeftPiece |'${|
//@[7:18)       VariableAccessSyntax
//@[7:18)        IdentifierSyntax
//@[7:18)         Identifier |myInterpKey|
//@[18:24)       StringMiddlePiece |}abc${|
//@[24:35)       VariableAccessSyntax
//@[24:35)        IdentifierSyntax
//@[24:35)         Identifier |myInterpKey|
//@[35:37)       StringRightPiece |}'|
//@[37:38)      Colon |:|
//@[39:40)      IntegerLiteralSyntax
//@[39:40)       Integer |3|
//@[40:42)     NewLine |\r\n|
  }
//@[2:3)     RightBrace |}|
//@[3:5)   NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource resourceWithEscaping 'My.Rp/mockResource@2020-01-01' = {
//@[0:234) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:29)  IdentifierSyntax
//@[9:29)   Identifier |resourceWithEscaping|
//@[30:61)  StringSyntax
//@[30:61)   StringComplete |'My.Rp/mockResource@2020-01-01'|
//@[62:63)  Assignment |=|
//@[64:234)  ObjectSyntax
//@[64:65)   LeftBrace |{|
//@[65:67)   NewLine |\r\n|
  name: 'test'
//@[2:14)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:14)    StringSyntax
//@[8:14)     StringComplete |'test'|
//@[14:16)   NewLine |\r\n|
  properties: {
//@[2:148)   ObjectPropertySyntax
//@[2:12)    IdentifierSyntax
//@[2:12)     Identifier |properties|
//@[12:13)    Colon |:|
//@[14:148)    ObjectSyntax
//@[14:15)     LeftBrace |{|
//@[15:17)     NewLine |\r\n|
    // both key and value should be escaped in template output
//@[62:64)     NewLine |\r\n|
    '[resourceGroup().location]': '[resourceGroup().location]'
//@[4:62)     ObjectPropertySyntax
//@[4:32)      StringSyntax
//@[4:32)       StringComplete |'[resourceGroup().location]'|
//@[32:33)      Colon |:|
//@[34:62)      StringSyntax
//@[34:62)       StringComplete |'[resourceGroup().location]'|
//@[62:64)     NewLine |\r\n|
  }
//@[2:3)     RightBrace |}|
//@[3:5)   NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

param shouldDeployVm bool = true
//@[0:32) ParameterDeclarationSyntax
//@[0:5)  Identifier |param|
//@[6:20)  IdentifierSyntax
//@[6:20)   Identifier |shouldDeployVm|
//@[21:25)  TypeSyntax
//@[21:25)   Identifier |bool|
//@[26:32)  ParameterDefaultValueSyntax
//@[26:27)   Assignment |=|
//@[28:32)   BooleanLiteralSyntax
//@[28:32)    TrueKeyword |true|
//@[32:36) NewLine |\r\n\r\n|

@sys.description('this is vmWithCondition')
//@[0:308) ResourceDeclarationSyntax
//@[0:43)  DecoratorSyntax
//@[0:1)   At |@|
//@[1:43)   InstanceFunctionCallSyntax
//@[1:4)    VariableAccessSyntax
//@[1:4)     IdentifierSyntax
//@[1:4)      Identifier |sys|
//@[4:5)    Dot |.|
//@[5:16)    IdentifierSyntax
//@[5:16)     Identifier |description|
//@[16:17)    LeftParen |(|
//@[17:42)    FunctionArgumentSyntax
//@[17:42)     StringSyntax
//@[17:42)      StringComplete |'this is vmWithCondition'|
//@[42:43)    RightParen |)|
//@[43:45)  NewLine |\r\n|
resource vmWithCondition 'Microsoft.Compute/virtualMachines@2020-06-01' = if (shouldDeployVm) {
//@[0:8)  Identifier |resource|
//@[9:24)  IdentifierSyntax
//@[9:24)   Identifier |vmWithCondition|
//@[25:71)  StringSyntax
//@[25:71)   StringComplete |'Microsoft.Compute/virtualMachines@2020-06-01'|
//@[72:73)  Assignment |=|
//@[74:263)  IfConditionSyntax
//@[74:76)   Identifier |if|
//@[77:93)   ParenthesizedExpressionSyntax
//@[77:78)    LeftParen |(|
//@[78:92)    VariableAccessSyntax
//@[78:92)     IdentifierSyntax
//@[78:92)      Identifier |shouldDeployVm|
//@[92:93)    RightParen |)|
//@[94:263)   ObjectSyntax
//@[94:95)    LeftBrace |{|
//@[95:97)    NewLine |\r\n|
  name: 'vmName'
//@[2:16)    ObjectPropertySyntax
//@[2:6)     IdentifierSyntax
//@[2:6)      Identifier |name|
//@[6:7)     Colon |:|
//@[8:16)     StringSyntax
//@[8:16)      StringComplete |'vmName'|
//@[16:18)    NewLine |\r\n|
  location: 'westus'
//@[2:20)    ObjectPropertySyntax
//@[2:10)     IdentifierSyntax
//@[2:10)      Identifier |location|
//@[10:11)     Colon |:|
//@[12:20)     StringSyntax
//@[12:20)      StringComplete |'westus'|
//@[20:22)    NewLine |\r\n|
  properties: {
//@[2:123)    ObjectPropertySyntax
//@[2:12)     IdentifierSyntax
//@[2:12)      Identifier |properties|
//@[12:13)     Colon |:|
//@[14:123)     ObjectSyntax
//@[14:15)      LeftBrace |{|
//@[15:17)      NewLine |\r\n|
    osProfile: {
//@[4:101)      ObjectPropertySyntax
//@[4:13)       IdentifierSyntax
//@[4:13)        Identifier |osProfile|
//@[13:14)       Colon |:|
//@[15:101)       ObjectSyntax
//@[15:16)        LeftBrace |{|
//@[16:18)        NewLine |\r\n|
      windowsConfiguration: {
//@[6:76)        ObjectPropertySyntax
//@[6:26)         IdentifierSyntax
//@[6:26)          Identifier |windowsConfiguration|
//@[26:27)         Colon |:|
//@[28:76)         ObjectSyntax
//@[28:29)          LeftBrace |{|
//@[29:31)          NewLine |\r\n|
        enableAutomaticUpdates: true
//@[8:36)          ObjectPropertySyntax
//@[8:30)           IdentifierSyntax
//@[8:30)            Identifier |enableAutomaticUpdates|
//@[30:31)           Colon |:|
//@[32:36)           BooleanLiteralSyntax
//@[32:36)            TrueKeyword |true|
//@[36:38)          NewLine |\r\n|
      }
//@[6:7)          RightBrace |}|
//@[7:9)        NewLine |\r\n|
    }
//@[4:5)        RightBrace |}|
//@[5:7)      NewLine |\r\n|
  }
//@[2:3)      RightBrace |}|
//@[3:5)    NewLine |\r\n|
}
//@[0:1)    RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource extension1 'My.Rp/extensionResource@2020-12-01' = {
//@[0:110) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:19)  IdentifierSyntax
//@[9:19)   Identifier |extension1|
//@[20:56)  StringSyntax
//@[20:56)   StringComplete |'My.Rp/extensionResource@2020-12-01'|
//@[57:58)  Assignment |=|
//@[59:110)  ObjectSyntax
//@[59:60)   LeftBrace |{|
//@[60:62)   NewLine |\r\n|
  name: 'extension'
//@[2:19)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:19)    StringSyntax
//@[8:19)     StringComplete |'extension'|
//@[19:21)   NewLine |\r\n|
  scope: vmWithCondition
//@[2:24)   ObjectPropertySyntax
//@[2:7)    IdentifierSyntax
//@[2:7)     Identifier |scope|
//@[7:8)    Colon |:|
//@[9:24)    VariableAccessSyntax
//@[9:24)     IdentifierSyntax
//@[9:24)      Identifier |vmWithCondition|
//@[24:26)   NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource extension2 'My.Rp/extensionResource@2020-12-01' = {
//@[0:105) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:19)  IdentifierSyntax
//@[9:19)   Identifier |extension2|
//@[20:56)  StringSyntax
//@[20:56)   StringComplete |'My.Rp/extensionResource@2020-12-01'|
//@[57:58)  Assignment |=|
//@[59:105)  ObjectSyntax
//@[59:60)   LeftBrace |{|
//@[60:62)   NewLine |\r\n|
  name: 'extension'
//@[2:19)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:19)    StringSyntax
//@[8:19)     StringComplete |'extension'|
//@[19:21)   NewLine |\r\n|
  scope: extension1
//@[2:19)   ObjectPropertySyntax
//@[2:7)    IdentifierSyntax
//@[2:7)     Identifier |scope|
//@[7:8)    Colon |:|
//@[9:19)    VariableAccessSyntax
//@[9:19)     IdentifierSyntax
//@[9:19)      Identifier |extension1|
//@[19:21)   NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource extensionDependencies 'My.Rp/mockResource@2020-01-01' = {
//@[0:359) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:30)  IdentifierSyntax
//@[9:30)   Identifier |extensionDependencies|
//@[31:62)  StringSyntax
//@[31:62)   StringComplete |'My.Rp/mockResource@2020-01-01'|
//@[63:64)  Assignment |=|
//@[65:359)  ObjectSyntax
//@[65:66)   LeftBrace |{|
//@[66:68)   NewLine |\r\n|
  name: 'extensionDependencies'
//@[2:31)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:31)    StringSyntax
//@[8:31)     StringComplete |'extensionDependencies'|
//@[31:33)   NewLine |\r\n|
  properties: {
//@[2:255)   ObjectPropertySyntax
//@[2:12)    IdentifierSyntax
//@[2:12)     Identifier |properties|
//@[12:13)    Colon |:|
//@[14:255)    ObjectSyntax
//@[14:15)     LeftBrace |{|
//@[15:17)     NewLine |\r\n|
    res1: vmWithCondition.id
//@[4:28)     ObjectPropertySyntax
//@[4:8)      IdentifierSyntax
//@[4:8)       Identifier |res1|
//@[8:9)      Colon |:|
//@[10:28)      PropertyAccessSyntax
//@[10:25)       VariableAccessSyntax
//@[10:25)        IdentifierSyntax
//@[10:25)         Identifier |vmWithCondition|
//@[25:26)       Dot |.|
//@[26:28)       IdentifierSyntax
//@[26:28)        Identifier |id|
//@[28:30)     NewLine |\r\n|
    res1runtime: vmWithCondition.properties.something
//@[4:53)     ObjectPropertySyntax
//@[4:15)      IdentifierSyntax
//@[4:15)       Identifier |res1runtime|
//@[15:16)      Colon |:|
//@[17:53)      PropertyAccessSyntax
//@[17:43)       PropertyAccessSyntax
//@[17:32)        VariableAccessSyntax
//@[17:32)         IdentifierSyntax
//@[17:32)          Identifier |vmWithCondition|
//@[32:33)        Dot |.|
//@[33:43)        IdentifierSyntax
//@[33:43)         Identifier |properties|
//@[43:44)       Dot |.|
//@[44:53)       IdentifierSyntax
//@[44:53)        Identifier |something|
//@[53:55)     NewLine |\r\n|
    res2: extension1.id
//@[4:23)     ObjectPropertySyntax
//@[4:8)      IdentifierSyntax
//@[4:8)       Identifier |res2|
//@[8:9)      Colon |:|
//@[10:23)      PropertyAccessSyntax
//@[10:20)       VariableAccessSyntax
//@[10:20)        IdentifierSyntax
//@[10:20)         Identifier |extension1|
//@[20:21)       Dot |.|
//@[21:23)       IdentifierSyntax
//@[21:23)        Identifier |id|
//@[23:25)     NewLine |\r\n|
    res2runtime: extension1.properties.something
//@[4:48)     ObjectPropertySyntax
//@[4:15)      IdentifierSyntax
//@[4:15)       Identifier |res2runtime|
//@[15:16)      Colon |:|
//@[17:48)      PropertyAccessSyntax
//@[17:38)       PropertyAccessSyntax
//@[17:27)        VariableAccessSyntax
//@[17:27)         IdentifierSyntax
//@[17:27)          Identifier |extension1|
//@[27:28)        Dot |.|
//@[28:38)        IdentifierSyntax
//@[28:38)         Identifier |properties|
//@[38:39)       Dot |.|
//@[39:48)       IdentifierSyntax
//@[39:48)        Identifier |something|
//@[48:50)     NewLine |\r\n|
    res3: extension2.id
//@[4:23)     ObjectPropertySyntax
//@[4:8)      IdentifierSyntax
//@[4:8)       Identifier |res3|
//@[8:9)      Colon |:|
//@[10:23)      PropertyAccessSyntax
//@[10:20)       VariableAccessSyntax
//@[10:20)        IdentifierSyntax
//@[10:20)         Identifier |extension2|
//@[20:21)       Dot |.|
//@[21:23)       IdentifierSyntax
//@[21:23)        Identifier |id|
//@[23:25)     NewLine |\r\n|
    res3runtime: extension2.properties.something
//@[4:48)     ObjectPropertySyntax
//@[4:15)      IdentifierSyntax
//@[4:15)       Identifier |res3runtime|
//@[15:16)      Colon |:|
//@[17:48)      PropertyAccessSyntax
//@[17:38)       PropertyAccessSyntax
//@[17:27)        VariableAccessSyntax
//@[17:27)         IdentifierSyntax
//@[17:27)          Identifier |extension2|
//@[27:28)        Dot |.|
//@[28:38)        IdentifierSyntax
//@[28:38)         Identifier |properties|
//@[38:39)       Dot |.|
//@[39:48)       IdentifierSyntax
//@[39:48)        Identifier |something|
//@[48:50)     NewLine |\r\n|
  }
//@[2:3)     RightBrace |}|
//@[3:5)   NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

@sys.description('this is existing1')
//@[0:162) ResourceDeclarationSyntax
//@[0:37)  DecoratorSyntax
//@[0:1)   At |@|
//@[1:37)   InstanceFunctionCallSyntax
//@[1:4)    VariableAccessSyntax
//@[1:4)     IdentifierSyntax
//@[1:4)      Identifier |sys|
//@[4:5)    Dot |.|
//@[5:16)    IdentifierSyntax
//@[5:16)     Identifier |description|
//@[16:17)    LeftParen |(|
//@[17:36)    FunctionArgumentSyntax
//@[17:36)     StringSyntax
//@[17:36)      StringComplete |'this is existing1'|
//@[36:37)    RightParen |)|
//@[37:39)  NewLine |\r\n|
resource existing1 'Mock.Rp/existingExtensionResource@2020-01-01' existing = {
//@[0:8)  Identifier |resource|
//@[9:18)  IdentifierSyntax
//@[9:18)   Identifier |existing1|
//@[19:65)  StringSyntax
//@[19:65)   StringComplete |'Mock.Rp/existingExtensionResource@2020-01-01'|
//@[66:74)  Identifier |existing|
//@[75:76)  Assignment |=|
//@[77:123)  ObjectSyntax
//@[77:78)   LeftBrace |{|
//@[78:80)   NewLine |\r\n|
  name: 'existing1'
//@[2:19)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:19)    StringSyntax
//@[8:19)     StringComplete |'existing1'|
//@[19:21)   NewLine |\r\n|
  scope: extension1
//@[2:19)   ObjectPropertySyntax
//@[2:7)    IdentifierSyntax
//@[2:7)     Identifier |scope|
//@[7:8)    Colon |:|
//@[9:19)    VariableAccessSyntax
//@[9:19)     IdentifierSyntax
//@[9:19)      Identifier |extension1|
//@[19:21)   NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource existing2 'Mock.Rp/existingExtensionResource@2020-01-01' existing = {
//@[0:122) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:18)  IdentifierSyntax
//@[9:18)   Identifier |existing2|
//@[19:65)  StringSyntax
//@[19:65)   StringComplete |'Mock.Rp/existingExtensionResource@2020-01-01'|
//@[66:74)  Identifier |existing|
//@[75:76)  Assignment |=|
//@[77:122)  ObjectSyntax
//@[77:78)   LeftBrace |{|
//@[78:80)   NewLine |\r\n|
  name: 'existing2'
//@[2:19)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:19)    StringSyntax
//@[8:19)     StringComplete |'existing2'|
//@[19:21)   NewLine |\r\n|
  scope: existing1
//@[2:18)   ObjectPropertySyntax
//@[2:7)    IdentifierSyntax
//@[2:7)     Identifier |scope|
//@[7:8)    Colon |:|
//@[9:18)    VariableAccessSyntax
//@[9:18)     IdentifierSyntax
//@[9:18)      Identifier |existing1|
//@[18:20)   NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource extension3 'My.Rp/extensionResource@2020-12-01' = {
//@[0:105) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:19)  IdentifierSyntax
//@[9:19)   Identifier |extension3|
//@[20:56)  StringSyntax
//@[20:56)   StringComplete |'My.Rp/extensionResource@2020-12-01'|
//@[57:58)  Assignment |=|
//@[59:105)  ObjectSyntax
//@[59:60)   LeftBrace |{|
//@[60:62)   NewLine |\r\n|
  name: 'extension3'
//@[2:20)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:20)    StringSyntax
//@[8:20)     StringComplete |'extension3'|
//@[20:22)   NewLine |\r\n|
  scope: existing1
//@[2:18)   ObjectPropertySyntax
//@[2:7)    IdentifierSyntax
//@[2:7)     Identifier |scope|
//@[7:8)    Colon |:|
//@[9:18)    VariableAccessSyntax
//@[9:18)     IdentifierSyntax
//@[9:18)      Identifier |existing1|
//@[18:20)   NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

/*
  valid loop cases
*/ 
//@[3:5) NewLine |\r\n|
var storageAccounts = [
//@[0:129) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:19)  IdentifierSyntax
//@[4:19)   Identifier |storageAccounts|
//@[20:21)  Assignment |=|
//@[22:129)  ArraySyntax
//@[22:23)   LeftSquare |[|
//@[23:25)   NewLine |\r\n|
  {
//@[2:50)   ArrayItemSyntax
//@[2:50)    ObjectSyntax
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
    location: 'eastus2'
//@[4:23)     ObjectPropertySyntax
//@[4:12)      IdentifierSyntax
//@[4:12)       Identifier |location|
//@[12:13)      Colon |:|
//@[14:23)      StringSyntax
//@[14:23)       StringComplete |'eastus2'|
//@[23:25)     NewLine |\r\n|
  }
//@[2:3)     RightBrace |}|
//@[3:5)   NewLine |\r\n|
  {
//@[2:49)   ArrayItemSyntax
//@[2:49)    ObjectSyntax
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
    location: 'westus'
//@[4:22)     ObjectPropertySyntax
//@[4:12)      IdentifierSyntax
//@[4:12)       Identifier |location|
//@[12:13)      Colon |:|
//@[14:22)      StringSyntax
//@[14:22)       StringComplete |'westus'|
//@[22:24)     NewLine |\r\n|
  }
//@[2:3)     RightBrace |}|
//@[3:5)   NewLine |\r\n|
]
//@[0:1)   RightSquare |]|
//@[1:5) NewLine |\r\n\r\n|

// just a storage account loop
//@[30:32) NewLine |\r\n|
@sys.description('this is just a storage account loop')
//@[0:284) ResourceDeclarationSyntax
//@[0:55)  DecoratorSyntax
//@[0:1)   At |@|
//@[1:55)   InstanceFunctionCallSyntax
//@[1:4)    VariableAccessSyntax
//@[1:4)     IdentifierSyntax
//@[1:4)      Identifier |sys|
//@[4:5)    Dot |.|
//@[5:16)    IdentifierSyntax
//@[5:16)     Identifier |description|
//@[16:17)    LeftParen |(|
//@[17:54)    FunctionArgumentSyntax
//@[17:54)     StringSyntax
//@[17:54)      StringComplete |'this is just a storage account loop'|
//@[54:55)    RightParen |)|
//@[55:57)  NewLine |\r\n|
resource storageResources 'Microsoft.Storage/storageAccounts@2019-06-01' = [for account in storageAccounts: {
//@[0:8)  Identifier |resource|
//@[9:25)  IdentifierSyntax
//@[9:25)   Identifier |storageResources|
//@[26:72)  StringSyntax
//@[26:72)   StringComplete |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[73:74)  Assignment |=|
//@[75:227)  ForSyntax
//@[75:76)   LeftSquare |[|
//@[76:79)   Identifier |for|
//@[80:87)   LocalVariableSyntax
//@[80:87)    IdentifierSyntax
//@[80:87)     Identifier |account|
//@[88:90)   Identifier |in|
//@[91:106)   VariableAccessSyntax
//@[91:106)    IdentifierSyntax
//@[91:106)     Identifier |storageAccounts|
//@[106:107)   Colon |:|
//@[108:226)   ObjectSyntax
//@[108:109)    LeftBrace |{|
//@[109:111)    NewLine |\r\n|
  name: account.name
//@[2:20)    ObjectPropertySyntax
//@[2:6)     IdentifierSyntax
//@[2:6)      Identifier |name|
//@[6:7)     Colon |:|
//@[8:20)     PropertyAccessSyntax
//@[8:15)      VariableAccessSyntax
//@[8:15)       IdentifierSyntax
//@[8:15)        Identifier |account|
//@[15:16)      Dot |.|
//@[16:20)      IdentifierSyntax
//@[16:20)       Identifier |name|
//@[20:22)    NewLine |\r\n|
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
//@[28:30)    NewLine |\r\n|
  sku: {
//@[2:39)    ObjectPropertySyntax
//@[2:5)     IdentifierSyntax
//@[2:5)      Identifier |sku|
//@[5:6)     Colon |:|
//@[7:39)     ObjectSyntax
//@[7:8)      LeftBrace |{|
//@[8:10)      NewLine |\r\n|
    name: 'Standard_LRS'
//@[4:24)      ObjectPropertySyntax
//@[4:8)       IdentifierSyntax
//@[4:8)        Identifier |name|
//@[8:9)       Colon |:|
//@[10:24)       StringSyntax
//@[10:24)        StringComplete |'Standard_LRS'|
//@[24:26)      NewLine |\r\n|
  }
//@[2:3)      RightBrace |}|
//@[3:5)    NewLine |\r\n|
  kind: 'StorageV2'
//@[2:19)    ObjectPropertySyntax
//@[2:6)     IdentifierSyntax
//@[2:6)      Identifier |kind|
//@[6:7)     Colon |:|
//@[8:19)     StringSyntax
//@[8:19)      StringComplete |'StorageV2'|
//@[19:21)    NewLine |\r\n|
}]
//@[0:1)    RightBrace |}|
//@[1:2)   RightSquare |]|
//@[2:6) NewLine |\r\n\r\n|

// storage account loop with index
//@[34:36) NewLine |\r\n|
@sys.description('this is just a storage account loop with index')
//@[0:318) ResourceDeclarationSyntax
//@[0:66)  DecoratorSyntax
//@[0:1)   At |@|
//@[1:66)   InstanceFunctionCallSyntax
//@[1:4)    VariableAccessSyntax
//@[1:4)     IdentifierSyntax
//@[1:4)      Identifier |sys|
//@[4:5)    Dot |.|
//@[5:16)    IdentifierSyntax
//@[5:16)     Identifier |description|
//@[16:17)    LeftParen |(|
//@[17:65)    FunctionArgumentSyntax
//@[17:65)     StringSyntax
//@[17:65)      StringComplete |'this is just a storage account loop with index'|
//@[65:66)    RightParen |)|
//@[66:68)  NewLine |\r\n|
resource storageResourcesWithIndex 'Microsoft.Storage/storageAccounts@2019-06-01' = [for (account, i) in storageAccounts: {
//@[0:8)  Identifier |resource|
//@[9:34)  IdentifierSyntax
//@[9:34)   Identifier |storageResourcesWithIndex|
//@[35:81)  StringSyntax
//@[35:81)   StringComplete |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[82:83)  Assignment |=|
//@[84:250)  ForSyntax
//@[84:85)   LeftSquare |[|
//@[85:88)   Identifier |for|
//@[89:101)   ForVariableBlockSyntax
//@[89:90)    LeftParen |(|
//@[90:97)    LocalVariableSyntax
//@[90:97)     IdentifierSyntax
//@[90:97)      Identifier |account|
//@[97:98)    Comma |,|
//@[99:100)    LocalVariableSyntax
//@[99:100)     IdentifierSyntax
//@[99:100)      Identifier |i|
//@[100:101)    RightParen |)|
//@[102:104)   Identifier |in|
//@[105:120)   VariableAccessSyntax
//@[105:120)    IdentifierSyntax
//@[105:120)     Identifier |storageAccounts|
//@[120:121)   Colon |:|
//@[122:249)   ObjectSyntax
//@[122:123)    LeftBrace |{|
//@[123:125)    NewLine |\r\n|
  name: '${account.name}${i}'
//@[2:29)    ObjectPropertySyntax
//@[2:6)     IdentifierSyntax
//@[2:6)      Identifier |name|
//@[6:7)     Colon |:|
//@[8:29)     StringSyntax
//@[8:11)      StringLeftPiece |'${|
//@[11:23)      PropertyAccessSyntax
//@[11:18)       VariableAccessSyntax
//@[11:18)        IdentifierSyntax
//@[11:18)         Identifier |account|
//@[18:19)       Dot |.|
//@[19:23)       IdentifierSyntax
//@[19:23)        Identifier |name|
//@[23:26)      StringMiddlePiece |}${|
//@[26:27)      VariableAccessSyntax
//@[26:27)       IdentifierSyntax
//@[26:27)        Identifier |i|
//@[27:29)      StringRightPiece |}'|
//@[29:31)    NewLine |\r\n|
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
//@[28:30)    NewLine |\r\n|
  sku: {
//@[2:39)    ObjectPropertySyntax
//@[2:5)     IdentifierSyntax
//@[2:5)      Identifier |sku|
//@[5:6)     Colon |:|
//@[7:39)     ObjectSyntax
//@[7:8)      LeftBrace |{|
//@[8:10)      NewLine |\r\n|
    name: 'Standard_LRS'
//@[4:24)      ObjectPropertySyntax
//@[4:8)       IdentifierSyntax
//@[4:8)        Identifier |name|
//@[8:9)       Colon |:|
//@[10:24)       StringSyntax
//@[10:24)        StringComplete |'Standard_LRS'|
//@[24:26)      NewLine |\r\n|
  }
//@[2:3)      RightBrace |}|
//@[3:5)    NewLine |\r\n|
  kind: 'StorageV2'
//@[2:19)    ObjectPropertySyntax
//@[2:6)     IdentifierSyntax
//@[2:6)      Identifier |kind|
//@[6:7)     Colon |:|
//@[8:19)     StringSyntax
//@[8:19)      StringComplete |'StorageV2'|
//@[19:21)    NewLine |\r\n|
}]
//@[0:1)    RightBrace |}|
//@[1:2)   RightSquare |]|
//@[2:6) NewLine |\r\n\r\n|

// basic nested loop
//@[20:22) NewLine |\r\n|
@sys.description('this is just a basic nested loop')
//@[0:399) ResourceDeclarationSyntax
//@[0:52)  DecoratorSyntax
//@[0:1)   At |@|
//@[1:52)   InstanceFunctionCallSyntax
//@[1:4)    VariableAccessSyntax
//@[1:4)     IdentifierSyntax
//@[1:4)      Identifier |sys|
//@[4:5)    Dot |.|
//@[5:16)    IdentifierSyntax
//@[5:16)     Identifier |description|
//@[16:17)    LeftParen |(|
//@[17:51)    FunctionArgumentSyntax
//@[17:51)     StringSyntax
//@[17:51)      StringComplete |'this is just a basic nested loop'|
//@[51:52)    RightParen |)|
//@[52:54)  NewLine |\r\n|
resource vnet 'Microsoft.Network/virtualNetworks@2020-06-01' = [for i in range(0, 3): {
//@[0:8)  Identifier |resource|
//@[9:13)  IdentifierSyntax
//@[9:13)   Identifier |vnet|
//@[14:60)  StringSyntax
//@[14:60)   StringComplete |'Microsoft.Network/virtualNetworks@2020-06-01'|
//@[61:62)  Assignment |=|
//@[63:345)  ForSyntax
//@[63:64)   LeftSquare |[|
//@[64:67)   Identifier |for|
//@[68:69)   LocalVariableSyntax
//@[68:69)    IdentifierSyntax
//@[68:69)     Identifier |i|
//@[70:72)   Identifier |in|
//@[73:84)   FunctionCallSyntax
//@[73:78)    IdentifierSyntax
//@[73:78)     Identifier |range|
//@[78:79)    LeftParen |(|
//@[79:81)    FunctionArgumentSyntax
//@[79:80)     IntegerLiteralSyntax
//@[79:80)      Integer |0|
//@[80:81)     Comma |,|
//@[82:83)    FunctionArgumentSyntax
//@[82:83)     IntegerLiteralSyntax
//@[82:83)      Integer |3|
//@[83:84)    RightParen |)|
//@[84:85)   Colon |:|
//@[86:344)   ObjectSyntax
//@[86:87)    LeftBrace |{|
//@[87:89)    NewLine |\r\n|
  name: 'vnet-${i}'
//@[2:19)    ObjectPropertySyntax
//@[2:6)     IdentifierSyntax
//@[2:6)      Identifier |name|
//@[6:7)     Colon |:|
//@[8:19)     StringSyntax
//@[8:16)      StringLeftPiece |'vnet-${|
//@[16:17)      VariableAccessSyntax
//@[16:17)       IdentifierSyntax
//@[16:17)        Identifier |i|
//@[17:19)      StringRightPiece |}'|
//@[19:21)    NewLine |\r\n|
  properties: {
//@[2:231)    ObjectPropertySyntax
//@[2:12)     IdentifierSyntax
//@[2:12)      Identifier |properties|
//@[12:13)     Colon |:|
//@[14:231)     ObjectSyntax
//@[14:15)      LeftBrace |{|
//@[15:17)      NewLine |\r\n|
    subnets: [for j in range(0, 4): {
//@[4:209)      ObjectPropertySyntax
//@[4:11)       IdentifierSyntax
//@[4:11)        Identifier |subnets|
//@[11:12)       Colon |:|
//@[13:209)       ForSyntax
//@[13:14)        LeftSquare |[|
//@[14:17)        Identifier |for|
//@[18:19)        LocalVariableSyntax
//@[18:19)         IdentifierSyntax
//@[18:19)          Identifier |j|
//@[20:22)        Identifier |in|
//@[23:34)        FunctionCallSyntax
//@[23:28)         IdentifierSyntax
//@[23:28)          Identifier |range|
//@[28:29)         LeftParen |(|
//@[29:31)         FunctionArgumentSyntax
//@[29:30)          IntegerLiteralSyntax
//@[29:30)           Integer |0|
//@[30:31)          Comma |,|
//@[32:33)         FunctionArgumentSyntax
//@[32:33)          IntegerLiteralSyntax
//@[32:33)           Integer |4|
//@[33:34)         RightParen |)|
//@[34:35)        Colon |:|
//@[36:208)        ObjectSyntax
//@[36:37)         LeftBrace |{|
//@[37:39)         NewLine |\r\n|
      // #completionTest(0,1,2,3,4,5) -> subnetIdAndProperties
//@[62:64)         NewLine |\r\n|
     
//@[5:7)         NewLine |\r\n|
      // #completionTest(6) -> subnetIdAndPropertiesNoColon
//@[59:61)         NewLine |\r\n|
      name: 'subnet-${i}-${j}'
//@[6:30)         ObjectPropertySyntax
//@[6:10)          IdentifierSyntax
//@[6:10)           Identifier |name|
//@[10:11)          Colon |:|
//@[12:30)          StringSyntax
//@[12:22)           StringLeftPiece |'subnet-${|
//@[22:23)           VariableAccessSyntax
//@[22:23)            IdentifierSyntax
//@[22:23)             Identifier |i|
//@[23:27)           StringMiddlePiece |}-${|
//@[27:28)           VariableAccessSyntax
//@[27:28)            IdentifierSyntax
//@[27:28)             Identifier |j|
//@[28:30)           StringRightPiece |}'|
//@[30:32)         NewLine |\r\n|
    }]
//@[4:5)         RightBrace |}|
//@[5:6)        RightSquare |]|
//@[6:8)      NewLine |\r\n|
  }
//@[2:3)      RightBrace |}|
//@[3:5)    NewLine |\r\n|
}]
//@[0:1)    RightBrace |}|
//@[1:2)   RightSquare |]|
//@[2:6) NewLine |\r\n\r\n|

// duplicate identifiers within the loop are allowed
//@[52:54) NewLine |\r\n|
resource duplicateIdentifiersWithinLoop 'Microsoft.Network/virtualNetworks@2020-06-01' = [for i in range(0, 3): {
//@[0:239) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:39)  IdentifierSyntax
//@[9:39)   Identifier |duplicateIdentifiersWithinLoop|
//@[40:86)  StringSyntax
//@[40:86)   StringComplete |'Microsoft.Network/virtualNetworks@2020-06-01'|
//@[87:88)  Assignment |=|
//@[89:239)  ForSyntax
//@[89:90)   LeftSquare |[|
//@[90:93)   Identifier |for|
//@[94:95)   LocalVariableSyntax
//@[94:95)    IdentifierSyntax
//@[94:95)     Identifier |i|
//@[96:98)   Identifier |in|
//@[99:110)   FunctionCallSyntax
//@[99:104)    IdentifierSyntax
//@[99:104)     Identifier |range|
//@[104:105)    LeftParen |(|
//@[105:107)    FunctionArgumentSyntax
//@[105:106)     IntegerLiteralSyntax
//@[105:106)      Integer |0|
//@[106:107)     Comma |,|
//@[108:109)    FunctionArgumentSyntax
//@[108:109)     IntegerLiteralSyntax
//@[108:109)      Integer |3|
//@[109:110)    RightParen |)|
//@[110:111)   Colon |:|
//@[112:238)   ObjectSyntax
//@[112:113)    LeftBrace |{|
//@[113:115)    NewLine |\r\n|
  name: 'vnet-${i}'
//@[2:19)    ObjectPropertySyntax
//@[2:6)     IdentifierSyntax
//@[2:6)      Identifier |name|
//@[6:7)     Colon |:|
//@[8:19)     StringSyntax
//@[8:16)      StringLeftPiece |'vnet-${|
//@[16:17)      VariableAccessSyntax
//@[16:17)       IdentifierSyntax
//@[16:17)        Identifier |i|
//@[17:19)      StringRightPiece |}'|
//@[19:21)    NewLine |\r\n|
  properties: {
//@[2:99)    ObjectPropertySyntax
//@[2:12)     IdentifierSyntax
//@[2:12)      Identifier |properties|
//@[12:13)     Colon |:|
//@[14:99)     ObjectSyntax
//@[14:15)      LeftBrace |{|
//@[15:17)      NewLine |\r\n|
    subnets: [for i in range(0, 4): {
//@[4:77)      ObjectPropertySyntax
//@[4:11)       IdentifierSyntax
//@[4:11)        Identifier |subnets|
//@[11:12)       Colon |:|
//@[13:77)       ForSyntax
//@[13:14)        LeftSquare |[|
//@[14:17)        Identifier |for|
//@[18:19)        LocalVariableSyntax
//@[18:19)         IdentifierSyntax
//@[18:19)          Identifier |i|
//@[20:22)        Identifier |in|
//@[23:34)        FunctionCallSyntax
//@[23:28)         IdentifierSyntax
//@[23:28)          Identifier |range|
//@[28:29)         LeftParen |(|
//@[29:31)         FunctionArgumentSyntax
//@[29:30)          IntegerLiteralSyntax
//@[29:30)           Integer |0|
//@[30:31)          Comma |,|
//@[32:33)         FunctionArgumentSyntax
//@[32:33)          IntegerLiteralSyntax
//@[32:33)           Integer |4|
//@[33:34)         RightParen |)|
//@[34:35)        Colon |:|
//@[36:76)        ObjectSyntax
//@[36:37)         LeftBrace |{|
//@[37:39)         NewLine |\r\n|
      name: 'subnet-${i}-${i}'
//@[6:30)         ObjectPropertySyntax
//@[6:10)          IdentifierSyntax
//@[6:10)           Identifier |name|
//@[10:11)          Colon |:|
//@[12:30)          StringSyntax
//@[12:22)           StringLeftPiece |'subnet-${|
//@[22:23)           VariableAccessSyntax
//@[22:23)            IdentifierSyntax
//@[22:23)             Identifier |i|
//@[23:27)           StringMiddlePiece |}-${|
//@[27:28)           VariableAccessSyntax
//@[27:28)            IdentifierSyntax
//@[27:28)             Identifier |i|
//@[28:30)           StringRightPiece |}'|
//@[30:32)         NewLine |\r\n|
    }]
//@[4:5)         RightBrace |}|
//@[5:6)        RightSquare |]|
//@[6:8)      NewLine |\r\n|
  }
//@[2:3)      RightBrace |}|
//@[3:5)    NewLine |\r\n|
}]
//@[0:1)    RightBrace |}|
//@[1:2)   RightSquare |]|
//@[2:6) NewLine |\r\n\r\n|

// duplicate identifers in global and single loop scope are allowed (inner variable hides the outer)
//@[100:102) NewLine |\r\n|
var canHaveDuplicatesAcrossScopes = 'hello'
//@[0:43) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:33)  IdentifierSyntax
//@[4:33)   Identifier |canHaveDuplicatesAcrossScopes|
//@[34:35)  Assignment |=|
//@[36:43)  StringSyntax
//@[36:43)   StringComplete |'hello'|
//@[43:45) NewLine |\r\n|
resource duplicateInGlobalAndOneLoop 'Microsoft.Network/virtualNetworks@2020-06-01' = [for canHaveDuplicatesAcrossScopes in range(0, 3): {
//@[0:292) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:36)  IdentifierSyntax
//@[9:36)   Identifier |duplicateInGlobalAndOneLoop|
//@[37:83)  StringSyntax
//@[37:83)   StringComplete |'Microsoft.Network/virtualNetworks@2020-06-01'|
//@[84:85)  Assignment |=|
//@[86:292)  ForSyntax
//@[86:87)   LeftSquare |[|
//@[87:90)   Identifier |for|
//@[91:120)   LocalVariableSyntax
//@[91:120)    IdentifierSyntax
//@[91:120)     Identifier |canHaveDuplicatesAcrossScopes|
//@[121:123)   Identifier |in|
//@[124:135)   FunctionCallSyntax
//@[124:129)    IdentifierSyntax
//@[124:129)     Identifier |range|
//@[129:130)    LeftParen |(|
//@[130:132)    FunctionArgumentSyntax
//@[130:131)     IntegerLiteralSyntax
//@[130:131)      Integer |0|
//@[131:132)     Comma |,|
//@[133:134)    FunctionArgumentSyntax
//@[133:134)     IntegerLiteralSyntax
//@[133:134)      Integer |3|
//@[134:135)    RightParen |)|
//@[135:136)   Colon |:|
//@[137:291)   ObjectSyntax
//@[137:138)    LeftBrace |{|
//@[138:140)    NewLine |\r\n|
  name: 'vnet-${canHaveDuplicatesAcrossScopes}'
//@[2:47)    ObjectPropertySyntax
//@[2:6)     IdentifierSyntax
//@[2:6)      Identifier |name|
//@[6:7)     Colon |:|
//@[8:47)     StringSyntax
//@[8:16)      StringLeftPiece |'vnet-${|
//@[16:45)      VariableAccessSyntax
//@[16:45)       IdentifierSyntax
//@[16:45)        Identifier |canHaveDuplicatesAcrossScopes|
//@[45:47)      StringRightPiece |}'|
//@[47:49)    NewLine |\r\n|
  properties: {
//@[2:99)    ObjectPropertySyntax
//@[2:12)     IdentifierSyntax
//@[2:12)      Identifier |properties|
//@[12:13)     Colon |:|
//@[14:99)     ObjectSyntax
//@[14:15)      LeftBrace |{|
//@[15:17)      NewLine |\r\n|
    subnets: [for i in range(0, 4): {
//@[4:77)      ObjectPropertySyntax
//@[4:11)       IdentifierSyntax
//@[4:11)        Identifier |subnets|
//@[11:12)       Colon |:|
//@[13:77)       ForSyntax
//@[13:14)        LeftSquare |[|
//@[14:17)        Identifier |for|
//@[18:19)        LocalVariableSyntax
//@[18:19)         IdentifierSyntax
//@[18:19)          Identifier |i|
//@[20:22)        Identifier |in|
//@[23:34)        FunctionCallSyntax
//@[23:28)         IdentifierSyntax
//@[23:28)          Identifier |range|
//@[28:29)         LeftParen |(|
//@[29:31)         FunctionArgumentSyntax
//@[29:30)          IntegerLiteralSyntax
//@[29:30)           Integer |0|
//@[30:31)          Comma |,|
//@[32:33)         FunctionArgumentSyntax
//@[32:33)          IntegerLiteralSyntax
//@[32:33)           Integer |4|
//@[33:34)         RightParen |)|
//@[34:35)        Colon |:|
//@[36:76)        ObjectSyntax
//@[36:37)         LeftBrace |{|
//@[37:39)         NewLine |\r\n|
      name: 'subnet-${i}-${i}'
//@[6:30)         ObjectPropertySyntax
//@[6:10)          IdentifierSyntax
//@[6:10)           Identifier |name|
//@[10:11)          Colon |:|
//@[12:30)          StringSyntax
//@[12:22)           StringLeftPiece |'subnet-${|
//@[22:23)           VariableAccessSyntax
//@[22:23)            IdentifierSyntax
//@[22:23)             Identifier |i|
//@[23:27)           StringMiddlePiece |}-${|
//@[27:28)           VariableAccessSyntax
//@[27:28)            IdentifierSyntax
//@[27:28)             Identifier |i|
//@[28:30)           StringRightPiece |}'|
//@[30:32)         NewLine |\r\n|
    }]
//@[4:5)         RightBrace |}|
//@[5:6)        RightSquare |]|
//@[6:8)      NewLine |\r\n|
  }
//@[2:3)      RightBrace |}|
//@[3:5)    NewLine |\r\n|
}]
//@[0:1)    RightBrace |}|
//@[1:2)   RightSquare |]|
//@[2:6) NewLine |\r\n\r\n|

// duplicate in global and multiple loop scopes are allowed (inner hides the outer)
//@[83:85) NewLine |\r\n|
var duplicatesEverywhere = 'hello'
//@[0:34) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:24)  IdentifierSyntax
//@[4:24)   Identifier |duplicatesEverywhere|
//@[25:26)  Assignment |=|
//@[27:34)  StringSyntax
//@[27:34)   StringComplete |'hello'|
//@[34:36) NewLine |\r\n|
resource duplicateInGlobalAndTwoLoops 'Microsoft.Network/virtualNetworks@2020-06-01' = [for duplicatesEverywhere in range(0, 3): {
//@[0:308) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:37)  IdentifierSyntax
//@[9:37)   Identifier |duplicateInGlobalAndTwoLoops|
//@[38:84)  StringSyntax
//@[38:84)   StringComplete |'Microsoft.Network/virtualNetworks@2020-06-01'|
//@[85:86)  Assignment |=|
//@[87:308)  ForSyntax
//@[87:88)   LeftSquare |[|
//@[88:91)   Identifier |for|
//@[92:112)   LocalVariableSyntax
//@[92:112)    IdentifierSyntax
//@[92:112)     Identifier |duplicatesEverywhere|
//@[113:115)   Identifier |in|
//@[116:127)   FunctionCallSyntax
//@[116:121)    IdentifierSyntax
//@[116:121)     Identifier |range|
//@[121:122)    LeftParen |(|
//@[122:124)    FunctionArgumentSyntax
//@[122:123)     IntegerLiteralSyntax
//@[122:123)      Integer |0|
//@[123:124)     Comma |,|
//@[125:126)    FunctionArgumentSyntax
//@[125:126)     IntegerLiteralSyntax
//@[125:126)      Integer |3|
//@[126:127)    RightParen |)|
//@[127:128)   Colon |:|
//@[129:307)   ObjectSyntax
//@[129:130)    LeftBrace |{|
//@[130:132)    NewLine |\r\n|
  name: 'vnet-${duplicatesEverywhere}'
//@[2:38)    ObjectPropertySyntax
//@[2:6)     IdentifierSyntax
//@[2:6)      Identifier |name|
//@[6:7)     Colon |:|
//@[8:38)     StringSyntax
//@[8:16)      StringLeftPiece |'vnet-${|
//@[16:36)      VariableAccessSyntax
//@[16:36)       IdentifierSyntax
//@[16:36)        Identifier |duplicatesEverywhere|
//@[36:38)      StringRightPiece |}'|
//@[38:40)    NewLine |\r\n|
  properties: {
//@[2:132)    ObjectPropertySyntax
//@[2:12)     IdentifierSyntax
//@[2:12)      Identifier |properties|
//@[12:13)     Colon |:|
//@[14:132)     ObjectSyntax
//@[14:15)      LeftBrace |{|
//@[15:17)      NewLine |\r\n|
    subnets: [for duplicatesEverywhere in range(0, 4): {
//@[4:110)      ObjectPropertySyntax
//@[4:11)       IdentifierSyntax
//@[4:11)        Identifier |subnets|
//@[11:12)       Colon |:|
//@[13:110)       ForSyntax
//@[13:14)        LeftSquare |[|
//@[14:17)        Identifier |for|
//@[18:38)        LocalVariableSyntax
//@[18:38)         IdentifierSyntax
//@[18:38)          Identifier |duplicatesEverywhere|
//@[39:41)        Identifier |in|
//@[42:53)        FunctionCallSyntax
//@[42:47)         IdentifierSyntax
//@[42:47)          Identifier |range|
//@[47:48)         LeftParen |(|
//@[48:50)         FunctionArgumentSyntax
//@[48:49)          IntegerLiteralSyntax
//@[48:49)           Integer |0|
//@[49:50)          Comma |,|
//@[51:52)         FunctionArgumentSyntax
//@[51:52)          IntegerLiteralSyntax
//@[51:52)           Integer |4|
//@[52:53)         RightParen |)|
//@[53:54)        Colon |:|
//@[55:109)        ObjectSyntax
//@[55:56)         LeftBrace |{|
//@[56:58)         NewLine |\r\n|
      name: 'subnet-${duplicatesEverywhere}'
//@[6:44)         ObjectPropertySyntax
//@[6:10)          IdentifierSyntax
//@[6:10)           Identifier |name|
//@[10:11)          Colon |:|
//@[12:44)          StringSyntax
//@[12:22)           StringLeftPiece |'subnet-${|
//@[22:42)           VariableAccessSyntax
//@[22:42)            IdentifierSyntax
//@[22:42)             Identifier |duplicatesEverywhere|
//@[42:44)           StringRightPiece |}'|
//@[44:46)         NewLine |\r\n|
    }]
//@[4:5)         RightBrace |}|
//@[5:6)        RightSquare |]|
//@[6:8)      NewLine |\r\n|
  }
//@[2:3)      RightBrace |}|
//@[3:5)    NewLine |\r\n|
}]
//@[0:1)    RightBrace |}|
//@[1:2)   RightSquare |]|
//@[2:6) NewLine |\r\n\r\n|

/*
  Scope values created via array access on a resource collection
*/
//@[2:4) NewLine |\r\n|
resource dnsZones 'Microsoft.Network/dnsZones@2018-05-01' = [for zone in range(0,4): {
//@[0:135) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:17)  IdentifierSyntax
//@[9:17)   Identifier |dnsZones|
//@[18:57)  StringSyntax
//@[18:57)   StringComplete |'Microsoft.Network/dnsZones@2018-05-01'|
//@[58:59)  Assignment |=|
//@[60:135)  ForSyntax
//@[60:61)   LeftSquare |[|
//@[61:64)   Identifier |for|
//@[65:69)   LocalVariableSyntax
//@[65:69)    IdentifierSyntax
//@[65:69)     Identifier |zone|
//@[70:72)   Identifier |in|
//@[73:83)   FunctionCallSyntax
//@[73:78)    IdentifierSyntax
//@[73:78)     Identifier |range|
//@[78:79)    LeftParen |(|
//@[79:81)    FunctionArgumentSyntax
//@[79:80)     IntegerLiteralSyntax
//@[79:80)      Integer |0|
//@[80:81)     Comma |,|
//@[81:82)    FunctionArgumentSyntax
//@[81:82)     IntegerLiteralSyntax
//@[81:82)      Integer |4|
//@[82:83)    RightParen |)|
//@[83:84)   Colon |:|
//@[85:134)   ObjectSyntax
//@[85:86)    LeftBrace |{|
//@[86:88)    NewLine |\r\n|
  name: 'zone${zone}'
//@[2:21)    ObjectPropertySyntax
//@[2:6)     IdentifierSyntax
//@[2:6)      Identifier |name|
//@[6:7)     Colon |:|
//@[8:21)     StringSyntax
//@[8:15)      StringLeftPiece |'zone${|
//@[15:19)      VariableAccessSyntax
//@[15:19)       IdentifierSyntax
//@[15:19)        Identifier |zone|
//@[19:21)      StringRightPiece |}'|
//@[21:23)    NewLine |\r\n|
  location: 'global'
//@[2:20)    ObjectPropertySyntax
//@[2:10)     IdentifierSyntax
//@[2:10)      Identifier |location|
//@[10:11)     Colon |:|
//@[12:20)     StringSyntax
//@[12:20)      StringComplete |'global'|
//@[20:22)    NewLine |\r\n|
}]
//@[0:1)    RightBrace |}|
//@[1:2)   RightSquare |]|
//@[2:6) NewLine |\r\n\r\n|

resource locksOnZones 'Microsoft.Authorization/locks@2016-09-01' = [for lock in range(0,2): {
//@[0:194) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:21)  IdentifierSyntax
//@[9:21)   Identifier |locksOnZones|
//@[22:64)  StringSyntax
//@[22:64)   StringComplete |'Microsoft.Authorization/locks@2016-09-01'|
//@[65:66)  Assignment |=|
//@[67:194)  ForSyntax
//@[67:68)   LeftSquare |[|
//@[68:71)   Identifier |for|
//@[72:76)   LocalVariableSyntax
//@[72:76)    IdentifierSyntax
//@[72:76)     Identifier |lock|
//@[77:79)   Identifier |in|
//@[80:90)   FunctionCallSyntax
//@[80:85)    IdentifierSyntax
//@[80:85)     Identifier |range|
//@[85:86)    LeftParen |(|
//@[86:88)    FunctionArgumentSyntax
//@[86:87)     IntegerLiteralSyntax
//@[86:87)      Integer |0|
//@[87:88)     Comma |,|
//@[88:89)    FunctionArgumentSyntax
//@[88:89)     IntegerLiteralSyntax
//@[88:89)      Integer |2|
//@[89:90)    RightParen |)|
//@[90:91)   Colon |:|
//@[92:193)   ObjectSyntax
//@[92:93)    LeftBrace |{|
//@[93:95)    NewLine |\r\n|
  name: 'lock${lock}'
//@[2:21)    ObjectPropertySyntax
//@[2:6)     IdentifierSyntax
//@[2:6)      Identifier |name|
//@[6:7)     Colon |:|
//@[8:21)     StringSyntax
//@[8:15)      StringLeftPiece |'lock${|
//@[15:19)      VariableAccessSyntax
//@[15:19)       IdentifierSyntax
//@[15:19)        Identifier |lock|
//@[19:21)      StringRightPiece |}'|
//@[21:23)    NewLine |\r\n|
  properties: {
//@[2:47)    ObjectPropertySyntax
//@[2:12)     IdentifierSyntax
//@[2:12)      Identifier |properties|
//@[12:13)     Colon |:|
//@[14:47)     ObjectSyntax
//@[14:15)      LeftBrace |{|
//@[15:17)      NewLine |\r\n|
    level: 'CanNotDelete'
//@[4:25)      ObjectPropertySyntax
//@[4:9)       IdentifierSyntax
//@[4:9)        Identifier |level|
//@[9:10)       Colon |:|
//@[11:25)       StringSyntax
//@[11:25)        StringComplete |'CanNotDelete'|
//@[25:27)      NewLine |\r\n|
  }
//@[2:3)      RightBrace |}|
//@[3:5)    NewLine |\r\n|
  scope: dnsZones[lock]
//@[2:23)    ObjectPropertySyntax
//@[2:7)     IdentifierSyntax
//@[2:7)      Identifier |scope|
//@[7:8)     Colon |:|
//@[9:23)     ArrayAccessSyntax
//@[9:17)      VariableAccessSyntax
//@[9:17)       IdentifierSyntax
//@[9:17)        Identifier |dnsZones|
//@[17:18)      LeftSquare |[|
//@[18:22)      VariableAccessSyntax
//@[18:22)       IdentifierSyntax
//@[18:22)        Identifier |lock|
//@[22:23)      RightSquare |]|
//@[23:25)    NewLine |\r\n|
}]
//@[0:1)    RightBrace |}|
//@[1:2)   RightSquare |]|
//@[2:6) NewLine |\r\n\r\n|

resource moreLocksOnZones 'Microsoft.Authorization/locks@2016-09-01' = [for (lock, i) in range(0,3): {
//@[0:196) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:25)  IdentifierSyntax
//@[9:25)   Identifier |moreLocksOnZones|
//@[26:68)  StringSyntax
//@[26:68)   StringComplete |'Microsoft.Authorization/locks@2016-09-01'|
//@[69:70)  Assignment |=|
//@[71:196)  ForSyntax
//@[71:72)   LeftSquare |[|
//@[72:75)   Identifier |for|
//@[76:85)   ForVariableBlockSyntax
//@[76:77)    LeftParen |(|
//@[77:81)    LocalVariableSyntax
//@[77:81)     IdentifierSyntax
//@[77:81)      Identifier |lock|
//@[81:82)    Comma |,|
//@[83:84)    LocalVariableSyntax
//@[83:84)     IdentifierSyntax
//@[83:84)      Identifier |i|
//@[84:85)    RightParen |)|
//@[86:88)   Identifier |in|
//@[89:99)   FunctionCallSyntax
//@[89:94)    IdentifierSyntax
//@[89:94)     Identifier |range|
//@[94:95)    LeftParen |(|
//@[95:97)    FunctionArgumentSyntax
//@[95:96)     IntegerLiteralSyntax
//@[95:96)      Integer |0|
//@[96:97)     Comma |,|
//@[97:98)    FunctionArgumentSyntax
//@[97:98)     IntegerLiteralSyntax
//@[97:98)      Integer |3|
//@[98:99)    RightParen |)|
//@[99:100)   Colon |:|
//@[101:195)   ObjectSyntax
//@[101:102)    LeftBrace |{|
//@[102:104)    NewLine |\r\n|
  name: 'another${i}'
//@[2:21)    ObjectPropertySyntax
//@[2:6)     IdentifierSyntax
//@[2:6)      Identifier |name|
//@[6:7)     Colon |:|
//@[8:21)     StringSyntax
//@[8:18)      StringLeftPiece |'another${|
//@[18:19)      VariableAccessSyntax
//@[18:19)       IdentifierSyntax
//@[18:19)        Identifier |i|
//@[19:21)      StringRightPiece |}'|
//@[21:23)    NewLine |\r\n|
  properties: {
//@[2:43)    ObjectPropertySyntax
//@[2:12)     IdentifierSyntax
//@[2:12)      Identifier |properties|
//@[12:13)     Colon |:|
//@[14:43)     ObjectSyntax
//@[14:15)      LeftBrace |{|
//@[15:17)      NewLine |\r\n|
    level: 'ReadOnly'
//@[4:21)      ObjectPropertySyntax
//@[4:9)       IdentifierSyntax
//@[4:9)        Identifier |level|
//@[9:10)       Colon |:|
//@[11:21)       StringSyntax
//@[11:21)        StringComplete |'ReadOnly'|
//@[21:23)      NewLine |\r\n|
  }
//@[2:3)      RightBrace |}|
//@[3:5)    NewLine |\r\n|
  scope: dnsZones[i]
//@[2:20)    ObjectPropertySyntax
//@[2:7)     IdentifierSyntax
//@[2:7)      Identifier |scope|
//@[7:8)     Colon |:|
//@[9:20)     ArrayAccessSyntax
//@[9:17)      VariableAccessSyntax
//@[9:17)       IdentifierSyntax
//@[9:17)        Identifier |dnsZones|
//@[17:18)      LeftSquare |[|
//@[18:19)      VariableAccessSyntax
//@[18:19)       IdentifierSyntax
//@[18:19)        Identifier |i|
//@[19:20)      RightSquare |]|
//@[20:22)    NewLine |\r\n|
}]
//@[0:1)    RightBrace |}|
//@[1:2)   RightSquare |]|
//@[2:6) NewLine |\r\n\r\n|

resource singleLockOnFirstZone 'Microsoft.Authorization/locks@2016-09-01' = {
//@[0:170) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:30)  IdentifierSyntax
//@[9:30)   Identifier |singleLockOnFirstZone|
//@[31:73)  StringSyntax
//@[31:73)   StringComplete |'Microsoft.Authorization/locks@2016-09-01'|
//@[74:75)  Assignment |=|
//@[76:170)  ObjectSyntax
//@[76:77)   LeftBrace |{|
//@[77:79)   NewLine |\r\n|
  name: 'single-lock'
//@[2:21)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:21)    StringSyntax
//@[8:21)     StringComplete |'single-lock'|
//@[21:23)   NewLine |\r\n|
  properties: {
//@[2:43)   ObjectPropertySyntax
//@[2:12)    IdentifierSyntax
//@[2:12)     Identifier |properties|
//@[12:13)    Colon |:|
//@[14:43)    ObjectSyntax
//@[14:15)     LeftBrace |{|
//@[15:17)     NewLine |\r\n|
    level: 'ReadOnly'
//@[4:21)     ObjectPropertySyntax
//@[4:9)      IdentifierSyntax
//@[4:9)       Identifier |level|
//@[9:10)      Colon |:|
//@[11:21)      StringSyntax
//@[11:21)       StringComplete |'ReadOnly'|
//@[21:23)     NewLine |\r\n|
  }
//@[2:3)     RightBrace |}|
//@[3:5)   NewLine |\r\n|
  scope: dnsZones[0]
//@[2:20)   ObjectPropertySyntax
//@[2:7)    IdentifierSyntax
//@[2:7)     Identifier |scope|
//@[7:8)    Colon |:|
//@[9:20)    ArrayAccessSyntax
//@[9:17)     VariableAccessSyntax
//@[9:17)      IdentifierSyntax
//@[9:17)       Identifier |dnsZones|
//@[17:18)     LeftSquare |[|
//@[18:19)     IntegerLiteralSyntax
//@[18:19)      Integer |0|
//@[19:20)     RightSquare |]|
//@[20:22)   NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:7) NewLine |\r\n\r\n\r\n|


resource p1_vnet 'Microsoft.Network/virtualNetworks@2020-06-01' = {
//@[0:234) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:16)  IdentifierSyntax
//@[9:16)   Identifier |p1_vnet|
//@[17:63)  StringSyntax
//@[17:63)   StringComplete |'Microsoft.Network/virtualNetworks@2020-06-01'|
//@[64:65)  Assignment |=|
//@[66:234)  ObjectSyntax
//@[66:67)   LeftBrace |{|
//@[67:69)   NewLine |\r\n|
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
//@[36:38)   NewLine |\r\n|
  name: 'myVnet'
//@[2:16)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:16)    StringSyntax
//@[8:16)     StringComplete |'myVnet'|
//@[16:18)   NewLine |\r\n|
  properties: {
//@[2:106)   ObjectPropertySyntax
//@[2:12)    IdentifierSyntax
//@[2:12)     Identifier |properties|
//@[12:13)    Colon |:|
//@[14:106)    ObjectSyntax
//@[14:15)     LeftBrace |{|
//@[15:17)     NewLine |\r\n|
    addressSpace: {
//@[4:84)     ObjectPropertySyntax
//@[4:16)      IdentifierSyntax
//@[4:16)       Identifier |addressSpace|
//@[16:17)      Colon |:|
//@[18:84)      ObjectSyntax
//@[18:19)       LeftBrace |{|
//@[19:21)       NewLine |\r\n|
      addressPrefixes: [
//@[6:56)       ObjectPropertySyntax
//@[6:21)        IdentifierSyntax
//@[6:21)         Identifier |addressPrefixes|
//@[21:22)        Colon |:|
//@[23:56)        ArraySyntax
//@[23:24)         LeftSquare |[|
//@[24:26)         NewLine |\r\n|
        '10.0.0.0/20'
//@[8:21)         ArrayItemSyntax
//@[8:21)          StringSyntax
//@[8:21)           StringComplete |'10.0.0.0/20'|
//@[21:23)         NewLine |\r\n|
      ]
//@[6:7)         RightSquare |]|
//@[7:9)       NewLine |\r\n|
    }
//@[4:5)       RightBrace |}|
//@[5:7)     NewLine |\r\n|
  }
//@[2:3)     RightBrace |}|
//@[3:5)   NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource p1_subnet1 'Microsoft.Network/virtualNetworks/subnets@2020-06-01' = {
//@[0:175) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:19)  IdentifierSyntax
//@[9:19)   Identifier |p1_subnet1|
//@[20:74)  StringSyntax
//@[20:74)   StringComplete |'Microsoft.Network/virtualNetworks/subnets@2020-06-01'|
//@[75:76)  Assignment |=|
//@[77:175)  ObjectSyntax
//@[77:78)   LeftBrace |{|
//@[78:80)   NewLine |\r\n|
  parent: p1_vnet
//@[2:17)   ObjectPropertySyntax
//@[2:8)    IdentifierSyntax
//@[2:8)     Identifier |parent|
//@[8:9)    Colon |:|
//@[10:17)    VariableAccessSyntax
//@[10:17)     IdentifierSyntax
//@[10:17)      Identifier |p1_vnet|
//@[17:19)   NewLine |\r\n|
  name: 'subnet1'
//@[2:17)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:17)    StringSyntax
//@[8:17)     StringComplete |'subnet1'|
//@[17:19)   NewLine |\r\n|
  properties: {
//@[2:54)   ObjectPropertySyntax
//@[2:12)    IdentifierSyntax
//@[2:12)     Identifier |properties|
//@[12:13)    Colon |:|
//@[14:54)    ObjectSyntax
//@[14:15)     LeftBrace |{|
//@[15:17)     NewLine |\r\n|
    addressPrefix: '10.0.0.0/24'
//@[4:32)     ObjectPropertySyntax
//@[4:17)      IdentifierSyntax
//@[4:17)       Identifier |addressPrefix|
//@[17:18)      Colon |:|
//@[19:32)      StringSyntax
//@[19:32)       StringComplete |'10.0.0.0/24'|
//@[32:34)     NewLine |\r\n|
  }
//@[2:3)     RightBrace |}|
//@[3:5)   NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource p1_subnet2 'Microsoft.Network/virtualNetworks/subnets@2020-06-01' = {
//@[0:175) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:19)  IdentifierSyntax
//@[9:19)   Identifier |p1_subnet2|
//@[20:74)  StringSyntax
//@[20:74)   StringComplete |'Microsoft.Network/virtualNetworks/subnets@2020-06-01'|
//@[75:76)  Assignment |=|
//@[77:175)  ObjectSyntax
//@[77:78)   LeftBrace |{|
//@[78:80)   NewLine |\r\n|
  parent: p1_vnet
//@[2:17)   ObjectPropertySyntax
//@[2:8)    IdentifierSyntax
//@[2:8)     Identifier |parent|
//@[8:9)    Colon |:|
//@[10:17)    VariableAccessSyntax
//@[10:17)     IdentifierSyntax
//@[10:17)      Identifier |p1_vnet|
//@[17:19)   NewLine |\r\n|
  name: 'subnet2'
//@[2:17)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:17)    StringSyntax
//@[8:17)     StringComplete |'subnet2'|
//@[17:19)   NewLine |\r\n|
  properties: {
//@[2:54)   ObjectPropertySyntax
//@[2:12)    IdentifierSyntax
//@[2:12)     Identifier |properties|
//@[12:13)    Colon |:|
//@[14:54)    ObjectSyntax
//@[14:15)     LeftBrace |{|
//@[15:17)     NewLine |\r\n|
    addressPrefix: '10.0.1.0/24'
//@[4:32)     ObjectPropertySyntax
//@[4:17)      IdentifierSyntax
//@[4:17)       Identifier |addressPrefix|
//@[17:18)      Colon |:|
//@[19:32)      StringSyntax
//@[19:32)       StringComplete |'10.0.1.0/24'|
//@[32:34)     NewLine |\r\n|
  }
//@[2:3)     RightBrace |}|
//@[3:5)   NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

output p1_subnet1prefix string = p1_subnet1.properties.addressPrefix
//@[0:68) OutputDeclarationSyntax
//@[0:6)  Identifier |output|
//@[7:23)  IdentifierSyntax
//@[7:23)   Identifier |p1_subnet1prefix|
//@[24:30)  TypeSyntax
//@[24:30)   Identifier |string|
//@[31:32)  Assignment |=|
//@[33:68)  PropertyAccessSyntax
//@[33:54)   PropertyAccessSyntax
//@[33:43)    VariableAccessSyntax
//@[33:43)     IdentifierSyntax
//@[33:43)      Identifier |p1_subnet1|
//@[43:44)    Dot |.|
//@[44:54)    IdentifierSyntax
//@[44:54)     Identifier |properties|
//@[54:55)   Dot |.|
//@[55:68)   IdentifierSyntax
//@[55:68)    Identifier |addressPrefix|
//@[68:70) NewLine |\r\n|
output p1_subnet1name string = p1_subnet1.name
//@[0:46) OutputDeclarationSyntax
//@[0:6)  Identifier |output|
//@[7:21)  IdentifierSyntax
//@[7:21)   Identifier |p1_subnet1name|
//@[22:28)  TypeSyntax
//@[22:28)   Identifier |string|
//@[29:30)  Assignment |=|
//@[31:46)  PropertyAccessSyntax
//@[31:41)   VariableAccessSyntax
//@[31:41)    IdentifierSyntax
//@[31:41)     Identifier |p1_subnet1|
//@[41:42)   Dot |.|
//@[42:46)   IdentifierSyntax
//@[42:46)    Identifier |name|
//@[46:48) NewLine |\r\n|
output p1_subnet1type string = p1_subnet1.type
//@[0:46) OutputDeclarationSyntax
//@[0:6)  Identifier |output|
//@[7:21)  IdentifierSyntax
//@[7:21)   Identifier |p1_subnet1type|
//@[22:28)  TypeSyntax
//@[22:28)   Identifier |string|
//@[29:30)  Assignment |=|
//@[31:46)  PropertyAccessSyntax
//@[31:41)   VariableAccessSyntax
//@[31:41)    IdentifierSyntax
//@[31:41)     Identifier |p1_subnet1|
//@[41:42)   Dot |.|
//@[42:46)   IdentifierSyntax
//@[42:46)    Identifier |type|
//@[46:48) NewLine |\r\n|
output p1_subnet1id string = p1_subnet1.id
//@[0:42) OutputDeclarationSyntax
//@[0:6)  Identifier |output|
//@[7:19)  IdentifierSyntax
//@[7:19)   Identifier |p1_subnet1id|
//@[20:26)  TypeSyntax
//@[20:26)   Identifier |string|
//@[27:28)  Assignment |=|
//@[29:42)  PropertyAccessSyntax
//@[29:39)   VariableAccessSyntax
//@[29:39)    IdentifierSyntax
//@[29:39)     Identifier |p1_subnet1|
//@[39:40)   Dot |.|
//@[40:42)   IdentifierSyntax
//@[40:42)    Identifier |id|
//@[42:46) NewLine |\r\n\r\n|

// parent property with extension resource
//@[42:44) NewLine |\r\n|
resource p2_res1 'Microsoft.Rp1/resource1@2020-06-01' = {
//@[0:76) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:16)  IdentifierSyntax
//@[9:16)   Identifier |p2_res1|
//@[17:53)  StringSyntax
//@[17:53)   StringComplete |'Microsoft.Rp1/resource1@2020-06-01'|
//@[54:55)  Assignment |=|
//@[56:76)  ObjectSyntax
//@[56:57)   LeftBrace |{|
//@[57:59)   NewLine |\r\n|
  name: 'res1'
//@[2:14)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:14)    StringSyntax
//@[8:14)     StringComplete |'res1'|
//@[14:16)   NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource p2_res1child 'Microsoft.Rp1/resource1/child1@2020-06-01' = {
//@[0:109) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:21)  IdentifierSyntax
//@[9:21)   Identifier |p2_res1child|
//@[22:65)  StringSyntax
//@[22:65)   StringComplete |'Microsoft.Rp1/resource1/child1@2020-06-01'|
//@[66:67)  Assignment |=|
//@[68:109)  ObjectSyntax
//@[68:69)   LeftBrace |{|
//@[69:71)   NewLine |\r\n|
  parent: p2_res1
//@[2:17)   ObjectPropertySyntax
//@[2:8)    IdentifierSyntax
//@[2:8)     Identifier |parent|
//@[8:9)    Colon |:|
//@[10:17)    VariableAccessSyntax
//@[10:17)     IdentifierSyntax
//@[10:17)      Identifier |p2_res1|
//@[17:19)   NewLine |\r\n|
  name: 'child1'
//@[2:16)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:16)    StringSyntax
//@[8:16)     StringComplete |'child1'|
//@[16:18)   NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource p2_res2 'Microsoft.Rp2/resource2@2020-06-01' = {
//@[0:99) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:16)  IdentifierSyntax
//@[9:16)   Identifier |p2_res2|
//@[17:53)  StringSyntax
//@[17:53)   StringComplete |'Microsoft.Rp2/resource2@2020-06-01'|
//@[54:55)  Assignment |=|
//@[56:99)  ObjectSyntax
//@[56:57)   LeftBrace |{|
//@[57:59)   NewLine |\r\n|
  scope: p2_res1child
//@[2:21)   ObjectPropertySyntax
//@[2:7)    IdentifierSyntax
//@[2:7)     Identifier |scope|
//@[7:8)    Colon |:|
//@[9:21)    VariableAccessSyntax
//@[9:21)     IdentifierSyntax
//@[9:21)      Identifier |p2_res1child|
//@[21:23)   NewLine |\r\n|
  name: 'res2'
//@[2:14)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:14)    StringSyntax
//@[8:14)     StringComplete |'res2'|
//@[14:16)   NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource p2_res2child 'Microsoft.Rp2/resource2/child2@2020-06-01' = {
//@[0:109) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:21)  IdentifierSyntax
//@[9:21)   Identifier |p2_res2child|
//@[22:65)  StringSyntax
//@[22:65)   StringComplete |'Microsoft.Rp2/resource2/child2@2020-06-01'|
//@[66:67)  Assignment |=|
//@[68:109)  ObjectSyntax
//@[68:69)   LeftBrace |{|
//@[69:71)   NewLine |\r\n|
  parent: p2_res2
//@[2:17)   ObjectPropertySyntax
//@[2:8)    IdentifierSyntax
//@[2:8)     Identifier |parent|
//@[8:9)    Colon |:|
//@[10:17)    VariableAccessSyntax
//@[10:17)     IdentifierSyntax
//@[10:17)      Identifier |p2_res2|
//@[17:19)   NewLine |\r\n|
  name: 'child2'
//@[2:16)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:16)    StringSyntax
//@[8:16)     StringComplete |'child2'|
//@[16:18)   NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

output p2_res2childprop string = p2_res2child.properties.someProp
//@[0:65) OutputDeclarationSyntax
//@[0:6)  Identifier |output|
//@[7:23)  IdentifierSyntax
//@[7:23)   Identifier |p2_res2childprop|
//@[24:30)  TypeSyntax
//@[24:30)   Identifier |string|
//@[31:32)  Assignment |=|
//@[33:65)  PropertyAccessSyntax
//@[33:56)   PropertyAccessSyntax
//@[33:45)    VariableAccessSyntax
//@[33:45)     IdentifierSyntax
//@[33:45)      Identifier |p2_res2child|
//@[45:46)    Dot |.|
//@[46:56)    IdentifierSyntax
//@[46:56)     Identifier |properties|
//@[56:57)   Dot |.|
//@[57:65)   IdentifierSyntax
//@[57:65)    Identifier |someProp|
//@[65:67) NewLine |\r\n|
output p2_res2childname string = p2_res2child.name
//@[0:50) OutputDeclarationSyntax
//@[0:6)  Identifier |output|
//@[7:23)  IdentifierSyntax
//@[7:23)   Identifier |p2_res2childname|
//@[24:30)  TypeSyntax
//@[24:30)   Identifier |string|
//@[31:32)  Assignment |=|
//@[33:50)  PropertyAccessSyntax
//@[33:45)   VariableAccessSyntax
//@[33:45)    IdentifierSyntax
//@[33:45)     Identifier |p2_res2child|
//@[45:46)   Dot |.|
//@[46:50)   IdentifierSyntax
//@[46:50)    Identifier |name|
//@[50:52) NewLine |\r\n|
output p2_res2childtype string = p2_res2child.type
//@[0:50) OutputDeclarationSyntax
//@[0:6)  Identifier |output|
//@[7:23)  IdentifierSyntax
//@[7:23)   Identifier |p2_res2childtype|
//@[24:30)  TypeSyntax
//@[24:30)   Identifier |string|
//@[31:32)  Assignment |=|
//@[33:50)  PropertyAccessSyntax
//@[33:45)   VariableAccessSyntax
//@[33:45)    IdentifierSyntax
//@[33:45)     Identifier |p2_res2child|
//@[45:46)   Dot |.|
//@[46:50)   IdentifierSyntax
//@[46:50)    Identifier |type|
//@[50:52) NewLine |\r\n|
output p2_res2childid string = p2_res2child.id
//@[0:46) OutputDeclarationSyntax
//@[0:6)  Identifier |output|
//@[7:21)  IdentifierSyntax
//@[7:21)   Identifier |p2_res2childid|
//@[22:28)  TypeSyntax
//@[22:28)   Identifier |string|
//@[29:30)  Assignment |=|
//@[31:46)  PropertyAccessSyntax
//@[31:43)   VariableAccessSyntax
//@[31:43)    IdentifierSyntax
//@[31:43)     Identifier |p2_res2child|
//@[43:44)   Dot |.|
//@[44:46)   IdentifierSyntax
//@[44:46)    Identifier |id|
//@[46:50) NewLine |\r\n\r\n|

// parent property with 'existing' resource
//@[43:45) NewLine |\r\n|
resource p3_res1 'Microsoft.Rp1/resource1@2020-06-01' existing = {
//@[0:85) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:16)  IdentifierSyntax
//@[9:16)   Identifier |p3_res1|
//@[17:53)  StringSyntax
//@[17:53)   StringComplete |'Microsoft.Rp1/resource1@2020-06-01'|
//@[54:62)  Identifier |existing|
//@[63:64)  Assignment |=|
//@[65:85)  ObjectSyntax
//@[65:66)   LeftBrace |{|
//@[66:68)   NewLine |\r\n|
  name: 'res1'
//@[2:14)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:14)    StringSyntax
//@[8:14)     StringComplete |'res1'|
//@[14:16)   NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource p3_child1 'Microsoft.Rp1/resource1/child1@2020-06-01' = {
//@[0:106) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:18)  IdentifierSyntax
//@[9:18)   Identifier |p3_child1|
//@[19:62)  StringSyntax
//@[19:62)   StringComplete |'Microsoft.Rp1/resource1/child1@2020-06-01'|
//@[63:64)  Assignment |=|
//@[65:106)  ObjectSyntax
//@[65:66)   LeftBrace |{|
//@[66:68)   NewLine |\r\n|
  parent: p3_res1
//@[2:17)   ObjectPropertySyntax
//@[2:8)    IdentifierSyntax
//@[2:8)     Identifier |parent|
//@[8:9)    Colon |:|
//@[10:17)    VariableAccessSyntax
//@[10:17)     IdentifierSyntax
//@[10:17)      Identifier |p3_res1|
//@[17:19)   NewLine |\r\n|
  name: 'child1'
//@[2:16)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:16)    StringSyntax
//@[8:16)     StringComplete |'child1'|
//@[16:18)   NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

output p3_res1childprop string = p3_child1.properties.someProp
//@[0:62) OutputDeclarationSyntax
//@[0:6)  Identifier |output|
//@[7:23)  IdentifierSyntax
//@[7:23)   Identifier |p3_res1childprop|
//@[24:30)  TypeSyntax
//@[24:30)   Identifier |string|
//@[31:32)  Assignment |=|
//@[33:62)  PropertyAccessSyntax
//@[33:53)   PropertyAccessSyntax
//@[33:42)    VariableAccessSyntax
//@[33:42)     IdentifierSyntax
//@[33:42)      Identifier |p3_child1|
//@[42:43)    Dot |.|
//@[43:53)    IdentifierSyntax
//@[43:53)     Identifier |properties|
//@[53:54)   Dot |.|
//@[54:62)   IdentifierSyntax
//@[54:62)    Identifier |someProp|
//@[62:64) NewLine |\r\n|
output p3_res1childname string = p3_child1.name
//@[0:47) OutputDeclarationSyntax
//@[0:6)  Identifier |output|
//@[7:23)  IdentifierSyntax
//@[7:23)   Identifier |p3_res1childname|
//@[24:30)  TypeSyntax
//@[24:30)   Identifier |string|
//@[31:32)  Assignment |=|
//@[33:47)  PropertyAccessSyntax
//@[33:42)   VariableAccessSyntax
//@[33:42)    IdentifierSyntax
//@[33:42)     Identifier |p3_child1|
//@[42:43)   Dot |.|
//@[43:47)   IdentifierSyntax
//@[43:47)    Identifier |name|
//@[47:49) NewLine |\r\n|
output p3_res1childtype string = p3_child1.type
//@[0:47) OutputDeclarationSyntax
//@[0:6)  Identifier |output|
//@[7:23)  IdentifierSyntax
//@[7:23)   Identifier |p3_res1childtype|
//@[24:30)  TypeSyntax
//@[24:30)   Identifier |string|
//@[31:32)  Assignment |=|
//@[33:47)  PropertyAccessSyntax
//@[33:42)   VariableAccessSyntax
//@[33:42)    IdentifierSyntax
//@[33:42)     Identifier |p3_child1|
//@[42:43)   Dot |.|
//@[43:47)   IdentifierSyntax
//@[43:47)    Identifier |type|
//@[47:49) NewLine |\r\n|
output p3_res1childid string = p3_child1.id
//@[0:43) OutputDeclarationSyntax
//@[0:6)  Identifier |output|
//@[7:21)  IdentifierSyntax
//@[7:21)   Identifier |p3_res1childid|
//@[22:28)  TypeSyntax
//@[22:28)   Identifier |string|
//@[29:30)  Assignment |=|
//@[31:43)  PropertyAccessSyntax
//@[31:40)   VariableAccessSyntax
//@[31:40)    IdentifierSyntax
//@[31:40)     Identifier |p3_child1|
//@[40:41)   Dot |.|
//@[41:43)   IdentifierSyntax
//@[41:43)    Identifier |id|
//@[43:47) NewLine |\r\n\r\n|

// parent & child with 'existing'
//@[33:35) NewLine |\r\n|
resource p4_res1 'Microsoft.Rp1/resource1@2020-06-01' existing = {
//@[0:104) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:16)  IdentifierSyntax
//@[9:16)   Identifier |p4_res1|
//@[17:53)  StringSyntax
//@[17:53)   StringComplete |'Microsoft.Rp1/resource1@2020-06-01'|
//@[54:62)  Identifier |existing|
//@[63:64)  Assignment |=|
//@[65:104)  ObjectSyntax
//@[65:66)   LeftBrace |{|
//@[66:68)   NewLine |\r\n|
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
//@[17:19)   NewLine |\r\n|
  name: 'res1'
//@[2:14)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:14)    StringSyntax
//@[8:14)     StringComplete |'res1'|
//@[14:16)   NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource p4_child1 'Microsoft.Rp1/resource1/child1@2020-06-01' existing = {
//@[0:115) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:18)  IdentifierSyntax
//@[9:18)   Identifier |p4_child1|
//@[19:62)  StringSyntax
//@[19:62)   StringComplete |'Microsoft.Rp1/resource1/child1@2020-06-01'|
//@[63:71)  Identifier |existing|
//@[72:73)  Assignment |=|
//@[74:115)  ObjectSyntax
//@[74:75)   LeftBrace |{|
//@[75:77)   NewLine |\r\n|
  parent: p4_res1
//@[2:17)   ObjectPropertySyntax
//@[2:8)    IdentifierSyntax
//@[2:8)     Identifier |parent|
//@[8:9)    Colon |:|
//@[10:17)    VariableAccessSyntax
//@[10:17)     IdentifierSyntax
//@[10:17)      Identifier |p4_res1|
//@[17:19)   NewLine |\r\n|
  name: 'child1'
//@[2:16)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:16)    StringSyntax
//@[8:16)     StringComplete |'child1'|
//@[16:18)   NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

output p4_res1childprop string = p4_child1.properties.someProp
//@[0:62) OutputDeclarationSyntax
//@[0:6)  Identifier |output|
//@[7:23)  IdentifierSyntax
//@[7:23)   Identifier |p4_res1childprop|
//@[24:30)  TypeSyntax
//@[24:30)   Identifier |string|
//@[31:32)  Assignment |=|
//@[33:62)  PropertyAccessSyntax
//@[33:53)   PropertyAccessSyntax
//@[33:42)    VariableAccessSyntax
//@[33:42)     IdentifierSyntax
//@[33:42)      Identifier |p4_child1|
//@[42:43)    Dot |.|
//@[43:53)    IdentifierSyntax
//@[43:53)     Identifier |properties|
//@[53:54)   Dot |.|
//@[54:62)   IdentifierSyntax
//@[54:62)    Identifier |someProp|
//@[62:64) NewLine |\r\n|
output p4_res1childname string = p4_child1.name
//@[0:47) OutputDeclarationSyntax
//@[0:6)  Identifier |output|
//@[7:23)  IdentifierSyntax
//@[7:23)   Identifier |p4_res1childname|
//@[24:30)  TypeSyntax
//@[24:30)   Identifier |string|
//@[31:32)  Assignment |=|
//@[33:47)  PropertyAccessSyntax
//@[33:42)   VariableAccessSyntax
//@[33:42)    IdentifierSyntax
//@[33:42)     Identifier |p4_child1|
//@[42:43)   Dot |.|
//@[43:47)   IdentifierSyntax
//@[43:47)    Identifier |name|
//@[47:49) NewLine |\r\n|
output p4_res1childtype string = p4_child1.type
//@[0:47) OutputDeclarationSyntax
//@[0:6)  Identifier |output|
//@[7:23)  IdentifierSyntax
//@[7:23)   Identifier |p4_res1childtype|
//@[24:30)  TypeSyntax
//@[24:30)   Identifier |string|
//@[31:32)  Assignment |=|
//@[33:47)  PropertyAccessSyntax
//@[33:42)   VariableAccessSyntax
//@[33:42)    IdentifierSyntax
//@[33:42)     Identifier |p4_child1|
//@[42:43)   Dot |.|
//@[43:47)   IdentifierSyntax
//@[43:47)    Identifier |type|
//@[47:49) NewLine |\r\n|
output p4_res1childid string = p4_child1.id
//@[0:43) OutputDeclarationSyntax
//@[0:6)  Identifier |output|
//@[7:21)  IdentifierSyntax
//@[7:21)   Identifier |p4_res1childid|
//@[22:28)  TypeSyntax
//@[22:28)   Identifier |string|
//@[29:30)  Assignment |=|
//@[31:43)  PropertyAccessSyntax
//@[31:40)   VariableAccessSyntax
//@[31:40)    IdentifierSyntax
//@[31:40)     Identifier |p4_child1|
//@[40:41)   Dot |.|
//@[41:43)   IdentifierSyntax
//@[41:43)    Identifier |id|
//@[43:45) NewLine |\r\n|

//@[0:0) EndOfFile ||
