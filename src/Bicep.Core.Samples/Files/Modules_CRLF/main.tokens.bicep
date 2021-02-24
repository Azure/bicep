param deployTimeSuffix string = newGuid()
//@[0:5) Identifier |param|
//@[6:22) Identifier |deployTimeSuffix|
//@[23:29) Identifier |string|
//@[30:31) Assignment |=|
//@[32:39) Identifier |newGuid|
//@[39:40) LeftParen |(|
//@[40:41) RightParen |)|
//@[41:45) NewLine |\r\n\r\n|

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

module modBWithCondition './child/moduleb.bicep' = if (1 + 1 == 2) {
//@[0:6) Identifier |module|
//@[7:24) Identifier |modBWithCondition|
//@[25:48) StringComplete |'./child/moduleb.bicep'|
//@[49:50) Assignment |=|
//@[51:53) Identifier |if|
//@[54:55) LeftParen |(|
//@[55:56) Integer |1|
//@[57:58) Plus |+|
//@[59:60) Integer |1|
//@[61:63) Equals |==|
//@[64:65) Integer |2|
//@[65:66) RightParen |)|
//@[67:68) LeftBrace |{|
//@[68:70) NewLine |\r\n|
  name: 'modBWithCondition'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:27) StringComplete |'modBWithCondition'|
//@[27:29) NewLine |\r\n|
  params: {
//@[2:8) Identifier |params|
//@[8:9) Colon |:|
//@[10:11) LeftBrace |{|
//@[11:13) NewLine |\r\n|
    location: 'East US'
//@[4:12) Identifier |location|
//@[12:13) Colon |:|
//@[14:23) StringComplete |'East US'|
//@[23:25) NewLine |\r\n|
  }
//@[2:3) RightBrace |}|
//@[3:5) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

module optionalWithNoParams1 './child/optionalParams.bicep'= {
//@[0:6) Identifier |module|
//@[7:28) Identifier |optionalWithNoParams1|
//@[29:59) StringComplete |'./child/optionalParams.bicep'|
//@[59:60) Assignment |=|
//@[61:62) LeftBrace |{|
//@[62:64) NewLine |\r\n|
  name: 'optionalWithNoParams1'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:31) StringComplete |'optionalWithNoParams1'|
//@[31:33) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

module optionalWithNoParams2 './child/optionalParams.bicep'= {
//@[0:6) Identifier |module|
//@[7:28) Identifier |optionalWithNoParams2|
//@[29:59) StringComplete |'./child/optionalParams.bicep'|
//@[59:60) Assignment |=|
//@[61:62) LeftBrace |{|
//@[62:64) NewLine |\r\n|
  name: 'optionalWithNoParams2'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:31) StringComplete |'optionalWithNoParams2'|
//@[31:33) NewLine |\r\n|
  params: {
//@[2:8) Identifier |params|
//@[8:9) Colon |:|
//@[10:11) LeftBrace |{|
//@[11:13) NewLine |\r\n|
  }
//@[2:3) RightBrace |}|
//@[3:5) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

module optionalWithAllParams './child/optionalParams.bicep'= {
//@[0:6) Identifier |module|
//@[7:28) Identifier |optionalWithAllParams|
//@[29:59) StringComplete |'./child/optionalParams.bicep'|
//@[59:60) Assignment |=|
//@[61:62) LeftBrace |{|
//@[62:64) NewLine |\r\n|
  name: 'optionalWithNoParams3'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:31) StringComplete |'optionalWithNoParams3'|
