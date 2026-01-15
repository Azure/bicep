/*
//@[000:3213) ProgramSyntax
  This is a block comment.
*/
//@[002:0004) ├─Token(NewLine) |\n\n|

// parameters without default value
//@[035:0036) ├─Token(NewLine) |\n|
@sys.description('''
//@[000:0097) ├─ParameterDeclarationSyntax
//@[000:0075) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0075) | | └─InstanceFunctionCallSyntax
//@[001:0004) | |   ├─VariableAccessSyntax
//@[001:0004) | |   | └─IdentifierSyntax
//@[001:0004) | |   |   └─Token(Identifier) |sys|
//@[004:0005) | |   ├─Token(Dot) |.|
//@[005:0016) | |   ├─IdentifierSyntax
//@[005:0016) | |   | └─Token(Identifier) |description|
//@[016:0017) | |   ├─Token(LeftParen) |(|
//@[017:0074) | |   ├─FunctionArgumentSyntax
//@[017:0074) | |   | └─StringSyntax
//@[017:0074) | |   |   └─Token(StringComplete) |'''\nthis is my multi line\ndescription for my myString\n'''|
this is my multi line
description for my myString
''')
//@[003:0004) | |   └─Token(RightParen) |)|
//@[004:0005) | ├─Token(NewLine) |\n|
param myString string
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0014) | ├─IdentifierSyntax
//@[006:0014) | | └─Token(Identifier) |myString|
//@[015:0021) | └─TypeVariableAccessSyntax
//@[015:0021) |   └─IdentifierSyntax
//@[015:0021) |     └─Token(Identifier) |string|
//@[021:0022) ├─Token(NewLine) |\n|
param myInt int
//@[000:0015) ├─ParameterDeclarationSyntax
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0011) | ├─IdentifierSyntax
//@[006:0011) | | └─Token(Identifier) |myInt|
//@[012:0015) | └─TypeVariableAccessSyntax
//@[012:0015) |   └─IdentifierSyntax
//@[012:0015) |     └─Token(Identifier) |int|
//@[015:0016) ├─Token(NewLine) |\n|
param myBool bool
//@[000:0017) ├─ParameterDeclarationSyntax
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0012) | ├─IdentifierSyntax
//@[006:0012) | | └─Token(Identifier) |myBool|
//@[013:0017) | └─TypeVariableAccessSyntax
//@[013:0017) |   └─IdentifierSyntax
//@[013:0017) |     └─Token(Identifier) |bool|
//@[017:0019) ├─Token(NewLine) |\n\n|

// parameters with default value
//@[032:0033) ├─Token(NewLine) |\n|
@sys.description('this is myString2')
//@[000:0135) ├─ParameterDeclarationSyntax
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
//@[017:0036) | |   |   └─Token(StringComplete) |'this is myString2'|
//@[036:0037) | |   └─Token(RightParen) |)|
//@[037:0038) | ├─Token(NewLine) |\n|
@metadata({
//@[000:0057) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0057) | | └─FunctionCallSyntax
//@[001:0009) | |   ├─IdentifierSyntax
//@[001:0009) | |   | └─Token(Identifier) |metadata|
//@[009:0010) | |   ├─Token(LeftParen) |(|
//@[010:0056) | |   ├─FunctionArgumentSyntax
//@[010:0056) | |   | └─ObjectSyntax
//@[010:0011) | |   |   ├─Token(LeftBrace) |{|
//@[011:0012) | |   |   ├─Token(NewLine) |\n|
  description: 'overwrite but still valid'
//@[002:0042) | |   |   ├─ObjectPropertySyntax
//@[002:0013) | |   |   | ├─IdentifierSyntax
//@[002:0013) | |   |   | | └─Token(Identifier) |description|
//@[013:0014) | |   |   | ├─Token(Colon) |:|
//@[015:0042) | |   |   | └─StringSyntax
//@[015:0042) | |   |   |   └─Token(StringComplete) |'overwrite but still valid'|
//@[042:0043) | |   |   ├─Token(NewLine) |\n|
})
//@[000:0001) | |   |   └─Token(RightBrace) |}|
//@[001:0002) | |   └─Token(RightParen) |)|
//@[002:0003) | ├─Token(NewLine) |\n|
param myString2 string = 'string value'
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0015) | ├─IdentifierSyntax
//@[006:0015) | | └─Token(Identifier) |myString2|
//@[016:0022) | ├─TypeVariableAccessSyntax
//@[016:0022) | | └─IdentifierSyntax
//@[016:0022) | |   └─Token(Identifier) |string|
//@[023:0039) | └─ParameterDefaultValueSyntax
//@[023:0024) |   ├─Token(Assignment) |=|
//@[025:0039) |   └─StringSyntax
//@[025:0039) |     └─Token(StringComplete) |'string value'|
//@[039:0040) ├─Token(NewLine) |\n|
param myInt2 int = 42
//@[000:0021) ├─ParameterDeclarationSyntax
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0012) | ├─IdentifierSyntax
//@[006:0012) | | └─Token(Identifier) |myInt2|
//@[013:0016) | ├─TypeVariableAccessSyntax
//@[013:0016) | | └─IdentifierSyntax
//@[013:0016) | |   └─Token(Identifier) |int|
//@[017:0021) | └─ParameterDefaultValueSyntax
//@[017:0018) |   ├─Token(Assignment) |=|
//@[019:0021) |   └─IntegerLiteralSyntax
//@[019:0021) |     └─Token(Integer) |42|
//@[021:0022) ├─Token(NewLine) |\n|
param myTruth bool = true
//@[000:0025) ├─ParameterDeclarationSyntax
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0013) | ├─IdentifierSyntax
//@[006:0013) | | └─Token(Identifier) |myTruth|
//@[014:0018) | ├─TypeVariableAccessSyntax
//@[014:0018) | | └─IdentifierSyntax
//@[014:0018) | |   └─Token(Identifier) |bool|
//@[019:0025) | └─ParameterDefaultValueSyntax
//@[019:0020) |   ├─Token(Assignment) |=|
//@[021:0025) |   └─BooleanLiteralSyntax
//@[021:0025) |     └─Token(TrueKeyword) |true|
//@[025:0026) ├─Token(NewLine) |\n|
param myFalsehood bool = false
//@[000:0030) ├─ParameterDeclarationSyntax
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0017) | ├─IdentifierSyntax
//@[006:0017) | | └─Token(Identifier) |myFalsehood|
//@[018:0022) | ├─TypeVariableAccessSyntax
//@[018:0022) | | └─IdentifierSyntax
//@[018:0022) | |   └─Token(Identifier) |bool|
//@[023:0030) | └─ParameterDefaultValueSyntax
//@[023:0024) |   ├─Token(Assignment) |=|
//@[025:0030) |   └─BooleanLiteralSyntax
//@[025:0030) |     └─Token(FalseKeyword) |false|
//@[030:0031) ├─Token(NewLine) |\n|
param myEscapedString string = 'First line\r\nSecond\ttabbed\tline'
//@[000:0067) ├─ParameterDeclarationSyntax
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0021) | ├─IdentifierSyntax
//@[006:0021) | | └─Token(Identifier) |myEscapedString|
//@[022:0028) | ├─TypeVariableAccessSyntax
//@[022:0028) | | └─IdentifierSyntax
//@[022:0028) | |   └─Token(Identifier) |string|
//@[029:0067) | └─ParameterDefaultValueSyntax
//@[029:0030) |   ├─Token(Assignment) |=|
//@[031:0067) |   └─StringSyntax
//@[031:0067) |     └─Token(StringComplete) |'First line\r\nSecond\ttabbed\tline'|
//@[067:0069) ├─Token(NewLine) |\n\n|

