
//@[0:1) NewLine |\n|
// unknown declaration
//@[22:23) NewLine |\n|
bad
//@[0:3) SkippedTriviaSyntax
//@[0:3)  Identifier |bad|
//@[3:5) NewLine |\n\n|

// incomplete variable declaration #completionTest(0,1,2) -> declarations
//@[73:74) NewLine |\n|
var
//@[0:3) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[3:3)  IdentifierSyntax
//@[3:3)   SkippedTriviaSyntax
//@[3:3)  SkippedTriviaSyntax
//@[3:3)  SkippedTriviaSyntax
//@[3:5) NewLine |\n\n|

// missing identifier #completionTest(4) -> empty
//@[49:50) NewLine |\n|
var 
//@[0:4) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:4)  IdentifierSyntax
//@[4:4)   SkippedTriviaSyntax
//@[4:4)  SkippedTriviaSyntax
//@[4:4)  SkippedTriviaSyntax
//@[4:6) NewLine |\n\n|

// incomplete keyword
//@[21:22) NewLine |\n|
// #completionTest(0,1) -> declarations
//@[39:40) NewLine |\n|
v
//@[0:1) SkippedTriviaSyntax
//@[0:1)  Identifier |v|
//@[1:2) NewLine |\n|
// #completionTest(0,1,2) -> declarations
//@[41:42) NewLine |\n|
va
//@[0:2) SkippedTriviaSyntax
//@[0:2)  Identifier |va|
//@[2:4) NewLine |\n\n|

// unassigned variable
//@[22:23) NewLine |\n|
var foo
//@[0:7) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:7)  IdentifierSyntax
//@[4:7)   Identifier |foo|
//@[7:7)  SkippedTriviaSyntax
//@[7:7)  SkippedTriviaSyntax
//@[7:9) NewLine |\n\n|

// #completionTest(18,19) -> symbols
//@[36:37) NewLine |\n|
var missingValue = 
//@[0:19) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:16)  IdentifierSyntax
//@[4:16)   Identifier |missingValue|
//@[17:18)  Assignment |=|
//@[19:19)  SkippedTriviaSyntax
//@[19:21) NewLine |\n\n|

// malformed identifier
//@[23:24) NewLine |\n|
var 2 
//@[0:6) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:5)  IdentifierSyntax
//@[4:5)   SkippedTriviaSyntax
//@[4:5)    Number |2|
//@[6:6)  SkippedTriviaSyntax
//@[6:6)  SkippedTriviaSyntax
//@[6:7) NewLine |\n|
var $ = 23
//@[0:10) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:5)  IdentifierSyntax
//@[4:5)   SkippedTriviaSyntax
//@[4:5)    Unrecognized |$|
//@[6:7)  Assignment |=|
//@[8:10)  NumericLiteralSyntax
//@[8:10)   Number |23|
//@[10:11) NewLine |\n|
var # 33 = 43
//@[0:13) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:8)  IdentifierSyntax
//@[4:8)   SkippedTriviaSyntax
//@[4:5)    Unrecognized |#|
//@[6:8)    Number |33|
//@[9:10)  Assignment |=|
//@[11:13)  NumericLiteralSyntax
//@[11:13)   Number |43|
//@[13:15) NewLine |\n\n|

// no value assigned
//@[20:21) NewLine |\n|
var foo =
//@[0:9) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:7)  IdentifierSyntax
//@[4:7)   Identifier |foo|
//@[8:9)  Assignment |=|
//@[9:9)  SkippedTriviaSyntax
//@[9:11) NewLine |\n\n|

// bad =
//@[8:9) NewLine |\n|
var badEquals 2
//@[0:15) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:13)  IdentifierSyntax
//@[4:13)   Identifier |badEquals|
//@[14:15)  SkippedTriviaSyntax
//@[14:15)   Number |2|
//@[15:15)  SkippedTriviaSyntax
//@[15:16) NewLine |\n|
var badEquals2 3 true
//@[0:21) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:14)  IdentifierSyntax
//@[4:14)   Identifier |badEquals2|
//@[15:21)  SkippedTriviaSyntax
//@[15:16)   Number |3|
//@[17:21)   TrueKeyword |true|
//@[21:21)  SkippedTriviaSyntax
//@[21:23) NewLine |\n\n|

