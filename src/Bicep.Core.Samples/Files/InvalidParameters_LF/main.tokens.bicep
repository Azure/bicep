/* 
  Valid and invalid code is mixed together to validate recovery logic. It can even contain ** * *** **.
*/
//@[2:4) NewLine |\n\n|

param myString string
//@[0:5) Identifier |param|
//@[6:14) Identifier |myString|
//@[15:21) Identifier |string|
//@[21:22) NewLine |\n|
wrong
//@[0:5) Identifier |wrong|
//@[5:7) NewLine |\n\n|

param myInt int
//@[0:5) Identifier |param|
//@[6:11) Identifier |myInt|
//@[12:15) Identifier |int|
//@[15:16) NewLine |\n|
param
//@[0:5) Identifier |param|
//@[5:7) NewLine |\n\n|

param 3
//@[0:5) Identifier |param|
//@[6:7) Integer |3|
//@[7:8) NewLine |\n|
param % string
//@[0:5) Identifier |param|
//@[6:7) Modulo |%|
//@[8:14) Identifier |string|
//@[14:15) NewLine |\n|
param % string 3 = 's'
//@[0:5) Identifier |param|
//@[6:7) Modulo |%|
//@[8:14) Identifier |string|
//@[15:16) Integer |3|
//@[17:18) Assignment |=|
//@[19:22) StringComplete |'s'|
//@[22:24) NewLine |\n\n|

param myBool bool
//@[0:5) Identifier |param|
//@[6:12) Identifier |myBool|
//@[13:17) Identifier |bool|
//@[17:19) NewLine |\n\n|

param missingType
//@[0:5) Identifier |param|
//@[6:17) Identifier |missingType|
//@[17:19) NewLine |\n\n|

// space after identifier #completionTest(32) -> paramTypes
//@[59:60) NewLine |\n|
param missingTypeWithSpaceAfter 
//@[0:5) Identifier |param|
//@[6:31) Identifier |missingTypeWithSpaceAfter|
//@[32:34) NewLine |\n\n|

// tab after identifier #completionTest(30) -> paramTypes
//@[57:58) NewLine |\n|
param missingTypeWithTabAfter	
//@[0:5) Identifier |param|
//@[6:29) Identifier |missingTypeWithTabAfter|
//@[30:32) NewLine |\n\n|

// #completionTest(20) -> paramTypes
//@[36:37) NewLine |\n|
param trailingSpace  
//@[0:5) Identifier |param|
//@[6:19) Identifier |trailingSpace|
//@[21:23) NewLine |\n\n|

// partial type #completionTest(18, 19, 20, 21) -> paramTypes
//@[61:62) NewLine |\n|
param partialType str
//@[0:5) Identifier |param|
//@[6:17) Identifier |partialType|
//@[18:21) Identifier |str|
//@[21:23) NewLine |\n\n|

param malformedType 44
//@[0:5) Identifier |param|
//@[6:19) Identifier |malformedType|
//@[20:22) Integer |44|
//@[22:24) NewLine |\n\n|

// malformed type but type check should still happen
//@[52:53) NewLine |\n|
param malformedType2 44 = f
//@[0:5) Identifier |param|
//@[6:20) Identifier |malformedType2|
//@[21:23) Integer |44|
//@[24:25) Assignment |=|
//@[26:27) Identifier |f|
//@[27:29) NewLine |\n\n|

// malformed type but type check should still happen
//@[52:53) NewLine |\n|
@secure('s')
//@[0:1) At |@|
//@[1:7) Identifier |secure|
//@[7:8) LeftParen |(|
//@[8:11) StringComplete |'s'|
//@[11:12) RightParen |)|
//@[12:13) NewLine |\n|
param malformedModifier 44
//@[0:5) Identifier |param|
//@[6:23) Identifier |malformedModifier|
//@[24:26) Integer |44|
//@[26:28) NewLine |\n\n|

param myString2 string = 'string value'
//@[0:5) Identifier |param|
//@[6:15) Identifier |myString2|
//@[16:22) Identifier |string|
//@[23:24) Assignment |=|
//@[25:39) StringComplete |'string value'|
//@[39:41) NewLine |\n\n|

param wrongDefaultValue string = 42
//@[0:5) Identifier |param|
//@[6:23) Identifier |wrongDefaultValue|
//@[24:30) Identifier |string|
//@[31:32) Assignment |=|
//@[33:35) Integer |42|
//@[35:37) NewLine |\n\n|

param myInt2 int = 42
//@[0:5) Identifier |param|
//@[6:12) Identifier |myInt2|
//@[13:16) Identifier |int|
//@[17:18) Assignment |=|
//@[19:21) Integer |42|
//@[21:22) NewLine |\n|
param noValueAfterColon int =   
//@[0:5) Identifier |param|
//@[6:23) Identifier |noValueAfterColon|
//@[24:27) Identifier |int|
//@[28:29) Assignment |=|
//@[32:34) NewLine |\n\n|

param myTruth bool = 'not a boolean'
//@[0:5) Identifier |param|
//@[6:13) Identifier |myTruth|
//@[14:18) Identifier |bool|
//@[19:20) Assignment |=|
//@[21:36) StringComplete |'not a boolean'|
//@[36:37) NewLine |\n|
param myFalsehood bool = 'false'
//@[0:5) Identifier |param|
//@[6:17) Identifier |myFalsehood|
//@[18:22) Identifier |bool|
//@[23:24) Assignment |=|
//@[25:32) StringComplete |'false'|
//@[32:34) NewLine |\n\n|

param wrongAssignmentToken string: 'hello'
//@[0:5) Identifier |param|
//@[6:26) Identifier |wrongAssignmentToken|
//@[27:33) Identifier |string|
//@[33:34) Colon |:|
//@[35:42) StringComplete |'hello'|
//@[42:44) NewLine |\n\n|

param WhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLong string = 'why not?'
//@[0:5) Identifier |param|
//@[6:267) Identifier |WhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLong|
//@[268:274) Identifier |string|
//@[275:276) Assignment |=|
//@[277:287) StringComplete |'why not?'|
//@[287:289) NewLine |\n\n|

// #completionTest(28,29) -> boolPlusSymbols
//@[44:45) NewLine |\n|
param boolCompletions bool = 
//@[0:5) Identifier |param|
//@[6:21) Identifier |boolCompletions|
//@[22:26) Identifier |bool|
//@[27:28) Assignment |=|
//@[29:31) NewLine |\n\n|

// #completionTest(30,31) -> arrayPlusSymbols
//@[45:46) NewLine |\n|
param arrayCompletions array = 
//@[0:5) Identifier |param|
//@[6:22) Identifier |arrayCompletions|
//@[23:28) Identifier |array|
//@[29:30) Assignment |=|
//@[31:33) NewLine |\n\n|

// #completionTest(32,33) -> objectPlusSymbols
//@[46:47) NewLine |\n|
param objectCompletions object = 
//@[0:5) Identifier |param|
//@[6:23) Identifier |objectCompletions|
//@[24:30) Identifier |object|
//@[31:32) Assignment |=|
//@[33:35) NewLine |\n\n|

// badly escaped string
//@[23:24) NewLine |\n|
param wrongType fluffyBunny = 'what's up doc?'
//@[0:5) Identifier |param|
//@[6:15) Identifier |wrongType|
//@[16:27) Identifier |fluffyBunny|
//@[28:29) Assignment |=|
//@[30:36) StringComplete |'what'|
//@[36:37) Identifier |s|
//@[38:40) Identifier |up|
//@[41:44) Identifier |doc|
//@[44:45) Question |?|
//@[45:46) StringComplete |'|
//@[46:48) NewLine |\n\n|

// invalid escape
//@[17:18) NewLine |\n|
param wrongType fluffyBunny = 'what\s up doc?'
//@[0:5) Identifier |param|
//@[6:15) Identifier |wrongType|
//@[16:27) Identifier |fluffyBunny|
//@[28:29) Assignment |=|
//@[30:46) StringComplete |'what\s up doc?'|
//@[46:48) NewLine |\n\n|

// unterminated string 
//@[23:24) NewLine |\n|
param wrongType fluffyBunny = 'what\'s up doc?
//@[0:5) Identifier |param|
//@[6:15) Identifier |wrongType|
//@[16:27) Identifier |fluffyBunny|
//@[28:29) Assignment |=|
//@[30:46) StringComplete |'what\'s up doc?|
//@[46:48) NewLine |\n\n|

// unterminated interpolated string
//@[35:36) NewLine |\n|
param wrongType fluffyBunny = 'what\'s ${
//@[0:5) Identifier |param|
//@[6:15) Identifier |wrongType|
//@[16:27) Identifier |fluffyBunny|
//@[28:29) Assignment |=|
//@[30:41) StringLeftPiece |'what\'s ${|
//@[41:41) StringRightPiece ||
//@[41:42) NewLine |\n|
param wrongType fluffyBunny = 'what\'s ${up
//@[0:5) Identifier |param|
//@[6:15) Identifier |wrongType|
//@[16:27) Identifier |fluffyBunny|
//@[28:29) Assignment |=|
//@[30:41) StringLeftPiece |'what\'s ${|
//@[41:43) Identifier |up|
//@[43:43) StringRightPiece ||
//@[43:44) NewLine |\n|
param wrongType fluffyBunny = 'what\'s ${up}
//@[0:5) Identifier |param|
//@[6:15) Identifier |wrongType|
//@[16:27) Identifier |fluffyBunny|
//@[28:29) Assignment |=|
//@[30:41) StringLeftPiece |'what\'s ${|
//@[41:43) Identifier |up|
//@[43:44) StringRightPiece |}|
//@[44:45) NewLine |\n|
param wrongType fluffyBunny = 'what\'s ${'up
//@[0:5) Identifier |param|
//@[6:15) Identifier |wrongType|
//@[16:27) Identifier |fluffyBunny|
//@[28:29) Assignment |=|
//@[30:41) StringLeftPiece |'what\'s ${|
//@[41:44) StringComplete |'up|
//@[44:44) StringRightPiece ||
//@[44:46) NewLine |\n\n|

