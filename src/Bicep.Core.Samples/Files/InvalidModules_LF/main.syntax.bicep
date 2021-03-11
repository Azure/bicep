module nonExistentFileRef './nonExistent.bicep' = {
//@[0:54) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:25)  IdentifierSyntax
//@[7:25)   Identifier |nonExistentFileRef|
//@[26:47)  StringSyntax
//@[26:47)   StringComplete |'./nonExistent.bicep'|
//@[48:49)  Assignment |=|
//@[50:54)  ObjectSyntax
//@[50:51)   LeftBrace |{|
//@[51:53)   NewLine |\n\n|

}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

// we should only look this file up once, but should still return the same failure
//@[82:83) NewLine |\n|
module nonExistentFileRefDuplicate './nonExistent.bicep' = {
//@[0:63) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:34)  IdentifierSyntax
//@[7:34)   Identifier |nonExistentFileRefDuplicate|
//@[35:56)  StringSyntax
//@[35:56)   StringComplete |'./nonExistent.bicep'|
//@[57:58)  Assignment |=|
//@[59:63)  ObjectSyntax
//@[59:60)   LeftBrace |{|
//@[60:62)   NewLine |\n\n|

}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

// we should only look this file up once, but should still return the same failure
//@[82:83) NewLine |\n|
module nonExistentFileRefEquivalentPath 'abc/def/../../nonExistent.bicep' = {
//@[0:80) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:39)  IdentifierSyntax
//@[7:39)   Identifier |nonExistentFileRefEquivalentPath|
//@[40:73)  StringSyntax
//@[40:73)   StringComplete |'abc/def/../../nonExistent.bicep'|
//@[74:75)  Assignment |=|
//@[76:80)  ObjectSyntax
//@[76:77)   LeftBrace |{|
//@[77:79)   NewLine |\n\n|

}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

module moduleWithoutPath = {
//@[0:28) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:24)  IdentifierSyntax
//@[7:24)   Identifier |moduleWithoutPath|
//@[25:28)  SkippedTriviaSyntax
//@[25:26)   Assignment |=|
//@[27:28)   LeftBrace |{|
//@[28:28)  SkippedTriviaSyntax
//@[28:28)  SkippedTriviaSyntax
//@[28:30) NewLine |\n\n|

}
//@[0:1) SkippedTriviaSyntax
//@[0:1)  RightBrace |}|
//@[1:3) NewLine |\n\n|

// missing identifier #completionTest(7) -> empty
//@[49:50) NewLine |\n|
module 
//@[0:7) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:7)  IdentifierSyntax
//@[7:7)   SkippedTriviaSyntax
//@[7:7)  SkippedTriviaSyntax
//@[7:7)  SkippedTriviaSyntax
//@[7:7)  SkippedTriviaSyntax
//@[7:9) NewLine |\n\n|

// #completionTest(24,25) -> object
//@[35:36) NewLine |\n|
module missingValue '' = 
//@[0:25) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:19)  IdentifierSyntax
//@[7:19)   Identifier |missingValue|
//@[20:22)  StringSyntax
//@[20:22)   StringComplete |''|
//@[23:24)  Assignment |=|
//@[25:25)  SkippedTriviaSyntax
//@[25:27) NewLine |\n\n|

var interp = 'hello'
//@[0:20) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:10)  IdentifierSyntax
//@[4:10)   Identifier |interp|
//@[11:12)  Assignment |=|
//@[13:20)  StringSyntax
//@[13:20)   StringComplete |'hello'|
//@[20:21) NewLine |\n|
module moduleWithInterpPath './${interp}.bicep' = {
//@[0:54) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:27)  IdentifierSyntax
//@[7:27)   Identifier |moduleWithInterpPath|
//@[28:47)  StringSyntax
//@[28:33)   StringLeftPiece |'./${|
//@[33:39)   VariableAccessSyntax
//@[33:39)    IdentifierSyntax
//@[33:39)     Identifier |interp|
//@[39:47)   StringRightPiece |}.bicep'|
//@[48:49)  Assignment |=|
//@[50:54)  ObjectSyntax
//@[50:51)   LeftBrace |{|
//@[51:53)   NewLine |\n\n|

}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

module moduleWithConditionAndInterpPath './${interp}.bicep' = if (true) {
//@[0:76) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:39)  IdentifierSyntax
//@[7:39)   Identifier |moduleWithConditionAndInterpPath|
//@[40:59)  StringSyntax
//@[40:45)   StringLeftPiece |'./${|
//@[45:51)   VariableAccessSyntax
//@[45:51)    IdentifierSyntax
//@[45:51)     Identifier |interp|
//@[51:59)   StringRightPiece |}.bicep'|
//@[60:61)  Assignment |=|
//@[62:76)  IfConditionSyntax
//@[62:64)   Identifier |if|
//@[65:71)   ParenthesizedExpressionSyntax
//@[65:66)    LeftParen |(|
//@[66:70)    BooleanLiteralSyntax
//@[66:70)     TrueKeyword |true|
//@[70:71)    RightParen |)|
//@[72:76)   ObjectSyntax
//@[72:73)    LeftBrace |{|
//@[73:75)    NewLine |\n\n|

}
//@[0:1)    RightBrace |}|
//@[1:3) NewLine |\n\n|

module moduleWithSelfCycle './main.bicep' = {
//@[0:48) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:26)  IdentifierSyntax
//@[7:26)   Identifier |moduleWithSelfCycle|
//@[27:41)  StringSyntax
//@[27:41)   StringComplete |'./main.bicep'|
//@[42:43)  Assignment |=|
//@[44:48)  ObjectSyntax
//@[44:45)   LeftBrace |{|
//@[45:47)   NewLine |\n\n|

}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

module moduleWithConditionAndSelfCycle './main.bicep' = if ('foo' == 'bar') {
//@[0:80) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:38)  IdentifierSyntax
//@[7:38)   Identifier |moduleWithConditionAndSelfCycle|
//@[39:53)  StringSyntax
//@[39:53)   StringComplete |'./main.bicep'|
//@[54:55)  Assignment |=|
//@[56:80)  IfConditionSyntax
//@[56:58)   Identifier |if|
//@[59:75)   ParenthesizedExpressionSyntax
//@[59:60)    LeftParen |(|
//@[60:74)    BinaryOperationSyntax
//@[60:65)     StringSyntax
//@[60:65)      StringComplete |'foo'|
//@[66:68)     Equals |==|
//@[69:74)     StringSyntax
//@[69:74)      StringComplete |'bar'|
//@[74:75)    RightParen |)|
//@[76:80)   ObjectSyntax
//@[76:77)    LeftBrace |{|
//@[77:79)    NewLine |\n\n|

}
//@[0:1)    RightBrace |}|
//@[1:3) NewLine |\n\n|

module './main.bicep' = {
//@[0:28) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:7)  IdentifierSyntax
//@[7:7)   SkippedTriviaSyntax
//@[7:21)  StringSyntax
//@[7:21)   StringComplete |'./main.bicep'|
//@[22:23)  Assignment |=|
//@[24:28)  ObjectSyntax
//@[24:25)   LeftBrace |{|
//@[25:27)   NewLine |\n\n|

}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

module './main.bicep' = if (1 + 2 == 3) {
//@[0:44) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:7)  IdentifierSyntax
//@[7:7)   SkippedTriviaSyntax
//@[7:21)  StringSyntax
//@[7:21)   StringComplete |'./main.bicep'|
//@[22:23)  Assignment |=|
//@[24:44)  IfConditionSyntax
//@[24:26)   Identifier |if|
//@[27:39)   ParenthesizedExpressionSyntax
//@[27:28)    LeftParen |(|
//@[28:38)    BinaryOperationSyntax
//@[28:33)     BinaryOperationSyntax
//@[28:29)      IntegerLiteralSyntax
//@[28:29)       Integer |1|
//@[30:31)      Plus |+|
//@[32:33)      IntegerLiteralSyntax
//@[32:33)       Integer |2|
//@[34:36)     Equals |==|
//@[37:38)     IntegerLiteralSyntax
//@[37:38)      Integer |3|
//@[38:39)    RightParen |)|
//@[40:44)   ObjectSyntax
//@[40:41)    LeftBrace |{|
//@[41:43)    NewLine |\n\n|

}
//@[0:1)    RightBrace |}|
//@[1:3) NewLine |\n\n|

module './main.bicep' = if
//@[0:26) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:7)  IdentifierSyntax
//@[7:7)   SkippedTriviaSyntax
//@[7:21)  StringSyntax
//@[7:21)   StringComplete |'./main.bicep'|
//@[22:23)  Assignment |=|
//@[24:26)  IfConditionSyntax
//@[24:26)   Identifier |if|
//@[26:26)   SkippedTriviaSyntax
//@[26:26)   SkippedTriviaSyntax
//@[26:28) NewLine |\n\n|

