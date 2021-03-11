targetScope = 'tenant'
//@[0:11) Identifier |targetScope|
//@[12:13) Assignment |=|
//@[14:22) StringComplete |'tenant'|
//@[22:26) NewLine |\r\n\r\n|

var managementGroups = [
//@[0:3) Identifier |var|
//@[4:20) Identifier |managementGroups|
//@[21:22) Assignment |=|
//@[23:24) LeftSquare |[|
//@[24:26) NewLine |\r\n|
  {
//@[2:3) LeftBrace |{|
//@[3:5) NewLine |\r\n|
    name: 'one'
//@[4:8) Identifier |name|
//@[8:9) Colon |:|
//@[10:15) StringComplete |'one'|
//@[15:17) NewLine |\r\n|
    displayName: 'The first'
//@[4:15) Identifier |displayName|
//@[15:16) Colon |:|
//@[17:28) StringComplete |'The first'|
//@[28:30) NewLine |\r\n|
  }
//@[2:3) RightBrace |}|
//@[3:5) NewLine |\r\n|
  {
//@[2:3) LeftBrace |{|
//@[3:5) NewLine |\r\n|
    name: 'two'
//@[4:8) Identifier |name|
//@[8:9) Colon |:|
//@[10:15) StringComplete |'two'|
//@[15:17) NewLine |\r\n|
    displayName: 'The second'
//@[4:15) Identifier |displayName|
//@[15:16) Colon |:|
//@[17:29) StringComplete |'The second'|
//@[29:31) NewLine |\r\n|
  }
//@[2:3) RightBrace |}|
//@[3:5) NewLine |\r\n|
]
//@[0:1) RightSquare |]|
//@[1:5) NewLine |\r\n\r\n|

resource singleGroup 'Microsoft.Management/managementGroups@2020-05-01' = {
//@[0:8) Identifier |resource|
//@[9:20) Identifier |singleGroup|
//@[21:71) StringComplete |'Microsoft.Management/managementGroups@2020-05-01'|
//@[72:73) Assignment |=|
//@[74:75) LeftBrace |{|
//@[75:77) NewLine |\r\n|
  name: 'myMG'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:14) StringComplete |'myMG'|
//@[14:16) NewLine |\r\n|
  properties: {
//@[2:12) Identifier |properties|
//@[12:13) Colon |:|
//@[14:15) LeftBrace |{|
//@[15:17) NewLine |\r\n|
    displayName: 'This one is mine!'
//@[4:15) Identifier |displayName|
//@[15:16) Colon |:|
//@[17:36) StringComplete |'This one is mine!'|
//@[36:38) NewLine |\r\n|
  }
//@[2:3) RightBrace |}|
//@[3:5) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource manyGroups 'Microsoft.Management/managementGroups@2020-05-01' = [for mg in managementGroups: {
//@[0:8) Identifier |resource|
//@[9:19) Identifier |manyGroups|
//@[20:70) StringComplete |'Microsoft.Management/managementGroups@2020-05-01'|
//@[71:72) Assignment |=|
//@[73:74) LeftSquare |[|
//@[74:77) Identifier |for|
//@[78:80) Identifier |mg|
//@[81:83) Identifier |in|
//@[84:100) Identifier |managementGroups|
//@[100:101) Colon |:|
//@[102:103) LeftBrace |{|
//@[103:105) NewLine |\r\n|
  name: mg.name
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:10) Identifier |mg|
//@[10:11) Dot |.|
//@[11:15) Identifier |name|
//@[15:17) NewLine |\r\n|
  properties: {
//@[2:12) Identifier |properties|
//@[12:13) Colon |:|
//@[14:15) LeftBrace |{|
//@[15:17) NewLine |\r\n|
    displayName: '${mg.displayName} (${singleGroup.properties.displayName})'
//@[4:15) Identifier |displayName|
//@[15:16) Colon |:|
//@[17:20) StringLeftPiece |'${|
//@[20:22) Identifier |mg|
//@[22:23) Dot |.|
//@[23:34) Identifier |displayName|
//@[34:39) StringMiddlePiece |} (${|
//@[39:50) Identifier |singleGroup|
//@[50:51) Dot |.|
//@[51:61) Identifier |properties|
//@[61:62) Dot |.|
//@[62:73) Identifier |displayName|
//@[73:76) StringRightPiece |})'|
//@[76:78) NewLine |\r\n|
  }
