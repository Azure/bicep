/* 
  This is a block comment.
*/
//@[2:4) NewLine |\n\n|

// parameters without default value
//@[35:36) NewLine |\n|
@sys.description('''
//@[0:98) ParameterDeclarationSyntax
//@[0:76)  DecoratorSyntax
//@[0:1)   At |@|
//@[1:76)   InstanceFunctionCallSyntax
//@[1:4)    VariableAccessSyntax
//@[1:4)     IdentifierSyntax
//@[1:4)      Identifier |sys|
//@[4:5)    Dot |.|
//@[5:16)    IdentifierSyntax
//@[5:16)     Identifier |description|
//@[16:17)    LeftParen |(|
//@[17:75)    FunctionArgumentSyntax
//@[17:75)     StringSyntax
//@[17:75)      MultilineString |'''\nthis is my multi line \ndescription for my myString\n'''|
this is my multi line 
description for my myString
''')
//@[3:4)    RightParen |)|
//@[4:5)  NewLine |\n|
param myString string
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
@sys.description('this is myString2')
//@[0:135) ParameterDeclarationSyntax
//@[0:37)  DecoratorSyntax
//@[0:1)   At |@|
//@[1:37)   InstanceFunctionCallSyntax
//@[1:4)    VariableAccessSyntax
//@[1:4)     IdentifierSyntax
//@[1:4)      Identifier |sys|
//@[4:5)    Dot |.|
//@[5:16)    IdentifierSyntax
//@[5:16)     Identifier |description|
//@[16:17)    LeftParen |(|
//@[17:36)    FunctionArgumentSyntax
//@[17:36)     StringSyntax
//@[17:36)      StringComplete |'this is myString2'|
//@[36:37)    RightParen |)|
//@[37:38)  NewLine |\n|
@metadata({
//@[0:57)  DecoratorSyntax
//@[0:1)   At |@|
//@[1:57)   FunctionCallSyntax
//@[1:9)    IdentifierSyntax
//@[1:9)     Identifier |metadata|
//@[9:10)    LeftParen |(|
//@[10:56)    FunctionArgumentSyntax
//@[10:56)     ObjectSyntax
//@[10:11)      LeftBrace |{|
//@[11:12)      NewLine |\n|
  description: 'overwrite but still valid'
//@[2:42)      ObjectPropertySyntax
//@[2:13)       IdentifierSyntax
//@[2:13)        Identifier |description|
//@[13:14)       Colon |:|
//@[15:42)       StringSyntax
//@[15:42)        StringComplete |'overwrite but still valid'|
//@[42:43)      NewLine |\n|
})
//@[0:1)      RightBrace |}|
//@[1:2)    RightParen |)|
//@[2:3)  NewLine |\n|
param myString2 string = 'string value'
//@[0:5)  Identifier |param|
//@[6:15)  IdentifierSyntax
//@[6:15)   Identifier |myString2|
//@[16:22)  TypeSyntax
//@[16:22)   Identifier |string|
//@[23:39)  ParameterDefaultValueSyntax
//@[23:24)   Assignment |=|
//@[25:39)   StringSyntax
//@[25:39)    StringComplete |'string value'|
//@[39:40) NewLine |\n|
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
param myEscapedString string = 'First line\r\nSecond\ttabbed\tline'
//@[0:67) ParameterDeclarationSyntax
//@[0:5)  Identifier |param|
//@[6:21)  IdentifierSyntax
//@[6:21)   Identifier |myEscapedString|
//@[22:28)  TypeSyntax
//@[22:28)   Identifier |string|
//@[29:67)  ParameterDefaultValueSyntax
//@[29:30)   Assignment |=|
//@[31:67)   StringSyntax
//@[31:67)    StringComplete |'First line\r\nSecond\ttabbed\tline'|
//@[67:69) NewLine |\n\n|

