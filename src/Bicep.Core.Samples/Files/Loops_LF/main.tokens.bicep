param name string
//@[0:5) Identifier |param|
//@[6:10) Identifier |name|
//@[11:17) Identifier |string|
//@[17:18) NewLine |\n|
param accounts array
//@[0:5) Identifier |param|
//@[6:14) Identifier |accounts|
//@[15:20) Identifier |array|
//@[20:21) NewLine |\n|
param index int
//@[0:5) Identifier |param|
//@[6:11) Identifier |index|
//@[12:15) Identifier |int|
//@[15:17) NewLine |\n\n|

// single resource
//@[18:19) NewLine |\n|
resource singleResource 'Microsoft.Storage/storageAccounts@2019-06-01' = {
//@[0:8) Identifier |resource|
//@[9:23) Identifier |singleResource|
//@[24:70) StringComplete |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[71:72) Assignment |=|
//@[73:74) LeftBrace |{|
//@[74:75) NewLine |\n|
  name: '${name}single-resource-name'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:11) StringLeftPiece |'${|
//@[11:15) Identifier |name|
//@[15:37) StringRightPiece |}single-resource-name'|
//@[37:38) NewLine |\n|
  location: resourceGroup().location
//@[2:10) Identifier |location|
//@[10:11) Colon |:|
//@[12:25) Identifier |resourceGroup|
//@[25:26) LeftParen |(|
//@[26:27) RightParen |)|
//@[27:28) Dot |.|
//@[28:36) Identifier |location|
//@[36:37) NewLine |\n|
  kind: 'StorageV2'
//@[2:6) Identifier |kind|
//@[6:7) Colon |:|
//@[8:19) StringComplete |'StorageV2'|
//@[19:20) NewLine |\n|
  sku: {
//@[2:5) Identifier |sku|
//@[5:6) Colon |:|
//@[7:8) LeftBrace |{|
//@[8:9) NewLine |\n|
    name: 'Standard_LRS'
//@[4:8) Identifier |name|
//@[8:9) Colon |:|
//@[10:24) StringComplete |'Standard_LRS'|
//@[24:25) NewLine |\n|
  }
//@[2:3) RightBrace |}|
//@[3:4) NewLine |\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

// extension of single resource
//@[31:32) NewLine |\n|
resource singleResourceExtension 'Microsoft.Authorization/locks@2016-09-01' = {
//@[0:8) Identifier |resource|
//@[9:32) Identifier |singleResourceExtension|
//@[33:75) StringComplete |'Microsoft.Authorization/locks@2016-09-01'|
//@[76:77) Assignment |=|
//@[78:79) LeftBrace |{|
//@[79:80) NewLine |\n|
  scope: singleResource
//@[2:7) Identifier |scope|
//@[7:8) Colon |:|
//@[9:23) Identifier |singleResource|
//@[23:24) NewLine |\n|
  name: 'single-resource-lock'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:30) StringComplete |'single-resource-lock'|
//@[30:31) NewLine |\n|
  properties: {
//@[2:12) Identifier |properties|
//@[12:13) Colon |:|
//@[14:15) LeftBrace |{|
//@[15:16) NewLine |\n|
    level: 'CanNotDelete'
//@[4:9) Identifier |level|
//@[9:10) Colon |:|
//@[11:25) StringComplete |'CanNotDelete'|
//@[25:26) NewLine |\n|
  }
//@[2:3) RightBrace |}|
//@[3:4) NewLine |\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

// single resource cascade extension
//@[36:37) NewLine |\n|
resource singleResourceCascadeExtension 'Microsoft.Authorization/locks@2016-09-01' = {
//@[0:8) Identifier |resource|
//@[9:39) Identifier |singleResourceCascadeExtension|
//@[40:82) StringComplete |'Microsoft.Authorization/locks@2016-09-01'|
//@[83:84) Assignment |=|
//@[85:86) LeftBrace |{|
//@[86:87) NewLine |\n|
  scope: singleResourceExtension
//@[2:7) Identifier |scope|
//@[7:8) Colon |:|
//@[9:32) Identifier |singleResourceExtension|
//@[32:33) NewLine |\n|
  name: 'single-resource-cascade-extension'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:43) StringComplete |'single-resource-cascade-extension'|
//@[43:44) NewLine |\n|
  properties: {
//@[2:12) Identifier |properties|
//@[12:13) Colon |:|
//@[14:15) LeftBrace |{|
//@[15:16) NewLine |\n|
    level: 'CanNotDelete'
//@[4:9) Identifier |level|
//@[9:10) Colon |:|
//@[11:25) StringComplete |'CanNotDelete'|
//@[25:26) NewLine |\n|
  }
//@[2:3) RightBrace |}|
//@[3:4) NewLine |\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

// resource collection
//@[22:23) NewLine |\n|
@batchSize(42)
//@[0:1) At |@|
//@[1:10) Identifier |batchSize|
//@[10:11) LeftParen |(|
//@[11:13) Integer |42|
//@[13:14) RightParen |)|
//@[14:15) NewLine |\n|
resource storageAccounts 'Microsoft.Storage/storageAccounts@2019-06-01' = [for account in accounts: {
//@[0:8) Identifier |resource|
//@[9:24) Identifier |storageAccounts|
//@[25:71) StringComplete |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[72:73) Assignment |=|
//@[74:75) LeftSquare |[|
//@[75:78) Identifier |for|
//@[79:86) Identifier |account|
//@[87:89) Identifier |in|
//@[90:98) Identifier |accounts|
//@[98:99) Colon |:|
//@[100:101) LeftBrace |{|
//@[101:102) NewLine |\n|
  name: '${name}-collection-${account.name}'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:11) StringLeftPiece |'${|
//@[11:15) Identifier |name|
//@[15:30) StringMiddlePiece |}-collection-${|
//@[30:37) Identifier |account|
//@[37:38) Dot |.|
//@[38:42) Identifier |name|
//@[42:44) StringRightPiece |}'|
//@[44:45) NewLine |\n|
  location: account.location
//@[2:10) Identifier |location|
//@[10:11) Colon |:|
//@[12:19) Identifier |account|
//@[19:20) Dot |.|
//@[20:28) Identifier |location|
//@[28:29) NewLine |\n|
  kind: 'StorageV2'
//@[2:6) Identifier |kind|
//@[6:7) Colon |:|
//@[8:19) StringComplete |'StorageV2'|
//@[19:20) NewLine |\n|
  sku: {
//@[2:5) Identifier |sku|
//@[5:6) Colon |:|
//@[7:8) LeftBrace |{|
//@[8:9) NewLine |\n|
    name: 'Standard_LRS'
//@[4:8) Identifier |name|
//@[8:9) Colon |:|
//@[10:24) StringComplete |'Standard_LRS'|
//@[24:25) NewLine |\n|
  }
//@[2:3) RightBrace |}|
//@[3:4) NewLine |\n|
  dependsOn: [
//@[2:11) Identifier |dependsOn|
//@[11:12) Colon |:|
//@[13:14) LeftSquare |[|
//@[14:15) NewLine |\n|
    singleResource
//@[4:18) Identifier |singleResource|
//@[18:19) NewLine |\n|
  ]
//@[2:3) RightSquare |]|
//@[3:4) NewLine |\n|
}]
//@[0:1) RightBrace |}|
//@[1:2) RightSquare |]|
//@[2:4) NewLine |\n\n|

// extension of a single resource in a collection
//@[49:50) NewLine |\n|
resource extendSingleResourceInCollection 'Microsoft.Authorization/locks@2016-09-01' = {
//@[0:8) Identifier |resource|
//@[9:41) Identifier |extendSingleResourceInCollection|
//@[42:84) StringComplete |'Microsoft.Authorization/locks@2016-09-01'|
//@[85:86) Assignment |=|
//@[87:88) LeftBrace |{|
//@[88:89) NewLine |\n|
  name: 'one-resource-collection-item-lock'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:43) StringComplete |'one-resource-collection-item-lock'|
//@[43:44) NewLine |\n|
  properties: {
//@[2:12) Identifier |properties|
//@[12:13) Colon |:|
//@[14:15) LeftBrace |{|
//@[15:16) NewLine |\n|
    level: 'ReadOnly'
//@[4:9) Identifier |level|
//@[9:10) Colon |:|
//@[11:21) StringComplete |'ReadOnly'|
//@[21:22) NewLine |\n|
  }
//@[2:3) RightBrace |}|
//@[3:4) NewLine |\n|
  scope: storageAccounts[index % 2]
//@[2:7) Identifier |scope|
//@[7:8) Colon |:|
//@[9:24) Identifier |storageAccounts|
//@[24:25) LeftSquare |[|
//@[25:30) Identifier |index|
//@[31:32) Modulo |%|
//@[33:34) Integer |2|
//@[34:35) RightSquare |]|
//@[35:36) NewLine |\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

// collection of extensions
//@[27:28) NewLine |\n|
resource extensionCollection 'Microsoft.Authorization/locks@2016-09-01' = [for i in range(0,1): {
//@[0:8) Identifier |resource|
//@[9:28) Identifier |extensionCollection|
//@[29:71) StringComplete |'Microsoft.Authorization/locks@2016-09-01'|
//@[72:73) Assignment |=|
//@[74:75) LeftSquare |[|
//@[75:78) Identifier |for|
//@[79:80) Identifier |i|
//@[81:83) Identifier |in|
//@[84:89) Identifier |range|
//@[89:90) LeftParen |(|
//@[90:91) Integer |0|
//@[91:92) Comma |,|
//@[92:93) Integer |1|
//@[93:94) RightParen |)|
//@[94:95) Colon |:|
//@[96:97) LeftBrace |{|
//@[97:98) NewLine |\n|
  name: 'lock-${i}'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:16) StringLeftPiece |'lock-${|
//@[16:17) Identifier |i|
//@[17:19) StringRightPiece |}'|
//@[19:20) NewLine |\n|
  properties: {
//@[2:12) Identifier |properties|
//@[12:13) Colon |:|
//@[14:15) LeftBrace |{|
//@[15:16) NewLine |\n|
    level: i == 0 ? 'CanNotDelete' : 'ReadOnly'
//@[4:9) Identifier |level|
//@[9:10) Colon |:|
//@[11:12) Identifier |i|
//@[13:15) Equals |==|
//@[16:17) Integer |0|
//@[18:19) Question |?|
//@[20:34) StringComplete |'CanNotDelete'|
//@[35:36) Colon |:|
//@[37:47) StringComplete |'ReadOnly'|
//@[47:48) NewLine |\n|
  }
//@[2:3) RightBrace |}|
//@[3:4) NewLine |\n|
  scope: singleResource
//@[2:7) Identifier |scope|
//@[7:8) Colon |:|
//@[9:23) Identifier |singleResource|
//@[23:24) NewLine |\n|
}]
//@[0:1) RightBrace |}|
//@[1:2) RightSquare |]|
//@[2:4) NewLine |\n\n|

