module nonExistentFileRef './nonExistent.bicep' = {
//@[000:13682) ProgramSyntax
//@[000:00054) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00025) | ├─IdentifierSyntax
//@[007:00025) | | └─Token(Identifier) |nonExistentFileRef|
//@[026:00047) | ├─StringSyntax
//@[026:00047) | | └─Token(StringComplete) |'./nonExistent.bicep'|
//@[048:00049) | ├─Token(Assignment) |=|
//@[050:00054) | └─ObjectSyntax
//@[050:00051) |   ├─Token(LeftBrace) |{|
//@[051:00053) |   ├─Token(NewLine) |\n\n|

}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00003) ├─Token(NewLine) |\n\n|

// we should only look this file up once, but should still return the same failure
//@[082:00083) ├─Token(NewLine) |\n|
module nonExistentFileRefDuplicate './nonExistent.bicep' = {
//@[000:00063) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00034) | ├─IdentifierSyntax
//@[007:00034) | | └─Token(Identifier) |nonExistentFileRefDuplicate|
//@[035:00056) | ├─StringSyntax
//@[035:00056) | | └─Token(StringComplete) |'./nonExistent.bicep'|
//@[057:00058) | ├─Token(Assignment) |=|
//@[059:00063) | └─ObjectSyntax
//@[059:00060) |   ├─Token(LeftBrace) |{|
//@[060:00062) |   ├─Token(NewLine) |\n\n|

}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00003) ├─Token(NewLine) |\n\n|

// we should only look this file up once, but should still return the same failure
//@[082:00083) ├─Token(NewLine) |\n|
module nonExistentFileRefEquivalentPath 'abc/def/../../nonExistent.bicep' = {
//@[000:00080) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00039) | ├─IdentifierSyntax
//@[007:00039) | | └─Token(Identifier) |nonExistentFileRefEquivalentPath|
//@[040:00073) | ├─StringSyntax
//@[040:00073) | | └─Token(StringComplete) |'abc/def/../../nonExistent.bicep'|
//@[074:00075) | ├─Token(Assignment) |=|
//@[076:00080) | └─ObjectSyntax
//@[076:00077) |   ├─Token(LeftBrace) |{|
//@[077:00079) |   ├─Token(NewLine) |\n\n|

}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00003) ├─Token(NewLine) |\n\n|

module moduleWithoutPath = {
//@[000:00028) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00024) | ├─IdentifierSyntax
//@[007:00024) | | └─Token(Identifier) |moduleWithoutPath|
//@[025:00028) | ├─SkippedTriviaSyntax
//@[025:00026) | | ├─Token(Assignment) |=|
//@[027:00028) | | └─Token(LeftBrace) |{|
//@[028:00028) | ├─SkippedTriviaSyntax
//@[028:00028) | └─SkippedTriviaSyntax
//@[028:00030) ├─Token(NewLine) |\n\n|

}
//@[000:00001) ├─SkippedTriviaSyntax
//@[000:00001) | └─Token(RightBrace) |}|
//@[001:00003) ├─Token(NewLine) |\n\n|

// #completionTest(41) -> moduleBodyCompletions
//@[047:00048) ├─Token(NewLine) |\n|
module moduleWithPath './moduleb.bicep' =
//@[000:00041) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00021) | ├─IdentifierSyntax
//@[007:00021) | | └─Token(Identifier) |moduleWithPath|
//@[022:00039) | ├─StringSyntax
//@[022:00039) | | └─Token(StringComplete) |'./moduleb.bicep'|
//@[040:00041) | ├─Token(Assignment) |=|
//@[041:00041) | └─SkippedTriviaSyntax
//@[041:00043) ├─Token(NewLine) |\n\n|

// missing identifier #completionTest(7) -> empty
//@[049:00050) ├─Token(NewLine) |\n|
module 
//@[000:00007) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00007) | ├─IdentifierSyntax
//@[007:00007) | | └─SkippedTriviaSyntax
//@[007:00007) | ├─SkippedTriviaSyntax
//@[007:00007) | ├─SkippedTriviaSyntax
//@[007:00007) | └─SkippedTriviaSyntax
//@[007:00009) ├─Token(NewLine) |\n\n|

// #completionTest(24,25) -> moduleObject
//@[041:00042) ├─Token(NewLine) |\n|
module missingValue '' = 
//@[000:00025) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00019) | ├─IdentifierSyntax
//@[007:00019) | | └─Token(Identifier) |missingValue|
//@[020:00022) | ├─StringSyntax
//@[020:00022) | | └─Token(StringComplete) |''|
//@[023:00024) | ├─Token(Assignment) |=|
//@[025:00025) | └─SkippedTriviaSyntax
//@[025:00027) ├─Token(NewLine) |\n\n|

var interp = 'hello'
//@[000:00020) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00010) | ├─IdentifierSyntax
//@[004:00010) | | └─Token(Identifier) |interp|
//@[011:00012) | ├─Token(Assignment) |=|
//@[013:00020) | └─StringSyntax
//@[013:00020) |   └─Token(StringComplete) |'hello'|
//@[020:00021) ├─Token(NewLine) |\n|
module moduleWithInterpPath './${interp}.bicep' = {
//@[000:00054) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00027) | ├─IdentifierSyntax
//@[007:00027) | | └─Token(Identifier) |moduleWithInterpPath|
//@[028:00047) | ├─StringSyntax
//@[028:00033) | | ├─Token(StringLeftPiece) |'./${|
//@[033:00039) | | ├─VariableAccessSyntax
//@[033:00039) | | | └─IdentifierSyntax
//@[033:00039) | | |   └─Token(Identifier) |interp|
//@[039:00047) | | └─Token(StringRightPiece) |}.bicep'|
//@[048:00049) | ├─Token(Assignment) |=|
//@[050:00054) | └─ObjectSyntax
//@[050:00051) |   ├─Token(LeftBrace) |{|
//@[051:00053) |   ├─Token(NewLine) |\n\n|

}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00003) ├─Token(NewLine) |\n\n|

module moduleWithConditionAndInterpPath './${interp}.bicep' = if (true) {
//@[000:00076) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00039) | ├─IdentifierSyntax
//@[007:00039) | | └─Token(Identifier) |moduleWithConditionAndInterpPath|
//@[040:00059) | ├─StringSyntax
//@[040:00045) | | ├─Token(StringLeftPiece) |'./${|
//@[045:00051) | | ├─VariableAccessSyntax
//@[045:00051) | | | └─IdentifierSyntax
//@[045:00051) | | |   └─Token(Identifier) |interp|
//@[051:00059) | | └─Token(StringRightPiece) |}.bicep'|
//@[060:00061) | ├─Token(Assignment) |=|
//@[062:00076) | └─IfConditionSyntax
//@[062:00064) |   ├─Token(Identifier) |if|
//@[065:00071) |   ├─ParenthesizedExpressionSyntax
//@[065:00066) |   | ├─Token(LeftParen) |(|
//@[066:00070) |   | ├─BooleanLiteralSyntax
//@[066:00070) |   | | └─Token(TrueKeyword) |true|
//@[070:00071) |   | └─Token(RightParen) |)|
//@[072:00076) |   └─ObjectSyntax
//@[072:00073) |     ├─Token(LeftBrace) |{|
//@[073:00075) |     ├─Token(NewLine) |\n\n|

}
//@[000:00001) |     └─Token(RightBrace) |}|
//@[001:00003) ├─Token(NewLine) |\n\n|

module moduleWithSelfCycle './main.bicep' = {
//@[000:00048) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00026) | ├─IdentifierSyntax
//@[007:00026) | | └─Token(Identifier) |moduleWithSelfCycle|
//@[027:00041) | ├─StringSyntax
//@[027:00041) | | └─Token(StringComplete) |'./main.bicep'|
//@[042:00043) | ├─Token(Assignment) |=|
//@[044:00048) | └─ObjectSyntax
//@[044:00045) |   ├─Token(LeftBrace) |{|
//@[045:00047) |   ├─Token(NewLine) |\n\n|

}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00003) ├─Token(NewLine) |\n\n|

module moduleWithConditionAndSelfCycle './main.bicep' = if ('foo' == 'bar') {
//@[000:00080) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00038) | ├─IdentifierSyntax
//@[007:00038) | | └─Token(Identifier) |moduleWithConditionAndSelfCycle|
//@[039:00053) | ├─StringSyntax
//@[039:00053) | | └─Token(StringComplete) |'./main.bicep'|
//@[054:00055) | ├─Token(Assignment) |=|
//@[056:00080) | └─IfConditionSyntax
//@[056:00058) |   ├─Token(Identifier) |if|
//@[059:00075) |   ├─ParenthesizedExpressionSyntax
//@[059:00060) |   | ├─Token(LeftParen) |(|
//@[060:00074) |   | ├─BinaryOperationSyntax
//@[060:00065) |   | | ├─StringSyntax
//@[060:00065) |   | | | └─Token(StringComplete) |'foo'|
//@[066:00068) |   | | ├─Token(Equals) |==|
//@[069:00074) |   | | └─StringSyntax
//@[069:00074) |   | |   └─Token(StringComplete) |'bar'|
//@[074:00075) |   | └─Token(RightParen) |)|
//@[076:00080) |   └─ObjectSyntax
//@[076:00077) |     ├─Token(LeftBrace) |{|
//@[077:00079) |     ├─Token(NewLine) |\n\n|

}
//@[000:00001) |     └─Token(RightBrace) |}|
//@[001:00003) ├─Token(NewLine) |\n\n|

module './main.bicep' = {
//@[000:00028) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00007) | ├─IdentifierSyntax
//@[007:00007) | | └─SkippedTriviaSyntax
//@[007:00021) | ├─StringSyntax
//@[007:00021) | | └─Token(StringComplete) |'./main.bicep'|
//@[022:00023) | ├─Token(Assignment) |=|
//@[024:00028) | └─ObjectSyntax
//@[024:00025) |   ├─Token(LeftBrace) |{|
//@[025:00027) |   ├─Token(NewLine) |\n\n|

}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00003) ├─Token(NewLine) |\n\n|

module './main.bicep' = if (1 + 2 == 3) {
//@[000:00044) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00007) | ├─IdentifierSyntax
//@[007:00007) | | └─SkippedTriviaSyntax
//@[007:00021) | ├─StringSyntax
//@[007:00021) | | └─Token(StringComplete) |'./main.bicep'|
//@[022:00023) | ├─Token(Assignment) |=|
//@[024:00044) | └─IfConditionSyntax
//@[024:00026) |   ├─Token(Identifier) |if|
//@[027:00039) |   ├─ParenthesizedExpressionSyntax
//@[027:00028) |   | ├─Token(LeftParen) |(|
//@[028:00038) |   | ├─BinaryOperationSyntax
//@[028:00033) |   | | ├─BinaryOperationSyntax
//@[028:00029) |   | | | ├─IntegerLiteralSyntax
//@[028:00029) |   | | | | └─Token(Integer) |1|
//@[030:00031) |   | | | ├─Token(Plus) |+|
//@[032:00033) |   | | | └─IntegerLiteralSyntax
//@[032:00033) |   | | |   └─Token(Integer) |2|
//@[034:00036) |   | | ├─Token(Equals) |==|
//@[037:00038) |   | | └─IntegerLiteralSyntax
//@[037:00038) |   | |   └─Token(Integer) |3|
//@[038:00039) |   | └─Token(RightParen) |)|
//@[040:00044) |   └─ObjectSyntax
//@[040:00041) |     ├─Token(LeftBrace) |{|
//@[041:00043) |     ├─Token(NewLine) |\n\n|

}
//@[000:00001) |     └─Token(RightBrace) |}|
//@[001:00003) ├─Token(NewLine) |\n\n|

module './main.bicep' = if
//@[000:00026) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00007) | ├─IdentifierSyntax
//@[007:00007) | | └─SkippedTriviaSyntax
//@[007:00021) | ├─StringSyntax
//@[007:00021) | | └─Token(StringComplete) |'./main.bicep'|
//@[022:00023) | ├─Token(Assignment) |=|
//@[024:00026) | └─IfConditionSyntax
//@[024:00026) |   ├─Token(Identifier) |if|
//@[026:00026) |   ├─SkippedTriviaSyntax
//@[026:00026) |   └─SkippedTriviaSyntax
//@[026:00028) ├─Token(NewLine) |\n\n|

