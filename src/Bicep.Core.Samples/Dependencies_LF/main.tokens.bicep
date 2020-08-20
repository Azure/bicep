param deployTimeParam string = 'steve'
//@[0:5] Identifier |param|
//@[6:21] Identifier |deployTimeParam|
//@[22:28] Identifier |string|
//@[29:30] Assignment |=|
//@[31:38] StringComplete |'steve'|
//@[38:39] NewLine |\n|
var deployTimeVar = 'nigel'
//@[0:3] Identifier |var|
//@[4:17] Identifier |deployTimeVar|
//@[18:19] Assignment |=|
//@[20:27] StringComplete |'nigel'|
//@[27:28] NewLine |\n|
var dependentVar = {
//@[0:3] Identifier |var|
//@[4:16] Identifier |dependentVar|
//@[17:18] Assignment |=|
//@[19:20] LeftBrace |{|
//@[20:21] NewLine |\n|
  dependencies: [
//@[2:14] Identifier |dependencies|
//@[14:15] Colon |:|
//@[16:17] LeftSquare |[|
//@[17:18] NewLine |\n|
    deployTimeVar
//@[4:17] Identifier |deployTimeVar|
//@[17:18] NewLine |\n|
    deployTimeParam
//@[4:19] Identifier |deployTimeParam|
//@[19:20] NewLine |\n|
  ]
//@[2:3] RightSquare |]|
//@[3:4] NewLine |\n|
}
//@[0:1] RightBrace |}|
//@[1:3] NewLine |\n\n|

var resourceDependency = {
//@[0:3] Identifier |var|
//@[4:22] Identifier |resourceDependency|
//@[23:24] Assignment |=|
//@[25:26] LeftBrace |{|
//@[26:27] NewLine |\n|
  dependenciesA: [
//@[2:15] Identifier |dependenciesA|
//@[15:16] Colon |:|
//@[17:18] LeftSquare |[|
//@[18:19] NewLine |\n|
    resA.id
//@[4:8] Identifier |resA|
//@[8:9] Dot |.|
//@[9:11] Identifier |id|
//@[11:12] NewLine |\n|
    resA.name
//@[4:8] Identifier |resA|
//@[8:9] Dot |.|
//@[9:13] Identifier |name|
//@[13:14] NewLine |\n|
    resA.type
//@[4:8] Identifier |resA|
//@[8:9] Dot |.|
//@[9:13] Identifier |type|
//@[13:14] NewLine |\n|
    resA.properties.deployTime
//@[4:8] Identifier |resA|
//@[8:9] Dot |.|
//@[9:19] Identifier |properties|
//@[19:20] Dot |.|
//@[20:30] Identifier |deployTime|
//@[30:31] NewLine |\n|
    resA.eTag
//@[4:8] Identifier |resA|
//@[8:9] Dot |.|
//@[9:13] Identifier |eTag|
//@[13:14] NewLine |\n|
  ]
//@[2:3] RightSquare |]|
//@[3:4] NewLine |\n|
}
//@[0:1] RightBrace |}|
//@[1:3] NewLine |\n\n|

output resourceAType string = resA.type
//@[0:6] Identifier |output|
//@[7:20] Identifier |resourceAType|
//@[21:27] Identifier |string|
//@[28:29] Assignment |=|
//@[30:34] Identifier |resA|
//@[34:35] Dot |.|
//@[35:39] Identifier |type|
//@[39:40] NewLine |\n|
resource resA 'My.Rp/myResourceType@2020-01-01' = {
//@[0:8] Identifier |resource|
//@[9:13] Identifier |resA|
//@[14:47] StringComplete |'My.Rp/myResourceType@2020-01-01'|
//@[48:49] Assignment |=|
//@[50:51] LeftBrace |{|
//@[51:52] NewLine |\n|
  name: 'resA'
//@[2:6] Identifier |name|
//@[6:7] Colon |:|
//@[8:14] StringComplete |'resA'|
//@[14:15] NewLine |\n|
  properties: {
//@[2:12] Identifier |properties|
//@[12:13] Colon |:|
//@[14:15] LeftBrace |{|
//@[15:16] NewLine |\n|
    deployTime: dependentVar
//@[4:14] Identifier |deployTime|
//@[14:15] Colon |:|
//@[16:28] Identifier |dependentVar|
//@[28:29] NewLine |\n|
  }
//@[2:3] RightBrace |}|
//@[3:4] NewLine |\n|
  eTag: '1234'
//@[2:6] Identifier |eTag|
//@[6:7] Colon |:|
//@[8:14] StringComplete |'1234'|
//@[14:15] NewLine |\n|
}
//@[0:1] RightBrace |}|
//@[1:3] NewLine |\n\n|

