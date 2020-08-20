
//@[0:2] NewLine |\r\n|
resource basicStorage 'Microsoft.Storage/storageAccounts@2019-06-01' = {
//@[0:8] Identifier |resource|
//@[9:21] Identifier |basicStorage|
//@[22:68] StringComplete |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[69:70] Assignment |=|
//@[71:72] LeftBrace |{|
//@[72:74] NewLine |\r\n|
  name: 'basicblobs'
//@[2:6] Identifier |name|
//@[6:7] Colon |:|
//@[8:20] StringComplete |'basicblobs'|
//@[20:22] NewLine |\r\n|
  location: 'westus'
//@[2:10] Identifier |location|
//@[10:11] Colon |:|
//@[12:20] StringComplete |'westus'|
//@[20:22] NewLine |\r\n|
}
//@[0:1] RightBrace |}|
//@[1:5] NewLine |\r\n\r\n|

resource dnsZone 'Microsoft.Network/dnszones@2018-05-01' = {
//@[0:8] Identifier |resource|
//@[9:16] Identifier |dnsZone|
//@[17:56] StringComplete |'Microsoft.Network/dnszones@2018-05-01'|
//@[57:58] Assignment |=|
//@[59:60] LeftBrace |{|
//@[60:62] NewLine |\r\n|
  name: 'myZone'
//@[2:6] Identifier |name|
//@[6:7] Colon |:|
//@[8:16] StringComplete |'myZone'|
//@[16:18] NewLine |\r\n|
  location: 'global'
//@[2:10] Identifier |location|
//@[10:11] Colon |:|
//@[12:20] StringComplete |'global'|
//@[20:22] NewLine |\r\n|
}
//@[0:1] RightBrace |}|
//@[1:5] NewLine |\r\n\r\n|

resource myStorageAccount 'Microsoft.Storage/storageAccounts@2017-10-01' = {
//@[0:8] Identifier |resource|
//@[9:25] Identifier |myStorageAccount|
//@[26:72] StringComplete |'Microsoft.Storage/storageAccounts@2017-10-01'|
//@[73:74] Assignment |=|
//@[75:76] LeftBrace |{|
//@[76:78] NewLine |\r\n|
  name: 'myencryptedone'
//@[2:6] Identifier |name|
//@[6:7] Colon |:|
//@[8:24] StringComplete |'myencryptedone'|
//@[24:26] NewLine |\r\n|
  location: 'eastus2'
//@[2:10] Identifier |location|
//@[10:11] Colon |:|
//@[12:21] StringComplete |'eastus2'|
//@[21:23] NewLine |\r\n|
  properties: {
//@[2:12] Identifier |properties|
//@[12:13] Colon |:|
//@[14:15] LeftBrace |{|
//@[15:17] NewLine |\r\n|
    supportsHttpsTrafficOnly: true
//@[4:28] Identifier |supportsHttpsTrafficOnly|
//@[28:29] Colon |:|
//@[30:34] TrueKeyword |true|
//@[34:36] NewLine |\r\n|
    accessTier: 'Hot'
//@[4:14] Identifier |accessTier|
//@[14:15] Colon |:|
//@[16:21] StringComplete |'Hot'|
//@[21:23] NewLine |\r\n|
    encryption: {
//@[4:14] Identifier |encryption|
//@[14:15] Colon |:|
//@[16:17] LeftBrace |{|
//@[17:19] NewLine |\r\n|
      keySource: 'Microsoft.Storage'
//@[6:15] Identifier |keySource|
//@[15:16] Colon |:|
//@[17:36] StringComplete |'Microsoft.Storage'|
//@[36:38] NewLine |\r\n|
      services: {
//@[6:14] Identifier |services|
//@[14:15] Colon |:|
//@[16:17] LeftBrace |{|
//@[17:19] NewLine |\r\n|
        blob: {
//@[8:12] Identifier |blob|
//@[12:13] Colon |:|
//@[14:15] LeftBrace |{|
//@[15:17] NewLine |\r\n|
          enabled: true
//@[10:17] Identifier |enabled|
//@[17:18] Colon |:|
//@[19:23] TrueKeyword |true|
//@[23:25] NewLine |\r\n|
        }
//@[8:9] RightBrace |}|
//@[9:11] NewLine |\r\n|
        file: {
//@[8:12] Identifier |file|
//@[12:13] Colon |:|
//@[14:15] LeftBrace |{|
//@[15:17] NewLine |\r\n|
          enabled: true
//@[10:17] Identifier |enabled|
//@[17:18] Colon |:|
//@[19:23] TrueKeyword |true|
//@[23:25] NewLine |\r\n|
        }
//@[8:9] RightBrace |}|
//@[9:11] NewLine |\r\n|
      }
//@[6:7] RightBrace |}|
//@[7:9] NewLine |\r\n|
    }
//@[4:5] RightBrace |}|
//@[5:7] NewLine |\r\n|
  }
//@[2:3] RightBrace |}|
//@[3:5] NewLine |\r\n|
  kind: 'StorageV2'
//@[2:6] Identifier |kind|
//@[6:7] Colon |:|
//@[8:19] StringComplete |'StorageV2'|
//@[19:21] NewLine |\r\n|
  sku: {
//@[2:5] Identifier |sku|
//@[5:6] Colon |:|
//@[7:8] LeftBrace |{|
//@[8:10] NewLine |\r\n|
    name: 'Standard_LRS'
//@[4:8] Identifier |name|
//@[8:9] Colon |:|
//@[10:24] StringComplete |'Standard_LRS'|
//@[24:26] NewLine |\r\n|
  }
//@[2:3] RightBrace |}|
//@[3:5] NewLine |\r\n|
}
//@[0:1] RightBrace |}|
//@[1:5] NewLine |\r\n\r\n|