// object default value
//@[023:0024) ├─Token(NewLine) |\n|
@sys.description('this is foo')
//@[000:0348) ├─ParameterDeclarationSyntax
//@[000:0031) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0031) | | └─InstanceFunctionCallSyntax
//@[001:0004) | |   ├─VariableAccessSyntax
//@[001:0004) | |   | └─IdentifierSyntax
//@[001:0004) | |   |   └─Token(Identifier) |sys|
//@[004:0005) | |   ├─Token(Dot) |.|
//@[005:0016) | |   ├─IdentifierSyntax
//@[005:0016) | |   | └─Token(Identifier) |description|
//@[016:0017) | |   ├─Token(LeftParen) |(|
//@[017:0030) | |   ├─FunctionArgumentSyntax
//@[017:0030) | |   | └─StringSyntax
//@[017:0030) | |   |   └─Token(StringComplete) |'this is foo'|
//@[030:0031) | |   └─Token(RightParen) |)|
//@[031:0032) | ├─Token(NewLine) |\n|
@metadata({
//@[000:0083) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0083) | | └─FunctionCallSyntax
//@[001:0009) | |   ├─IdentifierSyntax
//@[001:0009) | |   | └─Token(Identifier) |metadata|
//@[009:0010) | |   ├─Token(LeftParen) |(|
//@[010:0082) | |   ├─FunctionArgumentSyntax
//@[010:0082) | |   | └─ObjectSyntax
//@[010:0011) | |   |   ├─Token(LeftBrace) |{|
//@[011:0012) | |   |   ├─Token(NewLine) |\n|
  description: 'overwrite but still valid'
//@[002:0042) | |   |   ├─ObjectPropertySyntax
//@[002:0013) | |   |   | ├─IdentifierSyntax
//@[002:0013) | |   |   | | └─Token(Identifier) |description|
//@[013:0014) | |   |   | ├─Token(Colon) |:|
//@[015:0042) | |   |   | └─StringSyntax
//@[015:0042) | |   |   |   └─Token(StringComplete) |'overwrite but still valid'|
//@[042:0043) | |   |   ├─Token(NewLine) |\n|
  another: 'just for fun'
//@[002:0025) | |   |   ├─ObjectPropertySyntax
//@[002:0009) | |   |   | ├─IdentifierSyntax
//@[002:0009) | |   |   | | └─Token(Identifier) |another|
//@[009:0010) | |   |   | ├─Token(Colon) |:|
//@[011:0025) | |   |   | └─StringSyntax
//@[011:0025) | |   |   |   └─Token(StringComplete) |'just for fun'|
//@[025:0026) | |   |   ├─Token(NewLine) |\n|
})
//@[000:0001) | |   |   └─Token(RightBrace) |}|
//@[001:0002) | |   └─Token(RightParen) |)|
//@[002:0003) | ├─Token(NewLine) |\n|
param foo object = {
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0009) | ├─IdentifierSyntax
//@[006:0009) | | └─Token(Identifier) |foo|
//@[010:0016) | ├─TypeVariableAccessSyntax
//@[010:0016) | | └─IdentifierSyntax
//@[010:0016) | |   └─Token(Identifier) |object|
//@[017:0232) | └─ParameterDefaultValueSyntax
//@[017:0018) |   ├─Token(Assignment) |=|
//@[019:0232) |   └─ObjectSyntax
//@[019:0020) |     ├─Token(LeftBrace) |{|
//@[020:0021) |     ├─Token(NewLine) |\n|
  enabled: true
//@[002:0015) |     ├─ObjectPropertySyntax
//@[002:0009) |     | ├─IdentifierSyntax
//@[002:0009) |     | | └─Token(Identifier) |enabled|
//@[009:0010) |     | ├─Token(Colon) |:|
//@[011:0015) |     | └─BooleanLiteralSyntax
//@[011:0015) |     |   └─Token(TrueKeyword) |true|
//@[015:0016) |     ├─Token(NewLine) |\n|
  name: 'this is my object'
//@[002:0027) |     ├─ObjectPropertySyntax
//@[002:0006) |     | ├─IdentifierSyntax
//@[002:0006) |     | | └─Token(Identifier) |name|
//@[006:0007) |     | ├─Token(Colon) |:|
//@[008:0027) |     | └─StringSyntax
//@[008:0027) |     |   └─Token(StringComplete) |'this is my object'|
//@[027:0028) |     ├─Token(NewLine) |\n|
  priority: 3
//@[002:0013) |     ├─ObjectPropertySyntax
//@[002:0010) |     | ├─IdentifierSyntax
//@[002:0010) |     | | └─Token(Identifier) |priority|
//@[010:0011) |     | ├─Token(Colon) |:|
//@[012:0013) |     | └─IntegerLiteralSyntax
//@[012:0013) |     |   └─Token(Integer) |3|
//@[013:0014) |     ├─Token(NewLine) |\n|
  info: {
//@[002:0024) |     ├─ObjectPropertySyntax
//@[002:0006) |     | ├─IdentifierSyntax
//@[002:0006) |     | | └─Token(Identifier) |info|
//@[006:0007) |     | ├─Token(Colon) |:|
//@[008:0024) |     | └─ObjectSyntax
//@[008:0009) |     |   ├─Token(LeftBrace) |{|
//@[009:0010) |     |   ├─Token(NewLine) |\n|
    a: 'b'
//@[004:0010) |     |   ├─ObjectPropertySyntax
//@[004:0005) |     |   | ├─IdentifierSyntax
//@[004:0005) |     |   | | └─Token(Identifier) |a|
//@[005:0006) |     |   | ├─Token(Colon) |:|
//@[007:0010) |     |   | └─StringSyntax
//@[007:0010) |     |   |   └─Token(StringComplete) |'b'|
//@[010:0011) |     |   ├─Token(NewLine) |\n|
  }
//@[002:0003) |     |   └─Token(RightBrace) |}|
//@[003:0004) |     ├─Token(NewLine) |\n|
  empty: {
//@[002:0014) |     ├─ObjectPropertySyntax
//@[002:0007) |     | ├─IdentifierSyntax
//@[002:0007) |     | | └─Token(Identifier) |empty|
//@[007:0008) |     | ├─Token(Colon) |:|
//@[009:0014) |     | └─ObjectSyntax
//@[009:0010) |     |   ├─Token(LeftBrace) |{|
//@[010:0011) |     |   ├─Token(NewLine) |\n|
  }
//@[002:0003) |     |   └─Token(RightBrace) |}|
//@[003:0004) |     ├─Token(NewLine) |\n|
  array: [
//@[002:0111) |     ├─ObjectPropertySyntax
//@[002:0007) |     | ├─IdentifierSyntax
//@[002:0007) |     | | └─Token(Identifier) |array|
//@[007:0008) |     | ├─Token(Colon) |:|
//@[009:0111) |     | └─ArraySyntax
//@[009:0010) |     |   ├─Token(LeftSquare) |[|
//@[010:0011) |     |   ├─Token(NewLine) |\n|
    'string item'
//@[004:0017) |     |   ├─ArrayItemSyntax
//@[004:0017) |     |   | └─StringSyntax
//@[004:0017) |     |   |   └─Token(StringComplete) |'string item'|
//@[017:0018) |     |   ├─Token(NewLine) |\n|
    12
//@[004:0006) |     |   ├─ArrayItemSyntax
//@[004:0006) |     |   | └─IntegerLiteralSyntax
//@[004:0006) |     |   |   └─Token(Integer) |12|
//@[006:0007) |     |   ├─Token(NewLine) |\n|
    true
//@[004:0008) |     |   ├─ArrayItemSyntax
//@[004:0008) |     |   | └─BooleanLiteralSyntax
//@[004:0008) |     |   |   └─Token(TrueKeyword) |true|
//@[008:0009) |     |   ├─Token(NewLine) |\n|
    [
//@[004:0037) |     |   ├─ArrayItemSyntax
//@[004:0037) |     |   | └─ArraySyntax
//@[004:0005) |     |   |   ├─Token(LeftSquare) |[|
//@[005:0006) |     |   |   ├─Token(NewLine) |\n|
      'inner'
//@[006:0013) |     |   |   ├─ArrayItemSyntax
//@[006:0013) |     |   |   | └─StringSyntax
//@[006:0013) |     |   |   |   └─Token(StringComplete) |'inner'|
//@[013:0014) |     |   |   ├─Token(NewLine) |\n|
      false
//@[006:0011) |     |   |   ├─ArrayItemSyntax
//@[006:0011) |     |   |   | └─BooleanLiteralSyntax
//@[006:0011) |     |   |   |   └─Token(FalseKeyword) |false|
//@[011:0012) |     |   |   ├─Token(NewLine) |\n|
    ]
//@[004:0005) |     |   |   └─Token(RightSquare) |]|
//@[005:0006) |     |   ├─Token(NewLine) |\n|
    {
//@[004:0024) |     |   ├─ArrayItemSyntax
//@[004:0024) |     |   | └─ObjectSyntax
//@[004:0005) |     |   |   ├─Token(LeftBrace) |{|
//@[005:0006) |     |   |   ├─Token(NewLine) |\n|
      a: 'b'
//@[006:0012) |     |   |   ├─ObjectPropertySyntax
//@[006:0007) |     |   |   | ├─IdentifierSyntax
//@[006:0007) |     |   |   | | └─Token(Identifier) |a|
//@[007:0008) |     |   |   | ├─Token(Colon) |:|
//@[009:0012) |     |   |   | └─StringSyntax
//@[009:0012) |     |   |   |   └─Token(StringComplete) |'b'|
//@[012:0013) |     |   |   ├─Token(NewLine) |\n|
    }
//@[004:0005) |     |   |   └─Token(RightBrace) |}|
//@[005:0006) |     |   ├─Token(NewLine) |\n|
  ]
//@[002:0003) |     |   └─Token(RightSquare) |]|
//@[003:0004) |     ├─Token(NewLine) |\n|
}
//@[000:0001) |     └─Token(RightBrace) |}|
//@[001:0003) ├─Token(NewLine) |\n\n|

// array default value
//@[022:0023) ├─Token(NewLine) |\n|
param myArrayParam array = [
//@[000:0048) ├─ParameterDeclarationSyntax
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0018) | ├─IdentifierSyntax
//@[006:0018) | | └─Token(Identifier) |myArrayParam|
//@[019:0024) | ├─TypeVariableAccessSyntax
//@[019:0024) | | └─IdentifierSyntax
//@[019:0024) | |   └─Token(Identifier) |array|
//@[025:0048) | └─ParameterDefaultValueSyntax
//@[025:0026) |   ├─Token(Assignment) |=|
//@[027:0048) |   └─ArraySyntax
//@[027:0028) |     ├─Token(LeftSquare) |[|
//@[028:0029) |     ├─Token(NewLine) |\n|
  'a'
//@[002:0005) |     ├─ArrayItemSyntax
//@[002:0005) |     | └─StringSyntax
//@[002:0005) |     |   └─Token(StringComplete) |'a'|
//@[005:0006) |     ├─Token(NewLine) |\n|
  'b'
