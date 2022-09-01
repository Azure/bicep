param name string
//@[000:005) Identifier |param|
//@[006:010) Identifier |name|
//@[011:017) Identifier |string|
//@[017:018) NewLine |\n|
param accounts array
//@[000:005) Identifier |param|
//@[006:014) Identifier |accounts|
//@[015:020) Identifier |array|
//@[020:021) NewLine |\n|
param index int
//@[000:005) Identifier |param|
//@[006:011) Identifier |index|
//@[012:015) Identifier |int|
//@[015:017) NewLine |\n\n|

// single resource
//@[018:019) NewLine |\n|
resource singleResource 'Microsoft.Storage/storageAccounts@2019-06-01' = {
//@[000:008) Identifier |resource|
//@[009:023) Identifier |singleResource|
//@[024:070) StringComplete |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[071:072) Assignment |=|
//@[073:074) LeftBrace |{|
//@[074:075) NewLine |\n|
  name: '${name}single-resource-name'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:011) StringLeftPiece |'${|
//@[011:015) Identifier |name|
//@[015:037) StringRightPiece |}single-resource-name'|
//@[037:038) NewLine |\n|
  location: resourceGroup().location
//@[002:010) Identifier |location|
//@[010:011) Colon |:|
//@[012:025) Identifier |resourceGroup|
//@[025:026) LeftParen |(|
//@[026:027) RightParen |)|
//@[027:028) Dot |.|
//@[028:036) Identifier |location|
//@[036:037) NewLine |\n|
  kind: 'StorageV2'
//@[002:006) Identifier |kind|
//@[006:007) Colon |:|
//@[008:019) StringComplete |'StorageV2'|
//@[019:020) NewLine |\n|
  sku: {
//@[002:005) Identifier |sku|
//@[005:006) Colon |:|
//@[007:008) LeftBrace |{|
//@[008:009) NewLine |\n|
    name: 'Standard_LRS'
//@[004:008) Identifier |name|
//@[008:009) Colon |:|
//@[010:024) StringComplete |'Standard_LRS'|
//@[024:025) NewLine |\n|
  }
//@[002:003) RightBrace |}|
//@[003:004) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

// extension of single resource
//@[031:032) NewLine |\n|
resource singleResourceExtension 'Microsoft.Authorization/locks@2016-09-01' = {
//@[000:008) Identifier |resource|
//@[009:032) Identifier |singleResourceExtension|
//@[033:075) StringComplete |'Microsoft.Authorization/locks@2016-09-01'|
//@[076:077) Assignment |=|
//@[078:079) LeftBrace |{|
//@[079:080) NewLine |\n|
  scope: singleResource
//@[002:007) Identifier |scope|
//@[007:008) Colon |:|
//@[009:023) Identifier |singleResource|
//@[023:024) NewLine |\n|
  name: 'single-resource-lock'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:030) StringComplete |'single-resource-lock'|
//@[030:031) NewLine |\n|
  properties: {
//@[002:012) Identifier |properties|
//@[012:013) Colon |:|
//@[014:015) LeftBrace |{|
//@[015:016) NewLine |\n|
    level: 'CanNotDelete'
//@[004:009) Identifier |level|
//@[009:010) Colon |:|
//@[011:025) StringComplete |'CanNotDelete'|
//@[025:026) NewLine |\n|
  }
//@[002:003) RightBrace |}|
//@[003:004) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

// single resource cascade extension
//@[036:037) NewLine |\n|
resource singleResourceCascadeExtension 'Microsoft.Authorization/locks@2016-09-01' = {
//@[000:008) Identifier |resource|
//@[009:039) Identifier |singleResourceCascadeExtension|
//@[040:082) StringComplete |'Microsoft.Authorization/locks@2016-09-01'|
//@[083:084) Assignment |=|
//@[085:086) LeftBrace |{|
//@[086:087) NewLine |\n|
  scope: singleResourceExtension
//@[002:007) Identifier |scope|
//@[007:008) Colon |:|
//@[009:032) Identifier |singleResourceExtension|
//@[032:033) NewLine |\n|
  name: 'single-resource-cascade-extension'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:043) StringComplete |'single-resource-cascade-extension'|
//@[043:044) NewLine |\n|
  properties: {
//@[002:012) Identifier |properties|
//@[012:013) Colon |:|
//@[014:015) LeftBrace |{|
//@[015:016) NewLine |\n|
    level: 'CanNotDelete'
//@[004:009) Identifier |level|
//@[009:010) Colon |:|
//@[011:025) StringComplete |'CanNotDelete'|
//@[025:026) NewLine |\n|
  }
//@[002:003) RightBrace |}|
//@[003:004) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

// resource collection
//@[022:023) NewLine |\n|
@batchSize(42)
//@[000:001) At |@|
//@[001:010) Identifier |batchSize|
//@[010:011) LeftParen |(|
//@[011:013) Integer |42|
//@[013:014) RightParen |)|
//@[014:015) NewLine |\n|
resource storageAccounts 'Microsoft.Storage/storageAccounts@2019-06-01' = [for (account, index) in accounts: {
//@[000:008) Identifier |resource|
//@[009:024) Identifier |storageAccounts|
//@[025:071) StringComplete |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[072:073) Assignment |=|
//@[074:075) LeftSquare |[|
//@[075:078) Identifier |for|
//@[079:080) LeftParen |(|
//@[080:087) Identifier |account|
//@[087:088) Comma |,|
//@[089:094) Identifier |index|
//@[094:095) RightParen |)|
//@[096:098) Identifier |in|
//@[099:107) Identifier |accounts|
//@[107:108) Colon |:|
//@[109:110) LeftBrace |{|
//@[110:111) NewLine |\n|
  name: '${name}-collection-${account.name}-${index}'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:011) StringLeftPiece |'${|
//@[011:015) Identifier |name|
//@[015:030) StringMiddlePiece |}-collection-${|
//@[030:037) Identifier |account|
//@[037:038) Dot |.|
//@[038:042) Identifier |name|
//@[042:046) StringMiddlePiece |}-${|
//@[046:051) Identifier |index|
//@[051:053) StringRightPiece |}'|
//@[053:054) NewLine |\n|
  location: account.location
//@[002:010) Identifier |location|
//@[010:011) Colon |:|
//@[012:019) Identifier |account|
//@[019:020) Dot |.|
//@[020:028) Identifier |location|
//@[028:029) NewLine |\n|
  kind: 'StorageV2'
//@[002:006) Identifier |kind|
//@[006:007) Colon |:|
//@[008:019) StringComplete |'StorageV2'|
//@[019:020) NewLine |\n|
  sku: {
//@[002:005) Identifier |sku|
//@[005:006) Colon |:|
//@[007:008) LeftBrace |{|
//@[008:009) NewLine |\n|
    name: 'Standard_LRS'
//@[004:008) Identifier |name|
//@[008:009) Colon |:|
//@[010:024) StringComplete |'Standard_LRS'|
//@[024:025) NewLine |\n|
  }
//@[002:003) RightBrace |}|
//@[003:004) NewLine |\n|
  dependsOn: [
//@[002:011) Identifier |dependsOn|
//@[011:012) Colon |:|
//@[013:014) LeftSquare |[|
//@[014:015) NewLine |\n|
    singleResource
//@[004:018) Identifier |singleResource|
//@[018:019) NewLine |\n|
  ]
//@[002:003) RightSquare |]|
//@[003:004) NewLine |\n|
}]
//@[000:001) RightBrace |}|
//@[001:002) RightSquare |]|
//@[002:004) NewLine |\n\n|

// extension of a single resource in a collection
//@[049:050) NewLine |\n|
resource extendSingleResourceInCollection 'Microsoft.Authorization/locks@2016-09-01' = {
//@[000:008) Identifier |resource|
//@[009:041) Identifier |extendSingleResourceInCollection|
//@[042:084) StringComplete |'Microsoft.Authorization/locks@2016-09-01'|
//@[085:086) Assignment |=|
//@[087:088) LeftBrace |{|
//@[088:089) NewLine |\n|
  name: 'one-resource-collection-item-lock'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:043) StringComplete |'one-resource-collection-item-lock'|
//@[043:044) NewLine |\n|
  properties: {
//@[002:012) Identifier |properties|
//@[012:013) Colon |:|
//@[014:015) LeftBrace |{|
//@[015:016) NewLine |\n|
    level: 'ReadOnly'
//@[004:009) Identifier |level|
//@[009:010) Colon |:|
//@[011:021) StringComplete |'ReadOnly'|
//@[021:022) NewLine |\n|
  }
//@[002:003) RightBrace |}|
//@[003:004) NewLine |\n|
  scope: storageAccounts[index % 2]
//@[002:007) Identifier |scope|
//@[007:008) Colon |:|
//@[009:024) Identifier |storageAccounts|
//@[024:025) LeftSquare |[|
//@[025:030) Identifier |index|
//@[031:032) Modulo |%|
//@[033:034) Integer |2|
//@[034:035) RightSquare |]|
//@[035:036) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

// collection of extensions
//@[027:028) NewLine |\n|
resource extensionCollection 'Microsoft.Authorization/locks@2016-09-01' = [for (i, i2) in range(0,1): {
//@[000:008) Identifier |resource|
//@[009:028) Identifier |extensionCollection|
//@[029:071) StringComplete |'Microsoft.Authorization/locks@2016-09-01'|
//@[072:073) Assignment |=|
//@[074:075) LeftSquare |[|
//@[075:078) Identifier |for|
//@[079:080) LeftParen |(|
//@[080:081) Identifier |i|
//@[081:082) Comma |,|
//@[083:085) Identifier |i2|
//@[085:086) RightParen |)|
//@[087:089) Identifier |in|
//@[090:095) Identifier |range|
//@[095:096) LeftParen |(|
//@[096:097) Integer |0|
//@[097:098) Comma |,|
//@[098:099) Integer |1|
//@[099:100) RightParen |)|
//@[100:101) Colon |:|
//@[102:103) LeftBrace |{|
//@[103:104) NewLine |\n|
  name: 'lock-${i}-${i2}'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:016) StringLeftPiece |'lock-${|
//@[016:017) Identifier |i|
//@[017:021) StringMiddlePiece |}-${|
//@[021:023) Identifier |i2|
//@[023:025) StringRightPiece |}'|
//@[025:026) NewLine |\n|
  properties: {
//@[002:012) Identifier |properties|
//@[012:013) Colon |:|
//@[014:015) LeftBrace |{|
//@[015:016) NewLine |\n|
    level: i == 0 && i2 == 0 ? 'CanNotDelete' : 'ReadOnly'
//@[004:009) Identifier |level|
//@[009:010) Colon |:|
//@[011:012) Identifier |i|
//@[013:015) Equals |==|
//@[016:017) Integer |0|
//@[018:020) LogicalAnd |&&|
//@[021:023) Identifier |i2|
//@[024:026) Equals |==|
//@[027:028) Integer |0|
//@[029:030) Question |?|
//@[031:045) StringComplete |'CanNotDelete'|
//@[046:047) Colon |:|
//@[048:058) StringComplete |'ReadOnly'|
//@[058:059) NewLine |\n|
  }
//@[002:003) RightBrace |}|
//@[003:004) NewLine |\n|
  scope: singleResource
//@[002:007) Identifier |scope|
//@[007:008) Colon |:|
//@[009:023) Identifier |singleResource|
//@[023:024) NewLine |\n|
}]
//@[000:001) RightBrace |}|
//@[001:002) RightSquare |]|
//@[002:004) NewLine |\n\n|

