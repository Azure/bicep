
//@[00:01) NewLine |\n|
// unknown declaration
//@[22:23) NewLine |\n|
bad
//@[00:03) Identifier |bad|
//@[03:05) NewLine |\n\n|

// incomplete variable declaration #completionTest(0,1,2) -> declarations
//@[73:74) NewLine |\n|
var
//@[00:03) Identifier |var|
//@[03:05) NewLine |\n\n|

// missing identifier #completionTest(4) -> empty
//@[49:50) NewLine |\n|
var 
//@[00:03) Identifier |var|
//@[04:06) NewLine |\n\n|

// incomplete keyword
//@[21:22) NewLine |\n|
// #completionTest(0,1) -> declarations
//@[39:40) NewLine |\n|
v
//@[00:01) Identifier |v|
//@[01:02) NewLine |\n|
// #completionTest(0,1,2) -> declarations
//@[41:42) NewLine |\n|
va
//@[00:02) Identifier |va|
//@[02:04) NewLine |\n\n|

// unassigned variable
//@[22:23) NewLine |\n|
var foo
//@[00:03) Identifier |var|
//@[04:07) Identifier |foo|
//@[07:09) NewLine |\n\n|

// #completionTest(18,19) -> symbols
//@[36:37) NewLine |\n|
var missingValue = 
//@[00:03) Identifier |var|
//@[04:16) Identifier |missingValue|
//@[17:18) Assignment |=|
//@[19:21) NewLine |\n\n|

// malformed identifier
//@[23:24) NewLine |\n|
var 2 
//@[00:03) Identifier |var|
//@[04:05) Integer |2|
//@[06:07) NewLine |\n|
var $ = 23
//@[00:03) Identifier |var|
//@[04:05) Unrecognized |$|
//@[06:07) Assignment |=|
//@[08:10) Integer |23|
//@[10:11) NewLine |\n|
var # 33 = 43
//@[00:03) Identifier |var|
//@[04:05) Unrecognized |#|
//@[06:08) Integer |33|
//@[09:10) Assignment |=|
//@[11:13) Integer |43|
//@[13:15) NewLine |\n\n|

// no value assigned
//@[20:21) NewLine |\n|
var foo =
//@[00:03) Identifier |var|
//@[04:07) Identifier |foo|
//@[08:09) Assignment |=|
//@[09:11) NewLine |\n\n|

// bad =
//@[08:09) NewLine |\n|
var badEquals 2
//@[00:03) Identifier |var|
//@[04:13) Identifier |badEquals|
//@[14:15) Integer |2|
//@[15:16) NewLine |\n|
var badEquals2 3 true
//@[00:03) Identifier |var|
//@[04:14) Identifier |badEquals2|
//@[15:16) Integer |3|
//@[17:21) TrueKeyword |true|
//@[21:23) NewLine |\n\n|

// malformed identifier but type check should happen regardless
//@[63:64) NewLine |\n|
var 2 = x
//@[00:03) Identifier |var|
//@[04:05) Integer |2|
//@[06:07) Assignment |=|
//@[08:09) Identifier |x|
//@[09:11) NewLine |\n\n|

// bad token value
//@[18:19) NewLine |\n|
var foo = &
//@[00:03) Identifier |var|
//@[04:07) Identifier |foo|
//@[08:09) Assignment |=|
//@[10:11) Unrecognized |&|
//@[11:13) NewLine |\n\n|

// bad value
//@[12:13) NewLine |\n|
var foo = *
//@[00:03) Identifier |var|
//@[04:07) Identifier |foo|
//@[08:09) Assignment |=|
//@[10:11) Asterisk |*|
//@[11:13) NewLine |\n\n|

// expressions
//@[14:15) NewLine |\n|
var bar = x
//@[00:03) Identifier |var|
//@[04:07) Identifier |bar|
//@[08:09) Assignment |=|
//@[10:11) Identifier |x|
//@[11:12) NewLine |\n|
var bar = foo()
//@[00:03) Identifier |var|
//@[04:07) Identifier |bar|
//@[08:09) Assignment |=|
//@[10:13) Identifier |foo|
//@[13:14) LeftParen |(|
//@[14:15) RightParen |)|
//@[15:16) NewLine |\n|
var x = 2 + !3
//@[00:03) Identifier |var|
//@[04:05) Identifier |x|
//@[06:07) Assignment |=|
//@[08:09) Integer |2|
//@[10:11) Plus |+|
//@[12:13) Exclamation |!|
//@[13:14) Integer |3|
//@[14:15) NewLine |\n|
var y = false ? true + 1 : !4
//@[00:03) Identifier |var|
//@[04:05) Identifier |y|
//@[06:07) Assignment |=|
//@[08:13) FalseKeyword |false|
//@[14:15) Question |?|
//@[16:20) TrueKeyword |true|
//@[21:22) Plus |+|
//@[23:24) Integer |1|
//@[25:26) Colon |:|
//@[27:28) Exclamation |!|
//@[28:29) Integer |4|
//@[29:31) NewLine |\n\n|

// test for array item recovery
//@[31:32) NewLine |\n|
var x = [
//@[00:03) Identifier |var|
//@[04:05) Identifier |x|
//@[06:07) Assignment |=|
//@[08:09) LeftSquare |[|
//@[09:10) NewLine |\n|
  3 + 4
//@[02:03) Integer |3|
//@[04:05) Plus |+|
//@[06:07) Integer |4|
//@[07:08) NewLine |\n|
  =
//@[02:03) Assignment |=|
//@[03:04) NewLine |\n|
  !null
//@[02:03) Exclamation |!|
//@[03:07) NullKeyword |null|
//@[07:08) NewLine |\n|
]
//@[00:01) RightSquare |]|
//@[01:03) NewLine |\n\n|

