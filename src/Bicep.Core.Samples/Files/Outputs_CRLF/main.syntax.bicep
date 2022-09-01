
//@[000:1589) ProgramSyntax
//@[000:0002) ├─Token(NewLine) |\r\n|
@sys.description('string output description')
//@[000:0076) ├─OutputDeclarationSyntax
//@[000:0045) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0045) | | └─InstanceFunctionCallSyntax
//@[001:0004) | |   ├─VariableAccessSyntax
//@[001:0004) | |   | └─IdentifierSyntax
//@[001:0004) | |   |   └─Token(Identifier) |sys|
//@[004:0005) | |   ├─Token(Dot) |.|
//@[005:0016) | |   ├─IdentifierSyntax
//@[005:0016) | |   | └─Token(Identifier) |description|
//@[016:0017) | |   ├─Token(LeftParen) |(|
//@[017:0044) | |   ├─FunctionArgumentSyntax
//@[017:0044) | |   | └─StringSyntax
//@[017:0044) | |   |   └─Token(StringComplete) |'string output description'|
//@[044:0045) | |   └─Token(RightParen) |)|
//@[045:0047) | ├─Token(NewLine) |\r\n|
output myStr string = 'hello'
//@[000:0006) | ├─Token(Identifier) |output|
//@[007:0012) | ├─IdentifierSyntax
//@[007:0012) | | └─Token(Identifier) |myStr|
//@[013:0019) | ├─SimpleTypeSyntax
//@[013:0019) | | └─Token(Identifier) |string|
//@[020:0021) | ├─Token(Assignment) |=|
//@[022:0029) | └─StringSyntax
//@[022:0029) |   └─Token(StringComplete) |'hello'|
//@[029:0033) ├─Token(NewLine) |\r\n\r\n|

@sys.description('int output description')
//@[000:0064) ├─OutputDeclarationSyntax
//@[000:0042) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0042) | | └─InstanceFunctionCallSyntax
//@[001:0004) | |   ├─VariableAccessSyntax
//@[001:0004) | |   | └─IdentifierSyntax
//@[001:0004) | |   |   └─Token(Identifier) |sys|
//@[004:0005) | |   ├─Token(Dot) |.|
//@[005:0016) | |   ├─IdentifierSyntax
//@[005:0016) | |   | └─Token(Identifier) |description|
//@[016:0017) | |   ├─Token(LeftParen) |(|
//@[017:0041) | |   ├─FunctionArgumentSyntax
//@[017:0041) | |   | └─StringSyntax
//@[017:0041) | |   |   └─Token(StringComplete) |'int output description'|
//@[041:0042) | |   └─Token(RightParen) |)|
//@[042:0044) | ├─Token(NewLine) |\r\n|
output myInt int = 7
//@[000:0006) | ├─Token(Identifier) |output|
//@[007:0012) | ├─IdentifierSyntax
//@[007:0012) | | └─Token(Identifier) |myInt|
//@[013:0016) | ├─SimpleTypeSyntax
//@[013:0016) | | └─Token(Identifier) |int|
//@[017:0018) | ├─Token(Assignment) |=|
//@[019:0020) | └─IntegerLiteralSyntax
//@[019:0020) |   └─Token(Integer) |7|
//@[020:0022) ├─Token(NewLine) |\r\n|
output myOtherInt int = 20 / 13 + 80 % -4
//@[000:0041) ├─OutputDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |output|
//@[007:0017) | ├─IdentifierSyntax
//@[007:0017) | | └─Token(Identifier) |myOtherInt|
//@[018:0021) | ├─SimpleTypeSyntax
//@[018:0021) | | └─Token(Identifier) |int|
//@[022:0023) | ├─Token(Assignment) |=|
//@[024:0041) | └─BinaryOperationSyntax
//@[024:0031) |   ├─BinaryOperationSyntax
//@[024:0026) |   | ├─IntegerLiteralSyntax
//@[024:0026) |   | | └─Token(Integer) |20|
//@[027:0028) |   | ├─Token(Slash) |/|
//@[029:0031) |   | └─IntegerLiteralSyntax
//@[029:0031) |   |   └─Token(Integer) |13|
//@[032:0033) |   ├─Token(Plus) |+|
//@[034:0041) |   └─BinaryOperationSyntax
//@[034:0036) |     ├─IntegerLiteralSyntax
//@[034:0036) |     | └─Token(Integer) |80|
//@[037:0038) |     ├─Token(Modulo) |%|
//@[039:0041) |     └─UnaryOperationSyntax
//@[039:0040) |       ├─Token(Minus) |-|
//@[040:0041) |       └─IntegerLiteralSyntax
//@[040:0041) |         └─Token(Integer) |4|
//@[041:0045) ├─Token(NewLine) |\r\n\r\n|