// cascade extend the extension
//@[031:032) NewLine |\n|
@batchSize(1)
//@[000:001) At |@|
//@[001:010) Identifier |batchSize|
//@[010:011) LeftParen |(|
//@[011:012) Integer |1|
//@[012:013) RightParen |)|
//@[013:014) NewLine |\n|
resource lockTheLocks 'Microsoft.Authorization/locks@2016-09-01' = [for (i, i2) in range(0,1): {
//@[000:008) Identifier |resource|
//@[009:021) Identifier |lockTheLocks|
//@[022:064) StringComplete |'Microsoft.Authorization/locks@2016-09-01'|
//@[065:066) Assignment |=|
//@[067:068) LeftSquare |[|
//@[068:071) Identifier |for|
//@[072:073) LeftParen |(|
//@[073:074) Identifier |i|
//@[074:075) Comma |,|
//@[076:078) Identifier |i2|
//@[078:079) RightParen |)|
//@[080:082) Identifier |in|
//@[083:088) Identifier |range|
//@[088:089) LeftParen |(|
//@[089:090) Integer |0|
//@[090:091) Comma |,|
//@[091:092) Integer |1|
//@[092:093) RightParen |)|
//@[093:094) Colon |:|
//@[095:096) LeftBrace |{|
//@[096:097) NewLine |\n|
  name: 'lock-the-lock-${i}-${i2}'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:025) StringLeftPiece |'lock-the-lock-${|
//@[025:026) Identifier |i|
//@[026:030) StringMiddlePiece |}-${|
//@[030:032) Identifier |i2|
//@[032:034) StringRightPiece |}'|
//@[034:035) NewLine |\n|
  properties: {
//@[002:012) Identifier |properties|
//@[012:013) Colon |:|
//@[014:015) LeftBrace |{|
//@[015:016) NewLine |\n|
    level: i == 0 && i2 == 0 ? 'CanNotDelete' : 'ReadOnly'
//@[004:009) Identifier |level|
//@[009:010) Colon |:|
//@[011:012) Identifier |i|
//@[013:015) Equals |==|
//@[016:017) Integer |0|
//@[018:020) LogicalAnd |&&|
//@[021:023) Identifier |i2|
//@[024:026) Equals |==|
//@[027:028) Integer |0|
//@[029:030) Question |?|
//@[031:045) StringComplete |'CanNotDelete'|
//@[046:047) Colon |:|
//@[048:058) StringComplete |'ReadOnly'|
//@[058:059) NewLine |\n|
  }
//@[002:003) RightBrace |}|
//@[003:004) NewLine |\n|
  scope: extensionCollection[i2]
//@[002:007) Identifier |scope|
//@[007:008) Colon |:|
//@[009:028) Identifier |extensionCollection|
//@[028:029) LeftSquare |[|
//@[029:031) Identifier |i2|
//@[031:032) RightSquare |]|
//@[032:033) NewLine |\n|
}]
//@[000:001) RightBrace |}|
//@[001:002) RightSquare |]|
//@[002:004) NewLine |\n\n|

// special case property access
//@[031:032) NewLine |\n|
output indexedCollectionBlobEndpoint string = storageAccounts[index].properties.primaryEndpoints.blob
//@[000:006) Identifier |output|
//@[007:036) Identifier |indexedCollectionBlobEndpoint|
//@[037:043) Identifier |string|
//@[044:045) Assignment |=|
//@[046:061) Identifier |storageAccounts|
//@[061:062) LeftSquare |[|
//@[062:067) Identifier |index|
//@[067:068) RightSquare |]|
//@[068:069) Dot |.|
//@[069:079) Identifier |properties|
//@[079:080) Dot |.|
//@[080:096) Identifier |primaryEndpoints|
//@[096:097) Dot |.|
//@[097:101) Identifier |blob|
//@[101:102) NewLine |\n|
output indexedCollectionName string = storageAccounts[index].name
//@[000:006) Identifier |output|
//@[007:028) Identifier |indexedCollectionName|
//@[029:035) Identifier |string|
//@[036:037) Assignment |=|
//@[038:053) Identifier |storageAccounts|
//@[053:054) LeftSquare |[|
//@[054:059) Identifier |index|
//@[059:060) RightSquare |]|
//@[060:061) Dot |.|
//@[061:065) Identifier |name|
//@[065:066) NewLine |\n|
output indexedCollectionId string = storageAccounts[index].id
//@[000:006) Identifier |output|
//@[007:026) Identifier |indexedCollectionId|
//@[027:033) Identifier |string|
//@[034:035) Assignment |=|
//@[036:051) Identifier |storageAccounts|
//@[051:052) LeftSquare |[|
//@[052:057) Identifier |index|
//@[057:058) RightSquare |]|
//@[058:059) Dot |.|
//@[059:061) Identifier |id|
//@[061:062) NewLine |\n|
output indexedCollectionType string = storageAccounts[index].type
//@[000:006) Identifier |output|
//@[007:028) Identifier |indexedCollectionType|
//@[029:035) Identifier |string|
//@[036:037) Assignment |=|
//@[038:053) Identifier |storageAccounts|
//@[053:054) LeftSquare |[|
//@[054:059) Identifier |index|
//@[059:060) RightSquare |]|
//@[060:061) Dot |.|
//@[061:065) Identifier |type|
//@[065:066) NewLine |\n|
output indexedCollectionVersion string = storageAccounts[index].apiVersion
//@[000:006) Identifier |output|
//@[007:031) Identifier |indexedCollectionVersion|
//@[032:038) Identifier |string|
//@[039:040) Assignment |=|
//@[041:056) Identifier |storageAccounts|
//@[056:057) LeftSquare |[|
//@[057:062) Identifier |index|
//@[062:063) RightSquare |]|
//@[063:064) Dot |.|
//@[064:074) Identifier |apiVersion|
//@[074:076) NewLine |\n\n|

// general case property access
//@[031:032) NewLine |\n|
output indexedCollectionIdentity object = storageAccounts[index].identity
//@[000:006) Identifier |output|
//@[007:032) Identifier |indexedCollectionIdentity|
//@[033:039) Identifier |object|
//@[040:041) Assignment |=|
//@[042:057) Identifier |storageAccounts|
//@[057:058) LeftSquare |[|
//@[058:063) Identifier |index|
//@[063:064) RightSquare |]|
//@[064:065) Dot |.|
//@[065:073) Identifier |identity|
//@[073:075) NewLine |\n\n|

// indexed access of two properties
//@[035:036) NewLine |\n|
output indexedEndpointPair object = {
//@[000:006) Identifier |output|
//@[007:026) Identifier |indexedEndpointPair|
//@[027:033) Identifier |object|
//@[034:035) Assignment |=|
//@[036:037) LeftBrace |{|
//@[037:038) NewLine |\n|
  primary: storageAccounts[index].properties.primaryEndpoints.blob
//@[002:009) Identifier |primary|
//@[009:010) Colon |:|
//@[011:026) Identifier |storageAccounts|
//@[026:027) LeftSquare |[|
//@[027:032) Identifier |index|
//@[032:033) RightSquare |]|
//@[033:034) Dot |.|
//@[034:044) Identifier |properties|
//@[044:045) Dot |.|
//@[045:061) Identifier |primaryEndpoints|
//@[061:062) Dot |.|
//@[062:066) Identifier |blob|
//@[066:067) NewLine |\n|
  secondary: storageAccounts[index + 1].properties.secondaryEndpoints.blob
//@[002:011) Identifier |secondary|
//@[011:012) Colon |:|
//@[013:028) Identifier |storageAccounts|
//@[028:029) LeftSquare |[|
//@[029:034) Identifier |index|
//@[035:036) Plus |+|
//@[037:038) Integer |1|
//@[038:039) RightSquare |]|
//@[039:040) Dot |.|
//@[040:050) Identifier |properties|
//@[050:051) Dot |.|
//@[051:069) Identifier |secondaryEndpoints|
//@[069:070) Dot |.|
//@[070:074) Identifier |blob|
//@[074:075) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

// nested indexer?
//@[018:019) NewLine |\n|
output indexViaReference string = storageAccounts[int(storageAccounts[index].properties.creationTime)].properties.accessTier
//@[000:006) Identifier |output|
//@[007:024) Identifier |indexViaReference|
//@[025:031) Identifier |string|
//@[032:033) Assignment |=|
//@[034:049) Identifier |storageAccounts|
//@[049:050) LeftSquare |[|
//@[050:053) Identifier |int|
//@[053:054) LeftParen |(|
//@[054:069) Identifier |storageAccounts|
//@[069:070) LeftSquare |[|
//@[070:075) Identifier |index|
//@[075:076) RightSquare |]|
//@[076:077) Dot |.|
//@[077:087) Identifier |properties|
//@[087:088) Dot |.|
//@[088:100) Identifier |creationTime|
//@[100:101) RightParen |)|
//@[101:102) RightSquare |]|
//@[102:103) Dot |.|
//@[103:113) Identifier |properties|
//@[113:114) Dot |.|
//@[114:124) Identifier |accessTier|
//@[124:126) NewLine |\n\n|

// dependency on a resource collection
//@[038:039) NewLine |\n|
resource storageAccounts2 'Microsoft.Storage/storageAccounts@2019-06-01' = [for (account, idx) in accounts: {
//@[000:008) Identifier |resource|
//@[009:025) Identifier |storageAccounts2|
//@[026:072) StringComplete |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[073:074) Assignment |=|
//@[075:076) LeftSquare |[|
//@[076:079) Identifier |for|
//@[080:081) LeftParen |(|
//@[081:088) Identifier |account|
//@[088:089) Comma |,|
//@[090:093) Identifier |idx|
//@[093:094) RightParen |)|
//@[095:097) Identifier |in|
//@[098:106) Identifier |accounts|
//@[106:107) Colon |:|
//@[108:109) LeftBrace |{|
//@[109:110) NewLine |\n|
  name: '${name}-collection-${account.name}-${idx}'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:011) StringLeftPiece |'${|
//@[011:015) Identifier |name|
//@[015:030) StringMiddlePiece |}-collection-${|
//@[030:037) Identifier |account|
//@[037:038) Dot |.|
//@[038:042) Identifier |name|
//@[042:046) StringMiddlePiece |}-${|
//@[046:049) Identifier |idx|
//@[049:051) StringRightPiece |}'|
//@[051:052) NewLine |\n|
  location: account.location
//@[002:010) Identifier |location|
//@[010:011) Colon |:|
//@[012:019) Identifier |account|
//@[019:020) Dot |.|
//@[020:028) Identifier |location|
//@[028:029) NewLine |\n|
  kind: 'StorageV2'
//@[002:006) Identifier |kind|
//@[006:007) Colon |:|
//@[008:019) StringComplete |'StorageV2'|
//@[019:020) NewLine |\n|
  sku: {
//@[002:005) Identifier |sku|
//@[005:006) Colon |:|
//@[007:008) LeftBrace |{|
//@[008:009) NewLine |\n|
    name: 'Standard_LRS'
//@[004:008) Identifier |name|
//@[008:009) Colon |:|
//@[010:024) StringComplete |'Standard_LRS'|
//@[024:025) NewLine |\n|
  }
//@[002:003) RightBrace |}|
//@[003:004) NewLine |\n|
  dependsOn: [
//@[002:011) Identifier |dependsOn|
//@[011:012) Colon |:|
//@[013:014) LeftSquare |[|
//@[014:015) NewLine |\n|
    storageAccounts
//@[004:019) Identifier |storageAccounts|
//@[019:020) NewLine |\n|
  ]
//@[002:003) RightSquare |]|
//@[003:004) NewLine |\n|
}]
//@[000:001) RightBrace |}|
//@[001:002) RightSquare |]|
//@[002:004) NewLine |\n\n|