// cascade extend the extension
//@[31:32) NewLine |\n|
@batchSize(1)
//@[0:1) At |@|
//@[1:10) Identifier |batchSize|
//@[10:11) LeftParen |(|
//@[11:12) Integer |1|
//@[12:13) RightParen |)|
//@[13:14) NewLine |\n|
resource lockTheLocks 'Microsoft.Authorization/locks@2016-09-01' = [for i in range(0,1): {
//@[0:8) Identifier |resource|
//@[9:21) Identifier |lockTheLocks|
//@[22:64) StringComplete |'Microsoft.Authorization/locks@2016-09-01'|
//@[65:66) Assignment |=|
//@[67:68) LeftSquare |[|
//@[68:71) Identifier |for|
//@[72:73) Identifier |i|
//@[74:76) Identifier |in|
//@[77:82) Identifier |range|
//@[82:83) LeftParen |(|
//@[83:84) Integer |0|
//@[84:85) Comma |,|
//@[85:86) Integer |1|
//@[86:87) RightParen |)|
//@[87:88) Colon |:|
//@[89:90) LeftBrace |{|
//@[90:91) NewLine |\n|
  name: 'lock-the-lock-${i}'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:25) StringLeftPiece |'lock-the-lock-${|
//@[25:26) Identifier |i|
//@[26:28) StringRightPiece |}'|
//@[28:29) NewLine |\n|
  properties: {
//@[2:12) Identifier |properties|
//@[12:13) Colon |:|
//@[14:15) LeftBrace |{|
//@[15:16) NewLine |\n|
    level: i == 0 ? 'CanNotDelete' : 'ReadOnly'
//@[4:9) Identifier |level|
//@[9:10) Colon |:|
//@[11:12) Identifier |i|
//@[13:15) Equals |==|
//@[16:17) Integer |0|
//@[18:19) Question |?|
//@[20:34) StringComplete |'CanNotDelete'|
//@[35:36) Colon |:|
//@[37:47) StringComplete |'ReadOnly'|
//@[47:48) NewLine |\n|
  }
//@[2:3) RightBrace |}|
//@[3:4) NewLine |\n|
  scope: extensionCollection[i]
//@[2:7) Identifier |scope|
//@[7:8) Colon |:|
//@[9:28) Identifier |extensionCollection|
//@[28:29) LeftSquare |[|
//@[29:30) Identifier |i|
//@[30:31) RightSquare |]|
//@[31:32) NewLine |\n|
}]
//@[0:1) RightBrace |}|
//@[1:2) RightSquare |]|
//@[2:4) NewLine |\n\n|

// special case property access
//@[31:32) NewLine |\n|
output indexedCollectionBlobEndpoint string = storageAccounts[index].properties.primaryEndpoints.blob
//@[0:6) Identifier |output|
//@[7:36) Identifier |indexedCollectionBlobEndpoint|
//@[37:43) Identifier |string|
//@[44:45) Assignment |=|
//@[46:61) Identifier |storageAccounts|
//@[61:62) LeftSquare |[|
//@[62:67) Identifier |index|
//@[67:68) RightSquare |]|
//@[68:69) Dot |.|
//@[69:79) Identifier |properties|
//@[79:80) Dot |.|
//@[80:96) Identifier |primaryEndpoints|
//@[96:97) Dot |.|
//@[97:101) Identifier |blob|
//@[101:102) NewLine |\n|
output indexedCollectionName string = storageAccounts[index].name
//@[0:6) Identifier |output|
//@[7:28) Identifier |indexedCollectionName|
//@[29:35) Identifier |string|
//@[36:37) Assignment |=|
//@[38:53) Identifier |storageAccounts|
//@[53:54) LeftSquare |[|
//@[54:59) Identifier |index|
//@[59:60) RightSquare |]|
//@[60:61) Dot |.|
//@[61:65) Identifier |name|
//@[65:66) NewLine |\n|
output indexedCollectionId string = storageAccounts[index].id
//@[0:6) Identifier |output|
//@[7:26) Identifier |indexedCollectionId|
//@[27:33) Identifier |string|
//@[34:35) Assignment |=|
//@[36:51) Identifier |storageAccounts|
//@[51:52) LeftSquare |[|
//@[52:57) Identifier |index|
//@[57:58) RightSquare |]|
//@[58:59) Dot |.|
//@[59:61) Identifier |id|
//@[61:62) NewLine |\n|
output indexedCollectionType string = storageAccounts[index].type
//@[0:6) Identifier |output|
//@[7:28) Identifier |indexedCollectionType|
//@[29:35) Identifier |string|
//@[36:37) Assignment |=|
//@[38:53) Identifier |storageAccounts|
//@[53:54) LeftSquare |[|
//@[54:59) Identifier |index|
//@[59:60) RightSquare |]|
//@[60:61) Dot |.|
//@[61:65) Identifier |type|
//@[65:66) NewLine |\n|
output indexedCollectionVersion string = storageAccounts[index].apiVersion
//@[0:6) Identifier |output|
//@[7:31) Identifier |indexedCollectionVersion|
//@[32:38) Identifier |string|
//@[39:40) Assignment |=|
//@[41:56) Identifier |storageAccounts|
//@[56:57) LeftSquare |[|
//@[57:62) Identifier |index|
//@[62:63) RightSquare |]|
//@[63:64) Dot |.|
//@[64:74) Identifier |apiVersion|
//@[74:76) NewLine |\n\n|

// general case property access
//@[31:32) NewLine |\n|
output indexedCollectionIdentity object = storageAccounts[index].identity
//@[0:6) Identifier |output|
//@[7:32) Identifier |indexedCollectionIdentity|
//@[33:39) Identifier |object|
//@[40:41) Assignment |=|
//@[42:57) Identifier |storageAccounts|
//@[57:58) LeftSquare |[|
//@[58:63) Identifier |index|
//@[63:64) RightSquare |]|
//@[64:65) Dot |.|
//@[65:73) Identifier |identity|
//@[73:75) NewLine |\n\n|

// indexed access of two properties
//@[35:36) NewLine |\n|
output indexedEndpointPair object = {
//@[0:6) Identifier |output|
//@[7:26) Identifier |indexedEndpointPair|
//@[27:33) Identifier |object|
//@[34:35) Assignment |=|
//@[36:37) LeftBrace |{|
//@[37:38) NewLine |\n|
  primary: storageAccounts[index].properties.primaryEndpoints.blob
//@[2:9) Identifier |primary|
//@[9:10) Colon |:|
//@[11:26) Identifier |storageAccounts|
//@[26:27) LeftSquare |[|
//@[27:32) Identifier |index|
//@[32:33) RightSquare |]|
//@[33:34) Dot |.|
//@[34:44) Identifier |properties|
//@[44:45) Dot |.|
//@[45:61) Identifier |primaryEndpoints|
//@[61:62) Dot |.|
//@[62:66) Identifier |blob|
//@[66:67) NewLine |\n|
  secondary: storageAccounts[index + 1].properties.secondaryEndpoints.blob
//@[2:11) Identifier |secondary|
//@[11:12) Colon |:|
//@[13:28) Identifier |storageAccounts|
//@[28:29) LeftSquare |[|
//@[29:34) Identifier |index|
//@[35:36) Plus |+|
//@[37:38) Integer |1|
//@[38:39) RightSquare |]|
//@[39:40) Dot |.|
//@[40:50) Identifier |properties|
//@[50:51) Dot |.|
//@[51:69) Identifier |secondaryEndpoints|
//@[69:70) Dot |.|
//@[70:74) Identifier |blob|
//@[74:75) NewLine |\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

// nested indexer?
//@[18:19) NewLine |\n|
output indexViaReference string = storageAccounts[int(storageAccounts[index].properties.creationTime)].properties.accessTier
//@[0:6) Identifier |output|
//@[7:24) Identifier |indexViaReference|
//@[25:31) Identifier |string|
//@[32:33) Assignment |=|
//@[34:49) Identifier |storageAccounts|
//@[49:50) LeftSquare |[|
//@[50:53) Identifier |int|
//@[53:54) LeftParen |(|
//@[54:69) Identifier |storageAccounts|
//@[69:70) LeftSquare |[|
//@[70:75) Identifier |index|
//@[75:76) RightSquare |]|
//@[76:77) Dot |.|
//@[77:87) Identifier |properties|
//@[87:88) Dot |.|
//@[88:100) Identifier |creationTime|
//@[100:101) RightParen |)|
//@[101:102) RightSquare |]|
//@[102:103) Dot |.|
//@[103:113) Identifier |properties|
//@[113:114) Dot |.|
//@[114:124) Identifier |accessTier|
//@[124:126) NewLine |\n\n|

// dependency on a resource collection
//@[38:39) NewLine |\n|
resource storageAccounts2 'Microsoft.Storage/storageAccounts@2019-06-01' = [for account in accounts: {
//@[0:8) Identifier |resource|
//@[9:25) Identifier |storageAccounts2|
//@[26:72) StringComplete |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[73:74) Assignment |=|
//@[75:76) LeftSquare |[|
//@[76:79) Identifier |for|
//@[80:87) Identifier |account|
//@[88:90) Identifier |in|
//@[91:99) Identifier |accounts|
//@[99:100) Colon |:|
//@[101:102) LeftBrace |{|
//@[102:103) NewLine |\n|
  name: '${name}-collection-${account.name}'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:11) StringLeftPiece |'${|
//@[11:15) Identifier |name|
//@[15:30) StringMiddlePiece |}-collection-${|
//@[30:37) Identifier |account|
//@[37:38) Dot |.|
//@[38:42) Identifier |name|
//@[42:44) StringRightPiece |}'|
//@[44:45) NewLine |\n|
  location: account.location
//@[2:10) Identifier |location|
//@[10:11) Colon |:|
//@[12:19) Identifier |account|
//@[19:20) Dot |.|
//@[20:28) Identifier |location|
//@[28:29) NewLine |\n|
  kind: 'StorageV2'
//@[2:6) Identifier |kind|
//@[6:7) Colon |:|
//@[8:19) StringComplete |'StorageV2'|
//@[19:20) NewLine |\n|
  sku: {
//@[2:5) Identifier |sku|
//@[5:6) Colon |:|
//@[7:8) LeftBrace |{|
//@[8:9) NewLine |\n|
    name: 'Standard_LRS'
//@[4:8) Identifier |name|
//@[8:9) Colon |:|
//@[10:24) StringComplete |'Standard_LRS'|
//@[24:25) NewLine |\n|
  }
//@[2:3) RightBrace |}|
//@[3:4) NewLine |\n|
  dependsOn: [
//@[2:11) Identifier |dependsOn|
//@[11:12) Colon |:|
//@[13:14) LeftSquare |[|
//@[14:15) NewLine |\n|
    storageAccounts
//@[4:19) Identifier |storageAccounts|
//@[19:20) NewLine |\n|
  ]
//@[2:3) RightSquare |]|
//@[3:4) NewLine |\n|
}]
//@[0:1) RightBrace |}|
//@[1:2) RightSquare |]|
//@[2:4) NewLine |\n\n|

