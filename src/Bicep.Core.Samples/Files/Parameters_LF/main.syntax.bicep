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
//@[33:34)    IntegerLiteralSyntax
//@[33:34)     Integer |2|
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
//@[19:21)   IntegerLiteralSyntax
//@[19:21)    Integer |42|
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
//@[12:13)     IntegerLiteralSyntax
//@[12:13)      Integer |3|
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
//@[4:17)      ArrayItemSyntax
//@[4:17)       StringSyntax
//@[4:17)        StringComplete |'string item'|
//@[17:18)      NewLine |\n|
    12
//@[4:6)      ArrayItemSyntax
//@[4:6)       IntegerLiteralSyntax
//@[4:6)        Integer |12|
//@[6:7)      NewLine |\n|
    true
//@[4:8)      ArrayItemSyntax
//@[4:8)       BooleanLiteralSyntax
//@[4:8)        TrueKeyword |true|
//@[8:9)      NewLine |\n|
    [
//@[4:37)      ArrayItemSyntax
//@[4:37)       ArraySyntax
//@[4:5)        LeftSquare |[|
//@[5:6)        NewLine |\n|
      'inner'
//@[6:13)        ArrayItemSyntax
//@[6:13)         StringSyntax
//@[6:13)          StringComplete |'inner'|
//@[13:14)        NewLine |\n|
      false
//@[6:11)        ArrayItemSyntax
//@[6:11)         BooleanLiteralSyntax
//@[6:11)          FalseKeyword |false|
//@[11:12)        NewLine |\n|
    ]
//@[4:5)        RightSquare |]|
//@[5:6)      NewLine |\n|
    {
//@[4:24)      ArrayItemSyntax
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
//@[5:6)      NewLine |\n|
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
//@[2:5)    ArrayItemSyntax
//@[2:5)     StringSyntax
//@[2:5)      StringComplete |'a'|
//@[5:6)    NewLine |\n|
  'b'
//@[2:5)    ArrayItemSyntax
//@[2:5)     StringSyntax
//@[2:5)      StringComplete |'b'|
//@[5:6)    NewLine |\n|
  'c'
//@[2:5)    ArrayItemSyntax
//@[2:5)     StringSyntax
//@[2:5)      StringComplete |'c'|
//@[5:6)    NewLine |\n|
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
//@[4:7)     ArrayItemSyntax
//@[4:7)      StringSyntax
//@[4:7)       StringComplete |'a'|
//@[7:8)     NewLine |\n|
    'b'
//@[4:7)     ArrayItemSyntax
//@[4:7)      StringSyntax
//@[4:7)       StringComplete |'b'|
//@[7:8)     NewLine |\n|
    'c'
//@[4:7)     ArrayItemSyntax
//@[4:7)      StringSyntax
//@[4:7)       StringComplete |'c'|
//@[7:8)     NewLine |\n|
    newGuid()
//@[4:13)     ArrayItemSyntax
//@[4:13)      FunctionCallSyntax
//@[4:11)       IdentifierSyntax
//@[4:11)        Identifier |newGuid|
//@[11:12)       LeftParen |(|
//@[12:13)       RightParen |)|
//@[13:14)     NewLine |\n|
    utcNow()
//@[4:12)     ArrayItemSyntax
//@[4:12)      FunctionCallSyntax
//@[4:10)       IdentifierSyntax
//@[4:10)        Identifier |utcNow|
//@[10:11)       LeftParen |(|
//@[11:12)       RightParen |)|
//@[12:13)     NewLine |\n|
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

@secure()
//@[0:44) ParameterDeclarationSyntax
//@[0:9)  DecoratorSyntax
//@[0:1)   At |@|
//@[1:9)   FunctionCallSyntax
//@[1:7)    IdentifierSyntax
//@[1:7)     Identifier |secure|
//@[7:8)    LeftParen |(|
//@[8:9)    RightParen |)|
//@[9:10)  NewLine |\n|
param passwordWithDecorator string
//@[0:5)  Identifier |param|
//@[6:27)  IdentifierSyntax
//@[6:27)   Identifier |passwordWithDecorator|
//@[28:34)  TypeSyntax
//@[28:34)   Identifier |string|
//@[34:36) NewLine |\n\n|

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

@secure()
//@[0:48) ParameterDeclarationSyntax
//@[0:9)  DecoratorSyntax
//@[0:1)   At |@|
//@[1:9)   FunctionCallSyntax
//@[1:7)    IdentifierSyntax
//@[1:7)     Identifier |secure|
//@[7:8)    LeftParen |(|
//@[8:9)    RightParen |)|
//@[9:10)  NewLine |\n|
param secureObjectWithDecorator object
//@[0:5)  Identifier |param|
//@[6:31)  IdentifierSyntax
//@[6:31)   Identifier |secureObjectWithDecorator|
//@[32:38)  TypeSyntax
//@[32:38)   Identifier |object|
//@[38:40) NewLine |\n\n|

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
//@[4:18)     ArrayItemSyntax
//@[4:18)      StringSyntax
//@[4:18)       StringComplete |'Standard_LRS'|
//@[18:19)     NewLine |\n|
    'Standard_GRS'
//@[4:18)     ArrayItemSyntax
//@[4:18)      StringSyntax
//@[4:18)       StringComplete |'Standard_GRS'|
//@[18:19)     NewLine |\n|
  ]
//@[2:3)     RightSquare |]|
//@[3:4)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

@  allowed([
//@[0:82) ParameterDeclarationSyntax
//@[0:45)  DecoratorSyntax
//@[0:1)   At |@|
//@[3:45)   FunctionCallSyntax
//@[3:10)    IdentifierSyntax
//@[3:10)     Identifier |allowed|
//@[10:11)    LeftParen |(|
//@[11:44)    FunctionArgumentSyntax
//@[11:44)     ArraySyntax
//@[11:12)      LeftSquare |[|
//@[12:13)      NewLine |\n|
'Standard_LRS'
//@[0:14)      ArrayItemSyntax
//@[0:14)       StringSyntax
//@[0:14)        StringComplete |'Standard_LRS'|
//@[14:15)      NewLine |\n|
'Standard_GRS'
//@[0:14)      ArrayItemSyntax
//@[0:14)       StringSyntax
//@[0:14)        StringComplete |'Standard_GRS'|
//@[14:15)      NewLine |\n|
])
//@[0:1)      RightSquare |]|
//@[1:2)    RightParen |)|
//@[2:3)  NewLine |\n|
param storageSkuWithDecorator string
//@[0:5)  Identifier |param|
//@[6:29)  IdentifierSyntax
//@[6:29)   Identifier |storageSkuWithDecorator|
//@[30:36)  TypeSyntax
//@[30:36)   Identifier |string|
//@[36:38) NewLine |\n\n|

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
//@[13:14)    IntegerLiteralSyntax
//@[13:14)     Integer |3|
//@[14:15)   NewLine |\n|
  maxLength: 24