// one-to-one paired dependencies
//@[033:034) NewLine |\n|
resource firstSet 'Microsoft.Storage/storageAccounts@2019-06-01' = [for (i,ii) in range(0, length(accounts)): {
//@[000:008) Identifier |resource|
//@[009:017) Identifier |firstSet|
//@[018:064) StringComplete |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[065:066) Assignment |=|
//@[067:068) LeftSquare |[|
//@[068:071) Identifier |for|
//@[072:073) LeftParen |(|
//@[073:074) Identifier |i|
//@[074:075) Comma |,|
//@[075:077) Identifier |ii|
//@[077:078) RightParen |)|
//@[079:081) Identifier |in|
//@[082:087) Identifier |range|
//@[087:088) LeftParen |(|
//@[088:089) Integer |0|
//@[089:090) Comma |,|
//@[091:097) Identifier |length|
//@[097:098) LeftParen |(|
//@[098:106) Identifier |accounts|
//@[106:107) RightParen |)|
//@[107:108) RightParen |)|
//@[108:109) Colon |:|
//@[110:111) LeftBrace |{|
//@[111:112) NewLine |\n|
  name: '${name}-set1-${i}-${ii}'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:011) StringLeftPiece |'${|
//@[011:015) Identifier |name|
//@[015:024) StringMiddlePiece |}-set1-${|
//@[024:025) Identifier |i|
//@[025:029) StringMiddlePiece |}-${|
//@[029:031) Identifier |ii|
//@[031:033) StringRightPiece |}'|
//@[033:034) NewLine |\n|
  location: resourceGroup().location
//@[002:010) Identifier |location|
//@[010:011) Colon |:|
//@[012:025) Identifier |resourceGroup|
//@[025:026) LeftParen |(|
//@[026:027) RightParen |)|
//@[027:028) Dot |.|
//@[028:036) Identifier |location|
//@[036:037) NewLine |\n|
  kind: 'StorageV2'
//@[002:006) Identifier |kind|
//@[006:007) Colon |:|
//@[008:019) StringComplete |'StorageV2'|
//@[019:020) NewLine |\n|
  sku: {
//@[002:005) Identifier |sku|
//@[005:006) Colon |:|
//@[007:008) LeftBrace |{|
//@[008:009) NewLine |\n|
    name: 'Standard_LRS'
//@[004:008) Identifier |name|
//@[008:009) Colon |:|
//@[010:024) StringComplete |'Standard_LRS'|
//@[024:025) NewLine |\n|
  }
//@[002:003) RightBrace |}|
//@[003:004) NewLine |\n|
}]
//@[000:001) RightBrace |}|
//@[001:002) RightSquare |]|
//@[002:004) NewLine |\n\n|

resource secondSet 'Microsoft.Storage/storageAccounts@2019-06-01' = [for (i,iii) in range(0, length(accounts)): {
//@[000:008) Identifier |resource|
//@[009:018) Identifier |secondSet|
//@[019:065) StringComplete |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[066:067) Assignment |=|
//@[068:069) LeftSquare |[|
//@[069:072) Identifier |for|
//@[073:074) LeftParen |(|
//@[074:075) Identifier |i|
//@[075:076) Comma |,|
//@[076:079) Identifier |iii|
//@[079:080) RightParen |)|
//@[081:083) Identifier |in|
//@[084:089) Identifier |range|
//@[089:090) LeftParen |(|
//@[090:091) Integer |0|
//@[091:092) Comma |,|
//@[093:099) Identifier |length|
//@[099:100) LeftParen |(|
//@[100:108) Identifier |accounts|
//@[108:109) RightParen |)|
//@[109:110) RightParen |)|
//@[110:111) Colon |:|
//@[112:113) LeftBrace |{|
//@[113:114) NewLine |\n|
  name: '${name}-set2-${i}-${iii}'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:011) StringLeftPiece |'${|
//@[011:015) Identifier |name|
//@[015:024) StringMiddlePiece |}-set2-${|
//@[024:025) Identifier |i|
//@[025:029) StringMiddlePiece |}-${|
//@[029:032) Identifier |iii|
//@[032:034) StringRightPiece |}'|
//@[034:035) NewLine |\n|
  location: resourceGroup().location
//@[002:010) Identifier |location|
//@[010:011) Colon |:|
//@[012:025) Identifier |resourceGroup|
//@[025:026) LeftParen |(|
//@[026:027) RightParen |)|
//@[027:028) Dot |.|
//@[028:036) Identifier |location|
//@[036:037) NewLine |\n|
  kind: 'StorageV2'
//@[002:006) Identifier |kind|
//@[006:007) Colon |:|
//@[008:019) StringComplete |'StorageV2'|
//@[019:020) NewLine |\n|
  sku: {
//@[002:005) Identifier |sku|
//@[005:006) Colon |:|
//@[007:008) LeftBrace |{|
//@[008:009) NewLine |\n|
    name: 'Standard_LRS'
//@[004:008) Identifier |name|
//@[008:009) Colon |:|
//@[010:024) StringComplete |'Standard_LRS'|
//@[024:025) NewLine |\n|
  }
//@[002:003) RightBrace |}|
//@[003:004) NewLine |\n|
  dependsOn: [
//@[002:011) Identifier |dependsOn|
//@[011:012) Colon |:|
//@[013:014) LeftSquare |[|
//@[014:015) NewLine |\n|
    firstSet[iii]
//@[004:012) Identifier |firstSet|
//@[012:013) LeftSquare |[|
//@[013:016) Identifier |iii|
//@[016:017) RightSquare |]|
//@[017:018) NewLine |\n|
  ]
//@[002:003) RightSquare |]|
//@[003:004) NewLine |\n|
}]
//@[000:001) RightBrace |}|
//@[001:002) RightSquare |]|
//@[002:004) NewLine |\n\n|

// depending on collection and one resource in the collection optimizes the latter part away
//@[092:093) NewLine |\n|
resource anotherSingleResource 'Microsoft.Storage/storageAccounts@2019-06-01' = {
//@[000:008) Identifier |resource|
//@[009:030) Identifier |anotherSingleResource|
//@[031:077) StringComplete |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[078:079) Assignment |=|
//@[080:081) LeftBrace |{|
//@[081:082) NewLine |\n|
  name: '${name}single-resource-name'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:011) StringLeftPiece |'${|
//@[011:015) Identifier |name|
//@[015:037) StringRightPiece |}single-resource-name'|
//@[037:038) NewLine |\n|
  location: resourceGroup().location
//@[002:010) Identifier |location|
//@[010:011) Colon |:|
//@[012:025) Identifier |resourceGroup|
//@[025:026) LeftParen |(|
//@[026:027) RightParen |)|
//@[027:028) Dot |.|
//@[028:036) Identifier |location|
//@[036:037) NewLine |\n|
  kind: 'StorageV2'
//@[002:006) Identifier |kind|
//@[006:007) Colon |:|
//@[008:019) StringComplete |'StorageV2'|
//@[019:020) NewLine |\n|
  sku: {
//@[002:005) Identifier |sku|
//@[005:006) Colon |:|
//@[007:008) LeftBrace |{|
//@[008:009) NewLine |\n|
    name: 'Standard_LRS'
//@[004:008) Identifier |name|
//@[008:009) Colon |:|
//@[010:024) StringComplete |'Standard_LRS'|
//@[024:025) NewLine |\n|
  }
//@[002:003) RightBrace |}|
//@[003:004) NewLine |\n|
  dependsOn: [
//@[002:011) Identifier |dependsOn|
//@[011:012) Colon |:|
//@[013:014) LeftSquare |[|
//@[014:015) NewLine |\n|
    secondSet
//@[004:013) Identifier |secondSet|
//@[013:014) NewLine |\n|
    secondSet[0]
//@[004:013) Identifier |secondSet|
//@[013:014) LeftSquare |[|
//@[014:015) Integer |0|
//@[015:016) RightSquare |]|
//@[016:017) NewLine |\n|
  ]
//@[002:003) RightSquare |]|
//@[003:004) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

