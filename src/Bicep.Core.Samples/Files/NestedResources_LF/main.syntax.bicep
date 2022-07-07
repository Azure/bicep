resource basicParent 'My.Rp/parentType@2020-12-01' = {
//@[00:2073) ProgramSyntax
//@[00:0659) ├─ResourceDeclarationSyntax
//@[00:0008) | ├─Token(Identifier) |resource|
//@[09:0020) | ├─IdentifierSyntax
//@[09:0020) | | └─Token(Identifier) |basicParent|
//@[21:0050) | ├─StringSyntax
//@[21:0050) | | └─Token(StringComplete) |'My.Rp/parentType@2020-12-01'|
//@[51:0052) | ├─Token(Assignment) |=|
//@[53:0659) | └─ObjectSyntax
//@[53:0054) |   ├─Token(LeftBrace) |{|
//@[54:0055) |   ├─Token(NewLine) |\n|
  name: 'basicParent'
//@[02:0021) |   ├─ObjectPropertySyntax
//@[02:0006) |   | ├─IdentifierSyntax
//@[02:0006) |   | | └─Token(Identifier) |name|
//@[06:0007) |   | ├─Token(Colon) |:|
//@[08:0021) |   | └─StringSyntax
//@[08:0021) |   |   └─Token(StringComplete) |'basicParent'|
//@[21:0022) |   ├─Token(NewLine) |\n|
  properties: {
//@[02:0037) |   ├─ObjectPropertySyntax
//@[02:0012) |   | ├─IdentifierSyntax
//@[02:0012) |   | | └─Token(Identifier) |properties|
//@[12:0013) |   | ├─Token(Colon) |:|
//@[14:0037) |   | └─ObjectSyntax
//@[14:0015) |   |   ├─Token(LeftBrace) |{|
//@[15:0016) |   |   ├─Token(NewLine) |\n|
    size: 'large'
//@[04:0017) |   |   ├─ObjectPropertySyntax
//@[04:0008) |   |   | ├─IdentifierSyntax
//@[04:0008) |   |   | | └─Token(Identifier) |size|
//@[08:0009) |   |   | ├─Token(Colon) |:|
//@[10:0017) |   |   | └─StringSyntax
//@[10:0017) |   |   |   └─Token(StringComplete) |'large'|
//@[17:0018) |   |   ├─Token(NewLine) |\n|
  }
//@[02:0003) |   |   └─Token(RightBrace) |}|
//@[03:0005) |   ├─Token(NewLine) |\n\n|

  resource basicChild 'childType' = {
//@[02:0349) |   ├─ResourceDeclarationSyntax
//@[02:0010) |   | ├─Token(Identifier) |resource|
//@[11:0021) |   | ├─IdentifierSyntax
//@[11:0021) |   | | └─Token(Identifier) |basicChild|
//@[22:0033) |   | ├─StringSyntax
//@[22:0033) |   | | └─Token(StringComplete) |'childType'|
//@[34:0035) |   | ├─Token(Assignment) |=|
//@[36:0349) |   | └─ObjectSyntax
//@[36:0037) |   |   ├─Token(LeftBrace) |{|
//@[37:0038) |   |   ├─Token(NewLine) |\n|
    name: 'basicChild'
//@[04:0022) |   |   ├─ObjectPropertySyntax
//@[04:0008) |   |   | ├─IdentifierSyntax
//@[04:0008) |   |   | | └─Token(Identifier) |name|
//@[08:0009) |   |   | ├─Token(Colon) |:|
//@[10:0022) |   |   | └─StringSyntax
//@[10:0022) |   |   |   └─Token(StringComplete) |'basicChild'|
//@[22:0023) |   |   ├─Token(NewLine) |\n|
    properties: {
//@[04:0084) |   |   ├─ObjectPropertySyntax
//@[04:0014) |   |   | ├─IdentifierSyntax
//@[04:0014) |   |   | | └─Token(Identifier) |properties|
//@[14:0015) |   |   | ├─Token(Colon) |:|
//@[16:0084) |   |   | └─ObjectSyntax
//@[16:0017) |   |   |   ├─Token(LeftBrace) |{|
//@[17:0018) |   |   |   ├─Token(NewLine) |\n|
      size: basicParent.properties.large
//@[06:0040) |   |   |   ├─ObjectPropertySyntax
//@[06:0010) |   |   |   | ├─IdentifierSyntax
//@[06:0010) |   |   |   | | └─Token(Identifier) |size|
//@[10:0011) |   |   |   | ├─Token(Colon) |:|
//@[12:0040) |   |   |   | └─PropertyAccessSyntax
//@[12:0034) |   |   |   |   ├─PropertyAccessSyntax
//@[12:0023) |   |   |   |   | ├─VariableAccessSyntax
//@[12:0023) |   |   |   |   | | └─IdentifierSyntax
//@[12:0023) |   |   |   |   | |   └─Token(Identifier) |basicParent|
//@[23:0024) |   |   |   |   | ├─Token(Dot) |.|
//@[24:0034) |   |   |   |   | └─IdentifierSyntax
//@[24:0034) |   |   |   |   |   └─Token(Identifier) |properties|
//@[34:0035) |   |   |   |   ├─Token(Dot) |.|
//@[35:0040) |   |   |   |   └─IdentifierSyntax
//@[35:0040) |   |   |   |     └─Token(Identifier) |large|
//@[40:0041) |   |   |   ├─Token(NewLine) |\n|
      style: 'cool'
//@[06:0019) |   |   |   ├─ObjectPropertySyntax
//@[06:0011) |   |   |   | ├─IdentifierSyntax
//@[06:0011) |   |   |   | | └─Token(Identifier) |style|
//@[11:0012) |   |   |   | ├─Token(Colon) |:|
//@[13:0019) |   |   |   | └─StringSyntax
//@[13:0019) |   |   |   |   └─Token(StringComplete) |'cool'|
//@[19:0020) |   |   |   ├─Token(NewLine) |\n|
    }
//@[04:0005) |   |   |   └─Token(RightBrace) |}|
//@[05:0007) |   |   ├─Token(NewLine) |\n\n|

    resource basicGrandchild 'grandchildType' = {
//@[04:0198) |   |   ├─ResourceDeclarationSyntax
//@[04:0012) |   |   | ├─Token(Identifier) |resource|
//@[13:0028) |   |   | ├─IdentifierSyntax
//@[13:0028) |   |   | | └─Token(Identifier) |basicGrandchild|
//@[29:0045) |   |   | ├─StringSyntax
//@[29:0045) |   |   | | └─Token(StringComplete) |'grandchildType'|
//@[46:0047) |   |   | ├─Token(Assignment) |=|
//@[48:0198) |   |   | └─ObjectSyntax
//@[48:0049) |   |   |   ├─Token(LeftBrace) |{|
//@[49:0050) |   |   |   ├─Token(NewLine) |\n|
      name: 'basicGrandchild'
//@[06:0029) |   |   |   ├─ObjectPropertySyntax
//@[06:0010) |   |   |   | ├─IdentifierSyntax
//@[06:0010) |   |   |   | | └─Token(Identifier) |name|
//@[10:0011) |   |   |   | ├─Token(Colon) |:|
//@[12:0029) |   |   |   | └─StringSyntax
//@[12:0029) |   |   |   |   └─Token(StringComplete) |'basicGrandchild'|
//@[29:0030) |   |   |   ├─Token(NewLine) |\n|
      properties: {
//@[06:0112) |   |   |   ├─ObjectPropertySyntax
//@[06:0016) |   |   |   | ├─IdentifierSyntax
//@[06:0016) |   |   |   | | └─Token(Identifier) |properties|
//@[16:0017) |   |   |   | ├─Token(Colon) |:|
//@[18:0112) |   |   |   | └─ObjectSyntax
//@[18:0019) |   |   |   |   ├─Token(LeftBrace) |{|
//@[19:0020) |   |   |   |   ├─Token(NewLine) |\n|
        size: basicParent.properties.size
//@[08:0041) |   |   |   |   ├─ObjectPropertySyntax
//@[08:0012) |   |   |   |   | ├─IdentifierSyntax
//@[08:0012) |   |   |   |   | | └─Token(Identifier) |size|
//@[12:0013) |   |   |   |   | ├─Token(Colon) |:|
//@[14:0041) |   |   |   |   | └─PropertyAccessSyntax
//@[14:0036) |   |   |   |   |   ├─PropertyAccessSyntax
//@[14:0025) |   |   |   |   |   | ├─VariableAccessSyntax
//@[14:0025) |   |   |   |   |   | | └─IdentifierSyntax
//@[14:0025) |   |   |   |   |   | |   └─Token(Identifier) |basicParent|
//@[25:0026) |   |   |   |   |   | ├─Token(Dot) |.|
//@[26:0036) |   |   |   |   |   | └─IdentifierSyntax
//@[26:0036) |   |   |   |   |   |   └─Token(Identifier) |properties|
//@[36:0037) |   |   |   |   |   ├─Token(Dot) |.|
//@[37:0041) |   |   |   |   |   └─IdentifierSyntax
//@[37:0041) |   |   |   |   |     └─Token(Identifier) |size|
//@[41:0042) |   |   |   |   ├─Token(NewLine) |\n|
        style: basicChild.properties.style
//@[08:0042) |   |   |   |   ├─ObjectPropertySyntax
//@[08:0013) |   |   |   |   | ├─IdentifierSyntax
//@[08:0013) |   |   |   |   | | └─Token(Identifier) |style|
//@[13:0014) |   |   |   |   | ├─Token(Colon) |:|
//@[15:0042) |   |   |   |   | └─PropertyAccessSyntax
//@[15:0036) |   |   |   |   |   ├─PropertyAccessSyntax
//@[15:0025) |   |   |   |   |   | ├─VariableAccessSyntax
//@[15:0025) |   |   |   |   |   | | └─IdentifierSyntax
//@[15:0025) |   |   |   |   |   | |   └─Token(Identifier) |basicChild|
//@[25:0026) |   |   |   |   |   | ├─Token(Dot) |.|
//@[26:0036) |   |   |   |   |   | └─IdentifierSyntax
//@[26:0036) |   |   |   |   |   |   └─Token(Identifier) |properties|
//@[36:0037) |   |   |   |   |   ├─Token(Dot) |.|
//@[37:0042) |   |   |   |   |   └─IdentifierSyntax
//@[37:0042) |   |   |   |   |     └─Token(Identifier) |style|
//@[42:0043) |   |   |   |   ├─Token(NewLine) |\n|
      }
//@[06:0007) |   |   |   |   └─Token(RightBrace) |}|
//@[07:0008) |   |   |   ├─Token(NewLine) |\n|
    }
//@[04:0005) |   |   |   └─Token(RightBrace) |}|
//@[05:0006) |   |   ├─Token(NewLine) |\n|
  }
//@[02:0003) |   |   └─Token(RightBrace) |}|
//@[03:0005) |   ├─Token(NewLine) |\n\n|

  resource basicSibling 'childType' = {
//@[02:0190) |   ├─ResourceDeclarationSyntax
//@[02:0010) |   | ├─Token(Identifier) |resource|
//@[11:0023) |   | ├─IdentifierSyntax
//@[11:0023) |   | | └─Token(Identifier) |basicSibling|
//@[24:0035) |   | ├─StringSyntax
//@[24:0035) |   | | └─Token(StringComplete) |'childType'|
//@[36:0037) |   | ├─Token(Assignment) |=|
//@[38:0190) |   | └─ObjectSyntax
//@[38:0039) |   |   ├─Token(LeftBrace) |{|
//@[39:0040) |   |   ├─Token(NewLine) |\n|
    name: 'basicSibling'
//@[04:0024) |   |   ├─ObjectPropertySyntax
//@[04:0008) |   |   | ├─IdentifierSyntax
//@[04:0008) |   |   | | └─Token(Identifier) |name|
//@[08:0009) |   |   | ├─Token(Colon) |:|
//@[10:0024) |   |   | └─StringSyntax
//@[10:0024) |   |   |   └─Token(StringComplete) |'basicSibling'|
//@[24:0025) |   |   ├─Token(NewLine) |\n|
    properties: {
//@[04:0121) |   |   ├─ObjectPropertySyntax
//@[04:0014) |   |   | ├─IdentifierSyntax
//@[04:0014) |   |   | | └─Token(Identifier) |properties|
//@[14:0015) |   |   | ├─Token(Colon) |:|
//@[16:0121) |   |   | └─ObjectSyntax
//@[16:0017) |   |   |   ├─Token(LeftBrace) |{|
//@[17:0018) |   |   |   ├─Token(NewLine) |\n|
      size: basicParent.properties.size
//@[06:0039) |   |   |   ├─ObjectPropertySyntax
//@[06:0010) |   |   |   | ├─IdentifierSyntax
//@[06:0010) |   |   |   | | └─Token(Identifier) |size|
//@[10:0011) |   |   |   | ├─Token(Colon) |:|
//@[12:0039) |   |   |   | └─PropertyAccessSyntax
//@[12:0034) |   |   |   |   ├─PropertyAccessSyntax
//@[12:0023) |   |   |   |   | ├─VariableAccessSyntax
//@[12:0023) |   |   |   |   | | └─IdentifierSyntax
//@[12:0023) |   |   |   |   | |   └─Token(Identifier) |basicParent|
//@[23:0024) |   |   |   |   | ├─Token(Dot) |.|
//@[24:0034) |   |   |   |   | └─IdentifierSyntax
//@[24:0034) |   |   |   |   |   └─Token(Identifier) |properties|
//@[34:0035) |   |   |   |   ├─Token(Dot) |.|
//@[35:0039) |   |   |   |   └─IdentifierSyntax
//@[35:0039) |   |   |   |     └─Token(Identifier) |size|
//@[39:0040) |   |   |   ├─Token(NewLine) |\n|
      style: basicChild::basicGrandchild.properties.style
//@[06:0057) |   |   |   ├─ObjectPropertySyntax
//@[06:0011) |   |   |   | ├─IdentifierSyntax
//@[06:0011) |   |   |   | | └─Token(Identifier) |style|
//@[11:0012) |   |   |   | ├─Token(Colon) |:|
//@[13:0057) |   |   |   | └─PropertyAccessSyntax
//@[13:0051) |   |   |   |   ├─PropertyAccessSyntax
//@[13:0040) |   |   |   |   | ├─ResourceAccessSyntax
//@[13:0023) |   |   |   |   | | ├─VariableAccessSyntax
//@[13:0023) |   |   |   |   | | | └─IdentifierSyntax
//@[13:0023) |   |   |   |   | | |   └─Token(Identifier) |basicChild|
//@[23:0025) |   |   |   |   | | ├─Token(DoubleColon) |::|
//@[25:0040) |   |   |   |   | | └─IdentifierSyntax
//@[25:0040) |   |   |   |   | |   └─Token(Identifier) |basicGrandchild|
//@[40:0041) |   |   |   |   | ├─Token(Dot) |.|
//@[41:0051) |   |   |   |   | └─IdentifierSyntax
//@[41:0051) |   |   |   |   |   └─Token(Identifier) |properties|
//@[51:0052) |   |   |   |   ├─Token(Dot) |.|
//@[52:0057) |   |   |   |   └─IdentifierSyntax
//@[52:0057) |   |   |   |     └─Token(Identifier) |style|
//@[57:0058) |   |   |   ├─Token(NewLine) |\n|
    }
//@[04:0005) |   |   |   └─Token(RightBrace) |}|
//@[05:0006) |   |   ├─Token(NewLine) |\n|
  }
//@[02:0003) |   |   └─Token(RightBrace) |}|
//@[03:0004) |   ├─Token(NewLine) |\n|
}
//@[00:0001) |   └─Token(RightBrace) |}|
//@[01:0002) ├─Token(NewLine) |\n|
// #completionTest(50) -> childResources
//@[40:0041) ├─Token(NewLine) |\n|
output referenceBasicChild string = basicParent::basicChild.properties.size
//@[00:0075) ├─OutputDeclarationSyntax
//@[00:0006) | ├─Token(Identifier) |output|
//@[07:0026) | ├─IdentifierSyntax
//@[07:0026) | | └─Token(Identifier) |referenceBasicChild|
//@[27:0033) | ├─SimpleTypeSyntax
//@[27:0033) | | └─Token(Identifier) |string|
//@[34:0035) | ├─Token(Assignment) |=|
//@[36:0075) | └─PropertyAccessSyntax
//@[36:0070) |   ├─PropertyAccessSyntax
//@[36:0059) |   | ├─ResourceAccessSyntax
//@[36:0047) |   | | ├─VariableAccessSyntax
//@[36:0047) |   | | | └─IdentifierSyntax
//@[36:0047) |   | | |   └─Token(Identifier) |basicParent|
//@[47:0049) |   | | ├─Token(DoubleColon) |::|
//@[49:0059) |   | | └─IdentifierSyntax
//@[49:0059) |   | |   └─Token(Identifier) |basicChild|
//@[59:0060) |   | ├─Token(Dot) |.|
//@[60:0070) |   | └─IdentifierSyntax
//@[60:0070) |   |   └─Token(Identifier) |properties|
//@[70:0071) |   ├─Token(Dot) |.|
//@[71:0075) |   └─IdentifierSyntax
//@[71:0075) |     └─Token(Identifier) |size|
//@[75:0076) ├─Token(NewLine) |\n|
// #completionTest(67) -> grandChildResources
//@[45:0046) ├─Token(NewLine) |\n|
output referenceBasicGrandchild string = basicParent::basicChild::basicGrandchild.properties.style
//@[00:0098) ├─OutputDeclarationSyntax
//@[00:0006) | ├─Token(Identifier) |output|
//@[07:0031) | ├─IdentifierSyntax
//@[07:0031) | | └─Token(Identifier) |referenceBasicGrandchild|
//@[32:0038) | ├─SimpleTypeSyntax
//@[32:0038) | | └─Token(Identifier) |string|
//@[39:0040) | ├─Token(Assignment) |=|
//@[41:0098) | └─PropertyAccessSyntax
//@[41:0092) |   ├─PropertyAccessSyntax
//@[41:0081) |   | ├─ResourceAccessSyntax
//@[41:0064) |   | | ├─ResourceAccessSyntax
//@[41:0052) |   | | | ├─VariableAccessSyntax
//@[41:0052) |   | | | | └─IdentifierSyntax
//@[41:0052) |   | | | |   └─Token(Identifier) |basicParent|
//@[52:0054) |   | | | ├─Token(DoubleColon) |::|
//@[54:0064) |   | | | └─IdentifierSyntax
//@[54:0064) |   | | |   └─Token(Identifier) |basicChild|
//@[64:0066) |   | | ├─Token(DoubleColon) |::|
//@[66:0081) |   | | └─IdentifierSyntax
//@[66:0081) |   | |   └─Token(Identifier) |basicGrandchild|
//@[81:0082) |   | ├─Token(Dot) |.|
//@[82:0092) |   | └─IdentifierSyntax
//@[82:0092) |   |   └─Token(Identifier) |properties|
//@[92:0093) |   ├─Token(Dot) |.|
//@[93:0098) |   └─IdentifierSyntax
//@[93:0098) |     └─Token(Identifier) |style|
//@[98:0100) ├─Token(NewLine) |\n\n|