//@[002:0005) |     ├─ArrayItemSyntax
//@[002:0005) |     | └─StringSyntax
//@[002:0005) |     |   └─Token(StringComplete) |'b'|
//@[005:0006) |     ├─Token(NewLine) |\n|
  'c'
//@[002:0005) |     ├─ArrayItemSyntax
//@[002:0005) |     | └─StringSyntax
//@[002:0005) |     |   └─Token(StringComplete) |'c'|
//@[005:0006) |     ├─Token(NewLine) |\n|
]
//@[000:0001) |     └─Token(RightSquare) |]|
//@[001:0003) ├─Token(NewLine) |\n\n|

// secure string
//@[016:0017) ├─Token(NewLine) |\n|
@secure()
//@[000:0031) ├─ParameterDeclarationSyntax
//@[000:0009) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0009) | | └─FunctionCallSyntax
//@[001:0007) | |   ├─IdentifierSyntax
//@[001:0007) | |   | └─Token(Identifier) |secure|
//@[007:0008) | |   ├─Token(LeftParen) |(|
//@[008:0009) | |   └─Token(RightParen) |)|
//@[009:0010) | ├─Token(NewLine) |\n|
param password string
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0014) | ├─IdentifierSyntax
//@[006:0014) | | └─Token(Identifier) |password|
//@[015:0021) | └─TypeVariableAccessSyntax
//@[015:0021) |   └─IdentifierSyntax
//@[015:0021) |     └─Token(Identifier) |string|
//@[021:0023) ├─Token(NewLine) |\n\n|

// secure object
//@[016:0017) ├─Token(NewLine) |\n|
@secure()
//@[000:0035) ├─ParameterDeclarationSyntax
//@[000:0009) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0009) | | └─FunctionCallSyntax
//@[001:0007) | |   ├─IdentifierSyntax
//@[001:0007) | |   | └─Token(Identifier) |secure|
//@[007:0008) | |   ├─Token(LeftParen) |(|
//@[008:0009) | |   └─Token(RightParen) |)|
//@[009:0010) | ├─Token(NewLine) |\n|
param secretObject object
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0018) | ├─IdentifierSyntax
//@[006:0018) | | └─Token(Identifier) |secretObject|
//@[019:0025) | └─TypeVariableAccessSyntax
//@[019:0025) |   └─IdentifierSyntax
//@[019:0025) |     └─Token(Identifier) |object|
//@[025:0027) ├─Token(NewLine) |\n\n|

// enum parameter
//@[017:0018) ├─Token(NewLine) |\n|
@allowed([
//@[000:0071) ├─ParameterDeclarationSyntax
//@[000:0047) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0047) | | └─FunctionCallSyntax
//@[001:0008) | |   ├─IdentifierSyntax
//@[001:0008) | |   | └─Token(Identifier) |allowed|
//@[008:0009) | |   ├─Token(LeftParen) |(|
//@[009:0046) | |   ├─FunctionArgumentSyntax
//@[009:0046) | |   | └─ArraySyntax
//@[009:0010) | |   |   ├─Token(LeftSquare) |[|
//@[010:0011) | |   |   ├─Token(NewLine) |\n|
  'Standard_LRS'
//@[002:0016) | |   |   ├─ArrayItemSyntax
//@[002:0016) | |   |   | └─StringSyntax
//@[002:0016) | |   |   |   └─Token(StringComplete) |'Standard_LRS'|
//@[016:0017) | |   |   ├─Token(NewLine) |\n|
  'Standard_GRS'
//@[002:0016) | |   |   ├─ArrayItemSyntax
//@[002:0016) | |   |   | └─StringSyntax
//@[002:0016) | |   |   |   └─Token(StringComplete) |'Standard_GRS'|
//@[016:0017) | |   |   ├─Token(NewLine) |\n|
])
//@[000:0001) | |   |   └─Token(RightSquare) |]|
//@[001:0002) | |   └─Token(RightParen) |)|
//@[002:0003) | ├─Token(NewLine) |\n|
param storageSku string
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0016) | ├─IdentifierSyntax
//@[006:0016) | | └─Token(Identifier) |storageSku|
//@[017:0023) | └─TypeVariableAccessSyntax
//@[017:0023) |   └─IdentifierSyntax
//@[017:0023) |     └─Token(Identifier) |string|
//@[023:0025) ├─Token(NewLine) |\n\n|

@allowed([
//@[000:0043) ├─ParameterDeclarationSyntax
//@[000:0025) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0025) | | └─FunctionCallSyntax
//@[001:0008) | |   ├─IdentifierSyntax
//@[001:0008) | |   | └─Token(Identifier) |allowed|
//@[008:0009) | |   ├─Token(LeftParen) |(|
//@[009:0024) | |   ├─FunctionArgumentSyntax
//@[009:0024) | |   | └─ArraySyntax
//@[009:0010) | |   |   ├─Token(LeftSquare) |[|
//@[010:0011) | |   |   ├─Token(NewLine) |\n|
  1
//@[002:0003) | |   |   ├─ArrayItemSyntax
//@[002:0003) | |   |   | └─IntegerLiteralSyntax
//@[002:0003) | |   |   |   └─Token(Integer) |1|
//@[003:0004) | |   |   ├─Token(NewLine) |\n|
  2
//@[002:0003) | |   |   ├─ArrayItemSyntax
//@[002:0003) | |   |   | └─IntegerLiteralSyntax
//@[002:0003) | |   |   |   └─Token(Integer) |2|
//@[003:0004) | |   |   ├─Token(NewLine) |\n|
  3
//@[002:0003) | |   |   ├─ArrayItemSyntax
//@[002:0003) | |   |   | └─IntegerLiteralSyntax
//@[002:0003) | |   |   |   └─Token(Integer) |3|
//@[003:0004) | |   |   ├─Token(NewLine) |\n|
])
//@[000:0001) | |   |   └─Token(RightSquare) |]|
//@[001:0002) | |   └─Token(RightParen) |)|
//@[002:0003) | ├─Token(NewLine) |\n|
param intEnum int
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0013) | ├─IdentifierSyntax
//@[006:0013) | | └─Token(Identifier) |intEnum|
//@[014:0017) | └─TypeVariableAccessSyntax
//@[014:0017) |   └─IdentifierSyntax
//@[014:0017) |     └─Token(Identifier) |int|
//@[017:0019) ├─Token(NewLine) |\n\n|

// length constraint on a string
//@[032:0033) ├─Token(NewLine) |\n|
@minLength(3)
//@[000:0053) ├─ParameterDeclarationSyntax
//@[000:0013) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0013) | | └─FunctionCallSyntax
//@[001:0010) | |   ├─IdentifierSyntax
//@[001:0010) | |   | └─Token(Identifier) |minLength|
//@[010:0011) | |   ├─Token(LeftParen) |(|
//@[011:0012) | |   ├─FunctionArgumentSyntax
//@[011:0012) | |   | └─IntegerLiteralSyntax
//@[011:0012) | |   |   └─Token(Integer) |3|
//@[012:0013) | |   └─Token(RightParen) |)|
//@[013:0014) | ├─Token(NewLine) |\n|
@maxLength(24)
//@[000:0014) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0014) | | └─FunctionCallSyntax
//@[001:0010) | |   ├─IdentifierSyntax
//@[001:0010) | |   | └─Token(Identifier) |maxLength|
//@[010:0011) | |   ├─Token(LeftParen) |(|
//@[011:0013) | |   ├─FunctionArgumentSyntax
//@[011:0013) | |   | └─IntegerLiteralSyntax
//@[011:0013) | |   |   └─Token(Integer) |24|
//@[013:0014) | |   └─Token(RightParen) |)|
//@[014:0015) | ├─Token(NewLine) |\n|
param storageName string
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0017) | ├─IdentifierSyntax
//@[006:0017) | | └─Token(Identifier) |storageName|
//@[018:0024) | └─TypeVariableAccessSyntax
//@[018:0024) |   └─IdentifierSyntax
//@[018:0024) |     └─Token(Identifier) |string|
//@[024:0026) ├─Token(NewLine) |\n\n|

// length constraint on an array
//@[032:0033) ├─Token(NewLine) |\n|
@minLength(3)
//@[000:0050) ├─ParameterDeclarationSyntax
//@[000:0013) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0013) | | └─FunctionCallSyntax
//@[001:0010) | |   ├─IdentifierSyntax
//@[001:0010) | |   | └─Token(Identifier) |minLength|
//@[010:0011) | |   ├─Token(LeftParen) |(|
//@[011:0012) | |   ├─FunctionArgumentSyntax
//@[011:0012) | |   | └─IntegerLiteralSyntax
//@[011:0012) | |   |   └─Token(Integer) |3|
//@[012:0013) | |   └─Token(RightParen) |)|
//@[013:0014) | ├─Token(NewLine) |\n|
@maxLength(24)
//@[000:0014) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0014) | | └─FunctionCallSyntax
//@[001:0010) | |   ├─IdentifierSyntax
//@[001:0010) | |   | └─Token(Identifier) |maxLength|
//@[010:0011) | |   ├─Token(LeftParen) |(|
//@[011:0013) | |   ├─FunctionArgumentSyntax
//@[011:0013) | |   | └─IntegerLiteralSyntax
//@[011:0013) | |   |   └─Token(Integer) |24|
//@[013:0014) | |   └─Token(RightParen) |)|
//@[014:0015) | ├─Token(NewLine) |\n|
param someArray array
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0015) | ├─IdentifierSyntax
//@[006:0015) | | └─Token(Identifier) |someArray|
//@[016:0021) | └─TypeVariableAccessSyntax
//@[016:0021) |   └─IdentifierSyntax
//@[016:0021) |     └─Token(Identifier) |array|
//@[021:0023) ├─Token(NewLine) |\n\n|

