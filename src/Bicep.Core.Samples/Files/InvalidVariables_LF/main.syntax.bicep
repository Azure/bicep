
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
//@[4:5)    Integer |2|
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
//@[8:10)  IntegerLiteralSyntax
//@[8:10)   Integer |23|
//@[10:11) NewLine |\n|
var # 33 = 43
//@[0:13) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:8)  IdentifierSyntax
//@[4:8)   SkippedTriviaSyntax
//@[4:5)    Unrecognized |#|
//@[6:8)    Integer |33|
//@[9:10)  Assignment |=|
//@[11:13)  IntegerLiteralSyntax
//@[11:13)   Integer |43|
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
//@[14:15)   Integer |2|
//@[15:15)  SkippedTriviaSyntax
//@[15:16) NewLine |\n|
var badEquals2 3 true
//@[0:21) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:14)  IdentifierSyntax
//@[4:14)   Identifier |badEquals2|
//@[15:21)  SkippedTriviaSyntax
//@[15:16)   Integer |3|
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
//@[4:5)    Integer |2|
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
//@[8:9)   IntegerLiteralSyntax
//@[8:9)    Integer |2|
//@[10:11)   Plus |+|
//@[12:14)   UnaryOperationSyntax
//@[12:13)    Exclamation |!|
//@[13:14)    IntegerLiteralSyntax
//@[13:14)     Integer |3|
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
//@[23:24)    IntegerLiteralSyntax
//@[23:24)     Integer |1|
//@[25:26)   Colon |:|
//@[27:29)   UnaryOperationSyntax
//@[27:28)    Exclamation |!|
//@[28:29)    IntegerLiteralSyntax
//@[28:29)     Integer |4|
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
//@[2:3)     IntegerLiteralSyntax
//@[2:3)      Integer |3|
//@[4:5)     Plus |+|
//@[6:7)     IntegerLiteralSyntax
//@[6:7)      Integer |4|
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
//@[8:9)     IntegerLiteralSyntax
//@[8:9)      Integer |2|
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
//@[19:20)    IntegerLiteralSyntax
//@[19:20)     Integer |1|
//@[20:21)    Comma |,|
//@[21:22)   FunctionArgumentSyntax
//@[21:22)    IntegerLiteralSyntax
//@[21:22)     Integer |2|
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

// this does not work at the resource group scope
//@[49:50) NewLine |\n|
var invalidLocationVar = deployment().location
//@[0:46) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:22)  IdentifierSyntax
//@[4:22)   Identifier |invalidLocationVar|
//@[23:24)  Assignment |=|
//@[25:46)  PropertyAccessSyntax
//@[25:37)   FunctionCallSyntax
//@[25:35)    IdentifierSyntax
//@[25:35)     Identifier |deployment|
//@[35:36)    LeftParen |(|
//@[36:37)    RightParen |)|
//@[37:38)   Dot |.|
//@[38:46)   IdentifierSyntax
//@[38:46)    Identifier |location|
//@[46:48) NewLine |\n\n|

var invalidEnvironmentVar = environment().aosdufhsad
//@[0:52) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:25)  IdentifierSyntax
//@[4:25)   Identifier |invalidEnvironmentVar|
//@[26:27)  Assignment |=|
//@[28:52)  PropertyAccessSyntax
//@[28:41)   FunctionCallSyntax
//@[28:39)    IdentifierSyntax
//@[28:39)     Identifier |environment|
//@[39:40)    LeftParen |(|
//@[40:41)    RightParen |)|
//@[41:42)   Dot |.|
//@[42:52)   IdentifierSyntax
//@[42:52)    Identifier |aosdufhsad|
//@[52:53) NewLine |\n|
var invalidEnvAuthVar = environment().authentication.asdgdsag
//@[0:61) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:21)  IdentifierSyntax
//@[4:21)   Identifier |invalidEnvAuthVar|
//@[22:23)  Assignment |=|
//@[24:61)  PropertyAccessSyntax
//@[24:52)   PropertyAccessSyntax
//@[24:37)    FunctionCallSyntax
//@[24:35)     IdentifierSyntax
//@[24:35)      Identifier |environment|
//@[35:36)     LeftParen |(|
//@[36:37)     RightParen |)|
//@[37:38)    Dot |.|
//@[38:52)    IdentifierSyntax
//@[38:52)     Identifier |authentication|
//@[52:53)   Dot |.|
//@[53:61)   IdentifierSyntax
//@[53:61)    Identifier |asdgdsag|
//@[61:63) NewLine |\n\n|

// invalid use of reserved namespace
//@[36:37) NewLine |\n|
var az = 1
//@[0:10) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:6)  IdentifierSyntax
//@[4:6)   Identifier |az|
//@[7:8)  Assignment |=|
//@[9:10)  IntegerLiteralSyntax
//@[9:10)   Integer |1|
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
//@[9:11)    IntegerLiteralSyntax
//@[9:11)     Integer |42|
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
//@[11:13)        IntegerLiteralSyntax
//@[11:13)         Integer |44|
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
//@[56:57)    IntegerLiteralSyntax
//@[56:57)     Integer |0|
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
//@[57:58)    IntegerLiteralSyntax
//@[57:58)     Integer |0|
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
//@[54:55)    IntegerLiteralSyntax
//@[54:55)     Integer |0|
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
//@[55:56)    IntegerLiteralSyntax
//@[55:56)     Integer |0|
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
//@[55:56)    IntegerLiteralSyntax
//@[55:56)     Integer |0|
//@[56:57)    RightSquare |]|
//@[57:58)   LeftSquare |[|
//@[58:58)   SkippedTriviaSyntax
//@[58:59)   RightSquare |]|
//@[59:61) NewLine |\n\n|

