@description('The foo type')
//@[00:001) At |@|
//@[01:012) Identifier |description|
//@[12:013) LeftParen |(|
//@[13:027) StringComplete |'The foo type'|
//@[27:028) RightParen |)|
//@[28:029) NewLine |\n|
@sealed()
//@[00:001) At |@|
//@[01:007) Identifier |sealed|
//@[07:008) LeftParen |(|
//@[08:009) RightParen |)|
//@[09:010) NewLine |\n|
type foo = {
//@[00:004) Identifier |type|
//@[05:008) Identifier |foo|
//@[09:010) Assignment |=|
//@[11:012) LeftBrace |{|
//@[12:013) NewLine |\n|
  @minLength(3)
//@[02:003) At |@|
//@[03:012) Identifier |minLength|
//@[12:013) LeftParen |(|
//@[13:014) Integer |3|
//@[14:015) RightParen |)|
//@[15:016) NewLine |\n|
  @maxLength(10)
//@[02:003) At |@|
//@[03:012) Identifier |maxLength|
//@[12:013) LeftParen |(|
//@[13:015) Integer |10|
//@[15:016) RightParen |)|
//@[16:017) NewLine |\n|
  @description('A string property')
//@[02:003) At |@|
//@[03:014) Identifier |description|
//@[14:015) LeftParen |(|
//@[15:034) StringComplete |'A string property'|
//@[34:035) RightParen |)|
//@[35:036) NewLine |\n|
  stringProp: string
//@[02:012) Identifier |stringProp|
//@[12:013) Colon |:|
//@[14:020) Identifier |string|
//@[20:022) NewLine |\n\n|

  objectProp: {
//@[02:012) Identifier |objectProp|
//@[12:013) Colon |:|
//@[14:015) LeftBrace |{|
//@[15:016) NewLine |\n|
    @minValue(1)
//@[04:005) At |@|
//@[05:013) Identifier |minValue|
//@[13:014) LeftParen |(|
//@[14:015) Integer |1|
//@[15:016) RightParen |)|
//@[16:017) NewLine |\n|
    intProp: int
//@[04:011) Identifier |intProp|
//@[11:012) Colon |:|
//@[13:016) Identifier |int|
//@[16:018) NewLine |\n\n|

    intArrayArrayProp: int [] [] ?
//@[04:021) Identifier |intArrayArrayProp|
//@[21:022) Colon |:|
//@[23:026) Identifier |int|
//@[27:028) LeftSquare |[|
//@[28:029) RightSquare |]|
//@[30:031) LeftSquare |[|
//@[31:032) RightSquare |]|
//@[33:034) Question |?|
//@[34:035) NewLine |\n|
  }
//@[02:003) RightBrace |}|
//@[03:005) NewLine |\n\n|

  typeRefProp: bar
//@[02:013) Identifier |typeRefProp|
//@[13:014) Colon |:|
//@[15:018) Identifier |bar|
//@[18:020) NewLine |\n\n|

  literalProp: 'literal'
//@[02:013) Identifier |literalProp|
//@[13:014) Colon |:|
//@[15:024) StringComplete |'literal'|
//@[24:026) NewLine |\n\n|

  recursion: foo?
//@[02:011) Identifier |recursion|
//@[11:012) Colon |:|
//@[13:016) Identifier |foo|
//@[16:017) Question |?|
//@[17:018) NewLine |\n|
}
//@[00:001) RightBrace |}|
//@[01:003) NewLine |\n\n|

