
//@[000:002) NewLine |\r\n|
// wrong declaration
//@[020:022) NewLine |\r\n|
bad
//@[000:003) Identifier |bad|
//@[003:007) NewLine |\r\n\r\n|

// incomplete #completionTest(7) -> empty
//@[041:043) NewLine |\r\n|
output 
//@[000:006) Identifier |output|
//@[007:011) NewLine |\r\n\r\n|

var testSymbol = 42
//@[000:003) Identifier |var|
//@[004:014) Identifier |testSymbol|
//@[015:016) Assignment |=|
//@[017:019) Integer |42|
//@[019:023) NewLine |\r\n\r\n|

// #completionTest(28,29) -> symbols
//@[036:038) NewLine |\r\n|
output missingValueAndType = 
//@[000:006) Identifier |output|
//@[007:026) Identifier |missingValueAndType|
//@[027:028) Assignment |=|
//@[029:033) NewLine |\r\n\r\n|

// #completionTest(28,29) -> symbols
//@[036:038) NewLine |\r\n|
output missingValue string = 
//@[000:006) Identifier |output|
//@[007:019) Identifier |missingValue|
//@[020:026) Identifier |string|
//@[027:028) Assignment |=|
//@[029:033) NewLine |\r\n\r\n|

// #completionTest(31,32) -> arrayPlusSymbols
//@[045:047) NewLine |\r\n|
output arrayCompletions array = 
//@[000:006) Identifier |output|
//@[007:023) Identifier |arrayCompletions|
//@[024:029) Identifier |array|
//@[030:031) Assignment |=|
//@[032:036) NewLine |\r\n\r\n|

// #completionTest(33,34) -> objectPlusSymbols
//@[046:048) NewLine |\r\n|
output objectCompletions object = 
//@[000:006) Identifier |output|
//@[007:024) Identifier |objectCompletions|
//@[025:031) Identifier |object|
//@[032:033) Assignment |=|
//@[034:038) NewLine |\r\n\r\n|

// #completionTest(29,30) -> boolPlusSymbols
//@[044:046) NewLine |\r\n|
output boolCompletions bool = 
//@[000:006) Identifier |output|
//@[007:022) Identifier |boolCompletions|
//@[023:027) Identifier |bool|
//@[028:029) Assignment |=|
//@[030:034) NewLine |\r\n\r\n|

output foo
//@[000:006) Identifier |output|
//@[007:010) Identifier |foo|
//@[010:014) NewLine |\r\n\r\n|

// space after identifier #completionTest(20) -> outputTypes
//@[060:062) NewLine |\r\n|
output spaceAfterId 
//@[000:006) Identifier |output|
//@[007:019) Identifier |spaceAfterId|
//@[020:024) NewLine |\r\n\r\n|

// #completionTest(25) -> outputTypes
//@[037:039) NewLine |\r\n|
output spacesAfterCursor  
//@[000:006) Identifier |output|
//@[007:024) Identifier |spacesAfterCursor|
//@[026:030) NewLine |\r\n\r\n|

// partial type #completionTest(19, 20, 21, 22) -> outputTypes
//@[062:064) NewLine |\r\n|
output partialType obj
//@[000:006) Identifier |output|
//@[007:018) Identifier |partialType|
//@[019:022) Identifier |obj|
//@[022:026) NewLine |\r\n\r\n|

// malformed identifier
//@[023:025) NewLine |\r\n|
output 2
//@[000:006) Identifier |output|
//@[007:008) Integer |2|
//@[008:012) NewLine |\r\n\r\n|

// malformed type
//@[017:019) NewLine |\r\n|
output malformedType 3
//@[000:006) Identifier |output|
//@[007:020) Identifier |malformedType|
//@[021:022) Integer |3|
//@[022:026) NewLine |\r\n\r\n|

// malformed type but type check should still happen
//@[052:054) NewLine |\r\n|
output malformedType2 3 = 2 + null
//@[000:006) Identifier |output|
//@[007:021) Identifier |malformedType2|
//@[022:023) Integer |3|
//@[024:025) Assignment |=|
//@[026:027) Integer |2|
//@[028:029) Plus |+|
//@[030:034) NullKeyword |null|
//@[034:038) NewLine |\r\n\r\n|

