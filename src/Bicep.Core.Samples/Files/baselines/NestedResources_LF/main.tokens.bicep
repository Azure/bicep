resource basicParent 'My.Rp/parentType@2020-12-01' = {
//@[00:008) Identifier |resource|
//@[09:020) Identifier |basicParent|
//@[21:050) StringComplete |'My.Rp/parentType@2020-12-01'|
//@[51:052) Assignment |=|
//@[53:054) LeftBrace |{|
//@[54:055) NewLine |\n|
  name: 'basicParent'
//@[02:006) Identifier |name|
//@[06:007) Colon |:|
//@[08:021) StringComplete |'basicParent'|
//@[21:022) NewLine |\n|
  properties: {
//@[02:012) Identifier |properties|
//@[12:013) Colon |:|
//@[14:015) LeftBrace |{|
//@[15:016) NewLine |\n|
    size: 'large'
//@[04:008) Identifier |size|
//@[08:009) Colon |:|
//@[10:017) StringComplete |'large'|
//@[17:018) NewLine |\n|
  }
//@[02:003) RightBrace |}|
//@[03:005) NewLine |\n\n|

  resource basicChild 'childType' = {
//@[02:010) Identifier |resource|
//@[11:021) Identifier |basicChild|
//@[22:033) StringComplete |'childType'|
//@[34:035) Assignment |=|
//@[36:037) LeftBrace |{|
//@[37:038) NewLine |\n|
    name: 'basicChild'
//@[04:008) Identifier |name|
//@[08:009) Colon |:|
//@[10:022) StringComplete |'basicChild'|
//@[22:023) NewLine |\n|
    properties: {
//@[04:014) Identifier |properties|
//@[14:015) Colon |:|
//@[16:017) LeftBrace |{|
//@[17:018) NewLine |\n|
      size: basicParent.properties.large
//@[06:010) Identifier |size|
//@[10:011) Colon |:|
//@[12:023) Identifier |basicParent|
//@[23:024) Dot |.|
//@[24:034) Identifier |properties|
//@[34:035) Dot |.|
//@[35:040) Identifier |large|
//@[40:041) NewLine |\n|
      style: 'cool'
//@[06:011) Identifier |style|
//@[11:012) Colon |:|
//@[13:019) StringComplete |'cool'|
//@[19:020) NewLine |\n|
    }
//@[04:005) RightBrace |}|
//@[05:007) NewLine |\n\n|

    resource basicGrandchild 'grandchildType' = {
//@[04:012) Identifier |resource|
//@[13:028) Identifier |basicGrandchild|
//@[29:045) StringComplete |'grandchildType'|
//@[46:047) Assignment |=|
//@[48:049) LeftBrace |{|
//@[49:050) NewLine |\n|
      name: 'basicGrandchild'
//@[06:010) Identifier |name|
//@[10:011) Colon |:|
//@[12:029) StringComplete |'basicGrandchild'|
//@[29:030) NewLine |\n|
      properties: {
//@[06:016) Identifier |properties|
//@[16:017) Colon |:|
//@[18:019) LeftBrace |{|
//@[19:020) NewLine |\n|
        size: basicParent.properties.size
//@[08:012) Identifier |size|
//@[12:013) Colon |:|
//@[14:025) Identifier |basicParent|
//@[25:026) Dot |.|
//@[26:036) Identifier |properties|
//@[36:037) Dot |.|
//@[37:041) Identifier |size|
//@[41:042) NewLine |\n|
        style: basicChild.properties.style
//@[08:013) Identifier |style|
//@[13:014) Colon |:|
//@[15:025) Identifier |basicChild|
//@[25:026) Dot |.|
//@[26:036) Identifier |properties|
//@[36:037) Dot |.|
//@[37:042) Identifier |style|
//@[42:043) NewLine |\n|
      }
//@[06:007) RightBrace |}|
//@[07:008) NewLine |\n|
    }
//@[04:005) RightBrace |}|
//@[05:006) NewLine |\n|
  }
//@[02:003) RightBrace |}|
//@[03:005) NewLine |\n\n|

  resource basicSibling 'childType' = {
//@[02:010) Identifier |resource|
//@[11:023) Identifier |basicSibling|
//@[24:035) StringComplete |'childType'|
//@[36:037) Assignment |=|
//@[38:039) LeftBrace |{|
//@[39:040) NewLine |\n|
    name: 'basicSibling'
//@[04:008) Identifier |name|
//@[08:009) Colon |:|
//@[10:024) StringComplete |'basicSibling'|
//@[24:025) NewLine |\n|
    properties: {
//@[04:014) Identifier |properties|
//@[14:015) Colon |:|
//@[16:017) LeftBrace |{|
//@[17:018) NewLine |\n|
      size: basicParent.properties.size
//@[06:010) Identifier |size|
//@[10:011) Colon |:|
//@[12:023) Identifier |basicParent|
//@[23:024) Dot |.|
//@[24:034) Identifier |properties|
//@[34:035) Dot |.|
//@[35:039) Identifier |size|
//@[39:040) NewLine |\n|
      style: basicChild::basicGrandchild.properties.style
//@[06:011) Identifier |style|
//@[11:012) Colon |:|
//@[13:023) Identifier |basicChild|
//@[23:025) DoubleColon |::|
//@[25:040) Identifier |basicGrandchild|
//@[40:041) Dot |.|
//@[41:051) Identifier |properties|
//@[51:052) Dot |.|
//@[52:057) Identifier |style|
//@[57:058) NewLine |\n|
    }
//@[04:005) RightBrace |}|
//@[05:006) NewLine |\n|
  }
//@[02:003) RightBrace |}|
//@[03:004) NewLine |\n|
}
//@[00:001) RightBrace |}|
//@[01:002) NewLine |\n|
// #completionTest(50) -> childResources
//@[40:041) NewLine |\n|
output referenceBasicChild string = basicParent::basicChild.properties.size
//@[00:006) Identifier |output|
//@[07:026) Identifier |referenceBasicChild|
//@[27:033) Identifier |string|
//@[34:035) Assignment |=|
//@[36:047) Identifier |basicParent|
//@[47:049) DoubleColon |::|
//@[49:059) Identifier |basicChild|
//@[59:060) Dot |.|
//@[60:070) Identifier |properties|
//@[70:071) Dot |.|
//@[71:075) Identifier |size|
//@[75:076) NewLine |\n|
// #completionTest(67) -> grandChildResources
//@[45:046) NewLine |\n|
output referenceBasicGrandchild string = basicParent::basicChild::basicGrandchild.properties.style
//@[00:006) Identifier |output|
//@[07:031) Identifier |referenceBasicGrandchild|
//@[32:038) Identifier |string|
//@[39:040) Assignment |=|
//@[41:052) Identifier |basicParent|
//@[52:054) DoubleColon |::|
//@[54:064) Identifier |basicChild|
//@[64:066) DoubleColon |::|
//@[66:081) Identifier |basicGrandchild|
//@[81:082) Dot |.|
//@[82:092) Identifier |properties|
//@[92:093) Dot |.|
//@[93:098) Identifier |style|
//@[98:100) NewLine |\n\n|

