module nonExistentFileRef './nonExistent.bicep' = {
//@[000:006) Identifier |module|
//@[007:025) Identifier |nonExistentFileRef|
//@[026:047) StringComplete |'./nonExistent.bicep'|
//@[048:049) Assignment |=|
//@[050:051) LeftBrace |{|
//@[051:053) NewLine |\n\n|

}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

// we should only look this file up once, but should still return the same failure
//@[082:083) NewLine |\n|
module nonExistentFileRefDuplicate './nonExistent.bicep' = {
//@[000:006) Identifier |module|
//@[007:034) Identifier |nonExistentFileRefDuplicate|
//@[035:056) StringComplete |'./nonExistent.bicep'|
//@[057:058) Assignment |=|
//@[059:060) LeftBrace |{|
//@[060:062) NewLine |\n\n|

}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

// we should only look this file up once, but should still return the same failure
//@[082:083) NewLine |\n|
module nonExistentFileRefEquivalentPath 'abc/def/../../nonExistent.bicep' = {
//@[000:006) Identifier |module|
//@[007:039) Identifier |nonExistentFileRefEquivalentPath|
//@[040:073) StringComplete |'abc/def/../../nonExistent.bicep'|
//@[074:075) Assignment |=|
//@[076:077) LeftBrace |{|
//@[077:079) NewLine |\n\n|

}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

module moduleWithoutPath = {
//@[000:006) Identifier |module|
//@[007:024) Identifier |moduleWithoutPath|
//@[025:026) Assignment |=|
//@[027:028) LeftBrace |{|
//@[028:030) NewLine |\n\n|

}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

// #completionTest(41) -> moduleBodyCompletions
//@[047:048) NewLine |\n|
module moduleWithPath './moduleb.bicep' =
//@[000:006) Identifier |module|
//@[007:021) Identifier |moduleWithPath|
//@[022:039) StringComplete |'./moduleb.bicep'|
//@[040:041) Assignment |=|
//@[041:043) NewLine |\n\n|

// missing identifier #completionTest(7) -> empty
//@[049:050) NewLine |\n|
module 
//@[000:006) Identifier |module|
//@[007:009) NewLine |\n\n|

// #completionTest(24,25) -> moduleObject
//@[041:042) NewLine |\n|
module missingValue '' = 
//@[000:006) Identifier |module|
//@[007:019) Identifier |missingValue|
//@[020:022) StringComplete |''|
//@[023:024) Assignment |=|
//@[025:027) NewLine |\n\n|

var interp = 'hello'
//@[000:003) Identifier |var|
//@[004:010) Identifier |interp|
//@[011:012) Assignment |=|
//@[013:020) StringComplete |'hello'|
//@[020:021) NewLine |\n|
module moduleWithInterpPath './${interp}.bicep' = {
//@[000:006) Identifier |module|
//@[007:027) Identifier |moduleWithInterpPath|
//@[028:033) StringLeftPiece |'./${|
//@[033:039) Identifier |interp|
//@[039:047) StringRightPiece |}.bicep'|
//@[048:049) Assignment |=|
//@[050:051) LeftBrace |{|
//@[051:053) NewLine |\n\n|

}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

module moduleWithConditionAndInterpPath './${interp}.bicep' = if (true) {
//@[000:006) Identifier |module|
//@[007:039) Identifier |moduleWithConditionAndInterpPath|
//@[040:045) StringLeftPiece |'./${|
//@[045:051) Identifier |interp|
//@[051:059) StringRightPiece |}.bicep'|
//@[060:061) Assignment |=|
//@[062:064) Identifier |if|
//@[065:066) LeftParen |(|
//@[066:070) TrueKeyword |true|
//@[070:071) RightParen |)|
//@[072:073) LeftBrace |{|
//@[073:075) NewLine |\n\n|

}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

module moduleWithSelfCycle './main.bicep' = {
//@[000:006) Identifier |module|
//@[007:026) Identifier |moduleWithSelfCycle|
//@[027:041) StringComplete |'./main.bicep'|
//@[042:043) Assignment |=|
//@[044:045) LeftBrace |{|
//@[045:047) NewLine |\n\n|

}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

module moduleWithConditionAndSelfCycle './main.bicep' = if ('foo' == 'bar') {
//@[000:006) Identifier |module|
//@[007:038) Identifier |moduleWithConditionAndSelfCycle|
//@[039:053) StringComplete |'./main.bicep'|
//@[054:055) Assignment |=|
//@[056:058) Identifier |if|
//@[059:060) LeftParen |(|
//@[060:065) StringComplete |'foo'|
//@[066:068) Equals |==|
//@[069:074) StringComplete |'bar'|
//@[074:075) RightParen |)|
//@[076:077) LeftBrace |{|
//@[077:079) NewLine |\n\n|

}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

module './main.bicep' = {
//@[000:006) Identifier |module|
//@[007:021) StringComplete |'./main.bicep'|
//@[022:023) Assignment |=|
//@[024:025) LeftBrace |{|
//@[025:027) NewLine |\n\n|

}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

module './main.bicep' = if (1 + 2 == 3) {
//@[000:006) Identifier |module|
//@[007:021) StringComplete |'./main.bicep'|
//@[022:023) Assignment |=|
//@[024:026) Identifier |if|
//@[027:028) LeftParen |(|
//@[028:029) Integer |1|
//@[030:031) Plus |+|
//@[032:033) Integer |2|
//@[034:036) Equals |==|
//@[037:038) Integer |3|
//@[038:039) RightParen |)|
//@[040:041) LeftBrace |{|
//@[041:043) NewLine |\n\n|

}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

module './main.bicep' = if
//@[000:006) Identifier |module|
//@[007:021) StringComplete |'./main.bicep'|
//@[022:023) Assignment |=|
//@[024:026) Identifier |if|
//@[026:028) NewLine |\n\n|