// one-to-one paired dependencies
//@[33:34) NewLine |\n|
resource firstSet 'Microsoft.Storage/storageAccounts@2019-06-01' = [for i in range(0, length(accounts)): {
//@[0:8) Identifier |resource|
//@[9:17) Identifier |firstSet|
//@[18:64) StringComplete |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[65:66) Assignment |=|
//@[67:68) LeftSquare |[|
//@[68:71) Identifier |for|
//@[72:73) Identifier |i|
//@[74:76) Identifier |in|
//@[77:82) Identifier |range|
//@[82:83) LeftParen |(|
//@[83:84) Integer |0|
//@[84:85) Comma |,|
//@[86:92) Identifier |length|
//@[92:93) LeftParen |(|
//@[93:101) Identifier |accounts|
//@[101:102) RightParen |)|
//@[102:103) RightParen |)|
//@[103:104) Colon |:|
//@[105:106) LeftBrace |{|
//@[106:107) NewLine |\n|
  name: '${name}-set1-${i}'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:11) StringLeftPiece |'${|
//@[11:15) Identifier |name|
//@[15:24) StringMiddlePiece |}-set1-${|
//@[24:25) Identifier |i|
//@[25:27) StringRightPiece |}'|
//@[27:28) NewLine |\n|
  location: resourceGroup().location
//@[2:10) Identifier |location|
//@[10:11) Colon |:|
//@[12:25) Identifier |resourceGroup|
//@[25:26) LeftParen |(|
//@[26:27) RightParen |)|
//@[27:28) Dot |.|
//@[28:36) Identifier |location|
//@[36:37) NewLine |\n|
  kind: 'StorageV2'
//@[2:6) Identifier |kind|
//@[6:7) Colon |:|
//@[8:19) StringComplete |'StorageV2'|
//@[19:20) NewLine |\n|
  sku: {
//@[2:5) Identifier |sku|
//@[5:6) Colon |:|
//@[7:8) LeftBrace |{|
//@[8:9) NewLine |\n|
    name: 'Standard_LRS'
//@[4:8) Identifier |name|
//@[8:9) Colon |:|
//@[10:24) StringComplete |'Standard_LRS'|
//@[24:25) NewLine |\n|
  }
//@[2:3) RightBrace |}|
//@[3:4) NewLine |\n|
}]
//@[0:1) RightBrace |}|
//@[1:2) RightSquare |]|
//@[2:4) NewLine |\n\n|

resource secondSet 'Microsoft.Storage/storageAccounts@2019-06-01' = [for i in range(0, length(accounts)): {
//@[0:8) Identifier |resource|
//@[9:18) Identifier |secondSet|
//@[19:65) StringComplete |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[66:67) Assignment |=|
//@[68:69) LeftSquare |[|
//@[69:72) Identifier |for|
//@[73:74) Identifier |i|
//@[75:77) Identifier |in|
//@[78:83) Identifier |range|
//@[83:84) LeftParen |(|
//@[84:85) Integer |0|
//@[85:86) Comma |,|
//@[87:93) Identifier |length|
//@[93:94) LeftParen |(|
//@[94:102) Identifier |accounts|
//@[102:103) RightParen |)|
//@[103:104) RightParen |)|
//@[104:105) Colon |:|
//@[106:107) LeftBrace |{|
//@[107:108) NewLine |\n|
  name: '${name}-set2-${i}'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:11) StringLeftPiece |'${|
//@[11:15) Identifier |name|
//@[15:24) StringMiddlePiece |}-set2-${|
//@[24:25) Identifier |i|
//@[25:27) StringRightPiece |}'|
//@[27:28) NewLine |\n|
  location: resourceGroup().location
//@[2:10) Identifier |location|
//@[10:11) Colon |:|
//@[12:25) Identifier |resourceGroup|
//@[25:26) LeftParen |(|
//@[26:27) RightParen |)|
//@[27:28) Dot |.|
//@[28:36) Identifier |location|
//@[36:37) NewLine |\n|
  kind: 'StorageV2'
//@[2:6) Identifier |kind|
//@[6:7) Colon |:|
//@[8:19) StringComplete |'StorageV2'|
//@[19:20) NewLine |\n|
  sku: {
//@[2:5) Identifier |sku|
//@[5:6) Colon |:|
//@[7:8) LeftBrace |{|
//@[8:9) NewLine |\n|
    name: 'Standard_LRS'
//@[4:8) Identifier |name|
//@[8:9) Colon |:|
//@[10:24) StringComplete |'Standard_LRS'|
//@[24:25) NewLine |\n|
  }
//@[2:3) RightBrace |}|
//@[3:4) NewLine |\n|
  dependsOn: [
//@[2:11) Identifier |dependsOn|
//@[11:12) Colon |:|
//@[13:14) LeftSquare |[|
//@[14:15) NewLine |\n|
    firstSet[i]
//@[4:12) Identifier |firstSet|
//@[12:13) LeftSquare |[|
//@[13:14) Identifier |i|
//@[14:15) RightSquare |]|
//@[15:16) NewLine |\n|
  ]
//@[2:3) RightSquare |]|
//@[3:4) NewLine |\n|
}]
//@[0:1) RightBrace |}|
//@[1:2) RightSquare |]|
//@[2:4) NewLine |\n\n|

// depending on collection and one resource in the collection optimizes the latter part away
//@[92:93) NewLine |\n|
resource anotherSingleResource 'Microsoft.Storage/storageAccounts@2019-06-01' = {
//@[0:8) Identifier |resource|
//@[9:30) Identifier |anotherSingleResource|
//@[31:77) StringComplete |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[78:79) Assignment |=|
//@[80:81) LeftBrace |{|
//@[81:82) NewLine |\n|
  name: '${name}single-resource-name'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:11) StringLeftPiece |'${|
//@[11:15) Identifier |name|
//@[15:37) StringRightPiece |}single-resource-name'|
//@[37:38) NewLine |\n|
  location: resourceGroup().location
//@[2:10) Identifier |location|
//@[10:11) Colon |:|
//@[12:25) Identifier |resourceGroup|
//@[25:26) LeftParen |(|
//@[26:27) RightParen |)|
//@[27:28) Dot |.|
//@[28:36) Identifier |location|
//@[36:37) NewLine |\n|
  kind: 'StorageV2'
//@[2:6) Identifier |kind|
//@[6:7) Colon |:|
//@[8:19) StringComplete |'StorageV2'|
//@[19:20) NewLine |\n|
  sku: {
//@[2:5) Identifier |sku|
//@[5:6) Colon |:|
//@[7:8) LeftBrace |{|
//@[8:9) NewLine |\n|
    name: 'Standard_LRS'
//@[4:8) Identifier |name|
//@[8:9) Colon |:|
//@[10:24) StringComplete |'Standard_LRS'|
//@[24:25) NewLine |\n|
  }
//@[2:3) RightBrace |}|
//@[3:4) NewLine |\n|
  dependsOn: [
//@[2:11) Identifier |dependsOn|
//@[11:12) Colon |:|
//@[13:14) LeftSquare |[|
//@[14:15) NewLine |\n|
    secondSet
//@[4:13) Identifier |secondSet|
//@[13:14) NewLine |\n|
    secondSet[0]
//@[4:13) Identifier |secondSet|
//@[13:14) LeftSquare |[|
//@[14:15) Integer |0|
//@[15:16) RightSquare |]|
//@[16:17) NewLine |\n|
  ]
//@[2:3) RightSquare |]|
//@[3:4) NewLine |\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

// vnets
//@[8:9) NewLine |\n|
var vnetConfigurations = [
//@[0:3) Identifier |var|
//@[4:22) Identifier |vnetConfigurations|
//@[23:24) Assignment |=|
//@[25:26) LeftSquare |[|
//@[26:27) NewLine |\n|
  {
//@[2:3) LeftBrace |{|
//@[3:4) NewLine |\n|
    name: 'one'
//@[4:8) Identifier |name|
//@[8:9) Colon |:|
//@[10:15) StringComplete |'one'|
//@[15:16) NewLine |\n|
    location: resourceGroup().location
//@[4:12) Identifier |location|
//@[12:13) Colon |:|
//@[14:27) Identifier |resourceGroup|
//@[27:28) LeftParen |(|
//@[28:29) RightParen |)|
//@[29:30) Dot |.|
//@[30:38) Identifier |location|
//@[38:39) NewLine |\n|
  }
//@[2:3) RightBrace |}|
//@[3:4) NewLine |\n|
  {
//@[2:3) LeftBrace |{|
//@[3:4) NewLine |\n|
    name: 'two'
//@[4:8) Identifier |name|
//@[8:9) Colon |:|
//@[10:15) StringComplete |'two'|
//@[15:16) NewLine |\n|
    location: 'westus'
//@[4:12) Identifier |location|
//@[12:13) Colon |:|
//@[14:22) StringComplete |'westus'|
//@[22:23) NewLine |\n|
  }
//@[2:3) RightBrace |}|
//@[3:4) NewLine |\n|
]
//@[0:1) RightSquare |]|
//@[1:3) NewLine |\n\n|

resource vnets 'Microsoft.Network/virtualNetworks@2020-06-01' = [for vnetConfig in vnetConfigurations: {
//@[0:8) Identifier |resource|
//@[9:14) Identifier |vnets|
//@[15:61) StringComplete |'Microsoft.Network/virtualNetworks@2020-06-01'|
//@[62:63) Assignment |=|
//@[64:65) LeftSquare |[|
//@[65:68) Identifier |for|
//@[69:79) Identifier |vnetConfig|
//@[80:82) Identifier |in|
//@[83:101) Identifier |vnetConfigurations|
//@[101:102) Colon |:|
//@[103:104) LeftBrace |{|
//@[104:105) NewLine |\n|
  name: vnetConfig.name
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:18) Identifier |vnetConfig|
//@[18:19) Dot |.|
//@[19:23) Identifier |name|
//@[23:24) NewLine |\n|
  location: vnetConfig.location
//@[2:10) Identifier |location|
//@[10:11) Colon |:|
//@[12:22) Identifier |vnetConfig|
//@[22:23) Dot |.|
//@[23:31) Identifier |location|
//@[31:32) NewLine |\n|
}]
//@[0:1) RightBrace |}|
//@[1:2) RightSquare |]|
//@[2:4) NewLine |\n\n|

// implicit dependency on single resource from a resource collection
//@[68:69) NewLine |\n|
resource implicitDependencyOnSingleResourceByIndex 'Microsoft.Network/dnsZones@2018-05-01' = {
//@[0:8) Identifier |resource|
//@[9:50) Identifier |implicitDependencyOnSingleResourceByIndex|
//@[51:90) StringComplete |'Microsoft.Network/dnsZones@2018-05-01'|
//@[91:92) Assignment |=|
//@[93:94) LeftBrace |{|
//@[94:95) NewLine |\n|
  name: 'test'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:14) StringComplete |'test'|
//@[14:15) NewLine |\n|
  location: 'global'