// unterminated nested interpolated string
//@[42:43) NewLine |\n|
param wrongType fluffyBunny = 'what\'s ${'up${
//@[0:5) Identifier |param|
//@[6:15) Identifier |wrongType|
//@[16:27) Identifier |fluffyBunny|
//@[28:29) Assignment |=|
//@[30:41) StringLeftPiece |'what\'s ${|
//@[41:46) StringLeftPiece |'up${|
//@[46:46) StringRightPiece ||
//@[46:47) NewLine |\n|
param wrongType fluffyBunny = 'what\'s ${'up${
//@[0:5) Identifier |param|
//@[6:15) Identifier |wrongType|
//@[16:27) Identifier |fluffyBunny|
//@[28:29) Assignment |=|
//@[30:41) StringLeftPiece |'what\'s ${|
//@[41:46) StringLeftPiece |'up${|
//@[46:46) StringRightPiece ||
//@[46:47) NewLine |\n|
param wrongType fluffyBunny = 'what\'s ${'up${doc
//@[0:5) Identifier |param|
//@[6:15) Identifier |wrongType|
//@[16:27) Identifier |fluffyBunny|
//@[28:29) Assignment |=|
//@[30:41) StringLeftPiece |'what\'s ${|
//@[41:46) StringLeftPiece |'up${|
//@[46:49) Identifier |doc|
//@[49:49) StringRightPiece ||
//@[49:50) NewLine |\n|
param wrongType fluffyBunny = 'what\'s ${'up${doc}
//@[0:5) Identifier |param|
//@[6:15) Identifier |wrongType|
//@[16:27) Identifier |fluffyBunny|
//@[28:29) Assignment |=|
//@[30:41) StringLeftPiece |'what\'s ${|
//@[41:46) StringLeftPiece |'up${|
//@[46:49) Identifier |doc|
//@[49:50) StringRightPiece |}|
//@[50:50) StringRightPiece ||
//@[50:51) NewLine |\n|
param wrongType fluffyBunny = 'what\'s ${'up${doc}'
//@[0:5) Identifier |param|
//@[6:15) Identifier |wrongType|
//@[16:27) Identifier |fluffyBunny|
//@[28:29) Assignment |=|
//@[30:41) StringLeftPiece |'what\'s ${|
//@[41:46) StringLeftPiece |'up${|
//@[46:49) Identifier |doc|
//@[49:51) StringRightPiece |}'|
//@[51:51) StringRightPiece ||
//@[51:52) NewLine |\n|
param wrongType fluffyBunny = 'what\'s ${'up${doc}'}?
//@[0:5) Identifier |param|
//@[6:15) Identifier |wrongType|
//@[16:27) Identifier |fluffyBunny|
//@[28:29) Assignment |=|
//@[30:41) StringLeftPiece |'what\'s ${|
//@[41:46) StringLeftPiece |'up${|
//@[46:49) Identifier |doc|
//@[49:51) StringRightPiece |}'|
//@[51:53) StringRightPiece |}?|
//@[53:55) NewLine |\n\n|

// object literal inside interpolated string
//@[44:45) NewLine |\n|
param wrongType fluffyBunny = '${{this: doesnt}.work}'
//@[0:5) Identifier |param|
//@[6:15) Identifier |wrongType|
//@[16:27) Identifier |fluffyBunny|
//@[28:29) Assignment |=|
//@[30:33) StringLeftPiece |'${|
//@[33:34) LeftBrace |{|
//@[34:38) Identifier |this|
//@[38:39) Colon |:|
//@[40:46) Identifier |doesnt|
//@[46:47) RightBrace |}|
//@[47:48) Dot |.|
//@[48:52) Identifier |work|
//@[52:53) RightBrace |}|
//@[53:54) StringComplete |'|
//@[54:54) StringRightPiece ||
//@[54:56) NewLine |\n\n|

// bad interpolated string format
//@[33:34) NewLine |\n|
param badInterpolatedString string = 'hello ${}!'
//@[0:5) Identifier |param|
//@[6:27) Identifier |badInterpolatedString|
//@[28:34) Identifier |string|
//@[35:36) Assignment |=|
//@[37:46) StringLeftPiece |'hello ${|
//@[46:49) StringRightPiece |}!'|
//@[49:50) NewLine |\n|
param badInterpolatedString2 string = 'hello ${a b c}!'
//@[0:5) Identifier |param|
//@[6:28) Identifier |badInterpolatedString2|
//@[29:35) Identifier |string|
//@[36:37) Assignment |=|
//@[38:47) StringLeftPiece |'hello ${|
//@[47:48) Identifier |a|
//@[49:50) Identifier |b|
//@[51:52) Identifier |c|
//@[52:55) StringRightPiece |}!'|
//@[55:57) NewLine |\n\n|

param wrongType fluffyBunny = 'what\'s up doc?'
//@[0:5) Identifier |param|
//@[6:15) Identifier |wrongType|
//@[16:27) Identifier |fluffyBunny|
//@[28:29) Assignment |=|
//@[30:47) StringComplete |'what\'s up doc?'|
//@[47:49) NewLine |\n\n|

// modifier on an invalid type
//@[30:31) NewLine |\n|
@minLength(3)
//@[0:1) At |@|
//@[1:10) Identifier |minLength|
//@[10:11) LeftParen |(|
//@[11:12) Integer |3|
//@[12:13) RightParen |)|
//@[13:14) NewLine |\n|
@maxLength(24)
//@[0:1) At |@|
//@[1:10) Identifier |maxLength|
//@[10:11) LeftParen |(|
//@[11:13) Integer |24|
//@[13:14) RightParen |)|
//@[14:15) NewLine |\n|
param someArray arra
//@[0:5) Identifier |param|
//@[6:15) Identifier |someArray|
//@[16:20) Identifier |arra|
//@[20:22) NewLine |\n\n|

@secure()
//@[0:1) At |@|
//@[1:7) Identifier |secure|
//@[7:8) LeftParen |(|
//@[8:9) RightParen |)|
//@[9:10) NewLine |\n|
@minLength(3)
//@[0:1) At |@|
//@[1:10) Identifier |minLength|
//@[10:11) LeftParen |(|
//@[11:12) Integer |3|
//@[12:13) RightParen |)|
//@[13:14) NewLine |\n|
@maxLength(123)
//@[0:1) At |@|
//@[1:10) Identifier |maxLength|
//@[10:11) LeftParen |(|
//@[11:14) Integer |123|
//@[14:15) RightParen |)|
//@[15:16) NewLine |\n|
param secureInt int
//@[0:5) Identifier |param|
//@[6:15) Identifier |secureInt|
//@[16:19) Identifier |int|
//@[19:21) NewLine |\n\n|

// wrong modifier value types
//@[29:30) NewLine |\n|
@allowed([
//@[0:1) At |@|
//@[1:8) Identifier |allowed|
//@[8:9) LeftParen |(|
//@[9:10) LeftSquare |[|
//@[10:11) NewLine |\n|
  'test'
//@[2:8) StringComplete |'test'|
//@[8:9) NewLine |\n|
  true
//@[2:6) TrueKeyword |true|
//@[6:7) NewLine |\n|
])
//@[0:1) RightSquare |]|
//@[1:2) RightParen |)|
//@[2:3) NewLine |\n|
@minValue({
//@[0:1) At |@|
//@[1:9) Identifier |minValue|
//@[9:10) LeftParen |(|
//@[10:11) LeftBrace |{|
//@[11:12) NewLine |\n|
})
//@[0:1) RightBrace |}|
//@[1:2) RightParen |)|
//@[2:3) NewLine |\n|
@maxValue([
//@[0:1) At |@|
//@[1:9) Identifier |maxValue|
//@[9:10) LeftParen |(|
//@[10:11) LeftSquare |[|
//@[11:12) NewLine |\n|
])
//@[0:1) RightSquare |]|
//@[1:2) RightParen |)|
//@[2:3) NewLine |\n|
@metadata('wrong')
//@[0:1) At |@|
//@[1:9) Identifier |metadata|
//@[9:10) LeftParen |(|
//@[10:17) StringComplete |'wrong'|
//@[17:18) RightParen |)|
//@[18:19) NewLine |\n|
param wrongIntModifier int = true
//@[0:5) Identifier |param|
//@[6:22) Identifier |wrongIntModifier|
//@[23:26) Identifier |int|
//@[27:28) Assignment |=|
//@[29:33) TrueKeyword |true|
//@[33:35) NewLine |\n\n|

@metadata(any([]))
//@[0:1) At |@|
//@[1:9) Identifier |metadata|
//@[9:10) LeftParen |(|
//@[10:13) Identifier |any|
//@[13:14) LeftParen |(|
//@[14:15) LeftSquare |[|
//@[15:16) RightSquare |]|
//@[16:17) RightParen |)|
//@[17:18) RightParen |)|
//@[18:19) NewLine |\n|
@allowed(any(2))
//@[0:1) At |@|
//@[1:8) Identifier |allowed|
//@[8:9) LeftParen |(|
//@[9:12) Identifier |any|
//@[12:13) LeftParen |(|
//@[13:14) Integer |2|
//@[14:15) RightParen |)|
//@[15:16) RightParen |)|
//@[16:17) NewLine |\n|
param fatalErrorInIssue1713
//@[0:5) Identifier |param|
//@[6:27) Identifier |fatalErrorInIssue1713|
//@[27:29) NewLine |\n\n|

// wrong metadata schema
//@[24:25) NewLine |\n|
@metadata({
//@[0:1) At |@|
//@[1:9) Identifier |metadata|
//@[9:10) LeftParen |(|
//@[10:11) LeftBrace |{|
//@[11:12) NewLine |\n|
  description: true
//@[2:13) Identifier |description|
//@[13:14) Colon |:|
//@[15:19) TrueKeyword |true|
//@[19:20) NewLine |\n|
})
//@[0:1) RightBrace |}|
//@[1:2) RightParen |)|
//@[2:3) NewLine |\n|
param wrongMetadataSchema string
//@[0:5) Identifier |param|
//@[6:25) Identifier |wrongMetadataSchema|
//@[26:32) Identifier |string|
//@[32:34) NewLine |\n\n|

// expression in modifier
//@[25:26) NewLine |\n|
@maxLength(a + 2)
//@[0:1) At |@|
//@[1:10) Identifier |maxLength|
//@[10:11) LeftParen |(|
//@[11:12) Identifier |a|
//@[13:14) Plus |+|
//@[15:16) Integer |2|
//@[16:17) RightParen |)|
//@[17:18) NewLine |\n|
@minLength(foo())
//@[0:1) At |@|
//@[1:10) Identifier |minLength|
//@[10:11) LeftParen |(|
//@[11:14) Identifier |foo|
//@[14:15) LeftParen |(|
//@[15:16) RightParen |)|
//@[16:17) RightParen |)|
//@[17:18) NewLine |\n|
@allowed([
//@[0:1) At |@|
//@[1:8) Identifier |allowed|
//@[8:9) LeftParen |(|
//@[9:10) LeftSquare |[|
//@[10:11) NewLine |\n|
  i
//@[2:3) Identifier |i|
//@[3:4) NewLine |\n|
])
//@[0:1) RightSquare |]|
//@[1:2) RightParen |)|
//@[2:3) NewLine |\n|
param expressionInModifier string = 2 + 3
//@[0:5) Identifier |param|
//@[6:26) Identifier |expressionInModifier|
//@[27:33) Identifier |string|
//@[34:35) Assignment |=|
//@[36:37) Integer |2|
//@[38:39) Plus |+|
//@[40:41) Integer |3|
//@[41:43) NewLine |\n\n|

@maxLength(2 + 3)
//@[0:1) At |@|
//@[1:10) Identifier |maxLength|
//@[10:11) LeftParen |(|
//@[11:12) Integer |2|
//@[13:14) Plus |+|
//@[15:16) Integer |3|
//@[16:17) RightParen |)|
//@[17:18) NewLine |\n|
@minLength(length([]))
//@[0:1) At |@|
//@[1:10) Identifier |minLength|
//@[10:11) LeftParen |(|
//@[11:17) Identifier |length|
//@[17:18) LeftParen |(|
//@[18:19) LeftSquare |[|
//@[19:20) RightSquare |]|
//@[20:21) RightParen |)|
//@[21:22) RightParen |)|
//@[22:23) NewLine |\n|
@allowed([
//@[0:1) At |@|
//@[1:8) Identifier |allowed|
//@[8:9) LeftParen |(|
//@[9:10) LeftSquare |[|
//@[10:11) NewLine |\n|
  resourceGroup().id
//@[2:15) Identifier |resourceGroup|
//@[15:16) LeftParen |(|
//@[16:17) RightParen |)|
//@[17:18) Dot |.|
//@[18:20) Identifier |id|
//@[20:21) NewLine |\n|
])
//@[0:1) RightSquare |]|
//@[1:2) RightParen |)|
//@[2:3) NewLine |\n|
param nonCompileTimeConstant string
//@[0:5) Identifier |param|
//@[6:28) Identifier |nonCompileTimeConstant|
//@[29:35) Identifier |string|
//@[35:38) NewLine |\n\n\n|


@allowed([])
//@[0:1) At |@|
//@[1:8) Identifier |allowed|
//@[8:9) LeftParen |(|
//@[9:10) LeftSquare |[|
//@[10:11) RightSquare |]|
//@[11:12) RightParen |)|
//@[12:13) NewLine |\n|
param emptyAllowedString string
//@[0:5) Identifier |param|
//@[6:24) Identifier |emptyAllowedString|
//@[25:31) Identifier |string|
//@[31:33) NewLine |\n\n|

@allowed([])
//@[0:1) At |@|
//@[1:8) Identifier |allowed|
//@[8:9) LeftParen |(|
//@[9:10) LeftSquare |[|
//@[10:11) RightSquare |]|
//@[11:12) RightParen |)|
//@[12:13) NewLine |\n|
param emptyAllowedInt int
//@[0:5) Identifier |param|
//@[6:21) Identifier |emptyAllowedInt|
//@[22:25) Identifier |int|
//@[25:27) NewLine |\n\n|

// 1-cycle in params
//@[20:21) NewLine |\n|
param paramDefaultOneCycle string = paramDefaultOneCycle
//@[0:5) Identifier |param|
//@[6:26) Identifier |paramDefaultOneCycle|
//@[27:33) Identifier |string|
//@[34:35) Assignment |=|
//@[36:56) Identifier |paramDefaultOneCycle|
//@[56:58) NewLine |\n\n|

// 2-cycle in params
//@[20:21) NewLine |\n|
param paramDefaultTwoCycle1 string = paramDefaultTwoCycle2
//@[0:5) Identifier |param|
//@[6:27) Identifier |paramDefaultTwoCycle1|
//@[28:34) Identifier |string|
//@[35:36) Assignment |=|
//@[37:58) Identifier |paramDefaultTwoCycle2|
//@[58:59) NewLine |\n|
param paramDefaultTwoCycle2 string = paramDefaultTwoCycle1
//@[0:5) Identifier |param|
//@[6:27) Identifier |paramDefaultTwoCycle2|
//@[28:34) Identifier |string|
//@[35:36) Assignment |=|
//@[37:58) Identifier |paramDefaultTwoCycle1|
//@[58:60) NewLine |\n\n|

@allowed([
//@[0:1) At |@|
//@[1:8) Identifier |allowed|
//@[8:9) LeftParen |(|
//@[9:10) LeftSquare |[|
//@[10:11) NewLine |\n|
  paramModifierSelfCycle
//@[2:24) Identifier |paramModifierSelfCycle|
//@[24:25) NewLine |\n|
])
//@[0:1) RightSquare |]|
//@[1:2) RightParen |)|
//@[2:3) NewLine |\n|
param paramModifierSelfCycle string
//@[0:5) Identifier |param|
//@[6:28) Identifier |paramModifierSelfCycle|
//@[29:35) Identifier |string|
//@[35:37) NewLine |\n\n|

// wrong types of "variable"/identifier access
//@[46:47) NewLine |\n|
var sampleVar = 'sample'
//@[0:3) Identifier |var|
//@[4:13) Identifier |sampleVar|
//@[14:15) Assignment |=|
//@[16:24) StringComplete |'sample'|
//@[24:25) NewLine |\n|
resource sampleResource 'Microsoft.Foo/foos@2020-02-02' = {
//@[0:8) Identifier |resource|
//@[9:23) Identifier |sampleResource|
//@[24:55) StringComplete |'Microsoft.Foo/foos@2020-02-02'|
//@[56:57) Assignment |=|
//@[58:59) LeftBrace |{|
//@[59:60) NewLine |\n|
  name: 'foo'
//@[2:6) Identifier |name|
//@[6:7) Colon |:|
//@[8:13) StringComplete |'foo'|
//@[13:14) NewLine |\n|
}
//@[0:1) RightBrace |}|
//@[1:2) NewLine |\n|
output sampleOutput string = 'hello'
//@[0:6) Identifier |output|
//@[7:19) Identifier |sampleOutput|
//@[20:26) Identifier |string|
//@[27:28) Assignment |=|
//@[29:36) StringComplete |'hello'|
//@[36:38) NewLine |\n\n|

param paramAccessingVar string = concat(sampleVar, 's')
//@[0:5) Identifier |param|
//@[6:23) Identifier |paramAccessingVar|
//@[24:30) Identifier |string|
//@[31:32) Assignment |=|
//@[33:39) Identifier |concat|
//@[39:40) LeftParen |(|
//@[40:49) Identifier |sampleVar|
//@[49:50) Comma |,|
//@[51:54) StringComplete |'s'|
//@[54:55) RightParen |)|
//@[55:57) NewLine |\n\n|

param paramAccessingResource string = sampleResource
//@[0:5) Identifier |param|
//@[6:28) Identifier |paramAccessingResource|
//@[29:35) Identifier |string|
//@[36:37) Assignment |=|
//@[38:52) Identifier |sampleResource|
//@[52:54) NewLine |\n\n|

param paramAccessingOutput string = sampleOutput
//@[0:5) Identifier |param|
//@[6:26) Identifier |paramAccessingOutput|
//@[27:33) Identifier |string|
//@[34:35) Assignment |=|
//@[36:48) Identifier |sampleOutput|
//@[48:50) NewLine |\n\n|

// #completionTest(6) -> empty
//@[30:31) NewLine |\n|
param 
//@[0:5) Identifier |param|
//@[6:8) NewLine |\n\n|

// #completionTest(46,47) -> justSymbols
//@[40:41) NewLine |\n|
param defaultValueOneLinerCompletions string = 
//@[0:5) Identifier |param|
//@[6:37) Identifier |defaultValueOneLinerCompletions|
//@[38:44) Identifier |string|
//@[45:46) Assignment |=|
//@[47:49) NewLine |\n\n|

// invalid comma separator (array)
//@[34:35) NewLine |\n|
@metadata({
//@[0:1) At |@|
//@[1:9) Identifier |metadata|
//@[9:10) LeftParen |(|
//@[10:11) LeftBrace |{|
//@[11:12) NewLine |\n|
  description: 'Name of Virtual Machine'
//@[2:13) Identifier |description|
//@[13:14) Colon |:|
//@[15:40) StringComplete |'Name of Virtual Machine'|
//@[40:41) NewLine |\n|
})
//@[0:1) RightBrace |}|
//@[1:2) RightParen |)|
//@[2:3) NewLine |\n|
@allowed([
//@[0:1) At |@|
//@[1:8) Identifier |allowed|
//@[8:9) LeftParen |(|
//@[9:10) LeftSquare |[|
//@[10:11) NewLine |\n|
  'abc',
//@[2:7) StringComplete |'abc'|
//@[7:8) Comma |,|
//@[8:9) NewLine |\n|
  'def'
//@[2:7) StringComplete |'def'|
//@[7:8) NewLine |\n|
])
//@[0:1) RightSquare |]|
//@[1:2) RightParen |)|
//@[2:3) NewLine |\n|
param commaOne string
//@[0:5) Identifier |param|
//@[6:14) Identifier |commaOne|
//@[15:21) Identifier |string|
//@[21:23) NewLine |\n\n|

@secure
//@[0:1) At |@|
//@[1:7) Identifier |secure|
//@[7:8) NewLine |\n|
@
//@[0:1) At |@|
//@[1:2) NewLine |\n|
@&& xxx
//@[0:1) At |@|
//@[1:3) LogicalAnd |&&|
//@[4:7) Identifier |xxx|
//@[7:8) NewLine |\n|
@sys
//@[0:1) At |@|
//@[1:4) Identifier |sys|
//@[4:5) NewLine |\n|
@paramAccessingVar
//@[0:1) At |@|
//@[1:18) Identifier |paramAccessingVar|
//@[18:19) NewLine |\n|
param incompleteDecorators string
//@[0:5) Identifier |param|
//@[6:26) Identifier |incompleteDecorators|
//@[27:33) Identifier |string|
//@[33:35) NewLine |\n\n|

@concat(1, 2)
//@[0:1) At |@|
//@[1:7) Identifier |concat|
//@[7:8) LeftParen |(|
//@[8:9) Integer |1|
//@[9:10) Comma |,|
//@[11:12) Integer |2|
//@[12:13) RightParen |)|
//@[13:14) NewLine |\n|
@sys.concat('a', 'b')
//@[0:1) At |@|
//@[1:4) Identifier |sys|
//@[4:5) Dot |.|
//@[5:11) Identifier |concat|
//@[11:12) LeftParen |(|
//@[12:15) StringComplete |'a'|
//@[15:16) Comma |,|
//@[17:20) StringComplete |'b'|
//@[20:21) RightParen |)|
//@[21:22) NewLine |\n|
@secure()
//@[0:1) At |@|
//@[1:7) Identifier |secure|
//@[7:8) LeftParen |(|
//@[8:9) RightParen |)|
//@[9:10) NewLine |\n|
// wrong target type
//@[20:21) NewLine |\n|
@minValue(20)
//@[0:1) At |@|
//@[1:9) Identifier |minValue|
//@[9:10) LeftParen |(|
//@[10:12) Integer |20|
//@[12:13) RightParen |)|
//@[13:14) NewLine |\n|
param someString string
//@[0:5) Identifier |param|
//@[6:16) Identifier |someString|
//@[17:23) Identifier |string|
//@[23:25) NewLine |\n\n|

@allowed([
//@[0:1) At |@|
//@[1:8) Identifier |allowed|
//@[8:9) LeftParen |(|
//@[9:10) LeftSquare |[|
//@[10:11) NewLine |\n|
    true
//@[4:8) TrueKeyword |true|
//@[8:9) NewLine |\n|
    10
//@[4:6) Integer |10|
//@[6:7) NewLine |\n|
    'foo'
//@[4:9) StringComplete |'foo'|
//@[9:10) NewLine |\n|
])
//@[0:1) RightSquare |]|
//@[1:2) RightParen |)|
//@[2:3) NewLine |\n|
@secure()
//@[0:1) At |@|
//@[1:7) Identifier |secure|
//@[7:8) LeftParen |(|
//@[8:9) RightParen |)|
//@[9:10) NewLine |\n|
// #completionTest(1, 2, 3) -> intParameterDecoratorsPlusNamespace
//@[66:67) NewLine |\n|
@  
//@[0:1) At |@|
//@[3:4) NewLine |\n|
// #completionTest(5, 6) -> intParameterDecorators
//@[50:51) NewLine |\n|
@sys.   
//@[0:1) At |@|
//@[1:4) Identifier |sys|
//@[4:5) Dot |.|
//@[8:9) NewLine |\n|
param someInteger int = 20
//@[0:5) Identifier |param|
//@[6:17) Identifier |someInteger|
//@[18:21) Identifier |int|
//@[22:23) Assignment |=|
//@[24:26) Integer |20|
//@[26:28) NewLine |\n\n|

@allowed([], [], 2)
//@[0:1) At |@|
//@[1:8) Identifier |allowed|
//@[8:9) LeftParen |(|
//@[9:10) LeftSquare |[|
//@[10:11) RightSquare |]|
//@[11:12) Comma |,|
//@[13:14) LeftSquare |[|
//@[14:15) RightSquare |]|
//@[15:16) Comma |,|
//@[17:18) Integer |2|
//@[18:19) RightParen |)|
//@[19:20) NewLine |\n|
// #completionTest(4) -> empty
//@[30:31) NewLine |\n|
@az.
//@[0:1) At |@|
//@[1:3) Identifier |az|
//@[3:4) Dot |.|
//@[4:5) NewLine |\n|
param tooManyArguments1 int = 20
//@[0:5) Identifier |param|
//@[6:23) Identifier |tooManyArguments1|
//@[24:27) Identifier |int|
//@[28:29) Assignment |=|
//@[30:32) Integer |20|
//@[32:34) NewLine |\n\n|

@metadata({}, {}, true)
//@[0:1) At |@|
//@[1:9) Identifier |metadata|
//@[9:10) LeftParen |(|
//@[10:11) LeftBrace |{|
//@[11:12) RightBrace |}|
//@[12:13) Comma |,|
//@[14:15) LeftBrace |{|
//@[15:16) RightBrace |}|
//@[16:17) Comma |,|
//@[18:22) TrueKeyword |true|
//@[22:23) RightParen |)|
//@[23:24) NewLine |\n|
// #completionTest(2) -> stringParameterDecoratorsPlusNamespace
//@[63:64) NewLine |\n|
@m
//@[0:1) At |@|
//@[1:2) Identifier |m|
//@[2:3) NewLine |\n|
// #completionTest(1, 2, 3) -> stringParameterDecoratorsPlusNamespace
//@[69:70) NewLine |\n|
@   
//@[0:1) At |@|
//@[4:5) NewLine |\n|
// #completionTest(5) -> stringParameterDecorators
//@[50:51) NewLine |\n|
@sys.
//@[0:1) At |@|
//@[1:4) Identifier |sys|
//@[4:5) Dot |.|
//@[5:6) NewLine |\n|
param tooManyArguments2 string
//@[0:5) Identifier |param|
//@[6:23) Identifier |tooManyArguments2|
//@[24:30) Identifier |string|
//@[30:32) NewLine |\n\n|

@description(sys.concat(2))
//@[0:1) At |@|
//@[1:12) Identifier |description|
//@[12:13) LeftParen |(|
//@[13:16) Identifier |sys|
//@[16:17) Dot |.|
//@[17:23) Identifier |concat|
//@[23:24) LeftParen |(|
//@[24:25) Integer |2|
//@[25:26) RightParen |)|
//@[26:27) RightParen |)|
//@[27:28) NewLine |\n|
@allowed([for thing in []: 's'])
//@[0:1) At |@|
//@[1:8) Identifier |allowed|
//@[8:9) LeftParen |(|
//@[9:10) LeftSquare |[|
//@[10:13) Identifier |for|
//@[14:19) Identifier |thing|
//@[20:22) Identifier |in|
//@[23:24) LeftSquare |[|
//@[24:25) RightSquare |]|
//@[25:26) Colon |:|
//@[27:30) StringComplete |'s'|
//@[30:31) RightSquare |]|
//@[31:32) RightParen |)|
//@[32:33) NewLine |\n|
param nonConstantInDecorator string
//@[0:5) Identifier |param|
//@[6:28) Identifier |nonConstantInDecorator|
//@[29:35) Identifier |string|
//@[35:37) NewLine |\n\n|

@minValue(-length('s'))
//@[0:1) At |@|
//@[1:9) Identifier |minValue|
//@[9:10) LeftParen |(|
//@[10:11) Minus |-|
//@[11:17) Identifier |length|
//@[17:18) LeftParen |(|
//@[18:21) StringComplete |'s'|
//@[21:22) RightParen |)|
//@[22:23) RightParen |)|
//@[23:24) NewLine |\n|
@metadata({
//@[0:1) At |@|
//@[1:9) Identifier |metadata|
//@[9:10) LeftParen |(|
//@[10:11) LeftBrace |{|
//@[11:12) NewLine |\n|
  bool: !true
//@[2:6) Identifier |bool|
//@[6:7) Colon |:|
//@[8:9) Exclamation |!|
//@[9:13) TrueKeyword |true|
//@[13:14) NewLine |\n|
})
//@[0:1) RightBrace |}|
//@[1:2) RightParen |)|
//@[2:3) NewLine |\n|
param unaryMinusOnFunction int
//@[0:5) Identifier |param|
//@[6:26) Identifier |unaryMinusOnFunction|
//@[27:30) Identifier |int|
//@[30:32) NewLine |\n\n|

@minLength(1)
//@[0:1) At |@|
//@[1:10) Identifier |minLength|
//@[10:11) LeftParen |(|
//@[11:12) Integer |1|
//@[12:13) RightParen |)|
//@[13:14) NewLine |\n|
@minLength(2)
//@[0:1) At |@|
//@[1:10) Identifier |minLength|
//@[10:11) LeftParen |(|
//@[11:12) Integer |2|
//@[12:13) RightParen |)|
//@[13:14) NewLine |\n|
@secure()
//@[0:1) At |@|
//@[1:7) Identifier |secure|
//@[7:8) LeftParen |(|
//@[8:9) RightParen |)|
//@[9:10) NewLine |\n|
@maxLength(3)
//@[0:1) At |@|
//@[1:10) Identifier |maxLength|
//@[10:11) LeftParen |(|
//@[11:12) Integer |3|
//@[12:13) RightParen |)|
//@[13:14) NewLine |\n|
@maxLength(4)
//@[0:1) At |@|
//@[1:10) Identifier |maxLength|
//@[10:11) LeftParen |(|
//@[11:12) Integer |4|
//@[12:13) RightParen |)|
//@[13:14) NewLine |\n|
param duplicateDecorators string
//@[0:5) Identifier |param|
//@[6:25) Identifier |duplicateDecorators|
//@[26:32) Identifier |string|
//@[32:34) NewLine |\n\n|

@minLength(-1)
//@[0:1) At |@|
//@[1:10) Identifier |minLength|
//@[10:11) LeftParen |(|
//@[11:12) Minus |-|
//@[12:13) Integer |1|
//@[13:14) RightParen |)|
//@[14:15) NewLine |\n|
@maxLength(-100)
//@[0:1) At |@|
//@[1:10) Identifier |maxLength|
//@[10:11) LeftParen |(|
//@[11:12) Minus |-|
//@[12:15) Integer |100|
//@[15:16) RightParen |)|
//@[16:17) NewLine |\n|
param invalidLength string
//@[0:5) Identifier |param|
//@[6:19) Identifier |invalidLength|
//@[20:26) Identifier |string|
//@[26:28) NewLine |\n\n|

@allowed([
//@[0:1) At |@|
//@[1:8) Identifier |allowed|
//@[8:9) LeftParen |(|
//@[9:10) LeftSquare |[|
//@[10:11) NewLine |\n|
	'Microsoft.AnalysisServices/servers'
//@[1:37) StringComplete |'Microsoft.AnalysisServices/servers'|
//@[37:38) NewLine |\n|
	'Microsoft.ApiManagement/service'
//@[1:34) StringComplete |'Microsoft.ApiManagement/service'|
//@[34:35) NewLine |\n|
	'Microsoft.Network/applicationGateways'
//@[1:40) StringComplete |'Microsoft.Network/applicationGateways'|
//@[40:41) NewLine |\n|
	'Microsoft.Automation/automationAccounts'
//@[1:42) StringComplete |'Microsoft.Automation/automationAccounts'|
//@[42:43) NewLine |\n|
	'Microsoft.ContainerInstance/containerGroups'
//@[1:46) StringComplete |'Microsoft.ContainerInstance/containerGroups'|
//@[46:47) NewLine |\n|
	'Microsoft.ContainerRegistry/registries'
//@[1:41) StringComplete |'Microsoft.ContainerRegistry/registries'|
//@[41:42) NewLine |\n|
	'Microsoft.ContainerService/managedClusters'
//@[1:45) StringComplete |'Microsoft.ContainerService/managedClusters'|
//@[45:46) NewLine |\n|
])
//@[0:1) RightSquare |]|
//@[1:2) RightParen |)|
//@[2:3) NewLine |\n|
param invalidPermutation array = [
//@[0:5) Identifier |param|
//@[6:24) Identifier |invalidPermutation|
//@[25:30) Identifier |array|
//@[31:32) Assignment |=|
//@[33:34) LeftSquare |[|
//@[34:35) NewLine |\n|
	'foobar'
//@[1:9) StringComplete |'foobar'|
//@[9:10) NewLine |\n|
	true
//@[1:5) TrueKeyword |true|
//@[5:6) NewLine |\n|
    100
//@[4:7) Integer |100|
//@[7:8) NewLine |\n|
]
//@[0:1) RightSquare |]|
//@[1:3) NewLine |\n\n|

@allowed([
//@[0:1) At |@|
//@[1:8) Identifier |allowed|
//@[8:9) LeftParen |(|
//@[9:10) LeftSquare |[|
//@[10:11) NewLine |\n|
	[
//@[1:2) LeftSquare |[|
//@[2:3) NewLine |\n|
		'Microsoft.AnalysisServices/servers'
//@[2:38) StringComplete |'Microsoft.AnalysisServices/servers'|
//@[38:39) NewLine |\n|
		'Microsoft.ApiManagement/service'
//@[2:35) StringComplete |'Microsoft.ApiManagement/service'|
//@[35:36) NewLine |\n|
	]
//@[1:2) RightSquare |]|
//@[2:3) NewLine |\n|
	[
//@[1:2) LeftSquare |[|
//@[2:3) NewLine |\n|
		'Microsoft.Network/applicationGateways'
//@[2:41) StringComplete |'Microsoft.Network/applicationGateways'|
//@[41:42) NewLine |\n|
		'Microsoft.Automation/automationAccounts'
//@[2:43) StringComplete |'Microsoft.Automation/automationAccounts'|
//@[43:44) NewLine |\n|
	]
//@[1:2) RightSquare |]|
//@[2:3) NewLine |\n|
])
//@[0:1) RightSquare |]|
//@[1:2) RightParen |)|
//@[2:3) NewLine |\n|
param invalidDefaultWithAllowedArrayDecorator array = true
//@[0:5) Identifier |param|
//@[6:45) Identifier |invalidDefaultWithAllowedArrayDecorator|
//@[46:51) Identifier |array|
//@[52:53) Assignment |=|
//@[54:58) TrueKeyword |true|
//@[58:60) NewLine |\n\n|

// unterminated multi-line comment
//@[34:35) NewLine |\n|
/*    

//@[0:0) EndOfFile ||