@minLength(3)
//@[00:001) At |@|
//@[01:010) Identifier |minLength|
//@[10:011) LeftParen |(|
//@[11:012) Integer |3|
//@[12:013) RightParen |)|
//@[13:014) NewLine |\n|
@description('An array of array of arrays of arrays of ints')
//@[00:001) At |@|
//@[01:012) Identifier |description|
//@[12:013) LeftParen |(|
//@[13:060) StringComplete |'An array of array of arrays of arrays of ints'|
//@[60:061) RightParen |)|
//@[61:062) NewLine |\n|
@metadata({
//@[00:001) At |@|
//@[01:009) Identifier |metadata|
//@[09:010) LeftParen |(|
//@[10:011) LeftBrace |{|
//@[11:012) NewLine |\n|
  examples: [
//@[02:010) Identifier |examples|
//@[10:011) Colon |:|
//@[12:013) LeftSquare |[|
//@[13:014) NewLine |\n|
    [[[[1]]], [[[2]]], [[[3]]]]
//@[04:005) LeftSquare |[|
//@[05:006) LeftSquare |[|
//@[06:007) LeftSquare |[|
//@[07:008) LeftSquare |[|
//@[08:009) Integer |1|
//@[09:010) RightSquare |]|
//@[10:011) RightSquare |]|
//@[11:012) RightSquare |]|
//@[12:013) Comma |,|
//@[14:015) LeftSquare |[|
//@[15:016) LeftSquare |[|
//@[16:017) LeftSquare |[|
//@[17:018) Integer |2|
//@[18:019) RightSquare |]|
//@[19:020) RightSquare |]|
//@[20:021) RightSquare |]|
//@[21:022) Comma |,|
//@[23:024) LeftSquare |[|
//@[24:025) LeftSquare |[|
//@[25:026) LeftSquare |[|
//@[26:027) Integer |3|
//@[27:028) RightSquare |]|
//@[28:029) RightSquare |]|
//@[29:030) RightSquare |]|
//@[30:031) RightSquare |]|
//@[31:032) NewLine |\n|
  ]
//@[02:003) RightSquare |]|
//@[03:004) NewLine |\n|
})
//@[00:001) RightBrace |}|
//@[01:002) RightParen |)|
//@[02:003) NewLine |\n|
type bar = int[][][][]
//@[00:004) Identifier |type|
//@[05:008) Identifier |bar|
//@[09:010) Assignment |=|
//@[11:014) Identifier |int|
//@[14:015) LeftSquare |[|
//@[15:016) RightSquare |]|
//@[16:017) LeftSquare |[|
//@[17:018) RightSquare |]|
//@[18:019) LeftSquare |[|
//@[19:020) RightSquare |]|
//@[20:021) LeftSquare |[|
//@[21:022) RightSquare |]|
//@[22:024) NewLine |\n\n|

type aUnion = 'snap'|'crackle'|'pop'
//@[00:004) Identifier |type|
//@[05:011) Identifier |aUnion|
//@[12:013) Assignment |=|
//@[14:020) StringComplete |'snap'|
//@[20:021) Pipe |||
//@[21:030) StringComplete |'crackle'|
//@[30:031) Pipe |||
//@[31:036) StringComplete |'pop'|
//@[36:038) NewLine |\n\n|

type expandedUnion = aUnion|'fizz'|'buzz'|'pop'
//@[00:004) Identifier |type|
//@[05:018) Identifier |expandedUnion|
//@[19:020) Assignment |=|
//@[21:027) Identifier |aUnion|
//@[27:028) Pipe |||
//@[28:034) StringComplete |'fizz'|
//@[34:035) Pipe |||
//@[35:041) StringComplete |'buzz'|
//@[41:042) Pipe |||
//@[42:047) StringComplete |'pop'|
//@[47:049) NewLine |\n\n|

type tupleUnion = ['foo', 'bar', 'baz']
//@[00:004) Identifier |type|
//@[05:015) Identifier |tupleUnion|
//@[16:017) Assignment |=|
//@[18:019) LeftSquare |[|
//@[19:024) StringComplete |'foo'|
//@[24:025) Comma |,|
//@[26:031) StringComplete |'bar'|
//@[31:032) Comma |,|
//@[33:038) StringComplete |'baz'|
//@[38:039) RightSquare |]|
//@[39:040) NewLine |\n|
|['fizz', 'buzz']
//@[00:001) Pipe |||
//@[01:002) LeftSquare |[|
//@[02:008) StringComplete |'fizz'|
//@[08:009) Comma |,|
//@[10:016) StringComplete |'buzz'|
//@[16:017) RightSquare |]|
//@[17:018) NewLine |\n|
|['snap', 'crackle', 'pop']
//@[00:001) Pipe |||
//@[01:002) LeftSquare |[|
//@[02:008) StringComplete |'snap'|
//@[08:009) Comma |,|
//@[10:019) StringComplete |'crackle'|
//@[19:020) Comma |,|
//@[21:026) StringComplete |'pop'|
//@[26:027) RightSquare |]|
//@[27:029) NewLine |\n\n|