//@[2:10) Identifier |location|
//@[10:11) Colon |:|
//@[12:20) StringComplete |'global'|
//@[20:21) NewLine |\n|
  properties: {
//@[2:12) Identifier |properties|
//@[12:13) Colon |:|
//@[14:15) LeftBrace |{|
//@[15:16) NewLine |\n|
    resolutionVirtualNetworks: [
//@[4:29) Identifier |resolutionVirtualNetworks|
//@[29:30) Colon |:|
//@[31:32) LeftSquare |[|
//@[32:33) NewLine |\n|
      {
//@[6:7) LeftBrace |{|
//@[7:8) NewLine |\n|
        id: vnets[index+1].id
//@[8:10) Identifier |id|
//@[10:11) Colon |:|
//@[12:17) Identifier |vnets|
//@[17:18) LeftSquare |[|
//@[18:23) Identifier |index|
//@[23:24) Plus |+|
//@[24:25) Integer |1|
//@[25:26) RightSquare |]|
//@[26:27) Dot |.|
//@[27:29) Identifier |id|
//@[29:30) NewLine |\n|
      }
//@[6:7) RightBrace |}|
//@[7:8) NewLine |\n|
    ]
//@[4:5) RightSquare |]|
//@[5:6) NewLine |\n|
  }
//@[2:3) RightBrace |}|
//@[3:4) NewLine |\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

// implicit and explicit dependency combined
//@[44:45) NewLine |\n|
resource combinedDependencies 'Microsoft.Network/dnsZones@2018-05-01' = {
//@[0:8) Identifier |resource|
//@[9:29) Identifier |combinedDependencies|
//@[30:69) StringComplete |'Microsoft.Network/dnsZones@2018-05-01'|
//@[70:71) Assignment |=|
//@[72:73) LeftBrace |{|
//@[73:74) NewLine |\n|
  name: 'test2'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:15) StringComplete |'test2'|
//@[15:16) NewLine |\n|
  location: 'global'
//@[2:10) Identifier |location|
//@[10:11) Colon |:|
//@[12:20) StringComplete |'global'|
//@[20:21) NewLine |\n|
  properties: {
//@[2:12) Identifier |properties|
//@[12:13) Colon |:|
//@[14:15) LeftBrace |{|
//@[15:16) NewLine |\n|
    resolutionVirtualNetworks: [
//@[4:29) Identifier |resolutionVirtualNetworks|
//@[29:30) Colon |:|
//@[31:32) LeftSquare |[|
//@[32:33) NewLine |\n|
      {
//@[6:7) LeftBrace |{|
//@[7:8) NewLine |\n|
        id: vnets[index-1].id
//@[8:10) Identifier |id|
//@[10:11) Colon |:|
//@[12:17) Identifier |vnets|
//@[17:18) LeftSquare |[|
//@[18:23) Identifier |index|
//@[23:24) Minus |-|
//@[24:25) Integer |1|
//@[25:26) RightSquare |]|
//@[26:27) Dot |.|
//@[27:29) Identifier |id|
//@[29:30) NewLine |\n|
      }
//@[6:7) RightBrace |}|
//@[7:8) NewLine |\n|
      {
//@[6:7) LeftBrace |{|
//@[7:8) NewLine |\n|
        id: vnets[index * 2].id
//@[8:10) Identifier |id|
//@[10:11) Colon |:|
//@[12:17) Identifier |vnets|
//@[17:18) LeftSquare |[|
//@[18:23) Identifier |index|
//@[24:25) Asterisk |*|
//@[26:27) Integer |2|
//@[27:28) RightSquare |]|
//@[28:29) Dot |.|
//@[29:31) Identifier |id|
//@[31:32) NewLine |\n|
      }
//@[6:7) RightBrace |}|
//@[7:8) NewLine |\n|
    ]
//@[4:5) RightSquare |]|
//@[5:6) NewLine |\n|
  }
//@[2:3) RightBrace |}|
//@[3:4) NewLine |\n|
  dependsOn: [
//@[2:11) Identifier |dependsOn|
//@[11:12) Colon |:|
//@[13:14) LeftSquare |[|
//@[14:15) NewLine |\n|
    vnets
//@[4:9) Identifier |vnets|
//@[9:10) NewLine |\n|
  ]
//@[2:3) RightSquare |]|
//@[3:4) NewLine |\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

// single module
//@[16:17) NewLine |\n|
module singleModule 'passthrough.bicep' = {
//@[0:6) Identifier |module|
//@[7:19) Identifier |singleModule|
//@[20:39) StringComplete |'passthrough.bicep'|
//@[40:41) Assignment |=|
//@[42:43) LeftBrace |{|
//@[43:44) NewLine |\n|
  name: 'test'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:14) StringComplete |'test'|
//@[14:15) NewLine |\n|
  params: {
//@[2:8) Identifier |params|
//@[8:9) Colon |:|
//@[10:11) LeftBrace |{|
//@[11:12) NewLine |\n|
    myInput: 'hello'
//@[4:11) Identifier |myInput|
//@[11:12) Colon |:|
//@[13:20) StringComplete |'hello'|
//@[20:21) NewLine |\n|
  }
//@[2:3) RightBrace |}|
//@[3:4) NewLine |\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

var moduleSetup = [
//@[0:3) Identifier |var|
//@[4:15) Identifier |moduleSetup|
//@[16:17) Assignment |=|
//@[18:19) LeftSquare |[|
//@[19:20) NewLine |\n|
  'one'
//@[2:7) StringComplete |'one'|
//@[7:8) NewLine |\n|
  'two'
//@[2:7) StringComplete |'two'|
//@[7:8) NewLine |\n|
  'three'
//@[2:9) StringComplete |'three'|
//@[9:10) NewLine |\n|
]
//@[0:1) RightSquare |]|
//@[1:3) NewLine |\n\n|

// module collection plus explicit dependency on single module
//@[62:63) NewLine |\n|
@sys.batchSize(3)
//@[0:1) At |@|
//@[1:4) Identifier |sys|
//@[4:5) Dot |.|
//@[5:14) Identifier |batchSize|
//@[14:15) LeftParen |(|
//@[15:16) Integer |3|
//@[16:17) RightParen |)|
//@[17:18) NewLine |\n|
module moduleCollectionWithSingleDependency 'passthrough.bicep' = [for moduleName in moduleSetup: {
//@[0:6) Identifier |module|
//@[7:43) Identifier |moduleCollectionWithSingleDependency|
//@[44:63) StringComplete |'passthrough.bicep'|
//@[64:65) Assignment |=|
//@[66:67) LeftSquare |[|
//@[67:70) Identifier |for|
//@[71:81) Identifier |moduleName|
//@[82:84) Identifier |in|
//@[85:96) Identifier |moduleSetup|
//@[96:97) Colon |:|
//@[98:99) LeftBrace |{|
//@[99:100) NewLine |\n|
  name: moduleName
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:18) Identifier |moduleName|
//@[18:19) NewLine |\n|
  params: {
//@[2:8) Identifier |params|
//@[8:9) Colon |:|
//@[10:11) LeftBrace |{|
//@[11:12) NewLine |\n|
    myInput: 'in-${moduleName}'
//@[4:11) Identifier |myInput|
//@[11:12) Colon |:|
//@[13:19) StringLeftPiece |'in-${|
//@[19:29) Identifier |moduleName|
//@[29:31) StringRightPiece |}'|
//@[31:32) NewLine |\n|
  }
//@[2:3) RightBrace |}|
//@[3:4) NewLine |\n|
  dependsOn: [
//@[2:11) Identifier |dependsOn|
//@[11:12) Colon |:|
//@[13:14) LeftSquare |[|
//@[14:15) NewLine |\n|
    singleModule
//@[4:16) Identifier |singleModule|
//@[16:17) NewLine |\n|
    singleResource
//@[4:18) Identifier |singleResource|
//@[18:19) NewLine |\n|
  ]
//@[2:3) RightSquare |]|
//@[3:4) NewLine |\n|
}]
//@[0:1) RightBrace |}|
//@[1:2) RightSquare |]|
//@[2:4) NewLine |\n\n|

// another module collection with dependency on another module collection
//@[73:74) NewLine |\n|
module moduleCollectionWithCollectionDependencies 'passthrough.bicep' = [for moduleName in moduleSetup: {
//@[0:6) Identifier |module|
//@[7:49) Identifier |moduleCollectionWithCollectionDependencies|
//@[50:69) StringComplete |'passthrough.bicep'|
//@[70:71) Assignment |=|
//@[72:73) LeftSquare |[|
//@[73:76) Identifier |for|
//@[77:87) Identifier |moduleName|
//@[88:90) Identifier |in|
//@[91:102) Identifier |moduleSetup|
//@[102:103) Colon |:|
//@[104:105) LeftBrace |{|
//@[105:106) NewLine |\n|
  name: moduleName
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:18) Identifier |moduleName|
//@[18:19) NewLine |\n|
  params: {
//@[2:8) Identifier |params|
//@[8:9) Colon |:|
//@[10:11) LeftBrace |{|
//@[11:12) NewLine |\n|
    myInput: 'in-${moduleName}'
//@[4:11) Identifier |myInput|
//@[11:12) Colon |:|
//@[13:19) StringLeftPiece |'in-${|
//@[19:29) Identifier |moduleName|
//@[29:31) StringRightPiece |}'|
//@[31:32) NewLine |\n|
  }
//@[2:3) RightBrace |}|
//@[3:4) NewLine |\n|
  dependsOn: [
//@[2:11) Identifier |dependsOn|
//@[11:12) Colon |:|
//@[13:14) LeftSquare |[|
//@[14:15) NewLine |\n|
    storageAccounts
//@[4:19) Identifier |storageAccounts|
//@[19:20) NewLine |\n|
    moduleCollectionWithSingleDependency
//@[4:40) Identifier |moduleCollectionWithSingleDependency|
//@[40:41) NewLine |\n|
  ]
//@[2:3) RightSquare |]|
//@[3:4) NewLine |\n|
}]
//@[0:1) RightBrace |}|
//@[1:2) RightSquare |]|
//@[2:4) NewLine |\n\n|

module singleModuleWithIndexedDependencies 'passthrough.bicep' = {
//@[0:6) Identifier |module|
//@[7:42) Identifier |singleModuleWithIndexedDependencies|
//@[43:62) StringComplete |'passthrough.bicep'|
//@[63:64) Assignment |=|
//@[65:66) LeftBrace |{|
//@[66:67) NewLine |\n|
  name: 'hello'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:15) StringComplete |'hello'|
//@[15:16) NewLine |\n|
  params: {
//@[2:8) Identifier |params|
//@[8:9) Colon |:|
//@[10:11) LeftBrace |{|
//@[11:12) NewLine |\n|
    myInput: concat(moduleCollectionWithCollectionDependencies[index].outputs.myOutput, storageAccounts[index * 3].properties.accessTier)
//@[4:11) Identifier |myInput|
//@[11:12) Colon |:|
//@[13:19) Identifier |concat|
//@[19:20) LeftParen |(|
//@[20:62) Identifier |moduleCollectionWithCollectionDependencies|
//@[62:63) LeftSquare |[|
//@[63:68) Identifier |index|
//@[68:69) RightSquare |]|
//@[69:70) Dot |.|
//@[70:77) Identifier |outputs|
//@[77:78) Dot |.|
//@[78:86) Identifier |myOutput|
//@[86:87) Comma |,|
//@[88:103) Identifier |storageAccounts|
//@[103:104) LeftSquare |[|
//@[104:109) Identifier |index|
//@[110:111) Asterisk |*|
//@[112:113) Integer |3|
//@[113:114) RightSquare |]|
//@[114:115) Dot |.|
//@[115:125) Identifier |properties|
//@[125:126) Dot |.|
//@[126:136) Identifier |accessTier|
//@[136:137) RightParen |)|
//@[137:138) NewLine |\n|
  }
