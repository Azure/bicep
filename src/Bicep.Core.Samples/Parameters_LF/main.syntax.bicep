/* 
  This is a block comment.
*/
//@[2:4) NewLine |\n\n|

// parameters without default value
//@[35:36) NewLine |\n|
param myString string
//@[0:21) ParameterDeclarationSyntax
//@[0:5)  Identifier |param|
//@[6:14)  IdentifierSyntax
//@[6:14)   Identifier |myString|
//@[15:21)  TypeSyntax
//@[15:21)   Identifier |string|
//@[21:22) NewLine |\n|
param myInt int
//@[0:15) ParameterDeclarationSyntax
//@[0:5)  Identifier |param|
//@[6:11)  IdentifierSyntax
//@[6:11)   Identifier |myInt|
//@[12:15)  TypeSyntax
//@[12:15)   Identifier |int|
//@[15:16) NewLine |\n|
param myBool bool
//@[0:17) ParameterDeclarationSyntax
//@[0:5)  Identifier |param|
//@[6:12)  IdentifierSyntax
//@[6:12)   Identifier |myBool|
//@[13:17)  TypeSyntax
//@[13:17)   Identifier |bool|
//@[17:19) NewLine |\n\n|

// parameters with default value
//@[32:33) NewLine |\n|
param myString2 string = 'strin${2}g value'
//@[0:43) ParameterDeclarationSyntax
//@[0:5)  Identifier |param|
//@[6:15)  IdentifierSyntax
//@[6:15)   Identifier |myString2|
//@[16:22)  TypeSyntax
//@[16:22)   Identifier |string|
//@[23:43)  ParameterDefaultValueSyntax
//@[23:24)   Assignment |=|
//@[25:43)   StringSyntax
//@[25:33)    StringLeftPiece |'strin${|
//@[33:34)    NumericLiteralSyntax
//@[33:34)     Number |2|
//@[34:43)    StringRightPiece |}g value'|
//@[43:44) NewLine |\n|
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
param myTruth bool = true
//@[0:25) ParameterDeclarationSyntax
//@[0:5)  Identifier |param|
//@[6:13)  IdentifierSyntax
//@[6:13)   Identifier |myTruth|
//@[14:18)  TypeSyntax
//@[14:18)   Identifier |bool|
//@[19:25)  ParameterDefaultValueSyntax
//@[19:20)   Assignment |=|
//@[21:25)   BooleanLiteralSyntax
//@[21:25)    TrueKeyword |true|
//@[25:26) NewLine |\n|
param myFalsehood bool = false
//@[0:30) ParameterDeclarationSyntax
//@[0:5)  Identifier |param|
//@[6:17)  IdentifierSyntax
//@[6:17)   Identifier |myFalsehood|
//@[18:22)  TypeSyntax
//@[18:22)   Identifier |bool|
//@[23:30)  ParameterDefaultValueSyntax
//@[23:24)   Assignment |=|
//@[25:30)   BooleanLiteralSyntax
//@[25:30)    FalseKeyword |false|
//@[30:31) NewLine |\n|
param myEscapedString string = 'First line\nSecond\ttabbed\tline'
//@[0:65) ParameterDeclarationSyntax
//@[0:5)  Identifier |param|
//@[6:21)  IdentifierSyntax
//@[6:21)   Identifier |myEscapedString|
//@[22:28)  TypeSyntax
//@[22:28)   Identifier |string|
//@[29:65)  ParameterDefaultValueSyntax
//@[29:30)   Assignment |=|
//@[31:65)   StringSyntax
//@[31:65)    StringComplete |'First line\nSecond\ttabbed\tline'|
//@[65:66) NewLine |\n|
param myNewGuid string = newGuid()
//@[0:34) ParameterDeclarationSyntax
//@[0:5)  Identifier |param|
//@[6:15)  IdentifierSyntax
//@[6:15)   Identifier |myNewGuid|
//@[16:22)  TypeSyntax
//@[16:22)   Identifier |string|
//@[23:34)  ParameterDefaultValueSyntax
//@[23:24)   Assignment |=|
//@[25:34)   FunctionCallSyntax
//@[25:32)    IdentifierSyntax
//@[25:32)     Identifier |newGuid|
//@[32:33)    LeftParen |(|
//@[33:34)    RightParen |)|
//@[34:35) NewLine |\n|
param myUtcTime string = utcNow()
//@[0:33) ParameterDeclarationSyntax
//@[0:5)  Identifier |param|
//@[6:15)  IdentifierSyntax
//@[6:15)   Identifier |myUtcTime|
//@[16:22)  TypeSyntax
//@[16:22)   Identifier |string|
//@[23:33)  ParameterDefaultValueSyntax
//@[23:24)   Assignment |=|
//@[25:33)   FunctionCallSyntax
//@[25:31)    IdentifierSyntax
//@[25:31)     Identifier |utcNow|
//@[31:32)    LeftParen |(|
//@[32:33)    RightParen |)|
//@[33:35) NewLine |\n\n|