type mixedArray = ('heffalump'|'woozle'|{ shape: '*', size: '*'}|10|-10|true|!true|null)[]
//@[00:004) Identifier |type|
//@[05:015) Identifier |mixedArray|
//@[16:017) Assignment |=|
//@[18:019) LeftParen |(|
//@[19:030) StringComplete |'heffalump'|
//@[30:031) Pipe |||
//@[31:039) StringComplete |'woozle'|
//@[39:040) Pipe |||
//@[40:041) LeftBrace |{|
//@[42:047) Identifier |shape|
//@[47:048) Colon |:|
//@[49:052) StringComplete |'*'|
//@[52:053) Comma |,|
//@[54:058) Identifier |size|
//@[58:059) Colon |:|
//@[60:063) StringComplete |'*'|
//@[63:064) RightBrace |}|
//@[64:065) Pipe |||
//@[65:067) Integer |10|
//@[67:068) Pipe |||
//@[68:069) Minus |-|
//@[69:071) Integer |10|
//@[71:072) Pipe |||
//@[72:076) TrueKeyword |true|
//@[76:077) Pipe |||
//@[77:078) Exclamation |!|
//@[78:082) TrueKeyword |true|
//@[82:083) Pipe |||
//@[83:087) NullKeyword |null|
//@[87:088) RightParen |)|
//@[88:089) LeftSquare |[|
//@[89:090) RightSquare |]|
//@[90:092) NewLine |\n\n|

type bool = string
//@[00:004) Identifier |type|
//@[05:009) Identifier |bool|
//@[10:011) Assignment |=|
//@[12:018) Identifier |string|
//@[18:020) NewLine |\n\n|

param inlineObjectParam {
//@[00:005) Identifier |param|
//@[06:023) Identifier |inlineObjectParam|
//@[24:025) LeftBrace |{|
//@[25:026) NewLine |\n|
  foo: string
//@[02:005) Identifier |foo|
//@[05:006) Colon |:|
//@[07:013) Identifier |string|
//@[13:014) NewLine |\n|
  bar: 100|200|300|400|500
//@[02:005) Identifier |bar|
//@[05:006) Colon |:|
//@[07:010) Integer |100|
//@[10:011) Pipe |||
//@[11:014) Integer |200|
//@[14:015) Pipe |||
//@[15:018) Integer |300|
//@[18:019) Pipe |||
//@[19:022) Integer |400|
//@[22:023) Pipe |||
//@[23:026) Integer |500|
//@[26:027) NewLine |\n|
  baz: sys.bool
//@[02:005) Identifier |baz|
//@[05:006) Colon |:|
//@[07:010) Identifier |sys|
//@[10:011) Dot |.|
//@[11:015) Identifier |bool|
//@[15:016) NewLine |\n|
} = {
//@[00:001) RightBrace |}|
//@[02:003) Assignment |=|
//@[04:005) LeftBrace |{|
//@[05:006) NewLine |\n|
  foo: 'foo'
//@[02:005) Identifier |foo|
//@[05:006) Colon |:|
//@[07:012) StringComplete |'foo'|
//@[12:013) NewLine |\n|
  bar: 300
//@[02:005) Identifier |bar|
//@[05:006) Colon |:|
//@[07:010) Integer |300|
//@[10:011) NewLine |\n|
  baz: false
//@[02:005) Identifier |baz|
//@[05:006) Colon |:|
//@[07:012) FalseKeyword |false|
//@[12:013) NewLine |\n|
}
//@[00:001) RightBrace |}|
//@[01:003) NewLine |\n\n|

