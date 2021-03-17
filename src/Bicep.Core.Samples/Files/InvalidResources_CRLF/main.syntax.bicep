
//@[0:2) NewLine |\r\n|
// wrong declaration
//@[20:22) NewLine |\r\n|
bad
//@[0:3) SkippedTriviaSyntax
//@[0:3)  Identifier |bad|
//@[3:7) NewLine |\r\n\r\n|

// incomplete #completionTest(9) -> empty
//@[41:43) NewLine |\r\n|
resource 
//@[0:9) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:9)  IdentifierSyntax
//@[9:9)   SkippedTriviaSyntax
//@[9:9)  SkippedTriviaSyntax
//@[9:9)  SkippedTriviaSyntax
//@[9:9)  SkippedTriviaSyntax
//@[9:11) NewLine |\r\n|
resource foo
//@[0:12) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:12)  IdentifierSyntax
//@[9:12)   Identifier |foo|
//@[12:12)  SkippedTriviaSyntax
//@[12:12)  SkippedTriviaSyntax
//@[12:12)  SkippedTriviaSyntax
//@[12:14) NewLine |\r\n|
resource fo/o
//@[0:13) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:11)  IdentifierSyntax
//@[9:11)   Identifier |fo|
//@[11:13)  SkippedTriviaSyntax
//@[11:12)   Slash |/|
//@[12:13)   Identifier |o|
//@[13:13)  SkippedTriviaSyntax
//@[13:13)  SkippedTriviaSyntax
//@[13:15) NewLine |\r\n|
resource foo 'ddd'
//@[0:18) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:12)  IdentifierSyntax
//@[9:12)   Identifier |foo|
//@[13:18)  StringSyntax
//@[13:18)   StringComplete |'ddd'|
//@[18:18)  SkippedTriviaSyntax
//@[18:18)  SkippedTriviaSyntax
//@[18:22) NewLine |\r\n\r\n|

// #completionTest(23) -> resourceTypes
//@[39:41) NewLine |\r\n|
resource trailingSpace  
//@[0:24) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:22)  IdentifierSyntax
//@[9:22)   Identifier |trailingSpace|
//@[24:24)  SkippedTriviaSyntax
//@[24:24)  SkippedTriviaSyntax
//@[24:24)  SkippedTriviaSyntax
//@[24:28) NewLine |\r\n\r\n|

// #completionTest(19,20) -> object
//@[35:37) NewLine |\r\n|
resource foo 'ddd'= 
//@[0:20) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:12)  IdentifierSyntax
//@[9:12)   Identifier |foo|
//@[13:18)  StringSyntax
//@[13:18)   StringComplete |'ddd'|
//@[18:19)  Assignment |=|
//@[20:20)  SkippedTriviaSyntax
//@[20:24) NewLine |\r\n\r\n|

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

resource foo 'ddd'=if (1 + 1 == 2) {
//@[0:39) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:12)  IdentifierSyntax
//@[9:12)   Identifier |foo|
//@[13:18)  StringSyntax
//@[13:18)   StringComplete |'ddd'|
//@[18:19)  Assignment |=|
//@[19:39)  IfConditionSyntax
//@[19:21)   Identifier |if|
//@[22:34)   ParenthesizedExpressionSyntax
//@[22:23)    LeftParen |(|
//@[23:33)    BinaryOperationSyntax
//@[23:28)     BinaryOperationSyntax
//@[23:24)      IntegerLiteralSyntax
//@[23:24)       Integer |1|
//@[25:26)      Plus |+|
//@[27:28)      IntegerLiteralSyntax
//@[27:28)       Integer |1|
//@[29:31)     Equals |==|
//@[32:33)     IntegerLiteralSyntax
//@[32:33)      Integer |2|
//@[33:34)    RightParen |)|
//@[35:39)   ObjectSyntax
//@[35:36)    LeftBrace |{|
//@[36:38)    NewLine |\r\n|
}
//@[0:1)    RightBrace |}|
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

resource foo 'Microsoft.${provider}/foos@2020-02-02-alpha'= if (true) {
//@[0:74) ResourceDeclarationSyntax
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
//@[60:74)  IfConditionSyntax
//@[60:62)   Identifier |if|
//@[63:69)   ParenthesizedExpressionSyntax
//@[63:64)    LeftParen |(|
//@[64:68)    BooleanLiteralSyntax
//@[64:68)     TrueKeyword |true|
//@[68:69)    RightParen |)|
//@[70:74)   ObjectSyntax
//@[70:71)    LeftBrace |{|
//@[71:73)    NewLine |\r\n|
}
//@[0:1)    RightBrace |}|
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

resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= if (name == 'value') {
//@[0:77) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:12)  IdentifierSyntax
//@[9:12)   Identifier |foo|
//@[13:50)  StringSyntax
//@[13:50)   StringComplete |'Microsoft.Foo/foos@2020-02-02-alpha'|
//@[50:51)  Assignment |=|
//@[52:77)  IfConditionSyntax
//@[52:54)   Identifier |if|
//@[55:72)   ParenthesizedExpressionSyntax
//@[55:56)    LeftParen |(|
//@[56:71)    BinaryOperationSyntax
//@[56:60)     VariableAccessSyntax
//@[56:60)      IdentifierSyntax
//@[56:60)       Identifier |name|
//@[61:63)     Equals |==|
//@[64:71)     StringSyntax
//@[64:71)      StringComplete |'value'|
//@[71:72)    RightParen |)|
//@[73:77)   ObjectSyntax
//@[73:74)    LeftBrace |{|
//@[74:76)    NewLine |\r\n|
}
//@[0:1)    RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= if ({ 'a': b }.a == 'foo') {
//@[0:83) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:12)  IdentifierSyntax
//@[9:12)   Identifier |foo|
//@[13:50)  StringSyntax
//@[13:50)   StringComplete |'Microsoft.Foo/foos@2020-02-02-alpha'|
//@[50:51)  Assignment |=|
//@[52:83)  IfConditionSyntax
//@[52:54)   Identifier |if|
//@[55:83)   ParenthesizedExpressionSyntax
//@[55:56)    LeftParen |(|
//@[56:83)    ObjectSyntax
//@[56:57)     LeftBrace |{|
//@[58:64)     ObjectPropertySyntax
//@[58:61)      StringSyntax
//@[58:61)       StringComplete |'a'|
//@[61:62)      Colon |:|
//@[63:64)      VariableAccessSyntax
//@[63:64)       IdentifierSyntax
//@[63:64)        Identifier |b|
//@[65:82)     SkippedTriviaSyntax
//@[65:66)      RightBrace |}|
//@[66:67)      Dot |.|
//@[67:68)      Identifier |a|
//@[69:71)      Equals |==|
//@[72:77)      StringComplete |'foo'|
//@[77:78)      RightParen |)|
//@[79:80)      LeftBrace |{|
//@[80:82)      NewLine |\r\n|
}
//@[0:1)     RightBrace |}|
//@[1:1)    SkippedTriviaSyntax
//@[1:1)   SkippedTriviaSyntax
//@[1:5) NewLine |\r\n\r\n|

// simulate typing if condition
//@[31:33) NewLine |\r\n|
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= if
//@[0:54) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:12)  IdentifierSyntax
//@[9:12)   Identifier |foo|
//@[13:50)  StringSyntax
//@[13:50)   StringComplete |'Microsoft.Foo/foos@2020-02-02-alpha'|
//@[50:51)  Assignment |=|
//@[52:54)  IfConditionSyntax
//@[52:54)   Identifier |if|
//@[54:54)   SkippedTriviaSyntax
//@[54:54)   SkippedTriviaSyntax
//@[54:58) NewLine |\r\n\r\n|

resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= if (
//@[0:56) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:12)  IdentifierSyntax
//@[9:12)   Identifier |foo|
//@[13:50)  StringSyntax
//@[13:50)   StringComplete |'Microsoft.Foo/foos@2020-02-02-alpha'|
//@[50:51)  Assignment |=|
//@[52:56)  IfConditionSyntax
//@[52:54)   Identifier |if|
//@[55:56)   ParenthesizedExpressionSyntax
//@[55:56)    LeftParen |(|
//@[56:56)    SkippedTriviaSyntax
//@[56:56)    SkippedTriviaSyntax
//@[56:56)   SkippedTriviaSyntax
//@[56:60) NewLine |\r\n\r\n|

resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= if (true
//@[0:60) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:12)  IdentifierSyntax
//@[9:12)   Identifier |foo|
//@[13:50)  StringSyntax
//@[13:50)   StringComplete |'Microsoft.Foo/foos@2020-02-02-alpha'|
//@[50:51)  Assignment |=|
//@[52:60)  IfConditionSyntax
//@[52:54)   Identifier |if|
//@[55:60)   ParenthesizedExpressionSyntax
//@[55:56)    LeftParen |(|
//@[56:60)    BooleanLiteralSyntax
//@[56:60)     TrueKeyword |true|
//@[60:60)    SkippedTriviaSyntax
//@[60:60)   SkippedTriviaSyntax
//@[60:64) NewLine |\r\n\r\n|

resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= if (true)
//@[0:61) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:12)  IdentifierSyntax
//@[9:12)   Identifier |foo|
//@[13:50)  StringSyntax
//@[13:50)   StringComplete |'Microsoft.Foo/foos@2020-02-02-alpha'|
//@[50:51)  Assignment |=|
//@[52:61)  IfConditionSyntax
//@[52:54)   Identifier |if|
//@[55:61)   ParenthesizedExpressionSyntax
//@[55:56)    LeftParen |(|
//@[56:60)    BooleanLiteralSyntax
//@[56:60)     TrueKeyword |true|
//@[60:61)    RightParen |)|
//@[61:61)   SkippedTriviaSyntax
//@[61:65) NewLine |\r\n\r\n|

// missing condition
//@[20:22) NewLine |\r\n|
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= if {
//@[0:74) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:12)  IdentifierSyntax
//@[9:12)   Identifier |foo|
//@[13:50)  StringSyntax
//@[13:50)   StringComplete |'Microsoft.Foo/foos@2020-02-02-alpha'|
//@[50:51)  Assignment |=|
//@[52:74)  IfConditionSyntax
//@[52:54)   Identifier |if|
//@[55:55)   SkippedTriviaSyntax
//@[55:74)   ObjectSyntax
//@[55:56)    LeftBrace |{|
//@[56:58)    NewLine |\r\n|
  name: 'foo'
//@[2:13)    ObjectPropertySyntax
//@[2:6)     IdentifierSyntax
//@[2:6)      Identifier |name|
//@[6:7)     Colon |:|
//@[8:13)     StringSyntax
//@[8:13)      StringComplete |'foo'|
//@[13:15)    NewLine |\r\n|
}
//@[0:1)    RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

// empty condition
//@[18:20) NewLine |\r\n|
// #completionTest(56) -> symbols
//@[33:35) NewLine |\r\n|
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= if () {
//@[0:77) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:12)  IdentifierSyntax
//@[9:12)   Identifier |foo|
//@[13:50)  StringSyntax
//@[13:50)   StringComplete |'Microsoft.Foo/foos@2020-02-02-alpha'|
//@[50:51)  Assignment |=|
//@[52:77)  IfConditionSyntax
//@[52:54)   Identifier |if|
//@[55:57)   ParenthesizedExpressionSyntax
//@[55:56)    LeftParen |(|
//@[56:56)    SkippedTriviaSyntax
//@[56:57)    RightParen |)|
//@[58:77)   ObjectSyntax
//@[58:59)    LeftBrace |{|
//@[59:61)    NewLine |\r\n|
  name: 'foo'
//@[2:13)    ObjectPropertySyntax
//@[2:6)     IdentifierSyntax
//@[2:6)      Identifier |name|
//@[6:7)     Colon |:|
//@[8:13)     StringSyntax
//@[8:13)      StringComplete |'foo'|
//@[13:15)    NewLine |\r\n|
}
//@[0:1)    RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

// #completionTest(57, 59) -> symbols
//@[37:39) NewLine |\r\n|
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= if (     ) {
//@[0:82) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:12)  IdentifierSyntax
//@[9:12)   Identifier |foo|
//@[13:50)  StringSyntax
//@[13:50)   StringComplete |'Microsoft.Foo/foos@2020-02-02-alpha'|
//@[50:51)  Assignment |=|
//@[52:82)  IfConditionSyntax
//@[52:54)   Identifier |if|
//@[55:62)   ParenthesizedExpressionSyntax
//@[55:56)    LeftParen |(|
//@[61:61)    SkippedTriviaSyntax
//@[61:62)    RightParen |)|
//@[63:82)   ObjectSyntax
//@[63:64)    LeftBrace |{|
//@[64:66)    NewLine |\r\n|
  name: 'foo'
//@[2:13)    ObjectPropertySyntax
//@[2:6)     IdentifierSyntax
//@[2:6)      Identifier |name|
//@[6:7)     Colon |:|
//@[8:13)     StringSyntax
//@[8:13)      StringComplete |'foo'|
//@[13:15)    NewLine |\r\n|
}
//@[0:1)    RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

// invalid condition type
//@[25:27) NewLine |\r\n|
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= if (123) {
//@[0:80) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:12)  IdentifierSyntax
//@[9:12)   Identifier |foo|
//@[13:50)  StringSyntax
//@[13:50)   StringComplete |'Microsoft.Foo/foos@2020-02-02-alpha'|
//@[50:51)  Assignment |=|
//@[52:80)  IfConditionSyntax
//@[52:54)   Identifier |if|
//@[55:60)   ParenthesizedExpressionSyntax
//@[55:56)    LeftParen |(|
//@[56:59)    IntegerLiteralSyntax
//@[56:59)     Integer |123|
//@[59:60)    RightParen |)|
//@[61:80)   ObjectSyntax
//@[61:62)    LeftBrace |{|
//@[62:64)    NewLine |\r\n|
  name: 'foo'
//@[2:13)    ObjectPropertySyntax
//@[2:6)     IdentifierSyntax
//@[2:6)      Identifier |name|
//@[6:7)     Colon |:|
//@[8:13)     StringSyntax
//@[8:13)      StringComplete |'foo'|
//@[13:15)    NewLine |\r\n|
}
//@[0:1)    RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

// runtime functions are no allowed in resource conditions
//@[58:60) NewLine |\r\n|
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha' = if (reference('Micorosft.Management/managementGroups/MG', '2020-05-01').name == 'something') {
//@[0:165) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:12)  IdentifierSyntax
//@[9:12)   Identifier |foo|
//@[13:50)  StringSyntax
//@[13:50)   StringComplete |'Microsoft.Foo/foos@2020-02-02-alpha'|
//@[51:52)  Assignment |=|
//@[53:165)  IfConditionSyntax
//@[53:55)   Identifier |if|
//@[56:145)   ParenthesizedExpressionSyntax
//@[56:57)    LeftParen |(|
//@[57:144)    BinaryOperationSyntax
//@[57:129)     PropertyAccessSyntax
//@[57:124)      FunctionCallSyntax
//@[57:66)       IdentifierSyntax
//@[57:66)        Identifier |reference|
//@[66:67)       LeftParen |(|
//@[67:110)       FunctionArgumentSyntax
//@[67:109)        StringSyntax
//@[67:109)         StringComplete |'Micorosft.Management/managementGroups/MG'|
//@[109:110)        Comma |,|
//@[111:123)       FunctionArgumentSyntax
//@[111:123)        StringSyntax
//@[111:123)         StringComplete |'2020-05-01'|
//@[123:124)       RightParen |)|
//@[124:125)      Dot |.|
//@[125:129)      IdentifierSyntax
//@[125:129)       Identifier |name|
//@[130:132)     Equals |==|
//@[133:144)     StringSyntax
//@[133:144)      StringComplete |'something'|
//@[144:145)    RightParen |)|
//@[146:165)   ObjectSyntax
//@[146:147)    LeftBrace |{|
//@[147:149)    NewLine |\r\n|
  name: 'foo'
//@[2:13)    ObjectPropertySyntax
//@[2:6)     IdentifierSyntax
//@[2:6)      Identifier |name|
//@[6:7)     Colon |:|
//@[8:13)     StringSyntax
//@[8:13)      StringComplete |'foo'|
//@[13:15)    NewLine |\r\n|
}
//@[0:1)    RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource foo 'Microsoft.Foo/foos@2020-02-02-alpha' = if (listKeys('foo', '2020-05-01').bar == true) {
//@[0:119) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:12)  IdentifierSyntax
//@[9:12)   Identifier |foo|
//@[13:50)  StringSyntax
//@[13:50)   StringComplete |'Microsoft.Foo/foos@2020-02-02-alpha'|
//@[51:52)  Assignment |=|
//@[53:119)  IfConditionSyntax
//@[53:55)   Identifier |if|
//@[56:99)   ParenthesizedExpressionSyntax
//@[56:57)    LeftParen |(|
//@[57:98)    BinaryOperationSyntax
//@[57:90)     PropertyAccessSyntax
//@[57:86)      FunctionCallSyntax
//@[57:65)       IdentifierSyntax
//@[57:65)        Identifier |listKeys|
//@[65:66)       LeftParen |(|
//@[66:72)       FunctionArgumentSyntax
//@[66:71)        StringSyntax
//@[66:71)         StringComplete |'foo'|
//@[71:72)        Comma |,|
//@[73:85)       FunctionArgumentSyntax
//@[73:85)        StringSyntax
//@[73:85)         StringComplete |'2020-05-01'|
//@[85:86)       RightParen |)|
//@[86:87)      Dot |.|
//@[87:90)      IdentifierSyntax
//@[87:90)       Identifier |bar|
//@[91:93)     Equals |==|
//@[94:98)     BooleanLiteralSyntax
//@[94:98)      TrueKeyword |true|
//@[98:99)    RightParen |)|
//@[100:119)   ObjectSyntax
//@[100:101)    LeftBrace |{|
//@[101:103)    NewLine |\r\n|
  name: 'foo'
//@[2:13)    ObjectPropertySyntax
//@[2:6)     IdentifierSyntax
//@[2:6)      Identifier |name|
//@[6:7)     Colon |:|
//@[8:13)     StringSyntax
//@[8:13)      StringComplete |'foo'|
//@[13:15)    NewLine |\r\n|
}
//@[0:1)    RightBrace |}|
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
//@[2:13)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:13)    StringSyntax
//@[8:13)     StringComplete |'foo'|
//@[13:15)   NewLine |\r\n|
  name: true
//@[2:12)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:12)    BooleanLiteralSyntax
//@[8:12)     TrueKeyword |true|
//@[12:14)   NewLine |\r\n|
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
//@[2:13)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:13)    StringSyntax
//@[8:13)     StringComplete |'foo'|
//@[13:15)   NewLine |\r\n|
  'name': true
//@[2:14)   ObjectPropertySyntax
//@[2:8)    StringSyntax
//@[2:8)     StringComplete |'name'|
//@[8:9)    Colon |:|
//@[10:14)    BooleanLiteralSyntax
//@[10:14)     TrueKeyword |true|
//@[14:16)   NewLine |\r\n|
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
//@[2:13)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:13)    StringSyntax
//@[8:13)     StringComplete |'foo'|
//@[13:15)   NewLine |\r\n|
  properties: {
//@[2:48)   ObjectPropertySyntax
//@[2:12)    IdentifierSyntax
//@[2:12)     Identifier |properties|
//@[12:13)    Colon |:|
//@[14:48)    ObjectSyntax
//@[14:15)     LeftBrace |{|
//@[15:17)     NewLine |\r\n|
    foo: 'a'
//@[4:12)     ObjectPropertySyntax
//@[4:7)      IdentifierSyntax
//@[4:7)       Identifier |foo|
//@[7:8)      Colon |:|
//@[9:12)      StringSyntax
//@[9:12)       StringComplete |'a'|
//@[12:14)     NewLine |\r\n|
    foo: 'a'
//@[4:12)     ObjectPropertySyntax
//@[4:7)      IdentifierSyntax
//@[4:7)       Identifier |foo|
//@[7:8)      Colon |:|
//@[9:12)      StringSyntax
//@[9:12)       StringComplete |'a'|
//@[12:14)     NewLine |\r\n|
  }
//@[2:3)     RightBrace |}|
//@[3:5)   NewLine |\r\n|
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
//@[2:13)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:13)    StringSyntax
//@[8:13)     StringComplete |'foo'|
//@[13:15)   NewLine |\r\n|
  properties: {
//@[2:50)   ObjectPropertySyntax
//@[2:12)    IdentifierSyntax
//@[2:12)     Identifier |properties|
//@[12:13)    Colon |:|
//@[14:50)    ObjectSyntax
//@[14:15)     LeftBrace |{|
//@[15:17)     NewLine |\r\n|
    foo: 'a'
//@[4:12)     ObjectPropertySyntax
//@[4:7)      IdentifierSyntax
//@[4:7)       Identifier |foo|
//@[7:8)      Colon |:|
//@[9:12)      StringSyntax
//@[9:12)       StringComplete |'a'|
//@[12:14)     NewLine |\r\n|
    'foo': 'a'
//@[4:14)     ObjectPropertySyntax
//@[4:9)      StringSyntax
//@[4:9)       StringComplete |'foo'|
//@[9:10)      Colon |:|
//@[11:14)      StringSyntax
//@[11:14)       StringComplete |'a'|
//@[14:16)     NewLine |\r\n|
  }
//@[2:3)     RightBrace |}|
//@[3:5)   NewLine |\r\n|
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
//@[2:13)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:13)    StringSyntax
//@[8:13)     StringComplete |'foo'|
//@[13:15)   NewLine |\r\n|
  location: [
//@[2:18)   ObjectPropertySyntax
//@[2:10)    IdentifierSyntax
//@[2:10)     Identifier |location|
//@[10:11)    Colon |:|
//@[12:18)    ArraySyntax
//@[12:13)     LeftSquare |[|
//@[13:15)     NewLine |\r\n|
  ]
//@[2:3)     RightSquare |]|
//@[3:5)   NewLine |\r\n|
  tags: 'tag are not a string?'
//@[2:31)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |tags|
//@[6:7)    Colon |:|
//@[8:31)    StringSyntax
//@[8:31)     StringComplete |'tag are not a string?'|
//@[31:33)   NewLine |\r\n|
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
//@[2:28)   ObjectPropertySyntax
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
//@[27:28)      IntegerLiteralSyntax
//@[27:28)       Integer |1|
//@[28:30)   NewLine |\r\n|
  properties: {
//@[2:142)   ObjectPropertySyntax
//@[2:12)    IdentifierSyntax
//@[2:12)     Identifier |properties|
//@[12:13)    Colon |:|
//@[14:142)    ObjectSyntax
//@[14:15)     LeftBrace |{|
//@[15:17)     NewLine |\r\n|
    x: foo()
//@[4:12)     ObjectPropertySyntax
//@[4:5)      IdentifierSyntax
//@[4:5)       Identifier |x|
//@[5:6)      Colon |:|
//@[7:12)      FunctionCallSyntax
//@[7:10)       IdentifierSyntax
//@[7:10)        Identifier |foo|
//@[10:11)       LeftParen |(|
//@[11:12)       RightParen |)|
//@[12:14)     NewLine |\r\n|
    y: true && (null || !4)
//@[4:27)     ObjectPropertySyntax
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
//@[25:26)          IntegerLiteralSyntax
//@[25:26)           Integer |4|
//@[26:27)        RightParen |)|
//@[27:29)     NewLine |\r\n|
    a: [
//@[4:77)     ObjectPropertySyntax
//@[4:5)      IdentifierSyntax
//@[4:5)       Identifier |a|
//@[5:6)      Colon |:|
//@[7:77)      ArraySyntax
//@[7:8)       LeftSquare |[|
//@[8:10)       NewLine |\r\n|
      a
//@[6:7)       ArrayItemSyntax
//@[6:7)        VariableAccessSyntax
//@[6:7)         IdentifierSyntax
//@[6:7)          Identifier |a|
//@[7:9)       NewLine |\r\n|
      !null
//@[6:11)       ArrayItemSyntax
//@[6:11)        UnaryOperationSyntax
//@[6:7)         Exclamation |!|
//@[7:11)         NullLiteralSyntax
//@[7:11)          NullKeyword |null|
//@[11:13)       NewLine |\r\n|
      true && true || true + -true * 4
//@[6:38)       ArrayItemSyntax
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
//@[37:38)           IntegerLiteralSyntax
//@[37:38)            Integer |4|
//@[38:40)       NewLine |\r\n|
    ]
//@[4:5)       RightSquare |]|
//@[5:7)     NewLine |\r\n|
  }
//@[2:3)     RightBrace |}|
//@[3:5)   NewLine |\r\n|
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
//@[2:14)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:14)    StringSyntax
//@[8:14)     StringComplete |'test'|
//@[14:16)   NewLine |\r\n|
  id: 2
//@[2:7)   ObjectPropertySyntax
//@[2:4)    IdentifierSyntax
//@[2:4)     Identifier |id|
//@[4:5)    Colon |:|
//@[6:7)    IntegerLiteralSyntax
//@[6:7)     Integer |2|
//@[7:9)   NewLine |\r\n|
  type: 'hello'
//@[2:15)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |type|
//@[6:7)    Colon |:|
//@[8:15)    StringSyntax
//@[8:15)     StringComplete |'hello'|
//@[15:17)   NewLine |\r\n|
  apiVersion: true
//@[2:18)   ObjectPropertySyntax
//@[2:12)    IdentifierSyntax
//@[2:12)     Identifier |apiVersion|
//@[12:13)    Colon |:|
//@[14:18)    BooleanLiteralSyntax
//@[14:18)     TrueKeyword |true|
//@[18:20)   NewLine |\r\n|
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
//@[2:14)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:14)    StringSyntax
//@[8:14)     StringComplete |'test'|
//@[14:16)   NewLine |\r\n|
  dependsOn: [
//@[2:31)   ObjectPropertySyntax
//@[2:11)    IdentifierSyntax
//@[2:11)     Identifier |dependsOn|
//@[11:12)    Colon |:|
//@[13:31)    ArraySyntax
//@[13:14)     LeftSquare |[|
//@[14:16)     NewLine |\r\n|
    baz.id
//@[4:10)     ArrayItemSyntax
//@[4:10)      PropertyAccessSyntax
//@[4:7)       VariableAccessSyntax
//@[4:7)        IdentifierSyntax
//@[4:7)         Identifier |baz|
//@[7:8)       Dot |.|
//@[8:10)       IdentifierSyntax
//@[8:10)        Identifier |id|
//@[10:12)     NewLine |\r\n|
  ]
//@[2:3)     RightSquare |]|
//@[3:5)   NewLine |\r\n|
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
//@[2:14)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:14)    StringSyntax
//@[8:14)     StringComplete |'test'|
//@[14:16)   NewLine |\r\n|
  dependsOn: [
//@[2:42)   ObjectPropertySyntax
//@[2:11)    IdentifierSyntax
//@[2:11)     Identifier |dependsOn|
//@[11:12)    Colon |:|
//@[13:42)    ArraySyntax
//@[13:14)     LeftSquare |[|
//@[14:16)     NewLine |\r\n|
    'hello'
//@[4:11)     ArrayItemSyntax
//@[4:11)      StringSyntax
//@[4:11)       StringComplete |'hello'|
//@[11:13)     NewLine |\r\n|
    true
//@[4:8)     ArrayItemSyntax
//@[4:8)      BooleanLiteralSyntax
//@[4:8)       TrueKeyword |true|
//@[8:10)     NewLine |\r\n|
  ]
//@[2:3)     RightSquare |]|
//@[3:5)   NewLine |\r\n|
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
//@[2:14)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:14)    StringSyntax
//@[8:14)     StringComplete |'test'|
//@[14:16)   NewLine |\r\n|
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
//@[2:14)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:14)    StringSyntax
//@[8:14)     StringComplete |'test'|
//@[14:16)   NewLine |\r\n|
  dependsOn: [
//@[2:36)   ObjectPropertySyntax
//@[2:11)    IdentifierSyntax
//@[2:11)     Identifier |dependsOn|
//@[11:12)    Colon |:|
//@[13:36)    ArraySyntax
//@[13:14)     LeftSquare |[|
//@[14:16)     NewLine |\r\n|
    badDepends3
//@[4:15)     ArrayItemSyntax
//@[4:15)      VariableAccessSyntax
//@[4:15)       IdentifierSyntax
//@[4:15)        Identifier |badDepends3|
//@[15:17)     NewLine |\r\n|
  ]
//@[2:3)     RightSquare |]|
//@[3:5)   NewLine |\r\n|
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
//@[2:14)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:14)    StringSyntax
//@[8:14)     StringComplete |'test'|
//@[14:16)   NewLine |\r\n|
  dependsOn: badDepends3.dependsOn
//@[2:34)   ObjectPropertySyntax
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
//@[34:36)   NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

var interpVal = 'abc'
//@[0:21) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:13)  IdentifierSyntax
//@[4:13)   Identifier |interpVal|
//@[14:15)  Assignment |=|
//@[16:21)  StringSyntax
//@[16:21)   StringComplete |'abc'|
//@[21:23) NewLine |\r\n|
resource badInterp 'Microsoft.Foo/foos@2020-02-02-alpha' = {
//@[0:205) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:18)  IdentifierSyntax
//@[9:18)   Identifier |badInterp|
//@[19:56)  StringSyntax
//@[19:56)   StringComplete |'Microsoft.Foo/foos@2020-02-02-alpha'|
//@[57:58)  Assignment |=|
//@[59:205)  ObjectSyntax
//@[59:60)   LeftBrace |{|
//@[60:62)   NewLine |\r\n|
  name: 'test'
//@[2:14)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:14)    StringSyntax
//@[8:14)     StringComplete |'test'|
//@[14:16)   NewLine |\r\n|
  '${interpVal}': 'unsupported' // resource definition does not allow for additionalProperties
//@[2:31)   ObjectPropertySyntax
//@[2:16)    StringSyntax
//@[2:5)     StringLeftPiece |'${|
//@[5:14)     VariableAccessSyntax
//@[5:14)      IdentifierSyntax
//@[5:14)       Identifier |interpVal|
//@[14:16)     StringRightPiece |}'|
//@[16:17)    Colon |:|
//@[18:31)    StringSyntax
//@[18:31)     StringComplete |'unsupported'|
//@[94:96)   NewLine |\r\n|
  '${undefinedSymbol}': true
//@[2:28)   ObjectPropertySyntax
//@[2:22)    StringSyntax
//@[2:5)     StringLeftPiece |'${|
//@[5:20)     VariableAccessSyntax
//@[5:20)      IdentifierSyntax
//@[5:20)       Identifier |undefinedSymbol|
//@[20:22)     StringRightPiece |}'|
//@[22:23)    Colon |:|
//@[24:28)    BooleanLiteralSyntax
//@[24:28)     TrueKeyword |true|
//@[28:30)   NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

module validModule './module.bicep' = {
//@[0:106) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:18)  IdentifierSyntax
//@[7:18)   Identifier |validModule|
//@[19:35)  StringSyntax
//@[19:35)   StringComplete |'./module.bicep'|
//@[36:37)  Assignment |=|
//@[38:106)  ObjectSyntax
//@[38:39)   LeftBrace |{|
//@[39:41)   NewLine |\r\n|
  name: 'storageDeploy'
//@[2:23)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:23)    StringSyntax
//@[8:23)     StringComplete |'storageDeploy'|
//@[23:25)   NewLine |\r\n|
  params: {
//@[2:37)   ObjectPropertySyntax
//@[2:8)    IdentifierSyntax
//@[2:8)     Identifier |params|
//@[8:9)    Colon |:|
//@[10:37)    ObjectSyntax
//@[10:11)     LeftBrace |{|
//@[11:13)     NewLine |\r\n|
    name: 'contoso'
//@[4:19)     ObjectPropertySyntax
//@[4:8)      IdentifierSyntax
//@[4:8)       Identifier |name|
//@[8:9)      Colon |:|
//@[10:19)      StringSyntax
//@[10:19)       StringComplete |'contoso'|
//@[19:21)     NewLine |\r\n|
  }
//@[2:3)     RightBrace |}|
//@[3:5)   NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource runtimeValidRes1 'Microsoft.Compute/virtualMachines@2020-06-01' = {
//@[0:174) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:25)  IdentifierSyntax
//@[9:25)   Identifier |runtimeValidRes1|
//@[26:72)  StringSyntax
//@[26:72)   StringComplete |'Microsoft.Compute/virtualMachines@2020-06-01'|
//@[73:74)  Assignment |=|
//@[75:174)  ObjectSyntax
//@[75:76)   LeftBrace |{|
//@[76:78)   NewLine |\r\n|
  name: 'name1'
//@[2:15)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:15)    StringSyntax
//@[8:15)     StringComplete |'name1'|
//@[15:17)   NewLine |\r\n|
  location: 'eastus'
//@[2:20)   ObjectPropertySyntax
//@[2:10)    IdentifierSyntax
//@[2:10)     Identifier |location|
//@[10:11)    Colon |:|
//@[12:20)    StringSyntax
//@[12:20)     StringComplete |'eastus'|
//@[20:22)   NewLine |\r\n|
  properties: {
//@[2:54)   ObjectPropertySyntax
//@[2:12)    IdentifierSyntax
//@[2:12)     Identifier |properties|
//@[12:13)    Colon |:|
//@[14:54)    ObjectSyntax
//@[14:15)     LeftBrace |{|
//@[15:17)     NewLine |\r\n|
    evictionPolicy: 'Deallocate'
//@[4:32)     ObjectPropertySyntax
//@[4:18)      IdentifierSyntax
//@[4:18)       Identifier |evictionPolicy|
//@[18:19)      Colon |:|
//@[20:32)      StringSyntax
//@[20:32)       StringComplete |'Deallocate'|
//@[32:34)     NewLine |\r\n|
  }
//@[2:3)     RightBrace |}|
//@[3:5)   NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource runtimeValidRes2 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
//@[0:329) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:25)  IdentifierSyntax
//@[9:25)   Identifier |runtimeValidRes2|
//@[26:76)  StringSyntax
//@[26:76)   StringComplete |'Microsoft.Resources/deploymentScripts@2020-10-01'|
//@[77:78)  Assignment |=|
//@[79:329)  ObjectSyntax
//@[79:80)   LeftBrace |{|
//@[80:82)   NewLine |\r\n|
  name: concat(concat(runtimeValidRes1.id, runtimeValidRes1.name), runtimeValidRes1.type)
//@[2:89)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:89)    FunctionCallSyntax
//@[8:14)     IdentifierSyntax
//@[8:14)      Identifier |concat|
//@[14:15)     LeftParen |(|
//@[15:66)     FunctionArgumentSyntax
//@[15:65)      FunctionCallSyntax
//@[15:21)       IdentifierSyntax
//@[15:21)        Identifier |concat|
//@[21:22)       LeftParen |(|
//@[22:42)       FunctionArgumentSyntax
//@[22:41)        PropertyAccessSyntax
//@[22:38)         VariableAccessSyntax
//@[22:38)          IdentifierSyntax
//@[22:38)           Identifier |runtimeValidRes1|
//@[38:39)         Dot |.|
//@[39:41)         IdentifierSyntax
//@[39:41)          Identifier |id|
//@[41:42)        Comma |,|
//@[43:64)       FunctionArgumentSyntax
//@[43:64)        PropertyAccessSyntax
//@[43:59)         VariableAccessSyntax
//@[43:59)          IdentifierSyntax
//@[43:59)           Identifier |runtimeValidRes1|
//@[59:60)         Dot |.|
//@[60:64)         IdentifierSyntax
//@[60:64)          Identifier |name|
//@[64:65)       RightParen |)|
//@[65:66)      Comma |,|
//@[67:88)     FunctionArgumentSyntax
//@[67:88)      PropertyAccessSyntax
//@[67:83)       VariableAccessSyntax
//@[67:83)        IdentifierSyntax
//@[67:83)         Identifier |runtimeValidRes1|
//@[83:84)       Dot |.|
//@[84:88)       IdentifierSyntax
//@[84:88)        Identifier |type|
//@[88:89)     RightParen |)|
//@[89:91)   NewLine |\r\n|
  kind:'AzureCLI'
//@[2:17)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |kind|
//@[6:7)    Colon |:|
//@[7:17)    StringSyntax
//@[7:17)     StringComplete |'AzureCLI'|
//@[17:19)   NewLine |\r\n|
  location: 'eastus'
//@[2:20)   ObjectPropertySyntax
//@[2:10)    IdentifierSyntax
//@[2:10)     Identifier |location|
//@[10:11)    Colon |:|
//@[12:20)    StringSyntax
//@[12:20)     StringComplete |'eastus'|
//@[20:22)   NewLine |\r\n|
  properties: {
//@[2:112)   ObjectPropertySyntax
//@[2:12)    IdentifierSyntax
//@[2:12)     Identifier |properties|
//@[12:13)    Colon |:|
//@[14:112)    ObjectSyntax
//@[14:15)     LeftBrace |{|
//@[15:17)     NewLine |\r\n|
    azCliVersion: '2.0'
//@[4:23)     ObjectPropertySyntax
//@[4:16)      IdentifierSyntax
//@[4:16)       Identifier |azCliVersion|
//@[16:17)      Colon |:|
//@[18:23)      StringSyntax
//@[18:23)       StringComplete |'2.0'|
//@[23:25)     NewLine |\r\n|
    retentionInterval: runtimeValidRes1.properties.evictionPolicy
//@[4:65)     ObjectPropertySyntax
//@[4:21)      IdentifierSyntax
//@[4:21)       Identifier |retentionInterval|
//@[21:22)      Colon |:|
//@[23:65)      PropertyAccessSyntax
//@[23:50)       PropertyAccessSyntax
//@[23:39)        VariableAccessSyntax
//@[23:39)         IdentifierSyntax
//@[23:39)          Identifier |runtimeValidRes1|
//@[39:40)        Dot |.|
//@[40:50)        IdentifierSyntax
//@[40:50)         Identifier |properties|
//@[50:51)       Dot |.|
//@[51:65)       IdentifierSyntax
//@[51:65)        Identifier |evictionPolicy|
//@[65:67)     NewLine |\r\n|
  }
//@[2:3)     RightBrace |}|
//@[3:5)   NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource runtimeValidRes3 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[0:131) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:25)  IdentifierSyntax
//@[9:25)   Identifier |runtimeValidRes3|
//@[26:85)  StringSyntax
//@[26:85)   StringComplete |'Microsoft.Advisor/recommendations/suppressions@2020-01-01'|
//@[86:87)  Assignment |=|
//@[88:131)  ObjectSyntax
//@[88:89)   LeftBrace |{|
//@[89:91)   NewLine |\r\n|
  name: '${runtimeValidRes1.name}_v1'
//@[2:37)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:37)    StringSyntax
//@[8:11)     StringLeftPiece |'${|
//@[11:32)     PropertyAccessSyntax
//@[11:27)      VariableAccessSyntax
//@[11:27)       IdentifierSyntax
//@[11:27)        Identifier |runtimeValidRes1|
//@[27:28)      Dot |.|
//@[28:32)      IdentifierSyntax
//@[28:32)       Identifier |name|
//@[32:37)     StringRightPiece |}_v1'|
//@[37:39)   NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource runtimeValidRes4 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[0:135) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:25)  IdentifierSyntax
//@[9:25)   Identifier |runtimeValidRes4|
//@[26:85)  StringSyntax
//@[26:85)   StringComplete |'Microsoft.Advisor/recommendations/suppressions@2020-01-01'|
//@[86:87)  Assignment |=|
//@[88:135)  ObjectSyntax
//@[88:89)   LeftBrace |{|
//@[89:91)   NewLine |\r\n|
  name: concat(validModule['name'], 'v1')
//@[2:41)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:41)    FunctionCallSyntax
//@[8:14)     IdentifierSyntax
//@[8:14)      Identifier |concat|
//@[14:15)     LeftParen |(|
//@[15:35)     FunctionArgumentSyntax
//@[15:34)      ArrayAccessSyntax
//@[15:26)       VariableAccessSyntax
//@[15:26)        IdentifierSyntax
//@[15:26)         Identifier |validModule|
//@[26:27)       LeftSquare |[|
//@[27:33)       StringSyntax
//@[27:33)        StringComplete |'name'|
//@[33:34)       RightSquare |]|
//@[34:35)      Comma |,|
//@[36:40)     FunctionArgumentSyntax
//@[36:40)      StringSyntax
//@[36:40)       StringComplete |'v1'|
//@[40:41)     RightParen |)|
//@[41:43)   NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource runtimeValidRes5 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[0:126) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:25)  IdentifierSyntax
//@[9:25)   Identifier |runtimeValidRes5|
//@[26:85)  StringSyntax
//@[26:85)   StringComplete |'Microsoft.Advisor/recommendations/suppressions@2020-01-01'|
//@[86:87)  Assignment |=|
//@[88:126)  ObjectSyntax
//@[88:89)   LeftBrace |{|
//@[89:91)   NewLine |\r\n|
  name: '${validModule.name}_v1'
//@[2:32)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:32)    StringSyntax
//@[8:11)     StringLeftPiece |'${|
//@[11:27)     PropertyAccessSyntax
//@[11:22)      VariableAccessSyntax
//@[11:22)       IdentifierSyntax
//@[11:22)        Identifier |validModule|
//@[22:23)      Dot |.|
//@[23:27)      IdentifierSyntax
//@[23:27)       Identifier |name|
//@[27:32)     StringRightPiece |}_v1'|
//@[32:34)   NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource runtimeInvalidRes1 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[0:129) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:27)  IdentifierSyntax
//@[9:27)   Identifier |runtimeInvalidRes1|
//@[28:87)  StringSyntax
//@[28:87)   StringComplete |'Microsoft.Advisor/recommendations/suppressions@2020-01-01'|
//@[88:89)  Assignment |=|
//@[90:129)  ObjectSyntax
//@[90:91)   LeftBrace |{|
//@[91:93)   NewLine |\r\n|
  name: runtimeValidRes1.location
//@[2:33)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:33)    PropertyAccessSyntax
//@[8:24)     VariableAccessSyntax
//@[8:24)      IdentifierSyntax
//@[8:24)       Identifier |runtimeValidRes1|
//@[24:25)     Dot |.|
//@[25:33)     IdentifierSyntax
//@[25:33)      Identifier |location|
//@[33:35)   NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource runtimeInvalidRes2 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[0:132) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:27)  IdentifierSyntax
//@[9:27)   Identifier |runtimeInvalidRes2|
//@[28:87)  StringSyntax
//@[28:87)   StringComplete |'Microsoft.Advisor/recommendations/suppressions@2020-01-01'|
//@[88:89)  Assignment |=|
//@[90:132)  ObjectSyntax
//@[90:91)   LeftBrace |{|
//@[91:93)   NewLine |\r\n|
  name: runtimeValidRes1['location']
//@[2:36)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:36)    ArrayAccessSyntax
//@[8:24)     VariableAccessSyntax
//@[8:24)      IdentifierSyntax
//@[8:24)       Identifier |runtimeValidRes1|
//@[24:25)     LeftSquare |[|
//@[25:35)     StringSyntax
//@[25:35)      StringComplete |'location'|
//@[35:36)     RightSquare |]|
//@[36:38)   NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource runtimeInvalidRes3 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
//@[0:292) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:27)  IdentifierSyntax
//@[9:27)   Identifier |runtimeInvalidRes3|
//@[28:78)  StringSyntax
//@[28:78)   StringComplete |'Microsoft.Resources/deploymentScripts@2020-10-01'|
//@[79:80)  Assignment |=|
//@[81:292)  ObjectSyntax
//@[81:82)   LeftBrace |{|
//@[82:84)   NewLine |\r\n|
  name: runtimeValidRes1.properties.evictionPolicy
//@[2:50)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:50)    PropertyAccessSyntax
//@[8:35)     PropertyAccessSyntax
//@[8:24)      VariableAccessSyntax
//@[8:24)       IdentifierSyntax
//@[8:24)        Identifier |runtimeValidRes1|
//@[24:25)      Dot |.|
//@[25:35)      IdentifierSyntax
//@[25:35)       Identifier |properties|
//@[35:36)     Dot |.|
//@[36:50)     IdentifierSyntax
//@[36:50)      Identifier |evictionPolicy|
//@[50:52)   NewLine |\r\n|
  kind:'AzureCLI'
//@[2:17)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |kind|
//@[6:7)    Colon |:|
//@[7:17)    StringSyntax
//@[7:17)     StringComplete |'AzureCLI'|
//@[17:19)   NewLine |\r\n|
  location: 'eastus'
//@[2:20)   ObjectPropertySyntax
//@[2:10)    IdentifierSyntax
//@[2:10)     Identifier |location|
//@[10:11)    Colon |:|
//@[12:20)    StringSyntax
//@[12:20)     StringComplete |'eastus'|
//@[20:22)   NewLine |\r\n|
  properties: {
//@[2:112)   ObjectPropertySyntax
//@[2:12)    IdentifierSyntax
//@[2:12)     Identifier |properties|
//@[12:13)    Colon |:|
//@[14:112)    ObjectSyntax
//@[14:15)     LeftBrace |{|
//@[15:17)     NewLine |\r\n|
    azCliVersion: '2.0'
//@[4:23)     ObjectPropertySyntax
//@[4:16)      IdentifierSyntax
//@[4:16)       Identifier |azCliVersion|
//@[16:17)      Colon |:|
//@[18:23)      StringSyntax
//@[18:23)       StringComplete |'2.0'|
//@[23:25)     NewLine |\r\n|
    retentionInterval: runtimeValidRes1.properties.evictionPolicy
//@[4:65)     ObjectPropertySyntax
//@[4:21)      IdentifierSyntax
//@[4:21)       Identifier |retentionInterval|
//@[21:22)      Colon |:|
//@[23:65)      PropertyAccessSyntax
//@[23:50)       PropertyAccessSyntax
//@[23:39)        VariableAccessSyntax
//@[23:39)         IdentifierSyntax
//@[23:39)          Identifier |runtimeValidRes1|
//@[39:40)        Dot |.|
//@[40:50)        IdentifierSyntax
//@[40:50)         Identifier |properties|
//@[50:51)       Dot |.|
//@[51:65)       IdentifierSyntax
//@[51:65)        Identifier |evictionPolicy|
//@[65:67)     NewLine |\r\n|
  }
//@[2:3)     RightBrace |}|
//@[3:5)   NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource runtimeInvalidRes4 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[0:149) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:27)  IdentifierSyntax
//@[9:27)   Identifier |runtimeInvalidRes4|
//@[28:87)  StringSyntax
//@[28:87)   StringComplete |'Microsoft.Advisor/recommendations/suppressions@2020-01-01'|
//@[88:89)  Assignment |=|
//@[90:149)  ObjectSyntax
//@[90:91)   LeftBrace |{|
//@[91:93)   NewLine |\r\n|
  name: runtimeValidRes1['properties'].evictionPolicy
//@[2:53)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:53)    PropertyAccessSyntax
//@[8:38)     ArrayAccessSyntax
//@[8:24)      VariableAccessSyntax
//@[8:24)       IdentifierSyntax
//@[8:24)        Identifier |runtimeValidRes1|
//@[24:25)      LeftSquare |[|
//@[25:37)      StringSyntax
//@[25:37)       StringComplete |'properties'|
//@[37:38)      RightSquare |]|
//@[38:39)     Dot |.|
//@[39:53)     IdentifierSyntax
//@[39:53)      Identifier |evictionPolicy|
//@[53:55)   NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource runtimeInvalidRes5 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[0:152) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:27)  IdentifierSyntax
//@[9:27)   Identifier |runtimeInvalidRes5|
//@[28:87)  StringSyntax
//@[28:87)   StringComplete |'Microsoft.Advisor/recommendations/suppressions@2020-01-01'|
//@[88:89)  Assignment |=|
//@[90:152)  ObjectSyntax
//@[90:91)   LeftBrace |{|
//@[91:93)   NewLine |\r\n|
  name: runtimeValidRes1['properties']['evictionPolicy']
//@[2:56)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:56)    ArrayAccessSyntax
//@[8:38)     ArrayAccessSyntax
//@[8:24)      VariableAccessSyntax
//@[8:24)       IdentifierSyntax
//@[8:24)        Identifier |runtimeValidRes1|
//@[24:25)      LeftSquare |[|
//@[25:37)      StringSyntax
//@[25:37)       StringComplete |'properties'|
//@[37:38)      RightSquare |]|
//@[38:39)     LeftSquare |[|
//@[39:55)     StringSyntax
//@[39:55)      StringComplete |'evictionPolicy'|
//@[55:56)     RightSquare |]|
//@[56:58)   NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource runtimeInvalidRes6 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[0:149) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:27)  IdentifierSyntax
//@[9:27)   Identifier |runtimeInvalidRes6|
//@[28:87)  StringSyntax
//@[28:87)   StringComplete |'Microsoft.Advisor/recommendations/suppressions@2020-01-01'|
//@[88:89)  Assignment |=|
//@[90:149)  ObjectSyntax
//@[90:91)   LeftBrace |{|
//@[91:93)   NewLine |\r\n|
  name: runtimeValidRes1.properties['evictionPolicy']
//@[2:53)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:53)    ArrayAccessSyntax
//@[8:35)     PropertyAccessSyntax
//@[8:24)      VariableAccessSyntax
//@[8:24)       IdentifierSyntax
//@[8:24)        Identifier |runtimeValidRes1|
//@[24:25)      Dot |.|
//@[25:35)      IdentifierSyntax
//@[25:35)       Identifier |properties|
//@[35:36)     LeftSquare |[|
//@[36:52)     StringSyntax
//@[36:52)      StringComplete |'evictionPolicy'|
//@[52:53)     RightSquare |]|
//@[53:55)   NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource runtimeInvalidRes7 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[0:144) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:27)  IdentifierSyntax
//@[9:27)   Identifier |runtimeInvalidRes7|
//@[28:87)  StringSyntax
//@[28:87)   StringComplete |'Microsoft.Advisor/recommendations/suppressions@2020-01-01'|
//@[88:89)  Assignment |=|
//@[90:144)  ObjectSyntax
//@[90:91)   LeftBrace |{|
//@[91:93)   NewLine |\r\n|
  name: runtimeValidRes2.properties.azCliVersion
//@[2:48)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:48)    PropertyAccessSyntax
//@[8:35)     PropertyAccessSyntax
//@[8:24)      VariableAccessSyntax
//@[8:24)       IdentifierSyntax
//@[8:24)        Identifier |runtimeValidRes2|
//@[24:25)      Dot |.|
//@[25:35)      IdentifierSyntax
//@[25:35)       Identifier |properties|
//@[35:36)     Dot |.|
//@[36:48)     IdentifierSyntax
//@[36:48)      Identifier |azCliVersion|
//@[48:50)   NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

var magicString1 = 'location'
//@[0:29) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:16)  IdentifierSyntax
//@[4:16)   Identifier |magicString1|
//@[17:18)  Assignment |=|
//@[19:29)  StringSyntax
//@[19:29)   StringComplete |'location'|
//@[29:31) NewLine |\r\n|
resource runtimeInvalidRes8 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[0:139) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:27)  IdentifierSyntax
//@[9:27)   Identifier |runtimeInvalidRes8|
//@[28:87)  StringSyntax
//@[28:87)   StringComplete |'Microsoft.Advisor/recommendations/suppressions@2020-01-01'|
//@[88:89)  Assignment |=|
//@[90:139)  ObjectSyntax
//@[90:91)   LeftBrace |{|
//@[91:93)   NewLine |\r\n|
  name: runtimeValidRes2['${magicString1}']
//@[2:43)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:43)    ArrayAccessSyntax
//@[8:24)     VariableAccessSyntax
//@[8:24)      IdentifierSyntax
//@[8:24)       Identifier |runtimeValidRes2|
//@[24:25)     LeftSquare |[|
//@[25:42)     StringSyntax
//@[25:28)      StringLeftPiece |'${|
//@[28:40)      VariableAccessSyntax
//@[28:40)       IdentifierSyntax
//@[28:40)        Identifier |magicString1|
//@[40:42)      StringRightPiece |}'|
//@[42:43)     RightSquare |]|
//@[43:45)   NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

// note: this should be fine, but we block string interpolation all together if there's a potential runtime property usage for name.
//@[132:134) NewLine |\r\n|
var magicString2 = 'name'
//@[0:25) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:16)  IdentifierSyntax
//@[4:16)   Identifier |magicString2|
//@[17:18)  Assignment |=|
//@[19:25)  StringSyntax
//@[19:25)   StringComplete |'name'|
//@[25:27) NewLine |\r\n|
resource runtimeInvalidRes9 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[0:139) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:27)  IdentifierSyntax
//@[9:27)   Identifier |runtimeInvalidRes9|
//@[28:87)  StringSyntax
//@[28:87)   StringComplete |'Microsoft.Advisor/recommendations/suppressions@2020-01-01'|
//@[88:89)  Assignment |=|
//@[90:139)  ObjectSyntax
//@[90:91)   LeftBrace |{|
//@[91:93)   NewLine |\r\n|
  name: runtimeValidRes2['${magicString2}']
//@[2:43)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:43)    ArrayAccessSyntax
//@[8:24)     VariableAccessSyntax
//@[8:24)      IdentifierSyntax
//@[8:24)       Identifier |runtimeValidRes2|
//@[24:25)     LeftSquare |[|
//@[25:42)     StringSyntax
//@[25:28)      StringLeftPiece |'${|
//@[28:40)      VariableAccessSyntax
//@[28:40)       IdentifierSyntax
//@[28:40)        Identifier |magicString2|
//@[40:42)      StringRightPiece |}'|
//@[42:43)     RightSquare |]|
//@[43:45)   NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource runtimeInvalidRes10 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[0:135) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:28)  IdentifierSyntax
//@[9:28)   Identifier |runtimeInvalidRes10|
//@[29:88)  StringSyntax
//@[29:88)   StringComplete |'Microsoft.Advisor/recommendations/suppressions@2020-01-01'|
//@[89:90)  Assignment |=|
//@[91:135)  ObjectSyntax
//@[91:92)   LeftBrace |{|
//@[92:94)   NewLine |\r\n|
  name: '${runtimeValidRes3.location}'
//@[2:38)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:38)    StringSyntax
//@[8:11)     StringLeftPiece |'${|
//@[11:36)     PropertyAccessSyntax
//@[11:27)      VariableAccessSyntax
//@[11:27)       IdentifierSyntax
//@[11:27)        Identifier |runtimeValidRes3|
//@[27:28)      Dot |.|
//@[28:36)      IdentifierSyntax
//@[28:36)       Identifier |location|
//@[36:38)     StringRightPiece |}'|
//@[38:40)   NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource runtimeInvalidRes11 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[0:131) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:28)  IdentifierSyntax
//@[9:28)   Identifier |runtimeInvalidRes11|
//@[29:88)  StringSyntax
//@[29:88)   StringComplete |'Microsoft.Advisor/recommendations/suppressions@2020-01-01'|
//@[89:90)  Assignment |=|
//@[91:131)  ObjectSyntax
//@[91:92)   LeftBrace |{|
//@[92:94)   NewLine |\r\n|
  name: validModule.params['name']
//@[2:34)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:34)    ArrayAccessSyntax
//@[8:26)     PropertyAccessSyntax
//@[8:19)      VariableAccessSyntax
//@[8:19)       IdentifierSyntax
//@[8:19)        Identifier |validModule|
//@[19:20)      Dot |.|
//@[20:26)      IdentifierSyntax
//@[20:26)       Identifier |params|
//@[26:27)     LeftSquare |[|
//@[27:33)     StringSyntax
//@[27:33)      StringComplete |'name'|
//@[33:34)     RightSquare |]|
//@[34:36)   NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource runtimeInvalidRes12 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[0:240) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:28)  IdentifierSyntax
//@[9:28)   Identifier |runtimeInvalidRes12|
//@[29:88)  StringSyntax
//@[29:88)   StringComplete |'Microsoft.Advisor/recommendations/suppressions@2020-01-01'|
//@[89:90)  Assignment |=|
//@[91:240)  ObjectSyntax
//@[91:92)   LeftBrace |{|
//@[92:94)   NewLine |\r\n|
  name: concat(runtimeValidRes1.location, runtimeValidRes2['location'], runtimeInvalidRes3['properties'].azCliVersion, validModule.params.name)
//@[2:143)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:143)    FunctionCallSyntax
//@[8:14)     IdentifierSyntax
//@[8:14)      Identifier |concat|
//@[14:15)     LeftParen |(|
//@[15:41)     FunctionArgumentSyntax
//@[15:40)      PropertyAccessSyntax
//@[15:31)       VariableAccessSyntax
//@[15:31)        IdentifierSyntax
//@[15:31)         Identifier |runtimeValidRes1|
//@[31:32)       Dot |.|
//@[32:40)       IdentifierSyntax
//@[32:40)        Identifier |location|
//@[40:41)      Comma |,|
//@[42:71)     FunctionArgumentSyntax
//@[42:70)      ArrayAccessSyntax
//@[42:58)       VariableAccessSyntax
//@[42:58)        IdentifierSyntax
//@[42:58)         Identifier |runtimeValidRes2|
//@[58:59)       LeftSquare |[|
//@[59:69)       StringSyntax
//@[59:69)        StringComplete |'location'|
//@[69:70)       RightSquare |]|
//@[70:71)      Comma |,|
//@[72:118)     FunctionArgumentSyntax
//@[72:117)      PropertyAccessSyntax
//@[72:104)       ArrayAccessSyntax
//@[72:90)        VariableAccessSyntax
//@[72:90)         IdentifierSyntax
//@[72:90)          Identifier |runtimeInvalidRes3|
//@[90:91)        LeftSquare |[|
//@[91:103)        StringSyntax
//@[91:103)         StringComplete |'properties'|
//@[103:104)        RightSquare |]|
//@[104:105)       Dot |.|
//@[105:117)       IdentifierSyntax
//@[105:117)        Identifier |azCliVersion|
//@[117:118)      Comma |,|
//@[119:142)     FunctionArgumentSyntax
//@[119:142)      PropertyAccessSyntax
//@[119:137)       PropertyAccessSyntax
//@[119:130)        VariableAccessSyntax
//@[119:130)         IdentifierSyntax
//@[119:130)          Identifier |validModule|
//@[130:131)        Dot |.|
//@[131:137)        IdentifierSyntax
//@[131:137)         Identifier |params|
//@[137:138)       Dot |.|
//@[138:142)       IdentifierSyntax
//@[138:142)        Identifier |name|
//@[142:143)     RightParen |)|
//@[143:145)   NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource runtimeInvalidRes13 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[0:243) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:28)  IdentifierSyntax
//@[9:28)   Identifier |runtimeInvalidRes13|
//@[29:88)  StringSyntax
//@[29:88)   StringComplete |'Microsoft.Advisor/recommendations/suppressions@2020-01-01'|
//@[89:90)  Assignment |=|
//@[91:243)  ObjectSyntax
//@[91:92)   LeftBrace |{|
//@[92:94)   NewLine |\r\n|
  name: '${runtimeValidRes1.location}${runtimeValidRes2['location']}${runtimeInvalidRes3.properties['azCliVersion']}${validModule['params'].name}'
//@[2:146)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:146)    StringSyntax
//@[8:11)     StringLeftPiece |'${|
//@[11:36)     PropertyAccessSyntax
//@[11:27)      VariableAccessSyntax
//@[11:27)       IdentifierSyntax
//@[11:27)        Identifier |runtimeValidRes1|
//@[27:28)      Dot |.|
//@[28:36)      IdentifierSyntax
//@[28:36)       Identifier |location|
//@[36:39)     StringMiddlePiece |}${|
//@[39:67)     ArrayAccessSyntax
//@[39:55)      VariableAccessSyntax
//@[39:55)       IdentifierSyntax
//@[39:55)        Identifier |runtimeValidRes2|
//@[55:56)      LeftSquare |[|
//@[56:66)      StringSyntax
//@[56:66)       StringComplete |'location'|
//@[66:67)      RightSquare |]|
//@[67:70)     StringMiddlePiece |}${|
//@[70:115)     ArrayAccessSyntax
//@[70:99)      PropertyAccessSyntax
//@[70:88)       VariableAccessSyntax
//@[70:88)        IdentifierSyntax
//@[70:88)         Identifier |runtimeInvalidRes3|
//@[88:89)       Dot |.|
//@[89:99)       IdentifierSyntax
//@[89:99)        Identifier |properties|
//@[99:100)      LeftSquare |[|
//@[100:114)      StringSyntax
//@[100:114)       StringComplete |'azCliVersion'|
//@[114:115)      RightSquare |]|
//@[115:118)     StringMiddlePiece |}${|
//@[118:144)     PropertyAccessSyntax
//@[118:139)      ArrayAccessSyntax
//@[118:129)       VariableAccessSyntax
//@[118:129)        IdentifierSyntax
//@[118:129)         Identifier |validModule|
//@[129:130)       LeftSquare |[|
//@[130:138)       StringSyntax
//@[130:138)        StringComplete |'params'|
//@[138:139)       RightSquare |]|
//@[139:140)      Dot |.|
//@[140:144)      IdentifierSyntax
//@[140:144)       Identifier |name|
//@[144:146)     StringRightPiece |}'|
//@[146:148)   NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

// variable related runtime validation
//@[38:40) NewLine |\r\n|
var runtimefoo1 = runtimeValidRes1['location']
//@[0:46) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:15)  IdentifierSyntax
//@[4:15)   Identifier |runtimefoo1|
//@[16:17)  Assignment |=|
//@[18:46)  ArrayAccessSyntax
//@[18:34)   VariableAccessSyntax
//@[18:34)    IdentifierSyntax
//@[18:34)     Identifier |runtimeValidRes1|
//@[34:35)   LeftSquare |[|
//@[35:45)   StringSyntax
//@[35:45)    StringComplete |'location'|
//@[45:46)   RightSquare |]|
//@[46:48) NewLine |\r\n|
var runtimefoo2 = runtimeValidRes2['properties'].azCliVersion
//@[0:61) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:15)  IdentifierSyntax
//@[4:15)   Identifier |runtimefoo2|
//@[16:17)  Assignment |=|
//@[18:61)  PropertyAccessSyntax
//@[18:48)   ArrayAccessSyntax
//@[18:34)    VariableAccessSyntax
//@[18:34)     IdentifierSyntax
//@[18:34)      Identifier |runtimeValidRes2|
//@[34:35)    LeftSquare |[|
//@[35:47)    StringSyntax
//@[35:47)     StringComplete |'properties'|
//@[47:48)    RightSquare |]|
//@[48:49)   Dot |.|
//@[49:61)   IdentifierSyntax
//@[49:61)    Identifier |azCliVersion|
//@[61:63) NewLine |\r\n|
var runtimefoo3 = runtimeValidRes2
//@[0:34) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:15)  IdentifierSyntax
//@[4:15)   Identifier |runtimefoo3|
//@[16:17)  Assignment |=|
//@[18:34)  VariableAccessSyntax
//@[18:34)   IdentifierSyntax
//@[18:34)    Identifier |runtimeValidRes2|
//@[34:36) NewLine |\r\n|
var runtimefoo4 = {
//@[0:42) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:15)  IdentifierSyntax
//@[4:15)   Identifier |runtimefoo4|
//@[16:17)  Assignment |=|
//@[18:42)  ObjectSyntax
//@[18:19)   LeftBrace |{|
//@[19:21)   NewLine |\r\n|
  hop: runtimefoo2
//@[2:18)   ObjectPropertySyntax
//@[2:5)    IdentifierSyntax
//@[2:5)     Identifier |hop|
//@[5:6)    Colon |:|
//@[7:18)    VariableAccessSyntax
//@[7:18)     IdentifierSyntax
//@[7:18)      Identifier |runtimefoo2|
//@[18:20)   NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

var runtimeInvalid = {
//@[0:119) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:18)  IdentifierSyntax
//@[4:18)   Identifier |runtimeInvalid|
//@[19:20)  Assignment |=|
//@[21:119)  ObjectSyntax
//@[21:22)   LeftBrace |{|
//@[22:24)   NewLine |\r\n|
  foo1: runtimefoo1
//@[2:19)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |foo1|
//@[6:7)    Colon |:|
//@[8:19)    VariableAccessSyntax
//@[8:19)     IdentifierSyntax
//@[8:19)      Identifier |runtimefoo1|
//@[19:21)   NewLine |\r\n|
  foo2: runtimefoo2
//@[2:19)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |foo2|
//@[6:7)    Colon |:|
//@[8:19)    VariableAccessSyntax
//@[8:19)     IdentifierSyntax
//@[8:19)      Identifier |runtimefoo2|
//@[19:21)   NewLine |\r\n|
  foo3: runtimefoo3
//@[2:19)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |foo3|
//@[6:7)    Colon |:|
//@[8:19)    VariableAccessSyntax
//@[8:19)     IdentifierSyntax
//@[8:19)      Identifier |runtimefoo3|
//@[19:21)   NewLine |\r\n|
  foo4: runtimeValidRes1.name
//@[2:29)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |foo4|
//@[6:7)    Colon |:|
//@[8:29)    PropertyAccessSyntax
//@[8:24)     VariableAccessSyntax
//@[8:24)      IdentifierSyntax
//@[8:24)       Identifier |runtimeValidRes1|
//@[24:25)     Dot |.|
//@[25:29)     IdentifierSyntax
//@[25:29)      Identifier |name|
//@[29:31)   NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

var runtimeValid = {
//@[0:151) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:16)  IdentifierSyntax
//@[4:16)   Identifier |runtimeValid|
//@[17:18)  Assignment |=|
//@[19:151)  ObjectSyntax
//@[19:20)   LeftBrace |{|
//@[20:22)   NewLine |\r\n|
  foo1: runtimeValidRes1.name
//@[2:29)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |foo1|
//@[6:7)    Colon |:|
//@[8:29)    PropertyAccessSyntax
//@[8:24)     VariableAccessSyntax
//@[8:24)      IdentifierSyntax
//@[8:24)       Identifier |runtimeValidRes1|
//@[24:25)     Dot |.|
//@[25:29)     IdentifierSyntax
//@[25:29)      Identifier |name|
//@[29:31)   NewLine |\r\n|
  foo2: runtimeValidRes1.apiVersion
//@[2:35)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |foo2|
//@[6:7)    Colon |:|
//@[8:35)    PropertyAccessSyntax
//@[8:24)     VariableAccessSyntax
//@[8:24)      IdentifierSyntax
//@[8:24)       Identifier |runtimeValidRes1|
//@[24:25)     Dot |.|
//@[25:35)     IdentifierSyntax
//@[25:35)      Identifier |apiVersion|
//@[35:37)   NewLine |\r\n|
  foo3: runtimeValidRes2.type
//@[2:29)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |foo3|
//@[6:7)    Colon |:|
//@[8:29)    PropertyAccessSyntax
//@[8:24)     VariableAccessSyntax
//@[8:24)      IdentifierSyntax
//@[8:24)       Identifier |runtimeValidRes2|
//@[24:25)     Dot |.|
//@[25:29)     IdentifierSyntax
//@[25:29)      Identifier |type|
//@[29:31)   NewLine |\r\n|
  foo4: runtimeValidRes2.id
//@[2:27)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |foo4|
//@[6:7)    Colon |:|
//@[8:27)    PropertyAccessSyntax
//@[8:24)     VariableAccessSyntax
//@[8:24)      IdentifierSyntax
//@[8:24)       Identifier |runtimeValidRes2|
//@[24:25)     Dot |.|
//@[25:27)     IdentifierSyntax
//@[25:27)      Identifier |id|
//@[27:29)   NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource runtimeInvalidRes14 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[0:124) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:28)  IdentifierSyntax
//@[9:28)   Identifier |runtimeInvalidRes14|
//@[29:88)  StringSyntax
//@[29:88)   StringComplete |'Microsoft.Advisor/recommendations/suppressions@2020-01-01'|
//@[89:90)  Assignment |=|
//@[91:124)  ObjectSyntax
//@[91:92)   LeftBrace |{|
//@[92:94)   NewLine |\r\n|
  name: runtimeInvalid.foo1
//@[2:27)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:27)    PropertyAccessSyntax
//@[8:22)     VariableAccessSyntax
//@[8:22)      IdentifierSyntax
//@[8:22)       Identifier |runtimeInvalid|
//@[22:23)     Dot |.|
//@[23:27)     IdentifierSyntax
//@[23:27)      Identifier |foo1|
//@[27:29)   NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource runtimeInvalidRes15 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[0:124) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:28)  IdentifierSyntax
//@[9:28)   Identifier |runtimeInvalidRes15|
//@[29:88)  StringSyntax
//@[29:88)   StringComplete |'Microsoft.Advisor/recommendations/suppressions@2020-01-01'|
//@[89:90)  Assignment |=|
//@[91:124)  ObjectSyntax
//@[91:92)   LeftBrace |{|
//@[92:94)   NewLine |\r\n|
  name: runtimeInvalid.foo2
//@[2:27)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:27)    PropertyAccessSyntax
//@[8:22)     VariableAccessSyntax
//@[8:22)      IdentifierSyntax
//@[8:22)       Identifier |runtimeInvalid|
//@[22:23)     Dot |.|
//@[23:27)     IdentifierSyntax
//@[23:27)      Identifier |foo2|
//@[27:29)   NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource runtimeInvalidRes16 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[0:148) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:28)  IdentifierSyntax
//@[9:28)   Identifier |runtimeInvalidRes16|
//@[29:88)  StringSyntax
//@[29:88)   StringComplete |'Microsoft.Advisor/recommendations/suppressions@2020-01-01'|
//@[89:90)  Assignment |=|
//@[91:148)  ObjectSyntax
//@[91:92)   LeftBrace |{|
//@[92:94)   NewLine |\r\n|
  name: runtimeInvalid.foo3.properties.azCliVersion
//@[2:51)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:51)    PropertyAccessSyntax
//@[8:38)     PropertyAccessSyntax
//@[8:27)      PropertyAccessSyntax
//@[8:22)       VariableAccessSyntax
//@[8:22)        IdentifierSyntax
//@[8:22)         Identifier |runtimeInvalid|
//@[22:23)       Dot |.|
//@[23:27)       IdentifierSyntax
//@[23:27)        Identifier |foo3|
//@[27:28)      Dot |.|
//@[28:38)      IdentifierSyntax
//@[28:38)       Identifier |properties|
//@[38:39)     Dot |.|
//@[39:51)     IdentifierSyntax
//@[39:51)      Identifier |azCliVersion|
//@[51:53)   NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

// Note: This is actually a runtime valid value. However, other properties of the variable cannot be resolved, so we block this.
//@[128:130) NewLine |\r\n|
resource runtimeInvalidRes17 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[0:124) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:28)  IdentifierSyntax
//@[9:28)   Identifier |runtimeInvalidRes17|
//@[29:88)  StringSyntax
//@[29:88)   StringComplete |'Microsoft.Advisor/recommendations/suppressions@2020-01-01'|
//@[89:90)  Assignment |=|
//@[91:124)  ObjectSyntax
//@[91:92)   LeftBrace |{|
//@[92:94)   NewLine |\r\n|
  name: runtimeInvalid.foo4
//@[2:27)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:27)    PropertyAccessSyntax
//@[8:22)     VariableAccessSyntax
//@[8:22)      IdentifierSyntax
//@[8:22)       Identifier |runtimeInvalid|
//@[22:23)     Dot |.|
//@[23:27)     IdentifierSyntax
//@[23:27)      Identifier |foo4|
//@[27:29)   NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource runtimeInvalidRes18 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[0:226) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:28)  IdentifierSyntax
//@[9:28)   Identifier |runtimeInvalidRes18|
//@[29:88)  StringSyntax
//@[29:88)   StringComplete |'Microsoft.Advisor/recommendations/suppressions@2020-01-01'|
//@[89:90)  Assignment |=|
//@[91:226)  ObjectSyntax
//@[91:92)   LeftBrace |{|
//@[92:94)   NewLine |\r\n|
  name: concat(runtimeInvalid.foo1, runtimeValidRes2['properties'].azCliVersion, '${runtimeValidRes1.location}', runtimefoo4.hop)
//@[2:129)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:129)    FunctionCallSyntax
//@[8:14)     IdentifierSyntax
//@[8:14)      Identifier |concat|
//@[14:15)     LeftParen |(|
//@[15:35)     FunctionArgumentSyntax
//@[15:34)      PropertyAccessSyntax
//@[15:29)       VariableAccessSyntax
//@[15:29)        IdentifierSyntax
//@[15:29)         Identifier |runtimeInvalid|
//@[29:30)       Dot |.|
//@[30:34)       IdentifierSyntax
//@[30:34)        Identifier |foo1|
//@[34:35)      Comma |,|
//@[36:80)     FunctionArgumentSyntax
//@[36:79)      PropertyAccessSyntax
//@[36:66)       ArrayAccessSyntax
//@[36:52)        VariableAccessSyntax
//@[36:52)         IdentifierSyntax
//@[36:52)          Identifier |runtimeValidRes2|
//@[52:53)        LeftSquare |[|
//@[53:65)        StringSyntax
//@[53:65)         StringComplete |'properties'|
//@[65:66)        RightSquare |]|
//@[66:67)       Dot |.|
//@[67:79)       IdentifierSyntax
//@[67:79)        Identifier |azCliVersion|
//@[79:80)      Comma |,|
//@[81:112)     FunctionArgumentSyntax
//@[81:111)      StringSyntax
//@[81:84)       StringLeftPiece |'${|
//@[84:109)       PropertyAccessSyntax
//@[84:100)        VariableAccessSyntax
//@[84:100)         IdentifierSyntax
//@[84:100)          Identifier |runtimeValidRes1|
//@[100:101)        Dot |.|
//@[101:109)        IdentifierSyntax
//@[101:109)         Identifier |location|
//@[109:111)       StringRightPiece |}'|
//@[111:112)      Comma |,|
//@[113:128)     FunctionArgumentSyntax
//@[113:128)      PropertyAccessSyntax
//@[113:124)       VariableAccessSyntax
//@[113:124)        IdentifierSyntax
//@[113:124)         Identifier |runtimefoo4|
//@[124:125)       Dot |.|
//@[125:128)       IdentifierSyntax
//@[125:128)        Identifier |hop|
//@[128:129)     RightParen |)|
//@[129:131)   NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource runtimeValidRes6 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[0:119) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:25)  IdentifierSyntax
//@[9:25)   Identifier |runtimeValidRes6|
//@[26:85)  StringSyntax
//@[26:85)   StringComplete |'Microsoft.Advisor/recommendations/suppressions@2020-01-01'|
//@[86:87)  Assignment |=|
//@[88:119)  ObjectSyntax
//@[88:89)   LeftBrace |{|
//@[89:91)   NewLine |\r\n|
  name: runtimeValid.foo1
//@[2:25)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:25)    PropertyAccessSyntax
//@[8:20)     VariableAccessSyntax
//@[8:20)      IdentifierSyntax
//@[8:20)       Identifier |runtimeValid|
//@[20:21)     Dot |.|
//@[21:25)     IdentifierSyntax
//@[21:25)      Identifier |foo1|
//@[25:27)   NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource runtimeValidRes7 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[0:119) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:25)  IdentifierSyntax
//@[9:25)   Identifier |runtimeValidRes7|
//@[26:85)  StringSyntax
//@[26:85)   StringComplete |'Microsoft.Advisor/recommendations/suppressions@2020-01-01'|
//@[86:87)  Assignment |=|
//@[88:119)  ObjectSyntax
//@[88:89)   LeftBrace |{|
//@[89:91)   NewLine |\r\n|
  name: runtimeValid.foo2
//@[2:25)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:25)    PropertyAccessSyntax
//@[8:20)     VariableAccessSyntax
//@[8:20)      IdentifierSyntax
//@[8:20)       Identifier |runtimeValid|
//@[20:21)     Dot |.|
//@[21:25)     IdentifierSyntax
//@[21:25)      Identifier |foo2|
//@[25:27)   NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource runtimeValidRes8 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[0:119) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:25)  IdentifierSyntax
//@[9:25)   Identifier |runtimeValidRes8|
//@[26:85)  StringSyntax
//@[26:85)   StringComplete |'Microsoft.Advisor/recommendations/suppressions@2020-01-01'|
//@[86:87)  Assignment |=|
//@[88:119)  ObjectSyntax
//@[88:89)   LeftBrace |{|
//@[89:91)   NewLine |\r\n|
  name: runtimeValid.foo3
//@[2:25)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:25)    PropertyAccessSyntax
//@[8:20)     VariableAccessSyntax
//@[8:20)      IdentifierSyntax
//@[8:20)       Identifier |runtimeValid|
//@[20:21)     Dot |.|
//@[21:25)     IdentifierSyntax
//@[21:25)      Identifier |foo3|
//@[25:27)   NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource runtimeValidRes9 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[0:119) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:25)  IdentifierSyntax
//@[9:25)   Identifier |runtimeValidRes9|
//@[26:85)  StringSyntax
//@[26:85)   StringComplete |'Microsoft.Advisor/recommendations/suppressions@2020-01-01'|
//@[86:87)  Assignment |=|
//@[88:119)  ObjectSyntax
//@[88:89)   LeftBrace |{|
//@[89:91)   NewLine |\r\n|
  name: runtimeValid.foo4
//@[2:25)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:25)    PropertyAccessSyntax
//@[8:20)     VariableAccessSyntax
//@[8:20)      IdentifierSyntax
//@[8:20)       Identifier |runtimeValid|
//@[20:21)     Dot |.|
//@[21:25)     IdentifierSyntax
//@[21:25)      Identifier |foo4|
//@[25:27)   NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:7) NewLine |\r\n\r\n\r\n|


resource loopForRuntimeCheck 'Microsoft.Network/dnsZones@2018-05-01' = [for thing in []: {
//@[0:130) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:28)  IdentifierSyntax
//@[9:28)   Identifier |loopForRuntimeCheck|
//@[29:68)  StringSyntax
//@[29:68)   StringComplete |'Microsoft.Network/dnsZones@2018-05-01'|
//@[69:70)  Assignment |=|
//@[71:130)  ForSyntax
//@[71:72)   LeftSquare |[|
//@[72:75)   Identifier |for|
//@[76:81)   LocalVariableSyntax
//@[76:81)    IdentifierSyntax
//@[76:81)     Identifier |thing|
//@[82:84)   Identifier |in|
//@[85:87)   ArraySyntax
//@[85:86)    LeftSquare |[|
//@[86:87)    RightSquare |]|
//@[87:88)   Colon |:|
//@[89:129)   ObjectSyntax
//@[89:90)    LeftBrace |{|
//@[90:92)    NewLine |\r\n|
  name: 'test'
//@[2:14)    ObjectPropertySyntax
//@[2:6)     IdentifierSyntax
//@[2:6)      Identifier |name|
//@[6:7)     Colon |:|
//@[8:14)     StringSyntax
//@[8:14)      StringComplete |'test'|
//@[14:16)    NewLine |\r\n|
  location: 'test'
//@[2:18)    ObjectPropertySyntax
//@[2:10)     IdentifierSyntax
//@[2:10)      Identifier |location|
//@[10:11)     Colon |:|
//@[12:18)     StringSyntax
//@[12:18)      StringComplete |'test'|
//@[18:20)    NewLine |\r\n|
}]
//@[0:1)    RightBrace |}|
//@[1:2)   RightSquare |]|
//@[2:6) NewLine |\r\n\r\n|

var runtimeCheckVar = loopForRuntimeCheck[0].properties.zoneType
//@[0:64) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:19)  IdentifierSyntax
//@[4:19)   Identifier |runtimeCheckVar|
//@[20:21)  Assignment |=|
//@[22:64)  PropertyAccessSyntax
//@[22:55)   PropertyAccessSyntax
//@[22:44)    ArrayAccessSyntax
//@[22:41)     VariableAccessSyntax
//@[22:41)      IdentifierSyntax
//@[22:41)       Identifier |loopForRuntimeCheck|
//@[41:42)     LeftSquare |[|
//@[42:43)     IntegerLiteralSyntax
//@[42:43)      Integer |0|
//@[43:44)     RightSquare |]|
//@[44:45)    Dot |.|
//@[45:55)    IdentifierSyntax
//@[45:55)     Identifier |properties|
//@[55:56)   Dot |.|
//@[56:64)   IdentifierSyntax
//@[56:64)    Identifier |zoneType|
//@[64:66) NewLine |\r\n|
var runtimeCheckVar2 = runtimeCheckVar
//@[0:38) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:20)  IdentifierSyntax
//@[4:20)   Identifier |runtimeCheckVar2|
//@[21:22)  Assignment |=|
//@[23:38)  VariableAccessSyntax
//@[23:38)   IdentifierSyntax
//@[23:38)    Identifier |runtimeCheckVar|
//@[38:42) NewLine |\r\n\r\n|

resource singleResourceForRuntimeCheck 'Microsoft.Network/dnsZones@2018-05-01' = {
//@[0:131) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:38)  IdentifierSyntax
//@[9:38)   Identifier |singleResourceForRuntimeCheck|
//@[39:78)  StringSyntax
//@[39:78)   StringComplete |'Microsoft.Network/dnsZones@2018-05-01'|
//@[79:80)  Assignment |=|
//@[81:131)  ObjectSyntax
//@[81:82)   LeftBrace |{|
//@[82:84)   NewLine |\r\n|
  name: runtimeCheckVar2
//@[2:24)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:24)    VariableAccessSyntax
//@[8:24)     IdentifierSyntax
//@[8:24)      Identifier |runtimeCheckVar2|
//@[24:26)   NewLine |\r\n|
  location: 'test'
//@[2:18)   ObjectPropertySyntax
//@[2:10)    IdentifierSyntax
//@[2:10)     Identifier |location|
//@[10:11)    Colon |:|
//@[12:18)    StringSyntax
//@[12:18)     StringComplete |'test'|
//@[18:20)   NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource loopForRuntimeCheck2 'Microsoft.Network/dnsZones@2018-05-01' = [for thing in []: {
//@[0:141) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:29)  IdentifierSyntax
//@[9:29)   Identifier |loopForRuntimeCheck2|
//@[30:69)  StringSyntax
//@[30:69)   StringComplete |'Microsoft.Network/dnsZones@2018-05-01'|
//@[70:71)  Assignment |=|
//@[72:141)  ForSyntax
//@[72:73)   LeftSquare |[|
//@[73:76)   Identifier |for|
//@[77:82)   LocalVariableSyntax
//@[77:82)    IdentifierSyntax
//@[77:82)     Identifier |thing|
//@[83:85)   Identifier |in|
//@[86:88)   ArraySyntax
//@[86:87)    LeftSquare |[|
//@[87:88)    RightSquare |]|
//@[88:89)   Colon |:|
//@[90:140)   ObjectSyntax
//@[90:91)    LeftBrace |{|
//@[91:93)    NewLine |\r\n|
  name: runtimeCheckVar2
//@[2:24)    ObjectPropertySyntax
//@[2:6)     IdentifierSyntax
//@[2:6)      Identifier |name|
//@[6:7)     Colon |:|
//@[8:24)     VariableAccessSyntax
//@[8:24)      IdentifierSyntax
//@[8:24)       Identifier |runtimeCheckVar2|
//@[24:26)    NewLine |\r\n|
  location: 'test'
//@[2:18)    ObjectPropertySyntax
//@[2:10)     IdentifierSyntax
//@[2:10)      Identifier |location|
//@[10:11)     Colon |:|
//@[12:18)     StringSyntax
//@[12:18)      StringComplete |'test'|
//@[18:20)    NewLine |\r\n|
}]
//@[0:1)    RightBrace |}|
//@[1:2)   RightSquare |]|
//@[2:6) NewLine |\r\n\r\n|

resource loopForRuntimeCheck3 'Microsoft.Network/dnsZones@2018-05-01' = [for otherThing in []: {
//@[0:172) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:29)  IdentifierSyntax
//@[9:29)   Identifier |loopForRuntimeCheck3|
//@[30:69)  StringSyntax
//@[30:69)   StringComplete |'Microsoft.Network/dnsZones@2018-05-01'|
//@[70:71)  Assignment |=|
//@[72:172)  ForSyntax
//@[72:73)   LeftSquare |[|
//@[73:76)   Identifier |for|
//@[77:87)   LocalVariableSyntax
//@[77:87)    IdentifierSyntax
//@[77:87)     Identifier |otherThing|
//@[88:90)   Identifier |in|
//@[91:93)   ArraySyntax
//@[91:92)    LeftSquare |[|
//@[92:93)    RightSquare |]|
//@[93:94)   Colon |:|
//@[95:171)   ObjectSyntax
//@[95:96)    LeftBrace |{|
//@[96:98)    NewLine |\r\n|
  name: loopForRuntimeCheck[0].properties.zoneType
//@[2:50)    ObjectPropertySyntax
//@[2:6)     IdentifierSyntax
//@[2:6)      Identifier |name|
//@[6:7)     Colon |:|
//@[8:50)     PropertyAccessSyntax
//@[8:41)      PropertyAccessSyntax
//@[8:30)       ArrayAccessSyntax
//@[8:27)        VariableAccessSyntax
//@[8:27)         IdentifierSyntax
//@[8:27)          Identifier |loopForRuntimeCheck|
//@[27:28)        LeftSquare |[|
//@[28:29)        IntegerLiteralSyntax
//@[28:29)         Integer |0|
//@[29:30)        RightSquare |]|
//@[30:31)       Dot |.|
//@[31:41)       IdentifierSyntax
//@[31:41)        Identifier |properties|
//@[41:42)      Dot |.|
//@[42:50)      IdentifierSyntax
//@[42:50)       Identifier |zoneType|
//@[50:52)    NewLine |\r\n|
  location: 'test'
//@[2:18)    ObjectPropertySyntax
//@[2:10)     IdentifierSyntax
//@[2:10)      Identifier |location|
//@[10:11)     Colon |:|
//@[12:18)     StringSyntax
//@[12:18)      StringComplete |'test'|
//@[18:20)    NewLine |\r\n|
}]
//@[0:1)    RightBrace |}|
//@[1:2)   RightSquare |]|
//@[2:6) NewLine |\r\n\r\n|

var varForRuntimeCheck4a = loopForRuntimeCheck[0].properties.zoneType
//@[0:69) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:24)  IdentifierSyntax
//@[4:24)   Identifier |varForRuntimeCheck4a|
//@[25:26)  Assignment |=|
//@[27:69)  PropertyAccessSyntax
//@[27:60)   PropertyAccessSyntax
//@[27:49)    ArrayAccessSyntax
//@[27:46)     VariableAccessSyntax
//@[27:46)      IdentifierSyntax
//@[27:46)       Identifier |loopForRuntimeCheck|
//@[46:47)     LeftSquare |[|
//@[47:48)     IntegerLiteralSyntax
//@[47:48)      Integer |0|
//@[48:49)     RightSquare |]|
//@[49:50)    Dot |.|
//@[50:60)    IdentifierSyntax
//@[50:60)     Identifier |properties|
//@[60:61)   Dot |.|
//@[61:69)   IdentifierSyntax
//@[61:69)    Identifier |zoneType|
//@[69:71) NewLine |\r\n|
var varForRuntimeCheck4b = varForRuntimeCheck4a
//@[0:47) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:24)  IdentifierSyntax
//@[4:24)   Identifier |varForRuntimeCheck4b|
//@[25:26)  Assignment |=|
//@[27:47)  VariableAccessSyntax
//@[27:47)   IdentifierSyntax
//@[27:47)    Identifier |varForRuntimeCheck4a|
//@[47:49) NewLine |\r\n|
resource loopForRuntimeCheck4 'Microsoft.Network/dnsZones@2018-05-01' = [for otherThing in []: {
//@[0:150) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:29)  IdentifierSyntax
//@[9:29)   Identifier |loopForRuntimeCheck4|
//@[30:69)  StringSyntax
//@[30:69)   StringComplete |'Microsoft.Network/dnsZones@2018-05-01'|
//@[70:71)  Assignment |=|
//@[72:150)  ForSyntax
//@[72:73)   LeftSquare |[|
//@[73:76)   Identifier |for|
//@[77:87)   LocalVariableSyntax
//@[77:87)    IdentifierSyntax
//@[77:87)     Identifier |otherThing|
//@[88:90)   Identifier |in|
//@[91:93)   ArraySyntax
//@[91:92)    LeftSquare |[|
//@[92:93)    RightSquare |]|
//@[93:94)   Colon |:|
//@[95:149)   ObjectSyntax
//@[95:96)    LeftBrace |{|
//@[96:98)    NewLine |\r\n|
  name: varForRuntimeCheck4b
//@[2:28)    ObjectPropertySyntax
//@[2:6)     IdentifierSyntax
//@[2:6)      Identifier |name|
//@[6:7)     Colon |:|
//@[8:28)     VariableAccessSyntax
//@[8:28)      IdentifierSyntax
//@[8:28)       Identifier |varForRuntimeCheck4b|
//@[28:30)    NewLine |\r\n|
  location: 'test'
//@[2:18)    ObjectPropertySyntax
//@[2:10)     IdentifierSyntax
//@[2:10)      Identifier |location|
//@[10:11)     Colon |:|
//@[12:18)     StringSyntax
//@[12:18)      StringComplete |'test'|
//@[18:20)    NewLine |\r\n|
}]
//@[0:1)    RightBrace |}|
//@[1:2)   RightSquare |]|
//@[2:6) NewLine |\r\n\r\n|

resource missingTopLevelProperties 'Microsoft.Storage/storageAccounts@2020-08-01-preview' = {
//@[0:151) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:34)  IdentifierSyntax
//@[9:34)   Identifier |missingTopLevelProperties|
//@[35:89)  StringSyntax
//@[35:89)   StringComplete |'Microsoft.Storage/storageAccounts@2020-08-01-preview'|
//@[90:91)  Assignment |=|
//@[92:151)  ObjectSyntax
//@[92:93)   LeftBrace |{|
//@[93:95)   NewLine |\r\n|
  // #completionTest(0, 1, 2) -> topLevelProperties
//@[51:55)   NewLine |\r\n\r\n|

}
//@[0:1)   RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource missingTopLevelPropertiesExceptName 'Microsoft.Storage/storageAccounts@2020-08-01-preview' = {
//@[0:304) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:44)  IdentifierSyntax
//@[9:44)   Identifier |missingTopLevelPropertiesExceptName|
//@[45:99)  StringSyntax
//@[45:99)   StringComplete |'Microsoft.Storage/storageAccounts@2020-08-01-preview'|
//@[100:101)  Assignment |=|
//@[102:304)  ObjectSyntax
//@[102:103)   LeftBrace |{|
//@[103:105)   NewLine |\r\n|
  // #completionTest(0, 1, 2) -> topLevelPropertiesMinusName
//@[60:62)   NewLine |\r\n|
  name: 'me'
//@[2:12)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:12)    StringSyntax
//@[8:12)     StringComplete |'me'|
//@[12:14)   NewLine |\r\n|
  // do not remove whitespace before the closing curly
//@[54:56)   NewLine |\r\n|
  // #completionTest(0, 1, 2) -> topLevelPropertiesMinusName
//@[60:62)   NewLine |\r\n|
  
//@[2:4)   NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

// #completionTest(24,25,26,49,65) -> resourceTypes
//@[51:53) NewLine |\r\n|
resource unfinishedVnet 'Microsoft.Network/virtualNetworks@2020-06-01' = {
//@[0:468) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:23)  IdentifierSyntax
//@[9:23)   Identifier |unfinishedVnet|
//@[24:70)  StringSyntax
//@[24:70)   StringComplete |'Microsoft.Network/virtualNetworks@2020-06-01'|
//@[71:72)  Assignment |=|
//@[73:468)  ObjectSyntax
//@[73:74)   LeftBrace |{|
//@[74:76)   NewLine |\r\n|
  name: 'v'
//@[2:11)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:11)    StringSyntax
//@[8:11)     StringComplete |'v'|
//@[11:13)   NewLine |\r\n|
  location: 'eastus'
//@[2:20)   ObjectPropertySyntax
//@[2:10)    IdentifierSyntax
//@[2:10)     Identifier |location|
//@[10:11)    Colon |:|
//@[12:20)    StringSyntax
//@[12:20)     StringComplete |'eastus'|
//@[20:22)   NewLine |\r\n|
  properties: {
//@[2:354)   ObjectPropertySyntax
//@[2:12)    IdentifierSyntax
//@[2:12)     Identifier |properties|
//@[12:13)    Colon |:|
//@[14:354)    ObjectSyntax
//@[14:15)     LeftBrace |{|
//@[15:17)     NewLine |\r\n|
    subnets: [
//@[4:332)     ObjectPropertySyntax
//@[4:11)      IdentifierSyntax
//@[4:11)       Identifier |subnets|
//@[11:12)      Colon |:|
//@[13:332)      ArraySyntax
//@[13:14)       LeftSquare |[|
//@[14:16)       NewLine |\r\n|
      {
//@[6:309)       ArrayItemSyntax
//@[6:309)        ObjectSyntax
//@[6:7)         LeftBrace |{|
//@[7:9)         NewLine |\r\n|
        // #completionTest(0,1,2,3,4,5,6,7) -> subnetPropertiesMinusProperties
//@[78:80)         NewLine |\r\n|
        properties: {
//@[8:211)         ObjectPropertySyntax
//@[8:18)          IdentifierSyntax
//@[8:18)           Identifier |properties|
//@[18:19)          Colon |:|
//@[20:211)          ObjectSyntax
//@[20:21)           LeftBrace |{|
//@[21:23)           NewLine |\r\n|
          delegations: [
//@[10:177)           ObjectPropertySyntax
//@[10:21)            IdentifierSyntax
//@[10:21)             Identifier |delegations|
//@[21:22)            Colon |:|
//@[23:177)            ArraySyntax
//@[23:24)             LeftSquare |[|
//@[24:26)             NewLine |\r\n|
            {
//@[12:138)             ArrayItemSyntax
//@[12:138)              ObjectSyntax
//@[12:13)               LeftBrace |{|
//@[13:15)               NewLine |\r\n|
              // #completionTest(0,1,2,3,4,5,6,7,8,9,10,11,12,13,14) -> delegationProperties
//@[92:94)               NewLine |\r\n|
              
//@[14:16)               NewLine |\r\n|
            }
//@[12:13)               RightBrace |}|
//@[13:15)             NewLine |\r\n|
          ]
//@[10:11)             RightSquare |]|
//@[11:13)           NewLine |\r\n|
        }
//@[8:9)           RightBrace |}|
//@[9:11)         NewLine |\r\n|
      }
//@[6:7)         RightBrace |}|
//@[7:9)       NewLine |\r\n|
    ]
//@[4:5)       RightSquare |]|
//@[5:7)     NewLine |\r\n|
  }
//@[2:3)     RightBrace |}|
//@[3:5)   NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

/*
Discriminator key missing
*/
//@[2:4) NewLine |\r\n|
resource discriminatorKeyMissing 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
//@[0:148) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:32)  IdentifierSyntax
//@[9:32)   Identifier |discriminatorKeyMissing|
//@[33:83)  StringSyntax
//@[33:83)   StringComplete |'Microsoft.Resources/deploymentScripts@2020-10-01'|
//@[84:85)  Assignment |=|
//@[86:148)  ObjectSyntax
//@[86:87)   LeftBrace |{|
//@[87:89)   NewLine |\r\n|
  // #completionTest(0,1,2) -> discriminatorProperty
//@[52:54)   NewLine |\r\n|
  
//@[2:4)   NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

/*
Discriminator key missing (conditional)
*/
//@[2:4) NewLine |\r\n|
resource discriminatorKeyMissing_if 'Microsoft.Resources/deploymentScripts@2020-10-01' = if(true) {
//@[0:160) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:35)  IdentifierSyntax
//@[9:35)   Identifier |discriminatorKeyMissing_if|
//@[36:86)  StringSyntax
//@[36:86)   StringComplete |'Microsoft.Resources/deploymentScripts@2020-10-01'|
//@[87:88)  Assignment |=|
//@[89:160)  IfConditionSyntax
//@[89:91)   Identifier |if|
//@[91:97)   ParenthesizedExpressionSyntax
//@[91:92)    LeftParen |(|
//@[92:96)    BooleanLiteralSyntax
//@[92:96)     TrueKeyword |true|
//@[96:97)    RightParen |)|
//@[98:160)   ObjectSyntax
//@[98:99)    LeftBrace |{|
//@[99:101)    NewLine |\r\n|
  // #completionTest(0,1,2) -> discriminatorProperty
//@[52:54)    NewLine |\r\n|
  
//@[2:4)    NewLine |\r\n|
}
//@[0:1)    RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

/*
Discriminator key missing (loop)
*/
//@[2:4) NewLine |\r\n|
resource discriminatorKeyMissing_for 'Microsoft.Resources/deploymentScripts@2020-10-01' = [for thing in []: {
//@[0:171) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:36)  IdentifierSyntax
//@[9:36)   Identifier |discriminatorKeyMissing_for|
//@[37:87)  StringSyntax
//@[37:87)   StringComplete |'Microsoft.Resources/deploymentScripts@2020-10-01'|
//@[88:89)  Assignment |=|
//@[90:171)  ForSyntax
//@[90:91)   LeftSquare |[|
//@[91:94)   Identifier |for|
//@[95:100)   LocalVariableSyntax
//@[95:100)    IdentifierSyntax
//@[95:100)     Identifier |thing|
//@[101:103)   Identifier |in|
//@[104:106)   ArraySyntax
//@[104:105)    LeftSquare |[|
//@[105:106)    RightSquare |]|
//@[106:107)   Colon |:|
//@[108:170)   ObjectSyntax
//@[108:109)    LeftBrace |{|
//@[109:111)    NewLine |\r\n|
  // #completionTest(0,1,2) -> discriminatorProperty
//@[52:54)    NewLine |\r\n|
  
//@[2:4)    NewLine |\r\n|
}]
//@[0:1)    RightBrace |}|
//@[1:2)   RightSquare |]|
//@[2:6) NewLine |\r\n\r\n|

/*
Discriminator key missing (filtered loop)
*/
//@[2:4) NewLine |\r\n|
resource discriminatorKeyMissing_for_if 'Microsoft.Resources/deploymentScripts@2020-10-01' = [for thing in []: if(true) {
//@[0:183) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:39)  IdentifierSyntax
//@[9:39)   Identifier |discriminatorKeyMissing_for_if|
//@[40:90)  StringSyntax
//@[40:90)   StringComplete |'Microsoft.Resources/deploymentScripts@2020-10-01'|
//@[91:92)  Assignment |=|
//@[93:183)  ForSyntax
//@[93:94)   LeftSquare |[|
//@[94:97)   Identifier |for|
//@[98:103)   LocalVariableSyntax
//@[98:103)    IdentifierSyntax
//@[98:103)     Identifier |thing|
//@[104:106)   Identifier |in|
//@[107:109)   ArraySyntax
//@[107:108)    LeftSquare |[|
//@[108:109)    RightSquare |]|
//@[109:110)   Colon |:|
//@[111:182)   IfConditionSyntax
//@[111:113)    Identifier |if|
//@[113:119)    ParenthesizedExpressionSyntax
//@[113:114)     LeftParen |(|
//@[114:118)     BooleanLiteralSyntax
//@[114:118)      TrueKeyword |true|
//@[118:119)     RightParen |)|
//@[120:182)    ObjectSyntax
//@[120:121)     LeftBrace |{|
//@[121:123)     NewLine |\r\n|
  // #completionTest(0,1,2) -> discriminatorProperty
//@[52:54)     NewLine |\r\n|
  
//@[2:4)     NewLine |\r\n|
}]
//@[0:1)     RightBrace |}|
//@[1:2)   RightSquare |]|
//@[2:6) NewLine |\r\n\r\n|

/*
Discriminator key value missing with property access
*/
//@[2:4) NewLine |\r\n|
resource discriminatorKeyValueMissing 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
//@[0:175) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:37)  IdentifierSyntax
//@[9:37)   Identifier |discriminatorKeyValueMissing|
//@[38:88)  StringSyntax
//@[38:88)   StringComplete |'Microsoft.Resources/deploymentScripts@2020-10-01'|
//@[89:90)  Assignment |=|
//@[91:175)  ObjectSyntax
//@[91:92)   LeftBrace |{|
//@[92:94)   NewLine |\r\n|
  // #completionTest(7,8,9,10) -> deploymentScriptKindsPlusSymbols
//@[66:68)   NewLine |\r\n|
  kind:   
//@[2:10)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |kind|
//@[6:7)    Colon |:|
//@[10:10)    SkippedTriviaSyntax
//@[10:12)   NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\r\n|
// #completionTest(76) -> missingDiscriminatorPropertyAccess
//@[60:62) NewLine |\r\n|
var discriminatorKeyValueMissingCompletions = discriminatorKeyValueMissing.p
//@[0:76) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:43)  IdentifierSyntax
//@[4:43)   Identifier |discriminatorKeyValueMissingCompletions|
//@[44:45)  Assignment |=|
//@[46:76)  PropertyAccessSyntax
//@[46:74)   VariableAccessSyntax
//@[46:74)    IdentifierSyntax
//@[46:74)     Identifier |discriminatorKeyValueMissing|
//@[74:75)   Dot |.|
//@[75:76)   IdentifierSyntax
//@[75:76)    Identifier |p|
//@[76:78) NewLine |\r\n|
// #completionTest(76) -> missingDiscriminatorPropertyAccess
//@[60:62) NewLine |\r\n|
var discriminatorKeyValueMissingCompletions2 = discriminatorKeyValueMissing.
//@[0:76) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:44)  IdentifierSyntax
//@[4:44)   Identifier |discriminatorKeyValueMissingCompletions2|
//@[45:46)  Assignment |=|
//@[47:76)  PropertyAccessSyntax
//@[47:75)   VariableAccessSyntax
//@[47:75)    IdentifierSyntax
//@[47:75)     Identifier |discriminatorKeyValueMissing|
//@[75:76)   Dot |.|
//@[76:76)   IdentifierSyntax
//@[76:76)    SkippedTriviaSyntax
//@[76:80) NewLine |\r\n\r\n|

// #completionTest(76) -> missingDiscriminatorPropertyIndexPlusSymbols
//@[70:72) NewLine |\r\n|
var discriminatorKeyValueMissingCompletions3 = discriminatorKeyValueMissing[]
//@[0:77) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:44)  IdentifierSyntax
//@[4:44)   Identifier |discriminatorKeyValueMissingCompletions3|
//@[45:46)  Assignment |=|
//@[47:77)  ArrayAccessSyntax
//@[47:75)   VariableAccessSyntax
//@[47:75)    IdentifierSyntax
//@[47:75)     Identifier |discriminatorKeyValueMissing|
//@[75:76)   LeftSquare |[|
//@[76:76)   SkippedTriviaSyntax
//@[76:77)   RightSquare |]|
//@[77:81) NewLine |\r\n\r\n|

/*
Discriminator key value missing with property access (conditional)
*/
//@[2:4) NewLine |\r\n|
resource discriminatorKeyValueMissing_if 'Microsoft.Resources/deploymentScripts@2020-10-01' = if(false) {
//@[0:191) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:40)  IdentifierSyntax
//@[9:40)   Identifier |discriminatorKeyValueMissing_if|
//@[41:91)  StringSyntax
//@[41:91)   StringComplete |'Microsoft.Resources/deploymentScripts@2020-10-01'|
//@[92:93)  Assignment |=|
//@[94:191)  IfConditionSyntax
//@[94:96)   Identifier |if|
//@[96:103)   ParenthesizedExpressionSyntax
//@[96:97)    LeftParen |(|
//@[97:102)    BooleanLiteralSyntax
//@[97:102)     FalseKeyword |false|
//@[102:103)    RightParen |)|
//@[104:191)   ObjectSyntax
//@[104:105)    LeftBrace |{|
//@[105:107)    NewLine |\r\n|
  // #completionTest(7,8,9,10) -> deploymentScriptKindsPlusSymbols_if
//@[69:71)    NewLine |\r\n|
  kind:   
//@[2:10)    ObjectPropertySyntax
//@[2:6)     IdentifierSyntax
//@[2:6)      Identifier |kind|
//@[6:7)     Colon |:|
//@[10:10)     SkippedTriviaSyntax
//@[10:12)    NewLine |\r\n|
}
//@[0:1)    RightBrace |}|
//@[1:3) NewLine |\r\n|
// #completionTest(82) -> missingDiscriminatorPropertyAccess
//@[60:62) NewLine |\r\n|
var discriminatorKeyValueMissingCompletions_if = discriminatorKeyValueMissing_if.p
//@[0:82) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:46)  IdentifierSyntax
//@[4:46)   Identifier |discriminatorKeyValueMissingCompletions_if|
//@[47:48)  Assignment |=|
//@[49:82)  PropertyAccessSyntax
//@[49:80)   VariableAccessSyntax
//@[49:80)    IdentifierSyntax
//@[49:80)     Identifier |discriminatorKeyValueMissing_if|
//@[80:81)   Dot |.|
//@[81:82)   IdentifierSyntax
//@[81:82)    Identifier |p|
//@[82:84) NewLine |\r\n|
// #completionTest(82) -> missingDiscriminatorPropertyAccess
//@[60:62) NewLine |\r\n|
var discriminatorKeyValueMissingCompletions2_if = discriminatorKeyValueMissing_if.
//@[0:82) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:47)  IdentifierSyntax
//@[4:47)   Identifier |discriminatorKeyValueMissingCompletions2_if|
//@[48:49)  Assignment |=|
//@[50:82)  PropertyAccessSyntax
//@[50:81)   VariableAccessSyntax
//@[50:81)    IdentifierSyntax
//@[50:81)     Identifier |discriminatorKeyValueMissing_if|
//@[81:82)   Dot |.|
//@[82:82)   IdentifierSyntax
//@[82:82)    SkippedTriviaSyntax
//@[82:86) NewLine |\r\n\r\n|

// #completionTest(82) -> missingDiscriminatorPropertyIndexPlusSymbols_if
//@[73:75) NewLine |\r\n|
var discriminatorKeyValueMissingCompletions3_if = discriminatorKeyValueMissing_if[]
//@[0:83) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:47)  IdentifierSyntax
//@[4:47)   Identifier |discriminatorKeyValueMissingCompletions3_if|
//@[48:49)  Assignment |=|
//@[50:83)  ArrayAccessSyntax
//@[50:81)   VariableAccessSyntax
//@[50:81)    IdentifierSyntax
//@[50:81)     Identifier |discriminatorKeyValueMissing_if|
//@[81:82)   LeftSquare |[|
//@[82:82)   SkippedTriviaSyntax
//@[82:83)   RightSquare |]|
//@[83:87) NewLine |\r\n\r\n|

/*
Discriminator key value missing with property access (loops)
*/
//@[2:4) NewLine |\r\n|
resource discriminatorKeyValueMissing_for 'Microsoft.Resources/deploymentScripts@2020-10-01' = [for thing in []: {
//@[0:202) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:41)  IdentifierSyntax
//@[9:41)   Identifier |discriminatorKeyValueMissing_for|
//@[42:92)  StringSyntax
//@[42:92)   StringComplete |'Microsoft.Resources/deploymentScripts@2020-10-01'|
//@[93:94)  Assignment |=|
//@[95:202)  ForSyntax
//@[95:96)   LeftSquare |[|
//@[96:99)   Identifier |for|
//@[100:105)   LocalVariableSyntax
//@[100:105)    IdentifierSyntax
//@[100:105)     Identifier |thing|
//@[106:108)   Identifier |in|
//@[109:111)   ArraySyntax
//@[109:110)    LeftSquare |[|
//@[110:111)    RightSquare |]|
//@[111:112)   Colon |:|
//@[113:201)   ObjectSyntax
//@[113:114)    LeftBrace |{|
//@[114:116)    NewLine |\r\n|
  // #completionTest(7,8,9,10) -> deploymentScriptKindsPlusSymbols_for
//@[70:72)    NewLine |\r\n|
  kind:   
//@[2:10)    ObjectPropertySyntax
//@[2:6)     IdentifierSyntax
//@[2:6)      Identifier |kind|
//@[6:7)     Colon |:|
//@[10:10)     SkippedTriviaSyntax
//@[10:12)    NewLine |\r\n|
}]
//@[0:1)    RightBrace |}|
//@[1:2)   RightSquare |]|
//@[2:6) NewLine |\r\n\r\n|

// cannot . access properties of a resource loop
//@[48:50) NewLine |\r\n|
var resourceListIsNotSingleResource = discriminatorKeyValueMissing_for.kind
//@[0:75) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:35)  IdentifierSyntax
//@[4:35)   Identifier |resourceListIsNotSingleResource|
//@[36:37)  Assignment |=|
//@[38:75)  PropertyAccessSyntax
//@[38:70)   VariableAccessSyntax
//@[38:70)    IdentifierSyntax
//@[38:70)     Identifier |discriminatorKeyValueMissing_for|
//@[70:71)   Dot |.|
//@[71:75)   IdentifierSyntax
//@[71:75)    Identifier |kind|
//@[75:79) NewLine |\r\n\r\n|

// #completionTest(87) -> missingDiscriminatorPropertyAccess
//@[60:62) NewLine |\r\n|
var discriminatorKeyValueMissingCompletions_for = discriminatorKeyValueMissing_for[0].p
//@[0:87) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:47)  IdentifierSyntax
//@[4:47)   Identifier |discriminatorKeyValueMissingCompletions_for|
//@[48:49)  Assignment |=|
//@[50:87)  PropertyAccessSyntax
//@[50:85)   ArrayAccessSyntax
//@[50:82)    VariableAccessSyntax
//@[50:82)     IdentifierSyntax
//@[50:82)      Identifier |discriminatorKeyValueMissing_for|
//@[82:83)    LeftSquare |[|
//@[83:84)    IntegerLiteralSyntax
//@[83:84)     Integer |0|
//@[84:85)    RightSquare |]|
//@[85:86)   Dot |.|
//@[86:87)   IdentifierSyntax
//@[86:87)    Identifier |p|
//@[87:89) NewLine |\r\n|
// #completionTest(87) -> missingDiscriminatorPropertyAccess
//@[60:62) NewLine |\r\n|
var discriminatorKeyValueMissingCompletions2_for = discriminatorKeyValueMissing_for[0].
//@[0:87) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:48)  IdentifierSyntax
//@[4:48)   Identifier |discriminatorKeyValueMissingCompletions2_for|
//@[49:50)  Assignment |=|
//@[51:87)  PropertyAccessSyntax
//@[51:86)   ArrayAccessSyntax
//@[51:83)    VariableAccessSyntax
//@[51:83)     IdentifierSyntax
//@[51:83)      Identifier |discriminatorKeyValueMissing_for|
//@[83:84)    LeftSquare |[|
//@[84:85)    IntegerLiteralSyntax
//@[84:85)     Integer |0|
//@[85:86)    RightSquare |]|
//@[86:87)   Dot |.|
//@[87:87)   IdentifierSyntax
//@[87:87)    SkippedTriviaSyntax
//@[87:91) NewLine |\r\n\r\n|

// #completionTest(87) -> missingDiscriminatorPropertyIndexPlusSymbols_for
//@[74:76) NewLine |\r\n|
var discriminatorKeyValueMissingCompletions3_for = discriminatorKeyValueMissing_for[0][]
//@[0:88) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:48)  IdentifierSyntax
//@[4:48)   Identifier |discriminatorKeyValueMissingCompletions3_for|
//@[49:50)  Assignment |=|
//@[51:88)  ArrayAccessSyntax
//@[51:86)   ArrayAccessSyntax
//@[51:83)    VariableAccessSyntax
//@[51:83)     IdentifierSyntax
//@[51:83)      Identifier |discriminatorKeyValueMissing_for|
//@[83:84)    LeftSquare |[|
//@[84:85)    IntegerLiteralSyntax
//@[84:85)     Integer |0|
//@[85:86)    RightSquare |]|
//@[86:87)   LeftSquare |[|
//@[87:87)   SkippedTriviaSyntax
//@[87:88)   RightSquare |]|
//@[88:92) NewLine |\r\n\r\n|

/*
Discriminator key value missing with property access (filtered loops)
*/
//@[2:4) NewLine |\r\n|
resource discriminatorKeyValueMissing_for_if 'Microsoft.Resources/deploymentScripts@2020-10-01' = [for thing in []: if(true) {
//@[0:217) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:44)  IdentifierSyntax
//@[9:44)   Identifier |discriminatorKeyValueMissing_for_if|
//@[45:95)  StringSyntax
//@[45:95)   StringComplete |'Microsoft.Resources/deploymentScripts@2020-10-01'|
//@[96:97)  Assignment |=|
//@[98:217)  ForSyntax
//@[98:99)   LeftSquare |[|
//@[99:102)   Identifier |for|
//@[103:108)   LocalVariableSyntax
//@[103:108)    IdentifierSyntax
//@[103:108)     Identifier |thing|
//@[109:111)   Identifier |in|
//@[112:114)   ArraySyntax
//@[112:113)    LeftSquare |[|
//@[113:114)    RightSquare |]|
//@[114:115)   Colon |:|
//@[116:216)   IfConditionSyntax
//@[116:118)    Identifier |if|
//@[118:124)    ParenthesizedExpressionSyntax
//@[118:119)     LeftParen |(|
//@[119:123)     BooleanLiteralSyntax
//@[119:123)      TrueKeyword |true|
//@[123:124)     RightParen |)|
//@[125:216)    ObjectSyntax
//@[125:126)     LeftBrace |{|
//@[126:128)     NewLine |\r\n|
  // #completionTest(7,8,9,10) -> deploymentScriptKindsPlusSymbols_for_if
//@[73:75)     NewLine |\r\n|
  kind:   
//@[2:10)     ObjectPropertySyntax
//@[2:6)      IdentifierSyntax
//@[2:6)       Identifier |kind|
//@[6:7)      Colon |:|
//@[10:10)      SkippedTriviaSyntax
//@[10:12)     NewLine |\r\n|
}]
//@[0:1)     RightBrace |}|
//@[1:2)   RightSquare |]|
//@[2:6) NewLine |\r\n\r\n|

// cannot . access properties of a resource loop
//@[48:50) NewLine |\r\n|
var resourceListIsNotSingleResource_if = discriminatorKeyValueMissing_for_if.kind
//@[0:81) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:38)  IdentifierSyntax
//@[4:38)   Identifier |resourceListIsNotSingleResource_if|
//@[39:40)  Assignment |=|
//@[41:81)  PropertyAccessSyntax
//@[41:76)   VariableAccessSyntax
//@[41:76)    IdentifierSyntax
//@[41:76)     Identifier |discriminatorKeyValueMissing_for_if|
//@[76:77)   Dot |.|
//@[77:81)   IdentifierSyntax
//@[77:81)    Identifier |kind|
//@[81:85) NewLine |\r\n\r\n|

// #completionTest(93) -> missingDiscriminatorPropertyAccess
//@[60:62) NewLine |\r\n|
var discriminatorKeyValueMissingCompletions_for_if = discriminatorKeyValueMissing_for_if[0].p
//@[0:93) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:50)  IdentifierSyntax
//@[4:50)   Identifier |discriminatorKeyValueMissingCompletions_for_if|
//@[51:52)  Assignment |=|
//@[53:93)  PropertyAccessSyntax
//@[53:91)   ArrayAccessSyntax
//@[53:88)    VariableAccessSyntax
//@[53:88)     IdentifierSyntax
//@[53:88)      Identifier |discriminatorKeyValueMissing_for_if|
//@[88:89)    LeftSquare |[|
//@[89:90)    IntegerLiteralSyntax
//@[89:90)     Integer |0|
//@[90:91)    RightSquare |]|
//@[91:92)   Dot |.|
//@[92:93)   IdentifierSyntax
//@[92:93)    Identifier |p|
//@[93:95) NewLine |\r\n|
// #completionTest(93) -> missingDiscriminatorPropertyAccess
//@[60:62) NewLine |\r\n|
var discriminatorKeyValueMissingCompletions2_for_if = discriminatorKeyValueMissing_for_if[0].
//@[0:93) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:51)  IdentifierSyntax
//@[4:51)   Identifier |discriminatorKeyValueMissingCompletions2_for_if|
//@[52:53)  Assignment |=|
//@[54:93)  PropertyAccessSyntax
//@[54:92)   ArrayAccessSyntax
//@[54:89)    VariableAccessSyntax
//@[54:89)     IdentifierSyntax
//@[54:89)      Identifier |discriminatorKeyValueMissing_for_if|
//@[89:90)    LeftSquare |[|
//@[90:91)    IntegerLiteralSyntax
//@[90:91)     Integer |0|
//@[91:92)    RightSquare |]|
//@[92:93)   Dot |.|
//@[93:93)   IdentifierSyntax
//@[93:93)    SkippedTriviaSyntax
//@[93:97) NewLine |\r\n\r\n|

// #completionTest(93) -> missingDiscriminatorPropertyIndexPlusSymbols_for_if
//@[77:79) NewLine |\r\n|
var discriminatorKeyValueMissingCompletions3_for_if = discriminatorKeyValueMissing_for_if[0][]
//@[0:94) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:51)  IdentifierSyntax
//@[4:51)   Identifier |discriminatorKeyValueMissingCompletions3_for_if|
//@[52:53)  Assignment |=|
//@[54:94)  ArrayAccessSyntax
//@[54:92)   ArrayAccessSyntax
//@[54:89)    VariableAccessSyntax
//@[54:89)     IdentifierSyntax
//@[54:89)      Identifier |discriminatorKeyValueMissing_for_if|
//@[89:90)    LeftSquare |[|
//@[90:91)    IntegerLiteralSyntax
//@[90:91)     Integer |0|
//@[91:92)    RightSquare |]|
//@[92:93)   LeftSquare |[|
//@[93:93)   SkippedTriviaSyntax
//@[93:94)   RightSquare |]|
//@[94:98) NewLine |\r\n\r\n|

/*
Discriminator value set 1
*/
//@[2:4) NewLine |\r\n|
resource discriminatorKeySetOne 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
//@[0:264) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:31)  IdentifierSyntax
//@[9:31)   Identifier |discriminatorKeySetOne|
//@[32:82)  StringSyntax
//@[32:82)   StringComplete |'Microsoft.Resources/deploymentScripts@2020-10-01'|
//@[83:84)  Assignment |=|
//@[85:264)  ObjectSyntax
//@[85:86)   LeftBrace |{|
//@[86:88)   NewLine |\r\n|
  kind: 'AzureCLI'
//@[2:18)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |kind|
//@[6:7)    Colon |:|
//@[8:18)    StringSyntax
//@[8:18)     StringComplete |'AzureCLI'|
//@[18:20)   NewLine |\r\n|
  // #completionTest(0,1,2) -> deploymentScriptTopLevel
//@[55:59)   NewLine |\r\n\r\n|

  properties: {
//@[2:94)   ObjectPropertySyntax
//@[2:12)    IdentifierSyntax
//@[2:12)     Identifier |properties|
//@[12:13)    Colon |:|
//@[14:94)    ObjectSyntax
//@[14:15)     LeftBrace |{|
//@[15:17)     NewLine |\r\n|
    // #completionTest(0,1,2,3,4) -> deploymentScriptCliProperties
//@[66:68)     NewLine |\r\n|
    
//@[4:6)     NewLine |\r\n|
  }
//@[2:3)     RightBrace |}|
//@[3:5)   NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\r\n|
// #completionTest(75) -> cliPropertyAccess
//@[43:45) NewLine |\r\n|
var discriminatorKeySetOneCompletions = discriminatorKeySetOne.properties.a
//@[0:75) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:37)  IdentifierSyntax
//@[4:37)   Identifier |discriminatorKeySetOneCompletions|
//@[38:39)  Assignment |=|
//@[40:75)  PropertyAccessSyntax
//@[40:73)   PropertyAccessSyntax
//@[40:62)    VariableAccessSyntax
//@[40:62)     IdentifierSyntax
//@[40:62)      Identifier |discriminatorKeySetOne|
//@[62:63)    Dot |.|
//@[63:73)    IdentifierSyntax
//@[63:73)     Identifier |properties|
//@[73:74)   Dot |.|
//@[74:75)   IdentifierSyntax
//@[74:75)    Identifier |a|
//@[75:77) NewLine |\r\n|
// #completionTest(75) -> cliPropertyAccess
//@[43:45) NewLine |\r\n|
var discriminatorKeySetOneCompletions2 = discriminatorKeySetOne.properties.
//@[0:75) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:38)  IdentifierSyntax
//@[4:38)   Identifier |discriminatorKeySetOneCompletions2|
//@[39:40)  Assignment |=|
//@[41:75)  PropertyAccessSyntax
//@[41:74)   PropertyAccessSyntax
//@[41:63)    VariableAccessSyntax
//@[41:63)     IdentifierSyntax
//@[41:63)      Identifier |discriminatorKeySetOne|
//@[63:64)    Dot |.|
//@[64:74)    IdentifierSyntax
//@[64:74)     Identifier |properties|
//@[74:75)   Dot |.|
//@[75:75)   IdentifierSyntax
//@[75:75)    SkippedTriviaSyntax
//@[75:79) NewLine |\r\n\r\n|

// #completionTest(75) -> cliPropertyAccessIndexesPlusSymbols
//@[61:63) NewLine |\r\n|
var discriminatorKeySetOneCompletions3 = discriminatorKeySetOne.properties[]
//@[0:76) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:38)  IdentifierSyntax
//@[4:38)   Identifier |discriminatorKeySetOneCompletions3|
//@[39:40)  Assignment |=|
//@[41:76)  ArrayAccessSyntax
//@[41:74)   PropertyAccessSyntax
//@[41:63)    VariableAccessSyntax
//@[41:63)     IdentifierSyntax
//@[41:63)      Identifier |discriminatorKeySetOne|
//@[63:64)    Dot |.|
//@[64:74)    IdentifierSyntax
//@[64:74)     Identifier |properties|
//@[74:75)   LeftSquare |[|
//@[75:75)   SkippedTriviaSyntax
//@[75:76)   RightSquare |]|
//@[76:80) NewLine |\r\n\r\n|

/*
Discriminator value set 1 (conditional)
*/
//@[2:4) NewLine |\r\n|
resource discriminatorKeySetOne_if 'Microsoft.Resources/deploymentScripts@2020-10-01' = if(2==3) {
//@[0:276) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:34)  IdentifierSyntax
//@[9:34)   Identifier |discriminatorKeySetOne_if|
//@[35:85)  StringSyntax
//@[35:85)   StringComplete |'Microsoft.Resources/deploymentScripts@2020-10-01'|
//@[86:87)  Assignment |=|
//@[88:276)  IfConditionSyntax
//@[88:90)   Identifier |if|
//@[90:96)   ParenthesizedExpressionSyntax
//@[90:91)    LeftParen |(|
//@[91:95)    BinaryOperationSyntax
//@[91:92)     IntegerLiteralSyntax
//@[91:92)      Integer |2|
//@[92:94)     Equals |==|
//@[94:95)     IntegerLiteralSyntax
//@[94:95)      Integer |3|
//@[95:96)    RightParen |)|
//@[97:276)   ObjectSyntax
//@[97:98)    LeftBrace |{|
//@[98:100)    NewLine |\r\n|
  kind: 'AzureCLI'
//@[2:18)    ObjectPropertySyntax
//@[2:6)     IdentifierSyntax
//@[2:6)      Identifier |kind|
//@[6:7)     Colon |:|
//@[8:18)     StringSyntax
//@[8:18)      StringComplete |'AzureCLI'|
//@[18:20)    NewLine |\r\n|
  // #completionTest(0,1,2) -> deploymentScriptTopLevel
//@[55:59)    NewLine |\r\n\r\n|

  properties: {
//@[2:94)    ObjectPropertySyntax
//@[2:12)     IdentifierSyntax
//@[2:12)      Identifier |properties|
//@[12:13)     Colon |:|
//@[14:94)     ObjectSyntax
//@[14:15)      LeftBrace |{|
//@[15:17)      NewLine |\r\n|
    // #completionTest(0,1,2,3,4) -> deploymentScriptCliProperties
//@[66:68)      NewLine |\r\n|
    
//@[4:6)      NewLine |\r\n|
  }
//@[2:3)      RightBrace |}|
//@[3:5)    NewLine |\r\n|
}
//@[0:1)    RightBrace |}|
//@[1:3) NewLine |\r\n|
// #completionTest(81) -> cliPropertyAccess
//@[43:45) NewLine |\r\n|
var discriminatorKeySetOneCompletions_if = discriminatorKeySetOne_if.properties.a
//@[0:81) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:40)  IdentifierSyntax
//@[4:40)   Identifier |discriminatorKeySetOneCompletions_if|
//@[41:42)  Assignment |=|
//@[43:81)  PropertyAccessSyntax
//@[43:79)   PropertyAccessSyntax
//@[43:68)    VariableAccessSyntax
//@[43:68)     IdentifierSyntax
//@[43:68)      Identifier |discriminatorKeySetOne_if|
//@[68:69)    Dot |.|
//@[69:79)    IdentifierSyntax
//@[69:79)     Identifier |properties|
//@[79:80)   Dot |.|
//@[80:81)   IdentifierSyntax
//@[80:81)    Identifier |a|
//@[81:83) NewLine |\r\n|
// #completionTest(81) -> cliPropertyAccess
//@[43:45) NewLine |\r\n|
var discriminatorKeySetOneCompletions2_if = discriminatorKeySetOne_if.properties.
//@[0:81) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:41)  IdentifierSyntax
//@[4:41)   Identifier |discriminatorKeySetOneCompletions2_if|
//@[42:43)  Assignment |=|
//@[44:81)  PropertyAccessSyntax
//@[44:80)   PropertyAccessSyntax
//@[44:69)    VariableAccessSyntax
//@[44:69)     IdentifierSyntax
//@[44:69)      Identifier |discriminatorKeySetOne_if|
//@[69:70)    Dot |.|
//@[70:80)    IdentifierSyntax
//@[70:80)     Identifier |properties|
//@[80:81)   Dot |.|
//@[81:81)   IdentifierSyntax
//@[81:81)    SkippedTriviaSyntax
//@[81:85) NewLine |\r\n\r\n|

// #completionTest(81) -> cliPropertyAccessIndexesPlusSymbols_if
//@[64:66) NewLine |\r\n|
var discriminatorKeySetOneCompletions3_if = discriminatorKeySetOne_if.properties[]
//@[0:82) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:41)  IdentifierSyntax
//@[4:41)   Identifier |discriminatorKeySetOneCompletions3_if|
//@[42:43)  Assignment |=|
//@[44:82)  ArrayAccessSyntax
//@[44:80)   PropertyAccessSyntax
//@[44:69)    VariableAccessSyntax
//@[44:69)     IdentifierSyntax
//@[44:69)      Identifier |discriminatorKeySetOne_if|
//@[69:70)    Dot |.|
//@[70:80)    IdentifierSyntax
//@[70:80)     Identifier |properties|
//@[80:81)   LeftSquare |[|
//@[81:81)   SkippedTriviaSyntax
//@[81:82)   RightSquare |]|
//@[82:86) NewLine |\r\n\r\n|

/*
Discriminator value set 1 (loop)
*/
//@[2:4) NewLine |\r\n|
resource discriminatorKeySetOne_for 'Microsoft.Resources/deploymentScripts@2020-10-01' = [ for thing in []: {
//@[0:288) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:35)  IdentifierSyntax
//@[9:35)   Identifier |discriminatorKeySetOne_for|
//@[36:86)  StringSyntax
//@[36:86)   StringComplete |'Microsoft.Resources/deploymentScripts@2020-10-01'|
//@[87:88)  Assignment |=|
//@[89:288)  ForSyntax
//@[89:90)   LeftSquare |[|
//@[91:94)   Identifier |for|
//@[95:100)   LocalVariableSyntax
//@[95:100)    IdentifierSyntax
//@[95:100)     Identifier |thing|
//@[101:103)   Identifier |in|
//@[104:106)   ArraySyntax
//@[104:105)    LeftSquare |[|
//@[105:106)    RightSquare |]|
//@[106:107)   Colon |:|
//@[108:287)   ObjectSyntax
//@[108:109)    LeftBrace |{|
//@[109:111)    NewLine |\r\n|
  kind: 'AzureCLI'
//@[2:18)    ObjectPropertySyntax
//@[2:6)     IdentifierSyntax
//@[2:6)      Identifier |kind|
//@[6:7)     Colon |:|
//@[8:18)     StringSyntax
//@[8:18)      StringComplete |'AzureCLI'|
//@[18:20)    NewLine |\r\n|
  // #completionTest(0,1,2) -> deploymentScriptTopLevel
//@[55:59)    NewLine |\r\n\r\n|

  properties: {
//@[2:94)    ObjectPropertySyntax
//@[2:12)     IdentifierSyntax
//@[2:12)      Identifier |properties|
//@[12:13)     Colon |:|
//@[14:94)     ObjectSyntax
//@[14:15)      LeftBrace |{|
//@[15:17)      NewLine |\r\n|
    // #completionTest(0,1,2,3,4) -> deploymentScriptCliProperties
//@[66:68)      NewLine |\r\n|
    
//@[4:6)      NewLine |\r\n|
  }
//@[2:3)      RightBrace |}|
//@[3:5)    NewLine |\r\n|
}]
//@[0:1)    RightBrace |}|
//@[1:2)   RightSquare |]|
//@[2:4) NewLine |\r\n|
// #completionTest(86) -> cliPropertyAccess
//@[43:45) NewLine |\r\n|
var discriminatorKeySetOneCompletions_for = discriminatorKeySetOne_for[0].properties.a
//@[0:86) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:41)  IdentifierSyntax
//@[4:41)   Identifier |discriminatorKeySetOneCompletions_for|
//@[42:43)  Assignment |=|
//@[44:86)  PropertyAccessSyntax
//@[44:84)   PropertyAccessSyntax
//@[44:73)    ArrayAccessSyntax
//@[44:70)     VariableAccessSyntax
//@[44:70)      IdentifierSyntax
//@[44:70)       Identifier |discriminatorKeySetOne_for|
//@[70:71)     LeftSquare |[|
//@[71:72)     IntegerLiteralSyntax
//@[71:72)      Integer |0|
//@[72:73)     RightSquare |]|
//@[73:74)    Dot |.|
//@[74:84)    IdentifierSyntax
//@[74:84)     Identifier |properties|
//@[84:85)   Dot |.|
//@[85:86)   IdentifierSyntax
//@[85:86)    Identifier |a|
//@[86:88) NewLine |\r\n|
// #completionTest(94) -> cliPropertyAccess
//@[43:45) NewLine |\r\n|
var discriminatorKeySetOneCompletions2_for = discriminatorKeySetOne_for[any(true)].properties.
//@[0:94) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:42)  IdentifierSyntax
//@[4:42)   Identifier |discriminatorKeySetOneCompletions2_for|
//@[43:44)  Assignment |=|
//@[45:94)  PropertyAccessSyntax
//@[45:93)   PropertyAccessSyntax
//@[45:82)    ArrayAccessSyntax
//@[45:71)     VariableAccessSyntax
//@[45:71)      IdentifierSyntax
//@[45:71)       Identifier |discriminatorKeySetOne_for|
//@[71:72)     LeftSquare |[|
//@[72:81)     FunctionCallSyntax
//@[72:75)      IdentifierSyntax
//@[72:75)       Identifier |any|
//@[75:76)      LeftParen |(|
//@[76:80)      FunctionArgumentSyntax
//@[76:80)       BooleanLiteralSyntax
//@[76:80)        TrueKeyword |true|
//@[80:81)      RightParen |)|
//@[81:82)     RightSquare |]|
//@[82:83)    Dot |.|
//@[83:93)    IdentifierSyntax
//@[83:93)     Identifier |properties|
//@[93:94)   Dot |.|
//@[94:94)   IdentifierSyntax
//@[94:94)    SkippedTriviaSyntax
//@[94:98) NewLine |\r\n\r\n|

// #completionTest(86) -> cliPropertyAccessIndexesPlusSymbols_for
//@[65:67) NewLine |\r\n|
var discriminatorKeySetOneCompletions3_for = discriminatorKeySetOne_for[1].properties[]
//@[0:87) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:42)  IdentifierSyntax
//@[4:42)   Identifier |discriminatorKeySetOneCompletions3_for|
//@[43:44)  Assignment |=|
//@[45:87)  ArrayAccessSyntax
//@[45:85)   PropertyAccessSyntax
//@[45:74)    ArrayAccessSyntax
//@[45:71)     VariableAccessSyntax
//@[45:71)      IdentifierSyntax
//@[45:71)       Identifier |discriminatorKeySetOne_for|
//@[71:72)     LeftSquare |[|
//@[72:73)     IntegerLiteralSyntax
//@[72:73)      Integer |1|
//@[73:74)     RightSquare |]|
//@[74:75)    Dot |.|
//@[75:85)    IdentifierSyntax
//@[75:85)     Identifier |properties|
//@[85:86)   LeftSquare |[|
//@[86:86)   SkippedTriviaSyntax
//@[86:87)   RightSquare |]|
//@[87:91) NewLine |\r\n\r\n|

/*
Discriminator value set 1 (filtered loop)
*/
//@[2:4) NewLine |\r\n|
resource discriminatorKeySetOne_for_if 'Microsoft.Resources/deploymentScripts@2020-10-01' = [ for thing in []: if(true) {
//@[0:300) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:38)  IdentifierSyntax
//@[9:38)   Identifier |discriminatorKeySetOne_for_if|
//@[39:89)  StringSyntax
//@[39:89)   StringComplete |'Microsoft.Resources/deploymentScripts@2020-10-01'|
//@[90:91)  Assignment |=|
//@[92:300)  ForSyntax
//@[92:93)   LeftSquare |[|
//@[94:97)   Identifier |for|
//@[98:103)   LocalVariableSyntax
//@[98:103)    IdentifierSyntax
//@[98:103)     Identifier |thing|
//@[104:106)   Identifier |in|
//@[107:109)   ArraySyntax
//@[107:108)    LeftSquare |[|
//@[108:109)    RightSquare |]|
//@[109:110)   Colon |:|
//@[111:299)   IfConditionSyntax
//@[111:113)    Identifier |if|
//@[113:119)    ParenthesizedExpressionSyntax
//@[113:114)     LeftParen |(|
//@[114:118)     BooleanLiteralSyntax
//@[114:118)      TrueKeyword |true|
//@[118:119)     RightParen |)|
//@[120:299)    ObjectSyntax
//@[120:121)     LeftBrace |{|
//@[121:123)     NewLine |\r\n|
  kind: 'AzureCLI'
//@[2:18)     ObjectPropertySyntax
//@[2:6)      IdentifierSyntax
//@[2:6)       Identifier |kind|
//@[6:7)      Colon |:|
//@[8:18)      StringSyntax
//@[8:18)       StringComplete |'AzureCLI'|
//@[18:20)     NewLine |\r\n|
  // #completionTest(0,1,2) -> deploymentScriptTopLevel
//@[55:59)     NewLine |\r\n\r\n|

  properties: {
//@[2:94)     ObjectPropertySyntax
//@[2:12)      IdentifierSyntax
//@[2:12)       Identifier |properties|
//@[12:13)      Colon |:|
//@[14:94)      ObjectSyntax
//@[14:15)       LeftBrace |{|
//@[15:17)       NewLine |\r\n|
    // #completionTest(0,1,2,3,4) -> deploymentScriptCliProperties
//@[66:68)       NewLine |\r\n|
    
//@[4:6)       NewLine |\r\n|
  }
//@[2:3)       RightBrace |}|
//@[3:5)     NewLine |\r\n|
}]
//@[0:1)     RightBrace |}|
//@[1:2)   RightSquare |]|
//@[2:4) NewLine |\r\n|
// #completionTest(92) -> cliPropertyAccess
//@[43:45) NewLine |\r\n|
var discriminatorKeySetOneCompletions_for_if = discriminatorKeySetOne_for_if[0].properties.a
//@[0:92) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:44)  IdentifierSyntax
//@[4:44)   Identifier |discriminatorKeySetOneCompletions_for_if|
//@[45:46)  Assignment |=|
//@[47:92)  PropertyAccessSyntax
//@[47:90)   PropertyAccessSyntax
//@[47:79)    ArrayAccessSyntax
//@[47:76)     VariableAccessSyntax
//@[47:76)      IdentifierSyntax
//@[47:76)       Identifier |discriminatorKeySetOne_for_if|
//@[76:77)     LeftSquare |[|
//@[77:78)     IntegerLiteralSyntax
//@[77:78)      Integer |0|
//@[78:79)     RightSquare |]|
//@[79:80)    Dot |.|
//@[80:90)    IdentifierSyntax
//@[80:90)     Identifier |properties|
//@[90:91)   Dot |.|
//@[91:92)   IdentifierSyntax
//@[91:92)    Identifier |a|
//@[92:94) NewLine |\r\n|
// #completionTest(100) -> cliPropertyAccess
//@[44:46) NewLine |\r\n|
var discriminatorKeySetOneCompletions2_for_if = discriminatorKeySetOne_for_if[any(true)].properties.
//@[0:100) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:45)  IdentifierSyntax
//@[4:45)   Identifier |discriminatorKeySetOneCompletions2_for_if|
//@[46:47)  Assignment |=|
//@[48:100)  PropertyAccessSyntax
//@[48:99)   PropertyAccessSyntax
//@[48:88)    ArrayAccessSyntax
//@[48:77)     VariableAccessSyntax
//@[48:77)      IdentifierSyntax
//@[48:77)       Identifier |discriminatorKeySetOne_for_if|
//@[77:78)     LeftSquare |[|
//@[78:87)     FunctionCallSyntax
//@[78:81)      IdentifierSyntax
//@[78:81)       Identifier |any|
//@[81:82)      LeftParen |(|
//@[82:86)      FunctionArgumentSyntax
//@[82:86)       BooleanLiteralSyntax
//@[82:86)        TrueKeyword |true|
//@[86:87)      RightParen |)|
//@[87:88)     RightSquare |]|
//@[88:89)    Dot |.|
//@[89:99)    IdentifierSyntax
//@[89:99)     Identifier |properties|
//@[99:100)   Dot |.|
//@[100:100)   IdentifierSyntax
//@[100:100)    SkippedTriviaSyntax
//@[100:104) NewLine |\r\n\r\n|

// #completionTest(92) -> cliPropertyAccessIndexesPlusSymbols_for_if
//@[68:70) NewLine |\r\n|
var discriminatorKeySetOneCompletions3_for_if = discriminatorKeySetOne_for_if[1].properties[]
//@[0:93) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:45)  IdentifierSyntax
//@[4:45)   Identifier |discriminatorKeySetOneCompletions3_for_if|
//@[46:47)  Assignment |=|
//@[48:93)  ArrayAccessSyntax
//@[48:91)   PropertyAccessSyntax
//@[48:80)    ArrayAccessSyntax
//@[48:77)     VariableAccessSyntax
//@[48:77)      IdentifierSyntax
//@[48:77)       Identifier |discriminatorKeySetOne_for_if|
//@[77:78)     LeftSquare |[|
//@[78:79)     IntegerLiteralSyntax
//@[78:79)      Integer |1|
//@[79:80)     RightSquare |]|
//@[80:81)    Dot |.|
//@[81:91)    IdentifierSyntax
//@[81:91)     Identifier |properties|
//@[91:92)   LeftSquare |[|
//@[92:92)   SkippedTriviaSyntax
//@[92:93)   RightSquare |]|
//@[93:99) NewLine |\r\n\r\n\r\n|


/*
Discriminator value set 2
*/
//@[2:4) NewLine |\r\n|
resource discriminatorKeySetTwo 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
//@[0:270) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:31)  IdentifierSyntax
//@[9:31)   Identifier |discriminatorKeySetTwo|
//@[32:82)  StringSyntax
//@[32:82)   StringComplete |'Microsoft.Resources/deploymentScripts@2020-10-01'|
//@[83:84)  Assignment |=|
//@[85:270)  ObjectSyntax
//@[85:86)   LeftBrace |{|
//@[86:88)   NewLine |\r\n|
  kind: 'AzurePowerShell'
//@[2:25)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |kind|
//@[6:7)    Colon |:|
//@[8:25)    StringSyntax
//@[8:25)     StringComplete |'AzurePowerShell'|
//@[25:27)   NewLine |\r\n|
  // #completionTest(0,1,2) -> deploymentScriptTopLevel
//@[55:59)   NewLine |\r\n\r\n|

  properties: {
//@[2:93)   ObjectPropertySyntax
//@[2:12)    IdentifierSyntax
//@[2:12)     Identifier |properties|
//@[12:13)    Colon |:|
//@[14:93)    ObjectSyntax
//@[14:15)     LeftBrace |{|
//@[15:17)     NewLine |\r\n|
    // #completionTest(0,1,2,3,4) -> deploymentScriptPSProperties
//@[65:67)     NewLine |\r\n|
    
//@[4:6)     NewLine |\r\n|
  }
//@[2:3)     RightBrace |}|
//@[3:5)   NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\r\n|
// #completionTest(75) -> powershellPropertyAccess
//@[50:52) NewLine |\r\n|
var discriminatorKeySetTwoCompletions = discriminatorKeySetTwo.properties.a
//@[0:75) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:37)  IdentifierSyntax
//@[4:37)   Identifier |discriminatorKeySetTwoCompletions|
//@[38:39)  Assignment |=|
//@[40:75)  PropertyAccessSyntax
//@[40:73)   PropertyAccessSyntax
//@[40:62)    VariableAccessSyntax
//@[40:62)     IdentifierSyntax
//@[40:62)      Identifier |discriminatorKeySetTwo|
//@[62:63)    Dot |.|
//@[63:73)    IdentifierSyntax
//@[63:73)     Identifier |properties|
//@[73:74)   Dot |.|
//@[74:75)   IdentifierSyntax
//@[74:75)    Identifier |a|
//@[75:77) NewLine |\r\n|
// #completionTest(75) -> powershellPropertyAccess
//@[50:52) NewLine |\r\n|
var discriminatorKeySetTwoCompletions2 = discriminatorKeySetTwo.properties.
//@[0:75) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:38)  IdentifierSyntax
//@[4:38)   Identifier |discriminatorKeySetTwoCompletions2|
//@[39:40)  Assignment |=|
//@[41:75)  PropertyAccessSyntax
//@[41:74)   PropertyAccessSyntax
//@[41:63)    VariableAccessSyntax
//@[41:63)     IdentifierSyntax
//@[41:63)      Identifier |discriminatorKeySetTwo|
//@[63:64)    Dot |.|
//@[64:74)    IdentifierSyntax
//@[64:74)     Identifier |properties|
//@[74:75)   Dot |.|
//@[75:75)   IdentifierSyntax
//@[75:75)    SkippedTriviaSyntax
//@[75:79) NewLine |\r\n\r\n|

// #completionTest(90) -> powershellPropertyAccess
//@[50:52) NewLine |\r\n|
var discriminatorKeySetTwoCompletionsArrayIndexer = discriminatorKeySetTwo['properties'].a
//@[0:90) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:49)  IdentifierSyntax
//@[4:49)   Identifier |discriminatorKeySetTwoCompletionsArrayIndexer|
//@[50:51)  Assignment |=|
//@[52:90)  PropertyAccessSyntax
//@[52:88)   ArrayAccessSyntax
//@[52:74)    VariableAccessSyntax
//@[52:74)     IdentifierSyntax
//@[52:74)      Identifier |discriminatorKeySetTwo|
//@[74:75)    LeftSquare |[|
//@[75:87)    StringSyntax
//@[75:87)     StringComplete |'properties'|
//@[87:88)    RightSquare |]|
//@[88:89)   Dot |.|
//@[89:90)   IdentifierSyntax
//@[89:90)    Identifier |a|
//@[90:92) NewLine |\r\n|
// #completionTest(90) -> powershellPropertyAccess
//@[50:52) NewLine |\r\n|
var discriminatorKeySetTwoCompletionsArrayIndexer2 = discriminatorKeySetTwo['properties'].
//@[0:90) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:50)  IdentifierSyntax
//@[4:50)   Identifier |discriminatorKeySetTwoCompletionsArrayIndexer2|
//@[51:52)  Assignment |=|
//@[53:90)  PropertyAccessSyntax
//@[53:89)   ArrayAccessSyntax
//@[53:75)    VariableAccessSyntax
//@[53:75)     IdentifierSyntax
//@[53:75)      Identifier |discriminatorKeySetTwo|
//@[75:76)    LeftSquare |[|
//@[76:88)    StringSyntax
//@[76:88)     StringComplete |'properties'|
//@[88:89)    RightSquare |]|
//@[89:90)   Dot |.|
//@[90:90)   IdentifierSyntax
//@[90:90)    SkippedTriviaSyntax
//@[90:94) NewLine |\r\n\r\n|

/*
Discriminator value set 2 (conditional)
*/
//@[2:4) NewLine |\r\n|
resource discriminatorKeySetTwo_if 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
//@[0:273) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:34)  IdentifierSyntax
//@[9:34)   Identifier |discriminatorKeySetTwo_if|
//@[35:85)  StringSyntax
//@[35:85)   StringComplete |'Microsoft.Resources/deploymentScripts@2020-10-01'|
//@[86:87)  Assignment |=|
//@[88:273)  ObjectSyntax
//@[88:89)   LeftBrace |{|
//@[89:91)   NewLine |\r\n|
  kind: 'AzurePowerShell'
//@[2:25)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |kind|
//@[6:7)    Colon |:|
//@[8:25)    StringSyntax
//@[8:25)     StringComplete |'AzurePowerShell'|
//@[25:27)   NewLine |\r\n|
  // #completionTest(0,1,2) -> deploymentScriptTopLevel
//@[55:59)   NewLine |\r\n\r\n|

  properties: {
//@[2:93)   ObjectPropertySyntax
//@[2:12)    IdentifierSyntax
//@[2:12)     Identifier |properties|
//@[12:13)    Colon |:|
//@[14:93)    ObjectSyntax
//@[14:15)     LeftBrace |{|
//@[15:17)     NewLine |\r\n|
    // #completionTest(0,1,2,3,4) -> deploymentScriptPSProperties
//@[65:67)     NewLine |\r\n|
    
//@[4:6)     NewLine |\r\n|
  }
//@[2:3)     RightBrace |}|
//@[3:5)   NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\r\n|
// #completionTest(81) -> powershellPropertyAccess
//@[50:52) NewLine |\r\n|
var discriminatorKeySetTwoCompletions_if = discriminatorKeySetTwo_if.properties.a
//@[0:81) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:40)  IdentifierSyntax
//@[4:40)   Identifier |discriminatorKeySetTwoCompletions_if|
//@[41:42)  Assignment |=|
//@[43:81)  PropertyAccessSyntax
//@[43:79)   PropertyAccessSyntax
//@[43:68)    VariableAccessSyntax
//@[43:68)     IdentifierSyntax
//@[43:68)      Identifier |discriminatorKeySetTwo_if|
//@[68:69)    Dot |.|
//@[69:79)    IdentifierSyntax
//@[69:79)     Identifier |properties|
//@[79:80)   Dot |.|
//@[80:81)   IdentifierSyntax
//@[80:81)    Identifier |a|
//@[81:83) NewLine |\r\n|
// #completionTest(81) -> powershellPropertyAccess
//@[50:52) NewLine |\r\n|
var discriminatorKeySetTwoCompletions2_if = discriminatorKeySetTwo_if.properties.
//@[0:81) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:41)  IdentifierSyntax
//@[4:41)   Identifier |discriminatorKeySetTwoCompletions2_if|
//@[42:43)  Assignment |=|
//@[44:81)  PropertyAccessSyntax
//@[44:80)   PropertyAccessSyntax
//@[44:69)    VariableAccessSyntax
//@[44:69)     IdentifierSyntax
//@[44:69)      Identifier |discriminatorKeySetTwo_if|
//@[69:70)    Dot |.|
//@[70:80)    IdentifierSyntax
//@[70:80)     Identifier |properties|
//@[80:81)   Dot |.|
//@[81:81)   IdentifierSyntax
//@[81:81)    SkippedTriviaSyntax
//@[81:85) NewLine |\r\n\r\n|

// #completionTest(96) -> powershellPropertyAccess
//@[50:52) NewLine |\r\n|
var discriminatorKeySetTwoCompletionsArrayIndexer_if = discriminatorKeySetTwo_if['properties'].a
//@[0:96) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:52)  IdentifierSyntax
//@[4:52)   Identifier |discriminatorKeySetTwoCompletionsArrayIndexer_if|
//@[53:54)  Assignment |=|
//@[55:96)  PropertyAccessSyntax
//@[55:94)   ArrayAccessSyntax
//@[55:80)    VariableAccessSyntax
//@[55:80)     IdentifierSyntax
//@[55:80)      Identifier |discriminatorKeySetTwo_if|
//@[80:81)    LeftSquare |[|
//@[81:93)    StringSyntax
//@[81:93)     StringComplete |'properties'|
//@[93:94)    RightSquare |]|
//@[94:95)   Dot |.|
//@[95:96)   IdentifierSyntax
//@[95:96)    Identifier |a|
//@[96:98) NewLine |\r\n|
// #completionTest(96) -> powershellPropertyAccess
//@[50:52) NewLine |\r\n|
var discriminatorKeySetTwoCompletionsArrayIndexer2_if = discriminatorKeySetTwo_if['properties'].
//@[0:96) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:53)  IdentifierSyntax
//@[4:53)   Identifier |discriminatorKeySetTwoCompletionsArrayIndexer2_if|
//@[54:55)  Assignment |=|
//@[56:96)  PropertyAccessSyntax
//@[56:95)   ArrayAccessSyntax
//@[56:81)    VariableAccessSyntax
//@[56:81)     IdentifierSyntax
//@[56:81)      Identifier |discriminatorKeySetTwo_if|
//@[81:82)    LeftSquare |[|
//@[82:94)    StringSyntax
//@[82:94)     StringComplete |'properties'|
//@[94:95)    RightSquare |]|
//@[95:96)   Dot |.|
//@[96:96)   IdentifierSyntax
//@[96:96)    SkippedTriviaSyntax
//@[96:102) NewLine |\r\n\r\n\r\n|


/*
Discriminator value set 2 (loops)
*/
//@[2:4) NewLine |\r\n|
resource discriminatorKeySetTwo_for 'Microsoft.Resources/deploymentScripts@2020-10-01' = [for thing in []: {
//@[0:293) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:35)  IdentifierSyntax
//@[9:35)   Identifier |discriminatorKeySetTwo_for|
//@[36:86)  StringSyntax
//@[36:86)   StringComplete |'Microsoft.Resources/deploymentScripts@2020-10-01'|
//@[87:88)  Assignment |=|
//@[89:293)  ForSyntax
//@[89:90)   LeftSquare |[|
//@[90:93)   Identifier |for|
//@[94:99)   LocalVariableSyntax
//@[94:99)    IdentifierSyntax
//@[94:99)     Identifier |thing|
//@[100:102)   Identifier |in|
//@[103:105)   ArraySyntax
//@[103:104)    LeftSquare |[|
//@[104:105)    RightSquare |]|
//@[105:106)   Colon |:|
//@[107:292)   ObjectSyntax
//@[107:108)    LeftBrace |{|
//@[108:110)    NewLine |\r\n|
  kind: 'AzurePowerShell'
//@[2:25)    ObjectPropertySyntax
//@[2:6)     IdentifierSyntax
//@[2:6)      Identifier |kind|
//@[6:7)     Colon |:|
//@[8:25)     StringSyntax
//@[8:25)      StringComplete |'AzurePowerShell'|
//@[25:27)    NewLine |\r\n|
  // #completionTest(0,1,2) -> deploymentScriptTopLevel
//@[55:59)    NewLine |\r\n\r\n|

  properties: {
//@[2:93)    ObjectPropertySyntax
//@[2:12)     IdentifierSyntax
//@[2:12)      Identifier |properties|
//@[12:13)     Colon |:|
//@[14:93)     ObjectSyntax
//@[14:15)      LeftBrace |{|
//@[15:17)      NewLine |\r\n|
    // #completionTest(0,1,2,3,4) -> deploymentScriptPSProperties
//@[65:67)      NewLine |\r\n|
    
//@[4:6)      NewLine |\r\n|
  }
//@[2:3)      RightBrace |}|
//@[3:5)    NewLine |\r\n|
}]
//@[0:1)    RightBrace |}|
//@[1:2)   RightSquare |]|
//@[2:4) NewLine |\r\n|
// #completionTest(86) -> powershellPropertyAccess
//@[50:52) NewLine |\r\n|
var discriminatorKeySetTwoCompletions_for = discriminatorKeySetTwo_for[0].properties.a
//@[0:86) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:41)  IdentifierSyntax
//@[4:41)   Identifier |discriminatorKeySetTwoCompletions_for|
//@[42:43)  Assignment |=|
//@[44:86)  PropertyAccessSyntax
//@[44:84)   PropertyAccessSyntax
//@[44:73)    ArrayAccessSyntax
//@[44:70)     VariableAccessSyntax
//@[44:70)      IdentifierSyntax
//@[44:70)       Identifier |discriminatorKeySetTwo_for|
//@[70:71)     LeftSquare |[|
//@[71:72)     IntegerLiteralSyntax
//@[71:72)      Integer |0|
//@[72:73)     RightSquare |]|
//@[73:74)    Dot |.|
//@[74:84)    IdentifierSyntax
//@[74:84)     Identifier |properties|
//@[84:85)   Dot |.|
//@[85:86)   IdentifierSyntax
//@[85:86)    Identifier |a|
//@[86:88) NewLine |\r\n|
// #completionTest(86) -> powershellPropertyAccess
//@[50:52) NewLine |\r\n|
var discriminatorKeySetTwoCompletions2_for = discriminatorKeySetTwo_for[0].properties.
//@[0:86) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:42)  IdentifierSyntax
//@[4:42)   Identifier |discriminatorKeySetTwoCompletions2_for|
//@[43:44)  Assignment |=|
//@[45:86)  PropertyAccessSyntax
//@[45:85)   PropertyAccessSyntax
//@[45:74)    ArrayAccessSyntax
//@[45:71)     VariableAccessSyntax
//@[45:71)      IdentifierSyntax
//@[45:71)       Identifier |discriminatorKeySetTwo_for|
//@[71:72)     LeftSquare |[|
//@[72:73)     IntegerLiteralSyntax
//@[72:73)      Integer |0|
//@[73:74)     RightSquare |]|
//@[74:75)    Dot |.|
//@[75:85)    IdentifierSyntax
//@[75:85)     Identifier |properties|
//@[85:86)   Dot |.|
//@[86:86)   IdentifierSyntax
//@[86:86)    SkippedTriviaSyntax
//@[86:90) NewLine |\r\n\r\n|

// #completionTest(101) -> powershellPropertyAccess
//@[51:53) NewLine |\r\n|
var discriminatorKeySetTwoCompletionsArrayIndexer_for = discriminatorKeySetTwo_for[0]['properties'].a
//@[0:101) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:53)  IdentifierSyntax
//@[4:53)   Identifier |discriminatorKeySetTwoCompletionsArrayIndexer_for|
//@[54:55)  Assignment |=|
//@[56:101)  PropertyAccessSyntax
//@[56:99)   ArrayAccessSyntax
//@[56:85)    ArrayAccessSyntax
//@[56:82)     VariableAccessSyntax
//@[56:82)      IdentifierSyntax
//@[56:82)       Identifier |discriminatorKeySetTwo_for|
//@[82:83)     LeftSquare |[|
//@[83:84)     IntegerLiteralSyntax
//@[83:84)      Integer |0|
//@[84:85)     RightSquare |]|
//@[85:86)    LeftSquare |[|
//@[86:98)    StringSyntax
//@[86:98)     StringComplete |'properties'|
//@[98:99)    RightSquare |]|
//@[99:100)   Dot |.|
//@[100:101)   IdentifierSyntax
//@[100:101)    Identifier |a|
//@[101:103) NewLine |\r\n|
// #completionTest(101) -> powershellPropertyAccess
//@[51:53) NewLine |\r\n|
var discriminatorKeySetTwoCompletionsArrayIndexer2_for = discriminatorKeySetTwo_for[0]['properties'].
//@[0:101) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:54)  IdentifierSyntax
//@[4:54)   Identifier |discriminatorKeySetTwoCompletionsArrayIndexer2_for|
//@[55:56)  Assignment |=|
//@[57:101)  PropertyAccessSyntax
//@[57:100)   ArrayAccessSyntax
//@[57:86)    ArrayAccessSyntax
//@[57:83)     VariableAccessSyntax
//@[57:83)      IdentifierSyntax
//@[57:83)       Identifier |discriminatorKeySetTwo_for|
//@[83:84)     LeftSquare |[|
//@[84:85)     IntegerLiteralSyntax
//@[84:85)      Integer |0|
//@[85:86)     RightSquare |]|
//@[86:87)    LeftSquare |[|
//@[87:99)    StringSyntax
//@[87:99)     StringComplete |'properties'|
//@[99:100)    RightSquare |]|
//@[100:101)   Dot |.|
//@[101:101)   IdentifierSyntax
//@[101:101)    SkippedTriviaSyntax
//@[101:107) NewLine |\r\n\r\n\r\n|


/*
Discriminator value set 2 (filtered loops)
*/
//@[2:4) NewLine |\r\n|
resource discriminatorKeySetTwo_for_if 'Microsoft.Resources/deploymentScripts@2020-10-01' = [for thing in []: if(true) {
//@[0:305) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:38)  IdentifierSyntax
//@[9:38)   Identifier |discriminatorKeySetTwo_for_if|
//@[39:89)  StringSyntax
//@[39:89)   StringComplete |'Microsoft.Resources/deploymentScripts@2020-10-01'|
//@[90:91)  Assignment |=|
//@[92:305)  ForSyntax
//@[92:93)   LeftSquare |[|
//@[93:96)   Identifier |for|
//@[97:102)   LocalVariableSyntax
//@[97:102)    IdentifierSyntax
//@[97:102)     Identifier |thing|
//@[103:105)   Identifier |in|
//@[106:108)   ArraySyntax
//@[106:107)    LeftSquare |[|
//@[107:108)    RightSquare |]|
//@[108:109)   Colon |:|
//@[110:304)   IfConditionSyntax
//@[110:112)    Identifier |if|
//@[112:118)    ParenthesizedExpressionSyntax
//@[112:113)     LeftParen |(|
//@[113:117)     BooleanLiteralSyntax
//@[113:117)      TrueKeyword |true|
//@[117:118)     RightParen |)|
//@[119:304)    ObjectSyntax
//@[119:120)     LeftBrace |{|
//@[120:122)     NewLine |\r\n|
  kind: 'AzurePowerShell'
//@[2:25)     ObjectPropertySyntax
//@[2:6)      IdentifierSyntax
//@[2:6)       Identifier |kind|
//@[6:7)      Colon |:|
//@[8:25)      StringSyntax
//@[8:25)       StringComplete |'AzurePowerShell'|
//@[25:27)     NewLine |\r\n|
  // #completionTest(0,1,2) -> deploymentScriptTopLevel
//@[55:59)     NewLine |\r\n\r\n|

  properties: {
//@[2:93)     ObjectPropertySyntax
//@[2:12)      IdentifierSyntax
//@[2:12)       Identifier |properties|
//@[12:13)      Colon |:|
//@[14:93)      ObjectSyntax
//@[14:15)       LeftBrace |{|
//@[15:17)       NewLine |\r\n|
    // #completionTest(0,1,2,3,4) -> deploymentScriptPSProperties
//@[65:67)       NewLine |\r\n|
    
//@[4:6)       NewLine |\r\n|
  }
//@[2:3)       RightBrace |}|
//@[3:5)     NewLine |\r\n|
}]
//@[0:1)     RightBrace |}|
//@[1:2)   RightSquare |]|
//@[2:4) NewLine |\r\n|
// #completionTest(92) -> powershellPropertyAccess
//@[50:52) NewLine |\r\n|
var discriminatorKeySetTwoCompletions_for_if = discriminatorKeySetTwo_for_if[0].properties.a
//@[0:92) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:44)  IdentifierSyntax
//@[4:44)   Identifier |discriminatorKeySetTwoCompletions_for_if|
//@[45:46)  Assignment |=|
//@[47:92)  PropertyAccessSyntax
//@[47:90)   PropertyAccessSyntax
//@[47:79)    ArrayAccessSyntax
//@[47:76)     VariableAccessSyntax
//@[47:76)      IdentifierSyntax
//@[47:76)       Identifier |discriminatorKeySetTwo_for_if|
//@[76:77)     LeftSquare |[|
//@[77:78)     IntegerLiteralSyntax
//@[77:78)      Integer |0|
//@[78:79)     RightSquare |]|
//@[79:80)    Dot |.|
//@[80:90)    IdentifierSyntax
//@[80:90)     Identifier |properties|
//@[90:91)   Dot |.|
//@[91:92)   IdentifierSyntax
//@[91:92)    Identifier |a|
//@[92:94) NewLine |\r\n|
// #completionTest(92) -> powershellPropertyAccess
//@[50:52) NewLine |\r\n|
var discriminatorKeySetTwoCompletions2_for_if = discriminatorKeySetTwo_for_if[0].properties.
//@[0:92) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:45)  IdentifierSyntax
//@[4:45)   Identifier |discriminatorKeySetTwoCompletions2_for_if|
//@[46:47)  Assignment |=|
//@[48:92)  PropertyAccessSyntax
//@[48:91)   PropertyAccessSyntax
//@[48:80)    ArrayAccessSyntax
//@[48:77)     VariableAccessSyntax
//@[48:77)      IdentifierSyntax
//@[48:77)       Identifier |discriminatorKeySetTwo_for_if|
//@[77:78)     LeftSquare |[|
//@[78:79)     IntegerLiteralSyntax
//@[78:79)      Integer |0|
//@[79:80)     RightSquare |]|
//@[80:81)    Dot |.|
//@[81:91)    IdentifierSyntax
//@[81:91)     Identifier |properties|
//@[91:92)   Dot |.|
//@[92:92)   IdentifierSyntax
//@[92:92)    SkippedTriviaSyntax
//@[92:96) NewLine |\r\n\r\n|

// #completionTest(107) -> powershellPropertyAccess
//@[51:53) NewLine |\r\n|
var discriminatorKeySetTwoCompletionsArrayIndexer_for_if = discriminatorKeySetTwo_for_if[0]['properties'].a
//@[0:107) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:56)  IdentifierSyntax
//@[4:56)   Identifier |discriminatorKeySetTwoCompletionsArrayIndexer_for_if|
//@[57:58)  Assignment |=|
//@[59:107)  PropertyAccessSyntax
//@[59:105)   ArrayAccessSyntax
//@[59:91)    ArrayAccessSyntax
//@[59:88)     VariableAccessSyntax
//@[59:88)      IdentifierSyntax
//@[59:88)       Identifier |discriminatorKeySetTwo_for_if|
//@[88:89)     LeftSquare |[|
//@[89:90)     IntegerLiteralSyntax
//@[89:90)      Integer |0|
//@[90:91)     RightSquare |]|
//@[91:92)    LeftSquare |[|
//@[92:104)    StringSyntax
//@[92:104)     StringComplete |'properties'|
//@[104:105)    RightSquare |]|
//@[105:106)   Dot |.|
//@[106:107)   IdentifierSyntax
//@[106:107)    Identifier |a|
//@[107:109) NewLine |\r\n|
// #completionTest(107) -> powershellPropertyAccess
//@[51:53) NewLine |\r\n|
var discriminatorKeySetTwoCompletionsArrayIndexer2_for_if = discriminatorKeySetTwo_for_if[0]['properties'].
//@[0:107) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:57)  IdentifierSyntax
//@[4:57)   Identifier |discriminatorKeySetTwoCompletionsArrayIndexer2_for_if|
//@[58:59)  Assignment |=|
//@[60:107)  PropertyAccessSyntax
//@[60:106)   ArrayAccessSyntax
//@[60:92)    ArrayAccessSyntax
//@[60:89)     VariableAccessSyntax
//@[60:89)      IdentifierSyntax
//@[60:89)       Identifier |discriminatorKeySetTwo_for_if|
//@[89:90)     LeftSquare |[|
//@[90:91)     IntegerLiteralSyntax
//@[90:91)      Integer |0|
//@[91:92)     RightSquare |]|
//@[92:93)    LeftSquare |[|
//@[93:105)    StringSyntax
//@[93:105)     StringComplete |'properties'|
//@[105:106)    RightSquare |]|
//@[106:107)   Dot |.|
//@[107:107)   IdentifierSyntax
//@[107:107)    SkippedTriviaSyntax
//@[107:115) NewLine |\r\n\r\n\r\n\r\n|



resource incorrectPropertiesKey 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
//@[0:132) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:31)  IdentifierSyntax
//@[9:31)   Identifier |incorrectPropertiesKey|
//@[32:82)  StringSyntax
//@[32:82)   StringComplete |'Microsoft.Resources/deploymentScripts@2020-10-01'|
//@[83:84)  Assignment |=|
//@[85:132)  ObjectSyntax
//@[85:86)   LeftBrace |{|
//@[86:88)   NewLine |\r\n|
  kind: 'AzureCLI'
//@[2:18)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |kind|
//@[6:7)    Colon |:|
//@[8:18)    StringSyntax
//@[8:18)     StringComplete |'AzureCLI'|
//@[18:22)   NewLine |\r\n\r\n|

  propertes: {
//@[2:19)   ObjectPropertySyntax
//@[2:11)    IdentifierSyntax
//@[2:11)     Identifier |propertes|
//@[11:12)    Colon |:|
//@[13:19)    ObjectSyntax
//@[13:14)     LeftBrace |{|
//@[14:16)     NewLine |\r\n|
  }
//@[2:3)     RightBrace |}|
//@[3:5)   NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

var mock = incorrectPropertiesKey.p
//@[0:35) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:8)  IdentifierSyntax
//@[4:8)   Identifier |mock|
//@[9:10)  Assignment |=|
//@[11:35)  PropertyAccessSyntax
//@[11:33)   VariableAccessSyntax
//@[11:33)    IdentifierSyntax
//@[11:33)     Identifier |incorrectPropertiesKey|
//@[33:34)   Dot |.|
//@[34:35)   IdentifierSyntax
//@[34:35)    Identifier |p|
//@[35:39) NewLine |\r\n\r\n|

resource incorrectPropertiesKey2 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
//@[0:774) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:32)  IdentifierSyntax
//@[9:32)   Identifier |incorrectPropertiesKey2|
//@[33:83)  StringSyntax
//@[33:83)   StringComplete |'Microsoft.Resources/deploymentScripts@2020-10-01'|
//@[84:85)  Assignment |=|
//@[86:774)  ObjectSyntax
//@[86:87)   LeftBrace |{|
//@[87:89)   NewLine |\r\n|
  kind: 'AzureCLI'
//@[2:18)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |kind|
//@[6:7)    Colon |:|
//@[8:18)    StringSyntax
//@[8:18)     StringComplete |'AzureCLI'|
//@[18:20)   NewLine |\r\n|
  name: 'test'
//@[2:14)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:14)    StringSyntax
//@[8:14)     StringComplete |'test'|
//@[14:16)   NewLine |\r\n|
  location: ''
//@[2:14)   ObjectPropertySyntax
//@[2:10)    IdentifierSyntax
//@[2:10)     Identifier |location|
//@[10:11)    Colon |:|
//@[12:14)    StringSyntax
//@[12:14)     StringComplete |''|
//@[14:16)   NewLine |\r\n|
  properties: {
//@[2:630)   ObjectPropertySyntax
//@[2:12)    IdentifierSyntax
//@[2:12)     Identifier |properties|
//@[12:13)    Colon |:|
//@[14:630)    ObjectSyntax
//@[14:15)     LeftBrace |{|
//@[15:17)     NewLine |\r\n|
    azCliVersion: '2'
//@[4:21)     ObjectPropertySyntax
//@[4:16)      IdentifierSyntax
//@[4:16)       Identifier |azCliVersion|
//@[16:17)      Colon |:|
//@[18:21)      StringSyntax
//@[18:21)       StringComplete |'2'|
//@[21:23)     NewLine |\r\n|
    retentionInterval: 'PT1H'
//@[4:29)     ObjectPropertySyntax
//@[4:21)      IdentifierSyntax
//@[4:21)       Identifier |retentionInterval|
//@[21:22)      Colon |:|
//@[23:29)      StringSyntax
//@[23:29)       StringComplete |'PT1H'|
//@[29:31)     NewLine |\r\n|
    
//@[4:6)     NewLine |\r\n|
    // #completionTest(0,1,2,3,4) -> deploymentScriptCliPropertiesMinusSpecified
//@[80:82)     NewLine |\r\n|
    
//@[4:6)     NewLine |\r\n|
    // #completionTest(22,23) -> cleanupPreferencesPlusSymbols
//@[62:64)     NewLine |\r\n|
    cleanupPreference: 
//@[4:23)     ObjectPropertySyntax
//@[4:21)      IdentifierSyntax
//@[4:21)       Identifier |cleanupPreference|
//@[21:22)      Colon |:|
//@[23:23)      SkippedTriviaSyntax
//@[23:27)     NewLine |\r\n\r\n|

    // #completionTest(25,26) -> arrayPlusSymbols
//@[49:51)     NewLine |\r\n|
    supportingScriptUris: 
//@[4:26)     ObjectPropertySyntax
//@[4:24)      IdentifierSyntax
//@[4:24)       Identifier |supportingScriptUris|
//@[24:25)      Colon |:|
//@[26:26)      SkippedTriviaSyntax
//@[26:30)     NewLine |\r\n\r\n|

    // #completionTest(27,28) -> objectPlusSymbols
//@[50:52)     NewLine |\r\n|
    storageAccountSettings: 
//@[4:28)     ObjectPropertySyntax
//@[4:26)      IdentifierSyntax
//@[4:26)       Identifier |storageAccountSettings|
//@[26:27)      Colon |:|
//@[28:28)      SkippedTriviaSyntax
//@[28:32)     NewLine |\r\n\r\n|

    environmentVariables: [
//@[4:204)     ObjectPropertySyntax
//@[4:24)      IdentifierSyntax
//@[4:24)       Identifier |environmentVariables|
//@[24:25)      Colon |:|
//@[26:204)      ArraySyntax
//@[26:27)       LeftSquare |[|
//@[27:29)       NewLine |\r\n|
      {
//@[6:98)       ArrayItemSyntax
//@[6:98)        ObjectSyntax
//@[6:7)         LeftBrace |{|
//@[7:9)         NewLine |\r\n|
        // #completionTest(0,2,4,6,8) -> environmentVariableProperties
//@[70:72)         NewLine |\r\n|
        
//@[8:10)         NewLine |\r\n|
      }
//@[6:7)         RightBrace |}|
//@[7:9)       NewLine |\r\n|
      // #completionTest(0,1,2,3,4,5,6) -> objectPlusSymbols
//@[60:62)       NewLine |\r\n|
      
//@[6:8)       NewLine |\r\n|
    ]
//@[4:5)       RightSquare |]|
//@[5:7)     NewLine |\r\n|
  }
//@[2:3)     RightBrace |}|
//@[3:5)   NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

// #completionTest(21) -> resourceTypes
//@[39:41) NewLine |\r\n|
resource missingType 
//@[0:21) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:20)  IdentifierSyntax
//@[9:20)   Identifier |missingType|
//@[21:21)  SkippedTriviaSyntax
//@[21:21)  SkippedTriviaSyntax
//@[21:21)  SkippedTriviaSyntax
//@[21:25) NewLine |\r\n\r\n|

// #completionTest(37,38,39,40,41,42,43,44) -> resourceTypes
//@[60:62) NewLine |\r\n|
resource startedTypingTypeWithQuotes 'virma'
//@[0:44) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:36)  IdentifierSyntax
//@[9:36)   Identifier |startedTypingTypeWithQuotes|
//@[37:44)  StringSyntax
//@[37:44)   StringComplete |'virma'|
//@[44:44)  SkippedTriviaSyntax
//@[44:44)  SkippedTriviaSyntax
//@[44:48) NewLine |\r\n\r\n|

// #completionTest(40,41,42,43,44,45) -> resourceTypes
//@[54:56) NewLine |\r\n|
resource startedTypingTypeWithoutQuotes virma
//@[0:45) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:39)  IdentifierSyntax
//@[9:39)   Identifier |startedTypingTypeWithoutQuotes|
//@[40:45)  SkippedTriviaSyntax
//@[40:45)   Identifier |virma|
//@[45:45)  SkippedTriviaSyntax
//@[45:45)  SkippedTriviaSyntax
//@[45:49) NewLine |\r\n\r\n|

resource dashesInPropertyNames 'Microsoft.ContainerService/managedClusters@2020-09-01' = {
//@[0:93) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:30)  IdentifierSyntax
//@[9:30)   Identifier |dashesInPropertyNames|
//@[31:86)  StringSyntax
//@[31:86)   StringComplete |'Microsoft.ContainerService/managedClusters@2020-09-01'|
//@[87:88)  Assignment |=|
//@[89:93)  ObjectSyntax
//@[89:90)   LeftBrace |{|
//@[90:92)   NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\r\n|
// #completionTest(78) -> autoScalerPropertiesRequireEscaping
//@[61:63) NewLine |\r\n|
var letsAccessTheDashes = dashesInPropertyNames.properties.autoScalerProfile.s
//@[0:78) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:23)  IdentifierSyntax
//@[4:23)   Identifier |letsAccessTheDashes|
//@[24:25)  Assignment |=|
//@[26:78)  PropertyAccessSyntax
//@[26:76)   PropertyAccessSyntax
//@[26:58)    PropertyAccessSyntax
//@[26:47)     VariableAccessSyntax
//@[26:47)      IdentifierSyntax
//@[26:47)       Identifier |dashesInPropertyNames|
//@[47:48)     Dot |.|
//@[48:58)     IdentifierSyntax
//@[48:58)      Identifier |properties|
//@[58:59)    Dot |.|
//@[59:76)    IdentifierSyntax
//@[59:76)     Identifier |autoScalerProfile|
//@[76:77)   Dot |.|
//@[77:78)   IdentifierSyntax
//@[77:78)    Identifier |s|
//@[78:80) NewLine |\r\n|
// #completionTest(78) -> autoScalerPropertiesRequireEscaping
//@[61:63) NewLine |\r\n|
var letsAccessTheDashes2 = dashesInPropertyNames.properties.autoScalerProfile.
//@[0:78) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:24)  IdentifierSyntax
//@[4:24)   Identifier |letsAccessTheDashes2|
//@[25:26)  Assignment |=|
//@[27:78)  PropertyAccessSyntax
//@[27:77)   PropertyAccessSyntax
//@[27:59)    PropertyAccessSyntax
//@[27:48)     VariableAccessSyntax
//@[27:48)      IdentifierSyntax
//@[27:48)       Identifier |dashesInPropertyNames|
//@[48:49)     Dot |.|
//@[49:59)     IdentifierSyntax
//@[49:59)      Identifier |properties|
//@[59:60)    Dot |.|
//@[60:77)    IdentifierSyntax
//@[60:77)     Identifier |autoScalerProfile|
//@[77:78)   Dot |.|
//@[78:78)   IdentifierSyntax
//@[78:78)    SkippedTriviaSyntax
//@[78:82) NewLine |\r\n\r\n|

/* 
Nested discriminator missing key
*/
//@[2:4) NewLine |\r\n|
resource nestedDiscriminatorMissingKey 'Microsoft.DocumentDB/databaseAccounts@2020-06-01-preview' = {
//@[0:190) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:38)  IdentifierSyntax
//@[9:38)   Identifier |nestedDiscriminatorMissingKey|
//@[39:97)  StringSyntax
//@[39:97)   StringComplete |'Microsoft.DocumentDB/databaseAccounts@2020-06-01-preview'|
//@[98:99)  Assignment |=|
//@[100:190)  ObjectSyntax
//@[100:101)   LeftBrace |{|
//@[101:103)   NewLine |\r\n|
  name: 'test'
//@[2:14)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:14)    StringSyntax
//@[8:14)     StringComplete |'test'|
//@[14:16)   NewLine |\r\n|
  location: 'l'
//@[2:15)   ObjectPropertySyntax
//@[2:10)    IdentifierSyntax
//@[2:10)     Identifier |location|
//@[10:11)    Colon |:|
//@[12:15)    StringSyntax
//@[12:15)     StringComplete |'l'|
//@[15:17)   NewLine |\r\n|
  properties: {
//@[2:51)   ObjectPropertySyntax
//@[2:12)    IdentifierSyntax
//@[2:12)     Identifier |properties|
//@[12:13)    Colon |:|
//@[14:51)    ObjectSyntax
//@[14:15)     LeftBrace |{|
//@[15:17)     NewLine |\r\n|
    //createMode: 'Default'
//@[27:31)     NewLine |\r\n\r\n|

  }
//@[2:3)     RightBrace |}|
//@[3:5)   NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\r\n|
// #completionTest(90) -> createMode
//@[36:38) NewLine |\r\n|
var nestedDiscriminatorMissingKeyCompletions = nestedDiscriminatorMissingKey.properties.cr
//@[0:90) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:44)  IdentifierSyntax
//@[4:44)   Identifier |nestedDiscriminatorMissingKeyCompletions|
//@[45:46)  Assignment |=|
//@[47:90)  PropertyAccessSyntax
//@[47:87)   PropertyAccessSyntax
//@[47:76)    VariableAccessSyntax
//@[47:76)     IdentifierSyntax
//@[47:76)      Identifier |nestedDiscriminatorMissingKey|
//@[76:77)    Dot |.|
//@[77:87)    IdentifierSyntax
//@[77:87)     Identifier |properties|
//@[87:88)   Dot |.|
//@[88:90)   IdentifierSyntax
//@[88:90)    Identifier |cr|
//@[90:92) NewLine |\r\n|
// #completionTest(92) -> createMode
//@[36:38) NewLine |\r\n|
var nestedDiscriminatorMissingKeyCompletions2 = nestedDiscriminatorMissingKey['properties'].
//@[0:92) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:45)  IdentifierSyntax
//@[4:45)   Identifier |nestedDiscriminatorMissingKeyCompletions2|
//@[46:47)  Assignment |=|
//@[48:92)  PropertyAccessSyntax
//@[48:91)   ArrayAccessSyntax
//@[48:77)    VariableAccessSyntax
//@[48:77)     IdentifierSyntax
//@[48:77)      Identifier |nestedDiscriminatorMissingKey|
//@[77:78)    LeftSquare |[|
//@[78:90)    StringSyntax
//@[78:90)     StringComplete |'properties'|
//@[90:91)    RightSquare |]|
//@[91:92)   Dot |.|
//@[92:92)   IdentifierSyntax
//@[92:92)    SkippedTriviaSyntax
//@[92:96) NewLine |\r\n\r\n|

// #completionTest(94) -> createModeIndexPlusSymbols
//@[52:54) NewLine |\r\n|
var nestedDiscriminatorMissingKeyIndexCompletions = nestedDiscriminatorMissingKey.properties['']
//@[0:96) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:49)  IdentifierSyntax
//@[4:49)   Identifier |nestedDiscriminatorMissingKeyIndexCompletions|
//@[50:51)  Assignment |=|
//@[52:96)  ArrayAccessSyntax
//@[52:92)   PropertyAccessSyntax
//@[52:81)    VariableAccessSyntax
//@[52:81)     IdentifierSyntax
//@[52:81)      Identifier |nestedDiscriminatorMissingKey|
//@[81:82)    Dot |.|
//@[82:92)    IdentifierSyntax
//@[82:92)     Identifier |properties|
//@[92:93)   LeftSquare |[|
//@[93:95)   StringSyntax
//@[93:95)    StringComplete |''|
//@[95:96)   RightSquare |]|
//@[96:100) NewLine |\r\n\r\n|

/* 
Nested discriminator missing key (conditional)
*/
//@[2:4) NewLine |\r\n|
resource nestedDiscriminatorMissingKey_if 'Microsoft.DocumentDB/databaseAccounts@2020-06-01-preview' = if(bool(1)) {
//@[0:205) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:41)  IdentifierSyntax
//@[9:41)   Identifier |nestedDiscriminatorMissingKey_if|
//@[42:100)  StringSyntax
//@[42:100)   StringComplete |'Microsoft.DocumentDB/databaseAccounts@2020-06-01-preview'|
//@[101:102)  Assignment |=|
//@[103:205)  IfConditionSyntax
//@[103:105)   Identifier |if|
//@[105:114)   ParenthesizedExpressionSyntax
//@[105:106)    LeftParen |(|
//@[106:113)    FunctionCallSyntax
//@[106:110)     IdentifierSyntax
//@[106:110)      Identifier |bool|
//@[110:111)     LeftParen |(|
//@[111:112)     FunctionArgumentSyntax
//@[111:112)      IntegerLiteralSyntax
//@[111:112)       Integer |1|
//@[112:113)     RightParen |)|
//@[113:114)    RightParen |)|
//@[115:205)   ObjectSyntax
//@[115:116)    LeftBrace |{|
//@[116:118)    NewLine |\r\n|
  name: 'test'
//@[2:14)    ObjectPropertySyntax
//@[2:6)     IdentifierSyntax
//@[2:6)      Identifier |name|
//@[6:7)     Colon |:|
//@[8:14)     StringSyntax
//@[8:14)      StringComplete |'test'|
//@[14:16)    NewLine |\r\n|
  location: 'l'
//@[2:15)    ObjectPropertySyntax
//@[2:10)     IdentifierSyntax
//@[2:10)      Identifier |location|
//@[10:11)     Colon |:|
//@[12:15)     StringSyntax
//@[12:15)      StringComplete |'l'|
//@[15:17)    NewLine |\r\n|
  properties: {
//@[2:51)    ObjectPropertySyntax
//@[2:12)     IdentifierSyntax
//@[2:12)      Identifier |properties|
//@[12:13)     Colon |:|
//@[14:51)     ObjectSyntax
//@[14:15)      LeftBrace |{|
//@[15:17)      NewLine |\r\n|
    //createMode: 'Default'
//@[27:31)      NewLine |\r\n\r\n|

  }
//@[2:3)      RightBrace |}|
//@[3:5)    NewLine |\r\n|
}
//@[0:1)    RightBrace |}|
//@[1:3) NewLine |\r\n|
// #completionTest(96) -> createMode
//@[36:38) NewLine |\r\n|
var nestedDiscriminatorMissingKeyCompletions_if = nestedDiscriminatorMissingKey_if.properties.cr
//@[0:96) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:47)  IdentifierSyntax
//@[4:47)   Identifier |nestedDiscriminatorMissingKeyCompletions_if|
//@[48:49)  Assignment |=|
//@[50:96)  PropertyAccessSyntax
//@[50:93)   PropertyAccessSyntax
//@[50:82)    VariableAccessSyntax
//@[50:82)     IdentifierSyntax
//@[50:82)      Identifier |nestedDiscriminatorMissingKey_if|
//@[82:83)    Dot |.|
//@[83:93)    IdentifierSyntax
//@[83:93)     Identifier |properties|
//@[93:94)   Dot |.|
//@[94:96)   IdentifierSyntax
//@[94:96)    Identifier |cr|
//@[96:98) NewLine |\r\n|
// #completionTest(98) -> createMode
//@[36:38) NewLine |\r\n|
var nestedDiscriminatorMissingKeyCompletions2_if = nestedDiscriminatorMissingKey_if['properties'].
//@[0:98) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:48)  IdentifierSyntax
//@[4:48)   Identifier |nestedDiscriminatorMissingKeyCompletions2_if|
//@[49:50)  Assignment |=|
//@[51:98)  PropertyAccessSyntax
//@[51:97)   ArrayAccessSyntax
//@[51:83)    VariableAccessSyntax
//@[51:83)     IdentifierSyntax
//@[51:83)      Identifier |nestedDiscriminatorMissingKey_if|
//@[83:84)    LeftSquare |[|
//@[84:96)    StringSyntax
//@[84:96)     StringComplete |'properties'|
//@[96:97)    RightSquare |]|
//@[97:98)   Dot |.|
//@[98:98)   IdentifierSyntax
//@[98:98)    SkippedTriviaSyntax
//@[98:102) NewLine |\r\n\r\n|

// #completionTest(100) -> createModeIndexPlusSymbols_if
//@[56:58) NewLine |\r\n|
var nestedDiscriminatorMissingKeyIndexCompletions_if = nestedDiscriminatorMissingKey_if.properties['']
//@[0:102) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:52)  IdentifierSyntax
//@[4:52)   Identifier |nestedDiscriminatorMissingKeyIndexCompletions_if|
//@[53:54)  Assignment |=|
//@[55:102)  ArrayAccessSyntax
//@[55:98)   PropertyAccessSyntax
//@[55:87)    VariableAccessSyntax
//@[55:87)     IdentifierSyntax
//@[55:87)      Identifier |nestedDiscriminatorMissingKey_if|
//@[87:88)    Dot |.|
//@[88:98)    IdentifierSyntax
//@[88:98)     Identifier |properties|
//@[98:99)   LeftSquare |[|
//@[99:101)   StringSyntax
//@[99:101)    StringComplete |''|
//@[101:102)   RightSquare |]|
//@[102:106) NewLine |\r\n\r\n|

/* 
Nested discriminator missing key (loop)
*/
//@[2:4) NewLine |\r\n|
resource nestedDiscriminatorMissingKey_for 'Microsoft.DocumentDB/databaseAccounts@2020-06-01-preview' = [for thing in []: {
//@[0:213) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:42)  IdentifierSyntax
//@[9:42)   Identifier |nestedDiscriminatorMissingKey_for|
//@[43:101)  StringSyntax
//@[43:101)   StringComplete |'Microsoft.DocumentDB/databaseAccounts@2020-06-01-preview'|
//@[102:103)  Assignment |=|
//@[104:213)  ForSyntax
//@[104:105)   LeftSquare |[|
//@[105:108)   Identifier |for|
//@[109:114)   LocalVariableSyntax
//@[109:114)    IdentifierSyntax
//@[109:114)     Identifier |thing|
//@[115:117)   Identifier |in|
//@[118:120)   ArraySyntax
//@[118:119)    LeftSquare |[|
//@[119:120)    RightSquare |]|
//@[120:121)   Colon |:|
//@[122:212)   ObjectSyntax
//@[122:123)    LeftBrace |{|
//@[123:125)    NewLine |\r\n|
  name: 'test'
//@[2:14)    ObjectPropertySyntax
//@[2:6)     IdentifierSyntax
//@[2:6)      Identifier |name|
//@[6:7)     Colon |:|
//@[8:14)     StringSyntax
//@[8:14)      StringComplete |'test'|
//@[14:16)    NewLine |\r\n|
  location: 'l'
//@[2:15)    ObjectPropertySyntax
//@[2:10)     IdentifierSyntax
//@[2:10)      Identifier |location|
//@[10:11)     Colon |:|
//@[12:15)     StringSyntax
//@[12:15)      StringComplete |'l'|
//@[15:17)    NewLine |\r\n|
  properties: {
//@[2:51)    ObjectPropertySyntax
//@[2:12)     IdentifierSyntax
//@[2:12)      Identifier |properties|
//@[12:13)     Colon |:|
//@[14:51)     ObjectSyntax
//@[14:15)      LeftBrace |{|
//@[15:17)      NewLine |\r\n|
    //createMode: 'Default'
//@[27:31)      NewLine |\r\n\r\n|

  }
//@[2:3)      RightBrace |}|
//@[3:5)    NewLine |\r\n|
}]
//@[0:1)    RightBrace |}|
//@[1:2)   RightSquare |]|
//@[2:4) NewLine |\r\n|
// #completionTest(101) -> createMode
//@[37:39) NewLine |\r\n|
var nestedDiscriminatorMissingKeyCompletions_for = nestedDiscriminatorMissingKey_for[0].properties.cr
//@[0:101) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:48)  IdentifierSyntax
//@[4:48)   Identifier |nestedDiscriminatorMissingKeyCompletions_for|
//@[49:50)  Assignment |=|
//@[51:101)  PropertyAccessSyntax
//@[51:98)   PropertyAccessSyntax
//@[51:87)    ArrayAccessSyntax
//@[51:84)     VariableAccessSyntax
//@[51:84)      IdentifierSyntax
//@[51:84)       Identifier |nestedDiscriminatorMissingKey_for|
//@[84:85)     LeftSquare |[|
//@[85:86)     IntegerLiteralSyntax
//@[85:86)      Integer |0|
//@[86:87)     RightSquare |]|
//@[87:88)    Dot |.|
//@[88:98)    IdentifierSyntax
//@[88:98)     Identifier |properties|
//@[98:99)   Dot |.|
//@[99:101)   IdentifierSyntax
//@[99:101)    Identifier |cr|
//@[101:103) NewLine |\r\n|
// #completionTest(103) -> createMode
//@[37:39) NewLine |\r\n|
var nestedDiscriminatorMissingKeyCompletions2_for = nestedDiscriminatorMissingKey_for[0]['properties'].
//@[0:103) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:49)  IdentifierSyntax
//@[4:49)   Identifier |nestedDiscriminatorMissingKeyCompletions2_for|
//@[50:51)  Assignment |=|
//@[52:103)  PropertyAccessSyntax
//@[52:102)   ArrayAccessSyntax
//@[52:88)    ArrayAccessSyntax
//@[52:85)     VariableAccessSyntax
//@[52:85)      IdentifierSyntax
//@[52:85)       Identifier |nestedDiscriminatorMissingKey_for|
//@[85:86)     LeftSquare |[|
//@[86:87)     IntegerLiteralSyntax
//@[86:87)      Integer |0|
//@[87:88)     RightSquare |]|
//@[88:89)    LeftSquare |[|
//@[89:101)    StringSyntax
//@[89:101)     StringComplete |'properties'|
//@[101:102)    RightSquare |]|
//@[102:103)   Dot |.|
//@[103:103)   IdentifierSyntax
//@[103:103)    SkippedTriviaSyntax
//@[103:107) NewLine |\r\n\r\n|

// #completionTest(105) -> createModeIndexPlusSymbols_for
//@[57:59) NewLine |\r\n|
var nestedDiscriminatorMissingKeyIndexCompletions_for = nestedDiscriminatorMissingKey_for[0].properties['']
//@[0:107) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:53)  IdentifierSyntax
//@[4:53)   Identifier |nestedDiscriminatorMissingKeyIndexCompletions_for|
//@[54:55)  Assignment |=|
//@[56:107)  ArrayAccessSyntax
//@[56:103)   PropertyAccessSyntax
//@[56:92)    ArrayAccessSyntax
//@[56:89)     VariableAccessSyntax
//@[56:89)      IdentifierSyntax
//@[56:89)       Identifier |nestedDiscriminatorMissingKey_for|
//@[89:90)     LeftSquare |[|
//@[90:91)     IntegerLiteralSyntax
//@[90:91)      Integer |0|
//@[91:92)     RightSquare |]|
//@[92:93)    Dot |.|
//@[93:103)    IdentifierSyntax
//@[93:103)     Identifier |properties|
//@[103:104)   LeftSquare |[|
//@[104:106)   StringSyntax
//@[104:106)    StringComplete |''|
//@[106:107)   RightSquare |]|
//@[107:113) NewLine |\r\n\r\n\r\n|


/* 
Nested discriminator missing key (filtered loop)
*/
//@[2:4) NewLine |\r\n|
resource nestedDiscriminatorMissingKey_for_if 'Microsoft.DocumentDB/databaseAccounts@2020-06-01-preview' = [for thing in []: if(true) {
//@[0:225) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:45)  IdentifierSyntax
//@[9:45)   Identifier |nestedDiscriminatorMissingKey_for_if|
//@[46:104)  StringSyntax
//@[46:104)   StringComplete |'Microsoft.DocumentDB/databaseAccounts@2020-06-01-preview'|
//@[105:106)  Assignment |=|
//@[107:225)  ForSyntax
//@[107:108)   LeftSquare |[|
//@[108:111)   Identifier |for|
//@[112:117)   LocalVariableSyntax
//@[112:117)    IdentifierSyntax
//@[112:117)     Identifier |thing|
//@[118:120)   Identifier |in|
//@[121:123)   ArraySyntax
//@[121:122)    LeftSquare |[|
//@[122:123)    RightSquare |]|
//@[123:124)   Colon |:|
//@[125:224)   IfConditionSyntax
//@[125:127)    Identifier |if|
//@[127:133)    ParenthesizedExpressionSyntax
//@[127:128)     LeftParen |(|
//@[128:132)     BooleanLiteralSyntax
//@[128:132)      TrueKeyword |true|
//@[132:133)     RightParen |)|
//@[134:224)    ObjectSyntax
//@[134:135)     LeftBrace |{|
//@[135:137)     NewLine |\r\n|
  name: 'test'
//@[2:14)     ObjectPropertySyntax
//@[2:6)      IdentifierSyntax
//@[2:6)       Identifier |name|
//@[6:7)      Colon |:|
//@[8:14)      StringSyntax
//@[8:14)       StringComplete |'test'|
//@[14:16)     NewLine |\r\n|
  location: 'l'
//@[2:15)     ObjectPropertySyntax
//@[2:10)      IdentifierSyntax
//@[2:10)       Identifier |location|
//@[10:11)      Colon |:|
//@[12:15)      StringSyntax
//@[12:15)       StringComplete |'l'|
//@[15:17)     NewLine |\r\n|
  properties: {
//@[2:51)     ObjectPropertySyntax
//@[2:12)      IdentifierSyntax
//@[2:12)       Identifier |properties|
//@[12:13)      Colon |:|
//@[14:51)      ObjectSyntax
//@[14:15)       LeftBrace |{|
//@[15:17)       NewLine |\r\n|
    //createMode: 'Default'
//@[27:31)       NewLine |\r\n\r\n|

  }
//@[2:3)       RightBrace |}|
//@[3:5)     NewLine |\r\n|
}]
//@[0:1)     RightBrace |}|
//@[1:2)   RightSquare |]|
//@[2:4) NewLine |\r\n|
// #completionTest(107) -> createMode
//@[37:39) NewLine |\r\n|
var nestedDiscriminatorMissingKeyCompletions_for_if = nestedDiscriminatorMissingKey_for_if[0].properties.cr
//@[0:107) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:51)  IdentifierSyntax
//@[4:51)   Identifier |nestedDiscriminatorMissingKeyCompletions_for_if|
//@[52:53)  Assignment |=|
//@[54:107)  PropertyAccessSyntax
//@[54:104)   PropertyAccessSyntax
//@[54:93)    ArrayAccessSyntax
//@[54:90)     VariableAccessSyntax
//@[54:90)      IdentifierSyntax
//@[54:90)       Identifier |nestedDiscriminatorMissingKey_for_if|
//@[90:91)     LeftSquare |[|
//@[91:92)     IntegerLiteralSyntax
//@[91:92)      Integer |0|
//@[92:93)     RightSquare |]|
//@[93:94)    Dot |.|
//@[94:104)    IdentifierSyntax
//@[94:104)     Identifier |properties|
//@[104:105)   Dot |.|
//@[105:107)   IdentifierSyntax
//@[105:107)    Identifier |cr|
//@[107:109) NewLine |\r\n|
// #completionTest(109) -> createMode
//@[37:39) NewLine |\r\n|
var nestedDiscriminatorMissingKeyCompletions2_for_if = nestedDiscriminatorMissingKey_for_if[0]['properties'].
//@[0:109) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:52)  IdentifierSyntax
//@[4:52)   Identifier |nestedDiscriminatorMissingKeyCompletions2_for_if|
//@[53:54)  Assignment |=|
//@[55:109)  PropertyAccessSyntax
//@[55:108)   ArrayAccessSyntax
//@[55:94)    ArrayAccessSyntax
//@[55:91)     VariableAccessSyntax
//@[55:91)      IdentifierSyntax
//@[55:91)       Identifier |nestedDiscriminatorMissingKey_for_if|
//@[91:92)     LeftSquare |[|
//@[92:93)     IntegerLiteralSyntax
//@[92:93)      Integer |0|
//@[93:94)     RightSquare |]|
//@[94:95)    LeftSquare |[|
//@[95:107)    StringSyntax
//@[95:107)     StringComplete |'properties'|
//@[107:108)    RightSquare |]|
//@[108:109)   Dot |.|
//@[109:109)   IdentifierSyntax
//@[109:109)    SkippedTriviaSyntax
//@[109:113) NewLine |\r\n\r\n|

// #completionTest(111) -> createModeIndexPlusSymbols_for_if
//@[60:62) NewLine |\r\n|
var nestedDiscriminatorMissingKeyIndexCompletions_for_if = nestedDiscriminatorMissingKey_for_if[0].properties['']
//@[0:113) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:56)  IdentifierSyntax
//@[4:56)   Identifier |nestedDiscriminatorMissingKeyIndexCompletions_for_if|
//@[57:58)  Assignment |=|
//@[59:113)  ArrayAccessSyntax
//@[59:109)   PropertyAccessSyntax
//@[59:98)    ArrayAccessSyntax
//@[59:95)     VariableAccessSyntax
//@[59:95)      IdentifierSyntax
//@[59:95)       Identifier |nestedDiscriminatorMissingKey_for_if|
//@[95:96)     LeftSquare |[|
//@[96:97)     IntegerLiteralSyntax
//@[96:97)      Integer |0|
//@[97:98)     RightSquare |]|
//@[98:99)    Dot |.|
//@[99:109)    IdentifierSyntax
//@[99:109)     Identifier |properties|
//@[109:110)   LeftSquare |[|
//@[110:112)   StringSyntax
//@[110:112)    StringComplete |''|
//@[112:113)   RightSquare |]|
//@[113:119) NewLine |\r\n\r\n\r\n|


/*
Nested discriminator
*/
//@[2:4) NewLine |\r\n|
resource nestedDiscriminator 'Microsoft.DocumentDB/databaseAccounts@2020-06-01-preview' = {
//@[0:178) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:28)  IdentifierSyntax
//@[9:28)   Identifier |nestedDiscriminator|
//@[29:87)  StringSyntax
//@[29:87)   StringComplete |'Microsoft.DocumentDB/databaseAccounts@2020-06-01-preview'|
//@[88:89)  Assignment |=|
//@[90:178)  ObjectSyntax
//@[90:91)   LeftBrace |{|
//@[91:93)   NewLine |\r\n|
  name: 'test'
//@[2:14)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:14)    StringSyntax
//@[8:14)     StringComplete |'test'|
//@[14:16)   NewLine |\r\n|
  location: 'l'
//@[2:15)   ObjectPropertySyntax
//@[2:10)    IdentifierSyntax
//@[2:10)     Identifier |location|
//@[10:11)    Colon |:|
//@[12:15)    StringSyntax
//@[12:15)     StringComplete |'l'|
//@[15:17)   NewLine |\r\n|
  properties: {
//@[2:49)   ObjectPropertySyntax
//@[2:12)    IdentifierSyntax
//@[2:12)     Identifier |properties|
//@[12:13)    Colon |:|
//@[14:49)    ObjectSyntax
//@[14:15)     LeftBrace |{|
//@[15:17)     NewLine |\r\n|
    createMode: 'Default'
//@[4:25)     ObjectPropertySyntax
//@[4:14)      IdentifierSyntax
//@[4:14)       Identifier |createMode|
//@[14:15)      Colon |:|
//@[16:25)      StringSyntax
//@[16:25)       StringComplete |'Default'|
//@[25:29)     NewLine |\r\n\r\n|

  }
//@[2:3)     RightBrace |}|
//@[3:5)   NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\r\n|
// #completionTest(69) -> defaultCreateModeProperties
//@[53:55) NewLine |\r\n|
var nestedDiscriminatorCompletions = nestedDiscriminator.properties.a
//@[0:69) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:34)  IdentifierSyntax
//@[4:34)   Identifier |nestedDiscriminatorCompletions|
//@[35:36)  Assignment |=|
//@[37:69)  PropertyAccessSyntax
//@[37:67)   PropertyAccessSyntax
//@[37:56)    VariableAccessSyntax
//@[37:56)     IdentifierSyntax
//@[37:56)      Identifier |nestedDiscriminator|
//@[56:57)    Dot |.|
//@[57:67)    IdentifierSyntax
//@[57:67)     Identifier |properties|
//@[67:68)   Dot |.|
//@[68:69)   IdentifierSyntax
//@[68:69)    Identifier |a|
//@[69:71) NewLine |\r\n|
// #completionTest(73) -> defaultCreateModeProperties
//@[53:55) NewLine |\r\n|
var nestedDiscriminatorCompletions2 = nestedDiscriminator['properties'].a
//@[0:73) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:35)  IdentifierSyntax
//@[4:35)   Identifier |nestedDiscriminatorCompletions2|
//@[36:37)  Assignment |=|
//@[38:73)  PropertyAccessSyntax
//@[38:71)   ArrayAccessSyntax
//@[38:57)    VariableAccessSyntax
//@[38:57)     IdentifierSyntax
//@[38:57)      Identifier |nestedDiscriminator|
//@[57:58)    LeftSquare |[|
//@[58:70)    StringSyntax
//@[58:70)     StringComplete |'properties'|
//@[70:71)    RightSquare |]|
//@[71:72)   Dot |.|
//@[72:73)   IdentifierSyntax
//@[72:73)    Identifier |a|
//@[73:75) NewLine |\r\n|
// #completionTest(69) -> defaultCreateModeProperties
//@[53:55) NewLine |\r\n|
var nestedDiscriminatorCompletions3 = nestedDiscriminator.properties.
//@[0:69) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:35)  IdentifierSyntax
//@[4:35)   Identifier |nestedDiscriminatorCompletions3|
//@[36:37)  Assignment |=|
//@[38:69)  PropertyAccessSyntax
//@[38:68)   PropertyAccessSyntax
//@[38:57)    VariableAccessSyntax
//@[38:57)     IdentifierSyntax
//@[38:57)      Identifier |nestedDiscriminator|
//@[57:58)    Dot |.|
//@[58:68)    IdentifierSyntax
//@[58:68)     Identifier |properties|
//@[68:69)   Dot |.|
//@[69:69)   IdentifierSyntax
//@[69:69)    SkippedTriviaSyntax
//@[69:71) NewLine |\r\n|
// #completionTest(72) -> defaultCreateModeProperties
//@[53:55) NewLine |\r\n|
var nestedDiscriminatorCompletions4 = nestedDiscriminator['properties'].
//@[0:72) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:35)  IdentifierSyntax
//@[4:35)   Identifier |nestedDiscriminatorCompletions4|
//@[36:37)  Assignment |=|
//@[38:72)  PropertyAccessSyntax
//@[38:71)   ArrayAccessSyntax
//@[38:57)    VariableAccessSyntax
//@[38:57)     IdentifierSyntax
//@[38:57)      Identifier |nestedDiscriminator|
//@[57:58)    LeftSquare |[|
//@[58:70)    StringSyntax
//@[58:70)     StringComplete |'properties'|
//@[70:71)    RightSquare |]|
//@[71:72)   Dot |.|
//@[72:72)   IdentifierSyntax
//@[72:72)    SkippedTriviaSyntax
//@[72:76) NewLine |\r\n\r\n|

// #completionTest(79) -> defaultCreateModeIndexes
//@[50:52) NewLine |\r\n|
var nestedDiscriminatorArrayIndexCompletions = nestedDiscriminator.properties[a]
//@[0:80) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:44)  IdentifierSyntax
//@[4:44)   Identifier |nestedDiscriminatorArrayIndexCompletions|
//@[45:46)  Assignment |=|
//@[47:80)  ArrayAccessSyntax
//@[47:77)   PropertyAccessSyntax
//@[47:66)    VariableAccessSyntax
//@[47:66)     IdentifierSyntax
//@[47:66)      Identifier |nestedDiscriminator|
//@[66:67)    Dot |.|
//@[67:77)    IdentifierSyntax
//@[67:77)     Identifier |properties|
//@[77:78)   LeftSquare |[|
//@[78:79)   VariableAccessSyntax
//@[78:79)    IdentifierSyntax
//@[78:79)     Identifier |a|
//@[79:80)   RightSquare |]|
//@[80:84) NewLine |\r\n\r\n|

/*
Nested discriminator (conditional)
*/
//@[2:4) NewLine |\r\n|
resource nestedDiscriminator_if 'Microsoft.DocumentDB/databaseAccounts@2020-06-01-preview' = if(true) {
//@[0:190) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:31)  IdentifierSyntax
//@[9:31)   Identifier |nestedDiscriminator_if|
//@[32:90)  StringSyntax
//@[32:90)   StringComplete |'Microsoft.DocumentDB/databaseAccounts@2020-06-01-preview'|
//@[91:92)  Assignment |=|
//@[93:190)  IfConditionSyntax
//@[93:95)   Identifier |if|
//@[95:101)   ParenthesizedExpressionSyntax
//@[95:96)    LeftParen |(|
//@[96:100)    BooleanLiteralSyntax
//@[96:100)     TrueKeyword |true|
//@[100:101)    RightParen |)|
//@[102:190)   ObjectSyntax
//@[102:103)    LeftBrace |{|
//@[103:105)    NewLine |\r\n|
  name: 'test'
//@[2:14)    ObjectPropertySyntax
//@[2:6)     IdentifierSyntax
//@[2:6)      Identifier |name|
//@[6:7)     Colon |:|
//@[8:14)     StringSyntax
//@[8:14)      StringComplete |'test'|
//@[14:16)    NewLine |\r\n|
  location: 'l'
//@[2:15)    ObjectPropertySyntax
//@[2:10)     IdentifierSyntax
//@[2:10)      Identifier |location|
//@[10:11)     Colon |:|
//@[12:15)     StringSyntax
//@[12:15)      StringComplete |'l'|
//@[15:17)    NewLine |\r\n|
  properties: {
//@[2:49)    ObjectPropertySyntax
//@[2:12)     IdentifierSyntax
//@[2:12)      Identifier |properties|
//@[12:13)     Colon |:|
//@[14:49)     ObjectSyntax
//@[14:15)      LeftBrace |{|
//@[15:17)      NewLine |\r\n|
    createMode: 'Default'
//@[4:25)      ObjectPropertySyntax
//@[4:14)       IdentifierSyntax
//@[4:14)        Identifier |createMode|
//@[14:15)       Colon |:|
//@[16:25)       StringSyntax
//@[16:25)        StringComplete |'Default'|
//@[25:29)      NewLine |\r\n\r\n|

  }
//@[2:3)      RightBrace |}|
//@[3:5)    NewLine |\r\n|
}
//@[0:1)    RightBrace |}|
//@[1:3) NewLine |\r\n|
// #completionTest(75) -> defaultCreateModeProperties
//@[53:55) NewLine |\r\n|
var nestedDiscriminatorCompletions_if = nestedDiscriminator_if.properties.a
//@[0:75) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:37)  IdentifierSyntax
//@[4:37)   Identifier |nestedDiscriminatorCompletions_if|
//@[38:39)  Assignment |=|
//@[40:75)  PropertyAccessSyntax
//@[40:73)   PropertyAccessSyntax
//@[40:62)    VariableAccessSyntax
//@[40:62)     IdentifierSyntax
//@[40:62)      Identifier |nestedDiscriminator_if|
//@[62:63)    Dot |.|
//@[63:73)    IdentifierSyntax
//@[63:73)     Identifier |properties|
//@[73:74)   Dot |.|
//@[74:75)   IdentifierSyntax
//@[74:75)    Identifier |a|
//@[75:77) NewLine |\r\n|
// #completionTest(79) -> defaultCreateModeProperties
//@[53:55) NewLine |\r\n|
var nestedDiscriminatorCompletions2_if = nestedDiscriminator_if['properties'].a
//@[0:79) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:38)  IdentifierSyntax
//@[4:38)   Identifier |nestedDiscriminatorCompletions2_if|
//@[39:40)  Assignment |=|
//@[41:79)  PropertyAccessSyntax
//@[41:77)   ArrayAccessSyntax
//@[41:63)    VariableAccessSyntax
//@[41:63)     IdentifierSyntax
//@[41:63)      Identifier |nestedDiscriminator_if|
//@[63:64)    LeftSquare |[|
//@[64:76)    StringSyntax
//@[64:76)     StringComplete |'properties'|
//@[76:77)    RightSquare |]|
//@[77:78)   Dot |.|
//@[78:79)   IdentifierSyntax
//@[78:79)    Identifier |a|
//@[79:81) NewLine |\r\n|
// #completionTest(75) -> defaultCreateModeProperties
//@[53:55) NewLine |\r\n|
var nestedDiscriminatorCompletions3_if = nestedDiscriminator_if.properties.
//@[0:75) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:38)  IdentifierSyntax
//@[4:38)   Identifier |nestedDiscriminatorCompletions3_if|
//@[39:40)  Assignment |=|
//@[41:75)  PropertyAccessSyntax
//@[41:74)   PropertyAccessSyntax
//@[41:63)    VariableAccessSyntax
//@[41:63)     IdentifierSyntax
//@[41:63)      Identifier |nestedDiscriminator_if|
//@[63:64)    Dot |.|
//@[64:74)    IdentifierSyntax
//@[64:74)     Identifier |properties|
//@[74:75)   Dot |.|
//@[75:75)   IdentifierSyntax
//@[75:75)    SkippedTriviaSyntax
//@[75:77) NewLine |\r\n|
// #completionTest(78) -> defaultCreateModeProperties
//@[53:55) NewLine |\r\n|
var nestedDiscriminatorCompletions4_if = nestedDiscriminator_if['properties'].
//@[0:78) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:38)  IdentifierSyntax
//@[4:38)   Identifier |nestedDiscriminatorCompletions4_if|
//@[39:40)  Assignment |=|
//@[41:78)  PropertyAccessSyntax
//@[41:77)   ArrayAccessSyntax
//@[41:63)    VariableAccessSyntax
//@[41:63)     IdentifierSyntax
//@[41:63)      Identifier |nestedDiscriminator_if|
//@[63:64)    LeftSquare |[|
//@[64:76)    StringSyntax
//@[64:76)     StringComplete |'properties'|
//@[76:77)    RightSquare |]|
//@[77:78)   Dot |.|
//@[78:78)   IdentifierSyntax
//@[78:78)    SkippedTriviaSyntax
//@[78:82) NewLine |\r\n\r\n|

// #completionTest(85) -> defaultCreateModeIndexes_if
//@[53:55) NewLine |\r\n|
var nestedDiscriminatorArrayIndexCompletions_if = nestedDiscriminator_if.properties[a]
//@[0:86) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:47)  IdentifierSyntax
//@[4:47)   Identifier |nestedDiscriminatorArrayIndexCompletions_if|
//@[48:49)  Assignment |=|
//@[50:86)  ArrayAccessSyntax
//@[50:83)   PropertyAccessSyntax
//@[50:72)    VariableAccessSyntax
//@[50:72)     IdentifierSyntax
//@[50:72)      Identifier |nestedDiscriminator_if|
//@[72:73)    Dot |.|
//@[73:83)    IdentifierSyntax
//@[73:83)     Identifier |properties|
//@[83:84)   LeftSquare |[|
//@[84:85)   VariableAccessSyntax
//@[84:85)    IdentifierSyntax
//@[84:85)     Identifier |a|
//@[85:86)   RightSquare |]|
//@[86:92) NewLine |\r\n\r\n\r\n|


/*
Nested discriminator (loop)
*/
//@[2:4) NewLine |\r\n|
resource nestedDiscriminator_for 'Microsoft.DocumentDB/databaseAccounts@2020-06-01-preview' = [for thing in []: {
//@[0:201) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:32)  IdentifierSyntax
//@[9:32)   Identifier |nestedDiscriminator_for|
//@[33:91)  StringSyntax
//@[33:91)   StringComplete |'Microsoft.DocumentDB/databaseAccounts@2020-06-01-preview'|
//@[92:93)  Assignment |=|
//@[94:201)  ForSyntax
//@[94:95)   LeftSquare |[|
//@[95:98)   Identifier |for|
//@[99:104)   LocalVariableSyntax
//@[99:104)    IdentifierSyntax
//@[99:104)     Identifier |thing|
//@[105:107)   Identifier |in|
//@[108:110)   ArraySyntax
//@[108:109)    LeftSquare |[|
//@[109:110)    RightSquare |]|
//@[110:111)   Colon |:|
//@[112:200)   ObjectSyntax
//@[112:113)    LeftBrace |{|
//@[113:115)    NewLine |\r\n|
  name: 'test'
//@[2:14)    ObjectPropertySyntax
//@[2:6)     IdentifierSyntax
//@[2:6)      Identifier |name|
//@[6:7)     Colon |:|
//@[8:14)     StringSyntax
//@[8:14)      StringComplete |'test'|
//@[14:16)    NewLine |\r\n|
  location: 'l'
//@[2:15)    ObjectPropertySyntax
//@[2:10)     IdentifierSyntax
//@[2:10)      Identifier |location|
//@[10:11)     Colon |:|
//@[12:15)     StringSyntax
//@[12:15)      StringComplete |'l'|
//@[15:17)    NewLine |\r\n|
  properties: {
//@[2:49)    ObjectPropertySyntax
//@[2:12)     IdentifierSyntax
//@[2:12)      Identifier |properties|
//@[12:13)     Colon |:|
//@[14:49)     ObjectSyntax
//@[14:15)      LeftBrace |{|
//@[15:17)      NewLine |\r\n|
    createMode: 'Default'
//@[4:25)      ObjectPropertySyntax
//@[4:14)       IdentifierSyntax
//@[4:14)        Identifier |createMode|
//@[14:15)       Colon |:|
//@[16:25)       StringSyntax
//@[16:25)        StringComplete |'Default'|
//@[25:29)      NewLine |\r\n\r\n|

  }
//@[2:3)      RightBrace |}|
//@[3:5)    NewLine |\r\n|
}]
//@[0:1)    RightBrace |}|
//@[1:2)   RightSquare |]|
//@[2:4) NewLine |\r\n|
// #completionTest(80) -> defaultCreateModeProperties
//@[53:55) NewLine |\r\n|
var nestedDiscriminatorCompletions_for = nestedDiscriminator_for[0].properties.a
//@[0:80) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:38)  IdentifierSyntax
//@[4:38)   Identifier |nestedDiscriminatorCompletions_for|
//@[39:40)  Assignment |=|
//@[41:80)  PropertyAccessSyntax
//@[41:78)   PropertyAccessSyntax
//@[41:67)    ArrayAccessSyntax
//@[41:64)     VariableAccessSyntax
//@[41:64)      IdentifierSyntax
//@[41:64)       Identifier |nestedDiscriminator_for|
//@[64:65)     LeftSquare |[|
//@[65:66)     IntegerLiteralSyntax
//@[65:66)      Integer |0|
//@[66:67)     RightSquare |]|
//@[67:68)    Dot |.|
//@[68:78)    IdentifierSyntax
//@[68:78)     Identifier |properties|
//@[78:79)   Dot |.|
//@[79:80)   IdentifierSyntax
//@[79:80)    Identifier |a|
//@[80:82) NewLine |\r\n|
// #completionTest(84) -> defaultCreateModeProperties
//@[53:55) NewLine |\r\n|
var nestedDiscriminatorCompletions2_for = nestedDiscriminator_for[0]['properties'].a
//@[0:84) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:39)  IdentifierSyntax
//@[4:39)   Identifier |nestedDiscriminatorCompletions2_for|
//@[40:41)  Assignment |=|
//@[42:84)  PropertyAccessSyntax
//@[42:82)   ArrayAccessSyntax
//@[42:68)    ArrayAccessSyntax
//@[42:65)     VariableAccessSyntax
//@[42:65)      IdentifierSyntax
//@[42:65)       Identifier |nestedDiscriminator_for|
//@[65:66)     LeftSquare |[|
//@[66:67)     IntegerLiteralSyntax
//@[66:67)      Integer |0|
//@[67:68)     RightSquare |]|
//@[68:69)    LeftSquare |[|
//@[69:81)    StringSyntax
//@[69:81)     StringComplete |'properties'|
//@[81:82)    RightSquare |]|
//@[82:83)   Dot |.|
//@[83:84)   IdentifierSyntax
//@[83:84)    Identifier |a|
//@[84:86) NewLine |\r\n|
// #completionTest(80) -> defaultCreateModeProperties
//@[53:55) NewLine |\r\n|
var nestedDiscriminatorCompletions3_for = nestedDiscriminator_for[0].properties.
//@[0:80) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:39)  IdentifierSyntax
//@[4:39)   Identifier |nestedDiscriminatorCompletions3_for|
//@[40:41)  Assignment |=|
//@[42:80)  PropertyAccessSyntax
//@[42:79)   PropertyAccessSyntax
//@[42:68)    ArrayAccessSyntax
//@[42:65)     VariableAccessSyntax
//@[42:65)      IdentifierSyntax
//@[42:65)       Identifier |nestedDiscriminator_for|
//@[65:66)     LeftSquare |[|
//@[66:67)     IntegerLiteralSyntax
//@[66:67)      Integer |0|
//@[67:68)     RightSquare |]|
//@[68:69)    Dot |.|
//@[69:79)    IdentifierSyntax
//@[69:79)     Identifier |properties|
//@[79:80)   Dot |.|
//@[80:80)   IdentifierSyntax
//@[80:80)    SkippedTriviaSyntax
//@[80:82) NewLine |\r\n|
// #completionTest(83) -> defaultCreateModeProperties
//@[53:55) NewLine |\r\n|
var nestedDiscriminatorCompletions4_for = nestedDiscriminator_for[0]['properties'].
//@[0:83) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:39)  IdentifierSyntax
//@[4:39)   Identifier |nestedDiscriminatorCompletions4_for|
//@[40:41)  Assignment |=|
//@[42:83)  PropertyAccessSyntax
//@[42:82)   ArrayAccessSyntax
//@[42:68)    ArrayAccessSyntax
//@[42:65)     VariableAccessSyntax
//@[42:65)      IdentifierSyntax
//@[42:65)       Identifier |nestedDiscriminator_for|
//@[65:66)     LeftSquare |[|
//@[66:67)     IntegerLiteralSyntax
//@[66:67)      Integer |0|
//@[67:68)     RightSquare |]|
//@[68:69)    LeftSquare |[|
//@[69:81)    StringSyntax
//@[69:81)     StringComplete |'properties'|
//@[81:82)    RightSquare |]|
//@[82:83)   Dot |.|
//@[83:83)   IdentifierSyntax
//@[83:83)    SkippedTriviaSyntax
//@[83:87) NewLine |\r\n\r\n|

// #completionTest(90) -> defaultCreateModeIndexes_for
//@[54:56) NewLine |\r\n|
var nestedDiscriminatorArrayIndexCompletions_for = nestedDiscriminator_for[0].properties[a]
//@[0:91) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:48)  IdentifierSyntax
//@[4:48)   Identifier |nestedDiscriminatorArrayIndexCompletions_for|
//@[49:50)  Assignment |=|
//@[51:91)  ArrayAccessSyntax
//@[51:88)   PropertyAccessSyntax
//@[51:77)    ArrayAccessSyntax
//@[51:74)     VariableAccessSyntax
//@[51:74)      IdentifierSyntax
//@[51:74)       Identifier |nestedDiscriminator_for|
//@[74:75)     LeftSquare |[|
//@[75:76)     IntegerLiteralSyntax
//@[75:76)      Integer |0|
//@[76:77)     RightSquare |]|
//@[77:78)    Dot |.|
//@[78:88)    IdentifierSyntax
//@[78:88)     Identifier |properties|
//@[88:89)   LeftSquare |[|
//@[89:90)   VariableAccessSyntax
//@[89:90)    IdentifierSyntax
//@[89:90)     Identifier |a|
//@[90:91)   RightSquare |]|
//@[91:97) NewLine |\r\n\r\n\r\n|


/*
Nested discriminator (filtered loop)
*/
//@[2:4) NewLine |\r\n|
resource nestedDiscriminator_for_if 'Microsoft.DocumentDB/databaseAccounts@2020-06-01-preview' = [for thing in []: if(true) {
//@[0:213) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:35)  IdentifierSyntax
//@[9:35)   Identifier |nestedDiscriminator_for_if|
//@[36:94)  StringSyntax
//@[36:94)   StringComplete |'Microsoft.DocumentDB/databaseAccounts@2020-06-01-preview'|
//@[95:96)  Assignment |=|
//@[97:213)  ForSyntax
//@[97:98)   LeftSquare |[|
//@[98:101)   Identifier |for|
//@[102:107)   LocalVariableSyntax
//@[102:107)    IdentifierSyntax
//@[102:107)     Identifier |thing|
//@[108:110)   Identifier |in|
//@[111:113)   ArraySyntax
//@[111:112)    LeftSquare |[|
//@[112:113)    RightSquare |]|
//@[113:114)   Colon |:|
//@[115:212)   IfConditionSyntax
//@[115:117)    Identifier |if|
//@[117:123)    ParenthesizedExpressionSyntax
//@[117:118)     LeftParen |(|
//@[118:122)     BooleanLiteralSyntax
//@[118:122)      TrueKeyword |true|
//@[122:123)     RightParen |)|
//@[124:212)    ObjectSyntax
//@[124:125)     LeftBrace |{|
//@[125:127)     NewLine |\r\n|
  name: 'test'
//@[2:14)     ObjectPropertySyntax
//@[2:6)      IdentifierSyntax
//@[2:6)       Identifier |name|
//@[6:7)      Colon |:|
//@[8:14)      StringSyntax
//@[8:14)       StringComplete |'test'|
//@[14:16)     NewLine |\r\n|
  location: 'l'
//@[2:15)     ObjectPropertySyntax
//@[2:10)      IdentifierSyntax
//@[2:10)       Identifier |location|
//@[10:11)      Colon |:|
//@[12:15)      StringSyntax
//@[12:15)       StringComplete |'l'|
//@[15:17)     NewLine |\r\n|
  properties: {
//@[2:49)     ObjectPropertySyntax
//@[2:12)      IdentifierSyntax
//@[2:12)       Identifier |properties|
//@[12:13)      Colon |:|
//@[14:49)      ObjectSyntax
//@[14:15)       LeftBrace |{|
//@[15:17)       NewLine |\r\n|
    createMode: 'Default'
//@[4:25)       ObjectPropertySyntax
//@[4:14)        IdentifierSyntax
//@[4:14)         Identifier |createMode|
//@[14:15)        Colon |:|
//@[16:25)        StringSyntax
//@[16:25)         StringComplete |'Default'|
//@[25:29)       NewLine |\r\n\r\n|

  }
//@[2:3)       RightBrace |}|
//@[3:5)     NewLine |\r\n|
}]
//@[0:1)     RightBrace |}|
//@[1:2)   RightSquare |]|
//@[2:4) NewLine |\r\n|
// #completionTest(86) -> defaultCreateModeProperties
//@[53:55) NewLine |\r\n|
var nestedDiscriminatorCompletions_for_if = nestedDiscriminator_for_if[0].properties.a
//@[0:86) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:41)  IdentifierSyntax
//@[4:41)   Identifier |nestedDiscriminatorCompletions_for_if|
//@[42:43)  Assignment |=|
//@[44:86)  PropertyAccessSyntax
//@[44:84)   PropertyAccessSyntax
//@[44:73)    ArrayAccessSyntax
//@[44:70)     VariableAccessSyntax
//@[44:70)      IdentifierSyntax
//@[44:70)       Identifier |nestedDiscriminator_for_if|
//@[70:71)     LeftSquare |[|
//@[71:72)     IntegerLiteralSyntax
//@[71:72)      Integer |0|
//@[72:73)     RightSquare |]|
//@[73:74)    Dot |.|
//@[74:84)    IdentifierSyntax
//@[74:84)     Identifier |properties|
//@[84:85)   Dot |.|
//@[85:86)   IdentifierSyntax
//@[85:86)    Identifier |a|
//@[86:88) NewLine |\r\n|
// #completionTest(90) -> defaultCreateModeProperties
//@[53:55) NewLine |\r\n|
var nestedDiscriminatorCompletions2_for_if = nestedDiscriminator_for_if[0]['properties'].a
//@[0:90) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:42)  IdentifierSyntax
//@[4:42)   Identifier |nestedDiscriminatorCompletions2_for_if|
//@[43:44)  Assignment |=|
//@[45:90)  PropertyAccessSyntax
//@[45:88)   ArrayAccessSyntax
//@[45:74)    ArrayAccessSyntax
//@[45:71)     VariableAccessSyntax
//@[45:71)      IdentifierSyntax
//@[45:71)       Identifier |nestedDiscriminator_for_if|
//@[71:72)     LeftSquare |[|
//@[72:73)     IntegerLiteralSyntax
//@[72:73)      Integer |0|
//@[73:74)     RightSquare |]|
//@[74:75)    LeftSquare |[|
//@[75:87)    StringSyntax
//@[75:87)     StringComplete |'properties'|
//@[87:88)    RightSquare |]|
//@[88:89)   Dot |.|
//@[89:90)   IdentifierSyntax
//@[89:90)    Identifier |a|
//@[90:92) NewLine |\r\n|
// #completionTest(86) -> defaultCreateModeProperties
//@[53:55) NewLine |\r\n|
var nestedDiscriminatorCompletions3_for_if = nestedDiscriminator_for_if[0].properties.
//@[0:86) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:42)  IdentifierSyntax
//@[4:42)   Identifier |nestedDiscriminatorCompletions3_for_if|
//@[43:44)  Assignment |=|
//@[45:86)  PropertyAccessSyntax
//@[45:85)   PropertyAccessSyntax
//@[45:74)    ArrayAccessSyntax
//@[45:71)     VariableAccessSyntax
//@[45:71)      IdentifierSyntax
//@[45:71)       Identifier |nestedDiscriminator_for_if|
//@[71:72)     LeftSquare |[|
//@[72:73)     IntegerLiteralSyntax
//@[72:73)      Integer |0|
//@[73:74)     RightSquare |]|
//@[74:75)    Dot |.|
//@[75:85)    IdentifierSyntax
//@[75:85)     Identifier |properties|
//@[85:86)   Dot |.|
//@[86:86)   IdentifierSyntax
//@[86:86)    SkippedTriviaSyntax
//@[86:88) NewLine |\r\n|
// #completionTest(89) -> defaultCreateModeProperties
//@[53:55) NewLine |\r\n|
var nestedDiscriminatorCompletions4_for_if = nestedDiscriminator_for_if[0]['properties'].
//@[0:89) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:42)  IdentifierSyntax
//@[4:42)   Identifier |nestedDiscriminatorCompletions4_for_if|
//@[43:44)  Assignment |=|
//@[45:89)  PropertyAccessSyntax
//@[45:88)   ArrayAccessSyntax
//@[45:74)    ArrayAccessSyntax
//@[45:71)     VariableAccessSyntax
//@[45:71)      IdentifierSyntax
//@[45:71)       Identifier |nestedDiscriminator_for_if|
//@[71:72)     LeftSquare |[|
//@[72:73)     IntegerLiteralSyntax
//@[72:73)      Integer |0|
//@[73:74)     RightSquare |]|
//@[74:75)    LeftSquare |[|
//@[75:87)    StringSyntax
//@[75:87)     StringComplete |'properties'|
//@[87:88)    RightSquare |]|
//@[88:89)   Dot |.|
//@[89:89)   IdentifierSyntax
//@[89:89)    SkippedTriviaSyntax
//@[89:93) NewLine |\r\n\r\n|

// #completionTest(96) -> defaultCreateModeIndexes_for_if
//@[57:59) NewLine |\r\n|
var nestedDiscriminatorArrayIndexCompletions_for_if = nestedDiscriminator_for_if[0].properties[a]
//@[0:97) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:51)  IdentifierSyntax
//@[4:51)   Identifier |nestedDiscriminatorArrayIndexCompletions_for_if|
//@[52:53)  Assignment |=|
//@[54:97)  ArrayAccessSyntax
//@[54:94)   PropertyAccessSyntax
//@[54:83)    ArrayAccessSyntax
//@[54:80)     VariableAccessSyntax
//@[54:80)      IdentifierSyntax
//@[54:80)       Identifier |nestedDiscriminator_for_if|
//@[80:81)     LeftSquare |[|
//@[81:82)     IntegerLiteralSyntax
//@[81:82)      Integer |0|
//@[82:83)     RightSquare |]|
//@[83:84)    Dot |.|
//@[84:94)    IdentifierSyntax
//@[84:94)     Identifier |properties|
//@[94:95)   LeftSquare |[|
//@[95:96)   VariableAccessSyntax
//@[95:96)    IdentifierSyntax
//@[95:96)     Identifier |a|
//@[96:97)   RightSquare |]|
//@[97:105) NewLine |\r\n\r\n\r\n\r\n|



// sample resource to validate completions on the next declarations
//@[67:69) NewLine |\r\n|
resource nestedPropertyAccessOnConditional 'Microsoft.Compute/virtualMachines@2020-06-01' = if(true) {
//@[0:209) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:42)  IdentifierSyntax
//@[9:42)   Identifier |nestedPropertyAccessOnConditional|
//@[43:89)  StringSyntax
//@[43:89)   StringComplete |'Microsoft.Compute/virtualMachines@2020-06-01'|
//@[90:91)  Assignment |=|
//@[92:209)  IfConditionSyntax
//@[92:94)   Identifier |if|
//@[94:100)   ParenthesizedExpressionSyntax
//@[94:95)    LeftParen |(|
//@[95:99)    BooleanLiteralSyntax
//@[95:99)     TrueKeyword |true|
//@[99:100)    RightParen |)|
//@[101:209)   ObjectSyntax
//@[101:102)    LeftBrace |{|
//@[102:104)    NewLine |\r\n|
  location: 'test'
//@[2:18)    ObjectPropertySyntax
//@[2:10)     IdentifierSyntax
//@[2:10)      Identifier |location|
//@[10:11)     Colon |:|
//@[12:18)     StringSyntax
//@[12:18)      StringComplete |'test'|
//@[18:20)    NewLine |\r\n|
  name: 'test'
//@[2:14)    ObjectPropertySyntax
//@[2:6)     IdentifierSyntax
//@[2:6)      Identifier |name|
//@[6:7)     Colon |:|
//@[8:14)     StringSyntax
//@[8:14)      StringComplete |'test'|
//@[14:16)    NewLine |\r\n|
  properties: {
//@[2:66)    ObjectPropertySyntax
//@[2:12)     IdentifierSyntax
//@[2:12)      Identifier |properties|
//@[12:13)     Colon |:|
//@[14:66)     ObjectSyntax
//@[14:15)      LeftBrace |{|
//@[15:17)      NewLine |\r\n|
    additionalCapabilities: {
//@[4:44)      ObjectPropertySyntax
//@[4:26)       IdentifierSyntax
//@[4:26)        Identifier |additionalCapabilities|
//@[26:27)       Colon |:|
//@[28:44)       ObjectSyntax
//@[28:29)        LeftBrace |{|
//@[29:31)        NewLine |\r\n|
      
//@[6:8)        NewLine |\r\n|
    }
//@[4:5)        RightBrace |}|
//@[5:7)      NewLine |\r\n|
  }
//@[2:3)      RightBrace |}|
//@[3:5)    NewLine |\r\n|
}
//@[0:1)    RightBrace |}|
//@[1:3) NewLine |\r\n|
// this validates that we can get nested property access completions on a conditional resource
//@[94:96) NewLine |\r\n|
//#completionTest(56) -> vmProperties
//@[37:39) NewLine |\r\n|
var sigh = nestedPropertyAccessOnConditional.properties.
//@[0:56) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:8)  IdentifierSyntax
//@[4:8)   Identifier |sigh|
//@[9:10)  Assignment |=|
//@[11:56)  PropertyAccessSyntax
//@[11:55)   PropertyAccessSyntax
//@[11:44)    VariableAccessSyntax
//@[11:44)     IdentifierSyntax
//@[11:44)      Identifier |nestedPropertyAccessOnConditional|
//@[44:45)    Dot |.|
//@[45:55)    IdentifierSyntax
//@[45:55)     Identifier |properties|
//@[55:56)   Dot |.|
//@[56:56)   IdentifierSyntax
//@[56:56)    SkippedTriviaSyntax
//@[56:60) NewLine |\r\n\r\n|

/*
  boolean property value completions
*/ 
//@[3:5) NewLine |\r\n|
resource booleanPropertyPartialValue 'Microsoft.Compute/virtualMachines/extensions@2020-06-01' = {
//@[0:222) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:36)  IdentifierSyntax
//@[9:36)   Identifier |booleanPropertyPartialValue|
//@[37:94)  StringSyntax
//@[37:94)   StringComplete |'Microsoft.Compute/virtualMachines/extensions@2020-06-01'|
//@[95:96)  Assignment |=|
//@[97:222)  ObjectSyntax
//@[97:98)   LeftBrace |{|
//@[98:100)   NewLine |\r\n|
  properties: {
//@[2:119)   ObjectPropertySyntax
//@[2:12)    IdentifierSyntax
//@[2:12)     Identifier |properties|
//@[12:13)    Colon |:|
//@[14:119)    ObjectSyntax
//@[14:15)     LeftBrace |{|
//@[15:17)     NewLine |\r\n|
    // #completionTest(28,29,30) -> boolPropertyValuesPlusSymbols
//@[65:67)     NewLine |\r\n|
    autoUpgradeMinorVersion: t
//@[4:30)     ObjectPropertySyntax
//@[4:27)      IdentifierSyntax
//@[4:27)       Identifier |autoUpgradeMinorVersion|
//@[27:28)      Colon |:|
//@[29:30)      VariableAccessSyntax
//@[29:30)       IdentifierSyntax
//@[29:30)        Identifier |t|
//@[30:32)     NewLine |\r\n|
  }
//@[2:3)     RightBrace |}|
//@[3:5)   NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource selfScope 'My.Rp/mockResource@2020-12-01' = {
//@[0:98) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:18)  IdentifierSyntax
//@[9:18)   Identifier |selfScope|
//@[19:50)  StringSyntax
//@[19:50)   StringComplete |'My.Rp/mockResource@2020-12-01'|
//@[51:52)  Assignment |=|
//@[53:98)  ObjectSyntax
//@[53:54)   LeftBrace |{|
//@[54:56)   NewLine |\r\n|
  name: 'selfScope'
//@[2:19)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:19)    StringSyntax
//@[8:19)     StringComplete |'selfScope'|
//@[19:21)   NewLine |\r\n|
  scope: selfScope
//@[2:18)   ObjectPropertySyntax
//@[2:7)    IdentifierSyntax
//@[2:7)     Identifier |scope|
//@[7:8)    Colon |:|
//@[9:18)    VariableAccessSyntax
//@[9:18)     IdentifierSyntax
//@[9:18)      Identifier |selfScope|
//@[18:20)   NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

var notAResource = {
//@[0:54) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:16)  IdentifierSyntax
//@[4:16)   Identifier |notAResource|
//@[17:18)  Assignment |=|
//@[19:54)  ObjectSyntax
//@[19:20)   LeftBrace |{|
//@[20:22)   NewLine |\r\n|
  im: 'not'
//@[2:11)   ObjectPropertySyntax
//@[2:4)    IdentifierSyntax
//@[2:4)     Identifier |im|
//@[4:5)    Colon |:|
//@[6:11)    StringSyntax
//@[6:11)     StringComplete |'not'|
//@[11:13)   NewLine |\r\n|
  a: 'resource!'
//@[2:16)   ObjectPropertySyntax
//@[2:3)    IdentifierSyntax
//@[2:3)     Identifier |a|
//@[3:4)    Colon |:|
//@[5:16)    StringSyntax
//@[5:16)     StringComplete |'resource!'|
//@[16:18)   NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\r\n|
resource invalidScope 'My.Rp/mockResource@2020-12-01' = {
//@[0:107) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:21)  IdentifierSyntax
//@[9:21)   Identifier |invalidScope|
//@[22:53)  StringSyntax
//@[22:53)   StringComplete |'My.Rp/mockResource@2020-12-01'|
//@[54:55)  Assignment |=|
//@[56:107)  ObjectSyntax
//@[56:57)   LeftBrace |{|
//@[57:59)   NewLine |\r\n|
  name: 'invalidScope'
//@[2:22)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:22)    StringSyntax
//@[8:22)     StringComplete |'invalidScope'|
//@[22:24)   NewLine |\r\n|
  scope: notAResource
//@[2:21)   ObjectPropertySyntax
//@[2:7)    IdentifierSyntax
//@[2:7)     Identifier |scope|
//@[7:8)    Colon |:|
//@[9:21)    VariableAccessSyntax
//@[9:21)     IdentifierSyntax
//@[9:21)      Identifier |notAResource|
//@[21:23)   NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource invalidScope2 'My.Rp/mockResource@2020-12-01' = {
//@[0:112) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:22)  IdentifierSyntax
//@[9:22)   Identifier |invalidScope2|
//@[23:54)  StringSyntax
//@[23:54)   StringComplete |'My.Rp/mockResource@2020-12-01'|
//@[55:56)  Assignment |=|
//@[57:112)  ObjectSyntax
//@[57:58)   LeftBrace |{|
//@[58:60)   NewLine |\r\n|
  name: 'invalidScope2'
//@[2:23)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:23)    StringSyntax
//@[8:23)     StringComplete |'invalidScope2'|
//@[23:25)   NewLine |\r\n|
  scope: resourceGroup()
//@[2:24)   ObjectPropertySyntax
//@[2:7)    IdentifierSyntax
//@[2:7)     Identifier |scope|
//@[7:8)    Colon |:|
//@[9:24)    FunctionCallSyntax
//@[9:22)     IdentifierSyntax
//@[9:22)      Identifier |resourceGroup|
//@[22:23)     LeftParen |(|
//@[23:24)     RightParen |)|
//@[24:26)   NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource invalidScope3 'My.Rp/mockResource@2020-12-01' = {
//@[0:111) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:22)  IdentifierSyntax
//@[9:22)   Identifier |invalidScope3|
//@[23:54)  StringSyntax
//@[23:54)   StringComplete |'My.Rp/mockResource@2020-12-01'|
//@[55:56)  Assignment |=|
//@[57:111)  ObjectSyntax
//@[57:58)   LeftBrace |{|
//@[58:60)   NewLine |\r\n|
  name: 'invalidScope3'
//@[2:23)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:23)    StringSyntax
//@[8:23)     StringComplete |'invalidScope3'|
//@[23:25)   NewLine |\r\n|
  scope: subscription()
//@[2:23)   ObjectPropertySyntax
//@[2:7)    IdentifierSyntax
//@[2:7)     Identifier |scope|
//@[7:8)    Colon |:|
//@[9:23)    FunctionCallSyntax
//@[9:21)     IdentifierSyntax
//@[9:21)      Identifier |subscription|
//@[21:22)     LeftParen |(|
//@[22:23)     RightParen |)|
//@[23:25)   NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource invalidDuplicateName1 'Mock.Rp/mockResource@2020-01-01' = {
//@[0:103) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:30)  IdentifierSyntax
//@[9:30)   Identifier |invalidDuplicateName1|
//@[31:64)  StringSyntax
//@[31:64)   StringComplete |'Mock.Rp/mockResource@2020-01-01'|
//@[65:66)  Assignment |=|
//@[67:103)  ObjectSyntax
//@[67:68)   LeftBrace |{|
//@[68:70)   NewLine |\r\n|
  name: 'invalidDuplicateName'
//@[2:30)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:30)    StringSyntax
//@[8:30)     StringComplete |'invalidDuplicateName'|
//@[30:32)   NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\r\n|
resource invalidDuplicateName2 'Mock.Rp/mockResource@2020-01-01' = {
//@[0:103) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:30)  IdentifierSyntax
//@[9:30)   Identifier |invalidDuplicateName2|
//@[31:64)  StringSyntax
//@[31:64)   StringComplete |'Mock.Rp/mockResource@2020-01-01'|
//@[65:66)  Assignment |=|
//@[67:103)  ObjectSyntax
//@[67:68)   LeftBrace |{|
//@[68:70)   NewLine |\r\n|
  name: 'invalidDuplicateName'
//@[2:30)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:30)    StringSyntax
//@[8:30)     StringComplete |'invalidDuplicateName'|
//@[30:32)   NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\r\n|
resource invalidDuplicateName3 'Mock.Rp/mockResource@2019-01-01' = {
//@[0:103) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:30)  IdentifierSyntax
//@[9:30)   Identifier |invalidDuplicateName3|
//@[31:64)  StringSyntax
//@[31:64)   StringComplete |'Mock.Rp/mockResource@2019-01-01'|
//@[65:66)  Assignment |=|
//@[67:103)  ObjectSyntax
//@[67:68)   LeftBrace |{|
//@[68:70)   NewLine |\r\n|
  name: 'invalidDuplicateName'
//@[2:30)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:30)    StringSyntax
//@[8:30)     StringComplete |'invalidDuplicateName'|
//@[30:32)   NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource validResourceForInvalidExtensionResourceDuplicateName 'Mock.Rp/mockResource@2020-01-01' = {
//@[0:168) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:62)  IdentifierSyntax
//@[9:62)   Identifier |validResourceForInvalidExtensionResourceDuplicateName|
//@[63:96)  StringSyntax
//@[63:96)   StringComplete |'Mock.Rp/mockResource@2020-01-01'|
//@[97:98)  Assignment |=|
//@[99:168)  ObjectSyntax
//@[99:100)   LeftBrace |{|
//@[100:102)   NewLine |\r\n|
  name: 'validResourceForInvalidExtensionResourceDuplicateName'
//@[2:63)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:63)    StringSyntax
//@[8:63)     StringComplete |'validResourceForInvalidExtensionResourceDuplicateName'|
//@[63:65)   NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource invalidExtensionResourceDuplicateName1 'Mock.Rp/mockExtResource@2020-01-01' = {
//@[0:204) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:47)  IdentifierSyntax
//@[9:47)   Identifier |invalidExtensionResourceDuplicateName1|
//@[48:84)  StringSyntax
//@[48:84)   StringComplete |'Mock.Rp/mockExtResource@2020-01-01'|
//@[85:86)  Assignment |=|
//@[87:204)  ObjectSyntax
//@[87:88)   LeftBrace |{|
//@[88:90)   NewLine |\r\n|
  name: 'invalidExtensionResourceDuplicateName'
//@[2:47)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:47)    StringSyntax
//@[8:47)     StringComplete |'invalidExtensionResourceDuplicateName'|
//@[47:49)   NewLine |\r\n|
  scope: validResourceForInvalidExtensionResourceDuplicateName
//@[2:62)   ObjectPropertySyntax
//@[2:7)    IdentifierSyntax
//@[2:7)     Identifier |scope|
//@[7:8)    Colon |:|
//@[9:62)    VariableAccessSyntax
//@[9:62)     IdentifierSyntax
//@[9:62)      Identifier |validResourceForInvalidExtensionResourceDuplicateName|
//@[62:64)   NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource invalidExtensionResourceDuplicateName2 'Mock.Rp/mockExtResource@2019-01-01' = {
//@[0:204) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:47)  IdentifierSyntax
//@[9:47)   Identifier |invalidExtensionResourceDuplicateName2|
//@[48:84)  StringSyntax
//@[48:84)   StringComplete |'Mock.Rp/mockExtResource@2019-01-01'|
//@[85:86)  Assignment |=|
//@[87:204)  ObjectSyntax
//@[87:88)   LeftBrace |{|
//@[88:90)   NewLine |\r\n|
  name: 'invalidExtensionResourceDuplicateName'
//@[2:47)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:47)    StringSyntax
//@[8:47)     StringComplete |'invalidExtensionResourceDuplicateName'|
//@[47:49)   NewLine |\r\n|
  scope: validResourceForInvalidExtensionResourceDuplicateName
//@[2:62)   ObjectPropertySyntax
//@[2:7)    IdentifierSyntax
//@[2:7)     Identifier |scope|
//@[7:8)    Colon |:|
//@[9:62)    VariableAccessSyntax
//@[9:62)     IdentifierSyntax
//@[9:62)      Identifier |validResourceForInvalidExtensionResourceDuplicateName|
//@[62:64)   NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

@concat('foo', 'bar')
//@[0:131) ResourceDeclarationSyntax
//@[0:21)  DecoratorSyntax
//@[0:1)   At |@|
//@[1:21)   FunctionCallSyntax
//@[1:7)    IdentifierSyntax
//@[1:7)     Identifier |concat|
//@[7:8)    LeftParen |(|
//@[8:14)    FunctionArgumentSyntax
//@[8:13)     StringSyntax
//@[8:13)      StringComplete |'foo'|
//@[13:14)     Comma |,|
//@[15:20)    FunctionArgumentSyntax
//@[15:20)     StringSyntax
//@[15:20)      StringComplete |'bar'|
//@[20:21)    RightParen |)|
//@[21:23)  NewLine |\r\n|
@secure()
//@[0:9)  DecoratorSyntax
//@[0:1)   At |@|
//@[1:9)   FunctionCallSyntax
//@[1:7)    IdentifierSyntax
//@[1:7)     Identifier |secure|
//@[7:8)    LeftParen |(|
//@[8:9)    RightParen |)|
//@[9:11)  NewLine |\r\n|
resource invalidDecorator 'Microsoft.Foo/foos@2020-02-02-alpha'= {
//@[0:8)  Identifier |resource|
//@[9:25)  IdentifierSyntax
//@[9:25)   Identifier |invalidDecorator|
//@[26:63)  StringSyntax
//@[26:63)   StringComplete |'Microsoft.Foo/foos@2020-02-02-alpha'|
//@[63:64)  Assignment |=|
//@[65:97)  ObjectSyntax
//@[65:66)   LeftBrace |{|
//@[66:68)   NewLine |\r\n|
  name: 'invalidDecorator'
//@[2:26)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:26)    StringSyntax
//@[8:26)     StringComplete |'invalidDecorator'|
//@[26:28)   NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource cyclicRes 'Mock.Rp/mockExistingResource@2020-01-01' = {
//@[0:108) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:18)  IdentifierSyntax
//@[9:18)   Identifier |cyclicRes|
//@[19:60)  StringSyntax
//@[19:60)   StringComplete |'Mock.Rp/mockExistingResource@2020-01-01'|
//@[61:62)  Assignment |=|
//@[63:108)  ObjectSyntax
//@[63:64)   LeftBrace |{|
//@[64:66)   NewLine |\r\n|
  name: 'cyclicRes'
//@[2:19)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:19)    StringSyntax
//@[8:19)     StringComplete |'cyclicRes'|
//@[19:21)   NewLine |\r\n|
  scope: cyclicRes
//@[2:18)   ObjectPropertySyntax
//@[2:7)    IdentifierSyntax
//@[2:7)     Identifier |scope|
//@[7:8)    Colon |:|
//@[9:18)    VariableAccessSyntax
//@[9:18)     IdentifierSyntax
//@[9:18)      Identifier |cyclicRes|
//@[18:20)   NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource cyclicExistingRes 'Mock.Rp/mockExistingResource@2020-01-01' existing = {
//@[0:141) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:26)  IdentifierSyntax
//@[9:26)   Identifier |cyclicExistingRes|
//@[27:68)  StringSyntax
//@[27:68)   StringComplete |'Mock.Rp/mockExistingResource@2020-01-01'|
//@[69:77)  Identifier |existing|
//@[78:79)  Assignment |=|
//@[80:141)  ObjectSyntax
//@[80:81)   LeftBrace |{|
//@[81:83)   NewLine |\r\n|
  name: 'cyclicExistingRes'
//@[2:27)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:27)    StringSyntax
//@[8:27)     StringComplete |'cyclicExistingRes'|
//@[27:29)   NewLine |\r\n|
  scope: cyclicExistingRes
//@[2:26)   ObjectPropertySyntax
//@[2:7)    IdentifierSyntax
//@[2:7)     Identifier |scope|
//@[7:8)    Colon |:|
//@[9:26)    VariableAccessSyntax
//@[9:26)     IdentifierSyntax
//@[9:26)      Identifier |cyclicExistingRes|
//@[26:28)   NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

// loop parsing cases
//@[21:23) NewLine |\r\n|
resource expectedForKeyword 'Microsoft.Storage/storageAccounts@2019-06-01' = []
//@[0:79) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:27)  IdentifierSyntax
//@[9:27)   Identifier |expectedForKeyword|
//@[28:74)  StringSyntax
//@[28:74)   StringComplete |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[75:76)  Assignment |=|
//@[77:79)  SkippedTriviaSyntax
//@[77:78)   LeftSquare |[|
//@[78:79)   RightSquare |]|
//@[79:83) NewLine |\r\n\r\n|

resource expectedForKeyword2 'Microsoft.Storage/storageAccounts@2019-06-01' = [f]
//@[0:81) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:28)  IdentifierSyntax
//@[9:28)   Identifier |expectedForKeyword2|
//@[29:75)  StringSyntax
//@[29:75)   StringComplete |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[76:77)  Assignment |=|
//@[78:81)  SkippedTriviaSyntax
//@[78:79)   LeftSquare |[|
//@[79:80)   Identifier |f|
//@[80:81)   RightSquare |]|
//@[81:85) NewLine |\r\n\r\n|

resource expectedLoopVar 'Microsoft.Storage/storageAccounts@2019-06-01' = [for]
//@[0:79) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:24)  IdentifierSyntax
//@[9:24)   Identifier |expectedLoopVar|
//@[25:71)  StringSyntax
//@[25:71)   StringComplete |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[72:73)  Assignment |=|
//@[74:79)  ForSyntax
//@[74:75)   LeftSquare |[|
//@[75:78)   Identifier |for|
//@[78:78)   SkippedTriviaSyntax
//@[78:78)   SkippedTriviaSyntax
//@[78:78)   SkippedTriviaSyntax
//@[78:78)   SkippedTriviaSyntax
//@[78:78)   SkippedTriviaSyntax
//@[78:79)   RightSquare |]|
//@[79:83) NewLine |\r\n\r\n|

resource expectedInKeyword 'Microsoft.Storage/storageAccounts@2019-06-01' = [for x]
//@[0:83) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:26)  IdentifierSyntax
//@[9:26)   Identifier |expectedInKeyword|
//@[27:73)  StringSyntax
//@[27:73)   StringComplete |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[74:75)  Assignment |=|
//@[76:83)  ForSyntax
//@[76:77)   LeftSquare |[|
//@[77:80)   Identifier |for|
//@[81:82)   LocalVariableSyntax
//@[81:82)    IdentifierSyntax
//@[81:82)     Identifier |x|
//@[82:82)   SkippedTriviaSyntax
//@[82:82)   SkippedTriviaSyntax
//@[82:82)   SkippedTriviaSyntax
//@[82:82)   SkippedTriviaSyntax
//@[82:83)   RightSquare |]|
//@[83:87) NewLine |\r\n\r\n|

resource expectedInKeyword2 'Microsoft.Storage/storageAccounts@2019-06-01' = [for x b]
//@[0:86) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:27)  IdentifierSyntax
//@[9:27)   Identifier |expectedInKeyword2|
//@[28:74)  StringSyntax
//@[28:74)   StringComplete |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[75:76)  Assignment |=|
//@[77:86)  ForSyntax
//@[77:78)   LeftSquare |[|
//@[78:81)   Identifier |for|
//@[82:83)   LocalVariableSyntax
//@[82:83)    IdentifierSyntax
//@[82:83)     Identifier |x|
//@[84:85)   SkippedTriviaSyntax
//@[84:85)    Identifier |b|
//@[85:85)   SkippedTriviaSyntax
//@[85:85)   SkippedTriviaSyntax
//@[85:85)   SkippedTriviaSyntax
//@[85:86)   RightSquare |]|
//@[86:90) NewLine |\r\n\r\n|

resource expectedArrayExpression 'Microsoft.Storage/storageAccounts@2019-06-01' = [for x in]
//@[0:92) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:32)  IdentifierSyntax
//@[9:32)   Identifier |expectedArrayExpression|
//@[33:79)  StringSyntax
//@[33:79)   StringComplete |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[80:81)  Assignment |=|
//@[82:92)  ForSyntax
//@[82:83)   LeftSquare |[|
//@[83:86)   Identifier |for|
//@[87:88)   LocalVariableSyntax
//@[87:88)    IdentifierSyntax
//@[87:88)     Identifier |x|
//@[89:91)   Identifier |in|
//@[91:91)   SkippedTriviaSyntax
//@[91:91)   SkippedTriviaSyntax
//@[91:91)   SkippedTriviaSyntax
//@[91:92)   RightSquare |]|
//@[92:96) NewLine |\r\n\r\n|

resource expectedColon 'Microsoft.Storage/storageAccounts@2019-06-01' = [for x in y]
//@[0:84) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:22)  IdentifierSyntax
//@[9:22)   Identifier |expectedColon|
//@[23:69)  StringSyntax
//@[23:69)   StringComplete |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[70:71)  Assignment |=|
//@[72:84)  ForSyntax
//@[72:73)   LeftSquare |[|
//@[73:76)   Identifier |for|
//@[77:78)   LocalVariableSyntax
//@[77:78)    IdentifierSyntax
//@[77:78)     Identifier |x|
//@[79:81)   Identifier |in|
//@[82:83)   VariableAccessSyntax
//@[82:83)    IdentifierSyntax
//@[82:83)     Identifier |y|
//@[83:83)   SkippedTriviaSyntax
//@[83:83)   SkippedTriviaSyntax
//@[83:84)   RightSquare |]|
//@[84:88) NewLine |\r\n\r\n|

resource expectedLoopBody 'Microsoft.Storage/storageAccounts@2019-06-01' = [for x in y:]
//@[0:88) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:25)  IdentifierSyntax
//@[9:25)   Identifier |expectedLoopBody|
//@[26:72)  StringSyntax
//@[26:72)   StringComplete |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[73:74)  Assignment |=|
//@[75:88)  ForSyntax
//@[75:76)   LeftSquare |[|
//@[76:79)   Identifier |for|
//@[80:81)   LocalVariableSyntax
//@[80:81)    IdentifierSyntax
//@[80:81)     Identifier |x|
//@[82:84)   Identifier |in|
//@[85:86)   VariableAccessSyntax
//@[85:86)    IdentifierSyntax
//@[85:86)     Identifier |y|
//@[86:87)   Colon |:|
//@[87:87)   SkippedTriviaSyntax
//@[87:88)   RightSquare |]|
//@[88:92) NewLine |\r\n\r\n|

// loop index parsing cases
//@[27:29) NewLine |\r\n|
resource expectedLoopItemName 'Microsoft.Network/dnsZones@2018-05-01' = [for ()]
//@[0:80) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:29)  IdentifierSyntax
//@[9:29)   Identifier |expectedLoopItemName|
//@[30:69)  StringSyntax
//@[30:69)   StringComplete |'Microsoft.Network/dnsZones@2018-05-01'|
//@[70:71)  Assignment |=|
//@[72:80)  ForSyntax
//@[72:73)   LeftSquare |[|
//@[73:76)   Identifier |for|
//@[77:79)   ForVariableBlockSyntax
//@[77:78)    LeftParen |(|
//@[78:78)    LocalVariableSyntax
//@[78:78)     IdentifierSyntax
//@[78:78)      SkippedTriviaSyntax
//@[78:78)    SkippedTriviaSyntax
//@[78:78)    LocalVariableSyntax
//@[78:78)     IdentifierSyntax
//@[78:78)      SkippedTriviaSyntax
//@[78:79)    RightParen |)|
//@[79:79)   SkippedTriviaSyntax
//@[79:79)   SkippedTriviaSyntax
//@[79:79)   SkippedTriviaSyntax
//@[79:79)   SkippedTriviaSyntax
//@[79:80)   RightSquare |]|
//@[80:84) NewLine |\r\n\r\n|

resource expectedLoopItemName2 'Microsoft.Network/dnsZones@2018-05-01' = [for (
//@[0:79) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:30)  IdentifierSyntax
//@[9:30)   Identifier |expectedLoopItemName2|
//@[31:70)  StringSyntax
//@[31:70)   StringComplete |'Microsoft.Network/dnsZones@2018-05-01'|
//@[71:72)  Assignment |=|
//@[73:79)  ForSyntax
//@[73:74)   LeftSquare |[|
//@[74:77)   Identifier |for|
//@[78:79)   ForVariableBlockSyntax
//@[78:79)    LeftParen |(|
//@[79:79)    LocalVariableSyntax
//@[79:79)     IdentifierSyntax
//@[79:79)      SkippedTriviaSyntax
//@[79:79)    SkippedTriviaSyntax
//@[79:79)    LocalVariableSyntax
//@[79:79)     IdentifierSyntax
//@[79:79)      SkippedTriviaSyntax
//@[79:79)    SkippedTriviaSyntax
//@[79:79)   SkippedTriviaSyntax
//@[79:79)   SkippedTriviaSyntax
//@[79:79)   SkippedTriviaSyntax
//@[79:79)   SkippedTriviaSyntax
//@[79:79)   SkippedTriviaSyntax
//@[79:83) NewLine |\r\n\r\n|

resource expectedComma 'Microsoft.Network/dnsZones@2018-05-01' = [for (x)]
//@[0:74) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:22)  IdentifierSyntax
//@[9:22)   Identifier |expectedComma|
//@[23:62)  StringSyntax
//@[23:62)   StringComplete |'Microsoft.Network/dnsZones@2018-05-01'|
//@[63:64)  Assignment |=|
//@[65:74)  ForSyntax
//@[65:66)   LeftSquare |[|
//@[66:69)   Identifier |for|
//@[70:73)   ForVariableBlockSyntax
//@[70:71)    LeftParen |(|
//@[71:72)    LocalVariableSyntax
//@[71:72)     IdentifierSyntax
//@[71:72)      Identifier |x|
//@[72:72)    SkippedTriviaSyntax
//@[72:72)    LocalVariableSyntax
//@[72:72)     IdentifierSyntax
//@[72:72)      SkippedTriviaSyntax
//@[72:73)    RightParen |)|
//@[73:73)   SkippedTriviaSyntax
//@[73:73)   SkippedTriviaSyntax
//@[73:73)   SkippedTriviaSyntax
//@[73:73)   SkippedTriviaSyntax
//@[73:74)   RightSquare |]|
//@[74:78) NewLine |\r\n\r\n|

resource expectedLoopIndexName 'Microsoft.Network/dnsZones@2018-05-01' = [for (x, )]
//@[0:84) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:30)  IdentifierSyntax
//@[9:30)   Identifier |expectedLoopIndexName|
//@[31:70)  StringSyntax
//@[31:70)   StringComplete |'Microsoft.Network/dnsZones@2018-05-01'|
//@[71:72)  Assignment |=|
//@[73:84)  ForSyntax
//@[73:74)   LeftSquare |[|
//@[74:77)   Identifier |for|
//@[78:83)   ForVariableBlockSyntax
//@[78:79)    LeftParen |(|
//@[79:80)    LocalVariableSyntax
//@[79:80)     IdentifierSyntax
//@[79:80)      Identifier |x|
//@[80:81)    Comma |,|
//@[82:82)    LocalVariableSyntax
//@[82:82)     IdentifierSyntax
//@[82:82)      SkippedTriviaSyntax
//@[82:83)    RightParen |)|
//@[83:83)   SkippedTriviaSyntax
//@[83:83)   SkippedTriviaSyntax
//@[83:83)   SkippedTriviaSyntax
//@[83:83)   SkippedTriviaSyntax
//@[83:84)   RightSquare |]|
//@[84:88) NewLine |\r\n\r\n|

resource expectedInKeyword3 'Microsoft.Network/dnsZones@2018-05-01' = [for (x, y)]
//@[0:82) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:27)  IdentifierSyntax
//@[9:27)   Identifier |expectedInKeyword3|
//@[28:67)  StringSyntax
//@[28:67)   StringComplete |'Microsoft.Network/dnsZones@2018-05-01'|
//@[68:69)  Assignment |=|
//@[70:82)  ForSyntax
//@[70:71)   LeftSquare |[|
//@[71:74)   Identifier |for|
//@[75:81)   ForVariableBlockSyntax
//@[75:76)    LeftParen |(|
//@[76:77)    LocalVariableSyntax
//@[76:77)     IdentifierSyntax
//@[76:77)      Identifier |x|
//@[77:78)    Comma |,|
//@[79:80)    LocalVariableSyntax
//@[79:80)     IdentifierSyntax
//@[79:80)      Identifier |y|
//@[80:81)    RightParen |)|
//@[81:81)   SkippedTriviaSyntax
//@[81:81)   SkippedTriviaSyntax
//@[81:81)   SkippedTriviaSyntax
//@[81:81)   SkippedTriviaSyntax
//@[81:82)   RightSquare |]|
//@[82:86) NewLine |\r\n\r\n|

resource expectedInKeyword4 'Microsoft.Network/dnsZones@2018-05-01' = [for (x, y) z]
//@[0:84) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:27)  IdentifierSyntax
//@[9:27)   Identifier |expectedInKeyword4|
//@[28:67)  StringSyntax
//@[28:67)   StringComplete |'Microsoft.Network/dnsZones@2018-05-01'|
//@[68:69)  Assignment |=|
//@[70:84)  ForSyntax
//@[70:71)   LeftSquare |[|
//@[71:74)   Identifier |for|
//@[75:81)   ForVariableBlockSyntax
//@[75:76)    LeftParen |(|
//@[76:77)    LocalVariableSyntax
//@[76:77)     IdentifierSyntax
//@[76:77)      Identifier |x|
//@[77:78)    Comma |,|
//@[79:80)    LocalVariableSyntax
//@[79:80)     IdentifierSyntax
//@[79:80)      Identifier |y|
//@[80:81)    RightParen |)|
//@[82:83)   SkippedTriviaSyntax
//@[82:83)    Identifier |z|
//@[83:83)   SkippedTriviaSyntax
//@[83:83)   SkippedTriviaSyntax
//@[83:83)   SkippedTriviaSyntax
//@[83:84)   RightSquare |]|
//@[84:88) NewLine |\r\n\r\n|

resource expectedArrayExpression2 'Microsoft.Network/dnsZones@2018-05-01' = [for (x, y) in ]
//@[0:92) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:33)  IdentifierSyntax
//@[9:33)   Identifier |expectedArrayExpression2|
//@[34:73)  StringSyntax
//@[34:73)   StringComplete |'Microsoft.Network/dnsZones@2018-05-01'|
//@[74:75)  Assignment |=|
//@[76:92)  ForSyntax
//@[76:77)   LeftSquare |[|
//@[77:80)   Identifier |for|
//@[81:87)   ForVariableBlockSyntax
//@[81:82)    LeftParen |(|
//@[82:83)    LocalVariableSyntax
//@[82:83)     IdentifierSyntax
//@[82:83)      Identifier |x|
//@[83:84)    Comma |,|
//@[85:86)    LocalVariableSyntax
//@[85:86)     IdentifierSyntax
//@[85:86)      Identifier |y|
//@[86:87)    RightParen |)|
//@[88:90)   Identifier |in|
//@[91:91)   SkippedTriviaSyntax
//@[91:91)   SkippedTriviaSyntax
//@[91:91)   SkippedTriviaSyntax
//@[91:92)   RightSquare |]|
//@[92:96) NewLine |\r\n\r\n|

resource expectedColon2 'Microsoft.Network/dnsZones@2018-05-01' = [for (x, y) in z]
//@[0:83) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:23)  IdentifierSyntax
//@[9:23)   Identifier |expectedColon2|
//@[24:63)  StringSyntax
//@[24:63)   StringComplete |'Microsoft.Network/dnsZones@2018-05-01'|
//@[64:65)  Assignment |=|
//@[66:83)  ForSyntax
//@[66:67)   LeftSquare |[|
//@[67:70)   Identifier |for|
//@[71:77)   ForVariableBlockSyntax
//@[71:72)    LeftParen |(|
//@[72:73)    LocalVariableSyntax
//@[72:73)     IdentifierSyntax
//@[72:73)      Identifier |x|
//@[73:74)    Comma |,|
//@[75:76)    LocalVariableSyntax
//@[75:76)     IdentifierSyntax
//@[75:76)      Identifier |y|
//@[76:77)    RightParen |)|
//@[78:80)   Identifier |in|
//@[81:82)   VariableAccessSyntax
//@[81:82)    IdentifierSyntax
//@[81:82)     Identifier |z|
//@[82:82)   SkippedTriviaSyntax
//@[82:82)   SkippedTriviaSyntax
//@[82:83)   RightSquare |]|
//@[83:87) NewLine |\r\n\r\n|

resource expectedLoopBody2 'Microsoft.Network/dnsZones@2018-05-01' = [for (x, y) in z:]
//@[0:87) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:26)  IdentifierSyntax
//@[9:26)   Identifier |expectedLoopBody2|
//@[27:66)  StringSyntax
//@[27:66)   StringComplete |'Microsoft.Network/dnsZones@2018-05-01'|
//@[67:68)  Assignment |=|
//@[69:87)  ForSyntax
//@[69:70)   LeftSquare |[|
//@[70:73)   Identifier |for|
//@[74:80)   ForVariableBlockSyntax
//@[74:75)    LeftParen |(|
//@[75:76)    LocalVariableSyntax
//@[75:76)     IdentifierSyntax
//@[75:76)      Identifier |x|
//@[76:77)    Comma |,|
//@[78:79)    LocalVariableSyntax
//@[78:79)     IdentifierSyntax
//@[78:79)      Identifier |y|
//@[79:80)    RightParen |)|
//@[81:83)   Identifier |in|
//@[84:85)   VariableAccessSyntax
//@[84:85)    IdentifierSyntax
//@[84:85)     Identifier |z|
//@[85:86)   Colon |:|
//@[86:86)   SkippedTriviaSyntax
//@[86:87)   RightSquare |]|
//@[87:91) NewLine |\r\n\r\n|

// loop filter parsing cases
//@[28:30) NewLine |\r\n|
resource expectedLoopFilterOpenParen 'Microsoft.Storage/storageAccounts@2019-06-01' = [for x in y: if]
//@[0:102) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:36)  IdentifierSyntax
//@[9:36)   Identifier |expectedLoopFilterOpenParen|
//@[37:83)  StringSyntax
//@[37:83)   StringComplete |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[84:85)  Assignment |=|
//@[86:102)  ForSyntax
//@[86:87)   LeftSquare |[|
//@[87:90)   Identifier |for|
//@[91:92)   LocalVariableSyntax
//@[91:92)    IdentifierSyntax
//@[91:92)     Identifier |x|
//@[93:95)   Identifier |in|
//@[96:97)   VariableAccessSyntax
//@[96:97)    IdentifierSyntax
//@[96:97)     Identifier |y|
//@[97:98)   Colon |:|
//@[99:101)   IfConditionSyntax
//@[99:101)    Identifier |if|
//@[101:101)    SkippedTriviaSyntax
//@[101:101)    SkippedTriviaSyntax
//@[101:102)   RightSquare |]|
//@[102:104) NewLine |\r\n|
resource expectedLoopFilterOpenParen2 'Microsoft.Network/dnsZones@2018-05-01' = [for (x, y) in z: if]
//@[0:101) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:37)  IdentifierSyntax
//@[9:37)   Identifier |expectedLoopFilterOpenParen2|
//@[38:77)  StringSyntax
//@[38:77)   StringComplete |'Microsoft.Network/dnsZones@2018-05-01'|
//@[78:79)  Assignment |=|
//@[80:101)  ForSyntax
//@[80:81)   LeftSquare |[|
//@[81:84)   Identifier |for|
//@[85:91)   ForVariableBlockSyntax
//@[85:86)    LeftParen |(|
//@[86:87)    LocalVariableSyntax
//@[86:87)     IdentifierSyntax
//@[86:87)      Identifier |x|
//@[87:88)    Comma |,|
//@[89:90)    LocalVariableSyntax
//@[89:90)     IdentifierSyntax
//@[89:90)      Identifier |y|
//@[90:91)    RightParen |)|
//@[92:94)   Identifier |in|
//@[95:96)   VariableAccessSyntax
//@[95:96)    IdentifierSyntax
//@[95:96)     Identifier |z|
//@[96:97)   Colon |:|
//@[98:100)   IfConditionSyntax
//@[98:100)    Identifier |if|
//@[100:100)    SkippedTriviaSyntax
//@[100:100)    SkippedTriviaSyntax
//@[100:101)   RightSquare |]|
//@[101:105) NewLine |\r\n\r\n|

resource expectedLoopFilterPredicateAndBody 'Microsoft.Storage/storageAccounts@2019-06-01' = [for x in y: if()]
//@[0:111) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:43)  IdentifierSyntax
//@[9:43)   Identifier |expectedLoopFilterPredicateAndBody|
//@[44:90)  StringSyntax
//@[44:90)   StringComplete |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[91:92)  Assignment |=|
//@[93:111)  ForSyntax
//@[93:94)   LeftSquare |[|
//@[94:97)   Identifier |for|
//@[98:99)   LocalVariableSyntax
//@[98:99)    IdentifierSyntax
//@[98:99)     Identifier |x|
//@[100:102)   Identifier |in|
//@[103:104)   VariableAccessSyntax
//@[103:104)    IdentifierSyntax
//@[103:104)     Identifier |y|
//@[104:105)   Colon |:|
//@[106:110)   IfConditionSyntax
//@[106:108)    Identifier |if|
//@[108:110)    ParenthesizedExpressionSyntax
//@[108:109)     LeftParen |(|
//@[109:109)     SkippedTriviaSyntax
//@[109:110)     RightParen |)|
//@[110:110)    SkippedTriviaSyntax
//@[110:111)   RightSquare |]|
//@[111:113) NewLine |\r\n|
resource expectedLoopFilterPredicateAndBody2 'Microsoft.Network/dnsZones@2018-05-01' = [for (x, y) in z: if()]
//@[0:110) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:44)  IdentifierSyntax
//@[9:44)   Identifier |expectedLoopFilterPredicateAndBody2|
//@[45:84)  StringSyntax
//@[45:84)   StringComplete |'Microsoft.Network/dnsZones@2018-05-01'|
//@[85:86)  Assignment |=|
//@[87:110)  ForSyntax
//@[87:88)   LeftSquare |[|
//@[88:91)   Identifier |for|
//@[92:98)   ForVariableBlockSyntax
//@[92:93)    LeftParen |(|
//@[93:94)    LocalVariableSyntax
//@[93:94)     IdentifierSyntax
//@[93:94)      Identifier |x|
//@[94:95)    Comma |,|
//@[96:97)    LocalVariableSyntax
//@[96:97)     IdentifierSyntax
//@[96:97)      Identifier |y|
//@[97:98)    RightParen |)|
//@[99:101)   Identifier |in|
//@[102:103)   VariableAccessSyntax
//@[102:103)    IdentifierSyntax
//@[102:103)     Identifier |z|
//@[103:104)   Colon |:|
//@[105:109)   IfConditionSyntax
//@[105:107)    Identifier |if|
//@[107:109)    ParenthesizedExpressionSyntax
//@[107:108)     LeftParen |(|
//@[108:108)     SkippedTriviaSyntax
//@[108:109)     RightParen |)|
//@[109:109)    SkippedTriviaSyntax
//@[109:110)   RightSquare |]|
//@[110:114) NewLine |\r\n\r\n|

// wrong body type
//@[18:20) NewLine |\r\n|
var emptyArray = []
//@[0:19) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:14)  IdentifierSyntax
//@[4:14)   Identifier |emptyArray|
//@[15:16)  Assignment |=|
//@[17:19)  ArraySyntax
//@[17:18)   LeftSquare |[|
//@[18:19)   RightSquare |]|
//@[19:21) NewLine |\r\n|
resource wrongLoopBodyType 'Microsoft.Storage/storageAccounts@2019-06-01' = [for x in emptyArray:4]
//@[0:99) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:26)  IdentifierSyntax
//@[9:26)   Identifier |wrongLoopBodyType|
//@[27:73)  StringSyntax
//@[27:73)   StringComplete |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[74:75)  Assignment |=|
//@[76:99)  ForSyntax
//@[76:77)   LeftSquare |[|
//@[77:80)   Identifier |for|
//@[81:82)   LocalVariableSyntax
//@[81:82)    IdentifierSyntax
//@[81:82)     Identifier |x|
//@[83:85)   Identifier |in|
//@[86:96)   VariableAccessSyntax
//@[86:96)    IdentifierSyntax
//@[86:96)     Identifier |emptyArray|
//@[96:97)   Colon |:|
//@[97:98)   SkippedTriviaSyntax
//@[97:98)    Integer |4|
//@[98:99)   RightSquare |]|
//@[99:101) NewLine |\r\n|
resource wrongLoopBodyType2 'Microsoft.Storage/storageAccounts@2019-06-01' = [for (x ,i) in emptyArray:4]
//@[0:105) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:27)  IdentifierSyntax
//@[9:27)   Identifier |wrongLoopBodyType2|
//@[28:74)  StringSyntax
//@[28:74)   StringComplete |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[75:76)  Assignment |=|
//@[77:105)  ForSyntax
//@[77:78)   LeftSquare |[|
//@[78:81)   Identifier |for|
//@[82:88)   ForVariableBlockSyntax
//@[82:83)    LeftParen |(|
//@[83:84)    LocalVariableSyntax
//@[83:84)     IdentifierSyntax
//@[83:84)      Identifier |x|
//@[85:86)    Comma |,|
//@[86:87)    LocalVariableSyntax
//@[86:87)     IdentifierSyntax
//@[86:87)      Identifier |i|
//@[87:88)    RightParen |)|
//@[89:91)   Identifier |in|
//@[92:102)   VariableAccessSyntax
//@[92:102)    IdentifierSyntax
//@[92:102)     Identifier |emptyArray|
//@[102:103)   Colon |:|
//@[103:104)   SkippedTriviaSyntax
//@[103:104)    Integer |4|
//@[104:105)   RightSquare |]|
//@[105:109) NewLine |\r\n\r\n|

// duplicate variable in the same scope
//@[39:41) NewLine |\r\n|
resource itemAndIndexSameName 'Microsoft.AAD/domainServices@2020-01-01' = [for (same, same) in emptyArray: {
//@[0:112) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:29)  IdentifierSyntax
//@[9:29)   Identifier |itemAndIndexSameName|
//@[30:71)  StringSyntax
//@[30:71)   StringComplete |'Microsoft.AAD/domainServices@2020-01-01'|
//@[72:73)  Assignment |=|
//@[74:112)  ForSyntax
//@[74:75)   LeftSquare |[|
//@[75:78)   Identifier |for|
//@[79:91)   ForVariableBlockSyntax
//@[79:80)    LeftParen |(|
//@[80:84)    LocalVariableSyntax
//@[80:84)     IdentifierSyntax
//@[80:84)      Identifier |same|
//@[84:85)    Comma |,|
//@[86:90)    LocalVariableSyntax
//@[86:90)     IdentifierSyntax
//@[86:90)      Identifier |same|
//@[90:91)    RightParen |)|
//@[92:94)   Identifier |in|
//@[95:105)   VariableAccessSyntax
//@[95:105)    IdentifierSyntax
//@[95:105)     Identifier |emptyArray|
//@[105:106)   Colon |:|
//@[107:111)   ObjectSyntax
//@[107:108)    LeftBrace |{|
//@[108:110)    NewLine |\r\n|
}]
//@[0:1)    RightBrace |}|
//@[1:2)   RightSquare |]|
//@[2:6) NewLine |\r\n\r\n|

// errors in the array expression
//@[33:35) NewLine |\r\n|
resource arrayExpressionErrors 'Microsoft.Storage/storageAccounts@2019-06-01' = [for account in union([], 2): {
//@[0:115) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:30)  IdentifierSyntax
//@[9:30)   Identifier |arrayExpressionErrors|
//@[31:77)  StringSyntax
//@[31:77)   StringComplete |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[78:79)  Assignment |=|
//@[80:115)  ForSyntax
//@[80:81)   LeftSquare |[|
//@[81:84)   Identifier |for|
//@[85:92)   LocalVariableSyntax
//@[85:92)    IdentifierSyntax
//@[85:92)     Identifier |account|
//@[93:95)   Identifier |in|
//@[96:108)   FunctionCallSyntax
//@[96:101)    IdentifierSyntax
//@[96:101)     Identifier |union|
//@[101:102)    LeftParen |(|
//@[102:105)    FunctionArgumentSyntax
//@[102:104)     ArraySyntax
//@[102:103)      LeftSquare |[|
//@[103:104)      RightSquare |]|
//@[104:105)     Comma |,|
//@[106:107)    FunctionArgumentSyntax
//@[106:107)     IntegerLiteralSyntax
//@[106:107)      Integer |2|
//@[107:108)    RightParen |)|
//@[108:109)   Colon |:|
//@[110:114)   ObjectSyntax
//@[110:111)    LeftBrace |{|
//@[111:113)    NewLine |\r\n|
}]
//@[0:1)    RightBrace |}|
//@[1:2)   RightSquare |]|
//@[2:4) NewLine |\r\n|
resource arrayExpressionErrors2 'Microsoft.Storage/storageAccounts@2019-06-01' = [for (account,k) in union([], 2): {
//@[0:120) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:31)  IdentifierSyntax
//@[9:31)   Identifier |arrayExpressionErrors2|
//@[32:78)  StringSyntax
//@[32:78)   StringComplete |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[79:80)  Assignment |=|
//@[81:120)  ForSyntax
//@[81:82)   LeftSquare |[|
//@[82:85)   Identifier |for|
//@[86:97)   ForVariableBlockSyntax
//@[86:87)    LeftParen |(|
//@[87:94)    LocalVariableSyntax
//@[87:94)     IdentifierSyntax
//@[87:94)      Identifier |account|
//@[94:95)    Comma |,|
//@[95:96)    LocalVariableSyntax
//@[95:96)     IdentifierSyntax
//@[95:96)      Identifier |k|
//@[96:97)    RightParen |)|
//@[98:100)   Identifier |in|
//@[101:113)   FunctionCallSyntax
//@[101:106)    IdentifierSyntax
//@[101:106)     Identifier |union|
//@[106:107)    LeftParen |(|
//@[107:110)    FunctionArgumentSyntax
//@[107:109)     ArraySyntax
//@[107:108)      LeftSquare |[|
//@[108:109)      RightSquare |]|
//@[109:110)     Comma |,|
//@[111:112)    FunctionArgumentSyntax
//@[111:112)     IntegerLiteralSyntax
//@[111:112)      Integer |2|
//@[112:113)    RightParen |)|
//@[113:114)   Colon |:|
//@[115:119)   ObjectSyntax
//@[115:116)    LeftBrace |{|
//@[116:118)    NewLine |\r\n|
}]
//@[0:1)    RightBrace |}|
//@[1:2)   RightSquare |]|
//@[2:6) NewLine |\r\n\r\n|

// wrong array type
//@[19:21) NewLine |\r\n|
var notAnArray = true
//@[0:21) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:14)  IdentifierSyntax
//@[4:14)   Identifier |notAnArray|
//@[15:16)  Assignment |=|
//@[17:21)  BooleanLiteralSyntax
//@[17:21)   TrueKeyword |true|
//@[21:23) NewLine |\r\n|
resource wrongArrayType 'Microsoft.Storage/storageAccounts@2019-06-01' = [for account in notAnArray: {
//@[0:106) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:23)  IdentifierSyntax
//@[9:23)   Identifier |wrongArrayType|
//@[24:70)  StringSyntax
//@[24:70)   StringComplete |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[71:72)  Assignment |=|
//@[73:106)  ForSyntax
//@[73:74)   LeftSquare |[|
//@[74:77)   Identifier |for|
//@[78:85)   LocalVariableSyntax
//@[78:85)    IdentifierSyntax
//@[78:85)     Identifier |account|
//@[86:88)   Identifier |in|
//@[89:99)   VariableAccessSyntax
//@[89:99)    IdentifierSyntax
//@[89:99)     Identifier |notAnArray|
//@[99:100)   Colon |:|
//@[101:105)   ObjectSyntax
//@[101:102)    LeftBrace |{|
//@[102:104)    NewLine |\r\n|
}]
//@[0:1)    RightBrace |}|
//@[1:2)   RightSquare |]|
//@[2:4) NewLine |\r\n|
resource wrongArrayType2 'Microsoft.Storage/storageAccounts@2019-06-01' = [for (account,i) in notAnArray: {
//@[0:111) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:24)  IdentifierSyntax
//@[9:24)   Identifier |wrongArrayType2|
//@[25:71)  StringSyntax
//@[25:71)   StringComplete |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[72:73)  Assignment |=|
//@[74:111)  ForSyntax
//@[74:75)   LeftSquare |[|
//@[75:78)   Identifier |for|
//@[79:90)   ForVariableBlockSyntax
//@[79:80)    LeftParen |(|
//@[80:87)    LocalVariableSyntax
//@[80:87)     IdentifierSyntax
//@[80:87)      Identifier |account|
//@[87:88)    Comma |,|
//@[88:89)    LocalVariableSyntax
//@[88:89)     IdentifierSyntax
//@[88:89)      Identifier |i|
//@[89:90)    RightParen |)|
//@[91:93)   Identifier |in|
//@[94:104)   VariableAccessSyntax
//@[94:104)    IdentifierSyntax
//@[94:104)     Identifier |notAnArray|
//@[104:105)   Colon |:|
//@[106:110)   ObjectSyntax
//@[106:107)    LeftBrace |{|
//@[107:109)    NewLine |\r\n|
}]
//@[0:1)    RightBrace |}|
//@[1:2)   RightSquare |]|
//@[2:6) NewLine |\r\n\r\n|

// wrong filter expression type
//@[31:33) NewLine |\r\n|
resource wrongFilterExpressionType 'Microsoft.Storage/storageAccounts@2019-06-01' = [for account in emptyArray: if(4) {
//@[0:123) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:34)  IdentifierSyntax
//@[9:34)   Identifier |wrongFilterExpressionType|
//@[35:81)  StringSyntax
//@[35:81)   StringComplete |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[82:83)  Assignment |=|
//@[84:123)  ForSyntax
//@[84:85)   LeftSquare |[|
//@[85:88)   Identifier |for|
//@[89:96)   LocalVariableSyntax
//@[89:96)    IdentifierSyntax
//@[89:96)     Identifier |account|
//@[97:99)   Identifier |in|
//@[100:110)   VariableAccessSyntax
//@[100:110)    IdentifierSyntax
//@[100:110)     Identifier |emptyArray|
//@[110:111)   Colon |:|
//@[112:122)   IfConditionSyntax
//@[112:114)    Identifier |if|
//@[114:117)    ParenthesizedExpressionSyntax
//@[114:115)     LeftParen |(|
//@[115:116)     IntegerLiteralSyntax
//@[115:116)      Integer |4|
//@[116:117)     RightParen |)|
//@[118:122)    ObjectSyntax
//@[118:119)     LeftBrace |{|
//@[119:121)     NewLine |\r\n|
}]
//@[0:1)     RightBrace |}|
//@[1:2)   RightSquare |]|
//@[2:4) NewLine |\r\n|
resource wrongFilterExpressionType2 'Microsoft.Storage/storageAccounts@2019-06-01' = [for (account,i) in emptyArray: if(concat('s')){
//@[0:137) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:35)  IdentifierSyntax
//@[9:35)   Identifier |wrongFilterExpressionType2|
//@[36:82)  StringSyntax
//@[36:82)   StringComplete |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[83:84)  Assignment |=|
//@[85:137)  ForSyntax
//@[85:86)   LeftSquare |[|
//@[86:89)   Identifier |for|
//@[90:101)   ForVariableBlockSyntax
//@[90:91)    LeftParen |(|
//@[91:98)    LocalVariableSyntax
//@[91:98)     IdentifierSyntax
//@[91:98)      Identifier |account|
//@[98:99)    Comma |,|
//@[99:100)    LocalVariableSyntax
//@[99:100)     IdentifierSyntax
//@[99:100)      Identifier |i|
//@[100:101)    RightParen |)|
//@[102:104)   Identifier |in|
//@[105:115)   VariableAccessSyntax
//@[105:115)    IdentifierSyntax
//@[105:115)     Identifier |emptyArray|
//@[115:116)   Colon |:|
//@[117:136)   IfConditionSyntax
//@[117:119)    Identifier |if|
//@[119:132)    ParenthesizedExpressionSyntax
//@[119:120)     LeftParen |(|
//@[120:131)     FunctionCallSyntax
//@[120:126)      IdentifierSyntax
//@[120:126)       Identifier |concat|
//@[126:127)      LeftParen |(|
//@[127:130)      FunctionArgumentSyntax
//@[127:130)       StringSyntax
//@[127:130)        StringComplete |'s'|
//@[130:131)      RightParen |)|
//@[131:132)     RightParen |)|
//@[132:136)    ObjectSyntax
//@[132:133)     LeftBrace |{|
//@[133:135)     NewLine |\r\n|
}]
//@[0:1)     RightBrace |}|
//@[1:2)   RightSquare |]|
//@[2:6) NewLine |\r\n\r\n|

// missing required properties
//@[30:32) NewLine |\r\n|
resource missingRequiredProperties 'Microsoft.Storage/storageAccounts@2019-06-01' = [for account in []: {
//@[0:109) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:34)  IdentifierSyntax
//@[9:34)   Identifier |missingRequiredProperties|
//@[35:81)  StringSyntax
//@[35:81)   StringComplete |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[82:83)  Assignment |=|
//@[84:109)  ForSyntax
//@[84:85)   LeftSquare |[|
//@[85:88)   Identifier |for|
//@[89:96)   LocalVariableSyntax
//@[89:96)    IdentifierSyntax
//@[89:96)     Identifier |account|
//@[97:99)   Identifier |in|
//@[100:102)   ArraySyntax
//@[100:101)    LeftSquare |[|
//@[101:102)    RightSquare |]|
//@[102:103)   Colon |:|
//@[104:108)   ObjectSyntax
//@[104:105)    LeftBrace |{|
//@[105:107)    NewLine |\r\n|
}]
//@[0:1)    RightBrace |}|
//@[1:2)   RightSquare |]|
//@[2:4) NewLine |\r\n|
resource missingRequiredProperties2 'Microsoft.Storage/storageAccounts@2019-06-01' = [for (account,j) in []: {
//@[0:114) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:35)  IdentifierSyntax
//@[9:35)   Identifier |missingRequiredProperties2|
//@[36:82)  StringSyntax
//@[36:82)   StringComplete |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[83:84)  Assignment |=|
//@[85:114)  ForSyntax
//@[85:86)   LeftSquare |[|
//@[86:89)   Identifier |for|
//@[90:101)   ForVariableBlockSyntax
//@[90:91)    LeftParen |(|
//@[91:98)    LocalVariableSyntax
//@[91:98)     IdentifierSyntax
//@[91:98)      Identifier |account|
//@[98:99)    Comma |,|
//@[99:100)    LocalVariableSyntax
//@[99:100)     IdentifierSyntax
//@[99:100)      Identifier |j|
//@[100:101)    RightParen |)|
//@[102:104)   Identifier |in|
//@[105:107)   ArraySyntax
//@[105:106)    LeftSquare |[|
//@[106:107)    RightSquare |]|
//@[107:108)   Colon |:|
//@[109:113)   ObjectSyntax
//@[109:110)    LeftBrace |{|
//@[110:112)    NewLine |\r\n|
}]
//@[0:1)    RightBrace |}|
//@[1:2)   RightSquare |]|
//@[2:6) NewLine |\r\n\r\n|

// fewer missing required properties and a wrong property
//@[57:59) NewLine |\r\n|
resource missingFewerRequiredProperties 'Microsoft.Storage/storageAccounts@2019-06-01' = [for account in []: {
//@[0:196) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:39)  IdentifierSyntax
//@[9:39)   Identifier |missingFewerRequiredProperties|
//@[40:86)  StringSyntax
//@[40:86)   StringComplete |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[87:88)  Assignment |=|
//@[89:196)  ForSyntax
//@[89:90)   LeftSquare |[|
//@[90:93)   Identifier |for|
//@[94:101)   LocalVariableSyntax
//@[94:101)    IdentifierSyntax
//@[94:101)     Identifier |account|
//@[102:104)   Identifier |in|
//@[105:107)   ArraySyntax
//@[105:106)    LeftSquare |[|
//@[106:107)    RightSquare |]|
//@[107:108)   Colon |:|
//@[109:195)   ObjectSyntax
//@[109:110)    LeftBrace |{|
//@[110:112)    NewLine |\r\n|
  name: account
//@[2:15)    ObjectPropertySyntax
//@[2:6)     IdentifierSyntax
//@[2:6)      Identifier |name|
//@[6:7)     Colon |:|
//@[8:15)     VariableAccessSyntax
//@[8:15)      IdentifierSyntax
//@[8:15)       Identifier |account|
//@[15:17)    NewLine |\r\n|
  location: 'eastus42'
//@[2:22)    ObjectPropertySyntax
//@[2:10)     IdentifierSyntax
//@[2:10)      Identifier |location|
//@[10:11)     Colon |:|
//@[12:22)     StringSyntax
//@[12:22)      StringComplete |'eastus42'|
//@[22:24)    NewLine |\r\n|
  properties: {
//@[2:39)    ObjectPropertySyntax
//@[2:12)     IdentifierSyntax
//@[2:12)      Identifier |properties|
//@[12:13)     Colon |:|
//@[14:39)     ObjectSyntax
//@[14:15)      LeftBrace |{|
//@[15:17)      NewLine |\r\n|
    wrong: 'test'
//@[4:17)      ObjectPropertySyntax
//@[4:9)       IdentifierSyntax
//@[4:9)        Identifier |wrong|
//@[9:10)       Colon |:|
//@[11:17)       StringSyntax
//@[11:17)        StringComplete |'test'|
//@[17:19)      NewLine |\r\n|
  }
//@[2:3)      RightBrace |}|
//@[3:5)    NewLine |\r\n|
}]
//@[0:1)    RightBrace |}|
//@[1:2)   RightSquare |]|
//@[2:6) NewLine |\r\n\r\n|

// wrong property inside the nested property loop
//@[49:51) NewLine |\r\n|
resource wrongPropertyInNestedLoop 'Microsoft.Network/virtualNetworks@2020-06-01' = [for i in range(0, 3): {
//@[0:262) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:34)  IdentifierSyntax
//@[9:34)   Identifier |wrongPropertyInNestedLoop|
//@[35:81)  StringSyntax
//@[35:81)   StringComplete |'Microsoft.Network/virtualNetworks@2020-06-01'|
//@[82:83)  Assignment |=|
//@[84:262)  ForSyntax
//@[84:85)   LeftSquare |[|
//@[85:88)   Identifier |for|
//@[89:90)   LocalVariableSyntax
//@[89:90)    IdentifierSyntax
//@[89:90)     Identifier |i|
//@[91:93)   Identifier |in|
//@[94:105)   FunctionCallSyntax
//@[94:99)    IdentifierSyntax
//@[94:99)     Identifier |range|
//@[99:100)    LeftParen |(|
//@[100:102)    FunctionArgumentSyntax
//@[100:101)     IntegerLiteralSyntax
//@[100:101)      Integer |0|
//@[101:102)     Comma |,|
//@[103:104)    FunctionArgumentSyntax
//@[103:104)     IntegerLiteralSyntax
//@[103:104)      Integer |3|
//@[104:105)    RightParen |)|
//@[105:106)   Colon |:|
//@[107:261)   ObjectSyntax
//@[107:108)    LeftBrace |{|
//@[108:110)    NewLine |\r\n|
  name: 'vnet-${i}'
//@[2:19)    ObjectPropertySyntax
//@[2:6)     IdentifierSyntax
//@[2:6)      Identifier |name|
//@[6:7)     Colon |:|
//@[8:19)     StringSyntax
//@[8:16)      StringLeftPiece |'vnet-${|
//@[16:17)      VariableAccessSyntax
//@[16:17)       IdentifierSyntax
//@[16:17)        Identifier |i|
//@[17:19)      StringRightPiece |}'|
//@[19:21)    NewLine |\r\n|
  properties: {
//@[2:127)    ObjectPropertySyntax
//@[2:12)     IdentifierSyntax
//@[2:12)      Identifier |properties|
//@[12:13)     Colon |:|
//@[14:127)     ObjectSyntax
//@[14:15)      LeftBrace |{|
//@[15:17)      NewLine |\r\n|
    subnets: [for j in range(0, 4): {
//@[4:105)      ObjectPropertySyntax
//@[4:11)       IdentifierSyntax
//@[4:11)        Identifier |subnets|
//@[11:12)       Colon |:|
//@[13:105)       ForSyntax
//@[13:14)        LeftSquare |[|
//@[14:17)        Identifier |for|
//@[18:19)        LocalVariableSyntax
//@[18:19)         IdentifierSyntax
//@[18:19)          Identifier |j|
//@[20:22)        Identifier |in|
//@[23:34)        FunctionCallSyntax
//@[23:28)         IdentifierSyntax
//@[23:28)          Identifier |range|
//@[28:29)         LeftParen |(|
//@[29:31)         FunctionArgumentSyntax
//@[29:30)          IntegerLiteralSyntax
//@[29:30)           Integer |0|
//@[30:31)          Comma |,|
//@[32:33)         FunctionArgumentSyntax
//@[32:33)          IntegerLiteralSyntax
//@[32:33)           Integer |4|
//@[33:34)         RightParen |)|
//@[34:35)        Colon |:|
//@[36:104)        ObjectSyntax
//@[36:37)         LeftBrace |{|
//@[37:39)         NewLine |\r\n|
      doesNotExist: 'test'
//@[6:26)         ObjectPropertySyntax
//@[6:18)          IdentifierSyntax
//@[6:18)           Identifier |doesNotExist|
//@[18:19)          Colon |:|
//@[20:26)          StringSyntax
//@[20:26)           StringComplete |'test'|
//@[26:28)         NewLine |\r\n|
      name: 'subnet-${i}-${j}'
//@[6:30)         ObjectPropertySyntax
//@[6:10)          IdentifierSyntax
//@[6:10)           Identifier |name|
//@[10:11)          Colon |:|
//@[12:30)          StringSyntax
//@[12:22)           StringLeftPiece |'subnet-${|
//@[22:23)           VariableAccessSyntax
//@[22:23)            IdentifierSyntax
//@[22:23)             Identifier |i|
//@[23:27)           StringMiddlePiece |}-${|
//@[27:28)           VariableAccessSyntax
//@[27:28)            IdentifierSyntax
//@[27:28)             Identifier |j|
//@[28:30)           StringRightPiece |}'|
//@[30:32)         NewLine |\r\n|
    }]
//@[4:5)         RightBrace |}|
//@[5:6)        RightSquare |]|
//@[6:8)      NewLine |\r\n|
  }
//@[2:3)      RightBrace |}|
//@[3:5)    NewLine |\r\n|
}]
//@[0:1)    RightBrace |}|
//@[1:2)   RightSquare |]|
//@[2:4) NewLine |\r\n|
resource wrongPropertyInNestedLoop2 'Microsoft.Network/virtualNetworks@2020-06-01' = [for (i,k) in range(0, 3): {
//@[0:272) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:35)  IdentifierSyntax
//@[9:35)   Identifier |wrongPropertyInNestedLoop2|
//@[36:82)  StringSyntax
//@[36:82)   StringComplete |'Microsoft.Network/virtualNetworks@2020-06-01'|
//@[83:84)  Assignment |=|
//@[85:272)  ForSyntax
//@[85:86)   LeftSquare |[|
//@[86:89)   Identifier |for|
//@[90:95)   ForVariableBlockSyntax
//@[90:91)    LeftParen |(|
//@[91:92)    LocalVariableSyntax
//@[91:92)     IdentifierSyntax
//@[91:92)      Identifier |i|
//@[92:93)    Comma |,|
//@[93:94)    LocalVariableSyntax
//@[93:94)     IdentifierSyntax
//@[93:94)      Identifier |k|
//@[94:95)    RightParen |)|
//@[96:98)   Identifier |in|
//@[99:110)   FunctionCallSyntax
//@[99:104)    IdentifierSyntax
//@[99:104)     Identifier |range|
//@[104:105)    LeftParen |(|
//@[105:107)    FunctionArgumentSyntax
//@[105:106)     IntegerLiteralSyntax
//@[105:106)      Integer |0|
//@[106:107)     Comma |,|
//@[108:109)    FunctionArgumentSyntax
//@[108:109)     IntegerLiteralSyntax
//@[108:109)      Integer |3|
//@[109:110)    RightParen |)|
//@[110:111)   Colon |:|
//@[112:271)   ObjectSyntax
//@[112:113)    LeftBrace |{|
//@[113:115)    NewLine |\r\n|
  name: 'vnet-${i}'
//@[2:19)    ObjectPropertySyntax
//@[2:6)     IdentifierSyntax
//@[2:6)      Identifier |name|
//@[6:7)     Colon |:|
//@[8:19)     StringSyntax
//@[8:16)      StringLeftPiece |'vnet-${|
//@[16:17)      VariableAccessSyntax
//@[16:17)       IdentifierSyntax
//@[16:17)        Identifier |i|
//@[17:19)      StringRightPiece |}'|
//@[19:21)    NewLine |\r\n|
  properties: {
//@[2:132)    ObjectPropertySyntax
//@[2:12)     IdentifierSyntax
//@[2:12)      Identifier |properties|
//@[12:13)     Colon |:|
//@[14:132)     ObjectSyntax
//@[14:15)      LeftBrace |{|
//@[15:17)      NewLine |\r\n|
    subnets: [for j in range(0, 4): {
//@[4:110)      ObjectPropertySyntax
//@[4:11)       IdentifierSyntax
//@[4:11)        Identifier |subnets|
//@[11:12)       Colon |:|
//@[13:110)       ForSyntax
//@[13:14)        LeftSquare |[|
//@[14:17)        Identifier |for|
//@[18:19)        LocalVariableSyntax
//@[18:19)         IdentifierSyntax
//@[18:19)          Identifier |j|
//@[20:22)        Identifier |in|
//@[23:34)        FunctionCallSyntax
//@[23:28)         IdentifierSyntax
//@[23:28)          Identifier |range|
//@[28:29)         LeftParen |(|
//@[29:31)         FunctionArgumentSyntax
//@[29:30)          IntegerLiteralSyntax
//@[29:30)           Integer |0|
//@[30:31)          Comma |,|
//@[32:33)         FunctionArgumentSyntax
//@[32:33)          IntegerLiteralSyntax
//@[32:33)           Integer |4|
//@[33:34)         RightParen |)|
//@[34:35)        Colon |:|
//@[36:109)        ObjectSyntax
//@[36:37)         LeftBrace |{|
//@[37:39)         NewLine |\r\n|
      doesNotExist: 'test'
//@[6:26)         ObjectPropertySyntax
//@[6:18)          IdentifierSyntax
//@[6:18)           Identifier |doesNotExist|
//@[18:19)          Colon |:|
//@[20:26)          StringSyntax
//@[20:26)           StringComplete |'test'|
//@[26:28)         NewLine |\r\n|
      name: 'subnet-${i}-${j}-${k}'
//@[6:35)         ObjectPropertySyntax
//@[6:10)          IdentifierSyntax
//@[6:10)           Identifier |name|
//@[10:11)          Colon |:|
//@[12:35)          StringSyntax
//@[12:22)           StringLeftPiece |'subnet-${|
//@[22:23)           VariableAccessSyntax
//@[22:23)            IdentifierSyntax
//@[22:23)             Identifier |i|
//@[23:27)           StringMiddlePiece |}-${|
//@[27:28)           VariableAccessSyntax
//@[27:28)            IdentifierSyntax
//@[27:28)             Identifier |j|
//@[28:32)           StringMiddlePiece |}-${|
//@[32:33)           VariableAccessSyntax
//@[32:33)            IdentifierSyntax
//@[32:33)             Identifier |k|
//@[33:35)           StringRightPiece |}'|
//@[35:37)         NewLine |\r\n|
    }]
//@[4:5)         RightBrace |}|
//@[5:6)        RightSquare |]|
//@[6:8)      NewLine |\r\n|
  }
//@[2:3)      RightBrace |}|
//@[3:5)    NewLine |\r\n|
}]
//@[0:1)    RightBrace |}|
//@[1:2)   RightSquare |]|
//@[2:6) NewLine |\r\n\r\n|

// nonexistent arrays and loop variables
//@[40:42) NewLine |\r\n|
resource nonexistentArrays 'Microsoft.Network/virtualNetworks@2020-06-01' = [for i in notAThing: {
//@[0:280) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:26)  IdentifierSyntax
//@[9:26)   Identifier |nonexistentArrays|
//@[27:73)  StringSyntax
//@[27:73)   StringComplete |'Microsoft.Network/virtualNetworks@2020-06-01'|
//@[74:75)  Assignment |=|
//@[76:280)  ForSyntax
//@[76:77)   LeftSquare |[|
//@[77:80)   Identifier |for|
//@[81:82)   LocalVariableSyntax
//@[81:82)    IdentifierSyntax
//@[81:82)     Identifier |i|
//@[83:85)   Identifier |in|
//@[86:95)   VariableAccessSyntax
//@[86:95)    IdentifierSyntax
//@[86:95)     Identifier |notAThing|
//@[95:96)   Colon |:|
//@[97:279)   ObjectSyntax
//@[97:98)    LeftBrace |{|
//@[98:100)    NewLine |\r\n|
  name: 'vnet-${justPlainWrong}'
//@[2:32)    ObjectPropertySyntax
//@[2:6)     IdentifierSyntax
//@[2:6)      Identifier |name|
//@[6:7)     Colon |:|
//@[8:32)     StringSyntax
//@[8:16)      StringLeftPiece |'vnet-${|
//@[16:30)      VariableAccessSyntax
//@[16:30)       IdentifierSyntax
//@[16:30)        Identifier |justPlainWrong|
//@[30:32)      StringRightPiece |}'|
//@[32:34)    NewLine |\r\n|
  properties: {
//@[2:142)    ObjectPropertySyntax
//@[2:12)     IdentifierSyntax
//@[2:12)      Identifier |properties|
//@[12:13)     Colon |:|
//@[14:142)     ObjectSyntax
//@[14:15)      LeftBrace |{|
//@[15:17)      NewLine |\r\n|
    subnets: [for j in alsoNotAThing: {
//@[4:120)      ObjectPropertySyntax
//@[4:11)       IdentifierSyntax
//@[4:11)        Identifier |subnets|
//@[11:12)       Colon |:|
//@[13:120)       ForSyntax
//@[13:14)        LeftSquare |[|
//@[14:17)        Identifier |for|
//@[18:19)        LocalVariableSyntax
//@[18:19)         IdentifierSyntax
//@[18:19)          Identifier |j|
//@[20:22)        Identifier |in|
//@[23:36)        VariableAccessSyntax
//@[23:36)         IdentifierSyntax
//@[23:36)          Identifier |alsoNotAThing|
//@[36:37)        Colon |:|
//@[38:119)        ObjectSyntax
//@[38:39)         LeftBrace |{|
//@[39:41)         NewLine |\r\n|
      doesNotExist: 'test'
//@[6:26)         ObjectPropertySyntax
//@[6:18)          IdentifierSyntax
//@[6:18)           Identifier |doesNotExist|
//@[18:19)          Colon |:|
//@[20:26)          StringSyntax
//@[20:26)           StringComplete |'test'|
//@[26:28)         NewLine |\r\n|
      name: 'subnet-${fake}-${totallyFake}'
//@[6:43)         ObjectPropertySyntax
//@[6:10)          IdentifierSyntax
//@[6:10)           Identifier |name|
//@[10:11)          Colon |:|
//@[12:43)          StringSyntax
//@[12:22)           StringLeftPiece |'subnet-${|
//@[22:26)           VariableAccessSyntax
//@[22:26)            IdentifierSyntax
//@[22:26)             Identifier |fake|
//@[26:30)           StringMiddlePiece |}-${|
//@[30:41)           VariableAccessSyntax
//@[30:41)            IdentifierSyntax
//@[30:41)             Identifier |totallyFake|
//@[41:43)           StringRightPiece |}'|
//@[43:45)         NewLine |\r\n|
    }]
//@[4:5)         RightBrace |}|
//@[5:6)        RightSquare |]|
//@[6:8)      NewLine |\r\n|
  }
//@[2:3)      RightBrace |}|
//@[3:5)    NewLine |\r\n|
}]
//@[0:1)    RightBrace |}|
//@[1:2)   RightSquare |]|
//@[2:6) NewLine |\r\n\r\n|

// property loops cannot be nested
//@[34:36) NewLine |\r\n|
resource propertyLoopsCannotNest 'Microsoft.Storage/storageAccounts@2019-06-01' = [for account in storageAccounts: {
//@[0:428) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:32)  IdentifierSyntax
//@[9:32)   Identifier |propertyLoopsCannotNest|
//@[33:79)  StringSyntax
//@[33:79)   StringComplete |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[80:81)  Assignment |=|
//@[82:428)  ForSyntax
//@[82:83)   LeftSquare |[|
//@[83:86)   Identifier |for|
//@[87:94)   LocalVariableSyntax
//@[87:94)    IdentifierSyntax
//@[87:94)     Identifier |account|
//@[95:97)   Identifier |in|
//@[98:113)   VariableAccessSyntax
//@[98:113)    IdentifierSyntax
//@[98:113)     Identifier |storageAccounts|
//@[113:114)   Colon |:|
//@[115:427)   ObjectSyntax
//@[115:116)    LeftBrace |{|
//@[116:118)    NewLine |\r\n|
  name: account.name
//@[2:20)    ObjectPropertySyntax
//@[2:6)     IdentifierSyntax
//@[2:6)      Identifier |name|
//@[6:7)     Colon |:|
//@[8:20)     PropertyAccessSyntax
//@[8:15)      VariableAccessSyntax
//@[8:15)       IdentifierSyntax
//@[8:15)        Identifier |account|
//@[15:16)      Dot |.|
//@[16:20)      IdentifierSyntax
//@[16:20)       Identifier |name|
//@[20:22)    NewLine |\r\n|
  location: account.location
//@[2:28)    ObjectPropertySyntax
//@[2:10)     IdentifierSyntax
//@[2:10)      Identifier |location|
//@[10:11)     Colon |:|
//@[12:28)     PropertyAccessSyntax
//@[12:19)      VariableAccessSyntax
//@[12:19)       IdentifierSyntax
//@[12:19)        Identifier |account|
//@[19:20)      Dot |.|
//@[20:28)      IdentifierSyntax
//@[20:28)       Identifier |location|
//@[28:30)    NewLine |\r\n|
  sku: {
//@[2:39)    ObjectPropertySyntax
//@[2:5)     IdentifierSyntax
//@[2:5)      Identifier |sku|
//@[5:6)     Colon |:|
//@[7:39)     ObjectSyntax
//@[7:8)      LeftBrace |{|
//@[8:10)      NewLine |\r\n|
    name: 'Standard_LRS'
//@[4:24)      ObjectPropertySyntax
//@[4:8)       IdentifierSyntax
//@[4:8)        Identifier |name|
//@[8:9)       Colon |:|
//@[10:24)       StringSyntax
//@[10:24)        StringComplete |'Standard_LRS'|
//@[24:26)      NewLine |\r\n|
  }
//@[2:3)      RightBrace |}|
//@[3:5)    NewLine |\r\n|
  kind: 'StorageV2'
//@[2:19)    ObjectPropertySyntax
//@[2:6)     IdentifierSyntax
//@[2:6)      Identifier |kind|
//@[6:7)     Colon |:|
//@[8:19)     StringSyntax
//@[8:19)      StringComplete |'StorageV2'|
//@[19:21)    NewLine |\r\n|
  properties: {
//@[2:192)    ObjectPropertySyntax
//@[2:12)     IdentifierSyntax
//@[2:12)      Identifier |properties|
//@[12:13)     Colon |:|
//@[14:192)     ObjectSyntax
//@[14:15)      LeftBrace |{|
//@[15:19)      NewLine |\r\n\r\n|

    networkAcls: {
//@[4:168)      ObjectPropertySyntax
//@[4:15)       IdentifierSyntax
//@[4:15)        Identifier |networkAcls|
//@[15:16)       Colon |:|
//@[17:168)       ObjectSyntax
//@[17:18)        LeftBrace |{|
//@[18:20)        NewLine |\r\n|
      virtualNetworkRules: [for rule in []: {
//@[6:141)        ObjectPropertySyntax
//@[6:25)         IdentifierSyntax
//@[6:25)          Identifier |virtualNetworkRules|
//@[25:26)         Colon |:|
//@[27:141)         ForSyntax
//@[27:28)          LeftSquare |[|
//@[28:31)          Identifier |for|
//@[32:36)          LocalVariableSyntax
//@[32:36)           IdentifierSyntax
//@[32:36)            Identifier |rule|
//@[37:39)          Identifier |in|
//@[40:42)          ArraySyntax
//@[40:41)           LeftSquare |[|
//@[41:42)           RightSquare |]|
//@[42:43)          Colon |:|
//@[44:140)          ObjectSyntax
//@[44:45)           LeftBrace |{|
//@[45:47)           NewLine |\r\n|
        id: '${account.name}-${account.location}'
//@[8:49)           ObjectPropertySyntax
//@[8:10)            IdentifierSyntax
//@[8:10)             Identifier |id|
//@[10:11)            Colon |:|
//@[12:49)            StringSyntax
//@[12:15)             StringLeftPiece |'${|
//@[15:27)             PropertyAccessSyntax
//@[15:22)              VariableAccessSyntax
//@[15:22)               IdentifierSyntax
//@[15:22)                Identifier |account|
//@[22:23)              Dot |.|
//@[23:27)              IdentifierSyntax
//@[23:27)               Identifier |name|
//@[27:31)             StringMiddlePiece |}-${|
//@[31:47)             PropertyAccessSyntax
//@[31:38)              VariableAccessSyntax
//@[31:38)               IdentifierSyntax
//@[31:38)                Identifier |account|
//@[38:39)              Dot |.|
//@[39:47)              IdentifierSyntax
//@[39:47)               Identifier |location|
//@[47:49)             StringRightPiece |}'|
//@[49:51)           NewLine |\r\n|
        state: [for lol in []: 4]
//@[8:33)           ObjectPropertySyntax
//@[8:13)            IdentifierSyntax
//@[8:13)             Identifier |state|
//@[13:14)            Colon |:|
//@[15:33)            ForSyntax
//@[15:16)             LeftSquare |[|
//@[16:19)             Identifier |for|
//@[20:23)             LocalVariableSyntax
//@[20:23)              IdentifierSyntax
//@[20:23)               Identifier |lol|
//@[24:26)             Identifier |in|
//@[27:29)             ArraySyntax
//@[27:28)              LeftSquare |[|
//@[28:29)              RightSquare |]|
//@[29:30)             Colon |:|
//@[31:32)             IntegerLiteralSyntax
//@[31:32)              Integer |4|
//@[32:33)             RightSquare |]|
//@[33:35)           NewLine |\r\n|
      }]
//@[6:7)           RightBrace |}|
//@[7:8)          RightSquare |]|
//@[8:10)        NewLine |\r\n|
    }
//@[4:5)        RightBrace |}|
//@[5:7)      NewLine |\r\n|
  }
//@[2:3)      RightBrace |}|
//@[3:5)    NewLine |\r\n|
}]
//@[0:1)    RightBrace |}|
//@[1:2)   RightSquare |]|
//@[2:4) NewLine |\r\n|
resource propertyLoopsCannotNest2 'Microsoft.Storage/storageAccounts@2019-06-01' = [for (account,i) in storageAccounts: {
//@[0:441) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:33)  IdentifierSyntax
//@[9:33)   Identifier |propertyLoopsCannotNest2|
//@[34:80)  StringSyntax
//@[34:80)   StringComplete |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[81:82)  Assignment |=|
//@[83:441)  ForSyntax
//@[83:84)   LeftSquare |[|
//@[84:87)   Identifier |for|
//@[88:99)   ForVariableBlockSyntax
//@[88:89)    LeftParen |(|
//@[89:96)    LocalVariableSyntax
//@[89:96)     IdentifierSyntax
//@[89:96)      Identifier |account|
//@[96:97)    Comma |,|
//@[97:98)    LocalVariableSyntax
//@[97:98)     IdentifierSyntax
//@[97:98)      Identifier |i|
//@[98:99)    RightParen |)|
//@[100:102)   Identifier |in|
//@[103:118)   VariableAccessSyntax
//@[103:118)    IdentifierSyntax
//@[103:118)     Identifier |storageAccounts|
//@[118:119)   Colon |:|
//@[120:440)   ObjectSyntax
//@[120:121)    LeftBrace |{|
//@[121:123)    NewLine |\r\n|
  name: account.name
//@[2:20)    ObjectPropertySyntax
//@[2:6)     IdentifierSyntax
//@[2:6)      Identifier |name|
//@[6:7)     Colon |:|
//@[8:20)     PropertyAccessSyntax
//@[8:15)      VariableAccessSyntax
//@[8:15)       IdentifierSyntax
//@[8:15)        Identifier |account|
//@[15:16)      Dot |.|
//@[16:20)      IdentifierSyntax
//@[16:20)       Identifier |name|
//@[20:22)    NewLine |\r\n|
  location: account.location
//@[2:28)    ObjectPropertySyntax
//@[2:10)     IdentifierSyntax
//@[2:10)      Identifier |location|
//@[10:11)     Colon |:|
//@[12:28)     PropertyAccessSyntax
//@[12:19)      VariableAccessSyntax
//@[12:19)       IdentifierSyntax
//@[12:19)        Identifier |account|
//@[19:20)      Dot |.|
//@[20:28)      IdentifierSyntax
//@[20:28)       Identifier |location|
//@[28:30)    NewLine |\r\n|
  sku: {
//@[2:39)    ObjectPropertySyntax
//@[2:5)     IdentifierSyntax
//@[2:5)      Identifier |sku|
//@[5:6)     Colon |:|
//@[7:39)     ObjectSyntax
//@[7:8)      LeftBrace |{|
//@[8:10)      NewLine |\r\n|
    name: 'Standard_LRS'
//@[4:24)      ObjectPropertySyntax
//@[4:8)       IdentifierSyntax
//@[4:8)        Identifier |name|
//@[8:9)       Colon |:|
//@[10:24)       StringSyntax
//@[10:24)        StringComplete |'Standard_LRS'|
//@[24:26)      NewLine |\r\n|
  }
//@[2:3)      RightBrace |}|
//@[3:5)    NewLine |\r\n|
  kind: 'StorageV2'
//@[2:19)    ObjectPropertySyntax
//@[2:6)     IdentifierSyntax
//@[2:6)      Identifier |kind|
//@[6:7)     Colon |:|
//@[8:19)     StringSyntax
//@[8:19)      StringComplete |'StorageV2'|
//@[19:21)    NewLine |\r\n|
  properties: {
//@[2:200)    ObjectPropertySyntax
//@[2:12)     IdentifierSyntax
//@[2:12)      Identifier |properties|
//@[12:13)     Colon |:|
//@[14:200)     ObjectSyntax
//@[14:15)      LeftBrace |{|
//@[15:19)      NewLine |\r\n\r\n|

    networkAcls: {
//@[4:176)      ObjectPropertySyntax
//@[4:15)       IdentifierSyntax
//@[4:15)        Identifier |networkAcls|
//@[15:16)       Colon |:|
//@[17:176)       ObjectSyntax
//@[17:18)        LeftBrace |{|
//@[18:20)        NewLine |\r\n|
      virtualNetworkRules: [for (rule,j) in []: {
//@[6:149)        ObjectPropertySyntax
//@[6:25)         IdentifierSyntax
//@[6:25)          Identifier |virtualNetworkRules|
//@[25:26)         Colon |:|
//@[27:149)         ForSyntax
//@[27:28)          LeftSquare |[|
//@[28:31)          Identifier |for|
//@[32:40)          ForVariableBlockSyntax
//@[32:33)           LeftParen |(|
//@[33:37)           LocalVariableSyntax
//@[33:37)            IdentifierSyntax
//@[33:37)             Identifier |rule|
//@[37:38)           Comma |,|
//@[38:39)           LocalVariableSyntax
//@[38:39)            IdentifierSyntax
//@[38:39)             Identifier |j|
//@[39:40)           RightParen |)|
//@[41:43)          Identifier |in|
//@[44:46)          ArraySyntax
//@[44:45)           LeftSquare |[|
//@[45:46)           RightSquare |]|
//@[46:47)          Colon |:|
//@[48:148)          ObjectSyntax
//@[48:49)           LeftBrace |{|
//@[49:51)           NewLine |\r\n|
        id: '${account.name}-${account.location}'
//@[8:49)           ObjectPropertySyntax
//@[8:10)            IdentifierSyntax
//@[8:10)             Identifier |id|
//@[10:11)            Colon |:|
//@[12:49)            StringSyntax
//@[12:15)             StringLeftPiece |'${|
//@[15:27)             PropertyAccessSyntax
//@[15:22)              VariableAccessSyntax
//@[15:22)               IdentifierSyntax
//@[15:22)                Identifier |account|
//@[22:23)              Dot |.|
//@[23:27)              IdentifierSyntax
//@[23:27)               Identifier |name|
//@[27:31)             StringMiddlePiece |}-${|
//@[31:47)             PropertyAccessSyntax
//@[31:38)              VariableAccessSyntax
//@[31:38)               IdentifierSyntax
//@[31:38)                Identifier |account|
//@[38:39)              Dot |.|
//@[39:47)              IdentifierSyntax
//@[39:47)               Identifier |location|
//@[47:49)             StringRightPiece |}'|
//@[49:51)           NewLine |\r\n|
        state: [for (lol,k) in []: 4]
//@[8:37)           ObjectPropertySyntax
//@[8:13)            IdentifierSyntax
//@[8:13)             Identifier |state|
//@[13:14)            Colon |:|
//@[15:37)            ForSyntax
//@[15:16)             LeftSquare |[|
//@[16:19)             Identifier |for|
//@[20:27)             ForVariableBlockSyntax
//@[20:21)              LeftParen |(|
//@[21:24)              LocalVariableSyntax
//@[21:24)               IdentifierSyntax
//@[21:24)                Identifier |lol|
//@[24:25)              Comma |,|
//@[25:26)              LocalVariableSyntax
//@[25:26)               IdentifierSyntax
//@[25:26)                Identifier |k|
//@[26:27)              RightParen |)|
//@[28:30)             Identifier |in|
//@[31:33)             ArraySyntax
//@[31:32)              LeftSquare |[|
//@[32:33)              RightSquare |]|
//@[33:34)             Colon |:|
//@[35:36)             IntegerLiteralSyntax
//@[35:36)              Integer |4|
//@[36:37)             RightSquare |]|
//@[37:39)           NewLine |\r\n|
      }]
//@[6:7)           RightBrace |}|
//@[7:8)          RightSquare |]|
//@[8:10)        NewLine |\r\n|
    }
//@[4:5)        RightBrace |}|
//@[5:7)      NewLine |\r\n|
  }
//@[2:3)      RightBrace |}|
//@[3:5)    NewLine |\r\n|
}]
//@[0:1)    RightBrace |}|
//@[1:2)   RightSquare |]|
//@[2:6) NewLine |\r\n\r\n|

// property loops cannot be nested (even more nesting)
//@[54:56) NewLine |\r\n|
resource propertyLoopsCannotNest2 'Microsoft.Storage/storageAccounts@2019-06-01' = [for account in storageAccounts: {
//@[0:720) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:33)  IdentifierSyntax
//@[9:33)   Identifier |propertyLoopsCannotNest2|
//@[34:80)  StringSyntax
//@[34:80)   StringComplete |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[81:82)  Assignment |=|
//@[83:720)  ForSyntax
//@[83:84)   LeftSquare |[|
//@[84:87)   Identifier |for|
//@[88:95)   LocalVariableSyntax
//@[88:95)    IdentifierSyntax
//@[88:95)     Identifier |account|
//@[96:98)   Identifier |in|
//@[99:114)   VariableAccessSyntax
//@[99:114)    IdentifierSyntax
//@[99:114)     Identifier |storageAccounts|
//@[114:115)   Colon |:|
//@[116:719)   ObjectSyntax
//@[116:117)    LeftBrace |{|
//@[117:119)    NewLine |\r\n|
  name: account.name
//@[2:20)    ObjectPropertySyntax
//@[2:6)     IdentifierSyntax
//@[2:6)      Identifier |name|
//@[6:7)     Colon |:|
//@[8:20)     PropertyAccessSyntax
//@[8:15)      VariableAccessSyntax
//@[8:15)       IdentifierSyntax
//@[8:15)        Identifier |account|
//@[15:16)      Dot |.|
//@[16:20)      IdentifierSyntax
//@[16:20)       Identifier |name|
//@[20:22)    NewLine |\r\n|
  location: account.location
//@[2:28)    ObjectPropertySyntax
//@[2:10)     IdentifierSyntax
//@[2:10)      Identifier |location|
//@[10:11)     Colon |:|
//@[12:28)     PropertyAccessSyntax
//@[12:19)      VariableAccessSyntax
//@[12:19)       IdentifierSyntax
//@[12:19)        Identifier |account|
//@[19:20)      Dot |.|
//@[20:28)      IdentifierSyntax
//@[20:28)       Identifier |location|
//@[28:30)    NewLine |\r\n|
  sku: {
//@[2:39)    ObjectPropertySyntax
//@[2:5)     IdentifierSyntax
//@[2:5)      Identifier |sku|
//@[5:6)     Colon |:|
//@[7:39)     ObjectSyntax
//@[7:8)      LeftBrace |{|
//@[8:10)      NewLine |\r\n|
    name: 'Standard_LRS'
//@[4:24)      ObjectPropertySyntax
//@[4:8)       IdentifierSyntax
//@[4:8)        Identifier |name|
//@[8:9)       Colon |:|
//@[10:24)       StringSyntax
//@[10:24)        StringComplete |'Standard_LRS'|
//@[24:26)      NewLine |\r\n|
  }
//@[2:3)      RightBrace |}|
//@[3:5)    NewLine |\r\n|
  kind: 'StorageV2'
//@[2:19)    ObjectPropertySyntax
//@[2:6)     IdentifierSyntax
//@[2:6)      Identifier |kind|
//@[6:7)     Colon |:|
//@[8:19)     StringSyntax
//@[8:19)      StringComplete |'StorageV2'|
//@[19:21)    NewLine |\r\n|
  properties: {
//@[2:483)    ObjectPropertySyntax
//@[2:12)     IdentifierSyntax
//@[2:12)      Identifier |properties|
//@[12:13)     Colon |:|
//@[14:483)     ObjectSyntax
//@[14:15)      LeftBrace |{|
//@[15:17)      NewLine |\r\n|
    // #completionTest(17) -> symbolsPlusAccount
//@[48:50)      NewLine |\r\n|
    networkAcls: {
//@[4:411)      ObjectPropertySyntax
//@[4:15)       IdentifierSyntax
//@[4:15)        Identifier |networkAcls|
//@[15:16)       Colon |:|
//@[17:411)       ObjectSyntax
//@[17:18)        LeftBrace |{|
//@[18:20)        NewLine |\r\n|
      virtualNetworkRules: [for rule in []: {
//@[6:384)        ObjectPropertySyntax
//@[6:25)         IdentifierSyntax
//@[6:25)          Identifier |virtualNetworkRules|
//@[25:26)         Colon |:|
//@[27:384)         ForSyntax
//@[27:28)          LeftSquare |[|
//@[28:31)          Identifier |for|
//@[32:36)          LocalVariableSyntax
//@[32:36)           IdentifierSyntax
//@[32:36)            Identifier |rule|
//@[37:39)          Identifier |in|
//@[40:42)          ArraySyntax
//@[40:41)           LeftSquare |[|
//@[41:42)           RightSquare |]|
//@[42:43)          Colon |:|
//@[44:383)          ObjectSyntax
//@[44:45)           LeftBrace |{|
//@[45:47)           NewLine |\r\n|
        // #completionTest(12,15,31) -> symbolsPlusRule
//@[55:57)           NewLine |\r\n|
        id: '${account.name}-${account.location}'
//@[8:49)           ObjectPropertySyntax
//@[8:10)            IdentifierSyntax
//@[8:10)             Identifier |id|
//@[10:11)            Colon |:|
//@[12:49)            StringSyntax
//@[12:15)             StringLeftPiece |'${|
//@[15:27)             PropertyAccessSyntax
//@[15:22)              VariableAccessSyntax
//@[15:22)               IdentifierSyntax
//@[15:22)                Identifier |account|
//@[22:23)              Dot |.|
//@[23:27)              IdentifierSyntax
//@[23:27)               Identifier |name|
//@[27:31)             StringMiddlePiece |}-${|
//@[31:47)             PropertyAccessSyntax
//@[31:38)              VariableAccessSyntax
//@[31:38)               IdentifierSyntax
//@[31:38)                Identifier |account|
//@[38:39)              Dot |.|
//@[39:47)              IdentifierSyntax
//@[39:47)               Identifier |location|
//@[47:49)             StringRightPiece |}'|
//@[49:51)           NewLine |\r\n|
        state: [for state in []: {
//@[8:219)           ObjectPropertySyntax
//@[8:13)            IdentifierSyntax
//@[8:13)             Identifier |state|
//@[13:14)            Colon |:|
//@[15:219)            ForSyntax
//@[15:16)             LeftSquare |[|
//@[16:19)             Identifier |for|
//@[20:25)             LocalVariableSyntax
//@[20:25)              IdentifierSyntax
//@[20:25)               Identifier |state|
//@[26:28)             Identifier |in|
//@[29:31)             ArraySyntax
//@[29:30)              LeftSquare |[|
//@[30:31)              RightSquare |]|
//@[31:32)             Colon |:|
//@[33:218)             ObjectSyntax
//@[33:34)              LeftBrace |{|
//@[34:36)              NewLine |\r\n|
          // #completionTest(38) -> symbolsPlusAccountRuleStateSomething #completionTest(16,34) -> symbolsPlusAccountRuleState
//@[126:128)              NewLine |\r\n|
          fake: [for something in []: true]
//@[10:43)              ObjectPropertySyntax
//@[10:14)               IdentifierSyntax
//@[10:14)                Identifier |fake|
//@[14:15)               Colon |:|
//@[16:43)               ForSyntax
//@[16:17)                LeftSquare |[|
//@[17:20)                Identifier |for|
//@[21:30)                LocalVariableSyntax
//@[21:30)                 IdentifierSyntax
//@[21:30)                  Identifier |something|
//@[31:33)                Identifier |in|
//@[34:36)                ArraySyntax
//@[34:35)                 LeftSquare |[|
//@[35:36)                 RightSquare |]|
//@[36:37)                Colon |:|
//@[38:42)                BooleanLiteralSyntax
//@[38:42)                 TrueKeyword |true|
//@[42:43)                RightSquare |]|
//@[43:45)              NewLine |\r\n|
        }]
//@[8:9)              RightBrace |}|
//@[9:10)             RightSquare |]|
//@[10:12)           NewLine |\r\n|
      }]
//@[6:7)           RightBrace |}|
//@[7:8)          RightSquare |]|
//@[8:10)        NewLine |\r\n|
    }
//@[4:5)        RightBrace |}|
//@[5:7)      NewLine |\r\n|
  }
//@[2:3)      RightBrace |}|
//@[3:5)    NewLine |\r\n|
}]
//@[0:1)    RightBrace |}|
//@[1:2)   RightSquare |]|
//@[2:6) NewLine |\r\n\r\n|

// loops cannot be used inside of expressions
//@[45:47) NewLine |\r\n|
resource stuffs 'Microsoft.Storage/storageAccounts@2019-06-01' = [for account in storageAccounts: {
//@[0:381) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:15)  IdentifierSyntax
//@[9:15)   Identifier |stuffs|
//@[16:62)  StringSyntax
//@[16:62)   StringComplete |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[63:64)  Assignment |=|
//@[65:381)  ForSyntax
//@[65:66)   LeftSquare |[|
//@[66:69)   Identifier |for|
//@[70:77)   LocalVariableSyntax
//@[70:77)    IdentifierSyntax
//@[70:77)     Identifier |account|
//@[78:80)   Identifier |in|
//@[81:96)   VariableAccessSyntax
//@[81:96)    IdentifierSyntax
//@[81:96)     Identifier |storageAccounts|
//@[96:97)   Colon |:|
//@[98:380)   ObjectSyntax
//@[98:99)    LeftBrace |{|
//@[99:101)    NewLine |\r\n|
  name: account.name
//@[2:20)    ObjectPropertySyntax
//@[2:6)     IdentifierSyntax
//@[2:6)      Identifier |name|
//@[6:7)     Colon |:|
//@[8:20)     PropertyAccessSyntax
//@[8:15)      VariableAccessSyntax
//@[8:15)       IdentifierSyntax
//@[8:15)        Identifier |account|
//@[15:16)      Dot |.|
//@[16:20)      IdentifierSyntax
//@[16:20)       Identifier |name|
//@[20:22)    NewLine |\r\n|
  location: account.location
//@[2:28)    ObjectPropertySyntax
//@[2:10)     IdentifierSyntax
//@[2:10)      Identifier |location|
//@[10:11)     Colon |:|
//@[12:28)     PropertyAccessSyntax
//@[12:19)      VariableAccessSyntax
//@[12:19)       IdentifierSyntax
//@[12:19)        Identifier |account|
//@[19:20)      Dot |.|
//@[20:28)      IdentifierSyntax
//@[20:28)       Identifier |location|
//@[28:30)    NewLine |\r\n|
  sku: {
//@[2:39)    ObjectPropertySyntax
//@[2:5)     IdentifierSyntax
//@[2:5)      Identifier |sku|
//@[5:6)     Colon |:|
//@[7:39)     ObjectSyntax
//@[7:8)      LeftBrace |{|
//@[8:10)      NewLine |\r\n|
    name: 'Standard_LRS'
//@[4:24)      ObjectPropertySyntax
//@[4:8)       IdentifierSyntax
//@[4:8)        Identifier |name|
//@[8:9)       Colon |:|
//@[10:24)       StringSyntax
//@[10:24)        StringComplete |'Standard_LRS'|
//@[24:26)      NewLine |\r\n|
  }
//@[2:3)      RightBrace |}|
//@[3:5)    NewLine |\r\n|
  kind: 'StorageV2'
//@[2:19)    ObjectPropertySyntax
//@[2:6)     IdentifierSyntax
//@[2:6)      Identifier |kind|
//@[6:7)     Colon |:|
//@[8:19)     StringSyntax
//@[8:19)      StringComplete |'StorageV2'|
//@[19:21)    NewLine |\r\n|
  properties: {
//@[2:162)    ObjectPropertySyntax
//@[2:12)     IdentifierSyntax
//@[2:12)      Identifier |properties|
//@[12:13)     Colon |:|
//@[14:162)     ObjectSyntax
//@[14:15)      LeftBrace |{|
//@[15:17)      NewLine |\r\n|
    networkAcls: {
//@[4:140)      ObjectPropertySyntax
//@[4:15)       IdentifierSyntax
//@[4:15)        Identifier |networkAcls|
//@[15:16)       Colon |:|
//@[17:140)       ObjectSyntax
//@[17:18)        LeftBrace |{|
//@[18:20)        NewLine |\r\n|
      virtualNetworkRules: concat([for lol in []: {
//@[6:113)        ObjectPropertySyntax
//@[6:25)         IdentifierSyntax
//@[6:25)          Identifier |virtualNetworkRules|
//@[25:26)         Colon |:|
//@[27:113)         FunctionCallSyntax
//@[27:33)          IdentifierSyntax
//@[27:33)           Identifier |concat|
//@[33:34)          LeftParen |(|
//@[34:112)          FunctionArgumentSyntax
//@[34:112)           ForSyntax
//@[34:35)            LeftSquare |[|
//@[35:38)            Identifier |for|
//@[39:42)            LocalVariableSyntax
//@[39:42)             IdentifierSyntax
//@[39:42)              Identifier |lol|
//@[43:45)            Identifier |in|
//@[46:48)            ArraySyntax
//@[46:47)             LeftSquare |[|
//@[47:48)             RightSquare |]|
//@[48:49)            Colon |:|
//@[50:111)            ObjectSyntax
//@[50:51)             LeftBrace |{|
//@[51:53)             NewLine |\r\n|
        id: '${account.name}-${account.location}'
//@[8:49)             ObjectPropertySyntax
//@[8:10)              IdentifierSyntax
//@[8:10)               Identifier |id|
//@[10:11)              Colon |:|
//@[12:49)              StringSyntax
//@[12:15)               StringLeftPiece |'${|
//@[15:27)               PropertyAccessSyntax
//@[15:22)                VariableAccessSyntax
//@[15:22)                 IdentifierSyntax
//@[15:22)                  Identifier |account|
//@[22:23)                Dot |.|
//@[23:27)                IdentifierSyntax
//@[23:27)                 Identifier |name|
//@[27:31)               StringMiddlePiece |}-${|
//@[31:47)               PropertyAccessSyntax
//@[31:38)                VariableAccessSyntax
//@[31:38)                 IdentifierSyntax
//@[31:38)                  Identifier |account|
//@[38:39)                Dot |.|
//@[39:47)                IdentifierSyntax
//@[39:47)                 Identifier |location|
//@[47:49)               StringRightPiece |}'|
//@[49:51)             NewLine |\r\n|
      }])
//@[6:7)             RightBrace |}|
//@[7:8)            RightSquare |]|
//@[8:9)          RightParen |)|
//@[9:11)        NewLine |\r\n|
    }
//@[4:5)        RightBrace |}|
//@[5:7)      NewLine |\r\n|
  }
//@[2:3)      RightBrace |}|
//@[3:5)    NewLine |\r\n|
}]
//@[0:1)    RightBrace |}|
//@[1:2)   RightSquare |]|
//@[2:6) NewLine |\r\n\r\n|

// using the same loop variable in a new language scope should be allowed
//@[73:75) NewLine |\r\n|
resource premiumStorages 'Microsoft.Storage/storageAccounts@2019-06-01' = [for account in storageAccounts: {
//@[0:321) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:24)  IdentifierSyntax
//@[9:24)   Identifier |premiumStorages|
//@[25:71)  StringSyntax
//@[25:71)   StringComplete |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[72:73)  Assignment |=|
//@[74:321)  ForSyntax
//@[74:75)   LeftSquare |[|
//@[75:78)   Identifier |for|
//@[79:86)   LocalVariableSyntax
//@[79:86)    IdentifierSyntax
//@[79:86)     Identifier |account|
//@[87:89)   Identifier |in|
//@[90:105)   VariableAccessSyntax
//@[90:105)    IdentifierSyntax
//@[90:105)     Identifier |storageAccounts|
//@[105:106)   Colon |:|
//@[107:320)   ObjectSyntax
//@[107:108)    LeftBrace |{|
//@[108:110)    NewLine |\r\n|
  // #completionTest(7,8) -> symbolsPlusAccount2
//@[48:50)    NewLine |\r\n|
  name: account.name
//@[2:20)    ObjectPropertySyntax
//@[2:6)     IdentifierSyntax
//@[2:6)      Identifier |name|
//@[6:7)     Colon |:|
//@[8:20)     PropertyAccessSyntax
//@[8:15)      VariableAccessSyntax
//@[8:15)       IdentifierSyntax
//@[8:15)        Identifier |account|
//@[15:16)      Dot |.|
//@[16:20)      IdentifierSyntax
//@[16:20)       Identifier |name|
//@[20:22)    NewLine |\r\n|
  location: account.location
//@[2:28)    ObjectPropertySyntax
//@[2:10)     IdentifierSyntax
//@[2:10)      Identifier |location|
//@[10:11)     Colon |:|
//@[12:28)     PropertyAccessSyntax
//@[12:19)      VariableAccessSyntax
//@[12:19)       IdentifierSyntax
//@[12:19)        Identifier |account|
//@[19:20)      Dot |.|
//@[20:28)      IdentifierSyntax
//@[20:28)       Identifier |location|
//@[28:30)    NewLine |\r\n|
  sku: {
//@[2:84)    ObjectPropertySyntax
//@[2:5)     IdentifierSyntax
//@[2:5)      Identifier |sku|
//@[5:6)     Colon |:|
//@[7:84)     ObjectSyntax
//@[7:8)      LeftBrace |{|
//@[8:10)      NewLine |\r\n|
    // #completionTest(9,10) -> storageSkuNamePlusSymbols
//@[57:59)      NewLine |\r\n|
    name: 
//@[4:10)      ObjectPropertySyntax
//@[4:8)       IdentifierSyntax
//@[4:8)        Identifier |name|
//@[8:9)       Colon |:|
//@[10:10)       SkippedTriviaSyntax
//@[10:12)      NewLine |\r\n|
  }
//@[2:3)      RightBrace |}|
//@[3:5)    NewLine |\r\n|
  kind: 'StorageV2'
//@[2:19)    ObjectPropertySyntax
//@[2:6)     IdentifierSyntax
//@[2:6)      Identifier |kind|
//@[6:7)     Colon |:|
//@[8:19)     StringSyntax
//@[8:19)      StringComplete |'StorageV2'|
//@[19:21)    NewLine |\r\n|
}]
//@[0:1)    RightBrace |}|
//@[1:2)   RightSquare |]|
//@[2:6) NewLine |\r\n\r\n|

var directRefViaVar = premiumStorages
//@[0:37) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:19)  IdentifierSyntax
//@[4:19)   Identifier |directRefViaVar|
//@[20:21)  Assignment |=|
//@[22:37)  VariableAccessSyntax
//@[22:37)   IdentifierSyntax
//@[22:37)    Identifier |premiumStorages|
//@[37:39) NewLine |\r\n|
output directRefViaOutput array = union(premiumStorages, stuffs)
//@[0:64) OutputDeclarationSyntax
//@[0:6)  Identifier |output|
//@[7:25)  IdentifierSyntax
//@[7:25)   Identifier |directRefViaOutput|
//@[26:31)  TypeSyntax
//@[26:31)   Identifier |array|
//@[32:33)  Assignment |=|
//@[34:64)  FunctionCallSyntax
//@[34:39)   IdentifierSyntax
//@[34:39)    Identifier |union|
//@[39:40)   LeftParen |(|
//@[40:56)   FunctionArgumentSyntax
//@[40:55)    VariableAccessSyntax
//@[40:55)     IdentifierSyntax
//@[40:55)      Identifier |premiumStorages|
//@[55:56)    Comma |,|
//@[57:63)   FunctionArgumentSyntax
//@[57:63)    VariableAccessSyntax
//@[57:63)     IdentifierSyntax
//@[57:63)      Identifier |stuffs|
//@[63:64)   RightParen |)|
//@[64:68) NewLine |\r\n\r\n|

resource directRefViaSingleResourceBody 'Microsoft.Network/dnszones@2018-05-01' = {
//@[0:199) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:39)  IdentifierSyntax
//@[9:39)   Identifier |directRefViaSingleResourceBody|
//@[40:79)  StringSyntax
//@[40:79)   StringComplete |'Microsoft.Network/dnszones@2018-05-01'|
//@[80:81)  Assignment |=|
//@[82:199)  ObjectSyntax
//@[82:83)   LeftBrace |{|
//@[83:85)   NewLine |\r\n|
  name: 'myZone2'
//@[2:17)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:17)    StringSyntax
//@[8:17)     StringComplete |'myZone2'|
//@[17:19)   NewLine |\r\n|
  location: 'global'
//@[2:20)   ObjectPropertySyntax
//@[2:10)    IdentifierSyntax
//@[2:10)     Identifier |location|
//@[10:11)    Colon |:|
//@[12:20)    StringSyntax
//@[12:20)     StringComplete |'global'|
//@[20:22)   NewLine |\r\n|
  properties: {
//@[2:70)   ObjectPropertySyntax
//@[2:12)    IdentifierSyntax
//@[2:12)     Identifier |properties|
//@[12:13)    Colon |:|
//@[14:70)    ObjectSyntax
//@[14:15)     LeftBrace |{|
//@[15:17)     NewLine |\r\n|
    registrationVirtualNetworks: premiumStorages
//@[4:48)     ObjectPropertySyntax
//@[4:31)      IdentifierSyntax
//@[4:31)       Identifier |registrationVirtualNetworks|
//@[31:32)      Colon |:|
//@[33:48)      VariableAccessSyntax
//@[33:48)       IdentifierSyntax
//@[33:48)        Identifier |premiumStorages|
//@[48:50)     NewLine |\r\n|
  }
//@[2:3)     RightBrace |}|
//@[3:5)   NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource directRefViaSingleConditionalResourceBody 'Microsoft.Network/dnszones@2018-05-01' = if(true) {
//@[0:235) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:50)  IdentifierSyntax
//@[9:50)   Identifier |directRefViaSingleConditionalResourceBody|
//@[51:90)  StringSyntax
//@[51:90)   StringComplete |'Microsoft.Network/dnszones@2018-05-01'|
//@[91:92)  Assignment |=|
//@[93:235)  IfConditionSyntax
//@[93:95)   Identifier |if|
//@[95:101)   ParenthesizedExpressionSyntax
//@[95:96)    LeftParen |(|
//@[96:100)    BooleanLiteralSyntax
//@[96:100)     TrueKeyword |true|
//@[100:101)    RightParen |)|
//@[102:235)   ObjectSyntax
//@[102:103)    LeftBrace |{|
//@[103:105)    NewLine |\r\n|
  name: 'myZone3'
//@[2:17)    ObjectPropertySyntax
//@[2:6)     IdentifierSyntax
//@[2:6)      Identifier |name|
//@[6:7)     Colon |:|
//@[8:17)     StringSyntax
//@[8:17)      StringComplete |'myZone3'|
//@[17:19)    NewLine |\r\n|
  location: 'global'
//@[2:20)    ObjectPropertySyntax
//@[2:10)     IdentifierSyntax
//@[2:10)      Identifier |location|
//@[10:11)     Colon |:|
//@[12:20)     StringSyntax
//@[12:20)      StringComplete |'global'|
//@[20:22)    NewLine |\r\n|
  properties: {
//@[2:86)    ObjectPropertySyntax
//@[2:12)     IdentifierSyntax
//@[2:12)      Identifier |properties|
//@[12:13)     Colon |:|
//@[14:86)     ObjectSyntax
//@[14:15)      LeftBrace |{|
//@[15:17)      NewLine |\r\n|
    registrationVirtualNetworks: concat(premiumStorages, stuffs)
//@[4:64)      ObjectPropertySyntax
//@[4:31)       IdentifierSyntax
//@[4:31)        Identifier |registrationVirtualNetworks|
//@[31:32)       Colon |:|
//@[33:64)       FunctionCallSyntax
//@[33:39)        IdentifierSyntax
//@[33:39)         Identifier |concat|
//@[39:40)        LeftParen |(|
//@[40:56)        FunctionArgumentSyntax
//@[40:55)         VariableAccessSyntax
//@[40:55)          IdentifierSyntax
//@[40:55)           Identifier |premiumStorages|
//@[55:56)         Comma |,|
//@[57:63)        FunctionArgumentSyntax
//@[57:63)         VariableAccessSyntax
//@[57:63)          IdentifierSyntax
//@[57:63)           Identifier |stuffs|
//@[63:64)        RightParen |)|
//@[64:66)      NewLine |\r\n|
  }
//@[2:3)      RightBrace |}|
//@[3:5)    NewLine |\r\n|
}
//@[0:1)    RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

@batchSize()
//@[0:208) ResourceDeclarationSyntax
//@[0:12)  DecoratorSyntax
//@[0:1)   At |@|
//@[1:12)   FunctionCallSyntax
//@[1:10)    IdentifierSyntax
//@[1:10)     Identifier |batchSize|
//@[10:11)    LeftParen |(|
//@[11:12)    RightParen |)|
//@[12:14)  NewLine |\r\n|
resource directRefViaSingleLoopResourceBody 'Microsoft.Network/virtualNetworks@2020-06-01' = [for i in range(0, 3): {
//@[0:8)  Identifier |resource|
//@[9:43)  IdentifierSyntax
//@[9:43)   Identifier |directRefViaSingleLoopResourceBody|
//@[44:90)  StringSyntax
//@[44:90)   StringComplete |'Microsoft.Network/virtualNetworks@2020-06-01'|
//@[91:92)  Assignment |=|
//@[93:194)  ForSyntax
//@[93:94)   LeftSquare |[|
//@[94:97)   Identifier |for|
//@[98:99)   LocalVariableSyntax
//@[98:99)    IdentifierSyntax
//@[98:99)     Identifier |i|
//@[100:102)   Identifier |in|
//@[103:114)   FunctionCallSyntax
//@[103:108)    IdentifierSyntax
//@[103:108)     Identifier |range|
//@[108:109)    LeftParen |(|
//@[109:111)    FunctionArgumentSyntax
//@[109:110)     IntegerLiteralSyntax
//@[109:110)      Integer |0|
//@[110:111)     Comma |,|
//@[112:113)    FunctionArgumentSyntax
//@[112:113)     IntegerLiteralSyntax
//@[112:113)      Integer |3|
//@[113:114)    RightParen |)|
//@[114:115)   Colon |:|
//@[116:193)   ObjectSyntax
//@[116:117)    LeftBrace |{|
//@[117:119)    NewLine |\r\n|
  name: 'vnet-${i}'
//@[2:19)    ObjectPropertySyntax
//@[2:6)     IdentifierSyntax
//@[2:6)      Identifier |name|
//@[6:7)     Colon |:|
//@[8:19)     StringSyntax
//@[8:16)      StringLeftPiece |'vnet-${|
//@[16:17)      VariableAccessSyntax
//@[16:17)       IdentifierSyntax
//@[16:17)        Identifier |i|
//@[17:19)      StringRightPiece |}'|
//@[19:21)    NewLine |\r\n|
  properties: {
//@[2:50)    ObjectPropertySyntax
//@[2:12)     IdentifierSyntax
//@[2:12)      Identifier |properties|
//@[12:13)     Colon |:|
//@[14:50)     ObjectSyntax
//@[14:15)      LeftBrace |{|
//@[15:17)      NewLine |\r\n|
    subnets: premiumStorages
//@[4:28)      ObjectPropertySyntax
//@[4:11)       IdentifierSyntax
//@[4:11)        Identifier |subnets|
//@[11:12)       Colon |:|
//@[13:28)       VariableAccessSyntax
//@[13:28)        IdentifierSyntax
//@[13:28)         Identifier |premiumStorages|
//@[28:30)      NewLine |\r\n|
  }
//@[2:3)      RightBrace |}|
//@[3:5)    NewLine |\r\n|
}]
//@[0:1)    RightBrace |}|
//@[1:2)   RightSquare |]|
//@[2:6) NewLine |\r\n\r\n|

@batchSize(0)
//@[0:302) ResourceDeclarationSyntax
//@[0:13)  DecoratorSyntax
//@[0:1)   At |@|
//@[1:13)   FunctionCallSyntax
//@[1:10)    IdentifierSyntax
//@[1:10)     Identifier |batchSize|
//@[10:11)    LeftParen |(|
//@[11:12)    FunctionArgumentSyntax
//@[11:12)     IntegerLiteralSyntax
//@[11:12)      Integer |0|
//@[12:13)    RightParen |)|
//@[13:15)  NewLine |\r\n|
resource directRefViaSingleLoopResourceBodyWithExtraDependsOn 'Microsoft.Network/virtualNetworks@2020-06-01' = [for i in range(0, 3): {
//@[0:8)  Identifier |resource|
//@[9:61)  IdentifierSyntax
//@[9:61)   Identifier |directRefViaSingleLoopResourceBodyWithExtraDependsOn|
//@[62:108)  StringSyntax
//@[62:108)   StringComplete |'Microsoft.Network/virtualNetworks@2020-06-01'|
//@[109:110)  Assignment |=|
//@[111:287)  ForSyntax
//@[111:112)   LeftSquare |[|
//@[112:115)   Identifier |for|
//@[116:117)   LocalVariableSyntax
//@[116:117)    IdentifierSyntax
//@[116:117)     Identifier |i|
//@[118:120)   Identifier |in|
//@[121:132)   FunctionCallSyntax
//@[121:126)    IdentifierSyntax
//@[121:126)     Identifier |range|
//@[126:127)    LeftParen |(|
//@[127:129)    FunctionArgumentSyntax
//@[127:128)     IntegerLiteralSyntax
//@[127:128)      Integer |0|
//@[128:129)     Comma |,|
//@[130:131)    FunctionArgumentSyntax
//@[130:131)     IntegerLiteralSyntax
//@[130:131)      Integer |3|
//@[131:132)    RightParen |)|
//@[132:133)   Colon |:|
//@[134:286)   ObjectSyntax
//@[134:135)    LeftBrace |{|
//@[135:137)    NewLine |\r\n|
  name: 'vnet-${i}'
//@[2:19)    ObjectPropertySyntax
//@[2:6)     IdentifierSyntax
//@[2:6)      Identifier |name|
//@[6:7)     Colon |:|
//@[8:19)     StringSyntax
//@[8:16)      StringLeftPiece |'vnet-${|
//@[16:17)      VariableAccessSyntax
//@[16:17)       IdentifierSyntax
//@[16:17)        Identifier |i|
//@[17:19)      StringRightPiece |}'|
//@[19:21)    NewLine |\r\n|
  properties: {
//@[2:98)    ObjectPropertySyntax
//@[2:12)     IdentifierSyntax
//@[2:12)      Identifier |properties|
//@[12:13)     Colon |:|
//@[14:98)     ObjectSyntax
//@[14:15)      LeftBrace |{|
//@[15:17)      NewLine |\r\n|
    subnets: premiumStorages
//@[4:28)      ObjectPropertySyntax
//@[4:11)       IdentifierSyntax
//@[4:11)        Identifier |subnets|
//@[11:12)       Colon |:|
//@[13:28)       VariableAccessSyntax
//@[13:28)        IdentifierSyntax
//@[13:28)         Identifier |premiumStorages|
//@[28:30)      NewLine |\r\n|
    dependsOn: [
//@[4:46)      ObjectPropertySyntax
//@[4:13)       IdentifierSyntax
//@[4:13)        Identifier |dependsOn|
//@[13:14)       Colon |:|
//@[15:46)       ArraySyntax
//@[15:16)        LeftSquare |[|
//@[16:18)        NewLine |\r\n|
      premiumStorages
//@[6:21)        ArrayItemSyntax
//@[6:21)         VariableAccessSyntax
//@[6:21)          IdentifierSyntax
//@[6:21)           Identifier |premiumStorages|
//@[21:23)        NewLine |\r\n|
    ]
//@[4:5)        RightSquare |]|
//@[5:7)      NewLine |\r\n|
  }
//@[2:3)      RightBrace |}|
//@[3:5)    NewLine |\r\n|
  dependsOn: [
//@[2:25)    ObjectPropertySyntax
//@[2:11)     IdentifierSyntax
//@[2:11)      Identifier |dependsOn|
//@[11:12)     Colon |:|
//@[13:25)     ArraySyntax
//@[13:14)      LeftSquare |[|
//@[14:16)      NewLine |\r\n|
    
//@[4:6)      NewLine |\r\n|
  ]
//@[2:3)      RightSquare |]|
//@[3:5)    NewLine |\r\n|
}]
//@[0:1)    RightBrace |}|
//@[1:2)   RightSquare |]|
//@[2:6) NewLine |\r\n\r\n|

var expressionInPropertyLoopVar = true
//@[0:38) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:31)  IdentifierSyntax
//@[4:31)   Identifier |expressionInPropertyLoopVar|
//@[32:33)  Assignment |=|
//@[34:38)  BooleanLiteralSyntax
//@[34:38)   TrueKeyword |true|
//@[38:40) NewLine |\r\n|
resource expressionsInPropertyLoopName 'Microsoft.Network/dnsZones@2018-05-01' = {
//@[0:232) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:38)  IdentifierSyntax
//@[9:38)   Identifier |expressionsInPropertyLoopName|
//@[39:78)  StringSyntax
//@[39:78)   StringComplete |'Microsoft.Network/dnsZones@2018-05-01'|
//@[79:80)  Assignment |=|
//@[81:232)  ObjectSyntax
//@[81:82)   LeftBrace |{|
//@[82:84)   NewLine |\r\n|
  name: 'hello'
//@[2:15)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:15)    StringSyntax
//@[8:15)     StringComplete |'hello'|
//@[15:17)   NewLine |\r\n|
  location: 'eastus'
//@[2:20)   ObjectPropertySyntax
//@[2:10)    IdentifierSyntax
//@[2:10)     Identifier |location|
//@[10:11)    Colon |:|
//@[12:20)    StringSyntax
//@[12:20)     StringComplete |'eastus'|
//@[20:22)   NewLine |\r\n|
  properties: {
//@[2:106)   ObjectPropertySyntax
//@[2:12)    IdentifierSyntax
//@[2:12)     Identifier |properties|
//@[12:13)    Colon |:|
//@[14:106)    ObjectSyntax
//@[14:15)     LeftBrace |{|
//@[15:17)     NewLine |\r\n|
    'resolutionVirtualNetworks${expressionInPropertyLoopVar}': [for thing in []: {}]
//@[4:84)     ObjectPropertySyntax
//@[4:61)      StringSyntax
//@[4:32)       StringLeftPiece |'resolutionVirtualNetworks${|
//@[32:59)       VariableAccessSyntax
//@[32:59)        IdentifierSyntax
//@[32:59)         Identifier |expressionInPropertyLoopVar|
//@[59:61)       StringRightPiece |}'|
//@[61:62)      Colon |:|
//@[63:84)      ForSyntax
//@[63:64)       LeftSquare |[|
//@[64:67)       Identifier |for|
//@[68:73)       LocalVariableSyntax
//@[68:73)        IdentifierSyntax
//@[68:73)         Identifier |thing|
//@[74:76)       Identifier |in|
//@[77:79)       ArraySyntax
//@[77:78)        LeftSquare |[|
//@[78:79)        RightSquare |]|
//@[79:80)       Colon |:|
//@[81:83)       ObjectSyntax
//@[81:82)        LeftBrace |{|
//@[82:83)        RightBrace |}|
//@[83:84)       RightSquare |]|
//@[84:86)     NewLine |\r\n|
  }
//@[2:3)     RightBrace |}|
//@[3:5)   NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

// resource loop body that isn't an object
//@[42:44) NewLine |\r\n|
@batchSize(-1)
//@[0:118) ResourceDeclarationSyntax
//@[0:14)  DecoratorSyntax
//@[0:1)   At |@|
//@[1:14)   FunctionCallSyntax
//@[1:10)    IdentifierSyntax
//@[1:10)     Identifier |batchSize|
//@[10:11)    LeftParen |(|
//@[11:13)    FunctionArgumentSyntax
//@[11:13)     UnaryOperationSyntax
//@[11:12)      Minus |-|
//@[12:13)      IntegerLiteralSyntax
//@[12:13)       Integer |1|
//@[13:14)    RightParen |)|
//@[14:16)  NewLine |\r\n|
resource nonObjectResourceLoopBody 'Microsoft.Network/dnsZones@2018-05-01' = [for thing in []: 'test']
//@[0:8)  Identifier |resource|
//@[9:34)  IdentifierSyntax
//@[9:34)   Identifier |nonObjectResourceLoopBody|
//@[35:74)  StringSyntax
//@[35:74)   StringComplete |'Microsoft.Network/dnsZones@2018-05-01'|
//@[75:76)  Assignment |=|
//@[77:102)  ForSyntax
//@[77:78)   LeftSquare |[|
//@[78:81)   Identifier |for|
//@[82:87)   LocalVariableSyntax
//@[82:87)    IdentifierSyntax
//@[82:87)     Identifier |thing|
//@[88:90)   Identifier |in|
//@[91:93)   ArraySyntax
//@[91:92)    LeftSquare |[|
//@[92:93)    RightSquare |]|
//@[93:94)   Colon |:|
//@[95:101)   SkippedTriviaSyntax
//@[95:101)    StringComplete |'test'|
//@[101:102)   RightSquare |]|
//@[102:104) NewLine |\r\n|
resource nonObjectResourceLoopBody2 'Microsoft.Network/dnsZones@2018-05-01' = [for thing in []: environment()]
//@[0:110) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:35)  IdentifierSyntax
//@[9:35)   Identifier |nonObjectResourceLoopBody2|
//@[36:75)  StringSyntax
//@[36:75)   StringComplete |'Microsoft.Network/dnsZones@2018-05-01'|
//@[76:77)  Assignment |=|
//@[78:110)  ForSyntax
//@[78:79)   LeftSquare |[|
//@[79:82)   Identifier |for|
//@[83:88)   LocalVariableSyntax
//@[83:88)    IdentifierSyntax
//@[83:88)     Identifier |thing|
//@[89:91)   Identifier |in|
//@[92:94)   ArraySyntax
//@[92:93)    LeftSquare |[|
//@[93:94)    RightSquare |]|
//@[94:95)   Colon |:|
//@[96:109)   SkippedTriviaSyntax
//@[96:107)    Identifier |environment|
//@[107:108)    LeftParen |(|
//@[108:109)    RightParen |)|
//@[109:110)   RightSquare |]|
//@[110:112) NewLine |\r\n|
resource nonObjectResourceLoopBody3 'Microsoft.Network/dnsZones@2018-05-01' = [for (thing,i) in []: 'test']
//@[0:107) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:35)  IdentifierSyntax
//@[9:35)   Identifier |nonObjectResourceLoopBody3|
//@[36:75)  StringSyntax
//@[36:75)   StringComplete |'Microsoft.Network/dnsZones@2018-05-01'|
//@[76:77)  Assignment |=|
//@[78:107)  ForSyntax
//@[78:79)   LeftSquare |[|
//@[79:82)   Identifier |for|
//@[83:92)   ForVariableBlockSyntax
//@[83:84)    LeftParen |(|
//@[84:89)    LocalVariableSyntax
//@[84:89)     IdentifierSyntax
//@[84:89)      Identifier |thing|
//@[89:90)    Comma |,|
//@[90:91)    LocalVariableSyntax
//@[90:91)     IdentifierSyntax
//@[90:91)      Identifier |i|
//@[91:92)    RightParen |)|
//@[93:95)   Identifier |in|
//@[96:98)   ArraySyntax
//@[96:97)    LeftSquare |[|
//@[97:98)    RightSquare |]|
//@[98:99)   Colon |:|
//@[100:106)   SkippedTriviaSyntax
//@[100:106)    StringComplete |'test'|
//@[106:107)   RightSquare |]|
//@[107:109) NewLine |\r\n|
resource nonObjectResourceLoopBody4 'Microsoft.Network/dnsZones@2018-05-01' = [for (thing,i) in []: environment()]
//@[0:114) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:35)  IdentifierSyntax
//@[9:35)   Identifier |nonObjectResourceLoopBody4|
//@[36:75)  StringSyntax
//@[36:75)   StringComplete |'Microsoft.Network/dnsZones@2018-05-01'|
//@[76:77)  Assignment |=|
//@[78:114)  ForSyntax
//@[78:79)   LeftSquare |[|
//@[79:82)   Identifier |for|
//@[83:92)   ForVariableBlockSyntax
//@[83:84)    LeftParen |(|
//@[84:89)    LocalVariableSyntax
//@[84:89)     IdentifierSyntax
//@[84:89)      Identifier |thing|
//@[89:90)    Comma |,|
//@[90:91)    LocalVariableSyntax
//@[90:91)     IdentifierSyntax
//@[90:91)      Identifier |i|
//@[91:92)    RightParen |)|
//@[93:95)   Identifier |in|
//@[96:98)   ArraySyntax
//@[96:97)    LeftSquare |[|
//@[97:98)    RightSquare |]|
//@[98:99)   Colon |:|
//@[100:113)   SkippedTriviaSyntax
//@[100:111)    Identifier |environment|
//@[111:112)    LeftParen |(|
//@[112:113)    RightParen |)|
//@[113:114)   RightSquare |]|
//@[114:116) NewLine |\r\n|
resource nonObjectResourceLoopBody3 'Microsoft.Network/dnsZones@2018-05-01' = [for (thing,i) in []: if(true) 'test']
//@[0:116) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:35)  IdentifierSyntax
//@[9:35)   Identifier |nonObjectResourceLoopBody3|
//@[36:75)  StringSyntax
//@[36:75)   StringComplete |'Microsoft.Network/dnsZones@2018-05-01'|
//@[76:77)  Assignment |=|
//@[78:116)  ForSyntax
//@[78:79)   LeftSquare |[|
//@[79:82)   Identifier |for|
//@[83:92)   ForVariableBlockSyntax
//@[83:84)    LeftParen |(|
//@[84:89)    LocalVariableSyntax
//@[84:89)     IdentifierSyntax
//@[84:89)      Identifier |thing|
//@[89:90)    Comma |,|
//@[90:91)    LocalVariableSyntax
//@[90:91)     IdentifierSyntax
//@[90:91)      Identifier |i|
//@[91:92)    RightParen |)|
//@[93:95)   Identifier |in|
//@[96:98)   ArraySyntax
//@[96:97)    LeftSquare |[|
//@[97:98)    RightSquare |]|
//@[98:99)   Colon |:|
//@[100:115)   IfConditionSyntax
//@[100:102)    Identifier |if|
//@[102:108)    ParenthesizedExpressionSyntax
//@[102:103)     LeftParen |(|
//@[103:107)     BooleanLiteralSyntax
//@[103:107)      TrueKeyword |true|
//@[107:108)     RightParen |)|
//@[109:115)    SkippedTriviaSyntax
//@[109:115)     StringComplete |'test'|
//@[115:116)   RightSquare |]|
//@[116:118) NewLine |\r\n|
resource nonObjectResourceLoopBody4 'Microsoft.Network/dnsZones@2018-05-01' = [for (thing,i) in []: if(true) environment()]
//@[0:123) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:35)  IdentifierSyntax
//@[9:35)   Identifier |nonObjectResourceLoopBody4|
//@[36:75)  StringSyntax
//@[36:75)   StringComplete |'Microsoft.Network/dnsZones@2018-05-01'|
//@[76:77)  Assignment |=|
//@[78:123)  ForSyntax
//@[78:79)   LeftSquare |[|
//@[79:82)   Identifier |for|
//@[83:92)   ForVariableBlockSyntax
//@[83:84)    LeftParen |(|
//@[84:89)    LocalVariableSyntax
//@[84:89)     IdentifierSyntax
//@[84:89)      Identifier |thing|
//@[89:90)    Comma |,|
//@[90:91)    LocalVariableSyntax
//@[90:91)     IdentifierSyntax
//@[90:91)      Identifier |i|
//@[91:92)    RightParen |)|
//@[93:95)   Identifier |in|
//@[96:98)   ArraySyntax
//@[96:97)    LeftSquare |[|
//@[97:98)    RightSquare |]|
//@[98:99)   Colon |:|
//@[100:122)   IfConditionSyntax
//@[100:102)    Identifier |if|
//@[102:108)    ParenthesizedExpressionSyntax
//@[102:103)     LeftParen |(|
//@[103:107)     BooleanLiteralSyntax
//@[103:107)      TrueKeyword |true|
//@[107:108)     RightParen |)|
//@[109:122)    SkippedTriviaSyntax
//@[109:120)     Identifier |environment|
//@[120:121)     LeftParen |(|
//@[121:122)     RightParen |)|
//@[122:123)   RightSquare |]|
//@[123:127) NewLine |\r\n\r\n|

// #completionTest(54,55) -> objectPlusFor
//@[42:44) NewLine |\r\n|
resource foo 'Microsoft.Network/dnsZones@2018-05-01' = 
//@[0:55) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:12)  IdentifierSyntax
//@[9:12)   Identifier |foo|
//@[13:52)  StringSyntax
//@[13:52)   StringComplete |'Microsoft.Network/dnsZones@2018-05-01'|
//@[53:54)  Assignment |=|
//@[55:55)  SkippedTriviaSyntax
//@[55:59) NewLine |\r\n\r\n|

resource foo 'Microsoft.Network/dnsZones@2018-05-01' = [for item in []: {
//@[0:257) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:12)  IdentifierSyntax
//@[9:12)   Identifier |foo|
//@[13:52)  StringSyntax
//@[13:52)   StringComplete |'Microsoft.Network/dnsZones@2018-05-01'|
//@[53:54)  Assignment |=|
//@[55:257)  ForSyntax
//@[55:56)   LeftSquare |[|
//@[56:59)   Identifier |for|
//@[60:64)   LocalVariableSyntax
//@[60:64)    IdentifierSyntax
//@[60:64)     Identifier |item|
//@[65:67)   Identifier |in|
//@[68:70)   ArraySyntax
//@[68:69)    LeftSquare |[|
//@[69:70)    RightSquare |]|
//@[70:71)   Colon |:|
//@[72:256)   ObjectSyntax
//@[72:73)    LeftBrace |{|
//@[73:75)    NewLine |\r\n|
  properties: {
//@[2:178)    ObjectPropertySyntax
//@[2:12)     IdentifierSyntax
//@[2:12)      Identifier |properties|
//@[12:13)     Colon |:|
//@[14:178)     ObjectSyntax
//@[14:15)      LeftBrace |{|
//@[15:17)      NewLine |\r\n|
    // #completionTest(32,33) -> symbolsPlusArrayAndFor
//@[55:57)      NewLine |\r\n|
    registrationVirtualNetworks: 
//@[4:33)      ObjectPropertySyntax
//@[4:31)       IdentifierSyntax
//@[4:31)        Identifier |registrationVirtualNetworks|
//@[31:32)       Colon |:|
//@[33:33)       SkippedTriviaSyntax
//@[33:35)      NewLine |\r\n|
    resolutionVirtualNetworks: [for lol in []: {
//@[4:64)      ObjectPropertySyntax
//@[4:29)       IdentifierSyntax
//@[4:29)        Identifier |resolutionVirtualNetworks|
//@[29:30)       Colon |:|
//@[31:64)       ForSyntax
//@[31:32)        LeftSquare |[|
//@[32:35)        Identifier |for|
//@[36:39)        LocalVariableSyntax
//@[36:39)         IdentifierSyntax
//@[36:39)          Identifier |lol|
//@[40:42)        Identifier |in|
//@[43:45)        ArraySyntax
//@[43:44)         LeftSquare |[|
//@[44:45)         RightSquare |]|
//@[45:46)        Colon |:|
//@[47:63)        ObjectSyntax
//@[47:48)         LeftBrace |{|
//@[48:50)         NewLine |\r\n|
      
//@[6:8)         NewLine |\r\n|
    }]
//@[4:5)         RightBrace |}|
//@[5:6)        RightSquare |]|
//@[6:8)      NewLine |\r\n|
  }
//@[2:3)      RightBrace |}|
//@[3:5)    NewLine |\r\n|
}]
//@[0:1)    RightBrace |}|
//@[1:2)   RightSquare |]|
//@[2:6) NewLine |\r\n\r\n|

resource vnet 'Microsoft.Network/virtualNetworks@2020-06-01' = {
//@[0:325) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:13)  IdentifierSyntax
//@[9:13)   Identifier |vnet|
//@[14:60)  StringSyntax
//@[14:60)   StringComplete |'Microsoft.Network/virtualNetworks@2020-06-01'|
//@[61:62)  Assignment |=|
//@[63:325)  ObjectSyntax
//@[63:64)   LeftBrace |{|
//@[64:66)   NewLine |\r\n|
  properties: {
//@[2:256)   ObjectPropertySyntax
//@[2:12)    IdentifierSyntax
//@[2:12)     Identifier |properties|
//@[12:13)    Colon |:|
//@[14:256)    ObjectSyntax
//@[14:15)     LeftBrace |{|
//@[15:17)     NewLine |\r\n|
    virtualNetworkPeerings: [for item in []: {
//@[4:234)     ObjectPropertySyntax
//@[4:26)      IdentifierSyntax
//@[4:26)       Identifier |virtualNetworkPeerings|
//@[26:27)      Colon |:|
//@[28:234)      ForSyntax
//@[28:29)       LeftSquare |[|
//@[29:32)       Identifier |for|
//@[33:37)       LocalVariableSyntax
//@[33:37)        IdentifierSyntax
//@[33:37)         Identifier |item|
//@[38:40)       Identifier |in|
//@[41:43)       ArraySyntax
//@[41:42)        LeftSquare |[|
//@[42:43)        RightSquare |]|
//@[43:44)       Colon |:|
//@[45:233)       ObjectSyntax
//@[45:46)        LeftBrace |{|
//@[46:48)        NewLine |\r\n|
        properties: {
//@[8:178)        ObjectPropertySyntax
//@[8:18)         IdentifierSyntax
//@[8:18)          Identifier |properties|
//@[18:19)         Colon |:|
//@[20:178)         ObjectSyntax
//@[20:21)          LeftBrace |{|
//@[21:23)          NewLine |\r\n|
          remoteAddressSpace: {
//@[10:144)          ObjectPropertySyntax
//@[10:28)           IdentifierSyntax
//@[10:28)            Identifier |remoteAddressSpace|
//@[28:29)           Colon |:|
//@[30:144)           ObjectSyntax
//@[30:31)            LeftBrace |{|
//@[31:33)            NewLine |\r\n|
            // #completionTest(28,29) -> symbolsPlusArrayWithoutFor
//@[67:69)            NewLine |\r\n|
            addressPrefixes: 
//@[12:29)            ObjectPropertySyntax
//@[12:27)             IdentifierSyntax
//@[12:27)              Identifier |addressPrefixes|
//@[27:28)             Colon |:|
//@[29:29)             SkippedTriviaSyntax
//@[29:31)            NewLine |\r\n|
          }
//@[10:11)            RightBrace |}|
//@[11:13)          NewLine |\r\n|
        }
//@[8:9)          RightBrace |}|
//@[9:11)        NewLine |\r\n|
    }]
//@[4:5)        RightBrace |}|
//@[5:6)       RightSquare |]|
//@[6:8)     NewLine |\r\n|
  }
//@[2:3)     RightBrace |}|
//@[3:5)   NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

// parent property with 'existing' resource at different scope
//@[62:64) NewLine |\r\n|
resource p1_res1 'Microsoft.Rp1/resource1@2020-06-01' existing = {
//@[0:104) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:16)  IdentifierSyntax
//@[9:16)   Identifier |p1_res1|
//@[17:53)  StringSyntax
//@[17:53)   StringComplete |'Microsoft.Rp1/resource1@2020-06-01'|
//@[54:62)  Identifier |existing|
//@[63:64)  Assignment |=|
//@[65:104)  ObjectSyntax
//@[65:66)   LeftBrace |{|
//@[66:68)   NewLine |\r\n|
  scope: tenant()
//@[2:17)   ObjectPropertySyntax
//@[2:7)    IdentifierSyntax
//@[2:7)     Identifier |scope|
//@[7:8)    Colon |:|
//@[9:17)    FunctionCallSyntax
//@[9:15)     IdentifierSyntax
//@[9:15)      Identifier |tenant|
//@[15:16)     LeftParen |(|
//@[16:17)     RightParen |)|
//@[17:19)   NewLine |\r\n|
  name: 'res1'
//@[2:14)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:14)    StringSyntax
//@[8:14)     StringComplete |'res1'|
//@[14:16)   NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource p1_child1 'Microsoft.Rp1/resource1/child1@2020-06-01' = {
//@[0:106) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:18)  IdentifierSyntax
//@[9:18)   Identifier |p1_child1|
//@[19:62)  StringSyntax
//@[19:62)   StringComplete |'Microsoft.Rp1/resource1/child1@2020-06-01'|
//@[63:64)  Assignment |=|
//@[65:106)  ObjectSyntax
//@[65:66)   LeftBrace |{|
//@[66:68)   NewLine |\r\n|
  parent: p1_res1
//@[2:17)   ObjectPropertySyntax
//@[2:8)    IdentifierSyntax
//@[2:8)     Identifier |parent|
//@[8:9)    Colon |:|
//@[10:17)    VariableAccessSyntax
//@[10:17)     IdentifierSyntax
//@[10:17)      Identifier |p1_res1|
//@[17:19)   NewLine |\r\n|
  name: 'child1'
//@[2:16)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:16)    StringSyntax
//@[8:16)     StringComplete |'child1'|
//@[16:18)   NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

// parent property with scope on child resource
//@[47:49) NewLine |\r\n|
resource p2_res1 'Microsoft.Rp1/resource1@2020-06-01' = {
//@[0:76) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:16)  IdentifierSyntax
//@[9:16)   Identifier |p2_res1|
//@[17:53)  StringSyntax
//@[17:53)   StringComplete |'Microsoft.Rp1/resource1@2020-06-01'|
//@[54:55)  Assignment |=|
//@[56:76)  ObjectSyntax
//@[56:57)   LeftBrace |{|
//@[57:59)   NewLine |\r\n|
  name: 'res1'
//@[2:14)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:14)    StringSyntax
//@[8:14)     StringComplete |'res1'|
//@[14:16)   NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource p2_res2 'Microsoft.Rp2/resource2@2020-06-01' = {
//@[0:76) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:16)  IdentifierSyntax
//@[9:16)   Identifier |p2_res2|
//@[17:53)  StringSyntax
//@[17:53)   StringComplete |'Microsoft.Rp2/resource2@2020-06-01'|
//@[54:55)  Assignment |=|
//@[56:76)  ObjectSyntax
//@[56:57)   LeftBrace |{|
//@[57:59)   NewLine |\r\n|
  name: 'res2'
//@[2:14)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:14)    StringSyntax
//@[8:14)     StringComplete |'res2'|
//@[14:16)   NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource p2_res2child 'Microsoft.Rp2/resource2/child2@2020-06-01' = {
//@[0:127) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:21)  IdentifierSyntax
//@[9:21)   Identifier |p2_res2child|
//@[22:65)  StringSyntax
//@[22:65)   StringComplete |'Microsoft.Rp2/resource2/child2@2020-06-01'|
//@[66:67)  Assignment |=|
//@[68:127)  ObjectSyntax
//@[68:69)   LeftBrace |{|
//@[69:71)   NewLine |\r\n|
  scope: p2_res1
//@[2:16)   ObjectPropertySyntax
//@[2:7)    IdentifierSyntax
//@[2:7)     Identifier |scope|
//@[7:8)    Colon |:|
//@[9:16)    VariableAccessSyntax
//@[9:16)     IdentifierSyntax
//@[9:16)      Identifier |p2_res1|
//@[16:18)   NewLine |\r\n|
  parent: p2_res2
//@[2:17)   ObjectPropertySyntax
//@[2:8)    IdentifierSyntax
//@[2:8)     Identifier |parent|
//@[8:9)    Colon |:|
//@[10:17)    VariableAccessSyntax
//@[10:17)     IdentifierSyntax
//@[10:17)      Identifier |p2_res2|
//@[17:19)   NewLine |\r\n|
  name: 'child2'
//@[2:16)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:16)    StringSyntax
//@[8:16)     StringComplete |'child2'|
//@[16:18)   NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

// parent property self-cycle
//@[29:31) NewLine |\r\n|
resource p3_vmExt 'Microsoft.Compute/virtualMachines/extensions@2020-06-01' = {
//@[0:124) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:17)  IdentifierSyntax
//@[9:17)   Identifier |p3_vmExt|
//@[18:75)  StringSyntax
//@[18:75)   StringComplete |'Microsoft.Compute/virtualMachines/extensions@2020-06-01'|
//@[76:77)  Assignment |=|
//@[78:124)  ObjectSyntax
//@[78:79)   LeftBrace |{|
//@[79:81)   NewLine |\r\n|
  parent: p3_vmExt
//@[2:18)   ObjectPropertySyntax
//@[2:8)    IdentifierSyntax
//@[2:8)     Identifier |parent|
//@[8:9)    Colon |:|
//@[10:18)    VariableAccessSyntax
//@[10:18)     IdentifierSyntax
//@[10:18)      Identifier |p3_vmExt|
//@[18:20)   NewLine |\r\n|
  location: 'eastus'
//@[2:20)   ObjectPropertySyntax
//@[2:10)    IdentifierSyntax
//@[2:10)     Identifier |location|
//@[10:11)    Colon |:|
//@[12:20)    StringSyntax
//@[12:20)     StringComplete |'eastus'|
//@[20:22)   NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

// parent property 2-cycle
//@[26:28) NewLine |\r\n|
resource p4_vm 'Microsoft.Compute/virtualMachines@2020-06-01' = {
//@[0:110) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:14)  IdentifierSyntax
//@[9:14)   Identifier |p4_vm|
//@[15:61)  StringSyntax
//@[15:61)   StringComplete |'Microsoft.Compute/virtualMachines@2020-06-01'|
//@[62:63)  Assignment |=|
//@[64:110)  ObjectSyntax
//@[64:65)   LeftBrace |{|
//@[65:67)   NewLine |\r\n|
  parent: p4_vmExt
//@[2:18)   ObjectPropertySyntax
//@[2:8)    IdentifierSyntax
//@[2:8)     Identifier |parent|
//@[8:9)    Colon |:|
//@[10:18)    VariableAccessSyntax
//@[10:18)     IdentifierSyntax
//@[10:18)      Identifier |p4_vmExt|
//@[18:20)   NewLine |\r\n|
  location: 'eastus'
//@[2:20)   ObjectPropertySyntax
//@[2:10)    IdentifierSyntax
//@[2:10)     Identifier |location|
//@[10:11)    Colon |:|
//@[12:20)    StringSyntax
//@[12:20)     StringComplete |'eastus'|
//@[20:22)   NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource p4_vmExt 'Microsoft.Compute/virtualMachines/extensions@2020-06-01' = {
//@[0:121) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:17)  IdentifierSyntax
//@[9:17)   Identifier |p4_vmExt|
//@[18:75)  StringSyntax
//@[18:75)   StringComplete |'Microsoft.Compute/virtualMachines/extensions@2020-06-01'|
//@[76:77)  Assignment |=|
//@[78:121)  ObjectSyntax
//@[78:79)   LeftBrace |{|
//@[79:81)   NewLine |\r\n|
  parent: p4_vm
//@[2:15)   ObjectPropertySyntax
//@[2:8)    IdentifierSyntax
//@[2:8)     Identifier |parent|
//@[8:9)    Colon |:|
//@[10:15)    VariableAccessSyntax
//@[10:15)     IdentifierSyntax
//@[10:15)      Identifier |p4_vm|
//@[15:17)   NewLine |\r\n|
  location: 'eastus'
//@[2:20)   ObjectPropertySyntax
//@[2:10)    IdentifierSyntax
//@[2:10)     Identifier |location|
//@[10:11)    Colon |:|
//@[12:20)    StringSyntax
//@[12:20)     StringComplete |'eastus'|
//@[20:22)   NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

// parent property with invalid child
//@[37:39) NewLine |\r\n|
resource p5_res1 'Microsoft.Rp1/resource1@2020-06-01' = {
//@[0:76) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:16)  IdentifierSyntax
//@[9:16)   Identifier |p5_res1|
//@[17:53)  StringSyntax
//@[17:53)   StringComplete |'Microsoft.Rp1/resource1@2020-06-01'|
//@[54:55)  Assignment |=|
//@[56:76)  ObjectSyntax
//@[56:57)   LeftBrace |{|
//@[57:59)   NewLine |\r\n|
  name: 'res1'
//@[2:14)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:14)    StringSyntax
//@[8:14)     StringComplete |'res1'|
//@[14:16)   NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource p5_res2 'Microsoft.Rp2/resource2/child2@2020-06-01' = {
//@[0:102) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:16)  IdentifierSyntax
//@[9:16)   Identifier |p5_res2|
//@[17:60)  StringSyntax
//@[17:60)   StringComplete |'Microsoft.Rp2/resource2/child2@2020-06-01'|
//@[61:62)  Assignment |=|
//@[63:102)  ObjectSyntax
//@[63:64)   LeftBrace |{|
//@[64:66)   NewLine |\r\n|
  parent: p5_res1
//@[2:17)   ObjectPropertySyntax
//@[2:8)    IdentifierSyntax
//@[2:8)     Identifier |parent|
//@[8:9)    Colon |:|
//@[10:17)    VariableAccessSyntax
//@[10:17)     IdentifierSyntax
//@[10:17)      Identifier |p5_res1|
//@[17:19)   NewLine |\r\n|
  name: 'res2'
//@[2:14)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:14)    StringSyntax
//@[8:14)     StringComplete |'res2'|
//@[14:16)   NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

// parent property with invalid parent
//@[38:40) NewLine |\r\n|
resource p6_res1 '${true}' = {
//@[0:49) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:16)  IdentifierSyntax
//@[9:16)   Identifier |p6_res1|
//@[17:26)  StringSyntax
//@[17:20)   StringLeftPiece |'${|
//@[20:24)   BooleanLiteralSyntax
//@[20:24)    TrueKeyword |true|
//@[24:26)   StringRightPiece |}'|
//@[27:28)  Assignment |=|
//@[29:49)  ObjectSyntax
//@[29:30)   LeftBrace |{|
//@[30:32)   NewLine |\r\n|
  name: 'res1'
//@[2:14)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:14)    StringSyntax
//@[8:14)     StringComplete |'res1'|
//@[14:16)   NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource p6_res2 'Microsoft.Rp1/resource1/child2@2020-06-01' = {
//@[0:102) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:16)  IdentifierSyntax
//@[9:16)   Identifier |p6_res2|
//@[17:60)  StringSyntax
//@[17:60)   StringComplete |'Microsoft.Rp1/resource1/child2@2020-06-01'|
//@[61:62)  Assignment |=|
//@[63:102)  ObjectSyntax
//@[63:64)   LeftBrace |{|
//@[64:66)   NewLine |\r\n|
  parent: p6_res1
//@[2:17)   ObjectPropertySyntax
//@[2:8)    IdentifierSyntax
//@[2:8)     Identifier |parent|
//@[8:9)    Colon |:|
//@[10:17)    VariableAccessSyntax
//@[10:17)     IdentifierSyntax
//@[10:17)      Identifier |p6_res1|
//@[17:19)   NewLine |\r\n|
  name: 'res2'
//@[2:14)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:14)    StringSyntax
//@[8:14)     StringComplete |'res2'|
//@[14:16)   NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

// parent property with incorrectly-formatted name
//@[50:52) NewLine |\r\n|
resource p7_res1 'Microsoft.Rp1/resource1@2020-06-01' = {
//@[0:76) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:16)  IdentifierSyntax
//@[9:16)   Identifier |p7_res1|
//@[17:53)  StringSyntax
//@[17:53)   StringComplete |'Microsoft.Rp1/resource1@2020-06-01'|
//@[54:55)  Assignment |=|
//@[56:76)  ObjectSyntax
//@[56:57)   LeftBrace |{|
//@[57:59)   NewLine |\r\n|
  name: 'res1'
//@[2:14)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:14)    StringSyntax
//@[8:14)     StringComplete |'res1'|
//@[14:16)   NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource p7_res2 'Microsoft.Rp1/resource1/child2@2020-06-01' = {
//@[0:107) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:16)  IdentifierSyntax
//@[9:16)   Identifier |p7_res2|
//@[17:60)  StringSyntax
//@[17:60)   StringComplete |'Microsoft.Rp1/resource1/child2@2020-06-01'|
//@[61:62)  Assignment |=|
//@[63:107)  ObjectSyntax
//@[63:64)   LeftBrace |{|
//@[64:66)   NewLine |\r\n|
  parent: p7_res1
//@[2:17)   ObjectPropertySyntax
//@[2:8)    IdentifierSyntax
//@[2:8)     Identifier |parent|
//@[8:9)    Colon |:|
//@[10:17)    VariableAccessSyntax
//@[10:17)     IdentifierSyntax
//@[10:17)      Identifier |p7_res1|
//@[17:19)   NewLine |\r\n|
  name: 'res1/res2'
//@[2:19)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:19)    StringSyntax
//@[8:19)     StringComplete |'res1/res2'|
//@[19:21)   NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource p7_res3 'Microsoft.Rp1/resource1/child2@2020-06-01' = {
//@[0:118) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:16)  IdentifierSyntax
//@[9:16)   Identifier |p7_res3|
//@[17:60)  StringSyntax
//@[17:60)   StringComplete |'Microsoft.Rp1/resource1/child2@2020-06-01'|
//@[61:62)  Assignment |=|
//@[63:118)  ObjectSyntax
//@[63:64)   LeftBrace |{|
//@[64:66)   NewLine |\r\n|
  parent: p7_res1
//@[2:17)   ObjectPropertySyntax
//@[2:8)    IdentifierSyntax
//@[2:8)     Identifier |parent|
//@[8:9)    Colon |:|
//@[10:17)    VariableAccessSyntax
//@[10:17)     IdentifierSyntax
//@[10:17)      Identifier |p7_res1|
//@[17:19)   NewLine |\r\n|
  name: '${p7_res1.name}/res2'
//@[2:30)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:30)    StringSyntax
//@[8:11)     StringLeftPiece |'${|
//@[11:23)     PropertyAccessSyntax
//@[11:18)      VariableAccessSyntax
//@[11:18)       IdentifierSyntax
//@[11:18)        Identifier |p7_res1|
//@[18:19)      Dot |.|
//@[19:23)      IdentifierSyntax
//@[19:23)       Identifier |name|
//@[23:30)     StringRightPiece |}/res2'|
//@[30:32)   NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

// top-level resource with too many '/' characters
//@[50:52) NewLine |\r\n|
resource p8_res1 'Microsoft.Rp1/resource1@2020-06-01' = {
//@[0:81) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:16)  IdentifierSyntax
//@[9:16)   Identifier |p8_res1|
//@[17:53)  StringSyntax
//@[17:53)   StringComplete |'Microsoft.Rp1/resource1@2020-06-01'|
//@[54:55)  Assignment |=|
//@[56:81)  ObjectSyntax
//@[56:57)   LeftBrace |{|
//@[57:59)   NewLine |\r\n|
  name: 'res1/res2'
//@[2:19)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:19)    StringSyntax
//@[8:19)     StringComplete |'res1/res2'|
//@[19:21)   NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

resource existngResProperty 'Microsoft.Compute/virtualMachines@2020-06-01' existing = {
//@[0:164) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:27)  IdentifierSyntax
//@[9:27)   Identifier |existngResProperty|
//@[28:74)  StringSyntax
//@[28:74)   StringComplete |'Microsoft.Compute/virtualMachines@2020-06-01'|
//@[75:83)  Identifier |existing|
//@[84:85)  Assignment |=|
//@[86:164)  ObjectSyntax
//@[86:87)   LeftBrace |{|
//@[87:89)   NewLine |\r\n|
  name: 'existngResProperty'
//@[2:28)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:28)    StringSyntax
//@[8:28)     StringComplete |'existngResProperty'|
//@[28:30)   NewLine |\r\n|
  location: 'westeurope'
//@[2:24)   ObjectPropertySyntax
//@[2:10)    IdentifierSyntax
//@[2:10)     Identifier |location|
//@[10:11)    Colon |:|
//@[12:24)    StringSyntax
//@[12:24)     StringComplete |'westeurope'|
//@[24:26)   NewLine |\r\n|
  properties: {}
//@[2:16)   ObjectPropertySyntax
//@[2:12)    IdentifierSyntax
//@[2:12)     Identifier |properties|
//@[12:13)    Colon |:|
//@[14:16)    ObjectSyntax
//@[14:15)     LeftBrace |{|
//@[15:16)     RightBrace |}|
//@[16:18)   NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:1) EndOfFile ||