// vnets
//@[008:009) NewLine |\n|
var vnetConfigurations = [
//@[000:003) Identifier |var|
//@[004:022) Identifier |vnetConfigurations|
//@[023:024) Assignment |=|
//@[025:026) LeftSquare |[|
//@[026:027) NewLine |\n|
  {
//@[002:003) LeftBrace |{|
//@[003:004) NewLine |\n|
    name: 'one'
//@[004:008) Identifier |name|
//@[008:009) Colon |:|
//@[010:015) StringComplete |'one'|
//@[015:016) NewLine |\n|
    location: resourceGroup().location
//@[004:012) Identifier |location|
//@[012:013) Colon |:|
//@[014:027) Identifier |resourceGroup|
//@[027:028) LeftParen |(|
//@[028:029) RightParen |)|
//@[029:030) Dot |.|
//@[030:038) Identifier |location|
//@[038:039) NewLine |\n|
  }
//@[002:003) RightBrace |}|
//@[003:004) NewLine |\n|
  {
//@[002:003) LeftBrace |{|
//@[003:004) NewLine |\n|
    name: 'two'
//@[004:008) Identifier |name|
//@[008:009) Colon |:|
//@[010:015) StringComplete |'two'|
//@[015:016) NewLine |\n|
    location: 'westus'
//@[004:012) Identifier |location|
//@[012:013) Colon |:|
//@[014:022) StringComplete |'westus'|
//@[022:023) NewLine |\n|
  }
//@[002:003) RightBrace |}|
//@[003:004) NewLine |\n|
]
//@[000:001) RightSquare |]|
//@[001:003) NewLine |\n\n|

resource vnets 'Microsoft.Network/virtualNetworks@2020-06-01' = [for (vnetConfig, index) in vnetConfigurations: {
//@[000:008) Identifier |resource|
//@[009:014) Identifier |vnets|
//@[015:061) StringComplete |'Microsoft.Network/virtualNetworks@2020-06-01'|
//@[062:063) Assignment |=|
//@[064:065) LeftSquare |[|
//@[065:068) Identifier |for|
//@[069:070) LeftParen |(|
//@[070:080) Identifier |vnetConfig|
//@[080:081) Comma |,|
//@[082:087) Identifier |index|
//@[087:088) RightParen |)|
//@[089:091) Identifier |in|
//@[092:110) Identifier |vnetConfigurations|
//@[110:111) Colon |:|
//@[112:113) LeftBrace |{|
//@[113:114) NewLine |\n|
  name: '${vnetConfig.name}-${index}'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:011) StringLeftPiece |'${|
//@[011:021) Identifier |vnetConfig|
//@[021:022) Dot |.|
//@[022:026) Identifier |name|
//@[026:030) StringMiddlePiece |}-${|
//@[030:035) Identifier |index|
//@[035:037) StringRightPiece |}'|
//@[037:038) NewLine |\n|
  location: vnetConfig.location
//@[002:010) Identifier |location|
//@[010:011) Colon |:|
//@[012:022) Identifier |vnetConfig|
//@[022:023) Dot |.|
//@[023:031) Identifier |location|
//@[031:032) NewLine |\n|
}]
//@[000:001) RightBrace |}|
//@[001:002) RightSquare |]|
//@[002:004) NewLine |\n\n|

// implicit dependency on single resource from a resource collection
//@[068:069) NewLine |\n|
resource implicitDependencyOnSingleResourceByIndex 'Microsoft.Network/dnsZones@2018-05-01' = {
//@[000:008) Identifier |resource|
//@[009:050) Identifier |implicitDependencyOnSingleResourceByIndex|
//@[051:090) StringComplete |'Microsoft.Network/dnsZones@2018-05-01'|
//@[091:092) Assignment |=|
//@[093:094) LeftBrace |{|
//@[094:095) NewLine |\n|
  name: 'test'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:014) StringComplete |'test'|
//@[014:015) NewLine |\n|
  location: 'global'
//@[002:010) Identifier |location|
//@[010:011) Colon |:|
//@[012:020) StringComplete |'global'|
//@[020:021) NewLine |\n|
  properties: {
//@[002:012) Identifier |properties|
//@[012:013) Colon |:|
//@[014:015) LeftBrace |{|
//@[015:016) NewLine |\n|
    resolutionVirtualNetworks: [
//@[004:029) Identifier |resolutionVirtualNetworks|
//@[029:030) Colon |:|
//@[031:032) LeftSquare |[|
//@[032:033) NewLine |\n|
      {
//@[006:007) LeftBrace |{|
//@[007:008) NewLine |\n|
        id: vnets[index+1].id
//@[008:010) Identifier |id|
//@[010:011) Colon |:|
//@[012:017) Identifier |vnets|
//@[017:018) LeftSquare |[|
//@[018:023) Identifier |index|
//@[023:024) Plus |+|
//@[024:025) Integer |1|
//@[025:026) RightSquare |]|
//@[026:027) Dot |.|
//@[027:029) Identifier |id|
//@[029:030) NewLine |\n|
      }
//@[006:007) RightBrace |}|
//@[007:008) NewLine |\n|
    ]
//@[004:005) RightSquare |]|
//@[005:006) NewLine |\n|
  }
//@[002:003) RightBrace |}|
//@[003:004) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

// implicit and explicit dependency combined
//@[044:045) NewLine |\n|
resource combinedDependencies 'Microsoft.Network/dnsZones@2018-05-01' = {
//@[000:008) Identifier |resource|
//@[009:029) Identifier |combinedDependencies|
//@[030:069) StringComplete |'Microsoft.Network/dnsZones@2018-05-01'|
//@[070:071) Assignment |=|
//@[072:073) LeftBrace |{|
//@[073:074) NewLine |\n|
  name: 'test2'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:015) StringComplete |'test2'|
//@[015:016) NewLine |\n|
  location: 'global'
//@[002:010) Identifier |location|
//@[010:011) Colon |:|
//@[012:020) StringComplete |'global'|
//@[020:021) NewLine |\n|
  properties: {
//@[002:012) Identifier |properties|
//@[012:013) Colon |:|
//@[014:015) LeftBrace |{|
//@[015:016) NewLine |\n|
    resolutionVirtualNetworks: [
//@[004:029) Identifier |resolutionVirtualNetworks|
//@[029:030) Colon |:|
//@[031:032) LeftSquare |[|
//@[032:033) NewLine |\n|
      {
//@[006:007) LeftBrace |{|
//@[007:008) NewLine |\n|
        id: vnets[index-1].id
//@[008:010) Identifier |id|
//@[010:011) Colon |:|
//@[012:017) Identifier |vnets|
//@[017:018) LeftSquare |[|
//@[018:023) Identifier |index|
//@[023:024) Minus |-|
//@[024:025) Integer |1|
//@[025:026) RightSquare |]|
//@[026:027) Dot |.|
//@[027:029) Identifier |id|
//@[029:030) NewLine |\n|
      }
//@[006:007) RightBrace |}|
//@[007:008) NewLine |\n|
      {
//@[006:007) LeftBrace |{|
//@[007:008) NewLine |\n|
        id: vnets[index * 2].id
//@[008:010) Identifier |id|
//@[010:011) Colon |:|
//@[012:017) Identifier |vnets|
//@[017:018) LeftSquare |[|
//@[018:023) Identifier |index|
//@[024:025) Asterisk |*|
//@[026:027) Integer |2|
//@[027:028) RightSquare |]|
//@[028:029) Dot |.|
//@[029:031) Identifier |id|
//@[031:032) NewLine |\n|
      }
//@[006:007) RightBrace |}|
//@[007:008) NewLine |\n|
    ]
//@[004:005) RightSquare |]|
//@[005:006) NewLine |\n|
  }
//@[002:003) RightBrace |}|
//@[003:004) NewLine |\n|
  dependsOn: [
//@[002:011) Identifier |dependsOn|
//@[011:012) Colon |:|
//@[013:014) LeftSquare |[|
//@[014:015) NewLine |\n|
    vnets
//@[004:009) Identifier |vnets|
//@[009:010) NewLine |\n|
  ]
//@[002:003) RightSquare |]|
//@[003:004) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

// single module
//@[016:017) NewLine |\n|
module singleModule 'passthrough.bicep' = {
//@[000:006) Identifier |module|
//@[007:019) Identifier |singleModule|
//@[020:039) StringComplete |'passthrough.bicep'|
//@[040:041) Assignment |=|
//@[042:043) LeftBrace |{|
//@[043:044) NewLine |\n|
  name: 'test'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:014) StringComplete |'test'|
//@[014:015) NewLine |\n|
  params: {
//@[002:008) Identifier |params|
//@[008:009) Colon |:|
//@[010:011) LeftBrace |{|
//@[011:012) NewLine |\n|
    myInput: 'hello'
//@[004:011) Identifier |myInput|
//@[011:012) Colon |:|
//@[013:020) StringComplete |'hello'|
//@[020:021) NewLine |\n|
  }
//@[002:003) RightBrace |}|
//@[003:004) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

var moduleSetup = [
//@[000:003) Identifier |var|
//@[004:015) Identifier |moduleSetup|
//@[016:017) Assignment |=|
//@[018:019) LeftSquare |[|
//@[019:020) NewLine |\n|
  'one'
//@[002:007) StringComplete |'one'|
//@[007:008) NewLine |\n|
  'two'
//@[002:007) StringComplete |'two'|
//@[007:008) NewLine |\n|
  'three'
//@[002:009) StringComplete |'three'|
//@[009:010) NewLine |\n|
]
//@[000:001) RightSquare |]|
//@[001:003) NewLine |\n\n|

// module collection plus explicit dependency on single module
//@[062:063) NewLine |\n|
@sys.batchSize(3)
//@[000:001) At |@|
//@[001:004) Identifier |sys|
//@[004:005) Dot |.|
//@[005:014) Identifier |batchSize|
//@[014:015) LeftParen |(|
//@[015:016) Integer |3|
//@[016:017) RightParen |)|
//@[017:018) NewLine |\n|
module moduleCollectionWithSingleDependency 'passthrough.bicep' = [for (moduleName, moduleIndex) in moduleSetup: {
//@[000:006) Identifier |module|
//@[007:043) Identifier |moduleCollectionWithSingleDependency|
//@[044:063) StringComplete |'passthrough.bicep'|
//@[064:065) Assignment |=|
//@[066:067) LeftSquare |[|
//@[067:070) Identifier |for|
//@[071:072) LeftParen |(|
//@[072:082) Identifier |moduleName|
//@[082:083) Comma |,|
//@[084:095) Identifier |moduleIndex|
//@[095:096) RightParen |)|
//@[097:099) Identifier |in|
//@[100:111) Identifier |moduleSetup|
//@[111:112) Colon |:|
//@[113:114) LeftBrace |{|
//@[114:115) NewLine |\n|
  name: concat(moduleName, moduleIndex)
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:014) Identifier |concat|
//@[014:015) LeftParen |(|
//@[015:025) Identifier |moduleName|
//@[025:026) Comma |,|
//@[027:038) Identifier |moduleIndex|
//@[038:039) RightParen |)|
//@[039:040) NewLine |\n|
  params: {
//@[002:008) Identifier |params|
//@[008:009) Colon |:|
//@[010:011) LeftBrace |{|
//@[011:012) NewLine |\n|
    myInput: 'in-${moduleName}-${moduleIndex}'
//@[004:011) Identifier |myInput|
//@[011:012) Colon |:|
//@[013:019) StringLeftPiece |'in-${|
//@[019:029) Identifier |moduleName|
//@[029:033) StringMiddlePiece |}-${|
//@[033:044) Identifier |moduleIndex|
//@[044:046) StringRightPiece |}'|
//@[046:047) NewLine |\n|
  }
//@[002:003) RightBrace |}|
//@[003:004) NewLine |\n|
  dependsOn: [
//@[002:011) Identifier |dependsOn|
//@[011:012) Colon |:|
//@[013:014) LeftSquare |[|
//@[014:015) NewLine |\n|
    singleModule
//@[004:016) Identifier |singleModule|
//@[016:017) NewLine |\n|
    singleResource
//@[004:018) Identifier |singleResource|
//@[018:019) NewLine |\n|
  ]
//@[002:003) RightSquare |]|
//@[003:004) NewLine |\n|
}]
//@[000:001) RightBrace |}|
//@[001:002) RightSquare |]|
//@[002:004) NewLine |\n\n|

