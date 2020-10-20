
//@[0:2) NewLine |\r\n|
output myStr string = 'hello'
//@[0:29) OutputDeclarationSyntax
//@[0:6)  Identifier |output|
//@[7:12)  IdentifierSyntax
//@[7:12)   Identifier |myStr|
//@[13:19)  TypeSyntax
//@[13:19)   Identifier |string|
//@[20:21)  Assignment |=|
//@[22:29)  StringSyntax
//@[22:29)   StringComplete |'hello'|
//@[29:33) NewLine |\r\n\r\n|

output myInt int = 7
//@[0:20) OutputDeclarationSyntax
//@[0:6)  Identifier |output|
//@[7:12)  IdentifierSyntax
//@[7:12)   Identifier |myInt|
//@[13:16)  TypeSyntax
//@[13:16)   Identifier |int|
//@[17:18)  Assignment |=|
//@[19:20)  NumericLiteralSyntax
//@[19:20)   Number |7|
//@[20:22) NewLine |\r\n|
output myOtherInt int = 20 / 13 + 80 % -4
//@[0:41) OutputDeclarationSyntax
//@[0:6)  Identifier |output|
//@[7:17)  IdentifierSyntax
//@[7:17)   Identifier |myOtherInt|
//@[18:21)  TypeSyntax
//@[18:21)   Identifier |int|
//@[22:23)  Assignment |=|
//@[24:41)  BinaryOperationSyntax
//@[24:31)   BinaryOperationSyntax
//@[24:26)    NumericLiteralSyntax
//@[24:26)     Number |20|
//@[27:28)    Slash |/|
//@[29:31)    NumericLiteralSyntax
//@[29:31)     Number |13|
//@[32:33)   Plus |+|
//@[34:41)   BinaryOperationSyntax
//@[34:36)    NumericLiteralSyntax
//@[34:36)     Number |80|
//@[37:38)    Modulo |%|
//@[39:41)    UnaryOperationSyntax
//@[39:40)     Minus |-|
//@[40:41)     NumericLiteralSyntax
//@[40:41)      Number |4|
//@[41:45) NewLine |\r\n\r\n|

output myBool bool = !false
//@[0:27) OutputDeclarationSyntax
//@[0:6)  Identifier |output|
//@[7:13)  IdentifierSyntax
//@[7:13)   Identifier |myBool|
//@[14:18)  TypeSyntax
//@[14:18)   Identifier |bool|
//@[19:20)  Assignment |=|
//@[21:27)  UnaryOperationSyntax
//@[21:22)   Exclamation |!|
//@[22:27)   BooleanLiteralSyntax
//@[22:27)    FalseKeyword |false|
//@[27:29) NewLine |\r\n|
output myOtherBool bool = true
//@[0:30) OutputDeclarationSyntax
//@[0:6)  Identifier |output|
//@[7:18)  IdentifierSyntax
//@[7:18)   Identifier |myOtherBool|
//@[19:23)  TypeSyntax
//@[19:23)   Identifier |bool|
//@[24:25)  Assignment |=|
//@[26:30)  BooleanLiteralSyntax
//@[26:30)   TrueKeyword |true|
//@[30:34) NewLine |\r\n\r\n|

output suchEmpty array = [
//@[0:29) OutputDeclarationSyntax
//@[0:6)  Identifier |output|
//@[7:16)  IdentifierSyntax
//@[7:16)   Identifier |suchEmpty|
//@[17:22)  TypeSyntax
//@[17:22)   Identifier |array|
//@[23:24)  Assignment |=|
//@[25:29)  ArraySyntax
//@[25:26)   LeftSquare |[|
//@[26:28)   NewLine |\r\n|
]
//@[0:1)   RightSquare |]|
//@[1:5) NewLine |\r\n\r\n|

