/* 
  Valid and invalid code is mixed together to validate recovery logic. It can even contain ** * *** **.
*/
//@[2:4] NewLine |\n\n|

param myString string
//@[0:5] Identifier |param|
//@[6:14] Identifier |myString|
//@[15:21] Identifier |string|
//@[21:22] NewLine |\n|
wrong
//@[0:5] Identifier |wrong|
//@[5:7] NewLine |\n\n|

param myInt int
//@[0:5] Identifier |param|
//@[6:11] Identifier |myInt|
//@[12:15] Identifier |int|
//@[15:16] NewLine |\n|
param
//@[0:5] Identifier |param|
//@[5:7] NewLine |\n\n|

param myBool bool
//@[0:5] Identifier |param|
//@[6:12] Identifier |myBool|
//@[13:17] Identifier |bool|
//@[17:19] NewLine |\n\n|

param missingType
//@[0:5] Identifier |param|
//@[6:17] Identifier |missingType|
//@[17:19] NewLine |\n\n|

param myString2 string = 'string value'
//@[0:5] Identifier |param|
//@[6:15] Identifier |myString2|
//@[16:22] Identifier |string|
//@[23:24] Assignment |=|
//@[25:39] StringComplete |'string value'|
//@[39:41] NewLine |\n\n|

param wrongDefaultValue string = 42
//@[0:5] Identifier |param|
//@[6:23] Identifier |wrongDefaultValue|
//@[24:30] Identifier |string|
//@[31:32] Assignment |=|
//@[33:35] Number |42|
//@[35:37] NewLine |\n\n|

param myInt2 int = 42
//@[0:5] Identifier |param|
//@[6:12] Identifier |myInt2|
//@[13:16] Identifier |int|
//@[17:18] Assignment |=|
//@[19:21] Number |42|
//@[21:22] NewLine |\n|
param noValueAfterColon int =   
//@[0:5] Identifier |param|
//@[6:23] Identifier |noValueAfterColon|
//@[24:27] Identifier |int|
//@[28:29] Assignment |=|
//@[32:34] NewLine |\n\n|

param myTruth bool = 'not a boolean'
//@[0:5] Identifier |param|
//@[6:13] Identifier |myTruth|
//@[14:18] Identifier |bool|
//@[19:20] Assignment |=|
//@[21:36] StringComplete |'not a boolean'|
//@[36:37] NewLine |\n|
param myFalsehood bool = 'false'
//@[0:5] Identifier |param|
//@[6:17] Identifier |myFalsehood|
//@[18:22] Identifier |bool|
//@[23:24] Assignment |=|
//@[25:32] StringComplete |'false'|
//@[32:34] NewLine |\n\n|

param wrongAssignmentToken string: 'hello'
//@[0:5] Identifier |param|
//@[6:26] Identifier |wrongAssignmentToken|
//@[27:33] Identifier |string|
//@[33:34] Colon |:|
//@[35:42] StringComplete |'hello'|
//@[42:44] NewLine |\n\n|

param WhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLong string = 'why not?'
//@[0:5] Identifier |param|
//@[6:267] Identifier |WhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLong|
//@[268:274] Identifier |string|
//@[275:276] Assignment |=|
//@[277:287] StringComplete |'why not?'|
//@[287:289] NewLine |\n\n|

// badly escaped string
//@[23:24] NewLine |\n|
param wrongType fluffyBunny = 'what's up doc?'
//@[0:5] Identifier |param|
//@[6:15] Identifier |wrongType|
//@[16:27] Identifier |fluffyBunny|
//@[28:29] Assignment |=|
//@[30:36] StringComplete |'what'|
//@[36:37] Identifier |s|
//@[38:40] Identifier |up|
//@[41:44] Identifier |doc|
//@[44:45] Question |?|
//@[45:46] StringComplete |'|
//@[46:48] NewLine |\n\n|

// invalid escape
//@[17:18] NewLine |\n|
param wrongType fluffyBunny = 'what\s up doc?'
//@[0:5] Identifier |param|
//@[6:15] Identifier |wrongType|
//@[16:27] Identifier |fluffyBunny|
//@[28:29] Assignment |=|
//@[30:46] StringComplete |'what\s up doc?'|
//@[46:48] NewLine |\n\n|