// another module collection with dependency on another module collection
//@[073:074) NewLine |\n|
module moduleCollectionWithCollectionDependencies 'passthrough.bicep' = [for (moduleName, moduleIndex) in moduleSetup: {
//@[000:006) Identifier |module|
//@[007:049) Identifier |moduleCollectionWithCollectionDependencies|
//@[050:069) StringComplete |'passthrough.bicep'|
//@[070:071) Assignment |=|
//@[072:073) LeftSquare |[|
//@[073:076) Identifier |for|
//@[077:078) LeftParen |(|
//@[078:088) Identifier |moduleName|
//@[088:089) Comma |,|
//@[090:101) Identifier |moduleIndex|
//@[101:102) RightParen |)|
//@[103:105) Identifier |in|
//@[106:117) Identifier |moduleSetup|
//@[117:118) Colon |:|
//@[119:120) LeftBrace |{|
//@[120:121) NewLine |\n|
  name: concat(moduleName, moduleIndex)
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:014) Identifier |concat|
//@[014:015) LeftParen |(|
//@[015:025) Identifier |moduleName|
//@[025:026) Comma |,|
//@[027:038) Identifier |moduleIndex|
//@[038:039) RightParen |)|
//@[039:040) NewLine |\n|
  params: {
//@[002:008) Identifier |params|
//@[008:009) Colon |:|
//@[010:011) LeftBrace |{|
//@[011:012) NewLine |\n|
    myInput: 'in-${moduleName}-${moduleIndex}'
//@[004:011) Identifier |myInput|
//@[011:012) Colon |:|
//@[013:019) StringLeftPiece |'in-${|
//@[019:029) Identifier |moduleName|
//@[029:033) StringMiddlePiece |}-${|
//@[033:044) Identifier |moduleIndex|
//@[044:046) StringRightPiece |}'|
//@[046:047) NewLine |\n|
  }
//@[002:003) RightBrace |}|
//@[003:004) NewLine |\n|
  dependsOn: [
//@[002:011) Identifier |dependsOn|
//@[011:012) Colon |:|
//@[013:014) LeftSquare |[|
//@[014:015) NewLine |\n|
    storageAccounts
//@[004:019) Identifier |storageAccounts|
//@[019:020) NewLine |\n|
    moduleCollectionWithSingleDependency
//@[004:040) Identifier |moduleCollectionWithSingleDependency|
//@[040:041) NewLine |\n|
  ]
//@[002:003) RightSquare |]|
//@[003:004) NewLine |\n|
}]
//@[000:001) RightBrace |}|
//@[001:002) RightSquare |]|
//@[002:004) NewLine |\n\n|

module singleModuleWithIndexedDependencies 'passthrough.bicep' = {
//@[000:006) Identifier |module|
//@[007:042) Identifier |singleModuleWithIndexedDependencies|
//@[043:062) StringComplete |'passthrough.bicep'|
//@[063:064) Assignment |=|
//@[065:066) LeftBrace |{|
//@[066:067) NewLine |\n|
  name: 'hello'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:015) StringComplete |'hello'|
//@[015:016) NewLine |\n|
  params: {
//@[002:008) Identifier |params|
//@[008:009) Colon |:|
//@[010:011) LeftBrace |{|
//@[011:012) NewLine |\n|
    myInput: concat(moduleCollectionWithCollectionDependencies[index].outputs.myOutput, storageAccounts[index * 3].properties.accessTier)
//@[004:011) Identifier |myInput|
//@[011:012) Colon |:|
//@[013:019) Identifier |concat|
//@[019:020) LeftParen |(|
//@[020:062) Identifier |moduleCollectionWithCollectionDependencies|
//@[062:063) LeftSquare |[|
//@[063:068) Identifier |index|
//@[068:069) RightSquare |]|
//@[069:070) Dot |.|
//@[070:077) Identifier |outputs|
//@[077:078) Dot |.|
//@[078:086) Identifier |myOutput|
//@[086:087) Comma |,|
//@[088:103) Identifier |storageAccounts|
//@[103:104) LeftSquare |[|
//@[104:109) Identifier |index|
//@[110:111) Asterisk |*|
//@[112:113) Integer |3|
//@[113:114) RightSquare |]|
//@[114:115) Dot |.|
//@[115:125) Identifier |properties|
//@[125:126) Dot |.|
//@[126:136) Identifier |accessTier|
//@[136:137) RightParen |)|
//@[137:138) NewLine |\n|
  }
//@[002:003) RightBrace |}|
//@[003:004) NewLine |\n|
  dependsOn: [
//@[002:011) Identifier |dependsOn|
//@[011:012) Colon |:|
//@[013:014) LeftSquare |[|
//@[014:015) NewLine |\n|
    storageAccounts2[index - 10]
//@[004:020) Identifier |storageAccounts2|
//@[020:021) LeftSquare |[|
//@[021:026) Identifier |index|
//@[027:028) Minus |-|
//@[029:031) Integer |10|
//@[031:032) RightSquare |]|
//@[032:033) NewLine |\n|
  ]
//@[002:003) RightSquare |]|
//@[003:004) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

module moduleCollectionWithIndexedDependencies 'passthrough.bicep' = [for (moduleName, moduleIndex) in moduleSetup: {
//@[000:006) Identifier |module|
//@[007:046) Identifier |moduleCollectionWithIndexedDependencies|
//@[047:066) StringComplete |'passthrough.bicep'|
//@[067:068) Assignment |=|
//@[069:070) LeftSquare |[|
//@[070:073) Identifier |for|
//@[074:075) LeftParen |(|
//@[075:085) Identifier |moduleName|
//@[085:086) Comma |,|
//@[087:098) Identifier |moduleIndex|
//@[098:099) RightParen |)|
//@[100:102) Identifier |in|
//@[103:114) Identifier |moduleSetup|
//@[114:115) Colon |:|
//@[116:117) LeftBrace |{|
//@[117:118) NewLine |\n|
  name: concat(moduleName, moduleIndex)
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:014) Identifier |concat|
//@[014:015) LeftParen |(|
//@[015:025) Identifier |moduleName|
//@[025:026) Comma |,|
//@[027:038) Identifier |moduleIndex|
//@[038:039) RightParen |)|
//@[039:040) NewLine |\n|
  params: {
//@[002:008) Identifier |params|
//@[008:009) Colon |:|
//@[010:011) LeftBrace |{|
//@[011:012) NewLine |\n|
    myInput: '${moduleCollectionWithCollectionDependencies[index].outputs.myOutput} - ${storageAccounts[index * 3].properties.accessTier} - ${moduleName} - ${moduleIndex}'
//@[004:011) Identifier |myInput|
//@[011:012) Colon |:|
//@[013:016) StringLeftPiece |'${|
//@[016:058) Identifier |moduleCollectionWithCollectionDependencies|
//@[058:059) LeftSquare |[|
//@[059:064) Identifier |index|
//@[064:065) RightSquare |]|
//@[065:066) Dot |.|
//@[066:073) Identifier |outputs|
//@[073:074) Dot |.|
//@[074:082) Identifier |myOutput|
//@[082:088) StringMiddlePiece |} - ${|
//@[088:103) Identifier |storageAccounts|
//@[103:104) LeftSquare |[|
//@[104:109) Identifier |index|
//@[110:111) Asterisk |*|
//@[112:113) Integer |3|
//@[113:114) RightSquare |]|
//@[114:115) Dot |.|
//@[115:125) Identifier |properties|
//@[125:126) Dot |.|
//@[126:136) Identifier |accessTier|
//@[136:142) StringMiddlePiece |} - ${|
//@[142:152) Identifier |moduleName|
//@[152:158) StringMiddlePiece |} - ${|
//@[158:169) Identifier |moduleIndex|
//@[169:171) StringRightPiece |}'|
//@[171:172) NewLine |\n|
  }
//@[002:003) RightBrace |}|
//@[003:004) NewLine |\n|
  dependsOn: [
//@[002:011) Identifier |dependsOn|
//@[011:012) Colon |:|
//@[013:014) LeftSquare |[|
//@[014:015) NewLine |\n|
    storageAccounts2[index - 9]
//@[004:020) Identifier |storageAccounts2|
//@[020:021) LeftSquare |[|
//@[021:026) Identifier |index|
//@[027:028) Minus |-|
//@[029:030) Integer |9|
//@[030:031) RightSquare |]|
//@[031:032) NewLine |\n|
  ]
//@[002:003) RightSquare |]|
//@[003:004) NewLine |\n|
}]
//@[000:001) RightBrace |}|
//@[001:002) RightSquare |]|
//@[002:004) NewLine |\n\n|

output indexedModulesName string = moduleCollectionWithSingleDependency[index].name
//@[000:006) Identifier |output|
//@[007:025) Identifier |indexedModulesName|
//@[026:032) Identifier |string|
//@[033:034) Assignment |=|
//@[035:071) Identifier |moduleCollectionWithSingleDependency|
//@[071:072) LeftSquare |[|
//@[072:077) Identifier |index|
//@[077:078) RightSquare |]|
//@[078:079) Dot |.|
//@[079:083) Identifier |name|
//@[083:084) NewLine |\n|
output indexedModuleOutput string = moduleCollectionWithSingleDependency[index * 1].outputs.myOutput
//@[000:006) Identifier |output|
//@[007:026) Identifier |indexedModuleOutput|
//@[027:033) Identifier |string|
//@[034:035) Assignment |=|
//@[036:072) Identifier |moduleCollectionWithSingleDependency|
//@[072:073) LeftSquare |[|
//@[073:078) Identifier |index|
//@[079:080) Asterisk |*|
//@[081:082) Integer |1|
//@[082:083) RightSquare |]|
//@[083:084) Dot |.|
//@[084:091) Identifier |outputs|
//@[091:092) Dot |.|
//@[092:100) Identifier |myOutput|
//@[100:102) NewLine |\n\n|