module './main.bicep' = if (
//@[000:00028) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00007) | ├─IdentifierSyntax
//@[007:00007) | | └─SkippedTriviaSyntax
//@[007:00021) | ├─StringSyntax
//@[007:00021) | | └─Token(StringComplete) |'./main.bicep'|
//@[022:00023) | ├─Token(Assignment) |=|
//@[024:00028) | └─IfConditionSyntax
//@[024:00026) |   ├─Token(Identifier) |if|
//@[027:00028) |   ├─ParenthesizedExpressionSyntax
//@[027:00028) |   | ├─Token(LeftParen) |(|
//@[028:00028) |   | ├─SkippedTriviaSyntax
//@[028:00028) |   | └─SkippedTriviaSyntax
//@[028:00028) |   └─SkippedTriviaSyntax
//@[028:00030) ├─Token(NewLine) |\n\n|

module './main.bicep' = if (true
//@[000:00032) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00007) | ├─IdentifierSyntax
//@[007:00007) | | └─SkippedTriviaSyntax
//@[007:00021) | ├─StringSyntax
//@[007:00021) | | └─Token(StringComplete) |'./main.bicep'|
//@[022:00023) | ├─Token(Assignment) |=|
//@[024:00032) | └─IfConditionSyntax
//@[024:00026) |   ├─Token(Identifier) |if|
//@[027:00032) |   ├─ParenthesizedExpressionSyntax
//@[027:00028) |   | ├─Token(LeftParen) |(|
//@[028:00032) |   | ├─BooleanLiteralSyntax
//@[028:00032) |   | | └─Token(TrueKeyword) |true|
//@[032:00032) |   | └─SkippedTriviaSyntax
//@[032:00032) |   └─SkippedTriviaSyntax
//@[032:00034) ├─Token(NewLine) |\n\n|

module './main.bicep' = if (true)
//@[000:00033) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00007) | ├─IdentifierSyntax
//@[007:00007) | | └─SkippedTriviaSyntax
//@[007:00021) | ├─StringSyntax
//@[007:00021) | | └─Token(StringComplete) |'./main.bicep'|
//@[022:00023) | ├─Token(Assignment) |=|
//@[024:00033) | └─IfConditionSyntax
//@[024:00026) |   ├─Token(Identifier) |if|
//@[027:00033) |   ├─ParenthesizedExpressionSyntax
//@[027:00028) |   | ├─Token(LeftParen) |(|
//@[028:00032) |   | ├─BooleanLiteralSyntax
//@[028:00032) |   | | └─Token(TrueKeyword) |true|
//@[032:00033) |   | └─Token(RightParen) |)|
//@[033:00033) |   └─SkippedTriviaSyntax
//@[033:00035) ├─Token(NewLine) |\n\n|

module './main.bicep' = if {
//@[000:00031) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00007) | ├─IdentifierSyntax
//@[007:00007) | | └─SkippedTriviaSyntax
//@[007:00021) | ├─StringSyntax
//@[007:00021) | | └─Token(StringComplete) |'./main.bicep'|
//@[022:00023) | ├─Token(Assignment) |=|
//@[024:00031) | └─IfConditionSyntax
//@[024:00026) |   ├─Token(Identifier) |if|
//@[027:00027) |   ├─SkippedTriviaSyntax
//@[027:00031) |   └─ObjectSyntax
//@[027:00028) |     ├─Token(LeftBrace) |{|
//@[028:00030) |     ├─Token(NewLine) |\n\n|

}
//@[000:00001) |     └─Token(RightBrace) |}|
//@[001:00003) ├─Token(NewLine) |\n\n|

module './main.bicep' = if () {
//@[000:00034) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00007) | ├─IdentifierSyntax
//@[007:00007) | | └─SkippedTriviaSyntax
//@[007:00021) | ├─StringSyntax
//@[007:00021) | | └─Token(StringComplete) |'./main.bicep'|
//@[022:00023) | ├─Token(Assignment) |=|
//@[024:00034) | └─IfConditionSyntax
//@[024:00026) |   ├─Token(Identifier) |if|
//@[027:00029) |   ├─ParenthesizedExpressionSyntax
//@[027:00028) |   | ├─Token(LeftParen) |(|
//@[028:00028) |   | ├─SkippedTriviaSyntax
//@[028:00029) |   | └─Token(RightParen) |)|
//@[030:00034) |   └─ObjectSyntax
//@[030:00031) |     ├─Token(LeftBrace) |{|
//@[031:00033) |     ├─Token(NewLine) |\n\n|

}
//@[000:00001) |     └─Token(RightBrace) |}|
//@[001:00003) ├─Token(NewLine) |\n\n|

module './main.bicep' = if ('true') {
//@[000:00040) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00007) | ├─IdentifierSyntax
//@[007:00007) | | └─SkippedTriviaSyntax
//@[007:00021) | ├─StringSyntax
//@[007:00021) | | └─Token(StringComplete) |'./main.bicep'|
//@[022:00023) | ├─Token(Assignment) |=|
//@[024:00040) | └─IfConditionSyntax
//@[024:00026) |   ├─Token(Identifier) |if|
//@[027:00035) |   ├─ParenthesizedExpressionSyntax
//@[027:00028) |   | ├─Token(LeftParen) |(|
//@[028:00034) |   | ├─StringSyntax
//@[028:00034) |   | | └─Token(StringComplete) |'true'|
//@[034:00035) |   | └─Token(RightParen) |)|
//@[036:00040) |   └─ObjectSyntax
//@[036:00037) |     ├─Token(LeftBrace) |{|
//@[037:00039) |     ├─Token(NewLine) |\n\n|

}
//@[000:00001) |     └─Token(RightBrace) |}|
//@[001:00003) ├─Token(NewLine) |\n\n|

module modANoName './modulea.bicep' = {
//@[000:00093) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00017) | ├─IdentifierSyntax
//@[007:00017) | | └─Token(Identifier) |modANoName|
//@[018:00035) | ├─StringSyntax
//@[018:00035) | | └─Token(StringComplete) |'./modulea.bicep'|
//@[036:00037) | ├─Token(Assignment) |=|
//@[038:00093) | └─ObjectSyntax
//@[038:00039) |   ├─Token(LeftBrace) |{|
//@[039:00040) |   ├─Token(NewLine) |\n|
// #completionTest(0) -> moduleATopLevelProperties
//@[050:00052) |   ├─Token(NewLine) |\n\n|

}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00003) ├─Token(NewLine) |\n\n|

module modANoNameWithCondition './modulea.bicep' = if (true) {
//@[000:00129) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00030) | ├─IdentifierSyntax
//@[007:00030) | | └─Token(Identifier) |modANoNameWithCondition|
//@[031:00048) | ├─StringSyntax
//@[031:00048) | | └─Token(StringComplete) |'./modulea.bicep'|
//@[049:00050) | ├─Token(Assignment) |=|
//@[051:00129) | └─IfConditionSyntax
//@[051:00053) |   ├─Token(Identifier) |if|
//@[054:00060) |   ├─ParenthesizedExpressionSyntax
//@[054:00055) |   | ├─Token(LeftParen) |(|
//@[055:00059) |   | ├─BooleanLiteralSyntax
//@[055:00059) |   | | └─Token(TrueKeyword) |true|
//@[059:00060) |   | └─Token(RightParen) |)|
//@[061:00129) |   └─ObjectSyntax
//@[061:00062) |     ├─Token(LeftBrace) |{|
//@[062:00063) |     ├─Token(NewLine) |\n|
// #completionTest(0) -> moduleAWithConditionTopLevelProperties
//@[063:00065) |     ├─Token(NewLine) |\n\n|

}
//@[000:00001) |     └─Token(RightBrace) |}|
//@[001:00003) ├─Token(NewLine) |\n\n|

module modWithReferenceInCondition './main.bicep' = if (reference('Micorosft.Management/managementGroups/MG', '2020-05-01').name == 'something') {
//@[000:00149) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00034) | ├─IdentifierSyntax
//@[007:00034) | | └─Token(Identifier) |modWithReferenceInCondition|
//@[035:00049) | ├─StringSyntax
//@[035:00049) | | └─Token(StringComplete) |'./main.bicep'|
//@[050:00051) | ├─Token(Assignment) |=|
//@[052:00149) | └─IfConditionSyntax
//@[052:00054) |   ├─Token(Identifier) |if|
//@[055:00144) |   ├─ParenthesizedExpressionSyntax
//@[055:00056) |   | ├─Token(LeftParen) |(|
//@[056:00143) |   | ├─BinaryOperationSyntax
//@[056:00128) |   | | ├─PropertyAccessSyntax
//@[056:00123) |   | | | ├─FunctionCallSyntax
//@[056:00065) |   | | | | ├─IdentifierSyntax
//@[056:00065) |   | | | | | └─Token(Identifier) |reference|
//@[065:00066) |   | | | | ├─Token(LeftParen) |(|
//@[066:00108) |   | | | | ├─FunctionArgumentSyntax
//@[066:00108) |   | | | | | └─StringSyntax
//@[066:00108) |   | | | | |   └─Token(StringComplete) |'Micorosft.Management/managementGroups/MG'|
//@[108:00109) |   | | | | ├─Token(Comma) |,|
//@[110:00122) |   | | | | ├─FunctionArgumentSyntax
//@[110:00122) |   | | | | | └─StringSyntax
//@[110:00122) |   | | | | |   └─Token(StringComplete) |'2020-05-01'|
//@[122:00123) |   | | | | └─Token(RightParen) |)|
//@[123:00124) |   | | | ├─Token(Dot) |.|
//@[124:00128) |   | | | └─IdentifierSyntax
//@[124:00128) |   | | |   └─Token(Identifier) |name|
//@[129:00131) |   | | ├─Token(Equals) |==|
//@[132:00143) |   | | └─StringSyntax
//@[132:00143) |   | |   └─Token(StringComplete) |'something'|
//@[143:00144) |   | └─Token(RightParen) |)|
//@[145:00149) |   └─ObjectSyntax
//@[145:00146) |     ├─Token(LeftBrace) |{|
//@[146:00148) |     ├─Token(NewLine) |\n\n|

}
//@[000:00001) |     └─Token(RightBrace) |}|
//@[001:00003) ├─Token(NewLine) |\n\n|

module modWithListKeysInCondition './main.bicep' = if (listKeys('foo', '2020-05-01').bar == true) {
//@[000:00102) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00033) | ├─IdentifierSyntax
//@[007:00033) | | └─Token(Identifier) |modWithListKeysInCondition|
//@[034:00048) | ├─StringSyntax
//@[034:00048) | | └─Token(StringComplete) |'./main.bicep'|
//@[049:00050) | ├─Token(Assignment) |=|
//@[051:00102) | └─IfConditionSyntax
//@[051:00053) |   ├─Token(Identifier) |if|
//@[054:00097) |   ├─ParenthesizedExpressionSyntax
//@[054:00055) |   | ├─Token(LeftParen) |(|
//@[055:00096) |   | ├─BinaryOperationSyntax
//@[055:00088) |   | | ├─PropertyAccessSyntax
//@[055:00084) |   | | | ├─FunctionCallSyntax
//@[055:00063) |   | | | | ├─IdentifierSyntax
//@[055:00063) |   | | | | | └─Token(Identifier) |listKeys|
//@[063:00064) |   | | | | ├─Token(LeftParen) |(|
//@[064:00069) |   | | | | ├─FunctionArgumentSyntax
//@[064:00069) |   | | | | | └─StringSyntax
//@[064:00069) |   | | | | |   └─Token(StringComplete) |'foo'|
//@[069:00070) |   | | | | ├─Token(Comma) |,|
//@[071:00083) |   | | | | ├─FunctionArgumentSyntax
//@[071:00083) |   | | | | | └─StringSyntax
//@[071:00083) |   | | | | |   └─Token(StringComplete) |'2020-05-01'|
//@[083:00084) |   | | | | └─Token(RightParen) |)|
//@[084:00085) |   | | | ├─Token(Dot) |.|
//@[085:00088) |   | | | └─IdentifierSyntax
//@[085:00088) |   | | |   └─Token(Identifier) |bar|
//@[089:00091) |   | | ├─Token(Equals) |==|
//@[092:00096) |   | | └─BooleanLiteralSyntax
//@[092:00096) |   | |   └─Token(TrueKeyword) |true|
//@[096:00097) |   | └─Token(RightParen) |)|
//@[098:00102) |   └─ObjectSyntax
//@[098:00099) |     ├─Token(LeftBrace) |{|
//@[099:00101) |     ├─Token(NewLine) |\n\n|

}
//@[000:00001) |     └─Token(RightBrace) |}|
//@[001:00004) ├─Token(NewLine) |\n\n\n|


module modANoName './modulea.bicep' = if ({ 'a': b }.a == true) {
//@[000:00068) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00017) | ├─IdentifierSyntax
//@[007:00017) | | └─Token(Identifier) |modANoName|
//@[018:00035) | ├─StringSyntax
//@[018:00035) | | └─Token(StringComplete) |'./modulea.bicep'|
//@[036:00037) | ├─Token(Assignment) |=|
//@[038:00068) | └─IfConditionSyntax
//@[038:00040) |   ├─Token(Identifier) |if|
//@[041:00063) |   ├─ParenthesizedExpressionSyntax
//@[041:00042) |   | ├─Token(LeftParen) |(|
//@[042:00062) |   | ├─BinaryOperationSyntax
//@[042:00054) |   | | ├─PropertyAccessSyntax
//@[042:00052) |   | | | ├─ObjectSyntax
//@[042:00043) |   | | | | ├─Token(LeftBrace) |{|
//@[044:00050) |   | | | | ├─ObjectPropertySyntax
//@[044:00047) |   | | | | | ├─StringSyntax
//@[044:00047) |   | | | | | | └─Token(StringComplete) |'a'|
//@[047:00048) |   | | | | | ├─Token(Colon) |:|
//@[049:00050) |   | | | | | └─VariableAccessSyntax
//@[049:00050) |   | | | | |   └─IdentifierSyntax
//@[049:00050) |   | | | | |     └─Token(Identifier) |b|
//@[051:00052) |   | | | | └─Token(RightBrace) |}|
//@[052:00053) |   | | | ├─Token(Dot) |.|
//@[053:00054) |   | | | └─IdentifierSyntax
//@[053:00054) |   | | |   └─Token(Identifier) |a|
//@[055:00057) |   | | ├─Token(Equals) |==|
//@[058:00062) |   | | └─BooleanLiteralSyntax
//@[058:00062) |   | |   └─Token(TrueKeyword) |true|
//@[062:00063) |   | └─Token(RightParen) |)|
//@[064:00068) |   └─ObjectSyntax
//@[064:00065) |     ├─Token(LeftBrace) |{|
//@[065:00067) |     ├─Token(NewLine) |\n\n|

}
//@[000:00001) |     └─Token(RightBrace) |}|
//@[001:00003) ├─Token(NewLine) |\n\n|

module modANoInputs './modulea.bicep' = {
//@[000:00135) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00019) | ├─IdentifierSyntax
//@[007:00019) | | └─Token(Identifier) |modANoInputs|
//@[020:00037) | ├─StringSyntax
//@[020:00037) | | └─Token(StringComplete) |'./modulea.bicep'|
//@[038:00039) | ├─Token(Assignment) |=|
//@[040:00135) | └─ObjectSyntax
//@[040:00041) |   ├─Token(LeftBrace) |{|
//@[041:00042) |   ├─Token(NewLine) |\n|
  name: 'modANoInputs'
//@[002:00022) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00022) |   | └─StringSyntax
//@[008:00022) |   |   └─Token(StringComplete) |'modANoInputs'|
//@[022:00023) |   ├─Token(NewLine) |\n|
  // #completionTest(0,1,2) -> moduleATopLevelPropertiesMinusName
//@[065:00066) |   ├─Token(NewLine) |\n|
  
//@[002:00003) |   ├─Token(NewLine) |\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00003) ├─Token(NewLine) |\n\n|

module modANoInputsWithCondition './modulea.bicep' = if (length([
//@[000:00191) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00032) | ├─IdentifierSyntax
//@[007:00032) | | └─Token(Identifier) |modANoInputsWithCondition|
//@[033:00050) | ├─StringSyntax
//@[033:00050) | | └─Token(StringComplete) |'./modulea.bicep'|
//@[051:00052) | ├─Token(Assignment) |=|
//@[053:00191) | └─IfConditionSyntax
//@[053:00055) |   ├─Token(Identifier) |if|
//@[056:00082) |   ├─ParenthesizedExpressionSyntax
//@[056:00057) |   | ├─Token(LeftParen) |(|
//@[057:00081) |   | ├─BinaryOperationSyntax
//@[057:00076) |   | | ├─FunctionCallSyntax
//@[057:00063) |   | | | ├─IdentifierSyntax
//@[057:00063) |   | | | | └─Token(Identifier) |length|
//@[063:00064) |   | | | ├─Token(LeftParen) |(|
//@[064:00075) |   | | | ├─FunctionArgumentSyntax
//@[064:00075) |   | | | | └─ArraySyntax
//@[064:00065) |   | | | |   ├─Token(LeftSquare) |[|
//@[065:00066) |   | | | |   ├─Token(NewLine) |\n|
  'foo'
//@[002:00007) |   | | | |   ├─ArrayItemSyntax
//@[002:00007) |   | | | |   | └─StringSyntax
//@[002:00007) |   | | | |   |   └─Token(StringComplete) |'foo'|
//@[007:00008) |   | | | |   ├─Token(NewLine) |\n|
]) == 1) {
//@[000:00001) |   | | | |   └─Token(RightSquare) |]|
//@[001:00002) |   | | | └─Token(RightParen) |)|
//@[003:00005) |   | | ├─Token(Equals) |==|
//@[006:00007) |   | | └─IntegerLiteralSyntax
//@[006:00007) |   | |   └─Token(Integer) |1|
//@[007:00008) |   | └─Token(RightParen) |)|
//@[009:00117) |   └─ObjectSyntax
//@[009:00010) |     ├─Token(LeftBrace) |{|
//@[010:00011) |     ├─Token(NewLine) |\n|
  name: 'modANoInputs'
//@[002:00022) |     ├─ObjectPropertySyntax
//@[002:00006) |     | ├─IdentifierSyntax
//@[002:00006) |     | | └─Token(Identifier) |name|
//@[006:00007) |     | ├─Token(Colon) |:|
//@[008:00022) |     | └─StringSyntax
//@[008:00022) |     |   └─Token(StringComplete) |'modANoInputs'|
//@[022:00023) |     ├─Token(NewLine) |\n|
  // #completionTest(0,1,2) -> moduleAWithConditionTopLevelPropertiesMinusName
//@[078:00079) |     ├─Token(NewLine) |\n|
  
//@[002:00003) |     ├─Token(NewLine) |\n|
}
//@[000:00001) |     └─Token(RightBrace) |}|
//@[001:00003) ├─Token(NewLine) |\n\n|

module modAEmptyInputs './modulea.bicep' = {
//@[000:00141) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00022) | ├─IdentifierSyntax
//@[007:00022) | | └─Token(Identifier) |modAEmptyInputs|
//@[023:00040) | ├─StringSyntax
//@[023:00040) | | └─Token(StringComplete) |'./modulea.bicep'|
//@[041:00042) | ├─Token(Assignment) |=|
//@[043:00141) | └─ObjectSyntax
//@[043:00044) |   ├─Token(LeftBrace) |{|
//@[044:00045) |   ├─Token(NewLine) |\n|
  name: 'modANoInputs'
//@[002:00022) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00022) |   | └─StringSyntax
//@[008:00022) |   |   └─Token(StringComplete) |'modANoInputs'|
//@[022:00023) |   ├─Token(NewLine) |\n|
  params: {
//@[002:00071) |   ├─ObjectPropertySyntax
//@[002:00008) |   | ├─IdentifierSyntax
//@[002:00008) |   | | └─Token(Identifier) |params|
//@[008:00009) |   | ├─Token(Colon) |:|
//@[010:00071) |   | └─ObjectSyntax
//@[010:00011) |   |   ├─Token(LeftBrace) |{|
//@[011:00012) |   |   ├─Token(NewLine) |\n|
    // #completionTest(0,1,2,3,4) -> moduleAParams
//@[050:00051) |   |   ├─Token(NewLine) |\n|
    
//@[004:00005) |   |   ├─Token(NewLine) |\n|
  }
//@[002:00003) |   |   └─Token(RightBrace) |}|
//@[003:00004) |   ├─Token(NewLine) |\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00003) ├─Token(NewLine) |\n\n|

module modAEmptyInputsWithCondition './modulea.bicep' = if (1 + 2 == 2) {
//@[000:00183) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00035) | ├─IdentifierSyntax
//@[007:00035) | | └─Token(Identifier) |modAEmptyInputsWithCondition|
//@[036:00053) | ├─StringSyntax
//@[036:00053) | | └─Token(StringComplete) |'./modulea.bicep'|
//@[054:00055) | ├─Token(Assignment) |=|
//@[056:00183) | └─IfConditionSyntax
//@[056:00058) |   ├─Token(Identifier) |if|
//@[059:00071) |   ├─ParenthesizedExpressionSyntax
//@[059:00060) |   | ├─Token(LeftParen) |(|
//@[060:00070) |   | ├─BinaryOperationSyntax
//@[060:00065) |   | | ├─BinaryOperationSyntax
//@[060:00061) |   | | | ├─IntegerLiteralSyntax
//@[060:00061) |   | | | | └─Token(Integer) |1|
//@[062:00063) |   | | | ├─Token(Plus) |+|
//@[064:00065) |   | | | └─IntegerLiteralSyntax
//@[064:00065) |   | | |   └─Token(Integer) |2|
//@[066:00068) |   | | ├─Token(Equals) |==|
//@[069:00070) |   | | └─IntegerLiteralSyntax
//@[069:00070) |   | |   └─Token(Integer) |2|
//@[070:00071) |   | └─Token(RightParen) |)|
//@[072:00183) |   └─ObjectSyntax
//@[072:00073) |     ├─Token(LeftBrace) |{|
//@[073:00074) |     ├─Token(NewLine) |\n|
  name: 'modANoInputs'
//@[002:00022) |     ├─ObjectPropertySyntax
//@[002:00006) |     | ├─IdentifierSyntax
//@[002:00006) |     | | └─Token(Identifier) |name|
//@[006:00007) |     | ├─Token(Colon) |:|
//@[008:00022) |     | └─StringSyntax
//@[008:00022) |     |   └─Token(StringComplete) |'modANoInputs'|
//@[022:00023) |     ├─Token(NewLine) |\n|
  params: {
//@[002:00084) |     ├─ObjectPropertySyntax
//@[002:00008) |     | ├─IdentifierSyntax
//@[002:00008) |     | | └─Token(Identifier) |params|
//@[008:00009) |     | ├─Token(Colon) |:|
//@[010:00084) |     | └─ObjectSyntax
//@[010:00011) |     |   ├─Token(LeftBrace) |{|
//@[011:00012) |     |   ├─Token(NewLine) |\n|
    // #completionTest(0,1,2,3,4) -> moduleAWithConditionParams
//@[063:00064) |     |   ├─Token(NewLine) |\n|
    
//@[004:00005) |     |   ├─Token(NewLine) |\n|
  }
//@[002:00003) |     |   └─Token(RightBrace) |}|
//@[003:00004) |     ├─Token(NewLine) |\n|
}
//@[000:00001) |     └─Token(RightBrace) |}|
//@[001:00003) ├─Token(NewLine) |\n\n|

// #completionTest(55) -> moduleATopLevelPropertyAccess
//@[055:00056) ├─Token(NewLine) |\n|
var modulePropertyAccessCompletions = modAEmptyInputs.o
//@[000:00055) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00035) | ├─IdentifierSyntax
//@[004:00035) | | └─Token(Identifier) |modulePropertyAccessCompletions|
//@[036:00037) | ├─Token(Assignment) |=|
//@[038:00055) | └─PropertyAccessSyntax
//@[038:00053) |   ├─VariableAccessSyntax
//@[038:00053) |   | └─IdentifierSyntax
//@[038:00053) |   |   └─Token(Identifier) |modAEmptyInputs|
//@[053:00054) |   ├─Token(Dot) |.|
//@[054:00055) |   └─IdentifierSyntax
//@[054:00055) |     └─Token(Identifier) |o|
//@[055:00057) ├─Token(NewLine) |\n\n|

// #completionTest(81) -> moduleAWithConditionTopLevelPropertyAccess
//@[068:00069) ├─Token(NewLine) |\n|
var moduleWithConditionPropertyAccessCompletions = modAEmptyInputsWithCondition.o
//@[000:00081) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00048) | ├─IdentifierSyntax
//@[004:00048) | | └─Token(Identifier) |moduleWithConditionPropertyAccessCompletions|
//@[049:00050) | ├─Token(Assignment) |=|
//@[051:00081) | └─PropertyAccessSyntax
//@[051:00079) |   ├─VariableAccessSyntax
//@[051:00079) |   | └─IdentifierSyntax
//@[051:00079) |   |   └─Token(Identifier) |modAEmptyInputsWithCondition|
//@[079:00080) |   ├─Token(Dot) |.|
//@[080:00081) |   └─IdentifierSyntax
//@[080:00081) |     └─Token(Identifier) |o|
//@[081:00083) ├─Token(NewLine) |\n\n|

// #completionTest(56) -> moduleAOutputs
//@[040:00041) ├─Token(NewLine) |\n|
var moduleOutputsCompletions = modAEmptyInputs.outputs.s
//@[000:00056) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00028) | ├─IdentifierSyntax
//@[004:00028) | | └─Token(Identifier) |moduleOutputsCompletions|
//@[029:00030) | ├─Token(Assignment) |=|
//@[031:00056) | └─PropertyAccessSyntax
//@[031:00054) |   ├─PropertyAccessSyntax
//@[031:00046) |   | ├─VariableAccessSyntax
//@[031:00046) |   | | └─IdentifierSyntax
//@[031:00046) |   | |   └─Token(Identifier) |modAEmptyInputs|
//@[046:00047) |   | ├─Token(Dot) |.|
//@[047:00054) |   | └─IdentifierSyntax
//@[047:00054) |   |   └─Token(Identifier) |outputs|
//@[054:00055) |   ├─Token(Dot) |.|
//@[055:00056) |   └─IdentifierSyntax
//@[055:00056) |     └─Token(Identifier) |s|
//@[056:00058) ├─Token(NewLine) |\n\n|

// #completionTest(82) -> moduleAWithConditionOutputs
//@[053:00054) ├─Token(NewLine) |\n|
var moduleWithConditionOutputsCompletions = modAEmptyInputsWithCondition.outputs.s
//@[000:00082) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00041) | ├─IdentifierSyntax
//@[004:00041) | | └─Token(Identifier) |moduleWithConditionOutputsCompletions|
//@[042:00043) | ├─Token(Assignment) |=|
//@[044:00082) | └─PropertyAccessSyntax
//@[044:00080) |   ├─PropertyAccessSyntax
//@[044:00072) |   | ├─VariableAccessSyntax
//@[044:00072) |   | | └─IdentifierSyntax
//@[044:00072) |   | |   └─Token(Identifier) |modAEmptyInputsWithCondition|
//@[072:00073) |   | ├─Token(Dot) |.|
//@[073:00080) |   | └─IdentifierSyntax
//@[073:00080) |   |   └─Token(Identifier) |outputs|
//@[080:00081) |   ├─Token(Dot) |.|
//@[081:00082) |   └─IdentifierSyntax
//@[081:00082) |     └─Token(Identifier) |s|
//@[082:00084) ├─Token(NewLine) |\n\n|

module modAUnspecifiedInputs './modulea.bicep' = {
//@[000:00180) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00028) | ├─IdentifierSyntax
//@[007:00028) | | └─Token(Identifier) |modAUnspecifiedInputs|
//@[029:00046) | ├─StringSyntax
//@[029:00046) | | └─Token(StringComplete) |'./modulea.bicep'|
//@[047:00048) | ├─Token(Assignment) |=|
//@[049:00180) | └─ObjectSyntax
//@[049:00050) |   ├─Token(LeftBrace) |{|
//@[050:00051) |   ├─Token(NewLine) |\n|
  name: 'modAUnspecifiedInputs'
//@[002:00031) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00031) |   | └─StringSyntax
//@[008:00031) |   |   └─Token(StringComplete) |'modAUnspecifiedInputs'|
//@[031:00032) |   ├─Token(NewLine) |\n|
  params: {
//@[002:00095) |   ├─ObjectPropertySyntax
//@[002:00008) |   | ├─IdentifierSyntax
//@[002:00008) |   | | └─Token(Identifier) |params|
//@[008:00009) |   | ├─Token(Colon) |:|
//@[010:00095) |   | └─ObjectSyntax
//@[010:00011) |   |   ├─Token(LeftBrace) |{|
//@[011:00012) |   |   ├─Token(NewLine) |\n|
    stringParamB: ''
//@[004:00020) |   |   ├─ObjectPropertySyntax
//@[004:00016) |   |   | ├─IdentifierSyntax
//@[004:00016) |   |   | | └─Token(Identifier) |stringParamB|
//@[016:00017) |   |   | ├─Token(Colon) |:|
//@[018:00020) |   |   | └─StringSyntax
//@[018:00020) |   |   |   └─Token(StringComplete) |''|
//@[020:00021) |   |   ├─Token(NewLine) |\n|
    objParam: {}
//@[004:00016) |   |   ├─ObjectPropertySyntax
//@[004:00012) |   |   | ├─IdentifierSyntax
//@[004:00012) |   |   | | └─Token(Identifier) |objParam|
//@[012:00013) |   |   | ├─Token(Colon) |:|
//@[014:00016) |   |   | └─ObjectSyntax
//@[014:00015) |   |   |   ├─Token(LeftBrace) |{|
//@[015:00016) |   |   |   └─Token(RightBrace) |}|
//@[016:00017) |   |   ├─Token(NewLine) |\n|
    objArray: []
//@[004:00016) |   |   ├─ObjectPropertySyntax
//@[004:00012) |   |   | ├─IdentifierSyntax
//@[004:00012) |   |   | | └─Token(Identifier) |objArray|
//@[012:00013) |   |   | ├─Token(Colon) |:|
//@[014:00016) |   |   | └─ArraySyntax
//@[014:00015) |   |   |   ├─Token(LeftSquare) |[|
//@[015:00016) |   |   |   └─Token(RightSquare) |]|
//@[016:00017) |   |   ├─Token(NewLine) |\n|
    unspecifiedInput: ''
//@[004:00024) |   |   ├─ObjectPropertySyntax
//@[004:00020) |   |   | ├─IdentifierSyntax
//@[004:00020) |   |   | | └─Token(Identifier) |unspecifiedInput|
//@[020:00021) |   |   | ├─Token(Colon) |:|
//@[022:00024) |   |   | └─StringSyntax
//@[022:00024) |   |   |   └─Token(StringComplete) |''|
//@[024:00025) |   |   ├─Token(NewLine) |\n|
  }
//@[002:00003) |   |   └─Token(RightBrace) |}|
//@[003:00004) |   ├─Token(NewLine) |\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00003) ├─Token(NewLine) |\n\n|

var unspecifiedOutput = modAUnspecifiedInputs.outputs.test
//@[000:00058) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00021) | ├─IdentifierSyntax
//@[004:00021) | | └─Token(Identifier) |unspecifiedOutput|
//@[022:00023) | ├─Token(Assignment) |=|
//@[024:00058) | └─PropertyAccessSyntax
//@[024:00053) |   ├─PropertyAccessSyntax
//@[024:00045) |   | ├─VariableAccessSyntax
//@[024:00045) |   | | └─IdentifierSyntax
//@[024:00045) |   | |   └─Token(Identifier) |modAUnspecifiedInputs|
//@[045:00046) |   | ├─Token(Dot) |.|
//@[046:00053) |   | └─IdentifierSyntax
//@[046:00053) |   |   └─Token(Identifier) |outputs|
//@[053:00054) |   ├─Token(Dot) |.|
//@[054:00058) |   └─IdentifierSyntax
//@[054:00058) |     └─Token(Identifier) |test|
//@[058:00060) ├─Token(NewLine) |\n\n|

module modCycle './cycle.bicep' = {
//@[000:00040) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00015) | ├─IdentifierSyntax
//@[007:00015) | | └─Token(Identifier) |modCycle|
//@[016:00031) | ├─StringSyntax
//@[016:00031) | | └─Token(StringComplete) |'./cycle.bicep'|
//@[032:00033) | ├─Token(Assignment) |=|
//@[034:00040) | └─ObjectSyntax
//@[034:00035) |   ├─Token(LeftBrace) |{|
//@[035:00036) |   ├─Token(NewLine) |\n|
  
//@[002:00003) |   ├─Token(NewLine) |\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00003) ├─Token(NewLine) |\n\n|

module moduleWithEmptyPath '' = {
//@[000:00035) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00026) | ├─IdentifierSyntax
//@[007:00026) | | └─Token(Identifier) |moduleWithEmptyPath|
//@[027:00029) | ├─StringSyntax
//@[027:00029) | | └─Token(StringComplete) |''|
//@[030:00031) | ├─Token(Assignment) |=|
//@[032:00035) | └─ObjectSyntax
//@[032:00033) |   ├─Token(LeftBrace) |{|
//@[033:00034) |   ├─Token(NewLine) |\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00003) ├─Token(NewLine) |\n\n|

module moduleWithAbsolutePath '/abc/def.bicep' = {
//@[000:00052) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00029) | ├─IdentifierSyntax
//@[007:00029) | | └─Token(Identifier) |moduleWithAbsolutePath|
//@[030:00046) | ├─StringSyntax
//@[030:00046) | | └─Token(StringComplete) |'/abc/def.bicep'|
//@[047:00048) | ├─Token(Assignment) |=|
//@[049:00052) | └─ObjectSyntax
//@[049:00050) |   ├─Token(LeftBrace) |{|
//@[050:00051) |   ├─Token(NewLine) |\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00003) ├─Token(NewLine) |\n\n|

module moduleWithBackslash 'child\\file.bicep' = {
//@[000:00052) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00026) | ├─IdentifierSyntax
//@[007:00026) | | └─Token(Identifier) |moduleWithBackslash|
//@[027:00046) | ├─StringSyntax
//@[027:00046) | | └─Token(StringComplete) |'child\\file.bicep'|
//@[047:00048) | ├─Token(Assignment) |=|
//@[049:00052) | └─ObjectSyntax
//@[049:00050) |   ├─Token(LeftBrace) |{|
//@[050:00051) |   ├─Token(NewLine) |\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00003) ├─Token(NewLine) |\n\n|

module moduleWithInvalidChar 'child/fi|le.bicep' = {
//@[000:00054) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00028) | ├─IdentifierSyntax
//@[007:00028) | | └─Token(Identifier) |moduleWithInvalidChar|
//@[029:00048) | ├─StringSyntax
//@[029:00048) | | └─Token(StringComplete) |'child/fi|le.bicep'|
//@[049:00050) | ├─Token(Assignment) |=|
//@[051:00054) | └─ObjectSyntax
//@[051:00052) |   ├─Token(LeftBrace) |{|
//@[052:00053) |   ├─Token(NewLine) |\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00003) ├─Token(NewLine) |\n\n|

module moduleWithInvalidTerminatorChar 'child/test.' = {
//@[000:00058) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00038) | ├─IdentifierSyntax
//@[007:00038) | | └─Token(Identifier) |moduleWithInvalidTerminatorChar|
//@[039:00052) | ├─StringSyntax
//@[039:00052) | | └─Token(StringComplete) |'child/test.'|
//@[053:00054) | ├─Token(Assignment) |=|
//@[055:00058) | └─ObjectSyntax
//@[055:00056) |   ├─Token(LeftBrace) |{|
//@[056:00057) |   ├─Token(NewLine) |\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00003) ├─Token(NewLine) |\n\n|

module moduleWithValidScope './empty.bicep' = {
//@[000:00080) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00027) | ├─IdentifierSyntax
//@[007:00027) | | └─Token(Identifier) |moduleWithValidScope|
//@[028:00043) | ├─StringSyntax
//@[028:00043) | | └─Token(StringComplete) |'./empty.bicep'|
//@[044:00045) | ├─Token(Assignment) |=|
//@[046:00080) | └─ObjectSyntax
//@[046:00047) |   ├─Token(LeftBrace) |{|
//@[047:00048) |   ├─Token(NewLine) |\n|
  name: 'moduleWithValidScope'
//@[002:00030) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00030) |   | └─StringSyntax
//@[008:00030) |   |   └─Token(StringComplete) |'moduleWithValidScope'|
//@[030:00031) |   ├─Token(NewLine) |\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00003) ├─Token(NewLine) |\n\n|

module moduleWithInvalidScope './empty.bicep' = {
//@[000:00114) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00029) | ├─IdentifierSyntax
//@[007:00029) | | └─Token(Identifier) |moduleWithInvalidScope|
//@[030:00045) | ├─StringSyntax
//@[030:00045) | | └─Token(StringComplete) |'./empty.bicep'|
//@[046:00047) | ├─Token(Assignment) |=|
//@[048:00114) | └─ObjectSyntax
//@[048:00049) |   ├─Token(LeftBrace) |{|
//@[049:00050) |   ├─Token(NewLine) |\n|
  name: 'moduleWithInvalidScope'
//@[002:00032) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00032) |   | └─StringSyntax
//@[008:00032) |   |   └─Token(StringComplete) |'moduleWithInvalidScope'|
//@[032:00033) |   ├─Token(NewLine) |\n|
  scope: moduleWithValidScope
//@[002:00029) |   ├─ObjectPropertySyntax
//@[002:00007) |   | ├─IdentifierSyntax
//@[002:00007) |   | | └─Token(Identifier) |scope|
//@[007:00008) |   | ├─Token(Colon) |:|
//@[009:00029) |   | └─VariableAccessSyntax
//@[009:00029) |   |   └─IdentifierSyntax
//@[009:00029) |   |     └─Token(Identifier) |moduleWithValidScope|
//@[029:00030) |   ├─Token(NewLine) |\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00003) ├─Token(NewLine) |\n\n|

module moduleWithMissingRequiredScope './subscription_empty.bicep' = {
//@[000:00113) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00037) | ├─IdentifierSyntax
//@[007:00037) | | └─Token(Identifier) |moduleWithMissingRequiredScope|
//@[038:00066) | ├─StringSyntax
//@[038:00066) | | └─Token(StringComplete) |'./subscription_empty.bicep'|
//@[067:00068) | ├─Token(Assignment) |=|
//@[069:00113) | └─ObjectSyntax
//@[069:00070) |   ├─Token(LeftBrace) |{|
//@[070:00071) |   ├─Token(NewLine) |\n|
  name: 'moduleWithMissingRequiredScope'
//@[002:00040) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00040) |   | └─StringSyntax
//@[008:00040) |   |   └─Token(StringComplete) |'moduleWithMissingRequiredScope'|
//@[040:00041) |   ├─Token(NewLine) |\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00003) ├─Token(NewLine) |\n\n|

module moduleWithInvalidScope2 './empty.bicep' = {
//@[000:00113) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00030) | ├─IdentifierSyntax
//@[007:00030) | | └─Token(Identifier) |moduleWithInvalidScope2|
//@[031:00046) | ├─StringSyntax
//@[031:00046) | | └─Token(StringComplete) |'./empty.bicep'|
//@[047:00048) | ├─Token(Assignment) |=|
//@[049:00113) | └─ObjectSyntax
//@[049:00050) |   ├─Token(LeftBrace) |{|
//@[050:00051) |   ├─Token(NewLine) |\n|
  name: 'moduleWithInvalidScope2'
//@[002:00033) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00033) |   | └─StringSyntax
//@[008:00033) |   |   └─Token(StringComplete) |'moduleWithInvalidScope2'|
//@[033:00034) |   ├─Token(NewLine) |\n|
  scope: managementGroup()
//@[002:00026) |   ├─ObjectPropertySyntax
//@[002:00007) |   | ├─IdentifierSyntax
//@[002:00007) |   | | └─Token(Identifier) |scope|
//@[007:00008) |   | ├─Token(Colon) |:|
//@[009:00026) |   | └─FunctionCallSyntax
//@[009:00024) |   |   ├─IdentifierSyntax
//@[009:00024) |   |   | └─Token(Identifier) |managementGroup|
//@[024:00025) |   |   ├─Token(LeftParen) |(|
//@[025:00026) |   |   └─Token(RightParen) |)|
//@[026:00027) |   ├─Token(NewLine) |\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00003) ├─Token(NewLine) |\n\n|

module moduleWithUnsupprtedScope1 './mg_empty.bicep' = {
//@[000:00122) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00033) | ├─IdentifierSyntax
//@[007:00033) | | └─Token(Identifier) |moduleWithUnsupprtedScope1|
//@[034:00052) | ├─StringSyntax
//@[034:00052) | | └─Token(StringComplete) |'./mg_empty.bicep'|
//@[053:00054) | ├─Token(Assignment) |=|
//@[055:00122) | └─ObjectSyntax
//@[055:00056) |   ├─Token(LeftBrace) |{|
//@[056:00057) |   ├─Token(NewLine) |\n|
  name: 'moduleWithUnsupprtedScope1'
//@[002:00036) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00036) |   | └─StringSyntax
//@[008:00036) |   |   └─Token(StringComplete) |'moduleWithUnsupprtedScope1'|
//@[036:00037) |   ├─Token(NewLine) |\n|
  scope: managementGroup()
//@[002:00026) |   ├─ObjectPropertySyntax
//@[002:00007) |   | ├─IdentifierSyntax
//@[002:00007) |   | | └─Token(Identifier) |scope|
//@[007:00008) |   | ├─Token(Colon) |:|
//@[009:00026) |   | └─FunctionCallSyntax
//@[009:00024) |   |   ├─IdentifierSyntax
//@[009:00024) |   |   | └─Token(Identifier) |managementGroup|
//@[024:00025) |   |   ├─Token(LeftParen) |(|
//@[025:00026) |   |   └─Token(RightParen) |)|
//@[026:00027) |   ├─Token(NewLine) |\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00003) ├─Token(NewLine) |\n\n|

module moduleWithUnsupprtedScope2 './mg_empty.bicep' = {
//@[000:00126) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00033) | ├─IdentifierSyntax
//@[007:00033) | | └─Token(Identifier) |moduleWithUnsupprtedScope2|
//@[034:00052) | ├─StringSyntax
//@[034:00052) | | └─Token(StringComplete) |'./mg_empty.bicep'|
//@[053:00054) | ├─Token(Assignment) |=|
//@[055:00126) | └─ObjectSyntax
//@[055:00056) |   ├─Token(LeftBrace) |{|
//@[056:00057) |   ├─Token(NewLine) |\n|
  name: 'moduleWithUnsupprtedScope2'
//@[002:00036) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00036) |   | └─StringSyntax
//@[008:00036) |   |   └─Token(StringComplete) |'moduleWithUnsupprtedScope2'|
//@[036:00037) |   ├─Token(NewLine) |\n|
  scope: managementGroup('MG')
//@[002:00030) |   ├─ObjectPropertySyntax
//@[002:00007) |   | ├─IdentifierSyntax
//@[002:00007) |   | | └─Token(Identifier) |scope|
//@[007:00008) |   | ├─Token(Colon) |:|
//@[009:00030) |   | └─FunctionCallSyntax
//@[009:00024) |   |   ├─IdentifierSyntax
//@[009:00024) |   |   | └─Token(Identifier) |managementGroup|
//@[024:00025) |   |   ├─Token(LeftParen) |(|
//@[025:00029) |   |   ├─FunctionArgumentSyntax
//@[025:00029) |   |   | └─StringSyntax
//@[025:00029) |   |   |   └─Token(StringComplete) |'MG'|
//@[029:00030) |   |   └─Token(RightParen) |)|
//@[030:00031) |   ├─Token(NewLine) |\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00003) ├─Token(NewLine) |\n\n|

module moduleWithBadScope './empty.bicep' = {
//@[000:00099) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00025) | ├─IdentifierSyntax
//@[007:00025) | | └─Token(Identifier) |moduleWithBadScope|
//@[026:00041) | ├─StringSyntax
//@[026:00041) | | └─Token(StringComplete) |'./empty.bicep'|
//@[042:00043) | ├─Token(Assignment) |=|
//@[044:00099) | └─ObjectSyntax
//@[044:00045) |   ├─Token(LeftBrace) |{|
//@[045:00046) |   ├─Token(NewLine) |\n|
  name: 'moduleWithBadScope'
//@[002:00028) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00028) |   | └─StringSyntax
//@[008:00028) |   |   └─Token(StringComplete) |'moduleWithBadScope'|
//@[028:00029) |   ├─Token(NewLine) |\n|
  scope: 'stringScope'
//@[002:00022) |   ├─ObjectPropertySyntax
//@[002:00007) |   | ├─IdentifierSyntax
//@[002:00007) |   | | └─Token(Identifier) |scope|
//@[007:00008) |   | ├─Token(Colon) |:|
//@[009:00022) |   | └─StringSyntax
//@[009:00022) |   |   └─Token(StringComplete) |'stringScope'|
//@[022:00023) |   ├─Token(NewLine) |\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00003) ├─Token(NewLine) |\n\n|

resource runtimeValidRes1 'Microsoft.Storage/storageAccounts@2019-06-01' = {
//@[000:00190) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00025) | ├─IdentifierSyntax
//@[009:00025) | | └─Token(Identifier) |runtimeValidRes1|
//@[026:00072) | ├─StringSyntax
//@[026:00072) | | └─Token(StringComplete) |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[073:00074) | ├─Token(Assignment) |=|
//@[075:00190) | └─ObjectSyntax
//@[075:00076) |   ├─Token(LeftBrace) |{|
//@[076:00077) |   ├─Token(NewLine) |\n|
  name: 'runtimeValidRes1Name'
//@[002:00030) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00030) |   | └─StringSyntax
//@[008:00030) |   |   └─Token(StringComplete) |'runtimeValidRes1Name'|
//@[030:00031) |   ├─Token(NewLine) |\n|
  location: 'westeurope'
//@[002:00024) |   ├─ObjectPropertySyntax
//@[002:00010) |   | ├─IdentifierSyntax
//@[002:00010) |   | | └─Token(Identifier) |location|
//@[010:00011) |   | ├─Token(Colon) |:|
//@[012:00024) |   | └─StringSyntax
//@[012:00024) |   |   └─Token(StringComplete) |'westeurope'|
//@[024:00025) |   ├─Token(NewLine) |\n|
  kind: 'Storage'
//@[002:00017) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |kind|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00017) |   | └─StringSyntax
//@[008:00017) |   |   └─Token(StringComplete) |'Storage'|
//@[017:00018) |   ├─Token(NewLine) |\n|
  sku: {
//@[002:00037) |   ├─ObjectPropertySyntax
//@[002:00005) |   | ├─IdentifierSyntax
//@[002:00005) |   | | └─Token(Identifier) |sku|
//@[005:00006) |   | ├─Token(Colon) |:|
//@[007:00037) |   | └─ObjectSyntax
//@[007:00008) |   |   ├─Token(LeftBrace) |{|
//@[008:00009) |   |   ├─Token(NewLine) |\n|
    name: 'Standard_GRS'
//@[004:00024) |   |   ├─ObjectPropertySyntax
//@[004:00008) |   |   | ├─IdentifierSyntax
//@[004:00008) |   |   | | └─Token(Identifier) |name|
//@[008:00009) |   |   | ├─Token(Colon) |:|
//@[010:00024) |   |   | └─StringSyntax
//@[010:00024) |   |   |   └─Token(StringComplete) |'Standard_GRS'|
//@[024:00025) |   |   ├─Token(NewLine) |\n|
  }
//@[002:00003) |   |   └─Token(RightBrace) |}|
//@[003:00004) |   ├─Token(NewLine) |\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00003) ├─Token(NewLine) |\n\n|

module runtimeValidModule1 'empty.bicep' = {
//@[000:00136) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00026) | ├─IdentifierSyntax
//@[007:00026) | | └─Token(Identifier) |runtimeValidModule1|
//@[027:00040) | ├─StringSyntax
//@[027:00040) | | └─Token(StringComplete) |'empty.bicep'|
//@[041:00042) | ├─Token(Assignment) |=|
//@[043:00136) | └─ObjectSyntax
//@[043:00044) |   ├─Token(LeftBrace) |{|
//@[044:00045) |   ├─Token(NewLine) |\n|
  name: concat(concat(runtimeValidRes1.id, runtimeValidRes1.name), runtimeValidRes1.type)
//@[002:00089) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00089) |   | └─FunctionCallSyntax
//@[008:00014) |   |   ├─IdentifierSyntax
//@[008:00014) |   |   | └─Token(Identifier) |concat|
//@[014:00015) |   |   ├─Token(LeftParen) |(|
//@[015:00065) |   |   ├─FunctionArgumentSyntax
//@[015:00065) |   |   | └─FunctionCallSyntax
//@[015:00021) |   |   |   ├─IdentifierSyntax
//@[015:00021) |   |   |   | └─Token(Identifier) |concat|
//@[021:00022) |   |   |   ├─Token(LeftParen) |(|
//@[022:00041) |   |   |   ├─FunctionArgumentSyntax
//@[022:00041) |   |   |   | └─PropertyAccessSyntax
//@[022:00038) |   |   |   |   ├─VariableAccessSyntax
//@[022:00038) |   |   |   |   | └─IdentifierSyntax
//@[022:00038) |   |   |   |   |   └─Token(Identifier) |runtimeValidRes1|
//@[038:00039) |   |   |   |   ├─Token(Dot) |.|
//@[039:00041) |   |   |   |   └─IdentifierSyntax
//@[039:00041) |   |   |   |     └─Token(Identifier) |id|
//@[041:00042) |   |   |   ├─Token(Comma) |,|
//@[043:00064) |   |   |   ├─FunctionArgumentSyntax
//@[043:00064) |   |   |   | └─PropertyAccessSyntax
//@[043:00059) |   |   |   |   ├─VariableAccessSyntax
//@[043:00059) |   |   |   |   | └─IdentifierSyntax
//@[043:00059) |   |   |   |   |   └─Token(Identifier) |runtimeValidRes1|
//@[059:00060) |   |   |   |   ├─Token(Dot) |.|
//@[060:00064) |   |   |   |   └─IdentifierSyntax
//@[060:00064) |   |   |   |     └─Token(Identifier) |name|
//@[064:00065) |   |   |   └─Token(RightParen) |)|
//@[065:00066) |   |   ├─Token(Comma) |,|
//@[067:00088) |   |   ├─FunctionArgumentSyntax
//@[067:00088) |   |   | └─PropertyAccessSyntax
//@[067:00083) |   |   |   ├─VariableAccessSyntax
//@[067:00083) |   |   |   | └─IdentifierSyntax
//@[067:00083) |   |   |   |   └─Token(Identifier) |runtimeValidRes1|
//@[083:00084) |   |   |   ├─Token(Dot) |.|
//@[084:00088) |   |   |   └─IdentifierSyntax
//@[084:00088) |   |   |     └─Token(Identifier) |type|
//@[088:00089) |   |   └─Token(RightParen) |)|
//@[089:00090) |   ├─Token(NewLine) |\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00003) ├─Token(NewLine) |\n\n|

module runtimeInvalidModule1 'empty.bicep' = {
//@[000:00082) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00028) | ├─IdentifierSyntax
//@[007:00028) | | └─Token(Identifier) |runtimeInvalidModule1|
//@[029:00042) | ├─StringSyntax
//@[029:00042) | | └─Token(StringComplete) |'empty.bicep'|
//@[043:00044) | ├─Token(Assignment) |=|
//@[045:00082) | └─ObjectSyntax
//@[045:00046) |   ├─Token(LeftBrace) |{|
//@[046:00047) |   ├─Token(NewLine) |\n|
  name: runtimeValidRes1.location
//@[002:00033) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00033) |   | └─PropertyAccessSyntax
//@[008:00024) |   |   ├─VariableAccessSyntax
//@[008:00024) |   |   | └─IdentifierSyntax
//@[008:00024) |   |   |   └─Token(Identifier) |runtimeValidRes1|
//@[024:00025) |   |   ├─Token(Dot) |.|
//@[025:00033) |   |   └─IdentifierSyntax
//@[025:00033) |   |     └─Token(Identifier) |location|
//@[033:00034) |   ├─Token(NewLine) |\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00003) ├─Token(NewLine) |\n\n|

module runtimeInvalidModule2 'empty.bicep' = {
//@[000:00085) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00028) | ├─IdentifierSyntax
//@[007:00028) | | └─Token(Identifier) |runtimeInvalidModule2|
//@[029:00042) | ├─StringSyntax
//@[029:00042) | | └─Token(StringComplete) |'empty.bicep'|
//@[043:00044) | ├─Token(Assignment) |=|
//@[045:00085) | └─ObjectSyntax
//@[045:00046) |   ├─Token(LeftBrace) |{|
//@[046:00047) |   ├─Token(NewLine) |\n|
  name: runtimeValidRes1['location']
//@[002:00036) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00036) |   | └─ArrayAccessSyntax
//@[008:00024) |   |   ├─VariableAccessSyntax
//@[008:00024) |   |   | └─IdentifierSyntax
//@[008:00024) |   |   |   └─Token(Identifier) |runtimeValidRes1|
//@[024:00025) |   |   ├─Token(LeftSquare) |[|
//@[025:00035) |   |   ├─StringSyntax
//@[025:00035) |   |   | └─Token(StringComplete) |'location'|
//@[035:00036) |   |   └─Token(RightSquare) |]|
//@[036:00037) |   ├─Token(NewLine) |\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00003) ├─Token(NewLine) |\n\n|

module runtimeInvalidModule3 'empty.bicep' = {
//@[000:00082) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00028) | ├─IdentifierSyntax
//@[007:00028) | | └─Token(Identifier) |runtimeInvalidModule3|
//@[029:00042) | ├─StringSyntax
//@[029:00042) | | └─Token(StringComplete) |'empty.bicep'|
//@[043:00044) | ├─Token(Assignment) |=|
//@[045:00082) | └─ObjectSyntax
//@[045:00046) |   ├─Token(LeftBrace) |{|
//@[046:00047) |   ├─Token(NewLine) |\n|
  name: runtimeValidRes1.sku.name
//@[002:00033) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00033) |   | └─PropertyAccessSyntax
//@[008:00028) |   |   ├─PropertyAccessSyntax
//@[008:00024) |   |   | ├─VariableAccessSyntax
//@[008:00024) |   |   | | └─IdentifierSyntax
//@[008:00024) |   |   | |   └─Token(Identifier) |runtimeValidRes1|
//@[024:00025) |   |   | ├─Token(Dot) |.|
//@[025:00028) |   |   | └─IdentifierSyntax
//@[025:00028) |   |   |   └─Token(Identifier) |sku|
//@[028:00029) |   |   ├─Token(Dot) |.|
//@[029:00033) |   |   └─IdentifierSyntax
//@[029:00033) |   |     └─Token(Identifier) |name|
//@[033:00034) |   ├─Token(NewLine) |\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00003) ├─Token(NewLine) |\n\n|

module runtimeInvalidModule4 'empty.bicep' = {
//@[000:00085) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00028) | ├─IdentifierSyntax
//@[007:00028) | | └─Token(Identifier) |runtimeInvalidModule4|
//@[029:00042) | ├─StringSyntax
//@[029:00042) | | └─Token(StringComplete) |'empty.bicep'|
//@[043:00044) | ├─Token(Assignment) |=|
//@[045:00085) | └─ObjectSyntax
//@[045:00046) |   ├─Token(LeftBrace) |{|
//@[046:00047) |   ├─Token(NewLine) |\n|
  name: runtimeValidRes1.sku['name']
//@[002:00036) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00036) |   | └─ArrayAccessSyntax
//@[008:00028) |   |   ├─PropertyAccessSyntax
//@[008:00024) |   |   | ├─VariableAccessSyntax
//@[008:00024) |   |   | | └─IdentifierSyntax
//@[008:00024) |   |   | |   └─Token(Identifier) |runtimeValidRes1|
//@[024:00025) |   |   | ├─Token(Dot) |.|
//@[025:00028) |   |   | └─IdentifierSyntax
//@[025:00028) |   |   |   └─Token(Identifier) |sku|
//@[028:00029) |   |   ├─Token(LeftSquare) |[|
//@[029:00035) |   |   ├─StringSyntax
//@[029:00035) |   |   | └─Token(StringComplete) |'name'|
//@[035:00036) |   |   └─Token(RightSquare) |]|
//@[036:00037) |   ├─Token(NewLine) |\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00003) ├─Token(NewLine) |\n\n|

module runtimeInvalidModule5 'empty.bicep' = {
//@[000:00088) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00028) | ├─IdentifierSyntax
//@[007:00028) | | └─Token(Identifier) |runtimeInvalidModule5|
//@[029:00042) | ├─StringSyntax
//@[029:00042) | | └─Token(StringComplete) |'empty.bicep'|
//@[043:00044) | ├─Token(Assignment) |=|
//@[045:00088) | └─ObjectSyntax
//@[045:00046) |   ├─Token(LeftBrace) |{|
//@[046:00047) |   ├─Token(NewLine) |\n|
  name: runtimeValidRes1['sku']['name']
//@[002:00039) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00039) |   | └─ArrayAccessSyntax
//@[008:00031) |   |   ├─ArrayAccessSyntax
//@[008:00024) |   |   | ├─VariableAccessSyntax
//@[008:00024) |   |   | | └─IdentifierSyntax
//@[008:00024) |   |   | |   └─Token(Identifier) |runtimeValidRes1|
//@[024:00025) |   |   | ├─Token(LeftSquare) |[|
//@[025:00030) |   |   | ├─StringSyntax
//@[025:00030) |   |   | | └─Token(StringComplete) |'sku'|
//@[030:00031) |   |   | └─Token(RightSquare) |]|
//@[031:00032) |   |   ├─Token(LeftSquare) |[|
//@[032:00038) |   |   ├─StringSyntax
//@[032:00038) |   |   | └─Token(StringComplete) |'name'|
//@[038:00039) |   |   └─Token(RightSquare) |]|
//@[039:00040) |   ├─Token(NewLine) |\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00003) ├─Token(NewLine) |\n\n|

module runtimeInvalidModule6 'empty.bicep' = {
//@[000:00085) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00028) | ├─IdentifierSyntax
//@[007:00028) | | └─Token(Identifier) |runtimeInvalidModule6|
//@[029:00042) | ├─StringSyntax
//@[029:00042) | | └─Token(StringComplete) |'empty.bicep'|
//@[043:00044) | ├─Token(Assignment) |=|
//@[045:00085) | └─ObjectSyntax
//@[045:00046) |   ├─Token(LeftBrace) |{|
//@[046:00047) |   ├─Token(NewLine) |\n|
  name: runtimeValidRes1['sku'].name
//@[002:00036) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00036) |   | └─PropertyAccessSyntax
//@[008:00031) |   |   ├─ArrayAccessSyntax
//@[008:00024) |   |   | ├─VariableAccessSyntax
//@[008:00024) |   |   | | └─IdentifierSyntax
//@[008:00024) |   |   | |   └─Token(Identifier) |runtimeValidRes1|
//@[024:00025) |   |   | ├─Token(LeftSquare) |[|
//@[025:00030) |   |   | ├─StringSyntax
//@[025:00030) |   |   | | └─Token(StringComplete) |'sku'|
//@[030:00031) |   |   | └─Token(RightSquare) |]|
//@[031:00032) |   |   ├─Token(Dot) |.|
//@[032:00036) |   |   └─IdentifierSyntax
//@[032:00036) |   |     └─Token(Identifier) |name|
//@[036:00037) |   ├─Token(NewLine) |\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00003) ├─Token(NewLine) |\n\n|

module singleModuleForRuntimeCheck 'modulea.bicep' = {
//@[000:00071) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00034) | ├─IdentifierSyntax
//@[007:00034) | | └─Token(Identifier) |singleModuleForRuntimeCheck|
//@[035:00050) | ├─StringSyntax
//@[035:00050) | | └─Token(StringComplete) |'modulea.bicep'|
//@[051:00052) | ├─Token(Assignment) |=|
//@[053:00071) | └─ObjectSyntax
//@[053:00054) |   ├─Token(LeftBrace) |{|
//@[054:00055) |   ├─Token(NewLine) |\n|
  name: 'test'
//@[002:00014) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00014) |   | └─StringSyntax
//@[008:00014) |   |   └─Token(StringComplete) |'test'|
//@[014:00015) |   ├─Token(NewLine) |\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00003) ├─Token(NewLine) |\n\n|

var moduleRuntimeCheck = singleModuleForRuntimeCheck.outputs.stringOutputA
//@[000:00074) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00022) | ├─IdentifierSyntax
//@[004:00022) | | └─Token(Identifier) |moduleRuntimeCheck|
//@[023:00024) | ├─Token(Assignment) |=|
//@[025:00074) | └─PropertyAccessSyntax
//@[025:00060) |   ├─PropertyAccessSyntax
//@[025:00052) |   | ├─VariableAccessSyntax
//@[025:00052) |   | | └─IdentifierSyntax
//@[025:00052) |   | |   └─Token(Identifier) |singleModuleForRuntimeCheck|
//@[052:00053) |   | ├─Token(Dot) |.|
//@[053:00060) |   | └─IdentifierSyntax
//@[053:00060) |   |   └─Token(Identifier) |outputs|
//@[060:00061) |   ├─Token(Dot) |.|
//@[061:00074) |   └─IdentifierSyntax
//@[061:00074) |     └─Token(Identifier) |stringOutputA|
//@[074:00075) ├─Token(NewLine) |\n|
var moduleRuntimeCheck2 = moduleRuntimeCheck
//@[000:00044) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00023) | ├─IdentifierSyntax
//@[004:00023) | | └─Token(Identifier) |moduleRuntimeCheck2|
//@[024:00025) | ├─Token(Assignment) |=|
//@[026:00044) | └─VariableAccessSyntax
//@[026:00044) |   └─IdentifierSyntax
//@[026:00044) |     └─Token(Identifier) |moduleRuntimeCheck|
//@[044:00046) ├─Token(NewLine) |\n\n|

module moduleLoopForRuntimeCheck 'modulea.bicep' = [for thing in []: {
//@[000:00101) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00032) | ├─IdentifierSyntax
//@[007:00032) | | └─Token(Identifier) |moduleLoopForRuntimeCheck|
//@[033:00048) | ├─StringSyntax
//@[033:00048) | | └─Token(StringComplete) |'modulea.bicep'|
//@[049:00050) | ├─Token(Assignment) |=|
//@[051:00101) | └─ForSyntax
//@[051:00052) |   ├─Token(LeftSquare) |[|
//@[052:00055) |   ├─Token(Identifier) |for|
//@[056:00061) |   ├─LocalVariableSyntax
//@[056:00061) |   | └─IdentifierSyntax
//@[056:00061) |   |   └─Token(Identifier) |thing|
//@[062:00064) |   ├─Token(Identifier) |in|
//@[065:00067) |   ├─ArraySyntax
//@[065:00066) |   | ├─Token(LeftSquare) |[|
//@[066:00067) |   | └─Token(RightSquare) |]|
//@[067:00068) |   ├─Token(Colon) |:|
//@[069:00100) |   ├─ObjectSyntax
//@[069:00070) |   | ├─Token(LeftBrace) |{|
//@[070:00071) |   | ├─Token(NewLine) |\n|
  name: moduleRuntimeCheck2
//@[002:00027) |   | ├─ObjectPropertySyntax
//@[002:00006) |   | | ├─IdentifierSyntax
//@[002:00006) |   | | | └─Token(Identifier) |name|
//@[006:00007) |   | | ├─Token(Colon) |:|
//@[008:00027) |   | | └─VariableAccessSyntax
//@[008:00027) |   | |   └─IdentifierSyntax
//@[008:00027) |   | |     └─Token(Identifier) |moduleRuntimeCheck2|
//@[027:00028) |   | ├─Token(NewLine) |\n|
}]
//@[000:00001) |   | └─Token(RightBrace) |}|
//@[001:00002) |   └─Token(RightSquare) |]|
//@[002:00004) ├─Token(NewLine) |\n\n|

var moduleRuntimeCheck3 = moduleLoopForRuntimeCheck[1].outputs.stringOutputB
//@[000:00076) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00023) | ├─IdentifierSyntax
//@[004:00023) | | └─Token(Identifier) |moduleRuntimeCheck3|
//@[024:00025) | ├─Token(Assignment) |=|
//@[026:00076) | └─PropertyAccessSyntax
//@[026:00062) |   ├─PropertyAccessSyntax
//@[026:00054) |   | ├─ArrayAccessSyntax
//@[026:00051) |   | | ├─VariableAccessSyntax
//@[026:00051) |   | | | └─IdentifierSyntax
//@[026:00051) |   | | |   └─Token(Identifier) |moduleLoopForRuntimeCheck|
//@[051:00052) |   | | ├─Token(LeftSquare) |[|
//@[052:00053) |   | | ├─IntegerLiteralSyntax
//@[052:00053) |   | | | └─Token(Integer) |1|
//@[053:00054) |   | | └─Token(RightSquare) |]|
//@[054:00055) |   | ├─Token(Dot) |.|
//@[055:00062) |   | └─IdentifierSyntax
//@[055:00062) |   |   └─Token(Identifier) |outputs|
//@[062:00063) |   ├─Token(Dot) |.|
//@[063:00076) |   └─IdentifierSyntax
//@[063:00076) |     └─Token(Identifier) |stringOutputB|
//@[076:00077) ├─Token(NewLine) |\n|
var moduleRuntimeCheck4 = moduleRuntimeCheck3
//@[000:00045) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00023) | ├─IdentifierSyntax
//@[004:00023) | | └─Token(Identifier) |moduleRuntimeCheck4|
//@[024:00025) | ├─Token(Assignment) |=|
//@[026:00045) | └─VariableAccessSyntax
//@[026:00045) |   └─IdentifierSyntax
//@[026:00045) |     └─Token(Identifier) |moduleRuntimeCheck3|
//@[045:00046) ├─Token(NewLine) |\n|
module moduleLoopForRuntimeCheck2 'modulea.bicep' = [for thing in []: {
//@[000:00102) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00033) | ├─IdentifierSyntax
//@[007:00033) | | └─Token(Identifier) |moduleLoopForRuntimeCheck2|
//@[034:00049) | ├─StringSyntax
//@[034:00049) | | └─Token(StringComplete) |'modulea.bicep'|
//@[050:00051) | ├─Token(Assignment) |=|
//@[052:00102) | └─ForSyntax
//@[052:00053) |   ├─Token(LeftSquare) |[|
//@[053:00056) |   ├─Token(Identifier) |for|
//@[057:00062) |   ├─LocalVariableSyntax
//@[057:00062) |   | └─IdentifierSyntax
//@[057:00062) |   |   └─Token(Identifier) |thing|
//@[063:00065) |   ├─Token(Identifier) |in|
//@[066:00068) |   ├─ArraySyntax
//@[066:00067) |   | ├─Token(LeftSquare) |[|
//@[067:00068) |   | └─Token(RightSquare) |]|
//@[068:00069) |   ├─Token(Colon) |:|
//@[070:00101) |   ├─ObjectSyntax
//@[070:00071) |   | ├─Token(LeftBrace) |{|
//@[071:00072) |   | ├─Token(NewLine) |\n|
  name: moduleRuntimeCheck4
//@[002:00027) |   | ├─ObjectPropertySyntax
//@[002:00006) |   | | ├─IdentifierSyntax
//@[002:00006) |   | | | └─Token(Identifier) |name|
//@[006:00007) |   | | ├─Token(Colon) |:|
//@[008:00027) |   | | └─VariableAccessSyntax
//@[008:00027) |   | |   └─IdentifierSyntax
//@[008:00027) |   | |     └─Token(Identifier) |moduleRuntimeCheck4|
//@[027:00028) |   | ├─Token(NewLine) |\n|
}]
//@[000:00001) |   | └─Token(RightBrace) |}|
//@[001:00002) |   └─Token(RightSquare) |]|
//@[002:00004) ├─Token(NewLine) |\n\n|

module moduleLoopForRuntimeCheck3 'modulea.bicep' = [for thing in []: {
//@[000:00194) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00033) | ├─IdentifierSyntax
//@[007:00033) | | └─Token(Identifier) |moduleLoopForRuntimeCheck3|
//@[034:00049) | ├─StringSyntax
//@[034:00049) | | └─Token(StringComplete) |'modulea.bicep'|
//@[050:00051) | ├─Token(Assignment) |=|
//@[052:00194) | └─ForSyntax
//@[052:00053) |   ├─Token(LeftSquare) |[|
//@[053:00056) |   ├─Token(Identifier) |for|
//@[057:00062) |   ├─LocalVariableSyntax
//@[057:00062) |   | └─IdentifierSyntax
//@[057:00062) |   |   └─Token(Identifier) |thing|
//@[063:00065) |   ├─Token(Identifier) |in|
//@[066:00068) |   ├─ArraySyntax
//@[066:00067) |   | ├─Token(LeftSquare) |[|
//@[067:00068) |   | └─Token(RightSquare) |]|
//@[068:00069) |   ├─Token(Colon) |:|
//@[070:00193) |   ├─ObjectSyntax
//@[070:00071) |   | ├─Token(LeftBrace) |{|
//@[071:00072) |   | ├─Token(NewLine) |\n|
  name: concat(moduleLoopForRuntimeCheck[1].outputs.stringOutputB, moduleLoopForRuntimeCheck[1].outputs.stringOutputA )
//@[002:00119) |   | ├─ObjectPropertySyntax
//@[002:00006) |   | | ├─IdentifierSyntax
//@[002:00006) |   | | | └─Token(Identifier) |name|
//@[006:00007) |   | | ├─Token(Colon) |:|
//@[008:00119) |   | | └─FunctionCallSyntax
//@[008:00014) |   | |   ├─IdentifierSyntax
//@[008:00014) |   | |   | └─Token(Identifier) |concat|
//@[014:00015) |   | |   ├─Token(LeftParen) |(|
//@[015:00065) |   | |   ├─FunctionArgumentSyntax
//@[015:00065) |   | |   | └─PropertyAccessSyntax
//@[015:00051) |   | |   |   ├─PropertyAccessSyntax
//@[015:00043) |   | |   |   | ├─ArrayAccessSyntax
//@[015:00040) |   | |   |   | | ├─VariableAccessSyntax
//@[015:00040) |   | |   |   | | | └─IdentifierSyntax
//@[015:00040) |   | |   |   | | |   └─Token(Identifier) |moduleLoopForRuntimeCheck|
//@[040:00041) |   | |   |   | | ├─Token(LeftSquare) |[|
//@[041:00042) |   | |   |   | | ├─IntegerLiteralSyntax
//@[041:00042) |   | |   |   | | | └─Token(Integer) |1|
//@[042:00043) |   | |   |   | | └─Token(RightSquare) |]|
//@[043:00044) |   | |   |   | ├─Token(Dot) |.|
//@[044:00051) |   | |   |   | └─IdentifierSyntax
//@[044:00051) |   | |   |   |   └─Token(Identifier) |outputs|
//@[051:00052) |   | |   |   ├─Token(Dot) |.|
//@[052:00065) |   | |   |   └─IdentifierSyntax
//@[052:00065) |   | |   |     └─Token(Identifier) |stringOutputB|
//@[065:00066) |   | |   ├─Token(Comma) |,|
//@[067:00117) |   | |   ├─FunctionArgumentSyntax
//@[067:00117) |   | |   | └─PropertyAccessSyntax
//@[067:00103) |   | |   |   ├─PropertyAccessSyntax
//@[067:00095) |   | |   |   | ├─ArrayAccessSyntax
//@[067:00092) |   | |   |   | | ├─VariableAccessSyntax
//@[067:00092) |   | |   |   | | | └─IdentifierSyntax
//@[067:00092) |   | |   |   | | |   └─Token(Identifier) |moduleLoopForRuntimeCheck|
//@[092:00093) |   | |   |   | | ├─Token(LeftSquare) |[|
//@[093:00094) |   | |   |   | | ├─IntegerLiteralSyntax
//@[093:00094) |   | |   |   | | | └─Token(Integer) |1|
//@[094:00095) |   | |   |   | | └─Token(RightSquare) |]|
//@[095:00096) |   | |   |   | ├─Token(Dot) |.|
//@[096:00103) |   | |   |   | └─IdentifierSyntax
//@[096:00103) |   | |   |   |   └─Token(Identifier) |outputs|
//@[103:00104) |   | |   |   ├─Token(Dot) |.|
//@[104:00117) |   | |   |   └─IdentifierSyntax
//@[104:00117) |   | |   |     └─Token(Identifier) |stringOutputA|
//@[118:00119) |   | |   └─Token(RightParen) |)|
//@[119:00120) |   | ├─Token(NewLine) |\n|
}]
//@[000:00001) |   | └─Token(RightBrace) |}|
//@[001:00002) |   └─Token(RightSquare) |]|
//@[002:00004) ├─Token(NewLine) |\n\n|

module moduleWithDuplicateName1 './empty.bicep' = {
//@[000:00112) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00031) | ├─IdentifierSyntax
//@[007:00031) | | └─Token(Identifier) |moduleWithDuplicateName1|
//@[032:00047) | ├─StringSyntax
//@[032:00047) | | └─Token(StringComplete) |'./empty.bicep'|
//@[048:00049) | ├─Token(Assignment) |=|
//@[050:00112) | └─ObjectSyntax
//@[050:00051) |   ├─Token(LeftBrace) |{|
//@[051:00052) |   ├─Token(NewLine) |\n|
  name: 'moduleWithDuplicateName'
//@[002:00033) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00033) |   | └─StringSyntax
//@[008:00033) |   |   └─Token(StringComplete) |'moduleWithDuplicateName'|
//@[033:00034) |   ├─Token(NewLine) |\n|
  scope: resourceGroup()
//@[002:00024) |   ├─ObjectPropertySyntax
//@[002:00007) |   | ├─IdentifierSyntax
//@[002:00007) |   | | └─Token(Identifier) |scope|
//@[007:00008) |   | ├─Token(Colon) |:|
//@[009:00024) |   | └─FunctionCallSyntax
//@[009:00022) |   |   ├─IdentifierSyntax
//@[009:00022) |   |   | └─Token(Identifier) |resourceGroup|
//@[022:00023) |   |   ├─Token(LeftParen) |(|
//@[023:00024) |   |   └─Token(RightParen) |)|
//@[024:00025) |   ├─Token(NewLine) |\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00003) ├─Token(NewLine) |\n\n|

module moduleWithDuplicateName2 './empty.bicep' = {
//@[000:00087) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00031) | ├─IdentifierSyntax
//@[007:00031) | | └─Token(Identifier) |moduleWithDuplicateName2|
//@[032:00047) | ├─StringSyntax
//@[032:00047) | | └─Token(StringComplete) |'./empty.bicep'|
//@[048:00049) | ├─Token(Assignment) |=|
//@[050:00087) | └─ObjectSyntax
//@[050:00051) |   ├─Token(LeftBrace) |{|
//@[051:00052) |   ├─Token(NewLine) |\n|
  name: 'moduleWithDuplicateName'
//@[002:00033) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00033) |   | └─StringSyntax
//@[008:00033) |   |   └─Token(StringComplete) |'moduleWithDuplicateName'|
//@[033:00034) |   ├─Token(NewLine) |\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00003) ├─Token(NewLine) |\n\n|

// #completionTest(19, 20, 21) -> cwdFileCompletions
//@[052:00053) ├─Token(NewLine) |\n|
module completionB ''
//@[000:00021) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00018) | ├─IdentifierSyntax
//@[007:00018) | | └─Token(Identifier) |completionB|
//@[019:00021) | ├─StringSyntax
//@[019:00021) | | └─Token(StringComplete) |''|
//@[021:00021) | ├─SkippedTriviaSyntax
//@[021:00021) | └─SkippedTriviaSyntax
//@[021:00023) ├─Token(NewLine) |\n\n|

// #completionTest(19, 20, 21) -> cwdFileCompletions
//@[052:00053) ├─Token(NewLine) |\n|
module completionC '' =
//@[000:00023) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00018) | ├─IdentifierSyntax
//@[007:00018) | | └─Token(Identifier) |completionC|
//@[019:00021) | ├─StringSyntax
//@[019:00021) | | └─Token(StringComplete) |''|
//@[022:00023) | ├─Token(Assignment) |=|
//@[023:00023) | └─SkippedTriviaSyntax
//@[023:00025) ├─Token(NewLine) |\n\n|

// #completionTest(19, 20, 21) -> cwdFileCompletions
//@[052:00053) ├─Token(NewLine) |\n|
module completionD '' = {}
//@[000:00026) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00018) | ├─IdentifierSyntax
//@[007:00018) | | └─Token(Identifier) |completionD|
//@[019:00021) | ├─StringSyntax
//@[019:00021) | | └─Token(StringComplete) |''|
//@[022:00023) | ├─Token(Assignment) |=|
//@[024:00026) | └─ObjectSyntax
//@[024:00025) |   ├─Token(LeftBrace) |{|
//@[025:00026) |   └─Token(RightBrace) |}|
//@[026:00028) ├─Token(NewLine) |\n\n|

// #completionTest(19, 20, 21) -> cwdFileCompletions
//@[052:00053) ├─Token(NewLine) |\n|
module completionE '' = {
//@[000:00043) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00018) | ├─IdentifierSyntax
//@[007:00018) | | └─Token(Identifier) |completionE|
//@[019:00021) | ├─StringSyntax
//@[019:00021) | | └─Token(StringComplete) |''|
//@[022:00023) | ├─Token(Assignment) |=|
//@[024:00043) | └─ObjectSyntax
//@[024:00025) |   ├─Token(LeftBrace) |{|
//@[025:00026) |   ├─Token(NewLine) |\n|
  name: 'hello'
//@[002:00015) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00015) |   | └─StringSyntax
//@[008:00015) |   |   └─Token(StringComplete) |'hello'|
//@[015:00016) |   ├─Token(NewLine) |\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00003) ├─Token(NewLine) |\n\n|

// #completionTest(29) -> cwdDotFileCompletions
//@[047:00048) ├─Token(NewLine) |\n|
module cwdFileCompletionA './m'
//@[000:00031) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00025) | ├─IdentifierSyntax
//@[007:00025) | | └─Token(Identifier) |cwdFileCompletionA|
//@[026:00031) | ├─StringSyntax
//@[026:00031) | | └─Token(StringComplete) |'./m'|
//@[031:00031) | ├─SkippedTriviaSyntax
//@[031:00031) | └─SkippedTriviaSyntax
//@[031:00033) ├─Token(NewLine) |\n\n|

// #completionTest(26, 27) -> cwdFileCompletions
//@[048:00049) ├─Token(NewLine) |\n|
module cwdFileCompletionB m
//@[000:00027) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00025) | ├─IdentifierSyntax
//@[007:00025) | | └─Token(Identifier) |cwdFileCompletionB|
//@[026:00027) | ├─SkippedTriviaSyntax
//@[026:00027) | | └─Token(Identifier) |m|
//@[027:00027) | ├─SkippedTriviaSyntax
//@[027:00027) | └─SkippedTriviaSyntax
//@[027:00029) ├─Token(NewLine) |\n\n|

// #completionTest(26, 27, 28, 29) -> cwdFileCompletions
//@[056:00057) ├─Token(NewLine) |\n|
module cwdFileCompletionC 'm'
//@[000:00029) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00025) | ├─IdentifierSyntax
//@[007:00025) | | └─Token(Identifier) |cwdFileCompletionC|
//@[026:00029) | ├─StringSyntax
//@[026:00029) | | └─Token(StringComplete) |'m'|
//@[029:00029) | ├─SkippedTriviaSyntax
//@[029:00029) | └─SkippedTriviaSyntax
//@[029:00031) ├─Token(NewLine) |\n\n|

// #completionTest(24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39) -> childFileCompletions
//@[106:00107) ├─Token(NewLine) |\n|
module childCompletionA 'ChildModules/'
//@[000:00039) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00023) | ├─IdentifierSyntax
//@[007:00023) | | └─Token(Identifier) |childCompletionA|
//@[024:00039) | ├─StringSyntax
//@[024:00039) | | └─Token(StringComplete) |'ChildModules/'|
//@[039:00039) | ├─SkippedTriviaSyntax
//@[039:00039) | └─SkippedTriviaSyntax
//@[039:00041) ├─Token(NewLine) |\n\n|

// #completionTest(24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39) -> childDotFileCompletions
//@[109:00110) ├─Token(NewLine) |\n|
module childCompletionB './ChildModules/'
//@[000:00041) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00023) | ├─IdentifierSyntax
//@[007:00023) | | └─Token(Identifier) |childCompletionB|
//@[024:00041) | ├─StringSyntax
//@[024:00041) | | └─Token(StringComplete) |'./ChildModules/'|
//@[041:00041) | ├─SkippedTriviaSyntax
//@[041:00041) | └─SkippedTriviaSyntax
//@[041:00043) ├─Token(NewLine) |\n\n|

// #completionTest(24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40) -> childDotFileCompletions
//@[113:00114) ├─Token(NewLine) |\n|
module childCompletionC './ChildModules/m'
//@[000:00042) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00023) | ├─IdentifierSyntax
//@[007:00023) | | └─Token(Identifier) |childCompletionC|
//@[024:00042) | ├─StringSyntax
//@[024:00042) | | └─Token(StringComplete) |'./ChildModules/m'|
//@[042:00042) | ├─SkippedTriviaSyntax
//@[042:00042) | └─SkippedTriviaSyntax
//@[042:00044) ├─Token(NewLine) |\n\n|

// #completionTest(24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40) -> childFileCompletions
//@[110:00111) ├─Token(NewLine) |\n|
module childCompletionD 'ChildModules/e'
//@[000:00040) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00023) | ├─IdentifierSyntax
//@[007:00023) | | └─Token(Identifier) |childCompletionD|
//@[024:00040) | ├─StringSyntax
//@[024:00040) | | └─Token(StringComplete) |'ChildModules/e'|
//@[040:00040) | ├─SkippedTriviaSyntax
//@[040:00040) | └─SkippedTriviaSyntax
//@[040:00042) ├─Token(NewLine) |\n\n|

@minValue()
//@[000:00118) ├─ModuleDeclarationSyntax
//@[000:00011) | ├─DecoratorSyntax
//@[000:00001) | | ├─Token(At) |@|
//@[001:00011) | | └─FunctionCallSyntax
//@[001:00009) | |   ├─IdentifierSyntax
//@[001:00009) | |   | └─Token(Identifier) |minValue|
//@[009:00010) | |   ├─Token(LeftParen) |(|
//@[010:00011) | |   └─Token(RightParen) |)|
//@[011:00012) | ├─Token(NewLine) |\n|
module moduleWithNotAttachableDecorators './empty.bicep' = {
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00040) | ├─IdentifierSyntax
//@[007:00040) | | └─Token(Identifier) |moduleWithNotAttachableDecorators|
//@[041:00056) | ├─StringSyntax
//@[041:00056) | | └─Token(StringComplete) |'./empty.bicep'|
//@[057:00058) | ├─Token(Assignment) |=|
//@[059:00106) | └─ObjectSyntax
//@[059:00060) |   ├─Token(LeftBrace) |{|
//@[060:00061) |   ├─Token(NewLine) |\n|
  name: 'moduleWithNotAttachableDecorators'
//@[002:00043) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00043) |   | └─StringSyntax
//@[008:00043) |   |   └─Token(StringComplete) |'moduleWithNotAttachableDecorators'|
//@[043:00044) |   ├─Token(NewLine) |\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00003) ├─Token(NewLine) |\n\n|

// loop parsing cases
//@[021:00022) ├─Token(NewLine) |\n|
module expectedForKeyword 'modulea.bicep' = []
//@[000:00046) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00025) | ├─IdentifierSyntax
//@[007:00025) | | └─Token(Identifier) |expectedForKeyword|
//@[026:00041) | ├─StringSyntax
//@[026:00041) | | └─Token(StringComplete) |'modulea.bicep'|
//@[042:00043) | ├─Token(Assignment) |=|
//@[044:00046) | └─SkippedTriviaSyntax
//@[044:00045) |   ├─Token(LeftSquare) |[|
//@[045:00046) |   └─Token(RightSquare) |]|
//@[046:00048) ├─Token(NewLine) |\n\n|

module expectedForKeyword2 'modulea.bicep' = [f]
//@[000:00048) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00026) | ├─IdentifierSyntax
//@[007:00026) | | └─Token(Identifier) |expectedForKeyword2|
//@[027:00042) | ├─StringSyntax
//@[027:00042) | | └─Token(StringComplete) |'modulea.bicep'|
//@[043:00044) | ├─Token(Assignment) |=|
//@[045:00048) | └─SkippedTriviaSyntax
//@[045:00046) |   ├─Token(LeftSquare) |[|
//@[046:00047) |   ├─Token(Identifier) |f|
//@[047:00048) |   └─Token(RightSquare) |]|
//@[048:00050) ├─Token(NewLine) |\n\n|

module expectedLoopVar 'modulea.bicep' = [for]
//@[000:00046) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00022) | ├─IdentifierSyntax
//@[007:00022) | | └─Token(Identifier) |expectedLoopVar|
//@[023:00038) | ├─StringSyntax
//@[023:00038) | | └─Token(StringComplete) |'modulea.bicep'|
//@[039:00040) | ├─Token(Assignment) |=|
//@[041:00046) | └─ForSyntax
//@[041:00042) |   ├─Token(LeftSquare) |[|
//@[042:00045) |   ├─Token(Identifier) |for|
//@[045:00045) |   ├─SkippedTriviaSyntax
//@[045:00045) |   ├─SkippedTriviaSyntax
//@[045:00045) |   ├─SkippedTriviaSyntax
//@[045:00045) |   ├─SkippedTriviaSyntax
//@[045:00045) |   ├─SkippedTriviaSyntax
//@[045:00046) |   └─Token(RightSquare) |]|
//@[046:00048) ├─Token(NewLine) |\n\n|

module expectedInKeyword 'modulea.bicep' = [for x]
//@[000:00050) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00024) | ├─IdentifierSyntax
//@[007:00024) | | └─Token(Identifier) |expectedInKeyword|
//@[025:00040) | ├─StringSyntax
//@[025:00040) | | └─Token(StringComplete) |'modulea.bicep'|
//@[041:00042) | ├─Token(Assignment) |=|
//@[043:00050) | └─ForSyntax
//@[043:00044) |   ├─Token(LeftSquare) |[|
//@[044:00047) |   ├─Token(Identifier) |for|
//@[048:00049) |   ├─LocalVariableSyntax
//@[048:00049) |   | └─IdentifierSyntax
//@[048:00049) |   |   └─Token(Identifier) |x|
//@[049:00049) |   ├─SkippedTriviaSyntax
//@[049:00049) |   ├─SkippedTriviaSyntax
//@[049:00049) |   ├─SkippedTriviaSyntax
//@[049:00049) |   ├─SkippedTriviaSyntax
//@[049:00050) |   └─Token(RightSquare) |]|
//@[050:00052) ├─Token(NewLine) |\n\n|

module expectedInKeyword2 'modulea.bicep' = [for x b]
//@[000:00053) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00025) | ├─IdentifierSyntax
//@[007:00025) | | └─Token(Identifier) |expectedInKeyword2|
//@[026:00041) | ├─StringSyntax
//@[026:00041) | | └─Token(StringComplete) |'modulea.bicep'|
//@[042:00043) | ├─Token(Assignment) |=|
//@[044:00053) | └─ForSyntax
//@[044:00045) |   ├─Token(LeftSquare) |[|
//@[045:00048) |   ├─Token(Identifier) |for|
//@[049:00050) |   ├─LocalVariableSyntax
//@[049:00050) |   | └─IdentifierSyntax
//@[049:00050) |   |   └─Token(Identifier) |x|
//@[051:00052) |   ├─SkippedTriviaSyntax
//@[051:00052) |   | └─Token(Identifier) |b|
//@[052:00052) |   ├─SkippedTriviaSyntax
//@[052:00052) |   ├─SkippedTriviaSyntax
//@[052:00052) |   ├─SkippedTriviaSyntax
//@[052:00053) |   └─Token(RightSquare) |]|
//@[053:00055) ├─Token(NewLine) |\n\n|

module expectedArrayExpression 'modulea.bicep' = [for x in]
//@[000:00059) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00030) | ├─IdentifierSyntax
//@[007:00030) | | └─Token(Identifier) |expectedArrayExpression|
//@[031:00046) | ├─StringSyntax
//@[031:00046) | | └─Token(StringComplete) |'modulea.bicep'|
//@[047:00048) | ├─Token(Assignment) |=|
//@[049:00059) | └─ForSyntax
//@[049:00050) |   ├─Token(LeftSquare) |[|
//@[050:00053) |   ├─Token(Identifier) |for|
//@[054:00055) |   ├─LocalVariableSyntax
//@[054:00055) |   | └─IdentifierSyntax
//@[054:00055) |   |   └─Token(Identifier) |x|
//@[056:00058) |   ├─Token(Identifier) |in|
//@[058:00058) |   ├─SkippedTriviaSyntax
//@[058:00058) |   ├─SkippedTriviaSyntax
//@[058:00058) |   ├─SkippedTriviaSyntax
//@[058:00059) |   └─Token(RightSquare) |]|
//@[059:00061) ├─Token(NewLine) |\n\n|

module expectedColon 'modulea.bicep' = [for x in y]
//@[000:00051) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00020) | ├─IdentifierSyntax
//@[007:00020) | | └─Token(Identifier) |expectedColon|
//@[021:00036) | ├─StringSyntax
//@[021:00036) | | └─Token(StringComplete) |'modulea.bicep'|
//@[037:00038) | ├─Token(Assignment) |=|
//@[039:00051) | └─ForSyntax
//@[039:00040) |   ├─Token(LeftSquare) |[|
//@[040:00043) |   ├─Token(Identifier) |for|
//@[044:00045) |   ├─LocalVariableSyntax
//@[044:00045) |   | └─IdentifierSyntax
//@[044:00045) |   |   └─Token(Identifier) |x|
//@[046:00048) |   ├─Token(Identifier) |in|
//@[049:00050) |   ├─VariableAccessSyntax
//@[049:00050) |   | └─IdentifierSyntax
//@[049:00050) |   |   └─Token(Identifier) |y|
//@[050:00050) |   ├─SkippedTriviaSyntax
//@[050:00050) |   ├─SkippedTriviaSyntax
//@[050:00051) |   └─Token(RightSquare) |]|
//@[051:00053) ├─Token(NewLine) |\n\n|

module expectedLoopBody 'modulea.bicep' = [for x in y:]
//@[000:00055) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00023) | ├─IdentifierSyntax
//@[007:00023) | | └─Token(Identifier) |expectedLoopBody|
//@[024:00039) | ├─StringSyntax
//@[024:00039) | | └─Token(StringComplete) |'modulea.bicep'|
//@[040:00041) | ├─Token(Assignment) |=|
//@[042:00055) | └─ForSyntax
//@[042:00043) |   ├─Token(LeftSquare) |[|
//@[043:00046) |   ├─Token(Identifier) |for|
//@[047:00048) |   ├─LocalVariableSyntax
//@[047:00048) |   | └─IdentifierSyntax
//@[047:00048) |   |   └─Token(Identifier) |x|
//@[049:00051) |   ├─Token(Identifier) |in|
//@[052:00053) |   ├─VariableAccessSyntax
//@[052:00053) |   | └─IdentifierSyntax
//@[052:00053) |   |   └─Token(Identifier) |y|
//@[053:00054) |   ├─Token(Colon) |:|
//@[054:00054) |   ├─SkippedTriviaSyntax
//@[054:00055) |   └─Token(RightSquare) |]|
//@[055:00057) ├─Token(NewLine) |\n\n|

// indexed loop parsing cases
//@[029:00030) ├─Token(NewLine) |\n|
module expectedItemVarName 'modulea.bicep' = [for ()]
//@[000:00053) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00026) | ├─IdentifierSyntax
//@[007:00026) | | └─Token(Identifier) |expectedItemVarName|
//@[027:00042) | ├─StringSyntax
//@[027:00042) | | └─Token(StringComplete) |'modulea.bicep'|
//@[043:00044) | ├─Token(Assignment) |=|
//@[045:00053) | └─ForSyntax
//@[045:00046) |   ├─Token(LeftSquare) |[|
//@[046:00049) |   ├─Token(Identifier) |for|
//@[050:00052) |   ├─SkippedTriviaSyntax
//@[050:00052) |   | └─VariableBlockSyntax
//@[050:00051) |   |   ├─Token(LeftParen) |(|
//@[051:00052) |   |   └─Token(RightParen) |)|
//@[052:00052) |   ├─SkippedTriviaSyntax
//@[052:00052) |   ├─SkippedTriviaSyntax
//@[052:00052) |   ├─SkippedTriviaSyntax
//@[052:00052) |   ├─SkippedTriviaSyntax
//@[052:00053) |   └─Token(RightSquare) |]|
//@[053:00055) ├─Token(NewLine) |\n\n|

module expectedComma 'modulea.bicep' = [for (x)]
//@[000:00048) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00020) | ├─IdentifierSyntax
//@[007:00020) | | └─Token(Identifier) |expectedComma|
//@[021:00036) | ├─StringSyntax
//@[021:00036) | | └─Token(StringComplete) |'modulea.bicep'|
//@[037:00038) | ├─Token(Assignment) |=|
//@[039:00048) | └─ForSyntax
//@[039:00040) |   ├─Token(LeftSquare) |[|
//@[040:00043) |   ├─Token(Identifier) |for|
//@[044:00047) |   ├─SkippedTriviaSyntax
//@[044:00047) |   | └─VariableBlockSyntax
//@[044:00045) |   |   ├─Token(LeftParen) |(|
//@[045:00046) |   |   ├─LocalVariableSyntax
//@[045:00046) |   |   | └─IdentifierSyntax
//@[045:00046) |   |   |   └─Token(Identifier) |x|
//@[046:00047) |   |   └─Token(RightParen) |)|
//@[047:00047) |   ├─SkippedTriviaSyntax
//@[047:00047) |   ├─SkippedTriviaSyntax
//@[047:00047) |   ├─SkippedTriviaSyntax
//@[047:00047) |   ├─SkippedTriviaSyntax
//@[047:00048) |   └─Token(RightSquare) |]|
//@[048:00050) ├─Token(NewLine) |\n\n|

module expectedIndexVarName 'modulea.bicep' = [for (x,)]
//@[000:00056) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00027) | ├─IdentifierSyntax
//@[007:00027) | | └─Token(Identifier) |expectedIndexVarName|
//@[028:00043) | ├─StringSyntax
//@[028:00043) | | └─Token(StringComplete) |'modulea.bicep'|
//@[044:00045) | ├─Token(Assignment) |=|
//@[046:00056) | └─ForSyntax
//@[046:00047) |   ├─Token(LeftSquare) |[|
//@[047:00050) |   ├─Token(Identifier) |for|
//@[051:00055) |   ├─SkippedTriviaSyntax
//@[051:00055) |   | └─VariableBlockSyntax
//@[051:00052) |   |   ├─Token(LeftParen) |(|
//@[052:00053) |   |   ├─LocalVariableSyntax
//@[052:00053) |   |   | └─IdentifierSyntax
//@[052:00053) |   |   |   └─Token(Identifier) |x|
//@[053:00054) |   |   ├─Token(Comma) |,|
//@[054:00055) |   |   └─Token(RightParen) |)|
//@[055:00055) |   ├─SkippedTriviaSyntax
//@[055:00055) |   ├─SkippedTriviaSyntax
//@[055:00055) |   ├─SkippedTriviaSyntax
//@[055:00055) |   ├─SkippedTriviaSyntax
//@[055:00056) |   └─Token(RightSquare) |]|
//@[056:00058) ├─Token(NewLine) |\n\n|

module expectedInKeyword3 'modulea.bicep' = [for (x,y)]
//@[000:00055) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00025) | ├─IdentifierSyntax
//@[007:00025) | | └─Token(Identifier) |expectedInKeyword3|
//@[026:00041) | ├─StringSyntax
//@[026:00041) | | └─Token(StringComplete) |'modulea.bicep'|
//@[042:00043) | ├─Token(Assignment) |=|
//@[044:00055) | └─ForSyntax
//@[044:00045) |   ├─Token(LeftSquare) |[|
//@[045:00048) |   ├─Token(Identifier) |for|
//@[049:00054) |   ├─VariableBlockSyntax
//@[049:00050) |   | ├─Token(LeftParen) |(|
//@[050:00051) |   | ├─LocalVariableSyntax
//@[050:00051) |   | | └─IdentifierSyntax
//@[050:00051) |   | |   └─Token(Identifier) |x|
//@[051:00052) |   | ├─Token(Comma) |,|
//@[052:00053) |   | ├─LocalVariableSyntax
//@[052:00053) |   | | └─IdentifierSyntax
//@[052:00053) |   | |   └─Token(Identifier) |y|
//@[053:00054) |   | └─Token(RightParen) |)|
//@[054:00054) |   ├─SkippedTriviaSyntax
//@[054:00054) |   ├─SkippedTriviaSyntax
//@[054:00054) |   ├─SkippedTriviaSyntax
//@[054:00054) |   ├─SkippedTriviaSyntax
//@[054:00055) |   └─Token(RightSquare) |]|
//@[055:00057) ├─Token(NewLine) |\n\n|

module expectedArrayExpression2 'modulea.bicep' = [for (x,y) in ]
//@[000:00065) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00031) | ├─IdentifierSyntax
//@[007:00031) | | └─Token(Identifier) |expectedArrayExpression2|
//@[032:00047) | ├─StringSyntax
//@[032:00047) | | └─Token(StringComplete) |'modulea.bicep'|
//@[048:00049) | ├─Token(Assignment) |=|
//@[050:00065) | └─ForSyntax
//@[050:00051) |   ├─Token(LeftSquare) |[|
//@[051:00054) |   ├─Token(Identifier) |for|
//@[055:00060) |   ├─VariableBlockSyntax
//@[055:00056) |   | ├─Token(LeftParen) |(|
//@[056:00057) |   | ├─LocalVariableSyntax
//@[056:00057) |   | | └─IdentifierSyntax
//@[056:00057) |   | |   └─Token(Identifier) |x|
//@[057:00058) |   | ├─Token(Comma) |,|
//@[058:00059) |   | ├─LocalVariableSyntax
//@[058:00059) |   | | └─IdentifierSyntax
//@[058:00059) |   | |   └─Token(Identifier) |y|
//@[059:00060) |   | └─Token(RightParen) |)|
//@[061:00063) |   ├─Token(Identifier) |in|
//@[064:00064) |   ├─SkippedTriviaSyntax
//@[064:00064) |   ├─SkippedTriviaSyntax
//@[064:00064) |   ├─SkippedTriviaSyntax
//@[064:00065) |   └─Token(RightSquare) |]|
//@[065:00067) ├─Token(NewLine) |\n\n|

module expectedColon2 'modulea.bicep' = [for (x,y) in z]
//@[000:00056) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00021) | ├─IdentifierSyntax
//@[007:00021) | | └─Token(Identifier) |expectedColon2|
//@[022:00037) | ├─StringSyntax
//@[022:00037) | | └─Token(StringComplete) |'modulea.bicep'|
//@[038:00039) | ├─Token(Assignment) |=|
//@[040:00056) | └─ForSyntax
//@[040:00041) |   ├─Token(LeftSquare) |[|
//@[041:00044) |   ├─Token(Identifier) |for|
//@[045:00050) |   ├─VariableBlockSyntax
//@[045:00046) |   | ├─Token(LeftParen) |(|
//@[046:00047) |   | ├─LocalVariableSyntax
//@[046:00047) |   | | └─IdentifierSyntax
//@[046:00047) |   | |   └─Token(Identifier) |x|
//@[047:00048) |   | ├─Token(Comma) |,|
//@[048:00049) |   | ├─LocalVariableSyntax
//@[048:00049) |   | | └─IdentifierSyntax
//@[048:00049) |   | |   └─Token(Identifier) |y|
//@[049:00050) |   | └─Token(RightParen) |)|
//@[051:00053) |   ├─Token(Identifier) |in|
//@[054:00055) |   ├─VariableAccessSyntax
//@[054:00055) |   | └─IdentifierSyntax
//@[054:00055) |   |   └─Token(Identifier) |z|
//@[055:00055) |   ├─SkippedTriviaSyntax
//@[055:00055) |   ├─SkippedTriviaSyntax
//@[055:00056) |   └─Token(RightSquare) |]|
//@[056:00058) ├─Token(NewLine) |\n\n|

module expectedLoopBody2 'modulea.bicep' = [for (x,y) in z:]
//@[000:00060) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00024) | ├─IdentifierSyntax
//@[007:00024) | | └─Token(Identifier) |expectedLoopBody2|
//@[025:00040) | ├─StringSyntax
//@[025:00040) | | └─Token(StringComplete) |'modulea.bicep'|
//@[041:00042) | ├─Token(Assignment) |=|
//@[043:00060) | └─ForSyntax
//@[043:00044) |   ├─Token(LeftSquare) |[|
//@[044:00047) |   ├─Token(Identifier) |for|
//@[048:00053) |   ├─VariableBlockSyntax
//@[048:00049) |   | ├─Token(LeftParen) |(|
//@[049:00050) |   | ├─LocalVariableSyntax
//@[049:00050) |   | | └─IdentifierSyntax
//@[049:00050) |   | |   └─Token(Identifier) |x|
//@[050:00051) |   | ├─Token(Comma) |,|
//@[051:00052) |   | ├─LocalVariableSyntax
//@[051:00052) |   | | └─IdentifierSyntax
//@[051:00052) |   | |   └─Token(Identifier) |y|
//@[052:00053) |   | └─Token(RightParen) |)|
//@[054:00056) |   ├─Token(Identifier) |in|
//@[057:00058) |   ├─VariableAccessSyntax
//@[057:00058) |   | └─IdentifierSyntax
//@[057:00058) |   |   └─Token(Identifier) |z|
//@[058:00059) |   ├─Token(Colon) |:|
//@[059:00059) |   ├─SkippedTriviaSyntax
//@[059:00060) |   └─Token(RightSquare) |]|
//@[060:00062) ├─Token(NewLine) |\n\n|

// loop filter parsing cases
//@[028:00029) ├─Token(NewLine) |\n|
module expectedLoopFilterOpenParen 'modulea.bicep' = [for x in y: if]
//@[000:00069) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00034) | ├─IdentifierSyntax
//@[007:00034) | | └─Token(Identifier) |expectedLoopFilterOpenParen|
//@[035:00050) | ├─StringSyntax
//@[035:00050) | | └─Token(StringComplete) |'modulea.bicep'|
//@[051:00052) | ├─Token(Assignment) |=|
//@[053:00069) | └─ForSyntax
//@[053:00054) |   ├─Token(LeftSquare) |[|
//@[054:00057) |   ├─Token(Identifier) |for|
//@[058:00059) |   ├─LocalVariableSyntax
//@[058:00059) |   | └─IdentifierSyntax
//@[058:00059) |   |   └─Token(Identifier) |x|
//@[060:00062) |   ├─Token(Identifier) |in|
//@[063:00064) |   ├─VariableAccessSyntax
//@[063:00064) |   | └─IdentifierSyntax
//@[063:00064) |   |   └─Token(Identifier) |y|
//@[064:00065) |   ├─Token(Colon) |:|
//@[066:00068) |   ├─IfConditionSyntax
//@[066:00068) |   | ├─Token(Identifier) |if|
//@[068:00068) |   | ├─SkippedTriviaSyntax
//@[068:00068) |   | └─SkippedTriviaSyntax
//@[068:00069) |   └─Token(RightSquare) |]|
//@[069:00070) ├─Token(NewLine) |\n|
module expectedLoopFilterOpenParen2 'modulea.bicep' = [for (x,y) in z: if]
//@[000:00074) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00035) | ├─IdentifierSyntax
//@[007:00035) | | └─Token(Identifier) |expectedLoopFilterOpenParen2|
//@[036:00051) | ├─StringSyntax
//@[036:00051) | | └─Token(StringComplete) |'modulea.bicep'|
//@[052:00053) | ├─Token(Assignment) |=|
//@[054:00074) | └─ForSyntax
//@[054:00055) |   ├─Token(LeftSquare) |[|
//@[055:00058) |   ├─Token(Identifier) |for|
//@[059:00064) |   ├─VariableBlockSyntax
//@[059:00060) |   | ├─Token(LeftParen) |(|
//@[060:00061) |   | ├─LocalVariableSyntax
//@[060:00061) |   | | └─IdentifierSyntax
//@[060:00061) |   | |   └─Token(Identifier) |x|
//@[061:00062) |   | ├─Token(Comma) |,|
//@[062:00063) |   | ├─LocalVariableSyntax
//@[062:00063) |   | | └─IdentifierSyntax
//@[062:00063) |   | |   └─Token(Identifier) |y|
//@[063:00064) |   | └─Token(RightParen) |)|
//@[065:00067) |   ├─Token(Identifier) |in|
//@[068:00069) |   ├─VariableAccessSyntax
//@[068:00069) |   | └─IdentifierSyntax
//@[068:00069) |   |   └─Token(Identifier) |z|
//@[069:00070) |   ├─Token(Colon) |:|
//@[071:00073) |   ├─IfConditionSyntax
//@[071:00073) |   | ├─Token(Identifier) |if|
//@[073:00073) |   | ├─SkippedTriviaSyntax
//@[073:00073) |   | └─SkippedTriviaSyntax
//@[073:00074) |   └─Token(RightSquare) |]|
//@[074:00076) ├─Token(NewLine) |\n\n|

module expectedLoopFilterPredicateAndBody 'modulea.bicep' = [for x in y: if()]
//@[000:00078) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00041) | ├─IdentifierSyntax
//@[007:00041) | | └─Token(Identifier) |expectedLoopFilterPredicateAndBody|
//@[042:00057) | ├─StringSyntax
//@[042:00057) | | └─Token(StringComplete) |'modulea.bicep'|
//@[058:00059) | ├─Token(Assignment) |=|
//@[060:00078) | └─ForSyntax
//@[060:00061) |   ├─Token(LeftSquare) |[|
//@[061:00064) |   ├─Token(Identifier) |for|
//@[065:00066) |   ├─LocalVariableSyntax
//@[065:00066) |   | └─IdentifierSyntax
//@[065:00066) |   |   └─Token(Identifier) |x|
//@[067:00069) |   ├─Token(Identifier) |in|
//@[070:00071) |   ├─VariableAccessSyntax
//@[070:00071) |   | └─IdentifierSyntax
//@[070:00071) |   |   └─Token(Identifier) |y|
//@[071:00072) |   ├─Token(Colon) |:|
//@[073:00077) |   ├─IfConditionSyntax
//@[073:00075) |   | ├─Token(Identifier) |if|
//@[075:00077) |   | ├─ParenthesizedExpressionSyntax
//@[075:00076) |   | | ├─Token(LeftParen) |(|
//@[076:00076) |   | | ├─SkippedTriviaSyntax
//@[076:00077) |   | | └─Token(RightParen) |)|
//@[077:00077) |   | └─SkippedTriviaSyntax
//@[077:00078) |   └─Token(RightSquare) |]|
//@[078:00079) ├─Token(NewLine) |\n|
module expectedLoopFilterPredicateAndBody2 'modulea.bicep' = [for (x,y) in z: if()]
//@[000:00083) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00042) | ├─IdentifierSyntax
//@[007:00042) | | └─Token(Identifier) |expectedLoopFilterPredicateAndBody2|
//@[043:00058) | ├─StringSyntax
//@[043:00058) | | └─Token(StringComplete) |'modulea.bicep'|
//@[059:00060) | ├─Token(Assignment) |=|
//@[061:00083) | └─ForSyntax
//@[061:00062) |   ├─Token(LeftSquare) |[|
//@[062:00065) |   ├─Token(Identifier) |for|
//@[066:00071) |   ├─VariableBlockSyntax
//@[066:00067) |   | ├─Token(LeftParen) |(|
//@[067:00068) |   | ├─LocalVariableSyntax
//@[067:00068) |   | | └─IdentifierSyntax
//@[067:00068) |   | |   └─Token(Identifier) |x|
//@[068:00069) |   | ├─Token(Comma) |,|
//@[069:00070) |   | ├─LocalVariableSyntax
//@[069:00070) |   | | └─IdentifierSyntax
//@[069:00070) |   | |   └─Token(Identifier) |y|
//@[070:00071) |   | └─Token(RightParen) |)|
//@[072:00074) |   ├─Token(Identifier) |in|
//@[075:00076) |   ├─VariableAccessSyntax
//@[075:00076) |   | └─IdentifierSyntax
//@[075:00076) |   |   └─Token(Identifier) |z|
//@[076:00077) |   ├─Token(Colon) |:|
//@[078:00082) |   ├─IfConditionSyntax
//@[078:00080) |   | ├─Token(Identifier) |if|
//@[080:00082) |   | ├─ParenthesizedExpressionSyntax
//@[080:00081) |   | | ├─Token(LeftParen) |(|
//@[081:00081) |   | | ├─SkippedTriviaSyntax
//@[081:00082) |   | | └─Token(RightParen) |)|
//@[082:00082) |   | └─SkippedTriviaSyntax
//@[082:00083) |   └─Token(RightSquare) |]|
//@[083:00085) ├─Token(NewLine) |\n\n|

// wrong loop body type
//@[023:00024) ├─Token(NewLine) |\n|
var emptyArray = []
//@[000:00019) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00014) | ├─IdentifierSyntax
//@[004:00014) | | └─Token(Identifier) |emptyArray|
//@[015:00016) | ├─Token(Assignment) |=|
//@[017:00019) | └─ArraySyntax
//@[017:00018) |   ├─Token(LeftSquare) |[|
//@[018:00019) |   └─Token(RightSquare) |]|
//@[019:00020) ├─Token(NewLine) |\n|
module wrongLoopBodyType 'modulea.bicep' = [for x in emptyArray:4]
//@[000:00066) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00024) | ├─IdentifierSyntax
//@[007:00024) | | └─Token(Identifier) |wrongLoopBodyType|
//@[025:00040) | ├─StringSyntax
//@[025:00040) | | └─Token(StringComplete) |'modulea.bicep'|
//@[041:00042) | ├─Token(Assignment) |=|
//@[043:00066) | └─ForSyntax
//@[043:00044) |   ├─Token(LeftSquare) |[|
//@[044:00047) |   ├─Token(Identifier) |for|
//@[048:00049) |   ├─LocalVariableSyntax
//@[048:00049) |   | └─IdentifierSyntax
//@[048:00049) |   |   └─Token(Identifier) |x|
//@[050:00052) |   ├─Token(Identifier) |in|
//@[053:00063) |   ├─VariableAccessSyntax
//@[053:00063) |   | └─IdentifierSyntax
//@[053:00063) |   |   └─Token(Identifier) |emptyArray|
//@[063:00064) |   ├─Token(Colon) |:|
//@[064:00065) |   ├─SkippedTriviaSyntax
//@[064:00065) |   | └─Token(Integer) |4|
//@[065:00066) |   └─Token(RightSquare) |]|
//@[066:00067) ├─Token(NewLine) |\n|
module wrongLoopBodyType2 'modulea.bicep' = [for (x,i) in emptyArray:4]
//@[000:00071) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00025) | ├─IdentifierSyntax
//@[007:00025) | | └─Token(Identifier) |wrongLoopBodyType2|
//@[026:00041) | ├─StringSyntax
//@[026:00041) | | └─Token(StringComplete) |'modulea.bicep'|
//@[042:00043) | ├─Token(Assignment) |=|
//@[044:00071) | └─ForSyntax
//@[044:00045) |   ├─Token(LeftSquare) |[|
//@[045:00048) |   ├─Token(Identifier) |for|
//@[049:00054) |   ├─VariableBlockSyntax
//@[049:00050) |   | ├─Token(LeftParen) |(|
//@[050:00051) |   | ├─LocalVariableSyntax
//@[050:00051) |   | | └─IdentifierSyntax
//@[050:00051) |   | |   └─Token(Identifier) |x|
//@[051:00052) |   | ├─Token(Comma) |,|
//@[052:00053) |   | ├─LocalVariableSyntax
//@[052:00053) |   | | └─IdentifierSyntax
//@[052:00053) |   | |   └─Token(Identifier) |i|
//@[053:00054) |   | └─Token(RightParen) |)|
//@[055:00057) |   ├─Token(Identifier) |in|
//@[058:00068) |   ├─VariableAccessSyntax
//@[058:00068) |   | └─IdentifierSyntax
//@[058:00068) |   |   └─Token(Identifier) |emptyArray|
//@[068:00069) |   ├─Token(Colon) |:|
//@[069:00070) |   ├─SkippedTriviaSyntax
//@[069:00070) |   | └─Token(Integer) |4|
//@[070:00071) |   └─Token(RightSquare) |]|
//@[071:00073) ├─Token(NewLine) |\n\n|

// missing loop body properties
//@[031:00032) ├─Token(NewLine) |\n|
module missingLoopBodyProperties 'modulea.bicep' = [for x in emptyArray:{
//@[000:00076) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00032) | ├─IdentifierSyntax
//@[007:00032) | | └─Token(Identifier) |missingLoopBodyProperties|
//@[033:00048) | ├─StringSyntax
//@[033:00048) | | └─Token(StringComplete) |'modulea.bicep'|
//@[049:00050) | ├─Token(Assignment) |=|
//@[051:00076) | └─ForSyntax
//@[051:00052) |   ├─Token(LeftSquare) |[|
//@[052:00055) |   ├─Token(Identifier) |for|
//@[056:00057) |   ├─LocalVariableSyntax
//@[056:00057) |   | └─IdentifierSyntax
//@[056:00057) |   |   └─Token(Identifier) |x|
//@[058:00060) |   ├─Token(Identifier) |in|
//@[061:00071) |   ├─VariableAccessSyntax
//@[061:00071) |   | └─IdentifierSyntax
//@[061:00071) |   |   └─Token(Identifier) |emptyArray|
//@[071:00072) |   ├─Token(Colon) |:|
//@[072:00075) |   ├─ObjectSyntax
//@[072:00073) |   | ├─Token(LeftBrace) |{|
//@[073:00074) |   | ├─Token(NewLine) |\n|
}]
//@[000:00001) |   | └─Token(RightBrace) |}|
//@[001:00002) |   └─Token(RightSquare) |]|
//@[002:00003) ├─Token(NewLine) |\n|
module missingLoopBodyProperties2 'modulea.bicep' = [for (x,i) in emptyArray:{
//@[000:00081) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00033) | ├─IdentifierSyntax
//@[007:00033) | | └─Token(Identifier) |missingLoopBodyProperties2|
//@[034:00049) | ├─StringSyntax
//@[034:00049) | | └─Token(StringComplete) |'modulea.bicep'|
//@[050:00051) | ├─Token(Assignment) |=|
//@[052:00081) | └─ForSyntax
//@[052:00053) |   ├─Token(LeftSquare) |[|
//@[053:00056) |   ├─Token(Identifier) |for|
//@[057:00062) |   ├─VariableBlockSyntax
//@[057:00058) |   | ├─Token(LeftParen) |(|
//@[058:00059) |   | ├─LocalVariableSyntax
//@[058:00059) |   | | └─IdentifierSyntax
//@[058:00059) |   | |   └─Token(Identifier) |x|
//@[059:00060) |   | ├─Token(Comma) |,|
//@[060:00061) |   | ├─LocalVariableSyntax
//@[060:00061) |   | | └─IdentifierSyntax
//@[060:00061) |   | |   └─Token(Identifier) |i|
//@[061:00062) |   | └─Token(RightParen) |)|
//@[063:00065) |   ├─Token(Identifier) |in|
//@[066:00076) |   ├─VariableAccessSyntax
//@[066:00076) |   | └─IdentifierSyntax
//@[066:00076) |   |   └─Token(Identifier) |emptyArray|
//@[076:00077) |   ├─Token(Colon) |:|
//@[077:00080) |   ├─ObjectSyntax
//@[077:00078) |   | ├─Token(LeftBrace) |{|
//@[078:00079) |   | ├─Token(NewLine) |\n|
}]
//@[000:00001) |   | └─Token(RightBrace) |}|
//@[001:00002) |   └─Token(RightSquare) |]|
//@[002:00004) ├─Token(NewLine) |\n\n|

// wrong array type
//@[019:00020) ├─Token(NewLine) |\n|
var notAnArray = true
//@[000:00021) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00014) | ├─IdentifierSyntax
//@[004:00014) | | └─Token(Identifier) |notAnArray|
//@[015:00016) | ├─Token(Assignment) |=|
//@[017:00021) | └─BooleanLiteralSyntax
//@[017:00021) |   └─Token(TrueKeyword) |true|
//@[021:00022) ├─Token(NewLine) |\n|
module wrongArrayType 'modulea.bicep' = [for x in notAnArray:{
//@[000:00065) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00021) | ├─IdentifierSyntax
//@[007:00021) | | └─Token(Identifier) |wrongArrayType|
//@[022:00037) | ├─StringSyntax
//@[022:00037) | | └─Token(StringComplete) |'modulea.bicep'|
//@[038:00039) | ├─Token(Assignment) |=|
//@[040:00065) | └─ForSyntax
//@[040:00041) |   ├─Token(LeftSquare) |[|
//@[041:00044) |   ├─Token(Identifier) |for|
//@[045:00046) |   ├─LocalVariableSyntax
//@[045:00046) |   | └─IdentifierSyntax
//@[045:00046) |   |   └─Token(Identifier) |x|
//@[047:00049) |   ├─Token(Identifier) |in|
//@[050:00060) |   ├─VariableAccessSyntax
//@[050:00060) |   | └─IdentifierSyntax
//@[050:00060) |   |   └─Token(Identifier) |notAnArray|
//@[060:00061) |   ├─Token(Colon) |:|
//@[061:00064) |   ├─ObjectSyntax
//@[061:00062) |   | ├─Token(LeftBrace) |{|
//@[062:00063) |   | ├─Token(NewLine) |\n|
}]
//@[000:00001) |   | └─Token(RightBrace) |}|
//@[001:00002) |   └─Token(RightSquare) |]|
//@[002:00004) ├─Token(NewLine) |\n\n|

// missing fewer properties
//@[027:00028) ├─Token(NewLine) |\n|
module missingFewerLoopBodyProperties 'modulea.bicep' = [for x in emptyArray:{
//@[000:00119) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00037) | ├─IdentifierSyntax
//@[007:00037) | | └─Token(Identifier) |missingFewerLoopBodyProperties|
//@[038:00053) | ├─StringSyntax
//@[038:00053) | | └─Token(StringComplete) |'modulea.bicep'|
//@[054:00055) | ├─Token(Assignment) |=|
//@[056:00119) | └─ForSyntax
//@[056:00057) |   ├─Token(LeftSquare) |[|
//@[057:00060) |   ├─Token(Identifier) |for|
//@[061:00062) |   ├─LocalVariableSyntax
//@[061:00062) |   | └─IdentifierSyntax
//@[061:00062) |   |   └─Token(Identifier) |x|
//@[063:00065) |   ├─Token(Identifier) |in|
//@[066:00076) |   ├─VariableAccessSyntax
//@[066:00076) |   | └─IdentifierSyntax
//@[066:00076) |   |   └─Token(Identifier) |emptyArray|
//@[076:00077) |   ├─Token(Colon) |:|
//@[077:00118) |   ├─ObjectSyntax
//@[077:00078) |   | ├─Token(LeftBrace) |{|
//@[078:00079) |   | ├─Token(NewLine) |\n|
  name: 'hello-${x}'
//@[002:00020) |   | ├─ObjectPropertySyntax
//@[002:00006) |   | | ├─IdentifierSyntax
//@[002:00006) |   | | | └─Token(Identifier) |name|
//@[006:00007) |   | | ├─Token(Colon) |:|
//@[008:00020) |   | | └─StringSyntax
//@[008:00017) |   | |   ├─Token(StringLeftPiece) |'hello-${|
//@[017:00018) |   | |   ├─VariableAccessSyntax
//@[017:00018) |   | |   | └─IdentifierSyntax
//@[017:00018) |   | |   |   └─Token(Identifier) |x|
//@[018:00020) |   | |   └─Token(StringRightPiece) |}'|
//@[020:00021) |   | ├─Token(NewLine) |\n|
  params: {
//@[002:00016) |   | ├─ObjectPropertySyntax
//@[002:00008) |   | | ├─IdentifierSyntax
//@[002:00008) |   | | | └─Token(Identifier) |params|
//@[008:00009) |   | | ├─Token(Colon) |:|
//@[010:00016) |   | | └─ObjectSyntax
//@[010:00011) |   | |   ├─Token(LeftBrace) |{|
//@[011:00013) |   | |   ├─Token(NewLine) |\n\n|

  }
//@[002:00003) |   | |   └─Token(RightBrace) |}|
//@[003:00004) |   | ├─Token(NewLine) |\n|
}]
//@[000:00001) |   | └─Token(RightBrace) |}|
//@[001:00002) |   └─Token(RightSquare) |]|
//@[002:00004) ├─Token(NewLine) |\n\n|

// wrong parameter in the module loop
//@[037:00038) ├─Token(NewLine) |\n|
module wrongModuleParameterInLoop 'modulea.bicep' = [for x in emptyArray:{
//@[000:00263) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00033) | ├─IdentifierSyntax
//@[007:00033) | | └─Token(Identifier) |wrongModuleParameterInLoop|
//@[034:00049) | ├─StringSyntax
//@[034:00049) | | └─Token(StringComplete) |'modulea.bicep'|
//@[050:00051) | ├─Token(Assignment) |=|
//@[052:00263) | └─ForSyntax
//@[052:00053) |   ├─Token(LeftSquare) |[|
//@[053:00056) |   ├─Token(Identifier) |for|
//@[057:00058) |   ├─LocalVariableSyntax
//@[057:00058) |   | └─IdentifierSyntax
//@[057:00058) |   |   └─Token(Identifier) |x|
//@[059:00061) |   ├─Token(Identifier) |in|
//@[062:00072) |   ├─VariableAccessSyntax
//@[062:00072) |   | └─IdentifierSyntax
//@[062:00072) |   |   └─Token(Identifier) |emptyArray|
//@[072:00073) |   ├─Token(Colon) |:|
//@[073:00262) |   ├─ObjectSyntax
//@[073:00074) |   | ├─Token(LeftBrace) |{|
//@[074:00075) |   | ├─Token(NewLine) |\n|
  // #completionTest(17) -> symbolsPlusX
//@[040:00041) |   | ├─Token(NewLine) |\n|
  name: 'hello-${x}'
//@[002:00020) |   | ├─ObjectPropertySyntax
//@[002:00006) |   | | ├─IdentifierSyntax
//@[002:00006) |   | | | └─Token(Identifier) |name|
//@[006:00007) |   | | ├─Token(Colon) |:|
//@[008:00020) |   | | └─StringSyntax
//@[008:00017) |   | |   ├─Token(StringLeftPiece) |'hello-${|
//@[017:00018) |   | |   ├─VariableAccessSyntax
//@[017:00018) |   | |   | └─IdentifierSyntax
//@[017:00018) |   | |   |   └─Token(Identifier) |x|
//@[018:00020) |   | |   └─Token(StringRightPiece) |}'|
//@[020:00021) |   | ├─Token(NewLine) |\n|
  params: {
//@[002:00123) |   | ├─ObjectPropertySyntax
//@[002:00008) |   | | ├─IdentifierSyntax
//@[002:00008) |   | | | └─Token(Identifier) |params|
//@[008:00009) |   | | ├─Token(Colon) |:|
//@[010:00123) |   | | └─ObjectSyntax
//@[010:00011) |   | |   ├─Token(LeftBrace) |{|
//@[011:00012) |   | |   ├─Token(NewLine) |\n|
    arrayParam: []
//@[004:00018) |   | |   ├─ObjectPropertySyntax
//@[004:00014) |   | |   | ├─IdentifierSyntax
//@[004:00014) |   | |   | | └─Token(Identifier) |arrayParam|
//@[014:00015) |   | |   | ├─Token(Colon) |:|
//@[016:00018) |   | |   | └─ArraySyntax
//@[016:00017) |   | |   |   ├─Token(LeftSquare) |[|
//@[017:00018) |   | |   |   └─Token(RightSquare) |]|
//@[018:00019) |   | |   ├─Token(NewLine) |\n|
    objParam: {}
//@[004:00016) |   | |   ├─ObjectPropertySyntax
//@[004:00012) |   | |   | ├─IdentifierSyntax
//@[004:00012) |   | |   | | └─Token(Identifier) |objParam|
//@[012:00013) |   | |   | ├─Token(Colon) |:|
//@[014:00016) |   | |   | └─ObjectSyntax
//@[014:00015) |   | |   |   ├─Token(LeftBrace) |{|
//@[015:00016) |   | |   |   └─Token(RightBrace) |}|
//@[016:00017) |   | |   ├─Token(NewLine) |\n|
    stringParamA: 'test'
//@[004:00024) |   | |   ├─ObjectPropertySyntax
//@[004:00016) |   | |   | ├─IdentifierSyntax
//@[004:00016) |   | |   | | └─Token(Identifier) |stringParamA|
//@[016:00017) |   | |   | ├─Token(Colon) |:|
//@[018:00024) |   | |   | └─StringSyntax
//@[018:00024) |   | |   |   └─Token(StringComplete) |'test'|
//@[024:00025) |   | |   ├─Token(NewLine) |\n|
    stringParamB: 'test'
//@[004:00024) |   | |   ├─ObjectPropertySyntax
//@[004:00016) |   | |   | ├─IdentifierSyntax
//@[004:00016) |   | |   | | └─Token(Identifier) |stringParamB|
//@[016:00017) |   | |   | ├─Token(Colon) |:|
//@[018:00024) |   | |   | └─StringSyntax
//@[018:00024) |   | |   |   └─Token(StringComplete) |'test'|
//@[024:00025) |   | |   ├─Token(NewLine) |\n|
    notAThing: 'test'
//@[004:00021) |   | |   ├─ObjectPropertySyntax
//@[004:00013) |   | |   | ├─IdentifierSyntax
//@[004:00013) |   | |   | | └─Token(Identifier) |notAThing|
//@[013:00014) |   | |   | ├─Token(Colon) |:|
//@[015:00021) |   | |   | └─StringSyntax
//@[015:00021) |   | |   |   └─Token(StringComplete) |'test'|
//@[021:00022) |   | |   ├─Token(NewLine) |\n|
  }
//@[002:00003) |   | |   └─Token(RightBrace) |}|
//@[003:00004) |   | ├─Token(NewLine) |\n|
}]
//@[000:00001) |   | └─Token(RightBrace) |}|
//@[001:00002) |   └─Token(RightSquare) |]|
//@[002:00003) ├─Token(NewLine) |\n|
module wrongModuleParameterInFilteredLoop 'modulea.bicep' = [for x in emptyArray: if(true) {
//@[000:00284) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00041) | ├─IdentifierSyntax
//@[007:00041) | | └─Token(Identifier) |wrongModuleParameterInFilteredLoop|
//@[042:00057) | ├─StringSyntax
//@[042:00057) | | └─Token(StringComplete) |'modulea.bicep'|
//@[058:00059) | ├─Token(Assignment) |=|
//@[060:00284) | └─ForSyntax
//@[060:00061) |   ├─Token(LeftSquare) |[|
//@[061:00064) |   ├─Token(Identifier) |for|
//@[065:00066) |   ├─LocalVariableSyntax
//@[065:00066) |   | └─IdentifierSyntax
//@[065:00066) |   |   └─Token(Identifier) |x|
//@[067:00069) |   ├─Token(Identifier) |in|
//@[070:00080) |   ├─VariableAccessSyntax
//@[070:00080) |   | └─IdentifierSyntax
//@[070:00080) |   |   └─Token(Identifier) |emptyArray|
//@[080:00081) |   ├─Token(Colon) |:|
//@[082:00283) |   ├─IfConditionSyntax
//@[082:00084) |   | ├─Token(Identifier) |if|
//@[084:00090) |   | ├─ParenthesizedExpressionSyntax
//@[084:00085) |   | | ├─Token(LeftParen) |(|
//@[085:00089) |   | | ├─BooleanLiteralSyntax
//@[085:00089) |   | | | └─Token(TrueKeyword) |true|
//@[089:00090) |   | | └─Token(RightParen) |)|
//@[091:00283) |   | └─ObjectSyntax
//@[091:00092) |   |   ├─Token(LeftBrace) |{|
//@[092:00093) |   |   ├─Token(NewLine) |\n|
  // #completionTest(17) -> symbolsPlusX_if
//@[043:00044) |   |   ├─Token(NewLine) |\n|
  name: 'hello-${x}'
//@[002:00020) |   |   ├─ObjectPropertySyntax
//@[002:00006) |   |   | ├─IdentifierSyntax
//@[002:00006) |   |   | | └─Token(Identifier) |name|
//@[006:00007) |   |   | ├─Token(Colon) |:|
//@[008:00020) |   |   | └─StringSyntax
//@[008:00017) |   |   |   ├─Token(StringLeftPiece) |'hello-${|
//@[017:00018) |   |   |   ├─VariableAccessSyntax
//@[017:00018) |   |   |   | └─IdentifierSyntax
//@[017:00018) |   |   |   |   └─Token(Identifier) |x|
//@[018:00020) |   |   |   └─Token(StringRightPiece) |}'|
//@[020:00021) |   |   ├─Token(NewLine) |\n|
  params: {
//@[002:00123) |   |   ├─ObjectPropertySyntax
//@[002:00008) |   |   | ├─IdentifierSyntax
//@[002:00008) |   |   | | └─Token(Identifier) |params|
//@[008:00009) |   |   | ├─Token(Colon) |:|
//@[010:00123) |   |   | └─ObjectSyntax
//@[010:00011) |   |   |   ├─Token(LeftBrace) |{|
//@[011:00012) |   |   |   ├─Token(NewLine) |\n|
    arrayParam: []
//@[004:00018) |   |   |   ├─ObjectPropertySyntax
//@[004:00014) |   |   |   | ├─IdentifierSyntax
//@[004:00014) |   |   |   | | └─Token(Identifier) |arrayParam|
//@[014:00015) |   |   |   | ├─Token(Colon) |:|
//@[016:00018) |   |   |   | └─ArraySyntax
//@[016:00017) |   |   |   |   ├─Token(LeftSquare) |[|
//@[017:00018) |   |   |   |   └─Token(RightSquare) |]|
//@[018:00019) |   |   |   ├─Token(NewLine) |\n|
    objParam: {}
//@[004:00016) |   |   |   ├─ObjectPropertySyntax
//@[004:00012) |   |   |   | ├─IdentifierSyntax
//@[004:00012) |   |   |   | | └─Token(Identifier) |objParam|
//@[012:00013) |   |   |   | ├─Token(Colon) |:|
//@[014:00016) |   |   |   | └─ObjectSyntax
//@[014:00015) |   |   |   |   ├─Token(LeftBrace) |{|
//@[015:00016) |   |   |   |   └─Token(RightBrace) |}|
//@[016:00017) |   |   |   ├─Token(NewLine) |\n|
    stringParamA: 'test'
//@[004:00024) |   |   |   ├─ObjectPropertySyntax
//@[004:00016) |   |   |   | ├─IdentifierSyntax
//@[004:00016) |   |   |   | | └─Token(Identifier) |stringParamA|
//@[016:00017) |   |   |   | ├─Token(Colon) |:|
//@[018:00024) |   |   |   | └─StringSyntax
//@[018:00024) |   |   |   |   └─Token(StringComplete) |'test'|
//@[024:00025) |   |   |   ├─Token(NewLine) |\n|
    stringParamB: 'test'
//@[004:00024) |   |   |   ├─ObjectPropertySyntax
//@[004:00016) |   |   |   | ├─IdentifierSyntax
//@[004:00016) |   |   |   | | └─Token(Identifier) |stringParamB|
//@[016:00017) |   |   |   | ├─Token(Colon) |:|
//@[018:00024) |   |   |   | └─StringSyntax
//@[018:00024) |   |   |   |   └─Token(StringComplete) |'test'|
//@[024:00025) |   |   |   ├─Token(NewLine) |\n|
    notAThing: 'test'
//@[004:00021) |   |   |   ├─ObjectPropertySyntax
//@[004:00013) |   |   |   | ├─IdentifierSyntax
//@[004:00013) |   |   |   | | └─Token(Identifier) |notAThing|
//@[013:00014) |   |   |   | ├─Token(Colon) |:|
//@[015:00021) |   |   |   | └─StringSyntax
//@[015:00021) |   |   |   |   └─Token(StringComplete) |'test'|
//@[021:00022) |   |   |   ├─Token(NewLine) |\n|
  }
//@[002:00003) |   |   |   └─Token(RightBrace) |}|
//@[003:00004) |   |   ├─Token(NewLine) |\n|
}]
//@[000:00001) |   |   └─Token(RightBrace) |}|
//@[001:00002) |   └─Token(RightSquare) |]|
//@[002:00003) ├─Token(NewLine) |\n|
module wrongModuleParameterInLoop2 'modulea.bicep' = [for (x,i) in emptyArray:{
//@[000:00240) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00034) | ├─IdentifierSyntax
//@[007:00034) | | └─Token(Identifier) |wrongModuleParameterInLoop2|
//@[035:00050) | ├─StringSyntax
//@[035:00050) | | └─Token(StringComplete) |'modulea.bicep'|
//@[051:00052) | ├─Token(Assignment) |=|
//@[053:00240) | └─ForSyntax
//@[053:00054) |   ├─Token(LeftSquare) |[|
//@[054:00057) |   ├─Token(Identifier) |for|
//@[058:00063) |   ├─VariableBlockSyntax
//@[058:00059) |   | ├─Token(LeftParen) |(|
//@[059:00060) |   | ├─LocalVariableSyntax
//@[059:00060) |   | | └─IdentifierSyntax
//@[059:00060) |   | |   └─Token(Identifier) |x|
//@[060:00061) |   | ├─Token(Comma) |,|
//@[061:00062) |   | ├─LocalVariableSyntax
//@[061:00062) |   | | └─IdentifierSyntax
//@[061:00062) |   | |   └─Token(Identifier) |i|
//@[062:00063) |   | └─Token(RightParen) |)|
//@[064:00066) |   ├─Token(Identifier) |in|
//@[067:00077) |   ├─VariableAccessSyntax
//@[067:00077) |   | └─IdentifierSyntax
//@[067:00077) |   |   └─Token(Identifier) |emptyArray|
//@[077:00078) |   ├─Token(Colon) |:|
//@[078:00239) |   ├─ObjectSyntax
//@[078:00079) |   | ├─Token(LeftBrace) |{|
//@[079:00080) |   | ├─Token(NewLine) |\n|
  name: 'hello-${x}'
//@[002:00020) |   | ├─ObjectPropertySyntax
//@[002:00006) |   | | ├─IdentifierSyntax
//@[002:00006) |   | | | └─Token(Identifier) |name|
//@[006:00007) |   | | ├─Token(Colon) |:|
//@[008:00020) |   | | └─StringSyntax
//@[008:00017) |   | |   ├─Token(StringLeftPiece) |'hello-${|
//@[017:00018) |   | |   ├─VariableAccessSyntax
//@[017:00018) |   | |   | └─IdentifierSyntax
//@[017:00018) |   | |   |   └─Token(Identifier) |x|
//@[018:00020) |   | |   └─Token(StringRightPiece) |}'|
//@[020:00021) |   | ├─Token(NewLine) |\n|
  params: {
//@[002:00136) |   | ├─ObjectPropertySyntax
//@[002:00008) |   | | ├─IdentifierSyntax
//@[002:00008) |   | | | └─Token(Identifier) |params|
//@[008:00009) |   | | ├─Token(Colon) |:|
//@[010:00136) |   | | └─ObjectSyntax
//@[010:00011) |   | |   ├─Token(LeftBrace) |{|
//@[011:00012) |   | |   ├─Token(NewLine) |\n|
    arrayParam: [
//@[004:00031) |   | |   ├─ObjectPropertySyntax
//@[004:00014) |   | |   | ├─IdentifierSyntax
//@[004:00014) |   | |   | | └─Token(Identifier) |arrayParam|
//@[014:00015) |   | |   | ├─Token(Colon) |:|
//@[016:00031) |   | |   | └─ArraySyntax
//@[016:00017) |   | |   |   ├─Token(LeftSquare) |[|
//@[017:00018) |   | |   |   ├─Token(NewLine) |\n|
      i
//@[006:00007) |   | |   |   ├─ArrayItemSyntax
//@[006:00007) |   | |   |   | └─VariableAccessSyntax
//@[006:00007) |   | |   |   |   └─IdentifierSyntax
//@[006:00007) |   | |   |   |     └─Token(Identifier) |i|
//@[007:00008) |   | |   |   ├─Token(NewLine) |\n|
    ]
//@[004:00005) |   | |   |   └─Token(RightSquare) |]|
//@[005:00006) |   | |   ├─Token(NewLine) |\n|
    objParam: {}
//@[004:00016) |   | |   ├─ObjectPropertySyntax
//@[004:00012) |   | |   | ├─IdentifierSyntax
//@[004:00012) |   | |   | | └─Token(Identifier) |objParam|
//@[012:00013) |   | |   | ├─Token(Colon) |:|
//@[014:00016) |   | |   | └─ObjectSyntax
//@[014:00015) |   | |   |   ├─Token(LeftBrace) |{|
//@[015:00016) |   | |   |   └─Token(RightBrace) |}|
//@[016:00017) |   | |   ├─Token(NewLine) |\n|
    stringParamA: 'test'
//@[004:00024) |   | |   ├─ObjectPropertySyntax
//@[004:00016) |   | |   | ├─IdentifierSyntax
//@[004:00016) |   | |   | | └─Token(Identifier) |stringParamA|
//@[016:00017) |   | |   | ├─Token(Colon) |:|
//@[018:00024) |   | |   | └─StringSyntax
//@[018:00024) |   | |   |   └─Token(StringComplete) |'test'|
//@[024:00025) |   | |   ├─Token(NewLine) |\n|
    stringParamB: 'test'
//@[004:00024) |   | |   ├─ObjectPropertySyntax
//@[004:00016) |   | |   | ├─IdentifierSyntax
//@[004:00016) |   | |   | | └─Token(Identifier) |stringParamB|
//@[016:00017) |   | |   | ├─Token(Colon) |:|
//@[018:00024) |   | |   | └─StringSyntax
//@[018:00024) |   | |   |   └─Token(StringComplete) |'test'|
//@[024:00025) |   | |   ├─Token(NewLine) |\n|
    notAThing: 'test'
//@[004:00021) |   | |   ├─ObjectPropertySyntax
//@[004:00013) |   | |   | ├─IdentifierSyntax
//@[004:00013) |   | |   | | └─Token(Identifier) |notAThing|
//@[013:00014) |   | |   | ├─Token(Colon) |:|
//@[015:00021) |   | |   | └─StringSyntax
//@[015:00021) |   | |   |   └─Token(StringComplete) |'test'|
//@[021:00022) |   | |   ├─Token(NewLine) |\n|
  }
//@[002:00003) |   | |   └─Token(RightBrace) |}|
//@[003:00004) |   | ├─Token(NewLine) |\n|
}]
//@[000:00001) |   | └─Token(RightBrace) |}|
//@[001:00002) |   └─Token(RightSquare) |]|
//@[002:00004) ├─Token(NewLine) |\n\n|

module paramNameCompletionsInFilteredLoops 'modulea.bicep' = [for (x,i) in emptyArray: if(true) {
//@[000:00187) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00042) | ├─IdentifierSyntax
//@[007:00042) | | └─Token(Identifier) |paramNameCompletionsInFilteredLoops|
//@[043:00058) | ├─StringSyntax
//@[043:00058) | | └─Token(StringComplete) |'modulea.bicep'|
//@[059:00060) | ├─Token(Assignment) |=|
//@[061:00187) | └─ForSyntax
//@[061:00062) |   ├─Token(LeftSquare) |[|
//@[062:00065) |   ├─Token(Identifier) |for|
//@[066:00071) |   ├─VariableBlockSyntax
//@[066:00067) |   | ├─Token(LeftParen) |(|
//@[067:00068) |   | ├─LocalVariableSyntax
//@[067:00068) |   | | └─IdentifierSyntax
//@[067:00068) |   | |   └─Token(Identifier) |x|
//@[068:00069) |   | ├─Token(Comma) |,|
//@[069:00070) |   | ├─LocalVariableSyntax
//@[069:00070) |   | | └─IdentifierSyntax
//@[069:00070) |   | |   └─Token(Identifier) |i|
//@[070:00071) |   | └─Token(RightParen) |)|
//@[072:00074) |   ├─Token(Identifier) |in|
//@[075:00085) |   ├─VariableAccessSyntax
//@[075:00085) |   | └─IdentifierSyntax
//@[075:00085) |   |   └─Token(Identifier) |emptyArray|
//@[085:00086) |   ├─Token(Colon) |:|
//@[087:00186) |   ├─IfConditionSyntax
//@[087:00089) |   | ├─Token(Identifier) |if|
//@[089:00095) |   | ├─ParenthesizedExpressionSyntax
//@[089:00090) |   | | ├─Token(LeftParen) |(|
//@[090:00094) |   | | ├─BooleanLiteralSyntax
//@[090:00094) |   | | | └─Token(TrueKeyword) |true|
//@[094:00095) |   | | └─Token(RightParen) |)|
//@[096:00186) |   | └─ObjectSyntax
//@[096:00097) |   |   ├─Token(LeftBrace) |{|
//@[097:00098) |   |   ├─Token(NewLine) |\n|
  name: 'hello-${x}'
//@[002:00020) |   |   ├─ObjectPropertySyntax
//@[002:00006) |   |   | ├─IdentifierSyntax
//@[002:00006) |   |   | | └─Token(Identifier) |name|
//@[006:00007) |   |   | ├─Token(Colon) |:|
//@[008:00020) |   |   | └─StringSyntax
//@[008:00017) |   |   |   ├─Token(StringLeftPiece) |'hello-${|
//@[017:00018) |   |   |   ├─VariableAccessSyntax
//@[017:00018) |   |   |   | └─IdentifierSyntax
//@[017:00018) |   |   |   |   └─Token(Identifier) |x|
//@[018:00020) |   |   |   └─Token(StringRightPiece) |}'|
//@[020:00021) |   |   ├─Token(NewLine) |\n|
  params: {
//@[002:00065) |   |   ├─ObjectPropertySyntax
//@[002:00008) |   |   | ├─IdentifierSyntax
//@[002:00008) |   |   | | └─Token(Identifier) |params|
//@[008:00009) |   |   | ├─Token(Colon) |:|
//@[010:00065) |   |   | └─ObjectSyntax
//@[010:00011) |   |   |   ├─Token(LeftBrace) |{|
//@[011:00012) |   |   |   ├─Token(NewLine) |\n|
    // #completionTest(0,1,2) -> moduleAParams
//@[046:00047) |   |   |   ├─Token(NewLine) |\n|
  
//@[002:00003) |   |   |   ├─Token(NewLine) |\n|
  }
//@[002:00003) |   |   |   └─Token(RightBrace) |}|
//@[003:00004) |   |   ├─Token(NewLine) |\n|
}]
//@[000:00001) |   |   └─Token(RightBrace) |}|
//@[001:00002) |   └─Token(RightSquare) |]|
//@[002:00004) ├─Token(NewLine) |\n\n|

// #completionTest(100) -> moduleAOutputs
//@[041:00042) ├─Token(NewLine) |\n|
var propertyAccessCompletionsForFilteredModuleLoop = paramNameCompletionsInFilteredLoops[0].outputs.
//@[000:00100) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00050) | ├─IdentifierSyntax
//@[004:00050) | | └─Token(Identifier) |propertyAccessCompletionsForFilteredModuleLoop|
//@[051:00052) | ├─Token(Assignment) |=|
//@[053:00100) | └─PropertyAccessSyntax
//@[053:00099) |   ├─PropertyAccessSyntax
//@[053:00091) |   | ├─ArrayAccessSyntax
//@[053:00088) |   | | ├─VariableAccessSyntax
//@[053:00088) |   | | | └─IdentifierSyntax
//@[053:00088) |   | | |   └─Token(Identifier) |paramNameCompletionsInFilteredLoops|
//@[088:00089) |   | | ├─Token(LeftSquare) |[|
//@[089:00090) |   | | ├─IntegerLiteralSyntax
//@[089:00090) |   | | | └─Token(Integer) |0|
//@[090:00091) |   | | └─Token(RightSquare) |]|
//@[091:00092) |   | ├─Token(Dot) |.|
//@[092:00099) |   | └─IdentifierSyntax
//@[092:00099) |   |   └─Token(Identifier) |outputs|
//@[099:00100) |   ├─Token(Dot) |.|
//@[100:00100) |   └─IdentifierSyntax
//@[100:00100) |     └─SkippedTriviaSyntax
//@[100:00102) ├─Token(NewLine) |\n\n|

// nonexistent arrays and loop variables
//@[040:00041) ├─Token(NewLine) |\n|
var evenMoreDuplicates = 'there'
//@[000:00032) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00022) | ├─IdentifierSyntax
//@[004:00022) | | └─Token(Identifier) |evenMoreDuplicates|
//@[023:00024) | ├─Token(Assignment) |=|
//@[025:00032) | └─StringSyntax
//@[025:00032) |   └─Token(StringComplete) |'there'|
//@[032:00033) ├─Token(NewLine) |\n|
module nonexistentArrays 'modulea.bicep' = [for evenMoreDuplicates in alsoDoesNotExist: {
//@[000:00278) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00024) | ├─IdentifierSyntax
//@[007:00024) | | └─Token(Identifier) |nonexistentArrays|
//@[025:00040) | ├─StringSyntax
//@[025:00040) | | └─Token(StringComplete) |'modulea.bicep'|
//@[041:00042) | ├─Token(Assignment) |=|
//@[043:00278) | └─ForSyntax
//@[043:00044) |   ├─Token(LeftSquare) |[|
//@[044:00047) |   ├─Token(Identifier) |for|
//@[048:00066) |   ├─LocalVariableSyntax
//@[048:00066) |   | └─IdentifierSyntax
//@[048:00066) |   |   └─Token(Identifier) |evenMoreDuplicates|
//@[067:00069) |   ├─Token(Identifier) |in|
//@[070:00086) |   ├─VariableAccessSyntax
//@[070:00086) |   | └─IdentifierSyntax
//@[070:00086) |   |   └─Token(Identifier) |alsoDoesNotExist|
//@[086:00087) |   ├─Token(Colon) |:|
//@[088:00277) |   ├─ObjectSyntax
//@[088:00089) |   | ├─Token(LeftBrace) |{|
//@[089:00090) |   | ├─Token(NewLine) |\n|
  name: 'hello-${whyChooseRealVariablesWhenWeCanPretend}'
//@[002:00057) |   | ├─ObjectPropertySyntax
//@[002:00006) |   | | ├─IdentifierSyntax
//@[002:00006) |   | | | └─Token(Identifier) |name|
//@[006:00007) |   | | ├─Token(Colon) |:|
//@[008:00057) |   | | └─StringSyntax
//@[008:00017) |   | |   ├─Token(StringLeftPiece) |'hello-${|
//@[017:00055) |   | |   ├─VariableAccessSyntax
//@[017:00055) |   | |   | └─IdentifierSyntax
//@[017:00055) |   | |   |   └─Token(Identifier) |whyChooseRealVariablesWhenWeCanPretend|
//@[055:00057) |   | |   └─Token(StringRightPiece) |}'|
//@[057:00058) |   | ├─Token(NewLine) |\n|
  params: {
//@[002:00127) |   | ├─ObjectPropertySyntax
//@[002:00008) |   | | ├─IdentifierSyntax
//@[002:00008) |   | | | └─Token(Identifier) |params|
//@[008:00009) |   | | ├─Token(Colon) |:|
//@[010:00127) |   | | └─ObjectSyntax
//@[010:00011) |   | |   ├─Token(LeftBrace) |{|
//@[011:00012) |   | |   ├─Token(NewLine) |\n|
    objParam: {}
//@[004:00016) |   | |   ├─ObjectPropertySyntax
//@[004:00012) |   | |   | ├─IdentifierSyntax
//@[004:00012) |   | |   | | └─Token(Identifier) |objParam|
//@[012:00013) |   | |   | ├─Token(Colon) |:|
//@[014:00016) |   | |   | └─ObjectSyntax
//@[014:00015) |   | |   |   ├─Token(LeftBrace) |{|
//@[015:00016) |   | |   |   └─Token(RightBrace) |}|
//@[016:00017) |   | |   ├─Token(NewLine) |\n|
    stringParamB: 'test'
//@[004:00024) |   | |   ├─ObjectPropertySyntax
//@[004:00016) |   | |   | ├─IdentifierSyntax
//@[004:00016) |   | |   | | └─Token(Identifier) |stringParamB|
//@[016:00017) |   | |   | ├─Token(Colon) |:|
//@[018:00024) |   | |   | └─StringSyntax
//@[018:00024) |   | |   |   └─Token(StringComplete) |'test'|
//@[024:00025) |   | |   ├─Token(NewLine) |\n|
    arrayParam: [for evenMoreDuplicates in totallyFake: doesNotExist]
//@[004:00069) |   | |   ├─ObjectPropertySyntax
//@[004:00014) |   | |   | ├─IdentifierSyntax
//@[004:00014) |   | |   | | └─Token(Identifier) |arrayParam|
//@[014:00015) |   | |   | ├─Token(Colon) |:|
//@[016:00069) |   | |   | └─ForSyntax
//@[016:00017) |   | |   |   ├─Token(LeftSquare) |[|
//@[017:00020) |   | |   |   ├─Token(Identifier) |for|
//@[021:00039) |   | |   |   ├─LocalVariableSyntax
//@[021:00039) |   | |   |   | └─IdentifierSyntax
//@[021:00039) |   | |   |   |   └─Token(Identifier) |evenMoreDuplicates|
//@[040:00042) |   | |   |   ├─Token(Identifier) |in|
//@[043:00054) |   | |   |   ├─VariableAccessSyntax
//@[043:00054) |   | |   |   | └─IdentifierSyntax
//@[043:00054) |   | |   |   |   └─Token(Identifier) |totallyFake|
//@[054:00055) |   | |   |   ├─Token(Colon) |:|
//@[056:00068) |   | |   |   ├─VariableAccessSyntax
//@[056:00068) |   | |   |   | └─IdentifierSyntax
//@[056:00068) |   | |   |   |   └─Token(Identifier) |doesNotExist|
//@[068:00069) |   | |   |   └─Token(RightSquare) |]|
//@[069:00070) |   | |   ├─Token(NewLine) |\n|
  }
//@[002:00003) |   | |   └─Token(RightBrace) |}|
//@[003:00004) |   | ├─Token(NewLine) |\n|
}]
//@[000:00001) |   | └─Token(RightBrace) |}|
//@[001:00002) |   └─Token(RightSquare) |]|
//@[002:00004) ├─Token(NewLine) |\n\n|

output directRefToCollectionViaOutput array = nonexistentArrays
//@[000:00063) ├─OutputDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |output|
//@[007:00037) | ├─IdentifierSyntax
//@[007:00037) | | └─Token(Identifier) |directRefToCollectionViaOutput|
//@[038:00043) | ├─SimpleTypeSyntax
//@[038:00043) | | └─Token(Identifier) |array|
//@[044:00045) | ├─Token(Assignment) |=|
//@[046:00063) | └─VariableAccessSyntax
//@[046:00063) |   └─IdentifierSyntax
//@[046:00063) |     └─Token(Identifier) |nonexistentArrays|
//@[063:00065) ├─Token(NewLine) |\n\n|

module directRefToCollectionViaSingleBody 'modulea.bicep' = {
//@[000:00203) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00041) | ├─IdentifierSyntax
//@[007:00041) | | └─Token(Identifier) |directRefToCollectionViaSingleBody|
//@[042:00057) | ├─StringSyntax
//@[042:00057) | | └─Token(StringComplete) |'modulea.bicep'|
//@[058:00059) | ├─Token(Assignment) |=|
//@[060:00203) | └─ObjectSyntax
//@[060:00061) |   ├─Token(LeftBrace) |{|
//@[061:00062) |   ├─Token(NewLine) |\n|
  name: 'hello'
//@[002:00015) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00015) |   | └─StringSyntax
//@[008:00015) |   |   └─Token(StringComplete) |'hello'|
//@[015:00016) |   ├─Token(NewLine) |\n|
  params: {
//@[002:00123) |   ├─ObjectPropertySyntax
//@[002:00008) |   | ├─IdentifierSyntax
//@[002:00008) |   | | └─Token(Identifier) |params|
//@[008:00009) |   | ├─Token(Colon) |:|
//@[010:00123) |   | └─ObjectSyntax
//@[010:00011) |   |   ├─Token(LeftBrace) |{|
//@[011:00012) |   |   ├─Token(NewLine) |\n|
    arrayParam: concat(wrongModuleParameterInLoop, nonexistentArrays)
//@[004:00069) |   |   ├─ObjectPropertySyntax
//@[004:00014) |   |   | ├─IdentifierSyntax
//@[004:00014) |   |   | | └─Token(Identifier) |arrayParam|
//@[014:00015) |   |   | ├─Token(Colon) |:|
//@[016:00069) |   |   | └─FunctionCallSyntax
//@[016:00022) |   |   |   ├─IdentifierSyntax
//@[016:00022) |   |   |   | └─Token(Identifier) |concat|
//@[022:00023) |   |   |   ├─Token(LeftParen) |(|
//@[023:00049) |   |   |   ├─FunctionArgumentSyntax
//@[023:00049) |   |   |   | └─VariableAccessSyntax
//@[023:00049) |   |   |   |   └─IdentifierSyntax
//@[023:00049) |   |   |   |     └─Token(Identifier) |wrongModuleParameterInLoop|
//@[049:00050) |   |   |   ├─Token(Comma) |,|
//@[051:00068) |   |   |   ├─FunctionArgumentSyntax
//@[051:00068) |   |   |   | └─VariableAccessSyntax
//@[051:00068) |   |   |   |   └─IdentifierSyntax
//@[051:00068) |   |   |   |     └─Token(Identifier) |nonexistentArrays|
//@[068:00069) |   |   |   └─Token(RightParen) |)|
//@[069:00070) |   |   ├─Token(NewLine) |\n|
    objParam: {}
//@[004:00016) |   |   ├─ObjectPropertySyntax
//@[004:00012) |   |   | ├─IdentifierSyntax
//@[004:00012) |   |   | | └─Token(Identifier) |objParam|
//@[012:00013) |   |   | ├─Token(Colon) |:|
//@[014:00016) |   |   | └─ObjectSyntax
//@[014:00015) |   |   |   ├─Token(LeftBrace) |{|
//@[015:00016) |   |   |   └─Token(RightBrace) |}|
//@[016:00017) |   |   ├─Token(NewLine) |\n|
    stringParamB: ''
//@[004:00020) |   |   ├─ObjectPropertySyntax
//@[004:00016) |   |   | ├─IdentifierSyntax
//@[004:00016) |   |   | | └─Token(Identifier) |stringParamB|
//@[016:00017) |   |   | ├─Token(Colon) |:|
//@[018:00020) |   |   | └─StringSyntax
//@[018:00020) |   |   |   └─Token(StringComplete) |''|
//@[020:00021) |   |   ├─Token(NewLine) |\n|
  }
//@[002:00003) |   |   └─Token(RightBrace) |}|
//@[003:00004) |   ├─Token(NewLine) |\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00003) ├─Token(NewLine) |\n\n|

module directRefToCollectionViaSingleConditionalBody 'modulea.bicep' = if(true) {
//@[000:00224) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00052) | ├─IdentifierSyntax
//@[007:00052) | | └─Token(Identifier) |directRefToCollectionViaSingleConditionalBody|
//@[053:00068) | ├─StringSyntax
//@[053:00068) | | └─Token(StringComplete) |'modulea.bicep'|
//@[069:00070) | ├─Token(Assignment) |=|
//@[071:00224) | └─IfConditionSyntax
//@[071:00073) |   ├─Token(Identifier) |if|
//@[073:00079) |   ├─ParenthesizedExpressionSyntax
//@[073:00074) |   | ├─Token(LeftParen) |(|
//@[074:00078) |   | ├─BooleanLiteralSyntax
//@[074:00078) |   | | └─Token(TrueKeyword) |true|
//@[078:00079) |   | └─Token(RightParen) |)|
//@[080:00224) |   └─ObjectSyntax
//@[080:00081) |     ├─Token(LeftBrace) |{|
//@[081:00082) |     ├─Token(NewLine) |\n|
  name: 'hello2'
//@[002:00016) |     ├─ObjectPropertySyntax
//@[002:00006) |     | ├─IdentifierSyntax
//@[002:00006) |     | | └─Token(Identifier) |name|
//@[006:00007) |     | ├─Token(Colon) |:|
//@[008:00016) |     | └─StringSyntax
//@[008:00016) |     |   └─Token(StringComplete) |'hello2'|
//@[016:00017) |     ├─Token(NewLine) |\n|
  params: {
//@[002:00123) |     ├─ObjectPropertySyntax
//@[002:00008) |     | ├─IdentifierSyntax
//@[002:00008) |     | | └─Token(Identifier) |params|
//@[008:00009) |     | ├─Token(Colon) |:|
//@[010:00123) |     | └─ObjectSyntax
//@[010:00011) |     |   ├─Token(LeftBrace) |{|
//@[011:00012) |     |   ├─Token(NewLine) |\n|
    arrayParam: concat(wrongModuleParameterInLoop, nonexistentArrays)
//@[004:00069) |     |   ├─ObjectPropertySyntax
//@[004:00014) |     |   | ├─IdentifierSyntax
//@[004:00014) |     |   | | └─Token(Identifier) |arrayParam|
//@[014:00015) |     |   | ├─Token(Colon) |:|
//@[016:00069) |     |   | └─FunctionCallSyntax
//@[016:00022) |     |   |   ├─IdentifierSyntax
//@[016:00022) |     |   |   | └─Token(Identifier) |concat|
//@[022:00023) |     |   |   ├─Token(LeftParen) |(|
//@[023:00049) |     |   |   ├─FunctionArgumentSyntax
//@[023:00049) |     |   |   | └─VariableAccessSyntax
//@[023:00049) |     |   |   |   └─IdentifierSyntax
//@[023:00049) |     |   |   |     └─Token(Identifier) |wrongModuleParameterInLoop|
//@[049:00050) |     |   |   ├─Token(Comma) |,|
//@[051:00068) |     |   |   ├─FunctionArgumentSyntax
//@[051:00068) |     |   |   | └─VariableAccessSyntax
//@[051:00068) |     |   |   |   └─IdentifierSyntax
//@[051:00068) |     |   |   |     └─Token(Identifier) |nonexistentArrays|
//@[068:00069) |     |   |   └─Token(RightParen) |)|
//@[069:00070) |     |   ├─Token(NewLine) |\n|
    objParam: {}
//@[004:00016) |     |   ├─ObjectPropertySyntax
//@[004:00012) |     |   | ├─IdentifierSyntax
//@[004:00012) |     |   | | └─Token(Identifier) |objParam|
//@[012:00013) |     |   | ├─Token(Colon) |:|
//@[014:00016) |     |   | └─ObjectSyntax
//@[014:00015) |     |   |   ├─Token(LeftBrace) |{|
//@[015:00016) |     |   |   └─Token(RightBrace) |}|
//@[016:00017) |     |   ├─Token(NewLine) |\n|
    stringParamB: ''
//@[004:00020) |     |   ├─ObjectPropertySyntax
//@[004:00016) |     |   | ├─IdentifierSyntax
//@[004:00016) |     |   | | └─Token(Identifier) |stringParamB|
//@[016:00017) |     |   | ├─Token(Colon) |:|
//@[018:00020) |     |   | └─StringSyntax
//@[018:00020) |     |   |   └─Token(StringComplete) |''|
//@[020:00021) |     |   ├─Token(NewLine) |\n|
  }
//@[002:00003) |     |   └─Token(RightBrace) |}|
//@[003:00004) |     ├─Token(NewLine) |\n|
}
//@[000:00001) |     └─Token(RightBrace) |}|
//@[001:00003) ├─Token(NewLine) |\n\n|

module directRefToCollectionViaLoopBody 'modulea.bicep' = [for test in []: {
//@[000:00220) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00039) | ├─IdentifierSyntax
//@[007:00039) | | └─Token(Identifier) |directRefToCollectionViaLoopBody|
//@[040:00055) | ├─StringSyntax
//@[040:00055) | | └─Token(StringComplete) |'modulea.bicep'|
//@[056:00057) | ├─Token(Assignment) |=|
//@[058:00220) | └─ForSyntax
//@[058:00059) |   ├─Token(LeftSquare) |[|
//@[059:00062) |   ├─Token(Identifier) |for|
//@[063:00067) |   ├─LocalVariableSyntax
//@[063:00067) |   | └─IdentifierSyntax
//@[063:00067) |   |   └─Token(Identifier) |test|
//@[068:00070) |   ├─Token(Identifier) |in|
//@[071:00073) |   ├─ArraySyntax
//@[071:00072) |   | ├─Token(LeftSquare) |[|
//@[072:00073) |   | └─Token(RightSquare) |]|
//@[073:00074) |   ├─Token(Colon) |:|
//@[075:00219) |   ├─ObjectSyntax
//@[075:00076) |   | ├─Token(LeftBrace) |{|
//@[076:00077) |   | ├─Token(NewLine) |\n|
  name: 'hello3'
//@[002:00016) |   | ├─ObjectPropertySyntax
//@[002:00006) |   | | ├─IdentifierSyntax
//@[002:00006) |   | | | └─Token(Identifier) |name|
//@[006:00007) |   | | ├─Token(Colon) |:|
//@[008:00016) |   | | └─StringSyntax
//@[008:00016) |   | |   └─Token(StringComplete) |'hello3'|
//@[016:00017) |   | ├─Token(NewLine) |\n|
  params: {
//@[002:00123) |   | ├─ObjectPropertySyntax
//@[002:00008) |   | | ├─IdentifierSyntax
//@[002:00008) |   | | | └─Token(Identifier) |params|
//@[008:00009) |   | | ├─Token(Colon) |:|
//@[010:00123) |   | | └─ObjectSyntax
//@[010:00011) |   | |   ├─Token(LeftBrace) |{|
//@[011:00012) |   | |   ├─Token(NewLine) |\n|
    arrayParam: concat(wrongModuleParameterInLoop, nonexistentArrays)
//@[004:00069) |   | |   ├─ObjectPropertySyntax
//@[004:00014) |   | |   | ├─IdentifierSyntax
//@[004:00014) |   | |   | | └─Token(Identifier) |arrayParam|
//@[014:00015) |   | |   | ├─Token(Colon) |:|
//@[016:00069) |   | |   | └─FunctionCallSyntax
//@[016:00022) |   | |   |   ├─IdentifierSyntax
//@[016:00022) |   | |   |   | └─Token(Identifier) |concat|
//@[022:00023) |   | |   |   ├─Token(LeftParen) |(|
//@[023:00049) |   | |   |   ├─FunctionArgumentSyntax
//@[023:00049) |   | |   |   | └─VariableAccessSyntax
//@[023:00049) |   | |   |   |   └─IdentifierSyntax
//@[023:00049) |   | |   |   |     └─Token(Identifier) |wrongModuleParameterInLoop|
//@[049:00050) |   | |   |   ├─Token(Comma) |,|
//@[051:00068) |   | |   |   ├─FunctionArgumentSyntax
//@[051:00068) |   | |   |   | └─VariableAccessSyntax
//@[051:00068) |   | |   |   |   └─IdentifierSyntax
//@[051:00068) |   | |   |   |     └─Token(Identifier) |nonexistentArrays|
//@[068:00069) |   | |   |   └─Token(RightParen) |)|
//@[069:00070) |   | |   ├─Token(NewLine) |\n|
    objParam: {}
//@[004:00016) |   | |   ├─ObjectPropertySyntax
//@[004:00012) |   | |   | ├─IdentifierSyntax
//@[004:00012) |   | |   | | └─Token(Identifier) |objParam|
//@[012:00013) |   | |   | ├─Token(Colon) |:|
//@[014:00016) |   | |   | └─ObjectSyntax
//@[014:00015) |   | |   |   ├─Token(LeftBrace) |{|
//@[015:00016) |   | |   |   └─Token(RightBrace) |}|
//@[016:00017) |   | |   ├─Token(NewLine) |\n|
    stringParamB: ''
//@[004:00020) |   | |   ├─ObjectPropertySyntax
//@[004:00016) |   | |   | ├─IdentifierSyntax
//@[004:00016) |   | |   | | └─Token(Identifier) |stringParamB|
//@[016:00017) |   | |   | ├─Token(Colon) |:|
//@[018:00020) |   | |   | └─StringSyntax
//@[018:00020) |   | |   |   └─Token(StringComplete) |''|
//@[020:00021) |   | |   ├─Token(NewLine) |\n|
  }
//@[002:00003) |   | |   └─Token(RightBrace) |}|
//@[003:00004) |   | ├─Token(NewLine) |\n|
}]
//@[000:00001) |   | └─Token(RightBrace) |}|
//@[001:00002) |   └─Token(RightSquare) |]|
//@[002:00004) ├─Token(NewLine) |\n\n|

module directRefToCollectionViaLoopBodyWithExtraDependsOn 'modulea.bicep' = [for test in []: {
//@[000:00309) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00057) | ├─IdentifierSyntax
//@[007:00057) | | └─Token(Identifier) |directRefToCollectionViaLoopBodyWithExtraDependsOn|
//@[058:00073) | ├─StringSyntax
//@[058:00073) | | └─Token(StringComplete) |'modulea.bicep'|
//@[074:00075) | ├─Token(Assignment) |=|
//@[076:00309) | └─ForSyntax
//@[076:00077) |   ├─Token(LeftSquare) |[|
//@[077:00080) |   ├─Token(Identifier) |for|
//@[081:00085) |   ├─LocalVariableSyntax
//@[081:00085) |   | └─IdentifierSyntax
//@[081:00085) |   |   └─Token(Identifier) |test|
//@[086:00088) |   ├─Token(Identifier) |in|
//@[089:00091) |   ├─ArraySyntax
//@[089:00090) |   | ├─Token(LeftSquare) |[|
//@[090:00091) |   | └─Token(RightSquare) |]|
//@[091:00092) |   ├─Token(Colon) |:|
//@[093:00308) |   ├─ObjectSyntax
//@[093:00094) |   | ├─Token(LeftBrace) |{|
//@[094:00095) |   | ├─Token(NewLine) |\n|
  name: 'hello4'
//@[002:00016) |   | ├─ObjectPropertySyntax
//@[002:00006) |   | | ├─IdentifierSyntax
//@[002:00006) |   | | | └─Token(Identifier) |name|
//@[006:00007) |   | | ├─Token(Colon) |:|
//@[008:00016) |   | | └─StringSyntax
//@[008:00016) |   | |   └─Token(StringComplete) |'hello4'|
//@[016:00017) |   | ├─Token(NewLine) |\n|
  params: {
//@[002:00170) |   | ├─ObjectPropertySyntax
//@[002:00008) |   | | ├─IdentifierSyntax
//@[002:00008) |   | | | └─Token(Identifier) |params|
//@[008:00009) |   | | ├─Token(Colon) |:|
//@[010:00170) |   | | └─ObjectSyntax
//@[010:00011) |   | |   ├─Token(LeftBrace) |{|
//@[011:00012) |   | |   ├─Token(NewLine) |\n|
    arrayParam: concat(wrongModuleParameterInLoop, nonexistentArrays)
//@[004:00069) |   | |   ├─ObjectPropertySyntax
//@[004:00014) |   | |   | ├─IdentifierSyntax
//@[004:00014) |   | |   | | └─Token(Identifier) |arrayParam|
//@[014:00015) |   | |   | ├─Token(Colon) |:|
//@[016:00069) |   | |   | └─FunctionCallSyntax
//@[016:00022) |   | |   |   ├─IdentifierSyntax
//@[016:00022) |   | |   |   | └─Token(Identifier) |concat|
//@[022:00023) |   | |   |   ├─Token(LeftParen) |(|
//@[023:00049) |   | |   |   ├─FunctionArgumentSyntax
//@[023:00049) |   | |   |   | └─VariableAccessSyntax
//@[023:00049) |   | |   |   |   └─IdentifierSyntax
//@[023:00049) |   | |   |   |     └─Token(Identifier) |wrongModuleParameterInLoop|
//@[049:00050) |   | |   |   ├─Token(Comma) |,|
//@[051:00068) |   | |   |   ├─FunctionArgumentSyntax
//@[051:00068) |   | |   |   | └─VariableAccessSyntax
//@[051:00068) |   | |   |   |   └─IdentifierSyntax
//@[051:00068) |   | |   |   |     └─Token(Identifier) |nonexistentArrays|
//@[068:00069) |   | |   |   └─Token(RightParen) |)|
//@[069:00070) |   | |   ├─Token(NewLine) |\n|
    objParam: {}
//@[004:00016) |   | |   ├─ObjectPropertySyntax
//@[004:00012) |   | |   | ├─IdentifierSyntax
//@[004:00012) |   | |   | | └─Token(Identifier) |objParam|
//@[012:00013) |   | |   | ├─Token(Colon) |:|
//@[014:00016) |   | |   | └─ObjectSyntax
//@[014:00015) |   | |   |   ├─Token(LeftBrace) |{|
//@[015:00016) |   | |   |   └─Token(RightBrace) |}|
//@[016:00017) |   | |   ├─Token(NewLine) |\n|
    stringParamB: ''
//@[004:00020) |   | |   ├─ObjectPropertySyntax
//@[004:00016) |   | |   | ├─IdentifierSyntax
//@[004:00016) |   | |   | | └─Token(Identifier) |stringParamB|
//@[016:00017) |   | |   | ├─Token(Colon) |:|
//@[018:00020) |   | |   | └─StringSyntax
//@[018:00020) |   | |   |   └─Token(StringComplete) |''|
//@[020:00021) |   | |   ├─Token(NewLine) |\n|
    dependsOn: [
//@[004:00046) |   | |   ├─ObjectPropertySyntax
//@[004:00013) |   | |   | ├─IdentifierSyntax
//@[004:00013) |   | |   | | └─Token(Identifier) |dependsOn|
//@[013:00014) |   | |   | ├─Token(Colon) |:|
//@[015:00046) |   | |   | └─ArraySyntax
//@[015:00016) |   | |   |   ├─Token(LeftSquare) |[|
//@[016:00017) |   | |   |   ├─Token(NewLine) |\n|
      nonexistentArrays
//@[006:00023) |   | |   |   ├─ArrayItemSyntax
//@[006:00023) |   | |   |   | └─VariableAccessSyntax
//@[006:00023) |   | |   |   |   └─IdentifierSyntax
//@[006:00023) |   | |   |   |     └─Token(Identifier) |nonexistentArrays|
//@[023:00024) |   | |   |   ├─Token(NewLine) |\n|
    ]
//@[004:00005) |   | |   |   └─Token(RightSquare) |]|
//@[005:00006) |   | |   ├─Token(NewLine) |\n|
  }
//@[002:00003) |   | |   └─Token(RightBrace) |}|
//@[003:00004) |   | ├─Token(NewLine) |\n|
  dependsOn: [
//@[002:00023) |   | ├─ObjectPropertySyntax
//@[002:00011) |   | | ├─IdentifierSyntax
//@[002:00011) |   | | | └─Token(Identifier) |dependsOn|
//@[011:00012) |   | | ├─Token(Colon) |:|
//@[013:00023) |   | | └─ArraySyntax
//@[013:00014) |   | |   ├─Token(LeftSquare) |[|
//@[014:00015) |   | |   ├─Token(NewLine) |\n|
    
//@[004:00005) |   | |   ├─Token(NewLine) |\n|
  ]
//@[002:00003) |   | |   └─Token(RightSquare) |]|
//@[003:00004) |   | ├─Token(NewLine) |\n|
}]
//@[000:00001) |   | └─Token(RightBrace) |}|
//@[001:00002) |   └─Token(RightSquare) |]|
//@[002:00005) ├─Token(NewLine) |\n\n\n|


// module body that isn't an object
//@[035:00036) ├─Token(NewLine) |\n|
module nonObjectModuleBody 'modulea.bicep' = [for thing in []: 'hello']
//@[000:00071) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00026) | ├─IdentifierSyntax
//@[007:00026) | | └─Token(Identifier) |nonObjectModuleBody|
//@[027:00042) | ├─StringSyntax
//@[027:00042) | | └─Token(StringComplete) |'modulea.bicep'|
//@[043:00044) | ├─Token(Assignment) |=|
//@[045:00071) | └─ForSyntax
//@[045:00046) |   ├─Token(LeftSquare) |[|
//@[046:00049) |   ├─Token(Identifier) |for|
//@[050:00055) |   ├─LocalVariableSyntax
//@[050:00055) |   | └─IdentifierSyntax
//@[050:00055) |   |   └─Token(Identifier) |thing|
//@[056:00058) |   ├─Token(Identifier) |in|
//@[059:00061) |   ├─ArraySyntax
//@[059:00060) |   | ├─Token(LeftSquare) |[|
//@[060:00061) |   | └─Token(RightSquare) |]|
//@[061:00062) |   ├─Token(Colon) |:|
//@[063:00070) |   ├─SkippedTriviaSyntax
//@[063:00070) |   | └─Token(StringComplete) |'hello'|
//@[070:00071) |   └─Token(RightSquare) |]|
//@[071:00072) ├─Token(NewLine) |\n|
module nonObjectModuleBody2 'modulea.bicep' = [for thing in []: concat()]
//@[000:00073) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00027) | ├─IdentifierSyntax
//@[007:00027) | | └─Token(Identifier) |nonObjectModuleBody2|
//@[028:00043) | ├─StringSyntax
//@[028:00043) | | └─Token(StringComplete) |'modulea.bicep'|
//@[044:00045) | ├─Token(Assignment) |=|
//@[046:00073) | └─ForSyntax
//@[046:00047) |   ├─Token(LeftSquare) |[|
//@[047:00050) |   ├─Token(Identifier) |for|
//@[051:00056) |   ├─LocalVariableSyntax
//@[051:00056) |   | └─IdentifierSyntax
//@[051:00056) |   |   └─Token(Identifier) |thing|
//@[057:00059) |   ├─Token(Identifier) |in|
//@[060:00062) |   ├─ArraySyntax
//@[060:00061) |   | ├─Token(LeftSquare) |[|
//@[061:00062) |   | └─Token(RightSquare) |]|
//@[062:00063) |   ├─Token(Colon) |:|
//@[064:00072) |   ├─SkippedTriviaSyntax
//@[064:00070) |   | ├─Token(Identifier) |concat|
//@[070:00071) |   | ├─Token(LeftParen) |(|
//@[071:00072) |   | └─Token(RightParen) |)|
//@[072:00073) |   └─Token(RightSquare) |]|
//@[073:00074) ├─Token(NewLine) |\n|
module nonObjectModuleBody3 'modulea.bicep' = [for (thing,i) in []: 'hello']
//@[000:00076) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00027) | ├─IdentifierSyntax
//@[007:00027) | | └─Token(Identifier) |nonObjectModuleBody3|
//@[028:00043) | ├─StringSyntax
//@[028:00043) | | └─Token(StringComplete) |'modulea.bicep'|
//@[044:00045) | ├─Token(Assignment) |=|
//@[046:00076) | └─ForSyntax
//@[046:00047) |   ├─Token(LeftSquare) |[|
//@[047:00050) |   ├─Token(Identifier) |for|
//@[051:00060) |   ├─VariableBlockSyntax
//@[051:00052) |   | ├─Token(LeftParen) |(|
//@[052:00057) |   | ├─LocalVariableSyntax
//@[052:00057) |   | | └─IdentifierSyntax
//@[052:00057) |   | |   └─Token(Identifier) |thing|
//@[057:00058) |   | ├─Token(Comma) |,|
//@[058:00059) |   | ├─LocalVariableSyntax
//@[058:00059) |   | | └─IdentifierSyntax
//@[058:00059) |   | |   └─Token(Identifier) |i|
//@[059:00060) |   | └─Token(RightParen) |)|
//@[061:00063) |   ├─Token(Identifier) |in|
//@[064:00066) |   ├─ArraySyntax
//@[064:00065) |   | ├─Token(LeftSquare) |[|
//@[065:00066) |   | └─Token(RightSquare) |]|
//@[066:00067) |   ├─Token(Colon) |:|
//@[068:00075) |   ├─SkippedTriviaSyntax
//@[068:00075) |   | └─Token(StringComplete) |'hello'|
//@[075:00076) |   └─Token(RightSquare) |]|
//@[076:00077) ├─Token(NewLine) |\n|
module nonObjectModuleBody4 'modulea.bicep' = [for (thing,i) in []: concat()]
//@[000:00077) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00027) | ├─IdentifierSyntax
//@[007:00027) | | └─Token(Identifier) |nonObjectModuleBody4|
//@[028:00043) | ├─StringSyntax
//@[028:00043) | | └─Token(StringComplete) |'modulea.bicep'|
//@[044:00045) | ├─Token(Assignment) |=|
//@[046:00077) | └─ForSyntax
//@[046:00047) |   ├─Token(LeftSquare) |[|
//@[047:00050) |   ├─Token(Identifier) |for|
//@[051:00060) |   ├─VariableBlockSyntax
//@[051:00052) |   | ├─Token(LeftParen) |(|
//@[052:00057) |   | ├─LocalVariableSyntax
//@[052:00057) |   | | └─IdentifierSyntax
//@[052:00057) |   | |   └─Token(Identifier) |thing|
//@[057:00058) |   | ├─Token(Comma) |,|
//@[058:00059) |   | ├─LocalVariableSyntax
//@[058:00059) |   | | └─IdentifierSyntax
//@[058:00059) |   | |   └─Token(Identifier) |i|
//@[059:00060) |   | └─Token(RightParen) |)|
//@[061:00063) |   ├─Token(Identifier) |in|
//@[064:00066) |   ├─ArraySyntax
//@[064:00065) |   | ├─Token(LeftSquare) |[|
//@[065:00066) |   | └─Token(RightSquare) |]|
//@[066:00067) |   ├─Token(Colon) |:|
//@[068:00076) |   ├─SkippedTriviaSyntax
//@[068:00074) |   | ├─Token(Identifier) |concat|
//@[074:00075) |   | ├─Token(LeftParen) |(|
//@[075:00076) |   | └─Token(RightParen) |)|
//@[076:00077) |   └─Token(RightSquare) |]|
//@[077:00079) ├─Token(NewLine) |\n\n|

module anyTypeInScope 'empty.bicep' = {
//@[000:00091) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00021) | ├─IdentifierSyntax
//@[007:00021) | | └─Token(Identifier) |anyTypeInScope|
//@[022:00035) | ├─StringSyntax
//@[022:00035) | | └─Token(StringComplete) |'empty.bicep'|
//@[036:00037) | ├─Token(Assignment) |=|
//@[038:00091) | └─ObjectSyntax
//@[038:00039) |   ├─Token(LeftBrace) |{|
//@[039:00040) |   ├─Token(NewLine) |\n|
  dependsOn: [
//@[002:00031) |   ├─ObjectPropertySyntax
//@[002:00011) |   | ├─IdentifierSyntax
//@[002:00011) |   | | └─Token(Identifier) |dependsOn|
//@[011:00012) |   | ├─Token(Colon) |:|
//@[013:00031) |   | └─ArraySyntax
//@[013:00014) |   |   ├─Token(LeftSquare) |[|
//@[014:00015) |   |   ├─Token(NewLine) |\n|
    any('s')
//@[004:00012) |   |   ├─ArrayItemSyntax
//@[004:00012) |   |   | └─FunctionCallSyntax
//@[004:00007) |   |   |   ├─IdentifierSyntax
//@[004:00007) |   |   |   | └─Token(Identifier) |any|
//@[007:00008) |   |   |   ├─Token(LeftParen) |(|
//@[008:00011) |   |   |   ├─FunctionArgumentSyntax
//@[008:00011) |   |   |   | └─StringSyntax
//@[008:00011) |   |   |   |   └─Token(StringComplete) |'s'|
//@[011:00012) |   |   |   └─Token(RightParen) |)|
//@[012:00013) |   |   ├─Token(NewLine) |\n|
  ]
//@[002:00003) |   |   └─Token(RightSquare) |]|
//@[003:00005) |   ├─Token(NewLine) |\n\n|

  scope: any(42)
//@[002:00016) |   ├─ObjectPropertySyntax
//@[002:00007) |   | ├─IdentifierSyntax
//@[002:00007) |   | | └─Token(Identifier) |scope|
//@[007:00008) |   | ├─Token(Colon) |:|
//@[009:00016) |   | └─FunctionCallSyntax
//@[009:00012) |   |   ├─IdentifierSyntax
//@[009:00012) |   |   | └─Token(Identifier) |any|
//@[012:00013) |   |   ├─Token(LeftParen) |(|
//@[013:00015) |   |   ├─FunctionArgumentSyntax
//@[013:00015) |   |   | └─IntegerLiteralSyntax
//@[013:00015) |   |   |   └─Token(Integer) |42|
//@[015:00016) |   |   └─Token(RightParen) |)|
//@[016:00017) |   ├─Token(NewLine) |\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00003) ├─Token(NewLine) |\n\n|

module anyTypeInScopeConditional 'empty.bicep' = if(false) {
//@[000:00112) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00032) | ├─IdentifierSyntax
//@[007:00032) | | └─Token(Identifier) |anyTypeInScopeConditional|
//@[033:00046) | ├─StringSyntax
//@[033:00046) | | └─Token(StringComplete) |'empty.bicep'|
//@[047:00048) | ├─Token(Assignment) |=|
//@[049:00112) | └─IfConditionSyntax
//@[049:00051) |   ├─Token(Identifier) |if|
//@[051:00058) |   ├─ParenthesizedExpressionSyntax
//@[051:00052) |   | ├─Token(LeftParen) |(|
//@[052:00057) |   | ├─BooleanLiteralSyntax
//@[052:00057) |   | | └─Token(FalseKeyword) |false|
//@[057:00058) |   | └─Token(RightParen) |)|
//@[059:00112) |   └─ObjectSyntax
//@[059:00060) |     ├─Token(LeftBrace) |{|
//@[060:00061) |     ├─Token(NewLine) |\n|
  dependsOn: [
//@[002:00031) |     ├─ObjectPropertySyntax
//@[002:00011) |     | ├─IdentifierSyntax
//@[002:00011) |     | | └─Token(Identifier) |dependsOn|
//@[011:00012) |     | ├─Token(Colon) |:|
//@[013:00031) |     | └─ArraySyntax
//@[013:00014) |     |   ├─Token(LeftSquare) |[|
//@[014:00015) |     |   ├─Token(NewLine) |\n|
    any('s')
//@[004:00012) |     |   ├─ArrayItemSyntax
//@[004:00012) |     |   | └─FunctionCallSyntax
//@[004:00007) |     |   |   ├─IdentifierSyntax
//@[004:00007) |     |   |   | └─Token(Identifier) |any|
//@[007:00008) |     |   |   ├─Token(LeftParen) |(|
//@[008:00011) |     |   |   ├─FunctionArgumentSyntax
//@[008:00011) |     |   |   | └─StringSyntax
//@[008:00011) |     |   |   |   └─Token(StringComplete) |'s'|
//@[011:00012) |     |   |   └─Token(RightParen) |)|
//@[012:00013) |     |   ├─Token(NewLine) |\n|
  ]
//@[002:00003) |     |   └─Token(RightSquare) |]|
//@[003:00005) |     ├─Token(NewLine) |\n\n|

  scope: any(42)
//@[002:00016) |     ├─ObjectPropertySyntax
//@[002:00007) |     | ├─IdentifierSyntax
//@[002:00007) |     | | └─Token(Identifier) |scope|
//@[007:00008) |     | ├─Token(Colon) |:|
//@[009:00016) |     | └─FunctionCallSyntax
//@[009:00012) |     |   ├─IdentifierSyntax
//@[009:00012) |     |   | └─Token(Identifier) |any|
//@[012:00013) |     |   ├─Token(LeftParen) |(|
//@[013:00015) |     |   ├─FunctionArgumentSyntax
//@[013:00015) |     |   | └─IntegerLiteralSyntax
//@[013:00015) |     |   |   └─Token(Integer) |42|
//@[015:00016) |     |   └─Token(RightParen) |)|
//@[016:00017) |     ├─Token(NewLine) |\n|
}
//@[000:00001) |     └─Token(RightBrace) |}|
//@[001:00003) ├─Token(NewLine) |\n\n|

module anyTypeInScopeLoop 'empty.bicep' = [for thing in []: {
//@[000:00114) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00025) | ├─IdentifierSyntax
//@[007:00025) | | └─Token(Identifier) |anyTypeInScopeLoop|
//@[026:00039) | ├─StringSyntax
//@[026:00039) | | └─Token(StringComplete) |'empty.bicep'|
//@[040:00041) | ├─Token(Assignment) |=|
//@[042:00114) | └─ForSyntax
//@[042:00043) |   ├─Token(LeftSquare) |[|
//@[043:00046) |   ├─Token(Identifier) |for|
//@[047:00052) |   ├─LocalVariableSyntax
//@[047:00052) |   | └─IdentifierSyntax
//@[047:00052) |   |   └─Token(Identifier) |thing|
//@[053:00055) |   ├─Token(Identifier) |in|
//@[056:00058) |   ├─ArraySyntax
//@[056:00057) |   | ├─Token(LeftSquare) |[|
//@[057:00058) |   | └─Token(RightSquare) |]|
//@[058:00059) |   ├─Token(Colon) |:|
//@[060:00113) |   ├─ObjectSyntax
//@[060:00061) |   | ├─Token(LeftBrace) |{|
//@[061:00062) |   | ├─Token(NewLine) |\n|
  dependsOn: [
//@[002:00031) |   | ├─ObjectPropertySyntax
//@[002:00011) |   | | ├─IdentifierSyntax
//@[002:00011) |   | | | └─Token(Identifier) |dependsOn|
//@[011:00012) |   | | ├─Token(Colon) |:|
//@[013:00031) |   | | └─ArraySyntax
//@[013:00014) |   | |   ├─Token(LeftSquare) |[|
//@[014:00015) |   | |   ├─Token(NewLine) |\n|
    any('s')
//@[004:00012) |   | |   ├─ArrayItemSyntax
//@[004:00012) |   | |   | └─FunctionCallSyntax
//@[004:00007) |   | |   |   ├─IdentifierSyntax
//@[004:00007) |   | |   |   | └─Token(Identifier) |any|
//@[007:00008) |   | |   |   ├─Token(LeftParen) |(|
//@[008:00011) |   | |   |   ├─FunctionArgumentSyntax
//@[008:00011) |   | |   |   | └─StringSyntax
//@[008:00011) |   | |   |   |   └─Token(StringComplete) |'s'|
//@[011:00012) |   | |   |   └─Token(RightParen) |)|
//@[012:00013) |   | |   ├─Token(NewLine) |\n|
  ]
//@[002:00003) |   | |   └─Token(RightSquare) |]|
//@[003:00005) |   | ├─Token(NewLine) |\n\n|

  scope: any(42)
//@[002:00016) |   | ├─ObjectPropertySyntax
//@[002:00007) |   | | ├─IdentifierSyntax
//@[002:00007) |   | | | └─Token(Identifier) |scope|
//@[007:00008) |   | | ├─Token(Colon) |:|
//@[009:00016) |   | | └─FunctionCallSyntax
//@[009:00012) |   | |   ├─IdentifierSyntax
//@[009:00012) |   | |   | └─Token(Identifier) |any|
//@[012:00013) |   | |   ├─Token(LeftParen) |(|
//@[013:00015) |   | |   ├─FunctionArgumentSyntax
//@[013:00015) |   | |   | └─IntegerLiteralSyntax
//@[013:00015) |   | |   |   └─Token(Integer) |42|
//@[015:00016) |   | |   └─Token(RightParen) |)|
//@[016:00017) |   | ├─Token(NewLine) |\n|
}]
//@[000:00001) |   | └─Token(RightBrace) |}|
//@[001:00002) |   └─Token(RightSquare) |]|
//@[002:00004) ├─Token(NewLine) |\n\n|

// Key Vault Secret Reference
//@[029:00031) ├─Token(NewLine) |\n\n|

resource kv 'Microsoft.KeyVault/vaults@2019-09-01' existing = {
//@[000:00088) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00011) | ├─IdentifierSyntax
//@[009:00011) | | └─Token(Identifier) |kv|
//@[012:00050) | ├─StringSyntax
//@[012:00050) | | └─Token(StringComplete) |'Microsoft.KeyVault/vaults@2019-09-01'|
//@[051:00059) | ├─Token(Identifier) |existing|
//@[060:00061) | ├─Token(Assignment) |=|
//@[062:00088) | └─ObjectSyntax
//@[062:00063) |   ├─Token(LeftBrace) |{|
//@[063:00064) |   ├─Token(NewLine) |\n|
  name: 'testkeyvault'
//@[002:00022) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00022) |   | └─StringSyntax
//@[008:00022) |   |   └─Token(StringComplete) |'testkeyvault'|
//@[022:00023) |   ├─Token(NewLine) |\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00003) ├─Token(NewLine) |\n\n|

module secureModule1 'moduleb.bicep' = {
//@[000:00464) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00020) | ├─IdentifierSyntax
//@[007:00020) | | └─Token(Identifier) |secureModule1|
//@[021:00036) | ├─StringSyntax
//@[021:00036) | | └─Token(StringComplete) |'moduleb.bicep'|
//@[037:00038) | ├─Token(Assignment) |=|
//@[039:00464) | └─ObjectSyntax
//@[039:00040) |   ├─Token(LeftBrace) |{|
//@[040:00041) |   ├─Token(NewLine) |\n|
  name: 'secureModule1'
//@[002:00023) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00023) |   | └─StringSyntax
//@[008:00023) |   |   └─Token(StringComplete) |'secureModule1'|
//@[023:00024) |   ├─Token(NewLine) |\n|
  params: {       
//@[002:00397) |   ├─ObjectPropertySyntax
//@[002:00008) |   | ├─IdentifierSyntax
//@[002:00008) |   | | └─Token(Identifier) |params|
//@[008:00009) |   | ├─Token(Colon) |:|
//@[010:00397) |   | └─ObjectSyntax
//@[010:00011) |   |   ├─Token(LeftBrace) |{|
//@[018:00019) |   |   ├─Token(NewLine) |\n|
    stringParamA: kv.getSecret('mySecret')
//@[004:00042) |   |   ├─ObjectPropertySyntax
//@[004:00016) |   |   | ├─IdentifierSyntax
//@[004:00016) |   |   | | └─Token(Identifier) |stringParamA|
//@[016:00017) |   |   | ├─Token(Colon) |:|
//@[018:00042) |   |   | └─InstanceFunctionCallSyntax
//@[018:00020) |   |   |   ├─VariableAccessSyntax
//@[018:00020) |   |   |   | └─IdentifierSyntax
//@[018:00020) |   |   |   |   └─Token(Identifier) |kv|
//@[020:00021) |   |   |   ├─Token(Dot) |.|
//@[021:00030) |   |   |   ├─IdentifierSyntax
//@[021:00030) |   |   |   | └─Token(Identifier) |getSecret|
//@[030:00031) |   |   |   ├─Token(LeftParen) |(|
//@[031:00041) |   |   |   ├─FunctionArgumentSyntax
//@[031:00041) |   |   |   | └─StringSyntax
//@[031:00041) |   |   |   |   └─Token(StringComplete) |'mySecret'|
//@[041:00042) |   |   |   └─Token(RightParen) |)|
//@[042:00043) |   |   ├─Token(NewLine) |\n|
    stringParamB: '${kv.getSecret('mySecret')}'
//@[004:00047) |   |   ├─ObjectPropertySyntax
//@[004:00016) |   |   | ├─IdentifierSyntax
//@[004:00016) |   |   | | └─Token(Identifier) |stringParamB|
//@[016:00017) |   |   | ├─Token(Colon) |:|
//@[018:00047) |   |   | └─StringSyntax
//@[018:00021) |   |   |   ├─Token(StringLeftPiece) |'${|
//@[021:00045) |   |   |   ├─InstanceFunctionCallSyntax
//@[021:00023) |   |   |   | ├─VariableAccessSyntax
//@[021:00023) |   |   |   | | └─IdentifierSyntax
//@[021:00023) |   |   |   | |   └─Token(Identifier) |kv|
//@[023:00024) |   |   |   | ├─Token(Dot) |.|
//@[024:00033) |   |   |   | ├─IdentifierSyntax
//@[024:00033) |   |   |   | | └─Token(Identifier) |getSecret|
//@[033:00034) |   |   |   | ├─Token(LeftParen) |(|
//@[034:00044) |   |   |   | ├─FunctionArgumentSyntax
//@[034:00044) |   |   |   | | └─StringSyntax
//@[034:00044) |   |   |   | |   └─Token(StringComplete) |'mySecret'|
//@[044:00045) |   |   |   | └─Token(RightParen) |)|
//@[045:00047) |   |   |   └─Token(StringRightPiece) |}'|
//@[047:00048) |   |   ├─Token(NewLine) |\n|
    objParam: kv.getSecret('mySecret')
//@[004:00038) |   |   ├─ObjectPropertySyntax
//@[004:00012) |   |   | ├─IdentifierSyntax
//@[004:00012) |   |   | | └─Token(Identifier) |objParam|
//@[012:00013) |   |   | ├─Token(Colon) |:|
//@[014:00038) |   |   | └─InstanceFunctionCallSyntax
//@[014:00016) |   |   |   ├─VariableAccessSyntax
//@[014:00016) |   |   |   | └─IdentifierSyntax
//@[014:00016) |   |   |   |   └─Token(Identifier) |kv|
//@[016:00017) |   |   |   ├─Token(Dot) |.|
//@[017:00026) |   |   |   ├─IdentifierSyntax
//@[017:00026) |   |   |   | └─Token(Identifier) |getSecret|
//@[026:00027) |   |   |   ├─Token(LeftParen) |(|
//@[027:00037) |   |   |   ├─FunctionArgumentSyntax
//@[027:00037) |   |   |   | └─StringSyntax
//@[027:00037) |   |   |   |   └─Token(StringComplete) |'mySecret'|
//@[037:00038) |   |   |   └─Token(RightParen) |)|
//@[038:00039) |   |   ├─Token(NewLine) |\n|
    arrayParam: kv.getSecret('mySecret')
//@[004:00040) |   |   ├─ObjectPropertySyntax
//@[004:00014) |   |   | ├─IdentifierSyntax
//@[004:00014) |   |   | | └─Token(Identifier) |arrayParam|
//@[014:00015) |   |   | ├─Token(Colon) |:|
//@[016:00040) |   |   | └─InstanceFunctionCallSyntax
//@[016:00018) |   |   |   ├─VariableAccessSyntax
//@[016:00018) |   |   |   | └─IdentifierSyntax
//@[016:00018) |   |   |   |   └─Token(Identifier) |kv|
//@[018:00019) |   |   |   ├─Token(Dot) |.|
//@[019:00028) |   |   |   ├─IdentifierSyntax
//@[019:00028) |   |   |   | └─Token(Identifier) |getSecret|
//@[028:00029) |   |   |   ├─Token(LeftParen) |(|
//@[029:00039) |   |   |   ├─FunctionArgumentSyntax
//@[029:00039) |   |   |   | └─StringSyntax
//@[029:00039) |   |   |   |   └─Token(StringComplete) |'mySecret'|
//@[039:00040) |   |   |   └─Token(RightParen) |)|
//@[040:00041) |   |   ├─Token(NewLine) |\n|
    secureStringParam: '${kv.getSecret('mySecret')}'
//@[004:00052) |   |   ├─ObjectPropertySyntax
//@[004:00021) |   |   | ├─IdentifierSyntax
//@[004:00021) |   |   | | └─Token(Identifier) |secureStringParam|
//@[021:00022) |   |   | ├─Token(Colon) |:|
//@[023:00052) |   |   | └─StringSyntax
//@[023:00026) |   |   |   ├─Token(StringLeftPiece) |'${|
//@[026:00050) |   |   |   ├─InstanceFunctionCallSyntax
//@[026:00028) |   |   |   | ├─VariableAccessSyntax
//@[026:00028) |   |   |   | | └─IdentifierSyntax
//@[026:00028) |   |   |   | |   └─Token(Identifier) |kv|
//@[028:00029) |   |   |   | ├─Token(Dot) |.|
//@[029:00038) |   |   |   | ├─IdentifierSyntax
//@[029:00038) |   |   |   | | └─Token(Identifier) |getSecret|
//@[038:00039) |   |   |   | ├─Token(LeftParen) |(|
//@[039:00049) |   |   |   | ├─FunctionArgumentSyntax
//@[039:00049) |   |   |   | | └─StringSyntax
//@[039:00049) |   |   |   | |   └─Token(StringComplete) |'mySecret'|
//@[049:00050) |   |   |   | └─Token(RightParen) |)|
//@[050:00052) |   |   |   └─Token(StringRightPiece) |}'|
//@[052:00053) |   |   ├─Token(NewLine) |\n|
    secureObjectParam: kv.getSecret('mySecret')
//@[004:00047) |   |   ├─ObjectPropertySyntax
//@[004:00021) |   |   | ├─IdentifierSyntax
//@[004:00021) |   |   | | └─Token(Identifier) |secureObjectParam|
//@[021:00022) |   |   | ├─Token(Colon) |:|
//@[023:00047) |   |   | └─InstanceFunctionCallSyntax
//@[023:00025) |   |   |   ├─VariableAccessSyntax
//@[023:00025) |   |   |   | └─IdentifierSyntax
//@[023:00025) |   |   |   |   └─Token(Identifier) |kv|
//@[025:00026) |   |   |   ├─Token(Dot) |.|
//@[026:00035) |   |   |   ├─IdentifierSyntax
//@[026:00035) |   |   |   | └─Token(Identifier) |getSecret|
//@[035:00036) |   |   |   ├─Token(LeftParen) |(|
//@[036:00046) |   |   |   ├─FunctionArgumentSyntax
//@[036:00046) |   |   |   | └─StringSyntax
//@[036:00046) |   |   |   |   └─Token(StringComplete) |'mySecret'|
//@[046:00047) |   |   |   └─Token(RightParen) |)|
//@[047:00048) |   |   ├─Token(NewLine) |\n|
    secureStringParam2: '${kv.getSecret('mySecret')}'
//@[004:00053) |   |   ├─ObjectPropertySyntax
//@[004:00022) |   |   | ├─IdentifierSyntax
//@[004:00022) |   |   | | └─Token(Identifier) |secureStringParam2|
//@[022:00023) |   |   | ├─Token(Colon) |:|
//@[024:00053) |   |   | └─StringSyntax
//@[024:00027) |   |   |   ├─Token(StringLeftPiece) |'${|
//@[027:00051) |   |   |   ├─InstanceFunctionCallSyntax
//@[027:00029) |   |   |   | ├─VariableAccessSyntax
//@[027:00029) |   |   |   | | └─IdentifierSyntax
//@[027:00029) |   |   |   | |   └─Token(Identifier) |kv|
//@[029:00030) |   |   |   | ├─Token(Dot) |.|
//@[030:00039) |   |   |   | ├─IdentifierSyntax
//@[030:00039) |   |   |   | | └─Token(Identifier) |getSecret|
//@[039:00040) |   |   |   | ├─Token(LeftParen) |(|
//@[040:00050) |   |   |   | ├─FunctionArgumentSyntax
//@[040:00050) |   |   |   | | └─StringSyntax
//@[040:00050) |   |   |   | |   └─Token(StringComplete) |'mySecret'|
//@[050:00051) |   |   |   | └─Token(RightParen) |)|
//@[051:00053) |   |   |   └─Token(StringRightPiece) |}'|
//@[053:00054) |   |   ├─Token(NewLine) |\n|
    secureObjectParam2: kv.getSecret('mySecret')
//@[004:00048) |   |   ├─ObjectPropertySyntax
//@[004:00022) |   |   | ├─IdentifierSyntax
//@[004:00022) |   |   | | └─Token(Identifier) |secureObjectParam2|
//@[022:00023) |   |   | ├─Token(Colon) |:|
//@[024:00048) |   |   | └─InstanceFunctionCallSyntax
//@[024:00026) |   |   |   ├─VariableAccessSyntax
//@[024:00026) |   |   |   | └─IdentifierSyntax
//@[024:00026) |   |   |   |   └─Token(Identifier) |kv|
//@[026:00027) |   |   |   ├─Token(Dot) |.|
//@[027:00036) |   |   |   ├─IdentifierSyntax
//@[027:00036) |   |   |   | └─Token(Identifier) |getSecret|
//@[036:00037) |   |   |   ├─Token(LeftParen) |(|
//@[037:00047) |   |   |   ├─FunctionArgumentSyntax
//@[037:00047) |   |   |   | └─StringSyntax
//@[037:00047) |   |   |   |   └─Token(StringComplete) |'mySecret'|
//@[047:00048) |   |   |   └─Token(RightParen) |)|
//@[048:00049) |   |   ├─Token(NewLine) |\n|
  }
//@[002:00003) |   |   └─Token(RightBrace) |}|
//@[003:00004) |   ├─Token(NewLine) |\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00003) ├─Token(NewLine) |\n\n|

module secureModule2 'BAD_MODULE_PATH.bicep' = {
//@[000:00134) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00020) | ├─IdentifierSyntax
//@[007:00020) | | └─Token(Identifier) |secureModule2|
//@[021:00044) | ├─StringSyntax
//@[021:00044) | | └─Token(StringComplete) |'BAD_MODULE_PATH.bicep'|
//@[045:00046) | ├─Token(Assignment) |=|
//@[047:00134) | └─ObjectSyntax
//@[047:00048) |   ├─Token(LeftBrace) |{|
//@[048:00049) |   ├─Token(NewLine) |\n|
  name: 'secureModule2'
//@[002:00023) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00023) |   | └─StringSyntax
//@[008:00023) |   |   └─Token(StringComplete) |'secureModule2'|
//@[023:00024) |   ├─Token(NewLine) |\n|
  params: {       
//@[002:00059) |   ├─ObjectPropertySyntax
//@[002:00008) |   | ├─IdentifierSyntax
//@[002:00008) |   | | └─Token(Identifier) |params|
//@[008:00009) |   | ├─Token(Colon) |:|
//@[010:00059) |   | └─ObjectSyntax
//@[010:00011) |   |   ├─Token(LeftBrace) |{|
//@[018:00019) |   |   ├─Token(NewLine) |\n|
    secret: kv.getSecret('mySecret')
//@[004:00036) |   |   ├─ObjectPropertySyntax
//@[004:00010) |   |   | ├─IdentifierSyntax
//@[004:00010) |   |   | | └─Token(Identifier) |secret|
//@[010:00011) |   |   | ├─Token(Colon) |:|
//@[012:00036) |   |   | └─InstanceFunctionCallSyntax
//@[012:00014) |   |   |   ├─VariableAccessSyntax
//@[012:00014) |   |   |   | └─IdentifierSyntax
//@[012:00014) |   |   |   |   └─Token(Identifier) |kv|
//@[014:00015) |   |   |   ├─Token(Dot) |.|
//@[015:00024) |   |   |   ├─IdentifierSyntax
//@[015:00024) |   |   |   | └─Token(Identifier) |getSecret|
//@[024:00025) |   |   |   ├─Token(LeftParen) |(|
//@[025:00035) |   |   |   ├─FunctionArgumentSyntax
//@[025:00035) |   |   |   | └─StringSyntax
//@[025:00035) |   |   |   |   └─Token(StringComplete) |'mySecret'|
//@[035:00036) |   |   |   └─Token(RightParen) |)|
//@[036:00037) |   |   ├─Token(NewLine) |\n|
  }
//@[002:00003) |   |   └─Token(RightBrace) |}|
//@[003:00004) |   ├─Token(NewLine) |\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00003) ├─Token(NewLine) |\n\n|

module issue3000 'empty.bicep' = {
//@[000:00305) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00016) | ├─IdentifierSyntax
//@[007:00016) | | └─Token(Identifier) |issue3000|
//@[017:00030) | ├─StringSyntax
//@[017:00030) | | └─Token(StringComplete) |'empty.bicep'|
//@[031:00032) | ├─Token(Assignment) |=|
//@[033:00305) | └─ObjectSyntax
//@[033:00034) |   ├─Token(LeftBrace) |{|
//@[034:00035) |   ├─Token(NewLine) |\n|
  name: 'issue3000Module'
//@[002:00025) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00025) |   | └─StringSyntax
//@[008:00025) |   |   └─Token(StringComplete) |'issue3000Module'|
//@[025:00026) |   ├─Token(NewLine) |\n|
  params: {}
//@[002:00012) |   ├─ObjectPropertySyntax
//@[002:00008) |   | ├─IdentifierSyntax
//@[002:00008) |   | | └─Token(Identifier) |params|
//@[008:00009) |   | ├─Token(Colon) |:|
//@[010:00012) |   | └─ObjectSyntax
//@[010:00011) |   |   ├─Token(LeftBrace) |{|
//@[011:00012) |   |   └─Token(RightBrace) |}|
//@[012:00013) |   ├─Token(NewLine) |\n|
  identity: {
//@[002:00044) |   ├─ObjectPropertySyntax
//@[002:00010) |   | ├─IdentifierSyntax
//@[002:00010) |   | | └─Token(Identifier) |identity|
//@[010:00011) |   | ├─Token(Colon) |:|
//@[012:00044) |   | └─ObjectSyntax
//@[012:00013) |   |   ├─Token(LeftBrace) |{|
//@[013:00014) |   |   ├─Token(NewLine) |\n|
    type: 'SystemAssigned'
//@[004:00026) |   |   ├─ObjectPropertySyntax
//@[004:00008) |   |   | ├─IdentifierSyntax
//@[004:00008) |   |   | | └─Token(Identifier) |type|
//@[008:00009) |   |   | ├─Token(Colon) |:|
//@[010:00026) |   |   | └─StringSyntax
//@[010:00026) |   |   |   └─Token(StringComplete) |'SystemAssigned'|
//@[026:00027) |   |   ├─Token(NewLine) |\n|
  }
//@[002:00003) |   |   └─Token(RightBrace) |}|
//@[003:00004) |   ├─Token(NewLine) |\n|
  extendedLocation: {}
//@[002:00022) |   ├─ObjectPropertySyntax
//@[002:00018) |   | ├─IdentifierSyntax
//@[002:00018) |   | | └─Token(Identifier) |extendedLocation|
//@[018:00019) |   | ├─Token(Colon) |:|
//@[020:00022) |   | └─ObjectSyntax
//@[020:00021) |   |   ├─Token(LeftBrace) |{|
//@[021:00022) |   |   └─Token(RightBrace) |}|
//@[022:00023) |   ├─Token(NewLine) |\n|
  sku: {}
//@[002:00009) |   ├─ObjectPropertySyntax
//@[002:00005) |   | ├─IdentifierSyntax
//@[002:00005) |   | | └─Token(Identifier) |sku|
//@[005:00006) |   | ├─Token(Colon) |:|
//@[007:00009) |   | └─ObjectSyntax
//@[007:00008) |   |   ├─Token(LeftBrace) |{|
//@[008:00009) |   |   └─Token(RightBrace) |}|
//@[009:00010) |   ├─Token(NewLine) |\n|
  kind: 'V1'
//@[002:00012) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |kind|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00012) |   | └─StringSyntax
//@[008:00012) |   |   └─Token(StringComplete) |'V1'|
//@[012:00013) |   ├─Token(NewLine) |\n|
  managedBy: 'string'
//@[002:00021) |   ├─ObjectPropertySyntax
//@[002:00011) |   | ├─IdentifierSyntax
//@[002:00011) |   | | └─Token(Identifier) |managedBy|
//@[011:00012) |   | ├─Token(Colon) |:|
//@[013:00021) |   | └─StringSyntax
//@[013:00021) |   |   └─Token(StringComplete) |'string'|
//@[021:00022) |   ├─Token(NewLine) |\n|
  mangedByExtended: [
//@[002:00045) |   ├─ObjectPropertySyntax
//@[002:00018) |   | ├─IdentifierSyntax
//@[002:00018) |   | | └─Token(Identifier) |mangedByExtended|
//@[018:00019) |   | ├─Token(Colon) |:|
//@[020:00045) |   | └─ArraySyntax
//@[020:00021) |   |   ├─Token(LeftSquare) |[|
//@[021:00022) |   |   ├─Token(NewLine) |\n|
   'str1'
//@[003:00009) |   |   ├─ArrayItemSyntax
//@[003:00009) |   |   | └─StringSyntax
//@[003:00009) |   |   |   └─Token(StringComplete) |'str1'|
//@[009:00010) |   |   ├─Token(NewLine) |\n|
   'str2'
//@[003:00009) |   |   ├─ArrayItemSyntax
//@[003:00009) |   |   | └─StringSyntax
//@[003:00009) |   |   |   └─Token(StringComplete) |'str2'|
//@[009:00010) |   |   ├─Token(NewLine) |\n|
  ]
//@[002:00003) |   |   └─Token(RightSquare) |]|
//@[003:00004) |   ├─Token(NewLine) |\n|
  zones: [
//@[002:00034) |   ├─ObjectPropertySyntax
//@[002:00007) |   | ├─IdentifierSyntax
//@[002:00007) |   | | └─Token(Identifier) |zones|
//@[007:00008) |   | ├─Token(Colon) |:|
//@[009:00034) |   | └─ArraySyntax
//@[009:00010) |   |   ├─Token(LeftSquare) |[|
//@[010:00011) |   |   ├─Token(NewLine) |\n|
   'str1'
//@[003:00009) |   |   ├─ArrayItemSyntax
//@[003:00009) |   |   | └─StringSyntax
//@[003:00009) |   |   |   └─Token(StringComplete) |'str1'|
//@[009:00010) |   |   ├─Token(NewLine) |\n|
   'str2'
//@[003:00009) |   |   ├─ArrayItemSyntax
//@[003:00009) |   |   | └─StringSyntax
//@[003:00009) |   |   |   └─Token(StringComplete) |'str2'|
//@[009:00010) |   |   ├─Token(NewLine) |\n|
  ]
//@[002:00003) |   |   └─Token(RightSquare) |]|
//@[003:00004) |   ├─Token(NewLine) |\n|
  plan: {}
//@[002:00010) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |plan|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00010) |   | └─ObjectSyntax
//@[008:00009) |   |   ├─Token(LeftBrace) |{|
//@[009:00010) |   |   └─Token(RightBrace) |}|
//@[010:00011) |   ├─Token(NewLine) |\n|
  eTag: ''
//@[002:00010) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |eTag|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00010) |   | └─StringSyntax
//@[008:00010) |   |   └─Token(StringComplete) |''|
//@[010:00011) |   ├─Token(NewLine) |\n|
  scale: {}  
//@[002:00011) |   ├─ObjectPropertySyntax
//@[002:00007) |   | ├─IdentifierSyntax
//@[002:00007) |   | | └─Token(Identifier) |scale|
//@[007:00008) |   | ├─Token(Colon) |:|
//@[009:00011) |   | └─ObjectSyntax
//@[009:00010) |   |   ├─Token(LeftBrace) |{|
//@[010:00011) |   |   └─Token(RightBrace) |}|
//@[013:00014) |   ├─Token(NewLine) |\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00003) ├─Token(NewLine) |\n\n|

module invalidJsonMod 'modulec.json' = {
//@[000:00042) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00021) | ├─IdentifierSyntax
//@[007:00021) | | └─Token(Identifier) |invalidJsonMod|
//@[022:00036) | ├─StringSyntax
//@[022:00036) | | └─Token(StringComplete) |'modulec.json'|
//@[037:00038) | ├─Token(Assignment) |=|
//@[039:00042) | └─ObjectSyntax
//@[039:00040) |   ├─Token(LeftBrace) |{|
//@[040:00041) |   ├─Token(NewLine) |\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00003) ├─Token(NewLine) |\n\n|

module jsonModMissingParam 'moduled.json' = {
//@[000:00119) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00026) | ├─IdentifierSyntax
//@[007:00026) | | └─Token(Identifier) |jsonModMissingParam|
//@[027:00041) | ├─StringSyntax
//@[027:00041) | | └─Token(StringComplete) |'moduled.json'|
//@[042:00043) | ├─Token(Assignment) |=|
//@[044:00119) | └─ObjectSyntax
//@[044:00045) |   ├─Token(LeftBrace) |{|
//@[045:00046) |   ├─Token(NewLine) |\n|
  name: 'jsonModMissingParam'
//@[002:00029) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00029) |   | └─StringSyntax
//@[008:00029) |   |   └─Token(StringComplete) |'jsonModMissingParam'|
//@[029:00030) |   ├─Token(NewLine) |\n|
  params: {
//@[002:00041) |   ├─ObjectPropertySyntax
//@[002:00008) |   | ├─IdentifierSyntax
//@[002:00008) |   | | └─Token(Identifier) |params|
//@[008:00009) |   | ├─Token(Colon) |:|
//@[010:00041) |   | └─ObjectSyntax
//@[010:00011) |   |   ├─Token(LeftBrace) |{|
//@[011:00012) |   |   ├─Token(NewLine) |\n|
    foo: 123
//@[004:00012) |   |   ├─ObjectPropertySyntax
//@[004:00007) |   |   | ├─IdentifierSyntax
//@[004:00007) |   |   | | └─Token(Identifier) |foo|
//@[007:00008) |   |   | ├─Token(Colon) |:|
//@[009:00012) |   |   | └─IntegerLiteralSyntax
//@[009:00012) |   |   |   └─Token(Integer) |123|
//@[012:00013) |   |   ├─Token(NewLine) |\n|
    baz: 'C'
//@[004:00012) |   |   ├─ObjectPropertySyntax
//@[004:00007) |   |   | ├─IdentifierSyntax
//@[004:00007) |   |   | | └─Token(Identifier) |baz|
//@[007:00008) |   |   | ├─Token(Colon) |:|
//@[009:00012) |   |   | └─StringSyntax
//@[009:00012) |   |   |   └─Token(StringComplete) |'C'|
//@[012:00013) |   |   ├─Token(NewLine) |\n|
  }
//@[002:00003) |   |   └─Token(RightBrace) |}|
//@[003:00004) |   ├─Token(NewLine) |\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00003) ├─Token(NewLine) |\n\n|

module assignToOutput 'empty.bicep' = {
//@[000:00080) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00021) | ├─IdentifierSyntax
//@[007:00021) | | └─Token(Identifier) |assignToOutput|
//@[022:00035) | ├─StringSyntax
//@[022:00035) | | └─Token(StringComplete) |'empty.bicep'|
//@[036:00037) | ├─Token(Assignment) |=|
//@[038:00080) | └─ObjectSyntax
//@[038:00039) |   ├─Token(LeftBrace) |{|
//@[039:00040) |   ├─Token(NewLine) |\n|
  name: 'assignToOutput'
//@[002:00024) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00024) |   | └─StringSyntax
//@[008:00024) |   |   └─Token(StringComplete) |'assignToOutput'|
//@[024:00025) |   ├─Token(NewLine) |\n|
  outputs: {}
//@[002:00013) |   ├─ObjectPropertySyntax
//@[002:00009) |   | ├─IdentifierSyntax
//@[002:00009) |   | | └─Token(Identifier) |outputs|
//@[009:00010) |   | ├─Token(Colon) |:|
//@[011:00013) |   | └─ObjectSyntax
//@[011:00012) |   |   ├─Token(LeftBrace) |{|
//@[012:00013) |   |   └─Token(RightBrace) |}|
//@[013:00014) |   ├─Token(NewLine) |\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00001) └─Token(EndOfFile) ||
