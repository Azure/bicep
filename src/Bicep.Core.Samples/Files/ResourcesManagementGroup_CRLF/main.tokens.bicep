targetScope = 'managementGroup'
//@[0:11) Identifier |targetScope|
//@[12:13) Assignment |=|
//@[14:31) StringComplete |'managementGroup'|
//@[31:35) NewLine |\r\n\r\n|

param ownerPrincipalId string
//@[0:5) Identifier |param|
//@[6:22) Identifier |ownerPrincipalId|
//@[23:29) Identifier |string|
//@[29:33) NewLine |\r\n\r\n|

param contributorPrincipals array
//@[0:5) Identifier |param|
//@[6:27) Identifier |contributorPrincipals|
//@[28:33) Identifier |array|
//@[33:35) NewLine |\r\n|
param readerPrincipals array
//@[0:5) Identifier |param|
//@[6:22) Identifier |readerPrincipals|
//@[23:28) Identifier |array|
//@[28:32) NewLine |\r\n\r\n|

resource owner 'Microsoft.Authorization/roleAssignments@2020-04-01-preview' = {
//@[0:8) Identifier |resource|
//@[9:14) Identifier |owner|
//@[15:75) StringComplete |'Microsoft.Authorization/roleAssignments@2020-04-01-preview'|
//@[76:77) Assignment |=|
//@[78:79) LeftBrace |{|
//@[79:81) NewLine |\r\n|
  name: guid('owner', ownerPrincipalId)
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:12) Identifier |guid|
//@[12:13) LeftParen |(|
//@[13:20) StringComplete |'owner'|
//@[20:21) Comma |,|
//@[22:38) Identifier |ownerPrincipalId|
//@[38:39) RightParen |)|
//@[39:41) NewLine |\r\n|
  properties: {
//@[2:12) Identifier |properties|
//@[12:13) Colon |:|
//@[14:15) LeftBrace |{|
//@[15:17) NewLine |\r\n|
    principalId: ownerPrincipalId
//@[4:15) Identifier |principalId|
//@[15:16) Colon |:|
//@[17:33) Identifier |ownerPrincipalId|
//@[33:35) NewLine |\r\n|
    roleDefinitionId: '8e3af657-a8ff-443c-a75c-2fe8c4bcb635'
//@[4:20) Identifier |roleDefinitionId|
//@[20:21) Colon |:|
//@[22:60) StringComplete |'8e3af657-a8ff-443c-a75c-2fe8c4bcb635'|
//@[60:62) NewLine |\r\n|
  }
//@[2:3) RightBrace |}|
//@[3:5) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource contributors 'Microsoft.Authorization/roleAssignments@2020-04-01-preview' = [for contributor in contributorPrincipals: {
//@[0:8) Identifier |resource|
//@[9:21) Identifier |contributors|
//@[22:82) StringComplete |'Microsoft.Authorization/roleAssignments@2020-04-01-preview'|
//@[83:84) Assignment |=|
//@[85:86) LeftSquare |[|
//@[86:89) Identifier |for|
//@[90:101) Identifier |contributor|
//@[102:104) Identifier |in|
//@[105:126) Identifier |contributorPrincipals|
//@[126:127) Colon |:|
//@[128:129) LeftBrace |{|
//@[129:131) NewLine |\r\n|
  name: guid('contributor', contributor)
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:12) Identifier |guid|
//@[12:13) LeftParen |(|
//@[13:26) StringComplete |'contributor'|
//@[26:27) Comma |,|
//@[28:39) Identifier |contributor|
//@[39:40) RightParen |)|
//@[40:42) NewLine |\r\n|
  properties: {
//@[2:12) Identifier |properties|
//@[12:13) Colon |:|
//@[14:15) LeftBrace |{|
//@[15:17) NewLine |\r\n|
    principalId: contributor
//@[4:15) Identifier |principalId|
//@[15:16) Colon |:|
//@[17:28) Identifier |contributor|
//@[28:30) NewLine |\r\n|
    roleDefinitionId: 'b24988ac-6180-42a0-ab88-20f7382dd24c'
//@[4:20) Identifier |roleDefinitionId|
//@[20:21) Colon |:|
//@[22:60) StringComplete |'b24988ac-6180-42a0-ab88-20f7382dd24c'|
//@[60:62) NewLine |\r\n|
  }
//@[2:3) RightBrace |}|
//@[3:5) NewLine |\r\n|
  dependsOn: [
//@[2:11) Identifier |dependsOn|
//@[11:12) Colon |:|
//@[13:14) LeftSquare |[|
//@[14:16) NewLine |\r\n|
    owner
//@[4:9) Identifier |owner|
//@[9:11) NewLine |\r\n|
  ]
//@[2:3) RightSquare |]|
//@[3:5) NewLine |\r\n|
}]
//@[0:1) RightBrace |}|
//@[1:2) RightSquare |]|
//@[2:6) NewLine |\r\n\r\n|

