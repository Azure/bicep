
//@[000:7440) ProgramSyntax
//@[000:0001) ├─Token(NewLine) |\n|
// int
//@[006:0007) ├─Token(NewLine) |\n|
@sys.description('an int variable')
//@[000:0050) ├─VariableDeclarationSyntax
//@[000:0035) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0035) | | └─InstanceFunctionCallSyntax
//@[001:0004) | |   ├─VariableAccessSyntax
//@[001:0004) | |   | └─IdentifierSyntax
//@[001:0004) | |   |   └─Token(Identifier) |sys|
//@[004:0005) | |   ├─Token(Dot) |.|
//@[005:0016) | |   ├─IdentifierSyntax
//@[005:0016) | |   | └─Token(Identifier) |description|
//@[016:0017) | |   ├─Token(LeftParen) |(|
//@[017:0034) | |   ├─FunctionArgumentSyntax
//@[017:0034) | |   | └─StringSyntax
//@[017:0034) | |   |   └─Token(StringComplete) |'an int variable'|
//@[034:0035) | |   └─Token(RightParen) |)|
//@[035:0036) | ├─Token(NewLine) |\n|
var myInt = 42
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0009) | ├─IdentifierSyntax
//@[004:0009) | | └─Token(Identifier) |myInt|
//@[010:0011) | ├─Token(Assignment) |=|
//@[012:0014) | └─IntegerLiteralSyntax
//@[012:0014) |   └─Token(Integer) |42|
//@[014:0016) ├─Token(NewLine) |\n\n|

// string
//@[009:0010) ├─Token(NewLine) |\n|
@sys.description('a string variable')
//@[000:0055) ├─VariableDeclarationSyntax
//@[000:0037) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0037) | | └─InstanceFunctionCallSyntax
//@[001:0004) | |   ├─VariableAccessSyntax
//@[001:0004) | |   | └─IdentifierSyntax
//@[001:0004) | |   |   └─Token(Identifier) |sys|
//@[004:0005) | |   ├─Token(Dot) |.|
//@[005:0016) | |   ├─IdentifierSyntax
//@[005:0016) | |   | └─Token(Identifier) |description|
//@[016:0017) | |   ├─Token(LeftParen) |(|
//@[017:0036) | |   ├─FunctionArgumentSyntax
//@[017:0036) | |   | └─StringSyntax
//@[017:0036) | |   |   └─Token(StringComplete) |'a string variable'|
//@[036:0037) | |   └─Token(RightParen) |)|
//@[037:0038) | ├─Token(NewLine) |\n|
var myStr = 'str'
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0009) | ├─IdentifierSyntax
//@[004:0009) | | └─Token(Identifier) |myStr|
//@[010:0011) | ├─Token(Assignment) |=|
//@[012:0017) | └─StringSyntax
//@[012:0017) |   └─Token(StringComplete) |'str'|
//@[017:0018) ├─Token(NewLine) |\n|
var curliesWithNoInterp = '}{1}{'
//@[000:0033) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0023) | ├─IdentifierSyntax
//@[004:0023) | | └─Token(Identifier) |curliesWithNoInterp|
//@[024:0025) | ├─Token(Assignment) |=|
//@[026:0033) | └─StringSyntax
//@[026:0033) |   └─Token(StringComplete) |'}{1}{'|
//@[033:0034) ├─Token(NewLine) |\n|
var interp1 = 'abc${123}def'
//@[000:0028) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0011) | ├─IdentifierSyntax
//@[004:0011) | | └─Token(Identifier) |interp1|
//@[012:0013) | ├─Token(Assignment) |=|
//@[014:0028) | └─StringSyntax
//@[014:0020) |   ├─Token(StringLeftPiece) |'abc${|
//@[020:0023) |   ├─IntegerLiteralSyntax
//@[020:0023) |   | └─Token(Integer) |123|
//@[023:0028) |   └─Token(StringRightPiece) |}def'|
//@[028:0029) ├─Token(NewLine) |\n|
var interp2 = '${123}def'
//@[000:0025) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0011) | ├─IdentifierSyntax
//@[004:0011) | | └─Token(Identifier) |interp2|
//@[012:0013) | ├─Token(Assignment) |=|
//@[014:0025) | └─StringSyntax
//@[014:0017) |   ├─Token(StringLeftPiece) |'${|
//@[017:0020) |   ├─IntegerLiteralSyntax
//@[017:0020) |   | └─Token(Integer) |123|
//@[020:0025) |   └─Token(StringRightPiece) |}def'|
//@[025:0026) ├─Token(NewLine) |\n|
var interp3 = 'abc${123}'
//@[000:0025) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0011) | ├─IdentifierSyntax
//@[004:0011) | | └─Token(Identifier) |interp3|
//@[012:0013) | ├─Token(Assignment) |=|
//@[014:0025) | └─StringSyntax
//@[014:0020) |   ├─Token(StringLeftPiece) |'abc${|
//@[020:0023) |   ├─IntegerLiteralSyntax
//@[020:0023) |   | └─Token(Integer) |123|
//@[023:0025) |   └─Token(StringRightPiece) |}'|
//@[025:0026) ├─Token(NewLine) |\n|
var interp4 = 'abc${123}${456}jk$l${789}p$'
//@[000:0043) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0011) | ├─IdentifierSyntax
//@[004:0011) | | └─Token(Identifier) |interp4|
//@[012:0013) | ├─Token(Assignment) |=|
//@[014:0043) | └─StringSyntax
//@[014:0020) |   ├─Token(StringLeftPiece) |'abc${|
//@[020:0023) |   ├─IntegerLiteralSyntax
//@[020:0023) |   | └─Token(Integer) |123|
//@[023:0026) |   ├─Token(StringMiddlePiece) |}${|
//@[026:0029) |   ├─IntegerLiteralSyntax
//@[026:0029) |   | └─Token(Integer) |456|
//@[029:0036) |   ├─Token(StringMiddlePiece) |}jk$l${|
//@[036:0039) |   ├─IntegerLiteralSyntax
//@[036:0039) |   | └─Token(Integer) |789|
//@[039:0043) |   └─Token(StringRightPiece) |}p$'|
//@[043:0044) ├─Token(NewLine) |\n|
var doubleInterp = 'abc${'def${123}'}_${'${456}${789}'}'
//@[000:0056) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0016) | ├─IdentifierSyntax
//@[004:0016) | | └─Token(Identifier) |doubleInterp|
//@[017:0018) | ├─Token(Assignment) |=|
//@[019:0056) | └─StringSyntax
//@[019:0025) |   ├─Token(StringLeftPiece) |'abc${|
//@[025:0036) |   ├─StringSyntax
//@[025:0031) |   | ├─Token(StringLeftPiece) |'def${|
//@[031:0034) |   | ├─IntegerLiteralSyntax
//@[031:0034) |   | | └─Token(Integer) |123|
//@[034:0036) |   | └─Token(StringRightPiece) |}'|
//@[036:0040) |   ├─Token(StringMiddlePiece) |}_${|
//@[040:0054) |   ├─StringSyntax
//@[040:0043) |   | ├─Token(StringLeftPiece) |'${|
//@[043:0046) |   | ├─IntegerLiteralSyntax
//@[043:0046) |   | | └─Token(Integer) |456|
//@[046:0049) |   | ├─Token(StringMiddlePiece) |}${|
//@[049:0052) |   | ├─IntegerLiteralSyntax
//@[049:0052) |   | | └─Token(Integer) |789|
//@[052:0054) |   | └─Token(StringRightPiece) |}'|
//@[054:0056) |   └─Token(StringRightPiece) |}'|
//@[056:0057) ├─Token(NewLine) |\n|
var curliesInInterp = '{${123}{0}${true}}'
//@[000:0042) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0019) | ├─IdentifierSyntax
//@[004:0019) | | └─Token(Identifier) |curliesInInterp|
//@[020:0021) | ├─Token(Assignment) |=|
//@[022:0042) | └─StringSyntax
//@[022:0026) |   ├─Token(StringLeftPiece) |'{${|
//@[026:0029) |   ├─IntegerLiteralSyntax
//@[026:0029) |   | └─Token(Integer) |123|
//@[029:0035) |   ├─Token(StringMiddlePiece) |}{0}${|
//@[035:0039) |   ├─BooleanLiteralSyntax
//@[035:0039) |   | └─Token(TrueKeyword) |true|
//@[039:0042) |   └─Token(StringRightPiece) |}}'|
//@[042:0044) ├─Token(NewLine) |\n\n|

// #completionTest(0) -> declarations
//@[037:0039) ├─Token(NewLine) |\n\n|

// verify correct bracket escaping
//@[034:0035) ├─Token(NewLine) |\n|
var bracketInTheMiddle = 'a[b]'
//@[000:0031) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0022) | ├─IdentifierSyntax
//@[004:0022) | | └─Token(Identifier) |bracketInTheMiddle|
//@[023:0024) | ├─Token(Assignment) |=|
//@[025:0031) | └─StringSyntax
//@[025:0031) |   └─Token(StringComplete) |'a[b]'|
//@[031:0032) ├─Token(NewLine) |\n|
// #completionTest(25) -> empty
//@[031:0032) ├─Token(NewLine) |\n|
var bracketAtBeginning = '[test'
//@[000:0032) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0022) | ├─IdentifierSyntax
//@[004:0022) | | └─Token(Identifier) |bracketAtBeginning|
//@[023:0024) | ├─Token(Assignment) |=|
//@[025:0032) | └─StringSyntax
//@[025:0032) |   └─Token(StringComplete) |'[test'|
//@[032:0033) ├─Token(NewLine) |\n|
// #completionTest(23) -> symbolsPlusTypes
//@[042:0043) ├─Token(NewLine) |\n|
var enclosingBrackets = '[test]'
//@[000:0032) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0021) | ├─IdentifierSyntax
//@[004:0021) | | └─Token(Identifier) |enclosingBrackets|
//@[022:0023) | ├─Token(Assignment) |=|
//@[024:0032) | └─StringSyntax
//@[024:0032) |   └─Token(StringComplete) |'[test]'|
//@[032:0033) ├─Token(NewLine) |\n|
var emptyJsonArray = '[]'
//@[000:0025) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0018) | ├─IdentifierSyntax
//@[004:0018) | | └─Token(Identifier) |emptyJsonArray|
//@[019:0020) | ├─Token(Assignment) |=|
//@[021:0025) | └─StringSyntax
//@[021:0025) |   └─Token(StringComplete) |'[]'|
//@[025:0026) ├─Token(NewLine) |\n|
var interpolatedBrackets = '[${myInt}]'
//@[000:0039) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0024) | ├─IdentifierSyntax
//@[004:0024) | | └─Token(Identifier) |interpolatedBrackets|
//@[025:0026) | ├─Token(Assignment) |=|
//@[027:0039) | └─StringSyntax
//@[027:0031) |   ├─Token(StringLeftPiece) |'[${|
//@[031:0036) |   ├─VariableAccessSyntax
//@[031:0036) |   | └─IdentifierSyntax
//@[031:0036) |   |   └─Token(Identifier) |myInt|
//@[036:0039) |   └─Token(StringRightPiece) |}]'|
//@[039:0040) ├─Token(NewLine) |\n|
var nestedBrackets = '[test[]test2]'
//@[000:0036) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0018) | ├─IdentifierSyntax
//@[004:0018) | | └─Token(Identifier) |nestedBrackets|
//@[019:0020) | ├─Token(Assignment) |=|
//@[021:0036) | └─StringSyntax
//@[021:0036) |   └─Token(StringComplete) |'[test[]test2]'|
//@[036:0037) ├─Token(NewLine) |\n|
var nestedInterpolatedBrackets = '[${emptyJsonArray}]'
//@[000:0054) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0030) | ├─IdentifierSyntax
//@[004:0030) | | └─Token(Identifier) |nestedInterpolatedBrackets|
//@[031:0032) | ├─Token(Assignment) |=|
//@[033:0054) | └─StringSyntax
//@[033:0037) |   ├─Token(StringLeftPiece) |'[${|
//@[037:0051) |   ├─VariableAccessSyntax
//@[037:0051) |   | └─IdentifierSyntax
//@[037:0051) |   |   └─Token(Identifier) |emptyJsonArray|
//@[051:0054) |   └─Token(StringRightPiece) |}]'|
//@[054:0055) ├─Token(NewLine) |\n|
var bracketStringInExpression = concat('[', '\'test\'',']')
//@[000:0059) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0029) | ├─IdentifierSyntax
//@[004:0029) | | └─Token(Identifier) |bracketStringInExpression|
//@[030:0031) | ├─Token(Assignment) |=|
//@[032:0059) | └─FunctionCallSyntax
//@[032:0038) |   ├─IdentifierSyntax
//@[032:0038) |   | └─Token(Identifier) |concat|
//@[038:0039) |   ├─Token(LeftParen) |(|
//@[039:0042) |   ├─FunctionArgumentSyntax
//@[039:0042) |   | └─StringSyntax
//@[039:0042) |   |   └─Token(StringComplete) |'['|
//@[042:0043) |   ├─Token(Comma) |,|
//@[044:0054) |   ├─FunctionArgumentSyntax
//@[044:0054) |   | └─StringSyntax
//@[044:0054) |   |   └─Token(StringComplete) |'\'test\''|
//@[054:0055) |   ├─Token(Comma) |,|
//@[055:0058) |   ├─FunctionArgumentSyntax
//@[055:0058) |   | └─StringSyntax
//@[055:0058) |   |   └─Token(StringComplete) |']'|
//@[058:0059) |   └─Token(RightParen) |)|
//@[059:0061) ├─Token(NewLine) |\n\n|

// booleans
//@[011:0012) ├─Token(NewLine) |\n|
@sys.description('a bool variable')
//@[000:0054) ├─VariableDeclarationSyntax
//@[000:0035) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0035) | | └─InstanceFunctionCallSyntax
//@[001:0004) | |   ├─VariableAccessSyntax
//@[001:0004) | |   | └─IdentifierSyntax
//@[001:0004) | |   |   └─Token(Identifier) |sys|
//@[004:0005) | |   ├─Token(Dot) |.|
//@[005:0016) | |   ├─IdentifierSyntax
//@[005:0016) | |   | └─Token(Identifier) |description|
//@[016:0017) | |   ├─Token(LeftParen) |(|
//@[017:0034) | |   ├─FunctionArgumentSyntax
//@[017:0034) | |   | └─StringSyntax
//@[017:0034) | |   |   └─Token(StringComplete) |'a bool variable'|
//@[034:0035) | |   └─Token(RightParen) |)|
//@[035:0036) | ├─Token(NewLine) |\n|
var myTruth = true
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0011) | ├─IdentifierSyntax
//@[004:0011) | | └─Token(Identifier) |myTruth|
//@[012:0013) | ├─Token(Assignment) |=|
//@[014:0018) | └─BooleanLiteralSyntax
//@[014:0018) |   └─Token(TrueKeyword) |true|
//@[018:0019) ├─Token(NewLine) |\n|
var myFalsehood = false
//@[000:0023) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0015) | ├─IdentifierSyntax
//@[004:0015) | | └─Token(Identifier) |myFalsehood|
//@[016:0017) | ├─Token(Assignment) |=|
//@[018:0023) | └─BooleanLiteralSyntax
//@[018:0023) |   └─Token(FalseKeyword) |false|
//@[023:0025) ├─Token(NewLine) |\n\n|

var myEmptyObj = { }
//@[000:0020) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0014) | ├─IdentifierSyntax
//@[004:0014) | | └─Token(Identifier) |myEmptyObj|
//@[015:0016) | ├─Token(Assignment) |=|
//@[017:0020) | └─ObjectSyntax
//@[017:0018) |   ├─Token(LeftBrace) |{|
//@[019:0020) |   └─Token(RightBrace) |}|
//@[020:0021) ├─Token(NewLine) |\n|
var myEmptyArray = [ ]
//@[000:0022) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0016) | ├─IdentifierSyntax
//@[004:0016) | | └─Token(Identifier) |myEmptyArray|
//@[017:0018) | ├─Token(Assignment) |=|
//@[019:0022) | └─ArraySyntax
//@[019:0020) |   ├─Token(LeftSquare) |[|
//@[021:0022) |   └─Token(RightSquare) |]|
//@[022:0024) ├─Token(NewLine) |\n\n|

// object
//@[009:0010) ├─Token(NewLine) |\n|
@sys.description('a object variable')
//@[000:0242) ├─VariableDeclarationSyntax
//@[000:0037) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0037) | | └─InstanceFunctionCallSyntax
//@[001:0004) | |   ├─VariableAccessSyntax
//@[001:0004) | |   | └─IdentifierSyntax
//@[001:0004) | |   |   └─Token(Identifier) |sys|
//@[004:0005) | |   ├─Token(Dot) |.|
//@[005:0016) | |   ├─IdentifierSyntax
//@[005:0016) | |   | └─Token(Identifier) |description|
//@[016:0017) | |   ├─Token(LeftParen) |(|
//@[017:0036) | |   ├─FunctionArgumentSyntax
//@[017:0036) | |   | └─StringSyntax
//@[017:0036) | |   |   └─Token(StringComplete) |'a object variable'|
//@[036:0037) | |   └─Token(RightParen) |)|
//@[037:0038) | ├─Token(NewLine) |\n|
var myObj = {
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0009) | ├─IdentifierSyntax
//@[004:0009) | | └─Token(Identifier) |myObj|
//@[010:0011) | ├─Token(Assignment) |=|
//@[012:0204) | └─ObjectSyntax
//@[012:0013) |   ├─Token(LeftBrace) |{|
//@[013:0014) |   ├─Token(NewLine) |\n|
  a: 'a'
//@[002:0008) |   ├─ObjectPropertySyntax
//@[002:0003) |   | ├─IdentifierSyntax
//@[002:0003) |   | | └─Token(Identifier) |a|
//@[003:0004) |   | ├─Token(Colon) |:|
//@[005:0008) |   | └─StringSyntax
//@[005:0008) |   |   └─Token(StringComplete) |'a'|
//@[008:0009) |   ├─Token(NewLine) |\n|
  b: -12
//@[002:0008) |   ├─ObjectPropertySyntax
//@[002:0003) |   | ├─IdentifierSyntax
//@[002:0003) |   | | └─Token(Identifier) |b|
//@[003:0004) |   | ├─Token(Colon) |:|
//@[005:0008) |   | └─UnaryOperationSyntax
//@[005:0006) |   |   ├─Token(Minus) |-|
//@[006:0008) |   |   └─IntegerLiteralSyntax
//@[006:0008) |   |     └─Token(Integer) |12|
//@[008:0009) |   ├─Token(NewLine) |\n|
  c: true
//@[002:0009) |   ├─ObjectPropertySyntax
//@[002:0003) |   | ├─IdentifierSyntax
//@[002:0003) |   | | └─Token(Identifier) |c|
//@[003:0004) |   | ├─Token(Colon) |:|
//@[005:0009) |   | └─BooleanLiteralSyntax
//@[005:0009) |   |   └─Token(TrueKeyword) |true|
//@[009:0010) |   ├─Token(NewLine) |\n|
  d: !true