// malformed type assignment
//@[028:030) NewLine |\r\n|
output malformedAssignment 2 = 2
//@[000:006) Identifier |output|
//@[007:026) Identifier |malformedAssignment|
//@[027:028) Integer |2|
//@[029:030) Assignment |=|
//@[031:032) Integer |2|
//@[032:036) NewLine |\r\n\r\n|

// malformed type before assignment
//@[035:037) NewLine |\r\n|
output lol 2 = true
//@[000:006) Identifier |output|
//@[007:010) Identifier |lol|
//@[011:012) Integer |2|
//@[013:014) Assignment |=|
//@[015:019) TrueKeyword |true|
//@[019:023) NewLine |\r\n\r\n|

// wrong type + missing value
//@[029:031) NewLine |\r\n|
output foo fluffy
//@[000:006) Identifier |output|
//@[007:010) Identifier |foo|
//@[011:017) Identifier |fluffy|
//@[017:021) NewLine |\r\n\r\n|

// missing value
//@[016:018) NewLine |\r\n|
output foo string
//@[000:006) Identifier |output|
//@[007:010) Identifier |foo|
//@[011:017) Identifier |string|
//@[017:021) NewLine |\r\n\r\n|

// missing value
//@[016:018) NewLine |\r\n|
output foo string =
//@[000:006) Identifier |output|
//@[007:010) Identifier |foo|
//@[011:017) Identifier |string|
//@[018:019) Assignment |=|
//@[019:023) NewLine |\r\n\r\n|

// wrong string output values
//@[029:031) NewLine |\r\n|
output str string = true
//@[000:006) Identifier |output|
//@[007:010) Identifier |str|
//@[011:017) Identifier |string|
//@[018:019) Assignment |=|
//@[020:024) TrueKeyword |true|
//@[024:026) NewLine |\r\n|
output str string = false
//@[000:006) Identifier |output|
//@[007:010) Identifier |str|
//@[011:017) Identifier |string|
//@[018:019) Assignment |=|
//@[020:025) FalseKeyword |false|
//@[025:027) NewLine |\r\n|
output str string = [
//@[000:006) Identifier |output|
//@[007:010) Identifier |str|
//@[011:017) Identifier |string|
//@[018:019) Assignment |=|
//@[020:021) LeftSquare |[|
//@[021:023) NewLine |\r\n|
]
//@[000:001) RightSquare |]|
//@[001:003) NewLine |\r\n|
output str string = {
//@[000:006) Identifier |output|
//@[007:010) Identifier |str|
//@[011:017) Identifier |string|
//@[018:019) Assignment |=|
//@[020:021) LeftBrace |{|
//@[021:023) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\r\n|
output str string = 52
//@[000:006) Identifier |output|
//@[007:010) Identifier |str|
//@[011:017) Identifier |string|
//@[018:019) Assignment |=|
//@[020:022) Integer |52|
//@[022:026) NewLine |\r\n\r\n|

// wrong int output values
//@[026:028) NewLine |\r\n|
output i int = true
//@[000:006) Identifier |output|
//@[007:008) Identifier |i|
//@[009:012) Identifier |int|
//@[013:014) Assignment |=|
//@[015:019) TrueKeyword |true|
//@[019:021) NewLine |\r\n|
output i int = false
//@[000:006) Identifier |output|
//@[007:008) Identifier |i|
//@[009:012) Identifier |int|
//@[013:014) Assignment |=|
//@[015:020) FalseKeyword |false|
//@[020:022) NewLine |\r\n|
output i int = [
//@[000:006) Identifier |output|
//@[007:008) Identifier |i|
//@[009:012) Identifier |int|
//@[013:014) Assignment |=|
//@[015:016) LeftSquare |[|
//@[016:018) NewLine |\r\n|
]
//@[000:001) RightSquare |]|
//@[001:003) NewLine |\r\n|
output i int = }
//@[000:006) Identifier |output|
//@[007:008) Identifier |i|
//@[009:012) Identifier |int|
//@[013:014) Assignment |=|
//@[015:016) RightBrace |}|
//@[016:018) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\r\n|
output i int = 'test'
//@[000:006) Identifier |output|
//@[007:008) Identifier |i|
//@[009:012) Identifier |int|
//@[013:014) Assignment |=|
//@[015:021) StringComplete |'test'|
//@[021:025) NewLine |\r\n\r\n|