// test for object property recovery
//@[36:37) NewLine |\n|
var y = {
//@[00:03) Identifier |var|
//@[04:05) Identifier |y|
//@[06:07) Assignment |=|
//@[08:09) LeftBrace |{|
//@[09:10) NewLine |\n|
  =
//@[02:03) Assignment |=|
//@[03:04) NewLine |\n|
  foo: !2
//@[02:05) Identifier |foo|
//@[05:06) Colon |:|
//@[07:08) Exclamation |!|
//@[08:09) Integer |2|
//@[09:10) NewLine |\n|
}
//@[00:01) RightBrace |}|
//@[01:03) NewLine |\n\n|

// utcNow and newGuid used outside a param default value
//@[56:57) NewLine |\n|
var test = utcNow('u')
//@[00:03) Identifier |var|
//@[04:08) Identifier |test|
//@[09:10) Assignment |=|
//@[11:17) Identifier |utcNow|
//@[17:18) LeftParen |(|
//@[18:21) StringComplete |'u'|
//@[21:22) RightParen |)|
//@[22:23) NewLine |\n|
var test2 = newGuid()
//@[00:03) Identifier |var|
//@[04:09) Identifier |test2|
//@[10:11) Assignment |=|
//@[12:19) Identifier |newGuid|
//@[19:20) LeftParen |(|
//@[20:21) RightParen |)|
//@[21:23) NewLine |\n\n|

// bad string escape sequence in object key
//@[43:44) NewLine |\n|
var test3 = {
//@[00:03) Identifier |var|
//@[04:09) Identifier |test3|
//@[10:11) Assignment |=|
//@[12:13) LeftBrace |{|
//@[13:14) NewLine |\n|
  'bad\escape': true
//@[02:14) StringComplete |'bad\escape'|
//@[14:15) Colon |:|
//@[16:20) TrueKeyword |true|
//@[20:21) NewLine |\n|
}
//@[00:01) RightBrace |}|
//@[01:03) NewLine |\n\n|

// duplicate properties
//@[23:24) NewLine |\n|
var testDupe = {
//@[00:03) Identifier |var|
//@[04:12) Identifier |testDupe|
//@[13:14) Assignment |=|
//@[15:16) LeftBrace |{|
//@[16:17) NewLine |\n|
  'duplicate': true
//@[02:13) StringComplete |'duplicate'|
//@[13:14) Colon |:|
//@[15:19) TrueKeyword |true|
//@[19:20) NewLine |\n|
  duplicate: true
//@[02:11) Identifier |duplicate|
//@[11:12) Colon |:|
//@[13:17) TrueKeyword |true|
//@[17:18) NewLine |\n|
}
//@[00:01) RightBrace |}|
//@[01:03) NewLine |\n\n|

// interpolation with type errors in key
//@[40:41) NewLine |\n|
var objWithInterp = {
//@[00:03) Identifier |var|
//@[04:17) Identifier |objWithInterp|
//@[18:19) Assignment |=|
//@[20:21) LeftBrace |{|
//@[21:22) NewLine |\n|
  'ab${nonExistentIdentifier}cd': true
//@[02:07) StringLeftPiece |'ab${|
//@[07:28) Identifier |nonExistentIdentifier|
//@[28:32) StringRightPiece |}cd'|
//@[32:33) Colon |:|
//@[34:38) TrueKeyword |true|
//@[38:39) NewLine |\n|
}
//@[00:01) RightBrace |}|
//@[01:03) NewLine |\n\n|

// invalid fully qualified function access
//@[42:43) NewLine |\n|
var mySum = az.add(1,2)
//@[00:03) Identifier |var|
//@[04:09) Identifier |mySum|
//@[10:11) Assignment |=|
//@[12:14) Identifier |az|
//@[14:15) Dot |.|
//@[15:18) Identifier |add|
//@[18:19) LeftParen |(|
//@[19:20) Integer |1|
//@[20:21) Comma |,|
//@[21:22) Integer |2|
//@[22:23) RightParen |)|
//@[23:24) NewLine |\n|
var myConcat = sys.concat('a', az.concat('b', 'c'))
//@[00:03) Identifier |var|
//@[04:12) Identifier |myConcat|
//@[13:14) Assignment |=|
//@[15:18) Identifier |sys|
//@[18:19) Dot |.|
//@[19:25) Identifier |concat|
//@[25:26) LeftParen |(|
//@[26:29) StringComplete |'a'|
//@[29:30) Comma |,|
//@[31:33) Identifier |az|
//@[33:34) Dot |.|
//@[34:40) Identifier |concat|
//@[40:41) LeftParen |(|
//@[41:44) StringComplete |'b'|
//@[44:45) Comma |,|
//@[46:49) StringComplete |'c'|
//@[49:50) RightParen |)|
//@[50:51) RightParen |)|
//@[51:53) NewLine |\n\n|

// invalid string using double quotes
//@[37:38) NewLine |\n|
var doubleString = "bad string"
//@[00:03) Identifier |var|
//@[04:16) Identifier |doubleString|
//@[17:18) Assignment |=|
//@[19:20) Unrecognized |"|
//@[20:23) Identifier |bad|
//@[24:30) Identifier |string|
//@[30:31) Unrecognized |"|
//@[31:33) NewLine |\n\n|

var resourceGroup = ''
//@[00:03) Identifier |var|
//@[04:17) Identifier |resourceGroup|
//@[18:19) Assignment |=|
//@[20:22) StringComplete |''|
//@[22:23) NewLine |\n|
var rgName = resourceGroup().name
//@[00:03) Identifier |var|
//@[04:10) Identifier |rgName|
//@[11:12) Assignment |=|
//@[13:26) Identifier |resourceGroup|
//@[26:27) LeftParen |(|
//@[27:28) RightParen |)|
//@[28:29) Dot |.|
//@[29:33) Identifier |name|
//@[33:35) NewLine |\n\n|