resource existingParent 'My.Rp/parentType@2020-12-01' existing = {
//@[00:008) Identifier |resource|
//@[09:023) Identifier |existingParent|
//@[24:053) StringComplete |'My.Rp/parentType@2020-12-01'|
//@[54:062) Identifier |existing|
//@[63:064) Assignment |=|
//@[65:066) LeftBrace |{|
//@[66:067) NewLine |\n|
  name: 'existingParent'
//@[02:006) Identifier |name|
//@[06:007) Colon |:|
//@[08:024) StringComplete |'existingParent'|
//@[24:026) NewLine |\n\n|

  resource existingChild 'childType' existing = {
//@[02:010) Identifier |resource|
//@[11:024) Identifier |existingChild|
//@[25:036) StringComplete |'childType'|
//@[37:045) Identifier |existing|
//@[46:047) Assignment |=|
//@[48:049) LeftBrace |{|
//@[49:050) NewLine |\n|
    name: 'existingChild'
//@[04:008) Identifier |name|
//@[08:009) Colon |:|
//@[10:025) StringComplete |'existingChild'|
//@[25:027) NewLine |\n\n|

    resource existingGrandchild 'grandchildType' = {
//@[04:012) Identifier |resource|
//@[13:031) Identifier |existingGrandchild|
//@[32:048) StringComplete |'grandchildType'|
//@[49:050) Assignment |=|
//@[51:052) LeftBrace |{|
//@[52:053) NewLine |\n|
      name: 'existingGrandchild'
//@[06:010) Identifier |name|
//@[10:011) Colon |:|
//@[12:032) StringComplete |'existingGrandchild'|
//@[32:033) NewLine |\n|
      properties: {
//@[06:016) Identifier |properties|
//@[16:017) Colon |:|
//@[18:019) LeftBrace |{|
//@[19:020) NewLine |\n|
        size: existingParent.properties.size
//@[08:012) Identifier |size|
//@[12:013) Colon |:|
//@[14:028) Identifier |existingParent|
//@[28:029) Dot |.|
//@[29:039) Identifier |properties|
//@[39:040) Dot |.|
//@[40:044) Identifier |size|
//@[44:045) NewLine |\n|
        style: existingChild.properties.style
//@[08:013) Identifier |style|
//@[13:014) Colon |:|
//@[15:028) Identifier |existingChild|
//@[28:029) Dot |.|
//@[29:039) Identifier |properties|
//@[39:040) Dot |.|
//@[40:045) Identifier |style|
//@[45:046) NewLine |\n|
      }
//@[06:007) RightBrace |}|
//@[07:008) NewLine |\n|
    }
//@[04:005) RightBrace |}|
//@[05:006) NewLine |\n|
  }
//@[02:003) RightBrace |}|
//@[03:004) NewLine |\n|
}
//@[00:001) RightBrace |}|
//@[01:003) NewLine |\n\n|

