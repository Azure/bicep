
//@[000:12434) ProgramSyntax
//@[000:00002) ├─Token(NewLine) |\r\n|
@sys.description('this is basicStorage')
//@[000:00225) ├─ResourceDeclarationSyntax
//@[000:00040) | ├─DecoratorSyntax
//@[000:00001) | | ├─Token(At) |@|
//@[001:00040) | | └─InstanceFunctionCallSyntax
//@[001:00004) | |   ├─VariableAccessSyntax
//@[001:00004) | |   | └─IdentifierSyntax
//@[001:00004) | |   |   └─Token(Identifier) |sys|
//@[004:00005) | |   ├─Token(Dot) |.|
//@[005:00016) | |   ├─IdentifierSyntax
//@[005:00016) | |   | └─Token(Identifier) |description|
//@[016:00017) | |   ├─Token(LeftParen) |(|
//@[017:00039) | |   ├─FunctionArgumentSyntax
//@[017:00039) | |   | └─StringSyntax
//@[017:00039) | |   |   └─Token(StringComplete) |'this is basicStorage'|
//@[039:00040) | |   └─Token(RightParen) |)|
//@[040:00042) | ├─Token(NewLine) |\r\n|
resource basicStorage 'Microsoft.Storage/storageAccounts@2019-06-01' = {
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00021) | ├─IdentifierSyntax
//@[009:00021) | | └─Token(Identifier) |basicStorage|
//@[022:00068) | ├─StringSyntax
//@[022:00068) | | └─Token(StringComplete) |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[069:00070) | ├─Token(Assignment) |=|
//@[071:00183) | └─ObjectSyntax
//@[071:00072) |   ├─Token(LeftBrace) |{|
//@[072:00074) |   ├─Token(NewLine) |\r\n|
  name: 'basicblobs'
//@[002:00020) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00020) |   | └─StringSyntax
//@[008:00020) |   |   └─Token(StringComplete) |'basicblobs'|
//@[020:00022) |   ├─Token(NewLine) |\r\n|
  location: 'westus'
//@[002:00020) |   ├─ObjectPropertySyntax
//@[002:00010) |   | ├─IdentifierSyntax
//@[002:00010) |   | | └─Token(Identifier) |location|
//@[010:00011) |   | ├─Token(Colon) |:|
//@[012:00020) |   | └─StringSyntax
//@[012:00020) |   |   └─Token(StringComplete) |'westus'|
//@[020:00022) |   ├─Token(NewLine) |\r\n|
  kind: 'BlobStorage'
//@[002:00021) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |kind|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00021) |   | └─StringSyntax
//@[008:00021) |   |   └─Token(StringComplete) |'BlobStorage'|
//@[021:00023) |   ├─Token(NewLine) |\r\n|
  sku: {
//@[002:00039) |   ├─ObjectPropertySyntax
//@[002:00005) |   | ├─IdentifierSyntax
//@[002:00005) |   | | └─Token(Identifier) |sku|
//@[005:00006) |   | ├─Token(Colon) |:|
//@[007:00039) |   | └─ObjectSyntax
//@[007:00008) |   |   ├─Token(LeftBrace) |{|
//@[008:00010) |   |   ├─Token(NewLine) |\r\n|
    name: 'Standard_GRS'
//@[004:00024) |   |   ├─ObjectPropertySyntax
//@[004:00008) |   |   | ├─IdentifierSyntax
//@[004:00008) |   |   | | └─Token(Identifier) |name|
//@[008:00009) |   |   | ├─Token(Colon) |:|
//@[010:00024) |   |   | └─StringSyntax
//@[010:00024) |   |   |   └─Token(StringComplete) |'Standard_GRS'|
//@[024:00026) |   |   ├─Token(NewLine) |\r\n|
  }
//@[002:00003) |   |   └─Token(RightBrace) |}|
//@[003:00005) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

@sys.description('this is dnsZone')
//@[000:00140) ├─ResourceDeclarationSyntax
//@[000:00035) | ├─DecoratorSyntax
//@[000:00001) | | ├─Token(At) |@|
//@[001:00035) | | └─InstanceFunctionCallSyntax
//@[001:00004) | |   ├─VariableAccessSyntax
//@[001:00004) | |   | └─IdentifierSyntax
//@[001:00004) | |   |   └─Token(Identifier) |sys|
//@[004:00005) | |   ├─Token(Dot) |.|
//@[005:00016) | |   ├─IdentifierSyntax
//@[005:00016) | |   | └─Token(Identifier) |description|
//@[016:00017) | |   ├─Token(LeftParen) |(|
//@[017:00034) | |   ├─FunctionArgumentSyntax
//@[017:00034) | |   | └─StringSyntax
//@[017:00034) | |   |   └─Token(StringComplete) |'this is dnsZone'|
//@[034:00035) | |   └─Token(RightParen) |)|
//@[035:00037) | ├─Token(NewLine) |\r\n|
resource dnsZone 'Microsoft.Network/dnszones@2018-05-01' = {
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00016) | ├─IdentifierSyntax
//@[009:00016) | | └─Token(Identifier) |dnsZone|
//@[017:00056) | ├─StringSyntax
//@[017:00056) | | └─Token(StringComplete) |'Microsoft.Network/dnszones@2018-05-01'|
//@[057:00058) | ├─Token(Assignment) |=|
//@[059:00103) | └─ObjectSyntax
//@[059:00060) |   ├─Token(LeftBrace) |{|
//@[060:00062) |   ├─Token(NewLine) |\r\n|
  name: 'myZone'
//@[002:00016) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00016) |   | └─StringSyntax
//@[008:00016) |   |   └─Token(StringComplete) |'myZone'|
//@[016:00018) |   ├─Token(NewLine) |\r\n|
  location: 'global'
//@[002:00020) |   ├─ObjectPropertySyntax
//@[002:00010) |   | ├─IdentifierSyntax
//@[002:00010) |   | | └─Token(Identifier) |location|
//@[010:00011) |   | ├─Token(Colon) |:|
//@[012:00020) |   | └─StringSyntax
//@[012:00020) |   |   └─Token(StringComplete) |'global'|
//@[020:00022) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

