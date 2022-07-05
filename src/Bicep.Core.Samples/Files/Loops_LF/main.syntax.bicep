param name string
//@[000:12019) ProgramSyntax
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
//@[000:00289) ├─ResourceDeclarationSyntax
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
resource storageAccounts 'Microsoft.Storage/storageAccounts@2019-06-01' = [for account in accounts: {
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00024) | ├─IdentifierSyntax
//@[009:00024) | | └─Token(Identifier) |storageAccounts|
//@[025:00071) | ├─StringSyntax
//@[025:00071) | | └─Token(StringComplete) |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[072:00073) | ├─Token(Assignment) |=|
//@[074:00274) | └─ForSyntax
//@[074:00075) |   ├─Token(LeftSquare) |[|
//@[075:00078) |   ├─Token(Identifier) |for|
//@[079:00086) |   ├─LocalVariableSyntax
//@[079:00086) |   | └─IdentifierSyntax
//@[079:00086) |   |   └─Token(Identifier) |account|
//@[087:00089) |   ├─Token(Identifier) |in|
//@[090:00098) |   ├─VariableAccessSyntax
//@[090:00098) |   | └─IdentifierSyntax
//@[090:00098) |   |   └─Token(Identifier) |accounts|
//@[098:00099) |   ├─Token(Colon) |:|
//@[100:00273) |   ├─ObjectSyntax
//@[100:00101) |   | ├─Token(LeftBrace) |{|
//@[101:00102) |   | ├─Token(NewLine) |\n|
  name: '${name}-collection-${account.name}'
//@[002:00044) |   | ├─ObjectPropertySyntax
//@[002:00006) |   | | ├─IdentifierSyntax
//@[002:00006) |   | | | └─Token(Identifier) |name|
//@[006:00007) |   | | ├─Token(Colon) |:|
//@[008:00044) |   | | └─StringSyntax
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
//@[042:00044) |   | |   └─Token(StringRightPiece) |}'|
//@[044:00045) |   | ├─Token(NewLine) |\n|
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
resource extensionCollection 'Microsoft.Authorization/locks@2016-09-01' = [for i in range(0,1): {
//@[000:00212) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00028) | ├─IdentifierSyntax
//@[009:00028) | | └─Token(Identifier) |extensionCollection|
//@[029:00071) | ├─StringSyntax
//@[029:00071) | | └─Token(StringComplete) |'Microsoft.Authorization/locks@2016-09-01'|
//@[072:00073) | ├─Token(Assignment) |=|
//@[074:00212) | └─ForSyntax
//@[074:00075) |   ├─Token(LeftSquare) |[|
//@[075:00078) |   ├─Token(Identifier) |for|
//@[079:00080) |   ├─LocalVariableSyntax
//@[079:00080) |   | └─IdentifierSyntax
//@[079:00080) |   |   └─Token(Identifier) |i|
//@[081:00083) |   ├─Token(Identifier) |in|
//@[084:00094) |   ├─FunctionCallSyntax
//@[084:00089) |   | ├─IdentifierSyntax
//@[084:00089) |   | | └─Token(Identifier) |range|
//@[089:00090) |   | ├─Token(LeftParen) |(|
//@[090:00091) |   | ├─FunctionArgumentSyntax
//@[090:00091) |   | | └─IntegerLiteralSyntax
//@[090:00091) |   | |   └─Token(Integer) |0|
//@[091:00092) |   | ├─Token(Comma) |,|
//@[092:00093) |   | ├─FunctionArgumentSyntax
//@[092:00093) |   | | └─IntegerLiteralSyntax
//@[092:00093) |   | |   └─Token(Integer) |1|
//@[093:00094) |   | └─Token(RightParen) |)|
//@[094:00095) |   ├─Token(Colon) |:|
//@[096:00211) |   ├─ObjectSyntax
//@[096:00097) |   | ├─Token(LeftBrace) |{|
//@[097:00098) |   | ├─Token(NewLine) |\n|
  name: 'lock-${i}'
//@[002:00019) |   | ├─ObjectPropertySyntax
//@[002:00006) |   | | ├─IdentifierSyntax
//@[002:00006) |   | | | └─Token(Identifier) |name|
//@[006:00007) |   | | ├─Token(Colon) |:|
//@[008:00019) |   | | └─StringSyntax
//@[008:00016) |   | |   ├─Token(StringLeftPiece) |'lock-${|
//@[016:00017) |   | |   ├─VariableAccessSyntax
//@[016:00017) |   | |   | └─IdentifierSyntax
//@[016:00017) |   | |   |   └─Token(Identifier) |i|
//@[017:00019) |   | |   └─Token(StringRightPiece) |}'|
//@[019:00020) |   | ├─Token(NewLine) |\n|
  properties: {
//@[002:00067) |   | ├─ObjectPropertySyntax
//@[002:00012) |   | | ├─IdentifierSyntax
//@[002:00012) |   | | | └─Token(Identifier) |properties|
//@[012:00013) |   | | ├─Token(Colon) |:|
//@[014:00067) |   | | └─ObjectSyntax
//@[014:00015) |   | |   ├─Token(LeftBrace) |{|
//@[015:00016) |   | |   ├─Token(NewLine) |\n|
    level: i == 0 ? 'CanNotDelete' : 'ReadOnly'
//@[004:00047) |   | |   ├─ObjectPropertySyntax
//@[004:00009) |   | |   | ├─IdentifierSyntax
//@[004:00009) |   | |   | | └─Token(Identifier) |level|
//@[009:00010) |   | |   | ├─Token(Colon) |:|
//@[011:00047) |   | |   | └─TernaryOperationSyntax
//@[011:00017) |   | |   |   ├─BinaryOperationSyntax
//@[011:00012) |   | |   |   | ├─VariableAccessSyntax
//@[011:00012) |   | |   |   | | └─IdentifierSyntax
//@[011:00012) |   | |   |   | |   └─Token(Identifier) |i|
//@[013:00015) |   | |   |   | ├─Token(Equals) |==|
//@[016:00017) |   | |   |   | └─IntegerLiteralSyntax
//@[016:00017) |   | |   |   |   └─Token(Integer) |0|
//@[018:00019) |   | |   |   ├─Token(Question) |?|
//@[020:00034) |   | |   |   ├─StringSyntax
//@[020:00034) |   | |   |   | └─Token(StringComplete) |'CanNotDelete'|
//@[035:00036) |   | |   |   ├─Token(Colon) |:|
//@[037:00047) |   | |   |   └─StringSyntax
//@[037:00047) |   | |   |     └─Token(StringComplete) |'ReadOnly'|
//@[047:00048) |   | |   ├─Token(NewLine) |\n|
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
//@[000:00236) ├─ResourceDeclarationSyntax
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
resource lockTheLocks 'Microsoft.Authorization/locks@2016-09-01' = [for i in range(0,1): {
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00021) | ├─IdentifierSyntax
//@[009:00021) | | └─Token(Identifier) |lockTheLocks|
//@[022:00064) | ├─StringSyntax
//@[022:00064) | | └─Token(StringComplete) |'Microsoft.Authorization/locks@2016-09-01'|
//@[065:00066) | ├─Token(Assignment) |=|
//@[067:00222) | └─ForSyntax
//@[067:00068) |   ├─Token(LeftSquare) |[|
//@[068:00071) |   ├─Token(Identifier) |for|
//@[072:00073) |   ├─LocalVariableSyntax
//@[072:00073) |   | └─IdentifierSyntax
//@[072:00073) |   |   └─Token(Identifier) |i|
//@[074:00076) |   ├─Token(Identifier) |in|
//@[077:00087) |   ├─FunctionCallSyntax
//@[077:00082) |   | ├─IdentifierSyntax
//@[077:00082) |   | | └─Token(Identifier) |range|
//@[082:00083) |   | ├─Token(LeftParen) |(|
//@[083:00084) |   | ├─FunctionArgumentSyntax
//@[083:00084) |   | | └─IntegerLiteralSyntax
//@[083:00084) |   | |   └─Token(Integer) |0|
//@[084:00085) |   | ├─Token(Comma) |,|
//@[085:00086) |   | ├─FunctionArgumentSyntax
//@[085:00086) |   | | └─IntegerLiteralSyntax
//@[085:00086) |   | |   └─Token(Integer) |1|
//@[086:00087) |   | └─Token(RightParen) |)|
//@[087:00088) |   ├─Token(Colon) |:|
//@[089:00221) |   ├─ObjectSyntax
//@[089:00090) |   | ├─Token(LeftBrace) |{|
//@[090:00091) |   | ├─Token(NewLine) |\n|
  name: 'lock-the-lock-${i}'
//@[002:00028) |   | ├─ObjectPropertySyntax
//@[002:00006) |   | | ├─IdentifierSyntax
//@[002:00006) |   | | | └─Token(Identifier) |name|
//@[006:00007) |   | | ├─Token(Colon) |:|
//@[008:00028) |   | | └─StringSyntax
//@[008:00025) |   | |   ├─Token(StringLeftPiece) |'lock-the-lock-${|
//@[025:00026) |   | |   ├─VariableAccessSyntax
//@[025:00026) |   | |   | └─IdentifierSyntax
//@[025:00026) |   | |   |   └─Token(Identifier) |i|
//@[026:00028) |   | |   └─Token(StringRightPiece) |}'|
//@[028:00029) |   | ├─Token(NewLine) |\n|
  properties: {
//@[002:00067) |   | ├─ObjectPropertySyntax
//@[002:00012) |   | | ├─IdentifierSyntax
//@[002:00012) |   | | | └─Token(Identifier) |properties|
//@[012:00013) |   | | ├─Token(Colon) |:|
//@[014:00067) |   | | └─ObjectSyntax
//@[014:00015) |   | |   ├─Token(LeftBrace) |{|
//@[015:00016) |   | |   ├─Token(NewLine) |\n|
    level: i == 0 ? 'CanNotDelete' : 'ReadOnly'
//@[004:00047) |   | |   ├─ObjectPropertySyntax
//@[004:00009) |   | |   | ├─IdentifierSyntax
//@[004:00009) |   | |   | | └─Token(Identifier) |level|
//@[009:00010) |   | |   | ├─Token(Colon) |:|
//@[011:00047) |   | |   | └─TernaryOperationSyntax
//@[011:00017) |   | |   |   ├─BinaryOperationSyntax
//@[011:00012) |   | |   |   | ├─VariableAccessSyntax
//@[011:00012) |   | |   |   | | └─IdentifierSyntax
//@[011:00012) |   | |   |   | |   └─Token(Identifier) |i|
//@[013:00015) |   | |   |   | ├─Token(Equals) |==|
//@[016:00017) |   | |   |   | └─IntegerLiteralSyntax
//@[016:00017) |   | |   |   |   └─Token(Integer) |0|
//@[018:00019) |   | |   |   ├─Token(Question) |?|
//@[020:00034) |   | |   |   ├─StringSyntax
//@[020:00034) |   | |   |   | └─Token(StringComplete) |'CanNotDelete'|
//@[035:00036) |   | |   |   ├─Token(Colon) |:|
//@[037:00047) |   | |   |   └─StringSyntax
//@[037:00047) |   | |   |     └─Token(StringComplete) |'ReadOnly'|
//@[047:00048) |   | |   ├─Token(NewLine) |\n|
  }
//@[002:00003) |   | |   └─Token(RightBrace) |}|
//@[003:00004) |   | ├─Token(NewLine) |\n|
  scope: extensionCollection[i]
//@[002:00031) |   | ├─ObjectPropertySyntax
//@[002:00007) |   | | ├─IdentifierSyntax
//@[002:00007) |   | | | └─Token(Identifier) |scope|
//@[007:00008) |   | | ├─Token(Colon) |:|
//@[009:00031) |   | | └─ArrayAccessSyntax
//@[009:00028) |   | |   ├─VariableAccessSyntax
//@[009:00028) |   | |   | └─IdentifierSyntax
//@[009:00028) |   | |   |   └─Token(Identifier) |extensionCollection|
//@[028:00029) |   | |   ├─Token(LeftSquare) |[|
//@[029:00030) |   | |   ├─VariableAccessSyntax
//@[029:00030) |   | |   | └─IdentifierSyntax
//@[029:00030) |   | |   |   └─Token(Identifier) |i|
//@[030:00031) |   | |   └─Token(RightSquare) |]|
//@[031:00032) |   | ├─Token(NewLine) |\n|
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
resource storageAccounts2 'Microsoft.Storage/storageAccounts@2019-06-01' = [for account in accounts: {
//@[000:00276) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00025) | ├─IdentifierSyntax
//@[009:00025) | | └─Token(Identifier) |storageAccounts2|
//@[026:00072) | ├─StringSyntax
//@[026:00072) | | └─Token(StringComplete) |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[073:00074) | ├─Token(Assignment) |=|
//@[075:00276) | └─ForSyntax
//@[075:00076) |   ├─Token(LeftSquare) |[|
//@[076:00079) |   ├─Token(Identifier) |for|
//@[080:00087) |   ├─LocalVariableSyntax
//@[080:00087) |   | └─IdentifierSyntax
//@[080:00087) |   |   └─Token(Identifier) |account|
//@[088:00090) |   ├─Token(Identifier) |in|
//@[091:00099) |   ├─VariableAccessSyntax
//@[091:00099) |   | └─IdentifierSyntax
//@[091:00099) |   |   └─Token(Identifier) |accounts|
//@[099:00100) |   ├─Token(Colon) |:|
//@[101:00275) |   ├─ObjectSyntax
//@[101:00102) |   | ├─Token(LeftBrace) |{|
//@[102:00103) |   | ├─Token(NewLine) |\n|
  name: '${name}-collection-${account.name}'
//@[002:00044) |   | ├─ObjectPropertySyntax
//@[002:00006) |   | | ├─IdentifierSyntax
//@[002:00006) |   | | | └─Token(Identifier) |name|
//@[006:00007) |   | | ├─Token(Colon) |:|
//@[008:00044) |   | | └─StringSyntax
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
//@[042:00044) |   | |   └─Token(StringRightPiece) |}'|
//@[044:00045) |   | ├─Token(NewLine) |\n|
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
resource firstSet 'Microsoft.Storage/storageAccounts@2019-06-01' = [for i in range(0, length(accounts)): {
//@[000:00232) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00017) | ├─IdentifierSyntax
//@[009:00017) | | └─Token(Identifier) |firstSet|
//@[018:00064) | ├─StringSyntax
//@[018:00064) | | └─Token(StringComplete) |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[065:00066) | ├─Token(Assignment) |=|
//@[067:00232) | └─ForSyntax
//@[067:00068) |   ├─Token(LeftSquare) |[|
//@[068:00071) |   ├─Token(Identifier) |for|
//@[072:00073) |   ├─LocalVariableSyntax
//@[072:00073) |   | └─IdentifierSyntax
//@[072:00073) |   |   └─Token(Identifier) |i|
//@[074:00076) |   ├─Token(Identifier) |in|
//@[077:00103) |   ├─FunctionCallSyntax
//@[077:00082) |   | ├─IdentifierSyntax
//@[077:00082) |   | | └─Token(Identifier) |range|
//@[082:00083) |   | ├─Token(LeftParen) |(|
//@[083:00084) |   | ├─FunctionArgumentSyntax
//@[083:00084) |   | | └─IntegerLiteralSyntax
//@[083:00084) |   | |   └─Token(Integer) |0|
//@[084:00085) |   | ├─Token(Comma) |,|
//@[086:00102) |   | ├─FunctionArgumentSyntax
//@[086:00102) |   | | └─FunctionCallSyntax
//@[086:00092) |   | |   ├─IdentifierSyntax
//@[086:00092) |   | |   | └─Token(Identifier) |length|
//@[092:00093) |   | |   ├─Token(LeftParen) |(|
//@[093:00101) |   | |   ├─FunctionArgumentSyntax
//@[093:00101) |   | |   | └─VariableAccessSyntax
//@[093:00101) |   | |   |   └─IdentifierSyntax
//@[093:00101) |   | |   |     └─Token(Identifier) |accounts|
//@[101:00102) |   | |   └─Token(RightParen) |)|
//@[102:00103) |   | └─Token(RightParen) |)|
//@[103:00104) |   ├─Token(Colon) |:|
//@[105:00231) |   ├─ObjectSyntax
//@[105:00106) |   | ├─Token(LeftBrace) |{|
//@[106:00107) |   | ├─Token(NewLine) |\n|
  name: '${name}-set1-${i}'
//@[002:00027) |   | ├─ObjectPropertySyntax
//@[002:00006) |   | | ├─IdentifierSyntax
//@[002:00006) |   | | | └─Token(Identifier) |name|
//@[006:00007) |   | | ├─Token(Colon) |:|
//@[008:00027) |   | | └─StringSyntax
//@[008:00011) |   | |   ├─Token(StringLeftPiece) |'${|
//@[011:00015) |   | |   ├─VariableAccessSyntax
//@[011:00015) |   | |   | └─IdentifierSyntax
//@[011:00015) |   | |   |   └─Token(Identifier) |name|
//@[015:00024) |   | |   ├─Token(StringMiddlePiece) |}-set1-${|
//@[024:00025) |   | |   ├─VariableAccessSyntax
//@[024:00025) |   | |   | └─IdentifierSyntax
//@[024:00025) |   | |   |   └─Token(Identifier) |i|
//@[025:00027) |   | |   └─Token(StringRightPiece) |}'|
//@[027:00028) |   | ├─Token(NewLine) |\n|
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

resource secondSet 'Microsoft.Storage/storageAccounts@2019-06-01' = [for i in range(0, length(accounts)): {
//@[000:00268) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00018) | ├─IdentifierSyntax
//@[009:00018) | | └─Token(Identifier) |secondSet|
//@[019:00065) | ├─StringSyntax
//@[019:00065) | | └─Token(StringComplete) |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[066:00067) | ├─Token(Assignment) |=|
//@[068:00268) | └─ForSyntax
//@[068:00069) |   ├─Token(LeftSquare) |[|
//@[069:00072) |   ├─Token(Identifier) |for|
//@[073:00074) |   ├─LocalVariableSyntax
//@[073:00074) |   | └─IdentifierSyntax
//@[073:00074) |   |   └─Token(Identifier) |i|
//@[075:00077) |   ├─Token(Identifier) |in|
//@[078:00104) |   ├─FunctionCallSyntax
//@[078:00083) |   | ├─IdentifierSyntax
//@[078:00083) |   | | └─Token(Identifier) |range|
//@[083:00084) |   | ├─Token(LeftParen) |(|
//@[084:00085) |   | ├─FunctionArgumentSyntax
//@[084:00085) |   | | └─IntegerLiteralSyntax
//@[084:00085) |   | |   └─Token(Integer) |0|
//@[085:00086) |   | ├─Token(Comma) |,|
//@[087:00103) |   | ├─FunctionArgumentSyntax
//@[087:00103) |   | | └─FunctionCallSyntax
//@[087:00093) |   | |   ├─IdentifierSyntax
//@[087:00093) |   | |   | └─Token(Identifier) |length|
//@[093:00094) |   | |   ├─Token(LeftParen) |(|
//@[094:00102) |   | |   ├─FunctionArgumentSyntax
//@[094:00102) |   | |   | └─VariableAccessSyntax
//@[094:00102) |   | |   |   └─IdentifierSyntax
//@[094:00102) |   | |   |     └─Token(Identifier) |accounts|
//@[102:00103) |   | |   └─Token(RightParen) |)|
//@[103:00104) |   | └─Token(RightParen) |)|
//@[104:00105) |   ├─Token(Colon) |:|
//@[106:00267) |   ├─ObjectSyntax
//@[106:00107) |   | ├─Token(LeftBrace) |{|
//@[107:00108) |   | ├─Token(NewLine) |\n|
  name: '${name}-set2-${i}'
//@[002:00027) |   | ├─ObjectPropertySyntax
//@[002:00006) |   | | ├─IdentifierSyntax
//@[002:00006) |   | | | └─Token(Identifier) |name|
//@[006:00007) |   | | ├─Token(Colon) |:|
//@[008:00027) |   | | └─StringSyntax
//@[008:00011) |   | |   ├─Token(StringLeftPiece) |'${|
//@[011:00015) |   | |   ├─VariableAccessSyntax
//@[011:00015) |   | |   | └─IdentifierSyntax
//@[011:00015) |   | |   |   └─Token(Identifier) |name|
//@[015:00024) |   | |   ├─Token(StringMiddlePiece) |}-set2-${|
//@[024:00025) |   | |   ├─VariableAccessSyntax
//@[024:00025) |   | |   | └─IdentifierSyntax
//@[024:00025) |   | |   |   └─Token(Identifier) |i|
//@[025:00027) |   | |   └─Token(StringRightPiece) |}'|
//@[027:00028) |   | ├─Token(NewLine) |\n|
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
//@[002:00034) |   | ├─ObjectPropertySyntax
//@[002:00011) |   | | ├─IdentifierSyntax
//@[002:00011) |   | | | └─Token(Identifier) |dependsOn|
//@[011:00012) |   | | ├─Token(Colon) |:|
//@[013:00034) |   | | └─ArraySyntax
//@[013:00014) |   | |   ├─Token(LeftSquare) |[|
//@[014:00015) |   | |   ├─Token(NewLine) |\n|
    firstSet[i]
//@[004:00015) |   | |   ├─ArrayItemSyntax
//@[004:00015) |   | |   | └─ArrayAccessSyntax
//@[004:00012) |   | |   |   ├─VariableAccessSyntax
//@[004:00012) |   | |   |   | └─IdentifierSyntax
//@[004:00012) |   | |   |   |   └─Token(Identifier) |firstSet|
//@[012:00013) |   | |   |   ├─Token(LeftSquare) |[|
//@[013:00014) |   | |   |   ├─VariableAccessSyntax
//@[013:00014) |   | |   |   | └─IdentifierSyntax
//@[013:00014) |   | |   |   |   └─Token(Identifier) |i|
//@[014:00015) |   | |   |   └─Token(RightSquare) |]|
//@[015:00016) |   | |   ├─Token(NewLine) |\n|
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

resource vnets 'Microsoft.Network/virtualNetworks@2020-06-01' = [for vnetConfig in vnetConfigurations: {
//@[000:00163) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00014) | ├─IdentifierSyntax
//@[009:00014) | | └─Token(Identifier) |vnets|
//@[015:00061) | ├─StringSyntax
//@[015:00061) | | └─Token(StringComplete) |'Microsoft.Network/virtualNetworks@2020-06-01'|
//@[062:00063) | ├─Token(Assignment) |=|
//@[064:00163) | └─ForSyntax
//@[064:00065) |   ├─Token(LeftSquare) |[|
//@[065:00068) |   ├─Token(Identifier) |for|
//@[069:00079) |   ├─LocalVariableSyntax
//@[069:00079) |   | └─IdentifierSyntax
//@[069:00079) |   |   └─Token(Identifier) |vnetConfig|
//@[080:00082) |   ├─Token(Identifier) |in|
//@[083:00101) |   ├─VariableAccessSyntax
//@[083:00101) |   | └─IdentifierSyntax
//@[083:00101) |   |   └─Token(Identifier) |vnetConfigurations|
//@[101:00102) |   ├─Token(Colon) |:|
//@[103:00162) |   ├─ObjectSyntax
//@[103:00104) |   | ├─Token(LeftBrace) |{|
//@[104:00105) |   | ├─Token(NewLine) |\n|
  name: vnetConfig.name
//@[002:00023) |   | ├─ObjectPropertySyntax
//@[002:00006) |   | | ├─IdentifierSyntax
//@[002:00006) |   | | | └─Token(Identifier) |name|
//@[006:00007) |   | | ├─Token(Colon) |:|
//@[008:00023) |   | | └─PropertyAccessSyntax
//@[008:00018) |   | |   ├─VariableAccessSyntax
//@[008:00018) |   | |   | └─IdentifierSyntax
//@[008:00018) |   | |   |   └─Token(Identifier) |vnetConfig|
//@[018:00019) |   | |   ├─Token(Dot) |.|
//@[019:00023) |   | |   └─IdentifierSyntax
//@[019:00023) |   | |     └─Token(Identifier) |name|
//@[023:00024) |   | ├─Token(NewLine) |\n|
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
//@[000:00242) ├─ModuleDeclarationSyntax
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
module moduleCollectionWithSingleDependency 'passthrough.bicep' = [for moduleName in moduleSetup: {
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00043) | ├─IdentifierSyntax
//@[007:00043) | | └─Token(Identifier) |moduleCollectionWithSingleDependency|
//@[044:00063) | ├─StringSyntax
//@[044:00063) | | └─Token(StringComplete) |'passthrough.bicep'|
//@[064:00065) | ├─Token(Assignment) |=|
//@[066:00224) | └─ForSyntax
//@[066:00067) |   ├─Token(LeftSquare) |[|
//@[067:00070) |   ├─Token(Identifier) |for|
//@[071:00081) |   ├─LocalVariableSyntax
//@[071:00081) |   | └─IdentifierSyntax
//@[071:00081) |   |   └─Token(Identifier) |moduleName|
//@[082:00084) |   ├─Token(Identifier) |in|
//@[085:00096) |   ├─VariableAccessSyntax
//@[085:00096) |   | └─IdentifierSyntax
//@[085:00096) |   |   └─Token(Identifier) |moduleSetup|
//@[096:00097) |   ├─Token(Colon) |:|
//@[098:00223) |   ├─ObjectSyntax
//@[098:00099) |   | ├─Token(LeftBrace) |{|
//@[099:00100) |   | ├─Token(NewLine) |\n|
  name: moduleName
//@[002:00018) |   | ├─ObjectPropertySyntax
//@[002:00006) |   | | ├─IdentifierSyntax
//@[002:00006) |   | | | └─Token(Identifier) |name|
//@[006:00007) |   | | ├─Token(Colon) |:|
//@[008:00018) |   | | └─VariableAccessSyntax
//@[008:00018) |   | |   └─IdentifierSyntax
//@[008:00018) |   | |     └─Token(Identifier) |moduleName|
//@[018:00019) |   | ├─Token(NewLine) |\n|
  params: {
//@[002:00047) |   | ├─ObjectPropertySyntax
//@[002:00008) |   | | ├─IdentifierSyntax
//@[002:00008) |   | | | └─Token(Identifier) |params|
//@[008:00009) |   | | ├─Token(Colon) |:|
//@[010:00047) |   | | └─ObjectSyntax
//@[010:00011) |   | |   ├─Token(LeftBrace) |{|
//@[011:00012) |   | |   ├─Token(NewLine) |\n|
    myInput: 'in-${moduleName}'
//@[004:00031) |   | |   ├─ObjectPropertySyntax
//@[004:00011) |   | |   | ├─IdentifierSyntax
//@[004:00011) |   | |   | | └─Token(Identifier) |myInput|
//@[011:00012) |   | |   | ├─Token(Colon) |:|
//@[013:00031) |   | |   | └─StringSyntax
//@[013:00019) |   | |   |   ├─Token(StringLeftPiece) |'in-${|
//@[019:00029) |   | |   |   ├─VariableAccessSyntax
//@[019:00029) |   | |   |   | └─IdentifierSyntax
//@[019:00029) |   | |   |   |   └─Token(Identifier) |moduleName|
//@[029:00031) |   | |   |   └─Token(StringRightPiece) |}'|
//@[031:00032) |   | |   ├─Token(NewLine) |\n|
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
module moduleCollectionWithCollectionDependencies 'passthrough.bicep' = [for moduleName in moduleSetup: {
//@[000:00255) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00049) | ├─IdentifierSyntax
//@[007:00049) | | └─Token(Identifier) |moduleCollectionWithCollectionDependencies|
//@[050:00069) | ├─StringSyntax
//@[050:00069) | | └─Token(StringComplete) |'passthrough.bicep'|
//@[070:00071) | ├─Token(Assignment) |=|
//@[072:00255) | └─ForSyntax
//@[072:00073) |   ├─Token(LeftSquare) |[|
//@[073:00076) |   ├─Token(Identifier) |for|
//@[077:00087) |   ├─LocalVariableSyntax
//@[077:00087) |   | └─IdentifierSyntax
//@[077:00087) |   |   └─Token(Identifier) |moduleName|
//@[088:00090) |   ├─Token(Identifier) |in|
//@[091:00102) |   ├─VariableAccessSyntax
//@[091:00102) |   | └─IdentifierSyntax
//@[091:00102) |   |   └─Token(Identifier) |moduleSetup|
//@[102:00103) |   ├─Token(Colon) |:|
//@[104:00254) |   ├─ObjectSyntax
//@[104:00105) |   | ├─Token(LeftBrace) |{|
//@[105:00106) |   | ├─Token(NewLine) |\n|
  name: moduleName
//@[002:00018) |   | ├─ObjectPropertySyntax
//@[002:00006) |   | | ├─IdentifierSyntax
//@[002:00006) |   | | | └─Token(Identifier) |name|
//@[006:00007) |   | | ├─Token(Colon) |:|
//@[008:00018) |   | | └─VariableAccessSyntax
//@[008:00018) |   | |   └─IdentifierSyntax
//@[008:00018) |   | |     └─Token(Identifier) |moduleName|
//@[018:00019) |   | ├─Token(NewLine) |\n|
  params: {
//@[002:00047) |   | ├─ObjectPropertySyntax
//@[002:00008) |   | | ├─IdentifierSyntax
//@[002:00008) |   | | | └─Token(Identifier) |params|
//@[008:00009) |   | | ├─Token(Colon) |:|
//@[010:00047) |   | | └─ObjectSyntax
//@[010:00011) |   | |   ├─Token(LeftBrace) |{|
//@[011:00012) |   | |   ├─Token(NewLine) |\n|
    myInput: 'in-${moduleName}'
//@[004:00031) |   | |   ├─ObjectPropertySyntax
//@[004:00011) |   | |   | ├─IdentifierSyntax
//@[004:00011) |   | |   | | └─Token(Identifier) |myInput|
//@[011:00012) |   | |   | ├─Token(Colon) |:|
//@[013:00031) |   | |   | └─StringSyntax
//@[013:00019) |   | |   |   ├─Token(StringLeftPiece) |'in-${|
//@[019:00029) |   | |   |   ├─VariableAccessSyntax
//@[019:00029) |   | |   |   | └─IdentifierSyntax
//@[019:00029) |   | |   |   |   └─Token(Identifier) |moduleName|
//@[029:00031) |   | |   |   └─Token(StringRightPiece) |}'|
//@[031:00032) |   | |   ├─Token(NewLine) |\n|
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

module moduleCollectionWithIndexedDependencies 'passthrough.bicep' = [for moduleName in moduleSetup: {
//@[000:00346) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00046) | ├─IdentifierSyntax
//@[007:00046) | | └─Token(Identifier) |moduleCollectionWithIndexedDependencies|
//@[047:00066) | ├─StringSyntax
//@[047:00066) | | └─Token(StringComplete) |'passthrough.bicep'|
//@[067:00068) | ├─Token(Assignment) |=|
//@[069:00346) | └─ForSyntax
//@[069:00070) |   ├─Token(LeftSquare) |[|
//@[070:00073) |   ├─Token(Identifier) |for|
//@[074:00084) |   ├─LocalVariableSyntax
//@[074:00084) |   | └─IdentifierSyntax
//@[074:00084) |   |   └─Token(Identifier) |moduleName|
//@[085:00087) |   ├─Token(Identifier) |in|
//@[088:00099) |   ├─VariableAccessSyntax
//@[088:00099) |   | └─IdentifierSyntax
//@[088:00099) |   |   └─Token(Identifier) |moduleSetup|
//@[099:00100) |   ├─Token(Colon) |:|
//@[101:00345) |   ├─ObjectSyntax
//@[101:00102) |   | ├─Token(LeftBrace) |{|
//@[102:00103) |   | ├─Token(NewLine) |\n|
  name: moduleName
//@[002:00018) |   | ├─ObjectPropertySyntax
//@[002:00006) |   | | ├─IdentifierSyntax
//@[002:00006) |   | | | └─Token(Identifier) |name|
//@[006:00007) |   | | ├─Token(Colon) |:|
//@[008:00018) |   | | └─VariableAccessSyntax
//@[008:00018) |   | |   └─IdentifierSyntax
//@[008:00018) |   | |     └─Token(Identifier) |moduleName|
//@[018:00019) |   | ├─Token(NewLine) |\n|
  params: {
//@[002:00170) |   | ├─ObjectPropertySyntax
//@[002:00008) |   | | ├─IdentifierSyntax
//@[002:00008) |   | | | └─Token(Identifier) |params|
//@[008:00009) |   | | ├─Token(Colon) |:|
//@[010:00170) |   | | └─ObjectSyntax
//@[010:00011) |   | |   ├─Token(LeftBrace) |{|
//@[011:00012) |   | |   ├─Token(NewLine) |\n|
    myInput: '${moduleCollectionWithCollectionDependencies[index].outputs.myOutput} - ${storageAccounts[index * 3].properties.accessTier} - ${moduleName}'
//@[004:00154) |   | |   ├─ObjectPropertySyntax
//@[004:00011) |   | |   | ├─IdentifierSyntax
//@[004:00011) |   | |   | | └─Token(Identifier) |myInput|
//@[011:00012) |   | |   | ├─Token(Colon) |:|
//@[013:00154) |   | |   | └─StringSyntax
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
//@[152:00154) |   | |   |   └─Token(StringRightPiece) |}'|
//@[154:00155) |   | |   ├─Token(NewLine) |\n|
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
resource existingStorageAccounts 'Microsoft.Storage/storageAccounts@2019-06-01' existing = [for account in accounts: {
//@[000:00164) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00032) | ├─IdentifierSyntax
//@[009:00032) | | └─Token(Identifier) |existingStorageAccounts|
//@[033:00079) | ├─StringSyntax
//@[033:00079) | | └─Token(StringComplete) |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[080:00088) | ├─Token(Identifier) |existing|
//@[089:00090) | ├─Token(Assignment) |=|
//@[091:00164) | └─ForSyntax
//@[091:00092) |   ├─Token(LeftSquare) |[|
//@[092:00095) |   ├─Token(Identifier) |for|
//@[096:00103) |   ├─LocalVariableSyntax
//@[096:00103) |   | └─IdentifierSyntax
//@[096:00103) |   |   └─Token(Identifier) |account|
//@[104:00106) |   ├─Token(Identifier) |in|
//@[107:00115) |   ├─VariableAccessSyntax
//@[107:00115) |   | └─IdentifierSyntax
//@[107:00115) |   |   └─Token(Identifier) |accounts|
//@[115:00116) |   ├─Token(Colon) |:|
//@[117:00163) |   ├─ObjectSyntax
//@[117:00118) |   | ├─Token(LeftBrace) |{|
//@[118:00119) |   | ├─Token(NewLine) |\n|
  name: '${name}-existing-${account.name}'
//@[002:00042) |   | ├─ObjectPropertySyntax
//@[002:00006) |   | | ├─IdentifierSyntax
//@[002:00006) |   | | | └─Token(Identifier) |name|
//@[006:00007) |   | | ├─Token(Colon) |:|
//@[008:00042) |   | | └─StringSyntax
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
//@[040:00042) |   | |   └─Token(StringRightPiece) |}'|
//@[042:00043) |   | ├─Token(NewLine) |\n|
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

resource duplicatedNames 'Microsoft.Network/dnsZones@2018-05-01' = [for zone in []: {
//@[000:00136) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00024) | ├─IdentifierSyntax
//@[009:00024) | | └─Token(Identifier) |duplicatedNames|
//@[025:00064) | ├─StringSyntax
//@[025:00064) | | └─Token(StringComplete) |'Microsoft.Network/dnsZones@2018-05-01'|
//@[065:00066) | ├─Token(Assignment) |=|
//@[067:00136) | └─ForSyntax
//@[067:00068) |   ├─Token(LeftSquare) |[|
//@[068:00071) |   ├─Token(Identifier) |for|
//@[072:00076) |   ├─LocalVariableSyntax
//@[072:00076) |   | └─IdentifierSyntax
//@[072:00076) |   |   └─Token(Identifier) |zone|
//@[077:00079) |   ├─Token(Identifier) |in|
//@[080:00082) |   ├─ArraySyntax
//@[080:00081) |   | ├─Token(LeftSquare) |[|
//@[081:00082) |   | └─Token(RightSquare) |]|
//@[082:00083) |   ├─Token(Colon) |:|
//@[084:00135) |   ├─ObjectSyntax
//@[084:00085) |   | ├─Token(LeftBrace) |{|
//@[085:00086) |   | ├─Token(NewLine) |\n|
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
resource referenceToDuplicateNames 'Microsoft.Network/dnsZones@2018-05-01' = [for zone in []: {
//@[000:00194) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00034) | ├─IdentifierSyntax
//@[009:00034) | | └─Token(Identifier) |referenceToDuplicateNames|
//@[035:00074) | ├─StringSyntax
//@[035:00074) | | └─Token(StringComplete) |'Microsoft.Network/dnsZones@2018-05-01'|
//@[075:00076) | ├─Token(Assignment) |=|
//@[077:00194) | └─ForSyntax
//@[077:00078) |   ├─Token(LeftSquare) |[|
//@[078:00081) |   ├─Token(Identifier) |for|
//@[082:00086) |   ├─LocalVariableSyntax
//@[082:00086) |   | └─IdentifierSyntax
//@[082:00086) |   |   └─Token(Identifier) |zone|
//@[087:00089) |   ├─Token(Identifier) |in|
//@[090:00092) |   ├─ArraySyntax
//@[090:00091) |   | ├─Token(LeftSquare) |[|
//@[091:00092) |   | └─Token(RightSquare) |]|
//@[092:00093) |   ├─Token(Colon) |:|
//@[094:00193) |   ├─ObjectSyntax
//@[094:00095) |   | ├─Token(LeftBrace) |{|
//@[095:00096) |   | ├─Token(NewLine) |\n|
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

module apim 'passthrough.bicep' = [for region in regions: {
//@[000:00131) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00011) | ├─IdentifierSyntax
//@[007:00011) | | └─Token(Identifier) |apim|
//@[012:00031) | ├─StringSyntax
//@[012:00031) | | └─Token(StringComplete) |'passthrough.bicep'|
//@[032:00033) | ├─Token(Assignment) |=|
//@[034:00131) | └─ForSyntax
//@[034:00035) |   ├─Token(LeftSquare) |[|
//@[035:00038) |   ├─Token(Identifier) |for|
//@[039:00045) |   ├─LocalVariableSyntax
//@[039:00045) |   | └─IdentifierSyntax
//@[039:00045) |   |   └─Token(Identifier) |region|
//@[046:00048) |   ├─Token(Identifier) |in|
//@[049:00056) |   ├─VariableAccessSyntax
//@[049:00056) |   | └─IdentifierSyntax
//@[049:00056) |   |   └─Token(Identifier) |regions|
//@[056:00057) |   ├─Token(Colon) |:|
//@[058:00130) |   ├─ObjectSyntax
//@[058:00059) |   | ├─Token(LeftBrace) |{|
//@[059:00060) |   | ├─Token(NewLine) |\n|
  name: 'apim-${region}-${name}'
//@[002:00032) |   | ├─ObjectPropertySyntax
//@[002:00006) |   | | ├─IdentifierSyntax
//@[002:00006) |   | | | └─Token(Identifier) |name|
//@[006:00007) |   | | ├─Token(Colon) |:|
//@[008:00032) |   | | └─StringSyntax
//@[008:00016) |   | |   ├─Token(StringLeftPiece) |'apim-${|
//@[016:00022) |   | |   ├─VariableAccessSyntax
//@[016:00022) |   | |   | └─IdentifierSyntax
//@[016:00022) |   | |   |   └─Token(Identifier) |region|
//@[022:00026) |   | |   ├─Token(StringMiddlePiece) |}-${|
//@[026:00030) |   | |   ├─VariableAccessSyntax
//@[026:00030) |   | |   | └─IdentifierSyntax
//@[026:00030) |   | |   |   └─Token(Identifier) |name|
//@[030:00032) |   | |   └─Token(StringRightPiece) |}'|
//@[032:00033) |   | ├─Token(NewLine) |\n|
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
//@[000:00780) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00049) | ├─IdentifierSyntax
//@[009:00049) | | └─Token(Identifier) |propertyLoopDependencyOnModuleCollection|
//@[050:00091) | ├─StringSyntax
//@[050:00091) | | └─Token(StringComplete) |'Microsoft.Network/frontDoors@2020-05-01'|
//@[092:00093) | ├─Token(Assignment) |=|
//@[094:00780) | └─ObjectSyntax
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
//@[002:00648) |   ├─ObjectPropertySyntax
//@[002:00012) |   | ├─IdentifierSyntax
//@[002:00012) |   | | └─Token(Identifier) |properties|
//@[012:00013) |   | ├─Token(Colon) |:|
//@[014:00648) |   | └─ObjectSyntax
//@[014:00015) |   |   ├─Token(LeftBrace) |{|
//@[015:00016) |   |   ├─Token(NewLine) |\n|
    backendPools: [
//@[004:00628) |   |   ├─ObjectPropertySyntax
//@[004:00016) |   |   | ├─IdentifierSyntax
//@[004:00016) |   |   | | └─Token(Identifier) |backendPools|
//@[016:00017) |   |   | ├─Token(Colon) |:|
//@[018:00628) |   |   | └─ArraySyntax
//@[018:00019) |   |   |   ├─Token(LeftSquare) |[|
//@[019:00020) |   |   |   ├─Token(NewLine) |\n|
      {
//@[006:00602) |   |   |   ├─ArrayItemSyntax
//@[006:00602) |   |   |   | └─ObjectSyntax
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
//@[008:00557) |   |   |   |   ├─ObjectPropertySyntax
//@[008:00018) |   |   |   |   | ├─IdentifierSyntax
//@[008:00018) |   |   |   |   | | └─Token(Identifier) |properties|
//@[018:00019) |   |   |   |   | ├─Token(Colon) |:|
//@[020:00557) |   |   |   |   | └─ObjectSyntax
//@[020:00021) |   |   |   |   |   ├─Token(LeftBrace) |{|
//@[021:00022) |   |   |   |   |   ├─Token(NewLine) |\n|
          backends: [for index in range(0, length(regions)): {
//@[010:00525) |   |   |   |   |   ├─ObjectPropertySyntax
//@[010:00018) |   |   |   |   |   | ├─IdentifierSyntax
//@[010:00018) |   |   |   |   |   | | └─Token(Identifier) |backends|
//@[018:00019) |   |   |   |   |   | ├─Token(Colon) |:|
//@[020:00525) |   |   |   |   |   | └─ForSyntax
//@[020:00021) |   |   |   |   |   |   ├─Token(LeftSquare) |[|
//@[021:00024) |   |   |   |   |   |   ├─Token(Identifier) |for|
//@[025:00030) |   |   |   |   |   |   ├─LocalVariableSyntax
//@[025:00030) |   |   |   |   |   |   | └─IdentifierSyntax
//@[025:00030) |   |   |   |   |   |   |   └─Token(Identifier) |index|
//@[031:00033) |   |   |   |   |   |   ├─Token(Identifier) |in|
//@[034:00059) |   |   |   |   |   |   ├─FunctionCallSyntax
//@[034:00039) |   |   |   |   |   |   | ├─IdentifierSyntax
//@[034:00039) |   |   |   |   |   |   | | └─Token(Identifier) |range|
//@[039:00040) |   |   |   |   |   |   | ├─Token(LeftParen) |(|
//@[040:00041) |   |   |   |   |   |   | ├─FunctionArgumentSyntax
//@[040:00041) |   |   |   |   |   |   | | └─IntegerLiteralSyntax
//@[040:00041) |   |   |   |   |   |   | |   └─Token(Integer) |0|
//@[041:00042) |   |   |   |   |   |   | ├─Token(Comma) |,|
//@[043:00058) |   |   |   |   |   |   | ├─FunctionArgumentSyntax
//@[043:00058) |   |   |   |   |   |   | | └─FunctionCallSyntax
//@[043:00049) |   |   |   |   |   |   | |   ├─IdentifierSyntax
//@[043:00049) |   |   |   |   |   |   | |   | └─Token(Identifier) |length|
//@[049:00050) |   |   |   |   |   |   | |   ├─Token(LeftParen) |(|
//@[050:00057) |   |   |   |   |   |   | |   ├─FunctionArgumentSyntax
//@[050:00057) |   |   |   |   |   |   | |   | └─VariableAccessSyntax
//@[050:00057) |   |   |   |   |   |   | |   |   └─IdentifierSyntax
//@[050:00057) |   |   |   |   |   |   | |   |     └─Token(Identifier) |regions|
//@[057:00058) |   |   |   |   |   |   | |   └─Token(RightParen) |)|
//@[058:00059) |   |   |   |   |   |   | └─Token(RightParen) |)|
//@[059:00060) |   |   |   |   |   |   ├─Token(Colon) |:|
//@[061:00524) |   |   |   |   |   |   ├─ObjectSyntax
//@[061:00062) |   |   |   |   |   |   | ├─Token(LeftBrace) |{|
//@[062:00063) |   |   |   |   |   |   | ├─Token(NewLine) |\n|
            // we cannot codegen index correctly because the generated dependsOn property
//@[089:00090) |   |   |   |   |   |   | ├─Token(NewLine) |\n|
            // would be outside of the scope of the property loop
//@[065:00066) |   |   |   |   |   |   | ├─Token(NewLine) |\n|
            // as a result, this will generate a dependency on the entire collection
//@[084:00085) |   |   |   |   |   |   | ├─Token(NewLine) |\n|
            address: apim[index].outputs.myOutput
//@[012:00049) |   |   |   |   |   |   | ├─ObjectPropertySyntax
//@[012:00019) |   |   |   |   |   |   | | ├─IdentifierSyntax
//@[012:00019) |   |   |   |   |   |   | | | └─Token(Identifier) |address|
//@[019:00020) |   |   |   |   |   |   | | ├─Token(Colon) |:|
//@[021:00049) |   |   |   |   |   |   | | └─PropertyAccessSyntax
//@[021:00040) |   |   |   |   |   |   | |   ├─PropertyAccessSyntax
//@[021:00032) |   |   |   |   |   |   | |   | ├─ArrayAccessSyntax
//@[021:00025) |   |   |   |   |   |   | |   | | ├─VariableAccessSyntax
//@[021:00025) |   |   |   |   |   |   | |   | | | └─IdentifierSyntax
//@[021:00025) |   |   |   |   |   |   | |   | | |   └─Token(Identifier) |apim|
//@[025:00026) |   |   |   |   |   |   | |   | | ├─Token(LeftSquare) |[|
//@[026:00031) |   |   |   |   |   |   | |   | | ├─VariableAccessSyntax
//@[026:00031) |   |   |   |   |   |   | |   | | | └─IdentifierSyntax
//@[026:00031) |   |   |   |   |   |   | |   | | |   └─Token(Identifier) |index|
//@[031:00032) |   |   |   |   |   |   | |   | | └─Token(RightSquare) |]|
//@[032:00033) |   |   |   |   |   |   | |   | ├─Token(Dot) |.|
//@[033:00040) |   |   |   |   |   |   | |   | └─IdentifierSyntax
//@[033:00040) |   |   |   |   |   |   | |   |   └─Token(Identifier) |outputs|
//@[040:00041) |   |   |   |   |   |   | |   ├─Token(Dot) |.|
//@[041:00049) |   |   |   |   |   |   | |   └─IdentifierSyntax
//@[041:00049) |   |   |   |   |   |   | |     └─Token(Identifier) |myOutput|
//@[049:00050) |   |   |   |   |   |   | ├─Token(NewLine) |\n|
            backendHostHeader: apim[index].outputs.myOutput
//@[012:00059) |   |   |   |   |   |   | ├─ObjectPropertySyntax
//@[012:00029) |   |   |   |   |   |   | | ├─IdentifierSyntax
//@[012:00029) |   |   |   |   |   |   | | | └─Token(Identifier) |backendHostHeader|
//@[029:00030) |   |   |   |   |   |   | | ├─Token(Colon) |:|
//@[031:00059) |   |   |   |   |   |   | | └─PropertyAccessSyntax
//@[031:00050) |   |   |   |   |   |   | |   ├─PropertyAccessSyntax
//@[031:00042) |   |   |   |   |   |   | |   | ├─ArrayAccessSyntax
//@[031:00035) |   |   |   |   |   |   | |   | | ├─VariableAccessSyntax
//@[031:00035) |   |   |   |   |   |   | |   | | | └─IdentifierSyntax
//@[031:00035) |   |   |   |   |   |   | |   | | |   └─Token(Identifier) |apim|
//@[035:00036) |   |   |   |   |   |   | |   | | ├─Token(LeftSquare) |[|
//@[036:00041) |   |   |   |   |   |   | |   | | ├─VariableAccessSyntax
//@[036:00041) |   |   |   |   |   |   | |   | | | └─IdentifierSyntax
//@[036:00041) |   |   |   |   |   |   | |   | | |   └─Token(Identifier) |index|
//@[041:00042) |   |   |   |   |   |   | |   | | └─Token(RightSquare) |]|
//@[042:00043) |   |   |   |   |   |   | |   | ├─Token(Dot) |.|
//@[043:00050) |   |   |   |   |   |   | |   | └─IdentifierSyntax
//@[043:00050) |   |   |   |   |   |   | |   |   └─Token(Identifier) |outputs|
//@[050:00051) |   |   |   |   |   |   | |   ├─Token(Dot) |.|
//@[051:00059) |   |   |   |   |   |   | |   └─IdentifierSyntax
//@[051:00059) |   |   |   |   |   |   | |     └─Token(Identifier) |myOutput|
//@[059:00060) |   |   |   |   |   |   | ├─Token(NewLine) |\n|
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

resource indexedModuleCollectionDependency 'Microsoft.Network/frontDoors@2020-05-01' = [for index in range(0, length(regions)): {
//@[000:00757) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00042) | ├─IdentifierSyntax
//@[009:00042) | | └─Token(Identifier) |indexedModuleCollectionDependency|
//@[043:00084) | ├─StringSyntax
//@[043:00084) | | └─Token(StringComplete) |'Microsoft.Network/frontDoors@2020-05-01'|
//@[085:00086) | ├─Token(Assignment) |=|
//@[087:00757) | └─ForSyntax
//@[087:00088) |   ├─Token(LeftSquare) |[|
//@[088:00091) |   ├─Token(Identifier) |for|
//@[092:00097) |   ├─LocalVariableSyntax
//@[092:00097) |   | └─IdentifierSyntax
//@[092:00097) |   |   └─Token(Identifier) |index|
//@[098:00100) |   ├─Token(Identifier) |in|
//@[101:00126) |   ├─FunctionCallSyntax
//@[101:00106) |   | ├─IdentifierSyntax
//@[101:00106) |   | | └─Token(Identifier) |range|
//@[106:00107) |   | ├─Token(LeftParen) |(|
//@[107:00108) |   | ├─FunctionArgumentSyntax
//@[107:00108) |   | | └─IntegerLiteralSyntax
//@[107:00108) |   | |   └─Token(Integer) |0|
//@[108:00109) |   | ├─Token(Comma) |,|
//@[110:00125) |   | ├─FunctionArgumentSyntax
//@[110:00125) |   | | └─FunctionCallSyntax
//@[110:00116) |   | |   ├─IdentifierSyntax
//@[110:00116) |   | |   | └─Token(Identifier) |length|
//@[116:00117) |   | |   ├─Token(LeftParen) |(|
//@[117:00124) |   | |   ├─FunctionArgumentSyntax
//@[117:00124) |   | |   | └─VariableAccessSyntax
//@[117:00124) |   | |   |   └─IdentifierSyntax
//@[117:00124) |   | |   |     └─Token(Identifier) |regions|
//@[124:00125) |   | |   └─Token(RightParen) |)|
//@[125:00126) |   | └─Token(RightParen) |)|
//@[126:00127) |   ├─Token(Colon) |:|
//@[128:00756) |   ├─ObjectSyntax
//@[128:00129) |   | ├─Token(LeftBrace) |{|
//@[129:00130) |   | ├─Token(NewLine) |\n|
  name: '${name}-${index}'
//@[002:00026) |   | ├─ObjectPropertySyntax
//@[002:00006) |   | | ├─IdentifierSyntax
//@[002:00006) |   | | | └─Token(Identifier) |name|
//@[006:00007) |   | | ├─Token(Colon) |:|
//@[008:00026) |   | | └─StringSyntax
//@[008:00011) |   | |   ├─Token(StringLeftPiece) |'${|
//@[011:00015) |   | |   ├─VariableAccessSyntax
//@[011:00015) |   | |   | └─IdentifierSyntax
//@[011:00015) |   | |   |   └─Token(Identifier) |name|
//@[015:00019) |   | |   ├─Token(StringMiddlePiece) |}-${|
//@[019:00024) |   | |   ├─VariableAccessSyntax
//@[019:00024) |   | |   | └─IdentifierSyntax
//@[019:00024) |   | |   |   └─Token(Identifier) |index|
//@[024:00026) |   | |   └─Token(StringRightPiece) |}'|
//@[026:00027) |   | ├─Token(NewLine) |\n|
  location: 'Global'
//@[002:00020) |   | ├─ObjectPropertySyntax
//@[002:00010) |   | | ├─IdentifierSyntax
//@[002:00010) |   | | | └─Token(Identifier) |location|
//@[010:00011) |   | | ├─Token(Colon) |:|
//@[012:00020) |   | | └─StringSyntax
//@[012:00020) |   | |   └─Token(StringComplete) |'Global'|
//@[020:00021) |   | ├─Token(NewLine) |\n|
  properties: {
//@[002:00576) |   | ├─ObjectPropertySyntax
//@[002:00012) |   | | ├─IdentifierSyntax
//@[002:00012) |   | | | └─Token(Identifier) |properties|
//@[012:00013) |   | | ├─Token(Colon) |:|
//@[014:00576) |   | | └─ObjectSyntax
//@[014:00015) |   | |   ├─Token(LeftBrace) |{|
//@[015:00016) |   | |   ├─Token(NewLine) |\n|
    backendPools: [
//@[004:00556) |   | |   ├─ObjectPropertySyntax
//@[004:00016) |   | |   | ├─IdentifierSyntax
//@[004:00016) |   | |   | | └─Token(Identifier) |backendPools|
//@[016:00017) |   | |   | ├─Token(Colon) |:|
//@[018:00556) |   | |   | └─ArraySyntax
//@[018:00019) |   | |   |   ├─Token(LeftSquare) |[|
//@[019:00020) |   | |   |   ├─Token(NewLine) |\n|
      {
//@[006:00530) |   | |   |   ├─ArrayItemSyntax
//@[006:00530) |   | |   |   | └─ObjectSyntax
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
//@[008:00485) |   | |   |   |   ├─ObjectPropertySyntax
//@[008:00018) |   | |   |   |   | ├─IdentifierSyntax
//@[008:00018) |   | |   |   |   | | └─Token(Identifier) |properties|
//@[018:00019) |   | |   |   |   | ├─Token(Colon) |:|
//@[020:00485) |   | |   |   |   | └─ObjectSyntax
//@[020:00021) |   | |   |   |   |   ├─Token(LeftBrace) |{|
//@[021:00022) |   | |   |   |   |   ├─Token(NewLine) |\n|
          backends: [
//@[010:00453) |   | |   |   |   |   ├─ObjectPropertySyntax
//@[010:00018) |   | |   |   |   |   | ├─IdentifierSyntax
//@[010:00018) |   | |   |   |   |   | | └─Token(Identifier) |backends|
//@[018:00019) |   | |   |   |   |   | ├─Token(Colon) |:|
//@[020:00453) |   | |   |   |   |   | └─ArraySyntax
//@[020:00021) |   | |   |   |   |   |   ├─Token(LeftSquare) |[|
//@[021:00022) |   | |   |   |   |   |   ├─Token(NewLine) |\n|
            {
//@[012:00419) |   | |   |   |   |   |   ├─ArrayItemSyntax
//@[012:00419) |   | |   |   |   |   |   | └─ObjectSyntax
//@[012:00013) |   | |   |   |   |   |   |   ├─Token(LeftBrace) |{|
//@[013:00014) |   | |   |   |   |   |   |   ├─Token(NewLine) |\n|
              // this indexed dependency on a module collection will be generated correctly because
//@[099:00100) |   | |   |   |   |   |   |   ├─Token(NewLine) |\n|
              // copyIndex() can be invoked in the generated dependsOn
//@[070:00071) |   | |   |   |   |   |   |   ├─Token(NewLine) |\n|
              address: apim[index].outputs.myOutput
//@[014:00051) |   | |   |   |   |   |   |   ├─ObjectPropertySyntax
//@[014:00021) |   | |   |   |   |   |   |   | ├─IdentifierSyntax
//@[014:00021) |   | |   |   |   |   |   |   | | └─Token(Identifier) |address|
//@[021:00022) |   | |   |   |   |   |   |   | ├─Token(Colon) |:|
//@[023:00051) |   | |   |   |   |   |   |   | └─PropertyAccessSyntax
//@[023:00042) |   | |   |   |   |   |   |   |   ├─PropertyAccessSyntax
//@[023:00034) |   | |   |   |   |   |   |   |   | ├─ArrayAccessSyntax
//@[023:00027) |   | |   |   |   |   |   |   |   | | ├─VariableAccessSyntax
//@[023:00027) |   | |   |   |   |   |   |   |   | | | └─IdentifierSyntax
//@[023:00027) |   | |   |   |   |   |   |   |   | | |   └─Token(Identifier) |apim|
//@[027:00028) |   | |   |   |   |   |   |   |   | | ├─Token(LeftSquare) |[|
//@[028:00033) |   | |   |   |   |   |   |   |   | | ├─VariableAccessSyntax
//@[028:00033) |   | |   |   |   |   |   |   |   | | | └─IdentifierSyntax
//@[028:00033) |   | |   |   |   |   |   |   |   | | |   └─Token(Identifier) |index|
//@[033:00034) |   | |   |   |   |   |   |   |   | | └─Token(RightSquare) |]|
//@[034:00035) |   | |   |   |   |   |   |   |   | ├─Token(Dot) |.|
//@[035:00042) |   | |   |   |   |   |   |   |   | └─IdentifierSyntax
//@[035:00042) |   | |   |   |   |   |   |   |   |   └─Token(Identifier) |outputs|
//@[042:00043) |   | |   |   |   |   |   |   |   ├─Token(Dot) |.|
//@[043:00051) |   | |   |   |   |   |   |   |   └─IdentifierSyntax
//@[043:00051) |   | |   |   |   |   |   |   |     └─Token(Identifier) |myOutput|
//@[051:00052) |   | |   |   |   |   |   |   ├─Token(NewLine) |\n|
              backendHostHeader: apim[index].outputs.myOutput
//@[014:00061) |   | |   |   |   |   |   |   ├─ObjectPropertySyntax
//@[014:00031) |   | |   |   |   |   |   |   | ├─IdentifierSyntax
//@[014:00031) |   | |   |   |   |   |   |   | | └─Token(Identifier) |backendHostHeader|
//@[031:00032) |   | |   |   |   |   |   |   | ├─Token(Colon) |:|
//@[033:00061) |   | |   |   |   |   |   |   | └─PropertyAccessSyntax
//@[033:00052) |   | |   |   |   |   |   |   |   ├─PropertyAccessSyntax
//@[033:00044) |   | |   |   |   |   |   |   |   | ├─ArrayAccessSyntax
//@[033:00037) |   | |   |   |   |   |   |   |   | | ├─VariableAccessSyntax
//@[033:00037) |   | |   |   |   |   |   |   |   | | | └─IdentifierSyntax
//@[033:00037) |   | |   |   |   |   |   |   |   | | |   └─Token(Identifier) |apim|
//@[037:00038) |   | |   |   |   |   |   |   |   | | ├─Token(LeftSquare) |[|
//@[038:00043) |   | |   |   |   |   |   |   |   | | ├─VariableAccessSyntax
//@[038:00043) |   | |   |   |   |   |   |   |   | | | └─IdentifierSyntax
//@[038:00043) |   | |   |   |   |   |   |   |   | | |   └─Token(Identifier) |index|
//@[043:00044) |   | |   |   |   |   |   |   |   | | └─Token(RightSquare) |]|
//@[044:00045) |   | |   |   |   |   |   |   |   | ├─Token(Dot) |.|
//@[045:00052) |   | |   |   |   |   |   |   |   | └─IdentifierSyntax
//@[045:00052) |   | |   |   |   |   |   |   |   |   └─Token(Identifier) |outputs|
//@[052:00053) |   | |   |   |   |   |   |   |   ├─Token(Dot) |.|
//@[053:00061) |   | |   |   |   |   |   |   |   └─IdentifierSyntax
//@[053:00061) |   | |   |   |   |   |   |   |     └─Token(Identifier) |myOutput|
//@[061:00062) |   | |   |   |   |   |   |   ├─Token(NewLine) |\n|
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

resource indexedResourceCollectionDependency 'Microsoft.Network/frontDoors@2020-05-01' = [for index in range(0, length(accounts)): {
//@[000:00848) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00044) | ├─IdentifierSyntax
//@[009:00044) | | └─Token(Identifier) |indexedResourceCollectionDependency|
//@[045:00086) | ├─StringSyntax
//@[045:00086) | | └─Token(StringComplete) |'Microsoft.Network/frontDoors@2020-05-01'|
//@[087:00088) | ├─Token(Assignment) |=|
//@[089:00848) | └─ForSyntax
//@[089:00090) |   ├─Token(LeftSquare) |[|
//@[090:00093) |   ├─Token(Identifier) |for|
//@[094:00099) |   ├─LocalVariableSyntax
//@[094:00099) |   | └─IdentifierSyntax
//@[094:00099) |   |   └─Token(Identifier) |index|
//@[100:00102) |   ├─Token(Identifier) |in|
//@[103:00129) |   ├─FunctionCallSyntax
//@[103:00108) |   | ├─IdentifierSyntax
//@[103:00108) |   | | └─Token(Identifier) |range|
//@[108:00109) |   | ├─Token(LeftParen) |(|
//@[109:00110) |   | ├─FunctionArgumentSyntax
//@[109:00110) |   | | └─IntegerLiteralSyntax
//@[109:00110) |   | |   └─Token(Integer) |0|
//@[110:00111) |   | ├─Token(Comma) |,|
//@[112:00128) |   | ├─FunctionArgumentSyntax
//@[112:00128) |   | | └─FunctionCallSyntax
//@[112:00118) |   | |   ├─IdentifierSyntax
//@[112:00118) |   | |   | └─Token(Identifier) |length|
//@[118:00119) |   | |   ├─Token(LeftParen) |(|
//@[119:00127) |   | |   ├─FunctionArgumentSyntax
//@[119:00127) |   | |   | └─VariableAccessSyntax
//@[119:00127) |   | |   |   └─IdentifierSyntax
//@[119:00127) |   | |   |     └─Token(Identifier) |accounts|
//@[127:00128) |   | |   └─Token(RightParen) |)|
//@[128:00129) |   | └─Token(RightParen) |)|
//@[129:00130) |   ├─Token(Colon) |:|
//@[131:00847) |   ├─ObjectSyntax
//@[131:00132) |   | ├─Token(LeftBrace) |{|
//@[132:00133) |   | ├─Token(NewLine) |\n|
  name: '${name}-${index}'
//@[002:00026) |   | ├─ObjectPropertySyntax
//@[002:00006) |   | | ├─IdentifierSyntax
//@[002:00006) |   | | | └─Token(Identifier) |name|
//@[006:00007) |   | | ├─Token(Colon) |:|
//@[008:00026) |   | | └─StringSyntax
//@[008:00011) |   | |   ├─Token(StringLeftPiece) |'${|
//@[011:00015) |   | |   ├─VariableAccessSyntax
//@[011:00015) |   | |   | └─IdentifierSyntax
//@[011:00015) |   | |   |   └─Token(Identifier) |name|
//@[015:00019) |   | |   ├─Token(StringMiddlePiece) |}-${|
//@[019:00024) |   | |   ├─VariableAccessSyntax
//@[019:00024) |   | |   | └─IdentifierSyntax
//@[019:00024) |   | |   |   └─Token(Identifier) |index|
//@[024:00026) |   | |   └─Token(StringRightPiece) |}'|
//@[026:00027) |   | ├─Token(NewLine) |\n|
  location: 'Global'
//@[002:00020) |   | ├─ObjectPropertySyntax
//@[002:00010) |   | | ├─IdentifierSyntax
//@[002:00010) |   | | | └─Token(Identifier) |location|
//@[010:00011) |   | | ├─Token(Colon) |:|
//@[012:00020) |   | | └─StringSyntax
//@[012:00020) |   | |   └─Token(StringComplete) |'Global'|
//@[020:00021) |   | ├─Token(NewLine) |\n|
  properties: {
//@[002:00664) |   | ├─ObjectPropertySyntax
//@[002:00012) |   | | ├─IdentifierSyntax
//@[002:00012) |   | | | └─Token(Identifier) |properties|
//@[012:00013) |   | | ├─Token(Colon) |:|
//@[014:00664) |   | | └─ObjectSyntax
//@[014:00015) |   | |   ├─Token(LeftBrace) |{|
//@[015:00016) |   | |   ├─Token(NewLine) |\n|
    backendPools: [
//@[004:00644) |   | |   ├─ObjectPropertySyntax
//@[004:00016) |   | |   | ├─IdentifierSyntax
//@[004:00016) |   | |   | | └─Token(Identifier) |backendPools|
//@[016:00017) |   | |   | ├─Token(Colon) |:|
//@[018:00644) |   | |   | └─ArraySyntax
//@[018:00019) |   | |   |   ├─Token(LeftSquare) |[|
//@[019:00020) |   | |   |   ├─Token(NewLine) |\n|
      {
//@[006:00618) |   | |   |   ├─ArrayItemSyntax
//@[006:00618) |   | |   |   | └─ObjectSyntax
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
//@[008:00573) |   | |   |   |   ├─ObjectPropertySyntax
//@[008:00018) |   | |   |   |   | ├─IdentifierSyntax
//@[008:00018) |   | |   |   |   | | └─Token(Identifier) |properties|
//@[018:00019) |   | |   |   |   | ├─Token(Colon) |:|
//@[020:00573) |   | |   |   |   | └─ObjectSyntax
//@[020:00021) |   | |   |   |   |   ├─Token(LeftBrace) |{|
//@[021:00022) |   | |   |   |   |   ├─Token(NewLine) |\n|
          backends: [
//@[010:00541) |   | |   |   |   |   ├─ObjectPropertySyntax
//@[010:00018) |   | |   |   |   |   | ├─IdentifierSyntax
//@[010:00018) |   | |   |   |   |   | | └─Token(Identifier) |backends|
//@[018:00019) |   | |   |   |   |   | ├─Token(Colon) |:|
//@[020:00541) |   | |   |   |   |   | └─ArraySyntax
//@[020:00021) |   | |   |   |   |   |   ├─Token(LeftSquare) |[|
//@[021:00022) |   | |   |   |   |   |   ├─Token(NewLine) |\n|
            {
//@[012:00507) |   | |   |   |   |   |   ├─ArrayItemSyntax
//@[012:00507) |   | |   |   |   |   |   | └─ObjectSyntax
//@[012:00013) |   | |   |   |   |   |   |   ├─Token(LeftBrace) |{|
//@[013:00014) |   | |   |   |   |   |   |   ├─Token(NewLine) |\n|
              // this indexed dependency on a module collection will be generated correctly because
//@[099:00100) |   | |   |   |   |   |   |   ├─Token(NewLine) |\n|
              // copyIndex() can be invoked in the generated dependsOn
//@[070:00071) |   | |   |   |   |   |   |   ├─Token(NewLine) |\n|
              address: storageAccounts[index].properties.primaryEndpoints.internetEndpoints.web
//@[014:00095) |   | |   |   |   |   |   |   ├─ObjectPropertySyntax
//@[014:00021) |   | |   |   |   |   |   |   | ├─IdentifierSyntax
//@[014:00021) |   | |   |   |   |   |   |   | | └─Token(Identifier) |address|
//@[021:00022) |   | |   |   |   |   |   |   | ├─Token(Colon) |:|
//@[023:00095) |   | |   |   |   |   |   |   | └─PropertyAccessSyntax
//@[023:00091) |   | |   |   |   |   |   |   |   ├─PropertyAccessSyntax
//@[023:00073) |   | |   |   |   |   |   |   |   | ├─PropertyAccessSyntax
//@[023:00056) |   | |   |   |   |   |   |   |   | | ├─PropertyAccessSyntax
//@[023:00045) |   | |   |   |   |   |   |   |   | | | ├─ArrayAccessSyntax
//@[023:00038) |   | |   |   |   |   |   |   |   | | | | ├─VariableAccessSyntax
//@[023:00038) |   | |   |   |   |   |   |   |   | | | | | └─IdentifierSyntax
//@[023:00038) |   | |   |   |   |   |   |   |   | | | | |   └─Token(Identifier) |storageAccounts|
//@[038:00039) |   | |   |   |   |   |   |   |   | | | | ├─Token(LeftSquare) |[|
//@[039:00044) |   | |   |   |   |   |   |   |   | | | | ├─VariableAccessSyntax
//@[039:00044) |   | |   |   |   |   |   |   |   | | | | | └─IdentifierSyntax
//@[039:00044) |   | |   |   |   |   |   |   |   | | | | |   └─Token(Identifier) |index|
//@[044:00045) |   | |   |   |   |   |   |   |   | | | | └─Token(RightSquare) |]|
//@[045:00046) |   | |   |   |   |   |   |   |   | | | ├─Token(Dot) |.|
//@[046:00056) |   | |   |   |   |   |   |   |   | | | └─IdentifierSyntax
//@[046:00056) |   | |   |   |   |   |   |   |   | | |   └─Token(Identifier) |properties|
//@[056:00057) |   | |   |   |   |   |   |   |   | | ├─Token(Dot) |.|
//@[057:00073) |   | |   |   |   |   |   |   |   | | └─IdentifierSyntax
//@[057:00073) |   | |   |   |   |   |   |   |   | |   └─Token(Identifier) |primaryEndpoints|
//@[073:00074) |   | |   |   |   |   |   |   |   | ├─Token(Dot) |.|
//@[074:00091) |   | |   |   |   |   |   |   |   | └─IdentifierSyntax
//@[074:00091) |   | |   |   |   |   |   |   |   |   └─Token(Identifier) |internetEndpoints|
//@[091:00092) |   | |   |   |   |   |   |   |   ├─Token(Dot) |.|
//@[092:00095) |   | |   |   |   |   |   |   |   └─IdentifierSyntax
//@[092:00095) |   | |   |   |   |   |   |   |     └─Token(Identifier) |web|
//@[095:00096) |   | |   |   |   |   |   |   ├─Token(NewLine) |\n|
              backendHostHeader: storageAccounts[index].properties.primaryEndpoints.internetEndpoints.web
//@[014:00105) |   | |   |   |   |   |   |   ├─ObjectPropertySyntax
//@[014:00031) |   | |   |   |   |   |   |   | ├─IdentifierSyntax
//@[014:00031) |   | |   |   |   |   |   |   | | └─Token(Identifier) |backendHostHeader|
//@[031:00032) |   | |   |   |   |   |   |   | ├─Token(Colon) |:|
//@[033:00105) |   | |   |   |   |   |   |   | └─PropertyAccessSyntax
//@[033:00101) |   | |   |   |   |   |   |   |   ├─PropertyAccessSyntax
//@[033:00083) |   | |   |   |   |   |   |   |   | ├─PropertyAccessSyntax
//@[033:00066) |   | |   |   |   |   |   |   |   | | ├─PropertyAccessSyntax
//@[033:00055) |   | |   |   |   |   |   |   |   | | | ├─ArrayAccessSyntax
//@[033:00048) |   | |   |   |   |   |   |   |   | | | | ├─VariableAccessSyntax
//@[033:00048) |   | |   |   |   |   |   |   |   | | | | | └─IdentifierSyntax
//@[033:00048) |   | |   |   |   |   |   |   |   | | | | |   └─Token(Identifier) |storageAccounts|
//@[048:00049) |   | |   |   |   |   |   |   |   | | | | ├─Token(LeftSquare) |[|
//@[049:00054) |   | |   |   |   |   |   |   |   | | | | ├─VariableAccessSyntax
//@[049:00054) |   | |   |   |   |   |   |   |   | | | | | └─IdentifierSyntax
//@[049:00054) |   | |   |   |   |   |   |   |   | | | | |   └─Token(Identifier) |index|
//@[054:00055) |   | |   |   |   |   |   |   |   | | | | └─Token(RightSquare) |]|
//@[055:00056) |   | |   |   |   |   |   |   |   | | | ├─Token(Dot) |.|
//@[056:00066) |   | |   |   |   |   |   |   |   | | | └─IdentifierSyntax
//@[056:00066) |   | |   |   |   |   |   |   |   | | |   └─Token(Identifier) |properties|
//@[066:00067) |   | |   |   |   |   |   |   |   | | ├─Token(Dot) |.|
//@[067:00083) |   | |   |   |   |   |   |   |   | | └─IdentifierSyntax
//@[067:00083) |   | |   |   |   |   |   |   |   | |   └─Token(Identifier) |primaryEndpoints|
//@[083:00084) |   | |   |   |   |   |   |   |   | ├─Token(Dot) |.|
//@[084:00101) |   | |   |   |   |   |   |   |   | └─IdentifierSyntax
//@[084:00101) |   | |   |   |   |   |   |   |   |   └─Token(Identifier) |internetEndpoints|
//@[101:00102) |   | |   |   |   |   |   |   |   ├─Token(Dot) |.|
//@[102:00105) |   | |   |   |   |   |   |   |   └─IdentifierSyntax
//@[102:00105) |   | |   |   |   |   |   |   |     └─Token(Identifier) |web|
//@[105:00106) |   | |   |   |   |   |   |   ├─Token(NewLine) |\n|
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

resource filteredZones 'Microsoft.Network/dnsZones@2018-05-01' = [for i in range(0,10): if(i % 3 == 0) {
//@[000:00163) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00022) | ├─IdentifierSyntax
//@[009:00022) | | └─Token(Identifier) |filteredZones|
//@[023:00062) | ├─StringSyntax
//@[023:00062) | | └─Token(StringComplete) |'Microsoft.Network/dnsZones@2018-05-01'|
//@[063:00064) | ├─Token(Assignment) |=|
//@[065:00163) | └─ForSyntax
//@[065:00066) |   ├─Token(LeftSquare) |[|
//@[066:00069) |   ├─Token(Identifier) |for|
//@[070:00071) |   ├─LocalVariableSyntax
//@[070:00071) |   | └─IdentifierSyntax
//@[070:00071) |   |   └─Token(Identifier) |i|
//@[072:00074) |   ├─Token(Identifier) |in|
//@[075:00086) |   ├─FunctionCallSyntax
//@[075:00080) |   | ├─IdentifierSyntax
//@[075:00080) |   | | └─Token(Identifier) |range|
//@[080:00081) |   | ├─Token(LeftParen) |(|
//@[081:00082) |   | ├─FunctionArgumentSyntax
//@[081:00082) |   | | └─IntegerLiteralSyntax
//@[081:00082) |   | |   └─Token(Integer) |0|
//@[082:00083) |   | ├─Token(Comma) |,|
//@[083:00085) |   | ├─FunctionArgumentSyntax
//@[083:00085) |   | | └─IntegerLiteralSyntax
//@[083:00085) |   | |   └─Token(Integer) |10|
//@[085:00086) |   | └─Token(RightParen) |)|
//@[086:00087) |   ├─Token(Colon) |:|
//@[088:00162) |   ├─IfConditionSyntax
//@[088:00090) |   | ├─Token(Identifier) |if|
//@[090:00102) |   | ├─ParenthesizedExpressionSyntax
//@[090:00091) |   | | ├─Token(LeftParen) |(|
//@[091:00101) |   | | ├─BinaryOperationSyntax
//@[091:00096) |   | | | ├─BinaryOperationSyntax
//@[091:00092) |   | | | | ├─VariableAccessSyntax
//@[091:00092) |   | | | | | └─IdentifierSyntax
//@[091:00092) |   | | | | |   └─Token(Identifier) |i|
//@[093:00094) |   | | | | ├─Token(Modulo) |%|
//@[095:00096) |   | | | | └─IntegerLiteralSyntax
//@[095:00096) |   | | | |   └─Token(Integer) |3|
//@[097:00099) |   | | | ├─Token(Equals) |==|
//@[100:00101) |   | | | └─IntegerLiteralSyntax
//@[100:00101) |   | | |   └─Token(Integer) |0|
//@[101:00102) |   | | └─Token(RightParen) |)|
//@[103:00162) |   | └─ObjectSyntax
//@[103:00104) |   |   ├─Token(LeftBrace) |{|
//@[104:00105) |   |   ├─Token(NewLine) |\n|
  name: 'zone${i}'
//@[002:00018) |   |   ├─ObjectPropertySyntax
//@[002:00006) |   |   | ├─IdentifierSyntax
//@[002:00006) |   |   | | └─Token(Identifier) |name|
//@[006:00007) |   |   | ├─Token(Colon) |:|
//@[008:00018) |   |   | └─StringSyntax
//@[008:00015) |   |   |   ├─Token(StringLeftPiece) |'zone${|
//@[015:00016) |   |   |   ├─VariableAccessSyntax
//@[015:00016) |   |   |   | └─IdentifierSyntax
//@[015:00016) |   |   |   |   └─Token(Identifier) |i|
//@[016:00018) |   |   |   └─Token(StringRightPiece) |}'|
//@[018:00019) |   |   ├─Token(NewLine) |\n|
  location: resourceGroup().location
//@[002:00036) |   |   ├─ObjectPropertySyntax
//@[002:00010) |   |   | ├─IdentifierSyntax
//@[002:00010) |   |   | | └─Token(Identifier) |location|
//@[010:00011) |   |   | ├─Token(Colon) |:|
//@[012:00036) |   |   | └─PropertyAccessSyntax
//@[012:00027) |   |   |   ├─FunctionCallSyntax
//@[012:00025) |   |   |   | ├─IdentifierSyntax
//@[012:00025) |   |   |   | | └─Token(Identifier) |resourceGroup|
//@[025:00026) |   |   |   | ├─Token(LeftParen) |(|
//@[026:00027) |   |   |   | └─Token(RightParen) |)|
//@[027:00028) |   |   |   ├─Token(Dot) |.|
//@[028:00036) |   |   |   └─IdentifierSyntax
//@[028:00036) |   |   |     └─Token(Identifier) |location|
//@[036:00037) |   |   ├─Token(NewLine) |\n|
}]
//@[000:00001) |   |   └─Token(RightBrace) |}|
//@[001:00002) |   └─Token(RightSquare) |]|
//@[002:00004) ├─Token(NewLine) |\n\n|

module filteredModules 'passthrough.bicep' = [for i in range(0,6): if(i % 2 == 0) {
//@[000:00149) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00022) | ├─IdentifierSyntax
//@[007:00022) | | └─Token(Identifier) |filteredModules|
//@[023:00042) | ├─StringSyntax
//@[023:00042) | | └─Token(StringComplete) |'passthrough.bicep'|
//@[043:00044) | ├─Token(Assignment) |=|
//@[045:00149) | └─ForSyntax
//@[045:00046) |   ├─Token(LeftSquare) |[|
//@[046:00049) |   ├─Token(Identifier) |for|
//@[050:00051) |   ├─LocalVariableSyntax
//@[050:00051) |   | └─IdentifierSyntax
//@[050:00051) |   |   └─Token(Identifier) |i|
//@[052:00054) |   ├─Token(Identifier) |in|
//@[055:00065) |   ├─FunctionCallSyntax
//@[055:00060) |   | ├─IdentifierSyntax
//@[055:00060) |   | | └─Token(Identifier) |range|
//@[060:00061) |   | ├─Token(LeftParen) |(|
//@[061:00062) |   | ├─FunctionArgumentSyntax
//@[061:00062) |   | | └─IntegerLiteralSyntax
//@[061:00062) |   | |   └─Token(Integer) |0|
//@[062:00063) |   | ├─Token(Comma) |,|
//@[063:00064) |   | ├─FunctionArgumentSyntax
//@[063:00064) |   | | └─IntegerLiteralSyntax
//@[063:00064) |   | |   └─Token(Integer) |6|
//@[064:00065) |   | └─Token(RightParen) |)|
//@[065:00066) |   ├─Token(Colon) |:|
//@[067:00148) |   ├─IfConditionSyntax
//@[067:00069) |   | ├─Token(Identifier) |if|
//@[069:00081) |   | ├─ParenthesizedExpressionSyntax
//@[069:00070) |   | | ├─Token(LeftParen) |(|
//@[070:00080) |   | | ├─BinaryOperationSyntax
//@[070:00075) |   | | | ├─BinaryOperationSyntax
//@[070:00071) |   | | | | ├─VariableAccessSyntax
//@[070:00071) |   | | | | | └─IdentifierSyntax
//@[070:00071) |   | | | | |   └─Token(Identifier) |i|
//@[072:00073) |   | | | | ├─Token(Modulo) |%|
//@[074:00075) |   | | | | └─IntegerLiteralSyntax
//@[074:00075) |   | | | |   └─Token(Integer) |2|
//@[076:00078) |   | | | ├─Token(Equals) |==|
//@[079:00080) |   | | | └─IntegerLiteralSyntax
//@[079:00080) |   | | |   └─Token(Integer) |0|
//@[080:00081) |   | | └─Token(RightParen) |)|
//@[082:00148) |   | └─ObjectSyntax
//@[082:00083) |   |   ├─Token(LeftBrace) |{|
//@[083:00084) |   |   ├─Token(NewLine) |\n|
  name: 'stuff${i}'
//@[002:00019) |   |   ├─ObjectPropertySyntax
//@[002:00006) |   |   | ├─IdentifierSyntax
//@[002:00006) |   |   | | └─Token(Identifier) |name|
//@[006:00007) |   |   | ├─Token(Colon) |:|
//@[008:00019) |   |   | └─StringSyntax
//@[008:00016) |   |   |   ├─Token(StringLeftPiece) |'stuff${|
//@[016:00017) |   |   |   ├─VariableAccessSyntax
//@[016:00017) |   |   |   | └─IdentifierSyntax
//@[016:00017) |   |   |   |   └─Token(Identifier) |i|
//@[017:00019) |   |   |   └─Token(StringRightPiece) |}'|
//@[019:00020) |   |   ├─Token(NewLine) |\n|
  params: {
//@[002:00042) |   |   ├─ObjectPropertySyntax
//@[002:00008) |   |   | ├─IdentifierSyntax
//@[002:00008) |   |   | | └─Token(Identifier) |params|
//@[008:00009) |   |   | ├─Token(Colon) |:|
//@[010:00042) |   |   | └─ObjectSyntax
//@[010:00011) |   |   |   ├─Token(LeftBrace) |{|
//@[011:00012) |   |   |   ├─Token(NewLine) |\n|
    myInput: 'script-${i}'
//@[004:00026) |   |   |   ├─ObjectPropertySyntax
//@[004:00011) |   |   |   | ├─IdentifierSyntax
//@[004:00011) |   |   |   | | └─Token(Identifier) |myInput|
//@[011:00012) |   |   |   | ├─Token(Colon) |:|
//@[013:00026) |   |   |   | └─StringSyntax
//@[013:00023) |   |   |   |   ├─Token(StringLeftPiece) |'script-${|
//@[023:00024) |   |   |   |   ├─VariableAccessSyntax
//@[023:00024) |   |   |   |   | └─IdentifierSyntax
//@[023:00024) |   |   |   |   |   └─Token(Identifier) |i|
//@[024:00026) |   |   |   |   └─Token(StringRightPiece) |}'|
//@[026:00027) |   |   |   ├─Token(NewLine) |\n|
  }
//@[002:00003) |   |   |   └─Token(RightBrace) |}|
//@[003:00004) |   |   ├─Token(NewLine) |\n|
}]
//@[000:00001) |   |   └─Token(RightBrace) |}|
//@[001:00002) |   └─Token(RightSquare) |]|
//@[002:00004) ├─Token(NewLine) |\n\n|

resource filteredIndexedZones 'Microsoft.Network/dnsZones@2018-05-01' = [for (account, i) in accounts: if(account.enabled) {
//@[000:00199) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00029) | ├─IdentifierSyntax
//@[009:00029) | | └─Token(Identifier) |filteredIndexedZones|
//@[030:00069) | ├─StringSyntax
//@[030:00069) | | └─Token(StringComplete) |'Microsoft.Network/dnsZones@2018-05-01'|
//@[070:00071) | ├─Token(Assignment) |=|
//@[072:00199) | └─ForSyntax
//@[072:00073) |   ├─Token(LeftSquare) |[|
//@[073:00076) |   ├─Token(Identifier) |for|
//@[077:00089) |   ├─VariableBlockSyntax
//@[077:00078) |   | ├─Token(LeftParen) |(|
//@[078:00085) |   | ├─LocalVariableSyntax
//@[078:00085) |   | | └─IdentifierSyntax
//@[078:00085) |   | |   └─Token(Identifier) |account|
//@[085:00086) |   | ├─Token(Comma) |,|
//@[087:00088) |   | ├─LocalVariableSyntax
//@[087:00088) |   | | └─IdentifierSyntax
//@[087:00088) |   | |   └─Token(Identifier) |i|
//@[088:00089) |   | └─Token(RightParen) |)|
//@[090:00092) |   ├─Token(Identifier) |in|
//@[093:00101) |   ├─VariableAccessSyntax
//@[093:00101) |   | └─IdentifierSyntax
//@[093:00101) |   |   └─Token(Identifier) |accounts|
//@[101:00102) |   ├─Token(Colon) |:|
//@[103:00198) |   ├─IfConditionSyntax
//@[103:00105) |   | ├─Token(Identifier) |if|
//@[105:00122) |   | ├─ParenthesizedExpressionSyntax
//@[105:00106) |   | | ├─Token(LeftParen) |(|
//@[106:00121) |   | | ├─PropertyAccessSyntax
//@[106:00113) |   | | | ├─VariableAccessSyntax
//@[106:00113) |   | | | | └─IdentifierSyntax
//@[106:00113) |   | | | |   └─Token(Identifier) |account|
//@[113:00114) |   | | | ├─Token(Dot) |.|
//@[114:00121) |   | | | └─IdentifierSyntax
//@[114:00121) |   | | |   └─Token(Identifier) |enabled|
//@[121:00122) |   | | └─Token(RightParen) |)|
//@[123:00198) |   | └─ObjectSyntax
//@[123:00124) |   |   ├─Token(LeftBrace) |{|
//@[124:00125) |   |   ├─Token(NewLine) |\n|
  name: 'indexedZone-${account.name}-${i}'
//@[002:00042) |   |   ├─ObjectPropertySyntax
//@[002:00006) |   |   | ├─IdentifierSyntax
//@[002:00006) |   |   | | └─Token(Identifier) |name|
//@[006:00007) |   |   | ├─Token(Colon) |:|
//@[008:00042) |   |   | └─StringSyntax
//@[008:00023) |   |   |   ├─Token(StringLeftPiece) |'indexedZone-${|
//@[023:00035) |   |   |   ├─PropertyAccessSyntax
//@[023:00030) |   |   |   | ├─VariableAccessSyntax
//@[023:00030) |   |   |   | | └─IdentifierSyntax
//@[023:00030) |   |   |   | |   └─Token(Identifier) |account|
//@[030:00031) |   |   |   | ├─Token(Dot) |.|
//@[031:00035) |   |   |   | └─IdentifierSyntax
//@[031:00035) |   |   |   |   └─Token(Identifier) |name|
//@[035:00039) |   |   |   ├─Token(StringMiddlePiece) |}-${|
//@[039:00040) |   |   |   ├─VariableAccessSyntax
//@[039:00040) |   |   |   | └─IdentifierSyntax
//@[039:00040) |   |   |   |   └─Token(Identifier) |i|
//@[040:00042) |   |   |   └─Token(StringRightPiece) |}'|
//@[042:00043) |   |   ├─Token(NewLine) |\n|
  location: account.location
//@[002:00028) |   |   ├─ObjectPropertySyntax
//@[002:00010) |   |   | ├─IdentifierSyntax
//@[002:00010) |   |   | | └─Token(Identifier) |location|
//@[010:00011) |   |   | ├─Token(Colon) |:|
//@[012:00028) |   |   | └─PropertyAccessSyntax
//@[012:00019) |   |   |   ├─VariableAccessSyntax
//@[012:00019) |   |   |   | └─IdentifierSyntax
//@[012:00019) |   |   |   |   └─Token(Identifier) |account|
//@[019:00020) |   |   |   ├─Token(Dot) |.|
//@[020:00028) |   |   |   └─IdentifierSyntax
//@[020:00028) |   |   |     └─Token(Identifier) |location|
//@[028:00029) |   |   ├─Token(NewLine) |\n|
}]
//@[000:00001) |   |   └─Token(RightBrace) |}|
//@[001:00002) |   └─Token(RightSquare) |]|
//@[002:00004) ├─Token(NewLine) |\n\n|

output lastNameServers array = filteredIndexedZones[length(accounts) - 1].properties.nameServers
//@[000:00096) ├─OutputDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |output|
//@[007:00022) | ├─IdentifierSyntax
//@[007:00022) | | └─Token(Identifier) |lastNameServers|
//@[023:00028) | ├─SimpleTypeSyntax
//@[023:00028) | | └─Token(Identifier) |array|
//@[029:00030) | ├─Token(Assignment) |=|
//@[031:00096) | └─PropertyAccessSyntax
//@[031:00084) |   ├─PropertyAccessSyntax
//@[031:00073) |   | ├─ArrayAccessSyntax
//@[031:00051) |   | | ├─VariableAccessSyntax
//@[031:00051) |   | | | └─IdentifierSyntax
//@[031:00051) |   | | |   └─Token(Identifier) |filteredIndexedZones|
//@[051:00052) |   | | ├─Token(LeftSquare) |[|
//@[052:00072) |   | | ├─BinaryOperationSyntax
//@[052:00068) |   | | | ├─FunctionCallSyntax
//@[052:00058) |   | | | | ├─IdentifierSyntax
//@[052:00058) |   | | | | | └─Token(Identifier) |length|
//@[058:00059) |   | | | | ├─Token(LeftParen) |(|
//@[059:00067) |   | | | | ├─FunctionArgumentSyntax
//@[059:00067) |   | | | | | └─VariableAccessSyntax
//@[059:00067) |   | | | | |   └─IdentifierSyntax
//@[059:00067) |   | | | | |     └─Token(Identifier) |accounts|
//@[067:00068) |   | | | | └─Token(RightParen) |)|
//@[069:00070) |   | | | ├─Token(Minus) |-|
//@[071:00072) |   | | | └─IntegerLiteralSyntax
//@[071:00072) |   | | |   └─Token(Integer) |1|
//@[072:00073) |   | | └─Token(RightSquare) |]|
//@[073:00074) |   | ├─Token(Dot) |.|
//@[074:00084) |   | └─IdentifierSyntax
//@[074:00084) |   |   └─Token(Identifier) |properties|
//@[084:00085) |   ├─Token(Dot) |.|
//@[085:00096) |   └─IdentifierSyntax
//@[085:00096) |     └─Token(Identifier) |nameServers|
//@[096:00098) ├─Token(NewLine) |\n\n|

module filteredIndexedModules 'passthrough.bicep' = [for (account, i) in accounts: if(account.enabled) {
//@[000:00187) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00029) | ├─IdentifierSyntax
//@[007:00029) | | └─Token(Identifier) |filteredIndexedModules|
//@[030:00049) | ├─StringSyntax
//@[030:00049) | | └─Token(StringComplete) |'passthrough.bicep'|
//@[050:00051) | ├─Token(Assignment) |=|
//@[052:00187) | └─ForSyntax
//@[052:00053) |   ├─Token(LeftSquare) |[|
//@[053:00056) |   ├─Token(Identifier) |for|
//@[057:00069) |   ├─VariableBlockSyntax
//@[057:00058) |   | ├─Token(LeftParen) |(|
//@[058:00065) |   | ├─LocalVariableSyntax
//@[058:00065) |   | | └─IdentifierSyntax
//@[058:00065) |   | |   └─Token(Identifier) |account|
//@[065:00066) |   | ├─Token(Comma) |,|
//@[067:00068) |   | ├─LocalVariableSyntax
//@[067:00068) |   | | └─IdentifierSyntax
//@[067:00068) |   | |   └─Token(Identifier) |i|
//@[068:00069) |   | └─Token(RightParen) |)|
//@[070:00072) |   ├─Token(Identifier) |in|
//@[073:00081) |   ├─VariableAccessSyntax
//@[073:00081) |   | └─IdentifierSyntax
//@[073:00081) |   |   └─Token(Identifier) |accounts|
//@[081:00082) |   ├─Token(Colon) |:|
//@[083:00186) |   ├─IfConditionSyntax
//@[083:00085) |   | ├─Token(Identifier) |if|
//@[085:00102) |   | ├─ParenthesizedExpressionSyntax
//@[085:00086) |   | | ├─Token(LeftParen) |(|
//@[086:00101) |   | | ├─PropertyAccessSyntax
//@[086:00093) |   | | | ├─VariableAccessSyntax
//@[086:00093) |   | | | | └─IdentifierSyntax
//@[086:00093) |   | | | |   └─Token(Identifier) |account|
//@[093:00094) |   | | | ├─Token(Dot) |.|
//@[094:00101) |   | | | └─IdentifierSyntax
//@[094:00101) |   | | |   └─Token(Identifier) |enabled|
//@[101:00102) |   | | └─Token(RightParen) |)|
//@[103:00186) |   | └─ObjectSyntax
//@[103:00104) |   |   ├─Token(LeftBrace) |{|
//@[104:00105) |   |   ├─Token(NewLine) |\n|
  name: 'stuff-${i}'
//@[002:00020) |   |   ├─ObjectPropertySyntax
//@[002:00006) |   |   | ├─IdentifierSyntax
//@[002:00006) |   |   | | └─Token(Identifier) |name|
//@[006:00007) |   |   | ├─Token(Colon) |:|
//@[008:00020) |   |   | └─StringSyntax
//@[008:00017) |   |   |   ├─Token(StringLeftPiece) |'stuff-${|
//@[017:00018) |   |   |   ├─VariableAccessSyntax
//@[017:00018) |   |   |   | └─IdentifierSyntax
//@[017:00018) |   |   |   |   └─Token(Identifier) |i|
//@[018:00020) |   |   |   └─Token(StringRightPiece) |}'|
//@[020:00021) |   |   ├─Token(NewLine) |\n|
  params: {
//@[002:00058) |   |   ├─ObjectPropertySyntax
//@[002:00008) |   |   | ├─IdentifierSyntax
//@[002:00008) |   |   | | └─Token(Identifier) |params|
//@[008:00009) |   |   | ├─Token(Colon) |:|
//@[010:00058) |   |   | └─ObjectSyntax
//@[010:00011) |   |   |   ├─Token(LeftBrace) |{|
//@[011:00012) |   |   |   ├─Token(NewLine) |\n|
    myInput: 'script-${account.name}-${i}'
//@[004:00042) |   |   |   ├─ObjectPropertySyntax
//@[004:00011) |   |   |   | ├─IdentifierSyntax
//@[004:00011) |   |   |   | | └─Token(Identifier) |myInput|
//@[011:00012) |   |   |   | ├─Token(Colon) |:|
//@[013:00042) |   |   |   | └─StringSyntax
//@[013:00023) |   |   |   |   ├─Token(StringLeftPiece) |'script-${|
//@[023:00035) |   |   |   |   ├─PropertyAccessSyntax
//@[023:00030) |   |   |   |   | ├─VariableAccessSyntax
//@[023:00030) |   |   |   |   | | └─IdentifierSyntax
//@[023:00030) |   |   |   |   | |   └─Token(Identifier) |account|
//@[030:00031) |   |   |   |   | ├─Token(Dot) |.|
//@[031:00035) |   |   |   |   | └─IdentifierSyntax
//@[031:00035) |   |   |   |   |   └─Token(Identifier) |name|
//@[035:00039) |   |   |   |   ├─Token(StringMiddlePiece) |}-${|
//@[039:00040) |   |   |   |   ├─VariableAccessSyntax
//@[039:00040) |   |   |   |   | └─IdentifierSyntax
//@[039:00040) |   |   |   |   |   └─Token(Identifier) |i|
//@[040:00042) |   |   |   |   └─Token(StringRightPiece) |}'|
//@[042:00043) |   |   |   ├─Token(NewLine) |\n|
  }
//@[002:00003) |   |   |   └─Token(RightBrace) |}|
//@[003:00004) |   |   ├─Token(NewLine) |\n|
}]
//@[000:00001) |   |   └─Token(RightBrace) |}|
//@[001:00002) |   └─Token(RightSquare) |]|
//@[002:00004) ├─Token(NewLine) |\n\n|

output lastModuleOutput string = filteredIndexedModules[length(accounts) - 1].outputs.myOutput
//@[000:00094) ├─OutputDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |output|
//@[007:00023) | ├─IdentifierSyntax
//@[007:00023) | | └─Token(Identifier) |lastModuleOutput|
//@[024:00030) | ├─SimpleTypeSyntax
//@[024:00030) | | └─Token(Identifier) |string|
//@[031:00032) | ├─Token(Assignment) |=|
//@[033:00094) | └─PropertyAccessSyntax
//@[033:00085) |   ├─PropertyAccessSyntax
//@[033:00077) |   | ├─ArrayAccessSyntax
//@[033:00055) |   | | ├─VariableAccessSyntax
//@[033:00055) |   | | | └─IdentifierSyntax
//@[033:00055) |   | | |   └─Token(Identifier) |filteredIndexedModules|
//@[055:00056) |   | | ├─Token(LeftSquare) |[|
//@[056:00076) |   | | ├─BinaryOperationSyntax
//@[056:00072) |   | | | ├─FunctionCallSyntax
//@[056:00062) |   | | | | ├─IdentifierSyntax
//@[056:00062) |   | | | | | └─Token(Identifier) |length|
//@[062:00063) |   | | | | ├─Token(LeftParen) |(|
//@[063:00071) |   | | | | ├─FunctionArgumentSyntax
//@[063:00071) |   | | | | | └─VariableAccessSyntax
//@[063:00071) |   | | | | |   └─IdentifierSyntax
//@[063:00071) |   | | | | |     └─Token(Identifier) |accounts|
//@[071:00072) |   | | | | └─Token(RightParen) |)|
//@[073:00074) |   | | | ├─Token(Minus) |-|
//@[075:00076) |   | | | └─IntegerLiteralSyntax
//@[075:00076) |   | | |   └─Token(Integer) |1|
//@[076:00077) |   | | └─Token(RightSquare) |]|
//@[077:00078) |   | ├─Token(Dot) |.|
//@[078:00085) |   | └─IdentifierSyntax
//@[078:00085) |   |   └─Token(Identifier) |outputs|
//@[085:00086) |   ├─Token(Dot) |.|
//@[086:00094) |   └─IdentifierSyntax
//@[086:00094) |     └─Token(Identifier) |myOutput|
//@[094:00095) ├─Token(NewLine) |\n|

//@[000:00000) └─Token(EndOfFile) ||