//@[31:33) NewLine |\r\n|
  params: {
//@[2:8) Identifier |params|
//@[8:9) Colon |:|
//@[10:11) LeftBrace |{|
//@[11:13) NewLine |\r\n|
    optionalString: 'abc'
//@[4:18) Identifier |optionalString|
//@[18:19) Colon |:|
//@[20:25) StringComplete |'abc'|
//@[25:27) NewLine |\r\n|
    optionalInt: 42
//@[4:15) Identifier |optionalInt|
//@[15:16) Colon |:|
//@[17:19) Integer |42|
//@[19:21) NewLine |\r\n|
    optionalObj: { }
//@[4:15) Identifier |optionalObj|
//@[15:16) Colon |:|
//@[17:18) LeftBrace |{|
//@[19:20) RightBrace |}|
//@[20:22) NewLine |\r\n|
    optionalArray: [ ]
//@[4:17) Identifier |optionalArray|
//@[17:18) Colon |:|
//@[19:20) LeftSquare |[|
//@[21:22) RightSquare |]|
//@[22:24) NewLine |\r\n|
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

module optionalWithAllParamsAndManualDependency './child/optionalParams.bicep'= {
//@[0:6) Identifier |module|
//@[7:47) Identifier |optionalWithAllParamsAndManualDependency|
//@[48:78) StringComplete |'./child/optionalParams.bicep'|
//@[78:79) Assignment |=|
//@[80:81) LeftBrace |{|
//@[81:83) NewLine |\r\n|
  name: 'optionalWithAllParamsAndManualDependency'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:50) StringComplete |'optionalWithAllParamsAndManualDependency'|
//@[50:52) NewLine |\r\n|
  params: {
//@[2:8) Identifier |params|
//@[8:9) Colon |:|
//@[10:11) LeftBrace |{|
//@[11:13) NewLine |\r\n|
    optionalString: 'abc'
//@[4:18) Identifier |optionalString|
//@[18:19) Colon |:|
//@[20:25) StringComplete |'abc'|
//@[25:27) NewLine |\r\n|
    optionalInt: 42
//@[4:15) Identifier |optionalInt|
//@[15:16) Colon |:|
//@[17:19) Integer |42|
//@[19:21) NewLine |\r\n|
    optionalObj: { }
//@[4:15) Identifier |optionalObj|
//@[15:16) Colon |:|
//@[17:18) LeftBrace |{|
//@[19:20) RightBrace |}|
//@[20:22) NewLine |\r\n|
    optionalArray: [ ]
//@[4:17) Identifier |optionalArray|
//@[17:18) Colon |:|
//@[19:20) LeftSquare |[|
//@[21:22) RightSquare |]|
//@[22:24) NewLine |\r\n|
  }
//@[2:3) RightBrace |}|
//@[3:5) NewLine |\r\n|
  dependsOn: [
//@[2:11) Identifier |dependsOn|
//@[11:12) Colon |:|
//@[13:14) LeftSquare |[|
//@[14:16) NewLine |\r\n|
    resWithDependencies
//@[4:23) Identifier |resWithDependencies|
//@[23:25) NewLine |\r\n|
    optionalWithAllParams
//@[4:25) Identifier |optionalWithAllParams|
//@[25:27) NewLine |\r\n|
  ]
//@[2:3) RightSquare |]|
//@[3:5) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

module optionalWithImplicitDependency './child/optionalParams.bicep'= {
//@[0:6) Identifier |module|
//@[7:37) Identifier |optionalWithImplicitDependency|
//@[38:68) StringComplete |'./child/optionalParams.bicep'|
//@[68:69) Assignment |=|
//@[70:71) LeftBrace |{|
//@[71:73) NewLine |\r\n|
  name: 'optionalWithImplicitDependency'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:40) StringComplete |'optionalWithImplicitDependency'|
//@[40:42) NewLine |\r\n|
  params: {
//@[2:8) Identifier |params|
//@[8:9) Colon |:|
//@[10:11) LeftBrace |{|
//@[11:13) NewLine |\r\n|
    optionalString: concat(resWithDependencies.id, optionalWithAllParamsAndManualDependency.name)
//@[4:18) Identifier |optionalString|
//@[18:19) Colon |:|
//@[20:26) Identifier |concat|
//@[26:27) LeftParen |(|
//@[27:46) Identifier |resWithDependencies|
//@[46:47) Dot |.|
//@[47:49) Identifier |id|
//@[49:50) Comma |,|
//@[51:91) Identifier |optionalWithAllParamsAndManualDependency|
//@[91:92) Dot |.|
//@[92:96) Identifier |name|
//@[96:97) RightParen |)|
//@[97:99) NewLine |\r\n|
    optionalInt: 42
//@[4:15) Identifier |optionalInt|
//@[15:16) Colon |:|
//@[17:19) Integer |42|
//@[19:21) NewLine |\r\n|
    optionalObj: { }
//@[4:15) Identifier |optionalObj|
//@[15:16) Colon |:|
//@[17:18) LeftBrace |{|
//@[19:20) RightBrace |}|
//@[20:22) NewLine |\r\n|
    optionalArray: [ ]
//@[4:17) Identifier |optionalArray|
//@[17:18) Colon |:|
//@[19:20) LeftSquare |[|
//@[21:22) RightSquare |]|
//@[22:24) NewLine |\r\n|
  }
//@[2:3) RightBrace |}|
//@[3:5) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

module moduleWithCalculatedName './child/optionalParams.bicep'= {
//@[0:6) Identifier |module|
//@[7:31) Identifier |moduleWithCalculatedName|
//@[32:62) StringComplete |'./child/optionalParams.bicep'|
//@[62:63) Assignment |=|
//@[64:65) LeftBrace |{|
//@[65:67) NewLine |\r\n|
  name: '${optionalWithAllParamsAndManualDependency.name}${deployTimeSuffix}'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:11) StringLeftPiece |'${|
//@[11:51) Identifier |optionalWithAllParamsAndManualDependency|
//@[51:52) Dot |.|
//@[52:56) Identifier |name|
//@[56:59) StringMiddlePiece |}${|
//@[59:75) Identifier |deployTimeSuffix|
//@[75:77) StringRightPiece |}'|
//@[77:79) NewLine |\r\n|
  params: {
//@[2:8) Identifier |params|
//@[8:9) Colon |:|
//@[10:11) LeftBrace |{|
//@[11:13) NewLine |\r\n|
    optionalString: concat(resWithDependencies.id, optionalWithAllParamsAndManualDependency.name)
//@[4:18) Identifier |optionalString|
//@[18:19) Colon |:|
//@[20:26) Identifier |concat|
//@[26:27) LeftParen |(|
//@[27:46) Identifier |resWithDependencies|
//@[46:47) Dot |.|
//@[47:49) Identifier |id|
//@[49:50) Comma |,|
//@[51:91) Identifier |optionalWithAllParamsAndManualDependency|
//@[91:92) Dot |.|
//@[92:96) Identifier |name|
//@[96:97) RightParen |)|
//@[97:99) NewLine |\r\n|
    optionalInt: 42
//@[4:15) Identifier |optionalInt|
//@[15:16) Colon |:|
//@[17:19) Integer |42|
//@[19:21) NewLine |\r\n|
    optionalObj: { }
//@[4:15) Identifier |optionalObj|
//@[15:16) Colon |:|
//@[17:18) LeftBrace |{|
//@[19:20) RightBrace |}|
//@[20:22) NewLine |\r\n|
    optionalArray: [ ]
//@[4:17) Identifier |optionalArray|
//@[17:18) Colon |:|
//@[19:20) LeftSquare |[|
//@[21:22) RightSquare |]|
//@[22:24) NewLine |\r\n|
  }
//@[2:3) RightBrace |}|
//@[3:5) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource resWithCalculatedNameDependencies 'Mock.Rp/mockResource@2020-01-01' = {
//@[0:8) Identifier |resource|
//@[9:42) Identifier |resWithCalculatedNameDependencies|
//@[43:76) StringComplete |'Mock.Rp/mockResource@2020-01-01'|
//@[77:78) Assignment |=|
//@[79:80) LeftBrace |{|
//@[80:82) NewLine |\r\n|
  name: '${optionalWithAllParamsAndManualDependency.name}${deployTimeSuffix}'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:11) StringLeftPiece |'${|