resource existingParent 'My.Rp/parentType@2020-12-01' existing = {
//@[00:0386) ├─ResourceDeclarationSyntax
//@[00:0008) | ├─Token(Identifier) |resource|
//@[09:0023) | ├─IdentifierSyntax
//@[09:0023) | | └─Token(Identifier) |existingParent|
//@[24:0053) | ├─StringSyntax
//@[24:0053) | | └─Token(StringComplete) |'My.Rp/parentType@2020-12-01'|
//@[54:0062) | ├─Token(Identifier) |existing|
//@[63:0064) | ├─Token(Assignment) |=|
//@[65:0386) | └─ObjectSyntax
//@[65:0066) |   ├─Token(LeftBrace) |{|
//@[66:0067) |   ├─Token(NewLine) |\n|
  name: 'existingParent'
//@[02:0024) |   ├─ObjectPropertySyntax
//@[02:0006) |   | ├─IdentifierSyntax
//@[02:0006) |   | | └─Token(Identifier) |name|
//@[06:0007) |   | ├─Token(Colon) |:|
//@[08:0024) |   | └─StringSyntax
//@[08:0024) |   |   └─Token(StringComplete) |'existingParent'|
//@[24:0026) |   ├─Token(NewLine) |\n\n|

  resource existingChild 'childType' existing = {
//@[02:0291) |   ├─ResourceDeclarationSyntax
//@[02:0010) |   | ├─Token(Identifier) |resource|
//@[11:0024) |   | ├─IdentifierSyntax
//@[11:0024) |   | | └─Token(Identifier) |existingChild|
//@[25:0036) |   | ├─StringSyntax
//@[25:0036) |   | | └─Token(StringComplete) |'childType'|
//@[37:0045) |   | ├─Token(Identifier) |existing|
//@[46:0047) |   | ├─Token(Assignment) |=|
//@[48:0291) |   | └─ObjectSyntax
//@[48:0049) |   |   ├─Token(LeftBrace) |{|
//@[49:0050) |   |   ├─Token(NewLine) |\n|
    name: 'existingChild'
//@[04:0025) |   |   ├─ObjectPropertySyntax
//@[04:0008) |   |   | ├─IdentifierSyntax
//@[04:0008) |   |   | | └─Token(Identifier) |name|
//@[08:0009) |   |   | ├─Token(Colon) |:|
//@[10:0025) |   |   | └─StringSyntax
//@[10:0025) |   |   |   └─Token(StringComplete) |'existingChild'|
//@[25:0027) |   |   ├─Token(NewLine) |\n\n|

    resource existingGrandchild 'grandchildType' = {
//@[04:0210) |   |   ├─ResourceDeclarationSyntax
//@[04:0012) |   |   | ├─Token(Identifier) |resource|
//@[13:0031) |   |   | ├─IdentifierSyntax
//@[13:0031) |   |   | | └─Token(Identifier) |existingGrandchild|
//@[32:0048) |   |   | ├─StringSyntax
//@[32:0048) |   |   | | └─Token(StringComplete) |'grandchildType'|
//@[49:0050) |   |   | ├─Token(Assignment) |=|
//@[51:0210) |   |   | └─ObjectSyntax
//@[51:0052) |   |   |   ├─Token(LeftBrace) |{|
//@[52:0053) |   |   |   ├─Token(NewLine) |\n|
      name: 'existingGrandchild'
//@[06:0032) |   |   |   ├─ObjectPropertySyntax
//@[06:0010) |   |   |   | ├─IdentifierSyntax
//@[06:0010) |   |   |   | | └─Token(Identifier) |name|
//@[10:0011) |   |   |   | ├─Token(Colon) |:|
//@[12:0032) |   |   |   | └─StringSyntax
//@[12:0032) |   |   |   |   └─Token(StringComplete) |'existingGrandchild'|
//@[32:0033) |   |   |   ├─Token(NewLine) |\n|
      properties: {
//@[06:0118) |   |   |   ├─ObjectPropertySyntax
//@[06:0016) |   |   |   | ├─IdentifierSyntax
//@[06:0016) |   |   |   | | └─Token(Identifier) |properties|
//@[16:0017) |   |   |   | ├─Token(Colon) |:|
//@[18:0118) |   |   |   | └─ObjectSyntax
//@[18:0019) |   |   |   |   ├─Token(LeftBrace) |{|
//@[19:0020) |   |   |   |   ├─Token(NewLine) |\n|
        size: existingParent.properties.size
//@[08:0044) |   |   |   |   ├─ObjectPropertySyntax
//@[08:0012) |   |   |   |   | ├─IdentifierSyntax
//@[08:0012) |   |   |   |   | | └─Token(Identifier) |size|
//@[12:0013) |   |   |   |   | ├─Token(Colon) |:|
//@[14:0044) |   |   |   |   | └─PropertyAccessSyntax
//@[14:0039) |   |   |   |   |   ├─PropertyAccessSyntax
//@[14:0028) |   |   |   |   |   | ├─VariableAccessSyntax
//@[14:0028) |   |   |   |   |   | | └─IdentifierSyntax
//@[14:0028) |   |   |   |   |   | |   └─Token(Identifier) |existingParent|
//@[28:0029) |   |   |   |   |   | ├─Token(Dot) |.|
//@[29:0039) |   |   |   |   |   | └─IdentifierSyntax
//@[29:0039) |   |   |   |   |   |   └─Token(Identifier) |properties|
//@[39:0040) |   |   |   |   |   ├─Token(Dot) |.|
//@[40:0044) |   |   |   |   |   └─IdentifierSyntax
//@[40:0044) |   |   |   |   |     └─Token(Identifier) |size|
//@[44:0045) |   |   |   |   ├─Token(NewLine) |\n|
        style: existingChild.properties.style
//@[08:0045) |   |   |   |   ├─ObjectPropertySyntax
//@[08:0013) |   |   |   |   | ├─IdentifierSyntax
//@[08:0013) |   |   |   |   | | └─Token(Identifier) |style|
//@[13:0014) |   |   |   |   | ├─Token(Colon) |:|
//@[15:0045) |   |   |   |   | └─PropertyAccessSyntax
//@[15:0039) |   |   |   |   |   ├─PropertyAccessSyntax
//@[15:0028) |   |   |   |   |   | ├─VariableAccessSyntax
//@[15:0028) |   |   |   |   |   | | └─IdentifierSyntax
//@[15:0028) |   |   |   |   |   | |   └─Token(Identifier) |existingChild|
//@[28:0029) |   |   |   |   |   | ├─Token(Dot) |.|
//@[29:0039) |   |   |   |   |   | └─IdentifierSyntax
//@[29:0039) |   |   |   |   |   |   └─Token(Identifier) |properties|
//@[39:0040) |   |   |   |   |   ├─Token(Dot) |.|
//@[40:0045) |   |   |   |   |   └─IdentifierSyntax
//@[40:0045) |   |   |   |   |     └─Token(Identifier) |style|
//@[45:0046) |   |   |   |   ├─Token(NewLine) |\n|
      }
//@[06:0007) |   |   |   |   └─Token(RightBrace) |}|
//@[07:0008) |   |   |   ├─Token(NewLine) |\n|
    }
//@[04:0005) |   |   |   └─Token(RightBrace) |}|
//@[05:0006) |   |   ├─Token(NewLine) |\n|
  }
//@[02:0003) |   |   └─Token(RightBrace) |}|
//@[03:0004) |   ├─Token(NewLine) |\n|
}
//@[00:0001) |   └─Token(RightBrace) |}|
//@[01:0003) ├─Token(NewLine) |\n\n|

