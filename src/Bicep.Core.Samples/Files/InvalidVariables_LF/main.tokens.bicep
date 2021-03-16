
//@[0:1) NewLine |\n|
// unknown declaration
//@[22:23) NewLine |\n|
bad
//@[0:3) Identifier |bad|
//@[3:5) NewLine |\n\n|

// incomplete variable declaration #completionTest(0,1,2) -> declarations
//@[73:74) NewLine |\n|
var
//@[0:3) Identifier |var|
//@[3:5) NewLine |\n\n|

// missing identifier #completionTest(4) -> empty
//@[49:50) NewLine |\n|
var 
//@[0:3) Identifier |var|
//@[4:6) NewLine |\n\n|

// incomplete keyword
//@[21:22) NewLine |\n|
// #completionTest(0,1) -> declarations
//@[39:40) NewLine |\n|
v
//@[0:1) Identifier |v|
//@[1:2) NewLine |\n|
// #completionTest(0,1,2) -> declarations
//@[41:42) NewLine |\n|
va
//@[0:2) Identifier |va|
//@[2:4) NewLine |\n\n|

// unassigned variable
//@[22:23) NewLine |\n|
var foo
//@[0:3) Identifier |var|
//@[4:7) Identifier |foo|
//@[7:9) NewLine |\n\n|

// #completionTest(18,19) -> symbols
//@[36:37) NewLine |\n|
var missingValue = 
//@[0:3) Identifier |var|
//@[4:16) Identifier |missingValue|
//@[17:18) Assignment |=|
//@[19:21) NewLine |\n\n|

// malformed identifier
//@[23:24) NewLine |\n|
var 2 
//@[0:3) Identifier |var|
//@[4:5) Integer |2|
//@[6:7) NewLine |\n|
var $ = 23
//@[0:3) Identifier |var|
//@[4:5) Unrecognized |$|
//@[6:7) Assignment |=|
//@[8:10) Integer |23|
//@[10:11) NewLine |\n|
var # 33 = 43
//@[0:3) Identifier |var|
//@[4:5) Unrecognized |#|
//@[6:8) Integer |33|
//@[9:10) Assignment |=|
//@[11:13) Integer |43|
//@[13:15) NewLine |\n\n|

// no value assigned
//@[20:21) NewLine |\n|
var foo =
//@[0:3) Identifier |var|
//@[4:7) Identifier |foo|
//@[8:9) Assignment |=|
//@[9:11) NewLine |\n\n|

// bad =
//@[8:9) NewLine |\n|
var badEquals 2
//@[0:3) Identifier |var|
//@[4:13) Identifier |badEquals|
//@[14:15) Integer |2|
//@[15:16) NewLine |\n|
var badEquals2 3 true
//@[0:3) Identifier |var|
//@[4:14) Identifier |badEquals2|
//@[15:16) Integer |3|
//@[17:21) TrueKeyword |true|
//@[21:23) NewLine |\n\n|

// malformed identifier but type check should happen regardless
//@[63:64) NewLine |\n|
var 2 = x
//@[0:3) Identifier |var|
//@[4:5) Integer |2|
//@[6:7) Assignment |=|
//@[8:9) Identifier |x|
//@[9:11) NewLine |\n\n|

// bad token value
//@[18:19) NewLine |\n|
var foo = &
//@[0:3) Identifier |var|
//@[4:7) Identifier |foo|
//@[8:9) Assignment |=|
//@[10:11) Unrecognized |&|
//@[11:13) NewLine |\n\n|

// bad value
//@[12:13) NewLine |\n|
var foo = *
//@[0:3) Identifier |var|
//@[4:7) Identifier |foo|
//@[8:9) Assignment |=|
//@[10:11) Asterisk |*|
//@[11:13) NewLine |\n\n|