param createParent bool
//@[00:005) Identifier |param|
//@[06:018) Identifier |createParent|
//@[19:023) Identifier |bool|
//@[23:024) NewLine |\n|
param createChild bool
//@[00:005) Identifier |param|
//@[06:017) Identifier |createChild|
//@[18:022) Identifier |bool|
//@[22:023) NewLine |\n|
param createGrandchild bool
//@[00:005) Identifier |param|
//@[06:022) Identifier |createGrandchild|
//@[23:027) Identifier |bool|
//@[27:028) NewLine |\n|
resource conditionParent 'My.Rp/parentType@2020-12-01' = if (createParent) {
//@[00:008) Identifier |resource|
//@[09:024) Identifier |conditionParent|
//@[25:054) StringComplete |'My.Rp/parentType@2020-12-01'|
//@[55:056) Assignment |=|
//@[57:059) Identifier |if|
//@[60:061) LeftParen |(|
//@[61:073) Identifier |createParent|
//@[73:074) RightParen |)|
//@[75:076) LeftBrace |{|
//@[76:077) NewLine |\n|
  name: 'conditionParent'
//@[02:006) Identifier |name|
//@[06:007) Colon |:|
//@[08:025) StringComplete |'conditionParent'|
//@[25:027) NewLine |\n\n|

  resource conditionChild 'childType' = if (createChild) {
//@[02:010) Identifier |resource|
//@[11:025) Identifier |conditionChild|
//@[26:037) StringComplete |'childType'|
//@[38:039) Assignment |=|
//@[40:042) Identifier |if|
//@[43:044) LeftParen |(|
//@[44:055) Identifier |createChild|
//@[55:056) RightParen |)|
//@[57:058) LeftBrace |{|
//@[58:059) NewLine |\n|
    name: 'conditionChild'
//@[04:008) Identifier |name|
//@[08:009) Colon |:|
//@[10:026) StringComplete |'conditionChild'|
//@[26:028) NewLine |\n\n|

    resource conditionGrandchild 'grandchildType' = if (createGrandchild) {
//@[04:012) Identifier |resource|
//@[13:032) Identifier |conditionGrandchild|
//@[33:049) StringComplete |'grandchildType'|
//@[50:051) Assignment |=|
//@[52:054) Identifier |if|
//@[55:056) LeftParen |(|
//@[56:072) Identifier |createGrandchild|
//@[72:073) RightParen |)|
//@[74:075) LeftBrace |{|
//@[75:076) NewLine |\n|
      name: 'conditionGrandchild'
//@[06:010) Identifier |name|
//@[10:011) Colon |:|
//@[12:033) StringComplete |'conditionGrandchild'|
//@[33:034) NewLine |\n|
      properties: {
//@[06:016) Identifier |properties|
//@[16:017) Colon |:|
//@[18:019) LeftBrace |{|
//@[19:020) NewLine |\n|
        size: conditionParent.properties.size
//@[08:012) Identifier |size|
//@[12:013) Colon |:|
//@[14:029) Identifier |conditionParent|
//@[29:030) Dot |.|
//@[30:040) Identifier |properties|
//@[40:041) Dot |.|
//@[41:045) Identifier |size|
//@[45:046) NewLine |\n|
        style: conditionChild.properties.style
//@[08:013) Identifier |style|
//@[13:014) Colon |:|
//@[15:029) Identifier |conditionChild|
//@[29:030) Dot |.|
//@[30:040) Identifier |properties|
//@[40:041) Dot |.|
//@[41:046) Identifier |style|
//@[46:047) NewLine |\n|
      }
//@[06:007) RightBrace |}|
//@[07:008) NewLine |\n|
    }
//@[04:005) RightBrace |}|
//@[05:006) NewLine |\n|
  }
//@[02:003) RightBrace |}|
//@[03:004) NewLine |\n|
}
//@[00:001) RightBrace |}|
//@[01:003) NewLine |\n\n|