param unionParam {property: 'ping'}|{property: 'pong'} = {property: 'pong'}
//@[00:005) Identifier |param|
//@[06:016) Identifier |unionParam|
//@[17:018) LeftBrace |{|
//@[18:026) Identifier |property|
//@[26:027) Colon |:|
//@[28:034) StringComplete |'ping'|
//@[34:035) RightBrace |}|
//@[35:036) Pipe |||
//@[36:037) LeftBrace |{|
//@[37:045) Identifier |property|
//@[45:046) Colon |:|
//@[47:053) StringComplete |'pong'|
//@[53:054) RightBrace |}|
//@[55:056) Assignment |=|
//@[57:058) LeftBrace |{|
//@[58:066) Identifier |property|
//@[66:067) Colon |:|
//@[68:074) StringComplete |'pong'|
//@[74:075) RightBrace |}|
//@[75:077) NewLine |\n\n|

param paramUsingType mixedArray
//@[00:005) Identifier |param|
//@[06:020) Identifier |paramUsingType|
//@[21:031) Identifier |mixedArray|
//@[31:033) NewLine |\n\n|

output outputUsingType mixedArray = paramUsingType
//@[00:006) Identifier |output|
//@[07:022) Identifier |outputUsingType|
//@[23:033) Identifier |mixedArray|
//@[34:035) Assignment |=|
//@[36:050) Identifier |paramUsingType|
//@[50:052) NewLine |\n\n|

type tuple = [
//@[00:004) Identifier |type|
//@[05:010) Identifier |tuple|
//@[11:012) Assignment |=|
//@[13:014) LeftSquare |[|
//@[14:015) NewLine |\n|
    @description('A leading string')
//@[04:005) At |@|
//@[05:016) Identifier |description|
//@[16:017) LeftParen |(|
//@[17:035) StringComplete |'A leading string'|
//@[35:036) RightParen |)|
//@[36:037) NewLine |\n|
    string
//@[04:010) Identifier |string|
//@[10:012) NewLine |\n\n|

    @description('A second element using a type alias')
//@[04:005) At |@|
//@[05:016) Identifier |description|
//@[16:017) LeftParen |(|
//@[17:054) StringComplete |'A second element using a type alias'|
//@[54:055) RightParen |)|
//@[55:056) NewLine |\n|
    bar
//@[04:007) Identifier |bar|
//@[07:008) NewLine |\n|
]
//@[00:001) RightSquare |]|
//@[01:003) NewLine |\n\n|

type stringStringDictionary = {
//@[00:004) Identifier |type|
//@[05:027) Identifier |stringStringDictionary|
//@[28:029) Assignment |=|
//@[30:031) LeftBrace |{|
//@[31:032) NewLine |\n|
    *: string
//@[04:005) Asterisk |*|
//@[05:006) Colon |:|
//@[07:013) Identifier |string|
//@[13:014) NewLine |\n|
}
//@[00:001) RightBrace |}|
//@[01:003) NewLine |\n\n|

@minValue(1)
//@[00:001) At |@|
//@[01:009) Identifier |minValue|
//@[09:010) LeftParen |(|
//@[10:011) Integer |1|
//@[11:012) RightParen |)|
//@[12:013) NewLine |\n|
@maxValue(10)
//@[00:001) At |@|
//@[01:009) Identifier |maxValue|
//@[09:010) LeftParen |(|
//@[10:012) Integer |10|
//@[12:013) RightParen |)|
//@[13:014) NewLine |\n|
type constrainedInt = int
//@[00:004) Identifier |type|
//@[05:019) Identifier |constrainedInt|
//@[20:021) Assignment |=|
//@[22:025) Identifier |int|
//@[25:027) NewLine |\n\n|

param mightIncludeNull ({key: 'value'} | null)[]
//@[00:005) Identifier |param|
//@[06:022) Identifier |mightIncludeNull|
//@[23:024) LeftParen |(|
//@[24:025) LeftBrace |{|
//@[25:028) Identifier |key|
//@[28:029) Colon |:|
//@[30:037) StringComplete |'value'|
//@[37:038) RightBrace |}|
//@[39:040) Pipe |||
//@[41:045) NullKeyword |null|
//@[45:046) RightParen |)|
//@[46:047) LeftSquare |[|
//@[47:048) RightSquare |]|
//@[48:050) NewLine |\n\n|