//@[2:15)   ObjectPropertySyntax
//@[2:11)    IdentifierSyntax
//@[2:11)     Identifier |maxLength|
//@[11:12)    Colon |:|
//@[13:15)    IntegerLiteralSyntax
//@[13:15)     Integer |24|
//@[15:16)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

@minLength(3)
//@[0:66) ParameterDeclarationSyntax
//@[0:13)  DecoratorSyntax
//@[0:1)   At |@|
//@[1:13)   FunctionCallSyntax
//@[1:10)    IdentifierSyntax
//@[1:10)     Identifier |minLength|
//@[10:11)    LeftParen |(|
//@[11:12)    FunctionArgumentSyntax
//@[11:12)     IntegerLiteralSyntax
//@[11:12)      Integer |3|
//@[12:13)    RightParen |)|
//@[13:14)  NewLine |\n|
@maxLength(24)
//@[0:14)  DecoratorSyntax
//@[0:1)   At |@|
//@[1:14)   FunctionCallSyntax
//@[1:10)    IdentifierSyntax
//@[1:10)     Identifier |maxLength|
//@[10:11)    LeftParen |(|
//@[11:13)    FunctionArgumentSyntax
//@[11:13)     IntegerLiteralSyntax
//@[11:13)      Integer |24|
//@[13:14)    RightParen |)|
//@[14:15)  NewLine |\n|
param storageNameWithDecorator string
//@[0:5)  Identifier |param|
//@[6:30)  IdentifierSyntax
//@[6:30)   Identifier |storageNameWithDecorator|
//@[31:37)  TypeSyntax
//@[31:37)   Identifier |string|
//@[37:39) NewLine |\n\n|

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
//@[13:14)    IntegerLiteralSyntax
//@[13:14)     Integer |3|
//@[14:15)   NewLine |\n|
  maxLength: 24
//@[2:15)   ObjectPropertySyntax
//@[2:11)    IdentifierSyntax
//@[2:11)     Identifier |maxLength|
//@[11:12)    Colon |:|
//@[13:15)    IntegerLiteralSyntax
//@[13:15)     Integer |24|
//@[15:16)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

@minLength(3)
//@[0:63) ParameterDeclarationSyntax
//@[0:13)  DecoratorSyntax
//@[0:1)   At |@|
//@[1:13)   FunctionCallSyntax
//@[1:10)    IdentifierSyntax
//@[1:10)     Identifier |minLength|
//@[10:11)    LeftParen |(|
//@[11:12)    FunctionArgumentSyntax
//@[11:12)     IntegerLiteralSyntax
//@[11:12)      Integer |3|
//@[12:13)    RightParen |)|
//@[13:14)  NewLine |\n|
@maxLength(24)
//@[0:14)  DecoratorSyntax
//@[0:1)   At |@|
//@[1:14)   FunctionCallSyntax
//@[1:10)    IdentifierSyntax
//@[1:10)     Identifier |maxLength|
//@[10:11)    LeftParen |(|
//@[11:13)    FunctionArgumentSyntax
//@[11:13)     IntegerLiteralSyntax
//@[11:13)      Integer |24|
//@[13:14)    RightParen |)|
//@[14:15)  NewLine |\n|
param someArrayWithDecorator array
//@[0:5)  Identifier |param|
//@[6:28)  IdentifierSyntax
//@[6:28)   Identifier |someArrayWithDecorator|
//@[29:34)  TypeSyntax
//@[29:34)   Identifier |array|
//@[34:36) NewLine |\n\n|

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

@metadata({})
//@[0:53) ParameterDeclarationSyntax
//@[0:13)  DecoratorSyntax
//@[0:1)   At |@|
//@[1:13)   FunctionCallSyntax
//@[1:9)    IdentifierSyntax
//@[1:9)     Identifier |metadata|
//@[9:10)    LeftParen |(|
//@[10:12)    FunctionArgumentSyntax
//@[10:12)     ObjectSyntax
//@[10:11)      LeftBrace |{|
//@[11:12)      RightBrace |}|
//@[12:13)    RightParen |)|
//@[13:14)  NewLine |\n|
param emptyMetadataWithDecorator string
//@[0:5)  Identifier |param|
//@[6:32)  IdentifierSyntax
//@[6:32)   Identifier |emptyMetadataWithDecorator|
//@[33:39)  TypeSyntax
//@[33:39)   Identifier |string|
//@[39:41) NewLine |\n\n|

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

@metadata({
//@[0:84) ParameterDeclarationSyntax
//@[0:46)  DecoratorSyntax
//@[0:1)   At |@|
//@[1:46)   FunctionCallSyntax
//@[1:9)    IdentifierSyntax
//@[1:9)     Identifier |metadata|
//@[9:10)    LeftParen |(|
//@[10:45)    FunctionArgumentSyntax
//@[10:45)     ObjectSyntax
//@[10:11)      LeftBrace |{|
//@[11:12)      NewLine |\n|
  description: 'my description'
//@[2:31)      ObjectPropertySyntax
//@[2:13)       IdentifierSyntax
//@[2:13)        Identifier |description|
//@[13:14)       Colon |:|
//@[15:31)       StringSyntax
//@[15:31)        StringComplete |'my description'|
//@[31:32)      NewLine |\n|
})
//@[0:1)      RightBrace |}|
//@[1:2)    RightParen |)|
//@[2:3)  NewLine |\n|
param descriptionWithDecorator string
//@[0:5)  Identifier |param|
//@[6:30)  IdentifierSyntax
//@[6:30)   Identifier |descriptionWithDecorator|
//@[31:37)  TypeSyntax
//@[31:37)   Identifier |string|
//@[37:39) NewLine |\n\n|

