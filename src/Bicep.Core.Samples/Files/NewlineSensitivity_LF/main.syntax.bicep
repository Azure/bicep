var singleLineFunction = concat('abc', 'def')
//@[0:45) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:22)  IdentifierSyntax
//@[4:22)   Identifier |singleLineFunction|
//@[23:24)  Assignment |=|
//@[25:45)  FunctionCallSyntax
//@[25:31)   IdentifierSyntax
//@[25:31)    Identifier |concat|
//@[31:32)   LeftParen |(|
//@[32:37)   FunctionArgumentSyntax
//@[32:37)    StringSyntax
//@[32:37)     StringComplete |'abc'|
//@[37:38)   Comma |,|
//@[39:44)   FunctionArgumentSyntax
//@[39:44)    StringSyntax
//@[39:44)     StringComplete |'def'|
//@[44:45)   RightParen |)|
//@[45:47) NewLine |\n\n|

var multiLineFunction = concat(
//@[0:50) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:21)  IdentifierSyntax
//@[4:21)   Identifier |multiLineFunction|
//@[22:23)  Assignment |=|
//@[24:50)  FunctionCallSyntax
//@[24:30)   IdentifierSyntax
//@[24:30)    Identifier |concat|
//@[30:31)   LeftParen |(|
//@[31:32)   NewLine |\n|
  'abc',
//@[2:7)   FunctionArgumentSyntax
//@[2:7)    StringSyntax
//@[2:7)     StringComplete |'abc'|
//@[7:8)   Comma |,|
//@[8:9)   NewLine |\n|
  'def'
//@[2:7)   FunctionArgumentSyntax
//@[2:7)    StringSyntax
//@[2:7)     StringComplete |'def'|
//@[7:8)   NewLine |\n|
)
//@[0:1)   RightParen |)|
//@[1:3) NewLine |\n\n|

var singleLineArray = ['abc', 'def']
//@[0:36) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:19)  IdentifierSyntax
//@[4:19)   Identifier |singleLineArray|
//@[20:21)  Assignment |=|
//@[22:36)  ArraySyntax
//@[22:23)   LeftSquare |[|
//@[23:28)   ArrayItemSyntax
//@[23:28)    StringSyntax
//@[23:28)     StringComplete |'abc'|
//@[28:29)   Comma |,|
//@[30:35)   ArrayItemSyntax
//@[30:35)    StringSyntax
//@[30:35)     StringComplete |'def'|
//@[35:36)   RightSquare |]|
//@[36:37) NewLine |\n|
var singleLineArrayTrailingCommas = ['abc', 'def',]
//@[0:51) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:33)  IdentifierSyntax
//@[4:33)   Identifier |singleLineArrayTrailingCommas|
//@[34:35)  Assignment |=|
//@[36:51)  ArraySyntax
//@[36:37)   LeftSquare |[|
//@[37:42)   ArrayItemSyntax
//@[37:42)    StringSyntax
//@[37:42)     StringComplete |'abc'|
//@[42:43)   Comma |,|
//@[44:49)   ArrayItemSyntax
//@[44:49)    StringSyntax
//@[44:49)     StringComplete |'def'|
//@[49:50)   Comma |,|
//@[50:51)   RightSquare |]|
//@[51:53) NewLine |\n\n|

var multiLineArray = [
//@[0:40) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:18)  IdentifierSyntax
//@[4:18)   Identifier |multiLineArray|
//@[19:20)  Assignment |=|
//@[21:40)  ArraySyntax
//@[21:22)   LeftSquare |[|
//@[22:23)   NewLine |\n|
  'abc'
//@[2:7)   ArrayItemSyntax
//@[2:7)    StringSyntax
//@[2:7)     StringComplete |'abc'|
//@[7:8)   NewLine |\n|
  'def'
//@[2:7)   ArrayItemSyntax
//@[2:7)    StringSyntax
//@[2:7)     StringComplete |'def'|
//@[7:8)   NewLine |\n|
]
//@[0:1)   RightSquare |]|
//@[1:2) NewLine |\n|
var multiLineArrayCommas = [
//@[0:48) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:24)  IdentifierSyntax
//@[4:24)   Identifier |multiLineArrayCommas|
//@[25:26)  Assignment |=|
//@[27:48)  ArraySyntax
//@[27:28)   LeftSquare |[|
//@[28:29)   NewLine |\n|
  'abc',
//@[2:7)   ArrayItemSyntax
//@[2:7)    StringSyntax
//@[2:7)     StringComplete |'abc'|
//@[7:8)   Comma |,|
//@[8:9)   NewLine |\n|
  'def',
//@[2:7)   ArrayItemSyntax
//@[2:7)    StringSyntax
//@[2:7)     StringComplete |'def'|
//@[7:8)   Comma |,|
//@[8:9)   NewLine |\n|
]
//@[0:1)   RightSquare |]|
//@[1:3) NewLine |\n\n|