//@[002:0010) |   ├─ObjectPropertySyntax
//@[002:0003) |   | ├─IdentifierSyntax
//@[002:0003) |   | | └─Token(Identifier) |d|
//@[003:0004) |   | ├─Token(Colon) |:|
//@[005:0010) |   | └─UnaryOperationSyntax
//@[005:0006) |   |   ├─Token(Exclamation) |!|
//@[006:0010) |   |   └─BooleanLiteralSyntax
//@[006:0010) |   |     └─Token(TrueKeyword) |true|
//@[010:0011) |   ├─Token(NewLine) |\n|
  list: [
//@[002:0102) |   ├─ObjectPropertySyntax
//@[002:0006) |   | ├─IdentifierSyntax
//@[002:0006) |   | | └─Token(Identifier) |list|
//@[006:0007) |   | ├─Token(Colon) |:|
//@[008:0102) |   | └─ArraySyntax
//@[008:0009) |   |   ├─Token(LeftSquare) |[|
//@[009:0010) |   |   ├─Token(NewLine) |\n|
    1
//@[004:0005) |   |   ├─ArrayItemSyntax
//@[004:0005) |   |   | └─IntegerLiteralSyntax
//@[004:0005) |   |   |   └─Token(Integer) |1|
//@[005:0006) |   |   ├─Token(NewLine) |\n|
    2
//@[004:0005) |   |   ├─ArrayItemSyntax
//@[004:0005) |   |   | └─IntegerLiteralSyntax
//@[004:0005) |   |   |   └─Token(Integer) |2|
//@[005:0006) |   |   ├─Token(NewLine) |\n|
    2+1
//@[004:0007) |   |   ├─ArrayItemSyntax
//@[004:0007) |   |   | └─BinaryOperationSyntax
//@[004:0005) |   |   |   ├─IntegerLiteralSyntax
//@[004:0005) |   |   |   | └─Token(Integer) |2|
//@[005:0006) |   |   |   ├─Token(Plus) |+|
//@[006:0007) |   |   |   └─IntegerLiteralSyntax
//@[006:0007) |   |   |     └─Token(Integer) |1|
//@[007:0008) |   |   ├─Token(NewLine) |\n|
    {
//@[004:0053) |   |   ├─ArrayItemSyntax
//@[004:0053) |   |   | └─ObjectSyntax
//@[004:0005) |   |   |   ├─Token(LeftBrace) |{|
//@[005:0006) |   |   |   ├─Token(NewLine) |\n|
      test: 144 > 33 && true || 99 <= 199
//@[006:0041) |   |   |   ├─ObjectPropertySyntax
//@[006:0010) |   |   |   | ├─IdentifierSyntax
//@[006:0010) |   |   |   | | └─Token(Identifier) |test|
//@[010:0011) |   |   |   | ├─Token(Colon) |:|
//@[012:0041) |   |   |   | └─BinaryOperationSyntax
//@[012:0028) |   |   |   |   ├─BinaryOperationSyntax
//@[012:0020) |   |   |   |   | ├─BinaryOperationSyntax
//@[012:0015) |   |   |   |   | | ├─IntegerLiteralSyntax
//@[012:0015) |   |   |   |   | | | └─Token(Integer) |144|
//@[016:0017) |   |   |   |   | | ├─Token(GreaterThan) |>|
//@[018:0020) |   |   |   |   | | └─IntegerLiteralSyntax
//@[018:0020) |   |   |   |   | |   └─Token(Integer) |33|
//@[021:0023) |   |   |   |   | ├─Token(LogicalAnd) |&&|
//@[024:0028) |   |   |   |   | └─BooleanLiteralSyntax
//@[024:0028) |   |   |   |   |   └─Token(TrueKeyword) |true|
//@[029:0031) |   |   |   |   ├─Token(LogicalOr) ||||
//@[032:0041) |   |   |   |   └─BinaryOperationSyntax
//@[032:0034) |   |   |   |     ├─IntegerLiteralSyntax
//@[032:0034) |   |   |   |     | └─Token(Integer) |99|
//@[035:0037) |   |   |   |     ├─Token(LessThanOrEqual) |<=|
//@[038:0041) |   |   |   |     └─IntegerLiteralSyntax
//@[038:0041) |   |   |   |       └─Token(Integer) |199|
//@[041:0042) |   |   |   ├─Token(NewLine) |\n|
    }
//@[004:0005) |   |   |   └─Token(RightBrace) |}|
//@[005:0006) |   |   ├─Token(NewLine) |\n|
    'a' =~ 'b'
//@[004:0014) |   |   ├─ArrayItemSyntax
//@[004:0014) |   |   | └─BinaryOperationSyntax
//@[004:0007) |   |   |   ├─StringSyntax
//@[004:0007) |   |   |   | └─Token(StringComplete) |'a'|
//@[008:0010) |   |   |   ├─Token(EqualsInsensitive) |=~|
//@[011:0014) |   |   |   └─StringSyntax
//@[011:0014) |   |   |     └─Token(StringComplete) |'b'|
//@[014:0015) |   |   ├─Token(NewLine) |\n|
  ]
//@[002:0003) |   |   └─Token(RightSquare) |]|
//@[003:0004) |   ├─Token(NewLine) |\n|
  obj: {
//@[002:0046) |   ├─ObjectPropertySyntax
//@[002:0005) |   | ├─IdentifierSyntax
//@[002:0005) |   | | └─Token(Identifier) |obj|
//@[005:0006) |   | ├─Token(Colon) |:|
//@[007:0046) |   | └─ObjectSyntax
//@[007:0008) |   |   ├─Token(LeftBrace) |{|
//@[008:0009) |   |   ├─Token(NewLine) |\n|
    nested: [
//@[004:0033) |   |   ├─ObjectPropertySyntax
//@[004:0010) |   |   | ├─IdentifierSyntax
//@[004:0010) |   |   | | └─Token(Identifier) |nested|
//@[010:0011) |   |   | ├─Token(Colon) |:|
//@[012:0033) |   |   | └─ArraySyntax
//@[012:0013) |   |   |   ├─Token(LeftSquare) |[|
//@[013:0014) |   |   |   ├─Token(NewLine) |\n|
      'hello'
//@[006:0013) |   |   |   ├─ArrayItemSyntax
//@[006:0013) |   |   |   | └─StringSyntax
//@[006:0013) |   |   |   |   └─Token(StringComplete) |'hello'|
//@[013:0014) |   |   |   ├─Token(NewLine) |\n|
    ]
//@[004:0005) |   |   |   └─Token(RightSquare) |]|
//@[005:0006) |   |   ├─Token(NewLine) |\n|
  }
//@[002:0003) |   |   └─Token(RightBrace) |}|
//@[003:0004) |   ├─Token(NewLine) |\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0003) ├─Token(NewLine) |\n\n|

@sys.description('a object with interp')
//@[000:0157) ├─VariableDeclarationSyntax
//@[000:0040) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0040) | | └─InstanceFunctionCallSyntax
//@[001:0004) | |   ├─VariableAccessSyntax
//@[001:0004) | |   | └─IdentifierSyntax
//@[001:0004) | |   |   └─Token(Identifier) |sys|
//@[004:0005) | |   ├─Token(Dot) |.|
//@[005:0016) | |   ├─IdentifierSyntax
//@[005:0016) | |   | └─Token(Identifier) |description|
//@[016:0017) | |   ├─Token(LeftParen) |(|
//@[017:0039) | |   ├─FunctionArgumentSyntax
//@[017:0039) | |   | └─StringSyntax
//@[017:0039) | |   |   └─Token(StringComplete) |'a object with interp'|
//@[039:0040) | |   └─Token(RightParen) |)|
//@[040:0041) | ├─Token(NewLine) |\n|
var objWithInterp = {
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0017) | ├─IdentifierSyntax
//@[004:0017) | | └─Token(Identifier) |objWithInterp|
//@[018:0019) | ├─Token(Assignment) |=|
//@[020:0116) | └─ObjectSyntax
//@[020:0021) |   ├─Token(LeftBrace) |{|
//@[021:0022) |   ├─Token(NewLine) |\n|
  '${myStr}': 1
//@[002:0015) |   ├─ObjectPropertySyntax
//@[002:0012) |   | ├─StringSyntax
//@[002:0005) |   | | ├─Token(StringLeftPiece) |'${|
//@[005:0010) |   | | ├─VariableAccessSyntax
//@[005:0010) |   | | | └─IdentifierSyntax
//@[005:0010) |   | | |   └─Token(Identifier) |myStr|
//@[010:0012) |   | | └─Token(StringRightPiece) |}'|
//@[012:0013) |   | ├─Token(Colon) |:|
//@[014:0015) |   | └─IntegerLiteralSyntax
//@[014:0015) |   |   └─Token(Integer) |1|
//@[015:0016) |   ├─Token(NewLine) |\n|
  'abc${myStr}def': 2
//@[002:0021) |   ├─ObjectPropertySyntax
//@[002:0018) |   | ├─StringSyntax
//@[002:0008) |   | | ├─Token(StringLeftPiece) |'abc${|
//@[008:0013) |   | | ├─VariableAccessSyntax
//@[008:0013) |   | | | └─IdentifierSyntax
//@[008:0013) |   | | |   └─Token(Identifier) |myStr|
//@[013:0018) |   | | └─Token(StringRightPiece) |}def'|
//@[018:0019) |   | ├─Token(Colon) |:|
//@[020:0021) |   | └─IntegerLiteralSyntax
//@[020:0021) |   |   └─Token(Integer) |2|
//@[021:0022) |   ├─Token(NewLine) |\n|
  '${interp1}abc${interp2}': '${interp1}abc${interp2}'
//@[002:0054) |   ├─ObjectPropertySyntax
//@[002:0027) |   | ├─StringSyntax
//@[002:0005) |   | | ├─Token(StringLeftPiece) |'${|
//@[005:0012) |   | | ├─VariableAccessSyntax
//@[005:0012) |   | | | └─IdentifierSyntax
//@[005:0012) |   | | |   └─Token(Identifier) |interp1|
//@[012:0018) |   | | ├─Token(StringMiddlePiece) |}abc${|
//@[018:0025) |   | | ├─VariableAccessSyntax
//@[018:0025) |   | | | └─IdentifierSyntax
//@[018:0025) |   | | |   └─Token(Identifier) |interp2|
//@[025:0027) |   | | └─Token(StringRightPiece) |}'|
//@[027:0028) |   | ├─Token(Colon) |:|
//@[029:0054) |   | └─StringSyntax
//@[029:0032) |   |   ├─Token(StringLeftPiece) |'${|
//@[032:0039) |   |   ├─VariableAccessSyntax
//@[032:0039) |   |   | └─IdentifierSyntax
//@[032:0039) |   |   |   └─Token(Identifier) |interp1|
//@[039:0045) |   |   ├─Token(StringMiddlePiece) |}abc${|
//@[045:0052) |   |   ├─VariableAccessSyntax
//@[045:0052) |   |   | └─IdentifierSyntax
//@[045:0052) |   |   |   └─Token(Identifier) |interp2|
//@[052:0054) |   |   └─Token(StringRightPiece) |}'|
//@[054:0055) |   ├─Token(NewLine) |\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0003) ├─Token(NewLine) |\n\n|

// array
//@[008:0009) ├─Token(NewLine) |\n|
var myArr = [
//@[000:0043) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0009) | ├─IdentifierSyntax
//@[004:0009) | | └─Token(Identifier) |myArr|
//@[010:0011) | ├─Token(Assignment) |=|
//@[012:0043) | └─ArraySyntax
//@[012:0013) |   ├─Token(LeftSquare) |[|
//@[013:0014) |   ├─Token(NewLine) |\n|
  'pirates'
//@[002:0011) |   ├─ArrayItemSyntax
//@[002:0011) |   | └─StringSyntax
//@[002:0011) |   |   └─Token(StringComplete) |'pirates'|
//@[011:0012) |   ├─Token(NewLine) |\n|
  'say'
//@[002:0007) |   ├─ArrayItemSyntax
//@[002:0007) |   | └─StringSyntax
//@[002:0007) |   |   └─Token(StringComplete) |'say'|
//@[007:0008) |   ├─Token(NewLine) |\n|
  'arr'
//@[002:0007) |   ├─ArrayItemSyntax
//@[002:0007) |   | └─StringSyntax
//@[002:0007) |   |   └─Token(StringComplete) |'arr'|
//@[007:0008) |   ├─Token(NewLine) |\n|
]
//@[000:0001) |   └─Token(RightSquare) |]|
//@[001:0003) ├─Token(NewLine) |\n\n|

// array with objects
//@[021:0022) ├─Token(NewLine) |\n|
var myArrWithObjects = [
//@[000:0138) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0020) | ├─IdentifierSyntax
//@[004:0020) | | └─Token(Identifier) |myArrWithObjects|
//@[021:0022) | ├─Token(Assignment) |=|
//@[023:0138) | └─ArraySyntax
//@[023:0024) |   ├─Token(LeftSquare) |[|
//@[024:0025) |   ├─Token(NewLine) |\n|
  {
//@[002:0040) |   ├─ArrayItemSyntax
//@[002:0040) |   | └─ObjectSyntax
//@[002:0003) |   |   ├─Token(LeftBrace) |{|
//@[003:0004) |   |   ├─Token(NewLine) |\n|
    name: 'one'
//@[004:0015) |   |   ├─ObjectPropertySyntax
//@[004:0008) |   |   | ├─IdentifierSyntax
//@[004:0008) |   |   | | └─Token(Identifier) |name|
//@[008:0009) |   |   | ├─Token(Colon) |:|
//@[010:0015) |   |   | └─StringSyntax
//@[010:0015) |   |   |   └─Token(StringComplete) |'one'|
//@[015:0016) |   |   ├─Token(NewLine) |\n|
    enable: true
//@[004:0016) |   |   ├─ObjectPropertySyntax
//@[004:0010) |   |   | ├─IdentifierSyntax
//@[004:0010) |   |   | | └─Token(Identifier) |enable|
//@[010:0011) |   |   | ├─Token(Colon) |:|
//@[012:0016) |   |   | └─BooleanLiteralSyntax
//@[012:0016) |   |   |   └─Token(TrueKeyword) |true|
//@[016:0017) |   |   ├─Token(NewLine) |\n|
  }
//@[002:0003) |   |   └─Token(RightBrace) |}|
//@[003:0004) |   ├─Token(NewLine) |\n|
  {
//@[002:0070) |   ├─ArrayItemSyntax
//@[002:0070) |   | └─ObjectSyntax
//@[002:0003) |   |   ├─Token(LeftBrace) |{|
//@[003:0004) |   |   ├─Token(NewLine) |\n|
    name: 'two'
//@[004:0015) |   |   ├─ObjectPropertySyntax
//@[004:0008) |   |   | ├─IdentifierSyntax
//@[004:0008) |   |   | | └─Token(Identifier) |name|
//@[008:0009) |   |   | ├─Token(Colon) |:|
//@[010:0015) |   |   | └─StringSyntax
//@[010:0015) |   |   |   └─Token(StringComplete) |'two'|
//@[015:0016) |   |   ├─Token(NewLine) |\n|
    enable: false && false || 'two' !~ 'three'
//@[004:0046) |   |   ├─ObjectPropertySyntax
//@[004:0010) |   |   | ├─IdentifierSyntax
//@[004:0010) |   |   | | └─Token(Identifier) |enable|
//@[010:0011) |   |   | ├─Token(Colon) |:|
//@[012:0046) |   |   | └─BinaryOperationSyntax
//@[012:0026) |   |   |   ├─BinaryOperationSyntax
//@[012:0017) |   |   |   | ├─BooleanLiteralSyntax
//@[012:0017) |   |   |   | | └─Token(FalseKeyword) |false|
//@[018:0020) |   |   |   | ├─Token(LogicalAnd) |&&|
//@[021:0026) |   |   |   | └─BooleanLiteralSyntax
//@[021:0026) |   |   |   |   └─Token(FalseKeyword) |false|
//@[027:0029) |   |   |   ├─Token(LogicalOr) ||||
//@[030:0046) |   |   |   └─BinaryOperationSyntax
//@[030:0035) |   |   |     ├─StringSyntax
//@[030:0035) |   |   |     | └─Token(StringComplete) |'two'|
//@[036:0038) |   |   |     ├─Token(NotEqualsInsensitive) |!~|
//@[039:0046) |   |   |     └─StringSyntax
//@[039:0046) |   |   |       └─Token(StringComplete) |'three'|
//@[046:0047) |   |   ├─Token(NewLine) |\n|
  }
//@[002:0003) |   |   └─Token(RightBrace) |}|
//@[003:0004) |   ├─Token(NewLine) |\n|
]
//@[000:0001) |   └─Token(RightSquare) |]|
//@[001:0003) ├─Token(NewLine) |\n\n|

var expressionIndexOnAny = any({
//@[000:0064) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0024) | ├─IdentifierSyntax
//@[004:0024) | | └─Token(Identifier) |expressionIndexOnAny|
//@[025:0026) | ├─Token(Assignment) |=|
//@[027:0064) | └─ArrayAccessSyntax
//@[027:0035) |   ├─FunctionCallSyntax
//@[027:0030) |   | ├─IdentifierSyntax
//@[027:0030) |   | | └─Token(Identifier) |any|
//@[030:0031) |   | ├─Token(LeftParen) |(|
//@[031:0034) |   | ├─FunctionArgumentSyntax
//@[031:0034) |   | | └─ObjectSyntax
//@[031:0032) |   | |   ├─Token(LeftBrace) |{|
//@[032:0033) |   | |   ├─Token(NewLine) |\n|
})[az.resourceGroup().location]
//@[000:0001) |   | |   └─Token(RightBrace) |}|
//@[001:0002) |   | └─Token(RightParen) |)|
//@[002:0003) |   ├─Token(LeftSquare) |[|
//@[003:0030) |   ├─PropertyAccessSyntax
//@[003:0021) |   | ├─InstanceFunctionCallSyntax
//@[003:0005) |   | | ├─VariableAccessSyntax
//@[003:0005) |   | | | └─IdentifierSyntax
//@[003:0005) |   | | |   └─Token(Identifier) |az|
//@[005:0006) |   | | ├─Token(Dot) |.|
//@[006:0019) |   | | ├─IdentifierSyntax
//@[006:0019) |   | | | └─Token(Identifier) |resourceGroup|
//@[019:0020) |   | | ├─Token(LeftParen) |(|
//@[020:0021) |   | | └─Token(RightParen) |)|
//@[021:0022) |   | ├─Token(Dot) |.|
//@[022:0030) |   | └─IdentifierSyntax
//@[022:0030) |   |   └─Token(Identifier) |location|
//@[030:0031) |   └─Token(RightSquare) |]|
//@[031:0033) ├─Token(NewLine) |\n\n|

var anyIndexOnAny = any(true)[any(false)]
//@[000:0041) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0017) | ├─IdentifierSyntax
//@[004:0017) | | └─Token(Identifier) |anyIndexOnAny|
//@[018:0019) | ├─Token(Assignment) |=|
//@[020:0041) | └─ArrayAccessSyntax
//@[020:0029) |   ├─FunctionCallSyntax
//@[020:0023) |   | ├─IdentifierSyntax
//@[020:0023) |   | | └─Token(Identifier) |any|
//@[023:0024) |   | ├─Token(LeftParen) |(|
//@[024:0028) |   | ├─FunctionArgumentSyntax
//@[024:0028) |   | | └─BooleanLiteralSyntax
//@[024:0028) |   | |   └─Token(TrueKeyword) |true|
//@[028:0029) |   | └─Token(RightParen) |)|
//@[029:0030) |   ├─Token(LeftSquare) |[|
//@[030:0040) |   ├─FunctionCallSyntax
//@[030:0033) |   | ├─IdentifierSyntax
//@[030:0033) |   | | └─Token(Identifier) |any|
//@[033:0034) |   | ├─Token(LeftParen) |(|
//@[034:0039) |   | ├─FunctionArgumentSyntax
//@[034:0039) |   | | └─BooleanLiteralSyntax
//@[034:0039) |   | |   └─Token(FalseKeyword) |false|
//@[039:0040) |   | └─Token(RightParen) |)|
//@[040:0041) |   └─Token(RightSquare) |]|
//@[041:0043) ├─Token(NewLine) |\n\n|

