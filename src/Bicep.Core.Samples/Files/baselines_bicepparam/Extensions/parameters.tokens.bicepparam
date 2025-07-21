using 'main.bicep'
//@[000:005) Identifier |using|
//@[006:018) StringComplete |'main.bicep'|
//@[018:020) NewLine |\n\n|

var strVar1 = 'strVar1Value'
//@[000:003) Identifier |var|
//@[004:011) Identifier |strVar1|
//@[012:013) Assignment |=|
//@[014:028) StringComplete |'strVar1Value'|
//@[028:029) NewLine |\n|
param strParam1 = 'strParam1Value'
//@[000:005) Identifier |param|
//@[006:015) Identifier |strParam1|
//@[016:017) Assignment |=|
//@[018:034) StringComplete |'strParam1Value'|
//@[034:035) NewLine |\n|
param secureStrParam1 = az.getSecret('p', 'r', 'a', 'm')
//@[000:005) Identifier |param|
//@[006:021) Identifier |secureStrParam1|
//@[022:023) Assignment |=|
//@[024:026) Identifier |az|
//@[026:027) Dot |.|
//@[027:036) Identifier |getSecret|
//@[036:037) LeftParen |(|
//@[037:040) StringComplete |'p'|
//@[040:041) Comma |,|
//@[042:045) StringComplete |'r'|
//@[045:046) Comma |,|
//@[047:050) StringComplete |'a'|
//@[050:051) Comma |,|
//@[052:055) StringComplete |'m'|
//@[055:056) RightParen |)|
//@[056:058) NewLine |\n\n|