var mixedArray = ['abc', 'def'
//@[0:51) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:14)  IdentifierSyntax
//@[4:14)   Identifier |mixedArray|
//@[15:16)  Assignment |=|
//@[17:51)  ArraySyntax
//@[17:18)   LeftSquare |[|
//@[18:23)   ArrayItemSyntax
//@[18:23)    StringSyntax
//@[18:23)     StringComplete |'abc'|
//@[23:24)   Comma |,|
//@[25:30)   ArrayItemSyntax
//@[25:30)    StringSyntax
//@[25:30)     StringComplete |'def'|
//@[30:31)   NewLine |\n|
'ghi', 'jkl',
//@[0:5)   ArrayItemSyntax
//@[0:5)    StringSyntax
//@[0:5)     StringComplete |'ghi'|
//@[5:6)   Comma |,|
//@[7:12)   ArrayItemSyntax
//@[7:12)    StringSyntax
//@[7:12)     StringComplete |'jkl'|
//@[12:13)   Comma |,|
//@[13:14)   NewLine |\n|
'lmn']
//@[0:5)   ArrayItemSyntax
//@[0:5)    StringSyntax
//@[0:5)     StringComplete |'lmn'|
//@[5:6)   RightSquare |]|
//@[6:8) NewLine |\n\n|