var deploymentName = deployment().name
//@[000:0038) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0018) | ├─IdentifierSyntax
//@[004:0018) | | └─Token(Identifier) |deploymentName|
//@[019:0020) | ├─Token(Assignment) |=|
//@[021:0038) | └─PropertyAccessSyntax
//@[021:0033) |   ├─FunctionCallSyntax
//@[021:0031) |   | ├─IdentifierSyntax
//@[021:0031) |   | | └─Token(Identifier) |deployment|
//@[031:0032) |   | ├─Token(LeftParen) |(|
//@[032:0033) |   | └─Token(RightParen) |)|
//@[033:0034) |   ├─Token(Dot) |.|
//@[034:0038) |   └─IdentifierSyntax
//@[034:0038) |     └─Token(Identifier) |name|
//@[038:0039) ├─Token(NewLine) |\n|
var templateContentVersion = deployment().properties.template.contentVersion
//@[000:0076) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0026) | ├─IdentifierSyntax
//@[004:0026) | | └─Token(Identifier) |templateContentVersion|
//@[027:0028) | ├─Token(Assignment) |=|
//@[029:0076) | └─PropertyAccessSyntax
//@[029:0061) |   ├─PropertyAccessSyntax
//@[029:0052) |   | ├─PropertyAccessSyntax
//@[029:0041) |   | | ├─FunctionCallSyntax
//@[029:0039) |   | | | ├─IdentifierSyntax
//@[029:0039) |   | | | | └─Token(Identifier) |deployment|
//@[039:0040) |   | | | ├─Token(LeftParen) |(|
//@[040:0041) |   | | | └─Token(RightParen) |)|
//@[041:0042) |   | | ├─Token(Dot) |.|
//@[042:0052) |   | | └─IdentifierSyntax
//@[042:0052) |   | |   └─Token(Identifier) |properties|
//@[052:0053) |   | ├─Token(Dot) |.|
//@[053:0061) |   | └─IdentifierSyntax
//@[053:0061) |   |   └─Token(Identifier) |template|
//@[061:0062) |   ├─Token(Dot) |.|
//@[062:0076) |   └─IdentifierSyntax
//@[062:0076) |     └─Token(Identifier) |contentVersion|
//@[076:0077) ├─Token(NewLine) |\n|
var templateLinkUri = deployment().properties.templateLink.uri
//@[000:0062) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0019) | ├─IdentifierSyntax
//@[004:0019) | | └─Token(Identifier) |templateLinkUri|
//@[020:0021) | ├─Token(Assignment) |=|
//@[022:0062) | └─PropertyAccessSyntax
//@[022:0058) |   ├─PropertyAccessSyntax
//@[022:0045) |   | ├─PropertyAccessSyntax
//@[022:0034) |   | | ├─FunctionCallSyntax
//@[022:0032) |   | | | ├─IdentifierSyntax
//@[022:0032) |   | | | | └─Token(Identifier) |deployment|
//@[032:0033) |   | | | ├─Token(LeftParen) |(|
//@[033:0034) |   | | | └─Token(RightParen) |)|
//@[034:0035) |   | | ├─Token(Dot) |.|
//@[035:0045) |   | | └─IdentifierSyntax
//@[035:0045) |   | |   └─Token(Identifier) |properties|
//@[045:0046) |   | ├─Token(Dot) |.|
//@[046:0058) |   | └─IdentifierSyntax
//@[046:0058) |   |   └─Token(Identifier) |templateLink|
//@[058:0059) |   ├─Token(Dot) |.|
//@[059:0062) |   └─IdentifierSyntax
//@[059:0062) |     └─Token(Identifier) |uri|
//@[062:0063) ├─Token(NewLine) |\n|
var templateLinkId = deployment().properties.templateLink.id
//@[000:0060) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0018) | ├─IdentifierSyntax
//@[004:0018) | | └─Token(Identifier) |templateLinkId|
//@[019:0020) | ├─Token(Assignment) |=|
//@[021:0060) | └─PropertyAccessSyntax
//@[021:0057) |   ├─PropertyAccessSyntax
//@[021:0044) |   | ├─PropertyAccessSyntax
//@[021:0033) |   | | ├─FunctionCallSyntax
//@[021:0031) |   | | | ├─IdentifierSyntax
//@[021:0031) |   | | | | └─Token(Identifier) |deployment|
//@[031:0032) |   | | | ├─Token(LeftParen) |(|
//@[032:0033) |   | | | └─Token(RightParen) |)|
//@[033:0034) |   | | ├─Token(Dot) |.|
//@[034:0044) |   | | └─IdentifierSyntax
//@[034:0044) |   | |   └─Token(Identifier) |properties|
//@[044:0045) |   | ├─Token(Dot) |.|
//@[045:0057) |   | └─IdentifierSyntax
//@[045:0057) |   |   └─Token(Identifier) |templateLink|
//@[057:0058) |   ├─Token(Dot) |.|
//@[058:0060) |   └─IdentifierSyntax
//@[058:0060) |     └─Token(Identifier) |id|
//@[060:0062) ├─Token(NewLine) |\n\n|

var portalEndpoint = environment().portal
//@[000:0041) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0018) | ├─IdentifierSyntax
//@[004:0018) | | └─Token(Identifier) |portalEndpoint|
//@[019:0020) | ├─Token(Assignment) |=|
//@[021:0041) | └─PropertyAccessSyntax
//@[021:0034) |   ├─FunctionCallSyntax
//@[021:0032) |   | ├─IdentifierSyntax
//@[021:0032) |   | | └─Token(Identifier) |environment|
//@[032:0033) |   | ├─Token(LeftParen) |(|
//@[033:0034) |   | └─Token(RightParen) |)|
//@[034:0035) |   ├─Token(Dot) |.|
//@[035:0041) |   └─IdentifierSyntax
//@[035:0041) |     └─Token(Identifier) |portal|
//@[041:0042) ├─Token(NewLine) |\n|
var loginEndpoint = environment().authentication.loginEndpoint
//@[000:0062) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0017) | ├─IdentifierSyntax
//@[004:0017) | | └─Token(Identifier) |loginEndpoint|
//@[018:0019) | ├─Token(Assignment) |=|
//@[020:0062) | └─PropertyAccessSyntax
//@[020:0048) |   ├─PropertyAccessSyntax
//@[020:0033) |   | ├─FunctionCallSyntax
//@[020:0031) |   | | ├─IdentifierSyntax
//@[020:0031) |   | | | └─Token(Identifier) |environment|
//@[031:0032) |   | | ├─Token(LeftParen) |(|
//@[032:0033) |   | | └─Token(RightParen) |)|
//@[033:0034) |   | ├─Token(Dot) |.|
//@[034:0048) |   | └─IdentifierSyntax
//@[034:0048) |   |   └─Token(Identifier) |authentication|
//@[048:0049) |   ├─Token(Dot) |.|
//@[049:0062) |   └─IdentifierSyntax
//@[049:0062) |     └─Token(Identifier) |loginEndpoint|
//@[062:0064) ├─Token(NewLine) |\n\n|

var namedPropertyIndexer = {
//@[000:0048) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0024) | ├─IdentifierSyntax
//@[004:0024) | | └─Token(Identifier) |namedPropertyIndexer|
//@[025:0026) | ├─Token(Assignment) |=|
//@[027:0048) | └─ArrayAccessSyntax
//@[027:0041) |   ├─ObjectSyntax
//@[027:0028) |   | ├─Token(LeftBrace) |{|
//@[028:0029) |   | ├─Token(NewLine) |\n|
  foo: 's'
//@[002:0010) |   | ├─ObjectPropertySyntax
//@[002:0005) |   | | ├─IdentifierSyntax
//@[002:0005) |   | | | └─Token(Identifier) |foo|
//@[005:0006) |   | | ├─Token(Colon) |:|
//@[007:0010) |   | | └─StringSyntax
//@[007:0010) |   | |   └─Token(StringComplete) |'s'|
//@[010:0011) |   | ├─Token(NewLine) |\n|
}['foo']
//@[000:0001) |   | └─Token(RightBrace) |}|
//@[001:0002) |   ├─Token(LeftSquare) |[|
//@[002:0007) |   ├─StringSyntax
//@[002:0007) |   | └─Token(StringComplete) |'foo'|
//@[007:0008) |   └─Token(RightSquare) |]|
//@[008:0010) ├─Token(NewLine) |\n\n|

var intIndexer = [
//@[000:0029) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0014) | ├─IdentifierSyntax
//@[004:0014) | | └─Token(Identifier) |intIndexer|
//@[015:0016) | ├─Token(Assignment) |=|
//@[017:0029) | └─ArrayAccessSyntax
//@[017:0026) |   ├─ArraySyntax
//@[017:0018) |   | ├─Token(LeftSquare) |[|
//@[018:0019) |   | ├─Token(NewLine) |\n|
  's'
//@[002:0005) |   | ├─ArrayItemSyntax
//@[002:0005) |   | | └─StringSyntax
//@[002:0005) |   | |   └─Token(StringComplete) |'s'|
//@[005:0006) |   | ├─Token(NewLine) |\n|
][0]
//@[000:0001) |   | └─Token(RightSquare) |]|
//@[001:0002) |   ├─Token(LeftSquare) |[|
//@[002:0003) |   ├─IntegerLiteralSyntax
//@[002:0003) |   | └─Token(Integer) |0|
//@[003:0004) |   └─Token(RightSquare) |]|
//@[004:0006) ├─Token(NewLine) |\n\n|

var functionOnIndexer1 = concat([
//@[000:0050) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0022) | ├─IdentifierSyntax
//@[004:0022) | | └─Token(Identifier) |functionOnIndexer1|
//@[023:0024) | ├─Token(Assignment) |=|
//@[025:0050) | └─FunctionCallSyntax
//@[025:0031) |   ├─IdentifierSyntax
//@[025:0031) |   | └─Token(Identifier) |concat|
//@[031:0032) |   ├─Token(LeftParen) |(|
//@[032:0044) |   ├─FunctionArgumentSyntax
//@[032:0044) |   | └─ArrayAccessSyntax
//@[032:0041) |   |   ├─ArraySyntax
//@[032:0033) |   |   | ├─Token(LeftSquare) |[|
//@[033:0034) |   |   | ├─Token(NewLine) |\n|
  's'
//@[002:0005) |   |   | ├─ArrayItemSyntax
//@[002:0005) |   |   | | └─StringSyntax
//@[002:0005) |   |   | |   └─Token(StringComplete) |'s'|
//@[005:0006) |   |   | ├─Token(NewLine) |\n|
][0], 's')
//@[000:0001) |   |   | └─Token(RightSquare) |]|
//@[001:0002) |   |   ├─Token(LeftSquare) |[|
//@[002:0003) |   |   ├─IntegerLiteralSyntax
//@[002:0003) |   |   | └─Token(Integer) |0|
//@[003:0004) |   |   └─Token(RightSquare) |]|
//@[004:0005) |   ├─Token(Comma) |,|
//@[006:0009) |   ├─FunctionArgumentSyntax
//@[006:0009) |   | └─StringSyntax
//@[006:0009) |   |   └─Token(StringComplete) |'s'|
//@[009:0010) |   └─Token(RightParen) |)|
//@[010:0012) ├─Token(NewLine) |\n\n|

var functionOnIndexer2 = concat([
//@[000:0044) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0022) | ├─IdentifierSyntax
//@[004:0022) | | └─Token(Identifier) |functionOnIndexer2|
//@[023:0024) | ├─Token(Assignment) |=|
//@[025:0044) | └─FunctionCallSyntax
//@[025:0031) |   ├─IdentifierSyntax
//@[025:0031) |   | └─Token(Identifier) |concat|
//@[031:0032) |   ├─Token(LeftParen) |(|
//@[032:0038) |   ├─FunctionArgumentSyntax
//@[032:0038) |   | └─ArrayAccessSyntax
//@[032:0035) |   |   ├─ArraySyntax
//@[032:0033) |   |   | ├─Token(LeftSquare) |[|
//@[033:0034) |   |   | ├─Token(NewLine) |\n|
][0], 's')
//@[000:0001) |   |   | └─Token(RightSquare) |]|
//@[001:0002) |   |   ├─Token(LeftSquare) |[|
//@[002:0003) |   |   ├─IntegerLiteralSyntax
//@[002:0003) |   |   | └─Token(Integer) |0|
//@[003:0004) |   |   └─Token(RightSquare) |]|
//@[004:0005) |   ├─Token(Comma) |,|
//@[006:0009) |   ├─FunctionArgumentSyntax
//@[006:0009) |   | └─StringSyntax
//@[006:0009) |   |   └─Token(StringComplete) |'s'|
//@[009:0010) |   └─Token(RightParen) |)|
//@[010:0012) ├─Token(NewLine) |\n\n|

var functionOnIndexer3 = concat([
//@[000:0049) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0022) | ├─IdentifierSyntax
//@[004:0022) | | └─Token(Identifier) |functionOnIndexer3|
//@[023:0024) | ├─Token(Assignment) |=|
//@[025:0049) | └─FunctionCallSyntax
//@[025:0031) |   ├─IdentifierSyntax
//@[025:0031) |   | └─Token(Identifier) |concat|
//@[031:0032) |   ├─Token(LeftParen) |(|
//@[032:0038) |   ├─FunctionArgumentSyntax
//@[032:0038) |   | └─ArrayAccessSyntax
//@[032:0035) |   |   ├─ArraySyntax
//@[032:0033) |   |   | ├─Token(LeftSquare) |[|
//@[033:0034) |   |   | ├─Token(NewLine) |\n|
][0], any('s'))
//@[000:0001) |   |   | └─Token(RightSquare) |]|
//@[001:0002) |   |   ├─Token(LeftSquare) |[|
//@[002:0003) |   |   ├─IntegerLiteralSyntax
//@[002:0003) |   |   | └─Token(Integer) |0|
//@[003:0004) |   |   └─Token(RightSquare) |]|
//@[004:0005) |   ├─Token(Comma) |,|
//@[006:0014) |   ├─FunctionArgumentSyntax
//@[006:0014) |   | └─FunctionCallSyntax
//@[006:0009) |   |   ├─IdentifierSyntax
//@[006:0009) |   |   | └─Token(Identifier) |any|
//@[009:0010) |   |   ├─Token(LeftParen) |(|
//@[010:0013) |   |   ├─FunctionArgumentSyntax
//@[010:0013) |   |   | └─StringSyntax
//@[010:0013) |   |   |   └─Token(StringComplete) |'s'|
//@[013:0014) |   |   └─Token(RightParen) |)|
//@[014:0015) |   └─Token(RightParen) |)|
//@[015:0017) ├─Token(NewLine) |\n\n|

var singleQuote = '\''
//@[000:0022) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0015) | ├─IdentifierSyntax
//@[004:0015) | | └─Token(Identifier) |singleQuote|
//@[016:0017) | ├─Token(Assignment) |=|
//@[018:0022) | └─StringSyntax
//@[018:0022) |   └─Token(StringComplete) |'\''|
//@[022:0023) ├─Token(NewLine) |\n|
var myPropertyName = '${singleQuote}foo${singleQuote}'
//@[000:0054) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0018) | ├─IdentifierSyntax
//@[004:0018) | | └─Token(Identifier) |myPropertyName|
//@[019:0020) | ├─Token(Assignment) |=|
//@[021:0054) | └─StringSyntax
//@[021:0024) |   ├─Token(StringLeftPiece) |'${|
//@[024:0035) |   ├─VariableAccessSyntax
//@[024:0035) |   | └─IdentifierSyntax
//@[024:0035) |   |   └─Token(Identifier) |singleQuote|
//@[035:0041) |   ├─Token(StringMiddlePiece) |}foo${|
//@[041:0052) |   ├─VariableAccessSyntax
//@[041:0052) |   | └─IdentifierSyntax
//@[041:0052) |   |   └─Token(Identifier) |singleQuote|
//@[052:0054) |   └─Token(StringRightPiece) |}'|
//@[054:0056) ├─Token(NewLine) |\n\n|

var unusedIntermediate = listKeys(resourceId('Mock.RP/type', 'steve'), '2020-01-01')
//@[000:0084) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0022) | ├─IdentifierSyntax
//@[004:0022) | | └─Token(Identifier) |unusedIntermediate|
//@[023:0024) | ├─Token(Assignment) |=|
//@[025:0084) | └─FunctionCallSyntax
//@[025:0033) |   ├─IdentifierSyntax
//@[025:0033) |   | └─Token(Identifier) |listKeys|
//@[033:0034) |   ├─Token(LeftParen) |(|
//@[034:0069) |   ├─FunctionArgumentSyntax
//@[034:0069) |   | └─FunctionCallSyntax
//@[034:0044) |   |   ├─IdentifierSyntax
//@[034:0044) |   |   | └─Token(Identifier) |resourceId|
//@[044:0045) |   |   ├─Token(LeftParen) |(|
//@[045:0059) |   |   ├─FunctionArgumentSyntax
//@[045:0059) |   |   | └─StringSyntax
//@[045:0059) |   |   |   └─Token(StringComplete) |'Mock.RP/type'|
//@[059:0060) |   |   ├─Token(Comma) |,|
//@[061:0068) |   |   ├─FunctionArgumentSyntax
//@[061:0068) |   |   | └─StringSyntax
//@[061:0068) |   |   |   └─Token(StringComplete) |'steve'|
//@[068:0069) |   |   └─Token(RightParen) |)|
//@[069:0070) |   ├─Token(Comma) |,|
//@[071:0083) |   ├─FunctionArgumentSyntax
//@[071:0083) |   | └─StringSyntax
//@[071:0083) |   |   └─Token(StringComplete) |'2020-01-01'|
//@[083:0084) |   └─Token(RightParen) |)|
//@[084:0085) ├─Token(NewLine) |\n|
var unusedIntermediateRef = unusedIntermediate.secondaryKey
//@[000:0059) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0025) | ├─IdentifierSyntax
//@[004:0025) | | └─Token(Identifier) |unusedIntermediateRef|
//@[026:0027) | ├─Token(Assignment) |=|
//@[028:0059) | └─PropertyAccessSyntax
//@[028:0046) |   ├─VariableAccessSyntax
//@[028:0046) |   | └─IdentifierSyntax
//@[028:0046) |   |   └─Token(Identifier) |unusedIntermediate|
//@[046:0047) |   ├─Token(Dot) |.|
//@[047:0059) |   └─IdentifierSyntax
//@[047:0059) |     └─Token(Identifier) |secondaryKey|
//@[059:0061) ├─Token(NewLine) |\n\n|