// object default value
//@[23:24) NewLine |\n|
param foo object = {
//@[0:288) ParameterDeclarationSyntax
//@[0:5)  Identifier |param|
//@[6:9)  IdentifierSyntax
//@[6:9)   Identifier |foo|
//@[10:16)  TypeSyntax
//@[10:16)   Identifier |object|
//@[17:288)  ParameterDefaultValueSyntax
//@[17:18)   Assignment |=|
//@[19:288)   ObjectSyntax
//@[19:20)    LeftBrace |{|
//@[20:21)    NewLine |\n|
  enabled: true
//@[2:15)    ObjectPropertySyntax
//@[2:9)     IdentifierSyntax
//@[2:9)      Identifier |enabled|
//@[9:10)     Colon |:|
//@[11:15)     BooleanLiteralSyntax
//@[11:15)      TrueKeyword |true|
//@[15:16)    NewLine |\n|
  name: 'this is my object'
//@[2:27)    ObjectPropertySyntax
//@[2:6)     IdentifierSyntax
//@[2:6)      Identifier |name|
//@[6:7)     Colon |:|
//@[8:27)     StringSyntax
//@[8:27)      StringComplete |'this is my object'|
//@[27:28)    NewLine |\n|
  priority: 3
//@[2:13)    ObjectPropertySyntax
//@[2:10)     IdentifierSyntax
//@[2:10)      Identifier |priority|
//@[10:11)     Colon |:|
//@[12:13)     NumericLiteralSyntax
//@[12:13)      Number |3|
//@[13:14)    NewLine |\n|
  info: {
//@[2:24)    ObjectPropertySyntax
//@[2:6)     IdentifierSyntax
//@[2:6)      Identifier |info|
//@[6:7)     Colon |:|
//@[8:24)     ObjectSyntax
//@[8:9)      LeftBrace |{|
//@[9:10)      NewLine |\n|
    a: 'b'
//@[4:10)      ObjectPropertySyntax
//@[4:5)       IdentifierSyntax
//@[4:5)        Identifier |a|
//@[5:6)       Colon |:|
//@[7:10)       StringSyntax
//@[7:10)        StringComplete |'b'|
//@[10:11)      NewLine |\n|
  }
//@[2:3)      RightBrace |}|
//@[3:4)    NewLine |\n|
  empty: {
//@[2:14)    ObjectPropertySyntax
//@[2:7)     IdentifierSyntax
//@[2:7)      Identifier |empty|
//@[7:8)     Colon |:|
//@[9:14)     ObjectSyntax
//@[9:10)      LeftBrace |{|
//@[10:11)      NewLine |\n|
  }
//@[2:3)      RightBrace |}|
//@[3:4)    NewLine |\n|
  array: [
//@[2:111)    ObjectPropertySyntax
//@[2:7)     IdentifierSyntax
//@[2:7)      Identifier |array|
//@[7:8)     Colon |:|
//@[9:111)     ArraySyntax
//@[9:10)      LeftSquare |[|
//@[10:11)      NewLine |\n|
    'string item'
//@[4:18)      ArrayItemSyntax
//@[4:17)       StringSyntax
//@[4:17)        StringComplete |'string item'|
//@[17:18)       NewLine |\n|
    12
//@[4:7)      ArrayItemSyntax
//@[4:6)       NumericLiteralSyntax
//@[4:6)        Number |12|
//@[6:7)       NewLine |\n|
    true
//@[4:9)      ArrayItemSyntax
//@[4:8)       BooleanLiteralSyntax
//@[4:8)        TrueKeyword |true|
//@[8:9)       NewLine |\n|
    [
//@[4:38)      ArrayItemSyntax
//@[4:37)       ArraySyntax
//@[4:5)        LeftSquare |[|
//@[5:6)        NewLine |\n|
      'inner'
//@[6:14)        ArrayItemSyntax
//@[6:13)         StringSyntax
//@[6:13)          StringComplete |'inner'|
//@[13:14)         NewLine |\n|
      false
//@[6:12)        ArrayItemSyntax
//@[6:11)         BooleanLiteralSyntax
//@[6:11)          FalseKeyword |false|
//@[11:12)         NewLine |\n|
    ]
//@[4:5)        RightSquare |]|
//@[5:6)       NewLine |\n|
    {
//@[4:25)      ArrayItemSyntax
//@[4:24)       ObjectSyntax
//@[4:5)        LeftBrace |{|
//@[5:6)        NewLine |\n|
      a: 'b'
//@[6:12)        ObjectPropertySyntax
//@[6:7)         IdentifierSyntax
//@[6:7)          Identifier |a|
//@[7:8)         Colon |:|
//@[9:12)         StringSyntax
//@[9:12)          StringComplete |'b'|
//@[12:13)        NewLine |\n|
    }
//@[4:5)        RightBrace |}|
//@[5:6)       NewLine |\n|
  ]
//@[2:3)      RightSquare |]|
//@[3:4)    NewLine |\n|
  test: {
//@[2:55)    ObjectPropertySyntax
//@[2:6)     IdentifierSyntax
//@[2:6)      Identifier |test|
//@[6:7)     Colon |:|
//@[8:55)     ObjectSyntax
//@[8:9)      LeftBrace |{|
//@[9:10)      NewLine |\n|
    time: utcNow('u')
//@[4:21)      ObjectPropertySyntax
//@[4:8)       IdentifierSyntax
//@[4:8)        Identifier |time|
//@[8:9)       Colon |:|
//@[10:21)       FunctionCallSyntax
//@[10:16)        IdentifierSyntax
//@[10:16)         Identifier |utcNow|
//@[16:17)        LeftParen |(|
//@[17:20)        FunctionArgumentSyntax
//@[17:20)         StringSyntax
//@[17:20)          StringComplete |'u'|
//@[20:21)        RightParen |)|
//@[21:22)      NewLine |\n|
    guid: newGuid()
//@[4:19)      ObjectPropertySyntax
//@[4:8)       IdentifierSyntax
//@[4:8)        Identifier |guid|
//@[8:9)       Colon |:|
//@[10:19)       FunctionCallSyntax
//@[10:17)        IdentifierSyntax
//@[10:17)         Identifier |newGuid|
//@[17:18)        LeftParen |(|
//@[18:19)        RightParen |)|
//@[19:20)      NewLine |\n|
  }
//@[2:3)      RightBrace |}|
//@[3:4)    NewLine |\n|
}
//@[0:1)    RightBrace |}|
//@[1:3) NewLine |\n\n|

