targetScope = 'tenant'
//@[000:1350) ProgramSyntax
//@[000:0022) ├─TargetScopeSyntax
//@[000:0011) | ├─Token(Identifier) |targetScope|
//@[012:0013) | ├─Token(Assignment) |=|
//@[014:0022) | └─StringSyntax
//@[014:0022) |   └─Token(StringComplete) |'tenant'|
//@[022:0026) ├─Token(NewLine) |\r\n\r\n|

var managementGroups = [
//@[000:0142) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0020) | ├─IdentifierSyntax
//@[004:0020) | | └─Token(Identifier) |managementGroups|
//@[021:0022) | ├─Token(Assignment) |=|
//@[023:0142) | └─ArraySyntax
//@[023:0024) |   ├─Token(LeftSquare) |[|
//@[024:0026) |   ├─Token(NewLine) |\r\n|
  {
//@[002:0055) |   ├─ArrayItemSyntax
//@[002:0055) |   | └─ObjectSyntax
//@[002:0003) |   |   ├─Token(LeftBrace) |{|
//@[003:0005) |   |   ├─Token(NewLine) |\r\n|
    name: 'one'
//@[004:0015) |   |   ├─ObjectPropertySyntax
//@[004:0008) |   |   | ├─IdentifierSyntax
//@[004:0008) |   |   | | └─Token(Identifier) |name|
//@[008:0009) |   |   | ├─Token(Colon) |:|
//@[010:0015) |   |   | └─StringSyntax
//@[010:0015) |   |   |   └─Token(StringComplete) |'one'|
//@[015:0017) |   |   ├─Token(NewLine) |\r\n|
    displayName: 'The first'
//@[004:0028) |   |   ├─ObjectPropertySyntax
//@[004:0015) |   |   | ├─IdentifierSyntax
//@[004:0015) |   |   | | └─Token(Identifier) |displayName|
//@[015:0016) |   |   | ├─Token(Colon) |:|
//@[017:0028) |   |   | └─StringSyntax
//@[017:0028) |   |   |   └─Token(StringComplete) |'The first'|
//@[028:0030) |   |   ├─Token(NewLine) |\r\n|
  }
//@[002:0003) |   |   └─Token(RightBrace) |}|
//@[003:0005) |   ├─Token(NewLine) |\r\n|
  {
//@[002:0056) |   ├─ArrayItemSyntax
//@[002:0056) |   | └─ObjectSyntax
//@[002:0003) |   |   ├─Token(LeftBrace) |{|
//@[003:0005) |   |   ├─Token(NewLine) |\r\n|
    name: 'two'
//@[004:0015) |   |   ├─ObjectPropertySyntax
//@[004:0008) |   |   | ├─IdentifierSyntax
//@[004:0008) |   |   | | └─Token(Identifier) |name|
//@[008:0009) |   |   | ├─Token(Colon) |:|
//@[010:0015) |   |   | └─StringSyntax
//@[010:0015) |   |   |   └─Token(StringComplete) |'two'|
//@[015:0017) |   |   ├─Token(NewLine) |\r\n|
    displayName: 'The second'
//@[004:0029) |   |   ├─ObjectPropertySyntax
//@[004:0015) |   |   | ├─IdentifierSyntax
//@[004:0015) |   |   | | └─Token(Identifier) |displayName|
//@[015:0016) |   |   | ├─Token(Colon) |:|
//@[017:0029) |   |   | └─StringSyntax
//@[017:0029) |   |   |   └─Token(StringComplete) |'The second'|
//@[029:0031) |   |   ├─Token(NewLine) |\r\n|
  }
//@[002:0003) |   |   └─Token(RightBrace) |}|
//@[003:0005) |   ├─Token(NewLine) |\r\n|
]
//@[000:0001) |   └─Token(RightSquare) |]|
//@[001:0005) ├─Token(NewLine) |\r\n\r\n|

