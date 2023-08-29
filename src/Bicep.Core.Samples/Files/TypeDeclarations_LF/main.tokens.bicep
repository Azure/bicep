@description('The foo type')
//@[000:001) At |@|
//@[001:012) Identifier |description|
//@[012:013) LeftParen |(|
//@[013:027) StringComplete |'The foo type'|
//@[027:028) RightParen |)|
//@[028:029) NewLine |\n|
@sealed()
//@[000:001) At |@|
//@[001:007) Identifier |sealed|
//@[007:008) LeftParen |(|
//@[008:009) RightParen |)|
//@[009:010) NewLine |\n|
type foo = {
//@[000:004) Identifier |type|
//@[005:008) Identifier |foo|
//@[009:010) Assignment |=|
//@[011:012) LeftBrace |{|
//@[012:013) NewLine |\n|
  @minLength(3)
//@[002:003) At |@|
//@[003:012) Identifier |minLength|
//@[012:013) LeftParen |(|
//@[013:014) Integer |3|
//@[014:015) RightParen |)|
//@[015:016) NewLine |\n|
  @maxLength(10)
//@[002:003) At |@|
//@[003:012) Identifier |maxLength|
//@[012:013) LeftParen |(|
//@[013:015) Integer |10|
//@[015:016) RightParen |)|
//@[016:017) NewLine |\n|
  @description('A string property')
//@[002:003) At |@|
//@[003:014) Identifier |description|
//@[014:015) LeftParen |(|
//@[015:034) StringComplete |'A string property'|
//@[034:035) RightParen |)|
//@[035:036) NewLine |\n|
  stringProp: string
//@[002:012) Identifier |stringProp|
//@[012:013) Colon |:|
//@[014:020) Identifier |string|
//@[020:022) NewLine |\n\n|

  objectProp: {
//@[002:012) Identifier |objectProp|
//@[012:013) Colon |:|
//@[014:015) LeftBrace |{|
//@[015:016) NewLine |\n|
    @minValue(1)
//@[004:005) At |@|
//@[005:013) Identifier |minValue|
//@[013:014) LeftParen |(|
//@[014:015) Integer |1|
//@[015:016) RightParen |)|
//@[016:017) NewLine |\n|
    intProp: int
//@[004:011) Identifier |intProp|
//@[011:012) Colon |:|
//@[013:016) Identifier |int|
//@[016:018) NewLine |\n\n|

    intArrayArrayProp: int [] [] ?
//@[004:021) Identifier |intArrayArrayProp|
//@[021:022) Colon |:|
//@[023:026) Identifier |int|
//@[027:028) LeftSquare |[|
//@[028:029) RightSquare |]|
//@[030:031) LeftSquare |[|
//@[031:032) RightSquare |]|
//@[033:034) Question |?|
//@[034:035) NewLine |\n|
  }
//@[002:003) RightBrace |}|
//@[003:005) NewLine |\n\n|

  typeRefProp: bar
//@[002:013) Identifier |typeRefProp|
//@[013:014) Colon |:|
//@[015:018) Identifier |bar|
//@[018:020) NewLine |\n\n|

  literalProp: 'literal'
//@[002:013) Identifier |literalProp|
//@[013:014) Colon |:|
//@[015:024) StringComplete |'literal'|
//@[024:026) NewLine |\n\n|

  recursion: foo?
//@[002:011) Identifier |recursion|
//@[011:012) Colon |:|
//@[013:016) Identifier |foo|
//@[016:017) Question |?|
//@[017:018) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

@minLength(3)
//@[000:001) At |@|
//@[001:010) Identifier |minLength|
//@[010:011) LeftParen |(|
//@[011:012) Integer |3|
//@[012:013) RightParen |)|
//@[013:014) NewLine |\n|
@description('An array of array of arrays of arrays of ints')
//@[000:001) At |@|
//@[001:012) Identifier |description|
//@[012:013) LeftParen |(|
//@[013:060) StringComplete |'An array of array of arrays of arrays of ints'|
//@[060:061) RightParen |)|
//@[061:062) NewLine |\n|
@metadata({
//@[000:001) At |@|
//@[001:009) Identifier |metadata|
//@[009:010) LeftParen |(|
//@[010:011) LeftBrace |{|
//@[011:012) NewLine |\n|
  examples: [
//@[002:010) Identifier |examples|
//@[010:011) Colon |:|
//@[012:013) LeftSquare |[|
//@[013:014) NewLine |\n|
    [[[[1]]], [[[2]]], [[[3]]]]
//@[004:005) LeftSquare |[|
//@[005:006) LeftSquare |[|
//@[006:007) LeftSquare |[|
//@[007:008) LeftSquare |[|
//@[008:009) Integer |1|
//@[009:010) RightSquare |]|
//@[010:011) RightSquare |]|
//@[011:012) RightSquare |]|
//@[012:013) Comma |,|
//@[014:015) LeftSquare |[|
//@[015:016) LeftSquare |[|
//@[016:017) LeftSquare |[|
//@[017:018) Integer |2|
//@[018:019) RightSquare |]|
//@[019:020) RightSquare |]|
//@[020:021) RightSquare |]|
//@[021:022) Comma |,|
//@[023:024) LeftSquare |[|
//@[024:025) LeftSquare |[|
//@[025:026) LeftSquare |[|
//@[026:027) Integer |3|
//@[027:028) RightSquare |]|
//@[028:029) RightSquare |]|
//@[029:030) RightSquare |]|
//@[030:031) RightSquare |]|
//@[031:032) NewLine |\n|
  ]
//@[002:003) RightSquare |]|
//@[003:004) NewLine |\n|
})
//@[000:001) RightBrace |}|
//@[001:002) RightParen |)|
//@[002:003) NewLine |\n|
type bar = int[][][][]
//@[000:004) Identifier |type|
//@[005:008) Identifier |bar|
//@[009:010) Assignment |=|
//@[011:014) Identifier |int|
//@[014:015) LeftSquare |[|
//@[015:016) RightSquare |]|
//@[016:017) LeftSquare |[|
//@[017:018) RightSquare |]|
//@[018:019) LeftSquare |[|
//@[019:020) RightSquare |]|
//@[020:021) LeftSquare |[|
//@[021:022) RightSquare |]|
//@[022:024) NewLine |\n\n|

type aUnion = 'snap'|'crackle'|'pop'
//@[000:004) Identifier |type|
//@[005:011) Identifier |aUnion|
//@[012:013) Assignment |=|
//@[014:020) StringComplete |'snap'|
//@[020:021) Pipe |||
//@[021:030) StringComplete |'crackle'|
//@[030:031) Pipe |||
//@[031:036) StringComplete |'pop'|
//@[036:038) NewLine |\n\n|

type expandedUnion = aUnion|'fizz'|'buzz'|'pop'
//@[000:004) Identifier |type|
//@[005:018) Identifier |expandedUnion|
//@[019:020) Assignment |=|
//@[021:027) Identifier |aUnion|
//@[027:028) Pipe |||
//@[028:034) StringComplete |'fizz'|
//@[034:035) Pipe |||
//@[035:041) StringComplete |'buzz'|
//@[041:042) Pipe |||
//@[042:047) StringComplete |'pop'|
//@[047:049) NewLine |\n\n|

type tupleUnion = ['foo', 'bar', 'baz']
//@[000:004) Identifier |type|
//@[005:015) Identifier |tupleUnion|
//@[016:017) Assignment |=|
//@[018:019) LeftSquare |[|
//@[019:024) StringComplete |'foo'|
//@[024:025) Comma |,|
//@[026:031) StringComplete |'bar'|
//@[031:032) Comma |,|
//@[033:038) StringComplete |'baz'|
//@[038:039) RightSquare |]|
//@[039:040) NewLine |\n|
|['fizz', 'buzz']
//@[000:001) Pipe |||
//@[001:002) LeftSquare |[|
//@[002:008) StringComplete |'fizz'|
//@[008:009) Comma |,|
//@[010:016) StringComplete |'buzz'|
//@[016:017) RightSquare |]|
//@[017:018) NewLine |\n|
|['snap', 'crackle', 'pop']
//@[000:001) Pipe |||
//@[001:002) LeftSquare |[|
//@[002:008) StringComplete |'snap'|
//@[008:009) Comma |,|
//@[010:019) StringComplete |'crackle'|
//@[019:020) Comma |,|
//@[021:026) StringComplete |'pop'|
//@[026:027) RightSquare |]|
//@[027:029) NewLine |\n\n|

type mixedArray = ('heffalump'|'woozle'|{ shape: '*', size: '*'}|10|-10|true|!true|null)[]
//@[000:004) Identifier |type|
//@[005:015) Identifier |mixedArray|
//@[016:017) Assignment |=|
//@[018:019) LeftParen |(|
//@[019:030) StringComplete |'heffalump'|
//@[030:031) Pipe |||
//@[031:039) StringComplete |'woozle'|
//@[039:040) Pipe |||
//@[040:041) LeftBrace |{|
//@[042:047) Identifier |shape|
//@[047:048) Colon |:|
//@[049:052) StringComplete |'*'|
//@[052:053) Comma |,|
//@[054:058) Identifier |size|
//@[058:059) Colon |:|
//@[060:063) StringComplete |'*'|
//@[063:064) RightBrace |}|
//@[064:065) Pipe |||
//@[065:067) Integer |10|
//@[067:068) Pipe |||
//@[068:069) Minus |-|
//@[069:071) Integer |10|
//@[071:072) Pipe |||
//@[072:076) TrueKeyword |true|
//@[076:077) Pipe |||
//@[077:078) Exclamation |!|
//@[078:082) TrueKeyword |true|
//@[082:083) Pipe |||
//@[083:087) NullKeyword |null|
//@[087:088) RightParen |)|
//@[088:089) LeftSquare |[|
//@[089:090) RightSquare |]|
//@[090:092) NewLine |\n\n|

type bool = string
//@[000:004) Identifier |type|
//@[005:009) Identifier |bool|
//@[010:011) Assignment |=|
//@[012:018) Identifier |string|
//@[018:020) NewLine |\n\n|

param inlineObjectParam {
//@[000:005) Identifier |param|
//@[006:023) Identifier |inlineObjectParam|
//@[024:025) LeftBrace |{|
//@[025:026) NewLine |\n|
  foo: string
//@[002:005) Identifier |foo|
//@[005:006) Colon |:|
//@[007:013) Identifier |string|
//@[013:014) NewLine |\n|
  bar: 100|200|300|400|500
//@[002:005) Identifier |bar|
//@[005:006) Colon |:|
//@[007:010) Integer |100|
//@[010:011) Pipe |||
//@[011:014) Integer |200|
//@[014:015) Pipe |||
//@[015:018) Integer |300|
//@[018:019) Pipe |||
//@[019:022) Integer |400|
//@[022:023) Pipe |||
//@[023:026) Integer |500|
//@[026:027) NewLine |\n|
  baz: sys.bool
//@[002:005) Identifier |baz|
//@[005:006) Colon |:|
//@[007:010) Identifier |sys|
//@[010:011) Dot |.|
//@[011:015) Identifier |bool|
//@[015:016) NewLine |\n|
} = {
//@[000:001) RightBrace |}|
//@[002:003) Assignment |=|
//@[004:005) LeftBrace |{|
//@[005:006) NewLine |\n|
  foo: 'foo'
//@[002:005) Identifier |foo|
//@[005:006) Colon |:|
//@[007:012) StringComplete |'foo'|
//@[012:013) NewLine |\n|
  bar: 300
//@[002:005) Identifier |bar|
//@[005:006) Colon |:|
//@[007:010) Integer |300|
//@[010:011) NewLine |\n|
  baz: false
//@[002:005) Identifier |baz|
//@[005:006) Colon |:|
//@[007:012) FalseKeyword |false|
//@[012:013) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

param unionParam {property: 'ping'}|{property: 'pong'} = {property: 'pong'}
//@[000:005) Identifier |param|
//@[006:016) Identifier |unionParam|
//@[017:018) LeftBrace |{|
//@[018:026) Identifier |property|
//@[026:027) Colon |:|
//@[028:034) StringComplete |'ping'|
//@[034:035) RightBrace |}|
//@[035:036) Pipe |||
//@[036:037) LeftBrace |{|
//@[037:045) Identifier |property|
//@[045:046) Colon |:|
//@[047:053) StringComplete |'pong'|
//@[053:054) RightBrace |}|
//@[055:056) Assignment |=|
//@[057:058) LeftBrace |{|
//@[058:066) Identifier |property|
//@[066:067) Colon |:|
//@[068:074) StringComplete |'pong'|
//@[074:075) RightBrace |}|
//@[075:077) NewLine |\n\n|

param paramUsingType mixedArray
//@[000:005) Identifier |param|
//@[006:020) Identifier |paramUsingType|
//@[021:031) Identifier |mixedArray|
//@[031:033) NewLine |\n\n|

output outputUsingType mixedArray = paramUsingType
//@[000:006) Identifier |output|
//@[007:022) Identifier |outputUsingType|
//@[023:033) Identifier |mixedArray|
//@[034:035) Assignment |=|
//@[036:050) Identifier |paramUsingType|
//@[050:052) NewLine |\n\n|

type tuple = [
//@[000:004) Identifier |type|
//@[005:010) Identifier |tuple|
//@[011:012) Assignment |=|
//@[013:014) LeftSquare |[|
//@[014:015) NewLine |\n|
    @description('A leading string')
//@[004:005) At |@|
//@[005:016) Identifier |description|
//@[016:017) LeftParen |(|
//@[017:035) StringComplete |'A leading string'|
//@[035:036) RightParen |)|
//@[036:037) NewLine |\n|
    string
//@[004:010) Identifier |string|
//@[010:012) NewLine |\n\n|

    @description('A second element using a type alias')
//@[004:005) At |@|
//@[005:016) Identifier |description|
//@[016:017) LeftParen |(|
//@[017:054) StringComplete |'A second element using a type alias'|
//@[054:055) RightParen |)|
//@[055:056) NewLine |\n|
    bar
//@[004:007) Identifier |bar|
//@[007:008) NewLine |\n|
]
//@[000:001) RightSquare |]|
//@[001:003) NewLine |\n\n|

type stringStringDictionary = {
//@[000:004) Identifier |type|
//@[005:027) Identifier |stringStringDictionary|
//@[028:029) Assignment |=|
//@[030:031) LeftBrace |{|
//@[031:032) NewLine |\n|
    *: string
//@[004:005) Asterisk |*|
//@[005:006) Colon |:|
//@[007:013) Identifier |string|
//@[013:014) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

@minValue(1)
//@[000:001) At |@|
//@[001:009) Identifier |minValue|
//@[009:010) LeftParen |(|
//@[010:011) Integer |1|
//@[011:012) RightParen |)|
//@[012:013) NewLine |\n|
@maxValue(10)
//@[000:001) At |@|
//@[001:009) Identifier |maxValue|
//@[009:010) LeftParen |(|
//@[010:012) Integer |10|
//@[012:013) RightParen |)|
//@[013:014) NewLine |\n|
type constrainedInt = int
//@[000:004) Identifier |type|
//@[005:019) Identifier |constrainedInt|
//@[020:021) Assignment |=|
//@[022:025) Identifier |int|
//@[025:027) NewLine |\n\n|

param mightIncludeNull ({key: 'value'} | null)[]
//@[000:005) Identifier |param|
//@[006:022) Identifier |mightIncludeNull|
//@[023:024) LeftParen |(|
//@[024:025) LeftBrace |{|
//@[025:028) Identifier |key|
//@[028:029) Colon |:|
//@[030:037) StringComplete |'value'|
//@[037:038) RightBrace |}|
//@[039:040) Pipe |||
//@[041:045) NullKeyword |null|
//@[045:046) RightParen |)|
//@[046:047) LeftSquare |[|
//@[047:048) RightSquare |]|
//@[048:050) NewLine |\n\n|

var nonNull = mightIncludeNull[0]!.key
//@[000:003) Identifier |var|
//@[004:011) Identifier |nonNull|
//@[012:013) Assignment |=|
//@[014:030) Identifier |mightIncludeNull|
//@[030:031) LeftSquare |[|
//@[031:032) Integer |0|
//@[032:033) RightSquare |]|
//@[033:034) Exclamation |!|
//@[034:035) Dot |.|
//@[035:038) Identifier |key|
//@[038:040) NewLine |\n\n|

output nonNull string = nonNull
//@[000:006) Identifier |output|
//@[007:014) Identifier |nonNull|
//@[015:021) Identifier |string|
//@[022:023) Assignment |=|
//@[024:031) Identifier |nonNull|
//@[031:033) NewLine |\n\n|

var maybeNull = mightIncludeNull[0].?key
//@[000:003) Identifier |var|
//@[004:013) Identifier |maybeNull|
//@[014:015) Assignment |=|
//@[016:032) Identifier |mightIncludeNull|
//@[032:033) LeftSquare |[|
//@[033:034) Integer |0|
//@[034:035) RightSquare |]|
//@[035:036) Dot |.|
//@[036:037) Question |?|
//@[037:040) Identifier |key|
//@[040:042) NewLine |\n\n|

var maybeNull2 = mightIncludeNull[0][?'key']
//@[000:003) Identifier |var|
//@[004:014) Identifier |maybeNull2|
//@[015:016) Assignment |=|
//@[017:033) Identifier |mightIncludeNull|
//@[033:034) LeftSquare |[|
//@[034:035) Integer |0|
//@[035:036) RightSquare |]|
//@[036:037) LeftSquare |[|
//@[037:038) Question |?|
//@[038:043) StringComplete |'key'|
//@[043:044) RightSquare |]|
//@[044:046) NewLine |\n\n|

output maybeNull string? = maybeNull
//@[000:006) Identifier |output|
//@[007:016) Identifier |maybeNull|
//@[017:023) Identifier |string|
//@[023:024) Question |?|
//@[025:026) Assignment |=|
//@[027:036) Identifier |maybeNull|
//@[036:038) NewLine |\n\n|

type nullable = string?
//@[000:004) Identifier |type|
//@[005:013) Identifier |nullable|
//@[014:015) Assignment |=|
//@[016:022) Identifier |string|
//@[022:023) Question |?|
//@[023:025) NewLine |\n\n|

type nonNullable = nullable!
//@[000:004) Identifier |type|
//@[005:016) Identifier |nonNullable|
//@[017:018) Assignment |=|
//@[019:027) Identifier |nullable|
//@[027:028) Exclamation |!|
//@[028:030) NewLine |\n\n|

type typeA = {
//@[000:004) Identifier |type|
//@[005:010) Identifier |typeA|
//@[011:012) Assignment |=|
//@[013:014) LeftBrace |{|
//@[014:015) NewLine |\n|
  type: 'a'
//@[002:006) Identifier |type|
//@[006:007) Colon |:|
//@[008:011) StringComplete |'a'|
//@[011:012) NewLine |\n|
  value: string
//@[002:007) Identifier |value|
//@[007:008) Colon |:|
//@[009:015) Identifier |string|
//@[015:016) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

type typeB = {
//@[000:004) Identifier |type|
//@[005:010) Identifier |typeB|
//@[011:012) Assignment |=|
//@[013:014) LeftBrace |{|
//@[014:015) NewLine |\n|
  type: 'b'
//@[002:006) Identifier |type|
//@[006:007) Colon |:|
//@[008:011) StringComplete |'b'|
//@[011:012) NewLine |\n|
  value: int
//@[002:007) Identifier |value|
//@[007:008) Colon |:|
//@[009:012) Identifier |int|
//@[012:013) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

type typeC = {
//@[000:004) Identifier |type|
//@[005:010) Identifier |typeC|
//@[011:012) Assignment |=|
//@[013:014) LeftBrace |{|
//@[014:015) NewLine |\n|
  type: 'c'
//@[002:006) Identifier |type|
//@[006:007) Colon |:|
//@[008:011) StringComplete |'c'|
//@[011:012) NewLine |\n|
  value: bool
//@[002:007) Identifier |value|
//@[007:008) Colon |:|
//@[009:013) Identifier |bool|
//@[013:014) NewLine |\n|
  value2: string
//@[002:008) Identifier |value2|
//@[008:009) Colon |:|
//@[010:016) Identifier |string|
//@[016:017) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

type typeD = {
//@[000:004) Identifier |type|
//@[005:010) Identifier |typeD|
//@[011:012) Assignment |=|
//@[013:014) LeftBrace |{|
//@[014:015) NewLine |\n|
  type: 'd'
//@[002:006) Identifier |type|
//@[006:007) Colon |:|
//@[008:011) StringComplete |'d'|
//@[011:012) NewLine |\n|
  value: object
//@[002:007) Identifier |value|
//@[007:008) Colon |:|
//@[009:015) Identifier |object|
//@[015:016) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

type typeE = {
//@[000:004) Identifier |type|
//@[005:010) Identifier |typeE|
//@[011:012) Assignment |=|
//@[013:014) LeftBrace |{|
//@[014:015) NewLine |\n|
  type: 'e'
//@[002:006) Identifier |type|
//@[006:007) Colon |:|
//@[008:011) StringComplete |'e'|
//@[011:012) NewLine |\n|
  value: 'a' | 'b'
//@[002:007) Identifier |value|
//@[007:008) Colon |:|
//@[009:012) StringComplete |'a'|
//@[013:014) Pipe |||
//@[015:018) StringComplete |'b'|
//@[018:019) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

type typeF = {
//@[000:004) Identifier |type|
//@[005:010) Identifier |typeF|
//@[011:012) Assignment |=|
//@[013:014) LeftBrace |{|
//@[014:015) NewLine |\n|
  type: 'f'
//@[002:006) Identifier |type|
//@[006:007) Colon |:|
//@[008:011) StringComplete |'f'|
//@[011:012) NewLine |\n|
  *: string
//@[002:003) Asterisk |*|
//@[003:004) Colon |:|
//@[005:011) Identifier |string|
//@[011:012) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

@discriminator('type')
//@[000:001) At |@|
//@[001:014) Identifier |discriminator|
//@[014:015) LeftParen |(|
//@[015:021) StringComplete |'type'|
//@[021:022) RightParen |)|
//@[022:023) NewLine |\n|
type discriminatedUnion1 = typeA | typeB
//@[000:004) Identifier |type|
//@[005:024) Identifier |discriminatedUnion1|
//@[025:026) Assignment |=|
//@[027:032) Identifier |typeA|
//@[033:034) Pipe |||
//@[035:040) Identifier |typeB|
//@[040:042) NewLine |\n\n|

@discriminator('type')
//@[000:001) At |@|
//@[001:014) Identifier |discriminator|
//@[014:015) LeftParen |(|
//@[015:021) StringComplete |'type'|
//@[021:022) RightParen |)|
//@[022:023) NewLine |\n|
type discriminatedUnion2 = { type: 'c', value: string } | { type: 'd', value: bool }
//@[000:004) Identifier |type|
//@[005:024) Identifier |discriminatedUnion2|
//@[025:026) Assignment |=|
//@[027:028) LeftBrace |{|
//@[029:033) Identifier |type|
//@[033:034) Colon |:|
//@[035:038) StringComplete |'c'|
//@[038:039) Comma |,|
//@[040:045) Identifier |value|
//@[045:046) Colon |:|
//@[047:053) Identifier |string|
//@[054:055) RightBrace |}|
//@[056:057) Pipe |||
//@[058:059) LeftBrace |{|
//@[060:064) Identifier |type|
//@[064:065) Colon |:|
//@[066:069) StringComplete |'d'|
//@[069:070) Comma |,|
//@[071:076) Identifier |value|
//@[076:077) Colon |:|
//@[078:082) Identifier |bool|
//@[083:084) RightBrace |}|
//@[084:086) NewLine |\n\n|

@discriminator('type')
//@[000:001) At |@|
//@[001:014) Identifier |discriminator|
//@[014:015) LeftParen |(|
//@[015:021) StringComplete |'type'|
//@[021:022) RightParen |)|
//@[022:023) NewLine |\n|
type discriminatedUnion3 = discriminatedUnion1 | discriminatedUnion2 | { type: 'e', value: string }
//@[000:004) Identifier |type|
//@[005:024) Identifier |discriminatedUnion3|
//@[025:026) Assignment |=|
//@[027:046) Identifier |discriminatedUnion1|
//@[047:048) Pipe |||
//@[049:068) Identifier |discriminatedUnion2|
//@[069:070) Pipe |||
//@[071:072) LeftBrace |{|
//@[073:077) Identifier |type|
//@[077:078) Colon |:|
//@[079:082) StringComplete |'e'|
//@[082:083) Comma |,|
//@[084:089) Identifier |value|
//@[089:090) Colon |:|
//@[091:097) Identifier |string|
//@[098:099) RightBrace |}|
//@[099:101) NewLine |\n\n|

@discriminator('type')
//@[000:001) At |@|
//@[001:014) Identifier |discriminator|
//@[014:015) LeftParen |(|
//@[015:021) StringComplete |'type'|
//@[021:022) RightParen |)|
//@[022:023) NewLine |\n|
type discriminatedUnion4 = discriminatedUnion1 | (discriminatedUnion2 | typeE)
//@[000:004) Identifier |type|
//@[005:024) Identifier |discriminatedUnion4|
//@[025:026) Assignment |=|
//@[027:046) Identifier |discriminatedUnion1|
//@[047:048) Pipe |||
//@[049:050) LeftParen |(|
//@[050:069) Identifier |discriminatedUnion2|
//@[070:071) Pipe |||
//@[072:077) Identifier |typeE|
//@[077:078) RightParen |)|
//@[078:080) NewLine |\n\n|

@discriminator('type')
//@[000:001) At |@|
//@[001:014) Identifier |discriminator|
//@[014:015) LeftParen |(|
//@[015:021) StringComplete |'type'|
//@[021:022) RightParen |)|
//@[022:023) NewLine |\n|
type discriminatedUnion5 = (typeA | typeB)?
//@[000:004) Identifier |type|
//@[005:024) Identifier |discriminatedUnion5|
//@[025:026) Assignment |=|
//@[027:028) LeftParen |(|
//@[028:033) Identifier |typeA|
//@[034:035) Pipe |||
//@[036:041) Identifier |typeB|
//@[041:042) RightParen |)|
//@[042:043) Question |?|
//@[043:045) NewLine |\n\n|

@discriminator('type')
//@[000:001) At |@|
//@[001:014) Identifier |discriminator|
//@[014:015) LeftParen |(|
//@[015:021) StringComplete |'type'|
//@[021:022) RightParen |)|
//@[022:023) NewLine |\n|
type discriminatedUnion6 = (typeA | typeB)!
//@[000:004) Identifier |type|
//@[005:024) Identifier |discriminatedUnion6|
//@[025:026) Assignment |=|
//@[027:028) LeftParen |(|
//@[028:033) Identifier |typeA|
//@[034:035) Pipe |||
//@[036:041) Identifier |typeB|
//@[041:042) RightParen |)|
//@[042:043) Exclamation |!|
//@[043:045) NewLine |\n\n|

type inlineDiscriminatedUnion1 = {
//@[000:004) Identifier |type|
//@[005:030) Identifier |inlineDiscriminatedUnion1|
//@[031:032) Assignment |=|
//@[033:034) LeftBrace |{|
//@[034:035) NewLine |\n|
  @discriminator('type')
//@[002:003) At |@|
//@[003:016) Identifier |discriminator|
//@[016:017) LeftParen |(|
//@[017:023) StringComplete |'type'|
//@[023:024) RightParen |)|
//@[024:025) NewLine |\n|
  prop: typeA | typeC
//@[002:006) Identifier |prop|
//@[006:007) Colon |:|
//@[008:013) Identifier |typeA|
//@[014:015) Pipe |||
//@[016:021) Identifier |typeC|
//@[021:022) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

type inlineDiscriminatedUnion2 = {
//@[000:004) Identifier |type|
//@[005:030) Identifier |inlineDiscriminatedUnion2|
//@[031:032) Assignment |=|
//@[033:034) LeftBrace |{|
//@[034:035) NewLine |\n|
  @discriminator('type')
//@[002:003) At |@|
//@[003:016) Identifier |discriminator|
//@[016:017) LeftParen |(|
//@[017:023) StringComplete |'type'|
//@[023:024) RightParen |)|
//@[024:025) NewLine |\n|
  prop: { type: 'a', value: bool } | typeB
//@[002:006) Identifier |prop|
//@[006:007) Colon |:|
//@[008:009) LeftBrace |{|
//@[010:014) Identifier |type|
//@[014:015) Colon |:|
//@[016:019) StringComplete |'a'|
//@[019:020) Comma |,|
//@[021:026) Identifier |value|
//@[026:027) Colon |:|
//@[028:032) Identifier |bool|
//@[033:034) RightBrace |}|
//@[035:036) Pipe |||
//@[037:042) Identifier |typeB|
//@[042:043) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

@discriminator('type')
//@[000:001) At |@|
//@[001:014) Identifier |discriminator|
//@[014:015) LeftParen |(|
//@[015:021) StringComplete |'type'|
//@[021:022) RightParen |)|
//@[022:023) NewLine |\n|
type inlineDiscriminatedUnion3 = {
//@[000:004) Identifier |type|
//@[005:030) Identifier |inlineDiscriminatedUnion3|
//@[031:032) Assignment |=|
//@[033:034) LeftBrace |{|
//@[034:035) NewLine |\n|
  type: 'a'
//@[002:006) Identifier |type|
//@[006:007) Colon |:|
//@[008:011) StringComplete |'a'|
//@[011:012) NewLine |\n|
  @discriminator('type')
//@[002:003) At |@|
//@[003:016) Identifier |discriminator|
//@[016:017) LeftParen |(|
//@[017:023) StringComplete |'type'|
//@[023:024) RightParen |)|
//@[024:025) NewLine |\n|
  prop: { type: 'a', value: bool } | typeB
//@[002:006) Identifier |prop|
//@[006:007) Colon |:|
//@[008:009) LeftBrace |{|
//@[010:014) Identifier |type|
//@[014:015) Colon |:|
//@[016:019) StringComplete |'a'|
//@[019:020) Comma |,|
//@[021:026) Identifier |value|
//@[026:027) Colon |:|
//@[028:032) Identifier |bool|
//@[033:034) RightBrace |}|
//@[035:036) Pipe |||
//@[037:042) Identifier |typeB|
//@[042:043) NewLine |\n|
} | {
//@[000:001) RightBrace |}|
//@[002:003) Pipe |||
//@[004:005) LeftBrace |{|
//@[005:006) NewLine |\n|
  type: 'b'
//@[002:006) Identifier |type|
//@[006:007) Colon |:|
//@[008:011) StringComplete |'b'|
//@[011:012) NewLine |\n|
  @discriminator('type')
//@[002:003) At |@|
//@[003:016) Identifier |discriminator|
//@[016:017) LeftParen |(|
//@[017:023) StringComplete |'type'|
//@[023:024) RightParen |)|
//@[024:025) NewLine |\n|
  prop: discriminatedUnion1 | discriminatedUnion2
//@[002:006) Identifier |prop|
//@[006:007) Colon |:|
//@[008:027) Identifier |discriminatedUnion1|
//@[028:029) Pipe |||
//@[030:049) Identifier |discriminatedUnion2|
//@[049:050) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

type inlineDiscriminatedUnion4 = {
//@[000:004) Identifier |type|
//@[005:030) Identifier |inlineDiscriminatedUnion4|
//@[031:032) Assignment |=|
//@[033:034) LeftBrace |{|
//@[034:035) NewLine |\n|
  @discriminator('type')
//@[002:003) At |@|
//@[003:016) Identifier |discriminator|
//@[016:017) LeftParen |(|
//@[017:023) StringComplete |'type'|
//@[023:024) RightParen |)|
//@[024:025) NewLine |\n|
  prop: (typeA | typeC)?
//@[002:006) Identifier |prop|
//@[006:007) Colon |:|
//@[008:009) LeftParen |(|
//@[009:014) Identifier |typeA|
//@[015:016) Pipe |||
//@[017:022) Identifier |typeC|
//@[022:023) RightParen |)|
//@[023:024) Question |?|
//@[024:025) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

type discriminatorUnionAsPropertyType = {
//@[000:004) Identifier |type|
//@[005:037) Identifier |discriminatorUnionAsPropertyType|
//@[038:039) Assignment |=|
//@[040:041) LeftBrace |{|
//@[041:042) NewLine |\n|
  prop1: discriminatedUnion1
//@[002:007) Identifier |prop1|
//@[007:008) Colon |:|
//@[009:028) Identifier |discriminatedUnion1|
//@[028:029) NewLine |\n|
  prop2: discriminatedUnion3
//@[002:007) Identifier |prop2|
//@[007:008) Colon |:|
//@[009:028) Identifier |discriminatedUnion3|
//@[028:029) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

type discriminatedUnionInlineAdditionalProps1 = {
//@[000:004) Identifier |type|
//@[005:045) Identifier |discriminatedUnionInlineAdditionalProps1|
//@[046:047) Assignment |=|
//@[048:049) LeftBrace |{|
//@[049:050) NewLine |\n|
  @discriminator('type')
//@[002:003) At |@|
//@[003:016) Identifier |discriminator|
//@[016:017) LeftParen |(|
//@[017:023) StringComplete |'type'|
//@[023:024) RightParen |)|
//@[024:025) NewLine |\n|
  *: typeA | typeB
//@[002:003) Asterisk |*|
//@[003:004) Colon |:|
//@[005:010) Identifier |typeA|
//@[011:012) Pipe |||
//@[013:018) Identifier |typeB|
//@[018:019) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

type discriminatedUnionInlineAdditionalProps2 = {
//@[000:004) Identifier |type|
//@[005:045) Identifier |discriminatedUnionInlineAdditionalProps2|
//@[046:047) Assignment |=|
//@[048:049) LeftBrace |{|
//@[049:050) NewLine |\n|
  @discriminator('type')
//@[002:003) At |@|
//@[003:016) Identifier |discriminator|
//@[016:017) LeftParen |(|
//@[017:023) StringComplete |'type'|
//@[023:024) RightParen |)|
//@[024:025) NewLine |\n|
  *: (typeA | typeB)?
//@[002:003) Asterisk |*|
//@[003:004) Colon |:|
//@[005:006) LeftParen |(|
//@[006:011) Identifier |typeA|
//@[012:013) Pipe |||
//@[014:019) Identifier |typeB|
//@[019:020) RightParen |)|
//@[020:021) Question |?|
//@[021:022) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

@discriminator('type')
//@[000:001) At |@|
//@[001:014) Identifier |discriminator|
//@[014:015) LeftParen |(|
//@[015:021) StringComplete |'type'|
//@[021:022) RightParen |)|
//@[022:023) NewLine |\n|
type discriminatorMemberHasAdditionalProperties1 = typeA | typeF | { type: 'g', *: int }
//@[000:004) Identifier |type|
//@[005:048) Identifier |discriminatorMemberHasAdditionalProperties1|
//@[049:050) Assignment |=|
//@[051:056) Identifier |typeA|
//@[057:058) Pipe |||
//@[059:064) Identifier |typeF|
//@[065:066) Pipe |||
//@[067:068) LeftBrace |{|
//@[069:073) Identifier |type|
//@[073:074) Colon |:|
//@[075:078) StringComplete |'g'|
//@[078:079) Comma |,|
//@[080:081) Asterisk |*|
//@[081:082) Colon |:|
//@[083:086) Identifier |int|
//@[087:088) RightBrace |}|
//@[088:090) NewLine |\n\n|

@discriminator('type')
//@[000:001) At |@|
//@[001:014) Identifier |discriminator|
//@[014:015) LeftParen |(|
//@[015:021) StringComplete |'type'|
//@[021:022) RightParen |)|
//@[022:023) NewLine |\n|
type discriminatorInnerSelfOptionalCycle1 = typeA | {
//@[000:004) Identifier |type|
//@[005:041) Identifier |discriminatorInnerSelfOptionalCycle1|
//@[042:043) Assignment |=|
//@[044:049) Identifier |typeA|
//@[050:051) Pipe |||
//@[052:053) LeftBrace |{|
//@[053:054) NewLine |\n|
  type: 'b'
//@[002:006) Identifier |type|
//@[006:007) Colon |:|
//@[008:011) StringComplete |'b'|
//@[011:012) NewLine |\n|
  value: discriminatorInnerSelfOptionalCycle1?
//@[002:007) Identifier |value|
//@[007:008) Colon |:|
//@[009:045) Identifier |discriminatorInnerSelfOptionalCycle1|
//@[045:046) Question |?|
//@[046:047) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

type discriminatedUnionMemberOptionalCycle1 = {
//@[000:004) Identifier |type|
//@[005:043) Identifier |discriminatedUnionMemberOptionalCycle1|
//@[044:045) Assignment |=|
//@[046:047) LeftBrace |{|
//@[047:048) NewLine |\n|
  type: 'b'
//@[002:006) Identifier |type|
//@[006:007) Colon |:|
//@[008:011) StringComplete |'b'|
//@[011:012) NewLine |\n|
  @discriminator('type')
//@[002:003) At |@|
//@[003:016) Identifier |discriminator|
//@[016:017) LeftParen |(|
//@[017:023) StringComplete |'type'|
//@[023:024) RightParen |)|
//@[024:025) NewLine |\n|
  prop: (typeA | discriminatedUnionMemberOptionalCycle1)?
//@[002:006) Identifier |prop|
//@[006:007) Colon |:|
//@[008:009) LeftParen |(|
//@[009:014) Identifier |typeA|
//@[015:016) Pipe |||
//@[017:055) Identifier |discriminatedUnionMemberOptionalCycle1|
//@[055:056) RightParen |)|
//@[056:057) Question |?|
//@[057:058) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

type discriminatedUnionTuple1 = [
//@[000:004) Identifier |type|
//@[005:029) Identifier |discriminatedUnionTuple1|
//@[030:031) Assignment |=|
//@[032:033) LeftSquare |[|
//@[033:034) NewLine |\n|
  discriminatedUnion1
//@[002:021) Identifier |discriminatedUnion1|
//@[021:022) NewLine |\n|
  string
//@[002:008) Identifier |string|
//@[008:009) NewLine |\n|
]
//@[000:001) RightSquare |]|
//@[001:003) NewLine |\n\n|

type discriminatedUnionInlineTuple1 = [
//@[000:004) Identifier |type|
//@[005:035) Identifier |discriminatedUnionInlineTuple1|
//@[036:037) Assignment |=|
//@[038:039) LeftSquare |[|
//@[039:040) NewLine |\n|
  @discriminator('type')
//@[002:003) At |@|
//@[003:016) Identifier |discriminator|
//@[016:017) LeftParen |(|
//@[017:023) StringComplete |'type'|
//@[023:024) RightParen |)|
//@[024:025) NewLine |\n|
  typeA | typeB | { type: 'c', value: object }
//@[002:007) Identifier |typeA|
//@[008:009) Pipe |||
//@[010:015) Identifier |typeB|
//@[016:017) Pipe |||
//@[018:019) LeftBrace |{|
//@[020:024) Identifier |type|
//@[024:025) Colon |:|
//@[026:029) StringComplete |'c'|
//@[029:030) Comma |,|
//@[031:036) Identifier |value|
//@[036:037) Colon |:|
//@[038:044) Identifier |object|
//@[045:046) RightBrace |}|
//@[046:047) NewLine |\n|
  string
//@[002:008) Identifier |string|
//@[008:009) NewLine |\n|
]
//@[000:001) RightSquare |]|
//@[001:003) NewLine |\n\n|

param paramDiscriminatedUnionTypeAlias1 discriminatedUnion1
//@[000:005) Identifier |param|
//@[006:039) Identifier |paramDiscriminatedUnionTypeAlias1|
//@[040:059) Identifier |discriminatedUnion1|
//@[059:060) NewLine |\n|
param paramDiscriminatedUnionTypeAlias2 discriminatedUnion5
//@[000:005) Identifier |param|
//@[006:039) Identifier |paramDiscriminatedUnionTypeAlias2|
//@[040:059) Identifier |discriminatedUnion5|
//@[059:061) NewLine |\n\n|

@discriminator('type')
//@[000:001) At |@|
//@[001:014) Identifier |discriminator|
//@[014:015) LeftParen |(|
//@[015:021) StringComplete |'type'|
//@[021:022) RightParen |)|
//@[022:023) NewLine |\n|
param paramInlineDiscriminatedUnion1 typeA | typeB
//@[000:005) Identifier |param|
//@[006:036) Identifier |paramInlineDiscriminatedUnion1|
//@[037:042) Identifier |typeA|
//@[043:044) Pipe |||
//@[045:050) Identifier |typeB|
//@[050:052) NewLine |\n\n|

@discriminator('type')
//@[000:001) At |@|
//@[001:014) Identifier |discriminator|
//@[014:015) LeftParen |(|
//@[015:021) StringComplete |'type'|
//@[021:022) RightParen |)|
//@[022:023) NewLine |\n|
param paramInlineDiscriminatedUnion2 (typeA | typeB) = { type: 'b', value: 0 }
//@[000:005) Identifier |param|
//@[006:036) Identifier |paramInlineDiscriminatedUnion2|
//@[037:038) LeftParen |(|
//@[038:043) Identifier |typeA|
//@[044:045) Pipe |||
//@[046:051) Identifier |typeB|
//@[051:052) RightParen |)|
//@[053:054) Assignment |=|
//@[055:056) LeftBrace |{|
//@[057:061) Identifier |type|
//@[061:062) Colon |:|
//@[063:066) StringComplete |'b'|
//@[066:067) Comma |,|
//@[068:073) Identifier |value|
//@[073:074) Colon |:|
//@[075:076) Integer |0|
//@[077:078) RightBrace |}|
//@[078:080) NewLine |\n\n|

@discriminator('type')
//@[000:001) At |@|
//@[001:014) Identifier |discriminator|
//@[014:015) LeftParen |(|
//@[015:021) StringComplete |'type'|
//@[021:022) RightParen |)|
//@[022:023) NewLine |\n|
param paramInlineDiscriminatedUnion3 (typeA | typeB)?
//@[000:005) Identifier |param|
//@[006:036) Identifier |paramInlineDiscriminatedUnion3|
//@[037:038) LeftParen |(|
//@[038:043) Identifier |typeA|
//@[044:045) Pipe |||
//@[046:051) Identifier |typeB|
//@[051:052) RightParen |)|
//@[052:053) Question |?|
//@[053:055) NewLine |\n\n|

output outputDiscriminatedUnionTypeAlias1 discriminatedUnion1 = { type: 'a', value: 'str' }
//@[000:006) Identifier |output|
//@[007:041) Identifier |outputDiscriminatedUnionTypeAlias1|
//@[042:061) Identifier |discriminatedUnion1|
//@[062:063) Assignment |=|
//@[064:065) LeftBrace |{|
//@[066:070) Identifier |type|
//@[070:071) Colon |:|
//@[072:075) StringComplete |'a'|
//@[075:076) Comma |,|
//@[077:082) Identifier |value|
//@[082:083) Colon |:|
//@[084:089) StringComplete |'str'|
//@[090:091) RightBrace |}|
//@[091:092) NewLine |\n|
@discriminator('type')
//@[000:001) At |@|
//@[001:014) Identifier |discriminator|
//@[014:015) LeftParen |(|
//@[015:021) StringComplete |'type'|
//@[021:022) RightParen |)|
//@[022:023) NewLine |\n|
output outputDiscriminatedUnionTypeAlias2 discriminatedUnion1 = { type: 'a', value: 'str' }
//@[000:006) Identifier |output|
//@[007:041) Identifier |outputDiscriminatedUnionTypeAlias2|
//@[042:061) Identifier |discriminatedUnion1|
//@[062:063) Assignment |=|
//@[064:065) LeftBrace |{|
//@[066:070) Identifier |type|
//@[070:071) Colon |:|
//@[072:075) StringComplete |'a'|
//@[075:076) Comma |,|
//@[077:082) Identifier |value|
//@[082:083) Colon |:|
//@[084:089) StringComplete |'str'|
//@[090:091) RightBrace |}|
//@[091:092) NewLine |\n|
output outputDiscriminatedUnionTypeAlias3 discriminatedUnion5 = null
//@[000:006) Identifier |output|
//@[007:041) Identifier |outputDiscriminatedUnionTypeAlias3|
//@[042:061) Identifier |discriminatedUnion5|
//@[062:063) Assignment |=|
//@[064:068) NullKeyword |null|
//@[068:070) NewLine |\n\n|

@discriminator('type')
//@[000:001) At |@|
//@[001:014) Identifier |discriminator|
//@[014:015) LeftParen |(|
//@[015:021) StringComplete |'type'|
//@[021:022) RightParen |)|
//@[022:023) NewLine |\n|
output outputInlineDiscriminatedUnion1 typeA | typeB | { type: 'c', value: int } = { type: 'a', value: 'a' }
//@[000:006) Identifier |output|
//@[007:038) Identifier |outputInlineDiscriminatedUnion1|
//@[039:044) Identifier |typeA|
//@[045:046) Pipe |||
//@[047:052) Identifier |typeB|
//@[053:054) Pipe |||
//@[055:056) LeftBrace |{|
//@[057:061) Identifier |type|
//@[061:062) Colon |:|
//@[063:066) StringComplete |'c'|
//@[066:067) Comma |,|
//@[068:073) Identifier |value|
//@[073:074) Colon |:|
//@[075:078) Identifier |int|
//@[079:080) RightBrace |}|
//@[081:082) Assignment |=|
//@[083:084) LeftBrace |{|
//@[085:089) Identifier |type|
//@[089:090) Colon |:|
//@[091:094) StringComplete |'a'|
//@[094:095) Comma |,|
//@[096:101) Identifier |value|
//@[101:102) Colon |:|
//@[103:106) StringComplete |'a'|
//@[107:108) RightBrace |}|
//@[108:110) NewLine |\n\n|

@discriminator('type')
//@[000:001) At |@|
//@[001:014) Identifier |discriminator|
//@[014:015) LeftParen |(|
//@[015:021) StringComplete |'type'|
//@[021:022) RightParen |)|
//@[022:023) NewLine |\n|
output outputInlineDiscriminatedUnion2 typeA | typeB | ({ type: 'c', value: int }) = { type: 'c', value: 1 }
//@[000:006) Identifier |output|
//@[007:038) Identifier |outputInlineDiscriminatedUnion2|
//@[039:044) Identifier |typeA|
//@[045:046) Pipe |||
//@[047:052) Identifier |typeB|
//@[053:054) Pipe |||
//@[055:056) LeftParen |(|
//@[056:057) LeftBrace |{|
//@[058:062) Identifier |type|
//@[062:063) Colon |:|
//@[064:067) StringComplete |'c'|
//@[067:068) Comma |,|
//@[069:074) Identifier |value|
//@[074:075) Colon |:|
//@[076:079) Identifier |int|
//@[080:081) RightBrace |}|
//@[081:082) RightParen |)|
//@[083:084) Assignment |=|
//@[085:086) LeftBrace |{|
//@[087:091) Identifier |type|
//@[091:092) Colon |:|
//@[093:096) StringComplete |'c'|
//@[096:097) Comma |,|
//@[098:103) Identifier |value|
//@[103:104) Colon |:|
//@[105:106) Integer |1|
//@[107:108) RightBrace |}|
//@[108:110) NewLine |\n\n|

@discriminator('type')
//@[000:001) At |@|
//@[001:014) Identifier |discriminator|
//@[014:015) LeftParen |(|
//@[015:021) StringComplete |'type'|
//@[021:022) RightParen |)|
//@[022:023) NewLine |\n|
output outputInlineDiscriminatedUnion3 (typeA | typeB)? = null
//@[000:006) Identifier |output|
//@[007:038) Identifier |outputInlineDiscriminatedUnion3|
//@[039:040) LeftParen |(|
//@[040:045) Identifier |typeA|
//@[046:047) Pipe |||
//@[048:053) Identifier |typeB|
//@[053:054) RightParen |)|
//@[054:055) Question |?|
//@[056:057) Assignment |=|
//@[058:062) NullKeyword |null|
//@[062:063) NewLine |\n|

//@[000:000) EndOfFile ||
