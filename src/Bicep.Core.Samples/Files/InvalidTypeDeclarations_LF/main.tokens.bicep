type 44
//@[00:04) Identifier |type|
//@[05:07) Integer |44|
//@[07:09) NewLine |\n\n|

type noAssignment
//@[00:04) Identifier |type|
//@[05:17) Identifier |noAssignment|
//@[17:19) NewLine |\n\n|

type incompleteAssignment =
//@[00:04) Identifier |type|
//@[05:25) Identifier |incompleteAssignment|
//@[26:27) Assignment |=|
//@[27:29) NewLine |\n\n|

type resource = bool
//@[00:04) Identifier |type|
//@[05:13) Identifier |resource|
//@[14:15) Assignment |=|
//@[16:20) Identifier |bool|
//@[20:22) NewLine |\n\n|

@sealed()
//@[00:01) At |@|
//@[01:07) Identifier |sealed|
//@[07:08) LeftParen |(|
//@[08:09) RightParen |)|
//@[09:10) NewLine |\n|
type sealedString = string
//@[00:04) Identifier |type|
//@[05:17) Identifier |sealedString|
//@[18:19) Assignment |=|
//@[20:26) Identifier |string|
//@[26:28) NewLine |\n\n|

@sealed()
//@[00:01) At |@|
//@[01:07) Identifier |sealed|
//@[07:08) LeftParen |(|
//@[08:09) RightParen |)|
//@[09:10) NewLine |\n|
type sealedDictionary = {
//@[00:04) Identifier |type|
//@[05:21) Identifier |sealedDictionary|
//@[22:23) Assignment |=|
//@[24:25) LeftBrace |{|
//@[25:26) NewLine |\n|
	*: string
//@[01:02) Asterisk |*|
//@[02:03) Colon |:|
//@[04:10) Identifier |string|
//@[10:11) NewLine |\n|
}
//@[00:01) RightBrace |}|
//@[01:03) NewLine |\n\n|

type disallowedUnion = 'foo'|21
//@[00:04) Identifier |type|
//@[05:20) Identifier |disallowedUnion|
//@[21:22) Assignment |=|
//@[23:28) StringComplete |'foo'|
//@[28:29) Pipe |||
//@[29:31) Integer |21|
//@[31:33) NewLine |\n\n|

type validStringLiteralUnion = 'foo'|'bar'|'baz'
//@[00:04) Identifier |type|
//@[05:28) Identifier |validStringLiteralUnion|
//@[29:30) Assignment |=|
//@[31:36) StringComplete |'foo'|
//@[36:37) Pipe |||
//@[37:42) StringComplete |'bar'|
//@[42:43) Pipe |||
//@[43:48) StringComplete |'baz'|
//@[48:50) NewLine |\n\n|

type validUnionInvalidAddition = validStringLiteralUnion|10
//@[00:04) Identifier |type|
//@[05:30) Identifier |validUnionInvalidAddition|
//@[31:32) Assignment |=|
//@[33:56) Identifier |validStringLiteralUnion|
//@[56:57) Pipe |||
//@[57:59) Integer |10|
//@[59:61) NewLine |\n\n|

type invalidUnionInvalidAddition = disallowedUnion|true
//@[00:04) Identifier |type|
//@[05:32) Identifier |invalidUnionInvalidAddition|
//@[33:34) Assignment |=|
//@[35:50) Identifier |disallowedUnion|
//@[50:51) Pipe |||
//@[51:55) TrueKeyword |true|
//@[55:57) NewLine |\n\n|

type nullLiteral = null
//@[00:04) Identifier |type|
//@[05:16) Identifier |nullLiteral|
//@[17:18) Assignment |=|
//@[19:23) NullKeyword |null|
//@[23:25) NewLine |\n\n|

type unionOfNulls = null|null
//@[00:04) Identifier |type|
//@[05:17) Identifier |unionOfNulls|
//@[18:19) Assignment |=|
//@[20:24) NullKeyword |null|
//@[24:25) Pipe |||
//@[25:29) NullKeyword |null|
//@[29:31) NewLine |\n\n|

@minLength(3)
//@[00:01) At |@|
//@[01:10) Identifier |minLength|
//@[10:11) LeftParen |(|
//@[11:12) Integer |3|
//@[12:13) RightParen |)|
//@[13:14) NewLine |\n|
type lengthConstrainedInt = int
//@[00:04) Identifier |type|
//@[05:25) Identifier |lengthConstrainedInt|
//@[26:27) Assignment |=|
//@[28:31) Identifier |int|
//@[31:33) NewLine |\n\n|

@minValue(3)
//@[00:01) At |@|
//@[01:09) Identifier |minValue|
//@[09:10) LeftParen |(|
//@[10:11) Integer |3|
//@[11:12) RightParen |)|
//@[12:13) NewLine |\n|
type valueConstrainedString = string
//@[00:04) Identifier |type|
//@[05:27) Identifier |valueConstrainedString|
//@[28:29) Assignment |=|
//@[30:36) Identifier |string|
//@[36:38) NewLine |\n\n|

type tautology = tautology
//@[00:04) Identifier |type|
//@[05:14) Identifier |tautology|
//@[15:16) Assignment |=|
//@[17:26) Identifier |tautology|
//@[26:28) NewLine |\n\n|

type tautologicalUnion = tautologicalUnion|'foo'
//@[00:04) Identifier |type|
//@[05:22) Identifier |tautologicalUnion|
//@[23:24) Assignment |=|
//@[25:42) Identifier |tautologicalUnion|
//@[42:43) Pipe |||
//@[43:48) StringComplete |'foo'|
//@[48:50) NewLine |\n\n|