module './main.bicep' = if (
//@[000:006) Identifier |module|
//@[007:021) StringComplete |'./main.bicep'|
//@[022:023) Assignment |=|
//@[024:026) Identifier |if|
//@[027:028) LeftParen |(|
//@[028:030) NewLine |\n\n|

module './main.bicep' = if (true
//@[000:006) Identifier |module|
//@[007:021) StringComplete |'./main.bicep'|
//@[022:023) Assignment |=|
//@[024:026) Identifier |if|
//@[027:028) LeftParen |(|
//@[028:032) TrueKeyword |true|
//@[032:034) NewLine |\n\n|

module './main.bicep' = if (true)
//@[000:006) Identifier |module|
//@[007:021) StringComplete |'./main.bicep'|
//@[022:023) Assignment |=|
//@[024:026) Identifier |if|
//@[027:028) LeftParen |(|
//@[028:032) TrueKeyword |true|
//@[032:033) RightParen |)|
//@[033:035) NewLine |\n\n|

module './main.bicep' = if {
//@[000:006) Identifier |module|
//@[007:021) StringComplete |'./main.bicep'|
//@[022:023) Assignment |=|
//@[024:026) Identifier |if|
//@[027:028) LeftBrace |{|
//@[028:030) NewLine |\n\n|

}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

module './main.bicep' = if () {
//@[000:006) Identifier |module|
//@[007:021) StringComplete |'./main.bicep'|
//@[022:023) Assignment |=|
//@[024:026) Identifier |if|
//@[027:028) LeftParen |(|
//@[028:029) RightParen |)|
//@[030:031) LeftBrace |{|
//@[031:033) NewLine |\n\n|

}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

module './main.bicep' = if ('true') {
//@[000:006) Identifier |module|
//@[007:021) StringComplete |'./main.bicep'|
//@[022:023) Assignment |=|
//@[024:026) Identifier |if|
//@[027:028) LeftParen |(|
//@[028:034) StringComplete |'true'|
//@[034:035) RightParen |)|
//@[036:037) LeftBrace |{|
//@[037:039) NewLine |\n\n|

}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

module modANoName './modulea.bicep' = {
//@[000:006) Identifier |module|
//@[007:017) Identifier |modANoName|
//@[018:035) StringComplete |'./modulea.bicep'|
//@[036:037) Assignment |=|
//@[038:039) LeftBrace |{|
//@[039:040) NewLine |\n|
// #completionTest(0) -> moduleATopLevelProperties
//@[050:052) NewLine |\n\n|

}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

module modANoNameWithCondition './modulea.bicep' = if (true) {
//@[000:006) Identifier |module|
//@[007:030) Identifier |modANoNameWithCondition|
//@[031:048) StringComplete |'./modulea.bicep'|
//@[049:050) Assignment |=|
//@[051:053) Identifier |if|
//@[054:055) LeftParen |(|
//@[055:059) TrueKeyword |true|
//@[059:060) RightParen |)|
//@[061:062) LeftBrace |{|
//@[062:063) NewLine |\n|
// #completionTest(0) -> moduleAWithConditionTopLevelProperties
//@[063:065) NewLine |\n\n|

}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

module modWithReferenceInCondition './main.bicep' = if (reference('Micorosft.Management/managementGroups/MG', '2020-05-01').name == 'something') {
//@[000:006) Identifier |module|
//@[007:034) Identifier |modWithReferenceInCondition|
//@[035:049) StringComplete |'./main.bicep'|
//@[050:051) Assignment |=|
//@[052:054) Identifier |if|
//@[055:056) LeftParen |(|
//@[056:065) Identifier |reference|
//@[065:066) LeftParen |(|
//@[066:108) StringComplete |'Micorosft.Management/managementGroups/MG'|
//@[108:109) Comma |,|
//@[110:122) StringComplete |'2020-05-01'|
//@[122:123) RightParen |)|
//@[123:124) Dot |.|
//@[124:128) Identifier |name|
//@[129:131) Equals |==|
//@[132:143) StringComplete |'something'|
//@[143:144) RightParen |)|
//@[145:146) LeftBrace |{|
//@[146:148) NewLine |\n\n|

}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

module modWithListKeysInCondition './main.bicep' = if (listKeys('foo', '2020-05-01').bar == true) {
//@[000:006) Identifier |module|
//@[007:033) Identifier |modWithListKeysInCondition|
//@[034:048) StringComplete |'./main.bicep'|
//@[049:050) Assignment |=|
//@[051:053) Identifier |if|
//@[054:055) LeftParen |(|
//@[055:063) Identifier |listKeys|
//@[063:064) LeftParen |(|
//@[064:069) StringComplete |'foo'|
//@[069:070) Comma |,|
//@[071:083) StringComplete |'2020-05-01'|
//@[083:084) RightParen |)|
//@[084:085) Dot |.|
//@[085:088) Identifier |bar|
//@[089:091) Equals |==|
//@[092:096) TrueKeyword |true|
//@[096:097) RightParen |)|
//@[098:099) LeftBrace |{|
//@[099:101) NewLine |\n\n|

}
//@[000:001) RightBrace |}|
//@[001:004) NewLine |\n\n\n|


module modANoName './modulea.bicep' = if ({ 'a': b }.a == true) {
//@[000:006) Identifier |module|
//@[007:017) Identifier |modANoName|
//@[018:035) StringComplete |'./modulea.bicep'|
//@[036:037) Assignment |=|
//@[038:040) Identifier |if|
//@[041:042) LeftParen |(|
//@[042:043) LeftBrace |{|
//@[044:047) StringComplete |'a'|
//@[047:048) Colon |:|
//@[049:050) Identifier |b|
//@[051:052) RightBrace |}|
//@[052:053) Dot |.|
//@[053:054) Identifier |a|
//@[055:057) Equals |==|
//@[058:062) TrueKeyword |true|
//@[062:063) RightParen |)|
//@[064:065) LeftBrace |{|
//@[065:067) NewLine |\n\n|

}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

module modANoInputs './modulea.bicep' = {
//@[000:006) Identifier |module|
//@[007:019) Identifier |modANoInputs|
//@[020:037) StringComplete |'./modulea.bicep'|
//@[038:039) Assignment |=|
//@[040:041) LeftBrace |{|
//@[041:042) NewLine |\n|
  name: 'modANoInputs'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:022) StringComplete |'modANoInputs'|
//@[022:023) NewLine |\n|
  // #completionTest(0,1,2) -> moduleATopLevelPropertiesMinusName
//@[065:066) NewLine |\n|
  
//@[002:003) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

module modANoInputsWithCondition './modulea.bicep' = if (length([
//@[000:006) Identifier |module|
//@[007:032) Identifier |modANoInputsWithCondition|
//@[033:050) StringComplete |'./modulea.bicep'|
//@[051:052) Assignment |=|
//@[053:055) Identifier |if|
//@[056:057) LeftParen |(|
//@[057:063) Identifier |length|
//@[063:064) LeftParen |(|
//@[064:065) LeftSquare |[|
//@[065:066) NewLine |\n|
  'foo'
//@[002:007) StringComplete |'foo'|
//@[007:008) NewLine |\n|
]) == 1) {
//@[000:001) RightSquare |]|
//@[001:002) RightParen |)|
//@[003:005) Equals |==|
//@[006:007) Integer |1|
//@[007:008) RightParen |)|
//@[009:010) LeftBrace |{|
//@[010:011) NewLine |\n|
  name: 'modANoInputs'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:022) StringComplete |'modANoInputs'|
//@[022:023) NewLine |\n|
  // #completionTest(0,1,2) -> moduleAWithConditionTopLevelPropertiesMinusName
//@[078:079) NewLine |\n|
  
//@[002:003) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

module modAEmptyInputs './modulea.bicep' = {
//@[000:006) Identifier |module|
//@[007:022) Identifier |modAEmptyInputs|
//@[023:040) StringComplete |'./modulea.bicep'|
//@[041:042) Assignment |=|
//@[043:044) LeftBrace |{|
//@[044:045) NewLine |\n|
  name: 'modANoInputs'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:022) StringComplete |'modANoInputs'|
//@[022:023) NewLine |\n|
  params: {
//@[002:008) Identifier |params|
//@[008:009) Colon |:|
//@[010:011) LeftBrace |{|
//@[011:012) NewLine |\n|
    // #completionTest(0,1,2,3,4) -> moduleAParams
//@[050:051) NewLine |\n|
    
//@[004:005) NewLine |\n|
  }
//@[002:003) RightBrace |}|
//@[003:004) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

module modAEmptyInputsWithCondition './modulea.bicep' = if (1 + 2 == 2) {
//@[000:006) Identifier |module|
//@[007:035) Identifier |modAEmptyInputsWithCondition|
//@[036:053) StringComplete |'./modulea.bicep'|
//@[054:055) Assignment |=|
//@[056:058) Identifier |if|
//@[059:060) LeftParen |(|
//@[060:061) Integer |1|
//@[062:063) Plus |+|
//@[064:065) Integer |2|
//@[066:068) Equals |==|
//@[069:070) Integer |2|
//@[070:071) RightParen |)|
//@[072:073) LeftBrace |{|
//@[073:074) NewLine |\n|
  name: 'modANoInputs'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:022) StringComplete |'modANoInputs'|
//@[022:023) NewLine |\n|
  params: {
//@[002:008) Identifier |params|
//@[008:009) Colon |:|
//@[010:011) LeftBrace |{|
//@[011:012) NewLine |\n|
    // #completionTest(0,1,2,3,4) -> moduleAWithConditionParams
//@[063:064) NewLine |\n|
    
//@[004:005) NewLine |\n|
  }
//@[002:003) RightBrace |}|
//@[003:004) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

// #completionTest(55) -> moduleATopLevelPropertyAccess
//@[055:056) NewLine |\n|
var modulePropertyAccessCompletions = modAEmptyInputs.o
//@[000:003) Identifier |var|
//@[004:035) Identifier |modulePropertyAccessCompletions|
//@[036:037) Assignment |=|
//@[038:053) Identifier |modAEmptyInputs|
//@[053:054) Dot |.|
//@[054:055) Identifier |o|
//@[055:057) NewLine |\n\n|

// #completionTest(81) -> moduleAWithConditionTopLevelPropertyAccess
//@[068:069) NewLine |\n|
var moduleWithConditionPropertyAccessCompletions = modAEmptyInputsWithCondition.o
//@[000:003) Identifier |var|
//@[004:048) Identifier |moduleWithConditionPropertyAccessCompletions|
//@[049:050) Assignment |=|
//@[051:079) Identifier |modAEmptyInputsWithCondition|
//@[079:080) Dot |.|
//@[080:081) Identifier |o|
//@[081:083) NewLine |\n\n|

// #completionTest(56) -> moduleAOutputs
//@[040:041) NewLine |\n|
var moduleOutputsCompletions = modAEmptyInputs.outputs.s
//@[000:003) Identifier |var|
//@[004:028) Identifier |moduleOutputsCompletions|
//@[029:030) Assignment |=|
//@[031:046) Identifier |modAEmptyInputs|
//@[046:047) Dot |.|
//@[047:054) Identifier |outputs|
//@[054:055) Dot |.|
//@[055:056) Identifier |s|
//@[056:058) NewLine |\n\n|

// #completionTest(82) -> moduleAWithConditionOutputs
//@[053:054) NewLine |\n|
var moduleWithConditionOutputsCompletions = modAEmptyInputsWithCondition.outputs.s
//@[000:003) Identifier |var|
//@[004:041) Identifier |moduleWithConditionOutputsCompletions|
//@[042:043) Assignment |=|
//@[044:072) Identifier |modAEmptyInputsWithCondition|
//@[072:073) Dot |.|
//@[073:080) Identifier |outputs|
//@[080:081) Dot |.|
//@[081:082) Identifier |s|
//@[082:084) NewLine |\n\n|

module modAUnspecifiedInputs './modulea.bicep' = {
//@[000:006) Identifier |module|
//@[007:028) Identifier |modAUnspecifiedInputs|
//@[029:046) StringComplete |'./modulea.bicep'|
//@[047:048) Assignment |=|
//@[049:050) LeftBrace |{|
//@[050:051) NewLine |\n|
  name: 'modAUnspecifiedInputs'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:031) StringComplete |'modAUnspecifiedInputs'|
//@[031:032) NewLine |\n|
  params: {
//@[002:008) Identifier |params|
//@[008:009) Colon |:|
//@[010:011) LeftBrace |{|
//@[011:012) NewLine |\n|
    stringParamB: ''
//@[004:016) Identifier |stringParamB|
//@[016:017) Colon |:|
//@[018:020) StringComplete |''|
//@[020:021) NewLine |\n|
    objParam: {}
//@[004:012) Identifier |objParam|
//@[012:013) Colon |:|
//@[014:015) LeftBrace |{|
//@[015:016) RightBrace |}|
//@[016:017) NewLine |\n|
    objArray: []
//@[004:012) Identifier |objArray|
//@[012:013) Colon |:|
//@[014:015) LeftSquare |[|
//@[015:016) RightSquare |]|
//@[016:017) NewLine |\n|
    unspecifiedInput: ''
//@[004:020) Identifier |unspecifiedInput|
//@[020:021) Colon |:|
//@[022:024) StringComplete |''|
//@[024:025) NewLine |\n|
  }
//@[002:003) RightBrace |}|
//@[003:004) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

var unspecifiedOutput = modAUnspecifiedInputs.outputs.test
//@[000:003) Identifier |var|
//@[004:021) Identifier |unspecifiedOutput|
//@[022:023) Assignment |=|
//@[024:045) Identifier |modAUnspecifiedInputs|
//@[045:046) Dot |.|
//@[046:053) Identifier |outputs|
//@[053:054) Dot |.|
//@[054:058) Identifier |test|
//@[058:060) NewLine |\n\n|

module modCycle './cycle.bicep' = {
//@[000:006) Identifier |module|
//@[007:015) Identifier |modCycle|
//@[016:031) StringComplete |'./cycle.bicep'|
//@[032:033) Assignment |=|
//@[034:035) LeftBrace |{|
//@[035:036) NewLine |\n|
  
//@[002:003) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

module moduleWithEmptyPath '' = {
//@[000:006) Identifier |module|
//@[007:026) Identifier |moduleWithEmptyPath|
//@[027:029) StringComplete |''|
//@[030:031) Assignment |=|
//@[032:033) LeftBrace |{|
//@[033:034) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

module moduleWithAbsolutePath '/abc/def.bicep' = {
//@[000:006) Identifier |module|
//@[007:029) Identifier |moduleWithAbsolutePath|
//@[030:046) StringComplete |'/abc/def.bicep'|
//@[047:048) Assignment |=|
//@[049:050) LeftBrace |{|
//@[050:051) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

module moduleWithBackslash 'child\\file.bicep' = {
//@[000:006) Identifier |module|
//@[007:026) Identifier |moduleWithBackslash|
//@[027:046) StringComplete |'child\\file.bicep'|
//@[047:048) Assignment |=|
//@[049:050) LeftBrace |{|
//@[050:051) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

module moduleWithInvalidChar 'child/fi|le.bicep' = {
//@[000:006) Identifier |module|
//@[007:028) Identifier |moduleWithInvalidChar|
//@[029:048) StringComplete |'child/fi|le.bicep'|
//@[049:050) Assignment |=|
//@[051:052) LeftBrace |{|
//@[052:053) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

module moduleWithInvalidTerminatorChar 'child/test.' = {
//@[000:006) Identifier |module|
//@[007:038) Identifier |moduleWithInvalidTerminatorChar|
//@[039:052) StringComplete |'child/test.'|
//@[053:054) Assignment |=|
//@[055:056) LeftBrace |{|
//@[056:057) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

module moduleWithValidScope './empty.bicep' = {
//@[000:006) Identifier |module|
//@[007:027) Identifier |moduleWithValidScope|
//@[028:043) StringComplete |'./empty.bicep'|
//@[044:045) Assignment |=|
//@[046:047) LeftBrace |{|
//@[047:048) NewLine |\n|
  name: 'moduleWithValidScope'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:030) StringComplete |'moduleWithValidScope'|
//@[030:031) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

module moduleWithInvalidScope './empty.bicep' = {
//@[000:006) Identifier |module|
//@[007:029) Identifier |moduleWithInvalidScope|
//@[030:045) StringComplete |'./empty.bicep'|
//@[046:047) Assignment |=|
//@[048:049) LeftBrace |{|
//@[049:050) NewLine |\n|
  name: 'moduleWithInvalidScope'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:032) StringComplete |'moduleWithInvalidScope'|
//@[032:033) NewLine |\n|
  scope: moduleWithValidScope
//@[002:007) Identifier |scope|
//@[007:008) Colon |:|
//@[009:029) Identifier |moduleWithValidScope|
//@[029:030) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

module moduleWithMissingRequiredScope './subscription_empty.bicep' = {
//@[000:006) Identifier |module|
//@[007:037) Identifier |moduleWithMissingRequiredScope|
//@[038:066) StringComplete |'./subscription_empty.bicep'|
//@[067:068) Assignment |=|
//@[069:070) LeftBrace |{|
//@[070:071) NewLine |\n|
  name: 'moduleWithMissingRequiredScope'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:040) StringComplete |'moduleWithMissingRequiredScope'|
//@[040:041) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

module moduleWithInvalidScope2 './empty.bicep' = {
//@[000:006) Identifier |module|
//@[007:030) Identifier |moduleWithInvalidScope2|
//@[031:046) StringComplete |'./empty.bicep'|
//@[047:048) Assignment |=|
//@[049:050) LeftBrace |{|
//@[050:051) NewLine |\n|
  name: 'moduleWithInvalidScope2'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:033) StringComplete |'moduleWithInvalidScope2'|
//@[033:034) NewLine |\n|
  scope: managementGroup()
//@[002:007) Identifier |scope|
//@[007:008) Colon |:|
//@[009:024) Identifier |managementGroup|
//@[024:025) LeftParen |(|
//@[025:026) RightParen |)|
//@[026:027) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

module moduleWithUnsupprtedScope1 './mg_empty.bicep' = {
//@[000:006) Identifier |module|
//@[007:033) Identifier |moduleWithUnsupprtedScope1|
//@[034:052) StringComplete |'./mg_empty.bicep'|
//@[053:054) Assignment |=|
//@[055:056) LeftBrace |{|
//@[056:057) NewLine |\n|
  name: 'moduleWithUnsupprtedScope1'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:036) StringComplete |'moduleWithUnsupprtedScope1'|
//@[036:037) NewLine |\n|
  scope: managementGroup()
//@[002:007) Identifier |scope|
//@[007:008) Colon |:|
//@[009:024) Identifier |managementGroup|
//@[024:025) LeftParen |(|
//@[025:026) RightParen |)|
//@[026:027) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

module moduleWithUnsupprtedScope2 './mg_empty.bicep' = {
//@[000:006) Identifier |module|
//@[007:033) Identifier |moduleWithUnsupprtedScope2|
//@[034:052) StringComplete |'./mg_empty.bicep'|
//@[053:054) Assignment |=|
//@[055:056) LeftBrace |{|
//@[056:057) NewLine |\n|
  name: 'moduleWithUnsupprtedScope2'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:036) StringComplete |'moduleWithUnsupprtedScope2'|
//@[036:037) NewLine |\n|
  scope: managementGroup('MG')
//@[002:007) Identifier |scope|
//@[007:008) Colon |:|
//@[009:024) Identifier |managementGroup|
//@[024:025) LeftParen |(|
//@[025:029) StringComplete |'MG'|
//@[029:030) RightParen |)|
//@[030:031) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

module moduleWithBadScope './empty.bicep' = {
//@[000:006) Identifier |module|
//@[007:025) Identifier |moduleWithBadScope|
//@[026:041) StringComplete |'./empty.bicep'|
//@[042:043) Assignment |=|
//@[044:045) LeftBrace |{|
//@[045:046) NewLine |\n|
  name: 'moduleWithBadScope'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:028) StringComplete |'moduleWithBadScope'|
//@[028:029) NewLine |\n|
  scope: 'stringScope'
//@[002:007) Identifier |scope|
//@[007:008) Colon |:|
//@[009:022) StringComplete |'stringScope'|
//@[022:023) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

resource runtimeValidRes1 'Microsoft.Storage/storageAccounts@2019-06-01' = {
//@[000:008) Identifier |resource|
//@[009:025) Identifier |runtimeValidRes1|
//@[026:072) StringComplete |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[073:074) Assignment |=|
//@[075:076) LeftBrace |{|
//@[076:077) NewLine |\n|
  name: 'runtimeValidRes1Name'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:030) StringComplete |'runtimeValidRes1Name'|
//@[030:031) NewLine |\n|
  location: 'westeurope'
//@[002:010) Identifier |location|
//@[010:011) Colon |:|
//@[012:024) StringComplete |'westeurope'|
//@[024:025) NewLine |\n|
  kind: 'Storage'
//@[002:006) Identifier |kind|
//@[006:007) Colon |:|
//@[008:017) StringComplete |'Storage'|
//@[017:018) NewLine |\n|
  sku: {
//@[002:005) Identifier |sku|
//@[005:006) Colon |:|
//@[007:008) LeftBrace |{|
//@[008:009) NewLine |\n|
    name: 'Standard_GRS'
//@[004:008) Identifier |name|
//@[008:009) Colon |:|
//@[010:024) StringComplete |'Standard_GRS'|
//@[024:025) NewLine |\n|
  }
//@[002:003) RightBrace |}|
//@[003:004) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

module runtimeValidModule1 'empty.bicep' = {
//@[000:006) Identifier |module|
//@[007:026) Identifier |runtimeValidModule1|
//@[027:040) StringComplete |'empty.bicep'|
//@[041:042) Assignment |=|
//@[043:044) LeftBrace |{|
//@[044:045) NewLine |\n|
  name: concat(concat(runtimeValidRes1.id, runtimeValidRes1.name), runtimeValidRes1.type)
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:014) Identifier |concat|
//@[014:015) LeftParen |(|
//@[015:021) Identifier |concat|
//@[021:022) LeftParen |(|
//@[022:038) Identifier |runtimeValidRes1|
//@[038:039) Dot |.|
//@[039:041) Identifier |id|
//@[041:042) Comma |,|
//@[043:059) Identifier |runtimeValidRes1|
//@[059:060) Dot |.|
//@[060:064) Identifier |name|
//@[064:065) RightParen |)|
//@[065:066) Comma |,|
//@[067:083) Identifier |runtimeValidRes1|
//@[083:084) Dot |.|
//@[084:088) Identifier |type|
//@[088:089) RightParen |)|
//@[089:090) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

module runtimeInvalidModule1 'empty.bicep' = {
//@[000:006) Identifier |module|
//@[007:028) Identifier |runtimeInvalidModule1|
//@[029:042) StringComplete |'empty.bicep'|
//@[043:044) Assignment |=|
//@[045:046) LeftBrace |{|
//@[046:047) NewLine |\n|
  name: runtimeValidRes1.location
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:024) Identifier |runtimeValidRes1|
//@[024:025) Dot |.|
//@[025:033) Identifier |location|
//@[033:034) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

module runtimeInvalidModule2 'empty.bicep' = {
//@[000:006) Identifier |module|
//@[007:028) Identifier |runtimeInvalidModule2|
//@[029:042) StringComplete |'empty.bicep'|
//@[043:044) Assignment |=|
//@[045:046) LeftBrace |{|
//@[046:047) NewLine |\n|
  name: runtimeValidRes1['location']
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:024) Identifier |runtimeValidRes1|
//@[024:025) LeftSquare |[|
//@[025:035) StringComplete |'location'|
//@[035:036) RightSquare |]|
//@[036:037) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

module runtimeInvalidModule3 'empty.bicep' = {
//@[000:006) Identifier |module|
//@[007:028) Identifier |runtimeInvalidModule3|
//@[029:042) StringComplete |'empty.bicep'|
//@[043:044) Assignment |=|
//@[045:046) LeftBrace |{|
//@[046:047) NewLine |\n|
  name: runtimeValidRes1.sku.name
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:024) Identifier |runtimeValidRes1|
//@[024:025) Dot |.|
//@[025:028) Identifier |sku|
//@[028:029) Dot |.|
//@[029:033) Identifier |name|
//@[033:034) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

module runtimeInvalidModule4 'empty.bicep' = {
//@[000:006) Identifier |module|
//@[007:028) Identifier |runtimeInvalidModule4|
//@[029:042) StringComplete |'empty.bicep'|
//@[043:044) Assignment |=|
//@[045:046) LeftBrace |{|
//@[046:047) NewLine |\n|
  name: runtimeValidRes1.sku['name']
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:024) Identifier |runtimeValidRes1|
//@[024:025) Dot |.|
//@[025:028) Identifier |sku|
//@[028:029) LeftSquare |[|
//@[029:035) StringComplete |'name'|
//@[035:036) RightSquare |]|
//@[036:037) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

module runtimeInvalidModule5 'empty.bicep' = {
//@[000:006) Identifier |module|
//@[007:028) Identifier |runtimeInvalidModule5|
//@[029:042) StringComplete |'empty.bicep'|
//@[043:044) Assignment |=|
//@[045:046) LeftBrace |{|
//@[046:047) NewLine |\n|
  name: runtimeValidRes1['sku']['name']
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:024) Identifier |runtimeValidRes1|
//@[024:025) LeftSquare |[|
//@[025:030) StringComplete |'sku'|
//@[030:031) RightSquare |]|
//@[031:032) LeftSquare |[|
//@[032:038) StringComplete |'name'|
//@[038:039) RightSquare |]|
//@[039:040) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

module runtimeInvalidModule6 'empty.bicep' = {
//@[000:006) Identifier |module|
//@[007:028) Identifier |runtimeInvalidModule6|
//@[029:042) StringComplete |'empty.bicep'|
//@[043:044) Assignment |=|
//@[045:046) LeftBrace |{|
//@[046:047) NewLine |\n|
  name: runtimeValidRes1['sku'].name
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:024) Identifier |runtimeValidRes1|
//@[024:025) LeftSquare |[|
//@[025:030) StringComplete |'sku'|
//@[030:031) RightSquare |]|
//@[031:032) Dot |.|
//@[032:036) Identifier |name|
//@[036:037) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

module singleModuleForRuntimeCheck 'modulea.bicep' = {
//@[000:006) Identifier |module|
//@[007:034) Identifier |singleModuleForRuntimeCheck|
//@[035:050) StringComplete |'modulea.bicep'|
//@[051:052) Assignment |=|
//@[053:054) LeftBrace |{|
//@[054:055) NewLine |\n|
  name: 'test'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:014) StringComplete |'test'|
//@[014:015) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

var moduleRuntimeCheck = singleModuleForRuntimeCheck.outputs.stringOutputA
//@[000:003) Identifier |var|
//@[004:022) Identifier |moduleRuntimeCheck|
//@[023:024) Assignment |=|
//@[025:052) Identifier |singleModuleForRuntimeCheck|
//@[052:053) Dot |.|
//@[053:060) Identifier |outputs|
//@[060:061) Dot |.|
//@[061:074) Identifier |stringOutputA|
//@[074:075) NewLine |\n|
var moduleRuntimeCheck2 = moduleRuntimeCheck
//@[000:003) Identifier |var|
//@[004:023) Identifier |moduleRuntimeCheck2|
//@[024:025) Assignment |=|
//@[026:044) Identifier |moduleRuntimeCheck|
//@[044:046) NewLine |\n\n|

module moduleLoopForRuntimeCheck 'modulea.bicep' = [for thing in []: {
//@[000:006) Identifier |module|
//@[007:032) Identifier |moduleLoopForRuntimeCheck|
//@[033:048) StringComplete |'modulea.bicep'|
//@[049:050) Assignment |=|
//@[051:052) LeftSquare |[|
//@[052:055) Identifier |for|
//@[056:061) Identifier |thing|
//@[062:064) Identifier |in|
//@[065:066) LeftSquare |[|
//@[066:067) RightSquare |]|
//@[067:068) Colon |:|
//@[069:070) LeftBrace |{|
//@[070:071) NewLine |\n|
  name: moduleRuntimeCheck2
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:027) Identifier |moduleRuntimeCheck2|
//@[027:028) NewLine |\n|
}]
//@[000:001) RightBrace |}|
//@[001:002) RightSquare |]|
//@[002:004) NewLine |\n\n|

var moduleRuntimeCheck3 = moduleLoopForRuntimeCheck[1].outputs.stringOutputB
//@[000:003) Identifier |var|
//@[004:023) Identifier |moduleRuntimeCheck3|
//@[024:025) Assignment |=|
//@[026:051) Identifier |moduleLoopForRuntimeCheck|
//@[051:052) LeftSquare |[|
//@[052:053) Integer |1|
//@[053:054) RightSquare |]|
//@[054:055) Dot |.|
//@[055:062) Identifier |outputs|
//@[062:063) Dot |.|
//@[063:076) Identifier |stringOutputB|
//@[076:077) NewLine |\n|
var moduleRuntimeCheck4 = moduleRuntimeCheck3
//@[000:003) Identifier |var|
//@[004:023) Identifier |moduleRuntimeCheck4|
//@[024:025) Assignment |=|
//@[026:045) Identifier |moduleRuntimeCheck3|
//@[045:046) NewLine |\n|
module moduleLoopForRuntimeCheck2 'modulea.bicep' = [for thing in []: {
//@[000:006) Identifier |module|
//@[007:033) Identifier |moduleLoopForRuntimeCheck2|
//@[034:049) StringComplete |'modulea.bicep'|
//@[050:051) Assignment |=|
//@[052:053) LeftSquare |[|
//@[053:056) Identifier |for|
//@[057:062) Identifier |thing|
//@[063:065) Identifier |in|
//@[066:067) LeftSquare |[|
//@[067:068) RightSquare |]|
//@[068:069) Colon |:|
//@[070:071) LeftBrace |{|
//@[071:072) NewLine |\n|
  name: moduleRuntimeCheck4
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:027) Identifier |moduleRuntimeCheck4|
//@[027:028) NewLine |\n|
}]
//@[000:001) RightBrace |}|
//@[001:002) RightSquare |]|
//@[002:004) NewLine |\n\n|

module moduleLoopForRuntimeCheck3 'modulea.bicep' = [for thing in []: {
//@[000:006) Identifier |module|
//@[007:033) Identifier |moduleLoopForRuntimeCheck3|
//@[034:049) StringComplete |'modulea.bicep'|
//@[050:051) Assignment |=|
//@[052:053) LeftSquare |[|
//@[053:056) Identifier |for|
//@[057:062) Identifier |thing|
//@[063:065) Identifier |in|
//@[066:067) LeftSquare |[|
//@[067:068) RightSquare |]|
//@[068:069) Colon |:|
//@[070:071) LeftBrace |{|
//@[071:072) NewLine |\n|
  name: concat(moduleLoopForRuntimeCheck[1].outputs.stringOutputB, moduleLoopForRuntimeCheck[1].outputs.stringOutputA )
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:014) Identifier |concat|
//@[014:015) LeftParen |(|
//@[015:040) Identifier |moduleLoopForRuntimeCheck|
//@[040:041) LeftSquare |[|
//@[041:042) Integer |1|
//@[042:043) RightSquare |]|
//@[043:044) Dot |.|
//@[044:051) Identifier |outputs|
//@[051:052) Dot |.|
//@[052:065) Identifier |stringOutputB|
//@[065:066) Comma |,|
//@[067:092) Identifier |moduleLoopForRuntimeCheck|
//@[092:093) LeftSquare |[|
//@[093:094) Integer |1|
//@[094:095) RightSquare |]|
//@[095:096) Dot |.|
//@[096:103) Identifier |outputs|
//@[103:104) Dot |.|
//@[104:117) Identifier |stringOutputA|
//@[118:119) RightParen |)|
//@[119:120) NewLine |\n|
}]
//@[000:001) RightBrace |}|
//@[001:002) RightSquare |]|
//@[002:004) NewLine |\n\n|

module moduleWithDuplicateName1 './empty.bicep' = {
//@[000:006) Identifier |module|
//@[007:031) Identifier |moduleWithDuplicateName1|
//@[032:047) StringComplete |'./empty.bicep'|
//@[048:049) Assignment |=|
//@[050:051) LeftBrace |{|
//@[051:052) NewLine |\n|
  name: 'moduleWithDuplicateName'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:033) StringComplete |'moduleWithDuplicateName'|
//@[033:034) NewLine |\n|
  scope: resourceGroup()
//@[002:007) Identifier |scope|
//@[007:008) Colon |:|
//@[009:022) Identifier |resourceGroup|
//@[022:023) LeftParen |(|
//@[023:024) RightParen |)|
//@[024:025) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

module moduleWithDuplicateName2 './empty.bicep' = {
//@[000:006) Identifier |module|
//@[007:031) Identifier |moduleWithDuplicateName2|
//@[032:047) StringComplete |'./empty.bicep'|
//@[048:049) Assignment |=|
//@[050:051) LeftBrace |{|
//@[051:052) NewLine |\n|
  name: 'moduleWithDuplicateName'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:033) StringComplete |'moduleWithDuplicateName'|
//@[033:034) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

// #completionTest(19, 20, 21) -> cwdFileAndBicepRegitryTemplateSpecSchemaCompletions
//@[085:086) NewLine |\n|
module completionB ''
//@[000:006) Identifier |module|
//@[007:018) Identifier |completionB|
//@[019:021) StringComplete |''|
//@[021:023) NewLine |\n\n|

// #completionTest(19, 20, 21) -> cwdFileAndBicepRegitryTemplateSpecSchemaCompletions
//@[085:086) NewLine |\n|
module completionC '' =
//@[000:006) Identifier |module|
//@[007:018) Identifier |completionC|
//@[019:021) StringComplete |''|
//@[022:023) Assignment |=|
//@[023:025) NewLine |\n\n|

// #completionTest(19, 20, 21) -> cwdFileAndBicepRegitryTemplateSpecSchemaCompletions
//@[085:086) NewLine |\n|
module completionD '' = {}
//@[000:006) Identifier |module|
//@[007:018) Identifier |completionD|
//@[019:021) StringComplete |''|
//@[022:023) Assignment |=|
//@[024:025) LeftBrace |{|
//@[025:026) RightBrace |}|
//@[026:028) NewLine |\n\n|

// #completionTest(19, 20, 21) -> cwdFileAndBicepRegitryTemplateSpecSchemaCompletions
//@[085:086) NewLine |\n|
module completionE '' = {
//@[000:006) Identifier |module|
//@[007:018) Identifier |completionE|
//@[019:021) StringComplete |''|
//@[022:023) Assignment |=|
//@[024:025) LeftBrace |{|
//@[025:026) NewLine |\n|
  name: 'hello'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:015) StringComplete |'hello'|
//@[015:016) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

// #completionTest(29) -> cwdDotFileCompletions
//@[047:048) NewLine |\n|
module cwdFileCompletionA './m'
//@[000:006) Identifier |module|
//@[007:025) Identifier |cwdFileCompletionA|
//@[026:031) StringComplete |'./m'|
//@[031:033) NewLine |\n\n|

// #completionTest(26, 27) -> cwdFileCompletions
//@[048:049) NewLine |\n|
module cwdFileCompletionB m
//@[000:006) Identifier |module|
//@[007:025) Identifier |cwdFileCompletionB|
//@[026:027) Identifier |m|
//@[027:029) NewLine |\n\n|

// #completionTest(26, 27, 28, 29) -> cwdFileCompletions
//@[056:057) NewLine |\n|
module cwdFileCompletionC 'm'
//@[000:006) Identifier |module|
//@[007:025) Identifier |cwdFileCompletionC|
//@[026:029) StringComplete |'m'|
//@[029:031) NewLine |\n\n|

// #completionTest(24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39) -> childFileCompletions
//@[106:107) NewLine |\n|
module childCompletionA 'ChildModules/'
//@[000:006) Identifier |module|
//@[007:023) Identifier |childCompletionA|
//@[024:039) StringComplete |'ChildModules/'|
//@[039:041) NewLine |\n\n|

// #completionTest(24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39) -> childDotFileCompletions
//@[109:110) NewLine |\n|
module childCompletionB './ChildModules/'
//@[000:006) Identifier |module|
//@[007:023) Identifier |childCompletionB|
//@[024:041) StringComplete |'./ChildModules/'|
//@[041:043) NewLine |\n\n|

// #completionTest(24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40) -> childDotFileCompletions
//@[113:114) NewLine |\n|
module childCompletionC './ChildModules/m'
//@[000:006) Identifier |module|
//@[007:023) Identifier |childCompletionC|
//@[024:042) StringComplete |'./ChildModules/m'|
//@[042:044) NewLine |\n\n|

// #completionTest(24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40) -> childFileCompletions
//@[110:111) NewLine |\n|
module childCompletionD 'ChildModules/e'
//@[000:006) Identifier |module|
//@[007:023) Identifier |childCompletionD|
//@[024:040) StringComplete |'ChildModules/e'|
//@[040:042) NewLine |\n\n|

@minValue()
//@[000:001) At |@|
//@[001:009) Identifier |minValue|
//@[009:010) LeftParen |(|
//@[010:011) RightParen |)|
//@[011:012) NewLine |\n|
module moduleWithNotAttachableDecorators './empty.bicep' = {
//@[000:006) Identifier |module|
//@[007:040) Identifier |moduleWithNotAttachableDecorators|
//@[041:056) StringComplete |'./empty.bicep'|
//@[057:058) Assignment |=|
//@[059:060) LeftBrace |{|
//@[060:061) NewLine |\n|
  name: 'moduleWithNotAttachableDecorators'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:043) StringComplete |'moduleWithNotAttachableDecorators'|
//@[043:044) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

// loop parsing cases
//@[021:022) NewLine |\n|
module expectedForKeyword 'modulea.bicep' = []
//@[000:006) Identifier |module|
//@[007:025) Identifier |expectedForKeyword|
//@[026:041) StringComplete |'modulea.bicep'|
//@[042:043) Assignment |=|
//@[044:045) LeftSquare |[|
//@[045:046) RightSquare |]|
//@[046:048) NewLine |\n\n|

module expectedForKeyword2 'modulea.bicep' = [f]
//@[000:006) Identifier |module|
//@[007:026) Identifier |expectedForKeyword2|
//@[027:042) StringComplete |'modulea.bicep'|
//@[043:044) Assignment |=|
//@[045:046) LeftSquare |[|
//@[046:047) Identifier |f|
//@[047:048) RightSquare |]|
//@[048:050) NewLine |\n\n|

module expectedLoopVar 'modulea.bicep' = [for]
//@[000:006) Identifier |module|
//@[007:022) Identifier |expectedLoopVar|
//@[023:038) StringComplete |'modulea.bicep'|
//@[039:040) Assignment |=|
//@[041:042) LeftSquare |[|
//@[042:045) Identifier |for|
//@[045:046) RightSquare |]|
//@[046:048) NewLine |\n\n|

module expectedInKeyword 'modulea.bicep' = [for x]
//@[000:006) Identifier |module|
//@[007:024) Identifier |expectedInKeyword|
//@[025:040) StringComplete |'modulea.bicep'|
//@[041:042) Assignment |=|
//@[043:044) LeftSquare |[|
//@[044:047) Identifier |for|
//@[048:049) Identifier |x|
//@[049:050) RightSquare |]|
//@[050:052) NewLine |\n\n|

module expectedInKeyword2 'modulea.bicep' = [for x b]
//@[000:006) Identifier |module|
//@[007:025) Identifier |expectedInKeyword2|
//@[026:041) StringComplete |'modulea.bicep'|
//@[042:043) Assignment |=|
//@[044:045) LeftSquare |[|
//@[045:048) Identifier |for|
//@[049:050) Identifier |x|
//@[051:052) Identifier |b|
//@[052:053) RightSquare |]|
//@[053:055) NewLine |\n\n|

module expectedArrayExpression 'modulea.bicep' = [for x in]
//@[000:006) Identifier |module|
//@[007:030) Identifier |expectedArrayExpression|
//@[031:046) StringComplete |'modulea.bicep'|
//@[047:048) Assignment |=|
//@[049:050) LeftSquare |[|
//@[050:053) Identifier |for|
//@[054:055) Identifier |x|
//@[056:058) Identifier |in|
//@[058:059) RightSquare |]|
//@[059:061) NewLine |\n\n|

module expectedColon 'modulea.bicep' = [for x in y]
//@[000:006) Identifier |module|
//@[007:020) Identifier |expectedColon|
//@[021:036) StringComplete |'modulea.bicep'|
//@[037:038) Assignment |=|
//@[039:040) LeftSquare |[|
//@[040:043) Identifier |for|
//@[044:045) Identifier |x|
//@[046:048) Identifier |in|
//@[049:050) Identifier |y|
//@[050:051) RightSquare |]|
//@[051:053) NewLine |\n\n|

module expectedLoopBody 'modulea.bicep' = [for x in y:]
//@[000:006) Identifier |module|
//@[007:023) Identifier |expectedLoopBody|
//@[024:039) StringComplete |'modulea.bicep'|
//@[040:041) Assignment |=|
//@[042:043) LeftSquare |[|
//@[043:046) Identifier |for|
//@[047:048) Identifier |x|
//@[049:051) Identifier |in|
//@[052:053) Identifier |y|
//@[053:054) Colon |:|
//@[054:055) RightSquare |]|
//@[055:057) NewLine |\n\n|

// indexed loop parsing cases
//@[029:030) NewLine |\n|
module expectedItemVarName 'modulea.bicep' = [for ()]
//@[000:006) Identifier |module|
//@[007:026) Identifier |expectedItemVarName|
//@[027:042) StringComplete |'modulea.bicep'|
//@[043:044) Assignment |=|
//@[045:046) LeftSquare |[|
//@[046:049) Identifier |for|
//@[050:051) LeftParen |(|
//@[051:052) RightParen |)|
//@[052:053) RightSquare |]|
//@[053:055) NewLine |\n\n|

module expectedComma 'modulea.bicep' = [for (x)]
//@[000:006) Identifier |module|
//@[007:020) Identifier |expectedComma|
//@[021:036) StringComplete |'modulea.bicep'|
//@[037:038) Assignment |=|
//@[039:040) LeftSquare |[|
//@[040:043) Identifier |for|
//@[044:045) LeftParen |(|
//@[045:046) Identifier |x|
//@[046:047) RightParen |)|
//@[047:048) RightSquare |]|
//@[048:050) NewLine |\n\n|

module expectedIndexVarName 'modulea.bicep' = [for (x,)]
//@[000:006) Identifier |module|
//@[007:027) Identifier |expectedIndexVarName|
//@[028:043) StringComplete |'modulea.bicep'|
//@[044:045) Assignment |=|
//@[046:047) LeftSquare |[|
//@[047:050) Identifier |for|
//@[051:052) LeftParen |(|
//@[052:053) Identifier |x|
//@[053:054) Comma |,|
//@[054:055) RightParen |)|
//@[055:056) RightSquare |]|
//@[056:058) NewLine |\n\n|

module expectedInKeyword3 'modulea.bicep' = [for (x,y)]
//@[000:006) Identifier |module|
//@[007:025) Identifier |expectedInKeyword3|
//@[026:041) StringComplete |'modulea.bicep'|
//@[042:043) Assignment |=|
//@[044:045) LeftSquare |[|
//@[045:048) Identifier |for|
//@[049:050) LeftParen |(|
//@[050:051) Identifier |x|
//@[051:052) Comma |,|
//@[052:053) Identifier |y|
//@[053:054) RightParen |)|
//@[054:055) RightSquare |]|
//@[055:057) NewLine |\n\n|

module expectedArrayExpression2 'modulea.bicep' = [for (x,y) in ]
//@[000:006) Identifier |module|
//@[007:031) Identifier |expectedArrayExpression2|
//@[032:047) StringComplete |'modulea.bicep'|
//@[048:049) Assignment |=|
//@[050:051) LeftSquare |[|
//@[051:054) Identifier |for|
//@[055:056) LeftParen |(|
//@[056:057) Identifier |x|
//@[057:058) Comma |,|
//@[058:059) Identifier |y|
//@[059:060) RightParen |)|
//@[061:063) Identifier |in|
//@[064:065) RightSquare |]|
//@[065:067) NewLine |\n\n|

module expectedColon2 'modulea.bicep' = [for (x,y) in z]
//@[000:006) Identifier |module|
//@[007:021) Identifier |expectedColon2|
//@[022:037) StringComplete |'modulea.bicep'|
//@[038:039) Assignment |=|
//@[040:041) LeftSquare |[|
//@[041:044) Identifier |for|
//@[045:046) LeftParen |(|
//@[046:047) Identifier |x|
//@[047:048) Comma |,|
//@[048:049) Identifier |y|
//@[049:050) RightParen |)|
//@[051:053) Identifier |in|
//@[054:055) Identifier |z|
//@[055:056) RightSquare |]|
//@[056:058) NewLine |\n\n|

module expectedLoopBody2 'modulea.bicep' = [for (x,y) in z:]
//@[000:006) Identifier |module|
//@[007:024) Identifier |expectedLoopBody2|
//@[025:040) StringComplete |'modulea.bicep'|
//@[041:042) Assignment |=|
//@[043:044) LeftSquare |[|
//@[044:047) Identifier |for|
//@[048:049) LeftParen |(|
//@[049:050) Identifier |x|
//@[050:051) Comma |,|
//@[051:052) Identifier |y|
//@[052:053) RightParen |)|
//@[054:056) Identifier |in|
//@[057:058) Identifier |z|
//@[058:059) Colon |:|
//@[059:060) RightSquare |]|
//@[060:062) NewLine |\n\n|

// loop filter parsing cases
//@[028:029) NewLine |\n|
module expectedLoopFilterOpenParen 'modulea.bicep' = [for x in y: if]
//@[000:006) Identifier |module|
//@[007:034) Identifier |expectedLoopFilterOpenParen|
//@[035:050) StringComplete |'modulea.bicep'|
//@[051:052) Assignment |=|
//@[053:054) LeftSquare |[|
//@[054:057) Identifier |for|
//@[058:059) Identifier |x|
//@[060:062) Identifier |in|
//@[063:064) Identifier |y|
//@[064:065) Colon |:|
//@[066:068) Identifier |if|
//@[068:069) RightSquare |]|
//@[069:070) NewLine |\n|
module expectedLoopFilterOpenParen2 'modulea.bicep' = [for (x,y) in z: if]
//@[000:006) Identifier |module|
//@[007:035) Identifier |expectedLoopFilterOpenParen2|
//@[036:051) StringComplete |'modulea.bicep'|
//@[052:053) Assignment |=|
//@[054:055) LeftSquare |[|
//@[055:058) Identifier |for|
//@[059:060) LeftParen |(|
//@[060:061) Identifier |x|
//@[061:062) Comma |,|
//@[062:063) Identifier |y|
//@[063:064) RightParen |)|
//@[065:067) Identifier |in|
//@[068:069) Identifier |z|
//@[069:070) Colon |:|
//@[071:073) Identifier |if|
//@[073:074) RightSquare |]|
//@[074:076) NewLine |\n\n|

module expectedLoopFilterPredicateAndBody 'modulea.bicep' = [for x in y: if()]
//@[000:006) Identifier |module|
//@[007:041) Identifier |expectedLoopFilterPredicateAndBody|
//@[042:057) StringComplete |'modulea.bicep'|
//@[058:059) Assignment |=|
//@[060:061) LeftSquare |[|
//@[061:064) Identifier |for|
//@[065:066) Identifier |x|
//@[067:069) Identifier |in|
//@[070:071) Identifier |y|
//@[071:072) Colon |:|
//@[073:075) Identifier |if|
//@[075:076) LeftParen |(|
//@[076:077) RightParen |)|
//@[077:078) RightSquare |]|
//@[078:079) NewLine |\n|
module expectedLoopFilterPredicateAndBody2 'modulea.bicep' = [for (x,y) in z: if()]
//@[000:006) Identifier |module|
//@[007:042) Identifier |expectedLoopFilterPredicateAndBody2|
//@[043:058) StringComplete |'modulea.bicep'|
//@[059:060) Assignment |=|
//@[061:062) LeftSquare |[|
//@[062:065) Identifier |for|
//@[066:067) LeftParen |(|
//@[067:068) Identifier |x|
//@[068:069) Comma |,|
//@[069:070) Identifier |y|
//@[070:071) RightParen |)|
//@[072:074) Identifier |in|
//@[075:076) Identifier |z|
//@[076:077) Colon |:|
//@[078:080) Identifier |if|
//@[080:081) LeftParen |(|
//@[081:082) RightParen |)|
//@[082:083) RightSquare |]|
//@[083:085) NewLine |\n\n|

// wrong loop body type
//@[023:024) NewLine |\n|
var emptyArray = []
//@[000:003) Identifier |var|
//@[004:014) Identifier |emptyArray|
//@[015:016) Assignment |=|
//@[017:018) LeftSquare |[|
//@[018:019) RightSquare |]|
//@[019:020) NewLine |\n|
module wrongLoopBodyType 'modulea.bicep' = [for x in emptyArray:4]
//@[000:006) Identifier |module|
//@[007:024) Identifier |wrongLoopBodyType|
//@[025:040) StringComplete |'modulea.bicep'|
//@[041:042) Assignment |=|
//@[043:044) LeftSquare |[|
//@[044:047) Identifier |for|
//@[048:049) Identifier |x|
//@[050:052) Identifier |in|
//@[053:063) Identifier |emptyArray|
//@[063:064) Colon |:|
//@[064:065) Integer |4|
//@[065:066) RightSquare |]|
//@[066:067) NewLine |\n|
module wrongLoopBodyType2 'modulea.bicep' = [for (x,i) in emptyArray:4]
//@[000:006) Identifier |module|
//@[007:025) Identifier |wrongLoopBodyType2|
//@[026:041) StringComplete |'modulea.bicep'|
//@[042:043) Assignment |=|
//@[044:045) LeftSquare |[|
//@[045:048) Identifier |for|
//@[049:050) LeftParen |(|
//@[050:051) Identifier |x|
//@[051:052) Comma |,|
//@[052:053) Identifier |i|
//@[053:054) RightParen |)|
//@[055:057) Identifier |in|
//@[058:068) Identifier |emptyArray|
//@[068:069) Colon |:|
//@[069:070) Integer |4|
//@[070:071) RightSquare |]|
//@[071:073) NewLine |\n\n|

// missing loop body properties
//@[031:032) NewLine |\n|
module missingLoopBodyProperties 'modulea.bicep' = [for x in emptyArray:{
//@[000:006) Identifier |module|
//@[007:032) Identifier |missingLoopBodyProperties|
//@[033:048) StringComplete |'modulea.bicep'|
//@[049:050) Assignment |=|
//@[051:052) LeftSquare |[|
//@[052:055) Identifier |for|
//@[056:057) Identifier |x|
//@[058:060) Identifier |in|
//@[061:071) Identifier |emptyArray|
//@[071:072) Colon |:|
//@[072:073) LeftBrace |{|
//@[073:074) NewLine |\n|
}]
//@[000:001) RightBrace |}|
//@[001:002) RightSquare |]|
//@[002:003) NewLine |\n|
module missingLoopBodyProperties2 'modulea.bicep' = [for (x,i) in emptyArray:{
//@[000:006) Identifier |module|
//@[007:033) Identifier |missingLoopBodyProperties2|
//@[034:049) StringComplete |'modulea.bicep'|
//@[050:051) Assignment |=|
//@[052:053) LeftSquare |[|
//@[053:056) Identifier |for|
//@[057:058) LeftParen |(|
//@[058:059) Identifier |x|
//@[059:060) Comma |,|
//@[060:061) Identifier |i|
//@[061:062) RightParen |)|
//@[063:065) Identifier |in|
//@[066:076) Identifier |emptyArray|
//@[076:077) Colon |:|
//@[077:078) LeftBrace |{|
//@[078:079) NewLine |\n|
}]
//@[000:001) RightBrace |}|
//@[001:002) RightSquare |]|
//@[002:004) NewLine |\n\n|

// wrong array type
//@[019:020) NewLine |\n|
var notAnArray = true
//@[000:003) Identifier |var|
//@[004:014) Identifier |notAnArray|
//@[015:016) Assignment |=|
//@[017:021) TrueKeyword |true|
//@[021:022) NewLine |\n|
module wrongArrayType 'modulea.bicep' = [for x in notAnArray:{
//@[000:006) Identifier |module|
//@[007:021) Identifier |wrongArrayType|
//@[022:037) StringComplete |'modulea.bicep'|
//@[038:039) Assignment |=|
//@[040:041) LeftSquare |[|
//@[041:044) Identifier |for|
//@[045:046) Identifier |x|
//@[047:049) Identifier |in|
//@[050:060) Identifier |notAnArray|
//@[060:061) Colon |:|
//@[061:062) LeftBrace |{|
//@[062:063) NewLine |\n|
}]
//@[000:001) RightBrace |}|
//@[001:002) RightSquare |]|
//@[002:004) NewLine |\n\n|

// missing fewer properties
//@[027:028) NewLine |\n|
module missingFewerLoopBodyProperties 'modulea.bicep' = [for x in emptyArray:{
//@[000:006) Identifier |module|
//@[007:037) Identifier |missingFewerLoopBodyProperties|
//@[038:053) StringComplete |'modulea.bicep'|
//@[054:055) Assignment |=|
//@[056:057) LeftSquare |[|
//@[057:060) Identifier |for|
//@[061:062) Identifier |x|
//@[063:065) Identifier |in|
//@[066:076) Identifier |emptyArray|
//@[076:077) Colon |:|
//@[077:078) LeftBrace |{|
//@[078:079) NewLine |\n|
  name: 'hello-${x}'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:017) StringLeftPiece |'hello-${|
//@[017:018) Identifier |x|
//@[018:020) StringRightPiece |}'|
//@[020:021) NewLine |\n|
  params: {
//@[002:008) Identifier |params|
//@[008:009) Colon |:|
//@[010:011) LeftBrace |{|
//@[011:013) NewLine |\n\n|

  }
//@[002:003) RightBrace |}|
//@[003:004) NewLine |\n|
}]
//@[000:001) RightBrace |}|
//@[001:002) RightSquare |]|
//@[002:004) NewLine |\n\n|

// wrong parameter in the module loop
//@[037:038) NewLine |\n|
module wrongModuleParameterInLoop 'modulea.bicep' = [for x in emptyArray:{
//@[000:006) Identifier |module|
//@[007:033) Identifier |wrongModuleParameterInLoop|
//@[034:049) StringComplete |'modulea.bicep'|
//@[050:051) Assignment |=|
//@[052:053) LeftSquare |[|
//@[053:056) Identifier |for|
//@[057:058) Identifier |x|
//@[059:061) Identifier |in|
//@[062:072) Identifier |emptyArray|
//@[072:073) Colon |:|
//@[073:074) LeftBrace |{|
//@[074:075) NewLine |\n|
  // #completionTest(17) -> symbolsPlusX
//@[040:041) NewLine |\n|
  name: 'hello-${x}'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:017) StringLeftPiece |'hello-${|
//@[017:018) Identifier |x|
//@[018:020) StringRightPiece |}'|
//@[020:021) NewLine |\n|
  params: {
//@[002:008) Identifier |params|
//@[008:009) Colon |:|
//@[010:011) LeftBrace |{|
//@[011:012) NewLine |\n|
    arrayParam: []
//@[004:014) Identifier |arrayParam|
//@[014:015) Colon |:|
//@[016:017) LeftSquare |[|
//@[017:018) RightSquare |]|
//@[018:019) NewLine |\n|
    objParam: {}
//@[004:012) Identifier |objParam|
//@[012:013) Colon |:|
//@[014:015) LeftBrace |{|
//@[015:016) RightBrace |}|
//@[016:017) NewLine |\n|
    stringParamA: 'test'
//@[004:016) Identifier |stringParamA|
//@[016:017) Colon |:|
//@[018:024) StringComplete |'test'|
//@[024:025) NewLine |\n|
    stringParamB: 'test'
//@[004:016) Identifier |stringParamB|
//@[016:017) Colon |:|
//@[018:024) StringComplete |'test'|
//@[024:025) NewLine |\n|
    notAThing: 'test'
//@[004:013) Identifier |notAThing|
//@[013:014) Colon |:|
//@[015:021) StringComplete |'test'|
//@[021:022) NewLine |\n|
  }
//@[002:003) RightBrace |}|
//@[003:004) NewLine |\n|
}]
//@[000:001) RightBrace |}|
//@[001:002) RightSquare |]|
//@[002:003) NewLine |\n|
module wrongModuleParameterInFilteredLoop 'modulea.bicep' = [for x in emptyArray: if(true) {
//@[000:006) Identifier |module|
//@[007:041) Identifier |wrongModuleParameterInFilteredLoop|
//@[042:057) StringComplete |'modulea.bicep'|
//@[058:059) Assignment |=|
//@[060:061) LeftSquare |[|
//@[061:064) Identifier |for|
//@[065:066) Identifier |x|
//@[067:069) Identifier |in|
//@[070:080) Identifier |emptyArray|
//@[080:081) Colon |:|
//@[082:084) Identifier |if|
//@[084:085) LeftParen |(|
//@[085:089) TrueKeyword |true|
//@[089:090) RightParen |)|
//@[091:092) LeftBrace |{|
//@[092:093) NewLine |\n|
  // #completionTest(17) -> symbolsPlusX_if
//@[043:044) NewLine |\n|
  name: 'hello-${x}'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:017) StringLeftPiece |'hello-${|
//@[017:018) Identifier |x|
//@[018:020) StringRightPiece |}'|
//@[020:021) NewLine |\n|
  params: {
//@[002:008) Identifier |params|
//@[008:009) Colon |:|
//@[010:011) LeftBrace |{|
//@[011:012) NewLine |\n|
    arrayParam: []
//@[004:014) Identifier |arrayParam|
//@[014:015) Colon |:|
//@[016:017) LeftSquare |[|
//@[017:018) RightSquare |]|
//@[018:019) NewLine |\n|
    objParam: {}
//@[004:012) Identifier |objParam|
//@[012:013) Colon |:|
//@[014:015) LeftBrace |{|
//@[015:016) RightBrace |}|
//@[016:017) NewLine |\n|
    stringParamA: 'test'
//@[004:016) Identifier |stringParamA|
//@[016:017) Colon |:|
//@[018:024) StringComplete |'test'|
//@[024:025) NewLine |\n|
    stringParamB: 'test'
//@[004:016) Identifier |stringParamB|
//@[016:017) Colon |:|
//@[018:024) StringComplete |'test'|
//@[024:025) NewLine |\n|
    notAThing: 'test'
//@[004:013) Identifier |notAThing|
//@[013:014) Colon |:|
//@[015:021) StringComplete |'test'|
//@[021:022) NewLine |\n|
  }
//@[002:003) RightBrace |}|
//@[003:004) NewLine |\n|
}]
//@[000:001) RightBrace |}|
//@[001:002) RightSquare |]|
//@[002:003) NewLine |\n|
module wrongModuleParameterInLoop2 'modulea.bicep' = [for (x,i) in emptyArray:{
//@[000:006) Identifier |module|
//@[007:034) Identifier |wrongModuleParameterInLoop2|
//@[035:050) StringComplete |'modulea.bicep'|
//@[051:052) Assignment |=|
//@[053:054) LeftSquare |[|
//@[054:057) Identifier |for|
//@[058:059) LeftParen |(|
//@[059:060) Identifier |x|
//@[060:061) Comma |,|
//@[061:062) Identifier |i|
//@[062:063) RightParen |)|
//@[064:066) Identifier |in|
//@[067:077) Identifier |emptyArray|
//@[077:078) Colon |:|
//@[078:079) LeftBrace |{|
//@[079:080) NewLine |\n|
  name: 'hello-${x}'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:017) StringLeftPiece |'hello-${|
//@[017:018) Identifier |x|
//@[018:020) StringRightPiece |}'|
//@[020:021) NewLine |\n|
  params: {
//@[002:008) Identifier |params|
//@[008:009) Colon |:|
//@[010:011) LeftBrace |{|
//@[011:012) NewLine |\n|
    arrayParam: [
//@[004:014) Identifier |arrayParam|
//@[014:015) Colon |:|
//@[016:017) LeftSquare |[|
//@[017:018) NewLine |\n|
      i
//@[006:007) Identifier |i|
//@[007:008) NewLine |\n|
    ]
//@[004:005) RightSquare |]|
//@[005:006) NewLine |\n|
    objParam: {}
//@[004:012) Identifier |objParam|
//@[012:013) Colon |:|
//@[014:015) LeftBrace |{|
//@[015:016) RightBrace |}|
//@[016:017) NewLine |\n|
    stringParamA: 'test'
//@[004:016) Identifier |stringParamA|
//@[016:017) Colon |:|
//@[018:024) StringComplete |'test'|
//@[024:025) NewLine |\n|
    stringParamB: 'test'
//@[004:016) Identifier |stringParamB|
//@[016:017) Colon |:|
//@[018:024) StringComplete |'test'|
//@[024:025) NewLine |\n|
    notAThing: 'test'
//@[004:013) Identifier |notAThing|
//@[013:014) Colon |:|
//@[015:021) StringComplete |'test'|
//@[021:022) NewLine |\n|
  }
//@[002:003) RightBrace |}|
//@[003:004) NewLine |\n|
}]
//@[000:001) RightBrace |}|
//@[001:002) RightSquare |]|
//@[002:004) NewLine |\n\n|

module paramNameCompletionsInFilteredLoops 'modulea.bicep' = [for (x,i) in emptyArray: if(true) {
//@[000:006) Identifier |module|
//@[007:042) Identifier |paramNameCompletionsInFilteredLoops|
//@[043:058) StringComplete |'modulea.bicep'|
//@[059:060) Assignment |=|
//@[061:062) LeftSquare |[|
//@[062:065) Identifier |for|
//@[066:067) LeftParen |(|
//@[067:068) Identifier |x|
//@[068:069) Comma |,|
//@[069:070) Identifier |i|
//@[070:071) RightParen |)|
//@[072:074) Identifier |in|
//@[075:085) Identifier |emptyArray|
//@[085:086) Colon |:|
//@[087:089) Identifier |if|
//@[089:090) LeftParen |(|
//@[090:094) TrueKeyword |true|
//@[094:095) RightParen |)|
//@[096:097) LeftBrace |{|
//@[097:098) NewLine |\n|
  name: 'hello-${x}'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:017) StringLeftPiece |'hello-${|
//@[017:018) Identifier |x|
//@[018:020) StringRightPiece |}'|
//@[020:021) NewLine |\n|
  params: {
//@[002:008) Identifier |params|
//@[008:009) Colon |:|
//@[010:011) LeftBrace |{|
//@[011:012) NewLine |\n|
    // #completionTest(0,1,2) -> moduleAParams
//@[046:047) NewLine |\n|
  
//@[002:003) NewLine |\n|
  }
//@[002:003) RightBrace |}|
//@[003:004) NewLine |\n|
}]
//@[000:001) RightBrace |}|
//@[001:002) RightSquare |]|
//@[002:004) NewLine |\n\n|

