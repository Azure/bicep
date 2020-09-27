/* 
  Valid and invalid code is mixed together to validate recovery logic. It can even contain ** * *** **.
*/
//@[2:4) NewLine |\n\n|

param myString string
//@[0:21) ParameterDeclarationSyntax
//@[0:5)  Identifier |param|
//@[6:14)  IdentifierSyntax
//@[6:14)   Identifier |myString|
//@[15:21)  TypeSyntax
//@[15:21)   Identifier |string|
//@[21:22) NewLine |\n|
wrong
//@[0:5) SkippedTriviaSyntax
//@[0:5)  Identifier |wrong|
//@[5:7) NewLine |\n\n|

param myInt int
//@[0:15) ParameterDeclarationSyntax
//@[0:5)  Identifier |param|
//@[6:11)  IdentifierSyntax
//@[6:11)   Identifier |myInt|
//@[12:15)  TypeSyntax
//@[12:15)   Identifier |int|
//@[15:16) NewLine |\n|
param
//@[0:5) SkippedTriviaSyntax
//@[0:5)  Identifier |param|
//@[5:7) NewLine |\n\n|

param myBool bool
//@[0:17) ParameterDeclarationSyntax
//@[0:5)  Identifier |param|
//@[6:12)  IdentifierSyntax
//@[6:12)   Identifier |myBool|
//@[13:17)  TypeSyntax
//@[13:17)   Identifier |bool|
//@[17:19) NewLine |\n\n|

param missingType
//@[0:17) SkippedTriviaSyntax
//@[0:5)  Identifier |param|
//@[6:17)  Identifier |missingType|
//@[17:19) NewLine |\n\n|

param myString2 string = 'string value'
//@[0:39) ParameterDeclarationSyntax
//@[0:5)  Identifier |param|
//@[6:15)  IdentifierSyntax
//@[6:15)   Identifier |myString2|
//@[16:22)  TypeSyntax
//@[16:22)   Identifier |string|
//@[23:39)  ParameterDefaultValueSyntax
//@[23:24)   Assignment |=|
//@[25:39)   StringSyntax
//@[25:39)    StringComplete |'string value'|
//@[39:41) NewLine |\n\n|

param wrongDefaultValue string = 42
//@[0:35) ParameterDeclarationSyntax
//@[0:5)  Identifier |param|
//@[6:23)  IdentifierSyntax
//@[6:23)   Identifier |wrongDefaultValue|
//@[24:30)  TypeSyntax
//@[24:30)   Identifier |string|
//@[31:35)  ParameterDefaultValueSyntax
//@[31:32)   Assignment |=|
//@[33:35)   NumericLiteralSyntax
//@[33:35)    Number |42|
//@[35:37) NewLine |\n\n|

param myInt2 int = 42
//@[0:21) ParameterDeclarationSyntax
//@[0:5)  Identifier |param|
//@[6:12)  IdentifierSyntax
//@[6:12)   Identifier |myInt2|
//@[13:16)  TypeSyntax
//@[13:16)   Identifier |int|
//@[17:21)  ParameterDefaultValueSyntax
//@[17:18)   Assignment |=|
//@[19:21)   NumericLiteralSyntax
//@[19:21)    Number |42|
//@[21:22) NewLine |\n|
param noValueAfterColon int =   
//@[0:29) SkippedTriviaSyntax
//@[0:5)  Identifier |param|
//@[6:23)  Identifier |noValueAfterColon|
//@[24:27)  Identifier |int|
//@[28:29)  Assignment |=|
//@[32:34) NewLine |\n\n|

param myTruth bool = 'not a boolean'
//@[0:36) ParameterDeclarationSyntax
//@[0:5)  Identifier |param|
//@[6:13)  IdentifierSyntax
//@[6:13)   Identifier |myTruth|
//@[14:18)  TypeSyntax
//@[14:18)   Identifier |bool|
//@[19:36)  ParameterDefaultValueSyntax
//@[19:20)   Assignment |=|
//@[21:36)   StringSyntax
//@[21:36)    StringComplete |'not a boolean'|
//@[36:37) NewLine |\n|
param myFalsehood bool = 'false'
//@[0:32) ParameterDeclarationSyntax
//@[0:5)  Identifier |param|
//@[6:17)  IdentifierSyntax
//@[6:17)   Identifier |myFalsehood|
//@[18:22)  TypeSyntax
//@[18:22)   Identifier |bool|
//@[23:32)  ParameterDefaultValueSyntax
//@[23:24)   Assignment |=|
//@[25:32)   StringSyntax
//@[25:32)    StringComplete |'false'|
//@[32:34) NewLine |\n\n|

param wrongAssignmentToken string: 'hello'
//@[0:42) SkippedTriviaSyntax
//@[0:5)  Identifier |param|
//@[6:26)  Identifier |wrongAssignmentToken|
//@[27:33)  Identifier |string|
//@[33:34)  Colon |:|
//@[35:42)  StringComplete |'hello'|
//@[42:44) NewLine |\n\n|

param WhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLong string = 'why not?'
//@[0:287) ParameterDeclarationSyntax
//@[0:5)  Identifier |param|
//@[6:267)  IdentifierSyntax
//@[6:267)   Identifier |WhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLongWhySoLong|
//@[268:274)  TypeSyntax
//@[268:274)   Identifier |string|
//@[275:287)  ParameterDefaultValueSyntax
//@[275:276)   Assignment |=|
//@[277:287)   StringSyntax
//@[277:287)    StringComplete |'why not?'|
//@[287:289) NewLine |\n\n|

// badly escaped string
//@[23:24) NewLine |\n|
param wrongType fluffyBunny = 'what's up doc?'
//@[0:36) ParameterDeclarationSyntax
//@[0:5)  Identifier |param|
//@[6:15)  IdentifierSyntax
//@[6:15)   Identifier |wrongType|
//@[16:27)  TypeSyntax
//@[16:27)   Identifier |fluffyBunny|
//@[28:36)  ParameterDefaultValueSyntax
//@[28:29)   Assignment |=|
//@[30:36)   StringSyntax
//@[30:36)    StringComplete |'what'|
//@[36:48) SkippedTriviaSyntax
//@[36:37)  Identifier |s|
//@[38:40)  Identifier |up|
//@[41:44)  Identifier |doc|
//@[44:45)  Question |?|
//@[45:46)  StringComplete |'|
//@[46:48)  NewLine |\n\n|