// expressions
//@[14:15) NewLine |\n|
var bar = x
//@[0:3) Identifier |var|
//@[4:7) Identifier |bar|
//@[8:9) Assignment |=|
//@[10:11) Identifier |x|
//@[11:12) NewLine |\n|
var bar = foo()
//@[0:3) Identifier |var|
//@[4:7) Identifier |bar|
//@[8:9) Assignment |=|
//@[10:13) Identifier |foo|
//@[13:14) LeftParen |(|
//@[14:15) RightParen |)|
//@[15:16) NewLine |\n|
var x = 2 + !3
//@[0:3) Identifier |var|
//@[4:5) Identifier |x|
//@[6:7) Assignment |=|
//@[8:9) Integer |2|
//@[10:11) Plus |+|
//@[12:13) Exclamation |!|
//@[13:14) Integer |3|
//@[14:15) NewLine |\n|
var y = false ? true + 1 : !4
//@[0:3) Identifier |var|
//@[4:5) Identifier |y|
//@[6:7) Assignment |=|
//@[8:13) FalseKeyword |false|
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
//@[0:3) Identifier |var|
//@[4:5) Identifier |x|
//@[6:7) Assignment |=|
//@[8:9) LeftSquare |[|
//@[9:10) NewLine |\n|
  3 + 4
//@[2:3) Integer |3|
//@[4:5) Plus |+|
//@[6:7) Integer |4|
//@[7:8) NewLine |\n|
  =
//@[2:3) Assignment |=|
//@[3:4) NewLine |\n|
  !null
//@[2:3) Exclamation |!|
//@[3:7) NullKeyword |null|
//@[7:8) NewLine |\n|
]
//@[0:1) RightSquare |]|
//@[1:3) NewLine |\n\n|