// #completionTest(100) -> moduleAOutputs
//@[041:042) NewLine |\n|
var propertyAccessCompletionsForFilteredModuleLoop = paramNameCompletionsInFilteredLoops[0].outputs.
//@[000:003) Identifier |var|
//@[004:050) Identifier |propertyAccessCompletionsForFilteredModuleLoop|
//@[051:052) Assignment |=|
//@[053:088) Identifier |paramNameCompletionsInFilteredLoops|
//@[088:089) LeftSquare |[|
//@[089:090) Integer |0|
//@[090:091) RightSquare |]|
//@[091:092) Dot |.|
//@[092:099) Identifier |outputs|
//@[099:100) Dot |.|
//@[100:102) NewLine |\n\n|

// nonexistent arrays and loop variables
//@[040:041) NewLine |\n|
var evenMoreDuplicates = 'there'
//@[000:003) Identifier |var|
//@[004:022) Identifier |evenMoreDuplicates|
//@[023:024) Assignment |=|
//@[025:032) StringComplete |'there'|
//@[032:033) NewLine |\n|
module nonexistentArrays 'modulea.bicep' = [for evenMoreDuplicates in alsoDoesNotExist: {
//@[000:006) Identifier |module|
//@[007:024) Identifier |nonexistentArrays|
//@[025:040) StringComplete |'modulea.bicep'|
//@[041:042) Assignment |=|
//@[043:044) LeftSquare |[|
//@[044:047) Identifier |for|
//@[048:066) Identifier |evenMoreDuplicates|
//@[067:069) Identifier |in|
//@[070:086) Identifier |alsoDoesNotExist|
//@[086:087) Colon |:|
//@[088:089) LeftBrace |{|
//@[089:090) NewLine |\n|
  name: 'hello-${whyChooseRealVariablesWhenWeCanPretend}'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:017) StringLeftPiece |'hello-${|
//@[017:055) Identifier |whyChooseRealVariablesWhenWeCanPretend|
//@[055:057) StringRightPiece |}'|
//@[057:058) NewLine |\n|
  params: {
//@[002:008) Identifier |params|
//@[008:009) Colon |:|
//@[010:011) LeftBrace |{|
//@[011:012) NewLine |\n|
    objParam: {}
//@[004:012) Identifier |objParam|
//@[012:013) Colon |:|
//@[014:015) LeftBrace |{|
//@[015:016) RightBrace |}|
//@[016:017) NewLine |\n|
    stringParamB: 'test'
//@[004:016) Identifier |stringParamB|
//@[016:017) Colon |:|
//@[018:024) StringComplete |'test'|
//@[024:025) NewLine |\n|
    arrayParam: [for evenMoreDuplicates in totallyFake: doesNotExist]
//@[004:014) Identifier |arrayParam|
//@[014:015) Colon |:|
//@[016:017) LeftSquare |[|
//@[017:020) Identifier |for|
//@[021:039) Identifier |evenMoreDuplicates|
//@[040:042) Identifier |in|
//@[043:054) Identifier |totallyFake|
//@[054:055) Colon |:|
//@[056:068) Identifier |doesNotExist|
//@[068:069) RightSquare |]|
//@[069:070) NewLine |\n|
  }
//@[002:003) RightBrace |}|
//@[003:004) NewLine |\n|
}]
//@[000:001) RightBrace |}|
//@[001:002) RightSquare |]|
//@[002:004) NewLine |\n\n|