// resource collection
//@[022:023) NewLine |\n|
resource existingStorageAccounts 'Microsoft.Storage/storageAccounts@2019-06-01' existing = [for (account, i) in accounts: {
//@[000:008) Identifier |resource|
//@[009:032) Identifier |existingStorageAccounts|
//@[033:079) StringComplete |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[080:088) Identifier |existing|
//@[089:090) Assignment |=|
//@[091:092) LeftSquare |[|
//@[092:095) Identifier |for|
//@[096:097) LeftParen |(|
//@[097:104) Identifier |account|
//@[104:105) Comma |,|
//@[106:107) Identifier |i|
//@[107:108) RightParen |)|
//@[109:111) Identifier |in|
//@[112:120) Identifier |accounts|
//@[120:121) Colon |:|
//@[122:123) LeftBrace |{|
//@[123:124) NewLine |\n|
  name: '${name}-existing-${account.name}-${i}'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:011) StringLeftPiece |'${|
//@[011:015) Identifier |name|
//@[015:028) StringMiddlePiece |}-existing-${|
//@[028:035) Identifier |account|
//@[035:036) Dot |.|
//@[036:040) Identifier |name|
//@[040:044) StringMiddlePiece |}-${|
//@[044:045) Identifier |i|
//@[045:047) StringRightPiece |}'|
//@[047:048) NewLine |\n|
}]
//@[000:001) RightBrace |}|
//@[001:002) RightSquare |]|
//@[002:004) NewLine |\n\n|

output existingIndexedResourceName string = existingStorageAccounts[index * 0].name
//@[000:006) Identifier |output|
//@[007:034) Identifier |existingIndexedResourceName|
//@[035:041) Identifier |string|
//@[042:043) Assignment |=|
//@[044:067) Identifier |existingStorageAccounts|
//@[067:068) LeftSquare |[|
//@[068:073) Identifier |index|
//@[074:075) Asterisk |*|
//@[076:077) Integer |0|
//@[077:078) RightSquare |]|
//@[078:079) Dot |.|
//@[079:083) Identifier |name|
//@[083:084) NewLine |\n|
output existingIndexedResourceId string = existingStorageAccounts[index * 1].id
//@[000:006) Identifier |output|
//@[007:032) Identifier |existingIndexedResourceId|
//@[033:039) Identifier |string|
//@[040:041) Assignment |=|
//@[042:065) Identifier |existingStorageAccounts|
//@[065:066) LeftSquare |[|
//@[066:071) Identifier |index|
//@[072:073) Asterisk |*|
//@[074:075) Integer |1|
//@[075:076) RightSquare |]|
//@[076:077) Dot |.|
//@[077:079) Identifier |id|
//@[079:080) NewLine |\n|
output existingIndexedResourceType string = existingStorageAccounts[index+2].type
//@[000:006) Identifier |output|
//@[007:034) Identifier |existingIndexedResourceType|
//@[035:041) Identifier |string|
//@[042:043) Assignment |=|
//@[044:067) Identifier |existingStorageAccounts|
//@[067:068) LeftSquare |[|
//@[068:073) Identifier |index|
//@[073:074) Plus |+|
//@[074:075) Integer |2|
//@[075:076) RightSquare |]|
//@[076:077) Dot |.|
//@[077:081) Identifier |type|
//@[081:082) NewLine |\n|
output existingIndexedResourceApiVersion string = existingStorageAccounts[index-7].apiVersion
//@[000:006) Identifier |output|
//@[007:040) Identifier |existingIndexedResourceApiVersion|
//@[041:047) Identifier |string|
//@[048:049) Assignment |=|
//@[050:073) Identifier |existingStorageAccounts|
//@[073:074) LeftSquare |[|
//@[074:079) Identifier |index|
//@[079:080) Minus |-|
//@[080:081) Integer |7|
//@[081:082) RightSquare |]|
//@[082:083) Dot |.|
//@[083:093) Identifier |apiVersion|
//@[093:094) NewLine |\n|
output existingIndexedResourceLocation string = existingStorageAccounts[index/2].location
//@[000:006) Identifier |output|
//@[007:038) Identifier |existingIndexedResourceLocation|
//@[039:045) Identifier |string|
//@[046:047) Assignment |=|
//@[048:071) Identifier |existingStorageAccounts|
//@[071:072) LeftSquare |[|
//@[072:077) Identifier |index|
//@[077:078) Slash |/|
//@[078:079) Integer |2|
//@[079:080) RightSquare |]|
//@[080:081) Dot |.|
//@[081:089) Identifier |location|
//@[089:090) NewLine |\n|
output existingIndexedResourceAccessTier string = existingStorageAccounts[index%3].properties.accessTier
//@[000:006) Identifier |output|
//@[007:040) Identifier |existingIndexedResourceAccessTier|
//@[041:047) Identifier |string|
//@[048:049) Assignment |=|
//@[050:073) Identifier |existingStorageAccounts|
//@[073:074) LeftSquare |[|
//@[074:079) Identifier |index|
//@[079:080) Modulo |%|
//@[080:081) Integer |3|
//@[081:082) RightSquare |]|
//@[082:083) Dot |.|
//@[083:093) Identifier |properties|
//@[093:094) Dot |.|
//@[094:104) Identifier |accessTier|
//@[104:106) NewLine |\n\n|

resource duplicatedNames 'Microsoft.Network/dnsZones@2018-05-01' = [for (zone,i) in []: {
//@[000:008) Identifier |resource|
//@[009:024) Identifier |duplicatedNames|
//@[025:064) StringComplete |'Microsoft.Network/dnsZones@2018-05-01'|
//@[065:066) Assignment |=|
//@[067:068) LeftSquare |[|
//@[068:071) Identifier |for|
//@[072:073) LeftParen |(|
//@[073:077) Identifier |zone|
//@[077:078) Comma |,|
//@[078:079) Identifier |i|
//@[079:080) RightParen |)|
//@[081:083) Identifier |in|
//@[084:085) LeftSquare |[|
//@[085:086) RightSquare |]|
//@[086:087) Colon |:|
//@[088:089) LeftBrace |{|
//@[089:090) NewLine |\n|
  name: 'no loop variable'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:026) StringComplete |'no loop variable'|
//@[026:027) NewLine |\n|
  location: 'eastus'
//@[002:010) Identifier |location|
//@[010:011) Colon |:|
//@[012:020) StringComplete |'eastus'|
//@[020:021) NewLine |\n|
}]
//@[000:001) RightBrace |}|
//@[001:002) RightSquare |]|
//@[002:004) NewLine |\n\n|

// reference to a resource collection whose name expression does not reference any loop variables
//@[097:098) NewLine |\n|
resource referenceToDuplicateNames 'Microsoft.Network/dnsZones@2018-05-01' = [for (zone,i) in []: {
//@[000:008) Identifier |resource|
//@[009:034) Identifier |referenceToDuplicateNames|
//@[035:074) StringComplete |'Microsoft.Network/dnsZones@2018-05-01'|
//@[075:076) Assignment |=|
//@[077:078) LeftSquare |[|
//@[078:081) Identifier |for|
//@[082:083) LeftParen |(|
//@[083:087) Identifier |zone|
//@[087:088) Comma |,|
//@[088:089) Identifier |i|
//@[089:090) RightParen |)|
//@[091:093) Identifier |in|
//@[094:095) LeftSquare |[|
//@[095:096) RightSquare |]|
//@[096:097) Colon |:|
//@[098:099) LeftBrace |{|
//@[099:100) NewLine |\n|
  name: 'no loop variable 2'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:028) StringComplete |'no loop variable 2'|
//@[028:029) NewLine |\n|
  location: 'eastus'
//@[002:010) Identifier |location|
//@[010:011) Colon |:|
//@[012:020) StringComplete |'eastus'|
//@[020:021) NewLine |\n|
  dependsOn: [
//@[002:011) Identifier |dependsOn|
//@[011:012) Colon |:|
//@[013:014) LeftSquare |[|
//@[014:015) NewLine |\n|
    duplicatedNames[index]
//@[004:019) Identifier |duplicatedNames|
//@[019:020) LeftSquare |[|
//@[020:025) Identifier |index|
//@[025:026) RightSquare |]|
//@[026:027) NewLine |\n|
  ]
//@[002:003) RightSquare |]|
//@[003:004) NewLine |\n|
}]
//@[000:001) RightBrace |}|
//@[001:002) RightSquare |]|
//@[002:004) NewLine |\n\n|

var regions = [
//@[000:003) Identifier |var|
//@[004:011) Identifier |regions|
//@[012:013) Assignment |=|
//@[014:015) LeftSquare |[|
//@[015:016) NewLine |\n|
  'eastus'
//@[002:010) StringComplete |'eastus'|
//@[010:011) NewLine |\n|
  'westus'
//@[002:010) StringComplete |'westus'|
//@[010:011) NewLine |\n|
]
//@[000:001) RightSquare |]|
//@[001:003) NewLine |\n\n|

module apim 'passthrough.bicep' = [for (region, i) in regions: {
//@[000:006) Identifier |module|
//@[007:011) Identifier |apim|
//@[012:031) StringComplete |'passthrough.bicep'|
//@[032:033) Assignment |=|
//@[034:035) LeftSquare |[|
//@[035:038) Identifier |for|
//@[039:040) LeftParen |(|
//@[040:046) Identifier |region|
//@[046:047) Comma |,|
//@[048:049) Identifier |i|
//@[049:050) RightParen |)|
//@[051:053) Identifier |in|
//@[054:061) Identifier |regions|
//@[061:062) Colon |:|
//@[063:064) LeftBrace |{|
//@[064:065) NewLine |\n|
  name: 'apim-${region}-${name}-${i}'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:016) StringLeftPiece |'apim-${|
//@[016:022) Identifier |region|
//@[022:026) StringMiddlePiece |}-${|
//@[026:030) Identifier |name|
//@[030:034) StringMiddlePiece |}-${|
//@[034:035) Identifier |i|
//@[035:037) StringRightPiece |}'|
//@[037:038) NewLine |\n|
  params: {
//@[002:008) Identifier |params|
//@[008:009) Colon |:|
//@[010:011) LeftBrace |{|
//@[011:012) NewLine |\n|
    myInput: region
//@[004:011) Identifier |myInput|
//@[011:012) Colon |:|
//@[013:019) Identifier |region|
//@[019:020) NewLine |\n|
  }
//@[002:003) RightBrace |}|
//@[003:004) NewLine |\n|
}]
//@[000:001) RightBrace |}|
//@[001:002) RightSquare |]|
//@[002:004) NewLine |\n\n|