param createParent bool
//@[00:0023) ├─ParameterDeclarationSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0018) | ├─IdentifierSyntax
//@[06:0018) | | └─Token(Identifier) |createParent|
//@[19:0023) | └─SimpleTypeSyntax
//@[19:0023) |   └─Token(Identifier) |bool|
//@[23:0024) ├─Token(NewLine) |\n|
param createChild bool
//@[00:0022) ├─ParameterDeclarationSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0017) | ├─IdentifierSyntax
//@[06:0017) | | └─Token(Identifier) |createChild|
//@[18:0022) | └─SimpleTypeSyntax
//@[18:0022) |   └─Token(Identifier) |bool|
//@[22:0023) ├─Token(NewLine) |\n|
param createGrandchild bool
//@[00:0027) ├─ParameterDeclarationSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0022) | ├─IdentifierSyntax
//@[06:0022) | | └─Token(Identifier) |createGrandchild|
//@[23:0027) | └─SimpleTypeSyntax
//@[23:0027) |   └─Token(Identifier) |bool|
//@[27:0028) ├─Token(NewLine) |\n|
resource conditionParent 'My.Rp/parentType@2020-12-01' = if (createParent) {
//@[00:0433) ├─ResourceDeclarationSyntax
//@[00:0008) | ├─Token(Identifier) |resource|
//@[09:0024) | ├─IdentifierSyntax
//@[09:0024) | | └─Token(Identifier) |conditionParent|
//@[25:0054) | ├─StringSyntax
//@[25:0054) | | └─Token(StringComplete) |'My.Rp/parentType@2020-12-01'|
//@[55:0056) | ├─Token(Assignment) |=|
//@[57:0433) | └─IfConditionSyntax
//@[57:0059) |   ├─Token(Identifier) |if|
//@[60:0074) |   ├─ParenthesizedExpressionSyntax
//@[60:0061) |   | ├─Token(LeftParen) |(|
//@[61:0073) |   | ├─VariableAccessSyntax
//@[61:0073) |   | | └─IdentifierSyntax
//@[61:0073) |   | |   └─Token(Identifier) |createParent|
//@[73:0074) |   | └─Token(RightParen) |)|
//@[75:0433) |   └─ObjectSyntax
//@[75:0076) |     ├─Token(LeftBrace) |{|
//@[76:0077) |     ├─Token(NewLine) |\n|
  name: 'conditionParent'
//@[02:0025) |     ├─ObjectPropertySyntax
//@[02:0006) |     | ├─IdentifierSyntax
//@[02:0006) |     | | └─Token(Identifier) |name|
//@[06:0007) |     | ├─Token(Colon) |:|
//@[08:0025) |     | └─StringSyntax
//@[08:0025) |     |   └─Token(StringComplete) |'conditionParent'|
//@[25:0027) |     ├─Token(NewLine) |\n\n|

  resource conditionChild 'childType' = if (createChild) {
//@[02:0327) |     ├─ResourceDeclarationSyntax
//@[02:0010) |     | ├─Token(Identifier) |resource|
//@[11:0025) |     | ├─IdentifierSyntax
//@[11:0025) |     | | └─Token(Identifier) |conditionChild|
//@[26:0037) |     | ├─StringSyntax
//@[26:0037) |     | | └─Token(StringComplete) |'childType'|
//@[38:0039) |     | ├─Token(Assignment) |=|
//@[40:0327) |     | └─IfConditionSyntax
//@[40:0042) |     |   ├─Token(Identifier) |if|
//@[43:0056) |     |   ├─ParenthesizedExpressionSyntax
//@[43:0044) |     |   | ├─Token(LeftParen) |(|
//@[44:0055) |     |   | ├─VariableAccessSyntax
//@[44:0055) |     |   | | └─IdentifierSyntax
//@[44:0055) |     |   | |   └─Token(Identifier) |createChild|
//@[55:0056) |     |   | └─Token(RightParen) |)|
//@[57:0327) |     |   └─ObjectSyntax
//@[57:0058) |     |     ├─Token(LeftBrace) |{|
//@[58:0059) |     |     ├─Token(NewLine) |\n|
    name: 'conditionChild'
//@[04:0026) |     |     ├─ObjectPropertySyntax
//@[04:0008) |     |     | ├─IdentifierSyntax
//@[04:0008) |     |     | | └─Token(Identifier) |name|
//@[08:0009) |     |     | ├─Token(Colon) |:|
//@[10:0026) |     |     | └─StringSyntax
//@[10:0026) |     |     |   └─Token(StringComplete) |'conditionChild'|
//@[26:0028) |     |     ├─Token(NewLine) |\n\n|

    resource conditionGrandchild 'grandchildType' = if (createGrandchild) {
//@[04:0236) |     |     ├─ResourceDeclarationSyntax
//@[04:0012) |     |     | ├─Token(Identifier) |resource|
//@[13:0032) |     |     | ├─IdentifierSyntax
//@[13:0032) |     |     | | └─Token(Identifier) |conditionGrandchild|
//@[33:0049) |     |     | ├─StringSyntax
//@[33:0049) |     |     | | └─Token(StringComplete) |'grandchildType'|
//@[50:0051) |     |     | ├─Token(Assignment) |=|
//@[52:0236) |     |     | └─IfConditionSyntax
//@[52:0054) |     |     |   ├─Token(Identifier) |if|
//@[55:0073) |     |     |   ├─ParenthesizedExpressionSyntax
//@[55:0056) |     |     |   | ├─Token(LeftParen) |(|
//@[56:0072) |     |     |   | ├─VariableAccessSyntax
//@[56:0072) |     |     |   | | └─IdentifierSyntax
//@[56:0072) |     |     |   | |   └─Token(Identifier) |createGrandchild|
//@[72:0073) |     |     |   | └─Token(RightParen) |)|
//@[74:0236) |     |     |   └─ObjectSyntax
//@[74:0075) |     |     |     ├─Token(LeftBrace) |{|
//@[75:0076) |     |     |     ├─Token(NewLine) |\n|
      name: 'conditionGrandchild'
//@[06:0033) |     |     |     ├─ObjectPropertySyntax
//@[06:0010) |     |     |     | ├─IdentifierSyntax
//@[06:0010) |     |     |     | | └─Token(Identifier) |name|
//@[10:0011) |     |     |     | ├─Token(Colon) |:|
//@[12:0033) |     |     |     | └─StringSyntax
//@[12:0033) |     |     |     |   └─Token(StringComplete) |'conditionGrandchild'|
//@[33:0034) |     |     |     ├─Token(NewLine) |\n|
      properties: {
//@[06:0120) |     |     |     ├─ObjectPropertySyntax
//@[06:0016) |     |     |     | ├─IdentifierSyntax
//@[06:0016) |     |     |     | | └─Token(Identifier) |properties|
//@[16:0017) |     |     |     | ├─Token(Colon) |:|
//@[18:0120) |     |     |     | └─ObjectSyntax
//@[18:0019) |     |     |     |   ├─Token(LeftBrace) |{|
//@[19:0020) |     |     |     |   ├─Token(NewLine) |\n|
        size: conditionParent.properties.size
//@[08:0045) |     |     |     |   ├─ObjectPropertySyntax
//@[08:0012) |     |     |     |   | ├─IdentifierSyntax
//@[08:0012) |     |     |     |   | | └─Token(Identifier) |size|
//@[12:0013) |     |     |     |   | ├─Token(Colon) |:|
//@[14:0045) |     |     |     |   | └─PropertyAccessSyntax
//@[14:0040) |     |     |     |   |   ├─PropertyAccessSyntax
//@[14:0029) |     |     |     |   |   | ├─VariableAccessSyntax
//@[14:0029) |     |     |     |   |   | | └─IdentifierSyntax
//@[14:0029) |     |     |     |   |   | |   └─Token(Identifier) |conditionParent|
//@[29:0030) |     |     |     |   |   | ├─Token(Dot) |.|
//@[30:0040) |     |     |     |   |   | └─IdentifierSyntax
//@[30:0040) |     |     |     |   |   |   └─Token(Identifier) |properties|
//@[40:0041) |     |     |     |   |   ├─Token(Dot) |.|
//@[41:0045) |     |     |     |   |   └─IdentifierSyntax
//@[41:0045) |     |     |     |   |     └─Token(Identifier) |size|
//@[45:0046) |     |     |     |   ├─Token(NewLine) |\n|
        style: conditionChild.properties.style
//@[08:0046) |     |     |     |   ├─ObjectPropertySyntax
//@[08:0013) |     |     |     |   | ├─IdentifierSyntax
//@[08:0013) |     |     |     |   | | └─Token(Identifier) |style|
//@[13:0014) |     |     |     |   | ├─Token(Colon) |:|
//@[15:0046) |     |     |     |   | └─PropertyAccessSyntax
//@[15:0040) |     |     |     |   |   ├─PropertyAccessSyntax
//@[15:0029) |     |     |     |   |   | ├─VariableAccessSyntax
//@[15:0029) |     |     |     |   |   | | └─IdentifierSyntax
//@[15:0029) |     |     |     |   |   | |   └─Token(Identifier) |conditionChild|
//@[29:0030) |     |     |     |   |   | ├─Token(Dot) |.|
//@[30:0040) |     |     |     |   |   | └─IdentifierSyntax
//@[30:0040) |     |     |     |   |   |   └─Token(Identifier) |properties|
//@[40:0041) |     |     |     |   |   ├─Token(Dot) |.|
//@[41:0046) |     |     |     |   |   └─IdentifierSyntax
//@[41:0046) |     |     |     |   |     └─Token(Identifier) |style|
//@[46:0047) |     |     |     |   ├─Token(NewLine) |\n|
      }
//@[06:0007) |     |     |     |   └─Token(RightBrace) |}|
//@[07:0008) |     |     |     ├─Token(NewLine) |\n|
    }
//@[04:0005) |     |     |     └─Token(RightBrace) |}|
//@[05:0006) |     |     ├─Token(NewLine) |\n|
  }
//@[02:0003) |     |     └─Token(RightBrace) |}|
//@[03:0004) |     ├─Token(NewLine) |\n|
}
//@[00:0001) |     └─Token(RightBrace) |}|
//@[01:0003) ├─Token(NewLine) |\n\n|

