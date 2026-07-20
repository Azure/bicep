// REP 0015: the classic "hard" scope expression. A conditional (ternary) scope that today fails with
//@[000:554) ProgramSyntax
//@[101:102) ├─Token(NewLine) |\n|
// BCP420 ("scope could not be resolved at compile time") now compiles: both branches are ResourceScope
//@[103:104) ├─Token(NewLine) |\n|
// members sharing the 'resourceGroup' discriminator, so the whole expression is emitted verbatim into
//@[102:103) ├─Token(NewLine) |\n|
// "@scope" and the deployment engine resolves it at deploy time.
//@[065:066) ├─Token(NewLine) |\n|
param otherResourceGroup string = ''
//@[000:036) ├─ParameterDeclarationSyntax
//@[000:005) | ├─Token(Identifier) |param|
//@[006:024) | ├─IdentifierSyntax
//@[006:024) | | └─Token(Identifier) |otherResourceGroup|
//@[025:031) | ├─TypeVariableAccessSyntax
//@[025:031) | | └─IdentifierSyntax
//@[025:031) | |   └─Token(Identifier) |string|
//@[032:036) | └─ParameterDefaultValueSyntax
//@[032:033) |   ├─Token(Assignment) |=|
//@[034:036) |   └─StringSyntax
//@[034:036) |     └─Token(StringComplete) |''|
//@[036:038) ├─Token(NewLine) |\n\n|

module mod 'modules/mod.bicep' = {
//@[000:140) ├─ModuleDeclarationSyntax
//@[000:006) | ├─Token(Identifier) |module|
//@[007:010) | ├─IdentifierSyntax
//@[007:010) | | └─Token(Identifier) |mod|
//@[011:030) | ├─StringSyntax
//@[011:030) | | └─Token(StringComplete) |'modules/mod.bicep'|
//@[031:032) | ├─Token(Assignment) |=|
//@[033:140) | └─ObjectSyntax
//@[033:034) |   ├─Token(LeftBrace) |{|
//@[034:035) |   ├─Token(NewLine) |\n|
  name: 'mod'
//@[002:013) |   ├─ObjectPropertySyntax
//@[002:006) |   | ├─IdentifierSyntax
//@[002:006) |   | | └─Token(Identifier) |name|
//@[006:007) |   | ├─Token(Colon) |:|
//@[008:013) |   | └─StringSyntax
//@[008:013) |   |   └─Token(StringComplete) |'mod'|
//@[013:014) |   ├─Token(NewLine) |\n|
  scope: !empty(otherResourceGroup) ? resourceGroup(otherResourceGroup) : resourceGroup()
//@[002:089) |   ├─ObjectPropertySyntax
//@[002:007) |   | ├─IdentifierSyntax
//@[002:007) |   | | └─Token(Identifier) |scope|
//@[007:008) |   | ├─Token(Colon) |:|
//@[009:089) |   | └─TernaryOperationSyntax
//@[009:035) |   |   ├─UnaryOperationSyntax
//@[009:010) |   |   | ├─Token(Exclamation) |!|
//@[010:035) |   |   | └─FunctionCallSyntax
//@[010:015) |   |   |   ├─IdentifierSyntax
//@[010:015) |   |   |   | └─Token(Identifier) |empty|
//@[015:016) |   |   |   ├─Token(LeftParen) |(|
//@[016:034) |   |   |   ├─FunctionArgumentSyntax
//@[016:034) |   |   |   | └─VariableAccessSyntax
//@[016:034) |   |   |   |   └─IdentifierSyntax
//@[016:034) |   |   |   |     └─Token(Identifier) |otherResourceGroup|
//@[034:035) |   |   |   └─Token(RightParen) |)|
//@[036:037) |   |   ├─Token(Question) |?|
//@[038:071) |   |   ├─FunctionCallSyntax
//@[038:051) |   |   | ├─IdentifierSyntax
//@[038:051) |   |   | | └─Token(Identifier) |resourceGroup|
//@[051:052) |   |   | ├─Token(LeftParen) |(|
//@[052:070) |   |   | ├─FunctionArgumentSyntax
//@[052:070) |   |   | | └─VariableAccessSyntax
//@[052:070) |   |   | |   └─IdentifierSyntax
//@[052:070) |   |   | |     └─Token(Identifier) |otherResourceGroup|
//@[070:071) |   |   | └─Token(RightParen) |)|
//@[072:073) |   |   ├─Token(Colon) |:|
//@[074:089) |   |   └─FunctionCallSyntax
//@[074:087) |   |     ├─IdentifierSyntax
//@[074:087) |   |     | └─Token(Identifier) |resourceGroup|
//@[087:088) |   |     ├─Token(LeftParen) |(|
//@[088:089) |   |     └─Token(RightParen) |)|
//@[089:090) |   ├─Token(NewLine) |\n|
}
//@[000:001) |   └─Token(RightBrace) |}|
//@[001:002) ├─Token(NewLine) |\n|

//@[000:000) └─Token(EndOfFile) ||
