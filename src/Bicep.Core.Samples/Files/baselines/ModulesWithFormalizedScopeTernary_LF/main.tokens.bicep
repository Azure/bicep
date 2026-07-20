// REP 0015: the classic "hard" scope expression. A conditional (ternary) scope that today fails with
//@[101:102) NewLine |\n|
// BCP420 ("scope could not be resolved at compile time") now compiles: both branches are ResourceScope
//@[103:104) NewLine |\n|
// members sharing the 'resourceGroup' discriminator, so the whole expression is emitted verbatim into
//@[102:103) NewLine |\n|
// "@scope" and the deployment engine resolves it at deploy time.
//@[065:066) NewLine |\n|
param otherResourceGroup string = ''
//@[000:005) Identifier |param|
//@[006:024) Identifier |otherResourceGroup|
//@[025:031) Identifier |string|
//@[032:033) Assignment |=|
//@[034:036) StringComplete |''|
//@[036:038) NewLine |\n\n|

module mod 'modules/mod.bicep' = {
//@[000:006) Identifier |module|
//@[007:010) Identifier |mod|
//@[011:030) StringComplete |'modules/mod.bicep'|
//@[031:032) Assignment |=|
//@[033:034) LeftBrace |{|
//@[034:035) NewLine |\n|
  name: 'mod'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:013) StringComplete |'mod'|
//@[013:014) NewLine |\n|
  scope: !empty(otherResourceGroup) ? resourceGroup(otherResourceGroup) : resourceGroup()
//@[002:007) Identifier |scope|
//@[007:008) Colon |:|
//@[009:010) Exclamation |!|
//@[010:015) Identifier |empty|
//@[015:016) LeftParen |(|
//@[016:034) Identifier |otherResourceGroup|
//@[034:035) RightParen |)|
//@[036:037) Question |?|
//@[038:051) Identifier |resourceGroup|
//@[051:052) LeftParen |(|
//@[052:070) Identifier |otherResourceGroup|
//@[070:071) RightParen |)|
//@[072:073) Colon |:|
//@[074:087) Identifier |resourceGroup|
//@[087:088) LeftParen |(|
//@[088:089) RightParen |)|
//@[089:090) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:002) NewLine |\n|

//@[000:000) EndOfFile ||