// unterminated string 
//@[23:24] NewLine |\n|
param wrongType fluffyBunny = 'what\'s up doc?
//@[0:5] Identifier |param|
//@[6:15] Identifier |wrongType|
//@[16:27] Identifier |fluffyBunny|
//@[28:29] Assignment |=|
//@[30:46] StringComplete |'what\'s up doc?|
//@[46:48] NewLine |\n\n|

// unterminated interpolated string
//@[35:36] NewLine |\n|
param wrongType fluffyBunny = 'what\'s ${
//@[0:5] Identifier |param|
//@[6:15] Identifier |wrongType|
//@[16:27] Identifier |fluffyBunny|
//@[28:29] Assignment |=|
//@[30:41] StringLeftPiece |'what\'s ${|
//@[41:41] StringRightPiece ||
//@[41:42] NewLine |\n|
param wrongType fluffyBunny = 'what\'s ${up
//@[0:5] Identifier |param|
//@[6:15] Identifier |wrongType|
//@[16:27] Identifier |fluffyBunny|
//@[28:29] Assignment |=|
//@[30:41] StringLeftPiece |'what\'s ${|
//@[41:43] Identifier |up|
//@[43:43] StringRightPiece ||
//@[43:44] NewLine |\n|
param wrongType fluffyBunny = 'what\'s ${up}
//@[0:5] Identifier |param|
//@[6:15] Identifier |wrongType|
//@[16:27] Identifier |fluffyBunny|
//@[28:29] Assignment |=|
//@[30:41] StringLeftPiece |'what\'s ${|
//@[41:43] Identifier |up|
//@[43:44] StringRightPiece |}|
//@[44:45] NewLine |\n|
param wrongType fluffyBunny = 'what\'s ${'up
//@[0:5] Identifier |param|
//@[6:15] Identifier |wrongType|
//@[16:27] Identifier |fluffyBunny|
//@[28:29] Assignment |=|
//@[30:41] StringLeftPiece |'what\'s ${|
//@[41:44] StringComplete |'up|
//@[44:44] StringRightPiece ||
//@[44:46] NewLine |\n\n|