output directRefToCollectionViaOutput array = nonexistentArrays
//@[000:006) Identifier |output|
//@[007:037) Identifier |directRefToCollectionViaOutput|
//@[038:043) Identifier |array|
//@[044:045) Assignment |=|
//@[046:063) Identifier |nonexistentArrays|
//@[063:065) NewLine |\n\n|

module directRefToCollectionViaSingleBody 'modulea.bicep' = {
//@[000:006) Identifier |module|
//@[007:041) Identifier |directRefToCollectionViaSingleBody|
//@[042:057) StringComplete |'modulea.bicep'|
//@[058:059) Assignment |=|
//@[060:061) LeftBrace |{|
//@[061:062) NewLine |\n|
  name: 'hello'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:015) StringComplete |'hello'|
//@[015:016) NewLine |\n|
  params: {
//@[002:008) Identifier |params|
//@[008:009) Colon |:|
//@[010:011) LeftBrace |{|
//@[011:012) NewLine |\n|
    arrayParam: concat(wrongModuleParameterInLoop, nonexistentArrays)
//@[004:014) Identifier |arrayParam|
//@[014:015) Colon |:|
//@[016:022) Identifier |concat|
//@[022:023) LeftParen |(|
//@[023:049) Identifier |wrongModuleParameterInLoop|
//@[049:050) Comma |,|
//@[051:068) Identifier |nonexistentArrays|
//@[068:069) RightParen |)|
//@[069:070) NewLine |\n|
    objParam: {}
//@[004:012) Identifier |objParam|
//@[012:013) Colon |:|
//@[014:015) LeftBrace |{|
//@[015:016) RightBrace |}|
//@[016:017) NewLine |\n|
    stringParamB: ''
//@[004:016) Identifier |stringParamB|
//@[016:017) Colon |:|
//@[018:020) StringComplete |''|
//@[020:021) NewLine |\n|
  }