// invalid escape
//@[17:18) NewLine |\n|
param wrongType fluffyBunny = 'what\s up doc?'
//@[0:46) ParameterDeclarationSyntax
//@[0:5)  Identifier |param|
//@[6:15)  IdentifierSyntax
//@[6:15)   Identifier |wrongType|
//@[16:27)  TypeSyntax
//@[16:27)   Identifier |fluffyBunny|
//@[28:46)  ParameterDefaultValueSyntax
//@[28:29)   Assignment |=|
//@[30:46)   SkippedTriviaSyntax
//@[30:46)    StringComplete |'what\s up doc?'|
//@[46:48) NewLine |\n\n|

// unterminated string 
//@[23:24) NewLine |\n|
param wrongType fluffyBunny = 'what\'s up doc?
//@[0:46) ParameterDeclarationSyntax
//@[0:5)  Identifier |param|
//@[6:15)  IdentifierSyntax
//@[6:15)   Identifier |wrongType|
//@[16:27)  TypeSyntax
//@[16:27)   Identifier |fluffyBunny|
//@[28:46)  ParameterDefaultValueSyntax
//@[28:29)   Assignment |=|
//@[30:46)   SkippedTriviaSyntax
//@[30:46)    StringComplete |'what\'s up doc?|
//@[46:48) NewLine |\n\n|

// unterminated interpolated string
//@[35:36) NewLine |\n|
param wrongType fluffyBunny = 'what\'s ${
//@[0:41) ParameterDeclarationSyntax
//@[0:5)  Identifier |param|
//@[6:15)  IdentifierSyntax
//@[6:15)   Identifier |wrongType|
//@[16:27)  TypeSyntax
//@[16:27)   Identifier |fluffyBunny|
//@[28:41)  ParameterDefaultValueSyntax
//@[28:29)   Assignment |=|
//@[30:41)   SkippedTriviaSyntax
//@[30:41)    StringLeftPiece |'what\'s ${|
//@[41:41)    SkippedTriviaSyntax
//@[41:41)    StringRightPiece ||
//@[41:42) NewLine |\n|
param wrongType fluffyBunny = 'what\'s ${up
//@[0:43) ParameterDeclarationSyntax
//@[0:5)  Identifier |param|
//@[6:15)  IdentifierSyntax
//@[6:15)   Identifier |wrongType|
//@[16:27)  TypeSyntax
//@[16:27)   Identifier |fluffyBunny|
//@[28:43)  ParameterDefaultValueSyntax
//@[28:29)   Assignment |=|
//@[30:43)   SkippedTriviaSyntax
//@[30:41)    StringLeftPiece |'what\'s ${|
//@[41:43)    VariableAccessSyntax
//@[41:43)     IdentifierSyntax
//@[41:43)      Identifier |up|
//@[43:43)    StringRightPiece ||
//@[43:44) NewLine |\n|
param wrongType fluffyBunny = 'what\'s ${up}
//@[0:44) ParameterDeclarationSyntax
//@[0:5)  Identifier |param|
//@[6:15)  IdentifierSyntax
//@[6:15)   Identifier |wrongType|
//@[16:27)  TypeSyntax
//@[16:27)   Identifier |fluffyBunny|
//@[28:44)  ParameterDefaultValueSyntax
//@[28:29)   Assignment |=|
//@[30:44)   SkippedTriviaSyntax
//@[30:41)    StringLeftPiece |'what\'s ${|
//@[41:43)    VariableAccessSyntax
//@[41:43)     IdentifierSyntax
//@[41:43)      Identifier |up|
//@[43:44)    StringRightPiece |}|
//@[44:45) NewLine |\n|
param wrongType fluffyBunny = 'what\'s ${'up
//@[0:44) ParameterDeclarationSyntax
//@[0:5)  Identifier |param|
//@[6:15)  IdentifierSyntax
//@[6:15)   Identifier |wrongType|
//@[16:27)  TypeSyntax
//@[16:27)   Identifier |fluffyBunny|
//@[28:44)  ParameterDefaultValueSyntax
//@[28:29)   Assignment |=|
//@[30:44)   SkippedTriviaSyntax
//@[30:41)    StringLeftPiece |'what\'s ${|
//@[41:44)    SkippedTriviaSyntax
//@[41:44)     StringComplete |'up|
//@[44:44)    StringRightPiece ||
//@[44:46) NewLine |\n\n|