// empty metadata
//@[017:0018) ├─Token(NewLine) |\n|
@metadata({})
//@[000:0040) ├─ParameterDeclarationSyntax
//@[000:0013) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0013) | | └─FunctionCallSyntax
//@[001:0009) | |   ├─IdentifierSyntax
//@[001:0009) | |   | └─Token(Identifier) |metadata|
//@[009:0010) | |   ├─Token(LeftParen) |(|
//@[010:0012) | |   ├─FunctionArgumentSyntax
//@[010:0012) | |   | └─ObjectSyntax
//@[010:0011) | |   |   ├─Token(LeftBrace) |{|
//@[011:0012) | |   |   └─Token(RightBrace) |}|
//@[012:0013) | |   └─Token(RightParen) |)|
//@[013:0014) | ├─Token(NewLine) |\n|
param emptyMetadata string
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0019) | ├─IdentifierSyntax
//@[006:0019) | | └─Token(Identifier) |emptyMetadata|
//@[020:0026) | └─TypeVariableAccessSyntax
//@[020:0026) |   └─IdentifierSyntax
//@[020:0026) |     └─Token(Identifier) |string|
//@[026:0028) ├─Token(NewLine) |\n\n|

// description
//@[014:0015) ├─Token(NewLine) |\n|
@metadata({
//@[000:0071) ├─ParameterDeclarationSyntax
//@[000:0046) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0046) | | └─FunctionCallSyntax
//@[001:0009) | |   ├─IdentifierSyntax
//@[001:0009) | |   | └─Token(Identifier) |metadata|
//@[009:0010) | |   ├─Token(LeftParen) |(|
//@[010:0045) | |   ├─FunctionArgumentSyntax
//@[010:0045) | |   | └─ObjectSyntax
//@[010:0011) | |   |   ├─Token(LeftBrace) |{|
//@[011:0012) | |   |   ├─Token(NewLine) |\n|
  description: 'my description'
//@[002:0031) | |   |   ├─ObjectPropertySyntax
//@[002:0013) | |   |   | ├─IdentifierSyntax
//@[002:0013) | |   |   | | └─Token(Identifier) |description|
//@[013:0014) | |   |   | ├─Token(Colon) |:|
//@[015:0031) | |   |   | └─StringSyntax
//@[015:0031) | |   |   |   └─Token(StringComplete) |'my description'|
//@[031:0032) | |   |   ├─Token(NewLine) |\n|
})
//@[000:0001) | |   |   └─Token(RightBrace) |}|
//@[001:0002) | |   └─Token(RightParen) |)|
//@[002:0003) | ├─Token(NewLine) |\n|
param description string
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0017) | ├─IdentifierSyntax
//@[006:0017) | | └─Token(Identifier) |description|
//@[018:0024) | └─TypeVariableAccessSyntax
//@[018:0024) |   └─IdentifierSyntax
//@[018:0024) |     └─Token(Identifier) |string|
//@[024:0026) ├─Token(NewLine) |\n\n|

@sys.description('my description')
//@[000:0060) ├─ParameterDeclarationSyntax
//@[000:0034) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0034) | | └─InstanceFunctionCallSyntax
//@[001:0004) | |   ├─VariableAccessSyntax
//@[001:0004) | |   | └─IdentifierSyntax
//@[001:0004) | |   |   └─Token(Identifier) |sys|
//@[004:0005) | |   ├─Token(Dot) |.|
//@[005:0016) | |   ├─IdentifierSyntax
//@[005:0016) | |   | └─Token(Identifier) |description|
//@[016:0017) | |   ├─Token(LeftParen) |(|
//@[017:0033) | |   ├─FunctionArgumentSyntax
//@[017:0033) | |   | └─StringSyntax
//@[017:0033) | |   |   └─Token(StringComplete) |'my description'|
//@[033:0034) | |   └─Token(RightParen) |)|
//@[034:0035) | ├─Token(NewLine) |\n|
param description2 string
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0018) | ├─IdentifierSyntax
//@[006:0018) | | └─Token(Identifier) |description2|
//@[019:0025) | └─TypeVariableAccessSyntax
//@[019:0025) |   └─IdentifierSyntax
//@[019:0025) |     └─Token(Identifier) |string|
//@[025:0027) ├─Token(NewLine) |\n\n|

// random extra metadata
//@[024:0025) ├─Token(NewLine) |\n|
@metadata({
//@[000:0133) ├─ParameterDeclarationSyntax
//@[000:0101) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0101) | | └─FunctionCallSyntax
//@[001:0009) | |   ├─IdentifierSyntax
//@[001:0009) | |   | └─Token(Identifier) |metadata|
//@[009:0010) | |   ├─Token(LeftParen) |(|
//@[010:0100) | |   ├─FunctionArgumentSyntax
//@[010:0100) | |   | └─ObjectSyntax
//@[010:0011) | |   |   ├─Token(LeftBrace) |{|
//@[011:0012) | |   |   ├─Token(NewLine) |\n|
  description: 'my description'
//@[002:0031) | |   |   ├─ObjectPropertySyntax
//@[002:0013) | |   |   | ├─IdentifierSyntax
//@[002:0013) | |   |   | | └─Token(Identifier) |description|
//@[013:0014) | |   |   | ├─Token(Colon) |:|
//@[015:0031) | |   |   | └─StringSyntax
//@[015:0031) | |   |   |   └─Token(StringComplete) |'my description'|
//@[031:0032) | |   |   ├─Token(NewLine) |\n|
  a: 1
//@[002:0006) | |   |   ├─ObjectPropertySyntax
//@[002:0003) | |   |   | ├─IdentifierSyntax
//@[002:0003) | |   |   | | └─Token(Identifier) |a|
//@[003:0004) | |   |   | ├─Token(Colon) |:|
//@[005:0006) | |   |   | └─IntegerLiteralSyntax
//@[005:0006) | |   |   |   └─Token(Integer) |1|
//@[006:0007) | |   |   ├─Token(NewLine) |\n|
  b: true
//@[002:0009) | |   |   ├─ObjectPropertySyntax
//@[002:0003) | |   |   | ├─IdentifierSyntax
//@[002:0003) | |   |   | | └─Token(Identifier) |b|
//@[003:0004) | |   |   | ├─Token(Colon) |:|
//@[005:0009) | |   |   | └─BooleanLiteralSyntax
//@[005:0009) | |   |   |   └─Token(TrueKeyword) |true|
//@[009:0010) | |   |   ├─Token(NewLine) |\n|
  c: [
//@[002:0010) | |   |   ├─ObjectPropertySyntax
//@[002:0003) | |   |   | ├─IdentifierSyntax
//@[002:0003) | |   |   | | └─Token(Identifier) |c|
//@[003:0004) | |   |   | ├─Token(Colon) |:|
//@[005:0010) | |   |   | └─ArraySyntax
//@[005:0006) | |   |   |   ├─Token(LeftSquare) |[|
//@[006:0007) | |   |   |   ├─Token(NewLine) |\n|
  ]
//@[002:0003) | |   |   |   └─Token(RightSquare) |]|
//@[003:0004) | |   |   ├─Token(NewLine) |\n|
  d: {
//@[002:0026) | |   |   ├─ObjectPropertySyntax
//@[002:0003) | |   |   | ├─IdentifierSyntax
//@[002:0003) | |   |   | | └─Token(Identifier) |d|
//@[003:0004) | |   |   | ├─Token(Colon) |:|
//@[005:0026) | |   |   | └─ObjectSyntax
//@[005:0006) | |   |   |   ├─Token(LeftBrace) |{|
//@[006:0007) | |   |   |   ├─Token(NewLine) |\n|
    test: 'abc'
//@[004:0015) | |   |   |   ├─ObjectPropertySyntax
//@[004:0008) | |   |   |   | ├─IdentifierSyntax
//@[004:0008) | |   |   |   | | └─Token(Identifier) |test|
//@[008:0009) | |   |   |   | ├─Token(Colon) |:|
//@[010:0015) | |   |   |   | └─StringSyntax
//@[010:0015) | |   |   |   |   └─Token(StringComplete) |'abc'|
//@[015:0016) | |   |   |   ├─Token(NewLine) |\n|
  }
//@[002:0003) | |   |   |   └─Token(RightBrace) |}|
//@[003:0004) | |   |   ├─Token(NewLine) |\n|
})
//@[000:0001) | |   |   └─Token(RightBrace) |}|
//@[001:0002) | |   └─Token(RightParen) |)|
//@[002:0003) | ├─Token(NewLine) |\n|
param additionalMetadata string
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0024) | ├─IdentifierSyntax
//@[006:0024) | | └─Token(Identifier) |additionalMetadata|
//@[025:0031) | └─TypeVariableAccessSyntax
//@[025:0031) |   └─IdentifierSyntax
//@[025:0031) |     └─Token(Identifier) |string|
//@[031:0033) ├─Token(NewLine) |\n\n|