resource withExpressions 'Microsoft.Storage/storageAccounts@2017-10-01' = {
//@[0:8] Identifier |resource|
//@[9:24] Identifier |withExpressions|
//@[25:71] StringComplete |'Microsoft.Storage/storageAccounts@2017-10-01'|
//@[72:73] Assignment |=|
//@[74:75] LeftBrace |{|
//@[75:77] NewLine |\r\n|
  name: 'myencryptedone'
//@[2:6] Identifier |name|
//@[6:7] Colon |:|
//@[8:24] StringComplete |'myencryptedone'|
//@[24:26] NewLine |\r\n|
  location: 'eastus2'
//@[2:10] Identifier |location|
//@[10:11] Colon |:|
//@[12:21] StringComplete |'eastus2'|
//@[21:23] NewLine |\r\n|
  properties: {
//@[2:12] Identifier |properties|
//@[12:13] Colon |:|
//@[14:15] LeftBrace |{|
//@[15:17] NewLine |\r\n|
    supportsHttpsTrafficOnly: !false
//@[4:28] Identifier |supportsHttpsTrafficOnly|
//@[28:29] Colon |:|
//@[30:31] Exclamation |!|
//@[31:36] FalseKeyword |false|
//@[36:38] NewLine |\r\n|
    accessTier: true ? 'Hot' : 'Cold'
//@[4:14] Identifier |accessTier|
//@[14:15] Colon |:|
//@[16:20] TrueKeyword |true|
//@[21:22] Question |?|
//@[23:28] StringComplete |'Hot'|
//@[29:30] Colon |:|
//@[31:37] StringComplete |'Cold'|
//@[37:39] NewLine |\r\n|
    encryption: {
//@[4:14] Identifier |encryption|
//@[14:15] Colon |:|
//@[16:17] LeftBrace |{|
//@[17:19] NewLine |\r\n|
      keySource: 'Microsoft.Storage'
//@[6:15] Identifier |keySource|
//@[15:16] Colon |:|
//@[17:36] StringComplete |'Microsoft.Storage'|
//@[36:38] NewLine |\r\n|
      services: {
//@[6:14] Identifier |services|
//@[14:15] Colon |:|
//@[16:17] LeftBrace |{|
//@[17:19] NewLine |\r\n|
        blob: {
//@[8:12] Identifier |blob|
//@[12:13] Colon |:|
//@[14:15] LeftBrace |{|
//@[15:17] NewLine |\r\n|
          enabled: true || false
//@[10:17] Identifier |enabled|
//@[17:18] Colon |:|
//@[19:23] TrueKeyword |true|
//@[24:26] LogicalOr ||||
//@[27:32] FalseKeyword |false|
//@[32:34] NewLine |\r\n|
        }
//@[8:9] RightBrace |}|
//@[9:11] NewLine |\r\n|
        file: {
//@[8:12] Identifier |file|
//@[12:13] Colon |:|
//@[14:15] LeftBrace |{|
//@[15:17] NewLine |\r\n|
          enabled: true
//@[10:17] Identifier |enabled|
//@[17:18] Colon |:|
//@[19:23] TrueKeyword |true|
//@[23:25] NewLine |\r\n|
        }
//@[8:9] RightBrace |}|
//@[9:11] NewLine |\r\n|
      }
//@[6:7] RightBrace |}|
//@[7:9] NewLine |\r\n|
    }
//@[4:5] RightBrace |}|
//@[5:7] NewLine |\r\n|
  }
//@[2:3] RightBrace |}|
//@[3:5] NewLine |\r\n|
  kind: 'StorageV2'
//@[2:6] Identifier |kind|
//@[6:7] Colon |:|
//@[8:19] StringComplete |'StorageV2'|
//@[19:21] NewLine |\r\n|
  sku: {
//@[2:5] Identifier |sku|
//@[5:6] Colon |:|
//@[7:8] LeftBrace |{|
//@[8:10] NewLine |\r\n|
    name: 'Standard_LRS'
//@[4:8] Identifier |name|
//@[8:9] Colon |:|
//@[10:24] StringComplete |'Standard_LRS'|
//@[24:26] NewLine |\r\n|
  }
//@[2:3] RightBrace |}|
//@[3:5] NewLine |\r\n|
}
//@[0:1] RightBrace |}|
//@[1:5] NewLine |\r\n\r\n|