output resourceBId string = resB.id
//@[0:6] Identifier |output|
//@[7:18] Identifier |resourceBId|
//@[19:25] Identifier |string|
//@[26:27] Assignment |=|
//@[28:32] Identifier |resB|
//@[32:33] Dot |.|
//@[33:35] Identifier |id|
//@[35:36] NewLine |\n|
resource resB 'My.Rp/myResourceType@2020-01-01' = {
//@[0:8] Identifier |resource|
//@[9:13] Identifier |resB|
//@[14:47] StringComplete |'My.Rp/myResourceType@2020-01-01'|
//@[48:49] Assignment |=|
//@[50:51] LeftBrace |{|
//@[51:52] NewLine |\n|
  name: 'resB'
//@[2:6] Identifier |name|
//@[6:7] Colon |:|
//@[8:14] StringComplete |'resB'|
//@[14:15] NewLine |\n|
  properties: {
//@[2:12] Identifier |properties|
//@[12:13] Colon |:|
//@[14:15] LeftBrace |{|
//@[15:16] NewLine |\n|
    dependencies: resourceDependency
//@[4:16] Identifier |dependencies|
//@[16:17] Colon |:|
//@[18:36] Identifier |resourceDependency|
//@[36:37] NewLine |\n|
  }
//@[2:3] RightBrace |}|
//@[3:4] NewLine |\n|
}
//@[0:1] RightBrace |}|
//@[1:3] NewLine |\n\n|

var resourceIds = {
//@[0:3] Identifier |var|
//@[4:15] Identifier |resourceIds|
//@[16:17] Assignment |=|
//@[18:19] LeftBrace |{|
//@[19:20] NewLine |\n|
  a: resA.id
//@[2:3] Identifier |a|
//@[3:4] Colon |:|
//@[5:9] Identifier |resA|
//@[9:10] Dot |.|
//@[10:12] Identifier |id|
//@[12:13] NewLine |\n|
  b: resB.id
//@[2:3] Identifier |b|
//@[3:4] Colon |:|
//@[5:9] Identifier |resB|
//@[9:10] Dot |.|
//@[10:12] Identifier |id|
//@[12:13] NewLine |\n|
}
//@[0:1] RightBrace |}|
//@[1:3] NewLine |\n\n|

resource resC 'My.Rp/myResourceType@2020-01-01' = {
//@[0:8] Identifier |resource|
//@[9:13] Identifier |resC|
//@[14:47] StringComplete |'My.Rp/myResourceType@2020-01-01'|
//@[48:49] Assignment |=|
//@[50:51] LeftBrace |{|
//@[51:52] NewLine |\n|
  name: 'resC'
//@[2:6] Identifier |name|
//@[6:7] Colon |:|
//@[8:14] StringComplete |'resC'|
//@[14:15] NewLine |\n|
  properties: {
//@[2:12] Identifier |properties|
//@[12:13] Colon |:|
//@[14:15] LeftBrace |{|
//@[15:16] NewLine |\n|
    resourceIds: resourceIds
//@[4:15] Identifier |resourceIds|
//@[15:16] Colon |:|
//@[17:28] Identifier |resourceIds|
//@[28:29] NewLine |\n|
  }
//@[2:3] RightBrace |}|
//@[3:4] NewLine |\n|
}
//@[0:1] RightBrace |}|
//@[1:3] NewLine |\n\n|

resource resD 'My.Rp/myResourceType/childType@2020-01-01' = {
//@[0:8] Identifier |resource|
//@[9:13] Identifier |resD|
//@[14:57] StringComplete |'My.Rp/myResourceType/childType@2020-01-01'|
//@[58:59] Assignment |=|
//@[60:61] LeftBrace |{|
//@[61:62] NewLine |\n|
  name: '${resC.name}/resD'
//@[2:6] Identifier |name|
//@[6:7] Colon |:|
//@[8:11] StringLeftPiece |'${|
//@[11:15] Identifier |resC|
//@[15:16] Dot |.|
//@[16:20] Identifier |name|
//@[20:27] StringRightPiece |}/resD'|
//@[27:28] NewLine |\n|
  properties: {
//@[2:12] Identifier |properties|
//@[12:13] Colon |:|
//@[14:15] LeftBrace |{|
//@[15:16] NewLine |\n|
  }
//@[2:3] RightBrace |}|
//@[3:4] NewLine |\n|
}
//@[0:1] RightBrace |}|
//@[1:3] NewLine |\n\n|

resource resE 'My.Rp/myResourceType/childType@2020-01-01' = {
//@[0:8] Identifier |resource|
//@[9:13] Identifier |resE|
//@[14:57] StringComplete |'My.Rp/myResourceType/childType@2020-01-01'|
//@[58:59] Assignment |=|
//@[60:61] LeftBrace |{|
//@[61:62] NewLine |\n|
  name: 'resC/resD'
//@[2:6] Identifier |name|
//@[6:7] Colon |:|
//@[8:19] StringComplete |'resC/resD'|
//@[19:20] NewLine |\n|
  properties: {
//@[2:12] Identifier |properties|
//@[12:13] Colon |:|
//@[14:15] LeftBrace |{|
//@[15:16] NewLine |\n|
    resDRef: resD.id
//@[4:11] Identifier |resDRef|
//@[11:12] Colon |:|
//@[13:17] Identifier |resD|
//@[17:18] Dot |.|
//@[18:20] Identifier |id|
//@[20:21] NewLine |\n|
  }
//@[2:3] RightBrace |}|
//@[3:4] NewLine |\n|
}
//@[0:1] RightBrace |}|
//@[1:3] NewLine |\n\n|

output resourceCProperties object = resC.properties
//@[0:6] Identifier |output|
//@[7:26] Identifier |resourceCProperties|
//@[27:33] Identifier |object|
//@[34:35] Assignment |=|
//@[36:40] Identifier |resC|
//@[40:41] Dot |.|
//@[41:51] Identifier |properties|
//@[51:51] EndOfFile ||
