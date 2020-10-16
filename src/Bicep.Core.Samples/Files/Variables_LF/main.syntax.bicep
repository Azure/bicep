
//@[0:1) NewLine |\n|
// an int variable
//@[18:19) NewLine |\n|
var myInt = 42
//@[0:14) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:9)  IdentifierSyntax
//@[4:9)   Identifier |myInt|
//@[10:11)  Assignment |=|
//@[12:14)  NumericLiteralSyntax
//@[12:14)   Number |42|
//@[14:16) NewLine |\n\n|

// a string variable
//@[20:21) NewLine |\n|
var myStr = 'str'
//@[0:17) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:9)  IdentifierSyntax
//@[4:9)   Identifier |myStr|
//@[10:11)  Assignment |=|
//@[12:17)  StringSyntax
//@[12:17)   StringComplete |'str'|
//@[17:18) NewLine |\n|
var curliesWithNoInterp = '}{1}{'
//@[0:33) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:23)  IdentifierSyntax
//@[4:23)   Identifier |curliesWithNoInterp|
//@[24:25)  Assignment |=|
//@[26:33)  StringSyntax
//@[26:33)   StringComplete |'}{1}{'|
//@[33:34) NewLine |\n|
var interp1 = 'abc${123}def'
//@[0:28) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:11)  IdentifierSyntax
//@[4:11)   Identifier |interp1|
//@[12:13)  Assignment |=|
//@[14:28)  StringSyntax
//@[14:20)   StringLeftPiece |'abc${|
//@[20:23)   NumericLiteralSyntax
//@[20:23)    Number |123|
//@[23:28)   StringRightPiece |}def'|
//@[28:29) NewLine |\n|
var interp2 = '${123}def'
//@[0:25) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:11)  IdentifierSyntax
//@[4:11)   Identifier |interp2|
//@[12:13)  Assignment |=|
//@[14:25)  StringSyntax
//@[14:17)   StringLeftPiece |'${|
//@[17:20)   NumericLiteralSyntax
//@[17:20)    Number |123|
//@[20:25)   StringRightPiece |}def'|
//@[25:26) NewLine |\n|
var interp3 = 'abc${123}'
//@[0:25) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:11)  IdentifierSyntax
//@[4:11)   Identifier |interp3|
//@[12:13)  Assignment |=|
//@[14:25)  StringSyntax
//@[14:20)   StringLeftPiece |'abc${|
//@[20:23)   NumericLiteralSyntax
//@[20:23)    Number |123|
//@[23:25)   StringRightPiece |}'|
//@[25:26) NewLine |\n|
var interp4 = 'abc${123}${456}jk$l${789}p$'
//@[0:43) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:11)  IdentifierSyntax
//@[4:11)   Identifier |interp4|
//@[12:13)  Assignment |=|
//@[14:43)  StringSyntax
//@[14:20)   StringLeftPiece |'abc${|
//@[20:23)   NumericLiteralSyntax
//@[20:23)    Number |123|
//@[23:26)   StringMiddlePiece |}${|
//@[26:29)   NumericLiteralSyntax
//@[26:29)    Number |456|
//@[29:36)   StringMiddlePiece |}jk$l${|
//@[36:39)   NumericLiteralSyntax
//@[36:39)    Number |789|
//@[39:43)   StringRightPiece |}p$'|
//@[43:44) NewLine |\n|
var doubleInterp = 'abc${'def${123}'}_${'${456}${789}'}'
//@[0:56) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:16)  IdentifierSyntax
//@[4:16)   Identifier |doubleInterp|
//@[17:18)  Assignment |=|
//@[19:56)  StringSyntax
//@[19:25)   StringLeftPiece |'abc${|
//@[25:36)   StringSyntax
//@[25:31)    StringLeftPiece |'def${|
//@[31:34)    NumericLiteralSyntax
//@[31:34)     Number |123|
//@[34:36)    StringRightPiece |}'|
//@[36:40)   StringMiddlePiece |}_${|
//@[40:54)   StringSyntax
//@[40:43)    StringLeftPiece |'${|
//@[43:46)    NumericLiteralSyntax
//@[43:46)     Number |456|
//@[46:49)    StringMiddlePiece |}${|
//@[49:52)    NumericLiteralSyntax
//@[49:52)     Number |789|
//@[52:54)    StringRightPiece |}'|
//@[54:56)   StringRightPiece |}'|
//@[56:57) NewLine |\n|
var curliesInInterp = '{${123}{0}${true}}'
//@[0:42) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:19)  IdentifierSyntax
//@[4:19)   Identifier |curliesInInterp|
//@[20:21)  Assignment |=|
//@[22:42)  StringSyntax
//@[22:26)   StringLeftPiece |'{${|
//@[26:29)   NumericLiteralSyntax
//@[26:29)    Number |123|
//@[29:35)   StringMiddlePiece |}{0}${|
//@[35:39)   BooleanLiteralSyntax
//@[35:39)    TrueKeyword |true|
//@[39:42)   StringRightPiece |}}'|
//@[42:44) NewLine |\n\n|

// #completionTest(0) -> declarations
//@[37:39) NewLine |\n\n|

// verify correct bracket escaping
//@[34:35) NewLine |\n|
var bracketInTheMiddle = 'a[b]'
//@[0:31) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:22)  IdentifierSyntax
//@[4:22)   Identifier |bracketInTheMiddle|
//@[23:24)  Assignment |=|
//@[25:31)  StringSyntax
//@[25:31)   StringComplete |'a[b]'|
//@[31:32) NewLine |\n|
// #completionTest(25) -> symbolsPlusTypes
//@[42:43) NewLine |\n|
var bracketAtBeginning = '[test'
//@[0:32) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:22)  IdentifierSyntax
//@[4:22)   Identifier |bracketAtBeginning|
//@[23:24)  Assignment |=|
//@[25:32)  StringSyntax
//@[25:32)   StringComplete |'[test'|
//@[32:33) NewLine |\n|
var enclosingBrackets = '[test]'
//@[0:32) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:21)  IdentifierSyntax
//@[4:21)   Identifier |enclosingBrackets|
//@[22:23)  Assignment |=|
//@[24:32)  StringSyntax
//@[24:32)   StringComplete |'[test]'|
//@[32:33) NewLine |\n|
var emptyJsonArray = '[]'
//@[0:25) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:18)  IdentifierSyntax
//@[4:18)   Identifier |emptyJsonArray|
//@[19:20)  Assignment |=|
//@[21:25)  StringSyntax
//@[21:25)   StringComplete |'[]'|
//@[25:26) NewLine |\n|
var interpolatedBrackets = '[${myInt}]'
//@[0:39) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:24)  IdentifierSyntax
//@[4:24)   Identifier |interpolatedBrackets|
//@[25:26)  Assignment |=|
//@[27:39)  StringSyntax
//@[27:31)   StringLeftPiece |'[${|
//@[31:36)   VariableAccessSyntax
//@[31:36)    IdentifierSyntax
//@[31:36)     Identifier |myInt|
//@[36:39)   StringRightPiece |}]'|
//@[39:40) NewLine |\n|
var nestedBrackets = '[test[]test2]'
//@[0:36) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:18)  IdentifierSyntax
//@[4:18)   Identifier |nestedBrackets|
//@[19:20)  Assignment |=|
//@[21:36)  StringSyntax
//@[21:36)   StringComplete |'[test[]test2]'|
//@[36:37) NewLine |\n|
var nestedInterpolatedBrackets = '[${emptyJsonArray}]'
//@[0:54) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:30)  IdentifierSyntax
//@[4:30)   Identifier |nestedInterpolatedBrackets|
//@[31:32)  Assignment |=|
//@[33:54)  StringSyntax
//@[33:37)   StringLeftPiece |'[${|
//@[37:51)   VariableAccessSyntax
//@[37:51)    IdentifierSyntax
//@[37:51)     Identifier |emptyJsonArray|
//@[51:54)   StringRightPiece |}]'|
//@[54:55) NewLine |\n|
var bracketStringInExpression = concat('[', '\'test\'',']')
//@[0:59) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:29)  IdentifierSyntax
//@[4:29)   Identifier |bracketStringInExpression|
//@[30:31)  Assignment |=|
//@[32:59)  FunctionCallSyntax
//@[32:38)   IdentifierSyntax
//@[32:38)    Identifier |concat|
//@[38:39)   LeftParen |(|
//@[39:43)   FunctionArgumentSyntax
//@[39:42)    StringSyntax
//@[39:42)     StringComplete |'['|
//@[42:43)    Comma |,|
//@[44:55)   FunctionArgumentSyntax
//@[44:54)    StringSyntax
//@[44:54)     StringComplete |'\'test\''|
//@[54:55)    Comma |,|
//@[55:58)   FunctionArgumentSyntax
//@[55:58)    StringSyntax
//@[55:58)     StringComplete |']'|
//@[58:59)   RightParen |)|
//@[59:61) NewLine |\n\n|