param applicationName string = 'to-do-app${uniqueString(resourceGroup().id)}'
//@[0:5] Identifier |param|
//@[6:21] Identifier |applicationName|
//@[22:28] Identifier |string|
//@[29:30] Assignment |=|
//@[31:43] StringLeftPiece |'to-do-app${|
//@[43:55] Identifier |uniqueString|
//@[55:56] LeftParen |(|
//@[56:69] Identifier |resourceGroup|
//@[69:70] LeftParen |(|
//@[70:71] RightParen |)|
//@[71:72] Dot |.|
//@[72:74] Identifier |id|
//@[74:75] RightParen |)|
//@[75:77] StringRightPiece |}'|
//@[77:79] NewLine |\r\n|
var hostingPlanName = applicationName // why not just use the param directly?
//@[0:3] Identifier |var|
//@[4:19] Identifier |hostingPlanName|
//@[20:21] Assignment |=|
//@[22:37] Identifier |applicationName|
//@[77:81] NewLine |\r\n\r\n|

param appServicePlanTier string
//@[0:5] Identifier |param|
//@[6:24] Identifier |appServicePlanTier|
//@[25:31] Identifier |string|
//@[31:33] NewLine |\r\n|
param appServicePlanInstances int
//@[0:5] Identifier |param|
//@[6:29] Identifier |appServicePlanInstances|
//@[30:33] Identifier |int|
//@[33:37] NewLine |\r\n\r\n|

var location = resourceGroup().location
//@[0:3] Identifier |var|
//@[4:12] Identifier |location|
//@[13:14] Assignment |=|
//@[15:28] Identifier |resourceGroup|
//@[28:29] LeftParen |(|
//@[29:30] RightParen |)|
//@[30:31] Dot |.|
//@[31:39] Identifier |location|
//@[39:43] NewLine |\r\n\r\n|