var subscription = ''
//@[00:03) Identifier |var|
//@[04:16) Identifier |subscription|
//@[17:18) Assignment |=|
//@[19:21) StringComplete |''|
//@[21:22) NewLine |\n|
var subName = subscription().name
//@[00:03) Identifier |var|
//@[04:11) Identifier |subName|
//@[12:13) Assignment |=|
//@[14:26) Identifier |subscription|
//@[26:27) LeftParen |(|
//@[27:28) RightParen |)|
//@[28:29) Dot |.|
//@[29:33) Identifier |name|
//@[33:35) NewLine |\n\n|

// this does not work at the resource group scope
//@[49:50) NewLine |\n|
var invalidLocationVar = deployment().location
//@[00:03) Identifier |var|
//@[04:22) Identifier |invalidLocationVar|
//@[23:24) Assignment |=|
//@[25:35) Identifier |deployment|
//@[35:36) LeftParen |(|
//@[36:37) RightParen |)|
//@[37:38) Dot |.|
//@[38:46) Identifier |location|
//@[46:48) NewLine |\n\n|

var invalidEnvironmentVar = environment().aosdufhsad
//@[00:03) Identifier |var|
//@[04:25) Identifier |invalidEnvironmentVar|
//@[26:27) Assignment |=|
//@[28:39) Identifier |environment|
//@[39:40) LeftParen |(|
//@[40:41) RightParen |)|
//@[41:42) Dot |.|
//@[42:52) Identifier |aosdufhsad|
//@[52:53) NewLine |\n|
var invalidEnvAuthVar = environment().authentication.asdgdsag
//@[00:03) Identifier |var|
//@[04:21) Identifier |invalidEnvAuthVar|
//@[22:23) Assignment |=|
//@[24:35) Identifier |environment|
//@[35:36) LeftParen |(|
//@[36:37) RightParen |)|
//@[37:38) Dot |.|
//@[38:52) Identifier |authentication|
//@[52:53) Dot |.|
//@[53:61) Identifier |asdgdsag|
//@[61:63) NewLine |\n\n|

// invalid use of reserved namespace
//@[36:37) NewLine |\n|
var az = 1
//@[00:03) Identifier |var|
//@[04:06) Identifier |az|
//@[07:08) Assignment |=|
//@[09:10) Integer |1|
//@[10:12) NewLine |\n\n|

// cannot assign a variable to a namespace
//@[42:43) NewLine |\n|
var invalidNamespaceAssignment = az
//@[00:03) Identifier |var|
//@[04:30) Identifier |invalidNamespaceAssignment|
//@[31:32) Assignment |=|
//@[33:35) Identifier |az|
//@[35:37) NewLine |\n\n|

var objectLiteralType = {
//@[00:03) Identifier |var|
//@[04:21) Identifier |objectLiteralType|
//@[22:23) Assignment |=|
//@[24:25) LeftBrace |{|
//@[25:26) NewLine |\n|
  first: true
//@[02:07) Identifier |first|
//@[07:08) Colon |:|
//@[09:13) TrueKeyword |true|
//@[13:14) NewLine |\n|
  second: false
//@[02:08) Identifier |second|
//@[08:09) Colon |:|
//@[10:15) FalseKeyword |false|
//@[15:16) NewLine |\n|
  third: 42
//@[02:07) Identifier |third|
//@[07:08) Colon |:|
//@[09:11) Integer |42|
//@[11:12) NewLine |\n|
  fourth: 'test'
//@[02:08) Identifier |fourth|
//@[08:09) Colon |:|
//@[10:16) StringComplete |'test'|
//@[16:17) NewLine |\n|
  fifth: [
//@[02:07) Identifier |fifth|
//@[07:08) Colon |:|
//@[09:10) LeftSquare |[|
//@[10:11) NewLine |\n|
    {
//@[04:05) LeftBrace |{|
//@[05:06) NewLine |\n|
      one: true
//@[06:09) Identifier |one|
//@[09:10) Colon |:|
//@[11:15) TrueKeyword |true|
//@[15:16) NewLine |\n|
    }
//@[04:05) RightBrace |}|
//@[05:06) NewLine |\n|
    {
//@[04:05) LeftBrace |{|
//@[05:06) NewLine |\n|
      one: false
//@[06:09) Identifier |one|
//@[09:10) Colon |:|
//@[11:16) FalseKeyword |false|
//@[16:17) NewLine |\n|
    }
//@[04:05) RightBrace |}|
//@[05:06) NewLine |\n|
  ]
//@[02:03) RightSquare |]|
//@[03:04) NewLine |\n|
  sixth: [
//@[02:07) Identifier |sixth|
//@[07:08) Colon |:|
//@[09:10) LeftSquare |[|
//@[10:11) NewLine |\n|
    {
//@[04:05) LeftBrace |{|
//@[05:06) NewLine |\n|
      two: 44
//@[06:09) Identifier |two|
//@[09:10) Colon |:|
//@[11:13) Integer |44|
//@[13:14) NewLine |\n|
    }
//@[04:05) RightBrace |}|
//@[05:06) NewLine |\n|
  ]
//@[02:03) RightSquare |]|
//@[03:04) NewLine |\n|
}
//@[00:01) RightBrace |}|
//@[01:03) NewLine |\n\n|

// #completionTest(54) -> objectVarTopLevel
//@[43:44) NewLine |\n|
var objectVarTopLevelCompletions = objectLiteralType.f
//@[00:03) Identifier |var|
//@[04:32) Identifier |objectVarTopLevelCompletions|
//@[33:34) Assignment |=|
//@[35:52) Identifier |objectLiteralType|
//@[52:53) Dot |.|
//@[53:54) Identifier |f|
//@[54:55) NewLine |\n|
// #completionTest(54) -> objectVarTopLevel
//@[43:44) NewLine |\n|
var objectVarTopLevelCompletions2 = objectLiteralType.
//@[00:03) Identifier |var|
//@[04:33) Identifier |objectVarTopLevelCompletions2|
//@[34:35) Assignment |=|
//@[36:53) Identifier |objectLiteralType|
//@[53:54) Dot |.|
//@[54:56) NewLine |\n\n|

