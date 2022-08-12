param name string
//@[000:11469) ProgramSyntax
//@[000:00017) ├─ParameterDeclarationSyntax
//@[000:00005) | ├─Token(Identifier) |param|
//@[006:00010) | ├─IdentifierSyntax
//@[006:00010) | | └─Token(Identifier) |name|
//@[011:00017) | └─SimpleTypeSyntax
//@[011:00017) |   └─Token(Identifier) |string|
//@[017:00018) ├─Token(NewLine) |\n|
param accounts array
//@[000:00020) ├─ParameterDeclarationSyntax
//@[000:00005) | ├─Token(Identifier) |param|
//@[006:00014) | ├─IdentifierSyntax
//@[006:00014) | | └─Token(Identifier) |accounts|
//@[015:00020) | └─SimpleTypeSyntax
//@[015:00020) |   └─Token(Identifier) |array|
//@[020:00021) ├─Token(NewLine) |\n|
param index int
//@[000:00015) ├─ParameterDeclarationSyntax
//@[000:00005) | ├─Token(Identifier) |param|
//@[006:00011) | ├─IdentifierSyntax
//@[006:00011) | | └─Token(Identifier) |index|
//@[012:00015) | └─SimpleTypeSyntax
//@[012:00015) |   └─Token(Identifier) |int|
//@[015:00017) ├─Token(NewLine) |\n\n|