resource farm 'Microsoft.Web/serverFarms@2019-08-01' = {
//@[0:8] Identifier |resource|
//@[9:13] Identifier |farm|
//@[14:52] StringComplete |'Microsoft.Web/serverFarms@2019-08-01'|
//@[53:54] Assignment |=|
//@[55:56] LeftBrace |{|
//@[56:58] NewLine |\r\n|
  // dependsOn: resourceId('Microsoft.DocumentDB/databaseAccounts', cosmosAccountName)
//@[86:88] NewLine |\r\n|
  name: hostingPlanName
//@[2:6] Identifier |name|
//@[6:7] Colon |:|
//@[8:23] Identifier |hostingPlanName|
//@[23:25] NewLine |\r\n|
  location: location
//@[2:10] Identifier |location|
//@[10:11] Colon |:|
//@[12:20] Identifier |location|
//@[20:22] NewLine |\r\n|
  sku: {
//@[2:5] Identifier |sku|
//@[5:6] Colon |:|
//@[7:8] LeftBrace |{|
//@[8:10] NewLine |\r\n|
    name: appServicePlanTier
//@[4:8] Identifier |name|
//@[8:9] Colon |:|
//@[10:28] Identifier |appServicePlanTier|
//@[28:30] NewLine |\r\n|
    capacity: appServicePlanInstances
//@[4:12] Identifier |capacity|
//@[12:13] Colon |:|
//@[14:37] Identifier |appServicePlanInstances|
//@[37:39] NewLine |\r\n|
  }
//@[2:3] RightBrace |}|
//@[3:5] NewLine |\r\n|
  properties: {
//@[2:12] Identifier |properties|
//@[12:13] Colon |:|
//@[14:15] LeftBrace |{|
//@[15:17] NewLine |\r\n|
    name: hostingPlanName // just hostingPlanName results in an error
//@[4:8] Identifier |name|
//@[8:9] Colon |:|
//@[10:25] Identifier |hostingPlanName|
//@[69:71] NewLine |\r\n|
  }
//@[2:3] RightBrace |}|
//@[3:5] NewLine |\r\n|
}
//@[0:1] RightBrace |}|
//@[1:5] NewLine |\r\n\r\n|

var cosmosDbResourceId = resourceId('Microsoft.DocumentDB/databaseAccounts', cosmosDb.account)
//@[0:3] Identifier |var|
//@[4:22] Identifier |cosmosDbResourceId|
//@[23:24] Assignment |=|
//@[25:35] Identifier |resourceId|
//@[35:36] LeftParen |(|
//@[36:75] StringComplete |'Microsoft.DocumentDB/databaseAccounts'|
//@[75:76] Comma |,|
//@[77:85] Identifier |cosmosDb|
//@[85:86] Dot |.|
//@[86:93] Identifier |account|
//@[93:94] RightParen |)|
//@[94:96] NewLine |\r\n|
var cosmosDbRef = reference(cosmosDbResourceId).documentEndpoint
//@[0:3] Identifier |var|
//@[4:15] Identifier |cosmosDbRef|
//@[16:17] Assignment |=|
//@[18:27] Identifier |reference|
//@[27:28] LeftParen |(|
//@[28:46] Identifier |cosmosDbResourceId|
//@[46:47] RightParen |)|
//@[47:48] Dot |.|
//@[48:64] Identifier |documentEndpoint|
//@[64:68] NewLine |\r\n\r\n|

// this variable is not accessed anywhere in this template and depends on a run-time reference
//@[94:96] NewLine |\r\n|
// it should not be present at all in the template output as there is nowhere logical to put it
//@[95:97] NewLine |\r\n|
var cosmosDbEndpoint = cosmosDbRef.documentEndpoint
//@[0:3] Identifier |var|
//@[4:20] Identifier |cosmosDbEndpoint|
//@[21:22] Assignment |=|
//@[23:34] Identifier |cosmosDbRef|
//@[34:35] Dot |.|
//@[35:51] Identifier |documentEndpoint|
//@[51:55] NewLine |\r\n\r\n|