// previously this was not possible to emit correctly
//@[053:0054) ├─Token(NewLine) |\n|
var previousEmitLimit = [
//@[000:0299) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0021) | ├─IdentifierSyntax
//@[004:0021) | | └─Token(Identifier) |previousEmitLimit|
//@[022:0023) | ├─Token(Assignment) |=|
//@[024:0299) | └─ArraySyntax
//@[024:0025) |   ├─Token(LeftSquare) |[|
//@[025:0026) |   ├─Token(NewLine) |\n|
  concat('s')
//@[002:0013) |   ├─ArrayItemSyntax
//@[002:0013) |   | └─FunctionCallSyntax
//@[002:0008) |   |   ├─IdentifierSyntax
//@[002:0008) |   |   | └─Token(Identifier) |concat|
//@[008:0009) |   |   ├─Token(LeftParen) |(|
//@[009:0012) |   |   ├─FunctionArgumentSyntax
//@[009:0012) |   |   | └─StringSyntax
//@[009:0012) |   |   |   └─Token(StringComplete) |'s'|
//@[012:0013) |   |   └─Token(RightParen) |)|
//@[013:0014) |   ├─Token(NewLine) |\n|
  '${4}'
//@[002:0008) |   ├─ArrayItemSyntax
//@[002:0008) |   | └─StringSyntax
//@[002:0005) |   |   ├─Token(StringLeftPiece) |'${|
//@[005:0006) |   |   ├─IntegerLiteralSyntax
//@[005:0006) |   |   | └─Token(Integer) |4|
//@[006:0008) |   |   └─Token(StringRightPiece) |}'|
//@[008:0009) |   ├─Token(NewLine) |\n|
  {
//@[002:0248) |   ├─ArrayItemSyntax
//@[002:0248) |   | └─ObjectSyntax
//@[002:0003) |   |   ├─Token(LeftBrace) |{|
//@[003:0004) |   |   ├─Token(NewLine) |\n|
    a: {
//@[004:0240) |   |   ├─ObjectPropertySyntax
//@[004:0005) |   |   | ├─IdentifierSyntax
//@[004:0005) |   |   | | └─Token(Identifier) |a|
//@[005:0006) |   |   | ├─Token(Colon) |:|
//@[007:0240) |   |   | └─ObjectSyntax
//@[007:0008) |   |   |   ├─Token(LeftBrace) |{|
//@[008:0009) |   |   |   ├─Token(NewLine) |\n|
      b: base64('s')
//@[006:0020) |   |   |   ├─ObjectPropertySyntax
//@[006:0007) |   |   |   | ├─IdentifierSyntax
//@[006:0007) |   |   |   | | └─Token(Identifier) |b|
//@[007:0008) |   |   |   | ├─Token(Colon) |:|
//@[009:0020) |   |   |   | └─FunctionCallSyntax
//@[009:0015) |   |   |   |   ├─IdentifierSyntax
//@[009:0015) |   |   |   |   | └─Token(Identifier) |base64|
//@[015:0016) |   |   |   |   ├─Token(LeftParen) |(|
//@[016:0019) |   |   |   |   ├─FunctionArgumentSyntax
//@[016:0019) |   |   |   |   | └─StringSyntax
//@[016:0019) |   |   |   |   |   └─Token(StringComplete) |'s'|
//@[019:0020) |   |   |   |   └─Token(RightParen) |)|
//@[020:0021) |   |   |   ├─Token(NewLine) |\n|
      c: concat([
//@[006:0082) |   |   |   ├─ObjectPropertySyntax
//@[006:0007) |   |   |   | ├─IdentifierSyntax
//@[006:0007) |   |   |   | | └─Token(Identifier) |c|
//@[007:0008) |   |   |   | ├─Token(Colon) |:|
//@[009:0082) |   |   |   | └─FunctionCallSyntax
//@[009:0015) |   |   |   |   ├─IdentifierSyntax
//@[009:0015) |   |   |   |   | └─Token(Identifier) |concat|
//@[015:0016) |   |   |   |   ├─Token(LeftParen) |(|
//@[016:0040) |   |   |   |   ├─FunctionArgumentSyntax
//@[016:0040) |   |   |   |   | └─ArraySyntax
//@[016:0017) |   |   |   |   |   ├─Token(LeftSquare) |[|
//@[017:0018) |   |   |   |   |   ├─Token(NewLine) |\n|
        12 + 3
//@[008:0014) |   |   |   |   |   ├─ArrayItemSyntax
//@[008:0014) |   |   |   |   |   | └─BinaryOperationSyntax
//@[008:0010) |   |   |   |   |   |   ├─IntegerLiteralSyntax
//@[008:0010) |   |   |   |   |   |   | └─Token(Integer) |12|
//@[011:0012) |   |   |   |   |   |   ├─Token(Plus) |+|
//@[013:0014) |   |   |   |   |   |   └─IntegerLiteralSyntax
//@[013:0014) |   |   |   |   |   |     └─Token(Integer) |3|
//@[014:0015) |   |   |   |   |   ├─Token(NewLine) |\n|
      ], [
//@[006:0007) |   |   |   |   |   └─Token(RightSquare) |]|
//@[007:0008) |   |   |   |   ├─Token(Comma) |,|
//@[009:0048) |   |   |   |   ├─FunctionArgumentSyntax
//@[009:0048) |   |   |   |   | └─ArraySyntax
//@[009:0010) |   |   |   |   |   ├─Token(LeftSquare) |[|
//@[010:0011) |   |   |   |   |   ├─Token(NewLine) |\n|
        !true
//@[008:0013) |   |   |   |   |   ├─ArrayItemSyntax
//@[008:0013) |   |   |   |   |   | └─UnaryOperationSyntax
//@[008:0009) |   |   |   |   |   |   ├─Token(Exclamation) |!|
//@[009:0013) |   |   |   |   |   |   └─BooleanLiteralSyntax
//@[009:0013) |   |   |   |   |   |     └─Token(TrueKeyword) |true|
//@[013:0014) |   |   |   |   |   ├─Token(NewLine) |\n|
        'hello'
//@[008:0015) |   |   |   |   |   ├─ArrayItemSyntax
//@[008:0015) |   |   |   |   |   | └─StringSyntax
//@[008:0015) |   |   |   |   |   |   └─Token(StringComplete) |'hello'|
//@[015:0016) |   |   |   |   |   ├─Token(NewLine) |\n|
      ])
//@[006:0007) |   |   |   |   |   └─Token(RightSquare) |]|
//@[007:0008) |   |   |   |   └─Token(RightParen) |)|
//@[008:0009) |   |   |   ├─Token(NewLine) |\n|
      d: az.resourceGroup().location
//@[006:0036) |   |   |   ├─ObjectPropertySyntax
//@[006:0007) |   |   |   | ├─IdentifierSyntax
//@[006:0007) |   |   |   | | └─Token(Identifier) |d|
//@[007:0008) |   |   |   | ├─Token(Colon) |:|
//@[009:0036) |   |   |   | └─PropertyAccessSyntax
//@[009:0027) |   |   |   |   ├─InstanceFunctionCallSyntax
//@[009:0011) |   |   |   |   | ├─VariableAccessSyntax
//@[009:0011) |   |   |   |   | | └─IdentifierSyntax
//@[009:0011) |   |   |   |   | |   └─Token(Identifier) |az|
//@[011:0012) |   |   |   |   | ├─Token(Dot) |.|
//@[012:0025) |   |   |   |   | ├─IdentifierSyntax
//@[012:0025) |   |   |   |   | | └─Token(Identifier) |resourceGroup|
//@[025:0026) |   |   |   |   | ├─Token(LeftParen) |(|
//@[026:0027) |   |   |   |   | └─Token(RightParen) |)|
//@[027:0028) |   |   |   |   ├─Token(Dot) |.|
//@[028:0036) |   |   |   |   └─IdentifierSyntax
//@[028:0036) |   |   |   |     └─Token(Identifier) |location|
//@[036:0037) |   |   |   ├─Token(NewLine) |\n|
      e: concat([
//@[006:0039) |   |   |   ├─ObjectPropertySyntax
//@[006:0007) |   |   |   | ├─IdentifierSyntax
//@[006:0007) |   |   |   | | └─Token(Identifier) |e|
//@[007:0008) |   |   |   | ├─Token(Colon) |:|
//@[009:0039) |   |   |   | └─FunctionCallSyntax
//@[009:0015) |   |   |   |   ├─IdentifierSyntax
//@[009:0015) |   |   |   |   | └─Token(Identifier) |concat|
//@[015:0016) |   |   |   |   ├─Token(LeftParen) |(|
//@[016:0038) |   |   |   |   ├─FunctionArgumentSyntax
//@[016:0038) |   |   |   |   | └─ArraySyntax
//@[016:0017) |   |   |   |   |   ├─Token(LeftSquare) |[|
//@[017:0018) |   |   |   |   |   ├─Token(NewLine) |\n|
        true
//@[008:0012) |   |   |   |   |   ├─ArrayItemSyntax
//@[008:0012) |   |   |   |   |   | └─BooleanLiteralSyntax
//@[008:0012) |   |   |   |   |   |   └─Token(TrueKeyword) |true|
//@[012:0013) |   |   |   |   |   ├─Token(NewLine) |\n|
      ])
//@[006:0007) |   |   |   |   |   └─Token(RightSquare) |]|
//@[007:0008) |   |   |   |   └─Token(RightParen) |)|
//@[008:0009) |   |   |   ├─Token(NewLine) |\n|
      f: concat([
//@[006:0044) |   |   |   ├─ObjectPropertySyntax
//@[006:0007) |   |   |   | ├─IdentifierSyntax
//@[006:0007) |   |   |   | | └─Token(Identifier) |f|
//@[007:0008) |   |   |   | ├─Token(Colon) |:|
//@[009:0044) |   |   |   | └─FunctionCallSyntax
//@[009:0015) |   |   |   |   ├─IdentifierSyntax
//@[009:0015) |   |   |   |   | └─Token(Identifier) |concat|
//@[015:0016) |   |   |   |   ├─Token(LeftParen) |(|
//@[016:0043) |   |   |   |   ├─FunctionArgumentSyntax
//@[016:0043) |   |   |   |   | └─ArraySyntax
//@[016:0017) |   |   |   |   |   ├─Token(LeftSquare) |[|
//@[017:0018) |   |   |   |   |   ├─Token(NewLine) |\n|
        's' == 12
//@[008:0017) |   |   |   |   |   ├─ArrayItemSyntax
//@[008:0017) |   |   |   |   |   | └─BinaryOperationSyntax
//@[008:0011) |   |   |   |   |   |   ├─StringSyntax
//@[008:0011) |   |   |   |   |   |   | └─Token(StringComplete) |'s'|
//@[012:0014) |   |   |   |   |   |   ├─Token(Equals) |==|
//@[015:0017) |   |   |   |   |   |   └─IntegerLiteralSyntax
//@[015:0017) |   |   |   |   |   |     └─Token(Integer) |12|
//@[017:0018) |   |   |   |   |   ├─Token(NewLine) |\n|
      ])
//@[006:0007) |   |   |   |   |   └─Token(RightSquare) |]|
//@[007:0008) |   |   |   |   └─Token(RightParen) |)|
//@[008:0009) |   |   |   ├─Token(NewLine) |\n|
    }
//@[004:0005) |   |   |   └─Token(RightBrace) |}|
//@[005:0006) |   |   ├─Token(NewLine) |\n|
  }
//@[002:0003) |   |   └─Token(RightBrace) |}|
//@[003:0004) |   ├─Token(NewLine) |\n|
]
//@[000:0001) |   └─Token(RightSquare) |]|
//@[001:0003) ├─Token(NewLine) |\n\n|

// previously this was not possible to emit correctly
//@[053:0054) ├─Token(NewLine) |\n|
var previousEmitLimit2 = [
//@[000:0327) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0022) | ├─IdentifierSyntax
//@[004:0022) | | └─Token(Identifier) |previousEmitLimit2|
//@[023:0024) | ├─Token(Assignment) |=|
//@[025:0327) | └─ArraySyntax
//@[025:0026) |   ├─Token(LeftSquare) |[|
//@[026:0027) |   ├─Token(NewLine) |\n|
  concat('s')
//@[002:0013) |   ├─ArrayItemSyntax
//@[002:0013) |   | └─FunctionCallSyntax
//@[002:0008) |   |   ├─IdentifierSyntax
//@[002:0008) |   |   | └─Token(Identifier) |concat|
//@[008:0009) |   |   ├─Token(LeftParen) |(|
//@[009:0012) |   |   ├─FunctionArgumentSyntax
//@[009:0012) |   |   | └─StringSyntax
//@[009:0012) |   |   |   └─Token(StringComplete) |'s'|
//@[012:0013) |   |   └─Token(RightParen) |)|
//@[013:0014) |   ├─Token(NewLine) |\n|
  '${4}'
//@[002:0008) |   ├─ArrayItemSyntax
//@[002:0008) |   | └─StringSyntax
//@[002:0005) |   |   ├─Token(StringLeftPiece) |'${|
//@[005:0006) |   |   ├─IntegerLiteralSyntax
//@[005:0006) |   |   | └─Token(Integer) |4|
//@[006:0008) |   |   └─Token(StringRightPiece) |}'|
//@[008:0009) |   ├─Token(NewLine) |\n|
  {
//@[002:0275) |   ├─ArrayItemSyntax
//@[002:0275) |   | └─ObjectSyntax
//@[002:0003) |   |   ├─Token(LeftBrace) |{|
//@[003:0004) |   |   ├─Token(NewLine) |\n|
    a: {
//@[004:0267) |   |   ├─ObjectPropertySyntax
//@[004:0005) |   |   | ├─IdentifierSyntax
//@[004:0005) |   |   | | └─Token(Identifier) |a|
//@[005:0006) |   |   | ├─Token(Colon) |:|
//@[007:0267) |   |   | └─ObjectSyntax
//@[007:0008) |   |   |   ├─Token(LeftBrace) |{|
//@[008:0009) |   |   |   ├─Token(NewLine) |\n|
      b: base64('s')
//@[006:0020) |   |   |   ├─ObjectPropertySyntax
//@[006:0007) |   |   |   | ├─IdentifierSyntax
//@[006:0007) |   |   |   | | └─Token(Identifier) |b|
//@[007:0008) |   |   |   | ├─Token(Colon) |:|
//@[009:0020) |   |   |   | └─FunctionCallSyntax
//@[009:0015) |   |   |   |   ├─IdentifierSyntax
//@[009:0015) |   |   |   |   | └─Token(Identifier) |base64|
//@[015:0016) |   |   |   |   ├─Token(LeftParen) |(|
//@[016:0019) |   |   |   |   ├─FunctionArgumentSyntax
//@[016:0019) |   |   |   |   | └─StringSyntax
//@[016:0019) |   |   |   |   |   └─Token(StringComplete) |'s'|
//@[019:0020) |   |   |   |   └─Token(RightParen) |)|
//@[020:0021) |   |   |   ├─Token(NewLine) |\n|
      c: union({
//@[006:0090) |   |   |   ├─ObjectPropertySyntax
//@[006:0007) |   |   |   | ├─IdentifierSyntax
//@[006:0007) |   |   |   | | └─Token(Identifier) |c|
//@[007:0008) |   |   |   | ├─Token(Colon) |:|
//@[009:0090) |   |   |   | └─FunctionCallSyntax
//@[009:0014) |   |   |   |   ├─IdentifierSyntax
//@[009:0014) |   |   |   |   | └─Token(Identifier) |union|
//@[014:0015) |   |   |   |   ├─Token(LeftParen) |(|
//@[015:0042) |   |   |   |   ├─FunctionArgumentSyntax
//@[015:0042) |   |   |   |   | └─ObjectSyntax
//@[015:0016) |   |   |   |   |   ├─Token(LeftBrace) |{|
//@[016:0017) |   |   |   |   |   ├─Token(NewLine) |\n|
        a: 12 + 3
//@[008:0017) |   |   |   |   |   ├─ObjectPropertySyntax
//@[008:0009) |   |   |   |   |   | ├─IdentifierSyntax
//@[008:0009) |   |   |   |   |   | | └─Token(Identifier) |a|
//@[009:0010) |   |   |   |   |   | ├─Token(Colon) |:|
//@[011:0017) |   |   |   |   |   | └─BinaryOperationSyntax
//@[011:0013) |   |   |   |   |   |   ├─IntegerLiteralSyntax
//@[011:0013) |   |   |   |   |   |   | └─Token(Integer) |12|
//@[014:0015) |   |   |   |   |   |   ├─Token(Plus) |+|
//@[016:0017) |   |   |   |   |   |   └─IntegerLiteralSyntax
//@[016:0017) |   |   |   |   |   |     └─Token(Integer) |3|
//@[017:0018) |   |   |   |   |   ├─Token(NewLine) |\n|
      }, {
//@[006:0007) |   |   |   |   |   └─Token(RightBrace) |}|
//@[007:0008) |   |   |   |   ├─Token(Comma) |,|
//@[009:0054) |   |   |   |   ├─FunctionArgumentSyntax
//@[009:0054) |   |   |   |   | └─ObjectSyntax
//@[009:0010) |   |   |   |   |   ├─Token(LeftBrace) |{|
//@[010:0011) |   |   |   |   |   ├─Token(NewLine) |\n|
        b: !true
//@[008:0016) |   |   |   |   |   ├─ObjectPropertySyntax
//@[008:0009) |   |   |   |   |   | ├─IdentifierSyntax
//@[008:0009) |   |   |   |   |   | | └─Token(Identifier) |b|
//@[009:0010) |   |   |   |   |   | ├─Token(Colon) |:|
//@[011:0016) |   |   |   |   |   | └─UnaryOperationSyntax
//@[011:0012) |   |   |   |   |   |   ├─Token(Exclamation) |!|
//@[012:0016) |   |   |   |   |   |   └─BooleanLiteralSyntax
//@[012:0016) |   |   |   |   |   |     └─Token(TrueKeyword) |true|
//@[016:0017) |   |   |   |   |   ├─Token(NewLine) |\n|
        c: 'hello'
//@[008:0018) |   |   |   |   |   ├─ObjectPropertySyntax
//@[008:0009) |   |   |   |   |   | ├─IdentifierSyntax
//@[008:0009) |   |   |   |   |   | | └─Token(Identifier) |c|
//@[009:0010) |   |   |   |   |   | ├─Token(Colon) |:|
//@[011:0018) |   |   |   |   |   | └─StringSyntax
//@[011:0018) |   |   |   |   |   |   └─Token(StringComplete) |'hello'|
//@[018:0019) |   |   |   |   |   ├─Token(NewLine) |\n|
      })
//@[006:0007) |   |   |   |   |   └─Token(RightBrace) |}|
//@[007:0008) |   |   |   |   └─Token(RightParen) |)|
//@[008:0009) |   |   |   ├─Token(NewLine) |\n|
      d: az.resourceGroup().location
//@[006:0036) |   |   |   ├─ObjectPropertySyntax
//@[006:0007) |   |   |   | ├─IdentifierSyntax
//@[006:0007) |   |   |   | | └─Token(Identifier) |d|
//@[007:0008) |   |   |   | ├─Token(Colon) |:|
//@[009:0036) |   |   |   | └─PropertyAccessSyntax
//@[009:0027) |   |   |   |   ├─InstanceFunctionCallSyntax
//@[009:0011) |   |   |   |   | ├─VariableAccessSyntax
//@[009:0011) |   |   |   |   | | └─IdentifierSyntax
//@[009:0011) |   |   |   |   | |   └─Token(Identifier) |az|
//@[011:0012) |   |   |   |   | ├─Token(Dot) |.|
//@[012:0025) |   |   |   |   | ├─IdentifierSyntax
//@[012:0025) |   |   |   |   | | └─Token(Identifier) |resourceGroup|
//@[025:0026) |   |   |   |   | ├─Token(LeftParen) |(|
//@[026:0027) |   |   |   |   | └─Token(RightParen) |)|
//@[027:0028) |   |   |   |   ├─Token(Dot) |.|
//@[028:0036) |   |   |   |   └─IdentifierSyntax
//@[028:0036) |   |   |   |     └─Token(Identifier) |location|
//@[036:0037) |   |   |   ├─Token(NewLine) |\n|
      e: union({
//@[006:0045) |   |   |   ├─ObjectPropertySyntax
//@[006:0007) |   |   |   | ├─IdentifierSyntax
//@[006:0007) |   |   |   | | └─Token(Identifier) |e|
//@[007:0008) |   |   |   | ├─Token(Colon) |:|
//@[009:0045) |   |   |   | └─FunctionCallSyntax
//@[009:0014) |   |   |   |   ├─IdentifierSyntax
//@[009:0014) |   |   |   |   | └─Token(Identifier) |union|
//@[014:0015) |   |   |   |   ├─Token(LeftParen) |(|
//@[015:0040) |   |   |   |   ├─FunctionArgumentSyntax
//@[015:0040) |   |   |   |   | └─ObjectSyntax
//@[015:0016) |   |   |   |   |   ├─Token(LeftBrace) |{|
//@[016:0017) |   |   |   |   |   ├─Token(NewLine) |\n|
        x: true
//@[008:0015) |   |   |   |   |   ├─ObjectPropertySyntax
//@[008:0009) |   |   |   |   |   | ├─IdentifierSyntax
//@[008:0009) |   |   |   |   |   | | └─Token(Identifier) |x|
//@[009:0010) |   |   |   |   |   | ├─Token(Colon) |:|
//@[011:0015) |   |   |   |   |   | └─BooleanLiteralSyntax
//@[011:0015) |   |   |   |   |   |   └─Token(TrueKeyword) |true|
//@[015:0016) |   |   |   |   |   ├─Token(NewLine) |\n|
      }, {})
//@[006:0007) |   |   |   |   |   └─Token(RightBrace) |}|
//@[007:0008) |   |   |   |   ├─Token(Comma) |,|
//@[009:0011) |   |   |   |   ├─FunctionArgumentSyntax
//@[009:0011) |   |   |   |   | └─ObjectSyntax
//@[009:0010) |   |   |   |   |   ├─Token(LeftBrace) |{|
//@[010:0011) |   |   |   |   |   └─Token(RightBrace) |}|
//@[011:0012) |   |   |   |   └─Token(RightParen) |)|
//@[012:0013) |   |   |   ├─Token(NewLine) |\n|
      f: intersection({
//@[006:0057) |   |   |   ├─ObjectPropertySyntax
//@[006:0007) |   |   |   | ├─IdentifierSyntax
//@[006:0007) |   |   |   | | └─Token(Identifier) |f|
//@[007:0008) |   |   |   | ├─Token(Colon) |:|
//@[009:0057) |   |   |   | └─FunctionCallSyntax
//@[009:0021) |   |   |   |   ├─IdentifierSyntax
//@[009:0021) |   |   |   |   | └─Token(Identifier) |intersection|
//@[021:0022) |   |   |   |   ├─Token(LeftParen) |(|
//@[022:0052) |   |   |   |   ├─FunctionArgumentSyntax
//@[022:0052) |   |   |   |   | └─ObjectSyntax
//@[022:0023) |   |   |   |   |   ├─Token(LeftBrace) |{|
//@[023:0024) |   |   |   |   |   ├─Token(NewLine) |\n|
        q: 's' == 12
//@[008:0020) |   |   |   |   |   ├─ObjectPropertySyntax
//@[008:0009) |   |   |   |   |   | ├─IdentifierSyntax
//@[008:0009) |   |   |   |   |   | | └─Token(Identifier) |q|
//@[009:0010) |   |   |   |   |   | ├─Token(Colon) |:|
//@[011:0020) |   |   |   |   |   | └─BinaryOperationSyntax
//@[011:0014) |   |   |   |   |   |   ├─StringSyntax
//@[011:0014) |   |   |   |   |   |   | └─Token(StringComplete) |'s'|
//@[015:0017) |   |   |   |   |   |   ├─Token(Equals) |==|
//@[018:0020) |   |   |   |   |   |   └─IntegerLiteralSyntax
//@[018:0020) |   |   |   |   |   |     └─Token(Integer) |12|
//@[020:0021) |   |   |   |   |   ├─Token(NewLine) |\n|
      }, {})
//@[006:0007) |   |   |   |   |   └─Token(RightBrace) |}|
//@[007:0008) |   |   |   |   ├─Token(Comma) |,|
//@[009:0011) |   |   |   |   ├─FunctionArgumentSyntax
//@[009:0011) |   |   |   |   | └─ObjectSyntax
//@[009:0010) |   |   |   |   |   ├─Token(LeftBrace) |{|
//@[010:0011) |   |   |   |   |   └─Token(RightBrace) |}|
//@[011:0012) |   |   |   |   └─Token(RightParen) |)|
//@[012:0013) |   |   |   ├─Token(NewLine) |\n|
    }
//@[004:0005) |   |   |   └─Token(RightBrace) |}|
//@[005:0006) |   |   ├─Token(NewLine) |\n|
  }
//@[002:0003) |   |   └─Token(RightBrace) |}|
//@[003:0004) |   ├─Token(NewLine) |\n|
]
//@[000:0001) |   └─Token(RightSquare) |]|
//@[001:0003) ├─Token(NewLine) |\n\n|

