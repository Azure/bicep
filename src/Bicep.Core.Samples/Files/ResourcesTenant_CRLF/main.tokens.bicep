targetScope = 'tenant'
//@[000:011) Identifier |targetScope|
//@[012:013) Assignment |=|
//@[014:022) StringComplete |'tenant'|
//@[022:026) NewLine |\r\n\r\n|

var managementGroups = [
//@[000:003) Identifier |var|
//@[004:020) Identifier |managementGroups|
//@[021:022) Assignment |=|
//@[023:024) LeftSquare |[|
//@[024:026) NewLine |\r\n|
  {
//@[002:003) LeftBrace |{|
//@[003:005) NewLine |\r\n|
    name: 'one'
//@[004:008) Identifier |name|
//@[008:009) Colon |:|
//@[010:015) StringComplete |'one'|
//@[015:017) NewLine |\r\n|
    displayName: 'The first'
//@[004:015) Identifier |displayName|
//@[015:016) Colon |:|
//@[017:028) StringComplete |'The first'|
//@[028:030) NewLine |\r\n|
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
    displayName: 'The second'
//@[004:015) Identifier |displayName|
//@[015:016) Colon |:|
//@[017:029) StringComplete |'The second'|
//@[029:031) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
]
//@[000:001) RightSquare |]|
//@[001:005) NewLine |\r\n\r\n|

resource singleGroup 'Microsoft.Management/managementGroups@2020-05-01' = {
//@[000:008) Identifier |resource|
//@[009:020) Identifier |singleGroup|
//@[021:071) StringComplete |'Microsoft.Management/managementGroups@2020-05-01'|
//@[072:073) Assignment |=|
//@[074:075) LeftBrace |{|
//@[075:077) NewLine |\r\n|
  name: 'myMG'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:014) StringComplete |'myMG'|
//@[014:016) NewLine |\r\n|
  properties: {
//@[002:012) Identifier |properties|
//@[012:013) Colon |:|
//@[014:015) LeftBrace |{|
//@[015:017) NewLine |\r\n|
    displayName: 'This one is mine!'
//@[004:015) Identifier |displayName|
//@[015:016) Colon |:|
//@[017:036) StringComplete |'This one is mine!'|
//@[036:038) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

resource manyGroups 'Microsoft.Management/managementGroups@2020-05-01' = [for mg in managementGroups: {
//@[000:008) Identifier |resource|
//@[009:019) Identifier |manyGroups|
//@[020:070) StringComplete |'Microsoft.Management/managementGroups@2020-05-01'|
//@[071:072) Assignment |=|
//@[073:074) LeftSquare |[|
//@[074:077) Identifier |for|
//@[078:080) Identifier |mg|
//@[081:083) Identifier |in|
//@[084:100) Identifier |managementGroups|
//@[100:101) Colon |:|
//@[102:103) LeftBrace |{|
//@[103:105) NewLine |\r\n|
  name: mg.name
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:010) Identifier |mg|
//@[010:011) Dot |.|
//@[011:015) Identifier |name|
//@[015:017) NewLine |\r\n|
  properties: {
//@[002:012) Identifier |properties|
//@[012:013) Colon |:|
//@[014:015) LeftBrace |{|
//@[015:017) NewLine |\r\n|
    displayName: '${mg.displayName} (${singleGroup.properties.displayName})'
//@[004:015) Identifier |displayName|
//@[015:016) Colon |:|
//@[017:020) StringLeftPiece |'${|
//@[020:022) Identifier |mg|
//@[022:023) Dot |.|
//@[023:034) Identifier |displayName|
//@[034:039) StringMiddlePiece |} (${|
//@[039:050) Identifier |singleGroup|
//@[050:051) Dot |.|
//@[051:061) Identifier |properties|
//@[061:062) Dot |.|
//@[062:073) Identifier |displayName|
//@[073:076) StringRightPiece |})'|
//@[076:078) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
}]
//@[000:001) RightBrace |}|
//@[001:002) RightSquare |]|
//@[002:006) NewLine |\r\n\r\n|

resource anotherSet 'Microsoft.Management/managementGroups@2020-05-01' = [for (mg, index) in managementGroups: {
//@[000:008) Identifier |resource|
//@[009:019) Identifier |anotherSet|
//@[020:070) StringComplete |'Microsoft.Management/managementGroups@2020-05-01'|
//@[071:072) Assignment |=|
//@[073:074) LeftSquare |[|
//@[074:077) Identifier |for|
//@[078:079) LeftParen |(|
//@[079:081) Identifier |mg|
//@[081:082) Comma |,|
//@[083:088) Identifier |index|
//@[088:089) RightParen |)|
//@[090:092) Identifier |in|
//@[093:109) Identifier |managementGroups|
//@[109:110) Colon |:|
//@[111:112) LeftBrace |{|
//@[112:114) NewLine |\r\n|
  name: concat(mg.name, '-one-', index)
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:014) Identifier |concat|
//@[014:015) LeftParen |(|
//@[015:017) Identifier |mg|
//@[017:018) Dot |.|
//@[018:022) Identifier |name|
//@[022:023) Comma |,|
//@[024:031) StringComplete |'-one-'|
//@[031:032) Comma |,|
//@[033:038) Identifier |index|
//@[038:039) RightParen |)|
//@[039:041) NewLine |\r\n|
  properties: {
//@[002:012) Identifier |properties|
//@[012:013) Colon |:|
//@[014:015) LeftBrace |{|
//@[015:017) NewLine |\r\n|
    displayName: '${mg.displayName} (${singleGroup.properties.displayName}) (set 1) (index ${index})'
//@[004:015) Identifier |displayName|
//@[015:016) Colon |:|
//@[017:020) StringLeftPiece |'${|
//@[020:022) Identifier |mg|
//@[022:023) Dot |.|
//@[023:034) Identifier |displayName|
//@[034:039) StringMiddlePiece |} (${|
//@[039:050) Identifier |singleGroup|
//@[050:051) Dot |.|
//@[051:061) Identifier |properties|
//@[061:062) Dot |.|
//@[062:073) Identifier |displayName|
//@[073:093) StringMiddlePiece |}) (set 1) (index ${|
//@[093:098) Identifier |index|
//@[098:101) StringRightPiece |})'|
//@[101:103) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
  dependsOn: [
//@[002:011) Identifier |dependsOn|
//@[011:012) Colon |:|
//@[013:014) LeftSquare |[|
//@[014:016) NewLine |\r\n|
    manyGroups
//@[004:014) Identifier |manyGroups|
//@[014:016) NewLine |\r\n|
  ]
//@[002:003) RightSquare |]|
//@[003:005) NewLine |\r\n|
}]
//@[000:001) RightBrace |}|
//@[001:002) RightSquare |]|
//@[002:006) NewLine |\r\n\r\n|

resource yetAnotherSet 'Microsoft.Management/managementGroups@2020-05-01' = [for mg in managementGroups: {
//@[000:008) Identifier |resource|
//@[009:022) Identifier |yetAnotherSet|
//@[023:073) StringComplete |'Microsoft.Management/managementGroups@2020-05-01'|
//@[074:075) Assignment |=|
//@[076:077) LeftSquare |[|
//@[077:080) Identifier |for|
//@[081:083) Identifier |mg|
//@[084:086) Identifier |in|
//@[087:103) Identifier |managementGroups|
//@[103:104) Colon |:|
//@[105:106) LeftBrace |{|
//@[106:108) NewLine |\r\n|
  name: concat(mg.name, '-two')
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:014) Identifier |concat|
//@[014:015) LeftParen |(|
//@[015:017) Identifier |mg|
//@[017:018) Dot |.|
//@[018:022) Identifier |name|
//@[022:023) Comma |,|
//@[024:030) StringComplete |'-two'|
//@[030:031) RightParen |)|
//@[031:033) NewLine |\r\n|
  properties: {
//@[002:012) Identifier |properties|
//@[012:013) Colon |:|
//@[014:015) LeftBrace |{|
//@[015:017) NewLine |\r\n|
    displayName: '${mg.displayName} (${singleGroup.properties.displayName}) (set 2)'
//@[004:015) Identifier |displayName|
//@[015:016) Colon |:|
//@[017:020) StringLeftPiece |'${|
//@[020:022) Identifier |mg|
//@[022:023) Dot |.|
//@[023:034) Identifier |displayName|
//@[034:039) StringMiddlePiece |} (${|
//@[039:050) Identifier |singleGroup|
//@[050:051) Dot |.|
//@[051:061) Identifier |properties|
//@[061:062) Dot |.|
//@[062:073) Identifier |displayName|
//@[073:084) StringRightPiece |}) (set 2)'|
//@[084:086) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
  dependsOn: [
//@[002:011) Identifier |dependsOn|
//@[011:012) Colon |:|
//@[013:014) LeftSquare |[|
//@[014:016) NewLine |\r\n|
    anotherSet[0]
//@[004:014) Identifier |anotherSet|
//@[014:015) LeftSquare |[|
//@[015:016) Integer |0|
//@[016:017) RightSquare |]|
//@[017:019) NewLine |\r\n|
  ]
//@[002:003) RightSquare |]|
//@[003:005) NewLine |\r\n|
}]
//@[000:001) RightBrace |}|
//@[001:002) RightSquare |]|
//@[002:006) NewLine |\r\n\r\n|

output managementGroupIds array = [for i in range(0, length(managementGroups)): {
//@[000:006) Identifier |output|
//@[007:025) Identifier |managementGroupIds|
//@[026:031) Identifier |array|
//@[032:033) Assignment |=|
//@[034:035) LeftSquare |[|
//@[035:038) Identifier |for|
//@[039:040) Identifier |i|
//@[041:043) Identifier |in|
//@[044:049) Identifier |range|
//@[049:050) LeftParen |(|
//@[050:051) Integer |0|
//@[051:052) Comma |,|
//@[053:059) Identifier |length|
//@[059:060) LeftParen |(|
//@[060:076) Identifier |managementGroups|
//@[076:077) RightParen |)|
//@[077:078) RightParen |)|
//@[078:079) Colon |:|
//@[080:081) LeftBrace |{|
//@[081:083) NewLine |\r\n|
  name: yetAnotherSet[i].name
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:021) Identifier |yetAnotherSet|
//@[021:022) LeftSquare |[|
//@[022:023) Identifier |i|
//@[023:024) RightSquare |]|
//@[024:025) Dot |.|
//@[025:029) Identifier |name|
//@[029:031) NewLine |\r\n|
  displayName: yetAnotherSet[i].properties.displayName
//@[002:013) Identifier |displayName|
//@[013:014) Colon |:|
//@[015:028) Identifier |yetAnotherSet|
//@[028:029) LeftSquare |[|
//@[029:030) Identifier |i|
//@[030:031) RightSquare |]|
//@[031:032) Dot |.|
//@[032:042) Identifier |properties|
//@[042:043) Dot |.|
//@[043:054) Identifier |displayName|
//@[054:056) NewLine |\r\n|
}]
//@[000:001) RightBrace |}|
//@[001:002) RightSquare |]|
//@[002:004) NewLine |\r\n|

//@[000:000) EndOfFile ||