param webSiteName string
//@[0:5] Identifier |param|
//@[6:17] Identifier |webSiteName|
//@[18:24] Identifier |string|
//@[24:26] NewLine |\r\n|
param cosmosDb object
//@[0:5] Identifier |param|
//@[6:14] Identifier |cosmosDb|
//@[15:21] Identifier |object|
//@[21:23] NewLine |\r\n|
resource site 'Microsoft.Web/sites@2019-08-01' = {
//@[0:8] Identifier |resource|
//@[9:13] Identifier |site|
//@[14:46] StringComplete |'Microsoft.Web/sites@2019-08-01'|
//@[47:48] Assignment |=|
//@[49:50] LeftBrace |{|
//@[50:52] NewLine |\r\n|
  name: webSiteName
//@[2:6] Identifier |name|
//@[6:7] Colon |:|
//@[8:19] Identifier |webSiteName|
//@[19:21] NewLine |\r\n|
  location: location
//@[2:10] Identifier |location|
//@[10:11] Colon |:|
//@[12:20] Identifier |location|
//@[20:22] NewLine |\r\n|
  properties: {
//@[2:12] Identifier |properties|
//@[12:13] Colon |:|
//@[14:15] LeftBrace |{|
//@[15:17] NewLine |\r\n|
    // not yet supported // serverFarmId: farm.id
//@[49:51] NewLine |\r\n|
    siteConfig: {
//@[4:14] Identifier |siteConfig|
//@[14:15] Colon |:|
//@[16:17] LeftBrace |{|
//@[17:19] NewLine |\r\n|
      appSettings: [
//@[6:17] Identifier |appSettings|
//@[17:18] Colon |:|
//@[19:20] LeftSquare |[|
//@[20:22] NewLine |\r\n|
        {
//@[8:9] LeftBrace |{|
//@[9:11] NewLine |\r\n|
          name: 'CosmosDb:Account'
//@[10:14] Identifier |name|
//@[14:15] Colon |:|
//@[16:34] StringComplete |'CosmosDb:Account'|
//@[34:36] NewLine |\r\n|
          value: reference(cosmosDbResourceId).documentEndpoint
//@[10:15] Identifier |value|
//@[15:16] Colon |:|
//@[17:26] Identifier |reference|
//@[26:27] LeftParen |(|
//@[27:45] Identifier |cosmosDbResourceId|
//@[45:46] RightParen |)|
//@[46:47] Dot |.|
//@[47:63] Identifier |documentEndpoint|
//@[63:65] NewLine |\r\n|
        }
//@[8:9] RightBrace |}|
//@[9:11] NewLine |\r\n|
        {
//@[8:9] LeftBrace |{|
//@[9:11] NewLine |\r\n|
          name: 'CosmosDb:Key'
//@[10:14] Identifier |name|
//@[14:15] Colon |:|
//@[16:30] StringComplete |'CosmosDb:Key'|
//@[30:32] NewLine |\r\n|
          value: listKeys(cosmosDbResourceId, '2020-04-01').primaryMasterKey
//@[10:15] Identifier |value|
//@[15:16] Colon |:|
//@[17:25] Identifier |listKeys|
//@[25:26] LeftParen |(|
//@[26:44] Identifier |cosmosDbResourceId|
//@[44:45] Comma |,|
//@[46:58] StringComplete |'2020-04-01'|
//@[58:59] RightParen |)|
//@[59:60] Dot |.|
//@[60:76] Identifier |primaryMasterKey|
//@[76:78] NewLine |\r\n|
        }
//@[8:9] RightBrace |}|
//@[9:11] NewLine |\r\n|
        {
//@[8:9] LeftBrace |{|
//@[9:11] NewLine |\r\n|
          name: 'CosmosDb:DatabaseName'
//@[10:14] Identifier |name|
//@[14:15] Colon |:|
//@[16:39] StringComplete |'CosmosDb:DatabaseName'|
//@[39:41] NewLine |\r\n|
          value: cosmosDb.databaseName
//@[10:15] Identifier |value|
//@[15:16] Colon |:|
//@[17:25] Identifier |cosmosDb|
//@[25:26] Dot |.|
//@[26:38] Identifier |databaseName|
//@[38:40] NewLine |\r\n|
        }
//@[8:9] RightBrace |}|
//@[9:11] NewLine |\r\n|
        {
//@[8:9] LeftBrace |{|
//@[9:11] NewLine |\r\n|
          name: 'CosmosDb:ContainerName'
//@[10:14] Identifier |name|
//@[14:15] Colon |:|
//@[16:40] StringComplete |'CosmosDb:ContainerName'|
//@[40:42] NewLine |\r\n|
          value: cosmosDb.containerName
//@[10:15] Identifier |value|
//@[15:16] Colon |:|
//@[17:25] Identifier |cosmosDb|
//@[25:26] Dot |.|
//@[26:39] Identifier |containerName|
//@[39:41] NewLine |\r\n|
        }
//@[8:9] RightBrace |}|
//@[9:11] NewLine |\r\n|
      ]
//@[6:7] RightSquare |]|
//@[7:9] NewLine |\r\n|
    }
//@[4:5] RightBrace |}|
//@[5:7] NewLine |\r\n|
  }
//@[2:3] RightBrace |}|
//@[3:5] NewLine |\r\n|
}
//@[0:1] RightBrace |}|
//@[1:1] EndOfFile ||