type tautologicalArray = tautologicalArray[]
//@[00:04) Identifier |type|
//@[05:22) Identifier |tautologicalArray|
//@[23:24) Assignment |=|
//@[25:42) Identifier |tautologicalArray|
//@[42:43) LeftSquare |[|
//@[43:44) RightSquare |]|
//@[44:46) NewLine |\n\n|

type directCycleStart = directCycleReturn
//@[00:04) Identifier |type|
//@[05:21) Identifier |directCycleStart|
//@[22:23) Assignment |=|
//@[24:41) Identifier |directCycleReturn|
//@[41:43) NewLine |\n\n|

type directCycleReturn = directCycleStart
//@[00:04) Identifier |type|
//@[05:22) Identifier |directCycleReturn|
//@[23:24) Assignment |=|
//@[25:41) Identifier |directCycleStart|
//@[41:43) NewLine |\n\n|

type cycleRoot = connector
//@[00:04) Identifier |type|
//@[05:14) Identifier |cycleRoot|
//@[15:16) Assignment |=|
//@[17:26) Identifier |connector|
//@[26:28) NewLine |\n\n|

type connector = cycleBack
//@[00:04) Identifier |type|
//@[05:14) Identifier |connector|
//@[15:16) Assignment |=|
//@[17:26) Identifier |cycleBack|
//@[26:28) NewLine |\n\n|

type cycleBack = cycleRoot
//@[00:04) Identifier |type|
//@[05:14) Identifier |cycleBack|
//@[15:16) Assignment |=|
//@[17:26) Identifier |cycleRoot|
//@[26:28) NewLine |\n\n|

type objectWithInvalidPropertyDecorators = {
//@[00:04) Identifier |type|
//@[05:40) Identifier |objectWithInvalidPropertyDecorators|
//@[41:42) Assignment |=|
//@[43:44) LeftBrace |{|
//@[44:45) NewLine |\n|
  @sealed()
//@[02:03) At |@|
//@[03:09) Identifier |sealed|
//@[09:10) LeftParen |(|
//@[10:11) RightParen |)|
//@[11:12) NewLine |\n|
  fooProp: string
//@[02:09) Identifier |fooProp|
//@[09:10) Colon |:|
//@[11:17) Identifier |string|
//@[17:19) NewLine |\n\n|

  @secure()
//@[02:03) At |@|
//@[03:09) Identifier |secure|
//@[09:10) LeftParen |(|
//@[10:11) RightParen |)|
//@[11:12) NewLine |\n|
  barProp: string
//@[02:09) Identifier |barProp|
//@[09:10) Colon |:|
//@[11:17) Identifier |string|
//@[17:19) NewLine |\n\n|

  @allowed(['snap', 'crackle', 'pop'])
//@[02:03) At |@|
//@[03:10) Identifier |allowed|
//@[10:11) LeftParen |(|
//@[11:12) LeftSquare |[|
//@[12:18) StringComplete |'snap'|
//@[18:19) Comma |,|
//@[20:29) StringComplete |'crackle'|
//@[29:30) Comma |,|
//@[31:36) StringComplete |'pop'|
//@[36:37) RightSquare |]|
//@[37:38) RightParen |)|
//@[38:39) NewLine |\n|
  krispyProp: string
//@[02:12) Identifier |krispyProp|
//@[12:13) Colon |:|
//@[14:20) Identifier |string|
//@[20:21) NewLine |\n|
}
//@[00:01) RightBrace |}|
//@[01:03) NewLine |\n\n|

type objectWithInvalidRecursion = {
//@[00:04) Identifier |type|
//@[05:31) Identifier |objectWithInvalidRecursion|
//@[32:33) Assignment |=|
//@[34:35) LeftBrace |{|
//@[35:36) NewLine |\n|
  requiredAndRecursiveProp: objectWithInvalidRecursion
//@[02:26) Identifier |requiredAndRecursiveProp|
//@[26:27) Colon |:|
//@[28:54) Identifier |objectWithInvalidRecursion|
//@[54:55) NewLine |\n|
}
//@[00:01) RightBrace |}|
//@[01:03) NewLine |\n\n|

type arrayWithInvalidMember = objectWithInvalidRecursion[]
//@[00:04) Identifier |type|
//@[05:27) Identifier |arrayWithInvalidMember|
//@[28:29) Assignment |=|
//@[30:56) Identifier |objectWithInvalidRecursion|
//@[56:57) LeftSquare |[|
//@[57:58) RightSquare |]|
//@[58:60) NewLine |\n\n|

@sealed()
//@[00:01) At |@|
//@[01:07) Identifier |sealed|
//@[07:08) LeftParen |(|
//@[08:09) RightParen |)|
//@[09:10) NewLine |\n|
param sealedStringParam string
//@[00:05) Identifier |param|
//@[06:23) Identifier |sealedStringParam|
//@[24:30) Identifier |string|
//@[30:32) NewLine |\n\n|

param disallowedUnionParam 'foo'|-99
//@[00:05) Identifier |param|
//@[06:26) Identifier |disallowedUnionParam|
//@[27:32) StringComplete |'foo'|
//@[32:33) Pipe |||
//@[33:34) Minus |-|
//@[34:36) Integer |99|
//@[36:38) NewLine |\n\n|

param objectWithInvalidRecursionParam objectWithInvalidRecursion
//@[00:05) Identifier |param|
//@[06:37) Identifier |objectWithInvalidRecursionParam|
//@[38:64) Identifier |objectWithInvalidRecursion|
//@[64:65) NewLine |\n|

//@[00:00) EndOfFile ||