// previously this was not possible to emit correctly
//@[053:0054) ├─Token(NewLine) |\n|
var previousEmitLimit3 = {
//@[000:0140) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0022) | ├─IdentifierSyntax
//@[004:0022) | | └─Token(Identifier) |previousEmitLimit3|
//@[023:0024) | ├─Token(Assignment) |=|
//@[025:0140) | └─ObjectSyntax
//@[025:0026) |   ├─Token(LeftBrace) |{|
//@[026:0027) |   ├─Token(NewLine) |\n|
  a: {
//@[002:0111) |   ├─ObjectPropertySyntax
//@[002:0003) |   | ├─IdentifierSyntax
//@[002:0003) |   | | └─Token(Identifier) |a|
//@[003:0004) |   | ├─Token(Colon) |:|
//@[005:0111) |   | └─ObjectSyntax
//@[005:0006) |   |   ├─Token(LeftBrace) |{|
//@[006:0007) |   |   ├─Token(NewLine) |\n|
    b: {
//@[004:0056) |   |   ├─ObjectPropertySyntax
//@[004:0005) |   |   | ├─IdentifierSyntax
//@[004:0005) |   |   | | └─Token(Identifier) |b|
//@[005:0006) |   |   | ├─Token(Colon) |:|
//@[007:0056) |   |   | └─BinaryOperationSyntax
//@[007:0051) |   |   |   ├─ObjectSyntax
//@[007:0008) |   |   |   | ├─Token(LeftBrace) |{|
//@[008:0009) |   |   |   | ├─Token(NewLine) |\n|
      a: az.resourceGroup().location
//@[006:0036) |   |   |   | ├─ObjectPropertySyntax
//@[006:0007) |   |   |   | | ├─IdentifierSyntax
//@[006:0007) |   |   |   | | | └─Token(Identifier) |a|
//@[007:0008) |   |   |   | | ├─Token(Colon) |:|
//@[009:0036) |   |   |   | | └─PropertyAccessSyntax
//@[009:0027) |   |   |   | |   ├─InstanceFunctionCallSyntax
//@[009:0011) |   |   |   | |   | ├─VariableAccessSyntax
//@[009:0011) |   |   |   | |   | | └─IdentifierSyntax
//@[009:0011) |   |   |   | |   | |   └─Token(Identifier) |az|
//@[011:0012) |   |   |   | |   | ├─Token(Dot) |.|
//@[012:0025) |   |   |   | |   | ├─IdentifierSyntax
//@[012:0025) |   |   |   | |   | | └─Token(Identifier) |resourceGroup|
//@[025:0026) |   |   |   | |   | ├─Token(LeftParen) |(|
//@[026:0027) |   |   |   | |   | └─Token(RightParen) |)|
//@[027:0028) |   |   |   | |   ├─Token(Dot) |.|
//@[028:0036) |   |   |   | |   └─IdentifierSyntax
//@[028:0036) |   |   |   | |     └─Token(Identifier) |location|
//@[036:0037) |   |   |   | ├─Token(NewLine) |\n|
    } == 2
//@[004:0005) |   |   |   | └─Token(RightBrace) |}|
//@[006:0008) |   |   |   ├─Token(Equals) |==|
//@[009:0010) |   |   |   └─IntegerLiteralSyntax
//@[009:0010) |   |   |     └─Token(Integer) |2|
//@[010:0011) |   |   ├─Token(NewLine) |\n|
    c: concat([
//@[004:0043) |   |   ├─ObjectPropertySyntax
//@[004:0005) |   |   | ├─IdentifierSyntax
//@[004:0005) |   |   | | └─Token(Identifier) |c|
//@[005:0006) |   |   | ├─Token(Colon) |:|
//@[007:0043) |   |   | └─FunctionCallSyntax
//@[007:0013) |   |   |   ├─IdentifierSyntax
//@[007:0013) |   |   |   | └─Token(Identifier) |concat|
//@[013:0014) |   |   |   ├─Token(LeftParen) |(|
//@[014:0022) |   |   |   ├─FunctionArgumentSyntax
//@[014:0022) |   |   |   | └─ArraySyntax
//@[014:0015) |   |   |   |   ├─Token(LeftSquare) |[|
//@[015:0017) |   |   |   |   ├─Token(NewLine) |\n\n|

    ], [
//@[004:0005) |   |   |   |   └─Token(RightSquare) |]|
//@[005:0006) |   |   |   ├─Token(Comma) |,|
//@[007:0025) |   |   |   ├─FunctionArgumentSyntax
//@[007:0025) |   |   |   | └─ArraySyntax
//@[007:0008) |   |   |   |   ├─Token(LeftSquare) |[|
//@[008:0009) |   |   |   |   ├─Token(NewLine) |\n|
      true
//@[006:0010) |   |   |   |   ├─ArrayItemSyntax
//@[006:0010) |   |   |   |   | └─BooleanLiteralSyntax
//@[006:0010) |   |   |   |   |   └─Token(TrueKeyword) |true|
//@[010:0011) |   |   |   |   ├─Token(NewLine) |\n|
    ])
//@[004:0005) |   |   |   |   └─Token(RightSquare) |]|
//@[005:0006) |   |   |   └─Token(RightParen) |)|
//@[006:0007) |   |   ├─Token(NewLine) |\n|
  }
//@[002:0003) |   |   └─Token(RightBrace) |}|
//@[003:0004) |   ├─Token(NewLine) |\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0003) ├─Token(NewLine) |\n\n|

// #completionTest(0) -> declarations
//@[037:0039) ├─Token(NewLine) |\n\n|

var myVar = 'hello'
//@[000:0019) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0009) | ├─IdentifierSyntax
//@[004:0009) | | └─Token(Identifier) |myVar|
//@[010:0011) | ├─Token(Assignment) |=|
//@[012:0019) | └─StringSyntax
//@[012:0019) |   └─Token(StringComplete) |'hello'|
//@[019:0020) ├─Token(NewLine) |\n|
var myVar2 = any({
//@[000:0040) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0010) | ├─IdentifierSyntax
//@[004:0010) | | └─Token(Identifier) |myVar2|
//@[011:0012) | ├─Token(Assignment) |=|
//@[013:0040) | └─FunctionCallSyntax
//@[013:0016) |   ├─IdentifierSyntax
//@[013:0016) |   | └─Token(Identifier) |any|
//@[016:0017) |   ├─Token(LeftParen) |(|
//@[017:0039) |   ├─FunctionArgumentSyntax
//@[017:0039) |   | └─ObjectSyntax
//@[017:0018) |   |   ├─Token(LeftBrace) |{|
//@[018:0019) |   |   ├─Token(NewLine) |\n|
  something: myVar
//@[002:0018) |   |   ├─ObjectPropertySyntax
//@[002:0011) |   |   | ├─IdentifierSyntax
//@[002:0011) |   |   | | └─Token(Identifier) |something|
//@[011:0012) |   |   | ├─Token(Colon) |:|
//@[013:0018) |   |   | └─VariableAccessSyntax
//@[013:0018) |   |   |   └─IdentifierSyntax
//@[013:0018) |   |   |     └─Token(Identifier) |myVar|
//@[018:0019) |   |   ├─Token(NewLine) |\n|
})
//@[000:0001) |   |   └─Token(RightBrace) |}|
//@[001:0002) |   └─Token(RightParen) |)|
//@[002:0003) ├─Token(NewLine) |\n|
var myVar3 = any(any({
//@[000:0045) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0010) | ├─IdentifierSyntax
//@[004:0010) | | └─Token(Identifier) |myVar3|
//@[011:0012) | ├─Token(Assignment) |=|
//@[013:0045) | └─FunctionCallSyntax
//@[013:0016) |   ├─IdentifierSyntax
//@[013:0016) |   | └─Token(Identifier) |any|
//@[016:0017) |   ├─Token(LeftParen) |(|
//@[017:0044) |   ├─FunctionArgumentSyntax
//@[017:0044) |   | └─FunctionCallSyntax
//@[017:0020) |   |   ├─IdentifierSyntax
//@[017:0020) |   |   | └─Token(Identifier) |any|
//@[020:0021) |   |   ├─Token(LeftParen) |(|
//@[021:0043) |   |   ├─FunctionArgumentSyntax
//@[021:0043) |   |   | └─ObjectSyntax
//@[021:0022) |   |   |   ├─Token(LeftBrace) |{|
//@[022:0023) |   |   |   ├─Token(NewLine) |\n|
  something: myVar
//@[002:0018) |   |   |   ├─ObjectPropertySyntax
//@[002:0011) |   |   |   | ├─IdentifierSyntax
//@[002:0011) |   |   |   | | └─Token(Identifier) |something|
//@[011:0012) |   |   |   | ├─Token(Colon) |:|
//@[013:0018) |   |   |   | └─VariableAccessSyntax
//@[013:0018) |   |   |   |   └─IdentifierSyntax
//@[013:0018) |   |   |   |     └─Token(Identifier) |myVar|
//@[018:0019) |   |   |   ├─Token(NewLine) |\n|
}))
//@[000:0001) |   |   |   └─Token(RightBrace) |}|
//@[001:0002) |   |   └─Token(RightParen) |)|
//@[002:0003) |   └─Token(RightParen) |)|
//@[003:0004) ├─Token(NewLine) |\n|
var myVar4 = length(any(concat('s','a')))
//@[000:0041) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0010) | ├─IdentifierSyntax
//@[004:0010) | | └─Token(Identifier) |myVar4|
//@[011:0012) | ├─Token(Assignment) |=|
//@[013:0041) | └─FunctionCallSyntax
//@[013:0019) |   ├─IdentifierSyntax
//@[013:0019) |   | └─Token(Identifier) |length|
//@[019:0020) |   ├─Token(LeftParen) |(|
//@[020:0040) |   ├─FunctionArgumentSyntax
//@[020:0040) |   | └─FunctionCallSyntax
//@[020:0023) |   |   ├─IdentifierSyntax
//@[020:0023) |   |   | └─Token(Identifier) |any|
//@[023:0024) |   |   ├─Token(LeftParen) |(|
//@[024:0039) |   |   ├─FunctionArgumentSyntax
//@[024:0039) |   |   | └─FunctionCallSyntax
//@[024:0030) |   |   |   ├─IdentifierSyntax
//@[024:0030) |   |   |   | └─Token(Identifier) |concat|
//@[030:0031) |   |   |   ├─Token(LeftParen) |(|
//@[031:0034) |   |   |   ├─FunctionArgumentSyntax
//@[031:0034) |   |   |   | └─StringSyntax
//@[031:0034) |   |   |   |   └─Token(StringComplete) |'s'|
//@[034:0035) |   |   |   ├─Token(Comma) |,|
//@[035:0038) |   |   |   ├─FunctionArgumentSyntax
//@[035:0038) |   |   |   | └─StringSyntax
//@[035:0038) |   |   |   |   └─Token(StringComplete) |'a'|
//@[038:0039) |   |   |   └─Token(RightParen) |)|
//@[039:0040) |   |   └─Token(RightParen) |)|
//@[040:0041) |   └─Token(RightParen) |)|
//@[041:0043) ├─Token(NewLine) |\n\n|

// verify that unqualified banned function identifiers can be used as declaration identifiers
//@[093:0094) ├─Token(NewLine) |\n|
var variables = true
//@[000:0020) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0013) | ├─IdentifierSyntax
//@[004:0013) | | └─Token(Identifier) |variables|
//@[014:0015) | ├─Token(Assignment) |=|
//@[016:0020) | └─BooleanLiteralSyntax
//@[016:0020) |   └─Token(TrueKeyword) |true|
//@[020:0021) ├─Token(NewLine) |\n|
param parameters bool = true
//@[000:0028) ├─ParameterDeclarationSyntax
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0016) | ├─IdentifierSyntax
//@[006:0016) | | └─Token(Identifier) |parameters|
//@[017:0021) | ├─SimpleTypeSyntax
//@[017:0021) | | └─Token(Identifier) |bool|
//@[022:0028) | └─ParameterDefaultValueSyntax
//@[022:0023) |   ├─Token(Assignment) |=|
//@[024:0028) |   └─BooleanLiteralSyntax
//@[024:0028) |     └─Token(TrueKeyword) |true|
//@[028:0029) ├─Token(NewLine) |\n|
var if = true
//@[000:0013) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0006) | ├─IdentifierSyntax
//@[004:0006) | | └─Token(Identifier) |if|
//@[007:0008) | ├─Token(Assignment) |=|
//@[009:0013) | └─BooleanLiteralSyntax
//@[009:0013) |   └─Token(TrueKeyword) |true|
//@[013:0014) ├─Token(NewLine) |\n|
var createArray = true
//@[000:0022) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0015) | ├─IdentifierSyntax
//@[004:0015) | | └─Token(Identifier) |createArray|
//@[016:0017) | ├─Token(Assignment) |=|
//@[018:0022) | └─BooleanLiteralSyntax
//@[018:0022) |   └─Token(TrueKeyword) |true|
//@[022:0023) ├─Token(NewLine) |\n|
var createObject = true
//@[000:0023) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0016) | ├─IdentifierSyntax
//@[004:0016) | | └─Token(Identifier) |createObject|
//@[017:0018) | ├─Token(Assignment) |=|
//@[019:0023) | └─BooleanLiteralSyntax
//@[019:0023) |   └─Token(TrueKeyword) |true|
//@[023:0024) ├─Token(NewLine) |\n|
var add = true
//@[000:0014) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0007) | ├─IdentifierSyntax
//@[004:0007) | | └─Token(Identifier) |add|
//@[008:0009) | ├─Token(Assignment) |=|
//@[010:0014) | └─BooleanLiteralSyntax
//@[010:0014) |   └─Token(TrueKeyword) |true|
//@[014:0015) ├─Token(NewLine) |\n|
var sub = true
//@[000:0014) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0007) | ├─IdentifierSyntax
//@[004:0007) | | └─Token(Identifier) |sub|
//@[008:0009) | ├─Token(Assignment) |=|
//@[010:0014) | └─BooleanLiteralSyntax
//@[010:0014) |   └─Token(TrueKeyword) |true|
//@[014:0015) ├─Token(NewLine) |\n|
var mul = true
//@[000:0014) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0007) | ├─IdentifierSyntax
//@[004:0007) | | └─Token(Identifier) |mul|
//@[008:0009) | ├─Token(Assignment) |=|
//@[010:0014) | └─BooleanLiteralSyntax
//@[010:0014) |   └─Token(TrueKeyword) |true|
//@[014:0015) ├─Token(NewLine) |\n|
var div = true
//@[000:0014) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0007) | ├─IdentifierSyntax
//@[004:0007) | | └─Token(Identifier) |div|
//@[008:0009) | ├─Token(Assignment) |=|
//@[010:0014) | └─BooleanLiteralSyntax
//@[010:0014) |   └─Token(TrueKeyword) |true|
//@[014:0015) ├─Token(NewLine) |\n|
param mod bool = true
//@[000:0021) ├─ParameterDeclarationSyntax
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0009) | ├─IdentifierSyntax
//@[006:0009) | | └─Token(Identifier) |mod|
//@[010:0014) | ├─SimpleTypeSyntax
//@[010:0014) | | └─Token(Identifier) |bool|
//@[015:0021) | └─ParameterDefaultValueSyntax
//@[015:0016) |   ├─Token(Assignment) |=|
//@[017:0021) |   └─BooleanLiteralSyntax
//@[017:0021) |     └─Token(TrueKeyword) |true|
//@[021:0022) ├─Token(NewLine) |\n|
var less = true
//@[000:0015) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0008) | ├─IdentifierSyntax
//@[004:0008) | | └─Token(Identifier) |less|
//@[009:0010) | ├─Token(Assignment) |=|
//@[011:0015) | └─BooleanLiteralSyntax
//@[011:0015) |   └─Token(TrueKeyword) |true|
//@[015:0016) ├─Token(NewLine) |\n|
var lessOrEquals = true
//@[000:0023) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0016) | ├─IdentifierSyntax
//@[004:0016) | | └─Token(Identifier) |lessOrEquals|
//@[017:0018) | ├─Token(Assignment) |=|
//@[019:0023) | └─BooleanLiteralSyntax
//@[019:0023) |   └─Token(TrueKeyword) |true|
//@[023:0024) ├─Token(NewLine) |\n|
var greater = true
//@[000:0018) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0011) | ├─IdentifierSyntax
//@[004:0011) | | └─Token(Identifier) |greater|
//@[012:0013) | ├─Token(Assignment) |=|
//@[014:0018) | └─BooleanLiteralSyntax
//@[014:0018) |   └─Token(TrueKeyword) |true|
//@[018:0019) ├─Token(NewLine) |\n|
var greaterOrEquals = true
//@[000:0026) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0019) | ├─IdentifierSyntax
//@[004:0019) | | └─Token(Identifier) |greaterOrEquals|
//@[020:0021) | ├─Token(Assignment) |=|
//@[022:0026) | └─BooleanLiteralSyntax
//@[022:0026) |   └─Token(TrueKeyword) |true|
//@[026:0027) ├─Token(NewLine) |\n|
param equals bool = true
//@[000:0024) ├─ParameterDeclarationSyntax
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0012) | ├─IdentifierSyntax
//@[006:0012) | | └─Token(Identifier) |equals|
//@[013:0017) | ├─SimpleTypeSyntax
//@[013:0017) | | └─Token(Identifier) |bool|
//@[018:0024) | └─ParameterDefaultValueSyntax
//@[018:0019) |   ├─Token(Assignment) |=|
//@[020:0024) |   └─BooleanLiteralSyntax
//@[020:0024) |     └─Token(TrueKeyword) |true|
//@[024:0025) ├─Token(NewLine) |\n|
var not = true
//@[000:0014) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0007) | ├─IdentifierSyntax
//@[004:0007) | | └─Token(Identifier) |not|
//@[008:0009) | ├─Token(Assignment) |=|
//@[010:0014) | └─BooleanLiteralSyntax
//@[010:0014) |   └─Token(TrueKeyword) |true|
//@[014:0015) ├─Token(NewLine) |\n|
var and = true
//@[000:0014) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0007) | ├─IdentifierSyntax
//@[004:0007) | | └─Token(Identifier) |and|
//@[008:0009) | ├─Token(Assignment) |=|
//@[010:0014) | └─BooleanLiteralSyntax
//@[010:0014) |   └─Token(TrueKeyword) |true|
//@[014:0015) ├─Token(NewLine) |\n|
var or = true
//@[000:0013) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0006) | ├─IdentifierSyntax
//@[004:0006) | | └─Token(Identifier) |or|
//@[007:0008) | ├─Token(Assignment) |=|
//@[009:0013) | └─BooleanLiteralSyntax
//@[009:0013) |   └─Token(TrueKeyword) |true|
//@[013:0014) ├─Token(NewLine) |\n|
var I_WANT_IT_ALL = variables && parameters && if && createArray && createObject && add && sub && mul && div && mod && less && lessOrEquals && greater && greaterOrEquals && equals && not && and && or
//@[000:0199) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0017) | ├─IdentifierSyntax
//@[004:0017) | | └─Token(Identifier) |I_WANT_IT_ALL|
//@[018:0019) | ├─Token(Assignment) |=|
//@[020:0199) | └─BinaryOperationSyntax
//@[020:0193) |   ├─BinaryOperationSyntax
//@[020:0186) |   | ├─BinaryOperationSyntax
//@[020:0179) |   | | ├─BinaryOperationSyntax
//@[020:0169) |   | | | ├─BinaryOperationSyntax
//@[020:0150) |   | | | | ├─BinaryOperationSyntax
//@[020:0139) |   | | | | | ├─BinaryOperationSyntax
//@[020:0123) |   | | | | | | ├─BinaryOperationSyntax
//@[020:0115) |   | | | | | | | ├─BinaryOperationSyntax
//@[020:0108) |   | | | | | | | | ├─BinaryOperationSyntax
//@[020:0101) |   | | | | | | | | | ├─BinaryOperationSyntax
//@[020:0094) |   | | | | | | | | | | ├─BinaryOperationSyntax
//@[020:0087) |   | | | | | | | | | | | ├─BinaryOperationSyntax
//@[020:0080) |   | | | | | | | | | | | | ├─BinaryOperationSyntax
//@[020:0064) |   | | | | | | | | | | | | | ├─BinaryOperationSyntax
//@[020:0049) |   | | | | | | | | | | | | | | ├─BinaryOperationSyntax
//@[020:0043) |   | | | | | | | | | | | | | | | ├─BinaryOperationSyntax
//@[020:0029) |   | | | | | | | | | | | | | | | | ├─VariableAccessSyntax
//@[020:0029) |   | | | | | | | | | | | | | | | | | └─IdentifierSyntax
//@[020:0029) |   | | | | | | | | | | | | | | | | |   └─Token(Identifier) |variables|
//@[030:0032) |   | | | | | | | | | | | | | | | | ├─Token(LogicalAnd) |&&|
//@[033:0043) |   | | | | | | | | | | | | | | | | └─VariableAccessSyntax
//@[033:0043) |   | | | | | | | | | | | | | | | |   └─IdentifierSyntax
//@[033:0043) |   | | | | | | | | | | | | | | | |     └─Token(Identifier) |parameters|
//@[044:0046) |   | | | | | | | | | | | | | | | ├─Token(LogicalAnd) |&&|
//@[047:0049) |   | | | | | | | | | | | | | | | └─VariableAccessSyntax
//@[047:0049) |   | | | | | | | | | | | | | | |   └─IdentifierSyntax
//@[047:0049) |   | | | | | | | | | | | | | | |     └─Token(Identifier) |if|
//@[050:0052) |   | | | | | | | | | | | | | | ├─Token(LogicalAnd) |&&|
//@[053:0064) |   | | | | | | | | | | | | | | └─VariableAccessSyntax
//@[053:0064) |   | | | | | | | | | | | | | |   └─IdentifierSyntax
//@[053:0064) |   | | | | | | | | | | | | | |     └─Token(Identifier) |createArray|
//@[065:0067) |   | | | | | | | | | | | | | ├─Token(LogicalAnd) |&&|
//@[068:0080) |   | | | | | | | | | | | | | └─VariableAccessSyntax
//@[068:0080) |   | | | | | | | | | | | | |   └─IdentifierSyntax
//@[068:0080) |   | | | | | | | | | | | | |     └─Token(Identifier) |createObject|
//@[081:0083) |   | | | | | | | | | | | | ├─Token(LogicalAnd) |&&|
//@[084:0087) |   | | | | | | | | | | | | └─VariableAccessSyntax
//@[084:0087) |   | | | | | | | | | | | |   └─IdentifierSyntax
//@[084:0087) |   | | | | | | | | | | | |     └─Token(Identifier) |add|
//@[088:0090) |   | | | | | | | | | | | ├─Token(LogicalAnd) |&&|
//@[091:0094) |   | | | | | | | | | | | └─VariableAccessSyntax
//@[091:0094) |   | | | | | | | | | | |   └─IdentifierSyntax
//@[091:0094) |   | | | | | | | | | | |     └─Token(Identifier) |sub|
//@[095:0097) |   | | | | | | | | | | ├─Token(LogicalAnd) |&&|
//@[098:0101) |   | | | | | | | | | | └─VariableAccessSyntax
//@[098:0101) |   | | | | | | | | | |   └─IdentifierSyntax
//@[098:0101) |   | | | | | | | | | |     └─Token(Identifier) |mul|
//@[102:0104) |   | | | | | | | | | ├─Token(LogicalAnd) |&&|
//@[105:0108) |   | | | | | | | | | └─VariableAccessSyntax
//@[105:0108) |   | | | | | | | | |   └─IdentifierSyntax
//@[105:0108) |   | | | | | | | | |     └─Token(Identifier) |div|
//@[109:0111) |   | | | | | | | | ├─Token(LogicalAnd) |&&|
//@[112:0115) |   | | | | | | | | └─VariableAccessSyntax
//@[112:0115) |   | | | | | | | |   └─IdentifierSyntax
//@[112:0115) |   | | | | | | | |     └─Token(Identifier) |mod|
//@[116:0118) |   | | | | | | | ├─Token(LogicalAnd) |&&|
//@[119:0123) |   | | | | | | | └─VariableAccessSyntax
//@[119:0123) |   | | | | | | |   └─IdentifierSyntax
//@[119:0123) |   | | | | | | |     └─Token(Identifier) |less|
//@[124:0126) |   | | | | | | ├─Token(LogicalAnd) |&&|
//@[127:0139) |   | | | | | | └─VariableAccessSyntax
//@[127:0139) |   | | | | | |   └─IdentifierSyntax
//@[127:0139) |   | | | | | |     └─Token(Identifier) |lessOrEquals|
//@[140:0142) |   | | | | | ├─Token(LogicalAnd) |&&|
//@[143:0150) |   | | | | | └─VariableAccessSyntax
//@[143:0150) |   | | | | |   └─IdentifierSyntax
//@[143:0150) |   | | | | |     └─Token(Identifier) |greater|
//@[151:0153) |   | | | | ├─Token(LogicalAnd) |&&|
//@[154:0169) |   | | | | └─VariableAccessSyntax
//@[154:0169) |   | | | |   └─IdentifierSyntax
//@[154:0169) |   | | | |     └─Token(Identifier) |greaterOrEquals|
//@[170:0172) |   | | | ├─Token(LogicalAnd) |&&|
//@[173:0179) |   | | | └─VariableAccessSyntax
//@[173:0179) |   | | |   └─IdentifierSyntax
//@[173:0179) |   | | |     └─Token(Identifier) |equals|
//@[180:0182) |   | | ├─Token(LogicalAnd) |&&|
//@[183:0186) |   | | └─VariableAccessSyntax
//@[183:0186) |   | |   └─IdentifierSyntax
//@[183:0186) |   | |     └─Token(Identifier) |not|
//@[187:0189) |   | ├─Token(LogicalAnd) |&&|
//@[190:0193) |   | └─VariableAccessSyntax
//@[190:0193) |   |   └─IdentifierSyntax
//@[190:0193) |   |     └─Token(Identifier) |and|
//@[194:0196) |   ├─Token(LogicalAnd) |&&|
//@[197:0199) |   └─VariableAccessSyntax
//@[197:0199) |     └─IdentifierSyntax
//@[197:0199) |       └─Token(Identifier) |or|
//@[199:0201) ├─Token(NewLine) |\n\n|