// this does not produce any completions because mixed array items are of type "any"
//@[84:85) NewLine |\n|
// #completionTest(60) -> mixedArrayProperties
//@[46:47) NewLine |\n|
var mixedArrayTypeCompletions = objectLiteralType.fifth[0].o
//@[00:03) Identifier |var|
//@[04:29) Identifier |mixedArrayTypeCompletions|
//@[30:31) Assignment |=|
//@[32:49) Identifier |objectLiteralType|
//@[49:50) Dot |.|
//@[50:55) Identifier |fifth|
//@[55:56) LeftSquare |[|
//@[56:57) Integer |0|
//@[57:58) RightSquare |]|
//@[58:59) Dot |.|
//@[59:60) Identifier |o|
//@[60:61) NewLine |\n|
// #completionTest(60) -> mixedArrayProperties
//@[46:47) NewLine |\n|
var mixedArrayTypeCompletions2 = objectLiteralType.fifth[0].
//@[00:03) Identifier |var|
//@[04:30) Identifier |mixedArrayTypeCompletions2|
//@[31:32) Assignment |=|
//@[33:50) Identifier |objectLiteralType|
//@[50:51) Dot |.|
//@[51:56) Identifier |fifth|
//@[56:57) LeftSquare |[|
//@[57:58) Integer |0|
//@[58:59) RightSquare |]|
//@[59:60) Dot |.|
//@[60:62) NewLine |\n\n|

// #completionTest(58) -> oneArrayItemProperties
//@[48:49) NewLine |\n|
var oneArrayItemCompletions = objectLiteralType.sixth[0].t
//@[00:03) Identifier |var|
//@[04:27) Identifier |oneArrayItemCompletions|
//@[28:29) Assignment |=|
//@[30:47) Identifier |objectLiteralType|
//@[47:48) Dot |.|
//@[48:53) Identifier |sixth|
//@[53:54) LeftSquare |[|
//@[54:55) Integer |0|
//@[55:56) RightSquare |]|
//@[56:57) Dot |.|
//@[57:58) Identifier |t|
//@[58:59) NewLine |\n|
// #completionTest(58) -> oneArrayItemProperties
//@[48:49) NewLine |\n|
var oneArrayItemCompletions2 = objectLiteralType.sixth[0].
//@[00:03) Identifier |var|
//@[04:28) Identifier |oneArrayItemCompletions2|
//@[29:30) Assignment |=|
//@[31:48) Identifier |objectLiteralType|
//@[48:49) Dot |.|
//@[49:54) Identifier |sixth|
//@[54:55) LeftSquare |[|
//@[55:56) Integer |0|
//@[56:57) RightSquare |]|
//@[57:58) Dot |.|
//@[58:60) NewLine |\n\n|

// #completionTest(65) -> objectVarTopLevelIndexes
//@[50:51) NewLine |\n|
var objectVarTopLevelArrayIndexCompletions = objectLiteralType[f]
//@[00:03) Identifier |var|
//@[04:42) Identifier |objectVarTopLevelArrayIndexCompletions|
//@[43:44) Assignment |=|
//@[45:62) Identifier |objectLiteralType|
//@[62:63) LeftSquare |[|
//@[63:64) Identifier |f|
//@[64:65) RightSquare |]|
//@[65:67) NewLine |\n\n|

// #completionTest(58) -> twoIndexPlusSymbols
//@[45:46) NewLine |\n|
var oneArrayIndexCompletions = objectLiteralType.sixth[0][]
//@[00:03) Identifier |var|
//@[04:28) Identifier |oneArrayIndexCompletions|
//@[29:30) Assignment |=|
//@[31:48) Identifier |objectLiteralType|
//@[48:49) Dot |.|
//@[49:54) Identifier |sixth|
//@[54:55) LeftSquare |[|
//@[55:56) Integer |0|
//@[56:57) RightSquare |]|
//@[57:58) LeftSquare |[|
//@[58:59) RightSquare |]|
//@[59:61) NewLine |\n\n|

// Issue 486
//@[12:13) NewLine |\n|
var myFloat = 3.14
//@[00:03) Identifier |var|
//@[04:11) Identifier |myFloat|
//@[12:13) Assignment |=|
//@[14:15) Integer |3|
//@[15:16) Dot |.|
//@[16:18) Integer |14|
//@[18:20) NewLine |\n\n|

// secure cannot be used as a variable decorator
//@[48:49) NewLine |\n|
@sys.secure()
//@[00:01) At |@|
//@[01:04) Identifier |sys|
//@[04:05) Dot |.|
//@[05:11) Identifier |secure|
//@[11:12) LeftParen |(|
//@[12:13) RightParen |)|
//@[13:14) NewLine |\n|
var something = 1
//@[00:03) Identifier |var|
//@[04:13) Identifier |something|
//@[14:15) Assignment |=|
//@[16:17) Integer |1|
//@[17:19) NewLine |\n\n|

// #completionTest(1) -> sysAndDescription
//@[42:43) NewLine |\n|
@
//@[00:01) At |@|
//@[01:02) NewLine |\n|
// #completionTest(5) -> description
//@[36:37) NewLine |\n|
@sys.
//@[00:01) At |@|
//@[01:04) Identifier |sys|
//@[04:05) Dot |.|
//@[05:06) NewLine |\n|
var anotherThing = true
//@[00:03) Identifier |var|
//@[04:16) Identifier |anotherThing|
//@[17:18) Assignment |=|
//@[19:23) TrueKeyword |true|
//@[23:25) NewLine |\n\n|

