param deployTimeParam string = 'steve'
//@[00:1098) ProgramSyntax
//@[00:0038) ├─ParameterDeclarationSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0021) | ├─IdentifierSyntax
//@[06:0021) | | └─Token(Identifier) |deployTimeParam|
//@[22:0028) | ├─SimpleTypeSyntax
//@[22:0028) | | └─Token(Identifier) |string|
//@[29:0038) | └─ParameterDefaultValueSyntax
//@[29:0030) |   ├─Token(Assignment) |=|
//@[31:0038) |   └─StringSyntax
//@[31:0038) |     └─Token(StringComplete) |'steve'|
//@[38:0039) ├─Token(NewLine) |\n|
var deployTimeVar = 'nigel'
//@[00:0027) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0017) | ├─IdentifierSyntax
//@[04:0017) | | └─Token(Identifier) |deployTimeVar|
//@[18:0019) | ├─Token(Assignment) |=|
//@[20:0027) | └─StringSyntax
//@[20:0027) |   └─Token(StringComplete) |'nigel'|
//@[27:0028) ├─Token(NewLine) |\n|
var dependentVar = {
//@[00:0082) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0016) | ├─IdentifierSyntax
//@[04:0016) | | └─Token(Identifier) |dependentVar|
//@[17:0018) | ├─Token(Assignment) |=|
//@[19:0082) | └─ObjectSyntax
//@[19:0020) |   ├─Token(LeftBrace) |{|
//@[20:0021) |   ├─Token(NewLine) |\n|
  dependencies: [
//@[02:0059) |   ├─ObjectPropertySyntax
//@[02:0014) |   | ├─IdentifierSyntax
//@[02:0014) |   | | └─Token(Identifier) |dependencies|
//@[14:0015) |   | ├─Token(Colon) |:|
//@[16:0059) |   | └─ArraySyntax
//@[16:0017) |   |   ├─Token(LeftSquare) |[|
//@[17:0018) |   |   ├─Token(NewLine) |\n|
    deployTimeVar
//@[04:0017) |   |   ├─ArrayItemSyntax
//@[04:0017) |   |   | └─VariableAccessSyntax
//@[04:0017) |   |   |   └─IdentifierSyntax
//@[04:0017) |   |   |     └─Token(Identifier) |deployTimeVar|
//@[17:0018) |   |   ├─Token(NewLine) |\n|
    deployTimeParam
//@[04:0019) |   |   ├─ArrayItemSyntax
//@[04:0019) |   |   | └─VariableAccessSyntax
//@[04:0019) |   |   |   └─IdentifierSyntax
//@[04:0019) |   |   |     └─Token(Identifier) |deployTimeParam|
//@[19:0020) |   |   ├─Token(NewLine) |\n|
  ]
//@[02:0003) |   |   └─Token(RightSquare) |]|
//@[03:0004) |   ├─Token(NewLine) |\n|
}
//@[00:0001) |   └─Token(RightBrace) |}|
//@[01:0003) ├─Token(NewLine) |\n\n|