//@[002:003) RightBrace |}|
//@[003:004) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

module directRefToCollectionViaSingleConditionalBody 'modulea.bicep' = if(true) {
//@[000:006) Identifier |module|
//@[007:052) Identifier |directRefToCollectionViaSingleConditionalBody|
//@[053:068) StringComplete |'modulea.bicep'|
//@[069:070) Assignment |=|
//@[071:073) Identifier |if|
//@[073:074) LeftParen |(|
//@[074:078) TrueKeyword |true|
//@[078:079) RightParen |)|
//@[080:081) LeftBrace |{|
//@[081:082) NewLine |\n|
  name: 'hello2'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:016) StringComplete |'hello2'|
//@[016:017) NewLine |\n|
  params: {
//@[002:008) Identifier |params|
//@[008:009) Colon |:|
//@[010:011) LeftBrace |{|
//@[011:012) NewLine |\n|
    arrayParam: concat(wrongModuleParameterInLoop, nonexistentArrays)
//@[004:014) Identifier |arrayParam|
//@[014:015) Colon |:|
//@[016:022) Identifier |concat|
//@[022:023) LeftParen |(|
//@[023:049) Identifier |wrongModuleParameterInLoop|
//@[049:050) Comma |,|
//@[051:068) Identifier |nonexistentArrays|
//@[068:069) RightParen |)|
//@[069:070) NewLine |\n|
    objParam: {}
//@[004:012) Identifier |objParam|
//@[012:013) Colon |:|
//@[014:015) LeftBrace |{|
//@[015:016) RightBrace |}|
//@[016:017) NewLine |\n|
    stringParamB: ''
//@[004:016) Identifier |stringParamB|
//@[016:017) Colon |:|
//@[018:020) StringComplete |''|
//@[020:021) NewLine |\n|
  }