//@[2:3) RightBrace |}|
//@[3:5) NewLine |\r\n|
}]
//@[0:1) RightBrace |}|
//@[1:2) RightSquare |]|
//@[2:6) NewLine |\r\n\r\n|

resource anotherSet 'Microsoft.Management/managementGroups@2020-05-01' = [for (mg, index) in managementGroups: {
//@[0:8) Identifier |resource|
//@[9:19) Identifier |anotherSet|
//@[20:70) StringComplete |'Microsoft.Management/managementGroups@2020-05-01'|
//@[71:72) Assignment |=|
//@[73:74) LeftSquare |[|
//@[74:77) Identifier |for|
//@[78:79) LeftParen |(|
//@[79:81) Identifier |mg|
//@[81:82) Comma |,|
//@[83:88) Identifier |index|
//@[88:89) RightParen |)|
//@[90:92) Identifier |in|
//@[93:109) Identifier |managementGroups|
//@[109:110) Colon |:|
//@[111:112) LeftBrace |{|
//@[112:114) NewLine |\r\n|
  name: concat(mg.name, '-one-', index)
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:14) Identifier |concat|
//@[14:15) LeftParen |(|
//@[15:17) Identifier |mg|
//@[17:18) Dot |.|
//@[18:22) Identifier |name|
//@[22:23) Comma |,|
//@[24:31) StringComplete |'-one-'|
//@[31:32) Comma |,|
//@[33:38) Identifier |index|
//@[38:39) RightParen |)|
//@[39:41) NewLine |\r\n|
  properties: {
//@[2:12) Identifier |properties|
//@[12:13) Colon |:|
//@[14:15) LeftBrace |{|
//@[15:17) NewLine |\r\n|
    displayName: '${mg.displayName} (${singleGroup.properties.displayName}) (set 1) (index ${index})'
//@[4:15) Identifier |displayName|
//@[15:16) Colon |:|
//@[17:20) StringLeftPiece |'${|
//@[20:22) Identifier |mg|
//@[22:23) Dot |.|
//@[23:34) Identifier |displayName|
//@[34:39) StringMiddlePiece |} (${|
//@[39:50) Identifier |singleGroup|
//@[50:51) Dot |.|
//@[51:61) Identifier |properties|
//@[61:62) Dot |.|
//@[62:73) Identifier |displayName|
//@[73:93) StringMiddlePiece |}) (set 1) (index ${|
//@[93:98) Identifier |index|
//@[98:101) StringRightPiece |})'|
//@[101:103) NewLine |\r\n|
  }
//@[2:3) RightBrace |}|
//@[3:5) NewLine |\r\n|
  dependsOn: [
//@[2:11) Identifier |dependsOn|
//@[11:12) Colon |:|
//@[13:14) LeftSquare |[|
//@[14:16) NewLine |\r\n|
    manyGroups
//@[4:14) Identifier |manyGroups|
//@[14:16) NewLine |\r\n|
  ]
//@[2:3) RightSquare |]|
//@[3:5) NewLine |\r\n|
}]
//@[0:1) RightBrace |}|
//@[1:2) RightSquare |]|
//@[2:6) NewLine |\r\n\r\n|