// booleans
//@[11:12) NewLine |\n|
var myTruth = true
//@[0:18) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:11)  IdentifierSyntax
//@[4:11)   Identifier |myTruth|
//@[12:13)  Assignment |=|
//@[14:18)  BooleanLiteralSyntax
//@[14:18)   TrueKeyword |true|
//@[18:19) NewLine |\n|
var myFalsehood = false
//@[0:23) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:15)  IdentifierSyntax
//@[4:15)   Identifier |myFalsehood|
//@[16:17)  Assignment |=|
//@[18:23)  BooleanLiteralSyntax
//@[18:23)   FalseKeyword |false|
//@[23:25) NewLine |\n\n|

var myEmptyObj = { }
//@[0:20) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:14)  IdentifierSyntax
//@[4:14)   Identifier |myEmptyObj|
//@[15:16)  Assignment |=|
//@[17:20)  ObjectSyntax
//@[17:18)   LeftBrace |{|
//@[19:20)   RightBrace |}|
//@[20:21) NewLine |\n|
var myEmptyArray = [ ]
//@[0:22) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:16)  IdentifierSyntax
//@[4:16)   Identifier |myEmptyArray|
//@[17:18)  Assignment |=|
//@[19:22)  ArraySyntax
//@[19:20)   LeftSquare |[|
//@[21:22)   RightSquare |]|
//@[22:24) NewLine |\n\n|