//@[002:003) RightBrace |}|
//@[003:004) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

module directRefToCollectionViaLoopBody 'modulea.bicep' = [for test in []: {
//@[000:006) Identifier |module|
//@[007:039) Identifier |directRefToCollectionViaLoopBody|
//@[040:055) StringComplete |'modulea.bicep'|
//@[056:057) Assignment |=|
//@[058:059) LeftSquare |[|
//@[059:062) Identifier |for|
//@[063:067) Identifier |test|
//@[068:070) Identifier |in|
//@[071:072) LeftSquare |[|
//@[072:073) RightSquare |]|
//@[073:074) Colon |:|
//@[075:076) LeftBrace |{|
//@[076:077) NewLine |\n|
  name: 'hello3'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:016) StringComplete |'hello3'|
//@[016:017) NewLine |\n|
  params: {
//@[002:008) Identifier |params|
//@[008:009) Colon |:|
//@[010:011) LeftBrace |{|
//@[011:012) NewLine |\n|
    arrayParam: concat(wrongModuleParameterInLoop, nonexistentArrays)
//@[004:014) Identifier |arrayParam|
//@[014:015) Colon |:|
//@[016:022) Identifier |concat|
//@[022:023) LeftParen |(|
//@[023:049) Identifier |wrongModuleParameterInLoop|
//@[049:050) Comma |,|
//@[051:068) Identifier |nonexistentArrays|
//@[068:069) RightParen |)|
//@[069:070) NewLine |\n|
    objParam: {}
//@[004:012) Identifier |objParam|
//@[012:013) Colon |:|
//@[014:015) LeftBrace |{|
//@[015:016) RightBrace |}|
//@[016:017) NewLine |\n|
    stringParamB: ''
//@[004:016) Identifier |stringParamB|
//@[016:017) Colon |:|
//@[018:020) StringComplete |''|
//@[020:021) NewLine |\n|
  }
//@[002:003) RightBrace |}|
//@[003:004) NewLine |\n|
}]
//@[000:001) RightBrace |}|
//@[001:002) RightSquare |]|
//@[002:004) NewLine |\n\n|