module './main.bicep' = if (
//@[0:28) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:7)  IdentifierSyntax
//@[7:7)   SkippedTriviaSyntax
//@[7:21)  StringSyntax
//@[7:21)   StringComplete |'./main.bicep'|
//@[22:23)  Assignment |=|
//@[24:28)  IfConditionSyntax
//@[24:26)   Identifier |if|
//@[27:28)   ParenthesizedExpressionSyntax
//@[27:28)    LeftParen |(|
//@[28:28)    SkippedTriviaSyntax
//@[28:28)    SkippedTriviaSyntax
//@[28:28)   SkippedTriviaSyntax
//@[28:30) NewLine |\n\n|

module './main.bicep' = if (true
//@[0:32) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:7)  IdentifierSyntax
//@[7:7)   SkippedTriviaSyntax
//@[7:21)  StringSyntax
//@[7:21)   StringComplete |'./main.bicep'|
//@[22:23)  Assignment |=|
//@[24:32)  IfConditionSyntax
//@[24:26)   Identifier |if|
//@[27:32)   ParenthesizedExpressionSyntax
//@[27:28)    LeftParen |(|
//@[28:32)    BooleanLiteralSyntax
//@[28:32)     TrueKeyword |true|
//@[32:32)    SkippedTriviaSyntax
//@[32:32)   SkippedTriviaSyntax
//@[32:34) NewLine |\n\n|

module './main.bicep' = if (true)
//@[0:33) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:7)  IdentifierSyntax
//@[7:7)   SkippedTriviaSyntax
//@[7:21)  StringSyntax
//@[7:21)   StringComplete |'./main.bicep'|
//@[22:23)  Assignment |=|
//@[24:33)  IfConditionSyntax
//@[24:26)   Identifier |if|
//@[27:33)   ParenthesizedExpressionSyntax
//@[27:28)    LeftParen |(|
//@[28:32)    BooleanLiteralSyntax
//@[28:32)     TrueKeyword |true|
//@[32:33)    RightParen |)|
//@[33:33)   SkippedTriviaSyntax
//@[33:35) NewLine |\n\n|

module './main.bicep' = if {
//@[0:31) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:7)  IdentifierSyntax
//@[7:7)   SkippedTriviaSyntax
//@[7:21)  StringSyntax
//@[7:21)   StringComplete |'./main.bicep'|
//@[22:23)  Assignment |=|
//@[24:31)  IfConditionSyntax
//@[24:26)   Identifier |if|
//@[27:27)   SkippedTriviaSyntax
//@[27:31)   ObjectSyntax
//@[27:28)    LeftBrace |{|
//@[28:30)    NewLine |\n\n|

}
//@[0:1)    RightBrace |}|
//@[1:3) NewLine |\n\n|

module './main.bicep' = if () {
//@[0:34) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:7)  IdentifierSyntax
//@[7:7)   SkippedTriviaSyntax
//@[7:21)  StringSyntax
//@[7:21)   StringComplete |'./main.bicep'|
//@[22:23)  Assignment |=|
//@[24:34)  IfConditionSyntax
//@[24:26)   Identifier |if|
//@[27:29)   ParenthesizedExpressionSyntax
//@[27:28)    LeftParen |(|
//@[28:28)    SkippedTriviaSyntax
//@[28:29)    RightParen |)|
//@[30:34)   ObjectSyntax
//@[30:31)    LeftBrace |{|
//@[31:33)    NewLine |\n\n|

}
//@[0:1)    RightBrace |}|
//@[1:3) NewLine |\n\n|

module './main.bicep' = if ('true') {
//@[0:40) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:7)  IdentifierSyntax
//@[7:7)   SkippedTriviaSyntax
//@[7:21)  StringSyntax
//@[7:21)   StringComplete |'./main.bicep'|
//@[22:23)  Assignment |=|
//@[24:40)  IfConditionSyntax
//@[24:26)   Identifier |if|
//@[27:35)   ParenthesizedExpressionSyntax
//@[27:28)    LeftParen |(|
//@[28:34)    StringSyntax
//@[28:34)     StringComplete |'true'|
//@[34:35)    RightParen |)|
//@[36:40)   ObjectSyntax
//@[36:37)    LeftBrace |{|
//@[37:39)    NewLine |\n\n|

}
//@[0:1)    RightBrace |}|
//@[1:3) NewLine |\n\n|

module modANoName './modulea.bicep' = {
//@[0:93) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:17)  IdentifierSyntax
//@[7:17)   Identifier |modANoName|
//@[18:35)  StringSyntax
//@[18:35)   StringComplete |'./modulea.bicep'|
//@[36:37)  Assignment |=|
//@[38:93)  ObjectSyntax
//@[38:39)   LeftBrace |{|
//@[39:40)   NewLine |\n|
// #completionTest(0) -> moduleATopLevelProperties
//@[50:52)   NewLine |\n\n|

}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

module modANoNameWithCondition './modulea.bicep' = if (true) {
//@[0:129) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:30)  IdentifierSyntax
//@[7:30)   Identifier |modANoNameWithCondition|
//@[31:48)  StringSyntax
//@[31:48)   StringComplete |'./modulea.bicep'|
//@[49:50)  Assignment |=|
//@[51:129)  IfConditionSyntax
//@[51:53)   Identifier |if|
//@[54:60)   ParenthesizedExpressionSyntax
//@[54:55)    LeftParen |(|
//@[55:59)    BooleanLiteralSyntax
//@[55:59)     TrueKeyword |true|
//@[59:60)    RightParen |)|
//@[61:129)   ObjectSyntax
//@[61:62)    LeftBrace |{|
//@[62:63)    NewLine |\n|
// #completionTest(0) -> moduleAWithConditionTopLevelProperties
//@[63:65)    NewLine |\n\n|

}
//@[0:1)    RightBrace |}|
//@[1:3) NewLine |\n\n|

module modWithReferenceInCondition './main.bicep' = if (reference('Micorosft.Management/managementGroups/MG', '2020-05-01').name == 'something') {
//@[0:149) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:34)  IdentifierSyntax
//@[7:34)   Identifier |modWithReferenceInCondition|
//@[35:49)  StringSyntax
//@[35:49)   StringComplete |'./main.bicep'|
//@[50:51)  Assignment |=|
//@[52:149)  IfConditionSyntax
//@[52:54)   Identifier |if|
//@[55:144)   ParenthesizedExpressionSyntax
//@[55:56)    LeftParen |(|
//@[56:143)    BinaryOperationSyntax
//@[56:128)     PropertyAccessSyntax
//@[56:123)      FunctionCallSyntax
//@[56:65)       IdentifierSyntax
//@[56:65)        Identifier |reference|
//@[65:66)       LeftParen |(|
//@[66:109)       FunctionArgumentSyntax
//@[66:108)        StringSyntax
//@[66:108)         StringComplete |'Micorosft.Management/managementGroups/MG'|
//@[108:109)        Comma |,|
//@[110:122)       FunctionArgumentSyntax
//@[110:122)        StringSyntax
//@[110:122)         StringComplete |'2020-05-01'|
//@[122:123)       RightParen |)|
//@[123:124)      Dot |.|
//@[124:128)      IdentifierSyntax
//@[124:128)       Identifier |name|
//@[129:131)     Equals |==|
//@[132:143)     StringSyntax
//@[132:143)      StringComplete |'something'|
//@[143:144)    RightParen |)|
//@[145:149)   ObjectSyntax
//@[145:146)    LeftBrace |{|
//@[146:148)    NewLine |\n\n|

}
//@[0:1)    RightBrace |}|
//@[1:3) NewLine |\n\n|

module modWithListKeysInCondition './main.bicep' = if (listKeys('foo', '2020-05-01').bar == true) {
//@[0:102) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:33)  IdentifierSyntax
//@[7:33)   Identifier |modWithListKeysInCondition|
//@[34:48)  StringSyntax
//@[34:48)   StringComplete |'./main.bicep'|
//@[49:50)  Assignment |=|
//@[51:102)  IfConditionSyntax
//@[51:53)   Identifier |if|
//@[54:97)   ParenthesizedExpressionSyntax
//@[54:55)    LeftParen |(|
//@[55:96)    BinaryOperationSyntax
//@[55:88)     PropertyAccessSyntax
//@[55:84)      FunctionCallSyntax
//@[55:63)       IdentifierSyntax
//@[55:63)        Identifier |listKeys|
//@[63:64)       LeftParen |(|
//@[64:70)       FunctionArgumentSyntax
//@[64:69)        StringSyntax
//@[64:69)         StringComplete |'foo'|
//@[69:70)        Comma |,|
//@[71:83)       FunctionArgumentSyntax
//@[71:83)        StringSyntax
//@[71:83)         StringComplete |'2020-05-01'|
//@[83:84)       RightParen |)|
//@[84:85)      Dot |.|
//@[85:88)      IdentifierSyntax
//@[85:88)       Identifier |bar|
//@[89:91)     Equals |==|
//@[92:96)     BooleanLiteralSyntax
//@[92:96)      TrueKeyword |true|
//@[96:97)    RightParen |)|
//@[98:102)   ObjectSyntax
//@[98:99)    LeftBrace |{|
//@[99:101)    NewLine |\n\n|

}
//@[0:1)    RightBrace |}|
//@[1:4) NewLine |\n\n\n|


module modANoName './modulea.bicep' = if ({ 'a': b }.a == true) {
//@[0:68) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:17)  IdentifierSyntax
//@[7:17)   Identifier |modANoName|
//@[18:35)  StringSyntax
//@[18:35)   StringComplete |'./modulea.bicep'|
//@[36:37)  Assignment |=|
//@[38:68)  IfConditionSyntax
//@[38:40)   Identifier |if|
//@[41:68)   ParenthesizedExpressionSyntax
//@[41:42)    LeftParen |(|
//@[42:68)    ObjectSyntax
//@[42:43)     LeftBrace |{|
//@[44:50)     ObjectPropertySyntax
//@[44:47)      StringSyntax
//@[44:47)       StringComplete |'a'|
//@[47:48)      Colon |:|
//@[49:50)      VariableAccessSyntax
//@[49:50)       IdentifierSyntax
//@[49:50)        Identifier |b|
//@[51:67)     SkippedTriviaSyntax
//@[51:52)      RightBrace |}|
//@[52:53)      Dot |.|
//@[53:54)      Identifier |a|
//@[55:57)      Equals |==|
//@[58:62)      TrueKeyword |true|
//@[62:63)      RightParen |)|
//@[64:65)      LeftBrace |{|
//@[65:67)      NewLine |\n\n|

}
//@[0:1)     RightBrace |}|
//@[1:1)    SkippedTriviaSyntax
//@[1:1)   SkippedTriviaSyntax
//@[1:3) NewLine |\n\n|

module modANoInputs './modulea.bicep' = {
//@[0:135) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:19)  IdentifierSyntax
//@[7:19)   Identifier |modANoInputs|
//@[20:37)  StringSyntax
//@[20:37)   StringComplete |'./modulea.bicep'|
//@[38:39)  Assignment |=|
//@[40:135)  ObjectSyntax
//@[40:41)   LeftBrace |{|
//@[41:42)   NewLine |\n|
  name: 'modANoInputs'
//@[2:22)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:22)    StringSyntax
//@[8:22)     StringComplete |'modANoInputs'|
//@[22:23)   NewLine |\n|
  // #completionTest(0,1,2) -> moduleATopLevelPropertiesMinusName
//@[65:66)   NewLine |\n|
  
//@[2:3)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

module modANoInputsWithCondition './modulea.bicep' = if (length([
//@[0:191) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:32)  IdentifierSyntax
//@[7:32)   Identifier |modANoInputsWithCondition|
//@[33:50)  StringSyntax
//@[33:50)   StringComplete |'./modulea.bicep'|
//@[51:52)  Assignment |=|
//@[53:191)  IfConditionSyntax
//@[53:55)   Identifier |if|
//@[56:82)   ParenthesizedExpressionSyntax
//@[56:57)    LeftParen |(|
//@[57:81)    BinaryOperationSyntax
//@[57:76)     FunctionCallSyntax
//@[57:63)      IdentifierSyntax
//@[57:63)       Identifier |length|
//@[63:64)      LeftParen |(|
//@[64:75)      FunctionArgumentSyntax
//@[64:75)       ArraySyntax
//@[64:65)        LeftSquare |[|
//@[65:66)        NewLine |\n|
  'foo'
//@[2:7)        ArrayItemSyntax
//@[2:7)         StringSyntax
//@[2:7)          StringComplete |'foo'|
//@[7:8)        NewLine |\n|
]) == 1) {
//@[0:1)        RightSquare |]|
//@[1:2)      RightParen |)|
//@[3:5)     Equals |==|
//@[6:7)     IntegerLiteralSyntax
//@[6:7)      Integer |1|
//@[7:8)    RightParen |)|
//@[9:117)   ObjectSyntax
//@[9:10)    LeftBrace |{|
//@[10:11)    NewLine |\n|
  name: 'modANoInputs'
//@[2:22)    ObjectPropertySyntax
//@[2:6)     IdentifierSyntax
//@[2:6)      Identifier |name|
//@[6:7)     Colon |:|
//@[8:22)     StringSyntax
//@[8:22)      StringComplete |'modANoInputs'|
//@[22:23)    NewLine |\n|
  // #completionTest(0,1,2) -> moduleAWithConditionTopLevelPropertiesMinusName
//@[78:79)    NewLine |\n|
  
//@[2:3)    NewLine |\n|
}
//@[0:1)    RightBrace |}|
//@[1:3) NewLine |\n\n|

module modAEmptyInputs './modulea.bicep' = {
//@[0:141) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:22)  IdentifierSyntax
//@[7:22)   Identifier |modAEmptyInputs|
//@[23:40)  StringSyntax
//@[23:40)   StringComplete |'./modulea.bicep'|
//@[41:42)  Assignment |=|
//@[43:141)  ObjectSyntax
//@[43:44)   LeftBrace |{|
//@[44:45)   NewLine |\n|
  name: 'modANoInputs'
//@[2:22)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:22)    StringSyntax
//@[8:22)     StringComplete |'modANoInputs'|
//@[22:23)   NewLine |\n|
  params: {
//@[2:71)   ObjectPropertySyntax
//@[2:8)    IdentifierSyntax
//@[2:8)     Identifier |params|
//@[8:9)    Colon |:|
//@[10:71)    ObjectSyntax
//@[10:11)     LeftBrace |{|
//@[11:12)     NewLine |\n|
    // #completionTest(0,1,2,3,4) -> moduleAParams
//@[50:51)     NewLine |\n|
    
//@[4:5)     NewLine |\n|
  }
//@[2:3)     RightBrace |}|
//@[3:4)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

module modAEmptyInputsWithCondition './modulea.bicep' = if (1 + 2 == 2) {
//@[0:183) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:35)  IdentifierSyntax
//@[7:35)   Identifier |modAEmptyInputsWithCondition|
//@[36:53)  StringSyntax
//@[36:53)   StringComplete |'./modulea.bicep'|
//@[54:55)  Assignment |=|
//@[56:183)  IfConditionSyntax
//@[56:58)   Identifier |if|
//@[59:71)   ParenthesizedExpressionSyntax
//@[59:60)    LeftParen |(|
//@[60:70)    BinaryOperationSyntax
//@[60:65)     BinaryOperationSyntax
//@[60:61)      IntegerLiteralSyntax
//@[60:61)       Integer |1|
//@[62:63)      Plus |+|
//@[64:65)      IntegerLiteralSyntax
//@[64:65)       Integer |2|
//@[66:68)     Equals |==|
//@[69:70)     IntegerLiteralSyntax
//@[69:70)      Integer |2|
//@[70:71)    RightParen |)|
//@[72:183)   ObjectSyntax
//@[72:73)    LeftBrace |{|
//@[73:74)    NewLine |\n|
  name: 'modANoInputs'
//@[2:22)    ObjectPropertySyntax
//@[2:6)     IdentifierSyntax
//@[2:6)      Identifier |name|
//@[6:7)     Colon |:|
//@[8:22)     StringSyntax
//@[8:22)      StringComplete |'modANoInputs'|
//@[22:23)    NewLine |\n|
  params: {
//@[2:84)    ObjectPropertySyntax
//@[2:8)     IdentifierSyntax
//@[2:8)      Identifier |params|
//@[8:9)     Colon |:|
//@[10:84)     ObjectSyntax
//@[10:11)      LeftBrace |{|
//@[11:12)      NewLine |\n|
    // #completionTest(0,1,2,3,4) -> moduleAWithConditionParams
//@[63:64)      NewLine |\n|
    
//@[4:5)      NewLine |\n|
  }
//@[2:3)      RightBrace |}|
//@[3:4)    NewLine |\n|
}
//@[0:1)    RightBrace |}|
//@[1:3) NewLine |\n\n|

// #completionTest(55) -> moduleATopLevelPropertyAccess
//@[55:56) NewLine |\n|
var modulePropertyAccessCompletions = modAEmptyInputs.o
//@[0:55) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:35)  IdentifierSyntax
//@[4:35)   Identifier |modulePropertyAccessCompletions|
//@[36:37)  Assignment |=|
//@[38:55)  PropertyAccessSyntax
//@[38:53)   VariableAccessSyntax
//@[38:53)    IdentifierSyntax
//@[38:53)     Identifier |modAEmptyInputs|
//@[53:54)   Dot |.|
//@[54:55)   IdentifierSyntax
//@[54:55)    Identifier |o|
//@[55:57) NewLine |\n\n|

// #completionTest(81) -> moduleAWithConditionTopLevelPropertyAccess
//@[68:69) NewLine |\n|
var moduleWithConditionPropertyAccessCompletions = modAEmptyInputsWithCondition.o
//@[0:81) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:48)  IdentifierSyntax
//@[4:48)   Identifier |moduleWithConditionPropertyAccessCompletions|
//@[49:50)  Assignment |=|
//@[51:81)  PropertyAccessSyntax
//@[51:79)   VariableAccessSyntax
//@[51:79)    IdentifierSyntax
//@[51:79)     Identifier |modAEmptyInputsWithCondition|
//@[79:80)   Dot |.|
//@[80:81)   IdentifierSyntax
//@[80:81)    Identifier |o|
//@[81:83) NewLine |\n\n|

// #completionTest(56) -> moduleAOutputs
//@[40:41) NewLine |\n|
var moduleOutputsCompletions = modAEmptyInputs.outputs.s
//@[0:56) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:28)  IdentifierSyntax
//@[4:28)   Identifier |moduleOutputsCompletions|
//@[29:30)  Assignment |=|
//@[31:56)  PropertyAccessSyntax
//@[31:54)   PropertyAccessSyntax
//@[31:46)    VariableAccessSyntax
//@[31:46)     IdentifierSyntax
//@[31:46)      Identifier |modAEmptyInputs|
//@[46:47)    Dot |.|
//@[47:54)    IdentifierSyntax
//@[47:54)     Identifier |outputs|
//@[54:55)   Dot |.|
//@[55:56)   IdentifierSyntax
//@[55:56)    Identifier |s|
//@[56:58) NewLine |\n\n|

// #completionTest(82) -> moduleAWithConditionOutputs
//@[53:54) NewLine |\n|
var moduleWithConditionOutputsCompletions = modAEmptyInputsWithCondition.outputs.s
//@[0:82) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:41)  IdentifierSyntax
//@[4:41)   Identifier |moduleWithConditionOutputsCompletions|
//@[42:43)  Assignment |=|
//@[44:82)  PropertyAccessSyntax
//@[44:80)   PropertyAccessSyntax
//@[44:72)    VariableAccessSyntax
//@[44:72)     IdentifierSyntax
//@[44:72)      Identifier |modAEmptyInputsWithCondition|
//@[72:73)    Dot |.|
//@[73:80)    IdentifierSyntax
//@[73:80)     Identifier |outputs|
//@[80:81)   Dot |.|
//@[81:82)   IdentifierSyntax
//@[81:82)    Identifier |s|
//@[82:84) NewLine |\n\n|

module modAUnspecifiedInputs './modulea.bicep' = {
//@[0:180) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:28)  IdentifierSyntax
//@[7:28)   Identifier |modAUnspecifiedInputs|
//@[29:46)  StringSyntax
//@[29:46)   StringComplete |'./modulea.bicep'|
//@[47:48)  Assignment |=|
//@[49:180)  ObjectSyntax
//@[49:50)   LeftBrace |{|
//@[50:51)   NewLine |\n|
  name: 'modAUnspecifiedInputs'
//@[2:31)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:31)    StringSyntax
//@[8:31)     StringComplete |'modAUnspecifiedInputs'|
//@[31:32)   NewLine |\n|
  params: {
//@[2:95)   ObjectPropertySyntax
//@[2:8)    IdentifierSyntax
//@[2:8)     Identifier |params|
//@[8:9)    Colon |:|
//@[10:95)    ObjectSyntax
//@[10:11)     LeftBrace |{|
//@[11:12)     NewLine |\n|
    stringParamB: ''
//@[4:20)     ObjectPropertySyntax
//@[4:16)      IdentifierSyntax
//@[4:16)       Identifier |stringParamB|
//@[16:17)      Colon |:|
//@[18:20)      StringSyntax
//@[18:20)       StringComplete |''|
//@[20:21)     NewLine |\n|
    objParam: {}
//@[4:16)     ObjectPropertySyntax
//@[4:12)      IdentifierSyntax
//@[4:12)       Identifier |objParam|
//@[12:13)      Colon |:|
//@[14:16)      ObjectSyntax
//@[14:15)       LeftBrace |{|
//@[15:16)       RightBrace |}|
//@[16:17)     NewLine |\n|
    objArray: []
//@[4:16)     ObjectPropertySyntax
//@[4:12)      IdentifierSyntax
//@[4:12)       Identifier |objArray|
//@[12:13)      Colon |:|
//@[14:16)      ArraySyntax
//@[14:15)       LeftSquare |[|
//@[15:16)       RightSquare |]|
//@[16:17)     NewLine |\n|
    unspecifiedInput: ''
//@[4:24)     ObjectPropertySyntax
//@[4:20)      IdentifierSyntax
//@[4:20)       Identifier |unspecifiedInput|
//@[20:21)      Colon |:|
//@[22:24)      StringSyntax
//@[22:24)       StringComplete |''|
//@[24:25)     NewLine |\n|
  }
//@[2:3)     RightBrace |}|
//@[3:4)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

var unspecifiedOutput = modAUnspecifiedInputs.outputs.test
//@[0:58) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:21)  IdentifierSyntax
//@[4:21)   Identifier |unspecifiedOutput|
//@[22:23)  Assignment |=|
//@[24:58)  PropertyAccessSyntax
//@[24:53)   PropertyAccessSyntax
//@[24:45)    VariableAccessSyntax
//@[24:45)     IdentifierSyntax
//@[24:45)      Identifier |modAUnspecifiedInputs|
//@[45:46)    Dot |.|
//@[46:53)    IdentifierSyntax
//@[46:53)     Identifier |outputs|
//@[53:54)   Dot |.|
//@[54:58)   IdentifierSyntax
//@[54:58)    Identifier |test|
//@[58:60) NewLine |\n\n|

module modCycle './cycle.bicep' = {
//@[0:40) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:15)  IdentifierSyntax
//@[7:15)   Identifier |modCycle|
//@[16:31)  StringSyntax
//@[16:31)   StringComplete |'./cycle.bicep'|
//@[32:33)  Assignment |=|
//@[34:40)  ObjectSyntax
//@[34:35)   LeftBrace |{|
//@[35:36)   NewLine |\n|
  
//@[2:3)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

module moduleWithEmptyPath '' = {
//@[0:35) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:26)  IdentifierSyntax
//@[7:26)   Identifier |moduleWithEmptyPath|
//@[27:29)  StringSyntax
//@[27:29)   StringComplete |''|
//@[30:31)  Assignment |=|
//@[32:35)  ObjectSyntax
//@[32:33)   LeftBrace |{|
//@[33:34)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

module moduleWithAbsolutePath '/abc/def.bicep' = {
//@[0:52) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:29)  IdentifierSyntax
//@[7:29)   Identifier |moduleWithAbsolutePath|
//@[30:46)  StringSyntax
//@[30:46)   StringComplete |'/abc/def.bicep'|
//@[47:48)  Assignment |=|
//@[49:52)  ObjectSyntax
//@[49:50)   LeftBrace |{|
//@[50:51)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

module moduleWithBackslash 'child\\file.bicep' = {
//@[0:52) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:26)  IdentifierSyntax
//@[7:26)   Identifier |moduleWithBackslash|
//@[27:46)  StringSyntax
//@[27:46)   StringComplete |'child\\file.bicep'|
//@[47:48)  Assignment |=|
//@[49:52)  ObjectSyntax
//@[49:50)   LeftBrace |{|
//@[50:51)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

module moduleWithInvalidChar 'child/fi|le.bicep' = {
//@[0:54) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:28)  IdentifierSyntax
//@[7:28)   Identifier |moduleWithInvalidChar|
//@[29:48)  StringSyntax
//@[29:48)   StringComplete |'child/fi|le.bicep'|
//@[49:50)  Assignment |=|
//@[51:54)  ObjectSyntax
//@[51:52)   LeftBrace |{|
//@[52:53)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

module moduleWithInvalidTerminatorChar 'child/test.' = {
//@[0:58) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:38)  IdentifierSyntax
//@[7:38)   Identifier |moduleWithInvalidTerminatorChar|
//@[39:52)  StringSyntax
//@[39:52)   StringComplete |'child/test.'|
//@[53:54)  Assignment |=|
//@[55:58)  ObjectSyntax
//@[55:56)   LeftBrace |{|
//@[56:57)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

module moduleWithValidScope './empty.bicep' = {
//@[0:80) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:27)  IdentifierSyntax
//@[7:27)   Identifier |moduleWithValidScope|
//@[28:43)  StringSyntax
//@[28:43)   StringComplete |'./empty.bicep'|
//@[44:45)  Assignment |=|
//@[46:80)  ObjectSyntax
//@[46:47)   LeftBrace |{|
//@[47:48)   NewLine |\n|
  name: 'moduleWithValidScope'
//@[2:30)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:30)    StringSyntax
//@[8:30)     StringComplete |'moduleWithValidScope'|
//@[30:31)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

module moduleWithInvalidScope './empty.bicep' = {
//@[0:114) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:29)  IdentifierSyntax
//@[7:29)   Identifier |moduleWithInvalidScope|
//@[30:45)  StringSyntax
//@[30:45)   StringComplete |'./empty.bicep'|
//@[46:47)  Assignment |=|
//@[48:114)  ObjectSyntax
//@[48:49)   LeftBrace |{|
//@[49:50)   NewLine |\n|
  name: 'moduleWithInvalidScope'
//@[2:32)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:32)    StringSyntax
//@[8:32)     StringComplete |'moduleWithInvalidScope'|
//@[32:33)   NewLine |\n|
  scope: moduleWithValidScope
//@[2:29)   ObjectPropertySyntax
//@[2:7)    IdentifierSyntax
//@[2:7)     Identifier |scope|
//@[7:8)    Colon |:|
//@[9:29)    VariableAccessSyntax
//@[9:29)     IdentifierSyntax
//@[9:29)      Identifier |moduleWithValidScope|
//@[29:30)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

module moduleWithMissingRequiredScope './subscription_empty.bicep' = {
//@[0:113) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:37)  IdentifierSyntax
//@[7:37)   Identifier |moduleWithMissingRequiredScope|
//@[38:66)  StringSyntax
//@[38:66)   StringComplete |'./subscription_empty.bicep'|
//@[67:68)  Assignment |=|
//@[69:113)  ObjectSyntax
//@[69:70)   LeftBrace |{|
//@[70:71)   NewLine |\n|
  name: 'moduleWithMissingRequiredScope'
//@[2:40)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:40)    StringSyntax
//@[8:40)     StringComplete |'moduleWithMissingRequiredScope'|
//@[40:41)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

module moduleWithInvalidScope2 './empty.bicep' = {
//@[0:113) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:30)  IdentifierSyntax
//@[7:30)   Identifier |moduleWithInvalidScope2|
//@[31:46)  StringSyntax
//@[31:46)   StringComplete |'./empty.bicep'|
//@[47:48)  Assignment |=|
//@[49:113)  ObjectSyntax
//@[49:50)   LeftBrace |{|
//@[50:51)   NewLine |\n|
  name: 'moduleWithInvalidScope2'
//@[2:33)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:33)    StringSyntax
//@[8:33)     StringComplete |'moduleWithInvalidScope2'|
//@[33:34)   NewLine |\n|
  scope: managementGroup()
//@[2:26)   ObjectPropertySyntax
//@[2:7)    IdentifierSyntax
//@[2:7)     Identifier |scope|
//@[7:8)    Colon |:|
//@[9:26)    FunctionCallSyntax
//@[9:24)     IdentifierSyntax
//@[9:24)      Identifier |managementGroup|
//@[24:25)     LeftParen |(|
//@[25:26)     RightParen |)|
//@[26:27)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

module moduleWithUnsupprtedScope1 './mg_empty.bicep' = {
//@[0:122) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:33)  IdentifierSyntax
//@[7:33)   Identifier |moduleWithUnsupprtedScope1|
//@[34:52)  StringSyntax
//@[34:52)   StringComplete |'./mg_empty.bicep'|
//@[53:54)  Assignment |=|
//@[55:122)  ObjectSyntax
//@[55:56)   LeftBrace |{|
//@[56:57)   NewLine |\n|
  name: 'moduleWithUnsupprtedScope1'
//@[2:36)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:36)    StringSyntax
//@[8:36)     StringComplete |'moduleWithUnsupprtedScope1'|
//@[36:37)   NewLine |\n|
  scope: managementGroup()
//@[2:26)   ObjectPropertySyntax
//@[2:7)    IdentifierSyntax
//@[2:7)     Identifier |scope|
//@[7:8)    Colon |:|
//@[9:26)    FunctionCallSyntax
//@[9:24)     IdentifierSyntax
//@[9:24)      Identifier |managementGroup|
//@[24:25)     LeftParen |(|
//@[25:26)     RightParen |)|
//@[26:27)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

module moduleWithUnsupprtedScope2 './mg_empty.bicep' = {
//@[0:126) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:33)  IdentifierSyntax
//@[7:33)   Identifier |moduleWithUnsupprtedScope2|
//@[34:52)  StringSyntax
//@[34:52)   StringComplete |'./mg_empty.bicep'|
//@[53:54)  Assignment |=|
//@[55:126)  ObjectSyntax
//@[55:56)   LeftBrace |{|
//@[56:57)   NewLine |\n|
  name: 'moduleWithUnsupprtedScope2'
//@[2:36)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:36)    StringSyntax
//@[8:36)     StringComplete |'moduleWithUnsupprtedScope2'|
//@[36:37)   NewLine |\n|
  scope: managementGroup('MG')
//@[2:30)   ObjectPropertySyntax
//@[2:7)    IdentifierSyntax
//@[2:7)     Identifier |scope|
//@[7:8)    Colon |:|
//@[9:30)    FunctionCallSyntax
//@[9:24)     IdentifierSyntax
//@[9:24)      Identifier |managementGroup|
//@[24:25)     LeftParen |(|
//@[25:29)     FunctionArgumentSyntax
//@[25:29)      StringSyntax
//@[25:29)       StringComplete |'MG'|
//@[29:30)     RightParen |)|
//@[30:31)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

module moduleWithBadScope './empty.bicep' = {
//@[0:99) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:25)  IdentifierSyntax
//@[7:25)   Identifier |moduleWithBadScope|
//@[26:41)  StringSyntax
//@[26:41)   StringComplete |'./empty.bicep'|
//@[42:43)  Assignment |=|
//@[44:99)  ObjectSyntax
//@[44:45)   LeftBrace |{|
//@[45:46)   NewLine |\n|
  name: 'moduleWithBadScope'
//@[2:28)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:28)    StringSyntax
//@[8:28)     StringComplete |'moduleWithBadScope'|
//@[28:29)   NewLine |\n|
  scope: 'stringScope'
//@[2:22)   ObjectPropertySyntax
//@[2:7)    IdentifierSyntax
//@[2:7)     Identifier |scope|
//@[7:8)    Colon |:|
//@[9:22)    StringSyntax
//@[9:22)     StringComplete |'stringScope'|
//@[22:23)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

resource runtimeValidRes1 'Microsoft.Storage/storageAccounts@2019-06-01' = {
//@[0:190) ResourceDeclarationSyntax
//@[0:8)  Identifier |resource|
//@[9:25)  IdentifierSyntax
//@[9:25)   Identifier |runtimeValidRes1|
//@[26:72)  StringSyntax
//@[26:72)   StringComplete |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[73:74)  Assignment |=|
//@[75:190)  ObjectSyntax
//@[75:76)   LeftBrace |{|
//@[76:77)   NewLine |\n|
  name: 'runtimeValidRes1Name'
//@[2:30)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:30)    StringSyntax
//@[8:30)     StringComplete |'runtimeValidRes1Name'|
//@[30:31)   NewLine |\n|
  location: 'westeurope'
//@[2:24)   ObjectPropertySyntax
//@[2:10)    IdentifierSyntax
//@[2:10)     Identifier |location|
//@[10:11)    Colon |:|
//@[12:24)    StringSyntax
//@[12:24)     StringComplete |'westeurope'|
//@[24:25)   NewLine |\n|
  kind: 'Storage'
//@[2:17)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |kind|
//@[6:7)    Colon |:|
//@[8:17)    StringSyntax
//@[8:17)     StringComplete |'Storage'|
//@[17:18)   NewLine |\n|
  sku: {
//@[2:37)   ObjectPropertySyntax
//@[2:5)    IdentifierSyntax
//@[2:5)     Identifier |sku|
//@[5:6)    Colon |:|
//@[7:37)    ObjectSyntax
//@[7:8)     LeftBrace |{|
//@[8:9)     NewLine |\n|
    name: 'Standard_GRS'
//@[4:24)     ObjectPropertySyntax
//@[4:8)      IdentifierSyntax
//@[4:8)       Identifier |name|
//@[8:9)      Colon |:|
//@[10:24)      StringSyntax
//@[10:24)       StringComplete |'Standard_GRS'|
//@[24:25)     NewLine |\n|
  }
//@[2:3)     RightBrace |}|
//@[3:4)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

module runtimeValidModule1 'empty.bicep' = {
//@[0:136) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:26)  IdentifierSyntax
//@[7:26)   Identifier |runtimeValidModule1|
//@[27:40)  StringSyntax
//@[27:40)   StringComplete |'empty.bicep'|
//@[41:42)  Assignment |=|
//@[43:136)  ObjectSyntax
//@[43:44)   LeftBrace |{|
//@[44:45)   NewLine |\n|
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
//@[89:90)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

module runtimeInvalidModule1 'empty.bicep' = {
//@[0:82) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:28)  IdentifierSyntax
//@[7:28)   Identifier |runtimeInvalidModule1|
//@[29:42)  StringSyntax
//@[29:42)   StringComplete |'empty.bicep'|
//@[43:44)  Assignment |=|
//@[45:82)  ObjectSyntax
//@[45:46)   LeftBrace |{|
//@[46:47)   NewLine |\n|
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
//@[33:34)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

module runtimeInvalidModule2 'empty.bicep' = {
//@[0:85) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:28)  IdentifierSyntax
//@[7:28)   Identifier |runtimeInvalidModule2|
//@[29:42)  StringSyntax
//@[29:42)   StringComplete |'empty.bicep'|
//@[43:44)  Assignment |=|
//@[45:85)  ObjectSyntax
//@[45:46)   LeftBrace |{|
//@[46:47)   NewLine |\n|
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
//@[36:37)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

module runtimeInvalidModule3 'empty.bicep' = {
//@[0:82) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:28)  IdentifierSyntax
//@[7:28)   Identifier |runtimeInvalidModule3|
//@[29:42)  StringSyntax
//@[29:42)   StringComplete |'empty.bicep'|
//@[43:44)  Assignment |=|
//@[45:82)  ObjectSyntax
//@[45:46)   LeftBrace |{|
//@[46:47)   NewLine |\n|
  name: runtimeValidRes1.sku.name
//@[2:33)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:33)    PropertyAccessSyntax
//@[8:28)     PropertyAccessSyntax
//@[8:24)      VariableAccessSyntax
//@[8:24)       IdentifierSyntax
//@[8:24)        Identifier |runtimeValidRes1|
//@[24:25)      Dot |.|
//@[25:28)      IdentifierSyntax
//@[25:28)       Identifier |sku|
//@[28:29)     Dot |.|
//@[29:33)     IdentifierSyntax
//@[29:33)      Identifier |name|
//@[33:34)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

module runtimeInvalidModule4 'empty.bicep' = {
//@[0:85) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:28)  IdentifierSyntax
//@[7:28)   Identifier |runtimeInvalidModule4|
//@[29:42)  StringSyntax
//@[29:42)   StringComplete |'empty.bicep'|
//@[43:44)  Assignment |=|
//@[45:85)  ObjectSyntax
//@[45:46)   LeftBrace |{|
//@[46:47)   NewLine |\n|
  name: runtimeValidRes1.sku['name']
//@[2:36)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:36)    ArrayAccessSyntax
//@[8:28)     PropertyAccessSyntax
//@[8:24)      VariableAccessSyntax
//@[8:24)       IdentifierSyntax
//@[8:24)        Identifier |runtimeValidRes1|
//@[24:25)      Dot |.|
//@[25:28)      IdentifierSyntax
//@[25:28)       Identifier |sku|
//@[28:29)     LeftSquare |[|
//@[29:35)     StringSyntax
//@[29:35)      StringComplete |'name'|
//@[35:36)     RightSquare |]|
//@[36:37)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

module runtimeInvalidModule5 'empty.bicep' = {
//@[0:88) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:28)  IdentifierSyntax
//@[7:28)   Identifier |runtimeInvalidModule5|
//@[29:42)  StringSyntax
//@[29:42)   StringComplete |'empty.bicep'|
//@[43:44)  Assignment |=|
//@[45:88)  ObjectSyntax
//@[45:46)   LeftBrace |{|
//@[46:47)   NewLine |\n|
  name: runtimeValidRes1['sku']['name']
//@[2:39)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:39)    ArrayAccessSyntax
//@[8:31)     ArrayAccessSyntax
//@[8:24)      VariableAccessSyntax
//@[8:24)       IdentifierSyntax
//@[8:24)        Identifier |runtimeValidRes1|
//@[24:25)      LeftSquare |[|
//@[25:30)      StringSyntax
//@[25:30)       StringComplete |'sku'|
//@[30:31)      RightSquare |]|
//@[31:32)     LeftSquare |[|
//@[32:38)     StringSyntax
//@[32:38)      StringComplete |'name'|
//@[38:39)     RightSquare |]|
//@[39:40)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

module runtimeInvalidModule6 'empty.bicep' = {
//@[0:85) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:28)  IdentifierSyntax
//@[7:28)   Identifier |runtimeInvalidModule6|
//@[29:42)  StringSyntax
//@[29:42)   StringComplete |'empty.bicep'|
//@[43:44)  Assignment |=|
//@[45:85)  ObjectSyntax
//@[45:46)   LeftBrace |{|
//@[46:47)   NewLine |\n|
  name: runtimeValidRes1['sku'].name
//@[2:36)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:36)    PropertyAccessSyntax
//@[8:31)     ArrayAccessSyntax
//@[8:24)      VariableAccessSyntax
//@[8:24)       IdentifierSyntax
//@[8:24)        Identifier |runtimeValidRes1|
//@[24:25)      LeftSquare |[|
//@[25:30)      StringSyntax
//@[25:30)       StringComplete |'sku'|
//@[30:31)      RightSquare |]|
//@[31:32)     Dot |.|
//@[32:36)     IdentifierSyntax
//@[32:36)      Identifier |name|
//@[36:37)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

module singleModuleForRuntimeCheck 'modulea.bicep' = {
//@[0:71) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:34)  IdentifierSyntax
//@[7:34)   Identifier |singleModuleForRuntimeCheck|
//@[35:50)  StringSyntax
//@[35:50)   StringComplete |'modulea.bicep'|
//@[51:52)  Assignment |=|
//@[53:71)  ObjectSyntax
//@[53:54)   LeftBrace |{|
//@[54:55)   NewLine |\n|
  name: 'test'
//@[2:14)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:14)    StringSyntax
//@[8:14)     StringComplete |'test'|
//@[14:15)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

var moduleRuntimeCheck = singleModuleForRuntimeCheck.outputs.stringOutputA
//@[0:74) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:22)  IdentifierSyntax
//@[4:22)   Identifier |moduleRuntimeCheck|
//@[23:24)  Assignment |=|
//@[25:74)  PropertyAccessSyntax
//@[25:60)   PropertyAccessSyntax
//@[25:52)    VariableAccessSyntax
//@[25:52)     IdentifierSyntax
//@[25:52)      Identifier |singleModuleForRuntimeCheck|
//@[52:53)    Dot |.|
//@[53:60)    IdentifierSyntax
//@[53:60)     Identifier |outputs|
//@[60:61)   Dot |.|
//@[61:74)   IdentifierSyntax
//@[61:74)    Identifier |stringOutputA|
//@[74:75) NewLine |\n|
var moduleRuntimeCheck2 = moduleRuntimeCheck
//@[0:44) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:23)  IdentifierSyntax
//@[4:23)   Identifier |moduleRuntimeCheck2|
//@[24:25)  Assignment |=|
//@[26:44)  VariableAccessSyntax
//@[26:44)   IdentifierSyntax
//@[26:44)    Identifier |moduleRuntimeCheck|
//@[44:46) NewLine |\n\n|

module moduleLoopForRuntimeCheck 'modulea.bicep' = [for thing in []: {
//@[0:101) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:32)  IdentifierSyntax
//@[7:32)   Identifier |moduleLoopForRuntimeCheck|
//@[33:48)  StringSyntax
//@[33:48)   StringComplete |'modulea.bicep'|
//@[49:50)  Assignment |=|
//@[51:101)  ForSyntax
//@[51:52)   LeftSquare |[|
//@[52:55)   Identifier |for|
//@[56:61)   LocalVariableSyntax
//@[56:61)    IdentifierSyntax
//@[56:61)     Identifier |thing|
//@[62:64)   Identifier |in|
//@[65:67)   ArraySyntax
//@[65:66)    LeftSquare |[|
//@[66:67)    RightSquare |]|
//@[67:68)   Colon |:|
//@[69:100)   ObjectSyntax
//@[69:70)    LeftBrace |{|
//@[70:71)    NewLine |\n|
  name: moduleRuntimeCheck2
//@[2:27)    ObjectPropertySyntax
//@[2:6)     IdentifierSyntax
//@[2:6)      Identifier |name|
//@[6:7)     Colon |:|
//@[8:27)     VariableAccessSyntax
//@[8:27)      IdentifierSyntax
//@[8:27)       Identifier |moduleRuntimeCheck2|
//@[27:28)    NewLine |\n|
}]
//@[0:1)    RightBrace |}|
//@[1:2)   RightSquare |]|
//@[2:4) NewLine |\n\n|

var moduleRuntimeCheck3 = moduleLoopForRuntimeCheck[1].outputs.stringOutputB
//@[0:76) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:23)  IdentifierSyntax
//@[4:23)   Identifier |moduleRuntimeCheck3|
//@[24:25)  Assignment |=|
//@[26:76)  PropertyAccessSyntax
//@[26:62)   PropertyAccessSyntax
//@[26:54)    ArrayAccessSyntax
//@[26:51)     VariableAccessSyntax
//@[26:51)      IdentifierSyntax
//@[26:51)       Identifier |moduleLoopForRuntimeCheck|
//@[51:52)     LeftSquare |[|
//@[52:53)     IntegerLiteralSyntax
//@[52:53)      Integer |1|
//@[53:54)     RightSquare |]|
//@[54:55)    Dot |.|
//@[55:62)    IdentifierSyntax
//@[55:62)     Identifier |outputs|
//@[62:63)   Dot |.|
//@[63:76)   IdentifierSyntax
//@[63:76)    Identifier |stringOutputB|
//@[76:77) NewLine |\n|
var moduleRuntimeCheck4 = moduleRuntimeCheck3
//@[0:45) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:23)  IdentifierSyntax
//@[4:23)   Identifier |moduleRuntimeCheck4|
//@[24:25)  Assignment |=|
//@[26:45)  VariableAccessSyntax
//@[26:45)   IdentifierSyntax
//@[26:45)    Identifier |moduleRuntimeCheck3|
//@[45:46) NewLine |\n|
module moduleLoopForRuntimeCheck2 'modulea.bicep' = [for thing in []: {
//@[0:102) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:33)  IdentifierSyntax
//@[7:33)   Identifier |moduleLoopForRuntimeCheck2|
//@[34:49)  StringSyntax
//@[34:49)   StringComplete |'modulea.bicep'|
//@[50:51)  Assignment |=|
//@[52:102)  ForSyntax
//@[52:53)   LeftSquare |[|
//@[53:56)   Identifier |for|
//@[57:62)   LocalVariableSyntax
//@[57:62)    IdentifierSyntax
//@[57:62)     Identifier |thing|
//@[63:65)   Identifier |in|
//@[66:68)   ArraySyntax
//@[66:67)    LeftSquare |[|
//@[67:68)    RightSquare |]|
//@[68:69)   Colon |:|
//@[70:101)   ObjectSyntax
//@[70:71)    LeftBrace |{|
//@[71:72)    NewLine |\n|
  name: moduleRuntimeCheck4
//@[2:27)    ObjectPropertySyntax
//@[2:6)     IdentifierSyntax
//@[2:6)      Identifier |name|
//@[6:7)     Colon |:|
//@[8:27)     VariableAccessSyntax
//@[8:27)      IdentifierSyntax
//@[8:27)       Identifier |moduleRuntimeCheck4|
//@[27:28)    NewLine |\n|
}]
//@[0:1)    RightBrace |}|
//@[1:2)   RightSquare |]|
//@[2:4) NewLine |\n\n|

module moduleLoopForRuntimeCheck3 'modulea.bicep' = [for thing in []: {
//@[0:194) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:33)  IdentifierSyntax
//@[7:33)   Identifier |moduleLoopForRuntimeCheck3|
//@[34:49)  StringSyntax
//@[34:49)   StringComplete |'modulea.bicep'|
//@[50:51)  Assignment |=|
//@[52:194)  ForSyntax
//@[52:53)   LeftSquare |[|
//@[53:56)   Identifier |for|
//@[57:62)   LocalVariableSyntax
//@[57:62)    IdentifierSyntax
//@[57:62)     Identifier |thing|
//@[63:65)   Identifier |in|
//@[66:68)   ArraySyntax
//@[66:67)    LeftSquare |[|
//@[67:68)    RightSquare |]|
//@[68:69)   Colon |:|
//@[70:193)   ObjectSyntax
//@[70:71)    LeftBrace |{|
//@[71:72)    NewLine |\n|
  name: concat(moduleLoopForRuntimeCheck[1].outputs.stringOutputB, moduleLoopForRuntimeCheck[1].outputs.stringOutputA )
//@[2:119)    ObjectPropertySyntax
//@[2:6)     IdentifierSyntax
//@[2:6)      Identifier |name|
//@[6:7)     Colon |:|
//@[8:119)     FunctionCallSyntax
//@[8:14)      IdentifierSyntax
//@[8:14)       Identifier |concat|
//@[14:15)      LeftParen |(|
//@[15:66)      FunctionArgumentSyntax
//@[15:65)       PropertyAccessSyntax
//@[15:51)        PropertyAccessSyntax
//@[15:43)         ArrayAccessSyntax
//@[15:40)          VariableAccessSyntax
//@[15:40)           IdentifierSyntax
//@[15:40)            Identifier |moduleLoopForRuntimeCheck|
//@[40:41)          LeftSquare |[|
//@[41:42)          IntegerLiteralSyntax
//@[41:42)           Integer |1|
//@[42:43)          RightSquare |]|
//@[43:44)         Dot |.|
//@[44:51)         IdentifierSyntax
//@[44:51)          Identifier |outputs|
//@[51:52)        Dot |.|
//@[52:65)        IdentifierSyntax
//@[52:65)         Identifier |stringOutputB|
//@[65:66)       Comma |,|
//@[67:117)      FunctionArgumentSyntax
//@[67:117)       PropertyAccessSyntax
//@[67:103)        PropertyAccessSyntax
//@[67:95)         ArrayAccessSyntax
//@[67:92)          VariableAccessSyntax
//@[67:92)           IdentifierSyntax
//@[67:92)            Identifier |moduleLoopForRuntimeCheck|
//@[92:93)          LeftSquare |[|
//@[93:94)          IntegerLiteralSyntax
//@[93:94)           Integer |1|
//@[94:95)          RightSquare |]|
//@[95:96)         Dot |.|
//@[96:103)         IdentifierSyntax
//@[96:103)          Identifier |outputs|
//@[103:104)        Dot |.|
//@[104:117)        IdentifierSyntax
//@[104:117)         Identifier |stringOutputA|
//@[118:119)      RightParen |)|
//@[119:120)    NewLine |\n|
}]
//@[0:1)    RightBrace |}|
//@[1:2)   RightSquare |]|
//@[2:4) NewLine |\n\n|

module moduleWithDuplicateName1 './empty.bicep' = {
//@[0:112) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:31)  IdentifierSyntax
//@[7:31)   Identifier |moduleWithDuplicateName1|
//@[32:47)  StringSyntax
//@[32:47)   StringComplete |'./empty.bicep'|
//@[48:49)  Assignment |=|
//@[50:112)  ObjectSyntax
//@[50:51)   LeftBrace |{|
//@[51:52)   NewLine |\n|
  name: 'moduleWithDuplicateName'
//@[2:33)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:33)    StringSyntax
//@[8:33)     StringComplete |'moduleWithDuplicateName'|
//@[33:34)   NewLine |\n|
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
//@[24:25)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

module moduleWithDuplicateName2 './empty.bicep' = {
//@[0:87) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:31)  IdentifierSyntax
//@[7:31)   Identifier |moduleWithDuplicateName2|
//@[32:47)  StringSyntax
//@[32:47)   StringComplete |'./empty.bicep'|
//@[48:49)  Assignment |=|
//@[50:87)  ObjectSyntax
//@[50:51)   LeftBrace |{|
//@[51:52)   NewLine |\n|
  name: 'moduleWithDuplicateName'
//@[2:33)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:33)    StringSyntax
//@[8:33)     StringComplete |'moduleWithDuplicateName'|
//@[33:34)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

// #completionTest(19, 20, 21) -> cwdCompletions
//@[48:49) NewLine |\n|
module completionB ''
//@[0:21) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:18)  IdentifierSyntax
//@[7:18)   Identifier |completionB|
//@[19:21)  StringSyntax
//@[19:21)   StringComplete |''|
//@[21:21)  SkippedTriviaSyntax
//@[21:21)  SkippedTriviaSyntax
//@[21:23) NewLine |\n\n|

// #completionTest(19, 20, 21) -> cwdCompletions
//@[48:49) NewLine |\n|
module completionC '' =
//@[0:23) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:18)  IdentifierSyntax
//@[7:18)   Identifier |completionC|
//@[19:21)  StringSyntax
//@[19:21)   StringComplete |''|
//@[22:23)  Assignment |=|
//@[23:23)  SkippedTriviaSyntax
//@[23:25) NewLine |\n\n|

// #completionTest(19, 20, 21) -> cwdCompletions
//@[48:49) NewLine |\n|
module completionD '' = {}
//@[0:26) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:18)  IdentifierSyntax
//@[7:18)   Identifier |completionD|
//@[19:21)  StringSyntax
//@[19:21)   StringComplete |''|
//@[22:23)  Assignment |=|
//@[24:26)  ObjectSyntax
//@[24:25)   LeftBrace |{|
//@[25:26)   RightBrace |}|
//@[26:28) NewLine |\n\n|

// #completionTest(19, 20, 21) -> cwdCompletions
//@[48:49) NewLine |\n|
module completionE '' = {
//@[0:43) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:18)  IdentifierSyntax
//@[7:18)   Identifier |completionE|
//@[19:21)  StringSyntax
//@[19:21)   StringComplete |''|
//@[22:23)  Assignment |=|
//@[24:43)  ObjectSyntax
//@[24:25)   LeftBrace |{|
//@[25:26)   NewLine |\n|
  name: 'hello'
//@[2:15)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:15)    StringSyntax
//@[8:15)     StringComplete |'hello'|
//@[15:16)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

// #completionTest(26, 27, 28, 29) -> cwdFileCompletions
//@[56:57) NewLine |\n|
module cwdFileCompletionA '.'
//@[0:29) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:25)  IdentifierSyntax
//@[7:25)   Identifier |cwdFileCompletionA|
//@[26:29)  StringSyntax
//@[26:29)   StringComplete |'.'|
//@[29:29)  SkippedTriviaSyntax
//@[29:29)  SkippedTriviaSyntax
//@[29:31) NewLine |\n\n|

// #completionTest(26, 27) -> cwdMCompletions
//@[45:46) NewLine |\n|
module cwdFileCompletionB m
//@[0:27) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:25)  IdentifierSyntax
//@[7:25)   Identifier |cwdFileCompletionB|
//@[26:27)  SkippedTriviaSyntax
//@[26:27)   Identifier |m|
//@[27:27)  SkippedTriviaSyntax
//@[27:27)  SkippedTriviaSyntax
//@[27:29) NewLine |\n\n|

// #completionTest(26, 27, 28, 29) -> cwdMCompletions
//@[53:54) NewLine |\n|
module cwdFileCompletionC 'm'
//@[0:29) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:25)  IdentifierSyntax
//@[7:25)   Identifier |cwdFileCompletionC|
//@[26:29)  StringSyntax
//@[26:29)   StringComplete |'m'|
//@[29:29)  SkippedTriviaSyntax
//@[29:29)  SkippedTriviaSyntax
//@[29:31) NewLine |\n\n|

// #completionTest(24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39) -> childCompletions
//@[102:103) NewLine |\n|
module childCompletionA 'ChildModules/'
//@[0:39) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:23)  IdentifierSyntax
//@[7:23)   Identifier |childCompletionA|
//@[24:39)  StringSyntax
//@[24:39)   StringComplete |'ChildModules/'|
//@[39:39)  SkippedTriviaSyntax
//@[39:39)  SkippedTriviaSyntax
//@[39:41) NewLine |\n\n|

// #completionTest(24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39) -> childDotCompletions
//@[105:106) NewLine |\n|
module childCompletionB './ChildModules/'
//@[0:41) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:23)  IdentifierSyntax
//@[7:23)   Identifier |childCompletionB|
//@[24:41)  StringSyntax
//@[24:41)   StringComplete |'./ChildModules/'|
//@[41:41)  SkippedTriviaSyntax
//@[41:41)  SkippedTriviaSyntax
//@[41:43) NewLine |\n\n|

// #completionTest(24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40) -> childMCompletions
//@[107:108) NewLine |\n|
module childCompletionC './ChildModules/m'
//@[0:42) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:23)  IdentifierSyntax
//@[7:23)   Identifier |childCompletionC|
//@[24:42)  StringSyntax
//@[24:42)   StringComplete |'./ChildModules/m'|
//@[42:42)  SkippedTriviaSyntax
//@[42:42)  SkippedTriviaSyntax
//@[42:44) NewLine |\n\n|

// #completionTest(24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40) -> childECompletions
//@[107:108) NewLine |\n|
module childCompletionD 'ChildModules/e'
//@[0:40) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:23)  IdentifierSyntax
//@[7:23)   Identifier |childCompletionD|
//@[24:40)  StringSyntax
//@[24:40)   StringComplete |'ChildModules/e'|
//@[40:40)  SkippedTriviaSyntax
//@[40:40)  SkippedTriviaSyntax
//@[40:42) NewLine |\n\n|

@minValue()
//@[0:118) ModuleDeclarationSyntax
//@[0:11)  DecoratorSyntax
//@[0:1)   At |@|
//@[1:11)   FunctionCallSyntax
//@[1:9)    IdentifierSyntax
//@[1:9)     Identifier |minValue|
//@[9:10)    LeftParen |(|
//@[10:11)    RightParen |)|
//@[11:12)  NewLine |\n|
module moduleWithNotAttachableDecorators './empty.bicep' = {
//@[0:6)  Identifier |module|
//@[7:40)  IdentifierSyntax
//@[7:40)   Identifier |moduleWithNotAttachableDecorators|
//@[41:56)  StringSyntax
//@[41:56)   StringComplete |'./empty.bicep'|
//@[57:58)  Assignment |=|
//@[59:106)  ObjectSyntax
//@[59:60)   LeftBrace |{|
//@[60:61)   NewLine |\n|
  name: 'moduleWithNotAttachableDecorators'
//@[2:43)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:43)    StringSyntax
//@[8:43)     StringComplete |'moduleWithNotAttachableDecorators'|
//@[43:44)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

// loop parsing cases
//@[21:22) NewLine |\n|
module expectedForKeyword 'modulea.bicep' = []
//@[0:46) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:25)  IdentifierSyntax
//@[7:25)   Identifier |expectedForKeyword|
//@[26:41)  StringSyntax
//@[26:41)   StringComplete |'modulea.bicep'|
//@[42:43)  Assignment |=|
//@[44:46)  SkippedTriviaSyntax
//@[44:45)   LeftSquare |[|
//@[45:46)   RightSquare |]|
//@[46:48) NewLine |\n\n|

module expectedForKeyword2 'modulea.bicep' = [f]
//@[0:48) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:26)  IdentifierSyntax
//@[7:26)   Identifier |expectedForKeyword2|
//@[27:42)  StringSyntax
//@[27:42)   StringComplete |'modulea.bicep'|
//@[43:44)  Assignment |=|
//@[45:48)  SkippedTriviaSyntax
//@[45:46)   LeftSquare |[|
//@[46:47)   Identifier |f|
//@[47:48)   RightSquare |]|
//@[48:50) NewLine |\n\n|

module expectedLoopVar 'modulea.bicep' = [for]
//@[0:46) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:22)  IdentifierSyntax
//@[7:22)   Identifier |expectedLoopVar|
//@[23:38)  StringSyntax
//@[23:38)   StringComplete |'modulea.bicep'|
//@[39:40)  Assignment |=|
//@[41:46)  ForSyntax
//@[41:42)   LeftSquare |[|
//@[42:45)   Identifier |for|
//@[45:45)   SkippedTriviaSyntax
//@[45:45)   SkippedTriviaSyntax
//@[45:45)   SkippedTriviaSyntax
//@[45:45)   SkippedTriviaSyntax
//@[45:45)   SkippedTriviaSyntax
//@[45:46)   RightSquare |]|
//@[46:48) NewLine |\n\n|

module expectedInKeyword 'modulea.bicep' = [for x]
//@[0:50) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:24)  IdentifierSyntax
//@[7:24)   Identifier |expectedInKeyword|
//@[25:40)  StringSyntax
//@[25:40)   StringComplete |'modulea.bicep'|
//@[41:42)  Assignment |=|
//@[43:50)  ForSyntax
//@[43:44)   LeftSquare |[|
//@[44:47)   Identifier |for|
//@[48:49)   LocalVariableSyntax
//@[48:49)    IdentifierSyntax
//@[48:49)     Identifier |x|
//@[49:49)   SkippedTriviaSyntax
//@[49:49)   SkippedTriviaSyntax
//@[49:49)   SkippedTriviaSyntax
//@[49:49)   SkippedTriviaSyntax
//@[49:50)   RightSquare |]|
//@[50:52) NewLine |\n\n|

module expectedInKeyword2 'modulea.bicep' = [for x b]
//@[0:53) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:25)  IdentifierSyntax
//@[7:25)   Identifier |expectedInKeyword2|
//@[26:41)  StringSyntax
//@[26:41)   StringComplete |'modulea.bicep'|
//@[42:43)  Assignment |=|
//@[44:53)  ForSyntax
//@[44:45)   LeftSquare |[|
//@[45:48)   Identifier |for|
//@[49:50)   LocalVariableSyntax
//@[49:50)    IdentifierSyntax
//@[49:50)     Identifier |x|
//@[51:52)   SkippedTriviaSyntax
//@[51:52)    Identifier |b|
//@[52:52)   SkippedTriviaSyntax
//@[52:52)   SkippedTriviaSyntax
//@[52:52)   SkippedTriviaSyntax
//@[52:53)   RightSquare |]|
//@[53:55) NewLine |\n\n|

module expectedArrayExpression 'modulea.bicep' = [for x in]
//@[0:59) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:30)  IdentifierSyntax
//@[7:30)   Identifier |expectedArrayExpression|
//@[31:46)  StringSyntax
//@[31:46)   StringComplete |'modulea.bicep'|
//@[47:48)  Assignment |=|
//@[49:59)  ForSyntax
//@[49:50)   LeftSquare |[|
//@[50:53)   Identifier |for|
//@[54:55)   LocalVariableSyntax
//@[54:55)    IdentifierSyntax
//@[54:55)     Identifier |x|
//@[56:58)   Identifier |in|
//@[58:58)   SkippedTriviaSyntax
//@[58:58)   SkippedTriviaSyntax
//@[58:58)   SkippedTriviaSyntax
//@[58:59)   RightSquare |]|
//@[59:61) NewLine |\n\n|

module expectedColon 'modulea.bicep' = [for x in y]
//@[0:51) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:20)  IdentifierSyntax
//@[7:20)   Identifier |expectedColon|
//@[21:36)  StringSyntax
//@[21:36)   StringComplete |'modulea.bicep'|
//@[37:38)  Assignment |=|
//@[39:51)  ForSyntax
//@[39:40)   LeftSquare |[|
//@[40:43)   Identifier |for|
//@[44:45)   LocalVariableSyntax
//@[44:45)    IdentifierSyntax
//@[44:45)     Identifier |x|
//@[46:48)   Identifier |in|
//@[49:50)   VariableAccessSyntax
//@[49:50)    IdentifierSyntax
//@[49:50)     Identifier |y|
//@[50:50)   SkippedTriviaSyntax
//@[50:50)   SkippedTriviaSyntax
//@[50:51)   RightSquare |]|
//@[51:53) NewLine |\n\n|

module expectedLoopBody 'modulea.bicep' = [for x in y:]
//@[0:55) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:23)  IdentifierSyntax
//@[7:23)   Identifier |expectedLoopBody|
//@[24:39)  StringSyntax
//@[24:39)   StringComplete |'modulea.bicep'|
//@[40:41)  Assignment |=|
//@[42:55)  ForSyntax
//@[42:43)   LeftSquare |[|
//@[43:46)   Identifier |for|
//@[47:48)   LocalVariableSyntax
//@[47:48)    IdentifierSyntax
//@[47:48)     Identifier |x|
//@[49:51)   Identifier |in|
//@[52:53)   VariableAccessSyntax
//@[52:53)    IdentifierSyntax
//@[52:53)     Identifier |y|
//@[53:54)   Colon |:|
//@[54:54)   SkippedTriviaSyntax
//@[54:55)   RightSquare |]|
//@[55:57) NewLine |\n\n|

// indexed loop parsing cases
//@[29:30) NewLine |\n|
module expectedItemVarName 'modulea.bicep' = [for ()]
//@[0:53) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:26)  IdentifierSyntax
//@[7:26)   Identifier |expectedItemVarName|
//@[27:42)  StringSyntax
//@[27:42)   StringComplete |'modulea.bicep'|
//@[43:44)  Assignment |=|
//@[45:53)  ForSyntax
//@[45:46)   LeftSquare |[|
//@[46:49)   Identifier |for|
//@[50:52)   ForVariableBlockSyntax
//@[50:51)    LeftParen |(|
//@[51:51)    LocalVariableSyntax
//@[51:51)     IdentifierSyntax
//@[51:51)      SkippedTriviaSyntax
//@[51:51)    SkippedTriviaSyntax
//@[51:51)    LocalVariableSyntax
//@[51:51)     IdentifierSyntax
//@[51:51)      SkippedTriviaSyntax
//@[51:52)    RightParen |)|
//@[52:52)   SkippedTriviaSyntax
//@[52:52)   SkippedTriviaSyntax
//@[52:52)   SkippedTriviaSyntax
//@[52:52)   SkippedTriviaSyntax
//@[52:53)   RightSquare |]|
//@[53:55) NewLine |\n\n|

module expectedComma 'modulea.bicep' = [for (x)]
//@[0:48) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:20)  IdentifierSyntax
//@[7:20)   Identifier |expectedComma|
//@[21:36)  StringSyntax
//@[21:36)   StringComplete |'modulea.bicep'|
//@[37:38)  Assignment |=|
//@[39:48)  ForSyntax
//@[39:40)   LeftSquare |[|
//@[40:43)   Identifier |for|
//@[44:47)   ForVariableBlockSyntax
//@[44:45)    LeftParen |(|
//@[45:46)    LocalVariableSyntax
//@[45:46)     IdentifierSyntax
//@[45:46)      Identifier |x|
//@[46:46)    SkippedTriviaSyntax
//@[46:46)    LocalVariableSyntax
//@[46:46)     IdentifierSyntax
//@[46:46)      SkippedTriviaSyntax
//@[46:47)    RightParen |)|
//@[47:47)   SkippedTriviaSyntax
//@[47:47)   SkippedTriviaSyntax
//@[47:47)   SkippedTriviaSyntax
//@[47:47)   SkippedTriviaSyntax
//@[47:48)   RightSquare |]|
//@[48:50) NewLine |\n\n|

module expectedIndexVarName 'modulea.bicep' = [for (x,)]
//@[0:56) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:27)  IdentifierSyntax
//@[7:27)   Identifier |expectedIndexVarName|
//@[28:43)  StringSyntax
//@[28:43)   StringComplete |'modulea.bicep'|
//@[44:45)  Assignment |=|
//@[46:56)  ForSyntax
//@[46:47)   LeftSquare |[|
//@[47:50)   Identifier |for|
//@[51:55)   ForVariableBlockSyntax
//@[51:52)    LeftParen |(|
//@[52:53)    LocalVariableSyntax
//@[52:53)     IdentifierSyntax
//@[52:53)      Identifier |x|
//@[53:54)    Comma |,|
//@[54:54)    LocalVariableSyntax
//@[54:54)     IdentifierSyntax
//@[54:54)      SkippedTriviaSyntax
//@[54:55)    RightParen |)|
//@[55:55)   SkippedTriviaSyntax
//@[55:55)   SkippedTriviaSyntax
//@[55:55)   SkippedTriviaSyntax
//@[55:55)   SkippedTriviaSyntax
//@[55:56)   RightSquare |]|
//@[56:58) NewLine |\n\n|

module expectedInKeyword3 'modulea.bicep' = [for (x,y)]
//@[0:55) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:25)  IdentifierSyntax
//@[7:25)   Identifier |expectedInKeyword3|
//@[26:41)  StringSyntax
//@[26:41)   StringComplete |'modulea.bicep'|
//@[42:43)  Assignment |=|
//@[44:55)  ForSyntax
//@[44:45)   LeftSquare |[|
//@[45:48)   Identifier |for|
//@[49:54)   ForVariableBlockSyntax
//@[49:50)    LeftParen |(|
//@[50:51)    LocalVariableSyntax
//@[50:51)     IdentifierSyntax
//@[50:51)      Identifier |x|
//@[51:52)    Comma |,|
//@[52:53)    LocalVariableSyntax
//@[52:53)     IdentifierSyntax
//@[52:53)      Identifier |y|
//@[53:54)    RightParen |)|
//@[54:54)   SkippedTriviaSyntax
//@[54:54)   SkippedTriviaSyntax
//@[54:54)   SkippedTriviaSyntax
//@[54:54)   SkippedTriviaSyntax
//@[54:55)   RightSquare |]|
//@[55:57) NewLine |\n\n|

module expectedArrayExpression2 'modulea.bicep' = [for (x,y) in ]
//@[0:65) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:31)  IdentifierSyntax
//@[7:31)   Identifier |expectedArrayExpression2|
//@[32:47)  StringSyntax
//@[32:47)   StringComplete |'modulea.bicep'|
//@[48:49)  Assignment |=|
//@[50:65)  ForSyntax
//@[50:51)   LeftSquare |[|
//@[51:54)   Identifier |for|
//@[55:60)   ForVariableBlockSyntax
//@[55:56)    LeftParen |(|
//@[56:57)    LocalVariableSyntax
//@[56:57)     IdentifierSyntax
//@[56:57)      Identifier |x|
//@[57:58)    Comma |,|
//@[58:59)    LocalVariableSyntax
//@[58:59)     IdentifierSyntax
//@[58:59)      Identifier |y|
//@[59:60)    RightParen |)|
//@[61:63)   Identifier |in|
//@[64:64)   SkippedTriviaSyntax
//@[64:64)   SkippedTriviaSyntax
//@[64:64)   SkippedTriviaSyntax
//@[64:65)   RightSquare |]|
//@[65:67) NewLine |\n\n|

module expectedColon2 'modulea.bicep' = [for (x,y) in z]
//@[0:56) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:21)  IdentifierSyntax
//@[7:21)   Identifier |expectedColon2|
//@[22:37)  StringSyntax
//@[22:37)   StringComplete |'modulea.bicep'|
//@[38:39)  Assignment |=|
//@[40:56)  ForSyntax
//@[40:41)   LeftSquare |[|
//@[41:44)   Identifier |for|
//@[45:50)   ForVariableBlockSyntax
//@[45:46)    LeftParen |(|
//@[46:47)    LocalVariableSyntax
//@[46:47)     IdentifierSyntax
//@[46:47)      Identifier |x|
//@[47:48)    Comma |,|
//@[48:49)    LocalVariableSyntax
//@[48:49)     IdentifierSyntax
//@[48:49)      Identifier |y|
//@[49:50)    RightParen |)|
//@[51:53)   Identifier |in|
//@[54:55)   VariableAccessSyntax
//@[54:55)    IdentifierSyntax
//@[54:55)     Identifier |z|
//@[55:55)   SkippedTriviaSyntax
//@[55:55)   SkippedTriviaSyntax
//@[55:56)   RightSquare |]|
//@[56:58) NewLine |\n\n|

module expectedLoopBody2 'modulea.bicep' = [for (x,y) in z:]
//@[0:60) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:24)  IdentifierSyntax
//@[7:24)   Identifier |expectedLoopBody2|
//@[25:40)  StringSyntax
//@[25:40)   StringComplete |'modulea.bicep'|
//@[41:42)  Assignment |=|
//@[43:60)  ForSyntax
//@[43:44)   LeftSquare |[|
//@[44:47)   Identifier |for|
//@[48:53)   ForVariableBlockSyntax
//@[48:49)    LeftParen |(|
//@[49:50)    LocalVariableSyntax
//@[49:50)     IdentifierSyntax
//@[49:50)      Identifier |x|
//@[50:51)    Comma |,|
//@[51:52)    LocalVariableSyntax
//@[51:52)     IdentifierSyntax
//@[51:52)      Identifier |y|
//@[52:53)    RightParen |)|
//@[54:56)   Identifier |in|
//@[57:58)   VariableAccessSyntax
//@[57:58)    IdentifierSyntax
//@[57:58)     Identifier |z|
//@[58:59)   Colon |:|
//@[59:59)   SkippedTriviaSyntax
//@[59:60)   RightSquare |]|
//@[60:62) NewLine |\n\n|

// wrong loop body type
//@[23:24) NewLine |\n|
var emptyArray = []
//@[0:19) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:14)  IdentifierSyntax
//@[4:14)   Identifier |emptyArray|
//@[15:16)  Assignment |=|
//@[17:19)  ArraySyntax
//@[17:18)   LeftSquare |[|
//@[18:19)   RightSquare |]|
//@[19:20) NewLine |\n|
module wrongLoopBodyType 'modulea.bicep' = [for x in emptyArray:4]
//@[0:66) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:24)  IdentifierSyntax
//@[7:24)   Identifier |wrongLoopBodyType|
//@[25:40)  StringSyntax
//@[25:40)   StringComplete |'modulea.bicep'|
//@[41:42)  Assignment |=|
//@[43:66)  ForSyntax
//@[43:44)   LeftSquare |[|
//@[44:47)   Identifier |for|
//@[48:49)   LocalVariableSyntax
//@[48:49)    IdentifierSyntax
//@[48:49)     Identifier |x|
//@[50:52)   Identifier |in|
//@[53:63)   VariableAccessSyntax
//@[53:63)    IdentifierSyntax
//@[53:63)     Identifier |emptyArray|
//@[63:64)   Colon |:|
//@[64:65)   SkippedTriviaSyntax
//@[64:65)    Integer |4|
//@[65:66)   RightSquare |]|
//@[66:67) NewLine |\n|
module wrongLoopBodyType2 'modulea.bicep' = [for (x,i) in emptyArray:4]
//@[0:71) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:25)  IdentifierSyntax
//@[7:25)   Identifier |wrongLoopBodyType2|
//@[26:41)  StringSyntax
//@[26:41)   StringComplete |'modulea.bicep'|
//@[42:43)  Assignment |=|
//@[44:71)  ForSyntax
//@[44:45)   LeftSquare |[|
//@[45:48)   Identifier |for|
//@[49:54)   ForVariableBlockSyntax
//@[49:50)    LeftParen |(|
//@[50:51)    LocalVariableSyntax
//@[50:51)     IdentifierSyntax
//@[50:51)      Identifier |x|
//@[51:52)    Comma |,|
//@[52:53)    LocalVariableSyntax
//@[52:53)     IdentifierSyntax
//@[52:53)      Identifier |i|
//@[53:54)    RightParen |)|
//@[55:57)   Identifier |in|
//@[58:68)   VariableAccessSyntax
//@[58:68)    IdentifierSyntax
//@[58:68)     Identifier |emptyArray|
//@[68:69)   Colon |:|
//@[69:70)   SkippedTriviaSyntax
//@[69:70)    Integer |4|
//@[70:71)   RightSquare |]|
//@[71:73) NewLine |\n\n|

// missing loop body properties
//@[31:32) NewLine |\n|
module missingLoopBodyProperties 'modulea.bicep' = [for x in emptyArray:{
//@[0:76) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:32)  IdentifierSyntax
//@[7:32)   Identifier |missingLoopBodyProperties|
//@[33:48)  StringSyntax
//@[33:48)   StringComplete |'modulea.bicep'|
//@[49:50)  Assignment |=|
//@[51:76)  ForSyntax
//@[51:52)   LeftSquare |[|
//@[52:55)   Identifier |for|
//@[56:57)   LocalVariableSyntax
//@[56:57)    IdentifierSyntax
//@[56:57)     Identifier |x|
//@[58:60)   Identifier |in|
//@[61:71)   VariableAccessSyntax
//@[61:71)    IdentifierSyntax
//@[61:71)     Identifier |emptyArray|
//@[71:72)   Colon |:|
//@[72:75)   ObjectSyntax
//@[72:73)    LeftBrace |{|
//@[73:74)    NewLine |\n|
}]
//@[0:1)    RightBrace |}|
//@[1:2)   RightSquare |]|
//@[2:3) NewLine |\n|
module missingLoopBodyProperties2 'modulea.bicep' = [for (x,i) in emptyArray:{
//@[0:81) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:33)  IdentifierSyntax
//@[7:33)   Identifier |missingLoopBodyProperties2|
//@[34:49)  StringSyntax
//@[34:49)   StringComplete |'modulea.bicep'|
//@[50:51)  Assignment |=|
//@[52:81)  ForSyntax
//@[52:53)   LeftSquare |[|
//@[53:56)   Identifier |for|
//@[57:62)   ForVariableBlockSyntax
//@[57:58)    LeftParen |(|
//@[58:59)    LocalVariableSyntax
//@[58:59)     IdentifierSyntax
//@[58:59)      Identifier |x|
//@[59:60)    Comma |,|
//@[60:61)    LocalVariableSyntax
//@[60:61)     IdentifierSyntax
//@[60:61)      Identifier |i|
//@[61:62)    RightParen |)|
//@[63:65)   Identifier |in|
//@[66:76)   VariableAccessSyntax
//@[66:76)    IdentifierSyntax
//@[66:76)     Identifier |emptyArray|
//@[76:77)   Colon |:|
//@[77:80)   ObjectSyntax
//@[77:78)    LeftBrace |{|
//@[78:79)    NewLine |\n|
}]
//@[0:1)    RightBrace |}|
//@[1:2)   RightSquare |]|
//@[2:4) NewLine |\n\n|

// wrong array type
//@[19:20) NewLine |\n|
var notAnArray = true
//@[0:21) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:14)  IdentifierSyntax
//@[4:14)   Identifier |notAnArray|
//@[15:16)  Assignment |=|
//@[17:21)  BooleanLiteralSyntax
//@[17:21)   TrueKeyword |true|
//@[21:22) NewLine |\n|
module wrongArrayType 'modulea.bicep' = [for x in notAnArray:{
//@[0:65) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:21)  IdentifierSyntax
//@[7:21)   Identifier |wrongArrayType|
//@[22:37)  StringSyntax
//@[22:37)   StringComplete |'modulea.bicep'|
//@[38:39)  Assignment |=|
//@[40:65)  ForSyntax
//@[40:41)   LeftSquare |[|
//@[41:44)   Identifier |for|
//@[45:46)   LocalVariableSyntax
//@[45:46)    IdentifierSyntax
//@[45:46)     Identifier |x|
//@[47:49)   Identifier |in|
//@[50:60)   VariableAccessSyntax
//@[50:60)    IdentifierSyntax
//@[50:60)     Identifier |notAnArray|
//@[60:61)   Colon |:|
//@[61:64)   ObjectSyntax
//@[61:62)    LeftBrace |{|
//@[62:63)    NewLine |\n|
}]
//@[0:1)    RightBrace |}|
//@[1:2)   RightSquare |]|
//@[2:4) NewLine |\n\n|

// missing fewer properties
//@[27:28) NewLine |\n|
module missingFewerLoopBodyProperties 'modulea.bicep' = [for x in emptyArray:{
//@[0:119) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:37)  IdentifierSyntax
//@[7:37)   Identifier |missingFewerLoopBodyProperties|
//@[38:53)  StringSyntax
//@[38:53)   StringComplete |'modulea.bicep'|
//@[54:55)  Assignment |=|
//@[56:119)  ForSyntax
//@[56:57)   LeftSquare |[|
//@[57:60)   Identifier |for|
//@[61:62)   LocalVariableSyntax
//@[61:62)    IdentifierSyntax
//@[61:62)     Identifier |x|
//@[63:65)   Identifier |in|
//@[66:76)   VariableAccessSyntax
//@[66:76)    IdentifierSyntax
//@[66:76)     Identifier |emptyArray|
//@[76:77)   Colon |:|
//@[77:118)   ObjectSyntax
//@[77:78)    LeftBrace |{|
//@[78:79)    NewLine |\n|
  name: 'hello-${x}'
//@[2:20)    ObjectPropertySyntax
//@[2:6)     IdentifierSyntax
//@[2:6)      Identifier |name|
//@[6:7)     Colon |:|
//@[8:20)     StringSyntax
//@[8:17)      StringLeftPiece |'hello-${|
//@[17:18)      VariableAccessSyntax
//@[17:18)       IdentifierSyntax
//@[17:18)        Identifier |x|
//@[18:20)      StringRightPiece |}'|
//@[20:21)    NewLine |\n|
  params: {
//@[2:16)    ObjectPropertySyntax
//@[2:8)     IdentifierSyntax
//@[2:8)      Identifier |params|
//@[8:9)     Colon |:|
//@[10:16)     ObjectSyntax
//@[10:11)      LeftBrace |{|
//@[11:13)      NewLine |\n\n|

  }
//@[2:3)      RightBrace |}|
//@[3:4)    NewLine |\n|
}]
//@[0:1)    RightBrace |}|
//@[1:2)   RightSquare |]|
//@[2:4) NewLine |\n\n|

// wrong parameter in the module loop
//@[37:38) NewLine |\n|
module wrongModuleParameterInLoop 'modulea.bicep' = [for x in emptyArray:{
//@[0:263) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:33)  IdentifierSyntax
//@[7:33)   Identifier |wrongModuleParameterInLoop|
//@[34:49)  StringSyntax
//@[34:49)   StringComplete |'modulea.bicep'|
//@[50:51)  Assignment |=|
//@[52:263)  ForSyntax
//@[52:53)   LeftSquare |[|
//@[53:56)   Identifier |for|
//@[57:58)   LocalVariableSyntax
//@[57:58)    IdentifierSyntax
//@[57:58)     Identifier |x|
//@[59:61)   Identifier |in|
//@[62:72)   VariableAccessSyntax
//@[62:72)    IdentifierSyntax
//@[62:72)     Identifier |emptyArray|
//@[72:73)   Colon |:|
//@[73:262)   ObjectSyntax
//@[73:74)    LeftBrace |{|
//@[74:75)    NewLine |\n|
  // #completionTest(17) -> symbolsPlusX
//@[40:41)    NewLine |\n|
  name: 'hello-${x}'
//@[2:20)    ObjectPropertySyntax
//@[2:6)     IdentifierSyntax
//@[2:6)      Identifier |name|
//@[6:7)     Colon |:|
//@[8:20)     StringSyntax
//@[8:17)      StringLeftPiece |'hello-${|
//@[17:18)      VariableAccessSyntax
//@[17:18)       IdentifierSyntax
//@[17:18)        Identifier |x|
//@[18:20)      StringRightPiece |}'|
//@[20:21)    NewLine |\n|
  params: {
//@[2:123)    ObjectPropertySyntax
//@[2:8)     IdentifierSyntax
//@[2:8)      Identifier |params|
//@[8:9)     Colon |:|
//@[10:123)     ObjectSyntax
//@[10:11)      LeftBrace |{|
//@[11:12)      NewLine |\n|
    arrayParam: []
//@[4:18)      ObjectPropertySyntax
//@[4:14)       IdentifierSyntax
//@[4:14)        Identifier |arrayParam|
//@[14:15)       Colon |:|
//@[16:18)       ArraySyntax
//@[16:17)        LeftSquare |[|
//@[17:18)        RightSquare |]|
//@[18:19)      NewLine |\n|
    objParam: {}
//@[4:16)      ObjectPropertySyntax
//@[4:12)       IdentifierSyntax
//@[4:12)        Identifier |objParam|
//@[12:13)       Colon |:|
//@[14:16)       ObjectSyntax
//@[14:15)        LeftBrace |{|
//@[15:16)        RightBrace |}|
//@[16:17)      NewLine |\n|
    stringParamA: 'test'
//@[4:24)      ObjectPropertySyntax
//@[4:16)       IdentifierSyntax
//@[4:16)        Identifier |stringParamA|
//@[16:17)       Colon |:|
//@[18:24)       StringSyntax
//@[18:24)        StringComplete |'test'|
//@[24:25)      NewLine |\n|
    stringParamB: 'test'
//@[4:24)      ObjectPropertySyntax
//@[4:16)       IdentifierSyntax
//@[4:16)        Identifier |stringParamB|
//@[16:17)       Colon |:|
//@[18:24)       StringSyntax
//@[18:24)        StringComplete |'test'|
//@[24:25)      NewLine |\n|
    notAThing: 'test'
//@[4:21)      ObjectPropertySyntax
//@[4:13)       IdentifierSyntax
//@[4:13)        Identifier |notAThing|
//@[13:14)       Colon |:|
//@[15:21)       StringSyntax
//@[15:21)        StringComplete |'test'|
//@[21:22)      NewLine |\n|
  }
//@[2:3)      RightBrace |}|
//@[3:4)    NewLine |\n|
}]
//@[0:1)    RightBrace |}|
//@[1:2)   RightSquare |]|
//@[2:3) NewLine |\n|
module wrongModuleParameterInLoop2 'modulea.bicep' = [for (x,i) in emptyArray:{
//@[0:240) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:34)  IdentifierSyntax
//@[7:34)   Identifier |wrongModuleParameterInLoop2|
//@[35:50)  StringSyntax
//@[35:50)   StringComplete |'modulea.bicep'|
//@[51:52)  Assignment |=|
//@[53:240)  ForSyntax
//@[53:54)   LeftSquare |[|
//@[54:57)   Identifier |for|
//@[58:63)   ForVariableBlockSyntax
//@[58:59)    LeftParen |(|
//@[59:60)    LocalVariableSyntax
//@[59:60)     IdentifierSyntax
//@[59:60)      Identifier |x|
//@[60:61)    Comma |,|
//@[61:62)    LocalVariableSyntax
//@[61:62)     IdentifierSyntax
//@[61:62)      Identifier |i|
//@[62:63)    RightParen |)|
//@[64:66)   Identifier |in|
//@[67:77)   VariableAccessSyntax
//@[67:77)    IdentifierSyntax
//@[67:77)     Identifier |emptyArray|
//@[77:78)   Colon |:|
//@[78:239)   ObjectSyntax
//@[78:79)    LeftBrace |{|
//@[79:80)    NewLine |\n|
  name: 'hello-${x}'
//@[2:20)    ObjectPropertySyntax
//@[2:6)     IdentifierSyntax
//@[2:6)      Identifier |name|
//@[6:7)     Colon |:|
//@[8:20)     StringSyntax
//@[8:17)      StringLeftPiece |'hello-${|
//@[17:18)      VariableAccessSyntax
//@[17:18)       IdentifierSyntax
//@[17:18)        Identifier |x|
//@[18:20)      StringRightPiece |}'|
//@[20:21)    NewLine |\n|
  params: {
//@[2:136)    ObjectPropertySyntax
//@[2:8)     IdentifierSyntax
//@[2:8)      Identifier |params|
//@[8:9)     Colon |:|
//@[10:136)     ObjectSyntax
//@[10:11)      LeftBrace |{|
//@[11:12)      NewLine |\n|
    arrayParam: [
//@[4:31)      ObjectPropertySyntax
//@[4:14)       IdentifierSyntax
//@[4:14)        Identifier |arrayParam|
//@[14:15)       Colon |:|
//@[16:31)       ArraySyntax
//@[16:17)        LeftSquare |[|
//@[17:18)        NewLine |\n|
      i
//@[6:7)        ArrayItemSyntax
//@[6:7)         VariableAccessSyntax
//@[6:7)          IdentifierSyntax
//@[6:7)           Identifier |i|
//@[7:8)        NewLine |\n|
    ]
//@[4:5)        RightSquare |]|
//@[5:6)      NewLine |\n|
    objParam: {}
//@[4:16)      ObjectPropertySyntax
//@[4:12)       IdentifierSyntax
//@[4:12)        Identifier |objParam|
//@[12:13)       Colon |:|
//@[14:16)       ObjectSyntax
//@[14:15)        LeftBrace |{|
//@[15:16)        RightBrace |}|
//@[16:17)      NewLine |\n|
    stringParamA: 'test'
//@[4:24)      ObjectPropertySyntax
//@[4:16)       IdentifierSyntax
//@[4:16)        Identifier |stringParamA|
//@[16:17)       Colon |:|
//@[18:24)       StringSyntax
//@[18:24)        StringComplete |'test'|
//@[24:25)      NewLine |\n|
    stringParamB: 'test'
//@[4:24)      ObjectPropertySyntax
//@[4:16)       IdentifierSyntax
//@[4:16)        Identifier |stringParamB|
//@[16:17)       Colon |:|
//@[18:24)       StringSyntax
//@[18:24)        StringComplete |'test'|
//@[24:25)      NewLine |\n|
    notAThing: 'test'
//@[4:21)      ObjectPropertySyntax
//@[4:13)       IdentifierSyntax
//@[4:13)        Identifier |notAThing|
//@[13:14)       Colon |:|
//@[15:21)       StringSyntax
//@[15:21)        StringComplete |'test'|
//@[21:22)      NewLine |\n|
  }
//@[2:3)      RightBrace |}|
//@[3:4)    NewLine |\n|
}]
//@[0:1)    RightBrace |}|
//@[1:2)   RightSquare |]|
//@[2:4) NewLine |\n\n|

// nonexistent arrays and loop variables
//@[40:41) NewLine |\n|
var evenMoreDuplicates = 'there'
//@[0:32) VariableDeclarationSyntax
//@[0:3)  Identifier |var|
//@[4:22)  IdentifierSyntax
//@[4:22)   Identifier |evenMoreDuplicates|
//@[23:24)  Assignment |=|
//@[25:32)  StringSyntax
//@[25:32)   StringComplete |'there'|
//@[32:33) NewLine |\n|
module nonexistentArrays 'modulea.bicep' = [for evenMoreDuplicates in alsoDoesNotExist: {
//@[0:278) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:24)  IdentifierSyntax
//@[7:24)   Identifier |nonexistentArrays|
//@[25:40)  StringSyntax
//@[25:40)   StringComplete |'modulea.bicep'|
//@[41:42)  Assignment |=|
//@[43:278)  ForSyntax
//@[43:44)   LeftSquare |[|
//@[44:47)   Identifier |for|
//@[48:66)   LocalVariableSyntax
//@[48:66)    IdentifierSyntax
//@[48:66)     Identifier |evenMoreDuplicates|
//@[67:69)   Identifier |in|
//@[70:86)   VariableAccessSyntax
//@[70:86)    IdentifierSyntax
//@[70:86)     Identifier |alsoDoesNotExist|
//@[86:87)   Colon |:|
//@[88:277)   ObjectSyntax
//@[88:89)    LeftBrace |{|
//@[89:90)    NewLine |\n|
  name: 'hello-${whyChooseRealVariablesWhenWeCanPretend}'
//@[2:57)    ObjectPropertySyntax
//@[2:6)     IdentifierSyntax
//@[2:6)      Identifier |name|
//@[6:7)     Colon |:|
//@[8:57)     StringSyntax
//@[8:17)      StringLeftPiece |'hello-${|
//@[17:55)      VariableAccessSyntax
//@[17:55)       IdentifierSyntax
//@[17:55)        Identifier |whyChooseRealVariablesWhenWeCanPretend|
//@[55:57)      StringRightPiece |}'|
//@[57:58)    NewLine |\n|
  params: {
//@[2:127)    ObjectPropertySyntax
//@[2:8)     IdentifierSyntax
//@[2:8)      Identifier |params|
//@[8:9)     Colon |:|
//@[10:127)     ObjectSyntax
//@[10:11)      LeftBrace |{|
//@[11:12)      NewLine |\n|
    objParam: {}
//@[4:16)      ObjectPropertySyntax
//@[4:12)       IdentifierSyntax
//@[4:12)        Identifier |objParam|
//@[12:13)       Colon |:|
//@[14:16)       ObjectSyntax
//@[14:15)        LeftBrace |{|
//@[15:16)        RightBrace |}|
//@[16:17)      NewLine |\n|
    stringParamB: 'test'
//@[4:24)      ObjectPropertySyntax
//@[4:16)       IdentifierSyntax
//@[4:16)        Identifier |stringParamB|
//@[16:17)       Colon |:|
//@[18:24)       StringSyntax
//@[18:24)        StringComplete |'test'|
//@[24:25)      NewLine |\n|
    arrayParam: [for evenMoreDuplicates in totallyFake: doesNotExist]
//@[4:69)      ObjectPropertySyntax
//@[4:14)       IdentifierSyntax
//@[4:14)        Identifier |arrayParam|
//@[14:15)       Colon |:|
//@[16:69)       ForSyntax
//@[16:17)        LeftSquare |[|
//@[17:20)        Identifier |for|
//@[21:39)        LocalVariableSyntax
//@[21:39)         IdentifierSyntax
//@[21:39)          Identifier |evenMoreDuplicates|
//@[40:42)        Identifier |in|
//@[43:54)        VariableAccessSyntax
//@[43:54)         IdentifierSyntax
//@[43:54)          Identifier |totallyFake|
//@[54:55)        Colon |:|
//@[56:68)        VariableAccessSyntax
//@[56:68)         IdentifierSyntax
//@[56:68)          Identifier |doesNotExist|
//@[68:69)        RightSquare |]|
//@[69:70)      NewLine |\n|
  }
//@[2:3)      RightBrace |}|
//@[3:4)    NewLine |\n|
}]
//@[0:1)    RightBrace |}|
//@[1:2)   RightSquare |]|
//@[2:4) NewLine |\n\n|

output directRefToCollectionViaOutput array = nonexistentArrays
//@[0:63) OutputDeclarationSyntax
//@[0:6)  Identifier |output|
//@[7:37)  IdentifierSyntax
//@[7:37)   Identifier |directRefToCollectionViaOutput|
//@[38:43)  TypeSyntax
//@[38:43)   Identifier |array|
//@[44:45)  Assignment |=|
//@[46:63)  VariableAccessSyntax
//@[46:63)   IdentifierSyntax
//@[46:63)    Identifier |nonexistentArrays|
//@[63:65) NewLine |\n\n|

module directRefToCollectionViaSingleBody 'modulea.bicep' = {
//@[0:203) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:41)  IdentifierSyntax
//@[7:41)   Identifier |directRefToCollectionViaSingleBody|
//@[42:57)  StringSyntax
//@[42:57)   StringComplete |'modulea.bicep'|
//@[58:59)  Assignment |=|
//@[60:203)  ObjectSyntax
//@[60:61)   LeftBrace |{|
//@[61:62)   NewLine |\n|
  name: 'hello'
//@[2:15)   ObjectPropertySyntax
//@[2:6)    IdentifierSyntax
//@[2:6)     Identifier |name|
//@[6:7)    Colon |:|
//@[8:15)    StringSyntax
//@[8:15)     StringComplete |'hello'|
//@[15:16)   NewLine |\n|
  params: {
//@[2:123)   ObjectPropertySyntax
//@[2:8)    IdentifierSyntax
//@[2:8)     Identifier |params|
//@[8:9)    Colon |:|
//@[10:123)    ObjectSyntax
//@[10:11)     LeftBrace |{|
//@[11:12)     NewLine |\n|
    arrayParam: concat(wrongModuleParameterInLoop, nonexistentArrays)
//@[4:69)     ObjectPropertySyntax
//@[4:14)      IdentifierSyntax
//@[4:14)       Identifier |arrayParam|
//@[14:15)      Colon |:|
//@[16:69)      FunctionCallSyntax
//@[16:22)       IdentifierSyntax
//@[16:22)        Identifier |concat|
//@[22:23)       LeftParen |(|
//@[23:50)       FunctionArgumentSyntax
//@[23:49)        VariableAccessSyntax
//@[23:49)         IdentifierSyntax
//@[23:49)          Identifier |wrongModuleParameterInLoop|
//@[49:50)        Comma |,|
//@[51:68)       FunctionArgumentSyntax
//@[51:68)        VariableAccessSyntax
//@[51:68)         IdentifierSyntax
//@[51:68)          Identifier |nonexistentArrays|
//@[68:69)       RightParen |)|
//@[69:70)     NewLine |\n|
    objParam: {}
//@[4:16)     ObjectPropertySyntax
//@[4:12)      IdentifierSyntax
//@[4:12)       Identifier |objParam|
//@[12:13)      Colon |:|
//@[14:16)      ObjectSyntax
//@[14:15)       LeftBrace |{|
//@[15:16)       RightBrace |}|
//@[16:17)     NewLine |\n|
    stringParamB: ''
//@[4:20)     ObjectPropertySyntax
//@[4:16)      IdentifierSyntax
//@[4:16)       Identifier |stringParamB|
//@[16:17)      Colon |:|
//@[18:20)      StringSyntax
//@[18:20)       StringComplete |''|
//@[20:21)     NewLine |\n|
  }
//@[2:3)     RightBrace |}|
//@[3:4)   NewLine |\n|
}
//@[0:1)   RightBrace |}|
//@[1:3) NewLine |\n\n|

module directRefToCollectionViaSingleConditionalBody 'modulea.bicep' = if(true) {
//@[0:224) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:52)  IdentifierSyntax
//@[7:52)   Identifier |directRefToCollectionViaSingleConditionalBody|
//@[53:68)  StringSyntax
//@[53:68)   StringComplete |'modulea.bicep'|
//@[69:70)  Assignment |=|
//@[71:224)  IfConditionSyntax
//@[71:73)   Identifier |if|
//@[73:79)   ParenthesizedExpressionSyntax
//@[73:74)    LeftParen |(|
//@[74:78)    BooleanLiteralSyntax
//@[74:78)     TrueKeyword |true|
//@[78:79)    RightParen |)|
//@[80:224)   ObjectSyntax
//@[80:81)    LeftBrace |{|
//@[81:82)    NewLine |\n|
  name: 'hello2'
//@[2:16)    ObjectPropertySyntax
//@[2:6)     IdentifierSyntax
//@[2:6)      Identifier |name|
//@[6:7)     Colon |:|
//@[8:16)     StringSyntax
//@[8:16)      StringComplete |'hello2'|
//@[16:17)    NewLine |\n|
  params: {
//@[2:123)    ObjectPropertySyntax
//@[2:8)     IdentifierSyntax
//@[2:8)      Identifier |params|
//@[8:9)     Colon |:|
//@[10:123)     ObjectSyntax
//@[10:11)      LeftBrace |{|
//@[11:12)      NewLine |\n|
    arrayParam: concat(wrongModuleParameterInLoop, nonexistentArrays)
//@[4:69)      ObjectPropertySyntax
//@[4:14)       IdentifierSyntax
//@[4:14)        Identifier |arrayParam|
//@[14:15)       Colon |:|
//@[16:69)       FunctionCallSyntax
//@[16:22)        IdentifierSyntax
//@[16:22)         Identifier |concat|
//@[22:23)        LeftParen |(|
//@[23:50)        FunctionArgumentSyntax
//@[23:49)         VariableAccessSyntax
//@[23:49)          IdentifierSyntax
//@[23:49)           Identifier |wrongModuleParameterInLoop|
//@[49:50)         Comma |,|
//@[51:68)        FunctionArgumentSyntax
//@[51:68)         VariableAccessSyntax
//@[51:68)          IdentifierSyntax
//@[51:68)           Identifier |nonexistentArrays|
//@[68:69)        RightParen |)|
//@[69:70)      NewLine |\n|
    objParam: {}
//@[4:16)      ObjectPropertySyntax
//@[4:12)       IdentifierSyntax
//@[4:12)        Identifier |objParam|
//@[12:13)       Colon |:|
//@[14:16)       ObjectSyntax
//@[14:15)        LeftBrace |{|
//@[15:16)        RightBrace |}|
//@[16:17)      NewLine |\n|
    stringParamB: ''
//@[4:20)      ObjectPropertySyntax
//@[4:16)       IdentifierSyntax
//@[4:16)        Identifier |stringParamB|
//@[16:17)       Colon |:|
//@[18:20)       StringSyntax
//@[18:20)        StringComplete |''|
//@[20:21)      NewLine |\n|
  }
//@[2:3)      RightBrace |}|
//@[3:4)    NewLine |\n|
}
//@[0:1)    RightBrace |}|
//@[1:3) NewLine |\n\n|

module directRefToCollectionViaLoopBody 'modulea.bicep' = [for test in []: {
//@[0:220) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:39)  IdentifierSyntax
//@[7:39)   Identifier |directRefToCollectionViaLoopBody|
//@[40:55)  StringSyntax
//@[40:55)   StringComplete |'modulea.bicep'|
//@[56:57)  Assignment |=|
//@[58:220)  ForSyntax
//@[58:59)   LeftSquare |[|
//@[59:62)   Identifier |for|
//@[63:67)   LocalVariableSyntax
//@[63:67)    IdentifierSyntax
//@[63:67)     Identifier |test|
//@[68:70)   Identifier |in|
//@[71:73)   ArraySyntax
//@[71:72)    LeftSquare |[|
//@[72:73)    RightSquare |]|
//@[73:74)   Colon |:|
//@[75:219)   ObjectSyntax
//@[75:76)    LeftBrace |{|
//@[76:77)    NewLine |\n|
  name: 'hello3'
//@[2:16)    ObjectPropertySyntax
//@[2:6)     IdentifierSyntax
//@[2:6)      Identifier |name|
//@[6:7)     Colon |:|
//@[8:16)     StringSyntax
//@[8:16)      StringComplete |'hello3'|
//@[16:17)    NewLine |\n|
  params: {
//@[2:123)    ObjectPropertySyntax
//@[2:8)     IdentifierSyntax
//@[2:8)      Identifier |params|
//@[8:9)     Colon |:|
//@[10:123)     ObjectSyntax
//@[10:11)      LeftBrace |{|
//@[11:12)      NewLine |\n|
    arrayParam: concat(wrongModuleParameterInLoop, nonexistentArrays)
//@[4:69)      ObjectPropertySyntax
//@[4:14)       IdentifierSyntax
//@[4:14)        Identifier |arrayParam|
//@[14:15)       Colon |:|
//@[16:69)       FunctionCallSyntax
//@[16:22)        IdentifierSyntax
//@[16:22)         Identifier |concat|
//@[22:23)        LeftParen |(|
//@[23:50)        FunctionArgumentSyntax
//@[23:49)         VariableAccessSyntax
//@[23:49)          IdentifierSyntax
//@[23:49)           Identifier |wrongModuleParameterInLoop|
//@[49:50)         Comma |,|
//@[51:68)        FunctionArgumentSyntax
//@[51:68)         VariableAccessSyntax
//@[51:68)          IdentifierSyntax
//@[51:68)           Identifier |nonexistentArrays|
//@[68:69)        RightParen |)|
//@[69:70)      NewLine |\n|
    objParam: {}
//@[4:16)      ObjectPropertySyntax
//@[4:12)       IdentifierSyntax
//@[4:12)        Identifier |objParam|
//@[12:13)       Colon |:|
//@[14:16)       ObjectSyntax
//@[14:15)        LeftBrace |{|
//@[15:16)        RightBrace |}|
//@[16:17)      NewLine |\n|
    stringParamB: ''
//@[4:20)      ObjectPropertySyntax
//@[4:16)       IdentifierSyntax
//@[4:16)        Identifier |stringParamB|
//@[16:17)       Colon |:|
//@[18:20)       StringSyntax
//@[18:20)        StringComplete |''|
//@[20:21)      NewLine |\n|
  }
//@[2:3)      RightBrace |}|
//@[3:4)    NewLine |\n|
}]
//@[0:1)    RightBrace |}|
//@[1:2)   RightSquare |]|
//@[2:4) NewLine |\n\n|

module directRefToCollectionViaLoopBodyWithExtraDependsOn 'modulea.bicep' = [for test in []: {
//@[0:309) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:57)  IdentifierSyntax
//@[7:57)   Identifier |directRefToCollectionViaLoopBodyWithExtraDependsOn|
//@[58:73)  StringSyntax
//@[58:73)   StringComplete |'modulea.bicep'|
//@[74:75)  Assignment |=|
//@[76:309)  ForSyntax
//@[76:77)   LeftSquare |[|
//@[77:80)   Identifier |for|
//@[81:85)   LocalVariableSyntax
//@[81:85)    IdentifierSyntax
//@[81:85)     Identifier |test|
//@[86:88)   Identifier |in|
//@[89:91)   ArraySyntax
//@[89:90)    LeftSquare |[|
//@[90:91)    RightSquare |]|
//@[91:92)   Colon |:|
//@[93:308)   ObjectSyntax
//@[93:94)    LeftBrace |{|
//@[94:95)    NewLine |\n|
  name: 'hello4'
//@[2:16)    ObjectPropertySyntax
//@[2:6)     IdentifierSyntax
//@[2:6)      Identifier |name|
//@[6:7)     Colon |:|
//@[8:16)     StringSyntax
//@[8:16)      StringComplete |'hello4'|
//@[16:17)    NewLine |\n|
  params: {
//@[2:170)    ObjectPropertySyntax
//@[2:8)     IdentifierSyntax
//@[2:8)      Identifier |params|
//@[8:9)     Colon |:|
//@[10:170)     ObjectSyntax
//@[10:11)      LeftBrace |{|
//@[11:12)      NewLine |\n|
    arrayParam: concat(wrongModuleParameterInLoop, nonexistentArrays)
//@[4:69)      ObjectPropertySyntax
//@[4:14)       IdentifierSyntax
//@[4:14)        Identifier |arrayParam|
//@[14:15)       Colon |:|
//@[16:69)       FunctionCallSyntax
//@[16:22)        IdentifierSyntax
//@[16:22)         Identifier |concat|
//@[22:23)        LeftParen |(|
//@[23:50)        FunctionArgumentSyntax
//@[23:49)         VariableAccessSyntax
//@[23:49)          IdentifierSyntax
//@[23:49)           Identifier |wrongModuleParameterInLoop|
//@[49:50)         Comma |,|
//@[51:68)        FunctionArgumentSyntax
//@[51:68)         VariableAccessSyntax
//@[51:68)          IdentifierSyntax
//@[51:68)           Identifier |nonexistentArrays|
//@[68:69)        RightParen |)|
//@[69:70)      NewLine |\n|
    objParam: {}
//@[4:16)      ObjectPropertySyntax
//@[4:12)       IdentifierSyntax
//@[4:12)        Identifier |objParam|
//@[12:13)       Colon |:|
//@[14:16)       ObjectSyntax
//@[14:15)        LeftBrace |{|
//@[15:16)        RightBrace |}|
//@[16:17)      NewLine |\n|
    stringParamB: ''
//@[4:20)      ObjectPropertySyntax
//@[4:16)       IdentifierSyntax
//@[4:16)        Identifier |stringParamB|
//@[16:17)       Colon |:|
//@[18:20)       StringSyntax
//@[18:20)        StringComplete |''|
//@[20:21)      NewLine |\n|
    dependsOn: [
//@[4:46)      ObjectPropertySyntax
//@[4:13)       IdentifierSyntax
//@[4:13)        Identifier |dependsOn|
//@[13:14)       Colon |:|
//@[15:46)       ArraySyntax
//@[15:16)        LeftSquare |[|
//@[16:17)        NewLine |\n|
      nonexistentArrays
//@[6:23)        ArrayItemSyntax
//@[6:23)         VariableAccessSyntax
//@[6:23)          IdentifierSyntax
//@[6:23)           Identifier |nonexistentArrays|
//@[23:24)        NewLine |\n|
    ]
//@[4:5)        RightSquare |]|
//@[5:6)      NewLine |\n|
  }
//@[2:3)      RightBrace |}|
//@[3:4)    NewLine |\n|
  dependsOn: [
//@[2:23)    ObjectPropertySyntax
//@[2:11)     IdentifierSyntax
//@[2:11)      Identifier |dependsOn|
//@[11:12)     Colon |:|
//@[13:23)     ArraySyntax
//@[13:14)      LeftSquare |[|
//@[14:15)      NewLine |\n|
    
//@[4:5)      NewLine |\n|
  ]
//@[2:3)      RightSquare |]|
//@[3:4)    NewLine |\n|
}]
//@[0:1)    RightBrace |}|
//@[1:2)   RightSquare |]|
//@[2:5) NewLine |\n\n\n|


// module body that isn't an object
//@[35:36) NewLine |\n|
module nonObjectModuleBody 'modulea.bicep' = [for thing in []: 'hello']
//@[0:71) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:26)  IdentifierSyntax
//@[7:26)   Identifier |nonObjectModuleBody|
//@[27:42)  StringSyntax
//@[27:42)   StringComplete |'modulea.bicep'|
//@[43:44)  Assignment |=|
//@[45:71)  ForSyntax
//@[45:46)   LeftSquare |[|
//@[46:49)   Identifier |for|
//@[50:55)   LocalVariableSyntax
//@[50:55)    IdentifierSyntax
//@[50:55)     Identifier |thing|
//@[56:58)   Identifier |in|
//@[59:61)   ArraySyntax
//@[59:60)    LeftSquare |[|
//@[60:61)    RightSquare |]|
//@[61:62)   Colon |:|
//@[63:70)   SkippedTriviaSyntax
//@[63:70)    StringComplete |'hello'|
//@[70:71)   RightSquare |]|
//@[71:72) NewLine |\n|
module nonObjectModuleBody2 'modulea.bicep' = [for thing in []: concat()]
//@[0:73) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:27)  IdentifierSyntax
//@[7:27)   Identifier |nonObjectModuleBody2|
//@[28:43)  StringSyntax
//@[28:43)   StringComplete |'modulea.bicep'|
//@[44:45)  Assignment |=|
//@[46:73)  ForSyntax
//@[46:47)   LeftSquare |[|
//@[47:50)   Identifier |for|
//@[51:56)   LocalVariableSyntax
//@[51:56)    IdentifierSyntax
//@[51:56)     Identifier |thing|
//@[57:59)   Identifier |in|
//@[60:62)   ArraySyntax
//@[60:61)    LeftSquare |[|
//@[61:62)    RightSquare |]|
//@[62:63)   Colon |:|
//@[64:72)   SkippedTriviaSyntax
//@[64:70)    Identifier |concat|
//@[70:71)    LeftParen |(|
//@[71:72)    RightParen |)|
//@[72:73)   RightSquare |]|
//@[73:74) NewLine |\n|
module nonObjectModuleBody3 'modulea.bicep' = [for (thing,i) in []: 'hello']
//@[0:76) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:27)  IdentifierSyntax
//@[7:27)   Identifier |nonObjectModuleBody3|
//@[28:43)  StringSyntax
//@[28:43)   StringComplete |'modulea.bicep'|
//@[44:45)  Assignment |=|
//@[46:76)  ForSyntax
//@[46:47)   LeftSquare |[|
//@[47:50)   Identifier |for|
//@[51:60)   ForVariableBlockSyntax
//@[51:52)    LeftParen |(|
//@[52:57)    LocalVariableSyntax
//@[52:57)     IdentifierSyntax
//@[52:57)      Identifier |thing|
//@[57:58)    Comma |,|
//@[58:59)    LocalVariableSyntax
//@[58:59)     IdentifierSyntax
//@[58:59)      Identifier |i|
//@[59:60)    RightParen |)|
//@[61:63)   Identifier |in|
//@[64:66)   ArraySyntax
//@[64:65)    LeftSquare |[|
//@[65:66)    RightSquare |]|
//@[66:67)   Colon |:|
//@[68:75)   SkippedTriviaSyntax
//@[68:75)    StringComplete |'hello'|
//@[75:76)   RightSquare |]|
//@[76:77) NewLine |\n|
module nonObjectModuleBody4 'modulea.bicep' = [for (thing,i) in []: concat()]
//@[0:77) ModuleDeclarationSyntax
//@[0:6)  Identifier |module|
//@[7:27)  IdentifierSyntax
//@[7:27)   Identifier |nonObjectModuleBody4|
//@[28:43)  StringSyntax
//@[28:43)   StringComplete |'modulea.bicep'|
//@[44:45)  Assignment |=|
//@[46:77)  ForSyntax
//@[46:47)   LeftSquare |[|
//@[47:50)   Identifier |for|
//@[51:60)   ForVariableBlockSyntax
//@[51:52)    LeftParen |(|
//@[52:57)    LocalVariableSyntax
//@[52:57)     IdentifierSyntax
//@[52:57)      Identifier |thing|
//@[57:58)    Comma |,|
//@[58:59)    LocalVariableSyntax
//@[58:59)     IdentifierSyntax
//@[58:59)      Identifier |i|
//@[59:60)    RightParen |)|
//@[61:63)   Identifier |in|
//@[64:66)   ArraySyntax
//@[64:65)    LeftSquare |[|
//@[65:66)    RightSquare |]|
//@[66:67)   Colon |:|
//@[68:76)   SkippedTriviaSyntax
//@[68:74)    Identifier |concat|
//@[74:75)    LeftParen |(|
//@[75:76)    RightParen |)|
//@[76:77)   RightSquare |]|
//@[77:78) NewLine |\n|

//@[0:0) EndOfFile ||