// all modifiers together
//@[025:0026) ├─Token(NewLine) |\n|
@secure()
//@[000:0165) ├─ParameterDeclarationSyntax
//@[000:0009) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0009) | | └─FunctionCallSyntax
//@[001:0007) | |   ├─IdentifierSyntax
//@[001:0007) | |   | └─Token(Identifier) |secure|
//@[007:0008) | |   ├─Token(LeftParen) |(|
//@[008:0009) | |   └─Token(RightParen) |)|
//@[009:0010) | ├─Token(NewLine) |\n|
@minLength(3)
//@[000:0013) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0013) | | └─FunctionCallSyntax
//@[001:0010) | |   ├─IdentifierSyntax
//@[001:0010) | |   | └─Token(Identifier) |minLength|
//@[010:0011) | |   ├─Token(LeftParen) |(|
//@[011:0012) | |   ├─FunctionArgumentSyntax
//@[011:0012) | |   | └─IntegerLiteralSyntax
//@[011:0012) | |   |   └─Token(Integer) |3|
//@[012:0013) | |   └─Token(RightParen) |)|
//@[013:0014) | ├─Token(NewLine) |\n|
@maxLength(24)
//@[000:0014) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0014) | | └─FunctionCallSyntax
//@[001:0010) | |   ├─IdentifierSyntax
//@[001:0010) | |   | └─Token(Identifier) |maxLength|
//@[010:0011) | |   ├─Token(LeftParen) |(|
//@[011:0013) | |   ├─FunctionArgumentSyntax
//@[011:0013) | |   | └─IntegerLiteralSyntax
//@[011:0013) | |   |   └─Token(Integer) |24|
//@[013:0014) | |   └─Token(RightParen) |)|
//@[014:0015) | ├─Token(NewLine) |\n|
@allowed([
//@[000:0039) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0039) | | └─FunctionCallSyntax
//@[001:0008) | |   ├─IdentifierSyntax
//@[001:0008) | |   | └─Token(Identifier) |allowed|
//@[008:0009) | |   ├─Token(LeftParen) |(|
//@[009:0038) | |   ├─FunctionArgumentSyntax
//@[009:0038) | |   | └─ArraySyntax
//@[009:0010) | |   |   ├─Token(LeftSquare) |[|
//@[010:0011) | |   |   ├─Token(NewLine) |\n|
  'one'
//@[002:0007) | |   |   ├─ArrayItemSyntax
//@[002:0007) | |   |   | └─StringSyntax
//@[002:0007) | |   |   |   └─Token(StringComplete) |'one'|
//@[007:0008) | |   |   ├─Token(NewLine) |\n|
  'two'
//@[002:0007) | |   |   ├─ArrayItemSyntax
//@[002:0007) | |   |   | └─StringSyntax
//@[002:0007) | |   |   |   └─Token(StringComplete) |'two'|
//@[007:0008) | |   |   ├─Token(NewLine) |\n|
  'three'
//@[002:0009) | |   |   ├─ArrayItemSyntax
//@[002:0009) | |   |   | └─StringSyntax
//@[002:0009) | |   |   |   └─Token(StringComplete) |'three'|
//@[009:0010) | |   |   ├─Token(NewLine) |\n|
])
//@[000:0001) | |   |   └─Token(RightSquare) |]|
//@[001:0002) | |   └─Token(RightParen) |)|
//@[002:0003) | ├─Token(NewLine) |\n|
@metadata({
//@[000:0059) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0059) | | └─FunctionCallSyntax
//@[001:0009) | |   ├─IdentifierSyntax
//@[001:0009) | |   | └─Token(Identifier) |metadata|
//@[009:0010) | |   ├─Token(LeftParen) |(|
//@[010:0058) | |   ├─FunctionArgumentSyntax
//@[010:0058) | |   | └─ObjectSyntax
//@[010:0011) | |   |   ├─Token(LeftBrace) |{|
//@[011:0012) | |   |   ├─Token(NewLine) |\n|
  description: 'Name of the storage account'
//@[002:0044) | |   |   ├─ObjectPropertySyntax
//@[002:0013) | |   |   | ├─IdentifierSyntax
//@[002:0013) | |   |   | | └─Token(Identifier) |description|
//@[013:0014) | |   |   | ├─Token(Colon) |:|
//@[015:0044) | |   |   | └─StringSyntax
//@[015:0044) | |   |   |   └─Token(StringComplete) |'Name of the storage account'|
//@[044:0045) | |   |   ├─Token(NewLine) |\n|
})
//@[000:0001) | |   |   └─Token(RightBrace) |}|
//@[001:0002) | |   └─Token(RightParen) |)|
//@[002:0003) | ├─Token(NewLine) |\n|
param someParameter string
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0019) | ├─IdentifierSyntax
//@[006:0019) | | └─Token(Identifier) |someParameter|
//@[020:0026) | └─TypeVariableAccessSyntax
//@[020:0026) |   └─IdentifierSyntax
//@[020:0026) |     └─Token(Identifier) |string|
//@[026:0028) ├─Token(NewLine) |\n\n|

param defaultExpression bool = 18 != (true || false)
//@[000:0052) ├─ParameterDeclarationSyntax
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0023) | ├─IdentifierSyntax
//@[006:0023) | | └─Token(Identifier) |defaultExpression|
//@[024:0028) | ├─TypeVariableAccessSyntax
//@[024:0028) | | └─IdentifierSyntax
//@[024:0028) | |   └─Token(Identifier) |bool|
//@[029:0052) | └─ParameterDefaultValueSyntax
//@[029:0030) |   ├─Token(Assignment) |=|
//@[031:0052) |   └─BinaryOperationSyntax
//@[031:0033) |     ├─IntegerLiteralSyntax
//@[031:0033) |     | └─Token(Integer) |18|
//@[034:0036) |     ├─Token(NotEquals) |!=|
//@[037:0052) |     └─ParenthesizedExpressionSyntax
//@[037:0038) |       ├─Token(LeftParen) |(|
//@[038:0051) |       ├─BinaryOperationSyntax
//@[038:0042) |       | ├─BooleanLiteralSyntax
//@[038:0042) |       | | └─Token(TrueKeyword) |true|
//@[043:0045) |       | ├─Token(LogicalOr) ||||
//@[046:0051) |       | └─BooleanLiteralSyntax
//@[046:0051) |       |   └─Token(FalseKeyword) |false|
//@[051:0052) |       └─Token(RightParen) |)|
//@[052:0054) ├─Token(NewLine) |\n\n|

@allowed([
//@[000:0056) ├─ParameterDeclarationSyntax
//@[000:0029) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0029) | | └─FunctionCallSyntax
//@[001:0008) | |   ├─IdentifierSyntax
//@[001:0008) | |   | └─Token(Identifier) |allowed|
//@[008:0009) | |   ├─Token(LeftParen) |(|
//@[009:0028) | |   ├─FunctionArgumentSyntax
//@[009:0028) | |   | └─ArraySyntax
//@[009:0010) | |   |   ├─Token(LeftSquare) |[|
//@[010:0011) | |   |   ├─Token(NewLine) |\n|
  'abc'
//@[002:0007) | |   |   ├─ArrayItemSyntax
//@[002:0007) | |   |   | └─StringSyntax
//@[002:0007) | |   |   |   └─Token(StringComplete) |'abc'|
//@[007:0008) | |   |   ├─Token(NewLine) |\n|
  'def'
//@[002:0007) | |   |   ├─ArrayItemSyntax
//@[002:0007) | |   |   | └─StringSyntax
//@[002:0007) | |   |   |   └─Token(StringComplete) |'def'|
//@[007:0008) | |   |   ├─Token(NewLine) |\n|
])
//@[000:0001) | |   |   └─Token(RightSquare) |]|
//@[001:0002) | |   └─Token(RightParen) |)|
//@[002:0003) | ├─Token(NewLine) |\n|
param stringLiteral string
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0019) | ├─IdentifierSyntax
//@[006:0019) | | └─Token(Identifier) |stringLiteral|
//@[020:0026) | └─TypeVariableAccessSyntax
//@[020:0026) |   └─IdentifierSyntax
//@[020:0026) |     └─Token(Identifier) |string|
//@[026:0028) ├─Token(NewLine) |\n\n|

