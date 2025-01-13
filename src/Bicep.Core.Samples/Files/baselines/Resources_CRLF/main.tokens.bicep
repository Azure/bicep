
//@[000:002) NewLine |\r\n|
@sys.description('this is basicStorage')
//@[000:001) At |@|
//@[001:004) Identifier |sys|
//@[004:005) Dot |.|
//@[005:016) Identifier |description|
//@[016:017) LeftParen |(|
//@[017:039) StringComplete |'this is basicStorage'|
//@[039:040) RightParen |)|
//@[040:042) NewLine |\r\n|
resource basicStorage 'Microsoft.Storage/storageAccounts@2019-06-01' = {
//@[000:008) Identifier |resource|
//@[009:021) Identifier |basicStorage|
//@[022:068) StringComplete |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[069:070) Assignment |=|
//@[071:072) LeftBrace |{|
//@[072:074) NewLine |\r\n|
  name: 'basicblobs'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:020) StringComplete |'basicblobs'|
//@[020:022) NewLine |\r\n|
  location: 'westus'
//@[002:010) Identifier |location|
//@[010:011) Colon |:|
//@[012:020) StringComplete |'westus'|
//@[020:022) NewLine |\r\n|
  kind: 'BlobStorage'
//@[002:006) Identifier |kind|
//@[006:007) Colon |:|
//@[008:021) StringComplete |'BlobStorage'|
//@[021:023) NewLine |\r\n|
  sku: {
//@[002:005) Identifier |sku|
//@[005:006) Colon |:|
//@[007:008) LeftBrace |{|
//@[008:010) NewLine |\r\n|
    name: 'Standard_GRS'
//@[004:008) Identifier |name|
//@[008:009) Colon |:|
//@[010:024) StringComplete |'Standard_GRS'|
//@[024:026) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

@sys.description('this is dnsZone')
//@[000:001) At |@|
//@[001:004) Identifier |sys|
//@[004:005) Dot |.|
//@[005:016) Identifier |description|
//@[016:017) LeftParen |(|
//@[017:034) StringComplete |'this is dnsZone'|
//@[034:035) RightParen |)|
//@[035:037) NewLine |\r\n|
resource dnsZone 'Microsoft.Network/dnszones@2018-05-01' = {
//@[000:008) Identifier |resource|
//@[009:016) Identifier |dnsZone|
//@[017:056) StringComplete |'Microsoft.Network/dnszones@2018-05-01'|
//@[057:058) Assignment |=|
//@[059:060) LeftBrace |{|
//@[060:062) NewLine |\r\n|
  name: 'myZone'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:016) StringComplete |'myZone'|
//@[016:018) NewLine |\r\n|
  location: 'global'
//@[002:010) Identifier |location|
//@[010:011) Colon |:|
//@[012:020) StringComplete |'global'|
//@[020:022) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

resource myStorageAccount 'Microsoft.Storage/storageAccounts@2017-10-01' = {
//@[000:008) Identifier |resource|
//@[009:025) Identifier |myStorageAccount|
//@[026:072) StringComplete |'Microsoft.Storage/storageAccounts@2017-10-01'|
//@[073:074) Assignment |=|
//@[075:076) LeftBrace |{|
//@[076:078) NewLine |\r\n|
  name: 'myencryptedone'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:024) StringComplete |'myencryptedone'|
//@[024:026) NewLine |\r\n|
  location: 'eastus2'
//@[002:010) Identifier |location|
//@[010:011) Colon |:|
//@[012:021) StringComplete |'eastus2'|
//@[021:023) NewLine |\r\n|
  properties: {
//@[002:012) Identifier |properties|
//@[012:013) Colon |:|
//@[014:015) LeftBrace |{|
//@[015:017) NewLine |\r\n|
    supportsHttpsTrafficOnly: true
//@[004:028) Identifier |supportsHttpsTrafficOnly|
//@[028:029) Colon |:|
//@[030:034) TrueKeyword |true|
//@[034:036) NewLine |\r\n|
    accessTier: 'Hot'
//@[004:014) Identifier |accessTier|
//@[014:015) Colon |:|
//@[016:021) StringComplete |'Hot'|
//@[021:023) NewLine |\r\n|
    encryption: {
//@[004:014) Identifier |encryption|
//@[014:015) Colon |:|
//@[016:017) LeftBrace |{|
//@[017:019) NewLine |\r\n|
      keySource: 'Microsoft.Storage'
//@[006:015) Identifier |keySource|
//@[015:016) Colon |:|
//@[017:036) StringComplete |'Microsoft.Storage'|
//@[036:038) NewLine |\r\n|
      services: {
//@[006:014) Identifier |services|
//@[014:015) Colon |:|
//@[016:017) LeftBrace |{|
//@[017:019) NewLine |\r\n|
        blob: {
//@[008:012) Identifier |blob|
//@[012:013) Colon |:|
//@[014:015) LeftBrace |{|
//@[015:017) NewLine |\r\n|
          enabled: true
//@[010:017) Identifier |enabled|
//@[017:018) Colon |:|
//@[019:023) TrueKeyword |true|
//@[023:025) NewLine |\r\n|
        }
//@[008:009) RightBrace |}|
//@[009:011) NewLine |\r\n|
        file: {
//@[008:012) Identifier |file|
//@[012:013) Colon |:|
//@[014:015) LeftBrace |{|
//@[015:017) NewLine |\r\n|
          enabled: true
//@[010:017) Identifier |enabled|
//@[017:018) Colon |:|
//@[019:023) TrueKeyword |true|
//@[023:025) NewLine |\r\n|
        }
//@[008:009) RightBrace |}|
//@[009:011) NewLine |\r\n|
      }
//@[006:007) RightBrace |}|
//@[007:009) NewLine |\r\n|
    }
//@[004:005) RightBrace |}|
//@[005:007) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
  kind: 'StorageV2'
//@[002:006) Identifier |kind|
//@[006:007) Colon |:|
//@[008:019) StringComplete |'StorageV2'|
//@[019:021) NewLine |\r\n|
  sku: {
//@[002:005) Identifier |sku|
//@[005:006) Colon |:|
//@[007:008) LeftBrace |{|
//@[008:010) NewLine |\r\n|
    name: 'Standard_LRS'
//@[004:008) Identifier |name|
//@[008:009) Colon |:|
//@[010:024) StringComplete |'Standard_LRS'|
//@[024:026) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

resource withExpressions 'Microsoft.Storage/storageAccounts@2017-10-01' = {
//@[000:008) Identifier |resource|
//@[009:024) Identifier |withExpressions|
//@[025:071) StringComplete |'Microsoft.Storage/storageAccounts@2017-10-01'|
//@[072:073) Assignment |=|
//@[074:075) LeftBrace |{|
//@[075:077) NewLine |\r\n|
  name: 'myencryptedone2'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:025) StringComplete |'myencryptedone2'|
//@[025:027) NewLine |\r\n|
  location: 'eastus2'
//@[002:010) Identifier |location|
//@[010:011) Colon |:|
//@[012:021) StringComplete |'eastus2'|
//@[021:023) NewLine |\r\n|
  properties: {
//@[002:012) Identifier |properties|
//@[012:013) Colon |:|
//@[014:015) LeftBrace |{|
//@[015:017) NewLine |\r\n|
    supportsHttpsTrafficOnly: !false
//@[004:028) Identifier |supportsHttpsTrafficOnly|
//@[028:029) Colon |:|
//@[030:031) Exclamation |!|
//@[031:036) FalseKeyword |false|
//@[036:038) NewLine |\r\n|
    accessTier: true ? 'Hot' : 'Cold'
//@[004:014) Identifier |accessTier|
//@[014:015) Colon |:|
//@[016:020) TrueKeyword |true|
//@[021:022) Question |?|
//@[023:028) StringComplete |'Hot'|
//@[029:030) Colon |:|
//@[031:037) StringComplete |'Cold'|
//@[037:039) NewLine |\r\n|
    encryption: {
//@[004:014) Identifier |encryption|
//@[014:015) Colon |:|
//@[016:017) LeftBrace |{|
//@[017:019) NewLine |\r\n|
      keySource: 'Microsoft.Storage'
//@[006:015) Identifier |keySource|
//@[015:016) Colon |:|
//@[017:036) StringComplete |'Microsoft.Storage'|
//@[036:038) NewLine |\r\n|
      services: {
//@[006:014) Identifier |services|
//@[014:015) Colon |:|
//@[016:017) LeftBrace |{|
//@[017:019) NewLine |\r\n|
        blob: {
//@[008:012) Identifier |blob|
//@[012:013) Colon |:|
//@[014:015) LeftBrace |{|
//@[015:017) NewLine |\r\n|
          enabled: true || false
//@[010:017) Identifier |enabled|
//@[017:018) Colon |:|
//@[019:023) TrueKeyword |true|
//@[024:026) LogicalOr ||||
//@[027:032) FalseKeyword |false|
//@[032:034) NewLine |\r\n|
        }
//@[008:009) RightBrace |}|
//@[009:011) NewLine |\r\n|
        file: {
//@[008:012) Identifier |file|
//@[012:013) Colon |:|
//@[014:015) LeftBrace |{|
//@[015:017) NewLine |\r\n|
          enabled: true
//@[010:017) Identifier |enabled|
//@[017:018) Colon |:|
//@[019:023) TrueKeyword |true|
//@[023:025) NewLine |\r\n|
        }
//@[008:009) RightBrace |}|
//@[009:011) NewLine |\r\n|
      }
//@[006:007) RightBrace |}|
//@[007:009) NewLine |\r\n|
    }
//@[004:005) RightBrace |}|
//@[005:007) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
  kind: 'StorageV2'
//@[002:006) Identifier |kind|
//@[006:007) Colon |:|
//@[008:019) StringComplete |'StorageV2'|
//@[019:021) NewLine |\r\n|
  sku: {
//@[002:005) Identifier |sku|
//@[005:006) Colon |:|
//@[007:008) LeftBrace |{|
//@[008:010) NewLine |\r\n|
    name: 'Standard_LRS'
//@[004:008) Identifier |name|
//@[008:009) Colon |:|
//@[010:024) StringComplete |'Standard_LRS'|
//@[024:026) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
  dependsOn: [
//@[002:011) Identifier |dependsOn|
//@[011:012) Colon |:|
//@[013:014) LeftSquare |[|
//@[014:016) NewLine |\r\n|
    myStorageAccount
//@[004:020) Identifier |myStorageAccount|
//@[020:022) NewLine |\r\n|
  ]
//@[002:003) RightSquare |]|
//@[003:005) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

param applicationName string = 'to-do-app${uniqueString(resourceGroup().id)}'
//@[000:005) Identifier |param|
//@[006:021) Identifier |applicationName|
//@[022:028) Identifier |string|
//@[029:030) Assignment |=|
//@[031:043) StringLeftPiece |'to-do-app${|
//@[043:055) Identifier |uniqueString|
//@[055:056) LeftParen |(|
//@[056:069) Identifier |resourceGroup|
//@[069:070) LeftParen |(|
//@[070:071) RightParen |)|
//@[071:072) Dot |.|
//@[072:074) Identifier |id|
//@[074:075) RightParen |)|
//@[075:077) StringRightPiece |}'|
//@[077:079) NewLine |\r\n|
var hostingPlanName = applicationName // why not just use the param directly?
//@[000:003) Identifier |var|
//@[004:019) Identifier |hostingPlanName|
//@[020:021) Assignment |=|
//@[022:037) Identifier |applicationName|
//@[077:081) NewLine |\r\n\r\n|

param appServicePlanTier string
//@[000:005) Identifier |param|
//@[006:024) Identifier |appServicePlanTier|
//@[025:031) Identifier |string|
//@[031:033) NewLine |\r\n|
param appServicePlanInstances int
//@[000:005) Identifier |param|
//@[006:029) Identifier |appServicePlanInstances|
//@[030:033) Identifier |int|
//@[033:037) NewLine |\r\n\r\n|

var location = resourceGroup().location
//@[000:003) Identifier |var|
//@[004:012) Identifier |location|
//@[013:014) Assignment |=|
//@[015:028) Identifier |resourceGroup|
//@[028:029) LeftParen |(|
//@[029:030) RightParen |)|
//@[030:031) Dot |.|
//@[031:039) Identifier |location|
//@[039:043) NewLine |\r\n\r\n|

resource farm 'Microsoft.Web/serverFarms@2019-08-01' = {
//@[000:008) Identifier |resource|
//@[009:013) Identifier |farm|
//@[014:052) StringComplete |'Microsoft.Web/serverFarms@2019-08-01'|
//@[053:054) Assignment |=|
//@[055:056) LeftBrace |{|
//@[056:058) NewLine |\r\n|
  // dependsOn: resourceId('Microsoft.DocumentDB/databaseAccounts', cosmosAccountName)
//@[086:088) NewLine |\r\n|
  name: hostingPlanName
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:023) Identifier |hostingPlanName|
//@[023:025) NewLine |\r\n|
  location: location
//@[002:010) Identifier |location|
//@[010:011) Colon |:|
//@[012:020) Identifier |location|
//@[020:022) NewLine |\r\n|
  sku: {
//@[002:005) Identifier |sku|
//@[005:006) Colon |:|
//@[007:008) LeftBrace |{|
//@[008:010) NewLine |\r\n|
    name: appServicePlanTier
//@[004:008) Identifier |name|
//@[008:009) Colon |:|
//@[010:028) Identifier |appServicePlanTier|
//@[028:030) NewLine |\r\n|
    capacity: appServicePlanInstances
//@[004:012) Identifier |capacity|
//@[012:013) Colon |:|
//@[014:037) Identifier |appServicePlanInstances|
//@[037:039) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
  properties: {
//@[002:012) Identifier |properties|
//@[012:013) Colon |:|
//@[014:015) LeftBrace |{|
//@[015:017) NewLine |\r\n|
    name: hostingPlanName // just hostingPlanName results in an error
//@[004:008) Identifier |name|
//@[008:009) Colon |:|
//@[010:025) Identifier |hostingPlanName|
//@[069:071) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

var cosmosDbResourceId = resourceId('Microsoft.DocumentDB/databaseAccounts',
//@[000:003) Identifier |var|
//@[004:022) Identifier |cosmosDbResourceId|
//@[023:024) Assignment |=|
//@[025:035) Identifier |resourceId|
//@[035:036) LeftParen |(|
//@[036:075) StringComplete |'Microsoft.DocumentDB/databaseAccounts'|
//@[075:076) Comma |,|
//@[076:078) NewLine |\r\n|
// comment
//@[010:012) NewLine |\r\n|
cosmosDb.account)
//@[000:008) Identifier |cosmosDb|
//@[008:009) Dot |.|
//@[009:016) Identifier |account|
//@[016:017) RightParen |)|
//@[017:019) NewLine |\r\n|
var cosmosDbRef = reference(cosmosDbResourceId).documentEndpoint
//@[000:003) Identifier |var|
//@[004:015) Identifier |cosmosDbRef|
//@[016:017) Assignment |=|
//@[018:027) Identifier |reference|
//@[027:028) LeftParen |(|
//@[028:046) Identifier |cosmosDbResourceId|
//@[046:047) RightParen |)|
//@[047:048) Dot |.|
//@[048:064) Identifier |documentEndpoint|
//@[064:068) NewLine |\r\n\r\n|

// this variable is not accessed anywhere in this template and depends on a run-time reference
//@[094:096) NewLine |\r\n|
// it should not be present at all in the template output as there is nowhere logical to put it
//@[095:097) NewLine |\r\n|
var cosmosDbEndpoint = cosmosDbRef.documentEndpoint
//@[000:003) Identifier |var|
//@[004:020) Identifier |cosmosDbEndpoint|
//@[021:022) Assignment |=|
//@[023:034) Identifier |cosmosDbRef|
//@[034:035) Dot |.|
//@[035:051) Identifier |documentEndpoint|
//@[051:055) NewLine |\r\n\r\n|

param webSiteName string
//@[000:005) Identifier |param|
//@[006:017) Identifier |webSiteName|
//@[018:024) Identifier |string|
//@[024:026) NewLine |\r\n|
param cosmosDb object
//@[000:005) Identifier |param|
//@[006:014) Identifier |cosmosDb|
//@[015:021) Identifier |object|
//@[021:023) NewLine |\r\n|
resource site 'Microsoft.Web/sites@2019-08-01' = {
//@[000:008) Identifier |resource|
//@[009:013) Identifier |site|
//@[014:046) StringComplete |'Microsoft.Web/sites@2019-08-01'|
//@[047:048) Assignment |=|
//@[049:050) LeftBrace |{|
//@[050:052) NewLine |\r\n|
  name: webSiteName
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:019) Identifier |webSiteName|
//@[019:021) NewLine |\r\n|
  location: location
//@[002:010) Identifier |location|
//@[010:011) Colon |:|
//@[012:020) Identifier |location|
//@[020:022) NewLine |\r\n|
  properties: {
//@[002:012) Identifier |properties|
//@[012:013) Colon |:|
//@[014:015) LeftBrace |{|
//@[015:017) NewLine |\r\n|
    // not yet supported // serverFarmId: farm.id
//@[049:051) NewLine |\r\n|
    siteConfig: {
//@[004:014) Identifier |siteConfig|
//@[014:015) Colon |:|
//@[016:017) LeftBrace |{|
//@[017:019) NewLine |\r\n|
      appSettings: [
//@[006:017) Identifier |appSettings|
//@[017:018) Colon |:|
//@[019:020) LeftSquare |[|
//@[020:022) NewLine |\r\n|
        {
//@[008:009) LeftBrace |{|
//@[009:011) NewLine |\r\n|
          name: 'CosmosDb:Account'
//@[010:014) Identifier |name|
//@[014:015) Colon |:|
//@[016:034) StringComplete |'CosmosDb:Account'|
//@[034:036) NewLine |\r\n|
          value: reference(cosmosDbResourceId).documentEndpoint
//@[010:015) Identifier |value|
//@[015:016) Colon |:|
//@[017:026) Identifier |reference|
//@[026:027) LeftParen |(|
//@[027:045) Identifier |cosmosDbResourceId|
//@[045:046) RightParen |)|
//@[046:047) Dot |.|
//@[047:063) Identifier |documentEndpoint|
//@[063:065) NewLine |\r\n|
        }
//@[008:009) RightBrace |}|
//@[009:011) NewLine |\r\n|
        {
//@[008:009) LeftBrace |{|
//@[009:011) NewLine |\r\n|
          name: 'CosmosDb:Key'
//@[010:014) Identifier |name|
//@[014:015) Colon |:|
//@[016:030) StringComplete |'CosmosDb:Key'|
//@[030:032) NewLine |\r\n|
          value: listKeys(cosmosDbResourceId, '2020-04-01').primaryMasterKey
//@[010:015) Identifier |value|
//@[015:016) Colon |:|
//@[017:025) Identifier |listKeys|
//@[025:026) LeftParen |(|
//@[026:044) Identifier |cosmosDbResourceId|
//@[044:045) Comma |,|
//@[046:058) StringComplete |'2020-04-01'|
//@[058:059) RightParen |)|
//@[059:060) Dot |.|
//@[060:076) Identifier |primaryMasterKey|
//@[076:078) NewLine |\r\n|
        }
//@[008:009) RightBrace |}|
//@[009:011) NewLine |\r\n|
        {
//@[008:009) LeftBrace |{|
//@[009:011) NewLine |\r\n|
          name: 'CosmosDb:DatabaseName'
//@[010:014) Identifier |name|
//@[014:015) Colon |:|
//@[016:039) StringComplete |'CosmosDb:DatabaseName'|
//@[039:041) NewLine |\r\n|
          value: cosmosDb.databaseName
//@[010:015) Identifier |value|
//@[015:016) Colon |:|
//@[017:025) Identifier |cosmosDb|
//@[025:026) Dot |.|
//@[026:038) Identifier |databaseName|
//@[038:040) NewLine |\r\n|
        }
//@[008:009) RightBrace |}|
//@[009:011) NewLine |\r\n|
        {
//@[008:009) LeftBrace |{|
//@[009:011) NewLine |\r\n|
          name: 'CosmosDb:ContainerName'
//@[010:014) Identifier |name|
//@[014:015) Colon |:|
//@[016:040) StringComplete |'CosmosDb:ContainerName'|
//@[040:042) NewLine |\r\n|
          value: cosmosDb.containerName
//@[010:015) Identifier |value|
//@[015:016) Colon |:|
//@[017:025) Identifier |cosmosDb|
//@[025:026) Dot |.|
//@[026:039) Identifier |containerName|
//@[039:041) NewLine |\r\n|
        }
//@[008:009) RightBrace |}|
//@[009:011) NewLine |\r\n|
      ]
//@[006:007) RightSquare |]|
//@[007:009) NewLine |\r\n|
    }
//@[004:005) RightBrace |}|
//@[005:007) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

var _siteApiVersion = site.apiVersion
//@[000:003) Identifier |var|
//@[004:019) Identifier |_siteApiVersion|
//@[020:021) Assignment |=|
//@[022:026) Identifier |site|
//@[026:027) Dot |.|
//@[027:037) Identifier |apiVersion|
//@[037:039) NewLine |\r\n|
var _siteType = site.type
//@[000:003) Identifier |var|
//@[004:013) Identifier |_siteType|
//@[014:015) Assignment |=|
//@[016:020) Identifier |site|
//@[020:021) Dot |.|
//@[021:025) Identifier |type|
//@[025:029) NewLine |\r\n\r\n|

output siteApiVersion string = site.apiVersion
//@[000:006) Identifier |output|
//@[007:021) Identifier |siteApiVersion|
//@[022:028) Identifier |string|
//@[029:030) Assignment |=|
//@[031:035) Identifier |site|
//@[035:036) Dot |.|
//@[036:046) Identifier |apiVersion|
//@[046:048) NewLine |\r\n|
output siteType string = site.type
//@[000:006) Identifier |output|
//@[007:015) Identifier |siteType|
//@[016:022) Identifier |string|
//@[023:024) Assignment |=|
//@[025:029) Identifier |site|
//@[029:030) Dot |.|
//@[030:034) Identifier |type|
//@[034:038) NewLine |\r\n\r\n|

resource nested 'Microsoft.Resources/deployments@2019-10-01' = {
//@[000:008) Identifier |resource|
//@[009:015) Identifier |nested|
//@[016:060) StringComplete |'Microsoft.Resources/deployments@2019-10-01'|
//@[061:062) Assignment |=|
//@[063:064) LeftBrace |{|
//@[064:066) NewLine |\r\n|
  name: 'nestedTemplate1'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:025) StringComplete |'nestedTemplate1'|
//@[025:027) NewLine |\r\n|
  properties: {
//@[002:012) Identifier |properties|
//@[012:013) Colon |:|
//@[014:015) LeftBrace |{|
//@[015:017) NewLine |\r\n|
    mode: 'Incremental'
//@[004:008) Identifier |mode|
//@[008:009) Colon |:|
//@[010:023) StringComplete |'Incremental'|
//@[023:025) NewLine |\r\n|
    template: {
//@[004:012) Identifier |template|
//@[012:013) Colon |:|
//@[014:015) LeftBrace |{|
//@[015:017) NewLine |\r\n|
      // string key value
//@[025:027) NewLine |\r\n|
      '$schema': 'https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#'
//@[006:015) StringComplete |'$schema'|
//@[015:016) Colon |:|
//@[017:098) StringComplete |'https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#'|
//@[098:100) NewLine |\r\n|
      contentVersion: '1.0.0.0'
//@[006:020) Identifier |contentVersion|
//@[020:021) Colon |:|
//@[022:031) StringComplete |'1.0.0.0'|
//@[031:033) NewLine |\r\n|
      resources: [
//@[006:015) Identifier |resources|
//@[015:016) Colon |:|
//@[017:018) LeftSquare |[|
//@[018:020) NewLine |\r\n|
      ]
//@[006:007) RightSquare |]|
//@[007:009) NewLine |\r\n|
    }
//@[004:005) RightBrace |}|
//@[005:007) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

// should be able to access the read only properties
//@[052:054) NewLine |\r\n|
resource accessingReadOnlyProperties 'Microsoft.Foo/foos@2019-10-01' = {
//@[000:008) Identifier |resource|
//@[009:036) Identifier |accessingReadOnlyProperties|
//@[037:068) StringComplete |'Microsoft.Foo/foos@2019-10-01'|
//@[069:070) Assignment |=|
//@[071:072) LeftBrace |{|
//@[072:074) NewLine |\r\n|
  name: 'nestedTemplate1'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:025) StringComplete |'nestedTemplate1'|
//@[025:027) NewLine |\r\n|
  properties: {
//@[002:012) Identifier |properties|
//@[012:013) Colon |:|
//@[014:015) LeftBrace |{|
//@[015:017) NewLine |\r\n|
    otherId: nested.id
//@[004:011) Identifier |otherId|
//@[011:012) Colon |:|
//@[013:019) Identifier |nested|
//@[019:020) Dot |.|
//@[020:022) Identifier |id|
//@[022:024) NewLine |\r\n|
    otherName: nested.name
//@[004:013) Identifier |otherName|
//@[013:014) Colon |:|
//@[015:021) Identifier |nested|
//@[021:022) Dot |.|
//@[022:026) Identifier |name|
//@[026:028) NewLine |\r\n|
    otherVersion: nested.apiVersion
//@[004:016) Identifier |otherVersion|
//@[016:017) Colon |:|
//@[018:024) Identifier |nested|
//@[024:025) Dot |.|
//@[025:035) Identifier |apiVersion|
//@[035:037) NewLine |\r\n|
    otherType: nested.type
//@[004:013) Identifier |otherType|
//@[013:014) Colon |:|
//@[015:021) Identifier |nested|
//@[021:022) Dot |.|
//@[022:026) Identifier |type|
//@[026:030) NewLine |\r\n\r\n|

    otherThings: nested.properties.mode
//@[004:015) Identifier |otherThings|
//@[015:016) Colon |:|
//@[017:023) Identifier |nested|
//@[023:024) Dot |.|
//@[024:034) Identifier |properties|
//@[034:035) Dot |.|
//@[035:039) Identifier |mode|
//@[039:041) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

resource resourceA 'My.Rp/typeA@2020-01-01' = {
//@[000:008) Identifier |resource|
//@[009:018) Identifier |resourceA|
//@[019:043) StringComplete |'My.Rp/typeA@2020-01-01'|
//@[044:045) Assignment |=|
//@[046:047) LeftBrace |{|
//@[047:049) NewLine |\r\n|
  name: 'resourceA'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:019) StringComplete |'resourceA'|
//@[019:021) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

resource resourceB 'My.Rp/typeA/typeB@2020-01-01' = {
//@[000:008) Identifier |resource|
//@[009:018) Identifier |resourceB|
//@[019:049) StringComplete |'My.Rp/typeA/typeB@2020-01-01'|
//@[050:051) Assignment |=|
//@[052:053) LeftBrace |{|
//@[053:055) NewLine |\r\n|
  name: '${resourceA.name}/resourceB'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:011) StringLeftPiece |'${|
//@[011:020) Identifier |resourceA|
//@[020:021) Dot |.|
//@[021:025) Identifier |name|
//@[025:037) StringRightPiece |}/resourceB'|
//@[037:039) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

resource resourceC 'My.Rp/typeA/typeB@2020-01-01' = {
//@[000:008) Identifier |resource|
//@[009:018) Identifier |resourceC|
//@[019:049) StringComplete |'My.Rp/typeA/typeB@2020-01-01'|
//@[050:051) Assignment |=|
//@[052:053) LeftBrace |{|
//@[053:055) NewLine |\r\n|
  name: '${resourceA.name}/resourceC'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:011) StringLeftPiece |'${|
//@[011:020) Identifier |resourceA|
//@[020:021) Dot |.|
//@[021:025) Identifier |name|
//@[025:037) StringRightPiece |}/resourceC'|
//@[037:039) NewLine |\r\n|
  properties: {
//@[002:012) Identifier |properties|
//@[012:013) Colon |:|
//@[014:015) LeftBrace |{|
//@[015:017) NewLine |\r\n|
    aId: resourceA.id
//@[004:007) Identifier |aId|
//@[007:008) Colon |:|
//@[009:018) Identifier |resourceA|
//@[018:019) Dot |.|
//@[019:021) Identifier |id|
//@[021:023) NewLine |\r\n|
    aType: resourceA.type
//@[004:009) Identifier |aType|
//@[009:010) Colon |:|
//@[011:020) Identifier |resourceA|
//@[020:021) Dot |.|
//@[021:025) Identifier |type|
//@[025:027) NewLine |\r\n|
    aName: resourceA.name
//@[004:009) Identifier |aName|
//@[009:010) Colon |:|
//@[011:020) Identifier |resourceA|
//@[020:021) Dot |.|
//@[021:025) Identifier |name|
//@[025:027) NewLine |\r\n|
    aApiVersion: resourceA.apiVersion
//@[004:015) Identifier |aApiVersion|
//@[015:016) Colon |:|
//@[017:026) Identifier |resourceA|
//@[026:027) Dot |.|
//@[027:037) Identifier |apiVersion|
//@[037:039) NewLine |\r\n|
    bProperties: resourceB.properties
//@[004:015) Identifier |bProperties|
//@[015:016) Colon |:|
//@[017:026) Identifier |resourceB|
//@[026:027) Dot |.|
//@[027:037) Identifier |properties|
//@[037:039) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

var varARuntime = {
//@[000:003) Identifier |var|
//@[004:015) Identifier |varARuntime|
//@[016:017) Assignment |=|
//@[018:019) LeftBrace |{|
//@[019:021) NewLine |\r\n|
  bId: resourceB.id
//@[002:005) Identifier |bId|
//@[005:006) Colon |:|
//@[007:016) Identifier |resourceB|
//@[016:017) Dot |.|
//@[017:019) Identifier |id|
//@[019:021) NewLine |\r\n|
  bType: resourceB.type
//@[002:007) Identifier |bType|
//@[007:008) Colon |:|
//@[009:018) Identifier |resourceB|
//@[018:019) Dot |.|
//@[019:023) Identifier |type|
//@[023:025) NewLine |\r\n|
  bName: resourceB.name
//@[002:007) Identifier |bName|
//@[007:008) Colon |:|
//@[009:018) Identifier |resourceB|
//@[018:019) Dot |.|
//@[019:023) Identifier |name|
//@[023:025) NewLine |\r\n|
  bApiVersion: resourceB.apiVersion
//@[002:013) Identifier |bApiVersion|
//@[013:014) Colon |:|
//@[015:024) Identifier |resourceB|
//@[024:025) Dot |.|
//@[025:035) Identifier |apiVersion|
//@[035:037) NewLine |\r\n|
  aKind: resourceA.kind
//@[002:007) Identifier |aKind|
//@[007:008) Colon |:|
//@[009:018) Identifier |resourceA|
//@[018:019) Dot |.|
//@[019:023) Identifier |kind|
//@[023:025) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

var varBRuntime = [
//@[000:003) Identifier |var|
//@[004:015) Identifier |varBRuntime|
//@[016:017) Assignment |=|
//@[018:019) LeftSquare |[|
//@[019:021) NewLine |\r\n|
  varARuntime
//@[002:013) Identifier |varARuntime|
//@[013:015) NewLine |\r\n|
]
//@[000:001) RightSquare |]|
//@[001:005) NewLine |\r\n\r\n|

var resourceCRef = {
//@[000:003) Identifier |var|
//@[004:016) Identifier |resourceCRef|
//@[017:018) Assignment |=|
//@[019:020) LeftBrace |{|
//@[020:022) NewLine |\r\n|
  id: resourceC.id
//@[002:004) Identifier |id|
//@[004:005) Colon |:|
//@[006:015) Identifier |resourceC|
//@[015:016) Dot |.|
//@[016:018) Identifier |id|
//@[018:020) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\r\n|
var setResourceCRef = true
//@[000:003) Identifier |var|
//@[004:019) Identifier |setResourceCRef|
//@[020:021) Assignment |=|
//@[022:026) TrueKeyword |true|
//@[026:030) NewLine |\r\n\r\n|

resource resourceD 'My.Rp/typeD@2020-01-01' = {
//@[000:008) Identifier |resource|
//@[009:018) Identifier |resourceD|
//@[019:043) StringComplete |'My.Rp/typeD@2020-01-01'|
//@[044:045) Assignment |=|
//@[046:047) LeftBrace |{|
//@[047:049) NewLine |\r\n|
  name: 'constant'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:018) StringComplete |'constant'|
//@[018:020) NewLine |\r\n|
  properties: {
//@[002:012) Identifier |properties|
//@[012:013) Colon |:|
//@[014:015) LeftBrace |{|
//@[015:017) NewLine |\r\n|
    runtime: varBRuntime
//@[004:011) Identifier |runtime|
//@[011:012) Colon |:|
//@[013:024) Identifier |varBRuntime|
//@[024:026) NewLine |\r\n|
    // repro for https://github.com/Azure/bicep/issues/316
//@[058:060) NewLine |\r\n|
    repro316: setResourceCRef ? resourceCRef : null
//@[004:012) Identifier |repro316|
//@[012:013) Colon |:|
//@[014:029) Identifier |setResourceCRef|
//@[030:031) Question |?|
//@[032:044) Identifier |resourceCRef|
//@[045:046) Colon |:|
//@[047:051) NullKeyword |null|
//@[051:053) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

var myInterpKey = 'abc'
//@[000:003) Identifier |var|
//@[004:015) Identifier |myInterpKey|
//@[016:017) Assignment |=|
//@[018:023) StringComplete |'abc'|
//@[023:025) NewLine |\r\n|
resource resourceWithInterp 'My.Rp/interp@2020-01-01' = {
//@[000:008) Identifier |resource|
//@[009:027) Identifier |resourceWithInterp|
//@[028:053) StringComplete |'My.Rp/interp@2020-01-01'|
//@[054:055) Assignment |=|
//@[056:057) LeftBrace |{|
//@[057:059) NewLine |\r\n|
  name: 'interpTest'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:020) StringComplete |'interpTest'|
//@[020:022) NewLine |\r\n|
  properties: {
//@[002:012) Identifier |properties|
//@[012:013) Colon |:|
//@[014:015) LeftBrace |{|
//@[015:017) NewLine |\r\n|
    '${myInterpKey}': 1
//@[004:007) StringLeftPiece |'${|
//@[007:018) Identifier |myInterpKey|
//@[018:020) StringRightPiece |}'|
//@[020:021) Colon |:|
//@[022:023) Integer |1|
//@[023:025) NewLine |\r\n|
    'abc${myInterpKey}def': 2
//@[004:010) StringLeftPiece |'abc${|
//@[010:021) Identifier |myInterpKey|
//@[021:026) StringRightPiece |}def'|
//@[026:027) Colon |:|
//@[028:029) Integer |2|
//@[029:031) NewLine |\r\n|
    '${myInterpKey}abc${myInterpKey}': 3
//@[004:007) StringLeftPiece |'${|
//@[007:018) Identifier |myInterpKey|
//@[018:024) StringMiddlePiece |}abc${|
//@[024:035) Identifier |myInterpKey|
//@[035:037) StringRightPiece |}'|
//@[037:038) Colon |:|
//@[039:040) Integer |3|
//@[040:042) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

resource resourceWithEscaping 'My.Rp/mockResource@2020-01-01' = {
//@[000:008) Identifier |resource|
//@[009:029) Identifier |resourceWithEscaping|
//@[030:061) StringComplete |'My.Rp/mockResource@2020-01-01'|
//@[062:063) Assignment |=|
//@[064:065) LeftBrace |{|
//@[065:067) NewLine |\r\n|
  name: 'test'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:014) StringComplete |'test'|
//@[014:016) NewLine |\r\n|
  properties: {
//@[002:012) Identifier |properties|
//@[012:013) Colon |:|
//@[014:015) LeftBrace |{|
//@[015:017) NewLine |\r\n|
    // both key and value should be escaped in template output
//@[062:064) NewLine |\r\n|
    '[resourceGroup().location]': '[resourceGroup().location]'
//@[004:032) StringComplete |'[resourceGroup().location]'|
//@[032:033) Colon |:|
//@[034:062) StringComplete |'[resourceGroup().location]'|
//@[062:064) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

param shouldDeployVm bool = true
//@[000:005) Identifier |param|
//@[006:020) Identifier |shouldDeployVm|
//@[021:025) Identifier |bool|
//@[026:027) Assignment |=|
//@[028:032) TrueKeyword |true|
//@[032:036) NewLine |\r\n\r\n|

@sys.description('this is vmWithCondition')
//@[000:001) At |@|
//@[001:004) Identifier |sys|
//@[004:005) Dot |.|
//@[005:016) Identifier |description|
//@[016:017) LeftParen |(|
//@[017:042) StringComplete |'this is vmWithCondition'|
//@[042:043) RightParen |)|
//@[043:045) NewLine |\r\n|
resource vmWithCondition 'Microsoft.Compute/virtualMachines@2020-06-01' = if (shouldDeployVm) {
//@[000:008) Identifier |resource|
//@[009:024) Identifier |vmWithCondition|
//@[025:071) StringComplete |'Microsoft.Compute/virtualMachines@2020-06-01'|
//@[072:073) Assignment |=|
//@[074:076) Identifier |if|
//@[077:078) LeftParen |(|
//@[078:092) Identifier |shouldDeployVm|
//@[092:093) RightParen |)|
//@[094:095) LeftBrace |{|
//@[095:097) NewLine |\r\n|
  name: 'vmName'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:016) StringComplete |'vmName'|
//@[016:018) NewLine |\r\n|
  location: 'westus'
//@[002:010) Identifier |location|
//@[010:011) Colon |:|
//@[012:020) StringComplete |'westus'|
//@[020:022) NewLine |\r\n|
  properties: {
//@[002:012) Identifier |properties|
//@[012:013) Colon |:|
//@[014:015) LeftBrace |{|
//@[015:017) NewLine |\r\n|
    osProfile: {
//@[004:013) Identifier |osProfile|
//@[013:014) Colon |:|
//@[015:016) LeftBrace |{|
//@[016:018) NewLine |\r\n|
      windowsConfiguration: {
//@[006:026) Identifier |windowsConfiguration|
//@[026:027) Colon |:|
//@[028:029) LeftBrace |{|
//@[029:031) NewLine |\r\n|
        enableAutomaticUpdates: true
//@[008:030) Identifier |enableAutomaticUpdates|
//@[030:031) Colon |:|
//@[032:036) TrueKeyword |true|
//@[036:038) NewLine |\r\n|
      }
//@[006:007) RightBrace |}|
//@[007:009) NewLine |\r\n|
    }
//@[004:005) RightBrace |}|
//@[005:007) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

@sys.description('this is another vmWithCondition')
//@[000:001) At |@|
//@[001:004) Identifier |sys|
//@[004:005) Dot |.|
//@[005:016) Identifier |description|
//@[016:017) LeftParen |(|
//@[017:050) StringComplete |'this is another vmWithCondition'|
//@[050:051) RightParen |)|
//@[051:053) NewLine |\r\n|
resource vmWithCondition2 'Microsoft.Compute/virtualMachines@2020-06-01' =
//@[000:008) Identifier |resource|
//@[009:025) Identifier |vmWithCondition2|
//@[026:072) StringComplete |'Microsoft.Compute/virtualMachines@2020-06-01'|
//@[073:074) Assignment |=|
//@[074:076) NewLine |\r\n|
                    if (shouldDeployVm) {
//@[020:022) Identifier |if|
//@[023:024) LeftParen |(|
//@[024:038) Identifier |shouldDeployVm|
//@[038:039) RightParen |)|
//@[040:041) LeftBrace |{|
//@[041:043) NewLine |\r\n|
  name: 'vmName2'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:017) StringComplete |'vmName2'|
//@[017:019) NewLine |\r\n|
  location: 'westus'
//@[002:010) Identifier |location|
//@[010:011) Colon |:|
//@[012:020) StringComplete |'westus'|
//@[020:022) NewLine |\r\n|
  properties: {
//@[002:012) Identifier |properties|
//@[012:013) Colon |:|
//@[014:015) LeftBrace |{|
//@[015:017) NewLine |\r\n|
    osProfile: {
//@[004:013) Identifier |osProfile|
//@[013:014) Colon |:|
//@[015:016) LeftBrace |{|
//@[016:018) NewLine |\r\n|
      windowsConfiguration: {
//@[006:026) Identifier |windowsConfiguration|
//@[026:027) Colon |:|
//@[028:029) LeftBrace |{|
//@[029:031) NewLine |\r\n|
        enableAutomaticUpdates: true
//@[008:030) Identifier |enableAutomaticUpdates|
//@[030:031) Colon |:|
//@[032:036) TrueKeyword |true|
//@[036:038) NewLine |\r\n|
      }
//@[006:007) RightBrace |}|
//@[007:009) NewLine |\r\n|
    }
//@[004:005) RightBrace |}|
//@[005:007) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

resource extension1 'My.Rp/extensionResource@2020-12-01' = {
//@[000:008) Identifier |resource|
//@[009:019) Identifier |extension1|
//@[020:056) StringComplete |'My.Rp/extensionResource@2020-12-01'|
//@[057:058) Assignment |=|
//@[059:060) LeftBrace |{|
//@[060:062) NewLine |\r\n|
  name: 'extension'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:019) StringComplete |'extension'|
//@[019:021) NewLine |\r\n|
  scope: vmWithCondition
//@[002:007) Identifier |scope|
//@[007:008) Colon |:|
//@[009:024) Identifier |vmWithCondition|
//@[024:026) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

resource extension2 'My.Rp/extensionResource@2020-12-01' = {
//@[000:008) Identifier |resource|
//@[009:019) Identifier |extension2|
//@[020:056) StringComplete |'My.Rp/extensionResource@2020-12-01'|
//@[057:058) Assignment |=|
//@[059:060) LeftBrace |{|
//@[060:062) NewLine |\r\n|
  name: 'extension'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:019) StringComplete |'extension'|
//@[019:021) NewLine |\r\n|
  scope: extension1
//@[002:007) Identifier |scope|
//@[007:008) Colon |:|
//@[009:019) Identifier |extension1|
//@[019:021) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

resource extensionDependencies 'My.Rp/mockResource@2020-01-01' = {
//@[000:008) Identifier |resource|
//@[009:030) Identifier |extensionDependencies|
//@[031:062) StringComplete |'My.Rp/mockResource@2020-01-01'|
//@[063:064) Assignment |=|
//@[065:066) LeftBrace |{|
//@[066:068) NewLine |\r\n|
  name: 'extensionDependencies'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:031) StringComplete |'extensionDependencies'|
//@[031:033) NewLine |\r\n|
  properties: {
//@[002:012) Identifier |properties|
//@[012:013) Colon |:|
//@[014:015) LeftBrace |{|
//@[015:017) NewLine |\r\n|
    res1: vmWithCondition.id
//@[004:008) Identifier |res1|
//@[008:009) Colon |:|
//@[010:025) Identifier |vmWithCondition|
//@[025:026) Dot |.|
//@[026:028) Identifier |id|
//@[028:030) NewLine |\r\n|
    res1runtime: vmWithCondition.properties.something
//@[004:015) Identifier |res1runtime|
//@[015:016) Colon |:|
//@[017:032) Identifier |vmWithCondition|
//@[032:033) Dot |.|
//@[033:043) Identifier |properties|
//@[043:044) Dot |.|
//@[044:053) Identifier |something|
//@[053:055) NewLine |\r\n|
    res2: extension1.id
//@[004:008) Identifier |res2|
//@[008:009) Colon |:|
//@[010:020) Identifier |extension1|
//@[020:021) Dot |.|
//@[021:023) Identifier |id|
//@[023:025) NewLine |\r\n|
    res2runtime: extension1.properties.something
//@[004:015) Identifier |res2runtime|
//@[015:016) Colon |:|
//@[017:027) Identifier |extension1|
//@[027:028) Dot |.|
//@[028:038) Identifier |properties|
//@[038:039) Dot |.|
//@[039:048) Identifier |something|
//@[048:050) NewLine |\r\n|
    res3: extension2.id
//@[004:008) Identifier |res3|
//@[008:009) Colon |:|
//@[010:020) Identifier |extension2|
//@[020:021) Dot |.|
//@[021:023) Identifier |id|
//@[023:025) NewLine |\r\n|
    res3runtime: extension2.properties.something
//@[004:015) Identifier |res3runtime|
//@[015:016) Colon |:|
//@[017:027) Identifier |extension2|
//@[027:028) Dot |.|
//@[028:038) Identifier |properties|
//@[038:039) Dot |.|
//@[039:048) Identifier |something|
//@[048:050) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

@sys.description('this is existing1')
//@[000:001) At |@|
//@[001:004) Identifier |sys|
//@[004:005) Dot |.|
//@[005:016) Identifier |description|
//@[016:017) LeftParen |(|
//@[017:036) StringComplete |'this is existing1'|
//@[036:037) RightParen |)|
//@[037:039) NewLine |\r\n|
resource existing1 'Mock.Rp/existingExtensionResource@2020-01-01' existing = {
//@[000:008) Identifier |resource|
//@[009:018) Identifier |existing1|
//@[019:065) StringComplete |'Mock.Rp/existingExtensionResource@2020-01-01'|
//@[066:074) Identifier |existing|
//@[075:076) Assignment |=|
//@[077:078) LeftBrace |{|
//@[078:080) NewLine |\r\n|
  name: 'existing1'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:019) StringComplete |'existing1'|
//@[019:021) NewLine |\r\n|
  scope: extension1
//@[002:007) Identifier |scope|
//@[007:008) Colon |:|
//@[009:019) Identifier |extension1|
//@[019:021) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

resource existing2 'Mock.Rp/existingExtensionResource@2020-01-01' existing = {
//@[000:008) Identifier |resource|
//@[009:018) Identifier |existing2|
//@[019:065) StringComplete |'Mock.Rp/existingExtensionResource@2020-01-01'|
//@[066:074) Identifier |existing|
//@[075:076) Assignment |=|
//@[077:078) LeftBrace |{|
//@[078:080) NewLine |\r\n|
  name: 'existing2'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:019) StringComplete |'existing2'|
//@[019:021) NewLine |\r\n|
  scope: existing1
//@[002:007) Identifier |scope|
//@[007:008) Colon |:|
//@[009:018) Identifier |existing1|
//@[018:020) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

resource extension3 'My.Rp/extensionResource@2020-12-01' = {
//@[000:008) Identifier |resource|
//@[009:019) Identifier |extension3|
//@[020:056) StringComplete |'My.Rp/extensionResource@2020-12-01'|
//@[057:058) Assignment |=|
//@[059:060) LeftBrace |{|
//@[060:062) NewLine |\r\n|
  name: 'extension3'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:020) StringComplete |'extension3'|
//@[020:022) NewLine |\r\n|
  scope: existing1
//@[002:007) Identifier |scope|
//@[007:008) Colon |:|
//@[009:018) Identifier |existing1|
//@[018:020) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

/*
  valid loop cases
*/
//@[002:004) NewLine |\r\n|
var storageAccounts = [
//@[000:003) Identifier |var|
//@[004:019) Identifier |storageAccounts|
//@[020:021) Assignment |=|
//@[022:023) LeftSquare |[|
//@[023:025) NewLine |\r\n|
  {
//@[002:003) LeftBrace |{|
//@[003:005) NewLine |\r\n|
    name: 'one'
//@[004:008) Identifier |name|
//@[008:009) Colon |:|
//@[010:015) StringComplete |'one'|
//@[015:017) NewLine |\r\n|
    location: 'eastus2'
//@[004:012) Identifier |location|
//@[012:013) Colon |:|
//@[014:023) StringComplete |'eastus2'|
//@[023:025) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
  {
//@[002:003) LeftBrace |{|
//@[003:005) NewLine |\r\n|
    name: 'two'
//@[004:008) Identifier |name|
//@[008:009) Colon |:|
//@[010:015) StringComplete |'two'|
//@[015:017) NewLine |\r\n|
    location: 'westus'
//@[004:012) Identifier |location|
//@[012:013) Colon |:|
//@[014:022) StringComplete |'westus'|
//@[022:024) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
]
//@[000:001) RightSquare |]|
//@[001:005) NewLine |\r\n\r\n|

// just a storage account loop
//@[030:032) NewLine |\r\n|
@sys.description('this is just a storage account loop')
//@[000:001) At |@|
//@[001:004) Identifier |sys|
//@[004:005) Dot |.|
//@[005:016) Identifier |description|
//@[016:017) LeftParen |(|
//@[017:054) StringComplete |'this is just a storage account loop'|
//@[054:055) RightParen |)|
//@[055:057) NewLine |\r\n|
resource storageResources 'Microsoft.Storage/storageAccounts@2019-06-01' = [for account in storageAccounts: {
//@[000:008) Identifier |resource|
//@[009:025) Identifier |storageResources|
//@[026:072) StringComplete |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[073:074) Assignment |=|
//@[075:076) LeftSquare |[|
//@[076:079) Identifier |for|
//@[080:087) Identifier |account|
//@[088:090) Identifier |in|
//@[091:106) Identifier |storageAccounts|
//@[106:107) Colon |:|
//@[108:109) LeftBrace |{|
//@[109:111) NewLine |\r\n|
  name: account.name
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:015) Identifier |account|
//@[015:016) Dot |.|
//@[016:020) Identifier |name|
//@[020:022) NewLine |\r\n|
  location: account.location
//@[002:010) Identifier |location|
//@[010:011) Colon |:|
//@[012:019) Identifier |account|
//@[019:020) Dot |.|
//@[020:028) Identifier |location|
//@[028:030) NewLine |\r\n|
  sku: {
//@[002:005) Identifier |sku|
//@[005:006) Colon |:|
//@[007:008) LeftBrace |{|
//@[008:010) NewLine |\r\n|
    name: 'Standard_LRS'
//@[004:008) Identifier |name|
//@[008:009) Colon |:|
//@[010:024) StringComplete |'Standard_LRS'|
//@[024:026) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
  kind: 'StorageV2'
//@[002:006) Identifier |kind|
//@[006:007) Colon |:|
//@[008:019) StringComplete |'StorageV2'|
//@[019:021) NewLine |\r\n|
}]
//@[000:001) RightBrace |}|
//@[001:002) RightSquare |]|
//@[002:006) NewLine |\r\n\r\n|

// storage account loop with index
//@[034:036) NewLine |\r\n|
@sys.description('this is just a storage account loop with index')
//@[000:001) At |@|
//@[001:004) Identifier |sys|
//@[004:005) Dot |.|
//@[005:016) Identifier |description|
//@[016:017) LeftParen |(|
//@[017:065) StringComplete |'this is just a storage account loop with index'|
//@[065:066) RightParen |)|
//@[066:068) NewLine |\r\n|
resource storageResourcesWithIndex 'Microsoft.Storage/storageAccounts@2019-06-01' = [for (account, i) in storageAccounts: {
//@[000:008) Identifier |resource|
//@[009:034) Identifier |storageResourcesWithIndex|
//@[035:081) StringComplete |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[082:083) Assignment |=|
//@[084:085) LeftSquare |[|
//@[085:088) Identifier |for|
//@[089:090) LeftParen |(|
//@[090:097) Identifier |account|
//@[097:098) Comma |,|
//@[099:100) Identifier |i|
//@[100:101) RightParen |)|
//@[102:104) Identifier |in|
//@[105:120) Identifier |storageAccounts|
//@[120:121) Colon |:|
//@[122:123) LeftBrace |{|
//@[123:125) NewLine |\r\n|
  name: '${account.name}${i}'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:011) StringLeftPiece |'${|
//@[011:018) Identifier |account|
//@[018:019) Dot |.|
//@[019:023) Identifier |name|
//@[023:026) StringMiddlePiece |}${|
//@[026:027) Identifier |i|
//@[027:029) StringRightPiece |}'|
//@[029:031) NewLine |\r\n|
  location: account.location
//@[002:010) Identifier |location|
//@[010:011) Colon |:|
//@[012:019) Identifier |account|
//@[019:020) Dot |.|
//@[020:028) Identifier |location|
//@[028:030) NewLine |\r\n|
  sku: {
//@[002:005) Identifier |sku|
//@[005:006) Colon |:|
//@[007:008) LeftBrace |{|
//@[008:010) NewLine |\r\n|
    name: 'Standard_LRS'
//@[004:008) Identifier |name|
//@[008:009) Colon |:|
//@[010:024) StringComplete |'Standard_LRS'|
//@[024:026) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
  kind: 'StorageV2'
//@[002:006) Identifier |kind|
//@[006:007) Colon |:|
//@[008:019) StringComplete |'StorageV2'|
//@[019:021) NewLine |\r\n|
}]
//@[000:001) RightBrace |}|
//@[001:002) RightSquare |]|
//@[002:006) NewLine |\r\n\r\n|

// basic nested loop
//@[020:022) NewLine |\r\n|
@sys.description('this is just a basic nested loop')
//@[000:001) At |@|
//@[001:004) Identifier |sys|
//@[004:005) Dot |.|
//@[005:016) Identifier |description|
//@[016:017) LeftParen |(|
//@[017:051) StringComplete |'this is just a basic nested loop'|
//@[051:052) RightParen |)|
//@[052:054) NewLine |\r\n|
resource vnet 'Microsoft.Network/virtualNetworks@2020-06-01' = [for i in range(0, 3): {
//@[000:008) Identifier |resource|
//@[009:013) Identifier |vnet|
//@[014:060) StringComplete |'Microsoft.Network/virtualNetworks@2020-06-01'|
//@[061:062) Assignment |=|
//@[063:064) LeftSquare |[|
//@[064:067) Identifier |for|
//@[068:069) Identifier |i|
//@[070:072) Identifier |in|
//@[073:078) Identifier |range|
//@[078:079) LeftParen |(|
//@[079:080) Integer |0|
//@[080:081) Comma |,|
//@[082:083) Integer |3|
//@[083:084) RightParen |)|
//@[084:085) Colon |:|
//@[086:087) LeftBrace |{|
//@[087:089) NewLine |\r\n|
  name: 'vnet-${i}'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:016) StringLeftPiece |'vnet-${|
//@[016:017) Identifier |i|
//@[017:019) StringRightPiece |}'|
//@[019:021) NewLine |\r\n|
  properties: {
//@[002:012) Identifier |properties|
//@[012:013) Colon |:|
//@[014:015) LeftBrace |{|
//@[015:017) NewLine |\r\n|
    subnets: [for j in range(0, 4): {
//@[004:011) Identifier |subnets|
//@[011:012) Colon |:|
//@[013:014) LeftSquare |[|
//@[014:017) Identifier |for|
//@[018:019) Identifier |j|
//@[020:022) Identifier |in|
//@[023:028) Identifier |range|
//@[028:029) LeftParen |(|
//@[029:030) Integer |0|
//@[030:031) Comma |,|
//@[032:033) Integer |4|
//@[033:034) RightParen |)|
//@[034:035) Colon |:|
//@[036:037) LeftBrace |{|
//@[037:039) NewLine |\r\n|
      // #completionTest(0,1,2,3,4,5) -> subnetIdAndProperties
//@[062:066) NewLine |\r\n\r\n|

      // #completionTest(6) -> subnetIdAndPropertiesNoColon
//@[059:061) NewLine |\r\n|
      name: 'subnet-${i}-${j}'
//@[006:010) Identifier |name|
//@[010:011) Colon |:|
//@[012:022) StringLeftPiece |'subnet-${|
//@[022:023) Identifier |i|
//@[023:027) StringMiddlePiece |}-${|
//@[027:028) Identifier |j|
//@[028:030) StringRightPiece |}'|
//@[030:032) NewLine |\r\n|
    }]
//@[004:005) RightBrace |}|
//@[005:006) RightSquare |]|
//@[006:008) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
}]
//@[000:001) RightBrace |}|
//@[001:002) RightSquare |]|
//@[002:006) NewLine |\r\n\r\n|

// duplicate identifiers within the loop are allowed
//@[052:054) NewLine |\r\n|
resource duplicateIdentifiersWithinLoop 'Microsoft.Network/virtualNetworks@2020-06-01' = [for i in range(0, 3): {
//@[000:008) Identifier |resource|
//@[009:039) Identifier |duplicateIdentifiersWithinLoop|
//@[040:086) StringComplete |'Microsoft.Network/virtualNetworks@2020-06-01'|
//@[087:088) Assignment |=|
//@[089:090) LeftSquare |[|
//@[090:093) Identifier |for|
//@[094:095) Identifier |i|
//@[096:098) Identifier |in|
//@[099:104) Identifier |range|
//@[104:105) LeftParen |(|
//@[105:106) Integer |0|
//@[106:107) Comma |,|
//@[108:109) Integer |3|
//@[109:110) RightParen |)|
//@[110:111) Colon |:|
//@[112:113) LeftBrace |{|
//@[113:115) NewLine |\r\n|
  name: 'vnet-${i}'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:016) StringLeftPiece |'vnet-${|
//@[016:017) Identifier |i|
//@[017:019) StringRightPiece |}'|
//@[019:021) NewLine |\r\n|
  properties: {
//@[002:012) Identifier |properties|
//@[012:013) Colon |:|
//@[014:015) LeftBrace |{|
//@[015:017) NewLine |\r\n|
    subnets: [for i in range(0, 4): {
//@[004:011) Identifier |subnets|
//@[011:012) Colon |:|
//@[013:014) LeftSquare |[|
//@[014:017) Identifier |for|
//@[018:019) Identifier |i|
//@[020:022) Identifier |in|
//@[023:028) Identifier |range|
//@[028:029) LeftParen |(|
//@[029:030) Integer |0|
//@[030:031) Comma |,|
//@[032:033) Integer |4|
//@[033:034) RightParen |)|
//@[034:035) Colon |:|
//@[036:037) LeftBrace |{|
//@[037:039) NewLine |\r\n|
      name: 'subnet-${i}-${i}'
//@[006:010) Identifier |name|
//@[010:011) Colon |:|
//@[012:022) StringLeftPiece |'subnet-${|
//@[022:023) Identifier |i|
//@[023:027) StringMiddlePiece |}-${|
//@[027:028) Identifier |i|
//@[028:030) StringRightPiece |}'|
//@[030:032) NewLine |\r\n|
    }]
//@[004:005) RightBrace |}|
//@[005:006) RightSquare |]|
//@[006:008) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
}]
//@[000:001) RightBrace |}|
//@[001:002) RightSquare |]|
//@[002:006) NewLine |\r\n\r\n|

// duplicate identifiers in global and single loop scope are allowed (inner variable hides the outer)
//@[101:103) NewLine |\r\n|
var canHaveDuplicatesAcrossScopes = 'hello'
//@[000:003) Identifier |var|
//@[004:033) Identifier |canHaveDuplicatesAcrossScopes|
//@[034:035) Assignment |=|
//@[036:043) StringComplete |'hello'|
//@[043:045) NewLine |\r\n|
resource duplicateInGlobalAndOneLoop 'Microsoft.Network/virtualNetworks@2020-06-01' = [for canHaveDuplicatesAcrossScopes in range(0, 3): {
//@[000:008) Identifier |resource|
//@[009:036) Identifier |duplicateInGlobalAndOneLoop|
//@[037:083) StringComplete |'Microsoft.Network/virtualNetworks@2020-06-01'|
//@[084:085) Assignment |=|
//@[086:087) LeftSquare |[|
//@[087:090) Identifier |for|
//@[091:120) Identifier |canHaveDuplicatesAcrossScopes|
//@[121:123) Identifier |in|
//@[124:129) Identifier |range|
//@[129:130) LeftParen |(|
//@[130:131) Integer |0|
//@[131:132) Comma |,|
//@[133:134) Integer |3|
//@[134:135) RightParen |)|
//@[135:136) Colon |:|
//@[137:138) LeftBrace |{|
//@[138:140) NewLine |\r\n|
  name: 'vnet-${canHaveDuplicatesAcrossScopes}'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:016) StringLeftPiece |'vnet-${|
//@[016:045) Identifier |canHaveDuplicatesAcrossScopes|
//@[045:047) StringRightPiece |}'|
//@[047:049) NewLine |\r\n|
  properties: {
//@[002:012) Identifier |properties|
//@[012:013) Colon |:|
//@[014:015) LeftBrace |{|
//@[015:017) NewLine |\r\n|
    subnets: [for i in range(0, 4): {
//@[004:011) Identifier |subnets|
//@[011:012) Colon |:|
//@[013:014) LeftSquare |[|
//@[014:017) Identifier |for|
//@[018:019) Identifier |i|
//@[020:022) Identifier |in|
//@[023:028) Identifier |range|
//@[028:029) LeftParen |(|
//@[029:030) Integer |0|
//@[030:031) Comma |,|
//@[032:033) Integer |4|
//@[033:034) RightParen |)|
//@[034:035) Colon |:|
//@[036:037) LeftBrace |{|
//@[037:039) NewLine |\r\n|
      name: 'subnet-${i}-${i}'
//@[006:010) Identifier |name|
//@[010:011) Colon |:|
//@[012:022) StringLeftPiece |'subnet-${|
//@[022:023) Identifier |i|
//@[023:027) StringMiddlePiece |}-${|
//@[027:028) Identifier |i|
//@[028:030) StringRightPiece |}'|
//@[030:032) NewLine |\r\n|
    }]
//@[004:005) RightBrace |}|
//@[005:006) RightSquare |]|
//@[006:008) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
}]
//@[000:001) RightBrace |}|
//@[001:002) RightSquare |]|
//@[002:006) NewLine |\r\n\r\n|

// duplicate in global and multiple loop scopes are allowed (inner hides the outer)
//@[083:085) NewLine |\r\n|
var duplicatesEverywhere = 'hello'
//@[000:003) Identifier |var|
//@[004:024) Identifier |duplicatesEverywhere|
//@[025:026) Assignment |=|
//@[027:034) StringComplete |'hello'|
//@[034:036) NewLine |\r\n|
resource duplicateInGlobalAndTwoLoops 'Microsoft.Network/virtualNetworks@2020-06-01' = [for duplicatesEverywhere in range(0, 3): {
//@[000:008) Identifier |resource|
//@[009:037) Identifier |duplicateInGlobalAndTwoLoops|
//@[038:084) StringComplete |'Microsoft.Network/virtualNetworks@2020-06-01'|
//@[085:086) Assignment |=|
//@[087:088) LeftSquare |[|
//@[088:091) Identifier |for|
//@[092:112) Identifier |duplicatesEverywhere|
//@[113:115) Identifier |in|
//@[116:121) Identifier |range|
//@[121:122) LeftParen |(|
//@[122:123) Integer |0|
//@[123:124) Comma |,|
//@[125:126) Integer |3|
//@[126:127) RightParen |)|
//@[127:128) Colon |:|
//@[129:130) LeftBrace |{|
//@[130:132) NewLine |\r\n|
  name: 'vnet-${duplicatesEverywhere}'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:016) StringLeftPiece |'vnet-${|
//@[016:036) Identifier |duplicatesEverywhere|
//@[036:038) StringRightPiece |}'|
//@[038:040) NewLine |\r\n|
  properties: {
//@[002:012) Identifier |properties|
//@[012:013) Colon |:|
//@[014:015) LeftBrace |{|
//@[015:017) NewLine |\r\n|
    subnets: [for duplicatesEverywhere in range(0, 4): {
//@[004:011) Identifier |subnets|
//@[011:012) Colon |:|
//@[013:014) LeftSquare |[|
//@[014:017) Identifier |for|
//@[018:038) Identifier |duplicatesEverywhere|
//@[039:041) Identifier |in|
//@[042:047) Identifier |range|
//@[047:048) LeftParen |(|
//@[048:049) Integer |0|
//@[049:050) Comma |,|
//@[051:052) Integer |4|
//@[052:053) RightParen |)|
//@[053:054) Colon |:|
//@[055:056) LeftBrace |{|
//@[056:058) NewLine |\r\n|
      name: 'subnet-${duplicatesEverywhere}'
//@[006:010) Identifier |name|
//@[010:011) Colon |:|
//@[012:022) StringLeftPiece |'subnet-${|
//@[022:042) Identifier |duplicatesEverywhere|
//@[042:044) StringRightPiece |}'|
//@[044:046) NewLine |\r\n|
    }]
//@[004:005) RightBrace |}|
//@[005:006) RightSquare |]|
//@[006:008) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
}]
//@[000:001) RightBrace |}|
//@[001:002) RightSquare |]|
//@[002:006) NewLine |\r\n\r\n|

/*
  Scope values created via array access on a resource collection
*/
//@[002:004) NewLine |\r\n|
resource dnsZones 'Microsoft.Network/dnsZones@2018-05-01' = [for zone in range(0,4): {
//@[000:008) Identifier |resource|
//@[009:017) Identifier |dnsZones|
//@[018:057) StringComplete |'Microsoft.Network/dnsZones@2018-05-01'|
//@[058:059) Assignment |=|
//@[060:061) LeftSquare |[|
//@[061:064) Identifier |for|
//@[065:069) Identifier |zone|
//@[070:072) Identifier |in|
//@[073:078) Identifier |range|
//@[078:079) LeftParen |(|
//@[079:080) Integer |0|
//@[080:081) Comma |,|
//@[081:082) Integer |4|
//@[082:083) RightParen |)|
//@[083:084) Colon |:|
//@[085:086) LeftBrace |{|
//@[086:088) NewLine |\r\n|
  name: 'zone${zone}'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:015) StringLeftPiece |'zone${|
//@[015:019) Identifier |zone|
//@[019:021) StringRightPiece |}'|
//@[021:023) NewLine |\r\n|
  location: 'global'
//@[002:010) Identifier |location|
//@[010:011) Colon |:|
//@[012:020) StringComplete |'global'|
//@[020:022) NewLine |\r\n|
}]
//@[000:001) RightBrace |}|
//@[001:002) RightSquare |]|
//@[002:006) NewLine |\r\n\r\n|

resource locksOnZones 'Microsoft.Authorization/locks@2016-09-01' = [for lock in range(0,2): {
//@[000:008) Identifier |resource|
//@[009:021) Identifier |locksOnZones|
//@[022:064) StringComplete |'Microsoft.Authorization/locks@2016-09-01'|
//@[065:066) Assignment |=|
//@[067:068) LeftSquare |[|
//@[068:071) Identifier |for|
//@[072:076) Identifier |lock|
//@[077:079) Identifier |in|
//@[080:085) Identifier |range|
//@[085:086) LeftParen |(|
//@[086:087) Integer |0|
//@[087:088) Comma |,|
//@[088:089) Integer |2|
//@[089:090) RightParen |)|
//@[090:091) Colon |:|
//@[092:093) LeftBrace |{|
//@[093:095) NewLine |\r\n|
  name: 'lock${lock}'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:015) StringLeftPiece |'lock${|
//@[015:019) Identifier |lock|
//@[019:021) StringRightPiece |}'|
//@[021:023) NewLine |\r\n|
  properties: {
//@[002:012) Identifier |properties|
//@[012:013) Colon |:|
//@[014:015) LeftBrace |{|
//@[015:017) NewLine |\r\n|
    level: 'CanNotDelete'
//@[004:009) Identifier |level|
//@[009:010) Colon |:|
//@[011:025) StringComplete |'CanNotDelete'|
//@[025:027) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
  scope: dnsZones[lock]
//@[002:007) Identifier |scope|
//@[007:008) Colon |:|
//@[009:017) Identifier |dnsZones|
//@[017:018) LeftSquare |[|
//@[018:022) Identifier |lock|
//@[022:023) RightSquare |]|
//@[023:025) NewLine |\r\n|
}]
//@[000:001) RightBrace |}|
//@[001:002) RightSquare |]|
//@[002:006) NewLine |\r\n\r\n|

resource moreLocksOnZones 'Microsoft.Authorization/locks@2016-09-01' = [for (lock, i) in range(0,3): {
//@[000:008) Identifier |resource|
//@[009:025) Identifier |moreLocksOnZones|
//@[026:068) StringComplete |'Microsoft.Authorization/locks@2016-09-01'|
//@[069:070) Assignment |=|
//@[071:072) LeftSquare |[|
//@[072:075) Identifier |for|
//@[076:077) LeftParen |(|
//@[077:081) Identifier |lock|
//@[081:082) Comma |,|
//@[083:084) Identifier |i|
//@[084:085) RightParen |)|
//@[086:088) Identifier |in|
//@[089:094) Identifier |range|
//@[094:095) LeftParen |(|
//@[095:096) Integer |0|
//@[096:097) Comma |,|
//@[097:098) Integer |3|
//@[098:099) RightParen |)|
//@[099:100) Colon |:|
//@[101:102) LeftBrace |{|
//@[102:104) NewLine |\r\n|
  name: 'another${i}'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:018) StringLeftPiece |'another${|
//@[018:019) Identifier |i|
//@[019:021) StringRightPiece |}'|
//@[021:023) NewLine |\r\n|
  properties: {
//@[002:012) Identifier |properties|
//@[012:013) Colon |:|
//@[014:015) LeftBrace |{|
//@[015:017) NewLine |\r\n|
    level: 'ReadOnly'
//@[004:009) Identifier |level|
//@[009:010) Colon |:|
//@[011:021) StringComplete |'ReadOnly'|
//@[021:023) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
  scope: dnsZones[i]
//@[002:007) Identifier |scope|
//@[007:008) Colon |:|
//@[009:017) Identifier |dnsZones|
//@[017:018) LeftSquare |[|
//@[018:019) Identifier |i|
//@[019:020) RightSquare |]|
//@[020:022) NewLine |\r\n|
}]
//@[000:001) RightBrace |}|
//@[001:002) RightSquare |]|
//@[002:006) NewLine |\r\n\r\n|

resource singleLockOnFirstZone 'Microsoft.Authorization/locks@2016-09-01' = {
//@[000:008) Identifier |resource|
//@[009:030) Identifier |singleLockOnFirstZone|
//@[031:073) StringComplete |'Microsoft.Authorization/locks@2016-09-01'|
//@[074:075) Assignment |=|
//@[076:077) LeftBrace |{|
//@[077:079) NewLine |\r\n|
  name: 'single-lock'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:021) StringComplete |'single-lock'|
//@[021:023) NewLine |\r\n|
  properties: {
//@[002:012) Identifier |properties|
//@[012:013) Colon |:|
//@[014:015) LeftBrace |{|
//@[015:017) NewLine |\r\n|
    level: 'ReadOnly'
//@[004:009) Identifier |level|
//@[009:010) Colon |:|
//@[011:021) StringComplete |'ReadOnly'|
//@[021:023) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
  scope: dnsZones[0]
//@[002:007) Identifier |scope|
//@[007:008) Colon |:|
//@[009:017) Identifier |dnsZones|
//@[017:018) LeftSquare |[|
//@[018:019) Integer |0|
//@[019:020) RightSquare |]|
//@[020:022) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:007) NewLine |\r\n\r\n\r\n|


resource p1_vnet 'Microsoft.Network/virtualNetworks@2020-06-01' = {
//@[000:008) Identifier |resource|
//@[009:016) Identifier |p1_vnet|
//@[017:063) StringComplete |'Microsoft.Network/virtualNetworks@2020-06-01'|
//@[064:065) Assignment |=|
//@[066:067) LeftBrace |{|
//@[067:069) NewLine |\r\n|
  location: resourceGroup().location
//@[002:010) Identifier |location|
//@[010:011) Colon |:|
//@[012:025) Identifier |resourceGroup|
//@[025:026) LeftParen |(|
//@[026:027) RightParen |)|
//@[027:028) Dot |.|
//@[028:036) Identifier |location|
//@[036:038) NewLine |\r\n|
  name: 'myVnet'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:016) StringComplete |'myVnet'|
//@[016:018) NewLine |\r\n|
  properties: {
//@[002:012) Identifier |properties|
//@[012:013) Colon |:|
//@[014:015) LeftBrace |{|
//@[015:017) NewLine |\r\n|
    addressSpace: {
//@[004:016) Identifier |addressSpace|
//@[016:017) Colon |:|
//@[018:019) LeftBrace |{|
//@[019:021) NewLine |\r\n|
      addressPrefixes: [
//@[006:021) Identifier |addressPrefixes|
//@[021:022) Colon |:|
//@[023:024) LeftSquare |[|
//@[024:026) NewLine |\r\n|
        '10.0.0.0/20'
//@[008:021) StringComplete |'10.0.0.0/20'|
//@[021:023) NewLine |\r\n|
      ]
//@[006:007) RightSquare |]|
//@[007:009) NewLine |\r\n|
    }
//@[004:005) RightBrace |}|
//@[005:007) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

resource p1_subnet1 'Microsoft.Network/virtualNetworks/subnets@2020-06-01' = {
//@[000:008) Identifier |resource|
//@[009:019) Identifier |p1_subnet1|
//@[020:074) StringComplete |'Microsoft.Network/virtualNetworks/subnets@2020-06-01'|
//@[075:076) Assignment |=|
//@[077:078) LeftBrace |{|
//@[078:080) NewLine |\r\n|
  parent: p1_vnet
//@[002:008) Identifier |parent|
//@[008:009) Colon |:|
//@[010:017) Identifier |p1_vnet|
//@[017:019) NewLine |\r\n|
  name: 'subnet1'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:017) StringComplete |'subnet1'|
//@[017:019) NewLine |\r\n|
  properties: {
//@[002:012) Identifier |properties|
//@[012:013) Colon |:|
//@[014:015) LeftBrace |{|
//@[015:017) NewLine |\r\n|
    addressPrefix: '10.0.0.0/24'
//@[004:017) Identifier |addressPrefix|
//@[017:018) Colon |:|
//@[019:032) StringComplete |'10.0.0.0/24'|
//@[032:034) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

resource p1_subnet2 'Microsoft.Network/virtualNetworks/subnets@2020-06-01' = {
//@[000:008) Identifier |resource|
//@[009:019) Identifier |p1_subnet2|
//@[020:074) StringComplete |'Microsoft.Network/virtualNetworks/subnets@2020-06-01'|
//@[075:076) Assignment |=|
//@[077:078) LeftBrace |{|
//@[078:080) NewLine |\r\n|
  parent: p1_vnet
//@[002:008) Identifier |parent|
//@[008:009) Colon |:|
//@[010:017) Identifier |p1_vnet|
//@[017:019) NewLine |\r\n|
  name: 'subnet2'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:017) StringComplete |'subnet2'|
//@[017:019) NewLine |\r\n|
  properties: {
//@[002:012) Identifier |properties|
//@[012:013) Colon |:|
//@[014:015) LeftBrace |{|
//@[015:017) NewLine |\r\n|
    addressPrefix: '10.0.1.0/24'
//@[004:017) Identifier |addressPrefix|
//@[017:018) Colon |:|
//@[019:032) StringComplete |'10.0.1.0/24'|
//@[032:034) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

output p1_subnet1prefix string = p1_subnet1.properties.addressPrefix
//@[000:006) Identifier |output|
//@[007:023) Identifier |p1_subnet1prefix|
//@[024:030) Identifier |string|
//@[031:032) Assignment |=|
//@[033:043) Identifier |p1_subnet1|
//@[043:044) Dot |.|
//@[044:054) Identifier |properties|
//@[054:055) Dot |.|
//@[055:068) Identifier |addressPrefix|
//@[068:070) NewLine |\r\n|
output p1_subnet1name string = p1_subnet1.name
//@[000:006) Identifier |output|
//@[007:021) Identifier |p1_subnet1name|
//@[022:028) Identifier |string|
//@[029:030) Assignment |=|
//@[031:041) Identifier |p1_subnet1|
//@[041:042) Dot |.|
//@[042:046) Identifier |name|
//@[046:048) NewLine |\r\n|
output p1_subnet1type string = p1_subnet1.type
//@[000:006) Identifier |output|
//@[007:021) Identifier |p1_subnet1type|
//@[022:028) Identifier |string|
//@[029:030) Assignment |=|
//@[031:041) Identifier |p1_subnet1|
//@[041:042) Dot |.|
//@[042:046) Identifier |type|
//@[046:048) NewLine |\r\n|
output p1_subnet1id string = p1_subnet1.id
//@[000:006) Identifier |output|
//@[007:019) Identifier |p1_subnet1id|
//@[020:026) Identifier |string|
//@[027:028) Assignment |=|
//@[029:039) Identifier |p1_subnet1|
//@[039:040) Dot |.|
//@[040:042) Identifier |id|
//@[042:046) NewLine |\r\n\r\n|

// parent property with extension resource
//@[042:044) NewLine |\r\n|
resource p2_res1 'Microsoft.Rp1/resource1@2020-06-01' = {
//@[000:008) Identifier |resource|
//@[009:016) Identifier |p2_res1|
//@[017:053) StringComplete |'Microsoft.Rp1/resource1@2020-06-01'|
//@[054:055) Assignment |=|
//@[056:057) LeftBrace |{|
//@[057:059) NewLine |\r\n|
  name: 'p2res1'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:016) StringComplete |'p2res1'|
//@[016:018) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

resource p2_res1child 'Microsoft.Rp1/resource1/child1@2020-06-01' = {
//@[000:008) Identifier |resource|
//@[009:021) Identifier |p2_res1child|
//@[022:065) StringComplete |'Microsoft.Rp1/resource1/child1@2020-06-01'|
//@[066:067) Assignment |=|
//@[068:069) LeftBrace |{|
//@[069:071) NewLine |\r\n|
  parent: p2_res1
//@[002:008) Identifier |parent|
//@[008:009) Colon |:|
//@[010:017) Identifier |p2_res1|
//@[017:019) NewLine |\r\n|
  name: 'child1'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:016) StringComplete |'child1'|
//@[016:018) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

resource p2_res2 'Microsoft.Rp2/resource2@2020-06-01' = {
//@[000:008) Identifier |resource|
//@[009:016) Identifier |p2_res2|
//@[017:053) StringComplete |'Microsoft.Rp2/resource2@2020-06-01'|
//@[054:055) Assignment |=|
//@[056:057) LeftBrace |{|
//@[057:059) NewLine |\r\n|
  scope: p2_res1child
//@[002:007) Identifier |scope|
//@[007:008) Colon |:|
//@[009:021) Identifier |p2_res1child|
//@[021:023) NewLine |\r\n|
  name: 'res2'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:014) StringComplete |'res2'|
//@[014:016) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

resource p2_res2child 'Microsoft.Rp2/resource2/child2@2020-06-01' = {
//@[000:008) Identifier |resource|
//@[009:021) Identifier |p2_res2child|
//@[022:065) StringComplete |'Microsoft.Rp2/resource2/child2@2020-06-01'|
//@[066:067) Assignment |=|
//@[068:069) LeftBrace |{|
//@[069:071) NewLine |\r\n|
  parent: p2_res2
//@[002:008) Identifier |parent|
//@[008:009) Colon |:|
//@[010:017) Identifier |p2_res2|
//@[017:019) NewLine |\r\n|
  name: 'child2'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:016) StringComplete |'child2'|
//@[016:018) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

output p2_res2childprop string = p2_res2child.properties.someProp
//@[000:006) Identifier |output|
//@[007:023) Identifier |p2_res2childprop|
//@[024:030) Identifier |string|
//@[031:032) Assignment |=|
//@[033:045) Identifier |p2_res2child|
//@[045:046) Dot |.|
//@[046:056) Identifier |properties|
//@[056:057) Dot |.|
//@[057:065) Identifier |someProp|
//@[065:067) NewLine |\r\n|
output p2_res2childname string = p2_res2child.name
//@[000:006) Identifier |output|
//@[007:023) Identifier |p2_res2childname|
//@[024:030) Identifier |string|
//@[031:032) Assignment |=|
//@[033:045) Identifier |p2_res2child|
//@[045:046) Dot |.|
//@[046:050) Identifier |name|
//@[050:052) NewLine |\r\n|
output p2_res2childtype string = p2_res2child.type
//@[000:006) Identifier |output|
//@[007:023) Identifier |p2_res2childtype|
//@[024:030) Identifier |string|
//@[031:032) Assignment |=|
//@[033:045) Identifier |p2_res2child|
//@[045:046) Dot |.|
//@[046:050) Identifier |type|
//@[050:052) NewLine |\r\n|
output p2_res2childid string = p2_res2child.id
//@[000:006) Identifier |output|
//@[007:021) Identifier |p2_res2childid|
//@[022:028) Identifier |string|
//@[029:030) Assignment |=|
//@[031:043) Identifier |p2_res2child|
//@[043:044) Dot |.|
//@[044:046) Identifier |id|
//@[046:050) NewLine |\r\n\r\n|

// parent property with 'existing' resource
//@[043:045) NewLine |\r\n|
resource p3_res1 'Microsoft.Rp1/resource1@2020-06-01' existing = {
//@[000:008) Identifier |resource|
//@[009:016) Identifier |p3_res1|
//@[017:053) StringComplete |'Microsoft.Rp1/resource1@2020-06-01'|
//@[054:062) Identifier |existing|
//@[063:064) Assignment |=|
//@[065:066) LeftBrace |{|
//@[066:068) NewLine |\r\n|
  name: 'p3res1'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:016) StringComplete |'p3res1'|
//@[016:018) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

resource p3_child1 'Microsoft.Rp1/resource1/child1@2020-06-01' = {
//@[000:008) Identifier |resource|
//@[009:018) Identifier |p3_child1|
//@[019:062) StringComplete |'Microsoft.Rp1/resource1/child1@2020-06-01'|
//@[063:064) Assignment |=|
//@[065:066) LeftBrace |{|
//@[066:068) NewLine |\r\n|
  parent: p3_res1
//@[002:008) Identifier |parent|
//@[008:009) Colon |:|
//@[010:017) Identifier |p3_res1|
//@[017:019) NewLine |\r\n|
  name: 'child1'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:016) StringComplete |'child1'|
//@[016:018) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

output p3_res1childprop string = p3_child1.properties.someProp
//@[000:006) Identifier |output|
//@[007:023) Identifier |p3_res1childprop|
//@[024:030) Identifier |string|
//@[031:032) Assignment |=|
//@[033:042) Identifier |p3_child1|
//@[042:043) Dot |.|
//@[043:053) Identifier |properties|
//@[053:054) Dot |.|
//@[054:062) Identifier |someProp|
//@[062:064) NewLine |\r\n|
output p3_res1childname string = p3_child1.name
//@[000:006) Identifier |output|
//@[007:023) Identifier |p3_res1childname|
//@[024:030) Identifier |string|
//@[031:032) Assignment |=|
//@[033:042) Identifier |p3_child1|
//@[042:043) Dot |.|
//@[043:047) Identifier |name|
//@[047:049) NewLine |\r\n|
output p3_res1childtype string = p3_child1.type
//@[000:006) Identifier |output|
//@[007:023) Identifier |p3_res1childtype|
//@[024:030) Identifier |string|
//@[031:032) Assignment |=|
//@[033:042) Identifier |p3_child1|
//@[042:043) Dot |.|
//@[043:047) Identifier |type|
//@[047:049) NewLine |\r\n|
output p3_res1childid string = p3_child1.id
//@[000:006) Identifier |output|
//@[007:021) Identifier |p3_res1childid|
//@[022:028) Identifier |string|
//@[029:030) Assignment |=|
//@[031:040) Identifier |p3_child1|
//@[040:041) Dot |.|
//@[041:043) Identifier |id|
//@[043:047) NewLine |\r\n\r\n|

// parent & child with 'existing'
//@[033:035) NewLine |\r\n|
resource p4_res1 'Microsoft.Rp1/resource1@2020-06-01' existing = {
//@[000:008) Identifier |resource|
//@[009:016) Identifier |p4_res1|
//@[017:053) StringComplete |'Microsoft.Rp1/resource1@2020-06-01'|
//@[054:062) Identifier |existing|
//@[063:064) Assignment |=|
//@[065:066) LeftBrace |{|
//@[066:068) NewLine |\r\n|
  scope: tenant()
//@[002:007) Identifier |scope|
//@[007:008) Colon |:|
//@[009:015) Identifier |tenant|
//@[015:016) LeftParen |(|
//@[016:017) RightParen |)|
//@[017:019) NewLine |\r\n|
  name: 'p4res1'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:016) StringComplete |'p4res1'|
//@[016:018) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

resource p4_child1 'Microsoft.Rp1/resource1/child1@2020-06-01' existing = {
//@[000:008) Identifier |resource|
//@[009:018) Identifier |p4_child1|
//@[019:062) StringComplete |'Microsoft.Rp1/resource1/child1@2020-06-01'|
//@[063:071) Identifier |existing|
//@[072:073) Assignment |=|
//@[074:075) LeftBrace |{|
//@[075:077) NewLine |\r\n|
  parent: p4_res1
//@[002:008) Identifier |parent|
//@[008:009) Colon |:|
//@[010:017) Identifier |p4_res1|
//@[017:019) NewLine |\r\n|
  name: 'child1'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:016) StringComplete |'child1'|
//@[016:018) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

output p4_res1childprop string = p4_child1.properties.someProp
//@[000:006) Identifier |output|
//@[007:023) Identifier |p4_res1childprop|
//@[024:030) Identifier |string|
//@[031:032) Assignment |=|
//@[033:042) Identifier |p4_child1|
//@[042:043) Dot |.|
//@[043:053) Identifier |properties|
//@[053:054) Dot |.|
//@[054:062) Identifier |someProp|
//@[062:064) NewLine |\r\n|
output p4_res1childname string = p4_child1.name
//@[000:006) Identifier |output|
//@[007:023) Identifier |p4_res1childname|
//@[024:030) Identifier |string|
//@[031:032) Assignment |=|
//@[033:042) Identifier |p4_child1|
//@[042:043) Dot |.|
//@[043:047) Identifier |name|
//@[047:049) NewLine |\r\n|
output p4_res1childtype string = p4_child1.type
//@[000:006) Identifier |output|
//@[007:023) Identifier |p4_res1childtype|
//@[024:030) Identifier |string|
//@[031:032) Assignment |=|
//@[033:042) Identifier |p4_child1|
//@[042:043) Dot |.|
//@[043:047) Identifier |type|
//@[047:049) NewLine |\r\n|
output p4_res1childid string = p4_child1.id
//@[000:006) Identifier |output|
//@[007:021) Identifier |p4_res1childid|
//@[022:028) Identifier |string|
//@[029:030) Assignment |=|
//@[031:040) Identifier |p4_child1|
//@[040:041) Dot |.|
//@[041:043) Identifier |id|
//@[043:047) NewLine |\r\n\r\n|

// parent & nested child with decorators https://github.com/Azure/bicep/issues/10970
//@[084:086) NewLine |\r\n|
var dbs = ['db1', 'db2','db3']
//@[000:003) Identifier |var|
//@[004:007) Identifier |dbs|
//@[008:009) Assignment |=|
//@[010:011) LeftSquare |[|
//@[011:016) StringComplete |'db1'|
//@[016:017) Comma |,|
//@[018:023) StringComplete |'db2'|
//@[023:024) Comma |,|
//@[024:029) StringComplete |'db3'|
//@[029:030) RightSquare |]|
//@[030:032) NewLine |\r\n|
resource sqlServer 'Microsoft.Sql/servers@2021-11-01' = {
//@[000:008) Identifier |resource|
//@[009:018) Identifier |sqlServer|
//@[019:053) StringComplete |'Microsoft.Sql/servers@2021-11-01'|
//@[054:055) Assignment |=|
//@[056:057) LeftBrace |{|
//@[057:059) NewLine |\r\n|
  name: 'sql-server-name'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:025) StringComplete |'sql-server-name'|
//@[025:027) NewLine |\r\n|
  location: 'polandcentral'
//@[002:010) Identifier |location|
//@[010:011) Colon |:|
//@[012:027) StringComplete |'polandcentral'|
//@[027:031) NewLine |\r\n\r\n|

  @batchSize(1)
//@[002:003) At |@|
//@[003:012) Identifier |batchSize|
//@[012:013) LeftParen |(|
//@[013:014) Integer |1|
//@[014:015) RightParen |)|
//@[015:017) NewLine |\r\n|
  @description('Sql Databases')
//@[002:003) At |@|
//@[003:014) Identifier |description|
//@[014:015) LeftParen |(|
//@[015:030) StringComplete |'Sql Databases'|
//@[030:031) RightParen |)|
//@[031:033) NewLine |\r\n|
  resource sqlDatabases 'databases' = [for db in dbs: {
//@[002:010) Identifier |resource|
//@[011:023) Identifier |sqlDatabases|
//@[024:035) StringComplete |'databases'|
//@[036:037) Assignment |=|
//@[038:039) LeftSquare |[|
//@[039:042) Identifier |for|
//@[043:045) Identifier |db|
//@[046:048) Identifier |in|
//@[049:052) Identifier |dbs|
//@[052:053) Colon |:|
//@[054:055) LeftBrace |{|
//@[055:057) NewLine |\r\n|
    name: db
//@[004:008) Identifier |name|
//@[008:009) Colon |:|
//@[010:012) Identifier |db|
//@[012:014) NewLine |\r\n|
    location: 'polandcentral'
//@[004:012) Identifier |location|
//@[012:013) Colon |:|
//@[014:029) StringComplete |'polandcentral'|
//@[029:031) NewLine |\r\n|
  }]
//@[002:003) RightBrace |}|
//@[003:004) RightSquare |]|
//@[004:008) NewLine |\r\n\r\n|

  @description('Primary Sql Database')
//@[002:003) At |@|
//@[003:014) Identifier |description|
//@[014:015) LeftParen |(|
//@[015:037) StringComplete |'Primary Sql Database'|
//@[037:038) RightParen |)|
//@[038:040) NewLine |\r\n|
  resource primaryDb 'databases' = {
//@[002:010) Identifier |resource|
//@[011:020) Identifier |primaryDb|
//@[021:032) StringComplete |'databases'|
//@[033:034) Assignment |=|
//@[035:036) LeftBrace |{|
//@[036:038) NewLine |\r\n|
    name: 'primary-db'
//@[004:008) Identifier |name|
//@[008:009) Colon |:|
//@[010:022) StringComplete |'primary-db'|
//@[022:024) NewLine |\r\n|
    location: 'polandcentral'
//@[004:012) Identifier |location|
//@[012:013) Colon |:|
//@[014:029) StringComplete |'polandcentral'|
//@[029:033) NewLine |\r\n\r\n|

    resource threatProtection 'advancedThreatProtectionSettings' existing = {
//@[004:012) Identifier |resource|
//@[013:029) Identifier |threatProtection|
//@[030:064) StringComplete |'advancedThreatProtectionSettings'|
//@[065:073) Identifier |existing|
//@[074:075) Assignment |=|
//@[076:077) LeftBrace |{|
//@[077:079) NewLine |\r\n|
      name: 'default'
//@[006:010) Identifier |name|
//@[010:011) Colon |:|
//@[012:021) StringComplete |'default'|
//@[021:023) NewLine |\r\n|
    }
//@[004:005) RightBrace |}|
//@[005:007) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

//nameof
//@[008:010) NewLine |\r\n|
output nameof_sqlServer string = nameof(sqlServer)
//@[000:006) Identifier |output|
//@[007:023) Identifier |nameof_sqlServer|
//@[024:030) Identifier |string|
//@[031:032) Assignment |=|
//@[033:039) Identifier |nameof|
//@[039:040) LeftParen |(|
//@[040:049) Identifier |sqlServer|
//@[049:050) RightParen |)|
//@[050:052) NewLine |\r\n|
output nameof_location string = nameof(sqlServer.location)
//@[000:006) Identifier |output|
//@[007:022) Identifier |nameof_location|
//@[023:029) Identifier |string|
//@[030:031) Assignment |=|
//@[032:038) Identifier |nameof|
//@[038:039) LeftParen |(|
//@[039:048) Identifier |sqlServer|
//@[048:049) Dot |.|
//@[049:057) Identifier |location|
//@[057:058) RightParen |)|
//@[058:060) NewLine |\r\n|
output nameof_minCapacity string = nameof(sqlServer::primaryDb.properties.minCapacity)
//@[000:006) Identifier |output|
//@[007:025) Identifier |nameof_minCapacity|
//@[026:032) Identifier |string|
//@[033:034) Assignment |=|
//@[035:041) Identifier |nameof|
//@[041:042) LeftParen |(|
//@[042:051) Identifier |sqlServer|
//@[051:053) DoubleColon |::|
//@[053:062) Identifier |primaryDb|
//@[062:063) Dot |.|
//@[063:073) Identifier |properties|
//@[073:074) Dot |.|
//@[074:085) Identifier |minCapacity|
//@[085:086) RightParen |)|
//@[086:088) NewLine |\r\n|
output nameof_creationTime string = nameof(sqlServer::primaryDb::threatProtection.properties.creationTime)
//@[000:006) Identifier |output|
//@[007:026) Identifier |nameof_creationTime|
//@[027:033) Identifier |string|
//@[034:035) Assignment |=|
//@[036:042) Identifier |nameof|
//@[042:043) LeftParen |(|
//@[043:052) Identifier |sqlServer|
//@[052:054) DoubleColon |::|
//@[054:063) Identifier |primaryDb|
//@[063:065) DoubleColon |::|
//@[065:081) Identifier |threatProtection|
//@[081:082) Dot |.|
//@[082:092) Identifier |properties|
//@[092:093) Dot |.|
//@[093:105) Identifier |creationTime|
//@[105:106) RightParen |)|
//@[106:108) NewLine |\r\n|
output nameof_id string = nameof(sqlServer::sqlDatabases[0].id)
//@[000:006) Identifier |output|
//@[007:016) Identifier |nameof_id|
//@[017:023) Identifier |string|
//@[024:025) Assignment |=|
//@[026:032) Identifier |nameof|
//@[032:033) LeftParen |(|
//@[033:042) Identifier |sqlServer|
//@[042:044) DoubleColon |::|
//@[044:056) Identifier |sqlDatabases|
//@[056:057) LeftSquare |[|
//@[057:058) Integer |0|
//@[058:059) RightSquare |]|
//@[059:060) Dot |.|
//@[060:062) Identifier |id|
//@[062:063) RightParen |)|
//@[063:067) NewLine |\r\n\r\n|

var sqlConfig = {
//@[000:003) Identifier |var|
//@[004:013) Identifier |sqlConfig|
//@[014:015) Assignment |=|
//@[016:017) LeftBrace |{|
//@[017:019) NewLine |\r\n|
  westus: {}
//@[002:008) Identifier |westus|
//@[008:009) Colon |:|
//@[010:011) LeftBrace |{|
//@[011:012) RightBrace |}|
//@[012:014) NewLine |\r\n|
  'server-name': {}
//@[002:015) StringComplete |'server-name'|
//@[015:016) Colon |:|
//@[017:018) LeftBrace |{|
//@[018:019) RightBrace |}|
//@[019:021) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

resource sqlServerWithNameof 'Microsoft.Sql/servers@2021-11-01' = {
//@[000:008) Identifier |resource|
//@[009:028) Identifier |sqlServerWithNameof|
//@[029:063) StringComplete |'Microsoft.Sql/servers@2021-11-01'|
//@[064:065) Assignment |=|
//@[066:067) LeftBrace |{|
//@[067:069) NewLine |\r\n|
  name: 'sql-server-nameof-${nameof(sqlConfig['server-name'])}'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:029) StringLeftPiece |'sql-server-nameof-${|
//@[029:035) Identifier |nameof|
//@[035:036) LeftParen |(|
//@[036:045) Identifier |sqlConfig|
//@[045:046) LeftSquare |[|
//@[046:059) StringComplete |'server-name'|
//@[059:060) RightSquare |]|
//@[060:061) RightParen |)|
//@[061:063) StringRightPiece |}'|
//@[063:065) NewLine |\r\n|
  location: nameof(sqlConfig.westus)
//@[002:010) Identifier |location|
//@[010:011) Colon |:|
//@[012:018) Identifier |nameof|
//@[018:019) LeftParen |(|
//@[019:028) Identifier |sqlConfig|
//@[028:029) Dot |.|
//@[029:035) Identifier |westus|
//@[035:036) RightParen |)|
//@[036:038) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\r\n|

//@[000:000) EndOfFile ||