var resourceDependency = {
//@[00:0147) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0022) | ├─IdentifierSyntax
//@[04:0022) | | └─Token(Identifier) |resourceDependency|
//@[23:0024) | ├─Token(Assignment) |=|
//@[25:0147) | └─ObjectSyntax
//@[25:0026) |   ├─Token(LeftBrace) |{|
//@[26:0027) |   ├─Token(NewLine) |\n|
  dependenciesA: [
//@[02:0118) |   ├─ObjectPropertySyntax
//@[02:0015) |   | ├─IdentifierSyntax
//@[02:0015) |   | | └─Token(Identifier) |dependenciesA|
//@[15:0016) |   | ├─Token(Colon) |:|
//@[17:0118) |   | └─ArraySyntax
//@[17:0018) |   |   ├─Token(LeftSquare) |[|
//@[18:0019) |   |   ├─Token(NewLine) |\n|
    resA.id
//@[04:0011) |   |   ├─ArrayItemSyntax
//@[04:0011) |   |   | └─PropertyAccessSyntax
//@[04:0008) |   |   |   ├─VariableAccessSyntax
//@[04:0008) |   |   |   | └─IdentifierSyntax
//@[04:0008) |   |   |   |   └─Token(Identifier) |resA|
//@[08:0009) |   |   |   ├─Token(Dot) |.|
//@[09:0011) |   |   |   └─IdentifierSyntax
//@[09:0011) |   |   |     └─Token(Identifier) |id|
//@[11:0012) |   |   ├─Token(NewLine) |\n|
    resA.name
//@[04:0013) |   |   ├─ArrayItemSyntax
//@[04:0013) |   |   | └─PropertyAccessSyntax
//@[04:0008) |   |   |   ├─VariableAccessSyntax
//@[04:0008) |   |   |   | └─IdentifierSyntax
//@[04:0008) |   |   |   |   └─Token(Identifier) |resA|
//@[08:0009) |   |   |   ├─Token(Dot) |.|
//@[09:0013) |   |   |   └─IdentifierSyntax
//@[09:0013) |   |   |     └─Token(Identifier) |name|
//@[13:0014) |   |   ├─Token(NewLine) |\n|
    resA.type
//@[04:0013) |   |   ├─ArrayItemSyntax
//@[04:0013) |   |   | └─PropertyAccessSyntax
//@[04:0008) |   |   |   ├─VariableAccessSyntax
//@[04:0008) |   |   |   | └─IdentifierSyntax
//@[04:0008) |   |   |   |   └─Token(Identifier) |resA|
//@[08:0009) |   |   |   ├─Token(Dot) |.|
//@[09:0013) |   |   |   └─IdentifierSyntax
//@[09:0013) |   |   |     └─Token(Identifier) |type|
//@[13:0014) |   |   ├─Token(NewLine) |\n|
    resA.properties.deployTime
//@[04:0030) |   |   ├─ArrayItemSyntax
//@[04:0030) |   |   | └─PropertyAccessSyntax
//@[04:0019) |   |   |   ├─PropertyAccessSyntax
//@[04:0008) |   |   |   | ├─VariableAccessSyntax
//@[04:0008) |   |   |   | | └─IdentifierSyntax
//@[04:0008) |   |   |   | |   └─Token(Identifier) |resA|
//@[08:0009) |   |   |   | ├─Token(Dot) |.|
//@[09:0019) |   |   |   | └─IdentifierSyntax
//@[09:0019) |   |   |   |   └─Token(Identifier) |properties|
//@[19:0020) |   |   |   ├─Token(Dot) |.|
//@[20:0030) |   |   |   └─IdentifierSyntax
//@[20:0030) |   |   |     └─Token(Identifier) |deployTime|
//@[30:0031) |   |   ├─Token(NewLine) |\n|
    resA.properties.eTag
//@[04:0024) |   |   ├─ArrayItemSyntax
//@[04:0024) |   |   | └─PropertyAccessSyntax
//@[04:0019) |   |   |   ├─PropertyAccessSyntax
//@[04:0008) |   |   |   | ├─VariableAccessSyntax
//@[04:0008) |   |   |   | | └─IdentifierSyntax
//@[04:0008) |   |   |   | |   └─Token(Identifier) |resA|
//@[08:0009) |   |   |   | ├─Token(Dot) |.|
//@[09:0019) |   |   |   | └─IdentifierSyntax
//@[09:0019) |   |   |   |   └─Token(Identifier) |properties|
//@[19:0020) |   |   |   ├─Token(Dot) |.|
//@[20:0024) |   |   |   └─IdentifierSyntax
//@[20:0024) |   |   |     └─Token(Identifier) |eTag|
//@[24:0025) |   |   ├─Token(NewLine) |\n|
  ]
//@[02:0003) |   |   └─Token(RightSquare) |]|
//@[03:0004) |   ├─Token(NewLine) |\n|
}
//@[00:0001) |   └─Token(RightBrace) |}|
//@[01:0003) ├─Token(NewLine) |\n\n|