var items = [
//@[00:0027) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0009) | ├─IdentifierSyntax
//@[04:0009) | | └─Token(Identifier) |items|
//@[10:0011) | ├─Token(Assignment) |=|
//@[12:0027) | └─ArraySyntax
//@[12:0013) |   ├─Token(LeftSquare) |[|
//@[13:0014) |   ├─Token(NewLine) |\n|
  'a'
//@[02:0005) |   ├─ArrayItemSyntax
//@[02:0005) |   | └─StringSyntax
//@[02:0005) |   |   └─Token(StringComplete) |'a'|
//@[05:0006) |   ├─Token(NewLine) |\n|
  'b'
//@[02:0005) |   ├─ArrayItemSyntax
//@[02:0005) |   | └─StringSyntax
//@[02:0005) |   |   └─Token(StringComplete) |'b'|
//@[05:0006) |   ├─Token(NewLine) |\n|
]
//@[00:0001) |   └─Token(RightSquare) |]|
//@[01:0002) ├─Token(NewLine) |\n|
resource loopParent 'My.Rp/parentType@2020-12-01' = {
//@[00:0161) ├─ResourceDeclarationSyntax
//@[00:0008) | ├─Token(Identifier) |resource|
//@[09:0019) | ├─IdentifierSyntax
//@[09:0019) | | └─Token(Identifier) |loopParent|
//@[20:0049) | ├─StringSyntax
//@[20:0049) | | └─Token(StringComplete) |'My.Rp/parentType@2020-12-01'|
//@[50:0051) | ├─Token(Assignment) |=|
//@[52:0161) | └─ObjectSyntax
//@[52:0053) |   ├─Token(LeftBrace) |{|
//@[53:0054) |   ├─Token(NewLine) |\n|
  name: 'loopParent'
//@[02:0020) |   ├─ObjectPropertySyntax
//@[02:0006) |   | ├─IdentifierSyntax
//@[02:0006) |   | | └─Token(Identifier) |name|
//@[06:0007) |   | ├─Token(Colon) |:|
//@[08:0020) |   | └─StringSyntax
//@[08:0020) |   |   └─Token(StringComplete) |'loopParent'|
//@[20:0022) |   ├─Token(NewLine) |\n\n|

  resource loopChild 'childType' = [for item in items: {
//@[02:0083) |   ├─ResourceDeclarationSyntax
//@[02:0010) |   | ├─Token(Identifier) |resource|
//@[11:0020) |   | ├─IdentifierSyntax
//@[11:0020) |   | | └─Token(Identifier) |loopChild|
//@[21:0032) |   | ├─StringSyntax
//@[21:0032) |   | | └─Token(StringComplete) |'childType'|
//@[33:0034) |   | ├─Token(Assignment) |=|
//@[35:0083) |   | └─ForSyntax
//@[35:0036) |   |   ├─Token(LeftSquare) |[|
//@[36:0039) |   |   ├─Token(Identifier) |for|
//@[40:0044) |   |   ├─LocalVariableSyntax
//@[40:0044) |   |   | └─IdentifierSyntax
//@[40:0044) |   |   |   └─Token(Identifier) |item|
//@[45:0047) |   |   ├─Token(Identifier) |in|
//@[48:0053) |   |   ├─VariableAccessSyntax
//@[48:0053) |   |   | └─IdentifierSyntax
//@[48:0053) |   |   |   └─Token(Identifier) |items|
//@[53:0054) |   |   ├─Token(Colon) |:|
//@[55:0082) |   |   ├─ObjectSyntax
//@[55:0056) |   |   | ├─Token(LeftBrace) |{|
//@[56:0057) |   |   | ├─Token(NewLine) |\n|
    name: 'loopChild'
//@[04:0021) |   |   | ├─ObjectPropertySyntax
//@[04:0008) |   |   | | ├─IdentifierSyntax
//@[04:0008) |   |   | | | └─Token(Identifier) |name|
//@[08:0009) |   |   | | ├─Token(Colon) |:|
//@[10:0021) |   |   | | └─StringSyntax
//@[10:0021) |   |   | |   └─Token(StringComplete) |'loopChild'|
//@[21:0022) |   |   | ├─Token(NewLine) |\n|
  }]
//@[02:0003) |   |   | └─Token(RightBrace) |}|
//@[03:0004) |   |   └─Token(RightSquare) |]|
//@[04:0005) |   ├─Token(NewLine) |\n|
}
//@[00:0001) |   └─Token(RightBrace) |}|
//@[01:0003) ├─Token(NewLine) |\n\n|