@sys.description('my description')
//@[0:73) ParameterDeclarationSyntax
//@[0:34)  DecoratorSyntax
//@[0:1)   At |@|
//@[1:34)   InstanceFunctionCallSyntax
//@[1:4)    VariableAccessSyntax
//@[1:4)     IdentifierSyntax
//@[1:4)      Identifier |sys|
//@[4:5)    Dot |.|
//@[5:16)    IdentifierSyntax
//@[5:16)     Identifier |description|
//@[16:17)    LeftParen |(|
//@[17:33)    FunctionArgumentSyntax
//@[17:33)     StringSyntax
//@[17:33)      StringComplete |'my description'|
//@[33:34)    RightParen |)|
//@[34:35)  NewLine |\n|
param descriptionWithDecorator2 string
//@[0:5)  Identifier |param|
//@[6:31)  IdentifierSyntax
//@[6:31)   Identifier |descriptionWithDecorator2|
//@[32:38)  TypeSyntax
//@[32:38)   Identifier |string|
//@[38:40) NewLine |\n\n|

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
//@[7:8)      IntegerLiteralSyntax
//@[7:8)       Integer |1|
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

@metadata({
//@[0:138) ParameterDeclarationSyntax
//@[0:93)  DecoratorSyntax
//@[0:1)   At |@|
//@[1:93)   FunctionCallSyntax
//@[1:9)    IdentifierSyntax
//@[1:9)     Identifier |metadata|
//@[9:10)    LeftParen |(|
//@[10:92)    FunctionArgumentSyntax
//@[10:92)     ObjectSyntax
//@[10:11)      LeftBrace |{|
//@[11:12)      NewLine |\n|
	description: 'my description'
//@[1:30)      ObjectPropertySyntax
//@[1:12)       IdentifierSyntax
//@[1:12)        Identifier |description|
//@[12:13)       Colon |:|
//@[14:30)       StringSyntax
//@[14:30)        StringComplete |'my description'|
//@[30:31)      NewLine |\n|
	a: 1
//@[1:5)      ObjectPropertySyntax
//@[1:2)       IdentifierSyntax
//@[1:2)        Identifier |a|
//@[2:3)       Colon |:|
//@[4:5)       IntegerLiteralSyntax
//@[4:5)        Integer |1|
//@[5:6)      NewLine |\n|
	b: true
//@[1:8)      ObjectPropertySyntax
//@[1:2)       IdentifierSyntax
//@[1:2)        Identifier |b|
//@[2:3)       Colon |:|
//@[4:8)       BooleanLiteralSyntax
//@[4:8)        TrueKeyword |true|
//@[8:9)      NewLine |\n|
	c: [
//@[1:8)      ObjectPropertySyntax
//@[1:2)       IdentifierSyntax
//@[1:2)        Identifier |c|
//@[2:3)       Colon |:|
//@[4:8)       ArraySyntax
//@[4:5)        LeftSquare |[|
//@[5:6)        NewLine |\n|
	]
//@[1:2)        RightSquare |]|
//@[2:3)      NewLine |\n|
	d: {
//@[1:23)      ObjectPropertySyntax
//@[1:2)       IdentifierSyntax
//@[1:2)        Identifier |d|
//@[2:3)       Colon |:|
//@[4:23)       ObjectSyntax
//@[4:5)        LeftBrace |{|
//@[5:6)        NewLine |\n|
	  test: 'abc'
//@[3:14)        ObjectPropertySyntax
//@[3:7)         IdentifierSyntax
//@[3:7)          Identifier |test|
//@[7:8)         Colon |:|
//@[9:14)         StringSyntax
//@[9:14)          StringComplete |'abc'|
//@[14:15)        NewLine |\n|
	}
//@[1:2)        RightBrace |}|
//@[2:3)      NewLine |\n|
})
//@[0:1)      RightBrace |}|
//@[1:2)    RightParen |)|
//@[2:3)  NewLine |\n|
param additionalMetadataWithDecorator string
//@[0:5)  Identifier |param|
//@[6:37)  IdentifierSyntax
//@[6:37)   Identifier |additionalMetadataWithDecorator|
//@[38:44)  TypeSyntax
//@[38:44)   Identifier |string|
//@[44:46) NewLine |\n\n|

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
//@[13:14)    IntegerLiteralSyntax
//@[13:14)     Integer |3|
//@[14:15)   NewLine |\n|
  maxLength: 24
//@[2:15)   ObjectPropertySyntax
//@[2:11)    IdentifierSyntax
//@[2:11)     Identifier |maxLength|
//@[11:12)    Colon |:|
//@[13:15)    IntegerLiteralSyntax
//@[13:15)     Integer |24|
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
//@[4:9)     ArrayItemSyntax
//@[4:9)      StringSyntax
//@[4:9)       StringComplete |'one'|
//@[9:10)     NewLine |\n|
    'two'
//@[4:9)     ArrayItemSyntax
//@[4:9)      StringSyntax
//@[4:9)       StringComplete |'two'|
//@[9:10)     NewLine |\n|
    'three'
//@[4:11)     ArrayItemSyntax
//@[4:11)      StringSyntax
//@[4:11)       StringComplete |'three'|
//@[11:12)     NewLine |\n|
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