// invalid identifier character classes
//@[39:40) NewLine |\n|
var ☕ = true
//@[00:03) Identifier |var|
//@[04:05) Unrecognized |☕|
//@[06:07) Assignment |=|
//@[08:12) TrueKeyword |true|
//@[12:13) NewLine |\n|
var a☕ = true
//@[00:03) Identifier |var|
//@[04:05) Identifier |a|
//@[05:06) Unrecognized |☕|
//@[07:08) Assignment |=|
//@[09:13) TrueKeyword |true|
//@[13:15) NewLine |\n\n|

var missingArrayVariable = [for thing in stuff: 4]
//@[00:03) Identifier |var|
//@[04:24) Identifier |missingArrayVariable|
//@[25:26) Assignment |=|
//@[27:28) LeftSquare |[|
//@[28:31) Identifier |for|
//@[32:37) Identifier |thing|
//@[38:40) Identifier |in|
//@[41:46) Identifier |stuff|
//@[46:47) Colon |:|
//@[48:49) Integer |4|
//@[49:50) RightSquare |]|
//@[50:52) NewLine |\n\n|

// loops are only allowed at the top level
//@[42:43) NewLine |\n|
var nonTopLevelLoop = {
//@[00:03) Identifier |var|
//@[04:19) Identifier |nonTopLevelLoop|
//@[20:21) Assignment |=|
//@[22:23) LeftBrace |{|
//@[23:24) NewLine |\n|
  notOkHere: [for thing in stuff: 4]
//@[02:11) Identifier |notOkHere|
//@[11:12) Colon |:|
//@[13:14) LeftSquare |[|
//@[14:17) Identifier |for|
//@[18:23) Identifier |thing|
//@[24:26) Identifier |in|
//@[27:32) Identifier |stuff|
//@[32:33) Colon |:|
//@[34:35) Integer |4|
//@[35:36) RightSquare |]|
//@[36:37) NewLine |\n|
}
//@[00:01) RightBrace |}|
//@[01:03) NewLine |\n\n|

// loops with conditions won't even parse
//@[41:42) NewLine |\n|
var noFilteredLoopsInVariables = [for thing in stuff: if]
//@[00:03) Identifier |var|
//@[04:30) Identifier |noFilteredLoopsInVariables|
//@[31:32) Assignment |=|
//@[33:34) LeftSquare |[|
//@[34:37) Identifier |for|
//@[38:43) Identifier |thing|
//@[44:46) Identifier |in|
//@[47:52) Identifier |stuff|
//@[52:53) Colon |:|
//@[54:56) Identifier |if|
//@[56:57) RightSquare |]|
//@[57:59) NewLine |\n\n|

// nested loops are also not allowed
//@[36:37) NewLine |\n|
var noNestedVariableLoopsEither = [for thing in stuff: {
//@[00:03) Identifier |var|
//@[04:31) Identifier |noNestedVariableLoopsEither|
//@[32:33) Assignment |=|
//@[34:35) LeftSquare |[|
//@[35:38) Identifier |for|
//@[39:44) Identifier |thing|
//@[45:47) Identifier |in|
//@[48:53) Identifier |stuff|
//@[53:54) Colon |:|
//@[55:56) LeftBrace |{|
//@[56:57) NewLine |\n|
  hello: [for thing in []: 4]
//@[02:07) Identifier |hello|
//@[07:08) Colon |:|
//@[09:10) LeftSquare |[|
//@[10:13) Identifier |for|
//@[14:19) Identifier |thing|
//@[20:22) Identifier |in|
//@[23:24) LeftSquare |[|
//@[24:25) RightSquare |]|
//@[25:26) Colon |:|
//@[27:28) Integer |4|
//@[28:29) RightSquare |]|
//@[29:30) NewLine |\n|
}]
//@[00:01) RightBrace |}|
//@[01:02) RightSquare |]|
//@[02:04) NewLine |\n\n|

// loops in inner properties of a variable are also not supported
//@[65:66) NewLine |\n|
var innerPropertyLoop = {
//@[00:03) Identifier |var|
//@[04:21) Identifier |innerPropertyLoop|
//@[22:23) Assignment |=|
//@[24:25) LeftBrace |{|
//@[25:26) NewLine |\n|
  a: [for i in range(0,10): i]
//@[02:03) Identifier |a|
//@[03:04) Colon |:|
//@[05:06) LeftSquare |[|
//@[06:09) Identifier |for|
//@[10:11) Identifier |i|
//@[12:14) Identifier |in|
//@[15:20) Identifier |range|
//@[20:21) LeftParen |(|
//@[21:22) Integer |0|
//@[22:23) Comma |,|
//@[23:25) Integer |10|
//@[25:26) RightParen |)|
//@[26:27) Colon |:|
//@[28:29) Identifier |i|
//@[29:30) RightSquare |]|
//@[30:31) NewLine |\n|
}
//@[00:01) RightBrace |}|
//@[01:02) NewLine |\n|
var innerPropertyLoop2 = {
//@[00:03) Identifier |var|
//@[04:22) Identifier |innerPropertyLoop2|
//@[23:24) Assignment |=|
//@[25:26) LeftBrace |{|
//@[26:27) NewLine |\n|
  b: {
//@[02:03) Identifier |b|
//@[03:04) Colon |:|
//@[05:06) LeftBrace |{|
//@[06:07) NewLine |\n|
    a: [for i in range(0,10): i]
//@[04:05) Identifier |a|
//@[05:06) Colon |:|
//@[07:08) LeftSquare |[|
//@[08:11) Identifier |for|
//@[12:13) Identifier |i|
//@[14:16) Identifier |in|
//@[17:22) Identifier |range|
//@[22:23) LeftParen |(|
//@[23:24) Integer |0|
//@[24:25) Comma |,|
//@[25:27) Integer |10|
//@[27:28) RightParen |)|
//@[28:29) Colon |:|
//@[30:31) Identifier |i|
//@[31:32) RightSquare |]|
//@[32:33) NewLine |\n|
  }
//@[02:03) RightBrace |}|
//@[03:04) NewLine |\n|
}
//@[00:01) RightBrace |}|
//@[01:03) NewLine |\n\n|