output suchEmpty2 object = {
//@[0:31) OutputDeclarationSyntax
//@[0:6)  Identifier |output|
//@[7:17)  IdentifierSyntax
//@[7:17)   Identifier |suchEmpty2|
//@[18:24)  TypeSyntax
//@[18:24)   Identifier |object|
//@[25:26)  Assignment |=|
//@[27:31)  ObjectSyntax
//@[27:28)   LeftBrace |{|
//@[28:30)   NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

output obj object = {
//@[0:178) OutputDeclarationSyntax
//@[0:6)  Identifier |output|
//@[7:10)  IdentifierSyntax
//@[7:10)   Identifier |obj|
//@[11:17)  TypeSyntax
//@[11:17)   Identifier |object|
//@[18:19)  Assignment |=|
//@[20:178)  ObjectSyntax
//@[20:21)   LeftBrace |{|
//@[21:23)   NewLine |\r\n|
  a: 'a'
//@[2:8)   ObjectPropertySyntax
//@[2:3)    IdentifierSyntax
//@[2:3)     Identifier |a|
//@[3:4)    Colon |:|
//@[5:8)    StringSyntax
//@[5:8)     StringComplete |'a'|
//@[8:10)   NewLine |\r\n|
  b: 12
//@[2:7)   ObjectPropertySyntax
//@[2:3)    IdentifierSyntax
//@[2:3)     Identifier |b|
//@[3:4)    Colon |:|
//@[5:7)    NumericLiteralSyntax
//@[5:7)     Number |12|
//@[7:9)   NewLine |\r\n|
  c: true
//@[2:9)   ObjectPropertySyntax
//@[2:3)    IdentifierSyntax
//@[2:3)     Identifier |c|
//@[3:4)    Colon |:|
//@[5:9)    BooleanLiteralSyntax
//@[5:9)     TrueKeyword |true|
//@[9:11)   NewLine |\r\n|
  d: null
//@[2:9)   ObjectPropertySyntax
//@[2:3)    IdentifierSyntax
//@[2:3)     Identifier |d|
//@[3:4)    Colon |:|
//@[5:9)    NullLiteralSyntax
//@[5:9)     NullKeyword |null|
//@[9:11)   NewLine |\r\n|
  list: [
//@[2:59)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |list|
//@[6:7)    Colon |:|
//@[8:59)    ArraySyntax
//@[8:9)     LeftSquare |[|
//@[9:11)     NewLine |\r\n|
    1
//@[4:5)     ArrayItemSyntax
//@[4:5)      NumericLiteralSyntax
//@[4:5)       Number |1|
//@[5:7)     NewLine |\r\n|
    2
//@[4:5)     ArrayItemSyntax
//@[4:5)      NumericLiteralSyntax
//@[4:5)       Number |2|
//@[5:7)     NewLine |\r\n|
    3
//@[4:5)     ArrayItemSyntax
//@[4:5)      NumericLiteralSyntax
//@[4:5)       Number |3|
//@[5:7)     NewLine |\r\n|
    null
//@[4:8)     ArrayItemSyntax
//@[4:8)      NullLiteralSyntax
//@[4:8)       NullKeyword |null|
//@[8:10)     NewLine |\r\n|
    {
//@[4:12)     ArrayItemSyntax
//@[4:12)      ObjectSyntax
//@[4:5)       LeftBrace |{|
//@[5:7)       NewLine |\r\n|
    }
//@[4:5)       RightBrace |}|
//@[5:7)     NewLine |\r\n|
  ]
//@[2:3)     RightSquare |]|
//@[3:5)   NewLine |\r\n|
  obj: {
//@[2:50)   ObjectPropertySyntax
//@[2:5)    IdentifierSyntax
//@[2:5)     Identifier |obj|
//@[5:6)    Colon |:|
//@[7:50)    ObjectSyntax
//@[7:8)     LeftBrace |{|
//@[8:10)     NewLine |\r\n|
    nested: [
//@[4:35)     ObjectPropertySyntax
//@[4:10)      IdentifierSyntax
//@[4:10)       Identifier |nested|
//@[10:11)      Colon |:|
//@[12:35)      ArraySyntax
//@[12:13)       LeftSquare |[|
//@[13:15)       NewLine |\r\n|
      'hello'
//@[6:13)       ArrayItemSyntax
//@[6:13)        StringSyntax
//@[6:13)         StringComplete |'hello'|
//@[13:15)       NewLine |\r\n|
    ]
//@[4:5)       RightSquare |]|
//@[5:7)     NewLine |\r\n|
  }
//@[2:3)     RightBrace |}|
//@[3:5)   NewLine |\r\n|
}
//@[0:1)   RightBrace |}|
//@[1:5) NewLine |\r\n\r\n|

output myArr array = [
//@[0:74) OutputDeclarationSyntax
//@[0:6)  Identifier |output|
//@[7:12)  IdentifierSyntax
//@[7:12)   Identifier |myArr|
//@[13:18)  TypeSyntax
//@[13:18)   Identifier |array|
//@[19:20)  Assignment |=|
//@[21:74)  ArraySyntax
//@[21:22)   LeftSquare |[|
//@[22:24)   NewLine |\r\n|
  'pirates'
//@[2:11)   ArrayItemSyntax
//@[2:11)    StringSyntax
//@[2:11)     StringComplete |'pirates'|
//@[11:13)   NewLine |\r\n|
  'say'
//@[2:7)   ArrayItemSyntax
//@[2:7)    StringSyntax
//@[2:7)     StringComplete |'say'|
//@[7:9)   NewLine |\r\n|
   false ? 'arr2' : 'arr'
//@[3:25)   ArrayItemSyntax
//@[3:25)    TernaryOperationSyntax
//@[3:8)     BooleanLiteralSyntax
//@[3:8)      FalseKeyword |false|
//@[9:10)     Question |?|
//@[11:17)     StringSyntax
//@[11:17)      StringComplete |'arr2'|
//@[18:19)     Colon |:|
//@[20:25)     StringSyntax
//@[20:25)      StringComplete |'arr'|
//@[25:27)   NewLine |\r\n|
]
//@[0:1)   RightSquare |]|
//@[1:5) NewLine |\r\n\r\n|

output rgLocation string = resourceGroup().location
//@[0:51) OutputDeclarationSyntax
//@[0:6)  Identifier |output|
//@[7:17)  IdentifierSyntax
//@[7:17)   Identifier |rgLocation|
//@[18:24)  TypeSyntax
//@[18:24)   Identifier |string|
//@[25:26)  Assignment |=|
//@[27:51)  PropertyAccessSyntax
//@[27:42)   FunctionCallSyntax
//@[27:40)    IdentifierSyntax
//@[27:40)     Identifier |resourceGroup|
//@[40:41)    LeftParen |(|
//@[41:42)    RightParen |)|
//@[42:43)   Dot |.|
//@[43:51)   IdentifierSyntax
//@[43:51)    Identifier |location|
//@[51:55) NewLine |\r\n\r\n|

output crossRegion bool = resourceGroup().location == deployment().location ? false : true
//@[0:90) OutputDeclarationSyntax
//@[0:6)  Identifier |output|
//@[7:18)  IdentifierSyntax
//@[7:18)   Identifier |crossRegion|
//@[19:23)  TypeSyntax
//@[19:23)   Identifier |bool|
//@[24:25)  Assignment |=|
//@[26:90)  TernaryOperationSyntax
//@[26:75)   BinaryOperationSyntax
//@[26:50)    PropertyAccessSyntax
//@[26:41)     FunctionCallSyntax
//@[26:39)      IdentifierSyntax
//@[26:39)       Identifier |resourceGroup|
//@[39:40)      LeftParen |(|
//@[40:41)      RightParen |)|
//@[41:42)     Dot |.|
//@[42:50)     IdentifierSyntax
//@[42:50)      Identifier |location|
//@[51:53)    Equals |==|
//@[54:75)    PropertyAccessSyntax
//@[54:66)     FunctionCallSyntax
//@[54:64)      IdentifierSyntax
//@[54:64)       Identifier |deployment|
//@[64:65)      LeftParen |(|
//@[65:66)      RightParen |)|
//@[66:67)     Dot |.|
//@[67:75)     IdentifierSyntax
//@[67:75)      Identifier |location|
//@[76:77)   Question |?|
//@[78:83)   BooleanLiteralSyntax
//@[78:83)    FalseKeyword |false|
//@[84:85)   Colon |:|
//@[86:90)   BooleanLiteralSyntax
//@[86:90)    TrueKeyword |true|
//@[90:94) NewLine |\r\n\r\n|

output expressionBasedIndexer string = {
//@[0:140) OutputDeclarationSyntax
//@[0:6)  Identifier |output|
//@[7:29)  IdentifierSyntax
//@[7:29)   Identifier |expressionBasedIndexer|
//@[30:36)  TypeSyntax
//@[30:36)   Identifier |string|
//@[37:38)  Assignment |=|
//@[39:140)  PropertyAccessSyntax
//@[39:136)   ArrayAccessSyntax
//@[39:110)    ObjectSyntax
//@[39:40)     LeftBrace |{|
//@[40:42)     NewLine |\r\n|
  eastus: {
//@[2:31)     ObjectPropertySyntax
//@[2:8)      IdentifierSyntax
//@[2:8)       Identifier |eastus|
//@[8:9)      Colon |:|
//@[10:31)      ObjectSyntax
//@[10:11)       LeftBrace |{|
//@[11:13)       NewLine |\r\n|
    foo: true
//@[4:13)       ObjectPropertySyntax
//@[4:7)        IdentifierSyntax
//@[4:7)         Identifier |foo|
//@[7:8)        Colon |:|
//@[9:13)        BooleanLiteralSyntax
//@[9:13)         TrueKeyword |true|
//@[13:15)       NewLine |\r\n|
  }
//@[2:3)       RightBrace |}|
//@[3:5)     NewLine |\r\n|
  westus: {
//@[2:32)     ObjectPropertySyntax
//@[2:8)      IdentifierSyntax
//@[2:8)       Identifier |westus|
//@[8:9)      Colon |:|
//@[10:32)      ObjectSyntax
//@[10:11)       LeftBrace |{|
//@[11:13)       NewLine |\r\n|
    foo: false
//@[4:14)       ObjectPropertySyntax
//@[4:7)        IdentifierSyntax
//@[4:7)         Identifier |foo|
//@[7:8)        Colon |:|
//@[9:14)        BooleanLiteralSyntax
//@[9:14)         FalseKeyword |false|
//@[14:16)       NewLine |\r\n|
  }
//@[2:3)       RightBrace |}|
//@[3:5)     NewLine |\r\n|
}[resourceGroup().location].foo
//@[0:1)     RightBrace |}|
//@[1:2)    LeftSquare |[|
//@[2:26)    PropertyAccessSyntax
//@[2:17)     FunctionCallSyntax
//@[2:15)      IdentifierSyntax
//@[2:15)       Identifier |resourceGroup|
//@[15:16)      LeftParen |(|
//@[16:17)      RightParen |)|
//@[17:18)     Dot |.|
//@[18:26)     IdentifierSyntax
//@[18:26)      Identifier |location|
//@[26:27)    RightSquare |]|
//@[27:28)   Dot |.|
//@[28:31)   IdentifierSyntax
//@[28:31)    Identifier |foo|
//@[31:35) NewLine |\r\n\r\n|

var secondaryKeyIntermediateVar = listKeys(resourceId('Mock.RP/type', 'steve'), '2020-01-01').secondaryKey
//@[0:106) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:31)  IdentifierSyntax
//@[4:31)   Identifier |secondaryKeyIntermediateVar|
//@[32:33)  Assignment |=|
//@[34:106)  PropertyAccessSyntax
//@[34:93)   FunctionCallSyntax
//@[34:42)    IdentifierSyntax
//@[34:42)     Identifier |listKeys|
//@[42:43)    LeftParen |(|
//@[43:79)    FunctionArgumentSyntax
//@[43:78)     FunctionCallSyntax
//@[43:53)      IdentifierSyntax
//@[43:53)       Identifier |resourceId|
//@[53:54)      LeftParen |(|
//@[54:69)      FunctionArgumentSyntax
//@[54:68)       StringSyntax
//@[54:68)        StringComplete |'Mock.RP/type'|
//@[68:69)       Comma |,|
//@[70:77)      FunctionArgumentSyntax
//@[70:77)       StringSyntax
//@[70:77)        StringComplete |'steve'|
//@[77:78)      RightParen |)|
//@[78:79)     Comma |,|
//@[80:92)    FunctionArgumentSyntax
//@[80:92)     StringSyntax
//@[80:92)      StringComplete |'2020-01-01'|
//@[92:93)    RightParen |)|
//@[93:94)   Dot |.|
//@[94:106)   IdentifierSyntax
//@[94:106)    Identifier |secondaryKey|
//@[106:110) NewLine |\r\n\r\n|

output primaryKey string = listKeys(resourceId('Mock.RP/type', 'nigel'), '2020-01-01').primaryKey
//@[0:97) OutputDeclarationSyntax
//@[0:6)  Identifier |output|
//@[7:17)  IdentifierSyntax
//@[7:17)   Identifier |primaryKey|
//@[18:24)  TypeSyntax
//@[18:24)   Identifier |string|
//@[25:26)  Assignment |=|
//@[27:97)  PropertyAccessSyntax
//@[27:86)   FunctionCallSyntax
//@[27:35)    IdentifierSyntax
//@[27:35)     Identifier |listKeys|
//@[35:36)    LeftParen |(|
//@[36:72)    FunctionArgumentSyntax
//@[36:71)     FunctionCallSyntax
//@[36:46)      IdentifierSyntax
//@[36:46)       Identifier |resourceId|
//@[46:47)      LeftParen |(|
//@[47:62)      FunctionArgumentSyntax
//@[47:61)       StringSyntax
//@[47:61)        StringComplete |'Mock.RP/type'|
//@[61:62)       Comma |,|
//@[63:70)      FunctionArgumentSyntax
//@[63:70)       StringSyntax
//@[63:70)        StringComplete |'nigel'|
//@[70:71)      RightParen |)|
//@[71:72)     Comma |,|
//@[73:85)    FunctionArgumentSyntax
//@[73:85)     StringSyntax
//@[73:85)      StringComplete |'2020-01-01'|
//@[85:86)    RightParen |)|
//@[86:87)   Dot |.|
//@[87:97)   IdentifierSyntax
//@[87:97)    Identifier |primaryKey|
//@[97:99) NewLine |\r\n|
output secondaryKey string = secondaryKeyIntermediateVar
//@[0:56) OutputDeclarationSyntax
//@[0:6)  Identifier |output|
//@[7:19)  IdentifierSyntax
//@[7:19)   Identifier |secondaryKey|
//@[20:26)  TypeSyntax
//@[20:26)   Identifier |string|
//@[27:28)  Assignment |=|
//@[29:56)  VariableAccessSyntax
//@[29:56)   IdentifierSyntax
//@[29:56)    Identifier |secondaryKeyIntermediateVar|
//@[56:56) EndOfFile ||