output loopChildOutput string = loopParent::loopChild[0].name
//@[00:0061) ├─OutputDeclarationSyntax
//@[00:0006) | ├─Token(Identifier) |output|
//@[07:0022) | ├─IdentifierSyntax
//@[07:0022) | | └─Token(Identifier) |loopChildOutput|
//@[23:0029) | ├─SimpleTypeSyntax
//@[23:0029) | | └─Token(Identifier) |string|
//@[30:0031) | ├─Token(Assignment) |=|
//@[32:0061) | └─PropertyAccessSyntax
//@[32:0056) |   ├─ArrayAccessSyntax
//@[32:0053) |   | ├─ResourceAccessSyntax
//@[32:0042) |   | | ├─VariableAccessSyntax
//@[32:0042) |   | | | └─IdentifierSyntax
//@[32:0042) |   | | |   └─Token(Identifier) |loopParent|
//@[42:0044) |   | | ├─Token(DoubleColon) |::|
//@[44:0053) |   | | └─IdentifierSyntax
//@[44:0053) |   | |   └─Token(Identifier) |loopChild|
//@[53:0054) |   | ├─Token(LeftSquare) |[|
//@[54:0055) |   | ├─IntegerLiteralSyntax
//@[54:0055) |   | | └─Token(Integer) |0|
//@[55:0056) |   | └─Token(RightSquare) |]|
//@[56:0057) |   ├─Token(Dot) |.|
//@[57:0061) |   └─IdentifierSyntax
//@[57:0061) |     └─Token(Identifier) |name|
//@[61:0061) └─Token(EndOfFile) ||
