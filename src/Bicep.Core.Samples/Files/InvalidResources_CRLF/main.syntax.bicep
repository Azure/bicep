
//@[000:51671) ProgramSyntax
//@[000:00002) ├─Token(NewLine) |\r\n|
// wrong declaration
//@[020:00022) ├─Token(NewLine) |\r\n|
bad
//@[000:00003) ├─SkippedTriviaSyntax
//@[000:00003) | └─Token(Identifier) |bad|
//@[003:00007) ├─Token(NewLine) |\r\n\r\n|

// incomplete #completionTest(9) -> empty
//@[041:00043) ├─Token(NewLine) |\r\n|
resource 
//@[000:00009) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00009) | ├─IdentifierSyntax
//@[009:00009) | | └─SkippedTriviaSyntax
//@[009:00009) | ├─SkippedTriviaSyntax
//@[009:00009) | ├─SkippedTriviaSyntax
//@[009:00009) | └─SkippedTriviaSyntax
//@[009:00011) ├─Token(NewLine) |\r\n|
resource foo
//@[000:00012) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00012) | ├─IdentifierSyntax
//@[009:00012) | | └─Token(Identifier) |foo|
//@[012:00012) | ├─SkippedTriviaSyntax
//@[012:00012) | ├─SkippedTriviaSyntax
//@[012:00012) | └─SkippedTriviaSyntax
//@[012:00014) ├─Token(NewLine) |\r\n|
resource fo/o
//@[000:00013) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00011) | ├─IdentifierSyntax
//@[009:00011) | | └─Token(Identifier) |fo|
//@[011:00013) | ├─SkippedTriviaSyntax
//@[011:00012) | | ├─Token(Slash) |/|
//@[012:00013) | | └─Token(Identifier) |o|
//@[013:00013) | ├─SkippedTriviaSyntax
//@[013:00013) | └─SkippedTriviaSyntax
//@[013:00015) ├─Token(NewLine) |\r\n|
resource foo 'ddd'
//@[000:00018) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00012) | ├─IdentifierSyntax
//@[009:00012) | | └─Token(Identifier) |foo|
//@[013:00018) | ├─StringSyntax
//@[013:00018) | | └─Token(StringComplete) |'ddd'|
//@[018:00018) | ├─SkippedTriviaSyntax
//@[018:00018) | └─SkippedTriviaSyntax
//@[018:00022) ├─Token(NewLine) |\r\n\r\n|

// #completionTest(23) -> resourceTypes
//@[039:00041) ├─Token(NewLine) |\r\n|
resource trailingSpace  
//@[000:00024) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00022) | ├─IdentifierSyntax
//@[009:00022) | | └─Token(Identifier) |trailingSpace|
//@[024:00024) | ├─SkippedTriviaSyntax
//@[024:00024) | ├─SkippedTriviaSyntax
//@[024:00024) | └─SkippedTriviaSyntax
//@[024:00028) ├─Token(NewLine) |\r\n\r\n|

// #completionTest(19,20) -> resourceObject
//@[043:00045) ├─Token(NewLine) |\r\n|
resource foo 'ddd'= 
//@[000:00020) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00012) | ├─IdentifierSyntax
//@[009:00012) | | └─Token(Identifier) |foo|
//@[013:00018) | ├─StringSyntax
//@[013:00018) | | └─Token(StringComplete) |'ddd'|
//@[018:00019) | ├─Token(Assignment) |=|
//@[020:00020) | └─SkippedTriviaSyntax
//@[020:00024) ├─Token(NewLine) |\r\n\r\n|

// wrong resource type
//@[022:00024) ├─Token(NewLine) |\r\n|
resource foo 'ddd'={
//@[000:00023) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00012) | ├─IdentifierSyntax
//@[009:00012) | | └─Token(Identifier) |foo|
//@[013:00018) | ├─StringSyntax
//@[013:00018) | | └─Token(StringComplete) |'ddd'|
//@[018:00019) | ├─Token(Assignment) |=|
//@[019:00023) | └─ObjectSyntax
//@[019:00020) |   ├─Token(LeftBrace) |{|
//@[020:00022) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

resource foo 'ddd'=if (1 + 1 == 2) {
//@[000:00039) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00012) | ├─IdentifierSyntax
//@[009:00012) | | └─Token(Identifier) |foo|
//@[013:00018) | ├─StringSyntax
//@[013:00018) | | └─Token(StringComplete) |'ddd'|
//@[018:00019) | ├─Token(Assignment) |=|
//@[019:00039) | └─IfConditionSyntax
//@[019:00021) |   ├─Token(Identifier) |if|
//@[022:00034) |   ├─ParenthesizedExpressionSyntax
//@[022:00023) |   | ├─Token(LeftParen) |(|
//@[023:00033) |   | ├─BinaryOperationSyntax
//@[023:00028) |   | | ├─BinaryOperationSyntax
//@[023:00024) |   | | | ├─IntegerLiteralSyntax
//@[023:00024) |   | | | | └─Token(Integer) |1|
//@[025:00026) |   | | | ├─Token(Plus) |+|
//@[027:00028) |   | | | └─IntegerLiteralSyntax
//@[027:00028) |   | | |   └─Token(Integer) |1|
//@[029:00031) |   | | ├─Token(Equals) |==|
//@[032:00033) |   | | └─IntegerLiteralSyntax
//@[032:00033) |   | |   └─Token(Integer) |2|
//@[033:00034) |   | └─Token(RightParen) |)|
//@[035:00039) |   └─ObjectSyntax
//@[035:00036) |     ├─Token(LeftBrace) |{|
//@[036:00038) |     ├─Token(NewLine) |\r\n|
}
//@[000:00001) |     └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

// using string interpolation for the resource type
//@[051:00053) ├─Token(NewLine) |\r\n|
resource foo 'Microsoft.${provider}/foos@2020-02-02-alpha'= {
//@[000:00064) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00012) | ├─IdentifierSyntax
//@[009:00012) | | └─Token(Identifier) |foo|
//@[013:00058) | ├─StringSyntax
//@[013:00026) | | ├─Token(StringLeftPiece) |'Microsoft.${|
//@[026:00034) | | ├─VariableAccessSyntax
//@[026:00034) | | | └─IdentifierSyntax
//@[026:00034) | | |   └─Token(Identifier) |provider|
//@[034:00058) | | └─Token(StringRightPiece) |}/foos@2020-02-02-alpha'|
//@[058:00059) | ├─Token(Assignment) |=|
//@[060:00064) | └─ObjectSyntax
//@[060:00061) |   ├─Token(LeftBrace) |{|
//@[061:00063) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

resource foo 'Microsoft.${provider}/foos@2020-02-02-alpha'= if (true) {
//@[000:00074) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00012) | ├─IdentifierSyntax
//@[009:00012) | | └─Token(Identifier) |foo|
//@[013:00058) | ├─StringSyntax
//@[013:00026) | | ├─Token(StringLeftPiece) |'Microsoft.${|
//@[026:00034) | | ├─VariableAccessSyntax
//@[026:00034) | | | └─IdentifierSyntax
//@[026:00034) | | |   └─Token(Identifier) |provider|
//@[034:00058) | | └─Token(StringRightPiece) |}/foos@2020-02-02-alpha'|
//@[058:00059) | ├─Token(Assignment) |=|
//@[060:00074) | └─IfConditionSyntax
//@[060:00062) |   ├─Token(Identifier) |if|
//@[063:00069) |   ├─ParenthesizedExpressionSyntax
//@[063:00064) |   | ├─Token(LeftParen) |(|
//@[064:00068) |   | ├─BooleanLiteralSyntax
//@[064:00068) |   | | └─Token(TrueKeyword) |true|
//@[068:00069) |   | └─Token(RightParen) |)|
//@[070:00074) |   └─ObjectSyntax
//@[070:00071) |     ├─Token(LeftBrace) |{|
//@[071:00073) |     ├─Token(NewLine) |\r\n|
}
//@[000:00001) |     └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

// missing required property
//@[028:00030) ├─Token(NewLine) |\r\n|
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'={
//@[000:00055) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00012) | ├─IdentifierSyntax
//@[009:00012) | | └─Token(Identifier) |foo|
//@[013:00050) | ├─StringSyntax
//@[013:00050) | | └─Token(StringComplete) |'Microsoft.Foo/foos@2020-02-02-alpha'|
//@[050:00051) | ├─Token(Assignment) |=|
//@[051:00055) | └─ObjectSyntax
//@[051:00052) |   ├─Token(LeftBrace) |{|
//@[052:00054) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= if (name == 'value') {
//@[000:00077) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00012) | ├─IdentifierSyntax
//@[009:00012) | | └─Token(Identifier) |foo|
//@[013:00050) | ├─StringSyntax
//@[013:00050) | | └─Token(StringComplete) |'Microsoft.Foo/foos@2020-02-02-alpha'|
//@[050:00051) | ├─Token(Assignment) |=|
//@[052:00077) | └─IfConditionSyntax
//@[052:00054) |   ├─Token(Identifier) |if|
//@[055:00072) |   ├─ParenthesizedExpressionSyntax
//@[055:00056) |   | ├─Token(LeftParen) |(|
//@[056:00071) |   | ├─BinaryOperationSyntax
//@[056:00060) |   | | ├─VariableAccessSyntax
//@[056:00060) |   | | | └─IdentifierSyntax
//@[056:00060) |   | | |   └─Token(Identifier) |name|
//@[061:00063) |   | | ├─Token(Equals) |==|
//@[064:00071) |   | | └─StringSyntax
//@[064:00071) |   | |   └─Token(StringComplete) |'value'|
//@[071:00072) |   | └─Token(RightParen) |)|
//@[073:00077) |   └─ObjectSyntax
//@[073:00074) |     ├─Token(LeftBrace) |{|
//@[074:00076) |     ├─Token(NewLine) |\r\n|
}
//@[000:00001) |     └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= if ({ 'a': b }.a == 'foo') {
//@[000:00083) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00012) | ├─IdentifierSyntax
//@[009:00012) | | └─Token(Identifier) |foo|
//@[013:00050) | ├─StringSyntax
//@[013:00050) | | └─Token(StringComplete) |'Microsoft.Foo/foos@2020-02-02-alpha'|
//@[050:00051) | ├─Token(Assignment) |=|
//@[052:00083) | └─IfConditionSyntax
//@[052:00054) |   ├─Token(Identifier) |if|
//@[055:00078) |   ├─ParenthesizedExpressionSyntax
//@[055:00056) |   | ├─Token(LeftParen) |(|
//@[056:00077) |   | ├─BinaryOperationSyntax
//@[056:00068) |   | | ├─PropertyAccessSyntax
//@[056:00066) |   | | | ├─ObjectSyntax
//@[056:00057) |   | | | | ├─Token(LeftBrace) |{|
//@[058:00064) |   | | | | ├─ObjectPropertySyntax
//@[058:00061) |   | | | | | ├─StringSyntax
//@[058:00061) |   | | | | | | └─Token(StringComplete) |'a'|
//@[061:00062) |   | | | | | ├─Token(Colon) |:|
//@[063:00064) |   | | | | | └─VariableAccessSyntax
//@[063:00064) |   | | | | |   └─IdentifierSyntax
//@[063:00064) |   | | | | |     └─Token(Identifier) |b|
//@[065:00066) |   | | | | └─Token(RightBrace) |}|
//@[066:00067) |   | | | ├─Token(Dot) |.|
//@[067:00068) |   | | | └─IdentifierSyntax
//@[067:00068) |   | | |   └─Token(Identifier) |a|
//@[069:00071) |   | | ├─Token(Equals) |==|
//@[072:00077) |   | | └─StringSyntax
//@[072:00077) |   | |   └─Token(StringComplete) |'foo'|
//@[077:00078) |   | └─Token(RightParen) |)|
//@[079:00083) |   └─ObjectSyntax
//@[079:00080) |     ├─Token(LeftBrace) |{|
//@[080:00082) |     ├─Token(NewLine) |\r\n|
}
//@[000:00001) |     └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

// simulate typing if condition
//@[031:00033) ├─Token(NewLine) |\r\n|
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= if
//@[000:00054) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00012) | ├─IdentifierSyntax
//@[009:00012) | | └─Token(Identifier) |foo|
//@[013:00050) | ├─StringSyntax
//@[013:00050) | | └─Token(StringComplete) |'Microsoft.Foo/foos@2020-02-02-alpha'|
//@[050:00051) | ├─Token(Assignment) |=|
//@[052:00054) | └─IfConditionSyntax
//@[052:00054) |   ├─Token(Identifier) |if|
//@[054:00054) |   ├─SkippedTriviaSyntax
//@[054:00054) |   └─SkippedTriviaSyntax
//@[054:00058) ├─Token(NewLine) |\r\n\r\n|

resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= if (
//@[000:00056) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00012) | ├─IdentifierSyntax
//@[009:00012) | | └─Token(Identifier) |foo|
//@[013:00050) | ├─StringSyntax
//@[013:00050) | | └─Token(StringComplete) |'Microsoft.Foo/foos@2020-02-02-alpha'|
//@[050:00051) | ├─Token(Assignment) |=|
//@[052:00056) | └─IfConditionSyntax
//@[052:00054) |   ├─Token(Identifier) |if|
//@[055:00056) |   ├─ParenthesizedExpressionSyntax
//@[055:00056) |   | ├─Token(LeftParen) |(|
//@[056:00056) |   | ├─SkippedTriviaSyntax
//@[056:00056) |   | └─SkippedTriviaSyntax
//@[056:00056) |   └─SkippedTriviaSyntax
//@[056:00060) ├─Token(NewLine) |\r\n\r\n|

resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= if (true
//@[000:00060) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00012) | ├─IdentifierSyntax
//@[009:00012) | | └─Token(Identifier) |foo|
//@[013:00050) | ├─StringSyntax
//@[013:00050) | | └─Token(StringComplete) |'Microsoft.Foo/foos@2020-02-02-alpha'|
//@[050:00051) | ├─Token(Assignment) |=|
//@[052:00060) | └─IfConditionSyntax
//@[052:00054) |   ├─Token(Identifier) |if|
//@[055:00060) |   ├─ParenthesizedExpressionSyntax
//@[055:00056) |   | ├─Token(LeftParen) |(|
//@[056:00060) |   | ├─BooleanLiteralSyntax
//@[056:00060) |   | | └─Token(TrueKeyword) |true|
//@[060:00060) |   | └─SkippedTriviaSyntax
//@[060:00060) |   └─SkippedTriviaSyntax
//@[060:00064) ├─Token(NewLine) |\r\n\r\n|

resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= if (true)
//@[000:00061) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00012) | ├─IdentifierSyntax
//@[009:00012) | | └─Token(Identifier) |foo|
//@[013:00050) | ├─StringSyntax
//@[013:00050) | | └─Token(StringComplete) |'Microsoft.Foo/foos@2020-02-02-alpha'|
//@[050:00051) | ├─Token(Assignment) |=|
//@[052:00061) | └─IfConditionSyntax
//@[052:00054) |   ├─Token(Identifier) |if|
//@[055:00061) |   ├─ParenthesizedExpressionSyntax
//@[055:00056) |   | ├─Token(LeftParen) |(|
//@[056:00060) |   | ├─BooleanLiteralSyntax
//@[056:00060) |   | | └─Token(TrueKeyword) |true|
//@[060:00061) |   | └─Token(RightParen) |)|
//@[061:00061) |   └─SkippedTriviaSyntax
//@[061:00065) ├─Token(NewLine) |\r\n\r\n|

// missing condition
//@[020:00022) ├─Token(NewLine) |\r\n|
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= if {
//@[000:00074) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00012) | ├─IdentifierSyntax
//@[009:00012) | | └─Token(Identifier) |foo|
//@[013:00050) | ├─StringSyntax
//@[013:00050) | | └─Token(StringComplete) |'Microsoft.Foo/foos@2020-02-02-alpha'|
//@[050:00051) | ├─Token(Assignment) |=|
//@[052:00074) | └─IfConditionSyntax
//@[052:00054) |   ├─Token(Identifier) |if|
//@[055:00055) |   ├─SkippedTriviaSyntax
//@[055:00074) |   └─ObjectSyntax
//@[055:00056) |     ├─Token(LeftBrace) |{|
//@[056:00058) |     ├─Token(NewLine) |\r\n|
  name: 'foo'
//@[002:00013) |     ├─ObjectPropertySyntax
//@[002:00006) |     | ├─IdentifierSyntax
//@[002:00006) |     | | └─Token(Identifier) |name|
//@[006:00007) |     | ├─Token(Colon) |:|
//@[008:00013) |     | └─StringSyntax
//@[008:00013) |     |   └─Token(StringComplete) |'foo'|
//@[013:00015) |     ├─Token(NewLine) |\r\n|
}
//@[000:00001) |     └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

// empty condition
//@[018:00020) ├─Token(NewLine) |\r\n|
// #completionTest(56) -> symbols
//@[033:00035) ├─Token(NewLine) |\r\n|
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= if () {
//@[000:00077) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00012) | ├─IdentifierSyntax
//@[009:00012) | | └─Token(Identifier) |foo|
//@[013:00050) | ├─StringSyntax
//@[013:00050) | | └─Token(StringComplete) |'Microsoft.Foo/foos@2020-02-02-alpha'|
//@[050:00051) | ├─Token(Assignment) |=|
//@[052:00077) | └─IfConditionSyntax
//@[052:00054) |   ├─Token(Identifier) |if|
//@[055:00057) |   ├─ParenthesizedExpressionSyntax
//@[055:00056) |   | ├─Token(LeftParen) |(|
//@[056:00056) |   | ├─SkippedTriviaSyntax
//@[056:00057) |   | └─Token(RightParen) |)|
//@[058:00077) |   └─ObjectSyntax
//@[058:00059) |     ├─Token(LeftBrace) |{|
//@[059:00061) |     ├─Token(NewLine) |\r\n|
  name: 'foo'
//@[002:00013) |     ├─ObjectPropertySyntax
//@[002:00006) |     | ├─IdentifierSyntax
//@[002:00006) |     | | └─Token(Identifier) |name|
//@[006:00007) |     | ├─Token(Colon) |:|
//@[008:00013) |     | └─StringSyntax
//@[008:00013) |     |   └─Token(StringComplete) |'foo'|
//@[013:00015) |     ├─Token(NewLine) |\r\n|
}
//@[000:00001) |     └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

// #completionTest(57, 59) -> symbols
//@[037:00039) ├─Token(NewLine) |\r\n|
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= if (     ) {
//@[000:00082) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00012) | ├─IdentifierSyntax
//@[009:00012) | | └─Token(Identifier) |foo|
//@[013:00050) | ├─StringSyntax
//@[013:00050) | | └─Token(StringComplete) |'Microsoft.Foo/foos@2020-02-02-alpha'|
//@[050:00051) | ├─Token(Assignment) |=|
//@[052:00082) | └─IfConditionSyntax
//@[052:00054) |   ├─Token(Identifier) |if|
//@[055:00062) |   ├─ParenthesizedExpressionSyntax
//@[055:00056) |   | ├─Token(LeftParen) |(|
//@[056:00056) |   | ├─SkippedTriviaSyntax
//@[061:00062) |   | └─Token(RightParen) |)|
//@[063:00082) |   └─ObjectSyntax
//@[063:00064) |     ├─Token(LeftBrace) |{|
//@[064:00066) |     ├─Token(NewLine) |\r\n|
  name: 'foo'
//@[002:00013) |     ├─ObjectPropertySyntax
//@[002:00006) |     | ├─IdentifierSyntax
//@[002:00006) |     | | └─Token(Identifier) |name|
//@[006:00007) |     | ├─Token(Colon) |:|
//@[008:00013) |     | └─StringSyntax
//@[008:00013) |     |   └─Token(StringComplete) |'foo'|
//@[013:00015) |     ├─Token(NewLine) |\r\n|
}
//@[000:00001) |     └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

// invalid condition type
//@[025:00027) ├─Token(NewLine) |\r\n|
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= if (123) {
//@[000:00080) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00012) | ├─IdentifierSyntax
//@[009:00012) | | └─Token(Identifier) |foo|
//@[013:00050) | ├─StringSyntax
//@[013:00050) | | └─Token(StringComplete) |'Microsoft.Foo/foos@2020-02-02-alpha'|
//@[050:00051) | ├─Token(Assignment) |=|
//@[052:00080) | └─IfConditionSyntax
//@[052:00054) |   ├─Token(Identifier) |if|
//@[055:00060) |   ├─ParenthesizedExpressionSyntax
//@[055:00056) |   | ├─Token(LeftParen) |(|
//@[056:00059) |   | ├─IntegerLiteralSyntax
//@[056:00059) |   | | └─Token(Integer) |123|
//@[059:00060) |   | └─Token(RightParen) |)|
//@[061:00080) |   └─ObjectSyntax
//@[061:00062) |     ├─Token(LeftBrace) |{|
//@[062:00064) |     ├─Token(NewLine) |\r\n|
  name: 'foo'
//@[002:00013) |     ├─ObjectPropertySyntax
//@[002:00006) |     | ├─IdentifierSyntax
//@[002:00006) |     | | └─Token(Identifier) |name|
//@[006:00007) |     | ├─Token(Colon) |:|
//@[008:00013) |     | └─StringSyntax
//@[008:00013) |     |   └─Token(StringComplete) |'foo'|
//@[013:00015) |     ├─Token(NewLine) |\r\n|
}
//@[000:00001) |     └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

// runtime functions are no allowed in resource conditions
//@[058:00060) ├─Token(NewLine) |\r\n|
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha' = if (reference('Micorosft.Management/managementGroups/MG', '2020-05-01').name == 'something') {
//@[000:00165) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00012) | ├─IdentifierSyntax
//@[009:00012) | | └─Token(Identifier) |foo|
//@[013:00050) | ├─StringSyntax
//@[013:00050) | | └─Token(StringComplete) |'Microsoft.Foo/foos@2020-02-02-alpha'|
//@[051:00052) | ├─Token(Assignment) |=|
//@[053:00165) | └─IfConditionSyntax
//@[053:00055) |   ├─Token(Identifier) |if|
//@[056:00145) |   ├─ParenthesizedExpressionSyntax
//@[056:00057) |   | ├─Token(LeftParen) |(|
//@[057:00144) |   | ├─BinaryOperationSyntax
//@[057:00129) |   | | ├─PropertyAccessSyntax
//@[057:00124) |   | | | ├─FunctionCallSyntax
//@[057:00066) |   | | | | ├─IdentifierSyntax
//@[057:00066) |   | | | | | └─Token(Identifier) |reference|
//@[066:00067) |   | | | | ├─Token(LeftParen) |(|
//@[067:00109) |   | | | | ├─FunctionArgumentSyntax
//@[067:00109) |   | | | | | └─StringSyntax
//@[067:00109) |   | | | | |   └─Token(StringComplete) |'Micorosft.Management/managementGroups/MG'|
//@[109:00110) |   | | | | ├─Token(Comma) |,|
//@[111:00123) |   | | | | ├─FunctionArgumentSyntax
//@[111:00123) |   | | | | | └─StringSyntax
//@[111:00123) |   | | | | |   └─Token(StringComplete) |'2020-05-01'|
//@[123:00124) |   | | | | └─Token(RightParen) |)|
//@[124:00125) |   | | | ├─Token(Dot) |.|
//@[125:00129) |   | | | └─IdentifierSyntax
//@[125:00129) |   | | |   └─Token(Identifier) |name|
//@[130:00132) |   | | ├─Token(Equals) |==|
//@[133:00144) |   | | └─StringSyntax
//@[133:00144) |   | |   └─Token(StringComplete) |'something'|
//@[144:00145) |   | └─Token(RightParen) |)|
//@[146:00165) |   └─ObjectSyntax
//@[146:00147) |     ├─Token(LeftBrace) |{|
//@[147:00149) |     ├─Token(NewLine) |\r\n|
  name: 'foo'
//@[002:00013) |     ├─ObjectPropertySyntax
//@[002:00006) |     | ├─IdentifierSyntax
//@[002:00006) |     | | └─Token(Identifier) |name|
//@[006:00007) |     | ├─Token(Colon) |:|
//@[008:00013) |     | └─StringSyntax
//@[008:00013) |     |   └─Token(StringComplete) |'foo'|
//@[013:00015) |     ├─Token(NewLine) |\r\n|
}
//@[000:00001) |     └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

resource foo 'Microsoft.Foo/foos@2020-02-02-alpha' = if (listKeys('foo', '2020-05-01').bar == true) {
//@[000:00119) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00012) | ├─IdentifierSyntax
//@[009:00012) | | └─Token(Identifier) |foo|
//@[013:00050) | ├─StringSyntax
//@[013:00050) | | └─Token(StringComplete) |'Microsoft.Foo/foos@2020-02-02-alpha'|
//@[051:00052) | ├─Token(Assignment) |=|
//@[053:00119) | └─IfConditionSyntax
//@[053:00055) |   ├─Token(Identifier) |if|
//@[056:00099) |   ├─ParenthesizedExpressionSyntax
//@[056:00057) |   | ├─Token(LeftParen) |(|
//@[057:00098) |   | ├─BinaryOperationSyntax
//@[057:00090) |   | | ├─PropertyAccessSyntax
//@[057:00086) |   | | | ├─FunctionCallSyntax
//@[057:00065) |   | | | | ├─IdentifierSyntax
//@[057:00065) |   | | | | | └─Token(Identifier) |listKeys|
//@[065:00066) |   | | | | ├─Token(LeftParen) |(|
//@[066:00071) |   | | | | ├─FunctionArgumentSyntax
//@[066:00071) |   | | | | | └─StringSyntax
//@[066:00071) |   | | | | |   └─Token(StringComplete) |'foo'|
//@[071:00072) |   | | | | ├─Token(Comma) |,|
//@[073:00085) |   | | | | ├─FunctionArgumentSyntax
//@[073:00085) |   | | | | | └─StringSyntax
//@[073:00085) |   | | | | |   └─Token(StringComplete) |'2020-05-01'|
//@[085:00086) |   | | | | └─Token(RightParen) |)|
//@[086:00087) |   | | | ├─Token(Dot) |.|
//@[087:00090) |   | | | └─IdentifierSyntax
//@[087:00090) |   | | |   └─Token(Identifier) |bar|
//@[091:00093) |   | | ├─Token(Equals) |==|
//@[094:00098) |   | | └─BooleanLiteralSyntax
//@[094:00098) |   | |   └─Token(TrueKeyword) |true|
//@[098:00099) |   | └─Token(RightParen) |)|
//@[100:00119) |   └─ObjectSyntax
//@[100:00101) |     ├─Token(LeftBrace) |{|
//@[101:00103) |     ├─Token(NewLine) |\r\n|
  name: 'foo'
//@[002:00013) |     ├─ObjectPropertySyntax
//@[002:00006) |     | ├─IdentifierSyntax
//@[002:00006) |     | | └─Token(Identifier) |name|
//@[006:00007) |     | ├─Token(Colon) |:|
//@[008:00013) |     | └─StringSyntax
//@[008:00013) |     |   └─Token(StringComplete) |'foo'|
//@[013:00015) |     ├─Token(NewLine) |\r\n|
}
//@[000:00001) |     └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

// duplicate property at the top level
//@[038:00040) ├─Token(NewLine) |\r\n|
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= {
//@[000:00085) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00012) | ├─IdentifierSyntax
//@[009:00012) | | └─Token(Identifier) |foo|
//@[013:00050) | ├─StringSyntax
//@[013:00050) | | └─Token(StringComplete) |'Microsoft.Foo/foos@2020-02-02-alpha'|
//@[050:00051) | ├─Token(Assignment) |=|
//@[052:00085) | └─ObjectSyntax
//@[052:00053) |   ├─Token(LeftBrace) |{|
//@[053:00055) |   ├─Token(NewLine) |\r\n|
  name: 'foo'
//@[002:00013) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00013) |   | └─StringSyntax
//@[008:00013) |   |   └─Token(StringComplete) |'foo'|
//@[013:00015) |   ├─Token(NewLine) |\r\n|
  name: true
//@[002:00012) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00012) |   | └─BooleanLiteralSyntax
//@[008:00012) |   |   └─Token(TrueKeyword) |true|
//@[012:00014) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

// duplicate property at the top level with string literal syntax
//@[065:00067) ├─Token(NewLine) |\r\n|
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= {
//@[000:00087) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00012) | ├─IdentifierSyntax
//@[009:00012) | | └─Token(Identifier) |foo|
//@[013:00050) | ├─StringSyntax
//@[013:00050) | | └─Token(StringComplete) |'Microsoft.Foo/foos@2020-02-02-alpha'|
//@[050:00051) | ├─Token(Assignment) |=|
//@[052:00087) | └─ObjectSyntax
//@[052:00053) |   ├─Token(LeftBrace) |{|
//@[053:00055) |   ├─Token(NewLine) |\r\n|
  name: 'foo'
//@[002:00013) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00013) |   | └─StringSyntax
//@[008:00013) |   |   └─Token(StringComplete) |'foo'|
//@[013:00015) |   ├─Token(NewLine) |\r\n|
  'name': true
//@[002:00014) |   ├─ObjectPropertySyntax
//@[002:00008) |   | ├─StringSyntax
//@[002:00008) |   | | └─Token(StringComplete) |'name'|
//@[008:00009) |   | ├─Token(Colon) |:|
//@[010:00014) |   | └─BooleanLiteralSyntax
//@[010:00014) |   |   └─Token(TrueKeyword) |true|
//@[014:00016) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

// duplicate property inside
//@[028:00030) ├─Token(NewLine) |\r\n|
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= {
//@[000:00121) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00012) | ├─IdentifierSyntax
//@[009:00012) | | └─Token(Identifier) |foo|
//@[013:00050) | ├─StringSyntax
//@[013:00050) | | └─Token(StringComplete) |'Microsoft.Foo/foos@2020-02-02-alpha'|
//@[050:00051) | ├─Token(Assignment) |=|
//@[052:00121) | └─ObjectSyntax
//@[052:00053) |   ├─Token(LeftBrace) |{|
//@[053:00055) |   ├─Token(NewLine) |\r\n|
  name: 'foo'
//@[002:00013) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00013) |   | └─StringSyntax
//@[008:00013) |   |   └─Token(StringComplete) |'foo'|
//@[013:00015) |   ├─Token(NewLine) |\r\n|
  properties: {
//@[002:00048) |   ├─ObjectPropertySyntax
//@[002:00012) |   | ├─IdentifierSyntax
//@[002:00012) |   | | └─Token(Identifier) |properties|
//@[012:00013) |   | ├─Token(Colon) |:|
//@[014:00048) |   | └─ObjectSyntax
//@[014:00015) |   |   ├─Token(LeftBrace) |{|
//@[015:00017) |   |   ├─Token(NewLine) |\r\n|
    foo: 'a'
//@[004:00012) |   |   ├─ObjectPropertySyntax
//@[004:00007) |   |   | ├─IdentifierSyntax
//@[004:00007) |   |   | | └─Token(Identifier) |foo|
//@[007:00008) |   |   | ├─Token(Colon) |:|
//@[009:00012) |   |   | └─StringSyntax
//@[009:00012) |   |   |   └─Token(StringComplete) |'a'|
//@[012:00014) |   |   ├─Token(NewLine) |\r\n|
    foo: 'a'
//@[004:00012) |   |   ├─ObjectPropertySyntax
//@[004:00007) |   |   | ├─IdentifierSyntax
//@[004:00007) |   |   | | └─Token(Identifier) |foo|
//@[007:00008) |   |   | ├─Token(Colon) |:|
//@[009:00012) |   |   | └─StringSyntax
//@[009:00012) |   |   |   └─Token(StringComplete) |'a'|
//@[012:00014) |   |   ├─Token(NewLine) |\r\n|
  }
//@[002:00003) |   |   └─Token(RightBrace) |}|
//@[003:00005) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

// duplicate property inside with string literal syntax
//@[055:00057) ├─Token(NewLine) |\r\n|
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= {
//@[000:00123) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00012) | ├─IdentifierSyntax
//@[009:00012) | | └─Token(Identifier) |foo|
//@[013:00050) | ├─StringSyntax
//@[013:00050) | | └─Token(StringComplete) |'Microsoft.Foo/foos@2020-02-02-alpha'|
//@[050:00051) | ├─Token(Assignment) |=|
//@[052:00123) | └─ObjectSyntax
//@[052:00053) |   ├─Token(LeftBrace) |{|
//@[053:00055) |   ├─Token(NewLine) |\r\n|
  name: 'foo'
//@[002:00013) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00013) |   | └─StringSyntax
//@[008:00013) |   |   └─Token(StringComplete) |'foo'|
//@[013:00015) |   ├─Token(NewLine) |\r\n|
  properties: {
//@[002:00050) |   ├─ObjectPropertySyntax
//@[002:00012) |   | ├─IdentifierSyntax
//@[002:00012) |   | | └─Token(Identifier) |properties|
//@[012:00013) |   | ├─Token(Colon) |:|
//@[014:00050) |   | └─ObjectSyntax
//@[014:00015) |   |   ├─Token(LeftBrace) |{|
//@[015:00017) |   |   ├─Token(NewLine) |\r\n|
    foo: 'a'
//@[004:00012) |   |   ├─ObjectPropertySyntax
//@[004:00007) |   |   | ├─IdentifierSyntax
//@[004:00007) |   |   | | └─Token(Identifier) |foo|
//@[007:00008) |   |   | ├─Token(Colon) |:|
//@[009:00012) |   |   | └─StringSyntax
//@[009:00012) |   |   |   └─Token(StringComplete) |'a'|
//@[012:00014) |   |   ├─Token(NewLine) |\r\n|
    'foo': 'a'
//@[004:00014) |   |   ├─ObjectPropertySyntax
//@[004:00009) |   |   | ├─StringSyntax
//@[004:00009) |   |   | | └─Token(StringComplete) |'foo'|
//@[009:00010) |   |   | ├─Token(Colon) |:|
//@[011:00014) |   |   | └─StringSyntax
//@[011:00014) |   |   |   └─Token(StringComplete) |'a'|
//@[014:00016) |   |   ├─Token(NewLine) |\r\n|
  }
//@[002:00003) |   |   └─Token(RightBrace) |}|
//@[003:00005) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

// wrong property types
//@[023:00025) ├─Token(NewLine) |\r\n|
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= {
//@[000:00124) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00012) | ├─IdentifierSyntax
//@[009:00012) | | └─Token(Identifier) |foo|
//@[013:00050) | ├─StringSyntax
//@[013:00050) | | └─Token(StringComplete) |'Microsoft.Foo/foos@2020-02-02-alpha'|
//@[050:00051) | ├─Token(Assignment) |=|
//@[052:00124) | └─ObjectSyntax
//@[052:00053) |   ├─Token(LeftBrace) |{|
//@[053:00055) |   ├─Token(NewLine) |\r\n|
  name: 'foo'
//@[002:00013) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00013) |   | └─StringSyntax
//@[008:00013) |   |   └─Token(StringComplete) |'foo'|
//@[013:00015) |   ├─Token(NewLine) |\r\n|
  location: [
//@[002:00018) |   ├─ObjectPropertySyntax
//@[002:00010) |   | ├─IdentifierSyntax
//@[002:00010) |   | | └─Token(Identifier) |location|
//@[010:00011) |   | ├─Token(Colon) |:|
//@[012:00018) |   | └─ArraySyntax
//@[012:00013) |   |   ├─Token(LeftSquare) |[|
//@[013:00015) |   |   ├─Token(NewLine) |\r\n|
  ]
//@[002:00003) |   |   └─Token(RightSquare) |]|
//@[003:00005) |   ├─Token(NewLine) |\r\n|
  tags: 'tag are not a string?'
//@[002:00031) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |tags|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00031) |   | └─StringSyntax
//@[008:00031) |   |   └─Token(StringComplete) |'tag are not a string?'|
//@[031:00033) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

resource bar 'Microsoft.Foo/foos@2020-02-02-alpha' = {
//@[000:00231) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00012) | ├─IdentifierSyntax
//@[009:00012) | | └─Token(Identifier) |bar|
//@[013:00050) | ├─StringSyntax
//@[013:00050) | | └─Token(StringComplete) |'Microsoft.Foo/foos@2020-02-02-alpha'|
//@[051:00052) | ├─Token(Assignment) |=|
//@[053:00231) | └─ObjectSyntax
//@[053:00054) |   ├─Token(LeftBrace) |{|
//@[054:00056) |   ├─Token(NewLine) |\r\n|
  name: true ? 's' : 'a' + 1
//@[002:00028) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00028) |   | └─TernaryOperationSyntax
//@[008:00012) |   |   ├─BooleanLiteralSyntax
//@[008:00012) |   |   | └─Token(TrueKeyword) |true|
//@[013:00014) |   |   ├─Token(Question) |?|
//@[015:00018) |   |   ├─StringSyntax
//@[015:00018) |   |   | └─Token(StringComplete) |'s'|
//@[019:00020) |   |   ├─Token(Colon) |:|
//@[021:00028) |   |   └─BinaryOperationSyntax
//@[021:00024) |   |     ├─StringSyntax
//@[021:00024) |   |     | └─Token(StringComplete) |'a'|
//@[025:00026) |   |     ├─Token(Plus) |+|
//@[027:00028) |   |     └─IntegerLiteralSyntax
//@[027:00028) |   |       └─Token(Integer) |1|
//@[028:00030) |   ├─Token(NewLine) |\r\n|
  properties: {
//@[002:00142) |   ├─ObjectPropertySyntax
//@[002:00012) |   | ├─IdentifierSyntax
//@[002:00012) |   | | └─Token(Identifier) |properties|
//@[012:00013) |   | ├─Token(Colon) |:|
//@[014:00142) |   | └─ObjectSyntax
//@[014:00015) |   |   ├─Token(LeftBrace) |{|
//@[015:00017) |   |   ├─Token(NewLine) |\r\n|
    x: foo()
//@[004:00012) |   |   ├─ObjectPropertySyntax
//@[004:00005) |   |   | ├─IdentifierSyntax
//@[004:00005) |   |   | | └─Token(Identifier) |x|
//@[005:00006) |   |   | ├─Token(Colon) |:|
//@[007:00012) |   |   | └─FunctionCallSyntax
//@[007:00010) |   |   |   ├─IdentifierSyntax
//@[007:00010) |   |   |   | └─Token(Identifier) |foo|
//@[010:00011) |   |   |   ├─Token(LeftParen) |(|
//@[011:00012) |   |   |   └─Token(RightParen) |)|
//@[012:00014) |   |   ├─Token(NewLine) |\r\n|
    y: true && (null || !4)
//@[004:00027) |   |   ├─ObjectPropertySyntax
//@[004:00005) |   |   | ├─IdentifierSyntax
//@[004:00005) |   |   | | └─Token(Identifier) |y|
//@[005:00006) |   |   | ├─Token(Colon) |:|
//@[007:00027) |   |   | └─BinaryOperationSyntax
//@[007:00011) |   |   |   ├─BooleanLiteralSyntax
//@[007:00011) |   |   |   | └─Token(TrueKeyword) |true|
//@[012:00014) |   |   |   ├─Token(LogicalAnd) |&&|
//@[015:00027) |   |   |   └─ParenthesizedExpressionSyntax
//@[015:00016) |   |   |     ├─Token(LeftParen) |(|
//@[016:00026) |   |   |     ├─BinaryOperationSyntax
//@[016:00020) |   |   |     | ├─NullLiteralSyntax
//@[016:00020) |   |   |     | | └─Token(NullKeyword) |null|
//@[021:00023) |   |   |     | ├─Token(LogicalOr) ||||
//@[024:00026) |   |   |     | └─UnaryOperationSyntax
//@[024:00025) |   |   |     |   ├─Token(Exclamation) |!|
//@[025:00026) |   |   |     |   └─IntegerLiteralSyntax
//@[025:00026) |   |   |     |     └─Token(Integer) |4|
//@[026:00027) |   |   |     └─Token(RightParen) |)|
//@[027:00029) |   |   ├─Token(NewLine) |\r\n|
    a: [
//@[004:00077) |   |   ├─ObjectPropertySyntax
//@[004:00005) |   |   | ├─IdentifierSyntax
//@[004:00005) |   |   | | └─Token(Identifier) |a|
//@[005:00006) |   |   | ├─Token(Colon) |:|
//@[007:00077) |   |   | └─ArraySyntax
//@[007:00008) |   |   |   ├─Token(LeftSquare) |[|
//@[008:00010) |   |   |   ├─Token(NewLine) |\r\n|
      a
//@[006:00007) |   |   |   ├─ArrayItemSyntax
//@[006:00007) |   |   |   | └─VariableAccessSyntax
//@[006:00007) |   |   |   |   └─IdentifierSyntax
//@[006:00007) |   |   |   |     └─Token(Identifier) |a|
//@[007:00009) |   |   |   ├─Token(NewLine) |\r\n|
      !null
//@[006:00011) |   |   |   ├─ArrayItemSyntax
//@[006:00011) |   |   |   | └─UnaryOperationSyntax
//@[006:00007) |   |   |   |   ├─Token(Exclamation) |!|
//@[007:00011) |   |   |   |   └─NullLiteralSyntax
//@[007:00011) |   |   |   |     └─Token(NullKeyword) |null|
//@[011:00013) |   |   |   ├─Token(NewLine) |\r\n|
      true && true || true + -true * 4
//@[006:00038) |   |   |   ├─ArrayItemSyntax
//@[006:00038) |   |   |   | └─BinaryOperationSyntax
//@[006:00018) |   |   |   |   ├─BinaryOperationSyntax
//@[006:00010) |   |   |   |   | ├─BooleanLiteralSyntax
//@[006:00010) |   |   |   |   | | └─Token(TrueKeyword) |true|
//@[011:00013) |   |   |   |   | ├─Token(LogicalAnd) |&&|
//@[014:00018) |   |   |   |   | └─BooleanLiteralSyntax
//@[014:00018) |   |   |   |   |   └─Token(TrueKeyword) |true|
//@[019:00021) |   |   |   |   ├─Token(LogicalOr) ||||
//@[022:00038) |   |   |   |   └─BinaryOperationSyntax
//@[022:00026) |   |   |   |     ├─BooleanLiteralSyntax
//@[022:00026) |   |   |   |     | └─Token(TrueKeyword) |true|
//@[027:00028) |   |   |   |     ├─Token(Plus) |+|
//@[029:00038) |   |   |   |     └─BinaryOperationSyntax
//@[029:00034) |   |   |   |       ├─UnaryOperationSyntax
//@[029:00030) |   |   |   |       | ├─Token(Minus) |-|
//@[030:00034) |   |   |   |       | └─BooleanLiteralSyntax
//@[030:00034) |   |   |   |       |   └─Token(TrueKeyword) |true|
//@[035:00036) |   |   |   |       ├─Token(Asterisk) |*|
//@[037:00038) |   |   |   |       └─IntegerLiteralSyntax
//@[037:00038) |   |   |   |         └─Token(Integer) |4|
//@[038:00040) |   |   |   ├─Token(NewLine) |\r\n|
    ]
//@[004:00005) |   |   |   └─Token(RightSquare) |]|
//@[005:00007) |   |   ├─Token(NewLine) |\r\n|
  }
//@[002:00003) |   |   └─Token(RightBrace) |}|
//@[003:00005) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

// there should be no completions without the colon
//@[051:00053) ├─Token(NewLine) |\r\n|
resource noCompletionsWithoutColon 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
//@[000:00138) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00034) | ├─IdentifierSyntax
//@[009:00034) | | └─Token(Identifier) |noCompletionsWithoutColon|
//@[035:00085) | ├─StringSyntax
//@[035:00085) | | └─Token(StringComplete) |'Microsoft.Resources/deploymentScripts@2020-10-01'|
//@[086:00087) | ├─Token(Assignment) |=|
//@[088:00138) | └─ObjectSyntax
//@[088:00089) |   ├─Token(LeftBrace) |{|
//@[089:00091) |   ├─Token(NewLine) |\r\n|
  // #completionTest(7,8) -> empty
//@[034:00036) |   ├─Token(NewLine) |\r\n|
  kind  
//@[002:00008) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |kind|
//@[008:00008) |   | ├─SkippedTriviaSyntax
//@[008:00008) |   | └─SkippedTriviaSyntax
//@[008:00010) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

resource noCompletionsBeforeColon 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
//@[000:00138) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00033) | ├─IdentifierSyntax
//@[009:00033) | | └─Token(Identifier) |noCompletionsBeforeColon|
//@[034:00084) | ├─StringSyntax
//@[034:00084) | | └─Token(StringComplete) |'Microsoft.Resources/deploymentScripts@2020-10-01'|
//@[085:00086) | ├─Token(Assignment) |=|
//@[087:00138) | └─ObjectSyntax
//@[087:00088) |   ├─Token(LeftBrace) |{|
//@[088:00090) |   ├─Token(NewLine) |\r\n|
  // #completionTest(7,8) -> empty
//@[034:00036) |   ├─Token(NewLine) |\r\n|
  kind  :
//@[002:00009) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |kind|
//@[008:00009) |   | ├─Token(Colon) |:|
//@[009:00009) |   | └─SkippedTriviaSyntax
//@[009:00011) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

// unsupported resource ref
//@[027:00029) ├─Token(NewLine) |\r\n|
var resrefvar = bar.name
//@[000:00024) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00013) | ├─IdentifierSyntax
//@[004:00013) | | └─Token(Identifier) |resrefvar|
//@[014:00015) | ├─Token(Assignment) |=|
//@[016:00024) | └─PropertyAccessSyntax
//@[016:00019) |   ├─VariableAccessSyntax
//@[016:00019) |   | └─IdentifierSyntax
//@[016:00019) |   |   └─Token(Identifier) |bar|
//@[019:00020) |   ├─Token(Dot) |.|
//@[020:00024) |   └─IdentifierSyntax
//@[020:00024) |     └─Token(Identifier) |name|
//@[024:00028) ├─Token(NewLine) |\r\n\r\n|

param resrefpar string = foo.id
//@[000:00031) ├─ParameterDeclarationSyntax
//@[000:00005) | ├─Token(Identifier) |param|
//@[006:00015) | ├─IdentifierSyntax
//@[006:00015) | | └─Token(Identifier) |resrefpar|
//@[016:00022) | ├─SimpleTypeSyntax
//@[016:00022) | | └─Token(Identifier) |string|
//@[023:00031) | └─ParameterDefaultValueSyntax
//@[023:00024) |   ├─Token(Assignment) |=|
//@[025:00031) |   └─PropertyAccessSyntax
//@[025:00028) |     ├─VariableAccessSyntax
//@[025:00028) |     | └─IdentifierSyntax
//@[025:00028) |     |   └─Token(Identifier) |foo|
//@[028:00029) |     ├─Token(Dot) |.|
//@[029:00031) |     └─IdentifierSyntax
//@[029:00031) |       └─Token(Identifier) |id|
//@[031:00035) ├─Token(NewLine) |\r\n\r\n|

output resrefout bool = bar.id
//@[000:00030) ├─OutputDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |output|
//@[007:00016) | ├─IdentifierSyntax
//@[007:00016) | | └─Token(Identifier) |resrefout|
//@[017:00021) | ├─SimpleTypeSyntax
//@[017:00021) | | └─Token(Identifier) |bool|
//@[022:00023) | ├─Token(Assignment) |=|
//@[024:00030) | └─PropertyAccessSyntax
//@[024:00027) |   ├─VariableAccessSyntax
//@[024:00027) |   | └─IdentifierSyntax
//@[024:00027) |   |   └─Token(Identifier) |bar|
//@[027:00028) |   ├─Token(Dot) |.|
//@[028:00030) |   └─IdentifierSyntax
//@[028:00030) |     └─Token(Identifier) |id|
//@[030:00034) ├─Token(NewLine) |\r\n\r\n|

// attempting to set read-only properties
//@[041:00043) ├─Token(NewLine) |\r\n|
resource baz 'Microsoft.Foo/foos@2020-02-02-alpha' = {
//@[000:00119) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00012) | ├─IdentifierSyntax
//@[009:00012) | | └─Token(Identifier) |baz|
//@[013:00050) | ├─StringSyntax
//@[013:00050) | | └─Token(StringComplete) |'Microsoft.Foo/foos@2020-02-02-alpha'|
//@[051:00052) | ├─Token(Assignment) |=|
//@[053:00119) | └─ObjectSyntax
//@[053:00054) |   ├─Token(LeftBrace) |{|
//@[054:00056) |   ├─Token(NewLine) |\r\n|
  name: 'test'
//@[002:00014) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00014) |   | └─StringSyntax
//@[008:00014) |   |   └─Token(StringComplete) |'test'|
//@[014:00016) |   ├─Token(NewLine) |\r\n|
  id: 2
//@[002:00007) |   ├─ObjectPropertySyntax
//@[002:00004) |   | ├─IdentifierSyntax
//@[002:00004) |   | | └─Token(Identifier) |id|
//@[004:00005) |   | ├─Token(Colon) |:|
//@[006:00007) |   | └─IntegerLiteralSyntax
//@[006:00007) |   |   └─Token(Integer) |2|
//@[007:00009) |   ├─Token(NewLine) |\r\n|
  type: 'hello'
//@[002:00015) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |type|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00015) |   | └─StringSyntax
//@[008:00015) |   |   └─Token(StringComplete) |'hello'|
//@[015:00017) |   ├─Token(NewLine) |\r\n|
  apiVersion: true
//@[002:00018) |   ├─ObjectPropertySyntax
//@[002:00012) |   | ├─IdentifierSyntax
//@[002:00012) |   | | └─Token(Identifier) |apiVersion|
//@[012:00013) |   | ├─Token(Colon) |:|
//@[014:00018) |   | └─BooleanLiteralSyntax
//@[014:00018) |   |   └─Token(TrueKeyword) |true|
//@[018:00020) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

resource readOnlyPropertyAssignment 'Microsoft.Network/virtualNetworks@2020-06-01' = {
//@[000:00352) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00035) | ├─IdentifierSyntax
//@[009:00035) | | └─Token(Identifier) |readOnlyPropertyAssignment|
//@[036:00082) | ├─StringSyntax
//@[036:00082) | | └─Token(StringComplete) |'Microsoft.Network/virtualNetworks@2020-06-01'|
//@[083:00084) | ├─Token(Assignment) |=|
//@[085:00352) | └─ObjectSyntax
//@[085:00086) |   ├─Token(LeftBrace) |{|
//@[086:00088) |   ├─Token(NewLine) |\r\n|
  name: 'vnet-bicep'
//@[002:00020) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00020) |   | └─StringSyntax
//@[008:00020) |   |   └─Token(StringComplete) |'vnet-bicep'|
//@[020:00022) |   ├─Token(NewLine) |\r\n|
  location: 'westeurope'
//@[002:00024) |   ├─ObjectPropertySyntax
//@[002:00010) |   | ├─IdentifierSyntax
//@[002:00010) |   | | └─Token(Identifier) |location|
//@[010:00011) |   | ├─Token(Colon) |:|
//@[012:00024) |   | └─StringSyntax
//@[012:00024) |   |   └─Token(StringComplete) |'westeurope'|
//@[024:00026) |   ├─Token(NewLine) |\r\n|
  etag: 'assigning-to-read-only-value'
//@[002:00038) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |etag|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00038) |   | └─StringSyntax
//@[008:00038) |   |   └─Token(StringComplete) |'assigning-to-read-only-value'|
//@[038:00040) |   ├─Token(NewLine) |\r\n|
  properties: {
//@[002:00173) |   ├─ObjectPropertySyntax
//@[002:00012) |   | ├─IdentifierSyntax
//@[002:00012) |   | | └─Token(Identifier) |properties|
//@[012:00013) |   | ├─Token(Colon) |:|
//@[014:00173) |   | └─ObjectSyntax
//@[014:00015) |   |   ├─Token(LeftBrace) |{|
//@[015:00017) |   |   ├─Token(NewLine) |\r\n|
    resourceGuid: 'assigning-to-read-only-value'
//@[004:00048) |   |   ├─ObjectPropertySyntax
//@[004:00016) |   |   | ├─IdentifierSyntax
//@[004:00016) |   |   | | └─Token(Identifier) |resourceGuid|
//@[016:00017) |   |   | ├─Token(Colon) |:|
//@[018:00048) |   |   | └─StringSyntax
//@[018:00048) |   |   |   └─Token(StringComplete) |'assigning-to-read-only-value'|
//@[048:00050) |   |   ├─Token(NewLine) |\r\n|
    addressSpace: {
//@[004:00084) |   |   ├─ObjectPropertySyntax
//@[004:00016) |   |   | ├─IdentifierSyntax
//@[004:00016) |   |   | | └─Token(Identifier) |addressSpace|
//@[016:00017) |   |   | ├─Token(Colon) |:|
//@[018:00084) |   |   | └─ObjectSyntax
//@[018:00019) |   |   |   ├─Token(LeftBrace) |{|
//@[019:00021) |   |   |   ├─Token(NewLine) |\r\n|
      addressPrefixes: [
//@[006:00056) |   |   |   ├─ObjectPropertySyntax
//@[006:00021) |   |   |   | ├─IdentifierSyntax
//@[006:00021) |   |   |   | | └─Token(Identifier) |addressPrefixes|
//@[021:00022) |   |   |   | ├─Token(Colon) |:|
//@[023:00056) |   |   |   | └─ArraySyntax
//@[023:00024) |   |   |   |   ├─Token(LeftSquare) |[|
//@[024:00026) |   |   |   |   ├─Token(NewLine) |\r\n|
        '10.0.0.0/16'
//@[008:00021) |   |   |   |   ├─ArrayItemSyntax
//@[008:00021) |   |   |   |   | └─StringSyntax
//@[008:00021) |   |   |   |   |   └─Token(StringComplete) |'10.0.0.0/16'|
//@[021:00023) |   |   |   |   ├─Token(NewLine) |\r\n|
      ]
//@[006:00007) |   |   |   |   └─Token(RightSquare) |]|
//@[007:00009) |   |   |   ├─Token(NewLine) |\r\n|
    }
//@[004:00005) |   |   |   └─Token(RightBrace) |}|
//@[005:00007) |   |   ├─Token(NewLine) |\r\n|
    subnets: []
//@[004:00015) |   |   ├─ObjectPropertySyntax
//@[004:00011) |   |   | ├─IdentifierSyntax
//@[004:00011) |   |   | | └─Token(Identifier) |subnets|
//@[011:00012) |   |   | ├─Token(Colon) |:|
//@[013:00015) |   |   | └─ArraySyntax
//@[013:00014) |   |   |   ├─Token(LeftSquare) |[|
//@[014:00015) |   |   |   └─Token(RightSquare) |]|
//@[015:00017) |   |   ├─Token(NewLine) |\r\n|
  }
//@[002:00003) |   |   └─Token(RightBrace) |}|
//@[003:00005) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

resource badDepends 'Microsoft.Foo/foos@2020-02-02-alpha' = {
//@[000:00113) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00019) | ├─IdentifierSyntax
//@[009:00019) | | └─Token(Identifier) |badDepends|
//@[020:00057) | ├─StringSyntax
//@[020:00057) | | └─Token(StringComplete) |'Microsoft.Foo/foos@2020-02-02-alpha'|
//@[058:00059) | ├─Token(Assignment) |=|
//@[060:00113) | └─ObjectSyntax
//@[060:00061) |   ├─Token(LeftBrace) |{|
//@[061:00063) |   ├─Token(NewLine) |\r\n|
  name: 'test'
//@[002:00014) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00014) |   | └─StringSyntax
//@[008:00014) |   |   └─Token(StringComplete) |'test'|
//@[014:00016) |   ├─Token(NewLine) |\r\n|
  dependsOn: [
//@[002:00031) |   ├─ObjectPropertySyntax
//@[002:00011) |   | ├─IdentifierSyntax
//@[002:00011) |   | | └─Token(Identifier) |dependsOn|
//@[011:00012) |   | ├─Token(Colon) |:|
//@[013:00031) |   | └─ArraySyntax
//@[013:00014) |   |   ├─Token(LeftSquare) |[|
//@[014:00016) |   |   ├─Token(NewLine) |\r\n|
    baz.id
//@[004:00010) |   |   ├─ArrayItemSyntax
//@[004:00010) |   |   | └─PropertyAccessSyntax
//@[004:00007) |   |   |   ├─VariableAccessSyntax
//@[004:00007) |   |   |   | └─IdentifierSyntax
//@[004:00007) |   |   |   |   └─Token(Identifier) |baz|
//@[007:00008) |   |   |   ├─Token(Dot) |.|
//@[008:00010) |   |   |   └─IdentifierSyntax
//@[008:00010) |   |   |     └─Token(Identifier) |id|
//@[010:00012) |   |   ├─Token(NewLine) |\r\n|
  ]
//@[002:00003) |   |   └─Token(RightSquare) |]|
//@[003:00005) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

resource badDepends2 'Microsoft.Foo/foos@2020-02-02-alpha' = {
//@[000:00125) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00020) | ├─IdentifierSyntax
//@[009:00020) | | └─Token(Identifier) |badDepends2|
//@[021:00058) | ├─StringSyntax
//@[021:00058) | | └─Token(StringComplete) |'Microsoft.Foo/foos@2020-02-02-alpha'|
//@[059:00060) | ├─Token(Assignment) |=|
//@[061:00125) | └─ObjectSyntax
//@[061:00062) |   ├─Token(LeftBrace) |{|
//@[062:00064) |   ├─Token(NewLine) |\r\n|
  name: 'test'
//@[002:00014) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00014) |   | └─StringSyntax
//@[008:00014) |   |   └─Token(StringComplete) |'test'|
//@[014:00016) |   ├─Token(NewLine) |\r\n|
  dependsOn: [
//@[002:00042) |   ├─ObjectPropertySyntax
//@[002:00011) |   | ├─IdentifierSyntax
//@[002:00011) |   | | └─Token(Identifier) |dependsOn|
//@[011:00012) |   | ├─Token(Colon) |:|
//@[013:00042) |   | └─ArraySyntax
//@[013:00014) |   |   ├─Token(LeftSquare) |[|
//@[014:00016) |   |   ├─Token(NewLine) |\r\n|
    'hello'
//@[004:00011) |   |   ├─ArrayItemSyntax
//@[004:00011) |   |   | └─StringSyntax
//@[004:00011) |   |   |   └─Token(StringComplete) |'hello'|
//@[011:00013) |   |   ├─Token(NewLine) |\r\n|
    true
//@[004:00008) |   |   ├─ArrayItemSyntax
//@[004:00008) |   |   | └─BooleanLiteralSyntax
//@[004:00008) |   |   |   └─Token(TrueKeyword) |true|
//@[008:00010) |   |   ├─Token(NewLine) |\r\n|
  ]
//@[002:00003) |   |   └─Token(RightSquare) |]|
//@[003:00005) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

resource badDepends3 'Microsoft.Foo/foos@2020-02-02-alpha' = {
//@[000:00081) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00020) | ├─IdentifierSyntax
//@[009:00020) | | └─Token(Identifier) |badDepends3|
//@[021:00058) | ├─StringSyntax
//@[021:00058) | | └─Token(StringComplete) |'Microsoft.Foo/foos@2020-02-02-alpha'|
//@[059:00060) | ├─Token(Assignment) |=|
//@[061:00081) | └─ObjectSyntax
//@[061:00062) |   ├─Token(LeftBrace) |{|
//@[062:00064) |   ├─Token(NewLine) |\r\n|
  name: 'test'
//@[002:00014) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00014) |   | └─StringSyntax
//@[008:00014) |   |   └─Token(StringComplete) |'test'|
//@[014:00016) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

resource badDepends4 'Microsoft.Foo/foos@2020-02-02-alpha' = {
//@[000:00119) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00020) | ├─IdentifierSyntax
//@[009:00020) | | └─Token(Identifier) |badDepends4|
//@[021:00058) | ├─StringSyntax
//@[021:00058) | | └─Token(StringComplete) |'Microsoft.Foo/foos@2020-02-02-alpha'|
//@[059:00060) | ├─Token(Assignment) |=|
//@[061:00119) | └─ObjectSyntax
//@[061:00062) |   ├─Token(LeftBrace) |{|
//@[062:00064) |   ├─Token(NewLine) |\r\n|
  name: 'test'
//@[002:00014) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00014) |   | └─StringSyntax
//@[008:00014) |   |   └─Token(StringComplete) |'test'|
//@[014:00016) |   ├─Token(NewLine) |\r\n|
  dependsOn: [
//@[002:00036) |   ├─ObjectPropertySyntax
//@[002:00011) |   | ├─IdentifierSyntax
//@[002:00011) |   | | └─Token(Identifier) |dependsOn|
//@[011:00012) |   | ├─Token(Colon) |:|
//@[013:00036) |   | └─ArraySyntax
//@[013:00014) |   |   ├─Token(LeftSquare) |[|
//@[014:00016) |   |   ├─Token(NewLine) |\r\n|
    badDepends3
//@[004:00015) |   |   ├─ArrayItemSyntax
//@[004:00015) |   |   | └─VariableAccessSyntax
//@[004:00015) |   |   |   └─IdentifierSyntax
//@[004:00015) |   |   |     └─Token(Identifier) |badDepends3|
//@[015:00017) |   |   ├─Token(NewLine) |\r\n|
  ]
//@[002:00003) |   |   └─Token(RightSquare) |]|
//@[003:00005) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

resource badDepends5 'Microsoft.Foo/foos@2020-02-02-alpha' = {
//@[000:00117) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00020) | ├─IdentifierSyntax
//@[009:00020) | | └─Token(Identifier) |badDepends5|
//@[021:00058) | ├─StringSyntax
//@[021:00058) | | └─Token(StringComplete) |'Microsoft.Foo/foos@2020-02-02-alpha'|
//@[059:00060) | ├─Token(Assignment) |=|
//@[061:00117) | └─ObjectSyntax
//@[061:00062) |   ├─Token(LeftBrace) |{|
//@[062:00064) |   ├─Token(NewLine) |\r\n|
  name: 'test'
//@[002:00014) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00014) |   | └─StringSyntax
//@[008:00014) |   |   └─Token(StringComplete) |'test'|
//@[014:00016) |   ├─Token(NewLine) |\r\n|
  dependsOn: badDepends3.dependsOn
//@[002:00034) |   ├─ObjectPropertySyntax
//@[002:00011) |   | ├─IdentifierSyntax
//@[002:00011) |   | | └─Token(Identifier) |dependsOn|
//@[011:00012) |   | ├─Token(Colon) |:|
//@[013:00034) |   | └─PropertyAccessSyntax
//@[013:00024) |   |   ├─VariableAccessSyntax
//@[013:00024) |   |   | └─IdentifierSyntax
//@[013:00024) |   |   |   └─Token(Identifier) |badDepends3|
//@[024:00025) |   |   ├─Token(Dot) |.|
//@[025:00034) |   |   └─IdentifierSyntax
//@[025:00034) |   |     └─Token(Identifier) |dependsOn|
//@[034:00036) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

var interpVal = 'abc'
//@[000:00021) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00013) | ├─IdentifierSyntax
//@[004:00013) | | └─Token(Identifier) |interpVal|
//@[014:00015) | ├─Token(Assignment) |=|
//@[016:00021) | └─StringSyntax
//@[016:00021) |   └─Token(StringComplete) |'abc'|
//@[021:00023) ├─Token(NewLine) |\r\n|
resource badInterp 'Microsoft.Foo/foos@2020-02-02-alpha' = {
//@[000:00205) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00018) | ├─IdentifierSyntax
//@[009:00018) | | └─Token(Identifier) |badInterp|
//@[019:00056) | ├─StringSyntax
//@[019:00056) | | └─Token(StringComplete) |'Microsoft.Foo/foos@2020-02-02-alpha'|
//@[057:00058) | ├─Token(Assignment) |=|
//@[059:00205) | └─ObjectSyntax
//@[059:00060) |   ├─Token(LeftBrace) |{|
//@[060:00062) |   ├─Token(NewLine) |\r\n|
  name: 'test'
//@[002:00014) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00014) |   | └─StringSyntax
//@[008:00014) |   |   └─Token(StringComplete) |'test'|
//@[014:00016) |   ├─Token(NewLine) |\r\n|
  '${interpVal}': 'unsupported' // resource definition does not allow for additionalProperties
//@[002:00031) |   ├─ObjectPropertySyntax
//@[002:00016) |   | ├─StringSyntax
//@[002:00005) |   | | ├─Token(StringLeftPiece) |'${|
//@[005:00014) |   | | ├─VariableAccessSyntax
//@[005:00014) |   | | | └─IdentifierSyntax
//@[005:00014) |   | | |   └─Token(Identifier) |interpVal|
//@[014:00016) |   | | └─Token(StringRightPiece) |}'|
//@[016:00017) |   | ├─Token(Colon) |:|
//@[018:00031) |   | └─StringSyntax
//@[018:00031) |   |   └─Token(StringComplete) |'unsupported'|
//@[094:00096) |   ├─Token(NewLine) |\r\n|
  '${undefinedSymbol}': true
//@[002:00028) |   ├─ObjectPropertySyntax
//@[002:00022) |   | ├─StringSyntax
//@[002:00005) |   | | ├─Token(StringLeftPiece) |'${|
//@[005:00020) |   | | ├─VariableAccessSyntax
//@[005:00020) |   | | | └─IdentifierSyntax
//@[005:00020) |   | | |   └─Token(Identifier) |undefinedSymbol|
//@[020:00022) |   | | └─Token(StringRightPiece) |}'|
//@[022:00023) |   | ├─Token(Colon) |:|
//@[024:00028) |   | └─BooleanLiteralSyntax
//@[024:00028) |   |   └─Token(TrueKeyword) |true|
//@[028:00030) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

module validModule './module.bicep' = {
//@[000:00106) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00018) | ├─IdentifierSyntax
//@[007:00018) | | └─Token(Identifier) |validModule|
//@[019:00035) | ├─StringSyntax
//@[019:00035) | | └─Token(StringComplete) |'./module.bicep'|
//@[036:00037) | ├─Token(Assignment) |=|
//@[038:00106) | └─ObjectSyntax
//@[038:00039) |   ├─Token(LeftBrace) |{|
//@[039:00041) |   ├─Token(NewLine) |\r\n|
  name: 'storageDeploy'
//@[002:00023) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00023) |   | └─StringSyntax
//@[008:00023) |   |   └─Token(StringComplete) |'storageDeploy'|
//@[023:00025) |   ├─Token(NewLine) |\r\n|
  params: {
//@[002:00037) |   ├─ObjectPropertySyntax
//@[002:00008) |   | ├─IdentifierSyntax
//@[002:00008) |   | | └─Token(Identifier) |params|
//@[008:00009) |   | ├─Token(Colon) |:|
//@[010:00037) |   | └─ObjectSyntax
//@[010:00011) |   |   ├─Token(LeftBrace) |{|
//@[011:00013) |   |   ├─Token(NewLine) |\r\n|
    name: 'contoso'
//@[004:00019) |   |   ├─ObjectPropertySyntax
//@[004:00008) |   |   | ├─IdentifierSyntax
//@[004:00008) |   |   | | └─Token(Identifier) |name|
//@[008:00009) |   |   | ├─Token(Colon) |:|
//@[010:00019) |   |   | └─StringSyntax
//@[010:00019) |   |   |   └─Token(StringComplete) |'contoso'|
//@[019:00021) |   |   ├─Token(NewLine) |\r\n|
  }
//@[002:00003) |   |   └─Token(RightBrace) |}|
//@[003:00005) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

resource runtimeValidRes1 'Microsoft.Compute/virtualMachines@2020-06-01' = {
//@[000:00174) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00025) | ├─IdentifierSyntax
//@[009:00025) | | └─Token(Identifier) |runtimeValidRes1|
//@[026:00072) | ├─StringSyntax
//@[026:00072) | | └─Token(StringComplete) |'Microsoft.Compute/virtualMachines@2020-06-01'|
//@[073:00074) | ├─Token(Assignment) |=|
//@[075:00174) | └─ObjectSyntax
//@[075:00076) |   ├─Token(LeftBrace) |{|
//@[076:00078) |   ├─Token(NewLine) |\r\n|
  name: 'name1'
//@[002:00015) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00015) |   | └─StringSyntax
//@[008:00015) |   |   └─Token(StringComplete) |'name1'|
//@[015:00017) |   ├─Token(NewLine) |\r\n|
  location: 'eastus'
//@[002:00020) |   ├─ObjectPropertySyntax
//@[002:00010) |   | ├─IdentifierSyntax
//@[002:00010) |   | | └─Token(Identifier) |location|
//@[010:00011) |   | ├─Token(Colon) |:|
//@[012:00020) |   | └─StringSyntax
//@[012:00020) |   |   └─Token(StringComplete) |'eastus'|
//@[020:00022) |   ├─Token(NewLine) |\r\n|
  properties: {
//@[002:00054) |   ├─ObjectPropertySyntax
//@[002:00012) |   | ├─IdentifierSyntax
//@[002:00012) |   | | └─Token(Identifier) |properties|
//@[012:00013) |   | ├─Token(Colon) |:|
//@[014:00054) |   | └─ObjectSyntax
//@[014:00015) |   |   ├─Token(LeftBrace) |{|
//@[015:00017) |   |   ├─Token(NewLine) |\r\n|
    evictionPolicy: 'Deallocate'
//@[004:00032) |   |   ├─ObjectPropertySyntax
//@[004:00018) |   |   | ├─IdentifierSyntax
//@[004:00018) |   |   | | └─Token(Identifier) |evictionPolicy|
//@[018:00019) |   |   | ├─Token(Colon) |:|
//@[020:00032) |   |   | └─StringSyntax
//@[020:00032) |   |   |   └─Token(StringComplete) |'Deallocate'|
//@[032:00034) |   |   ├─Token(NewLine) |\r\n|
  }
//@[002:00003) |   |   └─Token(RightBrace) |}|
//@[003:00005) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

resource runtimeValidRes2 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
//@[000:00329) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00025) | ├─IdentifierSyntax
//@[009:00025) | | └─Token(Identifier) |runtimeValidRes2|
//@[026:00076) | ├─StringSyntax
//@[026:00076) | | └─Token(StringComplete) |'Microsoft.Resources/deploymentScripts@2020-10-01'|
//@[077:00078) | ├─Token(Assignment) |=|
//@[079:00329) | └─ObjectSyntax
//@[079:00080) |   ├─Token(LeftBrace) |{|
//@[080:00082) |   ├─Token(NewLine) |\r\n|
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
//@[089:00091) |   ├─Token(NewLine) |\r\n|
  kind:'AzureCLI'
//@[002:00017) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |kind|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[007:00017) |   | └─StringSyntax
//@[007:00017) |   |   └─Token(StringComplete) |'AzureCLI'|
//@[017:00019) |   ├─Token(NewLine) |\r\n|
  location: 'eastus'
//@[002:00020) |   ├─ObjectPropertySyntax
//@[002:00010) |   | ├─IdentifierSyntax
//@[002:00010) |   | | └─Token(Identifier) |location|
//@[010:00011) |   | ├─Token(Colon) |:|
//@[012:00020) |   | └─StringSyntax
//@[012:00020) |   |   └─Token(StringComplete) |'eastus'|
//@[020:00022) |   ├─Token(NewLine) |\r\n|
  properties: {
//@[002:00112) |   ├─ObjectPropertySyntax
//@[002:00012) |   | ├─IdentifierSyntax
//@[002:00012) |   | | └─Token(Identifier) |properties|
//@[012:00013) |   | ├─Token(Colon) |:|
//@[014:00112) |   | └─ObjectSyntax
//@[014:00015) |   |   ├─Token(LeftBrace) |{|
//@[015:00017) |   |   ├─Token(NewLine) |\r\n|
    azCliVersion: '2.0'
//@[004:00023) |   |   ├─ObjectPropertySyntax
//@[004:00016) |   |   | ├─IdentifierSyntax
//@[004:00016) |   |   | | └─Token(Identifier) |azCliVersion|
//@[016:00017) |   |   | ├─Token(Colon) |:|
//@[018:00023) |   |   | └─StringSyntax
//@[018:00023) |   |   |   └─Token(StringComplete) |'2.0'|
//@[023:00025) |   |   ├─Token(NewLine) |\r\n|
    retentionInterval: runtimeValidRes1.properties.evictionPolicy
//@[004:00065) |   |   ├─ObjectPropertySyntax
//@[004:00021) |   |   | ├─IdentifierSyntax
//@[004:00021) |   |   | | └─Token(Identifier) |retentionInterval|
//@[021:00022) |   |   | ├─Token(Colon) |:|
//@[023:00065) |   |   | └─PropertyAccessSyntax
//@[023:00050) |   |   |   ├─PropertyAccessSyntax
//@[023:00039) |   |   |   | ├─VariableAccessSyntax
//@[023:00039) |   |   |   | | └─IdentifierSyntax
//@[023:00039) |   |   |   | |   └─Token(Identifier) |runtimeValidRes1|
//@[039:00040) |   |   |   | ├─Token(Dot) |.|
//@[040:00050) |   |   |   | └─IdentifierSyntax
//@[040:00050) |   |   |   |   └─Token(Identifier) |properties|
//@[050:00051) |   |   |   ├─Token(Dot) |.|
//@[051:00065) |   |   |   └─IdentifierSyntax
//@[051:00065) |   |   |     └─Token(Identifier) |evictionPolicy|
//@[065:00067) |   |   ├─Token(NewLine) |\r\n|
  }
//@[002:00003) |   |   └─Token(RightBrace) |}|
//@[003:00005) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

resource runtimeValidRes3 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[000:00131) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00025) | ├─IdentifierSyntax
//@[009:00025) | | └─Token(Identifier) |runtimeValidRes3|
//@[026:00085) | ├─StringSyntax
//@[026:00085) | | └─Token(StringComplete) |'Microsoft.Advisor/recommendations/suppressions@2020-01-01'|
//@[086:00087) | ├─Token(Assignment) |=|
//@[088:00131) | └─ObjectSyntax
//@[088:00089) |   ├─Token(LeftBrace) |{|
//@[089:00091) |   ├─Token(NewLine) |\r\n|
  name: '${runtimeValidRes1.name}_v1'
//@[002:00037) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00037) |   | └─StringSyntax
//@[008:00011) |   |   ├─Token(StringLeftPiece) |'${|
//@[011:00032) |   |   ├─PropertyAccessSyntax
//@[011:00027) |   |   | ├─VariableAccessSyntax
//@[011:00027) |   |   | | └─IdentifierSyntax
//@[011:00027) |   |   | |   └─Token(Identifier) |runtimeValidRes1|
//@[027:00028) |   |   | ├─Token(Dot) |.|
//@[028:00032) |   |   | └─IdentifierSyntax
//@[028:00032) |   |   |   └─Token(Identifier) |name|
//@[032:00037) |   |   └─Token(StringRightPiece) |}_v1'|
//@[037:00039) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

resource runtimeValidRes4 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[000:00135) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00025) | ├─IdentifierSyntax
//@[009:00025) | | └─Token(Identifier) |runtimeValidRes4|
//@[026:00085) | ├─StringSyntax
//@[026:00085) | | └─Token(StringComplete) |'Microsoft.Advisor/recommendations/suppressions@2020-01-01'|
//@[086:00087) | ├─Token(Assignment) |=|
//@[088:00135) | └─ObjectSyntax
//@[088:00089) |   ├─Token(LeftBrace) |{|
//@[089:00091) |   ├─Token(NewLine) |\r\n|
  name: concat(validModule['name'], 'v1')
//@[002:00041) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00041) |   | └─FunctionCallSyntax
//@[008:00014) |   |   ├─IdentifierSyntax
//@[008:00014) |   |   | └─Token(Identifier) |concat|
//@[014:00015) |   |   ├─Token(LeftParen) |(|
//@[015:00034) |   |   ├─FunctionArgumentSyntax
//@[015:00034) |   |   | └─ArrayAccessSyntax
//@[015:00026) |   |   |   ├─VariableAccessSyntax
//@[015:00026) |   |   |   | └─IdentifierSyntax
//@[015:00026) |   |   |   |   └─Token(Identifier) |validModule|
//@[026:00027) |   |   |   ├─Token(LeftSquare) |[|
//@[027:00033) |   |   |   ├─StringSyntax
//@[027:00033) |   |   |   | └─Token(StringComplete) |'name'|
//@[033:00034) |   |   |   └─Token(RightSquare) |]|
//@[034:00035) |   |   ├─Token(Comma) |,|
//@[036:00040) |   |   ├─FunctionArgumentSyntax
//@[036:00040) |   |   | └─StringSyntax
//@[036:00040) |   |   |   └─Token(StringComplete) |'v1'|
//@[040:00041) |   |   └─Token(RightParen) |)|
//@[041:00043) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

resource runtimeValidRes5 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[000:00126) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00025) | ├─IdentifierSyntax
//@[009:00025) | | └─Token(Identifier) |runtimeValidRes5|
//@[026:00085) | ├─StringSyntax
//@[026:00085) | | └─Token(StringComplete) |'Microsoft.Advisor/recommendations/suppressions@2020-01-01'|
//@[086:00087) | ├─Token(Assignment) |=|
//@[088:00126) | └─ObjectSyntax
//@[088:00089) |   ├─Token(LeftBrace) |{|
//@[089:00091) |   ├─Token(NewLine) |\r\n|
  name: '${validModule.name}_v1'
//@[002:00032) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00032) |   | └─StringSyntax
//@[008:00011) |   |   ├─Token(StringLeftPiece) |'${|
//@[011:00027) |   |   ├─PropertyAccessSyntax
//@[011:00022) |   |   | ├─VariableAccessSyntax
//@[011:00022) |   |   | | └─IdentifierSyntax
//@[011:00022) |   |   | |   └─Token(Identifier) |validModule|
//@[022:00023) |   |   | ├─Token(Dot) |.|
//@[023:00027) |   |   | └─IdentifierSyntax
//@[023:00027) |   |   |   └─Token(Identifier) |name|
//@[027:00032) |   |   └─Token(StringRightPiece) |}_v1'|
//@[032:00034) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

resource runtimeInvalidRes1 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[000:00129) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00027) | ├─IdentifierSyntax
//@[009:00027) | | └─Token(Identifier) |runtimeInvalidRes1|
//@[028:00087) | ├─StringSyntax
//@[028:00087) | | └─Token(StringComplete) |'Microsoft.Advisor/recommendations/suppressions@2020-01-01'|
//@[088:00089) | ├─Token(Assignment) |=|
//@[090:00129) | └─ObjectSyntax
//@[090:00091) |   ├─Token(LeftBrace) |{|
//@[091:00093) |   ├─Token(NewLine) |\r\n|
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
//@[033:00035) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

resource runtimeInvalidRes2 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[000:00132) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00027) | ├─IdentifierSyntax
//@[009:00027) | | └─Token(Identifier) |runtimeInvalidRes2|
//@[028:00087) | ├─StringSyntax
//@[028:00087) | | └─Token(StringComplete) |'Microsoft.Advisor/recommendations/suppressions@2020-01-01'|
//@[088:00089) | ├─Token(Assignment) |=|
//@[090:00132) | └─ObjectSyntax
//@[090:00091) |   ├─Token(LeftBrace) |{|
//@[091:00093) |   ├─Token(NewLine) |\r\n|
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
//@[036:00038) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

resource runtimeInvalidRes3 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
//@[000:00292) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00027) | ├─IdentifierSyntax
//@[009:00027) | | └─Token(Identifier) |runtimeInvalidRes3|
//@[028:00078) | ├─StringSyntax
//@[028:00078) | | └─Token(StringComplete) |'Microsoft.Resources/deploymentScripts@2020-10-01'|
//@[079:00080) | ├─Token(Assignment) |=|
//@[081:00292) | └─ObjectSyntax
//@[081:00082) |   ├─Token(LeftBrace) |{|
//@[082:00084) |   ├─Token(NewLine) |\r\n|
  name: runtimeValidRes1.properties.evictionPolicy
//@[002:00050) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00050) |   | └─PropertyAccessSyntax
//@[008:00035) |   |   ├─PropertyAccessSyntax
//@[008:00024) |   |   | ├─VariableAccessSyntax
//@[008:00024) |   |   | | └─IdentifierSyntax
//@[008:00024) |   |   | |   └─Token(Identifier) |runtimeValidRes1|
//@[024:00025) |   |   | ├─Token(Dot) |.|
//@[025:00035) |   |   | └─IdentifierSyntax
//@[025:00035) |   |   |   └─Token(Identifier) |properties|
//@[035:00036) |   |   ├─Token(Dot) |.|
//@[036:00050) |   |   └─IdentifierSyntax
//@[036:00050) |   |     └─Token(Identifier) |evictionPolicy|
//@[050:00052) |   ├─Token(NewLine) |\r\n|
  kind:'AzureCLI'
//@[002:00017) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |kind|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[007:00017) |   | └─StringSyntax
//@[007:00017) |   |   └─Token(StringComplete) |'AzureCLI'|
//@[017:00019) |   ├─Token(NewLine) |\r\n|
  location: 'eastus'
//@[002:00020) |   ├─ObjectPropertySyntax
//@[002:00010) |   | ├─IdentifierSyntax
//@[002:00010) |   | | └─Token(Identifier) |location|
//@[010:00011) |   | ├─Token(Colon) |:|
//@[012:00020) |   | └─StringSyntax
//@[012:00020) |   |   └─Token(StringComplete) |'eastus'|
//@[020:00022) |   ├─Token(NewLine) |\r\n|
  properties: {
//@[002:00112) |   ├─ObjectPropertySyntax
//@[002:00012) |   | ├─IdentifierSyntax
//@[002:00012) |   | | └─Token(Identifier) |properties|
//@[012:00013) |   | ├─Token(Colon) |:|
//@[014:00112) |   | └─ObjectSyntax
//@[014:00015) |   |   ├─Token(LeftBrace) |{|
//@[015:00017) |   |   ├─Token(NewLine) |\r\n|
    azCliVersion: '2.0'
//@[004:00023) |   |   ├─ObjectPropertySyntax
//@[004:00016) |   |   | ├─IdentifierSyntax
//@[004:00016) |   |   | | └─Token(Identifier) |azCliVersion|
//@[016:00017) |   |   | ├─Token(Colon) |:|
//@[018:00023) |   |   | └─StringSyntax
//@[018:00023) |   |   |   └─Token(StringComplete) |'2.0'|
//@[023:00025) |   |   ├─Token(NewLine) |\r\n|
    retentionInterval: runtimeValidRes1.properties.evictionPolicy
//@[004:00065) |   |   ├─ObjectPropertySyntax
//@[004:00021) |   |   | ├─IdentifierSyntax
//@[004:00021) |   |   | | └─Token(Identifier) |retentionInterval|
//@[021:00022) |   |   | ├─Token(Colon) |:|
//@[023:00065) |   |   | └─PropertyAccessSyntax
//@[023:00050) |   |   |   ├─PropertyAccessSyntax
//@[023:00039) |   |   |   | ├─VariableAccessSyntax
//@[023:00039) |   |   |   | | └─IdentifierSyntax
//@[023:00039) |   |   |   | |   └─Token(Identifier) |runtimeValidRes1|
//@[039:00040) |   |   |   | ├─Token(Dot) |.|
//@[040:00050) |   |   |   | └─IdentifierSyntax
//@[040:00050) |   |   |   |   └─Token(Identifier) |properties|
//@[050:00051) |   |   |   ├─Token(Dot) |.|
//@[051:00065) |   |   |   └─IdentifierSyntax
//@[051:00065) |   |   |     └─Token(Identifier) |evictionPolicy|
//@[065:00067) |   |   ├─Token(NewLine) |\r\n|
  }
//@[002:00003) |   |   └─Token(RightBrace) |}|
//@[003:00005) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

resource runtimeInvalidRes4 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[000:00149) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00027) | ├─IdentifierSyntax
//@[009:00027) | | └─Token(Identifier) |runtimeInvalidRes4|
//@[028:00087) | ├─StringSyntax
//@[028:00087) | | └─Token(StringComplete) |'Microsoft.Advisor/recommendations/suppressions@2020-01-01'|
//@[088:00089) | ├─Token(Assignment) |=|
//@[090:00149) | └─ObjectSyntax
//@[090:00091) |   ├─Token(LeftBrace) |{|
//@[091:00093) |   ├─Token(NewLine) |\r\n|
  name: runtimeValidRes1['properties'].evictionPolicy
//@[002:00053) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00053) |   | └─PropertyAccessSyntax
//@[008:00038) |   |   ├─ArrayAccessSyntax
//@[008:00024) |   |   | ├─VariableAccessSyntax
//@[008:00024) |   |   | | └─IdentifierSyntax
//@[008:00024) |   |   | |   └─Token(Identifier) |runtimeValidRes1|
//@[024:00025) |   |   | ├─Token(LeftSquare) |[|
//@[025:00037) |   |   | ├─StringSyntax
//@[025:00037) |   |   | | └─Token(StringComplete) |'properties'|
//@[037:00038) |   |   | └─Token(RightSquare) |]|
//@[038:00039) |   |   ├─Token(Dot) |.|
//@[039:00053) |   |   └─IdentifierSyntax
//@[039:00053) |   |     └─Token(Identifier) |evictionPolicy|
//@[053:00055) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

resource runtimeInvalidRes5 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[000:00152) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00027) | ├─IdentifierSyntax
//@[009:00027) | | └─Token(Identifier) |runtimeInvalidRes5|
//@[028:00087) | ├─StringSyntax
//@[028:00087) | | └─Token(StringComplete) |'Microsoft.Advisor/recommendations/suppressions@2020-01-01'|
//@[088:00089) | ├─Token(Assignment) |=|
//@[090:00152) | └─ObjectSyntax
//@[090:00091) |   ├─Token(LeftBrace) |{|
//@[091:00093) |   ├─Token(NewLine) |\r\n|
  name: runtimeValidRes1['properties']['evictionPolicy']
//@[002:00056) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00056) |   | └─ArrayAccessSyntax
//@[008:00038) |   |   ├─ArrayAccessSyntax
//@[008:00024) |   |   | ├─VariableAccessSyntax
//@[008:00024) |   |   | | └─IdentifierSyntax
//@[008:00024) |   |   | |   └─Token(Identifier) |runtimeValidRes1|
//@[024:00025) |   |   | ├─Token(LeftSquare) |[|
//@[025:00037) |   |   | ├─StringSyntax
//@[025:00037) |   |   | | └─Token(StringComplete) |'properties'|
//@[037:00038) |   |   | └─Token(RightSquare) |]|
//@[038:00039) |   |   ├─Token(LeftSquare) |[|
//@[039:00055) |   |   ├─StringSyntax
//@[039:00055) |   |   | └─Token(StringComplete) |'evictionPolicy'|
//@[055:00056) |   |   └─Token(RightSquare) |]|
//@[056:00058) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

resource runtimeInvalidRes6 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[000:00149) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00027) | ├─IdentifierSyntax
//@[009:00027) | | └─Token(Identifier) |runtimeInvalidRes6|
//@[028:00087) | ├─StringSyntax
//@[028:00087) | | └─Token(StringComplete) |'Microsoft.Advisor/recommendations/suppressions@2020-01-01'|
//@[088:00089) | ├─Token(Assignment) |=|
//@[090:00149) | └─ObjectSyntax
//@[090:00091) |   ├─Token(LeftBrace) |{|
//@[091:00093) |   ├─Token(NewLine) |\r\n|
  name: runtimeValidRes1.properties['evictionPolicy']
//@[002:00053) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00053) |   | └─ArrayAccessSyntax
//@[008:00035) |   |   ├─PropertyAccessSyntax
//@[008:00024) |   |   | ├─VariableAccessSyntax
//@[008:00024) |   |   | | └─IdentifierSyntax
//@[008:00024) |   |   | |   └─Token(Identifier) |runtimeValidRes1|
//@[024:00025) |   |   | ├─Token(Dot) |.|
//@[025:00035) |   |   | └─IdentifierSyntax
//@[025:00035) |   |   |   └─Token(Identifier) |properties|
//@[035:00036) |   |   ├─Token(LeftSquare) |[|
//@[036:00052) |   |   ├─StringSyntax
//@[036:00052) |   |   | └─Token(StringComplete) |'evictionPolicy'|
//@[052:00053) |   |   └─Token(RightSquare) |]|
//@[053:00055) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

resource runtimeInvalidRes7 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[000:00144) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00027) | ├─IdentifierSyntax
//@[009:00027) | | └─Token(Identifier) |runtimeInvalidRes7|
//@[028:00087) | ├─StringSyntax
//@[028:00087) | | └─Token(StringComplete) |'Microsoft.Advisor/recommendations/suppressions@2020-01-01'|
//@[088:00089) | ├─Token(Assignment) |=|
//@[090:00144) | └─ObjectSyntax
//@[090:00091) |   ├─Token(LeftBrace) |{|
//@[091:00093) |   ├─Token(NewLine) |\r\n|
  name: runtimeValidRes2.properties.azCliVersion
//@[002:00048) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00048) |   | └─PropertyAccessSyntax
//@[008:00035) |   |   ├─PropertyAccessSyntax
//@[008:00024) |   |   | ├─VariableAccessSyntax
//@[008:00024) |   |   | | └─IdentifierSyntax
//@[008:00024) |   |   | |   └─Token(Identifier) |runtimeValidRes2|
//@[024:00025) |   |   | ├─Token(Dot) |.|
//@[025:00035) |   |   | └─IdentifierSyntax
//@[025:00035) |   |   |   └─Token(Identifier) |properties|
//@[035:00036) |   |   ├─Token(Dot) |.|
//@[036:00048) |   |   └─IdentifierSyntax
//@[036:00048) |   |     └─Token(Identifier) |azCliVersion|
//@[048:00050) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

var magicString1 = 'location'
//@[000:00029) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00016) | ├─IdentifierSyntax
//@[004:00016) | | └─Token(Identifier) |magicString1|
//@[017:00018) | ├─Token(Assignment) |=|
//@[019:00029) | └─StringSyntax
//@[019:00029) |   └─Token(StringComplete) |'location'|
//@[029:00031) ├─Token(NewLine) |\r\n|
resource runtimeInvalidRes8 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[000:00139) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00027) | ├─IdentifierSyntax
//@[009:00027) | | └─Token(Identifier) |runtimeInvalidRes8|
//@[028:00087) | ├─StringSyntax
//@[028:00087) | | └─Token(StringComplete) |'Microsoft.Advisor/recommendations/suppressions@2020-01-01'|
//@[088:00089) | ├─Token(Assignment) |=|
//@[090:00139) | └─ObjectSyntax
//@[090:00091) |   ├─Token(LeftBrace) |{|
//@[091:00093) |   ├─Token(NewLine) |\r\n|
  name: runtimeValidRes2['${magicString1}']
//@[002:00043) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00043) |   | └─ArrayAccessSyntax
//@[008:00024) |   |   ├─VariableAccessSyntax
//@[008:00024) |   |   | └─IdentifierSyntax
//@[008:00024) |   |   |   └─Token(Identifier) |runtimeValidRes2|
//@[024:00025) |   |   ├─Token(LeftSquare) |[|
//@[025:00042) |   |   ├─StringSyntax
//@[025:00028) |   |   | ├─Token(StringLeftPiece) |'${|
//@[028:00040) |   |   | ├─VariableAccessSyntax
//@[028:00040) |   |   | | └─IdentifierSyntax
//@[028:00040) |   |   | |   └─Token(Identifier) |magicString1|
//@[040:00042) |   |   | └─Token(StringRightPiece) |}'|
//@[042:00043) |   |   └─Token(RightSquare) |]|
//@[043:00045) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

// note: this should be fine, but we block string interpolation all together if there's a potential runtime property usage for name.
//@[132:00134) ├─Token(NewLine) |\r\n|
var magicString2 = 'name'
//@[000:00025) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00016) | ├─IdentifierSyntax
//@[004:00016) | | └─Token(Identifier) |magicString2|
//@[017:00018) | ├─Token(Assignment) |=|
//@[019:00025) | └─StringSyntax
//@[019:00025) |   └─Token(StringComplete) |'name'|
//@[025:00027) ├─Token(NewLine) |\r\n|
resource runtimeInvalidRes9 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[000:00139) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00027) | ├─IdentifierSyntax
//@[009:00027) | | └─Token(Identifier) |runtimeInvalidRes9|
//@[028:00087) | ├─StringSyntax
//@[028:00087) | | └─Token(StringComplete) |'Microsoft.Advisor/recommendations/suppressions@2020-01-01'|
//@[088:00089) | ├─Token(Assignment) |=|
//@[090:00139) | └─ObjectSyntax
//@[090:00091) |   ├─Token(LeftBrace) |{|
//@[091:00093) |   ├─Token(NewLine) |\r\n|
  name: runtimeValidRes2['${magicString2}']
//@[002:00043) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00043) |   | └─ArrayAccessSyntax
//@[008:00024) |   |   ├─VariableAccessSyntax
//@[008:00024) |   |   | └─IdentifierSyntax
//@[008:00024) |   |   |   └─Token(Identifier) |runtimeValidRes2|
//@[024:00025) |   |   ├─Token(LeftSquare) |[|
//@[025:00042) |   |   ├─StringSyntax
//@[025:00028) |   |   | ├─Token(StringLeftPiece) |'${|
//@[028:00040) |   |   | ├─VariableAccessSyntax
//@[028:00040) |   |   | | └─IdentifierSyntax
//@[028:00040) |   |   | |   └─Token(Identifier) |magicString2|
//@[040:00042) |   |   | └─Token(StringRightPiece) |}'|
//@[042:00043) |   |   └─Token(RightSquare) |]|
//@[043:00045) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

resource runtimeInvalidRes10 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[000:00135) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00028) | ├─IdentifierSyntax
//@[009:00028) | | └─Token(Identifier) |runtimeInvalidRes10|
//@[029:00088) | ├─StringSyntax
//@[029:00088) | | └─Token(StringComplete) |'Microsoft.Advisor/recommendations/suppressions@2020-01-01'|
//@[089:00090) | ├─Token(Assignment) |=|
//@[091:00135) | └─ObjectSyntax
//@[091:00092) |   ├─Token(LeftBrace) |{|
//@[092:00094) |   ├─Token(NewLine) |\r\n|
  name: '${runtimeValidRes3.location}'
//@[002:00038) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00038) |   | └─StringSyntax
//@[008:00011) |   |   ├─Token(StringLeftPiece) |'${|
//@[011:00036) |   |   ├─PropertyAccessSyntax
//@[011:00027) |   |   | ├─VariableAccessSyntax
//@[011:00027) |   |   | | └─IdentifierSyntax
//@[011:00027) |   |   | |   └─Token(Identifier) |runtimeValidRes3|
//@[027:00028) |   |   | ├─Token(Dot) |.|
//@[028:00036) |   |   | └─IdentifierSyntax
//@[028:00036) |   |   |   └─Token(Identifier) |location|
//@[036:00038) |   |   └─Token(StringRightPiece) |}'|
//@[038:00040) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

resource runtimeInvalidRes11 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[000:00131) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00028) | ├─IdentifierSyntax
//@[009:00028) | | └─Token(Identifier) |runtimeInvalidRes11|
//@[029:00088) | ├─StringSyntax
//@[029:00088) | | └─Token(StringComplete) |'Microsoft.Advisor/recommendations/suppressions@2020-01-01'|
//@[089:00090) | ├─Token(Assignment) |=|
//@[091:00131) | └─ObjectSyntax
//@[091:00092) |   ├─Token(LeftBrace) |{|
//@[092:00094) |   ├─Token(NewLine) |\r\n|
  name: validModule.params['name']
//@[002:00034) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00034) |   | └─ArrayAccessSyntax
//@[008:00026) |   |   ├─PropertyAccessSyntax
//@[008:00019) |   |   | ├─VariableAccessSyntax
//@[008:00019) |   |   | | └─IdentifierSyntax
//@[008:00019) |   |   | |   └─Token(Identifier) |validModule|
//@[019:00020) |   |   | ├─Token(Dot) |.|
//@[020:00026) |   |   | └─IdentifierSyntax
//@[020:00026) |   |   |   └─Token(Identifier) |params|
//@[026:00027) |   |   ├─Token(LeftSquare) |[|
//@[027:00033) |   |   ├─StringSyntax
//@[027:00033) |   |   | └─Token(StringComplete) |'name'|
//@[033:00034) |   |   └─Token(RightSquare) |]|
//@[034:00036) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

resource runtimeInvalidRes12 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[000:00240) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00028) | ├─IdentifierSyntax
//@[009:00028) | | └─Token(Identifier) |runtimeInvalidRes12|
//@[029:00088) | ├─StringSyntax
//@[029:00088) | | └─Token(StringComplete) |'Microsoft.Advisor/recommendations/suppressions@2020-01-01'|
//@[089:00090) | ├─Token(Assignment) |=|
//@[091:00240) | └─ObjectSyntax
//@[091:00092) |   ├─Token(LeftBrace) |{|
//@[092:00094) |   ├─Token(NewLine) |\r\n|
  name: concat(runtimeValidRes1.location, runtimeValidRes2['location'], runtimeInvalidRes3['properties'].azCliVersion, validModule.params.name)
//@[002:00143) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00143) |   | └─FunctionCallSyntax
//@[008:00014) |   |   ├─IdentifierSyntax
//@[008:00014) |   |   | └─Token(Identifier) |concat|
//@[014:00015) |   |   ├─Token(LeftParen) |(|
//@[015:00040) |   |   ├─FunctionArgumentSyntax
//@[015:00040) |   |   | └─PropertyAccessSyntax
//@[015:00031) |   |   |   ├─VariableAccessSyntax
//@[015:00031) |   |   |   | └─IdentifierSyntax
//@[015:00031) |   |   |   |   └─Token(Identifier) |runtimeValidRes1|
//@[031:00032) |   |   |   ├─Token(Dot) |.|
//@[032:00040) |   |   |   └─IdentifierSyntax
//@[032:00040) |   |   |     └─Token(Identifier) |location|
//@[040:00041) |   |   ├─Token(Comma) |,|
//@[042:00070) |   |   ├─FunctionArgumentSyntax
//@[042:00070) |   |   | └─ArrayAccessSyntax
//@[042:00058) |   |   |   ├─VariableAccessSyntax
//@[042:00058) |   |   |   | └─IdentifierSyntax
//@[042:00058) |   |   |   |   └─Token(Identifier) |runtimeValidRes2|
//@[058:00059) |   |   |   ├─Token(LeftSquare) |[|
//@[059:00069) |   |   |   ├─StringSyntax
//@[059:00069) |   |   |   | └─Token(StringComplete) |'location'|
//@[069:00070) |   |   |   └─Token(RightSquare) |]|
//@[070:00071) |   |   ├─Token(Comma) |,|
//@[072:00117) |   |   ├─FunctionArgumentSyntax
//@[072:00117) |   |   | └─PropertyAccessSyntax
//@[072:00104) |   |   |   ├─ArrayAccessSyntax
//@[072:00090) |   |   |   | ├─VariableAccessSyntax
//@[072:00090) |   |   |   | | └─IdentifierSyntax
//@[072:00090) |   |   |   | |   └─Token(Identifier) |runtimeInvalidRes3|
//@[090:00091) |   |   |   | ├─Token(LeftSquare) |[|
//@[091:00103) |   |   |   | ├─StringSyntax
//@[091:00103) |   |   |   | | └─Token(StringComplete) |'properties'|
//@[103:00104) |   |   |   | └─Token(RightSquare) |]|
//@[104:00105) |   |   |   ├─Token(Dot) |.|
//@[105:00117) |   |   |   └─IdentifierSyntax
//@[105:00117) |   |   |     └─Token(Identifier) |azCliVersion|
//@[117:00118) |   |   ├─Token(Comma) |,|
//@[119:00142) |   |   ├─FunctionArgumentSyntax
//@[119:00142) |   |   | └─PropertyAccessSyntax
//@[119:00137) |   |   |   ├─PropertyAccessSyntax
//@[119:00130) |   |   |   | ├─VariableAccessSyntax
//@[119:00130) |   |   |   | | └─IdentifierSyntax
//@[119:00130) |   |   |   | |   └─Token(Identifier) |validModule|
//@[130:00131) |   |   |   | ├─Token(Dot) |.|
//@[131:00137) |   |   |   | └─IdentifierSyntax
//@[131:00137) |   |   |   |   └─Token(Identifier) |params|
//@[137:00138) |   |   |   ├─Token(Dot) |.|
//@[138:00142) |   |   |   └─IdentifierSyntax
//@[138:00142) |   |   |     └─Token(Identifier) |name|
//@[142:00143) |   |   └─Token(RightParen) |)|
//@[143:00145) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

resource runtimeInvalidRes13 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[000:00243) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00028) | ├─IdentifierSyntax
//@[009:00028) | | └─Token(Identifier) |runtimeInvalidRes13|
//@[029:00088) | ├─StringSyntax
//@[029:00088) | | └─Token(StringComplete) |'Microsoft.Advisor/recommendations/suppressions@2020-01-01'|
//@[089:00090) | ├─Token(Assignment) |=|
//@[091:00243) | └─ObjectSyntax
//@[091:00092) |   ├─Token(LeftBrace) |{|
//@[092:00094) |   ├─Token(NewLine) |\r\n|
  name: '${runtimeValidRes1.location}${runtimeValidRes2['location']}${runtimeInvalidRes3.properties['azCliVersion']}${validModule['params'].name}'
//@[002:00146) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00146) |   | └─StringSyntax
//@[008:00011) |   |   ├─Token(StringLeftPiece) |'${|
//@[011:00036) |   |   ├─PropertyAccessSyntax
//@[011:00027) |   |   | ├─VariableAccessSyntax
//@[011:00027) |   |   | | └─IdentifierSyntax
//@[011:00027) |   |   | |   └─Token(Identifier) |runtimeValidRes1|
//@[027:00028) |   |   | ├─Token(Dot) |.|
//@[028:00036) |   |   | └─IdentifierSyntax
//@[028:00036) |   |   |   └─Token(Identifier) |location|
//@[036:00039) |   |   ├─Token(StringMiddlePiece) |}${|
//@[039:00067) |   |   ├─ArrayAccessSyntax
//@[039:00055) |   |   | ├─VariableAccessSyntax
//@[039:00055) |   |   | | └─IdentifierSyntax
//@[039:00055) |   |   | |   └─Token(Identifier) |runtimeValidRes2|
//@[055:00056) |   |   | ├─Token(LeftSquare) |[|
//@[056:00066) |   |   | ├─StringSyntax
//@[056:00066) |   |   | | └─Token(StringComplete) |'location'|
//@[066:00067) |   |   | └─Token(RightSquare) |]|
//@[067:00070) |   |   ├─Token(StringMiddlePiece) |}${|
//@[070:00115) |   |   ├─ArrayAccessSyntax
//@[070:00099) |   |   | ├─PropertyAccessSyntax
//@[070:00088) |   |   | | ├─VariableAccessSyntax
//@[070:00088) |   |   | | | └─IdentifierSyntax
//@[070:00088) |   |   | | |   └─Token(Identifier) |runtimeInvalidRes3|
//@[088:00089) |   |   | | ├─Token(Dot) |.|
//@[089:00099) |   |   | | └─IdentifierSyntax
//@[089:00099) |   |   | |   └─Token(Identifier) |properties|
//@[099:00100) |   |   | ├─Token(LeftSquare) |[|
//@[100:00114) |   |   | ├─StringSyntax
//@[100:00114) |   |   | | └─Token(StringComplete) |'azCliVersion'|
//@[114:00115) |   |   | └─Token(RightSquare) |]|
//@[115:00118) |   |   ├─Token(StringMiddlePiece) |}${|
//@[118:00144) |   |   ├─PropertyAccessSyntax
//@[118:00139) |   |   | ├─ArrayAccessSyntax
//@[118:00129) |   |   | | ├─VariableAccessSyntax
//@[118:00129) |   |   | | | └─IdentifierSyntax
//@[118:00129) |   |   | | |   └─Token(Identifier) |validModule|
//@[129:00130) |   |   | | ├─Token(LeftSquare) |[|
//@[130:00138) |   |   | | ├─StringSyntax
//@[130:00138) |   |   | | | └─Token(StringComplete) |'params'|
//@[138:00139) |   |   | | └─Token(RightSquare) |]|
//@[139:00140) |   |   | ├─Token(Dot) |.|
//@[140:00144) |   |   | └─IdentifierSyntax
//@[140:00144) |   |   |   └─Token(Identifier) |name|
//@[144:00146) |   |   └─Token(StringRightPiece) |}'|
//@[146:00148) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

// variable related runtime validation
//@[038:00040) ├─Token(NewLine) |\r\n|
var runtimefoo1 = runtimeValidRes1['location']
//@[000:00046) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00015) | ├─IdentifierSyntax
//@[004:00015) | | └─Token(Identifier) |runtimefoo1|
//@[016:00017) | ├─Token(Assignment) |=|
//@[018:00046) | └─ArrayAccessSyntax
//@[018:00034) |   ├─VariableAccessSyntax
//@[018:00034) |   | └─IdentifierSyntax
//@[018:00034) |   |   └─Token(Identifier) |runtimeValidRes1|
//@[034:00035) |   ├─Token(LeftSquare) |[|
//@[035:00045) |   ├─StringSyntax
//@[035:00045) |   | └─Token(StringComplete) |'location'|
//@[045:00046) |   └─Token(RightSquare) |]|
//@[046:00048) ├─Token(NewLine) |\r\n|
var runtimefoo2 = runtimeValidRes2['properties'].azCliVersion
//@[000:00061) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00015) | ├─IdentifierSyntax
//@[004:00015) | | └─Token(Identifier) |runtimefoo2|
//@[016:00017) | ├─Token(Assignment) |=|
//@[018:00061) | └─PropertyAccessSyntax
//@[018:00048) |   ├─ArrayAccessSyntax
//@[018:00034) |   | ├─VariableAccessSyntax
//@[018:00034) |   | | └─IdentifierSyntax
//@[018:00034) |   | |   └─Token(Identifier) |runtimeValidRes2|
//@[034:00035) |   | ├─Token(LeftSquare) |[|
//@[035:00047) |   | ├─StringSyntax
//@[035:00047) |   | | └─Token(StringComplete) |'properties'|
//@[047:00048) |   | └─Token(RightSquare) |]|
//@[048:00049) |   ├─Token(Dot) |.|
//@[049:00061) |   └─IdentifierSyntax
//@[049:00061) |     └─Token(Identifier) |azCliVersion|
//@[061:00063) ├─Token(NewLine) |\r\n|
var runtimefoo3 = runtimeValidRes2
//@[000:00034) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00015) | ├─IdentifierSyntax
//@[004:00015) | | └─Token(Identifier) |runtimefoo3|
//@[016:00017) | ├─Token(Assignment) |=|
//@[018:00034) | └─VariableAccessSyntax
//@[018:00034) |   └─IdentifierSyntax
//@[018:00034) |     └─Token(Identifier) |runtimeValidRes2|
//@[034:00036) ├─Token(NewLine) |\r\n|
var runtimefoo4 = {
//@[000:00042) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00015) | ├─IdentifierSyntax
//@[004:00015) | | └─Token(Identifier) |runtimefoo4|
//@[016:00017) | ├─Token(Assignment) |=|
//@[018:00042) | └─ObjectSyntax
//@[018:00019) |   ├─Token(LeftBrace) |{|
//@[019:00021) |   ├─Token(NewLine) |\r\n|
  hop: runtimefoo2
//@[002:00018) |   ├─ObjectPropertySyntax
//@[002:00005) |   | ├─IdentifierSyntax
//@[002:00005) |   | | └─Token(Identifier) |hop|
//@[005:00006) |   | ├─Token(Colon) |:|
//@[007:00018) |   | └─VariableAccessSyntax
//@[007:00018) |   |   └─IdentifierSyntax
//@[007:00018) |   |     └─Token(Identifier) |runtimefoo2|
//@[018:00020) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

var runtimeInvalid = {
//@[000:00119) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00018) | ├─IdentifierSyntax
//@[004:00018) | | └─Token(Identifier) |runtimeInvalid|
//@[019:00020) | ├─Token(Assignment) |=|
//@[021:00119) | └─ObjectSyntax
//@[021:00022) |   ├─Token(LeftBrace) |{|
//@[022:00024) |   ├─Token(NewLine) |\r\n|
  foo1: runtimefoo1
//@[002:00019) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |foo1|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00019) |   | └─VariableAccessSyntax
//@[008:00019) |   |   └─IdentifierSyntax
//@[008:00019) |   |     └─Token(Identifier) |runtimefoo1|
//@[019:00021) |   ├─Token(NewLine) |\r\n|
  foo2: runtimefoo2
//@[002:00019) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |foo2|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00019) |   | └─VariableAccessSyntax
//@[008:00019) |   |   └─IdentifierSyntax
//@[008:00019) |   |     └─Token(Identifier) |runtimefoo2|
//@[019:00021) |   ├─Token(NewLine) |\r\n|
  foo3: runtimefoo3
//@[002:00019) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |foo3|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00019) |   | └─VariableAccessSyntax
//@[008:00019) |   |   └─IdentifierSyntax
//@[008:00019) |   |     └─Token(Identifier) |runtimefoo3|
//@[019:00021) |   ├─Token(NewLine) |\r\n|
  foo4: runtimeValidRes1.name
//@[002:00029) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |foo4|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00029) |   | └─PropertyAccessSyntax
//@[008:00024) |   |   ├─VariableAccessSyntax
//@[008:00024) |   |   | └─IdentifierSyntax
//@[008:00024) |   |   |   └─Token(Identifier) |runtimeValidRes1|
//@[024:00025) |   |   ├─Token(Dot) |.|
//@[025:00029) |   |   └─IdentifierSyntax
//@[025:00029) |   |     └─Token(Identifier) |name|
//@[029:00031) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

var runtimeValid = {
//@[000:00151) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00016) | ├─IdentifierSyntax
//@[004:00016) | | └─Token(Identifier) |runtimeValid|
//@[017:00018) | ├─Token(Assignment) |=|
//@[019:00151) | └─ObjectSyntax
//@[019:00020) |   ├─Token(LeftBrace) |{|
//@[020:00022) |   ├─Token(NewLine) |\r\n|
  foo1: runtimeValidRes1.name
//@[002:00029) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |foo1|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00029) |   | └─PropertyAccessSyntax
//@[008:00024) |   |   ├─VariableAccessSyntax
//@[008:00024) |   |   | └─IdentifierSyntax
//@[008:00024) |   |   |   └─Token(Identifier) |runtimeValidRes1|
//@[024:00025) |   |   ├─Token(Dot) |.|
//@[025:00029) |   |   └─IdentifierSyntax
//@[025:00029) |   |     └─Token(Identifier) |name|
//@[029:00031) |   ├─Token(NewLine) |\r\n|
  foo2: runtimeValidRes1.apiVersion
//@[002:00035) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |foo2|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00035) |   | └─PropertyAccessSyntax
//@[008:00024) |   |   ├─VariableAccessSyntax
//@[008:00024) |   |   | └─IdentifierSyntax
//@[008:00024) |   |   |   └─Token(Identifier) |runtimeValidRes1|
//@[024:00025) |   |   ├─Token(Dot) |.|
//@[025:00035) |   |   └─IdentifierSyntax
//@[025:00035) |   |     └─Token(Identifier) |apiVersion|
//@[035:00037) |   ├─Token(NewLine) |\r\n|
  foo3: runtimeValidRes2.type
//@[002:00029) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |foo3|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00029) |   | └─PropertyAccessSyntax
//@[008:00024) |   |   ├─VariableAccessSyntax
//@[008:00024) |   |   | └─IdentifierSyntax
//@[008:00024) |   |   |   └─Token(Identifier) |runtimeValidRes2|
//@[024:00025) |   |   ├─Token(Dot) |.|
//@[025:00029) |   |   └─IdentifierSyntax
//@[025:00029) |   |     └─Token(Identifier) |type|
//@[029:00031) |   ├─Token(NewLine) |\r\n|
  foo4: runtimeValidRes2.id
//@[002:00027) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |foo4|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00027) |   | └─PropertyAccessSyntax
//@[008:00024) |   |   ├─VariableAccessSyntax
//@[008:00024) |   |   | └─IdentifierSyntax
//@[008:00024) |   |   |   └─Token(Identifier) |runtimeValidRes2|
//@[024:00025) |   |   ├─Token(Dot) |.|
//@[025:00027) |   |   └─IdentifierSyntax
//@[025:00027) |   |     └─Token(Identifier) |id|
//@[027:00029) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

resource runtimeInvalidRes14 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[000:00124) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00028) | ├─IdentifierSyntax
//@[009:00028) | | └─Token(Identifier) |runtimeInvalidRes14|
//@[029:00088) | ├─StringSyntax
//@[029:00088) | | └─Token(StringComplete) |'Microsoft.Advisor/recommendations/suppressions@2020-01-01'|
//@[089:00090) | ├─Token(Assignment) |=|
//@[091:00124) | └─ObjectSyntax
//@[091:00092) |   ├─Token(LeftBrace) |{|
//@[092:00094) |   ├─Token(NewLine) |\r\n|
  name: runtimeInvalid.foo1
//@[002:00027) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00027) |   | └─PropertyAccessSyntax
//@[008:00022) |   |   ├─VariableAccessSyntax
//@[008:00022) |   |   | └─IdentifierSyntax
//@[008:00022) |   |   |   └─Token(Identifier) |runtimeInvalid|
//@[022:00023) |   |   ├─Token(Dot) |.|
//@[023:00027) |   |   └─IdentifierSyntax
//@[023:00027) |   |     └─Token(Identifier) |foo1|
//@[027:00029) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

resource runtimeInvalidRes15 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[000:00124) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00028) | ├─IdentifierSyntax
//@[009:00028) | | └─Token(Identifier) |runtimeInvalidRes15|
//@[029:00088) | ├─StringSyntax
//@[029:00088) | | └─Token(StringComplete) |'Microsoft.Advisor/recommendations/suppressions@2020-01-01'|
//@[089:00090) | ├─Token(Assignment) |=|
//@[091:00124) | └─ObjectSyntax
//@[091:00092) |   ├─Token(LeftBrace) |{|
//@[092:00094) |   ├─Token(NewLine) |\r\n|
  name: runtimeInvalid.foo2
//@[002:00027) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00027) |   | └─PropertyAccessSyntax
//@[008:00022) |   |   ├─VariableAccessSyntax
//@[008:00022) |   |   | └─IdentifierSyntax
//@[008:00022) |   |   |   └─Token(Identifier) |runtimeInvalid|
//@[022:00023) |   |   ├─Token(Dot) |.|
//@[023:00027) |   |   └─IdentifierSyntax
//@[023:00027) |   |     └─Token(Identifier) |foo2|
//@[027:00029) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

resource runtimeInvalidRes16 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[000:00148) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00028) | ├─IdentifierSyntax
//@[009:00028) | | └─Token(Identifier) |runtimeInvalidRes16|
//@[029:00088) | ├─StringSyntax
//@[029:00088) | | └─Token(StringComplete) |'Microsoft.Advisor/recommendations/suppressions@2020-01-01'|
//@[089:00090) | ├─Token(Assignment) |=|
//@[091:00148) | └─ObjectSyntax
//@[091:00092) |   ├─Token(LeftBrace) |{|
//@[092:00094) |   ├─Token(NewLine) |\r\n|
  name: runtimeInvalid.foo3.properties.azCliVersion
//@[002:00051) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00051) |   | └─PropertyAccessSyntax
//@[008:00038) |   |   ├─PropertyAccessSyntax
//@[008:00027) |   |   | ├─PropertyAccessSyntax
//@[008:00022) |   |   | | ├─VariableAccessSyntax
//@[008:00022) |   |   | | | └─IdentifierSyntax
//@[008:00022) |   |   | | |   └─Token(Identifier) |runtimeInvalid|
//@[022:00023) |   |   | | ├─Token(Dot) |.|
//@[023:00027) |   |   | | └─IdentifierSyntax
//@[023:00027) |   |   | |   └─Token(Identifier) |foo3|
//@[027:00028) |   |   | ├─Token(Dot) |.|
//@[028:00038) |   |   | └─IdentifierSyntax
//@[028:00038) |   |   |   └─Token(Identifier) |properties|
//@[038:00039) |   |   ├─Token(Dot) |.|
//@[039:00051) |   |   └─IdentifierSyntax
//@[039:00051) |   |     └─Token(Identifier) |azCliVersion|
//@[051:00053) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

// Note: This is actually a runtime valid value. However, other properties of the variable cannot be resolved, so we block this.
//@[128:00130) ├─Token(NewLine) |\r\n|
resource runtimeInvalidRes17 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[000:00124) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00028) | ├─IdentifierSyntax
//@[009:00028) | | └─Token(Identifier) |runtimeInvalidRes17|
//@[029:00088) | ├─StringSyntax
//@[029:00088) | | └─Token(StringComplete) |'Microsoft.Advisor/recommendations/suppressions@2020-01-01'|
//@[089:00090) | ├─Token(Assignment) |=|
//@[091:00124) | └─ObjectSyntax
//@[091:00092) |   ├─Token(LeftBrace) |{|
//@[092:00094) |   ├─Token(NewLine) |\r\n|
  name: runtimeInvalid.foo4
//@[002:00027) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00027) |   | └─PropertyAccessSyntax
//@[008:00022) |   |   ├─VariableAccessSyntax
//@[008:00022) |   |   | └─IdentifierSyntax
//@[008:00022) |   |   |   └─Token(Identifier) |runtimeInvalid|
//@[022:00023) |   |   ├─Token(Dot) |.|
//@[023:00027) |   |   └─IdentifierSyntax
//@[023:00027) |   |     └─Token(Identifier) |foo4|
//@[027:00029) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

resource runtimeInvalidRes18 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[000:00226) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00028) | ├─IdentifierSyntax
//@[009:00028) | | └─Token(Identifier) |runtimeInvalidRes18|
//@[029:00088) | ├─StringSyntax
//@[029:00088) | | └─Token(StringComplete) |'Microsoft.Advisor/recommendations/suppressions@2020-01-01'|
//@[089:00090) | ├─Token(Assignment) |=|
//@[091:00226) | └─ObjectSyntax
//@[091:00092) |   ├─Token(LeftBrace) |{|
//@[092:00094) |   ├─Token(NewLine) |\r\n|
  name: concat(runtimeInvalid.foo1, runtimeValidRes2['properties'].azCliVersion, '${runtimeValidRes1.location}', runtimefoo4.hop)
//@[002:00129) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00129) |   | └─FunctionCallSyntax
//@[008:00014) |   |   ├─IdentifierSyntax
//@[008:00014) |   |   | └─Token(Identifier) |concat|
//@[014:00015) |   |   ├─Token(LeftParen) |(|
//@[015:00034) |   |   ├─FunctionArgumentSyntax
//@[015:00034) |   |   | └─PropertyAccessSyntax
//@[015:00029) |   |   |   ├─VariableAccessSyntax
//@[015:00029) |   |   |   | └─IdentifierSyntax
//@[015:00029) |   |   |   |   └─Token(Identifier) |runtimeInvalid|
//@[029:00030) |   |   |   ├─Token(Dot) |.|
//@[030:00034) |   |   |   └─IdentifierSyntax
//@[030:00034) |   |   |     └─Token(Identifier) |foo1|
//@[034:00035) |   |   ├─Token(Comma) |,|
//@[036:00079) |   |   ├─FunctionArgumentSyntax
//@[036:00079) |   |   | └─PropertyAccessSyntax
//@[036:00066) |   |   |   ├─ArrayAccessSyntax
//@[036:00052) |   |   |   | ├─VariableAccessSyntax
//@[036:00052) |   |   |   | | └─IdentifierSyntax
//@[036:00052) |   |   |   | |   └─Token(Identifier) |runtimeValidRes2|
//@[052:00053) |   |   |   | ├─Token(LeftSquare) |[|
//@[053:00065) |   |   |   | ├─StringSyntax
//@[053:00065) |   |   |   | | └─Token(StringComplete) |'properties'|
//@[065:00066) |   |   |   | └─Token(RightSquare) |]|
//@[066:00067) |   |   |   ├─Token(Dot) |.|
//@[067:00079) |   |   |   └─IdentifierSyntax
//@[067:00079) |   |   |     └─Token(Identifier) |azCliVersion|
//@[079:00080) |   |   ├─Token(Comma) |,|
//@[081:00111) |   |   ├─FunctionArgumentSyntax
//@[081:00111) |   |   | └─StringSyntax
//@[081:00084) |   |   |   ├─Token(StringLeftPiece) |'${|
//@[084:00109) |   |   |   ├─PropertyAccessSyntax
//@[084:00100) |   |   |   | ├─VariableAccessSyntax
//@[084:00100) |   |   |   | | └─IdentifierSyntax
//@[084:00100) |   |   |   | |   └─Token(Identifier) |runtimeValidRes1|
//@[100:00101) |   |   |   | ├─Token(Dot) |.|
//@[101:00109) |   |   |   | └─IdentifierSyntax
//@[101:00109) |   |   |   |   └─Token(Identifier) |location|
//@[109:00111) |   |   |   └─Token(StringRightPiece) |}'|
//@[111:00112) |   |   ├─Token(Comma) |,|
//@[113:00128) |   |   ├─FunctionArgumentSyntax
//@[113:00128) |   |   | └─PropertyAccessSyntax
//@[113:00124) |   |   |   ├─VariableAccessSyntax
//@[113:00124) |   |   |   | └─IdentifierSyntax
//@[113:00124) |   |   |   |   └─Token(Identifier) |runtimefoo4|
//@[124:00125) |   |   |   ├─Token(Dot) |.|
//@[125:00128) |   |   |   └─IdentifierSyntax
//@[125:00128) |   |   |     └─Token(Identifier) |hop|
//@[128:00129) |   |   └─Token(RightParen) |)|
//@[129:00131) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

resource runtimeValidRes6 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[000:00119) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00025) | ├─IdentifierSyntax
//@[009:00025) | | └─Token(Identifier) |runtimeValidRes6|
//@[026:00085) | ├─StringSyntax
//@[026:00085) | | └─Token(StringComplete) |'Microsoft.Advisor/recommendations/suppressions@2020-01-01'|
//@[086:00087) | ├─Token(Assignment) |=|
//@[088:00119) | └─ObjectSyntax
//@[088:00089) |   ├─Token(LeftBrace) |{|
//@[089:00091) |   ├─Token(NewLine) |\r\n|
  name: runtimeValid.foo1
//@[002:00025) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00025) |   | └─PropertyAccessSyntax
//@[008:00020) |   |   ├─VariableAccessSyntax
//@[008:00020) |   |   | └─IdentifierSyntax
//@[008:00020) |   |   |   └─Token(Identifier) |runtimeValid|
//@[020:00021) |   |   ├─Token(Dot) |.|
//@[021:00025) |   |   └─IdentifierSyntax
//@[021:00025) |   |     └─Token(Identifier) |foo1|
//@[025:00027) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

resource runtimeValidRes7 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[000:00119) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00025) | ├─IdentifierSyntax
//@[009:00025) | | └─Token(Identifier) |runtimeValidRes7|
//@[026:00085) | ├─StringSyntax
//@[026:00085) | | └─Token(StringComplete) |'Microsoft.Advisor/recommendations/suppressions@2020-01-01'|
//@[086:00087) | ├─Token(Assignment) |=|
//@[088:00119) | └─ObjectSyntax
//@[088:00089) |   ├─Token(LeftBrace) |{|
//@[089:00091) |   ├─Token(NewLine) |\r\n|
  name: runtimeValid.foo2
//@[002:00025) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00025) |   | └─PropertyAccessSyntax
//@[008:00020) |   |   ├─VariableAccessSyntax
//@[008:00020) |   |   | └─IdentifierSyntax
//@[008:00020) |   |   |   └─Token(Identifier) |runtimeValid|
//@[020:00021) |   |   ├─Token(Dot) |.|
//@[021:00025) |   |   └─IdentifierSyntax
//@[021:00025) |   |     └─Token(Identifier) |foo2|
//@[025:00027) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

resource runtimeValidRes8 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[000:00119) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00025) | ├─IdentifierSyntax
//@[009:00025) | | └─Token(Identifier) |runtimeValidRes8|
//@[026:00085) | ├─StringSyntax
//@[026:00085) | | └─Token(StringComplete) |'Microsoft.Advisor/recommendations/suppressions@2020-01-01'|
//@[086:00087) | ├─Token(Assignment) |=|
//@[088:00119) | └─ObjectSyntax
//@[088:00089) |   ├─Token(LeftBrace) |{|
//@[089:00091) |   ├─Token(NewLine) |\r\n|
  name: runtimeValid.foo3
//@[002:00025) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00025) |   | └─PropertyAccessSyntax
//@[008:00020) |   |   ├─VariableAccessSyntax
//@[008:00020) |   |   | └─IdentifierSyntax
//@[008:00020) |   |   |   └─Token(Identifier) |runtimeValid|
//@[020:00021) |   |   ├─Token(Dot) |.|
//@[021:00025) |   |   └─IdentifierSyntax
//@[021:00025) |   |     └─Token(Identifier) |foo3|
//@[025:00027) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

resource runtimeValidRes9 'Microsoft.Advisor/recommendations/suppressions@2020-01-01' = {
//@[000:00119) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00025) | ├─IdentifierSyntax
//@[009:00025) | | └─Token(Identifier) |runtimeValidRes9|
//@[026:00085) | ├─StringSyntax
//@[026:00085) | | └─Token(StringComplete) |'Microsoft.Advisor/recommendations/suppressions@2020-01-01'|
//@[086:00087) | ├─Token(Assignment) |=|
//@[088:00119) | └─ObjectSyntax
//@[088:00089) |   ├─Token(LeftBrace) |{|
//@[089:00091) |   ├─Token(NewLine) |\r\n|
  name: runtimeValid.foo4
//@[002:00025) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00025) |   | └─PropertyAccessSyntax
//@[008:00020) |   |   ├─VariableAccessSyntax
//@[008:00020) |   |   | └─IdentifierSyntax
//@[008:00020) |   |   |   └─Token(Identifier) |runtimeValid|
//@[020:00021) |   |   ├─Token(Dot) |.|
//@[021:00025) |   |   └─IdentifierSyntax
//@[021:00025) |   |     └─Token(Identifier) |foo4|
//@[025:00027) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00007) ├─Token(NewLine) |\r\n\r\n\r\n|


resource loopForRuntimeCheck 'Microsoft.Network/dnsZones@2018-05-01' = [for thing in []: {
//@[000:00130) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00028) | ├─IdentifierSyntax
//@[009:00028) | | └─Token(Identifier) |loopForRuntimeCheck|
//@[029:00068) | ├─StringSyntax
//@[029:00068) | | └─Token(StringComplete) |'Microsoft.Network/dnsZones@2018-05-01'|
//@[069:00070) | ├─Token(Assignment) |=|
//@[071:00130) | └─ForSyntax
//@[071:00072) |   ├─Token(LeftSquare) |[|
//@[072:00075) |   ├─Token(Identifier) |for|
//@[076:00081) |   ├─LocalVariableSyntax
//@[076:00081) |   | └─IdentifierSyntax
//@[076:00081) |   |   └─Token(Identifier) |thing|
//@[082:00084) |   ├─Token(Identifier) |in|
//@[085:00087) |   ├─ArraySyntax
//@[085:00086) |   | ├─Token(LeftSquare) |[|
//@[086:00087) |   | └─Token(RightSquare) |]|
//@[087:00088) |   ├─Token(Colon) |:|
//@[089:00129) |   ├─ObjectSyntax
//@[089:00090) |   | ├─Token(LeftBrace) |{|
//@[090:00092) |   | ├─Token(NewLine) |\r\n|
  name: 'test'
//@[002:00014) |   | ├─ObjectPropertySyntax
//@[002:00006) |   | | ├─IdentifierSyntax
//@[002:00006) |   | | | └─Token(Identifier) |name|
//@[006:00007) |   | | ├─Token(Colon) |:|
//@[008:00014) |   | | └─StringSyntax
//@[008:00014) |   | |   └─Token(StringComplete) |'test'|
//@[014:00016) |   | ├─Token(NewLine) |\r\n|
  location: 'test'
//@[002:00018) |   | ├─ObjectPropertySyntax
//@[002:00010) |   | | ├─IdentifierSyntax
//@[002:00010) |   | | | └─Token(Identifier) |location|
//@[010:00011) |   | | ├─Token(Colon) |:|
//@[012:00018) |   | | └─StringSyntax
//@[012:00018) |   | |   └─Token(StringComplete) |'test'|
//@[018:00020) |   | ├─Token(NewLine) |\r\n|
}]
//@[000:00001) |   | └─Token(RightBrace) |}|
//@[001:00002) |   └─Token(RightSquare) |]|
//@[002:00006) ├─Token(NewLine) |\r\n\r\n|

var runtimeCheckVar = loopForRuntimeCheck[0].properties.zoneType
//@[000:00064) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00019) | ├─IdentifierSyntax
//@[004:00019) | | └─Token(Identifier) |runtimeCheckVar|
//@[020:00021) | ├─Token(Assignment) |=|
//@[022:00064) | └─PropertyAccessSyntax
//@[022:00055) |   ├─PropertyAccessSyntax
//@[022:00044) |   | ├─ArrayAccessSyntax
//@[022:00041) |   | | ├─VariableAccessSyntax
//@[022:00041) |   | | | └─IdentifierSyntax
//@[022:00041) |   | | |   └─Token(Identifier) |loopForRuntimeCheck|
//@[041:00042) |   | | ├─Token(LeftSquare) |[|
//@[042:00043) |   | | ├─IntegerLiteralSyntax
//@[042:00043) |   | | | └─Token(Integer) |0|
//@[043:00044) |   | | └─Token(RightSquare) |]|
//@[044:00045) |   | ├─Token(Dot) |.|
//@[045:00055) |   | └─IdentifierSyntax
//@[045:00055) |   |   └─Token(Identifier) |properties|
//@[055:00056) |   ├─Token(Dot) |.|
//@[056:00064) |   └─IdentifierSyntax
//@[056:00064) |     └─Token(Identifier) |zoneType|
//@[064:00066) ├─Token(NewLine) |\r\n|
var runtimeCheckVar2 = runtimeCheckVar
//@[000:00038) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00020) | ├─IdentifierSyntax
//@[004:00020) | | └─Token(Identifier) |runtimeCheckVar2|
//@[021:00022) | ├─Token(Assignment) |=|
//@[023:00038) | └─VariableAccessSyntax
//@[023:00038) |   └─IdentifierSyntax
//@[023:00038) |     └─Token(Identifier) |runtimeCheckVar|
//@[038:00042) ├─Token(NewLine) |\r\n\r\n|

resource singleResourceForRuntimeCheck 'Microsoft.Network/dnsZones@2018-05-01' = {
//@[000:00131) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00038) | ├─IdentifierSyntax
//@[009:00038) | | └─Token(Identifier) |singleResourceForRuntimeCheck|
//@[039:00078) | ├─StringSyntax
//@[039:00078) | | └─Token(StringComplete) |'Microsoft.Network/dnsZones@2018-05-01'|
//@[079:00080) | ├─Token(Assignment) |=|
//@[081:00131) | └─ObjectSyntax
//@[081:00082) |   ├─Token(LeftBrace) |{|
//@[082:00084) |   ├─Token(NewLine) |\r\n|
  name: runtimeCheckVar2
//@[002:00024) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00024) |   | └─VariableAccessSyntax
//@[008:00024) |   |   └─IdentifierSyntax
//@[008:00024) |   |     └─Token(Identifier) |runtimeCheckVar2|
//@[024:00026) |   ├─Token(NewLine) |\r\n|
  location: 'test'
//@[002:00018) |   ├─ObjectPropertySyntax
//@[002:00010) |   | ├─IdentifierSyntax
//@[002:00010) |   | | └─Token(Identifier) |location|
//@[010:00011) |   | ├─Token(Colon) |:|
//@[012:00018) |   | └─StringSyntax
//@[012:00018) |   |   └─Token(StringComplete) |'test'|
//@[018:00020) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

resource loopForRuntimeCheck2 'Microsoft.Network/dnsZones@2018-05-01' = [for thing in []: {
//@[000:00141) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00029) | ├─IdentifierSyntax
//@[009:00029) | | └─Token(Identifier) |loopForRuntimeCheck2|
//@[030:00069) | ├─StringSyntax
//@[030:00069) | | └─Token(StringComplete) |'Microsoft.Network/dnsZones@2018-05-01'|
//@[070:00071) | ├─Token(Assignment) |=|
//@[072:00141) | └─ForSyntax
//@[072:00073) |   ├─Token(LeftSquare) |[|
//@[073:00076) |   ├─Token(Identifier) |for|
//@[077:00082) |   ├─LocalVariableSyntax
//@[077:00082) |   | └─IdentifierSyntax
//@[077:00082) |   |   └─Token(Identifier) |thing|
//@[083:00085) |   ├─Token(Identifier) |in|
//@[086:00088) |   ├─ArraySyntax
//@[086:00087) |   | ├─Token(LeftSquare) |[|
//@[087:00088) |   | └─Token(RightSquare) |]|
//@[088:00089) |   ├─Token(Colon) |:|
//@[090:00140) |   ├─ObjectSyntax
//@[090:00091) |   | ├─Token(LeftBrace) |{|
//@[091:00093) |   | ├─Token(NewLine) |\r\n|
  name: runtimeCheckVar2
//@[002:00024) |   | ├─ObjectPropertySyntax
//@[002:00006) |   | | ├─IdentifierSyntax
//@[002:00006) |   | | | └─Token(Identifier) |name|
//@[006:00007) |   | | ├─Token(Colon) |:|
//@[008:00024) |   | | └─VariableAccessSyntax
//@[008:00024) |   | |   └─IdentifierSyntax
//@[008:00024) |   | |     └─Token(Identifier) |runtimeCheckVar2|
//@[024:00026) |   | ├─Token(NewLine) |\r\n|
  location: 'test'
//@[002:00018) |   | ├─ObjectPropertySyntax
//@[002:00010) |   | | ├─IdentifierSyntax
//@[002:00010) |   | | | └─Token(Identifier) |location|
//@[010:00011) |   | | ├─Token(Colon) |:|
//@[012:00018) |   | | └─StringSyntax
//@[012:00018) |   | |   └─Token(StringComplete) |'test'|
//@[018:00020) |   | ├─Token(NewLine) |\r\n|
}]
//@[000:00001) |   | └─Token(RightBrace) |}|
//@[001:00002) |   └─Token(RightSquare) |]|
//@[002:00006) ├─Token(NewLine) |\r\n\r\n|

resource loopForRuntimeCheck3 'Microsoft.Network/dnsZones@2018-05-01' = [for otherThing in []: {
//@[000:00172) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00029) | ├─IdentifierSyntax
//@[009:00029) | | └─Token(Identifier) |loopForRuntimeCheck3|
//@[030:00069) | ├─StringSyntax
//@[030:00069) | | └─Token(StringComplete) |'Microsoft.Network/dnsZones@2018-05-01'|
//@[070:00071) | ├─Token(Assignment) |=|
//@[072:00172) | └─ForSyntax
//@[072:00073) |   ├─Token(LeftSquare) |[|
//@[073:00076) |   ├─Token(Identifier) |for|
//@[077:00087) |   ├─LocalVariableSyntax
//@[077:00087) |   | └─IdentifierSyntax
//@[077:00087) |   |   └─Token(Identifier) |otherThing|
//@[088:00090) |   ├─Token(Identifier) |in|
//@[091:00093) |   ├─ArraySyntax
//@[091:00092) |   | ├─Token(LeftSquare) |[|
//@[092:00093) |   | └─Token(RightSquare) |]|
//@[093:00094) |   ├─Token(Colon) |:|
//@[095:00171) |   ├─ObjectSyntax
//@[095:00096) |   | ├─Token(LeftBrace) |{|
//@[096:00098) |   | ├─Token(NewLine) |\r\n|
  name: loopForRuntimeCheck[0].properties.zoneType
//@[002:00050) |   | ├─ObjectPropertySyntax
//@[002:00006) |   | | ├─IdentifierSyntax
//@[002:00006) |   | | | └─Token(Identifier) |name|
//@[006:00007) |   | | ├─Token(Colon) |:|
//@[008:00050) |   | | └─PropertyAccessSyntax
//@[008:00041) |   | |   ├─PropertyAccessSyntax
//@[008:00030) |   | |   | ├─ArrayAccessSyntax
//@[008:00027) |   | |   | | ├─VariableAccessSyntax
//@[008:00027) |   | |   | | | └─IdentifierSyntax
//@[008:00027) |   | |   | | |   └─Token(Identifier) |loopForRuntimeCheck|
//@[027:00028) |   | |   | | ├─Token(LeftSquare) |[|
//@[028:00029) |   | |   | | ├─IntegerLiteralSyntax
//@[028:00029) |   | |   | | | └─Token(Integer) |0|
//@[029:00030) |   | |   | | └─Token(RightSquare) |]|
//@[030:00031) |   | |   | ├─Token(Dot) |.|
//@[031:00041) |   | |   | └─IdentifierSyntax
//@[031:00041) |   | |   |   └─Token(Identifier) |properties|
//@[041:00042) |   | |   ├─Token(Dot) |.|
//@[042:00050) |   | |   └─IdentifierSyntax
//@[042:00050) |   | |     └─Token(Identifier) |zoneType|
//@[050:00052) |   | ├─Token(NewLine) |\r\n|
  location: 'test'
//@[002:00018) |   | ├─ObjectPropertySyntax
//@[002:00010) |   | | ├─IdentifierSyntax
//@[002:00010) |   | | | └─Token(Identifier) |location|
//@[010:00011) |   | | ├─Token(Colon) |:|
//@[012:00018) |   | | └─StringSyntax
//@[012:00018) |   | |   └─Token(StringComplete) |'test'|
//@[018:00020) |   | ├─Token(NewLine) |\r\n|
}]
//@[000:00001) |   | └─Token(RightBrace) |}|
//@[001:00002) |   └─Token(RightSquare) |]|
//@[002:00006) ├─Token(NewLine) |\r\n\r\n|

var varForRuntimeCheck4a = loopForRuntimeCheck[0].properties.zoneType
//@[000:00069) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00024) | ├─IdentifierSyntax
//@[004:00024) | | └─Token(Identifier) |varForRuntimeCheck4a|
//@[025:00026) | ├─Token(Assignment) |=|
//@[027:00069) | └─PropertyAccessSyntax
//@[027:00060) |   ├─PropertyAccessSyntax
//@[027:00049) |   | ├─ArrayAccessSyntax
//@[027:00046) |   | | ├─VariableAccessSyntax
//@[027:00046) |   | | | └─IdentifierSyntax
//@[027:00046) |   | | |   └─Token(Identifier) |loopForRuntimeCheck|
//@[046:00047) |   | | ├─Token(LeftSquare) |[|
//@[047:00048) |   | | ├─IntegerLiteralSyntax
//@[047:00048) |   | | | └─Token(Integer) |0|
//@[048:00049) |   | | └─Token(RightSquare) |]|
//@[049:00050) |   | ├─Token(Dot) |.|
//@[050:00060) |   | └─IdentifierSyntax
//@[050:00060) |   |   └─Token(Identifier) |properties|
//@[060:00061) |   ├─Token(Dot) |.|
//@[061:00069) |   └─IdentifierSyntax
//@[061:00069) |     └─Token(Identifier) |zoneType|
//@[069:00071) ├─Token(NewLine) |\r\n|
var varForRuntimeCheck4b = varForRuntimeCheck4a
//@[000:00047) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00024) | ├─IdentifierSyntax
//@[004:00024) | | └─Token(Identifier) |varForRuntimeCheck4b|
//@[025:00026) | ├─Token(Assignment) |=|
//@[027:00047) | └─VariableAccessSyntax
//@[027:00047) |   └─IdentifierSyntax
//@[027:00047) |     └─Token(Identifier) |varForRuntimeCheck4a|
//@[047:00049) ├─Token(NewLine) |\r\n|
resource loopForRuntimeCheck4 'Microsoft.Network/dnsZones@2018-05-01' = [for otherThing in []: {
//@[000:00150) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00029) | ├─IdentifierSyntax
//@[009:00029) | | └─Token(Identifier) |loopForRuntimeCheck4|
//@[030:00069) | ├─StringSyntax
//@[030:00069) | | └─Token(StringComplete) |'Microsoft.Network/dnsZones@2018-05-01'|
//@[070:00071) | ├─Token(Assignment) |=|
//@[072:00150) | └─ForSyntax
//@[072:00073) |   ├─Token(LeftSquare) |[|
//@[073:00076) |   ├─Token(Identifier) |for|
//@[077:00087) |   ├─LocalVariableSyntax
//@[077:00087) |   | └─IdentifierSyntax
//@[077:00087) |   |   └─Token(Identifier) |otherThing|
//@[088:00090) |   ├─Token(Identifier) |in|
//@[091:00093) |   ├─ArraySyntax
//@[091:00092) |   | ├─Token(LeftSquare) |[|
//@[092:00093) |   | └─Token(RightSquare) |]|
//@[093:00094) |   ├─Token(Colon) |:|
//@[095:00149) |   ├─ObjectSyntax
//@[095:00096) |   | ├─Token(LeftBrace) |{|
//@[096:00098) |   | ├─Token(NewLine) |\r\n|
  name: varForRuntimeCheck4b
//@[002:00028) |   | ├─ObjectPropertySyntax
//@[002:00006) |   | | ├─IdentifierSyntax
//@[002:00006) |   | | | └─Token(Identifier) |name|
//@[006:00007) |   | | ├─Token(Colon) |:|
//@[008:00028) |   | | └─VariableAccessSyntax
//@[008:00028) |   | |   └─IdentifierSyntax
//@[008:00028) |   | |     └─Token(Identifier) |varForRuntimeCheck4b|
//@[028:00030) |   | ├─Token(NewLine) |\r\n|
  location: 'test'
//@[002:00018) |   | ├─ObjectPropertySyntax
//@[002:00010) |   | | ├─IdentifierSyntax
//@[002:00010) |   | | | └─Token(Identifier) |location|
//@[010:00011) |   | | ├─Token(Colon) |:|
//@[012:00018) |   | | └─StringSyntax
//@[012:00018) |   | |   └─Token(StringComplete) |'test'|
//@[018:00020) |   | ├─Token(NewLine) |\r\n|
}]
//@[000:00001) |   | └─Token(RightBrace) |}|
//@[001:00002) |   └─Token(RightSquare) |]|
//@[002:00006) ├─Token(NewLine) |\r\n\r\n|

resource missingTopLevelProperties 'Microsoft.Storage/storageAccounts@2020-08-01-preview' = {
//@[000:00153) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00034) | ├─IdentifierSyntax
//@[009:00034) | | └─Token(Identifier) |missingTopLevelProperties|
//@[035:00089) | ├─StringSyntax
//@[035:00089) | | └─Token(StringComplete) |'Microsoft.Storage/storageAccounts@2020-08-01-preview'|
//@[090:00091) | ├─Token(Assignment) |=|
//@[092:00153) | └─ObjectSyntax
//@[092:00093) |   ├─Token(LeftBrace) |{|
//@[093:00095) |   ├─Token(NewLine) |\r\n|
  // #completionTest(0, 1, 2) -> topLevelProperties
//@[051:00053) |   ├─Token(NewLine) |\r\n|
  
//@[002:00004) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

resource missingTopLevelPropertiesExceptName 'Microsoft.Storage/storageAccounts@2020-08-01-preview' = {
//@[000:00305) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00044) | ├─IdentifierSyntax
//@[009:00044) | | └─Token(Identifier) |missingTopLevelPropertiesExceptName|
//@[045:00099) | ├─StringSyntax
//@[045:00099) | | └─Token(StringComplete) |'Microsoft.Storage/storageAccounts@2020-08-01-preview'|
//@[100:00101) | ├─Token(Assignment) |=|
//@[102:00305) | └─ObjectSyntax
//@[102:00103) |   ├─Token(LeftBrace) |{|
//@[103:00105) |   ├─Token(NewLine) |\r\n|
  // #completionTest(2) -> topLevelPropertiesMinusNameNoColon
//@[061:00063) |   ├─Token(NewLine) |\r\n|
  name: 'me'
//@[002:00012) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00012) |   | └─StringSyntax
//@[008:00012) |   |   └─Token(StringComplete) |'me'|
//@[012:00014) |   ├─Token(NewLine) |\r\n|
  // do not remove whitespace before the closing curly
//@[054:00056) |   ├─Token(NewLine) |\r\n|
  // #completionTest(0, 1, 2) -> topLevelPropertiesMinusName
//@[060:00062) |   ├─Token(NewLine) |\r\n|
  
//@[002:00004) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

// #completionTest(24,25,26,49,65,69,70) -> virtualNetworksResourceTypes
//@[072:00074) ├─Token(NewLine) |\r\n|
resource unfinishedVnet 'Microsoft.Network/virtualNetworks@2020-06-01' = {
//@[000:00531) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00023) | ├─IdentifierSyntax
//@[009:00023) | | └─Token(Identifier) |unfinishedVnet|
//@[024:00070) | ├─StringSyntax
//@[024:00070) | | └─Token(StringComplete) |'Microsoft.Network/virtualNetworks@2020-06-01'|
//@[071:00072) | ├─Token(Assignment) |=|
//@[073:00531) | └─ObjectSyntax
//@[073:00074) |   ├─Token(LeftBrace) |{|
//@[074:00076) |   ├─Token(NewLine) |\r\n|
  name: 'v'
//@[002:00011) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00011) |   | └─StringSyntax
//@[008:00011) |   |   └─Token(StringComplete) |'v'|
//@[011:00013) |   ├─Token(NewLine) |\r\n|
  location: 'eastus'
//@[002:00020) |   ├─ObjectPropertySyntax
//@[002:00010) |   | ├─IdentifierSyntax
//@[002:00010) |   | | └─Token(Identifier) |location|
//@[010:00011) |   | ├─Token(Colon) |:|
//@[012:00020) |   | └─StringSyntax
//@[012:00020) |   |   └─Token(StringComplete) |'eastus'|
//@[020:00022) |   ├─Token(NewLine) |\r\n|
  properties: {
//@[002:00417) |   ├─ObjectPropertySyntax
//@[002:00012) |   | ├─IdentifierSyntax
//@[002:00012) |   | | └─Token(Identifier) |properties|
//@[012:00013) |   | ├─Token(Colon) |:|
//@[014:00417) |   | └─ObjectSyntax
//@[014:00015) |   |   ├─Token(LeftBrace) |{|
//@[015:00017) |   |   ├─Token(NewLine) |\r\n|
    subnets: [
//@[004:00395) |   |   ├─ObjectPropertySyntax
//@[004:00011) |   |   | ├─IdentifierSyntax
//@[004:00011) |   |   | | └─Token(Identifier) |subnets|
//@[011:00012) |   |   | ├─Token(Colon) |:|
//@[013:00395) |   |   | └─ArraySyntax
//@[013:00014) |   |   |   ├─Token(LeftSquare) |[|
//@[014:00016) |   |   |   ├─Token(NewLine) |\r\n|
      {
//@[006:00372) |   |   |   ├─ArrayItemSyntax
//@[006:00372) |   |   |   | └─ObjectSyntax
//@[006:00007) |   |   |   |   ├─Token(LeftBrace) |{|
//@[007:00009) |   |   |   |   ├─Token(NewLine) |\r\n|
        // #completionTest(0,1,2,3,4,5,6,7) -> subnetPropertiesMinusProperties
//@[078:00080) |   |   |   |   ├─Token(NewLine) |\r\n|
       
//@[007:00009) |   |   |   |   ├─Token(NewLine) |\r\n|
        // #completionTest(0,1,2,3,4,5,6,7) -> empty
//@[052:00054) |   |   |   |   ├─Token(NewLine) |\r\n|
        properties: {
//@[008:00211) |   |   |   |   ├─ObjectPropertySyntax
//@[008:00018) |   |   |   |   | ├─IdentifierSyntax
//@[008:00018) |   |   |   |   | | └─Token(Identifier) |properties|
//@[018:00019) |   |   |   |   | ├─Token(Colon) |:|
//@[020:00211) |   |   |   |   | └─ObjectSyntax
//@[020:00021) |   |   |   |   |   ├─Token(LeftBrace) |{|
//@[021:00023) |   |   |   |   |   ├─Token(NewLine) |\r\n|
          delegations: [
//@[010:00177) |   |   |   |   |   ├─ObjectPropertySyntax
//@[010:00021) |   |   |   |   |   | ├─IdentifierSyntax
//@[010:00021) |   |   |   |   |   | | └─Token(Identifier) |delegations|
//@[021:00022) |   |   |   |   |   | ├─Token(Colon) |:|
//@[023:00177) |   |   |   |   |   | └─ArraySyntax
//@[023:00024) |   |   |   |   |   |   ├─Token(LeftSquare) |[|
//@[024:00026) |   |   |   |   |   |   ├─Token(NewLine) |\r\n|
            {
//@[012:00138) |   |   |   |   |   |   ├─ArrayItemSyntax
//@[012:00138) |   |   |   |   |   |   | └─ObjectSyntax
//@[012:00013) |   |   |   |   |   |   |   ├─Token(LeftBrace) |{|
//@[013:00015) |   |   |   |   |   |   |   ├─Token(NewLine) |\r\n|
              // #completionTest(0,1,2,3,4,5,6,7,8,9,10,11,12,13,14) -> delegationProperties
//@[092:00094) |   |   |   |   |   |   |   ├─Token(NewLine) |\r\n|
              
//@[014:00016) |   |   |   |   |   |   |   ├─Token(NewLine) |\r\n|
            }
//@[012:00013) |   |   |   |   |   |   |   └─Token(RightBrace) |}|
//@[013:00015) |   |   |   |   |   |   ├─Token(NewLine) |\r\n|
          ]
//@[010:00011) |   |   |   |   |   |   └─Token(RightSquare) |]|
//@[011:00013) |   |   |   |   |   ├─Token(NewLine) |\r\n|
        }
//@[008:00009) |   |   |   |   |   └─Token(RightBrace) |}|
//@[009:00011) |   |   |   |   ├─Token(NewLine) |\r\n|
      }
//@[006:00007) |   |   |   |   └─Token(RightBrace) |}|
//@[007:00009) |   |   |   ├─Token(NewLine) |\r\n|
    ]
//@[004:00005) |   |   |   └─Token(RightSquare) |]|
//@[005:00007) |   |   ├─Token(NewLine) |\r\n|
  }
//@[002:00003) |   |   └─Token(RightBrace) |}|
//@[003:00005) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

/*
Discriminator key missing
*/
//@[002:00004) ├─Token(NewLine) |\r\n|
resource discriminatorKeyMissing 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
//@[000:00148) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00032) | ├─IdentifierSyntax
//@[009:00032) | | └─Token(Identifier) |discriminatorKeyMissing|
//@[033:00083) | ├─StringSyntax
//@[033:00083) | | └─Token(StringComplete) |'Microsoft.Resources/deploymentScripts@2020-10-01'|
//@[084:00085) | ├─Token(Assignment) |=|
//@[086:00148) | └─ObjectSyntax
//@[086:00087) |   ├─Token(LeftBrace) |{|
//@[087:00089) |   ├─Token(NewLine) |\r\n|
  // #completionTest(0,1,2) -> discriminatorProperty
//@[052:00054) |   ├─Token(NewLine) |\r\n|
  
//@[002:00004) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

/*
Discriminator key missing (conditional)
*/
//@[002:00004) ├─Token(NewLine) |\r\n|
resource discriminatorKeyMissing_if 'Microsoft.Resources/deploymentScripts@2020-10-01' = if(true) {
//@[000:00160) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00035) | ├─IdentifierSyntax
//@[009:00035) | | └─Token(Identifier) |discriminatorKeyMissing_if|
//@[036:00086) | ├─StringSyntax
//@[036:00086) | | └─Token(StringComplete) |'Microsoft.Resources/deploymentScripts@2020-10-01'|
//@[087:00088) | ├─Token(Assignment) |=|
//@[089:00160) | └─IfConditionSyntax
//@[089:00091) |   ├─Token(Identifier) |if|
//@[091:00097) |   ├─ParenthesizedExpressionSyntax
//@[091:00092) |   | ├─Token(LeftParen) |(|
//@[092:00096) |   | ├─BooleanLiteralSyntax
//@[092:00096) |   | | └─Token(TrueKeyword) |true|
//@[096:00097) |   | └─Token(RightParen) |)|
//@[098:00160) |   └─ObjectSyntax
//@[098:00099) |     ├─Token(LeftBrace) |{|
//@[099:00101) |     ├─Token(NewLine) |\r\n|
  // #completionTest(0,1,2) -> discriminatorProperty
//@[052:00054) |     ├─Token(NewLine) |\r\n|
  
//@[002:00004) |     ├─Token(NewLine) |\r\n|
}
//@[000:00001) |     └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

/*
Discriminator key missing (loop)
*/
//@[002:00004) ├─Token(NewLine) |\r\n|
resource discriminatorKeyMissing_for 'Microsoft.Resources/deploymentScripts@2020-10-01' = [for thing in []: {
//@[000:00171) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00036) | ├─IdentifierSyntax
//@[009:00036) | | └─Token(Identifier) |discriminatorKeyMissing_for|
//@[037:00087) | ├─StringSyntax
//@[037:00087) | | └─Token(StringComplete) |'Microsoft.Resources/deploymentScripts@2020-10-01'|
//@[088:00089) | ├─Token(Assignment) |=|
//@[090:00171) | └─ForSyntax
//@[090:00091) |   ├─Token(LeftSquare) |[|
//@[091:00094) |   ├─Token(Identifier) |for|
//@[095:00100) |   ├─LocalVariableSyntax
//@[095:00100) |   | └─IdentifierSyntax
//@[095:00100) |   |   └─Token(Identifier) |thing|
//@[101:00103) |   ├─Token(Identifier) |in|
//@[104:00106) |   ├─ArraySyntax
//@[104:00105) |   | ├─Token(LeftSquare) |[|
//@[105:00106) |   | └─Token(RightSquare) |]|
//@[106:00107) |   ├─Token(Colon) |:|
//@[108:00170) |   ├─ObjectSyntax
//@[108:00109) |   | ├─Token(LeftBrace) |{|
//@[109:00111) |   | ├─Token(NewLine) |\r\n|
  // #completionTest(0,1,2) -> discriminatorProperty
//@[052:00054) |   | ├─Token(NewLine) |\r\n|
  
//@[002:00004) |   | ├─Token(NewLine) |\r\n|
}]
//@[000:00001) |   | └─Token(RightBrace) |}|
//@[001:00002) |   └─Token(RightSquare) |]|
//@[002:00006) ├─Token(NewLine) |\r\n\r\n|

/*
Discriminator key missing (filtered loop)
*/
//@[002:00004) ├─Token(NewLine) |\r\n|
resource discriminatorKeyMissing_for_if 'Microsoft.Resources/deploymentScripts@2020-10-01' = [for thing in []: if(true) {
//@[000:00183) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00039) | ├─IdentifierSyntax
//@[009:00039) | | └─Token(Identifier) |discriminatorKeyMissing_for_if|
//@[040:00090) | ├─StringSyntax
//@[040:00090) | | └─Token(StringComplete) |'Microsoft.Resources/deploymentScripts@2020-10-01'|
//@[091:00092) | ├─Token(Assignment) |=|
//@[093:00183) | └─ForSyntax
//@[093:00094) |   ├─Token(LeftSquare) |[|
//@[094:00097) |   ├─Token(Identifier) |for|
//@[098:00103) |   ├─LocalVariableSyntax
//@[098:00103) |   | └─IdentifierSyntax
//@[098:00103) |   |   └─Token(Identifier) |thing|
//@[104:00106) |   ├─Token(Identifier) |in|
//@[107:00109) |   ├─ArraySyntax
//@[107:00108) |   | ├─Token(LeftSquare) |[|
//@[108:00109) |   | └─Token(RightSquare) |]|
//@[109:00110) |   ├─Token(Colon) |:|
//@[111:00182) |   ├─IfConditionSyntax
//@[111:00113) |   | ├─Token(Identifier) |if|
//@[113:00119) |   | ├─ParenthesizedExpressionSyntax
//@[113:00114) |   | | ├─Token(LeftParen) |(|
//@[114:00118) |   | | ├─BooleanLiteralSyntax
//@[114:00118) |   | | | └─Token(TrueKeyword) |true|
//@[118:00119) |   | | └─Token(RightParen) |)|
//@[120:00182) |   | └─ObjectSyntax
//@[120:00121) |   |   ├─Token(LeftBrace) |{|
//@[121:00123) |   |   ├─Token(NewLine) |\r\n|
  // #completionTest(0,1,2) -> discriminatorProperty
//@[052:00054) |   |   ├─Token(NewLine) |\r\n|
  
//@[002:00004) |   |   ├─Token(NewLine) |\r\n|
}]
//@[000:00001) |   |   └─Token(RightBrace) |}|
//@[001:00002) |   └─Token(RightSquare) |]|
//@[002:00006) ├─Token(NewLine) |\r\n\r\n|

/*
Discriminator key value missing with property access
*/
//@[002:00004) ├─Token(NewLine) |\r\n|
resource discriminatorKeyValueMissing 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
//@[000:00175) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00037) | ├─IdentifierSyntax
//@[009:00037) | | └─Token(Identifier) |discriminatorKeyValueMissing|
//@[038:00088) | ├─StringSyntax
//@[038:00088) | | └─Token(StringComplete) |'Microsoft.Resources/deploymentScripts@2020-10-01'|
//@[089:00090) | ├─Token(Assignment) |=|
//@[091:00175) | └─ObjectSyntax
//@[091:00092) |   ├─Token(LeftBrace) |{|
//@[092:00094) |   ├─Token(NewLine) |\r\n|
  // #completionTest(7,8,9,10) -> deploymentScriptKindsPlusSymbols
//@[066:00068) |   ├─Token(NewLine) |\r\n|
  kind:   
//@[002:00010) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |kind|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[010:00010) |   | └─SkippedTriviaSyntax
//@[010:00012) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00003) ├─Token(NewLine) |\r\n|
// #completionTest(76) -> missingDiscriminatorPropertyAccess
//@[060:00062) ├─Token(NewLine) |\r\n|
var discriminatorKeyValueMissingCompletions = discriminatorKeyValueMissing.p
//@[000:00076) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00043) | ├─IdentifierSyntax
//@[004:00043) | | └─Token(Identifier) |discriminatorKeyValueMissingCompletions|
//@[044:00045) | ├─Token(Assignment) |=|
//@[046:00076) | └─PropertyAccessSyntax
//@[046:00074) |   ├─VariableAccessSyntax
//@[046:00074) |   | └─IdentifierSyntax
//@[046:00074) |   |   └─Token(Identifier) |discriminatorKeyValueMissing|
//@[074:00075) |   ├─Token(Dot) |.|
//@[075:00076) |   └─IdentifierSyntax
//@[075:00076) |     └─Token(Identifier) |p|
//@[076:00078) ├─Token(NewLine) |\r\n|
// #completionTest(76) -> missingDiscriminatorPropertyAccess
//@[060:00062) ├─Token(NewLine) |\r\n|
var discriminatorKeyValueMissingCompletions2 = discriminatorKeyValueMissing.
//@[000:00076) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00044) | ├─IdentifierSyntax
//@[004:00044) | | └─Token(Identifier) |discriminatorKeyValueMissingCompletions2|
//@[045:00046) | ├─Token(Assignment) |=|
//@[047:00076) | └─PropertyAccessSyntax
//@[047:00075) |   ├─VariableAccessSyntax
//@[047:00075) |   | └─IdentifierSyntax
//@[047:00075) |   |   └─Token(Identifier) |discriminatorKeyValueMissing|
//@[075:00076) |   ├─Token(Dot) |.|
//@[076:00076) |   └─IdentifierSyntax
//@[076:00076) |     └─SkippedTriviaSyntax
//@[076:00080) ├─Token(NewLine) |\r\n\r\n|

// #completionTest(76) -> missingDiscriminatorPropertyIndexPlusSymbols
//@[070:00072) ├─Token(NewLine) |\r\n|
var discriminatorKeyValueMissingCompletions3 = discriminatorKeyValueMissing[]
//@[000:00077) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00044) | ├─IdentifierSyntax
//@[004:00044) | | └─Token(Identifier) |discriminatorKeyValueMissingCompletions3|
//@[045:00046) | ├─Token(Assignment) |=|
//@[047:00077) | └─ArrayAccessSyntax
//@[047:00075) |   ├─VariableAccessSyntax
//@[047:00075) |   | └─IdentifierSyntax
//@[047:00075) |   |   └─Token(Identifier) |discriminatorKeyValueMissing|
//@[075:00076) |   ├─Token(LeftSquare) |[|
//@[076:00076) |   ├─SkippedTriviaSyntax
//@[076:00077) |   └─Token(RightSquare) |]|
//@[077:00081) ├─Token(NewLine) |\r\n\r\n|

/*
Discriminator key value missing with property access (conditional)
*/
//@[002:00004) ├─Token(NewLine) |\r\n|
resource discriminatorKeyValueMissing_if 'Microsoft.Resources/deploymentScripts@2020-10-01' = if(false) {
//@[000:00191) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00040) | ├─IdentifierSyntax
//@[009:00040) | | └─Token(Identifier) |discriminatorKeyValueMissing_if|
//@[041:00091) | ├─StringSyntax
//@[041:00091) | | └─Token(StringComplete) |'Microsoft.Resources/deploymentScripts@2020-10-01'|
//@[092:00093) | ├─Token(Assignment) |=|
//@[094:00191) | └─IfConditionSyntax
//@[094:00096) |   ├─Token(Identifier) |if|
//@[096:00103) |   ├─ParenthesizedExpressionSyntax
//@[096:00097) |   | ├─Token(LeftParen) |(|
//@[097:00102) |   | ├─BooleanLiteralSyntax
//@[097:00102) |   | | └─Token(FalseKeyword) |false|
//@[102:00103) |   | └─Token(RightParen) |)|
//@[104:00191) |   └─ObjectSyntax
//@[104:00105) |     ├─Token(LeftBrace) |{|
//@[105:00107) |     ├─Token(NewLine) |\r\n|
  // #completionTest(7,8,9,10) -> deploymentScriptKindsPlusSymbols_if
//@[069:00071) |     ├─Token(NewLine) |\r\n|
  kind:   
//@[002:00010) |     ├─ObjectPropertySyntax
//@[002:00006) |     | ├─IdentifierSyntax
//@[002:00006) |     | | └─Token(Identifier) |kind|
//@[006:00007) |     | ├─Token(Colon) |:|
//@[010:00010) |     | └─SkippedTriviaSyntax
//@[010:00012) |     ├─Token(NewLine) |\r\n|
}
//@[000:00001) |     └─Token(RightBrace) |}|
//@[001:00003) ├─Token(NewLine) |\r\n|
// #completionTest(82) -> missingDiscriminatorPropertyAccess
//@[060:00062) ├─Token(NewLine) |\r\n|
var discriminatorKeyValueMissingCompletions_if = discriminatorKeyValueMissing_if.p
//@[000:00082) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00046) | ├─IdentifierSyntax
//@[004:00046) | | └─Token(Identifier) |discriminatorKeyValueMissingCompletions_if|
//@[047:00048) | ├─Token(Assignment) |=|
//@[049:00082) | └─PropertyAccessSyntax
//@[049:00080) |   ├─VariableAccessSyntax
//@[049:00080) |   | └─IdentifierSyntax
//@[049:00080) |   |   └─Token(Identifier) |discriminatorKeyValueMissing_if|
//@[080:00081) |   ├─Token(Dot) |.|
//@[081:00082) |   └─IdentifierSyntax
//@[081:00082) |     └─Token(Identifier) |p|
//@[082:00084) ├─Token(NewLine) |\r\n|
// #completionTest(82) -> missingDiscriminatorPropertyAccess
//@[060:00062) ├─Token(NewLine) |\r\n|
var discriminatorKeyValueMissingCompletions2_if = discriminatorKeyValueMissing_if.
//@[000:00082) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00047) | ├─IdentifierSyntax
//@[004:00047) | | └─Token(Identifier) |discriminatorKeyValueMissingCompletions2_if|
//@[048:00049) | ├─Token(Assignment) |=|
//@[050:00082) | └─PropertyAccessSyntax
//@[050:00081) |   ├─VariableAccessSyntax
//@[050:00081) |   | └─IdentifierSyntax
//@[050:00081) |   |   └─Token(Identifier) |discriminatorKeyValueMissing_if|
//@[081:00082) |   ├─Token(Dot) |.|
//@[082:00082) |   └─IdentifierSyntax
//@[082:00082) |     └─SkippedTriviaSyntax
//@[082:00086) ├─Token(NewLine) |\r\n\r\n|

// #completionTest(82) -> missingDiscriminatorPropertyIndexPlusSymbols_if
//@[073:00075) ├─Token(NewLine) |\r\n|
var discriminatorKeyValueMissingCompletions3_if = discriminatorKeyValueMissing_if[]
//@[000:00083) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00047) | ├─IdentifierSyntax
//@[004:00047) | | └─Token(Identifier) |discriminatorKeyValueMissingCompletions3_if|
//@[048:00049) | ├─Token(Assignment) |=|
//@[050:00083) | └─ArrayAccessSyntax
//@[050:00081) |   ├─VariableAccessSyntax
//@[050:00081) |   | └─IdentifierSyntax
//@[050:00081) |   |   └─Token(Identifier) |discriminatorKeyValueMissing_if|
//@[081:00082) |   ├─Token(LeftSquare) |[|
//@[082:00082) |   ├─SkippedTriviaSyntax
//@[082:00083) |   └─Token(RightSquare) |]|
//@[083:00087) ├─Token(NewLine) |\r\n\r\n|

/*
Discriminator key value missing with property access (loops)
*/
//@[002:00004) ├─Token(NewLine) |\r\n|
resource discriminatorKeyValueMissing_for 'Microsoft.Resources/deploymentScripts@2020-10-01' = [for thing in []: {
//@[000:00202) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00041) | ├─IdentifierSyntax
//@[009:00041) | | └─Token(Identifier) |discriminatorKeyValueMissing_for|
//@[042:00092) | ├─StringSyntax
//@[042:00092) | | └─Token(StringComplete) |'Microsoft.Resources/deploymentScripts@2020-10-01'|
//@[093:00094) | ├─Token(Assignment) |=|
//@[095:00202) | └─ForSyntax
//@[095:00096) |   ├─Token(LeftSquare) |[|
//@[096:00099) |   ├─Token(Identifier) |for|
//@[100:00105) |   ├─LocalVariableSyntax
//@[100:00105) |   | └─IdentifierSyntax
//@[100:00105) |   |   └─Token(Identifier) |thing|
//@[106:00108) |   ├─Token(Identifier) |in|
//@[109:00111) |   ├─ArraySyntax
//@[109:00110) |   | ├─Token(LeftSquare) |[|
//@[110:00111) |   | └─Token(RightSquare) |]|
//@[111:00112) |   ├─Token(Colon) |:|
//@[113:00201) |   ├─ObjectSyntax
//@[113:00114) |   | ├─Token(LeftBrace) |{|
//@[114:00116) |   | ├─Token(NewLine) |\r\n|
  // #completionTest(7,8,9,10) -> deploymentScriptKindsPlusSymbols_for
//@[070:00072) |   | ├─Token(NewLine) |\r\n|
  kind:   
//@[002:00010) |   | ├─ObjectPropertySyntax
//@[002:00006) |   | | ├─IdentifierSyntax
//@[002:00006) |   | | | └─Token(Identifier) |kind|
//@[006:00007) |   | | ├─Token(Colon) |:|
//@[010:00010) |   | | └─SkippedTriviaSyntax
//@[010:00012) |   | ├─Token(NewLine) |\r\n|
}]
//@[000:00001) |   | └─Token(RightBrace) |}|
//@[001:00002) |   └─Token(RightSquare) |]|
//@[002:00006) ├─Token(NewLine) |\r\n\r\n|

// cannot . access properties of a resource loop
//@[048:00050) ├─Token(NewLine) |\r\n|
var resourceListIsNotSingleResource = discriminatorKeyValueMissing_for.kind
//@[000:00075) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00035) | ├─IdentifierSyntax
//@[004:00035) | | └─Token(Identifier) |resourceListIsNotSingleResource|
//@[036:00037) | ├─Token(Assignment) |=|
//@[038:00075) | └─PropertyAccessSyntax
//@[038:00070) |   ├─VariableAccessSyntax
//@[038:00070) |   | └─IdentifierSyntax
//@[038:00070) |   |   └─Token(Identifier) |discriminatorKeyValueMissing_for|
//@[070:00071) |   ├─Token(Dot) |.|
//@[071:00075) |   └─IdentifierSyntax
//@[071:00075) |     └─Token(Identifier) |kind|
//@[075:00079) ├─Token(NewLine) |\r\n\r\n|

// #completionTest(87) -> missingDiscriminatorPropertyAccess
//@[060:00062) ├─Token(NewLine) |\r\n|
var discriminatorKeyValueMissingCompletions_for = discriminatorKeyValueMissing_for[0].p
//@[000:00087) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00047) | ├─IdentifierSyntax
//@[004:00047) | | └─Token(Identifier) |discriminatorKeyValueMissingCompletions_for|
//@[048:00049) | ├─Token(Assignment) |=|
//@[050:00087) | └─PropertyAccessSyntax
//@[050:00085) |   ├─ArrayAccessSyntax
//@[050:00082) |   | ├─VariableAccessSyntax
//@[050:00082) |   | | └─IdentifierSyntax
//@[050:00082) |   | |   └─Token(Identifier) |discriminatorKeyValueMissing_for|
//@[082:00083) |   | ├─Token(LeftSquare) |[|
//@[083:00084) |   | ├─IntegerLiteralSyntax
//@[083:00084) |   | | └─Token(Integer) |0|
//@[084:00085) |   | └─Token(RightSquare) |]|
//@[085:00086) |   ├─Token(Dot) |.|
//@[086:00087) |   └─IdentifierSyntax
//@[086:00087) |     └─Token(Identifier) |p|
//@[087:00089) ├─Token(NewLine) |\r\n|
// #completionTest(87) -> missingDiscriminatorPropertyAccess
//@[060:00062) ├─Token(NewLine) |\r\n|
var discriminatorKeyValueMissingCompletions2_for = discriminatorKeyValueMissing_for[0].
//@[000:00087) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00048) | ├─IdentifierSyntax
//@[004:00048) | | └─Token(Identifier) |discriminatorKeyValueMissingCompletions2_for|
//@[049:00050) | ├─Token(Assignment) |=|
//@[051:00087) | └─PropertyAccessSyntax
//@[051:00086) |   ├─ArrayAccessSyntax
//@[051:00083) |   | ├─VariableAccessSyntax
//@[051:00083) |   | | └─IdentifierSyntax
//@[051:00083) |   | |   └─Token(Identifier) |discriminatorKeyValueMissing_for|
//@[083:00084) |   | ├─Token(LeftSquare) |[|
//@[084:00085) |   | ├─IntegerLiteralSyntax
//@[084:00085) |   | | └─Token(Integer) |0|
//@[085:00086) |   | └─Token(RightSquare) |]|
//@[086:00087) |   ├─Token(Dot) |.|
//@[087:00087) |   └─IdentifierSyntax
//@[087:00087) |     └─SkippedTriviaSyntax
//@[087:00091) ├─Token(NewLine) |\r\n\r\n|

// #completionTest(87) -> missingDiscriminatorPropertyIndexPlusSymbols_for
//@[074:00076) ├─Token(NewLine) |\r\n|
var discriminatorKeyValueMissingCompletions3_for = discriminatorKeyValueMissing_for[0][]
//@[000:00088) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00048) | ├─IdentifierSyntax
//@[004:00048) | | └─Token(Identifier) |discriminatorKeyValueMissingCompletions3_for|
//@[049:00050) | ├─Token(Assignment) |=|
//@[051:00088) | └─ArrayAccessSyntax
//@[051:00086) |   ├─ArrayAccessSyntax
//@[051:00083) |   | ├─VariableAccessSyntax
//@[051:00083) |   | | └─IdentifierSyntax
//@[051:00083) |   | |   └─Token(Identifier) |discriminatorKeyValueMissing_for|
//@[083:00084) |   | ├─Token(LeftSquare) |[|
//@[084:00085) |   | ├─IntegerLiteralSyntax
//@[084:00085) |   | | └─Token(Integer) |0|
//@[085:00086) |   | └─Token(RightSquare) |]|
//@[086:00087) |   ├─Token(LeftSquare) |[|
//@[087:00087) |   ├─SkippedTriviaSyntax
//@[087:00088) |   └─Token(RightSquare) |]|
//@[088:00092) ├─Token(NewLine) |\r\n\r\n|

/*
Discriminator key value missing with property access (filtered loops)
*/
//@[002:00004) ├─Token(NewLine) |\r\n|
resource discriminatorKeyValueMissing_for_if 'Microsoft.Resources/deploymentScripts@2020-10-01' = [for thing in []: if(true) {
//@[000:00217) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00044) | ├─IdentifierSyntax
//@[009:00044) | | └─Token(Identifier) |discriminatorKeyValueMissing_for_if|
//@[045:00095) | ├─StringSyntax
//@[045:00095) | | └─Token(StringComplete) |'Microsoft.Resources/deploymentScripts@2020-10-01'|
//@[096:00097) | ├─Token(Assignment) |=|
//@[098:00217) | └─ForSyntax
//@[098:00099) |   ├─Token(LeftSquare) |[|
//@[099:00102) |   ├─Token(Identifier) |for|
//@[103:00108) |   ├─LocalVariableSyntax
//@[103:00108) |   | └─IdentifierSyntax
//@[103:00108) |   |   └─Token(Identifier) |thing|
//@[109:00111) |   ├─Token(Identifier) |in|
//@[112:00114) |   ├─ArraySyntax
//@[112:00113) |   | ├─Token(LeftSquare) |[|
//@[113:00114) |   | └─Token(RightSquare) |]|
//@[114:00115) |   ├─Token(Colon) |:|
//@[116:00216) |   ├─IfConditionSyntax
//@[116:00118) |   | ├─Token(Identifier) |if|
//@[118:00124) |   | ├─ParenthesizedExpressionSyntax
//@[118:00119) |   | | ├─Token(LeftParen) |(|
//@[119:00123) |   | | ├─BooleanLiteralSyntax
//@[119:00123) |   | | | └─Token(TrueKeyword) |true|
//@[123:00124) |   | | └─Token(RightParen) |)|
//@[125:00216) |   | └─ObjectSyntax
//@[125:00126) |   |   ├─Token(LeftBrace) |{|
//@[126:00128) |   |   ├─Token(NewLine) |\r\n|
  // #completionTest(7,8,9,10) -> deploymentScriptKindsPlusSymbols_for_if
//@[073:00075) |   |   ├─Token(NewLine) |\r\n|
  kind:   
//@[002:00010) |   |   ├─ObjectPropertySyntax
//@[002:00006) |   |   | ├─IdentifierSyntax
//@[002:00006) |   |   | | └─Token(Identifier) |kind|
//@[006:00007) |   |   | ├─Token(Colon) |:|
//@[010:00010) |   |   | └─SkippedTriviaSyntax
//@[010:00012) |   |   ├─Token(NewLine) |\r\n|
}]
//@[000:00001) |   |   └─Token(RightBrace) |}|
//@[001:00002) |   └─Token(RightSquare) |]|
//@[002:00006) ├─Token(NewLine) |\r\n\r\n|

// cannot . access properties of a resource loop
//@[048:00050) ├─Token(NewLine) |\r\n|
var resourceListIsNotSingleResource_if = discriminatorKeyValueMissing_for_if.kind
//@[000:00081) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00038) | ├─IdentifierSyntax
//@[004:00038) | | └─Token(Identifier) |resourceListIsNotSingleResource_if|
//@[039:00040) | ├─Token(Assignment) |=|
//@[041:00081) | └─PropertyAccessSyntax
//@[041:00076) |   ├─VariableAccessSyntax
//@[041:00076) |   | └─IdentifierSyntax
//@[041:00076) |   |   └─Token(Identifier) |discriminatorKeyValueMissing_for_if|
//@[076:00077) |   ├─Token(Dot) |.|
//@[077:00081) |   └─IdentifierSyntax
//@[077:00081) |     └─Token(Identifier) |kind|
//@[081:00085) ├─Token(NewLine) |\r\n\r\n|

// #completionTest(93) -> missingDiscriminatorPropertyAccess
//@[060:00062) ├─Token(NewLine) |\r\n|
var discriminatorKeyValueMissingCompletions_for_if = discriminatorKeyValueMissing_for_if[0].p
//@[000:00093) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00050) | ├─IdentifierSyntax
//@[004:00050) | | └─Token(Identifier) |discriminatorKeyValueMissingCompletions_for_if|
//@[051:00052) | ├─Token(Assignment) |=|
//@[053:00093) | └─PropertyAccessSyntax
//@[053:00091) |   ├─ArrayAccessSyntax
//@[053:00088) |   | ├─VariableAccessSyntax
//@[053:00088) |   | | └─IdentifierSyntax
//@[053:00088) |   | |   └─Token(Identifier) |discriminatorKeyValueMissing_for_if|
//@[088:00089) |   | ├─Token(LeftSquare) |[|
//@[089:00090) |   | ├─IntegerLiteralSyntax
//@[089:00090) |   | | └─Token(Integer) |0|
//@[090:00091) |   | └─Token(RightSquare) |]|
//@[091:00092) |   ├─Token(Dot) |.|
//@[092:00093) |   └─IdentifierSyntax
//@[092:00093) |     └─Token(Identifier) |p|
//@[093:00095) ├─Token(NewLine) |\r\n|
// #completionTest(93) -> missingDiscriminatorPropertyAccess
//@[060:00062) ├─Token(NewLine) |\r\n|
var discriminatorKeyValueMissingCompletions2_for_if = discriminatorKeyValueMissing_for_if[0].
//@[000:00093) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00051) | ├─IdentifierSyntax
//@[004:00051) | | └─Token(Identifier) |discriminatorKeyValueMissingCompletions2_for_if|
//@[052:00053) | ├─Token(Assignment) |=|
//@[054:00093) | └─PropertyAccessSyntax
//@[054:00092) |   ├─ArrayAccessSyntax
//@[054:00089) |   | ├─VariableAccessSyntax
//@[054:00089) |   | | └─IdentifierSyntax
//@[054:00089) |   | |   └─Token(Identifier) |discriminatorKeyValueMissing_for_if|
//@[089:00090) |   | ├─Token(LeftSquare) |[|
//@[090:00091) |   | ├─IntegerLiteralSyntax
//@[090:00091) |   | | └─Token(Integer) |0|
//@[091:00092) |   | └─Token(RightSquare) |]|
//@[092:00093) |   ├─Token(Dot) |.|
//@[093:00093) |   └─IdentifierSyntax
//@[093:00093) |     └─SkippedTriviaSyntax
//@[093:00097) ├─Token(NewLine) |\r\n\r\n|

// #completionTest(93) -> missingDiscriminatorPropertyIndexPlusSymbols_for_if
//@[077:00079) ├─Token(NewLine) |\r\n|
var discriminatorKeyValueMissingCompletions3_for_if = discriminatorKeyValueMissing_for_if[0][]
//@[000:00094) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00051) | ├─IdentifierSyntax
//@[004:00051) | | └─Token(Identifier) |discriminatorKeyValueMissingCompletions3_for_if|
//@[052:00053) | ├─Token(Assignment) |=|
//@[054:00094) | └─ArrayAccessSyntax
//@[054:00092) |   ├─ArrayAccessSyntax
//@[054:00089) |   | ├─VariableAccessSyntax
//@[054:00089) |   | | └─IdentifierSyntax
//@[054:00089) |   | |   └─Token(Identifier) |discriminatorKeyValueMissing_for_if|
//@[089:00090) |   | ├─Token(LeftSquare) |[|
//@[090:00091) |   | ├─IntegerLiteralSyntax
//@[090:00091) |   | | └─Token(Integer) |0|
//@[091:00092) |   | └─Token(RightSquare) |]|
//@[092:00093) |   ├─Token(LeftSquare) |[|
//@[093:00093) |   ├─SkippedTriviaSyntax
//@[093:00094) |   └─Token(RightSquare) |]|
//@[094:00098) ├─Token(NewLine) |\r\n\r\n|

/*
Discriminator value set 1
*/
//@[002:00004) ├─Token(NewLine) |\r\n|
resource discriminatorKeySetOne 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
//@[000:00266) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00031) | ├─IdentifierSyntax
//@[009:00031) | | └─Token(Identifier) |discriminatorKeySetOne|
//@[032:00082) | ├─StringSyntax
//@[032:00082) | | └─Token(StringComplete) |'Microsoft.Resources/deploymentScripts@2020-10-01'|
//@[083:00084) | ├─Token(Assignment) |=|
//@[085:00266) | └─ObjectSyntax
//@[085:00086) |   ├─Token(LeftBrace) |{|
//@[086:00088) |   ├─Token(NewLine) |\r\n|
  kind: 'AzureCLI'
//@[002:00018) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |kind|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00018) |   | └─StringSyntax
//@[008:00018) |   |   └─Token(StringComplete) |'AzureCLI'|
//@[018:00020) |   ├─Token(NewLine) |\r\n|
  // #completionTest(0,1,2) -> deploymentScriptTopLevel
//@[055:00057) |   ├─Token(NewLine) |\r\n|
  
//@[002:00004) |   ├─Token(NewLine) |\r\n|
  properties: {
//@[002:00094) |   ├─ObjectPropertySyntax
//@[002:00012) |   | ├─IdentifierSyntax
//@[002:00012) |   | | └─Token(Identifier) |properties|
//@[012:00013) |   | ├─Token(Colon) |:|
//@[014:00094) |   | └─ObjectSyntax
//@[014:00015) |   |   ├─Token(LeftBrace) |{|
//@[015:00017) |   |   ├─Token(NewLine) |\r\n|
    // #completionTest(0,1,2,3,4) -> deploymentScriptCliProperties
//@[066:00068) |   |   ├─Token(NewLine) |\r\n|
    
//@[004:00006) |   |   ├─Token(NewLine) |\r\n|
  }
//@[002:00003) |   |   └─Token(RightBrace) |}|
//@[003:00005) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00003) ├─Token(NewLine) |\r\n|
// #completionTest(75) -> cliPropertyAccess
//@[043:00045) ├─Token(NewLine) |\r\n|
var discriminatorKeySetOneCompletions = discriminatorKeySetOne.properties.a
//@[000:00075) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00037) | ├─IdentifierSyntax
//@[004:00037) | | └─Token(Identifier) |discriminatorKeySetOneCompletions|
//@[038:00039) | ├─Token(Assignment) |=|
//@[040:00075) | └─PropertyAccessSyntax
//@[040:00073) |   ├─PropertyAccessSyntax
//@[040:00062) |   | ├─VariableAccessSyntax
//@[040:00062) |   | | └─IdentifierSyntax
//@[040:00062) |   | |   └─Token(Identifier) |discriminatorKeySetOne|
//@[062:00063) |   | ├─Token(Dot) |.|
//@[063:00073) |   | └─IdentifierSyntax
//@[063:00073) |   |   └─Token(Identifier) |properties|
//@[073:00074) |   ├─Token(Dot) |.|
//@[074:00075) |   └─IdentifierSyntax
//@[074:00075) |     └─Token(Identifier) |a|
//@[075:00077) ├─Token(NewLine) |\r\n|
// #completionTest(75) -> cliPropertyAccess
//@[043:00045) ├─Token(NewLine) |\r\n|
var discriminatorKeySetOneCompletions2 = discriminatorKeySetOne.properties.
//@[000:00075) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00038) | ├─IdentifierSyntax
//@[004:00038) | | └─Token(Identifier) |discriminatorKeySetOneCompletions2|
//@[039:00040) | ├─Token(Assignment) |=|
//@[041:00075) | └─PropertyAccessSyntax
//@[041:00074) |   ├─PropertyAccessSyntax
//@[041:00063) |   | ├─VariableAccessSyntax
//@[041:00063) |   | | └─IdentifierSyntax
//@[041:00063) |   | |   └─Token(Identifier) |discriminatorKeySetOne|
//@[063:00064) |   | ├─Token(Dot) |.|
//@[064:00074) |   | └─IdentifierSyntax
//@[064:00074) |   |   └─Token(Identifier) |properties|
//@[074:00075) |   ├─Token(Dot) |.|
//@[075:00075) |   └─IdentifierSyntax
//@[075:00075) |     └─SkippedTriviaSyntax
//@[075:00079) ├─Token(NewLine) |\r\n\r\n|

// #completionTest(75) -> cliPropertyAccessIndexesPlusSymbols
//@[061:00063) ├─Token(NewLine) |\r\n|
var discriminatorKeySetOneCompletions3 = discriminatorKeySetOne.properties[]
//@[000:00076) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00038) | ├─IdentifierSyntax
//@[004:00038) | | └─Token(Identifier) |discriminatorKeySetOneCompletions3|
//@[039:00040) | ├─Token(Assignment) |=|
//@[041:00076) | └─ArrayAccessSyntax
//@[041:00074) |   ├─PropertyAccessSyntax
//@[041:00063) |   | ├─VariableAccessSyntax
//@[041:00063) |   | | └─IdentifierSyntax
//@[041:00063) |   | |   └─Token(Identifier) |discriminatorKeySetOne|
//@[063:00064) |   | ├─Token(Dot) |.|
//@[064:00074) |   | └─IdentifierSyntax
//@[064:00074) |   |   └─Token(Identifier) |properties|
//@[074:00075) |   ├─Token(LeftSquare) |[|
//@[075:00075) |   ├─SkippedTriviaSyntax
//@[075:00076) |   └─Token(RightSquare) |]|
//@[076:00080) ├─Token(NewLine) |\r\n\r\n|

/*
Discriminator value set 1 (conditional)
*/
//@[002:00004) ├─Token(NewLine) |\r\n|
resource discriminatorKeySetOne_if 'Microsoft.Resources/deploymentScripts@2020-10-01' = if(2==3) {
//@[000:00278) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00034) | ├─IdentifierSyntax
//@[009:00034) | | └─Token(Identifier) |discriminatorKeySetOne_if|
//@[035:00085) | ├─StringSyntax
//@[035:00085) | | └─Token(StringComplete) |'Microsoft.Resources/deploymentScripts@2020-10-01'|
//@[086:00087) | ├─Token(Assignment) |=|
//@[088:00278) | └─IfConditionSyntax
//@[088:00090) |   ├─Token(Identifier) |if|
//@[090:00096) |   ├─ParenthesizedExpressionSyntax
//@[090:00091) |   | ├─Token(LeftParen) |(|
//@[091:00095) |   | ├─BinaryOperationSyntax
//@[091:00092) |   | | ├─IntegerLiteralSyntax
//@[091:00092) |   | | | └─Token(Integer) |2|
//@[092:00094) |   | | ├─Token(Equals) |==|
//@[094:00095) |   | | └─IntegerLiteralSyntax
//@[094:00095) |   | |   └─Token(Integer) |3|
//@[095:00096) |   | └─Token(RightParen) |)|
//@[097:00278) |   └─ObjectSyntax
//@[097:00098) |     ├─Token(LeftBrace) |{|
//@[098:00100) |     ├─Token(NewLine) |\r\n|
  kind: 'AzureCLI'
//@[002:00018) |     ├─ObjectPropertySyntax
//@[002:00006) |     | ├─IdentifierSyntax
//@[002:00006) |     | | └─Token(Identifier) |kind|
//@[006:00007) |     | ├─Token(Colon) |:|
//@[008:00018) |     | └─StringSyntax
//@[008:00018) |     |   └─Token(StringComplete) |'AzureCLI'|
//@[018:00020) |     ├─Token(NewLine) |\r\n|
  // #completionTest(0,1,2) -> deploymentScriptTopLevel
//@[055:00057) |     ├─Token(NewLine) |\r\n|
  
//@[002:00004) |     ├─Token(NewLine) |\r\n|
  properties: {
//@[002:00094) |     ├─ObjectPropertySyntax
//@[002:00012) |     | ├─IdentifierSyntax
//@[002:00012) |     | | └─Token(Identifier) |properties|
//@[012:00013) |     | ├─Token(Colon) |:|
//@[014:00094) |     | └─ObjectSyntax
//@[014:00015) |     |   ├─Token(LeftBrace) |{|
//@[015:00017) |     |   ├─Token(NewLine) |\r\n|
    // #completionTest(0,1,2,3,4) -> deploymentScriptCliProperties
//@[066:00068) |     |   ├─Token(NewLine) |\r\n|
    
//@[004:00006) |     |   ├─Token(NewLine) |\r\n|
  }
//@[002:00003) |     |   └─Token(RightBrace) |}|
//@[003:00005) |     ├─Token(NewLine) |\r\n|
}
//@[000:00001) |     └─Token(RightBrace) |}|
//@[001:00003) ├─Token(NewLine) |\r\n|
// #completionTest(81) -> cliPropertyAccess
//@[043:00045) ├─Token(NewLine) |\r\n|
var discriminatorKeySetOneCompletions_if = discriminatorKeySetOne_if.properties.a
//@[000:00081) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00040) | ├─IdentifierSyntax
//@[004:00040) | | └─Token(Identifier) |discriminatorKeySetOneCompletions_if|
//@[041:00042) | ├─Token(Assignment) |=|
//@[043:00081) | └─PropertyAccessSyntax
//@[043:00079) |   ├─PropertyAccessSyntax
//@[043:00068) |   | ├─VariableAccessSyntax
//@[043:00068) |   | | └─IdentifierSyntax
//@[043:00068) |   | |   └─Token(Identifier) |discriminatorKeySetOne_if|
//@[068:00069) |   | ├─Token(Dot) |.|
//@[069:00079) |   | └─IdentifierSyntax
//@[069:00079) |   |   └─Token(Identifier) |properties|
//@[079:00080) |   ├─Token(Dot) |.|
//@[080:00081) |   └─IdentifierSyntax
//@[080:00081) |     └─Token(Identifier) |a|
//@[081:00083) ├─Token(NewLine) |\r\n|
// #completionTest(81) -> cliPropertyAccess
//@[043:00045) ├─Token(NewLine) |\r\n|
var discriminatorKeySetOneCompletions2_if = discriminatorKeySetOne_if.properties.
//@[000:00081) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00041) | ├─IdentifierSyntax
//@[004:00041) | | └─Token(Identifier) |discriminatorKeySetOneCompletions2_if|
//@[042:00043) | ├─Token(Assignment) |=|
//@[044:00081) | └─PropertyAccessSyntax
//@[044:00080) |   ├─PropertyAccessSyntax
//@[044:00069) |   | ├─VariableAccessSyntax
//@[044:00069) |   | | └─IdentifierSyntax
//@[044:00069) |   | |   └─Token(Identifier) |discriminatorKeySetOne_if|
//@[069:00070) |   | ├─Token(Dot) |.|
//@[070:00080) |   | └─IdentifierSyntax
//@[070:00080) |   |   └─Token(Identifier) |properties|
//@[080:00081) |   ├─Token(Dot) |.|
//@[081:00081) |   └─IdentifierSyntax
//@[081:00081) |     └─SkippedTriviaSyntax
//@[081:00085) ├─Token(NewLine) |\r\n\r\n|

// #completionTest(81) -> cliPropertyAccessIndexesPlusSymbols_if
//@[064:00066) ├─Token(NewLine) |\r\n|
var discriminatorKeySetOneCompletions3_if = discriminatorKeySetOne_if.properties[]
//@[000:00082) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00041) | ├─IdentifierSyntax
//@[004:00041) | | └─Token(Identifier) |discriminatorKeySetOneCompletions3_if|
//@[042:00043) | ├─Token(Assignment) |=|
//@[044:00082) | └─ArrayAccessSyntax
//@[044:00080) |   ├─PropertyAccessSyntax
//@[044:00069) |   | ├─VariableAccessSyntax
//@[044:00069) |   | | └─IdentifierSyntax
//@[044:00069) |   | |   └─Token(Identifier) |discriminatorKeySetOne_if|
//@[069:00070) |   | ├─Token(Dot) |.|
//@[070:00080) |   | └─IdentifierSyntax
//@[070:00080) |   |   └─Token(Identifier) |properties|
//@[080:00081) |   ├─Token(LeftSquare) |[|
//@[081:00081) |   ├─SkippedTriviaSyntax
//@[081:00082) |   └─Token(RightSquare) |]|
//@[082:00086) ├─Token(NewLine) |\r\n\r\n|

/*
Discriminator value set 1 (loop)
*/
//@[002:00004) ├─Token(NewLine) |\r\n|
resource discriminatorKeySetOne_for 'Microsoft.Resources/deploymentScripts@2020-10-01' = [ for thing in []: {
//@[000:00290) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00035) | ├─IdentifierSyntax
//@[009:00035) | | └─Token(Identifier) |discriminatorKeySetOne_for|
//@[036:00086) | ├─StringSyntax
//@[036:00086) | | └─Token(StringComplete) |'Microsoft.Resources/deploymentScripts@2020-10-01'|
//@[087:00088) | ├─Token(Assignment) |=|
//@[089:00290) | └─ForSyntax
//@[089:00090) |   ├─Token(LeftSquare) |[|
//@[091:00094) |   ├─Token(Identifier) |for|
//@[095:00100) |   ├─LocalVariableSyntax
//@[095:00100) |   | └─IdentifierSyntax
//@[095:00100) |   |   └─Token(Identifier) |thing|
//@[101:00103) |   ├─Token(Identifier) |in|
//@[104:00106) |   ├─ArraySyntax
//@[104:00105) |   | ├─Token(LeftSquare) |[|
//@[105:00106) |   | └─Token(RightSquare) |]|
//@[106:00107) |   ├─Token(Colon) |:|
//@[108:00289) |   ├─ObjectSyntax
//@[108:00109) |   | ├─Token(LeftBrace) |{|
//@[109:00111) |   | ├─Token(NewLine) |\r\n|
  kind: 'AzureCLI'
//@[002:00018) |   | ├─ObjectPropertySyntax
//@[002:00006) |   | | ├─IdentifierSyntax
//@[002:00006) |   | | | └─Token(Identifier) |kind|
//@[006:00007) |   | | ├─Token(Colon) |:|
//@[008:00018) |   | | └─StringSyntax
//@[008:00018) |   | |   └─Token(StringComplete) |'AzureCLI'|
//@[018:00020) |   | ├─Token(NewLine) |\r\n|
  // #completionTest(0,1,2) -> deploymentScriptTopLevel
//@[055:00057) |   | ├─Token(NewLine) |\r\n|
  
//@[002:00004) |   | ├─Token(NewLine) |\r\n|
  properties: {
//@[002:00094) |   | ├─ObjectPropertySyntax
//@[002:00012) |   | | ├─IdentifierSyntax
//@[002:00012) |   | | | └─Token(Identifier) |properties|
//@[012:00013) |   | | ├─Token(Colon) |:|
//@[014:00094) |   | | └─ObjectSyntax
//@[014:00015) |   | |   ├─Token(LeftBrace) |{|
//@[015:00017) |   | |   ├─Token(NewLine) |\r\n|
    // #completionTest(0,1,2,3,4) -> deploymentScriptCliProperties
//@[066:00068) |   | |   ├─Token(NewLine) |\r\n|
    
//@[004:00006) |   | |   ├─Token(NewLine) |\r\n|
  }
//@[002:00003) |   | |   └─Token(RightBrace) |}|
//@[003:00005) |   | ├─Token(NewLine) |\r\n|
}]
//@[000:00001) |   | └─Token(RightBrace) |}|
//@[001:00002) |   └─Token(RightSquare) |]|
//@[002:00004) ├─Token(NewLine) |\r\n|
// #completionTest(86) -> cliPropertyAccess
//@[043:00045) ├─Token(NewLine) |\r\n|
var discriminatorKeySetOneCompletions_for = discriminatorKeySetOne_for[0].properties.a
//@[000:00086) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00041) | ├─IdentifierSyntax
//@[004:00041) | | └─Token(Identifier) |discriminatorKeySetOneCompletions_for|
//@[042:00043) | ├─Token(Assignment) |=|
//@[044:00086) | └─PropertyAccessSyntax
//@[044:00084) |   ├─PropertyAccessSyntax
//@[044:00073) |   | ├─ArrayAccessSyntax
//@[044:00070) |   | | ├─VariableAccessSyntax
//@[044:00070) |   | | | └─IdentifierSyntax
//@[044:00070) |   | | |   └─Token(Identifier) |discriminatorKeySetOne_for|
//@[070:00071) |   | | ├─Token(LeftSquare) |[|
//@[071:00072) |   | | ├─IntegerLiteralSyntax
//@[071:00072) |   | | | └─Token(Integer) |0|
//@[072:00073) |   | | └─Token(RightSquare) |]|
//@[073:00074) |   | ├─Token(Dot) |.|
//@[074:00084) |   | └─IdentifierSyntax
//@[074:00084) |   |   └─Token(Identifier) |properties|
//@[084:00085) |   ├─Token(Dot) |.|
//@[085:00086) |   └─IdentifierSyntax
//@[085:00086) |     └─Token(Identifier) |a|
//@[086:00088) ├─Token(NewLine) |\r\n|
// #completionTest(94) -> cliPropertyAccess
//@[043:00045) ├─Token(NewLine) |\r\n|
var discriminatorKeySetOneCompletions2_for = discriminatorKeySetOne_for[any(true)].properties.
//@[000:00094) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00042) | ├─IdentifierSyntax
//@[004:00042) | | └─Token(Identifier) |discriminatorKeySetOneCompletions2_for|
//@[043:00044) | ├─Token(Assignment) |=|
//@[045:00094) | └─PropertyAccessSyntax
//@[045:00093) |   ├─PropertyAccessSyntax
//@[045:00082) |   | ├─ArrayAccessSyntax
//@[045:00071) |   | | ├─VariableAccessSyntax
//@[045:00071) |   | | | └─IdentifierSyntax
//@[045:00071) |   | | |   └─Token(Identifier) |discriminatorKeySetOne_for|
//@[071:00072) |   | | ├─Token(LeftSquare) |[|
//@[072:00081) |   | | ├─FunctionCallSyntax
//@[072:00075) |   | | | ├─IdentifierSyntax
//@[072:00075) |   | | | | └─Token(Identifier) |any|
//@[075:00076) |   | | | ├─Token(LeftParen) |(|
//@[076:00080) |   | | | ├─FunctionArgumentSyntax
//@[076:00080) |   | | | | └─BooleanLiteralSyntax
//@[076:00080) |   | | | |   └─Token(TrueKeyword) |true|
//@[080:00081) |   | | | └─Token(RightParen) |)|
//@[081:00082) |   | | └─Token(RightSquare) |]|
//@[082:00083) |   | ├─Token(Dot) |.|
//@[083:00093) |   | └─IdentifierSyntax
//@[083:00093) |   |   └─Token(Identifier) |properties|
//@[093:00094) |   ├─Token(Dot) |.|
//@[094:00094) |   └─IdentifierSyntax
//@[094:00094) |     └─SkippedTriviaSyntax
//@[094:00098) ├─Token(NewLine) |\r\n\r\n|

// #completionTest(86) -> cliPropertyAccessIndexesPlusSymbols_for
//@[065:00067) ├─Token(NewLine) |\r\n|
var discriminatorKeySetOneCompletions3_for = discriminatorKeySetOne_for[1].properties[]
//@[000:00087) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00042) | ├─IdentifierSyntax
//@[004:00042) | | └─Token(Identifier) |discriminatorKeySetOneCompletions3_for|
//@[043:00044) | ├─Token(Assignment) |=|
//@[045:00087) | └─ArrayAccessSyntax
//@[045:00085) |   ├─PropertyAccessSyntax
//@[045:00074) |   | ├─ArrayAccessSyntax
//@[045:00071) |   | | ├─VariableAccessSyntax
//@[045:00071) |   | | | └─IdentifierSyntax
//@[045:00071) |   | | |   └─Token(Identifier) |discriminatorKeySetOne_for|
//@[071:00072) |   | | ├─Token(LeftSquare) |[|
//@[072:00073) |   | | ├─IntegerLiteralSyntax
//@[072:00073) |   | | | └─Token(Integer) |1|
//@[073:00074) |   | | └─Token(RightSquare) |]|
//@[074:00075) |   | ├─Token(Dot) |.|
//@[075:00085) |   | └─IdentifierSyntax
//@[075:00085) |   |   └─Token(Identifier) |properties|
//@[085:00086) |   ├─Token(LeftSquare) |[|
//@[086:00086) |   ├─SkippedTriviaSyntax
//@[086:00087) |   └─Token(RightSquare) |]|
//@[087:00091) ├─Token(NewLine) |\r\n\r\n|

/*
Discriminator value set 1 (filtered loop)
*/
//@[002:00004) ├─Token(NewLine) |\r\n|
resource discriminatorKeySetOne_for_if 'Microsoft.Resources/deploymentScripts@2020-10-01' = [ for thing in []: if(true) {
//@[000:00302) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00038) | ├─IdentifierSyntax
//@[009:00038) | | └─Token(Identifier) |discriminatorKeySetOne_for_if|
//@[039:00089) | ├─StringSyntax
//@[039:00089) | | └─Token(StringComplete) |'Microsoft.Resources/deploymentScripts@2020-10-01'|
//@[090:00091) | ├─Token(Assignment) |=|
//@[092:00302) | └─ForSyntax
//@[092:00093) |   ├─Token(LeftSquare) |[|
//@[094:00097) |   ├─Token(Identifier) |for|
//@[098:00103) |   ├─LocalVariableSyntax
//@[098:00103) |   | └─IdentifierSyntax
//@[098:00103) |   |   └─Token(Identifier) |thing|
//@[104:00106) |   ├─Token(Identifier) |in|
//@[107:00109) |   ├─ArraySyntax
//@[107:00108) |   | ├─Token(LeftSquare) |[|
//@[108:00109) |   | └─Token(RightSquare) |]|
//@[109:00110) |   ├─Token(Colon) |:|
//@[111:00301) |   ├─IfConditionSyntax
//@[111:00113) |   | ├─Token(Identifier) |if|
//@[113:00119) |   | ├─ParenthesizedExpressionSyntax
//@[113:00114) |   | | ├─Token(LeftParen) |(|
//@[114:00118) |   | | ├─BooleanLiteralSyntax
//@[114:00118) |   | | | └─Token(TrueKeyword) |true|
//@[118:00119) |   | | └─Token(RightParen) |)|
//@[120:00301) |   | └─ObjectSyntax
//@[120:00121) |   |   ├─Token(LeftBrace) |{|
//@[121:00123) |   |   ├─Token(NewLine) |\r\n|
  kind: 'AzureCLI'
//@[002:00018) |   |   ├─ObjectPropertySyntax
//@[002:00006) |   |   | ├─IdentifierSyntax
//@[002:00006) |   |   | | └─Token(Identifier) |kind|
//@[006:00007) |   |   | ├─Token(Colon) |:|
//@[008:00018) |   |   | └─StringSyntax
//@[008:00018) |   |   |   └─Token(StringComplete) |'AzureCLI'|
//@[018:00020) |   |   ├─Token(NewLine) |\r\n|
  // #completionTest(0,1,2) -> deploymentScriptTopLevel
//@[055:00057) |   |   ├─Token(NewLine) |\r\n|
  
//@[002:00004) |   |   ├─Token(NewLine) |\r\n|
  properties: {
//@[002:00094) |   |   ├─ObjectPropertySyntax
//@[002:00012) |   |   | ├─IdentifierSyntax
//@[002:00012) |   |   | | └─Token(Identifier) |properties|
//@[012:00013) |   |   | ├─Token(Colon) |:|
//@[014:00094) |   |   | └─ObjectSyntax
//@[014:00015) |   |   |   ├─Token(LeftBrace) |{|
//@[015:00017) |   |   |   ├─Token(NewLine) |\r\n|
    // #completionTest(0,1,2,3,4) -> deploymentScriptCliProperties
//@[066:00068) |   |   |   ├─Token(NewLine) |\r\n|
    
//@[004:00006) |   |   |   ├─Token(NewLine) |\r\n|
  }
//@[002:00003) |   |   |   └─Token(RightBrace) |}|
//@[003:00005) |   |   ├─Token(NewLine) |\r\n|
}]
//@[000:00001) |   |   └─Token(RightBrace) |}|
//@[001:00002) |   └─Token(RightSquare) |]|
//@[002:00004) ├─Token(NewLine) |\r\n|
// #completionTest(92) -> cliPropertyAccess
//@[043:00045) ├─Token(NewLine) |\r\n|
var discriminatorKeySetOneCompletions_for_if = discriminatorKeySetOne_for_if[0].properties.a
//@[000:00092) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00044) | ├─IdentifierSyntax
//@[004:00044) | | └─Token(Identifier) |discriminatorKeySetOneCompletions_for_if|
//@[045:00046) | ├─Token(Assignment) |=|
//@[047:00092) | └─PropertyAccessSyntax
//@[047:00090) |   ├─PropertyAccessSyntax
//@[047:00079) |   | ├─ArrayAccessSyntax
//@[047:00076) |   | | ├─VariableAccessSyntax
//@[047:00076) |   | | | └─IdentifierSyntax
//@[047:00076) |   | | |   └─Token(Identifier) |discriminatorKeySetOne_for_if|
//@[076:00077) |   | | ├─Token(LeftSquare) |[|
//@[077:00078) |   | | ├─IntegerLiteralSyntax
//@[077:00078) |   | | | └─Token(Integer) |0|
//@[078:00079) |   | | └─Token(RightSquare) |]|
//@[079:00080) |   | ├─Token(Dot) |.|
//@[080:00090) |   | └─IdentifierSyntax
//@[080:00090) |   |   └─Token(Identifier) |properties|
//@[090:00091) |   ├─Token(Dot) |.|
//@[091:00092) |   └─IdentifierSyntax
//@[091:00092) |     └─Token(Identifier) |a|
//@[092:00094) ├─Token(NewLine) |\r\n|
// #completionTest(100) -> cliPropertyAccess
//@[044:00046) ├─Token(NewLine) |\r\n|
var discriminatorKeySetOneCompletions2_for_if = discriminatorKeySetOne_for_if[any(true)].properties.
//@[000:00100) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00045) | ├─IdentifierSyntax
//@[004:00045) | | └─Token(Identifier) |discriminatorKeySetOneCompletions2_for_if|
//@[046:00047) | ├─Token(Assignment) |=|
//@[048:00100) | └─PropertyAccessSyntax
//@[048:00099) |   ├─PropertyAccessSyntax
//@[048:00088) |   | ├─ArrayAccessSyntax
//@[048:00077) |   | | ├─VariableAccessSyntax
//@[048:00077) |   | | | └─IdentifierSyntax
//@[048:00077) |   | | |   └─Token(Identifier) |discriminatorKeySetOne_for_if|
//@[077:00078) |   | | ├─Token(LeftSquare) |[|
//@[078:00087) |   | | ├─FunctionCallSyntax
//@[078:00081) |   | | | ├─IdentifierSyntax
//@[078:00081) |   | | | | └─Token(Identifier) |any|
//@[081:00082) |   | | | ├─Token(LeftParen) |(|
//@[082:00086) |   | | | ├─FunctionArgumentSyntax
//@[082:00086) |   | | | | └─BooleanLiteralSyntax
//@[082:00086) |   | | | |   └─Token(TrueKeyword) |true|
//@[086:00087) |   | | | └─Token(RightParen) |)|
//@[087:00088) |   | | └─Token(RightSquare) |]|
//@[088:00089) |   | ├─Token(Dot) |.|
//@[089:00099) |   | └─IdentifierSyntax
//@[089:00099) |   |   └─Token(Identifier) |properties|
//@[099:00100) |   ├─Token(Dot) |.|
//@[100:00100) |   └─IdentifierSyntax
//@[100:00100) |     └─SkippedTriviaSyntax
//@[100:00104) ├─Token(NewLine) |\r\n\r\n|

// #completionTest(92) -> cliPropertyAccessIndexesPlusSymbols_for_if
//@[068:00070) ├─Token(NewLine) |\r\n|
var discriminatorKeySetOneCompletions3_for_if = discriminatorKeySetOne_for_if[1].properties[]
//@[000:00093) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00045) | ├─IdentifierSyntax
//@[004:00045) | | └─Token(Identifier) |discriminatorKeySetOneCompletions3_for_if|
//@[046:00047) | ├─Token(Assignment) |=|
//@[048:00093) | └─ArrayAccessSyntax
//@[048:00091) |   ├─PropertyAccessSyntax
//@[048:00080) |   | ├─ArrayAccessSyntax
//@[048:00077) |   | | ├─VariableAccessSyntax
//@[048:00077) |   | | | └─IdentifierSyntax
//@[048:00077) |   | | |   └─Token(Identifier) |discriminatorKeySetOne_for_if|
//@[077:00078) |   | | ├─Token(LeftSquare) |[|
//@[078:00079) |   | | ├─IntegerLiteralSyntax
//@[078:00079) |   | | | └─Token(Integer) |1|
//@[079:00080) |   | | └─Token(RightSquare) |]|
//@[080:00081) |   | ├─Token(Dot) |.|
//@[081:00091) |   | └─IdentifierSyntax
//@[081:00091) |   |   └─Token(Identifier) |properties|
//@[091:00092) |   ├─Token(LeftSquare) |[|
//@[092:00092) |   ├─SkippedTriviaSyntax
//@[092:00093) |   └─Token(RightSquare) |]|
//@[093:00099) ├─Token(NewLine) |\r\n\r\n\r\n|


/*
Discriminator value set 2
*/
//@[002:00004) ├─Token(NewLine) |\r\n|
resource discriminatorKeySetTwo 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
//@[000:00272) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00031) | ├─IdentifierSyntax
//@[009:00031) | | └─Token(Identifier) |discriminatorKeySetTwo|
//@[032:00082) | ├─StringSyntax
//@[032:00082) | | └─Token(StringComplete) |'Microsoft.Resources/deploymentScripts@2020-10-01'|
//@[083:00084) | ├─Token(Assignment) |=|
//@[085:00272) | └─ObjectSyntax
//@[085:00086) |   ├─Token(LeftBrace) |{|
//@[086:00088) |   ├─Token(NewLine) |\r\n|
  kind: 'AzurePowerShell'
//@[002:00025) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |kind|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00025) |   | └─StringSyntax
//@[008:00025) |   |   └─Token(StringComplete) |'AzurePowerShell'|
//@[025:00027) |   ├─Token(NewLine) |\r\n|
  // #completionTest(0,1,2) -> deploymentScriptTopLevel
//@[055:00057) |   ├─Token(NewLine) |\r\n|
  
//@[002:00004) |   ├─Token(NewLine) |\r\n|
  properties: {
//@[002:00093) |   ├─ObjectPropertySyntax
//@[002:00012) |   | ├─IdentifierSyntax
//@[002:00012) |   | | └─Token(Identifier) |properties|
//@[012:00013) |   | ├─Token(Colon) |:|
//@[014:00093) |   | └─ObjectSyntax
//@[014:00015) |   |   ├─Token(LeftBrace) |{|
//@[015:00017) |   |   ├─Token(NewLine) |\r\n|
    // #completionTest(0,1,2,3,4) -> deploymentScriptPSProperties
//@[065:00067) |   |   ├─Token(NewLine) |\r\n|
    
//@[004:00006) |   |   ├─Token(NewLine) |\r\n|
  }
//@[002:00003) |   |   └─Token(RightBrace) |}|
//@[003:00005) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00003) ├─Token(NewLine) |\r\n|
// #completionTest(75) -> powershellPropertyAccess
//@[050:00052) ├─Token(NewLine) |\r\n|
var discriminatorKeySetTwoCompletions = discriminatorKeySetTwo.properties.a
//@[000:00075) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00037) | ├─IdentifierSyntax
//@[004:00037) | | └─Token(Identifier) |discriminatorKeySetTwoCompletions|
//@[038:00039) | ├─Token(Assignment) |=|
//@[040:00075) | └─PropertyAccessSyntax
//@[040:00073) |   ├─PropertyAccessSyntax
//@[040:00062) |   | ├─VariableAccessSyntax
//@[040:00062) |   | | └─IdentifierSyntax
//@[040:00062) |   | |   └─Token(Identifier) |discriminatorKeySetTwo|
//@[062:00063) |   | ├─Token(Dot) |.|
//@[063:00073) |   | └─IdentifierSyntax
//@[063:00073) |   |   └─Token(Identifier) |properties|
//@[073:00074) |   ├─Token(Dot) |.|
//@[074:00075) |   └─IdentifierSyntax
//@[074:00075) |     └─Token(Identifier) |a|
//@[075:00077) ├─Token(NewLine) |\r\n|
// #completionTest(75) -> powershellPropertyAccess
//@[050:00052) ├─Token(NewLine) |\r\n|
var discriminatorKeySetTwoCompletions2 = discriminatorKeySetTwo.properties.
//@[000:00075) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00038) | ├─IdentifierSyntax
//@[004:00038) | | └─Token(Identifier) |discriminatorKeySetTwoCompletions2|
//@[039:00040) | ├─Token(Assignment) |=|
//@[041:00075) | └─PropertyAccessSyntax
//@[041:00074) |   ├─PropertyAccessSyntax
//@[041:00063) |   | ├─VariableAccessSyntax
//@[041:00063) |   | | └─IdentifierSyntax
//@[041:00063) |   | |   └─Token(Identifier) |discriminatorKeySetTwo|
//@[063:00064) |   | ├─Token(Dot) |.|
//@[064:00074) |   | └─IdentifierSyntax
//@[064:00074) |   |   └─Token(Identifier) |properties|
//@[074:00075) |   ├─Token(Dot) |.|
//@[075:00075) |   └─IdentifierSyntax
//@[075:00075) |     └─SkippedTriviaSyntax
//@[075:00079) ├─Token(NewLine) |\r\n\r\n|

// #completionTest(90) -> powershellPropertyAccess
//@[050:00052) ├─Token(NewLine) |\r\n|
var discriminatorKeySetTwoCompletionsArrayIndexer = discriminatorKeySetTwo['properties'].a
//@[000:00090) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00049) | ├─IdentifierSyntax
//@[004:00049) | | └─Token(Identifier) |discriminatorKeySetTwoCompletionsArrayIndexer|
//@[050:00051) | ├─Token(Assignment) |=|
//@[052:00090) | └─PropertyAccessSyntax
//@[052:00088) |   ├─ArrayAccessSyntax
//@[052:00074) |   | ├─VariableAccessSyntax
//@[052:00074) |   | | └─IdentifierSyntax
//@[052:00074) |   | |   └─Token(Identifier) |discriminatorKeySetTwo|
//@[074:00075) |   | ├─Token(LeftSquare) |[|
//@[075:00087) |   | ├─StringSyntax
//@[075:00087) |   | | └─Token(StringComplete) |'properties'|
//@[087:00088) |   | └─Token(RightSquare) |]|
//@[088:00089) |   ├─Token(Dot) |.|
//@[089:00090) |   └─IdentifierSyntax
//@[089:00090) |     └─Token(Identifier) |a|
//@[090:00092) ├─Token(NewLine) |\r\n|
// #completionTest(90) -> powershellPropertyAccess
//@[050:00052) ├─Token(NewLine) |\r\n|
var discriminatorKeySetTwoCompletionsArrayIndexer2 = discriminatorKeySetTwo['properties'].
//@[000:00090) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00050) | ├─IdentifierSyntax
//@[004:00050) | | └─Token(Identifier) |discriminatorKeySetTwoCompletionsArrayIndexer2|
//@[051:00052) | ├─Token(Assignment) |=|
//@[053:00090) | └─PropertyAccessSyntax
//@[053:00089) |   ├─ArrayAccessSyntax
//@[053:00075) |   | ├─VariableAccessSyntax
//@[053:00075) |   | | └─IdentifierSyntax
//@[053:00075) |   | |   └─Token(Identifier) |discriminatorKeySetTwo|
//@[075:00076) |   | ├─Token(LeftSquare) |[|
//@[076:00088) |   | ├─StringSyntax
//@[076:00088) |   | | └─Token(StringComplete) |'properties'|
//@[088:00089) |   | └─Token(RightSquare) |]|
//@[089:00090) |   ├─Token(Dot) |.|
//@[090:00090) |   └─IdentifierSyntax
//@[090:00090) |     └─SkippedTriviaSyntax
//@[090:00094) ├─Token(NewLine) |\r\n\r\n|

/*
Discriminator value set 2 (conditional)
*/
//@[002:00004) ├─Token(NewLine) |\r\n|
resource discriminatorKeySetTwo_if 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
//@[000:00275) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00034) | ├─IdentifierSyntax
//@[009:00034) | | └─Token(Identifier) |discriminatorKeySetTwo_if|
//@[035:00085) | ├─StringSyntax
//@[035:00085) | | └─Token(StringComplete) |'Microsoft.Resources/deploymentScripts@2020-10-01'|
//@[086:00087) | ├─Token(Assignment) |=|
//@[088:00275) | └─ObjectSyntax
//@[088:00089) |   ├─Token(LeftBrace) |{|
//@[089:00091) |   ├─Token(NewLine) |\r\n|
  kind: 'AzurePowerShell'
//@[002:00025) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |kind|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00025) |   | └─StringSyntax
//@[008:00025) |   |   └─Token(StringComplete) |'AzurePowerShell'|
//@[025:00027) |   ├─Token(NewLine) |\r\n|
  // #completionTest(0,1,2) -> deploymentScriptTopLevel
//@[055:00057) |   ├─Token(NewLine) |\r\n|
  
//@[002:00004) |   ├─Token(NewLine) |\r\n|
  properties: {
//@[002:00093) |   ├─ObjectPropertySyntax
//@[002:00012) |   | ├─IdentifierSyntax
//@[002:00012) |   | | └─Token(Identifier) |properties|
//@[012:00013) |   | ├─Token(Colon) |:|
//@[014:00093) |   | └─ObjectSyntax
//@[014:00015) |   |   ├─Token(LeftBrace) |{|
//@[015:00017) |   |   ├─Token(NewLine) |\r\n|
    // #completionTest(0,1,2,3,4) -> deploymentScriptPSProperties
//@[065:00067) |   |   ├─Token(NewLine) |\r\n|
    
//@[004:00006) |   |   ├─Token(NewLine) |\r\n|
  }
//@[002:00003) |   |   └─Token(RightBrace) |}|
//@[003:00005) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00003) ├─Token(NewLine) |\r\n|
// #completionTest(81) -> powershellPropertyAccess
//@[050:00052) ├─Token(NewLine) |\r\n|
var discriminatorKeySetTwoCompletions_if = discriminatorKeySetTwo_if.properties.a
//@[000:00081) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00040) | ├─IdentifierSyntax
//@[004:00040) | | └─Token(Identifier) |discriminatorKeySetTwoCompletions_if|
//@[041:00042) | ├─Token(Assignment) |=|
//@[043:00081) | └─PropertyAccessSyntax
//@[043:00079) |   ├─PropertyAccessSyntax
//@[043:00068) |   | ├─VariableAccessSyntax
//@[043:00068) |   | | └─IdentifierSyntax
//@[043:00068) |   | |   └─Token(Identifier) |discriminatorKeySetTwo_if|
//@[068:00069) |   | ├─Token(Dot) |.|
//@[069:00079) |   | └─IdentifierSyntax
//@[069:00079) |   |   └─Token(Identifier) |properties|
//@[079:00080) |   ├─Token(Dot) |.|
//@[080:00081) |   └─IdentifierSyntax
//@[080:00081) |     └─Token(Identifier) |a|
//@[081:00083) ├─Token(NewLine) |\r\n|
// #completionTest(81) -> powershellPropertyAccess
//@[050:00052) ├─Token(NewLine) |\r\n|
var discriminatorKeySetTwoCompletions2_if = discriminatorKeySetTwo_if.properties.
//@[000:00081) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00041) | ├─IdentifierSyntax
//@[004:00041) | | └─Token(Identifier) |discriminatorKeySetTwoCompletions2_if|
//@[042:00043) | ├─Token(Assignment) |=|
//@[044:00081) | └─PropertyAccessSyntax
//@[044:00080) |   ├─PropertyAccessSyntax
//@[044:00069) |   | ├─VariableAccessSyntax
//@[044:00069) |   | | └─IdentifierSyntax
//@[044:00069) |   | |   └─Token(Identifier) |discriminatorKeySetTwo_if|
//@[069:00070) |   | ├─Token(Dot) |.|
//@[070:00080) |   | └─IdentifierSyntax
//@[070:00080) |   |   └─Token(Identifier) |properties|
//@[080:00081) |   ├─Token(Dot) |.|
//@[081:00081) |   └─IdentifierSyntax
//@[081:00081) |     └─SkippedTriviaSyntax
//@[081:00085) ├─Token(NewLine) |\r\n\r\n|

// #completionTest(96) -> powershellPropertyAccess
//@[050:00052) ├─Token(NewLine) |\r\n|
var discriminatorKeySetTwoCompletionsArrayIndexer_if = discriminatorKeySetTwo_if['properties'].a
//@[000:00096) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00052) | ├─IdentifierSyntax
//@[004:00052) | | └─Token(Identifier) |discriminatorKeySetTwoCompletionsArrayIndexer_if|
//@[053:00054) | ├─Token(Assignment) |=|
//@[055:00096) | └─PropertyAccessSyntax
//@[055:00094) |   ├─ArrayAccessSyntax
//@[055:00080) |   | ├─VariableAccessSyntax
//@[055:00080) |   | | └─IdentifierSyntax
//@[055:00080) |   | |   └─Token(Identifier) |discriminatorKeySetTwo_if|
//@[080:00081) |   | ├─Token(LeftSquare) |[|
//@[081:00093) |   | ├─StringSyntax
//@[081:00093) |   | | └─Token(StringComplete) |'properties'|
//@[093:00094) |   | └─Token(RightSquare) |]|
//@[094:00095) |   ├─Token(Dot) |.|
//@[095:00096) |   └─IdentifierSyntax
//@[095:00096) |     └─Token(Identifier) |a|
//@[096:00098) ├─Token(NewLine) |\r\n|
// #completionTest(96) -> powershellPropertyAccess
//@[050:00052) ├─Token(NewLine) |\r\n|
var discriminatorKeySetTwoCompletionsArrayIndexer2_if = discriminatorKeySetTwo_if['properties'].
//@[000:00096) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00053) | ├─IdentifierSyntax
//@[004:00053) | | └─Token(Identifier) |discriminatorKeySetTwoCompletionsArrayIndexer2_if|
//@[054:00055) | ├─Token(Assignment) |=|
//@[056:00096) | └─PropertyAccessSyntax
//@[056:00095) |   ├─ArrayAccessSyntax
//@[056:00081) |   | ├─VariableAccessSyntax
//@[056:00081) |   | | └─IdentifierSyntax
//@[056:00081) |   | |   └─Token(Identifier) |discriminatorKeySetTwo_if|
//@[081:00082) |   | ├─Token(LeftSquare) |[|
//@[082:00094) |   | ├─StringSyntax
//@[082:00094) |   | | └─Token(StringComplete) |'properties'|
//@[094:00095) |   | └─Token(RightSquare) |]|
//@[095:00096) |   ├─Token(Dot) |.|
//@[096:00096) |   └─IdentifierSyntax
//@[096:00096) |     └─SkippedTriviaSyntax
//@[096:00102) ├─Token(NewLine) |\r\n\r\n\r\n|


/*
Discriminator value set 2 (loops)
*/
//@[002:00004) ├─Token(NewLine) |\r\n|
resource discriminatorKeySetTwo_for 'Microsoft.Resources/deploymentScripts@2020-10-01' = [for thing in []: {
//@[000:00295) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00035) | ├─IdentifierSyntax
//@[009:00035) | | └─Token(Identifier) |discriminatorKeySetTwo_for|
//@[036:00086) | ├─StringSyntax
//@[036:00086) | | └─Token(StringComplete) |'Microsoft.Resources/deploymentScripts@2020-10-01'|
//@[087:00088) | ├─Token(Assignment) |=|
//@[089:00295) | └─ForSyntax
//@[089:00090) |   ├─Token(LeftSquare) |[|
//@[090:00093) |   ├─Token(Identifier) |for|
//@[094:00099) |   ├─LocalVariableSyntax
//@[094:00099) |   | └─IdentifierSyntax
//@[094:00099) |   |   └─Token(Identifier) |thing|
//@[100:00102) |   ├─Token(Identifier) |in|
//@[103:00105) |   ├─ArraySyntax
//@[103:00104) |   | ├─Token(LeftSquare) |[|
//@[104:00105) |   | └─Token(RightSquare) |]|
//@[105:00106) |   ├─Token(Colon) |:|
//@[107:00294) |   ├─ObjectSyntax
//@[107:00108) |   | ├─Token(LeftBrace) |{|
//@[108:00110) |   | ├─Token(NewLine) |\r\n|
  kind: 'AzurePowerShell'
//@[002:00025) |   | ├─ObjectPropertySyntax
//@[002:00006) |   | | ├─IdentifierSyntax
//@[002:00006) |   | | | └─Token(Identifier) |kind|
//@[006:00007) |   | | ├─Token(Colon) |:|
//@[008:00025) |   | | └─StringSyntax
//@[008:00025) |   | |   └─Token(StringComplete) |'AzurePowerShell'|
//@[025:00027) |   | ├─Token(NewLine) |\r\n|
  // #completionTest(0,1,2) -> deploymentScriptTopLevel
//@[055:00057) |   | ├─Token(NewLine) |\r\n|
  
//@[002:00004) |   | ├─Token(NewLine) |\r\n|
  properties: {
//@[002:00093) |   | ├─ObjectPropertySyntax
//@[002:00012) |   | | ├─IdentifierSyntax
//@[002:00012) |   | | | └─Token(Identifier) |properties|
//@[012:00013) |   | | ├─Token(Colon) |:|
//@[014:00093) |   | | └─ObjectSyntax
//@[014:00015) |   | |   ├─Token(LeftBrace) |{|
//@[015:00017) |   | |   ├─Token(NewLine) |\r\n|
    // #completionTest(0,1,2,3,4) -> deploymentScriptPSProperties
//@[065:00067) |   | |   ├─Token(NewLine) |\r\n|
    
//@[004:00006) |   | |   ├─Token(NewLine) |\r\n|
  }
//@[002:00003) |   | |   └─Token(RightBrace) |}|
//@[003:00005) |   | ├─Token(NewLine) |\r\n|
}]
//@[000:00001) |   | └─Token(RightBrace) |}|
//@[001:00002) |   └─Token(RightSquare) |]|
//@[002:00004) ├─Token(NewLine) |\r\n|
// #completionTest(86) -> powershellPropertyAccess
//@[050:00052) ├─Token(NewLine) |\r\n|
var discriminatorKeySetTwoCompletions_for = discriminatorKeySetTwo_for[0].properties.a
//@[000:00086) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00041) | ├─IdentifierSyntax
//@[004:00041) | | └─Token(Identifier) |discriminatorKeySetTwoCompletions_for|
//@[042:00043) | ├─Token(Assignment) |=|
//@[044:00086) | └─PropertyAccessSyntax
//@[044:00084) |   ├─PropertyAccessSyntax
//@[044:00073) |   | ├─ArrayAccessSyntax
//@[044:00070) |   | | ├─VariableAccessSyntax
//@[044:00070) |   | | | └─IdentifierSyntax
//@[044:00070) |   | | |   └─Token(Identifier) |discriminatorKeySetTwo_for|
//@[070:00071) |   | | ├─Token(LeftSquare) |[|
//@[071:00072) |   | | ├─IntegerLiteralSyntax
//@[071:00072) |   | | | └─Token(Integer) |0|
//@[072:00073) |   | | └─Token(RightSquare) |]|
//@[073:00074) |   | ├─Token(Dot) |.|
//@[074:00084) |   | └─IdentifierSyntax
//@[074:00084) |   |   └─Token(Identifier) |properties|
//@[084:00085) |   ├─Token(Dot) |.|
//@[085:00086) |   └─IdentifierSyntax
//@[085:00086) |     └─Token(Identifier) |a|
//@[086:00088) ├─Token(NewLine) |\r\n|
// #completionTest(86) -> powershellPropertyAccess
//@[050:00052) ├─Token(NewLine) |\r\n|
var discriminatorKeySetTwoCompletions2_for = discriminatorKeySetTwo_for[0].properties.
//@[000:00086) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00042) | ├─IdentifierSyntax
//@[004:00042) | | └─Token(Identifier) |discriminatorKeySetTwoCompletions2_for|
//@[043:00044) | ├─Token(Assignment) |=|
//@[045:00086) | └─PropertyAccessSyntax
//@[045:00085) |   ├─PropertyAccessSyntax
//@[045:00074) |   | ├─ArrayAccessSyntax
//@[045:00071) |   | | ├─VariableAccessSyntax
//@[045:00071) |   | | | └─IdentifierSyntax
//@[045:00071) |   | | |   └─Token(Identifier) |discriminatorKeySetTwo_for|
//@[071:00072) |   | | ├─Token(LeftSquare) |[|
//@[072:00073) |   | | ├─IntegerLiteralSyntax
//@[072:00073) |   | | | └─Token(Integer) |0|
//@[073:00074) |   | | └─Token(RightSquare) |]|
//@[074:00075) |   | ├─Token(Dot) |.|
//@[075:00085) |   | └─IdentifierSyntax
//@[075:00085) |   |   └─Token(Identifier) |properties|
//@[085:00086) |   ├─Token(Dot) |.|
//@[086:00086) |   └─IdentifierSyntax
//@[086:00086) |     └─SkippedTriviaSyntax
//@[086:00090) ├─Token(NewLine) |\r\n\r\n|

// #completionTest(101) -> powershellPropertyAccess
//@[051:00053) ├─Token(NewLine) |\r\n|
var discriminatorKeySetTwoCompletionsArrayIndexer_for = discriminatorKeySetTwo_for[0]['properties'].a
//@[000:00101) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00053) | ├─IdentifierSyntax
//@[004:00053) | | └─Token(Identifier) |discriminatorKeySetTwoCompletionsArrayIndexer_for|
//@[054:00055) | ├─Token(Assignment) |=|
//@[056:00101) | └─PropertyAccessSyntax
//@[056:00099) |   ├─ArrayAccessSyntax
//@[056:00085) |   | ├─ArrayAccessSyntax
//@[056:00082) |   | | ├─VariableAccessSyntax
//@[056:00082) |   | | | └─IdentifierSyntax
//@[056:00082) |   | | |   └─Token(Identifier) |discriminatorKeySetTwo_for|
//@[082:00083) |   | | ├─Token(LeftSquare) |[|
//@[083:00084) |   | | ├─IntegerLiteralSyntax
//@[083:00084) |   | | | └─Token(Integer) |0|
//@[084:00085) |   | | └─Token(RightSquare) |]|
//@[085:00086) |   | ├─Token(LeftSquare) |[|
//@[086:00098) |   | ├─StringSyntax
//@[086:00098) |   | | └─Token(StringComplete) |'properties'|
//@[098:00099) |   | └─Token(RightSquare) |]|
//@[099:00100) |   ├─Token(Dot) |.|
//@[100:00101) |   └─IdentifierSyntax
//@[100:00101) |     └─Token(Identifier) |a|
//@[101:00103) ├─Token(NewLine) |\r\n|
// #completionTest(101) -> powershellPropertyAccess
//@[051:00053) ├─Token(NewLine) |\r\n|
var discriminatorKeySetTwoCompletionsArrayIndexer2_for = discriminatorKeySetTwo_for[0]['properties'].
//@[000:00101) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00054) | ├─IdentifierSyntax
//@[004:00054) | | └─Token(Identifier) |discriminatorKeySetTwoCompletionsArrayIndexer2_for|
//@[055:00056) | ├─Token(Assignment) |=|
//@[057:00101) | └─PropertyAccessSyntax
//@[057:00100) |   ├─ArrayAccessSyntax
//@[057:00086) |   | ├─ArrayAccessSyntax
//@[057:00083) |   | | ├─VariableAccessSyntax
//@[057:00083) |   | | | └─IdentifierSyntax
//@[057:00083) |   | | |   └─Token(Identifier) |discriminatorKeySetTwo_for|
//@[083:00084) |   | | ├─Token(LeftSquare) |[|
//@[084:00085) |   | | ├─IntegerLiteralSyntax
//@[084:00085) |   | | | └─Token(Integer) |0|
//@[085:00086) |   | | └─Token(RightSquare) |]|
//@[086:00087) |   | ├─Token(LeftSquare) |[|
//@[087:00099) |   | ├─StringSyntax
//@[087:00099) |   | | └─Token(StringComplete) |'properties'|
//@[099:00100) |   | └─Token(RightSquare) |]|
//@[100:00101) |   ├─Token(Dot) |.|
//@[101:00101) |   └─IdentifierSyntax
//@[101:00101) |     └─SkippedTriviaSyntax
//@[101:00107) ├─Token(NewLine) |\r\n\r\n\r\n|


/*
Discriminator value set 2 (filtered loops)
*/
//@[002:00004) ├─Token(NewLine) |\r\n|
resource discriminatorKeySetTwo_for_if 'Microsoft.Resources/deploymentScripts@2020-10-01' = [for thing in []: if(true) {
//@[000:00307) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00038) | ├─IdentifierSyntax
//@[009:00038) | | └─Token(Identifier) |discriminatorKeySetTwo_for_if|
//@[039:00089) | ├─StringSyntax
//@[039:00089) | | └─Token(StringComplete) |'Microsoft.Resources/deploymentScripts@2020-10-01'|
//@[090:00091) | ├─Token(Assignment) |=|
//@[092:00307) | └─ForSyntax
//@[092:00093) |   ├─Token(LeftSquare) |[|
//@[093:00096) |   ├─Token(Identifier) |for|
//@[097:00102) |   ├─LocalVariableSyntax
//@[097:00102) |   | └─IdentifierSyntax
//@[097:00102) |   |   └─Token(Identifier) |thing|
//@[103:00105) |   ├─Token(Identifier) |in|
//@[106:00108) |   ├─ArraySyntax
//@[106:00107) |   | ├─Token(LeftSquare) |[|
//@[107:00108) |   | └─Token(RightSquare) |]|
//@[108:00109) |   ├─Token(Colon) |:|
//@[110:00306) |   ├─IfConditionSyntax
//@[110:00112) |   | ├─Token(Identifier) |if|
//@[112:00118) |   | ├─ParenthesizedExpressionSyntax
//@[112:00113) |   | | ├─Token(LeftParen) |(|
//@[113:00117) |   | | ├─BooleanLiteralSyntax
//@[113:00117) |   | | | └─Token(TrueKeyword) |true|
//@[117:00118) |   | | └─Token(RightParen) |)|
//@[119:00306) |   | └─ObjectSyntax
//@[119:00120) |   |   ├─Token(LeftBrace) |{|
//@[120:00122) |   |   ├─Token(NewLine) |\r\n|
  kind: 'AzurePowerShell'
//@[002:00025) |   |   ├─ObjectPropertySyntax
//@[002:00006) |   |   | ├─IdentifierSyntax
//@[002:00006) |   |   | | └─Token(Identifier) |kind|
//@[006:00007) |   |   | ├─Token(Colon) |:|
//@[008:00025) |   |   | └─StringSyntax
//@[008:00025) |   |   |   └─Token(StringComplete) |'AzurePowerShell'|
//@[025:00027) |   |   ├─Token(NewLine) |\r\n|
  // #completionTest(0,1,2) -> deploymentScriptTopLevel
//@[055:00057) |   |   ├─Token(NewLine) |\r\n|
  
//@[002:00004) |   |   ├─Token(NewLine) |\r\n|
  properties: {
//@[002:00093) |   |   ├─ObjectPropertySyntax
//@[002:00012) |   |   | ├─IdentifierSyntax
//@[002:00012) |   |   | | └─Token(Identifier) |properties|
//@[012:00013) |   |   | ├─Token(Colon) |:|
//@[014:00093) |   |   | └─ObjectSyntax
//@[014:00015) |   |   |   ├─Token(LeftBrace) |{|
//@[015:00017) |   |   |   ├─Token(NewLine) |\r\n|
    // #completionTest(0,1,2,3,4) -> deploymentScriptPSProperties
//@[065:00067) |   |   |   ├─Token(NewLine) |\r\n|
    
//@[004:00006) |   |   |   ├─Token(NewLine) |\r\n|
  }
//@[002:00003) |   |   |   └─Token(RightBrace) |}|
//@[003:00005) |   |   ├─Token(NewLine) |\r\n|
}]
//@[000:00001) |   |   └─Token(RightBrace) |}|
//@[001:00002) |   └─Token(RightSquare) |]|
//@[002:00004) ├─Token(NewLine) |\r\n|
// #completionTest(92) -> powershellPropertyAccess
//@[050:00052) ├─Token(NewLine) |\r\n|
var discriminatorKeySetTwoCompletions_for_if = discriminatorKeySetTwo_for_if[0].properties.a
//@[000:00092) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00044) | ├─IdentifierSyntax
//@[004:00044) | | └─Token(Identifier) |discriminatorKeySetTwoCompletions_for_if|
//@[045:00046) | ├─Token(Assignment) |=|
//@[047:00092) | └─PropertyAccessSyntax
//@[047:00090) |   ├─PropertyAccessSyntax
//@[047:00079) |   | ├─ArrayAccessSyntax
//@[047:00076) |   | | ├─VariableAccessSyntax
//@[047:00076) |   | | | └─IdentifierSyntax
//@[047:00076) |   | | |   └─Token(Identifier) |discriminatorKeySetTwo_for_if|
//@[076:00077) |   | | ├─Token(LeftSquare) |[|
//@[077:00078) |   | | ├─IntegerLiteralSyntax
//@[077:00078) |   | | | └─Token(Integer) |0|
//@[078:00079) |   | | └─Token(RightSquare) |]|
//@[079:00080) |   | ├─Token(Dot) |.|
//@[080:00090) |   | └─IdentifierSyntax
//@[080:00090) |   |   └─Token(Identifier) |properties|
//@[090:00091) |   ├─Token(Dot) |.|
//@[091:00092) |   └─IdentifierSyntax
//@[091:00092) |     └─Token(Identifier) |a|
//@[092:00094) ├─Token(NewLine) |\r\n|
// #completionTest(92) -> powershellPropertyAccess
//@[050:00052) ├─Token(NewLine) |\r\n|
var discriminatorKeySetTwoCompletions2_for_if = discriminatorKeySetTwo_for_if[0].properties.
//@[000:00092) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00045) | ├─IdentifierSyntax
//@[004:00045) | | └─Token(Identifier) |discriminatorKeySetTwoCompletions2_for_if|
//@[046:00047) | ├─Token(Assignment) |=|
//@[048:00092) | └─PropertyAccessSyntax
//@[048:00091) |   ├─PropertyAccessSyntax
//@[048:00080) |   | ├─ArrayAccessSyntax
//@[048:00077) |   | | ├─VariableAccessSyntax
//@[048:00077) |   | | | └─IdentifierSyntax
//@[048:00077) |   | | |   └─Token(Identifier) |discriminatorKeySetTwo_for_if|
//@[077:00078) |   | | ├─Token(LeftSquare) |[|
//@[078:00079) |   | | ├─IntegerLiteralSyntax
//@[078:00079) |   | | | └─Token(Integer) |0|
//@[079:00080) |   | | └─Token(RightSquare) |]|
//@[080:00081) |   | ├─Token(Dot) |.|
//@[081:00091) |   | └─IdentifierSyntax
//@[081:00091) |   |   └─Token(Identifier) |properties|
//@[091:00092) |   ├─Token(Dot) |.|
//@[092:00092) |   └─IdentifierSyntax
//@[092:00092) |     └─SkippedTriviaSyntax
//@[092:00096) ├─Token(NewLine) |\r\n\r\n|

// #completionTest(107) -> powershellPropertyAccess
//@[051:00053) ├─Token(NewLine) |\r\n|
var discriminatorKeySetTwoCompletionsArrayIndexer_for_if = discriminatorKeySetTwo_for_if[0]['properties'].a
//@[000:00107) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00056) | ├─IdentifierSyntax
//@[004:00056) | | └─Token(Identifier) |discriminatorKeySetTwoCompletionsArrayIndexer_for_if|
//@[057:00058) | ├─Token(Assignment) |=|
//@[059:00107) | └─PropertyAccessSyntax
//@[059:00105) |   ├─ArrayAccessSyntax
//@[059:00091) |   | ├─ArrayAccessSyntax
//@[059:00088) |   | | ├─VariableAccessSyntax
//@[059:00088) |   | | | └─IdentifierSyntax
//@[059:00088) |   | | |   └─Token(Identifier) |discriminatorKeySetTwo_for_if|
//@[088:00089) |   | | ├─Token(LeftSquare) |[|
//@[089:00090) |   | | ├─IntegerLiteralSyntax
//@[089:00090) |   | | | └─Token(Integer) |0|
//@[090:00091) |   | | └─Token(RightSquare) |]|
//@[091:00092) |   | ├─Token(LeftSquare) |[|
//@[092:00104) |   | ├─StringSyntax
//@[092:00104) |   | | └─Token(StringComplete) |'properties'|
//@[104:00105) |   | └─Token(RightSquare) |]|
//@[105:00106) |   ├─Token(Dot) |.|
//@[106:00107) |   └─IdentifierSyntax
//@[106:00107) |     └─Token(Identifier) |a|
//@[107:00109) ├─Token(NewLine) |\r\n|
// #completionTest(107) -> powershellPropertyAccess
//@[051:00053) ├─Token(NewLine) |\r\n|
var discriminatorKeySetTwoCompletionsArrayIndexer2_for_if = discriminatorKeySetTwo_for_if[0]['properties'].
//@[000:00107) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00057) | ├─IdentifierSyntax
//@[004:00057) | | └─Token(Identifier) |discriminatorKeySetTwoCompletionsArrayIndexer2_for_if|
//@[058:00059) | ├─Token(Assignment) |=|
//@[060:00107) | └─PropertyAccessSyntax
//@[060:00106) |   ├─ArrayAccessSyntax
//@[060:00092) |   | ├─ArrayAccessSyntax
//@[060:00089) |   | | ├─VariableAccessSyntax
//@[060:00089) |   | | | └─IdentifierSyntax
//@[060:00089) |   | | |   └─Token(Identifier) |discriminatorKeySetTwo_for_if|
//@[089:00090) |   | | ├─Token(LeftSquare) |[|
//@[090:00091) |   | | ├─IntegerLiteralSyntax
//@[090:00091) |   | | | └─Token(Integer) |0|
//@[091:00092) |   | | └─Token(RightSquare) |]|
//@[092:00093) |   | ├─Token(LeftSquare) |[|
//@[093:00105) |   | ├─StringSyntax
//@[093:00105) |   | | └─Token(StringComplete) |'properties'|
//@[105:00106) |   | └─Token(RightSquare) |]|
//@[106:00107) |   ├─Token(Dot) |.|
//@[107:00107) |   └─IdentifierSyntax
//@[107:00107) |     └─SkippedTriviaSyntax
//@[107:00115) ├─Token(NewLine) |\r\n\r\n\r\n\r\n|



resource incorrectPropertiesKey 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
//@[000:00132) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00031) | ├─IdentifierSyntax
//@[009:00031) | | └─Token(Identifier) |incorrectPropertiesKey|
//@[032:00082) | ├─StringSyntax
//@[032:00082) | | └─Token(StringComplete) |'Microsoft.Resources/deploymentScripts@2020-10-01'|
//@[083:00084) | ├─Token(Assignment) |=|
//@[085:00132) | └─ObjectSyntax
//@[085:00086) |   ├─Token(LeftBrace) |{|
//@[086:00088) |   ├─Token(NewLine) |\r\n|
  kind: 'AzureCLI'
//@[002:00018) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |kind|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00018) |   | └─StringSyntax
//@[008:00018) |   |   └─Token(StringComplete) |'AzureCLI'|
//@[018:00022) |   ├─Token(NewLine) |\r\n\r\n|

  propertes: {
//@[002:00019) |   ├─ObjectPropertySyntax
//@[002:00011) |   | ├─IdentifierSyntax
//@[002:00011) |   | | └─Token(Identifier) |propertes|
//@[011:00012) |   | ├─Token(Colon) |:|
//@[013:00019) |   | └─ObjectSyntax
//@[013:00014) |   |   ├─Token(LeftBrace) |{|
//@[014:00016) |   |   ├─Token(NewLine) |\r\n|
  }
//@[002:00003) |   |   └─Token(RightBrace) |}|
//@[003:00005) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

var mock = incorrectPropertiesKey.p
//@[000:00035) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00008) | ├─IdentifierSyntax
//@[004:00008) | | └─Token(Identifier) |mock|
//@[009:00010) | ├─Token(Assignment) |=|
//@[011:00035) | └─PropertyAccessSyntax
//@[011:00033) |   ├─VariableAccessSyntax
//@[011:00033) |   | └─IdentifierSyntax
//@[011:00033) |   |   └─Token(Identifier) |incorrectPropertiesKey|
//@[033:00034) |   ├─Token(Dot) |.|
//@[034:00035) |   └─IdentifierSyntax
//@[034:00035) |     └─Token(Identifier) |p|
//@[035:00039) ├─Token(NewLine) |\r\n\r\n|

resource incorrectPropertiesKey2 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
//@[000:00796) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00032) | ├─IdentifierSyntax
//@[009:00032) | | └─Token(Identifier) |incorrectPropertiesKey2|
//@[033:00083) | ├─StringSyntax
//@[033:00083) | | └─Token(StringComplete) |'Microsoft.Resources/deploymentScripts@2020-10-01'|
//@[084:00085) | ├─Token(Assignment) |=|
//@[086:00796) | └─ObjectSyntax
//@[086:00087) |   ├─Token(LeftBrace) |{|
//@[087:00089) |   ├─Token(NewLine) |\r\n|
  kind: 'AzureCLI'
//@[002:00018) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |kind|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00018) |   | └─StringSyntax
//@[008:00018) |   |   └─Token(StringComplete) |'AzureCLI'|
//@[018:00020) |   ├─Token(NewLine) |\r\n|
  name: 'test'
//@[002:00014) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00014) |   | └─StringSyntax
//@[008:00014) |   |   └─Token(StringComplete) |'test'|
//@[014:00016) |   ├─Token(NewLine) |\r\n|
  location: ''
//@[002:00014) |   ├─ObjectPropertySyntax
//@[002:00010) |   | ├─IdentifierSyntax
//@[002:00010) |   | | └─Token(Identifier) |location|
//@[010:00011) |   | ├─Token(Colon) |:|
//@[012:00014) |   | └─StringSyntax
//@[012:00014) |   |   └─Token(StringComplete) |''|
//@[014:00016) |   ├─Token(NewLine) |\r\n|
  properties: {
//@[002:00652) |   ├─ObjectPropertySyntax
//@[002:00012) |   | ├─IdentifierSyntax
//@[002:00012) |   | | └─Token(Identifier) |properties|
//@[012:00013) |   | ├─Token(Colon) |:|
//@[014:00652) |   | └─ObjectSyntax
//@[014:00015) |   |   ├─Token(LeftBrace) |{|
//@[015:00017) |   |   ├─Token(NewLine) |\r\n|
    azCliVersion: '2'
//@[004:00021) |   |   ├─ObjectPropertySyntax
//@[004:00016) |   |   | ├─IdentifierSyntax
//@[004:00016) |   |   | | └─Token(Identifier) |azCliVersion|
//@[016:00017) |   |   | ├─Token(Colon) |:|
//@[018:00021) |   |   | └─StringSyntax
//@[018:00021) |   |   |   └─Token(StringComplete) |'2'|
//@[021:00023) |   |   ├─Token(NewLine) |\r\n|
    retentionInterval: 'PT1H'
//@[004:00029) |   |   ├─ObjectPropertySyntax
//@[004:00021) |   |   | ├─IdentifierSyntax
//@[004:00021) |   |   | | └─Token(Identifier) |retentionInterval|
//@[021:00022) |   |   | ├─Token(Colon) |:|
//@[023:00029) |   |   | └─StringSyntax
//@[023:00029) |   |   |   └─Token(StringComplete) |'PT1H'|
//@[029:00031) |   |   ├─Token(NewLine) |\r\n|
    
//@[004:00006) |   |   ├─Token(NewLine) |\r\n|
    // #completionTest(0,1,2,3,4) -> deploymentScriptCliPropertiesMinusSpecified
//@[080:00082) |   |   ├─Token(NewLine) |\r\n|
    
//@[004:00006) |   |   ├─Token(NewLine) |\r\n|
    // #completionTest(22,23) -> cleanupPreferencesPlusSymbols
//@[062:00064) |   |   ├─Token(NewLine) |\r\n|
    cleanupPreference: 
//@[004:00023) |   |   ├─ObjectPropertySyntax
//@[004:00021) |   |   | ├─IdentifierSyntax
//@[004:00021) |   |   | | └─Token(Identifier) |cleanupPreference|
//@[021:00022) |   |   | ├─Token(Colon) |:|
//@[023:00023) |   |   | └─SkippedTriviaSyntax
//@[023:00027) |   |   ├─Token(NewLine) |\r\n\r\n|

    // #completionTest(25,26) -> arrayPlusSymbols
//@[049:00051) |   |   ├─Token(NewLine) |\r\n|
    supportingScriptUris: 
//@[004:00026) |   |   ├─ObjectPropertySyntax
//@[004:00024) |   |   | ├─IdentifierSyntax
//@[004:00024) |   |   | | └─Token(Identifier) |supportingScriptUris|
//@[024:00025) |   |   | ├─Token(Colon) |:|
//@[026:00026) |   |   | └─SkippedTriviaSyntax
//@[026:00030) |   |   ├─Token(NewLine) |\r\n\r\n|

    // #completionTest(27,28) -> objectPlusSymbols
//@[050:00052) |   |   ├─Token(NewLine) |\r\n|
    storageAccountSettings: 
//@[004:00028) |   |   ├─ObjectPropertySyntax
//@[004:00026) |   |   | ├─IdentifierSyntax
//@[004:00026) |   |   | | └─Token(Identifier) |storageAccountSettings|
//@[026:00027) |   |   | ├─Token(Colon) |:|
//@[028:00028) |   |   | └─SkippedTriviaSyntax
//@[028:00032) |   |   ├─Token(NewLine) |\r\n\r\n|

    environmentVariables: [
//@[004:00226) |   |   ├─ObjectPropertySyntax
//@[004:00024) |   |   | ├─IdentifierSyntax
//@[004:00024) |   |   | | └─Token(Identifier) |environmentVariables|
//@[024:00025) |   |   | ├─Token(Colon) |:|
//@[026:00226) |   |   | └─ArraySyntax
//@[026:00027) |   |   |   ├─Token(LeftSquare) |[|
//@[027:00029) |   |   |   ├─Token(NewLine) |\r\n|
      {
//@[006:00098) |   |   |   ├─ArrayItemSyntax
//@[006:00098) |   |   |   | └─ObjectSyntax
//@[006:00007) |   |   |   |   ├─Token(LeftBrace) |{|
//@[007:00009) |   |   |   |   ├─Token(NewLine) |\r\n|
        // #completionTest(0,2,4,6,8) -> environmentVariableProperties
//@[070:00072) |   |   |   |   ├─Token(NewLine) |\r\n|
        
//@[008:00010) |   |   |   |   ├─Token(NewLine) |\r\n|
      }
//@[006:00007) |   |   |   |   └─Token(RightBrace) |}|
//@[007:00009) |   |   |   ├─Token(NewLine) |\r\n|
      // #completionTest(0,1,2,3,4,5,6) -> objectPlusSymbolsWithRequiredProperties
//@[082:00084) |   |   |   ├─Token(NewLine) |\r\n|
      
//@[006:00008) |   |   |   ├─Token(NewLine) |\r\n|
    ]
//@[004:00005) |   |   |   └─Token(RightSquare) |]|
//@[005:00007) |   |   ├─Token(NewLine) |\r\n|
  }
//@[002:00003) |   |   └─Token(RightBrace) |}|
//@[003:00005) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

// #completionTest(21) -> resourceTypes
//@[039:00041) ├─Token(NewLine) |\r\n|
resource missingType 
//@[000:00021) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00020) | ├─IdentifierSyntax
//@[009:00020) | | └─Token(Identifier) |missingType|
//@[021:00021) | ├─SkippedTriviaSyntax
//@[021:00021) | ├─SkippedTriviaSyntax
//@[021:00021) | └─SkippedTriviaSyntax
//@[021:00025) ├─Token(NewLine) |\r\n\r\n|

// #completionTest(37,38,39,40,41,42,43,44) -> resourceTypes
//@[060:00062) ├─Token(NewLine) |\r\n|
resource startedTypingTypeWithQuotes 'virma'
//@[000:00044) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00036) | ├─IdentifierSyntax
//@[009:00036) | | └─Token(Identifier) |startedTypingTypeWithQuotes|
//@[037:00044) | ├─StringSyntax
//@[037:00044) | | └─Token(StringComplete) |'virma'|
//@[044:00044) | ├─SkippedTriviaSyntax
//@[044:00044) | └─SkippedTriviaSyntax
//@[044:00048) ├─Token(NewLine) |\r\n\r\n|

// #completionTest(40,41,42,43,44,45) -> resourceTypes
//@[054:00056) ├─Token(NewLine) |\r\n|
resource startedTypingTypeWithoutQuotes virma
//@[000:00045) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00039) | ├─IdentifierSyntax
//@[009:00039) | | └─Token(Identifier) |startedTypingTypeWithoutQuotes|
//@[040:00045) | ├─SkippedTriviaSyntax
//@[040:00045) | | └─Token(Identifier) |virma|
//@[045:00045) | ├─SkippedTriviaSyntax
//@[045:00045) | └─SkippedTriviaSyntax
//@[045:00049) ├─Token(NewLine) |\r\n\r\n|

resource dashesInPropertyNames 'Microsoft.ContainerService/managedClusters@2020-09-01' = {
//@[000:00093) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00030) | ├─IdentifierSyntax
//@[009:00030) | | └─Token(Identifier) |dashesInPropertyNames|
//@[031:00086) | ├─StringSyntax
//@[031:00086) | | └─Token(StringComplete) |'Microsoft.ContainerService/managedClusters@2020-09-01'|
//@[087:00088) | ├─Token(Assignment) |=|
//@[089:00093) | └─ObjectSyntax
//@[089:00090) |   ├─Token(LeftBrace) |{|
//@[090:00092) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00003) ├─Token(NewLine) |\r\n|
// #completionTest(78) -> autoScalerPropertiesRequireEscaping
//@[061:00063) ├─Token(NewLine) |\r\n|
var letsAccessTheDashes = dashesInPropertyNames.properties.autoScalerProfile.s
//@[000:00078) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00023) | ├─IdentifierSyntax
//@[004:00023) | | └─Token(Identifier) |letsAccessTheDashes|
//@[024:00025) | ├─Token(Assignment) |=|
//@[026:00078) | └─PropertyAccessSyntax
//@[026:00076) |   ├─PropertyAccessSyntax
//@[026:00058) |   | ├─PropertyAccessSyntax
//@[026:00047) |   | | ├─VariableAccessSyntax
//@[026:00047) |   | | | └─IdentifierSyntax
//@[026:00047) |   | | |   └─Token(Identifier) |dashesInPropertyNames|
//@[047:00048) |   | | ├─Token(Dot) |.|
//@[048:00058) |   | | └─IdentifierSyntax
//@[048:00058) |   | |   └─Token(Identifier) |properties|
//@[058:00059) |   | ├─Token(Dot) |.|
//@[059:00076) |   | └─IdentifierSyntax
//@[059:00076) |   |   └─Token(Identifier) |autoScalerProfile|
//@[076:00077) |   ├─Token(Dot) |.|
//@[077:00078) |   └─IdentifierSyntax
//@[077:00078) |     └─Token(Identifier) |s|
//@[078:00080) ├─Token(NewLine) |\r\n|
// #completionTest(78) -> autoScalerPropertiesRequireEscaping
//@[061:00063) ├─Token(NewLine) |\r\n|
var letsAccessTheDashes2 = dashesInPropertyNames.properties.autoScalerProfile.
//@[000:00078) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00024) | ├─IdentifierSyntax
//@[004:00024) | | └─Token(Identifier) |letsAccessTheDashes2|
//@[025:00026) | ├─Token(Assignment) |=|
//@[027:00078) | └─PropertyAccessSyntax
//@[027:00077) |   ├─PropertyAccessSyntax
//@[027:00059) |   | ├─PropertyAccessSyntax
//@[027:00048) |   | | ├─VariableAccessSyntax
//@[027:00048) |   | | | └─IdentifierSyntax
//@[027:00048) |   | | |   └─Token(Identifier) |dashesInPropertyNames|
//@[048:00049) |   | | ├─Token(Dot) |.|
//@[049:00059) |   | | └─IdentifierSyntax
//@[049:00059) |   | |   └─Token(Identifier) |properties|
//@[059:00060) |   | ├─Token(Dot) |.|
//@[060:00077) |   | └─IdentifierSyntax
//@[060:00077) |   |   └─Token(Identifier) |autoScalerProfile|
//@[077:00078) |   ├─Token(Dot) |.|
//@[078:00078) |   └─IdentifierSyntax
//@[078:00078) |     └─SkippedTriviaSyntax
//@[078:00082) ├─Token(NewLine) |\r\n\r\n|

/* 
Nested discriminator missing key
*/
//@[002:00004) ├─Token(NewLine) |\r\n|
resource nestedDiscriminatorMissingKey 'Microsoft.DocumentDB/databaseAccounts@2020-06-01-preview' = {
//@[000:00190) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00038) | ├─IdentifierSyntax
//@[009:00038) | | └─Token(Identifier) |nestedDiscriminatorMissingKey|
//@[039:00097) | ├─StringSyntax
//@[039:00097) | | └─Token(StringComplete) |'Microsoft.DocumentDB/databaseAccounts@2020-06-01-preview'|
//@[098:00099) | ├─Token(Assignment) |=|
//@[100:00190) | └─ObjectSyntax
//@[100:00101) |   ├─Token(LeftBrace) |{|
//@[101:00103) |   ├─Token(NewLine) |\r\n|
  name: 'test'
//@[002:00014) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00014) |   | └─StringSyntax
//@[008:00014) |   |   └─Token(StringComplete) |'test'|
//@[014:00016) |   ├─Token(NewLine) |\r\n|
  location: 'l'
//@[002:00015) |   ├─ObjectPropertySyntax
//@[002:00010) |   | ├─IdentifierSyntax
//@[002:00010) |   | | └─Token(Identifier) |location|
//@[010:00011) |   | ├─Token(Colon) |:|
//@[012:00015) |   | └─StringSyntax
//@[012:00015) |   |   └─Token(StringComplete) |'l'|
//@[015:00017) |   ├─Token(NewLine) |\r\n|
  properties: {
//@[002:00051) |   ├─ObjectPropertySyntax
//@[002:00012) |   | ├─IdentifierSyntax
//@[002:00012) |   | | └─Token(Identifier) |properties|
//@[012:00013) |   | ├─Token(Colon) |:|
//@[014:00051) |   | └─ObjectSyntax
//@[014:00015) |   |   ├─Token(LeftBrace) |{|
//@[015:00017) |   |   ├─Token(NewLine) |\r\n|
    //createMode: 'Default'
//@[027:00031) |   |   ├─Token(NewLine) |\r\n\r\n|

  }
//@[002:00003) |   |   └─Token(RightBrace) |}|
//@[003:00005) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00003) ├─Token(NewLine) |\r\n|
// #completionTest(90) -> createMode
//@[036:00038) ├─Token(NewLine) |\r\n|
var nestedDiscriminatorMissingKeyCompletions = nestedDiscriminatorMissingKey.properties.cr
//@[000:00090) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00044) | ├─IdentifierSyntax
//@[004:00044) | | └─Token(Identifier) |nestedDiscriminatorMissingKeyCompletions|
//@[045:00046) | ├─Token(Assignment) |=|
//@[047:00090) | └─PropertyAccessSyntax
//@[047:00087) |   ├─PropertyAccessSyntax
//@[047:00076) |   | ├─VariableAccessSyntax
//@[047:00076) |   | | └─IdentifierSyntax
//@[047:00076) |   | |   └─Token(Identifier) |nestedDiscriminatorMissingKey|
//@[076:00077) |   | ├─Token(Dot) |.|
//@[077:00087) |   | └─IdentifierSyntax
//@[077:00087) |   |   └─Token(Identifier) |properties|
//@[087:00088) |   ├─Token(Dot) |.|
//@[088:00090) |   └─IdentifierSyntax
//@[088:00090) |     └─Token(Identifier) |cr|
//@[090:00092) ├─Token(NewLine) |\r\n|
// #completionTest(92) -> createMode
//@[036:00038) ├─Token(NewLine) |\r\n|
var nestedDiscriminatorMissingKeyCompletions2 = nestedDiscriminatorMissingKey['properties'].
//@[000:00092) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00045) | ├─IdentifierSyntax
//@[004:00045) | | └─Token(Identifier) |nestedDiscriminatorMissingKeyCompletions2|
//@[046:00047) | ├─Token(Assignment) |=|
//@[048:00092) | └─PropertyAccessSyntax
//@[048:00091) |   ├─ArrayAccessSyntax
//@[048:00077) |   | ├─VariableAccessSyntax
//@[048:00077) |   | | └─IdentifierSyntax
//@[048:00077) |   | |   └─Token(Identifier) |nestedDiscriminatorMissingKey|
//@[077:00078) |   | ├─Token(LeftSquare) |[|
//@[078:00090) |   | ├─StringSyntax
//@[078:00090) |   | | └─Token(StringComplete) |'properties'|
//@[090:00091) |   | └─Token(RightSquare) |]|
//@[091:00092) |   ├─Token(Dot) |.|
//@[092:00092) |   └─IdentifierSyntax
//@[092:00092) |     └─SkippedTriviaSyntax
//@[092:00096) ├─Token(NewLine) |\r\n\r\n|

// #completionTest(94) -> createModeIndexPlusSymbols
//@[052:00054) ├─Token(NewLine) |\r\n|
var nestedDiscriminatorMissingKeyIndexCompletions = nestedDiscriminatorMissingKey.properties['']
//@[000:00096) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00049) | ├─IdentifierSyntax
//@[004:00049) | | └─Token(Identifier) |nestedDiscriminatorMissingKeyIndexCompletions|
//@[050:00051) | ├─Token(Assignment) |=|
//@[052:00096) | └─ArrayAccessSyntax
//@[052:00092) |   ├─PropertyAccessSyntax
//@[052:00081) |   | ├─VariableAccessSyntax
//@[052:00081) |   | | └─IdentifierSyntax
//@[052:00081) |   | |   └─Token(Identifier) |nestedDiscriminatorMissingKey|
//@[081:00082) |   | ├─Token(Dot) |.|
//@[082:00092) |   | └─IdentifierSyntax
//@[082:00092) |   |   └─Token(Identifier) |properties|
//@[092:00093) |   ├─Token(LeftSquare) |[|
//@[093:00095) |   ├─StringSyntax
//@[093:00095) |   | └─Token(StringComplete) |''|
//@[095:00096) |   └─Token(RightSquare) |]|
//@[096:00100) ├─Token(NewLine) |\r\n\r\n|

/* 
Nested discriminator missing key (conditional)
*/
//@[002:00004) ├─Token(NewLine) |\r\n|
resource nestedDiscriminatorMissingKey_if 'Microsoft.DocumentDB/databaseAccounts@2020-06-01-preview' = if(bool(1)) {
//@[000:00205) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00041) | ├─IdentifierSyntax
//@[009:00041) | | └─Token(Identifier) |nestedDiscriminatorMissingKey_if|
//@[042:00100) | ├─StringSyntax
//@[042:00100) | | └─Token(StringComplete) |'Microsoft.DocumentDB/databaseAccounts@2020-06-01-preview'|
//@[101:00102) | ├─Token(Assignment) |=|
//@[103:00205) | └─IfConditionSyntax
//@[103:00105) |   ├─Token(Identifier) |if|
//@[105:00114) |   ├─ParenthesizedExpressionSyntax
//@[105:00106) |   | ├─Token(LeftParen) |(|
//@[106:00113) |   | ├─FunctionCallSyntax
//@[106:00110) |   | | ├─IdentifierSyntax
//@[106:00110) |   | | | └─Token(Identifier) |bool|
//@[110:00111) |   | | ├─Token(LeftParen) |(|
//@[111:00112) |   | | ├─FunctionArgumentSyntax
//@[111:00112) |   | | | └─IntegerLiteralSyntax
//@[111:00112) |   | | |   └─Token(Integer) |1|
//@[112:00113) |   | | └─Token(RightParen) |)|
//@[113:00114) |   | └─Token(RightParen) |)|
//@[115:00205) |   └─ObjectSyntax
//@[115:00116) |     ├─Token(LeftBrace) |{|
//@[116:00118) |     ├─Token(NewLine) |\r\n|
  name: 'test'
//@[002:00014) |     ├─ObjectPropertySyntax
//@[002:00006) |     | ├─IdentifierSyntax
//@[002:00006) |     | | └─Token(Identifier) |name|
//@[006:00007) |     | ├─Token(Colon) |:|
//@[008:00014) |     | └─StringSyntax
//@[008:00014) |     |   └─Token(StringComplete) |'test'|
//@[014:00016) |     ├─Token(NewLine) |\r\n|
  location: 'l'
//@[002:00015) |     ├─ObjectPropertySyntax
//@[002:00010) |     | ├─IdentifierSyntax
//@[002:00010) |     | | └─Token(Identifier) |location|
//@[010:00011) |     | ├─Token(Colon) |:|
//@[012:00015) |     | └─StringSyntax
//@[012:00015) |     |   └─Token(StringComplete) |'l'|
//@[015:00017) |     ├─Token(NewLine) |\r\n|
  properties: {
//@[002:00051) |     ├─ObjectPropertySyntax
//@[002:00012) |     | ├─IdentifierSyntax
//@[002:00012) |     | | └─Token(Identifier) |properties|
//@[012:00013) |     | ├─Token(Colon) |:|
//@[014:00051) |     | └─ObjectSyntax
//@[014:00015) |     |   ├─Token(LeftBrace) |{|
//@[015:00017) |     |   ├─Token(NewLine) |\r\n|
    //createMode: 'Default'
//@[027:00031) |     |   ├─Token(NewLine) |\r\n\r\n|

  }
//@[002:00003) |     |   └─Token(RightBrace) |}|
//@[003:00005) |     ├─Token(NewLine) |\r\n|
}
//@[000:00001) |     └─Token(RightBrace) |}|
//@[001:00003) ├─Token(NewLine) |\r\n|
// #completionTest(96) -> createMode
//@[036:00038) ├─Token(NewLine) |\r\n|
var nestedDiscriminatorMissingKeyCompletions_if = nestedDiscriminatorMissingKey_if.properties.cr
//@[000:00096) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00047) | ├─IdentifierSyntax
//@[004:00047) | | └─Token(Identifier) |nestedDiscriminatorMissingKeyCompletions_if|
//@[048:00049) | ├─Token(Assignment) |=|
//@[050:00096) | └─PropertyAccessSyntax
//@[050:00093) |   ├─PropertyAccessSyntax
//@[050:00082) |   | ├─VariableAccessSyntax
//@[050:00082) |   | | └─IdentifierSyntax
//@[050:00082) |   | |   └─Token(Identifier) |nestedDiscriminatorMissingKey_if|
//@[082:00083) |   | ├─Token(Dot) |.|
//@[083:00093) |   | └─IdentifierSyntax
//@[083:00093) |   |   └─Token(Identifier) |properties|
//@[093:00094) |   ├─Token(Dot) |.|
//@[094:00096) |   └─IdentifierSyntax
//@[094:00096) |     └─Token(Identifier) |cr|
//@[096:00098) ├─Token(NewLine) |\r\n|
// #completionTest(98) -> createMode
//@[036:00038) ├─Token(NewLine) |\r\n|
var nestedDiscriminatorMissingKeyCompletions2_if = nestedDiscriminatorMissingKey_if['properties'].
//@[000:00098) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00048) | ├─IdentifierSyntax
//@[004:00048) | | └─Token(Identifier) |nestedDiscriminatorMissingKeyCompletions2_if|
//@[049:00050) | ├─Token(Assignment) |=|
//@[051:00098) | └─PropertyAccessSyntax
//@[051:00097) |   ├─ArrayAccessSyntax
//@[051:00083) |   | ├─VariableAccessSyntax
//@[051:00083) |   | | └─IdentifierSyntax
//@[051:00083) |   | |   └─Token(Identifier) |nestedDiscriminatorMissingKey_if|
//@[083:00084) |   | ├─Token(LeftSquare) |[|
//@[084:00096) |   | ├─StringSyntax
//@[084:00096) |   | | └─Token(StringComplete) |'properties'|
//@[096:00097) |   | └─Token(RightSquare) |]|
//@[097:00098) |   ├─Token(Dot) |.|
//@[098:00098) |   └─IdentifierSyntax
//@[098:00098) |     └─SkippedTriviaSyntax
//@[098:00102) ├─Token(NewLine) |\r\n\r\n|

// #completionTest(100) -> createModeIndexPlusSymbols_if
//@[056:00058) ├─Token(NewLine) |\r\n|
var nestedDiscriminatorMissingKeyIndexCompletions_if = nestedDiscriminatorMissingKey_if.properties['']
//@[000:00102) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00052) | ├─IdentifierSyntax
//@[004:00052) | | └─Token(Identifier) |nestedDiscriminatorMissingKeyIndexCompletions_if|
//@[053:00054) | ├─Token(Assignment) |=|
//@[055:00102) | └─ArrayAccessSyntax
//@[055:00098) |   ├─PropertyAccessSyntax
//@[055:00087) |   | ├─VariableAccessSyntax
//@[055:00087) |   | | └─IdentifierSyntax
//@[055:00087) |   | |   └─Token(Identifier) |nestedDiscriminatorMissingKey_if|
//@[087:00088) |   | ├─Token(Dot) |.|
//@[088:00098) |   | └─IdentifierSyntax
//@[088:00098) |   |   └─Token(Identifier) |properties|
//@[098:00099) |   ├─Token(LeftSquare) |[|
//@[099:00101) |   ├─StringSyntax
//@[099:00101) |   | └─Token(StringComplete) |''|
//@[101:00102) |   └─Token(RightSquare) |]|
//@[102:00106) ├─Token(NewLine) |\r\n\r\n|

/* 
Nested discriminator missing key (loop)
*/
//@[002:00004) ├─Token(NewLine) |\r\n|
resource nestedDiscriminatorMissingKey_for 'Microsoft.DocumentDB/databaseAccounts@2020-06-01-preview' = [for thing in []: {
//@[000:00213) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00042) | ├─IdentifierSyntax
//@[009:00042) | | └─Token(Identifier) |nestedDiscriminatorMissingKey_for|
//@[043:00101) | ├─StringSyntax
//@[043:00101) | | └─Token(StringComplete) |'Microsoft.DocumentDB/databaseAccounts@2020-06-01-preview'|
//@[102:00103) | ├─Token(Assignment) |=|
//@[104:00213) | └─ForSyntax
//@[104:00105) |   ├─Token(LeftSquare) |[|
//@[105:00108) |   ├─Token(Identifier) |for|
//@[109:00114) |   ├─LocalVariableSyntax
//@[109:00114) |   | └─IdentifierSyntax
//@[109:00114) |   |   └─Token(Identifier) |thing|
//@[115:00117) |   ├─Token(Identifier) |in|
//@[118:00120) |   ├─ArraySyntax
//@[118:00119) |   | ├─Token(LeftSquare) |[|
//@[119:00120) |   | └─Token(RightSquare) |]|
//@[120:00121) |   ├─Token(Colon) |:|
//@[122:00212) |   ├─ObjectSyntax
//@[122:00123) |   | ├─Token(LeftBrace) |{|
//@[123:00125) |   | ├─Token(NewLine) |\r\n|
  name: 'test'
//@[002:00014) |   | ├─ObjectPropertySyntax
//@[002:00006) |   | | ├─IdentifierSyntax
//@[002:00006) |   | | | └─Token(Identifier) |name|
//@[006:00007) |   | | ├─Token(Colon) |:|
//@[008:00014) |   | | └─StringSyntax
//@[008:00014) |   | |   └─Token(StringComplete) |'test'|
//@[014:00016) |   | ├─Token(NewLine) |\r\n|
  location: 'l'
//@[002:00015) |   | ├─ObjectPropertySyntax
//@[002:00010) |   | | ├─IdentifierSyntax
//@[002:00010) |   | | | └─Token(Identifier) |location|
//@[010:00011) |   | | ├─Token(Colon) |:|
//@[012:00015) |   | | └─StringSyntax
//@[012:00015) |   | |   └─Token(StringComplete) |'l'|
//@[015:00017) |   | ├─Token(NewLine) |\r\n|
  properties: {
//@[002:00051) |   | ├─ObjectPropertySyntax
//@[002:00012) |   | | ├─IdentifierSyntax
//@[002:00012) |   | | | └─Token(Identifier) |properties|
//@[012:00013) |   | | ├─Token(Colon) |:|
//@[014:00051) |   | | └─ObjectSyntax
//@[014:00015) |   | |   ├─Token(LeftBrace) |{|
//@[015:00017) |   | |   ├─Token(NewLine) |\r\n|
    //createMode: 'Default'
//@[027:00031) |   | |   ├─Token(NewLine) |\r\n\r\n|

  }
//@[002:00003) |   | |   └─Token(RightBrace) |}|
//@[003:00005) |   | ├─Token(NewLine) |\r\n|
}]
//@[000:00001) |   | └─Token(RightBrace) |}|
//@[001:00002) |   └─Token(RightSquare) |]|
//@[002:00004) ├─Token(NewLine) |\r\n|
// #completionTest(101) -> createMode
//@[037:00039) ├─Token(NewLine) |\r\n|
var nestedDiscriminatorMissingKeyCompletions_for = nestedDiscriminatorMissingKey_for[0].properties.cr
//@[000:00101) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00048) | ├─IdentifierSyntax
//@[004:00048) | | └─Token(Identifier) |nestedDiscriminatorMissingKeyCompletions_for|
//@[049:00050) | ├─Token(Assignment) |=|
//@[051:00101) | └─PropertyAccessSyntax
//@[051:00098) |   ├─PropertyAccessSyntax
//@[051:00087) |   | ├─ArrayAccessSyntax
//@[051:00084) |   | | ├─VariableAccessSyntax
//@[051:00084) |   | | | └─IdentifierSyntax
//@[051:00084) |   | | |   └─Token(Identifier) |nestedDiscriminatorMissingKey_for|
//@[084:00085) |   | | ├─Token(LeftSquare) |[|
//@[085:00086) |   | | ├─IntegerLiteralSyntax
//@[085:00086) |   | | | └─Token(Integer) |0|
//@[086:00087) |   | | └─Token(RightSquare) |]|
//@[087:00088) |   | ├─Token(Dot) |.|
//@[088:00098) |   | └─IdentifierSyntax
//@[088:00098) |   |   └─Token(Identifier) |properties|
//@[098:00099) |   ├─Token(Dot) |.|
//@[099:00101) |   └─IdentifierSyntax
//@[099:00101) |     └─Token(Identifier) |cr|
//@[101:00103) ├─Token(NewLine) |\r\n|
// #completionTest(103) -> createMode
//@[037:00039) ├─Token(NewLine) |\r\n|
var nestedDiscriminatorMissingKeyCompletions2_for = nestedDiscriminatorMissingKey_for[0]['properties'].
//@[000:00103) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00049) | ├─IdentifierSyntax
//@[004:00049) | | └─Token(Identifier) |nestedDiscriminatorMissingKeyCompletions2_for|
//@[050:00051) | ├─Token(Assignment) |=|
//@[052:00103) | └─PropertyAccessSyntax
//@[052:00102) |   ├─ArrayAccessSyntax
//@[052:00088) |   | ├─ArrayAccessSyntax
//@[052:00085) |   | | ├─VariableAccessSyntax
//@[052:00085) |   | | | └─IdentifierSyntax
//@[052:00085) |   | | |   └─Token(Identifier) |nestedDiscriminatorMissingKey_for|
//@[085:00086) |   | | ├─Token(LeftSquare) |[|
//@[086:00087) |   | | ├─IntegerLiteralSyntax
//@[086:00087) |   | | | └─Token(Integer) |0|
//@[087:00088) |   | | └─Token(RightSquare) |]|
//@[088:00089) |   | ├─Token(LeftSquare) |[|
//@[089:00101) |   | ├─StringSyntax
//@[089:00101) |   | | └─Token(StringComplete) |'properties'|
//@[101:00102) |   | └─Token(RightSquare) |]|
//@[102:00103) |   ├─Token(Dot) |.|
//@[103:00103) |   └─IdentifierSyntax
//@[103:00103) |     └─SkippedTriviaSyntax
//@[103:00107) ├─Token(NewLine) |\r\n\r\n|

// #completionTest(105) -> createModeIndexPlusSymbols_for
//@[057:00059) ├─Token(NewLine) |\r\n|
var nestedDiscriminatorMissingKeyIndexCompletions_for = nestedDiscriminatorMissingKey_for[0].properties['']
//@[000:00107) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00053) | ├─IdentifierSyntax
//@[004:00053) | | └─Token(Identifier) |nestedDiscriminatorMissingKeyIndexCompletions_for|
//@[054:00055) | ├─Token(Assignment) |=|
//@[056:00107) | └─ArrayAccessSyntax
//@[056:00103) |   ├─PropertyAccessSyntax
//@[056:00092) |   | ├─ArrayAccessSyntax
//@[056:00089) |   | | ├─VariableAccessSyntax
//@[056:00089) |   | | | └─IdentifierSyntax
//@[056:00089) |   | | |   └─Token(Identifier) |nestedDiscriminatorMissingKey_for|
//@[089:00090) |   | | ├─Token(LeftSquare) |[|
//@[090:00091) |   | | ├─IntegerLiteralSyntax
//@[090:00091) |   | | | └─Token(Integer) |0|
//@[091:00092) |   | | └─Token(RightSquare) |]|
//@[092:00093) |   | ├─Token(Dot) |.|
//@[093:00103) |   | └─IdentifierSyntax
//@[093:00103) |   |   └─Token(Identifier) |properties|
//@[103:00104) |   ├─Token(LeftSquare) |[|
//@[104:00106) |   ├─StringSyntax
//@[104:00106) |   | └─Token(StringComplete) |''|
//@[106:00107) |   └─Token(RightSquare) |]|
//@[107:00113) ├─Token(NewLine) |\r\n\r\n\r\n|


/* 
Nested discriminator missing key (filtered loop)
*/
//@[002:00004) ├─Token(NewLine) |\r\n|
resource nestedDiscriminatorMissingKey_for_if 'Microsoft.DocumentDB/databaseAccounts@2020-06-01-preview' = [for thing in []: if(true) {
//@[000:00225) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00045) | ├─IdentifierSyntax
//@[009:00045) | | └─Token(Identifier) |nestedDiscriminatorMissingKey_for_if|
//@[046:00104) | ├─StringSyntax
//@[046:00104) | | └─Token(StringComplete) |'Microsoft.DocumentDB/databaseAccounts@2020-06-01-preview'|
//@[105:00106) | ├─Token(Assignment) |=|
//@[107:00225) | └─ForSyntax
//@[107:00108) |   ├─Token(LeftSquare) |[|
//@[108:00111) |   ├─Token(Identifier) |for|
//@[112:00117) |   ├─LocalVariableSyntax
//@[112:00117) |   | └─IdentifierSyntax
//@[112:00117) |   |   └─Token(Identifier) |thing|
//@[118:00120) |   ├─Token(Identifier) |in|
//@[121:00123) |   ├─ArraySyntax
//@[121:00122) |   | ├─Token(LeftSquare) |[|
//@[122:00123) |   | └─Token(RightSquare) |]|
//@[123:00124) |   ├─Token(Colon) |:|
//@[125:00224) |   ├─IfConditionSyntax
//@[125:00127) |   | ├─Token(Identifier) |if|
//@[127:00133) |   | ├─ParenthesizedExpressionSyntax
//@[127:00128) |   | | ├─Token(LeftParen) |(|
//@[128:00132) |   | | ├─BooleanLiteralSyntax
//@[128:00132) |   | | | └─Token(TrueKeyword) |true|
//@[132:00133) |   | | └─Token(RightParen) |)|
//@[134:00224) |   | └─ObjectSyntax
//@[134:00135) |   |   ├─Token(LeftBrace) |{|
//@[135:00137) |   |   ├─Token(NewLine) |\r\n|
  name: 'test'
//@[002:00014) |   |   ├─ObjectPropertySyntax
//@[002:00006) |   |   | ├─IdentifierSyntax
//@[002:00006) |   |   | | └─Token(Identifier) |name|
//@[006:00007) |   |   | ├─Token(Colon) |:|
//@[008:00014) |   |   | └─StringSyntax
//@[008:00014) |   |   |   └─Token(StringComplete) |'test'|
//@[014:00016) |   |   ├─Token(NewLine) |\r\n|
  location: 'l'
//@[002:00015) |   |   ├─ObjectPropertySyntax
//@[002:00010) |   |   | ├─IdentifierSyntax
//@[002:00010) |   |   | | └─Token(Identifier) |location|
//@[010:00011) |   |   | ├─Token(Colon) |:|
//@[012:00015) |   |   | └─StringSyntax
//@[012:00015) |   |   |   └─Token(StringComplete) |'l'|
//@[015:00017) |   |   ├─Token(NewLine) |\r\n|
  properties: {
//@[002:00051) |   |   ├─ObjectPropertySyntax
//@[002:00012) |   |   | ├─IdentifierSyntax
//@[002:00012) |   |   | | └─Token(Identifier) |properties|
//@[012:00013) |   |   | ├─Token(Colon) |:|
//@[014:00051) |   |   | └─ObjectSyntax
//@[014:00015) |   |   |   ├─Token(LeftBrace) |{|
//@[015:00017) |   |   |   ├─Token(NewLine) |\r\n|
    //createMode: 'Default'
//@[027:00031) |   |   |   ├─Token(NewLine) |\r\n\r\n|

  }
//@[002:00003) |   |   |   └─Token(RightBrace) |}|
//@[003:00005) |   |   ├─Token(NewLine) |\r\n|
}]
//@[000:00001) |   |   └─Token(RightBrace) |}|
//@[001:00002) |   └─Token(RightSquare) |]|
//@[002:00004) ├─Token(NewLine) |\r\n|
// #completionTest(107) -> createMode
//@[037:00039) ├─Token(NewLine) |\r\n|
var nestedDiscriminatorMissingKeyCompletions_for_if = nestedDiscriminatorMissingKey_for_if[0].properties.cr
//@[000:00107) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00051) | ├─IdentifierSyntax
//@[004:00051) | | └─Token(Identifier) |nestedDiscriminatorMissingKeyCompletions_for_if|
//@[052:00053) | ├─Token(Assignment) |=|
//@[054:00107) | └─PropertyAccessSyntax
//@[054:00104) |   ├─PropertyAccessSyntax
//@[054:00093) |   | ├─ArrayAccessSyntax
//@[054:00090) |   | | ├─VariableAccessSyntax
//@[054:00090) |   | | | └─IdentifierSyntax
//@[054:00090) |   | | |   └─Token(Identifier) |nestedDiscriminatorMissingKey_for_if|
//@[090:00091) |   | | ├─Token(LeftSquare) |[|
//@[091:00092) |   | | ├─IntegerLiteralSyntax
//@[091:00092) |   | | | └─Token(Integer) |0|
//@[092:00093) |   | | └─Token(RightSquare) |]|
//@[093:00094) |   | ├─Token(Dot) |.|
//@[094:00104) |   | └─IdentifierSyntax
//@[094:00104) |   |   └─Token(Identifier) |properties|
//@[104:00105) |   ├─Token(Dot) |.|
//@[105:00107) |   └─IdentifierSyntax
//@[105:00107) |     └─Token(Identifier) |cr|
//@[107:00109) ├─Token(NewLine) |\r\n|
// #completionTest(109) -> createMode
//@[037:00039) ├─Token(NewLine) |\r\n|
var nestedDiscriminatorMissingKeyCompletions2_for_if = nestedDiscriminatorMissingKey_for_if[0]['properties'].
//@[000:00109) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00052) | ├─IdentifierSyntax
//@[004:00052) | | └─Token(Identifier) |nestedDiscriminatorMissingKeyCompletions2_for_if|
//@[053:00054) | ├─Token(Assignment) |=|
//@[055:00109) | └─PropertyAccessSyntax
//@[055:00108) |   ├─ArrayAccessSyntax
//@[055:00094) |   | ├─ArrayAccessSyntax
//@[055:00091) |   | | ├─VariableAccessSyntax
//@[055:00091) |   | | | └─IdentifierSyntax
//@[055:00091) |   | | |   └─Token(Identifier) |nestedDiscriminatorMissingKey_for_if|
//@[091:00092) |   | | ├─Token(LeftSquare) |[|
//@[092:00093) |   | | ├─IntegerLiteralSyntax
//@[092:00093) |   | | | └─Token(Integer) |0|
//@[093:00094) |   | | └─Token(RightSquare) |]|
//@[094:00095) |   | ├─Token(LeftSquare) |[|
//@[095:00107) |   | ├─StringSyntax
//@[095:00107) |   | | └─Token(StringComplete) |'properties'|
//@[107:00108) |   | └─Token(RightSquare) |]|
//@[108:00109) |   ├─Token(Dot) |.|
//@[109:00109) |   └─IdentifierSyntax
//@[109:00109) |     └─SkippedTriviaSyntax
//@[109:00113) ├─Token(NewLine) |\r\n\r\n|

// #completionTest(111) -> createModeIndexPlusSymbols_for_if
//@[060:00062) ├─Token(NewLine) |\r\n|
var nestedDiscriminatorMissingKeyIndexCompletions_for_if = nestedDiscriminatorMissingKey_for_if[0].properties['']
//@[000:00113) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00056) | ├─IdentifierSyntax
//@[004:00056) | | └─Token(Identifier) |nestedDiscriminatorMissingKeyIndexCompletions_for_if|
//@[057:00058) | ├─Token(Assignment) |=|
//@[059:00113) | └─ArrayAccessSyntax
//@[059:00109) |   ├─PropertyAccessSyntax
//@[059:00098) |   | ├─ArrayAccessSyntax
//@[059:00095) |   | | ├─VariableAccessSyntax
//@[059:00095) |   | | | └─IdentifierSyntax
//@[059:00095) |   | | |   └─Token(Identifier) |nestedDiscriminatorMissingKey_for_if|
//@[095:00096) |   | | ├─Token(LeftSquare) |[|
//@[096:00097) |   | | ├─IntegerLiteralSyntax
//@[096:00097) |   | | | └─Token(Integer) |0|
//@[097:00098) |   | | └─Token(RightSquare) |]|
//@[098:00099) |   | ├─Token(Dot) |.|
//@[099:00109) |   | └─IdentifierSyntax
//@[099:00109) |   |   └─Token(Identifier) |properties|
//@[109:00110) |   ├─Token(LeftSquare) |[|
//@[110:00112) |   ├─StringSyntax
//@[110:00112) |   | └─Token(StringComplete) |''|
//@[112:00113) |   └─Token(RightSquare) |]|
//@[113:00119) ├─Token(NewLine) |\r\n\r\n\r\n|


/*
Nested discriminator
*/
//@[002:00004) ├─Token(NewLine) |\r\n|
resource nestedDiscriminator 'Microsoft.DocumentDB/databaseAccounts@2020-06-01-preview' = {
//@[000:00178) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00028) | ├─IdentifierSyntax
//@[009:00028) | | └─Token(Identifier) |nestedDiscriminator|
//@[029:00087) | ├─StringSyntax
//@[029:00087) | | └─Token(StringComplete) |'Microsoft.DocumentDB/databaseAccounts@2020-06-01-preview'|
//@[088:00089) | ├─Token(Assignment) |=|
//@[090:00178) | └─ObjectSyntax
//@[090:00091) |   ├─Token(LeftBrace) |{|
//@[091:00093) |   ├─Token(NewLine) |\r\n|
  name: 'test'
//@[002:00014) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00014) |   | └─StringSyntax
//@[008:00014) |   |   └─Token(StringComplete) |'test'|
//@[014:00016) |   ├─Token(NewLine) |\r\n|
  location: 'l'
//@[002:00015) |   ├─ObjectPropertySyntax
//@[002:00010) |   | ├─IdentifierSyntax
//@[002:00010) |   | | └─Token(Identifier) |location|
//@[010:00011) |   | ├─Token(Colon) |:|
//@[012:00015) |   | └─StringSyntax
//@[012:00015) |   |   └─Token(StringComplete) |'l'|
//@[015:00017) |   ├─Token(NewLine) |\r\n|
  properties: {
//@[002:00049) |   ├─ObjectPropertySyntax
//@[002:00012) |   | ├─IdentifierSyntax
//@[002:00012) |   | | └─Token(Identifier) |properties|
//@[012:00013) |   | ├─Token(Colon) |:|
//@[014:00049) |   | └─ObjectSyntax
//@[014:00015) |   |   ├─Token(LeftBrace) |{|
//@[015:00017) |   |   ├─Token(NewLine) |\r\n|
    createMode: 'Default'
//@[004:00025) |   |   ├─ObjectPropertySyntax
//@[004:00014) |   |   | ├─IdentifierSyntax
//@[004:00014) |   |   | | └─Token(Identifier) |createMode|
//@[014:00015) |   |   | ├─Token(Colon) |:|
//@[016:00025) |   |   | └─StringSyntax
//@[016:00025) |   |   |   └─Token(StringComplete) |'Default'|
//@[025:00029) |   |   ├─Token(NewLine) |\r\n\r\n|

  }
//@[002:00003) |   |   └─Token(RightBrace) |}|
//@[003:00005) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00003) ├─Token(NewLine) |\r\n|
// #completionTest(69) -> defaultCreateModeProperties
//@[053:00055) ├─Token(NewLine) |\r\n|
var nestedDiscriminatorCompletions = nestedDiscriminator.properties.a
//@[000:00069) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00034) | ├─IdentifierSyntax
//@[004:00034) | | └─Token(Identifier) |nestedDiscriminatorCompletions|
//@[035:00036) | ├─Token(Assignment) |=|
//@[037:00069) | └─PropertyAccessSyntax
//@[037:00067) |   ├─PropertyAccessSyntax
//@[037:00056) |   | ├─VariableAccessSyntax
//@[037:00056) |   | | └─IdentifierSyntax
//@[037:00056) |   | |   └─Token(Identifier) |nestedDiscriminator|
//@[056:00057) |   | ├─Token(Dot) |.|
//@[057:00067) |   | └─IdentifierSyntax
//@[057:00067) |   |   └─Token(Identifier) |properties|
//@[067:00068) |   ├─Token(Dot) |.|
//@[068:00069) |   └─IdentifierSyntax
//@[068:00069) |     └─Token(Identifier) |a|
//@[069:00071) ├─Token(NewLine) |\r\n|
// #completionTest(73) -> defaultCreateModeProperties
//@[053:00055) ├─Token(NewLine) |\r\n|
var nestedDiscriminatorCompletions2 = nestedDiscriminator['properties'].a
//@[000:00073) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00035) | ├─IdentifierSyntax
//@[004:00035) | | └─Token(Identifier) |nestedDiscriminatorCompletions2|
//@[036:00037) | ├─Token(Assignment) |=|
//@[038:00073) | └─PropertyAccessSyntax
//@[038:00071) |   ├─ArrayAccessSyntax
//@[038:00057) |   | ├─VariableAccessSyntax
//@[038:00057) |   | | └─IdentifierSyntax
//@[038:00057) |   | |   └─Token(Identifier) |nestedDiscriminator|
//@[057:00058) |   | ├─Token(LeftSquare) |[|
//@[058:00070) |   | ├─StringSyntax
//@[058:00070) |   | | └─Token(StringComplete) |'properties'|
//@[070:00071) |   | └─Token(RightSquare) |]|
//@[071:00072) |   ├─Token(Dot) |.|
//@[072:00073) |   └─IdentifierSyntax
//@[072:00073) |     └─Token(Identifier) |a|
//@[073:00075) ├─Token(NewLine) |\r\n|
// #completionTest(69) -> defaultCreateModeProperties
//@[053:00055) ├─Token(NewLine) |\r\n|
var nestedDiscriminatorCompletions3 = nestedDiscriminator.properties.
//@[000:00069) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00035) | ├─IdentifierSyntax
//@[004:00035) | | └─Token(Identifier) |nestedDiscriminatorCompletions3|
//@[036:00037) | ├─Token(Assignment) |=|
//@[038:00069) | └─PropertyAccessSyntax
//@[038:00068) |   ├─PropertyAccessSyntax
//@[038:00057) |   | ├─VariableAccessSyntax
//@[038:00057) |   | | └─IdentifierSyntax
//@[038:00057) |   | |   └─Token(Identifier) |nestedDiscriminator|
//@[057:00058) |   | ├─Token(Dot) |.|
//@[058:00068) |   | └─IdentifierSyntax
//@[058:00068) |   |   └─Token(Identifier) |properties|
//@[068:00069) |   ├─Token(Dot) |.|
//@[069:00069) |   └─IdentifierSyntax
//@[069:00069) |     └─SkippedTriviaSyntax
//@[069:00071) ├─Token(NewLine) |\r\n|
// #completionTest(72) -> defaultCreateModeProperties
//@[053:00055) ├─Token(NewLine) |\r\n|
var nestedDiscriminatorCompletions4 = nestedDiscriminator['properties'].
//@[000:00072) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00035) | ├─IdentifierSyntax
//@[004:00035) | | └─Token(Identifier) |nestedDiscriminatorCompletions4|
//@[036:00037) | ├─Token(Assignment) |=|
//@[038:00072) | └─PropertyAccessSyntax
//@[038:00071) |   ├─ArrayAccessSyntax
//@[038:00057) |   | ├─VariableAccessSyntax
//@[038:00057) |   | | └─IdentifierSyntax
//@[038:00057) |   | |   └─Token(Identifier) |nestedDiscriminator|
//@[057:00058) |   | ├─Token(LeftSquare) |[|
//@[058:00070) |   | ├─StringSyntax
//@[058:00070) |   | | └─Token(StringComplete) |'properties'|
//@[070:00071) |   | └─Token(RightSquare) |]|
//@[071:00072) |   ├─Token(Dot) |.|
//@[072:00072) |   └─IdentifierSyntax
//@[072:00072) |     └─SkippedTriviaSyntax
//@[072:00076) ├─Token(NewLine) |\r\n\r\n|

// #completionTest(79) -> defaultCreateModeIndexes
//@[050:00052) ├─Token(NewLine) |\r\n|
var nestedDiscriminatorArrayIndexCompletions = nestedDiscriminator.properties[a]
//@[000:00080) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00044) | ├─IdentifierSyntax
//@[004:00044) | | └─Token(Identifier) |nestedDiscriminatorArrayIndexCompletions|
//@[045:00046) | ├─Token(Assignment) |=|
//@[047:00080) | └─ArrayAccessSyntax
//@[047:00077) |   ├─PropertyAccessSyntax
//@[047:00066) |   | ├─VariableAccessSyntax
//@[047:00066) |   | | └─IdentifierSyntax
//@[047:00066) |   | |   └─Token(Identifier) |nestedDiscriminator|
//@[066:00067) |   | ├─Token(Dot) |.|
//@[067:00077) |   | └─IdentifierSyntax
//@[067:00077) |   |   └─Token(Identifier) |properties|
//@[077:00078) |   ├─Token(LeftSquare) |[|
//@[078:00079) |   ├─VariableAccessSyntax
//@[078:00079) |   | └─IdentifierSyntax
//@[078:00079) |   |   └─Token(Identifier) |a|
//@[079:00080) |   └─Token(RightSquare) |]|
//@[080:00084) ├─Token(NewLine) |\r\n\r\n|

/*
Nested discriminator (conditional)
*/
//@[002:00004) ├─Token(NewLine) |\r\n|
resource nestedDiscriminator_if 'Microsoft.DocumentDB/databaseAccounts@2020-06-01-preview' = if(true) {
//@[000:00190) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00031) | ├─IdentifierSyntax
//@[009:00031) | | └─Token(Identifier) |nestedDiscriminator_if|
//@[032:00090) | ├─StringSyntax
//@[032:00090) | | └─Token(StringComplete) |'Microsoft.DocumentDB/databaseAccounts@2020-06-01-preview'|
//@[091:00092) | ├─Token(Assignment) |=|
//@[093:00190) | └─IfConditionSyntax
//@[093:00095) |   ├─Token(Identifier) |if|
//@[095:00101) |   ├─ParenthesizedExpressionSyntax
//@[095:00096) |   | ├─Token(LeftParen) |(|
//@[096:00100) |   | ├─BooleanLiteralSyntax
//@[096:00100) |   | | └─Token(TrueKeyword) |true|
//@[100:00101) |   | └─Token(RightParen) |)|
//@[102:00190) |   └─ObjectSyntax
//@[102:00103) |     ├─Token(LeftBrace) |{|
//@[103:00105) |     ├─Token(NewLine) |\r\n|
  name: 'test'
//@[002:00014) |     ├─ObjectPropertySyntax
//@[002:00006) |     | ├─IdentifierSyntax
//@[002:00006) |     | | └─Token(Identifier) |name|
//@[006:00007) |     | ├─Token(Colon) |:|
//@[008:00014) |     | └─StringSyntax
//@[008:00014) |     |   └─Token(StringComplete) |'test'|
//@[014:00016) |     ├─Token(NewLine) |\r\n|
  location: 'l'
//@[002:00015) |     ├─ObjectPropertySyntax
//@[002:00010) |     | ├─IdentifierSyntax
//@[002:00010) |     | | └─Token(Identifier) |location|
//@[010:00011) |     | ├─Token(Colon) |:|
//@[012:00015) |     | └─StringSyntax
//@[012:00015) |     |   └─Token(StringComplete) |'l'|
//@[015:00017) |     ├─Token(NewLine) |\r\n|
  properties: {
//@[002:00049) |     ├─ObjectPropertySyntax
//@[002:00012) |     | ├─IdentifierSyntax
//@[002:00012) |     | | └─Token(Identifier) |properties|
//@[012:00013) |     | ├─Token(Colon) |:|
//@[014:00049) |     | └─ObjectSyntax
//@[014:00015) |     |   ├─Token(LeftBrace) |{|
//@[015:00017) |     |   ├─Token(NewLine) |\r\n|
    createMode: 'Default'
//@[004:00025) |     |   ├─ObjectPropertySyntax
//@[004:00014) |     |   | ├─IdentifierSyntax
//@[004:00014) |     |   | | └─Token(Identifier) |createMode|
//@[014:00015) |     |   | ├─Token(Colon) |:|
//@[016:00025) |     |   | └─StringSyntax
//@[016:00025) |     |   |   └─Token(StringComplete) |'Default'|
//@[025:00029) |     |   ├─Token(NewLine) |\r\n\r\n|

  }
//@[002:00003) |     |   └─Token(RightBrace) |}|
//@[003:00005) |     ├─Token(NewLine) |\r\n|
}
//@[000:00001) |     └─Token(RightBrace) |}|
//@[001:00003) ├─Token(NewLine) |\r\n|
// #completionTest(75) -> defaultCreateModeProperties
//@[053:00055) ├─Token(NewLine) |\r\n|
var nestedDiscriminatorCompletions_if = nestedDiscriminator_if.properties.a
//@[000:00075) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00037) | ├─IdentifierSyntax
//@[004:00037) | | └─Token(Identifier) |nestedDiscriminatorCompletions_if|
//@[038:00039) | ├─Token(Assignment) |=|
//@[040:00075) | └─PropertyAccessSyntax
//@[040:00073) |   ├─PropertyAccessSyntax
//@[040:00062) |   | ├─VariableAccessSyntax
//@[040:00062) |   | | └─IdentifierSyntax
//@[040:00062) |   | |   └─Token(Identifier) |nestedDiscriminator_if|
//@[062:00063) |   | ├─Token(Dot) |.|
//@[063:00073) |   | └─IdentifierSyntax
//@[063:00073) |   |   └─Token(Identifier) |properties|
//@[073:00074) |   ├─Token(Dot) |.|
//@[074:00075) |   └─IdentifierSyntax
//@[074:00075) |     └─Token(Identifier) |a|
//@[075:00077) ├─Token(NewLine) |\r\n|
// #completionTest(79) -> defaultCreateModeProperties
//@[053:00055) ├─Token(NewLine) |\r\n|
var nestedDiscriminatorCompletions2_if = nestedDiscriminator_if['properties'].a
//@[000:00079) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00038) | ├─IdentifierSyntax
//@[004:00038) | | └─Token(Identifier) |nestedDiscriminatorCompletions2_if|
//@[039:00040) | ├─Token(Assignment) |=|
//@[041:00079) | └─PropertyAccessSyntax
//@[041:00077) |   ├─ArrayAccessSyntax
//@[041:00063) |   | ├─VariableAccessSyntax
//@[041:00063) |   | | └─IdentifierSyntax
//@[041:00063) |   | |   └─Token(Identifier) |nestedDiscriminator_if|
//@[063:00064) |   | ├─Token(LeftSquare) |[|
//@[064:00076) |   | ├─StringSyntax
//@[064:00076) |   | | └─Token(StringComplete) |'properties'|
//@[076:00077) |   | └─Token(RightSquare) |]|
//@[077:00078) |   ├─Token(Dot) |.|
//@[078:00079) |   └─IdentifierSyntax
//@[078:00079) |     └─Token(Identifier) |a|
//@[079:00081) ├─Token(NewLine) |\r\n|
// #completionTest(75) -> defaultCreateModeProperties
//@[053:00055) ├─Token(NewLine) |\r\n|
var nestedDiscriminatorCompletions3_if = nestedDiscriminator_if.properties.
//@[000:00075) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00038) | ├─IdentifierSyntax
//@[004:00038) | | └─Token(Identifier) |nestedDiscriminatorCompletions3_if|
//@[039:00040) | ├─Token(Assignment) |=|
//@[041:00075) | └─PropertyAccessSyntax
//@[041:00074) |   ├─PropertyAccessSyntax
//@[041:00063) |   | ├─VariableAccessSyntax
//@[041:00063) |   | | └─IdentifierSyntax
//@[041:00063) |   | |   └─Token(Identifier) |nestedDiscriminator_if|
//@[063:00064) |   | ├─Token(Dot) |.|
//@[064:00074) |   | └─IdentifierSyntax
//@[064:00074) |   |   └─Token(Identifier) |properties|
//@[074:00075) |   ├─Token(Dot) |.|
//@[075:00075) |   └─IdentifierSyntax
//@[075:00075) |     └─SkippedTriviaSyntax
//@[075:00077) ├─Token(NewLine) |\r\n|
// #completionTest(78) -> defaultCreateModeProperties
//@[053:00055) ├─Token(NewLine) |\r\n|
var nestedDiscriminatorCompletions4_if = nestedDiscriminator_if['properties'].
//@[000:00078) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00038) | ├─IdentifierSyntax
//@[004:00038) | | └─Token(Identifier) |nestedDiscriminatorCompletions4_if|
//@[039:00040) | ├─Token(Assignment) |=|
//@[041:00078) | └─PropertyAccessSyntax
//@[041:00077) |   ├─ArrayAccessSyntax
//@[041:00063) |   | ├─VariableAccessSyntax
//@[041:00063) |   | | └─IdentifierSyntax
//@[041:00063) |   | |   └─Token(Identifier) |nestedDiscriminator_if|
//@[063:00064) |   | ├─Token(LeftSquare) |[|
//@[064:00076) |   | ├─StringSyntax
//@[064:00076) |   | | └─Token(StringComplete) |'properties'|
//@[076:00077) |   | └─Token(RightSquare) |]|
//@[077:00078) |   ├─Token(Dot) |.|
//@[078:00078) |   └─IdentifierSyntax
//@[078:00078) |     └─SkippedTriviaSyntax
//@[078:00082) ├─Token(NewLine) |\r\n\r\n|

// #completionTest(85) -> defaultCreateModeIndexes_if
//@[053:00055) ├─Token(NewLine) |\r\n|
var nestedDiscriminatorArrayIndexCompletions_if = nestedDiscriminator_if.properties[a]
//@[000:00086) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00047) | ├─IdentifierSyntax
//@[004:00047) | | └─Token(Identifier) |nestedDiscriminatorArrayIndexCompletions_if|
//@[048:00049) | ├─Token(Assignment) |=|
//@[050:00086) | └─ArrayAccessSyntax
//@[050:00083) |   ├─PropertyAccessSyntax
//@[050:00072) |   | ├─VariableAccessSyntax
//@[050:00072) |   | | └─IdentifierSyntax
//@[050:00072) |   | |   └─Token(Identifier) |nestedDiscriminator_if|
//@[072:00073) |   | ├─Token(Dot) |.|
//@[073:00083) |   | └─IdentifierSyntax
//@[073:00083) |   |   └─Token(Identifier) |properties|
//@[083:00084) |   ├─Token(LeftSquare) |[|
//@[084:00085) |   ├─VariableAccessSyntax
//@[084:00085) |   | └─IdentifierSyntax
//@[084:00085) |   |   └─Token(Identifier) |a|
//@[085:00086) |   └─Token(RightSquare) |]|
//@[086:00092) ├─Token(NewLine) |\r\n\r\n\r\n|


/*
Nested discriminator (loop)
*/
//@[002:00004) ├─Token(NewLine) |\r\n|
resource nestedDiscriminator_for 'Microsoft.DocumentDB/databaseAccounts@2020-06-01-preview' = [for thing in []: {
//@[000:00201) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00032) | ├─IdentifierSyntax
//@[009:00032) | | └─Token(Identifier) |nestedDiscriminator_for|
//@[033:00091) | ├─StringSyntax
//@[033:00091) | | └─Token(StringComplete) |'Microsoft.DocumentDB/databaseAccounts@2020-06-01-preview'|
//@[092:00093) | ├─Token(Assignment) |=|
//@[094:00201) | └─ForSyntax
//@[094:00095) |   ├─Token(LeftSquare) |[|
//@[095:00098) |   ├─Token(Identifier) |for|
//@[099:00104) |   ├─LocalVariableSyntax
//@[099:00104) |   | └─IdentifierSyntax
//@[099:00104) |   |   └─Token(Identifier) |thing|
//@[105:00107) |   ├─Token(Identifier) |in|
//@[108:00110) |   ├─ArraySyntax
//@[108:00109) |   | ├─Token(LeftSquare) |[|
//@[109:00110) |   | └─Token(RightSquare) |]|
//@[110:00111) |   ├─Token(Colon) |:|
//@[112:00200) |   ├─ObjectSyntax
//@[112:00113) |   | ├─Token(LeftBrace) |{|
//@[113:00115) |   | ├─Token(NewLine) |\r\n|
  name: 'test'
//@[002:00014) |   | ├─ObjectPropertySyntax
//@[002:00006) |   | | ├─IdentifierSyntax
//@[002:00006) |   | | | └─Token(Identifier) |name|
//@[006:00007) |   | | ├─Token(Colon) |:|
//@[008:00014) |   | | └─StringSyntax
//@[008:00014) |   | |   └─Token(StringComplete) |'test'|
//@[014:00016) |   | ├─Token(NewLine) |\r\n|
  location: 'l'
//@[002:00015) |   | ├─ObjectPropertySyntax
//@[002:00010) |   | | ├─IdentifierSyntax
//@[002:00010) |   | | | └─Token(Identifier) |location|
//@[010:00011) |   | | ├─Token(Colon) |:|
//@[012:00015) |   | | └─StringSyntax
//@[012:00015) |   | |   └─Token(StringComplete) |'l'|
//@[015:00017) |   | ├─Token(NewLine) |\r\n|
  properties: {
//@[002:00049) |   | ├─ObjectPropertySyntax
//@[002:00012) |   | | ├─IdentifierSyntax
//@[002:00012) |   | | | └─Token(Identifier) |properties|
//@[012:00013) |   | | ├─Token(Colon) |:|
//@[014:00049) |   | | └─ObjectSyntax
//@[014:00015) |   | |   ├─Token(LeftBrace) |{|
//@[015:00017) |   | |   ├─Token(NewLine) |\r\n|
    createMode: 'Default'
//@[004:00025) |   | |   ├─ObjectPropertySyntax
//@[004:00014) |   | |   | ├─IdentifierSyntax
//@[004:00014) |   | |   | | └─Token(Identifier) |createMode|
//@[014:00015) |   | |   | ├─Token(Colon) |:|
//@[016:00025) |   | |   | └─StringSyntax
//@[016:00025) |   | |   |   └─Token(StringComplete) |'Default'|
//@[025:00029) |   | |   ├─Token(NewLine) |\r\n\r\n|

  }
//@[002:00003) |   | |   └─Token(RightBrace) |}|
//@[003:00005) |   | ├─Token(NewLine) |\r\n|
}]
//@[000:00001) |   | └─Token(RightBrace) |}|
//@[001:00002) |   └─Token(RightSquare) |]|
//@[002:00004) ├─Token(NewLine) |\r\n|
// #completionTest(80) -> defaultCreateModeProperties
//@[053:00055) ├─Token(NewLine) |\r\n|
var nestedDiscriminatorCompletions_for = nestedDiscriminator_for[0].properties.a
//@[000:00080) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00038) | ├─IdentifierSyntax
//@[004:00038) | | └─Token(Identifier) |nestedDiscriminatorCompletions_for|
//@[039:00040) | ├─Token(Assignment) |=|
//@[041:00080) | └─PropertyAccessSyntax
//@[041:00078) |   ├─PropertyAccessSyntax
//@[041:00067) |   | ├─ArrayAccessSyntax
//@[041:00064) |   | | ├─VariableAccessSyntax
//@[041:00064) |   | | | └─IdentifierSyntax
//@[041:00064) |   | | |   └─Token(Identifier) |nestedDiscriminator_for|
//@[064:00065) |   | | ├─Token(LeftSquare) |[|
//@[065:00066) |   | | ├─IntegerLiteralSyntax
//@[065:00066) |   | | | └─Token(Integer) |0|
//@[066:00067) |   | | └─Token(RightSquare) |]|
//@[067:00068) |   | ├─Token(Dot) |.|
//@[068:00078) |   | └─IdentifierSyntax
//@[068:00078) |   |   └─Token(Identifier) |properties|
//@[078:00079) |   ├─Token(Dot) |.|
//@[079:00080) |   └─IdentifierSyntax
//@[079:00080) |     └─Token(Identifier) |a|
//@[080:00082) ├─Token(NewLine) |\r\n|
// #completionTest(84) -> defaultCreateModeProperties
//@[053:00055) ├─Token(NewLine) |\r\n|
var nestedDiscriminatorCompletions2_for = nestedDiscriminator_for[0]['properties'].a
//@[000:00084) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00039) | ├─IdentifierSyntax
//@[004:00039) | | └─Token(Identifier) |nestedDiscriminatorCompletions2_for|
//@[040:00041) | ├─Token(Assignment) |=|
//@[042:00084) | └─PropertyAccessSyntax
//@[042:00082) |   ├─ArrayAccessSyntax
//@[042:00068) |   | ├─ArrayAccessSyntax
//@[042:00065) |   | | ├─VariableAccessSyntax
//@[042:00065) |   | | | └─IdentifierSyntax
//@[042:00065) |   | | |   └─Token(Identifier) |nestedDiscriminator_for|
//@[065:00066) |   | | ├─Token(LeftSquare) |[|
//@[066:00067) |   | | ├─IntegerLiteralSyntax
//@[066:00067) |   | | | └─Token(Integer) |0|
//@[067:00068) |   | | └─Token(RightSquare) |]|
//@[068:00069) |   | ├─Token(LeftSquare) |[|
//@[069:00081) |   | ├─StringSyntax
//@[069:00081) |   | | └─Token(StringComplete) |'properties'|
//@[081:00082) |   | └─Token(RightSquare) |]|
//@[082:00083) |   ├─Token(Dot) |.|
//@[083:00084) |   └─IdentifierSyntax
//@[083:00084) |     └─Token(Identifier) |a|
//@[084:00086) ├─Token(NewLine) |\r\n|
// #completionTest(80) -> defaultCreateModeProperties
//@[053:00055) ├─Token(NewLine) |\r\n|
var nestedDiscriminatorCompletions3_for = nestedDiscriminator_for[0].properties.
//@[000:00080) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00039) | ├─IdentifierSyntax
//@[004:00039) | | └─Token(Identifier) |nestedDiscriminatorCompletions3_for|
//@[040:00041) | ├─Token(Assignment) |=|
//@[042:00080) | └─PropertyAccessSyntax
//@[042:00079) |   ├─PropertyAccessSyntax
//@[042:00068) |   | ├─ArrayAccessSyntax
//@[042:00065) |   | | ├─VariableAccessSyntax
//@[042:00065) |   | | | └─IdentifierSyntax
//@[042:00065) |   | | |   └─Token(Identifier) |nestedDiscriminator_for|
//@[065:00066) |   | | ├─Token(LeftSquare) |[|
//@[066:00067) |   | | ├─IntegerLiteralSyntax
//@[066:00067) |   | | | └─Token(Integer) |0|
//@[067:00068) |   | | └─Token(RightSquare) |]|
//@[068:00069) |   | ├─Token(Dot) |.|
//@[069:00079) |   | └─IdentifierSyntax
//@[069:00079) |   |   └─Token(Identifier) |properties|
//@[079:00080) |   ├─Token(Dot) |.|
//@[080:00080) |   └─IdentifierSyntax
//@[080:00080) |     └─SkippedTriviaSyntax
//@[080:00082) ├─Token(NewLine) |\r\n|
// #completionTest(83) -> defaultCreateModeProperties
//@[053:00055) ├─Token(NewLine) |\r\n|
var nestedDiscriminatorCompletions4_for = nestedDiscriminator_for[0]['properties'].
//@[000:00083) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00039) | ├─IdentifierSyntax
//@[004:00039) | | └─Token(Identifier) |nestedDiscriminatorCompletions4_for|
//@[040:00041) | ├─Token(Assignment) |=|
//@[042:00083) | └─PropertyAccessSyntax
//@[042:00082) |   ├─ArrayAccessSyntax
//@[042:00068) |   | ├─ArrayAccessSyntax
//@[042:00065) |   | | ├─VariableAccessSyntax
//@[042:00065) |   | | | └─IdentifierSyntax
//@[042:00065) |   | | |   └─Token(Identifier) |nestedDiscriminator_for|
//@[065:00066) |   | | ├─Token(LeftSquare) |[|
//@[066:00067) |   | | ├─IntegerLiteralSyntax
//@[066:00067) |   | | | └─Token(Integer) |0|
//@[067:00068) |   | | └─Token(RightSquare) |]|
//@[068:00069) |   | ├─Token(LeftSquare) |[|
//@[069:00081) |   | ├─StringSyntax
//@[069:00081) |   | | └─Token(StringComplete) |'properties'|
//@[081:00082) |   | └─Token(RightSquare) |]|
//@[082:00083) |   ├─Token(Dot) |.|
//@[083:00083) |   └─IdentifierSyntax
//@[083:00083) |     └─SkippedTriviaSyntax
//@[083:00087) ├─Token(NewLine) |\r\n\r\n|

// #completionTest(90) -> defaultCreateModeIndexes_for
//@[054:00056) ├─Token(NewLine) |\r\n|
var nestedDiscriminatorArrayIndexCompletions_for = nestedDiscriminator_for[0].properties[a]
//@[000:00091) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00048) | ├─IdentifierSyntax
//@[004:00048) | | └─Token(Identifier) |nestedDiscriminatorArrayIndexCompletions_for|
//@[049:00050) | ├─Token(Assignment) |=|
//@[051:00091) | └─ArrayAccessSyntax
//@[051:00088) |   ├─PropertyAccessSyntax
//@[051:00077) |   | ├─ArrayAccessSyntax
//@[051:00074) |   | | ├─VariableAccessSyntax
//@[051:00074) |   | | | └─IdentifierSyntax
//@[051:00074) |   | | |   └─Token(Identifier) |nestedDiscriminator_for|
//@[074:00075) |   | | ├─Token(LeftSquare) |[|
//@[075:00076) |   | | ├─IntegerLiteralSyntax
//@[075:00076) |   | | | └─Token(Integer) |0|
//@[076:00077) |   | | └─Token(RightSquare) |]|
//@[077:00078) |   | ├─Token(Dot) |.|
//@[078:00088) |   | └─IdentifierSyntax
//@[078:00088) |   |   └─Token(Identifier) |properties|
//@[088:00089) |   ├─Token(LeftSquare) |[|
//@[089:00090) |   ├─VariableAccessSyntax
//@[089:00090) |   | └─IdentifierSyntax
//@[089:00090) |   |   └─Token(Identifier) |a|
//@[090:00091) |   └─Token(RightSquare) |]|
//@[091:00097) ├─Token(NewLine) |\r\n\r\n\r\n|


/*
Nested discriminator (filtered loop)
*/
//@[002:00004) ├─Token(NewLine) |\r\n|
resource nestedDiscriminator_for_if 'Microsoft.DocumentDB/databaseAccounts@2020-06-01-preview' = [for thing in []: if(true) {
//@[000:00213) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00035) | ├─IdentifierSyntax
//@[009:00035) | | └─Token(Identifier) |nestedDiscriminator_for_if|
//@[036:00094) | ├─StringSyntax
//@[036:00094) | | └─Token(StringComplete) |'Microsoft.DocumentDB/databaseAccounts@2020-06-01-preview'|
//@[095:00096) | ├─Token(Assignment) |=|
//@[097:00213) | └─ForSyntax
//@[097:00098) |   ├─Token(LeftSquare) |[|
//@[098:00101) |   ├─Token(Identifier) |for|
//@[102:00107) |   ├─LocalVariableSyntax
//@[102:00107) |   | └─IdentifierSyntax
//@[102:00107) |   |   └─Token(Identifier) |thing|
//@[108:00110) |   ├─Token(Identifier) |in|
//@[111:00113) |   ├─ArraySyntax
//@[111:00112) |   | ├─Token(LeftSquare) |[|
//@[112:00113) |   | └─Token(RightSquare) |]|
//@[113:00114) |   ├─Token(Colon) |:|
//@[115:00212) |   ├─IfConditionSyntax
//@[115:00117) |   | ├─Token(Identifier) |if|
//@[117:00123) |   | ├─ParenthesizedExpressionSyntax
//@[117:00118) |   | | ├─Token(LeftParen) |(|
//@[118:00122) |   | | ├─BooleanLiteralSyntax
//@[118:00122) |   | | | └─Token(TrueKeyword) |true|
//@[122:00123) |   | | └─Token(RightParen) |)|
//@[124:00212) |   | └─ObjectSyntax
//@[124:00125) |   |   ├─Token(LeftBrace) |{|
//@[125:00127) |   |   ├─Token(NewLine) |\r\n|
  name: 'test'
//@[002:00014) |   |   ├─ObjectPropertySyntax
//@[002:00006) |   |   | ├─IdentifierSyntax
//@[002:00006) |   |   | | └─Token(Identifier) |name|
//@[006:00007) |   |   | ├─Token(Colon) |:|
//@[008:00014) |   |   | └─StringSyntax
//@[008:00014) |   |   |   └─Token(StringComplete) |'test'|
//@[014:00016) |   |   ├─Token(NewLine) |\r\n|
  location: 'l'
//@[002:00015) |   |   ├─ObjectPropertySyntax
//@[002:00010) |   |   | ├─IdentifierSyntax
//@[002:00010) |   |   | | └─Token(Identifier) |location|
//@[010:00011) |   |   | ├─Token(Colon) |:|
//@[012:00015) |   |   | └─StringSyntax
//@[012:00015) |   |   |   └─Token(StringComplete) |'l'|
//@[015:00017) |   |   ├─Token(NewLine) |\r\n|
  properties: {
//@[002:00049) |   |   ├─ObjectPropertySyntax
//@[002:00012) |   |   | ├─IdentifierSyntax
//@[002:00012) |   |   | | └─Token(Identifier) |properties|
//@[012:00013) |   |   | ├─Token(Colon) |:|
//@[014:00049) |   |   | └─ObjectSyntax
//@[014:00015) |   |   |   ├─Token(LeftBrace) |{|
//@[015:00017) |   |   |   ├─Token(NewLine) |\r\n|
    createMode: 'Default'
//@[004:00025) |   |   |   ├─ObjectPropertySyntax
//@[004:00014) |   |   |   | ├─IdentifierSyntax
//@[004:00014) |   |   |   | | └─Token(Identifier) |createMode|
//@[014:00015) |   |   |   | ├─Token(Colon) |:|
//@[016:00025) |   |   |   | └─StringSyntax
//@[016:00025) |   |   |   |   └─Token(StringComplete) |'Default'|
//@[025:00029) |   |   |   ├─Token(NewLine) |\r\n\r\n|

  }
//@[002:00003) |   |   |   └─Token(RightBrace) |}|
//@[003:00005) |   |   ├─Token(NewLine) |\r\n|
}]
//@[000:00001) |   |   └─Token(RightBrace) |}|
//@[001:00002) |   └─Token(RightSquare) |]|
//@[002:00004) ├─Token(NewLine) |\r\n|
// #completionTest(86) -> defaultCreateModeProperties
//@[053:00055) ├─Token(NewLine) |\r\n|
var nestedDiscriminatorCompletions_for_if = nestedDiscriminator_for_if[0].properties.a
//@[000:00086) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00041) | ├─IdentifierSyntax
//@[004:00041) | | └─Token(Identifier) |nestedDiscriminatorCompletions_for_if|
//@[042:00043) | ├─Token(Assignment) |=|
//@[044:00086) | └─PropertyAccessSyntax
//@[044:00084) |   ├─PropertyAccessSyntax
//@[044:00073) |   | ├─ArrayAccessSyntax
//@[044:00070) |   | | ├─VariableAccessSyntax
//@[044:00070) |   | | | └─IdentifierSyntax
//@[044:00070) |   | | |   └─Token(Identifier) |nestedDiscriminator_for_if|
//@[070:00071) |   | | ├─Token(LeftSquare) |[|
//@[071:00072) |   | | ├─IntegerLiteralSyntax
//@[071:00072) |   | | | └─Token(Integer) |0|
//@[072:00073) |   | | └─Token(RightSquare) |]|
//@[073:00074) |   | ├─Token(Dot) |.|
//@[074:00084) |   | └─IdentifierSyntax
//@[074:00084) |   |   └─Token(Identifier) |properties|
//@[084:00085) |   ├─Token(Dot) |.|
//@[085:00086) |   └─IdentifierSyntax
//@[085:00086) |     └─Token(Identifier) |a|
//@[086:00088) ├─Token(NewLine) |\r\n|
// #completionTest(90) -> defaultCreateModeProperties
//@[053:00055) ├─Token(NewLine) |\r\n|
var nestedDiscriminatorCompletions2_for_if = nestedDiscriminator_for_if[0]['properties'].a
//@[000:00090) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00042) | ├─IdentifierSyntax
//@[004:00042) | | └─Token(Identifier) |nestedDiscriminatorCompletions2_for_if|
//@[043:00044) | ├─Token(Assignment) |=|
//@[045:00090) | └─PropertyAccessSyntax
//@[045:00088) |   ├─ArrayAccessSyntax
//@[045:00074) |   | ├─ArrayAccessSyntax
//@[045:00071) |   | | ├─VariableAccessSyntax
//@[045:00071) |   | | | └─IdentifierSyntax
//@[045:00071) |   | | |   └─Token(Identifier) |nestedDiscriminator_for_if|
//@[071:00072) |   | | ├─Token(LeftSquare) |[|
//@[072:00073) |   | | ├─IntegerLiteralSyntax
//@[072:00073) |   | | | └─Token(Integer) |0|
//@[073:00074) |   | | └─Token(RightSquare) |]|
//@[074:00075) |   | ├─Token(LeftSquare) |[|
//@[075:00087) |   | ├─StringSyntax
//@[075:00087) |   | | └─Token(StringComplete) |'properties'|
//@[087:00088) |   | └─Token(RightSquare) |]|
//@[088:00089) |   ├─Token(Dot) |.|
//@[089:00090) |   └─IdentifierSyntax
//@[089:00090) |     └─Token(Identifier) |a|
//@[090:00092) ├─Token(NewLine) |\r\n|
// #completionTest(86) -> defaultCreateModeProperties
//@[053:00055) ├─Token(NewLine) |\r\n|
var nestedDiscriminatorCompletions3_for_if = nestedDiscriminator_for_if[0].properties.
//@[000:00086) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00042) | ├─IdentifierSyntax
//@[004:00042) | | └─Token(Identifier) |nestedDiscriminatorCompletions3_for_if|
//@[043:00044) | ├─Token(Assignment) |=|
//@[045:00086) | └─PropertyAccessSyntax
//@[045:00085) |   ├─PropertyAccessSyntax
//@[045:00074) |   | ├─ArrayAccessSyntax
//@[045:00071) |   | | ├─VariableAccessSyntax
//@[045:00071) |   | | | └─IdentifierSyntax
//@[045:00071) |   | | |   └─Token(Identifier) |nestedDiscriminator_for_if|
//@[071:00072) |   | | ├─Token(LeftSquare) |[|
//@[072:00073) |   | | ├─IntegerLiteralSyntax
//@[072:00073) |   | | | └─Token(Integer) |0|
//@[073:00074) |   | | └─Token(RightSquare) |]|
//@[074:00075) |   | ├─Token(Dot) |.|
//@[075:00085) |   | └─IdentifierSyntax
//@[075:00085) |   |   └─Token(Identifier) |properties|
//@[085:00086) |   ├─Token(Dot) |.|
//@[086:00086) |   └─IdentifierSyntax
//@[086:00086) |     └─SkippedTriviaSyntax
//@[086:00088) ├─Token(NewLine) |\r\n|
// #completionTest(89) -> defaultCreateModeProperties
//@[053:00055) ├─Token(NewLine) |\r\n|
var nestedDiscriminatorCompletions4_for_if = nestedDiscriminator_for_if[0]['properties'].
//@[000:00089) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00042) | ├─IdentifierSyntax
//@[004:00042) | | └─Token(Identifier) |nestedDiscriminatorCompletions4_for_if|
//@[043:00044) | ├─Token(Assignment) |=|
//@[045:00089) | └─PropertyAccessSyntax
//@[045:00088) |   ├─ArrayAccessSyntax
//@[045:00074) |   | ├─ArrayAccessSyntax
//@[045:00071) |   | | ├─VariableAccessSyntax
//@[045:00071) |   | | | └─IdentifierSyntax
//@[045:00071) |   | | |   └─Token(Identifier) |nestedDiscriminator_for_if|
//@[071:00072) |   | | ├─Token(LeftSquare) |[|
//@[072:00073) |   | | ├─IntegerLiteralSyntax
//@[072:00073) |   | | | └─Token(Integer) |0|
//@[073:00074) |   | | └─Token(RightSquare) |]|
//@[074:00075) |   | ├─Token(LeftSquare) |[|
//@[075:00087) |   | ├─StringSyntax
//@[075:00087) |   | | └─Token(StringComplete) |'properties'|
//@[087:00088) |   | └─Token(RightSquare) |]|
//@[088:00089) |   ├─Token(Dot) |.|
//@[089:00089) |   └─IdentifierSyntax
//@[089:00089) |     └─SkippedTriviaSyntax
//@[089:00093) ├─Token(NewLine) |\r\n\r\n|

// #completionTest(96) -> defaultCreateModeIndexes_for_if
//@[057:00059) ├─Token(NewLine) |\r\n|
var nestedDiscriminatorArrayIndexCompletions_for_if = nestedDiscriminator_for_if[0].properties[a]
//@[000:00097) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00051) | ├─IdentifierSyntax
//@[004:00051) | | └─Token(Identifier) |nestedDiscriminatorArrayIndexCompletions_for_if|
//@[052:00053) | ├─Token(Assignment) |=|
//@[054:00097) | └─ArrayAccessSyntax
//@[054:00094) |   ├─PropertyAccessSyntax
//@[054:00083) |   | ├─ArrayAccessSyntax
//@[054:00080) |   | | ├─VariableAccessSyntax
//@[054:00080) |   | | | └─IdentifierSyntax
//@[054:00080) |   | | |   └─Token(Identifier) |nestedDiscriminator_for_if|
//@[080:00081) |   | | ├─Token(LeftSquare) |[|
//@[081:00082) |   | | ├─IntegerLiteralSyntax
//@[081:00082) |   | | | └─Token(Integer) |0|
//@[082:00083) |   | | └─Token(RightSquare) |]|
//@[083:00084) |   | ├─Token(Dot) |.|
//@[084:00094) |   | └─IdentifierSyntax
//@[084:00094) |   |   └─Token(Identifier) |properties|
//@[094:00095) |   ├─Token(LeftSquare) |[|
//@[095:00096) |   ├─VariableAccessSyntax
//@[095:00096) |   | └─IdentifierSyntax
//@[095:00096) |   |   └─Token(Identifier) |a|
//@[096:00097) |   └─Token(RightSquare) |]|
//@[097:00105) ├─Token(NewLine) |\r\n\r\n\r\n\r\n|



// sample resource to validate completions on the next declarations
//@[067:00069) ├─Token(NewLine) |\r\n|
resource nestedPropertyAccessOnConditional 'Microsoft.Compute/virtualMachines@2020-06-01' = if(true) {
//@[000:00209) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00042) | ├─IdentifierSyntax
//@[009:00042) | | └─Token(Identifier) |nestedPropertyAccessOnConditional|
//@[043:00089) | ├─StringSyntax
//@[043:00089) | | └─Token(StringComplete) |'Microsoft.Compute/virtualMachines@2020-06-01'|
//@[090:00091) | ├─Token(Assignment) |=|
//@[092:00209) | └─IfConditionSyntax
//@[092:00094) |   ├─Token(Identifier) |if|
//@[094:00100) |   ├─ParenthesizedExpressionSyntax
//@[094:00095) |   | ├─Token(LeftParen) |(|
//@[095:00099) |   | ├─BooleanLiteralSyntax
//@[095:00099) |   | | └─Token(TrueKeyword) |true|
//@[099:00100) |   | └─Token(RightParen) |)|
//@[101:00209) |   └─ObjectSyntax
//@[101:00102) |     ├─Token(LeftBrace) |{|
//@[102:00104) |     ├─Token(NewLine) |\r\n|
  location: 'test'
//@[002:00018) |     ├─ObjectPropertySyntax
//@[002:00010) |     | ├─IdentifierSyntax
//@[002:00010) |     | | └─Token(Identifier) |location|
//@[010:00011) |     | ├─Token(Colon) |:|
//@[012:00018) |     | └─StringSyntax
//@[012:00018) |     |   └─Token(StringComplete) |'test'|
//@[018:00020) |     ├─Token(NewLine) |\r\n|
  name: 'test'
//@[002:00014) |     ├─ObjectPropertySyntax
//@[002:00006) |     | ├─IdentifierSyntax
//@[002:00006) |     | | └─Token(Identifier) |name|
//@[006:00007) |     | ├─Token(Colon) |:|
//@[008:00014) |     | └─StringSyntax
//@[008:00014) |     |   └─Token(StringComplete) |'test'|
//@[014:00016) |     ├─Token(NewLine) |\r\n|
  properties: {
//@[002:00066) |     ├─ObjectPropertySyntax
//@[002:00012) |     | ├─IdentifierSyntax
//@[002:00012) |     | | └─Token(Identifier) |properties|
//@[012:00013) |     | ├─Token(Colon) |:|
//@[014:00066) |     | └─ObjectSyntax
//@[014:00015) |     |   ├─Token(LeftBrace) |{|
//@[015:00017) |     |   ├─Token(NewLine) |\r\n|
    additionalCapabilities: {
//@[004:00044) |     |   ├─ObjectPropertySyntax
//@[004:00026) |     |   | ├─IdentifierSyntax
//@[004:00026) |     |   | | └─Token(Identifier) |additionalCapabilities|
//@[026:00027) |     |   | ├─Token(Colon) |:|
//@[028:00044) |     |   | └─ObjectSyntax
//@[028:00029) |     |   |   ├─Token(LeftBrace) |{|
//@[029:00031) |     |   |   ├─Token(NewLine) |\r\n|
      
//@[006:00008) |     |   |   ├─Token(NewLine) |\r\n|
    }
//@[004:00005) |     |   |   └─Token(RightBrace) |}|
//@[005:00007) |     |   ├─Token(NewLine) |\r\n|
  }
//@[002:00003) |     |   └─Token(RightBrace) |}|
//@[003:00005) |     ├─Token(NewLine) |\r\n|
}
//@[000:00001) |     └─Token(RightBrace) |}|
//@[001:00003) ├─Token(NewLine) |\r\n|
// this validates that we can get nested property access completions on a conditional resource
//@[094:00096) ├─Token(NewLine) |\r\n|
//#completionTest(56) -> vmProperties
//@[037:00039) ├─Token(NewLine) |\r\n|
var sigh = nestedPropertyAccessOnConditional.properties.
//@[000:00056) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00008) | ├─IdentifierSyntax
//@[004:00008) | | └─Token(Identifier) |sigh|
//@[009:00010) | ├─Token(Assignment) |=|
//@[011:00056) | └─PropertyAccessSyntax
//@[011:00055) |   ├─PropertyAccessSyntax
//@[011:00044) |   | ├─VariableAccessSyntax
//@[011:00044) |   | | └─IdentifierSyntax
//@[011:00044) |   | |   └─Token(Identifier) |nestedPropertyAccessOnConditional|
//@[044:00045) |   | ├─Token(Dot) |.|
//@[045:00055) |   | └─IdentifierSyntax
//@[045:00055) |   |   └─Token(Identifier) |properties|
//@[055:00056) |   ├─Token(Dot) |.|
//@[056:00056) |   └─IdentifierSyntax
//@[056:00056) |     └─SkippedTriviaSyntax
//@[056:00060) ├─Token(NewLine) |\r\n\r\n|

/*
  boolean property value completions
*/ 
//@[003:00005) ├─Token(NewLine) |\r\n|
resource booleanPropertyPartialValue 'Microsoft.Compute/virtualMachines/extensions@2020-06-01' = {
//@[000:00222) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00036) | ├─IdentifierSyntax
//@[009:00036) | | └─Token(Identifier) |booleanPropertyPartialValue|
//@[037:00094) | ├─StringSyntax
//@[037:00094) | | └─Token(StringComplete) |'Microsoft.Compute/virtualMachines/extensions@2020-06-01'|
//@[095:00096) | ├─Token(Assignment) |=|
//@[097:00222) | └─ObjectSyntax
//@[097:00098) |   ├─Token(LeftBrace) |{|
//@[098:00100) |   ├─Token(NewLine) |\r\n|
  properties: {
//@[002:00119) |   ├─ObjectPropertySyntax
//@[002:00012) |   | ├─IdentifierSyntax
//@[002:00012) |   | | └─Token(Identifier) |properties|
//@[012:00013) |   | ├─Token(Colon) |:|
//@[014:00119) |   | └─ObjectSyntax
//@[014:00015) |   |   ├─Token(LeftBrace) |{|
//@[015:00017) |   |   ├─Token(NewLine) |\r\n|
    // #completionTest(28,29,30) -> boolPropertyValuesPlusSymbols
//@[065:00067) |   |   ├─Token(NewLine) |\r\n|
    autoUpgradeMinorVersion: t
//@[004:00030) |   |   ├─ObjectPropertySyntax
//@[004:00027) |   |   | ├─IdentifierSyntax
//@[004:00027) |   |   | | └─Token(Identifier) |autoUpgradeMinorVersion|
//@[027:00028) |   |   | ├─Token(Colon) |:|
//@[029:00030) |   |   | └─VariableAccessSyntax
//@[029:00030) |   |   |   └─IdentifierSyntax
//@[029:00030) |   |   |     └─Token(Identifier) |t|
//@[030:00032) |   |   ├─Token(NewLine) |\r\n|
  }
//@[002:00003) |   |   └─Token(RightBrace) |}|
//@[003:00005) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

resource selfScope 'My.Rp/mockResource@2020-12-01' = {
//@[000:00098) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00018) | ├─IdentifierSyntax
//@[009:00018) | | └─Token(Identifier) |selfScope|
//@[019:00050) | ├─StringSyntax
//@[019:00050) | | └─Token(StringComplete) |'My.Rp/mockResource@2020-12-01'|
//@[051:00052) | ├─Token(Assignment) |=|
//@[053:00098) | └─ObjectSyntax
//@[053:00054) |   ├─Token(LeftBrace) |{|
//@[054:00056) |   ├─Token(NewLine) |\r\n|
  name: 'selfScope'
//@[002:00019) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00019) |   | └─StringSyntax
//@[008:00019) |   |   └─Token(StringComplete) |'selfScope'|
//@[019:00021) |   ├─Token(NewLine) |\r\n|
  scope: selfScope
//@[002:00018) |   ├─ObjectPropertySyntax
//@[002:00007) |   | ├─IdentifierSyntax
//@[002:00007) |   | | └─Token(Identifier) |scope|
//@[007:00008) |   | ├─Token(Colon) |:|
//@[009:00018) |   | └─VariableAccessSyntax
//@[009:00018) |   |   └─IdentifierSyntax
//@[009:00018) |   |     └─Token(Identifier) |selfScope|
//@[018:00020) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

var notAResource = {
//@[000:00054) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00016) | ├─IdentifierSyntax
//@[004:00016) | | └─Token(Identifier) |notAResource|
//@[017:00018) | ├─Token(Assignment) |=|
//@[019:00054) | └─ObjectSyntax
//@[019:00020) |   ├─Token(LeftBrace) |{|
//@[020:00022) |   ├─Token(NewLine) |\r\n|
  im: 'not'
//@[002:00011) |   ├─ObjectPropertySyntax
//@[002:00004) |   | ├─IdentifierSyntax
//@[002:00004) |   | | └─Token(Identifier) |im|
//@[004:00005) |   | ├─Token(Colon) |:|
//@[006:00011) |   | └─StringSyntax
//@[006:00011) |   |   └─Token(StringComplete) |'not'|
//@[011:00013) |   ├─Token(NewLine) |\r\n|
  a: 'resource!'
//@[002:00016) |   ├─ObjectPropertySyntax
//@[002:00003) |   | ├─IdentifierSyntax
//@[002:00003) |   | | └─Token(Identifier) |a|
//@[003:00004) |   | ├─Token(Colon) |:|
//@[005:00016) |   | └─StringSyntax
//@[005:00016) |   |   └─Token(StringComplete) |'resource!'|
//@[016:00018) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00003) ├─Token(NewLine) |\r\n|
resource invalidScope 'My.Rp/mockResource@2020-12-01' = {
//@[000:00107) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00021) | ├─IdentifierSyntax
//@[009:00021) | | └─Token(Identifier) |invalidScope|
//@[022:00053) | ├─StringSyntax
//@[022:00053) | | └─Token(StringComplete) |'My.Rp/mockResource@2020-12-01'|
//@[054:00055) | ├─Token(Assignment) |=|
//@[056:00107) | └─ObjectSyntax
//@[056:00057) |   ├─Token(LeftBrace) |{|
//@[057:00059) |   ├─Token(NewLine) |\r\n|
  name: 'invalidScope'
//@[002:00022) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00022) |   | └─StringSyntax
//@[008:00022) |   |   └─Token(StringComplete) |'invalidScope'|
//@[022:00024) |   ├─Token(NewLine) |\r\n|
  scope: notAResource
//@[002:00021) |   ├─ObjectPropertySyntax
//@[002:00007) |   | ├─IdentifierSyntax
//@[002:00007) |   | | └─Token(Identifier) |scope|
//@[007:00008) |   | ├─Token(Colon) |:|
//@[009:00021) |   | └─VariableAccessSyntax
//@[009:00021) |   |   └─IdentifierSyntax
//@[009:00021) |   |     └─Token(Identifier) |notAResource|
//@[021:00023) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

resource invalidScope2 'My.Rp/mockResource@2020-12-01' = {
//@[000:00112) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00022) | ├─IdentifierSyntax
//@[009:00022) | | └─Token(Identifier) |invalidScope2|
//@[023:00054) | ├─StringSyntax
//@[023:00054) | | └─Token(StringComplete) |'My.Rp/mockResource@2020-12-01'|
//@[055:00056) | ├─Token(Assignment) |=|
//@[057:00112) | └─ObjectSyntax
//@[057:00058) |   ├─Token(LeftBrace) |{|
//@[058:00060) |   ├─Token(NewLine) |\r\n|
  name: 'invalidScope2'
//@[002:00023) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00023) |   | └─StringSyntax
//@[008:00023) |   |   └─Token(StringComplete) |'invalidScope2'|
//@[023:00025) |   ├─Token(NewLine) |\r\n|
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
//@[024:00026) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

resource invalidScope3 'My.Rp/mockResource@2020-12-01' = {
//@[000:00111) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00022) | ├─IdentifierSyntax
//@[009:00022) | | └─Token(Identifier) |invalidScope3|
//@[023:00054) | ├─StringSyntax
//@[023:00054) | | └─Token(StringComplete) |'My.Rp/mockResource@2020-12-01'|
//@[055:00056) | ├─Token(Assignment) |=|
//@[057:00111) | └─ObjectSyntax
//@[057:00058) |   ├─Token(LeftBrace) |{|
//@[058:00060) |   ├─Token(NewLine) |\r\n|
  name: 'invalidScope3'
//@[002:00023) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00023) |   | └─StringSyntax
//@[008:00023) |   |   └─Token(StringComplete) |'invalidScope3'|
//@[023:00025) |   ├─Token(NewLine) |\r\n|
  scope: subscription()
//@[002:00023) |   ├─ObjectPropertySyntax
//@[002:00007) |   | ├─IdentifierSyntax
//@[002:00007) |   | | └─Token(Identifier) |scope|
//@[007:00008) |   | ├─Token(Colon) |:|
//@[009:00023) |   | └─FunctionCallSyntax
//@[009:00021) |   |   ├─IdentifierSyntax
//@[009:00021) |   |   | └─Token(Identifier) |subscription|
//@[021:00022) |   |   ├─Token(LeftParen) |(|
//@[022:00023) |   |   └─Token(RightParen) |)|
//@[023:00025) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

resource invalidDuplicateName1 'Mock.Rp/mockResource@2020-01-01' = {
//@[000:00103) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00030) | ├─IdentifierSyntax
//@[009:00030) | | └─Token(Identifier) |invalidDuplicateName1|
//@[031:00064) | ├─StringSyntax
//@[031:00064) | | └─Token(StringComplete) |'Mock.Rp/mockResource@2020-01-01'|
//@[065:00066) | ├─Token(Assignment) |=|
//@[067:00103) | └─ObjectSyntax
//@[067:00068) |   ├─Token(LeftBrace) |{|
//@[068:00070) |   ├─Token(NewLine) |\r\n|
  name: 'invalidDuplicateName'
//@[002:00030) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00030) |   | └─StringSyntax
//@[008:00030) |   |   └─Token(StringComplete) |'invalidDuplicateName'|
//@[030:00032) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00003) ├─Token(NewLine) |\r\n|
resource invalidDuplicateName2 'Mock.Rp/mockResource@2020-01-01' = {
//@[000:00103) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00030) | ├─IdentifierSyntax
//@[009:00030) | | └─Token(Identifier) |invalidDuplicateName2|
//@[031:00064) | ├─StringSyntax
//@[031:00064) | | └─Token(StringComplete) |'Mock.Rp/mockResource@2020-01-01'|
//@[065:00066) | ├─Token(Assignment) |=|
//@[067:00103) | └─ObjectSyntax
//@[067:00068) |   ├─Token(LeftBrace) |{|
//@[068:00070) |   ├─Token(NewLine) |\r\n|
  name: 'invalidDuplicateName'
//@[002:00030) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00030) |   | └─StringSyntax
//@[008:00030) |   |   └─Token(StringComplete) |'invalidDuplicateName'|
//@[030:00032) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00003) ├─Token(NewLine) |\r\n|
resource invalidDuplicateName3 'Mock.Rp/mockResource@2019-01-01' = {
//@[000:00103) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00030) | ├─IdentifierSyntax
//@[009:00030) | | └─Token(Identifier) |invalidDuplicateName3|
//@[031:00064) | ├─StringSyntax
//@[031:00064) | | └─Token(StringComplete) |'Mock.Rp/mockResource@2019-01-01'|
//@[065:00066) | ├─Token(Assignment) |=|
//@[067:00103) | └─ObjectSyntax
//@[067:00068) |   ├─Token(LeftBrace) |{|
//@[068:00070) |   ├─Token(NewLine) |\r\n|
  name: 'invalidDuplicateName'
//@[002:00030) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00030) |   | └─StringSyntax
//@[008:00030) |   |   └─Token(StringComplete) |'invalidDuplicateName'|
//@[030:00032) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

resource validResourceForInvalidExtensionResourceDuplicateName 'Mock.Rp/mockResource@2020-01-01' = {
//@[000:00168) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00062) | ├─IdentifierSyntax
//@[009:00062) | | └─Token(Identifier) |validResourceForInvalidExtensionResourceDuplicateName|
//@[063:00096) | ├─StringSyntax
//@[063:00096) | | └─Token(StringComplete) |'Mock.Rp/mockResource@2020-01-01'|
//@[097:00098) | ├─Token(Assignment) |=|
//@[099:00168) | └─ObjectSyntax
//@[099:00100) |   ├─Token(LeftBrace) |{|
//@[100:00102) |   ├─Token(NewLine) |\r\n|
  name: 'validResourceForInvalidExtensionResourceDuplicateName'
//@[002:00063) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00063) |   | └─StringSyntax
//@[008:00063) |   |   └─Token(StringComplete) |'validResourceForInvalidExtensionResourceDuplicateName'|
//@[063:00065) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

resource invalidExtensionResourceDuplicateName1 'Mock.Rp/mockExtResource@2020-01-01' = {
//@[000:00204) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00047) | ├─IdentifierSyntax
//@[009:00047) | | └─Token(Identifier) |invalidExtensionResourceDuplicateName1|
//@[048:00084) | ├─StringSyntax
//@[048:00084) | | └─Token(StringComplete) |'Mock.Rp/mockExtResource@2020-01-01'|
//@[085:00086) | ├─Token(Assignment) |=|
//@[087:00204) | └─ObjectSyntax
//@[087:00088) |   ├─Token(LeftBrace) |{|
//@[088:00090) |   ├─Token(NewLine) |\r\n|
  name: 'invalidExtensionResourceDuplicateName'
//@[002:00047) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00047) |   | └─StringSyntax
//@[008:00047) |   |   └─Token(StringComplete) |'invalidExtensionResourceDuplicateName'|
//@[047:00049) |   ├─Token(NewLine) |\r\n|
  scope: validResourceForInvalidExtensionResourceDuplicateName
//@[002:00062) |   ├─ObjectPropertySyntax
//@[002:00007) |   | ├─IdentifierSyntax
//@[002:00007) |   | | └─Token(Identifier) |scope|
//@[007:00008) |   | ├─Token(Colon) |:|
//@[009:00062) |   | └─VariableAccessSyntax
//@[009:00062) |   |   └─IdentifierSyntax
//@[009:00062) |   |     └─Token(Identifier) |validResourceForInvalidExtensionResourceDuplicateName|
//@[062:00064) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

resource invalidExtensionResourceDuplicateName2 'Mock.Rp/mockExtResource@2019-01-01' = {
//@[000:00204) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00047) | ├─IdentifierSyntax
//@[009:00047) | | └─Token(Identifier) |invalidExtensionResourceDuplicateName2|
//@[048:00084) | ├─StringSyntax
//@[048:00084) | | └─Token(StringComplete) |'Mock.Rp/mockExtResource@2019-01-01'|
//@[085:00086) | ├─Token(Assignment) |=|
//@[087:00204) | └─ObjectSyntax
//@[087:00088) |   ├─Token(LeftBrace) |{|
//@[088:00090) |   ├─Token(NewLine) |\r\n|
  name: 'invalidExtensionResourceDuplicateName'
//@[002:00047) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00047) |   | └─StringSyntax
//@[008:00047) |   |   └─Token(StringComplete) |'invalidExtensionResourceDuplicateName'|
//@[047:00049) |   ├─Token(NewLine) |\r\n|
  scope: validResourceForInvalidExtensionResourceDuplicateName
//@[002:00062) |   ├─ObjectPropertySyntax
//@[002:00007) |   | ├─IdentifierSyntax
//@[002:00007) |   | | └─Token(Identifier) |scope|
//@[007:00008) |   | ├─Token(Colon) |:|
//@[009:00062) |   | └─VariableAccessSyntax
//@[009:00062) |   |   └─IdentifierSyntax
//@[009:00062) |   |     └─Token(Identifier) |validResourceForInvalidExtensionResourceDuplicateName|
//@[062:00064) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

@concat('foo', 'bar')
//@[000:00131) ├─ResourceDeclarationSyntax
//@[000:00021) | ├─DecoratorSyntax
//@[000:00001) | | ├─Token(At) |@|
//@[001:00021) | | └─FunctionCallSyntax
//@[001:00007) | |   ├─IdentifierSyntax
//@[001:00007) | |   | └─Token(Identifier) |concat|
//@[007:00008) | |   ├─Token(LeftParen) |(|
//@[008:00013) | |   ├─FunctionArgumentSyntax
//@[008:00013) | |   | └─StringSyntax
//@[008:00013) | |   |   └─Token(StringComplete) |'foo'|
//@[013:00014) | |   ├─Token(Comma) |,|
//@[015:00020) | |   ├─FunctionArgumentSyntax
//@[015:00020) | |   | └─StringSyntax
//@[015:00020) | |   |   └─Token(StringComplete) |'bar'|
//@[020:00021) | |   └─Token(RightParen) |)|
//@[021:00023) | ├─Token(NewLine) |\r\n|
@secure()
//@[000:00009) | ├─DecoratorSyntax
//@[000:00001) | | ├─Token(At) |@|
//@[001:00009) | | └─FunctionCallSyntax
//@[001:00007) | |   ├─IdentifierSyntax
//@[001:00007) | |   | └─Token(Identifier) |secure|
//@[007:00008) | |   ├─Token(LeftParen) |(|
//@[008:00009) | |   └─Token(RightParen) |)|
//@[009:00011) | ├─Token(NewLine) |\r\n|
resource invalidDecorator 'Microsoft.Foo/foos@2020-02-02-alpha'= {
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00025) | ├─IdentifierSyntax
//@[009:00025) | | └─Token(Identifier) |invalidDecorator|
//@[026:00063) | ├─StringSyntax
//@[026:00063) | | └─Token(StringComplete) |'Microsoft.Foo/foos@2020-02-02-alpha'|
//@[063:00064) | ├─Token(Assignment) |=|
//@[065:00097) | └─ObjectSyntax
//@[065:00066) |   ├─Token(LeftBrace) |{|
//@[066:00068) |   ├─Token(NewLine) |\r\n|
  name: 'invalidDecorator'
//@[002:00026) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00026) |   | └─StringSyntax
//@[008:00026) |   |   └─Token(StringComplete) |'invalidDecorator'|
//@[026:00028) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

resource cyclicRes 'Mock.Rp/mockExistingResource@2020-01-01' = {
//@[000:00108) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00018) | ├─IdentifierSyntax
//@[009:00018) | | └─Token(Identifier) |cyclicRes|
//@[019:00060) | ├─StringSyntax
//@[019:00060) | | └─Token(StringComplete) |'Mock.Rp/mockExistingResource@2020-01-01'|
//@[061:00062) | ├─Token(Assignment) |=|
//@[063:00108) | └─ObjectSyntax
//@[063:00064) |   ├─Token(LeftBrace) |{|
//@[064:00066) |   ├─Token(NewLine) |\r\n|
  name: 'cyclicRes'
//@[002:00019) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00019) |   | └─StringSyntax
//@[008:00019) |   |   └─Token(StringComplete) |'cyclicRes'|
//@[019:00021) |   ├─Token(NewLine) |\r\n|
  scope: cyclicRes
//@[002:00018) |   ├─ObjectPropertySyntax
//@[002:00007) |   | ├─IdentifierSyntax
//@[002:00007) |   | | └─Token(Identifier) |scope|
//@[007:00008) |   | ├─Token(Colon) |:|
//@[009:00018) |   | └─VariableAccessSyntax
//@[009:00018) |   |   └─IdentifierSyntax
//@[009:00018) |   |     └─Token(Identifier) |cyclicRes|
//@[018:00020) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

resource cyclicExistingRes 'Mock.Rp/mockExistingResource@2020-01-01' existing = {
//@[000:00141) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00026) | ├─IdentifierSyntax
//@[009:00026) | | └─Token(Identifier) |cyclicExistingRes|
//@[027:00068) | ├─StringSyntax
//@[027:00068) | | └─Token(StringComplete) |'Mock.Rp/mockExistingResource@2020-01-01'|
//@[069:00077) | ├─Token(Identifier) |existing|
//@[078:00079) | ├─Token(Assignment) |=|
//@[080:00141) | └─ObjectSyntax
//@[080:00081) |   ├─Token(LeftBrace) |{|
//@[081:00083) |   ├─Token(NewLine) |\r\n|
  name: 'cyclicExistingRes'
//@[002:00027) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00027) |   | └─StringSyntax
//@[008:00027) |   |   └─Token(StringComplete) |'cyclicExistingRes'|
//@[027:00029) |   ├─Token(NewLine) |\r\n|
  scope: cyclicExistingRes
//@[002:00026) |   ├─ObjectPropertySyntax
//@[002:00007) |   | ├─IdentifierSyntax
//@[002:00007) |   | | └─Token(Identifier) |scope|
//@[007:00008) |   | ├─Token(Colon) |:|
//@[009:00026) |   | └─VariableAccessSyntax
//@[009:00026) |   |   └─IdentifierSyntax
//@[009:00026) |   |     └─Token(Identifier) |cyclicExistingRes|
//@[026:00028) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

// loop parsing cases
//@[021:00023) ├─Token(NewLine) |\r\n|
resource expectedForKeyword 'Microsoft.Storage/storageAccounts@2019-06-01' = []
//@[000:00079) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00027) | ├─IdentifierSyntax
//@[009:00027) | | └─Token(Identifier) |expectedForKeyword|
//@[028:00074) | ├─StringSyntax
//@[028:00074) | | └─Token(StringComplete) |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[075:00076) | ├─Token(Assignment) |=|
//@[077:00079) | └─SkippedTriviaSyntax
//@[077:00078) |   ├─Token(LeftSquare) |[|
//@[078:00079) |   └─Token(RightSquare) |]|
//@[079:00083) ├─Token(NewLine) |\r\n\r\n|

resource expectedForKeyword2 'Microsoft.Storage/storageAccounts@2019-06-01' = [f]
//@[000:00081) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00028) | ├─IdentifierSyntax
//@[009:00028) | | └─Token(Identifier) |expectedForKeyword2|
//@[029:00075) | ├─StringSyntax
//@[029:00075) | | └─Token(StringComplete) |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[076:00077) | ├─Token(Assignment) |=|
//@[078:00081) | └─SkippedTriviaSyntax
//@[078:00079) |   ├─Token(LeftSquare) |[|
//@[079:00080) |   ├─Token(Identifier) |f|
//@[080:00081) |   └─Token(RightSquare) |]|
//@[081:00085) ├─Token(NewLine) |\r\n\r\n|

resource expectedLoopVar 'Microsoft.Storage/storageAccounts@2019-06-01' = [for]
//@[000:00079) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00024) | ├─IdentifierSyntax
//@[009:00024) | | └─Token(Identifier) |expectedLoopVar|
//@[025:00071) | ├─StringSyntax
//@[025:00071) | | └─Token(StringComplete) |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[072:00073) | ├─Token(Assignment) |=|
//@[074:00079) | └─ForSyntax
//@[074:00075) |   ├─Token(LeftSquare) |[|
//@[075:00078) |   ├─Token(Identifier) |for|
//@[078:00078) |   ├─SkippedTriviaSyntax
//@[078:00078) |   ├─SkippedTriviaSyntax
//@[078:00078) |   ├─SkippedTriviaSyntax
//@[078:00078) |   ├─SkippedTriviaSyntax
//@[078:00078) |   ├─SkippedTriviaSyntax
//@[078:00079) |   └─Token(RightSquare) |]|
//@[079:00083) ├─Token(NewLine) |\r\n\r\n|

resource expectedInKeyword 'Microsoft.Storage/storageAccounts@2019-06-01' = [for x]
//@[000:00083) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00026) | ├─IdentifierSyntax
//@[009:00026) | | └─Token(Identifier) |expectedInKeyword|
//@[027:00073) | ├─StringSyntax
//@[027:00073) | | └─Token(StringComplete) |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[074:00075) | ├─Token(Assignment) |=|
//@[076:00083) | └─ForSyntax
//@[076:00077) |   ├─Token(LeftSquare) |[|
//@[077:00080) |   ├─Token(Identifier) |for|
//@[081:00082) |   ├─LocalVariableSyntax
//@[081:00082) |   | └─IdentifierSyntax
//@[081:00082) |   |   └─Token(Identifier) |x|
//@[082:00082) |   ├─SkippedTriviaSyntax
//@[082:00082) |   ├─SkippedTriviaSyntax
//@[082:00082) |   ├─SkippedTriviaSyntax
//@[082:00082) |   ├─SkippedTriviaSyntax
//@[082:00083) |   └─Token(RightSquare) |]|
//@[083:00087) ├─Token(NewLine) |\r\n\r\n|

resource expectedInKeyword2 'Microsoft.Storage/storageAccounts@2019-06-01' = [for x b]
//@[000:00086) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00027) | ├─IdentifierSyntax
//@[009:00027) | | └─Token(Identifier) |expectedInKeyword2|
//@[028:00074) | ├─StringSyntax
//@[028:00074) | | └─Token(StringComplete) |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[075:00076) | ├─Token(Assignment) |=|
//@[077:00086) | └─ForSyntax
//@[077:00078) |   ├─Token(LeftSquare) |[|
//@[078:00081) |   ├─Token(Identifier) |for|
//@[082:00083) |   ├─LocalVariableSyntax
//@[082:00083) |   | └─IdentifierSyntax
//@[082:00083) |   |   └─Token(Identifier) |x|
//@[084:00085) |   ├─SkippedTriviaSyntax
//@[084:00085) |   | └─Token(Identifier) |b|
//@[085:00085) |   ├─SkippedTriviaSyntax
//@[085:00085) |   ├─SkippedTriviaSyntax
//@[085:00085) |   ├─SkippedTriviaSyntax
//@[085:00086) |   └─Token(RightSquare) |]|
//@[086:00090) ├─Token(NewLine) |\r\n\r\n|

resource expectedArrayExpression 'Microsoft.Storage/storageAccounts@2019-06-01' = [for x in]
//@[000:00092) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00032) | ├─IdentifierSyntax
//@[009:00032) | | └─Token(Identifier) |expectedArrayExpression|
//@[033:00079) | ├─StringSyntax
//@[033:00079) | | └─Token(StringComplete) |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[080:00081) | ├─Token(Assignment) |=|
//@[082:00092) | └─ForSyntax
//@[082:00083) |   ├─Token(LeftSquare) |[|
//@[083:00086) |   ├─Token(Identifier) |for|
//@[087:00088) |   ├─LocalVariableSyntax
//@[087:00088) |   | └─IdentifierSyntax
//@[087:00088) |   |   └─Token(Identifier) |x|
//@[089:00091) |   ├─Token(Identifier) |in|
//@[091:00091) |   ├─SkippedTriviaSyntax
//@[091:00091) |   ├─SkippedTriviaSyntax
//@[091:00091) |   ├─SkippedTriviaSyntax
//@[091:00092) |   └─Token(RightSquare) |]|
//@[092:00096) ├─Token(NewLine) |\r\n\r\n|

resource expectedColon 'Microsoft.Storage/storageAccounts@2019-06-01' = [for x in y]
//@[000:00084) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00022) | ├─IdentifierSyntax
//@[009:00022) | | └─Token(Identifier) |expectedColon|
//@[023:00069) | ├─StringSyntax
//@[023:00069) | | └─Token(StringComplete) |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[070:00071) | ├─Token(Assignment) |=|
//@[072:00084) | └─ForSyntax
//@[072:00073) |   ├─Token(LeftSquare) |[|
//@[073:00076) |   ├─Token(Identifier) |for|
//@[077:00078) |   ├─LocalVariableSyntax
//@[077:00078) |   | └─IdentifierSyntax
//@[077:00078) |   |   └─Token(Identifier) |x|
//@[079:00081) |   ├─Token(Identifier) |in|
//@[082:00083) |   ├─VariableAccessSyntax
//@[082:00083) |   | └─IdentifierSyntax
//@[082:00083) |   |   └─Token(Identifier) |y|
//@[083:00083) |   ├─SkippedTriviaSyntax
//@[083:00083) |   ├─SkippedTriviaSyntax
//@[083:00084) |   └─Token(RightSquare) |]|
//@[084:00088) ├─Token(NewLine) |\r\n\r\n|

resource expectedLoopBody 'Microsoft.Storage/storageAccounts@2019-06-01' = [for x in y:]
//@[000:00088) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00025) | ├─IdentifierSyntax
//@[009:00025) | | └─Token(Identifier) |expectedLoopBody|
//@[026:00072) | ├─StringSyntax
//@[026:00072) | | └─Token(StringComplete) |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[073:00074) | ├─Token(Assignment) |=|
//@[075:00088) | └─ForSyntax
//@[075:00076) |   ├─Token(LeftSquare) |[|
//@[076:00079) |   ├─Token(Identifier) |for|
//@[080:00081) |   ├─LocalVariableSyntax
//@[080:00081) |   | └─IdentifierSyntax
//@[080:00081) |   |   └─Token(Identifier) |x|
//@[082:00084) |   ├─Token(Identifier) |in|
//@[085:00086) |   ├─VariableAccessSyntax
//@[085:00086) |   | └─IdentifierSyntax
//@[085:00086) |   |   └─Token(Identifier) |y|
//@[086:00087) |   ├─Token(Colon) |:|
//@[087:00087) |   ├─SkippedTriviaSyntax
//@[087:00088) |   └─Token(RightSquare) |]|
//@[088:00092) ├─Token(NewLine) |\r\n\r\n|

// loop index parsing cases
//@[027:00029) ├─Token(NewLine) |\r\n|
resource expectedLoopItemName 'Microsoft.Network/dnsZones@2018-05-01' = [for ()]
//@[000:00080) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00029) | ├─IdentifierSyntax
//@[009:00029) | | └─Token(Identifier) |expectedLoopItemName|
//@[030:00069) | ├─StringSyntax
//@[030:00069) | | └─Token(StringComplete) |'Microsoft.Network/dnsZones@2018-05-01'|
//@[070:00071) | ├─Token(Assignment) |=|
//@[072:00080) | └─ForSyntax
//@[072:00073) |   ├─Token(LeftSquare) |[|
//@[073:00076) |   ├─Token(Identifier) |for|
//@[077:00079) |   ├─SkippedTriviaSyntax
//@[077:00079) |   | └─VariableBlockSyntax
//@[077:00078) |   |   ├─Token(LeftParen) |(|
//@[078:00079) |   |   └─Token(RightParen) |)|
//@[079:00079) |   ├─SkippedTriviaSyntax
//@[079:00079) |   ├─SkippedTriviaSyntax
//@[079:00079) |   ├─SkippedTriviaSyntax
//@[079:00079) |   ├─SkippedTriviaSyntax
//@[079:00080) |   └─Token(RightSquare) |]|
//@[080:00084) ├─Token(NewLine) |\r\n\r\n|

resource expectedLoopItemName2 'Microsoft.Network/dnsZones@2018-05-01' = [for (
//@[000:00079) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00030) | ├─IdentifierSyntax
//@[009:00030) | | └─Token(Identifier) |expectedLoopItemName2|
//@[031:00070) | ├─StringSyntax
//@[031:00070) | | └─Token(StringComplete) |'Microsoft.Network/dnsZones@2018-05-01'|
//@[071:00072) | ├─Token(Assignment) |=|
//@[073:00079) | └─ForSyntax
//@[073:00074) |   ├─Token(LeftSquare) |[|
//@[074:00077) |   ├─Token(Identifier) |for|
//@[078:00079) |   ├─VariableBlockSyntax
//@[078:00079) |   | ├─Token(LeftParen) |(|
//@[079:00079) |   | ├─SkippedTriviaSyntax
//@[079:00079) |   | └─SkippedTriviaSyntax
//@[079:00079) |   ├─SkippedTriviaSyntax
//@[079:00079) |   ├─SkippedTriviaSyntax
//@[079:00079) |   ├─SkippedTriviaSyntax
//@[079:00079) |   ├─SkippedTriviaSyntax
//@[079:00079) |   └─SkippedTriviaSyntax
//@[079:00083) ├─Token(NewLine) |\r\n\r\n|

resource expectedComma 'Microsoft.Network/dnsZones@2018-05-01' = [for (x)]
//@[000:00074) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00022) | ├─IdentifierSyntax
//@[009:00022) | | └─Token(Identifier) |expectedComma|
//@[023:00062) | ├─StringSyntax
//@[023:00062) | | └─Token(StringComplete) |'Microsoft.Network/dnsZones@2018-05-01'|
//@[063:00064) | ├─Token(Assignment) |=|
//@[065:00074) | └─ForSyntax
//@[065:00066) |   ├─Token(LeftSquare) |[|
//@[066:00069) |   ├─Token(Identifier) |for|
//@[070:00073) |   ├─SkippedTriviaSyntax
//@[070:00073) |   | └─VariableBlockSyntax
//@[070:00071) |   |   ├─Token(LeftParen) |(|
//@[071:00072) |   |   ├─LocalVariableSyntax
//@[071:00072) |   |   | └─IdentifierSyntax
//@[071:00072) |   |   |   └─Token(Identifier) |x|
//@[072:00073) |   |   └─Token(RightParen) |)|
//@[073:00073) |   ├─SkippedTriviaSyntax
//@[073:00073) |   ├─SkippedTriviaSyntax
//@[073:00073) |   ├─SkippedTriviaSyntax
//@[073:00073) |   ├─SkippedTriviaSyntax
//@[073:00074) |   └─Token(RightSquare) |]|
//@[074:00078) ├─Token(NewLine) |\r\n\r\n|

resource expectedLoopIndexName 'Microsoft.Network/dnsZones@2018-05-01' = [for (x, )]
//@[000:00084) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00030) | ├─IdentifierSyntax
//@[009:00030) | | └─Token(Identifier) |expectedLoopIndexName|
//@[031:00070) | ├─StringSyntax
//@[031:00070) | | └─Token(StringComplete) |'Microsoft.Network/dnsZones@2018-05-01'|
//@[071:00072) | ├─Token(Assignment) |=|
//@[073:00084) | └─ForSyntax
//@[073:00074) |   ├─Token(LeftSquare) |[|
//@[074:00077) |   ├─Token(Identifier) |for|
//@[078:00083) |   ├─SkippedTriviaSyntax
//@[078:00083) |   | └─VariableBlockSyntax
//@[078:00079) |   |   ├─Token(LeftParen) |(|
//@[079:00080) |   |   ├─LocalVariableSyntax
//@[079:00080) |   |   | └─IdentifierSyntax
//@[079:00080) |   |   |   └─Token(Identifier) |x|
//@[080:00081) |   |   ├─Token(Comma) |,|
//@[082:00083) |   |   └─Token(RightParen) |)|
//@[083:00083) |   ├─SkippedTriviaSyntax
//@[083:00083) |   ├─SkippedTriviaSyntax
//@[083:00083) |   ├─SkippedTriviaSyntax
//@[083:00083) |   ├─SkippedTriviaSyntax
//@[083:00084) |   └─Token(RightSquare) |]|
//@[084:00088) ├─Token(NewLine) |\r\n\r\n|

resource expectedInKeyword3 'Microsoft.Network/dnsZones@2018-05-01' = [for (x, y)]
//@[000:00082) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00027) | ├─IdentifierSyntax
//@[009:00027) | | └─Token(Identifier) |expectedInKeyword3|
//@[028:00067) | ├─StringSyntax
//@[028:00067) | | └─Token(StringComplete) |'Microsoft.Network/dnsZones@2018-05-01'|
//@[068:00069) | ├─Token(Assignment) |=|
//@[070:00082) | └─ForSyntax
//@[070:00071) |   ├─Token(LeftSquare) |[|
//@[071:00074) |   ├─Token(Identifier) |for|
//@[075:00081) |   ├─VariableBlockSyntax
//@[075:00076) |   | ├─Token(LeftParen) |(|
//@[076:00077) |   | ├─LocalVariableSyntax
//@[076:00077) |   | | └─IdentifierSyntax
//@[076:00077) |   | |   └─Token(Identifier) |x|
//@[077:00078) |   | ├─Token(Comma) |,|
//@[079:00080) |   | ├─LocalVariableSyntax
//@[079:00080) |   | | └─IdentifierSyntax
//@[079:00080) |   | |   └─Token(Identifier) |y|
//@[080:00081) |   | └─Token(RightParen) |)|
//@[081:00081) |   ├─SkippedTriviaSyntax
//@[081:00081) |   ├─SkippedTriviaSyntax
//@[081:00081) |   ├─SkippedTriviaSyntax
//@[081:00081) |   ├─SkippedTriviaSyntax
//@[081:00082) |   └─Token(RightSquare) |]|
//@[082:00086) ├─Token(NewLine) |\r\n\r\n|

resource expectedInKeyword4 'Microsoft.Network/dnsZones@2018-05-01' = [for (x, y) z]
//@[000:00084) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00027) | ├─IdentifierSyntax
//@[009:00027) | | └─Token(Identifier) |expectedInKeyword4|
//@[028:00067) | ├─StringSyntax
//@[028:00067) | | └─Token(StringComplete) |'Microsoft.Network/dnsZones@2018-05-01'|
//@[068:00069) | ├─Token(Assignment) |=|
//@[070:00084) | └─ForSyntax
//@[070:00071) |   ├─Token(LeftSquare) |[|
//@[071:00074) |   ├─Token(Identifier) |for|
//@[075:00081) |   ├─VariableBlockSyntax
//@[075:00076) |   | ├─Token(LeftParen) |(|
//@[076:00077) |   | ├─LocalVariableSyntax
//@[076:00077) |   | | └─IdentifierSyntax
//@[076:00077) |   | |   └─Token(Identifier) |x|
//@[077:00078) |   | ├─Token(Comma) |,|
//@[079:00080) |   | ├─LocalVariableSyntax
//@[079:00080) |   | | └─IdentifierSyntax
//@[079:00080) |   | |   └─Token(Identifier) |y|
//@[080:00081) |   | └─Token(RightParen) |)|
//@[082:00083) |   ├─SkippedTriviaSyntax
//@[082:00083) |   | └─Token(Identifier) |z|
//@[083:00083) |   ├─SkippedTriviaSyntax
//@[083:00083) |   ├─SkippedTriviaSyntax
//@[083:00083) |   ├─SkippedTriviaSyntax
//@[083:00084) |   └─Token(RightSquare) |]|
//@[084:00088) ├─Token(NewLine) |\r\n\r\n|

resource expectedArrayExpression2 'Microsoft.Network/dnsZones@2018-05-01' = [for (x, y) in ]
//@[000:00092) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00033) | ├─IdentifierSyntax
//@[009:00033) | | └─Token(Identifier) |expectedArrayExpression2|
//@[034:00073) | ├─StringSyntax
//@[034:00073) | | └─Token(StringComplete) |'Microsoft.Network/dnsZones@2018-05-01'|
//@[074:00075) | ├─Token(Assignment) |=|
//@[076:00092) | └─ForSyntax
//@[076:00077) |   ├─Token(LeftSquare) |[|
//@[077:00080) |   ├─Token(Identifier) |for|
//@[081:00087) |   ├─VariableBlockSyntax
//@[081:00082) |   | ├─Token(LeftParen) |(|
//@[082:00083) |   | ├─LocalVariableSyntax
//@[082:00083) |   | | └─IdentifierSyntax
//@[082:00083) |   | |   └─Token(Identifier) |x|
//@[083:00084) |   | ├─Token(Comma) |,|
//@[085:00086) |   | ├─LocalVariableSyntax
//@[085:00086) |   | | └─IdentifierSyntax
//@[085:00086) |   | |   └─Token(Identifier) |y|
//@[086:00087) |   | └─Token(RightParen) |)|
//@[088:00090) |   ├─Token(Identifier) |in|
//@[091:00091) |   ├─SkippedTriviaSyntax
//@[091:00091) |   ├─SkippedTriviaSyntax
//@[091:00091) |   ├─SkippedTriviaSyntax
//@[091:00092) |   └─Token(RightSquare) |]|
//@[092:00096) ├─Token(NewLine) |\r\n\r\n|

resource expectedColon2 'Microsoft.Network/dnsZones@2018-05-01' = [for (x, y) in z]
//@[000:00083) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00023) | ├─IdentifierSyntax
//@[009:00023) | | └─Token(Identifier) |expectedColon2|
//@[024:00063) | ├─StringSyntax
//@[024:00063) | | └─Token(StringComplete) |'Microsoft.Network/dnsZones@2018-05-01'|
//@[064:00065) | ├─Token(Assignment) |=|
//@[066:00083) | └─ForSyntax
//@[066:00067) |   ├─Token(LeftSquare) |[|
//@[067:00070) |   ├─Token(Identifier) |for|
//@[071:00077) |   ├─VariableBlockSyntax
//@[071:00072) |   | ├─Token(LeftParen) |(|
//@[072:00073) |   | ├─LocalVariableSyntax
//@[072:00073) |   | | └─IdentifierSyntax
//@[072:00073) |   | |   └─Token(Identifier) |x|
//@[073:00074) |   | ├─Token(Comma) |,|
//@[075:00076) |   | ├─LocalVariableSyntax
//@[075:00076) |   | | └─IdentifierSyntax
//@[075:00076) |   | |   └─Token(Identifier) |y|
//@[076:00077) |   | └─Token(RightParen) |)|
//@[078:00080) |   ├─Token(Identifier) |in|
//@[081:00082) |   ├─VariableAccessSyntax
//@[081:00082) |   | └─IdentifierSyntax
//@[081:00082) |   |   └─Token(Identifier) |z|
//@[082:00082) |   ├─SkippedTriviaSyntax
//@[082:00082) |   ├─SkippedTriviaSyntax
//@[082:00083) |   └─Token(RightSquare) |]|
//@[083:00087) ├─Token(NewLine) |\r\n\r\n|

resource expectedLoopBody2 'Microsoft.Network/dnsZones@2018-05-01' = [for (x, y) in z:]
//@[000:00087) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00026) | ├─IdentifierSyntax
//@[009:00026) | | └─Token(Identifier) |expectedLoopBody2|
//@[027:00066) | ├─StringSyntax
//@[027:00066) | | └─Token(StringComplete) |'Microsoft.Network/dnsZones@2018-05-01'|
//@[067:00068) | ├─Token(Assignment) |=|
//@[069:00087) | └─ForSyntax
//@[069:00070) |   ├─Token(LeftSquare) |[|
//@[070:00073) |   ├─Token(Identifier) |for|
//@[074:00080) |   ├─VariableBlockSyntax
//@[074:00075) |   | ├─Token(LeftParen) |(|
//@[075:00076) |   | ├─LocalVariableSyntax
//@[075:00076) |   | | └─IdentifierSyntax
//@[075:00076) |   | |   └─Token(Identifier) |x|
//@[076:00077) |   | ├─Token(Comma) |,|
//@[078:00079) |   | ├─LocalVariableSyntax
//@[078:00079) |   | | └─IdentifierSyntax
//@[078:00079) |   | |   └─Token(Identifier) |y|
//@[079:00080) |   | └─Token(RightParen) |)|
//@[081:00083) |   ├─Token(Identifier) |in|
//@[084:00085) |   ├─VariableAccessSyntax
//@[084:00085) |   | └─IdentifierSyntax
//@[084:00085) |   |   └─Token(Identifier) |z|
//@[085:00086) |   ├─Token(Colon) |:|
//@[086:00086) |   ├─SkippedTriviaSyntax
//@[086:00087) |   └─Token(RightSquare) |]|
//@[087:00091) ├─Token(NewLine) |\r\n\r\n|

// loop filter parsing cases
//@[028:00030) ├─Token(NewLine) |\r\n|
resource expectedLoopFilterOpenParen 'Microsoft.Storage/storageAccounts@2019-06-01' = [for x in y: if]
//@[000:00102) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00036) | ├─IdentifierSyntax
//@[009:00036) | | └─Token(Identifier) |expectedLoopFilterOpenParen|
//@[037:00083) | ├─StringSyntax
//@[037:00083) | | └─Token(StringComplete) |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[084:00085) | ├─Token(Assignment) |=|
//@[086:00102) | └─ForSyntax
//@[086:00087) |   ├─Token(LeftSquare) |[|
//@[087:00090) |   ├─Token(Identifier) |for|
//@[091:00092) |   ├─LocalVariableSyntax
//@[091:00092) |   | └─IdentifierSyntax
//@[091:00092) |   |   └─Token(Identifier) |x|
//@[093:00095) |   ├─Token(Identifier) |in|
//@[096:00097) |   ├─VariableAccessSyntax
//@[096:00097) |   | └─IdentifierSyntax
//@[096:00097) |   |   └─Token(Identifier) |y|
//@[097:00098) |   ├─Token(Colon) |:|
//@[099:00101) |   ├─IfConditionSyntax
//@[099:00101) |   | ├─Token(Identifier) |if|
//@[101:00101) |   | ├─SkippedTriviaSyntax
//@[101:00101) |   | └─SkippedTriviaSyntax
//@[101:00102) |   └─Token(RightSquare) |]|
//@[102:00104) ├─Token(NewLine) |\r\n|
resource expectedLoopFilterOpenParen2 'Microsoft.Network/dnsZones@2018-05-01' = [for (x, y) in z: if]
//@[000:00101) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00037) | ├─IdentifierSyntax
//@[009:00037) | | └─Token(Identifier) |expectedLoopFilterOpenParen2|
//@[038:00077) | ├─StringSyntax
//@[038:00077) | | └─Token(StringComplete) |'Microsoft.Network/dnsZones@2018-05-01'|
//@[078:00079) | ├─Token(Assignment) |=|
//@[080:00101) | └─ForSyntax
//@[080:00081) |   ├─Token(LeftSquare) |[|
//@[081:00084) |   ├─Token(Identifier) |for|
//@[085:00091) |   ├─VariableBlockSyntax
//@[085:00086) |   | ├─Token(LeftParen) |(|
//@[086:00087) |   | ├─LocalVariableSyntax
//@[086:00087) |   | | └─IdentifierSyntax
//@[086:00087) |   | |   └─Token(Identifier) |x|
//@[087:00088) |   | ├─Token(Comma) |,|
//@[089:00090) |   | ├─LocalVariableSyntax
//@[089:00090) |   | | └─IdentifierSyntax
//@[089:00090) |   | |   └─Token(Identifier) |y|
//@[090:00091) |   | └─Token(RightParen) |)|
//@[092:00094) |   ├─Token(Identifier) |in|
//@[095:00096) |   ├─VariableAccessSyntax
//@[095:00096) |   | └─IdentifierSyntax
//@[095:00096) |   |   └─Token(Identifier) |z|
//@[096:00097) |   ├─Token(Colon) |:|
//@[098:00100) |   ├─IfConditionSyntax
//@[098:00100) |   | ├─Token(Identifier) |if|
//@[100:00100) |   | ├─SkippedTriviaSyntax
//@[100:00100) |   | └─SkippedTriviaSyntax
//@[100:00101) |   └─Token(RightSquare) |]|
//@[101:00105) ├─Token(NewLine) |\r\n\r\n|

resource expectedLoopFilterPredicateAndBody 'Microsoft.Storage/storageAccounts@2019-06-01' = [for x in y: if()]
//@[000:00111) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00043) | ├─IdentifierSyntax
//@[009:00043) | | └─Token(Identifier) |expectedLoopFilterPredicateAndBody|
//@[044:00090) | ├─StringSyntax
//@[044:00090) | | └─Token(StringComplete) |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[091:00092) | ├─Token(Assignment) |=|
//@[093:00111) | └─ForSyntax
//@[093:00094) |   ├─Token(LeftSquare) |[|
//@[094:00097) |   ├─Token(Identifier) |for|
//@[098:00099) |   ├─LocalVariableSyntax
//@[098:00099) |   | └─IdentifierSyntax
//@[098:00099) |   |   └─Token(Identifier) |x|
//@[100:00102) |   ├─Token(Identifier) |in|
//@[103:00104) |   ├─VariableAccessSyntax
//@[103:00104) |   | └─IdentifierSyntax
//@[103:00104) |   |   └─Token(Identifier) |y|
//@[104:00105) |   ├─Token(Colon) |:|
//@[106:00110) |   ├─IfConditionSyntax
//@[106:00108) |   | ├─Token(Identifier) |if|
//@[108:00110) |   | ├─ParenthesizedExpressionSyntax
//@[108:00109) |   | | ├─Token(LeftParen) |(|
//@[109:00109) |   | | ├─SkippedTriviaSyntax
//@[109:00110) |   | | └─Token(RightParen) |)|
//@[110:00110) |   | └─SkippedTriviaSyntax
//@[110:00111) |   └─Token(RightSquare) |]|
//@[111:00113) ├─Token(NewLine) |\r\n|
resource expectedLoopFilterPredicateAndBody2 'Microsoft.Network/dnsZones@2018-05-01' = [for (x, y) in z: if()]
//@[000:00110) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00044) | ├─IdentifierSyntax
//@[009:00044) | | └─Token(Identifier) |expectedLoopFilterPredicateAndBody2|
//@[045:00084) | ├─StringSyntax
//@[045:00084) | | └─Token(StringComplete) |'Microsoft.Network/dnsZones@2018-05-01'|
//@[085:00086) | ├─Token(Assignment) |=|
//@[087:00110) | └─ForSyntax
//@[087:00088) |   ├─Token(LeftSquare) |[|
//@[088:00091) |   ├─Token(Identifier) |for|
//@[092:00098) |   ├─VariableBlockSyntax
//@[092:00093) |   | ├─Token(LeftParen) |(|
//@[093:00094) |   | ├─LocalVariableSyntax
//@[093:00094) |   | | └─IdentifierSyntax
//@[093:00094) |   | |   └─Token(Identifier) |x|
//@[094:00095) |   | ├─Token(Comma) |,|
//@[096:00097) |   | ├─LocalVariableSyntax
//@[096:00097) |   | | └─IdentifierSyntax
//@[096:00097) |   | |   └─Token(Identifier) |y|
//@[097:00098) |   | └─Token(RightParen) |)|
//@[099:00101) |   ├─Token(Identifier) |in|
//@[102:00103) |   ├─VariableAccessSyntax
//@[102:00103) |   | └─IdentifierSyntax
//@[102:00103) |   |   └─Token(Identifier) |z|
//@[103:00104) |   ├─Token(Colon) |:|
//@[105:00109) |   ├─IfConditionSyntax
//@[105:00107) |   | ├─Token(Identifier) |if|
//@[107:00109) |   | ├─ParenthesizedExpressionSyntax
//@[107:00108) |   | | ├─Token(LeftParen) |(|
//@[108:00108) |   | | ├─SkippedTriviaSyntax
//@[108:00109) |   | | └─Token(RightParen) |)|
//@[109:00109) |   | └─SkippedTriviaSyntax
//@[109:00110) |   └─Token(RightSquare) |]|
//@[110:00114) ├─Token(NewLine) |\r\n\r\n|

// wrong body type
//@[018:00020) ├─Token(NewLine) |\r\n|
var emptyArray = []
//@[000:00019) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00014) | ├─IdentifierSyntax
//@[004:00014) | | └─Token(Identifier) |emptyArray|
//@[015:00016) | ├─Token(Assignment) |=|
//@[017:00019) | └─ArraySyntax
//@[017:00018) |   ├─Token(LeftSquare) |[|
//@[018:00019) |   └─Token(RightSquare) |]|
//@[019:00021) ├─Token(NewLine) |\r\n|
resource wrongLoopBodyType 'Microsoft.Storage/storageAccounts@2019-06-01' = [for x in emptyArray:4]
//@[000:00099) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00026) | ├─IdentifierSyntax
//@[009:00026) | | └─Token(Identifier) |wrongLoopBodyType|
//@[027:00073) | ├─StringSyntax
//@[027:00073) | | └─Token(StringComplete) |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[074:00075) | ├─Token(Assignment) |=|
//@[076:00099) | └─ForSyntax
//@[076:00077) |   ├─Token(LeftSquare) |[|
//@[077:00080) |   ├─Token(Identifier) |for|
//@[081:00082) |   ├─LocalVariableSyntax
//@[081:00082) |   | └─IdentifierSyntax
//@[081:00082) |   |   └─Token(Identifier) |x|
//@[083:00085) |   ├─Token(Identifier) |in|
//@[086:00096) |   ├─VariableAccessSyntax
//@[086:00096) |   | └─IdentifierSyntax
//@[086:00096) |   |   └─Token(Identifier) |emptyArray|
//@[096:00097) |   ├─Token(Colon) |:|
//@[097:00098) |   ├─SkippedTriviaSyntax
//@[097:00098) |   | └─Token(Integer) |4|
//@[098:00099) |   └─Token(RightSquare) |]|
//@[099:00101) ├─Token(NewLine) |\r\n|
resource wrongLoopBodyType2 'Microsoft.Storage/storageAccounts@2019-06-01' = [for (x ,i) in emptyArray:4]
//@[000:00105) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00027) | ├─IdentifierSyntax
//@[009:00027) | | └─Token(Identifier) |wrongLoopBodyType2|
//@[028:00074) | ├─StringSyntax
//@[028:00074) | | └─Token(StringComplete) |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[075:00076) | ├─Token(Assignment) |=|
//@[077:00105) | └─ForSyntax
//@[077:00078) |   ├─Token(LeftSquare) |[|
//@[078:00081) |   ├─Token(Identifier) |for|
//@[082:00088) |   ├─VariableBlockSyntax
//@[082:00083) |   | ├─Token(LeftParen) |(|
//@[083:00084) |   | ├─LocalVariableSyntax
//@[083:00084) |   | | └─IdentifierSyntax
//@[083:00084) |   | |   └─Token(Identifier) |x|
//@[085:00086) |   | ├─Token(Comma) |,|
//@[086:00087) |   | ├─LocalVariableSyntax
//@[086:00087) |   | | └─IdentifierSyntax
//@[086:00087) |   | |   └─Token(Identifier) |i|
//@[087:00088) |   | └─Token(RightParen) |)|
//@[089:00091) |   ├─Token(Identifier) |in|
//@[092:00102) |   ├─VariableAccessSyntax
//@[092:00102) |   | └─IdentifierSyntax
//@[092:00102) |   |   └─Token(Identifier) |emptyArray|
//@[102:00103) |   ├─Token(Colon) |:|
//@[103:00104) |   ├─SkippedTriviaSyntax
//@[103:00104) |   | └─Token(Integer) |4|
//@[104:00105) |   └─Token(RightSquare) |]|
//@[105:00109) ├─Token(NewLine) |\r\n\r\n|

// duplicate variable in the same scope
//@[039:00041) ├─Token(NewLine) |\r\n|
resource itemAndIndexSameName 'Microsoft.AAD/domainServices@2020-01-01' = [for (same, same) in emptyArray: {
//@[000:00112) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00029) | ├─IdentifierSyntax
//@[009:00029) | | └─Token(Identifier) |itemAndIndexSameName|
//@[030:00071) | ├─StringSyntax
//@[030:00071) | | └─Token(StringComplete) |'Microsoft.AAD/domainServices@2020-01-01'|
//@[072:00073) | ├─Token(Assignment) |=|
//@[074:00112) | └─ForSyntax
//@[074:00075) |   ├─Token(LeftSquare) |[|
//@[075:00078) |   ├─Token(Identifier) |for|
//@[079:00091) |   ├─VariableBlockSyntax
//@[079:00080) |   | ├─Token(LeftParen) |(|
//@[080:00084) |   | ├─LocalVariableSyntax
//@[080:00084) |   | | └─IdentifierSyntax
//@[080:00084) |   | |   └─Token(Identifier) |same|
//@[084:00085) |   | ├─Token(Comma) |,|
//@[086:00090) |   | ├─LocalVariableSyntax
//@[086:00090) |   | | └─IdentifierSyntax
//@[086:00090) |   | |   └─Token(Identifier) |same|
//@[090:00091) |   | └─Token(RightParen) |)|
//@[092:00094) |   ├─Token(Identifier) |in|
//@[095:00105) |   ├─VariableAccessSyntax
//@[095:00105) |   | └─IdentifierSyntax
//@[095:00105) |   |   └─Token(Identifier) |emptyArray|
//@[105:00106) |   ├─Token(Colon) |:|
//@[107:00111) |   ├─ObjectSyntax
//@[107:00108) |   | ├─Token(LeftBrace) |{|
//@[108:00110) |   | ├─Token(NewLine) |\r\n|
}]
//@[000:00001) |   | └─Token(RightBrace) |}|
//@[001:00002) |   └─Token(RightSquare) |]|
//@[002:00006) ├─Token(NewLine) |\r\n\r\n|

// errors in the array expression
//@[033:00035) ├─Token(NewLine) |\r\n|
resource arrayExpressionErrors 'Microsoft.Storage/storageAccounts@2019-06-01' = [for account in union([], 2): {
//@[000:00115) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00030) | ├─IdentifierSyntax
//@[009:00030) | | └─Token(Identifier) |arrayExpressionErrors|
//@[031:00077) | ├─StringSyntax
//@[031:00077) | | └─Token(StringComplete) |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[078:00079) | ├─Token(Assignment) |=|
//@[080:00115) | └─ForSyntax
//@[080:00081) |   ├─Token(LeftSquare) |[|
//@[081:00084) |   ├─Token(Identifier) |for|
//@[085:00092) |   ├─LocalVariableSyntax
//@[085:00092) |   | └─IdentifierSyntax
//@[085:00092) |   |   └─Token(Identifier) |account|
//@[093:00095) |   ├─Token(Identifier) |in|
//@[096:00108) |   ├─FunctionCallSyntax
//@[096:00101) |   | ├─IdentifierSyntax
//@[096:00101) |   | | └─Token(Identifier) |union|
//@[101:00102) |   | ├─Token(LeftParen) |(|
//@[102:00104) |   | ├─FunctionArgumentSyntax
//@[102:00104) |   | | └─ArraySyntax
//@[102:00103) |   | |   ├─Token(LeftSquare) |[|
//@[103:00104) |   | |   └─Token(RightSquare) |]|
//@[104:00105) |   | ├─Token(Comma) |,|
//@[106:00107) |   | ├─FunctionArgumentSyntax
//@[106:00107) |   | | └─IntegerLiteralSyntax
//@[106:00107) |   | |   └─Token(Integer) |2|
//@[107:00108) |   | └─Token(RightParen) |)|
//@[108:00109) |   ├─Token(Colon) |:|
//@[110:00114) |   ├─ObjectSyntax
//@[110:00111) |   | ├─Token(LeftBrace) |{|
//@[111:00113) |   | ├─Token(NewLine) |\r\n|
}]
//@[000:00001) |   | └─Token(RightBrace) |}|
//@[001:00002) |   └─Token(RightSquare) |]|
//@[002:00004) ├─Token(NewLine) |\r\n|
resource arrayExpressionErrors2 'Microsoft.Storage/storageAccounts@2019-06-01' = [for (account,k) in union([], 2): {
//@[000:00120) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00031) | ├─IdentifierSyntax
//@[009:00031) | | └─Token(Identifier) |arrayExpressionErrors2|
//@[032:00078) | ├─StringSyntax
//@[032:00078) | | └─Token(StringComplete) |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[079:00080) | ├─Token(Assignment) |=|
//@[081:00120) | └─ForSyntax
//@[081:00082) |   ├─Token(LeftSquare) |[|
//@[082:00085) |   ├─Token(Identifier) |for|
//@[086:00097) |   ├─VariableBlockSyntax
//@[086:00087) |   | ├─Token(LeftParen) |(|
//@[087:00094) |   | ├─LocalVariableSyntax
//@[087:00094) |   | | └─IdentifierSyntax
//@[087:00094) |   | |   └─Token(Identifier) |account|
//@[094:00095) |   | ├─Token(Comma) |,|
//@[095:00096) |   | ├─LocalVariableSyntax
//@[095:00096) |   | | └─IdentifierSyntax
//@[095:00096) |   | |   └─Token(Identifier) |k|
//@[096:00097) |   | └─Token(RightParen) |)|
//@[098:00100) |   ├─Token(Identifier) |in|
//@[101:00113) |   ├─FunctionCallSyntax
//@[101:00106) |   | ├─IdentifierSyntax
//@[101:00106) |   | | └─Token(Identifier) |union|
//@[106:00107) |   | ├─Token(LeftParen) |(|
//@[107:00109) |   | ├─FunctionArgumentSyntax
//@[107:00109) |   | | └─ArraySyntax
//@[107:00108) |   | |   ├─Token(LeftSquare) |[|
//@[108:00109) |   | |   └─Token(RightSquare) |]|
//@[109:00110) |   | ├─Token(Comma) |,|
//@[111:00112) |   | ├─FunctionArgumentSyntax
//@[111:00112) |   | | └─IntegerLiteralSyntax
//@[111:00112) |   | |   └─Token(Integer) |2|
//@[112:00113) |   | └─Token(RightParen) |)|
//@[113:00114) |   ├─Token(Colon) |:|
//@[115:00119) |   ├─ObjectSyntax
//@[115:00116) |   | ├─Token(LeftBrace) |{|
//@[116:00118) |   | ├─Token(NewLine) |\r\n|
}]
//@[000:00001) |   | └─Token(RightBrace) |}|
//@[001:00002) |   └─Token(RightSquare) |]|
//@[002:00006) ├─Token(NewLine) |\r\n\r\n|

// wrong array type
//@[019:00021) ├─Token(NewLine) |\r\n|
var notAnArray = true
//@[000:00021) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00014) | ├─IdentifierSyntax
//@[004:00014) | | └─Token(Identifier) |notAnArray|
//@[015:00016) | ├─Token(Assignment) |=|
//@[017:00021) | └─BooleanLiteralSyntax
//@[017:00021) |   └─Token(TrueKeyword) |true|
//@[021:00023) ├─Token(NewLine) |\r\n|
resource wrongArrayType 'Microsoft.Storage/storageAccounts@2019-06-01' = [for account in notAnArray: {
//@[000:00106) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00023) | ├─IdentifierSyntax
//@[009:00023) | | └─Token(Identifier) |wrongArrayType|
//@[024:00070) | ├─StringSyntax
//@[024:00070) | | └─Token(StringComplete) |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[071:00072) | ├─Token(Assignment) |=|
//@[073:00106) | └─ForSyntax
//@[073:00074) |   ├─Token(LeftSquare) |[|
//@[074:00077) |   ├─Token(Identifier) |for|
//@[078:00085) |   ├─LocalVariableSyntax
//@[078:00085) |   | └─IdentifierSyntax
//@[078:00085) |   |   └─Token(Identifier) |account|
//@[086:00088) |   ├─Token(Identifier) |in|
//@[089:00099) |   ├─VariableAccessSyntax
//@[089:00099) |   | └─IdentifierSyntax
//@[089:00099) |   |   └─Token(Identifier) |notAnArray|
//@[099:00100) |   ├─Token(Colon) |:|
//@[101:00105) |   ├─ObjectSyntax
//@[101:00102) |   | ├─Token(LeftBrace) |{|
//@[102:00104) |   | ├─Token(NewLine) |\r\n|
}]
//@[000:00001) |   | └─Token(RightBrace) |}|
//@[001:00002) |   └─Token(RightSquare) |]|
//@[002:00004) ├─Token(NewLine) |\r\n|
resource wrongArrayType2 'Microsoft.Storage/storageAccounts@2019-06-01' = [for (account,i) in notAnArray: {
//@[000:00111) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00024) | ├─IdentifierSyntax
//@[009:00024) | | └─Token(Identifier) |wrongArrayType2|
//@[025:00071) | ├─StringSyntax
//@[025:00071) | | └─Token(StringComplete) |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[072:00073) | ├─Token(Assignment) |=|
//@[074:00111) | └─ForSyntax
//@[074:00075) |   ├─Token(LeftSquare) |[|
//@[075:00078) |   ├─Token(Identifier) |for|
//@[079:00090) |   ├─VariableBlockSyntax
//@[079:00080) |   | ├─Token(LeftParen) |(|
//@[080:00087) |   | ├─LocalVariableSyntax
//@[080:00087) |   | | └─IdentifierSyntax
//@[080:00087) |   | |   └─Token(Identifier) |account|
//@[087:00088) |   | ├─Token(Comma) |,|
//@[088:00089) |   | ├─LocalVariableSyntax
//@[088:00089) |   | | └─IdentifierSyntax
//@[088:00089) |   | |   └─Token(Identifier) |i|
//@[089:00090) |   | └─Token(RightParen) |)|
//@[091:00093) |   ├─Token(Identifier) |in|
//@[094:00104) |   ├─VariableAccessSyntax
//@[094:00104) |   | └─IdentifierSyntax
//@[094:00104) |   |   └─Token(Identifier) |notAnArray|
//@[104:00105) |   ├─Token(Colon) |:|
//@[106:00110) |   ├─ObjectSyntax
//@[106:00107) |   | ├─Token(LeftBrace) |{|
//@[107:00109) |   | ├─Token(NewLine) |\r\n|
}]
//@[000:00001) |   | └─Token(RightBrace) |}|
//@[001:00002) |   └─Token(RightSquare) |]|
//@[002:00006) ├─Token(NewLine) |\r\n\r\n|

// wrong filter expression type
//@[031:00033) ├─Token(NewLine) |\r\n|
resource wrongFilterExpressionType 'Microsoft.Storage/storageAccounts@2019-06-01' = [for account in emptyArray: if(4) {
//@[000:00123) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00034) | ├─IdentifierSyntax
//@[009:00034) | | └─Token(Identifier) |wrongFilterExpressionType|
//@[035:00081) | ├─StringSyntax
//@[035:00081) | | └─Token(StringComplete) |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[082:00083) | ├─Token(Assignment) |=|
//@[084:00123) | └─ForSyntax
//@[084:00085) |   ├─Token(LeftSquare) |[|
//@[085:00088) |   ├─Token(Identifier) |for|
//@[089:00096) |   ├─LocalVariableSyntax
//@[089:00096) |   | └─IdentifierSyntax
//@[089:00096) |   |   └─Token(Identifier) |account|
//@[097:00099) |   ├─Token(Identifier) |in|
//@[100:00110) |   ├─VariableAccessSyntax
//@[100:00110) |   | └─IdentifierSyntax
//@[100:00110) |   |   └─Token(Identifier) |emptyArray|
//@[110:00111) |   ├─Token(Colon) |:|
//@[112:00122) |   ├─IfConditionSyntax
//@[112:00114) |   | ├─Token(Identifier) |if|
//@[114:00117) |   | ├─ParenthesizedExpressionSyntax
//@[114:00115) |   | | ├─Token(LeftParen) |(|
//@[115:00116) |   | | ├─IntegerLiteralSyntax
//@[115:00116) |   | | | └─Token(Integer) |4|
//@[116:00117) |   | | └─Token(RightParen) |)|
//@[118:00122) |   | └─ObjectSyntax
//@[118:00119) |   |   ├─Token(LeftBrace) |{|
//@[119:00121) |   |   ├─Token(NewLine) |\r\n|
}]
//@[000:00001) |   |   └─Token(RightBrace) |}|
//@[001:00002) |   └─Token(RightSquare) |]|
//@[002:00004) ├─Token(NewLine) |\r\n|
resource wrongFilterExpressionType2 'Microsoft.Storage/storageAccounts@2019-06-01' = [for (account,i) in emptyArray: if(concat('s')){
//@[000:00137) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00035) | ├─IdentifierSyntax
//@[009:00035) | | └─Token(Identifier) |wrongFilterExpressionType2|
//@[036:00082) | ├─StringSyntax
//@[036:00082) | | └─Token(StringComplete) |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[083:00084) | ├─Token(Assignment) |=|
//@[085:00137) | └─ForSyntax
//@[085:00086) |   ├─Token(LeftSquare) |[|
//@[086:00089) |   ├─Token(Identifier) |for|
//@[090:00101) |   ├─VariableBlockSyntax
//@[090:00091) |   | ├─Token(LeftParen) |(|
//@[091:00098) |   | ├─LocalVariableSyntax
//@[091:00098) |   | | └─IdentifierSyntax
//@[091:00098) |   | |   └─Token(Identifier) |account|
//@[098:00099) |   | ├─Token(Comma) |,|
//@[099:00100) |   | ├─LocalVariableSyntax
//@[099:00100) |   | | └─IdentifierSyntax
//@[099:00100) |   | |   └─Token(Identifier) |i|
//@[100:00101) |   | └─Token(RightParen) |)|
//@[102:00104) |   ├─Token(Identifier) |in|
//@[105:00115) |   ├─VariableAccessSyntax
//@[105:00115) |   | └─IdentifierSyntax
//@[105:00115) |   |   └─Token(Identifier) |emptyArray|
//@[115:00116) |   ├─Token(Colon) |:|
//@[117:00136) |   ├─IfConditionSyntax
//@[117:00119) |   | ├─Token(Identifier) |if|
//@[119:00132) |   | ├─ParenthesizedExpressionSyntax
//@[119:00120) |   | | ├─Token(LeftParen) |(|
//@[120:00131) |   | | ├─FunctionCallSyntax
//@[120:00126) |   | | | ├─IdentifierSyntax
//@[120:00126) |   | | | | └─Token(Identifier) |concat|
//@[126:00127) |   | | | ├─Token(LeftParen) |(|
//@[127:00130) |   | | | ├─FunctionArgumentSyntax
//@[127:00130) |   | | | | └─StringSyntax
//@[127:00130) |   | | | |   └─Token(StringComplete) |'s'|
//@[130:00131) |   | | | └─Token(RightParen) |)|
//@[131:00132) |   | | └─Token(RightParen) |)|
//@[132:00136) |   | └─ObjectSyntax
//@[132:00133) |   |   ├─Token(LeftBrace) |{|
//@[133:00135) |   |   ├─Token(NewLine) |\r\n|
}]
//@[000:00001) |   |   └─Token(RightBrace) |}|
//@[001:00002) |   └─Token(RightSquare) |]|
//@[002:00006) ├─Token(NewLine) |\r\n\r\n|

// missing required properties
//@[030:00032) ├─Token(NewLine) |\r\n|
resource missingRequiredProperties 'Microsoft.Storage/storageAccounts@2019-06-01' = [for account in []: {
//@[000:00109) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00034) | ├─IdentifierSyntax
//@[009:00034) | | └─Token(Identifier) |missingRequiredProperties|
//@[035:00081) | ├─StringSyntax
//@[035:00081) | | └─Token(StringComplete) |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[082:00083) | ├─Token(Assignment) |=|
//@[084:00109) | └─ForSyntax
//@[084:00085) |   ├─Token(LeftSquare) |[|
//@[085:00088) |   ├─Token(Identifier) |for|
//@[089:00096) |   ├─LocalVariableSyntax
//@[089:00096) |   | └─IdentifierSyntax
//@[089:00096) |   |   └─Token(Identifier) |account|
//@[097:00099) |   ├─Token(Identifier) |in|
//@[100:00102) |   ├─ArraySyntax
//@[100:00101) |   | ├─Token(LeftSquare) |[|
//@[101:00102) |   | └─Token(RightSquare) |]|
//@[102:00103) |   ├─Token(Colon) |:|
//@[104:00108) |   ├─ObjectSyntax
//@[104:00105) |   | ├─Token(LeftBrace) |{|
//@[105:00107) |   | ├─Token(NewLine) |\r\n|
}]
//@[000:00001) |   | └─Token(RightBrace) |}|
//@[001:00002) |   └─Token(RightSquare) |]|
//@[002:00004) ├─Token(NewLine) |\r\n|
resource missingRequiredProperties2 'Microsoft.Storage/storageAccounts@2019-06-01' = [for (account,j) in []: {
//@[000:00114) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00035) | ├─IdentifierSyntax
//@[009:00035) | | └─Token(Identifier) |missingRequiredProperties2|
//@[036:00082) | ├─StringSyntax
//@[036:00082) | | └─Token(StringComplete) |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[083:00084) | ├─Token(Assignment) |=|
//@[085:00114) | └─ForSyntax
//@[085:00086) |   ├─Token(LeftSquare) |[|
//@[086:00089) |   ├─Token(Identifier) |for|
//@[090:00101) |   ├─VariableBlockSyntax
//@[090:00091) |   | ├─Token(LeftParen) |(|
//@[091:00098) |   | ├─LocalVariableSyntax
//@[091:00098) |   | | └─IdentifierSyntax
//@[091:00098) |   | |   └─Token(Identifier) |account|
//@[098:00099) |   | ├─Token(Comma) |,|
//@[099:00100) |   | ├─LocalVariableSyntax
//@[099:00100) |   | | └─IdentifierSyntax
//@[099:00100) |   | |   └─Token(Identifier) |j|
//@[100:00101) |   | └─Token(RightParen) |)|
//@[102:00104) |   ├─Token(Identifier) |in|
//@[105:00107) |   ├─ArraySyntax
//@[105:00106) |   | ├─Token(LeftSquare) |[|
//@[106:00107) |   | └─Token(RightSquare) |]|
//@[107:00108) |   ├─Token(Colon) |:|
//@[109:00113) |   ├─ObjectSyntax
//@[109:00110) |   | ├─Token(LeftBrace) |{|
//@[110:00112) |   | ├─Token(NewLine) |\r\n|
}]
//@[000:00001) |   | └─Token(RightBrace) |}|
//@[001:00002) |   └─Token(RightSquare) |]|
//@[002:00006) ├─Token(NewLine) |\r\n\r\n|

// fewer missing required properties and a wrong property
//@[057:00059) ├─Token(NewLine) |\r\n|
resource missingFewerRequiredProperties 'Microsoft.Storage/storageAccounts@2019-06-01' = [for account in []: {
//@[000:00196) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00039) | ├─IdentifierSyntax
//@[009:00039) | | └─Token(Identifier) |missingFewerRequiredProperties|
//@[040:00086) | ├─StringSyntax
//@[040:00086) | | └─Token(StringComplete) |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[087:00088) | ├─Token(Assignment) |=|
//@[089:00196) | └─ForSyntax
//@[089:00090) |   ├─Token(LeftSquare) |[|
//@[090:00093) |   ├─Token(Identifier) |for|
//@[094:00101) |   ├─LocalVariableSyntax
//@[094:00101) |   | └─IdentifierSyntax
//@[094:00101) |   |   └─Token(Identifier) |account|
//@[102:00104) |   ├─Token(Identifier) |in|
//@[105:00107) |   ├─ArraySyntax
//@[105:00106) |   | ├─Token(LeftSquare) |[|
//@[106:00107) |   | └─Token(RightSquare) |]|
//@[107:00108) |   ├─Token(Colon) |:|
//@[109:00195) |   ├─ObjectSyntax
//@[109:00110) |   | ├─Token(LeftBrace) |{|
//@[110:00112) |   | ├─Token(NewLine) |\r\n|
  name: account
//@[002:00015) |   | ├─ObjectPropertySyntax
//@[002:00006) |   | | ├─IdentifierSyntax
//@[002:00006) |   | | | └─Token(Identifier) |name|
//@[006:00007) |   | | ├─Token(Colon) |:|
//@[008:00015) |   | | └─VariableAccessSyntax
//@[008:00015) |   | |   └─IdentifierSyntax
//@[008:00015) |   | |     └─Token(Identifier) |account|
//@[015:00017) |   | ├─Token(NewLine) |\r\n|
  location: 'eastus42'
//@[002:00022) |   | ├─ObjectPropertySyntax
//@[002:00010) |   | | ├─IdentifierSyntax
//@[002:00010) |   | | | └─Token(Identifier) |location|
//@[010:00011) |   | | ├─Token(Colon) |:|
//@[012:00022) |   | | └─StringSyntax
//@[012:00022) |   | |   └─Token(StringComplete) |'eastus42'|
//@[022:00024) |   | ├─Token(NewLine) |\r\n|
  properties: {
//@[002:00039) |   | ├─ObjectPropertySyntax
//@[002:00012) |   | | ├─IdentifierSyntax
//@[002:00012) |   | | | └─Token(Identifier) |properties|
//@[012:00013) |   | | ├─Token(Colon) |:|
//@[014:00039) |   | | └─ObjectSyntax
//@[014:00015) |   | |   ├─Token(LeftBrace) |{|
//@[015:00017) |   | |   ├─Token(NewLine) |\r\n|
    wrong: 'test'
//@[004:00017) |   | |   ├─ObjectPropertySyntax
//@[004:00009) |   | |   | ├─IdentifierSyntax
//@[004:00009) |   | |   | | └─Token(Identifier) |wrong|
//@[009:00010) |   | |   | ├─Token(Colon) |:|
//@[011:00017) |   | |   | └─StringSyntax
//@[011:00017) |   | |   |   └─Token(StringComplete) |'test'|
//@[017:00019) |   | |   ├─Token(NewLine) |\r\n|
  }
//@[002:00003) |   | |   └─Token(RightBrace) |}|
//@[003:00005) |   | ├─Token(NewLine) |\r\n|
}]
//@[000:00001) |   | └─Token(RightBrace) |}|
//@[001:00002) |   └─Token(RightSquare) |]|
//@[002:00006) ├─Token(NewLine) |\r\n\r\n|

// wrong property inside the nested property loop
//@[049:00051) ├─Token(NewLine) |\r\n|
resource wrongPropertyInNestedLoop 'Microsoft.Network/virtualNetworks@2020-06-01' = [for i in range(0, 3): {
//@[000:00262) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00034) | ├─IdentifierSyntax
//@[009:00034) | | └─Token(Identifier) |wrongPropertyInNestedLoop|
//@[035:00081) | ├─StringSyntax
//@[035:00081) | | └─Token(StringComplete) |'Microsoft.Network/virtualNetworks@2020-06-01'|
//@[082:00083) | ├─Token(Assignment) |=|
//@[084:00262) | └─ForSyntax
//@[084:00085) |   ├─Token(LeftSquare) |[|
//@[085:00088) |   ├─Token(Identifier) |for|
//@[089:00090) |   ├─LocalVariableSyntax
//@[089:00090) |   | └─IdentifierSyntax
//@[089:00090) |   |   └─Token(Identifier) |i|
//@[091:00093) |   ├─Token(Identifier) |in|
//@[094:00105) |   ├─FunctionCallSyntax
//@[094:00099) |   | ├─IdentifierSyntax
//@[094:00099) |   | | └─Token(Identifier) |range|
//@[099:00100) |   | ├─Token(LeftParen) |(|
//@[100:00101) |   | ├─FunctionArgumentSyntax
//@[100:00101) |   | | └─IntegerLiteralSyntax
//@[100:00101) |   | |   └─Token(Integer) |0|
//@[101:00102) |   | ├─Token(Comma) |,|
//@[103:00104) |   | ├─FunctionArgumentSyntax
//@[103:00104) |   | | └─IntegerLiteralSyntax
//@[103:00104) |   | |   └─Token(Integer) |3|
//@[104:00105) |   | └─Token(RightParen) |)|
//@[105:00106) |   ├─Token(Colon) |:|
//@[107:00261) |   ├─ObjectSyntax
//@[107:00108) |   | ├─Token(LeftBrace) |{|
//@[108:00110) |   | ├─Token(NewLine) |\r\n|
  name: 'vnet-${i}'
//@[002:00019) |   | ├─ObjectPropertySyntax
//@[002:00006) |   | | ├─IdentifierSyntax
//@[002:00006) |   | | | └─Token(Identifier) |name|
//@[006:00007) |   | | ├─Token(Colon) |:|
//@[008:00019) |   | | └─StringSyntax
//@[008:00016) |   | |   ├─Token(StringLeftPiece) |'vnet-${|
//@[016:00017) |   | |   ├─VariableAccessSyntax
//@[016:00017) |   | |   | └─IdentifierSyntax
//@[016:00017) |   | |   |   └─Token(Identifier) |i|
//@[017:00019) |   | |   └─Token(StringRightPiece) |}'|
//@[019:00021) |   | ├─Token(NewLine) |\r\n|
  properties: {
//@[002:00127) |   | ├─ObjectPropertySyntax
//@[002:00012) |   | | ├─IdentifierSyntax
//@[002:00012) |   | | | └─Token(Identifier) |properties|
//@[012:00013) |   | | ├─Token(Colon) |:|
//@[014:00127) |   | | └─ObjectSyntax
//@[014:00015) |   | |   ├─Token(LeftBrace) |{|
//@[015:00017) |   | |   ├─Token(NewLine) |\r\n|
    subnets: [for j in range(0, 4): {
//@[004:00105) |   | |   ├─ObjectPropertySyntax
//@[004:00011) |   | |   | ├─IdentifierSyntax
//@[004:00011) |   | |   | | └─Token(Identifier) |subnets|
//@[011:00012) |   | |   | ├─Token(Colon) |:|
//@[013:00105) |   | |   | └─ForSyntax
//@[013:00014) |   | |   |   ├─Token(LeftSquare) |[|
//@[014:00017) |   | |   |   ├─Token(Identifier) |for|
//@[018:00019) |   | |   |   ├─LocalVariableSyntax
//@[018:00019) |   | |   |   | └─IdentifierSyntax
//@[018:00019) |   | |   |   |   └─Token(Identifier) |j|
//@[020:00022) |   | |   |   ├─Token(Identifier) |in|
//@[023:00034) |   | |   |   ├─FunctionCallSyntax
//@[023:00028) |   | |   |   | ├─IdentifierSyntax
//@[023:00028) |   | |   |   | | └─Token(Identifier) |range|
//@[028:00029) |   | |   |   | ├─Token(LeftParen) |(|
//@[029:00030) |   | |   |   | ├─FunctionArgumentSyntax
//@[029:00030) |   | |   |   | | └─IntegerLiteralSyntax
//@[029:00030) |   | |   |   | |   └─Token(Integer) |0|
//@[030:00031) |   | |   |   | ├─Token(Comma) |,|
//@[032:00033) |   | |   |   | ├─FunctionArgumentSyntax
//@[032:00033) |   | |   |   | | └─IntegerLiteralSyntax
//@[032:00033) |   | |   |   | |   └─Token(Integer) |4|
//@[033:00034) |   | |   |   | └─Token(RightParen) |)|
//@[034:00035) |   | |   |   ├─Token(Colon) |:|
//@[036:00104) |   | |   |   ├─ObjectSyntax
//@[036:00037) |   | |   |   | ├─Token(LeftBrace) |{|
//@[037:00039) |   | |   |   | ├─Token(NewLine) |\r\n|
      doesNotExist: 'test'
//@[006:00026) |   | |   |   | ├─ObjectPropertySyntax
//@[006:00018) |   | |   |   | | ├─IdentifierSyntax
//@[006:00018) |   | |   |   | | | └─Token(Identifier) |doesNotExist|
//@[018:00019) |   | |   |   | | ├─Token(Colon) |:|
//@[020:00026) |   | |   |   | | └─StringSyntax
//@[020:00026) |   | |   |   | |   └─Token(StringComplete) |'test'|
//@[026:00028) |   | |   |   | ├─Token(NewLine) |\r\n|
      name: 'subnet-${i}-${j}'
//@[006:00030) |   | |   |   | ├─ObjectPropertySyntax
//@[006:00010) |   | |   |   | | ├─IdentifierSyntax
//@[006:00010) |   | |   |   | | | └─Token(Identifier) |name|
//@[010:00011) |   | |   |   | | ├─Token(Colon) |:|
//@[012:00030) |   | |   |   | | └─StringSyntax
//@[012:00022) |   | |   |   | |   ├─Token(StringLeftPiece) |'subnet-${|
//@[022:00023) |   | |   |   | |   ├─VariableAccessSyntax
//@[022:00023) |   | |   |   | |   | └─IdentifierSyntax
//@[022:00023) |   | |   |   | |   |   └─Token(Identifier) |i|
//@[023:00027) |   | |   |   | |   ├─Token(StringMiddlePiece) |}-${|
//@[027:00028) |   | |   |   | |   ├─VariableAccessSyntax
//@[027:00028) |   | |   |   | |   | └─IdentifierSyntax
//@[027:00028) |   | |   |   | |   |   └─Token(Identifier) |j|
//@[028:00030) |   | |   |   | |   └─Token(StringRightPiece) |}'|
//@[030:00032) |   | |   |   | ├─Token(NewLine) |\r\n|
    }]
//@[004:00005) |   | |   |   | └─Token(RightBrace) |}|
//@[005:00006) |   | |   |   └─Token(RightSquare) |]|
//@[006:00008) |   | |   ├─Token(NewLine) |\r\n|
  }
//@[002:00003) |   | |   └─Token(RightBrace) |}|
//@[003:00005) |   | ├─Token(NewLine) |\r\n|
}]
//@[000:00001) |   | └─Token(RightBrace) |}|
//@[001:00002) |   └─Token(RightSquare) |]|
//@[002:00004) ├─Token(NewLine) |\r\n|
resource wrongPropertyInNestedLoop2 'Microsoft.Network/virtualNetworks@2020-06-01' = [for (i,k) in range(0, 3): {
//@[000:00272) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00035) | ├─IdentifierSyntax
//@[009:00035) | | └─Token(Identifier) |wrongPropertyInNestedLoop2|
//@[036:00082) | ├─StringSyntax
//@[036:00082) | | └─Token(StringComplete) |'Microsoft.Network/virtualNetworks@2020-06-01'|
//@[083:00084) | ├─Token(Assignment) |=|
//@[085:00272) | └─ForSyntax
//@[085:00086) |   ├─Token(LeftSquare) |[|
//@[086:00089) |   ├─Token(Identifier) |for|
//@[090:00095) |   ├─VariableBlockSyntax
//@[090:00091) |   | ├─Token(LeftParen) |(|
//@[091:00092) |   | ├─LocalVariableSyntax
//@[091:00092) |   | | └─IdentifierSyntax
//@[091:00092) |   | |   └─Token(Identifier) |i|
//@[092:00093) |   | ├─Token(Comma) |,|
//@[093:00094) |   | ├─LocalVariableSyntax
//@[093:00094) |   | | └─IdentifierSyntax
//@[093:00094) |   | |   └─Token(Identifier) |k|
//@[094:00095) |   | └─Token(RightParen) |)|
//@[096:00098) |   ├─Token(Identifier) |in|
//@[099:00110) |   ├─FunctionCallSyntax
//@[099:00104) |   | ├─IdentifierSyntax
//@[099:00104) |   | | └─Token(Identifier) |range|
//@[104:00105) |   | ├─Token(LeftParen) |(|
//@[105:00106) |   | ├─FunctionArgumentSyntax
//@[105:00106) |   | | └─IntegerLiteralSyntax
//@[105:00106) |   | |   └─Token(Integer) |0|
//@[106:00107) |   | ├─Token(Comma) |,|
//@[108:00109) |   | ├─FunctionArgumentSyntax
//@[108:00109) |   | | └─IntegerLiteralSyntax
//@[108:00109) |   | |   └─Token(Integer) |3|
//@[109:00110) |   | └─Token(RightParen) |)|
//@[110:00111) |   ├─Token(Colon) |:|
//@[112:00271) |   ├─ObjectSyntax
//@[112:00113) |   | ├─Token(LeftBrace) |{|
//@[113:00115) |   | ├─Token(NewLine) |\r\n|
  name: 'vnet-${i}'
//@[002:00019) |   | ├─ObjectPropertySyntax
//@[002:00006) |   | | ├─IdentifierSyntax
//@[002:00006) |   | | | └─Token(Identifier) |name|
//@[006:00007) |   | | ├─Token(Colon) |:|
//@[008:00019) |   | | └─StringSyntax
//@[008:00016) |   | |   ├─Token(StringLeftPiece) |'vnet-${|
//@[016:00017) |   | |   ├─VariableAccessSyntax
//@[016:00017) |   | |   | └─IdentifierSyntax
//@[016:00017) |   | |   |   └─Token(Identifier) |i|
//@[017:00019) |   | |   └─Token(StringRightPiece) |}'|
//@[019:00021) |   | ├─Token(NewLine) |\r\n|
  properties: {
//@[002:00132) |   | ├─ObjectPropertySyntax
//@[002:00012) |   | | ├─IdentifierSyntax
//@[002:00012) |   | | | └─Token(Identifier) |properties|
//@[012:00013) |   | | ├─Token(Colon) |:|
//@[014:00132) |   | | └─ObjectSyntax
//@[014:00015) |   | |   ├─Token(LeftBrace) |{|
//@[015:00017) |   | |   ├─Token(NewLine) |\r\n|
    subnets: [for j in range(0, 4): {
//@[004:00110) |   | |   ├─ObjectPropertySyntax
//@[004:00011) |   | |   | ├─IdentifierSyntax
//@[004:00011) |   | |   | | └─Token(Identifier) |subnets|
//@[011:00012) |   | |   | ├─Token(Colon) |:|
//@[013:00110) |   | |   | └─ForSyntax
//@[013:00014) |   | |   |   ├─Token(LeftSquare) |[|
//@[014:00017) |   | |   |   ├─Token(Identifier) |for|
//@[018:00019) |   | |   |   ├─LocalVariableSyntax
//@[018:00019) |   | |   |   | └─IdentifierSyntax
//@[018:00019) |   | |   |   |   └─Token(Identifier) |j|
//@[020:00022) |   | |   |   ├─Token(Identifier) |in|
//@[023:00034) |   | |   |   ├─FunctionCallSyntax
//@[023:00028) |   | |   |   | ├─IdentifierSyntax
//@[023:00028) |   | |   |   | | └─Token(Identifier) |range|
//@[028:00029) |   | |   |   | ├─Token(LeftParen) |(|
//@[029:00030) |   | |   |   | ├─FunctionArgumentSyntax
//@[029:00030) |   | |   |   | | └─IntegerLiteralSyntax
//@[029:00030) |   | |   |   | |   └─Token(Integer) |0|
//@[030:00031) |   | |   |   | ├─Token(Comma) |,|
//@[032:00033) |   | |   |   | ├─FunctionArgumentSyntax
//@[032:00033) |   | |   |   | | └─IntegerLiteralSyntax
//@[032:00033) |   | |   |   | |   └─Token(Integer) |4|
//@[033:00034) |   | |   |   | └─Token(RightParen) |)|
//@[034:00035) |   | |   |   ├─Token(Colon) |:|
//@[036:00109) |   | |   |   ├─ObjectSyntax
//@[036:00037) |   | |   |   | ├─Token(LeftBrace) |{|
//@[037:00039) |   | |   |   | ├─Token(NewLine) |\r\n|
      doesNotExist: 'test'
//@[006:00026) |   | |   |   | ├─ObjectPropertySyntax
//@[006:00018) |   | |   |   | | ├─IdentifierSyntax
//@[006:00018) |   | |   |   | | | └─Token(Identifier) |doesNotExist|
//@[018:00019) |   | |   |   | | ├─Token(Colon) |:|
//@[020:00026) |   | |   |   | | └─StringSyntax
//@[020:00026) |   | |   |   | |   └─Token(StringComplete) |'test'|
//@[026:00028) |   | |   |   | ├─Token(NewLine) |\r\n|
      name: 'subnet-${i}-${j}-${k}'
//@[006:00035) |   | |   |   | ├─ObjectPropertySyntax
//@[006:00010) |   | |   |   | | ├─IdentifierSyntax
//@[006:00010) |   | |   |   | | | └─Token(Identifier) |name|
//@[010:00011) |   | |   |   | | ├─Token(Colon) |:|
//@[012:00035) |   | |   |   | | └─StringSyntax
//@[012:00022) |   | |   |   | |   ├─Token(StringLeftPiece) |'subnet-${|
//@[022:00023) |   | |   |   | |   ├─VariableAccessSyntax
//@[022:00023) |   | |   |   | |   | └─IdentifierSyntax
//@[022:00023) |   | |   |   | |   |   └─Token(Identifier) |i|
//@[023:00027) |   | |   |   | |   ├─Token(StringMiddlePiece) |}-${|
//@[027:00028) |   | |   |   | |   ├─VariableAccessSyntax
//@[027:00028) |   | |   |   | |   | └─IdentifierSyntax
//@[027:00028) |   | |   |   | |   |   └─Token(Identifier) |j|
//@[028:00032) |   | |   |   | |   ├─Token(StringMiddlePiece) |}-${|
//@[032:00033) |   | |   |   | |   ├─VariableAccessSyntax
//@[032:00033) |   | |   |   | |   | └─IdentifierSyntax
//@[032:00033) |   | |   |   | |   |   └─Token(Identifier) |k|
//@[033:00035) |   | |   |   | |   └─Token(StringRightPiece) |}'|
//@[035:00037) |   | |   |   | ├─Token(NewLine) |\r\n|
    }]
//@[004:00005) |   | |   |   | └─Token(RightBrace) |}|
//@[005:00006) |   | |   |   └─Token(RightSquare) |]|
//@[006:00008) |   | |   ├─Token(NewLine) |\r\n|
  }
//@[002:00003) |   | |   └─Token(RightBrace) |}|
//@[003:00005) |   | ├─Token(NewLine) |\r\n|
}]
//@[000:00001) |   | └─Token(RightBrace) |}|
//@[001:00002) |   └─Token(RightSquare) |]|
//@[002:00006) ├─Token(NewLine) |\r\n\r\n|

// nonexistent arrays and loop variables
//@[040:00042) ├─Token(NewLine) |\r\n|
resource nonexistentArrays 'Microsoft.Network/virtualNetworks@2020-06-01' = [for i in notAThing: {
//@[000:00280) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00026) | ├─IdentifierSyntax
//@[009:00026) | | └─Token(Identifier) |nonexistentArrays|
//@[027:00073) | ├─StringSyntax
//@[027:00073) | | └─Token(StringComplete) |'Microsoft.Network/virtualNetworks@2020-06-01'|
//@[074:00075) | ├─Token(Assignment) |=|
//@[076:00280) | └─ForSyntax
//@[076:00077) |   ├─Token(LeftSquare) |[|
//@[077:00080) |   ├─Token(Identifier) |for|
//@[081:00082) |   ├─LocalVariableSyntax
//@[081:00082) |   | └─IdentifierSyntax
//@[081:00082) |   |   └─Token(Identifier) |i|
//@[083:00085) |   ├─Token(Identifier) |in|
//@[086:00095) |   ├─VariableAccessSyntax
//@[086:00095) |   | └─IdentifierSyntax
//@[086:00095) |   |   └─Token(Identifier) |notAThing|
//@[095:00096) |   ├─Token(Colon) |:|
//@[097:00279) |   ├─ObjectSyntax
//@[097:00098) |   | ├─Token(LeftBrace) |{|
//@[098:00100) |   | ├─Token(NewLine) |\r\n|
  name: 'vnet-${justPlainWrong}'
//@[002:00032) |   | ├─ObjectPropertySyntax
//@[002:00006) |   | | ├─IdentifierSyntax
//@[002:00006) |   | | | └─Token(Identifier) |name|
//@[006:00007) |   | | ├─Token(Colon) |:|
//@[008:00032) |   | | └─StringSyntax
//@[008:00016) |   | |   ├─Token(StringLeftPiece) |'vnet-${|
//@[016:00030) |   | |   ├─VariableAccessSyntax
//@[016:00030) |   | |   | └─IdentifierSyntax
//@[016:00030) |   | |   |   └─Token(Identifier) |justPlainWrong|
//@[030:00032) |   | |   └─Token(StringRightPiece) |}'|
//@[032:00034) |   | ├─Token(NewLine) |\r\n|
  properties: {
//@[002:00142) |   | ├─ObjectPropertySyntax
//@[002:00012) |   | | ├─IdentifierSyntax
//@[002:00012) |   | | | └─Token(Identifier) |properties|
//@[012:00013) |   | | ├─Token(Colon) |:|
//@[014:00142) |   | | └─ObjectSyntax
//@[014:00015) |   | |   ├─Token(LeftBrace) |{|
//@[015:00017) |   | |   ├─Token(NewLine) |\r\n|
    subnets: [for j in alsoNotAThing: {
//@[004:00120) |   | |   ├─ObjectPropertySyntax
//@[004:00011) |   | |   | ├─IdentifierSyntax
//@[004:00011) |   | |   | | └─Token(Identifier) |subnets|
//@[011:00012) |   | |   | ├─Token(Colon) |:|
//@[013:00120) |   | |   | └─ForSyntax
//@[013:00014) |   | |   |   ├─Token(LeftSquare) |[|
//@[014:00017) |   | |   |   ├─Token(Identifier) |for|
//@[018:00019) |   | |   |   ├─LocalVariableSyntax
//@[018:00019) |   | |   |   | └─IdentifierSyntax
//@[018:00019) |   | |   |   |   └─Token(Identifier) |j|
//@[020:00022) |   | |   |   ├─Token(Identifier) |in|
//@[023:00036) |   | |   |   ├─VariableAccessSyntax
//@[023:00036) |   | |   |   | └─IdentifierSyntax
//@[023:00036) |   | |   |   |   └─Token(Identifier) |alsoNotAThing|
//@[036:00037) |   | |   |   ├─Token(Colon) |:|
//@[038:00119) |   | |   |   ├─ObjectSyntax
//@[038:00039) |   | |   |   | ├─Token(LeftBrace) |{|
//@[039:00041) |   | |   |   | ├─Token(NewLine) |\r\n|
      doesNotExist: 'test'
//@[006:00026) |   | |   |   | ├─ObjectPropertySyntax
//@[006:00018) |   | |   |   | | ├─IdentifierSyntax
//@[006:00018) |   | |   |   | | | └─Token(Identifier) |doesNotExist|
//@[018:00019) |   | |   |   | | ├─Token(Colon) |:|
//@[020:00026) |   | |   |   | | └─StringSyntax
//@[020:00026) |   | |   |   | |   └─Token(StringComplete) |'test'|
//@[026:00028) |   | |   |   | ├─Token(NewLine) |\r\n|
      name: 'subnet-${fake}-${totallyFake}'
//@[006:00043) |   | |   |   | ├─ObjectPropertySyntax
//@[006:00010) |   | |   |   | | ├─IdentifierSyntax
//@[006:00010) |   | |   |   | | | └─Token(Identifier) |name|
//@[010:00011) |   | |   |   | | ├─Token(Colon) |:|
//@[012:00043) |   | |   |   | | └─StringSyntax
//@[012:00022) |   | |   |   | |   ├─Token(StringLeftPiece) |'subnet-${|
//@[022:00026) |   | |   |   | |   ├─VariableAccessSyntax
//@[022:00026) |   | |   |   | |   | └─IdentifierSyntax
//@[022:00026) |   | |   |   | |   |   └─Token(Identifier) |fake|
//@[026:00030) |   | |   |   | |   ├─Token(StringMiddlePiece) |}-${|
//@[030:00041) |   | |   |   | |   ├─VariableAccessSyntax
//@[030:00041) |   | |   |   | |   | └─IdentifierSyntax
//@[030:00041) |   | |   |   | |   |   └─Token(Identifier) |totallyFake|
//@[041:00043) |   | |   |   | |   └─Token(StringRightPiece) |}'|
//@[043:00045) |   | |   |   | ├─Token(NewLine) |\r\n|
    }]
//@[004:00005) |   | |   |   | └─Token(RightBrace) |}|
//@[005:00006) |   | |   |   └─Token(RightSquare) |]|
//@[006:00008) |   | |   ├─Token(NewLine) |\r\n|
  }
//@[002:00003) |   | |   └─Token(RightBrace) |}|
//@[003:00005) |   | ├─Token(NewLine) |\r\n|
}]
//@[000:00001) |   | └─Token(RightBrace) |}|
//@[001:00002) |   └─Token(RightSquare) |]|
//@[002:00006) ├─Token(NewLine) |\r\n\r\n|

// property loops cannot be nested
//@[034:00036) ├─Token(NewLine) |\r\n|
resource propertyLoopsCannotNest 'Microsoft.Storage/storageAccounts@2019-06-01' = [for account in storageAccounts: {
//@[000:00428) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00032) | ├─IdentifierSyntax
//@[009:00032) | | └─Token(Identifier) |propertyLoopsCannotNest|
//@[033:00079) | ├─StringSyntax
//@[033:00079) | | └─Token(StringComplete) |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[080:00081) | ├─Token(Assignment) |=|
//@[082:00428) | └─ForSyntax
//@[082:00083) |   ├─Token(LeftSquare) |[|
//@[083:00086) |   ├─Token(Identifier) |for|
//@[087:00094) |   ├─LocalVariableSyntax
//@[087:00094) |   | └─IdentifierSyntax
//@[087:00094) |   |   └─Token(Identifier) |account|
//@[095:00097) |   ├─Token(Identifier) |in|
//@[098:00113) |   ├─VariableAccessSyntax
//@[098:00113) |   | └─IdentifierSyntax
//@[098:00113) |   |   └─Token(Identifier) |storageAccounts|
//@[113:00114) |   ├─Token(Colon) |:|
//@[115:00427) |   ├─ObjectSyntax
//@[115:00116) |   | ├─Token(LeftBrace) |{|
//@[116:00118) |   | ├─Token(NewLine) |\r\n|
  name: account.name
//@[002:00020) |   | ├─ObjectPropertySyntax
//@[002:00006) |   | | ├─IdentifierSyntax
//@[002:00006) |   | | | └─Token(Identifier) |name|
//@[006:00007) |   | | ├─Token(Colon) |:|
//@[008:00020) |   | | └─PropertyAccessSyntax
//@[008:00015) |   | |   ├─VariableAccessSyntax
//@[008:00015) |   | |   | └─IdentifierSyntax
//@[008:00015) |   | |   |   └─Token(Identifier) |account|
//@[015:00016) |   | |   ├─Token(Dot) |.|
//@[016:00020) |   | |   └─IdentifierSyntax
//@[016:00020) |   | |     └─Token(Identifier) |name|
//@[020:00022) |   | ├─Token(NewLine) |\r\n|
  location: account.location
//@[002:00028) |   | ├─ObjectPropertySyntax
//@[002:00010) |   | | ├─IdentifierSyntax
//@[002:00010) |   | | | └─Token(Identifier) |location|
//@[010:00011) |   | | ├─Token(Colon) |:|
//@[012:00028) |   | | └─PropertyAccessSyntax
//@[012:00019) |   | |   ├─VariableAccessSyntax
//@[012:00019) |   | |   | └─IdentifierSyntax
//@[012:00019) |   | |   |   └─Token(Identifier) |account|
//@[019:00020) |   | |   ├─Token(Dot) |.|
//@[020:00028) |   | |   └─IdentifierSyntax
//@[020:00028) |   | |     └─Token(Identifier) |location|
//@[028:00030) |   | ├─Token(NewLine) |\r\n|
  sku: {
//@[002:00039) |   | ├─ObjectPropertySyntax
//@[002:00005) |   | | ├─IdentifierSyntax
//@[002:00005) |   | | | └─Token(Identifier) |sku|
//@[005:00006) |   | | ├─Token(Colon) |:|
//@[007:00039) |   | | └─ObjectSyntax
//@[007:00008) |   | |   ├─Token(LeftBrace) |{|
//@[008:00010) |   | |   ├─Token(NewLine) |\r\n|
    name: 'Standard_LRS'
//@[004:00024) |   | |   ├─ObjectPropertySyntax
//@[004:00008) |   | |   | ├─IdentifierSyntax
//@[004:00008) |   | |   | | └─Token(Identifier) |name|
//@[008:00009) |   | |   | ├─Token(Colon) |:|
//@[010:00024) |   | |   | └─StringSyntax
//@[010:00024) |   | |   |   └─Token(StringComplete) |'Standard_LRS'|
//@[024:00026) |   | |   ├─Token(NewLine) |\r\n|
  }
//@[002:00003) |   | |   └─Token(RightBrace) |}|
//@[003:00005) |   | ├─Token(NewLine) |\r\n|
  kind: 'StorageV2'
//@[002:00019) |   | ├─ObjectPropertySyntax
//@[002:00006) |   | | ├─IdentifierSyntax
//@[002:00006) |   | | | └─Token(Identifier) |kind|
//@[006:00007) |   | | ├─Token(Colon) |:|
//@[008:00019) |   | | └─StringSyntax
//@[008:00019) |   | |   └─Token(StringComplete) |'StorageV2'|
//@[019:00021) |   | ├─Token(NewLine) |\r\n|
  properties: {
//@[002:00192) |   | ├─ObjectPropertySyntax
//@[002:00012) |   | | ├─IdentifierSyntax
//@[002:00012) |   | | | └─Token(Identifier) |properties|
//@[012:00013) |   | | ├─Token(Colon) |:|
//@[014:00192) |   | | └─ObjectSyntax
//@[014:00015) |   | |   ├─Token(LeftBrace) |{|
//@[015:00019) |   | |   ├─Token(NewLine) |\r\n\r\n|

    networkAcls: {
//@[004:00168) |   | |   ├─ObjectPropertySyntax
//@[004:00015) |   | |   | ├─IdentifierSyntax
//@[004:00015) |   | |   | | └─Token(Identifier) |networkAcls|
//@[015:00016) |   | |   | ├─Token(Colon) |:|
//@[017:00168) |   | |   | └─ObjectSyntax
//@[017:00018) |   | |   |   ├─Token(LeftBrace) |{|
//@[018:00020) |   | |   |   ├─Token(NewLine) |\r\n|
      virtualNetworkRules: [for rule in []: {
//@[006:00141) |   | |   |   ├─ObjectPropertySyntax
//@[006:00025) |   | |   |   | ├─IdentifierSyntax
//@[006:00025) |   | |   |   | | └─Token(Identifier) |virtualNetworkRules|
//@[025:00026) |   | |   |   | ├─Token(Colon) |:|
//@[027:00141) |   | |   |   | └─ForSyntax
//@[027:00028) |   | |   |   |   ├─Token(LeftSquare) |[|
//@[028:00031) |   | |   |   |   ├─Token(Identifier) |for|
//@[032:00036) |   | |   |   |   ├─LocalVariableSyntax
//@[032:00036) |   | |   |   |   | └─IdentifierSyntax
//@[032:00036) |   | |   |   |   |   └─Token(Identifier) |rule|
//@[037:00039) |   | |   |   |   ├─Token(Identifier) |in|
//@[040:00042) |   | |   |   |   ├─ArraySyntax
//@[040:00041) |   | |   |   |   | ├─Token(LeftSquare) |[|
//@[041:00042) |   | |   |   |   | └─Token(RightSquare) |]|
//@[042:00043) |   | |   |   |   ├─Token(Colon) |:|
//@[044:00140) |   | |   |   |   ├─ObjectSyntax
//@[044:00045) |   | |   |   |   | ├─Token(LeftBrace) |{|
//@[045:00047) |   | |   |   |   | ├─Token(NewLine) |\r\n|
        id: '${account.name}-${account.location}'
//@[008:00049) |   | |   |   |   | ├─ObjectPropertySyntax
//@[008:00010) |   | |   |   |   | | ├─IdentifierSyntax
//@[008:00010) |   | |   |   |   | | | └─Token(Identifier) |id|
//@[010:00011) |   | |   |   |   | | ├─Token(Colon) |:|
//@[012:00049) |   | |   |   |   | | └─StringSyntax
//@[012:00015) |   | |   |   |   | |   ├─Token(StringLeftPiece) |'${|
//@[015:00027) |   | |   |   |   | |   ├─PropertyAccessSyntax
//@[015:00022) |   | |   |   |   | |   | ├─VariableAccessSyntax
//@[015:00022) |   | |   |   |   | |   | | └─IdentifierSyntax
//@[015:00022) |   | |   |   |   | |   | |   └─Token(Identifier) |account|
//@[022:00023) |   | |   |   |   | |   | ├─Token(Dot) |.|
//@[023:00027) |   | |   |   |   | |   | └─IdentifierSyntax
//@[023:00027) |   | |   |   |   | |   |   └─Token(Identifier) |name|
//@[027:00031) |   | |   |   |   | |   ├─Token(StringMiddlePiece) |}-${|
//@[031:00047) |   | |   |   |   | |   ├─PropertyAccessSyntax
//@[031:00038) |   | |   |   |   | |   | ├─VariableAccessSyntax
//@[031:00038) |   | |   |   |   | |   | | └─IdentifierSyntax
//@[031:00038) |   | |   |   |   | |   | |   └─Token(Identifier) |account|
//@[038:00039) |   | |   |   |   | |   | ├─Token(Dot) |.|
//@[039:00047) |   | |   |   |   | |   | └─IdentifierSyntax
//@[039:00047) |   | |   |   |   | |   |   └─Token(Identifier) |location|
//@[047:00049) |   | |   |   |   | |   └─Token(StringRightPiece) |}'|
//@[049:00051) |   | |   |   |   | ├─Token(NewLine) |\r\n|
        state: [for lol in []: 4]
//@[008:00033) |   | |   |   |   | ├─ObjectPropertySyntax
//@[008:00013) |   | |   |   |   | | ├─IdentifierSyntax
//@[008:00013) |   | |   |   |   | | | └─Token(Identifier) |state|
//@[013:00014) |   | |   |   |   | | ├─Token(Colon) |:|
//@[015:00033) |   | |   |   |   | | └─ForSyntax
//@[015:00016) |   | |   |   |   | |   ├─Token(LeftSquare) |[|
//@[016:00019) |   | |   |   |   | |   ├─Token(Identifier) |for|
//@[020:00023) |   | |   |   |   | |   ├─LocalVariableSyntax
//@[020:00023) |   | |   |   |   | |   | └─IdentifierSyntax
//@[020:00023) |   | |   |   |   | |   |   └─Token(Identifier) |lol|
//@[024:00026) |   | |   |   |   | |   ├─Token(Identifier) |in|
//@[027:00029) |   | |   |   |   | |   ├─ArraySyntax
//@[027:00028) |   | |   |   |   | |   | ├─Token(LeftSquare) |[|
//@[028:00029) |   | |   |   |   | |   | └─Token(RightSquare) |]|
//@[029:00030) |   | |   |   |   | |   ├─Token(Colon) |:|
//@[031:00032) |   | |   |   |   | |   ├─IntegerLiteralSyntax
//@[031:00032) |   | |   |   |   | |   | └─Token(Integer) |4|
//@[032:00033) |   | |   |   |   | |   └─Token(RightSquare) |]|
//@[033:00035) |   | |   |   |   | ├─Token(NewLine) |\r\n|
      }]
//@[006:00007) |   | |   |   |   | └─Token(RightBrace) |}|
//@[007:00008) |   | |   |   |   └─Token(RightSquare) |]|
//@[008:00010) |   | |   |   ├─Token(NewLine) |\r\n|
    }
//@[004:00005) |   | |   |   └─Token(RightBrace) |}|
//@[005:00007) |   | |   ├─Token(NewLine) |\r\n|
  }
//@[002:00003) |   | |   └─Token(RightBrace) |}|
//@[003:00005) |   | ├─Token(NewLine) |\r\n|
}]
//@[000:00001) |   | └─Token(RightBrace) |}|
//@[001:00002) |   └─Token(RightSquare) |]|
//@[002:00004) ├─Token(NewLine) |\r\n|
resource propertyLoopsCannotNest2 'Microsoft.Storage/storageAccounts@2019-06-01' = [for (account,i) in storageAccounts: {
//@[000:00441) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00033) | ├─IdentifierSyntax
//@[009:00033) | | └─Token(Identifier) |propertyLoopsCannotNest2|
//@[034:00080) | ├─StringSyntax
//@[034:00080) | | └─Token(StringComplete) |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[081:00082) | ├─Token(Assignment) |=|
//@[083:00441) | └─ForSyntax
//@[083:00084) |   ├─Token(LeftSquare) |[|
//@[084:00087) |   ├─Token(Identifier) |for|
//@[088:00099) |   ├─VariableBlockSyntax
//@[088:00089) |   | ├─Token(LeftParen) |(|
//@[089:00096) |   | ├─LocalVariableSyntax
//@[089:00096) |   | | └─IdentifierSyntax
//@[089:00096) |   | |   └─Token(Identifier) |account|
//@[096:00097) |   | ├─Token(Comma) |,|
//@[097:00098) |   | ├─LocalVariableSyntax
//@[097:00098) |   | | └─IdentifierSyntax
//@[097:00098) |   | |   └─Token(Identifier) |i|
//@[098:00099) |   | └─Token(RightParen) |)|
//@[100:00102) |   ├─Token(Identifier) |in|
//@[103:00118) |   ├─VariableAccessSyntax
//@[103:00118) |   | └─IdentifierSyntax
//@[103:00118) |   |   └─Token(Identifier) |storageAccounts|
//@[118:00119) |   ├─Token(Colon) |:|
//@[120:00440) |   ├─ObjectSyntax
//@[120:00121) |   | ├─Token(LeftBrace) |{|
//@[121:00123) |   | ├─Token(NewLine) |\r\n|
  name: account.name
//@[002:00020) |   | ├─ObjectPropertySyntax
//@[002:00006) |   | | ├─IdentifierSyntax
//@[002:00006) |   | | | └─Token(Identifier) |name|
//@[006:00007) |   | | ├─Token(Colon) |:|
//@[008:00020) |   | | └─PropertyAccessSyntax
//@[008:00015) |   | |   ├─VariableAccessSyntax
//@[008:00015) |   | |   | └─IdentifierSyntax
//@[008:00015) |   | |   |   └─Token(Identifier) |account|
//@[015:00016) |   | |   ├─Token(Dot) |.|
//@[016:00020) |   | |   └─IdentifierSyntax
//@[016:00020) |   | |     └─Token(Identifier) |name|
//@[020:00022) |   | ├─Token(NewLine) |\r\n|
  location: account.location
//@[002:00028) |   | ├─ObjectPropertySyntax
//@[002:00010) |   | | ├─IdentifierSyntax
//@[002:00010) |   | | | └─Token(Identifier) |location|
//@[010:00011) |   | | ├─Token(Colon) |:|
//@[012:00028) |   | | └─PropertyAccessSyntax
//@[012:00019) |   | |   ├─VariableAccessSyntax
//@[012:00019) |   | |   | └─IdentifierSyntax
//@[012:00019) |   | |   |   └─Token(Identifier) |account|
//@[019:00020) |   | |   ├─Token(Dot) |.|
//@[020:00028) |   | |   └─IdentifierSyntax
//@[020:00028) |   | |     └─Token(Identifier) |location|
//@[028:00030) |   | ├─Token(NewLine) |\r\n|
  sku: {
//@[002:00039) |   | ├─ObjectPropertySyntax
//@[002:00005) |   | | ├─IdentifierSyntax
//@[002:00005) |   | | | └─Token(Identifier) |sku|
//@[005:00006) |   | | ├─Token(Colon) |:|
//@[007:00039) |   | | └─ObjectSyntax
//@[007:00008) |   | |   ├─Token(LeftBrace) |{|
//@[008:00010) |   | |   ├─Token(NewLine) |\r\n|
    name: 'Standard_LRS'
//@[004:00024) |   | |   ├─ObjectPropertySyntax
//@[004:00008) |   | |   | ├─IdentifierSyntax
//@[004:00008) |   | |   | | └─Token(Identifier) |name|
//@[008:00009) |   | |   | ├─Token(Colon) |:|
//@[010:00024) |   | |   | └─StringSyntax
//@[010:00024) |   | |   |   └─Token(StringComplete) |'Standard_LRS'|
//@[024:00026) |   | |   ├─Token(NewLine) |\r\n|
  }
//@[002:00003) |   | |   └─Token(RightBrace) |}|
//@[003:00005) |   | ├─Token(NewLine) |\r\n|
  kind: 'StorageV2'
//@[002:00019) |   | ├─ObjectPropertySyntax
//@[002:00006) |   | | ├─IdentifierSyntax
//@[002:00006) |   | | | └─Token(Identifier) |kind|
//@[006:00007) |   | | ├─Token(Colon) |:|
//@[008:00019) |   | | └─StringSyntax
//@[008:00019) |   | |   └─Token(StringComplete) |'StorageV2'|
//@[019:00021) |   | ├─Token(NewLine) |\r\n|
  properties: {
//@[002:00200) |   | ├─ObjectPropertySyntax
//@[002:00012) |   | | ├─IdentifierSyntax
//@[002:00012) |   | | | └─Token(Identifier) |properties|
//@[012:00013) |   | | ├─Token(Colon) |:|
//@[014:00200) |   | | └─ObjectSyntax
//@[014:00015) |   | |   ├─Token(LeftBrace) |{|
//@[015:00019) |   | |   ├─Token(NewLine) |\r\n\r\n|

    networkAcls: {
//@[004:00176) |   | |   ├─ObjectPropertySyntax
//@[004:00015) |   | |   | ├─IdentifierSyntax
//@[004:00015) |   | |   | | └─Token(Identifier) |networkAcls|
//@[015:00016) |   | |   | ├─Token(Colon) |:|
//@[017:00176) |   | |   | └─ObjectSyntax
//@[017:00018) |   | |   |   ├─Token(LeftBrace) |{|
//@[018:00020) |   | |   |   ├─Token(NewLine) |\r\n|
      virtualNetworkRules: [for (rule,j) in []: {
//@[006:00149) |   | |   |   ├─ObjectPropertySyntax
//@[006:00025) |   | |   |   | ├─IdentifierSyntax
//@[006:00025) |   | |   |   | | └─Token(Identifier) |virtualNetworkRules|
//@[025:00026) |   | |   |   | ├─Token(Colon) |:|
//@[027:00149) |   | |   |   | └─ForSyntax
//@[027:00028) |   | |   |   |   ├─Token(LeftSquare) |[|
//@[028:00031) |   | |   |   |   ├─Token(Identifier) |for|
//@[032:00040) |   | |   |   |   ├─VariableBlockSyntax
//@[032:00033) |   | |   |   |   | ├─Token(LeftParen) |(|
//@[033:00037) |   | |   |   |   | ├─LocalVariableSyntax
//@[033:00037) |   | |   |   |   | | └─IdentifierSyntax
//@[033:00037) |   | |   |   |   | |   └─Token(Identifier) |rule|
//@[037:00038) |   | |   |   |   | ├─Token(Comma) |,|
//@[038:00039) |   | |   |   |   | ├─LocalVariableSyntax
//@[038:00039) |   | |   |   |   | | └─IdentifierSyntax
//@[038:00039) |   | |   |   |   | |   └─Token(Identifier) |j|
//@[039:00040) |   | |   |   |   | └─Token(RightParen) |)|
//@[041:00043) |   | |   |   |   ├─Token(Identifier) |in|
//@[044:00046) |   | |   |   |   ├─ArraySyntax
//@[044:00045) |   | |   |   |   | ├─Token(LeftSquare) |[|
//@[045:00046) |   | |   |   |   | └─Token(RightSquare) |]|
//@[046:00047) |   | |   |   |   ├─Token(Colon) |:|
//@[048:00148) |   | |   |   |   ├─ObjectSyntax
//@[048:00049) |   | |   |   |   | ├─Token(LeftBrace) |{|
//@[049:00051) |   | |   |   |   | ├─Token(NewLine) |\r\n|
        id: '${account.name}-${account.location}'
//@[008:00049) |   | |   |   |   | ├─ObjectPropertySyntax
//@[008:00010) |   | |   |   |   | | ├─IdentifierSyntax
//@[008:00010) |   | |   |   |   | | | └─Token(Identifier) |id|
//@[010:00011) |   | |   |   |   | | ├─Token(Colon) |:|
//@[012:00049) |   | |   |   |   | | └─StringSyntax
//@[012:00015) |   | |   |   |   | |   ├─Token(StringLeftPiece) |'${|
//@[015:00027) |   | |   |   |   | |   ├─PropertyAccessSyntax
//@[015:00022) |   | |   |   |   | |   | ├─VariableAccessSyntax
//@[015:00022) |   | |   |   |   | |   | | └─IdentifierSyntax
//@[015:00022) |   | |   |   |   | |   | |   └─Token(Identifier) |account|
//@[022:00023) |   | |   |   |   | |   | ├─Token(Dot) |.|
//@[023:00027) |   | |   |   |   | |   | └─IdentifierSyntax
//@[023:00027) |   | |   |   |   | |   |   └─Token(Identifier) |name|
//@[027:00031) |   | |   |   |   | |   ├─Token(StringMiddlePiece) |}-${|
//@[031:00047) |   | |   |   |   | |   ├─PropertyAccessSyntax
//@[031:00038) |   | |   |   |   | |   | ├─VariableAccessSyntax
//@[031:00038) |   | |   |   |   | |   | | └─IdentifierSyntax
//@[031:00038) |   | |   |   |   | |   | |   └─Token(Identifier) |account|
//@[038:00039) |   | |   |   |   | |   | ├─Token(Dot) |.|
//@[039:00047) |   | |   |   |   | |   | └─IdentifierSyntax
//@[039:00047) |   | |   |   |   | |   |   └─Token(Identifier) |location|
//@[047:00049) |   | |   |   |   | |   └─Token(StringRightPiece) |}'|
//@[049:00051) |   | |   |   |   | ├─Token(NewLine) |\r\n|
        state: [for (lol,k) in []: 4]
//@[008:00037) |   | |   |   |   | ├─ObjectPropertySyntax
//@[008:00013) |   | |   |   |   | | ├─IdentifierSyntax
//@[008:00013) |   | |   |   |   | | | └─Token(Identifier) |state|
//@[013:00014) |   | |   |   |   | | ├─Token(Colon) |:|
//@[015:00037) |   | |   |   |   | | └─ForSyntax
//@[015:00016) |   | |   |   |   | |   ├─Token(LeftSquare) |[|
//@[016:00019) |   | |   |   |   | |   ├─Token(Identifier) |for|
//@[020:00027) |   | |   |   |   | |   ├─VariableBlockSyntax
//@[020:00021) |   | |   |   |   | |   | ├─Token(LeftParen) |(|
//@[021:00024) |   | |   |   |   | |   | ├─LocalVariableSyntax
//@[021:00024) |   | |   |   |   | |   | | └─IdentifierSyntax
//@[021:00024) |   | |   |   |   | |   | |   └─Token(Identifier) |lol|
//@[024:00025) |   | |   |   |   | |   | ├─Token(Comma) |,|
//@[025:00026) |   | |   |   |   | |   | ├─LocalVariableSyntax
//@[025:00026) |   | |   |   |   | |   | | └─IdentifierSyntax
//@[025:00026) |   | |   |   |   | |   | |   └─Token(Identifier) |k|
//@[026:00027) |   | |   |   |   | |   | └─Token(RightParen) |)|
//@[028:00030) |   | |   |   |   | |   ├─Token(Identifier) |in|
//@[031:00033) |   | |   |   |   | |   ├─ArraySyntax
//@[031:00032) |   | |   |   |   | |   | ├─Token(LeftSquare) |[|
//@[032:00033) |   | |   |   |   | |   | └─Token(RightSquare) |]|
//@[033:00034) |   | |   |   |   | |   ├─Token(Colon) |:|
//@[035:00036) |   | |   |   |   | |   ├─IntegerLiteralSyntax
//@[035:00036) |   | |   |   |   | |   | └─Token(Integer) |4|
//@[036:00037) |   | |   |   |   | |   └─Token(RightSquare) |]|
//@[037:00039) |   | |   |   |   | ├─Token(NewLine) |\r\n|
      }]
//@[006:00007) |   | |   |   |   | └─Token(RightBrace) |}|
//@[007:00008) |   | |   |   |   └─Token(RightSquare) |]|
//@[008:00010) |   | |   |   ├─Token(NewLine) |\r\n|
    }
//@[004:00005) |   | |   |   └─Token(RightBrace) |}|
//@[005:00007) |   | |   ├─Token(NewLine) |\r\n|
  }
//@[002:00003) |   | |   └─Token(RightBrace) |}|
//@[003:00005) |   | ├─Token(NewLine) |\r\n|
}]
//@[000:00001) |   | └─Token(RightBrace) |}|
//@[001:00002) |   └─Token(RightSquare) |]|
//@[002:00006) ├─Token(NewLine) |\r\n\r\n|

// property loops cannot be nested (even more nesting)
//@[054:00056) ├─Token(NewLine) |\r\n|
resource propertyLoopsCannotNest2 'Microsoft.Storage/storageAccounts@2019-06-01' = [for account in storageAccounts: {
//@[000:00634) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00033) | ├─IdentifierSyntax
//@[009:00033) | | └─Token(Identifier) |propertyLoopsCannotNest2|
//@[034:00080) | ├─StringSyntax
//@[034:00080) | | └─Token(StringComplete) |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[081:00082) | ├─Token(Assignment) |=|
//@[083:00634) | └─ForSyntax
//@[083:00084) |   ├─Token(LeftSquare) |[|
//@[084:00087) |   ├─Token(Identifier) |for|
//@[088:00095) |   ├─LocalVariableSyntax
//@[088:00095) |   | └─IdentifierSyntax
//@[088:00095) |   |   └─Token(Identifier) |account|
//@[096:00098) |   ├─Token(Identifier) |in|
//@[099:00114) |   ├─VariableAccessSyntax
//@[099:00114) |   | └─IdentifierSyntax
//@[099:00114) |   |   └─Token(Identifier) |storageAccounts|
//@[114:00115) |   ├─Token(Colon) |:|
//@[116:00633) |   ├─ObjectSyntax
//@[116:00117) |   | ├─Token(LeftBrace) |{|
//@[117:00119) |   | ├─Token(NewLine) |\r\n|
  name: account.name
//@[002:00020) |   | ├─ObjectPropertySyntax
//@[002:00006) |   | | ├─IdentifierSyntax
//@[002:00006) |   | | | └─Token(Identifier) |name|
//@[006:00007) |   | | ├─Token(Colon) |:|
//@[008:00020) |   | | └─PropertyAccessSyntax
//@[008:00015) |   | |   ├─VariableAccessSyntax
//@[008:00015) |   | |   | └─IdentifierSyntax
//@[008:00015) |   | |   |   └─Token(Identifier) |account|
//@[015:00016) |   | |   ├─Token(Dot) |.|
//@[016:00020) |   | |   └─IdentifierSyntax
//@[016:00020) |   | |     └─Token(Identifier) |name|
//@[020:00022) |   | ├─Token(NewLine) |\r\n|
  location: account.location
//@[002:00028) |   | ├─ObjectPropertySyntax
//@[002:00010) |   | | ├─IdentifierSyntax
//@[002:00010) |   | | | └─Token(Identifier) |location|
//@[010:00011) |   | | ├─Token(Colon) |:|
//@[012:00028) |   | | └─PropertyAccessSyntax
//@[012:00019) |   | |   ├─VariableAccessSyntax
//@[012:00019) |   | |   | └─IdentifierSyntax
//@[012:00019) |   | |   |   └─Token(Identifier) |account|
//@[019:00020) |   | |   ├─Token(Dot) |.|
//@[020:00028) |   | |   └─IdentifierSyntax
//@[020:00028) |   | |     └─Token(Identifier) |location|
//@[028:00030) |   | ├─Token(NewLine) |\r\n|
  sku: {
//@[002:00039) |   | ├─ObjectPropertySyntax
//@[002:00005) |   | | ├─IdentifierSyntax
//@[002:00005) |   | | | └─Token(Identifier) |sku|
//@[005:00006) |   | | ├─Token(Colon) |:|
//@[007:00039) |   | | └─ObjectSyntax
//@[007:00008) |   | |   ├─Token(LeftBrace) |{|
//@[008:00010) |   | |   ├─Token(NewLine) |\r\n|
    name: 'Standard_LRS'
//@[004:00024) |   | |   ├─ObjectPropertySyntax
//@[004:00008) |   | |   | ├─IdentifierSyntax
//@[004:00008) |   | |   | | └─Token(Identifier) |name|
//@[008:00009) |   | |   | ├─Token(Colon) |:|
//@[010:00024) |   | |   | └─StringSyntax
//@[010:00024) |   | |   |   └─Token(StringComplete) |'Standard_LRS'|
//@[024:00026) |   | |   ├─Token(NewLine) |\r\n|
  }
//@[002:00003) |   | |   └─Token(RightBrace) |}|
//@[003:00005) |   | ├─Token(NewLine) |\r\n|
  kind: 'StorageV2'
//@[002:00019) |   | ├─ObjectPropertySyntax
//@[002:00006) |   | | ├─IdentifierSyntax
//@[002:00006) |   | | | └─Token(Identifier) |kind|
//@[006:00007) |   | | ├─Token(Colon) |:|
//@[008:00019) |   | | └─StringSyntax
//@[008:00019) |   | |   └─Token(StringComplete) |'StorageV2'|
//@[019:00021) |   | ├─Token(NewLine) |\r\n|
  properties: {
//@[002:00397) |   | ├─ObjectPropertySyntax
//@[002:00012) |   | | ├─IdentifierSyntax
//@[002:00012) |   | | | └─Token(Identifier) |properties|
//@[012:00013) |   | | ├─Token(Colon) |:|
//@[014:00397) |   | | └─ObjectSyntax
//@[014:00015) |   | |   ├─Token(LeftBrace) |{|
//@[015:00017) |   | |   ├─Token(NewLine) |\r\n|
    networkAcls:  {
//@[004:00375) |   | |   ├─ObjectPropertySyntax
//@[004:00015) |   | |   | ├─IdentifierSyntax
//@[004:00015) |   | |   | | └─Token(Identifier) |networkAcls|
//@[015:00016) |   | |   | ├─Token(Colon) |:|
//@[018:00375) |   | |   | └─ObjectSyntax
//@[018:00019) |   | |   |   ├─Token(LeftBrace) |{|
//@[019:00021) |   | |   |   ├─Token(NewLine) |\r\n|
      virtualNetworkRules: [for rule in []: {
//@[006:00347) |   | |   |   ├─ObjectPropertySyntax
//@[006:00025) |   | |   |   | ├─IdentifierSyntax
//@[006:00025) |   | |   |   | | └─Token(Identifier) |virtualNetworkRules|
//@[025:00026) |   | |   |   | ├─Token(Colon) |:|
//@[027:00347) |   | |   |   | └─ForSyntax
//@[027:00028) |   | |   |   |   ├─Token(LeftSquare) |[|
//@[028:00031) |   | |   |   |   ├─Token(Identifier) |for|
//@[032:00036) |   | |   |   |   ├─LocalVariableSyntax
//@[032:00036) |   | |   |   |   | └─IdentifierSyntax
//@[032:00036) |   | |   |   |   |   └─Token(Identifier) |rule|
//@[037:00039) |   | |   |   |   ├─Token(Identifier) |in|
//@[040:00042) |   | |   |   |   ├─ArraySyntax
//@[040:00041) |   | |   |   |   | ├─Token(LeftSquare) |[|
//@[041:00042) |   | |   |   |   | └─Token(RightSquare) |]|
//@[042:00043) |   | |   |   |   ├─Token(Colon) |:|
//@[044:00346) |   | |   |   |   ├─ObjectSyntax
//@[044:00045) |   | |   |   |   | ├─Token(LeftBrace) |{|
//@[045:00047) |   | |   |   |   | ├─Token(NewLine) |\r\n|
        // #completionTest(15,31) -> symbolsPlusRule
//@[052:00054) |   | |   |   |   | ├─Token(NewLine) |\r\n|
        id: '${account.name}-${account.location}'
//@[008:00049) |   | |   |   |   | ├─ObjectPropertySyntax
//@[008:00010) |   | |   |   |   | | ├─IdentifierSyntax
//@[008:00010) |   | |   |   |   | | | └─Token(Identifier) |id|
//@[010:00011) |   | |   |   |   | | ├─Token(Colon) |:|
//@[012:00049) |   | |   |   |   | | └─StringSyntax
//@[012:00015) |   | |   |   |   | |   ├─Token(StringLeftPiece) |'${|
//@[015:00027) |   | |   |   |   | |   ├─PropertyAccessSyntax
//@[015:00022) |   | |   |   |   | |   | ├─VariableAccessSyntax
//@[015:00022) |   | |   |   |   | |   | | └─IdentifierSyntax
//@[015:00022) |   | |   |   |   | |   | |   └─Token(Identifier) |account|
//@[022:00023) |   | |   |   |   | |   | ├─Token(Dot) |.|
//@[023:00027) |   | |   |   |   | |   | └─IdentifierSyntax
//@[023:00027) |   | |   |   |   | |   |   └─Token(Identifier) |name|
//@[027:00031) |   | |   |   |   | |   ├─Token(StringMiddlePiece) |}-${|
//@[031:00047) |   | |   |   |   | |   ├─PropertyAccessSyntax
//@[031:00038) |   | |   |   |   | |   | ├─VariableAccessSyntax
//@[031:00038) |   | |   |   |   | |   | | └─IdentifierSyntax
//@[031:00038) |   | |   |   |   | |   | |   └─Token(Identifier) |account|
//@[038:00039) |   | |   |   |   | |   | ├─Token(Dot) |.|
//@[039:00047) |   | |   |   |   | |   | └─IdentifierSyntax
//@[039:00047) |   | |   |   |   | |   |   └─Token(Identifier) |location|
//@[047:00049) |   | |   |   |   | |   └─Token(StringRightPiece) |}'|
//@[049:00051) |   | |   |   |   | ├─Token(NewLine) |\r\n|
        state: [for state in []: {
//@[008:00185) |   | |   |   |   | ├─ObjectPropertySyntax
//@[008:00013) |   | |   |   |   | | ├─IdentifierSyntax
//@[008:00013) |   | |   |   |   | | | └─Token(Identifier) |state|
//@[013:00014) |   | |   |   |   | | ├─Token(Colon) |:|
//@[015:00185) |   | |   |   |   | | └─ForSyntax
//@[015:00016) |   | |   |   |   | |   ├─Token(LeftSquare) |[|
//@[016:00019) |   | |   |   |   | |   ├─Token(Identifier) |for|
//@[020:00025) |   | |   |   |   | |   ├─LocalVariableSyntax
//@[020:00025) |   | |   |   |   | |   | └─IdentifierSyntax
//@[020:00025) |   | |   |   |   | |   |   └─Token(Identifier) |state|
//@[026:00028) |   | |   |   |   | |   ├─Token(Identifier) |in|
//@[029:00031) |   | |   |   |   | |   ├─ArraySyntax
//@[029:00030) |   | |   |   |   | |   | ├─Token(LeftSquare) |[|
//@[030:00031) |   | |   |   |   | |   | └─Token(RightSquare) |]|
//@[031:00032) |   | |   |   |   | |   ├─Token(Colon) |:|
//@[033:00184) |   | |   |   |   | |   ├─ObjectSyntax
//@[033:00034) |   | |   |   |   | |   | ├─Token(LeftBrace) |{|
//@[034:00036) |   | |   |   |   | |   | ├─Token(NewLine) |\r\n|
          // #completionTest(38) -> empty #completionTest(16) -> symbolsPlusAccountRuleState
//@[092:00094) |   | |   |   |   | |   | ├─Token(NewLine) |\r\n|
          fake: [for something in []: true]
//@[010:00043) |   | |   |   |   | |   | ├─ObjectPropertySyntax
//@[010:00014) |   | |   |   |   | |   | | ├─IdentifierSyntax
//@[010:00014) |   | |   |   |   | |   | | | └─Token(Identifier) |fake|
//@[014:00015) |   | |   |   |   | |   | | ├─Token(Colon) |:|
//@[016:00043) |   | |   |   |   | |   | | └─ForSyntax
//@[016:00017) |   | |   |   |   | |   | |   ├─Token(LeftSquare) |[|
//@[017:00020) |   | |   |   |   | |   | |   ├─Token(Identifier) |for|
//@[021:00030) |   | |   |   |   | |   | |   ├─LocalVariableSyntax
//@[021:00030) |   | |   |   |   | |   | |   | └─IdentifierSyntax
//@[021:00030) |   | |   |   |   | |   | |   |   └─Token(Identifier) |something|
//@[031:00033) |   | |   |   |   | |   | |   ├─Token(Identifier) |in|
//@[034:00036) |   | |   |   |   | |   | |   ├─ArraySyntax
//@[034:00035) |   | |   |   |   | |   | |   | ├─Token(LeftSquare) |[|
//@[035:00036) |   | |   |   |   | |   | |   | └─Token(RightSquare) |]|
//@[036:00037) |   | |   |   |   | |   | |   ├─Token(Colon) |:|
//@[038:00042) |   | |   |   |   | |   | |   ├─BooleanLiteralSyntax
//@[038:00042) |   | |   |   |   | |   | |   | └─Token(TrueKeyword) |true|
//@[042:00043) |   | |   |   |   | |   | |   └─Token(RightSquare) |]|
//@[043:00045) |   | |   |   |   | |   | ├─Token(NewLine) |\r\n|
        }]
//@[008:00009) |   | |   |   |   | |   | └─Token(RightBrace) |}|
//@[009:00010) |   | |   |   |   | |   └─Token(RightSquare) |]|
//@[010:00012) |   | |   |   |   | ├─Token(NewLine) |\r\n|
      }]
//@[006:00007) |   | |   |   |   | └─Token(RightBrace) |}|
//@[007:00008) |   | |   |   |   └─Token(RightSquare) |]|
//@[008:00010) |   | |   |   ├─Token(NewLine) |\r\n|
    }
//@[004:00005) |   | |   |   └─Token(RightBrace) |}|
//@[005:00007) |   | |   ├─Token(NewLine) |\r\n|
  }
//@[002:00003) |   | |   └─Token(RightBrace) |}|
//@[003:00005) |   | ├─Token(NewLine) |\r\n|
}]
//@[000:00001) |   | └─Token(RightBrace) |}|
//@[001:00002) |   └─Token(RightSquare) |]|
//@[002:00006) ├─Token(NewLine) |\r\n\r\n|

// loops cannot be used inside of expressions
//@[045:00047) ├─Token(NewLine) |\r\n|
resource stuffs 'Microsoft.Storage/storageAccounts@2019-06-01' = [for account in storageAccounts: {
//@[000:00381) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00015) | ├─IdentifierSyntax
//@[009:00015) | | └─Token(Identifier) |stuffs|
//@[016:00062) | ├─StringSyntax
//@[016:00062) | | └─Token(StringComplete) |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[063:00064) | ├─Token(Assignment) |=|
//@[065:00381) | └─ForSyntax
//@[065:00066) |   ├─Token(LeftSquare) |[|
//@[066:00069) |   ├─Token(Identifier) |for|
//@[070:00077) |   ├─LocalVariableSyntax
//@[070:00077) |   | └─IdentifierSyntax
//@[070:00077) |   |   └─Token(Identifier) |account|
//@[078:00080) |   ├─Token(Identifier) |in|
//@[081:00096) |   ├─VariableAccessSyntax
//@[081:00096) |   | └─IdentifierSyntax
//@[081:00096) |   |   └─Token(Identifier) |storageAccounts|
//@[096:00097) |   ├─Token(Colon) |:|
//@[098:00380) |   ├─ObjectSyntax
//@[098:00099) |   | ├─Token(LeftBrace) |{|
//@[099:00101) |   | ├─Token(NewLine) |\r\n|
  name: account.name
//@[002:00020) |   | ├─ObjectPropertySyntax
//@[002:00006) |   | | ├─IdentifierSyntax
//@[002:00006) |   | | | └─Token(Identifier) |name|
//@[006:00007) |   | | ├─Token(Colon) |:|
//@[008:00020) |   | | └─PropertyAccessSyntax
//@[008:00015) |   | |   ├─VariableAccessSyntax
//@[008:00015) |   | |   | └─IdentifierSyntax
//@[008:00015) |   | |   |   └─Token(Identifier) |account|
//@[015:00016) |   | |   ├─Token(Dot) |.|
//@[016:00020) |   | |   └─IdentifierSyntax
//@[016:00020) |   | |     └─Token(Identifier) |name|
//@[020:00022) |   | ├─Token(NewLine) |\r\n|
  location: account.location
//@[002:00028) |   | ├─ObjectPropertySyntax
//@[002:00010) |   | | ├─IdentifierSyntax
//@[002:00010) |   | | | └─Token(Identifier) |location|
//@[010:00011) |   | | ├─Token(Colon) |:|
//@[012:00028) |   | | └─PropertyAccessSyntax
//@[012:00019) |   | |   ├─VariableAccessSyntax
//@[012:00019) |   | |   | └─IdentifierSyntax
//@[012:00019) |   | |   |   └─Token(Identifier) |account|
//@[019:00020) |   | |   ├─Token(Dot) |.|
//@[020:00028) |   | |   └─IdentifierSyntax
//@[020:00028) |   | |     └─Token(Identifier) |location|
//@[028:00030) |   | ├─Token(NewLine) |\r\n|
  sku: {
//@[002:00039) |   | ├─ObjectPropertySyntax
//@[002:00005) |   | | ├─IdentifierSyntax
//@[002:00005) |   | | | └─Token(Identifier) |sku|
//@[005:00006) |   | | ├─Token(Colon) |:|
//@[007:00039) |   | | └─ObjectSyntax
//@[007:00008) |   | |   ├─Token(LeftBrace) |{|
//@[008:00010) |   | |   ├─Token(NewLine) |\r\n|
    name: 'Standard_LRS'
//@[004:00024) |   | |   ├─ObjectPropertySyntax
//@[004:00008) |   | |   | ├─IdentifierSyntax
//@[004:00008) |   | |   | | └─Token(Identifier) |name|
//@[008:00009) |   | |   | ├─Token(Colon) |:|
//@[010:00024) |   | |   | └─StringSyntax
//@[010:00024) |   | |   |   └─Token(StringComplete) |'Standard_LRS'|
//@[024:00026) |   | |   ├─Token(NewLine) |\r\n|
  }
//@[002:00003) |   | |   └─Token(RightBrace) |}|
//@[003:00005) |   | ├─Token(NewLine) |\r\n|
  kind: 'StorageV2'
//@[002:00019) |   | ├─ObjectPropertySyntax
//@[002:00006) |   | | ├─IdentifierSyntax
//@[002:00006) |   | | | └─Token(Identifier) |kind|
//@[006:00007) |   | | ├─Token(Colon) |:|
//@[008:00019) |   | | └─StringSyntax
//@[008:00019) |   | |   └─Token(StringComplete) |'StorageV2'|
//@[019:00021) |   | ├─Token(NewLine) |\r\n|
  properties: {
//@[002:00162) |   | ├─ObjectPropertySyntax
//@[002:00012) |   | | ├─IdentifierSyntax
//@[002:00012) |   | | | └─Token(Identifier) |properties|
//@[012:00013) |   | | ├─Token(Colon) |:|
//@[014:00162) |   | | └─ObjectSyntax
//@[014:00015) |   | |   ├─Token(LeftBrace) |{|
//@[015:00017) |   | |   ├─Token(NewLine) |\r\n|
    networkAcls: {
//@[004:00140) |   | |   ├─ObjectPropertySyntax
//@[004:00015) |   | |   | ├─IdentifierSyntax
//@[004:00015) |   | |   | | └─Token(Identifier) |networkAcls|
//@[015:00016) |   | |   | ├─Token(Colon) |:|
//@[017:00140) |   | |   | └─ObjectSyntax
//@[017:00018) |   | |   |   ├─Token(LeftBrace) |{|
//@[018:00020) |   | |   |   ├─Token(NewLine) |\r\n|
      virtualNetworkRules: concat([for lol in []: {
//@[006:00113) |   | |   |   ├─ObjectPropertySyntax
//@[006:00025) |   | |   |   | ├─IdentifierSyntax
//@[006:00025) |   | |   |   | | └─Token(Identifier) |virtualNetworkRules|
//@[025:00026) |   | |   |   | ├─Token(Colon) |:|
//@[027:00113) |   | |   |   | └─FunctionCallSyntax
//@[027:00033) |   | |   |   |   ├─IdentifierSyntax
//@[027:00033) |   | |   |   |   | └─Token(Identifier) |concat|
//@[033:00034) |   | |   |   |   ├─Token(LeftParen) |(|
//@[034:00112) |   | |   |   |   ├─FunctionArgumentSyntax
//@[034:00112) |   | |   |   |   | └─ForSyntax
//@[034:00035) |   | |   |   |   |   ├─Token(LeftSquare) |[|
//@[035:00038) |   | |   |   |   |   ├─Token(Identifier) |for|
//@[039:00042) |   | |   |   |   |   ├─LocalVariableSyntax
//@[039:00042) |   | |   |   |   |   | └─IdentifierSyntax
//@[039:00042) |   | |   |   |   |   |   └─Token(Identifier) |lol|
//@[043:00045) |   | |   |   |   |   ├─Token(Identifier) |in|
//@[046:00048) |   | |   |   |   |   ├─ArraySyntax
//@[046:00047) |   | |   |   |   |   | ├─Token(LeftSquare) |[|
//@[047:00048) |   | |   |   |   |   | └─Token(RightSquare) |]|
//@[048:00049) |   | |   |   |   |   ├─Token(Colon) |:|
//@[050:00111) |   | |   |   |   |   ├─ObjectSyntax
//@[050:00051) |   | |   |   |   |   | ├─Token(LeftBrace) |{|
//@[051:00053) |   | |   |   |   |   | ├─Token(NewLine) |\r\n|
        id: '${account.name}-${account.location}'
//@[008:00049) |   | |   |   |   |   | ├─ObjectPropertySyntax
//@[008:00010) |   | |   |   |   |   | | ├─IdentifierSyntax
//@[008:00010) |   | |   |   |   |   | | | └─Token(Identifier) |id|
//@[010:00011) |   | |   |   |   |   | | ├─Token(Colon) |:|
//@[012:00049) |   | |   |   |   |   | | └─StringSyntax
//@[012:00015) |   | |   |   |   |   | |   ├─Token(StringLeftPiece) |'${|
//@[015:00027) |   | |   |   |   |   | |   ├─PropertyAccessSyntax
//@[015:00022) |   | |   |   |   |   | |   | ├─VariableAccessSyntax
//@[015:00022) |   | |   |   |   |   | |   | | └─IdentifierSyntax
//@[015:00022) |   | |   |   |   |   | |   | |   └─Token(Identifier) |account|
//@[022:00023) |   | |   |   |   |   | |   | ├─Token(Dot) |.|
//@[023:00027) |   | |   |   |   |   | |   | └─IdentifierSyntax
//@[023:00027) |   | |   |   |   |   | |   |   └─Token(Identifier) |name|
//@[027:00031) |   | |   |   |   |   | |   ├─Token(StringMiddlePiece) |}-${|
//@[031:00047) |   | |   |   |   |   | |   ├─PropertyAccessSyntax
//@[031:00038) |   | |   |   |   |   | |   | ├─VariableAccessSyntax
//@[031:00038) |   | |   |   |   |   | |   | | └─IdentifierSyntax
//@[031:00038) |   | |   |   |   |   | |   | |   └─Token(Identifier) |account|
//@[038:00039) |   | |   |   |   |   | |   | ├─Token(Dot) |.|
//@[039:00047) |   | |   |   |   |   | |   | └─IdentifierSyntax
//@[039:00047) |   | |   |   |   |   | |   |   └─Token(Identifier) |location|
//@[047:00049) |   | |   |   |   |   | |   └─Token(StringRightPiece) |}'|
//@[049:00051) |   | |   |   |   |   | ├─Token(NewLine) |\r\n|
      }])
//@[006:00007) |   | |   |   |   |   | └─Token(RightBrace) |}|
//@[007:00008) |   | |   |   |   |   └─Token(RightSquare) |]|
//@[008:00009) |   | |   |   |   └─Token(RightParen) |)|
//@[009:00011) |   | |   |   ├─Token(NewLine) |\r\n|
    }
//@[004:00005) |   | |   |   └─Token(RightBrace) |}|
//@[005:00007) |   | |   ├─Token(NewLine) |\r\n|
  }
//@[002:00003) |   | |   └─Token(RightBrace) |}|
//@[003:00005) |   | ├─Token(NewLine) |\r\n|
}]
//@[000:00001) |   | └─Token(RightBrace) |}|
//@[001:00002) |   └─Token(RightSquare) |]|
//@[002:00006) ├─Token(NewLine) |\r\n\r\n|

// using the same loop variable in a new language scope should be allowed
//@[073:00075) ├─Token(NewLine) |\r\n|
resource premiumStorages 'Microsoft.Storage/storageAccounts@2019-06-01' = [for account in storageAccounts: {
//@[000:00368) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00024) | ├─IdentifierSyntax
//@[009:00024) | | └─Token(Identifier) |premiumStorages|
//@[025:00071) | ├─StringSyntax
//@[025:00071) | | └─Token(StringComplete) |'Microsoft.Storage/storageAccounts@2019-06-01'|
//@[072:00073) | ├─Token(Assignment) |=|
//@[074:00368) | └─ForSyntax
//@[074:00075) |   ├─Token(LeftSquare) |[|
//@[075:00078) |   ├─Token(Identifier) |for|
//@[079:00086) |   ├─LocalVariableSyntax
//@[079:00086) |   | └─IdentifierSyntax
//@[079:00086) |   |   └─Token(Identifier) |account|
//@[087:00089) |   ├─Token(Identifier) |in|
//@[090:00105) |   ├─VariableAccessSyntax
//@[090:00105) |   | └─IdentifierSyntax
//@[090:00105) |   |   └─Token(Identifier) |storageAccounts|
//@[105:00106) |   ├─Token(Colon) |:|
//@[107:00367) |   ├─ObjectSyntax
//@[107:00108) |   | ├─Token(LeftBrace) |{|
//@[108:00110) |   | ├─Token(NewLine) |\r\n|
  // #completionTest(7) -> symbolsPlusAccount1
//@[046:00048) |   | ├─Token(NewLine) |\r\n|
  name: account.name
//@[002:00020) |   | ├─ObjectPropertySyntax
//@[002:00006) |   | | ├─IdentifierSyntax
//@[002:00006) |   | | | └─Token(Identifier) |name|
//@[006:00007) |   | | ├─Token(Colon) |:|
//@[008:00020) |   | | └─PropertyAccessSyntax
//@[008:00015) |   | |   ├─VariableAccessSyntax
//@[008:00015) |   | |   | └─IdentifierSyntax
//@[008:00015) |   | |   |   └─Token(Identifier) |account|
//@[015:00016) |   | |   ├─Token(Dot) |.|
//@[016:00020) |   | |   └─IdentifierSyntax
//@[016:00020) |   | |     └─Token(Identifier) |name|
//@[020:00022) |   | ├─Token(NewLine) |\r\n|
  // #completionTest(12) -> symbolsPlusAccount2
//@[047:00049) |   | ├─Token(NewLine) |\r\n|
  location: account.location
//@[002:00028) |   | ├─ObjectPropertySyntax
//@[002:00010) |   | | ├─IdentifierSyntax
//@[002:00010) |   | | | └─Token(Identifier) |location|
//@[010:00011) |   | | ├─Token(Colon) |:|
//@[012:00028) |   | | └─PropertyAccessSyntax
//@[012:00019) |   | |   ├─VariableAccessSyntax
//@[012:00019) |   | |   | └─IdentifierSyntax
//@[012:00019) |   | |   |   └─Token(Identifier) |account|
//@[019:00020) |   | |   ├─Token(Dot) |.|
//@[020:00028) |   | |   └─IdentifierSyntax
//@[020:00028) |   | |     └─Token(Identifier) |location|
//@[028:00030) |   | ├─Token(NewLine) |\r\n|
  sku: {
//@[002:00084) |   | ├─ObjectPropertySyntax
//@[002:00005) |   | | ├─IdentifierSyntax
//@[002:00005) |   | | | └─Token(Identifier) |sku|
//@[005:00006) |   | | ├─Token(Colon) |:|
//@[007:00084) |   | | └─ObjectSyntax
//@[007:00008) |   | |   ├─Token(LeftBrace) |{|
//@[008:00010) |   | |   ├─Token(NewLine) |\r\n|
    // #completionTest(9,10) -> storageSkuNamePlusSymbols
//@[057:00059) |   | |   ├─Token(NewLine) |\r\n|
    name: 
//@[004:00010) |   | |   ├─ObjectPropertySyntax
//@[004:00008) |   | |   | ├─IdentifierSyntax
//@[004:00008) |   | |   | | └─Token(Identifier) |name|
//@[008:00009) |   | |   | ├─Token(Colon) |:|
//@[010:00010) |   | |   | └─SkippedTriviaSyntax
//@[010:00012) |   | |   ├─Token(NewLine) |\r\n|
  }
//@[002:00003) |   | |   └─Token(RightBrace) |}|
//@[003:00005) |   | ├─Token(NewLine) |\r\n|
  kind: 'StorageV2'
//@[002:00019) |   | ├─ObjectPropertySyntax
//@[002:00006) |   | | ├─IdentifierSyntax
//@[002:00006) |   | | | └─Token(Identifier) |kind|
//@[006:00007) |   | | ├─Token(Colon) |:|
//@[008:00019) |   | | └─StringSyntax
//@[008:00019) |   | |   └─Token(StringComplete) |'StorageV2'|
//@[019:00021) |   | ├─Token(NewLine) |\r\n|
}]
//@[000:00001) |   | └─Token(RightBrace) |}|
//@[001:00002) |   └─Token(RightSquare) |]|
//@[002:00006) ├─Token(NewLine) |\r\n\r\n|

var directRefViaVar = premiumStorages
//@[000:00037) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00019) | ├─IdentifierSyntax
//@[004:00019) | | └─Token(Identifier) |directRefViaVar|
//@[020:00021) | ├─Token(Assignment) |=|
//@[022:00037) | └─VariableAccessSyntax
//@[022:00037) |   └─IdentifierSyntax
//@[022:00037) |     └─Token(Identifier) |premiumStorages|
//@[037:00039) ├─Token(NewLine) |\r\n|
output directRefViaOutput array = union(premiumStorages, stuffs)
//@[000:00064) ├─OutputDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |output|
//@[007:00025) | ├─IdentifierSyntax
//@[007:00025) | | └─Token(Identifier) |directRefViaOutput|
//@[026:00031) | ├─SimpleTypeSyntax
//@[026:00031) | | └─Token(Identifier) |array|
//@[032:00033) | ├─Token(Assignment) |=|
//@[034:00064) | └─FunctionCallSyntax
//@[034:00039) |   ├─IdentifierSyntax
//@[034:00039) |   | └─Token(Identifier) |union|
//@[039:00040) |   ├─Token(LeftParen) |(|
//@[040:00055) |   ├─FunctionArgumentSyntax
//@[040:00055) |   | └─VariableAccessSyntax
//@[040:00055) |   |   └─IdentifierSyntax
//@[040:00055) |   |     └─Token(Identifier) |premiumStorages|
//@[055:00056) |   ├─Token(Comma) |,|
//@[057:00063) |   ├─FunctionArgumentSyntax
//@[057:00063) |   | └─VariableAccessSyntax
//@[057:00063) |   |   └─IdentifierSyntax
//@[057:00063) |   |     └─Token(Identifier) |stuffs|
//@[063:00064) |   └─Token(RightParen) |)|
//@[064:00068) ├─Token(NewLine) |\r\n\r\n|

resource directRefViaSingleResourceBody 'Microsoft.Network/dnszones@2018-05-01' = {
//@[000:00199) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00039) | ├─IdentifierSyntax
//@[009:00039) | | └─Token(Identifier) |directRefViaSingleResourceBody|
//@[040:00079) | ├─StringSyntax
//@[040:00079) | | └─Token(StringComplete) |'Microsoft.Network/dnszones@2018-05-01'|
//@[080:00081) | ├─Token(Assignment) |=|
//@[082:00199) | └─ObjectSyntax
//@[082:00083) |   ├─Token(LeftBrace) |{|
//@[083:00085) |   ├─Token(NewLine) |\r\n|
  name: 'myZone2'
//@[002:00017) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00017) |   | └─StringSyntax
//@[008:00017) |   |   └─Token(StringComplete) |'myZone2'|
//@[017:00019) |   ├─Token(NewLine) |\r\n|
  location: 'global'
//@[002:00020) |   ├─ObjectPropertySyntax
//@[002:00010) |   | ├─IdentifierSyntax
//@[002:00010) |   | | └─Token(Identifier) |location|
//@[010:00011) |   | ├─Token(Colon) |:|
//@[012:00020) |   | └─StringSyntax
//@[012:00020) |   |   └─Token(StringComplete) |'global'|
//@[020:00022) |   ├─Token(NewLine) |\r\n|
  properties: {
//@[002:00070) |   ├─ObjectPropertySyntax
//@[002:00012) |   | ├─IdentifierSyntax
//@[002:00012) |   | | └─Token(Identifier) |properties|
//@[012:00013) |   | ├─Token(Colon) |:|
//@[014:00070) |   | └─ObjectSyntax
//@[014:00015) |   |   ├─Token(LeftBrace) |{|
//@[015:00017) |   |   ├─Token(NewLine) |\r\n|
    registrationVirtualNetworks: premiumStorages
//@[004:00048) |   |   ├─ObjectPropertySyntax
//@[004:00031) |   |   | ├─IdentifierSyntax
//@[004:00031) |   |   | | └─Token(Identifier) |registrationVirtualNetworks|
//@[031:00032) |   |   | ├─Token(Colon) |:|
//@[033:00048) |   |   | └─VariableAccessSyntax
//@[033:00048) |   |   |   └─IdentifierSyntax
//@[033:00048) |   |   |     └─Token(Identifier) |premiumStorages|
//@[048:00050) |   |   ├─Token(NewLine) |\r\n|
  }
//@[002:00003) |   |   └─Token(RightBrace) |}|
//@[003:00005) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

resource directRefViaSingleConditionalResourceBody 'Microsoft.Network/dnszones@2018-05-01' = if(true) {
//@[000:00235) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00050) | ├─IdentifierSyntax
//@[009:00050) | | └─Token(Identifier) |directRefViaSingleConditionalResourceBody|
//@[051:00090) | ├─StringSyntax
//@[051:00090) | | └─Token(StringComplete) |'Microsoft.Network/dnszones@2018-05-01'|
//@[091:00092) | ├─Token(Assignment) |=|
//@[093:00235) | └─IfConditionSyntax
//@[093:00095) |   ├─Token(Identifier) |if|
//@[095:00101) |   ├─ParenthesizedExpressionSyntax
//@[095:00096) |   | ├─Token(LeftParen) |(|
//@[096:00100) |   | ├─BooleanLiteralSyntax
//@[096:00100) |   | | └─Token(TrueKeyword) |true|
//@[100:00101) |   | └─Token(RightParen) |)|
//@[102:00235) |   └─ObjectSyntax
//@[102:00103) |     ├─Token(LeftBrace) |{|
//@[103:00105) |     ├─Token(NewLine) |\r\n|
  name: 'myZone3'
//@[002:00017) |     ├─ObjectPropertySyntax
//@[002:00006) |     | ├─IdentifierSyntax
//@[002:00006) |     | | └─Token(Identifier) |name|
//@[006:00007) |     | ├─Token(Colon) |:|
//@[008:00017) |     | └─StringSyntax
//@[008:00017) |     |   └─Token(StringComplete) |'myZone3'|
//@[017:00019) |     ├─Token(NewLine) |\r\n|
  location: 'global'
//@[002:00020) |     ├─ObjectPropertySyntax
//@[002:00010) |     | ├─IdentifierSyntax
//@[002:00010) |     | | └─Token(Identifier) |location|
//@[010:00011) |     | ├─Token(Colon) |:|
//@[012:00020) |     | └─StringSyntax
//@[012:00020) |     |   └─Token(StringComplete) |'global'|
//@[020:00022) |     ├─Token(NewLine) |\r\n|
  properties: {
//@[002:00086) |     ├─ObjectPropertySyntax
//@[002:00012) |     | ├─IdentifierSyntax
//@[002:00012) |     | | └─Token(Identifier) |properties|
//@[012:00013) |     | ├─Token(Colon) |:|
//@[014:00086) |     | └─ObjectSyntax
//@[014:00015) |     |   ├─Token(LeftBrace) |{|
//@[015:00017) |     |   ├─Token(NewLine) |\r\n|
    registrationVirtualNetworks: concat(premiumStorages, stuffs)
//@[004:00064) |     |   ├─ObjectPropertySyntax
//@[004:00031) |     |   | ├─IdentifierSyntax
//@[004:00031) |     |   | | └─Token(Identifier) |registrationVirtualNetworks|
//@[031:00032) |     |   | ├─Token(Colon) |:|
//@[033:00064) |     |   | └─FunctionCallSyntax
//@[033:00039) |     |   |   ├─IdentifierSyntax
//@[033:00039) |     |   |   | └─Token(Identifier) |concat|
//@[039:00040) |     |   |   ├─Token(LeftParen) |(|
//@[040:00055) |     |   |   ├─FunctionArgumentSyntax
//@[040:00055) |     |   |   | └─VariableAccessSyntax
//@[040:00055) |     |   |   |   └─IdentifierSyntax
//@[040:00055) |     |   |   |     └─Token(Identifier) |premiumStorages|
//@[055:00056) |     |   |   ├─Token(Comma) |,|
//@[057:00063) |     |   |   ├─FunctionArgumentSyntax
//@[057:00063) |     |   |   | └─VariableAccessSyntax
//@[057:00063) |     |   |   |   └─IdentifierSyntax
//@[057:00063) |     |   |   |     └─Token(Identifier) |stuffs|
//@[063:00064) |     |   |   └─Token(RightParen) |)|
//@[064:00066) |     |   ├─Token(NewLine) |\r\n|
  }
//@[002:00003) |     |   └─Token(RightBrace) |}|
//@[003:00005) |     ├─Token(NewLine) |\r\n|
}
//@[000:00001) |     └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

@batchSize()
//@[000:00208) ├─ResourceDeclarationSyntax
//@[000:00012) | ├─DecoratorSyntax
//@[000:00001) | | ├─Token(At) |@|
//@[001:00012) | | └─FunctionCallSyntax
//@[001:00010) | |   ├─IdentifierSyntax
//@[001:00010) | |   | └─Token(Identifier) |batchSize|
//@[010:00011) | |   ├─Token(LeftParen) |(|
//@[011:00012) | |   └─Token(RightParen) |)|
//@[012:00014) | ├─Token(NewLine) |\r\n|
resource directRefViaSingleLoopResourceBody 'Microsoft.Network/virtualNetworks@2020-06-01' = [for i in range(0, 3): {
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00043) | ├─IdentifierSyntax
//@[009:00043) | | └─Token(Identifier) |directRefViaSingleLoopResourceBody|
//@[044:00090) | ├─StringSyntax
//@[044:00090) | | └─Token(StringComplete) |'Microsoft.Network/virtualNetworks@2020-06-01'|
//@[091:00092) | ├─Token(Assignment) |=|
//@[093:00194) | └─ForSyntax
//@[093:00094) |   ├─Token(LeftSquare) |[|
//@[094:00097) |   ├─Token(Identifier) |for|
//@[098:00099) |   ├─LocalVariableSyntax
//@[098:00099) |   | └─IdentifierSyntax
//@[098:00099) |   |   └─Token(Identifier) |i|
//@[100:00102) |   ├─Token(Identifier) |in|
//@[103:00114) |   ├─FunctionCallSyntax
//@[103:00108) |   | ├─IdentifierSyntax
//@[103:00108) |   | | └─Token(Identifier) |range|
//@[108:00109) |   | ├─Token(LeftParen) |(|
//@[109:00110) |   | ├─FunctionArgumentSyntax
//@[109:00110) |   | | └─IntegerLiteralSyntax
//@[109:00110) |   | |   └─Token(Integer) |0|
//@[110:00111) |   | ├─Token(Comma) |,|
//@[112:00113) |   | ├─FunctionArgumentSyntax
//@[112:00113) |   | | └─IntegerLiteralSyntax
//@[112:00113) |   | |   └─Token(Integer) |3|
//@[113:00114) |   | └─Token(RightParen) |)|
//@[114:00115) |   ├─Token(Colon) |:|
//@[116:00193) |   ├─ObjectSyntax
//@[116:00117) |   | ├─Token(LeftBrace) |{|
//@[117:00119) |   | ├─Token(NewLine) |\r\n|
  name: 'vnet-${i}'
//@[002:00019) |   | ├─ObjectPropertySyntax
//@[002:00006) |   | | ├─IdentifierSyntax
//@[002:00006) |   | | | └─Token(Identifier) |name|
//@[006:00007) |   | | ├─Token(Colon) |:|
//@[008:00019) |   | | └─StringSyntax
//@[008:00016) |   | |   ├─Token(StringLeftPiece) |'vnet-${|
//@[016:00017) |   | |   ├─VariableAccessSyntax
//@[016:00017) |   | |   | └─IdentifierSyntax
//@[016:00017) |   | |   |   └─Token(Identifier) |i|
//@[017:00019) |   | |   └─Token(StringRightPiece) |}'|
//@[019:00021) |   | ├─Token(NewLine) |\r\n|
  properties: {
//@[002:00050) |   | ├─ObjectPropertySyntax
//@[002:00012) |   | | ├─IdentifierSyntax
//@[002:00012) |   | | | └─Token(Identifier) |properties|
//@[012:00013) |   | | ├─Token(Colon) |:|
//@[014:00050) |   | | └─ObjectSyntax
//@[014:00015) |   | |   ├─Token(LeftBrace) |{|
//@[015:00017) |   | |   ├─Token(NewLine) |\r\n|
    subnets: premiumStorages
//@[004:00028) |   | |   ├─ObjectPropertySyntax
//@[004:00011) |   | |   | ├─IdentifierSyntax
//@[004:00011) |   | |   | | └─Token(Identifier) |subnets|
//@[011:00012) |   | |   | ├─Token(Colon) |:|
//@[013:00028) |   | |   | └─VariableAccessSyntax
//@[013:00028) |   | |   |   └─IdentifierSyntax
//@[013:00028) |   | |   |     └─Token(Identifier) |premiumStorages|
//@[028:00030) |   | |   ├─Token(NewLine) |\r\n|
  }
//@[002:00003) |   | |   └─Token(RightBrace) |}|
//@[003:00005) |   | ├─Token(NewLine) |\r\n|
}]
//@[000:00001) |   | └─Token(RightBrace) |}|
//@[001:00002) |   └─Token(RightSquare) |]|
//@[002:00006) ├─Token(NewLine) |\r\n\r\n|

@batchSize(0)
//@[000:00302) ├─ResourceDeclarationSyntax
//@[000:00013) | ├─DecoratorSyntax
//@[000:00001) | | ├─Token(At) |@|
//@[001:00013) | | └─FunctionCallSyntax
//@[001:00010) | |   ├─IdentifierSyntax
//@[001:00010) | |   | └─Token(Identifier) |batchSize|
//@[010:00011) | |   ├─Token(LeftParen) |(|
//@[011:00012) | |   ├─FunctionArgumentSyntax
//@[011:00012) | |   | └─IntegerLiteralSyntax
//@[011:00012) | |   |   └─Token(Integer) |0|
//@[012:00013) | |   └─Token(RightParen) |)|
//@[013:00015) | ├─Token(NewLine) |\r\n|
resource directRefViaSingleLoopResourceBodyWithExtraDependsOn 'Microsoft.Network/virtualNetworks@2020-06-01' = [for i in range(0, 3): {
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00061) | ├─IdentifierSyntax
//@[009:00061) | | └─Token(Identifier) |directRefViaSingleLoopResourceBodyWithExtraDependsOn|
//@[062:00108) | ├─StringSyntax
//@[062:00108) | | └─Token(StringComplete) |'Microsoft.Network/virtualNetworks@2020-06-01'|
//@[109:00110) | ├─Token(Assignment) |=|
//@[111:00287) | └─ForSyntax
//@[111:00112) |   ├─Token(LeftSquare) |[|
//@[112:00115) |   ├─Token(Identifier) |for|
//@[116:00117) |   ├─LocalVariableSyntax
//@[116:00117) |   | └─IdentifierSyntax
//@[116:00117) |   |   └─Token(Identifier) |i|
//@[118:00120) |   ├─Token(Identifier) |in|
//@[121:00132) |   ├─FunctionCallSyntax
//@[121:00126) |   | ├─IdentifierSyntax
//@[121:00126) |   | | └─Token(Identifier) |range|
//@[126:00127) |   | ├─Token(LeftParen) |(|
//@[127:00128) |   | ├─FunctionArgumentSyntax
//@[127:00128) |   | | └─IntegerLiteralSyntax
//@[127:00128) |   | |   └─Token(Integer) |0|
//@[128:00129) |   | ├─Token(Comma) |,|
//@[130:00131) |   | ├─FunctionArgumentSyntax
//@[130:00131) |   | | └─IntegerLiteralSyntax
//@[130:00131) |   | |   └─Token(Integer) |3|
//@[131:00132) |   | └─Token(RightParen) |)|
//@[132:00133) |   ├─Token(Colon) |:|
//@[134:00286) |   ├─ObjectSyntax
//@[134:00135) |   | ├─Token(LeftBrace) |{|
//@[135:00137) |   | ├─Token(NewLine) |\r\n|
  name: 'vnet-${i}'
//@[002:00019) |   | ├─ObjectPropertySyntax
//@[002:00006) |   | | ├─IdentifierSyntax
//@[002:00006) |   | | | └─Token(Identifier) |name|
//@[006:00007) |   | | ├─Token(Colon) |:|
//@[008:00019) |   | | └─StringSyntax
//@[008:00016) |   | |   ├─Token(StringLeftPiece) |'vnet-${|
//@[016:00017) |   | |   ├─VariableAccessSyntax
//@[016:00017) |   | |   | └─IdentifierSyntax
//@[016:00017) |   | |   |   └─Token(Identifier) |i|
//@[017:00019) |   | |   └─Token(StringRightPiece) |}'|
//@[019:00021) |   | ├─Token(NewLine) |\r\n|
  properties: {
//@[002:00098) |   | ├─ObjectPropertySyntax
//@[002:00012) |   | | ├─IdentifierSyntax
//@[002:00012) |   | | | └─Token(Identifier) |properties|
//@[012:00013) |   | | ├─Token(Colon) |:|
//@[014:00098) |   | | └─ObjectSyntax
//@[014:00015) |   | |   ├─Token(LeftBrace) |{|
//@[015:00017) |   | |   ├─Token(NewLine) |\r\n|
    subnets: premiumStorages
//@[004:00028) |   | |   ├─ObjectPropertySyntax
//@[004:00011) |   | |   | ├─IdentifierSyntax
//@[004:00011) |   | |   | | └─Token(Identifier) |subnets|
//@[011:00012) |   | |   | ├─Token(Colon) |:|
//@[013:00028) |   | |   | └─VariableAccessSyntax
//@[013:00028) |   | |   |   └─IdentifierSyntax
//@[013:00028) |   | |   |     └─Token(Identifier) |premiumStorages|
//@[028:00030) |   | |   ├─Token(NewLine) |\r\n|
    dependsOn: [
//@[004:00046) |   | |   ├─ObjectPropertySyntax
//@[004:00013) |   | |   | ├─IdentifierSyntax
//@[004:00013) |   | |   | | └─Token(Identifier) |dependsOn|
//@[013:00014) |   | |   | ├─Token(Colon) |:|
//@[015:00046) |   | |   | └─ArraySyntax
//@[015:00016) |   | |   |   ├─Token(LeftSquare) |[|
//@[016:00018) |   | |   |   ├─Token(NewLine) |\r\n|
      premiumStorages
//@[006:00021) |   | |   |   ├─ArrayItemSyntax
//@[006:00021) |   | |   |   | └─VariableAccessSyntax
//@[006:00021) |   | |   |   |   └─IdentifierSyntax
//@[006:00021) |   | |   |   |     └─Token(Identifier) |premiumStorages|
//@[021:00023) |   | |   |   ├─Token(NewLine) |\r\n|
    ]
//@[004:00005) |   | |   |   └─Token(RightSquare) |]|
//@[005:00007) |   | |   ├─Token(NewLine) |\r\n|
  }
//@[002:00003) |   | |   └─Token(RightBrace) |}|
//@[003:00005) |   | ├─Token(NewLine) |\r\n|
  dependsOn: [
//@[002:00025) |   | ├─ObjectPropertySyntax
//@[002:00011) |   | | ├─IdentifierSyntax
//@[002:00011) |   | | | └─Token(Identifier) |dependsOn|
//@[011:00012) |   | | ├─Token(Colon) |:|
//@[013:00025) |   | | └─ArraySyntax
//@[013:00014) |   | |   ├─Token(LeftSquare) |[|
//@[014:00016) |   | |   ├─Token(NewLine) |\r\n|
    
//@[004:00006) |   | |   ├─Token(NewLine) |\r\n|
  ]
//@[002:00003) |   | |   └─Token(RightSquare) |]|
//@[003:00005) |   | ├─Token(NewLine) |\r\n|
}]
//@[000:00001) |   | └─Token(RightBrace) |}|
//@[001:00002) |   └─Token(RightSquare) |]|
//@[002:00006) ├─Token(NewLine) |\r\n\r\n|

var expressionInPropertyLoopVar = true
//@[000:00038) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00031) | ├─IdentifierSyntax
//@[004:00031) | | └─Token(Identifier) |expressionInPropertyLoopVar|
//@[032:00033) | ├─Token(Assignment) |=|
//@[034:00038) | └─BooleanLiteralSyntax
//@[034:00038) |   └─Token(TrueKeyword) |true|
//@[038:00040) ├─Token(NewLine) |\r\n|
resource expressionsInPropertyLoopName 'Microsoft.Network/dnsZones@2018-05-01' = {
//@[000:00232) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00038) | ├─IdentifierSyntax
//@[009:00038) | | └─Token(Identifier) |expressionsInPropertyLoopName|
//@[039:00078) | ├─StringSyntax
//@[039:00078) | | └─Token(StringComplete) |'Microsoft.Network/dnsZones@2018-05-01'|
//@[079:00080) | ├─Token(Assignment) |=|
//@[081:00232) | └─ObjectSyntax
//@[081:00082) |   ├─Token(LeftBrace) |{|
//@[082:00084) |   ├─Token(NewLine) |\r\n|
  name: 'hello'
//@[002:00015) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00015) |   | └─StringSyntax
//@[008:00015) |   |   └─Token(StringComplete) |'hello'|
//@[015:00017) |   ├─Token(NewLine) |\r\n|
  location: 'eastus'
//@[002:00020) |   ├─ObjectPropertySyntax
//@[002:00010) |   | ├─IdentifierSyntax
//@[002:00010) |   | | └─Token(Identifier) |location|
//@[010:00011) |   | ├─Token(Colon) |:|
//@[012:00020) |   | └─StringSyntax
//@[012:00020) |   |   └─Token(StringComplete) |'eastus'|
//@[020:00022) |   ├─Token(NewLine) |\r\n|
  properties: {
//@[002:00106) |   ├─ObjectPropertySyntax
//@[002:00012) |   | ├─IdentifierSyntax
//@[002:00012) |   | | └─Token(Identifier) |properties|
//@[012:00013) |   | ├─Token(Colon) |:|
//@[014:00106) |   | └─ObjectSyntax
//@[014:00015) |   |   ├─Token(LeftBrace) |{|
//@[015:00017) |   |   ├─Token(NewLine) |\r\n|
    'resolutionVirtualNetworks${expressionInPropertyLoopVar}': [for thing in []: {}]
//@[004:00084) |   |   ├─ObjectPropertySyntax
//@[004:00061) |   |   | ├─StringSyntax
//@[004:00032) |   |   | | ├─Token(StringLeftPiece) |'resolutionVirtualNetworks${|
//@[032:00059) |   |   | | ├─VariableAccessSyntax
//@[032:00059) |   |   | | | └─IdentifierSyntax
//@[032:00059) |   |   | | |   └─Token(Identifier) |expressionInPropertyLoopVar|
//@[059:00061) |   |   | | └─Token(StringRightPiece) |}'|
//@[061:00062) |   |   | ├─Token(Colon) |:|
//@[063:00084) |   |   | └─ForSyntax
//@[063:00064) |   |   |   ├─Token(LeftSquare) |[|
//@[064:00067) |   |   |   ├─Token(Identifier) |for|
//@[068:00073) |   |   |   ├─LocalVariableSyntax
//@[068:00073) |   |   |   | └─IdentifierSyntax
//@[068:00073) |   |   |   |   └─Token(Identifier) |thing|
//@[074:00076) |   |   |   ├─Token(Identifier) |in|
//@[077:00079) |   |   |   ├─ArraySyntax
//@[077:00078) |   |   |   | ├─Token(LeftSquare) |[|
//@[078:00079) |   |   |   | └─Token(RightSquare) |]|
//@[079:00080) |   |   |   ├─Token(Colon) |:|
//@[081:00083) |   |   |   ├─ObjectSyntax
//@[081:00082) |   |   |   | ├─Token(LeftBrace) |{|
//@[082:00083) |   |   |   | └─Token(RightBrace) |}|
//@[083:00084) |   |   |   └─Token(RightSquare) |]|
//@[084:00086) |   |   ├─Token(NewLine) |\r\n|
  }
//@[002:00003) |   |   └─Token(RightBrace) |}|
//@[003:00005) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

// resource loop body that isn't an object
//@[042:00044) ├─Token(NewLine) |\r\n|
@batchSize(-1)
//@[000:00118) ├─ResourceDeclarationSyntax
//@[000:00014) | ├─DecoratorSyntax
//@[000:00001) | | ├─Token(At) |@|
//@[001:00014) | | └─FunctionCallSyntax
//@[001:00010) | |   ├─IdentifierSyntax
//@[001:00010) | |   | └─Token(Identifier) |batchSize|
//@[010:00011) | |   ├─Token(LeftParen) |(|
//@[011:00013) | |   ├─FunctionArgumentSyntax
//@[011:00013) | |   | └─UnaryOperationSyntax
//@[011:00012) | |   |   ├─Token(Minus) |-|
//@[012:00013) | |   |   └─IntegerLiteralSyntax
//@[012:00013) | |   |     └─Token(Integer) |1|
//@[013:00014) | |   └─Token(RightParen) |)|
//@[014:00016) | ├─Token(NewLine) |\r\n|
resource nonObjectResourceLoopBody 'Microsoft.Network/dnsZones@2018-05-01' = [for thing in []: 'test']
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00034) | ├─IdentifierSyntax
//@[009:00034) | | └─Token(Identifier) |nonObjectResourceLoopBody|
//@[035:00074) | ├─StringSyntax
//@[035:00074) | | └─Token(StringComplete) |'Microsoft.Network/dnsZones@2018-05-01'|
//@[075:00076) | ├─Token(Assignment) |=|
//@[077:00102) | └─ForSyntax
//@[077:00078) |   ├─Token(LeftSquare) |[|
//@[078:00081) |   ├─Token(Identifier) |for|
//@[082:00087) |   ├─LocalVariableSyntax
//@[082:00087) |   | └─IdentifierSyntax
//@[082:00087) |   |   └─Token(Identifier) |thing|
//@[088:00090) |   ├─Token(Identifier) |in|
//@[091:00093) |   ├─ArraySyntax
//@[091:00092) |   | ├─Token(LeftSquare) |[|
//@[092:00093) |   | └─Token(RightSquare) |]|
//@[093:00094) |   ├─Token(Colon) |:|
//@[095:00101) |   ├─SkippedTriviaSyntax
//@[095:00101) |   | └─Token(StringComplete) |'test'|
//@[101:00102) |   └─Token(RightSquare) |]|
//@[102:00104) ├─Token(NewLine) |\r\n|
resource nonObjectResourceLoopBody2 'Microsoft.Network/dnsZones@2018-05-01' = [for thing in []: environment()]
//@[000:00110) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00035) | ├─IdentifierSyntax
//@[009:00035) | | └─Token(Identifier) |nonObjectResourceLoopBody2|
//@[036:00075) | ├─StringSyntax
//@[036:00075) | | └─Token(StringComplete) |'Microsoft.Network/dnsZones@2018-05-01'|
//@[076:00077) | ├─Token(Assignment) |=|
//@[078:00110) | └─ForSyntax
//@[078:00079) |   ├─Token(LeftSquare) |[|
//@[079:00082) |   ├─Token(Identifier) |for|
//@[083:00088) |   ├─LocalVariableSyntax
//@[083:00088) |   | └─IdentifierSyntax
//@[083:00088) |   |   └─Token(Identifier) |thing|
//@[089:00091) |   ├─Token(Identifier) |in|
//@[092:00094) |   ├─ArraySyntax
//@[092:00093) |   | ├─Token(LeftSquare) |[|
//@[093:00094) |   | └─Token(RightSquare) |]|
//@[094:00095) |   ├─Token(Colon) |:|
//@[096:00109) |   ├─SkippedTriviaSyntax
//@[096:00107) |   | ├─Token(Identifier) |environment|
//@[107:00108) |   | ├─Token(LeftParen) |(|
//@[108:00109) |   | └─Token(RightParen) |)|
//@[109:00110) |   └─Token(RightSquare) |]|
//@[110:00112) ├─Token(NewLine) |\r\n|
resource nonObjectResourceLoopBody3 'Microsoft.Network/dnsZones@2018-05-01' = [for (thing,i) in []: 'test']
//@[000:00107) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00035) | ├─IdentifierSyntax
//@[009:00035) | | └─Token(Identifier) |nonObjectResourceLoopBody3|
//@[036:00075) | ├─StringSyntax
//@[036:00075) | | └─Token(StringComplete) |'Microsoft.Network/dnsZones@2018-05-01'|
//@[076:00077) | ├─Token(Assignment) |=|
//@[078:00107) | └─ForSyntax
//@[078:00079) |   ├─Token(LeftSquare) |[|
//@[079:00082) |   ├─Token(Identifier) |for|
//@[083:00092) |   ├─VariableBlockSyntax
//@[083:00084) |   | ├─Token(LeftParen) |(|
//@[084:00089) |   | ├─LocalVariableSyntax
//@[084:00089) |   | | └─IdentifierSyntax
//@[084:00089) |   | |   └─Token(Identifier) |thing|
//@[089:00090) |   | ├─Token(Comma) |,|
//@[090:00091) |   | ├─LocalVariableSyntax
//@[090:00091) |   | | └─IdentifierSyntax
//@[090:00091) |   | |   └─Token(Identifier) |i|
//@[091:00092) |   | └─Token(RightParen) |)|
//@[093:00095) |   ├─Token(Identifier) |in|
//@[096:00098) |   ├─ArraySyntax
//@[096:00097) |   | ├─Token(LeftSquare) |[|
//@[097:00098) |   | └─Token(RightSquare) |]|
//@[098:00099) |   ├─Token(Colon) |:|
//@[100:00106) |   ├─SkippedTriviaSyntax
//@[100:00106) |   | └─Token(StringComplete) |'test'|
//@[106:00107) |   └─Token(RightSquare) |]|
//@[107:00109) ├─Token(NewLine) |\r\n|
resource nonObjectResourceLoopBody4 'Microsoft.Network/dnsZones@2018-05-01' = [for (thing,i) in []: environment()]
//@[000:00114) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00035) | ├─IdentifierSyntax
//@[009:00035) | | └─Token(Identifier) |nonObjectResourceLoopBody4|
//@[036:00075) | ├─StringSyntax
//@[036:00075) | | └─Token(StringComplete) |'Microsoft.Network/dnsZones@2018-05-01'|
//@[076:00077) | ├─Token(Assignment) |=|
//@[078:00114) | └─ForSyntax
//@[078:00079) |   ├─Token(LeftSquare) |[|
//@[079:00082) |   ├─Token(Identifier) |for|
//@[083:00092) |   ├─VariableBlockSyntax
//@[083:00084) |   | ├─Token(LeftParen) |(|
//@[084:00089) |   | ├─LocalVariableSyntax
//@[084:00089) |   | | └─IdentifierSyntax
//@[084:00089) |   | |   └─Token(Identifier) |thing|
//@[089:00090) |   | ├─Token(Comma) |,|
//@[090:00091) |   | ├─LocalVariableSyntax
//@[090:00091) |   | | └─IdentifierSyntax
//@[090:00091) |   | |   └─Token(Identifier) |i|
//@[091:00092) |   | └─Token(RightParen) |)|
//@[093:00095) |   ├─Token(Identifier) |in|
//@[096:00098) |   ├─ArraySyntax
//@[096:00097) |   | ├─Token(LeftSquare) |[|
//@[097:00098) |   | └─Token(RightSquare) |]|
//@[098:00099) |   ├─Token(Colon) |:|
//@[100:00113) |   ├─SkippedTriviaSyntax
//@[100:00111) |   | ├─Token(Identifier) |environment|
//@[111:00112) |   | ├─Token(LeftParen) |(|
//@[112:00113) |   | └─Token(RightParen) |)|
//@[113:00114) |   └─Token(RightSquare) |]|
//@[114:00116) ├─Token(NewLine) |\r\n|
resource nonObjectResourceLoopBody3 'Microsoft.Network/dnsZones@2018-05-01' = [for (thing,i) in []: if(true) 'test']
//@[000:00116) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00035) | ├─IdentifierSyntax
//@[009:00035) | | └─Token(Identifier) |nonObjectResourceLoopBody3|
//@[036:00075) | ├─StringSyntax
//@[036:00075) | | └─Token(StringComplete) |'Microsoft.Network/dnsZones@2018-05-01'|
//@[076:00077) | ├─Token(Assignment) |=|
//@[078:00116) | └─ForSyntax
//@[078:00079) |   ├─Token(LeftSquare) |[|
//@[079:00082) |   ├─Token(Identifier) |for|
//@[083:00092) |   ├─VariableBlockSyntax
//@[083:00084) |   | ├─Token(LeftParen) |(|
//@[084:00089) |   | ├─LocalVariableSyntax
//@[084:00089) |   | | └─IdentifierSyntax
//@[084:00089) |   | |   └─Token(Identifier) |thing|
//@[089:00090) |   | ├─Token(Comma) |,|
//@[090:00091) |   | ├─LocalVariableSyntax
//@[090:00091) |   | | └─IdentifierSyntax
//@[090:00091) |   | |   └─Token(Identifier) |i|
//@[091:00092) |   | └─Token(RightParen) |)|
//@[093:00095) |   ├─Token(Identifier) |in|
//@[096:00098) |   ├─ArraySyntax
//@[096:00097) |   | ├─Token(LeftSquare) |[|
//@[097:00098) |   | └─Token(RightSquare) |]|
//@[098:00099) |   ├─Token(Colon) |:|
//@[100:00115) |   ├─IfConditionSyntax
//@[100:00102) |   | ├─Token(Identifier) |if|
//@[102:00108) |   | ├─ParenthesizedExpressionSyntax
//@[102:00103) |   | | ├─Token(LeftParen) |(|
//@[103:00107) |   | | ├─BooleanLiteralSyntax
//@[103:00107) |   | | | └─Token(TrueKeyword) |true|
//@[107:00108) |   | | └─Token(RightParen) |)|
//@[109:00115) |   | └─SkippedTriviaSyntax
//@[109:00115) |   |   └─Token(StringComplete) |'test'|
//@[115:00116) |   └─Token(RightSquare) |]|
//@[116:00118) ├─Token(NewLine) |\r\n|
resource nonObjectResourceLoopBody4 'Microsoft.Network/dnsZones@2018-05-01' = [for (thing,i) in []: if(true) environment()]
//@[000:00123) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00035) | ├─IdentifierSyntax
//@[009:00035) | | └─Token(Identifier) |nonObjectResourceLoopBody4|
//@[036:00075) | ├─StringSyntax
//@[036:00075) | | └─Token(StringComplete) |'Microsoft.Network/dnsZones@2018-05-01'|
//@[076:00077) | ├─Token(Assignment) |=|
//@[078:00123) | └─ForSyntax
//@[078:00079) |   ├─Token(LeftSquare) |[|
//@[079:00082) |   ├─Token(Identifier) |for|
//@[083:00092) |   ├─VariableBlockSyntax
//@[083:00084) |   | ├─Token(LeftParen) |(|
//@[084:00089) |   | ├─LocalVariableSyntax
//@[084:00089) |   | | └─IdentifierSyntax
//@[084:00089) |   | |   └─Token(Identifier) |thing|
//@[089:00090) |   | ├─Token(Comma) |,|
//@[090:00091) |   | ├─LocalVariableSyntax
//@[090:00091) |   | | └─IdentifierSyntax
//@[090:00091) |   | |   └─Token(Identifier) |i|
//@[091:00092) |   | └─Token(RightParen) |)|
//@[093:00095) |   ├─Token(Identifier) |in|
//@[096:00098) |   ├─ArraySyntax
//@[096:00097) |   | ├─Token(LeftSquare) |[|
//@[097:00098) |   | └─Token(RightSquare) |]|
//@[098:00099) |   ├─Token(Colon) |:|
//@[100:00122) |   ├─IfConditionSyntax
//@[100:00102) |   | ├─Token(Identifier) |if|
//@[102:00108) |   | ├─ParenthesizedExpressionSyntax
//@[102:00103) |   | | ├─Token(LeftParen) |(|
//@[103:00107) |   | | ├─BooleanLiteralSyntax
//@[103:00107) |   | | | └─Token(TrueKeyword) |true|
//@[107:00108) |   | | └─Token(RightParen) |)|
//@[109:00122) |   | └─SkippedTriviaSyntax
//@[109:00120) |   |   ├─Token(Identifier) |environment|
//@[120:00121) |   |   ├─Token(LeftParen) |(|
//@[121:00122) |   |   └─Token(RightParen) |)|
//@[122:00123) |   └─Token(RightSquare) |]|
//@[123:00127) ├─Token(NewLine) |\r\n\r\n|

// #completionTest(54,55) -> objectPlusFor
//@[042:00044) ├─Token(NewLine) |\r\n|
resource foo 'Microsoft.Network/dnsZones@2018-05-01' = 
//@[000:00055) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00012) | ├─IdentifierSyntax
//@[009:00012) | | └─Token(Identifier) |foo|
//@[013:00052) | ├─StringSyntax
//@[013:00052) | | └─Token(StringComplete) |'Microsoft.Network/dnsZones@2018-05-01'|
//@[053:00054) | ├─Token(Assignment) |=|
//@[055:00055) | └─SkippedTriviaSyntax
//@[055:00059) ├─Token(NewLine) |\r\n\r\n|

resource foo 'Microsoft.Network/dnsZones@2018-05-01' = [for item in []: {
//@[000:00257) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00012) | ├─IdentifierSyntax
//@[009:00012) | | └─Token(Identifier) |foo|
//@[013:00052) | ├─StringSyntax
//@[013:00052) | | └─Token(StringComplete) |'Microsoft.Network/dnsZones@2018-05-01'|
//@[053:00054) | ├─Token(Assignment) |=|
//@[055:00257) | └─ForSyntax
//@[055:00056) |   ├─Token(LeftSquare) |[|
//@[056:00059) |   ├─Token(Identifier) |for|
//@[060:00064) |   ├─LocalVariableSyntax
//@[060:00064) |   | └─IdentifierSyntax
//@[060:00064) |   |   └─Token(Identifier) |item|
//@[065:00067) |   ├─Token(Identifier) |in|
//@[068:00070) |   ├─ArraySyntax
//@[068:00069) |   | ├─Token(LeftSquare) |[|
//@[069:00070) |   | └─Token(RightSquare) |]|
//@[070:00071) |   ├─Token(Colon) |:|
//@[072:00256) |   ├─ObjectSyntax
//@[072:00073) |   | ├─Token(LeftBrace) |{|
//@[073:00075) |   | ├─Token(NewLine) |\r\n|
  properties: {
//@[002:00178) |   | ├─ObjectPropertySyntax
//@[002:00012) |   | | ├─IdentifierSyntax
//@[002:00012) |   | | | └─Token(Identifier) |properties|
//@[012:00013) |   | | ├─Token(Colon) |:|
//@[014:00178) |   | | └─ObjectSyntax
//@[014:00015) |   | |   ├─Token(LeftBrace) |{|
//@[015:00017) |   | |   ├─Token(NewLine) |\r\n|
    // #completionTest(32,33) -> symbolsPlusArrayAndFor
//@[055:00057) |   | |   ├─Token(NewLine) |\r\n|
    registrationVirtualNetworks: 
//@[004:00033) |   | |   ├─ObjectPropertySyntax
//@[004:00031) |   | |   | ├─IdentifierSyntax
//@[004:00031) |   | |   | | └─Token(Identifier) |registrationVirtualNetworks|
//@[031:00032) |   | |   | ├─Token(Colon) |:|
//@[033:00033) |   | |   | └─SkippedTriviaSyntax
//@[033:00035) |   | |   ├─Token(NewLine) |\r\n|
    resolutionVirtualNetworks: [for lol in []: {
//@[004:00064) |   | |   ├─ObjectPropertySyntax
//@[004:00029) |   | |   | ├─IdentifierSyntax
//@[004:00029) |   | |   | | └─Token(Identifier) |resolutionVirtualNetworks|
//@[029:00030) |   | |   | ├─Token(Colon) |:|
//@[031:00064) |   | |   | └─ForSyntax
//@[031:00032) |   | |   |   ├─Token(LeftSquare) |[|
//@[032:00035) |   | |   |   ├─Token(Identifier) |for|
//@[036:00039) |   | |   |   ├─LocalVariableSyntax
//@[036:00039) |   | |   |   | └─IdentifierSyntax
//@[036:00039) |   | |   |   |   └─Token(Identifier) |lol|
//@[040:00042) |   | |   |   ├─Token(Identifier) |in|
//@[043:00045) |   | |   |   ├─ArraySyntax
//@[043:00044) |   | |   |   | ├─Token(LeftSquare) |[|
//@[044:00045) |   | |   |   | └─Token(RightSquare) |]|
//@[045:00046) |   | |   |   ├─Token(Colon) |:|
//@[047:00063) |   | |   |   ├─ObjectSyntax
//@[047:00048) |   | |   |   | ├─Token(LeftBrace) |{|
//@[048:00050) |   | |   |   | ├─Token(NewLine) |\r\n|
      
//@[006:00008) |   | |   |   | ├─Token(NewLine) |\r\n|
    }]
//@[004:00005) |   | |   |   | └─Token(RightBrace) |}|
//@[005:00006) |   | |   |   └─Token(RightSquare) |]|
//@[006:00008) |   | |   ├─Token(NewLine) |\r\n|
  }
//@[002:00003) |   | |   └─Token(RightBrace) |}|
//@[003:00005) |   | ├─Token(NewLine) |\r\n|
}]
//@[000:00001) |   | └─Token(RightBrace) |}|
//@[001:00002) |   └─Token(RightSquare) |]|
//@[002:00006) ├─Token(NewLine) |\r\n\r\n|

resource vnet 'Microsoft.Network/virtualNetworks@2020-06-01' = {
//@[000:00325) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00013) | ├─IdentifierSyntax
//@[009:00013) | | └─Token(Identifier) |vnet|
//@[014:00060) | ├─StringSyntax
//@[014:00060) | | └─Token(StringComplete) |'Microsoft.Network/virtualNetworks@2020-06-01'|
//@[061:00062) | ├─Token(Assignment) |=|
//@[063:00325) | └─ObjectSyntax
//@[063:00064) |   ├─Token(LeftBrace) |{|
//@[064:00066) |   ├─Token(NewLine) |\r\n|
  properties: {
//@[002:00256) |   ├─ObjectPropertySyntax
//@[002:00012) |   | ├─IdentifierSyntax
//@[002:00012) |   | | └─Token(Identifier) |properties|
//@[012:00013) |   | ├─Token(Colon) |:|
//@[014:00256) |   | └─ObjectSyntax
//@[014:00015) |   |   ├─Token(LeftBrace) |{|
//@[015:00017) |   |   ├─Token(NewLine) |\r\n|
    virtualNetworkPeerings: [for item in []: {
//@[004:00234) |   |   ├─ObjectPropertySyntax
//@[004:00026) |   |   | ├─IdentifierSyntax
//@[004:00026) |   |   | | └─Token(Identifier) |virtualNetworkPeerings|
//@[026:00027) |   |   | ├─Token(Colon) |:|
//@[028:00234) |   |   | └─ForSyntax
//@[028:00029) |   |   |   ├─Token(LeftSquare) |[|
//@[029:00032) |   |   |   ├─Token(Identifier) |for|
//@[033:00037) |   |   |   ├─LocalVariableSyntax
//@[033:00037) |   |   |   | └─IdentifierSyntax
//@[033:00037) |   |   |   |   └─Token(Identifier) |item|
//@[038:00040) |   |   |   ├─Token(Identifier) |in|
//@[041:00043) |   |   |   ├─ArraySyntax
//@[041:00042) |   |   |   | ├─Token(LeftSquare) |[|
//@[042:00043) |   |   |   | └─Token(RightSquare) |]|
//@[043:00044) |   |   |   ├─Token(Colon) |:|
//@[045:00233) |   |   |   ├─ObjectSyntax
//@[045:00046) |   |   |   | ├─Token(LeftBrace) |{|
//@[046:00048) |   |   |   | ├─Token(NewLine) |\r\n|
        properties: {
//@[008:00178) |   |   |   | ├─ObjectPropertySyntax
//@[008:00018) |   |   |   | | ├─IdentifierSyntax
//@[008:00018) |   |   |   | | | └─Token(Identifier) |properties|
//@[018:00019) |   |   |   | | ├─Token(Colon) |:|
//@[020:00178) |   |   |   | | └─ObjectSyntax
//@[020:00021) |   |   |   | |   ├─Token(LeftBrace) |{|
//@[021:00023) |   |   |   | |   ├─Token(NewLine) |\r\n|
          remoteAddressSpace: {
//@[010:00144) |   |   |   | |   ├─ObjectPropertySyntax
//@[010:00028) |   |   |   | |   | ├─IdentifierSyntax
//@[010:00028) |   |   |   | |   | | └─Token(Identifier) |remoteAddressSpace|
//@[028:00029) |   |   |   | |   | ├─Token(Colon) |:|
//@[030:00144) |   |   |   | |   | └─ObjectSyntax
//@[030:00031) |   |   |   | |   |   ├─Token(LeftBrace) |{|
//@[031:00033) |   |   |   | |   |   ├─Token(NewLine) |\r\n|
            // #completionTest(28,29) -> symbolsPlusArrayWithoutFor
//@[067:00069) |   |   |   | |   |   ├─Token(NewLine) |\r\n|
            addressPrefixes: 
//@[012:00029) |   |   |   | |   |   ├─ObjectPropertySyntax
//@[012:00027) |   |   |   | |   |   | ├─IdentifierSyntax
//@[012:00027) |   |   |   | |   |   | | └─Token(Identifier) |addressPrefixes|
//@[027:00028) |   |   |   | |   |   | ├─Token(Colon) |:|
//@[029:00029) |   |   |   | |   |   | └─SkippedTriviaSyntax
//@[029:00031) |   |   |   | |   |   ├─Token(NewLine) |\r\n|
          }
//@[010:00011) |   |   |   | |   |   └─Token(RightBrace) |}|
//@[011:00013) |   |   |   | |   ├─Token(NewLine) |\r\n|
        }
//@[008:00009) |   |   |   | |   └─Token(RightBrace) |}|
//@[009:00011) |   |   |   | ├─Token(NewLine) |\r\n|
    }]
//@[004:00005) |   |   |   | └─Token(RightBrace) |}|
//@[005:00006) |   |   |   └─Token(RightSquare) |]|
//@[006:00008) |   |   ├─Token(NewLine) |\r\n|
  }
//@[002:00003) |   |   └─Token(RightBrace) |}|
//@[003:00005) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

// parent property with 'existing' resource at different scope
//@[062:00064) ├─Token(NewLine) |\r\n|
resource p1_res1 'Microsoft.Rp1/resource1@2020-06-01' existing = {
//@[000:00110) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00016) | ├─IdentifierSyntax
//@[009:00016) | | └─Token(Identifier) |p1_res1|
//@[017:00053) | ├─StringSyntax
//@[017:00053) | | └─Token(StringComplete) |'Microsoft.Rp1/resource1@2020-06-01'|
//@[054:00062) | ├─Token(Identifier) |existing|
//@[063:00064) | ├─Token(Assignment) |=|
//@[065:00110) | └─ObjectSyntax
//@[065:00066) |   ├─Token(LeftBrace) |{|
//@[066:00068) |   ├─Token(NewLine) |\r\n|
  scope: subscription()
//@[002:00023) |   ├─ObjectPropertySyntax
//@[002:00007) |   | ├─IdentifierSyntax
//@[002:00007) |   | | └─Token(Identifier) |scope|
//@[007:00008) |   | ├─Token(Colon) |:|
//@[009:00023) |   | └─FunctionCallSyntax
//@[009:00021) |   |   ├─IdentifierSyntax
//@[009:00021) |   |   | └─Token(Identifier) |subscription|
//@[021:00022) |   |   ├─Token(LeftParen) |(|
//@[022:00023) |   |   └─Token(RightParen) |)|
//@[023:00025) |   ├─Token(NewLine) |\r\n|
  name: 'res1'
//@[002:00014) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00014) |   | └─StringSyntax
//@[008:00014) |   |   └─Token(StringComplete) |'res1'|
//@[014:00016) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

resource p1_child1 'Microsoft.Rp1/resource1/child1@2020-06-01' = {
//@[000:00106) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00018) | ├─IdentifierSyntax
//@[009:00018) | | └─Token(Identifier) |p1_child1|
//@[019:00062) | ├─StringSyntax
//@[019:00062) | | └─Token(StringComplete) |'Microsoft.Rp1/resource1/child1@2020-06-01'|
//@[063:00064) | ├─Token(Assignment) |=|
//@[065:00106) | └─ObjectSyntax
//@[065:00066) |   ├─Token(LeftBrace) |{|
//@[066:00068) |   ├─Token(NewLine) |\r\n|
  parent: p1_res1
//@[002:00017) |   ├─ObjectPropertySyntax
//@[002:00008) |   | ├─IdentifierSyntax
//@[002:00008) |   | | └─Token(Identifier) |parent|
//@[008:00009) |   | ├─Token(Colon) |:|
//@[010:00017) |   | └─VariableAccessSyntax
//@[010:00017) |   |   └─IdentifierSyntax
//@[010:00017) |   |     └─Token(Identifier) |p1_res1|
//@[017:00019) |   ├─Token(NewLine) |\r\n|
  name: 'child1'
//@[002:00016) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00016) |   | └─StringSyntax
//@[008:00016) |   |   └─Token(StringComplete) |'child1'|
//@[016:00018) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

// parent property with scope on child resource
//@[047:00049) ├─Token(NewLine) |\r\n|
resource p2_res1 'Microsoft.Rp1/resource1@2020-06-01' = {
//@[000:00076) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00016) | ├─IdentifierSyntax
//@[009:00016) | | └─Token(Identifier) |p2_res1|
//@[017:00053) | ├─StringSyntax
//@[017:00053) | | └─Token(StringComplete) |'Microsoft.Rp1/resource1@2020-06-01'|
//@[054:00055) | ├─Token(Assignment) |=|
//@[056:00076) | └─ObjectSyntax
//@[056:00057) |   ├─Token(LeftBrace) |{|
//@[057:00059) |   ├─Token(NewLine) |\r\n|
  name: 'res1'
//@[002:00014) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00014) |   | └─StringSyntax
//@[008:00014) |   |   └─Token(StringComplete) |'res1'|
//@[014:00016) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

resource p2_res2 'Microsoft.Rp2/resource2@2020-06-01' = {
//@[000:00076) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00016) | ├─IdentifierSyntax
//@[009:00016) | | └─Token(Identifier) |p2_res2|
//@[017:00053) | ├─StringSyntax
//@[017:00053) | | └─Token(StringComplete) |'Microsoft.Rp2/resource2@2020-06-01'|
//@[054:00055) | ├─Token(Assignment) |=|
//@[056:00076) | └─ObjectSyntax
//@[056:00057) |   ├─Token(LeftBrace) |{|
//@[057:00059) |   ├─Token(NewLine) |\r\n|
  name: 'res2'
//@[002:00014) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00014) |   | └─StringSyntax
//@[008:00014) |   |   └─Token(StringComplete) |'res2'|
//@[014:00016) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

resource p2_res2child 'Microsoft.Rp2/resource2/child2@2020-06-01' = {
//@[000:00127) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00021) | ├─IdentifierSyntax
//@[009:00021) | | └─Token(Identifier) |p2_res2child|
//@[022:00065) | ├─StringSyntax
//@[022:00065) | | └─Token(StringComplete) |'Microsoft.Rp2/resource2/child2@2020-06-01'|
//@[066:00067) | ├─Token(Assignment) |=|
//@[068:00127) | └─ObjectSyntax
//@[068:00069) |   ├─Token(LeftBrace) |{|
//@[069:00071) |   ├─Token(NewLine) |\r\n|
  scope: p2_res1
//@[002:00016) |   ├─ObjectPropertySyntax
//@[002:00007) |   | ├─IdentifierSyntax
//@[002:00007) |   | | └─Token(Identifier) |scope|
//@[007:00008) |   | ├─Token(Colon) |:|
//@[009:00016) |   | └─VariableAccessSyntax
//@[009:00016) |   |   └─IdentifierSyntax
//@[009:00016) |   |     └─Token(Identifier) |p2_res1|
//@[016:00018) |   ├─Token(NewLine) |\r\n|
  parent: p2_res2
//@[002:00017) |   ├─ObjectPropertySyntax
//@[002:00008) |   | ├─IdentifierSyntax
//@[002:00008) |   | | └─Token(Identifier) |parent|
//@[008:00009) |   | ├─Token(Colon) |:|
//@[010:00017) |   | └─VariableAccessSyntax
//@[010:00017) |   |   └─IdentifierSyntax
//@[010:00017) |   |     └─Token(Identifier) |p2_res2|
//@[017:00019) |   ├─Token(NewLine) |\r\n|
  name: 'child2'
//@[002:00016) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00016) |   | └─StringSyntax
//@[008:00016) |   |   └─Token(StringComplete) |'child2'|
//@[016:00018) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

// parent property self-cycle
//@[029:00031) ├─Token(NewLine) |\r\n|
resource p3_vmExt 'Microsoft.Compute/virtualMachines/extensions@2020-06-01' = {
//@[000:00124) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00017) | ├─IdentifierSyntax
//@[009:00017) | | └─Token(Identifier) |p3_vmExt|
//@[018:00075) | ├─StringSyntax
//@[018:00075) | | └─Token(StringComplete) |'Microsoft.Compute/virtualMachines/extensions@2020-06-01'|
//@[076:00077) | ├─Token(Assignment) |=|
//@[078:00124) | └─ObjectSyntax
//@[078:00079) |   ├─Token(LeftBrace) |{|
//@[079:00081) |   ├─Token(NewLine) |\r\n|
  parent: p3_vmExt
//@[002:00018) |   ├─ObjectPropertySyntax
//@[002:00008) |   | ├─IdentifierSyntax
//@[002:00008) |   | | └─Token(Identifier) |parent|
//@[008:00009) |   | ├─Token(Colon) |:|
//@[010:00018) |   | └─VariableAccessSyntax
//@[010:00018) |   |   └─IdentifierSyntax
//@[010:00018) |   |     └─Token(Identifier) |p3_vmExt|
//@[018:00020) |   ├─Token(NewLine) |\r\n|
  location: 'eastus'
//@[002:00020) |   ├─ObjectPropertySyntax
//@[002:00010) |   | ├─IdentifierSyntax
//@[002:00010) |   | | └─Token(Identifier) |location|
//@[010:00011) |   | ├─Token(Colon) |:|
//@[012:00020) |   | └─StringSyntax
//@[012:00020) |   |   └─Token(StringComplete) |'eastus'|
//@[020:00022) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

// parent property 2-cycle
//@[026:00028) ├─Token(NewLine) |\r\n|
resource p4_vm 'Microsoft.Compute/virtualMachines@2020-06-01' = {
//@[000:00110) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00014) | ├─IdentifierSyntax
//@[009:00014) | | └─Token(Identifier) |p4_vm|
//@[015:00061) | ├─StringSyntax
//@[015:00061) | | └─Token(StringComplete) |'Microsoft.Compute/virtualMachines@2020-06-01'|
//@[062:00063) | ├─Token(Assignment) |=|
//@[064:00110) | └─ObjectSyntax
//@[064:00065) |   ├─Token(LeftBrace) |{|
//@[065:00067) |   ├─Token(NewLine) |\r\n|
  parent: p4_vmExt
//@[002:00018) |   ├─ObjectPropertySyntax
//@[002:00008) |   | ├─IdentifierSyntax
//@[002:00008) |   | | └─Token(Identifier) |parent|
//@[008:00009) |   | ├─Token(Colon) |:|
//@[010:00018) |   | └─VariableAccessSyntax
//@[010:00018) |   |   └─IdentifierSyntax
//@[010:00018) |   |     └─Token(Identifier) |p4_vmExt|
//@[018:00020) |   ├─Token(NewLine) |\r\n|
  location: 'eastus'
//@[002:00020) |   ├─ObjectPropertySyntax
//@[002:00010) |   | ├─IdentifierSyntax
//@[002:00010) |   | | └─Token(Identifier) |location|
//@[010:00011) |   | ├─Token(Colon) |:|
//@[012:00020) |   | └─StringSyntax
//@[012:00020) |   |   └─Token(StringComplete) |'eastus'|
//@[020:00022) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

resource p4_vmExt 'Microsoft.Compute/virtualMachines/extensions@2020-06-01' = {
//@[000:00121) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00017) | ├─IdentifierSyntax
//@[009:00017) | | └─Token(Identifier) |p4_vmExt|
//@[018:00075) | ├─StringSyntax
//@[018:00075) | | └─Token(StringComplete) |'Microsoft.Compute/virtualMachines/extensions@2020-06-01'|
//@[076:00077) | ├─Token(Assignment) |=|
//@[078:00121) | └─ObjectSyntax
//@[078:00079) |   ├─Token(LeftBrace) |{|
//@[079:00081) |   ├─Token(NewLine) |\r\n|
  parent: p4_vm
//@[002:00015) |   ├─ObjectPropertySyntax
//@[002:00008) |   | ├─IdentifierSyntax
//@[002:00008) |   | | └─Token(Identifier) |parent|
//@[008:00009) |   | ├─Token(Colon) |:|
//@[010:00015) |   | └─VariableAccessSyntax
//@[010:00015) |   |   └─IdentifierSyntax
//@[010:00015) |   |     └─Token(Identifier) |p4_vm|
//@[015:00017) |   ├─Token(NewLine) |\r\n|
  location: 'eastus'
//@[002:00020) |   ├─ObjectPropertySyntax
//@[002:00010) |   | ├─IdentifierSyntax
//@[002:00010) |   | | └─Token(Identifier) |location|
//@[010:00011) |   | ├─Token(Colon) |:|
//@[012:00020) |   | └─StringSyntax
//@[012:00020) |   |   └─Token(StringComplete) |'eastus'|
//@[020:00022) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

// parent property with invalid child
//@[037:00039) ├─Token(NewLine) |\r\n|
resource p5_res1 'Microsoft.Rp1/resource1@2020-06-01' = {
//@[000:00076) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00016) | ├─IdentifierSyntax
//@[009:00016) | | └─Token(Identifier) |p5_res1|
//@[017:00053) | ├─StringSyntax
//@[017:00053) | | └─Token(StringComplete) |'Microsoft.Rp1/resource1@2020-06-01'|
//@[054:00055) | ├─Token(Assignment) |=|
//@[056:00076) | └─ObjectSyntax
//@[056:00057) |   ├─Token(LeftBrace) |{|
//@[057:00059) |   ├─Token(NewLine) |\r\n|
  name: 'res1'
//@[002:00014) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00014) |   | └─StringSyntax
//@[008:00014) |   |   └─Token(StringComplete) |'res1'|
//@[014:00016) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

resource p5_res2 'Microsoft.Rp2/resource2/child2@2020-06-01' = {
//@[000:00102) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00016) | ├─IdentifierSyntax
//@[009:00016) | | └─Token(Identifier) |p5_res2|
//@[017:00060) | ├─StringSyntax
//@[017:00060) | | └─Token(StringComplete) |'Microsoft.Rp2/resource2/child2@2020-06-01'|
//@[061:00062) | ├─Token(Assignment) |=|
//@[063:00102) | └─ObjectSyntax
//@[063:00064) |   ├─Token(LeftBrace) |{|
//@[064:00066) |   ├─Token(NewLine) |\r\n|
  parent: p5_res1
//@[002:00017) |   ├─ObjectPropertySyntax
//@[002:00008) |   | ├─IdentifierSyntax
//@[002:00008) |   | | └─Token(Identifier) |parent|
//@[008:00009) |   | ├─Token(Colon) |:|
//@[010:00017) |   | └─VariableAccessSyntax
//@[010:00017) |   |   └─IdentifierSyntax
//@[010:00017) |   |     └─Token(Identifier) |p5_res1|
//@[017:00019) |   ├─Token(NewLine) |\r\n|
  name: 'res2'
//@[002:00014) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00014) |   | └─StringSyntax
//@[008:00014) |   |   └─Token(StringComplete) |'res2'|
//@[014:00016) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

// parent property with invalid parent
//@[038:00040) ├─Token(NewLine) |\r\n|
resource p6_res1 '${true}' = {
//@[000:00049) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00016) | ├─IdentifierSyntax
//@[009:00016) | | └─Token(Identifier) |p6_res1|
//@[017:00026) | ├─StringSyntax
//@[017:00020) | | ├─Token(StringLeftPiece) |'${|
//@[020:00024) | | ├─BooleanLiteralSyntax
//@[020:00024) | | | └─Token(TrueKeyword) |true|
//@[024:00026) | | └─Token(StringRightPiece) |}'|
//@[027:00028) | ├─Token(Assignment) |=|
//@[029:00049) | └─ObjectSyntax
//@[029:00030) |   ├─Token(LeftBrace) |{|
//@[030:00032) |   ├─Token(NewLine) |\r\n|
  name: 'res1'
//@[002:00014) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00014) |   | └─StringSyntax
//@[008:00014) |   |   └─Token(StringComplete) |'res1'|
//@[014:00016) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

resource p6_res2 'Microsoft.Rp1/resource1/child2@2020-06-01' = {
//@[000:00102) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00016) | ├─IdentifierSyntax
//@[009:00016) | | └─Token(Identifier) |p6_res2|
//@[017:00060) | ├─StringSyntax
//@[017:00060) | | └─Token(StringComplete) |'Microsoft.Rp1/resource1/child2@2020-06-01'|
//@[061:00062) | ├─Token(Assignment) |=|
//@[063:00102) | └─ObjectSyntax
//@[063:00064) |   ├─Token(LeftBrace) |{|
//@[064:00066) |   ├─Token(NewLine) |\r\n|
  parent: p6_res1
//@[002:00017) |   ├─ObjectPropertySyntax
//@[002:00008) |   | ├─IdentifierSyntax
//@[002:00008) |   | | └─Token(Identifier) |parent|
//@[008:00009) |   | ├─Token(Colon) |:|
//@[010:00017) |   | └─VariableAccessSyntax
//@[010:00017) |   |   └─IdentifierSyntax
//@[010:00017) |   |     └─Token(Identifier) |p6_res1|
//@[017:00019) |   ├─Token(NewLine) |\r\n|
  name: 'res2'
//@[002:00014) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00014) |   | └─StringSyntax
//@[008:00014) |   |   └─Token(StringComplete) |'res2'|
//@[014:00016) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

// parent property with incorrectly-formatted name
//@[050:00052) ├─Token(NewLine) |\r\n|
resource p7_res1 'Microsoft.Rp1/resource1@2020-06-01' = {
//@[000:00076) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00016) | ├─IdentifierSyntax
//@[009:00016) | | └─Token(Identifier) |p7_res1|
//@[017:00053) | ├─StringSyntax
//@[017:00053) | | └─Token(StringComplete) |'Microsoft.Rp1/resource1@2020-06-01'|
//@[054:00055) | ├─Token(Assignment) |=|
//@[056:00076) | └─ObjectSyntax
//@[056:00057) |   ├─Token(LeftBrace) |{|
//@[057:00059) |   ├─Token(NewLine) |\r\n|
  name: 'res1'
//@[002:00014) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00014) |   | └─StringSyntax
//@[008:00014) |   |   └─Token(StringComplete) |'res1'|
//@[014:00016) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

resource p7_res2 'Microsoft.Rp1/resource1/child2@2020-06-01' = {
//@[000:00107) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00016) | ├─IdentifierSyntax
//@[009:00016) | | └─Token(Identifier) |p7_res2|
//@[017:00060) | ├─StringSyntax
//@[017:00060) | | └─Token(StringComplete) |'Microsoft.Rp1/resource1/child2@2020-06-01'|
//@[061:00062) | ├─Token(Assignment) |=|
//@[063:00107) | └─ObjectSyntax
//@[063:00064) |   ├─Token(LeftBrace) |{|
//@[064:00066) |   ├─Token(NewLine) |\r\n|
  parent: p7_res1
//@[002:00017) |   ├─ObjectPropertySyntax
//@[002:00008) |   | ├─IdentifierSyntax
//@[002:00008) |   | | └─Token(Identifier) |parent|
//@[008:00009) |   | ├─Token(Colon) |:|
//@[010:00017) |   | └─VariableAccessSyntax
//@[010:00017) |   |   └─IdentifierSyntax
//@[010:00017) |   |     └─Token(Identifier) |p7_res1|
//@[017:00019) |   ├─Token(NewLine) |\r\n|
  name: 'res1/res2'
//@[002:00019) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00019) |   | └─StringSyntax
//@[008:00019) |   |   └─Token(StringComplete) |'res1/res2'|
//@[019:00021) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

resource p7_res3 'Microsoft.Rp1/resource1/child2@2020-06-01' = {
//@[000:00118) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00016) | ├─IdentifierSyntax
//@[009:00016) | | └─Token(Identifier) |p7_res3|
//@[017:00060) | ├─StringSyntax
//@[017:00060) | | └─Token(StringComplete) |'Microsoft.Rp1/resource1/child2@2020-06-01'|
//@[061:00062) | ├─Token(Assignment) |=|
//@[063:00118) | └─ObjectSyntax
//@[063:00064) |   ├─Token(LeftBrace) |{|
//@[064:00066) |   ├─Token(NewLine) |\r\n|
  parent: p7_res1
//@[002:00017) |   ├─ObjectPropertySyntax
//@[002:00008) |   | ├─IdentifierSyntax
//@[002:00008) |   | | └─Token(Identifier) |parent|
//@[008:00009) |   | ├─Token(Colon) |:|
//@[010:00017) |   | └─VariableAccessSyntax
//@[010:00017) |   |   └─IdentifierSyntax
//@[010:00017) |   |     └─Token(Identifier) |p7_res1|
//@[017:00019) |   ├─Token(NewLine) |\r\n|
  name: '${p7_res1.name}/res2'
//@[002:00030) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00030) |   | └─StringSyntax
//@[008:00011) |   |   ├─Token(StringLeftPiece) |'${|
//@[011:00023) |   |   ├─PropertyAccessSyntax
//@[011:00018) |   |   | ├─VariableAccessSyntax
//@[011:00018) |   |   | | └─IdentifierSyntax
//@[011:00018) |   |   | |   └─Token(Identifier) |p7_res1|
//@[018:00019) |   |   | ├─Token(Dot) |.|
//@[019:00023) |   |   | └─IdentifierSyntax
//@[019:00023) |   |   |   └─Token(Identifier) |name|
//@[023:00030) |   |   └─Token(StringRightPiece) |}/res2'|
//@[030:00032) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

// top-level resource with too many '/' characters
//@[050:00052) ├─Token(NewLine) |\r\n|
resource p8_res1 'Microsoft.Rp1/resource1@2020-06-01' = {
//@[000:00081) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00016) | ├─IdentifierSyntax
//@[009:00016) | | └─Token(Identifier) |p8_res1|
//@[017:00053) | ├─StringSyntax
//@[017:00053) | | └─Token(StringComplete) |'Microsoft.Rp1/resource1@2020-06-01'|
//@[054:00055) | ├─Token(Assignment) |=|
//@[056:00081) | └─ObjectSyntax
//@[056:00057) |   ├─Token(LeftBrace) |{|
//@[057:00059) |   ├─Token(NewLine) |\r\n|
  name: 'res1/res2'
//@[002:00019) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00019) |   | └─StringSyntax
//@[008:00019) |   |   └─Token(StringComplete) |'res1/res2'|
//@[019:00021) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

resource existingResProperty 'Microsoft.Compute/virtualMachines@2020-06-01' existing = {
//@[000:00166) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00028) | ├─IdentifierSyntax
//@[009:00028) | | └─Token(Identifier) |existingResProperty|
//@[029:00075) | ├─StringSyntax
//@[029:00075) | | └─Token(StringComplete) |'Microsoft.Compute/virtualMachines@2020-06-01'|
//@[076:00084) | ├─Token(Identifier) |existing|
//@[085:00086) | ├─Token(Assignment) |=|
//@[087:00166) | └─ObjectSyntax
//@[087:00088) |   ├─Token(LeftBrace) |{|
//@[088:00090) |   ├─Token(NewLine) |\r\n|
  name: 'existingResProperty'
//@[002:00029) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00029) |   | └─StringSyntax
//@[008:00029) |   |   └─Token(StringComplete) |'existingResProperty'|
//@[029:00031) |   ├─Token(NewLine) |\r\n|
  location: 'westeurope'
//@[002:00024) |   ├─ObjectPropertySyntax
//@[002:00010) |   | ├─IdentifierSyntax
//@[002:00010) |   | | └─Token(Identifier) |location|
//@[010:00011) |   | ├─Token(Colon) |:|
//@[012:00024) |   | └─StringSyntax
//@[012:00024) |   |   └─Token(StringComplete) |'westeurope'|
//@[024:00026) |   ├─Token(NewLine) |\r\n|
  properties: {}
//@[002:00016) |   ├─ObjectPropertySyntax
//@[002:00012) |   | ├─IdentifierSyntax
//@[002:00012) |   | | └─Token(Identifier) |properties|
//@[012:00013) |   | ├─Token(Colon) |:|
//@[014:00016) |   | └─ObjectSyntax
//@[014:00015) |   |   ├─Token(LeftBrace) |{|
//@[015:00016) |   |   └─Token(RightBrace) |}|
//@[016:00018) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

resource invalidExistingLocationRef 'Microsoft.Compute/virtualMachines/extensions@2020-06-01' = {
//@[000:00196) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00035) | ├─IdentifierSyntax
//@[009:00035) | | └─Token(Identifier) |invalidExistingLocationRef|
//@[036:00093) | ├─StringSyntax
//@[036:00093) | | └─Token(StringComplete) |'Microsoft.Compute/virtualMachines/extensions@2020-06-01'|
//@[094:00095) | ├─Token(Assignment) |=|
//@[096:00196) | └─ObjectSyntax
//@[096:00097) |   ├─Token(LeftBrace) |{|
//@[097:00099) |   ├─Token(NewLine) |\r\n|
    parent: existingResProperty
//@[004:00031) |   ├─ObjectPropertySyntax
//@[004:00010) |   | ├─IdentifierSyntax
//@[004:00010) |   | | └─Token(Identifier) |parent|
//@[010:00011) |   | ├─Token(Colon) |:|
//@[012:00031) |   | └─VariableAccessSyntax
//@[012:00031) |   |   └─IdentifierSyntax
//@[012:00031) |   |     └─Token(Identifier) |existingResProperty|
//@[031:00033) |   ├─Token(NewLine) |\r\n|
    name: 'myExt'
//@[004:00017) |   ├─ObjectPropertySyntax
//@[004:00008) |   | ├─IdentifierSyntax
//@[004:00008) |   | | └─Token(Identifier) |name|
//@[008:00009) |   | ├─Token(Colon) |:|
//@[010:00017) |   | └─StringSyntax
//@[010:00017) |   |   └─Token(StringComplete) |'myExt'|
//@[017:00019) |   ├─Token(NewLine) |\r\n|
    location: existingResProperty.location
//@[004:00042) |   ├─ObjectPropertySyntax
//@[004:00012) |   | ├─IdentifierSyntax
//@[004:00012) |   | | └─Token(Identifier) |location|
//@[012:00013) |   | ├─Token(Colon) |:|
//@[014:00042) |   | └─PropertyAccessSyntax
//@[014:00033) |   |   ├─VariableAccessSyntax
//@[014:00033) |   |   | └─IdentifierSyntax
//@[014:00033) |   |   |   └─Token(Identifier) |existingResProperty|
//@[033:00034) |   |   ├─Token(Dot) |.|
//@[034:00042) |   |   └─IdentifierSyntax
//@[034:00042) |   |     └─Token(Identifier) |location|
//@[042:00044) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

resource anyTypeInDependsOn 'Microsoft.Network/dnsZones@2018-05-01' = {
//@[000:00259) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00027) | ├─IdentifierSyntax
//@[009:00027) | | └─Token(Identifier) |anyTypeInDependsOn|
//@[028:00067) | ├─StringSyntax
//@[028:00067) | | └─Token(StringComplete) |'Microsoft.Network/dnsZones@2018-05-01'|
//@[068:00069) | ├─Token(Assignment) |=|
//@[070:00259) | └─ObjectSyntax
//@[070:00071) |   ├─Token(LeftBrace) |{|
//@[071:00073) |   ├─Token(NewLine) |\r\n|
  name: 'anyTypeInDependsOn'
//@[002:00028) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00028) |   | └─StringSyntax
//@[008:00028) |   |   └─Token(StringComplete) |'anyTypeInDependsOn'|
//@[028:00030) |   ├─Token(NewLine) |\r\n|
  location: resourceGroup().location
//@[002:00036) |   ├─ObjectPropertySyntax
//@[002:00010) |   | ├─IdentifierSyntax
//@[002:00010) |   | | └─Token(Identifier) |location|
//@[010:00011) |   | ├─Token(Colon) |:|
//@[012:00036) |   | └─PropertyAccessSyntax
//@[012:00027) |   |   ├─FunctionCallSyntax
//@[012:00025) |   |   | ├─IdentifierSyntax
//@[012:00025) |   |   | | └─Token(Identifier) |resourceGroup|
//@[025:00026) |   |   | ├─Token(LeftParen) |(|
//@[026:00027) |   |   | └─Token(RightParen) |)|
//@[027:00028) |   |   ├─Token(Dot) |.|
//@[028:00036) |   |   └─IdentifierSyntax
//@[028:00036) |   |     └─Token(Identifier) |location|
//@[036:00038) |   ├─Token(NewLine) |\r\n|
  dependsOn: [
//@[002:00115) |   ├─ObjectPropertySyntax
//@[002:00011) |   | ├─IdentifierSyntax
//@[002:00011) |   | | └─Token(Identifier) |dependsOn|
//@[011:00012) |   | ├─Token(Colon) |:|
//@[013:00115) |   | └─ArraySyntax
//@[013:00014) |   |   ├─Token(LeftSquare) |[|
//@[014:00016) |   |   ├─Token(NewLine) |\r\n|
    any(invalidExistingLocationRef.properties.autoUpgradeMinorVersion)
//@[004:00070) |   |   ├─ArrayItemSyntax
//@[004:00070) |   |   | └─FunctionCallSyntax
//@[004:00007) |   |   |   ├─IdentifierSyntax
//@[004:00007) |   |   |   | └─Token(Identifier) |any|
//@[007:00008) |   |   |   ├─Token(LeftParen) |(|
//@[008:00069) |   |   |   ├─FunctionArgumentSyntax
//@[008:00069) |   |   |   | └─PropertyAccessSyntax
//@[008:00045) |   |   |   |   ├─PropertyAccessSyntax
//@[008:00034) |   |   |   |   | ├─VariableAccessSyntax
//@[008:00034) |   |   |   |   | | └─IdentifierSyntax
//@[008:00034) |   |   |   |   | |   └─Token(Identifier) |invalidExistingLocationRef|
//@[034:00035) |   |   |   |   | ├─Token(Dot) |.|
//@[035:00045) |   |   |   |   | └─IdentifierSyntax
//@[035:00045) |   |   |   |   |   └─Token(Identifier) |properties|
//@[045:00046) |   |   |   |   ├─Token(Dot) |.|
//@[046:00069) |   |   |   |   └─IdentifierSyntax
//@[046:00069) |   |   |   |     └─Token(Identifier) |autoUpgradeMinorVersion|
//@[069:00070) |   |   |   └─Token(RightParen) |)|
//@[070:00072) |   |   ├─Token(NewLine) |\r\n|
    's'
//@[004:00007) |   |   ├─ArrayItemSyntax
//@[004:00007) |   |   | └─StringSyntax
//@[004:00007) |   |   |   └─Token(StringComplete) |'s'|
//@[007:00009) |   |   ├─Token(NewLine) |\r\n|
    any(true)
//@[004:00013) |   |   ├─ArrayItemSyntax
//@[004:00013) |   |   | └─FunctionCallSyntax
//@[004:00007) |   |   |   ├─IdentifierSyntax
//@[004:00007) |   |   |   | └─Token(Identifier) |any|
//@[007:00008) |   |   |   ├─Token(LeftParen) |(|
//@[008:00012) |   |   |   ├─FunctionArgumentSyntax
//@[008:00012) |   |   |   | └─BooleanLiteralSyntax
//@[008:00012) |   |   |   |   └─Token(TrueKeyword) |true|
//@[012:00013) |   |   |   └─Token(RightParen) |)|
//@[013:00015) |   |   ├─Token(NewLine) |\r\n|
  ]
//@[002:00003) |   |   └─Token(RightSquare) |]|
//@[003:00005) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

resource anyTypeInParent 'Microsoft.Network/dnsZones/CNAME@2018-05-01' = {
//@[000:00098) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00024) | ├─IdentifierSyntax
//@[009:00024) | | └─Token(Identifier) |anyTypeInParent|
//@[025:00070) | ├─StringSyntax
//@[025:00070) | | └─Token(StringComplete) |'Microsoft.Network/dnsZones/CNAME@2018-05-01'|
//@[071:00072) | ├─Token(Assignment) |=|
//@[073:00098) | └─ObjectSyntax
//@[073:00074) |   ├─Token(LeftBrace) |{|
//@[074:00076) |   ├─Token(NewLine) |\r\n|
  parent: any(true)
//@[002:00019) |   ├─ObjectPropertySyntax
//@[002:00008) |   | ├─IdentifierSyntax
//@[002:00008) |   | | └─Token(Identifier) |parent|
//@[008:00009) |   | ├─Token(Colon) |:|
//@[010:00019) |   | └─FunctionCallSyntax
//@[010:00013) |   |   ├─IdentifierSyntax
//@[010:00013) |   |   | └─Token(Identifier) |any|
//@[013:00014) |   |   ├─Token(LeftParen) |(|
//@[014:00018) |   |   ├─FunctionArgumentSyntax
//@[014:00018) |   |   | └─BooleanLiteralSyntax
//@[014:00018) |   |   |   └─Token(TrueKeyword) |true|
//@[018:00019) |   |   └─Token(RightParen) |)|
//@[019:00021) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

resource anyTypeInParentLoop 'Microsoft.Network/dnsZones/CNAME@2018-05-01' = [for thing in []: {
//@[000:00121) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00028) | ├─IdentifierSyntax
//@[009:00028) | | └─Token(Identifier) |anyTypeInParentLoop|
//@[029:00074) | ├─StringSyntax
//@[029:00074) | | └─Token(StringComplete) |'Microsoft.Network/dnsZones/CNAME@2018-05-01'|
//@[075:00076) | ├─Token(Assignment) |=|
//@[077:00121) | └─ForSyntax
//@[077:00078) |   ├─Token(LeftSquare) |[|
//@[078:00081) |   ├─Token(Identifier) |for|
//@[082:00087) |   ├─LocalVariableSyntax
//@[082:00087) |   | └─IdentifierSyntax
//@[082:00087) |   |   └─Token(Identifier) |thing|
//@[088:00090) |   ├─Token(Identifier) |in|
//@[091:00093) |   ├─ArraySyntax
//@[091:00092) |   | ├─Token(LeftSquare) |[|
//@[092:00093) |   | └─Token(RightSquare) |]|
//@[093:00094) |   ├─Token(Colon) |:|
//@[095:00120) |   ├─ObjectSyntax
//@[095:00096) |   | ├─Token(LeftBrace) |{|
//@[096:00098) |   | ├─Token(NewLine) |\r\n|
  parent: any(true)
//@[002:00019) |   | ├─ObjectPropertySyntax
//@[002:00008) |   | | ├─IdentifierSyntax
//@[002:00008) |   | | | └─Token(Identifier) |parent|
//@[008:00009) |   | | ├─Token(Colon) |:|
//@[010:00019) |   | | └─FunctionCallSyntax
//@[010:00013) |   | |   ├─IdentifierSyntax
//@[010:00013) |   | |   | └─Token(Identifier) |any|
//@[013:00014) |   | |   ├─Token(LeftParen) |(|
//@[014:00018) |   | |   ├─FunctionArgumentSyntax
//@[014:00018) |   | |   | └─BooleanLiteralSyntax
//@[014:00018) |   | |   |   └─Token(TrueKeyword) |true|
//@[018:00019) |   | |   └─Token(RightParen) |)|
//@[019:00021) |   | ├─Token(NewLine) |\r\n|
}]
//@[000:00001) |   | └─Token(RightBrace) |}|
//@[001:00002) |   └─Token(RightSquare) |]|
//@[002:00006) ├─Token(NewLine) |\r\n\r\n|

resource anyTypeInScope 'Microsoft.Authorization/locks@2016-09-01' = {
//@[000:00115) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00023) | ├─IdentifierSyntax
//@[009:00023) | | └─Token(Identifier) |anyTypeInScope|
//@[024:00066) | ├─StringSyntax
//@[024:00066) | | └─Token(StringComplete) |'Microsoft.Authorization/locks@2016-09-01'|
//@[067:00068) | ├─Token(Assignment) |=|
//@[069:00115) | └─ObjectSyntax
//@[069:00070) |   ├─Token(LeftBrace) |{|
//@[070:00072) |   ├─Token(NewLine) |\r\n|
  scope: any(invalidExistingLocationRef)
//@[002:00040) |   ├─ObjectPropertySyntax
//@[002:00007) |   | ├─IdentifierSyntax
//@[002:00007) |   | | └─Token(Identifier) |scope|
//@[007:00008) |   | ├─Token(Colon) |:|
//@[009:00040) |   | └─FunctionCallSyntax
//@[009:00012) |   |   ├─IdentifierSyntax
//@[009:00012) |   |   | └─Token(Identifier) |any|
//@[012:00013) |   |   ├─Token(LeftParen) |(|
//@[013:00039) |   |   ├─FunctionArgumentSyntax
//@[013:00039) |   |   | └─VariableAccessSyntax
//@[013:00039) |   |   |   └─IdentifierSyntax
//@[013:00039) |   |   |     └─Token(Identifier) |invalidExistingLocationRef|
//@[039:00040) |   |   └─Token(RightParen) |)|
//@[040:00042) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

resource anyTypeInScopeConditional 'Microsoft.Authorization/locks@2016-09-01' = if(true) {
//@[000:00135) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00034) | ├─IdentifierSyntax
//@[009:00034) | | └─Token(Identifier) |anyTypeInScopeConditional|
//@[035:00077) | ├─StringSyntax
//@[035:00077) | | └─Token(StringComplete) |'Microsoft.Authorization/locks@2016-09-01'|
//@[078:00079) | ├─Token(Assignment) |=|
//@[080:00135) | └─IfConditionSyntax
//@[080:00082) |   ├─Token(Identifier) |if|
//@[082:00088) |   ├─ParenthesizedExpressionSyntax
//@[082:00083) |   | ├─Token(LeftParen) |(|
//@[083:00087) |   | ├─BooleanLiteralSyntax
//@[083:00087) |   | | └─Token(TrueKeyword) |true|
//@[087:00088) |   | └─Token(RightParen) |)|
//@[089:00135) |   └─ObjectSyntax
//@[089:00090) |     ├─Token(LeftBrace) |{|
//@[090:00092) |     ├─Token(NewLine) |\r\n|
  scope: any(invalidExistingLocationRef)
//@[002:00040) |     ├─ObjectPropertySyntax
//@[002:00007) |     | ├─IdentifierSyntax
//@[002:00007) |     | | └─Token(Identifier) |scope|
//@[007:00008) |     | ├─Token(Colon) |:|
//@[009:00040) |     | └─FunctionCallSyntax
//@[009:00012) |     |   ├─IdentifierSyntax
//@[009:00012) |     |   | └─Token(Identifier) |any|
//@[012:00013) |     |   ├─Token(LeftParen) |(|
//@[013:00039) |     |   ├─FunctionArgumentSyntax
//@[013:00039) |     |   | └─VariableAccessSyntax
//@[013:00039) |     |   |   └─IdentifierSyntax
//@[013:00039) |     |   |     └─Token(Identifier) |invalidExistingLocationRef|
//@[039:00040) |     |   └─Token(RightParen) |)|
//@[040:00042) |     ├─Token(NewLine) |\r\n|
}
//@[000:00001) |     └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

resource anyTypeInExistingScope 'Microsoft.Network/dnsZones/AAAA@2018-05-01' existing = {
//@[000:00132) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00031) | ├─IdentifierSyntax
//@[009:00031) | | └─Token(Identifier) |anyTypeInExistingScope|
//@[032:00076) | ├─StringSyntax
//@[032:00076) | | └─Token(StringComplete) |'Microsoft.Network/dnsZones/AAAA@2018-05-01'|
//@[077:00085) | ├─Token(Identifier) |existing|
//@[086:00087) | ├─Token(Assignment) |=|
//@[088:00132) | └─ObjectSyntax
//@[088:00089) |   ├─Token(LeftBrace) |{|
//@[089:00091) |   ├─Token(NewLine) |\r\n|
  parent: any('')
//@[002:00017) |   ├─ObjectPropertySyntax
//@[002:00008) |   | ├─IdentifierSyntax
//@[002:00008) |   | | └─Token(Identifier) |parent|
//@[008:00009) |   | ├─Token(Colon) |:|
//@[010:00017) |   | └─FunctionCallSyntax
//@[010:00013) |   |   ├─IdentifierSyntax
//@[010:00013) |   |   | └─Token(Identifier) |any|
//@[013:00014) |   |   ├─Token(LeftParen) |(|
//@[014:00016) |   |   ├─FunctionArgumentSyntax
//@[014:00016) |   |   | └─StringSyntax
//@[014:00016) |   |   |   └─Token(StringComplete) |''|
//@[016:00017) |   |   └─Token(RightParen) |)|
//@[017:00019) |   ├─Token(NewLine) |\r\n|
  scope: any(false)
//@[002:00019) |   ├─ObjectPropertySyntax
//@[002:00007) |   | ├─IdentifierSyntax
//@[002:00007) |   | | └─Token(Identifier) |scope|
//@[007:00008) |   | ├─Token(Colon) |:|
//@[009:00019) |   | └─FunctionCallSyntax
//@[009:00012) |   |   ├─IdentifierSyntax
//@[009:00012) |   |   | └─Token(Identifier) |any|
//@[012:00013) |   |   ├─Token(LeftParen) |(|
//@[013:00018) |   |   ├─FunctionArgumentSyntax
//@[013:00018) |   |   | └─BooleanLiteralSyntax
//@[013:00018) |   |   |   └─Token(FalseKeyword) |false|
//@[018:00019) |   |   └─Token(RightParen) |)|
//@[019:00021) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

resource anyTypeInExistingScopeLoop 'Microsoft.Network/dnsZones/AAAA@2018-05-01' existing = [for thing in []: {
//@[000:00155) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00035) | ├─IdentifierSyntax
//@[009:00035) | | └─Token(Identifier) |anyTypeInExistingScopeLoop|
//@[036:00080) | ├─StringSyntax
//@[036:00080) | | └─Token(StringComplete) |'Microsoft.Network/dnsZones/AAAA@2018-05-01'|
//@[081:00089) | ├─Token(Identifier) |existing|
//@[090:00091) | ├─Token(Assignment) |=|
//@[092:00155) | └─ForSyntax
//@[092:00093) |   ├─Token(LeftSquare) |[|
//@[093:00096) |   ├─Token(Identifier) |for|
//@[097:00102) |   ├─LocalVariableSyntax
//@[097:00102) |   | └─IdentifierSyntax
//@[097:00102) |   |   └─Token(Identifier) |thing|
//@[103:00105) |   ├─Token(Identifier) |in|
//@[106:00108) |   ├─ArraySyntax
//@[106:00107) |   | ├─Token(LeftSquare) |[|
//@[107:00108) |   | └─Token(RightSquare) |]|
//@[108:00109) |   ├─Token(Colon) |:|
//@[110:00154) |   ├─ObjectSyntax
//@[110:00111) |   | ├─Token(LeftBrace) |{|
//@[111:00113) |   | ├─Token(NewLine) |\r\n|
  parent: any('')
//@[002:00017) |   | ├─ObjectPropertySyntax
//@[002:00008) |   | | ├─IdentifierSyntax
//@[002:00008) |   | | | └─Token(Identifier) |parent|
//@[008:00009) |   | | ├─Token(Colon) |:|
//@[010:00017) |   | | └─FunctionCallSyntax
//@[010:00013) |   | |   ├─IdentifierSyntax
//@[010:00013) |   | |   | └─Token(Identifier) |any|
//@[013:00014) |   | |   ├─Token(LeftParen) |(|
//@[014:00016) |   | |   ├─FunctionArgumentSyntax
//@[014:00016) |   | |   | └─StringSyntax
//@[014:00016) |   | |   |   └─Token(StringComplete) |''|
//@[016:00017) |   | |   └─Token(RightParen) |)|
//@[017:00019) |   | ├─Token(NewLine) |\r\n|
  scope: any(false)
//@[002:00019) |   | ├─ObjectPropertySyntax
//@[002:00007) |   | | ├─IdentifierSyntax
//@[002:00007) |   | | | └─Token(Identifier) |scope|
//@[007:00008) |   | | ├─Token(Colon) |:|
//@[009:00019) |   | | └─FunctionCallSyntax
//@[009:00012) |   | |   ├─IdentifierSyntax
//@[009:00012) |   | |   | └─Token(Identifier) |any|
//@[012:00013) |   | |   ├─Token(LeftParen) |(|
//@[013:00018) |   | |   ├─FunctionArgumentSyntax
//@[013:00018) |   | |   | └─BooleanLiteralSyntax
//@[013:00018) |   | |   |   └─Token(FalseKeyword) |false|
//@[018:00019) |   | |   └─Token(RightParen) |)|
//@[019:00021) |   | ├─Token(NewLine) |\r\n|
}]
//@[000:00001) |   | └─Token(RightBrace) |}|
//@[001:00002) |   └─Token(RightSquare) |]|
//@[002:00006) ├─Token(NewLine) |\r\n\r\n|

resource tenantLevelResourceBlocked 'Microsoft.Management/managementGroups@2020-05-01' = {
//@[000:00131) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00035) | ├─IdentifierSyntax
//@[009:00035) | | └─Token(Identifier) |tenantLevelResourceBlocked|
//@[036:00086) | ├─StringSyntax
//@[036:00086) | | └─Token(StringComplete) |'Microsoft.Management/managementGroups@2020-05-01'|
//@[087:00088) | ├─Token(Assignment) |=|
//@[089:00131) | └─ObjectSyntax
//@[089:00090) |   ├─Token(LeftBrace) |{|
//@[090:00092) |   ├─Token(NewLine) |\r\n|
  name: 'tenantLevelResourceBlocked'
//@[002:00036) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00036) |   | └─StringSyntax
//@[008:00036) |   |   └─Token(StringComplete) |'tenantLevelResourceBlocked'|
//@[036:00038) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

// #completionTest(15,36,37) -> resourceTypes
//@[045:00047) ├─Token(NewLine) |\r\n|
resource comp1 'Microsoft.Resources/'
//@[000:00037) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00014) | ├─IdentifierSyntax
//@[009:00014) | | └─Token(Identifier) |comp1|
//@[015:00037) | ├─StringSyntax
//@[015:00037) | | └─Token(StringComplete) |'Microsoft.Resources/'|
//@[037:00037) | ├─SkippedTriviaSyntax
//@[037:00037) | └─SkippedTriviaSyntax
//@[037:00041) ├─Token(NewLine) |\r\n\r\n|

// #completionTest(15,16,17) -> resourceTypes
//@[045:00047) ├─Token(NewLine) |\r\n|
resource comp2 ''
//@[000:00017) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00014) | ├─IdentifierSyntax
//@[009:00014) | | └─Token(Identifier) |comp2|
//@[015:00017) | ├─StringSyntax
//@[015:00017) | | └─Token(StringComplete) |''|
//@[017:00017) | ├─SkippedTriviaSyntax
//@[017:00017) | └─SkippedTriviaSyntax
//@[017:00021) ├─Token(NewLine) |\r\n\r\n|

// #completionTest(38) -> resourceTypes
//@[039:00041) ├─Token(NewLine) |\r\n|
resource comp3 'Microsoft.Resources/t'
//@[000:00038) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00014) | ├─IdentifierSyntax
//@[009:00014) | | └─Token(Identifier) |comp3|
//@[015:00038) | ├─StringSyntax
//@[015:00038) | | └─Token(StringComplete) |'Microsoft.Resources/t'|
//@[038:00038) | ├─SkippedTriviaSyntax
//@[038:00038) | └─SkippedTriviaSyntax
//@[038:00042) ├─Token(NewLine) |\r\n\r\n|

// #completionTest(40) -> resourceTypes
//@[039:00041) ├─Token(NewLine) |\r\n|
resource comp4 'Microsoft.Resources/t/v'
//@[000:00040) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00014) | ├─IdentifierSyntax
//@[009:00014) | | └─Token(Identifier) |comp4|
//@[015:00040) | ├─StringSyntax
//@[015:00040) | | └─Token(StringComplete) |'Microsoft.Resources/t/v'|
//@[040:00040) | ├─SkippedTriviaSyntax
//@[040:00040) | └─SkippedTriviaSyntax
//@[040:00044) ├─Token(NewLine) |\r\n\r\n|

// #completionTest(49) -> resourceTypes
//@[039:00041) ├─Token(NewLine) |\r\n|
resource comp5 'Microsoft.Storage/storageAccounts'
//@[000:00050) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00014) | ├─IdentifierSyntax
//@[009:00014) | | └─Token(Identifier) |comp5|
//@[015:00050) | ├─StringSyntax
//@[015:00050) | | └─Token(StringComplete) |'Microsoft.Storage/storageAccounts'|
//@[050:00050) | ├─SkippedTriviaSyntax
//@[050:00050) | └─SkippedTriviaSyntax
//@[050:00054) ├─Token(NewLine) |\r\n\r\n|

// #completionTest(50) -> storageAccountsResourceTypes
//@[054:00056) ├─Token(NewLine) |\r\n|
resource comp6 'Microsoft.Storage/storageAccounts@'
//@[000:00051) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00014) | ├─IdentifierSyntax
//@[009:00014) | | └─Token(Identifier) |comp6|
//@[015:00051) | ├─StringSyntax
//@[015:00051) | | └─Token(StringComplete) |'Microsoft.Storage/storageAccounts@'|
//@[051:00051) | ├─SkippedTriviaSyntax
//@[051:00051) | └─SkippedTriviaSyntax
//@[051:00055) ├─Token(NewLine) |\r\n\r\n|

// #completionTest(52) -> templateSpecsResourceTypes
//@[052:00054) ├─Token(NewLine) |\r\n|
resource comp7 'Microsoft.Resources/templateSpecs@20'
//@[000:00053) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00014) | ├─IdentifierSyntax
//@[009:00014) | | └─Token(Identifier) |comp7|
//@[015:00053) | ├─StringSyntax
//@[015:00053) | | └─Token(StringComplete) |'Microsoft.Resources/templateSpecs@20'|
//@[053:00053) | ├─SkippedTriviaSyntax
//@[053:00053) | └─SkippedTriviaSyntax
//@[053:00057) ├─Token(NewLine) |\r\n\r\n|

// #completionTest(60,61) -> virtualNetworksResourceTypes
//@[057:00059) ├─Token(NewLine) |\r\n|
resource comp8 'Microsoft.Network/virtualNetworks@2020-06-01'
//@[000:00061) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00014) | ├─IdentifierSyntax
//@[009:00014) | | └─Token(Identifier) |comp8|
//@[015:00061) | ├─StringSyntax
//@[015:00061) | | └─Token(StringComplete) |'Microsoft.Network/virtualNetworks@2020-06-01'|
//@[061:00061) | ├─SkippedTriviaSyntax
//@[061:00061) | └─SkippedTriviaSyntax
//@[061:00067) ├─Token(NewLine) |\r\n\r\n\r\n|


// issue #3000
//@[014:00016) ├─Token(NewLine) |\r\n|
resource issue3000LogicApp1 'Microsoft.Logic/workflows@2019-05-01' = {
//@[000:00453) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00027) | ├─IdentifierSyntax
//@[009:00027) | | └─Token(Identifier) |issue3000LogicApp1|
//@[028:00066) | ├─StringSyntax
//@[028:00066) | | └─Token(StringComplete) |'Microsoft.Logic/workflows@2019-05-01'|
//@[067:00068) | ├─Token(Assignment) |=|
//@[069:00453) | └─ObjectSyntax
//@[069:00070) |   ├─Token(LeftBrace) |{|
//@[070:00072) |   ├─Token(NewLine) |\r\n|
  name: 'issue3000LogicApp1'
//@[002:00028) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00028) |   | └─StringSyntax
//@[008:00028) |   |   └─Token(StringComplete) |'issue3000LogicApp1'|
//@[028:00030) |   ├─Token(NewLine) |\r\n|
  location: resourceGroup().location
//@[002:00036) |   ├─ObjectPropertySyntax
//@[002:00010) |   | ├─IdentifierSyntax
//@[002:00010) |   | | └─Token(Identifier) |location|
//@[010:00011) |   | ├─Token(Colon) |:|
//@[012:00036) |   | └─PropertyAccessSyntax
//@[012:00027) |   |   ├─FunctionCallSyntax
//@[012:00025) |   |   | ├─IdentifierSyntax
//@[012:00025) |   |   | | └─Token(Identifier) |resourceGroup|
//@[025:00026) |   |   | ├─Token(LeftParen) |(|
//@[026:00027) |   |   | └─Token(RightParen) |)|
//@[027:00028) |   |   ├─Token(Dot) |.|
//@[028:00036) |   |   └─IdentifierSyntax
//@[028:00036) |   |     └─Token(Identifier) |location|
//@[036:00038) |   ├─Token(NewLine) |\r\n|
  properties: {
//@[002:00062) |   ├─ObjectPropertySyntax
//@[002:00012) |   | ├─IdentifierSyntax
//@[002:00012) |   | | └─Token(Identifier) |properties|
//@[012:00013) |   | ├─Token(Colon) |:|
//@[014:00062) |   | └─ObjectSyntax
//@[014:00015) |   |   ├─Token(LeftBrace) |{|
//@[015:00017) |   |   ├─Token(NewLine) |\r\n|
    state: 'Enabled'
//@[004:00020) |   |   ├─ObjectPropertySyntax
//@[004:00009) |   |   | ├─IdentifierSyntax
//@[004:00009) |   |   | | └─Token(Identifier) |state|
//@[009:00010) |   |   | ├─Token(Colon) |:|
//@[011:00020) |   |   | └─StringSyntax
//@[011:00020) |   |   |   └─Token(StringComplete) |'Enabled'|
//@[020:00022) |   |   ├─Token(NewLine) |\r\n|
    definition: ''
//@[004:00018) |   |   ├─ObjectPropertySyntax
//@[004:00014) |   |   | ├─IdentifierSyntax
//@[004:00014) |   |   | | └─Token(Identifier) |definition|
//@[014:00015) |   |   | ├─Token(Colon) |:|
//@[016:00018) |   |   | └─StringSyntax
//@[016:00018) |   |   |   └─Token(StringComplete) |''|
//@[018:00020) |   |   ├─Token(NewLine) |\r\n|
  }
//@[002:00003) |   |   └─Token(RightBrace) |}|
//@[003:00005) |   ├─Token(NewLine) |\r\n|
  identity: {
//@[002:00046) |   ├─ObjectPropertySyntax
//@[002:00010) |   | ├─IdentifierSyntax
//@[002:00010) |   | | └─Token(Identifier) |identity|
//@[010:00011) |   | ├─Token(Colon) |:|
//@[012:00046) |   | └─ObjectSyntax
//@[012:00013) |   |   ├─Token(LeftBrace) |{|
//@[013:00015) |   |   ├─Token(NewLine) |\r\n|
    type: 'SystemAssigned'
//@[004:00026) |   |   ├─ObjectPropertySyntax
//@[004:00008) |   |   | ├─IdentifierSyntax
//@[004:00008) |   |   | | └─Token(Identifier) |type|
//@[008:00009) |   |   | ├─Token(Colon) |:|
//@[010:00026) |   |   | └─StringSyntax
//@[010:00026) |   |   |   └─Token(StringComplete) |'SystemAssigned'|
//@[026:00028) |   |   ├─Token(NewLine) |\r\n|
  }
//@[002:00003) |   |   └─Token(RightBrace) |}|
//@[003:00005) |   ├─Token(NewLine) |\r\n|
  extendedLocation: {}
//@[002:00022) |   ├─ObjectPropertySyntax
//@[002:00018) |   | ├─IdentifierSyntax
//@[002:00018) |   | | └─Token(Identifier) |extendedLocation|
//@[018:00019) |   | ├─Token(Colon) |:|
//@[020:00022) |   | └─ObjectSyntax
//@[020:00021) |   |   ├─Token(LeftBrace) |{|
//@[021:00022) |   |   └─Token(RightBrace) |}|
//@[022:00024) |   ├─Token(NewLine) |\r\n|
  sku: {}
//@[002:00009) |   ├─ObjectPropertySyntax
//@[002:00005) |   | ├─IdentifierSyntax
//@[002:00005) |   | | └─Token(Identifier) |sku|
//@[005:00006) |   | ├─Token(Colon) |:|
//@[007:00009) |   | └─ObjectSyntax
//@[007:00008) |   |   ├─Token(LeftBrace) |{|
//@[008:00009) |   |   └─Token(RightBrace) |}|
//@[009:00011) |   ├─Token(NewLine) |\r\n|
  kind: 'V1'
//@[002:00012) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |kind|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00012) |   | └─StringSyntax
//@[008:00012) |   |   └─Token(StringComplete) |'V1'|
//@[012:00014) |   ├─Token(NewLine) |\r\n|
  managedBy: 'string'
//@[002:00021) |   ├─ObjectPropertySyntax
//@[002:00011) |   | ├─IdentifierSyntax
//@[002:00011) |   | | └─Token(Identifier) |managedBy|
//@[011:00012) |   | ├─Token(Colon) |:|
//@[013:00021) |   | └─StringSyntax
//@[013:00021) |   |   └─Token(StringComplete) |'string'|
//@[021:00023) |   ├─Token(NewLine) |\r\n|
  mangedByExtended: [
//@[002:00048) |   ├─ObjectPropertySyntax
//@[002:00018) |   | ├─IdentifierSyntax
//@[002:00018) |   | | └─Token(Identifier) |mangedByExtended|
//@[018:00019) |   | ├─Token(Colon) |:|
//@[020:00048) |   | └─ArraySyntax
//@[020:00021) |   |   ├─Token(LeftSquare) |[|
//@[021:00023) |   |   ├─Token(NewLine) |\r\n|
   'str1'
//@[003:00009) |   |   ├─ArrayItemSyntax
//@[003:00009) |   |   | └─StringSyntax
//@[003:00009) |   |   |   └─Token(StringComplete) |'str1'|
//@[009:00011) |   |   ├─Token(NewLine) |\r\n|
   'str2'
//@[003:00009) |   |   ├─ArrayItemSyntax
//@[003:00009) |   |   | └─StringSyntax
//@[003:00009) |   |   |   └─Token(StringComplete) |'str2'|
//@[009:00011) |   |   ├─Token(NewLine) |\r\n|
  ]
//@[002:00003) |   |   └─Token(RightSquare) |]|
//@[003:00005) |   ├─Token(NewLine) |\r\n|
  zones: [
//@[002:00037) |   ├─ObjectPropertySyntax
//@[002:00007) |   | ├─IdentifierSyntax
//@[002:00007) |   | | └─Token(Identifier) |zones|
//@[007:00008) |   | ├─Token(Colon) |:|
//@[009:00037) |   | └─ArraySyntax
//@[009:00010) |   |   ├─Token(LeftSquare) |[|
//@[010:00012) |   |   ├─Token(NewLine) |\r\n|
   'str1'
//@[003:00009) |   |   ├─ArrayItemSyntax
//@[003:00009) |   |   | └─StringSyntax
//@[003:00009) |   |   |   └─Token(StringComplete) |'str1'|
//@[009:00011) |   |   ├─Token(NewLine) |\r\n|
   'str2'
//@[003:00009) |   |   ├─ArrayItemSyntax
//@[003:00009) |   |   | └─StringSyntax
//@[003:00009) |   |   |   └─Token(StringComplete) |'str2'|
//@[009:00011) |   |   ├─Token(NewLine) |\r\n|
  ]
//@[002:00003) |   |   └─Token(RightSquare) |]|
//@[003:00005) |   ├─Token(NewLine) |\r\n|
  plan: {}
//@[002:00010) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |plan|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00010) |   | └─ObjectSyntax
//@[008:00009) |   |   ├─Token(LeftBrace) |{|
//@[009:00010) |   |   └─Token(RightBrace) |}|
//@[010:00012) |   ├─Token(NewLine) |\r\n|
  eTag: ''
//@[002:00010) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |eTag|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00010) |   | └─StringSyntax
//@[008:00010) |   |   └─Token(StringComplete) |''|
//@[010:00012) |   ├─Token(NewLine) |\r\n|
  scale: {}  
//@[002:00011) |   ├─ObjectPropertySyntax
//@[002:00007) |   | ├─IdentifierSyntax
//@[002:00007) |   | | └─Token(Identifier) |scale|
//@[007:00008) |   | ├─Token(Colon) |:|
//@[009:00011) |   | └─ObjectSyntax
//@[009:00010) |   |   ├─Token(LeftBrace) |{|
//@[010:00011) |   |   └─Token(RightBrace) |}|
//@[013:00015) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

resource issue3000LogicApp2 'Microsoft.Logic/workflows@2019-05-01' = {
//@[000:00452) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00027) | ├─IdentifierSyntax
//@[009:00027) | | └─Token(Identifier) |issue3000LogicApp2|
//@[028:00066) | ├─StringSyntax
//@[028:00066) | | └─Token(StringComplete) |'Microsoft.Logic/workflows@2019-05-01'|
//@[067:00068) | ├─Token(Assignment) |=|
//@[069:00452) | └─ObjectSyntax
//@[069:00070) |   ├─Token(LeftBrace) |{|
//@[070:00072) |   ├─Token(NewLine) |\r\n|
  name: 'issue3000LogicApp2'
//@[002:00028) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00028) |   | └─StringSyntax
//@[008:00028) |   |   └─Token(StringComplete) |'issue3000LogicApp2'|
//@[028:00030) |   ├─Token(NewLine) |\r\n|
  location: resourceGroup().location
//@[002:00036) |   ├─ObjectPropertySyntax
//@[002:00010) |   | ├─IdentifierSyntax
//@[002:00010) |   | | └─Token(Identifier) |location|
//@[010:00011) |   | ├─Token(Colon) |:|
//@[012:00036) |   | └─PropertyAccessSyntax
//@[012:00027) |   |   ├─FunctionCallSyntax
//@[012:00025) |   |   | ├─IdentifierSyntax
//@[012:00025) |   |   | | └─Token(Identifier) |resourceGroup|
//@[025:00026) |   |   | ├─Token(LeftParen) |(|
//@[026:00027) |   |   | └─Token(RightParen) |)|
//@[027:00028) |   |   ├─Token(Dot) |.|
//@[028:00036) |   |   └─IdentifierSyntax
//@[028:00036) |   |     └─Token(Identifier) |location|
//@[036:00038) |   ├─Token(NewLine) |\r\n|
  properties: {
//@[002:00062) |   ├─ObjectPropertySyntax
//@[002:00012) |   | ├─IdentifierSyntax
//@[002:00012) |   | | └─Token(Identifier) |properties|
//@[012:00013) |   | ├─Token(Colon) |:|
//@[014:00062) |   | └─ObjectSyntax
//@[014:00015) |   |   ├─Token(LeftBrace) |{|
//@[015:00017) |   |   ├─Token(NewLine) |\r\n|
    state: 'Enabled'
//@[004:00020) |   |   ├─ObjectPropertySyntax
//@[004:00009) |   |   | ├─IdentifierSyntax
//@[004:00009) |   |   | | └─Token(Identifier) |state|
//@[009:00010) |   |   | ├─Token(Colon) |:|
//@[011:00020) |   |   | └─StringSyntax
//@[011:00020) |   |   |   └─Token(StringComplete) |'Enabled'|
//@[020:00022) |   |   ├─Token(NewLine) |\r\n|
    definition: ''
//@[004:00018) |   |   ├─ObjectPropertySyntax
//@[004:00014) |   |   | ├─IdentifierSyntax
//@[004:00014) |   |   | | └─Token(Identifier) |definition|
//@[014:00015) |   |   | ├─Token(Colon) |:|
//@[016:00018) |   |   | └─StringSyntax
//@[016:00018) |   |   |   └─Token(StringComplete) |''|
//@[018:00020) |   |   ├─Token(NewLine) |\r\n|
  }
//@[002:00003) |   |   └─Token(RightBrace) |}|
//@[003:00005) |   ├─Token(NewLine) |\r\n|
  identity: 'SystemAssigned'
//@[002:00028) |   ├─ObjectPropertySyntax
//@[002:00010) |   | ├─IdentifierSyntax
//@[002:00010) |   | | └─Token(Identifier) |identity|
//@[010:00011) |   | ├─Token(Colon) |:|
//@[012:00028) |   | └─StringSyntax
//@[012:00028) |   |   └─Token(StringComplete) |'SystemAssigned'|
//@[028:00030) |   ├─Token(NewLine) |\r\n|
  extendedLocation: 'eastus'
//@[002:00028) |   ├─ObjectPropertySyntax
//@[002:00018) |   | ├─IdentifierSyntax
//@[002:00018) |   | | └─Token(Identifier) |extendedLocation|
//@[018:00019) |   | ├─Token(Colon) |:|
//@[020:00028) |   | └─StringSyntax
//@[020:00028) |   |   └─Token(StringComplete) |'eastus'|
//@[028:00030) |   ├─Token(NewLine) |\r\n|
  sku: 'Basic'
//@[002:00014) |   ├─ObjectPropertySyntax
//@[002:00005) |   | ├─IdentifierSyntax
//@[002:00005) |   | | └─Token(Identifier) |sku|
//@[005:00006) |   | ├─Token(Colon) |:|
//@[007:00014) |   | └─StringSyntax
//@[007:00014) |   |   └─Token(StringComplete) |'Basic'|
//@[014:00016) |   ├─Token(NewLine) |\r\n|
  kind: {
//@[002:00030) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |kind|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00030) |   | └─ObjectSyntax
//@[008:00009) |   |   ├─Token(LeftBrace) |{|
//@[009:00011) |   |   ├─Token(NewLine) |\r\n|
    name: 'V1'
//@[004:00014) |   |   ├─ObjectPropertySyntax
//@[004:00008) |   |   | ├─IdentifierSyntax
//@[004:00008) |   |   | | └─Token(Identifier) |name|
//@[008:00009) |   |   | ├─Token(Colon) |:|
//@[010:00014) |   |   | └─StringSyntax
//@[010:00014) |   |   |   └─Token(StringComplete) |'V1'|
//@[014:00016) |   |   ├─Token(NewLine) |\r\n|
  }
//@[002:00003) |   |   └─Token(RightBrace) |}|
//@[003:00005) |   ├─Token(NewLine) |\r\n|
  managedBy: {}
//@[002:00015) |   ├─ObjectPropertySyntax
//@[002:00011) |   | ├─IdentifierSyntax
//@[002:00011) |   | | └─Token(Identifier) |managedBy|
//@[011:00012) |   | ├─Token(Colon) |:|
//@[013:00015) |   | └─ObjectSyntax
//@[013:00014) |   |   ├─Token(LeftBrace) |{|
//@[014:00015) |   |   └─Token(RightBrace) |}|
//@[015:00017) |   ├─Token(NewLine) |\r\n|
  mangedByExtended: [
//@[002:00040) |   ├─ObjectPropertySyntax
//@[002:00018) |   | ├─IdentifierSyntax
//@[002:00018) |   | | └─Token(Identifier) |mangedByExtended|
//@[018:00019) |   | ├─Token(Colon) |:|
//@[020:00040) |   | └─ArraySyntax
//@[020:00021) |   |   ├─Token(LeftSquare) |[|
//@[021:00023) |   |   ├─Token(NewLine) |\r\n|
   {}
//@[003:00005) |   |   ├─ArrayItemSyntax
//@[003:00005) |   |   | └─ObjectSyntax
//@[003:00004) |   |   |   ├─Token(LeftBrace) |{|
//@[004:00005) |   |   |   └─Token(RightBrace) |}|
//@[005:00007) |   |   ├─Token(NewLine) |\r\n|
   {}
//@[003:00005) |   |   ├─ArrayItemSyntax
//@[003:00005) |   |   | └─ObjectSyntax
//@[003:00004) |   |   |   ├─Token(LeftBrace) |{|
//@[004:00005) |   |   |   └─Token(RightBrace) |}|
//@[005:00007) |   |   ├─Token(NewLine) |\r\n|
  ]
//@[002:00003) |   |   └─Token(RightSquare) |]|
//@[003:00005) |   ├─Token(NewLine) |\r\n|
  zones: [
//@[002:00029) |   ├─ObjectPropertySyntax
//@[002:00007) |   | ├─IdentifierSyntax
//@[002:00007) |   | | └─Token(Identifier) |zones|
//@[007:00008) |   | ├─Token(Colon) |:|
//@[009:00029) |   | └─ArraySyntax
//@[009:00010) |   |   ├─Token(LeftSquare) |[|
//@[010:00012) |   |   ├─Token(NewLine) |\r\n|
   {}
//@[003:00005) |   |   ├─ArrayItemSyntax
//@[003:00005) |   |   | └─ObjectSyntax
//@[003:00004) |   |   |   ├─Token(LeftBrace) |{|
//@[004:00005) |   |   |   └─Token(RightBrace) |}|
//@[005:00007) |   |   ├─Token(NewLine) |\r\n|
   {}
//@[003:00005) |   |   ├─ArrayItemSyntax
//@[003:00005) |   |   | └─ObjectSyntax
//@[003:00004) |   |   |   ├─Token(LeftBrace) |{|
//@[004:00005) |   |   |   └─Token(RightBrace) |}|
//@[005:00007) |   |   ├─Token(NewLine) |\r\n|
  ]
//@[002:00003) |   |   └─Token(RightSquare) |]|
//@[003:00005) |   ├─Token(NewLine) |\r\n|
  plan: ''
//@[002:00010) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |plan|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00010) |   | └─StringSyntax
//@[008:00010) |   |   └─Token(StringComplete) |''|
//@[010:00012) |   ├─Token(NewLine) |\r\n|
  eTag: {}
//@[002:00010) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |eTag|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00010) |   | └─ObjectSyntax
//@[008:00009) |   |   ├─Token(LeftBrace) |{|
//@[009:00010) |   |   └─Token(RightBrace) |}|
//@[010:00012) |   ├─Token(NewLine) |\r\n|
  scale: [
//@[002:00021) |   ├─ObjectPropertySyntax
//@[002:00007) |   | ├─IdentifierSyntax
//@[002:00007) |   | | └─Token(Identifier) |scale|
//@[007:00008) |   | ├─Token(Colon) |:|
//@[009:00021) |   | └─ArraySyntax
//@[009:00010) |   |   ├─Token(LeftSquare) |[|
//@[010:00012) |   |   ├─Token(NewLine) |\r\n|
  {}
//@[002:00004) |   |   ├─ArrayItemSyntax
//@[002:00004) |   |   | └─ObjectSyntax
//@[002:00003) |   |   |   ├─Token(LeftBrace) |{|
//@[003:00004) |   |   |   └─Token(RightBrace) |}|
//@[004:00006) |   |   ├─Token(NewLine) |\r\n|
  ]  
//@[002:00003) |   |   └─Token(RightSquare) |]|
//@[005:00007) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

resource issue3000stg 'Microsoft.Storage/storageAccounts@2021-04-01' = {
//@[000:00234) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00021) | ├─IdentifierSyntax
//@[009:00021) | | └─Token(Identifier) |issue3000stg|
//@[022:00068) | ├─StringSyntax
//@[022:00068) | | └─Token(StringComplete) |'Microsoft.Storage/storageAccounts@2021-04-01'|
//@[069:00070) | ├─Token(Assignment) |=|
//@[071:00234) | └─ObjectSyntax
//@[071:00072) |   ├─Token(LeftBrace) |{|
//@[072:00074) |   ├─Token(NewLine) |\r\n|
  name: 'issue3000stg'
//@[002:00022) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00022) |   | └─StringSyntax
//@[008:00022) |   |   └─Token(StringComplete) |'issue3000stg'|
//@[022:00024) |   ├─Token(NewLine) |\r\n|
  kind: 'StorageV2'
//@[002:00019) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |kind|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00019) |   | └─StringSyntax
//@[008:00019) |   |   └─Token(StringComplete) |'StorageV2'|
//@[019:00021) |   ├─Token(NewLine) |\r\n|
  location: 'West US'
//@[002:00021) |   ├─ObjectPropertySyntax
//@[002:00010) |   | ├─IdentifierSyntax
//@[002:00010) |   | | └─Token(Identifier) |location|
//@[010:00011) |   | ├─Token(Colon) |:|
//@[012:00021) |   | └─StringSyntax
//@[012:00021) |   |   └─Token(StringComplete) |'West US'|
//@[021:00023) |   ├─Token(NewLine) |\r\n|
  sku: {
//@[002:00042) |   ├─ObjectPropertySyntax
//@[002:00005) |   | ├─IdentifierSyntax
//@[002:00005) |   | | └─Token(Identifier) |sku|
//@[005:00006) |   | ├─Token(Colon) |:|
//@[007:00042) |   | └─ObjectSyntax
//@[007:00008) |   |   ├─Token(LeftBrace) |{|
//@[008:00010) |   |   ├─Token(NewLine) |\r\n|
    name: 'Premium_LRS'    
//@[004:00023) |   |   ├─ObjectPropertySyntax
//@[004:00008) |   |   | ├─IdentifierSyntax
//@[004:00008) |   |   | | └─Token(Identifier) |name|
//@[008:00009) |   |   | ├─Token(Colon) |:|
//@[010:00023) |   |   | └─StringSyntax
//@[010:00023) |   |   |   └─Token(StringComplete) |'Premium_LRS'|
//@[027:00029) |   |   ├─Token(NewLine) |\r\n|
  }
//@[002:00003) |   |   └─Token(RightBrace) |}|
//@[003:00005) |   ├─Token(NewLine) |\r\n|
  madeUpProperty: {}
//@[002:00020) |   ├─ObjectPropertySyntax
//@[002:00016) |   | ├─IdentifierSyntax
//@[002:00016) |   | | └─Token(Identifier) |madeUpProperty|
//@[016:00017) |   | ├─Token(Colon) |:|
//@[018:00020) |   | └─ObjectSyntax
//@[018:00019) |   |   ├─Token(LeftBrace) |{|
//@[019:00020) |   |   └─Token(RightBrace) |}|
//@[020:00022) |   ├─Token(NewLine) |\r\n|
  managedByExtended: []
//@[002:00023) |   ├─ObjectPropertySyntax
//@[002:00019) |   | ├─IdentifierSyntax
//@[002:00019) |   | | └─Token(Identifier) |managedByExtended|
//@[019:00020) |   | ├─Token(Colon) |:|
//@[021:00023) |   | └─ArraySyntax
//@[021:00022) |   |   ├─Token(LeftSquare) |[|
//@[022:00023) |   |   └─Token(RightSquare) |]|
//@[023:00025) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

var issue3000stgMadeUpProperty = issue3000stg.madeUpProperty
//@[000:00060) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00030) | ├─IdentifierSyntax
//@[004:00030) | | └─Token(Identifier) |issue3000stgMadeUpProperty|
//@[031:00032) | ├─Token(Assignment) |=|
//@[033:00060) | └─PropertyAccessSyntax
//@[033:00045) |   ├─VariableAccessSyntax
//@[033:00045) |   | └─IdentifierSyntax
//@[033:00045) |   |   └─Token(Identifier) |issue3000stg|
//@[045:00046) |   ├─Token(Dot) |.|
//@[046:00060) |   └─IdentifierSyntax
//@[046:00060) |     └─Token(Identifier) |madeUpProperty|
//@[060:00062) ├─Token(NewLine) |\r\n|
var issue3000stgManagedBy = issue3000stg.managedBy
//@[000:00050) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00025) | ├─IdentifierSyntax
//@[004:00025) | | └─Token(Identifier) |issue3000stgManagedBy|
//@[026:00027) | ├─Token(Assignment) |=|
//@[028:00050) | └─PropertyAccessSyntax
//@[028:00040) |   ├─VariableAccessSyntax
//@[028:00040) |   | └─IdentifierSyntax
//@[028:00040) |   |   └─Token(Identifier) |issue3000stg|
//@[040:00041) |   ├─Token(Dot) |.|
//@[041:00050) |   └─IdentifierSyntax
//@[041:00050) |     └─Token(Identifier) |managedBy|
//@[050:00052) ├─Token(NewLine) |\r\n|
var issue3000stgManagedByExtended = issue3000stg.managedByExtended
//@[000:00066) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00033) | ├─IdentifierSyntax
//@[004:00033) | | └─Token(Identifier) |issue3000stgManagedByExtended|
//@[034:00035) | ├─Token(Assignment) |=|
//@[036:00066) | └─PropertyAccessSyntax
//@[036:00048) |   ├─VariableAccessSyntax
//@[036:00048) |   | └─IdentifierSyntax
//@[036:00048) |   |   └─Token(Identifier) |issue3000stg|
//@[048:00049) |   ├─Token(Dot) |.|
//@[049:00066) |   └─IdentifierSyntax
//@[049:00066) |     └─Token(Identifier) |managedByExtended|
//@[066:00070) ├─Token(NewLine) |\r\n\r\n|

param dataCollectionRule object
//@[000:00031) ├─ParameterDeclarationSyntax
//@[000:00005) | ├─Token(Identifier) |param|
//@[006:00024) | ├─IdentifierSyntax
//@[006:00024) | | └─Token(Identifier) |dataCollectionRule|
//@[025:00031) | └─SimpleTypeSyntax
//@[025:00031) |   └─Token(Identifier) |object|
//@[031:00033) ├─Token(NewLine) |\r\n|
param tags object
//@[000:00017) ├─ParameterDeclarationSyntax
//@[000:00005) | ├─Token(Identifier) |param|
//@[006:00010) | ├─IdentifierSyntax
//@[006:00010) | | └─Token(Identifier) |tags|
//@[011:00017) | └─SimpleTypeSyntax
//@[011:00017) |   └─Token(Identifier) |object|
//@[017:00021) ├─Token(NewLine) |\r\n\r\n|

var defaultLogAnalyticsWorkspace = {
//@[000:00088) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00032) | ├─IdentifierSyntax
//@[004:00032) | | └─Token(Identifier) |defaultLogAnalyticsWorkspace|
//@[033:00034) | ├─Token(Assignment) |=|
//@[035:00088) | └─ObjectSyntax
//@[035:00036) |   ├─Token(LeftBrace) |{|
//@[036:00038) |   ├─Token(NewLine) |\r\n|
  subscriptionId: subscription().subscriptionId
//@[002:00047) |   ├─ObjectPropertySyntax
//@[002:00016) |   | ├─IdentifierSyntax
//@[002:00016) |   | | └─Token(Identifier) |subscriptionId|
//@[016:00017) |   | ├─Token(Colon) |:|
//@[018:00047) |   | └─PropertyAccessSyntax
//@[018:00032) |   |   ├─FunctionCallSyntax
//@[018:00030) |   |   | ├─IdentifierSyntax
//@[018:00030) |   |   | | └─Token(Identifier) |subscription|
//@[030:00031) |   |   | ├─Token(LeftParen) |(|
//@[031:00032) |   |   | └─Token(RightParen) |)|
//@[032:00033) |   |   ├─Token(Dot) |.|
//@[033:00047) |   |   └─IdentifierSyntax
//@[033:00047) |   |     └─Token(Identifier) |subscriptionId|
//@[047:00049) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

resource logAnalyticsWorkspaces 'Microsoft.OperationalInsights/workspaces@2020-10-01' existing = [for logAnalyticsWorkspace in dataCollectionRule.destinations.logAnalyticsWorkspaces: {
//@[000:00364) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00031) | ├─IdentifierSyntax
//@[009:00031) | | └─Token(Identifier) |logAnalyticsWorkspaces|
//@[032:00085) | ├─StringSyntax
//@[032:00085) | | └─Token(StringComplete) |'Microsoft.OperationalInsights/workspaces@2020-10-01'|
//@[086:00094) | ├─Token(Identifier) |existing|
//@[095:00096) | ├─Token(Assignment) |=|
//@[097:00364) | └─ForSyntax
//@[097:00098) |   ├─Token(LeftSquare) |[|
//@[098:00101) |   ├─Token(Identifier) |for|
//@[102:00123) |   ├─LocalVariableSyntax
//@[102:00123) |   | └─IdentifierSyntax
//@[102:00123) |   |   └─Token(Identifier) |logAnalyticsWorkspace|
//@[124:00126) |   ├─Token(Identifier) |in|
//@[127:00181) |   ├─PropertyAccessSyntax
//@[127:00158) |   | ├─PropertyAccessSyntax
//@[127:00145) |   | | ├─VariableAccessSyntax
//@[127:00145) |   | | | └─IdentifierSyntax
//@[127:00145) |   | | |   └─Token(Identifier) |dataCollectionRule|
//@[145:00146) |   | | ├─Token(Dot) |.|
//@[146:00158) |   | | └─IdentifierSyntax
//@[146:00158) |   | |   └─Token(Identifier) |destinations|
//@[158:00159) |   | ├─Token(Dot) |.|
//@[159:00181) |   | └─IdentifierSyntax
//@[159:00181) |   |   └─Token(Identifier) |logAnalyticsWorkspaces|
//@[181:00182) |   ├─Token(Colon) |:|
//@[183:00363) |   ├─ObjectSyntax
//@[183:00184) |   | ├─Token(LeftBrace) |{|
//@[184:00186) |   | ├─Token(NewLine) |\r\n|
  name: logAnalyticsWorkspace.name
//@[002:00034) |   | ├─ObjectPropertySyntax
//@[002:00006) |   | | ├─IdentifierSyntax
//@[002:00006) |   | | | └─Token(Identifier) |name|
//@[006:00007) |   | | ├─Token(Colon) |:|
//@[008:00034) |   | | └─PropertyAccessSyntax
//@[008:00029) |   | |   ├─VariableAccessSyntax
//@[008:00029) |   | |   | └─IdentifierSyntax
//@[008:00029) |   | |   |   └─Token(Identifier) |logAnalyticsWorkspace|
//@[029:00030) |   | |   ├─Token(Dot) |.|
//@[030:00034) |   | |   └─IdentifierSyntax
//@[030:00034) |   | |     └─Token(Identifier) |name|
//@[034:00036) |   | ├─Token(NewLine) |\r\n|
  scope: resourceGroup( union( defaultLogAnalyticsWorkspace, logAnalyticsWorkspace ).subscriptionId, logAnalyticsWorkspace.resourceGroup )
//@[002:00138) |   | ├─ObjectPropertySyntax
//@[002:00007) |   | | ├─IdentifierSyntax
//@[002:00007) |   | | | └─Token(Identifier) |scope|
//@[007:00008) |   | | ├─Token(Colon) |:|
//@[009:00138) |   | | └─FunctionCallSyntax
//@[009:00022) |   | |   ├─IdentifierSyntax
//@[009:00022) |   | |   | └─Token(Identifier) |resourceGroup|
//@[022:00023) |   | |   ├─Token(LeftParen) |(|
//@[024:00099) |   | |   ├─FunctionArgumentSyntax
//@[024:00099) |   | |   | └─PropertyAccessSyntax
//@[024:00084) |   | |   |   ├─FunctionCallSyntax
//@[024:00029) |   | |   |   | ├─IdentifierSyntax
//@[024:00029) |   | |   |   | | └─Token(Identifier) |union|
//@[029:00030) |   | |   |   | ├─Token(LeftParen) |(|
//@[031:00059) |   | |   |   | ├─FunctionArgumentSyntax
//@[031:00059) |   | |   |   | | └─VariableAccessSyntax
//@[031:00059) |   | |   |   | |   └─IdentifierSyntax
//@[031:00059) |   | |   |   | |     └─Token(Identifier) |defaultLogAnalyticsWorkspace|
//@[059:00060) |   | |   |   | ├─Token(Comma) |,|
//@[061:00082) |   | |   |   | ├─FunctionArgumentSyntax
//@[061:00082) |   | |   |   | | └─VariableAccessSyntax
//@[061:00082) |   | |   |   | |   └─IdentifierSyntax
//@[061:00082) |   | |   |   | |     └─Token(Identifier) |logAnalyticsWorkspace|
//@[083:00084) |   | |   |   | └─Token(RightParen) |)|
//@[084:00085) |   | |   |   ├─Token(Dot) |.|
//@[085:00099) |   | |   |   └─IdentifierSyntax
//@[085:00099) |   | |   |     └─Token(Identifier) |subscriptionId|
//@[099:00100) |   | |   ├─Token(Comma) |,|
//@[101:00136) |   | |   ├─FunctionArgumentSyntax
//@[101:00136) |   | |   | └─PropertyAccessSyntax
//@[101:00122) |   | |   |   ├─VariableAccessSyntax
//@[101:00122) |   | |   |   | └─IdentifierSyntax
//@[101:00122) |   | |   |   |   └─Token(Identifier) |logAnalyticsWorkspace|
//@[122:00123) |   | |   |   ├─Token(Dot) |.|
//@[123:00136) |   | |   |   └─IdentifierSyntax
//@[123:00136) |   | |   |     └─Token(Identifier) |resourceGroup|
//@[137:00138) |   | |   └─Token(RightParen) |)|
//@[138:00140) |   | ├─Token(NewLine) |\r\n|
}]
//@[000:00001) |   | └─Token(RightBrace) |}|
//@[001:00002) |   └─Token(RightSquare) |]|
//@[002:00006) ├─Token(NewLine) |\r\n\r\n|

resource dataCollectionRuleRes 'Microsoft.Insights/dataCollectionRules@2021-04-01' = {
//@[000:00837) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00030) | ├─IdentifierSyntax
//@[009:00030) | | └─Token(Identifier) |dataCollectionRuleRes|
//@[031:00082) | ├─StringSyntax
//@[031:00082) | | └─Token(StringComplete) |'Microsoft.Insights/dataCollectionRules@2021-04-01'|
//@[083:00084) | ├─Token(Assignment) |=|
//@[085:00837) | └─ObjectSyntax
//@[085:00086) |   ├─Token(LeftBrace) |{|
//@[086:00088) |   ├─Token(NewLine) |\r\n|
  name: dataCollectionRule.name
//@[002:00031) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00031) |   | └─PropertyAccessSyntax
//@[008:00026) |   |   ├─VariableAccessSyntax
//@[008:00026) |   |   | └─IdentifierSyntax
//@[008:00026) |   |   |   └─Token(Identifier) |dataCollectionRule|
//@[026:00027) |   |   ├─Token(Dot) |.|
//@[027:00031) |   |   └─IdentifierSyntax
//@[027:00031) |   |     └─Token(Identifier) |name|
//@[031:00033) |   ├─Token(NewLine) |\r\n|
  location: dataCollectionRule.location
//@[002:00039) |   ├─ObjectPropertySyntax
//@[002:00010) |   | ├─IdentifierSyntax
//@[002:00010) |   | | └─Token(Identifier) |location|
//@[010:00011) |   | ├─Token(Colon) |:|
//@[012:00039) |   | └─PropertyAccessSyntax
//@[012:00030) |   |   ├─VariableAccessSyntax
//@[012:00030) |   |   | └─IdentifierSyntax
//@[012:00030) |   |   |   └─Token(Identifier) |dataCollectionRule|
//@[030:00031) |   |   ├─Token(Dot) |.|
//@[031:00039) |   |   └─IdentifierSyntax
//@[031:00039) |   |     └─Token(Identifier) |location|
//@[039:00041) |   ├─Token(NewLine) |\r\n|
  tags: tags
//@[002:00012) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |tags|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00012) |   | └─VariableAccessSyntax
//@[008:00012) |   |   └─IdentifierSyntax
//@[008:00012) |   |     └─Token(Identifier) |tags|
//@[012:00014) |   ├─Token(NewLine) |\r\n|
  kind: dataCollectionRule.kind
//@[002:00031) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |kind|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00031) |   | └─PropertyAccessSyntax
//@[008:00026) |   |   ├─VariableAccessSyntax
//@[008:00026) |   |   | └─IdentifierSyntax
//@[008:00026) |   |   |   └─Token(Identifier) |dataCollectionRule|
//@[026:00027) |   |   ├─Token(Dot) |.|
//@[027:00031) |   |   └─IdentifierSyntax
//@[027:00031) |   |     └─Token(Identifier) |kind|
//@[031:00033) |   ├─Token(NewLine) |\r\n|
  properties: {
//@[002:00625) |   ├─ObjectPropertySyntax
//@[002:00012) |   | ├─IdentifierSyntax
//@[002:00012) |   | | └─Token(Identifier) |properties|
//@[012:00013) |   | ├─Token(Colon) |:|
//@[014:00625) |   | └─ObjectSyntax
//@[014:00015) |   |   ├─Token(LeftBrace) |{|
//@[015:00017) |   |   ├─Token(NewLine) |\r\n|
    description: dataCollectionRule.description
//@[004:00047) |   |   ├─ObjectPropertySyntax
//@[004:00015) |   |   | ├─IdentifierSyntax
//@[004:00015) |   |   | | └─Token(Identifier) |description|
//@[015:00016) |   |   | ├─Token(Colon) |:|
//@[017:00047) |   |   | └─PropertyAccessSyntax
//@[017:00035) |   |   |   ├─VariableAccessSyntax
//@[017:00035) |   |   |   | └─IdentifierSyntax
//@[017:00035) |   |   |   |   └─Token(Identifier) |dataCollectionRule|
//@[035:00036) |   |   |   ├─Token(Dot) |.|
//@[036:00047) |   |   |   └─IdentifierSyntax
//@[036:00047) |   |   |     └─Token(Identifier) |description|
//@[047:00049) |   |   ├─Token(NewLine) |\r\n|
    destinations: union(empty(dataCollectionRule.destinations.azureMonitorMetrics.name) ? {} : {
//@[004:00460) |   |   ├─ObjectPropertySyntax
//@[004:00016) |   |   | ├─IdentifierSyntax
//@[004:00016) |   |   | | └─Token(Identifier) |destinations|
//@[016:00017) |   |   | ├─Token(Colon) |:|
//@[018:00460) |   |   | └─FunctionCallSyntax
//@[018:00023) |   |   |   ├─IdentifierSyntax
//@[018:00023) |   |   |   | └─Token(Identifier) |union|
//@[023:00024) |   |   |   ├─Token(LeftParen) |(|
//@[024:00214) |   |   |   ├─FunctionArgumentSyntax
//@[024:00214) |   |   |   | └─TernaryOperationSyntax
//@[024:00087) |   |   |   |   ├─FunctionCallSyntax
//@[024:00029) |   |   |   |   | ├─IdentifierSyntax
//@[024:00029) |   |   |   |   | | └─Token(Identifier) |empty|
//@[029:00030) |   |   |   |   | ├─Token(LeftParen) |(|
//@[030:00086) |   |   |   |   | ├─FunctionArgumentSyntax
//@[030:00086) |   |   |   |   | | └─PropertyAccessSyntax
//@[030:00081) |   |   |   |   | |   ├─PropertyAccessSyntax
//@[030:00061) |   |   |   |   | |   | ├─PropertyAccessSyntax
//@[030:00048) |   |   |   |   | |   | | ├─VariableAccessSyntax
//@[030:00048) |   |   |   |   | |   | | | └─IdentifierSyntax
//@[030:00048) |   |   |   |   | |   | | |   └─Token(Identifier) |dataCollectionRule|
//@[048:00049) |   |   |   |   | |   | | ├─Token(Dot) |.|
//@[049:00061) |   |   |   |   | |   | | └─IdentifierSyntax
//@[049:00061) |   |   |   |   | |   | |   └─Token(Identifier) |destinations|
//@[061:00062) |   |   |   |   | |   | ├─Token(Dot) |.|
//@[062:00081) |   |   |   |   | |   | └─IdentifierSyntax
//@[062:00081) |   |   |   |   | |   |   └─Token(Identifier) |azureMonitorMetrics|
//@[081:00082) |   |   |   |   | |   ├─Token(Dot) |.|
//@[082:00086) |   |   |   |   | |   └─IdentifierSyntax
//@[082:00086) |   |   |   |   | |     └─Token(Identifier) |name|
//@[086:00087) |   |   |   |   | └─Token(RightParen) |)|
//@[088:00089) |   |   |   |   ├─Token(Question) |?|
//@[090:00092) |   |   |   |   ├─ObjectSyntax
//@[090:00091) |   |   |   |   | ├─Token(LeftBrace) |{|
//@[091:00092) |   |   |   |   | └─Token(RightBrace) |}|
//@[093:00094) |   |   |   |   ├─Token(Colon) |:|
//@[095:00214) |   |   |   |   └─ObjectSyntax
//@[095:00096) |   |   |   |     ├─Token(LeftBrace) |{|
//@[096:00098) |   |   |   |     ├─Token(NewLine) |\r\n|
      azureMonitorMetrics: {
//@[006:00109) |   |   |   |     ├─ObjectPropertySyntax
//@[006:00025) |   |   |   |     | ├─IdentifierSyntax
//@[006:00025) |   |   |   |     | | └─Token(Identifier) |azureMonitorMetrics|
//@[025:00026) |   |   |   |     | ├─Token(Colon) |:|
//@[027:00109) |   |   |   |     | └─ObjectSyntax
//@[027:00028) |   |   |   |     |   ├─Token(LeftBrace) |{|
//@[028:00030) |   |   |   |     |   ├─Token(NewLine) |\r\n|
        name: dataCollectionRule.destinations.azureMonitorMetrics.name
//@[008:00070) |   |   |   |     |   ├─ObjectPropertySyntax
//@[008:00012) |   |   |   |     |   | ├─IdentifierSyntax
//@[008:00012) |   |   |   |     |   | | └─Token(Identifier) |name|
//@[012:00013) |   |   |   |     |   | ├─Token(Colon) |:|
//@[014:00070) |   |   |   |     |   | └─PropertyAccessSyntax
//@[014:00065) |   |   |   |     |   |   ├─PropertyAccessSyntax
//@[014:00045) |   |   |   |     |   |   | ├─PropertyAccessSyntax
//@[014:00032) |   |   |   |     |   |   | | ├─VariableAccessSyntax
//@[014:00032) |   |   |   |     |   |   | | | └─IdentifierSyntax
//@[014:00032) |   |   |   |     |   |   | | |   └─Token(Identifier) |dataCollectionRule|
//@[032:00033) |   |   |   |     |   |   | | ├─Token(Dot) |.|
//@[033:00045) |   |   |   |     |   |   | | └─IdentifierSyntax
//@[033:00045) |   |   |   |     |   |   | |   └─Token(Identifier) |destinations|
//@[045:00046) |   |   |   |     |   |   | ├─Token(Dot) |.|
//@[046:00065) |   |   |   |     |   |   | └─IdentifierSyntax
//@[046:00065) |   |   |   |     |   |   |   └─Token(Identifier) |azureMonitorMetrics|
//@[065:00066) |   |   |   |     |   |   ├─Token(Dot) |.|
//@[066:00070) |   |   |   |     |   |   └─IdentifierSyntax
//@[066:00070) |   |   |   |     |   |     └─Token(Identifier) |name|
//@[070:00072) |   |   |   |     |   ├─Token(NewLine) |\r\n|
      }
//@[006:00007) |   |   |   |     |   └─Token(RightBrace) |}|
//@[007:00009) |   |   |   |     ├─Token(NewLine) |\r\n|
    },{
//@[004:00005) |   |   |   |     └─Token(RightBrace) |}|
//@[005:00006) |   |   |   ├─Token(Comma) |,|
//@[006:00250) |   |   |   ├─FunctionArgumentSyntax
//@[006:00250) |   |   |   | └─ObjectSyntax
//@[006:00007) |   |   |   |   ├─Token(LeftBrace) |{|
//@[007:00009) |   |   |   |   ├─Token(NewLine) |\r\n|
      logAnalytics: [for (logAnalyticsWorkspace, i) in dataCollectionRule.destinations.logAnalyticsWorkspaces: {
//@[006:00234) |   |   |   |   ├─ObjectPropertySyntax
//@[006:00018) |   |   |   |   | ├─IdentifierSyntax
//@[006:00018) |   |   |   |   | | └─Token(Identifier) |logAnalytics|
//@[018:00019) |   |   |   |   | ├─Token(Colon) |:|
//@[020:00234) |   |   |   |   | └─ForSyntax
//@[020:00021) |   |   |   |   |   ├─Token(LeftSquare) |[|
//@[021:00024) |   |   |   |   |   ├─Token(Identifier) |for|
//@[025:00051) |   |   |   |   |   ├─VariableBlockSyntax
//@[025:00026) |   |   |   |   |   | ├─Token(LeftParen) |(|
//@[026:00047) |   |   |   |   |   | ├─LocalVariableSyntax
//@[026:00047) |   |   |   |   |   | | └─IdentifierSyntax
//@[026:00047) |   |   |   |   |   | |   └─Token(Identifier) |logAnalyticsWorkspace|
//@[047:00048) |   |   |   |   |   | ├─Token(Comma) |,|
//@[049:00050) |   |   |   |   |   | ├─LocalVariableSyntax
//@[049:00050) |   |   |   |   |   | | └─IdentifierSyntax
//@[049:00050) |   |   |   |   |   | |   └─Token(Identifier) |i|
//@[050:00051) |   |   |   |   |   | └─Token(RightParen) |)|
//@[052:00054) |   |   |   |   |   ├─Token(Identifier) |in|
//@[055:00109) |   |   |   |   |   ├─PropertyAccessSyntax
//@[055:00086) |   |   |   |   |   | ├─PropertyAccessSyntax
//@[055:00073) |   |   |   |   |   | | ├─VariableAccessSyntax
//@[055:00073) |   |   |   |   |   | | | └─IdentifierSyntax
//@[055:00073) |   |   |   |   |   | | |   └─Token(Identifier) |dataCollectionRule|
//@[073:00074) |   |   |   |   |   | | ├─Token(Dot) |.|
//@[074:00086) |   |   |   |   |   | | └─IdentifierSyntax
//@[074:00086) |   |   |   |   |   | |   └─Token(Identifier) |destinations|
//@[086:00087) |   |   |   |   |   | ├─Token(Dot) |.|
//@[087:00109) |   |   |   |   |   | └─IdentifierSyntax
//@[087:00109) |   |   |   |   |   |   └─Token(Identifier) |logAnalyticsWorkspaces|
//@[109:00110) |   |   |   |   |   ├─Token(Colon) |:|
//@[111:00233) |   |   |   |   |   ├─ObjectSyntax
//@[111:00112) |   |   |   |   |   | ├─Token(LeftBrace) |{|
//@[112:00114) |   |   |   |   |   | ├─Token(NewLine) |\r\n|
        name: logAnalyticsWorkspace.destinationName
//@[008:00051) |   |   |   |   |   | ├─ObjectPropertySyntax
//@[008:00012) |   |   |   |   |   | | ├─IdentifierSyntax
//@[008:00012) |   |   |   |   |   | | | └─Token(Identifier) |name|
//@[012:00013) |   |   |   |   |   | | ├─Token(Colon) |:|
//@[014:00051) |   |   |   |   |   | | └─PropertyAccessSyntax
//@[014:00035) |   |   |   |   |   | |   ├─VariableAccessSyntax
//@[014:00035) |   |   |   |   |   | |   | └─IdentifierSyntax
//@[014:00035) |   |   |   |   |   | |   |   └─Token(Identifier) |logAnalyticsWorkspace|
//@[035:00036) |   |   |   |   |   | |   ├─Token(Dot) |.|
//@[036:00051) |   |   |   |   |   | |   └─IdentifierSyntax
//@[036:00051) |   |   |   |   |   | |     └─Token(Identifier) |destinationName|
//@[051:00053) |   |   |   |   |   | ├─Token(NewLine) |\r\n|
        workspaceResourceId: logAnalyticsWorkspaces[i].id
//@[008:00057) |   |   |   |   |   | ├─ObjectPropertySyntax
//@[008:00027) |   |   |   |   |   | | ├─IdentifierSyntax
//@[008:00027) |   |   |   |   |   | | | └─Token(Identifier) |workspaceResourceId|
//@[027:00028) |   |   |   |   |   | | ├─Token(Colon) |:|
//@[029:00057) |   |   |   |   |   | | └─PropertyAccessSyntax
//@[029:00054) |   |   |   |   |   | |   ├─ArrayAccessSyntax
//@[029:00051) |   |   |   |   |   | |   | ├─VariableAccessSyntax
//@[029:00051) |   |   |   |   |   | |   | | └─IdentifierSyntax
//@[029:00051) |   |   |   |   |   | |   | |   └─Token(Identifier) |logAnalyticsWorkspaces|
//@[051:00052) |   |   |   |   |   | |   | ├─Token(LeftSquare) |[|
//@[052:00053) |   |   |   |   |   | |   | ├─VariableAccessSyntax
//@[052:00053) |   |   |   |   |   | |   | | └─IdentifierSyntax
//@[052:00053) |   |   |   |   |   | |   | |   └─Token(Identifier) |i|
//@[053:00054) |   |   |   |   |   | |   | └─Token(RightSquare) |]|
//@[054:00055) |   |   |   |   |   | |   ├─Token(Dot) |.|
//@[055:00057) |   |   |   |   |   | |   └─IdentifierSyntax
//@[055:00057) |   |   |   |   |   | |     └─Token(Identifier) |id|
//@[057:00059) |   |   |   |   |   | ├─Token(NewLine) |\r\n|
      }]
//@[006:00007) |   |   |   |   |   | └─Token(RightBrace) |}|
//@[007:00008) |   |   |   |   |   └─Token(RightSquare) |]|
//@[008:00010) |   |   |   |   ├─Token(NewLine) |\r\n|
    })
//@[004:00005) |   |   |   |   └─Token(RightBrace) |}|
//@[005:00006) |   |   |   └─Token(RightParen) |)|
//@[006:00008) |   |   ├─Token(NewLine) |\r\n|
    dataSources: dataCollectionRule.dataSources
//@[004:00047) |   |   ├─ObjectPropertySyntax
//@[004:00015) |   |   | ├─IdentifierSyntax
//@[004:00015) |   |   | | └─Token(Identifier) |dataSources|
//@[015:00016) |   |   | ├─Token(Colon) |:|
//@[017:00047) |   |   | └─PropertyAccessSyntax
//@[017:00035) |   |   |   ├─VariableAccessSyntax
//@[017:00035) |   |   |   | └─IdentifierSyntax
//@[017:00035) |   |   |   |   └─Token(Identifier) |dataCollectionRule|
//@[035:00036) |   |   |   ├─Token(Dot) |.|
//@[036:00047) |   |   |   └─IdentifierSyntax
//@[036:00047) |   |   |     └─Token(Identifier) |dataSources|
//@[047:00049) |   |   ├─Token(NewLine) |\r\n|
    dataFlows: dataCollectionRule.dataFlows
//@[004:00043) |   |   ├─ObjectPropertySyntax
//@[004:00013) |   |   | ├─IdentifierSyntax
//@[004:00013) |   |   | | └─Token(Identifier) |dataFlows|
//@[013:00014) |   |   | ├─Token(Colon) |:|
//@[015:00043) |   |   | └─PropertyAccessSyntax
//@[015:00033) |   |   |   ├─VariableAccessSyntax
//@[015:00033) |   |   |   | └─IdentifierSyntax
//@[015:00033) |   |   |   |   └─Token(Identifier) |dataCollectionRule|
//@[033:00034) |   |   |   ├─Token(Dot) |.|
//@[034:00043) |   |   |   └─IdentifierSyntax
//@[034:00043) |   |   |     └─Token(Identifier) |dataFlows|
//@[043:00045) |   |   ├─Token(NewLine) |\r\n|
  }
//@[002:00003) |   |   └─Token(RightBrace) |}|
//@[003:00005) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

resource dataCollectionRuleRes2 'Microsoft.Insights/dataCollectionRules@2021-04-01' = {
//@[000:00445) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00031) | ├─IdentifierSyntax
//@[009:00031) | | └─Token(Identifier) |dataCollectionRuleRes2|
//@[032:00083) | ├─StringSyntax
//@[032:00083) | | └─Token(StringComplete) |'Microsoft.Insights/dataCollectionRules@2021-04-01'|
//@[084:00085) | ├─Token(Assignment) |=|
//@[086:00445) | └─ObjectSyntax
//@[086:00087) |   ├─Token(LeftBrace) |{|
//@[087:00089) |   ├─Token(NewLine) |\r\n|
  name: dataCollectionRule.name
//@[002:00031) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00031) |   | └─PropertyAccessSyntax
//@[008:00026) |   |   ├─VariableAccessSyntax
//@[008:00026) |   |   | └─IdentifierSyntax
//@[008:00026) |   |   |   └─Token(Identifier) |dataCollectionRule|
//@[026:00027) |   |   ├─Token(Dot) |.|
//@[027:00031) |   |   └─IdentifierSyntax
//@[027:00031) |   |     └─Token(Identifier) |name|
//@[031:00033) |   ├─Token(NewLine) |\r\n|
  location: dataCollectionRule.location
//@[002:00039) |   ├─ObjectPropertySyntax
//@[002:00010) |   | ├─IdentifierSyntax
//@[002:00010) |   | | └─Token(Identifier) |location|
//@[010:00011) |   | ├─Token(Colon) |:|
//@[012:00039) |   | └─PropertyAccessSyntax
//@[012:00030) |   |   ├─VariableAccessSyntax
//@[012:00030) |   |   | └─IdentifierSyntax
//@[012:00030) |   |   |   └─Token(Identifier) |dataCollectionRule|
//@[030:00031) |   |   ├─Token(Dot) |.|
//@[031:00039) |   |   └─IdentifierSyntax
//@[031:00039) |   |     └─Token(Identifier) |location|
//@[039:00041) |   ├─Token(NewLine) |\r\n|
  tags: tags
//@[002:00012) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |tags|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00012) |   | └─VariableAccessSyntax
//@[008:00012) |   |   └─IdentifierSyntax
//@[008:00012) |   |     └─Token(Identifier) |tags|
//@[012:00014) |   ├─Token(NewLine) |\r\n|
  kind: dataCollectionRule.kind
//@[002:00031) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |kind|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00031) |   | └─PropertyAccessSyntax
//@[008:00026) |   |   ├─VariableAccessSyntax
//@[008:00026) |   |   | └─IdentifierSyntax
//@[008:00026) |   |   |   └─Token(Identifier) |dataCollectionRule|
//@[026:00027) |   |   ├─Token(Dot) |.|
//@[027:00031) |   |   └─IdentifierSyntax
//@[027:00031) |   |     └─Token(Identifier) |kind|
//@[031:00033) |   ├─Token(NewLine) |\r\n|
  properties: {
//@[002:00232) |   ├─ObjectPropertySyntax
//@[002:00012) |   | ├─IdentifierSyntax
//@[002:00012) |   | | └─Token(Identifier) |properties|
//@[012:00013) |   | ├─Token(Colon) |:|
//@[014:00232) |   | └─ObjectSyntax
//@[014:00015) |   |   ├─Token(LeftBrace) |{|
//@[015:00017) |   |   ├─Token(NewLine) |\r\n|
    description: dataCollectionRule.description
//@[004:00047) |   |   ├─ObjectPropertySyntax
//@[004:00015) |   |   | ├─IdentifierSyntax
//@[004:00015) |   |   | | └─Token(Identifier) |description|
//@[015:00016) |   |   | ├─Token(Colon) |:|
//@[017:00047) |   |   | └─PropertyAccessSyntax
//@[017:00035) |   |   |   ├─VariableAccessSyntax
//@[017:00035) |   |   |   | └─IdentifierSyntax
//@[017:00035) |   |   |   |   └─Token(Identifier) |dataCollectionRule|
//@[035:00036) |   |   |   ├─Token(Dot) |.|
//@[036:00047) |   |   |   └─IdentifierSyntax
//@[036:00047) |   |   |     └─Token(Identifier) |description|
//@[047:00049) |   |   ├─Token(NewLine) |\r\n|
    destinations: empty([]) ? [for x in []: {}] : [for x in []: {}]
//@[004:00067) |   |   ├─ObjectPropertySyntax
//@[004:00016) |   |   | ├─IdentifierSyntax
//@[004:00016) |   |   | | └─Token(Identifier) |destinations|
//@[016:00017) |   |   | ├─Token(Colon) |:|
//@[018:00067) |   |   | └─TernaryOperationSyntax
//@[018:00027) |   |   |   ├─FunctionCallSyntax
//@[018:00023) |   |   |   | ├─IdentifierSyntax
//@[018:00023) |   |   |   | | └─Token(Identifier) |empty|
//@[023:00024) |   |   |   | ├─Token(LeftParen) |(|
//@[024:00026) |   |   |   | ├─FunctionArgumentSyntax
//@[024:00026) |   |   |   | | └─ArraySyntax
//@[024:00025) |   |   |   | |   ├─Token(LeftSquare) |[|
//@[025:00026) |   |   |   | |   └─Token(RightSquare) |]|
//@[026:00027) |   |   |   | └─Token(RightParen) |)|
//@[028:00029) |   |   |   ├─Token(Question) |?|
//@[030:00047) |   |   |   ├─ForSyntax
//@[030:00031) |   |   |   | ├─Token(LeftSquare) |[|
//@[031:00034) |   |   |   | ├─Token(Identifier) |for|
//@[035:00036) |   |   |   | ├─LocalVariableSyntax
//@[035:00036) |   |   |   | | └─IdentifierSyntax
//@[035:00036) |   |   |   | |   └─Token(Identifier) |x|
//@[037:00039) |   |   |   | ├─Token(Identifier) |in|
//@[040:00042) |   |   |   | ├─ArraySyntax
//@[040:00041) |   |   |   | | ├─Token(LeftSquare) |[|
//@[041:00042) |   |   |   | | └─Token(RightSquare) |]|
//@[042:00043) |   |   |   | ├─Token(Colon) |:|
//@[044:00046) |   |   |   | ├─ObjectSyntax
//@[044:00045) |   |   |   | | ├─Token(LeftBrace) |{|
//@[045:00046) |   |   |   | | └─Token(RightBrace) |}|
//@[046:00047) |   |   |   | └─Token(RightSquare) |]|
//@[048:00049) |   |   |   ├─Token(Colon) |:|
//@[050:00067) |   |   |   └─ForSyntax
//@[050:00051) |   |   |     ├─Token(LeftSquare) |[|
//@[051:00054) |   |   |     ├─Token(Identifier) |for|
//@[055:00056) |   |   |     ├─LocalVariableSyntax
//@[055:00056) |   |   |     | └─IdentifierSyntax
//@[055:00056) |   |   |     |   └─Token(Identifier) |x|
//@[057:00059) |   |   |     ├─Token(Identifier) |in|
//@[060:00062) |   |   |     ├─ArraySyntax
//@[060:00061) |   |   |     | ├─Token(LeftSquare) |[|
//@[061:00062) |   |   |     | └─Token(RightSquare) |]|
//@[062:00063) |   |   |     ├─Token(Colon) |:|
//@[064:00066) |   |   |     ├─ObjectSyntax
//@[064:00065) |   |   |     | ├─Token(LeftBrace) |{|
//@[065:00066) |   |   |     | └─Token(RightBrace) |}|
//@[066:00067) |   |   |     └─Token(RightSquare) |]|
//@[067:00069) |   |   ├─Token(NewLine) |\r\n|
    dataSources: dataCollectionRule.dataSources
//@[004:00047) |   |   ├─ObjectPropertySyntax
//@[004:00015) |   |   | ├─IdentifierSyntax
//@[004:00015) |   |   | | └─Token(Identifier) |dataSources|
//@[015:00016) |   |   | ├─Token(Colon) |:|
//@[017:00047) |   |   | └─PropertyAccessSyntax
//@[017:00035) |   |   |   ├─VariableAccessSyntax
//@[017:00035) |   |   |   | └─IdentifierSyntax
//@[017:00035) |   |   |   |   └─Token(Identifier) |dataCollectionRule|
//@[035:00036) |   |   |   ├─Token(Dot) |.|
//@[036:00047) |   |   |   └─IdentifierSyntax
//@[036:00047) |   |   |     └─Token(Identifier) |dataSources|
//@[047:00049) |   |   ├─Token(NewLine) |\r\n|
    dataFlows: dataCollectionRule.dataFlows
//@[004:00043) |   |   ├─ObjectPropertySyntax
//@[004:00013) |   |   | ├─IdentifierSyntax
//@[004:00013) |   |   | | └─Token(Identifier) |dataFlows|
//@[013:00014) |   |   | ├─Token(Colon) |:|
//@[015:00043) |   |   | └─PropertyAccessSyntax
//@[015:00033) |   |   |   ├─VariableAccessSyntax
//@[015:00033) |   |   |   | └─IdentifierSyntax
//@[015:00033) |   |   |   |   └─Token(Identifier) |dataCollectionRule|
//@[033:00034) |   |   |   ├─Token(Dot) |.|
//@[034:00043) |   |   |   └─IdentifierSyntax
//@[034:00043) |   |   |     └─Token(Identifier) |dataFlows|
//@[043:00045) |   |   ├─Token(NewLine) |\r\n|
  }
//@[002:00003) |   |   └─Token(RightBrace) |}|
//@[003:00005) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

@description('The language of the Deployment Script. AzurePowerShell or AzureCLI.')
//@[000:00176) ├─ParameterDeclarationSyntax
//@[000:00083) | ├─DecoratorSyntax
//@[000:00001) | | ├─Token(At) |@|
//@[001:00083) | | └─FunctionCallSyntax
//@[001:00012) | |   ├─IdentifierSyntax
//@[001:00012) | |   | └─Token(Identifier) |description|
//@[012:00013) | |   ├─Token(LeftParen) |(|
//@[013:00082) | |   ├─FunctionArgumentSyntax
//@[013:00082) | |   | └─StringSyntax
//@[013:00082) | |   |   └─Token(StringComplete) |'The language of the Deployment Script. AzurePowerShell or AzureCLI.'|
//@[082:00083) | |   └─Token(RightParen) |)|
//@[083:00085) | ├─Token(NewLine) |\r\n|
@allowed([
//@[000:00049) | ├─DecoratorSyntax
//@[000:00001) | | ├─Token(At) |@|
//@[001:00049) | | └─FunctionCallSyntax
//@[001:00008) | |   ├─IdentifierSyntax
//@[001:00008) | |   | └─Token(Identifier) |allowed|
//@[008:00009) | |   ├─Token(LeftParen) |(|
//@[009:00048) | |   ├─FunctionArgumentSyntax
//@[009:00048) | |   | └─ArraySyntax
//@[009:00010) | |   |   ├─Token(LeftSquare) |[|
//@[010:00012) | |   |   ├─Token(NewLine) |\r\n|
  'AzureCLI'
//@[002:00012) | |   |   ├─ArrayItemSyntax
//@[002:00012) | |   |   | └─StringSyntax
//@[002:00012) | |   |   |   └─Token(StringComplete) |'AzureCLI'|
//@[012:00014) | |   |   ├─Token(NewLine) |\r\n|
  'AzurePowerShell'
//@[002:00019) | |   |   ├─ArrayItemSyntax
//@[002:00019) | |   |   | └─StringSyntax
//@[002:00019) | |   |   |   └─Token(StringComplete) |'AzurePowerShell'|
//@[019:00021) | |   |   ├─Token(NewLine) |\r\n|
])
//@[000:00001) | |   |   └─Token(RightSquare) |]|
//@[001:00002) | |   └─Token(RightParen) |)|
//@[002:00004) | ├─Token(NewLine) |\r\n|
param issue4668_kind string = 'AzureCLI'
//@[000:00005) | ├─Token(Identifier) |param|
//@[006:00020) | ├─IdentifierSyntax
//@[006:00020) | | └─Token(Identifier) |issue4668_kind|
//@[021:00027) | ├─SimpleTypeSyntax
//@[021:00027) | | └─Token(Identifier) |string|
//@[028:00040) | └─ParameterDefaultValueSyntax
//@[028:00029) |   ├─Token(Assignment) |=|
//@[030:00040) |   └─StringSyntax
//@[030:00040) |     └─Token(StringComplete) |'AzureCLI'|
//@[040:00042) ├─Token(NewLine) |\r\n|
@description('The identity that will be used to execute the Deployment Script.')
//@[000:00113) ├─ParameterDeclarationSyntax
//@[000:00080) | ├─DecoratorSyntax
//@[000:00001) | | ├─Token(At) |@|
//@[001:00080) | | └─FunctionCallSyntax
//@[001:00012) | |   ├─IdentifierSyntax
//@[001:00012) | |   | └─Token(Identifier) |description|
//@[012:00013) | |   ├─Token(LeftParen) |(|
//@[013:00079) | |   ├─FunctionArgumentSyntax
//@[013:00079) | |   | └─StringSyntax
//@[013:00079) | |   |   └─Token(StringComplete) |'The identity that will be used to execute the Deployment Script.'|
//@[079:00080) | |   └─Token(RightParen) |)|
//@[080:00082) | ├─Token(NewLine) |\r\n|
param issue4668_identity object
//@[000:00005) | ├─Token(Identifier) |param|
//@[006:00024) | ├─IdentifierSyntax
//@[006:00024) | | └─Token(Identifier) |issue4668_identity|
//@[025:00031) | └─SimpleTypeSyntax
//@[025:00031) |   └─Token(Identifier) |object|
//@[031:00033) ├─Token(NewLine) |\r\n|
@description('The properties of the Deployment Script.')
//@[000:00091) ├─ParameterDeclarationSyntax
//@[000:00056) | ├─DecoratorSyntax
//@[000:00001) | | ├─Token(At) |@|
//@[001:00056) | | └─FunctionCallSyntax
//@[001:00012) | |   ├─IdentifierSyntax
//@[001:00012) | |   | └─Token(Identifier) |description|
//@[012:00013) | |   ├─Token(LeftParen) |(|
//@[013:00055) | |   ├─FunctionArgumentSyntax
//@[013:00055) | |   | └─StringSyntax
//@[013:00055) | |   |   └─Token(StringComplete) |'The properties of the Deployment Script.'|
//@[055:00056) | |   └─Token(RightParen) |)|
//@[056:00058) | ├─Token(NewLine) |\r\n|
param issue4668_properties object
//@[000:00005) | ├─Token(Identifier) |param|
//@[006:00026) | ├─IdentifierSyntax
//@[006:00026) | | └─Token(Identifier) |issue4668_properties|
//@[027:00033) | └─SimpleTypeSyntax
//@[027:00033) |   └─Token(Identifier) |object|
//@[033:00035) ├─Token(NewLine) |\r\n|
resource issue4668_mainResource 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
//@[000:00229) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00031) | ├─IdentifierSyntax
//@[009:00031) | | └─Token(Identifier) |issue4668_mainResource|
//@[032:00082) | ├─StringSyntax
//@[032:00082) | | └─Token(StringComplete) |'Microsoft.Resources/deploymentScripts@2020-10-01'|
//@[083:00084) | ├─Token(Assignment) |=|
//@[085:00229) | └─ObjectSyntax
//@[085:00086) |   ├─Token(LeftBrace) |{|
//@[086:00088) |   ├─Token(NewLine) |\r\n|
  name: 'testscript'
//@[002:00020) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00020) |   | └─StringSyntax
//@[008:00020) |   |   └─Token(StringComplete) |'testscript'|
//@[020:00022) |   ├─Token(NewLine) |\r\n|
  location: 'westeurope'
//@[002:00024) |   ├─ObjectPropertySyntax
//@[002:00010) |   | ├─IdentifierSyntax
//@[002:00010) |   | | └─Token(Identifier) |location|
//@[010:00011) |   | ├─Token(Colon) |:|
//@[012:00024) |   | └─StringSyntax
//@[012:00024) |   |   └─Token(StringComplete) |'westeurope'|
//@[024:00026) |   ├─Token(NewLine) |\r\n|
  kind: issue4668_kind
//@[002:00022) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |kind|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00022) |   | └─VariableAccessSyntax
//@[008:00022) |   |   └─IdentifierSyntax
//@[008:00022) |   |     └─Token(Identifier) |issue4668_kind|
//@[022:00024) |   ├─Token(NewLine) |\r\n|
  identity: issue4668_identity
//@[002:00030) |   ├─ObjectPropertySyntax
//@[002:00010) |   | ├─IdentifierSyntax
//@[002:00010) |   | | └─Token(Identifier) |identity|
//@[010:00011) |   | ├─Token(Colon) |:|
//@[012:00030) |   | └─VariableAccessSyntax
//@[012:00030) |   |   └─IdentifierSyntax
//@[012:00030) |   |     └─Token(Identifier) |issue4668_identity|
//@[030:00032) |   ├─Token(NewLine) |\r\n|
  properties: issue4668_properties
//@[002:00034) |   ├─ObjectPropertySyntax
//@[002:00012) |   | ├─IdentifierSyntax
//@[002:00012) |   | | └─Token(Identifier) |properties|
//@[012:00013) |   | ├─Token(Colon) |:|
//@[014:00034) |   | └─VariableAccessSyntax
//@[014:00034) |   |   └─IdentifierSyntax
//@[014:00034) |   |     └─Token(Identifier) |issue4668_properties|
//@[034:00036) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00003) ├─Token(NewLine) |\r\n|

//@[000:00000) └─Token(EndOfFile) ||