//@[11:51) Identifier |optionalWithAllParamsAndManualDependency|
//@[51:52) Dot |.|
//@[52:56) Identifier |name|
//@[56:59) StringMiddlePiece |}${|
//@[59:75) Identifier |deployTimeSuffix|
//@[75:77) StringRightPiece |}'|
//@[77:79) NewLine |\r\n|
  properties: {
//@[2:12) Identifier |properties|
//@[12:13) Colon |:|
//@[14:15) LeftBrace |{|
//@[15:17) NewLine |\r\n|
    modADep: moduleWithCalculatedName.outputs.outputObj
//@[4:11) Identifier |modADep|
//@[11:12) Colon |:|
//@[13:37) Identifier |moduleWithCalculatedName|
//@[37:38) Dot |.|
//@[38:45) Identifier |outputs|
//@[45:46) Dot |.|
//@[46:55) Identifier |outputObj|
//@[55:57) NewLine |\r\n|
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
//@[55:57) NewLine |\r\n|
output modCalculatedNameOutput object = moduleWithCalculatedName.outputs.outputObj
//@[0:6) Identifier |output|
//@[7:30) Identifier |modCalculatedNameOutput|
//@[31:37) Identifier |object|
//@[38:39) Assignment |=|
//@[40:64) Identifier |moduleWithCalculatedName|
//@[64:65) Dot |.|
//@[65:72) Identifier |outputs|
//@[72:73) Dot |.|
//@[73:82) Identifier |outputObj|
//@[82:86) NewLine |\r\n\r\n|

/*
  valid loop cases
*/ 
//@[3:5) NewLine |\r\n|
var myModules = [
//@[0:3) Identifier |var|
//@[4:13) Identifier |myModules|
//@[14:15) Assignment |=|
//@[16:17) LeftSquare |[|
//@[17:19) NewLine |\r\n|
  {
//@[2:3) LeftBrace |{|
//@[3:5) NewLine |\r\n|
    name: 'one'
//@[4:8) Identifier |name|
//@[8:9) Colon |:|
//@[10:15) StringComplete |'one'|
//@[15:17) NewLine |\r\n|
    location: 'eastus2'
//@[4:12) Identifier |location|
//@[12:13) Colon |:|
//@[14:23) StringComplete |'eastus2'|
//@[23:25) NewLine |\r\n|
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
    location: 'westus'
//@[4:12) Identifier |location|
//@[12:13) Colon |:|
//@[14:22) StringComplete |'westus'|
//@[22:24) NewLine |\r\n|
  }
//@[2:3) RightBrace |}|
//@[3:5) NewLine |\r\n|
]
//@[0:1) RightSquare |]|
//@[1:5) NewLine |\r\n\r\n|

var emptyArray = []
//@[0:3) Identifier |var|
//@[4:14) Identifier |emptyArray|
//@[15:16) Assignment |=|
//@[17:18) LeftSquare |[|
//@[18:19) RightSquare |]|
//@[19:23) NewLine |\r\n\r\n|

// simple module loop
//@[21:23) NewLine |\r\n|
module storageResources 'modulea.bicep' = [for module in myModules: {
//@[0:6) Identifier |module|
//@[7:23) Identifier |storageResources|
//@[24:39) StringComplete |'modulea.bicep'|
//@[40:41) Assignment |=|
//@[42:43) LeftSquare |[|
//@[43:46) Identifier |for|
//@[47:53) Identifier |module|
//@[54:56) Identifier |in|
//@[57:66) Identifier |myModules|
//@[66:67) Colon |:|
//@[68:69) LeftBrace |{|
//@[69:71) NewLine |\r\n|
  name: module.name
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:14) Identifier |module|
//@[14:15) Dot |.|
//@[15:19) Identifier |name|
//@[19:21) NewLine |\r\n|
  params: {
//@[2:8) Identifier |params|
//@[8:9) Colon |:|
//@[10:11) LeftBrace |{|
//@[11:13) NewLine |\r\n|
    arrayParam: []
//@[4:14) Identifier |arrayParam|
//@[14:15) Colon |:|
//@[16:17) LeftSquare |[|
//@[17:18) RightSquare |]|
//@[18:20) NewLine |\r\n|
    objParam: module
//@[4:12) Identifier |objParam|
//@[12:13) Colon |:|
//@[14:20) Identifier |module|
//@[20:22) NewLine |\r\n|
    stringParamB: module.location
//@[4:16) Identifier |stringParamB|
//@[16:17) Colon |:|
//@[18:24) Identifier |module|
//@[24:25) Dot |.|
//@[25:33) Identifier |location|
//@[33:35) NewLine |\r\n|
  }
//@[2:3) RightBrace |}|
//@[3:5) NewLine |\r\n|
}]
//@[0:1) RightBrace |}|
//@[1:2) RightSquare |]|
//@[2:6) NewLine |\r\n\r\n|