// object
//@[9:10) NewLine |\n|
var myObj = {
//@[0:204) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:9)  IdentifierSyntax
//@[4:9)   Identifier |myObj|
//@[10:11)  Assignment |=|
//@[12:204)  ObjectSyntax
//@[12:13)   LeftBrace |{|
//@[13:14)   NewLine |\n|
  a: 'a'
//@[2:8)   ObjectPropertySyntax
//@[2:3)    IdentifierSyntax
//@[2:3)     Identifier |a|
//@[3:4)    Colon |:|
//@[5:8)    StringSyntax
//@[5:8)     StringComplete |'a'|
//@[8:9)   NewLine |\n|
  b: -12
//@[2:8)   ObjectPropertySyntax
//@[2:3)    IdentifierSyntax
//@[2:3)     Identifier |b|
//@[3:4)    Colon |:|
//@[5:8)    UnaryOperationSyntax
//@[5:6)     Minus |-|
//@[6:8)     NumericLiteralSyntax
//@[6:8)      Number |12|
//@[8:9)   NewLine |\n|
  c: true
//@[2:9)   ObjectPropertySyntax
//@[2:3)    IdentifierSyntax
//@[2:3)     Identifier |c|
//@[3:4)    Colon |:|
//@[5:9)    BooleanLiteralSyntax
//@[5:9)     TrueKeyword |true|
//@[9:10)   NewLine |\n|
  d: !true
//@[2:10)   ObjectPropertySyntax
//@[2:3)    IdentifierSyntax
//@[2:3)     Identifier |d|
//@[3:4)    Colon |:|
//@[5:10)    UnaryOperationSyntax
//@[5:6)     Exclamation |!|
//@[6:10)     BooleanLiteralSyntax
//@[6:10)      TrueKeyword |true|
//@[10:11)   NewLine |\n|
  list: [
//@[2:102)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |list|
//@[6:7)    Colon |:|
//@[8:102)    ArraySyntax
//@[8:9)     LeftSquare |[|
//@[9:10)     NewLine |\n|
    1
//@[4:6)     ArrayItemSyntax
//@[4:5)      NumericLiteralSyntax
//@[4:5)       Number |1|
//@[5:6)      NewLine |\n|
    2
//@[4:6)     ArrayItemSyntax
//@[4:5)      NumericLiteralSyntax
//@[4:5)       Number |2|
//@[5:6)      NewLine |\n|
    2+1
//@[4:8)     ArrayItemSyntax
//@[4:7)      BinaryOperationSyntax
//@[4:5)       NumericLiteralSyntax
//@[4:5)        Number |2|
//@[5:6)       Plus |+|
//@[6:7)       NumericLiteralSyntax
//@[6:7)        Number |1|
//@[7:8)      NewLine |\n|
    {
//@[4:54)     ArrayItemSyntax
//@[4:53)      ObjectSyntax
//@[4:5)       LeftBrace |{|
//@[5:6)       NewLine |\n|
      test: 144 > 33 && true || 99 <= 199
//@[6:41)       ObjectPropertySyntax
//@[6:10)        IdentifierSyntax
//@[6:10)         Identifier |test|
//@[10:11)        Colon |:|
//@[12:41)        BinaryOperationSyntax
//@[12:28)         BinaryOperationSyntax
//@[12:20)          BinaryOperationSyntax
//@[12:15)           NumericLiteralSyntax
//@[12:15)            Number |144|
//@[16:17)           GreaterThan |>|
//@[18:20)           NumericLiteralSyntax
//@[18:20)            Number |33|
//@[21:23)          LogicalAnd |&&|
//@[24:28)          BooleanLiteralSyntax
//@[24:28)           TrueKeyword |true|
//@[29:31)         LogicalOr ||||
//@[32:41)         BinaryOperationSyntax
//@[32:34)          NumericLiteralSyntax
//@[32:34)           Number |99|
//@[35:37)          LessThanOrEqual |<=|
//@[38:41)          NumericLiteralSyntax
//@[38:41)           Number |199|
//@[41:42)       NewLine |\n|
    }
//@[4:5)       RightBrace |}|
//@[5:6)      NewLine |\n|
    'a' =~ 'b'
//@[4:15)     ArrayItemSyntax
//@[4:14)      BinaryOperationSyntax
//@[4:7)       StringSyntax
//@[4:7)        StringComplete |'a'|
//@[8:10)       EqualsInsensitive |=~|
//@[11:14)       StringSyntax
//@[11:14)        StringComplete |'b'|
//@[14:15)      NewLine |\n|
  ]
//@[2:3)     RightSquare |]|
//@[3:4)   NewLine |\n|
  obj: {
//@[2:46)   ObjectPropertySyntax
//@[2:5)    IdentifierSyntax
//@[2:5)     Identifier |obj|
//@[5:6)    Colon |:|
//@[7:46)    ObjectSyntax
//@[7:8)     LeftBrace |{|
//@[8:9)     NewLine |\n|
    nested: [
//@[4:33)     ObjectPropertySyntax
//@[4:10)      IdentifierSyntax
//@[4:10)       Identifier |nested|
//@[10:11)      Colon |:|
//@[12:33)      ArraySyntax
//@[12:13)       LeftSquare |[|
//@[13:14)       NewLine |\n|
      'hello'
//@[6:14)       ArrayItemSyntax
//@[6:13)        StringSyntax
//@[6:13)         StringComplete |'hello'|
//@[13:14)        NewLine |\n|
    ]
//@[4:5)       RightSquare |]|
//@[5:6)     NewLine |\n|
  }
//@[2:3)     RightBrace |}|
//@[3:4)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

var objWithInterp = {
//@[0:116) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:17)  IdentifierSyntax
//@[4:17)   Identifier |objWithInterp|
//@[18:19)  Assignment |=|
//@[20:116)  ObjectSyntax
//@[20:21)   LeftBrace |{|
//@[21:22)   NewLine |\n|
  '${myStr}': 1
//@[2:15)   ObjectPropertySyntax
//@[2:12)    StringSyntax
//@[2:5)     StringLeftPiece |'${|
//@[5:10)     VariableAccessSyntax
//@[5:10)      IdentifierSyntax
//@[5:10)       Identifier |myStr|
//@[10:12)     StringRightPiece |}'|
//@[12:13)    Colon |:|
//@[14:15)    NumericLiteralSyntax
//@[14:15)     Number |1|
//@[15:16)   NewLine |\n|
  'abc${myStr}def': 2
//@[2:21)   ObjectPropertySyntax
//@[2:18)    StringSyntax
//@[2:8)     StringLeftPiece |'abc${|
//@[8:13)     VariableAccessSyntax
//@[8:13)      IdentifierSyntax
//@[8:13)       Identifier |myStr|
//@[13:18)     StringRightPiece |}def'|
//@[18:19)    Colon |:|
//@[20:21)    NumericLiteralSyntax
//@[20:21)     Number |2|
//@[21:22)   NewLine |\n|
  '${interp1}abc${interp2}': '${interp1}abc${interp2}'
//@[2:54)   ObjectPropertySyntax
//@[2:27)    StringSyntax
//@[2:5)     StringLeftPiece |'${|
//@[5:12)     VariableAccessSyntax
//@[5:12)      IdentifierSyntax
//@[5:12)       Identifier |interp1|
//@[12:18)     StringMiddlePiece |}abc${|
//@[18:25)     VariableAccessSyntax
//@[18:25)      IdentifierSyntax
//@[18:25)       Identifier |interp2|
//@[25:27)     StringRightPiece |}'|
//@[27:28)    Colon |:|
//@[29:54)    StringSyntax
//@[29:32)     StringLeftPiece |'${|
//@[32:39)     VariableAccessSyntax
//@[32:39)      IdentifierSyntax
//@[32:39)       Identifier |interp1|
//@[39:45)     StringMiddlePiece |}abc${|
//@[45:52)     VariableAccessSyntax
//@[45:52)      IdentifierSyntax
//@[45:52)       Identifier |interp2|
//@[52:54)     StringRightPiece |}'|
//@[54:55)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

// array
//@[8:9) NewLine |\n|
var myArr = [
//@[0:43) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:9)  IdentifierSyntax
//@[4:9)   Identifier |myArr|
//@[10:11)  Assignment |=|
//@[12:43)  ArraySyntax
//@[12:13)   LeftSquare |[|
//@[13:14)   NewLine |\n|
  'pirates'
//@[2:12)   ArrayItemSyntax
//@[2:11)    StringSyntax
//@[2:11)     StringComplete |'pirates'|
//@[11:12)    NewLine |\n|
  'say'
//@[2:8)   ArrayItemSyntax
//@[2:7)    StringSyntax
//@[2:7)     StringComplete |'say'|
//@[7:8)    NewLine |\n|
  'arr'
//@[2:8)   ArrayItemSyntax
//@[2:7)    StringSyntax
//@[2:7)     StringComplete |'arr'|
//@[7:8)    NewLine |\n|
]
//@[0:1)   RightSquare |]|
//@[1:3) NewLine |\n\n|

// array with objects
//@[21:22) NewLine |\n|
var myArrWithObjects = [
//@[0:138) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:20)  IdentifierSyntax
//@[4:20)   Identifier |myArrWithObjects|
//@[21:22)  Assignment |=|
//@[23:138)  ArraySyntax
//@[23:24)   LeftSquare |[|
//@[24:25)   NewLine |\n|
  {
//@[2:41)   ArrayItemSyntax
//@[2:40)    ObjectSyntax
//@[2:3)     LeftBrace |{|
//@[3:4)     NewLine |\n|
    name: 'one'
//@[4:15)     ObjectPropertySyntax
//@[4:8)      IdentifierSyntax
//@[4:8)       Identifier |name|
//@[8:9)      Colon |:|
//@[10:15)      StringSyntax
//@[10:15)       StringComplete |'one'|
//@[15:16)     NewLine |\n|
    enable: true
//@[4:16)     ObjectPropertySyntax
//@[4:10)      IdentifierSyntax
//@[4:10)       Identifier |enable|
//@[10:11)      Colon |:|
//@[12:16)      BooleanLiteralSyntax
//@[12:16)       TrueKeyword |true|
//@[16:17)     NewLine |\n|
  }
//@[2:3)     RightBrace |}|
//@[3:4)    NewLine |\n|
  {
//@[2:71)   ArrayItemSyntax
//@[2:70)    ObjectSyntax
//@[2:3)     LeftBrace |{|
//@[3:4)     NewLine |\n|
    name: 'two'
//@[4:15)     ObjectPropertySyntax
//@[4:8)      IdentifierSyntax
//@[4:8)       Identifier |name|
//@[8:9)      Colon |:|
//@[10:15)      StringSyntax
//@[10:15)       StringComplete |'two'|
//@[15:16)     NewLine |\n|
    enable: false && false || 'two' !~ 'three'
//@[4:46)     ObjectPropertySyntax
//@[4:10)      IdentifierSyntax
//@[4:10)       Identifier |enable|
//@[10:11)      Colon |:|
//@[12:46)      BinaryOperationSyntax
//@[12:26)       BinaryOperationSyntax
//@[12:17)        BooleanLiteralSyntax
//@[12:17)         FalseKeyword |false|
//@[18:20)        LogicalAnd |&&|
//@[21:26)        BooleanLiteralSyntax
//@[21:26)         FalseKeyword |false|
//@[27:29)       LogicalOr ||||
//@[30:46)       BinaryOperationSyntax
//@[30:35)        StringSyntax
//@[30:35)         StringComplete |'two'|
//@[36:38)        NotEqualsInsensitive |!~|
//@[39:46)        StringSyntax
//@[39:46)         StringComplete |'three'|
//@[46:47)     NewLine |\n|
  }
//@[2:3)     RightBrace |}|
//@[3:4)    NewLine |\n|
]
//@[0:1)   RightSquare |]|
//@[1:3) NewLine |\n\n|

var expressionIndexOnAny = any({
//@[0:64) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:24)  IdentifierSyntax
//@[4:24)   Identifier |expressionIndexOnAny|
//@[25:26)  Assignment |=|
//@[27:64)  ArrayAccessSyntax
//@[27:35)   FunctionCallSyntax
//@[27:30)    IdentifierSyntax
//@[27:30)     Identifier |any|
//@[30:31)    LeftParen |(|
//@[31:34)    FunctionArgumentSyntax
//@[31:34)     ObjectSyntax
//@[31:32)      LeftBrace |{|
//@[32:33)      NewLine |\n|
})[az.resourceGroup().location]
//@[0:1)      RightBrace |}|
//@[1:2)    RightParen |)|
//@[2:3)   LeftSquare |[|
//@[3:30)   PropertyAccessSyntax
//@[3:21)    InstanceFunctionCallSyntax
//@[3:5)     VariableAccessSyntax
//@[3:5)      IdentifierSyntax
//@[3:5)       Identifier |az|
//@[5:6)     Dot |.|
//@[6:19)     IdentifierSyntax
//@[6:19)      Identifier |resourceGroup|
//@[19:20)     LeftParen |(|
//@[20:21)     RightParen |)|
//@[21:22)    Dot |.|
//@[22:30)    IdentifierSyntax
//@[22:30)     Identifier |location|
//@[30:31)   RightSquare |]|
//@[31:33) NewLine |\n\n|

var anyIndexOnAny = any(true)[any(false)]
//@[0:41) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:17)  IdentifierSyntax
//@[4:17)   Identifier |anyIndexOnAny|
//@[18:19)  Assignment |=|
//@[20:41)  ArrayAccessSyntax
//@[20:29)   FunctionCallSyntax
//@[20:23)    IdentifierSyntax
//@[20:23)     Identifier |any|
//@[23:24)    LeftParen |(|
//@[24:28)    FunctionArgumentSyntax
//@[24:28)     BooleanLiteralSyntax
//@[24:28)      TrueKeyword |true|
//@[28:29)    RightParen |)|
//@[29:30)   LeftSquare |[|
//@[30:40)   FunctionCallSyntax
//@[30:33)    IdentifierSyntax
//@[30:33)     Identifier |any|
//@[33:34)    LeftParen |(|
//@[34:39)    FunctionArgumentSyntax
//@[34:39)     BooleanLiteralSyntax
//@[34:39)      FalseKeyword |false|
//@[39:40)    RightParen |)|
//@[40:41)   RightSquare |]|
//@[41:43) NewLine |\n\n|

var namedPropertyIndexer = {
//@[0:48) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:24)  IdentifierSyntax
//@[4:24)   Identifier |namedPropertyIndexer|
//@[25:26)  Assignment |=|
//@[27:48)  ArrayAccessSyntax
//@[27:41)   ObjectSyntax
//@[27:28)    LeftBrace |{|
//@[28:29)    NewLine |\n|
  foo: 's'
//@[2:10)    ObjectPropertySyntax
//@[2:5)     IdentifierSyntax
//@[2:5)      Identifier |foo|
//@[5:6)     Colon |:|
//@[7:10)     StringSyntax
//@[7:10)      StringComplete |'s'|
//@[10:11)    NewLine |\n|
}['foo']
//@[0:1)    RightBrace |}|
//@[1:2)   LeftSquare |[|
//@[2:7)   StringSyntax
//@[2:7)    StringComplete |'foo'|
//@[7:8)   RightSquare |]|
//@[8:10) NewLine |\n\n|

var intIndexer = [
//@[0:29) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:14)  IdentifierSyntax
//@[4:14)   Identifier |intIndexer|
//@[15:16)  Assignment |=|
//@[17:29)  ArrayAccessSyntax
//@[17:26)   ArraySyntax
//@[17:18)    LeftSquare |[|
//@[18:19)    NewLine |\n|
  's'
//@[2:6)    ArrayItemSyntax
//@[2:5)     StringSyntax
//@[2:5)      StringComplete |'s'|
//@[5:6)     NewLine |\n|
][0]
//@[0:1)    RightSquare |]|
//@[1:2)   LeftSquare |[|
//@[2:3)   NumericLiteralSyntax
//@[2:3)    Number |0|
//@[3:4)   RightSquare |]|
//@[4:6) NewLine |\n\n|

var functionOnIndexer1 = concat([
//@[0:50) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:22)  IdentifierSyntax
//@[4:22)   Identifier |functionOnIndexer1|
//@[23:24)  Assignment |=|
//@[25:50)  FunctionCallSyntax
//@[25:31)   IdentifierSyntax
//@[25:31)    Identifier |concat|
//@[31:32)   LeftParen |(|
//@[32:45)   FunctionArgumentSyntax
//@[32:44)    ArrayAccessSyntax
//@[32:41)     ArraySyntax
//@[32:33)      LeftSquare |[|
//@[33:34)      NewLine |\n|
  's'
//@[2:6)      ArrayItemSyntax
//@[2:5)       StringSyntax
//@[2:5)        StringComplete |'s'|
//@[5:6)       NewLine |\n|
][0], 's')
//@[0:1)      RightSquare |]|
//@[1:2)     LeftSquare |[|
//@[2:3)     NumericLiteralSyntax
//@[2:3)      Number |0|
//@[3:4)     RightSquare |]|
//@[4:5)    Comma |,|
//@[6:9)   FunctionArgumentSyntax
//@[6:9)    StringSyntax
//@[6:9)     StringComplete |'s'|
//@[9:10)   RightParen |)|
//@[10:12) NewLine |\n\n|

var functionOnIndexer2 = concat([
//@[0:44) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:22)  IdentifierSyntax
//@[4:22)   Identifier |functionOnIndexer2|
//@[23:24)  Assignment |=|
//@[25:44)  FunctionCallSyntax
//@[25:31)   IdentifierSyntax
//@[25:31)    Identifier |concat|
//@[31:32)   LeftParen |(|
//@[32:39)   FunctionArgumentSyntax
//@[32:38)    ArrayAccessSyntax
//@[32:35)     ArraySyntax
//@[32:33)      LeftSquare |[|
//@[33:34)      NewLine |\n|
][0], 's')
//@[0:1)      RightSquare |]|
//@[1:2)     LeftSquare |[|
//@[2:3)     NumericLiteralSyntax
//@[2:3)      Number |0|
//@[3:4)     RightSquare |]|
//@[4:5)    Comma |,|
//@[6:9)   FunctionArgumentSyntax
//@[6:9)    StringSyntax
//@[6:9)     StringComplete |'s'|
//@[9:10)   RightParen |)|
//@[10:12) NewLine |\n\n|

var functionOnIndexer3 = concat([
//@[0:49) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:22)  IdentifierSyntax
//@[4:22)   Identifier |functionOnIndexer3|
//@[23:24)  Assignment |=|
//@[25:49)  FunctionCallSyntax
//@[25:31)   IdentifierSyntax
//@[25:31)    Identifier |concat|
//@[31:32)   LeftParen |(|
//@[32:39)   FunctionArgumentSyntax
//@[32:38)    ArrayAccessSyntax
//@[32:35)     ArraySyntax
//@[32:33)      LeftSquare |[|
//@[33:34)      NewLine |\n|
][0], any('s'))
//@[0:1)      RightSquare |]|
//@[1:2)     LeftSquare |[|
//@[2:3)     NumericLiteralSyntax
//@[2:3)      Number |0|
//@[3:4)     RightSquare |]|
//@[4:5)    Comma |,|
//@[6:14)   FunctionArgumentSyntax
//@[6:14)    FunctionCallSyntax
//@[6:9)     IdentifierSyntax
//@[6:9)      Identifier |any|
//@[9:10)     LeftParen |(|
//@[10:13)     FunctionArgumentSyntax
//@[10:13)      StringSyntax
//@[10:13)       StringComplete |'s'|
//@[13:14)     RightParen |)|
//@[14:15)   RightParen |)|
//@[15:17) NewLine |\n\n|

var singleQuote = '\''
//@[0:22) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:15)  IdentifierSyntax
//@[4:15)   Identifier |singleQuote|
//@[16:17)  Assignment |=|
//@[18:22)  StringSyntax
//@[18:22)   StringComplete |'\''|
//@[22:23) NewLine |\n|
var myPropertyName = '${singleQuote}foo${singleQuote}'
//@[0:54) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:18)  IdentifierSyntax
//@[4:18)   Identifier |myPropertyName|
//@[19:20)  Assignment |=|
//@[21:54)  StringSyntax
//@[21:24)   StringLeftPiece |'${|
//@[24:35)   VariableAccessSyntax
//@[24:35)    IdentifierSyntax
//@[24:35)     Identifier |singleQuote|
//@[35:41)   StringMiddlePiece |}foo${|
//@[41:52)   VariableAccessSyntax
//@[41:52)    IdentifierSyntax
//@[41:52)     Identifier |singleQuote|
//@[52:54)   StringRightPiece |}'|
//@[54:56) NewLine |\n\n|

var unusedIntermediate = listKeys(resourceId('Mock.RP/type', 'steve'), '2020-01-01')
//@[0:84) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:22)  IdentifierSyntax
//@[4:22)   Identifier |unusedIntermediate|
//@[23:24)  Assignment |=|
//@[25:84)  FunctionCallSyntax
//@[25:33)   IdentifierSyntax
//@[25:33)    Identifier |listKeys|
//@[33:34)   LeftParen |(|
//@[34:70)   FunctionArgumentSyntax
//@[34:69)    FunctionCallSyntax
//@[34:44)     IdentifierSyntax
//@[34:44)      Identifier |resourceId|
//@[44:45)     LeftParen |(|
//@[45:60)     FunctionArgumentSyntax
//@[45:59)      StringSyntax
//@[45:59)       StringComplete |'Mock.RP/type'|
//@[59:60)      Comma |,|
//@[61:68)     FunctionArgumentSyntax
//@[61:68)      StringSyntax
//@[61:68)       StringComplete |'steve'|
//@[68:69)     RightParen |)|
//@[69:70)    Comma |,|
//@[71:83)   FunctionArgumentSyntax
//@[71:83)    StringSyntax
//@[71:83)     StringComplete |'2020-01-01'|
//@[83:84)   RightParen |)|
//@[84:85) NewLine |\n|
var unusedIntermediateRef = unusedIntermediate.secondaryKey
//@[0:59) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:25)  IdentifierSyntax
//@[4:25)   Identifier |unusedIntermediateRef|
//@[26:27)  Assignment |=|
//@[28:59)  PropertyAccessSyntax
//@[28:46)   VariableAccessSyntax
//@[28:46)    IdentifierSyntax
//@[28:46)     Identifier |unusedIntermediate|
//@[46:47)   Dot |.|
//@[47:59)   IdentifierSyntax
//@[47:59)    Identifier |secondaryKey|
//@[59:61) NewLine |\n\n|

// previously this was not possible to emit correctly
//@[53:54) NewLine |\n|
var previousEmitLimit = [
//@[0:299) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:21)  IdentifierSyntax
//@[4:21)   Identifier |previousEmitLimit|
//@[22:23)  Assignment |=|
//@[24:299)  ArraySyntax
//@[24:25)   LeftSquare |[|
//@[25:26)   NewLine |\n|
  concat('s')
//@[2:14)   ArrayItemSyntax
//@[2:13)    FunctionCallSyntax
//@[2:8)     IdentifierSyntax
//@[2:8)      Identifier |concat|
//@[8:9)     LeftParen |(|
//@[9:12)     FunctionArgumentSyntax
//@[9:12)      StringSyntax
//@[9:12)       StringComplete |'s'|
//@[12:13)     RightParen |)|
//@[13:14)    NewLine |\n|
  '${4}'
//@[2:9)   ArrayItemSyntax
//@[2:8)    StringSyntax
//@[2:5)     StringLeftPiece |'${|
//@[5:6)     NumericLiteralSyntax
//@[5:6)      Number |4|
//@[6:8)     StringRightPiece |}'|
//@[8:9)    NewLine |\n|
  {
//@[2:249)   ArrayItemSyntax
//@[2:248)    ObjectSyntax
//@[2:3)     LeftBrace |{|
//@[3:4)     NewLine |\n|
    a: {
//@[4:240)     ObjectPropertySyntax
//@[4:5)      IdentifierSyntax
//@[4:5)       Identifier |a|
//@[5:6)      Colon |:|
//@[7:240)      ObjectSyntax
//@[7:8)       LeftBrace |{|
//@[8:9)       NewLine |\n|
      b: base64('s')
//@[6:20)       ObjectPropertySyntax
//@[6:7)        IdentifierSyntax
//@[6:7)         Identifier |b|
//@[7:8)        Colon |:|
//@[9:20)        FunctionCallSyntax
//@[9:15)         IdentifierSyntax
//@[9:15)          Identifier |base64|
//@[15:16)         LeftParen |(|
//@[16:19)         FunctionArgumentSyntax
//@[16:19)          StringSyntax
//@[16:19)           StringComplete |'s'|
//@[19:20)         RightParen |)|
//@[20:21)       NewLine |\n|
      c: concat([
//@[6:82)       ObjectPropertySyntax
//@[6:7)        IdentifierSyntax
//@[6:7)         Identifier |c|
//@[7:8)        Colon |:|
//@[9:82)        FunctionCallSyntax
//@[9:15)         IdentifierSyntax
//@[9:15)          Identifier |concat|
//@[15:16)         LeftParen |(|
//@[16:41)         FunctionArgumentSyntax
//@[16:40)          ArraySyntax
//@[16:17)           LeftSquare |[|
//@[17:18)           NewLine |\n|
        12 + 3
//@[8:15)           ArrayItemSyntax
//@[8:14)            BinaryOperationSyntax
//@[8:10)             NumericLiteralSyntax
//@[8:10)              Number |12|
//@[11:12)             Plus |+|
//@[13:14)             NumericLiteralSyntax
//@[13:14)              Number |3|
//@[14:15)            NewLine |\n|
      ], [
//@[6:7)           RightSquare |]|
//@[7:8)          Comma |,|
//@[9:48)         FunctionArgumentSyntax
//@[9:48)          ArraySyntax
//@[9:10)           LeftSquare |[|
//@[10:11)           NewLine |\n|
        !true
//@[8:14)           ArrayItemSyntax
//@[8:13)            UnaryOperationSyntax
//@[8:9)             Exclamation |!|
//@[9:13)             BooleanLiteralSyntax
//@[9:13)              TrueKeyword |true|
//@[13:14)            NewLine |\n|
        'hello'
//@[8:16)           ArrayItemSyntax
//@[8:15)            StringSyntax
//@[8:15)             StringComplete |'hello'|
//@[15:16)            NewLine |\n|
      ])
//@[6:7)           RightSquare |]|
//@[7:8)         RightParen |)|
//@[8:9)       NewLine |\n|
      d: az.resourceGroup().location
//@[6:36)       ObjectPropertySyntax
//@[6:7)        IdentifierSyntax
//@[6:7)         Identifier |d|
//@[7:8)        Colon |:|
//@[9:36)        PropertyAccessSyntax
//@[9:27)         InstanceFunctionCallSyntax
//@[9:11)          VariableAccessSyntax
//@[9:11)           IdentifierSyntax
//@[9:11)            Identifier |az|
//@[11:12)          Dot |.|
//@[12:25)          IdentifierSyntax
//@[12:25)           Identifier |resourceGroup|
//@[25:26)          LeftParen |(|
//@[26:27)          RightParen |)|
//@[27:28)         Dot |.|
//@[28:36)         IdentifierSyntax
//@[28:36)          Identifier |location|
//@[36:37)       NewLine |\n|
      e: concat([
//@[6:39)       ObjectPropertySyntax
//@[6:7)        IdentifierSyntax
//@[6:7)         Identifier |e|
//@[7:8)        Colon |:|
//@[9:39)        FunctionCallSyntax
//@[9:15)         IdentifierSyntax
//@[9:15)          Identifier |concat|
//@[15:16)         LeftParen |(|
//@[16:38)         FunctionArgumentSyntax
//@[16:38)          ArraySyntax
//@[16:17)           LeftSquare |[|
//@[17:18)           NewLine |\n|
        true
//@[8:13)           ArrayItemSyntax
//@[8:12)            BooleanLiteralSyntax
//@[8:12)             TrueKeyword |true|
//@[12:13)            NewLine |\n|
      ])
//@[6:7)           RightSquare |]|
//@[7:8)         RightParen |)|
//@[8:9)       NewLine |\n|
      f: concat([
//@[6:44)       ObjectPropertySyntax
//@[6:7)        IdentifierSyntax
//@[6:7)         Identifier |f|
//@[7:8)        Colon |:|
//@[9:44)        FunctionCallSyntax
//@[9:15)         IdentifierSyntax
//@[9:15)          Identifier |concat|
//@[15:16)         LeftParen |(|
//@[16:43)         FunctionArgumentSyntax
//@[16:43)          ArraySyntax
//@[16:17)           LeftSquare |[|
//@[17:18)           NewLine |\n|
        's' == 12
//@[8:18)           ArrayItemSyntax
//@[8:17)            BinaryOperationSyntax
//@[8:11)             StringSyntax
//@[8:11)              StringComplete |'s'|
//@[12:14)             Equals |==|
//@[15:17)             NumericLiteralSyntax
//@[15:17)              Number |12|
//@[17:18)            NewLine |\n|
      ])
//@[6:7)           RightSquare |]|
//@[7:8)         RightParen |)|
//@[8:9)       NewLine |\n|
    }
//@[4:5)       RightBrace |}|
//@[5:6)     NewLine |\n|
  }
//@[2:3)     RightBrace |}|
//@[3:4)    NewLine |\n|
]
//@[0:1)   RightSquare |]|
//@[1:3) NewLine |\n\n|

// previously this was not possible to emit correctly
//@[53:54) NewLine |\n|
var previousEmitLimit2 = [
//@[0:327) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:22)  IdentifierSyntax
//@[4:22)   Identifier |previousEmitLimit2|
//@[23:24)  Assignment |=|
//@[25:327)  ArraySyntax
//@[25:26)   LeftSquare |[|
//@[26:27)   NewLine |\n|
  concat('s')
//@[2:14)   ArrayItemSyntax
//@[2:13)    FunctionCallSyntax
//@[2:8)     IdentifierSyntax
//@[2:8)      Identifier |concat|
//@[8:9)     LeftParen |(|
//@[9:12)     FunctionArgumentSyntax
//@[9:12)      StringSyntax
//@[9:12)       StringComplete |'s'|
//@[12:13)     RightParen |)|
//@[13:14)    NewLine |\n|
  '${4}'
//@[2:9)   ArrayItemSyntax
//@[2:8)    StringSyntax
//@[2:5)     StringLeftPiece |'${|
//@[5:6)     NumericLiteralSyntax
//@[5:6)      Number |4|
//@[6:8)     StringRightPiece |}'|
//@[8:9)    NewLine |\n|
  {
//@[2:276)   ArrayItemSyntax
//@[2:275)    ObjectSyntax
//@[2:3)     LeftBrace |{|
//@[3:4)     NewLine |\n|
    a: {
//@[4:267)     ObjectPropertySyntax
//@[4:5)      IdentifierSyntax
//@[4:5)       Identifier |a|
//@[5:6)      Colon |:|
//@[7:267)      ObjectSyntax
//@[7:8)       LeftBrace |{|
//@[8:9)       NewLine |\n|
      b: base64('s')
//@[6:20)       ObjectPropertySyntax
//@[6:7)        IdentifierSyntax
//@[6:7)         Identifier |b|
//@[7:8)        Colon |:|
//@[9:20)        FunctionCallSyntax
//@[9:15)         IdentifierSyntax
//@[9:15)          Identifier |base64|
//@[15:16)         LeftParen |(|
//@[16:19)         FunctionArgumentSyntax
//@[16:19)          StringSyntax
//@[16:19)           StringComplete |'s'|
//@[19:20)         RightParen |)|
//@[20:21)       NewLine |\n|
      c: union({
//@[6:90)       ObjectPropertySyntax
//@[6:7)        IdentifierSyntax
//@[6:7)         Identifier |c|
//@[7:8)        Colon |:|
//@[9:90)        FunctionCallSyntax
//@[9:14)         IdentifierSyntax
//@[9:14)          Identifier |union|
//@[14:15)         LeftParen |(|
//@[15:43)         FunctionArgumentSyntax
//@[15:42)          ObjectSyntax
//@[15:16)           LeftBrace |{|
//@[16:17)           NewLine |\n|
        a: 12 + 3
//@[8:17)           ObjectPropertySyntax
//@[8:9)            IdentifierSyntax
//@[8:9)             Identifier |a|
//@[9:10)            Colon |:|
//@[11:17)            BinaryOperationSyntax
//@[11:13)             NumericLiteralSyntax
//@[11:13)              Number |12|
//@[14:15)             Plus |+|
//@[16:17)             NumericLiteralSyntax
//@[16:17)              Number |3|
//@[17:18)           NewLine |\n|
      }, {
//@[6:7)           RightBrace |}|
//@[7:8)          Comma |,|
//@[9:54)         FunctionArgumentSyntax
//@[9:54)          ObjectSyntax
//@[9:10)           LeftBrace |{|
//@[10:11)           NewLine |\n|
        b: !true
//@[8:16)           ObjectPropertySyntax
//@[8:9)            IdentifierSyntax
//@[8:9)             Identifier |b|
//@[9:10)            Colon |:|
//@[11:16)            UnaryOperationSyntax
//@[11:12)             Exclamation |!|
//@[12:16)             BooleanLiteralSyntax
//@[12:16)              TrueKeyword |true|
//@[16:17)           NewLine |\n|
        c: 'hello'
//@[8:18)           ObjectPropertySyntax
//@[8:9)            IdentifierSyntax
//@[8:9)             Identifier |c|
//@[9:10)            Colon |:|
//@[11:18)            StringSyntax
//@[11:18)             StringComplete |'hello'|
//@[18:19)           NewLine |\n|
      })
//@[6:7)           RightBrace |}|
//@[7:8)         RightParen |)|
//@[8:9)       NewLine |\n|
      d: az.resourceGroup().location
//@[6:36)       ObjectPropertySyntax
//@[6:7)        IdentifierSyntax
//@[6:7)         Identifier |d|
//@[7:8)        Colon |:|
//@[9:36)        PropertyAccessSyntax
//@[9:27)         InstanceFunctionCallSyntax
//@[9:11)          VariableAccessSyntax
//@[9:11)           IdentifierSyntax
//@[9:11)            Identifier |az|
//@[11:12)          Dot |.|
//@[12:25)          IdentifierSyntax
//@[12:25)           Identifier |resourceGroup|
//@[25:26)          LeftParen |(|
//@[26:27)          RightParen |)|
//@[27:28)         Dot |.|
//@[28:36)         IdentifierSyntax
//@[28:36)          Identifier |location|
//@[36:37)       NewLine |\n|
      e: union({
//@[6:45)       ObjectPropertySyntax
//@[6:7)        IdentifierSyntax
//@[6:7)         Identifier |e|
//@[7:8)        Colon |:|
//@[9:45)        FunctionCallSyntax
//@[9:14)         IdentifierSyntax
//@[9:14)          Identifier |union|
//@[14:15)         LeftParen |(|
//@[15:41)         FunctionArgumentSyntax
//@[15:40)          ObjectSyntax
//@[15:16)           LeftBrace |{|
//@[16:17)           NewLine |\n|
        x: true
//@[8:15)           ObjectPropertySyntax
//@[8:9)            IdentifierSyntax
//@[8:9)             Identifier |x|
//@[9:10)            Colon |:|
//@[11:15)            BooleanLiteralSyntax
//@[11:15)             TrueKeyword |true|
//@[15:16)           NewLine |\n|
      }, {})
//@[6:7)           RightBrace |}|
//@[7:8)          Comma |,|
//@[9:11)         FunctionArgumentSyntax
//@[9:11)          ObjectSyntax
//@[9:10)           LeftBrace |{|
//@[10:11)           RightBrace |}|
//@[11:12)         RightParen |)|
//@[12:13)       NewLine |\n|
      f: intersection({
//@[6:57)       ObjectPropertySyntax
//@[6:7)        IdentifierSyntax
//@[6:7)         Identifier |f|
//@[7:8)        Colon |:|
//@[9:57)        FunctionCallSyntax
//@[9:21)         IdentifierSyntax
//@[9:21)          Identifier |intersection|
//@[21:22)         LeftParen |(|
//@[22:53)         FunctionArgumentSyntax
//@[22:52)          ObjectSyntax
//@[22:23)           LeftBrace |{|
//@[23:24)           NewLine |\n|
        q: 's' == 12
//@[8:20)           ObjectPropertySyntax
//@[8:9)            IdentifierSyntax
//@[8:9)             Identifier |q|
//@[9:10)            Colon |:|
//@[11:20)            BinaryOperationSyntax
//@[11:14)             StringSyntax
//@[11:14)              StringComplete |'s'|
//@[15:17)             Equals |==|
//@[18:20)             NumericLiteralSyntax
//@[18:20)              Number |12|
//@[20:21)           NewLine |\n|
      }, {})
//@[6:7)           RightBrace |}|
//@[7:8)          Comma |,|
//@[9:11)         FunctionArgumentSyntax
//@[9:11)          ObjectSyntax
//@[9:10)           LeftBrace |{|
//@[10:11)           RightBrace |}|
//@[11:12)         RightParen |)|
//@[12:13)       NewLine |\n|
    }
//@[4:5)       RightBrace |}|
//@[5:6)     NewLine |\n|
  }
//@[2:3)     RightBrace |}|
//@[3:4)    NewLine |\n|
]
//@[0:1)   RightSquare |]|
//@[1:3) NewLine |\n\n|

// previously this was not possible to emit correctly
//@[53:54) NewLine |\n|
var previousEmitLimit3 = {
//@[0:140) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:22)  IdentifierSyntax
//@[4:22)   Identifier |previousEmitLimit3|
//@[23:24)  Assignment |=|
//@[25:140)  ObjectSyntax
//@[25:26)   LeftBrace |{|
//@[26:27)   NewLine |\n|
  a: {
//@[2:111)   ObjectPropertySyntax
//@[2:3)    IdentifierSyntax
//@[2:3)     Identifier |a|
//@[3:4)    Colon |:|
//@[5:111)    ObjectSyntax
//@[5:6)     LeftBrace |{|
//@[6:7)     NewLine |\n|
    b: {
//@[4:56)     ObjectPropertySyntax
//@[4:5)      IdentifierSyntax
//@[4:5)       Identifier |b|
//@[5:6)      Colon |:|
//@[7:56)      BinaryOperationSyntax
//@[7:51)       ObjectSyntax
//@[7:8)        LeftBrace |{|
//@[8:9)        NewLine |\n|
      a: az.resourceGroup().location
//@[6:36)        ObjectPropertySyntax
//@[6:7)         IdentifierSyntax
//@[6:7)          Identifier |a|
//@[7:8)         Colon |:|
//@[9:36)         PropertyAccessSyntax
//@[9:27)          InstanceFunctionCallSyntax
//@[9:11)           VariableAccessSyntax
//@[9:11)            IdentifierSyntax
//@[9:11)             Identifier |az|
//@[11:12)           Dot |.|
//@[12:25)           IdentifierSyntax
//@[12:25)            Identifier |resourceGroup|
//@[25:26)           LeftParen |(|
//@[26:27)           RightParen |)|
//@[27:28)          Dot |.|
//@[28:36)          IdentifierSyntax
//@[28:36)           Identifier |location|
//@[36:37)        NewLine |\n|
    } == 2
//@[4:5)        RightBrace |}|
//@[6:8)       Equals |==|
//@[9:10)       NumericLiteralSyntax
//@[9:10)        Number |2|
//@[10:11)     NewLine |\n|
    c: concat([
//@[4:43)     ObjectPropertySyntax
//@[4:5)      IdentifierSyntax
//@[4:5)       Identifier |c|
//@[5:6)      Colon |:|
//@[7:43)      FunctionCallSyntax
//@[7:13)       IdentifierSyntax
//@[7:13)        Identifier |concat|
//@[13:14)       LeftParen |(|
//@[14:23)       FunctionArgumentSyntax
//@[14:22)        ArraySyntax
//@[14:15)         LeftSquare |[|
//@[15:17)         NewLine |\n\n|

    ], [
//@[4:5)         RightSquare |]|
//@[5:6)        Comma |,|
//@[7:25)       FunctionArgumentSyntax
//@[7:25)        ArraySyntax
//@[7:8)         LeftSquare |[|
//@[8:9)         NewLine |\n|
      true
//@[6:11)         ArrayItemSyntax
//@[6:10)          BooleanLiteralSyntax
//@[6:10)           TrueKeyword |true|
//@[10:11)          NewLine |\n|
    ])
//@[4:5)         RightSquare |]|
//@[5:6)       RightParen |)|
//@[6:7)     NewLine |\n|
  }
//@[2:3)     RightBrace |}|
//@[3:4)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

// #completionTest(0) -> declarations
//@[37:39) NewLine |\n\n|

var myVar = 'hello'
//@[0:19) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:9)  IdentifierSyntax
//@[4:9)   Identifier |myVar|
//@[10:11)  Assignment |=|
//@[12:19)  StringSyntax
//@[12:19)   StringComplete |'hello'|
//@[19:20) NewLine |\n|
var myVar2 = any({
//@[0:40) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:10)  IdentifierSyntax
//@[4:10)   Identifier |myVar2|
//@[11:12)  Assignment |=|
//@[13:40)  FunctionCallSyntax
//@[13:16)   IdentifierSyntax
//@[13:16)    Identifier |any|
//@[16:17)   LeftParen |(|
//@[17:39)   FunctionArgumentSyntax
//@[17:39)    ObjectSyntax
//@[17:18)     LeftBrace |{|
//@[18:19)     NewLine |\n|
  something: myVar
//@[2:18)     ObjectPropertySyntax
//@[2:11)      IdentifierSyntax
//@[2:11)       Identifier |something|
//@[11:12)      Colon |:|
//@[13:18)      VariableAccessSyntax
//@[13:18)       IdentifierSyntax
//@[13:18)        Identifier |myVar|
//@[18:19)     NewLine |\n|
})
//@[0:1)     RightBrace |}|
//@[1:2)   RightParen |)|
//@[2:3) NewLine |\n|
var myVar3 = any(any({
//@[0:45) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:10)  IdentifierSyntax
//@[4:10)   Identifier |myVar3|
//@[11:12)  Assignment |=|
//@[13:45)  FunctionCallSyntax
//@[13:16)   IdentifierSyntax
//@[13:16)    Identifier |any|
//@[16:17)   LeftParen |(|
//@[17:44)   FunctionArgumentSyntax
//@[17:44)    FunctionCallSyntax
//@[17:20)     IdentifierSyntax
//@[17:20)      Identifier |any|
//@[20:21)     LeftParen |(|
//@[21:43)     FunctionArgumentSyntax
//@[21:43)      ObjectSyntax
//@[21:22)       LeftBrace |{|
//@[22:23)       NewLine |\n|
  something: myVar
//@[2:18)       ObjectPropertySyntax
//@[2:11)        IdentifierSyntax
//@[2:11)         Identifier |something|
//@[11:12)        Colon |:|
//@[13:18)        VariableAccessSyntax
//@[13:18)         IdentifierSyntax
//@[13:18)          Identifier |myVar|
//@[18:19)       NewLine |\n|
}))
//@[0:1)       RightBrace |}|
//@[1:2)     RightParen |)|
//@[2:3)   RightParen |)|
//@[3:4) NewLine |\n|
var myVar4 = length(any(concat('s','a')))
//@[0:41) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:10)  IdentifierSyntax
//@[4:10)   Identifier |myVar4|
//@[11:12)  Assignment |=|
//@[13:41)  FunctionCallSyntax
//@[13:19)   IdentifierSyntax
//@[13:19)    Identifier |length|
//@[19:20)   LeftParen |(|
//@[20:40)   FunctionArgumentSyntax
//@[20:40)    FunctionCallSyntax
//@[20:23)     IdentifierSyntax
//@[20:23)      Identifier |any|
//@[23:24)     LeftParen |(|
//@[24:39)     FunctionArgumentSyntax
//@[24:39)      FunctionCallSyntax
//@[24:30)       IdentifierSyntax
//@[24:30)        Identifier |concat|
//@[30:31)       LeftParen |(|
//@[31:35)       FunctionArgumentSyntax
//@[31:34)        StringSyntax
//@[31:34)         StringComplete |'s'|
//@[34:35)        Comma |,|
//@[35:38)       FunctionArgumentSyntax
//@[35:38)        StringSyntax
//@[35:38)         StringComplete |'a'|
//@[38:39)       RightParen |)|
//@[39:40)     RightParen |)|
//@[40:41)   RightParen |)|
//@[41:43) NewLine |\n\n|

// identifiers can have underscores
//@[35:36) NewLine |\n|
var _ = 3
//@[0:9) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:5)  IdentifierSyntax
//@[4:5)   Identifier |_|
//@[6:7)  Assignment |=|
//@[8:9)  NumericLiteralSyntax
//@[8:9)   Number |3|
//@[9:10) NewLine |\n|
var __ = 10 * _
//@[0:15) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:6)  IdentifierSyntax
//@[4:6)   Identifier |__|
//@[7:8)  Assignment |=|
//@[9:15)  BinaryOperationSyntax
//@[9:11)   NumericLiteralSyntax
//@[9:11)    Number |10|
//@[12:13)   Asterisk |*|
//@[14:15)   VariableAccessSyntax
//@[14:15)    IdentifierSyntax
//@[14:15)     Identifier |_|
//@[15:16) NewLine |\n|
var _0a_1b = true
//@[0:17) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:10)  IdentifierSyntax
//@[4:10)   Identifier |_0a_1b|
//@[11:12)  Assignment |=|
//@[13:17)  BooleanLiteralSyntax
//@[13:17)   TrueKeyword |true|
//@[17:18) NewLine |\n|
var _1_ = _0a_1b || (__ + _ % 2 == 0)
//@[0:37) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:7)  IdentifierSyntax
//@[4:7)   Identifier |_1_|
//@[8:9)  Assignment |=|
//@[10:37)  BinaryOperationSyntax
//@[10:16)   VariableAccessSyntax
//@[10:16)    IdentifierSyntax
//@[10:16)     Identifier |_0a_1b|
//@[17:19)   LogicalOr ||||
//@[20:37)   ParenthesizedExpressionSyntax
//@[20:21)    LeftParen |(|
//@[21:36)    BinaryOperationSyntax
//@[21:31)     BinaryOperationSyntax
//@[21:23)      VariableAccessSyntax
//@[21:23)       IdentifierSyntax
//@[21:23)        Identifier |__|
//@[24:25)      Plus |+|
//@[26:31)      BinaryOperationSyntax
//@[26:27)       VariableAccessSyntax
//@[26:27)        IdentifierSyntax
//@[26:27)         Identifier |_|
//@[28:29)       Modulo |%|
//@[30:31)       NumericLiteralSyntax
//@[30:31)        Number |2|
//@[32:34)     Equals |==|
//@[35:36)     NumericLiteralSyntax
//@[35:36)      Number |0|
//@[36:37)    RightParen |)|
//@[37:39) NewLine |\n\n|

// fully qualified access
//@[25:26) NewLine |\n|
var resourceGroup = 'something'
//@[0:31) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:17)  IdentifierSyntax
//@[4:17)   Identifier |resourceGroup|
//@[18:19)  Assignment |=|
//@[20:31)  StringSyntax
//@[20:31)   StringComplete |'something'|
//@[31:32) NewLine |\n|
var resourceGroupName = az.resourceGroup().name
//@[0:47) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:21)  IdentifierSyntax
//@[4:21)   Identifier |resourceGroupName|
//@[22:23)  Assignment |=|
//@[24:47)  PropertyAccessSyntax
//@[24:42)   InstanceFunctionCallSyntax
//@[24:26)    VariableAccessSyntax
//@[24:26)     IdentifierSyntax
//@[24:26)      Identifier |az|
//@[26:27)    Dot |.|
//@[27:40)    IdentifierSyntax
//@[27:40)     Identifier |resourceGroup|
//@[40:41)    LeftParen |(|
//@[41:42)    RightParen |)|
//@[42:43)   Dot |.|
//@[43:47)   IdentifierSyntax
//@[43:47)    Identifier |name|
//@[47:48) NewLine |\n|
var resourceGroupObject = az.resourceGroup()
//@[0:44) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:23)  IdentifierSyntax
//@[4:23)   Identifier |resourceGroupObject|
//@[24:25)  Assignment |=|
//@[26:44)  InstanceFunctionCallSyntax
//@[26:28)   VariableAccessSyntax
//@[26:28)    IdentifierSyntax
//@[26:28)     Identifier |az|
//@[28:29)   Dot |.|
//@[29:42)   IdentifierSyntax
//@[29:42)    Identifier |resourceGroup|
//@[42:43)   LeftParen |(|
//@[43:44)   RightParen |)|
//@[44:45) NewLine |\n|
var propertyAccessFromObject = resourceGroupObject.name
//@[0:55) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:28)  IdentifierSyntax
//@[4:28)   Identifier |propertyAccessFromObject|
//@[29:30)  Assignment |=|
//@[31:55)  PropertyAccessSyntax
//@[31:50)   VariableAccessSyntax
//@[31:50)    IdentifierSyntax
//@[31:50)     Identifier |resourceGroupObject|
//@[50:51)   Dot |.|
//@[51:55)   IdentifierSyntax
//@[51:55)    Identifier |name|
//@[55:56) NewLine |\n|
var isTrue = sys.add(1, 2) == 3
//@[0:31) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:10)  IdentifierSyntax
//@[4:10)   Identifier |isTrue|
//@[11:12)  Assignment |=|
//@[13:31)  BinaryOperationSyntax
//@[13:26)   InstanceFunctionCallSyntax
//@[13:16)    VariableAccessSyntax
//@[13:16)     IdentifierSyntax
//@[13:16)      Identifier |sys|
//@[16:17)    Dot |.|
//@[17:20)    IdentifierSyntax
//@[17:20)     Identifier |add|
//@[20:21)    LeftParen |(|
//@[21:23)    FunctionArgumentSyntax
//@[21:22)     NumericLiteralSyntax
//@[21:22)      Number |1|
//@[22:23)     Comma |,|
//@[24:25)    FunctionArgumentSyntax
//@[24:25)     NumericLiteralSyntax
//@[24:25)      Number |2|
//@[25:26)    RightParen |)|
//@[27:29)   Equals |==|
//@[30:31)   NumericLiteralSyntax
//@[30:31)    Number |3|
//@[31:32) NewLine |\n|
var isFalse = !isTrue
//@[0:21) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:11)  IdentifierSyntax
//@[4:11)   Identifier |isFalse|
//@[12:13)  Assignment |=|
//@[14:21)  UnaryOperationSyntax
//@[14:15)   Exclamation |!|
//@[15:21)   VariableAccessSyntax
//@[15:21)    IdentifierSyntax
//@[15:21)     Identifier |isTrue|
//@[21:22) NewLine |\n|
var someText = isTrue ? sys.concat('a', sys.concat('b', 'c')) : 'someText'
//@[0:74) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:12)  IdentifierSyntax
//@[4:12)   Identifier |someText|
//@[13:14)  Assignment |=|
//@[15:74)  TernaryOperationSyntax
//@[15:21)   VariableAccessSyntax
//@[15:21)    IdentifierSyntax
//@[15:21)     Identifier |isTrue|
//@[22:23)   Question |?|
//@[24:61)   InstanceFunctionCallSyntax
//@[24:27)    VariableAccessSyntax
//@[24:27)     IdentifierSyntax
//@[24:27)      Identifier |sys|
//@[27:28)    Dot |.|
//@[28:34)    IdentifierSyntax
//@[28:34)     Identifier |concat|
//@[34:35)    LeftParen |(|
//@[35:39)    FunctionArgumentSyntax
//@[35:38)     StringSyntax
//@[35:38)      StringComplete |'a'|
//@[38:39)     Comma |,|
//@[40:60)    FunctionArgumentSyntax
//@[40:60)     InstanceFunctionCallSyntax
//@[40:43)      VariableAccessSyntax
//@[40:43)       IdentifierSyntax
//@[40:43)        Identifier |sys|
//@[43:44)      Dot |.|
//@[44:50)      IdentifierSyntax
//@[44:50)       Identifier |concat|
//@[50:51)      LeftParen |(|
//@[51:55)      FunctionArgumentSyntax
//@[51:54)       StringSyntax
//@[51:54)        StringComplete |'b'|
//@[54:55)       Comma |,|
//@[56:59)      FunctionArgumentSyntax
//@[56:59)       StringSyntax
//@[56:59)        StringComplete |'c'|
//@[59:60)      RightParen |)|
//@[60:61)    RightParen |)|
//@[62:63)   Colon |:|
//@[64:74)   StringSyntax
//@[64:74)    StringComplete |'someText'|
//@[74:75) NewLine |\n|

//@[0:0) EndOfFile ||