// malformed identifier but type check should happen regardless
//@[63:64) NewLine |\n|
var 2 = x
//@[0:9) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:5)  IdentifierSyntax
//@[4:5)   SkippedTriviaSyntax
//@[4:5)    Number |2|
//@[6:7)  Assignment |=|
//@[8:9)  VariableAccessSyntax
//@[8:9)   IdentifierSyntax
//@[8:9)    Identifier |x|
//@[9:11) NewLine |\n\n|

// bad token value
//@[18:19) NewLine |\n|
var foo = &
//@[0:11) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:7)  IdentifierSyntax
//@[4:7)   Identifier |foo|
//@[8:9)  Assignment |=|
//@[10:11)  SkippedTriviaSyntax
//@[10:11)   Unrecognized |&|
//@[11:13) NewLine |\n\n|

// bad value
//@[12:13) NewLine |\n|
var foo = *
//@[0:11) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:7)  IdentifierSyntax
//@[4:7)   Identifier |foo|
//@[8:9)  Assignment |=|
//@[10:11)  SkippedTriviaSyntax
//@[10:11)   Asterisk |*|
//@[11:13) NewLine |\n\n|

// expressions
//@[14:15) NewLine |\n|
var bar = x
//@[0:11) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:7)  IdentifierSyntax
//@[4:7)   Identifier |bar|
//@[8:9)  Assignment |=|
//@[10:11)  VariableAccessSyntax
//@[10:11)   IdentifierSyntax
//@[10:11)    Identifier |x|
//@[11:12) NewLine |\n|
var bar = foo()
//@[0:15) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:7)  IdentifierSyntax
//@[4:7)   Identifier |bar|
//@[8:9)  Assignment |=|
//@[10:15)  FunctionCallSyntax
//@[10:13)   IdentifierSyntax
//@[10:13)    Identifier |foo|
//@[13:14)   LeftParen |(|
//@[14:15)   RightParen |)|
//@[15:16) NewLine |\n|
var x = 2 + !3
//@[0:14) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:5)  IdentifierSyntax
//@[4:5)   Identifier |x|
//@[6:7)  Assignment |=|
//@[8:14)  BinaryOperationSyntax
//@[8:9)   NumericLiteralSyntax
//@[8:9)    Number |2|
//@[10:11)   Plus |+|
//@[12:14)   UnaryOperationSyntax
//@[12:13)    Exclamation |!|
//@[13:14)    NumericLiteralSyntax
//@[13:14)     Number |3|
//@[14:15) NewLine |\n|
var y = false ? true + 1 : !4
//@[0:29) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:5)  IdentifierSyntax
//@[4:5)   Identifier |y|
//@[6:7)  Assignment |=|
//@[8:29)  TernaryOperationSyntax
//@[8:13)   BooleanLiteralSyntax
//@[8:13)    FalseKeyword |false|
//@[14:15)   Question |?|
//@[16:24)   BinaryOperationSyntax
//@[16:20)    BooleanLiteralSyntax
//@[16:20)     TrueKeyword |true|
//@[21:22)    Plus |+|
//@[23:24)    NumericLiteralSyntax
//@[23:24)     Number |1|
//@[25:26)   Colon |:|
//@[27:29)   UnaryOperationSyntax
//@[27:28)    Exclamation |!|
//@[28:29)    NumericLiteralSyntax
//@[28:29)     Number |4|
//@[29:31) NewLine |\n\n|

// test for array item recovery
//@[31:32) NewLine |\n|
var x = [
//@[0:31) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:5)  IdentifierSyntax
//@[4:5)   Identifier |x|
//@[6:7)  Assignment |=|
//@[8:31)  ArraySyntax
//@[8:9)   LeftSquare |[|
//@[9:10)   NewLine |\n|
  3 + 4
//@[2:7)   ArrayItemSyntax
//@[2:7)    BinaryOperationSyntax
//@[2:3)     NumericLiteralSyntax
//@[2:3)      Number |3|
//@[4:5)     Plus |+|
//@[6:7)     NumericLiteralSyntax
//@[6:7)      Number |4|
//@[7:8)   NewLine |\n|
  =
//@[2:3)   SkippedTriviaSyntax
//@[2:3)    Assignment |=|
//@[3:4)   NewLine |\n|
  !null
//@[2:7)   ArrayItemSyntax
//@[2:7)    UnaryOperationSyntax
//@[2:3)     Exclamation |!|
//@[3:7)     NullLiteralSyntax
//@[3:7)      NullKeyword |null|
//@[7:8)   NewLine |\n|
]
//@[0:1)   RightSquare |]|
//@[1:3) NewLine |\n\n|