// wrong bool output values
//@[027:029) NewLine |\r\n|
output b bool = [
//@[000:006) Identifier |output|
//@[007:008) Identifier |b|
//@[009:013) Identifier |bool|
//@[014:015) Assignment |=|
//@[016:017) LeftSquare |[|
//@[017:019) NewLine |\r\n|
]
//@[000:001) RightSquare |]|
//@[001:003) NewLine |\r\n|
output b bool = {
//@[000:006) Identifier |output|
//@[007:008) Identifier |b|
//@[009:013) Identifier |bool|
//@[014:015) Assignment |=|
//@[016:017) LeftBrace |{|
//@[017:019) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\r\n|
output b bool = 32
//@[000:006) Identifier |output|
//@[007:008) Identifier |b|
//@[009:013) Identifier |bool|
//@[014:015) Assignment |=|
//@[016:018) Integer |32|
//@[018:020) NewLine |\r\n|
output b bool = 'str'
//@[000:006) Identifier |output|
//@[007:008) Identifier |b|
//@[009:013) Identifier |bool|
//@[014:015) Assignment |=|
//@[016:021) StringComplete |'str'|
//@[021:025) NewLine |\r\n\r\n|

// wrong array output values
//@[028:030) NewLine |\r\n|
output arr array = 32
//@[000:006) Identifier |output|
//@[007:010) Identifier |arr|
//@[011:016) Identifier |array|
//@[017:018) Assignment |=|
//@[019:021) Integer |32|
//@[021:023) NewLine |\r\n|
output arr array = true
//@[000:006) Identifier |output|
//@[007:010) Identifier |arr|
//@[011:016) Identifier |array|
//@[017:018) Assignment |=|
//@[019:023) TrueKeyword |true|
//@[023:025) NewLine |\r\n|
output arr array = false
//@[000:006) Identifier |output|
//@[007:010) Identifier |arr|
//@[011:016) Identifier |array|
//@[017:018) Assignment |=|
//@[019:024) FalseKeyword |false|
//@[024:026) NewLine |\r\n|
output arr array = {
//@[000:006) Identifier |output|
//@[007:010) Identifier |arr|
//@[011:016) Identifier |array|
//@[017:018) Assignment |=|
//@[019:020) LeftBrace |{|
//@[020:022) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\r\n|
output arr array = 'str'
//@[000:006) Identifier |output|
//@[007:010) Identifier |arr|
//@[011:016) Identifier |array|
//@[017:018) Assignment |=|
//@[019:024) StringComplete |'str'|
//@[024:028) NewLine |\r\n\r\n|

// wrong object output values
//@[029:031) NewLine |\r\n|
output o object = 32
//@[000:006) Identifier |output|
//@[007:008) Identifier |o|
//@[009:015) Identifier |object|
//@[016:017) Assignment |=|
//@[018:020) Integer |32|
//@[020:022) NewLine |\r\n|
output o object = true
//@[000:006) Identifier |output|
//@[007:008) Identifier |o|
//@[009:015) Identifier |object|
//@[016:017) Assignment |=|
//@[018:022) TrueKeyword |true|
//@[022:024) NewLine |\r\n|
output o object = false
//@[000:006) Identifier |output|
//@[007:008) Identifier |o|
//@[009:015) Identifier |object|
//@[016:017) Assignment |=|
//@[018:023) FalseKeyword |false|
//@[023:025) NewLine |\r\n|
output o object = [
//@[000:006) Identifier |output|
//@[007:008) Identifier |o|
//@[009:015) Identifier |object|
//@[016:017) Assignment |=|
//@[018:019) LeftSquare |[|
//@[019:021) NewLine |\r\n|
]
//@[000:001) RightSquare |]|
//@[001:003) NewLine |\r\n|
output o object = 'str'
//@[000:006) Identifier |output|
//@[007:008) Identifier |o|
//@[009:015) Identifier |object|
//@[016:017) Assignment |=|
//@[018:023) StringComplete |'str'|
//@[023:027) NewLine |\r\n\r\n|

// a few expression cases
//@[025:027) NewLine |\r\n|
output exp string = 2 + 3
//@[000:006) Identifier |output|
//@[007:010) Identifier |exp|
//@[011:017) Identifier |string|
//@[018:019) Assignment |=|
//@[020:021) Integer |2|
//@[022:023) Plus |+|
//@[024:025) Integer |3|
//@[025:027) NewLine |\r\n|
output union string = true ? 's' : 1
//@[000:006) Identifier |output|
//@[007:012) Identifier |union|
//@[013:019) Identifier |string|
//@[020:021) Assignment |=|
//@[022:026) TrueKeyword |true|
//@[027:028) Question |?|
//@[029:032) StringComplete |'s'|
//@[033:034) Colon |:|
//@[035:036) Integer |1|
//@[036:038) NewLine |\r\n|
output bad int = true && !4
//@[000:006) Identifier |output|
//@[007:010) Identifier |bad|
//@[011:014) Identifier |int|
//@[015:016) Assignment |=|
//@[017:021) TrueKeyword |true|
//@[022:024) LogicalAnd |&&|
//@[025:026) Exclamation |!|
//@[026:027) Integer |4|
//@[027:029) NewLine |\r\n|
output deeper bool = true ? -true : (14 && 's') + 10
//@[000:006) Identifier |output|
//@[007:013) Identifier |deeper|
//@[014:018) Identifier |bool|
//@[019:020) Assignment |=|
//@[021:025) TrueKeyword |true|
//@[026:027) Question |?|
//@[028:029) Minus |-|
//@[029:033) TrueKeyword |true|
//@[034:035) Colon |:|
//@[036:037) LeftParen |(|
//@[037:039) Integer |14|
//@[040:042) LogicalAnd |&&|
//@[043:046) StringComplete |'s'|
//@[046:047) RightParen |)|
//@[048:049) Plus |+|
//@[050:052) Integer |10|
//@[052:056) NewLine |\r\n\r\n|

output myOutput string = 'hello'
//@[000:006) Identifier |output|
//@[007:015) Identifier |myOutput|
//@[016:022) Identifier |string|
//@[023:024) Assignment |=|
//@[025:032) StringComplete |'hello'|
//@[032:034) NewLine |\r\n|
var attemptToReferenceAnOutput = myOutput
//@[000:003) Identifier |var|
//@[004:030) Identifier |attemptToReferenceAnOutput|
//@[031:032) Assignment |=|
//@[033:041) Identifier |myOutput|
//@[041:045) NewLine |\r\n\r\n|

@sys.maxValue(20)
//@[000:001) At |@|
//@[001:004) Identifier |sys|
//@[004:005) Dot |.|
//@[005:013) Identifier |maxValue|
//@[013:014) LeftParen |(|
//@[014:016) Integer |20|
//@[016:017) RightParen |)|
//@[017:019) NewLine |\r\n|
@minValue(10)
//@[000:001) At |@|
//@[001:009) Identifier |minValue|
//@[009:010) LeftParen |(|
//@[010:012) Integer |10|
//@[012:013) RightParen |)|
//@[013:015) NewLine |\r\n|
output notAttachableDecorators int = 32
//@[000:006) Identifier |output|
//@[007:030) Identifier |notAttachableDecorators|
//@[031:034) Identifier |int|
//@[035:036) Assignment |=|
//@[037:039) Integer |32|
//@[039:043) NewLine |\r\n\r\n|

// nested loops inside output loops are not supported
//@[053:055) NewLine |\r\n|
output noNestedLoops array = [for thing in things: {
//@[000:006) Identifier |output|
//@[007:020) Identifier |noNestedLoops|
//@[021:026) Identifier |array|
//@[027:028) Assignment |=|
//@[029:030) LeftSquare |[|
//@[030:033) Identifier |for|
//@[034:039) Identifier |thing|
//@[040:042) Identifier |in|
//@[043:049) Identifier |things|
//@[049:050) Colon |:|
//@[051:052) LeftBrace |{|
//@[052:054) NewLine |\r\n|
  something: [
//@[002:011) Identifier |something|
//@[011:012) Colon |:|
//@[013:014) LeftSquare |[|
//@[014:016) NewLine |\r\n|
    [for thing in things: true]
//@[004:005) LeftSquare |[|
//@[005:008) Identifier |for|
//@[009:014) Identifier |thing|
//@[015:017) Identifier |in|
//@[018:024) Identifier |things|
//@[024:025) Colon |:|
//@[026:030) TrueKeyword |true|
//@[030:031) RightSquare |]|
//@[031:033) NewLine |\r\n|
  ]
//@[002:003) RightSquare |]|
//@[003:005) NewLine |\r\n|
}]
//@[000:001) RightBrace |}|
//@[001:002) RightSquare |]|
//@[002:006) NewLine |\r\n\r\n|

// loops in inner properties inside outputs are not supported
//@[061:063) NewLine |\r\n|
output noInnerLoopsInOutputs object = {
//@[000:006) Identifier |output|
//@[007:028) Identifier |noInnerLoopsInOutputs|
//@[029:035) Identifier |object|
//@[036:037) Assignment |=|
//@[038:039) LeftBrace |{|
//@[039:041) NewLine |\r\n|
  a: [for i in range(0,10): i]
//@[002:003) Identifier |a|
//@[003:004) Colon |:|
//@[005:006) LeftSquare |[|
//@[006:009) Identifier |for|
//@[010:011) Identifier |i|
//@[012:014) Identifier |in|
//@[015:020) Identifier |range|
//@[020:021) LeftParen |(|
//@[021:022) Integer |0|
//@[022:023) Comma |,|
//@[023:025) Integer |10|
//@[025:026) RightParen |)|
//@[026:027) Colon |:|
//@[028:029) Identifier |i|
//@[029:030) RightSquare |]|
//@[030:032) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\r\n|
output noInnerLoopsInOutputs2 object = {
//@[000:006) Identifier |output|
//@[007:029) Identifier |noInnerLoopsInOutputs2|
//@[030:036) Identifier |object|
//@[037:038) Assignment |=|
//@[039:040) LeftBrace |{|
//@[040:042) NewLine |\r\n|
  a: [for i in range(0,10): {
//@[002:003) Identifier |a|
//@[003:004) Colon |:|
//@[005:006) LeftSquare |[|
//@[006:009) Identifier |for|
//@[010:011) Identifier |i|
//@[012:014) Identifier |in|
//@[015:020) Identifier |range|
//@[020:021) LeftParen |(|
//@[021:022) Integer |0|
//@[022:023) Comma |,|
//@[023:025) Integer |10|
//@[025:026) RightParen |)|
//@[026:027) Colon |:|
//@[028:029) LeftBrace |{|
//@[029:031) NewLine |\r\n|
    b: [for j in range(0,10): i+j]
//@[004:005) Identifier |b|
//@[005:006) Colon |:|
//@[007:008) LeftSquare |[|
//@[008:011) Identifier |for|
//@[012:013) Identifier |j|
//@[014:016) Identifier |in|
//@[017:022) Identifier |range|
//@[022:023) LeftParen |(|
//@[023:024) Integer |0|
//@[024:025) Comma |,|
//@[025:027) Integer |10|
//@[027:028) RightParen |)|
//@[028:029) Colon |:|
//@[030:031) Identifier |i|
//@[031:032) Plus |+|
//@[032:033) Identifier |j|
//@[033:034) RightSquare |]|
//@[034:036) NewLine |\r\n|
  }]
//@[002:003) RightBrace |}|
//@[003:004) RightSquare |]|
//@[004:006) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

//KeyVault Secret Reference
//@[027:029) NewLine |\r\n|
resource kv 'Microsoft.KeyVault/vaults@2019-09-01' existing = {
//@[000:008) Identifier |resource|
//@[009:011) Identifier |kv|
//@[012:050) StringComplete |'Microsoft.KeyVault/vaults@2019-09-01'|
//@[051:059) Identifier |existing|
//@[060:061) Assignment |=|
//@[062:063) LeftBrace |{|
//@[063:065) NewLine |\r\n|
  name: 'testkeyvault'
//@[002:006) Identifier |name|
//@[006:007) Colon |:|
//@[008:022) StringComplete |'testkeyvault'|
//@[022:024) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:005) NewLine |\r\n\r\n|

output keyVaultSecretOutput string = kv.getSecret('mySecret')
//@[000:006) Identifier |output|
//@[007:027) Identifier |keyVaultSecretOutput|
//@[028:034) Identifier |string|
//@[035:036) Assignment |=|
//@[037:039) Identifier |kv|
//@[039:040) Dot |.|
//@[040:049) Identifier |getSecret|
//@[049:050) LeftParen |(|
//@[050:060) StringComplete |'mySecret'|
//@[060:061) RightParen |)|
//@[061:063) NewLine |\r\n|
output keyVaultSecretInterpolatedOutput string = '${kv.getSecret('mySecret')}'
//@[000:006) Identifier |output|
//@[007:039) Identifier |keyVaultSecretInterpolatedOutput|
//@[040:046) Identifier |string|
//@[047:048) Assignment |=|
//@[049:052) StringLeftPiece |'${|
//@[052:054) Identifier |kv|
//@[054:055) Dot |.|
//@[055:064) Identifier |getSecret|
//@[064:065) LeftParen |(|
//@[065:075) StringComplete |'mySecret'|
//@[075:076) RightParen |)|
//@[076:078) StringRightPiece |}'|
//@[078:080) NewLine |\r\n|
output keyVaultSecretObjectOutput object = {
//@[000:006) Identifier |output|
//@[007:033) Identifier |keyVaultSecretObjectOutput|
//@[034:040) Identifier |object|
//@[041:042) Assignment |=|
//@[043:044) LeftBrace |{|
//@[044:046) NewLine |\r\n|
  secret: kv.getSecret('mySecret')
//@[002:008) Identifier |secret|
//@[008:009) Colon |:|
//@[010:012) Identifier |kv|
//@[012:013) Dot |.|
//@[013:022) Identifier |getSecret|
//@[022:023) LeftParen |(|
//@[023:033) StringComplete |'mySecret'|
//@[033:034) RightParen |)|
//@[034:036) NewLine |\r\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\r\n|
output keyVaultSecretArrayOutput array = [
//@[000:006) Identifier |output|
//@[007:032) Identifier |keyVaultSecretArrayOutput|
//@[033:038) Identifier |array|
//@[039:040) Assignment |=|
//@[041:042) LeftSquare |[|
//@[042:044) NewLine |\r\n|
  kv.getSecret('mySecret')
//@[002:004) Identifier |kv|
//@[004:005) Dot |.|
//@[005:014) Identifier |getSecret|
//@[014:015) LeftParen |(|
//@[015:025) StringComplete |'mySecret'|
//@[025:026) RightParen |)|
//@[026:028) NewLine |\r\n|
]
//@[000:001) RightSquare |]|
//@[001:003) NewLine |\r\n|
output keyVaultSecretArrayInterpolatedOutput array = [
//@[000:006) Identifier |output|
//@[007:044) Identifier |keyVaultSecretArrayInterpolatedOutput|
//@[045:050) Identifier |array|
//@[051:052) Assignment |=|
//@[053:054) LeftSquare |[|
//@[054:056) NewLine |\r\n|
  '${kv.getSecret('mySecret')}'
//@[002:005) StringLeftPiece |'${|
//@[005:007) Identifier |kv|
//@[007:008) Dot |.|
//@[008:017) Identifier |getSecret|
//@[017:018) LeftParen |(|
//@[018:028) StringComplete |'mySecret'|
//@[028:029) RightParen |)|
//@[029:031) StringRightPiece |}'|
//@[031:033) NewLine |\r\n|
]
//@[000:001) RightSquare |]|
//@[001:005) NewLine |\r\n\r\n|

// WARNING!!!!! dangling decorators
//@[035:039) NewLine |\r\n\r\n|

// #completionTest(1) -> decoratorsPlusNamespace
//@[048:050) NewLine |\r\n|
@
//@[000:001) At |@|
//@[001:003) NewLine |\r\n|
// #completionTest(5) -> decorators
//@[035:037) NewLine |\r\n|
@sys.
//@[000:001) At |@|
//@[001:004) Identifier |sys|
//@[004:005) Dot |.|
//@[005:009) NewLine |\r\n\r\n|

// WARNING!!!!! dangling decorators - to make sure the tests work, please do not add contents after this line 
//@[110:110) EndOfFile ||