var items = [
//@[00:003) Identifier |var|
//@[04:009) Identifier |items|
//@[10:011) Assignment |=|
//@[12:013) LeftSquare |[|
//@[13:014) NewLine |\n|
  'a'
//@[02:005) StringComplete |'a'|
//@[05:006) NewLine |\n|
  'b'
//@[02:005) StringComplete |'b'|
//@[05:006) NewLine |\n|
]
//@[00:001) RightSquare |]|
//@[01:002) NewLine |\n|
resource loopParent 'My.Rp/parentType@2020-12-01' = {
//@[00:008) Identifier |resource|
//@[09:019) Identifier |loopParent|
//@[20:049) StringComplete |'My.Rp/parentType@2020-12-01'|
//@[50:051) Assignment |=|
//@[52:053) LeftBrace |{|
//@[53:054) NewLine |\n|
  name: 'loopParent'
//@[02:006) Identifier |name|
//@[06:007) Colon |:|
//@[08:020) StringComplete |'loopParent'|
//@[20:022) NewLine |\n\n|

  resource loopChild 'childType' = [for item in items: {
//@[02:010) Identifier |resource|
//@[11:020) Identifier |loopChild|
//@[21:032) StringComplete |'childType'|
//@[33:034) Assignment |=|
//@[35:036) LeftSquare |[|
//@[36:039) Identifier |for|
//@[40:044) Identifier |item|
//@[45:047) Identifier |in|
//@[48:053) Identifier |items|
//@[53:054) Colon |:|
//@[55:056) LeftBrace |{|
//@[56:057) NewLine |\n|
    name: 'loopChild'
//@[04:008) Identifier |name|
//@[08:009) Colon |:|
//@[10:021) StringComplete |'loopChild'|
//@[21:022) NewLine |\n|
  }]
//@[02:003) RightBrace |}|
//@[03:004) RightSquare |]|
//@[04:005) NewLine |\n|
}
//@[00:001) RightBrace |}|
//@[01:003) NewLine |\n\n|

output loopChildOutput string = loopParent::loopChild[0].name
//@[00:006) Identifier |output|
//@[07:022) Identifier |loopChildOutput|
//@[23:029) Identifier |string|
//@[30:031) Assignment |=|
//@[32:042) Identifier |loopParent|
//@[42:044) DoubleColon |::|
//@[44:053) Identifier |loopChild|
//@[53:054) LeftSquare |[|
//@[54:055) Integer |0|
//@[55:056) RightSquare |]|
//@[56:057) Dot |.|
//@[57:061) Identifier |name|
//@[61:061) EndOfFile ||