var nonNull = mightIncludeNull[0]!.key
//@[00:003) Identifier |var|
//@[04:011) Identifier |nonNull|
//@[12:013) Assignment |=|
//@[14:030) Identifier |mightIncludeNull|
//@[30:031) LeftSquare |[|
//@[31:032) Integer |0|
//@[32:033) RightSquare |]|
//@[33:034) Exclamation |!|
//@[34:035) Dot |.|
//@[35:038) Identifier |key|
//@[38:040) NewLine |\n\n|

output nonNull string = nonNull
//@[00:006) Identifier |output|
//@[07:014) Identifier |nonNull|
//@[15:021) Identifier |string|
//@[22:023) Assignment |=|
//@[24:031) Identifier |nonNull|
//@[31:033) NewLine |\n\n|

var maybeNull = mightIncludeNull[0].?key
//@[00:003) Identifier |var|
//@[04:013) Identifier |maybeNull|
//@[14:015) Assignment |=|
//@[16:032) Identifier |mightIncludeNull|
//@[32:033) LeftSquare |[|
//@[33:034) Integer |0|
//@[34:035) RightSquare |]|
//@[35:036) Dot |.|
//@[36:037) Question |?|
//@[37:040) Identifier |key|
//@[40:042) NewLine |\n\n|

output maybeNull string? = maybeNull
//@[00:006) Identifier |output|
//@[07:016) Identifier |maybeNull|
//@[17:023) Identifier |string|
//@[23:024) Question |?|
//@[25:026) Assignment |=|
//@[27:036) Identifier |maybeNull|
//@[36:038) NewLine |\n\n|

type nullable = string?
//@[00:004) Identifier |type|
//@[05:013) Identifier |nullable|
//@[14:015) Assignment |=|
//@[16:022) Identifier |string|
//@[22:023) Question |?|
//@[23:025) NewLine |\n\n|

type nonNullable = nullable!
//@[00:004) Identifier |type|
//@[05:016) Identifier |nonNullable|
//@[17:018) Assignment |=|
//@[19:027) Identifier |nullable|
//@[27:028) Exclamation |!|
//@[28:030) NewLine |\n\n|

type typeA = {
//@[00:004) Identifier |type|
//@[05:010) Identifier |typeA|
//@[11:012) Assignment |=|
//@[13:014) LeftBrace |{|
//@[14:015) NewLine |\n|
  type: 'a'
//@[02:006) Identifier |type|
//@[06:007) Colon |:|
//@[08:011) StringComplete |'a'|
//@[11:012) NewLine |\n|
  value: string
//@[02:007) Identifier |value|
//@[07:008) Colon |:|
//@[09:015) Identifier |string|
//@[15:016) NewLine |\n|
}
//@[00:001) RightBrace |}|
//@[01:003) NewLine |\n\n|

type typeB = {
//@[00:004) Identifier |type|
//@[05:010) Identifier |typeB|
//@[11:012) Assignment |=|
//@[13:014) LeftBrace |{|
//@[14:015) NewLine |\n|
  type: 'b'
//@[02:006) Identifier |type|
//@[06:007) Colon |:|
//@[08:011) StringComplete |'b'|
//@[11:012) NewLine |\n|
  value: int
//@[02:007) Identifier |value|
//@[07:008) Colon |:|
//@[09:012) Identifier |int|
//@[12:013) NewLine |\n|
}
//@[00:001) RightBrace |}|
//@[01:003) NewLine |\n\n|

type typeC = {
//@[00:004) Identifier |type|
//@[05:010) Identifier |typeC|
//@[11:012) Assignment |=|
//@[13:014) LeftBrace |{|
//@[14:015) NewLine |\n|
  type: 'c'
//@[02:006) Identifier |type|
//@[06:007) Colon |:|
//@[08:011) StringComplete |'c'|
//@[11:012) NewLine |\n|
  value: bool
//@[02:007) Identifier |value|
//@[07:008) Colon |:|
//@[09:013) Identifier |bool|
//@[13:014) NewLine |\n|
  value2: string
//@[02:008) Identifier |value2|
//@[08:009) Colon |:|
//@[10:016) Identifier |string|
//@[16:017) NewLine |\n|
}
//@[00:001) RightBrace |}|
//@[01:003) NewLine |\n\n|