// nested module loop
//@[21:23) NewLine |\r\n|
module nestedModuleLoop 'modulea.bicep' = [for module in myModules: {
//@[0:6) Identifier |module|
//@[7:23) Identifier |nestedModuleLoop|
//@[24:39) StringComplete |'modulea.bicep'|
//@[40:41) Assignment |=|
//@[42:43) LeftSquare |[|
//@[43:46) Identifier |for|
//@[47:53) Identifier |module|
//@[54:56) Identifier |in|
//@[57:66) Identifier |myModules|
//@[66:67) Colon |:|
//@[68:69) LeftBrace |{|
//@[69:71) NewLine |\r\n|
  name: module.name
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:14) Identifier |module|
//@[14:15) Dot |.|
//@[15:19) Identifier |name|
//@[19:21) NewLine |\r\n|
  params: {
//@[2:8) Identifier |params|
//@[8:9) Colon |:|
//@[10:11) LeftBrace |{|
//@[11:13) NewLine |\r\n|
    arrayParam: [for i in range(0,3): concat('test-', i, '-', module.name)]
//@[4:14) Identifier |arrayParam|
//@[14:15) Colon |:|
//@[16:17) LeftSquare |[|
//@[17:20) Identifier |for|
//@[21:22) Identifier |i|
//@[23:25) Identifier |in|
//@[26:31) Identifier |range|
//@[31:32) LeftParen |(|
//@[32:33) Integer |0|
//@[33:34) Comma |,|
//@[34:35) Integer |3|
//@[35:36) RightParen |)|
//@[36:37) Colon |:|
//@[38:44) Identifier |concat|
//@[44:45) LeftParen |(|
//@[45:52) StringComplete |'test-'|
//@[52:53) Comma |,|
//@[54:55) Identifier |i|
//@[55:56) Comma |,|
//@[57:60) StringComplete |'-'|
//@[60:61) Comma |,|
//@[62:68) Identifier |module|
//@[68:69) Dot |.|
//@[69:73) Identifier |name|
//@[73:74) RightParen |)|
//@[74:75) RightSquare |]|
//@[75:77) NewLine |\r\n|
    objParam: module
//@[4:12) Identifier |objParam|
//@[12:13) Colon |:|
//@[14:20) Identifier |module|
//@[20:22) NewLine |\r\n|
    stringParamB: module.location
//@[4:16) Identifier |stringParamB|
//@[16:17) Colon |:|
//@[18:24) Identifier |module|
//@[24:25) Dot |.|
//@[25:33) Identifier |location|
//@[33:35) NewLine |\r\n|
  }
//@[2:3) RightBrace |}|
//@[3:5) NewLine |\r\n|
}]
//@[0:1) RightBrace |}|
//@[1:2) RightSquare |]|
//@[2:6) NewLine |\r\n\r\n|