output resourceAType string = resA.type
//@[00:0039) ├─OutputDeclarationSyntax
//@[00:0006) | ├─Token(Identifier) |output|
//@[07:0020) | ├─IdentifierSyntax
//@[07:0020) | | └─Token(Identifier) |resourceAType|
//@[21:0027) | ├─SimpleTypeSyntax
//@[21:0027) | | └─Token(Identifier) |string|
//@[28:0029) | ├─Token(Assignment) |=|
//@[30:0039) | └─PropertyAccessSyntax
//@[30:0034) |   ├─VariableAccessSyntax
//@[30:0034) |   | └─IdentifierSyntax
//@[30:0034) |   |   └─Token(Identifier) |resA|
//@[34:0035) |   ├─Token(Dot) |.|
//@[35:0039) |   └─IdentifierSyntax
//@[35:0039) |     └─Token(Identifier) |type|
//@[39:0040) ├─Token(NewLine) |\n|
resource resA 'My.Rp/myResourceType@2020-01-01' = {
//@[00:0134) ├─ResourceDeclarationSyntax
//@[00:0008) | ├─Token(Identifier) |resource|
//@[09:0013) | ├─IdentifierSyntax
//@[09:0013) | | └─Token(Identifier) |resA|
//@[14:0047) | ├─StringSyntax
//@[14:0047) | | └─Token(StringComplete) |'My.Rp/myResourceType@2020-01-01'|
//@[48:0049) | ├─Token(Assignment) |=|
//@[50:0134) | └─ObjectSyntax
//@[50:0051) |   ├─Token(LeftBrace) |{|
//@[51:0052) |   ├─Token(NewLine) |\n|
  name: 'resA'
//@[02:0014) |   ├─ObjectPropertySyntax
//@[02:0006) |   | ├─IdentifierSyntax
//@[02:0006) |   | | └─Token(Identifier) |name|
//@[06:0007) |   | ├─Token(Colon) |:|
//@[08:0014) |   | └─StringSyntax
//@[08:0014) |   |   └─Token(StringComplete) |'resA'|
//@[14:0015) |   ├─Token(NewLine) |\n|
  properties: {
//@[02:0065) |   ├─ObjectPropertySyntax
//@[02:0012) |   | ├─IdentifierSyntax
//@[02:0012) |   | | └─Token(Identifier) |properties|
//@[12:0013) |   | ├─Token(Colon) |:|
//@[14:0065) |   | └─ObjectSyntax
//@[14:0015) |   |   ├─Token(LeftBrace) |{|
//@[15:0016) |   |   ├─Token(NewLine) |\n|
    deployTime: dependentVar
//@[04:0028) |   |   ├─ObjectPropertySyntax
//@[04:0014) |   |   | ├─IdentifierSyntax
//@[04:0014) |   |   | | └─Token(Identifier) |deployTime|
//@[14:0015) |   |   | ├─Token(Colon) |:|
//@[16:0028) |   |   | └─VariableAccessSyntax
//@[16:0028) |   |   |   └─IdentifierSyntax
//@[16:0028) |   |   |     └─Token(Identifier) |dependentVar|
//@[28:0029) |   |   ├─Token(NewLine) |\n|
    eTag: '1234'
//@[04:0016) |   |   ├─ObjectPropertySyntax
//@[04:0008) |   |   | ├─IdentifierSyntax
//@[04:0008) |   |   | | └─Token(Identifier) |eTag|
//@[08:0009) |   |   | ├─Token(Colon) |:|
//@[10:0016) |   |   | └─StringSyntax
//@[10:0016) |   |   |   └─Token(StringComplete) |'1234'|
//@[16:0017) |   |   ├─Token(NewLine) |\n|
  }
//@[02:0003) |   |   └─Token(RightBrace) |}|
//@[03:0004) |   ├─Token(NewLine) |\n|
}
//@[00:0001) |   └─Token(RightBrace) |}|
//@[01:0003) ├─Token(NewLine) |\n\n|

