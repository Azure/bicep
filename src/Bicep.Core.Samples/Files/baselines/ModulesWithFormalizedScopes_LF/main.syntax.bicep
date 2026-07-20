targetScope = 'subscription'
//@[00:655) ProgramSyntax
//@[00:028) ├─TargetScopeSyntax
//@[00:011) | ├─Token(Identifier) |targetScope|
//@[12:013) | ├─Token(Assignment) |=|
//@[14:028) | └─StringSyntax
//@[14:028) |   └─Token(StringComplete) |'subscription'|
//@[28:030) ├─Token(NewLine) |\n\n|

param location string = 'eastus'
//@[00:032) ├─ParameterDeclarationSyntax
//@[00:005) | ├─Token(Identifier) |param|
//@[06:014) | ├─IdentifierSyntax
//@[06:014) | | └─Token(Identifier) |location|
//@[15:021) | ├─TypeVariableAccessSyntax
//@[15:021) | | └─IdentifierSyntax
//@[15:021) | |   └─Token(Identifier) |string|
//@[22:032) | └─ParameterDefaultValueSyntax
//@[22:023) |   ├─Token(Assignment) |=|
//@[24:032) |   └─StringSyntax
//@[24:032) |     └─Token(StringComplete) |'eastus'|
//@[32:034) ├─Token(NewLine) |\n\n|

// REP 0015: with the 'formalizedScope' experimental feature enabled, this module's cross-scope
//@[95:096) ├─Token(NewLine) |\n|
// targeting is emitted as a single duck-typed "@scope" object instead of the legacy
//@[84:085) ├─Token(NewLine) |\n|
// "subscriptionId" / "resourceGroup" properties.
//@[49:050) ├─Token(NewLine) |\n|
module storageMod 'modules/mod.bicep' = {
//@[00:135) ├─ModuleDeclarationSyntax
//@[00:006) | ├─Token(Identifier) |module|
//@[07:017) | ├─IdentifierSyntax
//@[07:017) | | └─Token(Identifier) |storageMod|
//@[18:037) | ├─StringSyntax
//@[18:037) | | └─Token(StringComplete) |'modules/mod.bicep'|
//@[38:039) | ├─Token(Assignment) |=|
//@[40:135) | └─ObjectSyntax
//@[40:041) |   ├─Token(LeftBrace) |{|
//@[41:042) |   ├─Token(NewLine) |\n|
  name: 'storageMod'
//@[02:020) |   ├─ObjectPropertySyntax
//@[02:006) |   | ├─IdentifierSyntax
//@[02:006) |   | | └─Token(Identifier) |name|
//@[06:007) |   | ├─Token(Colon) |:|
//@[08:020) |   | └─StringSyntax
//@[08:020) |   |   └─Token(StringComplete) |'storageMod'|
//@[20:021) |   ├─Token(NewLine) |\n|
  scope: resourceGroup('my-rg')
//@[02:031) |   ├─ObjectPropertySyntax
//@[02:007) |   | ├─IdentifierSyntax
//@[02:007) |   | | └─Token(Identifier) |scope|
//@[07:008) |   | ├─Token(Colon) |:|
//@[09:031) |   | └─FunctionCallSyntax
//@[09:022) |   |   ├─IdentifierSyntax
//@[09:022) |   |   | └─Token(Identifier) |resourceGroup|
//@[22:023) |   |   ├─Token(LeftParen) |(|
//@[23:030) |   |   ├─FunctionArgumentSyntax
//@[23:030) |   |   | └─StringSyntax
//@[23:030) |   |   |   └─Token(StringComplete) |'my-rg'|
//@[30:031) |   |   └─Token(RightParen) |)|
//@[31:032) |   ├─Token(NewLine) |\n|
  params: {
//@[02:038) |   ├─ObjectPropertySyntax
//@[02:008) |   | ├─IdentifierSyntax
//@[02:008) |   | | └─Token(Identifier) |params|
//@[08:009) |   | ├─Token(Colon) |:|
//@[10:038) |   | └─ObjectSyntax
//@[10:011) |   |   ├─Token(LeftBrace) |{|
//@[11:012) |   |   ├─Token(NewLine) |\n|
    location: location
//@[04:022) |   |   ├─ObjectPropertySyntax
//@[04:012) |   |   | ├─IdentifierSyntax
//@[04:012) |   |   | | └─Token(Identifier) |location|
//@[12:013) |   |   | ├─Token(Colon) |:|
//@[14:022) |   |   | └─VariableAccessSyntax
//@[14:022) |   |   |   └─IdentifierSyntax
//@[14:022) |   |   |     └─Token(Identifier) |location|
//@[22:023) |   |   ├─Token(NewLine) |\n|
  }
//@[02:003) |   |   └─Token(RightBrace) |}|
//@[03:004) |   ├─Token(NewLine) |\n|
}
//@[00:001) |   └─Token(RightBrace) |}|
//@[01:003) ├─Token(NewLine) |\n\n|