// array default value
//@[22:23) NewLine |\n|
param myArrayParam array = [
//@[0:48) ParameterDeclarationSyntax
//@[0:5)  Identifier |param|
//@[6:18)  IdentifierSyntax
//@[6:18)   Identifier |myArrayParam|
//@[19:24)  TypeSyntax
//@[19:24)   Identifier |array|
//@[25:48)  ParameterDefaultValueSyntax
//@[25:26)   Assignment |=|
//@[27:48)   ArraySyntax
//@[27:28)    LeftSquare |[|
//@[28:29)    NewLine |\n|
  'a'
//@[2:6)    ArrayItemSyntax
//@[2:5)     StringSyntax
//@[2:5)      StringComplete |'a'|
//@[5:6)     NewLine |\n|
  'b'
//@[2:6)    ArrayItemSyntax
//@[2:5)     StringSyntax
//@[2:5)      StringComplete |'b'|
//@[5:6)     NewLine |\n|
  'c'
//@[2:6)    ArrayItemSyntax
//@[2:5)     StringSyntax
//@[2:5)      StringComplete |'c'|
//@[5:6)     NewLine |\n|
]
//@[0:1)    RightSquare |]|
//@[1:3) NewLine |\n\n|

// alternative array parameter
//@[30:31) NewLine |\n|
param myAlternativeArrayParam array {
//@[0:107) ParameterDeclarationSyntax
//@[0:5)  Identifier |param|
//@[6:29)  IdentifierSyntax
//@[6:29)   Identifier |myAlternativeArrayParam|
//@[30:35)  TypeSyntax
//@[30:35)   Identifier |array|
//@[36:107)  ObjectSyntax
//@[36:37)   LeftBrace |{|
//@[37:38)   NewLine |\n|
  default: [
//@[2:67)   ObjectPropertySyntax
//@[2:9)    IdentifierSyntax
//@[2:9)     Identifier |default|
//@[9:10)    Colon |:|
//@[11:67)    ArraySyntax
//@[11:12)     LeftSquare |[|
//@[12:13)     NewLine |\n|
    'a'
//@[4:8)     ArrayItemSyntax
//@[4:7)      StringSyntax
//@[4:7)       StringComplete |'a'|
//@[7:8)      NewLine |\n|
    'b'
//@[4:8)     ArrayItemSyntax
//@[4:7)      StringSyntax
//@[4:7)       StringComplete |'b'|
//@[7:8)      NewLine |\n|
    'c'
//@[4:8)     ArrayItemSyntax
//@[4:7)      StringSyntax
//@[4:7)       StringComplete |'c'|
//@[7:8)      NewLine |\n|
    newGuid()
//@[4:14)     ArrayItemSyntax
//@[4:13)      FunctionCallSyntax
//@[4:11)       IdentifierSyntax
//@[4:11)        Identifier |newGuid|
//@[11:12)       LeftParen |(|
//@[12:13)       RightParen |)|
//@[13:14)      NewLine |\n|
    utcNow()
//@[4:13)     ArrayItemSyntax
//@[4:12)      FunctionCallSyntax
//@[4:10)       IdentifierSyntax
//@[4:10)        Identifier |utcNow|
//@[10:11)       LeftParen |(|
//@[11:12)       RightParen |)|
//@[12:13)      NewLine |\n|
  ]
//@[2:3)     RightSquare |]|
//@[3:4)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

// secure string
//@[16:17) NewLine |\n|
param password string {
//@[0:40) ParameterDeclarationSyntax
//@[0:5)  Identifier |param|
//@[6:14)  IdentifierSyntax
//@[6:14)   Identifier |password|
//@[15:21)  TypeSyntax
//@[15:21)   Identifier |string|
//@[22:40)  ObjectSyntax
//@[22:23)   LeftBrace |{|
//@[23:24)   NewLine |\n|
  secure: true
//@[2:14)   ObjectPropertySyntax
//@[2:8)    IdentifierSyntax
//@[2:8)     Identifier |secure|
//@[8:9)    Colon |:|
//@[10:14)    BooleanLiteralSyntax
//@[10:14)     TrueKeyword |true|
//@[14:15)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

// non-secure string
//@[20:21) NewLine |\n|
param nonSecure string {
//@[0:42) ParameterDeclarationSyntax
//@[0:5)  Identifier |param|
//@[6:15)  IdentifierSyntax
//@[6:15)   Identifier |nonSecure|
//@[16:22)  TypeSyntax
//@[16:22)   Identifier |string|
//@[23:42)  ObjectSyntax
//@[23:24)   LeftBrace |{|
//@[24:25)   NewLine |\n|
  secure: false
//@[2:15)   ObjectPropertySyntax
//@[2:8)    IdentifierSyntax
//@[2:8)     Identifier |secure|
//@[8:9)    Colon |:|
//@[10:15)    BooleanLiteralSyntax
//@[10:15)     FalseKeyword |false|
//@[15:16)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

// secure object
//@[16:17) NewLine |\n|
param secretObject object {
//@[0:44) ParameterDeclarationSyntax
//@[0:5)  Identifier |param|
//@[6:18)  IdentifierSyntax
//@[6:18)   Identifier |secretObject|
//@[19:25)  TypeSyntax
//@[19:25)   Identifier |object|
//@[26:44)  ObjectSyntax
//@[26:27)   LeftBrace |{|
//@[27:28)   NewLine |\n|
  secure: true
//@[2:14)   ObjectPropertySyntax
//@[2:8)    IdentifierSyntax
//@[2:8)     Identifier |secure|
//@[8:9)    Colon |:|
//@[10:14)    BooleanLiteralSyntax
//@[10:14)     TrueKeyword |true|
//@[14:15)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

// enum parameter
//@[17:18) NewLine |\n|
param storageSku string {
//@[0:82) ParameterDeclarationSyntax
//@[0:5)  Identifier |param|
//@[6:16)  IdentifierSyntax
//@[6:16)   Identifier |storageSku|
//@[17:23)  TypeSyntax
//@[17:23)   Identifier |string|
//@[24:82)  ObjectSyntax
//@[24:25)   LeftBrace |{|
//@[25:26)   NewLine |\n|
  allowed: [
//@[2:54)   ObjectPropertySyntax
//@[2:9)    IdentifierSyntax
//@[2:9)     Identifier |allowed|
//@[9:10)    Colon |:|
//@[11:54)    ArraySyntax
//@[11:12)     LeftSquare |[|
//@[12:13)     NewLine |\n|
    'Standard_LRS'
//@[4:19)     ArrayItemSyntax
//@[4:18)      StringSyntax
//@[4:18)       StringComplete |'Standard_LRS'|
//@[18:19)      NewLine |\n|
    'Standard_GRS'
//@[4:19)     ArrayItemSyntax
//@[4:18)      StringSyntax
//@[4:18)       StringComplete |'Standard_GRS'|
//@[18:19)      NewLine |\n|
  ]
//@[2:3)     RightSquare |]|
//@[3:4)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

// length constraint on a string
//@[32:33) NewLine |\n|
param storageName string {
//@[0:59) ParameterDeclarationSyntax
//@[0:5)  Identifier |param|
//@[6:17)  IdentifierSyntax
//@[6:17)   Identifier |storageName|
//@[18:24)  TypeSyntax
//@[18:24)   Identifier |string|
//@[25:59)  ObjectSyntax
//@[25:26)   LeftBrace |{|
//@[26:27)   NewLine |\n|
  minLength: 3
//@[2:14)   ObjectPropertySyntax
//@[2:11)    IdentifierSyntax
//@[2:11)     Identifier |minLength|
//@[11:12)    Colon |:|
//@[13:14)    NumericLiteralSyntax
//@[13:14)     Number |3|
//@[14:15)   NewLine |\n|
  maxLength: 24
//@[2:15)   ObjectPropertySyntax
//@[2:11)    IdentifierSyntax
//@[2:11)     Identifier |maxLength|
//@[11:12)    Colon |:|
//@[13:15)    NumericLiteralSyntax
//@[13:15)     Number |24|
//@[15:16)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

// length constraint on an array
//@[32:33) NewLine |\n|
param someArray array {
//@[0:56) ParameterDeclarationSyntax
//@[0:5)  Identifier |param|
//@[6:15)  IdentifierSyntax
//@[6:15)   Identifier |someArray|
//@[16:21)  TypeSyntax
//@[16:21)   Identifier |array|
//@[22:56)  ObjectSyntax
//@[22:23)   LeftBrace |{|
//@[23:24)   NewLine |\n|
  minLength: 3
//@[2:14)   ObjectPropertySyntax
//@[2:11)    IdentifierSyntax
//@[2:11)     Identifier |minLength|
//@[11:12)    Colon |:|
//@[13:14)    NumericLiteralSyntax
//@[13:14)     Number |3|
//@[14:15)   NewLine |\n|
  maxLength: 24
//@[2:15)   ObjectPropertySyntax
//@[2:11)    IdentifierSyntax
//@[2:11)     Identifier |maxLength|
//@[11:12)    Colon |:|
//@[13:15)    NumericLiteralSyntax
//@[13:15)     Number |24|
//@[15:16)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

// empty metadata
//@[17:18) NewLine |\n|
param emptyMetadata string {
//@[0:48) ParameterDeclarationSyntax
//@[0:5)  Identifier |param|
//@[6:19)  IdentifierSyntax
//@[6:19)   Identifier |emptyMetadata|
//@[20:26)  TypeSyntax
//@[20:26)   Identifier |string|
//@[27:48)  ObjectSyntax
//@[27:28)   LeftBrace |{|
//@[28:29)   NewLine |\n|
  metadata: {
//@[2:17)   ObjectPropertySyntax
//@[2:10)    IdentifierSyntax
//@[2:10)     Identifier |metadata|
//@[10:11)    Colon |:|
//@[12:17)    ObjectSyntax
//@[12:13)     LeftBrace |{|
//@[13:14)     NewLine |\n|
  }
//@[2:3)     RightBrace |}|
//@[3:4)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

// description
//@[14:15) NewLine |\n|
param description string {
//@[0:80) ParameterDeclarationSyntax
//@[0:5)  Identifier |param|
//@[6:17)  IdentifierSyntax
//@[6:17)   Identifier |description|
//@[18:24)  TypeSyntax
//@[18:24)   Identifier |string|
//@[25:80)  ObjectSyntax
//@[25:26)   LeftBrace |{|
//@[26:27)   NewLine |\n|
  metadata: {
//@[2:51)   ObjectPropertySyntax
//@[2:10)    IdentifierSyntax
//@[2:10)     Identifier |metadata|
//@[10:11)    Colon |:|
//@[12:51)    ObjectSyntax
//@[12:13)     LeftBrace |{|
//@[13:14)     NewLine |\n|
    description: 'my description'
//@[4:33)     ObjectPropertySyntax
//@[4:15)      IdentifierSyntax
//@[4:15)       Identifier |description|
//@[15:16)      Colon |:|
//@[17:33)      StringSyntax
//@[17:33)       StringComplete |'my description'|
//@[33:34)     NewLine |\n|
  }
//@[2:3)     RightBrace |}|
//@[3:4)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

// random extra metadata
//@[24:25) NewLine |\n|
param additionalMetadata string {
//@[0:156) ParameterDeclarationSyntax
//@[0:5)  Identifier |param|
//@[6:24)  IdentifierSyntax
//@[6:24)   Identifier |additionalMetadata|
//@[25:31)  TypeSyntax
//@[25:31)   Identifier |string|
//@[32:156)  ObjectSyntax
//@[32:33)   LeftBrace |{|
//@[33:34)   NewLine |\n|
  metadata: {
//@[2:120)   ObjectPropertySyntax
//@[2:10)    IdentifierSyntax
//@[2:10)     Identifier |metadata|
//@[10:11)    Colon |:|
//@[12:120)    ObjectSyntax
//@[12:13)     LeftBrace |{|
//@[13:14)     NewLine |\n|
    description: 'my description'
//@[4:33)     ObjectPropertySyntax
//@[4:15)      IdentifierSyntax
//@[4:15)       Identifier |description|
//@[15:16)      Colon |:|
//@[17:33)      StringSyntax
//@[17:33)       StringComplete |'my description'|
//@[33:34)     NewLine |\n|
    a: 1
//@[4:8)     ObjectPropertySyntax
//@[4:5)      IdentifierSyntax
//@[4:5)       Identifier |a|
//@[5:6)      Colon |:|
//@[7:8)      NumericLiteralSyntax
//@[7:8)       Number |1|
//@[8:9)     NewLine |\n|
    b: true
//@[4:11)     ObjectPropertySyntax
//@[4:5)      IdentifierSyntax
//@[4:5)       Identifier |b|
//@[5:6)      Colon |:|
//@[7:11)      BooleanLiteralSyntax
//@[7:11)       TrueKeyword |true|
//@[11:12)     NewLine |\n|
    c: [
//@[4:14)     ObjectPropertySyntax
//@[4:5)      IdentifierSyntax
//@[4:5)       Identifier |c|
//@[5:6)      Colon |:|
//@[7:14)      ArraySyntax
//@[7:8)       LeftSquare |[|
//@[8:9)       NewLine |\n|
    ]
//@[4:5)       RightSquare |]|
//@[5:6)     NewLine |\n|
    d: {
//@[4:32)     ObjectPropertySyntax
//@[4:5)      IdentifierSyntax
//@[4:5)       Identifier |d|
//@[5:6)      Colon |:|
//@[7:32)      ObjectSyntax
//@[7:8)       LeftBrace |{|
//@[8:9)       NewLine |\n|
      test: 'abc'
//@[6:17)       ObjectPropertySyntax
//@[6:10)        IdentifierSyntax
//@[6:10)         Identifier |test|
//@[10:11)        Colon |:|
//@[12:17)        StringSyntax
//@[12:17)         StringComplete |'abc'|
//@[17:18)       NewLine |\n|
    }
//@[4:5)       RightBrace |}|
//@[5:6)     NewLine |\n|
  }
//@[2:3)     RightBrace |}|
//@[3:4)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

// all modifiers together
//@[25:26) NewLine |\n|
param someParameter string {
//@[0:207) ParameterDeclarationSyntax
//@[0:5)  Identifier |param|
//@[6:19)  IdentifierSyntax
//@[6:19)   Identifier |someParameter|
//@[20:26)  TypeSyntax
//@[20:26)   Identifier |string|
//@[27:207)  ObjectSyntax
//@[27:28)   LeftBrace |{|
//@[28:29)   NewLine |\n|
  secure: true
//@[2:14)   ObjectPropertySyntax
//@[2:8)    IdentifierSyntax
//@[2:8)     Identifier |secure|
//@[8:9)    Colon |:|
//@[10:14)    BooleanLiteralSyntax
//@[10:14)     TrueKeyword |true|
//@[14:15)   NewLine |\n|
  minLength: 3
//@[2:14)   ObjectPropertySyntax
//@[2:11)    IdentifierSyntax
//@[2:11)     Identifier |minLength|
//@[11:12)    Colon |:|
//@[13:14)    NumericLiteralSyntax
//@[13:14)     Number |3|
//@[14:15)   NewLine |\n|
  maxLength: 24
//@[2:15)   ObjectPropertySyntax
//@[2:11)    IdentifierSyntax
//@[2:11)     Identifier |maxLength|
//@[11:12)    Colon |:|
//@[13:15)    NumericLiteralSyntax
//@[13:15)     Number |24|
//@[15:16)   NewLine |\n|
  default: 'one'
//@[2:16)   ObjectPropertySyntax
//@[2:9)    IdentifierSyntax
//@[2:9)     Identifier |default|
//@[9:10)    Colon |:|
//@[11:16)    StringSyntax
//@[11:16)     StringComplete |'one'|
//@[16:17)   NewLine |\n|
  allowed: [
//@[2:48)   ObjectPropertySyntax
//@[2:9)    IdentifierSyntax
//@[2:9)     Identifier |allowed|
//@[9:10)    Colon |:|
//@[11:48)    ArraySyntax
//@[11:12)     LeftSquare |[|
//@[12:13)     NewLine |\n|
    'one'
//@[4:10)     ArrayItemSyntax
//@[4:9)      StringSyntax
//@[4:9)       StringComplete |'one'|
//@[9:10)      NewLine |\n|
    'two'
//@[4:10)     ArrayItemSyntax
//@[4:9)      StringSyntax
//@[4:9)       StringComplete |'two'|
//@[9:10)      NewLine |\n|
    'three'
//@[4:12)     ArrayItemSyntax
//@[4:11)      StringSyntax
//@[4:11)       StringComplete |'three'|
//@[11:12)      NewLine |\n|
  ]
//@[2:3)     RightSquare |]|
//@[3:4)   NewLine |\n|
  metadata: {
//@[2:64)   ObjectPropertySyntax
//@[2:10)    IdentifierSyntax
//@[2:10)     Identifier |metadata|
//@[10:11)    Colon |:|
//@[12:64)    ObjectSyntax
//@[12:13)     LeftBrace |{|
//@[13:14)     NewLine |\n|
    description: 'Name of the storage account'
//@[4:46)     ObjectPropertySyntax
//@[4:15)      IdentifierSyntax
//@[4:15)       Identifier |description|
//@[15:16)      Colon |:|
//@[17:46)      StringSyntax
//@[17:46)       StringComplete |'Name of the storage account'|
//@[46:47)     NewLine |\n|
  }
//@[2:3)     RightBrace |}|
//@[3:4)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

param defaultValueExpression int {
//@[0:66) ParameterDeclarationSyntax
//@[0:5)  Identifier |param|
//@[6:28)  IdentifierSyntax
//@[6:28)   Identifier |defaultValueExpression|
//@[29:32)  TypeSyntax
//@[29:32)   Identifier |int|
//@[33:66)  ObjectSyntax
//@[33:34)   LeftBrace |{|
//@[34:35)   NewLine |\n|
  default: true ? 4 + 2*3 : 0
//@[2:29)   ObjectPropertySyntax
//@[2:9)    IdentifierSyntax
//@[2:9)     Identifier |default|
//@[9:10)    Colon |:|
//@[11:29)    TernaryOperationSyntax
//@[11:15)     BooleanLiteralSyntax
//@[11:15)      TrueKeyword |true|
//@[16:17)     Question |?|
//@[18:25)     BinaryOperationSyntax
//@[18:19)      NumericLiteralSyntax
//@[18:19)       Number |4|
//@[20:21)      Plus |+|
//@[22:25)      BinaryOperationSyntax
//@[22:23)       NumericLiteralSyntax
//@[22:23)        Number |2|
//@[23:24)       Asterisk |*|
//@[24:25)       NumericLiteralSyntax
//@[24:25)        Number |3|
//@[26:27)     Colon |:|
//@[28:29)     NumericLiteralSyntax
//@[28:29)      Number |0|
//@[29:30)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

param defaultExpression bool = 18 != (true || false)
//@[0:52) ParameterDeclarationSyntax
//@[0:5)  Identifier |param|
//@[6:23)  IdentifierSyntax
//@[6:23)   Identifier |defaultExpression|
//@[24:28)  TypeSyntax
//@[24:28)   Identifier |bool|
//@[29:52)  ParameterDefaultValueSyntax
//@[29:30)   Assignment |=|
//@[31:52)   BinaryOperationSyntax
//@[31:33)    NumericLiteralSyntax
//@[31:33)     Number |18|
//@[34:36)    NotEquals |!=|
//@[37:52)    ParenthesizedExpressionSyntax
//@[37:38)     LeftParen |(|
//@[38:51)     BinaryOperationSyntax
//@[38:42)      BooleanLiteralSyntax
//@[38:42)       TrueKeyword |true|
//@[43:45)      LogicalOr ||||
//@[46:51)      BooleanLiteralSyntax
//@[46:51)       FalseKeyword |false|
//@[51:52)     RightParen |)|
//@[52:53) NewLine |\n|

//@[0:0) EndOfFile ||