resource readers 'Microsoft.Authorization/roleAssignments@2020-04-01-preview' = [for reader in readerPrincipals: {
//@[0:8) Identifier |resource|
//@[9:16) Identifier |readers|
//@[17:77) StringComplete |'Microsoft.Authorization/roleAssignments@2020-04-01-preview'|
//@[78:79) Assignment |=|
//@[80:81) LeftSquare |[|
//@[81:84) Identifier |for|
//@[85:91) Identifier |reader|
//@[92:94) Identifier |in|
//@[95:111) Identifier |readerPrincipals|
//@[111:112) Colon |:|
//@[113:114) LeftBrace |{|
//@[114:116) NewLine |\r\n|
  name: guid('reader', reader)
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:12) Identifier |guid|
//@[12:13) LeftParen |(|
//@[13:21) StringComplete |'reader'|
//@[21:22) Comma |,|
//@[23:29) Identifier |reader|
//@[29:30) RightParen |)|
//@[30:32) NewLine |\r\n|
  properties: {
//@[2:12) Identifier |properties|
//@[12:13) Colon |:|
//@[14:15) LeftBrace |{|
//@[15:17) NewLine |\r\n|
    principalId: reader
//@[4:15) Identifier |principalId|
//@[15:16) Colon |:|
//@[17:23) Identifier |reader|
//@[23:25) NewLine |\r\n|
    roleDefinitionId: 'b24988ac-6180-42a0-ab88-20f7382dd24c'
//@[4:20) Identifier |roleDefinitionId|
//@[20:21) Colon |:|
//@[22:60) StringComplete |'b24988ac-6180-42a0-ab88-20f7382dd24c'|
//@[60:62) NewLine |\r\n|
  }
//@[2:3) RightBrace |}|
//@[3:5) NewLine |\r\n|
  dependsOn: [
//@[2:11) Identifier |dependsOn|
//@[11:12) Colon |:|
//@[13:14) LeftSquare |[|
//@[14:16) NewLine |\r\n|
    owner
//@[4:9) Identifier |owner|
//@[9:11) NewLine |\r\n|
    contributors[0]
//@[4:16) Identifier |contributors|
//@[16:17) LeftSquare |[|
//@[17:18) Integer |0|
//@[18:19) RightSquare |]|
//@[19:21) NewLine |\r\n|
  ]
//@[2:3) RightSquare |]|
//@[3:5) NewLine |\r\n|
}]
//@[0:1) RightBrace |}|
//@[1:2) RightSquare |]|
//@[2:6) NewLine |\r\n\r\n|

resource single_mg 'Microsoft.Management/managementGroups@2020-05-01' = {
//@[0:8) Identifier |resource|
//@[9:18) Identifier |single_mg|
//@[19:69) StringComplete |'Microsoft.Management/managementGroups@2020-05-01'|
//@[70:71) Assignment |=|
//@[72:73) LeftBrace |{|
//@[73:75) NewLine |\r\n|
  scope: tenant()
//@[2:7) Identifier |scope|
//@[7:8) Colon |:|
//@[9:15) Identifier |tenant|
//@[15:16) LeftParen |(|
//@[16:17) RightParen |)|
//@[17:19) NewLine |\r\n|
  name: 'one-mg'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:16) StringComplete |'one-mg'|
//@[16:18) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

// Blueprints are read-only at tenant Scope, but it's a convenient example to use to validate this.
//@[99:101) NewLine |\r\n|
resource tenant_blueprint 'Microsoft.Blueprint/blueprints@2018-11-01-preview' = {
//@[0:8) Identifier |resource|
//@[9:25) Identifier |tenant_blueprint|
//@[26:77) StringComplete |'Microsoft.Blueprint/blueprints@2018-11-01-preview'|
//@[78:79) Assignment |=|
//@[80:81) LeftBrace |{|
//@[81:83) NewLine |\r\n|
  name: 'tenant-blueprint'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:26) StringComplete |'tenant-blueprint'|
//@[26:28) NewLine |\r\n|
  properties: {}
//@[2:12) Identifier |properties|
//@[12:13) Colon |:|
//@[14:15) LeftBrace |{|
//@[15:16) RightBrace |}|
//@[16:18) NewLine |\r\n|
  scope: tenant()
//@[2:7) Identifier |scope|
//@[7:8) Colon |:|
//@[9:15) Identifier |tenant|
//@[15:16) LeftParen |(|
//@[16:17) RightParen |)|
//@[17:19) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource mg_blueprint 'Microsoft.Blueprint/blueprints@2018-11-01-preview' = {
//@[0:8) Identifier |resource|
//@[9:21) Identifier |mg_blueprint|
//@[22:73) StringComplete |'Microsoft.Blueprint/blueprints@2018-11-01-preview'|
//@[74:75) Assignment |=|
//@[76:77) LeftBrace |{|
//@[77:79) NewLine |\r\n|
  name: 'mg-blueprint'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:22) StringComplete |'mg-blueprint'|
//@[22:24) NewLine |\r\n|
  properties: {}
//@[2:12) Identifier |properties|
//@[12:13) Colon |:|
//@[14:15) LeftBrace |{|
//@[15:16) RightBrace |}|
//@[16:18) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\r\n|

//@[0:0) EndOfFile ||