resource yetAnotherSet 'Microsoft.Management/managementGroups@2020-05-01' = [for mg in managementGroups: {
//@[0:8) Identifier |resource|
//@[9:22) Identifier |yetAnotherSet|
//@[23:73) StringComplete |'Microsoft.Management/managementGroups@2020-05-01'|
//@[74:75) Assignment |=|
//@[76:77) LeftSquare |[|
//@[77:80) Identifier |for|
//@[81:83) Identifier |mg|
//@[84:86) Identifier |in|
//@[87:103) Identifier |managementGroups|
//@[103:104) Colon |:|
//@[105:106) LeftBrace |{|
//@[106:108) NewLine |\r\n|
  name: concat(mg.name, '-two')
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:14) Identifier |concat|
//@[14:15) LeftParen |(|
//@[15:17) Identifier |mg|
//@[17:18) Dot |.|
//@[18:22) Identifier |name|
//@[22:23) Comma |,|
//@[24:30) StringComplete |'-two'|
//@[30:31) RightParen |)|
//@[31:33) NewLine |\r\n|
  properties: {
//@[2:12) Identifier |properties|
//@[12:13) Colon |:|
//@[14:15) LeftBrace |{|
//@[15:17) NewLine |\r\n|
    displayName: '${mg.displayName} (${singleGroup.properties.displayName}) (set 2)'
//@[4:15) Identifier |displayName|
//@[15:16) Colon |:|
//@[17:20) StringLeftPiece |'${|
//@[20:22) Identifier |mg|
//@[22:23) Dot |.|
//@[23:34) Identifier |displayName|
//@[34:39) StringMiddlePiece |} (${|
//@[39:50) Identifier |singleGroup|
//@[50:51) Dot |.|
//@[51:61) Identifier |properties|
//@[61:62) Dot |.|
//@[62:73) Identifier |displayName|
//@[73:84) StringRightPiece |}) (set 2)'|
//@[84:86) NewLine |\r\n|
  }
//@[2:3) RightBrace |}|
//@[3:5) NewLine |\r\n|
  dependsOn: [
//@[2:11) Identifier |dependsOn|
//@[11:12) Colon |:|
//@[13:14) LeftSquare |[|
//@[14:16) NewLine |\r\n|
    anotherSet[0]
//@[4:14) Identifier |anotherSet|
//@[14:15) LeftSquare |[|
//@[15:16) Integer |0|
//@[16:17) RightSquare |]|
//@[17:19) NewLine |\r\n|
  ]
//@[2:3) RightSquare |]|
//@[3:5) NewLine |\r\n|
}]
//@[0:1) RightBrace |}|
//@[1:2) RightSquare |]|
//@[2:6) NewLine |\r\n\r\n|

output managementGroupIds array = [for i in range(0, length(managementGroups)): {
//@[0:6) Identifier |output|
//@[7:25) Identifier |managementGroupIds|
//@[26:31) Identifier |array|
//@[32:33) Assignment |=|
//@[34:35) LeftSquare |[|
//@[35:38) Identifier |for|
//@[39:40) Identifier |i|
//@[41:43) Identifier |in|
//@[44:49) Identifier |range|
//@[49:50) LeftParen |(|
//@[50:51) Integer |0|
//@[51:52) Comma |,|
//@[53:59) Identifier |length|
//@[59:60) LeftParen |(|
//@[60:76) Identifier |managementGroups|
//@[76:77) RightParen |)|
//@[77:78) RightParen |)|
//@[78:79) Colon |:|
//@[80:81) LeftBrace |{|
//@[81:83) NewLine |\r\n|
  name: yetAnotherSet[i].name
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:21) Identifier |yetAnotherSet|
//@[21:22) LeftSquare |[|
//@[22:23) Identifier |i|
//@[23:24) RightSquare |]|
//@[24:25) Dot |.|
//@[25:29) Identifier |name|
//@[29:31) NewLine |\r\n|
  displayName: yetAnotherSet[i].properties.displayName
//@[2:13) Identifier |displayName|
//@[13:14) Colon |:|
//@[15:28) Identifier |yetAnotherSet|
//@[28:29) LeftSquare |[|
//@[29:30) Identifier |i|
//@[30:31) RightSquare |]|
//@[31:32) Dot |.|
//@[32:42) Identifier |properties|
//@[42:43) Dot |.|
//@[43:54) Identifier |displayName|
//@[54:56) NewLine |\r\n|
}]
//@[0:1) RightBrace |}|
//@[1:2) RightSquare |]|
//@[2:4) NewLine |\r\n|

//@[0:0) EndOfFile ||