var singleLineObject = { abc: 'def', ghi: 'jkl'}
//@[0:48) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:20)  IdentifierSyntax
//@[4:20)   Identifier |singleLineObject|
//@[21:22)  Assignment |=|
//@[23:48)  ObjectSyntax
//@[23:24)   LeftBrace |{|
//@[25:35)   ObjectPropertySyntax
//@[25:28)    IdentifierSyntax
//@[25:28)     Identifier |abc|
//@[28:29)    Colon |:|
//@[30:35)    StringSyntax
//@[30:35)     StringComplete |'def'|
//@[35:36)   Comma |,|
//@[37:47)   ObjectPropertySyntax
//@[37:40)    IdentifierSyntax
//@[37:40)     Identifier |ghi|
//@[40:41)    Colon |:|
//@[42:47)    StringSyntax
//@[42:47)     StringComplete |'jkl'|
//@[47:48)   RightBrace |}|
//@[48:49) NewLine |\n|
var singleLineObjectTrailingCommas = { abc: 'def', ghi: 'jkl',}
//@[0:63) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:34)  IdentifierSyntax
//@[4:34)   Identifier |singleLineObjectTrailingCommas|
//@[35:36)  Assignment |=|
//@[37:63)  ObjectSyntax
//@[37:38)   LeftBrace |{|
//@[39:49)   ObjectPropertySyntax
//@[39:42)    IdentifierSyntax
//@[39:42)     Identifier |abc|
//@[42:43)    Colon |:|
//@[44:49)    StringSyntax
//@[44:49)     StringComplete |'def'|
//@[49:50)   Comma |,|
//@[51:61)   ObjectPropertySyntax
//@[51:54)    IdentifierSyntax
//@[51:54)     Identifier |ghi|
//@[54:55)    Colon |:|
//@[56:61)    StringSyntax
//@[56:61)     StringComplete |'jkl'|
//@[61:62)   Comma |,|
//@[62:63)   RightBrace |}|
//@[63:64) NewLine |\n|
var multiLineObject = {
//@[0:51) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:19)  IdentifierSyntax
//@[4:19)   Identifier |multiLineObject|
//@[20:21)  Assignment |=|
//@[22:51)  ObjectSyntax
//@[22:23)   LeftBrace |{|
//@[23:24)   NewLine |\n|
  abc: 'def'
//@[2:12)   ObjectPropertySyntax
//@[2:5)    IdentifierSyntax
//@[2:5)     Identifier |abc|
//@[5:6)    Colon |:|
//@[7:12)    StringSyntax
//@[7:12)     StringComplete |'def'|
//@[12:13)   NewLine |\n|
  ghi: 'jkl'
//@[2:12)   ObjectPropertySyntax
//@[2:5)    IdentifierSyntax
//@[2:5)     Identifier |ghi|
//@[5:6)    Colon |:|
//@[7:12)    StringSyntax
//@[7:12)     StringComplete |'jkl'|
//@[12:13)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:2) NewLine |\n|
var multiLineObjectCommas = {
//@[0:59) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:25)  IdentifierSyntax
//@[4:25)   Identifier |multiLineObjectCommas|
//@[26:27)  Assignment |=|
//@[28:59)  ObjectSyntax
//@[28:29)   LeftBrace |{|
//@[29:30)   NewLine |\n|
  abc: 'def',
//@[2:12)   ObjectPropertySyntax
//@[2:5)    IdentifierSyntax
//@[2:5)     Identifier |abc|
//@[5:6)    Colon |:|
//@[7:12)    StringSyntax
//@[7:12)     StringComplete |'def'|
//@[12:13)   Comma |,|
//@[13:14)   NewLine |\n|
  ghi: 'jkl',
//@[2:12)   ObjectPropertySyntax
//@[2:5)    IdentifierSyntax
//@[2:5)     Identifier |ghi|
//@[5:6)    Colon |:|
//@[7:12)    StringSyntax
//@[7:12)     StringComplete |'jkl'|
//@[12:13)   Comma |,|
//@[13:14)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:2) NewLine |\n|
var mixedObject = { abc: 'abc', def: 'def'
//@[0:79) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:15)  IdentifierSyntax
//@[4:15)   Identifier |mixedObject|
//@[16:17)  Assignment |=|
//@[18:79)  ObjectSyntax
//@[18:19)   LeftBrace |{|
//@[20:30)   ObjectPropertySyntax
//@[20:23)    IdentifierSyntax
//@[20:23)     Identifier |abc|
//@[23:24)    Colon |:|
//@[25:30)    StringSyntax
//@[25:30)     StringComplete |'abc'|
//@[30:31)   Comma |,|
//@[32:42)   ObjectPropertySyntax
//@[32:35)    IdentifierSyntax
//@[32:35)     Identifier |def|
//@[35:36)    Colon |:|
//@[37:42)    StringSyntax
//@[37:42)     StringComplete |'def'|
//@[42:43)   NewLine |\n|
ghi: 'ghi', jkl: 'jkl',
//@[0:10)   ObjectPropertySyntax
//@[0:3)    IdentifierSyntax
//@[0:3)     Identifier |ghi|
//@[3:4)    Colon |:|
//@[5:10)    StringSyntax
//@[5:10)     StringComplete |'ghi'|
//@[10:11)   Comma |,|
//@[12:22)   ObjectPropertySyntax
//@[12:15)    IdentifierSyntax
//@[12:15)     Identifier |jkl|
//@[15:16)    Colon |:|
//@[17:22)    StringSyntax
//@[17:22)     StringComplete |'jkl'|
//@[22:23)   Comma |,|
//@[23:24)   NewLine |\n|
lmn: 'lmn' }
//@[0:10)   ObjectPropertySyntax
//@[0:3)    IdentifierSyntax
//@[0:3)     Identifier |lmn|
//@[3:4)    Colon |:|
//@[5:10)    StringSyntax
//@[5:10)     StringComplete |'lmn'|
//@[11:12)   RightBrace |}|
//@[12:14) NewLine |\n\n|

var nestedMixed = {
//@[0:88) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:15)  IdentifierSyntax
//@[4:15)   Identifier |nestedMixed|
//@[16:17)  Assignment |=|
//@[18:88)  ObjectSyntax
//@[18:19)   LeftBrace |{|
//@[19:20)   NewLine |\n|
  abc: { 'def': 'ghi', abc: 'def', foo: [
//@[2:66)   ObjectPropertySyntax
//@[2:5)    IdentifierSyntax
//@[2:5)     Identifier |abc|
//@[5:6)    Colon |:|
//@[7:66)    ObjectSyntax
//@[7:8)     LeftBrace |{|
//@[9:21)     ObjectPropertySyntax
//@[9:14)      StringSyntax
//@[9:14)       StringComplete |'def'|
//@[14:15)      Colon |:|
//@[16:21)      StringSyntax
//@[16:21)       StringComplete |'ghi'|
//@[21:22)     Comma |,|
//@[23:33)     ObjectPropertySyntax
//@[23:26)      IdentifierSyntax
//@[23:26)       Identifier |abc|
//@[26:27)      Colon |:|
//@[28:33)      StringSyntax
//@[28:33)       StringComplete |'def'|
//@[33:34)     Comma |,|
//@[35:64)     ObjectPropertySyntax
//@[35:38)      IdentifierSyntax
//@[35:38)       Identifier |foo|
//@[38:39)      Colon |:|
//@[40:64)      ArraySyntax
//@[40:41)       LeftSquare |[|
//@[41:42)       NewLine |\n|
    'bar', 'blah',
//@[4:9)       ArrayItemSyntax
//@[4:9)        StringSyntax
//@[4:9)         StringComplete |'bar'|
//@[9:10)       Comma |,|
//@[11:17)       ArrayItemSyntax
//@[11:17)        StringSyntax
//@[11:17)         StringComplete |'blah'|
//@[17:18)       Comma |,|
//@[18:19)       NewLine |\n|
  ] }
//@[2:3)       RightSquare |]|
//@[4:5)     RightBrace |}|
//@[5:6)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:2) NewLine |\n|

//@[0:0) EndOfFile ||