// test for object property recovery
//@[36:37) NewLine |\n|
var y = {
//@[0:3) Identifier |var|
//@[4:5) Identifier |y|
//@[6:7) Assignment |=|
//@[8:9) LeftBrace |{|
//@[9:10) NewLine |\n|
  =
//@[2:3) Assignment |=|
//@[3:4) NewLine |\n|
  foo: !2
//@[2:5) Identifier |foo|
//@[5:6) Colon |:|
//@[7:8) Exclamation |!|
//@[8:9) Integer |2|
//@[9:10) NewLine |\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

// utcNow and newGuid used outside a param default value
//@[56:57) NewLine |\n|
var test = utcNow('u')
//@[0:3) Identifier |var|
//@[4:8) Identifier |test|
//@[9:10) Assignment |=|
//@[11:17) Identifier |utcNow|
//@[17:18) LeftParen |(|
//@[18:21) StringComplete |'u'|
//@[21:22) RightParen |)|
//@[22:23) NewLine |\n|
var test2 = newGuid()
//@[0:3) Identifier |var|
//@[4:9) Identifier |test2|
//@[10:11) Assignment |=|
//@[12:19) Identifier |newGuid|
//@[19:20) LeftParen |(|
//@[20:21) RightParen |)|
//@[21:23) NewLine |\n\n|

// bad string escape sequence in object key
//@[43:44) NewLine |\n|
var test3 = {
//@[0:3) Identifier |var|
//@[4:9) Identifier |test3|
//@[10:11) Assignment |=|
//@[12:13) LeftBrace |{|
//@[13:14) NewLine |\n|
  'bad\escape': true
//@[2:14) StringComplete |'bad\escape'|
//@[14:15) Colon |:|
//@[16:20) TrueKeyword |true|
//@[20:21) NewLine |\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

// duplicate properties
//@[23:24) NewLine |\n|
var testDupe = {
//@[0:3) Identifier |var|
//@[4:12) Identifier |testDupe|
//@[13:14) Assignment |=|
//@[15:16) LeftBrace |{|
//@[16:17) NewLine |\n|
  'duplicate': true
//@[2:13) StringComplete |'duplicate'|
//@[13:14) Colon |:|
//@[15:19) TrueKeyword |true|
//@[19:20) NewLine |\n|
  duplicate: true
//@[2:11) Identifier |duplicate|
//@[11:12) Colon |:|
//@[13:17) TrueKeyword |true|
//@[17:18) NewLine |\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

// interpolation with type errors in key
//@[40:41) NewLine |\n|
var objWithInterp = {
//@[0:3) Identifier |var|
//@[4:17) Identifier |objWithInterp|
//@[18:19) Assignment |=|
//@[20:21) LeftBrace |{|
//@[21:22) NewLine |\n|
  'ab${nonExistentIdentifier}cd': true
//@[2:7) StringLeftPiece |'ab${|
//@[7:28) Identifier |nonExistentIdentifier|
//@[28:32) StringRightPiece |}cd'|
//@[32:33) Colon |:|
//@[34:38) TrueKeyword |true|
//@[38:39) NewLine |\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

// invalid fully qualified function access
//@[42:43) NewLine |\n|
var mySum = az.add(1,2)
//@[0:3) Identifier |var|
//@[4:9) Identifier |mySum|
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
//@[0:3) Identifier |var|
//@[4:12) Identifier |myConcat|
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
//@[0:3) Identifier |var|
//@[4:16) Identifier |doubleString|
//@[17:18) Assignment |=|
//@[19:20) Unrecognized |"|
//@[20:23) Identifier |bad|
//@[24:30) Identifier |string|
//@[30:31) Unrecognized |"|
//@[31:33) NewLine |\n\n|

var resourceGroup = ''
//@[0:3) Identifier |var|
//@[4:17) Identifier |resourceGroup|
//@[18:19) Assignment |=|
//@[20:22) StringComplete |''|
//@[22:23) NewLine |\n|
var rgName = resourceGroup().name
//@[0:3) Identifier |var|
//@[4:10) Identifier |rgName|
//@[11:12) Assignment |=|
//@[13:26) Identifier |resourceGroup|
//@[26:27) LeftParen |(|
//@[27:28) RightParen |)|
//@[28:29) Dot |.|
//@[29:33) Identifier |name|
//@[33:35) NewLine |\n\n|

// this does not work at the resource group scope
//@[49:50) NewLine |\n|
var invalidLocationVar = deployment().location
//@[0:3) Identifier |var|
//@[4:22) Identifier |invalidLocationVar|
//@[23:24) Assignment |=|
//@[25:35) Identifier |deployment|
//@[35:36) LeftParen |(|
//@[36:37) RightParen |)|
//@[37:38) Dot |.|
//@[38:46) Identifier |location|
//@[46:48) NewLine |\n\n|

var invalidEnvironmentVar = environment().aosdufhsad
//@[0:3) Identifier |var|
//@[4:25) Identifier |invalidEnvironmentVar|
//@[26:27) Assignment |=|
//@[28:39) Identifier |environment|
//@[39:40) LeftParen |(|
//@[40:41) RightParen |)|
//@[41:42) Dot |.|
//@[42:52) Identifier |aosdufhsad|
//@[52:53) NewLine |\n|
var invalidEnvAuthVar = environment().authentication.asdgdsag
//@[0:3) Identifier |var|
//@[4:21) Identifier |invalidEnvAuthVar|
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
//@[0:3) Identifier |var|
//@[4:6) Identifier |az|
//@[7:8) Assignment |=|
//@[9:10) Integer |1|
//@[10:12) NewLine |\n\n|

// cannot assign a variable to a namespace
//@[42:43) NewLine |\n|
var invalidNamespaceAssignment = az
//@[0:3) Identifier |var|
//@[4:30) Identifier |invalidNamespaceAssignment|
//@[31:32) Assignment |=|
//@[33:35) Identifier |az|
//@[35:37) NewLine |\n\n|

var objectLiteralType = {
//@[0:3) Identifier |var|
//@[4:21) Identifier |objectLiteralType|
//@[22:23) Assignment |=|
//@[24:25) LeftBrace |{|
//@[25:26) NewLine |\n|
  first: true
//@[2:7) Identifier |first|
//@[7:8) Colon |:|
//@[9:13) TrueKeyword |true|
//@[13:14) NewLine |\n|
  second: false
//@[2:8) Identifier |second|
//@[8:9) Colon |:|
//@[10:15) FalseKeyword |false|
//@[15:16) NewLine |\n|
  third: 42
//@[2:7) Identifier |third|
//@[7:8) Colon |:|
//@[9:11) Integer |42|
//@[11:12) NewLine |\n|
  fourth: 'test'
//@[2:8) Identifier |fourth|
//@[8:9) Colon |:|
//@[10:16) StringComplete |'test'|
//@[16:17) NewLine |\n|
  fifth: [
//@[2:7) Identifier |fifth|
//@[7:8) Colon |:|
//@[9:10) LeftSquare |[|
//@[10:11) NewLine |\n|
    {
//@[4:5) LeftBrace |{|
//@[5:6) NewLine |\n|
      one: true
//@[6:9) Identifier |one|
//@[9:10) Colon |:|
//@[11:15) TrueKeyword |true|
//@[15:16) NewLine |\n|
    }
//@[4:5) RightBrace |}|
//@[5:6) NewLine |\n|
    {
//@[4:5) LeftBrace |{|
//@[5:6) NewLine |\n|
      one: false
//@[6:9) Identifier |one|
//@[9:10) Colon |:|
//@[11:16) FalseKeyword |false|
//@[16:17) NewLine |\n|
    }
//@[4:5) RightBrace |}|
//@[5:6) NewLine |\n|
  ]
//@[2:3) RightSquare |]|
//@[3:4) NewLine |\n|
  sixth: [
//@[2:7) Identifier |sixth|
//@[7:8) Colon |:|
//@[9:10) LeftSquare |[|
//@[10:11) NewLine |\n|
    {
//@[4:5) LeftBrace |{|
//@[5:6) NewLine |\n|
      two: 44
//@[6:9) Identifier |two|
//@[9:10) Colon |:|
//@[11:13) Integer |44|
//@[13:14) NewLine |\n|
    }
//@[4:5) RightBrace |}|
//@[5:6) NewLine |\n|
  ]
//@[2:3) RightSquare |]|
//@[3:4) NewLine |\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

// #completionTest(54) -> objectVarTopLevel
//@[43:44) NewLine |\n|
var objectVarTopLevelCompletions = objectLiteralType.f
//@[0:3) Identifier |var|
//@[4:32) Identifier |objectVarTopLevelCompletions|
//@[33:34) Assignment |=|
//@[35:52) Identifier |objectLiteralType|
//@[52:53) Dot |.|
//@[53:54) Identifier |f|
//@[54:55) NewLine |\n|
// #completionTest(54) -> objectVarTopLevel
//@[43:44) NewLine |\n|
var objectVarTopLevelCompletions2 = objectLiteralType.
//@[0:3) Identifier |var|
//@[4:33) Identifier |objectVarTopLevelCompletions2|
//@[34:35) Assignment |=|
//@[36:53) Identifier |objectLiteralType|
//@[53:54) Dot |.|
//@[54:56) NewLine |\n\n|

// this does not produce any completions because mixed array items are of type "any"
//@[84:85) NewLine |\n|
// #completionTest(60) -> mixedArrayProperties
//@[46:47) NewLine |\n|
var mixedArrayTypeCompletions = objectLiteralType.fifth[0].o
//@[0:3) Identifier |var|
//@[4:29) Identifier |mixedArrayTypeCompletions|
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
//@[0:3) Identifier |var|
//@[4:30) Identifier |mixedArrayTypeCompletions2|
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
//@[0:3) Identifier |var|
//@[4:27) Identifier |oneArrayItemCompletions|
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
//@[0:3) Identifier |var|
//@[4:28) Identifier |oneArrayItemCompletions2|
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
//@[0:3) Identifier |var|
//@[4:42) Identifier |objectVarTopLevelArrayIndexCompletions|
//@[43:44) Assignment |=|
//@[45:62) Identifier |objectLiteralType|
//@[62:63) LeftSquare |[|
//@[63:64) Identifier |f|
//@[64:65) RightSquare |]|
//@[65:67) NewLine |\n\n|

// #completionTest(58) -> twoIndexPlusSymbols
//@[45:46) NewLine |\n|
var oneArrayIndexCompletions = objectLiteralType.sixth[0][]
//@[0:3) Identifier |var|
//@[4:28) Identifier |oneArrayIndexCompletions|
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
//@[0:3) Identifier |var|
//@[4:11) Identifier |myFloat|
//@[12:13) Assignment |=|
//@[14:15) Integer |3|
//@[15:16) Dot |.|
//@[16:18) Integer |14|
//@[18:20) NewLine |\n\n|

// secure cannot be used as a varaible decorator
//@[48:49) NewLine |\n|
@sys.secure()
//@[0:1) At |@|
//@[1:4) Identifier |sys|
//@[4:5) Dot |.|
//@[5:11) Identifier |secure|
//@[11:12) LeftParen |(|
//@[12:13) RightParen |)|
//@[13:14) NewLine |\n|
var something = 1
//@[0:3) Identifier |var|
//@[4:13) Identifier |something|
//@[14:15) Assignment |=|
//@[16:17) Integer |1|
//@[17:19) NewLine |\n\n|

// #completionTest(1) -> empty
//@[30:31) NewLine |\n|
@
//@[0:1) At |@|
//@[1:2) NewLine |\n|
// #completionTest(5) -> empty
//@[30:31) NewLine |\n|
@sys.
//@[0:1) At |@|
//@[1:4) Identifier |sys|
//@[4:5) Dot |.|
//@[5:6) NewLine |\n|
var anotherThing = true
//@[0:3) Identifier |var|
//@[4:16) Identifier |anotherThing|
//@[17:18) Assignment |=|
//@[19:23) TrueKeyword |true|
//@[23:25) NewLine |\n\n|

// invalid identifier character classes
//@[39:40) NewLine |\n|
var ☕ = true
//@[0:3) Identifier |var|
//@[4:5) Unrecognized |☕|
//@[6:7) Assignment |=|
//@[8:12) TrueKeyword |true|
//@[12:13) NewLine |\n|
var a☕ = true
//@[0:3) Identifier |var|
//@[4:5) Identifier |a|
//@[5:6) Unrecognized |☕|
//@[7:8) Assignment |=|
//@[9:13) TrueKeyword |true|
//@[13:15) NewLine |\n\n|

// loops are not allowed in variables
//@[37:38) NewLine |\n|
var noVariableLoopsYet = [for thing in stuff: 4]
//@[0:3) Identifier |var|
//@[4:22) Identifier |noVariableLoopsYet|
//@[23:24) Assignment |=|
//@[25:26) LeftSquare |[|
//@[26:29) Identifier |for|
//@[30:35) Identifier |thing|
//@[36:38) Identifier |in|
//@[39:44) Identifier |stuff|
//@[44:45) Colon |:|
//@[46:47) Integer |4|
//@[47:48) RightSquare |]|
//@[48:50) NewLine |\n\n|

// loops with conditions won't even parse
//@[41:42) NewLine |\n|
var noFilteredLoopsInVariables = [for thing in stuff: if]
//@[0:3) Identifier |var|
//@[4:30) Identifier |noFilteredLoopsInVariables|
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
//@[0:3) Identifier |var|
//@[4:31) Identifier |noNestedVariableLoopsEither|
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
//@[2:7) Identifier |hello|
//@[7:8) Colon |:|
//@[9:10) LeftSquare |[|
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
//@[0:1) RightBrace |}|
//@[1:2) RightSquare |]|
//@[2:4) NewLine |\n\n|

// loops in inner properties of a variable are also not supported
//@[65:66) NewLine |\n|
var innerPropertyLoop = {
//@[0:3) Identifier |var|
//@[4:21) Identifier |innerPropertyLoop|
//@[22:23) Assignment |=|
//@[24:25) LeftBrace |{|
//@[25:26) NewLine |\n|
  a: [for i in range(0,10): i]
//@[2:3) Identifier |a|
//@[3:4) Colon |:|
//@[5:6) LeftSquare |[|
//@[6:9) Identifier |for|
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
//@[0:1) RightBrace |}|
//@[1:2) NewLine |\n|
var innerPropertyLoop2 = {
//@[0:3) Identifier |var|
//@[4:22) Identifier |innerPropertyLoop2|
//@[23:24) Assignment |=|
//@[25:26) LeftBrace |{|
//@[26:27) NewLine |\n|
  b: {
//@[2:3) Identifier |b|
//@[3:4) Colon |:|
//@[5:6) LeftBrace |{|
//@[6:7) NewLine |\n|
    a: [for i in range(0,10): i]
//@[4:5) Identifier |a|
//@[5:6) Colon |:|
//@[7:8) LeftSquare |[|
//@[8:11) Identifier |for|
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
//@[2:3) RightBrace |}|
//@[3:4) NewLine |\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

// cannot use loops in expressions
//@[34:35) NewLine |\n|
var loopExpression = union([for thing in stuff: 4], [for thing in stuff: true])
//@[0:3) Identifier |var|
//@[4:18) Identifier |loopExpression|
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
//@[0:1) At |@|
//@[1:10) Identifier |batchSize|
//@[10:11) LeftParen |(|
//@[11:12) Integer |1|
//@[12:13) RightParen |)|
//@[13:14) NewLine |\n|
var batchSizeMakesNoSenseHere = false
//@[0:3) Identifier |var|
//@[4:29) Identifier |batchSizeMakesNoSenseHere|
//@[30:31) Assignment |=|
//@[32:37) FalseKeyword |false|
//@[37:38) NewLine |\n|

//@[0:0) EndOfFile ||
