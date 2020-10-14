module moduleWithMissingPath './nonExistent.bicep' = {
//@[0:6) Identifier |module|
//@[7:28) Identifier |moduleWithMissingPath|
//@[29:50) StringComplete |'./nonExistent.bicep'|
//@[51:52) Assignment |=|
//@[53:54) LeftBrace |{|
//@[54:56) NewLine |\n\n|

}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

module moduleWithoutPath = {
//@[0:6) Identifier |module|
//@[7:24) Identifier |moduleWithoutPath|
//@[25:26) Assignment |=|
//@[27:28) LeftBrace |{|
//@[28:30) NewLine |\n\n|

}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

var interp = 'hello'
//@[0:3) Identifier |var|
//@[4:10) Identifier |interp|
//@[11:12) Assignment |=|
//@[13:20) StringComplete |'hello'|
//@[20:21) NewLine |\n|
module moduleWithInterpPath './${interp}.bicep' = {
//@[0:6) Identifier |module|
//@[7:27) Identifier |moduleWithInterpPath|
//@[28:33) StringLeftPiece |'./${|
//@[33:39) Identifier |interp|
//@[39:47) StringRightPiece |}.bicep'|
//@[48:49) Assignment |=|
//@[50:51) LeftBrace |{|
//@[51:53) NewLine |\n\n|

}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

module moduleWithSelfCycle './main.bicep' = {
//@[0:6) Identifier |module|
//@[7:26) Identifier |moduleWithSelfCycle|
//@[27:41) StringComplete |'./main.bicep'|
//@[42:43) Assignment |=|
//@[44:45) LeftBrace |{|
//@[45:47) NewLine |\n\n|

}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

module './main.bicep' = {
//@[0:6) Identifier |module|
//@[7:21) StringComplete |'./main.bicep'|
//@[22:23) Assignment |=|
//@[24:25) LeftBrace |{|
//@[25:27) NewLine |\n\n|

}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

module modANoInputs './modulea.bicep' = {
//@[0:6) Identifier |module|
//@[7:19) Identifier |modANoInputs|
//@[20:37) StringComplete |'./modulea.bicep'|
//@[38:39) Assignment |=|
//@[40:41) LeftBrace |{|
//@[41:42) NewLine |\n|
  
//@[2:3) NewLine |\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

module modCycle './cycle.bicep' = {
//@[0:6) Identifier |module|
//@[7:15) Identifier |modCycle|
//@[16:31) StringComplete |'./cycle.bicep'|
//@[32:33) Assignment |=|
//@[34:35) LeftBrace |{|
//@[35:36) NewLine |\n|
  
//@[2:3) NewLine |\n|
}
//@[0:1) RightBrace |}|
//@[1:1) EndOfFile ||