//@[2:3) RightBrace |}|
//@[3:4) NewLine |\n|
  dependsOn: [
//@[2:11) Identifier |dependsOn|
//@[11:12) Colon |:|
//@[13:14) LeftSquare |[|
//@[14:15) NewLine |\n|
    storageAccounts2[index - 10]
//@[4:20) Identifier |storageAccounts2|
//@[20:21) LeftSquare |[|
//@[21:26) Identifier |index|
//@[27:28) Minus |-|
//@[29:31) Integer |10|
//@[31:32) RightSquare |]|
//@[32:33) NewLine |\n|
  ]
//@[2:3) RightSquare |]|
//@[3:4) NewLine |\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

module moduleCollectionWithIndexedDependencies 'passthrough.bicep' = [for moduleName in moduleSetup: {
//@[0:6) Identifier |module|
//@[7:46) Identifier |moduleCollectionWithIndexedDependencies|
//@[47:66) StringComplete |'passthrough.bicep'|
//@[67:68) Assignment |=|
//@[69:70) LeftSquare |[|
//@[70:73) Identifier |for|
//@[74:84) Identifier |moduleName|
//@[85:87) Identifier |in|
//@[88:99) Identifier |moduleSetup|
//@[99:100) Colon |:|
//@[101:102) LeftBrace |{|
//@[102:103) NewLine |\n|
  name: moduleName
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:18) Identifier |moduleName|
//@[18:19) NewLine |\n|
  params: {
//@[2:8) Identifier |params|
//@[8:9) Colon |:|
//@[10:11) LeftBrace |{|
//@[11:12) NewLine |\n|
    myInput: '${moduleCollectionWithCollectionDependencies[index].outputs.myOutput} - ${storageAccounts[index * 3].properties.accessTier} - ${moduleName}'
//@[4:11) Identifier |myInput|
//@[11:12) Colon |:|
//@[13:16) StringLeftPiece |'${|
//@[16:58) Identifier |moduleCollectionWithCollectionDependencies|
//@[58:59) LeftSquare |[|
//@[59:64) Identifier |index|
//@[64:65) RightSquare |]|
//@[65:66) Dot |.|
//@[66:73) Identifier |outputs|
//@[73:74) Dot |.|
//@[74:82) Identifier |myOutput|
//@[82:88) StringMiddlePiece |} - ${|
//@[88:103) Identifier |storageAccounts|
//@[103:104) LeftSquare |[|
//@[104:109) Identifier |index|
//@[110:111) Asterisk |*|
//@[112:113) Integer |3|
//@[113:114) RightSquare |]|
//@[114:115) Dot |.|
//@[115:125) Identifier |properties|
//@[125:126) Dot |.|
//@[126:136) Identifier |accessTier|
//@[136:142) StringMiddlePiece |} - ${|
//@[142:152) Identifier |moduleName|
//@[152:154) StringRightPiece |}'|
//@[154:155) NewLine |\n|
  }
//@[2:3) RightBrace |}|
//@[3:4) NewLine |\n|
  dependsOn: [
//@[2:11) Identifier |dependsOn|
//@[11:12) Colon |:|
//@[13:14) LeftSquare |[|
//@[14:15) NewLine |\n|
    storageAccounts2[index - 9]
//@[4:20) Identifier |storageAccounts2|
//@[20:21) LeftSquare |[|
//@[21:26) Identifier |index|
//@[27:28) Minus |-|
//@[29:30) Integer |9|
//@[30:31) RightSquare |]|
//@[31:32) NewLine |\n|
  ]
//@[2:3) RightSquare |]|
//@[3:4) NewLine |\n|
}]
//@[0:1) RightBrace |}|
//@[1:2) RightSquare |]|
//@[2:4) NewLine |\n\n|

output indexedModulesName string = moduleCollectionWithSingleDependency[index].name
//@[0:6) Identifier |output|
//@[7:25) Identifier |indexedModulesName|
//@[26:32) Identifier |string|
//@[33:34) Assignment |=|
//@[35:71) Identifier |moduleCollectionWithSingleDependency|
//@[71:72) LeftSquare |[|
//@[72:77) Identifier |index|
//@[77:78) RightSquare |]|
//@[78:79) Dot |.|
//@[79:83) Identifier |name|
//@[83:84) NewLine |\n|
output indexedModuleOutput string = moduleCollectionWithSingleDependency[index * 1].outputs.myOutput
//@[0:6) Identifier |output|
//@[7:26) Identifier |indexedModuleOutput|
//@[27:33) Identifier |string|
//@[34:35) Assignment |=|
//@[36:72) Identifier |moduleCollectionWithSingleDependency|
//@[72:73) LeftSquare |[|
//@[73:78) Identifier |index|
//@[79:80) Asterisk |*|
//@[81:82) Integer |1|
//@[82:83) RightSquare |]|
//@[83:84) Dot |.|
//@[84:91) Identifier |outputs|
//@[91:92) Dot |.|
//@[92:100) Identifier |myOutput|
//@[100:102) NewLine |\n\n|

// resource collection
//@[22:23) NewLine |\n|
resource existingStorageAccounts 'Microsoft.Storage/storageAccounts@2019-06-01' existing = [for account in accounts: {
//@[0:8) Identifier |resource|
//@[9:32) Identifier |existingStorageAccounts|
//@[33:79) StringComplete |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[80:88) Identifier |existing|
//@[89:90) Assignment |=|
//@[91:92) LeftSquare |[|
//@[92:95) Identifier |for|
//@[96:103) Identifier |account|
//@[104:106) Identifier |in|
//@[107:115) Identifier |accounts|
//@[115:116) Colon |:|
//@[117:118) LeftBrace |{|
//@[118:119) NewLine |\n|
  name: '${name}-existing-${account.name}'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:11) StringLeftPiece |'${|
//@[11:15) Identifier |name|
//@[15:28) StringMiddlePiece |}-existing-${|
//@[28:35) Identifier |account|
//@[35:36) Dot |.|
//@[36:40) Identifier |name|
//@[40:42) StringRightPiece |}'|
//@[42:43) NewLine |\n|
}]
//@[0:1) RightBrace |}|
//@[1:2) RightSquare |]|
//@[2:4) NewLine |\n\n|

output existingIndexedResourceName string = existingStorageAccounts[index * 0].name
//@[0:6) Identifier |output|
//@[7:34) Identifier |existingIndexedResourceName|
//@[35:41) Identifier |string|
//@[42:43) Assignment |=|
//@[44:67) Identifier |existingStorageAccounts|
//@[67:68) LeftSquare |[|
//@[68:73) Identifier |index|
//@[74:75) Asterisk |*|
//@[76:77) Integer |0|
//@[77:78) RightSquare |]|
//@[78:79) Dot |.|
//@[79:83) Identifier |name|
//@[83:84) NewLine |\n|
output existingIndexedResourceId string = existingStorageAccounts[index * 1].id
//@[0:6) Identifier |output|
//@[7:32) Identifier |existingIndexedResourceId|
//@[33:39) Identifier |string|
//@[40:41) Assignment |=|
//@[42:65) Identifier |existingStorageAccounts|
//@[65:66) LeftSquare |[|
//@[66:71) Identifier |index|
//@[72:73) Asterisk |*|
//@[74:75) Integer |1|
//@[75:76) RightSquare |]|
//@[76:77) Dot |.|
//@[77:79) Identifier |id|
//@[79:80) NewLine |\n|
output existingIndexedResourceType string = existingStorageAccounts[index+2].type
//@[0:6) Identifier |output|
//@[7:34) Identifier |existingIndexedResourceType|
//@[35:41) Identifier |string|
//@[42:43) Assignment |=|
//@[44:67) Identifier |existingStorageAccounts|
//@[67:68) LeftSquare |[|
//@[68:73) Identifier |index|
//@[73:74) Plus |+|
//@[74:75) Integer |2|
//@[75:76) RightSquare |]|
//@[76:77) Dot |.|
//@[77:81) Identifier |type|
//@[81:82) NewLine |\n|
output existingIndexedResourceApiVersion string = existingStorageAccounts[index-7].apiVersion
//@[0:6) Identifier |output|
//@[7:40) Identifier |existingIndexedResourceApiVersion|
//@[41:47) Identifier |string|
//@[48:49) Assignment |=|
//@[50:73) Identifier |existingStorageAccounts|
//@[73:74) LeftSquare |[|
//@[74:79) Identifier |index|
//@[79:80) Minus |-|
//@[80:81) Integer |7|
//@[81:82) RightSquare |]|
//@[82:83) Dot |.|
//@[83:93) Identifier |apiVersion|
//@[93:94) NewLine |\n|
output existingIndexedResourceLocation string = existingStorageAccounts[index/2].location
//@[0:6) Identifier |output|
//@[7:38) Identifier |existingIndexedResourceLocation|
//@[39:45) Identifier |string|
//@[46:47) Assignment |=|
//@[48:71) Identifier |existingStorageAccounts|
//@[71:72) LeftSquare |[|
//@[72:77) Identifier |index|
//@[77:78) Slash |/|
//@[78:79) Integer |2|
//@[79:80) RightSquare |]|
//@[80:81) Dot |.|
//@[81:89) Identifier |location|
//@[89:90) NewLine |\n|
output existingIndexedResourceAccessTier string = existingStorageAccounts[index%3].properties.accessTier
//@[0:6) Identifier |output|
//@[7:40) Identifier |existingIndexedResourceAccessTier|
//@[41:47) Identifier |string|
//@[48:49) Assignment |=|
//@[50:73) Identifier |existingStorageAccounts|
//@[73:74) LeftSquare |[|
//@[74:79) Identifier |index|
//@[79:80) Modulo |%|
//@[80:81) Integer |3|
//@[81:82) RightSquare |]|
//@[82:83) Dot |.|
//@[83:93) Identifier |properties|
//@[93:94) Dot |.|
//@[94:104) Identifier |accessTier|
//@[104:106) NewLine |\n\n|

