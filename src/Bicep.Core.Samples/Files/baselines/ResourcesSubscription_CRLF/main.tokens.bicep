targetScope = 'subscription'
//@[000:011) Identifier |targetScope|
//@[012:013) Assignment |=|
//@[014:028) StringComplete |'subscription'|
//@[028:032) NewLine |\r\n\r\n|

param ownerPrincipalId string
//@[000:005) Identifier |param|
//@[006:022) Identifier |ownerPrincipalId|
//@[023:029) Identifier |string|
//@[029:033) NewLine |\r\n\r\n|

param contributorPrincipals array
//@[000:005) Identifier |param|
//@[006:027) Identifier |contributorPrincipals|
//@[028:033) Identifier |array|
//@[033:035) NewLine |\r\n|
param readerPrincipals array
//@[000:005) Identifier |param|
//@[006:022) Identifier |readerPrincipals|
//@[023:028) Identifier |array|
//@[028:032) NewLine |\r\n\r\n|

resource owner 'Microsoft.Authorization/roleAssignments@2020-04-01-preview' = {
//@[000:008) Identifier |resource|
//@[009:014) Identifier |owner|
//@[015:075) StringComplete |'Microsoft.Authorization/roleAssignments@2020-04-01-preview'|
//@[076:077) Assignment |=|
//@[078:079) LeftBrace |{|
//@[079:081) NewLine |\r\n|
  name: guid('owner', ownerPrincipalId)
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:012) Identifier |guid|
//@[012:013) LeftParen |(|
//@[013:020) StringComplete |'owner'|
//@[020:021) Comma |,|
//@[022:038) Identifier |ownerPrincipalId|
//@[038:039) RightParen |)|
//@[039:041) NewLine |\r\n|
  properties: {
//@[002:012) Identifier |properties|
//@[012:013) Colon |:|
//@[014:015) LeftBrace |{|
//@[015:017) NewLine |\r\n|
    principalId: ownerPrincipalId
//@[004:015) Identifier |principalId|
//@[015:016) Colon |:|
//@[017:033) Identifier |ownerPrincipalId|
//@[033:035) NewLine |\r\n|
    roleDefinitionId: '8e3af657-a8ff-443c-a75c-2fe8c4bcb635'
//@[004:020) Identifier |roleDefinitionId|
//@[020:021) Colon |:|
//@[022:060) StringComplete |'8e3af657-a8ff-443c-a75c-2fe8c4bcb635'|
//@[060:062) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

resource contributors 'Microsoft.Authorization/roleAssignments@2020-04-01-preview' = [for contributor in contributorPrincipals: {
//@[000:008) Identifier |resource|
//@[009:021) Identifier |contributors|
//@[022:082) StringComplete |'Microsoft.Authorization/roleAssignments@2020-04-01-preview'|
//@[083:084) Assignment |=|
//@[085:086) LeftSquare |[|
//@[086:089) Identifier |for|
//@[090:101) Identifier |contributor|
//@[102:104) Identifier |in|
//@[105:126) Identifier |contributorPrincipals|
//@[126:127) Colon |:|
//@[128:129) LeftBrace |{|
//@[129:131) NewLine |\r\n|
  name: guid('contributor', contributor)
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:012) Identifier |guid|
//@[012:013) LeftParen |(|
//@[013:026) StringComplete |'contributor'|
//@[026:027) Comma |,|
//@[028:039) Identifier |contributor|
//@[039:040) RightParen |)|
//@[040:042) NewLine |\r\n|
  properties: {
//@[002:012) Identifier |properties|
//@[012:013) Colon |:|
//@[014:015) LeftBrace |{|
//@[015:017) NewLine |\r\n|
    principalId: contributor
//@[004:015) Identifier |principalId|
//@[015:016) Colon |:|
//@[017:028) Identifier |contributor|
//@[028:030) NewLine |\r\n|
    roleDefinitionId: 'b24988ac-6180-42a0-ab88-20f7382dd24c'
//@[004:020) Identifier |roleDefinitionId|
//@[020:021) Colon |:|
//@[022:060) StringComplete |'b24988ac-6180-42a0-ab88-20f7382dd24c'|
//@[060:062) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
  dependsOn: [
//@[002:011) Identifier |dependsOn|
//@[011:012) Colon |:|
//@[013:014) LeftSquare |[|
//@[014:016) NewLine |\r\n|
    owner
//@[004:009) Identifier |owner|
//@[009:011) NewLine |\r\n|
  ]
//@[002:003) RightSquare |]|
//@[003:005) NewLine |\r\n|
}]
//@[000:001) RightBrace |}|
//@[001:002) RightSquare |]|
//@[002:006) NewLine |\r\n\r\n|

resource readers 'Microsoft.Authorization/roleAssignments@2020-04-01-preview' = [for reader in readerPrincipals: {
//@[000:008) Identifier |resource|
//@[009:016) Identifier |readers|
//@[017:077) StringComplete |'Microsoft.Authorization/roleAssignments@2020-04-01-preview'|
//@[078:079) Assignment |=|
//@[080:081) LeftSquare |[|
//@[081:084) Identifier |for|
//@[085:091) Identifier |reader|
//@[092:094) Identifier |in|
//@[095:111) Identifier |readerPrincipals|
//@[111:112) Colon |:|
//@[113:114) LeftBrace |{|
//@[114:116) NewLine |\r\n|
  name: guid('reader', reader)
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:012) Identifier |guid|
//@[012:013) LeftParen |(|
//@[013:021) StringComplete |'reader'|
//@[021:022) Comma |,|
//@[023:029) Identifier |reader|
//@[029:030) RightParen |)|
//@[030:032) NewLine |\r\n|
  properties: {
//@[002:012) Identifier |properties|
//@[012:013) Colon |:|
//@[014:015) LeftBrace |{|
//@[015:017) NewLine |\r\n|
    principalId: reader
//@[004:015) Identifier |principalId|
//@[015:016) Colon |:|
//@[017:023) Identifier |reader|
//@[023:025) NewLine |\r\n|
    roleDefinitionId: 'b24988ac-6180-42a0-ab88-20f7382dd24c'
//@[004:020) Identifier |roleDefinitionId|
//@[020:021) Colon |:|
//@[022:060) StringComplete |'b24988ac-6180-42a0-ab88-20f7382dd24c'|
//@[060:062) NewLine |\r\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\r\n|
  dependsOn: [
//@[002:011) Identifier |dependsOn|
//@[011:012) Colon |:|
//@[013:014) LeftSquare |[|
//@[014:016) NewLine |\r\n|
    owner
//@[004:009) Identifier |owner|
//@[009:011) NewLine |\r\n|
    contributors[0]
//@[004:016) Identifier |contributors|
//@[016:017) LeftSquare |[|
//@[017:018) Integer |0|
//@[018:019) RightSquare |]|
//@[019:021) NewLine |\r\n|
  ]
//@[002:003) RightSquare |]|
//@[003:005) NewLine |\r\n|
}]
//@[000:001) RightBrace |}|
//@[001:002) RightSquare |]|
//@[002:004) NewLine |\r\n|

//@[000:000) EndOfFile ||