output resourceBId string = resB.id
//@[00:0035) ├─OutputDeclarationSyntax
//@[00:0006) | ├─Token(Identifier) |output|
//@[07:0018) | ├─IdentifierSyntax
//@[07:0018) | | └─Token(Identifier) |resourceBId|
//@[19:0025) | ├─SimpleTypeSyntax
//@[19:0025) | | └─Token(Identifier) |string|
//@[26:0027) | ├─Token(Assignment) |=|
//@[28:0035) | └─PropertyAccessSyntax
//@[28:0032) |   ├─VariableAccessSyntax
//@[28:0032) |   | └─IdentifierSyntax
//@[28:0032) |   |   └─Token(Identifier) |resB|
//@[32:0033) |   ├─Token(Dot) |.|
//@[33:0035) |   └─IdentifierSyntax
//@[33:0035) |     └─Token(Identifier) |id|
//@[35:0036) ├─Token(NewLine) |\n|
resource resB 'My.Rp/myResourceType@2020-01-01' = {
//@[00:0125) ├─ResourceDeclarationSyntax
//@[00:0008) | ├─Token(Identifier) |resource|
//@[09:0013) | ├─IdentifierSyntax
//@[09:0013) | | └─Token(Identifier) |resB|
//@[14:0047) | ├─StringSyntax
//@[14:0047) | | └─Token(StringComplete) |'My.Rp/myResourceType@2020-01-01'|
//@[48:0049) | ├─Token(Assignment) |=|
//@[50:0125) | └─ObjectSyntax
//@[50:0051) |   ├─Token(LeftBrace) |{|
//@[51:0052) |   ├─Token(NewLine) |\n|
  name: 'resB'
//@[02:0014) |   ├─ObjectPropertySyntax
//@[02:0006) |   | ├─IdentifierSyntax
//@[02:0006) |   | | └─Token(Identifier) |name|
//@[06:0007) |   | ├─Token(Colon) |:|
//@[08:0014) |   | └─StringSyntax
//@[08:0014) |   |   └─Token(StringComplete) |'resB'|
//@[14:0015) |   ├─Token(NewLine) |\n|
  properties: {
//@[02:0056) |   ├─ObjectPropertySyntax
//@[02:0012) |   | ├─IdentifierSyntax
//@[02:0012) |   | | └─Token(Identifier) |properties|
//@[12:0013) |   | ├─Token(Colon) |:|
//@[14:0056) |   | └─ObjectSyntax
//@[14:0015) |   |   ├─Token(LeftBrace) |{|
//@[15:0016) |   |   ├─Token(NewLine) |\n|
    dependencies: resourceDependency
//@[04:0036) |   |   ├─ObjectPropertySyntax
//@[04:0016) |   |   | ├─IdentifierSyntax
//@[04:0016) |   |   | | └─Token(Identifier) |dependencies|
//@[16:0017) |   |   | ├─Token(Colon) |:|
//@[18:0036) |   |   | └─VariableAccessSyntax
//@[18:0036) |   |   |   └─IdentifierSyntax
//@[18:0036) |   |   |     └─Token(Identifier) |resourceDependency|
//@[36:0037) |   |   ├─Token(NewLine) |\n|
  }
//@[02:0003) |   |   └─Token(RightBrace) |}|
//@[03:0004) |   ├─Token(NewLine) |\n|
}
//@[00:0001) |   └─Token(RightBrace) |}|
//@[01:0003) ├─Token(NewLine) |\n\n|

var resourceIds = {
//@[00:0047) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0015) | ├─IdentifierSyntax
//@[04:0015) | | └─Token(Identifier) |resourceIds|
//@[16:0017) | ├─Token(Assignment) |=|
//@[18:0047) | └─ObjectSyntax
//@[18:0019) |   ├─Token(LeftBrace) |{|
//@[19:0020) |   ├─Token(NewLine) |\n|
  a: resA.id
//@[02:0012) |   ├─ObjectPropertySyntax
//@[02:0003) |   | ├─IdentifierSyntax
//@[02:0003) |   | | └─Token(Identifier) |a|
//@[03:0004) |   | ├─Token(Colon) |:|
//@[05:0012) |   | └─PropertyAccessSyntax
//@[05:0009) |   |   ├─VariableAccessSyntax
//@[05:0009) |   |   | └─IdentifierSyntax
//@[05:0009) |   |   |   └─Token(Identifier) |resA|
//@[09:0010) |   |   ├─Token(Dot) |.|
//@[10:0012) |   |   └─IdentifierSyntax
//@[10:0012) |   |     └─Token(Identifier) |id|
//@[12:0013) |   ├─Token(NewLine) |\n|
  b: resB.id
//@[02:0012) |   ├─ObjectPropertySyntax
//@[02:0003) |   | ├─IdentifierSyntax
//@[02:0003) |   | | └─Token(Identifier) |b|
//@[03:0004) |   | ├─Token(Colon) |:|
//@[05:0012) |   | └─PropertyAccessSyntax
//@[05:0009) |   |   ├─VariableAccessSyntax
//@[05:0009) |   |   | └─IdentifierSyntax
//@[05:0009) |   |   |   └─Token(Identifier) |resB|
//@[09:0010) |   |   ├─Token(Dot) |.|
//@[10:0012) |   |   └─IdentifierSyntax
//@[10:0012) |   |     └─Token(Identifier) |id|
//@[12:0013) |   ├─Token(NewLine) |\n|
}
//@[00:0001) |   └─Token(RightBrace) |}|
//@[01:0003) ├─Token(NewLine) |\n\n|

