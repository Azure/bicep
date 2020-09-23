
//@[0:2) NewLine |\r\n|
// wrong declaration
//@[20:22) NewLine |\r\n|
bad
//@[0:3) SkippedTriviaSyntax
//@[0:3)  Identifier |bad|
//@[3:7) NewLine |\r\n\r\n|

// incomplete
//@[13:15) NewLine |\r\n|
resource 
//@[0:8) SkippedTriviaSyntax
//@[0:8)  Identifier |resource|
//@[9:11) NewLine |\r\n|
resource foo
//@[0:12) SkippedTriviaSyntax
//@[0:8)  Identifier |resource|
//@[9:12)  Identifier |foo|
//@[12:14) NewLine |\r\n|
resource fo/o
//@[0:13) SkippedTriviaSyntax
//@[0:8)  Identifier |resource|
//@[9:11)  Identifier |fo|
//@[11:12)  Slash |/|
//@[12:13)  Identifier |o|
//@[13:15) NewLine |\r\n|
resource foo 'ddd'
//@[0:18) SkippedTriviaSyntax
//@[0:8)  Identifier |resource|
//@[9:12)  Identifier |foo|
//@[13:18)  StringComplete |'ddd'|
//@[18:20) NewLine |\r\n|
resource foo 'ddd'=
//@[0:19) SkippedTriviaSyntax
//@[0:8)  Identifier |resource|
//@[9:12)  Identifier |foo|
//@[13:18)  StringComplete |'ddd'|
//@[18:19)  Assignment |=|
//@[19:23) NewLine |\r\n\r\n|