// test for object property recovery
//@[36:37) NewLine |\n|
var y = {
//@[0:25) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:5)  IdentifierSyntax
//@[4:5)   Identifier |y|
//@[6:7)  Assignment |=|
//@[8:25)  ObjectSyntax
//@[8:9)   LeftBrace |{|
//@[9:10)   NewLine |\n|
  =
//@[2:3)   ObjectPropertySyntax
//@[2:3)    SkippedTriviaSyntax
//@[2:3)     Assignment |=|
//@[3:3)    SkippedTriviaSyntax
//@[3:3)    SkippedTriviaSyntax
//@[3:4)   NewLine |\n|
  foo: !2
//@[2:9)   ObjectPropertySyntax
//@[2:5)    IdentifierSyntax
//@[2:5)     Identifier |foo|
//@[5:6)    Colon |:|
//@[7:9)    UnaryOperationSyntax
//@[7:8)     Exclamation |!|
//@[8:9)     NumericLiteralSyntax
//@[8:9)      Number |2|
//@[9:10)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

// utcNow and newGuid used outside a param default value
//@[56:57) NewLine |\n|
var test = utcNow('u')
//@[0:22) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:8)  IdentifierSyntax
//@[4:8)   Identifier |test|
//@[9:10)  Assignment |=|
//@[11:22)  FunctionCallSyntax
//@[11:17)   IdentifierSyntax
//@[11:17)    Identifier |utcNow|
//@[17:18)   LeftParen |(|
//@[18:21)   FunctionArgumentSyntax
//@[18:21)    StringSyntax
//@[18:21)     StringComplete |'u'|
//@[21:22)   RightParen |)|
//@[22:23) NewLine |\n|
var test2 = newGuid()
//@[0:21) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:9)  IdentifierSyntax
//@[4:9)   Identifier |test2|
//@[10:11)  Assignment |=|
//@[12:21)  FunctionCallSyntax
//@[12:19)   IdentifierSyntax
//@[12:19)    Identifier |newGuid|
//@[19:20)   LeftParen |(|
//@[20:21)   RightParen |)|
//@[21:23) NewLine |\n\n|

// bad string escape sequence in object key
//@[43:44) NewLine |\n|
var test3 = {
//@[0:36) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:9)  IdentifierSyntax
//@[4:9)   Identifier |test3|
//@[10:11)  Assignment |=|
//@[12:36)  ObjectSyntax
//@[12:13)   LeftBrace |{|
//@[13:14)   NewLine |\n|
  'bad\escape': true
//@[2:20)   ObjectPropertySyntax
//@[2:14)    SkippedTriviaSyntax
//@[2:14)     StringComplete |'bad\escape'|
//@[14:15)    Colon |:|
//@[16:20)    BooleanLiteralSyntax
//@[16:20)     TrueKeyword |true|
//@[20:21)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

// duplicate properties
//@[23:24) NewLine |\n|
var testDupe = {
//@[0:56) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:12)  IdentifierSyntax
//@[4:12)   Identifier |testDupe|
//@[13:14)  Assignment |=|
//@[15:56)  ObjectSyntax
//@[15:16)   LeftBrace |{|
//@[16:17)   NewLine |\n|
  'duplicate': true
//@[2:19)   ObjectPropertySyntax
//@[2:13)    StringSyntax
//@[2:13)     StringComplete |'duplicate'|
//@[13:14)    Colon |:|
//@[15:19)    BooleanLiteralSyntax
//@[15:19)     TrueKeyword |true|
//@[19:20)   NewLine |\n|
  duplicate: true
//@[2:17)   ObjectPropertySyntax
//@[2:11)    IdentifierSyntax
//@[2:11)     Identifier |duplicate|
//@[11:12)    Colon |:|
//@[13:17)    BooleanLiteralSyntax
//@[13:17)     TrueKeyword |true|
//@[17:18)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

// interpolation with type errors in key
//@[40:41) NewLine |\n|
var objWithInterp = {
//@[0:62) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:17)  IdentifierSyntax
//@[4:17)   Identifier |objWithInterp|
//@[18:19)  Assignment |=|
//@[20:62)  ObjectSyntax
//@[20:21)   LeftBrace |{|
//@[21:22)   NewLine |\n|
  'ab${nonExistentIdentifier}cd': true
//@[2:38)   ObjectPropertySyntax
//@[2:32)    StringSyntax
//@[2:7)     StringLeftPiece |'ab${|
//@[7:28)     VariableAccessSyntax
//@[7:28)      IdentifierSyntax
//@[7:28)       Identifier |nonExistentIdentifier|
//@[28:32)     StringRightPiece |}cd'|
//@[32:33)    Colon |:|
//@[34:38)    BooleanLiteralSyntax
//@[34:38)     TrueKeyword |true|
//@[38:39)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

// invalid fully qualified function access
//@[42:43) NewLine |\n|
var mySum = az.add(1,2)
//@[0:23) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:9)  IdentifierSyntax
//@[4:9)   Identifier |mySum|
//@[10:11)  Assignment |=|
//@[12:23)  InstanceFunctionCallSyntax
//@[12:14)   VariableAccessSyntax
//@[12:14)    IdentifierSyntax
//@[12:14)     Identifier |az|
//@[14:15)   Dot |.|
//@[15:18)   IdentifierSyntax
//@[15:18)    Identifier |add|
//@[18:19)   LeftParen |(|
//@[19:21)   FunctionArgumentSyntax
//@[19:20)    NumericLiteralSyntax
//@[19:20)     Number |1|
//@[20:21)    Comma |,|
//@[21:22)   FunctionArgumentSyntax
//@[21:22)    NumericLiteralSyntax
//@[21:22)     Number |2|
//@[22:23)   RightParen |)|
//@[23:24) NewLine |\n|
var myConcat = sys.concat('a', az.concat('b', 'c'))
//@[0:51) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:12)  IdentifierSyntax
//@[4:12)   Identifier |myConcat|
//@[13:14)  Assignment |=|
//@[15:51)  InstanceFunctionCallSyntax
//@[15:18)   VariableAccessSyntax
//@[15:18)    IdentifierSyntax
//@[15:18)     Identifier |sys|
//@[18:19)   Dot |.|
//@[19:25)   IdentifierSyntax
//@[19:25)    Identifier |concat|
//@[25:26)   LeftParen |(|
//@[26:30)   FunctionArgumentSyntax
//@[26:29)    StringSyntax
//@[26:29)     StringComplete |'a'|
//@[29:30)    Comma |,|
//@[31:50)   FunctionArgumentSyntax
//@[31:50)    InstanceFunctionCallSyntax
//@[31:33)     VariableAccessSyntax
//@[31:33)      IdentifierSyntax
//@[31:33)       Identifier |az|
//@[33:34)     Dot |.|
//@[34:40)     IdentifierSyntax
//@[34:40)      Identifier |concat|
//@[40:41)     LeftParen |(|
//@[41:45)     FunctionArgumentSyntax
//@[41:44)      StringSyntax
//@[41:44)       StringComplete |'b'|
//@[44:45)      Comma |,|
//@[46:49)     FunctionArgumentSyntax
//@[46:49)      StringSyntax
//@[46:49)       StringComplete |'c'|
//@[49:50)     RightParen |)|
//@[50:51)   RightParen |)|
//@[51:53) NewLine |\n\n|

// invalid string using double quotes
//@[37:38) NewLine |\n|
var doubleString = "bad string"
//@[0:31) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:16)  IdentifierSyntax
//@[4:16)   Identifier |doubleString|
//@[17:18)  Assignment |=|
//@[19:31)  SkippedTriviaSyntax
//@[19:20)   Unrecognized |"|
//@[20:23)   Identifier |bad|
//@[24:30)   Identifier |string|
//@[30:31)   Unrecognized |"|
//@[31:33) NewLine |\n\n|

var resourceGroup = ''
//@[0:22) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:17)  IdentifierSyntax
//@[4:17)   Identifier |resourceGroup|
//@[18:19)  Assignment |=|
//@[20:22)  StringSyntax
//@[20:22)   StringComplete |''|
//@[22:23) NewLine |\n|
var rgName = resourceGroup().name
//@[0:33) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:10)  IdentifierSyntax
//@[4:10)   Identifier |rgName|
//@[11:12)  Assignment |=|
//@[13:33)  PropertyAccessSyntax
//@[13:28)   FunctionCallSyntax
//@[13:26)    IdentifierSyntax
//@[13:26)     Identifier |resourceGroup|
//@[26:27)    LeftParen |(|
//@[27:28)    RightParen |)|
//@[28:29)   Dot |.|
//@[29:33)   IdentifierSyntax
//@[29:33)    Identifier |name|
//@[33:35) NewLine |\n\n|

// invalid use of reserved namespace
//@[36:37) NewLine |\n|
var az = 1
//@[0:10) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:6)  IdentifierSyntax
//@[4:6)   Identifier |az|
//@[7:8)  Assignment |=|
//@[9:10)  NumericLiteralSyntax
//@[9:10)   Number |1|
//@[10:12) NewLine |\n\n|

// cannot assign a variable to a namespace
//@[42:43) NewLine |\n|
var invalidNamespaceAssignment = az
//@[0:35) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:30)  IdentifierSyntax
//@[4:30)   Identifier |invalidNamespaceAssignment|
//@[31:32)  Assignment |=|
//@[33:35)  VariableAccessSyntax
//@[33:35)   IdentifierSyntax
//@[33:35)    Identifier |az|
//@[35:37) NewLine |\n\n|

var objectLiteralType = {
//@[0:199) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:21)  IdentifierSyntax
//@[4:21)   Identifier |objectLiteralType|
//@[22:23)  Assignment |=|
//@[24:199)  ObjectSyntax
//@[24:25)   LeftBrace |{|
//@[25:26)   NewLine |\n|
  first: true
//@[2:13)   ObjectPropertySyntax
//@[2:7)    IdentifierSyntax
//@[2:7)     Identifier |first|
//@[7:8)    Colon |:|
//@[9:13)    BooleanLiteralSyntax
//@[9:13)     TrueKeyword |true|
//@[13:14)   NewLine |\n|
  second: false
//@[2:15)   ObjectPropertySyntax
//@[2:8)    IdentifierSyntax
//@[2:8)     Identifier |second|
//@[8:9)    Colon |:|
//@[10:15)    BooleanLiteralSyntax
//@[10:15)     FalseKeyword |false|
//@[15:16)   NewLine |\n|
  third: 42
//@[2:11)   ObjectPropertySyntax
//@[2:7)    IdentifierSyntax
//@[2:7)     Identifier |third|
//@[7:8)    Colon |:|
//@[9:11)    NumericLiteralSyntax
//@[9:11)     Number |42|
//@[11:12)   NewLine |\n|
  fourth: 'test'
//@[2:16)   ObjectPropertySyntax
//@[2:8)    IdentifierSyntax
//@[2:8)     Identifier |fourth|
//@[8:9)    Colon |:|
//@[10:16)    StringSyntax
//@[10:16)     StringComplete |'test'|
//@[16:17)   NewLine |\n|
  fifth: [
//@[2:71)   ObjectPropertySyntax
//@[2:7)    IdentifierSyntax
//@[2:7)     Identifier |fifth|
//@[7:8)    Colon |:|
//@[9:71)    ArraySyntax
//@[9:10)     LeftSquare |[|
//@[10:11)     NewLine |\n|
    {
//@[4:27)     ArrayItemSyntax
//@[4:27)      ObjectSyntax
//@[4:5)       LeftBrace |{|
//@[5:6)       NewLine |\n|
      one: true
//@[6:15)       ObjectPropertySyntax
//@[6:9)        IdentifierSyntax
//@[6:9)         Identifier |one|
//@[9:10)        Colon |:|
//@[11:15)        BooleanLiteralSyntax
//@[11:15)         TrueKeyword |true|
//@[15:16)       NewLine |\n|
    }
//@[4:5)       RightBrace |}|
//@[5:6)     NewLine |\n|
    {
//@[4:28)     ArrayItemSyntax
//@[4:28)      ObjectSyntax
//@[4:5)       LeftBrace |{|
//@[5:6)       NewLine |\n|
      one: false
//@[6:16)       ObjectPropertySyntax
//@[6:9)        IdentifierSyntax
//@[6:9)         Identifier |one|
//@[9:10)        Colon |:|
//@[11:16)        BooleanLiteralSyntax
//@[11:16)         FalseKeyword |false|
//@[16:17)       NewLine |\n|
    }
//@[4:5)       RightBrace |}|
//@[5:6)     NewLine |\n|
  ]
//@[2:3)     RightSquare |]|
//@[3:4)   NewLine |\n|
  sixth: [
//@[2:40)   ObjectPropertySyntax
//@[2:7)    IdentifierSyntax
//@[2:7)     Identifier |sixth|
//@[7:8)    Colon |:|
//@[9:40)    ArraySyntax
//@[9:10)     LeftSquare |[|
//@[10:11)     NewLine |\n|
    {
//@[4:25)     ArrayItemSyntax
//@[4:25)      ObjectSyntax
//@[4:5)       LeftBrace |{|
//@[5:6)       NewLine |\n|
      two: 44
//@[6:13)       ObjectPropertySyntax
//@[6:9)        IdentifierSyntax
//@[6:9)         Identifier |two|
//@[9:10)        Colon |:|
//@[11:13)        NumericLiteralSyntax
//@[11:13)         Number |44|
//@[13:14)       NewLine |\n|
    }
//@[4:5)       RightBrace |}|
//@[5:6)     NewLine |\n|
  ]
//@[2:3)     RightSquare |]|
//@[3:4)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

// #completionTest(54) -> objectVarTopLevel
//@[43:44) NewLine |\n|
var objectVarTopLevelCompletions = objectLiteralType.f
//@[0:54) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:32)  IdentifierSyntax
//@[4:32)   Identifier |objectVarTopLevelCompletions|
//@[33:34)  Assignment |=|
//@[35:54)  PropertyAccessSyntax
//@[35:52)   VariableAccessSyntax
//@[35:52)    IdentifierSyntax
//@[35:52)     Identifier |objectLiteralType|
//@[52:53)   Dot |.|
//@[53:54)   IdentifierSyntax
//@[53:54)    Identifier |f|
//@[54:55) NewLine |\n|
// #completionTest(54) -> objectVarTopLevel
//@[43:44) NewLine |\n|
var objectVarTopLevelCompletions2 = objectLiteralType.
//@[0:54) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:33)  IdentifierSyntax
//@[4:33)   Identifier |objectVarTopLevelCompletions2|
//@[34:35)  Assignment |=|
//@[36:54)  PropertyAccessSyntax
//@[36:53)   VariableAccessSyntax
//@[36:53)    IdentifierSyntax
//@[36:53)     Identifier |objectLiteralType|
//@[53:54)   Dot |.|
//@[54:54)   IdentifierSyntax
//@[54:54)    SkippedTriviaSyntax
//@[54:56) NewLine |\n\n|

// this does not produce any completions because mixed array items are of type "any"
//@[84:85) NewLine |\n|
// #completionTest(60) -> mixedArrayProperties
//@[46:47) NewLine |\n|
var mixedArrayTypeCompletions = objectLiteralType.fifth[0].o
//@[0:60) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:29)  IdentifierSyntax
//@[4:29)   Identifier |mixedArrayTypeCompletions|
//@[30:31)  Assignment |=|
//@[32:60)  PropertyAccessSyntax
//@[32:58)   ArrayAccessSyntax
//@[32:55)    PropertyAccessSyntax
//@[32:49)     VariableAccessSyntax
//@[32:49)      IdentifierSyntax
//@[32:49)       Identifier |objectLiteralType|
//@[49:50)     Dot |.|
//@[50:55)     IdentifierSyntax
//@[50:55)      Identifier |fifth|
//@[55:56)    LeftSquare |[|
//@[56:57)    NumericLiteralSyntax
//@[56:57)     Number |0|
//@[57:58)    RightSquare |]|
//@[58:59)   Dot |.|
//@[59:60)   IdentifierSyntax
//@[59:60)    Identifier |o|
//@[60:61) NewLine |\n|
// #completionTest(60) -> mixedArrayProperties
//@[46:47) NewLine |\n|
var mixedArrayTypeCompletions2 = objectLiteralType.fifth[0].
//@[0:60) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:30)  IdentifierSyntax
//@[4:30)   Identifier |mixedArrayTypeCompletions2|
//@[31:32)  Assignment |=|
//@[33:60)  PropertyAccessSyntax
//@[33:59)   ArrayAccessSyntax
//@[33:56)    PropertyAccessSyntax
//@[33:50)     VariableAccessSyntax
//@[33:50)      IdentifierSyntax
//@[33:50)       Identifier |objectLiteralType|
//@[50:51)     Dot |.|
//@[51:56)     IdentifierSyntax
//@[51:56)      Identifier |fifth|
//@[56:57)    LeftSquare |[|
//@[57:58)    NumericLiteralSyntax
//@[57:58)     Number |0|
//@[58:59)    RightSquare |]|
//@[59:60)   Dot |.|
//@[60:60)   IdentifierSyntax
//@[60:60)    SkippedTriviaSyntax
//@[60:62) NewLine |\n\n|

// #completionTest(58) -> oneArrayItemProperties
//@[48:49) NewLine |\n|
var oneArrayItemCompletions = objectLiteralType.sixth[0].t
//@[0:58) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:27)  IdentifierSyntax
//@[4:27)   Identifier |oneArrayItemCompletions|
//@[28:29)  Assignment |=|
//@[30:58)  PropertyAccessSyntax
//@[30:56)   ArrayAccessSyntax
//@[30:53)    PropertyAccessSyntax
//@[30:47)     VariableAccessSyntax
//@[30:47)      IdentifierSyntax
//@[30:47)       Identifier |objectLiteralType|
//@[47:48)     Dot |.|
//@[48:53)     IdentifierSyntax
//@[48:53)      Identifier |sixth|
//@[53:54)    LeftSquare |[|
//@[54:55)    NumericLiteralSyntax
//@[54:55)     Number |0|
//@[55:56)    RightSquare |]|
//@[56:57)   Dot |.|
//@[57:58)   IdentifierSyntax
//@[57:58)    Identifier |t|
//@[58:59) NewLine |\n|
// #completionTest(58) -> oneArrayItemProperties
//@[48:49) NewLine |\n|
var oneArrayItemCompletions2 = objectLiteralType.sixth[0].
//@[0:58) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:28)  IdentifierSyntax
//@[4:28)   Identifier |oneArrayItemCompletions2|
//@[29:30)  Assignment |=|
//@[31:58)  PropertyAccessSyntax
//@[31:57)   ArrayAccessSyntax
//@[31:54)    PropertyAccessSyntax
//@[31:48)     VariableAccessSyntax
//@[31:48)      IdentifierSyntax
//@[31:48)       Identifier |objectLiteralType|
//@[48:49)     Dot |.|
//@[49:54)     IdentifierSyntax
//@[49:54)      Identifier |sixth|
//@[54:55)    LeftSquare |[|
//@[55:56)    NumericLiteralSyntax
//@[55:56)     Number |0|
//@[56:57)    RightSquare |]|
//@[57:58)   Dot |.|
//@[58:58)   IdentifierSyntax
//@[58:58)    SkippedTriviaSyntax
//@[58:60) NewLine |\n\n|

// #completionTest(65) -> objectVarTopLevelIndexes
//@[50:51) NewLine |\n|
var objectVarTopLevelArrayIndexCompletions = objectLiteralType[f]
//@[0:65) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:42)  IdentifierSyntax
//@[4:42)   Identifier |objectVarTopLevelArrayIndexCompletions|
//@[43:44)  Assignment |=|
//@[45:65)  ArrayAccessSyntax
//@[45:62)   VariableAccessSyntax
//@[45:62)    IdentifierSyntax
//@[45:62)     Identifier |objectLiteralType|
//@[62:63)   LeftSquare |[|
//@[63:64)   VariableAccessSyntax
//@[63:64)    IdentifierSyntax
//@[63:64)     Identifier |f|
//@[64:65)   RightSquare |]|
//@[65:67) NewLine |\n\n|

// #completionTest(58) -> twoIndexPlusSymbols
//@[45:46) NewLine |\n|
var oneArrayIndexCompletions = objectLiteralType.sixth[0][]
//@[0:59) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:28)  IdentifierSyntax
//@[4:28)   Identifier |oneArrayIndexCompletions|
//@[29:30)  Assignment |=|
//@[31:59)  ArrayAccessSyntax
//@[31:57)   ArrayAccessSyntax
//@[31:54)    PropertyAccessSyntax
//@[31:48)     VariableAccessSyntax
//@[31:48)      IdentifierSyntax
//@[31:48)       Identifier |objectLiteralType|
//@[48:49)     Dot |.|
//@[49:54)     IdentifierSyntax
//@[49:54)      Identifier |sixth|
//@[54:55)    LeftSquare |[|
//@[55:56)    NumericLiteralSyntax
//@[55:56)     Number |0|
//@[56:57)    RightSquare |]|
//@[57:58)   LeftSquare |[|
//@[58:58)   SkippedTriviaSyntax
//@[58:59)   RightSquare |]|
//@[59:60) NewLine |\n|

//@[0:0) EndOfFile ||
