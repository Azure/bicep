var vmProperties = {
//@[00:804) ProgramSyntax
//@[00:187) ├─VariableDeclarationSyntax
//@[00:003) | ├─Token(Identifier) |var|
//@[04:016) | ├─IdentifierSyntax
//@[04:016) | | └─Token(Identifier) |vmProperties|
//@[17:018) | ├─Token(Assignment) |=|
//@[19:187) | └─ObjectSyntax
//@[19:020) |   ├─Token(LeftBrace) |{|
//@[20:022) |   ├─Token(NewLine) |\r\n|
  diagnosticsProfile: {
//@[02:130) |   ├─ObjectPropertySyntax
//@[02:020) |   | ├─IdentifierSyntax
//@[02:020) |   | | └─Token(Identifier) |diagnosticsProfile|
//@[20:021) |   | ├─Token(Colon) |:|
//@[22:130) |   | └─ObjectSyntax
//@[22:023) |   |   ├─Token(LeftBrace) |{|
//@[23:025) |   |   ├─Token(NewLine) |\r\n|
    bootDiagnostics: {
//@[04:100) |   |   ├─ObjectPropertySyntax
//@[04:019) |   |   | ├─IdentifierSyntax
//@[04:019) |   |   | | └─Token(Identifier) |bootDiagnostics|
//@[19:020) |   |   | ├─Token(Colon) |:|
//@[21:100) |   |   | └─ObjectSyntax
//@[21:022) |   |   |   ├─Token(LeftBrace) |{|
//@[22:024) |   |   |   ├─Token(NewLine) |\r\n|
      enabled: 123
//@[06:018) |   |   |   ├─ObjectPropertySyntax
//@[06:013) |   |   |   | ├─IdentifierSyntax
//@[06:013) |   |   |   | | └─Token(Identifier) |enabled|
//@[13:014) |   |   |   | ├─Token(Colon) |:|
//@[15:018) |   |   |   | └─IntegerLiteralSyntax
//@[15:018) |   |   |   |   └─Token(Integer) |123|
//@[18:020) |   |   |   ├─Token(NewLine) |\r\n|
      storageUri: true
//@[06:022) |   |   |   ├─ObjectPropertySyntax
//@[06:016) |   |   |   | ├─IdentifierSyntax
//@[06:016) |   |   |   | | └─Token(Identifier) |storageUri|
//@[16:017) |   |   |   | ├─Token(Colon) |:|
//@[18:022) |   |   |   | └─BooleanLiteralSyntax
//@[18:022) |   |   |   |   └─Token(TrueKeyword) |true|
//@[22:024) |   |   |   ├─Token(NewLine) |\r\n|
      unknownProp: 'asdf'
//@[06:025) |   |   |   ├─ObjectPropertySyntax
//@[06:017) |   |   |   | ├─IdentifierSyntax
//@[06:017) |   |   |   | | └─Token(Identifier) |unknownProp|
//@[17:018) |   |   |   | ├─Token(Colon) |:|
//@[19:025) |   |   |   | └─StringSyntax
//@[19:025) |   |   |   |   └─Token(StringComplete) |'asdf'|
//@[25:027) |   |   |   ├─Token(NewLine) |\r\n|
    }
//@[04:005) |   |   |   └─Token(RightBrace) |}|
//@[05:007) |   |   ├─Token(NewLine) |\r\n|
  }
//@[02:003) |   |   └─Token(RightBrace) |}|
//@[03:005) |   ├─Token(NewLine) |\r\n|
  evictionPolicy: 'Deallocate'
//@[02:030) |   ├─ObjectPropertySyntax
//@[02:016) |   | ├─IdentifierSyntax
//@[02:016) |   | | └─Token(Identifier) |evictionPolicy|
//@[16:017) |   | ├─Token(Colon) |:|
//@[18:030) |   | └─StringSyntax
//@[18:030) |   |   └─Token(StringComplete) |'Deallocate'|
//@[30:032) |   ├─Token(NewLine) |\r\n|
}
//@[00:001) |   └─Token(RightBrace) |}|
//@[01:003) ├─Token(NewLine) |\r\n|
resource vm 'Microsoft.Compute/virtualMachines@2020-12-01' = {
//@[00:164) ├─ResourceDeclarationSyntax
//@[00:008) | ├─Token(Identifier) |resource|
//@[09:011) | ├─IdentifierSyntax
//@[09:011) | | └─Token(Identifier) |vm|
//@[12:058) | ├─StringSyntax
//@[12:058) | | └─Token(StringComplete) |'Microsoft.Compute/virtualMachines@2020-12-01'|
//@[59:060) | ├─Token(Assignment) |=|
//@[61:164) | └─ObjectSyntax
//@[61:062) |   ├─Token(LeftBrace) |{|
//@[62:064) |   ├─Token(NewLine) |\r\n|
  name: 'vm'
//@[02:012) |   ├─ObjectPropertySyntax
//@[02:006) |   | ├─IdentifierSyntax
//@[02:006) |   | | └─Token(Identifier) |name|
//@[06:007) |   | ├─Token(Colon) |:|
//@[08:012) |   | └─StringSyntax
//@[08:012) |   |   └─Token(StringComplete) |'vm'|
//@[12:014) |   ├─Token(NewLine) |\r\n|
  location: 'West US'
//@[02:021) |   ├─ObjectPropertySyntax
//@[02:010) |   | ├─IdentifierSyntax
//@[02:010) |   | | └─Token(Identifier) |location|
//@[10:011) |   | ├─Token(Colon) |:|
//@[12:021) |   | └─StringSyntax
//@[12:021) |   |   └─Token(StringComplete) |'West US'|
//@[21:023) |   ├─Token(NewLine) |\r\n|
#disable-next-line BCP036 BCP037
//@[32:034) |   ├─Token(NewLine) |\r\n|
  properties: vmProperties
//@[02:026) |   ├─ObjectPropertySyntax
//@[02:012) |   | ├─IdentifierSyntax
//@[02:012) |   | | └─Token(Identifier) |properties|
//@[12:013) |   | ├─Token(Colon) |:|
//@[14:026) |   | └─VariableAccessSyntax
//@[14:026) |   |   └─IdentifierSyntax
//@[14:026) |   |     └─Token(Identifier) |vmProperties|
//@[26:028) |   ├─Token(NewLine) |\r\n|
}
//@[00:001) |   └─Token(RightBrace) |}|
//@[01:003) ├─Token(NewLine) |\r\n|
#disable-next-line no-unused-params
//@[35:037) ├─Token(NewLine) |\r\n|
param storageAccount1 string = 'testStorageAccount'
//@[00:051) ├─ParameterDeclarationSyntax
//@[00:005) | ├─Token(Identifier) |param|
//@[06:021) | ├─IdentifierSyntax
//@[06:021) | | └─Token(Identifier) |storageAccount1|
//@[22:028) | ├─SimpleTypeSyntax
//@[22:028) | | └─Token(Identifier) |string|
//@[29:051) | └─ParameterDefaultValueSyntax
//@[29:030) |   ├─Token(Assignment) |=|
//@[31:051) |   └─StringSyntax
//@[31:051) |     └─Token(StringComplete) |'testStorageAccount'|
//@[51:053) ├─Token(NewLine) |\r\n|
#disable-next-line          no-unused-params
//@[44:046) ├─Token(NewLine) |\r\n|
param storageAccount2 string = 'testStorageAccount'
//@[00:051) ├─ParameterDeclarationSyntax
//@[00:005) | ├─Token(Identifier) |param|
//@[06:021) | ├─IdentifierSyntax
//@[06:021) | | └─Token(Identifier) |storageAccount2|
//@[22:028) | ├─SimpleTypeSyntax
//@[22:028) | | └─Token(Identifier) |string|
//@[29:051) | └─ParameterDefaultValueSyntax
//@[29:030) |   ├─Token(Assignment) |=|
//@[31:051) |   └─StringSyntax
//@[31:051) |     └─Token(StringComplete) |'testStorageAccount'|
//@[51:053) ├─Token(NewLine) |\r\n|
#disable-next-line   no-unused-params                /* Test comment 1 */
//@[73:075) ├─Token(NewLine) |\r\n|
param storageAccount3 string = 'testStorageAccount'
//@[00:051) ├─ParameterDeclarationSyntax
//@[00:005) | ├─Token(Identifier) |param|
//@[06:021) | ├─IdentifierSyntax
//@[06:021) | | └─Token(Identifier) |storageAccount3|
//@[22:028) | ├─SimpleTypeSyntax
//@[22:028) | | └─Token(Identifier) |string|
//@[29:051) | └─ParameterDefaultValueSyntax
//@[29:030) |   ├─Token(Assignment) |=|
//@[31:051) |   └─StringSyntax
//@[31:051) |     └─Token(StringComplete) |'testStorageAccount'|
//@[51:053) ├─Token(NewLine) |\r\n|
         #disable-next-line   no-unused-params                // Test comment 2
//@[79:081) ├─Token(NewLine) |\r\n|
param storageAccount5 string = 'testStorageAccount'
//@[00:051) ├─ParameterDeclarationSyntax
//@[00:005) | ├─Token(Identifier) |param|
//@[06:021) | ├─IdentifierSyntax
//@[06:021) | | └─Token(Identifier) |storageAccount5|
//@[22:028) | ├─SimpleTypeSyntax
//@[22:028) | | └─Token(Identifier) |string|
//@[29:051) | └─ParameterDefaultValueSyntax
//@[29:030) |   ├─Token(Assignment) |=|
//@[31:051) |   └─StringSyntax
//@[31:051) |     └─Token(StringComplete) |'testStorageAccount'|
//@[51:051) └─Token(EndOfFile) ||