// loops using expressions with a runtime dependency are also not allowed
//@[73:74) NewLine |\n|
var keys = listKeys('fake','fake')
//@[00:03) Identifier |var|
//@[04:08) Identifier |keys|
//@[09:10) Assignment |=|
//@[11:19) Identifier |listKeys|
//@[19:20) LeftParen |(|
//@[20:26) StringComplete |'fake'|
//@[26:27) Comma |,|
//@[27:33) StringComplete |'fake'|
//@[33:34) RightParen |)|
//@[34:35) NewLine |\n|
var indirection = keys
//@[00:03) Identifier |var|
//@[04:15) Identifier |indirection|
//@[16:17) Assignment |=|
//@[18:22) Identifier |keys|
//@[22:24) NewLine |\n\n|

var runtimeLoop = [for (item, index) in []: indirection]
//@[00:03) Identifier |var|
//@[04:15) Identifier |runtimeLoop|
//@[16:17) Assignment |=|
//@[18:19) LeftSquare |[|
//@[19:22) Identifier |for|
//@[23:24) LeftParen |(|
//@[24:28) Identifier |item|
//@[28:29) Comma |,|
//@[30:35) Identifier |index|
//@[35:36) RightParen |)|
//@[37:39) Identifier |in|
//@[40:41) LeftSquare |[|
//@[41:42) RightSquare |]|
//@[42:43) Colon |:|
//@[44:55) Identifier |indirection|
//@[55:56) RightSquare |]|
//@[56:57) NewLine |\n|
var runtimeLoop2 = [for (item, index) in indirection.keys: 's']
//@[00:03) Identifier |var|
//@[04:16) Identifier |runtimeLoop2|
//@[17:18) Assignment |=|
//@[19:20) LeftSquare |[|
//@[20:23) Identifier |for|
//@[24:25) LeftParen |(|
//@[25:29) Identifier |item|
//@[29:30) Comma |,|
//@[31:36) Identifier |index|
//@[36:37) RightParen |)|
//@[38:40) Identifier |in|
//@[41:52) Identifier |indirection|
//@[52:53) Dot |.|
//@[53:57) Identifier |keys|
//@[57:58) Colon |:|
//@[59:62) StringComplete |'s'|
//@[62:63) RightSquare |]|
//@[63:65) NewLine |\n\n|

var zoneInput = []
//@[00:03) Identifier |var|
//@[04:13) Identifier |zoneInput|
//@[14:15) Assignment |=|
//@[16:17) LeftSquare |[|
//@[17:18) RightSquare |]|
//@[18:19) NewLine |\n|
resource zones 'Microsoft.Network/dnsZones@2018-05-01' = [for (zone, i) in zoneInput: {
//@[00:08) Identifier |resource|
//@[09:14) Identifier |zones|
//@[15:54) StringComplete |'Microsoft.Network/dnsZones@2018-05-01'|
//@[55:56) Assignment |=|
//@[57:58) LeftSquare |[|
//@[58:61) Identifier |for|
//@[62:63) LeftParen |(|
//@[63:67) Identifier |zone|
//@[67:68) Comma |,|
//@[69:70) Identifier |i|
//@[70:71) RightParen |)|
//@[72:74) Identifier |in|
//@[75:84) Identifier |zoneInput|
//@[84:85) Colon |:|
//@[86:87) LeftBrace |{|
//@[87:88) NewLine |\n|
  name: zone
//@[02:06) Identifier |name|
//@[06:07) Colon |:|
//@[08:12) Identifier |zone|
//@[12:13) NewLine |\n|
  location: az.resourceGroup().location
//@[02:10) Identifier |location|
//@[10:11) Colon |:|
//@[12:14) Identifier |az|
//@[14:15) Dot |.|
//@[15:28) Identifier |resourceGroup|
//@[28:29) LeftParen |(|
//@[29:30) RightParen |)|
//@[30:31) Dot |.|
//@[31:39) Identifier |location|
//@[39:40) NewLine |\n|
}]
//@[00:01) RightBrace |}|
//@[01:02) RightSquare |]|
//@[02:03) NewLine |\n|
var inlinedVariable = zones[0].properties.zoneType
//@[00:03) Identifier |var|
//@[04:19) Identifier |inlinedVariable|
//@[20:21) Assignment |=|
//@[22:27) Identifier |zones|
//@[27:28) LeftSquare |[|
//@[28:29) Integer |0|
//@[29:30) RightSquare |]|
//@[30:31) Dot |.|
//@[31:41) Identifier |properties|
//@[41:42) Dot |.|
//@[42:50) Identifier |zoneType|
//@[50:52) NewLine |\n\n|

var runtimeLoop3 = [for (zone, i) in zoneInput: {
//@[00:03) Identifier |var|
//@[04:16) Identifier |runtimeLoop3|
//@[17:18) Assignment |=|
//@[19:20) LeftSquare |[|
//@[20:23) Identifier |for|
//@[24:25) LeftParen |(|
//@[25:29) Identifier |zone|
//@[29:30) Comma |,|
//@[31:32) Identifier |i|
//@[32:33) RightParen |)|
//@[34:36) Identifier |in|
//@[37:46) Identifier |zoneInput|
//@[46:47) Colon |:|
//@[48:49) LeftBrace |{|
//@[49:50) NewLine |\n|
  a: inlinedVariable
//@[02:03) Identifier |a|
//@[03:04) Colon |:|
//@[05:20) Identifier |inlinedVariable|
//@[20:21) NewLine |\n|
}]
//@[00:01) RightBrace |}|
//@[01:02) RightSquare |]|
//@[02:04) NewLine |\n\n|