// Issue 486
//@[12:13) NewLine |\n|
var myFloat = 3.14
//@[0:16) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:11)  IdentifierSyntax
//@[4:11)   Identifier |myFloat|
//@[12:13)  Assignment |=|
//@[14:16)  PropertyAccessSyntax
//@[14:15)   IntegerLiteralSyntax
//@[14:15)    Integer |3|
//@[15:16)   Dot |.|
//@[16:16)   IdentifierSyntax
//@[16:16)    SkippedTriviaSyntax
//@[16:20) SkippedTriviaSyntax
//@[16:18)  Integer |14|
//@[18:20)  NewLine |\n\n|

// secure cannot be used as a variable decorator
//@[48:49) NewLine |\n|
@sys.secure()
//@[0:31) VariableDeclarationSyntax
//@[0:13)  DecoratorSyntax
//@[0:1)   At |@|
//@[1:13)   InstanceFunctionCallSyntax
//@[1:4)    VariableAccessSyntax
//@[1:4)     IdentifierSyntax
//@[1:4)      Identifier |sys|
//@[4:5)    Dot |.|
//@[5:11)    IdentifierSyntax
//@[5:11)     Identifier |secure|
//@[11:12)    LeftParen |(|
//@[12:13)    RightParen |)|
//@[13:14)  NewLine |\n|
var something = 1
//@[0:3)  Identifier |var|
//@[4:13)  IdentifierSyntax
//@[4:13)   Identifier |something|
//@[14:15)  Assignment |=|
//@[16:17)  IntegerLiteralSyntax
//@[16:17)   Integer |1|
//@[17:19) NewLine |\n\n|

// #completionTest(1) -> sysAndDescription
//@[42:43) NewLine |\n|
@
//@[0:68) VariableDeclarationSyntax
//@[0:1)  DecoratorSyntax
//@[0:1)   At |@|
//@[1:1)   SkippedTriviaSyntax
//@[1:2)  NewLine |\n|
// #completionTest(5) -> description
//@[36:37)  NewLine |\n|
@sys.
//@[0:5)  DecoratorSyntax
//@[0:1)   At |@|
//@[1:5)   PropertyAccessSyntax
//@[1:4)    VariableAccessSyntax
//@[1:4)     IdentifierSyntax
//@[1:4)      Identifier |sys|
//@[4:5)    Dot |.|
//@[5:5)    IdentifierSyntax
//@[5:5)     SkippedTriviaSyntax
//@[5:6)  NewLine |\n|
var anotherThing = true
//@[0:3)  Identifier |var|
//@[4:16)  IdentifierSyntax
//@[4:16)   Identifier |anotherThing|
//@[17:18)  Assignment |=|
//@[19:23)  BooleanLiteralSyntax
//@[19:23)   TrueKeyword |true|
//@[23:25) NewLine |\n\n|

// invalid identifier character classes
//@[39:40) NewLine |\n|
var ☕ = true
//@[0:12) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:5)  IdentifierSyntax
//@[4:5)   SkippedTriviaSyntax
//@[4:5)    Unrecognized |☕|
//@[6:7)  Assignment |=|
//@[8:12)  BooleanLiteralSyntax
//@[8:12)   TrueKeyword |true|
//@[12:13) NewLine |\n|
var a☕ = true
//@[0:13) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:5)  IdentifierSyntax
//@[4:5)   Identifier |a|
//@[5:13)  SkippedTriviaSyntax
//@[5:6)   Unrecognized |☕|
//@[7:8)   Assignment |=|
//@[9:13)   TrueKeyword |true|
//@[13:13)  SkippedTriviaSyntax
//@[13:15) NewLine |\n\n|

var missingArrayVariable = [for thing in stuff: 4]
//@[0:50) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:24)  IdentifierSyntax
//@[4:24)   Identifier |missingArrayVariable|
//@[25:26)  Assignment |=|
//@[27:50)  ForSyntax
//@[27:28)   LeftSquare |[|
//@[28:31)   Identifier |for|
//@[32:37)   LocalVariableSyntax
//@[32:37)    IdentifierSyntax
//@[32:37)     Identifier |thing|
//@[38:40)   Identifier |in|
//@[41:46)   VariableAccessSyntax
//@[41:46)    IdentifierSyntax
//@[41:46)     Identifier |stuff|
//@[46:47)   Colon |:|
//@[48:49)   IntegerLiteralSyntax
//@[48:49)    Integer |4|
//@[49:50)   RightSquare |]|
//@[50:52) NewLine |\n\n|

