module modATest './modulea.bicep' = {
//@[0:6) Identifier |module|
//@[7:15) Identifier |modATest|
//@[16:33) StringComplete |'./modulea.bicep'|
//@[34:35) Assignment |=|
//@[36:37) LeftBrace |{|
//@[37:39) NewLine |\r\n|
  stringParamB: 'hello!'
//@[2:14) Identifier |stringParamB|
//@[14:15) Colon |:|
//@[16:24) StringComplete |'hello!'|
//@[24:26) NewLine |\r\n|
  objParam: {
//@[2:10) Identifier |objParam|
//@[10:11) Colon |:|
//@[12:13) LeftBrace |{|
//@[13:15) NewLine |\r\n|
    a: 'b'
//@[4:5) Identifier |a|
//@[5:6) Colon |:|
//@[7:10) StringComplete |'b'|
//@[10:12) NewLine |\r\n|
  }
//@[2:3) RightBrace |}|
//@[3:5) NewLine |\r\n|
  arrayParam: [
//@[2:12) Identifier |arrayParam|
//@[12:13) Colon |:|
//@[14:15) LeftSquare |[|
//@[15:17) NewLine |\r\n|
    {
//@[4:5) LeftBrace |{|
//@[5:7) NewLine |\r\n|
      a: 'b'
//@[6:7) Identifier |a|
//@[7:8) Colon |:|
//@[9:12) StringComplete |'b'|
//@[12:14) NewLine |\r\n|
    }
//@[4:5) RightBrace |}|
//@[5:7) NewLine |\r\n|
    'abc'
//@[4:9) StringComplete |'abc'|
//@[9:11) NewLine |\r\n|
  ]
//@[2:3) RightSquare |]|
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
  location: 'West US'
//@[2:10) Identifier |location|
//@[10:11) Colon |:|
//@[12:21) StringComplete |'West US'|
//@[21:23) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:1) EndOfFile ||
