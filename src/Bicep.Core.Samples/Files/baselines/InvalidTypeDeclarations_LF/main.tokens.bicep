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
//@[061:062) NewLine |\n|

//@[000:000) EndOfFile ||