// unterminated nested interpolated string
//@[42:43] NewLine |\n|
param wrongType fluffyBunny = 'what\'s ${'up${
//@[0:5] Identifier |param|
//@[6:15] Identifier |wrongType|
//@[16:27] Identifier |fluffyBunny|
//@[28:29] Assignment |=|
//@[30:41] StringLeftPiece |'what\'s ${|
//@[41:46] StringLeftPiece |'up${|
//@[46:46] StringRightPiece ||
//@[46:47] NewLine |\n|
param wrongType fluffyBunny = 'what\'s ${'up${
//@[0:5] Identifier |param|
//@[6:15] Identifier |wrongType|
//@[16:27] Identifier |fluffyBunny|
//@[28:29] Assignment |=|
//@[30:41] StringLeftPiece |'what\'s ${|
//@[41:46] StringLeftPiece |'up${|
//@[46:46] StringRightPiece ||
//@[46:47] NewLine |\n|
param wrongType fluffyBunny = 'what\'s ${'up${doc
//@[0:5] Identifier |param|
//@[6:15] Identifier |wrongType|
//@[16:27] Identifier |fluffyBunny|
//@[28:29] Assignment |=|
//@[30:41] StringLeftPiece |'what\'s ${|
//@[41:46] StringLeftPiece |'up${|
//@[46:49] Identifier |doc|
//@[49:49] StringRightPiece ||
//@[49:50] NewLine |\n|
param wrongType fluffyBunny = 'what\'s ${'up${doc}
//@[0:5] Identifier |param|
//@[6:15] Identifier |wrongType|
//@[16:27] Identifier |fluffyBunny|
//@[28:29] Assignment |=|
//@[30:41] StringLeftPiece |'what\'s ${|
//@[41:46] StringLeftPiece |'up${|
//@[46:49] Identifier |doc|
//@[49:50] StringRightPiece |}|
//@[50:50] StringRightPiece ||
//@[50:51] NewLine |\n|
param wrongType fluffyBunny = 'what\'s ${'up${doc}'
//@[0:5] Identifier |param|
//@[6:15] Identifier |wrongType|
//@[16:27] Identifier |fluffyBunny|
//@[28:29] Assignment |=|
//@[30:41] StringLeftPiece |'what\'s ${|
//@[41:46] StringLeftPiece |'up${|
//@[46:49] Identifier |doc|
//@[49:51] StringRightPiece |}'|
//@[51:51] StringRightPiece ||
//@[51:52] NewLine |\n|
param wrongType fluffyBunny = 'what\'s ${'up${doc}'}?
//@[0:5] Identifier |param|
//@[6:15] Identifier |wrongType|
//@[16:27] Identifier |fluffyBunny|
//@[28:29] Assignment |=|
//@[30:41] StringLeftPiece |'what\'s ${|
//@[41:46] StringLeftPiece |'up${|
//@[46:49] Identifier |doc|
//@[49:51] StringRightPiece |}'|
//@[51:53] StringRightPiece |}?|
//@[53:55] NewLine |\n\n|

// object literal inside interpolated string
//@[44:45] NewLine |\n|
param wrongType fluffyBunny = '${{this: doesnt}.work}'
//@[0:5] Identifier |param|
//@[6:15] Identifier |wrongType|
//@[16:27] Identifier |fluffyBunny|
//@[28:29] Assignment |=|
//@[30:33] StringLeftPiece |'${|
//@[33:34] LeftBrace |{|
//@[34:38] Identifier |this|
//@[38:39] Colon |:|
//@[40:46] Identifier |doesnt|
//@[46:47] RightBrace |}|
//@[47:48] Dot |.|
//@[48:52] Identifier |work|
//@[52:53] RightBrace |}|
//@[53:54] StringComplete |'|
//@[54:54] StringRightPiece ||
//@[54:56] NewLine |\n\n|

param wrongType fluffyBunny = 'what\'s up doc?'
//@[0:5] Identifier |param|
//@[6:15] Identifier |wrongType|
//@[16:27] Identifier |fluffyBunny|
//@[28:29] Assignment |=|
//@[30:47] StringComplete |'what\'s up doc?'|
//@[47:49] NewLine |\n\n|

// modifier on an invalid type
//@[30:31] NewLine |\n|
param someArray arra {
//@[0:5] Identifier |param|
//@[6:15] Identifier |someArray|
//@[16:20] Identifier |arra|
//@[21:22] LeftBrace |{|
//@[22:23] NewLine |\n|
  minLength: 3
//@[2:11] Identifier |minLength|
//@[11:12] Colon |:|
//@[13:14] Number |3|
//@[14:15] NewLine |\n|
  maxLength: 24
//@[2:11] Identifier |maxLength|
//@[11:12] Colon |:|
//@[13:15] Number |24|
//@[15:16] NewLine |\n|
}
//@[0:1] RightBrace |}|
//@[1:3] NewLine |\n\n|

// duplicate modifier property
//@[30:31] NewLine |\n|
param duplicatedModifierProperty string {
//@[0:5] Identifier |param|
//@[6:32] Identifier |duplicatedModifierProperty|
//@[33:39] Identifier |string|
//@[40:41] LeftBrace |{|
//@[41:42] NewLine |\n|
  minLength: 3
//@[2:11] Identifier |minLength|
//@[11:12] Colon |:|
//@[13:14] Number |3|
//@[14:15] NewLine |\n|
  minLength: 24
//@[2:11] Identifier |minLength|
//@[11:12] Colon |:|
//@[13:15] Number |24|
//@[15:16] NewLine |\n|
}
//@[0:1] RightBrace |}|
//@[1:3] NewLine |\n\n|

// non-existent modifiers
//@[25:26] NewLine |\n|
param secureInt int {
//@[0:5] Identifier |param|
//@[6:15] Identifier |secureInt|
//@[16:19] Identifier |int|
//@[20:21] LeftBrace |{|
//@[21:22] NewLine |\n|
  secure: true
//@[2:8] Identifier |secure|
//@[8:9] Colon |:|
//@[10:14] TrueKeyword |true|
//@[14:15] NewLine |\n|
  minLength: 3
//@[2:11] Identifier |minLength|
//@[11:12] Colon |:|
//@[13:14] Number |3|
//@[14:15] NewLine |\n|
  maxLength: 123
//@[2:11] Identifier |maxLength|
//@[11:12] Colon |:|
//@[13:16] Number |123|
//@[16:17] NewLine |\n|
}
//@[0:1] RightBrace |}|
//@[1:3] NewLine |\n\n|

// wrong modifier value types
//@[29:30] NewLine |\n|
param wrongIntModifier int {
//@[0:5] Identifier |param|
//@[6:22] Identifier |wrongIntModifier|
//@[23:26] Identifier |int|
//@[27:28] LeftBrace |{|
//@[28:29] NewLine |\n|
  default: true
//@[2:9] Identifier |default|
//@[9:10] Colon |:|
//@[11:15] TrueKeyword |true|
//@[15:16] NewLine |\n|
  allowedValues: [
//@[2:15] Identifier |allowedValues|
//@[15:16] Colon |:|
//@[17:18] LeftSquare |[|
//@[18:19] NewLine |\n|
    'test'
//@[4:10] StringComplete |'test'|
//@[10:11] NewLine |\n|
    true
//@[4:8] TrueKeyword |true|
//@[8:9] NewLine |\n|
  ]
//@[2:3] RightSquare |]|
//@[3:4] NewLine |\n|
  minValue: {
//@[2:10] Identifier |minValue|
//@[10:11] Colon |:|
//@[12:13] LeftBrace |{|
//@[13:14] NewLine |\n|
  }
//@[2:3] RightBrace |}|
//@[3:4] NewLine |\n|
  maxValue: [
//@[2:10] Identifier |maxValue|
//@[10:11] Colon |:|
//@[12:13] LeftSquare |[|
//@[13:14] NewLine |\n|
  ]
//@[2:3] RightSquare |]|
//@[3:4] NewLine |\n|
  metadata: 'wrong'
//@[2:10] Identifier |metadata|
//@[10:11] Colon |:|
//@[12:19] StringComplete |'wrong'|
//@[19:20] NewLine |\n|
}
//@[0:1] RightBrace |}|
//@[1:3] NewLine |\n\n|

// wrong metadata schema
//@[24:25] NewLine |\n|
param wrongMetadataSchema string {
//@[0:5] Identifier |param|
//@[6:25] Identifier |wrongMetadataSchema|
//@[26:32] Identifier |string|
//@[33:34] LeftBrace |{|
//@[34:35] NewLine |\n|
  metadata: {
//@[2:10] Identifier |metadata|
//@[10:11] Colon |:|
//@[12:13] LeftBrace |{|
//@[13:14] NewLine |\n|
    description: true
//@[4:15] Identifier |description|
//@[15:16] Colon |:|
//@[17:21] TrueKeyword |true|
//@[21:22] NewLine |\n|
  }
//@[2:3] RightBrace |}|
//@[3:4] NewLine |\n|
}
//@[0:1] RightBrace |}|
//@[1:3] NewLine |\n\n|

// expression in modifier
//@[25:26] NewLine |\n|
param expressionInModifier string {
//@[0:5] Identifier |param|
//@[6:26] Identifier |expressionInModifier|
//@[27:33] Identifier |string|
//@[34:35] LeftBrace |{|
//@[35:36] NewLine |\n|
  default: 2 + 3
//@[2:9] Identifier |default|
//@[9:10] Colon |:|
//@[11:12] Number |2|
//@[13:14] Plus |+|
//@[15:16] Number |3|
//@[16:17] NewLine |\n|
  maxLength: a + 2
//@[2:11] Identifier |maxLength|
//@[11:12] Colon |:|
//@[13:14] Identifier |a|
//@[15:16] Plus |+|
//@[17:18] Number |2|
//@[18:19] NewLine |\n|
  minLength: foo()
//@[2:11] Identifier |minLength|
//@[11:12] Colon |:|
//@[13:16] Identifier |foo|
//@[16:17] LeftParen |(|
//@[17:18] RightParen |)|
//@[18:19] NewLine |\n|
  allowedValues: [
//@[2:15] Identifier |allowedValues|
//@[15:16] Colon |:|
//@[17:18] LeftSquare |[|
//@[18:19] NewLine |\n|
    i
//@[4:5] Identifier |i|
//@[5:6] NewLine |\n|
  ]
//@[2:3] RightSquare |]|
//@[3:4] NewLine |\n|
}
//@[0:1] RightBrace |}|
//@[1:3] NewLine |\n\n|

// 1-cycle in params
//@[20:21] NewLine |\n|
param paramDefaultOneCycle string = paramDefaultOneCycle
//@[0:5] Identifier |param|
//@[6:26] Identifier |paramDefaultOneCycle|
//@[27:33] Identifier |string|
//@[34:35] Assignment |=|
//@[36:56] Identifier |paramDefaultOneCycle|
//@[56:58] NewLine |\n\n|

// 2-cycle in params
//@[20:21] NewLine |\n|
param paramDefaultTwoCycle1 string = paramDefaultTwoCycle2
//@[0:5] Identifier |param|
//@[6:27] Identifier |paramDefaultTwoCycle1|
//@[28:34] Identifier |string|
//@[35:36] Assignment |=|
//@[37:58] Identifier |paramDefaultTwoCycle2|
//@[58:59] NewLine |\n|
param paramDefaultTwoCycle2 string = paramDefaultTwoCycle1
//@[0:5] Identifier |param|
//@[6:27] Identifier |paramDefaultTwoCycle2|
//@[28:34] Identifier |string|
//@[35:36] Assignment |=|
//@[37:58] Identifier |paramDefaultTwoCycle1|
//@[58:60] NewLine |\n\n|

// 1-cycle in modifier params
//@[29:30] NewLine |\n|
param paramModifierOneCycle string {
//@[0:5] Identifier |param|
//@[6:27] Identifier |paramModifierOneCycle|
//@[28:34] Identifier |string|
//@[35:36] LeftBrace |{|
//@[36:37] NewLine |\n|
  default: paramModifierOneCycle
//@[2:9] Identifier |default|
//@[9:10] Colon |:|
//@[11:32] Identifier |paramModifierOneCycle|
//@[32:33] NewLine |\n|
}
//@[0:1] RightBrace |}|
//@[1:3] NewLine |\n\n|

// 1-cycle in modifier with non-default property
//@[48:49] NewLine |\n|
param paramModifierSelfCycle string {
//@[0:5] Identifier |param|
//@[6:28] Identifier |paramModifierSelfCycle|
//@[29:35] Identifier |string|
//@[36:37] LeftBrace |{|
//@[37:38] NewLine |\n|
  allowedValues: [
//@[2:15] Identifier |allowedValues|
//@[15:16] Colon |:|
//@[17:18] LeftSquare |[|
//@[18:19] NewLine |\n|
    paramModifierSelfCycle
//@[4:26] Identifier |paramModifierSelfCycle|
//@[26:27] NewLine |\n|
  ]
//@[2:3] RightSquare |]|
//@[3:4] NewLine |\n|
}
//@[0:1] RightBrace |}|
//@[1:3] NewLine |\n\n|

// 2-cycle in modifier params
//@[29:30] NewLine |\n|
param paramModifierTwoCycle1 string {
//@[0:5] Identifier |param|
//@[6:28] Identifier |paramModifierTwoCycle1|
//@[29:35] Identifier |string|
//@[36:37] LeftBrace |{|
//@[37:38] NewLine |\n|
  default: paramModifierTwoCycle2
//@[2:9] Identifier |default|
//@[9:10] Colon |:|
//@[11:33] Identifier |paramModifierTwoCycle2|
//@[33:34] NewLine |\n|
}
//@[0:1] RightBrace |}|
//@[1:2] NewLine |\n|
param paramModifierTwoCycle2 string {
//@[0:5] Identifier |param|
//@[6:28] Identifier |paramModifierTwoCycle2|
//@[29:35] Identifier |string|
//@[36:37] LeftBrace |{|
//@[37:38] NewLine |\n|
  default: paramModifierTwoCycle1
//@[2:9] Identifier |default|
//@[9:10] Colon |:|
//@[11:33] Identifier |paramModifierTwoCycle1|
//@[33:34] NewLine |\n|
}
//@[0:1] RightBrace |}|
//@[1:3] NewLine |\n\n|

// 2-cycle mixed param syntaxes
//@[31:32] NewLine |\n|
param paramMixedTwoCycle1 string = paramMixedTwoCycle2
//@[0:5] Identifier |param|
//@[6:25] Identifier |paramMixedTwoCycle1|
//@[26:32] Identifier |string|
//@[33:34] Assignment |=|
//@[35:54] Identifier |paramMixedTwoCycle2|
//@[54:55] NewLine |\n|
param paramMixedTwoCycle2 string {
//@[0:5] Identifier |param|
//@[6:25] Identifier |paramMixedTwoCycle2|
//@[26:32] Identifier |string|
//@[33:34] LeftBrace |{|
//@[34:35] NewLine |\n|
  default: paramMixedTwoCycle1
//@[2:9] Identifier |default|
//@[9:10] Colon |:|
//@[11:30] Identifier |paramMixedTwoCycle1|
//@[30:31] NewLine |\n|
}
//@[0:1] RightBrace |}|
//@[1:3] NewLine |\n\n|

// unterminated multi-line comment
//@[34:35] NewLine |\n|
/*    
//@[6:6] EndOfFile ||