module storageMod2 'modules/mod.bicep' = {
//@[00:178) ├─ModuleDeclarationSyntax
//@[00:006) | ├─Token(Identifier) |module|
//@[07:018) | ├─IdentifierSyntax
//@[07:018) | | └─Token(Identifier) |storageMod2|
//@[19:038) | ├─StringSyntax
//@[19:038) | | └─Token(StringComplete) |'modules/mod.bicep'|
//@[39:040) | ├─Token(Assignment) |=|
//@[41:178) | └─ObjectSyntax
//@[41:042) |   ├─Token(LeftBrace) |{|
//@[42:043) |   ├─Token(NewLine) |\n|
  name: 'storageMod2'
//@[02:021) |   ├─ObjectPropertySyntax
//@[02:006) |   | ├─IdentifierSyntax
//@[02:006) |   | | └─Token(Identifier) |name|
//@[06:007) |   | ├─Token(Colon) |:|
//@[08:021) |   | └─StringSyntax
//@[08:021) |   |   └─Token(StringComplete) |'storageMod2'|
//@[21:022) |   ├─Token(NewLine) |\n|
  scope: location != 'eastus' ? resourceGroup() : resourceGroup('my-rg')
//@[02:072) |   ├─ObjectPropertySyntax
//@[02:007) |   | ├─IdentifierSyntax
//@[02:007) |   | | └─Token(Identifier) |scope|
//@[07:008) |   | ├─Token(Colon) |:|
//@[09:072) |   | └─TernaryOperationSyntax
//@[09:029) |   |   ├─BinaryOperationSyntax
//@[09:017) |   |   | ├─VariableAccessSyntax
//@[09:017) |   |   | | └─IdentifierSyntax
//@[09:017) |   |   | |   └─Token(Identifier) |location|
//@[18:020) |   |   | ├─Token(NotEquals) |!=|
//@[21:029) |   |   | └─StringSyntax
//@[21:029) |   |   |   └─Token(StringComplete) |'eastus'|
//@[30:031) |   |   ├─Token(Question) |?|
//@[32:047) |   |   ├─FunctionCallSyntax
//@[32:045) |   |   | ├─IdentifierSyntax
//@[32:045) |   |   | | └─Token(Identifier) |resourceGroup|
//@[45:046) |   |   | ├─Token(LeftParen) |(|
//@[46:047) |   |   | └─Token(RightParen) |)|
//@[48:049) |   |   ├─Token(Colon) |:|
//@[50:072) |   |   └─FunctionCallSyntax
//@[50:063) |   |     ├─IdentifierSyntax
//@[50:063) |   |     | └─Token(Identifier) |resourceGroup|
//@[63:064) |   |     ├─Token(LeftParen) |(|
//@[64:071) |   |     ├─FunctionArgumentSyntax
//@[64:071) |   |     | └─StringSyntax
//@[64:071) |   |     |   └─Token(StringComplete) |'my-rg'|
//@[71:072) |   |     └─Token(RightParen) |)|
//@[72:073) |   ├─Token(NewLine) |\n|
  params: {
//@[02:038) |   ├─ObjectPropertySyntax
//@[02:008) |   | ├─IdentifierSyntax
//@[02:008) |   | | └─Token(Identifier) |params|
//@[08:009) |   | ├─Token(Colon) |:|
//@[10:038) |   | └─ObjectSyntax
//@[10:011) |   |   ├─Token(LeftBrace) |{|
//@[11:012) |   |   ├─Token(NewLine) |\n|
    location: location
//@[04:022) |   |   ├─ObjectPropertySyntax
//@[04:012) |   |   | ├─IdentifierSyntax
//@[04:012) |   |   | | └─Token(Identifier) |location|
//@[12:013) |   |   | ├─Token(Colon) |:|
//@[14:022) |   |   | └─VariableAccessSyntax
//@[14:022) |   |   |   └─IdentifierSyntax
//@[14:022) |   |   |     └─Token(Identifier) |location|
//@[22:023) |   |   ├─Token(NewLine) |\n|
  }
//@[02:003) |   |   └─Token(RightBrace) |}|
//@[03:004) |   ├─Token(NewLine) |\n|
}
//@[00:001) |   └─Token(RightBrace) |}|
//@[01:003) ├─Token(NewLine) |\n\n|

output loc string = storageMod.outputs.loc
//@[00:042) ├─OutputDeclarationSyntax
//@[00:006) | ├─Token(Identifier) |output|
//@[07:010) | ├─IdentifierSyntax
//@[07:010) | | └─Token(Identifier) |loc|
//@[11:017) | ├─TypeVariableAccessSyntax
//@[11:017) | | └─IdentifierSyntax
//@[11:017) | |   └─Token(Identifier) |string|
//@[18:019) | ├─Token(Assignment) |=|
//@[20:042) | └─PropertyAccessSyntax
//@[20:038) |   ├─PropertyAccessSyntax
//@[20:030) |   | ├─VariableAccessSyntax
//@[20:030) |   | | └─IdentifierSyntax
//@[20:030) |   | |   └─Token(Identifier) |storageMod|
//@[30:031) |   | ├─Token(Dot) |.|
//@[31:038) |   | └─IdentifierSyntax
//@[31:038) |   |   └─Token(Identifier) |outputs|
//@[38:039) |   ├─Token(Dot) |.|
//@[39:042) |   └─IdentifierSyntax
//@[39:042) |     └─Token(Identifier) |loc|
//@[42:043) ├─Token(NewLine) |\n|

//@[00:000) └─Token(EndOfFile) ||