// identifiers can have underscores
//@[035:0036) ├─Token(NewLine) |\n|
var _ = 3
//@[000:0009) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0005) | ├─IdentifierSyntax
//@[004:0005) | | └─Token(Identifier) |_|
//@[006:0007) | ├─Token(Assignment) |=|
//@[008:0009) | └─IntegerLiteralSyntax
//@[008:0009) |   └─Token(Integer) |3|
//@[009:0010) ├─Token(NewLine) |\n|
var __ = 10 * _
//@[000:0015) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0006) | ├─IdentifierSyntax
//@[004:0006) | | └─Token(Identifier) |__|
//@[007:0008) | ├─Token(Assignment) |=|
//@[009:0015) | └─BinaryOperationSyntax
//@[009:0011) |   ├─IntegerLiteralSyntax
//@[009:0011) |   | └─Token(Integer) |10|
//@[012:0013) |   ├─Token(Asterisk) |*|
//@[014:0015) |   └─VariableAccessSyntax
//@[014:0015) |     └─IdentifierSyntax
//@[014:0015) |       └─Token(Identifier) |_|
//@[015:0016) ├─Token(NewLine) |\n|
var _0a_1b = true
//@[000:0017) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0010) | ├─IdentifierSyntax
//@[004:0010) | | └─Token(Identifier) |_0a_1b|
//@[011:0012) | ├─Token(Assignment) |=|
//@[013:0017) | └─BooleanLiteralSyntax
//@[013:0017) |   └─Token(TrueKeyword) |true|
//@[017:0018) ├─Token(NewLine) |\n|
var _1_ = _0a_1b || (__ + _ % 2 == 0)
//@[000:0037) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0007) | ├─IdentifierSyntax
//@[004:0007) | | └─Token(Identifier) |_1_|
//@[008:0009) | ├─Token(Assignment) |=|
//@[010:0037) | └─BinaryOperationSyntax
//@[010:0016) |   ├─VariableAccessSyntax
//@[010:0016) |   | └─IdentifierSyntax
//@[010:0016) |   |   └─Token(Identifier) |_0a_1b|
//@[017:0019) |   ├─Token(LogicalOr) ||||
//@[020:0037) |   └─ParenthesizedExpressionSyntax
//@[020:0021) |     ├─Token(LeftParen) |(|
//@[021:0036) |     ├─BinaryOperationSyntax
//@[021:0031) |     | ├─BinaryOperationSyntax
//@[021:0023) |     | | ├─VariableAccessSyntax
//@[021:0023) |     | | | └─IdentifierSyntax
//@[021:0023) |     | | |   └─Token(Identifier) |__|
//@[024:0025) |     | | ├─Token(Plus) |+|
//@[026:0031) |     | | └─BinaryOperationSyntax
//@[026:0027) |     | |   ├─VariableAccessSyntax
//@[026:0027) |     | |   | └─IdentifierSyntax
//@[026:0027) |     | |   |   └─Token(Identifier) |_|
//@[028:0029) |     | |   ├─Token(Modulo) |%|
//@[030:0031) |     | |   └─IntegerLiteralSyntax
//@[030:0031) |     | |     └─Token(Integer) |2|
//@[032:0034) |     | ├─Token(Equals) |==|
//@[035:0036) |     | └─IntegerLiteralSyntax
//@[035:0036) |     |   └─Token(Integer) |0|
//@[036:0037) |     └─Token(RightParen) |)|
//@[037:0039) ├─Token(NewLine) |\n\n|

// fully qualified access
//@[025:0026) ├─Token(NewLine) |\n|
var resourceGroup = 'something'
//@[000:0031) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0017) | ├─IdentifierSyntax
//@[004:0017) | | └─Token(Identifier) |resourceGroup|
//@[018:0019) | ├─Token(Assignment) |=|
//@[020:0031) | └─StringSyntax
//@[020:0031) |   └─Token(StringComplete) |'something'|
//@[031:0032) ├─Token(NewLine) |\n|
var resourceGroupName = az.resourceGroup().name
//@[000:0047) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0021) | ├─IdentifierSyntax
//@[004:0021) | | └─Token(Identifier) |resourceGroupName|
//@[022:0023) | ├─Token(Assignment) |=|
//@[024:0047) | └─PropertyAccessSyntax
//@[024:0042) |   ├─InstanceFunctionCallSyntax
//@[024:0026) |   | ├─VariableAccessSyntax
//@[024:0026) |   | | └─IdentifierSyntax
//@[024:0026) |   | |   └─Token(Identifier) |az|
//@[026:0027) |   | ├─Token(Dot) |.|
//@[027:0040) |   | ├─IdentifierSyntax
//@[027:0040) |   | | └─Token(Identifier) |resourceGroup|
//@[040:0041) |   | ├─Token(LeftParen) |(|
//@[041:0042) |   | └─Token(RightParen) |)|
//@[042:0043) |   ├─Token(Dot) |.|
//@[043:0047) |   └─IdentifierSyntax
//@[043:0047) |     └─Token(Identifier) |name|
//@[047:0048) ├─Token(NewLine) |\n|
var resourceGroupObject = az.resourceGroup()
//@[000:0044) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0023) | ├─IdentifierSyntax
//@[004:0023) | | └─Token(Identifier) |resourceGroupObject|
//@[024:0025) | ├─Token(Assignment) |=|
//@[026:0044) | └─InstanceFunctionCallSyntax
//@[026:0028) |   ├─VariableAccessSyntax
//@[026:0028) |   | └─IdentifierSyntax
//@[026:0028) |   |   └─Token(Identifier) |az|
//@[028:0029) |   ├─Token(Dot) |.|
//@[029:0042) |   ├─IdentifierSyntax
//@[029:0042) |   | └─Token(Identifier) |resourceGroup|
//@[042:0043) |   ├─Token(LeftParen) |(|
//@[043:0044) |   └─Token(RightParen) |)|
//@[044:0045) ├─Token(NewLine) |\n|
var propertyAccessFromObject = resourceGroupObject.name
//@[000:0055) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0028) | ├─IdentifierSyntax
//@[004:0028) | | └─Token(Identifier) |propertyAccessFromObject|
//@[029:0030) | ├─Token(Assignment) |=|
//@[031:0055) | └─PropertyAccessSyntax
//@[031:0050) |   ├─VariableAccessSyntax
//@[031:0050) |   | └─IdentifierSyntax
//@[031:0050) |   |   └─Token(Identifier) |resourceGroupObject|
//@[050:0051) |   ├─Token(Dot) |.|
//@[051:0055) |   └─IdentifierSyntax
//@[051:0055) |     └─Token(Identifier) |name|
//@[055:0056) ├─Token(NewLine) |\n|
var isTrue = sys.max(1, 2) == 3
//@[000:0031) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0010) | ├─IdentifierSyntax
//@[004:0010) | | └─Token(Identifier) |isTrue|
//@[011:0012) | ├─Token(Assignment) |=|
//@[013:0031) | └─BinaryOperationSyntax
//@[013:0026) |   ├─InstanceFunctionCallSyntax
//@[013:0016) |   | ├─VariableAccessSyntax
//@[013:0016) |   | | └─IdentifierSyntax
//@[013:0016) |   | |   └─Token(Identifier) |sys|
//@[016:0017) |   | ├─Token(Dot) |.|
//@[017:0020) |   | ├─IdentifierSyntax
//@[017:0020) |   | | └─Token(Identifier) |max|
//@[020:0021) |   | ├─Token(LeftParen) |(|
//@[021:0022) |   | ├─FunctionArgumentSyntax
//@[021:0022) |   | | └─IntegerLiteralSyntax
//@[021:0022) |   | |   └─Token(Integer) |1|
//@[022:0023) |   | ├─Token(Comma) |,|
//@[024:0025) |   | ├─FunctionArgumentSyntax
//@[024:0025) |   | | └─IntegerLiteralSyntax
//@[024:0025) |   | |   └─Token(Integer) |2|
//@[025:0026) |   | └─Token(RightParen) |)|
//@[027:0029) |   ├─Token(Equals) |==|
//@[030:0031) |   └─IntegerLiteralSyntax
//@[030:0031) |     └─Token(Integer) |3|
//@[031:0032) ├─Token(NewLine) |\n|
var isFalse = !isTrue
//@[000:0021) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0011) | ├─IdentifierSyntax
//@[004:0011) | | └─Token(Identifier) |isFalse|
//@[012:0013) | ├─Token(Assignment) |=|
//@[014:0021) | └─UnaryOperationSyntax
//@[014:0015) |   ├─Token(Exclamation) |!|
//@[015:0021) |   └─VariableAccessSyntax
//@[015:0021) |     └─IdentifierSyntax
//@[015:0021) |       └─Token(Identifier) |isTrue|
//@[021:0022) ├─Token(NewLine) |\n|
var someText = isTrue ? sys.concat('a', sys.concat('b', 'c')) : 'someText'
//@[000:0074) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0012) | ├─IdentifierSyntax
//@[004:0012) | | └─Token(Identifier) |someText|
//@[013:0014) | ├─Token(Assignment) |=|
//@[015:0074) | └─TernaryOperationSyntax
//@[015:0021) |   ├─VariableAccessSyntax
//@[015:0021) |   | └─IdentifierSyntax
//@[015:0021) |   |   └─Token(Identifier) |isTrue|
//@[022:0023) |   ├─Token(Question) |?|
//@[024:0061) |   ├─InstanceFunctionCallSyntax
//@[024:0027) |   | ├─VariableAccessSyntax
//@[024:0027) |   | | └─IdentifierSyntax
//@[024:0027) |   | |   └─Token(Identifier) |sys|
//@[027:0028) |   | ├─Token(Dot) |.|
//@[028:0034) |   | ├─IdentifierSyntax
//@[028:0034) |   | | └─Token(Identifier) |concat|
//@[034:0035) |   | ├─Token(LeftParen) |(|
//@[035:0038) |   | ├─FunctionArgumentSyntax
//@[035:0038) |   | | └─StringSyntax
//@[035:0038) |   | |   └─Token(StringComplete) |'a'|
//@[038:0039) |   | ├─Token(Comma) |,|
//@[040:0060) |   | ├─FunctionArgumentSyntax
//@[040:0060) |   | | └─InstanceFunctionCallSyntax
//@[040:0043) |   | |   ├─VariableAccessSyntax
//@[040:0043) |   | |   | └─IdentifierSyntax
//@[040:0043) |   | |   |   └─Token(Identifier) |sys|
//@[043:0044) |   | |   ├─Token(Dot) |.|
//@[044:0050) |   | |   ├─IdentifierSyntax
//@[044:0050) |   | |   | └─Token(Identifier) |concat|
//@[050:0051) |   | |   ├─Token(LeftParen) |(|
//@[051:0054) |   | |   ├─FunctionArgumentSyntax
//@[051:0054) |   | |   | └─StringSyntax
//@[051:0054) |   | |   |   └─Token(StringComplete) |'b'|
//@[054:0055) |   | |   ├─Token(Comma) |,|
//@[056:0059) |   | |   ├─FunctionArgumentSyntax
//@[056:0059) |   | |   | └─StringSyntax
//@[056:0059) |   | |   |   └─Token(StringComplete) |'c'|
//@[059:0060) |   | |   └─Token(RightParen) |)|
//@[060:0061) |   | └─Token(RightParen) |)|
//@[062:0063) |   ├─Token(Colon) |:|
//@[064:0074) |   └─StringSyntax
//@[064:0074) |     └─Token(StringComplete) |'someText'|
//@[074:0076) ├─Token(NewLine) |\n\n|