resource singleGroup 'Microsoft.Management/managementGroups@2020-05-01' = {
//@[000:0154) ├─ResourceDeclarationSyntax
//@[000:0008) | ├─Token(Identifier) |resource|
//@[009:0020) | ├─IdentifierSyntax
//@[009:0020) | | └─Token(Identifier) |singleGroup|
//@[021:0071) | ├─StringSyntax
//@[021:0071) | | └─Token(StringComplete) |'Microsoft.Management/managementGroups@2020-05-01'|
//@[072:0073) | ├─Token(Assignment) |=|
//@[074:0154) | └─ObjectSyntax
//@[074:0075) |   ├─Token(LeftBrace) |{|
//@[075:0077) |   ├─Token(NewLine) |\r\n|
  name: 'myMG'
//@[002:0014) |   ├─ObjectPropertySyntax
//@[002:0006) |   | ├─IdentifierSyntax
//@[002:0006) |   | | └─Token(Identifier) |name|
//@[006:0007) |   | ├─Token(Colon) |:|
//@[008:0014) |   | └─StringSyntax
//@[008:0014) |   |   └─Token(StringComplete) |'myMG'|
//@[014:0016) |   ├─Token(NewLine) |\r\n|
  properties: {
//@[002:0058) |   ├─ObjectPropertySyntax
//@[002:0012) |   | ├─IdentifierSyntax
//@[002:0012) |   | | └─Token(Identifier) |properties|
//@[012:0013) |   | ├─Token(Colon) |:|
//@[014:0058) |   | └─ObjectSyntax
//@[014:0015) |   |   ├─Token(LeftBrace) |{|
//@[015:0017) |   |   ├─Token(NewLine) |\r\n|
    displayName: 'This one is mine!'
//@[004:0036) |   |   ├─ObjectPropertySyntax
//@[004:0015) |   |   | ├─IdentifierSyntax
//@[004:0015) |   |   | | └─Token(Identifier) |displayName|
//@[015:0016) |   |   | ├─Token(Colon) |:|
//@[017:0036) |   |   | └─StringSyntax
//@[017:0036) |   |   |   └─Token(StringComplete) |'This one is mine!'|
//@[036:0038) |   |   ├─Token(NewLine) |\r\n|
  }
//@[002:0003) |   |   └─Token(RightBrace) |}|
//@[003:0005) |   ├─Token(NewLine) |\r\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0005) ├─Token(NewLine) |\r\n\r\n|

resource manyGroups 'Microsoft.Management/managementGroups@2020-05-01' = [for mg in managementGroups: {
//@[000:0224) ├─ResourceDeclarationSyntax
//@[000:0008) | ├─Token(Identifier) |resource|
//@[009:0019) | ├─IdentifierSyntax
//@[009:0019) | | └─Token(Identifier) |manyGroups|
//@[020:0070) | ├─StringSyntax
//@[020:0070) | | └─Token(StringComplete) |'Microsoft.Management/managementGroups@2020-05-01'|
//@[071:0072) | ├─Token(Assignment) |=|
//@[073:0224) | └─ForSyntax
//@[073:0074) |   ├─Token(LeftSquare) |[|
//@[074:0077) |   ├─Token(Identifier) |for|
//@[078:0080) |   ├─LocalVariableSyntax
//@[078:0080) |   | └─IdentifierSyntax
//@[078:0080) |   |   └─Token(Identifier) |mg|
//@[081:0083) |   ├─Token(Identifier) |in|
//@[084:0100) |   ├─VariableAccessSyntax
//@[084:0100) |   | └─IdentifierSyntax
//@[084:0100) |   |   └─Token(Identifier) |managementGroups|
//@[100:0101) |   ├─Token(Colon) |:|
//@[102:0223) |   ├─ObjectSyntax
//@[102:0103) |   | ├─Token(LeftBrace) |{|
//@[103:0105) |   | ├─Token(NewLine) |\r\n|
  name: mg.name
//@[002:0015) |   | ├─ObjectPropertySyntax
//@[002:0006) |   | | ├─IdentifierSyntax
//@[002:0006) |   | | | └─Token(Identifier) |name|
//@[006:0007) |   | | ├─Token(Colon) |:|
//@[008:0015) |   | | └─PropertyAccessSyntax
//@[008:0010) |   | |   ├─VariableAccessSyntax
//@[008:0010) |   | |   | └─IdentifierSyntax
//@[008:0010) |   | |   |   └─Token(Identifier) |mg|
//@[010:0011) |   | |   ├─Token(Dot) |.|
//@[011:0015) |   | |   └─IdentifierSyntax
//@[011:0015) |   | |     └─Token(Identifier) |name|
//@[015:0017) |   | ├─Token(NewLine) |\r\n|
  properties: {
//@[002:0098) |   | ├─ObjectPropertySyntax
//@[002:0012) |   | | ├─IdentifierSyntax
//@[002:0012) |   | | | └─Token(Identifier) |properties|
//@[012:0013) |   | | ├─Token(Colon) |:|
//@[014:0098) |   | | └─ObjectSyntax
//@[014:0015) |   | |   ├─Token(LeftBrace) |{|
//@[015:0017) |   | |   ├─Token(NewLine) |\r\n|
    displayName: '${mg.displayName} (${singleGroup.properties.displayName})'
//@[004:0076) |   | |   ├─ObjectPropertySyntax
//@[004:0015) |   | |   | ├─IdentifierSyntax
//@[004:0015) |   | |   | | └─Token(Identifier) |displayName|
//@[015:0016) |   | |   | ├─Token(Colon) |:|
//@[017:0076) |   | |   | └─StringSyntax
//@[017:0020) |   | |   |   ├─Token(StringLeftPiece) |'${|
//@[020:0034) |   | |   |   ├─PropertyAccessSyntax
//@[020:0022) |   | |   |   | ├─VariableAccessSyntax
//@[020:0022) |   | |   |   | | └─IdentifierSyntax
//@[020:0022) |   | |   |   | |   └─Token(Identifier) |mg|
//@[022:0023) |   | |   |   | ├─Token(Dot) |.|
//@[023:0034) |   | |   |   | └─IdentifierSyntax
//@[023:0034) |   | |   |   |   └─Token(Identifier) |displayName|
//@[034:0039) |   | |   |   ├─Token(StringMiddlePiece) |} (${|
//@[039:0073) |   | |   |   ├─PropertyAccessSyntax
//@[039:0061) |   | |   |   | ├─PropertyAccessSyntax
//@[039:0050) |   | |   |   | | ├─VariableAccessSyntax
//@[039:0050) |   | |   |   | | | └─IdentifierSyntax
//@[039:0050) |   | |   |   | | |   └─Token(Identifier) |singleGroup|
//@[050:0051) |   | |   |   | | ├─Token(Dot) |.|
//@[051:0061) |   | |   |   | | └─IdentifierSyntax
//@[051:0061) |   | |   |   | |   └─Token(Identifier) |properties|
//@[061:0062) |   | |   |   | ├─Token(Dot) |.|
//@[062:0073) |   | |   |   | └─IdentifierSyntax
//@[062:0073) |   | |   |   |   └─Token(Identifier) |displayName|
//@[073:0076) |   | |   |   └─Token(StringRightPiece) |})'|
//@[076:0078) |   | |   ├─Token(NewLine) |\r\n|
  }
//@[002:0003) |   | |   └─Token(RightBrace) |}|
//@[003:0005) |   | ├─Token(NewLine) |\r\n|
}]
//@[000:0001) |   | └─Token(RightBrace) |}|
//@[001:0002) |   └─Token(RightSquare) |]|
//@[002:0006) ├─Token(NewLine) |\r\n\r\n|

resource anotherSet 'Microsoft.Management/managementGroups@2020-05-01' = [for (mg, index) in managementGroups: {
//@[000:0319) ├─ResourceDeclarationSyntax
//@[000:0008) | ├─Token(Identifier) |resource|
//@[009:0019) | ├─IdentifierSyntax
//@[009:0019) | | └─Token(Identifier) |anotherSet|
//@[020:0070) | ├─StringSyntax
//@[020:0070) | | └─Token(StringComplete) |'Microsoft.Management/managementGroups@2020-05-01'|
//@[071:0072) | ├─Token(Assignment) |=|
//@[073:0319) | └─ForSyntax
//@[073:0074) |   ├─Token(LeftSquare) |[|
//@[074:0077) |   ├─Token(Identifier) |for|
//@[078:0089) |   ├─VariableBlockSyntax
//@[078:0079) |   | ├─Token(LeftParen) |(|
//@[079:0081) |   | ├─LocalVariableSyntax
//@[079:0081) |   | | └─IdentifierSyntax
//@[079:0081) |   | |   └─Token(Identifier) |mg|
//@[081:0082) |   | ├─Token(Comma) |,|
//@[083:0088) |   | ├─LocalVariableSyntax
//@[083:0088) |   | | └─IdentifierSyntax
//@[083:0088) |   | |   └─Token(Identifier) |index|
//@[088:0089) |   | └─Token(RightParen) |)|
//@[090:0092) |   ├─Token(Identifier) |in|
//@[093:0109) |   ├─VariableAccessSyntax
//@[093:0109) |   | └─IdentifierSyntax
//@[093:0109) |   |   └─Token(Identifier) |managementGroups|
//@[109:0110) |   ├─Token(Colon) |:|
//@[111:0318) |   ├─ObjectSyntax
//@[111:0112) |   | ├─Token(LeftBrace) |{|
//@[112:0114) |   | ├─Token(NewLine) |\r\n|
  name: concat(mg.name, '-one-', index)
//@[002:0039) |   | ├─ObjectPropertySyntax
//@[002:0006) |   | | ├─IdentifierSyntax
//@[002:0006) |   | | | └─Token(Identifier) |name|
//@[006:0007) |   | | ├─Token(Colon) |:|
//@[008:0039) |   | | └─FunctionCallSyntax
//@[008:0014) |   | |   ├─IdentifierSyntax
//@[008:0014) |   | |   | └─Token(Identifier) |concat|
//@[014:0015) |   | |   ├─Token(LeftParen) |(|
//@[015:0022) |   | |   ├─FunctionArgumentSyntax
//@[015:0022) |   | |   | └─PropertyAccessSyntax
//@[015:0017) |   | |   |   ├─VariableAccessSyntax
//@[015:0017) |   | |   |   | └─IdentifierSyntax
//@[015:0017) |   | |   |   |   └─Token(Identifier) |mg|
//@[017:0018) |   | |   |   ├─Token(Dot) |.|
//@[018:0022) |   | |   |   └─IdentifierSyntax
//@[018:0022) |   | |   |     └─Token(Identifier) |name|
//@[022:0023) |   | |   ├─Token(Comma) |,|
//@[024:0031) |   | |   ├─FunctionArgumentSyntax
//@[024:0031) |   | |   | └─StringSyntax
//@[024:0031) |   | |   |   └─Token(StringComplete) |'-one-'|
//@[031:0032) |   | |   ├─Token(Comma) |,|
//@[033:0038) |   | |   ├─FunctionArgumentSyntax
//@[033:0038) |   | |   | └─VariableAccessSyntax
//@[033:0038) |   | |   |   └─IdentifierSyntax
//@[033:0038) |   | |   |     └─Token(Identifier) |index|
//@[038:0039) |   | |   └─Token(RightParen) |)|
//@[039:0041) |   | ├─Token(NewLine) |\r\n|
  properties: {
//@[002:0123) |   | ├─ObjectPropertySyntax
//@[002:0012) |   | | ├─IdentifierSyntax
//@[002:0012) |   | | | └─Token(Identifier) |properties|
//@[012:0013) |   | | ├─Token(Colon) |:|
//@[014:0123) |   | | └─ObjectSyntax
//@[014:0015) |   | |   ├─Token(LeftBrace) |{|
//@[015:0017) |   | |   ├─Token(NewLine) |\r\n|
    displayName: '${mg.displayName} (${singleGroup.properties.displayName}) (set 1) (index ${index})'
//@[004:0101) |   | |   ├─ObjectPropertySyntax
//@[004:0015) |   | |   | ├─IdentifierSyntax
//@[004:0015) |   | |   | | └─Token(Identifier) |displayName|
//@[015:0016) |   | |   | ├─Token(Colon) |:|
//@[017:0101) |   | |   | └─StringSyntax
//@[017:0020) |   | |   |   ├─Token(StringLeftPiece) |'${|
//@[020:0034) |   | |   |   ├─PropertyAccessSyntax
//@[020:0022) |   | |   |   | ├─VariableAccessSyntax
//@[020:0022) |   | |   |   | | └─IdentifierSyntax
//@[020:0022) |   | |   |   | |   └─Token(Identifier) |mg|
//@[022:0023) |   | |   |   | ├─Token(Dot) |.|
//@[023:0034) |   | |   |   | └─IdentifierSyntax
//@[023:0034) |   | |   |   |   └─Token(Identifier) |displayName|
//@[034:0039) |   | |   |   ├─Token(StringMiddlePiece) |} (${|
//@[039:0073) |   | |   |   ├─PropertyAccessSyntax
//@[039:0061) |   | |   |   | ├─PropertyAccessSyntax
//@[039:0050) |   | |   |   | | ├─VariableAccessSyntax
//@[039:0050) |   | |   |   | | | └─IdentifierSyntax
//@[039:0050) |   | |   |   | | |   └─Token(Identifier) |singleGroup|
//@[050:0051) |   | |   |   | | ├─Token(Dot) |.|
//@[051:0061) |   | |   |   | | └─IdentifierSyntax
//@[051:0061) |   | |   |   | |   └─Token(Identifier) |properties|
//@[061:0062) |   | |   |   | ├─Token(Dot) |.|
//@[062:0073) |   | |   |   | └─IdentifierSyntax
//@[062:0073) |   | |   |   |   └─Token(Identifier) |displayName|
//@[073:0093) |   | |   |   ├─Token(StringMiddlePiece) |}) (set 1) (index ${|
//@[093:0098) |   | |   |   ├─VariableAccessSyntax
//@[093:0098) |   | |   |   | └─IdentifierSyntax
//@[093:0098) |   | |   |   |   └─Token(Identifier) |index|
//@[098:0101) |   | |   |   └─Token(StringRightPiece) |})'|
//@[101:0103) |   | |   ├─Token(NewLine) |\r\n|
  }
//@[002:0003) |   | |   └─Token(RightBrace) |}|
//@[003:0005) |   | ├─Token(NewLine) |\r\n|
  dependsOn: [
//@[002:0035) |   | ├─ObjectPropertySyntax
//@[002:0011) |   | | ├─IdentifierSyntax
//@[002:0011) |   | | | └─Token(Identifier) |dependsOn|
//@[011:0012) |   | | ├─Token(Colon) |:|
//@[013:0035) |   | | └─ArraySyntax
//@[013:0014) |   | |   ├─Token(LeftSquare) |[|
//@[014:0016) |   | |   ├─Token(NewLine) |\r\n|
    manyGroups
//@[004:0014) |   | |   ├─ArrayItemSyntax
//@[004:0014) |   | |   | └─VariableAccessSyntax
//@[004:0014) |   | |   |   └─IdentifierSyntax
//@[004:0014) |   | |   |     └─Token(Identifier) |manyGroups|
//@[014:0016) |   | |   ├─Token(NewLine) |\r\n|
  ]
//@[002:0003) |   | |   └─Token(RightSquare) |]|
//@[003:0005) |   | ├─Token(NewLine) |\r\n|
}]
//@[000:0001) |   | └─Token(RightBrace) |}|
//@[001:0002) |   └─Token(RightSquare) |]|
//@[002:0006) ├─Token(NewLine) |\r\n\r\n|

resource yetAnotherSet 'Microsoft.Management/managementGroups@2020-05-01' = [for mg in managementGroups: {
//@[000:0291) ├─ResourceDeclarationSyntax
//@[000:0008) | ├─Token(Identifier) |resource|
//@[009:0022) | ├─IdentifierSyntax
//@[009:0022) | | └─Token(Identifier) |yetAnotherSet|
//@[023:0073) | ├─StringSyntax
//@[023:0073) | | └─Token(StringComplete) |'Microsoft.Management/managementGroups@2020-05-01'|
//@[074:0075) | ├─Token(Assignment) |=|
//@[076:0291) | └─ForSyntax
//@[076:0077) |   ├─Token(LeftSquare) |[|
//@[077:0080) |   ├─Token(Identifier) |for|
//@[081:0083) |   ├─LocalVariableSyntax
//@[081:0083) |   | └─IdentifierSyntax
//@[081:0083) |   |   └─Token(Identifier) |mg|
//@[084:0086) |   ├─Token(Identifier) |in|
//@[087:0103) |   ├─VariableAccessSyntax
//@[087:0103) |   | └─IdentifierSyntax
//@[087:0103) |   |   └─Token(Identifier) |managementGroups|
//@[103:0104) |   ├─Token(Colon) |:|
//@[105:0290) |   ├─ObjectSyntax
//@[105:0106) |   | ├─Token(LeftBrace) |{|
//@[106:0108) |   | ├─Token(NewLine) |\r\n|
  name: concat(mg.name, '-two')
//@[002:0031) |   | ├─ObjectPropertySyntax
//@[002:0006) |   | | ├─IdentifierSyntax
//@[002:0006) |   | | | └─Token(Identifier) |name|
//@[006:0007) |   | | ├─Token(Colon) |:|
//@[008:0031) |   | | └─FunctionCallSyntax
//@[008:0014) |   | |   ├─IdentifierSyntax
//@[008:0014) |   | |   | └─Token(Identifier) |concat|
//@[014:0015) |   | |   ├─Token(LeftParen) |(|
//@[015:0022) |   | |   ├─FunctionArgumentSyntax
//@[015:0022) |   | |   | └─PropertyAccessSyntax
//@[015:0017) |   | |   |   ├─VariableAccessSyntax
//@[015:0017) |   | |   |   | └─IdentifierSyntax
//@[015:0017) |   | |   |   |   └─Token(Identifier) |mg|
//@[017:0018) |   | |   |   ├─Token(Dot) |.|
//@[018:0022) |   | |   |   └─IdentifierSyntax
//@[018:0022) |   | |   |     └─Token(Identifier) |name|
//@[022:0023) |   | |   ├─Token(Comma) |,|
//@[024:0030) |   | |   ├─FunctionArgumentSyntax
//@[024:0030) |   | |   | └─StringSyntax
//@[024:0030) |   | |   |   └─Token(StringComplete) |'-two'|
//@[030:0031) |   | |   └─Token(RightParen) |)|
//@[031:0033) |   | ├─Token(NewLine) |\r\n|
  properties: {
//@[002:0106) |   | ├─ObjectPropertySyntax
//@[002:0012) |   | | ├─IdentifierSyntax
//@[002:0012) |   | | | └─Token(Identifier) |properties|
//@[012:0013) |   | | ├─Token(Colon) |:|
//@[014:0106) |   | | └─ObjectSyntax
//@[014:0015) |   | |   ├─Token(LeftBrace) |{|
//@[015:0017) |   | |   ├─Token(NewLine) |\r\n|
    displayName: '${mg.displayName} (${singleGroup.properties.displayName}) (set 2)'
//@[004:0084) |   | |   ├─ObjectPropertySyntax
//@[004:0015) |   | |   | ├─IdentifierSyntax
//@[004:0015) |   | |   | | └─Token(Identifier) |displayName|
//@[015:0016) |   | |   | ├─Token(Colon) |:|
//@[017:0084) |   | |   | └─StringSyntax
//@[017:0020) |   | |   |   ├─Token(StringLeftPiece) |'${|
//@[020:0034) |   | |   |   ├─PropertyAccessSyntax
//@[020:0022) |   | |   |   | ├─VariableAccessSyntax
//@[020:0022) |   | |   |   | | └─IdentifierSyntax
//@[020:0022) |   | |   |   | |   └─Token(Identifier) |mg|
//@[022:0023) |   | |   |   | ├─Token(Dot) |.|
//@[023:0034) |   | |   |   | └─IdentifierSyntax
//@[023:0034) |   | |   |   |   └─Token(Identifier) |displayName|
//@[034:0039) |   | |   |   ├─Token(StringMiddlePiece) |} (${|
//@[039:0073) |   | |   |   ├─PropertyAccessSyntax
//@[039:0061) |   | |   |   | ├─PropertyAccessSyntax
//@[039:0050) |   | |   |   | | ├─VariableAccessSyntax
//@[039:0050) |   | |   |   | | | └─IdentifierSyntax
//@[039:0050) |   | |   |   | | |   └─Token(Identifier) |singleGroup|
//@[050:0051) |   | |   |   | | ├─Token(Dot) |.|
//@[051:0061) |   | |   |   | | └─IdentifierSyntax
//@[051:0061) |   | |   |   | |   └─Token(Identifier) |properties|
//@[061:0062) |   | |   |   | ├─Token(Dot) |.|
//@[062:0073) |   | |   |   | └─IdentifierSyntax
//@[062:0073) |   | |   |   |   └─Token(Identifier) |displayName|
//@[073:0084) |   | |   |   └─Token(StringRightPiece) |}) (set 2)'|
//@[084:0086) |   | |   ├─Token(NewLine) |\r\n|
  }
//@[002:0003) |   | |   └─Token(RightBrace) |}|
//@[003:0005) |   | ├─Token(NewLine) |\r\n|
  dependsOn: [
//@[002:0038) |   | ├─ObjectPropertySyntax
//@[002:0011) |   | | ├─IdentifierSyntax
//@[002:0011) |   | | | └─Token(Identifier) |dependsOn|
//@[011:0012) |   | | ├─Token(Colon) |:|
//@[013:0038) |   | | └─ArraySyntax
//@[013:0014) |   | |   ├─Token(LeftSquare) |[|
//@[014:0016) |   | |   ├─Token(NewLine) |\r\n|
    anotherSet[0]
//@[004:0017) |   | |   ├─ArrayItemSyntax
//@[004:0017) |   | |   | └─ArrayAccessSyntax
//@[004:0014) |   | |   |   ├─VariableAccessSyntax
//@[004:0014) |   | |   |   | └─IdentifierSyntax
//@[004:0014) |   | |   |   |   └─Token(Identifier) |anotherSet|
//@[014:0015) |   | |   |   ├─Token(LeftSquare) |[|
//@[015:0016) |   | |   |   ├─IntegerLiteralSyntax
//@[015:0016) |   | |   |   | └─Token(Integer) |0|
//@[016:0017) |   | |   |   └─Token(RightSquare) |]|
//@[017:0019) |   | |   ├─Token(NewLine) |\r\n|
  ]
//@[002:0003) |   | |   └─Token(RightSquare) |]|
//@[003:0005) |   | ├─Token(NewLine) |\r\n|
}]
//@[000:0001) |   | └─Token(RightBrace) |}|
//@[001:0002) |   └─Token(RightSquare) |]|
//@[002:0006) ├─Token(NewLine) |\r\n\r\n|

output managementGroupIds array = [for i in range(0, length(managementGroups)): {
//@[000:0172) ├─OutputDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |output|
//@[007:0025) | ├─IdentifierSyntax
//@[007:0025) | | └─Token(Identifier) |managementGroupIds|
//@[026:0031) | ├─SimpleTypeSyntax
//@[026:0031) | | └─Token(Identifier) |array|
//@[032:0033) | ├─Token(Assignment) |=|
//@[034:0172) | └─ForSyntax
//@[034:0035) |   ├─Token(LeftSquare) |[|
//@[035:0038) |   ├─Token(Identifier) |for|
//@[039:0040) |   ├─LocalVariableSyntax
//@[039:0040) |   | └─IdentifierSyntax
//@[039:0040) |   |   └─Token(Identifier) |i|
//@[041:0043) |   ├─Token(Identifier) |in|
//@[044:0078) |   ├─FunctionCallSyntax
//@[044:0049) |   | ├─IdentifierSyntax
//@[044:0049) |   | | └─Token(Identifier) |range|
//@[049:0050) |   | ├─Token(LeftParen) |(|
//@[050:0051) |   | ├─FunctionArgumentSyntax
//@[050:0051) |   | | └─IntegerLiteralSyntax
//@[050:0051) |   | |   └─Token(Integer) |0|
//@[051:0052) |   | ├─Token(Comma) |,|
//@[053:0077) |   | ├─FunctionArgumentSyntax
//@[053:0077) |   | | └─FunctionCallSyntax
//@[053:0059) |   | |   ├─IdentifierSyntax
//@[053:0059) |   | |   | └─Token(Identifier) |length|
//@[059:0060) |   | |   ├─Token(LeftParen) |(|
//@[060:0076) |   | |   ├─FunctionArgumentSyntax
//@[060:0076) |   | |   | └─VariableAccessSyntax
//@[060:0076) |   | |   |   └─IdentifierSyntax
//@[060:0076) |   | |   |     └─Token(Identifier) |managementGroups|
//@[076:0077) |   | |   └─Token(RightParen) |)|
//@[077:0078) |   | └─Token(RightParen) |)|
//@[078:0079) |   ├─Token(Colon) |:|
//@[080:0171) |   ├─ObjectSyntax
//@[080:0081) |   | ├─Token(LeftBrace) |{|
//@[081:0083) |   | ├─Token(NewLine) |\r\n|
  name: yetAnotherSet[i].name
//@[002:0029) |   | ├─ObjectPropertySyntax
//@[002:0006) |   | | ├─IdentifierSyntax
//@[002:0006) |   | | | └─Token(Identifier) |name|
//@[006:0007) |   | | ├─Token(Colon) |:|
//@[008:0029) |   | | └─PropertyAccessSyntax
//@[008:0024) |   | |   ├─ArrayAccessSyntax
//@[008:0021) |   | |   | ├─VariableAccessSyntax
//@[008:0021) |   | |   | | └─IdentifierSyntax
//@[008:0021) |   | |   | |   └─Token(Identifier) |yetAnotherSet|
//@[021:0022) |   | |   | ├─Token(LeftSquare) |[|
//@[022:0023) |   | |   | ├─VariableAccessSyntax
//@[022:0023) |   | |   | | └─IdentifierSyntax
//@[022:0023) |   | |   | |   └─Token(Identifier) |i|
//@[023:0024) |   | |   | └─Token(RightSquare) |]|
//@[024:0025) |   | |   ├─Token(Dot) |.|
//@[025:0029) |   | |   └─IdentifierSyntax
//@[025:0029) |   | |     └─Token(Identifier) |name|
//@[029:0031) |   | ├─Token(NewLine) |\r\n|
  displayName: yetAnotherSet[i].properties.displayName
//@[002:0054) |   | ├─ObjectPropertySyntax
//@[002:0013) |   | | ├─IdentifierSyntax
//@[002:0013) |   | | | └─Token(Identifier) |displayName|
//@[013:0014) |   | | ├─Token(Colon) |:|
//@[015:0054) |   | | └─PropertyAccessSyntax
//@[015:0042) |   | |   ├─PropertyAccessSyntax
//@[015:0031) |   | |   | ├─ArrayAccessSyntax
//@[015:0028) |   | |   | | ├─VariableAccessSyntax
//@[015:0028) |   | |   | | | └─IdentifierSyntax
//@[015:0028) |   | |   | | |   └─Token(Identifier) |yetAnotherSet|
//@[028:0029) |   | |   | | ├─Token(LeftSquare) |[|
//@[029:0030) |   | |   | | ├─VariableAccessSyntax
//@[029:0030) |   | |   | | | └─IdentifierSyntax
//@[029:0030) |   | |   | | |   └─Token(Identifier) |i|
//@[030:0031) |   | |   | | └─Token(RightSquare) |]|
//@[031:0032) |   | |   | ├─Token(Dot) |.|
//@[032:0042) |   | |   | └─IdentifierSyntax
//@[032:0042) |   | |   |   └─Token(Identifier) |properties|
//@[042:0043) |   | |   ├─Token(Dot) |.|
//@[043:0054) |   | |   └─IdentifierSyntax
//@[043:0054) |   | |     └─Token(Identifier) |displayName|
//@[054:0056) |   | ├─Token(NewLine) |\r\n|
}]
//@[000:0001) |   | └─Token(RightBrace) |}|
//@[001:0002) |   └─Token(RightSquare) |]|
//@[002:0004) ├─Token(NewLine) |\r\n|

//@[000:0000) └─Token(EndOfFile) ||
