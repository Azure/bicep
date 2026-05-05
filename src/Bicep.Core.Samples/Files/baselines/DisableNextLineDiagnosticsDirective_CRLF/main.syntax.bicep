var vmProperties = {
//@[00:1294) ProgramSyntax
//@[00:0187) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0016) | ├─IdentifierSyntax
//@[04:0016) | | └─Token(Identifier) |vmProperties|
//@[17:0018) | ├─Token(Assignment) |=|
//@[19:0187) | └─ObjectSyntax
//@[19:0020) |   ├─Token(LeftBrace) |{|
//@[20:0022) |   ├─Token(NewLine) |\r\n|
  diagnosticsProfile: {
//@[02:0130) |   ├─ObjectPropertySyntax
//@[02:0020) |   | ├─IdentifierSyntax
//@[02:0020) |   | | └─Token(Identifier) |diagnosticsProfile|
//@[20:0021) |   | ├─Token(Colon) |:|
//@[22:0130) |   | └─ObjectSyntax
//@[22:0023) |   |   ├─Token(LeftBrace) |{|
//@[23:0025) |   |   ├─Token(NewLine) |\r\n|
    bootDiagnostics: {
//@[04:0100) |   |   ├─ObjectPropertySyntax
//@[04:0019) |   |   | ├─IdentifierSyntax
//@[04:0019) |   |   | | └─Token(Identifier) |bootDiagnostics|
//@[19:0020) |   |   | ├─Token(Colon) |:|
//@[21:0100) |   |   | └─ObjectSyntax
//@[21:0022) |   |   |   ├─Token(LeftBrace) |{|
//@[22:0024) |   |   |   ├─Token(NewLine) |\r\n|
      enabled: 123
//@[06:0018) |   |   |   ├─ObjectPropertySyntax
//@[06:0013) |   |   |   | ├─IdentifierSyntax
//@[06:0013) |   |   |   | | └─Token(Identifier) |enabled|
//@[13:0014) |   |   |   | ├─Token(Colon) |:|
//@[15:0018) |   |   |   | └─IntegerLiteralSyntax
//@[15:0018) |   |   |   |   └─Token(Integer) |123|
//@[18:0020) |   |   |   ├─Token(NewLine) |\r\n|
      storageUri: true
//@[06:0022) |   |   |   ├─ObjectPropertySyntax
//@[06:0016) |   |   |   | ├─IdentifierSyntax
//@[06:0016) |   |   |   | | └─Token(Identifier) |storageUri|
//@[16:0017) |   |   |   | ├─Token(Colon) |:|
//@[18:0022) |   |   |   | └─BooleanLiteralSyntax
//@[18:0022) |   |   |   |   └─Token(TrueKeyword) |true|
//@[22:0024) |   |   |   ├─Token(NewLine) |\r\n|
      unknownProp: 'asdf'
//@[06:0025) |   |   |   ├─ObjectPropertySyntax
//@[06:0017) |   |   |   | ├─IdentifierSyntax
//@[06:0017) |   |   |   | | └─Token(Identifier) |unknownProp|
//@[17:0018) |   |   |   | ├─Token(Colon) |:|
//@[19:0025) |   |   |   | └─StringSyntax
//@[19:0025) |   |   |   |   └─Token(StringComplete) |'asdf'|
//@[25:0027) |   |   |   ├─Token(NewLine) |\r\n|
    }
//@[04:0005) |   |   |   └─Token(RightBrace) |}|
//@[05:0007) |   |   ├─Token(NewLine) |\r\n|
  }
//@[02:0003) |   |   └─Token(RightBrace) |}|
//@[03:0005) |   ├─Token(NewLine) |\r\n|
  evictionPolicy: 'Deallocate'
//@[02:0030) |   ├─ObjectPropertySyntax
//@[02:0016) |   | ├─IdentifierSyntax
//@[02:0016) |   | | └─Token(Identifier) |evictionPolicy|
//@[16:0017) |   | ├─Token(Colon) |:|
//@[18:0030) |   | └─StringSyntax
//@[18:0030) |   |   └─Token(StringComplete) |'Deallocate'|
//@[30:0032) |   ├─Token(NewLine) |\r\n|
}
//@[00:0001) |   └─Token(RightBrace) |}|
//@[01:0003) ├─Token(NewLine) |\r\n|
resource vm 'Microsoft.Compute/virtualMachines@2020-12-01' = {
//@[00:0164) ├─ResourceDeclarationSyntax
//@[00:0008) | ├─Token(Identifier) |resource|
//@[09:0011) | ├─IdentifierSyntax
//@[09:0011) | | └─Token(Identifier) |vm|
//@[12:0058) | ├─StringSyntax
//@[12:0058) | | └─Token(StringComplete) |'Microsoft.Compute/virtualMachines@2020-12-01'|
//@[59:0060) | ├─Token(Assignment) |=|
//@[61:0164) | └─ObjectSyntax
//@[61:0062) |   ├─Token(LeftBrace) |{|
//@[62:0064) |   ├─Token(NewLine) |\r\n|
  name: 'vm'
//@[02:0012) |   ├─ObjectPropertySyntax
//@[02:0006) |   | ├─IdentifierSyntax
//@[02:0006) |   | | └─Token(Identifier) |name|
//@[06:0007) |   | ├─Token(Colon) |:|
//@[08:0012) |   | └─StringSyntax
//@[08:0012) |   |   └─Token(StringComplete) |'vm'|
//@[12:0014) |   ├─Token(NewLine) |\r\n|
  location: 'West US'
//@[02:0021) |   ├─ObjectPropertySyntax
//@[02:0010) |   | ├─IdentifierSyntax
//@[02:0010) |   | | └─Token(Identifier) |location|
//@[10:0011) |   | ├─Token(Colon) |:|
//@[12:0021) |   | └─StringSyntax
//@[12:0021) |   |   └─Token(StringComplete) |'West US'|
//@[21:0023) |   ├─Token(NewLine) |\r\n|
#disable-next-line BCP036 BCP037
//@[32:0034) |   ├─Token(NewLine) |\r\n|
  properties: vmProperties
//@[02:0026) |   ├─ObjectPropertySyntax
//@[02:0012) |   | ├─IdentifierSyntax
//@[02:0012) |   | | └─Token(Identifier) |properties|
//@[12:0013) |   | ├─Token(Colon) |:|
//@[14:0026) |   | └─VariableAccessSyntax
//@[14:0026) |   |   └─IdentifierSyntax
//@[14:0026) |   |     └─Token(Identifier) |vmProperties|
//@[26:0028) |   ├─Token(NewLine) |\r\n|
}
//@[00:0001) |   └─Token(RightBrace) |}|
//@[01:0003) ├─Token(NewLine) |\r\n|
#disable-next-line no-unused-params
//@[35:0037) ├─Token(NewLine) |\r\n|
param storageAccount1 string = 'testStorageAccount'
//@[00:0051) ├─ParameterDeclarationSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0021) | ├─IdentifierSyntax
//@[06:0021) | | └─Token(Identifier) |storageAccount1|
//@[22:0028) | ├─TypeVariableAccessSyntax
//@[22:0028) | | └─IdentifierSyntax
//@[22:0028) | |   └─Token(Identifier) |string|
//@[29:0051) | └─ParameterDefaultValueSyntax
//@[29:0030) |   ├─Token(Assignment) |=|
//@[31:0051) |   └─StringSyntax
//@[31:0051) |     └─Token(StringComplete) |'testStorageAccount'|
//@[51:0053) ├─Token(NewLine) |\r\n|
#disable-next-line          no-unused-params
//@[44:0046) ├─Token(NewLine) |\r\n|
param storageAccount2 string = 'testStorageAccount'
//@[00:0051) ├─ParameterDeclarationSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0021) | ├─IdentifierSyntax
//@[06:0021) | | └─Token(Identifier) |storageAccount2|
//@[22:0028) | ├─TypeVariableAccessSyntax
//@[22:0028) | | └─IdentifierSyntax
//@[22:0028) | |   └─Token(Identifier) |string|
//@[29:0051) | └─ParameterDefaultValueSyntax
//@[29:0030) |   ├─Token(Assignment) |=|
//@[31:0051) |   └─StringSyntax
//@[31:0051) |     └─Token(StringComplete) |'testStorageAccount'|
//@[51:0053) ├─Token(NewLine) |\r\n|
#disable-next-line   no-unused-params                /* Test comment 1 */
//@[73:0075) ├─Token(NewLine) |\r\n|
param storageAccount3 string = 'testStorageAccount'
//@[00:0051) ├─ParameterDeclarationSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0021) | ├─IdentifierSyntax
//@[06:0021) | | └─Token(Identifier) |storageAccount3|
//@[22:0028) | ├─TypeVariableAccessSyntax
//@[22:0028) | | └─IdentifierSyntax
//@[22:0028) | |   └─Token(Identifier) |string|
//@[29:0051) | └─ParameterDefaultValueSyntax
//@[29:0030) |   ├─Token(Assignment) |=|
//@[31:0051) |   └─StringSyntax
//@[31:0051) |     └─Token(StringComplete) |'testStorageAccount'|
//@[51:0053) ├─Token(NewLine) |\r\n|
         #disable-next-line   no-unused-params                // Test comment 2