@allowed(
//@[000:0130) ├─ParameterDeclarationSyntax
//@[000:0062) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0062) | | └─FunctionCallSyntax
//@[001:0008) | |   ├─IdentifierSyntax
//@[001:0008) | |   | └─Token(Identifier) |allowed|
//@[008:0009) | |   ├─Token(LeftParen) |(|
//@[009:0010) | |   ├─Token(NewLine) |\n|
    // some comment
//@[019:0020) | |   ├─Token(NewLine) |\n|
    [
//@[004:0031) | |   ├─FunctionArgumentSyntax
//@[004:0031) | |   | └─ArraySyntax
//@[004:0005) | |   |   ├─Token(LeftSquare) |[|
//@[005:0006) | |   |   ├─Token(NewLine) |\n|
  'abc'
//@[002:0007) | |   |   ├─ArrayItemSyntax
//@[002:0007) | |   |   | └─StringSyntax
//@[002:0007) | |   |   |   └─Token(StringComplete) |'abc'|
//@[007:0008) | |   |   ├─Token(NewLine) |\n|
  'def'
//@[002:0007) | |   |   ├─ArrayItemSyntax
//@[002:0007) | |   |   | └─StringSyntax
//@[002:0007) | |   |   |   └─Token(StringComplete) |'def'|
//@[007:0008) | |   |   ├─Token(NewLine) |\n|
  'ghi'
//@[002:0007) | |   |   ├─ArrayItemSyntax
//@[002:0007) | |   |   | └─StringSyntax
//@[002:0007) | |   |   |   └─Token(StringComplete) |'ghi'|
//@[007:0008) | |   |   ├─Token(NewLine) |\n|
])
//@[000:0001) | |   |   └─Token(RightSquare) |]|
//@[001:0002) | |   └─Token(RightParen) |)|
//@[002:0003) | ├─Token(NewLine) |\n|
param stringLiteralWithAllowedValuesSuperset string = stringLiteral
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0044) | ├─IdentifierSyntax
//@[006:0044) | | └─Token(Identifier) |stringLiteralWithAllowedValuesSuperset|
//@[045:0051) | ├─TypeVariableAccessSyntax
//@[045:0051) | | └─IdentifierSyntax
//@[045:0051) | |   └─Token(Identifier) |string|
//@[052:0067) | └─ParameterDefaultValueSyntax
//@[052:0053) |   ├─Token(Assignment) |=|
//@[054:0067) |   └─VariableAccessSyntax
//@[054:0067) |     └─IdentifierSyntax
//@[054:0067) |       └─Token(Identifier) |stringLiteral|
//@[067:0069) ├─Token(NewLine) |\n\n|

@secure()
//@[000:0104) ├─ParameterDeclarationSyntax
//@[000:0009) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0009) | | └─FunctionCallSyntax
//@[001:0007) | |   ├─IdentifierSyntax
//@[001:0007) | |   | └─Token(Identifier) |secure|
//@[007:0008) | |   ├─Token(LeftParen) |(|
//@[008:0009) | |   └─Token(RightParen) |)|
//@[009:0010) | ├─Token(NewLine) |\n|
@minLength(2)
//@[000:0013) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0013) | | └─FunctionCallSyntax
//@[001:0010) | |   ├─IdentifierSyntax
//@[001:0010) | |   | └─Token(Identifier) |minLength|
//@[010:0011) | |   ├─Token(LeftParen) |(|
//@[011:0012) | |   ├─FunctionArgumentSyntax
//@[011:0012) | |   | └─IntegerLiteralSyntax
//@[011:0012) | |   |   └─Token(Integer) |2|
//@[012:0013) | |   └─Token(RightParen) |)|
//@[013:0014) | ├─Token(NewLine) |\n|
  @maxLength(10)
//@[002:0016) | ├─DecoratorSyntax
//@[002:0003) | | ├─Token(At) |@|
//@[003:0016) | | └─FunctionCallSyntax
//@[003:0012) | |   ├─IdentifierSyntax
//@[003:0012) | |   | └─Token(Identifier) |maxLength|
//@[012:0013) | |   ├─Token(LeftParen) |(|
//@[013:0015) | |   ├─FunctionArgumentSyntax
//@[013:0015) | |   | └─IntegerLiteralSyntax
//@[013:0015) | |   |   └─Token(Integer) |10|
//@[015:0016) | |   └─Token(RightParen) |)|
//@[016:0017) | ├─Token(NewLine) |\n|
@allowed([
//@[000:0034) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0034) | | └─FunctionCallSyntax
//@[001:0008) | |   ├─IdentifierSyntax
//@[001:0008) | |   | └─Token(Identifier) |allowed|
//@[008:0009) | |   ├─Token(LeftParen) |(|
//@[009:0033) | |   ├─FunctionArgumentSyntax
//@[009:0033) | |   | └─ArraySyntax
//@[009:0010) | |   |   ├─Token(LeftSquare) |[|
//@[010:0011) | |   |   ├─Token(NewLine) |\n|
  'Apple'
//@[002:0009) | |   |   ├─ArrayItemSyntax
//@[002:0009) | |   |   | └─StringSyntax
//@[002:0009) | |   |   |   └─Token(StringComplete) |'Apple'|
//@[009:0010) | |   |   ├─Token(NewLine) |\n|
  'Banana'
//@[002:0010) | |   |   ├─ArrayItemSyntax
//@[002:0010) | |   |   | └─StringSyntax
//@[002:0010) | |   |   |   └─Token(StringComplete) |'Banana'|
//@[010:0011) | |   |   ├─Token(NewLine) |\n|
])
//@[000:0001) | |   |   └─Token(RightSquare) |]|
//@[001:0002) | |   └─Token(RightParen) |)|
//@[002:0003) | ├─Token(NewLine) |\n|
param decoratedString string
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0021) | ├─IdentifierSyntax
//@[006:0021) | | └─Token(Identifier) |decoratedString|
//@[022:0028) | └─TypeVariableAccessSyntax
//@[022:0028) |   └─IdentifierSyntax
//@[022:0028) |     └─Token(Identifier) |string|
//@[028:0030) ├─Token(NewLine) |\n\n|

@minValue(100)
//@[000:0043) ├─ParameterDeclarationSyntax
//@[000:0014) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0014) | | └─FunctionCallSyntax
//@[001:0009) | |   ├─IdentifierSyntax
//@[001:0009) | |   | └─Token(Identifier) |minValue|
//@[009:0010) | |   ├─Token(LeftParen) |(|
//@[010:0013) | |   ├─FunctionArgumentSyntax
//@[010:0013) | |   | └─IntegerLiteralSyntax
//@[010:0013) | |   |   └─Token(Integer) |100|
//@[013:0014) | |   └─Token(RightParen) |)|
//@[014:0015) | ├─Token(NewLine) |\n|
param decoratedInt int = 123
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0018) | ├─IdentifierSyntax
//@[006:0018) | | └─Token(Identifier) |decoratedInt|
//@[019:0022) | ├─TypeVariableAccessSyntax
//@[019:0022) | | └─IdentifierSyntax
//@[019:0022) | |   └─Token(Identifier) |int|
//@[023:0028) | └─ParameterDefaultValueSyntax
//@[023:0024) |   ├─Token(Assignment) |=|
//@[025:0028) |   └─IntegerLiteralSyntax
//@[025:0028) |     └─Token(Integer) |123|
//@[028:0030) ├─Token(NewLine) |\n\n|

// negative integer literals are allowed as decorator values
//@[060:0061) ├─Token(NewLine) |\n|
@minValue(-10)
//@[000:0053) ├─ParameterDeclarationSyntax
//@[000:0014) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0014) | | └─FunctionCallSyntax
//@[001:0009) | |   ├─IdentifierSyntax
//@[001:0009) | |   | └─Token(Identifier) |minValue|
//@[009:0010) | |   ├─Token(LeftParen) |(|
//@[010:0013) | |   ├─FunctionArgumentSyntax
//@[010:0013) | |   | └─UnaryOperationSyntax
//@[010:0011) | |   |   ├─Token(Minus) |-|
//@[011:0013) | |   |   └─IntegerLiteralSyntax
//@[011:0013) | |   |     └─Token(Integer) |10|
//@[013:0014) | |   └─Token(RightParen) |)|
//@[014:0015) | ├─Token(NewLine) |\n|
@maxValue(-3)
//@[000:0013) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0013) | | └─FunctionCallSyntax
//@[001:0009) | |   ├─IdentifierSyntax
//@[001:0009) | |   | └─Token(Identifier) |maxValue|
//@[009:0010) | |   ├─Token(LeftParen) |(|
//@[010:0012) | |   ├─FunctionArgumentSyntax
//@[010:0012) | |   | └─UnaryOperationSyntax
//@[010:0011) | |   |   ├─Token(Minus) |-|
//@[011:0012) | |   |   └─IntegerLiteralSyntax
//@[011:0012) | |   |     └─Token(Integer) |3|
//@[012:0013) | |   └─Token(RightParen) |)|
//@[013:0014) | ├─Token(NewLine) |\n|
param negativeValues int
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0020) | ├─IdentifierSyntax
//@[006:0020) | | └─Token(Identifier) |negativeValues|
//@[021:0024) | └─TypeVariableAccessSyntax
//@[021:0024) |   └─IdentifierSyntax
//@[021:0024) |     └─Token(Identifier) |int|
//@[024:0026) ├─Token(NewLine) |\n\n|