type typeD = {
//@[00:004) Identifier |type|
//@[05:010) Identifier |typeD|
//@[11:012) Assignment |=|
//@[13:014) LeftBrace |{|
//@[14:015) NewLine |\n|
  type: 'd'
//@[02:006) Identifier |type|
//@[06:007) Colon |:|
//@[08:011) StringComplete |'d'|
//@[11:012) NewLine |\n|
  value: object
//@[02:007) Identifier |value|
//@[07:008) Colon |:|
//@[09:015) Identifier |object|
//@[15:016) NewLine |\n|
}
//@[00:001) RightBrace |}|
//@[01:003) NewLine |\n\n|

type typeE = {
//@[00:004) Identifier |type|
//@[05:010) Identifier |typeE|
//@[11:012) Assignment |=|
//@[13:014) LeftBrace |{|
//@[14:015) NewLine |\n|
  type: 'e'
//@[02:006) Identifier |type|
//@[06:007) Colon |:|
//@[08:011) StringComplete |'e'|
//@[11:012) NewLine |\n|
  value: 'a' | 'b'
//@[02:007) Identifier |value|
//@[07:008) Colon |:|
//@[09:012) StringComplete |'a'|
//@[13:014) Pipe |||
//@[15:018) StringComplete |'b'|
//@[18:019) NewLine |\n|
}
//@[00:001) RightBrace |}|
//@[01:003) NewLine |\n\n|

@discriminator('type')
//@[00:001) At |@|
//@[01:014) Identifier |discriminator|
//@[14:015) LeftParen |(|
//@[15:021) StringComplete |'type'|
//@[21:022) RightParen |)|
//@[22:023) NewLine |\n|
type discriminatedUnion1 = typeA | typeB
//@[00:004) Identifier |type|
//@[05:024) Identifier |discriminatedUnion1|
//@[25:026) Assignment |=|
//@[27:032) Identifier |typeA|
//@[33:034) Pipe |||
//@[35:040) Identifier |typeB|
//@[40:042) NewLine |\n\n|

@discriminator('type')
//@[00:001) At |@|
//@[01:014) Identifier |discriminator|
//@[14:015) LeftParen |(|
//@[15:021) StringComplete |'type'|
//@[21:022) RightParen |)|
//@[22:023) NewLine |\n|
type discriminatedUnion2 = { type: 'c', value: string } | { type: 'd', value: bool }
//@[00:004) Identifier |type|
//@[05:024) Identifier |discriminatedUnion2|
//@[25:026) Assignment |=|
//@[27:028) LeftBrace |{|
//@[29:033) Identifier |type|
//@[33:034) Colon |:|
//@[35:038) StringComplete |'c'|
//@[38:039) Comma |,|
//@[40:045) Identifier |value|
//@[45:046) Colon |:|
//@[47:053) Identifier |string|
//@[54:055) RightBrace |}|
//@[56:057) Pipe |||
//@[58:059) LeftBrace |{|
//@[60:064) Identifier |type|
//@[64:065) Colon |:|
//@[66:069) StringComplete |'d'|
//@[69:070) Comma |,|
//@[71:076) Identifier |value|
//@[76:077) Colon |:|
//@[78:082) Identifier |bool|
//@[83:084) RightBrace |}|
//@[84:086) NewLine |\n\n|