@secure()
//@[0:186) ParameterDeclarationSyntax
//@[0:9)  DecoratorSyntax
//@[0:1)   At |@|
//@[1:9)   FunctionCallSyntax
//@[1:7)    IdentifierSyntax
//@[1:7)     Identifier |secure|
//@[7:8)    LeftParen |(|
//@[8:9)    RightParen |)|
//@[9:10)  NewLine |\n|
@minLength(3)
//@[0:13)  DecoratorSyntax
//@[0:1)   At |@|
//@[1:13)   FunctionCallSyntax
//@[1:10)    IdentifierSyntax
//@[1:10)     Identifier |minLength|
//@[10:11)    LeftParen |(|
//@[11:12)    FunctionArgumentSyntax
//@[11:12)     IntegerLiteralSyntax
//@[11:12)      Integer |3|
//@[12:13)    RightParen |)|
//@[13:14)  NewLine |\n|
@maxLength(24)
//@[0:14)  DecoratorSyntax
//@[0:1)   At |@|
//@[1:14)   FunctionCallSyntax
//@[1:10)    IdentifierSyntax
//@[1:10)     Identifier |maxLength|
//@[10:11)    LeftParen |(|
//@[11:13)    FunctionArgumentSyntax
//@[11:13)     IntegerLiteralSyntax
//@[11:13)      Integer |24|
//@[13:14)    RightParen |)|
//@[14:15)  NewLine |\n|
@allowed([
//@[0:39)  DecoratorSyntax
//@[0:1)   At |@|
//@[1:39)   FunctionCallSyntax
//@[1:8)    IdentifierSyntax
//@[1:8)     Identifier |allowed|
//@[8:9)    LeftParen |(|
//@[9:38)    FunctionArgumentSyntax
//@[9:38)     ArraySyntax
//@[9:10)      LeftSquare |[|
//@[10:11)      NewLine |\n|
  'one'
//@[2:7)      ArrayItemSyntax
//@[2:7)       StringSyntax
//@[2:7)        StringComplete |'one'|
//@[7:8)      NewLine |\n|
  'two'
//@[2:7)      ArrayItemSyntax
//@[2:7)       StringSyntax
//@[2:7)        StringComplete |'two'|
//@[7:8)      NewLine |\n|
  'three'
//@[2:9)      ArrayItemSyntax
//@[2:9)       StringSyntax
//@[2:9)        StringComplete |'three'|
//@[9:10)      NewLine |\n|
])
//@[0:1)      RightSquare |]|
//@[1:2)    RightParen |)|
//@[2:3)  NewLine |\n|
@metadata({
//@[0:59)  DecoratorSyntax
//@[0:1)   At |@|
//@[1:59)   FunctionCallSyntax
//@[1:9)    IdentifierSyntax
//@[1:9)     Identifier |metadata|
//@[9:10)    LeftParen |(|
//@[10:58)    FunctionArgumentSyntax
//@[10:58)     ObjectSyntax
//@[10:11)      LeftBrace |{|
//@[11:12)      NewLine |\n|
  description: 'Name of the storage account'
//@[2:44)      ObjectPropertySyntax
//@[2:13)       IdentifierSyntax
//@[2:13)        Identifier |description|
//@[13:14)       Colon |:|
//@[15:44)       StringSyntax
//@[15:44)        StringComplete |'Name of the storage account'|
//@[44:45)      NewLine |\n|
})
//@[0:1)      RightBrace |}|
//@[1:2)    RightParen |)|
//@[2:3)  NewLine |\n|
param someParameterWithDecorator string = 'one'
//@[0:5)  Identifier |param|
//@[6:32)  IdentifierSyntax
//@[6:32)   Identifier |someParameterWithDecorator|
//@[33:39)  TypeSyntax
//@[33:39)   Identifier |string|
//@[40:47)  ParameterDefaultValueSyntax
//@[40:41)   Assignment |=|
//@[42:47)   StringSyntax
//@[42:47)    StringComplete |'one'|
//@[47:49) NewLine |\n\n|

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
//@[18:19)      IntegerLiteralSyntax
//@[18:19)       Integer |4|
//@[20:21)      Plus |+|
//@[22:25)      BinaryOperationSyntax
//@[22:23)       IntegerLiteralSyntax
//@[22:23)        Integer |2|
//@[23:24)       Asterisk |*|
//@[24:25)       IntegerLiteralSyntax
//@[24:25)        Integer |3|
//@[26:27)     Colon |:|
//@[28:29)     IntegerLiteralSyntax
//@[28:29)      Integer |0|
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
//@[31:33)    IntegerLiteralSyntax
//@[31:33)     Integer |18|
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
//@[52:54) NewLine |\n\n|

@secure()
//@[0:104) ParameterDeclarationSyntax
//@[0:9)  DecoratorSyntax
//@[0:1)   At |@|
//@[1:9)   FunctionCallSyntax
//@[1:7)    IdentifierSyntax
//@[1:7)     Identifier |secure|
//@[7:8)    LeftParen |(|
//@[8:9)    RightParen |)|
//@[9:10)  NewLine |\n|
@minLength(2)
//@[0:13)  DecoratorSyntax
//@[0:1)   At |@|
//@[1:13)   FunctionCallSyntax
//@[1:10)    IdentifierSyntax
//@[1:10)     Identifier |minLength|
//@[10:11)    LeftParen |(|
//@[11:12)    FunctionArgumentSyntax
//@[11:12)     IntegerLiteralSyntax
//@[11:12)      Integer |2|
//@[12:13)    RightParen |)|
//@[13:14)  NewLine |\n|
  @maxLength(10)
//@[2:16)  DecoratorSyntax
//@[2:3)   At |@|
//@[3:16)   FunctionCallSyntax
//@[3:12)    IdentifierSyntax
//@[3:12)     Identifier |maxLength|
//@[12:13)    LeftParen |(|
//@[13:15)    FunctionArgumentSyntax
//@[13:15)     IntegerLiteralSyntax
//@[13:15)      Integer |10|
//@[15:16)    RightParen |)|
//@[16:17)  NewLine |\n|
@allowed([
//@[0:34)  DecoratorSyntax
//@[0:1)   At |@|
//@[1:34)   FunctionCallSyntax
//@[1:8)    IdentifierSyntax
//@[1:8)     Identifier |allowed|
//@[8:9)    LeftParen |(|
//@[9:33)    FunctionArgumentSyntax
//@[9:33)     ArraySyntax
//@[9:10)      LeftSquare |[|
//@[10:11)      NewLine |\n|
  'Apple'
//@[2:9)      ArrayItemSyntax
//@[2:9)       StringSyntax
//@[2:9)        StringComplete |'Apple'|
//@[9:10)      NewLine |\n|
  'Banana'
//@[2:10)      ArrayItemSyntax
//@[2:10)       StringSyntax
//@[2:10)        StringComplete |'Banana'|
//@[10:11)      NewLine |\n|
])
//@[0:1)      RightSquare |]|
//@[1:2)    RightParen |)|
//@[2:3)  NewLine |\n|
param decoratedString string
//@[0:5)  Identifier |param|
//@[6:21)  IdentifierSyntax
//@[6:21)   Identifier |decoratedString|
//@[22:28)  TypeSyntax
//@[22:28)   Identifier |string|
//@[28:30) NewLine |\n\n|

@minValue(200)
//@[0:43) ParameterDeclarationSyntax
//@[0:14)  DecoratorSyntax
//@[0:1)   At |@|
//@[1:14)   FunctionCallSyntax
//@[1:9)    IdentifierSyntax
//@[1:9)     Identifier |minValue|
//@[9:10)    LeftParen |(|
//@[10:13)    FunctionArgumentSyntax
//@[10:13)     IntegerLiteralSyntax
//@[10:13)      Integer |200|
//@[13:14)    RightParen |)|
//@[14:15)  NewLine |\n|
param decoratedInt int = 123
//@[0:5)  Identifier |param|
//@[6:18)  IdentifierSyntax
//@[6:18)   Identifier |decoratedInt|
//@[19:22)  TypeSyntax
//@[19:22)   Identifier |int|
//@[23:28)  ParameterDefaultValueSyntax
//@[23:24)   Assignment |=|
//@[25:28)   IntegerLiteralSyntax
//@[25:28)    Integer |123|
//@[28:30) NewLine |\n\n|