// single resource
//@[018:00019) ├─Token(NewLine) |\n|
resource singleResource 'Microsoft.Storage/storageAccounts@2019-06-01' = {
//@[000:00209) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00023) | ├─IdentifierSyntax
//@[009:00023) | | └─Token(Identifier) |singleResource|
//@[024:00070) | ├─StringSyntax
//@[024:00070) | | └─Token(StringComplete) |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[071:00072) | ├─Token(Assignment) |=|
//@[073:00209) | └─ObjectSyntax
//@[073:00074) |   ├─Token(LeftBrace) |{|
//@[074:00075) |   ├─Token(NewLine) |\n|
  name: '${name}single-resource-name'
//@[002:00037) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00037) |   | └─StringSyntax
//@[008:00011) |   |   ├─Token(StringLeftPiece) |'${|
//@[011:00015) |   |   ├─VariableAccessSyntax
//@[011:00015) |   |   | └─IdentifierSyntax
//@[011:00015) |   |   |   └─Token(Identifier) |name|
//@[015:00037) |   |   └─Token(StringRightPiece) |}single-resource-name'|
//@[037:00038) |   ├─Token(NewLine) |\n|
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
//@[036:00037) |   ├─Token(NewLine) |\n|
  kind: 'StorageV2'
//@[002:00019) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |kind|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00019) |   | └─StringSyntax
//@[008:00019) |   |   └─Token(StringComplete) |'StorageV2'|
//@[019:00020) |   ├─Token(NewLine) |\n|
  sku: {
//@[002:00037) |   ├─ObjectPropertySyntax
//@[002:00005) |   | ├─IdentifierSyntax
//@[002:00005) |   | | └─Token(Identifier) |sku|
//@[005:00006) |   | ├─Token(Colon) |:|
//@[007:00037) |   | └─ObjectSyntax
//@[007:00008) |   |   ├─Token(LeftBrace) |{|
//@[008:00009) |   |   ├─Token(NewLine) |\n|
    name: 'Standard_LRS'
//@[004:00024) |   |   ├─ObjectPropertySyntax
//@[004:00008) |   |   | ├─IdentifierSyntax
//@[004:00008) |   |   | | └─Token(Identifier) |name|
//@[008:00009) |   |   | ├─Token(Colon) |:|
//@[010:00024) |   |   | └─StringSyntax
//@[010:00024) |   |   |   └─Token(StringComplete) |'Standard_LRS'|
//@[024:00025) |   |   ├─Token(NewLine) |\n|
  }
//@[002:00003) |   |   └─Token(RightBrace) |}|
//@[003:00004) |   ├─Token(NewLine) |\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00003) ├─Token(NewLine) |\n\n|

// extension of single resource
//@[031:00032) ├─Token(NewLine) |\n|
resource singleResourceExtension 'Microsoft.Authorization/locks@2016-09-01' = {
//@[000:00182) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00032) | ├─IdentifierSyntax
//@[009:00032) | | └─Token(Identifier) |singleResourceExtension|
//@[033:00075) | ├─StringSyntax
//@[033:00075) | | └─Token(StringComplete) |'Microsoft.Authorization/locks@2016-09-01'|
//@[076:00077) | ├─Token(Assignment) |=|
//@[078:00182) | └─ObjectSyntax
//@[078:00079) |   ├─Token(LeftBrace) |{|
//@[079:00080) |   ├─Token(NewLine) |\n|
  scope: singleResource
//@[002:00023) |   ├─ObjectPropertySyntax
//@[002:00007) |   | ├─IdentifierSyntax
//@[002:00007) |   | | └─Token(Identifier) |scope|
//@[007:00008) |   | ├─Token(Colon) |:|
//@[009:00023) |   | └─VariableAccessSyntax
//@[009:00023) |   |   └─IdentifierSyntax
//@[009:00023) |   |     └─Token(Identifier) |singleResource|
//@[023:00024) |   ├─Token(NewLine) |\n|
  name: 'single-resource-lock'
//@[002:00030) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00030) |   | └─StringSyntax
//@[008:00030) |   |   └─Token(StringComplete) |'single-resource-lock'|
//@[030:00031) |   ├─Token(NewLine) |\n|
  properties: {
//@[002:00045) |   ├─ObjectPropertySyntax
//@[002:00012) |   | ├─IdentifierSyntax
//@[002:00012) |   | | └─Token(Identifier) |properties|
//@[012:00013) |   | ├─Token(Colon) |:|
//@[014:00045) |   | └─ObjectSyntax
//@[014:00015) |   |   ├─Token(LeftBrace) |{|
//@[015:00016) |   |   ├─Token(NewLine) |\n|
    level: 'CanNotDelete'
//@[004:00025) |   |   ├─ObjectPropertySyntax
//@[004:00009) |   |   | ├─IdentifierSyntax
//@[004:00009) |   |   | | └─Token(Identifier) |level|
//@[009:00010) |   |   | ├─Token(Colon) |:|
//@[011:00025) |   |   | └─StringSyntax
//@[011:00025) |   |   |   └─Token(StringComplete) |'CanNotDelete'|
//@[025:00026) |   |   ├─Token(NewLine) |\n|
  }
//@[002:00003) |   |   └─Token(RightBrace) |}|
//@[003:00004) |   ├─Token(NewLine) |\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00003) ├─Token(NewLine) |\n\n|

// single resource cascade extension
//@[036:00037) ├─Token(NewLine) |\n|
resource singleResourceCascadeExtension 'Microsoft.Authorization/locks@2016-09-01' = {
//@[000:00211) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00039) | ├─IdentifierSyntax
//@[009:00039) | | └─Token(Identifier) |singleResourceCascadeExtension|
//@[040:00082) | ├─StringSyntax
//@[040:00082) | | └─Token(StringComplete) |'Microsoft.Authorization/locks@2016-09-01'|
//@[083:00084) | ├─Token(Assignment) |=|
//@[085:00211) | └─ObjectSyntax
//@[085:00086) |   ├─Token(LeftBrace) |{|
//@[086:00087) |   ├─Token(NewLine) |\n|
  scope: singleResourceExtension
//@[002:00032) |   ├─ObjectPropertySyntax
//@[002:00007) |   | ├─IdentifierSyntax
//@[002:00007) |   | | └─Token(Identifier) |scope|
//@[007:00008) |   | ├─Token(Colon) |:|
//@[009:00032) |   | └─VariableAccessSyntax
//@[009:00032) |   |   └─IdentifierSyntax
//@[009:00032) |   |     └─Token(Identifier) |singleResourceExtension|
//@[032:00033) |   ├─Token(NewLine) |\n|
  name: 'single-resource-cascade-extension'
//@[002:00043) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00043) |   | └─StringSyntax
//@[008:00043) |   |   └─Token(StringComplete) |'single-resource-cascade-extension'|
//@[043:00044) |   ├─Token(NewLine) |\n|
  properties: {
//@[002:00045) |   ├─ObjectPropertySyntax
//@[002:00012) |   | ├─IdentifierSyntax
//@[002:00012) |   | | └─Token(Identifier) |properties|
//@[012:00013) |   | ├─Token(Colon) |:|
//@[014:00045) |   | └─ObjectSyntax
//@[014:00015) |   |   ├─Token(LeftBrace) |{|
//@[015:00016) |   |   ├─Token(NewLine) |\n|
    level: 'CanNotDelete'
//@[004:00025) |   |   ├─ObjectPropertySyntax
//@[004:00009) |   |   | ├─IdentifierSyntax
//@[004:00009) |   |   | | └─Token(Identifier) |level|
//@[009:00010) |   |   | ├─Token(Colon) |:|
//@[011:00025) |   |   | └─StringSyntax
//@[011:00025) |   |   |   └─Token(StringComplete) |'CanNotDelete'|
//@[025:00026) |   |   ├─Token(NewLine) |\n|
  }
//@[002:00003) |   |   └─Token(RightBrace) |}|
//@[003:00004) |   ├─Token(NewLine) |\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00003) ├─Token(NewLine) |\n\n|

// resource collection
//@[022:00023) ├─Token(NewLine) |\n|
@batchSize(42)
//@[000:00307) ├─ResourceDeclarationSyntax
//@[000:00014) | ├─DecoratorSyntax
//@[000:00001) | | ├─Token(At) |@|
//@[001:00014) | | └─FunctionCallSyntax
//@[001:00010) | |   ├─IdentifierSyntax
//@[001:00010) | |   | └─Token(Identifier) |batchSize|
//@[010:00011) | |   ├─Token(LeftParen) |(|
//@[011:00013) | |   ├─FunctionArgumentSyntax
//@[011:00013) | |   | └─IntegerLiteralSyntax
//@[011:00013) | |   |   └─Token(Integer) |42|
//@[013:00014) | |   └─Token(RightParen) |)|
//@[014:00015) | ├─Token(NewLine) |\n|
resource storageAccounts 'Microsoft.Storage/storageAccounts@2019-06-01' = [for (account, index) in accounts: {
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00024) | ├─IdentifierSyntax
//@[009:00024) | | └─Token(Identifier) |storageAccounts|
//@[025:00071) | ├─StringSyntax
//@[025:00071) | | └─Token(StringComplete) |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[072:00073) | ├─Token(Assignment) |=|
//@[074:00292) | └─ForSyntax
//@[074:00075) |   ├─Token(LeftSquare) |[|
//@[075:00078) |   ├─Token(Identifier) |for|
//@[079:00095) |   ├─VariableBlockSyntax
//@[079:00080) |   | ├─Token(LeftParen) |(|
//@[080:00087) |   | ├─LocalVariableSyntax
//@[080:00087) |   | | └─IdentifierSyntax
//@[080:00087) |   | |   └─Token(Identifier) |account|
//@[087:00088) |   | ├─Token(Comma) |,|
//@[089:00094) |   | ├─LocalVariableSyntax
//@[089:00094) |   | | └─IdentifierSyntax
//@[089:00094) |   | |   └─Token(Identifier) |index|
//@[094:00095) |   | └─Token(RightParen) |)|
//@[096:00098) |   ├─Token(Identifier) |in|
//@[099:00107) |   ├─VariableAccessSyntax
//@[099:00107) |   | └─IdentifierSyntax
//@[099:00107) |   |   └─Token(Identifier) |accounts|
//@[107:00108) |   ├─Token(Colon) |:|
//@[109:00291) |   ├─ObjectSyntax
//@[109:00110) |   | ├─Token(LeftBrace) |{|
//@[110:00111) |   | ├─Token(NewLine) |\n|
  name: '${name}-collection-${account.name}-${index}'
//@[002:00053) |   | ├─ObjectPropertySyntax
//@[002:00006) |   | | ├─IdentifierSyntax
//@[002:00006) |   | | | └─Token(Identifier) |name|
//@[006:00007) |   | | ├─Token(Colon) |:|
//@[008:00053) |   | | └─StringSyntax
//@[008:00011) |   | |   ├─Token(StringLeftPiece) |'${|
//@[011:00015) |   | |   ├─VariableAccessSyntax
//@[011:00015) |   | |   | └─IdentifierSyntax
//@[011:00015) |   | |   |   └─Token(Identifier) |name|
//@[015:00030) |   | |   ├─Token(StringMiddlePiece) |}-collection-${|
//@[030:00042) |   | |   ├─PropertyAccessSyntax
//@[030:00037) |   | |   | ├─VariableAccessSyntax
//@[030:00037) |   | |   | | └─IdentifierSyntax
//@[030:00037) |   | |   | |   └─Token(Identifier) |account|
//@[037:00038) |   | |   | ├─Token(Dot) |.|
//@[038:00042) |   | |   | └─IdentifierSyntax
//@[038:00042) |   | |   |   └─Token(Identifier) |name|
//@[042:00046) |   | |   ├─Token(StringMiddlePiece) |}-${|
//@[046:00051) |   | |   ├─VariableAccessSyntax
//@[046:00051) |   | |   | └─IdentifierSyntax
//@[046:00051) |   | |   |   └─Token(Identifier) |index|
//@[051:00053) |   | |   └─Token(StringRightPiece) |}'|
//@[053:00054) |   | ├─Token(NewLine) |\n|
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
//@[028:00029) |   | ├─Token(NewLine) |\n|
  kind: 'StorageV2'
//@[002:00019) |   | ├─ObjectPropertySyntax
//@[002:00006) |   | | ├─IdentifierSyntax
//@[002:00006) |   | | | └─Token(Identifier) |kind|
//@[006:00007) |   | | ├─Token(Colon) |:|
//@[008:00019) |   | | └─StringSyntax
//@[008:00019) |   | |   └─Token(StringComplete) |'StorageV2'|
//@[019:00020) |   | ├─Token(NewLine) |\n|
  sku: {
//@[002:00037) |   | ├─ObjectPropertySyntax
//@[002:00005) |   | | ├─IdentifierSyntax
//@[002:00005) |   | | | └─Token(Identifier) |sku|
//@[005:00006) |   | | ├─Token(Colon) |:|
//@[007:00037) |   | | └─ObjectSyntax
//@[007:00008) |   | |   ├─Token(LeftBrace) |{|
//@[008:00009) |   | |   ├─Token(NewLine) |\n|
    name: 'Standard_LRS'
//@[004:00024) |   | |   ├─ObjectPropertySyntax
//@[004:00008) |   | |   | ├─IdentifierSyntax
//@[004:00008) |   | |   | | └─Token(Identifier) |name|
//@[008:00009) |   | |   | ├─Token(Colon) |:|
//@[010:00024) |   | |   | └─StringSyntax
//@[010:00024) |   | |   |   └─Token(StringComplete) |'Standard_LRS'|
//@[024:00025) |   | |   ├─Token(NewLine) |\n|
  }
//@[002:00003) |   | |   └─Token(RightBrace) |}|
//@[003:00004) |   | ├─Token(NewLine) |\n|
  dependsOn: [
//@[002:00037) |   | ├─ObjectPropertySyntax
//@[002:00011) |   | | ├─IdentifierSyntax
//@[002:00011) |   | | | └─Token(Identifier) |dependsOn|
//@[011:00012) |   | | ├─Token(Colon) |:|
//@[013:00037) |   | | └─ArraySyntax
//@[013:00014) |   | |   ├─Token(LeftSquare) |[|
//@[014:00015) |   | |   ├─Token(NewLine) |\n|
    singleResource
//@[004:00018) |   | |   ├─ArrayItemSyntax
//@[004:00018) |   | |   | └─VariableAccessSyntax
//@[004:00018) |   | |   |   └─IdentifierSyntax
//@[004:00018) |   | |   |     └─Token(Identifier) |singleResource|
//@[018:00019) |   | |   ├─Token(NewLine) |\n|
  ]
//@[002:00003) |   | |   └─Token(RightSquare) |]|
//@[003:00004) |   | ├─Token(NewLine) |\n|
}]
//@[000:00001) |   | └─Token(RightBrace) |}|
//@[001:00002) |   └─Token(RightSquare) |]|
//@[002:00004) ├─Token(NewLine) |\n\n|

// extension of a single resource in a collection
//@[049:00050) ├─Token(NewLine) |\n|
resource extendSingleResourceInCollection 'Microsoft.Authorization/locks@2016-09-01' = {
//@[000:00212) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00041) | ├─IdentifierSyntax
//@[009:00041) | | └─Token(Identifier) |extendSingleResourceInCollection|
//@[042:00084) | ├─StringSyntax
//@[042:00084) | | └─Token(StringComplete) |'Microsoft.Authorization/locks@2016-09-01'|
//@[085:00086) | ├─Token(Assignment) |=|
//@[087:00212) | └─ObjectSyntax
//@[087:00088) |   ├─Token(LeftBrace) |{|
//@[088:00089) |   ├─Token(NewLine) |\n|
  name: 'one-resource-collection-item-lock'
//@[002:00043) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00043) |   | └─StringSyntax
//@[008:00043) |   |   └─Token(StringComplete) |'one-resource-collection-item-lock'|
//@[043:00044) |   ├─Token(NewLine) |\n|
  properties: {
//@[002:00041) |   ├─ObjectPropertySyntax
//@[002:00012) |   | ├─IdentifierSyntax
//@[002:00012) |   | | └─Token(Identifier) |properties|
//@[012:00013) |   | ├─Token(Colon) |:|
//@[014:00041) |   | └─ObjectSyntax
//@[014:00015) |   |   ├─Token(LeftBrace) |{|
//@[015:00016) |   |   ├─Token(NewLine) |\n|
    level: 'ReadOnly'
//@[004:00021) |   |   ├─ObjectPropertySyntax
//@[004:00009) |   |   | ├─IdentifierSyntax
//@[004:00009) |   |   | | └─Token(Identifier) |level|
//@[009:00010) |   |   | ├─Token(Colon) |:|
//@[011:00021) |   |   | └─StringSyntax
//@[011:00021) |   |   |   └─Token(StringComplete) |'ReadOnly'|
//@[021:00022) |   |   ├─Token(NewLine) |\n|
  }
//@[002:00003) |   |   └─Token(RightBrace) |}|
//@[003:00004) |   ├─Token(NewLine) |\n|
  scope: storageAccounts[index % 2]
//@[002:00035) |   ├─ObjectPropertySyntax
//@[002:00007) |   | ├─IdentifierSyntax
//@[002:00007) |   | | └─Token(Identifier) |scope|
//@[007:00008) |   | ├─Token(Colon) |:|
//@[009:00035) |   | └─ArrayAccessSyntax
//@[009:00024) |   |   ├─VariableAccessSyntax
//@[009:00024) |   |   | └─IdentifierSyntax
//@[009:00024) |   |   |   └─Token(Identifier) |storageAccounts|
//@[024:00025) |   |   ├─Token(LeftSquare) |[|
//@[025:00034) |   |   ├─BinaryOperationSyntax
//@[025:00030) |   |   | ├─VariableAccessSyntax
//@[025:00030) |   |   | | └─IdentifierSyntax
//@[025:00030) |   |   | |   └─Token(Identifier) |index|
//@[031:00032) |   |   | ├─Token(Modulo) |%|
//@[033:00034) |   |   | └─IntegerLiteralSyntax
//@[033:00034) |   |   |   └─Token(Integer) |2|
//@[034:00035) |   |   └─Token(RightSquare) |]|
//@[035:00036) |   ├─Token(NewLine) |\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00003) ├─Token(NewLine) |\n\n|

// collection of extensions
//@[027:00028) ├─Token(NewLine) |\n|
resource extensionCollection 'Microsoft.Authorization/locks@2016-09-01' = [for (i, i2) in range(0,1): {
//@[000:00235) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00028) | ├─IdentifierSyntax
//@[009:00028) | | └─Token(Identifier) |extensionCollection|
//@[029:00071) | ├─StringSyntax
//@[029:00071) | | └─Token(StringComplete) |'Microsoft.Authorization/locks@2016-09-01'|
//@[072:00073) | ├─Token(Assignment) |=|
//@[074:00235) | └─ForSyntax
//@[074:00075) |   ├─Token(LeftSquare) |[|
//@[075:00078) |   ├─Token(Identifier) |for|
//@[079:00086) |   ├─VariableBlockSyntax
//@[079:00080) |   | ├─Token(LeftParen) |(|
//@[080:00081) |   | ├─LocalVariableSyntax
//@[080:00081) |   | | └─IdentifierSyntax
//@[080:00081) |   | |   └─Token(Identifier) |i|
//@[081:00082) |   | ├─Token(Comma) |,|
//@[083:00085) |   | ├─LocalVariableSyntax
//@[083:00085) |   | | └─IdentifierSyntax
//@[083:00085) |   | |   └─Token(Identifier) |i2|
//@[085:00086) |   | └─Token(RightParen) |)|
//@[087:00089) |   ├─Token(Identifier) |in|
//@[090:00100) |   ├─FunctionCallSyntax
//@[090:00095) |   | ├─IdentifierSyntax
//@[090:00095) |   | | └─Token(Identifier) |range|
//@[095:00096) |   | ├─Token(LeftParen) |(|
//@[096:00097) |   | ├─FunctionArgumentSyntax
//@[096:00097) |   | | └─IntegerLiteralSyntax
//@[096:00097) |   | |   └─Token(Integer) |0|
//@[097:00098) |   | ├─Token(Comma) |,|
//@[098:00099) |   | ├─FunctionArgumentSyntax
//@[098:00099) |   | | └─IntegerLiteralSyntax
//@[098:00099) |   | |   └─Token(Integer) |1|
//@[099:00100) |   | └─Token(RightParen) |)|
//@[100:00101) |   ├─Token(Colon) |:|
//@[102:00234) |   ├─ObjectSyntax
//@[102:00103) |   | ├─Token(LeftBrace) |{|
//@[103:00104) |   | ├─Token(NewLine) |\n|
  name: 'lock-${i}-${i2}'
//@[002:00025) |   | ├─ObjectPropertySyntax
//@[002:00006) |   | | ├─IdentifierSyntax
//@[002:00006) |   | | | └─Token(Identifier) |name|
//@[006:00007) |   | | ├─Token(Colon) |:|
//@[008:00025) |   | | └─StringSyntax
//@[008:00016) |   | |   ├─Token(StringLeftPiece) |'lock-${|
//@[016:00017) |   | |   ├─VariableAccessSyntax
//@[016:00017) |   | |   | └─IdentifierSyntax
//@[016:00017) |   | |   |   └─Token(Identifier) |i|
//@[017:00021) |   | |   ├─Token(StringMiddlePiece) |}-${|
//@[021:00023) |   | |   ├─VariableAccessSyntax
//@[021:00023) |   | |   | └─IdentifierSyntax
//@[021:00023) |   | |   |   └─Token(Identifier) |i2|
//@[023:00025) |   | |   └─Token(StringRightPiece) |}'|
//@[025:00026) |   | ├─Token(NewLine) |\n|
  properties: {
//@[002:00078) |   | ├─ObjectPropertySyntax
//@[002:00012) |   | | ├─IdentifierSyntax
//@[002:00012) |   | | | └─Token(Identifier) |properties|
//@[012:00013) |   | | ├─Token(Colon) |:|
//@[014:00078) |   | | └─ObjectSyntax
//@[014:00015) |   | |   ├─Token(LeftBrace) |{|
//@[015:00016) |   | |   ├─Token(NewLine) |\n|
    level: i == 0 && i2 == 0 ? 'CanNotDelete' : 'ReadOnly'
//@[004:00058) |   | |   ├─ObjectPropertySyntax
//@[004:00009) |   | |   | ├─IdentifierSyntax
//@[004:00009) |   | |   | | └─Token(Identifier) |level|
//@[009:00010) |   | |   | ├─Token(Colon) |:|
//@[011:00058) |   | |   | └─TernaryOperationSyntax
//@[011:00028) |   | |   |   ├─BinaryOperationSyntax
//@[011:00017) |   | |   |   | ├─BinaryOperationSyntax
//@[011:00012) |   | |   |   | | ├─VariableAccessSyntax
//@[011:00012) |   | |   |   | | | └─IdentifierSyntax
//@[011:00012) |   | |   |   | | |   └─Token(Identifier) |i|
//@[013:00015) |   | |   |   | | ├─Token(Equals) |==|
//@[016:00017) |   | |   |   | | └─IntegerLiteralSyntax
//@[016:00017) |   | |   |   | |   └─Token(Integer) |0|
//@[018:00020) |   | |   |   | ├─Token(LogicalAnd) |&&|
//@[021:00028) |   | |   |   | └─BinaryOperationSyntax
//@[021:00023) |   | |   |   |   ├─VariableAccessSyntax
//@[021:00023) |   | |   |   |   | └─IdentifierSyntax
//@[021:00023) |   | |   |   |   |   └─Token(Identifier) |i2|
//@[024:00026) |   | |   |   |   ├─Token(Equals) |==|
//@[027:00028) |   | |   |   |   └─IntegerLiteralSyntax
//@[027:00028) |   | |   |   |     └─Token(Integer) |0|
//@[029:00030) |   | |   |   ├─Token(Question) |?|
//@[031:00045) |   | |   |   ├─StringSyntax
//@[031:00045) |   | |   |   | └─Token(StringComplete) |'CanNotDelete'|
//@[046:00047) |   | |   |   ├─Token(Colon) |:|
//@[048:00058) |   | |   |   └─StringSyntax
//@[048:00058) |   | |   |     └─Token(StringComplete) |'ReadOnly'|
//@[058:00059) |   | |   ├─Token(NewLine) |\n|
  }
//@[002:00003) |   | |   └─Token(RightBrace) |}|
//@[003:00004) |   | ├─Token(NewLine) |\n|
  scope: singleResource
//@[002:00023) |   | ├─ObjectPropertySyntax
//@[002:00007) |   | | ├─IdentifierSyntax
//@[002:00007) |   | | | └─Token(Identifier) |scope|
//@[007:00008) |   | | ├─Token(Colon) |:|
//@[009:00023) |   | | └─VariableAccessSyntax
//@[009:00023) |   | |   └─IdentifierSyntax
//@[009:00023) |   | |     └─Token(Identifier) |singleResource|
//@[023:00024) |   | ├─Token(NewLine) |\n|
}]
//@[000:00001) |   | └─Token(RightBrace) |}|
//@[001:00002) |   └─Token(RightSquare) |]|
//@[002:00004) ├─Token(NewLine) |\n\n|

// cascade extend the extension
//@[031:00032) ├─Token(NewLine) |\n|
@batchSize(1)
//@[000:00260) ├─ResourceDeclarationSyntax
//@[000:00013) | ├─DecoratorSyntax
//@[000:00001) | | ├─Token(At) |@|
//@[001:00013) | | └─FunctionCallSyntax
//@[001:00010) | |   ├─IdentifierSyntax
//@[001:00010) | |   | └─Token(Identifier) |batchSize|
//@[010:00011) | |   ├─Token(LeftParen) |(|
//@[011:00012) | |   ├─FunctionArgumentSyntax
//@[011:00012) | |   | └─IntegerLiteralSyntax
//@[011:00012) | |   |   └─Token(Integer) |1|
//@[012:00013) | |   └─Token(RightParen) |)|
//@[013:00014) | ├─Token(NewLine) |\n|
resource lockTheLocks 'Microsoft.Authorization/locks@2016-09-01' = [for (i, i2) in range(0,1): {
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00021) | ├─IdentifierSyntax
//@[009:00021) | | └─Token(Identifier) |lockTheLocks|
//@[022:00064) | ├─StringSyntax
//@[022:00064) | | └─Token(StringComplete) |'Microsoft.Authorization/locks@2016-09-01'|
//@[065:00066) | ├─Token(Assignment) |=|
//@[067:00246) | └─ForSyntax
//@[067:00068) |   ├─Token(LeftSquare) |[|
//@[068:00071) |   ├─Token(Identifier) |for|
//@[072:00079) |   ├─VariableBlockSyntax
//@[072:00073) |   | ├─Token(LeftParen) |(|
//@[073:00074) |   | ├─LocalVariableSyntax
//@[073:00074) |   | | └─IdentifierSyntax
//@[073:00074) |   | |   └─Token(Identifier) |i|
//@[074:00075) |   | ├─Token(Comma) |,|
//@[076:00078) |   | ├─LocalVariableSyntax
//@[076:00078) |   | | └─IdentifierSyntax
//@[076:00078) |   | |   └─Token(Identifier) |i2|
//@[078:00079) |   | └─Token(RightParen) |)|
//@[080:00082) |   ├─Token(Identifier) |in|
//@[083:00093) |   ├─FunctionCallSyntax
//@[083:00088) |   | ├─IdentifierSyntax
//@[083:00088) |   | | └─Token(Identifier) |range|
//@[088:00089) |   | ├─Token(LeftParen) |(|
//@[089:00090) |   | ├─FunctionArgumentSyntax
//@[089:00090) |   | | └─IntegerLiteralSyntax
//@[089:00090) |   | |   └─Token(Integer) |0|
//@[090:00091) |   | ├─Token(Comma) |,|
//@[091:00092) |   | ├─FunctionArgumentSyntax
//@[091:00092) |   | | └─IntegerLiteralSyntax
//@[091:00092) |   | |   └─Token(Integer) |1|
//@[092:00093) |   | └─Token(RightParen) |)|
//@[093:00094) |   ├─Token(Colon) |:|
//@[095:00245) |   ├─ObjectSyntax
//@[095:00096) |   | ├─Token(LeftBrace) |{|
//@[096:00097) |   | ├─Token(NewLine) |\n|
  name: 'lock-the-lock-${i}-${i2}'
//@[002:00034) |   | ├─ObjectPropertySyntax
//@[002:00006) |   | | ├─IdentifierSyntax
//@[002:00006) |   | | | └─Token(Identifier) |name|
//@[006:00007) |   | | ├─Token(Colon) |:|
//@[008:00034) |   | | └─StringSyntax
//@[008:00025) |   | |   ├─Token(StringLeftPiece) |'lock-the-lock-${|
//@[025:00026) |   | |   ├─VariableAccessSyntax
//@[025:00026) |   | |   | └─IdentifierSyntax
//@[025:00026) |   | |   |   └─Token(Identifier) |i|
//@[026:00030) |   | |   ├─Token(StringMiddlePiece) |}-${|
//@[030:00032) |   | |   ├─VariableAccessSyntax
//@[030:00032) |   | |   | └─IdentifierSyntax
//@[030:00032) |   | |   |   └─Token(Identifier) |i2|
//@[032:00034) |   | |   └─Token(StringRightPiece) |}'|
//@[034:00035) |   | ├─Token(NewLine) |\n|
  properties: {
//@[002:00078) |   | ├─ObjectPropertySyntax
//@[002:00012) |   | | ├─IdentifierSyntax
//@[002:00012) |   | | | └─Token(Identifier) |properties|
//@[012:00013) |   | | ├─Token(Colon) |:|
//@[014:00078) |   | | └─ObjectSyntax
//@[014:00015) |   | |   ├─Token(LeftBrace) |{|
//@[015:00016) |   | |   ├─Token(NewLine) |\n|
    level: i == 0 && i2 == 0 ? 'CanNotDelete' : 'ReadOnly'
//@[004:00058) |   | |   ├─ObjectPropertySyntax
//@[004:00009) |   | |   | ├─IdentifierSyntax
//@[004:00009) |   | |   | | └─Token(Identifier) |level|
//@[009:00010) |   | |   | ├─Token(Colon) |:|
//@[011:00058) |   | |   | └─TernaryOperationSyntax
//@[011:00028) |   | |   |   ├─BinaryOperationSyntax
//@[011:00017) |   | |   |   | ├─BinaryOperationSyntax
//@[011:00012) |   | |   |   | | ├─VariableAccessSyntax
//@[011:00012) |   | |   |   | | | └─IdentifierSyntax
//@[011:00012) |   | |   |   | | |   └─Token(Identifier) |i|
//@[013:00015) |   | |   |   | | ├─Token(Equals) |==|
//@[016:00017) |   | |   |   | | └─IntegerLiteralSyntax
//@[016:00017) |   | |   |   | |   └─Token(Integer) |0|
//@[018:00020) |   | |   |   | ├─Token(LogicalAnd) |&&|
//@[021:00028) |   | |   |   | └─BinaryOperationSyntax
//@[021:00023) |   | |   |   |   ├─VariableAccessSyntax
//@[021:00023) |   | |   |   |   | └─IdentifierSyntax
//@[021:00023) |   | |   |   |   |   └─Token(Identifier) |i2|
//@[024:00026) |   | |   |   |   ├─Token(Equals) |==|
//@[027:00028) |   | |   |   |   └─IntegerLiteralSyntax
//@[027:00028) |   | |   |   |     └─Token(Integer) |0|
//@[029:00030) |   | |   |   ├─Token(Question) |?|
//@[031:00045) |   | |   |   ├─StringSyntax
//@[031:00045) |   | |   |   | └─Token(StringComplete) |'CanNotDelete'|
//@[046:00047) |   | |   |   ├─Token(Colon) |:|
//@[048:00058) |   | |   |   └─StringSyntax
//@[048:00058) |   | |   |     └─Token(StringComplete) |'ReadOnly'|
//@[058:00059) |   | |   ├─Token(NewLine) |\n|
  }
//@[002:00003) |   | |   └─Token(RightBrace) |}|
//@[003:00004) |   | ├─Token(NewLine) |\n|
  scope: extensionCollection[i2]
//@[002:00032) |   | ├─ObjectPropertySyntax
//@[002:00007) |   | | ├─IdentifierSyntax
//@[002:00007) |   | | | └─Token(Identifier) |scope|
//@[007:00008) |   | | ├─Token(Colon) |:|
//@[009:00032) |   | | └─ArrayAccessSyntax
//@[009:00028) |   | |   ├─VariableAccessSyntax
//@[009:00028) |   | |   | └─IdentifierSyntax
//@[009:00028) |   | |   |   └─Token(Identifier) |extensionCollection|
//@[028:00029) |   | |   ├─Token(LeftSquare) |[|
//@[029:00031) |   | |   ├─VariableAccessSyntax
//@[029:00031) |   | |   | └─IdentifierSyntax
//@[029:00031) |   | |   |   └─Token(Identifier) |i2|
//@[031:00032) |   | |   └─Token(RightSquare) |]|
//@[032:00033) |   | ├─Token(NewLine) |\n|
}]
//@[000:00001) |   | └─Token(RightBrace) |}|
//@[001:00002) |   └─Token(RightSquare) |]|
//@[002:00004) ├─Token(NewLine) |\n\n|

// special case property access
//@[031:00032) ├─Token(NewLine) |\n|
output indexedCollectionBlobEndpoint string = storageAccounts[index].properties.primaryEndpoints.blob
//@[000:00101) ├─OutputDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |output|
//@[007:00036) | ├─IdentifierSyntax
//@[007:00036) | | └─Token(Identifier) |indexedCollectionBlobEndpoint|
//@[037:00043) | ├─SimpleTypeSyntax
//@[037:00043) | | └─Token(Identifier) |string|
//@[044:00045) | ├─Token(Assignment) |=|
//@[046:00101) | └─PropertyAccessSyntax
//@[046:00096) |   ├─PropertyAccessSyntax
//@[046:00079) |   | ├─PropertyAccessSyntax
//@[046:00068) |   | | ├─ArrayAccessSyntax
//@[046:00061) |   | | | ├─VariableAccessSyntax
//@[046:00061) |   | | | | └─IdentifierSyntax
//@[046:00061) |   | | | |   └─Token(Identifier) |storageAccounts|
//@[061:00062) |   | | | ├─Token(LeftSquare) |[|
//@[062:00067) |   | | | ├─VariableAccessSyntax
//@[062:00067) |   | | | | └─IdentifierSyntax
//@[062:00067) |   | | | |   └─Token(Identifier) |index|
//@[067:00068) |   | | | └─Token(RightSquare) |]|
//@[068:00069) |   | | ├─Token(Dot) |.|
//@[069:00079) |   | | └─IdentifierSyntax
//@[069:00079) |   | |   └─Token(Identifier) |properties|
//@[079:00080) |   | ├─Token(Dot) |.|
//@[080:00096) |   | └─IdentifierSyntax
//@[080:00096) |   |   └─Token(Identifier) |primaryEndpoints|
//@[096:00097) |   ├─Token(Dot) |.|
//@[097:00101) |   └─IdentifierSyntax
//@[097:00101) |     └─Token(Identifier) |blob|
//@[101:00102) ├─Token(NewLine) |\n|
output indexedCollectionName string = storageAccounts[index].name
//@[000:00065) ├─OutputDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |output|
//@[007:00028) | ├─IdentifierSyntax
//@[007:00028) | | └─Token(Identifier) |indexedCollectionName|
//@[029:00035) | ├─SimpleTypeSyntax
//@[029:00035) | | └─Token(Identifier) |string|
//@[036:00037) | ├─Token(Assignment) |=|
//@[038:00065) | └─PropertyAccessSyntax
//@[038:00060) |   ├─ArrayAccessSyntax
//@[038:00053) |   | ├─VariableAccessSyntax
//@[038:00053) |   | | └─IdentifierSyntax
//@[038:00053) |   | |   └─Token(Identifier) |storageAccounts|
//@[053:00054) |   | ├─Token(LeftSquare) |[|
//@[054:00059) |   | ├─VariableAccessSyntax
//@[054:00059) |   | | └─IdentifierSyntax
//@[054:00059) |   | |   └─Token(Identifier) |index|
//@[059:00060) |   | └─Token(RightSquare) |]|
//@[060:00061) |   ├─Token(Dot) |.|
//@[061:00065) |   └─IdentifierSyntax
//@[061:00065) |     └─Token(Identifier) |name|
//@[065:00066) ├─Token(NewLine) |\n|
output indexedCollectionId string = storageAccounts[index].id
//@[000:00061) ├─OutputDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |output|
//@[007:00026) | ├─IdentifierSyntax
//@[007:00026) | | └─Token(Identifier) |indexedCollectionId|
//@[027:00033) | ├─SimpleTypeSyntax
//@[027:00033) | | └─Token(Identifier) |string|
//@[034:00035) | ├─Token(Assignment) |=|
//@[036:00061) | └─PropertyAccessSyntax
//@[036:00058) |   ├─ArrayAccessSyntax
//@[036:00051) |   | ├─VariableAccessSyntax
//@[036:00051) |   | | └─IdentifierSyntax
//@[036:00051) |   | |   └─Token(Identifier) |storageAccounts|
//@[051:00052) |   | ├─Token(LeftSquare) |[|
//@[052:00057) |   | ├─VariableAccessSyntax
//@[052:00057) |   | | └─IdentifierSyntax
//@[052:00057) |   | |   └─Token(Identifier) |index|
//@[057:00058) |   | └─Token(RightSquare) |]|
//@[058:00059) |   ├─Token(Dot) |.|
//@[059:00061) |   └─IdentifierSyntax
//@[059:00061) |     └─Token(Identifier) |id|
//@[061:00062) ├─Token(NewLine) |\n|
output indexedCollectionType string = storageAccounts[index].type
//@[000:00065) ├─OutputDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |output|
//@[007:00028) | ├─IdentifierSyntax
//@[007:00028) | | └─Token(Identifier) |indexedCollectionType|
//@[029:00035) | ├─SimpleTypeSyntax
//@[029:00035) | | └─Token(Identifier) |string|
//@[036:00037) | ├─Token(Assignment) |=|
//@[038:00065) | └─PropertyAccessSyntax
//@[038:00060) |   ├─ArrayAccessSyntax
//@[038:00053) |   | ├─VariableAccessSyntax
//@[038:00053) |   | | └─IdentifierSyntax
//@[038:00053) |   | |   └─Token(Identifier) |storageAccounts|
//@[053:00054) |   | ├─Token(LeftSquare) |[|
//@[054:00059) |   | ├─VariableAccessSyntax
//@[054:00059) |   | | └─IdentifierSyntax
//@[054:00059) |   | |   └─Token(Identifier) |index|
//@[059:00060) |   | └─Token(RightSquare) |]|
//@[060:00061) |   ├─Token(Dot) |.|
//@[061:00065) |   └─IdentifierSyntax
//@[061:00065) |     └─Token(Identifier) |type|
//@[065:00066) ├─Token(NewLine) |\n|
output indexedCollectionVersion string = storageAccounts[index].apiVersion
//@[000:00074) ├─OutputDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |output|
//@[007:00031) | ├─IdentifierSyntax
//@[007:00031) | | └─Token(Identifier) |indexedCollectionVersion|
//@[032:00038) | ├─SimpleTypeSyntax
//@[032:00038) | | └─Token(Identifier) |string|
//@[039:00040) | ├─Token(Assignment) |=|
//@[041:00074) | └─PropertyAccessSyntax
//@[041:00063) |   ├─ArrayAccessSyntax
//@[041:00056) |   | ├─VariableAccessSyntax
//@[041:00056) |   | | └─IdentifierSyntax
//@[041:00056) |   | |   └─Token(Identifier) |storageAccounts|
//@[056:00057) |   | ├─Token(LeftSquare) |[|
//@[057:00062) |   | ├─VariableAccessSyntax
//@[057:00062) |   | | └─IdentifierSyntax
//@[057:00062) |   | |   └─Token(Identifier) |index|
//@[062:00063) |   | └─Token(RightSquare) |]|
//@[063:00064) |   ├─Token(Dot) |.|
//@[064:00074) |   └─IdentifierSyntax
//@[064:00074) |     └─Token(Identifier) |apiVersion|
//@[074:00076) ├─Token(NewLine) |\n\n|

// general case property access
//@[031:00032) ├─Token(NewLine) |\n|
output indexedCollectionIdentity object = storageAccounts[index].identity
//@[000:00073) ├─OutputDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |output|
//@[007:00032) | ├─IdentifierSyntax
//@[007:00032) | | └─Token(Identifier) |indexedCollectionIdentity|
//@[033:00039) | ├─SimpleTypeSyntax
//@[033:00039) | | └─Token(Identifier) |object|
//@[040:00041) | ├─Token(Assignment) |=|
//@[042:00073) | └─PropertyAccessSyntax
//@[042:00064) |   ├─ArrayAccessSyntax
//@[042:00057) |   | ├─VariableAccessSyntax
//@[042:00057) |   | | └─IdentifierSyntax
//@[042:00057) |   | |   └─Token(Identifier) |storageAccounts|
//@[057:00058) |   | ├─Token(LeftSquare) |[|
//@[058:00063) |   | ├─VariableAccessSyntax
//@[058:00063) |   | | └─IdentifierSyntax
//@[058:00063) |   | |   └─Token(Identifier) |index|
//@[063:00064) |   | └─Token(RightSquare) |]|
//@[064:00065) |   ├─Token(Dot) |.|
//@[065:00073) |   └─IdentifierSyntax
//@[065:00073) |     └─Token(Identifier) |identity|
//@[073:00075) ├─Token(NewLine) |\n\n|

// indexed access of two properties
//@[035:00036) ├─Token(NewLine) |\n|
output indexedEndpointPair object = {
//@[000:00181) ├─OutputDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |output|
//@[007:00026) | ├─IdentifierSyntax
//@[007:00026) | | └─Token(Identifier) |indexedEndpointPair|
//@[027:00033) | ├─SimpleTypeSyntax
//@[027:00033) | | └─Token(Identifier) |object|
//@[034:00035) | ├─Token(Assignment) |=|
//@[036:00181) | └─ObjectSyntax
//@[036:00037) |   ├─Token(LeftBrace) |{|
//@[037:00038) |   ├─Token(NewLine) |\n|
  primary: storageAccounts[index].properties.primaryEndpoints.blob
//@[002:00066) |   ├─ObjectPropertySyntax
//@[002:00009) |   | ├─IdentifierSyntax
//@[002:00009) |   | | └─Token(Identifier) |primary|
//@[009:00010) |   | ├─Token(Colon) |:|
//@[011:00066) |   | └─PropertyAccessSyntax
//@[011:00061) |   |   ├─PropertyAccessSyntax
//@[011:00044) |   |   | ├─PropertyAccessSyntax
//@[011:00033) |   |   | | ├─ArrayAccessSyntax
//@[011:00026) |   |   | | | ├─VariableAccessSyntax
//@[011:00026) |   |   | | | | └─IdentifierSyntax
//@[011:00026) |   |   | | | |   └─Token(Identifier) |storageAccounts|
//@[026:00027) |   |   | | | ├─Token(LeftSquare) |[|
//@[027:00032) |   |   | | | ├─VariableAccessSyntax
//@[027:00032) |   |   | | | | └─IdentifierSyntax
//@[027:00032) |   |   | | | |   └─Token(Identifier) |index|
//@[032:00033) |   |   | | | └─Token(RightSquare) |]|
//@[033:00034) |   |   | | ├─Token(Dot) |.|
//@[034:00044) |   |   | | └─IdentifierSyntax
//@[034:00044) |   |   | |   └─Token(Identifier) |properties|
//@[044:00045) |   |   | ├─Token(Dot) |.|
//@[045:00061) |   |   | └─IdentifierSyntax
//@[045:00061) |   |   |   └─Token(Identifier) |primaryEndpoints|
//@[061:00062) |   |   ├─Token(Dot) |.|
//@[062:00066) |   |   └─IdentifierSyntax
//@[062:00066) |   |     └─Token(Identifier) |blob|
//@[066:00067) |   ├─Token(NewLine) |\n|
  secondary: storageAccounts[index + 1].properties.secondaryEndpoints.blob
//@[002:00074) |   ├─ObjectPropertySyntax
//@[002:00011) |   | ├─IdentifierSyntax
//@[002:00011) |   | | └─Token(Identifier) |secondary|
//@[011:00012) |   | ├─Token(Colon) |:|
//@[013:00074) |   | └─PropertyAccessSyntax
//@[013:00069) |   |   ├─PropertyAccessSyntax
//@[013:00050) |   |   | ├─PropertyAccessSyntax
//@[013:00039) |   |   | | ├─ArrayAccessSyntax
//@[013:00028) |   |   | | | ├─VariableAccessSyntax
//@[013:00028) |   |   | | | | └─IdentifierSyntax
//@[013:00028) |   |   | | | |   └─Token(Identifier) |storageAccounts|
//@[028:00029) |   |   | | | ├─Token(LeftSquare) |[|
//@[029:00038) |   |   | | | ├─BinaryOperationSyntax
//@[029:00034) |   |   | | | | ├─VariableAccessSyntax
//@[029:00034) |   |   | | | | | └─IdentifierSyntax
//@[029:00034) |   |   | | | | |   └─Token(Identifier) |index|
//@[035:00036) |   |   | | | | ├─Token(Plus) |+|
//@[037:00038) |   |   | | | | └─IntegerLiteralSyntax
//@[037:00038) |   |   | | | |   └─Token(Integer) |1|
//@[038:00039) |   |   | | | └─Token(RightSquare) |]|
//@[039:00040) |   |   | | ├─Token(Dot) |.|
//@[040:00050) |   |   | | └─IdentifierSyntax
//@[040:00050) |   |   | |   └─Token(Identifier) |properties|
//@[050:00051) |   |   | ├─Token(Dot) |.|
//@[051:00069) |   |   | └─IdentifierSyntax
//@[051:00069) |   |   |   └─Token(Identifier) |secondaryEndpoints|
//@[069:00070) |   |   ├─Token(Dot) |.|
//@[070:00074) |   |   └─IdentifierSyntax
//@[070:00074) |   |     └─Token(Identifier) |blob|
//@[074:00075) |   ├─Token(NewLine) |\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00003) ├─Token(NewLine) |\n\n|

// nested indexer?
//@[018:00019) ├─Token(NewLine) |\n|
output indexViaReference string = storageAccounts[int(storageAccounts[index].properties.creationTime)].properties.accessTier
//@[000:00124) ├─OutputDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |output|
//@[007:00024) | ├─IdentifierSyntax
//@[007:00024) | | └─Token(Identifier) |indexViaReference|
//@[025:00031) | ├─SimpleTypeSyntax
//@[025:00031) | | └─Token(Identifier) |string|
//@[032:00033) | ├─Token(Assignment) |=|
//@[034:00124) | └─PropertyAccessSyntax
//@[034:00113) |   ├─PropertyAccessSyntax
//@[034:00102) |   | ├─ArrayAccessSyntax
//@[034:00049) |   | | ├─VariableAccessSyntax
//@[034:00049) |   | | | └─IdentifierSyntax
//@[034:00049) |   | | |   └─Token(Identifier) |storageAccounts|
//@[049:00050) |   | | ├─Token(LeftSquare) |[|
//@[050:00101) |   | | ├─FunctionCallSyntax
//@[050:00053) |   | | | ├─IdentifierSyntax
//@[050:00053) |   | | | | └─Token(Identifier) |int|
//@[053:00054) |   | | | ├─Token(LeftParen) |(|
//@[054:00100) |   | | | ├─FunctionArgumentSyntax
//@[054:00100) |   | | | | └─PropertyAccessSyntax
//@[054:00087) |   | | | |   ├─PropertyAccessSyntax
//@[054:00076) |   | | | |   | ├─ArrayAccessSyntax
//@[054:00069) |   | | | |   | | ├─VariableAccessSyntax
//@[054:00069) |   | | | |   | | | └─IdentifierSyntax
//@[054:00069) |   | | | |   | | |   └─Token(Identifier) |storageAccounts|
//@[069:00070) |   | | | |   | | ├─Token(LeftSquare) |[|
//@[070:00075) |   | | | |   | | ├─VariableAccessSyntax
//@[070:00075) |   | | | |   | | | └─IdentifierSyntax
//@[070:00075) |   | | | |   | | |   └─Token(Identifier) |index|
//@[075:00076) |   | | | |   | | └─Token(RightSquare) |]|
//@[076:00077) |   | | | |   | ├─Token(Dot) |.|
//@[077:00087) |   | | | |   | └─IdentifierSyntax
//@[077:00087) |   | | | |   |   └─Token(Identifier) |properties|
//@[087:00088) |   | | | |   ├─Token(Dot) |.|
//@[088:00100) |   | | | |   └─IdentifierSyntax
//@[088:00100) |   | | | |     └─Token(Identifier) |creationTime|
//@[100:00101) |   | | | └─Token(RightParen) |)|
//@[101:00102) |   | | └─Token(RightSquare) |]|
//@[102:00103) |   | ├─Token(Dot) |.|
//@[103:00113) |   | └─IdentifierSyntax
//@[103:00113) |   |   └─Token(Identifier) |properties|
//@[113:00114) |   ├─Token(Dot) |.|
//@[114:00124) |   └─IdentifierSyntax
//@[114:00124) |     └─Token(Identifier) |accessTier|
//@[124:00126) ├─Token(NewLine) |\n\n|

// dependency on a resource collection
//@[038:00039) ├─Token(NewLine) |\n|
resource storageAccounts2 'Microsoft.Storage/storageAccounts@2019-06-01' = [for (account, idx) in accounts: {
//@[000:00290) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00025) | ├─IdentifierSyntax
//@[009:00025) | | └─Token(Identifier) |storageAccounts2|
//@[026:00072) | ├─StringSyntax
//@[026:00072) | | └─Token(StringComplete) |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[073:00074) | ├─Token(Assignment) |=|
//@[075:00290) | └─ForSyntax
//@[075:00076) |   ├─Token(LeftSquare) |[|
//@[076:00079) |   ├─Token(Identifier) |for|
//@[080:00094) |   ├─VariableBlockSyntax
//@[080:00081) |   | ├─Token(LeftParen) |(|
//@[081:00088) |   | ├─LocalVariableSyntax
//@[081:00088) |   | | └─IdentifierSyntax
//@[081:00088) |   | |   └─Token(Identifier) |account|
//@[088:00089) |   | ├─Token(Comma) |,|
//@[090:00093) |   | ├─LocalVariableSyntax
//@[090:00093) |   | | └─IdentifierSyntax
//@[090:00093) |   | |   └─Token(Identifier) |idx|
//@[093:00094) |   | └─Token(RightParen) |)|
//@[095:00097) |   ├─Token(Identifier) |in|
//@[098:00106) |   ├─VariableAccessSyntax
//@[098:00106) |   | └─IdentifierSyntax
//@[098:00106) |   |   └─Token(Identifier) |accounts|
//@[106:00107) |   ├─Token(Colon) |:|
//@[108:00289) |   ├─ObjectSyntax
//@[108:00109) |   | ├─Token(LeftBrace) |{|
//@[109:00110) |   | ├─Token(NewLine) |\n|
  name: '${name}-collection-${account.name}-${idx}'
//@[002:00051) |   | ├─ObjectPropertySyntax
//@[002:00006) |   | | ├─IdentifierSyntax
//@[002:00006) |   | | | └─Token(Identifier) |name|
//@[006:00007) |   | | ├─Token(Colon) |:|
//@[008:00051) |   | | └─StringSyntax
//@[008:00011) |   | |   ├─Token(StringLeftPiece) |'${|
//@[011:00015) |   | |   ├─VariableAccessSyntax
//@[011:00015) |   | |   | └─IdentifierSyntax
//@[011:00015) |   | |   |   └─Token(Identifier) |name|
//@[015:00030) |   | |   ├─Token(StringMiddlePiece) |}-collection-${|
//@[030:00042) |   | |   ├─PropertyAccessSyntax
//@[030:00037) |   | |   | ├─VariableAccessSyntax
//@[030:00037) |   | |   | | └─IdentifierSyntax
//@[030:00037) |   | |   | |   └─Token(Identifier) |account|
//@[037:00038) |   | |   | ├─Token(Dot) |.|
//@[038:00042) |   | |   | └─IdentifierSyntax
//@[038:00042) |   | |   |   └─Token(Identifier) |name|
//@[042:00046) |   | |   ├─Token(StringMiddlePiece) |}-${|
//@[046:00049) |   | |   ├─VariableAccessSyntax
//@[046:00049) |   | |   | └─IdentifierSyntax
//@[046:00049) |   | |   |   └─Token(Identifier) |idx|
//@[049:00051) |   | |   └─Token(StringRightPiece) |}'|
//@[051:00052) |   | ├─Token(NewLine) |\n|
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
//@[028:00029) |   | ├─Token(NewLine) |\n|
  kind: 'StorageV2'
//@[002:00019) |   | ├─ObjectPropertySyntax
//@[002:00006) |   | | ├─IdentifierSyntax
//@[002:00006) |   | | | └─Token(Identifier) |kind|
//@[006:00007) |   | | ├─Token(Colon) |:|
//@[008:00019) |   | | └─StringSyntax
//@[008:00019) |   | |   └─Token(StringComplete) |'StorageV2'|
//@[019:00020) |   | ├─Token(NewLine) |\n|
  sku: {
//@[002:00037) |   | ├─ObjectPropertySyntax
//@[002:00005) |   | | ├─IdentifierSyntax
//@[002:00005) |   | | | └─Token(Identifier) |sku|
//@[005:00006) |   | | ├─Token(Colon) |:|
//@[007:00037) |   | | └─ObjectSyntax
//@[007:00008) |   | |   ├─Token(LeftBrace) |{|
//@[008:00009) |   | |   ├─Token(NewLine) |\n|
    name: 'Standard_LRS'
//@[004:00024) |   | |   ├─ObjectPropertySyntax
//@[004:00008) |   | |   | ├─IdentifierSyntax
//@[004:00008) |   | |   | | └─Token(Identifier) |name|
//@[008:00009) |   | |   | ├─Token(Colon) |:|
//@[010:00024) |   | |   | └─StringSyntax
//@[010:00024) |   | |   |   └─Token(StringComplete) |'Standard_LRS'|
//@[024:00025) |   | |   ├─Token(NewLine) |\n|
  }
//@[002:00003) |   | |   └─Token(RightBrace) |}|
//@[003:00004) |   | ├─Token(NewLine) |\n|
  dependsOn: [
//@[002:00038) |   | ├─ObjectPropertySyntax
//@[002:00011) |   | | ├─IdentifierSyntax
//@[002:00011) |   | | | └─Token(Identifier) |dependsOn|
//@[011:00012) |   | | ├─Token(Colon) |:|
//@[013:00038) |   | | └─ArraySyntax
//@[013:00014) |   | |   ├─Token(LeftSquare) |[|
//@[014:00015) |   | |   ├─Token(NewLine) |\n|
    storageAccounts
//@[004:00019) |   | |   ├─ArrayItemSyntax
//@[004:00019) |   | |   | └─VariableAccessSyntax
//@[004:00019) |   | |   |   └─IdentifierSyntax
//@[004:00019) |   | |   |     └─Token(Identifier) |storageAccounts|
//@[019:00020) |   | |   ├─Token(NewLine) |\n|
  ]
//@[002:00003) |   | |   └─Token(RightSquare) |]|
//@[003:00004) |   | ├─Token(NewLine) |\n|
}]
//@[000:00001) |   | └─Token(RightBrace) |}|
//@[001:00002) |   └─Token(RightSquare) |]|
//@[002:00004) ├─Token(NewLine) |\n\n|

// one-to-one paired dependencies
//@[033:00034) ├─Token(NewLine) |\n|
resource firstSet 'Microsoft.Storage/storageAccounts@2019-06-01' = [for (i,ii) in range(0, length(accounts)): {
//@[000:00243) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00017) | ├─IdentifierSyntax
//@[009:00017) | | └─Token(Identifier) |firstSet|
//@[018:00064) | ├─StringSyntax
//@[018:00064) | | └─Token(StringComplete) |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[065:00066) | ├─Token(Assignment) |=|
//@[067:00243) | └─ForSyntax
//@[067:00068) |   ├─Token(LeftSquare) |[|
//@[068:00071) |   ├─Token(Identifier) |for|
//@[072:00078) |   ├─VariableBlockSyntax
//@[072:00073) |   | ├─Token(LeftParen) |(|
//@[073:00074) |   | ├─LocalVariableSyntax
//@[073:00074) |   | | └─IdentifierSyntax
//@[073:00074) |   | |   └─Token(Identifier) |i|
//@[074:00075) |   | ├─Token(Comma) |,|
//@[075:00077) |   | ├─LocalVariableSyntax
//@[075:00077) |   | | └─IdentifierSyntax
//@[075:00077) |   | |   └─Token(Identifier) |ii|
//@[077:00078) |   | └─Token(RightParen) |)|
//@[079:00081) |   ├─Token(Identifier) |in|
//@[082:00108) |   ├─FunctionCallSyntax
//@[082:00087) |   | ├─IdentifierSyntax
//@[082:00087) |   | | └─Token(Identifier) |range|
//@[087:00088) |   | ├─Token(LeftParen) |(|
//@[088:00089) |   | ├─FunctionArgumentSyntax
//@[088:00089) |   | | └─IntegerLiteralSyntax
//@[088:00089) |   | |   └─Token(Integer) |0|
//@[089:00090) |   | ├─Token(Comma) |,|
//@[091:00107) |   | ├─FunctionArgumentSyntax
//@[091:00107) |   | | └─FunctionCallSyntax
//@[091:00097) |   | |   ├─IdentifierSyntax
//@[091:00097) |   | |   | └─Token(Identifier) |length|
//@[097:00098) |   | |   ├─Token(LeftParen) |(|
//@[098:00106) |   | |   ├─FunctionArgumentSyntax
//@[098:00106) |   | |   | └─VariableAccessSyntax
//@[098:00106) |   | |   |   └─IdentifierSyntax
//@[098:00106) |   | |   |     └─Token(Identifier) |accounts|
//@[106:00107) |   | |   └─Token(RightParen) |)|
//@[107:00108) |   | └─Token(RightParen) |)|
//@[108:00109) |   ├─Token(Colon) |:|
//@[110:00242) |   ├─ObjectSyntax
//@[110:00111) |   | ├─Token(LeftBrace) |{|
//@[111:00112) |   | ├─Token(NewLine) |\n|
  name: '${name}-set1-${i}-${ii}'
//@[002:00033) |   | ├─ObjectPropertySyntax
//@[002:00006) |   | | ├─IdentifierSyntax
//@[002:00006) |   | | | └─Token(Identifier) |name|
//@[006:00007) |   | | ├─Token(Colon) |:|
//@[008:00033) |   | | └─StringSyntax
//@[008:00011) |   | |   ├─Token(StringLeftPiece) |'${|
//@[011:00015) |   | |   ├─VariableAccessSyntax
//@[011:00015) |   | |   | └─IdentifierSyntax
//@[011:00015) |   | |   |   └─Token(Identifier) |name|
//@[015:00024) |   | |   ├─Token(StringMiddlePiece) |}-set1-${|
//@[024:00025) |   | |   ├─VariableAccessSyntax
//@[024:00025) |   | |   | └─IdentifierSyntax
//@[024:00025) |   | |   |   └─Token(Identifier) |i|
//@[025:00029) |   | |   ├─Token(StringMiddlePiece) |}-${|
//@[029:00031) |   | |   ├─VariableAccessSyntax
//@[029:00031) |   | |   | └─IdentifierSyntax
//@[029:00031) |   | |   |   └─Token(Identifier) |ii|
//@[031:00033) |   | |   └─Token(StringRightPiece) |}'|
//@[033:00034) |   | ├─Token(NewLine) |\n|
  location: resourceGroup().location
//@[002:00036) |   | ├─ObjectPropertySyntax
//@[002:00010) |   | | ├─IdentifierSyntax
//@[002:00010) |   | | | └─Token(Identifier) |location|
//@[010:00011) |   | | ├─Token(Colon) |:|
//@[012:00036) |   | | └─PropertyAccessSyntax
//@[012:00027) |   | |   ├─FunctionCallSyntax
//@[012:00025) |   | |   | ├─IdentifierSyntax
//@[012:00025) |   | |   | | └─Token(Identifier) |resourceGroup|
//@[025:00026) |   | |   | ├─Token(LeftParen) |(|
//@[026:00027) |   | |   | └─Token(RightParen) |)|
//@[027:00028) |   | |   ├─Token(Dot) |.|
//@[028:00036) |   | |   └─IdentifierSyntax
//@[028:00036) |   | |     └─Token(Identifier) |location|
//@[036:00037) |   | ├─Token(NewLine) |\n|
  kind: 'StorageV2'
//@[002:00019) |   | ├─ObjectPropertySyntax
//@[002:00006) |   | | ├─IdentifierSyntax
//@[002:00006) |   | | | └─Token(Identifier) |kind|
//@[006:00007) |   | | ├─Token(Colon) |:|
//@[008:00019) |   | | └─StringSyntax
//@[008:00019) |   | |   └─Token(StringComplete) |'StorageV2'|
//@[019:00020) |   | ├─Token(NewLine) |\n|
  sku: {
//@[002:00037) |   | ├─ObjectPropertySyntax
//@[002:00005) |   | | ├─IdentifierSyntax
//@[002:00005) |   | | | └─Token(Identifier) |sku|
//@[005:00006) |   | | ├─Token(Colon) |:|
//@[007:00037) |   | | └─ObjectSyntax
//@[007:00008) |   | |   ├─Token(LeftBrace) |{|
//@[008:00009) |   | |   ├─Token(NewLine) |\n|
    name: 'Standard_LRS'
//@[004:00024) |   | |   ├─ObjectPropertySyntax
//@[004:00008) |   | |   | ├─IdentifierSyntax
//@[004:00008) |   | |   | | └─Token(Identifier) |name|
//@[008:00009) |   | |   | ├─Token(Colon) |:|
//@[010:00024) |   | |   | └─StringSyntax
//@[010:00024) |   | |   |   └─Token(StringComplete) |'Standard_LRS'|
//@[024:00025) |   | |   ├─Token(NewLine) |\n|
  }
//@[002:00003) |   | |   └─Token(RightBrace) |}|
//@[003:00004) |   | ├─Token(NewLine) |\n|
}]
//@[000:00001) |   | └─Token(RightBrace) |}|
//@[001:00002) |   └─Token(RightSquare) |]|
//@[002:00004) ├─Token(NewLine) |\n\n|

resource secondSet 'Microsoft.Storage/storageAccounts@2019-06-01' = [for (i,iii) in range(0, length(accounts)): {
//@[000:00283) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00018) | ├─IdentifierSyntax
//@[009:00018) | | └─Token(Identifier) |secondSet|
//@[019:00065) | ├─StringSyntax
//@[019:00065) | | └─Token(StringComplete) |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[066:00067) | ├─Token(Assignment) |=|
//@[068:00283) | └─ForSyntax
//@[068:00069) |   ├─Token(LeftSquare) |[|
//@[069:00072) |   ├─Token(Identifier) |for|
//@[073:00080) |   ├─VariableBlockSyntax
//@[073:00074) |   | ├─Token(LeftParen) |(|
//@[074:00075) |   | ├─LocalVariableSyntax
//@[074:00075) |   | | └─IdentifierSyntax
//@[074:00075) |   | |   └─Token(Identifier) |i|
//@[075:00076) |   | ├─Token(Comma) |,|
//@[076:00079) |   | ├─LocalVariableSyntax
//@[076:00079) |   | | └─IdentifierSyntax
//@[076:00079) |   | |   └─Token(Identifier) |iii|
//@[079:00080) |   | └─Token(RightParen) |)|
//@[081:00083) |   ├─Token(Identifier) |in|
//@[084:00110) |   ├─FunctionCallSyntax
//@[084:00089) |   | ├─IdentifierSyntax
//@[084:00089) |   | | └─Token(Identifier) |range|
//@[089:00090) |   | ├─Token(LeftParen) |(|
//@[090:00091) |   | ├─FunctionArgumentSyntax
//@[090:00091) |   | | └─IntegerLiteralSyntax
//@[090:00091) |   | |   └─Token(Integer) |0|
//@[091:00092) |   | ├─Token(Comma) |,|
//@[093:00109) |   | ├─FunctionArgumentSyntax
//@[093:00109) |   | | └─FunctionCallSyntax
//@[093:00099) |   | |   ├─IdentifierSyntax
//@[093:00099) |   | |   | └─Token(Identifier) |length|
//@[099:00100) |   | |   ├─Token(LeftParen) |(|
//@[100:00108) |   | |   ├─FunctionArgumentSyntax
//@[100:00108) |   | |   | └─VariableAccessSyntax
//@[100:00108) |   | |   |   └─IdentifierSyntax
//@[100:00108) |   | |   |     └─Token(Identifier) |accounts|
//@[108:00109) |   | |   └─Token(RightParen) |)|
//@[109:00110) |   | └─Token(RightParen) |)|
//@[110:00111) |   ├─Token(Colon) |:|
//@[112:00282) |   ├─ObjectSyntax
//@[112:00113) |   | ├─Token(LeftBrace) |{|
//@[113:00114) |   | ├─Token(NewLine) |\n|
  name: '${name}-set2-${i}-${iii}'
//@[002:00034) |   | ├─ObjectPropertySyntax
//@[002:00006) |   | | ├─IdentifierSyntax
//@[002:00006) |   | | | └─Token(Identifier) |name|
//@[006:00007) |   | | ├─Token(Colon) |:|
//@[008:00034) |   | | └─StringSyntax
//@[008:00011) |   | |   ├─Token(StringLeftPiece) |'${|
//@[011:00015) |   | |   ├─VariableAccessSyntax
//@[011:00015) |   | |   | └─IdentifierSyntax
//@[011:00015) |   | |   |   └─Token(Identifier) |name|
//@[015:00024) |   | |   ├─Token(StringMiddlePiece) |}-set2-${|
//@[024:00025) |   | |   ├─VariableAccessSyntax
//@[024:00025) |   | |   | └─IdentifierSyntax
//@[024:00025) |   | |   |   └─Token(Identifier) |i|
//@[025:00029) |   | |   ├─Token(StringMiddlePiece) |}-${|
//@[029:00032) |   | |   ├─VariableAccessSyntax
//@[029:00032) |   | |   | └─IdentifierSyntax
//@[029:00032) |   | |   |   └─Token(Identifier) |iii|
//@[032:00034) |   | |   └─Token(StringRightPiece) |}'|
//@[034:00035) |   | ├─Token(NewLine) |\n|
  location: resourceGroup().location
//@[002:00036) |   | ├─ObjectPropertySyntax
//@[002:00010) |   | | ├─IdentifierSyntax
//@[002:00010) |   | | | └─Token(Identifier) |location|
//@[010:00011) |   | | ├─Token(Colon) |:|
//@[012:00036) |   | | └─PropertyAccessSyntax
//@[012:00027) |   | |   ├─FunctionCallSyntax
//@[012:00025) |   | |   | ├─IdentifierSyntax
//@[012:00025) |   | |   | | └─Token(Identifier) |resourceGroup|
//@[025:00026) |   | |   | ├─Token(LeftParen) |(|
//@[026:00027) |   | |   | └─Token(RightParen) |)|
//@[027:00028) |   | |   ├─Token(Dot) |.|
//@[028:00036) |   | |   └─IdentifierSyntax
//@[028:00036) |   | |     └─Token(Identifier) |location|
//@[036:00037) |   | ├─Token(NewLine) |\n|
  kind: 'StorageV2'
//@[002:00019) |   | ├─ObjectPropertySyntax
//@[002:00006) |   | | ├─IdentifierSyntax
//@[002:00006) |   | | | └─Token(Identifier) |kind|
//@[006:00007) |   | | ├─Token(Colon) |:|
//@[008:00019) |   | | └─StringSyntax
//@[008:00019) |   | |   └─Token(StringComplete) |'StorageV2'|
//@[019:00020) |   | ├─Token(NewLine) |\n|
  sku: {
//@[002:00037) |   | ├─ObjectPropertySyntax
//@[002:00005) |   | | ├─IdentifierSyntax
//@[002:00005) |   | | | └─Token(Identifier) |sku|
//@[005:00006) |   | | ├─Token(Colon) |:|
//@[007:00037) |   | | └─ObjectSyntax
//@[007:00008) |   | |   ├─Token(LeftBrace) |{|
//@[008:00009) |   | |   ├─Token(NewLine) |\n|
    name: 'Standard_LRS'
//@[004:00024) |   | |   ├─ObjectPropertySyntax
//@[004:00008) |   | |   | ├─IdentifierSyntax
//@[004:00008) |   | |   | | └─Token(Identifier) |name|
//@[008:00009) |   | |   | ├─Token(Colon) |:|
//@[010:00024) |   | |   | └─StringSyntax
//@[010:00024) |   | |   |   └─Token(StringComplete) |'Standard_LRS'|
//@[024:00025) |   | |   ├─Token(NewLine) |\n|
  }
//@[002:00003) |   | |   └─Token(RightBrace) |}|
//@[003:00004) |   | ├─Token(NewLine) |\n|
  dependsOn: [
//@[002:00036) |   | ├─ObjectPropertySyntax
//@[002:00011) |   | | ├─IdentifierSyntax
//@[002:00011) |   | | | └─Token(Identifier) |dependsOn|
//@[011:00012) |   | | ├─Token(Colon) |:|
//@[013:00036) |   | | └─ArraySyntax
//@[013:00014) |   | |   ├─Token(LeftSquare) |[|
//@[014:00015) |   | |   ├─Token(NewLine) |\n|
    firstSet[iii]
//@[004:00017) |   | |   ├─ArrayItemSyntax
//@[004:00017) |   | |   | └─ArrayAccessSyntax
//@[004:00012) |   | |   |   ├─VariableAccessSyntax
//@[004:00012) |   | |   |   | └─IdentifierSyntax
//@[004:00012) |   | |   |   |   └─Token(Identifier) |firstSet|
//@[012:00013) |   | |   |   ├─Token(LeftSquare) |[|
//@[013:00016) |   | |   |   ├─VariableAccessSyntax
//@[013:00016) |   | |   |   | └─IdentifierSyntax
//@[013:00016) |   | |   |   |   └─Token(Identifier) |iii|
//@[016:00017) |   | |   |   └─Token(RightSquare) |]|
//@[017:00018) |   | |   ├─Token(NewLine) |\n|
  ]
//@[002:00003) |   | |   └─Token(RightSquare) |]|
//@[003:00004) |   | ├─Token(NewLine) |\n|
}]
//@[000:00001) |   | └─Token(RightBrace) |}|
//@[001:00002) |   └─Token(RightSquare) |]|
//@[002:00004) ├─Token(NewLine) |\n\n|

// depending on collection and one resource in the collection optimizes the latter part away
//@[092:00093) ├─Token(NewLine) |\n|
resource anotherSingleResource 'Microsoft.Storage/storageAccounts@2019-06-01' = {
//@[000:00266) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00030) | ├─IdentifierSyntax
//@[009:00030) | | └─Token(Identifier) |anotherSingleResource|
//@[031:00077) | ├─StringSyntax
//@[031:00077) | | └─Token(StringComplete) |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[078:00079) | ├─Token(Assignment) |=|
//@[080:00266) | └─ObjectSyntax
//@[080:00081) |   ├─Token(LeftBrace) |{|
//@[081:00082) |   ├─Token(NewLine) |\n|
  name: '${name}single-resource-name'
//@[002:00037) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00037) |   | └─StringSyntax
//@[008:00011) |   |   ├─Token(StringLeftPiece) |'${|
//@[011:00015) |   |   ├─VariableAccessSyntax
//@[011:00015) |   |   | └─IdentifierSyntax
//@[011:00015) |   |   |   └─Token(Identifier) |name|
//@[015:00037) |   |   └─Token(StringRightPiece) |}single-resource-name'|
//@[037:00038) |   ├─Token(NewLine) |\n|
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
//@[036:00037) |   ├─Token(NewLine) |\n|
  kind: 'StorageV2'
//@[002:00019) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |kind|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00019) |   | └─StringSyntax
//@[008:00019) |   |   └─Token(StringComplete) |'StorageV2'|
//@[019:00020) |   ├─Token(NewLine) |\n|
  sku: {
//@[002:00037) |   ├─ObjectPropertySyntax
//@[002:00005) |   | ├─IdentifierSyntax
//@[002:00005) |   | | └─Token(Identifier) |sku|
//@[005:00006) |   | ├─Token(Colon) |:|
//@[007:00037) |   | └─ObjectSyntax
//@[007:00008) |   |   ├─Token(LeftBrace) |{|
//@[008:00009) |   |   ├─Token(NewLine) |\n|
    name: 'Standard_LRS'
//@[004:00024) |   |   ├─ObjectPropertySyntax
//@[004:00008) |   |   | ├─IdentifierSyntax
//@[004:00008) |   |   | | └─Token(Identifier) |name|
//@[008:00009) |   |   | ├─Token(Colon) |:|
//@[010:00024) |   |   | └─StringSyntax
//@[010:00024) |   |   |   └─Token(StringComplete) |'Standard_LRS'|
//@[024:00025) |   |   ├─Token(NewLine) |\n|
  }
//@[002:00003) |   |   └─Token(RightBrace) |}|
//@[003:00004) |   ├─Token(NewLine) |\n|
  dependsOn: [
//@[002:00049) |   ├─ObjectPropertySyntax
//@[002:00011) |   | ├─IdentifierSyntax
//@[002:00011) |   | | └─Token(Identifier) |dependsOn|
//@[011:00012) |   | ├─Token(Colon) |:|
//@[013:00049) |   | └─ArraySyntax
//@[013:00014) |   |   ├─Token(LeftSquare) |[|
//@[014:00015) |   |   ├─Token(NewLine) |\n|
    secondSet
//@[004:00013) |   |   ├─ArrayItemSyntax
//@[004:00013) |   |   | └─VariableAccessSyntax
//@[004:00013) |   |   |   └─IdentifierSyntax
//@[004:00013) |   |   |     └─Token(Identifier) |secondSet|
//@[013:00014) |   |   ├─Token(NewLine) |\n|
    secondSet[0]
//@[004:00016) |   |   ├─ArrayItemSyntax
//@[004:00016) |   |   | └─ArrayAccessSyntax
//@[004:00013) |   |   |   ├─VariableAccessSyntax
//@[004:00013) |   |   |   | └─IdentifierSyntax
//@[004:00013) |   |   |   |   └─Token(Identifier) |secondSet|
//@[013:00014) |   |   |   ├─Token(LeftSquare) |[|
//@[014:00015) |   |   |   ├─IntegerLiteralSyntax
//@[014:00015) |   |   |   | └─Token(Integer) |0|
//@[015:00016) |   |   |   └─Token(RightSquare) |]|
//@[016:00017) |   |   ├─Token(NewLine) |\n|
  ]
//@[002:00003) |   |   └─Token(RightSquare) |]|
//@[003:00004) |   ├─Token(NewLine) |\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00003) ├─Token(NewLine) |\n\n|

// vnets
//@[008:00009) ├─Token(NewLine) |\n|
var vnetConfigurations = [
//@[000:00138) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00022) | ├─IdentifierSyntax
//@[004:00022) | | └─Token(Identifier) |vnetConfigurations|
//@[023:00024) | ├─Token(Assignment) |=|
//@[025:00138) | └─ArraySyntax
//@[025:00026) |   ├─Token(LeftSquare) |[|
//@[026:00027) |   ├─Token(NewLine) |\n|
  {
//@[002:00062) |   ├─ArrayItemSyntax
//@[002:00062) |   | └─ObjectSyntax
//@[002:00003) |   |   ├─Token(LeftBrace) |{|
//@[003:00004) |   |   ├─Token(NewLine) |\n|
    name: 'one'
//@[004:00015) |   |   ├─ObjectPropertySyntax
//@[004:00008) |   |   | ├─IdentifierSyntax
//@[004:00008) |   |   | | └─Token(Identifier) |name|
//@[008:00009) |   |   | ├─Token(Colon) |:|
//@[010:00015) |   |   | └─StringSyntax
//@[010:00015) |   |   |   └─Token(StringComplete) |'one'|
//@[015:00016) |   |   ├─Token(NewLine) |\n|
    location: resourceGroup().location
//@[004:00038) |   |   ├─ObjectPropertySyntax
//@[004:00012) |   |   | ├─IdentifierSyntax
//@[004:00012) |   |   | | └─Token(Identifier) |location|
//@[012:00013) |   |   | ├─Token(Colon) |:|
//@[014:00038) |   |   | └─PropertyAccessSyntax
//@[014:00029) |   |   |   ├─FunctionCallSyntax
//@[014:00027) |   |   |   | ├─IdentifierSyntax
//@[014:00027) |   |   |   | | └─Token(Identifier) |resourceGroup|
//@[027:00028) |   |   |   | ├─Token(LeftParen) |(|
//@[028:00029) |   |   |   | └─Token(RightParen) |)|
//@[029:00030) |   |   |   ├─Token(Dot) |.|
//@[030:00038) |   |   |   └─IdentifierSyntax
//@[030:00038) |   |   |     └─Token(Identifier) |location|
//@[038:00039) |   |   ├─Token(NewLine) |\n|
  }
//@[002:00003) |   |   └─Token(RightBrace) |}|
//@[003:00004) |   ├─Token(NewLine) |\n|
  {
//@[002:00046) |   ├─ArrayItemSyntax
//@[002:00046) |   | └─ObjectSyntax
//@[002:00003) |   |   ├─Token(LeftBrace) |{|
//@[003:00004) |   |   ├─Token(NewLine) |\n|
    name: 'two'
//@[004:00015) |   |   ├─ObjectPropertySyntax
//@[004:00008) |   |   | ├─IdentifierSyntax
//@[004:00008) |   |   | | └─Token(Identifier) |name|
//@[008:00009) |   |   | ├─Token(Colon) |:|
//@[010:00015) |   |   | └─StringSyntax
//@[010:00015) |   |   |   └─Token(StringComplete) |'two'|
//@[015:00016) |   |   ├─Token(NewLine) |\n|
    location: 'westus'
//@[004:00022) |   |   ├─ObjectPropertySyntax
//@[004:00012) |   |   | ├─IdentifierSyntax
//@[004:00012) |   |   | | └─Token(Identifier) |location|
//@[012:00013) |   |   | ├─Token(Colon) |:|
//@[014:00022) |   |   | └─StringSyntax
//@[014:00022) |   |   |   └─Token(StringComplete) |'westus'|
//@[022:00023) |   |   ├─Token(NewLine) |\n|
  }
//@[002:00003) |   |   └─Token(RightBrace) |}|
//@[003:00004) |   ├─Token(NewLine) |\n|
]
//@[000:00001) |   └─Token(RightSquare) |]|
//@[001:00003) ├─Token(NewLine) |\n\n|

resource vnets 'Microsoft.Network/virtualNetworks@2020-06-01' = [for (vnetConfig, index) in vnetConfigurations: {
//@[000:00186) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00014) | ├─IdentifierSyntax
//@[009:00014) | | └─Token(Identifier) |vnets|
//@[015:00061) | ├─StringSyntax
//@[015:00061) | | └─Token(StringComplete) |'Microsoft.Network/virtualNetworks@2020-06-01'|
//@[062:00063) | ├─Token(Assignment) |=|
//@[064:00186) | └─ForSyntax
//@[064:00065) |   ├─Token(LeftSquare) |[|
//@[065:00068) |   ├─Token(Identifier) |for|
//@[069:00088) |   ├─VariableBlockSyntax
//@[069:00070) |   | ├─Token(LeftParen) |(|
//@[070:00080) |   | ├─LocalVariableSyntax
//@[070:00080) |   | | └─IdentifierSyntax
//@[070:00080) |   | |   └─Token(Identifier) |vnetConfig|
//@[080:00081) |   | ├─Token(Comma) |,|
//@[082:00087) |   | ├─LocalVariableSyntax
//@[082:00087) |   | | └─IdentifierSyntax
//@[082:00087) |   | |   └─Token(Identifier) |index|
//@[087:00088) |   | └─Token(RightParen) |)|
//@[089:00091) |   ├─Token(Identifier) |in|
//@[092:00110) |   ├─VariableAccessSyntax
//@[092:00110) |   | └─IdentifierSyntax
//@[092:00110) |   |   └─Token(Identifier) |vnetConfigurations|
//@[110:00111) |   ├─Token(Colon) |:|
//@[112:00185) |   ├─ObjectSyntax
//@[112:00113) |   | ├─Token(LeftBrace) |{|
//@[113:00114) |   | ├─Token(NewLine) |\n|
  name: '${vnetConfig.name}-${index}'
//@[002:00037) |   | ├─ObjectPropertySyntax
//@[002:00006) |   | | ├─IdentifierSyntax
//@[002:00006) |   | | | └─Token(Identifier) |name|
//@[006:00007) |   | | ├─Token(Colon) |:|
//@[008:00037) |   | | └─StringSyntax
//@[008:00011) |   | |   ├─Token(StringLeftPiece) |'${|
//@[011:00026) |   | |   ├─PropertyAccessSyntax
//@[011:00021) |   | |   | ├─VariableAccessSyntax
//@[011:00021) |   | |   | | └─IdentifierSyntax
//@[011:00021) |   | |   | |   └─Token(Identifier) |vnetConfig|
//@[021:00022) |   | |   | ├─Token(Dot) |.|
//@[022:00026) |   | |   | └─IdentifierSyntax
//@[022:00026) |   | |   |   └─Token(Identifier) |name|
//@[026:00030) |   | |   ├─Token(StringMiddlePiece) |}-${|
//@[030:00035) |   | |   ├─VariableAccessSyntax
//@[030:00035) |   | |   | └─IdentifierSyntax
//@[030:00035) |   | |   |   └─Token(Identifier) |index|
//@[035:00037) |   | |   └─Token(StringRightPiece) |}'|
//@[037:00038) |   | ├─Token(NewLine) |\n|
  location: vnetConfig.location
//@[002:00031) |   | ├─ObjectPropertySyntax
//@[002:00010) |   | | ├─IdentifierSyntax
//@[002:00010) |   | | | └─Token(Identifier) |location|
//@[010:00011) |   | | ├─Token(Colon) |:|
//@[012:00031) |   | | └─PropertyAccessSyntax
//@[012:00022) |   | |   ├─VariableAccessSyntax
//@[012:00022) |   | |   | └─IdentifierSyntax
//@[012:00022) |   | |   |   └─Token(Identifier) |vnetConfig|
//@[022:00023) |   | |   ├─Token(Dot) |.|
//@[023:00031) |   | |   └─IdentifierSyntax
//@[023:00031) |   | |     └─Token(Identifier) |location|
//@[031:00032) |   | ├─Token(NewLine) |\n|
}]
//@[000:00001) |   | └─Token(RightBrace) |}|
//@[001:00002) |   └─Token(RightSquare) |]|
//@[002:00004) ├─Token(NewLine) |\n\n|

// implicit dependency on single resource from a resource collection
//@[068:00069) ├─Token(NewLine) |\n|
resource implicitDependencyOnSingleResourceByIndex 'Microsoft.Network/dnsZones@2018-05-01' = {
//@[000:00237) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00050) | ├─IdentifierSyntax
//@[009:00050) | | └─Token(Identifier) |implicitDependencyOnSingleResourceByIndex|
//@[051:00090) | ├─StringSyntax
//@[051:00090) | | └─Token(StringComplete) |'Microsoft.Network/dnsZones@2018-05-01'|
//@[091:00092) | ├─Token(Assignment) |=|
//@[093:00237) | └─ObjectSyntax
//@[093:00094) |   ├─Token(LeftBrace) |{|
//@[094:00095) |   ├─Token(NewLine) |\n|
  name: 'test'
//@[002:00014) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00014) |   | └─StringSyntax
//@[008:00014) |   |   └─Token(StringComplete) |'test'|
//@[014:00015) |   ├─Token(NewLine) |\n|
  location: 'global'
//@[002:00020) |   ├─ObjectPropertySyntax
//@[002:00010) |   | ├─IdentifierSyntax
//@[002:00010) |   | | └─Token(Identifier) |location|
//@[010:00011) |   | ├─Token(Colon) |:|
//@[012:00020) |   | └─StringSyntax
//@[012:00020) |   |   └─Token(StringComplete) |'global'|
//@[020:00021) |   ├─Token(NewLine) |\n|
  properties: {
//@[002:00104) |   ├─ObjectPropertySyntax
//@[002:00012) |   | ├─IdentifierSyntax
//@[002:00012) |   | | └─Token(Identifier) |properties|
//@[012:00013) |   | ├─Token(Colon) |:|
//@[014:00104) |   | └─ObjectSyntax
//@[014:00015) |   |   ├─Token(LeftBrace) |{|
//@[015:00016) |   |   ├─Token(NewLine) |\n|
    resolutionVirtualNetworks: [
//@[004:00084) |   |   ├─ObjectPropertySyntax
//@[004:00029) |   |   | ├─IdentifierSyntax
//@[004:00029) |   |   | | └─Token(Identifier) |resolutionVirtualNetworks|
//@[029:00030) |   |   | ├─Token(Colon) |:|
//@[031:00084) |   |   | └─ArraySyntax
//@[031:00032) |   |   |   ├─Token(LeftSquare) |[|
//@[032:00033) |   |   |   ├─Token(NewLine) |\n|
      {
//@[006:00045) |   |   |   ├─ArrayItemSyntax
//@[006:00045) |   |   |   | └─ObjectSyntax
//@[006:00007) |   |   |   |   ├─Token(LeftBrace) |{|
//@[007:00008) |   |   |   |   ├─Token(NewLine) |\n|
        id: vnets[index+1].id
//@[008:00029) |   |   |   |   ├─ObjectPropertySyntax
//@[008:00010) |   |   |   |   | ├─IdentifierSyntax
//@[008:00010) |   |   |   |   | | └─Token(Identifier) |id|
//@[010:00011) |   |   |   |   | ├─Token(Colon) |:|
//@[012:00029) |   |   |   |   | └─PropertyAccessSyntax
//@[012:00026) |   |   |   |   |   ├─ArrayAccessSyntax
//@[012:00017) |   |   |   |   |   | ├─VariableAccessSyntax
//@[012:00017) |   |   |   |   |   | | └─IdentifierSyntax
//@[012:00017) |   |   |   |   |   | |   └─Token(Identifier) |vnets|
//@[017:00018) |   |   |   |   |   | ├─Token(LeftSquare) |[|
//@[018:00025) |   |   |   |   |   | ├─BinaryOperationSyntax
//@[018:00023) |   |   |   |   |   | | ├─VariableAccessSyntax
//@[018:00023) |   |   |   |   |   | | | └─IdentifierSyntax
//@[018:00023) |   |   |   |   |   | | |   └─Token(Identifier) |index|
//@[023:00024) |   |   |   |   |   | | ├─Token(Plus) |+|
//@[024:00025) |   |   |   |   |   | | └─IntegerLiteralSyntax
//@[024:00025) |   |   |   |   |   | |   └─Token(Integer) |1|
//@[025:00026) |   |   |   |   |   | └─Token(RightSquare) |]|
//@[026:00027) |   |   |   |   |   ├─Token(Dot) |.|
//@[027:00029) |   |   |   |   |   └─IdentifierSyntax
//@[027:00029) |   |   |   |   |     └─Token(Identifier) |id|
//@[029:00030) |   |   |   |   ├─Token(NewLine) |\n|
      }
//@[006:00007) |   |   |   |   └─Token(RightBrace) |}|
//@[007:00008) |   |   |   ├─Token(NewLine) |\n|
    ]
//@[004:00005) |   |   |   └─Token(RightSquare) |]|
//@[005:00006) |   |   ├─Token(NewLine) |\n|
  }
//@[002:00003) |   |   └─Token(RightBrace) |}|
//@[003:00004) |   ├─Token(NewLine) |\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00003) ├─Token(NewLine) |\n\n|

// implicit and explicit dependency combined
//@[044:00045) ├─Token(NewLine) |\n|
resource combinedDependencies 'Microsoft.Network/dnsZones@2018-05-01' = {
//@[000:00294) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00029) | ├─IdentifierSyntax
//@[009:00029) | | └─Token(Identifier) |combinedDependencies|
//@[030:00069) | ├─StringSyntax
//@[030:00069) | | └─Token(StringComplete) |'Microsoft.Network/dnsZones@2018-05-01'|
//@[070:00071) | ├─Token(Assignment) |=|
//@[072:00294) | └─ObjectSyntax
//@[072:00073) |   ├─Token(LeftBrace) |{|
//@[073:00074) |   ├─Token(NewLine) |\n|
  name: 'test2'
//@[002:00015) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00015) |   | └─StringSyntax
//@[008:00015) |   |   └─Token(StringComplete) |'test2'|
//@[015:00016) |   ├─Token(NewLine) |\n|
  location: 'global'
//@[002:00020) |   ├─ObjectPropertySyntax
//@[002:00010) |   | ├─IdentifierSyntax
//@[002:00010) |   | | └─Token(Identifier) |location|
//@[010:00011) |   | ├─Token(Colon) |:|
//@[012:00020) |   | └─StringSyntax
//@[012:00020) |   |   └─Token(StringComplete) |'global'|
//@[020:00021) |   ├─Token(NewLine) |\n|
  properties: {
//@[002:00152) |   ├─ObjectPropertySyntax
//@[002:00012) |   | ├─IdentifierSyntax
//@[002:00012) |   | | └─Token(Identifier) |properties|
//@[012:00013) |   | ├─Token(Colon) |:|
//@[014:00152) |   | └─ObjectSyntax
//@[014:00015) |   |   ├─Token(LeftBrace) |{|
//@[015:00016) |   |   ├─Token(NewLine) |\n|
    resolutionVirtualNetworks: [
//@[004:00132) |   |   ├─ObjectPropertySyntax
//@[004:00029) |   |   | ├─IdentifierSyntax
//@[004:00029) |   |   | | └─Token(Identifier) |resolutionVirtualNetworks|
//@[029:00030) |   |   | ├─Token(Colon) |:|
//@[031:00132) |   |   | └─ArraySyntax
//@[031:00032) |   |   |   ├─Token(LeftSquare) |[|
//@[032:00033) |   |   |   ├─Token(NewLine) |\n|
      {
//@[006:00045) |   |   |   ├─ArrayItemSyntax
//@[006:00045) |   |   |   | └─ObjectSyntax
//@[006:00007) |   |   |   |   ├─Token(LeftBrace) |{|
//@[007:00008) |   |   |   |   ├─Token(NewLine) |\n|
        id: vnets[index-1].id
//@[008:00029) |   |   |   |   ├─ObjectPropertySyntax
//@[008:00010) |   |   |   |   | ├─IdentifierSyntax
//@[008:00010) |   |   |   |   | | └─Token(Identifier) |id|
//@[010:00011) |   |   |   |   | ├─Token(Colon) |:|
//@[012:00029) |   |   |   |   | └─PropertyAccessSyntax
//@[012:00026) |   |   |   |   |   ├─ArrayAccessSyntax
//@[012:00017) |   |   |   |   |   | ├─VariableAccessSyntax
//@[012:00017) |   |   |   |   |   | | └─IdentifierSyntax
//@[012:00017) |   |   |   |   |   | |   └─Token(Identifier) |vnets|
//@[017:00018) |   |   |   |   |   | ├─Token(LeftSquare) |[|
//@[018:00025) |   |   |   |   |   | ├─BinaryOperationSyntax
//@[018:00023) |   |   |   |   |   | | ├─VariableAccessSyntax
//@[018:00023) |   |   |   |   |   | | | └─IdentifierSyntax
//@[018:00023) |   |   |   |   |   | | |   └─Token(Identifier) |index|
//@[023:00024) |   |   |   |   |   | | ├─Token(Minus) |-|
//@[024:00025) |   |   |   |   |   | | └─IntegerLiteralSyntax
//@[024:00025) |   |   |   |   |   | |   └─Token(Integer) |1|
//@[025:00026) |   |   |   |   |   | └─Token(RightSquare) |]|
//@[026:00027) |   |   |   |   |   ├─Token(Dot) |.|
//@[027:00029) |   |   |   |   |   └─IdentifierSyntax
//@[027:00029) |   |   |   |   |     └─Token(Identifier) |id|
//@[029:00030) |   |   |   |   ├─Token(NewLine) |\n|
      }
//@[006:00007) |   |   |   |   └─Token(RightBrace) |}|
//@[007:00008) |   |   |   ├─Token(NewLine) |\n|
      {
//@[006:00047) |   |   |   ├─ArrayItemSyntax
//@[006:00047) |   |   |   | └─ObjectSyntax
//@[006:00007) |   |   |   |   ├─Token(LeftBrace) |{|
//@[007:00008) |   |   |   |   ├─Token(NewLine) |\n|
        id: vnets[index * 2].id
//@[008:00031) |   |   |   |   ├─ObjectPropertySyntax
//@[008:00010) |   |   |   |   | ├─IdentifierSyntax
//@[008:00010) |   |   |   |   | | └─Token(Identifier) |id|
//@[010:00011) |   |   |   |   | ├─Token(Colon) |:|
//@[012:00031) |   |   |   |   | └─PropertyAccessSyntax
//@[012:00028) |   |   |   |   |   ├─ArrayAccessSyntax
//@[012:00017) |   |   |   |   |   | ├─VariableAccessSyntax
//@[012:00017) |   |   |   |   |   | | └─IdentifierSyntax
//@[012:00017) |   |   |   |   |   | |   └─Token(Identifier) |vnets|
//@[017:00018) |   |   |   |   |   | ├─Token(LeftSquare) |[|
//@[018:00027) |   |   |   |   |   | ├─BinaryOperationSyntax
//@[018:00023) |   |   |   |   |   | | ├─VariableAccessSyntax
//@[018:00023) |   |   |   |   |   | | | └─IdentifierSyntax
//@[018:00023) |   |   |   |   |   | | |   └─Token(Identifier) |index|
//@[024:00025) |   |   |   |   |   | | ├─Token(Asterisk) |*|
//@[026:00027) |   |   |   |   |   | | └─IntegerLiteralSyntax
//@[026:00027) |   |   |   |   |   | |   └─Token(Integer) |2|
//@[027:00028) |   |   |   |   |   | └─Token(RightSquare) |]|
//@[028:00029) |   |   |   |   |   ├─Token(Dot) |.|
//@[029:00031) |   |   |   |   |   └─IdentifierSyntax
//@[029:00031) |   |   |   |   |     └─Token(Identifier) |id|
//@[031:00032) |   |   |   |   ├─Token(NewLine) |\n|
      }
//@[006:00007) |   |   |   |   └─Token(RightBrace) |}|
//@[007:00008) |   |   |   ├─Token(NewLine) |\n|
    ]
//@[004:00005) |   |   |   └─Token(RightSquare) |]|
//@[005:00006) |   |   ├─Token(NewLine) |\n|
  }
//@[002:00003) |   |   └─Token(RightBrace) |}|
//@[003:00004) |   ├─Token(NewLine) |\n|
  dependsOn: [
//@[002:00028) |   ├─ObjectPropertySyntax
//@[002:00011) |   | ├─IdentifierSyntax
//@[002:00011) |   | | └─Token(Identifier) |dependsOn|
//@[011:00012) |   | ├─Token(Colon) |:|
//@[013:00028) |   | └─ArraySyntax
//@[013:00014) |   |   ├─Token(LeftSquare) |[|
//@[014:00015) |   |   ├─Token(NewLine) |\n|
    vnets
//@[004:00009) |   |   ├─ArrayItemSyntax
//@[004:00009) |   |   | └─VariableAccessSyntax
//@[004:00009) |   |   |   └─IdentifierSyntax
//@[004:00009) |   |   |     └─Token(Identifier) |vnets|
//@[009:00010) |   |   ├─Token(NewLine) |\n|
  ]
//@[002:00003) |   |   └─Token(RightSquare) |]|
//@[003:00004) |   ├─Token(NewLine) |\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00003) ├─Token(NewLine) |\n\n|

// single module
//@[016:00017) ├─Token(NewLine) |\n|
module singleModule 'passthrough.bicep' = {
//@[000:00097) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00019) | ├─IdentifierSyntax
//@[007:00019) | | └─Token(Identifier) |singleModule|
//@[020:00039) | ├─StringSyntax
//@[020:00039) | | └─Token(StringComplete) |'passthrough.bicep'|
//@[040:00041) | ├─Token(Assignment) |=|
//@[042:00097) | └─ObjectSyntax
//@[042:00043) |   ├─Token(LeftBrace) |{|
//@[043:00044) |   ├─Token(NewLine) |\n|
  name: 'test'
//@[002:00014) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00014) |   | └─StringSyntax
//@[008:00014) |   |   └─Token(StringComplete) |'test'|
//@[014:00015) |   ├─Token(NewLine) |\n|
  params: {
//@[002:00036) |   ├─ObjectPropertySyntax
//@[002:00008) |   | ├─IdentifierSyntax
//@[002:00008) |   | | └─Token(Identifier) |params|
//@[008:00009) |   | ├─Token(Colon) |:|
//@[010:00036) |   | └─ObjectSyntax
//@[010:00011) |   |   ├─Token(LeftBrace) |{|
//@[011:00012) |   |   ├─Token(NewLine) |\n|
    myInput: 'hello'
//@[004:00020) |   |   ├─ObjectPropertySyntax
//@[004:00011) |   |   | ├─IdentifierSyntax
//@[004:00011) |   |   | | └─Token(Identifier) |myInput|
//@[011:00012) |   |   | ├─Token(Colon) |:|
//@[013:00020) |   |   | └─StringSyntax
//@[013:00020) |   |   |   └─Token(StringComplete) |'hello'|
//@[020:00021) |   |   ├─Token(NewLine) |\n|
  }
//@[002:00003) |   |   └─Token(RightBrace) |}|
//@[003:00004) |   ├─Token(NewLine) |\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00003) ├─Token(NewLine) |\n\n|

var moduleSetup = [
//@[000:00047) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00015) | ├─IdentifierSyntax
//@[004:00015) | | └─Token(Identifier) |moduleSetup|
//@[016:00017) | ├─Token(Assignment) |=|
//@[018:00047) | └─ArraySyntax
//@[018:00019) |   ├─Token(LeftSquare) |[|
//@[019:00020) |   ├─Token(NewLine) |\n|
  'one'
//@[002:00007) |   ├─ArrayItemSyntax
//@[002:00007) |   | └─StringSyntax
//@[002:00007) |   |   └─Token(StringComplete) |'one'|
//@[007:00008) |   ├─Token(NewLine) |\n|
  'two'
//@[002:00007) |   ├─ArrayItemSyntax
//@[002:00007) |   | └─StringSyntax
//@[002:00007) |   |   └─Token(StringComplete) |'two'|
//@[007:00008) |   ├─Token(NewLine) |\n|
  'three'
//@[002:00009) |   ├─ArrayItemSyntax
//@[002:00009) |   | └─StringSyntax
//@[002:00009) |   |   └─Token(StringComplete) |'three'|
//@[009:00010) |   ├─Token(NewLine) |\n|
]
//@[000:00001) |   └─Token(RightSquare) |]|
//@[001:00003) ├─Token(NewLine) |\n\n|

// module collection plus explicit dependency on single module
//@[062:00063) ├─Token(NewLine) |\n|
@sys.batchSize(3)
//@[000:00293) ├─ModuleDeclarationSyntax
//@[000:00017) | ├─DecoratorSyntax
//@[000:00001) | | ├─Token(At) |@|
//@[001:00017) | | └─InstanceFunctionCallSyntax
//@[001:00004) | |   ├─VariableAccessSyntax
//@[001:00004) | |   | └─IdentifierSyntax
//@[001:00004) | |   |   └─Token(Identifier) |sys|
//@[004:00005) | |   ├─Token(Dot) |.|
//@[005:00014) | |   ├─IdentifierSyntax
//@[005:00014) | |   | └─Token(Identifier) |batchSize|
//@[014:00015) | |   ├─Token(LeftParen) |(|
//@[015:00016) | |   ├─FunctionArgumentSyntax
//@[015:00016) | |   | └─IntegerLiteralSyntax
//@[015:00016) | |   |   └─Token(Integer) |3|
//@[016:00017) | |   └─Token(RightParen) |)|
//@[017:00018) | ├─Token(NewLine) |\n|
module moduleCollectionWithSingleDependency 'passthrough.bicep' = [for (moduleName, moduleIndex) in moduleSetup: {
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00043) | ├─IdentifierSyntax
//@[007:00043) | | └─Token(Identifier) |moduleCollectionWithSingleDependency|
//@[044:00063) | ├─StringSyntax
//@[044:00063) | | └─Token(StringComplete) |'passthrough.bicep'|
//@[064:00065) | ├─Token(Assignment) |=|
//@[066:00275) | └─ForSyntax
//@[066:00067) |   ├─Token(LeftSquare) |[|
//@[067:00070) |   ├─Token(Identifier) |for|
//@[071:00096) |   ├─VariableBlockSyntax
//@[071:00072) |   | ├─Token(LeftParen) |(|
//@[072:00082) |   | ├─LocalVariableSyntax
//@[072:00082) |   | | └─IdentifierSyntax
//@[072:00082) |   | |   └─Token(Identifier) |moduleName|
//@[082:00083) |   | ├─Token(Comma) |,|
//@[084:00095) |   | ├─LocalVariableSyntax
//@[084:00095) |   | | └─IdentifierSyntax
//@[084:00095) |   | |   └─Token(Identifier) |moduleIndex|
//@[095:00096) |   | └─Token(RightParen) |)|
//@[097:00099) |   ├─Token(Identifier) |in|
//@[100:00111) |   ├─VariableAccessSyntax
//@[100:00111) |   | └─IdentifierSyntax
//@[100:00111) |   |   └─Token(Identifier) |moduleSetup|
//@[111:00112) |   ├─Token(Colon) |:|
//@[113:00274) |   ├─ObjectSyntax
//@[113:00114) |   | ├─Token(LeftBrace) |{|
//@[114:00115) |   | ├─Token(NewLine) |\n|
  name: concat(moduleName, moduleIndex)
//@[002:00039) |   | ├─ObjectPropertySyntax
//@[002:00006) |   | | ├─IdentifierSyntax
//@[002:00006) |   | | | └─Token(Identifier) |name|
//@[006:00007) |   | | ├─Token(Colon) |:|
//@[008:00039) |   | | └─FunctionCallSyntax
//@[008:00014) |   | |   ├─IdentifierSyntax
//@[008:00014) |   | |   | └─Token(Identifier) |concat|
//@[014:00015) |   | |   ├─Token(LeftParen) |(|
//@[015:00025) |   | |   ├─FunctionArgumentSyntax
//@[015:00025) |   | |   | └─VariableAccessSyntax
//@[015:00025) |   | |   |   └─IdentifierSyntax
//@[015:00025) |   | |   |     └─Token(Identifier) |moduleName|
//@[025:00026) |   | |   ├─Token(Comma) |,|
//@[027:00038) |   | |   ├─FunctionArgumentSyntax
//@[027:00038) |   | |   | └─VariableAccessSyntax
//@[027:00038) |   | |   |   └─IdentifierSyntax
//@[027:00038) |   | |   |     └─Token(Identifier) |moduleIndex|
//@[038:00039) |   | |   └─Token(RightParen) |)|
//@[039:00040) |   | ├─Token(NewLine) |\n|
  params: {
//@[002:00062) |   | ├─ObjectPropertySyntax
//@[002:00008) |   | | ├─IdentifierSyntax
//@[002:00008) |   | | | └─Token(Identifier) |params|
//@[008:00009) |   | | ├─Token(Colon) |:|
//@[010:00062) |   | | └─ObjectSyntax
//@[010:00011) |   | |   ├─Token(LeftBrace) |{|
//@[011:00012) |   | |   ├─Token(NewLine) |\n|
    myInput: 'in-${moduleName}-${moduleIndex}'
//@[004:00046) |   | |   ├─ObjectPropertySyntax
//@[004:00011) |   | |   | ├─IdentifierSyntax
//@[004:00011) |   | |   | | └─Token(Identifier) |myInput|
//@[011:00012) |   | |   | ├─Token(Colon) |:|
//@[013:00046) |   | |   | └─StringSyntax
//@[013:00019) |   | |   |   ├─Token(StringLeftPiece) |'in-${|
//@[019:00029) |   | |   |   ├─VariableAccessSyntax
//@[019:00029) |   | |   |   | └─IdentifierSyntax
//@[019:00029) |   | |   |   |   └─Token(Identifier) |moduleName|
//@[029:00033) |   | |   |   ├─Token(StringMiddlePiece) |}-${|
//@[033:00044) |   | |   |   ├─VariableAccessSyntax
//@[033:00044) |   | |   |   | └─IdentifierSyntax
//@[033:00044) |   | |   |   |   └─Token(Identifier) |moduleIndex|
//@[044:00046) |   | |   |   └─Token(StringRightPiece) |}'|
//@[046:00047) |   | |   ├─Token(NewLine) |\n|
  }
//@[002:00003) |   | |   └─Token(RightBrace) |}|
//@[003:00004) |   | ├─Token(NewLine) |\n|
  dependsOn: [
//@[002:00054) |   | ├─ObjectPropertySyntax
//@[002:00011) |   | | ├─IdentifierSyntax
//@[002:00011) |   | | | └─Token(Identifier) |dependsOn|
//@[011:00012) |   | | ├─Token(Colon) |:|
//@[013:00054) |   | | └─ArraySyntax
//@[013:00014) |   | |   ├─Token(LeftSquare) |[|
//@[014:00015) |   | |   ├─Token(NewLine) |\n|
    singleModule
//@[004:00016) |   | |   ├─ArrayItemSyntax
//@[004:00016) |   | |   | └─VariableAccessSyntax
//@[004:00016) |   | |   |   └─IdentifierSyntax
//@[004:00016) |   | |   |     └─Token(Identifier) |singleModule|
//@[016:00017) |   | |   ├─Token(NewLine) |\n|
    singleResource
//@[004:00018) |   | |   ├─ArrayItemSyntax
//@[004:00018) |   | |   | └─VariableAccessSyntax
//@[004:00018) |   | |   |   └─IdentifierSyntax
//@[004:00018) |   | |   |     └─Token(Identifier) |singleResource|
//@[018:00019) |   | |   ├─Token(NewLine) |\n|
  ]
//@[002:00003) |   | |   └─Token(RightSquare) |]|
//@[003:00004) |   | ├─Token(NewLine) |\n|
}]
//@[000:00001) |   | └─Token(RightBrace) |}|
//@[001:00002) |   └─Token(RightSquare) |]|
//@[002:00004) ├─Token(NewLine) |\n\n|

// another module collection with dependency on another module collection
//@[073:00074) ├─Token(NewLine) |\n|
module moduleCollectionWithCollectionDependencies 'passthrough.bicep' = [for (moduleName, moduleIndex) in moduleSetup: {
//@[000:00306) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00049) | ├─IdentifierSyntax
//@[007:00049) | | └─Token(Identifier) |moduleCollectionWithCollectionDependencies|
//@[050:00069) | ├─StringSyntax
//@[050:00069) | | └─Token(StringComplete) |'passthrough.bicep'|
//@[070:00071) | ├─Token(Assignment) |=|
//@[072:00306) | └─ForSyntax
//@[072:00073) |   ├─Token(LeftSquare) |[|
//@[073:00076) |   ├─Token(Identifier) |for|
//@[077:00102) |   ├─VariableBlockSyntax
//@[077:00078) |   | ├─Token(LeftParen) |(|
//@[078:00088) |   | ├─LocalVariableSyntax
//@[078:00088) |   | | └─IdentifierSyntax
//@[078:00088) |   | |   └─Token(Identifier) |moduleName|
//@[088:00089) |   | ├─Token(Comma) |,|
//@[090:00101) |   | ├─LocalVariableSyntax
//@[090:00101) |   | | └─IdentifierSyntax
//@[090:00101) |   | |   └─Token(Identifier) |moduleIndex|
//@[101:00102) |   | └─Token(RightParen) |)|
//@[103:00105) |   ├─Token(Identifier) |in|
//@[106:00117) |   ├─VariableAccessSyntax
//@[106:00117) |   | └─IdentifierSyntax
//@[106:00117) |   |   └─Token(Identifier) |moduleSetup|
//@[117:00118) |   ├─Token(Colon) |:|
//@[119:00305) |   ├─ObjectSyntax
//@[119:00120) |   | ├─Token(LeftBrace) |{|
//@[120:00121) |   | ├─Token(NewLine) |\n|
  name: concat(moduleName, moduleIndex)
//@[002:00039) |   | ├─ObjectPropertySyntax
//@[002:00006) |   | | ├─IdentifierSyntax
//@[002:00006) |   | | | └─Token(Identifier) |name|
//@[006:00007) |   | | ├─Token(Colon) |:|
//@[008:00039) |   | | └─FunctionCallSyntax
//@[008:00014) |   | |   ├─IdentifierSyntax
//@[008:00014) |   | |   | └─Token(Identifier) |concat|
//@[014:00015) |   | |   ├─Token(LeftParen) |(|
//@[015:00025) |   | |   ├─FunctionArgumentSyntax
//@[015:00025) |   | |   | └─VariableAccessSyntax
//@[015:00025) |   | |   |   └─IdentifierSyntax
//@[015:00025) |   | |   |     └─Token(Identifier) |moduleName|
//@[025:00026) |   | |   ├─Token(Comma) |,|
//@[027:00038) |   | |   ├─FunctionArgumentSyntax
//@[027:00038) |   | |   | └─VariableAccessSyntax
//@[027:00038) |   | |   |   └─IdentifierSyntax
//@[027:00038) |   | |   |     └─Token(Identifier) |moduleIndex|
//@[038:00039) |   | |   └─Token(RightParen) |)|
//@[039:00040) |   | ├─Token(NewLine) |\n|
  params: {
//@[002:00062) |   | ├─ObjectPropertySyntax
//@[002:00008) |   | | ├─IdentifierSyntax
//@[002:00008) |   | | | └─Token(Identifier) |params|
//@[008:00009) |   | | ├─Token(Colon) |:|
//@[010:00062) |   | | └─ObjectSyntax
//@[010:00011) |   | |   ├─Token(LeftBrace) |{|
//@[011:00012) |   | |   ├─Token(NewLine) |\n|
    myInput: 'in-${moduleName}-${moduleIndex}'
//@[004:00046) |   | |   ├─ObjectPropertySyntax
//@[004:00011) |   | |   | ├─IdentifierSyntax
//@[004:00011) |   | |   | | └─Token(Identifier) |myInput|
//@[011:00012) |   | |   | ├─Token(Colon) |:|
//@[013:00046) |   | |   | └─StringSyntax
//@[013:00019) |   | |   |   ├─Token(StringLeftPiece) |'in-${|
//@[019:00029) |   | |   |   ├─VariableAccessSyntax
//@[019:00029) |   | |   |   | └─IdentifierSyntax
//@[019:00029) |   | |   |   |   └─Token(Identifier) |moduleName|
//@[029:00033) |   | |   |   ├─Token(StringMiddlePiece) |}-${|
//@[033:00044) |   | |   |   ├─VariableAccessSyntax
//@[033:00044) |   | |   |   | └─IdentifierSyntax
//@[033:00044) |   | |   |   |   └─Token(Identifier) |moduleIndex|
//@[044:00046) |   | |   |   └─Token(StringRightPiece) |}'|
//@[046:00047) |   | |   ├─Token(NewLine) |\n|
  }
//@[002:00003) |   | |   └─Token(RightBrace) |}|
//@[003:00004) |   | ├─Token(NewLine) |\n|
  dependsOn: [
//@[002:00079) |   | ├─ObjectPropertySyntax
//@[002:00011) |   | | ├─IdentifierSyntax
//@[002:00011) |   | | | └─Token(Identifier) |dependsOn|
//@[011:00012) |   | | ├─Token(Colon) |:|
//@[013:00079) |   | | └─ArraySyntax
//@[013:00014) |   | |   ├─Token(LeftSquare) |[|
//@[014:00015) |   | |   ├─Token(NewLine) |\n|
    storageAccounts
//@[004:00019) |   | |   ├─ArrayItemSyntax
//@[004:00019) |   | |   | └─VariableAccessSyntax
//@[004:00019) |   | |   |   └─IdentifierSyntax
//@[004:00019) |   | |   |     └─Token(Identifier) |storageAccounts|
//@[019:00020) |   | |   ├─Token(NewLine) |\n|
    moduleCollectionWithSingleDependency
//@[004:00040) |   | |   ├─ArrayItemSyntax
//@[004:00040) |   | |   | └─VariableAccessSyntax
//@[004:00040) |   | |   |   └─IdentifierSyntax
//@[004:00040) |   | |   |     └─Token(Identifier) |moduleCollectionWithSingleDependency|
//@[040:00041) |   | |   ├─Token(NewLine) |\n|
  ]
//@[002:00003) |   | |   └─Token(RightSquare) |]|
//@[003:00004) |   | ├─Token(NewLine) |\n|
}]
//@[000:00001) |   | └─Token(RightBrace) |}|
//@[001:00002) |   └─Token(RightSquare) |]|
//@[002:00004) ├─Token(NewLine) |\n\n|

module singleModuleWithIndexedDependencies 'passthrough.bicep' = {
//@[000:00290) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00042) | ├─IdentifierSyntax
//@[007:00042) | | └─Token(Identifier) |singleModuleWithIndexedDependencies|
//@[043:00062) | ├─StringSyntax
//@[043:00062) | | └─Token(StringComplete) |'passthrough.bicep'|
//@[063:00064) | ├─Token(Assignment) |=|
//@[065:00290) | └─ObjectSyntax
//@[065:00066) |   ├─Token(LeftBrace) |{|
//@[066:00067) |   ├─Token(NewLine) |\n|
  name: 'hello'
//@[002:00015) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00015) |   | └─StringSyntax
//@[008:00015) |   |   └─Token(StringComplete) |'hello'|
//@[015:00016) |   ├─Token(NewLine) |\n|
  params: {
//@[002:00153) |   ├─ObjectPropertySyntax
//@[002:00008) |   | ├─IdentifierSyntax
//@[002:00008) |   | | └─Token(Identifier) |params|
//@[008:00009) |   | ├─Token(Colon) |:|
//@[010:00153) |   | └─ObjectSyntax
//@[010:00011) |   |   ├─Token(LeftBrace) |{|
//@[011:00012) |   |   ├─Token(NewLine) |\n|
    myInput: concat(moduleCollectionWithCollectionDependencies[index].outputs.myOutput, storageAccounts[index * 3].properties.accessTier)
//@[004:00137) |   |   ├─ObjectPropertySyntax
//@[004:00011) |   |   | ├─IdentifierSyntax
//@[004:00011) |   |   | | └─Token(Identifier) |myInput|
//@[011:00012) |   |   | ├─Token(Colon) |:|
//@[013:00137) |   |   | └─FunctionCallSyntax
//@[013:00019) |   |   |   ├─IdentifierSyntax
//@[013:00019) |   |   |   | └─Token(Identifier) |concat|
//@[019:00020) |   |   |   ├─Token(LeftParen) |(|
//@[020:00086) |   |   |   ├─FunctionArgumentSyntax
//@[020:00086) |   |   |   | └─PropertyAccessSyntax
//@[020:00077) |   |   |   |   ├─PropertyAccessSyntax
//@[020:00069) |   |   |   |   | ├─ArrayAccessSyntax
//@[020:00062) |   |   |   |   | | ├─VariableAccessSyntax
//@[020:00062) |   |   |   |   | | | └─IdentifierSyntax
//@[020:00062) |   |   |   |   | | |   └─Token(Identifier) |moduleCollectionWithCollectionDependencies|
//@[062:00063) |   |   |   |   | | ├─Token(LeftSquare) |[|
//@[063:00068) |   |   |   |   | | ├─VariableAccessSyntax
//@[063:00068) |   |   |   |   | | | └─IdentifierSyntax
//@[063:00068) |   |   |   |   | | |   └─Token(Identifier) |index|
//@[068:00069) |   |   |   |   | | └─Token(RightSquare) |]|
//@[069:00070) |   |   |   |   | ├─Token(Dot) |.|
//@[070:00077) |   |   |   |   | └─IdentifierSyntax
//@[070:00077) |   |   |   |   |   └─Token(Identifier) |outputs|
//@[077:00078) |   |   |   |   ├─Token(Dot) |.|
//@[078:00086) |   |   |   |   └─IdentifierSyntax
//@[078:00086) |   |   |   |     └─Token(Identifier) |myOutput|
//@[086:00087) |   |   |   ├─Token(Comma) |,|
//@[088:00136) |   |   |   ├─FunctionArgumentSyntax
//@[088:00136) |   |   |   | └─PropertyAccessSyntax
//@[088:00125) |   |   |   |   ├─PropertyAccessSyntax
//@[088:00114) |   |   |   |   | ├─ArrayAccessSyntax
//@[088:00103) |   |   |   |   | | ├─VariableAccessSyntax
//@[088:00103) |   |   |   |   | | | └─IdentifierSyntax
//@[088:00103) |   |   |   |   | | |   └─Token(Identifier) |storageAccounts|
//@[103:00104) |   |   |   |   | | ├─Token(LeftSquare) |[|
//@[104:00113) |   |   |   |   | | ├─BinaryOperationSyntax
//@[104:00109) |   |   |   |   | | | ├─VariableAccessSyntax
//@[104:00109) |   |   |   |   | | | | └─IdentifierSyntax
//@[104:00109) |   |   |   |   | | | |   └─Token(Identifier) |index|
//@[110:00111) |   |   |   |   | | | ├─Token(Asterisk) |*|
//@[112:00113) |   |   |   |   | | | └─IntegerLiteralSyntax
//@[112:00113) |   |   |   |   | | |   └─Token(Integer) |3|
//@[113:00114) |   |   |   |   | | └─Token(RightSquare) |]|
//@[114:00115) |   |   |   |   | ├─Token(Dot) |.|
//@[115:00125) |   |   |   |   | └─IdentifierSyntax
//@[115:00125) |   |   |   |   |   └─Token(Identifier) |properties|
//@[125:00126) |   |   |   |   ├─Token(Dot) |.|
//@[126:00136) |   |   |   |   └─IdentifierSyntax
//@[126:00136) |   |   |   |     └─Token(Identifier) |accessTier|
//@[136:00137) |   |   |   └─Token(RightParen) |)|
//@[137:00138) |   |   ├─Token(NewLine) |\n|
  }
//@[002:00003) |   |   └─Token(RightBrace) |}|
//@[003:00004) |   ├─Token(NewLine) |\n|
  dependsOn: [
//@[002:00051) |   ├─ObjectPropertySyntax
//@[002:00011) |   | ├─IdentifierSyntax
//@[002:00011) |   | | └─Token(Identifier) |dependsOn|
//@[011:00012) |   | ├─Token(Colon) |:|
//@[013:00051) |   | └─ArraySyntax
//@[013:00014) |   |   ├─Token(LeftSquare) |[|
//@[014:00015) |   |   ├─Token(NewLine) |\n|
    storageAccounts2[index - 10]
//@[004:00032) |   |   ├─ArrayItemSyntax
//@[004:00032) |   |   | └─ArrayAccessSyntax
//@[004:00020) |   |   |   ├─VariableAccessSyntax
//@[004:00020) |   |   |   | └─IdentifierSyntax
//@[004:00020) |   |   |   |   └─Token(Identifier) |storageAccounts2|
//@[020:00021) |   |   |   ├─Token(LeftSquare) |[|
//@[021:00031) |   |   |   ├─BinaryOperationSyntax
//@[021:00026) |   |   |   | ├─VariableAccessSyntax
//@[021:00026) |   |   |   | | └─IdentifierSyntax
//@[021:00026) |   |   |   | |   └─Token(Identifier) |index|
//@[027:00028) |   |   |   | ├─Token(Minus) |-|
//@[029:00031) |   |   |   | └─IntegerLiteralSyntax
//@[029:00031) |   |   |   |   └─Token(Integer) |10|
//@[031:00032) |   |   |   └─Token(RightSquare) |]|
//@[032:00033) |   |   ├─Token(NewLine) |\n|
  ]
//@[002:00003) |   |   └─Token(RightSquare) |]|
//@[003:00004) |   ├─Token(NewLine) |\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00003) ├─Token(NewLine) |\n\n|

module moduleCollectionWithIndexedDependencies 'passthrough.bicep' = [for (moduleName, moduleIndex) in moduleSetup: {
//@[000:00399) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00046) | ├─IdentifierSyntax
//@[007:00046) | | └─Token(Identifier) |moduleCollectionWithIndexedDependencies|
//@[047:00066) | ├─StringSyntax
//@[047:00066) | | └─Token(StringComplete) |'passthrough.bicep'|
//@[067:00068) | ├─Token(Assignment) |=|
//@[069:00399) | └─ForSyntax
//@[069:00070) |   ├─Token(LeftSquare) |[|
//@[070:00073) |   ├─Token(Identifier) |for|
//@[074:00099) |   ├─VariableBlockSyntax
//@[074:00075) |   | ├─Token(LeftParen) |(|
//@[075:00085) |   | ├─LocalVariableSyntax
//@[075:00085) |   | | └─IdentifierSyntax
//@[075:00085) |   | |   └─Token(Identifier) |moduleName|
//@[085:00086) |   | ├─Token(Comma) |,|
//@[087:00098) |   | ├─LocalVariableSyntax
//@[087:00098) |   | | └─IdentifierSyntax
//@[087:00098) |   | |   └─Token(Identifier) |moduleIndex|
//@[098:00099) |   | └─Token(RightParen) |)|
//@[100:00102) |   ├─Token(Identifier) |in|
//@[103:00114) |   ├─VariableAccessSyntax
//@[103:00114) |   | └─IdentifierSyntax
//@[103:00114) |   |   └─Token(Identifier) |moduleSetup|
//@[114:00115) |   ├─Token(Colon) |:|
//@[116:00398) |   ├─ObjectSyntax
//@[116:00117) |   | ├─Token(LeftBrace) |{|
//@[117:00118) |   | ├─Token(NewLine) |\n|
  name: concat(moduleName, moduleIndex)
//@[002:00039) |   | ├─ObjectPropertySyntax
//@[002:00006) |   | | ├─IdentifierSyntax
//@[002:00006) |   | | | └─Token(Identifier) |name|
//@[006:00007) |   | | ├─Token(Colon) |:|
//@[008:00039) |   | | └─FunctionCallSyntax
//@[008:00014) |   | |   ├─IdentifierSyntax
//@[008:00014) |   | |   | └─Token(Identifier) |concat|
//@[014:00015) |   | |   ├─Token(LeftParen) |(|
//@[015:00025) |   | |   ├─FunctionArgumentSyntax
//@[015:00025) |   | |   | └─VariableAccessSyntax
//@[015:00025) |   | |   |   └─IdentifierSyntax
//@[015:00025) |   | |   |     └─Token(Identifier) |moduleName|
//@[025:00026) |   | |   ├─Token(Comma) |,|
//@[027:00038) |   | |   ├─FunctionArgumentSyntax
//@[027:00038) |   | |   | └─VariableAccessSyntax
//@[027:00038) |   | |   |   └─IdentifierSyntax
//@[027:00038) |   | |   |     └─Token(Identifier) |moduleIndex|
//@[038:00039) |   | |   └─Token(RightParen) |)|
//@[039:00040) |   | ├─Token(NewLine) |\n|
  params: {
//@[002:00187) |   | ├─ObjectPropertySyntax
//@[002:00008) |   | | ├─IdentifierSyntax
//@[002:00008) |   | | | └─Token(Identifier) |params|
//@[008:00009) |   | | ├─Token(Colon) |:|
//@[010:00187) |   | | └─ObjectSyntax
//@[010:00011) |   | |   ├─Token(LeftBrace) |{|
//@[011:00012) |   | |   ├─Token(NewLine) |\n|
    myInput: '${moduleCollectionWithCollectionDependencies[index].outputs.myOutput} - ${storageAccounts[index * 3].properties.accessTier} - ${moduleName} - ${moduleIndex}'
//@[004:00171) |   | |   ├─ObjectPropertySyntax
//@[004:00011) |   | |   | ├─IdentifierSyntax
//@[004:00011) |   | |   | | └─Token(Identifier) |myInput|
//@[011:00012) |   | |   | ├─Token(Colon) |:|
//@[013:00171) |   | |   | └─StringSyntax
//@[013:00016) |   | |   |   ├─Token(StringLeftPiece) |'${|
//@[016:00082) |   | |   |   ├─PropertyAccessSyntax
//@[016:00073) |   | |   |   | ├─PropertyAccessSyntax
//@[016:00065) |   | |   |   | | ├─ArrayAccessSyntax
//@[016:00058) |   | |   |   | | | ├─VariableAccessSyntax
//@[016:00058) |   | |   |   | | | | └─IdentifierSyntax
//@[016:00058) |   | |   |   | | | |   └─Token(Identifier) |moduleCollectionWithCollectionDependencies|
//@[058:00059) |   | |   |   | | | ├─Token(LeftSquare) |[|
//@[059:00064) |   | |   |   | | | ├─VariableAccessSyntax
//@[059:00064) |   | |   |   | | | | └─IdentifierSyntax
//@[059:00064) |   | |   |   | | | |   └─Token(Identifier) |index|
//@[064:00065) |   | |   |   | | | └─Token(RightSquare) |]|
//@[065:00066) |   | |   |   | | ├─Token(Dot) |.|
//@[066:00073) |   | |   |   | | └─IdentifierSyntax
//@[066:00073) |   | |   |   | |   └─Token(Identifier) |outputs|
//@[073:00074) |   | |   |   | ├─Token(Dot) |.|
//@[074:00082) |   | |   |   | └─IdentifierSyntax
//@[074:00082) |   | |   |   |   └─Token(Identifier) |myOutput|
//@[082:00088) |   | |   |   ├─Token(StringMiddlePiece) |} - ${|
//@[088:00136) |   | |   |   ├─PropertyAccessSyntax
//@[088:00125) |   | |   |   | ├─PropertyAccessSyntax
//@[088:00114) |   | |   |   | | ├─ArrayAccessSyntax
//@[088:00103) |   | |   |   | | | ├─VariableAccessSyntax
//@[088:00103) |   | |   |   | | | | └─IdentifierSyntax
//@[088:00103) |   | |   |   | | | |   └─Token(Identifier) |storageAccounts|
//@[103:00104) |   | |   |   | | | ├─Token(LeftSquare) |[|
//@[104:00113) |   | |   |   | | | ├─BinaryOperationSyntax
//@[104:00109) |   | |   |   | | | | ├─VariableAccessSyntax
//@[104:00109) |   | |   |   | | | | | └─IdentifierSyntax
//@[104:00109) |   | |   |   | | | | |   └─Token(Identifier) |index|
//@[110:00111) |   | |   |   | | | | ├─Token(Asterisk) |*|
//@[112:00113) |   | |   |   | | | | └─IntegerLiteralSyntax
//@[112:00113) |   | |   |   | | | |   └─Token(Integer) |3|
//@[113:00114) |   | |   |   | | | └─Token(RightSquare) |]|
//@[114:00115) |   | |   |   | | ├─Token(Dot) |.|
//@[115:00125) |   | |   |   | | └─IdentifierSyntax
//@[115:00125) |   | |   |   | |   └─Token(Identifier) |properties|
//@[125:00126) |   | |   |   | ├─Token(Dot) |.|
//@[126:00136) |   | |   |   | └─IdentifierSyntax
//@[126:00136) |   | |   |   |   └─Token(Identifier) |accessTier|
//@[136:00142) |   | |   |   ├─Token(StringMiddlePiece) |} - ${|
//@[142:00152) |   | |   |   ├─VariableAccessSyntax
//@[142:00152) |   | |   |   | └─IdentifierSyntax
//@[142:00152) |   | |   |   |   └─Token(Identifier) |moduleName|
//@[152:00158) |   | |   |   ├─Token(StringMiddlePiece) |} - ${|
//@[158:00169) |   | |   |   ├─VariableAccessSyntax
//@[158:00169) |   | |   |   | └─IdentifierSyntax
//@[158:00169) |   | |   |   |   └─Token(Identifier) |moduleIndex|
//@[169:00171) |   | |   |   └─Token(StringRightPiece) |}'|
//@[171:00172) |   | |   ├─Token(NewLine) |\n|
  }
//@[002:00003) |   | |   └─Token(RightBrace) |}|
//@[003:00004) |   | ├─Token(NewLine) |\n|
  dependsOn: [
//@[002:00050) |   | ├─ObjectPropertySyntax
//@[002:00011) |   | | ├─IdentifierSyntax
//@[002:00011) |   | | | └─Token(Identifier) |dependsOn|
//@[011:00012) |   | | ├─Token(Colon) |:|
//@[013:00050) |   | | └─ArraySyntax
//@[013:00014) |   | |   ├─Token(LeftSquare) |[|
//@[014:00015) |   | |   ├─Token(NewLine) |\n|
    storageAccounts2[index - 9]
//@[004:00031) |   | |   ├─ArrayItemSyntax
//@[004:00031) |   | |   | └─ArrayAccessSyntax
//@[004:00020) |   | |   |   ├─VariableAccessSyntax
//@[004:00020) |   | |   |   | └─IdentifierSyntax
//@[004:00020) |   | |   |   |   └─Token(Identifier) |storageAccounts2|
//@[020:00021) |   | |   |   ├─Token(LeftSquare) |[|
//@[021:00030) |   | |   |   ├─BinaryOperationSyntax
//@[021:00026) |   | |   |   | ├─VariableAccessSyntax
//@[021:00026) |   | |   |   | | └─IdentifierSyntax
//@[021:00026) |   | |   |   | |   └─Token(Identifier) |index|
//@[027:00028) |   | |   |   | ├─Token(Minus) |-|
//@[029:00030) |   | |   |   | └─IntegerLiteralSyntax
//@[029:00030) |   | |   |   |   └─Token(Integer) |9|
//@[030:00031) |   | |   |   └─Token(RightSquare) |]|
//@[031:00032) |   | |   ├─Token(NewLine) |\n|
  ]
//@[002:00003) |   | |   └─Token(RightSquare) |]|
//@[003:00004) |   | ├─Token(NewLine) |\n|
}]
//@[000:00001) |   | └─Token(RightBrace) |}|
//@[001:00002) |   └─Token(RightSquare) |]|
//@[002:00004) ├─Token(NewLine) |\n\n|

output indexedModulesName string = moduleCollectionWithSingleDependency[index].name
//@[000:00083) ├─OutputDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |output|
//@[007:00025) | ├─IdentifierSyntax
//@[007:00025) | | └─Token(Identifier) |indexedModulesName|
//@[026:00032) | ├─SimpleTypeSyntax
//@[026:00032) | | └─Token(Identifier) |string|
//@[033:00034) | ├─Token(Assignment) |=|
//@[035:00083) | └─PropertyAccessSyntax
//@[035:00078) |   ├─ArrayAccessSyntax
//@[035:00071) |   | ├─VariableAccessSyntax
//@[035:00071) |   | | └─IdentifierSyntax
//@[035:00071) |   | |   └─Token(Identifier) |moduleCollectionWithSingleDependency|
//@[071:00072) |   | ├─Token(LeftSquare) |[|
//@[072:00077) |   | ├─VariableAccessSyntax
//@[072:00077) |   | | └─IdentifierSyntax
//@[072:00077) |   | |   └─Token(Identifier) |index|
//@[077:00078) |   | └─Token(RightSquare) |]|
//@[078:00079) |   ├─Token(Dot) |.|
//@[079:00083) |   └─IdentifierSyntax
//@[079:00083) |     └─Token(Identifier) |name|
//@[083:00084) ├─Token(NewLine) |\n|
output indexedModuleOutput string = moduleCollectionWithSingleDependency[index * 1].outputs.myOutput
//@[000:00100) ├─OutputDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |output|
//@[007:00026) | ├─IdentifierSyntax
//@[007:00026) | | └─Token(Identifier) |indexedModuleOutput|
//@[027:00033) | ├─SimpleTypeSyntax
//@[027:00033) | | └─Token(Identifier) |string|
//@[034:00035) | ├─Token(Assignment) |=|
//@[036:00100) | └─PropertyAccessSyntax
//@[036:00091) |   ├─PropertyAccessSyntax
//@[036:00083) |   | ├─ArrayAccessSyntax
//@[036:00072) |   | | ├─VariableAccessSyntax
//@[036:00072) |   | | | └─IdentifierSyntax
//@[036:00072) |   | | |   └─Token(Identifier) |moduleCollectionWithSingleDependency|
//@[072:00073) |   | | ├─Token(LeftSquare) |[|
//@[073:00082) |   | | ├─BinaryOperationSyntax
//@[073:00078) |   | | | ├─VariableAccessSyntax
//@[073:00078) |   | | | | └─IdentifierSyntax
//@[073:00078) |   | | | |   └─Token(Identifier) |index|
//@[079:00080) |   | | | ├─Token(Asterisk) |*|
//@[081:00082) |   | | | └─IntegerLiteralSyntax
//@[081:00082) |   | | |   └─Token(Integer) |1|
//@[082:00083) |   | | └─Token(RightSquare) |]|
//@[083:00084) |   | ├─Token(Dot) |.|
//@[084:00091) |   | └─IdentifierSyntax
//@[084:00091) |   |   └─Token(Identifier) |outputs|
//@[091:00092) |   ├─Token(Dot) |.|
//@[092:00100) |   └─IdentifierSyntax
//@[092:00100) |     └─Token(Identifier) |myOutput|
//@[100:00102) ├─Token(NewLine) |\n\n|

// resource collection
//@[022:00023) ├─Token(NewLine) |\n|
resource existingStorageAccounts 'Microsoft.Storage/storageAccounts@2019-06-01' existing = [for (account, i) in accounts: {
//@[000:00174) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00032) | ├─IdentifierSyntax
//@[009:00032) | | └─Token(Identifier) |existingStorageAccounts|
//@[033:00079) | ├─StringSyntax
//@[033:00079) | | └─Token(StringComplete) |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[080:00088) | ├─Token(Identifier) |existing|
//@[089:00090) | ├─Token(Assignment) |=|
//@[091:00174) | └─ForSyntax
//@[091:00092) |   ├─Token(LeftSquare) |[|
//@[092:00095) |   ├─Token(Identifier) |for|
//@[096:00108) |   ├─VariableBlockSyntax
//@[096:00097) |   | ├─Token(LeftParen) |(|
//@[097:00104) |   | ├─LocalVariableSyntax
//@[097:00104) |   | | └─IdentifierSyntax
//@[097:00104) |   | |   └─Token(Identifier) |account|
//@[104:00105) |   | ├─Token(Comma) |,|
//@[106:00107) |   | ├─LocalVariableSyntax
//@[106:00107) |   | | └─IdentifierSyntax
//@[106:00107) |   | |   └─Token(Identifier) |i|
//@[107:00108) |   | └─Token(RightParen) |)|
//@[109:00111) |   ├─Token(Identifier) |in|
//@[112:00120) |   ├─VariableAccessSyntax
//@[112:00120) |   | └─IdentifierSyntax
//@[112:00120) |   |   └─Token(Identifier) |accounts|
//@[120:00121) |   ├─Token(Colon) |:|
//@[122:00173) |   ├─ObjectSyntax
//@[122:00123) |   | ├─Token(LeftBrace) |{|
//@[123:00124) |   | ├─Token(NewLine) |\n|
  name: '${name}-existing-${account.name}-${i}'
//@[002:00047) |   | ├─ObjectPropertySyntax
//@[002:00006) |   | | ├─IdentifierSyntax
//@[002:00006) |   | | | └─Token(Identifier) |name|
//@[006:00007) |   | | ├─Token(Colon) |:|
//@[008:00047) |   | | └─StringSyntax
//@[008:00011) |   | |   ├─Token(StringLeftPiece) |'${|
//@[011:00015) |   | |   ├─VariableAccessSyntax
//@[011:00015) |   | |   | └─IdentifierSyntax
//@[011:00015) |   | |   |   └─Token(Identifier) |name|
//@[015:00028) |   | |   ├─Token(StringMiddlePiece) |}-existing-${|
//@[028:00040) |   | |   ├─PropertyAccessSyntax
//@[028:00035) |   | |   | ├─VariableAccessSyntax
//@[028:00035) |   | |   | | └─IdentifierSyntax
//@[028:00035) |   | |   | |   └─Token(Identifier) |account|
//@[035:00036) |   | |   | ├─Token(Dot) |.|
//@[036:00040) |   | |   | └─IdentifierSyntax
//@[036:00040) |   | |   |   └─Token(Identifier) |name|
//@[040:00044) |   | |   ├─Token(StringMiddlePiece) |}-${|
//@[044:00045) |   | |   ├─VariableAccessSyntax
//@[044:00045) |   | |   | └─IdentifierSyntax
//@[044:00045) |   | |   |   └─Token(Identifier) |i|
//@[045:00047) |   | |   └─Token(StringRightPiece) |}'|
//@[047:00048) |   | ├─Token(NewLine) |\n|
}]
//@[000:00001) |   | └─Token(RightBrace) |}|
//@[001:00002) |   └─Token(RightSquare) |]|
//@[002:00004) ├─Token(NewLine) |\n\n|

output existingIndexedResourceName string = existingStorageAccounts[index * 0].name
//@[000:00083) ├─OutputDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |output|
//@[007:00034) | ├─IdentifierSyntax
//@[007:00034) | | └─Token(Identifier) |existingIndexedResourceName|
//@[035:00041) | ├─SimpleTypeSyntax
//@[035:00041) | | └─Token(Identifier) |string|
//@[042:00043) | ├─Token(Assignment) |=|
//@[044:00083) | └─PropertyAccessSyntax
//@[044:00078) |   ├─ArrayAccessSyntax
//@[044:00067) |   | ├─VariableAccessSyntax
//@[044:00067) |   | | └─IdentifierSyntax
//@[044:00067) |   | |   └─Token(Identifier) |existingStorageAccounts|
//@[067:00068) |   | ├─Token(LeftSquare) |[|
//@[068:00077) |   | ├─BinaryOperationSyntax
//@[068:00073) |   | | ├─VariableAccessSyntax
//@[068:00073) |   | | | └─IdentifierSyntax
//@[068:00073) |   | | |   └─Token(Identifier) |index|
//@[074:00075) |   | | ├─Token(Asterisk) |*|
//@[076:00077) |   | | └─IntegerLiteralSyntax
//@[076:00077) |   | |   └─Token(Integer) |0|
//@[077:00078) |   | └─Token(RightSquare) |]|
//@[078:00079) |   ├─Token(Dot) |.|
//@[079:00083) |   └─IdentifierSyntax
//@[079:00083) |     └─Token(Identifier) |name|
//@[083:00084) ├─Token(NewLine) |\n|
output existingIndexedResourceId string = existingStorageAccounts[index * 1].id
//@[000:00079) ├─OutputDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |output|
//@[007:00032) | ├─IdentifierSyntax
//@[007:00032) | | └─Token(Identifier) |existingIndexedResourceId|
//@[033:00039) | ├─SimpleTypeSyntax
//@[033:00039) | | └─Token(Identifier) |string|
//@[040:00041) | ├─Token(Assignment) |=|
//@[042:00079) | └─PropertyAccessSyntax
//@[042:00076) |   ├─ArrayAccessSyntax
//@[042:00065) |   | ├─VariableAccessSyntax
//@[042:00065) |   | | └─IdentifierSyntax
//@[042:00065) |   | |   └─Token(Identifier) |existingStorageAccounts|
//@[065:00066) |   | ├─Token(LeftSquare) |[|
//@[066:00075) |   | ├─BinaryOperationSyntax
//@[066:00071) |   | | ├─VariableAccessSyntax
//@[066:00071) |   | | | └─IdentifierSyntax
//@[066:00071) |   | | |   └─Token(Identifier) |index|
//@[072:00073) |   | | ├─Token(Asterisk) |*|
//@[074:00075) |   | | └─IntegerLiteralSyntax
//@[074:00075) |   | |   └─Token(Integer) |1|
//@[075:00076) |   | └─Token(RightSquare) |]|
//@[076:00077) |   ├─Token(Dot) |.|
//@[077:00079) |   └─IdentifierSyntax
//@[077:00079) |     └─Token(Identifier) |id|
//@[079:00080) ├─Token(NewLine) |\n|
output existingIndexedResourceType string = existingStorageAccounts[index+2].type
//@[000:00081) ├─OutputDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |output|
//@[007:00034) | ├─IdentifierSyntax
//@[007:00034) | | └─Token(Identifier) |existingIndexedResourceType|
//@[035:00041) | ├─SimpleTypeSyntax
//@[035:00041) | | └─Token(Identifier) |string|
//@[042:00043) | ├─Token(Assignment) |=|
//@[044:00081) | └─PropertyAccessSyntax
//@[044:00076) |   ├─ArrayAccessSyntax
//@[044:00067) |   | ├─VariableAccessSyntax
//@[044:00067) |   | | └─IdentifierSyntax
//@[044:00067) |   | |   └─Token(Identifier) |existingStorageAccounts|
//@[067:00068) |   | ├─Token(LeftSquare) |[|
//@[068:00075) |   | ├─BinaryOperationSyntax
//@[068:00073) |   | | ├─VariableAccessSyntax
//@[068:00073) |   | | | └─IdentifierSyntax
//@[068:00073) |   | | |   └─Token(Identifier) |index|
//@[073:00074) |   | | ├─Token(Plus) |+|
//@[074:00075) |   | | └─IntegerLiteralSyntax
//@[074:00075) |   | |   └─Token(Integer) |2|
//@[075:00076) |   | └─Token(RightSquare) |]|
//@[076:00077) |   ├─Token(Dot) |.|
//@[077:00081) |   └─IdentifierSyntax
//@[077:00081) |     └─Token(Identifier) |type|
//@[081:00082) ├─Token(NewLine) |\n|
output existingIndexedResourceApiVersion string = existingStorageAccounts[index-7].apiVersion
//@[000:00093) ├─OutputDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |output|
//@[007:00040) | ├─IdentifierSyntax
//@[007:00040) | | └─Token(Identifier) |existingIndexedResourceApiVersion|
//@[041:00047) | ├─SimpleTypeSyntax
//@[041:00047) | | └─Token(Identifier) |string|
//@[048:00049) | ├─Token(Assignment) |=|
//@[050:00093) | └─PropertyAccessSyntax
//@[050:00082) |   ├─ArrayAccessSyntax
//@[050:00073) |   | ├─VariableAccessSyntax
//@[050:00073) |   | | └─IdentifierSyntax
//@[050:00073) |   | |   └─Token(Identifier) |existingStorageAccounts|
//@[073:00074) |   | ├─Token(LeftSquare) |[|
//@[074:00081) |   | ├─BinaryOperationSyntax
//@[074:00079) |   | | ├─VariableAccessSyntax
//@[074:00079) |   | | | └─IdentifierSyntax
//@[074:00079) |   | | |   └─Token(Identifier) |index|
//@[079:00080) |   | | ├─Token(Minus) |-|
//@[080:00081) |   | | └─IntegerLiteralSyntax
//@[080:00081) |   | |   └─Token(Integer) |7|
//@[081:00082) |   | └─Token(RightSquare) |]|
//@[082:00083) |   ├─Token(Dot) |.|
//@[083:00093) |   └─IdentifierSyntax
//@[083:00093) |     └─Token(Identifier) |apiVersion|
//@[093:00094) ├─Token(NewLine) |\n|
output existingIndexedResourceLocation string = existingStorageAccounts[index/2].location
//@[000:00089) ├─OutputDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |output|
//@[007:00038) | ├─IdentifierSyntax
//@[007:00038) | | └─Token(Identifier) |existingIndexedResourceLocation|
//@[039:00045) | ├─SimpleTypeSyntax
//@[039:00045) | | └─Token(Identifier) |string|
//@[046:00047) | ├─Token(Assignment) |=|
//@[048:00089) | └─PropertyAccessSyntax
//@[048:00080) |   ├─ArrayAccessSyntax
//@[048:00071) |   | ├─VariableAccessSyntax
//@[048:00071) |   | | └─IdentifierSyntax
//@[048:00071) |   | |   └─Token(Identifier) |existingStorageAccounts|
//@[071:00072) |   | ├─Token(LeftSquare) |[|
//@[072:00079) |   | ├─BinaryOperationSyntax
//@[072:00077) |   | | ├─VariableAccessSyntax
//@[072:00077) |   | | | └─IdentifierSyntax
//@[072:00077) |   | | |   └─Token(Identifier) |index|
//@[077:00078) |   | | ├─Token(Slash) |/|
//@[078:00079) |   | | └─IntegerLiteralSyntax
//@[078:00079) |   | |   └─Token(Integer) |2|
//@[079:00080) |   | └─Token(RightSquare) |]|
//@[080:00081) |   ├─Token(Dot) |.|
//@[081:00089) |   └─IdentifierSyntax
//@[081:00089) |     └─Token(Identifier) |location|
//@[089:00090) ├─Token(NewLine) |\n|
output existingIndexedResourceAccessTier string = existingStorageAccounts[index%3].properties.accessTier
//@[000:00104) ├─OutputDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |output|
//@[007:00040) | ├─IdentifierSyntax
//@[007:00040) | | └─Token(Identifier) |existingIndexedResourceAccessTier|
//@[041:00047) | ├─SimpleTypeSyntax
//@[041:00047) | | └─Token(Identifier) |string|
//@[048:00049) | ├─Token(Assignment) |=|
//@[050:00104) | └─PropertyAccessSyntax
//@[050:00093) |   ├─PropertyAccessSyntax
//@[050:00082) |   | ├─ArrayAccessSyntax
//@[050:00073) |   | | ├─VariableAccessSyntax
//@[050:00073) |   | | | └─IdentifierSyntax
//@[050:00073) |   | | |   └─Token(Identifier) |existingStorageAccounts|
//@[073:00074) |   | | ├─Token(LeftSquare) |[|
//@[074:00081) |   | | ├─BinaryOperationSyntax
//@[074:00079) |   | | | ├─VariableAccessSyntax
//@[074:00079) |   | | | | └─IdentifierSyntax
//@[074:00079) |   | | | |   └─Token(Identifier) |index|
//@[079:00080) |   | | | ├─Token(Modulo) |%|
//@[080:00081) |   | | | └─IntegerLiteralSyntax
//@[080:00081) |   | | |   └─Token(Integer) |3|
//@[081:00082) |   | | └─Token(RightSquare) |]|
//@[082:00083) |   | ├─Token(Dot) |.|
//@[083:00093) |   | └─IdentifierSyntax
//@[083:00093) |   |   └─Token(Identifier) |properties|
//@[093:00094) |   ├─Token(Dot) |.|
//@[094:00104) |   └─IdentifierSyntax
//@[094:00104) |     └─Token(Identifier) |accessTier|
//@[104:00106) ├─Token(NewLine) |\n\n|

resource duplicatedNames 'Microsoft.Network/dnsZones@2018-05-01' = [for (zone,i) in []: {
//@[000:00140) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00024) | ├─IdentifierSyntax
//@[009:00024) | | └─Token(Identifier) |duplicatedNames|
//@[025:00064) | ├─StringSyntax
//@[025:00064) | | └─Token(StringComplete) |'Microsoft.Network/dnsZones@2018-05-01'|
//@[065:00066) | ├─Token(Assignment) |=|
//@[067:00140) | └─ForSyntax
//@[067:00068) |   ├─Token(LeftSquare) |[|
//@[068:00071) |   ├─Token(Identifier) |for|
//@[072:00080) |   ├─VariableBlockSyntax
//@[072:00073) |   | ├─Token(LeftParen) |(|
//@[073:00077) |   | ├─LocalVariableSyntax
//@[073:00077) |   | | └─IdentifierSyntax
//@[073:00077) |   | |   └─Token(Identifier) |zone|
//@[077:00078) |   | ├─Token(Comma) |,|
//@[078:00079) |   | ├─LocalVariableSyntax
//@[078:00079) |   | | └─IdentifierSyntax
//@[078:00079) |   | |   └─Token(Identifier) |i|
//@[079:00080) |   | └─Token(RightParen) |)|
//@[081:00083) |   ├─Token(Identifier) |in|
//@[084:00086) |   ├─ArraySyntax
//@[084:00085) |   | ├─Token(LeftSquare) |[|
//@[085:00086) |   | └─Token(RightSquare) |]|
//@[086:00087) |   ├─Token(Colon) |:|
//@[088:00139) |   ├─ObjectSyntax
//@[088:00089) |   | ├─Token(LeftBrace) |{|
//@[089:00090) |   | ├─Token(NewLine) |\n|
  name: 'no loop variable'
//@[002:00026) |   | ├─ObjectPropertySyntax
//@[002:00006) |   | | ├─IdentifierSyntax
//@[002:00006) |   | | | └─Token(Identifier) |name|
//@[006:00007) |   | | ├─Token(Colon) |:|
//@[008:00026) |   | | └─StringSyntax
//@[008:00026) |   | |   └─Token(StringComplete) |'no loop variable'|
//@[026:00027) |   | ├─Token(NewLine) |\n|
  location: 'eastus'
//@[002:00020) |   | ├─ObjectPropertySyntax
//@[002:00010) |   | | ├─IdentifierSyntax
//@[002:00010) |   | | | └─Token(Identifier) |location|
//@[010:00011) |   | | ├─Token(Colon) |:|
//@[012:00020) |   | | └─StringSyntax
//@[012:00020) |   | |   └─Token(StringComplete) |'eastus'|
//@[020:00021) |   | ├─Token(NewLine) |\n|
}]
//@[000:00001) |   | └─Token(RightBrace) |}|
//@[001:00002) |   └─Token(RightSquare) |]|
//@[002:00004) ├─Token(NewLine) |\n\n|

// reference to a resource collection whose name expression does not reference any loop variables
//@[097:00098) ├─Token(NewLine) |\n|
resource referenceToDuplicateNames 'Microsoft.Network/dnsZones@2018-05-01' = [for (zone,i) in []: {
//@[000:00198) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00034) | ├─IdentifierSyntax
//@[009:00034) | | └─Token(Identifier) |referenceToDuplicateNames|
//@[035:00074) | ├─StringSyntax
//@[035:00074) | | └─Token(StringComplete) |'Microsoft.Network/dnsZones@2018-05-01'|
//@[075:00076) | ├─Token(Assignment) |=|
//@[077:00198) | └─ForSyntax
//@[077:00078) |   ├─Token(LeftSquare) |[|
//@[078:00081) |   ├─Token(Identifier) |for|
//@[082:00090) |   ├─VariableBlockSyntax
//@[082:00083) |   | ├─Token(LeftParen) |(|
//@[083:00087) |   | ├─LocalVariableSyntax
//@[083:00087) |   | | └─IdentifierSyntax
//@[083:00087) |   | |   └─Token(Identifier) |zone|
//@[087:00088) |   | ├─Token(Comma) |,|
//@[088:00089) |   | ├─LocalVariableSyntax
//@[088:00089) |   | | └─IdentifierSyntax
//@[088:00089) |   | |   └─Token(Identifier) |i|
//@[089:00090) |   | └─Token(RightParen) |)|
//@[091:00093) |   ├─Token(Identifier) |in|
//@[094:00096) |   ├─ArraySyntax
//@[094:00095) |   | ├─Token(LeftSquare) |[|
//@[095:00096) |   | └─Token(RightSquare) |]|
//@[096:00097) |   ├─Token(Colon) |:|
//@[098:00197) |   ├─ObjectSyntax
//@[098:00099) |   | ├─Token(LeftBrace) |{|
//@[099:00100) |   | ├─Token(NewLine) |\n|
  name: 'no loop variable 2'
//@[002:00028) |   | ├─ObjectPropertySyntax
//@[002:00006) |   | | ├─IdentifierSyntax
//@[002:00006) |   | | | └─Token(Identifier) |name|
//@[006:00007) |   | | ├─Token(Colon) |:|
//@[008:00028) |   | | └─StringSyntax
//@[008:00028) |   | |   └─Token(StringComplete) |'no loop variable 2'|
//@[028:00029) |   | ├─Token(NewLine) |\n|
  location: 'eastus'
//@[002:00020) |   | ├─ObjectPropertySyntax
//@[002:00010) |   | | ├─IdentifierSyntax
//@[002:00010) |   | | | └─Token(Identifier) |location|
//@[010:00011) |   | | ├─Token(Colon) |:|
//@[012:00020) |   | | └─StringSyntax
//@[012:00020) |   | |   └─Token(StringComplete) |'eastus'|
//@[020:00021) |   | ├─Token(NewLine) |\n|
  dependsOn: [
//@[002:00045) |   | ├─ObjectPropertySyntax
//@[002:00011) |   | | ├─IdentifierSyntax
//@[002:00011) |   | | | └─Token(Identifier) |dependsOn|
//@[011:00012) |   | | ├─Token(Colon) |:|
//@[013:00045) |   | | └─ArraySyntax
//@[013:00014) |   | |   ├─Token(LeftSquare) |[|
//@[014:00015) |   | |   ├─Token(NewLine) |\n|
    duplicatedNames[index]
//@[004:00026) |   | |   ├─ArrayItemSyntax
//@[004:00026) |   | |   | └─ArrayAccessSyntax
//@[004:00019) |   | |   |   ├─VariableAccessSyntax
//@[004:00019) |   | |   |   | └─IdentifierSyntax
//@[004:00019) |   | |   |   |   └─Token(Identifier) |duplicatedNames|
//@[019:00020) |   | |   |   ├─Token(LeftSquare) |[|
//@[020:00025) |   | |   |   ├─VariableAccessSyntax
//@[020:00025) |   | |   |   | └─IdentifierSyntax
//@[020:00025) |   | |   |   |   └─Token(Identifier) |index|
//@[025:00026) |   | |   |   └─Token(RightSquare) |]|
//@[026:00027) |   | |   ├─Token(NewLine) |\n|
  ]
//@[002:00003) |   | |   └─Token(RightSquare) |]|
//@[003:00004) |   | ├─Token(NewLine) |\n|
}]
//@[000:00001) |   | └─Token(RightBrace) |}|
//@[001:00002) |   └─Token(RightSquare) |]|
//@[002:00004) ├─Token(NewLine) |\n\n|

var regions = [
//@[000:00039) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00011) | ├─IdentifierSyntax
//@[004:00011) | | └─Token(Identifier) |regions|
//@[012:00013) | ├─Token(Assignment) |=|
//@[014:00039) | └─ArraySyntax
//@[014:00015) |   ├─Token(LeftSquare) |[|
//@[015:00016) |   ├─Token(NewLine) |\n|
  'eastus'
//@[002:00010) |   ├─ArrayItemSyntax
//@[002:00010) |   | └─StringSyntax
//@[002:00010) |   |   └─Token(StringComplete) |'eastus'|
//@[010:00011) |   ├─Token(NewLine) |\n|
  'westus'
//@[002:00010) |   ├─ArrayItemSyntax
//@[002:00010) |   | └─StringSyntax
//@[002:00010) |   |   └─Token(StringComplete) |'westus'|
//@[010:00011) |   ├─Token(NewLine) |\n|
]
//@[000:00001) |   └─Token(RightSquare) |]|
//@[001:00003) ├─Token(NewLine) |\n\n|

module apim 'passthrough.bicep' = [for (region, i) in regions: {
//@[000:00141) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00011) | ├─IdentifierSyntax
//@[007:00011) | | └─Token(Identifier) |apim|
//@[012:00031) | ├─StringSyntax
//@[012:00031) | | └─Token(StringComplete) |'passthrough.bicep'|
//@[032:00033) | ├─Token(Assignment) |=|
//@[034:00141) | └─ForSyntax
//@[034:00035) |   ├─Token(LeftSquare) |[|
//@[035:00038) |   ├─Token(Identifier) |for|
//@[039:00050) |   ├─VariableBlockSyntax
//@[039:00040) |   | ├─Token(LeftParen) |(|
//@[040:00046) |   | ├─LocalVariableSyntax
//@[040:00046) |   | | └─IdentifierSyntax
//@[040:00046) |   | |   └─Token(Identifier) |region|
//@[046:00047) |   | ├─Token(Comma) |,|
//@[048:00049) |   | ├─LocalVariableSyntax
//@[048:00049) |   | | └─IdentifierSyntax
//@[048:00049) |   | |   └─Token(Identifier) |i|
//@[049:00050) |   | └─Token(RightParen) |)|
//@[051:00053) |   ├─Token(Identifier) |in|
//@[054:00061) |   ├─VariableAccessSyntax
//@[054:00061) |   | └─IdentifierSyntax
//@[054:00061) |   |   └─Token(Identifier) |regions|
//@[061:00062) |   ├─Token(Colon) |:|
//@[063:00140) |   ├─ObjectSyntax
//@[063:00064) |   | ├─Token(LeftBrace) |{|
//@[064:00065) |   | ├─Token(NewLine) |\n|
  name: 'apim-${region}-${name}-${i}'
//@[002:00037) |   | ├─ObjectPropertySyntax
//@[002:00006) |   | | ├─IdentifierSyntax
//@[002:00006) |   | | | └─Token(Identifier) |name|
//@[006:00007) |   | | ├─Token(Colon) |:|
//@[008:00037) |   | | └─StringSyntax
//@[008:00016) |   | |   ├─Token(StringLeftPiece) |'apim-${|
//@[016:00022) |   | |   ├─VariableAccessSyntax
//@[016:00022) |   | |   | └─IdentifierSyntax
//@[016:00022) |   | |   |   └─Token(Identifier) |region|
//@[022:00026) |   | |   ├─Token(StringMiddlePiece) |}-${|
//@[026:00030) |   | |   ├─VariableAccessSyntax
//@[026:00030) |   | |   | └─IdentifierSyntax
//@[026:00030) |   | |   |   └─Token(Identifier) |name|
//@[030:00034) |   | |   ├─Token(StringMiddlePiece) |}-${|
//@[034:00035) |   | |   ├─VariableAccessSyntax
//@[034:00035) |   | |   | └─IdentifierSyntax
//@[034:00035) |   | |   |   └─Token(Identifier) |i|
//@[035:00037) |   | |   └─Token(StringRightPiece) |}'|
//@[037:00038) |   | ├─Token(NewLine) |\n|
  params: {
//@[002:00035) |   | ├─ObjectPropertySyntax
//@[002:00008) |   | | ├─IdentifierSyntax
//@[002:00008) |   | | | └─Token(Identifier) |params|
//@[008:00009) |   | | ├─Token(Colon) |:|
//@[010:00035) |   | | └─ObjectSyntax
//@[010:00011) |   | |   ├─Token(LeftBrace) |{|
//@[011:00012) |   | |   ├─Token(NewLine) |\n|
    myInput: region
//@[004:00019) |   | |   ├─ObjectPropertySyntax
//@[004:00011) |   | |   | ├─IdentifierSyntax
//@[004:00011) |   | |   | | └─Token(Identifier) |myInput|
//@[011:00012) |   | |   | ├─Token(Colon) |:|
//@[013:00019) |   | |   | └─VariableAccessSyntax
//@[013:00019) |   | |   |   └─IdentifierSyntax
//@[013:00019) |   | |   |     └─Token(Identifier) |region|
//@[019:00020) |   | |   ├─Token(NewLine) |\n|
  }
//@[002:00003) |   | |   └─Token(RightBrace) |}|
//@[003:00004) |   | ├─Token(NewLine) |\n|
}]
//@[000:00001) |   | └─Token(RightBrace) |}|
//@[001:00002) |   └─Token(RightSquare) |]|
//@[002:00004) ├─Token(NewLine) |\n\n|

resource propertyLoopDependencyOnModuleCollection 'Microsoft.Network/frontDoors@2020-05-01' = {
//@[000:00792) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00049) | ├─IdentifierSyntax
//@[009:00049) | | └─Token(Identifier) |propertyLoopDependencyOnModuleCollection|
//@[050:00091) | ├─StringSyntax
//@[050:00091) | | └─Token(StringComplete) |'Microsoft.Network/frontDoors@2020-05-01'|
//@[092:00093) | ├─Token(Assignment) |=|
//@[094:00792) | └─ObjectSyntax
//@[094:00095) |   ├─Token(LeftBrace) |{|
//@[095:00096) |   ├─Token(NewLine) |\n|
  name: name
//@[002:00012) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00012) |   | └─VariableAccessSyntax
//@[008:00012) |   |   └─IdentifierSyntax
//@[008:00012) |   |     └─Token(Identifier) |name|
//@[012:00013) |   ├─Token(NewLine) |\n|
  location: 'Global'
//@[002:00020) |   ├─ObjectPropertySyntax
//@[002:00010) |   | ├─IdentifierSyntax
//@[002:00010) |   | | └─Token(Identifier) |location|
//@[010:00011) |   | ├─Token(Colon) |:|
//@[012:00020) |   | └─StringSyntax
//@[012:00020) |   |   └─Token(StringComplete) |'Global'|
//@[020:00021) |   ├─Token(NewLine) |\n|
  properties: {
//@[002:00660) |   ├─ObjectPropertySyntax
//@[002:00012) |   | ├─IdentifierSyntax
//@[002:00012) |   | | └─Token(Identifier) |properties|
//@[012:00013) |   | ├─Token(Colon) |:|
//@[014:00660) |   | └─ObjectSyntax
//@[014:00015) |   |   ├─Token(LeftBrace) |{|
//@[015:00016) |   |   ├─Token(NewLine) |\n|
    backendPools: [
//@[004:00640) |   |   ├─ObjectPropertySyntax
//@[004:00016) |   |   | ├─IdentifierSyntax
//@[004:00016) |   |   | | └─Token(Identifier) |backendPools|
//@[016:00017) |   |   | ├─Token(Colon) |:|
//@[018:00640) |   |   | └─ArraySyntax
//@[018:00019) |   |   |   ├─Token(LeftSquare) |[|
//@[019:00020) |   |   |   ├─Token(NewLine) |\n|
      {
//@[006:00614) |   |   |   ├─ArrayItemSyntax
//@[006:00614) |   |   |   | └─ObjectSyntax
//@[006:00007) |   |   |   |   ├─Token(LeftBrace) |{|
//@[007:00008) |   |   |   |   ├─Token(NewLine) |\n|
        name: 'BackendAPIMs'
//@[008:00028) |   |   |   |   ├─ObjectPropertySyntax
//@[008:00012) |   |   |   |   | ├─IdentifierSyntax
//@[008:00012) |   |   |   |   | | └─Token(Identifier) |name|
//@[012:00013) |   |   |   |   | ├─Token(Colon) |:|
//@[014:00028) |   |   |   |   | └─StringSyntax
//@[014:00028) |   |   |   |   |   └─Token(StringComplete) |'BackendAPIMs'|
//@[028:00029) |   |   |   |   ├─Token(NewLine) |\n|
        properties: {
//@[008:00569) |   |   |   |   ├─ObjectPropertySyntax
//@[008:00018) |   |   |   |   | ├─IdentifierSyntax
//@[008:00018) |   |   |   |   | | └─Token(Identifier) |properties|
//@[018:00019) |   |   |   |   | ├─Token(Colon) |:|
//@[020:00569) |   |   |   |   | └─ObjectSyntax
//@[020:00021) |   |   |   |   |   ├─Token(LeftBrace) |{|
//@[021:00022) |   |   |   |   |   ├─Token(NewLine) |\n|
          backends: [for (index,i) in range(0, length(regions)): {
//@[010:00537) |   |   |   |   |   ├─ObjectPropertySyntax
//@[010:00018) |   |   |   |   |   | ├─IdentifierSyntax
//@[010:00018) |   |   |   |   |   | | └─Token(Identifier) |backends|
//@[018:00019) |   |   |   |   |   | ├─Token(Colon) |:|
//@[020:00537) |   |   |   |   |   | └─ForSyntax
//@[020:00021) |   |   |   |   |   |   ├─Token(LeftSquare) |[|
//@[021:00024) |   |   |   |   |   |   ├─Token(Identifier) |for|
//@[025:00034) |   |   |   |   |   |   ├─VariableBlockSyntax
//@[025:00026) |   |   |   |   |   |   | ├─Token(LeftParen) |(|
//@[026:00031) |   |   |   |   |   |   | ├─LocalVariableSyntax
//@[026:00031) |   |   |   |   |   |   | | └─IdentifierSyntax
//@[026:00031) |   |   |   |   |   |   | |   └─Token(Identifier) |index|
//@[031:00032) |   |   |   |   |   |   | ├─Token(Comma) |,|
//@[032:00033) |   |   |   |   |   |   | ├─LocalVariableSyntax
//@[032:00033) |   |   |   |   |   |   | | └─IdentifierSyntax
//@[032:00033) |   |   |   |   |   |   | |   └─Token(Identifier) |i|
//@[033:00034) |   |   |   |   |   |   | └─Token(RightParen) |)|
//@[035:00037) |   |   |   |   |   |   ├─Token(Identifier) |in|
//@[038:00063) |   |   |   |   |   |   ├─FunctionCallSyntax
//@[038:00043) |   |   |   |   |   |   | ├─IdentifierSyntax
//@[038:00043) |   |   |   |   |   |   | | └─Token(Identifier) |range|
//@[043:00044) |   |   |   |   |   |   | ├─Token(LeftParen) |(|
//@[044:00045) |   |   |   |   |   |   | ├─FunctionArgumentSyntax
//@[044:00045) |   |   |   |   |   |   | | └─IntegerLiteralSyntax
//@[044:00045) |   |   |   |   |   |   | |   └─Token(Integer) |0|
//@[045:00046) |   |   |   |   |   |   | ├─Token(Comma) |,|
//@[047:00062) |   |   |   |   |   |   | ├─FunctionArgumentSyntax
//@[047:00062) |   |   |   |   |   |   | | └─FunctionCallSyntax
//@[047:00053) |   |   |   |   |   |   | |   ├─IdentifierSyntax
//@[047:00053) |   |   |   |   |   |   | |   | └─Token(Identifier) |length|
//@[053:00054) |   |   |   |   |   |   | |   ├─Token(LeftParen) |(|
//@[054:00061) |   |   |   |   |   |   | |   ├─FunctionArgumentSyntax
//@[054:00061) |   |   |   |   |   |   | |   | └─VariableAccessSyntax
//@[054:00061) |   |   |   |   |   |   | |   |   └─IdentifierSyntax
//@[054:00061) |   |   |   |   |   |   | |   |     └─Token(Identifier) |regions|
//@[061:00062) |   |   |   |   |   |   | |   └─Token(RightParen) |)|
//@[062:00063) |   |   |   |   |   |   | └─Token(RightParen) |)|
//@[063:00064) |   |   |   |   |   |   ├─Token(Colon) |:|
//@[065:00536) |   |   |   |   |   |   ├─ObjectSyntax
//@[065:00066) |   |   |   |   |   |   | ├─Token(LeftBrace) |{|
//@[066:00067) |   |   |   |   |   |   | ├─Token(NewLine) |\n|
            // we cannot codegen index correctly because the generated dependsOn property
//@[089:00090) |   |   |   |   |   |   | ├─Token(NewLine) |\n|
            // would be outside of the scope of the property loop
//@[065:00066) |   |   |   |   |   |   | ├─Token(NewLine) |\n|
            // as a result, this will generate a dependency on the entire collection
//@[084:00085) |   |   |   |   |   |   | ├─Token(NewLine) |\n|
            address: apim[index + i].outputs.myOutput
//@[012:00053) |   |   |   |   |   |   | ├─ObjectPropertySyntax
//@[012:00019) |   |   |   |   |   |   | | ├─IdentifierSyntax
//@[012:00019) |   |   |   |   |   |   | | | └─Token(Identifier) |address|
//@[019:00020) |   |   |   |   |   |   | | ├─Token(Colon) |:|
//@[021:00053) |   |   |   |   |   |   | | └─PropertyAccessSyntax
//@[021:00044) |   |   |   |   |   |   | |   ├─PropertyAccessSyntax
//@[021:00036) |   |   |   |   |   |   | |   | ├─ArrayAccessSyntax
//@[021:00025) |   |   |   |   |   |   | |   | | ├─VariableAccessSyntax
//@[021:00025) |   |   |   |   |   |   | |   | | | └─IdentifierSyntax
//@[021:00025) |   |   |   |   |   |   | |   | | |   └─Token(Identifier) |apim|
//@[025:00026) |   |   |   |   |   |   | |   | | ├─Token(LeftSquare) |[|
//@[026:00035) |   |   |   |   |   |   | |   | | ├─BinaryOperationSyntax
//@[026:00031) |   |   |   |   |   |   | |   | | | ├─VariableAccessSyntax
//@[026:00031) |   |   |   |   |   |   | |   | | | | └─IdentifierSyntax
//@[026:00031) |   |   |   |   |   |   | |   | | | |   └─Token(Identifier) |index|
//@[032:00033) |   |   |   |   |   |   | |   | | | ├─Token(Plus) |+|
//@[034:00035) |   |   |   |   |   |   | |   | | | └─VariableAccessSyntax
//@[034:00035) |   |   |   |   |   |   | |   | | |   └─IdentifierSyntax
//@[034:00035) |   |   |   |   |   |   | |   | | |     └─Token(Identifier) |i|
//@[035:00036) |   |   |   |   |   |   | |   | | └─Token(RightSquare) |]|
//@[036:00037) |   |   |   |   |   |   | |   | ├─Token(Dot) |.|
//@[037:00044) |   |   |   |   |   |   | |   | └─IdentifierSyntax
//@[037:00044) |   |   |   |   |   |   | |   |   └─Token(Identifier) |outputs|
//@[044:00045) |   |   |   |   |   |   | |   ├─Token(Dot) |.|
//@[045:00053) |   |   |   |   |   |   | |   └─IdentifierSyntax
//@[045:00053) |   |   |   |   |   |   | |     └─Token(Identifier) |myOutput|
//@[053:00054) |   |   |   |   |   |   | ├─Token(NewLine) |\n|
            backendHostHeader: apim[index + i].outputs.myOutput
//@[012:00063) |   |   |   |   |   |   | ├─ObjectPropertySyntax
//@[012:00029) |   |   |   |   |   |   | | ├─IdentifierSyntax
//@[012:00029) |   |   |   |   |   |   | | | └─Token(Identifier) |backendHostHeader|
//@[029:00030) |   |   |   |   |   |   | | ├─Token(Colon) |:|
//@[031:00063) |   |   |   |   |   |   | | └─PropertyAccessSyntax
//@[031:00054) |   |   |   |   |   |   | |   ├─PropertyAccessSyntax
//@[031:00046) |   |   |   |   |   |   | |   | ├─ArrayAccessSyntax
//@[031:00035) |   |   |   |   |   |   | |   | | ├─VariableAccessSyntax
//@[031:00035) |   |   |   |   |   |   | |   | | | └─IdentifierSyntax
//@[031:00035) |   |   |   |   |   |   | |   | | |   └─Token(Identifier) |apim|
//@[035:00036) |   |   |   |   |   |   | |   | | ├─Token(LeftSquare) |[|
//@[036:00045) |   |   |   |   |   |   | |   | | ├─BinaryOperationSyntax
//@[036:00041) |   |   |   |   |   |   | |   | | | ├─VariableAccessSyntax
//@[036:00041) |   |   |   |   |   |   | |   | | | | └─IdentifierSyntax
//@[036:00041) |   |   |   |   |   |   | |   | | | |   └─Token(Identifier) |index|
//@[042:00043) |   |   |   |   |   |   | |   | | | ├─Token(Plus) |+|
//@[044:00045) |   |   |   |   |   |   | |   | | | └─VariableAccessSyntax
//@[044:00045) |   |   |   |   |   |   | |   | | |   └─IdentifierSyntax
//@[044:00045) |   |   |   |   |   |   | |   | | |     └─Token(Identifier) |i|
//@[045:00046) |   |   |   |   |   |   | |   | | └─Token(RightSquare) |]|
//@[046:00047) |   |   |   |   |   |   | |   | ├─Token(Dot) |.|
//@[047:00054) |   |   |   |   |   |   | |   | └─IdentifierSyntax
//@[047:00054) |   |   |   |   |   |   | |   |   └─Token(Identifier) |outputs|
//@[054:00055) |   |   |   |   |   |   | |   ├─Token(Dot) |.|
//@[055:00063) |   |   |   |   |   |   | |   └─IdentifierSyntax
//@[055:00063) |   |   |   |   |   |   | |     └─Token(Identifier) |myOutput|
//@[063:00064) |   |   |   |   |   |   | ├─Token(NewLine) |\n|
            httpPort: 80
//@[012:00024) |   |   |   |   |   |   | ├─ObjectPropertySyntax
//@[012:00020) |   |   |   |   |   |   | | ├─IdentifierSyntax
//@[012:00020) |   |   |   |   |   |   | | | └─Token(Identifier) |httpPort|
//@[020:00021) |   |   |   |   |   |   | | ├─Token(Colon) |:|
//@[022:00024) |   |   |   |   |   |   | | └─IntegerLiteralSyntax
//@[022:00024) |   |   |   |   |   |   | |   └─Token(Integer) |80|
//@[024:00025) |   |   |   |   |   |   | ├─Token(NewLine) |\n|
            httpsPort: 443
//@[012:00026) |   |   |   |   |   |   | ├─ObjectPropertySyntax
//@[012:00021) |   |   |   |   |   |   | | ├─IdentifierSyntax
//@[012:00021) |   |   |   |   |   |   | | | └─Token(Identifier) |httpsPort|
//@[021:00022) |   |   |   |   |   |   | | ├─Token(Colon) |:|
//@[023:00026) |   |   |   |   |   |   | | └─IntegerLiteralSyntax
//@[023:00026) |   |   |   |   |   |   | |   └─Token(Integer) |443|
//@[026:00027) |   |   |   |   |   |   | ├─Token(NewLine) |\n|
            priority: 1
//@[012:00023) |   |   |   |   |   |   | ├─ObjectPropertySyntax
//@[012:00020) |   |   |   |   |   |   | | ├─IdentifierSyntax
//@[012:00020) |   |   |   |   |   |   | | | └─Token(Identifier) |priority|
//@[020:00021) |   |   |   |   |   |   | | ├─Token(Colon) |:|
//@[022:00023) |   |   |   |   |   |   | | └─IntegerLiteralSyntax
//@[022:00023) |   |   |   |   |   |   | |   └─Token(Integer) |1|
//@[023:00024) |   |   |   |   |   |   | ├─Token(NewLine) |\n|
            weight: 50
//@[012:00022) |   |   |   |   |   |   | ├─ObjectPropertySyntax
//@[012:00018) |   |   |   |   |   |   | | ├─IdentifierSyntax
//@[012:00018) |   |   |   |   |   |   | | | └─Token(Identifier) |weight|
//@[018:00019) |   |   |   |   |   |   | | ├─Token(Colon) |:|
//@[020:00022) |   |   |   |   |   |   | | └─IntegerLiteralSyntax
//@[020:00022) |   |   |   |   |   |   | |   └─Token(Integer) |50|
//@[022:00023) |   |   |   |   |   |   | ├─Token(NewLine) |\n|
          }]
//@[010:00011) |   |   |   |   |   |   | └─Token(RightBrace) |}|
//@[011:00012) |   |   |   |   |   |   └─Token(RightSquare) |]|
//@[012:00013) |   |   |   |   |   ├─Token(NewLine) |\n|
        }
//@[008:00009) |   |   |   |   |   └─Token(RightBrace) |}|
//@[009:00010) |   |   |   |   ├─Token(NewLine) |\n|
      }
//@[006:00007) |   |   |   |   └─Token(RightBrace) |}|
//@[007:00008) |   |   |   ├─Token(NewLine) |\n|
    ]
//@[004:00005) |   |   |   └─Token(RightSquare) |]|
//@[005:00006) |   |   ├─Token(NewLine) |\n|
  }
//@[002:00003) |   |   └─Token(RightBrace) |}|
//@[003:00004) |   ├─Token(NewLine) |\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00003) ├─Token(NewLine) |\n\n|

resource indexedModuleCollectionDependency 'Microsoft.Network/frontDoors@2020-05-01' = [for (index, i) in range(0, length(regions)): {
//@[000:00771) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00042) | ├─IdentifierSyntax
//@[009:00042) | | └─Token(Identifier) |indexedModuleCollectionDependency|
//@[043:00084) | ├─StringSyntax
//@[043:00084) | | └─Token(StringComplete) |'Microsoft.Network/frontDoors@2020-05-01'|
//@[085:00086) | ├─Token(Assignment) |=|
//@[087:00771) | └─ForSyntax
//@[087:00088) |   ├─Token(LeftSquare) |[|
//@[088:00091) |   ├─Token(Identifier) |for|
//@[092:00102) |   ├─VariableBlockSyntax
//@[092:00093) |   | ├─Token(LeftParen) |(|
//@[093:00098) |   | ├─LocalVariableSyntax
//@[093:00098) |   | | └─IdentifierSyntax
//@[093:00098) |   | |   └─Token(Identifier) |index|
//@[098:00099) |   | ├─Token(Comma) |,|
//@[100:00101) |   | ├─LocalVariableSyntax
//@[100:00101) |   | | └─IdentifierSyntax
//@[100:00101) |   | |   └─Token(Identifier) |i|
//@[101:00102) |   | └─Token(RightParen) |)|
//@[103:00105) |   ├─Token(Identifier) |in|
//@[106:00131) |   ├─FunctionCallSyntax
//@[106:00111) |   | ├─IdentifierSyntax
//@[106:00111) |   | | └─Token(Identifier) |range|
//@[111:00112) |   | ├─Token(LeftParen) |(|
//@[112:00113) |   | ├─FunctionArgumentSyntax
//@[112:00113) |   | | └─IntegerLiteralSyntax
//@[112:00113) |   | |   └─Token(Integer) |0|
//@[113:00114) |   | ├─Token(Comma) |,|
//@[115:00130) |   | ├─FunctionArgumentSyntax
//@[115:00130) |   | | └─FunctionCallSyntax
//@[115:00121) |   | |   ├─IdentifierSyntax
//@[115:00121) |   | |   | └─Token(Identifier) |length|
//@[121:00122) |   | |   ├─Token(LeftParen) |(|
//@[122:00129) |   | |   ├─FunctionArgumentSyntax
//@[122:00129) |   | |   | └─VariableAccessSyntax
//@[122:00129) |   | |   |   └─IdentifierSyntax
//@[122:00129) |   | |   |     └─Token(Identifier) |regions|
//@[129:00130) |   | |   └─Token(RightParen) |)|
//@[130:00131) |   | └─Token(RightParen) |)|
//@[131:00132) |   ├─Token(Colon) |:|
//@[133:00770) |   ├─ObjectSyntax
//@[133:00134) |   | ├─Token(LeftBrace) |{|
//@[134:00135) |   | ├─Token(NewLine) |\n|
  name: '${name}-${index}-${i}'
//@[002:00031) |   | ├─ObjectPropertySyntax
//@[002:00006) |   | | ├─IdentifierSyntax
//@[002:00006) |   | | | └─Token(Identifier) |name|
//@[006:00007) |   | | ├─Token(Colon) |:|
//@[008:00031) |   | | └─StringSyntax
//@[008:00011) |   | |   ├─Token(StringLeftPiece) |'${|
//@[011:00015) |   | |   ├─VariableAccessSyntax
//@[011:00015) |   | |   | └─IdentifierSyntax
//@[011:00015) |   | |   |   └─Token(Identifier) |name|
//@[015:00019) |   | |   ├─Token(StringMiddlePiece) |}-${|
//@[019:00024) |   | |   ├─VariableAccessSyntax
//@[019:00024) |   | |   | └─IdentifierSyntax
//@[019:00024) |   | |   |   └─Token(Identifier) |index|
//@[024:00028) |   | |   ├─Token(StringMiddlePiece) |}-${|
//@[028:00029) |   | |   ├─VariableAccessSyntax
//@[028:00029) |   | |   | └─IdentifierSyntax
//@[028:00029) |   | |   |   └─Token(Identifier) |i|
//@[029:00031) |   | |   └─Token(StringRightPiece) |}'|
//@[031:00032) |   | ├─Token(NewLine) |\n|
  location: 'Global'
//@[002:00020) |   | ├─ObjectPropertySyntax
//@[002:00010) |   | | ├─IdentifierSyntax
//@[002:00010) |   | | | └─Token(Identifier) |location|
//@[010:00011) |   | | ├─Token(Colon) |:|
//@[012:00020) |   | | └─StringSyntax
//@[012:00020) |   | |   └─Token(StringComplete) |'Global'|
//@[020:00021) |   | ├─Token(NewLine) |\n|
  properties: {
//@[002:00580) |   | ├─ObjectPropertySyntax
//@[002:00012) |   | | ├─IdentifierSyntax
//@[002:00012) |   | | | └─Token(Identifier) |properties|
//@[012:00013) |   | | ├─Token(Colon) |:|
//@[014:00580) |   | | └─ObjectSyntax
//@[014:00015) |   | |   ├─Token(LeftBrace) |{|
//@[015:00016) |   | |   ├─Token(NewLine) |\n|
    backendPools: [
//@[004:00560) |   | |   ├─ObjectPropertySyntax
//@[004:00016) |   | |   | ├─IdentifierSyntax
//@[004:00016) |   | |   | | └─Token(Identifier) |backendPools|
//@[016:00017) |   | |   | ├─Token(Colon) |:|
//@[018:00560) |   | |   | └─ArraySyntax
//@[018:00019) |   | |   |   ├─Token(LeftSquare) |[|
//@[019:00020) |   | |   |   ├─Token(NewLine) |\n|
      {
//@[006:00534) |   | |   |   ├─ArrayItemSyntax
//@[006:00534) |   | |   |   | └─ObjectSyntax
//@[006:00007) |   | |   |   |   ├─Token(LeftBrace) |{|
//@[007:00008) |   | |   |   |   ├─Token(NewLine) |\n|
        name: 'BackendAPIMs'
//@[008:00028) |   | |   |   |   ├─ObjectPropertySyntax
//@[008:00012) |   | |   |   |   | ├─IdentifierSyntax
//@[008:00012) |   | |   |   |   | | └─Token(Identifier) |name|
//@[012:00013) |   | |   |   |   | ├─Token(Colon) |:|
//@[014:00028) |   | |   |   |   | └─StringSyntax
//@[014:00028) |   | |   |   |   |   └─Token(StringComplete) |'BackendAPIMs'|
//@[028:00029) |   | |   |   |   ├─Token(NewLine) |\n|
        properties: {
//@[008:00489) |   | |   |   |   ├─ObjectPropertySyntax
//@[008:00018) |   | |   |   |   | ├─IdentifierSyntax
//@[008:00018) |   | |   |   |   | | └─Token(Identifier) |properties|
//@[018:00019) |   | |   |   |   | ├─Token(Colon) |:|
//@[020:00489) |   | |   |   |   | └─ObjectSyntax
//@[020:00021) |   | |   |   |   |   ├─Token(LeftBrace) |{|
//@[021:00022) |   | |   |   |   |   ├─Token(NewLine) |\n|
          backends: [
//@[010:00457) |   | |   |   |   |   ├─ObjectPropertySyntax
//@[010:00018) |   | |   |   |   |   | ├─IdentifierSyntax
//@[010:00018) |   | |   |   |   |   | | └─Token(Identifier) |backends|
//@[018:00019) |   | |   |   |   |   | ├─Token(Colon) |:|
//@[020:00457) |   | |   |   |   |   | └─ArraySyntax
//@[020:00021) |   | |   |   |   |   |   ├─Token(LeftSquare) |[|
//@[021:00022) |   | |   |   |   |   |   ├─Token(NewLine) |\n|
            {
//@[012:00423) |   | |   |   |   |   |   ├─ArrayItemSyntax
//@[012:00423) |   | |   |   |   |   |   | └─ObjectSyntax
//@[012:00013) |   | |   |   |   |   |   |   ├─Token(LeftBrace) |{|
//@[013:00014) |   | |   |   |   |   |   |   ├─Token(NewLine) |\n|
              // this indexed dependency on a module collection will be generated correctly because
//@[099:00100) |   | |   |   |   |   |   |   ├─Token(NewLine) |\n|
              // copyIndex() can be invoked in the generated dependsOn
//@[070:00071) |   | |   |   |   |   |   |   ├─Token(NewLine) |\n|
              address: apim[index+i].outputs.myOutput
//@[014:00053) |   | |   |   |   |   |   |   ├─ObjectPropertySyntax
//@[014:00021) |   | |   |   |   |   |   |   | ├─IdentifierSyntax
//@[014:00021) |   | |   |   |   |   |   |   | | └─Token(Identifier) |address|
//@[021:00022) |   | |   |   |   |   |   |   | ├─Token(Colon) |:|
//@[023:00053) |   | |   |   |   |   |   |   | └─PropertyAccessSyntax
//@[023:00044) |   | |   |   |   |   |   |   |   ├─PropertyAccessSyntax
//@[023:00036) |   | |   |   |   |   |   |   |   | ├─ArrayAccessSyntax
//@[023:00027) |   | |   |   |   |   |   |   |   | | ├─VariableAccessSyntax
//@[023:00027) |   | |   |   |   |   |   |   |   | | | └─IdentifierSyntax
//@[023:00027) |   | |   |   |   |   |   |   |   | | |   └─Token(Identifier) |apim|
//@[027:00028) |   | |   |   |   |   |   |   |   | | ├─Token(LeftSquare) |[|
//@[028:00035) |   | |   |   |   |   |   |   |   | | ├─BinaryOperationSyntax
//@[028:00033) |   | |   |   |   |   |   |   |   | | | ├─VariableAccessSyntax
//@[028:00033) |   | |   |   |   |   |   |   |   | | | | └─IdentifierSyntax
//@[028:00033) |   | |   |   |   |   |   |   |   | | | |   └─Token(Identifier) |index|
//@[033:00034) |   | |   |   |   |   |   |   |   | | | ├─Token(Plus) |+|
//@[034:00035) |   | |   |   |   |   |   |   |   | | | └─VariableAccessSyntax
//@[034:00035) |   | |   |   |   |   |   |   |   | | |   └─IdentifierSyntax
//@[034:00035) |   | |   |   |   |   |   |   |   | | |     └─Token(Identifier) |i|
//@[035:00036) |   | |   |   |   |   |   |   |   | | └─Token(RightSquare) |]|
//@[036:00037) |   | |   |   |   |   |   |   |   | ├─Token(Dot) |.|
//@[037:00044) |   | |   |   |   |   |   |   |   | └─IdentifierSyntax
//@[037:00044) |   | |   |   |   |   |   |   |   |   └─Token(Identifier) |outputs|
//@[044:00045) |   | |   |   |   |   |   |   |   ├─Token(Dot) |.|
//@[045:00053) |   | |   |   |   |   |   |   |   └─IdentifierSyntax
//@[045:00053) |   | |   |   |   |   |   |   |     └─Token(Identifier) |myOutput|
//@[053:00054) |   | |   |   |   |   |   |   ├─Token(NewLine) |\n|
              backendHostHeader: apim[index+i].outputs.myOutput
//@[014:00063) |   | |   |   |   |   |   |   ├─ObjectPropertySyntax
//@[014:00031) |   | |   |   |   |   |   |   | ├─IdentifierSyntax
//@[014:00031) |   | |   |   |   |   |   |   | | └─Token(Identifier) |backendHostHeader|
//@[031:00032) |   | |   |   |   |   |   |   | ├─Token(Colon) |:|
//@[033:00063) |   | |   |   |   |   |   |   | └─PropertyAccessSyntax
//@[033:00054) |   | |   |   |   |   |   |   |   ├─PropertyAccessSyntax
//@[033:00046) |   | |   |   |   |   |   |   |   | ├─ArrayAccessSyntax
//@[033:00037) |   | |   |   |   |   |   |   |   | | ├─VariableAccessSyntax
//@[033:00037) |   | |   |   |   |   |   |   |   | | | └─IdentifierSyntax
//@[033:00037) |   | |   |   |   |   |   |   |   | | |   └─Token(Identifier) |apim|
//@[037:00038) |   | |   |   |   |   |   |   |   | | ├─Token(LeftSquare) |[|
//@[038:00045) |   | |   |   |   |   |   |   |   | | ├─BinaryOperationSyntax
//@[038:00043) |   | |   |   |   |   |   |   |   | | | ├─VariableAccessSyntax
//@[038:00043) |   | |   |   |   |   |   |   |   | | | | └─IdentifierSyntax
//@[038:00043) |   | |   |   |   |   |   |   |   | | | |   └─Token(Identifier) |index|
//@[043:00044) |   | |   |   |   |   |   |   |   | | | ├─Token(Plus) |+|
//@[044:00045) |   | |   |   |   |   |   |   |   | | | └─VariableAccessSyntax
//@[044:00045) |   | |   |   |   |   |   |   |   | | |   └─IdentifierSyntax
//@[044:00045) |   | |   |   |   |   |   |   |   | | |     └─Token(Identifier) |i|
//@[045:00046) |   | |   |   |   |   |   |   |   | | └─Token(RightSquare) |]|
//@[046:00047) |   | |   |   |   |   |   |   |   | ├─Token(Dot) |.|
//@[047:00054) |   | |   |   |   |   |   |   |   | └─IdentifierSyntax
//@[047:00054) |   | |   |   |   |   |   |   |   |   └─Token(Identifier) |outputs|
//@[054:00055) |   | |   |   |   |   |   |   |   ├─Token(Dot) |.|
//@[055:00063) |   | |   |   |   |   |   |   |   └─IdentifierSyntax
//@[055:00063) |   | |   |   |   |   |   |   |     └─Token(Identifier) |myOutput|
//@[063:00064) |   | |   |   |   |   |   |   ├─Token(NewLine) |\n|
              httpPort: 80
//@[014:00026) |   | |   |   |   |   |   |   ├─ObjectPropertySyntax
//@[014:00022) |   | |   |   |   |   |   |   | ├─IdentifierSyntax
//@[014:00022) |   | |   |   |   |   |   |   | | └─Token(Identifier) |httpPort|
//@[022:00023) |   | |   |   |   |   |   |   | ├─Token(Colon) |:|
//@[024:00026) |   | |   |   |   |   |   |   | └─IntegerLiteralSyntax
//@[024:00026) |   | |   |   |   |   |   |   |   └─Token(Integer) |80|
//@[026:00027) |   | |   |   |   |   |   |   ├─Token(NewLine) |\n|
              httpsPort: 443
//@[014:00028) |   | |   |   |   |   |   |   ├─ObjectPropertySyntax
//@[014:00023) |   | |   |   |   |   |   |   | ├─IdentifierSyntax
//@[014:00023) |   | |   |   |   |   |   |   | | └─Token(Identifier) |httpsPort|
//@[023:00024) |   | |   |   |   |   |   |   | ├─Token(Colon) |:|
//@[025:00028) |   | |   |   |   |   |   |   | └─IntegerLiteralSyntax
//@[025:00028) |   | |   |   |   |   |   |   |   └─Token(Integer) |443|
//@[028:00029) |   | |   |   |   |   |   |   ├─Token(NewLine) |\n|
              priority: 1
//@[014:00025) |   | |   |   |   |   |   |   ├─ObjectPropertySyntax
//@[014:00022) |   | |   |   |   |   |   |   | ├─IdentifierSyntax
//@[014:00022) |   | |   |   |   |   |   |   | | └─Token(Identifier) |priority|
//@[022:00023) |   | |   |   |   |   |   |   | ├─Token(Colon) |:|
//@[024:00025) |   | |   |   |   |   |   |   | └─IntegerLiteralSyntax
//@[024:00025) |   | |   |   |   |   |   |   |   └─Token(Integer) |1|
//@[025:00026) |   | |   |   |   |   |   |   ├─Token(NewLine) |\n|
              weight: 50
//@[014:00024) |   | |   |   |   |   |   |   ├─ObjectPropertySyntax
//@[014:00020) |   | |   |   |   |   |   |   | ├─IdentifierSyntax
//@[014:00020) |   | |   |   |   |   |   |   | | └─Token(Identifier) |weight|
//@[020:00021) |   | |   |   |   |   |   |   | ├─Token(Colon) |:|
//@[022:00024) |   | |   |   |   |   |   |   | └─IntegerLiteralSyntax
//@[022:00024) |   | |   |   |   |   |   |   |   └─Token(Integer) |50|
//@[024:00025) |   | |   |   |   |   |   |   ├─Token(NewLine) |\n|
            }
//@[012:00013) |   | |   |   |   |   |   |   └─Token(RightBrace) |}|
//@[013:00014) |   | |   |   |   |   |   ├─Token(NewLine) |\n|
          ]
//@[010:00011) |   | |   |   |   |   |   └─Token(RightSquare) |]|
//@[011:00012) |   | |   |   |   |   ├─Token(NewLine) |\n|
        }
//@[008:00009) |   | |   |   |   |   └─Token(RightBrace) |}|
//@[009:00010) |   | |   |   |   ├─Token(NewLine) |\n|
      }
//@[006:00007) |   | |   |   |   └─Token(RightBrace) |}|
//@[007:00008) |   | |   |   ├─Token(NewLine) |\n|
    ]
//@[004:00005) |   | |   |   └─Token(RightSquare) |]|
//@[005:00006) |   | |   ├─Token(NewLine) |\n|
  }
//@[002:00003) |   | |   └─Token(RightBrace) |}|
//@[003:00004) |   | ├─Token(NewLine) |\n|
}]
//@[000:00001) |   | └─Token(RightBrace) |}|
//@[001:00002) |   └─Token(RightSquare) |]|
//@[002:00004) ├─Token(NewLine) |\n\n|

resource propertyLoopDependencyOnResourceCollection 'Microsoft.Network/frontDoors@2020-05-01' = {
//@[000:00871) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00051) | ├─IdentifierSyntax
//@[009:00051) | | └─Token(Identifier) |propertyLoopDependencyOnResourceCollection|
//@[052:00093) | ├─StringSyntax
//@[052:00093) | | └─Token(StringComplete) |'Microsoft.Network/frontDoors@2020-05-01'|
//@[094:00095) | ├─Token(Assignment) |=|
//@[096:00871) | └─ObjectSyntax
//@[096:00097) |   ├─Token(LeftBrace) |{|
//@[097:00098) |   ├─Token(NewLine) |\n|
  name: name
//@[002:00012) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00012) |   | └─VariableAccessSyntax
//@[008:00012) |   |   └─IdentifierSyntax
//@[008:00012) |   |     └─Token(Identifier) |name|
//@[012:00013) |   ├─Token(NewLine) |\n|
  location: 'Global'
//@[002:00020) |   ├─ObjectPropertySyntax
//@[002:00010) |   | ├─IdentifierSyntax
//@[002:00010) |   | | └─Token(Identifier) |location|
//@[010:00011) |   | ├─Token(Colon) |:|
//@[012:00020) |   | └─StringSyntax
//@[012:00020) |   |   └─Token(StringComplete) |'Global'|
//@[020:00021) |   ├─Token(NewLine) |\n|
  properties: {
//@[002:00737) |   ├─ObjectPropertySyntax
//@[002:00012) |   | ├─IdentifierSyntax
//@[002:00012) |   | | └─Token(Identifier) |properties|
//@[012:00013) |   | ├─Token(Colon) |:|
//@[014:00737) |   | └─ObjectSyntax
//@[014:00015) |   |   ├─Token(LeftBrace) |{|
//@[015:00016) |   |   ├─Token(NewLine) |\n|
    backendPools: [
//@[004:00717) |   |   ├─ObjectPropertySyntax
//@[004:00016) |   |   | ├─IdentifierSyntax
//@[004:00016) |   |   | | └─Token(Identifier) |backendPools|
//@[016:00017) |   |   | ├─Token(Colon) |:|
//@[018:00717) |   |   | └─ArraySyntax
//@[018:00019) |   |   |   ├─Token(LeftSquare) |[|
//@[019:00020) |   |   |   ├─Token(NewLine) |\n|
      {
//@[006:00691) |   |   |   ├─ArrayItemSyntax
//@[006:00691) |   |   |   | └─ObjectSyntax
//@[006:00007) |   |   |   |   ├─Token(LeftBrace) |{|
//@[007:00008) |   |   |   |   ├─Token(NewLine) |\n|
        name: 'BackendAPIMs'
//@[008:00028) |   |   |   |   ├─ObjectPropertySyntax
//@[008:00012) |   |   |   |   | ├─IdentifierSyntax
//@[008:00012) |   |   |   |   | | └─Token(Identifier) |name|
//@[012:00013) |   |   |   |   | ├─Token(Colon) |:|
//@[014:00028) |   |   |   |   | └─StringSyntax
//@[014:00028) |   |   |   |   |   └─Token(StringComplete) |'BackendAPIMs'|
//@[028:00029) |   |   |   |   ├─Token(NewLine) |\n|
        properties: {
//@[008:00646) |   |   |   |   ├─ObjectPropertySyntax
//@[008:00018) |   |   |   |   | ├─IdentifierSyntax
//@[008:00018) |   |   |   |   | | └─Token(Identifier) |properties|
//@[018:00019) |   |   |   |   | ├─Token(Colon) |:|
//@[020:00646) |   |   |   |   | └─ObjectSyntax
//@[020:00021) |   |   |   |   |   ├─Token(LeftBrace) |{|
//@[021:00022) |   |   |   |   |   ├─Token(NewLine) |\n|
          backends: [for index in range(0, length(accounts)): {
//@[010:00614) |   |   |   |   |   ├─ObjectPropertySyntax
//@[010:00018) |   |   |   |   |   | ├─IdentifierSyntax
//@[010:00018) |   |   |   |   |   | | └─Token(Identifier) |backends|
//@[018:00019) |   |   |   |   |   | ├─Token(Colon) |:|
//@[020:00614) |   |   |   |   |   | └─ForSyntax
//@[020:00021) |   |   |   |   |   |   ├─Token(LeftSquare) |[|
//@[021:00024) |   |   |   |   |   |   ├─Token(Identifier) |for|
//@[025:00030) |   |   |   |   |   |   ├─LocalVariableSyntax
//@[025:00030) |   |   |   |   |   |   | └─IdentifierSyntax
//@[025:00030) |   |   |   |   |   |   |   └─Token(Identifier) |index|
//@[031:00033) |   |   |   |   |   |   ├─Token(Identifier) |in|
//@[034:00060) |   |   |   |   |   |   ├─FunctionCallSyntax
//@[034:00039) |   |   |   |   |   |   | ├─IdentifierSyntax
//@[034:00039) |   |   |   |   |   |   | | └─Token(Identifier) |range|
//@[039:00040) |   |   |   |   |   |   | ├─Token(LeftParen) |(|
//@[040:00041) |   |   |   |   |   |   | ├─FunctionArgumentSyntax
//@[040:00041) |   |   |   |   |   |   | | └─IntegerLiteralSyntax
//@[040:00041) |   |   |   |   |   |   | |   └─Token(Integer) |0|
//@[041:00042) |   |   |   |   |   |   | ├─Token(Comma) |,|
//@[043:00059) |   |   |   |   |   |   | ├─FunctionArgumentSyntax
//@[043:00059) |   |   |   |   |   |   | | └─FunctionCallSyntax
//@[043:00049) |   |   |   |   |   |   | |   ├─IdentifierSyntax
//@[043:00049) |   |   |   |   |   |   | |   | └─Token(Identifier) |length|
//@[049:00050) |   |   |   |   |   |   | |   ├─Token(LeftParen) |(|
//@[050:00058) |   |   |   |   |   |   | |   ├─FunctionArgumentSyntax
//@[050:00058) |   |   |   |   |   |   | |   | └─VariableAccessSyntax
//@[050:00058) |   |   |   |   |   |   | |   |   └─IdentifierSyntax
//@[050:00058) |   |   |   |   |   |   | |   |     └─Token(Identifier) |accounts|
//@[058:00059) |   |   |   |   |   |   | |   └─Token(RightParen) |)|
//@[059:00060) |   |   |   |   |   |   | └─Token(RightParen) |)|
//@[060:00061) |   |   |   |   |   |   ├─Token(Colon) |:|
//@[062:00613) |   |   |   |   |   |   ├─ObjectSyntax
//@[062:00063) |   |   |   |   |   |   | ├─Token(LeftBrace) |{|
//@[063:00064) |   |   |   |   |   |   | ├─Token(NewLine) |\n|
            // we cannot codegen index correctly because the generated dependsOn property
//@[089:00090) |   |   |   |   |   |   | ├─Token(NewLine) |\n|
            // would be outside of the scope of the property loop
//@[065:00066) |   |   |   |   |   |   | ├─Token(NewLine) |\n|
            // as a result, this will generate a dependency on the entire collection
//@[084:00085) |   |   |   |   |   |   | ├─Token(NewLine) |\n|
            address: storageAccounts[index].properties.primaryEndpoints.internetEndpoints.web
//@[012:00093) |   |   |   |   |   |   | ├─ObjectPropertySyntax
//@[012:00019) |   |   |   |   |   |   | | ├─IdentifierSyntax
//@[012:00019) |   |   |   |   |   |   | | | └─Token(Identifier) |address|
//@[019:00020) |   |   |   |   |   |   | | ├─Token(Colon) |:|
//@[021:00093) |   |   |   |   |   |   | | └─PropertyAccessSyntax
//@[021:00089) |   |   |   |   |   |   | |   ├─PropertyAccessSyntax
//@[021:00071) |   |   |   |   |   |   | |   | ├─PropertyAccessSyntax
//@[021:00054) |   |   |   |   |   |   | |   | | ├─PropertyAccessSyntax
//@[021:00043) |   |   |   |   |   |   | |   | | | ├─ArrayAccessSyntax
//@[021:00036) |   |   |   |   |   |   | |   | | | | ├─VariableAccessSyntax
//@[021:00036) |   |   |   |   |   |   | |   | | | | | └─IdentifierSyntax
//@[021:00036) |   |   |   |   |   |   | |   | | | | |   └─Token(Identifier) |storageAccounts|
//@[036:00037) |   |   |   |   |   |   | |   | | | | ├─Token(LeftSquare) |[|
//@[037:00042) |   |   |   |   |   |   | |   | | | | ├─VariableAccessSyntax
//@[037:00042) |   |   |   |   |   |   | |   | | | | | └─IdentifierSyntax
//@[037:00042) |   |   |   |   |   |   | |   | | | | |   └─Token(Identifier) |index|
//@[042:00043) |   |   |   |   |   |   | |   | | | | └─Token(RightSquare) |]|
//@[043:00044) |   |   |   |   |   |   | |   | | | ├─Token(Dot) |.|
//@[044:00054) |   |   |   |   |   |   | |   | | | └─IdentifierSyntax
//@[044:00054) |   |   |   |   |   |   | |   | | |   └─Token(Identifier) |properties|
//@[054:00055) |   |   |   |   |   |   | |   | | ├─Token(Dot) |.|
//@[055:00071) |   |   |   |   |   |   | |   | | └─IdentifierSyntax
//@[055:00071) |   |   |   |   |   |   | |   | |   └─Token(Identifier) |primaryEndpoints|
//@[071:00072) |   |   |   |   |   |   | |   | ├─Token(Dot) |.|
//@[072:00089) |   |   |   |   |   |   | |   | └─IdentifierSyntax
//@[072:00089) |   |   |   |   |   |   | |   |   └─Token(Identifier) |internetEndpoints|
//@[089:00090) |   |   |   |   |   |   | |   ├─Token(Dot) |.|
//@[090:00093) |   |   |   |   |   |   | |   └─IdentifierSyntax
//@[090:00093) |   |   |   |   |   |   | |     └─Token(Identifier) |web|
//@[093:00094) |   |   |   |   |   |   | ├─Token(NewLine) |\n|
            backendHostHeader: storageAccounts[index].properties.primaryEndpoints.internetEndpoints.web
//@[012:00103) |   |   |   |   |   |   | ├─ObjectPropertySyntax
//@[012:00029) |   |   |   |   |   |   | | ├─IdentifierSyntax
//@[012:00029) |   |   |   |   |   |   | | | └─Token(Identifier) |backendHostHeader|
//@[029:00030) |   |   |   |   |   |   | | ├─Token(Colon) |:|
//@[031:00103) |   |   |   |   |   |   | | └─PropertyAccessSyntax
//@[031:00099) |   |   |   |   |   |   | |   ├─PropertyAccessSyntax
//@[031:00081) |   |   |   |   |   |   | |   | ├─PropertyAccessSyntax
//@[031:00064) |   |   |   |   |   |   | |   | | ├─PropertyAccessSyntax
//@[031:00053) |   |   |   |   |   |   | |   | | | ├─ArrayAccessSyntax
//@[031:00046) |   |   |   |   |   |   | |   | | | | ├─VariableAccessSyntax
//@[031:00046) |   |   |   |   |   |   | |   | | | | | └─IdentifierSyntax
//@[031:00046) |   |   |   |   |   |   | |   | | | | |   └─Token(Identifier) |storageAccounts|
//@[046:00047) |   |   |   |   |   |   | |   | | | | ├─Token(LeftSquare) |[|
//@[047:00052) |   |   |   |   |   |   | |   | | | | ├─VariableAccessSyntax
//@[047:00052) |   |   |   |   |   |   | |   | | | | | └─IdentifierSyntax
//@[047:00052) |   |   |   |   |   |   | |   | | | | |   └─Token(Identifier) |index|
//@[052:00053) |   |   |   |   |   |   | |   | | | | └─Token(RightSquare) |]|
//@[053:00054) |   |   |   |   |   |   | |   | | | ├─Token(Dot) |.|
//@[054:00064) |   |   |   |   |   |   | |   | | | └─IdentifierSyntax
//@[054:00064) |   |   |   |   |   |   | |   | | |   └─Token(Identifier) |properties|
//@[064:00065) |   |   |   |   |   |   | |   | | ├─Token(Dot) |.|
//@[065:00081) |   |   |   |   |   |   | |   | | └─IdentifierSyntax
//@[065:00081) |   |   |   |   |   |   | |   | |   └─Token(Identifier) |primaryEndpoints|
//@[081:00082) |   |   |   |   |   |   | |   | ├─Token(Dot) |.|
//@[082:00099) |   |   |   |   |   |   | |   | └─IdentifierSyntax
//@[082:00099) |   |   |   |   |   |   | |   |   └─Token(Identifier) |internetEndpoints|
//@[099:00100) |   |   |   |   |   |   | |   ├─Token(Dot) |.|
//@[100:00103) |   |   |   |   |   |   | |   └─IdentifierSyntax
//@[100:00103) |   |   |   |   |   |   | |     └─Token(Identifier) |web|
//@[103:00104) |   |   |   |   |   |   | ├─Token(NewLine) |\n|
            httpPort: 80
//@[012:00024) |   |   |   |   |   |   | ├─ObjectPropertySyntax
//@[012:00020) |   |   |   |   |   |   | | ├─IdentifierSyntax
//@[012:00020) |   |   |   |   |   |   | | | └─Token(Identifier) |httpPort|
//@[020:00021) |   |   |   |   |   |   | | ├─Token(Colon) |:|
//@[022:00024) |   |   |   |   |   |   | | └─IntegerLiteralSyntax
//@[022:00024) |   |   |   |   |   |   | |   └─Token(Integer) |80|
//@[024:00025) |   |   |   |   |   |   | ├─Token(NewLine) |\n|
            httpsPort: 443
//@[012:00026) |   |   |   |   |   |   | ├─ObjectPropertySyntax
//@[012:00021) |   |   |   |   |   |   | | ├─IdentifierSyntax
//@[012:00021) |   |   |   |   |   |   | | | └─Token(Identifier) |httpsPort|
//@[021:00022) |   |   |   |   |   |   | | ├─Token(Colon) |:|
//@[023:00026) |   |   |   |   |   |   | | └─IntegerLiteralSyntax
//@[023:00026) |   |   |   |   |   |   | |   └─Token(Integer) |443|
//@[026:00027) |   |   |   |   |   |   | ├─Token(NewLine) |\n|
            priority: 1
//@[012:00023) |   |   |   |   |   |   | ├─ObjectPropertySyntax
//@[012:00020) |   |   |   |   |   |   | | ├─IdentifierSyntax
//@[012:00020) |   |   |   |   |   |   | | | └─Token(Identifier) |priority|
//@[020:00021) |   |   |   |   |   |   | | ├─Token(Colon) |:|
//@[022:00023) |   |   |   |   |   |   | | └─IntegerLiteralSyntax
//@[022:00023) |   |   |   |   |   |   | |   └─Token(Integer) |1|
//@[023:00024) |   |   |   |   |   |   | ├─Token(NewLine) |\n|
            weight: 50
//@[012:00022) |   |   |   |   |   |   | ├─ObjectPropertySyntax
//@[012:00018) |   |   |   |   |   |   | | ├─IdentifierSyntax
//@[012:00018) |   |   |   |   |   |   | | | └─Token(Identifier) |weight|
//@[018:00019) |   |   |   |   |   |   | | ├─Token(Colon) |:|
//@[020:00022) |   |   |   |   |   |   | | └─IntegerLiteralSyntax
//@[020:00022) |   |   |   |   |   |   | |   └─Token(Integer) |50|
//@[022:00023) |   |   |   |   |   |   | ├─Token(NewLine) |\n|
          }]
//@[010:00011) |   |   |   |   |   |   | └─Token(RightBrace) |}|
//@[011:00012) |   |   |   |   |   |   └─Token(RightSquare) |]|
//@[012:00013) |   |   |   |   |   ├─Token(NewLine) |\n|
        }
//@[008:00009) |   |   |   |   |   └─Token(RightBrace) |}|
//@[009:00010) |   |   |   |   ├─Token(NewLine) |\n|
      }
//@[006:00007) |   |   |   |   └─Token(RightBrace) |}|
//@[007:00008) |   |   |   ├─Token(NewLine) |\n|
    ]
//@[004:00005) |   |   |   └─Token(RightSquare) |]|
//@[005:00006) |   |   ├─Token(NewLine) |\n|
  }
//@[002:00003) |   |   └─Token(RightBrace) |}|
//@[003:00004) |   ├─Token(NewLine) |\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00003) ├─Token(NewLine) |\n\n|

resource indexedResourceCollectionDependency 'Microsoft.Network/frontDoors@2020-05-01' = [for (index,i) in range(0, length(accounts)): {
//@[000:00861) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00044) | ├─IdentifierSyntax
//@[009:00044) | | └─Token(Identifier) |indexedResourceCollectionDependency|
//@[045:00086) | ├─StringSyntax
//@[045:00086) | | └─Token(StringComplete) |'Microsoft.Network/frontDoors@2020-05-01'|
//@[087:00088) | ├─Token(Assignment) |=|
//@[089:00861) | └─ForSyntax
//@[089:00090) |   ├─Token(LeftSquare) |[|
//@[090:00093) |   ├─Token(Identifier) |for|
//@[094:00103) |   ├─VariableBlockSyntax
//@[094:00095) |   | ├─Token(LeftParen) |(|
//@[095:00100) |   | ├─LocalVariableSyntax
//@[095:00100) |   | | └─IdentifierSyntax
//@[095:00100) |   | |   └─Token(Identifier) |index|
//@[100:00101) |   | ├─Token(Comma) |,|
//@[101:00102) |   | ├─LocalVariableSyntax
//@[101:00102) |   | | └─IdentifierSyntax
//@[101:00102) |   | |   └─Token(Identifier) |i|
//@[102:00103) |   | └─Token(RightParen) |)|
//@[104:00106) |   ├─Token(Identifier) |in|
//@[107:00133) |   ├─FunctionCallSyntax
//@[107:00112) |   | ├─IdentifierSyntax
//@[107:00112) |   | | └─Token(Identifier) |range|
//@[112:00113) |   | ├─Token(LeftParen) |(|
//@[113:00114) |   | ├─FunctionArgumentSyntax
//@[113:00114) |   | | └─IntegerLiteralSyntax
//@[113:00114) |   | |   └─Token(Integer) |0|
//@[114:00115) |   | ├─Token(Comma) |,|
//@[116:00132) |   | ├─FunctionArgumentSyntax
//@[116:00132) |   | | └─FunctionCallSyntax
//@[116:00122) |   | |   ├─IdentifierSyntax
//@[116:00122) |   | |   | └─Token(Identifier) |length|
//@[122:00123) |   | |   ├─Token(LeftParen) |(|
//@[123:00131) |   | |   ├─FunctionArgumentSyntax
//@[123:00131) |   | |   | └─VariableAccessSyntax
//@[123:00131) |   | |   |   └─IdentifierSyntax
//@[123:00131) |   | |   |     └─Token(Identifier) |accounts|
//@[131:00132) |   | |   └─Token(RightParen) |)|
//@[132:00133) |   | └─Token(RightParen) |)|
//@[133:00134) |   ├─Token(Colon) |:|
//@[135:00860) |   ├─ObjectSyntax
//@[135:00136) |   | ├─Token(LeftBrace) |{|
//@[136:00137) |   | ├─Token(NewLine) |\n|
  name: '${name}-${index}-${i}'
//@[002:00031) |   | ├─ObjectPropertySyntax
//@[002:00006) |   | | ├─IdentifierSyntax
//@[002:00006) |   | | | └─Token(Identifier) |name|
//@[006:00007) |   | | ├─Token(Colon) |:|
//@[008:00031) |   | | └─StringSyntax
//@[008:00011) |   | |   ├─Token(StringLeftPiece) |'${|
//@[011:00015) |   | |   ├─VariableAccessSyntax
//@[011:00015) |   | |   | └─IdentifierSyntax
//@[011:00015) |   | |   |   └─Token(Identifier) |name|
//@[015:00019) |   | |   ├─Token(StringMiddlePiece) |}-${|
//@[019:00024) |   | |   ├─VariableAccessSyntax
//@[019:00024) |   | |   | └─IdentifierSyntax
//@[019:00024) |   | |   |   └─Token(Identifier) |index|
//@[024:00028) |   | |   ├─Token(StringMiddlePiece) |}-${|
//@[028:00029) |   | |   ├─VariableAccessSyntax
//@[028:00029) |   | |   | └─IdentifierSyntax
//@[028:00029) |   | |   |   └─Token(Identifier) |i|
//@[029:00031) |   | |   └─Token(StringRightPiece) |}'|
//@[031:00032) |   | ├─Token(NewLine) |\n|
  location: 'Global'
//@[002:00020) |   | ├─ObjectPropertySyntax
//@[002:00010) |   | | ├─IdentifierSyntax
//@[002:00010) |   | | | └─Token(Identifier) |location|
//@[010:00011) |   | | ├─Token(Colon) |:|
//@[012:00020) |   | | └─StringSyntax
//@[012:00020) |   | |   └─Token(StringComplete) |'Global'|
//@[020:00021) |   | ├─Token(NewLine) |\n|
  properties: {
//@[002:00668) |   | ├─ObjectPropertySyntax
//@[002:00012) |   | | ├─IdentifierSyntax
//@[002:00012) |   | | | └─Token(Identifier) |properties|
//@[012:00013) |   | | ├─Token(Colon) |:|
//@[014:00668) |   | | └─ObjectSyntax
//@[014:00015) |   | |   ├─Token(LeftBrace) |{|
//@[015:00016) |   | |   ├─Token(NewLine) |\n|
    backendPools: [
//@[004:00648) |   | |   ├─ObjectPropertySyntax
//@[004:00016) |   | |   | ├─IdentifierSyntax
//@[004:00016) |   | |   | | └─Token(Identifier) |backendPools|
//@[016:00017) |   | |   | ├─Token(Colon) |:|
//@[018:00648) |   | |   | └─ArraySyntax
//@[018:00019) |   | |   |   ├─Token(LeftSquare) |[|
//@[019:00020) |   | |   |   ├─Token(NewLine) |\n|
      {
//@[006:00622) |   | |   |   ├─ArrayItemSyntax
//@[006:00622) |   | |   |   | └─ObjectSyntax
//@[006:00007) |   | |   |   |   ├─Token(LeftBrace) |{|
//@[007:00008) |   | |   |   |   ├─Token(NewLine) |\n|
        name: 'BackendAPIMs'
//@[008:00028) |   | |   |   |   ├─ObjectPropertySyntax
//@[008:00012) |   | |   |   |   | ├─IdentifierSyntax
//@[008:00012) |   | |   |   |   | | └─Token(Identifier) |name|
//@[012:00013) |   | |   |   |   | ├─Token(Colon) |:|
//@[014:00028) |   | |   |   |   | └─StringSyntax
//@[014:00028) |   | |   |   |   |   └─Token(StringComplete) |'BackendAPIMs'|
//@[028:00029) |   | |   |   |   ├─Token(NewLine) |\n|
        properties: {
//@[008:00577) |   | |   |   |   ├─ObjectPropertySyntax
//@[008:00018) |   | |   |   |   | ├─IdentifierSyntax
//@[008:00018) |   | |   |   |   | | └─Token(Identifier) |properties|
//@[018:00019) |   | |   |   |   | ├─Token(Colon) |:|
//@[020:00577) |   | |   |   |   | └─ObjectSyntax
//@[020:00021) |   | |   |   |   |   ├─Token(LeftBrace) |{|
//@[021:00022) |   | |   |   |   |   ├─Token(NewLine) |\n|
          backends: [
//@[010:00545) |   | |   |   |   |   ├─ObjectPropertySyntax
//@[010:00018) |   | |   |   |   |   | ├─IdentifierSyntax
//@[010:00018) |   | |   |   |   |   | | └─Token(Identifier) |backends|
//@[018:00019) |   | |   |   |   |   | ├─Token(Colon) |:|
//@[020:00545) |   | |   |   |   |   | └─ArraySyntax
//@[020:00021) |   | |   |   |   |   |   ├─Token(LeftSquare) |[|
//@[021:00022) |   | |   |   |   |   |   ├─Token(NewLine) |\n|
            {
//@[012:00511) |   | |   |   |   |   |   ├─ArrayItemSyntax
//@[012:00511) |   | |   |   |   |   |   | └─ObjectSyntax
//@[012:00013) |   | |   |   |   |   |   |   ├─Token(LeftBrace) |{|
//@[013:00014) |   | |   |   |   |   |   |   ├─Token(NewLine) |\n|
              // this indexed dependency on a module collection will be generated correctly because
//@[099:00100) |   | |   |   |   |   |   |   ├─Token(NewLine) |\n|
              // copyIndex() can be invoked in the generated dependsOn
//@[070:00071) |   | |   |   |   |   |   |   ├─Token(NewLine) |\n|
              address: storageAccounts[index+i].properties.primaryEndpoints.internetEndpoints.web
//@[014:00097) |   | |   |   |   |   |   |   ├─ObjectPropertySyntax
//@[014:00021) |   | |   |   |   |   |   |   | ├─IdentifierSyntax
//@[014:00021) |   | |   |   |   |   |   |   | | └─Token(Identifier) |address|
//@[021:00022) |   | |   |   |   |   |   |   | ├─Token(Colon) |:|
//@[023:00097) |   | |   |   |   |   |   |   | └─PropertyAccessSyntax
//@[023:00093) |   | |   |   |   |   |   |   |   ├─PropertyAccessSyntax
//@[023:00075) |   | |   |   |   |   |   |   |   | ├─PropertyAccessSyntax
//@[023:00058) |   | |   |   |   |   |   |   |   | | ├─PropertyAccessSyntax
//@[023:00047) |   | |   |   |   |   |   |   |   | | | ├─ArrayAccessSyntax
//@[023:00038) |   | |   |   |   |   |   |   |   | | | | ├─VariableAccessSyntax
//@[023:00038) |   | |   |   |   |   |   |   |   | | | | | └─IdentifierSyntax
//@[023:00038) |   | |   |   |   |   |   |   |   | | | | |   └─Token(Identifier) |storageAccounts|
//@[038:00039) |   | |   |   |   |   |   |   |   | | | | ├─Token(LeftSquare) |[|
//@[039:00046) |   | |   |   |   |   |   |   |   | | | | ├─BinaryOperationSyntax
//@[039:00044) |   | |   |   |   |   |   |   |   | | | | | ├─VariableAccessSyntax
//@[039:00044) |   | |   |   |   |   |   |   |   | | | | | | └─IdentifierSyntax
//@[039:00044) |   | |   |   |   |   |   |   |   | | | | | |   └─Token(Identifier) |index|
//@[044:00045) |   | |   |   |   |   |   |   |   | | | | | ├─Token(Plus) |+|
//@[045:00046) |   | |   |   |   |   |   |   |   | | | | | └─VariableAccessSyntax
//@[045:00046) |   | |   |   |   |   |   |   |   | | | | |   └─IdentifierSyntax
//@[045:00046) |   | |   |   |   |   |   |   |   | | | | |     └─Token(Identifier) |i|
//@[046:00047) |   | |   |   |   |   |   |   |   | | | | └─Token(RightSquare) |]|
//@[047:00048) |   | |   |   |   |   |   |   |   | | | ├─Token(Dot) |.|
//@[048:00058) |   | |   |   |   |   |   |   |   | | | └─IdentifierSyntax
//@[048:00058) |   | |   |   |   |   |   |   |   | | |   └─Token(Identifier) |properties|
//@[058:00059) |   | |   |   |   |   |   |   |   | | ├─Token(Dot) |.|
//@[059:00075) |   | |   |   |   |   |   |   |   | | └─IdentifierSyntax
//@[059:00075) |   | |   |   |   |   |   |   |   | |   └─Token(Identifier) |primaryEndpoints|
//@[075:00076) |   | |   |   |   |   |   |   |   | ├─Token(Dot) |.|
//@[076:00093) |   | |   |   |   |   |   |   |   | └─IdentifierSyntax
//@[076:00093) |   | |   |   |   |   |   |   |   |   └─Token(Identifier) |internetEndpoints|
//@[093:00094) |   | |   |   |   |   |   |   |   ├─Token(Dot) |.|
//@[094:00097) |   | |   |   |   |   |   |   |   └─IdentifierSyntax
//@[094:00097) |   | |   |   |   |   |   |   |     └─Token(Identifier) |web|
//@[097:00098) |   | |   |   |   |   |   |   ├─Token(NewLine) |\n|
              backendHostHeader: storageAccounts[index+i].properties.primaryEndpoints.internetEndpoints.web
//@[014:00107) |   | |   |   |   |   |   |   ├─ObjectPropertySyntax
//@[014:00031) |   | |   |   |   |   |   |   | ├─IdentifierSyntax
//@[014:00031) |   | |   |   |   |   |   |   | | └─Token(Identifier) |backendHostHeader|
//@[031:00032) |   | |   |   |   |   |   |   | ├─Token(Colon) |:|
//@[033:00107) |   | |   |   |   |   |   |   | └─PropertyAccessSyntax
//@[033:00103) |   | |   |   |   |   |   |   |   ├─PropertyAccessSyntax
//@[033:00085) |   | |   |   |   |   |   |   |   | ├─PropertyAccessSyntax
//@[033:00068) |   | |   |   |   |   |   |   |   | | ├─PropertyAccessSyntax
//@[033:00057) |   | |   |   |   |   |   |   |   | | | ├─ArrayAccessSyntax
//@[033:00048) |   | |   |   |   |   |   |   |   | | | | ├─VariableAccessSyntax
//@[033:00048) |   | |   |   |   |   |   |   |   | | | | | └─IdentifierSyntax
//@[033:00048) |   | |   |   |   |   |   |   |   | | | | |   └─Token(Identifier) |storageAccounts|
//@[048:00049) |   | |   |   |   |   |   |   |   | | | | ├─Token(LeftSquare) |[|
//@[049:00056) |   | |   |   |   |   |   |   |   | | | | ├─BinaryOperationSyntax
//@[049:00054) |   | |   |   |   |   |   |   |   | | | | | ├─VariableAccessSyntax
//@[049:00054) |   | |   |   |   |   |   |   |   | | | | | | └─IdentifierSyntax
//@[049:00054) |   | |   |   |   |   |   |   |   | | | | | |   └─Token(Identifier) |index|
//@[054:00055) |   | |   |   |   |   |   |   |   | | | | | ├─Token(Plus) |+|
//@[055:00056) |   | |   |   |   |   |   |   |   | | | | | └─VariableAccessSyntax
//@[055:00056) |   | |   |   |   |   |   |   |   | | | | |   └─IdentifierSyntax
//@[055:00056) |   | |   |   |   |   |   |   |   | | | | |     └─Token(Identifier) |i|
//@[056:00057) |   | |   |   |   |   |   |   |   | | | | └─Token(RightSquare) |]|
//@[057:00058) |   | |   |   |   |   |   |   |   | | | ├─Token(Dot) |.|
//@[058:00068) |   | |   |   |   |   |   |   |   | | | └─IdentifierSyntax
//@[058:00068) |   | |   |   |   |   |   |   |   | | |   └─Token(Identifier) |properties|
//@[068:00069) |   | |   |   |   |   |   |   |   | | ├─Token(Dot) |.|
//@[069:00085) |   | |   |   |   |   |   |   |   | | └─IdentifierSyntax
//@[069:00085) |   | |   |   |   |   |   |   |   | |   └─Token(Identifier) |primaryEndpoints|
//@[085:00086) |   | |   |   |   |   |   |   |   | ├─Token(Dot) |.|
//@[086:00103) |   | |   |   |   |   |   |   |   | └─IdentifierSyntax
//@[086:00103) |   | |   |   |   |   |   |   |   |   └─Token(Identifier) |internetEndpoints|
//@[103:00104) |   | |   |   |   |   |   |   |   ├─Token(Dot) |.|
//@[104:00107) |   | |   |   |   |   |   |   |   └─IdentifierSyntax
//@[104:00107) |   | |   |   |   |   |   |   |     └─Token(Identifier) |web|
//@[107:00108) |   | |   |   |   |   |   |   ├─Token(NewLine) |\n|
              httpPort: 80
//@[014:00026) |   | |   |   |   |   |   |   ├─ObjectPropertySyntax
//@[014:00022) |   | |   |   |   |   |   |   | ├─IdentifierSyntax
//@[014:00022) |   | |   |   |   |   |   |   | | └─Token(Identifier) |httpPort|
//@[022:00023) |   | |   |   |   |   |   |   | ├─Token(Colon) |:|
//@[024:00026) |   | |   |   |   |   |   |   | └─IntegerLiteralSyntax
//@[024:00026) |   | |   |   |   |   |   |   |   └─Token(Integer) |80|
//@[026:00027) |   | |   |   |   |   |   |   ├─Token(NewLine) |\n|
              httpsPort: 443
//@[014:00028) |   | |   |   |   |   |   |   ├─ObjectPropertySyntax
//@[014:00023) |   | |   |   |   |   |   |   | ├─IdentifierSyntax
//@[014:00023) |   | |   |   |   |   |   |   | | └─Token(Identifier) |httpsPort|
//@[023:00024) |   | |   |   |   |   |   |   | ├─Token(Colon) |:|
//@[025:00028) |   | |   |   |   |   |   |   | └─IntegerLiteralSyntax
//@[025:00028) |   | |   |   |   |   |   |   |   └─Token(Integer) |443|
//@[028:00029) |   | |   |   |   |   |   |   ├─Token(NewLine) |\n|
              priority: 1
//@[014:00025) |   | |   |   |   |   |   |   ├─ObjectPropertySyntax
//@[014:00022) |   | |   |   |   |   |   |   | ├─IdentifierSyntax
//@[014:00022) |   | |   |   |   |   |   |   | | └─Token(Identifier) |priority|
//@[022:00023) |   | |   |   |   |   |   |   | ├─Token(Colon) |:|
//@[024:00025) |   | |   |   |   |   |   |   | └─IntegerLiteralSyntax
//@[024:00025) |   | |   |   |   |   |   |   |   └─Token(Integer) |1|
//@[025:00026) |   | |   |   |   |   |   |   ├─Token(NewLine) |\n|
              weight: 50
//@[014:00024) |   | |   |   |   |   |   |   ├─ObjectPropertySyntax
//@[014:00020) |   | |   |   |   |   |   |   | ├─IdentifierSyntax
//@[014:00020) |   | |   |   |   |   |   |   | | └─Token(Identifier) |weight|
//@[020:00021) |   | |   |   |   |   |   |   | ├─Token(Colon) |:|
//@[022:00024) |   | |   |   |   |   |   |   | └─IntegerLiteralSyntax
//@[022:00024) |   | |   |   |   |   |   |   |   └─Token(Integer) |50|
//@[024:00025) |   | |   |   |   |   |   |   ├─Token(NewLine) |\n|
            }
//@[012:00013) |   | |   |   |   |   |   |   └─Token(RightBrace) |}|
//@[013:00014) |   | |   |   |   |   |   ├─Token(NewLine) |\n|
          ]
//@[010:00011) |   | |   |   |   |   |   └─Token(RightSquare) |]|
//@[011:00012) |   | |   |   |   |   ├─Token(NewLine) |\n|
        }
//@[008:00009) |   | |   |   |   |   └─Token(RightBrace) |}|
//@[009:00010) |   | |   |   |   ├─Token(NewLine) |\n|
      }
//@[006:00007) |   | |   |   |   └─Token(RightBrace) |}|
//@[007:00008) |   | |   |   ├─Token(NewLine) |\n|
    ]
//@[004:00005) |   | |   |   └─Token(RightSquare) |]|
//@[005:00006) |   | |   ├─Token(NewLine) |\n|
  }
//@[002:00003) |   | |   └─Token(RightBrace) |}|
//@[003:00004) |   | ├─Token(NewLine) |\n|
}]
//@[000:00001) |   | └─Token(RightBrace) |}|
//@[001:00002) |   └─Token(RightSquare) |]|
//@[002:00003) ├─Token(NewLine) |\n|

//@[000:00000) └─Token(EndOfFile) ||