//@[79:0081) ├─Token(NewLine) |\r\n|
param storageAccount5 string = 'testStorageAccount'
//@[00:0051) ├─ParameterDeclarationSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0021) | ├─IdentifierSyntax
//@[06:0021) | | └─Token(Identifier) |storageAccount5|
//@[22:0028) | ├─TypeVariableAccessSyntax
//@[22:0028) | | └─IdentifierSyntax
//@[22:0028) | |   └─Token(Identifier) |string|
//@[29:0051) | └─ParameterDefaultValueSyntax
//@[29:0030) |   ├─Token(Assignment) |=|
//@[31:0051) |   └─StringSyntax
//@[31:0051) |     └─Token(StringComplete) |'testStorageAccount'|
//@[51:0055) ├─Token(NewLine) |\r\n\r\n|

#disable-diagnostics                 no-unused-params                      no-unused-vars
//@[89:0091) ├─Token(NewLine) |\r\n|
param storageAccount4 string = 'testStorageAccount'
//@[00:0051) ├─ParameterDeclarationSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0021) | ├─IdentifierSyntax
//@[06:0021) | | └─Token(Identifier) |storageAccount4|
//@[22:0028) | ├─TypeVariableAccessSyntax
//@[22:0028) | | └─IdentifierSyntax
//@[22:0028) | |   └─Token(Identifier) |string|
//@[29:0051) | └─ParameterDefaultValueSyntax
//@[29:0030) |   ├─Token(Assignment) |=|
//@[31:0051) |   └─StringSyntax
//@[31:0051) |     └─Token(StringComplete) |'testStorageAccount'|
//@[51:0053) ├─Token(NewLine) |\r\n|
var unusedVar1 = 'This is an unused variable'
//@[00:0045) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0014) | ├─IdentifierSyntax
//@[04:0014) | | └─Token(Identifier) |unusedVar1|
//@[15:0016) | ├─Token(Assignment) |=|
//@[17:0045) | └─StringSyntax
//@[17:0045) |   └─Token(StringComplete) |'This is an unused variable'|
//@[45:0047) ├─Token(NewLine) |\r\n|
var unusedVar2 = 'This is another unused variable'
//@[00:0050) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0014) | ├─IdentifierSyntax
//@[04:0014) | | └─Token(Identifier) |unusedVar2|
//@[15:0016) | ├─Token(Assignment) |=|
//@[17:0050) | └─StringSyntax
//@[17:0050) |   └─Token(StringComplete) |'This is another unused variable'|
//@[50:0052) ├─Token(NewLine) |\r\n|
#restore-diagnostics   no-unused-vars
//@[37:0039) ├─Token(NewLine) |\r\n|
param storageAccount6 string = 'testStorageAccount'
//@[00:0051) ├─ParameterDeclarationSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0021) | ├─IdentifierSyntax
//@[06:0021) | | └─Token(Identifier) |storageAccount6|
//@[22:0028) | ├─TypeVariableAccessSyntax
//@[22:0028) | | └─IdentifierSyntax
//@[22:0028) | |   └─Token(Identifier) |string|
//@[29:0051) | └─ParameterDefaultValueSyntax
//@[29:0030) |   ├─Token(Assignment) |=|
//@[31:0051) |   └─StringSyntax
//@[31:0051) |     └─Token(StringComplete) |'testStorageAccount'|
//@[51:0053) ├─Token(NewLine) |\r\n|
var unusedVar3 = 'This is yet another unused variable'
//@[00:0054) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0014) | ├─IdentifierSyntax
//@[04:0014) | | └─Token(Identifier) |unusedVar3|
//@[15:0016) | ├─Token(Assignment) |=|
//@[17:0054) | └─StringSyntax
//@[17:0054) |   └─Token(StringComplete) |'This is yet another unused variable'|
//@[54:0056) ├─Token(NewLine) |\r\n|
#restore-diagnostics    no-unused-params
//@[40:0042) ├─Token(NewLine) |\r\n|
param storageAccount7 string = 'testStorageAccount'
//@[00:0051) ├─ParameterDeclarationSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0021) | ├─IdentifierSyntax
//@[06:0021) | | └─Token(Identifier) |storageAccount7|
//@[22:0028) | ├─TypeVariableAccessSyntax
//@[22:0028) | | └─IdentifierSyntax
//@[22:0028) | |   └─Token(Identifier) |string|
//@[29:0051) | └─ParameterDefaultValueSyntax
//@[29:0030) |   ├─Token(Assignment) |=|
//@[31:0051) |   └─StringSyntax
//@[31:0051) |     └─Token(StringComplete) |'testStorageAccount'|
//@[51:0053) ├─Token(NewLine) |\r\n|

//@[00:0000) └─Token(EndOfFile) ||