resource resC 'My.Rp/myResourceType@2020-01-01' = {
//@[00:0117) ├─ResourceDeclarationSyntax
//@[00:0008) | ├─Token(Identifier) |resource|
//@[09:0013) | ├─IdentifierSyntax
//@[09:0013) | | └─Token(Identifier) |resC|
//@[14:0047) | ├─StringSyntax
//@[14:0047) | | └─Token(StringComplete) |'My.Rp/myResourceType@2020-01-01'|
//@[48:0049) | ├─Token(Assignment) |=|
//@[50:0117) | └─ObjectSyntax
//@[50:0051) |   ├─Token(LeftBrace) |{|
//@[51:0052) |   ├─Token(NewLine) |\n|
  name: 'resC'
//@[02:0014) |   ├─ObjectPropertySyntax
//@[02:0006) |   | ├─IdentifierSyntax
//@[02:0006) |   | | └─Token(Identifier) |name|
//@[06:0007) |   | ├─Token(Colon) |:|
//@[08:0014) |   | └─StringSyntax
//@[08:0014) |   |   └─Token(StringComplete) |'resC'|
//@[14:0015) |   ├─Token(NewLine) |\n|
  properties: {
//@[02:0048) |   ├─ObjectPropertySyntax
//@[02:0012) |   | ├─IdentifierSyntax
//@[02:0012) |   | | └─Token(Identifier) |properties|
//@[12:0013) |   | ├─Token(Colon) |:|
//@[14:0048) |   | └─ObjectSyntax
//@[14:0015) |   |   ├─Token(LeftBrace) |{|
//@[15:0016) |   |   ├─Token(NewLine) |\n|
    resourceIds: resourceIds
//@[04:0028) |   |   ├─ObjectPropertySyntax
//@[04:0015) |   |   | ├─IdentifierSyntax
//@[04:0015) |   |   | | └─Token(Identifier) |resourceIds|
//@[15:0016) |   |   | ├─Token(Colon) |:|
//@[17:0028) |   |   | └─VariableAccessSyntax
//@[17:0028) |   |   |   └─IdentifierSyntax
//@[17:0028) |   |   |     └─Token(Identifier) |resourceIds|
//@[28:0029) |   |   ├─Token(NewLine) |\n|
  }
//@[02:0003) |   |   └─Token(RightBrace) |}|
//@[03:0004) |   ├─Token(NewLine) |\n|
}
//@[00:0001) |   └─Token(RightBrace) |}|
//@[01:0003) ├─Token(NewLine) |\n\n|

resource resD 'My.Rp/myResourceType/childType@2020-01-01' = {
//@[00:0111) ├─ResourceDeclarationSyntax
//@[00:0008) | ├─Token(Identifier) |resource|
//@[09:0013) | ├─IdentifierSyntax
//@[09:0013) | | └─Token(Identifier) |resD|
//@[14:0057) | ├─StringSyntax
//@[14:0057) | | └─Token(StringComplete) |'My.Rp/myResourceType/childType@2020-01-01'|
//@[58:0059) | ├─Token(Assignment) |=|
//@[60:0111) | └─ObjectSyntax
//@[60:0061) |   ├─Token(LeftBrace) |{|
//@[61:0062) |   ├─Token(NewLine) |\n|
  name: '${resC.name}/resD'
//@[02:0027) |   ├─ObjectPropertySyntax
//@[02:0006) |   | ├─IdentifierSyntax
//@[02:0006) |   | | └─Token(Identifier) |name|
//@[06:0007) |   | ├─Token(Colon) |:|
//@[08:0027) |   | └─StringSyntax
//@[08:0011) |   |   ├─Token(StringLeftPiece) |'${|
//@[11:0020) |   |   ├─PropertyAccessSyntax
//@[11:0015) |   |   | ├─VariableAccessSyntax
//@[11:0015) |   |   | | └─IdentifierSyntax
//@[11:0015) |   |   | |   └─Token(Identifier) |resC|
//@[15:0016) |   |   | ├─Token(Dot) |.|
//@[16:0020) |   |   | └─IdentifierSyntax
//@[16:0020) |   |   |   └─Token(Identifier) |name|
//@[20:0027) |   |   └─Token(StringRightPiece) |}/resD'|
//@[27:0028) |   ├─Token(NewLine) |\n|
  properties: {
//@[02:0019) |   ├─ObjectPropertySyntax
//@[02:0012) |   | ├─IdentifierSyntax
//@[02:0012) |   | | └─Token(Identifier) |properties|
//@[12:0013) |   | ├─Token(Colon) |:|
//@[14:0019) |   | └─ObjectSyntax
//@[14:0015) |   |   ├─Token(LeftBrace) |{|
//@[15:0016) |   |   ├─Token(NewLine) |\n|
  }
//@[02:0003) |   |   └─Token(RightBrace) |}|
//@[03:0004) |   ├─Token(NewLine) |\n|
}
//@[00:0001) |   └─Token(RightBrace) |}|
//@[01:0003) ├─Token(NewLine) |\n\n|

