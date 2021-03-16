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
param malformedModifier 44 {
//@[0:5) Identifier |param|
//@[6:23) Identifier |malformedModifier|
//@[24:26) Integer |44|
//@[27:28) LeftBrace |{|
//@[28:29) NewLine |\n|
  secure: 's'
//@[2:8) Identifier |secure|
//@[8:9) Colon |:|
//@[10:13) StringComplete |'s'|
//@[13:14) NewLine |\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

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
param someArray arra {
//@[0:5) Identifier |param|
//@[6:15) Identifier |someArray|
//@[16:20) Identifier |arra|
//@[21:22) LeftBrace |{|
//@[22:23) NewLine |\n|
  minLength: 3
//@[2:11) Identifier |minLength|
//@[11:12) Colon |:|
//@[13:14) Integer |3|
//@[14:15) NewLine |\n|
  maxLength: 24
//@[2:11) Identifier |maxLength|
//@[11:12) Colon |:|
//@[13:15) Integer |24|
//@[15:16) NewLine |\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

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
param someArrayWithDecorator arra
//@[0:5) Identifier |param|
//@[6:28) Identifier |someArrayWithDecorator|
//@[29:33) Identifier |arra|
//@[33:35) NewLine |\n\n|

// duplicate modifier property
//@[30:31) NewLine |\n|
param duplicatedModifierProperty string {
//@[0:5) Identifier |param|
//@[6:32) Identifier |duplicatedModifierProperty|
//@[33:39) Identifier |string|
//@[40:41) LeftBrace |{|
//@[41:42) NewLine |\n|
  minLength: 3
//@[2:11) Identifier |minLength|
//@[11:12) Colon |:|
//@[13:14) Integer |3|
//@[14:15) NewLine |\n|
  minLength: 24
//@[2:11) Identifier |minLength|
//@[11:12) Colon |:|
//@[13:15) Integer |24|
//@[15:16) NewLine |\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

// non-existent modifiers
//@[25:26) NewLine |\n|
param secureInt int {
//@[0:5) Identifier |param|
//@[6:15) Identifier |secureInt|
//@[16:19) Identifier |int|
//@[20:21) LeftBrace |{|
//@[21:22) NewLine |\n|
  secure: true
//@[2:8) Identifier |secure|
//@[8:9) Colon |:|
//@[10:14) TrueKeyword |true|
//@[14:15) NewLine |\n|
  minLength: 3
//@[2:11) Identifier |minLength|
//@[11:12) Colon |:|
//@[13:14) Integer |3|
//@[14:15) NewLine |\n|
  maxLength: 123
//@[2:11) Identifier |maxLength|
//@[11:12) Colon |:|
//@[13:16) Integer |123|
//@[16:17) NewLine |\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

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
param secureIntWithDecorator int
//@[0:5) Identifier |param|
//@[6:28) Identifier |secureIntWithDecorator|
//@[29:32) Identifier |int|
//@[32:34) NewLine |\n\n|

// wrong modifier value types
//@[29:30) NewLine |\n|
param wrongIntModifier int {
//@[0:5) Identifier |param|
//@[6:22) Identifier |wrongIntModifier|
//@[23:26) Identifier |int|
//@[27:28) LeftBrace |{|
//@[28:29) NewLine |\n|
  default: true
//@[2:9) Identifier |default|
//@[9:10) Colon |:|
//@[11:15) TrueKeyword |true|
//@[15:16) NewLine |\n|
  allowed: [
//@[2:9) Identifier |allowed|
//@[9:10) Colon |:|
//@[11:12) LeftSquare |[|
//@[12:13) NewLine |\n|
    'test'
//@[4:10) StringComplete |'test'|
//@[10:11) NewLine |\n|
    true
//@[4:8) TrueKeyword |true|
//@[8:9) NewLine |\n|
  ]
//@[2:3) RightSquare |]|
//@[3:4) NewLine |\n|
  minValue: {
//@[2:10) Identifier |minValue|
//@[10:11) Colon |:|
//@[12:13) LeftBrace |{|
//@[13:14) NewLine |\n|
  }
//@[2:3) RightBrace |}|
//@[3:4) NewLine |\n|
  maxValue: [
//@[2:10) Identifier |maxValue|
//@[10:11) Colon |:|
//@[12:13) LeftSquare |[|
//@[13:14) NewLine |\n|
  ]
//@[2:3) RightSquare |]|
//@[3:4) NewLine |\n|
  metadata: 'wrong'
//@[2:10) Identifier |metadata|
//@[10:11) Colon |:|
//@[12:19) StringComplete |'wrong'|
//@[19:20) NewLine |\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

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
param wrongIntModifierWithDecorator int = true
//@[0:5) Identifier |param|
//@[6:35) Identifier |wrongIntModifierWithDecorator|
//@[36:39) Identifier |int|
//@[40:41) Assignment |=|
//@[42:46) TrueKeyword |true|
//@[46:48) NewLine |\n\n|

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
param wrongMetadataSchema string {
//@[0:5) Identifier |param|
//@[6:25) Identifier |wrongMetadataSchema|
//@[26:32) Identifier |string|
//@[33:34) LeftBrace |{|
//@[34:35) NewLine |\n|
  metadata: {
//@[2:10) Identifier |metadata|
//@[10:11) Colon |:|
//@[12:13) LeftBrace |{|
//@[13:14) NewLine |\n|
    description: true
//@[4:15) Identifier |description|
//@[15:16) Colon |:|
//@[17:21) TrueKeyword |true|
//@[21:22) NewLine |\n|
  }
//@[2:3) RightBrace |}|
//@[3:4) NewLine |\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

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
param wrongMetadataSchemaWithDecorator string
//@[0:5) Identifier |param|
//@[6:38) Identifier |wrongMetadataSchemaWithDecorator|
//@[39:45) Identifier |string|
//@[45:47) NewLine |\n\n|

// expression in modifier
//@[25:26) NewLine |\n|
param expressionInModifier string {
//@[0:5) Identifier |param|
//@[6:26) Identifier |expressionInModifier|
//@[27:33) Identifier |string|
//@[34:35) LeftBrace |{|
//@[35:36) NewLine |\n|
  // #completionTest(10,11) -> symbolsPlusParamDefaultFunctions
//@[63:64) NewLine |\n|
  default: 2 + 3
//@[2:9) Identifier |default|
//@[9:10) Colon |:|
//@[11:12) Integer |2|
//@[13:14) Plus |+|
//@[15:16) Integer |3|
//@[16:17) NewLine |\n|
  maxLength: a + 2
//@[2:11) Identifier |maxLength|
//@[11:12) Colon |:|
//@[13:14) Identifier |a|
//@[15:16) Plus |+|
//@[17:18) Integer |2|
//@[18:19) NewLine |\n|
  minLength: foo()
//@[2:11) Identifier |minLength|
//@[11:12) Colon |:|
//@[13:16) Identifier |foo|
//@[16:17) LeftParen |(|
//@[17:18) RightParen |)|
//@[18:19) NewLine |\n|
  allowed: [
//@[2:9) Identifier |allowed|
//@[9:10) Colon |:|
//@[11:12) LeftSquare |[|
//@[12:13) NewLine |\n|
    i
//@[4:5) Identifier |i|
//@[5:6) NewLine |\n|
  ]
//@[2:3) RightSquare |]|
//@[3:4) NewLine |\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

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
param expressionInModifierWithDecorator string = 2 + 3
//@[0:5) Identifier |param|
//@[6:39) Identifier |expressionInModifierWithDecorator|
//@[40:46) Identifier |string|
//@[47:48) Assignment |=|
//@[49:50) Integer |2|
//@[51:52) Plus |+|
//@[53:54) Integer |3|
//@[54:56) NewLine |\n\n|

param nonCompileTimeConstant string {
//@[0:5) Identifier |param|
//@[6:28) Identifier |nonCompileTimeConstant|
//@[29:35) Identifier |string|
//@[36:37) LeftBrace |{|
//@[37:38) NewLine |\n|
  maxLength: 2 + 3
//@[2:11) Identifier |maxLength|
//@[11:12) Colon |:|
//@[13:14) Integer |2|
//@[15:16) Plus |+|
//@[17:18) Integer |3|
//@[18:19) NewLine |\n|
  minLength: length([])
//@[2:11) Identifier |minLength|
//@[11:12) Colon |:|
//@[13:19) Identifier |length|
//@[19:20) LeftParen |(|
//@[20:21) LeftSquare |[|
//@[21:22) RightSquare |]|
//@[22:23) RightParen |)|
//@[23:24) NewLine |\n|
  allowed: [
//@[2:9) Identifier |allowed|
//@[9:10) Colon |:|
//@[11:12) LeftSquare |[|
//@[12:13) NewLine |\n|
    resourceGroup().id
//@[4:17) Identifier |resourceGroup|
//@[17:18) LeftParen |(|
//@[18:19) RightParen |)|
//@[19:20) Dot |.|
//@[20:22) Identifier |id|
//@[22:23) NewLine |\n|
  ]
//@[2:3) RightSquare |]|
//@[3:4) NewLine |\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

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
param nonCompileTimeConstantWithDecorator string
//@[0:5) Identifier |param|
//@[6:41) Identifier |nonCompileTimeConstantWithDecorator|
//@[42:48) Identifier |string|
//@[48:51) NewLine |\n\n\n|


param emptyAllowedString string {
//@[0:5) Identifier |param|
//@[6:24) Identifier |emptyAllowedString|
//@[25:31) Identifier |string|
//@[32:33) LeftBrace |{|
//@[33:34) NewLine |\n|
  allowed: []
//@[2:9) Identifier |allowed|
//@[9:10) Colon |:|
//@[11:12) LeftSquare |[|
//@[12:13) RightSquare |]|
//@[13:14) NewLine |\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

@allowed([])
//@[0:1) At |@|
//@[1:8) Identifier |allowed|
//@[8:9) LeftParen |(|
//@[9:10) LeftSquare |[|
//@[10:11) RightSquare |]|
//@[11:12) RightParen |)|
//@[12:13) NewLine |\n|
param emptyAllowedStringWithDecorator string
//@[0:5) Identifier |param|
//@[6:37) Identifier |emptyAllowedStringWithDecorator|
//@[38:44) Identifier |string|
//@[44:46) NewLine |\n\n|

param emptyAllowedInt int {
//@[0:5) Identifier |param|
//@[6:21) Identifier |emptyAllowedInt|
//@[22:25) Identifier |int|
//@[26:27) LeftBrace |{|
//@[27:28) NewLine |\n|
  allowed: []
//@[2:9) Identifier |allowed|
//@[9:10) Colon |:|
//@[11:12) LeftSquare |[|
//@[12:13) RightSquare |]|
//@[13:14) NewLine |\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

@allowed([])
//@[0:1) At |@|
//@[1:8) Identifier |allowed|
//@[8:9) LeftParen |(|
//@[9:10) LeftSquare |[|
//@[10:11) RightSquare |]|
//@[11:12) RightParen |)|
//@[12:13) NewLine |\n|
param emptyAllowedIntWithDecorator int
//@[0:5) Identifier |param|
//@[6:34) Identifier |emptyAllowedIntWithDecorator|
//@[35:38) Identifier |int|
//@[38:40) NewLine |\n\n|

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

// 1-cycle in modifier params
//@[29:30) NewLine |\n|
param paramModifierOneCycle string {
//@[0:5) Identifier |param|
//@[6:27) Identifier |paramModifierOneCycle|
//@[28:34) Identifier |string|
//@[35:36) LeftBrace |{|
//@[36:37) NewLine |\n|
  default: paramModifierOneCycle
//@[2:9) Identifier |default|
//@[9:10) Colon |:|
//@[11:32) Identifier |paramModifierOneCycle|
//@[32:33) NewLine |\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

// 1-cycle in modifier with non-default property
//@[48:49) NewLine |\n|
param paramModifierSelfCycle string {
//@[0:5) Identifier |param|
//@[6:28) Identifier |paramModifierSelfCycle|
//@[29:35) Identifier |string|
//@[36:37) LeftBrace |{|
//@[37:38) NewLine |\n|
  allowed: [
//@[2:9) Identifier |allowed|
//@[9:10) Colon |:|
//@[11:12) LeftSquare |[|
//@[12:13) NewLine |\n|
    paramModifierSelfCycle
//@[4:26) Identifier |paramModifierSelfCycle|
//@[26:27) NewLine |\n|
  ]
//@[2:3) RightSquare |]|
//@[3:4) NewLine |\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

@allowed([
//@[0:1) At |@|
//@[1:8) Identifier |allowed|
//@[8:9) LeftParen |(|
//@[9:10) LeftSquare |[|
//@[10:11) NewLine |\n|
  paramModifierSelfCycleWithDecorator
//@[2:37) Identifier |paramModifierSelfCycleWithDecorator|
//@[37:38) NewLine |\n|
])
//@[0:1) RightSquare |]|
//@[1:2) RightParen |)|
//@[2:3) NewLine |\n|
param paramModifierSelfCycleWithDecorator string
//@[0:5) Identifier |param|
//@[6:41) Identifier |paramModifierSelfCycleWithDecorator|
//@[42:48) Identifier |string|
//@[48:50) NewLine |\n\n|

// 2-cycle in modifier params
//@[29:30) NewLine |\n|
param paramModifierTwoCycle1 string {
//@[0:5) Identifier |param|
//@[6:28) Identifier |paramModifierTwoCycle1|
//@[29:35) Identifier |string|
//@[36:37) LeftBrace |{|
//@[37:38) NewLine |\n|
  default: paramModifierTwoCycle2
//@[2:9) Identifier |default|
//@[9:10) Colon |:|
//@[11:33) Identifier |paramModifierTwoCycle2|
//@[33:34) NewLine |\n|
}
//@[0:1) RightBrace |}|
//@[1:2) NewLine |\n|
param paramModifierTwoCycle2 string {
//@[0:5) Identifier |param|
//@[6:28) Identifier |paramModifierTwoCycle2|
//@[29:35) Identifier |string|
//@[36:37) LeftBrace |{|
//@[37:38) NewLine |\n|
  default: paramModifierTwoCycle1
//@[2:9) Identifier |default|
//@[9:10) Colon |:|
//@[11:33) Identifier |paramModifierTwoCycle1|
//@[33:34) NewLine |\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

// 2-cycle mixed param syntaxes
//@[31:32) NewLine |\n|
param paramMixedTwoCycle1 string = paramMixedTwoCycle2
//@[0:5) Identifier |param|
//@[6:25) Identifier |paramMixedTwoCycle1|
//@[26:32) Identifier |string|
//@[33:34) Assignment |=|
//@[35:54) Identifier |paramMixedTwoCycle2|
//@[54:55) NewLine |\n|
param paramMixedTwoCycle2 string {
//@[0:5) Identifier |param|
//@[6:25) Identifier |paramMixedTwoCycle2|
//@[26:32) Identifier |string|
//@[33:34) LeftBrace |{|
//@[34:35) NewLine |\n|
  default: paramMixedTwoCycle1
//@[2:9) Identifier |default|
//@[9:10) Colon |:|
//@[11:30) Identifier |paramMixedTwoCycle1|
//@[30:31) NewLine |\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

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
//@[55:56) NewLine |\n|
param paramAccessingVar2 string {
//@[0:5) Identifier |param|
//@[6:24) Identifier |paramAccessingVar2|
//@[25:31) Identifier |string|
//@[32:33) LeftBrace |{|
//@[33:34) NewLine |\n|
  default: 'foo ${sampleVar} foo'
//@[2:9) Identifier |default|
//@[9:10) Colon |:|
//@[11:18) StringLeftPiece |'foo ${|
//@[18:27) Identifier |sampleVar|
//@[27:33) StringRightPiece |} foo'|
//@[33:34) NewLine |\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

param paramAccessingResource string = sampleResource
//@[0:5) Identifier |param|
//@[6:28) Identifier |paramAccessingResource|
//@[29:35) Identifier |string|
//@[36:37) Assignment |=|
//@[38:52) Identifier |sampleResource|
//@[52:53) NewLine |\n|
param paramAccessingResource2 string {
//@[0:5) Identifier |param|
//@[6:29) Identifier |paramAccessingResource2|
//@[30:36) Identifier |string|
//@[37:38) LeftBrace |{|
//@[38:39) NewLine |\n|
  default: base64(sampleResource.properties.foo)
//@[2:9) Identifier |default|
//@[9:10) Colon |:|
//@[11:17) Identifier |base64|
//@[17:18) LeftParen |(|
//@[18:32) Identifier |sampleResource|
//@[32:33) Dot |.|
//@[33:43) Identifier |properties|
//@[43:44) Dot |.|
//@[44:47) Identifier |foo|
//@[47:48) RightParen |)|
//@[48:49) NewLine |\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

param paramAccessingOutput string = sampleOutput
//@[0:5) Identifier |param|
//@[6:26) Identifier |paramAccessingOutput|
//@[27:33) Identifier |string|
//@[34:35) Assignment |=|
//@[36:48) Identifier |sampleOutput|
//@[48:49) NewLine |\n|
param paramAccessingOutput2 string {
//@[0:5) Identifier |param|
//@[6:27) Identifier |paramAccessingOutput2|
//@[28:34) Identifier |string|
//@[35:36) LeftBrace |{|
//@[36:37) NewLine |\n|
  default: sampleOutput
//@[2:9) Identifier |default|
//@[9:10) Colon |:|
//@[11:23) Identifier |sampleOutput|
//@[23:24) NewLine |\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

param stringLiteral string {
//@[0:5) Identifier |param|
//@[6:19) Identifier |stringLiteral|
//@[20:26) Identifier |string|
//@[27:28) LeftBrace |{|
//@[28:29) NewLine |\n|
  allowed: [
//@[2:9) Identifier |allowed|
//@[9:10) Colon |:|
//@[11:12) LeftSquare |[|
//@[12:13) NewLine |\n|
    'def'
//@[4:9) StringComplete |'def'|
//@[9:10) NewLine |\n|
  ]
//@[2:3) RightSquare |]|
//@[3:4) NewLine |\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

param stringLiteral2 string {
//@[0:5) Identifier |param|
//@[6:20) Identifier |stringLiteral2|
//@[21:27) Identifier |string|
//@[28:29) LeftBrace |{|
//@[29:30) NewLine |\n|
  allowed: [
//@[2:9) Identifier |allowed|
//@[9:10) Colon |:|
//@[11:12) LeftSquare |[|
//@[12:13) NewLine |\n|
    'abc'
//@[4:9) StringComplete |'abc'|
//@[9:10) NewLine |\n|
    'def'
//@[4:9) StringComplete |'def'|
//@[9:10) NewLine |\n|
  ]
//@[2:3) RightSquare |]|
//@[3:4) NewLine |\n|
  default: stringLiteral
//@[2:9) Identifier |default|
//@[9:10) Colon |:|
//@[11:24) Identifier |stringLiteral|
//@[24:25) NewLine |\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

param stringLiteral3 string {
//@[0:5) Identifier |param|
//@[6:20) Identifier |stringLiteral3|
//@[21:27) Identifier |string|
//@[28:29) LeftBrace |{|
//@[29:30) NewLine |\n|
  allowed: [
//@[2:9) Identifier |allowed|
//@[9:10) Colon |:|
//@[11:12) LeftSquare |[|
//@[12:13) NewLine |\n|
    'abc'
//@[4:9) StringComplete |'abc'|
//@[9:10) NewLine |\n|
  ]
//@[2:3) RightSquare |]|
//@[3:4) NewLine |\n|
  default: stringLiteral2
//@[2:9) Identifier |default|
//@[9:10) Colon |:|
//@[11:25) Identifier |stringLiteral2|
//@[25:26) NewLine |\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

// #completionTest(6) -> empty
//@[30:31) NewLine |\n|
param 
//@[0:5) Identifier |param|
//@[6:8) NewLine |\n\n|

param stringModifierCompletions string {
//@[0:5) Identifier |param|
//@[6:31) Identifier |stringModifierCompletions|
//@[32:38) Identifier |string|
//@[39:40) LeftBrace |{|
//@[40:41) NewLine |\n|
  // #completionTest(0,1,2) -> stringModifierProperties
//@[55:56) NewLine |\n|
  
//@[2:3) NewLine |\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

param intModifierCompletions int {
//@[0:5) Identifier |param|
//@[6:28) Identifier |intModifierCompletions|
//@[29:32) Identifier |int|
//@[33:34) LeftBrace |{|
//@[34:35) NewLine |\n|
  // #completionTest(0,1,2) -> intModifierProperties
//@[52:53) NewLine |\n|
  
//@[2:3) NewLine |\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

// #completionTest(46,47) -> justSymbols
//@[40:41) NewLine |\n|
param defaultValueOneLinerCompletions string = 
//@[0:5) Identifier |param|
//@[6:37) Identifier |defaultValueOneLinerCompletions|
//@[38:44) Identifier |string|
//@[45:46) Assignment |=|
//@[47:49) NewLine |\n\n|

param defaultValueCompletions string {
//@[0:5) Identifier |param|
//@[6:29) Identifier |defaultValueCompletions|
//@[30:36) Identifier |string|
//@[37:38) LeftBrace |{|
//@[38:39) NewLine |\n|
  allowed: [
//@[2:9) Identifier |allowed|
//@[9:10) Colon |:|
//@[11:12) LeftSquare |[|
//@[12:13) NewLine |\n|
    'one'
//@[4:9) StringComplete |'one'|
//@[9:10) NewLine |\n|
    'two'
//@[4:9) StringComplete |'two'|
//@[9:10) NewLine |\n|
    'three'
//@[4:11) StringComplete |'three'|
//@[11:12) NewLine |\n|
    // #completionTest(0,1,2,3,4) -> oneTwoThree
//@[48:49) NewLine |\n|
    
//@[4:5) NewLine |\n|
  ]
//@[2:3) RightSquare |]|
//@[3:4) NewLine |\n|
  // #completionTest(10,11) -> oneTwoThreePlusSymbols
//@[53:54) NewLine |\n|
  default: 
//@[2:9) Identifier |default|
//@[9:10) Colon |:|
//@[11:12) NewLine |\n|
  
//@[2:3) NewLine |\n|
  // #completionTest(9,10) -> booleanValues
//@[43:44) NewLine |\n|
  secure: 
//@[2:8) Identifier |secure|
//@[8:9) Colon |:|
//@[10:12) NewLine |\n\n|

  metadata: {
//@[2:10) Identifier |metadata|
//@[10:11) Colon |:|
//@[12:13) LeftBrace |{|
//@[13:14) NewLine |\n|
    // #completionTest(0,1,2,3) -> description
//@[46:47) NewLine |\n|
    
//@[4:5) NewLine |\n|
  }
//@[2:3) RightBrace |}|
//@[3:4) NewLine |\n|
  // #completionTest(0,1,2) -> stringLengthConstraints
//@[54:55) NewLine |\n|
  
//@[2:3) NewLine |\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

// invalid comma separator (array)
//@[34:35) NewLine |\n|
param commaOne string {
//@[0:5) Identifier |param|
//@[6:14) Identifier |commaOne|
//@[15:21) Identifier |string|
//@[22:23) LeftBrace |{|
//@[23:24) NewLine |\n|
    metadata: {
//@[4:12) Identifier |metadata|
//@[12:13) Colon |:|
//@[14:15) LeftBrace |{|
//@[15:16) NewLine |\n|
      description: 'Name of Virtual Machine'
//@[6:17) Identifier |description|
//@[17:18) Colon |:|
//@[19:44) StringComplete |'Name of Virtual Machine'|
//@[44:45) NewLine |\n|
    }
//@[4:5) RightBrace |}|
//@[5:6) NewLine |\n|
    secure: true
//@[4:10) Identifier |secure|
//@[10:11) Colon |:|
//@[12:16) TrueKeyword |true|
//@[16:17) NewLine |\n|
    allowed: [
//@[4:11) Identifier |allowed|
//@[11:12) Colon |:|
//@[13:14) LeftSquare |[|
//@[14:15) NewLine |\n|
      'abc',
//@[6:11) StringComplete |'abc'|
//@[11:12) Comma |,|
//@[12:13) NewLine |\n|
      'def'
//@[6:11) StringComplete |'def'|
//@[11:12) NewLine |\n|
    ]
//@[4:5) RightSquare |]|
//@[5:6) NewLine |\n|
    default: 'abc'
//@[4:11) Identifier |default|
//@[11:12) Colon |:|
//@[13:18) StringComplete |'abc'|
//@[18:19) NewLine |\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

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
param commaOneWithDecorator string
//@[0:5) Identifier |param|
//@[6:27) Identifier |commaOneWithDecorator|
//@[28:34) Identifier |string|
//@[34:36) NewLine |\n\n|

// invalid comma separator (object)
//@[35:36) NewLine |\n|
param commaTwo string {
//@[0:5) Identifier |param|
//@[6:14) Identifier |commaTwo|
//@[15:21) Identifier |string|
//@[22:23) LeftBrace |{|
//@[23:24) NewLine |\n|
    metadata: {
//@[4:12) Identifier |metadata|
//@[12:13) Colon |:|
//@[14:15) LeftBrace |{|
//@[15:16) NewLine |\n|
      description: 'Name of Virtual Machine'
//@[6:17) Identifier |description|
//@[17:18) Colon |:|
//@[19:44) StringComplete |'Name of Virtual Machine'|
//@[44:45) NewLine |\n|
    },
//@[4:5) RightBrace |}|
//@[5:6) Comma |,|
//@[6:7) NewLine |\n|
    secure: true
//@[4:10) Identifier |secure|
//@[10:11) Colon |:|
//@[12:16) TrueKeyword |true|
//@[16:17) NewLine |\n|
    allowed: [
//@[4:11) Identifier |allowed|
//@[11:12) Colon |:|
//@[13:14) LeftSquare |[|
//@[14:15) NewLine |\n|
      'abc'
//@[6:11) StringComplete |'abc'|
//@[11:12) NewLine |\n|
      'def'
//@[6:11) StringComplete |'def'|
//@[11:12) NewLine |\n|
    ]
//@[4:5) RightSquare |]|
//@[5:6) NewLine |\n|
    default: 'abc'
//@[4:11) Identifier |default|
//@[11:12) Colon |:|
//@[13:18) StringComplete |'abc'|
//@[18:19) NewLine |\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

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
param someString string {
//@[0:5) Identifier |param|
//@[6:16) Identifier |someString|
//@[17:23) Identifier |string|
//@[24:25) LeftBrace |{|
//@[25:26) NewLine |\n|
	// using decorators and modifier at the same time
//@[50:51) NewLine |\n|
    secure: true
//@[4:10) Identifier |secure|
//@[10:11) Colon |:|
//@[12:16) TrueKeyword |true|
//@[16:17) NewLine |\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

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

// unterminated multi-line comment
//@[34:35) NewLine |\n|
/*    

//@[0:0) EndOfFile ||