// unterminated nested interpolated string
//@[42:43) NewLine |\n|
param wrongType fluffyBunny = 'what\'s ${'up${
//@[0:46) ParameterDeclarationSyntax
//@[0:5)  Identifier |param|
//@[6:15)  IdentifierSyntax
//@[6:15)   Identifier |wrongType|
//@[16:27)  TypeSyntax
//@[16:27)   Identifier |fluffyBunny|
//@[28:46)  ParameterDefaultValueSyntax
//@[28:29)   Assignment |=|
//@[30:46)   SkippedTriviaSyntax
//@[30:41)    StringLeftPiece |'what\'s ${|
//@[41:46)    SkippedTriviaSyntax
//@[41:46)     StringLeftPiece |'up${|
//@[46:46)     SkippedTriviaSyntax
//@[46:46)     StringRightPiece ||
//@[46:47) NewLine |\n|
param wrongType fluffyBunny = 'what\'s ${'up${
//@[0:46) ParameterDeclarationSyntax
//@[0:5)  Identifier |param|
//@[6:15)  IdentifierSyntax
//@[6:15)   Identifier |wrongType|
//@[16:27)  TypeSyntax
//@[16:27)   Identifier |fluffyBunny|
//@[28:46)  ParameterDefaultValueSyntax
//@[28:29)   Assignment |=|
//@[30:46)   SkippedTriviaSyntax
//@[30:41)    StringLeftPiece |'what\'s ${|
//@[41:46)    SkippedTriviaSyntax
//@[41:46)     StringLeftPiece |'up${|
//@[46:46)     SkippedTriviaSyntax
//@[46:46)     StringRightPiece ||
//@[46:47) NewLine |\n|
param wrongType fluffyBunny = 'what\'s ${'up${doc
//@[0:49) ParameterDeclarationSyntax
//@[0:5)  Identifier |param|
//@[6:15)  IdentifierSyntax
//@[6:15)   Identifier |wrongType|
//@[16:27)  TypeSyntax
//@[16:27)   Identifier |fluffyBunny|
//@[28:49)  ParameterDefaultValueSyntax
//@[28:29)   Assignment |=|
//@[30:49)   SkippedTriviaSyntax
//@[30:41)    StringLeftPiece |'what\'s ${|
//@[41:49)    SkippedTriviaSyntax
//@[41:46)     StringLeftPiece |'up${|
//@[46:49)     VariableAccessSyntax
//@[46:49)      IdentifierSyntax
//@[46:49)       Identifier |doc|
//@[49:49)     StringRightPiece ||
//@[49:50) NewLine |\n|
param wrongType fluffyBunny = 'what\'s ${'up${doc}
//@[0:50) ParameterDeclarationSyntax
//@[0:5)  Identifier |param|
//@[6:15)  IdentifierSyntax
//@[6:15)   Identifier |wrongType|
//@[16:27)  TypeSyntax
//@[16:27)   Identifier |fluffyBunny|
//@[28:50)  ParameterDefaultValueSyntax
//@[28:29)   Assignment |=|
//@[30:50)   SkippedTriviaSyntax
//@[30:41)    StringLeftPiece |'what\'s ${|
//@[41:50)    SkippedTriviaSyntax
//@[41:46)     StringLeftPiece |'up${|
//@[46:49)     VariableAccessSyntax
//@[46:49)      IdentifierSyntax
//@[46:49)       Identifier |doc|
//@[49:50)     StringRightPiece |}|
//@[50:50)    StringRightPiece ||
//@[50:51) NewLine |\n|
param wrongType fluffyBunny = 'what\'s ${'up${doc}'
//@[0:51) ParameterDeclarationSyntax
//@[0:5)  Identifier |param|
//@[6:15)  IdentifierSyntax
//@[6:15)   Identifier |wrongType|
//@[16:27)  TypeSyntax
//@[16:27)   Identifier |fluffyBunny|
//@[28:51)  ParameterDefaultValueSyntax
//@[28:29)   Assignment |=|
//@[30:51)   SkippedTriviaSyntax
//@[30:41)    StringLeftPiece |'what\'s ${|
//@[41:51)    StringSyntax
//@[41:46)     StringLeftPiece |'up${|
//@[46:49)     VariableAccessSyntax
//@[46:49)      IdentifierSyntax
//@[46:49)       Identifier |doc|
//@[49:51)     StringRightPiece |}'|
//@[51:51)    StringRightPiece ||
//@[51:52) NewLine |\n|
param wrongType fluffyBunny = 'what\'s ${'up${doc}'}?
//@[0:53) ParameterDeclarationSyntax
//@[0:5)  Identifier |param|
//@[6:15)  IdentifierSyntax
//@[6:15)   Identifier |wrongType|
//@[16:27)  TypeSyntax
//@[16:27)   Identifier |fluffyBunny|
//@[28:53)  ParameterDefaultValueSyntax
//@[28:29)   Assignment |=|
//@[30:53)   SkippedTriviaSyntax
//@[30:41)    StringLeftPiece |'what\'s ${|
//@[41:51)    StringSyntax
//@[41:46)     StringLeftPiece |'up${|
//@[46:49)     VariableAccessSyntax
//@[46:49)      IdentifierSyntax
//@[46:49)       Identifier |doc|
//@[49:51)     StringRightPiece |}'|
//@[51:53)    StringRightPiece |}?|
//@[53:55) NewLine |\n\n|

// object literal inside interpolated string
//@[44:45) NewLine |\n|
param wrongType fluffyBunny = '${{this: doesnt}.work}'
//@[0:54) ParameterDeclarationSyntax
//@[0:5)  Identifier |param|
//@[6:15)  IdentifierSyntax
//@[6:15)   Identifier |wrongType|
//@[16:27)  TypeSyntax
//@[16:27)   Identifier |fluffyBunny|
//@[28:54)  ParameterDefaultValueSyntax
//@[28:29)   Assignment |=|
//@[30:54)   SkippedTriviaSyntax
//@[30:33)    StringLeftPiece |'${|
//@[33:54)    SkippedTriviaSyntax
//@[33:34)     LeftBrace |{|
//@[34:38)     Identifier |this|
//@[38:39)     Colon |:|
//@[40:46)     Identifier |doesnt|
//@[46:47)     RightBrace |}|
//@[47:48)     Dot |.|
//@[48:52)     Identifier |work|
//@[52:53)     RightBrace |}|
//@[53:54)     StringComplete |'|
//@[54:54)    StringRightPiece ||
//@[54:56) NewLine |\n\n|

// bad interpolated string format
//@[33:34) NewLine |\n|
param badInterpolatedString string = 'hello ${}!'
//@[0:49) ParameterDeclarationSyntax
//@[0:5)  Identifier |param|
//@[6:27)  IdentifierSyntax
//@[6:27)   Identifier |badInterpolatedString|
//@[28:34)  TypeSyntax
//@[28:34)   Identifier |string|
//@[35:49)  ParameterDefaultValueSyntax
//@[35:36)   Assignment |=|
//@[37:49)   StringSyntax
//@[37:46)    StringLeftPiece |'hello ${|
//@[46:46)    SkippedTriviaSyntax
//@[46:49)    StringRightPiece |}!'|
//@[49:50) NewLine |\n|
param badInterpolatedString2 string = 'hello ${a b c}!'
//@[0:55) ParameterDeclarationSyntax
//@[0:5)  Identifier |param|
//@[6:28)  IdentifierSyntax
//@[6:28)   Identifier |badInterpolatedString2|
//@[29:35)  TypeSyntax
//@[29:35)   Identifier |string|
//@[36:55)  ParameterDefaultValueSyntax
//@[36:37)   Assignment |=|
//@[38:55)   StringSyntax
//@[38:47)    StringLeftPiece |'hello ${|
//@[47:52)    SkippedTriviaSyntax
//@[47:48)     VariableAccessSyntax
//@[47:48)      IdentifierSyntax
//@[47:48)       Identifier |a|
//@[49:52)     SkippedTriviaSyntax
//@[49:50)      Identifier |b|
//@[51:52)      Identifier |c|
//@[52:55)    StringRightPiece |}!'|
//@[55:57) NewLine |\n\n|

param wrongType fluffyBunny = 'what\'s up doc?'
//@[0:47) ParameterDeclarationSyntax
//@[0:5)  Identifier |param|
//@[6:15)  IdentifierSyntax
//@[6:15)   Identifier |wrongType|
//@[16:27)  TypeSyntax
//@[16:27)   Identifier |fluffyBunny|
//@[28:47)  ParameterDefaultValueSyntax
//@[28:29)   Assignment |=|
//@[30:47)   StringSyntax
//@[30:47)    StringComplete |'what\'s up doc?'|
//@[47:49) NewLine |\n\n|

// modifier on an invalid type
//@[30:31) NewLine |\n|
param someArray arra {
//@[0:55) ParameterDeclarationSyntax
//@[0:5)  Identifier |param|
//@[6:15)  IdentifierSyntax
//@[6:15)   Identifier |someArray|
//@[16:20)  TypeSyntax
//@[16:20)   Identifier |arra|
//@[21:55)  ObjectSyntax
//@[21:22)   LeftBrace |{|
//@[22:23)   NewLine |\n|
  minLength: 3
//@[2:15)   ObjectPropertySyntax
//@[2:11)    IdentifierSyntax
//@[2:11)     Identifier |minLength|
//@[11:12)    Colon |:|
//@[13:14)    NumericLiteralSyntax
//@[13:14)     Number |3|
//@[14:15)    NewLine |\n|
  maxLength: 24
//@[2:16)   ObjectPropertySyntax
//@[2:11)    IdentifierSyntax
//@[2:11)     Identifier |maxLength|
//@[11:12)    Colon |:|
//@[13:15)    NumericLiteralSyntax
//@[13:15)     Number |24|
//@[15:16)    NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

// duplicate modifier property
//@[30:31) NewLine |\n|
param duplicatedModifierProperty string {
//@[0:74) ParameterDeclarationSyntax
//@[0:5)  Identifier |param|
//@[6:32)  IdentifierSyntax
//@[6:32)   Identifier |duplicatedModifierProperty|
//@[33:39)  TypeSyntax
//@[33:39)   Identifier |string|
//@[40:74)  ObjectSyntax
//@[40:41)   LeftBrace |{|
//@[41:42)   NewLine |\n|
  minLength: 3
//@[2:15)   ObjectPropertySyntax
//@[2:11)    IdentifierSyntax
//@[2:11)     Identifier |minLength|
//@[11:12)    Colon |:|
//@[13:14)    NumericLiteralSyntax
//@[13:14)     Number |3|
//@[14:15)    NewLine |\n|
  minLength: 24
//@[2:16)   ObjectPropertySyntax
//@[2:11)    IdentifierSyntax
//@[2:11)     Identifier |minLength|
//@[11:12)    Colon |:|
//@[13:15)    NumericLiteralSyntax
//@[13:15)     Number |24|
//@[15:16)    NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

// non-existent modifiers
//@[25:26) NewLine |\n|
param secureInt int {
//@[0:70) ParameterDeclarationSyntax
//@[0:5)  Identifier |param|
//@[6:15)  IdentifierSyntax
//@[6:15)   Identifier |secureInt|
//@[16:19)  TypeSyntax
//@[16:19)   Identifier |int|
//@[20:70)  ObjectSyntax
//@[20:21)   LeftBrace |{|
//@[21:22)   NewLine |\n|
  secure: true
//@[2:15)   ObjectPropertySyntax
//@[2:8)    IdentifierSyntax
//@[2:8)     Identifier |secure|
//@[8:9)    Colon |:|
//@[10:14)    BooleanLiteralSyntax
//@[10:14)     TrueKeyword |true|
//@[14:15)    NewLine |\n|
  minLength: 3
//@[2:15)   ObjectPropertySyntax
//@[2:11)    IdentifierSyntax
//@[2:11)     Identifier |minLength|
//@[11:12)    Colon |:|
//@[13:14)    NumericLiteralSyntax
//@[13:14)     Number |3|
//@[14:15)    NewLine |\n|
  maxLength: 123
//@[2:17)   ObjectPropertySyntax
//@[2:11)    IdentifierSyntax
//@[2:11)     Identifier |maxLength|
//@[11:12)    Colon |:|
//@[13:16)    NumericLiteralSyntax
//@[13:16)     Number |123|
//@[16:17)    NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

// wrong modifier value types
//@[29:30) NewLine |\n|
param wrongIntModifier int {
//@[0:139) ParameterDeclarationSyntax
//@[0:5)  Identifier |param|
//@[6:22)  IdentifierSyntax
//@[6:22)   Identifier |wrongIntModifier|
//@[23:26)  TypeSyntax
//@[23:26)   Identifier |int|
//@[27:139)  ObjectSyntax
//@[27:28)   LeftBrace |{|
//@[28:29)   NewLine |\n|
  default: true
//@[2:16)   ObjectPropertySyntax
//@[2:9)    IdentifierSyntax
//@[2:9)     Identifier |default|
//@[9:10)    Colon |:|
//@[11:15)    BooleanLiteralSyntax
//@[11:15)     TrueKeyword |true|
//@[15:16)    NewLine |\n|
  allowed: [
//@[2:37)   ObjectPropertySyntax
//@[2:9)    IdentifierSyntax
//@[2:9)     Identifier |allowed|
//@[9:10)    Colon |:|
//@[11:36)    ArraySyntax
//@[11:12)     LeftSquare |[|
//@[12:13)     NewLine |\n|
    'test'
//@[4:11)     ArrayItemSyntax
//@[4:10)      StringSyntax
//@[4:10)       StringComplete |'test'|
//@[10:11)      NewLine |\n|
    true
//@[4:9)     ArrayItemSyntax
//@[4:8)      BooleanLiteralSyntax
//@[4:8)       TrueKeyword |true|
//@[8:9)      NewLine |\n|
  ]
//@[2:3)     RightSquare |]|
//@[3:4)    NewLine |\n|
  minValue: {
//@[2:18)   ObjectPropertySyntax
//@[2:10)    IdentifierSyntax
//@[2:10)     Identifier |minValue|
//@[10:11)    Colon |:|
//@[12:17)    ObjectSyntax
//@[12:13)     LeftBrace |{|
//@[13:14)     NewLine |\n|
  }
//@[2:3)     RightBrace |}|
//@[3:4)    NewLine |\n|
  maxValue: [
//@[2:18)   ObjectPropertySyntax
//@[2:10)    IdentifierSyntax
//@[2:10)     Identifier |maxValue|
//@[10:11)    Colon |:|
//@[12:17)    ArraySyntax
//@[12:13)     LeftSquare |[|
//@[13:14)     NewLine |\n|
  ]
//@[2:3)     RightSquare |]|
//@[3:4)    NewLine |\n|
  metadata: 'wrong'
//@[2:20)   ObjectPropertySyntax
//@[2:10)    IdentifierSyntax
//@[2:10)     Identifier |metadata|
//@[10:11)    Colon |:|
//@[12:19)    StringSyntax
//@[12:19)     StringComplete |'wrong'|
//@[19:20)    NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

// wrong metadata schema
//@[24:25) NewLine |\n|
param wrongMetadataSchema string {
//@[0:76) ParameterDeclarationSyntax
//@[0:5)  Identifier |param|
//@[6:25)  IdentifierSyntax
//@[6:25)   Identifier |wrongMetadataSchema|
//@[26:32)  TypeSyntax
//@[26:32)   Identifier |string|
//@[33:76)  ObjectSyntax
//@[33:34)   LeftBrace |{|
//@[34:35)   NewLine |\n|
  metadata: {
//@[2:40)   ObjectPropertySyntax
//@[2:10)    IdentifierSyntax
//@[2:10)     Identifier |metadata|
//@[10:11)    Colon |:|
//@[12:39)    ObjectSyntax
//@[12:13)     LeftBrace |{|
//@[13:14)     NewLine |\n|
    description: true
//@[4:22)     ObjectPropertySyntax
//@[4:15)      IdentifierSyntax
//@[4:15)       Identifier |description|
//@[15:16)      Colon |:|
//@[17:21)      BooleanLiteralSyntax
//@[17:21)       TrueKeyword |true|
//@[21:22)      NewLine |\n|
  }
//@[2:3)     RightBrace |}|
//@[3:4)    NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

// expression in modifier
//@[25:26) NewLine |\n|
param expressionInModifier string {
//@[0:115) ParameterDeclarationSyntax
//@[0:5)  Identifier |param|
//@[6:26)  IdentifierSyntax
//@[6:26)   Identifier |expressionInModifier|
//@[27:33)  TypeSyntax
//@[27:33)   Identifier |string|
//@[34:115)  ObjectSyntax
//@[34:35)   LeftBrace |{|
//@[35:36)   NewLine |\n|
  default: 2 + 3
//@[2:17)   ObjectPropertySyntax
//@[2:9)    IdentifierSyntax
//@[2:9)     Identifier |default|
//@[9:10)    Colon |:|
//@[11:16)    BinaryOperationSyntax
//@[11:12)     NumericLiteralSyntax
//@[11:12)      Number |2|
//@[13:14)     Plus |+|
//@[15:16)     NumericLiteralSyntax
//@[15:16)      Number |3|
//@[16:17)    NewLine |\n|
  maxLength: a + 2
//@[2:19)   ObjectPropertySyntax
//@[2:11)    IdentifierSyntax
//@[2:11)     Identifier |maxLength|
//@[11:12)    Colon |:|
//@[13:18)    BinaryOperationSyntax
//@[13:14)     VariableAccessSyntax
//@[13:14)      IdentifierSyntax
//@[13:14)       Identifier |a|
//@[15:16)     Plus |+|
//@[17:18)     NumericLiteralSyntax
//@[17:18)      Number |2|
//@[18:19)    NewLine |\n|
  minLength: foo()
//@[2:19)   ObjectPropertySyntax
//@[2:11)    IdentifierSyntax
//@[2:11)     Identifier |minLength|
//@[11:12)    Colon |:|
//@[13:18)    FunctionCallSyntax
//@[13:16)     IdentifierSyntax
//@[13:16)      Identifier |foo|
//@[16:17)     LeftParen |(|
//@[17:18)     RightParen |)|
//@[18:19)    NewLine |\n|
  allowed: [
//@[2:23)   ObjectPropertySyntax
//@[2:9)    IdentifierSyntax
//@[2:9)     Identifier |allowed|
//@[9:10)    Colon |:|
//@[11:22)    ArraySyntax
//@[11:12)     LeftSquare |[|
//@[12:13)     NewLine |\n|
    i
//@[4:6)     ArrayItemSyntax
//@[4:5)      VariableAccessSyntax
//@[4:5)       IdentifierSyntax
//@[4:5)        Identifier |i|
//@[5:6)      NewLine |\n|
  ]
//@[2:3)     RightSquare |]|
//@[3:4)    NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

// 1-cycle in params
//@[20:21) NewLine |\n|
param paramDefaultOneCycle string = paramDefaultOneCycle
//@[0:56) ParameterDeclarationSyntax
//@[0:5)  Identifier |param|
//@[6:26)  IdentifierSyntax
//@[6:26)   Identifier |paramDefaultOneCycle|
//@[27:33)  TypeSyntax
//@[27:33)   Identifier |string|
//@[34:56)  ParameterDefaultValueSyntax
//@[34:35)   Assignment |=|
//@[36:56)   VariableAccessSyntax
//@[36:56)    IdentifierSyntax
//@[36:56)     Identifier |paramDefaultOneCycle|
//@[56:58) NewLine |\n\n|

// 2-cycle in params
//@[20:21) NewLine |\n|
param paramDefaultTwoCycle1 string = paramDefaultTwoCycle2
//@[0:58) ParameterDeclarationSyntax
//@[0:5)  Identifier |param|
//@[6:27)  IdentifierSyntax
//@[6:27)   Identifier |paramDefaultTwoCycle1|
//@[28:34)  TypeSyntax
//@[28:34)   Identifier |string|
//@[35:58)  ParameterDefaultValueSyntax
//@[35:36)   Assignment |=|
//@[37:58)   VariableAccessSyntax
//@[37:58)    IdentifierSyntax
//@[37:58)     Identifier |paramDefaultTwoCycle2|
//@[58:59) NewLine |\n|
param paramDefaultTwoCycle2 string = paramDefaultTwoCycle1
//@[0:58) ParameterDeclarationSyntax
//@[0:5)  Identifier |param|
//@[6:27)  IdentifierSyntax
//@[6:27)   Identifier |paramDefaultTwoCycle2|
//@[28:34)  TypeSyntax
//@[28:34)   Identifier |string|
//@[35:58)  ParameterDefaultValueSyntax
//@[35:36)   Assignment |=|
//@[37:58)   VariableAccessSyntax
//@[37:58)    IdentifierSyntax
//@[37:58)     Identifier |paramDefaultTwoCycle1|
//@[58:60) NewLine |\n\n|

// 1-cycle in modifier params
//@[29:30) NewLine |\n|
param paramModifierOneCycle string {
//@[0:71) ParameterDeclarationSyntax
//@[0:5)  Identifier |param|
//@[6:27)  IdentifierSyntax
//@[6:27)   Identifier |paramModifierOneCycle|
//@[28:34)  TypeSyntax
//@[28:34)   Identifier |string|
//@[35:71)  ObjectSyntax
//@[35:36)   LeftBrace |{|
//@[36:37)   NewLine |\n|
  default: paramModifierOneCycle
//@[2:33)   ObjectPropertySyntax
//@[2:9)    IdentifierSyntax
//@[2:9)     Identifier |default|
//@[9:10)    Colon |:|
//@[11:32)    VariableAccessSyntax
//@[11:32)     IdentifierSyntax
//@[11:32)      Identifier |paramModifierOneCycle|
//@[32:33)    NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

// 1-cycle in modifier with non-default property
//@[48:49) NewLine |\n|
param paramModifierSelfCycle string {
//@[0:83) ParameterDeclarationSyntax
//@[0:5)  Identifier |param|
//@[6:28)  IdentifierSyntax
//@[6:28)   Identifier |paramModifierSelfCycle|
//@[29:35)  TypeSyntax
//@[29:35)   Identifier |string|
//@[36:83)  ObjectSyntax
//@[36:37)   LeftBrace |{|
//@[37:38)   NewLine |\n|
  allowed: [
//@[2:44)   ObjectPropertySyntax
//@[2:9)    IdentifierSyntax
//@[2:9)     Identifier |allowed|
//@[9:10)    Colon |:|
//@[11:43)    ArraySyntax
//@[11:12)     LeftSquare |[|
//@[12:13)     NewLine |\n|
    paramModifierSelfCycle
//@[4:27)     ArrayItemSyntax
//@[4:26)      VariableAccessSyntax
//@[4:26)       IdentifierSyntax
//@[4:26)        Identifier |paramModifierSelfCycle|
//@[26:27)      NewLine |\n|
  ]
//@[2:3)     RightSquare |]|
//@[3:4)    NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

// 2-cycle in modifier params
//@[29:30) NewLine |\n|
param paramModifierTwoCycle1 string {
//@[0:73) ParameterDeclarationSyntax
//@[0:5)  Identifier |param|
//@[6:28)  IdentifierSyntax
//@[6:28)   Identifier |paramModifierTwoCycle1|
//@[29:35)  TypeSyntax
//@[29:35)   Identifier |string|
//@[36:73)  ObjectSyntax
//@[36:37)   LeftBrace |{|
//@[37:38)   NewLine |\n|
  default: paramModifierTwoCycle2
//@[2:34)   ObjectPropertySyntax
//@[2:9)    IdentifierSyntax
//@[2:9)     Identifier |default|
//@[9:10)    Colon |:|
//@[11:33)    VariableAccessSyntax
//@[11:33)     IdentifierSyntax
//@[11:33)      Identifier |paramModifierTwoCycle2|
//@[33:34)    NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:2) NewLine |\n|
param paramModifierTwoCycle2 string {
//@[0:73) ParameterDeclarationSyntax
//@[0:5)  Identifier |param|
//@[6:28)  IdentifierSyntax
//@[6:28)   Identifier |paramModifierTwoCycle2|
//@[29:35)  TypeSyntax
//@[29:35)   Identifier |string|
//@[36:73)  ObjectSyntax
//@[36:37)   LeftBrace |{|
//@[37:38)   NewLine |\n|
  default: paramModifierTwoCycle1
//@[2:34)   ObjectPropertySyntax
//@[2:9)    IdentifierSyntax
//@[2:9)     Identifier |default|
//@[9:10)    Colon |:|
//@[11:33)    VariableAccessSyntax
//@[11:33)     IdentifierSyntax
//@[11:33)      Identifier |paramModifierTwoCycle1|
//@[33:34)    NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

// 2-cycle mixed param syntaxes
//@[31:32) NewLine |\n|
param paramMixedTwoCycle1 string = paramMixedTwoCycle2
//@[0:54) ParameterDeclarationSyntax
//@[0:5)  Identifier |param|
//@[6:25)  IdentifierSyntax
//@[6:25)   Identifier |paramMixedTwoCycle1|
//@[26:32)  TypeSyntax
//@[26:32)   Identifier |string|
//@[33:54)  ParameterDefaultValueSyntax
//@[33:34)   Assignment |=|
//@[35:54)   VariableAccessSyntax
//@[35:54)    IdentifierSyntax
//@[35:54)     Identifier |paramMixedTwoCycle2|
//@[54:55) NewLine |\n|
param paramMixedTwoCycle2 string {
//@[0:67) ParameterDeclarationSyntax
//@[0:5)  Identifier |param|
//@[6:25)  IdentifierSyntax
//@[6:25)   Identifier |paramMixedTwoCycle2|
//@[26:32)  TypeSyntax
//@[26:32)   Identifier |string|
//@[33:67)  ObjectSyntax
//@[33:34)   LeftBrace |{|
//@[34:35)   NewLine |\n|
  default: paramMixedTwoCycle1
//@[2:31)   ObjectPropertySyntax
//@[2:9)    IdentifierSyntax
//@[2:9)     Identifier |default|
//@[9:10)    Colon |:|
//@[11:30)    VariableAccessSyntax
//@[11:30)     IdentifierSyntax
//@[11:30)      Identifier |paramMixedTwoCycle1|
//@[30:31)    NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

// wrong types of "variable"/identifier access
//@[46:47) NewLine |\n|
var sampleVar = 'sample'
//@[0:24) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:13)  IdentifierSyntax
//@[4:13)   Identifier |sampleVar|
//@[14:15)  Assignment |=|
//@[16:24)  StringSyntax
//@[16:24)   StringComplete |'sample'|
//@[24:25) NewLine |\n|
resource sampleResource 'Microsoft.Foo/foos@2020-02-02' = {
//@[0:75) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:23)  IdentifierSyntax
//@[9:23)   Identifier |sampleResource|
//@[24:55)  StringSyntax
//@[24:55)   StringComplete |'Microsoft.Foo/foos@2020-02-02'|
//@[56:57)  Assignment |=|
//@[58:75)  ObjectSyntax
//@[58:59)   LeftBrace |{|
//@[59:60)   NewLine |\n|
  name: 'foo'
//@[2:14)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:13)    StringSyntax
//@[8:13)     StringComplete |'foo'|
//@[13:14)    NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:2) NewLine |\n|
output sampleOutput string = 'hello'
//@[0:36) OutputDeclarationSyntax
//@[0:6)  Identifier |output|
//@[7:19)  IdentifierSyntax
//@[7:19)   Identifier |sampleOutput|
//@[20:26)  TypeSyntax
//@[20:26)   Identifier |string|
//@[27:28)  Assignment |=|
//@[29:36)  StringSyntax
//@[29:36)   StringComplete |'hello'|
//@[36:38) NewLine |\n\n|

param paramAccessingVar string = concat(sampleVar, 's')
//@[0:55) ParameterDeclarationSyntax
//@[0:5)  Identifier |param|
//@[6:23)  IdentifierSyntax
//@[6:23)   Identifier |paramAccessingVar|
//@[24:30)  TypeSyntax
//@[24:30)   Identifier |string|
//@[31:55)  ParameterDefaultValueSyntax
//@[31:32)   Assignment |=|
//@[33:55)   FunctionCallSyntax
//@[33:39)    IdentifierSyntax
//@[33:39)     Identifier |concat|
//@[39:40)    LeftParen |(|
//@[40:50)    FunctionArgumentSyntax
//@[40:49)     VariableAccessSyntax
//@[40:49)      IdentifierSyntax
//@[40:49)       Identifier |sampleVar|
//@[49:50)     Comma |,|
//@[51:54)    FunctionArgumentSyntax
//@[51:54)     StringSyntax
//@[51:54)      StringComplete |'s'|
//@[54:55)    RightParen |)|
//@[55:56) NewLine |\n|
param paramAccessingVar2 string {
//@[0:69) ParameterDeclarationSyntax
//@[0:5)  Identifier |param|
//@[6:24)  IdentifierSyntax
//@[6:24)   Identifier |paramAccessingVar2|
//@[25:31)  TypeSyntax
//@[25:31)   Identifier |string|
//@[32:69)  ObjectSyntax
//@[32:33)   LeftBrace |{|
//@[33:34)   NewLine |\n|
  default: 'foo ${sampleVar} foo'
//@[2:34)   ObjectPropertySyntax
//@[2:9)    IdentifierSyntax
//@[2:9)     Identifier |default|
//@[9:10)    Colon |:|
//@[11:33)    StringSyntax
//@[11:18)     StringLeftPiece |'foo ${|
//@[18:27)     VariableAccessSyntax
//@[18:27)      IdentifierSyntax
//@[18:27)       Identifier |sampleVar|
//@[27:33)     StringRightPiece |} foo'|
//@[33:34)    NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

param paramAccessingResource string = sampleResource
//@[0:52) ParameterDeclarationSyntax
//@[0:5)  Identifier |param|
//@[6:28)  IdentifierSyntax
//@[6:28)   Identifier |paramAccessingResource|
//@[29:35)  TypeSyntax
//@[29:35)   Identifier |string|
//@[36:52)  ParameterDefaultValueSyntax
//@[36:37)   Assignment |=|
//@[38:52)   VariableAccessSyntax
//@[38:52)    IdentifierSyntax
//@[38:52)     Identifier |sampleResource|
//@[52:53) NewLine |\n|
param paramAccessingResource2 string {
//@[0:89) ParameterDeclarationSyntax
//@[0:5)  Identifier |param|
//@[6:29)  IdentifierSyntax
//@[6:29)   Identifier |paramAccessingResource2|
//@[30:36)  TypeSyntax
//@[30:36)   Identifier |string|
//@[37:89)  ObjectSyntax
//@[37:38)   LeftBrace |{|
//@[38:39)   NewLine |\n|
  default: base64(sampleResource.properties.foo)
//@[2:49)   ObjectPropertySyntax
//@[2:9)    IdentifierSyntax
//@[2:9)     Identifier |default|
//@[9:10)    Colon |:|
//@[11:48)    FunctionCallSyntax
//@[11:17)     IdentifierSyntax
//@[11:17)      Identifier |base64|
//@[17:18)     LeftParen |(|
//@[18:47)     FunctionArgumentSyntax
//@[18:47)      PropertyAccessSyntax
//@[18:43)       PropertyAccessSyntax
//@[18:32)        VariableAccessSyntax
//@[18:32)         IdentifierSyntax
//@[18:32)          Identifier |sampleResource|
//@[32:33)        Dot |.|
//@[33:43)        IdentifierSyntax
//@[33:43)         Identifier |properties|
//@[43:44)       Dot |.|
//@[44:47)       IdentifierSyntax
//@[44:47)        Identifier |foo|
//@[47:48)     RightParen |)|
//@[48:49)    NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

param paramAccessingOutput string = sampleOutput
//@[0:48) ParameterDeclarationSyntax
//@[0:5)  Identifier |param|
//@[6:26)  IdentifierSyntax
//@[6:26)   Identifier |paramAccessingOutput|
//@[27:33)  TypeSyntax
//@[27:33)   Identifier |string|
//@[34:48)  ParameterDefaultValueSyntax
//@[34:35)   Assignment |=|
//@[36:48)   VariableAccessSyntax
//@[36:48)    IdentifierSyntax
//@[36:48)     Identifier |sampleOutput|
//@[48:49) NewLine |\n|
param paramAccessingOutput2 string {
//@[0:62) ParameterDeclarationSyntax
//@[0:5)  Identifier |param|
//@[6:27)  IdentifierSyntax
//@[6:27)   Identifier |paramAccessingOutput2|
//@[28:34)  TypeSyntax
//@[28:34)   Identifier |string|
//@[35:62)  ObjectSyntax
//@[35:36)   LeftBrace |{|
//@[36:37)   NewLine |\n|
  default: sampleOutput
//@[2:24)   ObjectPropertySyntax
//@[2:9)    IdentifierSyntax
//@[2:9)     Identifier |default|
//@[9:10)    Colon |:|
//@[11:23)    VariableAccessSyntax
//@[11:23)     IdentifierSyntax
//@[11:23)      Identifier |sampleOutput|
//@[23:24)    NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

param stringLiteral string {
//@[0:57) ParameterDeclarationSyntax
//@[0:5)  Identifier |param|
//@[6:19)  IdentifierSyntax
//@[6:19)   Identifier |stringLiteral|
//@[20:26)  TypeSyntax
//@[20:26)   Identifier |string|
//@[27:57)  ObjectSyntax
//@[27:28)   LeftBrace |{|
//@[28:29)   NewLine |\n|
  allowed: [
//@[2:27)   ObjectPropertySyntax
//@[2:9)    IdentifierSyntax
//@[2:9)     Identifier |allowed|
//@[9:10)    Colon |:|
//@[11:26)    ArraySyntax
//@[11:12)     LeftSquare |[|
//@[12:13)     NewLine |\n|
    'def'
//@[4:10)     ArrayItemSyntax
//@[4:9)      StringSyntax
//@[4:9)       StringComplete |'def'|
//@[9:10)      NewLine |\n|
  ]
//@[2:3)     RightSquare |]|
//@[3:4)    NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

param stringLiteral2 string {
//@[0:93) ParameterDeclarationSyntax
//@[0:5)  Identifier |param|
//@[6:20)  IdentifierSyntax
//@[6:20)   Identifier |stringLiteral2|
//@[21:27)  TypeSyntax
//@[21:27)   Identifier |string|
//@[28:93)  ObjectSyntax
//@[28:29)   LeftBrace |{|
//@[29:30)   NewLine |\n|
  allowed: [
//@[2:37)   ObjectPropertySyntax
//@[2:9)    IdentifierSyntax
//@[2:9)     Identifier |allowed|
//@[9:10)    Colon |:|
//@[11:36)    ArraySyntax
//@[11:12)     LeftSquare |[|
//@[12:13)     NewLine |\n|
    'abc'
//@[4:10)     ArrayItemSyntax
//@[4:9)      StringSyntax
//@[4:9)       StringComplete |'abc'|
//@[9:10)      NewLine |\n|
    'def'
//@[4:10)     ArrayItemSyntax
//@[4:9)      StringSyntax
//@[4:9)       StringComplete |'def'|
//@[9:10)      NewLine |\n|
  ]
//@[2:3)     RightSquare |]|
//@[3:4)    NewLine |\n|
  default: stringLiteral
//@[2:25)   ObjectPropertySyntax
//@[2:9)    IdentifierSyntax
//@[2:9)     Identifier |default|
//@[9:10)    Colon |:|
//@[11:24)    VariableAccessSyntax
//@[11:24)     IdentifierSyntax
//@[11:24)      Identifier |stringLiteral|
//@[24:25)    NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

param stringLiteral3 string {
//@[0:84) ParameterDeclarationSyntax
//@[0:5)  Identifier |param|
//@[6:20)  IdentifierSyntax
//@[6:20)   Identifier |stringLiteral3|
//@[21:27)  TypeSyntax
//@[21:27)   Identifier |string|
//@[28:84)  ObjectSyntax
//@[28:29)   LeftBrace |{|
//@[29:30)   NewLine |\n|
  allowed: [
//@[2:27)   ObjectPropertySyntax
//@[2:9)    IdentifierSyntax
//@[2:9)     Identifier |allowed|
//@[9:10)    Colon |:|
//@[11:26)    ArraySyntax
//@[11:12)     LeftSquare |[|
//@[12:13)     NewLine |\n|
    'abc'
//@[4:10)     ArrayItemSyntax
//@[4:9)      StringSyntax
//@[4:9)       StringComplete |'abc'|
//@[9:10)      NewLine |\n|
  ]
//@[2:3)     RightSquare |]|
//@[3:4)    NewLine |\n|
  default: stringLiteral2
//@[2:26)   ObjectPropertySyntax
//@[2:9)    IdentifierSyntax
//@[2:9)     Identifier |default|
//@[9:10)    Colon |:|
//@[11:25)    VariableAccessSyntax
//@[11:25)     IdentifierSyntax
//@[11:25)      Identifier |stringLiteral2|
//@[25:26)    NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

// unterminated multi-line comment
//@[34:35) NewLine |\n|
/*    
//@[6:6) EndOfFile ||
