
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
//@[1:5) NewLine |\r\n\r\n|

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
//@[1:3) NewLine |\r\n|

//@[0:0) EndOfFile ||
