var boolVal = true
//@[0:3) Identifier |var|
//@[4:11) Identifier |boolVal|
//@[12:13) Assignment |=|
//@[14:18) TrueKeyword |true|
//@[18:20) NewLine |\n\n|

var vmProperties = {
//@[0:3) Identifier |var|
//@[4:16) Identifier |vmProperties|
//@[17:18) Assignment |=|
//@[19:20) LeftBrace |{|
//@[20:21) NewLine |\n|
  diagnosticsProfile: {
//@[2:20) Identifier |diagnosticsProfile|
//@[20:21) Colon |:|
//@[22:23) LeftBrace |{|
//@[23:24) NewLine |\n|
    bootDiagnostics: {
//@[4:19) Identifier |bootDiagnostics|
//@[19:20) Colon |:|
//@[21:22) LeftBrace |{|
//@[22:23) NewLine |\n|
      enabled: 123
//@[6:13) Identifier |enabled|
//@[13:14) Colon |:|
//@[15:18) Integer |123|
//@[18:19) NewLine |\n|
      storageUri: true
//@[6:16) Identifier |storageUri|
//@[16:17) Colon |:|
//@[18:22) TrueKeyword |true|
//@[22:23) NewLine |\n|
      unknownProp: 'asdf'
//@[6:17) Identifier |unknownProp|
//@[17:18) Colon |:|
//@[19:25) StringComplete |'asdf'|
//@[25:26) NewLine |\n|
    }
//@[4:5) RightBrace |}|
//@[5:6) NewLine |\n|
  }
//@[2:3) RightBrace |}|
//@[3:4) NewLine |\n|
  evictionPolicy: boolVal
//@[2:16) Identifier |evictionPolicy|
//@[16:17) Colon |:|
//@[18:25) Identifier |boolVal|
//@[25:26) NewLine |\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

resource vm 'Microsoft.Compute/virtualMachines@2020-12-01' = {
//@[0:8) Identifier |resource|
//@[9:11) Identifier |vm|
//@[12:58) StringComplete |'Microsoft.Compute/virtualMachines@2020-12-01'|
//@[59:60) Assignment |=|
//@[61:62) LeftBrace |{|
//@[62:63) NewLine |\n|
  name: 'vm'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:12) StringComplete |'vm'|
//@[12:13) NewLine |\n|
  location: 'West US'
//@[2:10) Identifier |location|
//@[10:11) Colon |:|
//@[12:21) StringComplete |'West US'|
//@[21:22) NewLine |\n|
  properties: vmProperties
//@[2:12) Identifier |properties|
//@[12:13) Colon |:|
//@[14:26) Identifier |vmProperties|
//@[26:27) NewLine |\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

var ipConfigurations = [for i in range(0, 2): {
//@[0:3) Identifier |var|
//@[4:20) Identifier |ipConfigurations|
//@[21:22) Assignment |=|
//@[23:24) LeftSquare |[|
//@[24:27) Identifier |for|
//@[28:29) Identifier |i|
//@[30:32) Identifier |in|
//@[33:38) Identifier |range|
//@[38:39) LeftParen |(|
//@[39:40) Integer |0|
//@[40:41) Comma |,|
//@[42:43) Integer |2|
//@[43:44) RightParen |)|
//@[44:45) Colon |:|
//@[46:47) LeftBrace |{|
//@[47:48) NewLine |\n|
  id: true
//@[2:4) Identifier |id|
//@[4:5) Colon |:|
//@[6:10) TrueKeyword |true|
//@[10:11) NewLine |\n|
  name: 'asdf${i}'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:15) StringLeftPiece |'asdf${|
//@[15:16) Identifier |i|
//@[16:18) StringRightPiece |}'|
//@[18:19) NewLine |\n|
  properties: {
//@[2:12) Identifier |properties|
//@[12:13) Colon |:|
//@[14:15) LeftBrace |{|
//@[15:16) NewLine |\n|
    madeUpProperty: boolVal
//@[4:18) Identifier |madeUpProperty|
//@[18:19) Colon |:|
//@[20:27) Identifier |boolVal|
//@[27:28) NewLine |\n|
    subnet: 'hello'
//@[4:10) Identifier |subnet|
//@[10:11) Colon |:|
//@[12:19) StringComplete |'hello'|
//@[19:20) NewLine |\n|
  }
//@[2:3) RightBrace |}|
//@[3:4) NewLine |\n|
}]
//@[0:1) RightBrace |}|
//@[1:2) RightSquare |]|
//@[2:4) NewLine |\n\n|