@sys.description('bool output description')
//@[000:0072) ├─OutputDeclarationSyntax
//@[000:0043) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0043) | | └─InstanceFunctionCallSyntax
//@[001:0004) | |   ├─VariableAccessSyntax
//@[001:0004) | |   | └─IdentifierSyntax
//@[001:0004) | |   |   └─Token(Identifier) |sys|
//@[004:0005) | |   ├─Token(Dot) |.|
//@[005:0016) | |   ├─IdentifierSyntax
//@[005:0016) | |   | └─Token(Identifier) |description|
//@[016:0017) | |   ├─Token(LeftParen) |(|
//@[017:0042) | |   ├─FunctionArgumentSyntax
//@[017:0042) | |   | └─StringSyntax
//@[017:0042) | |   |   └─Token(StringComplete) |'bool output description'|
//@[042:0043) | |   └─Token(RightParen) |)|
//@[043:0045) | ├─Token(NewLine) |\r\n|
output myBool bool = !false
//@[000:0006) | ├─Token(Identifier) |output|
//@[007:0013) | ├─IdentifierSyntax
//@[007:0013) | | └─Token(Identifier) |myBool|
//@[014:0018) | ├─SimpleTypeSyntax
//@[014:0018) | | └─Token(Identifier) |bool|
//@[019:0020) | ├─Token(Assignment) |=|
//@[021:0027) | └─UnaryOperationSyntax
//@[021:0022) |   ├─Token(Exclamation) |!|
//@[022:0027) |   └─BooleanLiteralSyntax
//@[022:0027) |     └─Token(FalseKeyword) |false|
//@[027:0029) ├─Token(NewLine) |\r\n|
output myOtherBool bool = true
//@[000:0030) ├─OutputDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |output|
//@[007:0018) | ├─IdentifierSyntax
//@[007:0018) | | └─Token(Identifier) |myOtherBool|
//@[019:0023) | ├─SimpleTypeSyntax
//@[019:0023) | | └─Token(Identifier) |bool|
//@[024:0025) | ├─Token(Assignment) |=|
//@[026:0030) | └─BooleanLiteralSyntax
//@[026:0030) |   └─Token(TrueKeyword) |true|
//@[030:0034) ├─Token(NewLine) |\r\n\r\n|

@sys.description('object array description')
//@[000:0075) ├─OutputDeclarationSyntax
//@[000:0044) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0044) | | └─InstanceFunctionCallSyntax
//@[001:0004) | |   ├─VariableAccessSyntax
//@[001:0004) | |   | └─IdentifierSyntax
//@[001:0004) | |   |   └─Token(Identifier) |sys|
//@[004:0005) | |   ├─Token(Dot) |.|
//@[005:0016) | |   ├─IdentifierSyntax
//@[005:0016) | |   | └─Token(Identifier) |description|
//@[016:0017) | |   ├─Token(LeftParen) |(|
//@[017:0043) | |   ├─FunctionArgumentSyntax
//@[017:0043) | |   | └─StringSyntax
//@[017:0043) | |   |   └─Token(StringComplete) |'object array description'|
//@[043:0044) | |   └─Token(RightParen) |)|
//@[044:0046) | ├─Token(NewLine) |\r\n|
output suchEmpty array = [
//@[000:0006) | ├─Token(Identifier) |output|
//@[007:0016) | ├─IdentifierSyntax
//@[007:0016) | | └─Token(Identifier) |suchEmpty|
//@[017:0022) | ├─SimpleTypeSyntax
//@[017:0022) | | └─Token(Identifier) |array|
//@[023:0024) | ├─Token(Assignment) |=|
//@[025:0029) | └─ArraySyntax
//@[025:0026) |   ├─Token(LeftSquare) |[|
//@[026:0028) |   ├─Token(NewLine) |\r\n|
]
//@[000:0001) |   └─Token(RightSquare) |]|
//@[001:0005) ├─Token(NewLine) |\r\n\r\n|

