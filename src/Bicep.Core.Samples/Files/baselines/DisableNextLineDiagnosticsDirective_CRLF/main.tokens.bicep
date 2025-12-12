var vmProperties = {
//@[00:03) Identifier |var|
//@[04:16) Identifier |vmProperties|
//@[17:18) Assignment |=|
//@[19:20) LeftBrace |{|
//@[20:22) NewLine |\r\n|
  diagnosticsProfile: {
//@[02:20) Identifier |diagnosticsProfile|
//@[20:21) Colon |:|
//@[22:23) LeftBrace |{|
//@[23:25) NewLine |\r\n|
    bootDiagnostics: {
//@[04:19) Identifier |bootDiagnostics|
//@[19:20) Colon |:|
//@[21:22) LeftBrace |{|
//@[22:24) NewLine |\r\n|
      enabled: 123
//@[06:13) Identifier |enabled|
//@[13:14) Colon |:|
//@[15:18) Integer |123|
//@[18:20) NewLine |\r\n|
      storageUri: true
//@[06:16) Identifier |storageUri|
//@[16:17) Colon |:|
//@[18:22) TrueKeyword |true|
//@[22:24) NewLine |\r\n|
      unknownProp: 'asdf'
//@[06:17) Identifier |unknownProp|
//@[17:18) Colon |:|
//@[19:25) StringComplete |'asdf'|
//@[25:27) NewLine |\r\n|
    }
//@[04:05) RightBrace |}|
//@[05:07) NewLine |\r\n|
  }
//@[02:03) RightBrace |}|
//@[03:05) NewLine |\r\n|
  evictionPolicy: 'Deallocate'
//@[02:16) Identifier |evictionPolicy|
//@[16:17) Colon |:|
//@[18:30) StringComplete |'Deallocate'|
//@[30:32) NewLine |\r\n|
}
//@[00:01) RightBrace |}|
//@[01:03) NewLine |\r\n|
resource vm 'Microsoft.Compute/virtualMachines@2020-12-01' = {
//@[00:08) Identifier |resource|
//@[09:11) Identifier |vm|
//@[12:58) StringComplete |'Microsoft.Compute/virtualMachines@2020-12-01'|
//@[59:60) Assignment |=|
//@[61:62) LeftBrace |{|
//@[62:64) NewLine |\r\n|
  name: 'vm'
//@[02:06) Identifier |name|
//@[06:07) Colon |:|
//@[08:12) StringComplete |'vm'|
//@[12:14) NewLine |\r\n|
  location: 'West US'
//@[02:10) Identifier |location|
//@[10:11) Colon |:|
//@[12:21) StringComplete |'West US'|
//@[21:23) NewLine |\r\n|
#disable-next-line BCP036 BCP037
//@[32:34) NewLine |\r\n|
  properties: vmProperties
//@[02:12) Identifier |properties|
//@[12:13) Colon |:|
//@[14:26) Identifier |vmProperties|
//@[26:28) NewLine |\r\n|
}
//@[00:01) RightBrace |}|
//@[01:03) NewLine |\r\n|
#disable-next-line no-unused-params
//@[35:37) NewLine |\r\n|
param storageAccount1 string = 'testStorageAccount'
//@[00:05) Identifier |param|
//@[06:21) Identifier |storageAccount1|
//@[22:28) Identifier |string|
//@[29:30) Assignment |=|
//@[31:51) StringComplete |'testStorageAccount'|
//@[51:53) NewLine |\r\n|
#disable-next-line          no-unused-params
//@[44:46) NewLine |\r\n|
param storageAccount2 string = 'testStorageAccount'
//@[00:05) Identifier |param|
//@[06:21) Identifier |storageAccount2|
//@[22:28) Identifier |string|
//@[29:30) Assignment |=|
//@[31:51) StringComplete |'testStorageAccount'|
//@[51:53) NewLine |\r\n|
#disable-next-line   no-unused-params                /* Test comment 1 */
//@[73:75) NewLine |\r\n|
param storageAccount3 string = 'testStorageAccount'
//@[00:05) Identifier |param|
//@[06:21) Identifier |storageAccount3|
//@[22:28) Identifier |string|
//@[29:30) Assignment |=|
//@[31:51) StringComplete |'testStorageAccount'|
//@[51:53) NewLine |\r\n|
         #disable-next-line   no-unused-params                // Test comment 2
//@[79:81) NewLine |\r\n|
param storageAccount5 string = 'testStorageAccount'
//@[00:05) Identifier |param|
//@[06:21) Identifier |storageAccount5|
//@[22:28) Identifier |string|
//@[29:30) Assignment |=|
//@[31:51) StringComplete |'testStorageAccount'|
//@[51:55) NewLine |\r\n\r\n|

#disable-diagnostics                 no-unused-params                      no-unused-vars
//@[89:91) NewLine |\r\n|
param storageAccount4 string = 'testStorageAccount'
//@[00:05) Identifier |param|
//@[06:21) Identifier |storageAccount4|
//@[22:28) Identifier |string|
//@[29:30) Assignment |=|
//@[31:51) StringComplete |'testStorageAccount'|
//@[51:53) NewLine |\r\n|
var unusedVar1 = 'This is an unused variable'
//@[00:03) Identifier |var|
//@[04:14) Identifier |unusedVar1|
//@[15:16) Assignment |=|
//@[17:45) StringComplete |'This is an unused variable'|
//@[45:47) NewLine |\r\n|
var unusedVar2 = 'This is another unused variable'
//@[00:03) Identifier |var|
//@[04:14) Identifier |unusedVar2|
//@[15:16) Assignment |=|
//@[17:50) StringComplete |'This is another unused variable'|
//@[50:52) NewLine |\r\n|
#restore-diagnostics   no-unused-vars
//@[37:39) NewLine |\r\n|
param storageAccount6 string = 'testStorageAccount'
//@[00:05) Identifier |param|
//@[06:21) Identifier |storageAccount6|
//@[22:28) Identifier |string|
//@[29:30) Assignment |=|
//@[31:51) StringComplete |'testStorageAccount'|
//@[51:53) NewLine |\r\n|
var unusedVar3 = 'This is yet another unused variable'
//@[00:03) Identifier |var|
//@[04:14) Identifier |unusedVar3|
//@[15:16) Assignment |=|
//@[17:54) StringComplete |'This is yet another unused variable'|
//@[54:56) NewLine |\r\n|
#restore-diagnostics    no-unused-params
//@[40:42) NewLine |\r\n|
param storageAccount7 string = 'testStorageAccount'
//@[00:05) Identifier |param|
//@[06:21) Identifier |storageAccount7|
//@[22:28) Identifier |string|
//@[29:30) Assignment |=|
//@[31:51) StringComplete |'testStorageAccount'|
//@[51:53) NewLine |\r\n|

//@[00:00) EndOfFile ||