// negative integer literals are allowed as decorator values
//@[60:61) NewLine |\n|
@minValue(-10)
//@[0:53) ParameterDeclarationSyntax
//@[0:14)  DecoratorSyntax
//@[0:1)   At |@|
//@[1:14)   FunctionCallSyntax
//@[1:9)    IdentifierSyntax
//@[1:9)     Identifier |minValue|
//@[9:10)    LeftParen |(|
//@[10:13)    FunctionArgumentSyntax
//@[10:13)     UnaryOperationSyntax
//@[10:11)      Minus |-|
//@[11:13)      IntegerLiteralSyntax
//@[11:13)       Integer |10|
//@[13:14)    RightParen |)|
//@[14:15)  NewLine |\n|
@maxValue(-3)
//@[0:13)  DecoratorSyntax
//@[0:1)   At |@|
//@[1:13)   FunctionCallSyntax
//@[1:9)    IdentifierSyntax
//@[1:9)     Identifier |maxValue|
//@[9:10)    LeftParen |(|
//@[10:12)    FunctionArgumentSyntax
//@[10:12)     UnaryOperationSyntax
//@[10:11)      Minus |-|
//@[11:12)      IntegerLiteralSyntax
//@[11:12)       Integer |3|
//@[12:13)    RightParen |)|
//@[13:14)  NewLine |\n|
param negativeValues int
//@[0:5)  Identifier |param|
//@[6:20)  IdentifierSyntax
//@[6:20)   Identifier |negativeValues|
//@[21:24)  TypeSyntax
//@[21:24)   Identifier |int|
//@[24:26) NewLine |\n\n|

// negative zeros are valid lengths
//@[35:36) NewLine |\n|
@minLength(-0)
//@[0:56) ParameterDeclarationSyntax
//@[0:14)  DecoratorSyntax
//@[0:1)   At |@|
//@[1:14)   FunctionCallSyntax
//@[1:10)    IdentifierSyntax
//@[1:10)     Identifier |minLength|
//@[10:11)    LeftParen |(|
//@[11:13)    FunctionArgumentSyntax
//@[11:13)     UnaryOperationSyntax
//@[11:12)      Minus |-|
//@[12:13)      IntegerLiteralSyntax
//@[12:13)       Integer |0|
//@[13:14)    RightParen |)|
//@[14:15)  NewLine |\n|
@maxLength(-0)
//@[0:14)  DecoratorSyntax
//@[0:1)   At |@|
//@[1:14)   FunctionCallSyntax
//@[1:10)    IdentifierSyntax
//@[1:10)     Identifier |maxLength|
//@[10:11)    LeftParen |(|
//@[11:13)    FunctionArgumentSyntax
//@[11:13)     UnaryOperationSyntax
//@[11:12)      Minus |-|
//@[12:13)      IntegerLiteralSyntax
//@[12:13)       Integer |0|
//@[13:14)    RightParen |)|
//@[14:15)  NewLine |\n|
param negativeZeros string
//@[0:5)  Identifier |param|
//@[6:19)  IdentifierSyntax
//@[6:19)   Identifier |negativeZeros|
//@[20:26)  TypeSyntax
//@[20:26)   Identifier |string|
//@[26:28) NewLine |\n\n|