extensionConfig hasObjConfig1 with {
//@[000:015) Identifier |extensionConfig|
//@[016:029) Identifier |hasObjConfig1|
//@[030:034) Identifier |with|
//@[035:036) LeftBrace |{|
//@[036:037) NewLine |\n|
  requiredString: 'valueFromParams'
//@[002:016) Identifier |requiredString|
//@[016:017) Colon |:|
//@[018:035) StringComplete |'valueFromParams'|
//@[035:036) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

extensionConfig hasObjConfig2 with {
//@[000:015) Identifier |extensionConfig|
//@[016:029) Identifier |hasObjConfig2|
//@[030:034) Identifier |with|
//@[035:036) LeftBrace |{|
//@[036:037) NewLine |\n|
  optionalString: 'optional'
//@[002:016) Identifier |optionalString|
//@[016:017) Colon |:|
//@[018:028) StringComplete |'optional'|
//@[028:029) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

extensionConfig hasObjConfig3 with {}
//@[000:015) Identifier |extensionConfig|
//@[016:029) Identifier |hasObjConfig3|
//@[030:034) Identifier |with|
//@[035:036) LeftBrace |{|
//@[036:037) RightBrace |}|
//@[037:039) NewLine |\n\n|

// hasObjConfig4 not here to test assignment is not required because required field is defaulted
//@[096:098) NewLine |\n\n|

extensionConfig hasObjConfig5 with {
//@[000:015) Identifier |extensionConfig|
//@[016:029) Identifier |hasObjConfig5|
//@[030:034) Identifier |with|
//@[035:036) LeftBrace |{|
//@[036:037) NewLine |\n|
  requiredString: strVar1
//@[002:016) Identifier |requiredString|
//@[016:017) Colon |:|
//@[018:025) Identifier |strVar1|
//@[025:026) NewLine |\n|
  optionalString: bool(readEnvironmentVariable('xyz', 'false')) ? 'inlineVal' : strVar1
//@[002:016) Identifier |optionalString|
//@[016:017) Colon |:|
//@[018:022) Identifier |bool|
//@[022:023) LeftParen |(|
//@[023:046) Identifier |readEnvironmentVariable|
//@[046:047) LeftParen |(|
//@[047:052) StringComplete |'xyz'|
//@[052:053) Comma |,|
//@[054:061) StringComplete |'false'|
//@[061:062) RightParen |)|
//@[062:063) RightParen |)|
//@[064:065) Question |?|
//@[066:077) StringComplete |'inlineVal'|
//@[078:079) Colon |:|
//@[080:087) Identifier |strVar1|
//@[087:088) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:004) NewLine |\n\n\n|


extensionConfig hasSecureConfig1 with {
//@[000:015) Identifier |extensionConfig|
//@[016:032) Identifier |hasSecureConfig1|
//@[033:037) Identifier |with|
//@[038:039) LeftBrace |{|
//@[039:040) NewLine |\n|
  requiredSecureString: az.getSecret('00000000-0000-0000-0000-000000000001', 'mock-rg', 'kv', 'mySecret')
//@[002:022) Identifier |requiredSecureString|
//@[022:023) Colon |:|
//@[024:026) Identifier |az|
//@[026:027) Dot |.|
//@[027:036) Identifier |getSecret|
//@[036:037) LeftParen |(|
//@[037:075) StringComplete |'00000000-0000-0000-0000-000000000001'|
//@[075:076) Comma |,|
//@[077:086) StringComplete |'mock-rg'|
//@[086:087) Comma |,|
//@[088:092) StringComplete |'kv'|
//@[092:093) Comma |,|
//@[094:104) StringComplete |'mySecret'|
//@[104:105) RightParen |)|
//@[105:106) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

extensionConfig hasSecureConfig2 with {
//@[000:015) Identifier |extensionConfig|
//@[016:032) Identifier |hasSecureConfig2|
//@[033:037) Identifier |with|
//@[038:039) LeftBrace |{|
//@[039:040) NewLine |\n|
  requiredSecureString: 'Inlined'
//@[002:022) Identifier |requiredSecureString|
//@[022:023) Colon |:|
//@[024:033) StringComplete |'Inlined'|
//@[033:034) NewLine |\n|
  optionalString: 'valueFromParams'
//@[002:016) Identifier |optionalString|
//@[016:017) Colon |:|
//@[018:035) StringComplete |'valueFromParams'|
//@[035:036) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

extensionConfig hasSecureConfig3 with {
//@[000:015) Identifier |extensionConfig|
//@[016:032) Identifier |hasSecureConfig3|
//@[033:037) Identifier |with|
//@[038:039) LeftBrace |{|
//@[039:040) NewLine |\n|
  requiredSecureString: strVar1
//@[002:022) Identifier |requiredSecureString|
//@[022:023) Colon |:|
//@[024:031) Identifier |strVar1|
//@[031:032) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

extensionConfig hasSecureConfig4 with {
//@[000:015) Identifier |extensionConfig|
//@[016:032) Identifier |hasSecureConfig4|
//@[033:037) Identifier |with|
//@[038:039) LeftBrace |{|
//@[039:040) NewLine |\n|
  requiredSecureString: strParam1
//@[002:022) Identifier |requiredSecureString|
//@[022:023) Colon |:|
//@[024:033) Identifier |strParam1|
//@[033:034) NewLine |\n|
  optionalString: strParam1
//@[002:016) Identifier |optionalString|
//@[016:017) Colon |:|
//@[018:027) Identifier |strParam1|
//@[027:028) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

extensionConfig hasDiscrimConfig1 with {
//@[000:015) Identifier |extensionConfig|
//@[016:033) Identifier |hasDiscrimConfig1|
//@[034:038) Identifier |with|
//@[039:040) LeftBrace |{|
//@[040:041) NewLine |\n|
  discrim: 'a'
//@[002:009) Identifier |discrim|
//@[009:010) Colon |:|
//@[011:014) StringComplete |'a'|
//@[014:015) NewLine |\n|
  z1: 'z1v'
//@[002:004) Identifier |z1|
//@[004:005) Colon |:|
//@[006:011) StringComplete |'z1v'|
//@[011:012) NewLine |\n|
  a1: 'a1v'
//@[002:004) Identifier |a1|
//@[004:005) Colon |:|
//@[006:011) StringComplete |'a1v'|
//@[011:012) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

extensionConfig hasDiscrimConfig2 with {
//@[000:015) Identifier |extensionConfig|
//@[016:033) Identifier |hasDiscrimConfig2|
//@[034:038) Identifier |with|
//@[039:040) LeftBrace |{|
//@[040:041) NewLine |\n|
  a1: 'a1v'
//@[002:004) Identifier |a1|
//@[004:005) Colon |:|
//@[006:011) StringComplete |'a1v'|
//@[011:012) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

extensionConfig hasDiscrimConfig3 with {
//@[000:015) Identifier |extensionConfig|
//@[016:033) Identifier |hasDiscrimConfig3|
//@[034:038) Identifier |with|
//@[039:040) LeftBrace |{|
//@[040:041) NewLine |\n|
  discrim: 'b'
//@[002:009) Identifier |discrim|
//@[009:010) Colon |:|
//@[011:014) StringComplete |'b'|
//@[014:015) NewLine |\n|
  b1: bool(readEnvironmentVariable('xyz', 'false')) ? 'b1True' : 'b1False'
//@[002:004) Identifier |b1|
//@[004:005) Colon |:|
//@[006:010) Identifier |bool|
//@[010:011) LeftParen |(|
//@[011:034) Identifier |readEnvironmentVariable|
//@[034:035) LeftParen |(|
//@[035:040) StringComplete |'xyz'|
//@[040:041) Comma |,|
//@[042:049) StringComplete |'false'|
//@[049:050) RightParen |)|
//@[050:051) RightParen |)|
//@[052:053) Question |?|
//@[054:062) StringComplete |'b1True'|
//@[063:064) Colon |:|
//@[065:074) StringComplete |'b1False'|
//@[074:075) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:002) NewLine |\n|

//@[000:000) EndOfFile ||