@sys.description('A boolean.')
//@[000:0283) ├─ParameterDeclarationSyntax
//@[000:0030) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0030) | | └─InstanceFunctionCallSyntax
//@[001:0004) | |   ├─VariableAccessSyntax
//@[001:0004) | |   | └─IdentifierSyntax
//@[001:0004) | |   |   └─Token(Identifier) |sys|
//@[004:0005) | |   ├─Token(Dot) |.|
//@[005:0016) | |   ├─IdentifierSyntax
//@[005:0016) | |   | └─Token(Identifier) |description|
//@[016:0017) | |   ├─Token(LeftParen) |(|
//@[017:0029) | |   ├─FunctionArgumentSyntax
//@[017:0029) | |   | └─StringSyntax
//@[017:0029) | |   |   └─Token(StringComplete) |'A boolean.'|
//@[029:0030) | |   └─Token(RightParen) |)|
//@[030:0031) | ├─Token(NewLine) |\n|
@metadata({
//@[000:0137) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0137) | | └─FunctionCallSyntax
//@[001:0009) | |   ├─IdentifierSyntax
//@[001:0009) | |   | └─Token(Identifier) |metadata|
//@[009:0010) | |   ├─Token(LeftParen) |(|
//@[010:0136) | |   ├─FunctionArgumentSyntax
//@[010:0136) | |   | └─ObjectSyntax
//@[010:0011) | |   |   ├─Token(LeftBrace) |{|
//@[011:0012) | |   |   ├─Token(NewLine) |\n|
    description: 'I will be overrode.'
//@[004:0038) | |   |   ├─ObjectPropertySyntax
//@[004:0015) | |   |   | ├─IdentifierSyntax
//@[004:0015) | |   |   | | └─Token(Identifier) |description|
//@[015:0016) | |   |   | ├─Token(Colon) |:|
//@[017:0038) | |   |   | └─StringSyntax
//@[017:0038) | |   |   |   └─Token(StringComplete) |'I will be overrode.'|
//@[038:0039) | |   |   ├─Token(NewLine) |\n|
    foo: 'something'
//@[004:0020) | |   |   ├─ObjectPropertySyntax
//@[004:0007) | |   |   | ├─IdentifierSyntax
//@[004:0007) | |   |   | | └─Token(Identifier) |foo|
//@[007:0008) | |   |   | ├─Token(Colon) |:|
//@[009:0020) | |   |   | └─StringSyntax
//@[009:0020) | |   |   |   └─Token(StringComplete) |'something'|
//@[020:0021) | |   |   ├─Token(NewLine) |\n|
    bar: [
//@[004:0062) | |   |   ├─ObjectPropertySyntax
//@[004:0007) | |   |   | ├─IdentifierSyntax
//@[004:0007) | |   |   | | └─Token(Identifier) |bar|
//@[007:0008) | |   |   | ├─Token(Colon) |:|
//@[009:0062) | |   |   | └─ArraySyntax
//@[009:0010) | |   |   |   ├─Token(LeftSquare) |[|
//@[010:0011) | |   |   |   ├─Token(NewLine) |\n|
        {          }
//@[008:0020) | |   |   |   ├─ArrayItemSyntax
//@[008:0020) | |   |   |   | └─ObjectSyntax
//@[008:0009) | |   |   |   |   ├─Token(LeftBrace) |{|
//@[019:0020) | |   |   |   |   └─Token(RightBrace) |}|
//@[020:0021) | |   |   |   ├─Token(NewLine) |\n|
        true
//@[008:0012) | |   |   |   ├─ArrayItemSyntax
//@[008:0012) | |   |   |   | └─BooleanLiteralSyntax
//@[008:0012) | |   |   |   |   └─Token(TrueKeyword) |true|
//@[012:0013) | |   |   |   ├─Token(NewLine) |\n|
        123
//@[008:0011) | |   |   |   ├─ArrayItemSyntax
//@[008:0011) | |   |   |   | └─IntegerLiteralSyntax
//@[008:0011) | |   |   |   |   └─Token(Integer) |123|
//@[011:0012) | |   |   |   ├─Token(NewLine) |\n|
    ]
//@[004:0005) | |   |   |   └─Token(RightSquare) |]|
//@[005:0006) | |   |   ├─Token(NewLine) |\n|
})
//@[000:0001) | |   |   └─Token(RightBrace) |}|
//@[001:0002) | |   └─Token(RightParen) |)|
//@[002:0003) | ├─Token(NewLine) |\n|
param decoratedBool bool = /* comment1 */ /* comment2*/      /* comment3 */ /* comment4 */ (true && false) != true
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0019) | ├─IdentifierSyntax
//@[006:0019) | | └─Token(Identifier) |decoratedBool|
//@[020:0024) | ├─TypeVariableAccessSyntax
//@[020:0024) | | └─IdentifierSyntax
//@[020:0024) | |   └─Token(Identifier) |bool|
//@[025:0114) | └─ParameterDefaultValueSyntax
//@[025:0026) |   ├─Token(Assignment) |=|
//@[091:0114) |   └─BinaryOperationSyntax
//@[091:0106) |     ├─ParenthesizedExpressionSyntax
//@[091:0092) |     | ├─Token(LeftParen) |(|
//@[092:0105) |     | ├─BinaryOperationSyntax
//@[092:0096) |     | | ├─BooleanLiteralSyntax
//@[092:0096) |     | | | └─Token(TrueKeyword) |true|
//@[097:0099) |     | | ├─Token(LogicalAnd) |&&|
//@[100:0105) |     | | └─BooleanLiteralSyntax
//@[100:0105) |     | |   └─Token(FalseKeyword) |false|
//@[105:0106) |     | └─Token(RightParen) |)|
//@[107:0109) |     ├─Token(NotEquals) |!=|
//@[110:0114) |     └─BooleanLiteralSyntax
//@[110:0114) |       └─Token(TrueKeyword) |true|
//@[114:0116) ├─Token(NewLine) |\n\n|

@secure()
//@[000:0254) ├─ParameterDeclarationSyntax
//@[000:0009) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0009) | | └─FunctionCallSyntax
//@[001:0007) | |   ├─IdentifierSyntax
//@[001:0007) | |   | └─Token(Identifier) |secure|
//@[007:0008) | |   ├─Token(LeftParen) |(|
//@[008:0009) | |   └─Token(RightParen) |)|
//@[009:0010) | ├─Token(NewLine) |\n|
param decoratedObject object = {
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0021) | ├─IdentifierSyntax
//@[006:0021) | | └─Token(Identifier) |decoratedObject|
//@[022:0028) | ├─TypeVariableAccessSyntax
//@[022:0028) | | └─IdentifierSyntax
//@[022:0028) | |   └─Token(Identifier) |object|
//@[029:0244) | └─ParameterDefaultValueSyntax
//@[029:0030) |   ├─Token(Assignment) |=|
//@[031:0244) |   └─ObjectSyntax
//@[031:0032) |     ├─Token(LeftBrace) |{|
//@[032:0033) |     ├─Token(NewLine) |\n|
  enabled: true
//@[002:0015) |     ├─ObjectPropertySyntax
//@[002:0009) |     | ├─IdentifierSyntax
//@[002:0009) |     | | └─Token(Identifier) |enabled|
//@[009:0010) |     | ├─Token(Colon) |:|
//@[011:0015) |     | └─BooleanLiteralSyntax
//@[011:0015) |     |   └─Token(TrueKeyword) |true|
//@[015:0016) |     ├─Token(NewLine) |\n|
  name: 'this is my object'
//@[002:0027) |     ├─ObjectPropertySyntax
//@[002:0006) |     | ├─IdentifierSyntax
//@[002:0006) |     | | └─Token(Identifier) |name|
//@[006:0007) |     | ├─Token(Colon) |:|
//@[008:0027) |     | └─StringSyntax
//@[008:0027) |     |   └─Token(StringComplete) |'this is my object'|
//@[027:0028) |     ├─Token(NewLine) |\n|
  priority: 3
//@[002:0013) |     ├─ObjectPropertySyntax
//@[002:0010) |     | ├─IdentifierSyntax
//@[002:0010) |     | | └─Token(Identifier) |priority|
//@[010:0011) |     | ├─Token(Colon) |:|
//@[012:0013) |     | └─IntegerLiteralSyntax
//@[012:0013) |     |   └─Token(Integer) |3|
//@[013:0014) |     ├─Token(NewLine) |\n|
  info: {
//@[002:0024) |     ├─ObjectPropertySyntax
//@[002:0006) |     | ├─IdentifierSyntax
//@[002:0006) |     | | └─Token(Identifier) |info|
//@[006:0007) |     | ├─Token(Colon) |:|
//@[008:0024) |     | └─ObjectSyntax
//@[008:0009) |     |   ├─Token(LeftBrace) |{|
//@[009:0010) |     |   ├─Token(NewLine) |\n|
    a: 'b'
//@[004:0010) |     |   ├─ObjectPropertySyntax
//@[004:0005) |     |   | ├─IdentifierSyntax
//@[004:0005) |     |   | | └─Token(Identifier) |a|
//@[005:0006) |     |   | ├─Token(Colon) |:|
//@[007:0010) |     |   | └─StringSyntax
//@[007:0010) |     |   |   └─Token(StringComplete) |'b'|
//@[010:0011) |     |   ├─Token(NewLine) |\n|
  }
//@[002:0003) |     |   └─Token(RightBrace) |}|
//@[003:0004) |     ├─Token(NewLine) |\n|
  empty: {
//@[002:0014) |     ├─ObjectPropertySyntax
//@[002:0007) |     | ├─IdentifierSyntax
//@[002:0007) |     | | └─Token(Identifier) |empty|
//@[007:0008) |     | ├─Token(Colon) |:|
//@[009:0014) |     | └─ObjectSyntax
//@[009:0010) |     |   ├─Token(LeftBrace) |{|
//@[010:0011) |     |   ├─Token(NewLine) |\n|
  }