resource duplicatedNames 'Microsoft.Network/dnsZones@2018-05-01' = [for zone in []: {
//@[0:8) Identifier |resource|
//@[9:24) Identifier |duplicatedNames|
//@[25:64) StringComplete |'Microsoft.Network/dnsZones@2018-05-01'|
//@[65:66) Assignment |=|
//@[67:68) LeftSquare |[|
//@[68:71) Identifier |for|
//@[72:76) Identifier |zone|
//@[77:79) Identifier |in|
//@[80:81) LeftSquare |[|
//@[81:82) RightSquare |]|
//@[82:83) Colon |:|
//@[84:85) LeftBrace |{|
//@[85:86) NewLine |\n|
  name: 'no loop variable'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:26) StringComplete |'no loop variable'|
//@[26:27) NewLine |\n|
  location: 'eastus'
//@[2:10) Identifier |location|
//@[10:11) Colon |:|
//@[12:20) StringComplete |'eastus'|
//@[20:21) NewLine |\n|
}]
//@[0:1) RightBrace |}|
//@[1:2) RightSquare |]|
//@[2:4) NewLine |\n\n|

// reference to a resource collection whose name expression does not reference any loop variables
//@[97:98) NewLine |\n|
resource referenceToDuplicateNames 'Microsoft.Network/dnsZones@2018-05-01' = [for zone in []: {
//@[0:8) Identifier |resource|
//@[9:34) Identifier |referenceToDuplicateNames|
//@[35:74) StringComplete |'Microsoft.Network/dnsZones@2018-05-01'|
//@[75:76) Assignment |=|
//@[77:78) LeftSquare |[|
//@[78:81) Identifier |for|
//@[82:86) Identifier |zone|
//@[87:89) Identifier |in|
//@[90:91) LeftSquare |[|
//@[91:92) RightSquare |]|
//@[92:93) Colon |:|
//@[94:95) LeftBrace |{|
//@[95:96) NewLine |\n|
  name: 'no loop variable 2'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:28) StringComplete |'no loop variable 2'|
//@[28:29) NewLine |\n|
  location: 'eastus'
//@[2:10) Identifier |location|
//@[10:11) Colon |:|
//@[12:20) StringComplete |'eastus'|
//@[20:21) NewLine |\n|
  dependsOn: [
//@[2:11) Identifier |dependsOn|
//@[11:12) Colon |:|
//@[13:14) LeftSquare |[|
//@[14:15) NewLine |\n|
    duplicatedNames[index]
//@[4:19) Identifier |duplicatedNames|
//@[19:20) LeftSquare |[|
//@[20:25) Identifier |index|
//@[25:26) RightSquare |]|
//@[26:27) NewLine |\n|
  ]
//@[2:3) RightSquare |]|
//@[3:4) NewLine |\n|
}]
//@[0:1) RightBrace |}|
//@[1:2) RightSquare |]|
//@[2:4) NewLine |\n\n|

var regions = [
//@[0:3) Identifier |var|
//@[4:11) Identifier |regions|
//@[12:13) Assignment |=|
//@[14:15) LeftSquare |[|
//@[15:16) NewLine |\n|
  'eastus'
//@[2:10) StringComplete |'eastus'|
//@[10:11) NewLine |\n|
  'westus'
//@[2:10) StringComplete |'westus'|
//@[10:11) NewLine |\n|
]
//@[0:1) RightSquare |]|
//@[1:3) NewLine |\n\n|

module apim 'passthrough.bicep' = [for region in regions: {
//@[0:6) Identifier |module|
//@[7:11) Identifier |apim|
//@[12:31) StringComplete |'passthrough.bicep'|
//@[32:33) Assignment |=|
//@[34:35) LeftSquare |[|
//@[35:38) Identifier |for|
//@[39:45) Identifier |region|
//@[46:48) Identifier |in|
//@[49:56) Identifier |regions|
//@[56:57) Colon |:|
//@[58:59) LeftBrace |{|
//@[59:60) NewLine |\n|
  name: 'apim-${region}-${name}'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:16) StringLeftPiece |'apim-${|
//@[16:22) Identifier |region|
//@[22:26) StringMiddlePiece |}-${|
//@[26:30) Identifier |name|
//@[30:32) StringRightPiece |}'|
//@[32:33) NewLine |\n|
  params: {
//@[2:8) Identifier |params|
//@[8:9) Colon |:|
//@[10:11) LeftBrace |{|
//@[11:12) NewLine |\n|
    myInput: region
//@[4:11) Identifier |myInput|
//@[11:12) Colon |:|
//@[13:19) Identifier |region|
//@[19:20) NewLine |\n|
  }
//@[2:3) RightBrace |}|
//@[3:4) NewLine |\n|
}]
//@[0:1) RightBrace |}|
//@[1:2) RightSquare |]|
//@[2:4) NewLine |\n\n|

resource propertyLoopDependencyOnModuleCollection 'Microsoft.Network/frontDoors@2020-05-01' = {
//@[0:8) Identifier |resource|
//@[9:49) Identifier |propertyLoopDependencyOnModuleCollection|
//@[50:91) StringComplete |'Microsoft.Network/frontDoors@2020-05-01'|
//@[92:93) Assignment |=|
//@[94:95) LeftBrace |{|
//@[95:96) NewLine |\n|
  name: name
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:12) Identifier |name|
//@[12:13) NewLine |\n|
  location: 'Global'
//@[2:10) Identifier |location|
//@[10:11) Colon |:|
//@[12:20) StringComplete |'Global'|
//@[20:21) NewLine |\n|
  properties: {
//@[2:12) Identifier |properties|
//@[12:13) Colon |:|
//@[14:15) LeftBrace |{|
//@[15:16) NewLine |\n|
    backendPools: [
//@[4:16) Identifier |backendPools|
//@[16:17) Colon |:|
//@[18:19) LeftSquare |[|
//@[19:20) NewLine |\n|
      {
//@[6:7) LeftBrace |{|
//@[7:8) NewLine |\n|
        name: 'BackendAPIMs'
//@[8:12) Identifier |name|
//@[12:13) Colon |:|
//@[14:28) StringComplete |'BackendAPIMs'|
//@[28:29) NewLine |\n|
        properties: {
//@[8:18) Identifier |properties|
//@[18:19) Colon |:|
//@[20:21) LeftBrace |{|
//@[21:22) NewLine |\n|
          backends: [for index in range(0, length(regions)): {
//@[10:18) Identifier |backends|
//@[18:19) Colon |:|
//@[20:21) LeftSquare |[|
//@[21:24) Identifier |for|
//@[25:30) Identifier |index|
//@[31:33) Identifier |in|
//@[34:39) Identifier |range|
//@[39:40) LeftParen |(|
//@[40:41) Integer |0|
//@[41:42) Comma |,|
//@[43:49) Identifier |length|
//@[49:50) LeftParen |(|
//@[50:57) Identifier |regions|
//@[57:58) RightParen |)|
//@[58:59) RightParen |)|
//@[59:60) Colon |:|
//@[61:62) LeftBrace |{|
//@[62:63) NewLine |\n|
            // we cannot codegen index correctly because the generated dependsOn property
//@[89:90) NewLine |\n|
            // would be outside of the scope of the property loop
//@[65:66) NewLine |\n|
            // as a result, this will generate a dependency on the entire collection
//@[84:85) NewLine |\n|
            address: apim[index].outputs.myOutput
//@[12:19) Identifier |address|
//@[19:20) Colon |:|
//@[21:25) Identifier |apim|
//@[25:26) LeftSquare |[|
//@[26:31) Identifier |index|
//@[31:32) RightSquare |]|
//@[32:33) Dot |.|
//@[33:40) Identifier |outputs|
//@[40:41) Dot |.|
//@[41:49) Identifier |myOutput|
//@[49:50) NewLine |\n|
            backendHostHeader: apim[index].outputs.myOutput
//@[12:29) Identifier |backendHostHeader|
//@[29:30) Colon |:|
//@[31:35) Identifier |apim|
//@[35:36) LeftSquare |[|
//@[36:41) Identifier |index|
//@[41:42) RightSquare |]|
//@[42:43) Dot |.|
//@[43:50) Identifier |outputs|
//@[50:51) Dot |.|
//@[51:59) Identifier |myOutput|
//@[59:60) NewLine |\n|
            httpPort: 80
//@[12:20) Identifier |httpPort|
//@[20:21) Colon |:|
//@[22:24) Integer |80|
//@[24:25) NewLine |\n|
            httpsPort: 443
//@[12:21) Identifier |httpsPort|
//@[21:22) Colon |:|
//@[23:26) Integer |443|
//@[26:27) NewLine |\n|
            priority: 1
//@[12:20) Identifier |priority|
//@[20:21) Colon |:|
//@[22:23) Integer |1|
//@[23:24) NewLine |\n|
            weight: 50
//@[12:18) Identifier |weight|
//@[18:19) Colon |:|
//@[20:22) Integer |50|
//@[22:23) NewLine |\n|
          }]
//@[10:11) RightBrace |}|
//@[11:12) RightSquare |]|
//@[12:13) NewLine |\n|
        }
//@[8:9) RightBrace |}|
//@[9:10) NewLine |\n|
      }
//@[6:7) RightBrace |}|
//@[7:8) NewLine |\n|
    ]
//@[4:5) RightSquare |]|
//@[5:6) NewLine |\n|
  }
//@[2:3) RightBrace |}|
//@[3:4) NewLine |\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