resource myStorageAccount 'Microsoft.Storage/storageAccounts@2017-10-01' = {
//@[000:00469) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00025) | ├─IdentifierSyntax
//@[009:00025) | | └─Token(Identifier) |myStorageAccount|
//@[026:00072) | ├─StringSyntax
//@[026:00072) | | └─Token(StringComplete) |'Microsoft.Storage/storageAccounts@2017-10-01'|
//@[073:00074) | ├─Token(Assignment) |=|
//@[075:00469) | └─ObjectSyntax
//@[075:00076) |   ├─Token(LeftBrace) |{|
//@[076:00078) |   ├─Token(NewLine) |\r\n|
  name: 'myencryptedone'
//@[002:00024) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00024) |   | └─StringSyntax
//@[008:00024) |   |   └─Token(StringComplete) |'myencryptedone'|
//@[024:00026) |   ├─Token(NewLine) |\r\n|
  location: 'eastus2'
//@[002:00021) |   ├─ObjectPropertySyntax
//@[002:00010) |   | ├─IdentifierSyntax
//@[002:00010) |   | | └─Token(Identifier) |location|
//@[010:00011) |   | ├─Token(Colon) |:|
//@[012:00021) |   | └─StringSyntax
//@[012:00021) |   |   └─Token(StringComplete) |'eastus2'|
//@[021:00023) |   ├─Token(NewLine) |\r\n|
  properties: {
//@[002:00277) |   ├─ObjectPropertySyntax
//@[002:00012) |   | ├─IdentifierSyntax
//@[002:00012) |   | | └─Token(Identifier) |properties|
//@[012:00013) |   | ├─Token(Colon) |:|
//@[014:00277) |   | └─ObjectSyntax
//@[014:00015) |   |   ├─Token(LeftBrace) |{|
//@[015:00017) |   |   ├─Token(NewLine) |\r\n|
    supportsHttpsTrafficOnly: true
//@[004:00034) |   |   ├─ObjectPropertySyntax
//@[004:00028) |   |   | ├─IdentifierSyntax
//@[004:00028) |   |   | | └─Token(Identifier) |supportsHttpsTrafficOnly|
//@[028:00029) |   |   | ├─Token(Colon) |:|
//@[030:00034) |   |   | └─BooleanLiteralSyntax
//@[030:00034) |   |   |   └─Token(TrueKeyword) |true|
//@[034:00036) |   |   ├─Token(NewLine) |\r\n|
    accessTier: 'Hot'
//@[004:00021) |   |   ├─ObjectPropertySyntax
//@[004:00014) |   |   | ├─IdentifierSyntax
//@[004:00014) |   |   | | └─Token(Identifier) |accessTier|
//@[014:00015) |   |   | ├─Token(Colon) |:|
//@[016:00021) |   |   | └─StringSyntax
//@[016:00021) |   |   |   └─Token(StringComplete) |'Hot'|
//@[021:00023) |   |   ├─Token(NewLine) |\r\n|
    encryption: {
//@[004:00196) |   |   ├─ObjectPropertySyntax
//@[004:00014) |   |   | ├─IdentifierSyntax
//@[004:00014) |   |   | | └─Token(Identifier) |encryption|
//@[014:00015) |   |   | ├─Token(Colon) |:|
//@[016:00196) |   |   | └─ObjectSyntax
//@[016:00017) |   |   |   ├─Token(LeftBrace) |{|
//@[017:00019) |   |   |   ├─Token(NewLine) |\r\n|
      keySource: 'Microsoft.Storage'
//@[006:00036) |   |   |   ├─ObjectPropertySyntax
//@[006:00015) |   |   |   | ├─IdentifierSyntax
//@[006:00015) |   |   |   | | └─Token(Identifier) |keySource|
//@[015:00016) |   |   |   | ├─Token(Colon) |:|
//@[017:00036) |   |   |   | └─StringSyntax
//@[017:00036) |   |   |   |   └─Token(StringComplete) |'Microsoft.Storage'|
//@[036:00038) |   |   |   ├─Token(NewLine) |\r\n|
      services: {
//@[006:00132) |   |   |   ├─ObjectPropertySyntax
//@[006:00014) |   |   |   | ├─IdentifierSyntax
//@[006:00014) |   |   |   | | └─Token(Identifier) |services|
//@[014:00015) |   |   |   | ├─Token(Colon) |:|
//@[016:00132) |   |   |   | └─ObjectSyntax
//@[016:00017) |   |   |   |   ├─Token(LeftBrace) |{|
//@[017:00019) |   |   |   |   ├─Token(NewLine) |\r\n|
        blob: {
//@[008:00051) |   |   |   |   ├─ObjectPropertySyntax
//@[008:00012) |   |   |   |   | ├─IdentifierSyntax
//@[008:00012) |   |   |   |   | | └─Token(Identifier) |blob|
//@[012:00013) |   |   |   |   | ├─Token(Colon) |:|
//@[014:00051) |   |   |   |   | └─ObjectSyntax
//@[014:00015) |   |   |   |   |   ├─Token(LeftBrace) |{|
//@[015:00017) |   |   |   |   |   ├─Token(NewLine) |\r\n|
          enabled: true
//@[010:00023) |   |   |   |   |   ├─ObjectPropertySyntax
//@[010:00017) |   |   |   |   |   | ├─IdentifierSyntax
//@[010:00017) |   |   |   |   |   | | └─Token(Identifier) |enabled|
//@[017:00018) |   |   |   |   |   | ├─Token(Colon) |:|
//@[019:00023) |   |   |   |   |   | └─BooleanLiteralSyntax
//@[019:00023) |   |   |   |   |   |   └─Token(TrueKeyword) |true|
//@[023:00025) |   |   |   |   |   ├─Token(NewLine) |\r\n|
        }
//@[008:00009) |   |   |   |   |   └─Token(RightBrace) |}|
//@[009:00011) |   |   |   |   ├─Token(NewLine) |\r\n|
        file: {
//@[008:00051) |   |   |   |   ├─ObjectPropertySyntax
//@[008:00012) |   |   |   |   | ├─IdentifierSyntax
//@[008:00012) |   |   |   |   | | └─Token(Identifier) |file|
//@[012:00013) |   |   |   |   | ├─Token(Colon) |:|
//@[014:00051) |   |   |   |   | └─ObjectSyntax
//@[014:00015) |   |   |   |   |   ├─Token(LeftBrace) |{|
//@[015:00017) |   |   |   |   |   ├─Token(NewLine) |\r\n|
          enabled: true
//@[010:00023) |   |   |   |   |   ├─ObjectPropertySyntax
//@[010:00017) |   |   |   |   |   | ├─IdentifierSyntax
//@[010:00017) |   |   |   |   |   | | └─Token(Identifier) |enabled|
//@[017:00018) |   |   |   |   |   | ├─Token(Colon) |:|
//@[019:00023) |   |   |   |   |   | └─BooleanLiteralSyntax
//@[019:00023) |   |   |   |   |   |   └─Token(TrueKeyword) |true|
//@[023:00025) |   |   |   |   |   ├─Token(NewLine) |\r\n|
        }
//@[008:00009) |   |   |   |   |   └─Token(RightBrace) |}|
//@[009:00011) |   |   |   |   ├─Token(NewLine) |\r\n|
      }
//@[006:00007) |   |   |   |   └─Token(RightBrace) |}|
//@[007:00009) |   |   |   ├─Token(NewLine) |\r\n|
    }
//@[004:00005) |   |   |   └─Token(RightBrace) |}|
//@[005:00007) |   |   ├─Token(NewLine) |\r\n|
  }
//@[002:00003) |   |   └─Token(RightBrace) |}|
//@[003:00005) |   ├─Token(NewLine) |\r\n|
  kind: 'StorageV2'
//@[002:00019) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |kind|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00019) |   | └─StringSyntax
//@[008:00019) |   |   └─Token(StringComplete) |'StorageV2'|
//@[019:00021) |   ├─Token(NewLine) |\r\n|
  sku: {
//@[002:00039) |   ├─ObjectPropertySyntax
//@[002:00005) |   | ├─IdentifierSyntax
//@[002:00005) |   | | └─Token(Identifier) |sku|
//@[005:00006) |   | ├─Token(Colon) |:|
//@[007:00039) |   | └─ObjectSyntax
//@[007:00008) |   |   ├─Token(LeftBrace) |{|
//@[008:00010) |   |   ├─Token(NewLine) |\r\n|
    name: 'Standard_LRS'
//@[004:00024) |   |   ├─ObjectPropertySyntax
//@[004:00008) |   |   | ├─IdentifierSyntax
//@[004:00008) |   |   | | └─Token(Identifier) |name|
//@[008:00009) |   |   | ├─Token(Colon) |:|
//@[010:00024) |   |   | └─StringSyntax
//@[010:00024) |   |   |   └─Token(StringComplete) |'Standard_LRS'|
//@[024:00026) |   |   ├─Token(NewLine) |\r\n|
  }
//@[002:00003) |   |   └─Token(RightBrace) |}|
//@[003:00005) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

resource withExpressions 'Microsoft.Storage/storageAccounts@2017-10-01' = {
//@[000:00539) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00024) | ├─IdentifierSyntax
//@[009:00024) | | └─Token(Identifier) |withExpressions|
//@[025:00071) | ├─StringSyntax
//@[025:00071) | | └─Token(StringComplete) |'Microsoft.Storage/storageAccounts@2017-10-01'|
//@[072:00073) | ├─Token(Assignment) |=|
//@[074:00539) | └─ObjectSyntax
//@[074:00075) |   ├─Token(LeftBrace) |{|
//@[075:00077) |   ├─Token(NewLine) |\r\n|
  name: 'myencryptedone2'
//@[002:00025) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00025) |   | └─StringSyntax
//@[008:00025) |   |   └─Token(StringComplete) |'myencryptedone2'|
//@[025:00027) |   ├─Token(NewLine) |\r\n|
  location: 'eastus2'
//@[002:00021) |   ├─ObjectPropertySyntax
//@[002:00010) |   | ├─IdentifierSyntax
//@[002:00010) |   | | └─Token(Identifier) |location|
//@[010:00011) |   | ├─Token(Colon) |:|
//@[012:00021) |   | └─StringSyntax
//@[012:00021) |   |   └─Token(StringComplete) |'eastus2'|
//@[021:00023) |   ├─Token(NewLine) |\r\n|
  properties: {
//@[002:00304) |   ├─ObjectPropertySyntax
//@[002:00012) |   | ├─IdentifierSyntax
//@[002:00012) |   | | └─Token(Identifier) |properties|
//@[012:00013) |   | ├─Token(Colon) |:|
//@[014:00304) |   | └─ObjectSyntax
//@[014:00015) |   |   ├─Token(LeftBrace) |{|
//@[015:00017) |   |   ├─Token(NewLine) |\r\n|
    supportsHttpsTrafficOnly: !false
//@[004:00036) |   |   ├─ObjectPropertySyntax
//@[004:00028) |   |   | ├─IdentifierSyntax
//@[004:00028) |   |   | | └─Token(Identifier) |supportsHttpsTrafficOnly|
//@[028:00029) |   |   | ├─Token(Colon) |:|
//@[030:00036) |   |   | └─UnaryOperationSyntax
//@[030:00031) |   |   |   ├─Token(Exclamation) |!|
//@[031:00036) |   |   |   └─BooleanLiteralSyntax
//@[031:00036) |   |   |     └─Token(FalseKeyword) |false|
//@[036:00038) |   |   ├─Token(NewLine) |\r\n|
    accessTier: true ? 'Hot' : 'Cold'
//@[004:00037) |   |   ├─ObjectPropertySyntax
//@[004:00014) |   |   | ├─IdentifierSyntax
//@[004:00014) |   |   | | └─Token(Identifier) |accessTier|
//@[014:00015) |   |   | ├─Token(Colon) |:|
//@[016:00037) |   |   | └─TernaryOperationSyntax
//@[016:00020) |   |   |   ├─BooleanLiteralSyntax
//@[016:00020) |   |   |   | └─Token(TrueKeyword) |true|
//@[021:00022) |   |   |   ├─Token(Question) |?|
//@[023:00028) |   |   |   ├─StringSyntax
//@[023:00028) |   |   |   | └─Token(StringComplete) |'Hot'|
//@[029:00030) |   |   |   ├─Token(Colon) |:|
//@[031:00037) |   |   |   └─StringSyntax
//@[031:00037) |   |   |     └─Token(StringComplete) |'Cold'|
//@[037:00039) |   |   ├─Token(NewLine) |\r\n|
    encryption: {
//@[004:00205) |   |   ├─ObjectPropertySyntax
//@[004:00014) |   |   | ├─IdentifierSyntax
//@[004:00014) |   |   | | └─Token(Identifier) |encryption|
//@[014:00015) |   |   | ├─Token(Colon) |:|
//@[016:00205) |   |   | └─ObjectSyntax
//@[016:00017) |   |   |   ├─Token(LeftBrace) |{|
//@[017:00019) |   |   |   ├─Token(NewLine) |\r\n|
      keySource: 'Microsoft.Storage'
//@[006:00036) |   |   |   ├─ObjectPropertySyntax
//@[006:00015) |   |   |   | ├─IdentifierSyntax
//@[006:00015) |   |   |   | | └─Token(Identifier) |keySource|
//@[015:00016) |   |   |   | ├─Token(Colon) |:|
//@[017:00036) |   |   |   | └─StringSyntax
//@[017:00036) |   |   |   |   └─Token(StringComplete) |'Microsoft.Storage'|
//@[036:00038) |   |   |   ├─Token(NewLine) |\r\n|
      services: {
//@[006:00141) |   |   |   ├─ObjectPropertySyntax
//@[006:00014) |   |   |   | ├─IdentifierSyntax
//@[006:00014) |   |   |   | | └─Token(Identifier) |services|
//@[014:00015) |   |   |   | ├─Token(Colon) |:|
//@[016:00141) |   |   |   | └─ObjectSyntax
//@[016:00017) |   |   |   |   ├─Token(LeftBrace) |{|
//@[017:00019) |   |   |   |   ├─Token(NewLine) |\r\n|
        blob: {
//@[008:00060) |   |   |   |   ├─ObjectPropertySyntax
//@[008:00012) |   |   |   |   | ├─IdentifierSyntax
//@[008:00012) |   |   |   |   | | └─Token(Identifier) |blob|
//@[012:00013) |   |   |   |   | ├─Token(Colon) |:|
//@[014:00060) |   |   |   |   | └─ObjectSyntax
//@[014:00015) |   |   |   |   |   ├─Token(LeftBrace) |{|
//@[015:00017) |   |   |   |   |   ├─Token(NewLine) |\r\n|
          enabled: true || false
//@[010:00032) |   |   |   |   |   ├─ObjectPropertySyntax
//@[010:00017) |   |   |   |   |   | ├─IdentifierSyntax
//@[010:00017) |   |   |   |   |   | | └─Token(Identifier) |enabled|
//@[017:00018) |   |   |   |   |   | ├─Token(Colon) |:|
//@[019:00032) |   |   |   |   |   | └─BinaryOperationSyntax
//@[019:00023) |   |   |   |   |   |   ├─BooleanLiteralSyntax
//@[019:00023) |   |   |   |   |   |   | └─Token(TrueKeyword) |true|
//@[024:00026) |   |   |   |   |   |   ├─Token(LogicalOr) ||||
//@[027:00032) |   |   |   |   |   |   └─BooleanLiteralSyntax
//@[027:00032) |   |   |   |   |   |     └─Token(FalseKeyword) |false|
//@[032:00034) |   |   |   |   |   ├─Token(NewLine) |\r\n|
        }
//@[008:00009) |   |   |   |   |   └─Token(RightBrace) |}|
//@[009:00011) |   |   |   |   ├─Token(NewLine) |\r\n|
        file: {
//@[008:00051) |   |   |   |   ├─ObjectPropertySyntax
//@[008:00012) |   |   |   |   | ├─IdentifierSyntax
//@[008:00012) |   |   |   |   | | └─Token(Identifier) |file|
//@[012:00013) |   |   |   |   | ├─Token(Colon) |:|
//@[014:00051) |   |   |   |   | └─ObjectSyntax
//@[014:00015) |   |   |   |   |   ├─Token(LeftBrace) |{|
//@[015:00017) |   |   |   |   |   ├─Token(NewLine) |\r\n|
          enabled: true
//@[010:00023) |   |   |   |   |   ├─ObjectPropertySyntax
//@[010:00017) |   |   |   |   |   | ├─IdentifierSyntax
//@[010:00017) |   |   |   |   |   | | └─Token(Identifier) |enabled|
//@[017:00018) |   |   |   |   |   | ├─Token(Colon) |:|
//@[019:00023) |   |   |   |   |   | └─BooleanLiteralSyntax
//@[019:00023) |   |   |   |   |   |   └─Token(TrueKeyword) |true|
//@[023:00025) |   |   |   |   |   ├─Token(NewLine) |\r\n|
        }
//@[008:00009) |   |   |   |   |   └─Token(RightBrace) |}|
//@[009:00011) |   |   |   |   ├─Token(NewLine) |\r\n|
      }
//@[006:00007) |   |   |   |   └─Token(RightBrace) |}|
//@[007:00009) |   |   |   ├─Token(NewLine) |\r\n|
    }
//@[004:00005) |   |   |   └─Token(RightBrace) |}|
//@[005:00007) |   |   ├─Token(NewLine) |\r\n|
  }
//@[002:00003) |   |   └─Token(RightBrace) |}|
//@[003:00005) |   ├─Token(NewLine) |\r\n|
  kind: 'StorageV2'
//@[002:00019) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |kind|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00019) |   | └─StringSyntax
//@[008:00019) |   |   └─Token(StringComplete) |'StorageV2'|
//@[019:00021) |   ├─Token(NewLine) |\r\n|
  sku: {
//@[002:00039) |   ├─ObjectPropertySyntax
//@[002:00005) |   | ├─IdentifierSyntax
//@[002:00005) |   | | └─Token(Identifier) |sku|
//@[005:00006) |   | ├─Token(Colon) |:|
//@[007:00039) |   | └─ObjectSyntax
//@[007:00008) |   |   ├─Token(LeftBrace) |{|
//@[008:00010) |   |   ├─Token(NewLine) |\r\n|
    name: 'Standard_LRS'
//@[004:00024) |   |   ├─ObjectPropertySyntax
//@[004:00008) |   |   | ├─IdentifierSyntax
//@[004:00008) |   |   | | └─Token(Identifier) |name|
//@[008:00009) |   |   | ├─Token(Colon) |:|
//@[010:00024) |   |   | └─StringSyntax
//@[010:00024) |   |   |   └─Token(StringComplete) |'Standard_LRS'|
//@[024:00026) |   |   ├─Token(NewLine) |\r\n|
  }
//@[002:00003) |   |   └─Token(RightBrace) |}|
//@[003:00005) |   ├─Token(NewLine) |\r\n|
  dependsOn: [
//@[002:00041) |   ├─ObjectPropertySyntax
//@[002:00011) |   | ├─IdentifierSyntax
//@[002:00011) |   | | └─Token(Identifier) |dependsOn|
//@[011:00012) |   | ├─Token(Colon) |:|
//@[013:00041) |   | └─ArraySyntax
//@[013:00014) |   |   ├─Token(LeftSquare) |[|
//@[014:00016) |   |   ├─Token(NewLine) |\r\n|
    myStorageAccount
//@[004:00020) |   |   ├─ArrayItemSyntax
//@[004:00020) |   |   | └─VariableAccessSyntax
//@[004:00020) |   |   |   └─IdentifierSyntax
//@[004:00020) |   |   |     └─Token(Identifier) |myStorageAccount|
//@[020:00022) |   |   ├─Token(NewLine) |\r\n|
  ]
//@[002:00003) |   |   └─Token(RightSquare) |]|
//@[003:00005) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

param applicationName string = 'to-do-app${uniqueString(resourceGroup().id)}'
//@[000:00077) ├─ParameterDeclarationSyntax
//@[000:00005) | ├─Token(Identifier) |param|
//@[006:00021) | ├─IdentifierSyntax
//@[006:00021) | | └─Token(Identifier) |applicationName|
//@[022:00028) | ├─SimpleTypeSyntax
//@[022:00028) | | └─Token(Identifier) |string|
//@[029:00077) | └─ParameterDefaultValueSyntax
//@[029:00030) |   ├─Token(Assignment) |=|
//@[031:00077) |   └─StringSyntax
//@[031:00043) |     ├─Token(StringLeftPiece) |'to-do-app${|
//@[043:00075) |     ├─FunctionCallSyntax
//@[043:00055) |     | ├─IdentifierSyntax
//@[043:00055) |     | | └─Token(Identifier) |uniqueString|
//@[055:00056) |     | ├─Token(LeftParen) |(|
//@[056:00074) |     | ├─FunctionArgumentSyntax
//@[056:00074) |     | | └─PropertyAccessSyntax
//@[056:00071) |     | |   ├─FunctionCallSyntax
//@[056:00069) |     | |   | ├─IdentifierSyntax
//@[056:00069) |     | |   | | └─Token(Identifier) |resourceGroup|
//@[069:00070) |     | |   | ├─Token(LeftParen) |(|
//@[070:00071) |     | |   | └─Token(RightParen) |)|
//@[071:00072) |     | |   ├─Token(Dot) |.|
//@[072:00074) |     | |   └─IdentifierSyntax
//@[072:00074) |     | |     └─Token(Identifier) |id|
//@[074:00075) |     | └─Token(RightParen) |)|
//@[075:00077) |     └─Token(StringRightPiece) |}'|
//@[077:00079) ├─Token(NewLine) |\r\n|
var hostingPlanName = applicationName // why not just use the param directly?
//@[000:00037) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00019) | ├─IdentifierSyntax
//@[004:00019) | | └─Token(Identifier) |hostingPlanName|
//@[020:00021) | ├─Token(Assignment) |=|
//@[022:00037) | └─VariableAccessSyntax
//@[022:00037) |   └─IdentifierSyntax
//@[022:00037) |     └─Token(Identifier) |applicationName|
//@[077:00081) ├─Token(NewLine) |\r\n\r\n|

param appServicePlanTier string
//@[000:00031) ├─ParameterDeclarationSyntax
//@[000:00005) | ├─Token(Identifier) |param|
//@[006:00024) | ├─IdentifierSyntax
//@[006:00024) | | └─Token(Identifier) |appServicePlanTier|
//@[025:00031) | └─SimpleTypeSyntax
//@[025:00031) |   └─Token(Identifier) |string|
//@[031:00033) ├─Token(NewLine) |\r\n|
param appServicePlanInstances int
//@[000:00033) ├─ParameterDeclarationSyntax
//@[000:00005) | ├─Token(Identifier) |param|
//@[006:00029) | ├─IdentifierSyntax
//@[006:00029) | | └─Token(Identifier) |appServicePlanInstances|
//@[030:00033) | └─SimpleTypeSyntax
//@[030:00033) |   └─Token(Identifier) |int|
//@[033:00037) ├─Token(NewLine) |\r\n\r\n|

var location = resourceGroup().location
//@[000:00039) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00012) | ├─IdentifierSyntax
//@[004:00012) | | └─Token(Identifier) |location|
//@[013:00014) | ├─Token(Assignment) |=|
//@[015:00039) | └─PropertyAccessSyntax
//@[015:00030) |   ├─FunctionCallSyntax
//@[015:00028) |   | ├─IdentifierSyntax
//@[015:00028) |   | | └─Token(Identifier) |resourceGroup|
//@[028:00029) |   | ├─Token(LeftParen) |(|
//@[029:00030) |   | └─Token(RightParen) |)|
//@[030:00031) |   ├─Token(Dot) |.|
//@[031:00039) |   └─IdentifierSyntax
//@[031:00039) |     └─Token(Identifier) |location|
//@[039:00043) ├─Token(NewLine) |\r\n\r\n|

resource farm 'Microsoft.Web/serverFarms@2019-08-01' = {
//@[000:00371) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00013) | ├─IdentifierSyntax
//@[009:00013) | | └─Token(Identifier) |farm|
//@[014:00052) | ├─StringSyntax
//@[014:00052) | | └─Token(StringComplete) |'Microsoft.Web/serverFarms@2019-08-01'|
//@[053:00054) | ├─Token(Assignment) |=|
//@[055:00371) | └─ObjectSyntax
//@[055:00056) |   ├─Token(LeftBrace) |{|
//@[056:00058) |   ├─Token(NewLine) |\r\n|
  // dependsOn: resourceId('Microsoft.DocumentDB/databaseAccounts', cosmosAccountName)
//@[086:00088) |   ├─Token(NewLine) |\r\n|
  name: hostingPlanName
//@[002:00023) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00023) |   | └─VariableAccessSyntax
//@[008:00023) |   |   └─IdentifierSyntax
//@[008:00023) |   |     └─Token(Identifier) |hostingPlanName|
//@[023:00025) |   ├─Token(NewLine) |\r\n|
  location: location
//@[002:00020) |   ├─ObjectPropertySyntax
//@[002:00010) |   | ├─IdentifierSyntax
//@[002:00010) |   | | └─Token(Identifier) |location|
//@[010:00011) |   | ├─Token(Colon) |:|
//@[012:00020) |   | └─VariableAccessSyntax
//@[012:00020) |   |   └─IdentifierSyntax
//@[012:00020) |   |     └─Token(Identifier) |location|
//@[020:00022) |   ├─Token(NewLine) |\r\n|
  sku: {
//@[002:00082) |   ├─ObjectPropertySyntax
//@[002:00005) |   | ├─IdentifierSyntax
//@[002:00005) |   | | └─Token(Identifier) |sku|
//@[005:00006) |   | ├─Token(Colon) |:|
//@[007:00082) |   | └─ObjectSyntax
//@[007:00008) |   |   ├─Token(LeftBrace) |{|
//@[008:00010) |   |   ├─Token(NewLine) |\r\n|
    name: appServicePlanTier
//@[004:00028) |   |   ├─ObjectPropertySyntax
//@[004:00008) |   |   | ├─IdentifierSyntax
//@[004:00008) |   |   | | └─Token(Identifier) |name|
//@[008:00009) |   |   | ├─Token(Colon) |:|
//@[010:00028) |   |   | └─VariableAccessSyntax
//@[010:00028) |   |   |   └─IdentifierSyntax
//@[010:00028) |   |   |     └─Token(Identifier) |appServicePlanTier|
//@[028:00030) |   |   ├─Token(NewLine) |\r\n|
    capacity: appServicePlanInstances
//@[004:00037) |   |   ├─ObjectPropertySyntax
//@[004:00012) |   |   | ├─IdentifierSyntax
//@[004:00012) |   |   | | └─Token(Identifier) |capacity|
//@[012:00013) |   |   | ├─Token(Colon) |:|
//@[014:00037) |   |   | └─VariableAccessSyntax
//@[014:00037) |   |   |   └─IdentifierSyntax
//@[014:00037) |   |   |     └─Token(Identifier) |appServicePlanInstances|
//@[037:00039) |   |   ├─Token(NewLine) |\r\n|
  }
//@[002:00003) |   |   └─Token(RightBrace) |}|
//@[003:00005) |   ├─Token(NewLine) |\r\n|
  properties: {
//@[002:00091) |   ├─ObjectPropertySyntax
//@[002:00012) |   | ├─IdentifierSyntax
//@[002:00012) |   | | └─Token(Identifier) |properties|
//@[012:00013) |   | ├─Token(Colon) |:|
//@[014:00091) |   | └─ObjectSyntax
//@[014:00015) |   |   ├─Token(LeftBrace) |{|
//@[015:00017) |   |   ├─Token(NewLine) |\r\n|
    name: hostingPlanName // just hostingPlanName results in an error
//@[004:00025) |   |   ├─ObjectPropertySyntax
//@[004:00008) |   |   | ├─IdentifierSyntax
//@[004:00008) |   |   | | └─Token(Identifier) |name|
//@[008:00009) |   |   | ├─Token(Colon) |:|
//@[010:00025) |   |   | └─VariableAccessSyntax
//@[010:00025) |   |   |   └─IdentifierSyntax
//@[010:00025) |   |   |     └─Token(Identifier) |hostingPlanName|
//@[069:00071) |   |   ├─Token(NewLine) |\r\n|
  }
//@[002:00003) |   |   └─Token(RightBrace) |}|
//@[003:00005) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

var cosmosDbResourceId = resourceId('Microsoft.DocumentDB/databaseAccounts', cosmosDb.account)
//@[000:00094) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00022) | ├─IdentifierSyntax
//@[004:00022) | | └─Token(Identifier) |cosmosDbResourceId|
//@[023:00024) | ├─Token(Assignment) |=|
//@[025:00094) | └─FunctionCallSyntax
//@[025:00035) |   ├─IdentifierSyntax
//@[025:00035) |   | └─Token(Identifier) |resourceId|
//@[035:00036) |   ├─Token(LeftParen) |(|
//@[036:00075) |   ├─FunctionArgumentSyntax
//@[036:00075) |   | └─StringSyntax
//@[036:00075) |   |   └─Token(StringComplete) |'Microsoft.DocumentDB/databaseAccounts'|
//@[075:00076) |   ├─Token(Comma) |,|
//@[077:00093) |   ├─FunctionArgumentSyntax
//@[077:00093) |   | └─PropertyAccessSyntax
//@[077:00085) |   |   ├─VariableAccessSyntax
//@[077:00085) |   |   | └─IdentifierSyntax
//@[077:00085) |   |   |   └─Token(Identifier) |cosmosDb|
//@[085:00086) |   |   ├─Token(Dot) |.|
//@[086:00093) |   |   └─IdentifierSyntax
//@[086:00093) |   |     └─Token(Identifier) |account|
//@[093:00094) |   └─Token(RightParen) |)|
//@[094:00096) ├─Token(NewLine) |\r\n|
var cosmosDbRef = reference(cosmosDbResourceId).documentEndpoint
//@[000:00064) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00015) | ├─IdentifierSyntax
//@[004:00015) | | └─Token(Identifier) |cosmosDbRef|
//@[016:00017) | ├─Token(Assignment) |=|
//@[018:00064) | └─PropertyAccessSyntax
//@[018:00047) |   ├─FunctionCallSyntax
//@[018:00027) |   | ├─IdentifierSyntax
//@[018:00027) |   | | └─Token(Identifier) |reference|
//@[027:00028) |   | ├─Token(LeftParen) |(|
//@[028:00046) |   | ├─FunctionArgumentSyntax
//@[028:00046) |   | | └─VariableAccessSyntax
//@[028:00046) |   | |   └─IdentifierSyntax
//@[028:00046) |   | |     └─Token(Identifier) |cosmosDbResourceId|
//@[046:00047) |   | └─Token(RightParen) |)|
//@[047:00048) |   ├─Token(Dot) |.|
//@[048:00064) |   └─IdentifierSyntax
//@[048:00064) |     └─Token(Identifier) |documentEndpoint|
//@[064:00068) ├─Token(NewLine) |\r\n\r\n|

// this variable is not accessed anywhere in this template and depends on a run-time reference
//@[094:00096) ├─Token(NewLine) |\r\n|
// it should not be present at all in the template output as there is nowhere logical to put it
//@[095:00097) ├─Token(NewLine) |\r\n|
var cosmosDbEndpoint = cosmosDbRef.documentEndpoint
//@[000:00051) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00020) | ├─IdentifierSyntax
//@[004:00020) | | └─Token(Identifier) |cosmosDbEndpoint|
//@[021:00022) | ├─Token(Assignment) |=|
//@[023:00051) | └─PropertyAccessSyntax
//@[023:00034) |   ├─VariableAccessSyntax
//@[023:00034) |   | └─IdentifierSyntax
//@[023:00034) |   |   └─Token(Identifier) |cosmosDbRef|
//@[034:00035) |   ├─Token(Dot) |.|
//@[035:00051) |   └─IdentifierSyntax
//@[035:00051) |     └─Token(Identifier) |documentEndpoint|
//@[051:00055) ├─Token(NewLine) |\r\n\r\n|

param webSiteName string
//@[000:00024) ├─ParameterDeclarationSyntax
//@[000:00005) | ├─Token(Identifier) |param|
//@[006:00017) | ├─IdentifierSyntax
//@[006:00017) | | └─Token(Identifier) |webSiteName|
//@[018:00024) | └─SimpleTypeSyntax
//@[018:00024) |   └─Token(Identifier) |string|
//@[024:00026) ├─Token(NewLine) |\r\n|
param cosmosDb object
//@[000:00021) ├─ParameterDeclarationSyntax
//@[000:00005) | ├─Token(Identifier) |param|
//@[006:00014) | ├─IdentifierSyntax
//@[006:00014) | | └─Token(Identifier) |cosmosDb|
//@[015:00021) | └─SimpleTypeSyntax
//@[015:00021) |   └─Token(Identifier) |object|
//@[021:00023) ├─Token(NewLine) |\r\n|
resource site 'Microsoft.Web/sites@2019-08-01' = {
//@[000:00689) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00013) | ├─IdentifierSyntax
//@[009:00013) | | └─Token(Identifier) |site|
//@[014:00046) | ├─StringSyntax
//@[014:00046) | | └─Token(StringComplete) |'Microsoft.Web/sites@2019-08-01'|
//@[047:00048) | ├─Token(Assignment) |=|
//@[049:00689) | └─ObjectSyntax
//@[049:00050) |   ├─Token(LeftBrace) |{|
//@[050:00052) |   ├─Token(NewLine) |\r\n|
  name: webSiteName
//@[002:00019) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00019) |   | └─VariableAccessSyntax
//@[008:00019) |   |   └─IdentifierSyntax
//@[008:00019) |   |     └─Token(Identifier) |webSiteName|
//@[019:00021) |   ├─Token(NewLine) |\r\n|
  location: location
//@[002:00020) |   ├─ObjectPropertySyntax
//@[002:00010) |   | ├─IdentifierSyntax
//@[002:00010) |   | | └─Token(Identifier) |location|
//@[010:00011) |   | ├─Token(Colon) |:|
//@[012:00020) |   | └─VariableAccessSyntax
//@[012:00020) |   |   └─IdentifierSyntax
//@[012:00020) |   |     └─Token(Identifier) |location|
//@[020:00022) |   ├─Token(NewLine) |\r\n|
  properties: {
//@[002:00591) |   ├─ObjectPropertySyntax
//@[002:00012) |   | ├─IdentifierSyntax
//@[002:00012) |   | | └─Token(Identifier) |properties|
//@[012:00013) |   | ├─Token(Colon) |:|
//@[014:00591) |   | └─ObjectSyntax
//@[014:00015) |   |   ├─Token(LeftBrace) |{|
//@[015:00017) |   |   ├─Token(NewLine) |\r\n|
    // not yet supported // serverFarmId: farm.id
//@[049:00051) |   |   ├─Token(NewLine) |\r\n|
    siteConfig: {
//@[004:00518) |   |   ├─ObjectPropertySyntax
//@[004:00014) |   |   | ├─IdentifierSyntax
//@[004:00014) |   |   | | └─Token(Identifier) |siteConfig|
//@[014:00015) |   |   | ├─Token(Colon) |:|
//@[016:00518) |   |   | └─ObjectSyntax
//@[016:00017) |   |   |   ├─Token(LeftBrace) |{|
//@[017:00019) |   |   |   ├─Token(NewLine) |\r\n|
      appSettings: [
//@[006:00492) |   |   |   ├─ObjectPropertySyntax
//@[006:00017) |   |   |   | ├─IdentifierSyntax
//@[006:00017) |   |   |   | | └─Token(Identifier) |appSettings|
//@[017:00018) |   |   |   | ├─Token(Colon) |:|
//@[019:00492) |   |   |   | └─ArraySyntax
//@[019:00020) |   |   |   |   ├─Token(LeftSquare) |[|
//@[020:00022) |   |   |   |   ├─Token(NewLine) |\r\n|
        {
//@[008:00121) |   |   |   |   ├─ArrayItemSyntax
//@[008:00121) |   |   |   |   | └─ObjectSyntax
//@[008:00009) |   |   |   |   |   ├─Token(LeftBrace) |{|
//@[009:00011) |   |   |   |   |   ├─Token(NewLine) |\r\n|
          name: 'CosmosDb:Account'
//@[010:00034) |   |   |   |   |   ├─ObjectPropertySyntax
//@[010:00014) |   |   |   |   |   | ├─IdentifierSyntax
//@[010:00014) |   |   |   |   |   | | └─Token(Identifier) |name|
//@[014:00015) |   |   |   |   |   | ├─Token(Colon) |:|
//@[016:00034) |   |   |   |   |   | └─StringSyntax
//@[016:00034) |   |   |   |   |   |   └─Token(StringComplete) |'CosmosDb:Account'|
//@[034:00036) |   |   |   |   |   ├─Token(NewLine) |\r\n|
          value: reference(cosmosDbResourceId).documentEndpoint
//@[010:00063) |   |   |   |   |   ├─ObjectPropertySyntax
//@[010:00015) |   |   |   |   |   | ├─IdentifierSyntax
//@[010:00015) |   |   |   |   |   | | └─Token(Identifier) |value|
//@[015:00016) |   |   |   |   |   | ├─Token(Colon) |:|
//@[017:00063) |   |   |   |   |   | └─PropertyAccessSyntax
//@[017:00046) |   |   |   |   |   |   ├─FunctionCallSyntax
//@[017:00026) |   |   |   |   |   |   | ├─IdentifierSyntax
//@[017:00026) |   |   |   |   |   |   | | └─Token(Identifier) |reference|
//@[026:00027) |   |   |   |   |   |   | ├─Token(LeftParen) |(|
//@[027:00045) |   |   |   |   |   |   | ├─FunctionArgumentSyntax
//@[027:00045) |   |   |   |   |   |   | | └─VariableAccessSyntax
//@[027:00045) |   |   |   |   |   |   | |   └─IdentifierSyntax
//@[027:00045) |   |   |   |   |   |   | |     └─Token(Identifier) |cosmosDbResourceId|
//@[045:00046) |   |   |   |   |   |   | └─Token(RightParen) |)|
//@[046:00047) |   |   |   |   |   |   ├─Token(Dot) |.|
//@[047:00063) |   |   |   |   |   |   └─IdentifierSyntax
//@[047:00063) |   |   |   |   |   |     └─Token(Identifier) |documentEndpoint|
//@[063:00065) |   |   |   |   |   ├─Token(NewLine) |\r\n|
        }
//@[008:00009) |   |   |   |   |   └─Token(RightBrace) |}|
//@[009:00011) |   |   |   |   ├─Token(NewLine) |\r\n|
        {
//@[008:00130) |   |   |   |   ├─ArrayItemSyntax
//@[008:00130) |   |   |   |   | └─ObjectSyntax
//@[008:00009) |   |   |   |   |   ├─Token(LeftBrace) |{|
//@[009:00011) |   |   |   |   |   ├─Token(NewLine) |\r\n|
          name: 'CosmosDb:Key'
//@[010:00030) |   |   |   |   |   ├─ObjectPropertySyntax
//@[010:00014) |   |   |   |   |   | ├─IdentifierSyntax
//@[010:00014) |   |   |   |   |   | | └─Token(Identifier) |name|
//@[014:00015) |   |   |   |   |   | ├─Token(Colon) |:|
//@[016:00030) |   |   |   |   |   | └─StringSyntax
//@[016:00030) |   |   |   |   |   |   └─Token(StringComplete) |'CosmosDb:Key'|
//@[030:00032) |   |   |   |   |   ├─Token(NewLine) |\r\n|
          value: listKeys(cosmosDbResourceId, '2020-04-01').primaryMasterKey
//@[010:00076) |   |   |   |   |   ├─ObjectPropertySyntax
//@[010:00015) |   |   |   |   |   | ├─IdentifierSyntax
//@[010:00015) |   |   |   |   |   | | └─Token(Identifier) |value|
//@[015:00016) |   |   |   |   |   | ├─Token(Colon) |:|
//@[017:00076) |   |   |   |   |   | └─PropertyAccessSyntax
//@[017:00059) |   |   |   |   |   |   ├─FunctionCallSyntax
//@[017:00025) |   |   |   |   |   |   | ├─IdentifierSyntax
//@[017:00025) |   |   |   |   |   |   | | └─Token(Identifier) |listKeys|
//@[025:00026) |   |   |   |   |   |   | ├─Token(LeftParen) |(|
//@[026:00044) |   |   |   |   |   |   | ├─FunctionArgumentSyntax
//@[026:00044) |   |   |   |   |   |   | | └─VariableAccessSyntax
//@[026:00044) |   |   |   |   |   |   | |   └─IdentifierSyntax
//@[026:00044) |   |   |   |   |   |   | |     └─Token(Identifier) |cosmosDbResourceId|
//@[044:00045) |   |   |   |   |   |   | ├─Token(Comma) |,|
//@[046:00058) |   |   |   |   |   |   | ├─FunctionArgumentSyntax
//@[046:00058) |   |   |   |   |   |   | | └─StringSyntax
//@[046:00058) |   |   |   |   |   |   | |   └─Token(StringComplete) |'2020-04-01'|
//@[058:00059) |   |   |   |   |   |   | └─Token(RightParen) |)|
//@[059:00060) |   |   |   |   |   |   ├─Token(Dot) |.|
//@[060:00076) |   |   |   |   |   |   └─IdentifierSyntax
//@[060:00076) |   |   |   |   |   |     └─Token(Identifier) |primaryMasterKey|
//@[076:00078) |   |   |   |   |   ├─Token(NewLine) |\r\n|
        }
//@[008:00009) |   |   |   |   |   └─Token(RightBrace) |}|
//@[009:00011) |   |   |   |   ├─Token(NewLine) |\r\n|
        {
//@[008:00101) |   |   |   |   ├─ArrayItemSyntax
//@[008:00101) |   |   |   |   | └─ObjectSyntax
//@[008:00009) |   |   |   |   |   ├─Token(LeftBrace) |{|
//@[009:00011) |   |   |   |   |   ├─Token(NewLine) |\r\n|
          name: 'CosmosDb:DatabaseName'
//@[010:00039) |   |   |   |   |   ├─ObjectPropertySyntax
//@[010:00014) |   |   |   |   |   | ├─IdentifierSyntax
//@[010:00014) |   |   |   |   |   | | └─Token(Identifier) |name|
//@[014:00015) |   |   |   |   |   | ├─Token(Colon) |:|
//@[016:00039) |   |   |   |   |   | └─StringSyntax
//@[016:00039) |   |   |   |   |   |   └─Token(StringComplete) |'CosmosDb:DatabaseName'|
//@[039:00041) |   |   |   |   |   ├─Token(NewLine) |\r\n|
          value: cosmosDb.databaseName
//@[010:00038) |   |   |   |   |   ├─ObjectPropertySyntax
//@[010:00015) |   |   |   |   |   | ├─IdentifierSyntax
//@[010:00015) |   |   |   |   |   | | └─Token(Identifier) |value|
//@[015:00016) |   |   |   |   |   | ├─Token(Colon) |:|
//@[017:00038) |   |   |   |   |   | └─PropertyAccessSyntax
//@[017:00025) |   |   |   |   |   |   ├─VariableAccessSyntax
//@[017:00025) |   |   |   |   |   |   | └─IdentifierSyntax
//@[017:00025) |   |   |   |   |   |   |   └─Token(Identifier) |cosmosDb|
//@[025:00026) |   |   |   |   |   |   ├─Token(Dot) |.|
//@[026:00038) |   |   |   |   |   |   └─IdentifierSyntax
//@[026:00038) |   |   |   |   |   |     └─Token(Identifier) |databaseName|
//@[038:00040) |   |   |   |   |   ├─Token(NewLine) |\r\n|
        }
//@[008:00009) |   |   |   |   |   └─Token(RightBrace) |}|
//@[009:00011) |   |   |   |   ├─Token(NewLine) |\r\n|
        {
//@[008:00103) |   |   |   |   ├─ArrayItemSyntax
//@[008:00103) |   |   |   |   | └─ObjectSyntax
//@[008:00009) |   |   |   |   |   ├─Token(LeftBrace) |{|
//@[009:00011) |   |   |   |   |   ├─Token(NewLine) |\r\n|
          name: 'CosmosDb:ContainerName'
//@[010:00040) |   |   |   |   |   ├─ObjectPropertySyntax
//@[010:00014) |   |   |   |   |   | ├─IdentifierSyntax
//@[010:00014) |   |   |   |   |   | | └─Token(Identifier) |name|
//@[014:00015) |   |   |   |   |   | ├─Token(Colon) |:|
//@[016:00040) |   |   |   |   |   | └─StringSyntax
//@[016:00040) |   |   |   |   |   |   └─Token(StringComplete) |'CosmosDb:ContainerName'|
//@[040:00042) |   |   |   |   |   ├─Token(NewLine) |\r\n|
          value: cosmosDb.containerName
//@[010:00039) |   |   |   |   |   ├─ObjectPropertySyntax
//@[010:00015) |   |   |   |   |   | ├─IdentifierSyntax
//@[010:00015) |   |   |   |   |   | | └─Token(Identifier) |value|
//@[015:00016) |   |   |   |   |   | ├─Token(Colon) |:|
//@[017:00039) |   |   |   |   |   | └─PropertyAccessSyntax
//@[017:00025) |   |   |   |   |   |   ├─VariableAccessSyntax
//@[017:00025) |   |   |   |   |   |   | └─IdentifierSyntax
//@[017:00025) |   |   |   |   |   |   |   └─Token(Identifier) |cosmosDb|
//@[025:00026) |   |   |   |   |   |   ├─Token(Dot) |.|
//@[026:00039) |   |   |   |   |   |   └─IdentifierSyntax
//@[026:00039) |   |   |   |   |   |     └─Token(Identifier) |containerName|
//@[039:00041) |   |   |   |   |   ├─Token(NewLine) |\r\n|
        }
//@[008:00009) |   |   |   |   |   └─Token(RightBrace) |}|
//@[009:00011) |   |   |   |   ├─Token(NewLine) |\r\n|
      ]
//@[006:00007) |   |   |   |   └─Token(RightSquare) |]|
//@[007:00009) |   |   |   ├─Token(NewLine) |\r\n|
    }
//@[004:00005) |   |   |   └─Token(RightBrace) |}|
//@[005:00007) |   |   ├─Token(NewLine) |\r\n|
  }
//@[002:00003) |   |   └─Token(RightBrace) |}|
//@[003:00005) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

var _siteApiVersion = site.apiVersion
//@[000:00037) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00019) | ├─IdentifierSyntax
//@[004:00019) | | └─Token(Identifier) |_siteApiVersion|
//@[020:00021) | ├─Token(Assignment) |=|
//@[022:00037) | └─PropertyAccessSyntax
//@[022:00026) |   ├─VariableAccessSyntax
//@[022:00026) |   | └─IdentifierSyntax
//@[022:00026) |   |   └─Token(Identifier) |site|
//@[026:00027) |   ├─Token(Dot) |.|
//@[027:00037) |   └─IdentifierSyntax
//@[027:00037) |     └─Token(Identifier) |apiVersion|
//@[037:00039) ├─Token(NewLine) |\r\n|
var _siteType = site.type
//@[000:00025) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00013) | ├─IdentifierSyntax
//@[004:00013) | | └─Token(Identifier) |_siteType|
//@[014:00015) | ├─Token(Assignment) |=|
//@[016:00025) | └─PropertyAccessSyntax
//@[016:00020) |   ├─VariableAccessSyntax
//@[016:00020) |   | └─IdentifierSyntax
//@[016:00020) |   |   └─Token(Identifier) |site|
//@[020:00021) |   ├─Token(Dot) |.|
//@[021:00025) |   └─IdentifierSyntax
//@[021:00025) |     └─Token(Identifier) |type|
//@[025:00029) ├─Token(NewLine) |\r\n\r\n|

output siteApiVersion string = site.apiVersion
//@[000:00046) ├─OutputDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |output|
//@[007:00021) | ├─IdentifierSyntax
//@[007:00021) | | └─Token(Identifier) |siteApiVersion|
//@[022:00028) | ├─SimpleTypeSyntax
//@[022:00028) | | └─Token(Identifier) |string|
//@[029:00030) | ├─Token(Assignment) |=|
//@[031:00046) | └─PropertyAccessSyntax
//@[031:00035) |   ├─VariableAccessSyntax
//@[031:00035) |   | └─IdentifierSyntax
//@[031:00035) |   |   └─Token(Identifier) |site|
//@[035:00036) |   ├─Token(Dot) |.|
//@[036:00046) |   └─IdentifierSyntax
//@[036:00046) |     └─Token(Identifier) |apiVersion|
//@[046:00048) ├─Token(NewLine) |\r\n|
output siteType string = site.type
//@[000:00034) ├─OutputDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |output|
//@[007:00015) | ├─IdentifierSyntax
//@[007:00015) | | └─Token(Identifier) |siteType|
//@[016:00022) | ├─SimpleTypeSyntax
//@[016:00022) | | └─Token(Identifier) |string|
//@[023:00024) | ├─Token(Assignment) |=|
//@[025:00034) | └─PropertyAccessSyntax
//@[025:00029) |   ├─VariableAccessSyntax
//@[025:00029) |   | └─IdentifierSyntax
//@[025:00029) |   |   └─Token(Identifier) |site|
//@[029:00030) |   ├─Token(Dot) |.|
//@[030:00034) |   └─IdentifierSyntax
//@[030:00034) |     └─Token(Identifier) |type|
//@[034:00038) ├─Token(NewLine) |\r\n\r\n|

resource nested 'Microsoft.Resources/deployments@2019-10-01' = {
//@[000:00354) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00015) | ├─IdentifierSyntax
//@[009:00015) | | └─Token(Identifier) |nested|
//@[016:00060) | ├─StringSyntax
//@[016:00060) | | └─Token(StringComplete) |'Microsoft.Resources/deployments@2019-10-01'|
//@[061:00062) | ├─Token(Assignment) |=|
//@[063:00354) | └─ObjectSyntax
//@[063:00064) |   ├─Token(LeftBrace) |{|
//@[064:00066) |   ├─Token(NewLine) |\r\n|
  name: 'nestedTemplate1'
//@[002:00025) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00025) |   | └─StringSyntax
//@[008:00025) |   |   └─Token(StringComplete) |'nestedTemplate1'|
//@[025:00027) |   ├─Token(NewLine) |\r\n|
  properties: {
//@[002:00258) |   ├─ObjectPropertySyntax
//@[002:00012) |   | ├─IdentifierSyntax
//@[002:00012) |   | | └─Token(Identifier) |properties|
//@[012:00013) |   | ├─Token(Colon) |:|
//@[014:00258) |   | └─ObjectSyntax
//@[014:00015) |   |   ├─Token(LeftBrace) |{|
//@[015:00017) |   |   ├─Token(NewLine) |\r\n|
    mode: 'Incremental'
//@[004:00023) |   |   ├─ObjectPropertySyntax
//@[004:00008) |   |   | ├─IdentifierSyntax
//@[004:00008) |   |   | | └─Token(Identifier) |mode|
//@[008:00009) |   |   | ├─Token(Colon) |:|
//@[010:00023) |   |   | └─StringSyntax
//@[010:00023) |   |   |   └─Token(StringComplete) |'Incremental'|
//@[023:00025) |   |   ├─Token(NewLine) |\r\n|
    template: {
//@[004:00211) |   |   ├─ObjectPropertySyntax
//@[004:00012) |   |   | ├─IdentifierSyntax
//@[004:00012) |   |   | | └─Token(Identifier) |template|
//@[012:00013) |   |   | ├─Token(Colon) |:|
//@[014:00211) |   |   | └─ObjectSyntax
//@[014:00015) |   |   |   ├─Token(LeftBrace) |{|
//@[015:00017) |   |   |   ├─Token(NewLine) |\r\n|
      // string key value
//@[025:00027) |   |   |   ├─Token(NewLine) |\r\n|
      '$schema': 'https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#'
//@[006:00098) |   |   |   ├─ObjectPropertySyntax
//@[006:00015) |   |   |   | ├─StringSyntax
//@[006:00015) |   |   |   | | └─Token(StringComplete) |'$schema'|
//@[015:00016) |   |   |   | ├─Token(Colon) |:|
//@[017:00098) |   |   |   | └─StringSyntax
//@[017:00098) |   |   |   |   └─Token(StringComplete) |'https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#'|
//@[098:00100) |   |   |   ├─Token(NewLine) |\r\n|
      contentVersion: '1.0.0.0'
//@[006:00031) |   |   |   ├─ObjectPropertySyntax
//@[006:00020) |   |   |   | ├─IdentifierSyntax
//@[006:00020) |   |   |   | | └─Token(Identifier) |contentVersion|
//@[020:00021) |   |   |   | ├─Token(Colon) |:|
//@[022:00031) |   |   |   | └─StringSyntax
//@[022:00031) |   |   |   |   └─Token(StringComplete) |'1.0.0.0'|
//@[031:00033) |   |   |   ├─Token(NewLine) |\r\n|
      resources: [
//@[006:00027) |   |   |   ├─ObjectPropertySyntax
//@[006:00015) |   |   |   | ├─IdentifierSyntax
//@[006:00015) |   |   |   | | └─Token(Identifier) |resources|
//@[015:00016) |   |   |   | ├─Token(Colon) |:|
//@[017:00027) |   |   |   | └─ArraySyntax
//@[017:00018) |   |   |   |   ├─Token(LeftSquare) |[|
//@[018:00020) |   |   |   |   ├─Token(NewLine) |\r\n|
      ]
//@[006:00007) |   |   |   |   └─Token(RightSquare) |]|
//@[007:00009) |   |   |   ├─Token(NewLine) |\r\n|
    }
//@[004:00005) |   |   |   └─Token(RightBrace) |}|
//@[005:00007) |   |   ├─Token(NewLine) |\r\n|
  }
//@[002:00003) |   |   └─Token(RightBrace) |}|
//@[003:00005) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

// should be able to access the read only properties
//@[052:00054) ├─Token(NewLine) |\r\n|
resource accessingReadOnlyProperties 'Microsoft.Foo/foos@2019-10-01' = {
//@[000:00284) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00036) | ├─IdentifierSyntax
//@[009:00036) | | └─Token(Identifier) |accessingReadOnlyProperties|
//@[037:00068) | ├─StringSyntax
//@[037:00068) | | └─Token(StringComplete) |'Microsoft.Foo/foos@2019-10-01'|
//@[069:00070) | ├─Token(Assignment) |=|
//@[071:00284) | └─ObjectSyntax
//@[071:00072) |   ├─Token(LeftBrace) |{|
//@[072:00074) |   ├─Token(NewLine) |\r\n|
  name: 'nestedTemplate1'
//@[002:00025) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00025) |   | └─StringSyntax
//@[008:00025) |   |   └─Token(StringComplete) |'nestedTemplate1'|
//@[025:00027) |   ├─Token(NewLine) |\r\n|
  properties: {
//@[002:00180) |   ├─ObjectPropertySyntax
//@[002:00012) |   | ├─IdentifierSyntax
//@[002:00012) |   | | └─Token(Identifier) |properties|
//@[012:00013) |   | ├─Token(Colon) |:|
//@[014:00180) |   | └─ObjectSyntax
//@[014:00015) |   |   ├─Token(LeftBrace) |{|
//@[015:00017) |   |   ├─Token(NewLine) |\r\n|
    otherId: nested.id
//@[004:00022) |   |   ├─ObjectPropertySyntax
//@[004:00011) |   |   | ├─IdentifierSyntax
//@[004:00011) |   |   | | └─Token(Identifier) |otherId|
//@[011:00012) |   |   | ├─Token(Colon) |:|
//@[013:00022) |   |   | └─PropertyAccessSyntax
//@[013:00019) |   |   |   ├─VariableAccessSyntax
//@[013:00019) |   |   |   | └─IdentifierSyntax
//@[013:00019) |   |   |   |   └─Token(Identifier) |nested|
//@[019:00020) |   |   |   ├─Token(Dot) |.|
//@[020:00022) |   |   |   └─IdentifierSyntax
//@[020:00022) |   |   |     └─Token(Identifier) |id|
//@[022:00024) |   |   ├─Token(NewLine) |\r\n|
    otherName: nested.name
//@[004:00026) |   |   ├─ObjectPropertySyntax
//@[004:00013) |   |   | ├─IdentifierSyntax
//@[004:00013) |   |   | | └─Token(Identifier) |otherName|
//@[013:00014) |   |   | ├─Token(Colon) |:|
//@[015:00026) |   |   | └─PropertyAccessSyntax
//@[015:00021) |   |   |   ├─VariableAccessSyntax
//@[015:00021) |   |   |   | └─IdentifierSyntax
//@[015:00021) |   |   |   |   └─Token(Identifier) |nested|
//@[021:00022) |   |   |   ├─Token(Dot) |.|
//@[022:00026) |   |   |   └─IdentifierSyntax
//@[022:00026) |   |   |     └─Token(Identifier) |name|
//@[026:00028) |   |   ├─Token(NewLine) |\r\n|
    otherVersion: nested.apiVersion
//@[004:00035) |   |   ├─ObjectPropertySyntax
//@[004:00016) |   |   | ├─IdentifierSyntax
//@[004:00016) |   |   | | └─Token(Identifier) |otherVersion|
//@[016:00017) |   |   | ├─Token(Colon) |:|
//@[018:00035) |   |   | └─PropertyAccessSyntax
//@[018:00024) |   |   |   ├─VariableAccessSyntax
//@[018:00024) |   |   |   | └─IdentifierSyntax
//@[018:00024) |   |   |   |   └─Token(Identifier) |nested|
//@[024:00025) |   |   |   ├─Token(Dot) |.|
//@[025:00035) |   |   |   └─IdentifierSyntax
//@[025:00035) |   |   |     └─Token(Identifier) |apiVersion|
//@[035:00037) |   |   ├─Token(NewLine) |\r\n|
    otherType: nested.type
//@[004:00026) |   |   ├─ObjectPropertySyntax
//@[004:00013) |   |   | ├─IdentifierSyntax
//@[004:00013) |   |   | | └─Token(Identifier) |otherType|
//@[013:00014) |   |   | ├─Token(Colon) |:|
//@[015:00026) |   |   | └─PropertyAccessSyntax
//@[015:00021) |   |   |   ├─VariableAccessSyntax
//@[015:00021) |   |   |   | └─IdentifierSyntax
//@[015:00021) |   |   |   |   └─Token(Identifier) |nested|
//@[021:00022) |   |   |   ├─Token(Dot) |.|
//@[022:00026) |   |   |   └─IdentifierSyntax
//@[022:00026) |   |   |     └─Token(Identifier) |type|
//@[026:00030) |   |   ├─Token(NewLine) |\r\n\r\n|

    otherThings: nested.properties.mode
//@[004:00039) |   |   ├─ObjectPropertySyntax
//@[004:00015) |   |   | ├─IdentifierSyntax
//@[004:00015) |   |   | | └─Token(Identifier) |otherThings|
//@[015:00016) |   |   | ├─Token(Colon) |:|
//@[017:00039) |   |   | └─PropertyAccessSyntax
//@[017:00034) |   |   |   ├─PropertyAccessSyntax
//@[017:00023) |   |   |   | ├─VariableAccessSyntax
//@[017:00023) |   |   |   | | └─IdentifierSyntax
//@[017:00023) |   |   |   | |   └─Token(Identifier) |nested|
//@[023:00024) |   |   |   | ├─Token(Dot) |.|
//@[024:00034) |   |   |   | └─IdentifierSyntax
//@[024:00034) |   |   |   |   └─Token(Identifier) |properties|
//@[034:00035) |   |   |   ├─Token(Dot) |.|
//@[035:00039) |   |   |   └─IdentifierSyntax
//@[035:00039) |   |   |     └─Token(Identifier) |mode|
//@[039:00041) |   |   ├─Token(NewLine) |\r\n|
  }
//@[002:00003) |   |   └─Token(RightBrace) |}|
//@[003:00005) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

resource resourceA 'My.Rp/typeA@2020-01-01' = {
//@[000:00071) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00018) | ├─IdentifierSyntax
//@[009:00018) | | └─Token(Identifier) |resourceA|
//@[019:00043) | ├─StringSyntax
//@[019:00043) | | └─Token(StringComplete) |'My.Rp/typeA@2020-01-01'|
//@[044:00045) | ├─Token(Assignment) |=|
//@[046:00071) | └─ObjectSyntax
//@[046:00047) |   ├─Token(LeftBrace) |{|
//@[047:00049) |   ├─Token(NewLine) |\r\n|
  name: 'resourceA'
//@[002:00019) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00019) |   | └─StringSyntax
//@[008:00019) |   |   └─Token(StringComplete) |'resourceA'|
//@[019:00021) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

resource resourceB 'My.Rp/typeA/typeB@2020-01-01' = {
//@[000:00092) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00018) | ├─IdentifierSyntax
//@[009:00018) | | └─Token(Identifier) |resourceB|
//@[019:00049) | ├─StringSyntax
//@[019:00049) | | └─Token(StringComplete) |'My.Rp/typeA/typeB@2020-01-01'|
//@[050:00051) | ├─Token(Assignment) |=|
//@[052:00092) | └─ObjectSyntax
//@[052:00053) |   ├─Token(LeftBrace) |{|
//@[053:00055) |   ├─Token(NewLine) |\r\n|
  name: '${resourceA.name}/myName'
//@[002:00034) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00034) |   | └─StringSyntax
//@[008:00011) |   |   ├─Token(StringLeftPiece) |'${|
//@[011:00025) |   |   ├─PropertyAccessSyntax
//@[011:00020) |   |   | ├─VariableAccessSyntax
//@[011:00020) |   |   | | └─IdentifierSyntax
//@[011:00020) |   |   | |   └─Token(Identifier) |resourceA|
//@[020:00021) |   |   | ├─Token(Dot) |.|
//@[021:00025) |   |   | └─IdentifierSyntax
//@[021:00025) |   |   |   └─Token(Identifier) |name|
//@[025:00034) |   |   └─Token(StringRightPiece) |}/myName'|
//@[034:00036) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

resource resourceC 'My.Rp/typeA/typeB@2020-01-01' = {
//@[000:00269) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00018) | ├─IdentifierSyntax
//@[009:00018) | | └─Token(Identifier) |resourceC|
//@[019:00049) | ├─StringSyntax
//@[019:00049) | | └─Token(StringComplete) |'My.Rp/typeA/typeB@2020-01-01'|
//@[050:00051) | ├─Token(Assignment) |=|
//@[052:00269) | └─ObjectSyntax
//@[052:00053) |   ├─Token(LeftBrace) |{|
//@[053:00055) |   ├─Token(NewLine) |\r\n|
  name: '${resourceA.name}/myName'
//@[002:00034) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00034) |   | └─StringSyntax
//@[008:00011) |   |   ├─Token(StringLeftPiece) |'${|
//@[011:00025) |   |   ├─PropertyAccessSyntax
//@[011:00020) |   |   | ├─VariableAccessSyntax
//@[011:00020) |   |   | | └─IdentifierSyntax
//@[011:00020) |   |   | |   └─Token(Identifier) |resourceA|
//@[020:00021) |   |   | ├─Token(Dot) |.|
//@[021:00025) |   |   | └─IdentifierSyntax
//@[021:00025) |   |   |   └─Token(Identifier) |name|
//@[025:00034) |   |   └─Token(StringRightPiece) |}/myName'|
//@[034:00036) |   ├─Token(NewLine) |\r\n|
  properties: {
//@[002:00175) |   ├─ObjectPropertySyntax
//@[002:00012) |   | ├─IdentifierSyntax
//@[002:00012) |   | | └─Token(Identifier) |properties|
//@[012:00013) |   | ├─Token(Colon) |:|
//@[014:00175) |   | └─ObjectSyntax
//@[014:00015) |   |   ├─Token(LeftBrace) |{|
//@[015:00017) |   |   ├─Token(NewLine) |\r\n|
    aId: resourceA.id
//@[004:00021) |   |   ├─ObjectPropertySyntax
//@[004:00007) |   |   | ├─IdentifierSyntax
//@[004:00007) |   |   | | └─Token(Identifier) |aId|
//@[007:00008) |   |   | ├─Token(Colon) |:|
//@[009:00021) |   |   | └─PropertyAccessSyntax
//@[009:00018) |   |   |   ├─VariableAccessSyntax
//@[009:00018) |   |   |   | └─IdentifierSyntax
//@[009:00018) |   |   |   |   └─Token(Identifier) |resourceA|
//@[018:00019) |   |   |   ├─Token(Dot) |.|
//@[019:00021) |   |   |   └─IdentifierSyntax
//@[019:00021) |   |   |     └─Token(Identifier) |id|
//@[021:00023) |   |   ├─Token(NewLine) |\r\n|
    aType: resourceA.type
//@[004:00025) |   |   ├─ObjectPropertySyntax
//@[004:00009) |   |   | ├─IdentifierSyntax
//@[004:00009) |   |   | | └─Token(Identifier) |aType|
//@[009:00010) |   |   | ├─Token(Colon) |:|
//@[011:00025) |   |   | └─PropertyAccessSyntax
//@[011:00020) |   |   |   ├─VariableAccessSyntax
//@[011:00020) |   |   |   | └─IdentifierSyntax
//@[011:00020) |   |   |   |   └─Token(Identifier) |resourceA|
//@[020:00021) |   |   |   ├─Token(Dot) |.|
//@[021:00025) |   |   |   └─IdentifierSyntax
//@[021:00025) |   |   |     └─Token(Identifier) |type|
//@[025:00027) |   |   ├─Token(NewLine) |\r\n|
    aName: resourceA.name
//@[004:00025) |   |   ├─ObjectPropertySyntax
//@[004:00009) |   |   | ├─IdentifierSyntax
//@[004:00009) |   |   | | └─Token(Identifier) |aName|
//@[009:00010) |   |   | ├─Token(Colon) |:|
//@[011:00025) |   |   | └─PropertyAccessSyntax
//@[011:00020) |   |   |   ├─VariableAccessSyntax
//@[011:00020) |   |   |   | └─IdentifierSyntax
//@[011:00020) |   |   |   |   └─Token(Identifier) |resourceA|
//@[020:00021) |   |   |   ├─Token(Dot) |.|
//@[021:00025) |   |   |   └─IdentifierSyntax
//@[021:00025) |   |   |     └─Token(Identifier) |name|
//@[025:00027) |   |   ├─Token(NewLine) |\r\n|
    aApiVersion: resourceA.apiVersion
//@[004:00037) |   |   ├─ObjectPropertySyntax
//@[004:00015) |   |   | ├─IdentifierSyntax
//@[004:00015) |   |   | | └─Token(Identifier) |aApiVersion|
//@[015:00016) |   |   | ├─Token(Colon) |:|
//@[017:00037) |   |   | └─PropertyAccessSyntax
//@[017:00026) |   |   |   ├─VariableAccessSyntax
//@[017:00026) |   |   |   | └─IdentifierSyntax
//@[017:00026) |   |   |   |   └─Token(Identifier) |resourceA|
//@[026:00027) |   |   |   ├─Token(Dot) |.|
//@[027:00037) |   |   |   └─IdentifierSyntax
//@[027:00037) |   |   |     └─Token(Identifier) |apiVersion|
//@[037:00039) |   |   ├─Token(NewLine) |\r\n|
    bProperties: resourceB.properties
//@[004:00037) |   |   ├─ObjectPropertySyntax
//@[004:00015) |   |   | ├─IdentifierSyntax
//@[004:00015) |   |   | | └─Token(Identifier) |bProperties|
//@[015:00016) |   |   | ├─Token(Colon) |:|
//@[017:00037) |   |   | └─PropertyAccessSyntax
//@[017:00026) |   |   |   ├─VariableAccessSyntax
//@[017:00026) |   |   |   | └─IdentifierSyntax
//@[017:00026) |   |   |   |   └─Token(Identifier) |resourceB|
//@[026:00027) |   |   |   ├─Token(Dot) |.|
//@[027:00037) |   |   |   └─IdentifierSyntax
//@[027:00037) |   |   |     └─Token(Identifier) |properties|
//@[037:00039) |   |   ├─Token(NewLine) |\r\n|
  }
//@[002:00003) |   |   └─Token(RightBrace) |}|
//@[003:00005) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

var varARuntime = {
//@[000:00155) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00015) | ├─IdentifierSyntax
//@[004:00015) | | └─Token(Identifier) |varARuntime|
//@[016:00017) | ├─Token(Assignment) |=|
//@[018:00155) | └─ObjectSyntax
//@[018:00019) |   ├─Token(LeftBrace) |{|
//@[019:00021) |   ├─Token(NewLine) |\r\n|
  bId: resourceB.id
//@[002:00019) |   ├─ObjectPropertySyntax
//@[002:00005) |   | ├─IdentifierSyntax
//@[002:00005) |   | | └─Token(Identifier) |bId|
//@[005:00006) |   | ├─Token(Colon) |:|
//@[007:00019) |   | └─PropertyAccessSyntax
//@[007:00016) |   |   ├─VariableAccessSyntax
//@[007:00016) |   |   | └─IdentifierSyntax
//@[007:00016) |   |   |   └─Token(Identifier) |resourceB|
//@[016:00017) |   |   ├─Token(Dot) |.|
//@[017:00019) |   |   └─IdentifierSyntax
//@[017:00019) |   |     └─Token(Identifier) |id|
//@[019:00021) |   ├─Token(NewLine) |\r\n|
  bType: resourceB.type
//@[002:00023) |   ├─ObjectPropertySyntax
//@[002:00007) |   | ├─IdentifierSyntax
//@[002:00007) |   | | └─Token(Identifier) |bType|
//@[007:00008) |   | ├─Token(Colon) |:|
//@[009:00023) |   | └─PropertyAccessSyntax
//@[009:00018) |   |   ├─VariableAccessSyntax
//@[009:00018) |   |   | └─IdentifierSyntax
//@[009:00018) |   |   |   └─Token(Identifier) |resourceB|
//@[018:00019) |   |   ├─Token(Dot) |.|
//@[019:00023) |   |   └─IdentifierSyntax
//@[019:00023) |   |     └─Token(Identifier) |type|
//@[023:00025) |   ├─Token(NewLine) |\r\n|
  bName: resourceB.name
//@[002:00023) |   ├─ObjectPropertySyntax
//@[002:00007) |   | ├─IdentifierSyntax
//@[002:00007) |   | | └─Token(Identifier) |bName|
//@[007:00008) |   | ├─Token(Colon) |:|
//@[009:00023) |   | └─PropertyAccessSyntax
//@[009:00018) |   |   ├─VariableAccessSyntax
//@[009:00018) |   |   | └─IdentifierSyntax
//@[009:00018) |   |   |   └─Token(Identifier) |resourceB|
//@[018:00019) |   |   ├─Token(Dot) |.|
//@[019:00023) |   |   └─IdentifierSyntax
//@[019:00023) |   |     └─Token(Identifier) |name|
//@[023:00025) |   ├─Token(NewLine) |\r\n|
  bApiVersion: resourceB.apiVersion
//@[002:00035) |   ├─ObjectPropertySyntax
//@[002:00013) |   | ├─IdentifierSyntax
//@[002:00013) |   | | └─Token(Identifier) |bApiVersion|
//@[013:00014) |   | ├─Token(Colon) |:|
//@[015:00035) |   | └─PropertyAccessSyntax
//@[015:00024) |   |   ├─VariableAccessSyntax
//@[015:00024) |   |   | └─IdentifierSyntax
//@[015:00024) |   |   |   └─Token(Identifier) |resourceB|
//@[024:00025) |   |   ├─Token(Dot) |.|
//@[025:00035) |   |   └─IdentifierSyntax
//@[025:00035) |   |     └─Token(Identifier) |apiVersion|
//@[035:00037) |   ├─Token(NewLine) |\r\n|
  aKind: resourceA.kind
//@[002:00023) |   ├─ObjectPropertySyntax
//@[002:00007) |   | ├─IdentifierSyntax
//@[002:00007) |   | | └─Token(Identifier) |aKind|
//@[007:00008) |   | ├─Token(Colon) |:|
//@[009:00023) |   | └─PropertyAccessSyntax
//@[009:00018) |   |   ├─VariableAccessSyntax
//@[009:00018) |   |   | └─IdentifierSyntax
//@[009:00018) |   |   |   └─Token(Identifier) |resourceA|
//@[018:00019) |   |   ├─Token(Dot) |.|
//@[019:00023) |   |   └─IdentifierSyntax
//@[019:00023) |   |     └─Token(Identifier) |kind|
//@[023:00025) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

var varBRuntime = [
//@[000:00037) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00015) | ├─IdentifierSyntax
//@[004:00015) | | └─Token(Identifier) |varBRuntime|
//@[016:00017) | ├─Token(Assignment) |=|
//@[018:00037) | └─ArraySyntax
//@[018:00019) |   ├─Token(LeftSquare) |[|
//@[019:00021) |   ├─Token(NewLine) |\r\n|
  varARuntime
//@[002:00013) |   ├─ArrayItemSyntax
//@[002:00013) |   | └─VariableAccessSyntax
//@[002:00013) |   |   └─IdentifierSyntax
//@[002:00013) |   |     └─Token(Identifier) |varARuntime|
//@[013:00015) |   ├─Token(NewLine) |\r\n|
]
//@[000:00001) |   └─Token(RightSquare) |]|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

var resourceCRef = {
//@[000:00043) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00016) | ├─IdentifierSyntax
//@[004:00016) | | └─Token(Identifier) |resourceCRef|
//@[017:00018) | ├─Token(Assignment) |=|
//@[019:00043) | └─ObjectSyntax
//@[019:00020) |   ├─Token(LeftBrace) |{|
//@[020:00022) |   ├─Token(NewLine) |\r\n|
  id: resourceC.id
//@[002:00018) |   ├─ObjectPropertySyntax
//@[002:00004) |   | ├─IdentifierSyntax
//@[002:00004) |   | | └─Token(Identifier) |id|
//@[004:00005) |   | ├─Token(Colon) |:|
//@[006:00018) |   | └─PropertyAccessSyntax
//@[006:00015) |   |   ├─VariableAccessSyntax
//@[006:00015) |   |   | └─IdentifierSyntax
//@[006:00015) |   |   |   └─Token(Identifier) |resourceC|
//@[015:00016) |   |   ├─Token(Dot) |.|
//@[016:00018) |   |   └─IdentifierSyntax
//@[016:00018) |   |     └─Token(Identifier) |id|
//@[018:00020) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00003) ├─Token(NewLine) |\r\n|
var setResourceCRef = true
//@[000:00026) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00019) | ├─IdentifierSyntax
//@[004:00019) | | └─Token(Identifier) |setResourceCRef|
//@[020:00021) | ├─Token(Assignment) |=|
//@[022:00026) | └─BooleanLiteralSyntax
//@[022:00026) |   └─Token(TrueKeyword) |true|
//@[026:00030) ├─Token(NewLine) |\r\n\r\n|

resource resourceD 'My.Rp/typeD@2020-01-01' = {
//@[000:00231) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00018) | ├─IdentifierSyntax
//@[009:00018) | | └─Token(Identifier) |resourceD|
//@[019:00043) | ├─StringSyntax
//@[019:00043) | | └─Token(StringComplete) |'My.Rp/typeD@2020-01-01'|
//@[044:00045) | ├─Token(Assignment) |=|
//@[046:00231) | └─ObjectSyntax
//@[046:00047) |   ├─Token(LeftBrace) |{|
//@[047:00049) |   ├─Token(NewLine) |\r\n|
  name: 'constant'
//@[002:00018) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00018) |   | └─StringSyntax
//@[008:00018) |   |   └─Token(StringComplete) |'constant'|
//@[018:00020) |   ├─Token(NewLine) |\r\n|
  properties: {
//@[002:00159) |   ├─ObjectPropertySyntax
//@[002:00012) |   | ├─IdentifierSyntax
//@[002:00012) |   | | └─Token(Identifier) |properties|
//@[012:00013) |   | ├─Token(Colon) |:|
//@[014:00159) |   | └─ObjectSyntax
//@[014:00015) |   |   ├─Token(LeftBrace) |{|
//@[015:00017) |   |   ├─Token(NewLine) |\r\n|
    runtime: varBRuntime
//@[004:00024) |   |   ├─ObjectPropertySyntax
//@[004:00011) |   |   | ├─IdentifierSyntax
//@[004:00011) |   |   | | └─Token(Identifier) |runtime|
//@[011:00012) |   |   | ├─Token(Colon) |:|
//@[013:00024) |   |   | └─VariableAccessSyntax
//@[013:00024) |   |   |   └─IdentifierSyntax
//@[013:00024) |   |   |     └─Token(Identifier) |varBRuntime|
//@[024:00026) |   |   ├─Token(NewLine) |\r\n|
    // repro for https://github.com/Azure/bicep/issues/316
//@[058:00060) |   |   ├─Token(NewLine) |\r\n|
    repro316: setResourceCRef ? resourceCRef : null
//@[004:00051) |   |   ├─ObjectPropertySyntax
//@[004:00012) |   |   | ├─IdentifierSyntax
//@[004:00012) |   |   | | └─Token(Identifier) |repro316|
//@[012:00013) |   |   | ├─Token(Colon) |:|
//@[014:00051) |   |   | └─TernaryOperationSyntax
//@[014:00029) |   |   |   ├─VariableAccessSyntax
//@[014:00029) |   |   |   | └─IdentifierSyntax
//@[014:00029) |   |   |   |   └─Token(Identifier) |setResourceCRef|
//@[030:00031) |   |   |   ├─Token(Question) |?|
//@[032:00044) |   |   |   ├─VariableAccessSyntax
//@[032:00044) |   |   |   | └─IdentifierSyntax
//@[032:00044) |   |   |   |   └─Token(Identifier) |resourceCRef|
//@[045:00046) |   |   |   ├─Token(Colon) |:|
//@[047:00051) |   |   |   └─NullLiteralSyntax
//@[047:00051) |   |   |     └─Token(NullKeyword) |null|
//@[051:00053) |   |   ├─Token(NewLine) |\r\n|
  }
//@[002:00003) |   |   └─Token(RightBrace) |}|
//@[003:00005) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

var myInterpKey = 'abc'
//@[000:00023) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00015) | ├─IdentifierSyntax
//@[004:00015) | | └─Token(Identifier) |myInterpKey|
//@[016:00017) | ├─Token(Assignment) |=|
//@[018:00023) | └─StringSyntax
//@[018:00023) |   └─Token(StringComplete) |'abc'|
//@[023:00025) ├─Token(NewLine) |\r\n|
resource resourceWithInterp 'My.Rp/interp@2020-01-01' = {
//@[000:00202) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00027) | ├─IdentifierSyntax
//@[009:00027) | | └─Token(Identifier) |resourceWithInterp|
//@[028:00053) | ├─StringSyntax
//@[028:00053) | | └─Token(StringComplete) |'My.Rp/interp@2020-01-01'|
//@[054:00055) | ├─Token(Assignment) |=|
//@[056:00202) | └─ObjectSyntax
//@[056:00057) |   ├─Token(LeftBrace) |{|
//@[057:00059) |   ├─Token(NewLine) |\r\n|
  name: 'interpTest'
//@[002:00020) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00020) |   | └─StringSyntax
//@[008:00020) |   |   └─Token(StringComplete) |'interpTest'|
//@[020:00022) |   ├─Token(NewLine) |\r\n|
  properties: {
//@[002:00118) |   ├─ObjectPropertySyntax
//@[002:00012) |   | ├─IdentifierSyntax
//@[002:00012) |   | | └─Token(Identifier) |properties|
//@[012:00013) |   | ├─Token(Colon) |:|
//@[014:00118) |   | └─ObjectSyntax
//@[014:00015) |   |   ├─Token(LeftBrace) |{|
//@[015:00017) |   |   ├─Token(NewLine) |\r\n|
    '${myInterpKey}': 1
//@[004:00023) |   |   ├─ObjectPropertySyntax
//@[004:00020) |   |   | ├─StringSyntax
//@[004:00007) |   |   | | ├─Token(StringLeftPiece) |'${|
//@[007:00018) |   |   | | ├─VariableAccessSyntax
//@[007:00018) |   |   | | | └─IdentifierSyntax
//@[007:00018) |   |   | | |   └─Token(Identifier) |myInterpKey|
//@[018:00020) |   |   | | └─Token(StringRightPiece) |}'|
//@[020:00021) |   |   | ├─Token(Colon) |:|
//@[022:00023) |   |   | └─IntegerLiteralSyntax
//@[022:00023) |   |   |   └─Token(Integer) |1|
//@[023:00025) |   |   ├─Token(NewLine) |\r\n|
    'abc${myInterpKey}def': 2
//@[004:00029) |   |   ├─ObjectPropertySyntax
//@[004:00026) |   |   | ├─StringSyntax
//@[004:00010) |   |   | | ├─Token(StringLeftPiece) |'abc${|
//@[010:00021) |   |   | | ├─VariableAccessSyntax
//@[010:00021) |   |   | | | └─IdentifierSyntax
//@[010:00021) |   |   | | |   └─Token(Identifier) |myInterpKey|
//@[021:00026) |   |   | | └─Token(StringRightPiece) |}def'|
//@[026:00027) |   |   | ├─Token(Colon) |:|
//@[028:00029) |   |   | └─IntegerLiteralSyntax
//@[028:00029) |   |   |   └─Token(Integer) |2|
//@[029:00031) |   |   ├─Token(NewLine) |\r\n|
    '${myInterpKey}abc${myInterpKey}': 3
//@[004:00040) |   |   ├─ObjectPropertySyntax
//@[004:00037) |   |   | ├─StringSyntax
//@[004:00007) |   |   | | ├─Token(StringLeftPiece) |'${|
//@[007:00018) |   |   | | ├─VariableAccessSyntax
//@[007:00018) |   |   | | | └─IdentifierSyntax
//@[007:00018) |   |   | | |   └─Token(Identifier) |myInterpKey|
//@[018:00024) |   |   | | ├─Token(StringMiddlePiece) |}abc${|
//@[024:00035) |   |   | | ├─VariableAccessSyntax
//@[024:00035) |   |   | | | └─IdentifierSyntax
//@[024:00035) |   |   | | |   └─Token(Identifier) |myInterpKey|
//@[035:00037) |   |   | | └─Token(StringRightPiece) |}'|
//@[037:00038) |   |   | ├─Token(Colon) |:|
//@[039:00040) |   |   | └─IntegerLiteralSyntax
//@[039:00040) |   |   |   └─Token(Integer) |3|
//@[040:00042) |   |   ├─Token(NewLine) |\r\n|
  }
//@[002:00003) |   |   └─Token(RightBrace) |}|
//@[003:00005) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

resource resourceWithEscaping 'My.Rp/mockResource@2020-01-01' = {
//@[000:00234) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00029) | ├─IdentifierSyntax
//@[009:00029) | | └─Token(Identifier) |resourceWithEscaping|
//@[030:00061) | ├─StringSyntax
//@[030:00061) | | └─Token(StringComplete) |'My.Rp/mockResource@2020-01-01'|
//@[062:00063) | ├─Token(Assignment) |=|
//@[064:00234) | └─ObjectSyntax
//@[064:00065) |   ├─Token(LeftBrace) |{|
//@[065:00067) |   ├─Token(NewLine) |\r\n|
  name: 'test'
//@[002:00014) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00014) |   | └─StringSyntax
//@[008:00014) |   |   └─Token(StringComplete) |'test'|
//@[014:00016) |   ├─Token(NewLine) |\r\n|
  properties: {
//@[002:00148) |   ├─ObjectPropertySyntax
//@[002:00012) |   | ├─IdentifierSyntax
//@[002:00012) |   | | └─Token(Identifier) |properties|
//@[012:00013) |   | ├─Token(Colon) |:|
//@[014:00148) |   | └─ObjectSyntax
//@[014:00015) |   |   ├─Token(LeftBrace) |{|
//@[015:00017) |   |   ├─Token(NewLine) |\r\n|
    // both key and value should be escaped in template output
//@[062:00064) |   |   ├─Token(NewLine) |\r\n|
    '[resourceGroup().location]': '[resourceGroup().location]'
//@[004:00062) |   |   ├─ObjectPropertySyntax
//@[004:00032) |   |   | ├─StringSyntax
//@[004:00032) |   |   | | └─Token(StringComplete) |'[resourceGroup().location]'|
//@[032:00033) |   |   | ├─Token(Colon) |:|
//@[034:00062) |   |   | └─StringSyntax
//@[034:00062) |   |   |   └─Token(StringComplete) |'[resourceGroup().location]'|
//@[062:00064) |   |   ├─Token(NewLine) |\r\n|
  }
//@[002:00003) |   |   └─Token(RightBrace) |}|
//@[003:00005) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

param shouldDeployVm bool = true
//@[000:00032) ├─ParameterDeclarationSyntax
//@[000:00005) | ├─Token(Identifier) |param|
//@[006:00020) | ├─IdentifierSyntax
//@[006:00020) | | └─Token(Identifier) |shouldDeployVm|
//@[021:00025) | ├─SimpleTypeSyntax
//@[021:00025) | | └─Token(Identifier) |bool|
//@[026:00032) | └─ParameterDefaultValueSyntax
//@[026:00027) |   ├─Token(Assignment) |=|
//@[028:00032) |   └─BooleanLiteralSyntax
//@[028:00032) |     └─Token(TrueKeyword) |true|
//@[032:00036) ├─Token(NewLine) |\r\n\r\n|

@sys.description('this is vmWithCondition')
//@[000:00308) ├─ResourceDeclarationSyntax
//@[000:00043) | ├─DecoratorSyntax
//@[000:00001) | | ├─Token(At) |@|
//@[001:00043) | | └─InstanceFunctionCallSyntax
//@[001:00004) | |   ├─VariableAccessSyntax
//@[001:00004) | |   | └─IdentifierSyntax
//@[001:00004) | |   |   └─Token(Identifier) |sys|
//@[004:00005) | |   ├─Token(Dot) |.|
//@[005:00016) | |   ├─IdentifierSyntax
//@[005:00016) | |   | └─Token(Identifier) |description|
//@[016:00017) | |   ├─Token(LeftParen) |(|
//@[017:00042) | |   ├─FunctionArgumentSyntax
//@[017:00042) | |   | └─StringSyntax
//@[017:00042) | |   |   └─Token(StringComplete) |'this is vmWithCondition'|
//@[042:00043) | |   └─Token(RightParen) |)|
//@[043:00045) | ├─Token(NewLine) |\r\n|
resource vmWithCondition 'Microsoft.Compute/virtualMachines@2020-06-01' = if (shouldDeployVm) {
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00024) | ├─IdentifierSyntax
//@[009:00024) | | └─Token(Identifier) |vmWithCondition|
//@[025:00071) | ├─StringSyntax
//@[025:00071) | | └─Token(StringComplete) |'Microsoft.Compute/virtualMachines@2020-06-01'|
//@[072:00073) | ├─Token(Assignment) |=|
//@[074:00263) | └─IfConditionSyntax
//@[074:00076) |   ├─Token(Identifier) |if|
//@[077:00093) |   ├─ParenthesizedExpressionSyntax
//@[077:00078) |   | ├─Token(LeftParen) |(|
//@[078:00092) |   | ├─VariableAccessSyntax
//@[078:00092) |   | | └─IdentifierSyntax
//@[078:00092) |   | |   └─Token(Identifier) |shouldDeployVm|
//@[092:00093) |   | └─Token(RightParen) |)|
//@[094:00263) |   └─ObjectSyntax
//@[094:00095) |     ├─Token(LeftBrace) |{|
//@[095:00097) |     ├─Token(NewLine) |\r\n|
  name: 'vmName'
//@[002:00016) |     ├─ObjectPropertySyntax
//@[002:00006) |     | ├─IdentifierSyntax
//@[002:00006) |     | | └─Token(Identifier) |name|
//@[006:00007) |     | ├─Token(Colon) |:|
//@[008:00016) |     | └─StringSyntax
//@[008:00016) |     |   └─Token(StringComplete) |'vmName'|
//@[016:00018) |     ├─Token(NewLine) |\r\n|
  location: 'westus'
//@[002:00020) |     ├─ObjectPropertySyntax
//@[002:00010) |     | ├─IdentifierSyntax
//@[002:00010) |     | | └─Token(Identifier) |location|
//@[010:00011) |     | ├─Token(Colon) |:|
//@[012:00020) |     | └─StringSyntax
//@[012:00020) |     |   └─Token(StringComplete) |'westus'|
//@[020:00022) |     ├─Token(NewLine) |\r\n|
  properties: {
//@[002:00123) |     ├─ObjectPropertySyntax
//@[002:00012) |     | ├─IdentifierSyntax
//@[002:00012) |     | | └─Token(Identifier) |properties|
//@[012:00013) |     | ├─Token(Colon) |:|
//@[014:00123) |     | └─ObjectSyntax
//@[014:00015) |     |   ├─Token(LeftBrace) |{|
//@[015:00017) |     |   ├─Token(NewLine) |\r\n|
    osProfile: {
//@[004:00101) |     |   ├─ObjectPropertySyntax
//@[004:00013) |     |   | ├─IdentifierSyntax
//@[004:00013) |     |   | | └─Token(Identifier) |osProfile|
//@[013:00014) |     |   | ├─Token(Colon) |:|
//@[015:00101) |     |   | └─ObjectSyntax
//@[015:00016) |     |   |   ├─Token(LeftBrace) |{|
//@[016:00018) |     |   |   ├─Token(NewLine) |\r\n|
      windowsConfiguration: {
//@[006:00076) |     |   |   ├─ObjectPropertySyntax
//@[006:00026) |     |   |   | ├─IdentifierSyntax
//@[006:00026) |     |   |   | | └─Token(Identifier) |windowsConfiguration|
//@[026:00027) |     |   |   | ├─Token(Colon) |:|
//@[028:00076) |     |   |   | └─ObjectSyntax
//@[028:00029) |     |   |   |   ├─Token(LeftBrace) |{|
//@[029:00031) |     |   |   |   ├─Token(NewLine) |\r\n|
        enableAutomaticUpdates: true
//@[008:00036) |     |   |   |   ├─ObjectPropertySyntax
//@[008:00030) |     |   |   |   | ├─IdentifierSyntax
//@[008:00030) |     |   |   |   | | └─Token(Identifier) |enableAutomaticUpdates|
//@[030:00031) |     |   |   |   | ├─Token(Colon) |:|
//@[032:00036) |     |   |   |   | └─BooleanLiteralSyntax
//@[032:00036) |     |   |   |   |   └─Token(TrueKeyword) |true|
//@[036:00038) |     |   |   |   ├─Token(NewLine) |\r\n|
      }
//@[006:00007) |     |   |   |   └─Token(RightBrace) |}|
//@[007:00009) |     |   |   ├─Token(NewLine) |\r\n|
    }
//@[004:00005) |     |   |   └─Token(RightBrace) |}|
//@[005:00007) |     |   ├─Token(NewLine) |\r\n|
  }
//@[002:00003) |     |   └─Token(RightBrace) |}|
//@[003:00005) |     ├─Token(NewLine) |\r\n|
}
//@[000:00001) |     └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

resource extension1 'My.Rp/extensionResource@2020-12-01' = {
//@[000:00110) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00019) | ├─IdentifierSyntax
//@[009:00019) | | └─Token(Identifier) |extension1|
//@[020:00056) | ├─StringSyntax
//@[020:00056) | | └─Token(StringComplete) |'My.Rp/extensionResource@2020-12-01'|
//@[057:00058) | ├─Token(Assignment) |=|
//@[059:00110) | └─ObjectSyntax
//@[059:00060) |   ├─Token(LeftBrace) |{|
//@[060:00062) |   ├─Token(NewLine) |\r\n|
  name: 'extension'
//@[002:00019) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00019) |   | └─StringSyntax
//@[008:00019) |   |   └─Token(StringComplete) |'extension'|
//@[019:00021) |   ├─Token(NewLine) |\r\n|
  scope: vmWithCondition
//@[002:00024) |   ├─ObjectPropertySyntax
//@[002:00007) |   | ├─IdentifierSyntax
//@[002:00007) |   | | └─Token(Identifier) |scope|
//@[007:00008) |   | ├─Token(Colon) |:|
//@[009:00024) |   | └─VariableAccessSyntax
//@[009:00024) |   |   └─IdentifierSyntax
//@[009:00024) |   |     └─Token(Identifier) |vmWithCondition|
//@[024:00026) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

resource extension2 'My.Rp/extensionResource@2020-12-01' = {
//@[000:00105) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00019) | ├─IdentifierSyntax
//@[009:00019) | | └─Token(Identifier) |extension2|
//@[020:00056) | ├─StringSyntax
//@[020:00056) | | └─Token(StringComplete) |'My.Rp/extensionResource@2020-12-01'|
//@[057:00058) | ├─Token(Assignment) |=|
//@[059:00105) | └─ObjectSyntax
//@[059:00060) |   ├─Token(LeftBrace) |{|
//@[060:00062) |   ├─Token(NewLine) |\r\n|
  name: 'extension'
//@[002:00019) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00019) |   | └─StringSyntax
//@[008:00019) |   |   └─Token(StringComplete) |'extension'|
//@[019:00021) |   ├─Token(NewLine) |\r\n|
  scope: extension1
//@[002:00019) |   ├─ObjectPropertySyntax
//@[002:00007) |   | ├─IdentifierSyntax
//@[002:00007) |   | | └─Token(Identifier) |scope|
//@[007:00008) |   | ├─Token(Colon) |:|
//@[009:00019) |   | └─VariableAccessSyntax
//@[009:00019) |   |   └─IdentifierSyntax
//@[009:00019) |   |     └─Token(Identifier) |extension1|
//@[019:00021) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

resource extensionDependencies 'My.Rp/mockResource@2020-01-01' = {
//@[000:00359) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00030) | ├─IdentifierSyntax
//@[009:00030) | | └─Token(Identifier) |extensionDependencies|
//@[031:00062) | ├─StringSyntax
//@[031:00062) | | └─Token(StringComplete) |'My.Rp/mockResource@2020-01-01'|
//@[063:00064) | ├─Token(Assignment) |=|
//@[065:00359) | └─ObjectSyntax
//@[065:00066) |   ├─Token(LeftBrace) |{|
//@[066:00068) |   ├─Token(NewLine) |\r\n|
  name: 'extensionDependencies'
//@[002:00031) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00031) |   | └─StringSyntax
//@[008:00031) |   |   └─Token(StringComplete) |'extensionDependencies'|
//@[031:00033) |   ├─Token(NewLine) |\r\n|
  properties: {
//@[002:00255) |   ├─ObjectPropertySyntax
//@[002:00012) |   | ├─IdentifierSyntax
//@[002:00012) |   | | └─Token(Identifier) |properties|
//@[012:00013) |   | ├─Token(Colon) |:|
//@[014:00255) |   | └─ObjectSyntax
//@[014:00015) |   |   ├─Token(LeftBrace) |{|
//@[015:00017) |   |   ├─Token(NewLine) |\r\n|
    res1: vmWithCondition.id
//@[004:00028) |   |   ├─ObjectPropertySyntax
//@[004:00008) |   |   | ├─IdentifierSyntax
//@[004:00008) |   |   | | └─Token(Identifier) |res1|
//@[008:00009) |   |   | ├─Token(Colon) |:|
//@[010:00028) |   |   | └─PropertyAccessSyntax
//@[010:00025) |   |   |   ├─VariableAccessSyntax
//@[010:00025) |   |   |   | └─IdentifierSyntax
//@[010:00025) |   |   |   |   └─Token(Identifier) |vmWithCondition|
//@[025:00026) |   |   |   ├─Token(Dot) |.|
//@[026:00028) |   |   |   └─IdentifierSyntax
//@[026:00028) |   |   |     └─Token(Identifier) |id|
//@[028:00030) |   |   ├─Token(NewLine) |\r\n|
    res1runtime: vmWithCondition.properties.something
//@[004:00053) |   |   ├─ObjectPropertySyntax
//@[004:00015) |   |   | ├─IdentifierSyntax
//@[004:00015) |   |   | | └─Token(Identifier) |res1runtime|
//@[015:00016) |   |   | ├─Token(Colon) |:|
//@[017:00053) |   |   | └─PropertyAccessSyntax
//@[017:00043) |   |   |   ├─PropertyAccessSyntax
//@[017:00032) |   |   |   | ├─VariableAccessSyntax
//@[017:00032) |   |   |   | | └─IdentifierSyntax
//@[017:00032) |   |   |   | |   └─Token(Identifier) |vmWithCondition|
//@[032:00033) |   |   |   | ├─Token(Dot) |.|
//@[033:00043) |   |   |   | └─IdentifierSyntax
//@[033:00043) |   |   |   |   └─Token(Identifier) |properties|
//@[043:00044) |   |   |   ├─Token(Dot) |.|
//@[044:00053) |   |   |   └─IdentifierSyntax
//@[044:00053) |   |   |     └─Token(Identifier) |something|
//@[053:00055) |   |   ├─Token(NewLine) |\r\n|
    res2: extension1.id
//@[004:00023) |   |   ├─ObjectPropertySyntax
//@[004:00008) |   |   | ├─IdentifierSyntax
//@[004:00008) |   |   | | └─Token(Identifier) |res2|
//@[008:00009) |   |   | ├─Token(Colon) |:|
//@[010:00023) |   |   | └─PropertyAccessSyntax
//@[010:00020) |   |   |   ├─VariableAccessSyntax
//@[010:00020) |   |   |   | └─IdentifierSyntax
//@[010:00020) |   |   |   |   └─Token(Identifier) |extension1|
//@[020:00021) |   |   |   ├─Token(Dot) |.|
//@[021:00023) |   |   |   └─IdentifierSyntax
//@[021:00023) |   |   |     └─Token(Identifier) |id|
//@[023:00025) |   |   ├─Token(NewLine) |\r\n|
    res2runtime: extension1.properties.something
//@[004:00048) |   |   ├─ObjectPropertySyntax
//@[004:00015) |   |   | ├─IdentifierSyntax
//@[004:00015) |   |   | | └─Token(Identifier) |res2runtime|
//@[015:00016) |   |   | ├─Token(Colon) |:|
//@[017:00048) |   |   | └─PropertyAccessSyntax
//@[017:00038) |   |   |   ├─PropertyAccessSyntax
//@[017:00027) |   |   |   | ├─VariableAccessSyntax
//@[017:00027) |   |   |   | | └─IdentifierSyntax
//@[017:00027) |   |   |   | |   └─Token(Identifier) |extension1|
//@[027:00028) |   |   |   | ├─Token(Dot) |.|
//@[028:00038) |   |   |   | └─IdentifierSyntax
//@[028:00038) |   |   |   |   └─Token(Identifier) |properties|
//@[038:00039) |   |   |   ├─Token(Dot) |.|
//@[039:00048) |   |   |   └─IdentifierSyntax
//@[039:00048) |   |   |     └─Token(Identifier) |something|
//@[048:00050) |   |   ├─Token(NewLine) |\r\n|
    res3: extension2.id
//@[004:00023) |   |   ├─ObjectPropertySyntax
//@[004:00008) |   |   | ├─IdentifierSyntax
//@[004:00008) |   |   | | └─Token(Identifier) |res3|
//@[008:00009) |   |   | ├─Token(Colon) |:|
//@[010:00023) |   |   | └─PropertyAccessSyntax
//@[010:00020) |   |   |   ├─VariableAccessSyntax
//@[010:00020) |   |   |   | └─IdentifierSyntax
//@[010:00020) |   |   |   |   └─Token(Identifier) |extension2|
//@[020:00021) |   |   |   ├─Token(Dot) |.|
//@[021:00023) |   |   |   └─IdentifierSyntax
//@[021:00023) |   |   |     └─Token(Identifier) |id|
//@[023:00025) |   |   ├─Token(NewLine) |\r\n|
    res3runtime: extension2.properties.something
//@[004:00048) |   |   ├─ObjectPropertySyntax
//@[004:00015) |   |   | ├─IdentifierSyntax
//@[004:00015) |   |   | | └─Token(Identifier) |res3runtime|
//@[015:00016) |   |   | ├─Token(Colon) |:|
//@[017:00048) |   |   | └─PropertyAccessSyntax
//@[017:00038) |   |   |   ├─PropertyAccessSyntax
//@[017:00027) |   |   |   | ├─VariableAccessSyntax
//@[017:00027) |   |   |   | | └─IdentifierSyntax
//@[017:00027) |   |   |   | |   └─Token(Identifier) |extension2|
//@[027:00028) |   |   |   | ├─Token(Dot) |.|
//@[028:00038) |   |   |   | └─IdentifierSyntax
//@[028:00038) |   |   |   |   └─Token(Identifier) |properties|
//@[038:00039) |   |   |   ├─Token(Dot) |.|
//@[039:00048) |   |   |   └─IdentifierSyntax
//@[039:00048) |   |   |     └─Token(Identifier) |something|
//@[048:00050) |   |   ├─Token(NewLine) |\r\n|
  }
//@[002:00003) |   |   └─Token(RightBrace) |}|
//@[003:00005) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

@sys.description('this is existing1')
//@[000:00162) ├─ResourceDeclarationSyntax
//@[000:00037) | ├─DecoratorSyntax
//@[000:00001) | | ├─Token(At) |@|
//@[001:00037) | | └─InstanceFunctionCallSyntax
//@[001:00004) | |   ├─VariableAccessSyntax
//@[001:00004) | |   | └─IdentifierSyntax
//@[001:00004) | |   |   └─Token(Identifier) |sys|
//@[004:00005) | |   ├─Token(Dot) |.|
//@[005:00016) | |   ├─IdentifierSyntax
//@[005:00016) | |   | └─Token(Identifier) |description|
//@[016:00017) | |   ├─Token(LeftParen) |(|
//@[017:00036) | |   ├─FunctionArgumentSyntax
//@[017:00036) | |   | └─StringSyntax
//@[017:00036) | |   |   └─Token(StringComplete) |'this is existing1'|
//@[036:00037) | |   └─Token(RightParen) |)|
//@[037:00039) | ├─Token(NewLine) |\r\n|
resource existing1 'Mock.Rp/existingExtensionResource@2020-01-01' existing = {
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00018) | ├─IdentifierSyntax
//@[009:00018) | | └─Token(Identifier) |existing1|
//@[019:00065) | ├─StringSyntax
//@[019:00065) | | └─Token(StringComplete) |'Mock.Rp/existingExtensionResource@2020-01-01'|
//@[066:00074) | ├─Token(Identifier) |existing|
//@[075:00076) | ├─Token(Assignment) |=|
//@[077:00123) | └─ObjectSyntax
//@[077:00078) |   ├─Token(LeftBrace) |{|
//@[078:00080) |   ├─Token(NewLine) |\r\n|
  name: 'existing1'
//@[002:00019) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00019) |   | └─StringSyntax
//@[008:00019) |   |   └─Token(StringComplete) |'existing1'|
//@[019:00021) |   ├─Token(NewLine) |\r\n|
  scope: extension1
//@[002:00019) |   ├─ObjectPropertySyntax
//@[002:00007) |   | ├─IdentifierSyntax
//@[002:00007) |   | | └─Token(Identifier) |scope|
//@[007:00008) |   | ├─Token(Colon) |:|
//@[009:00019) |   | └─VariableAccessSyntax
//@[009:00019) |   |   └─IdentifierSyntax
//@[009:00019) |   |     └─Token(Identifier) |extension1|
//@[019:00021) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

resource existing2 'Mock.Rp/existingExtensionResource@2020-01-01' existing = {
//@[000:00122) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00018) | ├─IdentifierSyntax
//@[009:00018) | | └─Token(Identifier) |existing2|
//@[019:00065) | ├─StringSyntax
//@[019:00065) | | └─Token(StringComplete) |'Mock.Rp/existingExtensionResource@2020-01-01'|
//@[066:00074) | ├─Token(Identifier) |existing|
//@[075:00076) | ├─Token(Assignment) |=|
//@[077:00122) | └─ObjectSyntax
//@[077:00078) |   ├─Token(LeftBrace) |{|
//@[078:00080) |   ├─Token(NewLine) |\r\n|
  name: 'existing2'
//@[002:00019) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00019) |   | └─StringSyntax
//@[008:00019) |   |   └─Token(StringComplete) |'existing2'|
//@[019:00021) |   ├─Token(NewLine) |\r\n|
  scope: existing1
//@[002:00018) |   ├─ObjectPropertySyntax
//@[002:00007) |   | ├─IdentifierSyntax
//@[002:00007) |   | | └─Token(Identifier) |scope|
//@[007:00008) |   | ├─Token(Colon) |:|
//@[009:00018) |   | └─VariableAccessSyntax
//@[009:00018) |   |   └─IdentifierSyntax
//@[009:00018) |   |     └─Token(Identifier) |existing1|
//@[018:00020) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

resource extension3 'My.Rp/extensionResource@2020-12-01' = {
//@[000:00105) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00019) | ├─IdentifierSyntax
//@[009:00019) | | └─Token(Identifier) |extension3|
//@[020:00056) | ├─StringSyntax
//@[020:00056) | | └─Token(StringComplete) |'My.Rp/extensionResource@2020-12-01'|
//@[057:00058) | ├─Token(Assignment) |=|
//@[059:00105) | └─ObjectSyntax
//@[059:00060) |   ├─Token(LeftBrace) |{|
//@[060:00062) |   ├─Token(NewLine) |\r\n|
  name: 'extension3'
//@[002:00020) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00020) |   | └─StringSyntax
//@[008:00020) |   |   └─Token(StringComplete) |'extension3'|
//@[020:00022) |   ├─Token(NewLine) |\r\n|
  scope: existing1
//@[002:00018) |   ├─ObjectPropertySyntax
//@[002:00007) |   | ├─IdentifierSyntax
//@[002:00007) |   | | └─Token(Identifier) |scope|
//@[007:00008) |   | ├─Token(Colon) |:|
//@[009:00018) |   | └─VariableAccessSyntax
//@[009:00018) |   |   └─IdentifierSyntax
//@[009:00018) |   |     └─Token(Identifier) |existing1|
//@[018:00020) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

/*
  valid loop cases
*/ 
//@[003:00005) ├─Token(NewLine) |\r\n|
var storageAccounts = [
//@[000:00129) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00019) | ├─IdentifierSyntax
//@[004:00019) | | └─Token(Identifier) |storageAccounts|
//@[020:00021) | ├─Token(Assignment) |=|
//@[022:00129) | └─ArraySyntax
//@[022:00023) |   ├─Token(LeftSquare) |[|
//@[023:00025) |   ├─Token(NewLine) |\r\n|
  {
//@[002:00050) |   ├─ArrayItemSyntax
//@[002:00050) |   | └─ObjectSyntax
//@[002:00003) |   |   ├─Token(LeftBrace) |{|
//@[003:00005) |   |   ├─Token(NewLine) |\r\n|
    name: 'one'
//@[004:00015) |   |   ├─ObjectPropertySyntax
//@[004:00008) |   |   | ├─IdentifierSyntax
//@[004:00008) |   |   | | └─Token(Identifier) |name|
//@[008:00009) |   |   | ├─Token(Colon) |:|
//@[010:00015) |   |   | └─StringSyntax
//@[010:00015) |   |   |   └─Token(StringComplete) |'one'|
//@[015:00017) |   |   ├─Token(NewLine) |\r\n|
    location: 'eastus2'
//@[004:00023) |   |   ├─ObjectPropertySyntax
//@[004:00012) |   |   | ├─IdentifierSyntax
//@[004:00012) |   |   | | └─Token(Identifier) |location|
//@[012:00013) |   |   | ├─Token(Colon) |:|
//@[014:00023) |   |   | └─StringSyntax
//@[014:00023) |   |   |   └─Token(StringComplete) |'eastus2'|
//@[023:00025) |   |   ├─Token(NewLine) |\r\n|
  }
//@[002:00003) |   |   └─Token(RightBrace) |}|
//@[003:00005) |   ├─Token(NewLine) |\r\n|
  {
//@[002:00049) |   ├─ArrayItemSyntax
//@[002:00049) |   | └─ObjectSyntax
//@[002:00003) |   |   ├─Token(LeftBrace) |{|
//@[003:00005) |   |   ├─Token(NewLine) |\r\n|
    name: 'two'
//@[004:00015) |   |   ├─ObjectPropertySyntax
//@[004:00008) |   |   | ├─IdentifierSyntax
//@[004:00008) |   |   | | └─Token(Identifier) |name|
//@[008:00009) |   |   | ├─Token(Colon) |:|
//@[010:00015) |   |   | └─StringSyntax
//@[010:00015) |   |   |   └─Token(StringComplete) |'two'|
//@[015:00017) |   |   ├─Token(NewLine) |\r\n|
    location: 'westus'
//@[004:00022) |   |   ├─ObjectPropertySyntax
//@[004:00012) |   |   | ├─IdentifierSyntax
//@[004:00012) |   |   | | └─Token(Identifier) |location|
//@[012:00013) |   |   | ├─Token(Colon) |:|
//@[014:00022) |   |   | └─StringSyntax
//@[014:00022) |   |   |   └─Token(StringComplete) |'westus'|
//@[022:00024) |   |   ├─Token(NewLine) |\r\n|
  }
//@[002:00003) |   |   └─Token(RightBrace) |}|
//@[003:00005) |   ├─Token(NewLine) |\r\n|
]
//@[000:00001) |   └─Token(RightSquare) |]|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

// just a storage account loop
//@[030:00032) ├─Token(NewLine) |\r\n|
@sys.description('this is just a storage account loop')
//@[000:00284) ├─ResourceDeclarationSyntax
//@[000:00055) | ├─DecoratorSyntax
//@[000:00001) | | ├─Token(At) |@|
//@[001:00055) | | └─InstanceFunctionCallSyntax
//@[001:00004) | |   ├─VariableAccessSyntax
//@[001:00004) | |   | └─IdentifierSyntax
//@[001:00004) | |   |   └─Token(Identifier) |sys|
//@[004:00005) | |   ├─Token(Dot) |.|
//@[005:00016) | |   ├─IdentifierSyntax
//@[005:00016) | |   | └─Token(Identifier) |description|
//@[016:00017) | |   ├─Token(LeftParen) |(|
//@[017:00054) | |   ├─FunctionArgumentSyntax
//@[017:00054) | |   | └─StringSyntax
//@[017:00054) | |   |   └─Token(StringComplete) |'this is just a storage account loop'|
//@[054:00055) | |   └─Token(RightParen) |)|
//@[055:00057) | ├─Token(NewLine) |\r\n|
resource storageResources 'Microsoft.Storage/storageAccounts@2019-06-01' = [for account in storageAccounts: {
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00025) | ├─IdentifierSyntax
//@[009:00025) | | └─Token(Identifier) |storageResources|
//@[026:00072) | ├─StringSyntax
//@[026:00072) | | └─Token(StringComplete) |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[073:00074) | ├─Token(Assignment) |=|
//@[075:00227) | └─ForSyntax
//@[075:00076) |   ├─Token(LeftSquare) |[|
//@[076:00079) |   ├─Token(Identifier) |for|
//@[080:00087) |   ├─LocalVariableSyntax
//@[080:00087) |   | └─IdentifierSyntax
//@[080:00087) |   |   └─Token(Identifier) |account|
//@[088:00090) |   ├─Token(Identifier) |in|
//@[091:00106) |   ├─VariableAccessSyntax
//@[091:00106) |   | └─IdentifierSyntax
//@[091:00106) |   |   └─Token(Identifier) |storageAccounts|
//@[106:00107) |   ├─Token(Colon) |:|
//@[108:00226) |   ├─ObjectSyntax
//@[108:00109) |   | ├─Token(LeftBrace) |{|
//@[109:00111) |   | ├─Token(NewLine) |\r\n|
  name: account.name
//@[002:00020) |   | ├─ObjectPropertySyntax
//@[002:00006) |   | | ├─IdentifierSyntax
//@[002:00006) |   | | | └─Token(Identifier) |name|
//@[006:00007) |   | | ├─Token(Colon) |:|
//@[008:00020) |   | | └─PropertyAccessSyntax
//@[008:00015) |   | |   ├─VariableAccessSyntax
//@[008:00015) |   | |   | └─IdentifierSyntax
//@[008:00015) |   | |   |   └─Token(Identifier) |account|
//@[015:00016) |   | |   ├─Token(Dot) |.|
//@[016:00020) |   | |   └─IdentifierSyntax
//@[016:00020) |   | |     └─Token(Identifier) |name|
//@[020:00022) |   | ├─Token(NewLine) |\r\n|
  location: account.location
//@[002:00028) |   | ├─ObjectPropertySyntax
//@[002:00010) |   | | ├─IdentifierSyntax
//@[002:00010) |   | | | └─Token(Identifier) |location|
//@[010:00011) |   | | ├─Token(Colon) |:|
//@[012:00028) |   | | └─PropertyAccessSyntax
//@[012:00019) |   | |   ├─VariableAccessSyntax
//@[012:00019) |   | |   | └─IdentifierSyntax
//@[012:00019) |   | |   |   └─Token(Identifier) |account|
//@[019:00020) |   | |   ├─Token(Dot) |.|
//@[020:00028) |   | |   └─IdentifierSyntax
//@[020:00028) |   | |     └─Token(Identifier) |location|
//@[028:00030) |   | ├─Token(NewLine) |\r\n|
  sku: {
//@[002:00039) |   | ├─ObjectPropertySyntax
//@[002:00005) |   | | ├─IdentifierSyntax
//@[002:00005) |   | | | └─Token(Identifier) |sku|
//@[005:00006) |   | | ├─Token(Colon) |:|
//@[007:00039) |   | | └─ObjectSyntax
//@[007:00008) |   | |   ├─Token(LeftBrace) |{|
//@[008:00010) |   | |   ├─Token(NewLine) |\r\n|
    name: 'Standard_LRS'
//@[004:00024) |   | |   ├─ObjectPropertySyntax
//@[004:00008) |   | |   | ├─IdentifierSyntax
//@[004:00008) |   | |   | | └─Token(Identifier) |name|
//@[008:00009) |   | |   | ├─Token(Colon) |:|
//@[010:00024) |   | |   | └─StringSyntax
//@[010:00024) |   | |   |   └─Token(StringComplete) |'Standard_LRS'|
//@[024:00026) |   | |   ├─Token(NewLine) |\r\n|
  }
//@[002:00003) |   | |   └─Token(RightBrace) |}|
//@[003:00005) |   | ├─Token(NewLine) |\r\n|
  kind: 'StorageV2'
//@[002:00019) |   | ├─ObjectPropertySyntax
//@[002:00006) |   | | ├─IdentifierSyntax
//@[002:00006) |   | | | └─Token(Identifier) |kind|
//@[006:00007) |   | | ├─Token(Colon) |:|
//@[008:00019) |   | | └─StringSyntax
//@[008:00019) |   | |   └─Token(StringComplete) |'StorageV2'|
//@[019:00021) |   | ├─Token(NewLine) |\r\n|
}]
//@[000:00001) |   | └─Token(RightBrace) |}|
//@[001:00002) |   └─Token(RightSquare) |]|
//@[002:00006) ├─Token(NewLine) |\r\n\r\n|

// storage account loop with index
//@[034:00036) ├─Token(NewLine) |\r\n|
@sys.description('this is just a storage account loop with index')
//@[000:00318) ├─ResourceDeclarationSyntax
//@[000:00066) | ├─DecoratorSyntax
//@[000:00001) | | ├─Token(At) |@|
//@[001:00066) | | └─InstanceFunctionCallSyntax
//@[001:00004) | |   ├─VariableAccessSyntax
//@[001:00004) | |   | └─IdentifierSyntax
//@[001:00004) | |   |   └─Token(Identifier) |sys|
//@[004:00005) | |   ├─Token(Dot) |.|
//@[005:00016) | |   ├─IdentifierSyntax
//@[005:00016) | |   | └─Token(Identifier) |description|
//@[016:00017) | |   ├─Token(LeftParen) |(|
//@[017:00065) | |   ├─FunctionArgumentSyntax
//@[017:00065) | |   | └─StringSyntax
//@[017:00065) | |   |   └─Token(StringComplete) |'this is just a storage account loop with index'|
//@[065:00066) | |   └─Token(RightParen) |)|
//@[066:00068) | ├─Token(NewLine) |\r\n|
resource storageResourcesWithIndex 'Microsoft.Storage/storageAccounts@2019-06-01' = [for (account, i) in storageAccounts: {
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00034) | ├─IdentifierSyntax
//@[009:00034) | | └─Token(Identifier) |storageResourcesWithIndex|
//@[035:00081) | ├─StringSyntax
//@[035:00081) | | └─Token(StringComplete) |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[082:00083) | ├─Token(Assignment) |=|
//@[084:00250) | └─ForSyntax
//@[084:00085) |   ├─Token(LeftSquare) |[|
//@[085:00088) |   ├─Token(Identifier) |for|
//@[089:00101) |   ├─VariableBlockSyntax
//@[089:00090) |   | ├─Token(LeftParen) |(|
//@[090:00097) |   | ├─LocalVariableSyntax
//@[090:00097) |   | | └─IdentifierSyntax
//@[090:00097) |   | |   └─Token(Identifier) |account|
//@[097:00098) |   | ├─Token(Comma) |,|
//@[099:00100) |   | ├─LocalVariableSyntax
//@[099:00100) |   | | └─IdentifierSyntax
//@[099:00100) |   | |   └─Token(Identifier) |i|
//@[100:00101) |   | └─Token(RightParen) |)|
//@[102:00104) |   ├─Token(Identifier) |in|
//@[105:00120) |   ├─VariableAccessSyntax
//@[105:00120) |   | └─IdentifierSyntax
//@[105:00120) |   |   └─Token(Identifier) |storageAccounts|
//@[120:00121) |   ├─Token(Colon) |:|
//@[122:00249) |   ├─ObjectSyntax
//@[122:00123) |   | ├─Token(LeftBrace) |{|
//@[123:00125) |   | ├─Token(NewLine) |\r\n|
  name: '${account.name}${i}'
//@[002:00029) |   | ├─ObjectPropertySyntax
//@[002:00006) |   | | ├─IdentifierSyntax
//@[002:00006) |   | | | └─Token(Identifier) |name|
//@[006:00007) |   | | ├─Token(Colon) |:|
//@[008:00029) |   | | └─StringSyntax
//@[008:00011) |   | |   ├─Token(StringLeftPiece) |'${|
//@[011:00023) |   | |   ├─PropertyAccessSyntax
//@[011:00018) |   | |   | ├─VariableAccessSyntax
//@[011:00018) |   | |   | | └─IdentifierSyntax
//@[011:00018) |   | |   | |   └─Token(Identifier) |account|
//@[018:00019) |   | |   | ├─Token(Dot) |.|
//@[019:00023) |   | |   | └─IdentifierSyntax
//@[019:00023) |   | |   |   └─Token(Identifier) |name|
//@[023:00026) |   | |   ├─Token(StringMiddlePiece) |}${|
//@[026:00027) |   | |   ├─VariableAccessSyntax
//@[026:00027) |   | |   | └─IdentifierSyntax
//@[026:00027) |   | |   |   └─Token(Identifier) |i|
//@[027:00029) |   | |   └─Token(StringRightPiece) |}'|
//@[029:00031) |   | ├─Token(NewLine) |\r\n|
  location: account.location
//@[002:00028) |   | ├─ObjectPropertySyntax
//@[002:00010) |   | | ├─IdentifierSyntax
//@[002:00010) |   | | | └─Token(Identifier) |location|
//@[010:00011) |   | | ├─Token(Colon) |:|
//@[012:00028) |   | | └─PropertyAccessSyntax
//@[012:00019) |   | |   ├─VariableAccessSyntax
//@[012:00019) |   | |   | └─IdentifierSyntax
//@[012:00019) |   | |   |   └─Token(Identifier) |account|
//@[019:00020) |   | |   ├─Token(Dot) |.|
//@[020:00028) |   | |   └─IdentifierSyntax
//@[020:00028) |   | |     └─Token(Identifier) |location|
//@[028:00030) |   | ├─Token(NewLine) |\r\n|
  sku: {
//@[002:00039) |   | ├─ObjectPropertySyntax
//@[002:00005) |   | | ├─IdentifierSyntax
//@[002:00005) |   | | | └─Token(Identifier) |sku|
//@[005:00006) |   | | ├─Token(Colon) |:|
//@[007:00039) |   | | └─ObjectSyntax
//@[007:00008) |   | |   ├─Token(LeftBrace) |{|
//@[008:00010) |   | |   ├─Token(NewLine) |\r\n|
    name: 'Standard_LRS'
//@[004:00024) |   | |   ├─ObjectPropertySyntax
//@[004:00008) |   | |   | ├─IdentifierSyntax
//@[004:00008) |   | |   | | └─Token(Identifier) |name|
//@[008:00009) |   | |   | ├─Token(Colon) |:|
//@[010:00024) |   | |   | └─StringSyntax
//@[010:00024) |   | |   |   └─Token(StringComplete) |'Standard_LRS'|
//@[024:00026) |   | |   ├─Token(NewLine) |\r\n|
  }
//@[002:00003) |   | |   └─Token(RightBrace) |}|
//@[003:00005) |   | ├─Token(NewLine) |\r\n|
  kind: 'StorageV2'
//@[002:00019) |   | ├─ObjectPropertySyntax
//@[002:00006) |   | | ├─IdentifierSyntax
//@[002:00006) |   | | | └─Token(Identifier) |kind|
//@[006:00007) |   | | ├─Token(Colon) |:|
//@[008:00019) |   | | └─StringSyntax
//@[008:00019) |   | |   └─Token(StringComplete) |'StorageV2'|
//@[019:00021) |   | ├─Token(NewLine) |\r\n|
}]
//@[000:00001) |   | └─Token(RightBrace) |}|
//@[001:00002) |   └─Token(RightSquare) |]|
//@[002:00006) ├─Token(NewLine) |\r\n\r\n|

// basic nested loop
//@[020:00022) ├─Token(NewLine) |\r\n|
@sys.description('this is just a basic nested loop')
//@[000:00399) ├─ResourceDeclarationSyntax
//@[000:00052) | ├─DecoratorSyntax
//@[000:00001) | | ├─Token(At) |@|
//@[001:00052) | | └─InstanceFunctionCallSyntax
//@[001:00004) | |   ├─VariableAccessSyntax
//@[001:00004) | |   | └─IdentifierSyntax
//@[001:00004) | |   |   └─Token(Identifier) |sys|
//@[004:00005) | |   ├─Token(Dot) |.|
//@[005:00016) | |   ├─IdentifierSyntax
//@[005:00016) | |   | └─Token(Identifier) |description|
//@[016:00017) | |   ├─Token(LeftParen) |(|
//@[017:00051) | |   ├─FunctionArgumentSyntax
//@[017:00051) | |   | └─StringSyntax
//@[017:00051) | |   |   └─Token(StringComplete) |'this is just a basic nested loop'|
//@[051:00052) | |   └─Token(RightParen) |)|
//@[052:00054) | ├─Token(NewLine) |\r\n|
resource vnet 'Microsoft.Network/virtualNetworks@2020-06-01' = [for i in range(0, 3): {
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00013) | ├─IdentifierSyntax
//@[009:00013) | | └─Token(Identifier) |vnet|
//@[014:00060) | ├─StringSyntax
//@[014:00060) | | └─Token(StringComplete) |'Microsoft.Network/virtualNetworks@2020-06-01'|
//@[061:00062) | ├─Token(Assignment) |=|
//@[063:00345) | └─ForSyntax
//@[063:00064) |   ├─Token(LeftSquare) |[|
//@[064:00067) |   ├─Token(Identifier) |for|
//@[068:00069) |   ├─LocalVariableSyntax
//@[068:00069) |   | └─IdentifierSyntax
//@[068:00069) |   |   └─Token(Identifier) |i|
//@[070:00072) |   ├─Token(Identifier) |in|
//@[073:00084) |   ├─FunctionCallSyntax
//@[073:00078) |   | ├─IdentifierSyntax
//@[073:00078) |   | | └─Token(Identifier) |range|
//@[078:00079) |   | ├─Token(LeftParen) |(|
//@[079:00080) |   | ├─FunctionArgumentSyntax
//@[079:00080) |   | | └─IntegerLiteralSyntax
//@[079:00080) |   | |   └─Token(Integer) |0|
//@[080:00081) |   | ├─Token(Comma) |,|
//@[082:00083) |   | ├─FunctionArgumentSyntax
//@[082:00083) |   | | └─IntegerLiteralSyntax
//@[082:00083) |   | |   └─Token(Integer) |3|
//@[083:00084) |   | └─Token(RightParen) |)|
//@[084:00085) |   ├─Token(Colon) |:|
//@[086:00344) |   ├─ObjectSyntax
//@[086:00087) |   | ├─Token(LeftBrace) |{|
//@[087:00089) |   | ├─Token(NewLine) |\r\n|
  name: 'vnet-${i}'
//@[002:00019) |   | ├─ObjectPropertySyntax
//@[002:00006) |   | | ├─IdentifierSyntax
//@[002:00006) |   | | | └─Token(Identifier) |name|
//@[006:00007) |   | | ├─Token(Colon) |:|
//@[008:00019) |   | | └─StringSyntax
//@[008:00016) |   | |   ├─Token(StringLeftPiece) |'vnet-${|
//@[016:00017) |   | |   ├─VariableAccessSyntax
//@[016:00017) |   | |   | └─IdentifierSyntax
//@[016:00017) |   | |   |   └─Token(Identifier) |i|
//@[017:00019) |   | |   └─Token(StringRightPiece) |}'|
//@[019:00021) |   | ├─Token(NewLine) |\r\n|
  properties: {
//@[002:00231) |   | ├─ObjectPropertySyntax
//@[002:00012) |   | | ├─IdentifierSyntax
//@[002:00012) |   | | | └─Token(Identifier) |properties|
//@[012:00013) |   | | ├─Token(Colon) |:|
//@[014:00231) |   | | └─ObjectSyntax
//@[014:00015) |   | |   ├─Token(LeftBrace) |{|
//@[015:00017) |   | |   ├─Token(NewLine) |\r\n|
    subnets: [for j in range(0, 4): {
//@[004:00209) |   | |   ├─ObjectPropertySyntax
//@[004:00011) |   | |   | ├─IdentifierSyntax
//@[004:00011) |   | |   | | └─Token(Identifier) |subnets|
//@[011:00012) |   | |   | ├─Token(Colon) |:|
//@[013:00209) |   | |   | └─ForSyntax
//@[013:00014) |   | |   |   ├─Token(LeftSquare) |[|
//@[014:00017) |   | |   |   ├─Token(Identifier) |for|
//@[018:00019) |   | |   |   ├─LocalVariableSyntax
//@[018:00019) |   | |   |   | └─IdentifierSyntax
//@[018:00019) |   | |   |   |   └─Token(Identifier) |j|
//@[020:00022) |   | |   |   ├─Token(Identifier) |in|
//@[023:00034) |   | |   |   ├─FunctionCallSyntax
//@[023:00028) |   | |   |   | ├─IdentifierSyntax
//@[023:00028) |   | |   |   | | └─Token(Identifier) |range|
//@[028:00029) |   | |   |   | ├─Token(LeftParen) |(|
//@[029:00030) |   | |   |   | ├─FunctionArgumentSyntax
//@[029:00030) |   | |   |   | | └─IntegerLiteralSyntax
//@[029:00030) |   | |   |   | |   └─Token(Integer) |0|
//@[030:00031) |   | |   |   | ├─Token(Comma) |,|
//@[032:00033) |   | |   |   | ├─FunctionArgumentSyntax
//@[032:00033) |   | |   |   | | └─IntegerLiteralSyntax
//@[032:00033) |   | |   |   | |   └─Token(Integer) |4|
//@[033:00034) |   | |   |   | └─Token(RightParen) |)|
//@[034:00035) |   | |   |   ├─Token(Colon) |:|
//@[036:00208) |   | |   |   ├─ObjectSyntax
//@[036:00037) |   | |   |   | ├─Token(LeftBrace) |{|
//@[037:00039) |   | |   |   | ├─Token(NewLine) |\r\n|
      // #completionTest(0,1,2,3,4,5) -> subnetIdAndProperties
//@[062:00064) |   | |   |   | ├─Token(NewLine) |\r\n|
     
//@[005:00007) |   | |   |   | ├─Token(NewLine) |\r\n|
      // #completionTest(6) -> subnetIdAndPropertiesNoColon
//@[059:00061) |   | |   |   | ├─Token(NewLine) |\r\n|
      name: 'subnet-${i}-${j}'
//@[006:00030) |   | |   |   | ├─ObjectPropertySyntax
//@[006:00010) |   | |   |   | | ├─IdentifierSyntax
//@[006:00010) |   | |   |   | | | └─Token(Identifier) |name|
//@[010:00011) |   | |   |   | | ├─Token(Colon) |:|
//@[012:00030) |   | |   |   | | └─StringSyntax
//@[012:00022) |   | |   |   | |   ├─Token(StringLeftPiece) |'subnet-${|
//@[022:00023) |   | |   |   | |   ├─VariableAccessSyntax
//@[022:00023) |   | |   |   | |   | └─IdentifierSyntax
//@[022:00023) |   | |   |   | |   |   └─Token(Identifier) |i|
//@[023:00027) |   | |   |   | |   ├─Token(StringMiddlePiece) |}-${|
//@[027:00028) |   | |   |   | |   ├─VariableAccessSyntax
//@[027:00028) |   | |   |   | |   | └─IdentifierSyntax
//@[027:00028) |   | |   |   | |   |   └─Token(Identifier) |j|
//@[028:00030) |   | |   |   | |   └─Token(StringRightPiece) |}'|
//@[030:00032) |   | |   |   | ├─Token(NewLine) |\r\n|
    }]
//@[004:00005) |   | |   |   | └─Token(RightBrace) |}|
//@[005:00006) |   | |   |   └─Token(RightSquare) |]|
//@[006:00008) |   | |   ├─Token(NewLine) |\r\n|
  }
//@[002:00003) |   | |   └─Token(RightBrace) |}|
//@[003:00005) |   | ├─Token(NewLine) |\r\n|
}]
//@[000:00001) |   | └─Token(RightBrace) |}|
//@[001:00002) |   └─Token(RightSquare) |]|
//@[002:00006) ├─Token(NewLine) |\r\n\r\n|

// duplicate identifiers within the loop are allowed
//@[052:00054) ├─Token(NewLine) |\r\n|
resource duplicateIdentifiersWithinLoop 'Microsoft.Network/virtualNetworks@2020-06-01' = [for i in range(0, 3): {
//@[000:00239) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00039) | ├─IdentifierSyntax
//@[009:00039) | | └─Token(Identifier) |duplicateIdentifiersWithinLoop|
//@[040:00086) | ├─StringSyntax
//@[040:00086) | | └─Token(StringComplete) |'Microsoft.Network/virtualNetworks@2020-06-01'|
//@[087:00088) | ├─Token(Assignment) |=|
//@[089:00239) | └─ForSyntax
//@[089:00090) |   ├─Token(LeftSquare) |[|
//@[090:00093) |   ├─Token(Identifier) |for|
//@[094:00095) |   ├─LocalVariableSyntax
//@[094:00095) |   | └─IdentifierSyntax
//@[094:00095) |   |   └─Token(Identifier) |i|
//@[096:00098) |   ├─Token(Identifier) |in|
//@[099:00110) |   ├─FunctionCallSyntax
//@[099:00104) |   | ├─IdentifierSyntax
//@[099:00104) |   | | └─Token(Identifier) |range|
//@[104:00105) |   | ├─Token(LeftParen) |(|
//@[105:00106) |   | ├─FunctionArgumentSyntax
//@[105:00106) |   | | └─IntegerLiteralSyntax
//@[105:00106) |   | |   └─Token(Integer) |0|
//@[106:00107) |   | ├─Token(Comma) |,|
//@[108:00109) |   | ├─FunctionArgumentSyntax
//@[108:00109) |   | | └─IntegerLiteralSyntax
//@[108:00109) |   | |   └─Token(Integer) |3|
//@[109:00110) |   | └─Token(RightParen) |)|
//@[110:00111) |   ├─Token(Colon) |:|
//@[112:00238) |   ├─ObjectSyntax
//@[112:00113) |   | ├─Token(LeftBrace) |{|
//@[113:00115) |   | ├─Token(NewLine) |\r\n|
  name: 'vnet-${i}'
//@[002:00019) |   | ├─ObjectPropertySyntax
//@[002:00006) |   | | ├─IdentifierSyntax
//@[002:00006) |   | | | └─Token(Identifier) |name|
//@[006:00007) |   | | ├─Token(Colon) |:|
//@[008:00019) |   | | └─StringSyntax
//@[008:00016) |   | |   ├─Token(StringLeftPiece) |'vnet-${|
//@[016:00017) |   | |   ├─VariableAccessSyntax
//@[016:00017) |   | |   | └─IdentifierSyntax
//@[016:00017) |   | |   |   └─Token(Identifier) |i|
//@[017:00019) |   | |   └─Token(StringRightPiece) |}'|
//@[019:00021) |   | ├─Token(NewLine) |\r\n|
  properties: {
//@[002:00099) |   | ├─ObjectPropertySyntax
//@[002:00012) |   | | ├─IdentifierSyntax
//@[002:00012) |   | | | └─Token(Identifier) |properties|
//@[012:00013) |   | | ├─Token(Colon) |:|
//@[014:00099) |   | | └─ObjectSyntax
//@[014:00015) |   | |   ├─Token(LeftBrace) |{|
//@[015:00017) |   | |   ├─Token(NewLine) |\r\n|
    subnets: [for i in range(0, 4): {
//@[004:00077) |   | |   ├─ObjectPropertySyntax
//@[004:00011) |   | |   | ├─IdentifierSyntax
//@[004:00011) |   | |   | | └─Token(Identifier) |subnets|
//@[011:00012) |   | |   | ├─Token(Colon) |:|
//@[013:00077) |   | |   | └─ForSyntax
//@[013:00014) |   | |   |   ├─Token(LeftSquare) |[|
//@[014:00017) |   | |   |   ├─Token(Identifier) |for|
//@[018:00019) |   | |   |   ├─LocalVariableSyntax
//@[018:00019) |   | |   |   | └─IdentifierSyntax
//@[018:00019) |   | |   |   |   └─Token(Identifier) |i|
//@[020:00022) |   | |   |   ├─Token(Identifier) |in|
//@[023:00034) |   | |   |   ├─FunctionCallSyntax
//@[023:00028) |   | |   |   | ├─IdentifierSyntax
//@[023:00028) |   | |   |   | | └─Token(Identifier) |range|
//@[028:00029) |   | |   |   | ├─Token(LeftParen) |(|
//@[029:00030) |   | |   |   | ├─FunctionArgumentSyntax
//@[029:00030) |   | |   |   | | └─IntegerLiteralSyntax
//@[029:00030) |   | |   |   | |   └─Token(Integer) |0|
//@[030:00031) |   | |   |   | ├─Token(Comma) |,|
//@[032:00033) |   | |   |   | ├─FunctionArgumentSyntax
//@[032:00033) |   | |   |   | | └─IntegerLiteralSyntax
//@[032:00033) |   | |   |   | |   └─Token(Integer) |4|
//@[033:00034) |   | |   |   | └─Token(RightParen) |)|
//@[034:00035) |   | |   |   ├─Token(Colon) |:|
//@[036:00076) |   | |   |   ├─ObjectSyntax
//@[036:00037) |   | |   |   | ├─Token(LeftBrace) |{|
//@[037:00039) |   | |   |   | ├─Token(NewLine) |\r\n|
      name: 'subnet-${i}-${i}'
//@[006:00030) |   | |   |   | ├─ObjectPropertySyntax
//@[006:00010) |   | |   |   | | ├─IdentifierSyntax
//@[006:00010) |   | |   |   | | | └─Token(Identifier) |name|
//@[010:00011) |   | |   |   | | ├─Token(Colon) |:|
//@[012:00030) |   | |   |   | | └─StringSyntax
//@[012:00022) |   | |   |   | |   ├─Token(StringLeftPiece) |'subnet-${|
//@[022:00023) |   | |   |   | |   ├─VariableAccessSyntax
//@[022:00023) |   | |   |   | |   | └─IdentifierSyntax
//@[022:00023) |   | |   |   | |   |   └─Token(Identifier) |i|
//@[023:00027) |   | |   |   | |   ├─Token(StringMiddlePiece) |}-${|
//@[027:00028) |   | |   |   | |   ├─VariableAccessSyntax
//@[027:00028) |   | |   |   | |   | └─IdentifierSyntax
//@[027:00028) |   | |   |   | |   |   └─Token(Identifier) |i|
//@[028:00030) |   | |   |   | |   └─Token(StringRightPiece) |}'|
//@[030:00032) |   | |   |   | ├─Token(NewLine) |\r\n|
    }]
//@[004:00005) |   | |   |   | └─Token(RightBrace) |}|
//@[005:00006) |   | |   |   └─Token(RightSquare) |]|
//@[006:00008) |   | |   ├─Token(NewLine) |\r\n|
  }
//@[002:00003) |   | |   └─Token(RightBrace) |}|
//@[003:00005) |   | ├─Token(NewLine) |\r\n|
}]
//@[000:00001) |   | └─Token(RightBrace) |}|
//@[001:00002) |   └─Token(RightSquare) |]|
//@[002:00006) ├─Token(NewLine) |\r\n\r\n|

// duplicate identifers in global and single loop scope are allowed (inner variable hides the outer)
//@[100:00102) ├─Token(NewLine) |\r\n|
var canHaveDuplicatesAcrossScopes = 'hello'
//@[000:00043) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00033) | ├─IdentifierSyntax
//@[004:00033) | | └─Token(Identifier) |canHaveDuplicatesAcrossScopes|
//@[034:00035) | ├─Token(Assignment) |=|
//@[036:00043) | └─StringSyntax
//@[036:00043) |   └─Token(StringComplete) |'hello'|
//@[043:00045) ├─Token(NewLine) |\r\n|
resource duplicateInGlobalAndOneLoop 'Microsoft.Network/virtualNetworks@2020-06-01' = [for canHaveDuplicatesAcrossScopes in range(0, 3): {
//@[000:00292) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00036) | ├─IdentifierSyntax
//@[009:00036) | | └─Token(Identifier) |duplicateInGlobalAndOneLoop|
//@[037:00083) | ├─StringSyntax
//@[037:00083) | | └─Token(StringComplete) |'Microsoft.Network/virtualNetworks@2020-06-01'|
//@[084:00085) | ├─Token(Assignment) |=|
//@[086:00292) | └─ForSyntax
//@[086:00087) |   ├─Token(LeftSquare) |[|
//@[087:00090) |   ├─Token(Identifier) |for|
//@[091:00120) |   ├─LocalVariableSyntax
//@[091:00120) |   | └─IdentifierSyntax
//@[091:00120) |   |   └─Token(Identifier) |canHaveDuplicatesAcrossScopes|
//@[121:00123) |   ├─Token(Identifier) |in|
//@[124:00135) |   ├─FunctionCallSyntax
//@[124:00129) |   | ├─IdentifierSyntax
//@[124:00129) |   | | └─Token(Identifier) |range|
//@[129:00130) |   | ├─Token(LeftParen) |(|
//@[130:00131) |   | ├─FunctionArgumentSyntax
//@[130:00131) |   | | └─IntegerLiteralSyntax
//@[130:00131) |   | |   └─Token(Integer) |0|
//@[131:00132) |   | ├─Token(Comma) |,|
//@[133:00134) |   | ├─FunctionArgumentSyntax
//@[133:00134) |   | | └─IntegerLiteralSyntax
//@[133:00134) |   | |   └─Token(Integer) |3|
//@[134:00135) |   | └─Token(RightParen) |)|
//@[135:00136) |   ├─Token(Colon) |:|
//@[137:00291) |   ├─ObjectSyntax
//@[137:00138) |   | ├─Token(LeftBrace) |{|
//@[138:00140) |   | ├─Token(NewLine) |\r\n|
  name: 'vnet-${canHaveDuplicatesAcrossScopes}'
//@[002:00047) |   | ├─ObjectPropertySyntax
//@[002:00006) |   | | ├─IdentifierSyntax
//@[002:00006) |   | | | └─Token(Identifier) |name|
//@[006:00007) |   | | ├─Token(Colon) |:|
//@[008:00047) |   | | └─StringSyntax
//@[008:00016) |   | |   ├─Token(StringLeftPiece) |'vnet-${|
//@[016:00045) |   | |   ├─VariableAccessSyntax
//@[016:00045) |   | |   | └─IdentifierSyntax
//@[016:00045) |   | |   |   └─Token(Identifier) |canHaveDuplicatesAcrossScopes|
//@[045:00047) |   | |   └─Token(StringRightPiece) |}'|
//@[047:00049) |   | ├─Token(NewLine) |\r\n|
  properties: {
//@[002:00099) |   | ├─ObjectPropertySyntax
//@[002:00012) |   | | ├─IdentifierSyntax
//@[002:00012) |   | | | └─Token(Identifier) |properties|
//@[012:00013) |   | | ├─Token(Colon) |:|
//@[014:00099) |   | | └─ObjectSyntax
//@[014:00015) |   | |   ├─Token(LeftBrace) |{|
//@[015:00017) |   | |   ├─Token(NewLine) |\r\n|
    subnets: [for i in range(0, 4): {
//@[004:00077) |   | |   ├─ObjectPropertySyntax
//@[004:00011) |   | |   | ├─IdentifierSyntax
//@[004:00011) |   | |   | | └─Token(Identifier) |subnets|
//@[011:00012) |   | |   | ├─Token(Colon) |:|
//@[013:00077) |   | |   | └─ForSyntax
//@[013:00014) |   | |   |   ├─Token(LeftSquare) |[|
//@[014:00017) |   | |   |   ├─Token(Identifier) |for|
//@[018:00019) |   | |   |   ├─LocalVariableSyntax
//@[018:00019) |   | |   |   | └─IdentifierSyntax
//@[018:00019) |   | |   |   |   └─Token(Identifier) |i|
//@[020:00022) |   | |   |   ├─Token(Identifier) |in|
//@[023:00034) |   | |   |   ├─FunctionCallSyntax
//@[023:00028) |   | |   |   | ├─IdentifierSyntax
//@[023:00028) |   | |   |   | | └─Token(Identifier) |range|
//@[028:00029) |   | |   |   | ├─Token(LeftParen) |(|
//@[029:00030) |   | |   |   | ├─FunctionArgumentSyntax
//@[029:00030) |   | |   |   | | └─IntegerLiteralSyntax
//@[029:00030) |   | |   |   | |   └─Token(Integer) |0|
//@[030:00031) |   | |   |   | ├─Token(Comma) |,|
//@[032:00033) |   | |   |   | ├─FunctionArgumentSyntax
//@[032:00033) |   | |   |   | | └─IntegerLiteralSyntax
//@[032:00033) |   | |   |   | |   └─Token(Integer) |4|
//@[033:00034) |   | |   |   | └─Token(RightParen) |)|
//@[034:00035) |   | |   |   ├─Token(Colon) |:|
//@[036:00076) |   | |   |   ├─ObjectSyntax
//@[036:00037) |   | |   |   | ├─Token(LeftBrace) |{|
//@[037:00039) |   | |   |   | ├─Token(NewLine) |\r\n|
      name: 'subnet-${i}-${i}'
//@[006:00030) |   | |   |   | ├─ObjectPropertySyntax
//@[006:00010) |   | |   |   | | ├─IdentifierSyntax
//@[006:00010) |   | |   |   | | | └─Token(Identifier) |name|
//@[010:00011) |   | |   |   | | ├─Token(Colon) |:|
//@[012:00030) |   | |   |   | | └─StringSyntax
//@[012:00022) |   | |   |   | |   ├─Token(StringLeftPiece) |'subnet-${|
//@[022:00023) |   | |   |   | |   ├─VariableAccessSyntax
//@[022:00023) |   | |   |   | |   | └─IdentifierSyntax
//@[022:00023) |   | |   |   | |   |   └─Token(Identifier) |i|
//@[023:00027) |   | |   |   | |   ├─Token(StringMiddlePiece) |}-${|
//@[027:00028) |   | |   |   | |   ├─VariableAccessSyntax
//@[027:00028) |   | |   |   | |   | └─IdentifierSyntax
//@[027:00028) |   | |   |   | |   |   └─Token(Identifier) |i|
//@[028:00030) |   | |   |   | |   └─Token(StringRightPiece) |}'|
//@[030:00032) |   | |   |   | ├─Token(NewLine) |\r\n|
    }]
//@[004:00005) |   | |   |   | └─Token(RightBrace) |}|
//@[005:00006) |   | |   |   └─Token(RightSquare) |]|
//@[006:00008) |   | |   ├─Token(NewLine) |\r\n|
  }
//@[002:00003) |   | |   └─Token(RightBrace) |}|
//@[003:00005) |   | ├─Token(NewLine) |\r\n|
}]
//@[000:00001) |   | └─Token(RightBrace) |}|
//@[001:00002) |   └─Token(RightSquare) |]|
//@[002:00006) ├─Token(NewLine) |\r\n\r\n|

// duplicate in global and multiple loop scopes are allowed (inner hides the outer)
//@[083:00085) ├─Token(NewLine) |\r\n|
var duplicatesEverywhere = 'hello'
//@[000:00034) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00024) | ├─IdentifierSyntax
//@[004:00024) | | └─Token(Identifier) |duplicatesEverywhere|
//@[025:00026) | ├─Token(Assignment) |=|
//@[027:00034) | └─StringSyntax
//@[027:00034) |   └─Token(StringComplete) |'hello'|
//@[034:00036) ├─Token(NewLine) |\r\n|
resource duplicateInGlobalAndTwoLoops 'Microsoft.Network/virtualNetworks@2020-06-01' = [for duplicatesEverywhere in range(0, 3): {
//@[000:00308) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00037) | ├─IdentifierSyntax
//@[009:00037) | | └─Token(Identifier) |duplicateInGlobalAndTwoLoops|
//@[038:00084) | ├─StringSyntax
//@[038:00084) | | └─Token(StringComplete) |'Microsoft.Network/virtualNetworks@2020-06-01'|
//@[085:00086) | ├─Token(Assignment) |=|
//@[087:00308) | └─ForSyntax
//@[087:00088) |   ├─Token(LeftSquare) |[|
//@[088:00091) |   ├─Token(Identifier) |for|
//@[092:00112) |   ├─LocalVariableSyntax
//@[092:00112) |   | └─IdentifierSyntax
//@[092:00112) |   |   └─Token(Identifier) |duplicatesEverywhere|
//@[113:00115) |   ├─Token(Identifier) |in|
//@[116:00127) |   ├─FunctionCallSyntax
//@[116:00121) |   | ├─IdentifierSyntax
//@[116:00121) |   | | └─Token(Identifier) |range|
//@[121:00122) |   | ├─Token(LeftParen) |(|
//@[122:00123) |   | ├─FunctionArgumentSyntax
//@[122:00123) |   | | └─IntegerLiteralSyntax
//@[122:00123) |   | |   └─Token(Integer) |0|
//@[123:00124) |   | ├─Token(Comma) |,|
//@[125:00126) |   | ├─FunctionArgumentSyntax
//@[125:00126) |   | | └─IntegerLiteralSyntax
//@[125:00126) |   | |   └─Token(Integer) |3|
//@[126:00127) |   | └─Token(RightParen) |)|
//@[127:00128) |   ├─Token(Colon) |:|
//@[129:00307) |   ├─ObjectSyntax
//@[129:00130) |   | ├─Token(LeftBrace) |{|
//@[130:00132) |   | ├─Token(NewLine) |\r\n|
  name: 'vnet-${duplicatesEverywhere}'
//@[002:00038) |   | ├─ObjectPropertySyntax
//@[002:00006) |   | | ├─IdentifierSyntax
//@[002:00006) |   | | | └─Token(Identifier) |name|
//@[006:00007) |   | | ├─Token(Colon) |:|
//@[008:00038) |   | | └─StringSyntax
//@[008:00016) |   | |   ├─Token(StringLeftPiece) |'vnet-${|
//@[016:00036) |   | |   ├─VariableAccessSyntax
//@[016:00036) |   | |   | └─IdentifierSyntax
//@[016:00036) |   | |   |   └─Token(Identifier) |duplicatesEverywhere|
//@[036:00038) |   | |   └─Token(StringRightPiece) |}'|
//@[038:00040) |   | ├─Token(NewLine) |\r\n|
  properties: {
//@[002:00132) |   | ├─ObjectPropertySyntax
//@[002:00012) |   | | ├─IdentifierSyntax
//@[002:00012) |   | | | └─Token(Identifier) |properties|
//@[012:00013) |   | | ├─Token(Colon) |:|
//@[014:00132) |   | | └─ObjectSyntax
//@[014:00015) |   | |   ├─Token(LeftBrace) |{|
//@[015:00017) |   | |   ├─Token(NewLine) |\r\n|
    subnets: [for duplicatesEverywhere in range(0, 4): {
//@[004:00110) |   | |   ├─ObjectPropertySyntax
//@[004:00011) |   | |   | ├─IdentifierSyntax
//@[004:00011) |   | |   | | └─Token(Identifier) |subnets|
//@[011:00012) |   | |   | ├─Token(Colon) |:|
//@[013:00110) |   | |   | └─ForSyntax
//@[013:00014) |   | |   |   ├─Token(LeftSquare) |[|
//@[014:00017) |   | |   |   ├─Token(Identifier) |for|
//@[018:00038) |   | |   |   ├─LocalVariableSyntax
//@[018:00038) |   | |   |   | └─IdentifierSyntax
//@[018:00038) |   | |   |   |   └─Token(Identifier) |duplicatesEverywhere|
//@[039:00041) |   | |   |   ├─Token(Identifier) |in|
//@[042:00053) |   | |   |   ├─FunctionCallSyntax
//@[042:00047) |   | |   |   | ├─IdentifierSyntax
//@[042:00047) |   | |   |   | | └─Token(Identifier) |range|
//@[047:00048) |   | |   |   | ├─Token(LeftParen) |(|
//@[048:00049) |   | |   |   | ├─FunctionArgumentSyntax
//@[048:00049) |   | |   |   | | └─IntegerLiteralSyntax
//@[048:00049) |   | |   |   | |   └─Token(Integer) |0|
//@[049:00050) |   | |   |   | ├─Token(Comma) |,|
//@[051:00052) |   | |   |   | ├─FunctionArgumentSyntax
//@[051:00052) |   | |   |   | | └─IntegerLiteralSyntax
//@[051:00052) |   | |   |   | |   └─Token(Integer) |4|
//@[052:00053) |   | |   |   | └─Token(RightParen) |)|
//@[053:00054) |   | |   |   ├─Token(Colon) |:|
//@[055:00109) |   | |   |   ├─ObjectSyntax
//@[055:00056) |   | |   |   | ├─Token(LeftBrace) |{|
//@[056:00058) |   | |   |   | ├─Token(NewLine) |\r\n|
      name: 'subnet-${duplicatesEverywhere}'
//@[006:00044) |   | |   |   | ├─ObjectPropertySyntax
//@[006:00010) |   | |   |   | | ├─IdentifierSyntax
//@[006:00010) |   | |   |   | | | └─Token(Identifier) |name|
//@[010:00011) |   | |   |   | | ├─Token(Colon) |:|
//@[012:00044) |   | |   |   | | └─StringSyntax
//@[012:00022) |   | |   |   | |   ├─Token(StringLeftPiece) |'subnet-${|
//@[022:00042) |   | |   |   | |   ├─VariableAccessSyntax
//@[022:00042) |   | |   |   | |   | └─IdentifierSyntax
//@[022:00042) |   | |   |   | |   |   └─Token(Identifier) |duplicatesEverywhere|
//@[042:00044) |   | |   |   | |   └─Token(StringRightPiece) |}'|
//@[044:00046) |   | |   |   | ├─Token(NewLine) |\r\n|
    }]
//@[004:00005) |   | |   |   | └─Token(RightBrace) |}|
//@[005:00006) |   | |   |   └─Token(RightSquare) |]|
//@[006:00008) |   | |   ├─Token(NewLine) |\r\n|
  }
//@[002:00003) |   | |   └─Token(RightBrace) |}|
//@[003:00005) |   | ├─Token(NewLine) |\r\n|
}]
//@[000:00001) |   | └─Token(RightBrace) |}|
//@[001:00002) |   └─Token(RightSquare) |]|
//@[002:00006) ├─Token(NewLine) |\r\n\r\n|

/*
  Scope values created via array access on a resource collection
*/
//@[002:00004) ├─Token(NewLine) |\r\n|
resource dnsZones 'Microsoft.Network/dnsZones@2018-05-01' = [for zone in range(0,4): {
//@[000:00135) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00017) | ├─IdentifierSyntax
//@[009:00017) | | └─Token(Identifier) |dnsZones|
//@[018:00057) | ├─StringSyntax
//@[018:00057) | | └─Token(StringComplete) |'Microsoft.Network/dnsZones@2018-05-01'|
//@[058:00059) | ├─Token(Assignment) |=|
//@[060:00135) | └─ForSyntax
//@[060:00061) |   ├─Token(LeftSquare) |[|
//@[061:00064) |   ├─Token(Identifier) |for|
//@[065:00069) |   ├─LocalVariableSyntax
//@[065:00069) |   | └─IdentifierSyntax
//@[065:00069) |   |   └─Token(Identifier) |zone|
//@[070:00072) |   ├─Token(Identifier) |in|
//@[073:00083) |   ├─FunctionCallSyntax
//@[073:00078) |   | ├─IdentifierSyntax
//@[073:00078) |   | | └─Token(Identifier) |range|
//@[078:00079) |   | ├─Token(LeftParen) |(|
//@[079:00080) |   | ├─FunctionArgumentSyntax
//@[079:00080) |   | | └─IntegerLiteralSyntax
//@[079:00080) |   | |   └─Token(Integer) |0|
//@[080:00081) |   | ├─Token(Comma) |,|
//@[081:00082) |   | ├─FunctionArgumentSyntax
//@[081:00082) |   | | └─IntegerLiteralSyntax
//@[081:00082) |   | |   └─Token(Integer) |4|
//@[082:00083) |   | └─Token(RightParen) |)|
//@[083:00084) |   ├─Token(Colon) |:|
//@[085:00134) |   ├─ObjectSyntax
//@[085:00086) |   | ├─Token(LeftBrace) |{|
//@[086:00088) |   | ├─Token(NewLine) |\r\n|
  name: 'zone${zone}'
//@[002:00021) |   | ├─ObjectPropertySyntax
//@[002:00006) |   | | ├─IdentifierSyntax
//@[002:00006) |   | | | └─Token(Identifier) |name|
//@[006:00007) |   | | ├─Token(Colon) |:|
//@[008:00021) |   | | └─StringSyntax
//@[008:00015) |   | |   ├─Token(StringLeftPiece) |'zone${|
//@[015:00019) |   | |   ├─VariableAccessSyntax
//@[015:00019) |   | |   | └─IdentifierSyntax
//@[015:00019) |   | |   |   └─Token(Identifier) |zone|
//@[019:00021) |   | |   └─Token(StringRightPiece) |}'|
//@[021:00023) |   | ├─Token(NewLine) |\r\n|
  location: 'global'
//@[002:00020) |   | ├─ObjectPropertySyntax
//@[002:00010) |   | | ├─IdentifierSyntax
//@[002:00010) |   | | | └─Token(Identifier) |location|
//@[010:00011) |   | | ├─Token(Colon) |:|
//@[012:00020) |   | | └─StringSyntax
//@[012:00020) |   | |   └─Token(StringComplete) |'global'|
//@[020:00022) |   | ├─Token(NewLine) |\r\n|
}]
//@[000:00001) |   | └─Token(RightBrace) |}|
//@[001:00002) |   └─Token(RightSquare) |]|
//@[002:00006) ├─Token(NewLine) |\r\n\r\n|

resource locksOnZones 'Microsoft.Authorization/locks@2016-09-01' = [for lock in range(0,2): {
//@[000:00194) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00021) | ├─IdentifierSyntax
//@[009:00021) | | └─Token(Identifier) |locksOnZones|
//@[022:00064) | ├─StringSyntax
//@[022:00064) | | └─Token(StringComplete) |'Microsoft.Authorization/locks@2016-09-01'|
//@[065:00066) | ├─Token(Assignment) |=|
//@[067:00194) | └─ForSyntax
//@[067:00068) |   ├─Token(LeftSquare) |[|
//@[068:00071) |   ├─Token(Identifier) |for|
//@[072:00076) |   ├─LocalVariableSyntax
//@[072:00076) |   | └─IdentifierSyntax
//@[072:00076) |   |   └─Token(Identifier) |lock|
//@[077:00079) |   ├─Token(Identifier) |in|
//@[080:00090) |   ├─FunctionCallSyntax
//@[080:00085) |   | ├─IdentifierSyntax
//@[080:00085) |   | | └─Token(Identifier) |range|
//@[085:00086) |   | ├─Token(LeftParen) |(|
//@[086:00087) |   | ├─FunctionArgumentSyntax
//@[086:00087) |   | | └─IntegerLiteralSyntax
//@[086:00087) |   | |   └─Token(Integer) |0|
//@[087:00088) |   | ├─Token(Comma) |,|
//@[088:00089) |   | ├─FunctionArgumentSyntax
//@[088:00089) |   | | └─IntegerLiteralSyntax
//@[088:00089) |   | |   └─Token(Integer) |2|
//@[089:00090) |   | └─Token(RightParen) |)|
//@[090:00091) |   ├─Token(Colon) |:|
//@[092:00193) |   ├─ObjectSyntax
//@[092:00093) |   | ├─Token(LeftBrace) |{|
//@[093:00095) |   | ├─Token(NewLine) |\r\n|
  name: 'lock${lock}'
//@[002:00021) |   | ├─ObjectPropertySyntax
//@[002:00006) |   | | ├─IdentifierSyntax
//@[002:00006) |   | | | └─Token(Identifier) |name|
//@[006:00007) |   | | ├─Token(Colon) |:|
//@[008:00021) |   | | └─StringSyntax
//@[008:00015) |   | |   ├─Token(StringLeftPiece) |'lock${|
//@[015:00019) |   | |   ├─VariableAccessSyntax
//@[015:00019) |   | |   | └─IdentifierSyntax
//@[015:00019) |   | |   |   └─Token(Identifier) |lock|
//@[019:00021) |   | |   └─Token(StringRightPiece) |}'|
//@[021:00023) |   | ├─Token(NewLine) |\r\n|
  properties: {
//@[002:00047) |   | ├─ObjectPropertySyntax
//@[002:00012) |   | | ├─IdentifierSyntax
//@[002:00012) |   | | | └─Token(Identifier) |properties|
//@[012:00013) |   | | ├─Token(Colon) |:|
//@[014:00047) |   | | └─ObjectSyntax
//@[014:00015) |   | |   ├─Token(LeftBrace) |{|
//@[015:00017) |   | |   ├─Token(NewLine) |\r\n|
    level: 'CanNotDelete'
//@[004:00025) |   | |   ├─ObjectPropertySyntax
//@[004:00009) |   | |   | ├─IdentifierSyntax
//@[004:00009) |   | |   | | └─Token(Identifier) |level|
//@[009:00010) |   | |   | ├─Token(Colon) |:|
//@[011:00025) |   | |   | └─StringSyntax
//@[011:00025) |   | |   |   └─Token(StringComplete) |'CanNotDelete'|
//@[025:00027) |   | |   ├─Token(NewLine) |\r\n|
  }
//@[002:00003) |   | |   └─Token(RightBrace) |}|
//@[003:00005) |   | ├─Token(NewLine) |\r\n|
  scope: dnsZones[lock]
//@[002:00023) |   | ├─ObjectPropertySyntax
//@[002:00007) |   | | ├─IdentifierSyntax
//@[002:00007) |   | | | └─Token(Identifier) |scope|
//@[007:00008) |   | | ├─Token(Colon) |:|
//@[009:00023) |   | | └─ArrayAccessSyntax
//@[009:00017) |   | |   ├─VariableAccessSyntax
//@[009:00017) |   | |   | └─IdentifierSyntax
//@[009:00017) |   | |   |   └─Token(Identifier) |dnsZones|
//@[017:00018) |   | |   ├─Token(LeftSquare) |[|
//@[018:00022) |   | |   ├─VariableAccessSyntax
//@[018:00022) |   | |   | └─IdentifierSyntax
//@[018:00022) |   | |   |   └─Token(Identifier) |lock|
//@[022:00023) |   | |   └─Token(RightSquare) |]|
//@[023:00025) |   | ├─Token(NewLine) |\r\n|
}]
//@[000:00001) |   | └─Token(RightBrace) |}|
//@[001:00002) |   └─Token(RightSquare) |]|
//@[002:00006) ├─Token(NewLine) |\r\n\r\n|

resource moreLocksOnZones 'Microsoft.Authorization/locks@2016-09-01' = [for (lock, i) in range(0,3): {
//@[000:00196) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00025) | ├─IdentifierSyntax
//@[009:00025) | | └─Token(Identifier) |moreLocksOnZones|
//@[026:00068) | ├─StringSyntax
//@[026:00068) | | └─Token(StringComplete) |'Microsoft.Authorization/locks@2016-09-01'|
//@[069:00070) | ├─Token(Assignment) |=|
//@[071:00196) | └─ForSyntax
//@[071:00072) |   ├─Token(LeftSquare) |[|
//@[072:00075) |   ├─Token(Identifier) |for|
//@[076:00085) |   ├─VariableBlockSyntax
//@[076:00077) |   | ├─Token(LeftParen) |(|
//@[077:00081) |   | ├─LocalVariableSyntax
//@[077:00081) |   | | └─IdentifierSyntax
//@[077:00081) |   | |   └─Token(Identifier) |lock|
//@[081:00082) |   | ├─Token(Comma) |,|
//@[083:00084) |   | ├─LocalVariableSyntax
//@[083:00084) |   | | └─IdentifierSyntax
//@[083:00084) |   | |   └─Token(Identifier) |i|
//@[084:00085) |   | └─Token(RightParen) |)|
//@[086:00088) |   ├─Token(Identifier) |in|
//@[089:00099) |   ├─FunctionCallSyntax
//@[089:00094) |   | ├─IdentifierSyntax
//@[089:00094) |   | | └─Token(Identifier) |range|
//@[094:00095) |   | ├─Token(LeftParen) |(|
//@[095:00096) |   | ├─FunctionArgumentSyntax
//@[095:00096) |   | | └─IntegerLiteralSyntax
//@[095:00096) |   | |   └─Token(Integer) |0|
//@[096:00097) |   | ├─Token(Comma) |,|
//@[097:00098) |   | ├─FunctionArgumentSyntax
//@[097:00098) |   | | └─IntegerLiteralSyntax
//@[097:00098) |   | |   └─Token(Integer) |3|
//@[098:00099) |   | └─Token(RightParen) |)|
//@[099:00100) |   ├─Token(Colon) |:|
//@[101:00195) |   ├─ObjectSyntax
//@[101:00102) |   | ├─Token(LeftBrace) |{|
//@[102:00104) |   | ├─Token(NewLine) |\r\n|
  name: 'another${i}'
//@[002:00021) |   | ├─ObjectPropertySyntax
//@[002:00006) |   | | ├─IdentifierSyntax
//@[002:00006) |   | | | └─Token(Identifier) |name|
//@[006:00007) |   | | ├─Token(Colon) |:|
//@[008:00021) |   | | └─StringSyntax
//@[008:00018) |   | |   ├─Token(StringLeftPiece) |'another${|
//@[018:00019) |   | |   ├─VariableAccessSyntax
//@[018:00019) |   | |   | └─IdentifierSyntax
//@[018:00019) |   | |   |   └─Token(Identifier) |i|
//@[019:00021) |   | |   └─Token(StringRightPiece) |}'|
//@[021:00023) |   | ├─Token(NewLine) |\r\n|
  properties: {
//@[002:00043) |   | ├─ObjectPropertySyntax
//@[002:00012) |   | | ├─IdentifierSyntax
//@[002:00012) |   | | | └─Token(Identifier) |properties|
//@[012:00013) |   | | ├─Token(Colon) |:|
//@[014:00043) |   | | └─ObjectSyntax
//@[014:00015) |   | |   ├─Token(LeftBrace) |{|
//@[015:00017) |   | |   ├─Token(NewLine) |\r\n|
    level: 'ReadOnly'
//@[004:00021) |   | |   ├─ObjectPropertySyntax
//@[004:00009) |   | |   | ├─IdentifierSyntax
//@[004:00009) |   | |   | | └─Token(Identifier) |level|
//@[009:00010) |   | |   | ├─Token(Colon) |:|
//@[011:00021) |   | |   | └─StringSyntax
//@[011:00021) |   | |   |   └─Token(StringComplete) |'ReadOnly'|
//@[021:00023) |   | |   ├─Token(NewLine) |\r\n|
  }
//@[002:00003) |   | |   └─Token(RightBrace) |}|
//@[003:00005) |   | ├─Token(NewLine) |\r\n|
  scope: dnsZones[i]
//@[002:00020) |   | ├─ObjectPropertySyntax
//@[002:00007) |   | | ├─IdentifierSyntax
//@[002:00007) |   | | | └─Token(Identifier) |scope|
//@[007:00008) |   | | ├─Token(Colon) |:|
//@[009:00020) |   | | └─ArrayAccessSyntax
//@[009:00017) |   | |   ├─VariableAccessSyntax
//@[009:00017) |   | |   | └─IdentifierSyntax
//@[009:00017) |   | |   |   └─Token(Identifier) |dnsZones|
//@[017:00018) |   | |   ├─Token(LeftSquare) |[|
//@[018:00019) |   | |   ├─VariableAccessSyntax
//@[018:00019) |   | |   | └─IdentifierSyntax
//@[018:00019) |   | |   |   └─Token(Identifier) |i|
//@[019:00020) |   | |   └─Token(RightSquare) |]|
//@[020:00022) |   | ├─Token(NewLine) |\r\n|
}]
//@[000:00001) |   | └─Token(RightBrace) |}|
//@[001:00002) |   └─Token(RightSquare) |]|
//@[002:00006) ├─Token(NewLine) |\r\n\r\n|

resource singleLockOnFirstZone 'Microsoft.Authorization/locks@2016-09-01' = {
//@[000:00170) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00030) | ├─IdentifierSyntax
//@[009:00030) | | └─Token(Identifier) |singleLockOnFirstZone|
//@[031:00073) | ├─StringSyntax
//@[031:00073) | | └─Token(StringComplete) |'Microsoft.Authorization/locks@2016-09-01'|
//@[074:00075) | ├─Token(Assignment) |=|
//@[076:00170) | └─ObjectSyntax
//@[076:00077) |   ├─Token(LeftBrace) |{|
//@[077:00079) |   ├─Token(NewLine) |\r\n|
  name: 'single-lock'
//@[002:00021) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00021) |   | └─StringSyntax
//@[008:00021) |   |   └─Token(StringComplete) |'single-lock'|
//@[021:00023) |   ├─Token(NewLine) |\r\n|
  properties: {
//@[002:00043) |   ├─ObjectPropertySyntax
//@[002:00012) |   | ├─IdentifierSyntax
//@[002:00012) |   | | └─Token(Identifier) |properties|
//@[012:00013) |   | ├─Token(Colon) |:|
//@[014:00043) |   | └─ObjectSyntax
//@[014:00015) |   |   ├─Token(LeftBrace) |{|
//@[015:00017) |   |   ├─Token(NewLine) |\r\n|
    level: 'ReadOnly'
//@[004:00021) |   |   ├─ObjectPropertySyntax
//@[004:00009) |   |   | ├─IdentifierSyntax
//@[004:00009) |   |   | | └─Token(Identifier) |level|
//@[009:00010) |   |   | ├─Token(Colon) |:|
//@[011:00021) |   |   | └─StringSyntax
//@[011:00021) |   |   |   └─Token(StringComplete) |'ReadOnly'|
//@[021:00023) |   |   ├─Token(NewLine) |\r\n|
  }
//@[002:00003) |   |   └─Token(RightBrace) |}|
//@[003:00005) |   ├─Token(NewLine) |\r\n|
  scope: dnsZones[0]
//@[002:00020) |   ├─ObjectPropertySyntax
//@[002:00007) |   | ├─IdentifierSyntax
//@[002:00007) |   | | └─Token(Identifier) |scope|
//@[007:00008) |   | ├─Token(Colon) |:|
//@[009:00020) |   | └─ArrayAccessSyntax
//@[009:00017) |   |   ├─VariableAccessSyntax
//@[009:00017) |   |   | └─IdentifierSyntax
//@[009:00017) |   |   |   └─Token(Identifier) |dnsZones|
//@[017:00018) |   |   ├─Token(LeftSquare) |[|
//@[018:00019) |   |   ├─IntegerLiteralSyntax
//@[018:00019) |   |   | └─Token(Integer) |0|
//@[019:00020) |   |   └─Token(RightSquare) |]|
//@[020:00022) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00007) ├─Token(NewLine) |\r\n\r\n\r\n|


resource p1_vnet 'Microsoft.Network/virtualNetworks@2020-06-01' = {
//@[000:00234) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00016) | ├─IdentifierSyntax
//@[009:00016) | | └─Token(Identifier) |p1_vnet|
//@[017:00063) | ├─StringSyntax
//@[017:00063) | | └─Token(StringComplete) |'Microsoft.Network/virtualNetworks@2020-06-01'|
//@[064:00065) | ├─Token(Assignment) |=|
//@[066:00234) | └─ObjectSyntax
//@[066:00067) |   ├─Token(LeftBrace) |{|
//@[067:00069) |   ├─Token(NewLine) |\r\n|
  location: resourceGroup().location
//@[002:00036) |   ├─ObjectPropertySyntax
//@[002:00010) |   | ├─IdentifierSyntax
//@[002:00010) |   | | └─Token(Identifier) |location|
//@[010:00011) |   | ├─Token(Colon) |:|
//@[012:00036) |   | └─PropertyAccessSyntax
//@[012:00027) |   |   ├─FunctionCallSyntax
//@[012:00025) |   |   | ├─IdentifierSyntax
//@[012:00025) |   |   | | └─Token(Identifier) |resourceGroup|
//@[025:00026) |   |   | ├─Token(LeftParen) |(|
//@[026:00027) |   |   | └─Token(RightParen) |)|
//@[027:00028) |   |   ├─Token(Dot) |.|
//@[028:00036) |   |   └─IdentifierSyntax
//@[028:00036) |   |     └─Token(Identifier) |location|
//@[036:00038) |   ├─Token(NewLine) |\r\n|
  name: 'myVnet'
//@[002:00016) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00016) |   | └─StringSyntax
//@[008:00016) |   |   └─Token(StringComplete) |'myVnet'|
//@[016:00018) |   ├─Token(NewLine) |\r\n|
  properties: {
//@[002:00106) |   ├─ObjectPropertySyntax
//@[002:00012) |   | ├─IdentifierSyntax
//@[002:00012) |   | | └─Token(Identifier) |properties|
//@[012:00013) |   | ├─Token(Colon) |:|
//@[014:00106) |   | └─ObjectSyntax
//@[014:00015) |   |   ├─Token(LeftBrace) |{|
//@[015:00017) |   |   ├─Token(NewLine) |\r\n|
    addressSpace: {
//@[004:00084) |   |   ├─ObjectPropertySyntax
//@[004:00016) |   |   | ├─IdentifierSyntax
//@[004:00016) |   |   | | └─Token(Identifier) |addressSpace|
//@[016:00017) |   |   | ├─Token(Colon) |:|
//@[018:00084) |   |   | └─ObjectSyntax
//@[018:00019) |   |   |   ├─Token(LeftBrace) |{|
//@[019:00021) |   |   |   ├─Token(NewLine) |\r\n|
      addressPrefixes: [
//@[006:00056) |   |   |   ├─ObjectPropertySyntax
//@[006:00021) |   |   |   | ├─IdentifierSyntax
//@[006:00021) |   |   |   | | └─Token(Identifier) |addressPrefixes|
//@[021:00022) |   |   |   | ├─Token(Colon) |:|
//@[023:00056) |   |   |   | └─ArraySyntax
//@[023:00024) |   |   |   |   ├─Token(LeftSquare) |[|
//@[024:00026) |   |   |   |   ├─Token(NewLine) |\r\n|
        '10.0.0.0/20'
//@[008:00021) |   |   |   |   ├─ArrayItemSyntax
//@[008:00021) |   |   |   |   | └─StringSyntax
//@[008:00021) |   |   |   |   |   └─Token(StringComplete) |'10.0.0.0/20'|
//@[021:00023) |   |   |   |   ├─Token(NewLine) |\r\n|
      ]
//@[006:00007) |   |   |   |   └─Token(RightSquare) |]|
//@[007:00009) |   |   |   ├─Token(NewLine) |\r\n|
    }
//@[004:00005) |   |   |   └─Token(RightBrace) |}|
//@[005:00007) |   |   ├─Token(NewLine) |\r\n|
  }
//@[002:00003) |   |   └─Token(RightBrace) |}|
//@[003:00005) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

resource p1_subnet1 'Microsoft.Network/virtualNetworks/subnets@2020-06-01' = {
//@[000:00175) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00019) | ├─IdentifierSyntax
//@[009:00019) | | └─Token(Identifier) |p1_subnet1|
//@[020:00074) | ├─StringSyntax
//@[020:00074) | | └─Token(StringComplete) |'Microsoft.Network/virtualNetworks/subnets@2020-06-01'|
//@[075:00076) | ├─Token(Assignment) |=|
//@[077:00175) | └─ObjectSyntax
//@[077:00078) |   ├─Token(LeftBrace) |{|
//@[078:00080) |   ├─Token(NewLine) |\r\n|
  parent: p1_vnet
//@[002:00017) |   ├─ObjectPropertySyntax
//@[002:00008) |   | ├─IdentifierSyntax
//@[002:00008) |   | | └─Token(Identifier) |parent|
//@[008:00009) |   | ├─Token(Colon) |:|
//@[010:00017) |   | └─VariableAccessSyntax
//@[010:00017) |   |   └─IdentifierSyntax
//@[010:00017) |   |     └─Token(Identifier) |p1_vnet|
//@[017:00019) |   ├─Token(NewLine) |\r\n|
  name: 'subnet1'
//@[002:00017) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00017) |   | └─StringSyntax
//@[008:00017) |   |   └─Token(StringComplete) |'subnet1'|
//@[017:00019) |   ├─Token(NewLine) |\r\n|
  properties: {
//@[002:00054) |   ├─ObjectPropertySyntax
//@[002:00012) |   | ├─IdentifierSyntax
//@[002:00012) |   | | └─Token(Identifier) |properties|
//@[012:00013) |   | ├─Token(Colon) |:|
//@[014:00054) |   | └─ObjectSyntax
//@[014:00015) |   |   ├─Token(LeftBrace) |{|
//@[015:00017) |   |   ├─Token(NewLine) |\r\n|
    addressPrefix: '10.0.0.0/24'
//@[004:00032) |   |   ├─ObjectPropertySyntax
//@[004:00017) |   |   | ├─IdentifierSyntax
//@[004:00017) |   |   | | └─Token(Identifier) |addressPrefix|
//@[017:00018) |   |   | ├─Token(Colon) |:|
//@[019:00032) |   |   | └─StringSyntax
//@[019:00032) |   |   |   └─Token(StringComplete) |'10.0.0.0/24'|
//@[032:00034) |   |   ├─Token(NewLine) |\r\n|
  }
//@[002:00003) |   |   └─Token(RightBrace) |}|
//@[003:00005) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

resource p1_subnet2 'Microsoft.Network/virtualNetworks/subnets@2020-06-01' = {
//@[000:00175) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00019) | ├─IdentifierSyntax
//@[009:00019) | | └─Token(Identifier) |p1_subnet2|
//@[020:00074) | ├─StringSyntax
//@[020:00074) | | └─Token(StringComplete) |'Microsoft.Network/virtualNetworks/subnets@2020-06-01'|
//@[075:00076) | ├─Token(Assignment) |=|
//@[077:00175) | └─ObjectSyntax
//@[077:00078) |   ├─Token(LeftBrace) |{|
//@[078:00080) |   ├─Token(NewLine) |\r\n|
  parent: p1_vnet
//@[002:00017) |   ├─ObjectPropertySyntax
//@[002:00008) |   | ├─IdentifierSyntax
//@[002:00008) |   | | └─Token(Identifier) |parent|
//@[008:00009) |   | ├─Token(Colon) |:|
//@[010:00017) |   | └─VariableAccessSyntax
//@[010:00017) |   |   └─IdentifierSyntax
//@[010:00017) |   |     └─Token(Identifier) |p1_vnet|
//@[017:00019) |   ├─Token(NewLine) |\r\n|
  name: 'subnet2'
//@[002:00017) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00017) |   | └─StringSyntax
//@[008:00017) |   |   └─Token(StringComplete) |'subnet2'|
//@[017:00019) |   ├─Token(NewLine) |\r\n|
  properties: {
//@[002:00054) |   ├─ObjectPropertySyntax
//@[002:00012) |   | ├─IdentifierSyntax
//@[002:00012) |   | | └─Token(Identifier) |properties|
//@[012:00013) |   | ├─Token(Colon) |:|
//@[014:00054) |   | └─ObjectSyntax
//@[014:00015) |   |   ├─Token(LeftBrace) |{|
//@[015:00017) |   |   ├─Token(NewLine) |\r\n|
    addressPrefix: '10.0.1.0/24'
//@[004:00032) |   |   ├─ObjectPropertySyntax
//@[004:00017) |   |   | ├─IdentifierSyntax
//@[004:00017) |   |   | | └─Token(Identifier) |addressPrefix|
//@[017:00018) |   |   | ├─Token(Colon) |:|
//@[019:00032) |   |   | └─StringSyntax
//@[019:00032) |   |   |   └─Token(StringComplete) |'10.0.1.0/24'|
//@[032:00034) |   |   ├─Token(NewLine) |\r\n|
  }
//@[002:00003) |   |   └─Token(RightBrace) |}|
//@[003:00005) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

output p1_subnet1prefix string = p1_subnet1.properties.addressPrefix
//@[000:00068) ├─OutputDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |output|
//@[007:00023) | ├─IdentifierSyntax
//@[007:00023) | | └─Token(Identifier) |p1_subnet1prefix|
//@[024:00030) | ├─SimpleTypeSyntax
//@[024:00030) | | └─Token(Identifier) |string|
//@[031:00032) | ├─Token(Assignment) |=|
//@[033:00068) | └─PropertyAccessSyntax
//@[033:00054) |   ├─PropertyAccessSyntax
//@[033:00043) |   | ├─VariableAccessSyntax
//@[033:00043) |   | | └─IdentifierSyntax
//@[033:00043) |   | |   └─Token(Identifier) |p1_subnet1|
//@[043:00044) |   | ├─Token(Dot) |.|
//@[044:00054) |   | └─IdentifierSyntax
//@[044:00054) |   |   └─Token(Identifier) |properties|
//@[054:00055) |   ├─Token(Dot) |.|
//@[055:00068) |   └─IdentifierSyntax
//@[055:00068) |     └─Token(Identifier) |addressPrefix|
//@[068:00070) ├─Token(NewLine) |\r\n|
output p1_subnet1name string = p1_subnet1.name
//@[000:00046) ├─OutputDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |output|
//@[007:00021) | ├─IdentifierSyntax
//@[007:00021) | | └─Token(Identifier) |p1_subnet1name|
//@[022:00028) | ├─SimpleTypeSyntax
//@[022:00028) | | └─Token(Identifier) |string|
//@[029:00030) | ├─Token(Assignment) |=|
//@[031:00046) | └─PropertyAccessSyntax
//@[031:00041) |   ├─VariableAccessSyntax
//@[031:00041) |   | └─IdentifierSyntax
//@[031:00041) |   |   └─Token(Identifier) |p1_subnet1|
//@[041:00042) |   ├─Token(Dot) |.|
//@[042:00046) |   └─IdentifierSyntax
//@[042:00046) |     └─Token(Identifier) |name|
//@[046:00048) ├─Token(NewLine) |\r\n|
output p1_subnet1type string = p1_subnet1.type
//@[000:00046) ├─OutputDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |output|
//@[007:00021) | ├─IdentifierSyntax
//@[007:00021) | | └─Token(Identifier) |p1_subnet1type|
//@[022:00028) | ├─SimpleTypeSyntax
//@[022:00028) | | └─Token(Identifier) |string|
//@[029:00030) | ├─Token(Assignment) |=|
//@[031:00046) | └─PropertyAccessSyntax
//@[031:00041) |   ├─VariableAccessSyntax
//@[031:00041) |   | └─IdentifierSyntax
//@[031:00041) |   |   └─Token(Identifier) |p1_subnet1|
//@[041:00042) |   ├─Token(Dot) |.|
//@[042:00046) |   └─IdentifierSyntax
//@[042:00046) |     └─Token(Identifier) |type|
//@[046:00048) ├─Token(NewLine) |\r\n|
output p1_subnet1id string = p1_subnet1.id
//@[000:00042) ├─OutputDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |output|
//@[007:00019) | ├─IdentifierSyntax
//@[007:00019) | | └─Token(Identifier) |p1_subnet1id|
//@[020:00026) | ├─SimpleTypeSyntax
//@[020:00026) | | └─Token(Identifier) |string|
//@[027:00028) | ├─Token(Assignment) |=|
//@[029:00042) | └─PropertyAccessSyntax
//@[029:00039) |   ├─VariableAccessSyntax
//@[029:00039) |   | └─IdentifierSyntax
//@[029:00039) |   |   └─Token(Identifier) |p1_subnet1|
//@[039:00040) |   ├─Token(Dot) |.|
//@[040:00042) |   └─IdentifierSyntax
//@[040:00042) |     └─Token(Identifier) |id|
//@[042:00046) ├─Token(NewLine) |\r\n\r\n|

// parent property with extension resource
//@[042:00044) ├─Token(NewLine) |\r\n|
resource p2_res1 'Microsoft.Rp1/resource1@2020-06-01' = {
//@[000:00076) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00016) | ├─IdentifierSyntax
//@[009:00016) | | └─Token(Identifier) |p2_res1|
//@[017:00053) | ├─StringSyntax
//@[017:00053) | | └─Token(StringComplete) |'Microsoft.Rp1/resource1@2020-06-01'|
//@[054:00055) | ├─Token(Assignment) |=|
//@[056:00076) | └─ObjectSyntax
//@[056:00057) |   ├─Token(LeftBrace) |{|
//@[057:00059) |   ├─Token(NewLine) |\r\n|
  name: 'res1'
//@[002:00014) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00014) |   | └─StringSyntax
//@[008:00014) |   |   └─Token(StringComplete) |'res1'|
//@[014:00016) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

resource p2_res1child 'Microsoft.Rp1/resource1/child1@2020-06-01' = {
//@[000:00109) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00021) | ├─IdentifierSyntax
//@[009:00021) | | └─Token(Identifier) |p2_res1child|
//@[022:00065) | ├─StringSyntax
//@[022:00065) | | └─Token(StringComplete) |'Microsoft.Rp1/resource1/child1@2020-06-01'|
//@[066:00067) | ├─Token(Assignment) |=|
//@[068:00109) | └─ObjectSyntax
//@[068:00069) |   ├─Token(LeftBrace) |{|
//@[069:00071) |   ├─Token(NewLine) |\r\n|
  parent: p2_res1
//@[002:00017) |   ├─ObjectPropertySyntax
//@[002:00008) |   | ├─IdentifierSyntax
//@[002:00008) |   | | └─Token(Identifier) |parent|
//@[008:00009) |   | ├─Token(Colon) |:|
//@[010:00017) |   | └─VariableAccessSyntax
//@[010:00017) |   |   └─IdentifierSyntax
//@[010:00017) |   |     └─Token(Identifier) |p2_res1|
//@[017:00019) |   ├─Token(NewLine) |\r\n|
  name: 'child1'
//@[002:00016) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00016) |   | └─StringSyntax
//@[008:00016) |   |   └─Token(StringComplete) |'child1'|
//@[016:00018) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

resource p2_res2 'Microsoft.Rp2/resource2@2020-06-01' = {
//@[000:00099) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00016) | ├─IdentifierSyntax
//@[009:00016) | | └─Token(Identifier) |p2_res2|
//@[017:00053) | ├─StringSyntax
//@[017:00053) | | └─Token(StringComplete) |'Microsoft.Rp2/resource2@2020-06-01'|
//@[054:00055) | ├─Token(Assignment) |=|
//@[056:00099) | └─ObjectSyntax
//@[056:00057) |   ├─Token(LeftBrace) |{|
//@[057:00059) |   ├─Token(NewLine) |\r\n|
  scope: p2_res1child
//@[002:00021) |   ├─ObjectPropertySyntax
//@[002:00007) |   | ├─IdentifierSyntax
//@[002:00007) |   | | └─Token(Identifier) |scope|
//@[007:00008) |   | ├─Token(Colon) |:|
//@[009:00021) |   | └─VariableAccessSyntax
//@[009:00021) |   |   └─IdentifierSyntax
//@[009:00021) |   |     └─Token(Identifier) |p2_res1child|
//@[021:00023) |   ├─Token(NewLine) |\r\n|
  name: 'res2'
//@[002:00014) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00014) |   | └─StringSyntax
//@[008:00014) |   |   └─Token(StringComplete) |'res2'|
//@[014:00016) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

resource p2_res2child 'Microsoft.Rp2/resource2/child2@2020-06-01' = {
//@[000:00109) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00021) | ├─IdentifierSyntax
//@[009:00021) | | └─Token(Identifier) |p2_res2child|
//@[022:00065) | ├─StringSyntax
//@[022:00065) | | └─Token(StringComplete) |'Microsoft.Rp2/resource2/child2@2020-06-01'|
//@[066:00067) | ├─Token(Assignment) |=|
//@[068:00109) | └─ObjectSyntax
//@[068:00069) |   ├─Token(LeftBrace) |{|
//@[069:00071) |   ├─Token(NewLine) |\r\n|
  parent: p2_res2
//@[002:00017) |   ├─ObjectPropertySyntax
//@[002:00008) |   | ├─IdentifierSyntax
//@[002:00008) |   | | └─Token(Identifier) |parent|
//@[008:00009) |   | ├─Token(Colon) |:|
//@[010:00017) |   | └─VariableAccessSyntax
//@[010:00017) |   |   └─IdentifierSyntax
//@[010:00017) |   |     └─Token(Identifier) |p2_res2|
//@[017:00019) |   ├─Token(NewLine) |\r\n|
  name: 'child2'
//@[002:00016) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00016) |   | └─StringSyntax
//@[008:00016) |   |   └─Token(StringComplete) |'child2'|
//@[016:00018) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

output p2_res2childprop string = p2_res2child.properties.someProp
//@[000:00065) ├─OutputDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |output|
//@[007:00023) | ├─IdentifierSyntax
//@[007:00023) | | └─Token(Identifier) |p2_res2childprop|
//@[024:00030) | ├─SimpleTypeSyntax
//@[024:00030) | | └─Token(Identifier) |string|
//@[031:00032) | ├─Token(Assignment) |=|
//@[033:00065) | └─PropertyAccessSyntax
//@[033:00056) |   ├─PropertyAccessSyntax
//@[033:00045) |   | ├─VariableAccessSyntax
//@[033:00045) |   | | └─IdentifierSyntax
//@[033:00045) |   | |   └─Token(Identifier) |p2_res2child|
//@[045:00046) |   | ├─Token(Dot) |.|
//@[046:00056) |   | └─IdentifierSyntax
//@[046:00056) |   |   └─Token(Identifier) |properties|
//@[056:00057) |   ├─Token(Dot) |.|
//@[057:00065) |   └─IdentifierSyntax
//@[057:00065) |     └─Token(Identifier) |someProp|
//@[065:00067) ├─Token(NewLine) |\r\n|
output p2_res2childname string = p2_res2child.name
//@[000:00050) ├─OutputDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |output|
//@[007:00023) | ├─IdentifierSyntax
//@[007:00023) | | └─Token(Identifier) |p2_res2childname|
//@[024:00030) | ├─SimpleTypeSyntax
//@[024:00030) | | └─Token(Identifier) |string|
//@[031:00032) | ├─Token(Assignment) |=|
//@[033:00050) | └─PropertyAccessSyntax
//@[033:00045) |   ├─VariableAccessSyntax
//@[033:00045) |   | └─IdentifierSyntax
//@[033:00045) |   |   └─Token(Identifier) |p2_res2child|
//@[045:00046) |   ├─Token(Dot) |.|
//@[046:00050) |   └─IdentifierSyntax
//@[046:00050) |     └─Token(Identifier) |name|
//@[050:00052) ├─Token(NewLine) |\r\n|
output p2_res2childtype string = p2_res2child.type
//@[000:00050) ├─OutputDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |output|
//@[007:00023) | ├─IdentifierSyntax
//@[007:00023) | | └─Token(Identifier) |p2_res2childtype|
//@[024:00030) | ├─SimpleTypeSyntax
//@[024:00030) | | └─Token(Identifier) |string|
//@[031:00032) | ├─Token(Assignment) |=|
//@[033:00050) | └─PropertyAccessSyntax
//@[033:00045) |   ├─VariableAccessSyntax
//@[033:00045) |   | └─IdentifierSyntax
//@[033:00045) |   |   └─Token(Identifier) |p2_res2child|
//@[045:00046) |   ├─Token(Dot) |.|
//@[046:00050) |   └─IdentifierSyntax
//@[046:00050) |     └─Token(Identifier) |type|
//@[050:00052) ├─Token(NewLine) |\r\n|
output p2_res2childid string = p2_res2child.id
//@[000:00046) ├─OutputDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |output|
//@[007:00021) | ├─IdentifierSyntax
//@[007:00021) | | └─Token(Identifier) |p2_res2childid|
//@[022:00028) | ├─SimpleTypeSyntax
//@[022:00028) | | └─Token(Identifier) |string|
//@[029:00030) | ├─Token(Assignment) |=|
//@[031:00046) | └─PropertyAccessSyntax
//@[031:00043) |   ├─VariableAccessSyntax
//@[031:00043) |   | └─IdentifierSyntax
//@[031:00043) |   |   └─Token(Identifier) |p2_res2child|
//@[043:00044) |   ├─Token(Dot) |.|
//@[044:00046) |   └─IdentifierSyntax
//@[044:00046) |     └─Token(Identifier) |id|
//@[046:00050) ├─Token(NewLine) |\r\n\r\n|

// parent property with 'existing' resource
//@[043:00045) ├─Token(NewLine) |\r\n|
resource p3_res1 'Microsoft.Rp1/resource1@2020-06-01' existing = {
//@[000:00085) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00016) | ├─IdentifierSyntax
//@[009:00016) | | └─Token(Identifier) |p3_res1|
//@[017:00053) | ├─StringSyntax
//@[017:00053) | | └─Token(StringComplete) |'Microsoft.Rp1/resource1@2020-06-01'|
//@[054:00062) | ├─Token(Identifier) |existing|
//@[063:00064) | ├─Token(Assignment) |=|
//@[065:00085) | └─ObjectSyntax
//@[065:00066) |   ├─Token(LeftBrace) |{|
//@[066:00068) |   ├─Token(NewLine) |\r\n|
  name: 'res1'
//@[002:00014) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00014) |   | └─StringSyntax
//@[008:00014) |   |   └─Token(StringComplete) |'res1'|
//@[014:00016) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

resource p3_child1 'Microsoft.Rp1/resource1/child1@2020-06-01' = {
//@[000:00106) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00018) | ├─IdentifierSyntax
//@[009:00018) | | └─Token(Identifier) |p3_child1|
//@[019:00062) | ├─StringSyntax
//@[019:00062) | | └─Token(StringComplete) |'Microsoft.Rp1/resource1/child1@2020-06-01'|
//@[063:00064) | ├─Token(Assignment) |=|
//@[065:00106) | └─ObjectSyntax
//@[065:00066) |   ├─Token(LeftBrace) |{|
//@[066:00068) |   ├─Token(NewLine) |\r\n|
  parent: p3_res1
//@[002:00017) |   ├─ObjectPropertySyntax
//@[002:00008) |   | ├─IdentifierSyntax
//@[002:00008) |   | | └─Token(Identifier) |parent|
//@[008:00009) |   | ├─Token(Colon) |:|
//@[010:00017) |   | └─VariableAccessSyntax
//@[010:00017) |   |   └─IdentifierSyntax
//@[010:00017) |   |     └─Token(Identifier) |p3_res1|
//@[017:00019) |   ├─Token(NewLine) |\r\n|
  name: 'child1'
//@[002:00016) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00016) |   | └─StringSyntax
//@[008:00016) |   |   └─Token(StringComplete) |'child1'|
//@[016:00018) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

output p3_res1childprop string = p3_child1.properties.someProp
//@[000:00062) ├─OutputDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |output|
//@[007:00023) | ├─IdentifierSyntax
//@[007:00023) | | └─Token(Identifier) |p3_res1childprop|
//@[024:00030) | ├─SimpleTypeSyntax
//@[024:00030) | | └─Token(Identifier) |string|
//@[031:00032) | ├─Token(Assignment) |=|
//@[033:00062) | └─PropertyAccessSyntax
//@[033:00053) |   ├─PropertyAccessSyntax
//@[033:00042) |   | ├─VariableAccessSyntax
//@[033:00042) |   | | └─IdentifierSyntax
//@[033:00042) |   | |   └─Token(Identifier) |p3_child1|
//@[042:00043) |   | ├─Token(Dot) |.|
//@[043:00053) |   | └─IdentifierSyntax
//@[043:00053) |   |   └─Token(Identifier) |properties|
//@[053:00054) |   ├─Token(Dot) |.|
//@[054:00062) |   └─IdentifierSyntax
//@[054:00062) |     └─Token(Identifier) |someProp|
//@[062:00064) ├─Token(NewLine) |\r\n|
output p3_res1childname string = p3_child1.name
//@[000:00047) ├─OutputDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |output|
//@[007:00023) | ├─IdentifierSyntax
//@[007:00023) | | └─Token(Identifier) |p3_res1childname|
//@[024:00030) | ├─SimpleTypeSyntax
//@[024:00030) | | └─Token(Identifier) |string|
//@[031:00032) | ├─Token(Assignment) |=|
//@[033:00047) | └─PropertyAccessSyntax
//@[033:00042) |   ├─VariableAccessSyntax
//@[033:00042) |   | └─IdentifierSyntax
//@[033:00042) |   |   └─Token(Identifier) |p3_child1|
//@[042:00043) |   ├─Token(Dot) |.|
//@[043:00047) |   └─IdentifierSyntax
//@[043:00047) |     └─Token(Identifier) |name|
//@[047:00049) ├─Token(NewLine) |\r\n|
output p3_res1childtype string = p3_child1.type
//@[000:00047) ├─OutputDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |output|
//@[007:00023) | ├─IdentifierSyntax
//@[007:00023) | | └─Token(Identifier) |p3_res1childtype|
//@[024:00030) | ├─SimpleTypeSyntax
//@[024:00030) | | └─Token(Identifier) |string|
//@[031:00032) | ├─Token(Assignment) |=|
//@[033:00047) | └─PropertyAccessSyntax
//@[033:00042) |   ├─VariableAccessSyntax
//@[033:00042) |   | └─IdentifierSyntax
//@[033:00042) |   |   └─Token(Identifier) |p3_child1|
//@[042:00043) |   ├─Token(Dot) |.|
//@[043:00047) |   └─IdentifierSyntax
//@[043:00047) |     └─Token(Identifier) |type|
//@[047:00049) ├─Token(NewLine) |\r\n|
output p3_res1childid string = p3_child1.id
//@[000:00043) ├─OutputDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |output|
//@[007:00021) | ├─IdentifierSyntax
//@[007:00021) | | └─Token(Identifier) |p3_res1childid|
//@[022:00028) | ├─SimpleTypeSyntax
//@[022:00028) | | └─Token(Identifier) |string|
//@[029:00030) | ├─Token(Assignment) |=|
//@[031:00043) | └─PropertyAccessSyntax
//@[031:00040) |   ├─VariableAccessSyntax
//@[031:00040) |   | └─IdentifierSyntax
//@[031:00040) |   |   └─Token(Identifier) |p3_child1|
//@[040:00041) |   ├─Token(Dot) |.|
//@[041:00043) |   └─IdentifierSyntax
//@[041:00043) |     └─Token(Identifier) |id|
//@[043:00047) ├─Token(NewLine) |\r\n\r\n|

// parent & child with 'existing'
//@[033:00035) ├─Token(NewLine) |\r\n|
resource p4_res1 'Microsoft.Rp1/resource1@2020-06-01' existing = {
//@[000:00104) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00016) | ├─IdentifierSyntax
//@[009:00016) | | └─Token(Identifier) |p4_res1|
//@[017:00053) | ├─StringSyntax
//@[017:00053) | | └─Token(StringComplete) |'Microsoft.Rp1/resource1@2020-06-01'|
//@[054:00062) | ├─Token(Identifier) |existing|
//@[063:00064) | ├─Token(Assignment) |=|
//@[065:00104) | └─ObjectSyntax
//@[065:00066) |   ├─Token(LeftBrace) |{|
//@[066:00068) |   ├─Token(NewLine) |\r\n|
  scope: tenant()
//@[002:00017) |   ├─ObjectPropertySyntax
//@[002:00007) |   | ├─IdentifierSyntax
//@[002:00007) |   | | └─Token(Identifier) |scope|
//@[007:00008) |   | ├─Token(Colon) |:|
//@[009:00017) |   | └─FunctionCallSyntax
//@[009:00015) |   |   ├─IdentifierSyntax
//@[009:00015) |   |   | └─Token(Identifier) |tenant|
//@[015:00016) |   |   ├─Token(LeftParen) |(|
//@[016:00017) |   |   └─Token(RightParen) |)|
//@[017:00019) |   ├─Token(NewLine) |\r\n|
  name: 'res1'
//@[002:00014) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00014) |   | └─StringSyntax
//@[008:00014) |   |   └─Token(StringComplete) |'res1'|
//@[014:00016) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

resource p4_child1 'Microsoft.Rp1/resource1/child1@2020-06-01' existing = {
//@[000:00115) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00018) | ├─IdentifierSyntax
//@[009:00018) | | └─Token(Identifier) |p4_child1|
//@[019:00062) | ├─StringSyntax
//@[019:00062) | | └─Token(StringComplete) |'Microsoft.Rp1/resource1/child1@2020-06-01'|
//@[063:00071) | ├─Token(Identifier) |existing|
//@[072:00073) | ├─Token(Assignment) |=|
//@[074:00115) | └─ObjectSyntax
//@[074:00075) |   ├─Token(LeftBrace) |{|
//@[075:00077) |   ├─Token(NewLine) |\r\n|
  parent: p4_res1
//@[002:00017) |   ├─ObjectPropertySyntax
//@[002:00008) |   | ├─IdentifierSyntax
//@[002:00008) |   | | └─Token(Identifier) |parent|
//@[008:00009) |   | ├─Token(Colon) |:|
//@[010:00017) |   | └─VariableAccessSyntax
//@[010:00017) |   |   └─IdentifierSyntax
//@[010:00017) |   |     └─Token(Identifier) |p4_res1|
//@[017:00019) |   ├─Token(NewLine) |\r\n|
  name: 'child1'
//@[002:00016) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00016) |   | └─StringSyntax
//@[008:00016) |   |   └─Token(StringComplete) |'child1'|
//@[016:00018) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

output p4_res1childprop string = p4_child1.properties.someProp
//@[000:00062) ├─OutputDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |output|
//@[007:00023) | ├─IdentifierSyntax
//@[007:00023) | | └─Token(Identifier) |p4_res1childprop|
//@[024:00030) | ├─SimpleTypeSyntax
//@[024:00030) | | └─Token(Identifier) |string|
//@[031:00032) | ├─Token(Assignment) |=|
//@[033:00062) | └─PropertyAccessSyntax
//@[033:00053) |   ├─PropertyAccessSyntax
//@[033:00042) |   | ├─VariableAccessSyntax
//@[033:00042) |   | | └─IdentifierSyntax
//@[033:00042) |   | |   └─Token(Identifier) |p4_child1|
//@[042:00043) |   | ├─Token(Dot) |.|
//@[043:00053) |   | └─IdentifierSyntax
//@[043:00053) |   |   └─Token(Identifier) |properties|
//@[053:00054) |   ├─Token(Dot) |.|
//@[054:00062) |   └─IdentifierSyntax
//@[054:00062) |     └─Token(Identifier) |someProp|
//@[062:00064) ├─Token(NewLine) |\r\n|
output p4_res1childname string = p4_child1.name
//@[000:00047) ├─OutputDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |output|
//@[007:00023) | ├─IdentifierSyntax
//@[007:00023) | | └─Token(Identifier) |p4_res1childname|
//@[024:00030) | ├─SimpleTypeSyntax
//@[024:00030) | | └─Token(Identifier) |string|
//@[031:00032) | ├─Token(Assignment) |=|
//@[033:00047) | └─PropertyAccessSyntax
//@[033:00042) |   ├─VariableAccessSyntax
//@[033:00042) |   | └─IdentifierSyntax
//@[033:00042) |   |   └─Token(Identifier) |p4_child1|
//@[042:00043) |   ├─Token(Dot) |.|
//@[043:00047) |   └─IdentifierSyntax
//@[043:00047) |     └─Token(Identifier) |name|
//@[047:00049) ├─Token(NewLine) |\r\n|
output p4_res1childtype string = p4_child1.type
//@[000:00047) ├─OutputDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |output|
//@[007:00023) | ├─IdentifierSyntax
//@[007:00023) | | └─Token(Identifier) |p4_res1childtype|
//@[024:00030) | ├─SimpleTypeSyntax
//@[024:00030) | | └─Token(Identifier) |string|
//@[031:00032) | ├─Token(Assignment) |=|
//@[033:00047) | └─PropertyAccessSyntax
//@[033:00042) |   ├─VariableAccessSyntax
//@[033:00042) |   | └─IdentifierSyntax
//@[033:00042) |   |   └─Token(Identifier) |p4_child1|
//@[042:00043) |   ├─Token(Dot) |.|
//@[043:00047) |   └─IdentifierSyntax
//@[043:00047) |     └─Token(Identifier) |type|
//@[047:00049) ├─Token(NewLine) |\r\n|
output p4_res1childid string = p4_child1.id
//@[000:00043) ├─OutputDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |output|
//@[007:00021) | ├─IdentifierSyntax
//@[007:00021) | | └─Token(Identifier) |p4_res1childid|
//@[022:00028) | ├─SimpleTypeSyntax
//@[022:00028) | | └─Token(Identifier) |string|
//@[029:00030) | ├─Token(Assignment) |=|
//@[031:00043) | └─PropertyAccessSyntax
//@[031:00040) |   ├─VariableAccessSyntax
//@[031:00040) |   | └─IdentifierSyntax
//@[031:00040) |   |   └─Token(Identifier) |p4_child1|
//@[040:00041) |   ├─Token(Dot) |.|
//@[041:00043) |   └─IdentifierSyntax
//@[041:00043) |     └─Token(Identifier) |id|
//@[043:00045) ├─Token(NewLine) |\r\n|

//@[000:00000) └─Token(EndOfFile) ||