// duplicate identifiers across scopes are allowed (inner hides the outer)
//@[74:76) NewLine |\r\n|
module duplicateIdentifiersWithinLoop 'modulea.bicep' = [for x in emptyArray:{
//@[0:6) Identifier |module|
//@[7:37) Identifier |duplicateIdentifiersWithinLoop|
//@[38:53) StringComplete |'modulea.bicep'|
//@[54:55) Assignment |=|
//@[56:57) LeftSquare |[|
//@[57:60) Identifier |for|
//@[61:62) Identifier |x|
//@[63:65) Identifier |in|
//@[66:76) Identifier |emptyArray|
//@[76:77) Colon |:|
//@[77:78) LeftBrace |{|
//@[78:80) NewLine |\r\n|
  name: 'hello-${x}'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:17) StringLeftPiece |'hello-${|
//@[17:18) Identifier |x|
//@[18:20) StringRightPiece |}'|
//@[20:22) NewLine |\r\n|
  params: {
//@[2:8) Identifier |params|
//@[8:9) Colon |:|
//@[10:11) LeftBrace |{|
//@[11:13) NewLine |\r\n|
    objParam: {}
//@[4:12) Identifier |objParam|
//@[12:13) Colon |:|
//@[14:15) LeftBrace |{|
//@[15:16) RightBrace |}|
//@[16:18) NewLine |\r\n|
    stringParamA: 'test'
//@[4:16) Identifier |stringParamA|
//@[16:17) Colon |:|
//@[18:24) StringComplete |'test'|
//@[24:26) NewLine |\r\n|
    stringParamB: 'test'
//@[4:16) Identifier |stringParamB|
//@[16:17) Colon |:|
//@[18:24) StringComplete |'test'|
//@[24:26) NewLine |\r\n|
    arrayParam: [for x in emptyArray: x]
//@[4:14) Identifier |arrayParam|
//@[14:15) Colon |:|
//@[16:17) LeftSquare |[|
//@[17:20) Identifier |for|
//@[21:22) Identifier |x|
//@[23:25) Identifier |in|
//@[26:36) Identifier |emptyArray|
//@[36:37) Colon |:|
//@[38:39) Identifier |x|
//@[39:40) RightSquare |]|
//@[40:42) NewLine |\r\n|
  }
//@[2:3) RightBrace |}|
//@[3:5) NewLine |\r\n|
}]
//@[0:1) RightBrace |}|
//@[1:2) RightSquare |]|
//@[2:6) NewLine |\r\n\r\n|