module directRefToCollectionViaLoopBodyWithExtraDependsOn 'modulea.bicep' = [for test in []: {
//@[000:006) Identifier |module|
//@[007:057) Identifier |directRefToCollectionViaLoopBodyWithExtraDependsOn|
//@[058:073) StringComplete |'modulea.bicep'|
//@[074:075) Assignment |=|
//@[076:077) LeftSquare |[|
//@[077:080) Identifier |for|
//@[081:085) Identifier |test|
//@[086:088) Identifier |in|
//@[089:090) LeftSquare |[|
//@[090:091) RightSquare |]|
//@[091:092) Colon |:|
//@[093:094) LeftBrace |{|
//@[094:095) NewLine |\n|
  name: 'hello4'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:016) StringComplete |'hello4'|
//@[016:017) NewLine |\n|
  params: {
//@[002:008) Identifier |params|
//@[008:009) Colon |:|
//@[010:011) LeftBrace |{|
//@[011:012) NewLine |\n|
    arrayParam: concat(wrongModuleParameterInLoop, nonexistentArrays)
//@[004:014) Identifier |arrayParam|
//@[014:015) Colon |:|
//@[016:022) Identifier |concat|
//@[022:023) LeftParen |(|
//@[023:049) Identifier |wrongModuleParameterInLoop|
//@[049:050) Comma |,|
//@[051:068) Identifier |nonexistentArrays|
//@[068:069) RightParen |)|
//@[069:070) NewLine |\n|
    objParam: {}
//@[004:012) Identifier |objParam|
//@[012:013) Colon |:|
//@[014:015) LeftBrace |{|
//@[015:016) RightBrace |}|
//@[016:017) NewLine |\n|
    stringParamB: ''
//@[004:016) Identifier |stringParamB|
//@[016:017) Colon |:|
//@[018:020) StringComplete |''|
//@[020:021) NewLine |\n|
    dependsOn: [
//@[004:013) Identifier |dependsOn|
//@[013:014) Colon |:|
//@[015:016) LeftSquare |[|
//@[016:017) NewLine |\n|
      nonexistentArrays
//@[006:023) Identifier |nonexistentArrays|
//@[023:024) NewLine |\n|
    ]
//@[004:005) RightSquare |]|
//@[005:006) NewLine |\n|
  }
//@[002:003) RightBrace |}|
//@[003:004) NewLine |\n|
  dependsOn: [
//@[002:011) Identifier |dependsOn|
//@[011:012) Colon |:|
//@[013:014) LeftSquare |[|
//@[014:015) NewLine |\n|
    
//@[004:005) NewLine |\n|
  ]
//@[002:003) RightSquare |]|
//@[003:004) NewLine |\n|
}]
//@[000:001) RightBrace |}|
//@[001:002) RightSquare |]|
//@[002:005) NewLine |\n\n\n|


// module body that isn't an object
//@[035:036) NewLine |\n|
module nonObjectModuleBody 'modulea.bicep' = [for thing in []: 'hello']
//@[000:006) Identifier |module|
//@[007:026) Identifier |nonObjectModuleBody|
//@[027:042) StringComplete |'modulea.bicep'|
//@[043:044) Assignment |=|
//@[045:046) LeftSquare |[|
//@[046:049) Identifier |for|
//@[050:055) Identifier |thing|
//@[056:058) Identifier |in|
//@[059:060) LeftSquare |[|
//@[060:061) RightSquare |]|
//@[061:062) Colon |:|
//@[063:070) StringComplete |'hello'|
//@[070:071) RightSquare |]|
//@[071:072) NewLine |\n|
module nonObjectModuleBody2 'modulea.bicep' = [for thing in []: concat()]
//@[000:006) Identifier |module|
//@[007:027) Identifier |nonObjectModuleBody2|
//@[028:043) StringComplete |'modulea.bicep'|
//@[044:045) Assignment |=|
//@[046:047) LeftSquare |[|
//@[047:050) Identifier |for|
//@[051:056) Identifier |thing|
//@[057:059) Identifier |in|
//@[060:061) LeftSquare |[|
//@[061:062) RightSquare |]|
//@[062:063) Colon |:|
//@[064:070) Identifier |concat|
//@[070:071) LeftParen |(|
//@[071:072) RightParen |)|
//@[072:073) RightSquare |]|
//@[073:074) NewLine |\n|
module nonObjectModuleBody3 'modulea.bicep' = [for (thing,i) in []: 'hello']
//@[000:006) Identifier |module|
//@[007:027) Identifier |nonObjectModuleBody3|
//@[028:043) StringComplete |'modulea.bicep'|
//@[044:045) Assignment |=|
//@[046:047) LeftSquare |[|
//@[047:050) Identifier |for|
//@[051:052) LeftParen |(|
//@[052:057) Identifier |thing|
//@[057:058) Comma |,|
//@[058:059) Identifier |i|
//@[059:060) RightParen |)|
//@[061:063) Identifier |in|
//@[064:065) LeftSquare |[|
//@[065:066) RightSquare |]|
//@[066:067) Colon |:|
//@[068:075) StringComplete |'hello'|
//@[075:076) RightSquare |]|
//@[076:077) NewLine |\n|
module nonObjectModuleBody4 'modulea.bicep' = [for (thing,i) in []: concat()]
//@[000:006) Identifier |module|
//@[007:027) Identifier |nonObjectModuleBody4|
//@[028:043) StringComplete |'modulea.bicep'|
//@[044:045) Assignment |=|
//@[046:047) LeftSquare |[|
//@[047:050) Identifier |for|
//@[051:052) LeftParen |(|
//@[052:057) Identifier |thing|
//@[057:058) Comma |,|
//@[058:059) Identifier |i|
//@[059:060) RightParen |)|
//@[061:063) Identifier |in|
//@[064:065) LeftSquare |[|
//@[065:066) RightSquare |]|
//@[066:067) Colon |:|
//@[068:074) Identifier |concat|
//@[074:075) LeftParen |(|
//@[075:076) RightParen |)|
//@[076:077) RightSquare |]|
//@[077:079) NewLine |\n\n|

module anyTypeInScope 'empty.bicep' = {
//@[000:006) Identifier |module|
//@[007:021) Identifier |anyTypeInScope|
//@[022:035) StringComplete |'empty.bicep'|
//@[036:037) Assignment |=|
//@[038:039) LeftBrace |{|
//@[039:040) NewLine |\n|
  dependsOn: [
//@[002:011) Identifier |dependsOn|
//@[011:012) Colon |:|
//@[013:014) LeftSquare |[|
//@[014:015) NewLine |\n|
    any('s')
//@[004:007) Identifier |any|
//@[007:008) LeftParen |(|
//@[008:011) StringComplete |'s'|
//@[011:012) RightParen |)|
//@[012:013) NewLine |\n|
  ]
//@[002:003) RightSquare |]|
//@[003:005) NewLine |\n\n|

  scope: any(42)
//@[002:007) Identifier |scope|
//@[007:008) Colon |:|
//@[009:012) Identifier |any|
//@[012:013) LeftParen |(|
//@[013:015) Integer |42|
//@[015:016) RightParen |)|
//@[016:017) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

module anyTypeInScopeConditional 'empty.bicep' = if(false) {
//@[000:006) Identifier |module|
//@[007:032) Identifier |anyTypeInScopeConditional|
//@[033:046) StringComplete |'empty.bicep'|
//@[047:048) Assignment |=|
//@[049:051) Identifier |if|
//@[051:052) LeftParen |(|
//@[052:057) FalseKeyword |false|
//@[057:058) RightParen |)|
//@[059:060) LeftBrace |{|
//@[060:061) NewLine |\n|
  dependsOn: [
//@[002:011) Identifier |dependsOn|
//@[011:012) Colon |:|
//@[013:014) LeftSquare |[|
//@[014:015) NewLine |\n|
    any('s')
//@[004:007) Identifier |any|
//@[007:008) LeftParen |(|
//@[008:011) StringComplete |'s'|
//@[011:012) RightParen |)|
//@[012:013) NewLine |\n|
  ]
//@[002:003) RightSquare |]|
//@[003:005) NewLine |\n\n|

