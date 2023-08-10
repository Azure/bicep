type 44
//@[000:004) Identifier |type|
//@[005:007) Integer |44|
//@[007:009) NewLine |\n\n|

type noAssignment
//@[000:004) Identifier |type|
//@[005:017) Identifier |noAssignment|
//@[017:019) NewLine |\n\n|

type incompleteAssignment =
//@[000:004) Identifier |type|
//@[005:025) Identifier |incompleteAssignment|
//@[026:027) Assignment |=|
//@[027:029) NewLine |\n\n|

type resource = bool
//@[000:004) Identifier |type|
//@[005:013) Identifier |resource|
//@[014:015) Assignment |=|
//@[016:020) Identifier |bool|
//@[020:022) NewLine |\n\n|

@sealed()
//@[000:001) At |@|
//@[001:007) Identifier |sealed|
//@[007:008) LeftParen |(|
//@[008:009) RightParen |)|
//@[009:010) NewLine |\n|
type sealedString = string
//@[000:004) Identifier |type|
//@[005:017) Identifier |sealedString|
//@[018:019) Assignment |=|
//@[020:026) Identifier |string|
//@[026:028) NewLine |\n\n|

@sealed()
//@[000:001) At |@|
//@[001:007) Identifier |sealed|
//@[007:008) LeftParen |(|
//@[008:009) RightParen |)|
//@[009:010) NewLine |\n|
type sealedDictionary = {
//@[000:004) Identifier |type|
//@[005:021) Identifier |sealedDictionary|
//@[022:023) Assignment |=|
//@[024:025) LeftBrace |{|
//@[025:026) NewLine |\n|
	*: string
//@[001:002) Asterisk |*|
//@[002:003) Colon |:|
//@[004:010) Identifier |string|
//@[010:011) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

type disallowedUnion = 'foo'|21
//@[000:004) Identifier |type|
//@[005:020) Identifier |disallowedUnion|
//@[021:022) Assignment |=|
//@[023:028) StringComplete |'foo'|
//@[028:029) Pipe |||
//@[029:031) Integer |21|
//@[031:033) NewLine |\n\n|

type validStringLiteralUnion = 'foo'|'bar'|'baz'
//@[000:004) Identifier |type|
//@[005:028) Identifier |validStringLiteralUnion|
//@[029:030) Assignment |=|
//@[031:036) StringComplete |'foo'|
//@[036:037) Pipe |||
//@[037:042) StringComplete |'bar'|
//@[042:043) Pipe |||
//@[043:048) StringComplete |'baz'|
//@[048:050) NewLine |\n\n|

type validUnionInvalidAddition = validStringLiteralUnion|10
//@[000:004) Identifier |type|
//@[005:030) Identifier |validUnionInvalidAddition|
//@[031:032) Assignment |=|
//@[033:056) Identifier |validStringLiteralUnion|
//@[056:057) Pipe |||
//@[057:059) Integer |10|
//@[059:061) NewLine |\n\n|

type invalidUnionInvalidAddition = disallowedUnion|true
//@[000:004) Identifier |type|
//@[005:032) Identifier |invalidUnionInvalidAddition|
//@[033:034) Assignment |=|
//@[035:050) Identifier |disallowedUnion|
//@[050:051) Pipe |||
//@[051:055) TrueKeyword |true|
//@[055:057) NewLine |\n\n|

type nullLiteral = null
//@[000:004) Identifier |type|
//@[005:016) Identifier |nullLiteral|
//@[017:018) Assignment |=|
//@[019:023) NullKeyword |null|
//@[023:025) NewLine |\n\n|

type unionOfNulls = null|null
//@[000:004) Identifier |type|
//@[005:017) Identifier |unionOfNulls|
//@[018:019) Assignment |=|
//@[020:024) NullKeyword |null|
//@[024:025) Pipe |||
//@[025:029) NullKeyword |null|
//@[029:031) NewLine |\n\n|

@minLength(3)
//@[000:001) At |@|
//@[001:010) Identifier |minLength|
//@[010:011) LeftParen |(|
//@[011:012) Integer |3|
//@[012:013) RightParen |)|
//@[013:014) NewLine |\n|
type lengthConstrainedInt = int
//@[000:004) Identifier |type|
//@[005:025) Identifier |lengthConstrainedInt|
//@[026:027) Assignment |=|
//@[028:031) Identifier |int|
//@[031:033) NewLine |\n\n|

@minValue(3)
//@[000:001) At |@|
//@[001:009) Identifier |minValue|
//@[009:010) LeftParen |(|
//@[010:011) Integer |3|
//@[011:012) RightParen |)|
//@[012:013) NewLine |\n|
type valueConstrainedString = string
//@[000:004) Identifier |type|
//@[005:027) Identifier |valueConstrainedString|
//@[028:029) Assignment |=|
//@[030:036) Identifier |string|
//@[036:038) NewLine |\n\n|

type tautology = tautology
//@[000:004) Identifier |type|
//@[005:014) Identifier |tautology|
//@[015:016) Assignment |=|
//@[017:026) Identifier |tautology|
//@[026:028) NewLine |\n\n|

type tautologicalUnion = tautologicalUnion|'foo'
//@[000:004) Identifier |type|
//@[005:022) Identifier |tautologicalUnion|
//@[023:024) Assignment |=|
//@[025:042) Identifier |tautologicalUnion|
//@[042:043) Pipe |||
//@[043:048) StringComplete |'foo'|
//@[048:050) NewLine |\n\n|

type tautologicalArray = tautologicalArray[]
//@[000:004) Identifier |type|
//@[005:022) Identifier |tautologicalArray|
//@[023:024) Assignment |=|
//@[025:042) Identifier |tautologicalArray|
//@[042:043) LeftSquare |[|
//@[043:044) RightSquare |]|
//@[044:046) NewLine |\n\n|

type directCycleStart = directCycleReturn
//@[000:004) Identifier |type|
//@[005:021) Identifier |directCycleStart|
//@[022:023) Assignment |=|
//@[024:041) Identifier |directCycleReturn|
//@[041:043) NewLine |\n\n|

type directCycleReturn = directCycleStart
//@[000:004) Identifier |type|
//@[005:022) Identifier |directCycleReturn|
//@[023:024) Assignment |=|
//@[025:041) Identifier |directCycleStart|
//@[041:043) NewLine |\n\n|

type cycleRoot = connector
//@[000:004) Identifier |type|
//@[005:014) Identifier |cycleRoot|
//@[015:016) Assignment |=|
//@[017:026) Identifier |connector|
//@[026:028) NewLine |\n\n|

type connector = cycleBack
//@[000:004) Identifier |type|
//@[005:014) Identifier |connector|
//@[015:016) Assignment |=|
//@[017:026) Identifier |cycleBack|
//@[026:028) NewLine |\n\n|

type cycleBack = cycleRoot
//@[000:004) Identifier |type|
//@[005:014) Identifier |cycleBack|
//@[015:016) Assignment |=|
//@[017:026) Identifier |cycleRoot|
//@[026:028) NewLine |\n\n|

type objectWithInvalidPropertyDecorators = {
//@[000:004) Identifier |type|
//@[005:040) Identifier |objectWithInvalidPropertyDecorators|
//@[041:042) Assignment |=|
//@[043:044) LeftBrace |{|
//@[044:045) NewLine |\n|
  @sealed()
//@[002:003) At |@|
//@[003:009) Identifier |sealed|
//@[009:010) LeftParen |(|
//@[010:011) RightParen |)|
//@[011:012) NewLine |\n|
  fooProp: string
//@[002:009) Identifier |fooProp|
//@[009:010) Colon |:|
//@[011:017) Identifier |string|
//@[017:019) NewLine |\n\n|

  @secure()
//@[002:003) At |@|
//@[003:009) Identifier |secure|
//@[009:010) LeftParen |(|
//@[010:011) RightParen |)|
//@[011:012) NewLine |\n|
  barProp: string
//@[002:009) Identifier |barProp|
//@[009:010) Colon |:|
//@[011:017) Identifier |string|
//@[017:019) NewLine |\n\n|

  @allowed(['snap', 'crackle', 'pop'])
//@[002:003) At |@|
//@[003:010) Identifier |allowed|
//@[010:011) LeftParen |(|
//@[011:012) LeftSquare |[|
//@[012:018) StringComplete |'snap'|
//@[018:019) Comma |,|
//@[020:029) StringComplete |'crackle'|
//@[029:030) Comma |,|
//@[031:036) StringComplete |'pop'|
//@[036:037) RightSquare |]|
//@[037:038) RightParen |)|
//@[038:039) NewLine |\n|
  krispyProp: string
//@[002:012) Identifier |krispyProp|
//@[012:013) Colon |:|
//@[014:020) Identifier |string|
//@[020:021) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

type objectWithInvalidRecursion = {
//@[000:004) Identifier |type|
//@[005:031) Identifier |objectWithInvalidRecursion|
//@[032:033) Assignment |=|
//@[034:035) LeftBrace |{|
//@[035:036) NewLine |\n|
  requiredAndRecursiveProp: objectWithInvalidRecursion
//@[002:026) Identifier |requiredAndRecursiveProp|
//@[026:027) Colon |:|
//@[028:054) Identifier |objectWithInvalidRecursion|
//@[054:055) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

type arrayWithInvalidMember = objectWithInvalidRecursion[]
//@[000:004) Identifier |type|
//@[005:027) Identifier |arrayWithInvalidMember|
//@[028:029) Assignment |=|
//@[030:056) Identifier |objectWithInvalidRecursion|
//@[056:057) LeftSquare |[|
//@[057:058) RightSquare |]|
//@[058:060) NewLine |\n\n|

@sealed()
//@[000:001) At |@|
//@[001:007) Identifier |sealed|
//@[007:008) LeftParen |(|
//@[008:009) RightParen |)|
//@[009:010) NewLine |\n|
param sealedStringParam string
//@[000:005) Identifier |param|
//@[006:023) Identifier |sealedStringParam|
//@[024:030) Identifier |string|
//@[030:032) NewLine |\n\n|

param disallowedUnionParam 'foo'|-99
//@[000:005) Identifier |param|
//@[006:026) Identifier |disallowedUnionParam|
//@[027:032) StringComplete |'foo'|
//@[032:033) Pipe |||
//@[033:034) Minus |-|
//@[034:036) Integer |99|
//@[036:038) NewLine |\n\n|

param objectWithInvalidRecursionParam objectWithInvalidRecursion
//@[000:005) Identifier |param|
//@[006:037) Identifier |objectWithInvalidRecursionParam|
//@[038:064) Identifier |objectWithInvalidRecursion|
//@[064:066) NewLine |\n\n|

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

@discriminator('type')
//@[000:001) At |@|
//@[001:014) Identifier |discriminator|
//@[014:015) LeftParen |(|
//@[015:021) StringComplete |'type'|
//@[021:022) RightParen |)|
//@[022:023) NewLine |\n|
type unionAB = typeA | typeB
//@[000:004) Identifier |type|
//@[005:012) Identifier |unionAB|
//@[013:014) Assignment |=|
//@[015:020) Identifier |typeA|
//@[021:022) Pipe |||
//@[023:028) Identifier |typeB|
//@[028:030) NewLine |\n\n|

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
  *: string
//@[002:003) Asterisk |*|
//@[003:004) Colon |:|
//@[005:011) Identifier |string|
//@[011:012) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

type typeF = {
//@[000:004) Identifier |type|
//@[005:010) Identifier |typeF|
//@[011:012) Assignment |=|
//@[013:014) LeftBrace |{|
//@[014:015) NewLine |\n|
  type: 0
//@[002:006) Identifier |type|
//@[006:007) Colon |:|
//@[008:009) Integer |0|
//@[009:010) NewLine |\n|
  value: string
//@[002:007) Identifier |value|
//@[007:008) Colon |:|
//@[009:015) Identifier |string|
//@[015:016) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

type typeG = {
//@[000:004) Identifier |type|
//@[005:010) Identifier |typeG|
//@[011:012) Assignment |=|
//@[013:014) LeftBrace |{|
//@[014:015) NewLine |\n|
  type: 'g'?
//@[002:006) Identifier |type|
//@[006:007) Colon |:|
//@[008:011) StringComplete |'g'|
//@[011:012) Question |?|
//@[012:013) NewLine |\n|
  value: string
//@[002:007) Identifier |value|
//@[007:008) Colon |:|
//@[009:015) Identifier |string|
//@[015:016) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

type objectUnion = typeA | typeB
//@[000:004) Identifier |type|
//@[005:016) Identifier |objectUnion|
//@[017:018) Assignment |=|
//@[019:024) Identifier |typeA|
//@[025:026) Pipe |||
//@[027:032) Identifier |typeB|
//@[032:034) NewLine |\n\n|

@discriminator()
//@[000:001) At |@|
//@[001:014) Identifier |discriminator|
//@[014:015) LeftParen |(|
//@[015:016) RightParen |)|
//@[016:017) NewLine |\n|
type noDiscriminatorParam = typeA | typeB
//@[000:004) Identifier |type|
//@[005:025) Identifier |noDiscriminatorParam|
//@[026:027) Assignment |=|
//@[028:033) Identifier |typeA|
//@[034:035) Pipe |||
//@[036:041) Identifier |typeB|
//@[041:043) NewLine |\n\n|

@discriminator(true)
//@[000:001) At |@|
//@[001:014) Identifier |discriminator|
//@[014:015) LeftParen |(|
//@[015:019) TrueKeyword |true|
//@[019:020) RightParen |)|
//@[020:021) NewLine |\n|
type wrongDiscriminatorParamType = typeA | typeB
//@[000:004) Identifier |type|
//@[005:032) Identifier |wrongDiscriminatorParamType|
//@[033:034) Assignment |=|
//@[035:040) Identifier |typeA|
//@[041:042) Pipe |||
//@[043:048) Identifier |typeB|
//@[048:050) NewLine |\n\n|

@discriminator('nonexistent')
//@[000:001) At |@|
//@[001:014) Identifier |discriminator|
//@[014:015) LeftParen |(|
//@[015:028) StringComplete |'nonexistent'|
//@[028:029) RightParen |)|
//@[029:030) NewLine |\n|
type discriminatorPropertyNotExistAtAll = typeA | typeB
//@[000:004) Identifier |type|
//@[005:039) Identifier |discriminatorPropertyNotExistAtAll|
//@[040:041) Assignment |=|
//@[042:047) Identifier |typeA|
//@[048:049) Pipe |||
//@[050:055) Identifier |typeB|
//@[055:057) NewLine |\n\n|

@discriminator('nonexistent')
//@[000:001) At |@|
//@[001:014) Identifier |discriminator|
//@[014:015) LeftParen |(|
//@[015:028) StringComplete |'nonexistent'|
//@[028:029) RightParen |)|
//@[029:030) NewLine |\n|
type discriminatorPropertyMismatch = unionAB
//@[000:004) Identifier |type|
//@[005:034) Identifier |discriminatorPropertyMismatch|
//@[035:036) Assignment |=|
//@[037:044) Identifier |unionAB|
//@[044:046) NewLine |\n\n|

@discriminator('type')
//@[000:001) At |@|
//@[001:014) Identifier |discriminator|
//@[014:015) LeftParen |(|
//@[015:021) StringComplete |'type'|
//@[021:022) RightParen |)|
//@[022:023) NewLine |\n|
type discriminatorPropertyNotExistOnAtLeastOne = typeA | { value: bool }
//@[000:004) Identifier |type|
//@[005:046) Identifier |discriminatorPropertyNotExistOnAtLeastOne|
//@[047:048) Assignment |=|
//@[049:054) Identifier |typeA|
//@[055:056) Pipe |||
//@[057:058) LeftBrace |{|
//@[059:064) Identifier |value|
//@[064:065) Colon |:|
//@[066:070) Identifier |bool|
//@[071:072) RightBrace |}|
//@[072:074) NewLine |\n\n|

@discriminator('type')
//@[000:001) At |@|
//@[001:014) Identifier |discriminator|
//@[014:015) LeftParen |(|
//@[015:021) StringComplete |'type'|
//@[021:022) RightParen |)|
//@[022:023) NewLine |\n|
type discriminatorWithOnlyOneMember = typeA
//@[000:004) Identifier |type|
//@[005:035) Identifier |discriminatorWithOnlyOneMember|
//@[036:037) Assignment |=|
//@[038:043) Identifier |typeA|
//@[043:045) NewLine |\n\n|

@discriminator('type')
//@[000:001) At |@|
//@[001:014) Identifier |discriminator|
//@[014:015) LeftParen |(|
//@[015:021) StringComplete |'type'|
//@[021:022) RightParen |)|
//@[022:023) NewLine |\n|
type discriminatorPropertyNotRequiredStringLiteral1 = typeA | typeF
//@[000:004) Identifier |type|
//@[005:051) Identifier |discriminatorPropertyNotRequiredStringLiteral1|
//@[052:053) Assignment |=|
//@[054:059) Identifier |typeA|
//@[060:061) Pipe |||
//@[062:067) Identifier |typeF|
//@[067:069) NewLine |\n\n|

@discriminator('type')
//@[000:001) At |@|
//@[001:014) Identifier |discriminator|
//@[014:015) LeftParen |(|
//@[015:021) StringComplete |'type'|
//@[021:022) RightParen |)|
//@[022:023) NewLine |\n|
type discriminatorPropertyNotRequiredStringLiteral2 = typeA | typeG
//@[000:004) Identifier |type|
//@[005:051) Identifier |discriminatorPropertyNotRequiredStringLiteral2|
//@[052:053) Assignment |=|
//@[054:059) Identifier |typeA|
//@[060:061) Pipe |||
//@[062:067) Identifier |typeG|
//@[067:069) NewLine |\n\n|

@discriminator('type')
//@[000:001) At |@|
//@[001:014) Identifier |discriminator|
//@[014:015) LeftParen |(|
//@[015:021) StringComplete |'type'|
//@[021:022) RightParen |)|
//@[022:023) NewLine |\n|
type discriminatorDuplicatedMember1 = typeA | typeA
//@[000:004) Identifier |type|
//@[005:035) Identifier |discriminatorDuplicatedMember1|
//@[036:037) Assignment |=|
//@[038:043) Identifier |typeA|
//@[044:045) Pipe |||
//@[046:051) Identifier |typeA|
//@[051:053) NewLine |\n\n|

@discriminator('type')
//@[000:001) At |@|
//@[001:014) Identifier |discriminator|
//@[014:015) LeftParen |(|
//@[015:021) StringComplete |'type'|
//@[021:022) RightParen |)|
//@[022:023) NewLine |\n|
type discriminatorDuplicatedMember2 = typeA | { type: 'a', config: object }
//@[000:004) Identifier |type|
//@[005:035) Identifier |discriminatorDuplicatedMember2|
//@[036:037) Assignment |=|
//@[038:043) Identifier |typeA|
//@[044:045) Pipe |||
//@[046:047) LeftBrace |{|
//@[048:052) Identifier |type|
//@[052:053) Colon |:|
//@[054:057) StringComplete |'a'|
//@[057:058) Comma |,|
//@[059:065) Identifier |config|
//@[065:066) Colon |:|
//@[067:073) Identifier |object|
//@[074:075) RightBrace |}|
//@[075:077) NewLine |\n\n|

@discriminator('type')
//@[000:001) At |@|
//@[001:014) Identifier |discriminator|
//@[014:015) LeftParen |(|
//@[015:021) StringComplete |'type'|
//@[021:022) RightParen |)|
//@[022:023) NewLine |\n|
type discriminatorOnlyOneNonNullMember1 = typeA | null
//@[000:004) Identifier |type|
//@[005:039) Identifier |discriminatorOnlyOneNonNullMember1|
//@[040:041) Assignment |=|
//@[042:047) Identifier |typeA|
//@[048:049) Pipe |||
//@[050:054) NullKeyword |null|
//@[054:056) NewLine |\n\n|

@discriminator('type')
//@[000:001) At |@|
//@[001:014) Identifier |discriminator|
//@[014:015) LeftParen |(|
//@[015:021) StringComplete |'type'|
//@[021:022) RightParen |)|
//@[022:023) NewLine |\n|
type discriminatorOnlyOneNonNullMember2 = (typeA)?
//@[000:004) Identifier |type|
//@[005:039) Identifier |discriminatorOnlyOneNonNullMember2|
//@[040:041) Assignment |=|
//@[042:043) LeftParen |(|
//@[043:048) Identifier |typeA|
//@[048:049) RightParen |)|
//@[049:050) Question |?|
//@[050:052) NewLine |\n\n|

@discriminator('type')
//@[000:001) At |@|
//@[001:014) Identifier |discriminator|
//@[014:015) LeftParen |(|
//@[015:021) StringComplete |'type'|
//@[021:022) RightParen |)|
//@[022:023) NewLine |\n|
type discriminatorMemberHasAdditionalProperties = typeA | typeE
//@[000:004) Identifier |type|
//@[005:047) Identifier |discriminatorMemberHasAdditionalProperties|
//@[048:049) Assignment |=|
//@[050:055) Identifier |typeA|
//@[056:057) Pipe |||
//@[058:063) Identifier |typeE|
//@[063:065) NewLine |\n\n|

@discriminator('type')
//@[000:001) At |@|
//@[001:014) Identifier |discriminator|
//@[014:015) LeftParen |(|
//@[015:021) StringComplete |'type'|
//@[021:022) RightParen |)|
//@[022:023) NewLine |\n|
type discriminatorSelfCycle1 = typeA | discriminatorSelfCycle1
//@[000:004) Identifier |type|
//@[005:028) Identifier |discriminatorSelfCycle1|
//@[029:030) Assignment |=|
//@[031:036) Identifier |typeA|
//@[037:038) Pipe |||
//@[039:062) Identifier |discriminatorSelfCycle1|
//@[062:064) NewLine |\n\n|

@discriminator('type')
//@[000:001) At |@|
//@[001:014) Identifier |discriminator|
//@[014:015) LeftParen |(|
//@[015:021) StringComplete |'type'|
//@[021:022) RightParen |)|
//@[022:023) NewLine |\n|
type discriminatorSelfCycle2 = (typeA | discriminatorSelfCycle2)?
//@[000:004) Identifier |type|
//@[005:028) Identifier |discriminatorSelfCycle2|
//@[029:030) Assignment |=|
//@[031:032) LeftParen |(|
//@[032:037) Identifier |typeA|
//@[038:039) Pipe |||
//@[040:063) Identifier |discriminatorSelfCycle2|
//@[063:064) RightParen |)|
//@[064:065) Question |?|
//@[065:067) NewLine |\n\n|

@discriminator('type')
//@[000:001) At |@|
//@[001:014) Identifier |discriminator|
//@[014:015) LeftParen |(|
//@[015:021) StringComplete |'type'|
//@[021:022) RightParen |)|
//@[022:023) NewLine |\n|
type discriminatorTopLevelCycleA = typeA | discriminatorTopLevelCycleB
//@[000:004) Identifier |type|
//@[005:032) Identifier |discriminatorTopLevelCycleA|
//@[033:034) Assignment |=|
//@[035:040) Identifier |typeA|
//@[041:042) Pipe |||
//@[043:070) Identifier |discriminatorTopLevelCycleB|
//@[070:071) NewLine |\n|
@discriminator('type')
//@[000:001) At |@|
//@[001:014) Identifier |discriminator|
//@[014:015) LeftParen |(|
//@[015:021) StringComplete |'type'|
//@[021:022) RightParen |)|
//@[022:023) NewLine |\n|
type discriminatorTopLevelCycleB = typeB | discriminatorTopLevelCycleA
//@[000:004) Identifier |type|
//@[005:032) Identifier |discriminatorTopLevelCycleB|
//@[033:034) Assignment |=|
//@[035:040) Identifier |typeB|
//@[041:042) Pipe |||
//@[043:070) Identifier |discriminatorTopLevelCycleA|
//@[070:072) NewLine |\n\n|

@discriminator('type')
//@[000:001) At |@|
//@[001:014) Identifier |discriminator|
//@[014:015) LeftParen |(|
//@[015:021) StringComplete |'type'|
//@[021:022) RightParen |)|
//@[022:023) NewLine |\n|
type discriminatorInnerSelfCycle1 = typeA | {
//@[000:004) Identifier |type|
//@[005:033) Identifier |discriminatorInnerSelfCycle1|
//@[034:035) Assignment |=|
//@[036:041) Identifier |typeA|
//@[042:043) Pipe |||
//@[044:045) LeftBrace |{|
//@[045:046) NewLine |\n|
  type: 'b'
//@[002:006) Identifier |type|
//@[006:007) Colon |:|
//@[008:011) StringComplete |'b'|
//@[011:012) NewLine |\n|
  value: discriminatorInnerSelfCycle1
//@[002:007) Identifier |value|
//@[007:008) Colon |:|
//@[009:037) Identifier |discriminatorInnerSelfCycle1|
//@[037:038) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

type discriminatorInnerSelfCycle2Helper = {
//@[000:004) Identifier |type|
//@[005:039) Identifier |discriminatorInnerSelfCycle2Helper|
//@[040:041) Assignment |=|
//@[042:043) LeftBrace |{|
//@[043:044) NewLine |\n|
  type: 'b'
//@[002:006) Identifier |type|
//@[006:007) Colon |:|
//@[008:011) StringComplete |'b'|
//@[011:012) NewLine |\n|
  value: discriminatorInnerSelfCycle2
//@[002:007) Identifier |value|
//@[007:008) Colon |:|
//@[009:037) Identifier |discriminatorInnerSelfCycle2|
//@[037:038) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:002) NewLine |\n|
@discriminator('type')
//@[000:001) At |@|
//@[001:014) Identifier |discriminator|
//@[014:015) LeftParen |(|
//@[015:021) StringComplete |'type'|
//@[021:022) RightParen |)|
//@[022:023) NewLine |\n|
type discriminatorInnerSelfCycle2 = typeA | discriminatorInnerSelfCycle2Helper
//@[000:004) Identifier |type|
//@[005:033) Identifier |discriminatorInnerSelfCycle2|
//@[034:035) Assignment |=|
//@[036:041) Identifier |typeA|
//@[042:043) Pipe |||
//@[044:078) Identifier |discriminatorInnerSelfCycle2Helper|
//@[078:080) NewLine |\n\n|

@discriminator('type')
//@[000:001) At |@|
//@[001:014) Identifier |discriminator|
//@[014:015) LeftParen |(|
//@[015:021) StringComplete |'type'|
//@[021:022) RightParen |)|
//@[022:023) NewLine |\n|
type discriminatorTupleBadType1 = [typeA, typeB]
//@[000:004) Identifier |type|
//@[005:031) Identifier |discriminatorTupleBadType1|
//@[032:033) Assignment |=|
//@[034:035) LeftSquare |[|
//@[035:040) Identifier |typeA|
//@[040:041) Comma |,|
//@[042:047) Identifier |typeB|
//@[047:048) RightSquare |]|
//@[048:050) NewLine |\n\n|

@discriminator('type')
//@[000:001) At |@|
//@[001:014) Identifier |discriminator|
//@[014:015) LeftParen |(|
//@[015:021) StringComplete |'type'|
//@[021:022) RightParen |)|
//@[022:023) NewLine |\n|
type discriminatorTupleBadType2 = [typeA | typeB]
//@[000:004) Identifier |type|
//@[005:031) Identifier |discriminatorTupleBadType2|
//@[032:033) Assignment |=|
//@[034:035) LeftSquare |[|
//@[035:040) Identifier |typeA|
//@[041:042) Pipe |||
//@[043:048) Identifier |typeB|
//@[048:049) RightSquare |]|
//@[049:051) NewLine |\n\n|

@discriminator('type')
//@[000:001) At |@|
//@[001:014) Identifier |discriminator|
//@[014:015) LeftParen |(|
//@[015:021) StringComplete |'type'|
//@[021:022) RightParen |)|
//@[022:023) NewLine |\n|
type discriminatorTupleBadType3 = [typeA | typeB, typeC | typeD]
//@[000:004) Identifier |type|
//@[005:031) Identifier |discriminatorTupleBadType3|
//@[032:033) Assignment |=|
//@[034:035) LeftSquare |[|
//@[035:040) Identifier |typeA|
//@[041:042) Pipe |||
//@[043:048) Identifier |typeB|
//@[048:049) Comma |,|
//@[050:055) Identifier |typeC|
//@[056:057) Pipe |||
//@[058:063) Identifier |typeD|
//@[063:064) RightSquare |]|
//@[064:066) NewLine |\n\n|

type discriminatorInlineAdditionalPropsBadType1 = {
//@[000:004) Identifier |type|
//@[005:047) Identifier |discriminatorInlineAdditionalPropsBadType1|
//@[048:049) Assignment |=|
//@[050:051) LeftBrace |{|
//@[051:052) NewLine |\n|
  @discriminator('type')
//@[002:003) At |@|
//@[003:016) Identifier |discriminator|
//@[016:017) LeftParen |(|
//@[017:023) StringComplete |'type'|
//@[023:024) RightParen |)|
//@[024:025) NewLine |\n|
  *: typeA
//@[002:003) Asterisk |*|
//@[003:004) Colon |:|
//@[005:010) Identifier |typeA|
//@[010:011) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

type discriminatorInlineAdditionalPropsBadType2 = {
//@[000:004) Identifier |type|
//@[005:047) Identifier |discriminatorInlineAdditionalPropsBadType2|
//@[048:049) Assignment |=|
//@[050:051) LeftBrace |{|
//@[051:052) NewLine |\n|
  @discriminator('type')
//@[002:003) At |@|
//@[003:016) Identifier |discriminator|
//@[016:017) LeftParen |(|
//@[017:023) StringComplete |'type'|
//@[023:024) RightParen |)|
//@[024:025) NewLine |\n|
  *: typeA | typeA
//@[002:003) Asterisk |*|
//@[003:004) Colon |:|
//@[005:010) Identifier |typeA|
//@[011:012) Pipe |||
//@[013:018) Identifier |typeA|
//@[018:019) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

type discriminatorInlineAdditionalPropsBadType3 = {
//@[000:004) Identifier |type|
//@[005:047) Identifier |discriminatorInlineAdditionalPropsBadType3|
//@[048:049) Assignment |=|
//@[050:051) LeftBrace |{|
//@[051:052) NewLine |\n|
  @discriminator('type')
//@[002:003) At |@|
//@[003:016) Identifier |discriminator|
//@[016:017) LeftParen |(|
//@[017:023) StringComplete |'type'|
//@[023:024) RightParen |)|
//@[024:025) NewLine |\n|
  *: string
//@[002:003) Asterisk |*|
//@[003:004) Colon |:|
//@[005:011) Identifier |string|
//@[011:012) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:003) NewLine |\n\n|

type discriminatorInlineAdditionalPropsCycle1 = {
//@[000:004) Identifier |type|
//@[005:045) Identifier |discriminatorInlineAdditionalPropsCycle1|
//@[046:047) Assignment |=|
//@[048:049) LeftBrace |{|
//@[049:050) NewLine |\n|
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
  *: typeA | discriminatorInlineAdditionalPropsCycle1
//@[002:003) Asterisk |*|
//@[003:004) Colon |:|
//@[005:010) Identifier |typeA|
//@[011:012) Pipe |||
//@[013:053) Identifier |discriminatorInlineAdditionalPropsCycle1|
//@[053:054) NewLine |\n|
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
type discriminatedUnionDuplicateMemberInsensitive = { type: 'a', value: string } | { type: 'A', value: int }
//@[000:004) Identifier |type|
//@[005:049) Identifier |discriminatedUnionDuplicateMemberInsensitive|
//@[050:051) Assignment |=|
//@[052:053) LeftBrace |{|
//@[054:058) Identifier |type|
//@[058:059) Colon |:|
//@[060:063) StringComplete |'a'|
//@[063:064) Comma |,|
//@[065:070) Identifier |value|
//@[070:071) Colon |:|
//@[072:078) Identifier |string|
//@[079:080) RightBrace |}|
//@[081:082) Pipe |||
//@[083:084) LeftBrace |{|
//@[085:089) Identifier |type|
//@[089:090) Colon |:|
//@[091:094) StringComplete |'A'|
//@[094:095) Comma |,|
//@[096:101) Identifier |value|
//@[101:102) Colon |:|
//@[103:106) Identifier |int|
//@[107:108) RightBrace |}|
//@[108:110) NewLine |\n\n|

@discriminator('TYPE')
//@[000:001) At |@|
//@[001:014) Identifier |discriminator|
//@[014:015) LeftParen |(|
//@[015:021) StringComplete |'TYPE'|
//@[021:022) RightParen |)|
//@[022:023) NewLine |\n|
type discriminatedUnionCaseSensitiveDiscriminator = { type: 'a', value: string } | { type: 'b', value: int }
//@[000:004) Identifier |type|
//@[005:049) Identifier |discriminatedUnionCaseSensitiveDiscriminator|
//@[050:051) Assignment |=|
//@[052:053) LeftBrace |{|
//@[054:058) Identifier |type|
//@[058:059) Colon |:|
//@[060:063) StringComplete |'a'|
//@[063:064) Comma |,|
//@[065:070) Identifier |value|
//@[070:071) Colon |:|
//@[072:078) Identifier |string|
//@[079:080) RightBrace |}|
//@[081:082) Pipe |||
//@[083:084) LeftBrace |{|
//@[085:089) Identifier |type|
//@[089:090) Colon |:|
//@[091:094) StringComplete |'b'|
//@[094:095) Comma |,|
//@[096:101) Identifier |value|
//@[101:102) Colon |:|
//@[103:106) Identifier |int|
//@[107:108) RightBrace |}|
//@[108:110) NewLine |\n\n|

@discriminator('type')
//@[000:001) At |@|
//@[001:014) Identifier |discriminator|
//@[014:015) LeftParen |(|
//@[015:021) StringComplete |'type'|
//@[021:022) RightParen |)|
//@[022:023) NewLine |\n|
param discriminatorParamBadType1 typeA
//@[000:005) Identifier |param|
//@[006:032) Identifier |discriminatorParamBadType1|
//@[033:038) Identifier |typeA|
//@[038:040) NewLine |\n\n|

@discriminator('type')
//@[000:001) At |@|
//@[001:014) Identifier |discriminator|
//@[014:015) LeftParen |(|
//@[015:021) StringComplete |'type'|
//@[021:022) RightParen |)|
//@[022:023) NewLine |\n|
param discriminatorParamBadType2 'a' | 'b'
//@[000:005) Identifier |param|
//@[006:032) Identifier |discriminatorParamBadType2|
//@[033:036) StringComplete |'a'|
//@[037:038) Pipe |||
//@[039:042) StringComplete |'b'|
//@[042:044) NewLine |\n\n|

@discriminator('type')
//@[000:001) At |@|
//@[001:014) Identifier |discriminator|
//@[014:015) LeftParen |(|
//@[015:021) StringComplete |'type'|
//@[021:022) RightParen |)|
//@[022:023) NewLine |\n|
output discriminatorOutputBadType1 typeA = { type: 'a', value: 'a' }
//@[000:006) Identifier |output|
//@[007:034) Identifier |discriminatorOutputBadType1|
//@[035:040) Identifier |typeA|
//@[041:042) Assignment |=|
//@[043:044) LeftBrace |{|
//@[045:049) Identifier |type|
//@[049:050) Colon |:|
//@[051:054) StringComplete |'a'|
//@[054:055) Comma |,|
//@[056:061) Identifier |value|
//@[061:062) Colon |:|
//@[063:066) StringComplete |'a'|
//@[067:068) RightBrace |}|
//@[068:070) NewLine |\n\n|

@discriminator('type')
//@[000:001) At |@|
//@[001:014) Identifier |discriminator|
//@[014:015) LeftParen |(|
//@[015:021) StringComplete |'type'|
//@[021:022) RightParen |)|
//@[022:023) NewLine |\n|
output discriminatorOutputBadType2 object = { prop: 'value' }
//@[000:006) Identifier |output|
//@[007:034) Identifier |discriminatorOutputBadType2|
//@[035:041) Identifier |object|
//@[042:043) Assignment |=|
//@[044:045) LeftBrace |{|
//@[046:050) Identifier |prop|
//@[050:051) Colon |:|
//@[052:059) StringComplete |'value'|
//@[060:061) RightBrace |}|
//@[061:063) NewLine |\n\n|

// BEGIN: valid tagged unions baselines; move this back to TypeDeclarations_LF when backend updates are released and uncomment typesA-D
//@[135:136) NewLine |\n|
//type typeA = {
//@[016:017) NewLine |\n|
//  type: 'a'
//@[013:014) NewLine |\n|
//  value: string
//@[017:018) NewLine |\n|
//}
//@[003:004) NewLine |\n|
//
//@[002:003) NewLine |\n|
//type typeB = {
//@[016:017) NewLine |\n|
//  type: 'b'
//@[013:014) NewLine |\n|
//  value: int
//@[014:015) NewLine |\n|
//}
//@[003:004) NewLine |\n|
//
//@[002:003) NewLine |\n|
//type typeC = {
//@[016:017) NewLine |\n|
//  type: 'c'
//@[013:014) NewLine |\n|
//  value: bool
//@[015:016) NewLine |\n|
//  value2: string
//@[018:019) NewLine |\n|
//}
//@[003:004) NewLine |\n|
//
//@[002:003) NewLine |\n|
//type typeD = {
//@[016:017) NewLine |\n|
//  type: 'd'
//@[013:014) NewLine |\n|
//  value: object
//@[017:018) NewLine |\n|
//}
//@[003:005) NewLine |\n\n|

type typeH = {
//@[000:004) Identifier |type|
//@[005:010) Identifier |typeH|
//@[011:012) Assignment |=|
//@[013:014) LeftBrace |{|
//@[014:015) NewLine |\n|
  type: 'h'
//@[002:006) Identifier |type|
//@[006:007) Colon |:|
//@[008:011) StringComplete |'h'|
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

type typeI = {
//@[000:004) Identifier |type|
//@[005:010) Identifier |typeI|
//@[011:012) Assignment |=|
//@[013:014) LeftBrace |{|
//@[014:015) NewLine |\n|
  type: 'i'
//@[002:006) Identifier |type|
//@[006:007) Colon |:|
//@[008:011) StringComplete |'i'|
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
type discriminatedUnion4 = discriminatedUnion1 | (discriminatedUnion2 | typeH)
//@[000:004) Identifier |type|
//@[005:024) Identifier |discriminatedUnion4|
//@[025:026) Assignment |=|
//@[027:046) Identifier |discriminatedUnion1|
//@[047:048) Pipe |||
//@[049:050) LeftParen |(|
//@[050:069) Identifier |discriminatedUnion2|
//@[070:071) Pipe |||
//@[072:077) Identifier |typeH|
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
type discriminatorMemberHasAdditionalProperties1 = typeA | typeI | { type: 'g', *: int }
//@[000:004) Identifier |type|
//@[005:048) Identifier |discriminatorMemberHasAdditionalProperties1|
//@[049:050) Assignment |=|
//@[051:056) Identifier |typeA|
//@[057:058) Pipe |||
//@[059:064) Identifier |typeI|
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
// END: valid tagged unions baselines; move this back to TypeDeclarations_LF when backend updates are released and uncomment typesA-D
//@[133:134) NewLine |\n|

//@[000:000) EndOfFile ||