output suchEmpty2 object = {
//@[000:0031) ├─OutputDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |output|
//@[007:0017) | ├─IdentifierSyntax
//@[007:0017) | | └─Token(Identifier) |suchEmpty2|
//@[018:0024) | ├─SimpleTypeSyntax
//@[018:0024) | | └─Token(Identifier) |object|
//@[025:0026) | ├─Token(Assignment) |=|
//@[027:0031) | └─ObjectSyntax
//@[027:0028) |   ├─Token(LeftBrace) |{|
//@[028:0030) |   ├─Token(NewLine) |\r\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0005) ├─Token(NewLine) |\r\n\r\n|

@sys.description('object output description')
//@[000:0225) ├─OutputDeclarationSyntax
//@[000:0045) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0045) | | └─InstanceFunctionCallSyntax
//@[001:0004) | |   ├─VariableAccessSyntax
//@[001:0004) | |   | └─IdentifierSyntax
//@[001:0004) | |   |   └─Token(Identifier) |sys|
//@[004:0005) | |   ├─Token(Dot) |.|
//@[005:0016) | |   ├─IdentifierSyntax
//@[005:0016) | |   | └─Token(Identifier) |description|
//@[016:0017) | |   ├─Token(LeftParen) |(|
//@[017:0044) | |   ├─FunctionArgumentSyntax
//@[017:0044) | |   | └─StringSyntax
//@[017:0044) | |   |   └─Token(StringComplete) |'object output description'|
//@[044:0045) | |   └─Token(RightParen) |)|
//@[045:0047) | ├─Token(NewLine) |\r\n|
output obj object = {
//@[000:0006) | ├─Token(Identifier) |output|
//@[007:0010) | ├─IdentifierSyntax
//@[007:0010) | | └─Token(Identifier) |obj|
//@[011:0017) | ├─SimpleTypeSyntax
//@[011:0017) | | └─Token(Identifier) |object|
//@[018:0019) | ├─Token(Assignment) |=|
//@[020:0178) | └─ObjectSyntax
//@[020:0021) |   ├─Token(LeftBrace) |{|
//@[021:0023) |   ├─Token(NewLine) |\r\n|
  a: 'a'
//@[002:0008) |   ├─ObjectPropertySyntax
//@[002:0003) |   | ├─IdentifierSyntax
//@[002:0003) |   | | └─Token(Identifier) |a|
//@[003:0004) |   | ├─Token(Colon) |:|
//@[005:0008) |   | └─StringSyntax
//@[005:0008) |   |   └─Token(StringComplete) |'a'|
//@[008:0010) |   ├─Token(NewLine) |\r\n|
  b: 12
//@[002:0007) |   ├─ObjectPropertySyntax
//@[002:0003) |   | ├─IdentifierSyntax
//@[002:0003) |   | | └─Token(Identifier) |b|
//@[003:0004) |   | ├─Token(Colon) |:|
//@[005:0007) |   | └─IntegerLiteralSyntax
//@[005:0007) |   |   └─Token(Integer) |12|
//@[007:0009) |   ├─Token(NewLine) |\r\n|
  c: true
//@[002:0009) |   ├─ObjectPropertySyntax
//@[002:0003) |   | ├─IdentifierSyntax
//@[002:0003) |   | | └─Token(Identifier) |c|
//@[003:0004) |   | ├─Token(Colon) |:|
//@[005:0009) |   | └─BooleanLiteralSyntax
//@[005:0009) |   |   └─Token(TrueKeyword) |true|
//@[009:0011) |   ├─Token(NewLine) |\r\n|
  d: null
//@[002:0009) |   ├─ObjectPropertySyntax
//@[002:0003) |   | ├─IdentifierSyntax
//@[002:0003) |   | | └─Token(Identifier) |d|
//@[003:0004) |   | ├─Token(Colon) |:|
//@[005:0009) |   | └─NullLiteralSyntax
//@[005:0009) |   |   └─Token(NullKeyword) |null|
//@[009:0011) |   ├─Token(NewLine) |\r\n|
  list: [
//@[002:0059) |   ├─ObjectPropertySyntax
//@[002:0006) |   | ├─IdentifierSyntax
//@[002:0006) |   | | └─Token(Identifier) |list|
//@[006:0007) |   | ├─Token(Colon) |:|
//@[008:0059) |   | └─ArraySyntax
//@[008:0009) |   |   ├─Token(LeftSquare) |[|
//@[009:0011) |   |   ├─Token(NewLine) |\r\n|
    1
//@[004:0005) |   |   ├─ArrayItemSyntax
//@[004:0005) |   |   | └─IntegerLiteralSyntax
//@[004:0005) |   |   |   └─Token(Integer) |1|
//@[005:0007) |   |   ├─Token(NewLine) |\r\n|
    2
//@[004:0005) |   |   ├─ArrayItemSyntax
//@[004:0005) |   |   | └─IntegerLiteralSyntax
//@[004:0005) |   |   |   └─Token(Integer) |2|
//@[005:0007) |   |   ├─Token(NewLine) |\r\n|
    3
//@[004:0005) |   |   ├─ArrayItemSyntax
//@[004:0005) |   |   | └─IntegerLiteralSyntax
//@[004:0005) |   |   |   └─Token(Integer) |3|
//@[005:0007) |   |   ├─Token(NewLine) |\r\n|
    null
//@[004:0008) |   |   ├─ArrayItemSyntax
//@[004:0008) |   |   | └─NullLiteralSyntax
//@[004:0008) |   |   |   └─Token(NullKeyword) |null|
//@[008:0010) |   |   ├─Token(NewLine) |\r\n|
    {
//@[004:0012) |   |   ├─ArrayItemSyntax
//@[004:0012) |   |   | └─ObjectSyntax
//@[004:0005) |   |   |   ├─Token(LeftBrace) |{|
//@[005:0007) |   |   |   ├─Token(NewLine) |\r\n|
    }
//@[004:0005) |   |   |   └─Token(RightBrace) |}|
//@[005:0007) |   |   ├─Token(NewLine) |\r\n|
  ]
//@[002:0003) |   |   └─Token(RightSquare) |]|
//@[003:0005) |   ├─Token(NewLine) |\r\n|
  obj: {
//@[002:0050) |   ├─ObjectPropertySyntax
//@[002:0005) |   | ├─IdentifierSyntax
//@[002:0005) |   | | └─Token(Identifier) |obj|
//@[005:0006) |   | ├─Token(Colon) |:|
//@[007:0050) |   | └─ObjectSyntax
//@[007:0008) |   |   ├─Token(LeftBrace) |{|
//@[008:0010) |   |   ├─Token(NewLine) |\r\n|
    nested: [
//@[004:0035) |   |   ├─ObjectPropertySyntax
//@[004:0010) |   |   | ├─IdentifierSyntax
//@[004:0010) |   |   | | └─Token(Identifier) |nested|
//@[010:0011) |   |   | ├─Token(Colon) |:|
//@[012:0035) |   |   | └─ArraySyntax
//@[012:0013) |   |   |   ├─Token(LeftSquare) |[|
//@[013:0015) |   |   |   ├─Token(NewLine) |\r\n|
      'hello'
//@[006:0013) |   |   |   ├─ArrayItemSyntax
//@[006:0013) |   |   |   | └─StringSyntax
//@[006:0013) |   |   |   |   └─Token(StringComplete) |'hello'|
//@[013:0015) |   |   |   ├─Token(NewLine) |\r\n|
    ]
//@[004:0005) |   |   |   └─Token(RightSquare) |]|
//@[005:0007) |   |   ├─Token(NewLine) |\r\n|
  }
//@[002:0003) |   |   └─Token(RightBrace) |}|
//@[003:0005) |   ├─Token(NewLine) |\r\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0005) ├─Token(NewLine) |\r\n\r\n|

output myArr array = [
//@[000:0074) ├─OutputDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |output|
//@[007:0012) | ├─IdentifierSyntax
//@[007:0012) | | └─Token(Identifier) |myArr|
//@[013:0018) | ├─SimpleTypeSyntax
//@[013:0018) | | └─Token(Identifier) |array|
//@[019:0020) | ├─Token(Assignment) |=|
//@[021:0074) | └─ArraySyntax
//@[021:0022) |   ├─Token(LeftSquare) |[|
//@[022:0024) |   ├─Token(NewLine) |\r\n|
  'pirates'
//@[002:0011) |   ├─ArrayItemSyntax
//@[002:0011) |   | └─StringSyntax
//@[002:0011) |   |   └─Token(StringComplete) |'pirates'|
//@[011:0013) |   ├─Token(NewLine) |\r\n|
  'say'
//@[002:0007) |   ├─ArrayItemSyntax
//@[002:0007) |   | └─StringSyntax
//@[002:0007) |   |   └─Token(StringComplete) |'say'|
//@[007:0009) |   ├─Token(NewLine) |\r\n|
   false ? 'arr2' : 'arr'
//@[003:0025) |   ├─ArrayItemSyntax
//@[003:0025) |   | └─TernaryOperationSyntax
//@[003:0008) |   |   ├─BooleanLiteralSyntax
//@[003:0008) |   |   | └─Token(FalseKeyword) |false|
//@[009:0010) |   |   ├─Token(Question) |?|
//@[011:0017) |   |   ├─StringSyntax
//@[011:0017) |   |   | └─Token(StringComplete) |'arr2'|
//@[018:0019) |   |   ├─Token(Colon) |:|
//@[020:0025) |   |   └─StringSyntax
//@[020:0025) |   |     └─Token(StringComplete) |'arr'|
//@[025:0027) |   ├─Token(NewLine) |\r\n|
]
//@[000:0001) |   └─Token(RightSquare) |]|
//@[001:0005) ├─Token(NewLine) |\r\n\r\n|

output rgLocation string = resourceGroup().location
//@[000:0051) ├─OutputDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |output|
//@[007:0017) | ├─IdentifierSyntax
//@[007:0017) | | └─Token(Identifier) |rgLocation|
//@[018:0024) | ├─SimpleTypeSyntax
//@[018:0024) | | └─Token(Identifier) |string|
//@[025:0026) | ├─Token(Assignment) |=|
//@[027:0051) | └─PropertyAccessSyntax
//@[027:0042) |   ├─FunctionCallSyntax
//@[027:0040) |   | ├─IdentifierSyntax
//@[027:0040) |   | | └─Token(Identifier) |resourceGroup|
//@[040:0041) |   | ├─Token(LeftParen) |(|
//@[041:0042) |   | └─Token(RightParen) |)|
//@[042:0043) |   ├─Token(Dot) |.|
//@[043:0051) |   └─IdentifierSyntax
//@[043:0051) |     └─Token(Identifier) |location|
//@[051:0055) ├─Token(NewLine) |\r\n\r\n|

output isWestUs bool = resourceGroup().location != 'westus' ? false : true
//@[000:0074) ├─OutputDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |output|
//@[007:0015) | ├─IdentifierSyntax
//@[007:0015) | | └─Token(Identifier) |isWestUs|
//@[016:0020) | ├─SimpleTypeSyntax
//@[016:0020) | | └─Token(Identifier) |bool|
//@[021:0022) | ├─Token(Assignment) |=|
//@[023:0074) | └─TernaryOperationSyntax
//@[023:0059) |   ├─BinaryOperationSyntax
//@[023:0047) |   | ├─PropertyAccessSyntax
//@[023:0038) |   | | ├─FunctionCallSyntax
//@[023:0036) |   | | | ├─IdentifierSyntax
//@[023:0036) |   | | | | └─Token(Identifier) |resourceGroup|
//@[036:0037) |   | | | ├─Token(LeftParen) |(|
//@[037:0038) |   | | | └─Token(RightParen) |)|
//@[038:0039) |   | | ├─Token(Dot) |.|
//@[039:0047) |   | | └─IdentifierSyntax
//@[039:0047) |   | |   └─Token(Identifier) |location|
//@[048:0050) |   | ├─Token(NotEquals) |!=|
//@[051:0059) |   | └─StringSyntax
//@[051:0059) |   |   └─Token(StringComplete) |'westus'|
//@[060:0061) |   ├─Token(Question) |?|
//@[062:0067) |   ├─BooleanLiteralSyntax
//@[062:0067) |   | └─Token(FalseKeyword) |false|
//@[068:0069) |   ├─Token(Colon) |:|
//@[070:0074) |   └─BooleanLiteralSyntax
//@[070:0074) |     └─Token(TrueKeyword) |true|
//@[074:0078) ├─Token(NewLine) |\r\n\r\n|

output expressionBasedIndexer string = {
//@[000:0140) ├─OutputDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |output|
//@[007:0029) | ├─IdentifierSyntax
//@[007:0029) | | └─Token(Identifier) |expressionBasedIndexer|
//@[030:0036) | ├─SimpleTypeSyntax
//@[030:0036) | | └─Token(Identifier) |string|
//@[037:0038) | ├─Token(Assignment) |=|
//@[039:0140) | └─PropertyAccessSyntax
//@[039:0136) |   ├─ArrayAccessSyntax
//@[039:0110) |   | ├─ObjectSyntax
//@[039:0040) |   | | ├─Token(LeftBrace) |{|
//@[040:0042) |   | | ├─Token(NewLine) |\r\n|
  eastus: {
//@[002:0031) |   | | ├─ObjectPropertySyntax
//@[002:0008) |   | | | ├─IdentifierSyntax
//@[002:0008) |   | | | | └─Token(Identifier) |eastus|
//@[008:0009) |   | | | ├─Token(Colon) |:|
//@[010:0031) |   | | | └─ObjectSyntax
//@[010:0011) |   | | |   ├─Token(LeftBrace) |{|
//@[011:0013) |   | | |   ├─Token(NewLine) |\r\n|
    foo: true
//@[004:0013) |   | | |   ├─ObjectPropertySyntax
//@[004:0007) |   | | |   | ├─IdentifierSyntax
//@[004:0007) |   | | |   | | └─Token(Identifier) |foo|
//@[007:0008) |   | | |   | ├─Token(Colon) |:|
//@[009:0013) |   | | |   | └─BooleanLiteralSyntax
//@[009:0013) |   | | |   |   └─Token(TrueKeyword) |true|
//@[013:0015) |   | | |   ├─Token(NewLine) |\r\n|
  }
//@[002:0003) |   | | |   └─Token(RightBrace) |}|
//@[003:0005) |   | | ├─Token(NewLine) |\r\n|
  westus: {
//@[002:0032) |   | | ├─ObjectPropertySyntax
//@[002:0008) |   | | | ├─IdentifierSyntax
//@[002:0008) |   | | | | └─Token(Identifier) |westus|
//@[008:0009) |   | | | ├─Token(Colon) |:|
//@[010:0032) |   | | | └─ObjectSyntax
//@[010:0011) |   | | |   ├─Token(LeftBrace) |{|
//@[011:0013) |   | | |   ├─Token(NewLine) |\r\n|
    foo: false
//@[004:0014) |   | | |   ├─ObjectPropertySyntax
//@[004:0007) |   | | |   | ├─IdentifierSyntax
//@[004:0007) |   | | |   | | └─Token(Identifier) |foo|
//@[007:0008) |   | | |   | ├─Token(Colon) |:|
//@[009:0014) |   | | |   | └─BooleanLiteralSyntax
//@[009:0014) |   | | |   |   └─Token(FalseKeyword) |false|
//@[014:0016) |   | | |   ├─Token(NewLine) |\r\n|
  }
//@[002:0003) |   | | |   └─Token(RightBrace) |}|
//@[003:0005) |   | | ├─Token(NewLine) |\r\n|
}[resourceGroup().location].foo
//@[000:0001) |   | | └─Token(RightBrace) |}|
//@[001:0002) |   | ├─Token(LeftSquare) |[|
//@[002:0026) |   | ├─PropertyAccessSyntax
//@[002:0017) |   | | ├─FunctionCallSyntax
//@[002:0015) |   | | | ├─IdentifierSyntax
//@[002:0015) |   | | | | └─Token(Identifier) |resourceGroup|
//@[015:0016) |   | | | ├─Token(LeftParen) |(|
//@[016:0017) |   | | | └─Token(RightParen) |)|
//@[017:0018) |   | | ├─Token(Dot) |.|
//@[018:0026) |   | | └─IdentifierSyntax
//@[018:0026) |   | |   └─Token(Identifier) |location|
//@[026:0027) |   | └─Token(RightSquare) |]|
//@[027:0028) |   ├─Token(Dot) |.|
//@[028:0031) |   └─IdentifierSyntax
//@[028:0031) |     └─Token(Identifier) |foo|
//@[031:0035) ├─Token(NewLine) |\r\n\r\n|

var secondaryKeyIntermediateVar = listKeys(resourceId('Mock.RP/type', 'steve'), '2020-01-01').secondaryKey
//@[000:0106) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0031) | ├─IdentifierSyntax
//@[004:0031) | | └─Token(Identifier) |secondaryKeyIntermediateVar|
//@[032:0033) | ├─Token(Assignment) |=|
//@[034:0106) | └─PropertyAccessSyntax
//@[034:0093) |   ├─FunctionCallSyntax
//@[034:0042) |   | ├─IdentifierSyntax
//@[034:0042) |   | | └─Token(Identifier) |listKeys|
//@[042:0043) |   | ├─Token(LeftParen) |(|
//@[043:0078) |   | ├─FunctionArgumentSyntax
//@[043:0078) |   | | └─FunctionCallSyntax
//@[043:0053) |   | |   ├─IdentifierSyntax
//@[043:0053) |   | |   | └─Token(Identifier) |resourceId|
//@[053:0054) |   | |   ├─Token(LeftParen) |(|
//@[054:0068) |   | |   ├─FunctionArgumentSyntax
//@[054:0068) |   | |   | └─StringSyntax
//@[054:0068) |   | |   |   └─Token(StringComplete) |'Mock.RP/type'|
//@[068:0069) |   | |   ├─Token(Comma) |,|
//@[070:0077) |   | |   ├─FunctionArgumentSyntax
//@[070:0077) |   | |   | └─StringSyntax
//@[070:0077) |   | |   |   └─Token(StringComplete) |'steve'|
//@[077:0078) |   | |   └─Token(RightParen) |)|
//@[078:0079) |   | ├─Token(Comma) |,|
//@[080:0092) |   | ├─FunctionArgumentSyntax
//@[080:0092) |   | | └─StringSyntax
//@[080:0092) |   | |   └─Token(StringComplete) |'2020-01-01'|
//@[092:0093) |   | └─Token(RightParen) |)|
//@[093:0094) |   ├─Token(Dot) |.|
//@[094:0106) |   └─IdentifierSyntax
//@[094:0106) |     └─Token(Identifier) |secondaryKey|
//@[106:0110) ├─Token(NewLine) |\r\n\r\n|

output primaryKey string = listKeys(resourceId('Mock.RP/type', 'nigel'), '2020-01-01').primaryKey
//@[000:0097) ├─OutputDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |output|
//@[007:0017) | ├─IdentifierSyntax
//@[007:0017) | | └─Token(Identifier) |primaryKey|
//@[018:0024) | ├─SimpleTypeSyntax
//@[018:0024) | | └─Token(Identifier) |string|
//@[025:0026) | ├─Token(Assignment) |=|
//@[027:0097) | └─PropertyAccessSyntax
//@[027:0086) |   ├─FunctionCallSyntax
//@[027:0035) |   | ├─IdentifierSyntax
//@[027:0035) |   | | └─Token(Identifier) |listKeys|
//@[035:0036) |   | ├─Token(LeftParen) |(|
//@[036:0071) |   | ├─FunctionArgumentSyntax
//@[036:0071) |   | | └─FunctionCallSyntax
//@[036:0046) |   | |   ├─IdentifierSyntax
//@[036:0046) |   | |   | └─Token(Identifier) |resourceId|
//@[046:0047) |   | |   ├─Token(LeftParen) |(|
//@[047:0061) |   | |   ├─FunctionArgumentSyntax
//@[047:0061) |   | |   | └─StringSyntax
//@[047:0061) |   | |   |   └─Token(StringComplete) |'Mock.RP/type'|
//@[061:0062) |   | |   ├─Token(Comma) |,|
//@[063:0070) |   | |   ├─FunctionArgumentSyntax
//@[063:0070) |   | |   | └─StringSyntax
//@[063:0070) |   | |   |   └─Token(StringComplete) |'nigel'|
//@[070:0071) |   | |   └─Token(RightParen) |)|
//@[071:0072) |   | ├─Token(Comma) |,|
//@[073:0085) |   | ├─FunctionArgumentSyntax
//@[073:0085) |   | | └─StringSyntax
//@[073:0085) |   | |   └─Token(StringComplete) |'2020-01-01'|
//@[085:0086) |   | └─Token(RightParen) |)|
//@[086:0087) |   ├─Token(Dot) |.|
//@[087:0097) |   └─IdentifierSyntax
//@[087:0097) |     └─Token(Identifier) |primaryKey|
//@[097:0099) ├─Token(NewLine) |\r\n|
output secondaryKey string = secondaryKeyIntermediateVar
//@[000:0056) ├─OutputDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |output|
//@[007:0019) | ├─IdentifierSyntax
//@[007:0019) | | └─Token(Identifier) |secondaryKey|
//@[020:0026) | ├─SimpleTypeSyntax
//@[020:0026) | | └─Token(Identifier) |string|
//@[027:0028) | ├─Token(Assignment) |=|
//@[029:0056) | └─VariableAccessSyntax
//@[029:0056) |   └─IdentifierSyntax
//@[029:0056) |     └─Token(Identifier) |secondaryKeyIntermediateVar|
//@[056:0060) ├─Token(NewLine) |\r\n\r\n|

var varWithOverlappingOutput = 'hello'
//@[000:0038) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0028) | ├─IdentifierSyntax
//@[004:0028) | | └─Token(Identifier) |varWithOverlappingOutput|
//@[029:0030) | ├─Token(Assignment) |=|
//@[031:0038) | └─StringSyntax
//@[031:0038) |   └─Token(StringComplete) |'hello'|
//@[038:0040) ├─Token(NewLine) |\r\n|
param paramWithOverlappingOutput string
//@[000:0039) ├─ParameterDeclarationSyntax
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0032) | ├─IdentifierSyntax
//@[006:0032) | | └─Token(Identifier) |paramWithOverlappingOutput|
//@[033:0039) | └─SimpleTypeSyntax
//@[033:0039) |   └─Token(Identifier) |string|
//@[039:0043) ├─Token(NewLine) |\r\n\r\n|

output varWithOverlappingOutput string = varWithOverlappingOutput
//@[000:0065) ├─OutputDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |output|
//@[007:0031) | ├─IdentifierSyntax
//@[007:0031) | | └─Token(Identifier) |varWithOverlappingOutput|
//@[032:0038) | ├─SimpleTypeSyntax
//@[032:0038) | | └─Token(Identifier) |string|
//@[039:0040) | ├─Token(Assignment) |=|
//@[041:0065) | └─VariableAccessSyntax
//@[041:0065) |   └─IdentifierSyntax
//@[041:0065) |     └─Token(Identifier) |varWithOverlappingOutput|
//@[065:0067) ├─Token(NewLine) |\r\n|
output paramWithOverlappingOutput string = paramWithOverlappingOutput
//@[000:0069) ├─OutputDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |output|
//@[007:0033) | ├─IdentifierSyntax
//@[007:0033) | | └─Token(Identifier) |paramWithOverlappingOutput|
//@[034:0040) | ├─SimpleTypeSyntax
//@[034:0040) | | └─Token(Identifier) |string|
//@[041:0042) | ├─Token(Assignment) |=|
//@[043:0069) | └─VariableAccessSyntax
//@[043:0069) |   └─IdentifierSyntax
//@[043:0069) |     └─Token(Identifier) |paramWithOverlappingOutput|
//@[069:0073) ├─Token(NewLine) |\r\n\r\n|

// top-level output loops are supported
//@[039:0041) ├─Token(NewLine) |\r\n|
output generatedArray array = [for i in range(0,10): i]
//@[000:0055) ├─OutputDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |output|
//@[007:0021) | ├─IdentifierSyntax
//@[007:0021) | | └─Token(Identifier) |generatedArray|
//@[022:0027) | ├─SimpleTypeSyntax
//@[022:0027) | | └─Token(Identifier) |array|
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
//@[053:0054) |   ├─VariableAccessSyntax
//@[053:0054) |   | └─IdentifierSyntax
//@[053:0054) |   |   └─Token(Identifier) |i|
//@[054:0055) |   └─Token(RightSquare) |]|
//@[055:0057) ├─Token(NewLine) |\r\n|

//@[000:0000) └─Token(EndOfFile) ||