@discriminator('type')
//@[00:001) At |@|
//@[01:014) Identifier |discriminator|
//@[14:015) LeftParen |(|
//@[15:021) StringComplete |'type'|
//@[21:022) RightParen |)|
//@[22:023) NewLine |\n|
type discriminatedUnion3 = discriminatedUnion1 | discriminatedUnion2 | { type: 'e', value: string }
//@[00:004) Identifier |type|
//@[05:024) Identifier |discriminatedUnion3|
//@[25:026) Assignment |=|
//@[27:046) Identifier |discriminatedUnion1|
//@[47:048) Pipe |||
//@[49:068) Identifier |discriminatedUnion2|
//@[69:070) Pipe |||
//@[71:072) LeftBrace |{|
//@[73:077) Identifier |type|
//@[77:078) Colon |:|
//@[79:082) StringComplete |'e'|
//@[82:083) Comma |,|
//@[84:089) Identifier |value|
//@[89:090) Colon |:|
//@[91:097) Identifier |string|
//@[98:099) RightBrace |}|
//@[99:101) NewLine |\n\n|

@discriminator('type')
//@[00:001) At |@|
//@[01:014) Identifier |discriminator|
//@[14:015) LeftParen |(|
//@[15:021) StringComplete |'type'|
//@[21:022) RightParen |)|
//@[22:023) NewLine |\n|
type discriminatedUnion4 = discriminatedUnion1 | (discriminatedUnion2 | typeE)
//@[00:004) Identifier |type|
//@[05:024) Identifier |discriminatedUnion4|
//@[25:026) Assignment |=|
//@[27:046) Identifier |discriminatedUnion1|
//@[47:048) Pipe |||
//@[49:050) LeftParen |(|
//@[50:069) Identifier |discriminatedUnion2|
//@[70:071) Pipe |||
//@[72:077) Identifier |typeE|
//@[77:078) RightParen |)|
//@[78:080) NewLine |\n\n|

type inlineDiscriminatedUnion1 = {
//@[00:004) Identifier |type|
//@[05:030) Identifier |inlineDiscriminatedUnion1|
//@[31:032) Assignment |=|
//@[33:034) LeftBrace |{|
//@[34:035) NewLine |\n|
  @discriminator('type')
//@[02:003) At |@|
//@[03:016) Identifier |discriminator|
//@[16:017) LeftParen |(|
//@[17:023) StringComplete |'type'|
//@[23:024) RightParen |)|
//@[24:025) NewLine |\n|
  prop: typeA | typeC
//@[02:006) Identifier |prop|
//@[06:007) Colon |:|
//@[08:013) Identifier |typeA|
//@[14:015) Pipe |||
//@[16:021) Identifier |typeC|
//@[21:022) NewLine |\n|
}
//@[00:001) RightBrace |}|
//@[01:003) NewLine |\n\n|

type inlineDiscriminatedUnion2 = {
//@[00:004) Identifier |type|
//@[05:030) Identifier |inlineDiscriminatedUnion2|
//@[31:032) Assignment |=|
//@[33:034) LeftBrace |{|
//@[34:035) NewLine |\n|
  @discriminator('type')
//@[02:003) At |@|
//@[03:016) Identifier |discriminator|
//@[16:017) LeftParen |(|
//@[17:023) StringComplete |'type'|
//@[23:024) RightParen |)|
//@[24:025) NewLine |\n|
  prop: { type: 'a', value: bool } | typeB
//@[02:006) Identifier |prop|
//@[06:007) Colon |:|
//@[08:009) LeftBrace |{|
//@[10:014) Identifier |type|
//@[14:015) Colon |:|
//@[16:019) StringComplete |'a'|
//@[19:020) Comma |,|
//@[21:026) Identifier |value|
//@[26:027) Colon |:|
//@[28:032) Identifier |bool|
//@[33:034) RightBrace |}|
//@[35:036) Pipe |||
//@[37:042) Identifier |typeB|
//@[42:043) NewLine |\n|
}
//@[00:001) RightBrace |}|
//@[01:003) NewLine |\n\n|

