module modATest './modulea.bicep' = {
//@[0:6) Identifier |module|
//@[7:15) Identifier |modATest|
//@[16:33) StringComplete |'./modulea.bicep'|
//@[34:35) Assignment |=|
//@[36:37) LeftBrace |{|
//@[37:39) NewLine |\r\n|
  name: 'modATest'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:18) StringComplete |'modATest'|
//@[18:20) NewLine |\r\n|
  params: {
//@[2:8) Identifier |params|
//@[8:9) Colon |:|
//@[10:11) LeftBrace |{|
//@[11:13) NewLine |\r\n|
    stringParamB: 'hello!'
//@[4:16) Identifier |stringParamB|
//@[16:17) Colon |:|
//@[18:26) StringComplete |'hello!'|
//@[26:28) NewLine |\r\n|
    objParam: {
//@[4:12) Identifier |objParam|
//@[12:13) Colon |:|
//@[14:15) LeftBrace |{|
//@[15:17) NewLine |\r\n|
      a: 'b'
//@[6:7) Identifier |a|
//@[7:8) Colon |:|
//@[9:12) StringComplete |'b'|
//@[12:14) NewLine |\r\n|
    }
//@[4:5) RightBrace |}|
//@[5:7) NewLine |\r\n|
    arrayParam: [
//@[4:14) Identifier |arrayParam|
//@[14:15) Colon |:|
//@[16:17) LeftSquare |[|
//@[17:19) NewLine |\r\n|
      {
//@[6:7) LeftBrace |{|
//@[7:9) NewLine |\r\n|
        a: 'b'
//@[8:9) Identifier |a|
//@[9:10) Colon |:|
//@[11:14) StringComplete |'b'|
//@[14:16) NewLine |\r\n|
      }
//@[6:7) RightBrace |}|
//@[7:9) NewLine |\r\n|
      'abc'
//@[6:11) StringComplete |'abc'|
//@[11:13) NewLine |\r\n|
    ]
//@[4:5) RightSquare |]|
//@[5:7) NewLine |\r\n|
  }
//@[2:3) RightBrace |}|
//@[3:5) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

module modB './child/moduleb.bicep' = {
//@[0:6) Identifier |module|
//@[7:11) Identifier |modB|
//@[12:35) StringComplete |'./child/moduleb.bicep'|
//@[36:37) Assignment |=|
//@[38:39) LeftBrace |{|
//@[39:41) NewLine |\r\n|
  name: 'modB'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:14) StringComplete |'modB'|
//@[14:16) NewLine |\r\n|
  params: {
//@[2:8) Identifier |params|
//@[8:9) Colon |:|
//@[10:11) LeftBrace |{|
//@[11:13) NewLine |\r\n|
    location: 'West US'
//@[4:12) Identifier |location|
//@[12:13) Colon |:|
//@[14:23) StringComplete |'West US'|
//@[23:25) NewLine |\r\n|
  }
//@[2:3) RightBrace |}|
//@[3:5) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource resWithDependencies 'Mock.Rp/mockResource@2020-01-01' = {
//@[0:8) Identifier |resource|
//@[9:28) Identifier |resWithDependencies|
//@[29:62) StringComplete |'Mock.Rp/mockResource@2020-01-01'|
//@[63:64) Assignment |=|
//@[65:66) LeftBrace |{|
//@[66:68) NewLine |\r\n|
  name: 'harry'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:15) StringComplete |'harry'|
//@[15:17) NewLine |\r\n|
  properties: {
//@[2:12) Identifier |properties|
//@[12:13) Colon |:|
//@[14:15) LeftBrace |{|
//@[15:17) NewLine |\r\n|
    modADep: modATest.outputs.stringOutputA
//@[4:11) Identifier |modADep|
//@[11:12) Colon |:|
//@[13:21) Identifier |modATest|
//@[21:22) Dot |.|
//@[22:29) Identifier |outputs|
//@[29:30) Dot |.|
//@[30:43) Identifier |stringOutputA|
//@[43:45) NewLine |\r\n|
    modBDep: modB.outputs.myResourceId
//@[4:11) Identifier |modBDep|
//@[11:12) Colon |:|
//@[13:17) Identifier |modB|
//@[17:18) Dot |.|
//@[18:25) Identifier |outputs|
//@[25:26) Dot |.|
//@[26:38) Identifier |myResourceId|
//@[38:40) NewLine |\r\n|
  }
//@[2:3) RightBrace |}|
//@[3:5) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

output stringOutputA string = modATest.outputs.stringOutputA
//@[0:6) Identifier |output|
//@[7:20) Identifier |stringOutputA|
//@[21:27) Identifier |string|
//@[28:29) Assignment |=|
//@[30:38) Identifier |modATest|
//@[38:39) Dot |.|
//@[39:46) Identifier |outputs|
//@[46:47) Dot |.|
//@[47:60) Identifier |stringOutputA|
//@[60:62) NewLine |\r\n|
output stringOutputB string = modATest.outputs.stringOutputB
//@[0:6) Identifier |output|
//@[7:20) Identifier |stringOutputB|
//@[21:27) Identifier |string|
//@[28:29) Assignment |=|
//@[30:38) Identifier |modATest|
//@[38:39) Dot |.|
//@[39:46) Identifier |outputs|
//@[46:47) Dot |.|
//@[47:60) Identifier |stringOutputB|
//@[60:62) NewLine |\r\n|
output objOutput object = modATest.outputs.objOutput
//@[0:6) Identifier |output|
//@[7:16) Identifier |objOutput|
//@[17:23) Identifier |object|
//@[24:25) Assignment |=|
//@[26:34) Identifier |modATest|
//@[34:35) Dot |.|
//@[35:42) Identifier |outputs|
//@[42:43) Dot |.|
//@[43:52) Identifier |objOutput|
//@[52:54) NewLine |\r\n|
output arrayOutput array = modATest.outputs.arrayOutput
//@[0:6) Identifier |output|
//@[7:18) Identifier |arrayOutput|
//@[19:24) Identifier |array|
//@[25:26) Assignment |=|
//@[27:35) Identifier |modATest|
//@[35:36) Dot |.|
//@[36:43) Identifier |outputs|
//@[43:44) Dot |.|
//@[44:55) Identifier |arrayOutput|
//@[55:55) EndOfFile ||
