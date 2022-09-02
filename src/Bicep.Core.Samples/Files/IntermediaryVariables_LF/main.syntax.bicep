var boolVal = true
//@[000:1058) ProgramSyntax
//@[000:0018) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0011) | ├─IdentifierSyntax
//@[004:0011) | | └─Token(Identifier) |boolVal|
//@[012:0013) | ├─Token(Assignment) |=|
//@[014:0018) | └─BooleanLiteralSyntax
//@[014:0018) |   └─Token(TrueKeyword) |true|
//@[018:0020) ├─Token(NewLine) |\n\n|

var vmProperties = {
//@[000:0173) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0016) | ├─IdentifierSyntax
//@[004:0016) | | └─Token(Identifier) |vmProperties|
//@[017:0018) | ├─Token(Assignment) |=|
//@[019:0173) | └─ObjectSyntax
//@[019:0020) |   ├─Token(LeftBrace) |{|
//@[020:0021) |   ├─Token(NewLine) |\n|
  diagnosticsProfile: {
//@[002:0124) |   ├─ObjectPropertySyntax
//@[002:0020) |   | ├─IdentifierSyntax
//@[002:0020) |   | | └─Token(Identifier) |diagnosticsProfile|
//@[020:0021) |   | ├─Token(Colon) |:|
//@[022:0124) |   | └─ObjectSyntax
//@[022:0023) |   |   ├─Token(LeftBrace) |{|
//@[023:0024) |   |   ├─Token(NewLine) |\n|
    bootDiagnostics: {
//@[004:0096) |   |   ├─ObjectPropertySyntax
//@[004:0019) |   |   | ├─IdentifierSyntax
//@[004:0019) |   |   | | └─Token(Identifier) |bootDiagnostics|
//@[019:0020) |   |   | ├─Token(Colon) |:|
//@[021:0096) |   |   | └─ObjectSyntax
//@[021:0022) |   |   |   ├─Token(LeftBrace) |{|
//@[022:0023) |   |   |   ├─Token(NewLine) |\n|
      enabled: 123
//@[006:0018) |   |   |   ├─ObjectPropertySyntax
//@[006:0013) |   |   |   | ├─IdentifierSyntax
//@[006:0013) |   |   |   | | └─Token(Identifier) |enabled|
//@[013:0014) |   |   |   | ├─Token(Colon) |:|
//@[015:0018) |   |   |   | └─IntegerLiteralSyntax
//@[015:0018) |   |   |   |   └─Token(Integer) |123|
//@[018:0019) |   |   |   ├─Token(NewLine) |\n|
      storageUri: true
//@[006:0022) |   |   |   ├─ObjectPropertySyntax
//@[006:0016) |   |   |   | ├─IdentifierSyntax
//@[006:0016) |   |   |   | | └─Token(Identifier) |storageUri|
//@[016:0017) |   |   |   | ├─Token(Colon) |:|
//@[018:0022) |   |   |   | └─BooleanLiteralSyntax
//@[018:0022) |   |   |   |   └─Token(TrueKeyword) |true|
//@[022:0023) |   |   |   ├─Token(NewLine) |\n|
      unknownProp: 'asdf'
//@[006:0025) |   |   |   ├─ObjectPropertySyntax
//@[006:0017) |   |   |   | ├─IdentifierSyntax
//@[006:0017) |   |   |   | | └─Token(Identifier) |unknownProp|
//@[017:0018) |   |   |   | ├─Token(Colon) |:|
//@[019:0025) |   |   |   | └─StringSyntax
//@[019:0025) |   |   |   |   └─Token(StringComplete) |'asdf'|
//@[025:0026) |   |   |   ├─Token(NewLine) |\n|
    }
//@[004:0005) |   |   |   └─Token(RightBrace) |}|
//@[005:0006) |   |   ├─Token(NewLine) |\n|
  }
//@[002:0003) |   |   └─Token(RightBrace) |}|
//@[003:0004) |   ├─Token(NewLine) |\n|
  evictionPolicy: boolVal
//@[002:0025) |   ├─ObjectPropertySyntax
//@[002:0016) |   | ├─IdentifierSyntax
//@[002:0016) |   | | └─Token(Identifier) |evictionPolicy|
//@[016:0017) |   | ├─Token(Colon) |:|
//@[018:0025) |   | └─VariableAccessSyntax
//@[018:0025) |   |   └─IdentifierSyntax
//@[018:0025) |   |     └─Token(Identifier) |boolVal|
//@[025:0026) |   ├─Token(NewLine) |\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0003) ├─Token(NewLine) |\n\n|

resource vm 'Microsoft.Compute/virtualMachines@2020-12-01' = {
//@[000:0126) ├─ResourceDeclarationSyntax
//@[000:0008) | ├─Token(Identifier) |resource|
//@[009:0011) | ├─IdentifierSyntax
//@[009:0011) | | └─Token(Identifier) |vm|
//@[012:0058) | ├─StringSyntax
//@[012:0058) | | └─Token(StringComplete) |'Microsoft.Compute/virtualMachines@2020-12-01'|
//@[059:0060) | ├─Token(Assignment) |=|
//@[061:0126) | └─ObjectSyntax
//@[061:0062) |   ├─Token(LeftBrace) |{|
//@[062:0063) |   ├─Token(NewLine) |\n|
  name: 'vm'
//@[002:0012) |   ├─ObjectPropertySyntax
//@[002:0006) |   | ├─IdentifierSyntax
//@[002:0006) |   | | └─Token(Identifier) |name|
//@[006:0007) |   | ├─Token(Colon) |:|
//@[008:0012) |   | └─StringSyntax
//@[008:0012) |   |   └─Token(StringComplete) |'vm'|
//@[012:0013) |   ├─Token(NewLine) |\n|
  location: 'West US'
//@[002:0021) |   ├─ObjectPropertySyntax
//@[002:0010) |   | ├─IdentifierSyntax
//@[002:0010) |   | | └─Token(Identifier) |location|
//@[010:0011) |   | ├─Token(Colon) |:|
//@[012:0021) |   | └─StringSyntax
//@[012:0021) |   |   └─Token(StringComplete) |'West US'|
//@[021:0022) |   ├─Token(NewLine) |\n|
  properties: vmProperties
//@[002:0026) |   ├─ObjectPropertySyntax
//@[002:0012) |   | ├─IdentifierSyntax
//@[002:0012) |   | | └─Token(Identifier) |properties|
//@[012:0013) |   | ├─Token(Colon) |:|
//@[014:0026) |   | └─VariableAccessSyntax
//@[014:0026) |   |   └─IdentifierSyntax
//@[014:0026) |   |     └─Token(Identifier) |vmProperties|
//@[026:0027) |   ├─Token(NewLine) |\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0003) ├─Token(NewLine) |\n\n|

var ipConfigurations = [for i in range(0, 2): {
//@[000:0148) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0020) | ├─IdentifierSyntax
//@[004:0020) | | └─Token(Identifier) |ipConfigurations|
//@[021:0022) | ├─Token(Assignment) |=|
//@[023:0148) | └─ForSyntax
//@[023:0024) |   ├─Token(LeftSquare) |[|
//@[024:0027) |   ├─Token(Identifier) |for|
//@[028:0029) |   ├─LocalVariableSyntax
//@[028:0029) |   | └─IdentifierSyntax
//@[028:0029) |   |   └─Token(Identifier) |i|
//@[030:0032) |   ├─Token(Identifier) |in|
//@[033:0044) |   ├─FunctionCallSyntax
//@[033:0038) |   | ├─IdentifierSyntax
//@[033:0038) |   | | └─Token(Identifier) |range|
//@[038:0039) |   | ├─Token(LeftParen) |(|
//@[039:0040) |   | ├─FunctionArgumentSyntax
//@[039:0040) |   | | └─IntegerLiteralSyntax
//@[039:0040) |   | |   └─Token(Integer) |0|
//@[040:0041) |   | ├─Token(Comma) |,|
//@[042:0043) |   | ├─FunctionArgumentSyntax
//@[042:0043) |   | | └─IntegerLiteralSyntax
//@[042:0043) |   | |   └─Token(Integer) |2|
//@[043:0044) |   | └─Token(RightParen) |)|
//@[044:0045) |   ├─Token(Colon) |:|
//@[046:0147) |   ├─ObjectSyntax
//@[046:0047) |   | ├─Token(LeftBrace) |{|
//@[047:0048) |   | ├─Token(NewLine) |\n|
  id: true
//@[002:0010) |   | ├─ObjectPropertySyntax
//@[002:0004) |   | | ├─IdentifierSyntax
//@[002:0004) |   | | | └─Token(Identifier) |id|
//@[004:0005) |   | | ├─Token(Colon) |:|
//@[006:0010) |   | | └─BooleanLiteralSyntax
//@[006:0010) |   | |   └─Token(TrueKeyword) |true|
//@[010:0011) |   | ├─Token(NewLine) |\n|
  name: 'asdf${i}'
//@[002:0018) |   | ├─ObjectPropertySyntax
//@[002:0006) |   | | ├─IdentifierSyntax
//@[002:0006) |   | | | └─Token(Identifier) |name|
//@[006:0007) |   | | ├─Token(Colon) |:|
//@[008:0018) |   | | └─StringSyntax
//@[008:0015) |   | |   ├─Token(StringLeftPiece) |'asdf${|
//@[015:0016) |   | |   ├─VariableAccessSyntax
//@[015:0016) |   | |   | └─IdentifierSyntax
//@[015:0016) |   | |   |   └─Token(Identifier) |i|
//@[016:0018) |   | |   └─Token(StringRightPiece) |}'|
//@[018:0019) |   | ├─Token(NewLine) |\n|
  properties: {
//@[002:0067) |   | ├─ObjectPropertySyntax
//@[002:0012) |   | | ├─IdentifierSyntax
//@[002:0012) |   | | | └─Token(Identifier) |properties|
//@[012:0013) |   | | ├─Token(Colon) |:|
//@[014:0067) |   | | └─ObjectSyntax
//@[014:0015) |   | |   ├─Token(LeftBrace) |{|
//@[015:0016) |   | |   ├─Token(NewLine) |\n|
    madeUpProperty: boolVal
//@[004:0027) |   | |   ├─ObjectPropertySyntax
//@[004:0018) |   | |   | ├─IdentifierSyntax
//@[004:0018) |   | |   | | └─Token(Identifier) |madeUpProperty|
//@[018:0019) |   | |   | ├─Token(Colon) |:|
//@[020:0027) |   | |   | └─VariableAccessSyntax
//@[020:0027) |   | |   |   └─IdentifierSyntax
//@[020:0027) |   | |   |     └─Token(Identifier) |boolVal|
//@[027:0028) |   | |   ├─Token(NewLine) |\n|
    subnet: 'hello'
//@[004:0019) |   | |   ├─ObjectPropertySyntax
//@[004:0010) |   | |   | ├─IdentifierSyntax
//@[004:0010) |   | |   | | └─Token(Identifier) |subnet|
//@[010:0011) |   | |   | ├─Token(Colon) |:|
//@[012:0019) |   | |   | └─StringSyntax
//@[012:0019) |   | |   |   └─Token(StringComplete) |'hello'|
//@[019:0020) |   | |   ├─Token(NewLine) |\n|
  }
//@[002:0003) |   | |   └─Token(RightBrace) |}|
//@[003:0004) |   | ├─Token(NewLine) |\n|
}]
//@[000:0001) |   | └─Token(RightBrace) |}|
//@[001:0002) |   └─Token(RightSquare) |]|
//@[002:0004) ├─Token(NewLine) |\n\n|

resource nic 'Microsoft.Network/networkInterfaces@2020-11-01' = {
//@[000:0140) ├─ResourceDeclarationSyntax
//@[000:0008) | ├─Token(Identifier) |resource|
//@[009:0012) | ├─IdentifierSyntax
//@[009:0012) | | └─Token(Identifier) |nic|
//@[013:0061) | ├─StringSyntax
//@[013:0061) | | └─Token(StringComplete) |'Microsoft.Network/networkInterfaces@2020-11-01'|
//@[062:0063) | ├─Token(Assignment) |=|
//@[064:0140) | └─ObjectSyntax
//@[064:0065) |   ├─Token(LeftBrace) |{|
//@[065:0066) |   ├─Token(NewLine) |\n|
  name: 'abc'
//@[002:0013) |   ├─ObjectPropertySyntax
//@[002:0006) |   | ├─IdentifierSyntax
//@[002:0006) |   | | └─Token(Identifier) |name|
//@[006:0007) |   | ├─Token(Colon) |:|
//@[008:0013) |   | └─StringSyntax
//@[008:0013) |   |   └─Token(StringComplete) |'abc'|
//@[013:0014) |   ├─Token(NewLine) |\n|
  properties: {
//@[002:0058) |   ├─ObjectPropertySyntax
//@[002:0012) |   | ├─IdentifierSyntax
//@[002:0012) |   | | └─Token(Identifier) |properties|
//@[012:0013) |   | ├─Token(Colon) |:|
//@[014:0058) |   | └─ObjectSyntax
//@[014:0015) |   |   ├─Token(LeftBrace) |{|
//@[015:0016) |   |   ├─Token(NewLine) |\n|
    ipConfigurations: ipConfigurations
//@[004:0038) |   |   ├─ObjectPropertySyntax
//@[004:0020) |   |   | ├─IdentifierSyntax
//@[004:0020) |   |   | | └─Token(Identifier) |ipConfigurations|
//@[020:0021) |   |   | ├─Token(Colon) |:|
//@[022:0038) |   |   | └─VariableAccessSyntax
//@[022:0038) |   |   |   └─IdentifierSyntax
//@[022:0038) |   |   |     └─Token(Identifier) |ipConfigurations|
//@[038:0039) |   |   ├─Token(NewLine) |\n|
  }
//@[002:0003) |   |   └─Token(RightBrace) |}|
//@[003:0004) |   ├─Token(NewLine) |\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0003) ├─Token(NewLine) |\n\n|

resource nicLoop 'Microsoft.Network/networkInterfaces@2020-11-01' = [for i in range(0, 2): {
//@[000:0213) ├─ResourceDeclarationSyntax
//@[000:0008) | ├─Token(Identifier) |resource|
//@[009:0016) | ├─IdentifierSyntax
//@[009:0016) | | └─Token(Identifier) |nicLoop|
//@[017:0065) | ├─StringSyntax
//@[017:0065) | | └─Token(StringComplete) |'Microsoft.Network/networkInterfaces@2020-11-01'|
//@[066:0067) | ├─Token(Assignment) |=|
//@[068:0213) | └─ForSyntax
//@[068:0069) |   ├─Token(LeftSquare) |[|
//@[069:0072) |   ├─Token(Identifier) |for|
//@[073:0074) |   ├─LocalVariableSyntax
//@[073:0074) |   | └─IdentifierSyntax
//@[073:0074) |   |   └─Token(Identifier) |i|
//@[075:0077) |   ├─Token(Identifier) |in|
//@[078:0089) |   ├─FunctionCallSyntax
//@[078:0083) |   | ├─IdentifierSyntax
//@[078:0083) |   | | └─Token(Identifier) |range|
//@[083:0084) |   | ├─Token(LeftParen) |(|
//@[084:0085) |   | ├─FunctionArgumentSyntax
//@[084:0085) |   | | └─IntegerLiteralSyntax
//@[084:0085) |   | |   └─Token(Integer) |0|
//@[085:0086) |   | ├─Token(Comma) |,|
//@[087:0088) |   | ├─FunctionArgumentSyntax
//@[087:0088) |   | | └─IntegerLiteralSyntax
//@[087:0088) |   | |   └─Token(Integer) |2|
//@[088:0089) |   | └─Token(RightParen) |)|
//@[089:0090) |   ├─Token(Colon) |:|
//@[091:0212) |   ├─ObjectSyntax
//@[091:0092) |   | ├─Token(LeftBrace) |{|
//@[092:0093) |   | ├─Token(NewLine) |\n|
  name: 'abc${i}'
//@[002:0017) |   | ├─ObjectPropertySyntax
//@[002:0006) |   | | ├─IdentifierSyntax
//@[002:0006) |   | | | └─Token(Identifier) |name|
//@[006:0007) |   | | ├─Token(Colon) |:|
//@[008:0017) |   | | └─StringSyntax
//@[008:0014) |   | |   ├─Token(StringLeftPiece) |'abc${|
//@[014:0015) |   | |   ├─VariableAccessSyntax
//@[014:0015) |   | |   | └─IdentifierSyntax
//@[014:0015) |   | |   |   └─Token(Identifier) |i|
//@[015:0017) |   | |   └─Token(StringRightPiece) |}'|
//@[017:0018) |   | ├─Token(NewLine) |\n|
  properties: {
//@[002:0099) |   | ├─ObjectPropertySyntax
//@[002:0012) |   | | ├─IdentifierSyntax
//@[002:0012) |   | | | └─Token(Identifier) |properties|
//@[012:0013) |   | | ├─Token(Colon) |:|
//@[014:0099) |   | | └─ObjectSyntax
//@[014:0015) |   | |   ├─Token(LeftBrace) |{|
//@[015:0016) |   | |   ├─Token(NewLine) |\n|
    ipConfigurations: [
//@[004:0079) |   | |   ├─ObjectPropertySyntax
//@[004:0020) |   | |   | ├─IdentifierSyntax
//@[004:0020) |   | |   | | └─Token(Identifier) |ipConfigurations|
//@[020:0021) |   | |   | ├─Token(Colon) |:|
//@[022:0079) |   | |   | └─ArraySyntax
//@[022:0023) |   | |   |   ├─Token(LeftSquare) |[|
//@[023:0024) |   | |   |   ├─Token(NewLine) |\n|
      // TODO: fix this
//@[023:0024) |   | |   |   ├─Token(NewLine) |\n|
      ipConfigurations[i]
//@[006:0025) |   | |   |   ├─ArrayItemSyntax
//@[006:0025) |   | |   |   | └─ArrayAccessSyntax
//@[006:0022) |   | |   |   |   ├─VariableAccessSyntax
//@[006:0022) |   | |   |   |   | └─IdentifierSyntax
//@[006:0022) |   | |   |   |   |   └─Token(Identifier) |ipConfigurations|
//@[022:0023) |   | |   |   |   ├─Token(LeftSquare) |[|
//@[023:0024) |   | |   |   |   ├─VariableAccessSyntax
//@[023:0024) |   | |   |   |   | └─IdentifierSyntax
//@[023:0024) |   | |   |   |   |   └─Token(Identifier) |i|
//@[024:0025) |   | |   |   |   └─Token(RightSquare) |]|
//@[025:0026) |   | |   |   ├─Token(NewLine) |\n|
    ]
//@[004:0005) |   | |   |   └─Token(RightSquare) |]|
//@[005:0006) |   | |   ├─Token(NewLine) |\n|
  }
//@[002:0003) |   | |   └─Token(RightBrace) |}|
//@[003:0004) |   | ├─Token(NewLine) |\n|
}]
//@[000:0001) |   | └─Token(RightBrace) |}|
//@[001:0002) |   └─Token(RightSquare) |]|
//@[002:0004) ├─Token(NewLine) |\n\n|

resource nicLoop2 'Microsoft.Network/networkInterfaces@2020-11-01' = [for ipConfig in ipConfigurations: {
//@[000:0227) ├─ResourceDeclarationSyntax
//@[000:0008) | ├─Token(Identifier) |resource|
//@[009:0017) | ├─IdentifierSyntax
//@[009:0017) | | └─Token(Identifier) |nicLoop2|
//@[018:0066) | ├─StringSyntax
//@[018:0066) | | └─Token(StringComplete) |'Microsoft.Network/networkInterfaces@2020-11-01'|
//@[067:0068) | ├─Token(Assignment) |=|
//@[069:0227) | └─ForSyntax
//@[069:0070) |   ├─Token(LeftSquare) |[|
//@[070:0073) |   ├─Token(Identifier) |for|
//@[074:0082) |   ├─LocalVariableSyntax
//@[074:0082) |   | └─IdentifierSyntax
//@[074:0082) |   |   └─Token(Identifier) |ipConfig|
//@[083:0085) |   ├─Token(Identifier) |in|
//@[086:0102) |   ├─VariableAccessSyntax
//@[086:0102) |   | └─IdentifierSyntax
//@[086:0102) |   |   └─Token(Identifier) |ipConfigurations|
//@[102:0103) |   ├─Token(Colon) |:|
//@[104:0226) |   ├─ObjectSyntax
//@[104:0105) |   | ├─Token(LeftBrace) |{|
//@[105:0106) |   | ├─Token(NewLine) |\n|
  name: 'abc${ipConfig.name}'
//@[002:0029) |   | ├─ObjectPropertySyntax
//@[002:0006) |   | | ├─IdentifierSyntax
//@[002:0006) |   | | | └─Token(Identifier) |name|
//@[006:0007) |   | | ├─Token(Colon) |:|
//@[008:0029) |   | | └─StringSyntax
//@[008:0014) |   | |   ├─Token(StringLeftPiece) |'abc${|
//@[014:0027) |   | |   ├─PropertyAccessSyntax
//@[014:0022) |   | |   | ├─VariableAccessSyntax
//@[014:0022) |   | |   | | └─IdentifierSyntax
//@[014:0022) |   | |   | |   └─Token(Identifier) |ipConfig|
//@[022:0023) |   | |   | ├─Token(Dot) |.|
//@[023:0027) |   | |   | └─IdentifierSyntax
//@[023:0027) |   | |   |   └─Token(Identifier) |name|
//@[027:0029) |   | |   └─Token(StringRightPiece) |}'|
//@[029:0030) |   | ├─Token(NewLine) |\n|
  properties: {
//@[002:0088) |   | ├─ObjectPropertySyntax
//@[002:0012) |   | | ├─IdentifierSyntax
//@[002:0012) |   | | | └─Token(Identifier) |properties|
//@[012:0013) |   | | ├─Token(Colon) |:|
//@[014:0088) |   | | └─ObjectSyntax
//@[014:0015) |   | |   ├─Token(LeftBrace) |{|
//@[015:0016) |   | |   ├─Token(NewLine) |\n|
    ipConfigurations: [
//@[004:0068) |   | |   ├─ObjectPropertySyntax
//@[004:0020) |   | |   | ├─IdentifierSyntax
//@[004:0020) |   | |   | | └─Token(Identifier) |ipConfigurations|
//@[020:0021) |   | |   | ├─Token(Colon) |:|
//@[022:0068) |   | |   | └─ArraySyntax
//@[022:0023) |   | |   |   ├─Token(LeftSquare) |[|
//@[023:0024) |   | |   |   ├─Token(NewLine) |\n|
      // TODO: fix this
//@[023:0024) |   | |   |   ├─Token(NewLine) |\n|
      ipConfig
//@[006:0014) |   | |   |   ├─ArrayItemSyntax
//@[006:0014) |   | |   |   | └─VariableAccessSyntax
//@[006:0014) |   | |   |   |   └─IdentifierSyntax
//@[006:0014) |   | |   |   |     └─Token(Identifier) |ipConfig|
//@[014:0015) |   | |   |   ├─Token(NewLine) |\n|
    ]
//@[004:0005) |   | |   |   └─Token(RightSquare) |]|
//@[005:0006) |   | |   ├─Token(NewLine) |\n|
  }
//@[002:0003) |   | |   └─Token(RightBrace) |}|
//@[003:0004) |   | ├─Token(NewLine) |\n|
}]
//@[000:0001) |   | └─Token(RightBrace) |}|
//@[001:0002) |   └─Token(RightSquare) |]|
//@[002:0003) ├─Token(NewLine) |\n|

//@[000:0000) └─Token(EndOfFile) ||
