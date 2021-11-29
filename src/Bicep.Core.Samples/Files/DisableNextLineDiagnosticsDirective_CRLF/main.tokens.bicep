var vmProperties = {
//@[0:3) Identifier |var|
//@[4:16) Identifier |vmProperties|
//@[17:18) Assignment |=|
//@[19:20) LeftBrace |{|
//@[20:22) NewLine |\r\n|
  diagnosticsProfile: {
//@[2:20) Identifier |diagnosticsProfile|
//@[20:21) Colon |:|
//@[22:23) LeftBrace |{|
//@[23:25) NewLine |\r\n|
    bootDiagnostics: {
//@[4:19) Identifier |bootDiagnostics|
//@[19:20) Colon |:|
//@[21:22) LeftBrace |{|
//@[22:24) NewLine |\r\n|
      enabled: 123
//@[6:13) Identifier |enabled|
//@[13:14) Colon |:|
//@[15:18) Integer |123|
//@[18:20) NewLine |\r\n|
      storageUri: true
//@[6:16) Identifier |storageUri|
//@[16:17) Colon |:|
//@[18:22) TrueKeyword |true|
//@[22:24) NewLine |\r\n|
      unknownProp: 'asdf'
//@[6:17) Identifier |unknownProp|
//@[17:18) Colon |:|
//@[19:25) StringComplete |'asdf'|
//@[25:27) NewLine |\r\n|
    }
//@[4:5) RightBrace |}|
//@[5:7) NewLine |\r\n|
  }
//@[2:3) RightBrace |}|
//@[3:5) NewLine |\r\n|
  evictionPolicy: 'Deallocate'
//@[2:16) Identifier |evictionPolicy|
//@[16:17) Colon |:|
//@[18:30) StringComplete |'Deallocate'|
//@[30:32) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\r\n|
resource vm 'Microsoft.Compute/virtualMachines@2020-12-01' = {
//@[0:8) Identifier |resource|
//@[9:11) Identifier |vm|
//@[12:58) StringComplete |'Microsoft.Compute/virtualMachines@2020-12-01'|
//@[59:60) Assignment |=|
//@[61:62) LeftBrace |{|
//@[62:64) NewLine |\r\n|
  name: 'vm'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:12) StringComplete |'vm'|
//@[12:14) NewLine |\r\n|
  location: 'West US'
//@[2:10) Identifier |location|
//@[10:11) Colon |:|
//@[12:21) StringComplete |'West US'|
//@[21:23) NewLine |\r\n|
#disable-next-line BCP036 BCP037
//@[32:34) NewLine |\r\n|
  properties: vmProperties
//@[2:12) Identifier |properties|
//@[12:13) Colon |:|
//@[14:26) Identifier |vmProperties|
//@[26:28) NewLine |\r\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\r\n|
#disable-next-line no-unused-params
//@[35:37) NewLine |\r\n|
param storageAccount1 string = 'testStorageAccount'
//@[0:5) Identifier |param|
//@[6:21) Identifier |storageAccount1|
//@[22:28) Identifier |string|
//@[29:30) Assignment |=|
//@[31:51) StringComplete |'testStorageAccount'|
//@[51:53) NewLine |\r\n|
#disable-next-line          no-unused-params
//@[44:46) NewLine |\r\n|
param storageAccount2 string = 'testStorageAccount'
//@[0:5) Identifier |param|
//@[6:21) Identifier |storageAccount2|
//@[22:28) Identifier |string|
//@[29:30) Assignment |=|
//@[31:51) StringComplete |'testStorageAccount'|
//@[51:53) NewLine |\r\n|
#disable-next-line   no-unused-params                /* Test comment 1 */
//@[73:75) NewLine |\r\n|
param storageAccount3 string = 'testStorageAccount'
//@[0:5) Identifier |param|
//@[6:21) Identifier |storageAccount3|
//@[22:28) Identifier |string|
//@[29:30) Assignment |=|
//@[31:51) StringComplete |'testStorageAccount'|
//@[51:53) NewLine |\r\n|
         #disable-next-line   no-unused-params                // Test comment 2
//@[79:81) NewLine |\r\n|
param storageAccount5 string = 'testStorageAccount'
//@[0:5) Identifier |param|
//@[6:21) Identifier |storageAccount5|
//@[22:28) Identifier |string|
//@[29:30) Assignment |=|
//@[31:51) StringComplete |'testStorageAccount'|
//@[51:51) EndOfFile ||