// wrong resource type
//@[22:24) NewLine |\r\n|
resource foo 'ddd'={
//@[0:23) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:12)  IdentifierSyntax
//@[9:12)   Identifier |foo|
//@[13:18)  StringSyntax
//@[13:18)   StringComplete |'ddd'|
//@[18:19)  Assignment |=|
//@[19:23)  ObjectSyntax
//@[19:20)   LeftBrace |{|
//@[20:22)   NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

// using string interpolation for the resource type
//@[51:53) NewLine |\r\n|
resource foo 'Microsoft.${provider}/foos@2020-02-02-alpha'= {
//@[0:64) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:12)  IdentifierSyntax
//@[9:12)   Identifier |foo|
//@[13:58)  StringSyntax
//@[13:26)   StringLeftPiece |'Microsoft.${|
//@[26:34)   VariableAccessSyntax
//@[26:34)    IdentifierSyntax
//@[26:34)     Identifier |provider|
//@[34:58)   StringRightPiece |}/foos@2020-02-02-alpha'|
//@[58:59)  Assignment |=|
//@[60:64)  ObjectSyntax
//@[60:61)   LeftBrace |{|
//@[61:63)   NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

// missing required property
//@[28:30) NewLine |\r\n|
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'={
//@[0:55) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:12)  IdentifierSyntax
//@[9:12)   Identifier |foo|
//@[13:50)  StringSyntax
//@[13:50)   StringComplete |'Microsoft.Foo/foos@2020-02-02-alpha'|
//@[50:51)  Assignment |=|
//@[51:55)  ObjectSyntax
//@[51:52)   LeftBrace |{|
//@[52:54)   NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

// duplicate property at the top level
//@[38:40) NewLine |\r\n|
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= {
//@[0:85) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:12)  IdentifierSyntax
//@[9:12)   Identifier |foo|
//@[13:50)  StringSyntax
//@[13:50)   StringComplete |'Microsoft.Foo/foos@2020-02-02-alpha'|
//@[50:51)  Assignment |=|
//@[52:85)  ObjectSyntax
//@[52:53)   LeftBrace |{|
//@[53:55)   NewLine |\r\n|
  name: 'foo'
//@[2:15)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:13)    StringSyntax
//@[8:13)     StringComplete |'foo'|
//@[13:15)    NewLine |\r\n|
  name: true
//@[2:14)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:12)    BooleanLiteralSyntax
//@[8:12)     TrueKeyword |true|
//@[12:14)    NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

// duplicate property at the top level with string literal syntax
//@[65:67) NewLine |\r\n|
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= {
//@[0:87) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:12)  IdentifierSyntax
//@[9:12)   Identifier |foo|
//@[13:50)  StringSyntax
//@[13:50)   StringComplete |'Microsoft.Foo/foos@2020-02-02-alpha'|
//@[50:51)  Assignment |=|
//@[52:87)  ObjectSyntax
//@[52:53)   LeftBrace |{|
//@[53:55)   NewLine |\r\n|
  name: 'foo'
//@[2:15)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:13)    StringSyntax
//@[8:13)     StringComplete |'foo'|
//@[13:15)    NewLine |\r\n|
  'name': true
//@[2:16)   ObjectPropertySyntax
//@[2:8)    StringSyntax
//@[2:8)     StringComplete |'name'|
//@[8:9)    Colon |:|
//@[10:14)    BooleanLiteralSyntax
//@[10:14)     TrueKeyword |true|
//@[14:16)    NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

// duplicate property inside
//@[28:30) NewLine |\r\n|
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= {
//@[0:121) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:12)  IdentifierSyntax
//@[9:12)   Identifier |foo|
//@[13:50)  StringSyntax
//@[13:50)   StringComplete |'Microsoft.Foo/foos@2020-02-02-alpha'|
//@[50:51)  Assignment |=|
//@[52:121)  ObjectSyntax
//@[52:53)   LeftBrace |{|
//@[53:55)   NewLine |\r\n|
  name: 'foo'
//@[2:15)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:13)    StringSyntax
//@[8:13)     StringComplete |'foo'|
//@[13:15)    NewLine |\r\n|
  properties: {
//@[2:50)   ObjectPropertySyntax
//@[2:12)    IdentifierSyntax
//@[2:12)     Identifier |properties|
//@[12:13)    Colon |:|
//@[14:48)    ObjectSyntax
//@[14:15)     LeftBrace |{|
//@[15:17)     NewLine |\r\n|
    foo: 'a'
//@[4:14)     ObjectPropertySyntax
//@[4:7)      IdentifierSyntax
//@[4:7)       Identifier |foo|
//@[7:8)      Colon |:|
//@[9:12)      StringSyntax
//@[9:12)       StringComplete |'a'|
//@[12:14)      NewLine |\r\n|
    foo: 'a'
//@[4:14)     ObjectPropertySyntax
//@[4:7)      IdentifierSyntax
//@[4:7)       Identifier |foo|
//@[7:8)      Colon |:|
//@[9:12)      StringSyntax
//@[9:12)       StringComplete |'a'|
//@[12:14)      NewLine |\r\n|
  }
//@[2:3)     RightBrace |}|
//@[3:5)    NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

// duplicate property inside with string literal syntax
//@[55:57) NewLine |\r\n|
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= {
//@[0:123) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:12)  IdentifierSyntax
//@[9:12)   Identifier |foo|
//@[13:50)  StringSyntax
//@[13:50)   StringComplete |'Microsoft.Foo/foos@2020-02-02-alpha'|
//@[50:51)  Assignment |=|
//@[52:123)  ObjectSyntax
//@[52:53)   LeftBrace |{|
//@[53:55)   NewLine |\r\n|
  name: 'foo'
//@[2:15)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:13)    StringSyntax
//@[8:13)     StringComplete |'foo'|
//@[13:15)    NewLine |\r\n|
  properties: {
//@[2:52)   ObjectPropertySyntax
//@[2:12)    IdentifierSyntax
//@[2:12)     Identifier |properties|
//@[12:13)    Colon |:|
//@[14:50)    ObjectSyntax
//@[14:15)     LeftBrace |{|
//@[15:17)     NewLine |\r\n|
    foo: 'a'
//@[4:14)     ObjectPropertySyntax
//@[4:7)      IdentifierSyntax
//@[4:7)       Identifier |foo|
//@[7:8)      Colon |:|
//@[9:12)      StringSyntax
//@[9:12)       StringComplete |'a'|
//@[12:14)      NewLine |\r\n|
    'foo': 'a'
//@[4:16)     ObjectPropertySyntax
//@[4:9)      StringSyntax
//@[4:9)       StringComplete |'foo'|
//@[9:10)      Colon |:|
//@[11:14)      StringSyntax
//@[11:14)       StringComplete |'a'|
//@[14:16)      NewLine |\r\n|
  }
//@[2:3)     RightBrace |}|
//@[3:5)    NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

// wrong property types
//@[23:25) NewLine |\r\n|
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= {
//@[0:124) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:12)  IdentifierSyntax
//@[9:12)   Identifier |foo|
//@[13:50)  StringSyntax
//@[13:50)   StringComplete |'Microsoft.Foo/foos@2020-02-02-alpha'|
//@[50:51)  Assignment |=|
//@[52:124)  ObjectSyntax
//@[52:53)   LeftBrace |{|
//@[53:55)   NewLine |\r\n|
  name: 'foo'
//@[2:15)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:13)    StringSyntax
//@[8:13)     StringComplete |'foo'|
//@[13:15)    NewLine |\r\n|
  location: [
//@[2:20)   ObjectPropertySyntax
//@[2:10)    IdentifierSyntax
//@[2:10)     Identifier |location|
//@[10:11)    Colon |:|
//@[12:18)    ArraySyntax
//@[12:13)     LeftSquare |[|
//@[13:15)     NewLine |\r\n|
  ]
//@[2:3)     RightSquare |]|
//@[3:5)    NewLine |\r\n|
  tags: 'tag are not a string?'
//@[2:33)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |tags|
//@[6:7)    Colon |:|
//@[8:31)    StringSyntax
//@[8:31)     StringComplete |'tag are not a string?'|
//@[31:33)    NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource bar 'Microsoft.Foo/foos@2020-02-02-alpha' = {
//@[0:231) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:12)  IdentifierSyntax
//@[9:12)   Identifier |bar|
//@[13:50)  StringSyntax
//@[13:50)   StringComplete |'Microsoft.Foo/foos@2020-02-02-alpha'|
//@[51:52)  Assignment |=|
//@[53:231)  ObjectSyntax
//@[53:54)   LeftBrace |{|
//@[54:56)   NewLine |\r\n|
  name: true ? 's' : 'a' + 1
//@[2:30)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:28)    TernaryOperationSyntax
//@[8:12)     BooleanLiteralSyntax
//@[8:12)      TrueKeyword |true|
//@[13:14)     Question |?|
//@[15:18)     StringSyntax
//@[15:18)      StringComplete |'s'|
//@[19:20)     Colon |:|
//@[21:28)     BinaryOperationSyntax
//@[21:24)      StringSyntax
//@[21:24)       StringComplete |'a'|
//@[25:26)      Plus |+|
//@[27:28)      NumericLiteralSyntax
//@[27:28)       Number |1|
//@[28:30)    NewLine |\r\n|
  properties: {
//@[2:144)   ObjectPropertySyntax
//@[2:12)    IdentifierSyntax
//@[2:12)     Identifier |properties|
//@[12:13)    Colon |:|
//@[14:142)    ObjectSyntax
//@[14:15)     LeftBrace |{|
//@[15:17)     NewLine |\r\n|
    x: foo()
//@[4:14)     ObjectPropertySyntax
//@[4:5)      IdentifierSyntax
//@[4:5)       Identifier |x|
//@[5:6)      Colon |:|
//@[7:12)      FunctionCallSyntax
//@[7:10)       IdentifierSyntax
//@[7:10)        Identifier |foo|
//@[10:11)       LeftParen |(|
//@[11:12)       RightParen |)|
//@[12:14)      NewLine |\r\n|
    y: true && (null || !4)
//@[4:29)     ObjectPropertySyntax
//@[4:5)      IdentifierSyntax
//@[4:5)       Identifier |y|
//@[5:6)      Colon |:|
//@[7:27)      BinaryOperationSyntax
//@[7:11)       BooleanLiteralSyntax
//@[7:11)        TrueKeyword |true|
//@[12:14)       LogicalAnd |&&|
//@[15:27)       ParenthesizedExpressionSyntax
//@[15:16)        LeftParen |(|
//@[16:26)        BinaryOperationSyntax
//@[16:20)         NullLiteralSyntax
//@[16:20)          NullKeyword |null|
//@[21:23)         LogicalOr ||||
//@[24:26)         UnaryOperationSyntax
//@[24:25)          Exclamation |!|
//@[25:26)          NumericLiteralSyntax
//@[25:26)           Number |4|
//@[26:27)        RightParen |)|
//@[27:29)      NewLine |\r\n|
    a: [
//@[4:79)     ObjectPropertySyntax
//@[4:5)      IdentifierSyntax
//@[4:5)       Identifier |a|
//@[5:6)      Colon |:|
//@[7:77)      ArraySyntax
//@[7:8)       LeftSquare |[|
//@[8:10)       NewLine |\r\n|
      a
//@[6:9)       ArrayItemSyntax
//@[6:7)        VariableAccessSyntax
//@[6:7)         IdentifierSyntax
//@[6:7)          Identifier |a|
//@[7:9)        NewLine |\r\n|
      !null
//@[6:13)       ArrayItemSyntax
//@[6:11)        UnaryOperationSyntax
//@[6:7)         Exclamation |!|
//@[7:11)         NullLiteralSyntax
//@[7:11)          NullKeyword |null|
//@[11:13)        NewLine |\r\n|
      true && true || true + -true * 4
//@[6:40)       ArrayItemSyntax
//@[6:38)        BinaryOperationSyntax
//@[6:18)         BinaryOperationSyntax
//@[6:10)          BooleanLiteralSyntax
//@[6:10)           TrueKeyword |true|
//@[11:13)          LogicalAnd |&&|
//@[14:18)          BooleanLiteralSyntax
//@[14:18)           TrueKeyword |true|
//@[19:21)         LogicalOr ||||
//@[22:38)         BinaryOperationSyntax
//@[22:26)          BooleanLiteralSyntax
//@[22:26)           TrueKeyword |true|
//@[27:28)          Plus |+|
//@[29:38)          BinaryOperationSyntax
//@[29:34)           UnaryOperationSyntax
//@[29:30)            Minus |-|
//@[30:34)            BooleanLiteralSyntax
//@[30:34)             TrueKeyword |true|
//@[35:36)           Asterisk |*|
//@[37:38)           NumericLiteralSyntax
//@[37:38)            Number |4|
//@[38:40)        NewLine |\r\n|
    ]
//@[4:5)       RightSquare |]|
//@[5:7)      NewLine |\r\n|
  }
//@[2:3)     RightBrace |}|
//@[3:5)    NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

// unsupported resource ref
//@[27:29) NewLine |\r\n|
var resrefvar = bar.name
//@[0:24) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:13)  IdentifierSyntax
//@[4:13)   Identifier |resrefvar|
//@[14:15)  Assignment |=|
//@[16:24)  PropertyAccessSyntax
//@[16:19)   VariableAccessSyntax
//@[16:19)    IdentifierSyntax
//@[16:19)     Identifier |bar|
//@[19:20)   Dot |.|
//@[20:24)   IdentifierSyntax
//@[20:24)    Identifier |name|
//@[24:28) NewLine |\r\n\r\n|

param resrefpar string = foo.id
//@[0:31) ParameterDeclarationSyntax
//@[0:5)  Identifier |param|
//@[6:15)  IdentifierSyntax
//@[6:15)   Identifier |resrefpar|
//@[16:22)  TypeSyntax
//@[16:22)   Identifier |string|
//@[23:31)  ParameterDefaultValueSyntax
//@[23:24)   Assignment |=|
//@[25:31)   PropertyAccessSyntax
//@[25:28)    VariableAccessSyntax
//@[25:28)     IdentifierSyntax
//@[25:28)      Identifier |foo|
//@[28:29)    Dot |.|
//@[29:31)    IdentifierSyntax
//@[29:31)     Identifier |id|
//@[31:35) NewLine |\r\n\r\n|

output resrefout bool = bar.id
//@[0:30) OutputDeclarationSyntax
//@[0:6)  Identifier |output|
//@[7:16)  IdentifierSyntax
//@[7:16)   Identifier |resrefout|
//@[17:21)  TypeSyntax
//@[17:21)   Identifier |bool|
//@[22:23)  Assignment |=|
//@[24:30)  PropertyAccessSyntax
//@[24:27)   VariableAccessSyntax
//@[24:27)    IdentifierSyntax
//@[24:27)     Identifier |bar|
//@[27:28)   Dot |.|
//@[28:30)   IdentifierSyntax
//@[28:30)    Identifier |id|
//@[30:34) NewLine |\r\n\r\n|

// attempting to set read-only properties
//@[41:43) NewLine |\r\n|
resource baz 'Microsoft.Foo/foos@2020-02-02-alpha' = {
//@[0:119) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:12)  IdentifierSyntax
//@[9:12)   Identifier |baz|
//@[13:50)  StringSyntax
//@[13:50)   StringComplete |'Microsoft.Foo/foos@2020-02-02-alpha'|
//@[51:52)  Assignment |=|
//@[53:119)  ObjectSyntax
//@[53:54)   LeftBrace |{|
//@[54:56)   NewLine |\r\n|
  name: 'test'
//@[2:16)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:14)    StringSyntax
//@[8:14)     StringComplete |'test'|
//@[14:16)    NewLine |\r\n|
  id: 2
//@[2:9)   ObjectPropertySyntax
//@[2:4)    IdentifierSyntax
//@[2:4)     Identifier |id|
//@[4:5)    Colon |:|
//@[6:7)    NumericLiteralSyntax
//@[6:7)     Number |2|
//@[7:9)    NewLine |\r\n|
  type: 'hello'
//@[2:17)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |type|
//@[6:7)    Colon |:|
//@[8:15)    StringSyntax
//@[8:15)     StringComplete |'hello'|
//@[15:17)    NewLine |\r\n|
  apiVersion: true
//@[2:20)   ObjectPropertySyntax
//@[2:12)    IdentifierSyntax
//@[2:12)     Identifier |apiVersion|
//@[12:13)    Colon |:|
//@[14:18)    BooleanLiteralSyntax
//@[14:18)     TrueKeyword |true|
//@[18:20)    NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource badDepends 'Microsoft.Foo/foos@2020-02-02-alpha' = {
//@[0:113) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:19)  IdentifierSyntax
//@[9:19)   Identifier |badDepends|
//@[20:57)  StringSyntax
//@[20:57)   StringComplete |'Microsoft.Foo/foos@2020-02-02-alpha'|
//@[58:59)  Assignment |=|
//@[60:113)  ObjectSyntax
//@[60:61)   LeftBrace |{|
//@[61:63)   NewLine |\r\n|
  name: 'test'
//@[2:16)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:14)    StringSyntax
//@[8:14)     StringComplete |'test'|
//@[14:16)    NewLine |\r\n|
  dependsOn: [
//@[2:33)   ObjectPropertySyntax
//@[2:11)    IdentifierSyntax
//@[2:11)     Identifier |dependsOn|
//@[11:12)    Colon |:|
//@[13:31)    ArraySyntax
//@[13:14)     LeftSquare |[|
//@[14:16)     NewLine |\r\n|
    baz.id
//@[4:12)     ArrayItemSyntax
//@[4:10)      PropertyAccessSyntax
//@[4:7)       VariableAccessSyntax
//@[4:7)        IdentifierSyntax
//@[4:7)         Identifier |baz|
//@[7:8)       Dot |.|
//@[8:10)       IdentifierSyntax
//@[8:10)        Identifier |id|
//@[10:12)      NewLine |\r\n|
  ]
//@[2:3)     RightSquare |]|
//@[3:5)    NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource badDepends2 'Microsoft.Foo/foos@2020-02-02-alpha' = {
//@[0:125) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:20)  IdentifierSyntax
//@[9:20)   Identifier |badDepends2|
//@[21:58)  StringSyntax
//@[21:58)   StringComplete |'Microsoft.Foo/foos@2020-02-02-alpha'|
//@[59:60)  Assignment |=|
//@[61:125)  ObjectSyntax
//@[61:62)   LeftBrace |{|
//@[62:64)   NewLine |\r\n|
  name: 'test'
//@[2:16)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:14)    StringSyntax
//@[8:14)     StringComplete |'test'|
//@[14:16)    NewLine |\r\n|
  dependsOn: [
//@[2:44)   ObjectPropertySyntax
//@[2:11)    IdentifierSyntax
//@[2:11)     Identifier |dependsOn|
//@[11:12)    Colon |:|
//@[13:42)    ArraySyntax
//@[13:14)     LeftSquare |[|
//@[14:16)     NewLine |\r\n|
    'hello'
//@[4:13)     ArrayItemSyntax
//@[4:11)      StringSyntax
//@[4:11)       StringComplete |'hello'|
//@[11:13)      NewLine |\r\n|
    true
//@[4:10)     ArrayItemSyntax
//@[4:8)      BooleanLiteralSyntax
//@[4:8)       TrueKeyword |true|
//@[8:10)      NewLine |\r\n|
  ]
//@[2:3)     RightSquare |]|
//@[3:5)    NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource badDepends3 'Microsoft.Foo/foos@2020-02-02-alpha' = {
//@[0:81) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:20)  IdentifierSyntax
//@[9:20)   Identifier |badDepends3|
//@[21:58)  StringSyntax
//@[21:58)   StringComplete |'Microsoft.Foo/foos@2020-02-02-alpha'|
//@[59:60)  Assignment |=|
//@[61:81)  ObjectSyntax
//@[61:62)   LeftBrace |{|
//@[62:64)   NewLine |\r\n|
  name: 'test'
//@[2:16)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:14)    StringSyntax
//@[8:14)     StringComplete |'test'|
//@[14:16)    NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource badDepends4 'Microsoft.Foo/foos@2020-02-02-alpha' = {
//@[0:119) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:20)  IdentifierSyntax
//@[9:20)   Identifier |badDepends4|
//@[21:58)  StringSyntax
//@[21:58)   StringComplete |'Microsoft.Foo/foos@2020-02-02-alpha'|
//@[59:60)  Assignment |=|
//@[61:119)  ObjectSyntax
//@[61:62)   LeftBrace |{|
//@[62:64)   NewLine |\r\n|
  name: 'test'
//@[2:16)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:14)    StringSyntax
//@[8:14)     StringComplete |'test'|
//@[14:16)    NewLine |\r\n|
  dependsOn: [
//@[2:38)   ObjectPropertySyntax
//@[2:11)    IdentifierSyntax
//@[2:11)     Identifier |dependsOn|
//@[11:12)    Colon |:|
//@[13:36)    ArraySyntax
//@[13:14)     LeftSquare |[|
//@[14:16)     NewLine |\r\n|
    badDepends3
//@[4:17)     ArrayItemSyntax
//@[4:15)      VariableAccessSyntax
//@[4:15)       IdentifierSyntax
//@[4:15)        Identifier |badDepends3|
//@[15:17)      NewLine |\r\n|
  ]
//@[2:3)     RightSquare |]|
//@[3:5)    NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource badDepends5 'Microsoft.Foo/foos@2020-02-02-alpha' = {
//@[0:117) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:20)  IdentifierSyntax
//@[9:20)   Identifier |badDepends5|
//@[21:58)  StringSyntax
//@[21:58)   StringComplete |'Microsoft.Foo/foos@2020-02-02-alpha'|
//@[59:60)  Assignment |=|
//@[61:117)  ObjectSyntax
//@[61:62)   LeftBrace |{|
//@[62:64)   NewLine |\r\n|
  name: 'test'
//@[2:16)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:14)    StringSyntax
//@[8:14)     StringComplete |'test'|
//@[14:16)    NewLine |\r\n|
  dependsOn: badDepends3.dependsOn
//@[2:36)   ObjectPropertySyntax
//@[2:11)    IdentifierSyntax
//@[2:11)     Identifier |dependsOn|
//@[11:12)    Colon |:|
//@[13:34)    PropertyAccessSyntax
//@[13:24)     VariableAccessSyntax
//@[13:24)      IdentifierSyntax
//@[13:24)       Identifier |badDepends3|
//@[24:25)     Dot |.|
//@[25:34)     IdentifierSyntax
//@[25:34)      Identifier |dependsOn|
//@[34:36)    NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:1) EndOfFile ||