// Bicep functions that cannot be converted into ARM functions
//@[062:0063) ├─Token(NewLine) |\n|
var scopesWithoutArmRepresentation = {
//@[000:0195) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0034) | ├─IdentifierSyntax
//@[004:0034) | | └─Token(Identifier) |scopesWithoutArmRepresentation|
//@[035:0036) | ├─Token(Assignment) |=|
//@[037:0195) | └─ObjectSyntax
//@[037:0038) |   ├─Token(LeftBrace) |{|
//@[038:0039) |   ├─Token(NewLine) |\n|
  subscription: subscription('10b57a01-6350-4ce2-972a-6a13642f00bf')
//@[002:0068) |   ├─ObjectPropertySyntax
//@[002:0014) |   | ├─IdentifierSyntax
//@[002:0014) |   | | └─Token(Identifier) |subscription|
//@[014:0015) |   | ├─Token(Colon) |:|
//@[016:0068) |   | └─FunctionCallSyntax
//@[016:0028) |   |   ├─IdentifierSyntax
//@[016:0028) |   |   | └─Token(Identifier) |subscription|
//@[028:0029) |   |   ├─Token(LeftParen) |(|
//@[029:0067) |   |   ├─FunctionArgumentSyntax
//@[029:0067) |   |   | └─StringSyntax
//@[029:0067) |   |   |   └─Token(StringComplete) |'10b57a01-6350-4ce2-972a-6a13642f00bf'|
//@[067:0068) |   |   └─Token(RightParen) |)|
//@[068:0069) |   ├─Token(NewLine) |\n|
  resourceGroup: az.resourceGroup('10b57a01-6350-4ce2-972a-6a13642f00bf', 'myRgName')
//@[002:0085) |   ├─ObjectPropertySyntax
//@[002:0015) |   | ├─IdentifierSyntax
//@[002:0015) |   | | └─Token(Identifier) |resourceGroup|
//@[015:0016) |   | ├─Token(Colon) |:|
//@[017:0085) |   | └─InstanceFunctionCallSyntax
//@[017:0019) |   |   ├─VariableAccessSyntax
//@[017:0019) |   |   | └─IdentifierSyntax
//@[017:0019) |   |   |   └─Token(Identifier) |az|
//@[019:0020) |   |   ├─Token(Dot) |.|
//@[020:0033) |   |   ├─IdentifierSyntax
//@[020:0033) |   |   | └─Token(Identifier) |resourceGroup|
//@[033:0034) |   |   ├─Token(LeftParen) |(|
//@[034:0072) |   |   ├─FunctionArgumentSyntax
//@[034:0072) |   |   | └─StringSyntax
//@[034:0072) |   |   |   └─Token(StringComplete) |'10b57a01-6350-4ce2-972a-6a13642f00bf'|
//@[072:0073) |   |   ├─Token(Comma) |,|
//@[074:0084) |   |   ├─FunctionArgumentSyntax
//@[074:0084) |   |   | └─StringSyntax
//@[074:0084) |   |   |   └─Token(StringComplete) |'myRgName'|
//@[084:0085) |   |   └─Token(RightParen) |)|
//@[085:0086) |   ├─Token(NewLine) |\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0003) ├─Token(NewLine) |\n\n|

var scopesWithArmRepresentation = {
//@[000:0123) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0031) | ├─IdentifierSyntax
//@[004:0031) | | └─Token(Identifier) |scopesWithArmRepresentation|
//@[032:0033) | ├─Token(Assignment) |=|
//@[034:0123) | └─ObjectSyntax
//@[034:0035) |   ├─Token(LeftBrace) |{|
//@[035:0036) |   ├─Token(NewLine) |\n|
  tenant: tenant()
//@[002:0018) |   ├─ObjectPropertySyntax
//@[002:0008) |   | ├─IdentifierSyntax
//@[002:0008) |   | | └─Token(Identifier) |tenant|
//@[008:0009) |   | ├─Token(Colon) |:|
//@[010:0018) |   | └─FunctionCallSyntax
//@[010:0016) |   |   ├─IdentifierSyntax
//@[010:0016) |   |   | └─Token(Identifier) |tenant|
//@[016:0017) |   |   ├─Token(LeftParen) |(|
//@[017:0018) |   |   └─Token(RightParen) |)|
//@[018:0019) |   ├─Token(NewLine) |\n|
  subscription: subscription()
//@[002:0030) |   ├─ObjectPropertySyntax
//@[002:0014) |   | ├─IdentifierSyntax
//@[002:0014) |   | | └─Token(Identifier) |subscription|
//@[014:0015) |   | ├─Token(Colon) |:|
//@[016:0030) |   | └─FunctionCallSyntax
//@[016:0028) |   |   ├─IdentifierSyntax
//@[016:0028) |   |   | └─Token(Identifier) |subscription|
//@[028:0029) |   |   ├─Token(LeftParen) |(|
//@[029:0030) |   |   └─Token(RightParen) |)|
//@[030:0031) |   ├─Token(NewLine) |\n|
  resourceGroup: az.resourceGroup()
//@[002:0035) |   ├─ObjectPropertySyntax
//@[002:0015) |   | ├─IdentifierSyntax
//@[002:0015) |   | | └─Token(Identifier) |resourceGroup|
//@[015:0016) |   | ├─Token(Colon) |:|
//@[017:0035) |   | └─InstanceFunctionCallSyntax
//@[017:0019) |   |   ├─VariableAccessSyntax
//@[017:0019) |   |   | └─IdentifierSyntax
//@[017:0019) |   |   |   └─Token(Identifier) |az|
//@[019:0020) |   |   ├─Token(Dot) |.|
//@[020:0033) |   |   ├─IdentifierSyntax
//@[020:0033) |   |   | └─Token(Identifier) |resourceGroup|
//@[033:0034) |   |   ├─Token(LeftParen) |(|
//@[034:0035) |   |   └─Token(RightParen) |)|
//@[035:0036) |   ├─Token(NewLine) |\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0003) ├─Token(NewLine) |\n\n|

// Issue #1332
//@[014:0015) ├─Token(NewLine) |\n|
var issue1332_propname = 'ptest'
//@[000:0032) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0022) | ├─IdentifierSyntax
//@[004:0022) | | └─Token(Identifier) |issue1332_propname|
//@[023:0024) | ├─Token(Assignment) |=|
//@[025:0032) | └─StringSyntax
//@[025:0032) |   └─Token(StringComplete) |'ptest'|
//@[032:0033) ├─Token(NewLine) |\n|
var issue1332 = true ? {
//@[000:0086) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0013) | ├─IdentifierSyntax
//@[004:0013) | | └─Token(Identifier) |issue1332|
//@[014:0015) | ├─Token(Assignment) |=|
//@[016:0086) | └─TernaryOperationSyntax
//@[016:0020) |   ├─BooleanLiteralSyntax
//@[016:0020) |   | └─Token(TrueKeyword) |true|
//@[021:0022) |   ├─Token(Question) |?|
//@[023:0081) |   ├─ObjectSyntax
//@[023:0024) |   | ├─Token(LeftBrace) |{|
//@[024:0025) |   | ├─Token(NewLine) |\n|
    prop1: {
//@[004:0054) |   | ├─ObjectPropertySyntax
//@[004:0009) |   | | ├─IdentifierSyntax
//@[004:0009) |   | | | └─Token(Identifier) |prop1|
//@[009:0010) |   | | ├─Token(Colon) |:|
//@[011:0054) |   | | └─ObjectSyntax
//@[011:0012) |   | |   ├─Token(LeftBrace) |{|
//@[012:0013) |   | |   ├─Token(NewLine) |\n|
        '${issue1332_propname}': {}
//@[008:0035) |   | |   ├─ObjectPropertySyntax
//@[008:0031) |   | |   | ├─StringSyntax
//@[008:0011) |   | |   | | ├─Token(StringLeftPiece) |'${|
//@[011:0029) |   | |   | | ├─VariableAccessSyntax
//@[011:0029) |   | |   | | | └─IdentifierSyntax
//@[011:0029) |   | |   | | |   └─Token(Identifier) |issue1332_propname|
//@[029:0031) |   | |   | | └─Token(StringRightPiece) |}'|
//@[031:0032) |   | |   | ├─Token(Colon) |:|
//@[033:0035) |   | |   | └─ObjectSyntax
//@[033:0034) |   | |   |   ├─Token(LeftBrace) |{|
//@[034:0035) |   | |   |   └─Token(RightBrace) |}|
//@[035:0036) |   | |   ├─Token(NewLine) |\n|
    }
//@[004:0005) |   | |   └─Token(RightBrace) |}|
//@[005:0006) |   | ├─Token(NewLine) |\n|
} : {}
//@[000:0001) |   | └─Token(RightBrace) |}|
//@[002:0003) |   ├─Token(Colon) |:|
//@[004:0006) |   └─ObjectSyntax
//@[004:0005) |     ├─Token(LeftBrace) |{|
//@[005:0006) |     └─Token(RightBrace) |}|
//@[006:0008) ├─Token(NewLine) |\n\n|

// Issue #486
//@[013:0014) ├─Token(NewLine) |\n|
var myBigInt = 2199023255552
//@[000:0028) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0012) | ├─IdentifierSyntax
//@[004:0012) | | └─Token(Identifier) |myBigInt|
//@[013:0014) | ├─Token(Assignment) |=|
//@[015:0028) | └─IntegerLiteralSyntax
//@[015:0028) |   └─Token(Integer) |2199023255552|
//@[028:0029) ├─Token(NewLine) |\n|
var myIntExpression = 5 * 5
//@[000:0027) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0019) | ├─IdentifierSyntax
//@[004:0019) | | └─Token(Identifier) |myIntExpression|
//@[020:0021) | ├─Token(Assignment) |=|
//@[022:0027) | └─BinaryOperationSyntax
//@[022:0023) |   ├─IntegerLiteralSyntax
//@[022:0023) |   | └─Token(Integer) |5|
//@[024:0025) |   ├─Token(Asterisk) |*|
//@[026:0027) |   └─IntegerLiteralSyntax
//@[026:0027) |     └─Token(Integer) |5|
//@[027:0028) ├─Token(NewLine) |\n|
var myBigIntExpression = 2199023255552 * 2
//@[000:0042) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0022) | ├─IdentifierSyntax
//@[004:0022) | | └─Token(Identifier) |myBigIntExpression|
//@[023:0024) | ├─Token(Assignment) |=|
//@[025:0042) | └─BinaryOperationSyntax
//@[025:0038) |   ├─IntegerLiteralSyntax
//@[025:0038) |   | └─Token(Integer) |2199023255552|
//@[039:0040) |   ├─Token(Asterisk) |*|
//@[041:0042) |   └─IntegerLiteralSyntax
//@[041:0042) |     └─Token(Integer) |2|
//@[042:0043) ├─Token(NewLine) |\n|
var myBigIntExpression2 = 2199023255552 * 2199023255552
//@[000:0055) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0023) | ├─IdentifierSyntax
//@[004:0023) | | └─Token(Identifier) |myBigIntExpression2|
//@[024:0025) | ├─Token(Assignment) |=|
//@[026:0055) | └─BinaryOperationSyntax
//@[026:0039) |   ├─IntegerLiteralSyntax
//@[026:0039) |   | └─Token(Integer) |2199023255552|
//@[040:0041) |   ├─Token(Asterisk) |*|
//@[042:0055) |   └─IntegerLiteralSyntax
//@[042:0055) |     └─Token(Integer) |2199023255552|
//@[055:0057) ├─Token(NewLine) |\n\n|

// variable loops
//@[017:0018) ├─Token(NewLine) |\n|
var incrementingNumbers = [for i in range(0,10) : i]
//@[000:0052) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0023) | ├─IdentifierSyntax
//@[004:0023) | | └─Token(Identifier) |incrementingNumbers|
//@[024:0025) | ├─Token(Assignment) |=|
//@[026:0052) | └─ForSyntax
//@[026:0027) |   ├─Token(LeftSquare) |[|
//@[027:0030) |   ├─Token(Identifier) |for|
//@[031:0032) |   ├─LocalVariableSyntax
//@[031:0032) |   | └─IdentifierSyntax
//@[031:0032) |   |   └─Token(Identifier) |i|
//@[033:0035) |   ├─Token(Identifier) |in|
//@[036:0047) |   ├─FunctionCallSyntax
//@[036:0041) |   | ├─IdentifierSyntax
//@[036:0041) |   | | └─Token(Identifier) |range|
//@[041:0042) |   | ├─Token(LeftParen) |(|
//@[042:0043) |   | ├─FunctionArgumentSyntax
//@[042:0043) |   | | └─IntegerLiteralSyntax
//@[042:0043) |   | |   └─Token(Integer) |0|
//@[043:0044) |   | ├─Token(Comma) |,|
//@[044:0046) |   | ├─FunctionArgumentSyntax
//@[044:0046) |   | | └─IntegerLiteralSyntax
//@[044:0046) |   | |   └─Token(Integer) |10|
//@[046:0047) |   | └─Token(RightParen) |)|
//@[048:0049) |   ├─Token(Colon) |:|
//@[050:0051) |   ├─VariableAccessSyntax
//@[050:0051) |   | └─IdentifierSyntax
//@[050:0051) |   |   └─Token(Identifier) |i|
//@[051:0052) |   └─Token(RightSquare) |]|
//@[052:0053) ├─Token(NewLine) |\n|
var loopInput = [
//@[000:0035) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0013) | ├─IdentifierSyntax
//@[004:0013) | | └─Token(Identifier) |loopInput|
//@[014:0015) | ├─Token(Assignment) |=|
//@[016:0035) | └─ArraySyntax
//@[016:0017) |   ├─Token(LeftSquare) |[|
//@[017:0018) |   ├─Token(NewLine) |\n|
  'one'
//@[002:0007) |   ├─ArrayItemSyntax
//@[002:0007) |   | └─StringSyntax
//@[002:0007) |   |   └─Token(StringComplete) |'one'|
//@[007:0008) |   ├─Token(NewLine) |\n|
  'two'
//@[002:0007) |   ├─ArrayItemSyntax
//@[002:0007) |   | └─StringSyntax
//@[002:0007) |   |   └─Token(StringComplete) |'two'|
//@[007:0008) |   ├─Token(NewLine) |\n|
]
//@[000:0001) |   └─Token(RightSquare) |]|
//@[001:0002) ├─Token(NewLine) |\n|
var arrayOfStringsViaLoop = [for (name, i) in loopInput: 'prefix-${i}-${name}']
//@[000:0079) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0025) | ├─IdentifierSyntax
//@[004:0025) | | └─Token(Identifier) |arrayOfStringsViaLoop|
//@[026:0027) | ├─Token(Assignment) |=|
//@[028:0079) | └─ForSyntax
//@[028:0029) |   ├─Token(LeftSquare) |[|
//@[029:0032) |   ├─Token(Identifier) |for|
//@[033:0042) |   ├─VariableBlockSyntax
//@[033:0034) |   | ├─Token(LeftParen) |(|
//@[034:0038) |   | ├─LocalVariableSyntax
//@[034:0038) |   | | └─IdentifierSyntax
//@[034:0038) |   | |   └─Token(Identifier) |name|
//@[038:0039) |   | ├─Token(Comma) |,|
//@[040:0041) |   | ├─LocalVariableSyntax
//@[040:0041) |   | | └─IdentifierSyntax
//@[040:0041) |   | |   └─Token(Identifier) |i|
//@[041:0042) |   | └─Token(RightParen) |)|
//@[043:0045) |   ├─Token(Identifier) |in|
//@[046:0055) |   ├─VariableAccessSyntax
//@[046:0055) |   | └─IdentifierSyntax
//@[046:0055) |   |   └─Token(Identifier) |loopInput|
//@[055:0056) |   ├─Token(Colon) |:|
//@[057:0078) |   ├─StringSyntax
//@[057:0067) |   | ├─Token(StringLeftPiece) |'prefix-${|
//@[067:0068) |   | ├─VariableAccessSyntax
//@[067:0068) |   | | └─IdentifierSyntax
//@[067:0068) |   | |   └─Token(Identifier) |i|
//@[068:0072) |   | ├─Token(StringMiddlePiece) |}-${|
//@[072:0076) |   | ├─VariableAccessSyntax
//@[072:0076) |   | | └─IdentifierSyntax
//@[072:0076) |   | |   └─Token(Identifier) |name|
//@[076:0078) |   | └─Token(StringRightPiece) |}'|
//@[078:0079) |   └─Token(RightSquare) |]|
//@[079:0080) ├─Token(NewLine) |\n|
var arrayOfObjectsViaLoop = [for (name, i) in loopInput: {
//@[000:0123) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0025) | ├─IdentifierSyntax
//@[004:0025) | | └─Token(Identifier) |arrayOfObjectsViaLoop|
//@[026:0027) | ├─Token(Assignment) |=|
//@[028:0123) | └─ForSyntax
//@[028:0029) |   ├─Token(LeftSquare) |[|
//@[029:0032) |   ├─Token(Identifier) |for|
//@[033:0042) |   ├─VariableBlockSyntax
//@[033:0034) |   | ├─Token(LeftParen) |(|
//@[034:0038) |   | ├─LocalVariableSyntax
//@[034:0038) |   | | └─IdentifierSyntax
//@[034:0038) |   | |   └─Token(Identifier) |name|
//@[038:0039) |   | ├─Token(Comma) |,|
//@[040:0041) |   | ├─LocalVariableSyntax
//@[040:0041) |   | | └─IdentifierSyntax
//@[040:0041) |   | |   └─Token(Identifier) |i|
//@[041:0042) |   | └─Token(RightParen) |)|
//@[043:0045) |   ├─Token(Identifier) |in|
//@[046:0055) |   ├─VariableAccessSyntax
//@[046:0055) |   | └─IdentifierSyntax
//@[046:0055) |   |   └─Token(Identifier) |loopInput|
//@[055:0056) |   ├─Token(Colon) |:|
//@[057:0122) |   ├─ObjectSyntax
//@[057:0058) |   | ├─Token(LeftBrace) |{|
//@[058:0059) |   | ├─Token(NewLine) |\n|
  index: i
//@[002:0010) |   | ├─ObjectPropertySyntax
//@[002:0007) |   | | ├─IdentifierSyntax
//@[002:0007) |   | | | └─Token(Identifier) |index|
//@[007:0008) |   | | ├─Token(Colon) |:|
//@[009:0010) |   | | └─VariableAccessSyntax
//@[009:0010) |   | |   └─IdentifierSyntax
//@[009:0010) |   | |     └─Token(Identifier) |i|
//@[010:0011) |   | ├─Token(NewLine) |\n|
  name: name
//@[002:0012) |   | ├─ObjectPropertySyntax
//@[002:0006) |   | | ├─IdentifierSyntax
//@[002:0006) |   | | | └─Token(Identifier) |name|
//@[006:0007) |   | | ├─Token(Colon) |:|
//@[008:0012) |   | | └─VariableAccessSyntax
//@[008:0012) |   | |   └─IdentifierSyntax
//@[008:0012) |   | |     └─Token(Identifier) |name|
//@[012:0013) |   | ├─Token(NewLine) |\n|
  value: 'prefix-${i}-${name}-suffix'
//@[002:0037) |   | ├─ObjectPropertySyntax
//@[002:0007) |   | | ├─IdentifierSyntax
//@[002:0007) |   | | | └─Token(Identifier) |value|
//@[007:0008) |   | | ├─Token(Colon) |:|
//@[009:0037) |   | | └─StringSyntax
//@[009:0019) |   | |   ├─Token(StringLeftPiece) |'prefix-${|
//@[019:0020) |   | |   ├─VariableAccessSyntax
//@[019:0020) |   | |   | └─IdentifierSyntax
//@[019:0020) |   | |   |   └─Token(Identifier) |i|
//@[020:0024) |   | |   ├─Token(StringMiddlePiece) |}-${|
//@[024:0028) |   | |   ├─VariableAccessSyntax
//@[024:0028) |   | |   | └─IdentifierSyntax
//@[024:0028) |   | |   |   └─Token(Identifier) |name|
//@[028:0037) |   | |   └─Token(StringRightPiece) |}-suffix'|
//@[037:0038) |   | ├─Token(NewLine) |\n|
}]
//@[000:0001) |   | └─Token(RightBrace) |}|
//@[001:0002) |   └─Token(RightSquare) |]|
//@[002:0003) ├─Token(NewLine) |\n|
var arrayOfArraysViaLoop = [for (name, i) in loopInput: [
//@[000:0102) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0024) | ├─IdentifierSyntax
//@[004:0024) | | └─Token(Identifier) |arrayOfArraysViaLoop|
//@[025:0026) | ├─Token(Assignment) |=|
//@[027:0102) | └─ForSyntax
//@[027:0028) |   ├─Token(LeftSquare) |[|
//@[028:0031) |   ├─Token(Identifier) |for|
//@[032:0041) |   ├─VariableBlockSyntax
//@[032:0033) |   | ├─Token(LeftParen) |(|
//@[033:0037) |   | ├─LocalVariableSyntax
//@[033:0037) |   | | └─IdentifierSyntax
//@[033:0037) |   | |   └─Token(Identifier) |name|
//@[037:0038) |   | ├─Token(Comma) |,|
//@[039:0040) |   | ├─LocalVariableSyntax
//@[039:0040) |   | | └─IdentifierSyntax
//@[039:0040) |   | |   └─Token(Identifier) |i|
//@[040:0041) |   | └─Token(RightParen) |)|
//@[042:0044) |   ├─Token(Identifier) |in|
//@[045:0054) |   ├─VariableAccessSyntax
//@[045:0054) |   | └─IdentifierSyntax
//@[045:0054) |   |   └─Token(Identifier) |loopInput|
//@[054:0055) |   ├─Token(Colon) |:|
//@[056:0101) |   ├─ArraySyntax
//@[056:0057) |   | ├─Token(LeftSquare) |[|
//@[057:0058) |   | ├─Token(NewLine) |\n|
  i
//@[002:0003) |   | ├─ArrayItemSyntax
//@[002:0003) |   | | └─VariableAccessSyntax
//@[002:0003) |   | |   └─IdentifierSyntax
//@[002:0003) |   | |     └─Token(Identifier) |i|
//@[003:0004) |   | ├─Token(NewLine) |\n|
  name
//@[002:0006) |   | ├─ArrayItemSyntax
//@[002:0006) |   | | └─VariableAccessSyntax
//@[002:0006) |   | |   └─IdentifierSyntax
//@[002:0006) |   | |     └─Token(Identifier) |name|
//@[006:0007) |   | ├─Token(NewLine) |\n|
  'prefix-${i}-${name}-suffix'
//@[002:0030) |   | ├─ArrayItemSyntax
//@[002:0030) |   | | └─StringSyntax
//@[002:0012) |   | |   ├─Token(StringLeftPiece) |'prefix-${|
//@[012:0013) |   | |   ├─VariableAccessSyntax
//@[012:0013) |   | |   | └─IdentifierSyntax
//@[012:0013) |   | |   |   └─Token(Identifier) |i|
//@[013:0017) |   | |   ├─Token(StringMiddlePiece) |}-${|
//@[017:0021) |   | |   ├─VariableAccessSyntax
//@[017:0021) |   | |   | └─IdentifierSyntax
//@[017:0021) |   | |   |   └─Token(Identifier) |name|
//@[021:0030) |   | |   └─Token(StringRightPiece) |}-suffix'|
//@[030:0031) |   | ├─Token(NewLine) |\n|
]]
//@[000:0001) |   | └─Token(RightSquare) |]|
//@[001:0002) |   └─Token(RightSquare) |]|
//@[002:0003) ├─Token(NewLine) |\n|
var arrayOfBooleans = [for (name, i) in loopInput: i % 2 == 0]
//@[000:0062) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0019) | ├─IdentifierSyntax
//@[004:0019) | | └─Token(Identifier) |arrayOfBooleans|
//@[020:0021) | ├─Token(Assignment) |=|
//@[022:0062) | └─ForSyntax
//@[022:0023) |   ├─Token(LeftSquare) |[|
//@[023:0026) |   ├─Token(Identifier) |for|
//@[027:0036) |   ├─VariableBlockSyntax
//@[027:0028) |   | ├─Token(LeftParen) |(|
//@[028:0032) |   | ├─LocalVariableSyntax
//@[028:0032) |   | | └─IdentifierSyntax
//@[028:0032) |   | |   └─Token(Identifier) |name|
//@[032:0033) |   | ├─Token(Comma) |,|
//@[034:0035) |   | ├─LocalVariableSyntax
//@[034:0035) |   | | └─IdentifierSyntax
//@[034:0035) |   | |   └─Token(Identifier) |i|
//@[035:0036) |   | └─Token(RightParen) |)|
//@[037:0039) |   ├─Token(Identifier) |in|
//@[040:0049) |   ├─VariableAccessSyntax
//@[040:0049) |   | └─IdentifierSyntax
//@[040:0049) |   |   └─Token(Identifier) |loopInput|
//@[049:0050) |   ├─Token(Colon) |:|
//@[051:0061) |   ├─BinaryOperationSyntax
//@[051:0056) |   | ├─BinaryOperationSyntax
//@[051:0052) |   | | ├─VariableAccessSyntax
//@[051:0052) |   | | | └─IdentifierSyntax
//@[051:0052) |   | | |   └─Token(Identifier) |i|
//@[053:0054) |   | | ├─Token(Modulo) |%|
//@[055:0056) |   | | └─IntegerLiteralSyntax
//@[055:0056) |   | |   └─Token(Integer) |2|
//@[057:0059) |   | ├─Token(Equals) |==|
//@[060:0061) |   | └─IntegerLiteralSyntax
//@[060:0061) |   |   └─Token(Integer) |0|
//@[061:0062) |   └─Token(RightSquare) |]|
//@[062:0063) ├─Token(NewLine) |\n|
var arrayOfHardCodedNumbers = [for i in range(0,10): 3]
//@[000:0055) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0027) | ├─IdentifierSyntax
//@[004:0027) | | └─Token(Identifier) |arrayOfHardCodedNumbers|
//@[028:0029) | ├─Token(Assignment) |=|
//@[030:0055) | └─ForSyntax
//@[030:0031) |   ├─Token(LeftSquare) |[|
//@[031:0034) |   ├─Token(Identifier) |for|
//@[035:0036) |   ├─LocalVariableSyntax
//@[035:0036) |   | └─IdentifierSyntax
//@[035:0036) |   |   └─Token(Identifier) |i|
//@[037:0039) |   ├─Token(Identifier) |in|
//@[040:0051) |   ├─FunctionCallSyntax
//@[040:0045) |   | ├─IdentifierSyntax
//@[040:0045) |   | | └─Token(Identifier) |range|
//@[045:0046) |   | ├─Token(LeftParen) |(|
//@[046:0047) |   | ├─FunctionArgumentSyntax
//@[046:0047) |   | | └─IntegerLiteralSyntax
//@[046:0047) |   | |   └─Token(Integer) |0|
//@[047:0048) |   | ├─Token(Comma) |,|
//@[048:0050) |   | ├─FunctionArgumentSyntax
//@[048:0050) |   | | └─IntegerLiteralSyntax
//@[048:0050) |   | |   └─Token(Integer) |10|
//@[050:0051) |   | └─Token(RightParen) |)|
//@[051:0052) |   ├─Token(Colon) |:|
//@[053:0054) |   ├─IntegerLiteralSyntax
//@[053:0054) |   | └─Token(Integer) |3|
//@[054:0055) |   └─Token(RightSquare) |]|
//@[055:0056) ├─Token(NewLine) |\n|
var arrayOfHardCodedBools = [for i in range(0,10): false]
//@[000:0057) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0025) | ├─IdentifierSyntax
//@[004:0025) | | └─Token(Identifier) |arrayOfHardCodedBools|
//@[026:0027) | ├─Token(Assignment) |=|
//@[028:0057) | └─ForSyntax
//@[028:0029) |   ├─Token(LeftSquare) |[|
//@[029:0032) |   ├─Token(Identifier) |for|
//@[033:0034) |   ├─LocalVariableSyntax
//@[033:0034) |   | └─IdentifierSyntax
//@[033:0034) |   |   └─Token(Identifier) |i|
//@[035:0037) |   ├─Token(Identifier) |in|
//@[038:0049) |   ├─FunctionCallSyntax
//@[038:0043) |   | ├─IdentifierSyntax
//@[038:0043) |   | | └─Token(Identifier) |range|
//@[043:0044) |   | ├─Token(LeftParen) |(|
//@[044:0045) |   | ├─FunctionArgumentSyntax
//@[044:0045) |   | | └─IntegerLiteralSyntax
//@[044:0045) |   | |   └─Token(Integer) |0|
//@[045:0046) |   | ├─Token(Comma) |,|
//@[046:0048) |   | ├─FunctionArgumentSyntax
//@[046:0048) |   | | └─IntegerLiteralSyntax
//@[046:0048) |   | |   └─Token(Integer) |10|
//@[048:0049) |   | └─Token(RightParen) |)|
//@[049:0050) |   ├─Token(Colon) |:|
//@[051:0056) |   ├─BooleanLiteralSyntax
//@[051:0056) |   | └─Token(FalseKeyword) |false|
//@[056:0057) |   └─Token(RightSquare) |]|
//@[057:0058) ├─Token(NewLine) |\n|
var arrayOfHardCodedStrings = [for i in range(0,3): 'hi']
//@[000:0057) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0027) | ├─IdentifierSyntax
//@[004:0027) | | └─Token(Identifier) |arrayOfHardCodedStrings|
//@[028:0029) | ├─Token(Assignment) |=|
//@[030:0057) | └─ForSyntax
//@[030:0031) |   ├─Token(LeftSquare) |[|
//@[031:0034) |   ├─Token(Identifier) |for|
//@[035:0036) |   ├─LocalVariableSyntax
//@[035:0036) |   | └─IdentifierSyntax
//@[035:0036) |   |   └─Token(Identifier) |i|
//@[037:0039) |   ├─Token(Identifier) |in|
//@[040:0050) |   ├─FunctionCallSyntax
//@[040:0045) |   | ├─IdentifierSyntax
//@[040:0045) |   | | └─Token(Identifier) |range|
//@[045:0046) |   | ├─Token(LeftParen) |(|
//@[046:0047) |   | ├─FunctionArgumentSyntax
//@[046:0047) |   | | └─IntegerLiteralSyntax
//@[046:0047) |   | |   └─Token(Integer) |0|
//@[047:0048) |   | ├─Token(Comma) |,|
//@[048:0049) |   | ├─FunctionArgumentSyntax
//@[048:0049) |   | | └─IntegerLiteralSyntax
//@[048:0049) |   | |   └─Token(Integer) |3|
//@[049:0050) |   | └─Token(RightParen) |)|
//@[050:0051) |   ├─Token(Colon) |:|
//@[052:0056) |   ├─StringSyntax
//@[052:0056) |   | └─Token(StringComplete) |'hi'|
//@[056:0057) |   └─Token(RightSquare) |]|
//@[057:0058) ├─Token(NewLine) |\n|
var arrayOfNonRuntimeFunctionCalls = [for i in range(0,3): concat('hi', i)]
//@[000:0075) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0034) | ├─IdentifierSyntax
//@[004:0034) | | └─Token(Identifier) |arrayOfNonRuntimeFunctionCalls|
//@[035:0036) | ├─Token(Assignment) |=|
//@[037:0075) | └─ForSyntax
//@[037:0038) |   ├─Token(LeftSquare) |[|
//@[038:0041) |   ├─Token(Identifier) |for|
//@[042:0043) |   ├─LocalVariableSyntax
//@[042:0043) |   | └─IdentifierSyntax
//@[042:0043) |   |   └─Token(Identifier) |i|
//@[044:0046) |   ├─Token(Identifier) |in|
//@[047:0057) |   ├─FunctionCallSyntax
//@[047:0052) |   | ├─IdentifierSyntax
//@[047:0052) |   | | └─Token(Identifier) |range|
//@[052:0053) |   | ├─Token(LeftParen) |(|
//@[053:0054) |   | ├─FunctionArgumentSyntax
//@[053:0054) |   | | └─IntegerLiteralSyntax
//@[053:0054) |   | |   └─Token(Integer) |0|
//@[054:0055) |   | ├─Token(Comma) |,|
//@[055:0056) |   | ├─FunctionArgumentSyntax
//@[055:0056) |   | | └─IntegerLiteralSyntax
//@[055:0056) |   | |   └─Token(Integer) |3|
//@[056:0057) |   | └─Token(RightParen) |)|
//@[057:0058) |   ├─Token(Colon) |:|
//@[059:0074) |   ├─FunctionCallSyntax
//@[059:0065) |   | ├─IdentifierSyntax
//@[059:0065) |   | | └─Token(Identifier) |concat|
//@[065:0066) |   | ├─Token(LeftParen) |(|
//@[066:0070) |   | ├─FunctionArgumentSyntax
//@[066:0070) |   | | └─StringSyntax
//@[066:0070) |   | |   └─Token(StringComplete) |'hi'|
//@[070:0071) |   | ├─Token(Comma) |,|
//@[072:0073) |   | ├─FunctionArgumentSyntax
//@[072:0073) |   | | └─VariableAccessSyntax
//@[072:0073) |   | |   └─IdentifierSyntax
//@[072:0073) |   | |     └─Token(Identifier) |i|
//@[073:0074) |   | └─Token(RightParen) |)|
//@[074:0075) |   └─Token(RightSquare) |]|
//@[075:0077) ├─Token(NewLine) |\n\n|

var multilineString = '''
//@[000:0036) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0019) | ├─IdentifierSyntax
//@[004:0019) | | └─Token(Identifier) |multilineString|
//@[020:0021) | ├─Token(Assignment) |=|
//@[022:0036) | └─StringSyntax
//@[022:0036) |   └─Token(MultilineString) |'''\nHELLO!\n'''|
HELLO!
'''
//@[003:0005) ├─Token(NewLine) |\n\n|

var multilineEmpty = ''''''
//@[000:0027) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0018) | ├─IdentifierSyntax
//@[004:0018) | | └─Token(Identifier) |multilineEmpty|
//@[019:0020) | ├─Token(Assignment) |=|
//@[021:0027) | └─StringSyntax
//@[021:0027) |   └─Token(MultilineString) |''''''|
//@[027:0028) ├─Token(NewLine) |\n|
var multilineEmptyNewline = '''
//@[000:0035) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0025) | ├─IdentifierSyntax
//@[004:0025) | | └─Token(Identifier) |multilineEmptyNewline|
//@[026:0027) | ├─Token(Assignment) |=|
//@[028:0035) | └─StringSyntax
//@[028:0035) |   └─Token(MultilineString) |'''\n'''|
'''
//@[003:0005) ├─Token(NewLine) |\n\n|

// evaluates to '\'abc\''
//@[025:0026) ├─Token(NewLine) |\n|
var multilineExtraQuotes = ''''abc''''
//@[000:0038) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0024) | ├─IdentifierSyntax
//@[004:0024) | | └─Token(Identifier) |multilineExtraQuotes|
//@[025:0026) | ├─Token(Assignment) |=|
//@[027:0038) | └─StringSyntax
//@[027:0038) |   └─Token(MultilineString) |''''abc''''|
//@[038:0040) ├─Token(NewLine) |\n\n|

// evaluates to '\'\nabc\n\''
//@[029:0030) ├─Token(NewLine) |\n|
var multilineExtraQuotesNewlines = ''''
//@[000:0048) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0032) | ├─IdentifierSyntax
//@[004:0032) | | └─Token(Identifier) |multilineExtraQuotesNewlines|
//@[033:0034) | ├─Token(Assignment) |=|
//@[035:0048) | └─StringSyntax
//@[035:0048) |   └─Token(MultilineString) |''''\nabc\n''''|
abc
''''
//@[004:0006) ├─Token(NewLine) |\n\n|

var multilineSingleLine = '''hello!'''
//@[000:0038) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0023) | ├─IdentifierSyntax
//@[004:0023) | | └─Token(Identifier) |multilineSingleLine|
//@[024:0025) | ├─Token(Assignment) |=|
//@[026:0038) | └─StringSyntax
//@[026:0038) |   └─Token(MultilineString) |'''hello!'''|
//@[038:0040) ├─Token(NewLine) |\n\n|

var multilineFormatted = format('''
//@[000:0073) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0022) | ├─IdentifierSyntax
//@[004:0022) | | └─Token(Identifier) |multilineFormatted|
//@[023:0024) | ├─Token(Assignment) |=|
//@[025:0073) | └─FunctionCallSyntax
//@[025:0031) |   ├─IdentifierSyntax
//@[025:0031) |   | └─Token(Identifier) |format|
//@[031:0032) |   ├─Token(LeftParen) |(|
//@[032:0061) |   ├─FunctionArgumentSyntax
//@[032:0061) |   | └─StringSyntax
//@[032:0061) |   |   └─Token(MultilineString) |'''\nHello,\nmy\nname is\n{0}\n'''|
Hello,
my
name is
{0}
''', 'Anthony')
//@[003:0004) |   ├─Token(Comma) |,|
//@[005:0014) |   ├─FunctionArgumentSyntax
//@[005:0014) |   | └─StringSyntax
//@[005:0014) |   |   └─Token(StringComplete) |'Anthony'|
//@[014:0015) |   └─Token(RightParen) |)|
//@[015:0017) ├─Token(NewLine) |\n\n|

var multilineJavaScript = '''
//@[000:0586) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0023) | ├─IdentifierSyntax
//@[004:0023) | | └─Token(Identifier) |multilineJavaScript|
//@[024:0025) | ├─Token(Assignment) |=|
//@[026:0586) | └─StringSyntax
//@[026:0586) |   └─Token(MultilineString) |'''\n// NOT RECOMMENDED PATTERN\nconst fs = require('fs');\n\nmodule.exports = function (context) {\n    fs.readFile('./hello.txt', (err, data) => {\n        if (err) {\n            context.log.error('ERROR', err);\n            // BUG #1: This will result in an uncaught exception that crashes the entire process\n            throw err;\n        }\n        context.log(`Data from file: ${data}`);\n        // context.done() should be called here\n    });\n    // BUG #2: Data is not guaranteed to be read before the Azure Function's invocation ends\n    context.done();\n}\n'''|
// NOT RECOMMENDED PATTERN
const fs = require('fs');

module.exports = function (context) {
    fs.readFile('./hello.txt', (err, data) => {
        if (err) {
            context.log.error('ERROR', err);
            // BUG #1: This will result in an uncaught exception that crashes the entire process
            throw err;
        }
        context.log(`Data from file: ${data}`);
        // context.done() should be called here
    });
    // BUG #2: Data is not guaranteed to be read before the Azure Function's invocation ends
    context.done();
}
'''
//@[003:0005) ├─Token(NewLine) |\n\n|

var providersTest = providers('Microsoft.Resources').namespace
//@[000:0062) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0017) | ├─IdentifierSyntax
//@[004:0017) | | └─Token(Identifier) |providersTest|
//@[018:0019) | ├─Token(Assignment) |=|
//@[020:0062) | └─PropertyAccessSyntax
//@[020:0052) |   ├─FunctionCallSyntax
//@[020:0029) |   | ├─IdentifierSyntax
//@[020:0029) |   | | └─Token(Identifier) |providers|
//@[029:0030) |   | ├─Token(LeftParen) |(|
//@[030:0051) |   | ├─FunctionArgumentSyntax
//@[030:0051) |   | | └─StringSyntax
//@[030:0051) |   | |   └─Token(StringComplete) |'Microsoft.Resources'|
//@[051:0052) |   | └─Token(RightParen) |)|
//@[052:0053) |   ├─Token(Dot) |.|
//@[053:0062) |   └─IdentifierSyntax
//@[053:0062) |     └─Token(Identifier) |namespace|
//@[062:0063) ├─Token(NewLine) |\n|
var providersTest2 = providers('Microsoft.Resources', 'deployments').locations
//@[000:0078) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0018) | ├─IdentifierSyntax
//@[004:0018) | | └─Token(Identifier) |providersTest2|
//@[019:0020) | ├─Token(Assignment) |=|
//@[021:0078) | └─PropertyAccessSyntax
//@[021:0068) |   ├─FunctionCallSyntax
//@[021:0030) |   | ├─IdentifierSyntax
//@[021:0030) |   | | └─Token(Identifier) |providers|
//@[030:0031) |   | ├─Token(LeftParen) |(|
//@[031:0052) |   | ├─FunctionArgumentSyntax
//@[031:0052) |   | | └─StringSyntax
//@[031:0052) |   | |   └─Token(StringComplete) |'Microsoft.Resources'|
//@[052:0053) |   | ├─Token(Comma) |,|
//@[054:0067) |   | ├─FunctionArgumentSyntax
//@[054:0067) |   | | └─StringSyntax
//@[054:0067) |   | |   └─Token(StringComplete) |'deployments'|
//@[067:0068) |   | └─Token(RightParen) |)|
//@[068:0069) |   ├─Token(Dot) |.|
//@[069:0078) |   └─IdentifierSyntax
//@[069:0078) |     └─Token(Identifier) |locations|
//@[078:0080) ├─Token(NewLine) |\n\n|

var copyBlockInObject = {
//@[000:0120) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0021) | ├─IdentifierSyntax
//@[004:0021) | | └─Token(Identifier) |copyBlockInObject|
//@[022:0023) | ├─Token(Assignment) |=|
//@[024:0120) | └─ObjectSyntax
//@[024:0025) |   ├─Token(LeftBrace) |{|
//@[025:0026) |   ├─Token(NewLine) |\n|
  copy: [
//@[002:0092) |   ├─ObjectPropertySyntax
//@[002:0006) |   | ├─IdentifierSyntax
//@[002:0006) |   | | └─Token(Identifier) |copy|
//@[006:0007) |   | ├─Token(Colon) |:|
//@[008:0092) |   | └─ArraySyntax
//@[008:0009) |   |   ├─Token(LeftSquare) |[|
//@[009:0010) |   |   ├─Token(NewLine) |\n|
    {
//@[004:0078) |   |   ├─ArrayItemSyntax
//@[004:0078) |   |   | └─ObjectSyntax
//@[004:0005) |   |   |   ├─Token(LeftBrace) |{|
//@[005:0006) |   |   |   ├─Token(NewLine) |\n|
      name: 'blah'
//@[006:0018) |   |   |   ├─ObjectPropertySyntax
//@[006:0010) |   |   |   | ├─IdentifierSyntax
//@[006:0010) |   |   |   | | └─Token(Identifier) |name|
//@[010:0011) |   |   |   | ├─Token(Colon) |:|
//@[012:0018) |   |   |   | └─StringSyntax
//@[012:0018) |   |   |   |   └─Token(StringComplete) |'blah'|
//@[018:0019) |   |   |   ├─Token(NewLine) |\n|
      count: '[notAFunction()]'
//@[006:0031) |   |   |   ├─ObjectPropertySyntax
//@[006:0011) |   |   |   | ├─IdentifierSyntax
//@[006:0011) |   |   |   | | └─Token(Identifier) |count|
//@[011:0012) |   |   |   | ├─Token(Colon) |:|
//@[013:0031) |   |   |   | └─StringSyntax
//@[013:0031) |   |   |   |   └─Token(StringComplete) |'[notAFunction()]'|
//@[031:0032) |   |   |   ├─Token(NewLine) |\n|
      input: {}
//@[006:0015) |   |   |   ├─ObjectPropertySyntax
//@[006:0011) |   |   |   | ├─IdentifierSyntax
//@[006:0011) |   |   |   | | └─Token(Identifier) |input|
//@[011:0012) |   |   |   | ├─Token(Colon) |:|
//@[013:0015) |   |   |   | └─ObjectSyntax
//@[013:0014) |   |   |   |   ├─Token(LeftBrace) |{|
//@[014:0015) |   |   |   |   └─Token(RightBrace) |}|
//@[015:0016) |   |   |   ├─Token(NewLine) |\n|
    }
//@[004:0005) |   |   |   └─Token(RightBrace) |}|
//@[005:0006) |   |   ├─Token(NewLine) |\n|
  ]
//@[002:0003) |   |   └─Token(RightSquare) |]|
//@[003:0004) |   ├─Token(NewLine) |\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0003) ├─Token(NewLine) |\n\n|

var joinedString = join(['I', 'love', 'Bicep!'], ' ')
//@[000:0053) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0016) | ├─IdentifierSyntax
//@[004:0016) | | └─Token(Identifier) |joinedString|
//@[017:0018) | ├─Token(Assignment) |=|
//@[019:0053) | └─FunctionCallSyntax
//@[019:0023) |   ├─IdentifierSyntax
//@[019:0023) |   | └─Token(Identifier) |join|
//@[023:0024) |   ├─Token(LeftParen) |(|
//@[024:0047) |   ├─FunctionArgumentSyntax
//@[024:0047) |   | └─ArraySyntax
//@[024:0025) |   |   ├─Token(LeftSquare) |[|
//@[025:0028) |   |   ├─ArrayItemSyntax
//@[025:0028) |   |   | └─StringSyntax
//@[025:0028) |   |   |   └─Token(StringComplete) |'I'|
//@[028:0029) |   |   ├─Token(Comma) |,|
//@[030:0036) |   |   ├─ArrayItemSyntax
//@[030:0036) |   |   | └─StringSyntax
//@[030:0036) |   |   |   └─Token(StringComplete) |'love'|
//@[036:0037) |   |   ├─Token(Comma) |,|
//@[038:0046) |   |   ├─ArrayItemSyntax
//@[038:0046) |   |   | └─StringSyntax
//@[038:0046) |   |   |   └─Token(StringComplete) |'Bicep!'|
//@[046:0047) |   |   └─Token(RightSquare) |]|
//@[047:0048) |   ├─Token(Comma) |,|
//@[049:0052) |   ├─FunctionArgumentSyntax
//@[049:0052) |   | └─StringSyntax
//@[049:0052) |   |   └─Token(StringComplete) |' '|
//@[052:0053) |   └─Token(RightParen) |)|
//@[053:0054) ├─Token(NewLine) |\n|

//@[000:0000) └─Token(EndOfFile) ||