resource resE 'My.Rp/myResourceType/childType@2020-01-01' = {
//@[00:0124) ├─ResourceDeclarationSyntax
//@[00:0008) | ├─Token(Identifier) |resource|
//@[09:0013) | ├─IdentifierSyntax
//@[09:0013) | | └─Token(Identifier) |resE|
//@[14:0057) | ├─StringSyntax
//@[14:0057) | | └─Token(StringComplete) |'My.Rp/myResourceType/childType@2020-01-01'|
//@[58:0059) | ├─Token(Assignment) |=|
//@[60:0124) | └─ObjectSyntax
//@[60:0061) |   ├─Token(LeftBrace) |{|
//@[61:0062) |   ├─Token(NewLine) |\n|
  name: 'resC/resD'
//@[02:0019) |   ├─ObjectPropertySyntax
//@[02:0006) |   | ├─IdentifierSyntax
//@[02:0006) |   | | └─Token(Identifier) |name|
//@[06:0007) |   | ├─Token(Colon) |:|
//@[08:0019) |   | └─StringSyntax
//@[08:0019) |   |   └─Token(StringComplete) |'resC/resD'|
//@[19:0020) |   ├─Token(NewLine) |\n|
  properties: {
//@[02:0040) |   ├─ObjectPropertySyntax
//@[02:0012) |   | ├─IdentifierSyntax
//@[02:0012) |   | | └─Token(Identifier) |properties|
//@[12:0013) |   | ├─Token(Colon) |:|
//@[14:0040) |   | └─ObjectSyntax
//@[14:0015) |   |   ├─Token(LeftBrace) |{|
//@[15:0016) |   |   ├─Token(NewLine) |\n|
    resDRef: resD.id
//@[04:0020) |   |   ├─ObjectPropertySyntax
//@[04:0011) |   |   | ├─IdentifierSyntax
//@[04:0011) |   |   | | └─Token(Identifier) |resDRef|
//@[11:0012) |   |   | ├─Token(Colon) |:|
//@[13:0020) |   |   | └─PropertyAccessSyntax
//@[13:0017) |   |   |   ├─VariableAccessSyntax
//@[13:0017) |   |   |   | └─IdentifierSyntax
//@[13:0017) |   |   |   |   └─Token(Identifier) |resD|
//@[17:0018) |   |   |   ├─Token(Dot) |.|
//@[18:0020) |   |   |   └─IdentifierSyntax
//@[18:0020) |   |   |     └─Token(Identifier) |id|
//@[20:0021) |   |   ├─Token(NewLine) |\n|
  }
//@[02:0003) |   |   └─Token(RightBrace) |}|
//@[03:0004) |   ├─Token(NewLine) |\n|
}
//@[00:0001) |   └─Token(RightBrace) |}|
//@[01:0003) ├─Token(NewLine) |\n\n|

output resourceCProperties object = resC.properties
//@[00:0051) ├─OutputDeclarationSyntax
//@[00:0006) | ├─Token(Identifier) |output|
//@[07:0026) | ├─IdentifierSyntax
//@[07:0026) | | └─Token(Identifier) |resourceCProperties|
//@[27:0033) | ├─SimpleTypeSyntax
//@[27:0033) | | └─Token(Identifier) |object|
//@[34:0035) | ├─Token(Assignment) |=|
//@[36:0051) | └─PropertyAccessSyntax
//@[36:0040) |   ├─VariableAccessSyntax
//@[36:0040) |   | └─IdentifierSyntax
//@[36:0040) |   |   └─Token(Identifier) |resC|
//@[40:0041) |   ├─Token(Dot) |.|
//@[41:0051) |   └─IdentifierSyntax
//@[41:0051) |     └─Token(Identifier) |properties|
//@[51:0052) ├─Token(NewLine) |\n|

//@[00:0000) └─Token(EndOfFile) ||