// duplicate identifiers across scopes are allowed (inner hides the outer)
//@[74:76) NewLine |\r\n|
var duplicateAcrossScopes = 'hello'
//@[0:3) Identifier |var|
//@[4:25) Identifier |duplicateAcrossScopes|
//@[26:27) Assignment |=|
//@[28:35) StringComplete |'hello'|
//@[35:37) NewLine |\r\n|
module duplicateInGlobalAndOneLoop 'modulea.bicep' = [for duplicateAcrossScopes in []: {
//@[0:6) Identifier |module|
//@[7:34) Identifier |duplicateInGlobalAndOneLoop|
//@[35:50) StringComplete |'modulea.bicep'|
//@[51:52) Assignment |=|
//@[53:54) LeftSquare |[|
//@[54:57) Identifier |for|
//@[58:79) Identifier |duplicateAcrossScopes|
//@[80:82) Identifier |in|
//@[83:84) LeftSquare |[|
//@[84:85) RightSquare |]|
//@[85:86) Colon |:|
//@[87:88) LeftBrace |{|
//@[88:90) NewLine |\r\n|
  name: 'hello-${duplicateAcrossScopes}'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:17) StringLeftPiece |'hello-${|
//@[17:38) Identifier |duplicateAcrossScopes|
//@[38:40) StringRightPiece |}'|
//@[40:42) NewLine |\r\n|
  params: {
//@[2:8) Identifier |params|
//@[8:9) Colon |:|
//@[10:11) LeftBrace |{|
//@[11:13) NewLine |\r\n|
    objParam: {}
//@[4:12) Identifier |objParam|
//@[12:13) Colon |:|
//@[14:15) LeftBrace |{|
//@[15:16) RightBrace |}|
//@[16:18) NewLine |\r\n|
    stringParamA: 'test'
//@[4:16) Identifier |stringParamA|
//@[16:17) Colon |:|
//@[18:24) StringComplete |'test'|
//@[24:26) NewLine |\r\n|
    stringParamB: 'test'
//@[4:16) Identifier |stringParamB|
//@[16:17) Colon |:|
//@[18:24) StringComplete |'test'|
//@[24:26) NewLine |\r\n|
    arrayParam: [for x in emptyArray: x]
//@[4:14) Identifier |arrayParam|
//@[14:15) Colon |:|
//@[16:17) LeftSquare |[|
//@[17:20) Identifier |for|
//@[21:22) Identifier |x|
//@[23:25) Identifier |in|
//@[26:36) Identifier |emptyArray|
//@[36:37) Colon |:|
//@[38:39) Identifier |x|
//@[39:40) RightSquare |]|
//@[40:42) NewLine |\r\n|
  }
//@[2:3) RightBrace |}|
//@[3:5) NewLine |\r\n|
}]
//@[0:1) RightBrace |}|
//@[1:2) RightSquare |]|
//@[2:6) NewLine |\r\n\r\n|