resource propertyLoopDependencyOnModuleCollection 'Microsoft.Network/frontDoors@2020-05-01' = {
//@[000:008) Identifier |resource|
//@[009:049) Identifier |propertyLoopDependencyOnModuleCollection|
//@[050:091) StringComplete |'Microsoft.Network/frontDoors@2020-05-01'|
//@[092:093) Assignment |=|
//@[094:095) LeftBrace |{|
//@[095:096) NewLine |\n|
  name: name
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:012) Identifier |name|
//@[012:013) NewLine |\n|
  location: 'Global'
//@[002:010) Identifier |location|
//@[010:011) Colon |:|
//@[012:020) StringComplete |'Global'|
//@[020:021) NewLine |\n|
  properties: {
//@[002:012) Identifier |properties|
//@[012:013) Colon |:|
//@[014:015) LeftBrace |{|
//@[015:016) NewLine |\n|
    backendPools: [
//@[004:016) Identifier |backendPools|
//@[016:017) Colon |:|
//@[018:019) LeftSquare |[|
//@[019:020) NewLine |\n|
      {
//@[006:007) LeftBrace |{|
//@[007:008) NewLine |\n|
        name: 'BackendAPIMs'
//@[008:012) Identifier |name|
//@[012:013) Colon |:|
//@[014:028) StringComplete |'BackendAPIMs'|
//@[028:029) NewLine |\n|
        properties: {
//@[008:018) Identifier |properties|
//@[018:019) Colon |:|
//@[020:021) LeftBrace |{|
//@[021:022) NewLine |\n|
          backends: [for (index,i) in range(0, length(regions)): {
//@[010:018) Identifier |backends|
//@[018:019) Colon |:|
//@[020:021) LeftSquare |[|
//@[021:024) Identifier |for|
//@[025:026) LeftParen |(|
//@[026:031) Identifier |index|
//@[031:032) Comma |,|
//@[032:033) Identifier |i|
//@[033:034) RightParen |)|
//@[035:037) Identifier |in|
//@[038:043) Identifier |range|
//@[043:044) LeftParen |(|
//@[044:045) Integer |0|
//@[045:046) Comma |,|
//@[047:053) Identifier |length|
//@[053:054) LeftParen |(|
//@[054:061) Identifier |regions|
//@[061:062) RightParen |)|
//@[062:063) RightParen |)|
//@[063:064) Colon |:|
//@[065:066) LeftBrace |{|
//@[066:067) NewLine |\n|
            // we cannot codegen index correctly because the generated dependsOn property
//@[089:090) NewLine |\n|
            // would be outside of the scope of the property loop
//@[065:066) NewLine |\n|
            // as a result, this will generate a dependency on the entire collection
//@[084:085) NewLine |\n|
            address: apim[index + i].outputs.myOutput
//@[012:019) Identifier |address|
//@[019:020) Colon |:|
//@[021:025) Identifier |apim|
//@[025:026) LeftSquare |[|
//@[026:031) Identifier |index|
//@[032:033) Plus |+|
//@[034:035) Identifier |i|
//@[035:036) RightSquare |]|
//@[036:037) Dot |.|
//@[037:044) Identifier |outputs|
//@[044:045) Dot |.|
//@[045:053) Identifier |myOutput|
//@[053:054) NewLine |\n|
            backendHostHeader: apim[index + i].outputs.myOutput
//@[012:029) Identifier |backendHostHeader|
//@[029:030) Colon |:|
//@[031:035) Identifier |apim|
//@[035:036) LeftSquare |[|
//@[036:041) Identifier |index|
//@[042:043) Plus |+|
//@[044:045) Identifier |i|
//@[045:046) RightSquare |]|
//@[046:047) Dot |.|
//@[047:054) Identifier |outputs|
//@[054:055) Dot |.|
//@[055:063) Identifier |myOutput|
//@[063:064) NewLine |\n|
            httpPort: 80
//@[012:020) Identifier |httpPort|
//@[020:021) Colon |:|
//@[022:024) Integer |80|
//@[024:025) NewLine |\n|
            httpsPort: 443
//@[012:021) Identifier |httpsPort|
//@[021:022) Colon |:|
//@[023:026) Integer |443|
//@[026:027) NewLine |\n|
            priority: 1
//@[012:020) Identifier |priority|
//@[020:021) Colon |:|
//@[022:023) Integer |1|
//@[023:024) NewLine |\n|
            weight: 50
//@[012:018) Identifier |weight|
//@[018:019) Colon |:|
//@[020:022) Integer |50|
//@[022:023) NewLine |\n|
          }]
//@[010:011) RightBrace |}|
//@[011:012) RightSquare |]|
//@[012:013) NewLine |\n|
        }
//@[008:009) RightBrace |}|
//@[009:010) NewLine |\n|
      }
//@[006:007) RightBrace |}|
//@[007:008) NewLine |\n|
    ]
//@[004:005) RightSquare |]|
//@[005:006) NewLine |\n|
  }
//@[002:003) RightBrace |}|
//@[003:004) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

resource indexedModuleCollectionDependency 'Microsoft.Network/frontDoors@2020-05-01' = [for (index, i) in range(0, length(regions)): {
//@[000:008) Identifier |resource|
//@[009:042) Identifier |indexedModuleCollectionDependency|
//@[043:084) StringComplete |'Microsoft.Network/frontDoors@2020-05-01'|
//@[085:086) Assignment |=|
//@[087:088) LeftSquare |[|
//@[088:091) Identifier |for|
//@[092:093) LeftParen |(|
//@[093:098) Identifier |index|
//@[098:099) Comma |,|
//@[100:101) Identifier |i|
//@[101:102) RightParen |)|
//@[103:105) Identifier |in|
//@[106:111) Identifier |range|
//@[111:112) LeftParen |(|
//@[112:113) Integer |0|
//@[113:114) Comma |,|
//@[115:121) Identifier |length|
//@[121:122) LeftParen |(|
//@[122:129) Identifier |regions|
//@[129:130) RightParen |)|
//@[130:131) RightParen |)|
//@[131:132) Colon |:|
//@[133:134) LeftBrace |{|
//@[134:135) NewLine |\n|
  name: '${name}-${index}-${i}'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:011) StringLeftPiece |'${|
//@[011:015) Identifier |name|
//@[015:019) StringMiddlePiece |}-${|
//@[019:024) Identifier |index|
//@[024:028) StringMiddlePiece |}-${|
//@[028:029) Identifier |i|
//@[029:031) StringRightPiece |}'|
//@[031:032) NewLine |\n|
  location: 'Global'
//@[002:010) Identifier |location|
//@[010:011) Colon |:|
//@[012:020) StringComplete |'Global'|
//@[020:021) NewLine |\n|
  properties: {
//@[002:012) Identifier |properties|
//@[012:013) Colon |:|
//@[014:015) LeftBrace |{|
//@[015:016) NewLine |\n|
    backendPools: [
//@[004:016) Identifier |backendPools|
//@[016:017) Colon |:|
//@[018:019) LeftSquare |[|
//@[019:020) NewLine |\n|
      {
//@[006:007) LeftBrace |{|
//@[007:008) NewLine |\n|
        name: 'BackendAPIMs'
//@[008:012) Identifier |name|
//@[012:013) Colon |:|
//@[014:028) StringComplete |'BackendAPIMs'|
//@[028:029) NewLine |\n|
        properties: {
//@[008:018) Identifier |properties|
//@[018:019) Colon |:|
//@[020:021) LeftBrace |{|
//@[021:022) NewLine |\n|
          backends: [
//@[010:018) Identifier |backends|
//@[018:019) Colon |:|
//@[020:021) LeftSquare |[|
//@[021:022) NewLine |\n|
            {
//@[012:013) LeftBrace |{|
//@[013:014) NewLine |\n|
              // this indexed dependency on a module collection will be generated correctly because
//@[099:100) NewLine |\n|
              // copyIndex() can be invoked in the generated dependsOn
//@[070:071) NewLine |\n|
              address: apim[index+i].outputs.myOutput
//@[014:021) Identifier |address|
//@[021:022) Colon |:|
//@[023:027) Identifier |apim|
//@[027:028) LeftSquare |[|
//@[028:033) Identifier |index|
//@[033:034) Plus |+|
//@[034:035) Identifier |i|
//@[035:036) RightSquare |]|
//@[036:037) Dot |.|
//@[037:044) Identifier |outputs|
//@[044:045) Dot |.|
//@[045:053) Identifier |myOutput|
//@[053:054) NewLine |\n|
              backendHostHeader: apim[index+i].outputs.myOutput
//@[014:031) Identifier |backendHostHeader|
//@[031:032) Colon |:|
//@[033:037) Identifier |apim|
//@[037:038) LeftSquare |[|
//@[038:043) Identifier |index|
//@[043:044) Plus |+|
//@[044:045) Identifier |i|
//@[045:046) RightSquare |]|
//@[046:047) Dot |.|
//@[047:054) Identifier |outputs|
//@[054:055) Dot |.|
//@[055:063) Identifier |myOutput|
//@[063:064) NewLine |\n|
              httpPort: 80
//@[014:022) Identifier |httpPort|
//@[022:023) Colon |:|
//@[024:026) Integer |80|
//@[026:027) NewLine |\n|
              httpsPort: 443
//@[014:023) Identifier |httpsPort|
//@[023:024) Colon |:|
//@[025:028) Integer |443|
//@[028:029) NewLine |\n|
              priority: 1
//@[014:022) Identifier |priority|
//@[022:023) Colon |:|
//@[024:025) Integer |1|
//@[025:026) NewLine |\n|
              weight: 50
//@[014:020) Identifier |weight|
//@[020:021) Colon |:|
//@[022:024) Integer |50|
//@[024:025) NewLine |\n|
            }
//@[012:013) RightBrace |}|
//@[013:014) NewLine |\n|
          ]
//@[010:011) RightSquare |]|
//@[011:012) NewLine |\n|
        }
//@[008:009) RightBrace |}|
//@[009:010) NewLine |\n|
      }
//@[006:007) RightBrace |}|
//@[007:008) NewLine |\n|
    ]
//@[004:005) RightSquare |]|
//@[005:006) NewLine |\n|
  }
//@[002:003) RightBrace |}|
//@[003:004) NewLine |\n|
}]
//@[000:001) RightBrace |}|
//@[001:002) RightSquare |]|
//@[002:004) NewLine |\n\n|

resource propertyLoopDependencyOnResourceCollection 'Microsoft.Network/frontDoors@2020-05-01' = {
//@[000:008) Identifier |resource|
//@[009:051) Identifier |propertyLoopDependencyOnResourceCollection|
//@[052:093) StringComplete |'Microsoft.Network/frontDoors@2020-05-01'|
//@[094:095) Assignment |=|
//@[096:097) LeftBrace |{|
//@[097:098) NewLine |\n|
  name: name
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:012) Identifier |name|
//@[012:013) NewLine |\n|
  location: 'Global'
