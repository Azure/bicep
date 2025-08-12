using 'main.bicep'
//@[000:005) Identifier |using|
//@[006:018) StringComplete |'main.bicep'|
//@[018:020) NewLine |\n\n|

var emptyObjVar = {}
//@[000:003) Identifier |var|
//@[004:015) Identifier |emptyObjVar|
//@[016:017) Assignment |=|
//@[018:019) LeftBrace |{|
//@[019:020) RightBrace |}|
//@[020:021) NewLine |\n|
param strParam1 = 'strParam1Value'
//@[000:005) Identifier |param|
//@[006:015) Identifier |strParam1|
//@[016:017) Assignment |=|
//@[018:034) StringComplete |'strParam1Value'|
//@[034:035) NewLine |\n|
var strVar1 = 'strVar1Value'
//@[000:003) Identifier |var|
//@[004:011) Identifier |strVar1|
//@[012:013) Assignment |=|
//@[014:028) StringComplete |'strVar1Value'|
//@[028:029) NewLine |\n|
param secureStrParam1 = az.getSecret('a', 'b', 'c', 'param')
//@[000:005) Identifier |param|
//@[006:021) Identifier |secureStrParam1|
//@[022:023) Assignment |=|
//@[024:026) Identifier |az|
//@[026:027) Dot |.|
//@[027:036) Identifier |getSecret|
//@[036:037) LeftParen |(|
//@[037:040) StringComplete |'a'|
//@[040:041) Comma |,|
//@[042:045) StringComplete |'b'|
//@[045:046) Comma |,|
//@[047:050) StringComplete |'c'|
//@[050:051) Comma |,|
//@[052:059) StringComplete |'param'|
//@[059:060) RightParen |)|
//@[060:061) NewLine |\n|
var secureStrVar1 = az.getSecret('a', 'b', 'c', 'var')
//@[000:003) Identifier |var|
//@[004:017) Identifier |secureStrVar1|
//@[018:019) Assignment |=|
//@[020:022) Identifier |az|
//@[022:023) Dot |.|
//@[023:032) Identifier |getSecret|
//@[032:033) LeftParen |(|
//@[033:036) StringComplete |'a'|
//@[036:037) Comma |,|
//@[038:041) StringComplete |'b'|
//@[041:042) Comma |,|
//@[043:046) StringComplete |'c'|
//@[046:047) Comma |,|
//@[048:053) StringComplete |'var'|
//@[053:054) RightParen |)|
//@[054:056) NewLine |\n\n|