  scope: any(42)
//@[002:007) Identifier |scope|
//@[007:008) Colon |:|
//@[009:012) Identifier |any|
//@[012:013) LeftParen |(|
//@[013:015) Integer |42|
//@[015:016) RightParen |)|
//@[016:017) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

module anyTypeInScopeLoop 'empty.bicep' = [for thing in []: {
//@[000:006) Identifier |module|
//@[007:025) Identifier |anyTypeInScopeLoop|
//@[026:039) StringComplete |'empty.bicep'|
//@[040:041) Assignment |=|
//@[042:043) LeftSquare |[|
//@[043:046) Identifier |for|
//@[047:052) Identifier |thing|
//@[053:055) Identifier |in|
//@[056:057) LeftSquare |[|
//@[057:058) RightSquare |]|
//@[058:059) Colon |:|
//@[060:061) LeftBrace |{|
//@[061:062) NewLine |\n|
  dependsOn: [
//@[002:011) Identifier |dependsOn|
//@[011:012) Colon |:|
//@[013:014) LeftSquare |[|
//@[014:015) NewLine |\n|
    any('s')
//@[004:007) Identifier |any|
//@[007:008) LeftParen |(|
//@[008:011) StringComplete |'s'|
//@[011:012) RightParen |)|
//@[012:013) NewLine |\n|
  ]
//@[002:003) RightSquare |]|
//@[003:005) NewLine |\n\n|

  scope: any(42)
//@[002:007) Identifier |scope|
//@[007:008) Colon |:|
//@[009:012) Identifier |any|
//@[012:013) LeftParen |(|
//@[013:015) Integer |42|
//@[015:016) RightParen |)|
//@[016:017) NewLine |\n|
}]
//@[000:001) RightBrace |}|
//@[001:002) RightSquare |]|
//@[002:004) NewLine |\n\n|

// Key Vault Secret Reference
//@[029:031) NewLine |\n\n|

resource kv 'Microsoft.KeyVault/vaults@2019-09-01' existing = {
//@[000:008) Identifier |resource|
//@[009:011) Identifier |kv|
//@[012:050) StringComplete |'Microsoft.KeyVault/vaults@2019-09-01'|
//@[051:059) Identifier |existing|
//@[060:061) Assignment |=|
//@[062:063) LeftBrace |{|
//@[063:064) NewLine |\n|
  name: 'testkeyvault'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:022) StringComplete |'testkeyvault'|
//@[022:023) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

module secureModule1 'moduleb.bicep' = {
//@[000:006) Identifier |module|
//@[007:020) Identifier |secureModule1|
//@[021:036) StringComplete |'moduleb.bicep'|
//@[037:038) Assignment |=|
//@[039:040) LeftBrace |{|
//@[040:041) NewLine |\n|
  name: 'secureModule1'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:023) StringComplete |'secureModule1'|
//@[023:024) NewLine |\n|
  params: {       
//@[002:008) Identifier |params|
//@[008:009) Colon |:|
//@[010:011) LeftBrace |{|
//@[018:019) NewLine |\n|
    stringParamA: kv.getSecret('mySecret')
//@[004:016) Identifier |stringParamA|
//@[016:017) Colon |:|
//@[018:020) Identifier |kv|
//@[020:021) Dot |.|
//@[021:030) Identifier |getSecret|
//@[030:031) LeftParen |(|
//@[031:041) StringComplete |'mySecret'|
//@[041:042) RightParen |)|
//@[042:043) NewLine |\n|
    stringParamB: '${kv.getSecret('mySecret')}'
//@[004:016) Identifier |stringParamB|
//@[016:017) Colon |:|
//@[018:021) StringLeftPiece |'${|
//@[021:023) Identifier |kv|
//@[023:024) Dot |.|
//@[024:033) Identifier |getSecret|
//@[033:034) LeftParen |(|
//@[034:044) StringComplete |'mySecret'|
//@[044:045) RightParen |)|
//@[045:047) StringRightPiece |}'|
//@[047:048) NewLine |\n|
    objParam: kv.getSecret('mySecret')
//@[004:012) Identifier |objParam|
//@[012:013) Colon |:|
//@[014:016) Identifier |kv|
//@[016:017) Dot |.|
//@[017:026) Identifier |getSecret|
//@[026:027) LeftParen |(|
//@[027:037) StringComplete |'mySecret'|
//@[037:038) RightParen |)|
//@[038:039) NewLine |\n|
    arrayParam: kv.getSecret('mySecret')
//@[004:014) Identifier |arrayParam|
//@[014:015) Colon |:|
//@[016:018) Identifier |kv|
//@[018:019) Dot |.|
//@[019:028) Identifier |getSecret|
//@[028:029) LeftParen |(|
//@[029:039) StringComplete |'mySecret'|
//@[039:040) RightParen |)|
//@[040:041) NewLine |\n|
    secureStringParam: '${kv.getSecret('mySecret')}'
//@[004:021) Identifier |secureStringParam|
//@[021:022) Colon |:|
//@[023:026) StringLeftPiece |'${|
//@[026:028) Identifier |kv|
//@[028:029) Dot |.|
//@[029:038) Identifier |getSecret|
//@[038:039) LeftParen |(|
//@[039:049) StringComplete |'mySecret'|
//@[049:050) RightParen |)|
//@[050:052) StringRightPiece |}'|
//@[052:053) NewLine |\n|
    secureObjectParam: kv.getSecret('mySecret')
//@[004:021) Identifier |secureObjectParam|
//@[021:022) Colon |:|
//@[023:025) Identifier |kv|
//@[025:026) Dot |.|
//@[026:035) Identifier |getSecret|
//@[035:036) LeftParen |(|
//@[036:046) StringComplete |'mySecret'|
//@[046:047) RightParen |)|
//@[047:048) NewLine |\n|
    secureStringParam2: '${kv.getSecret('mySecret')}'
//@[004:022) Identifier |secureStringParam2|
//@[022:023) Colon |:|
//@[024:027) StringLeftPiece |'${|
//@[027:029) Identifier |kv|
//@[029:030) Dot |.|
//@[030:039) Identifier |getSecret|
//@[039:040) LeftParen |(|
//@[040:050) StringComplete |'mySecret'|
//@[050:051) RightParen |)|
//@[051:053) StringRightPiece |}'|
//@[053:054) NewLine |\n|
    secureObjectParam2: kv.getSecret('mySecret')
//@[004:022) Identifier |secureObjectParam2|
//@[022:023) Colon |:|
//@[024:026) Identifier |kv|
//@[026:027) Dot |.|
//@[027:036) Identifier |getSecret|
//@[036:037) LeftParen |(|
//@[037:047) StringComplete |'mySecret'|
//@[047:048) RightParen |)|
//@[048:049) NewLine |\n|
  }
//@[002:003) RightBrace |}|
//@[003:004) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

module secureModule2 'BAD_MODULE_PATH.bicep' = {
//@[000:006) Identifier |module|
//@[007:020) Identifier |secureModule2|
//@[021:044) StringComplete |'BAD_MODULE_PATH.bicep'|
//@[045:046) Assignment |=|
//@[047:048) LeftBrace |{|
//@[048:049) NewLine |\n|
  name: 'secureModule2'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:023) StringComplete |'secureModule2'|
//@[023:024) NewLine |\n|
  params: {       
//@[002:008) Identifier |params|
//@[008:009) Colon |:|
//@[010:011) LeftBrace |{|
//@[018:019) NewLine |\n|
    secret: kv.getSecret('mySecret')
//@[004:010) Identifier |secret|
//@[010:011) Colon |:|
//@[012:014) Identifier |kv|
//@[014:015) Dot |.|
//@[015:024) Identifier |getSecret|
//@[024:025) LeftParen |(|
//@[025:035) StringComplete |'mySecret'|
//@[035:036) RightParen |)|
//@[036:037) NewLine |\n|
  }
//@[002:003) RightBrace |}|
//@[003:004) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

module issue3000 'empty.bicep' = {
//@[000:006) Identifier |module|
//@[007:016) Identifier |issue3000|
//@[017:030) StringComplete |'empty.bicep'|
//@[031:032) Assignment |=|
//@[033:034) LeftBrace |{|
//@[034:035) NewLine |\n|
  name: 'issue3000Module'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:025) StringComplete |'issue3000Module'|
//@[025:026) NewLine |\n|
  params: {}
//@[002:008) Identifier |params|
//@[008:009) Colon |:|
//@[010:011) LeftBrace |{|
//@[011:012) RightBrace |}|
//@[012:013) NewLine |\n|
  identity: {
//@[002:010) Identifier |identity|
//@[010:011) Colon |:|
//@[012:013) LeftBrace |{|
//@[013:014) NewLine |\n|
    type: 'SystemAssigned'
//@[004:008) Identifier |type|
//@[008:009) Colon |:|
//@[010:026) StringComplete |'SystemAssigned'|
//@[026:027) NewLine |\n|
  }
//@[002:003) RightBrace |}|
//@[003:004) NewLine |\n|
  extendedLocation: {}
//@[002:018) Identifier |extendedLocation|
//@[018:019) Colon |:|
//@[020:021) LeftBrace |{|
//@[021:022) RightBrace |}|
//@[022:023) NewLine |\n|
  sku: {}
//@[002:005) Identifier |sku|
//@[005:006) Colon |:|
//@[007:008) LeftBrace |{|
//@[008:009) RightBrace |}|
//@[009:010) NewLine |\n|
  kind: 'V1'
//@[002:006) Identifier |kind|
//@[006:007) Colon |:|
//@[008:012) StringComplete |'V1'|
//@[012:013) NewLine |\n|
  managedBy: 'string'
//@[002:011) Identifier |managedBy|
//@[011:012) Colon |:|
//@[013:021) StringComplete |'string'|
//@[021:022) NewLine |\n|
  mangedByExtended: [
//@[002:018) Identifier |mangedByExtended|
//@[018:019) Colon |:|
//@[020:021) LeftSquare |[|
//@[021:022) NewLine |\n|
   'str1'
//@[003:009) StringComplete |'str1'|
//@[009:010) NewLine |\n|
   'str2'
//@[003:009) StringComplete |'str2'|
//@[009:010) NewLine |\n|
  ]
//@[002:003) RightSquare |]|
//@[003:004) NewLine |\n|
  zones: [
//@[002:007) Identifier |zones|
//@[007:008) Colon |:|
//@[009:010) LeftSquare |[|
//@[010:011) NewLine |\n|
   'str1'
//@[003:009) StringComplete |'str1'|
//@[009:010) NewLine |\n|
   'str2'
//@[003:009) StringComplete |'str2'|
//@[009:010) NewLine |\n|
  ]
//@[002:003) RightSquare |]|
//@[003:004) NewLine |\n|
  plan: {}
//@[002:006) Identifier |plan|
//@[006:007) Colon |:|
//@[008:009) LeftBrace |{|
//@[009:010) RightBrace |}|
//@[010:011) NewLine |\n|
  eTag: ''
//@[002:006) Identifier |eTag|
//@[006:007) Colon |:|
//@[008:010) StringComplete |''|
//@[010:011) NewLine |\n|
  scale: {}  
//@[002:007) Identifier |scale|
//@[007:008) Colon |:|
//@[009:010) LeftBrace |{|
//@[010:011) RightBrace |}|
//@[013:014) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

module invalidJsonMod 'modulec.json' = {
//@[000:006) Identifier |module|
//@[007:021) Identifier |invalidJsonMod|
//@[022:036) StringComplete |'modulec.json'|
//@[037:038) Assignment |=|
//@[039:040) LeftBrace |{|
//@[040:041) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

module jsonModMissingParam 'moduled.json' = {
//@[000:006) Identifier |module|
//@[007:026) Identifier |jsonModMissingParam|
//@[027:041) StringComplete |'moduled.json'|
//@[042:043) Assignment |=|
//@[044:045) LeftBrace |{|
//@[045:046) NewLine |\n|
  name: 'jsonModMissingParam'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:029) StringComplete |'jsonModMissingParam'|
//@[029:030) NewLine |\n|
  params: {
//@[002:008) Identifier |params|
//@[008:009) Colon |:|
//@[010:011) LeftBrace |{|
//@[011:012) NewLine |\n|
    foo: 123
//@[004:007) Identifier |foo|
//@[007:008) Colon |:|
//@[009:012) Integer |123|
//@[012:013) NewLine |\n|
    baz: 'C'
//@[004:007) Identifier |baz|
//@[007:008) Colon |:|
//@[009:012) StringComplete |'C'|
//@[012:013) NewLine |\n|
  }
//@[002:003) RightBrace |}|
//@[003:004) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

module assignToOutput 'empty.bicep' = {
//@[000:006) Identifier |module|
//@[007:021) Identifier |assignToOutput|
//@[022:035) StringComplete |'empty.bicep'|
//@[036:037) Assignment |=|
//@[038:039) LeftBrace |{|
//@[039:040) NewLine |\n|
  name: 'assignToOutput'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:024) StringComplete |'assignToOutput'|
//@[024:025) NewLine |\n|
  outputs: {}
//@[002:009) Identifier |outputs|
//@[009:010) Colon |:|
//@[011:012) LeftBrace |{|
//@[012:013) RightBrace |}|
//@[013:014) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:001) EndOfFile ||