@discriminator('type')
//@[00:001) At |@|
//@[01:014) Identifier |discriminator|
//@[14:015) LeftParen |(|
//@[15:021) StringComplete |'type'|
//@[21:022) RightParen |)|
//@[22:023) NewLine |\n|
type inlineDiscriminatedUnion3 = {
//@[00:004) Identifier |type|
//@[05:030) Identifier |inlineDiscriminatedUnion3|
//@[31:032) Assignment |=|
//@[33:034) LeftBrace |{|
//@[34:035) NewLine |\n|
  type: 'a'
//@[02:006) Identifier |type|
//@[06:007) Colon |:|
//@[08:011) StringComplete |'a'|
//@[11:012) NewLine |\n|
  @discriminator('type')
//@[02:003) At |@|
//@[03:016) Identifier |discriminator|
//@[16:017) LeftParen |(|
//@[17:023) StringComplete |'type'|
//@[23:024) RightParen |)|
//@[24:025) NewLine |\n|
  prop: { type: 'a', value: bool } | typeB
//@[02:006) Identifier |prop|
//@[06:007) Colon |:|
//@[08:009) LeftBrace |{|
//@[10:014) Identifier |type|
//@[14:015) Colon |:|
//@[16:019) StringComplete |'a'|
//@[19:020) Comma |,|
//@[21:026) Identifier |value|
//@[26:027) Colon |:|
//@[28:032) Identifier |bool|
//@[33:034) RightBrace |}|
//@[35:036) Pipe |||
//@[37:042) Identifier |typeB|
//@[42:043) NewLine |\n|
} | {
//@[00:001) RightBrace |}|
//@[02:003) Pipe |||
//@[04:005) LeftBrace |{|
//@[05:006) NewLine |\n|
  type: 'b'
//@[02:006) Identifier |type|
//@[06:007) Colon |:|
//@[08:011) StringComplete |'b'|
//@[11:012) NewLine |\n|
  @discriminator('type')
//@[02:003) At |@|
//@[03:016) Identifier |discriminator|
//@[16:017) LeftParen |(|
//@[17:023) StringComplete |'type'|
//@[23:024) RightParen |)|
//@[24:025) NewLine |\n|
  prop: discriminatedUnion1 | discriminatedUnion2
//@[02:006) Identifier |prop|
//@[06:007) Colon |:|
//@[08:027) Identifier |discriminatedUnion1|
//@[28:029) Pipe |||
//@[30:049) Identifier |discriminatedUnion2|
//@[49:050) NewLine |\n|
}
//@[00:001) RightBrace |}|
//@[01:003) NewLine |\n\n|

type discriminatorUnionAsPropertyType = {
//@[00:004) Identifier |type|
//@[05:037) Identifier |discriminatorUnionAsPropertyType|
//@[38:039) Assignment |=|
//@[40:041) LeftBrace |{|
//@[41:042) NewLine |\n|
  prop1: discriminatedUnion1
//@[02:007) Identifier |prop1|
//@[07:008) Colon |:|
//@[09:028) Identifier |discriminatedUnion1|
//@[28:029) NewLine |\n|
  prop2: discriminatedUnion3
//@[02:007) Identifier |prop2|
//@[07:008) Colon |:|
//@[09:028) Identifier |discriminatedUnion3|
//@[28:029) NewLine |\n|
}
//@[00:001) RightBrace |}|
//@[01:003) NewLine |\n\n|

@discriminator('type')
//@[00:001) At |@|
//@[01:014) Identifier |discriminator|
//@[14:015) LeftParen |(|
//@[15:021) StringComplete |'type'|
//@[21:022) RightParen |)|
//@[22:023) NewLine |\n|
type discriminatorInnerSelfOptionalCycle1 = typeA | {
//@[00:004) Identifier |type|
//@[05:041) Identifier |discriminatorInnerSelfOptionalCycle1|
//@[42:043) Assignment |=|
//@[44:049) Identifier |typeA|
//@[50:051) Pipe |||
//@[52:053) LeftBrace |{|
//@[53:054) NewLine |\n|
  type: 'b'
//@[02:006) Identifier |type|
//@[06:007) Colon |:|
//@[08:011) StringComplete |'b'|
//@[11:012) NewLine |\n|
  value: discriminatorInnerSelfOptionalCycle1?
//@[02:007) Identifier |value|
//@[07:008) Colon |:|
//@[09:045) Identifier |discriminatorInnerSelfOptionalCycle1|
//@[45:046) Question |?|
//@[46:047) NewLine |\n|
}
//@[00:001) RightBrace |}|
//@[01:001) EndOfFile ||