var runtimeLoop4 = [for (zone, i) in zones[0].properties.registrationVirtualNetworks: {
//@[00:03) Identifier |var|
//@[04:16) Identifier |runtimeLoop4|
//@[17:18) Assignment |=|
//@[19:20) LeftSquare |[|
//@[20:23) Identifier |for|
//@[24:25) LeftParen |(|
//@[25:29) Identifier |zone|
//@[29:30) Comma |,|
//@[31:32) Identifier |i|
//@[32:33) RightParen |)|
//@[34:36) Identifier |in|
//@[37:42) Identifier |zones|
//@[42:43) LeftSquare |[|
//@[43:44) Integer |0|
//@[44:45) RightSquare |]|
//@[45:46) Dot |.|
//@[46:56) Identifier |properties|
//@[56:57) Dot |.|
//@[57:84) Identifier |registrationVirtualNetworks|
//@[84:85) Colon |:|
//@[86:87) LeftBrace |{|
//@[87:88) NewLine |\n|
  a: 0
//@[02:03) Identifier |a|
//@[03:04) Colon |:|
//@[05:06) Integer |0|
//@[06:07) NewLine |\n|
}]
//@[00:01) RightBrace |}|
//@[01:02) RightSquare |]|
//@[02:04) NewLine |\n\n|

var notRuntime = concat('a','b')
//@[00:03) Identifier |var|
//@[04:14) Identifier |notRuntime|
//@[15:16) Assignment |=|
//@[17:23) Identifier |concat|
//@[23:24) LeftParen |(|
//@[24:27) StringComplete |'a'|
//@[27:28) Comma |,|
//@[28:31) StringComplete |'b'|
//@[31:32) RightParen |)|
//@[32:33) NewLine |\n|
var evenMoreIndirection = concat(notRuntime, string(moreIndirection))
//@[00:03) Identifier |var|
//@[04:23) Identifier |evenMoreIndirection|
//@[24:25) Assignment |=|
//@[26:32) Identifier |concat|
//@[32:33) LeftParen |(|
//@[33:43) Identifier |notRuntime|
//@[43:44) Comma |,|
//@[45:51) Identifier |string|
//@[51:52) LeftParen |(|
//@[52:67) Identifier |moreIndirection|
//@[67:68) RightParen |)|
//@[68:69) RightParen |)|
//@[69:70) NewLine |\n|
var moreIndirection = reference('s','s', 'Full')
//@[00:03) Identifier |var|
//@[04:19) Identifier |moreIndirection|
//@[20:21) Assignment |=|
//@[22:31) Identifier |reference|
//@[31:32) LeftParen |(|
//@[32:35) StringComplete |'s'|
//@[35:36) Comma |,|
//@[36:39) StringComplete |'s'|
//@[39:40) Comma |,|
//@[41:47) StringComplete |'Full'|
//@[47:48) RightParen |)|
//@[48:50) NewLine |\n\n|

var myRef = [
//@[00:03) Identifier |var|
//@[04:09) Identifier |myRef|
//@[10:11) Assignment |=|
//@[12:13) LeftSquare |[|
//@[13:14) NewLine |\n|
  evenMoreIndirection
//@[02:21) Identifier |evenMoreIndirection|
//@[21:22) NewLine |\n|
]
//@[00:01) RightSquare |]|
//@[01:02) NewLine |\n|
var runtimeLoop5 = [for (item, index) in myRef: 's']
//@[00:03) Identifier |var|
//@[04:16) Identifier |runtimeLoop5|
//@[17:18) Assignment |=|
//@[19:20) LeftSquare |[|
//@[20:23) Identifier |for|
//@[24:25) LeftParen |(|
//@[25:29) Identifier |item|
//@[29:30) Comma |,|
//@[31:36) Identifier |index|
//@[36:37) RightParen |)|
//@[38:40) Identifier |in|
//@[41:46) Identifier |myRef|
//@[46:47) Colon |:|
//@[48:51) StringComplete |'s'|
//@[51:52) RightSquare |]|
//@[52:54) NewLine |\n\n|

// cannot use loops in expressions
//@[34:35) NewLine |\n|
var loopExpression = union([for thing in stuff: 4], [for thing in stuff: true])
//@[00:03) Identifier |var|
//@[04:18) Identifier |loopExpression|
//@[19:20) Assignment |=|
//@[21:26) Identifier |union|
//@[26:27) LeftParen |(|
//@[27:28) LeftSquare |[|
//@[28:31) Identifier |for|
//@[32:37) Identifier |thing|
//@[38:40) Identifier |in|
//@[41:46) Identifier |stuff|
//@[46:47) Colon |:|
//@[48:49) Integer |4|
//@[49:50) RightSquare |]|
//@[50:51) Comma |,|
//@[52:53) LeftSquare |[|
//@[53:56) Identifier |for|
//@[57:62) Identifier |thing|
//@[63:65) Identifier |in|
//@[66:71) Identifier |stuff|
//@[71:72) Colon |:|
//@[73:77) TrueKeyword |true|
//@[77:78) RightSquare |]|
//@[78:79) RightParen |)|
//@[79:81) NewLine |\n\n|

@batchSize(1)
//@[00:01) At |@|
//@[01:10) Identifier |batchSize|
//@[10:11) LeftParen |(|
//@[11:12) Integer |1|
//@[12:13) RightParen |)|
//@[13:14) NewLine |\n|
var batchSizeMakesNoSenseHere = false
//@[00:03) Identifier |var|
//@[04:29) Identifier |batchSizeMakesNoSenseHere|
//@[30:31) Assignment |=|
//@[32:37) FalseKeyword |false|
//@[37:40) NewLine |\n\n\n|