//@[002:010) Identifier |location|
//@[010:011) Colon |:|
//@[012:020) StringComplete |'Global'|
//@[020:021) NewLine |\n|
  properties: {
//@[002:012) Identifier |properties|
//@[012:013) Colon |:|
//@[014:015) LeftBrace |{|
//@[015:016) NewLine |\n|
    backendPools: [
//@[004:016) Identifier |backendPools|
//@[016:017) Colon |:|
//@[018:019) LeftSquare |[|
//@[019:020) NewLine |\n|
      {
//@[006:007) LeftBrace |{|
//@[007:008) NewLine |\n|
        name: 'BackendAPIMs'
//@[008:012) Identifier |name|
//@[012:013) Colon |:|
//@[014:028) StringComplete |'BackendAPIMs'|
//@[028:029) NewLine |\n|
        properties: {
//@[008:018) Identifier |properties|
//@[018:019) Colon |:|
//@[020:021) LeftBrace |{|
//@[021:022) NewLine |\n|
          backends: [for index in range(0, length(accounts)): {
//@[010:018) Identifier |backends|
//@[018:019) Colon |:|
//@[020:021) LeftSquare |[|
//@[021:024) Identifier |for|
//@[025:030) Identifier |index|
//@[031:033) Identifier |in|
//@[034:039) Identifier |range|
//@[039:040) LeftParen |(|
//@[040:041) Integer |0|
//@[041:042) Comma |,|
//@[043:049) Identifier |length|
//@[049:050) LeftParen |(|
//@[050:058) Identifier |accounts|
//@[058:059) RightParen |)|
//@[059:060) RightParen |)|
//@[060:061) Colon |:|
//@[062:063) LeftBrace |{|
//@[063:064) NewLine |\n|
            // we cannot codegen index correctly because the generated dependsOn property
//@[089:090) NewLine |\n|
            // would be outside of the scope of the property loop
//@[065:066) NewLine |\n|
            // as a result, this will generate a dependency on the entire collection
//@[084:085) NewLine |\n|
            address: storageAccounts[index].properties.primaryEndpoints.internetEndpoints.web
//@[012:019) Identifier |address|
//@[019:020) Colon |:|
//@[021:036) Identifier |storageAccounts|
//@[036:037) LeftSquare |[|
//@[037:042) Identifier |index|
//@[042:043) RightSquare |]|
//@[043:044) Dot |.|
//@[044:054) Identifier |properties|
//@[054:055) Dot |.|
//@[055:071) Identifier |primaryEndpoints|
//@[071:072) Dot |.|
//@[072:089) Identifier |internetEndpoints|
//@[089:090) Dot |.|
//@[090:093) Identifier |web|
//@[093:094) NewLine |\n|
            backendHostHeader: storageAccounts[index].properties.primaryEndpoints.internetEndpoints.web
//@[012:029) Identifier |backendHostHeader|
//@[029:030) Colon |:|
//@[031:046) Identifier |storageAccounts|
//@[046:047) LeftSquare |[|
//@[047:052) Identifier |index|
//@[052:053) RightSquare |]|
//@[053:054) Dot |.|
//@[054:064) Identifier |properties|
//@[064:065) Dot |.|
//@[065:081) Identifier |primaryEndpoints|
//@[081:082) Dot |.|
//@[082:099) Identifier |internetEndpoints|
//@[099:100) Dot |.|
//@[100:103) Identifier |web|
//@[103:104) NewLine |\n|
            httpPort: 80
//@[012:020) Identifier |httpPort|
//@[020:021) Colon |:|
//@[022:024) Integer |80|
//@[024:025) NewLine |\n|
            httpsPort: 443
//@[012:021) Identifier |httpsPort|
//@[021:022) Colon |:|
//@[023:026) Integer |443|
//@[026:027) NewLine |\n|
            priority: 1
//@[012:020) Identifier |priority|
//@[020:021) Colon |:|
//@[022:023) Integer |1|
//@[023:024) NewLine |\n|
            weight: 50
//@[012:018) Identifier |weight|
//@[018:019) Colon |:|
//@[020:022) Integer |50|
//@[022:023) NewLine |\n|
          }]
//@[010:011) RightBrace |}|
//@[011:012) RightSquare |]|
//@[012:013) NewLine |\n|
        }
//@[008:009) RightBrace |}|
//@[009:010) NewLine |\n|
      }
//@[006:007) RightBrace |}|
//@[007:008) NewLine |\n|
    ]
//@[004:005) RightSquare |]|
//@[005:006) NewLine |\n|
  }
//@[002:003) RightBrace |}|
//@[003:004) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

resource indexedResourceCollectionDependency 'Microsoft.Network/frontDoors@2020-05-01' = [for (index,i) in range(0, length(accounts)): {
//@[000:008) Identifier |resource|
//@[009:044) Identifier |indexedResourceCollectionDependency|
//@[045:086) StringComplete |'Microsoft.Network/frontDoors@2020-05-01'|
//@[087:088) Assignment |=|
//@[089:090) LeftSquare |[|
//@[090:093) Identifier |for|
//@[094:095) LeftParen |(|
//@[095:100) Identifier |index|
//@[100:101) Comma |,|
//@[101:102) Identifier |i|
//@[102:103) RightParen |)|
//@[104:106) Identifier |in|
//@[107:112) Identifier |range|
//@[112:113) LeftParen |(|
//@[113:114) Integer |0|
//@[114:115) Comma |,|
//@[116:122) Identifier |length|
//@[122:123) LeftParen |(|
//@[123:131) Identifier |accounts|
//@[131:132) RightParen |)|
//@[132:133) RightParen |)|
//@[133:134) Colon |:|
//@[135:136) LeftBrace |{|
//@[136:137) NewLine |\n|
  name: '${name}-${index}-${i}'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:011) StringLeftPiece |'${|
//@[011:015) Identifier |name|
//@[015:019) StringMiddlePiece |}-${|
//@[019:024) Identifier |index|
//@[024:028) StringMiddlePiece |}-${|
//@[028:029) Identifier |i|
//@[029:031) StringRightPiece |}'|
//@[031:032) NewLine |\n|
  location: 'Global'
//@[002:010) Identifier |location|
//@[010:011) Colon |:|
//@[012:020) StringComplete |'Global'|
//@[020:021) NewLine |\n|
  properties: {
//@[002:012) Identifier |properties|
//@[012:013) Colon |:|
//@[014:015) LeftBrace |{|
//@[015:016) NewLine |\n|
    backendPools: [
//@[004:016) Identifier |backendPools|
//@[016:017) Colon |:|
//@[018:019) LeftSquare |[|
//@[019:020) NewLine |\n|
      {
//@[006:007) LeftBrace |{|
//@[007:008) NewLine |\n|
        name: 'BackendAPIMs'
//@[008:012) Identifier |name|
//@[012:013) Colon |:|
//@[014:028) StringComplete |'BackendAPIMs'|
//@[028:029) NewLine |\n|
        properties: {
//@[008:018) Identifier |properties|
//@[018:019) Colon |:|
//@[020:021) LeftBrace |{|
//@[021:022) NewLine |\n|
          backends: [
//@[010:018) Identifier |backends|
//@[018:019) Colon |:|
//@[020:021) LeftSquare |[|
//@[021:022) NewLine |\n|
            {
//@[012:013) LeftBrace |{|
//@[013:014) NewLine |\n|
              // this indexed dependency on a module collection will be generated correctly because
//@[099:100) NewLine |\n|
              // copyIndex() can be invoked in the generated dependsOn
//@[070:071) NewLine |\n|
              address: storageAccounts[index+i].properties.primaryEndpoints.internetEndpoints.web
//@[014:021) Identifier |address|
//@[021:022) Colon |:|
//@[023:038) Identifier |storageAccounts|
//@[038:039) LeftSquare |[|
//@[039:044) Identifier |index|
//@[044:045) Plus |+|
//@[045:046) Identifier |i|
//@[046:047) RightSquare |]|
//@[047:048) Dot |.|
//@[048:058) Identifier |properties|
//@[058:059) Dot |.|
//@[059:075) Identifier |primaryEndpoints|
//@[075:076) Dot |.|
//@[076:093) Identifier |internetEndpoints|
//@[093:094) Dot |.|
//@[094:097) Identifier |web|
//@[097:098) NewLine |\n|
              backendHostHeader: storageAccounts[index+i].properties.primaryEndpoints.internetEndpoints.web
//@[014:031) Identifier |backendHostHeader|
//@[031:032) Colon |:|
//@[033:048) Identifier |storageAccounts|
//@[048:049) LeftSquare |[|
//@[049:054) Identifier |index|
//@[054:055) Plus |+|
//@[055:056) Identifier |i|
//@[056:057) RightSquare |]|
//@[057:058) Dot |.|
//@[058:068) Identifier |properties|
//@[068:069) Dot |.|
//@[069:085) Identifier |primaryEndpoints|
//@[085:086) Dot |.|
//@[086:103) Identifier |internetEndpoints|
//@[103:104) Dot |.|
//@[104:107) Identifier |web|
//@[107:108) NewLine |\n|
              httpPort: 80
//@[014:022) Identifier |httpPort|
//@[022:023) Colon |:|
//@[024:026) Integer |80|
//@[026:027) NewLine |\n|
              httpsPort: 443
//@[014:023) Identifier |httpsPort|
//@[023:024) Colon |:|
//@[025:028) Integer |443|
//@[028:029) NewLine |\n|
              priority: 1
//@[014:022) Identifier |priority|
//@[022:023) Colon |:|
//@[024:025) Integer |1|
//@[025:026) NewLine |\n|
              weight: 50
//@[014:020) Identifier |weight|
//@[020:021) Colon |:|
//@[022:024) Integer |50|
//@[024:025) NewLine |\n|
            }
//@[012:013) RightBrace |}|
//@[013:014) NewLine |\n|
          ]
//@[010:011) RightSquare |]|
//@[011:012) NewLine |\n|
        }
//@[008:009) RightBrace |}|
//@[009:010) NewLine |\n|
      }
//@[006:007) RightBrace |}|
//@[007:008) NewLine |\n|
    ]
//@[004:005) RightSquare |]|
//@[005:006) NewLine |\n|
  }
//@[002:003) RightBrace |}|
//@[003:004) NewLine |\n|
}]
//@[000:001) RightBrace |}|
//@[001:002) RightSquare |]|
//@[002:003) NewLine |\n|

//@[000:000) EndOfFile ||