extensionConfig validAssignment1 with {
//@[000:015) Identifier |extensionConfig|
//@[016:032) Identifier |validAssignment1|
//@[033:037) Identifier |with|
//@[038:039) LeftBrace |{|
//@[039:040) NewLine |\n|
  requiredString: 'value'
//@[002:016) Identifier |requiredString|
//@[016:017) Colon |:|
//@[018:025) StringComplete |'value'|
//@[025:026) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

extensionConfig validSecretAssignment1 with {
//@[000:015) Identifier |extensionConfig|
//@[016:038) Identifier |validSecretAssignment1|
//@[039:043) Identifier |with|
//@[044:045) LeftBrace |{|
//@[045:046) NewLine |\n|
  requiredSecureString: az.getSecret('a', 'b', 'c', 'valid')
//@[002:022) Identifier |requiredSecureString|
//@[022:023) Colon |:|
//@[024:026) Identifier |az|
//@[026:027) Dot |.|
//@[027:036) Identifier |getSecret|
//@[036:037) LeftParen |(|
//@[037:040) StringComplete |'a'|
//@[040:041) Comma |,|
//@[042:045) StringComplete |'b'|
//@[045:046) Comma |,|
//@[047:050) StringComplete |'c'|
//@[050:051) Comma |,|
//@[052:059) StringComplete |'valid'|
//@[059:060) RightParen |)|
//@[060:061) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

param invalidParamAssignment1 = validAssignment1.requiredString
//@[000:005) Identifier |param|
//@[006:029) Identifier |invalidParamAssignment1|
//@[030:031) Assignment |=|
//@[032:048) Identifier |validAssignment1|
//@[048:049) Dot |.|
//@[049:063) Identifier |requiredString|
//@[063:064) NewLine |\n|
param invalidParamAssignment2 = validAssignment1
//@[000:005) Identifier |param|
//@[006:029) Identifier |invalidParamAssignment2|
//@[030:031) Assignment |=|
//@[032:048) Identifier |validAssignment1|
//@[048:049) NewLine |\n|
param invalidParamAssignment3 = validSecretAssignment1.requiredSecureString
//@[000:005) Identifier |param|
//@[006:029) Identifier |invalidParamAssignment3|
//@[030:031) Assignment |=|
//@[032:054) Identifier |validSecretAssignment1|
//@[054:055) Dot |.|
//@[055:075) Identifier |requiredSecureString|
//@[075:077) NewLine |\n\n|

var invalidVarAssignment1 = validAssignment1.requiredString
//@[000:003) Identifier |var|
//@[004:025) Identifier |invalidVarAssignment1|
//@[026:027) Assignment |=|
//@[028:044) Identifier |validAssignment1|
//@[044:045) Dot |.|
//@[045:059) Identifier |requiredString|
//@[059:060) NewLine |\n|
var invalidVarAssignment2 = validAssignment1
//@[000:003) Identifier |var|
//@[004:025) Identifier |invalidVarAssignment2|
//@[026:027) Assignment |=|
//@[028:044) Identifier |validAssignment1|
//@[044:045) NewLine |\n|
var invalidVarAssignment3 = validSecretAssignment1.requiredSecureString
//@[000:003) Identifier |var|
//@[004:025) Identifier |invalidVarAssignment3|
//@[026:027) Assignment |=|
//@[028:050) Identifier |validSecretAssignment1|
//@[050:051) Dot |.|
//@[051:071) Identifier |requiredSecureString|
//@[071:073) NewLine |\n\n|

extensionConfig
//@[000:015) Identifier |extensionConfig|
//@[015:017) NewLine |\n\n|

extensionConfig incompleteAssignment1
//@[000:015) Identifier |extensionConfig|
//@[016:037) Identifier |incompleteAssignment1|
//@[037:038) NewLine |\n|
extensionConfig incompleteAssignment2 with
//@[000:015) Identifier |extensionConfig|
//@[016:037) Identifier |incompleteAssignment2|
//@[038:042) Identifier |with|
//@[042:044) NewLine |\n\n|

extensionConfig hasNoConfig with {}
//@[000:015) Identifier |extensionConfig|
//@[016:027) Identifier |hasNoConfig|
//@[028:032) Identifier |with|
//@[033:034) LeftBrace |{|
//@[034:035) RightBrace |}|
//@[035:037) NewLine |\n\n|

extensionConfig invalidSyntax1 = emptyObjVar
//@[000:015) Identifier |extensionConfig|
//@[016:030) Identifier |invalidSyntax1|
//@[031:032) Assignment |=|
//@[033:044) Identifier |emptyObjVar|
//@[044:045) NewLine |\n|
extensionConfig invalidSyntax2 with emptyObjVar
//@[000:015) Identifier |extensionConfig|
//@[016:030) Identifier |invalidSyntax2|
//@[031:035) Identifier |with|
//@[036:047) Identifier |emptyObjVar|
//@[047:048) NewLine |\n|
extensionConfig invalidSyntax3 with {
//@[000:015) Identifier |extensionConfig|
//@[016:030) Identifier |invalidSyntax3|
//@[031:035) Identifier |with|
//@[036:037) LeftBrace |{|
//@[037:038) NewLine |\n|
  ...emptyObjVar
//@[002:005) Ellipsis |...|
//@[005:016) Identifier |emptyObjVar|
//@[016:017) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

extensionConfig invalidSyntax4 with {
//@[000:015) Identifier |extensionConfig|
//@[016:030) Identifier |invalidSyntax4|
//@[031:035) Identifier |with|
//@[036:037) LeftBrace |{|
//@[037:038) NewLine |\n|
  requiredString: validAssignment1.requiredString
//@[002:016) Identifier |requiredString|
//@[016:017) Colon |:|
//@[018:034) Identifier |validAssignment1|
//@[034:035) Dot |.|
//@[035:049) Identifier |requiredString|
//@[049:050) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

extensionConfig invalidSyntax5 with {
//@[000:015) Identifier |extensionConfig|
//@[016:030) Identifier |invalidSyntax5|
//@[031:035) Identifier |with|
//@[036:037) LeftBrace |{|
//@[037:038) NewLine |\n|
  ...validAssignment1
//@[002:005) Ellipsis |...|
//@[005:021) Identifier |validAssignment1|
//@[021:022) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

extensionConfig invalidAssignment1 with {
//@[000:015) Identifier |extensionConfig|
//@[016:034) Identifier |invalidAssignment1|
//@[035:039) Identifier |with|
//@[040:041) LeftBrace |{|
//@[041:042) NewLine |\n|
  requiredString: strParam1
//@[002:016) Identifier |requiredString|
//@[016:017) Colon |:|
//@[018:027) Identifier |strParam1|
//@[027:028) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

extensionConfig invalidAssignment2 with {
//@[000:015) Identifier |extensionConfig|
//@[016:034) Identifier |invalidAssignment2|
//@[035:039) Identifier |with|
//@[040:041) LeftBrace |{|
//@[041:042) NewLine |\n|
  requiredString: strVar1
//@[002:016) Identifier |requiredString|
//@[016:017) Colon |:|
//@[018:025) Identifier |strVar1|
//@[025:026) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

extensionConfig invalidSecretAssignment1 with {
//@[000:015) Identifier |extensionConfig|
//@[016:040) Identifier |invalidSecretAssignment1|
//@[041:045) Identifier |with|
//@[046:047) LeftBrace |{|
//@[047:048) NewLine |\n|
  requiredSecureString: bool(readEnvironmentVariable('xyz', 'false')) ? az.getSecret('a', 'b', 'c', 'd') : az.getSecret('w', 'x', 'y', 'z')
//@[002:022) Identifier |requiredSecureString|
//@[022:023) Colon |:|
//@[024:028) Identifier |bool|
//@[028:029) LeftParen |(|
//@[029:052) Identifier |readEnvironmentVariable|
//@[052:053) LeftParen |(|
//@[053:058) StringComplete |'xyz'|
//@[058:059) Comma |,|
//@[060:067) StringComplete |'false'|
//@[067:068) RightParen |)|
//@[068:069) RightParen |)|
//@[070:071) Question |?|
//@[072:074) Identifier |az|
//@[074:075) Dot |.|
//@[075:084) Identifier |getSecret|
//@[084:085) LeftParen |(|
//@[085:088) StringComplete |'a'|
//@[088:089) Comma |,|
//@[090:093) StringComplete |'b'|
//@[093:094) Comma |,|
//@[095:098) StringComplete |'c'|
//@[098:099) Comma |,|
//@[100:103) StringComplete |'d'|
//@[103:104) RightParen |)|
//@[105:106) Colon |:|
//@[107:109) Identifier |az|
//@[109:110) Dot |.|
//@[110:119) Identifier |getSecret|
//@[119:120) LeftParen |(|
//@[120:123) StringComplete |'w'|
//@[123:124) Comma |,|
//@[125:128) StringComplete |'x'|
//@[128:129) Comma |,|
//@[130:133) StringComplete |'y'|
//@[133:134) Comma |,|
//@[135:138) StringComplete |'z'|
//@[138:139) RightParen |)|
//@[139:140) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

extensionConfig invalidSecretAssignment2 with {
//@[000:015) Identifier |extensionConfig|
//@[016:040) Identifier |invalidSecretAssignment2|
//@[041:045) Identifier |with|
//@[046:047) LeftBrace |{|
//@[047:048) NewLine |\n|
  requiredSecureString: secureStrParam1
//@[002:022) Identifier |requiredSecureString|
//@[022:023) Colon |:|
//@[024:039) Identifier |secureStrParam1|
//@[039:040) NewLine |\n|
  optionalString: secureStrParam1
//@[002:016) Identifier |optionalString|
//@[016:017) Colon |:|
//@[018:033) Identifier |secureStrParam1|
//@[033:034) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

extensionConfig invalidSecretAssignment3 with {
//@[000:015) Identifier |extensionConfig|
//@[016:040) Identifier |invalidSecretAssignment3|
//@[041:045) Identifier |with|
//@[046:047) LeftBrace |{|
//@[047:048) NewLine |\n|
  requiredSecureString: secureStrVar1
//@[002:022) Identifier |requiredSecureString|
//@[022:023) Colon |:|
//@[024:037) Identifier |secureStrVar1|
//@[037:038) NewLine |\n|
  optionalString: secureStrVar1
//@[002:016) Identifier |optionalString|
//@[016:017) Colon |:|
//@[018:031) Identifier |secureStrVar1|
//@[031:032) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

extensionConfig invalidDiscrimAssignment1 with {
//@[000:015) Identifier |extensionConfig|
//@[016:041) Identifier |invalidDiscrimAssignment1|
//@[042:046) Identifier |with|
//@[047:048) LeftBrace |{|
//@[048:049) NewLine |\n|
  discrim: 'a' // this property cannot be reassigned
//@[002:009) Identifier |discrim|
//@[009:010) Colon |:|
//@[011:014) StringComplete |'a'|
//@[052:053) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:002) NewLine |\n|

//@[000:000) EndOfFile ||