var someDuplicate = true
//@[0:3) Identifier |var|
//@[4:17) Identifier |someDuplicate|
//@[18:19) Assignment |=|
//@[20:24) TrueKeyword |true|
//@[24:26) NewLine |\r\n|
var otherDuplicate = false
//@[0:3) Identifier |var|
//@[4:18) Identifier |otherDuplicate|
//@[19:20) Assignment |=|
//@[21:26) FalseKeyword |false|
//@[26:28) NewLine |\r\n|
module duplicatesEverywhere 'modulea.bicep' = [for someDuplicate in []: {
//@[0:6) Identifier |module|
//@[7:27) Identifier |duplicatesEverywhere|
//@[28:43) StringComplete |'modulea.bicep'|
//@[44:45) Assignment |=|
//@[46:47) LeftSquare |[|
//@[47:50) Identifier |for|
//@[51:64) Identifier |someDuplicate|
//@[65:67) Identifier |in|
//@[68:69) LeftSquare |[|
//@[69:70) RightSquare |]|
//@[70:71) Colon |:|
//@[72:73) LeftBrace |{|
//@[73:75) NewLine |\r\n|
  name: 'hello-${someDuplicate}'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:17) StringLeftPiece |'hello-${|
//@[17:30) Identifier |someDuplicate|
//@[30:32) StringRightPiece |}'|
//@[32:34) NewLine |\r\n|
  params: {
//@[2:8) Identifier |params|
//@[8:9) Colon |:|
//@[10:11) LeftBrace |{|
//@[11:13) NewLine |\r\n|
    objParam: {}
//@[4:12) Identifier |objParam|
//@[12:13) Colon |:|
//@[14:15) LeftBrace |{|
//@[15:16) RightBrace |}|
//@[16:18) NewLine |\r\n|
    stringParamB: 'test'
//@[4:16) Identifier |stringParamB|
//@[16:17) Colon |:|
//@[18:24) StringComplete |'test'|
//@[24:26) NewLine |\r\n|
    arrayParam: [for otherDuplicate in emptyArray: '${someDuplicate}-${otherDuplicate}']
//@[4:14) Identifier |arrayParam|
//@[14:15) Colon |:|
//@[16:17) LeftSquare |[|
//@[17:20) Identifier |for|
//@[21:35) Identifier |otherDuplicate|
//@[36:38) Identifier |in|
//@[39:49) Identifier |emptyArray|
//@[49:50) Colon |:|
//@[51:54) StringLeftPiece |'${|
//@[54:67) Identifier |someDuplicate|
//@[67:71) StringMiddlePiece |}-${|
//@[71:85) Identifier |otherDuplicate|
//@[85:87) StringRightPiece |}'|
//@[87:88) RightSquare |]|
//@[88:90) NewLine |\r\n|
  }
//@[2:3) RightBrace |}|
//@[3:5) NewLine |\r\n|
}]
//@[0:1) RightBrace |}|
//@[1:2) RightSquare |]|
//@[2:4) NewLine |\r\n|

//@[0:0) EndOfFile ||