// object default value
//@[23:24) NewLine |\n|
@sys.description('this is foo')
//@[0:348) ParameterDeclarationSyntax
//@[0:31)  DecoratorSyntax
//@[0:1)   At |@|
//@[1:31)   InstanceFunctionCallSyntax
//@[1:4)    VariableAccessSyntax
//@[1:4)     IdentifierSyntax
//@[1:4)      Identifier |sys|
//@[4:5)    Dot |.|
//@[5:16)    IdentifierSyntax
//@[5:16)     Identifier |description|
//@[16:17)    LeftParen |(|
//@[17:30)    FunctionArgumentSyntax
//@[17:30)     StringSyntax
//@[17:30)      StringComplete |'this is foo'|
//@[30:31)    RightParen |)|
//@[31:32)  NewLine |\n|
@metadata({
//@[0:83)  DecoratorSyntax
//@[0:1)   At |@|
//@[1:83)   FunctionCallSyntax
//@[1:9)    IdentifierSyntax
//@[1:9)     Identifier |metadata|
//@[9:10)    LeftParen |(|
//@[10:82)    FunctionArgumentSyntax
//@[10:82)     ObjectSyntax
//@[10:11)      LeftBrace |{|
//@[11:12)      NewLine |\n|
  description: 'overwrite but still valid'
//@[2:42)      ObjectPropertySyntax
//@[2:13)       IdentifierSyntax
//@[2:13)        Identifier |description|
//@[13:14)       Colon |:|
//@[15:42)       StringSyntax
//@[15:42)        StringComplete |'overwrite but still valid'|
//@[42:43)      NewLine |\n|
  another: 'just for fun'
//@[2:25)      ObjectPropertySyntax
//@[2:9)       IdentifierSyntax
//@[2:9)        Identifier |another|
//@[9:10)       Colon |:|
//@[11:25)       StringSyntax
//@[11:25)        StringComplete |'just for fun'|
//@[25:26)      NewLine |\n|
})
//@[0:1)      RightBrace |}|
//@[1:2)    RightParen |)|
//@[2:3)  NewLine |\n|
param foo object = {
//@[0:5)  Identifier |param|
//@[6:9)  IdentifierSyntax
//@[6:9)   Identifier |foo|
//@[10:16)  TypeSyntax
//@[10:16)   Identifier |object|
//@[17:232)  ParameterDefaultValueSyntax
//@[17:18)   Assignment |=|
//@[19:232)   ObjectSyntax
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

// secure string
//@[16:17) NewLine |\n|
@secure()
//@[0:31) ParameterDeclarationSyntax
//@[0:9)  DecoratorSyntax
//@[0:1)   At |@|
//@[1:9)   FunctionCallSyntax
//@[1:7)    IdentifierSyntax
//@[1:7)     Identifier |secure|
//@[7:8)    LeftParen |(|
//@[8:9)    RightParen |)|
//@[9:10)  NewLine |\n|
param password string
//@[0:5)  Identifier |param|
//@[6:14)  IdentifierSyntax
//@[6:14)   Identifier |password|
//@[15:21)  TypeSyntax
//@[15:21)   Identifier |string|
//@[21:23) NewLine |\n\n|

// secure object
//@[16:17) NewLine |\n|
@secure()
//@[0:35) ParameterDeclarationSyntax
//@[0:9)  DecoratorSyntax
//@[0:1)   At |@|
//@[1:9)   FunctionCallSyntax
//@[1:7)    IdentifierSyntax
//@[1:7)     Identifier |secure|
//@[7:8)    LeftParen |(|
//@[8:9)    RightParen |)|
//@[9:10)  NewLine |\n|
param secretObject object
//@[0:5)  Identifier |param|
//@[6:18)  IdentifierSyntax
//@[6:18)   Identifier |secretObject|
//@[19:25)  TypeSyntax
//@[19:25)   Identifier |object|
//@[25:27) NewLine |\n\n|

// enum parameter
//@[17:18) NewLine |\n|
@allowed([
//@[0:71) ParameterDeclarationSyntax
//@[0:47)  DecoratorSyntax
//@[0:1)   At |@|
//@[1:47)   FunctionCallSyntax
//@[1:8)    IdentifierSyntax
//@[1:8)     Identifier |allowed|
//@[8:9)    LeftParen |(|
//@[9:46)    FunctionArgumentSyntax
//@[9:46)     ArraySyntax
//@[9:10)      LeftSquare |[|
//@[10:11)      NewLine |\n|
  'Standard_LRS'
//@[2:16)      ArrayItemSyntax
//@[2:16)       StringSyntax
//@[2:16)        StringComplete |'Standard_LRS'|
//@[16:17)      NewLine |\n|
  'Standard_GRS'
//@[2:16)      ArrayItemSyntax
//@[2:16)       StringSyntax
//@[2:16)        StringComplete |'Standard_GRS'|
//@[16:17)      NewLine |\n|
])
//@[0:1)      RightSquare |]|
//@[1:2)    RightParen |)|
//@[2:3)  NewLine |\n|
param storageSku string
//@[0:5)  Identifier |param|
//@[6:16)  IdentifierSyntax
//@[6:16)   Identifier |storageSku|
//@[17:23)  TypeSyntax
//@[17:23)   Identifier |string|
//@[23:25) NewLine |\n\n|

// length constraint on a string
//@[32:33) NewLine |\n|
@minLength(3)
//@[0:53) ParameterDeclarationSyntax
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
param storageName string
//@[0:5)  Identifier |param|
//@[6:17)  IdentifierSyntax
//@[6:17)   Identifier |storageName|
//@[18:24)  TypeSyntax
//@[18:24)   Identifier |string|
//@[24:26) NewLine |\n\n|

// length constraint on an array
//@[32:33) NewLine |\n|
@minLength(3)
//@[0:50) ParameterDeclarationSyntax
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
param someArray array
//@[0:5)  Identifier |param|
//@[6:15)  IdentifierSyntax
//@[6:15)   Identifier |someArray|
//@[16:21)  TypeSyntax
//@[16:21)   Identifier |array|
//@[21:23) NewLine |\n\n|

// empty metadata
//@[17:18) NewLine |\n|
@metadata({})
//@[0:40) ParameterDeclarationSyntax
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
param emptyMetadata string
//@[0:5)  Identifier |param|
//@[6:19)  IdentifierSyntax
//@[6:19)   Identifier |emptyMetadata|
//@[20:26)  TypeSyntax
//@[20:26)   Identifier |string|
//@[26:28) NewLine |\n\n|

// description
//@[14:15) NewLine |\n|
@metadata({
//@[0:71) ParameterDeclarationSyntax
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
param description string
//@[0:5)  Identifier |param|
//@[6:17)  IdentifierSyntax
//@[6:17)   Identifier |description|
//@[18:24)  TypeSyntax
//@[18:24)   Identifier |string|
//@[24:26) NewLine |\n\n|

@sys.description('my description')
//@[0:60) ParameterDeclarationSyntax
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
param description2 string
//@[0:5)  Identifier |param|
//@[6:18)  IdentifierSyntax
//@[6:18)   Identifier |description2|
//@[19:25)  TypeSyntax
//@[19:25)   Identifier |string|
//@[25:27) NewLine |\n\n|

// random extra metadata
//@[24:25) NewLine |\n|
@metadata({
//@[0:133) ParameterDeclarationSyntax
//@[0:101)  DecoratorSyntax
//@[0:1)   At |@|
//@[1:101)   FunctionCallSyntax
//@[1:9)    IdentifierSyntax
//@[1:9)     Identifier |metadata|
//@[9:10)    LeftParen |(|
//@[10:100)    FunctionArgumentSyntax
//@[10:100)     ObjectSyntax
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
  a: 1
//@[2:6)      ObjectPropertySyntax
//@[2:3)       IdentifierSyntax
//@[2:3)        Identifier |a|
//@[3:4)       Colon |:|
//@[5:6)       IntegerLiteralSyntax
//@[5:6)        Integer |1|
//@[6:7)      NewLine |\n|
  b: true
//@[2:9)      ObjectPropertySyntax
//@[2:3)       IdentifierSyntax
//@[2:3)        Identifier |b|
//@[3:4)       Colon |:|
//@[5:9)       BooleanLiteralSyntax
//@[5:9)        TrueKeyword |true|
//@[9:10)      NewLine |\n|
  c: [
//@[2:10)      ObjectPropertySyntax
//@[2:3)       IdentifierSyntax
//@[2:3)        Identifier |c|
//@[3:4)       Colon |:|
//@[5:10)       ArraySyntax
//@[5:6)        LeftSquare |[|
//@[6:7)        NewLine |\n|
  ]
//@[2:3)        RightSquare |]|
//@[3:4)      NewLine |\n|
  d: {
//@[2:26)      ObjectPropertySyntax
//@[2:3)       IdentifierSyntax
//@[2:3)        Identifier |d|
//@[3:4)       Colon |:|
//@[5:26)       ObjectSyntax
//@[5:6)        LeftBrace |{|
//@[6:7)        NewLine |\n|
    test: 'abc'
//@[4:15)        ObjectPropertySyntax
//@[4:8)         IdentifierSyntax
//@[4:8)          Identifier |test|
//@[8:9)         Colon |:|
//@[10:15)         StringSyntax
//@[10:15)          StringComplete |'abc'|
//@[15:16)        NewLine |\n|
  }
//@[2:3)        RightBrace |}|
//@[3:4)      NewLine |\n|
})
//@[0:1)      RightBrace |}|
//@[1:2)    RightParen |)|
//@[2:3)  NewLine |\n|
param additionalMetadata string
//@[0:5)  Identifier |param|
//@[6:24)  IdentifierSyntax
//@[6:24)   Identifier |additionalMetadata|
//@[25:31)  TypeSyntax
//@[25:31)   Identifier |string|
//@[31:33) NewLine |\n\n|

// all modifiers together
//@[25:26) NewLine |\n|
@secure()
//@[0:165) ParameterDeclarationSyntax
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
param someParameter string
//@[0:5)  Identifier |param|
//@[6:19)  IdentifierSyntax
//@[6:19)   Identifier |someParameter|
//@[20:26)  TypeSyntax
//@[20:26)   Identifier |string|
//@[26:28) NewLine |\n\n|

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

@allowed([
//@[0:56) ParameterDeclarationSyntax
//@[0:29)  DecoratorSyntax
//@[0:1)   At |@|
//@[1:29)   FunctionCallSyntax
//@[1:8)    IdentifierSyntax
//@[1:8)     Identifier |allowed|
//@[8:9)    LeftParen |(|
//@[9:28)    FunctionArgumentSyntax
//@[9:28)     ArraySyntax
//@[9:10)      LeftSquare |[|
//@[10:11)      NewLine |\n|
  'abc'
//@[2:7)      ArrayItemSyntax
//@[2:7)       StringSyntax
//@[2:7)        StringComplete |'abc'|
//@[7:8)      NewLine |\n|
  'def'
//@[2:7)      ArrayItemSyntax
//@[2:7)       StringSyntax
//@[2:7)        StringComplete |'def'|
//@[7:8)      NewLine |\n|
])
//@[0:1)      RightSquare |]|
//@[1:2)    RightParen |)|
//@[2:3)  NewLine |\n|
param stringLiteral string
//@[0:5)  Identifier |param|
//@[6:19)  IdentifierSyntax
//@[6:19)   Identifier |stringLiteral|
//@[20:26)  TypeSyntax
//@[20:26)   Identifier |string|
//@[26:28) NewLine |\n\n|

@allowed([
//@[0:105) ParameterDeclarationSyntax
//@[0:37)  DecoratorSyntax
//@[0:1)   At |@|
//@[1:37)   FunctionCallSyntax
//@[1:8)    IdentifierSyntax
//@[1:8)     Identifier |allowed|
//@[8:9)    LeftParen |(|
//@[9:36)    FunctionArgumentSyntax
//@[9:36)     ArraySyntax
//@[9:10)      LeftSquare |[|
//@[10:11)      NewLine |\n|
  'abc'
//@[2:7)      ArrayItemSyntax
//@[2:7)       StringSyntax
//@[2:7)        StringComplete |'abc'|
//@[7:8)      NewLine |\n|
  'def'
//@[2:7)      ArrayItemSyntax
//@[2:7)       StringSyntax
//@[2:7)        StringComplete |'def'|
//@[7:8)      NewLine |\n|
  'ghi'
//@[2:7)      ArrayItemSyntax
//@[2:7)       StringSyntax
//@[2:7)        StringComplete |'ghi'|
//@[7:8)      NewLine |\n|
])
//@[0:1)      RightSquare |]|
//@[1:2)    RightParen |)|
//@[2:3)  NewLine |\n|
param stringLiteralWithAllowedValuesSuperset string = stringLiteral
//@[0:5)  Identifier |param|
//@[6:44)  IdentifierSyntax
//@[6:44)   Identifier |stringLiteralWithAllowedValuesSuperset|
//@[45:51)  TypeSyntax
//@[45:51)   Identifier |string|
//@[52:67)  ParameterDefaultValueSyntax
//@[52:53)   Assignment |=|
//@[54:67)   VariableAccessSyntax
//@[54:67)    IdentifierSyntax
//@[54:67)     Identifier |stringLiteral|
//@[67:69) NewLine |\n\n|

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

@sys.description('A boolean.')
//@[0:219) ParameterDeclarationSyntax
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
param decoratedBool bool = (true && false) != true
//@[0:5)  Identifier |param|
//@[6:19)  IdentifierSyntax
//@[6:19)   Identifier |decoratedBool|
//@[20:24)  TypeSyntax
//@[20:24)   Identifier |bool|
//@[25:50)  ParameterDefaultValueSyntax
//@[25:26)   Assignment |=|
//@[27:50)   BinaryOperationSyntax
//@[27:42)    ParenthesizedExpressionSyntax
//@[27:28)     LeftParen |(|
//@[28:41)     BinaryOperationSyntax
//@[28:32)      BooleanLiteralSyntax
//@[28:32)       TrueKeyword |true|
//@[33:35)      LogicalAnd |&&|
//@[36:41)      BooleanLiteralSyntax
//@[36:41)       FalseKeyword |false|
//@[41:42)     RightParen |)|
//@[43:45)    NotEquals |!=|
//@[46:50)    BooleanLiteralSyntax
//@[46:50)     TrueKeyword |true|
//@[50:52) NewLine |\n\n|

@secure()
//@[0:254) ParameterDeclarationSyntax
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
//@[29:244)  ParameterDefaultValueSyntax
//@[29:30)   Assignment |=|
//@[31:244)   ObjectSyntax
//@[31:32)    LeftBrace |{|
//@[32:33)    NewLine |\n|
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
}
//@[0:1)    RightBrace |}|
//@[1:3) NewLine |\n\n|

@sys.metadata({
//@[0:166) ParameterDeclarationSyntax
//@[0:47)  DecoratorSyntax
//@[0:1)   At |@|
//@[1:47)   InstanceFunctionCallSyntax
//@[1:4)    VariableAccessSyntax
//@[1:4)     IdentifierSyntax
//@[1:4)      Identifier |sys|
//@[4:5)    Dot |.|
//@[5:13)    IdentifierSyntax
//@[5:13)     Identifier |metadata|
//@[13:14)    LeftParen |(|
//@[14:46)    FunctionArgumentSyntax
//@[14:46)     ObjectSyntax
//@[14:15)      LeftBrace |{|
//@[15:16)      NewLine |\n|
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
@sys.maxLength(20)
//@[0:18)  DecoratorSyntax
//@[0:1)   At |@|
//@[1:18)   InstanceFunctionCallSyntax
//@[1:4)    VariableAccessSyntax
//@[1:4)     IdentifierSyntax
//@[1:4)      Identifier |sys|
//@[4:5)    Dot |.|
//@[5:14)    IdentifierSyntax
//@[5:14)     Identifier |maxLength|
//@[14:15)    LeftParen |(|
//@[15:17)    FunctionArgumentSyntax
//@[15:17)     IntegerLiteralSyntax
//@[15:17)      Integer |20|
//@[17:18)    RightParen |)|
//@[18:19)  NewLine |\n|
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
param decoratedArray array = [
//@[0:5)  Identifier |param|
//@[6:20)  IdentifierSyntax
//@[6:20)   Identifier |decoratedArray|
//@[21:26)  TypeSyntax
//@[21:26)   Identifier |array|
//@[27:59)  ParameterDefaultValueSyntax
//@[27:28)   Assignment |=|
//@[29:59)   ArraySyntax
//@[29:30)    LeftSquare |[|
//@[30:31)    NewLine |\n|
    utcNow()
//@[4:12)    ArrayItemSyntax
//@[4:12)     FunctionCallSyntax
//@[4:10)      IdentifierSyntax
//@[4:10)       Identifier |utcNow|
//@[10:11)      LeftParen |(|
//@[11:12)      RightParen |)|
//@[12:13)    NewLine |\n|
    newGuid()
//@[4:13)    ArrayItemSyntax
//@[4:13)     FunctionCallSyntax
//@[4:11)      IdentifierSyntax
//@[4:11)       Identifier |newGuid|
//@[11:12)      LeftParen |(|
//@[12:13)      RightParen |)|
//@[13:14)    NewLine |\n|
]
//@[0:1)    RightSquare |]|
//@[1:2) NewLine |\n|

//@[0:0) EndOfFile ||