// loops are only allowed at the top level
//@[42:43) NewLine |\n|
var nonTopLevelLoop = {
//@[0:62) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:19)  IdentifierSyntax
//@[4:19)   Identifier |nonTopLevelLoop|
//@[20:21)  Assignment |=|
//@[22:62)  ObjectSyntax
//@[22:23)   LeftBrace |{|
//@[23:24)   NewLine |\n|
  notOkHere: [for thing in stuff: 4]
//@[2:36)   ObjectPropertySyntax
//@[2:11)    IdentifierSyntax
//@[2:11)     Identifier |notOkHere|
//@[11:12)    Colon |:|
//@[13:36)    ForSyntax
//@[13:14)     LeftSquare |[|
//@[14:17)     Identifier |for|
//@[18:23)     LocalVariableSyntax
//@[18:23)      IdentifierSyntax
//@[18:23)       Identifier |thing|
//@[24:26)     Identifier |in|
//@[27:32)     VariableAccessSyntax
//@[27:32)      IdentifierSyntax
//@[27:32)       Identifier |stuff|
//@[32:33)     Colon |:|
//@[34:35)     IntegerLiteralSyntax
//@[34:35)      Integer |4|
//@[35:36)     RightSquare |]|
//@[36:37)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

// loops with conditions won't even parse
//@[41:42) NewLine |\n|
var noFilteredLoopsInVariables = [for thing in stuff: if]
//@[0:57) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:30)  IdentifierSyntax
//@[4:30)   Identifier |noFilteredLoopsInVariables|
//@[31:32)  Assignment |=|
//@[33:57)  ForSyntax
//@[33:34)   LeftSquare |[|
//@[34:37)   Identifier |for|
//@[38:43)   LocalVariableSyntax
//@[38:43)    IdentifierSyntax
//@[38:43)     Identifier |thing|
//@[44:46)   Identifier |in|
//@[47:52)   VariableAccessSyntax
//@[47:52)    IdentifierSyntax
//@[47:52)     Identifier |stuff|
//@[52:53)   Colon |:|
//@[54:56)   VariableAccessSyntax
//@[54:56)    IdentifierSyntax
//@[54:56)     Identifier |if|
//@[56:57)   RightSquare |]|
//@[57:59) NewLine |\n\n|

// nested loops are also not allowed
//@[36:37) NewLine |\n|
var noNestedVariableLoopsEither = [for thing in stuff: {
//@[0:89) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:31)  IdentifierSyntax
//@[4:31)   Identifier |noNestedVariableLoopsEither|
//@[32:33)  Assignment |=|
//@[34:89)  ForSyntax
//@[34:35)   LeftSquare |[|
//@[35:38)   Identifier |for|
//@[39:44)   LocalVariableSyntax
//@[39:44)    IdentifierSyntax
//@[39:44)     Identifier |thing|
//@[45:47)   Identifier |in|
//@[48:53)   VariableAccessSyntax
//@[48:53)    IdentifierSyntax
//@[48:53)     Identifier |stuff|
//@[53:54)   Colon |:|
//@[55:88)   ObjectSyntax
//@[55:56)    LeftBrace |{|
//@[56:57)    NewLine |\n|
  hello: [for thing in []: 4]
//@[2:29)    ObjectPropertySyntax
//@[2:7)     IdentifierSyntax
//@[2:7)      Identifier |hello|
//@[7:8)     Colon |:|
//@[9:29)     ForSyntax
//@[9:10)      LeftSquare |[|
//@[10:13)      Identifier |for|
//@[14:19)      LocalVariableSyntax
//@[14:19)       IdentifierSyntax
//@[14:19)        Identifier |thing|
//@[20:22)      Identifier |in|
//@[23:25)      ArraySyntax
//@[23:24)       LeftSquare |[|
//@[24:25)       RightSquare |]|
//@[25:26)      Colon |:|
//@[27:28)      IntegerLiteralSyntax
//@[27:28)       Integer |4|
//@[28:29)      RightSquare |]|
//@[29:30)    NewLine |\n|
}]
//@[0:1)    RightBrace |}|
//@[1:2)   RightSquare |]|
//@[2:4) NewLine |\n\n|

// loops in inner properties of a variable are also not supported
//@[65:66) NewLine |\n|
var innerPropertyLoop = {
//@[0:58) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:21)  IdentifierSyntax
//@[4:21)   Identifier |innerPropertyLoop|
//@[22:23)  Assignment |=|
//@[24:58)  ObjectSyntax
//@[24:25)   LeftBrace |{|
//@[25:26)   NewLine |\n|
  a: [for i in range(0,10): i]
//@[2:30)   ObjectPropertySyntax
//@[2:3)    IdentifierSyntax
//@[2:3)     Identifier |a|
//@[3:4)    Colon |:|
//@[5:30)    ForSyntax
//@[5:6)     LeftSquare |[|
//@[6:9)     Identifier |for|
//@[10:11)     LocalVariableSyntax
//@[10:11)      IdentifierSyntax
//@[10:11)       Identifier |i|
//@[12:14)     Identifier |in|
//@[15:26)     FunctionCallSyntax
//@[15:20)      IdentifierSyntax
//@[15:20)       Identifier |range|
//@[20:21)      LeftParen |(|
//@[21:23)      FunctionArgumentSyntax
//@[21:22)       IntegerLiteralSyntax
//@[21:22)        Integer |0|
//@[22:23)       Comma |,|
//@[23:25)      FunctionArgumentSyntax
//@[23:25)       IntegerLiteralSyntax
//@[23:25)        Integer |10|
//@[25:26)      RightParen |)|
//@[26:27)     Colon |:|
//@[28:29)     VariableAccessSyntax
//@[28:29)      IdentifierSyntax
//@[28:29)       Identifier |i|
//@[29:30)     RightSquare |]|
//@[30:31)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:2) NewLine |\n|
var innerPropertyLoop2 = {
//@[0:72) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:22)  IdentifierSyntax
//@[4:22)   Identifier |innerPropertyLoop2|
//@[23:24)  Assignment |=|
//@[25:72)  ObjectSyntax
//@[25:26)   LeftBrace |{|
//@[26:27)   NewLine |\n|
  b: {
//@[2:43)   ObjectPropertySyntax
//@[2:3)    IdentifierSyntax
//@[2:3)     Identifier |b|
//@[3:4)    Colon |:|
//@[5:43)    ObjectSyntax
//@[5:6)     LeftBrace |{|
//@[6:7)     NewLine |\n|
    a: [for i in range(0,10): i]
//@[4:32)     ObjectPropertySyntax
//@[4:5)      IdentifierSyntax
//@[4:5)       Identifier |a|
//@[5:6)      Colon |:|
//@[7:32)      ForSyntax
//@[7:8)       LeftSquare |[|
//@[8:11)       Identifier |for|
//@[12:13)       LocalVariableSyntax
//@[12:13)        IdentifierSyntax
//@[12:13)         Identifier |i|
//@[14:16)       Identifier |in|
//@[17:28)       FunctionCallSyntax
//@[17:22)        IdentifierSyntax
//@[17:22)         Identifier |range|
//@[22:23)        LeftParen |(|
//@[23:25)        FunctionArgumentSyntax
//@[23:24)         IntegerLiteralSyntax
//@[23:24)          Integer |0|
//@[24:25)         Comma |,|
//@[25:27)        FunctionArgumentSyntax
//@[25:27)         IntegerLiteralSyntax
//@[25:27)          Integer |10|
//@[27:28)        RightParen |)|
//@[28:29)       Colon |:|
//@[30:31)       VariableAccessSyntax
//@[30:31)        IdentifierSyntax
//@[30:31)         Identifier |i|
//@[31:32)       RightSquare |]|
//@[32:33)     NewLine |\n|
  }
//@[2:3)     RightBrace |}|
//@[3:4)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

// loops using expressions with a runtime dependency are also not allowed
//@[73:74) NewLine |\n|
var keys = listKeys('fake','fake')
//@[0:34) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:8)  IdentifierSyntax
//@[4:8)   Identifier |keys|
//@[9:10)  Assignment |=|
//@[11:34)  FunctionCallSyntax
//@[11:19)   IdentifierSyntax
//@[11:19)    Identifier |listKeys|
//@[19:20)   LeftParen |(|
//@[20:27)   FunctionArgumentSyntax
//@[20:26)    StringSyntax
//@[20:26)     StringComplete |'fake'|
//@[26:27)    Comma |,|
//@[27:33)   FunctionArgumentSyntax
//@[27:33)    StringSyntax
//@[27:33)     StringComplete |'fake'|
//@[33:34)   RightParen |)|
//@[34:35) NewLine |\n|
var indirection = keys
//@[0:22) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:15)  IdentifierSyntax
//@[4:15)   Identifier |indirection|
//@[16:17)  Assignment |=|
//@[18:22)  VariableAccessSyntax
//@[18:22)   IdentifierSyntax
//@[18:22)    Identifier |keys|
//@[22:24) NewLine |\n\n|

var runtimeLoop = [for (item, index) in []: indirection]
//@[0:56) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:15)  IdentifierSyntax
//@[4:15)   Identifier |runtimeLoop|
//@[16:17)  Assignment |=|
//@[18:56)  ForSyntax
//@[18:19)   LeftSquare |[|
//@[19:22)   Identifier |for|
//@[23:36)   ForVariableBlockSyntax
//@[23:24)    LeftParen |(|
//@[24:28)    LocalVariableSyntax
//@[24:28)     IdentifierSyntax
//@[24:28)      Identifier |item|
//@[28:29)    Comma |,|
//@[30:35)    LocalVariableSyntax
//@[30:35)     IdentifierSyntax
//@[30:35)      Identifier |index|
//@[35:36)    RightParen |)|
//@[37:39)   Identifier |in|
//@[40:42)   ArraySyntax
//@[40:41)    LeftSquare |[|
//@[41:42)    RightSquare |]|
//@[42:43)   Colon |:|
//@[44:55)   VariableAccessSyntax
//@[44:55)    IdentifierSyntax
//@[44:55)     Identifier |indirection|
//@[55:56)   RightSquare |]|
//@[56:57) NewLine |\n|
var runtimeLoop2 = [for (item, index) in indirection.keys: 's']
//@[0:63) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:16)  IdentifierSyntax
//@[4:16)   Identifier |runtimeLoop2|
//@[17:18)  Assignment |=|
//@[19:63)  ForSyntax
//@[19:20)   LeftSquare |[|
//@[20:23)   Identifier |for|
//@[24:37)   ForVariableBlockSyntax
//@[24:25)    LeftParen |(|
//@[25:29)    LocalVariableSyntax
//@[25:29)     IdentifierSyntax
//@[25:29)      Identifier |item|
//@[29:30)    Comma |,|
//@[31:36)    LocalVariableSyntax
//@[31:36)     IdentifierSyntax
//@[31:36)      Identifier |index|
//@[36:37)    RightParen |)|
//@[38:40)   Identifier |in|
//@[41:57)   PropertyAccessSyntax
//@[41:52)    VariableAccessSyntax
//@[41:52)     IdentifierSyntax
//@[41:52)      Identifier |indirection|
//@[52:53)    Dot |.|
//@[53:57)    IdentifierSyntax
//@[53:57)     Identifier |keys|
//@[57:58)   Colon |:|
//@[59:62)   StringSyntax
//@[59:62)    StringComplete |'s'|
//@[62:63)   RightSquare |]|
//@[63:65) NewLine |\n\n|

var zoneInput = []
//@[0:18) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:13)  IdentifierSyntax
//@[4:13)   Identifier |zoneInput|
//@[14:15)  Assignment |=|
//@[16:18)  ArraySyntax
//@[16:17)   LeftSquare |[|
//@[17:18)   RightSquare |]|
//@[18:19) NewLine |\n|
resource zones 'Microsoft.Network/dnsZones@2018-05-01' = [for (zone, i) in zoneInput: {
//@[0:143) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:14)  IdentifierSyntax
//@[9:14)   Identifier |zones|
//@[15:54)  StringSyntax
//@[15:54)   StringComplete |'Microsoft.Network/dnsZones@2018-05-01'|
//@[55:56)  Assignment |=|
//@[57:143)  ForSyntax
//@[57:58)   LeftSquare |[|
//@[58:61)   Identifier |for|
//@[62:71)   ForVariableBlockSyntax
//@[62:63)    LeftParen |(|
//@[63:67)    LocalVariableSyntax
//@[63:67)     IdentifierSyntax
//@[63:67)      Identifier |zone|
//@[67:68)    Comma |,|
//@[69:70)    LocalVariableSyntax
//@[69:70)     IdentifierSyntax
//@[69:70)      Identifier |i|
//@[70:71)    RightParen |)|
//@[72:74)   Identifier |in|
//@[75:84)   VariableAccessSyntax
//@[75:84)    IdentifierSyntax
//@[75:84)     Identifier |zoneInput|
//@[84:85)   Colon |:|
//@[86:142)   ObjectSyntax
//@[86:87)    LeftBrace |{|
//@[87:88)    NewLine |\n|
  name: zone
//@[2:12)    ObjectPropertySyntax
//@[2:6)     IdentifierSyntax
//@[2:6)      Identifier |name|
//@[6:7)     Colon |:|
//@[8:12)     VariableAccessSyntax
//@[8:12)      IdentifierSyntax
//@[8:12)       Identifier |zone|
//@[12:13)    NewLine |\n|
  location: az.resourceGroup().location
//@[2:39)    ObjectPropertySyntax
//@[2:10)     IdentifierSyntax
//@[2:10)      Identifier |location|
//@[10:11)     Colon |:|
//@[12:39)     PropertyAccessSyntax
//@[12:30)      InstanceFunctionCallSyntax
//@[12:14)       VariableAccessSyntax
//@[12:14)        IdentifierSyntax
//@[12:14)         Identifier |az|
//@[14:15)       Dot |.|
//@[15:28)       IdentifierSyntax
//@[15:28)        Identifier |resourceGroup|
//@[28:29)       LeftParen |(|
//@[29:30)       RightParen |)|
//@[30:31)      Dot |.|
//@[31:39)      IdentifierSyntax
//@[31:39)       Identifier |location|
//@[39:40)    NewLine |\n|
}]
//@[0:1)    RightBrace |}|
//@[1:2)   RightSquare |]|
//@[2:3) NewLine |\n|
var inlinedVariable = zones[0].properties.zoneType
//@[0:50) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:19)  IdentifierSyntax
//@[4:19)   Identifier |inlinedVariable|
//@[20:21)  Assignment |=|
//@[22:50)  PropertyAccessSyntax
//@[22:41)   PropertyAccessSyntax
//@[22:30)    ArrayAccessSyntax
//@[22:27)     VariableAccessSyntax
//@[22:27)      IdentifierSyntax
//@[22:27)       Identifier |zones|
//@[27:28)     LeftSquare |[|
//@[28:29)     IntegerLiteralSyntax
//@[28:29)      Integer |0|
//@[29:30)     RightSquare |]|
//@[30:31)    Dot |.|
//@[31:41)    IdentifierSyntax
//@[31:41)     Identifier |properties|
//@[41:42)   Dot |.|
//@[42:50)   IdentifierSyntax
//@[42:50)    Identifier |zoneType|
//@[50:52) NewLine |\n\n|

var runtimeLoop3 = [for (zone, i) in zoneInput: {
//@[0:73) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:16)  IdentifierSyntax
//@[4:16)   Identifier |runtimeLoop3|
//@[17:18)  Assignment |=|
//@[19:73)  ForSyntax
//@[19:20)   LeftSquare |[|
//@[20:23)   Identifier |for|
//@[24:33)   ForVariableBlockSyntax
//@[24:25)    LeftParen |(|
//@[25:29)    LocalVariableSyntax
//@[25:29)     IdentifierSyntax
//@[25:29)      Identifier |zone|
//@[29:30)    Comma |,|
//@[31:32)    LocalVariableSyntax
//@[31:32)     IdentifierSyntax
//@[31:32)      Identifier |i|
//@[32:33)    RightParen |)|
//@[34:36)   Identifier |in|
//@[37:46)   VariableAccessSyntax
//@[37:46)    IdentifierSyntax
//@[37:46)     Identifier |zoneInput|
//@[46:47)   Colon |:|
//@[48:72)   ObjectSyntax
//@[48:49)    LeftBrace |{|
//@[49:50)    NewLine |\n|
  a: inlinedVariable
//@[2:20)    ObjectPropertySyntax
//@[2:3)     IdentifierSyntax
//@[2:3)      Identifier |a|
//@[3:4)     Colon |:|
//@[5:20)     VariableAccessSyntax
//@[5:20)      IdentifierSyntax
//@[5:20)       Identifier |inlinedVariable|
//@[20:21)    NewLine |\n|
}]
//@[0:1)    RightBrace |}|
//@[1:2)   RightSquare |]|
//@[2:4) NewLine |\n\n|

var runtimeLoop4 = [for (zone, i) in zones[0].properties.registrationVirtualNetworks: {
//@[0:97) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:16)  IdentifierSyntax
//@[4:16)   Identifier |runtimeLoop4|
//@[17:18)  Assignment |=|
//@[19:97)  ForSyntax
//@[19:20)   LeftSquare |[|
//@[20:23)   Identifier |for|
//@[24:33)   ForVariableBlockSyntax
//@[24:25)    LeftParen |(|
//@[25:29)    LocalVariableSyntax
//@[25:29)     IdentifierSyntax
//@[25:29)      Identifier |zone|
//@[29:30)    Comma |,|
//@[31:32)    LocalVariableSyntax
//@[31:32)     IdentifierSyntax
//@[31:32)      Identifier |i|
//@[32:33)    RightParen |)|
//@[34:36)   Identifier |in|
//@[37:84)   PropertyAccessSyntax
//@[37:56)    PropertyAccessSyntax
//@[37:45)     ArrayAccessSyntax
//@[37:42)      VariableAccessSyntax
//@[37:42)       IdentifierSyntax
//@[37:42)        Identifier |zones|
//@[42:43)      LeftSquare |[|
//@[43:44)      IntegerLiteralSyntax
//@[43:44)       Integer |0|
//@[44:45)      RightSquare |]|
//@[45:46)     Dot |.|
//@[46:56)     IdentifierSyntax
//@[46:56)      Identifier |properties|
//@[56:57)    Dot |.|
//@[57:84)    IdentifierSyntax
//@[57:84)     Identifier |registrationVirtualNetworks|
//@[84:85)   Colon |:|
//@[86:96)   ObjectSyntax
//@[86:87)    LeftBrace |{|
//@[87:88)    NewLine |\n|
  a: 0
//@[2:6)    ObjectPropertySyntax
//@[2:3)     IdentifierSyntax
//@[2:3)      Identifier |a|
//@[3:4)     Colon |:|
//@[5:6)     IntegerLiteralSyntax
//@[5:6)      Integer |0|
//@[6:7)    NewLine |\n|
}]
//@[0:1)    RightBrace |}|
//@[1:2)   RightSquare |]|
//@[2:4) NewLine |\n\n|

var notRuntime = concat('a','b')
//@[0:32) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:14)  IdentifierSyntax
//@[4:14)   Identifier |notRuntime|
//@[15:16)  Assignment |=|
//@[17:32)  FunctionCallSyntax
//@[17:23)   IdentifierSyntax
//@[17:23)    Identifier |concat|
//@[23:24)   LeftParen |(|
//@[24:28)   FunctionArgumentSyntax
//@[24:27)    StringSyntax
//@[24:27)     StringComplete |'a'|
//@[27:28)    Comma |,|
//@[28:31)   FunctionArgumentSyntax
//@[28:31)    StringSyntax
//@[28:31)     StringComplete |'b'|
//@[31:32)   RightParen |)|
//@[32:33) NewLine |\n|
var evenMoreIndirection = concat(notRuntime, string(moreIndirection))
//@[0:69) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:23)  IdentifierSyntax
//@[4:23)   Identifier |evenMoreIndirection|
//@[24:25)  Assignment |=|
//@[26:69)  FunctionCallSyntax
//@[26:32)   IdentifierSyntax
//@[26:32)    Identifier |concat|
//@[32:33)   LeftParen |(|
//@[33:44)   FunctionArgumentSyntax
//@[33:43)    VariableAccessSyntax
//@[33:43)     IdentifierSyntax
//@[33:43)      Identifier |notRuntime|
//@[43:44)    Comma |,|
//@[45:68)   FunctionArgumentSyntax
//@[45:68)    FunctionCallSyntax
//@[45:51)     IdentifierSyntax
//@[45:51)      Identifier |string|
//@[51:52)     LeftParen |(|
//@[52:67)     FunctionArgumentSyntax
//@[52:67)      VariableAccessSyntax
//@[52:67)       IdentifierSyntax
//@[52:67)        Identifier |moreIndirection|
//@[67:68)     RightParen |)|
//@[68:69)   RightParen |)|
//@[69:70) NewLine |\n|
var moreIndirection = reference('s','s', 'Full')
//@[0:48) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:19)  IdentifierSyntax
//@[4:19)   Identifier |moreIndirection|
//@[20:21)  Assignment |=|
//@[22:48)  FunctionCallSyntax
//@[22:31)   IdentifierSyntax
//@[22:31)    Identifier |reference|
//@[31:32)   LeftParen |(|
//@[32:36)   FunctionArgumentSyntax
//@[32:35)    StringSyntax
//@[32:35)     StringComplete |'s'|
//@[35:36)    Comma |,|
//@[36:40)   FunctionArgumentSyntax
//@[36:39)    StringSyntax
//@[36:39)     StringComplete |'s'|
//@[39:40)    Comma |,|
//@[41:47)   FunctionArgumentSyntax
//@[41:47)    StringSyntax
//@[41:47)     StringComplete |'Full'|
//@[47:48)   RightParen |)|
//@[48:50) NewLine |\n\n|

var myRef = [
//@[0:37) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:9)  IdentifierSyntax
//@[4:9)   Identifier |myRef|
//@[10:11)  Assignment |=|
//@[12:37)  ArraySyntax
//@[12:13)   LeftSquare |[|
//@[13:14)   NewLine |\n|
  evenMoreIndirection
//@[2:21)   ArrayItemSyntax
//@[2:21)    VariableAccessSyntax
//@[2:21)     IdentifierSyntax
//@[2:21)      Identifier |evenMoreIndirection|
//@[21:22)   NewLine |\n|
]
//@[0:1)   RightSquare |]|
//@[1:2) NewLine |\n|
var runtimeLoop5 = [for (item, index) in myRef: 's']
//@[0:52) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:16)  IdentifierSyntax
//@[4:16)   Identifier |runtimeLoop5|
//@[17:18)  Assignment |=|
//@[19:52)  ForSyntax
//@[19:20)   LeftSquare |[|
//@[20:23)   Identifier |for|
//@[24:37)   ForVariableBlockSyntax
//@[24:25)    LeftParen |(|
//@[25:29)    LocalVariableSyntax
//@[25:29)     IdentifierSyntax
//@[25:29)      Identifier |item|
//@[29:30)    Comma |,|
//@[31:36)    LocalVariableSyntax
//@[31:36)     IdentifierSyntax
//@[31:36)      Identifier |index|
//@[36:37)    RightParen |)|
//@[38:40)   Identifier |in|
//@[41:46)   VariableAccessSyntax
//@[41:46)    IdentifierSyntax
//@[41:46)     Identifier |myRef|
//@[46:47)   Colon |:|
//@[48:51)   StringSyntax
//@[48:51)    StringComplete |'s'|
//@[51:52)   RightSquare |]|
//@[52:54) NewLine |\n\n|

// cannot use loops in expressions
//@[34:35) NewLine |\n|
var loopExpression = union([for thing in stuff: 4], [for thing in stuff: true])
//@[0:79) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:18)  IdentifierSyntax
//@[4:18)   Identifier |loopExpression|
//@[19:20)  Assignment |=|
//@[21:79)  FunctionCallSyntax
//@[21:26)   IdentifierSyntax
//@[21:26)    Identifier |union|
//@[26:27)   LeftParen |(|
//@[27:51)   FunctionArgumentSyntax
//@[27:50)    ForSyntax
//@[27:28)     LeftSquare |[|
//@[28:31)     Identifier |for|
//@[32:37)     LocalVariableSyntax
//@[32:37)      IdentifierSyntax
//@[32:37)       Identifier |thing|
//@[38:40)     Identifier |in|
//@[41:46)     VariableAccessSyntax
//@[41:46)      IdentifierSyntax
//@[41:46)       Identifier |stuff|
//@[46:47)     Colon |:|
//@[48:49)     IntegerLiteralSyntax
//@[48:49)      Integer |4|
//@[49:50)     RightSquare |]|
//@[50:51)    Comma |,|
//@[52:78)   FunctionArgumentSyntax
//@[52:78)    ForSyntax
//@[52:53)     LeftSquare |[|
//@[53:56)     Identifier |for|
//@[57:62)     LocalVariableSyntax
//@[57:62)      IdentifierSyntax
//@[57:62)       Identifier |thing|
//@[63:65)     Identifier |in|
//@[66:71)     VariableAccessSyntax
//@[66:71)      IdentifierSyntax
//@[66:71)       Identifier |stuff|
//@[71:72)     Colon |:|
//@[73:77)     BooleanLiteralSyntax
//@[73:77)      TrueKeyword |true|
//@[77:78)     RightSquare |]|
//@[78:79)   RightParen |)|
//@[79:81) NewLine |\n\n|

@batchSize(1)
//@[0:51) VariableDeclarationSyntax
//@[0:13)  DecoratorSyntax
//@[0:1)   At |@|
//@[1:13)   FunctionCallSyntax
//@[1:10)    IdentifierSyntax
//@[1:10)     Identifier |batchSize|
//@[10:11)    LeftParen |(|
//@[11:12)    FunctionArgumentSyntax
//@[11:12)     IntegerLiteralSyntax
//@[11:12)      Integer |1|
//@[12:13)    RightParen |)|
//@[13:14)  NewLine |\n|
var batchSizeMakesNoSenseHere = false
//@[0:3)  Identifier |var|
//@[4:29)  IdentifierSyntax
//@[4:29)   Identifier |batchSizeMakesNoSenseHere|
//@[30:31)  Assignment |=|
//@[32:37)  BooleanLiteralSyntax
//@[32:37)   FalseKeyword |false|
//@[37:40) NewLine |\n\n\n|


//KeyVault Secret Reference
//@[27:28) NewLine |\n|
resource kv 'Microsoft.KeyVault/vaults@2019-09-01' existing = {
//@[0:88) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:11)  IdentifierSyntax
//@[9:11)   Identifier |kv|
//@[12:50)  StringSyntax
//@[12:50)   StringComplete |'Microsoft.KeyVault/vaults@2019-09-01'|
//@[51:59)  Identifier |existing|
//@[60:61)  Assignment |=|
//@[62:88)  ObjectSyntax
//@[62:63)   LeftBrace |{|
//@[63:64)   NewLine |\n|
  name: 'testkeyvault'
//@[2:22)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:22)    StringSyntax
//@[8:22)     StringComplete |'testkeyvault'|
//@[22:23)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

var keyVaultSecretVar = kv.getSecret('mySecret')
//@[0:48) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:21)  IdentifierSyntax
//@[4:21)   Identifier |keyVaultSecretVar|
//@[22:23)  Assignment |=|
//@[24:48)  InstanceFunctionCallSyntax
//@[24:26)   VariableAccessSyntax
//@[24:26)    IdentifierSyntax
//@[24:26)     Identifier |kv|
//@[26:27)   Dot |.|
//@[27:36)   IdentifierSyntax
//@[27:36)    Identifier |getSecret|
//@[36:37)   LeftParen |(|
//@[37:47)   FunctionArgumentSyntax
//@[37:47)    StringSyntax
//@[37:47)     StringComplete |'mySecret'|
//@[47:48)   RightParen |)|
//@[48:49) NewLine |\n|
var keyVaultSecretInterpolatedVar = '${kv.getSecret('mySecret')}'
//@[0:65) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:33)  IdentifierSyntax
//@[4:33)   Identifier |keyVaultSecretInterpolatedVar|
//@[34:35)  Assignment |=|
//@[36:65)  StringSyntax
//@[36:39)   StringLeftPiece |'${|
//@[39:63)   InstanceFunctionCallSyntax
//@[39:41)    VariableAccessSyntax
//@[39:41)     IdentifierSyntax
//@[39:41)      Identifier |kv|
//@[41:42)    Dot |.|
//@[42:51)    IdentifierSyntax
//@[42:51)     Identifier |getSecret|
//@[51:52)    LeftParen |(|
//@[52:62)    FunctionArgumentSyntax
//@[52:62)     StringSyntax
//@[52:62)      StringComplete |'mySecret'|
//@[62:63)    RightParen |)|
//@[63:65)   StringRightPiece |}'|
//@[65:66) NewLine |\n|
var keyVaultSecretObjectVar = {
//@[0:68) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:27)  IdentifierSyntax
//@[4:27)   Identifier |keyVaultSecretObjectVar|
//@[28:29)  Assignment |=|
//@[30:68)  ObjectSyntax
//@[30:31)   LeftBrace |{|
//@[31:32)   NewLine |\n|
  secret: kv.getSecret('mySecret')
//@[2:34)   ObjectPropertySyntax
//@[2:8)    IdentifierSyntax
//@[2:8)     Identifier |secret|
//@[8:9)    Colon |:|
//@[10:34)    InstanceFunctionCallSyntax
//@[10:12)     VariableAccessSyntax
//@[10:12)      IdentifierSyntax
//@[10:12)       Identifier |kv|
//@[12:13)     Dot |.|
//@[13:22)     IdentifierSyntax
//@[13:22)      Identifier |getSecret|
//@[22:23)     LeftParen |(|
//@[23:33)     FunctionArgumentSyntax
//@[23:33)      StringSyntax
//@[23:33)       StringComplete |'mySecret'|
//@[33:34)     RightParen |)|
//@[34:35)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:2) NewLine |\n|
var keyVaultSecretArrayVar = [
//@[0:59) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:26)  IdentifierSyntax
//@[4:26)   Identifier |keyVaultSecretArrayVar|
//@[27:28)  Assignment |=|
//@[29:59)  ArraySyntax
//@[29:30)   LeftSquare |[|
//@[30:31)   NewLine |\n|
  kv.getSecret('mySecret')
//@[2:26)   ArrayItemSyntax
//@[2:26)    InstanceFunctionCallSyntax
//@[2:4)     VariableAccessSyntax
//@[2:4)      IdentifierSyntax
//@[2:4)       Identifier |kv|
//@[4:5)     Dot |.|
//@[5:14)     IdentifierSyntax
//@[5:14)      Identifier |getSecret|
//@[14:15)     LeftParen |(|
//@[15:25)     FunctionArgumentSyntax
//@[15:25)      StringSyntax
//@[15:25)       StringComplete |'mySecret'|
//@[25:26)     RightParen |)|
//@[26:27)   NewLine |\n|
]
//@[0:1)   RightSquare |]|
//@[1:2) NewLine |\n|
var keyVaultSecretArrayInterpolatedVar = [
//@[0:76) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:38)  IdentifierSyntax
//@[4:38)   Identifier |keyVaultSecretArrayInterpolatedVar|
//@[39:40)  Assignment |=|
//@[41:76)  ArraySyntax
//@[41:42)   LeftSquare |[|
//@[42:43)   NewLine |\n|
  '${kv.getSecret('mySecret')}'
//@[2:31)   ArrayItemSyntax
//@[2:31)    StringSyntax
//@[2:5)     StringLeftPiece |'${|
//@[5:29)     InstanceFunctionCallSyntax
//@[5:7)      VariableAccessSyntax
//@[5:7)       IdentifierSyntax
//@[5:7)        Identifier |kv|
//@[7:8)      Dot |.|
//@[8:17)      IdentifierSyntax
//@[8:17)       Identifier |getSecret|
//@[17:18)      LeftParen |(|
//@[18:28)      FunctionArgumentSyntax
//@[18:28)       StringSyntax
//@[18:28)        StringComplete |'mySecret'|
//@[28:29)      RightParen |)|
//@[29:31)     StringRightPiece |}'|
//@[31:32)   NewLine |\n|
]
//@[0:1)   RightSquare |]|
//@[1:2) NewLine |\n|

//@[0:0) EndOfFile ||