// negative integer literals in modifiers
//@[41:42) NewLine |\n|
param negativeModifiers int {
//@[0:64) ParameterDeclarationSyntax
//@[0:5)  Identifier |param|
//@[6:23)  IdentifierSyntax
//@[6:23)   Identifier |negativeModifiers|
//@[24:27)  TypeSyntax
//@[24:27)   Identifier |int|
//@[28:64)  ObjectSyntax
//@[28:29)   LeftBrace |{|
//@[29:30)   NewLine |\n|
  minValue: -100
//@[2:16)   ObjectPropertySyntax
//@[2:10)    IdentifierSyntax
//@[2:10)     Identifier |minValue|
//@[10:11)    Colon |:|
//@[12:16)    UnaryOperationSyntax
//@[12:13)     Minus |-|
//@[13:16)     IntegerLiteralSyntax
//@[13:16)      Integer |100|
//@[16:17)   NewLine |\n|
  maxValue: -33
//@[2:15)   ObjectPropertySyntax
//@[2:10)    IdentifierSyntax
//@[2:10)     Identifier |maxValue|
//@[10:11)    Colon |:|
//@[12:15)    UnaryOperationSyntax
//@[12:13)     Minus |-|
//@[13:15)     IntegerLiteralSyntax
//@[13:15)      Integer |33|
//@[15:16)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

@sys.description('A boolean.')
//@[0:193) ParameterDeclarationSyntax
//@[0:30)  DecoratorSyntax
//@[0:1)   At |@|
//@[1:30)   InstanceFunctionCallSyntax
//@[1:4)    VariableAccessSyntax
//@[1:4)     IdentifierSyntax
//@[1:4)      Identifier |sys|
//@[4:5)    Dot |.|
//@[5:16)    IdentifierSyntax
//@[5:16)     Identifier |description|
//@[16:17)    LeftParen |(|
//@[17:29)    FunctionArgumentSyntax
//@[17:29)     StringSyntax
//@[17:29)      StringComplete |'A boolean.'|
//@[29:30)    RightParen |)|
//@[30:31)  NewLine |\n|
@metadata({
//@[0:137)  DecoratorSyntax
//@[0:1)   At |@|
//@[1:137)   FunctionCallSyntax
//@[1:9)    IdentifierSyntax
//@[1:9)     Identifier |metadata|
//@[9:10)    LeftParen |(|
//@[10:136)    FunctionArgumentSyntax
//@[10:136)     ObjectSyntax
//@[10:11)      LeftBrace |{|
//@[11:12)      NewLine |\n|
    description: 'I will be overrode.'
//@[4:38)      ObjectPropertySyntax
//@[4:15)       IdentifierSyntax
//@[4:15)        Identifier |description|
//@[15:16)       Colon |:|
//@[17:38)       StringSyntax
//@[17:38)        StringComplete |'I will be overrode.'|
//@[38:39)      NewLine |\n|
    foo: 'something'
//@[4:20)      ObjectPropertySyntax
//@[4:7)       IdentifierSyntax
//@[4:7)        Identifier |foo|
//@[7:8)       Colon |:|
//@[9:20)       StringSyntax
//@[9:20)        StringComplete |'something'|
//@[20:21)      NewLine |\n|
    bar: [
//@[4:62)      ObjectPropertySyntax
//@[4:7)       IdentifierSyntax
//@[4:7)        Identifier |bar|
//@[7:8)       Colon |:|
//@[9:62)       ArraySyntax
//@[9:10)        LeftSquare |[|
//@[10:11)        NewLine |\n|
        {          }
//@[8:20)        ArrayItemSyntax
//@[8:20)         ObjectSyntax
//@[8:9)          LeftBrace |{|
//@[19:20)          RightBrace |}|
//@[20:21)        NewLine |\n|
        true
//@[8:12)        ArrayItemSyntax
//@[8:12)         BooleanLiteralSyntax
//@[8:12)          TrueKeyword |true|
//@[12:13)        NewLine |\n|
        123
//@[8:11)        ArrayItemSyntax
//@[8:11)         IntegerLiteralSyntax
//@[8:11)          Integer |123|
//@[11:12)        NewLine |\n|
    ]
//@[4:5)        RightSquare |]|
//@[5:6)      NewLine |\n|
})
//@[0:1)      RightBrace |}|
//@[1:2)    RightParen |)|
//@[2:3)  NewLine |\n|
param decoratedBool bool
//@[0:5)  Identifier |param|
//@[6:19)  IdentifierSyntax
//@[6:19)   Identifier |decoratedBool|
//@[20:24)  TypeSyntax
//@[20:24)   Identifier |bool|
//@[24:26) NewLine |\n\n|

@secure()
//@[0:65) ParameterDeclarationSyntax
//@[0:9)  DecoratorSyntax
//@[0:1)   At |@|
//@[1:9)   FunctionCallSyntax
//@[1:7)    IdentifierSyntax
//@[1:7)     Identifier |secure|
//@[7:8)    LeftParen |(|
//@[8:9)    RightParen |)|
//@[9:10)  NewLine |\n|
param decoratedObject object = {
//@[0:5)  Identifier |param|
//@[6:21)  IdentifierSyntax
//@[6:21)   Identifier |decoratedObject|
//@[22:28)  TypeSyntax
//@[22:28)   Identifier |object|
//@[29:55)  ParameterDefaultValueSyntax
//@[29:30)   Assignment |=|
//@[31:55)   ObjectSyntax
//@[31:32)    LeftBrace |{|
//@[32:33)    NewLine |\n|
  location: 'westus'
//@[2:20)    ObjectPropertySyntax
//@[2:10)     IdentifierSyntax
//@[2:10)      Identifier |location|
//@[10:11)     Colon |:|
//@[12:20)     StringSyntax
//@[12:20)      StringComplete |'westus'|
//@[20:21)    NewLine |\n|
}
//@[0:1)    RightBrace |}|
//@[1:4) NewLine |\n\n\n|


@metadata({
//@[0:125) ParameterDeclarationSyntax
//@[0:43)  DecoratorSyntax
//@[0:1)   At |@|
//@[1:43)   FunctionCallSyntax
//@[1:9)    IdentifierSyntax
//@[1:9)     Identifier |metadata|
//@[9:10)    LeftParen |(|
//@[10:42)    FunctionArgumentSyntax
//@[10:42)     ObjectSyntax
//@[10:11)      LeftBrace |{|
//@[11:12)      NewLine |\n|
    description: 'An array.'
//@[4:28)      ObjectPropertySyntax
//@[4:15)       IdentifierSyntax
//@[4:15)        Identifier |description|
//@[15:16)       Colon |:|
//@[17:28)       StringSyntax
//@[17:28)        StringComplete |'An array.'|
//@[28:29)      NewLine |\n|
})
//@[0:1)      RightBrace |}|
//@[1:2)    RightParen |)|
//@[2:3)  NewLine |\n|
@maxLength(20)
//@[0:14)  DecoratorSyntax
//@[0:1)   At |@|
//@[1:14)   FunctionCallSyntax
//@[1:10)    IdentifierSyntax
//@[1:10)     Identifier |maxLength|
//@[10:11)    LeftParen |(|
//@[11:13)    FunctionArgumentSyntax
//@[11:13)     IntegerLiteralSyntax
//@[11:13)      Integer |20|
//@[13:14)    RightParen |)|
//@[14:15)  NewLine |\n|
@sys.description('I will be overrode.')
//@[0:39)  DecoratorSyntax
//@[0:1)   At |@|
//@[1:39)   InstanceFunctionCallSyntax
//@[1:4)    VariableAccessSyntax
//@[1:4)     IdentifierSyntax
//@[1:4)      Identifier |sys|
//@[4:5)    Dot |.|
//@[5:16)    IdentifierSyntax
//@[5:16)     Identifier |description|
//@[16:17)    LeftParen |(|
//@[17:38)    FunctionArgumentSyntax
//@[17:38)     StringSyntax
//@[17:38)      StringComplete |'I will be overrode.'|
//@[38:39)    RightParen |)|
//@[39:40)  NewLine |\n|
param decoratedArray array
//@[0:5)  Identifier |param|
//@[6:20)  IdentifierSyntax
//@[6:20)   Identifier |decoratedArray|
//@[21:26)  TypeSyntax
//@[21:26)   Identifier |array|
//@[26:28) NewLine |\n\n|

param allowedPermutation array {
//@[0:454) ParameterDeclarationSyntax
//@[0:5)  Identifier |param|
//@[6:24)  IdentifierSyntax
//@[6:24)   Identifier |allowedPermutation|
//@[25:30)  TypeSyntax
//@[25:30)   Identifier |array|
//@[31:454)  ObjectSyntax
//@[31:32)   LeftBrace |{|
//@[32:33)   NewLine |\n|
    default: [
//@[4:99)   ObjectPropertySyntax
//@[4:11)    IdentifierSyntax
//@[4:11)     Identifier |default|
//@[11:12)    Colon |:|
//@[13:99)    ArraySyntax
//@[13:14)     LeftSquare |[|
//@[14:15)     NewLine |\n|
		'Microsoft.AnalysisServices/servers'
//@[2:38)     ArrayItemSyntax
//@[2:38)      StringSyntax
//@[2:38)       StringComplete |'Microsoft.AnalysisServices/servers'|
//@[38:39)     NewLine |\n|
		'Microsoft.ContainerRegistry/registries'
//@[2:42)     ArrayItemSyntax
//@[2:42)      StringSyntax
//@[2:42)       StringComplete |'Microsoft.ContainerRegistry/registries'|
//@[42:43)     NewLine |\n|
	]
//@[1:2)     RightSquare |]|
//@[2:3)   NewLine |\n|
    allowed: [
//@[4:319)   ObjectPropertySyntax
//@[4:11)    IdentifierSyntax
//@[4:11)     Identifier |allowed|
//@[11:12)    Colon |:|
//@[13:319)    ArraySyntax
//@[13:14)     LeftSquare |[|
//@[14:15)     NewLine |\n|
		'Microsoft.AnalysisServices/servers'
//@[2:38)     ArrayItemSyntax
//@[2:38)      StringSyntax
//@[2:38)       StringComplete |'Microsoft.AnalysisServices/servers'|
//@[38:39)     NewLine |\n|
		'Microsoft.ApiManagement/service'
//@[2:35)     ArrayItemSyntax
//@[2:35)      StringSyntax
//@[2:35)       StringComplete |'Microsoft.ApiManagement/service'|
//@[35:36)     NewLine |\n|
		'Microsoft.Network/applicationGateways'
//@[2:41)     ArrayItemSyntax
//@[2:41)      StringSyntax
//@[2:41)       StringComplete |'Microsoft.Network/applicationGateways'|
//@[41:42)     NewLine |\n|
		'Microsoft.Automation/automationAccounts'
//@[2:43)     ArrayItemSyntax
//@[2:43)      StringSyntax
//@[2:43)       StringComplete |'Microsoft.Automation/automationAccounts'|
//@[43:44)     NewLine |\n|
		'Microsoft.ContainerInstance/containerGroups'
//@[2:47)     ArrayItemSyntax
//@[2:47)      StringSyntax
//@[2:47)       StringComplete |'Microsoft.ContainerInstance/containerGroups'|
//@[47:48)     NewLine |\n|
		'Microsoft.ContainerRegistry/registries'
//@[2:42)     ArrayItemSyntax
//@[2:42)      StringSyntax
//@[2:42)       StringComplete |'Microsoft.ContainerRegistry/registries'|
//@[42:43)     NewLine |\n|
		'Microsoft.ContainerService/managedClusters'
//@[2:46)     ArrayItemSyntax
//@[2:46)      StringSyntax
//@[2:46)       StringComplete |'Microsoft.ContainerService/managedClusters'|
//@[46:47)     NewLine |\n|
    ]
//@[4:5)     RightSquare |]|
//@[5:6)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

@allowed([
//@[0:435) ParameterDeclarationSyntax
//@[0:305)  DecoratorSyntax
//@[0:1)   At |@|
//@[1:305)   FunctionCallSyntax
//@[1:8)    IdentifierSyntax
//@[1:8)     Identifier |allowed|
//@[8:9)    LeftParen |(|
//@[9:304)    FunctionArgumentSyntax
//@[9:304)     ArraySyntax
//@[9:10)      LeftSquare |[|
//@[10:11)      NewLine |\n|
	'Microsoft.AnalysisServices/servers'
//@[1:37)      ArrayItemSyntax
//@[1:37)       StringSyntax
//@[1:37)        StringComplete |'Microsoft.AnalysisServices/servers'|
//@[37:38)      NewLine |\n|
	'Microsoft.ApiManagement/service'
//@[1:34)      ArrayItemSyntax
//@[1:34)       StringSyntax
//@[1:34)        StringComplete |'Microsoft.ApiManagement/service'|
//@[34:35)      NewLine |\n|
	'Microsoft.Network/applicationGateways'
//@[1:40)      ArrayItemSyntax
//@[1:40)       StringSyntax
//@[1:40)        StringComplete |'Microsoft.Network/applicationGateways'|
//@[40:41)      NewLine |\n|
	'Microsoft.Automation/automationAccounts'
//@[1:42)      ArrayItemSyntax
//@[1:42)       StringSyntax
//@[1:42)        StringComplete |'Microsoft.Automation/automationAccounts'|
//@[42:43)      NewLine |\n|
	'Microsoft.ContainerInstance/containerGroups'
//@[1:46)      ArrayItemSyntax
//@[1:46)       StringSyntax
//@[1:46)        StringComplete |'Microsoft.ContainerInstance/containerGroups'|
//@[46:47)      NewLine |\n|
	'Microsoft.ContainerRegistry/registries'
//@[1:41)      ArrayItemSyntax
//@[1:41)       StringSyntax
//@[1:41)        StringComplete |'Microsoft.ContainerRegistry/registries'|
//@[41:42)      NewLine |\n|
	'Microsoft.ContainerService/managedClusters'
//@[1:45)      ArrayItemSyntax
//@[1:45)       StringSyntax
//@[1:45)        StringComplete |'Microsoft.ContainerService/managedClusters'|
//@[45:46)      NewLine |\n|
])
//@[0:1)      RightSquare |]|
//@[1:2)    RightParen |)|
//@[2:3)  NewLine |\n|
param allowedPermutationWithDecorator array = [
//@[0:5)  Identifier |param|
//@[6:37)  IdentifierSyntax
//@[6:37)   Identifier |allowedPermutationWithDecorator|
//@[38:43)  TypeSyntax
//@[38:43)   Identifier |array|
//@[44:129)  ParameterDefaultValueSyntax
//@[44:45)   Assignment |=|
//@[46:129)   ArraySyntax
//@[46:47)    LeftSquare |[|
//@[47:48)    NewLine |\n|
	'Microsoft.AnalysisServices/servers'
//@[1:37)    ArrayItemSyntax
//@[1:37)     StringSyntax
//@[1:37)      StringComplete |'Microsoft.AnalysisServices/servers'|
//@[37:38)    NewLine |\n|
	'Microsoft.ContainerRegistry/registries'
//@[1:41)    ArrayItemSyntax
//@[1:41)     StringSyntax
//@[1:41)      StringComplete |'Microsoft.ContainerRegistry/registries'|
//@[41:42)    NewLine |\n|
]
//@[0:1)    RightSquare |]|
//@[1:3) NewLine |\n\n|

param allowedArray array {
//@[0:323) ParameterDeclarationSyntax
//@[0:5)  Identifier |param|
//@[6:18)  IdentifierSyntax
//@[6:18)   Identifier |allowedArray|
//@[19:24)  TypeSyntax
//@[19:24)   Identifier |array|
//@[25:323)  ObjectSyntax
//@[25:26)   LeftBrace |{|
//@[26:27)   NewLine |\n|
    default: [
//@[4:92)   ObjectPropertySyntax
//@[4:11)    IdentifierSyntax
//@[4:11)     Identifier |default|
//@[11:12)    Colon |:|
//@[13:92)    ArraySyntax
//@[13:14)     LeftSquare |[|
//@[14:15)     NewLine |\n|
		'Microsoft.AnalysisServices/servers'
//@[2:38)     ArrayItemSyntax
//@[2:38)      StringSyntax
//@[2:38)       StringComplete |'Microsoft.AnalysisServices/servers'|
//@[38:39)     NewLine |\n|
		'Microsoft.ApiManagement/service'
//@[2:35)     ArrayItemSyntax
//@[2:35)      StringSyntax
//@[2:35)       StringComplete |'Microsoft.ApiManagement/service'|
//@[35:36)     NewLine |\n|
	]
//@[1:2)     RightSquare |]|
//@[2:3)   NewLine |\n|
    allowed: [
//@[4:201)   ObjectPropertySyntax
//@[4:11)    IdentifierSyntax
//@[4:11)     Identifier |allowed|
//@[11:12)    Colon |:|
//@[13:201)    ArraySyntax
//@[13:14)     LeftSquare |[|
//@[14:15)     NewLine |\n|
		[
//@[2:84)     ArrayItemSyntax
//@[2:84)      ArraySyntax
//@[2:3)       LeftSquare |[|
//@[3:4)       NewLine |\n|
			'Microsoft.AnalysisServices/servers'
//@[3:39)       ArrayItemSyntax
//@[3:39)        StringSyntax
//@[3:39)         StringComplete |'Microsoft.AnalysisServices/servers'|
//@[39:40)       NewLine |\n|
			'Microsoft.ApiManagement/service'
//@[3:36)       ArrayItemSyntax
//@[3:36)        StringSyntax
//@[3:36)         StringComplete |'Microsoft.ApiManagement/service'|
//@[36:37)       NewLine |\n|
		]
//@[2:3)       RightSquare |]|
//@[3:4)     NewLine |\n|
		[
//@[2:95)     ArrayItemSyntax
//@[2:95)      ArraySyntax
//@[2:3)       LeftSquare |[|
//@[3:4)       NewLine |\n|
			'Microsoft.Network/applicationGateways'
//@[3:42)       ArrayItemSyntax
//@[3:42)        StringSyntax
//@[3:42)         StringComplete |'Microsoft.Network/applicationGateways'|
//@[42:43)       NewLine |\n|
			'Microsoft.Automation/automationAccounts'
//@[3:44)       ArrayItemSyntax
//@[3:44)        StringSyntax
//@[3:44)         StringComplete |'Microsoft.Automation/automationAccounts'|
//@[44:45)       NewLine |\n|
		]
//@[2:3)       RightSquare |]|
//@[3:4)     NewLine |\n|
    ]
//@[4:5)     RightSquare |]|
//@[5:6)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

@allowed([
//@[0:303) ParameterDeclarationSyntax
//@[0:186)  DecoratorSyntax
//@[0:1)   At |@|
//@[1:186)   FunctionCallSyntax
//@[1:8)    IdentifierSyntax
//@[1:8)     Identifier |allowed|
//@[8:9)    LeftParen |(|
//@[9:185)    FunctionArgumentSyntax
//@[9:185)     ArraySyntax
//@[9:10)      LeftSquare |[|
//@[10:11)      NewLine |\n|
	[
//@[1:80)      ArrayItemSyntax
//@[1:80)       ArraySyntax
//@[1:2)        LeftSquare |[|
//@[2:3)        NewLine |\n|
		'Microsoft.AnalysisServices/servers'
//@[2:38)        ArrayItemSyntax
//@[2:38)         StringSyntax
//@[2:38)          StringComplete |'Microsoft.AnalysisServices/servers'|
//@[38:39)        NewLine |\n|
		'Microsoft.ApiManagement/service'
//@[2:35)        ArrayItemSyntax
//@[2:35)         StringSyntax
//@[2:35)          StringComplete |'Microsoft.ApiManagement/service'|
//@[35:36)        NewLine |\n|
	]
//@[1:2)        RightSquare |]|
//@[2:3)      NewLine |\n|
	[
//@[1:91)      ArrayItemSyntax
//@[1:91)       ArraySyntax
//@[1:2)        LeftSquare |[|
//@[2:3)        NewLine |\n|
		'Microsoft.Network/applicationGateways'
//@[2:41)        ArrayItemSyntax
//@[2:41)         StringSyntax
//@[2:41)          StringComplete |'Microsoft.Network/applicationGateways'|
//@[41:42)        NewLine |\n|
		'Microsoft.Automation/automationAccounts'
//@[2:43)        ArrayItemSyntax
//@[2:43)         StringSyntax
//@[2:43)          StringComplete |'Microsoft.Automation/automationAccounts'|
//@[43:44)        NewLine |\n|
	]
//@[1:2)        RightSquare |]|
//@[2:3)      NewLine |\n|
])
//@[0:1)      RightSquare |]|
//@[1:2)    RightParen |)|
//@[2:3)  NewLine |\n|
param allowedArrayWithDecorator array = [
//@[0:5)  Identifier |param|
//@[6:31)  IdentifierSyntax
//@[6:31)   Identifier |allowedArrayWithDecorator|
//@[32:37)  TypeSyntax
//@[32:37)   Identifier |array|
//@[38:116)  ParameterDefaultValueSyntax
//@[38:39)   Assignment |=|
//@[40:116)   ArraySyntax
//@[40:41)    LeftSquare |[|
//@[41:42)    NewLine |\n|
	'Microsoft.AnalysisServices/servers'
//@[1:37)    ArrayItemSyntax
//@[1:37)     StringSyntax
//@[1:37)      StringComplete |'Microsoft.AnalysisServices/servers'|
//@[37:38)    NewLine |\n|
	'Microsoft.ApiManagement/service'
//@[1:34)    ArrayItemSyntax
//@[1:34)     StringSyntax
//@[1:34)      StringComplete |'Microsoft.ApiManagement/service'|
//@[34:35)    NewLine |\n|
]
//@[0:1)    RightSquare |]|
//@[1:2) NewLine |\n|

//@[0:0) EndOfFile ||