//@[002:0003) |     |   └─Token(RightBrace) |}|
//@[003:0004) |     ├─Token(NewLine) |\n|
  array: [
//@[002:0111) |     ├─ObjectPropertySyntax
//@[002:0007) |     | ├─IdentifierSyntax
//@[002:0007) |     | | └─Token(Identifier) |array|
//@[007:0008) |     | ├─Token(Colon) |:|
//@[009:0111) |     | └─ArraySyntax
//@[009:0010) |     |   ├─Token(LeftSquare) |[|
//@[010:0011) |     |   ├─Token(NewLine) |\n|
    'string item'
//@[004:0017) |     |   ├─ArrayItemSyntax
//@[004:0017) |     |   | └─StringSyntax
//@[004:0017) |     |   |   └─Token(StringComplete) |'string item'|
//@[017:0018) |     |   ├─Token(NewLine) |\n|
    12
//@[004:0006) |     |   ├─ArrayItemSyntax
//@[004:0006) |     |   | └─IntegerLiteralSyntax
//@[004:0006) |     |   |   └─Token(Integer) |12|
//@[006:0007) |     |   ├─Token(NewLine) |\n|
    true
//@[004:0008) |     |   ├─ArrayItemSyntax
//@[004:0008) |     |   | └─BooleanLiteralSyntax
//@[004:0008) |     |   |   └─Token(TrueKeyword) |true|
//@[008:0009) |     |   ├─Token(NewLine) |\n|
    [
//@[004:0037) |     |   ├─ArrayItemSyntax
//@[004:0037) |     |   | └─ArraySyntax
//@[004:0005) |     |   |   ├─Token(LeftSquare) |[|
//@[005:0006) |     |   |   ├─Token(NewLine) |\n|
      'inner'
//@[006:0013) |     |   |   ├─ArrayItemSyntax
//@[006:0013) |     |   |   | └─StringSyntax
//@[006:0013) |     |   |   |   └─Token(StringComplete) |'inner'|
//@[013:0014) |     |   |   ├─Token(NewLine) |\n|
      false
//@[006:0011) |     |   |   ├─ArrayItemSyntax
//@[006:0011) |     |   |   | └─BooleanLiteralSyntax
//@[006:0011) |     |   |   |   └─Token(FalseKeyword) |false|
//@[011:0012) |     |   |   ├─Token(NewLine) |\n|
    ]
//@[004:0005) |     |   |   └─Token(RightSquare) |]|
//@[005:0006) |     |   ├─Token(NewLine) |\n|
    {
//@[004:0024) |     |   ├─ArrayItemSyntax
//@[004:0024) |     |   | └─ObjectSyntax
//@[004:0005) |     |   |   ├─Token(LeftBrace) |{|
//@[005:0006) |     |   |   ├─Token(NewLine) |\n|
      a: 'b'
//@[006:0012) |     |   |   ├─ObjectPropertySyntax
//@[006:0007) |     |   |   | ├─IdentifierSyntax
//@[006:0007) |     |   |   | | └─Token(Identifier) |a|
//@[007:0008) |     |   |   | ├─Token(Colon) |:|
//@[009:0012) |     |   |   | └─StringSyntax
//@[009:0012) |     |   |   |   └─Token(StringComplete) |'b'|
//@[012:0013) |     |   |   ├─Token(NewLine) |\n|
    }
//@[004:0005) |     |   |   └─Token(RightBrace) |}|
//@[005:0006) |     |   ├─Token(NewLine) |\n|
  ]
//@[002:0003) |     |   └─Token(RightSquare) |]|
//@[003:0004) |     ├─Token(NewLine) |\n|
}
//@[000:0001) |     └─Token(RightBrace) |}|
//@[001:0003) ├─Token(NewLine) |\n\n|

@sys.metadata({
//@[000:0166) ├─ParameterDeclarationSyntax
//@[000:0057) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0057) | | └─InstanceFunctionCallSyntax
//@[001:0004) | |   ├─VariableAccessSyntax
//@[001:0004) | |   | └─IdentifierSyntax
//@[001:0004) | |   |   └─Token(Identifier) |sys|
//@[004:0005) | |   ├─Token(Dot) |.|
//@[005:0013) | |   ├─IdentifierSyntax
//@[005:0013) | |   | └─Token(Identifier) |metadata|
//@[013:0014) | |   ├─Token(LeftParen) |(|
//@[014:0056) | |   ├─FunctionArgumentSyntax
//@[014:0056) | |   | └─ObjectSyntax
//@[014:0015) | |   |   ├─Token(LeftBrace) |{|
//@[015:0016) | |   |   ├─Token(NewLine) |\n|
    description: 'I will be overrode.'
//@[004:0038) | |   |   ├─ObjectPropertySyntax
//@[004:0015) | |   |   | ├─IdentifierSyntax
//@[004:0015) | |   |   | | └─Token(Identifier) |description|
//@[015:0016) | |   |   | ├─Token(Colon) |:|
//@[017:0038) | |   |   | └─StringSyntax
//@[017:0038) | |   |   |   └─Token(StringComplete) |'I will be overrode.'|
//@[038:0039) | |   |   ├─Token(NewLine) |\n|
})
//@[000:0001) | |   |   └─Token(RightBrace) |}|
//@[001:0002) | |   └─Token(RightParen) |)|
//@[002:0003) | ├─Token(NewLine) |\n|
@sys.maxLength(20)
//@[000:0018) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0018) | | └─InstanceFunctionCallSyntax
//@[001:0004) | |   ├─VariableAccessSyntax
//@[001:0004) | |   | └─IdentifierSyntax
//@[001:0004) | |   |   └─Token(Identifier) |sys|
//@[004:0005) | |   ├─Token(Dot) |.|
//@[005:0014) | |   ├─IdentifierSyntax
//@[005:0014) | |   | └─Token(Identifier) |maxLength|
//@[014:0015) | |   ├─Token(LeftParen) |(|
//@[015:0017) | |   ├─FunctionArgumentSyntax
//@[015:0017) | |   | └─IntegerLiteralSyntax
//@[015:0017) | |   |   └─Token(Integer) |20|
//@[017:0018) | |   └─Token(RightParen) |)|
//@[018:0019) | ├─Token(NewLine) |\n|
@sys.description('An array.')
//@[000:0029) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0029) | | └─InstanceFunctionCallSyntax
//@[001:0004) | |   ├─VariableAccessSyntax
//@[001:0004) | |   | └─IdentifierSyntax
//@[001:0004) | |   |   └─Token(Identifier) |sys|
//@[004:0005) | |   ├─Token(Dot) |.|
//@[005:0016) | |   ├─IdentifierSyntax
//@[005:0016) | |   | └─Token(Identifier) |description|
//@[016:0017) | |   ├─Token(LeftParen) |(|
//@[017:0028) | |   ├─FunctionArgumentSyntax
//@[017:0028) | |   | └─StringSyntax
//@[017:0028) | |   |   └─Token(StringComplete) |'An array.'|
//@[028:0029) | |   └─Token(RightParen) |)|
//@[029:0030) | ├─Token(NewLine) |\n|
param decoratedArray array = [
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0020) | ├─IdentifierSyntax
//@[006:0020) | | └─Token(Identifier) |decoratedArray|
//@[021:0026) | ├─TypeVariableAccessSyntax
//@[021:0026) | | └─IdentifierSyntax
//@[021:0026) | |   └─Token(Identifier) |array|
//@[027:0059) | └─ParameterDefaultValueSyntax
//@[027:0028) |   ├─Token(Assignment) |=|
//@[029:0059) |   └─ArraySyntax
//@[029:0030) |     ├─Token(LeftSquare) |[|
//@[030:0031) |     ├─Token(NewLine) |\n|
    utcNow()
//@[004:0012) |     ├─ArrayItemSyntax
//@[004:0012) |     | └─FunctionCallSyntax
//@[004:0010) |     |   ├─IdentifierSyntax
//@[004:0010) |     |   | └─Token(Identifier) |utcNow|
//@[010:0011) |     |   ├─Token(LeftParen) |(|
//@[011:0012) |     |   └─Token(RightParen) |)|
//@[012:0013) |     ├─Token(NewLine) |\n|
    newGuid()
//@[004:0013) |     ├─ArrayItemSyntax
//@[004:0013) |     | └─FunctionCallSyntax
//@[004:0011) |     |   ├─IdentifierSyntax
//@[004:0011) |     |   | └─Token(Identifier) |newGuid|
//@[011:0012) |     |   ├─Token(LeftParen) |(|
//@[012:0013) |     |   └─Token(RightParen) |)|
//@[013:0014) |     ├─Token(NewLine) |\n|
]
//@[000:0001) |     └─Token(RightSquare) |]|
//@[001:0003) ├─Token(NewLine) |\n\n|

param nameofParam string = nameof(decoratedArray)
//@[000:0049) ├─ParameterDeclarationSyntax
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0017) | ├─IdentifierSyntax
//@[006:0017) | | └─Token(Identifier) |nameofParam|
//@[018:0024) | ├─TypeVariableAccessSyntax
//@[018:0024) | | └─IdentifierSyntax
//@[018:0024) | |   └─Token(Identifier) |string|
//@[025:0049) | └─ParameterDefaultValueSyntax
//@[025:0026) |   ├─Token(Assignment) |=|
//@[027:0049) |   └─FunctionCallSyntax
//@[027:0033) |     ├─IdentifierSyntax
//@[027:0033) |     | └─Token(Identifier) |nameof|
//@[033:0034) |     ├─Token(LeftParen) |(|
//@[034:0048) |     ├─FunctionArgumentSyntax
//@[034:0048) |     | └─VariableAccessSyntax
//@[034:0048) |     |   └─IdentifierSyntax
//@[034:0048) |     |     └─Token(Identifier) |decoratedArray|
//@[048:0049) |     └─Token(RightParen) |)|
//@[049:0050) ├─Token(NewLine) |\n|

//@[000:0000) └─Token(EndOfFile) ||