resource nic 'Microsoft.Network/networkInterfaces@2020-11-01' = {
//@[0:8) Identifier |resource|
//@[9:12) Identifier |nic|
//@[13:61) StringComplete |'Microsoft.Network/networkInterfaces@2020-11-01'|
//@[62:63) Assignment |=|
//@[64:65) LeftBrace |{|
//@[65:66) NewLine |\n|
  name: 'abc'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:13) StringComplete |'abc'|
//@[13:14) NewLine |\n|
  properties: {
//@[2:12) Identifier |properties|
//@[12:13) Colon |:|
//@[14:15) LeftBrace |{|
//@[15:16) NewLine |\n|
    ipConfigurations: ipConfigurations
//@[4:20) Identifier |ipConfigurations|
//@[20:21) Colon |:|
//@[22:38) Identifier |ipConfigurations|
//@[38:39) NewLine |\n|
  }
//@[2:3) RightBrace |}|
//@[3:4) NewLine |\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

resource nicLoop 'Microsoft.Network/networkInterfaces@2020-11-01' = [for i in range(0, 2): {
//@[0:8) Identifier |resource|
//@[9:16) Identifier |nicLoop|
//@[17:65) StringComplete |'Microsoft.Network/networkInterfaces@2020-11-01'|
//@[66:67) Assignment |=|
//@[68:69) LeftSquare |[|
//@[69:72) Identifier |for|
//@[73:74) Identifier |i|
//@[75:77) Identifier |in|
//@[78:83) Identifier |range|
//@[83:84) LeftParen |(|
//@[84:85) Integer |0|
//@[85:86) Comma |,|
//@[87:88) Integer |2|
//@[88:89) RightParen |)|
//@[89:90) Colon |:|
//@[91:92) LeftBrace |{|
//@[92:93) NewLine |\n|
  name: 'abc${i}'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:14) StringLeftPiece |'abc${|
//@[14:15) Identifier |i|
//@[15:17) StringRightPiece |}'|
//@[17:18) NewLine |\n|
  properties: {
//@[2:12) Identifier |properties|
//@[12:13) Colon |:|
//@[14:15) LeftBrace |{|
//@[15:16) NewLine |\n|
    ipConfigurations: [
//@[4:20) Identifier |ipConfigurations|
//@[20:21) Colon |:|
//@[22:23) LeftSquare |[|
//@[23:24) NewLine |\n|
      // TODO: fix this
//@[23:24) NewLine |\n|
      ipConfigurations[i]
//@[6:22) Identifier |ipConfigurations|
//@[22:23) LeftSquare |[|
//@[23:24) Identifier |i|
//@[24:25) RightSquare |]|
//@[25:26) NewLine |\n|
    ]
//@[4:5) RightSquare |]|
//@[5:6) NewLine |\n|
  }
//@[2:3) RightBrace |}|
//@[3:4) NewLine |\n|
}]
//@[0:1) RightBrace |}|
//@[1:2) RightSquare |]|
//@[2:4) NewLine |\n\n|

resource nicLoop2 'Microsoft.Network/networkInterfaces@2020-11-01' = [for ipConfig in ipConfigurations: {
//@[0:8) Identifier |resource|
//@[9:17) Identifier |nicLoop2|
//@[18:66) StringComplete |'Microsoft.Network/networkInterfaces@2020-11-01'|
//@[67:68) Assignment |=|
//@[69:70) LeftSquare |[|
//@[70:73) Identifier |for|
//@[74:82) Identifier |ipConfig|
//@[83:85) Identifier |in|
//@[86:102) Identifier |ipConfigurations|
//@[102:103) Colon |:|
//@[104:105) LeftBrace |{|
//@[105:106) NewLine |\n|
  name: 'abc${ipConfig.name}'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:14) StringLeftPiece |'abc${|
//@[14:22) Identifier |ipConfig|
//@[22:23) Dot |.|
//@[23:27) Identifier |name|
//@[27:29) StringRightPiece |}'|
//@[29:30) NewLine |\n|
  properties: {
//@[2:12) Identifier |properties|
//@[12:13) Colon |:|
//@[14:15) LeftBrace |{|
//@[15:16) NewLine |\n|
    ipConfigurations: [
//@[4:20) Identifier |ipConfigurations|
//@[20:21) Colon |:|
//@[22:23) LeftSquare |[|
//@[23:24) NewLine |\n|
      // TODO: fix this
//@[23:24) NewLine |\n|
      ipConfig
//@[6:14) Identifier |ipConfig|
//@[14:15) NewLine |\n|
    ]
//@[4:5) RightSquare |]|
//@[5:6) NewLine |\n|
  }
//@[2:3) RightBrace |}|
//@[3:4) NewLine |\n|
}]
//@[0:1) RightBrace |}|
//@[1:2) RightSquare |]|
//@[2:3) NewLine |\n|

//@[0:0) EndOfFile ||