resource indexedModuleCollectionDependency 'Microsoft.Network/frontDoors@2020-05-01' = [for index in range(0, length(regions)): {
//@[0:8) Identifier |resource|
//@[9:42) Identifier |indexedModuleCollectionDependency|
//@[43:84) StringComplete |'Microsoft.Network/frontDoors@2020-05-01'|
//@[85:86) Assignment |=|
//@[87:88) LeftSquare |[|
//@[88:91) Identifier |for|
//@[92:97) Identifier |index|
//@[98:100) Identifier |in|
//@[101:106) Identifier |range|
//@[106:107) LeftParen |(|
//@[107:108) Integer |0|
//@[108:109) Comma |,|
//@[110:116) Identifier |length|
//@[116:117) LeftParen |(|
//@[117:124) Identifier |regions|
//@[124:125) RightParen |)|
//@[125:126) RightParen |)|
//@[126:127) Colon |:|
//@[128:129) LeftBrace |{|
//@[129:130) NewLine |\n|
  name: '${name}-${index}'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:11) StringLeftPiece |'${|
//@[11:15) Identifier |name|
//@[15:19) StringMiddlePiece |}-${|
//@[19:24) Identifier |index|
//@[24:26) StringRightPiece |}'|
//@[26:27) NewLine |\n|
  location: 'Global'
//@[2:10) Identifier |location|
//@[10:11) Colon |:|
//@[12:20) StringComplete |'Global'|
//@[20:21) NewLine |\n|
  properties: {
//@[2:12) Identifier |properties|
//@[12:13) Colon |:|
//@[14:15) LeftBrace |{|
//@[15:16) NewLine |\n|
    backendPools: [
//@[4:16) Identifier |backendPools|
//@[16:17) Colon |:|
//@[18:19) LeftSquare |[|
//@[19:20) NewLine |\n|
      {
//@[6:7) LeftBrace |{|
//@[7:8) NewLine |\n|
        name: 'BackendAPIMs'
//@[8:12) Identifier |name|
//@[12:13) Colon |:|
//@[14:28) StringComplete |'BackendAPIMs'|
//@[28:29) NewLine |\n|
        properties: {
//@[8:18) Identifier |properties|
//@[18:19) Colon |:|
//@[20:21) LeftBrace |{|
//@[21:22) NewLine |\n|
          backends: [
//@[10:18) Identifier |backends|
//@[18:19) Colon |:|
//@[20:21) LeftSquare |[|
//@[21:22) NewLine |\n|
            {
//@[12:13) LeftBrace |{|
//@[13:14) NewLine |\n|
              // this indexed dependency on a module collection will be generated correctly because
//@[99:100) NewLine |\n|
              // copyIndex() can be invoked in the generated dependsOn
//@[70:71) NewLine |\n|
              address: apim[index].outputs.myOutput
//@[14:21) Identifier |address|
//@[21:22) Colon |:|
//@[23:27) Identifier |apim|
//@[27:28) LeftSquare |[|
//@[28:33) Identifier |index|
//@[33:34) RightSquare |]|
//@[34:35) Dot |.|
//@[35:42) Identifier |outputs|
//@[42:43) Dot |.|
//@[43:51) Identifier |myOutput|
//@[51:52) NewLine |\n|
              backendHostHeader: apim[index].outputs.myOutput
//@[14:31) Identifier |backendHostHeader|
//@[31:32) Colon |:|
//@[33:37) Identifier |apim|
//@[37:38) LeftSquare |[|
//@[38:43) Identifier |index|
//@[43:44) RightSquare |]|
//@[44:45) Dot |.|
//@[45:52) Identifier |outputs|
//@[52:53) Dot |.|
//@[53:61) Identifier |myOutput|
//@[61:62) NewLine |\n|
              httpPort: 80
//@[14:22) Identifier |httpPort|
//@[22:23) Colon |:|
//@[24:26) Integer |80|
//@[26:27) NewLine |\n|
              httpsPort: 443
//@[14:23) Identifier |httpsPort|
//@[23:24) Colon |:|
//@[25:28) Integer |443|
//@[28:29) NewLine |\n|
              priority: 1
//@[14:22) Identifier |priority|
//@[22:23) Colon |:|
//@[24:25) Integer |1|
//@[25:26) NewLine |\n|
              weight: 50
//@[14:20) Identifier |weight|
//@[20:21) Colon |:|
//@[22:24) Integer |50|
//@[24:25) NewLine |\n|
            }
//@[12:13) RightBrace |}|
//@[13:14) NewLine |\n|
          ]
//@[10:11) RightSquare |]|
//@[11:12) NewLine |\n|
        }
//@[8:9) RightBrace |}|
//@[9:10) NewLine |\n|
      }
//@[6:7) RightBrace |}|
//@[7:8) NewLine |\n|
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

resource propertyLoopDependencyOnResourceCollection 'Microsoft.Network/frontDoors@2020-05-01' = {
//@[0:8) Identifier |resource|
//@[9:51) Identifier |propertyLoopDependencyOnResourceCollection|
//@[52:93) StringComplete |'Microsoft.Network/frontDoors@2020-05-01'|
//@[94:95) Assignment |=|
//@[96:97) LeftBrace |{|
//@[97:98) NewLine |\n|
  name: name
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:12) Identifier |name|
//@[12:13) NewLine |\n|
  location: 'Global'
//@[2:10) Identifier |location|
//@[10:11) Colon |:|
//@[12:20) StringComplete |'Global'|
//@[20:21) NewLine |\n|
  properties: {
//@[2:12) Identifier |properties|
//@[12:13) Colon |:|
//@[14:15) LeftBrace |{|
//@[15:16) NewLine |\n|
    backendPools: [
//@[4:16) Identifier |backendPools|
//@[16:17) Colon |:|
//@[18:19) LeftSquare |[|
//@[19:20) NewLine |\n|
      {
//@[6:7) LeftBrace |{|
//@[7:8) NewLine |\n|
        name: 'BackendAPIMs'
//@[8:12) Identifier |name|
//@[12:13) Colon |:|
//@[14:28) StringComplete |'BackendAPIMs'|
//@[28:29) NewLine |\n|
        properties: {
//@[8:18) Identifier |properties|
//@[18:19) Colon |:|
//@[20:21) LeftBrace |{|
//@[21:22) NewLine |\n|
          backends: [for index in range(0, length(accounts)): {
//@[10:18) Identifier |backends|
//@[18:19) Colon |:|
//@[20:21) LeftSquare |[|
//@[21:24) Identifier |for|
//@[25:30) Identifier |index|
//@[31:33) Identifier |in|
//@[34:39) Identifier |range|
//@[39:40) LeftParen |(|
//@[40:41) Integer |0|
//@[41:42) Comma |,|
//@[43:49) Identifier |length|
//@[49:50) LeftParen |(|
//@[50:58) Identifier |accounts|
//@[58:59) RightParen |)|
//@[59:60) RightParen |)|
//@[60:61) Colon |:|
//@[62:63) LeftBrace |{|
//@[63:64) NewLine |\n|
            // we cannot codegen index correctly because the generated dependsOn property
//@[89:90) NewLine |\n|
            // would be outside of the scope of the property loop
//@[65:66) NewLine |\n|
            // as a result, this will generate a dependency on the entire collection
//@[84:85) NewLine |\n|
            address: storageAccounts[index].properties.primaryEndpoints.internetEndpoints.web
//@[12:19) Identifier |address|
//@[19:20) Colon |:|
//@[21:36) Identifier |storageAccounts|
//@[36:37) LeftSquare |[|
//@[37:42) Identifier |index|
//@[42:43) RightSquare |]|
//@[43:44) Dot |.|
//@[44:54) Identifier |properties|
//@[54:55) Dot |.|
//@[55:71) Identifier |primaryEndpoints|
//@[71:72) Dot |.|
//@[72:89) Identifier |internetEndpoints|
//@[89:90) Dot |.|
//@[90:93) Identifier |web|
//@[93:94) NewLine |\n|
            backendHostHeader: storageAccounts[index].properties.primaryEndpoints.internetEndpoints.web
//@[12:29) Identifier |backendHostHeader|
//@[29:30) Colon |:|
//@[31:46) Identifier |storageAccounts|
//@[46:47) LeftSquare |[|
//@[47:52) Identifier |index|
//@[52:53) RightSquare |]|
//@[53:54) Dot |.|
//@[54:64) Identifier |properties|
//@[64:65) Dot |.|
//@[65:81) Identifier |primaryEndpoints|
//@[81:82) Dot |.|
//@[82:99) Identifier |internetEndpoints|
//@[99:100) Dot |.|
//@[100:103) Identifier |web|
//@[103:104) NewLine |\n|
            httpPort: 80
//@[12:20) Identifier |httpPort|
//@[20:21) Colon |:|
//@[22:24) Integer |80|
//@[24:25) NewLine |\n|
            httpsPort: 443
//@[12:21) Identifier |httpsPort|
//@[21:22) Colon |:|
//@[23:26) Integer |443|
//@[26:27) NewLine |\n|
            priority: 1
//@[12:20) Identifier |priority|
//@[20:21) Colon |:|
//@[22:23) Integer |1|
//@[23:24) NewLine |\n|
            weight: 50
//@[12:18) Identifier |weight|
//@[18:19) Colon |:|
//@[20:22) Integer |50|
//@[22:23) NewLine |\n|
          }]
//@[10:11) RightBrace |}|
//@[11:12) RightSquare |]|
//@[12:13) NewLine |\n|
        }
//@[8:9) RightBrace |}|
//@[9:10) NewLine |\n|
      }
//@[6:7) RightBrace |}|
//@[7:8) NewLine |\n|
    ]
//@[4:5) RightSquare |]|
//@[5:6) NewLine |\n|
  }
//@[2:3) RightBrace |}|
//@[3:4) NewLine |\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

resource indexedResourceCollectionDependency 'Microsoft.Network/frontDoors@2020-05-01' = [for index in range(0, length(accounts)): {
//@[0:8) Identifier |resource|
//@[9:44) Identifier |indexedResourceCollectionDependency|
//@[45:86) StringComplete |'Microsoft.Network/frontDoors@2020-05-01'|
//@[87:88) Assignment |=|
//@[89:90) LeftSquare |[|
//@[90:93) Identifier |for|
//@[94:99) Identifier |index|
//@[100:102) Identifier |in|
//@[103:108) Identifier |range|
//@[108:109) LeftParen |(|
//@[109:110) Integer |0|
//@[110:111) Comma |,|
//@[112:118) Identifier |length|
//@[118:119) LeftParen |(|
//@[119:127) Identifier |accounts|
//@[127:128) RightParen |)|
//@[128:129) RightParen |)|
//@[129:130) Colon |:|
//@[131:132) LeftBrace |{|
//@[132:133) NewLine |\n|
  name: '${name}-${index}'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:11) StringLeftPiece |'${|
//@[11:15) Identifier |name|
//@[15:19) StringMiddlePiece |}-${|
//@[19:24) Identifier |index|
//@[24:26) StringRightPiece |}'|
//@[26:27) NewLine |\n|
  location: 'Global'
//@[2:10) Identifier |location|
//@[10:11) Colon |:|
//@[12:20) StringComplete |'Global'|
//@[20:21) NewLine |\n|
  properties: {
//@[2:12) Identifier |properties|
//@[12:13) Colon |:|
//@[14:15) LeftBrace |{|
//@[15:16) NewLine |\n|
    backendPools: [
//@[4:16) Identifier |backendPools|
//@[16:17) Colon |:|
//@[18:19) LeftSquare |[|
//@[19:20) NewLine |\n|
      {
//@[6:7) LeftBrace |{|
//@[7:8) NewLine |\n|
        name: 'BackendAPIMs'
//@[8:12) Identifier |name|
//@[12:13) Colon |:|
//@[14:28) StringComplete |'BackendAPIMs'|
//@[28:29) NewLine |\n|
        properties: {
//@[8:18) Identifier |properties|
//@[18:19) Colon |:|
//@[20:21) LeftBrace |{|
//@[21:22) NewLine |\n|
          backends: [
//@[10:18) Identifier |backends|
//@[18:19) Colon |:|
//@[20:21) LeftSquare |[|
//@[21:22) NewLine |\n|
            {
//@[12:13) LeftBrace |{|
//@[13:14) NewLine |\n|
              // this indexed dependency on a module collection will be generated correctly because
//@[99:100) NewLine |\n|
              // copyIndex() can be invoked in the generated dependsOn
//@[70:71) NewLine |\n|
              address: storageAccounts[index].properties.primaryEndpoints.internetEndpoints.web
//@[14:21) Identifier |address|
//@[21:22) Colon |:|
//@[23:38) Identifier |storageAccounts|
//@[38:39) LeftSquare |[|
//@[39:44) Identifier |index|
//@[44:45) RightSquare |]|
//@[45:46) Dot |.|
//@[46:56) Identifier |properties|
//@[56:57) Dot |.|
//@[57:73) Identifier |primaryEndpoints|
//@[73:74) Dot |.|
//@[74:91) Identifier |internetEndpoints|
//@[91:92) Dot |.|
//@[92:95) Identifier |web|
//@[95:96) NewLine |\n|
              backendHostHeader: storageAccounts[index].properties.primaryEndpoints.internetEndpoints.web
//@[14:31) Identifier |backendHostHeader|
//@[31:32) Colon |:|
//@[33:48) Identifier |storageAccounts|
//@[48:49) LeftSquare |[|
//@[49:54) Identifier |index|
//@[54:55) RightSquare |]|
//@[55:56) Dot |.|
//@[56:66) Identifier |properties|
//@[66:67) Dot |.|
//@[67:83) Identifier |primaryEndpoints|
//@[83:84) Dot |.|
//@[84:101) Identifier |internetEndpoints|
//@[101:102) Dot |.|
//@[102:105) Identifier |web|
//@[105:106) NewLine |\n|
              httpPort: 80
//@[14:22) Identifier |httpPort|
//@[22:23) Colon |:|
//@[24:26) Integer |80|
//@[26:27) NewLine |\n|
              httpsPort: 443
//@[14:23) Identifier |httpsPort|
//@[23:24) Colon |:|
//@[25:28) Integer |443|
//@[28:29) NewLine |\n|
              priority: 1
//@[14:22) Identifier |priority|
//@[22:23) Colon |:|
//@[24:25) Integer |1|
//@[25:26) NewLine |\n|
              weight: 50
//@[14:20) Identifier |weight|
//@[20:21) Colon |:|
//@[22:24) Integer |50|
//@[24:25) NewLine |\n|
            }
//@[12:13) RightBrace |}|
//@[13:14) NewLine |\n|
          ]
//@[10:11) RightSquare |]|
//@[11:12) NewLine |\n|
        }
//@[8:9) RightBrace |}|
//@[9:10) NewLine |\n|
      }
//@[6:7) RightBrace |}|
//@[7:8) NewLine |\n|
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

resource filteredZones 'Microsoft.Network/dnsZones@2018-05-01' = [for i in range(0,10): if(i % 3 == 0) {
//@[0:8) Identifier |resource|
//@[9:22) Identifier |filteredZones|
//@[23:62) StringComplete |'Microsoft.Network/dnsZones@2018-05-01'|
//@[63:64) Assignment |=|
//@[65:66) LeftSquare |[|
//@[66:69) Identifier |for|
//@[70:71) Identifier |i|
//@[72:74) Identifier |in|
//@[75:80) Identifier |range|
//@[80:81) LeftParen |(|
//@[81:82) Integer |0|
//@[82:83) Comma |,|
//@[83:85) Integer |10|
//@[85:86) RightParen |)|
//@[86:87) Colon |:|
//@[88:90) Identifier |if|
//@[90:91) LeftParen |(|
//@[91:92) Identifier |i|
//@[93:94) Modulo |%|
//@[95:96) Integer |3|
//@[97:99) Equals |==|
//@[100:101) Integer |0|
//@[101:102) RightParen |)|
//@[103:104) LeftBrace |{|
//@[104:105) NewLine |\n|
  name: 'zone${i}'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:15) StringLeftPiece |'zone${|
//@[15:16) Identifier |i|
//@[16:18) StringRightPiece |}'|
//@[18:19) NewLine |\n|
  location: resourceGroup().location
//@[2:10) Identifier |location|
//@[10:11) Colon |:|
//@[12:25) Identifier |resourceGroup|
//@[25:26) LeftParen |(|
//@[26:27) RightParen |)|
//@[27:28) Dot |.|
//@[28:36) Identifier |location|
//@[36:37) NewLine |\n|
}]
//@[0:1) RightBrace |}|
//@[1:2) RightSquare |]|
//@[2:4) NewLine |\n\n|

module filteredModules 'passthrough.bicep' = [for i in range(0,6): if(i % 2 == 0) {
//@[0:6) Identifier |module|
//@[7:22) Identifier |filteredModules|
//@[23:42) StringComplete |'passthrough.bicep'|
//@[43:44) Assignment |=|
//@[45:46) LeftSquare |[|
//@[46:49) Identifier |for|
//@[50:51) Identifier |i|
//@[52:54) Identifier |in|
//@[55:60) Identifier |range|
//@[60:61) LeftParen |(|
//@[61:62) Integer |0|
//@[62:63) Comma |,|
//@[63:64) Integer |6|
//@[64:65) RightParen |)|
//@[65:66) Colon |:|
//@[67:69) Identifier |if|
//@[69:70) LeftParen |(|
//@[70:71) Identifier |i|
//@[72:73) Modulo |%|
//@[74:75) Integer |2|
//@[76:78) Equals |==|
//@[79:80) Integer |0|
//@[80:81) RightParen |)|
//@[82:83) LeftBrace |{|
//@[83:84) NewLine |\n|
  name: 'stuff${i}'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:16) StringLeftPiece |'stuff${|
//@[16:17) Identifier |i|
//@[17:19) StringRightPiece |}'|
//@[19:20) NewLine |\n|
  params: {
//@[2:8) Identifier |params|
//@[8:9) Colon |:|
//@[10:11) LeftBrace |{|
//@[11:12) NewLine |\n|
    myInput: 'script-${i}'
//@[4:11) Identifier |myInput|
//@[11:12) Colon |:|
//@[13:23) StringLeftPiece |'script-${|
//@[23:24) Identifier |i|
//@[24:26) StringRightPiece |}'|
//@[26:27) NewLine |\n|
  }
//@[2:3) RightBrace |}|
//@[3:4) NewLine |\n|
}]
//@[0:1) RightBrace |}|
//@[1:2) RightSquare |]|
//@[2:4) NewLine |\n\n|

resource filteredIndexedZones 'Microsoft.Network/dnsZones@2018-05-01' = [for (account, i) in accounts: if(account.enabled) {
//@[0:8) Identifier |resource|
//@[9:29) Identifier |filteredIndexedZones|
//@[30:69) StringComplete |'Microsoft.Network/dnsZones@2018-05-01'|
//@[70:71) Assignment |=|
//@[72:73) LeftSquare |[|
//@[73:76) Identifier |for|
//@[77:78) LeftParen |(|
//@[78:85) Identifier |account|
//@[85:86) Comma |,|
//@[87:88) Identifier |i|
//@[88:89) RightParen |)|
//@[90:92) Identifier |in|
//@[93:101) Identifier |accounts|
//@[101:102) Colon |:|
//@[103:105) Identifier |if|
//@[105:106) LeftParen |(|
//@[106:113) Identifier |account|
//@[113:114) Dot |.|
//@[114:121) Identifier |enabled|
//@[121:122) RightParen |)|
//@[123:124) LeftBrace |{|
//@[124:125) NewLine |\n|
  name: 'indexedZone-${account.name}-${i}'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:23) StringLeftPiece |'indexedZone-${|
//@[23:30) Identifier |account|
//@[30:31) Dot |.|
//@[31:35) Identifier |name|
//@[35:39) StringMiddlePiece |}-${|
//@[39:40) Identifier |i|
//@[40:42) StringRightPiece |}'|
//@[42:43) NewLine |\n|
  location: account.location
//@[2:10) Identifier |location|
//@[10:11) Colon |:|
//@[12:19) Identifier |account|
//@[19:20) Dot |.|
//@[20:28) Identifier |location|
//@[28:29) NewLine |\n|
}]
//@[0:1) RightBrace |}|
//@[1:2) RightSquare |]|
//@[2:4) NewLine |\n\n|

output lastNameServers array = filteredIndexedZones[length(accounts) - 1].properties.nameServers
//@[0:6) Identifier |output|
//@[7:22) Identifier |lastNameServers|
//@[23:28) Identifier |array|
//@[29:30) Assignment |=|
//@[31:51) Identifier |filteredIndexedZones|
//@[51:52) LeftSquare |[|
//@[52:58) Identifier |length|
//@[58:59) LeftParen |(|
//@[59:67) Identifier |accounts|
//@[67:68) RightParen |)|
//@[69:70) Minus |-|
//@[71:72) Integer |1|
//@[72:73) RightSquare |]|
//@[73:74) Dot |.|
//@[74:84) Identifier |properties|
//@[84:85) Dot |.|
//@[85:96) Identifier |nameServers|
//@[96:98) NewLine |\n\n|

module filteredIndexedModules 'passthrough.bicep' = [for (account, i) in accounts: if(account.enabled) {
//@[0:6) Identifier |module|
//@[7:29) Identifier |filteredIndexedModules|
//@[30:49) StringComplete |'passthrough.bicep'|
//@[50:51) Assignment |=|
//@[52:53) LeftSquare |[|
//@[53:56) Identifier |for|
//@[57:58) LeftParen |(|
//@[58:65) Identifier |account|
//@[65:66) Comma |,|
//@[67:68) Identifier |i|
//@[68:69) RightParen |)|
//@[70:72) Identifier |in|
//@[73:81) Identifier |accounts|
//@[81:82) Colon |:|
//@[83:85) Identifier |if|
//@[85:86) LeftParen |(|
//@[86:93) Identifier |account|
//@[93:94) Dot |.|
//@[94:101) Identifier |enabled|
//@[101:102) RightParen |)|
//@[103:104) LeftBrace |{|
//@[104:105) NewLine |\n|
  name: 'stuff-${i}'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:17) StringLeftPiece |'stuff-${|
//@[17:18) Identifier |i|
//@[18:20) StringRightPiece |}'|
//@[20:21) NewLine |\n|
  params: {
//@[2:8) Identifier |params|
//@[8:9) Colon |:|
//@[10:11) LeftBrace |{|
//@[11:12) NewLine |\n|
    myInput: 'script-${account.name}-${i}'
//@[4:11) Identifier |myInput|
//@[11:12) Colon |:|
//@[13:23) StringLeftPiece |'script-${|
//@[23:30) Identifier |account|
//@[30:31) Dot |.|
//@[31:35) Identifier |name|
//@[35:39) StringMiddlePiece |}-${|
//@[39:40) Identifier |i|
//@[40:42) StringRightPiece |}'|
//@[42:43) NewLine |\n|
  }
//@[2:3) RightBrace |}|
//@[3:4) NewLine |\n|
}]
//@[0:1) RightBrace |}|
//@[1:2) RightSquare |]|
//@[2:4) NewLine |\n\n|

output lastModuleOutput string = filteredIndexedModules[length(accounts) - 1].outputs.myOutput
//@[0:6) Identifier |output|
//@[7:23) Identifier |lastModuleOutput|
//@[24:30) Identifier |string|
//@[31:32) Assignment |=|
//@[33:55) Identifier |filteredIndexedModules|
//@[55:56) LeftSquare |[|
//@[56:62) Identifier |length|
//@[62:63) LeftParen |(|
//@[63:71) Identifier |accounts|
//@[71:72) RightParen |)|
//@[73:74) Minus |-|
//@[75:76) Integer |1|
//@[76:77) RightSquare |]|
//@[77:78) Dot |.|
//@[78:85) Identifier |outputs|
//@[85:86) Dot |.|
//@[86:94) Identifier |myOutput|
//@[94:95) NewLine |\n|

//@[0:0) EndOfFile ||