//KeyVault Secret Reference
//@[27:28) NewLine |\n|
resource kv 'Microsoft.KeyVault/vaults@2019-09-01' existing = {
//@[00:08) Identifier |resource|
//@[09:11) Identifier |kv|
//@[12:50) StringComplete |'Microsoft.KeyVault/vaults@2019-09-01'|
//@[51:59) Identifier |existing|
//@[60:61) Assignment |=|
//@[62:63) LeftBrace |{|
//@[63:64) NewLine |\n|
  name: 'testkeyvault'
//@[02:06) Identifier |name|
//@[06:07) Colon |:|
//@[08:22) StringComplete |'testkeyvault'|
//@[22:23) NewLine |\n|
}
//@[00:01) RightBrace |}|
//@[01:03) NewLine |\n\n|

var keyVaultSecretVar = kv.getSecret('mySecret')
//@[00:03) Identifier |var|
//@[04:21) Identifier |keyVaultSecretVar|
//@[22:23) Assignment |=|
//@[24:26) Identifier |kv|
//@[26:27) Dot |.|
//@[27:36) Identifier |getSecret|
//@[36:37) LeftParen |(|
//@[37:47) StringComplete |'mySecret'|
//@[47:48) RightParen |)|
//@[48:49) NewLine |\n|
var keyVaultSecretInterpolatedVar = '${kv.getSecret('mySecret')}'
//@[00:03) Identifier |var|
//@[04:33) Identifier |keyVaultSecretInterpolatedVar|
//@[34:35) Assignment |=|
//@[36:39) StringLeftPiece |'${|
//@[39:41) Identifier |kv|
//@[41:42) Dot |.|
//@[42:51) Identifier |getSecret|
//@[51:52) LeftParen |(|
//@[52:62) StringComplete |'mySecret'|
//@[62:63) RightParen |)|
//@[63:65) StringRightPiece |}'|
//@[65:66) NewLine |\n|
var keyVaultSecretObjectVar = {
//@[00:03) Identifier |var|
//@[04:27) Identifier |keyVaultSecretObjectVar|
//@[28:29) Assignment |=|
//@[30:31) LeftBrace |{|
//@[31:32) NewLine |\n|
  secret: kv.getSecret('mySecret')
//@[02:08) Identifier |secret|
//@[08:09) Colon |:|
//@[10:12) Identifier |kv|
//@[12:13) Dot |.|
//@[13:22) Identifier |getSecret|
//@[22:23) LeftParen |(|
//@[23:33) StringComplete |'mySecret'|
//@[33:34) RightParen |)|
//@[34:35) NewLine |\n|
}
//@[00:01) RightBrace |}|
//@[01:02) NewLine |\n|
var keyVaultSecretArrayVar = [
//@[00:03) Identifier |var|
//@[04:26) Identifier |keyVaultSecretArrayVar|
//@[27:28) Assignment |=|
//@[29:30) LeftSquare |[|
//@[30:31) NewLine |\n|
  kv.getSecret('mySecret')
//@[02:04) Identifier |kv|
//@[04:05) Dot |.|
//@[05:14) Identifier |getSecret|
//@[14:15) LeftParen |(|
//@[15:25) StringComplete |'mySecret'|
//@[25:26) RightParen |)|
//@[26:27) NewLine |\n|
]
//@[00:01) RightSquare |]|
//@[01:02) NewLine |\n|
var keyVaultSecretArrayInterpolatedVar = [
//@[00:03) Identifier |var|
//@[04:38) Identifier |keyVaultSecretArrayInterpolatedVar|
//@[39:40) Assignment |=|
//@[41:42) LeftSquare |[|
//@[42:43) NewLine |\n|
  '${kv.getSecret('mySecret')}'
//@[02:05) StringLeftPiece |'${|
//@[05:07) Identifier |kv|
//@[07:08) Dot |.|
//@[08:17) Identifier |getSecret|
//@[17:18) LeftParen |(|
//@[18:28) StringComplete |'mySecret'|
//@[28:29) RightParen |)|
//@[29:31) StringRightPiece |}'|
//@[31:32) NewLine |\n|
]
//@[00:01) RightSquare |]|
//@[01:03) NewLine |\n\n|

var listSecrets= ''
//@[00:03) Identifier |var|
//@[04:15) Identifier |listSecrets|
//@[15:16) Assignment |=|
//@[17:19) StringComplete |''|
//@[19:20) NewLine |\n|
var listSecretsVar = listSecrets()
//@[00:03) Identifier |var|
//@[04:18) Identifier |listSecretsVar|
//@[19:20) Assignment |=|
//@[21:32) Identifier |listSecrets|
//@[32:33) LeftParen |(|
//@[33:34) RightParen |)|
//@[34:36) NewLine |\n\n|

var copy = [
//@[00:03) Identifier |var|
//@[04:08) Identifier |copy|
//@[09:10) Assignment |=|
//@[11:12) LeftSquare |[|
//@[12:13) NewLine |\n|
  {
//@[02:03) LeftBrace |{|
//@[03:04) NewLine |\n|
    name: 'one'
//@[04:08) Identifier |name|
//@[08:09) Colon |:|
//@[10:15) StringComplete |'one'|
//@[15:16) NewLine |\n|
    count: '[notAFunction()]'
//@[04:09) Identifier |count|
//@[09:10) Colon |:|
//@[11:29) StringComplete |'[notAFunction()]'|
//@[29:30) NewLine |\n|
    input: {}
//@[04:09) Identifier |input|
//@[09:10) Colon |:|
//@[11:12) LeftBrace |{|
//@[12:13) RightBrace |}|
//@[13:14) NewLine |\n|
  }
//@[02:03) RightBrace |}|
//@[03:04) NewLine |\n|
]
//@[00:01) RightSquare |]|
//@[01:02) NewLine |\n|

//@[00:00) EndOfFile ||
