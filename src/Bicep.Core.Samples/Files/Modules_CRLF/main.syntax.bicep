
//@[000:8466) ProgramSyntax
//@[000:0002) ├─Token(NewLine) |\r\n|
@sys.description('this is deployTimeSuffix param')
//@[000:0093) ├─ParameterDeclarationSyntax
//@[000:0050) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0050) | | └─InstanceFunctionCallSyntax
//@[001:0004) | |   ├─VariableAccessSyntax
//@[001:0004) | |   | └─IdentifierSyntax
//@[001:0004) | |   |   └─Token(Identifier) |sys|
//@[004:0005) | |   ├─Token(Dot) |.|
//@[005:0016) | |   ├─IdentifierSyntax
//@[005:0016) | |   | └─Token(Identifier) |description|
//@[016:0017) | |   ├─Token(LeftParen) |(|
//@[017:0049) | |   ├─FunctionArgumentSyntax
//@[017:0049) | |   | └─StringSyntax
//@[017:0049) | |   |   └─Token(StringComplete) |'this is deployTimeSuffix param'|
//@[049:0050) | |   └─Token(RightParen) |)|
//@[050:0052) | ├─Token(NewLine) |\r\n|
param deployTimeSuffix string = newGuid()
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0022) | ├─IdentifierSyntax
//@[006:0022) | | └─Token(Identifier) |deployTimeSuffix|
//@[023:0029) | ├─SimpleTypeSyntax
//@[023:0029) | | └─Token(Identifier) |string|
//@[030:0041) | └─ParameterDefaultValueSyntax
//@[030:0031) |   ├─Token(Assignment) |=|
//@[032:0041) |   └─FunctionCallSyntax
//@[032:0039) |     ├─IdentifierSyntax
//@[032:0039) |     | └─Token(Identifier) |newGuid|
//@[039:0040) |     ├─Token(LeftParen) |(|
//@[040:0041) |     └─Token(RightParen) |)|
//@[041:0045) ├─Token(NewLine) |\r\n\r\n|

@sys.description('this module a')
//@[000:0252) ├─ModuleDeclarationSyntax
//@[000:0033) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0033) | | └─InstanceFunctionCallSyntax
//@[001:0004) | |   ├─VariableAccessSyntax
//@[001:0004) | |   | └─IdentifierSyntax
//@[001:0004) | |   |   └─Token(Identifier) |sys|
//@[004:0005) | |   ├─Token(Dot) |.|
//@[005:0016) | |   ├─IdentifierSyntax
//@[005:0016) | |   | └─Token(Identifier) |description|
//@[016:0017) | |   ├─Token(LeftParen) |(|
//@[017:0032) | |   ├─FunctionArgumentSyntax
//@[017:0032) | |   | └─StringSyntax
//@[017:0032) | |   |   └─Token(StringComplete) |'this module a'|
//@[032:0033) | |   └─Token(RightParen) |)|
//@[033:0035) | ├─Token(NewLine) |\r\n|
module modATest './modulea.bicep' = {
//@[000:0006) | ├─Token(Identifier) |module|
//@[007:0015) | ├─IdentifierSyntax
//@[007:0015) | | └─Token(Identifier) |modATest|
//@[016:0033) | ├─StringSyntax
//@[016:0033) | | └─Token(StringComplete) |'./modulea.bicep'|
//@[034:0035) | ├─Token(Assignment) |=|
//@[036:0217) | └─ObjectSyntax
//@[036:0037) |   ├─Token(LeftBrace) |{|
//@[037:0039) |   ├─Token(NewLine) |\r\n|
  name: 'modATest'
//@[002:0018) |   ├─ObjectPropertySyntax
//@[002:0006) |   | ├─IdentifierSyntax
//@[002:0006) |   | | └─Token(Identifier) |name|
//@[006:0007) |   | ├─Token(Colon) |:|
//@[008:0018) |   | └─StringSyntax
//@[008:0018) |   |   └─Token(StringComplete) |'modATest'|
//@[018:0020) |   ├─Token(NewLine) |\r\n|
  params: {
//@[002:0155) |   ├─ObjectPropertySyntax
//@[002:0008) |   | ├─IdentifierSyntax
//@[002:0008) |   | | └─Token(Identifier) |params|
//@[008:0009) |   | ├─Token(Colon) |:|
//@[010:0155) |   | └─ObjectSyntax
//@[010:0011) |   |   ├─Token(LeftBrace) |{|
//@[011:0013) |   |   ├─Token(NewLine) |\r\n|
    stringParamB: 'hello!'
//@[004:0026) |   |   ├─ObjectPropertySyntax
//@[004:0016) |   |   | ├─IdentifierSyntax
//@[004:0016) |   |   | | └─Token(Identifier) |stringParamB|
//@[016:0017) |   |   | ├─Token(Colon) |:|
//@[018:0026) |   |   | └─StringSyntax
//@[018:0026) |   |   |   └─Token(StringComplete) |'hello!'|
//@[026:0028) |   |   ├─Token(NewLine) |\r\n|
    objParam: {
//@[004:0036) |   |   ├─ObjectPropertySyntax
//@[004:0012) |   |   | ├─IdentifierSyntax
//@[004:0012) |   |   | | └─Token(Identifier) |objParam|
//@[012:0013) |   |   | ├─Token(Colon) |:|
//@[014:0036) |   |   | └─ObjectSyntax
//@[014:0015) |   |   |   ├─Token(LeftBrace) |{|
//@[015:0017) |   |   |   ├─Token(NewLine) |\r\n|
      a: 'b'
//@[006:0012) |   |   |   ├─ObjectPropertySyntax
//@[006:0007) |   |   |   | ├─IdentifierSyntax
//@[006:0007) |   |   |   | | └─Token(Identifier) |a|
//@[007:0008) |   |   |   | ├─Token(Colon) |:|
//@[009:0012) |   |   |   | └─StringSyntax
//@[009:0012) |   |   |   |   └─Token(StringComplete) |'b'|
//@[012:0014) |   |   |   ├─Token(NewLine) |\r\n|
    }
//@[004:0005) |   |   |   └─Token(RightBrace) |}|
//@[005:0007) |   |   ├─Token(NewLine) |\r\n|
    arrayParam: [
//@[004:0071) |   |   ├─ObjectPropertySyntax
//@[004:0014) |   |   | ├─IdentifierSyntax
//@[004:0014) |   |   | | └─Token(Identifier) |arrayParam|
//@[014:0015) |   |   | ├─Token(Colon) |:|
//@[016:0071) |   |   | └─ArraySyntax
//@[016:0017) |   |   |   ├─Token(LeftSquare) |[|
//@[017:0019) |   |   |   ├─Token(NewLine) |\r\n|
      {
//@[006:0032) |   |   |   ├─ArrayItemSyntax
//@[006:0032) |   |   |   | └─ObjectSyntax
//@[006:0007) |   |   |   |   ├─Token(LeftBrace) |{|
//@[007:0009) |   |   |   |   ├─Token(NewLine) |\r\n|
        a: 'b'
//@[008:0014) |   |   |   |   ├─ObjectPropertySyntax
//@[008:0009) |   |   |   |   | ├─IdentifierSyntax
//@[008:0009) |   |   |   |   | | └─Token(Identifier) |a|
//@[009:0010) |   |   |   |   | ├─Token(Colon) |:|
//@[011:0014) |   |   |   |   | └─StringSyntax
//@[011:0014) |   |   |   |   |   └─Token(StringComplete) |'b'|
//@[014:0016) |   |   |   |   ├─Token(NewLine) |\r\n|
      }
//@[006:0007) |   |   |   |   └─Token(RightBrace) |}|
//@[007:0009) |   |   |   ├─Token(NewLine) |\r\n|
      'abc'
//@[006:0011) |   |   |   ├─ArrayItemSyntax
//@[006:0011) |   |   |   | └─StringSyntax
//@[006:0011) |   |   |   |   └─Token(StringComplete) |'abc'|
//@[011:0013) |   |   |   ├─Token(NewLine) |\r\n|
    ]
//@[004:0005) |   |   |   └─Token(RightSquare) |]|
//@[005:0007) |   |   ├─Token(NewLine) |\r\n|
  }
//@[002:0003) |   |   └─Token(RightBrace) |}|
//@[003:0005) |   ├─Token(NewLine) |\r\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0007) ├─Token(NewLine) |\r\n\r\n\r\n|


@sys.description('this module b')
//@[000:0136) ├─ModuleDeclarationSyntax
//@[000:0033) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0033) | | └─InstanceFunctionCallSyntax
//@[001:0004) | |   ├─VariableAccessSyntax
//@[001:0004) | |   | └─IdentifierSyntax
//@[001:0004) | |   |   └─Token(Identifier) |sys|
//@[004:0005) | |   ├─Token(Dot) |.|
//@[005:0016) | |   ├─IdentifierSyntax
//@[005:0016) | |   | └─Token(Identifier) |description|
//@[016:0017) | |   ├─Token(LeftParen) |(|
//@[017:0032) | |   ├─FunctionArgumentSyntax
//@[017:0032) | |   | └─StringSyntax
//@[017:0032) | |   |   └─Token(StringComplete) |'this module b'|
//@[032:0033) | |   └─Token(RightParen) |)|
//@[033:0035) | ├─Token(NewLine) |\r\n|
module modB './child/moduleb.bicep' = {
//@[000:0006) | ├─Token(Identifier) |module|
//@[007:0011) | ├─IdentifierSyntax
//@[007:0011) | | └─Token(Identifier) |modB|
//@[012:0035) | ├─StringSyntax
//@[012:0035) | | └─Token(StringComplete) |'./child/moduleb.bicep'|
//@[036:0037) | ├─Token(Assignment) |=|
//@[038:0101) | └─ObjectSyntax
//@[038:0039) |   ├─Token(LeftBrace) |{|
//@[039:0041) |   ├─Token(NewLine) |\r\n|
  name: 'modB'
//@[002:0014) |   ├─ObjectPropertySyntax
//@[002:0006) |   | ├─IdentifierSyntax
//@[002:0006) |   | | └─Token(Identifier) |name|
//@[006:0007) |   | ├─Token(Colon) |:|
//@[008:0014) |   | └─StringSyntax
//@[008:0014) |   |   └─Token(StringComplete) |'modB'|
//@[014:0016) |   ├─Token(NewLine) |\r\n|
  params: {
//@[002:0041) |   ├─ObjectPropertySyntax
//@[002:0008) |   | ├─IdentifierSyntax
//@[002:0008) |   | | └─Token(Identifier) |params|
//@[008:0009) |   | ├─Token(Colon) |:|
//@[010:0041) |   | └─ObjectSyntax
//@[010:0011) |   |   ├─Token(LeftBrace) |{|
//@[011:0013) |   |   ├─Token(NewLine) |\r\n|
    location: 'West US'
//@[004:0023) |   |   ├─ObjectPropertySyntax
//@[004:0012) |   |   | ├─IdentifierSyntax
//@[004:0012) |   |   | | └─Token(Identifier) |location|
//@[012:0013) |   |   | ├─Token(Colon) |:|
//@[014:0023) |   |   | └─StringSyntax
//@[014:0023) |   |   |   └─Token(StringComplete) |'West US'|
//@[023:0025) |   |   ├─Token(NewLine) |\r\n|
  }
//@[002:0003) |   |   └─Token(RightBrace) |}|
//@[003:0005) |   ├─Token(NewLine) |\r\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0005) ├─Token(NewLine) |\r\n\r\n|

@sys.description('this is just module b with a condition')
//@[000:0203) ├─ModuleDeclarationSyntax
//@[000:0058) | ├─DecoratorSyntax
//@[000:0001) | | ├─Token(At) |@|
//@[001:0058) | | └─InstanceFunctionCallSyntax
//@[001:0004) | |   ├─VariableAccessSyntax
//@[001:0004) | |   | └─IdentifierSyntax
//@[001:0004) | |   |   └─Token(Identifier) |sys|
//@[004:0005) | |   ├─Token(Dot) |.|
//@[005:0016) | |   ├─IdentifierSyntax
//@[005:0016) | |   | └─Token(Identifier) |description|
//@[016:0017) | |   ├─Token(LeftParen) |(|
//@[017:0057) | |   ├─FunctionArgumentSyntax
//@[017:0057) | |   | └─StringSyntax
//@[017:0057) | |   |   └─Token(StringComplete) |'this is just module b with a condition'|
//@[057:0058) | |   └─Token(RightParen) |)|
//@[058:0060) | ├─Token(NewLine) |\r\n|
module modBWithCondition './child/moduleb.bicep' = if (1 + 1 == 2) {
//@[000:0006) | ├─Token(Identifier) |module|
//@[007:0024) | ├─IdentifierSyntax
//@[007:0024) | | └─Token(Identifier) |modBWithCondition|
//@[025:0048) | ├─StringSyntax
//@[025:0048) | | └─Token(StringComplete) |'./child/moduleb.bicep'|
//@[049:0050) | ├─Token(Assignment) |=|
//@[051:0143) | └─IfConditionSyntax
//@[051:0053) |   ├─Token(Identifier) |if|
//@[054:0066) |   ├─ParenthesizedExpressionSyntax
//@[054:0055) |   | ├─Token(LeftParen) |(|
//@[055:0065) |   | ├─BinaryOperationSyntax
//@[055:0060) |   | | ├─BinaryOperationSyntax
//@[055:0056) |   | | | ├─IntegerLiteralSyntax
//@[055:0056) |   | | | | └─Token(Integer) |1|
//@[057:0058) |   | | | ├─Token(Plus) |+|
//@[059:0060) |   | | | └─IntegerLiteralSyntax
//@[059:0060) |   | | |   └─Token(Integer) |1|
//@[061:0063) |   | | ├─Token(Equals) |==|
//@[064:0065) |   | | └─IntegerLiteralSyntax
//@[064:0065) |   | |   └─Token(Integer) |2|
//@[065:0066) |   | └─Token(RightParen) |)|
//@[067:0143) |   └─ObjectSyntax
//@[067:0068) |     ├─Token(LeftBrace) |{|
//@[068:0070) |     ├─Token(NewLine) |\r\n|
  name: 'modBWithCondition'
//@[002:0027) |     ├─ObjectPropertySyntax
//@[002:0006) |     | ├─IdentifierSyntax
//@[002:0006) |     | | └─Token(Identifier) |name|
//@[006:0007) |     | ├─Token(Colon) |:|
//@[008:0027) |     | └─StringSyntax
//@[008:0027) |     |   └─Token(StringComplete) |'modBWithCondition'|
//@[027:0029) |     ├─Token(NewLine) |\r\n|
  params: {
//@[002:0041) |     ├─ObjectPropertySyntax
//@[002:0008) |     | ├─IdentifierSyntax
//@[002:0008) |     | | └─Token(Identifier) |params|
//@[008:0009) |     | ├─Token(Colon) |:|
//@[010:0041) |     | └─ObjectSyntax
//@[010:0011) |     |   ├─Token(LeftBrace) |{|
//@[011:0013) |     |   ├─Token(NewLine) |\r\n|
    location: 'East US'
//@[004:0023) |     |   ├─ObjectPropertySyntax
//@[004:0012) |     |   | ├─IdentifierSyntax
//@[004:0012) |     |   | | └─Token(Identifier) |location|
//@[012:0013) |     |   | ├─Token(Colon) |:|
//@[014:0023) |     |   | └─StringSyntax
//@[014:0023) |     |   |   └─Token(StringComplete) |'East US'|
//@[023:0025) |     |   ├─Token(NewLine) |\r\n|
  }
//@[002:0003) |     |   └─Token(RightBrace) |}|
//@[003:0005) |     ├─Token(NewLine) |\r\n|
}
//@[000:0001) |     └─Token(RightBrace) |}|
//@[001:0005) ├─Token(NewLine) |\r\n\r\n|

module modC './child/modulec.json' = {
//@[000:0100) ├─ModuleDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |module|
//@[007:0011) | ├─IdentifierSyntax
//@[007:0011) | | └─Token(Identifier) |modC|
//@[012:0034) | ├─StringSyntax
//@[012:0034) | | └─Token(StringComplete) |'./child/modulec.json'|
//@[035:0036) | ├─Token(Assignment) |=|
//@[037:0100) | └─ObjectSyntax
//@[037:0038) |   ├─Token(LeftBrace) |{|
//@[038:0040) |   ├─Token(NewLine) |\r\n|
  name: 'modC'
//@[002:0014) |   ├─ObjectPropertySyntax
//@[002:0006) |   | ├─IdentifierSyntax
//@[002:0006) |   | | └─Token(Identifier) |name|
//@[006:0007) |   | ├─Token(Colon) |:|
//@[008:0014) |   | └─StringSyntax
//@[008:0014) |   |   └─Token(StringComplete) |'modC'|
//@[014:0016) |   ├─Token(NewLine) |\r\n|
  params: {
//@[002:0041) |   ├─ObjectPropertySyntax
//@[002:0008) |   | ├─IdentifierSyntax
//@[002:0008) |   | | └─Token(Identifier) |params|
//@[008:0009) |   | ├─Token(Colon) |:|
//@[010:0041) |   | └─ObjectSyntax
//@[010:0011) |   |   ├─Token(LeftBrace) |{|
//@[011:0013) |   |   ├─Token(NewLine) |\r\n|
    location: 'West US'
//@[004:0023) |   |   ├─ObjectPropertySyntax
//@[004:0012) |   |   | ├─IdentifierSyntax
//@[004:0012) |   |   | | └─Token(Identifier) |location|
//@[012:0013) |   |   | ├─Token(Colon) |:|
//@[014:0023) |   |   | └─StringSyntax
//@[014:0023) |   |   |   └─Token(StringComplete) |'West US'|
//@[023:0025) |   |   ├─Token(NewLine) |\r\n|
  }
//@[002:0003) |   |   └─Token(RightBrace) |}|
//@[003:0005) |   ├─Token(NewLine) |\r\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0005) ├─Token(NewLine) |\r\n\r\n|

module modCWithCondition './child/modulec.json' = if (2 - 1 == 1) {
//@[000:0142) ├─ModuleDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |module|
//@[007:0024) | ├─IdentifierSyntax
//@[007:0024) | | └─Token(Identifier) |modCWithCondition|
//@[025:0047) | ├─StringSyntax
//@[025:0047) | | └─Token(StringComplete) |'./child/modulec.json'|
//@[048:0049) | ├─Token(Assignment) |=|
//@[050:0142) | └─IfConditionSyntax
//@[050:0052) |   ├─Token(Identifier) |if|
//@[053:0065) |   ├─ParenthesizedExpressionSyntax
//@[053:0054) |   | ├─Token(LeftParen) |(|
//@[054:0064) |   | ├─BinaryOperationSyntax
//@[054:0059) |   | | ├─BinaryOperationSyntax
//@[054:0055) |   | | | ├─IntegerLiteralSyntax
//@[054:0055) |   | | | | └─Token(Integer) |2|
//@[056:0057) |   | | | ├─Token(Minus) |-|
//@[058:0059) |   | | | └─IntegerLiteralSyntax
//@[058:0059) |   | | |   └─Token(Integer) |1|
//@[060:0062) |   | | ├─Token(Equals) |==|
//@[063:0064) |   | | └─IntegerLiteralSyntax
//@[063:0064) |   | |   └─Token(Integer) |1|
//@[064:0065) |   | └─Token(RightParen) |)|
//@[066:0142) |   └─ObjectSyntax
//@[066:0067) |     ├─Token(LeftBrace) |{|
//@[067:0069) |     ├─Token(NewLine) |\r\n|
  name: 'modCWithCondition'
//@[002:0027) |     ├─ObjectPropertySyntax
//@[002:0006) |     | ├─IdentifierSyntax
//@[002:0006) |     | | └─Token(Identifier) |name|
//@[006:0007) |     | ├─Token(Colon) |:|
//@[008:0027) |     | └─StringSyntax
//@[008:0027) |     |   └─Token(StringComplete) |'modCWithCondition'|
//@[027:0029) |     ├─Token(NewLine) |\r\n|
  params: {
//@[002:0041) |     ├─ObjectPropertySyntax
//@[002:0008) |     | ├─IdentifierSyntax
//@[002:0008) |     | | └─Token(Identifier) |params|
//@[008:0009) |     | ├─Token(Colon) |:|
//@[010:0041) |     | └─ObjectSyntax
//@[010:0011) |     |   ├─Token(LeftBrace) |{|
//@[011:0013) |     |   ├─Token(NewLine) |\r\n|
    location: 'East US'
//@[004:0023) |     |   ├─ObjectPropertySyntax
//@[004:0012) |     |   | ├─IdentifierSyntax
//@[004:0012) |     |   | | └─Token(Identifier) |location|
//@[012:0013) |     |   | ├─Token(Colon) |:|
//@[014:0023) |     |   | └─StringSyntax
//@[014:0023) |     |   |   └─Token(StringComplete) |'East US'|
//@[023:0025) |     |   ├─Token(NewLine) |\r\n|
  }
//@[002:0003) |     |   └─Token(RightBrace) |}|
//@[003:0005) |     ├─Token(NewLine) |\r\n|
}
//@[000:0001) |     └─Token(RightBrace) |}|
//@[001:0005) ├─Token(NewLine) |\r\n\r\n|

module optionalWithNoParams1 './child/optionalParams.bicep'= {
//@[000:0098) ├─ModuleDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |module|
//@[007:0028) | ├─IdentifierSyntax
//@[007:0028) | | └─Token(Identifier) |optionalWithNoParams1|
//@[029:0059) | ├─StringSyntax
//@[029:0059) | | └─Token(StringComplete) |'./child/optionalParams.bicep'|
//@[059:0060) | ├─Token(Assignment) |=|
//@[061:0098) | └─ObjectSyntax
//@[061:0062) |   ├─Token(LeftBrace) |{|
//@[062:0064) |   ├─Token(NewLine) |\r\n|
  name: 'optionalWithNoParams1'
//@[002:0031) |   ├─ObjectPropertySyntax
//@[002:0006) |   | ├─IdentifierSyntax
//@[002:0006) |   | | └─Token(Identifier) |name|
//@[006:0007) |   | ├─Token(Colon) |:|
//@[008:0031) |   | └─StringSyntax
//@[008:0031) |   |   └─Token(StringComplete) |'optionalWithNoParams1'|
//@[031:0033) |   ├─Token(NewLine) |\r\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0005) ├─Token(NewLine) |\r\n\r\n|

module optionalWithNoParams2 './child/optionalParams.bicep'= {
//@[000:0116) ├─ModuleDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |module|
//@[007:0028) | ├─IdentifierSyntax
//@[007:0028) | | └─Token(Identifier) |optionalWithNoParams2|
//@[029:0059) | ├─StringSyntax
//@[029:0059) | | └─Token(StringComplete) |'./child/optionalParams.bicep'|
//@[059:0060) | ├─Token(Assignment) |=|
//@[061:0116) | └─ObjectSyntax
//@[061:0062) |   ├─Token(LeftBrace) |{|
//@[062:0064) |   ├─Token(NewLine) |\r\n|
  name: 'optionalWithNoParams2'
//@[002:0031) |   ├─ObjectPropertySyntax
//@[002:0006) |   | ├─IdentifierSyntax
//@[002:0006) |   | | └─Token(Identifier) |name|
//@[006:0007) |   | ├─Token(Colon) |:|
//@[008:0031) |   | └─StringSyntax
//@[008:0031) |   |   └─Token(StringComplete) |'optionalWithNoParams2'|
//@[031:0033) |   ├─Token(NewLine) |\r\n|
  params: {
//@[002:0016) |   ├─ObjectPropertySyntax
//@[002:0008) |   | ├─IdentifierSyntax
//@[002:0008) |   | | └─Token(Identifier) |params|
//@[008:0009) |   | ├─Token(Colon) |:|
//@[010:0016) |   | └─ObjectSyntax
//@[010:0011) |   |   ├─Token(LeftBrace) |{|
//@[011:0013) |   |   ├─Token(NewLine) |\r\n|
  }
//@[002:0003) |   |   └─Token(RightBrace) |}|
//@[003:0005) |   ├─Token(NewLine) |\r\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0005) ├─Token(NewLine) |\r\n\r\n|

module optionalWithAllParams './child/optionalParams.bicep'= {
//@[000:0210) ├─ModuleDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |module|
//@[007:0028) | ├─IdentifierSyntax
//@[007:0028) | | └─Token(Identifier) |optionalWithAllParams|
//@[029:0059) | ├─StringSyntax
//@[029:0059) | | └─Token(StringComplete) |'./child/optionalParams.bicep'|
//@[059:0060) | ├─Token(Assignment) |=|
//@[061:0210) | └─ObjectSyntax
//@[061:0062) |   ├─Token(LeftBrace) |{|
//@[062:0064) |   ├─Token(NewLine) |\r\n|
  name: 'optionalWithNoParams3'
//@[002:0031) |   ├─ObjectPropertySyntax
//@[002:0006) |   | ├─IdentifierSyntax
//@[002:0006) |   | | └─Token(Identifier) |name|
//@[006:0007) |   | ├─Token(Colon) |:|
//@[008:0031) |   | └─StringSyntax
//@[008:0031) |   |   └─Token(StringComplete) |'optionalWithNoParams3'|
//@[031:0033) |   ├─Token(NewLine) |\r\n|
  params: {
//@[002:0110) |   ├─ObjectPropertySyntax
//@[002:0008) |   | ├─IdentifierSyntax
//@[002:0008) |   | | └─Token(Identifier) |params|
//@[008:0009) |   | ├─Token(Colon) |:|
//@[010:0110) |   | └─ObjectSyntax
//@[010:0011) |   |   ├─Token(LeftBrace) |{|
//@[011:0013) |   |   ├─Token(NewLine) |\r\n|
    optionalString: 'abc'
//@[004:0025) |   |   ├─ObjectPropertySyntax
//@[004:0018) |   |   | ├─IdentifierSyntax
//@[004:0018) |   |   | | └─Token(Identifier) |optionalString|
//@[018:0019) |   |   | ├─Token(Colon) |:|
//@[020:0025) |   |   | └─StringSyntax
//@[020:0025) |   |   |   └─Token(StringComplete) |'abc'|
//@[025:0027) |   |   ├─Token(NewLine) |\r\n|
    optionalInt: 42
//@[004:0019) |   |   ├─ObjectPropertySyntax
//@[004:0015) |   |   | ├─IdentifierSyntax
//@[004:0015) |   |   | | └─Token(Identifier) |optionalInt|
//@[015:0016) |   |   | ├─Token(Colon) |:|
//@[017:0019) |   |   | └─IntegerLiteralSyntax
//@[017:0019) |   |   |   └─Token(Integer) |42|
//@[019:0021) |   |   ├─Token(NewLine) |\r\n|
    optionalObj: { }
//@[004:0020) |   |   ├─ObjectPropertySyntax
//@[004:0015) |   |   | ├─IdentifierSyntax
//@[004:0015) |   |   | | └─Token(Identifier) |optionalObj|
//@[015:0016) |   |   | ├─Token(Colon) |:|
//@[017:0020) |   |   | └─ObjectSyntax
//@[017:0018) |   |   |   ├─Token(LeftBrace) |{|
//@[019:0020) |   |   |   └─Token(RightBrace) |}|
//@[020:0022) |   |   ├─Token(NewLine) |\r\n|
    optionalArray: [ ]
//@[004:0022) |   |   ├─ObjectPropertySyntax
//@[004:0017) |   |   | ├─IdentifierSyntax
//@[004:0017) |   |   | | └─Token(Identifier) |optionalArray|
//@[017:0018) |   |   | ├─Token(Colon) |:|
//@[019:0022) |   |   | └─ArraySyntax
//@[019:0020) |   |   |   ├─Token(LeftSquare) |[|
//@[021:0022) |   |   |   └─Token(RightSquare) |]|
//@[022:0024) |   |   ├─Token(NewLine) |\r\n|
  }
//@[002:0003) |   |   └─Token(RightBrace) |}|
//@[003:0005) |   ├─Token(NewLine) |\r\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0005) ├─Token(NewLine) |\r\n\r\n|

resource resWithDependencies 'Mock.Rp/mockResource@2020-01-01' = {
//@[000:0233) ├─ResourceDeclarationSyntax
//@[000:0008) | ├─Token(Identifier) |resource|
//@[009:0028) | ├─IdentifierSyntax
//@[009:0028) | | └─Token(Identifier) |resWithDependencies|
//@[029:0062) | ├─StringSyntax
//@[029:0062) | | └─Token(StringComplete) |'Mock.Rp/mockResource@2020-01-01'|
//@[063:0064) | ├─Token(Assignment) |=|
//@[065:0233) | └─ObjectSyntax
//@[065:0066) |   ├─Token(LeftBrace) |{|
//@[066:0068) |   ├─Token(NewLine) |\r\n|
  name: 'harry'
//@[002:0015) |   ├─ObjectPropertySyntax
//@[002:0006) |   | ├─IdentifierSyntax
//@[002:0006) |   | | └─Token(Identifier) |name|
//@[006:0007) |   | ├─Token(Colon) |:|
//@[008:0015) |   | └─StringSyntax
//@[008:0015) |   |   └─Token(StringComplete) |'harry'|
//@[015:0017) |   ├─Token(NewLine) |\r\n|
  properties: {
//@[002:0145) |   ├─ObjectPropertySyntax
//@[002:0012) |   | ├─IdentifierSyntax
//@[002:0012) |   | | └─Token(Identifier) |properties|
//@[012:0013) |   | ├─Token(Colon) |:|
//@[014:0145) |   | └─ObjectSyntax
//@[014:0015) |   |   ├─Token(LeftBrace) |{|
//@[015:0017) |   |   ├─Token(NewLine) |\r\n|
    modADep: modATest.outputs.stringOutputA
//@[004:0043) |   |   ├─ObjectPropertySyntax
//@[004:0011) |   |   | ├─IdentifierSyntax
//@[004:0011) |   |   | | └─Token(Identifier) |modADep|
//@[011:0012) |   |   | ├─Token(Colon) |:|
//@[013:0043) |   |   | └─PropertyAccessSyntax
//@[013:0029) |   |   |   ├─PropertyAccessSyntax
//@[013:0021) |   |   |   | ├─VariableAccessSyntax
//@[013:0021) |   |   |   | | └─IdentifierSyntax
//@[013:0021) |   |   |   | |   └─Token(Identifier) |modATest|
//@[021:0022) |   |   |   | ├─Token(Dot) |.|
//@[022:0029) |   |   |   | └─IdentifierSyntax
//@[022:0029) |   |   |   |   └─Token(Identifier) |outputs|
//@[029:0030) |   |   |   ├─Token(Dot) |.|
//@[030:0043) |   |   |   └─IdentifierSyntax
//@[030:0043) |   |   |     └─Token(Identifier) |stringOutputA|
//@[043:0045) |   |   ├─Token(NewLine) |\r\n|
    modBDep: modB.outputs.myResourceId
//@[004:0038) |   |   ├─ObjectPropertySyntax
//@[004:0011) |   |   | ├─IdentifierSyntax
//@[004:0011) |   |   | | └─Token(Identifier) |modBDep|
//@[011:0012) |   |   | ├─Token(Colon) |:|
//@[013:0038) |   |   | └─PropertyAccessSyntax
//@[013:0025) |   |   |   ├─PropertyAccessSyntax
//@[013:0017) |   |   |   | ├─VariableAccessSyntax
//@[013:0017) |   |   |   | | └─IdentifierSyntax
//@[013:0017) |   |   |   | |   └─Token(Identifier) |modB|
//@[017:0018) |   |   |   | ├─Token(Dot) |.|
//@[018:0025) |   |   |   | └─IdentifierSyntax
//@[018:0025) |   |   |   |   └─Token(Identifier) |outputs|
//@[025:0026) |   |   |   ├─Token(Dot) |.|
//@[026:0038) |   |   |   └─IdentifierSyntax
//@[026:0038) |   |   |     └─Token(Identifier) |myResourceId|
//@[038:0040) |   |   ├─Token(NewLine) |\r\n|
    modCDep: modC.outputs.myResourceId
//@[004:0038) |   |   ├─ObjectPropertySyntax
//@[004:0011) |   |   | ├─IdentifierSyntax
//@[004:0011) |   |   | | └─Token(Identifier) |modCDep|
//@[011:0012) |   |   | ├─Token(Colon) |:|
//@[013:0038) |   |   | └─PropertyAccessSyntax
//@[013:0025) |   |   |   ├─PropertyAccessSyntax
//@[013:0017) |   |   |   | ├─VariableAccessSyntax
//@[013:0017) |   |   |   | | └─IdentifierSyntax
//@[013:0017) |   |   |   | |   └─Token(Identifier) |modC|
//@[017:0018) |   |   |   | ├─Token(Dot) |.|
//@[018:0025) |   |   |   | └─IdentifierSyntax
//@[018:0025) |   |   |   |   └─Token(Identifier) |outputs|
//@[025:0026) |   |   |   ├─Token(Dot) |.|
//@[026:0038) |   |   |   └─IdentifierSyntax
//@[026:0038) |   |   |     └─Token(Identifier) |myResourceId|
//@[038:0040) |   |   ├─Token(NewLine) |\r\n|
  }
//@[002:0003) |   |   └─Token(RightBrace) |}|
//@[003:0005) |   ├─Token(NewLine) |\r\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0005) ├─Token(NewLine) |\r\n\r\n|

module optionalWithAllParamsAndManualDependency './child/optionalParams.bicep'= {
//@[000:0321) ├─ModuleDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |module|
//@[007:0047) | ├─IdentifierSyntax
//@[007:0047) | | └─Token(Identifier) |optionalWithAllParamsAndManualDependency|
//@[048:0078) | ├─StringSyntax
//@[048:0078) | | └─Token(StringComplete) |'./child/optionalParams.bicep'|
//@[078:0079) | ├─Token(Assignment) |=|
//@[080:0321) | └─ObjectSyntax
//@[080:0081) |   ├─Token(LeftBrace) |{|
//@[081:0083) |   ├─Token(NewLine) |\r\n|
  name: 'optionalWithAllParamsAndManualDependency'
//@[002:0050) |   ├─ObjectPropertySyntax
//@[002:0006) |   | ├─IdentifierSyntax
//@[002:0006) |   | | └─Token(Identifier) |name|
//@[006:0007) |   | ├─Token(Colon) |:|
//@[008:0050) |   | └─StringSyntax
//@[008:0050) |   |   └─Token(StringComplete) |'optionalWithAllParamsAndManualDependency'|
//@[050:0052) |   ├─Token(NewLine) |\r\n|
  params: {
//@[002:0110) |   ├─ObjectPropertySyntax
//@[002:0008) |   | ├─IdentifierSyntax
//@[002:0008) |   | | └─Token(Identifier) |params|
//@[008:0009) |   | ├─Token(Colon) |:|
//@[010:0110) |   | └─ObjectSyntax
//@[010:0011) |   |   ├─Token(LeftBrace) |{|
//@[011:0013) |   |   ├─Token(NewLine) |\r\n|
    optionalString: 'abc'
//@[004:0025) |   |   ├─ObjectPropertySyntax
//@[004:0018) |   |   | ├─IdentifierSyntax
//@[004:0018) |   |   | | └─Token(Identifier) |optionalString|
//@[018:0019) |   |   | ├─Token(Colon) |:|
//@[020:0025) |   |   | └─StringSyntax
//@[020:0025) |   |   |   └─Token(StringComplete) |'abc'|
//@[025:0027) |   |   ├─Token(NewLine) |\r\n|
    optionalInt: 42
//@[004:0019) |   |   ├─ObjectPropertySyntax
//@[004:0015) |   |   | ├─IdentifierSyntax
//@[004:0015) |   |   | | └─Token(Identifier) |optionalInt|
//@[015:0016) |   |   | ├─Token(Colon) |:|
//@[017:0019) |   |   | └─IntegerLiteralSyntax
//@[017:0019) |   |   |   └─Token(Integer) |42|
//@[019:0021) |   |   ├─Token(NewLine) |\r\n|
    optionalObj: { }
//@[004:0020) |   |   ├─ObjectPropertySyntax
//@[004:0015) |   |   | ├─IdentifierSyntax
//@[004:0015) |   |   | | └─Token(Identifier) |optionalObj|
//@[015:0016) |   |   | ├─Token(Colon) |:|
//@[017:0020) |   |   | └─ObjectSyntax
//@[017:0018) |   |   |   ├─Token(LeftBrace) |{|
//@[019:0020) |   |   |   └─Token(RightBrace) |}|
//@[020:0022) |   |   ├─Token(NewLine) |\r\n|
    optionalArray: [ ]
//@[004:0022) |   |   ├─ObjectPropertySyntax
//@[004:0017) |   |   | ├─IdentifierSyntax
//@[004:0017) |   |   | | └─Token(Identifier) |optionalArray|
//@[017:0018) |   |   | ├─Token(Colon) |:|
//@[019:0022) |   |   | └─ArraySyntax
//@[019:0020) |   |   |   ├─Token(LeftSquare) |[|
//@[021:0022) |   |   |   └─Token(RightSquare) |]|
//@[022:0024) |   |   ├─Token(NewLine) |\r\n|
  }
//@[002:0003) |   |   └─Token(RightBrace) |}|
//@[003:0005) |   ├─Token(NewLine) |\r\n|
  dependsOn: [
//@[002:0071) |   ├─ObjectPropertySyntax
//@[002:0011) |   | ├─IdentifierSyntax
//@[002:0011) |   | | └─Token(Identifier) |dependsOn|
//@[011:0012) |   | ├─Token(Colon) |:|
//@[013:0071) |   | └─ArraySyntax
//@[013:0014) |   |   ├─Token(LeftSquare) |[|
//@[014:0016) |   |   ├─Token(NewLine) |\r\n|
    resWithDependencies
//@[004:0023) |   |   ├─ArrayItemSyntax
//@[004:0023) |   |   | └─VariableAccessSyntax
//@[004:0023) |   |   |   └─IdentifierSyntax
//@[004:0023) |   |   |     └─Token(Identifier) |resWithDependencies|
//@[023:0025) |   |   ├─Token(NewLine) |\r\n|
    optionalWithAllParams
//@[004:0025) |   |   ├─ArrayItemSyntax
//@[004:0025) |   |   | └─VariableAccessSyntax
//@[004:0025) |   |   |   └─IdentifierSyntax
//@[004:0025) |   |   |     └─Token(Identifier) |optionalWithAllParams|
//@[025:0027) |   |   ├─Token(NewLine) |\r\n|
  ]
//@[002:0003) |   |   └─Token(RightSquare) |]|
//@[003:0005) |   ├─Token(NewLine) |\r\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0005) ├─Token(NewLine) |\r\n\r\n|

module optionalWithImplicitDependency './child/optionalParams.bicep'= {
//@[000:0300) ├─ModuleDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |module|
//@[007:0037) | ├─IdentifierSyntax
//@[007:0037) | | └─Token(Identifier) |optionalWithImplicitDependency|
//@[038:0068) | ├─StringSyntax
//@[038:0068) | | └─Token(StringComplete) |'./child/optionalParams.bicep'|
//@[068:0069) | ├─Token(Assignment) |=|
//@[070:0300) | └─ObjectSyntax
//@[070:0071) |   ├─Token(LeftBrace) |{|
//@[071:0073) |   ├─Token(NewLine) |\r\n|
  name: 'optionalWithImplicitDependency'
//@[002:0040) |   ├─ObjectPropertySyntax
//@[002:0006) |   | ├─IdentifierSyntax
//@[002:0006) |   | | └─Token(Identifier) |name|
//@[006:0007) |   | ├─Token(Colon) |:|
//@[008:0040) |   | └─StringSyntax
//@[008:0040) |   |   └─Token(StringComplete) |'optionalWithImplicitDependency'|
//@[040:0042) |   ├─Token(NewLine) |\r\n|
  params: {
//@[002:0182) |   ├─ObjectPropertySyntax
//@[002:0008) |   | ├─IdentifierSyntax
//@[002:0008) |   | | └─Token(Identifier) |params|
//@[008:0009) |   | ├─Token(Colon) |:|
//@[010:0182) |   | └─ObjectSyntax
//@[010:0011) |   |   ├─Token(LeftBrace) |{|
//@[011:0013) |   |   ├─Token(NewLine) |\r\n|
    optionalString: concat(resWithDependencies.id, optionalWithAllParamsAndManualDependency.name)
//@[004:0097) |   |   ├─ObjectPropertySyntax
//@[004:0018) |   |   | ├─IdentifierSyntax
//@[004:0018) |   |   | | └─Token(Identifier) |optionalString|
//@[018:0019) |   |   | ├─Token(Colon) |:|
//@[020:0097) |   |   | └─FunctionCallSyntax
//@[020:0026) |   |   |   ├─IdentifierSyntax
//@[020:0026) |   |   |   | └─Token(Identifier) |concat|
//@[026:0027) |   |   |   ├─Token(LeftParen) |(|
//@[027:0049) |   |   |   ├─FunctionArgumentSyntax
//@[027:0049) |   |   |   | └─PropertyAccessSyntax
//@[027:0046) |   |   |   |   ├─VariableAccessSyntax
//@[027:0046) |   |   |   |   | └─IdentifierSyntax
//@[027:0046) |   |   |   |   |   └─Token(Identifier) |resWithDependencies|
//@[046:0047) |   |   |   |   ├─Token(Dot) |.|
//@[047:0049) |   |   |   |   └─IdentifierSyntax
//@[047:0049) |   |   |   |     └─Token(Identifier) |id|
//@[049:0050) |   |   |   ├─Token(Comma) |,|
//@[051:0096) |   |   |   ├─FunctionArgumentSyntax
//@[051:0096) |   |   |   | └─PropertyAccessSyntax
//@[051:0091) |   |   |   |   ├─VariableAccessSyntax
//@[051:0091) |   |   |   |   | └─IdentifierSyntax
//@[051:0091) |   |   |   |   |   └─Token(Identifier) |optionalWithAllParamsAndManualDependency|
//@[091:0092) |   |   |   |   ├─Token(Dot) |.|
//@[092:0096) |   |   |   |   └─IdentifierSyntax
//@[092:0096) |   |   |   |     └─Token(Identifier) |name|
//@[096:0097) |   |   |   └─Token(RightParen) |)|
//@[097:0099) |   |   ├─Token(NewLine) |\r\n|
    optionalInt: 42
//@[004:0019) |   |   ├─ObjectPropertySyntax
//@[004:0015) |   |   | ├─IdentifierSyntax
//@[004:0015) |   |   | | └─Token(Identifier) |optionalInt|
//@[015:0016) |   |   | ├─Token(Colon) |:|
//@[017:0019) |   |   | └─IntegerLiteralSyntax
//@[017:0019) |   |   |   └─Token(Integer) |42|
//@[019:0021) |   |   ├─Token(NewLine) |\r\n|
    optionalObj: { }
//@[004:0020) |   |   ├─ObjectPropertySyntax
//@[004:0015) |   |   | ├─IdentifierSyntax
//@[004:0015) |   |   | | └─Token(Identifier) |optionalObj|
//@[015:0016) |   |   | ├─Token(Colon) |:|
//@[017:0020) |   |   | └─ObjectSyntax
//@[017:0018) |   |   |   ├─Token(LeftBrace) |{|
//@[019:0020) |   |   |   └─Token(RightBrace) |}|
//@[020:0022) |   |   ├─Token(NewLine) |\r\n|
    optionalArray: [ ]
//@[004:0022) |   |   ├─ObjectPropertySyntax
//@[004:0017) |   |   | ├─IdentifierSyntax
//@[004:0017) |   |   | | └─Token(Identifier) |optionalArray|
//@[017:0018) |   |   | ├─Token(Colon) |:|
//@[019:0022) |   |   | └─ArraySyntax
//@[019:0020) |   |   |   ├─Token(LeftSquare) |[|
//@[021:0022) |   |   |   └─Token(RightSquare) |]|
//@[022:0024) |   |   ├─Token(NewLine) |\r\n|
  }
//@[002:0003) |   |   └─Token(RightBrace) |}|
//@[003:0005) |   ├─Token(NewLine) |\r\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0005) ├─Token(NewLine) |\r\n\r\n|

module moduleWithCalculatedName './child/optionalParams.bicep'= {
//@[000:0331) ├─ModuleDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |module|
//@[007:0031) | ├─IdentifierSyntax
//@[007:0031) | | └─Token(Identifier) |moduleWithCalculatedName|
//@[032:0062) | ├─StringSyntax
//@[032:0062) | | └─Token(StringComplete) |'./child/optionalParams.bicep'|
//@[062:0063) | ├─Token(Assignment) |=|
//@[064:0331) | └─ObjectSyntax
//@[064:0065) |   ├─Token(LeftBrace) |{|
//@[065:0067) |   ├─Token(NewLine) |\r\n|
  name: '${optionalWithAllParamsAndManualDependency.name}${deployTimeSuffix}'
//@[002:0077) |   ├─ObjectPropertySyntax
//@[002:0006) |   | ├─IdentifierSyntax
//@[002:0006) |   | | └─Token(Identifier) |name|
//@[006:0007) |   | ├─Token(Colon) |:|
//@[008:0077) |   | └─StringSyntax
//@[008:0011) |   |   ├─Token(StringLeftPiece) |'${|
//@[011:0056) |   |   ├─PropertyAccessSyntax
//@[011:0051) |   |   | ├─VariableAccessSyntax
//@[011:0051) |   |   | | └─IdentifierSyntax
//@[011:0051) |   |   | |   └─Token(Identifier) |optionalWithAllParamsAndManualDependency|
//@[051:0052) |   |   | ├─Token(Dot) |.|
//@[052:0056) |   |   | └─IdentifierSyntax
//@[052:0056) |   |   |   └─Token(Identifier) |name|
//@[056:0059) |   |   ├─Token(StringMiddlePiece) |}${|
//@[059:0075) |   |   ├─VariableAccessSyntax
//@[059:0075) |   |   | └─IdentifierSyntax
//@[059:0075) |   |   |   └─Token(Identifier) |deployTimeSuffix|
//@[075:0077) |   |   └─Token(StringRightPiece) |}'|
//@[077:0079) |   ├─Token(NewLine) |\r\n|
  params: {
//@[002:0182) |   ├─ObjectPropertySyntax
//@[002:0008) |   | ├─IdentifierSyntax
//@[002:0008) |   | | └─Token(Identifier) |params|
//@[008:0009) |   | ├─Token(Colon) |:|
//@[010:0182) |   | └─ObjectSyntax
//@[010:0011) |   |   ├─Token(LeftBrace) |{|
//@[011:0013) |   |   ├─Token(NewLine) |\r\n|
    optionalString: concat(resWithDependencies.id, optionalWithAllParamsAndManualDependency.name)
//@[004:0097) |   |   ├─ObjectPropertySyntax
//@[004:0018) |   |   | ├─IdentifierSyntax
//@[004:0018) |   |   | | └─Token(Identifier) |optionalString|
//@[018:0019) |   |   | ├─Token(Colon) |:|
//@[020:0097) |   |   | └─FunctionCallSyntax
//@[020:0026) |   |   |   ├─IdentifierSyntax
//@[020:0026) |   |   |   | └─Token(Identifier) |concat|
//@[026:0027) |   |   |   ├─Token(LeftParen) |(|
//@[027:0049) |   |   |   ├─FunctionArgumentSyntax
//@[027:0049) |   |   |   | └─PropertyAccessSyntax
//@[027:0046) |   |   |   |   ├─VariableAccessSyntax
//@[027:0046) |   |   |   |   | └─IdentifierSyntax
//@[027:0046) |   |   |   |   |   └─Token(Identifier) |resWithDependencies|
//@[046:0047) |   |   |   |   ├─Token(Dot) |.|
//@[047:0049) |   |   |   |   └─IdentifierSyntax
//@[047:0049) |   |   |   |     └─Token(Identifier) |id|
//@[049:0050) |   |   |   ├─Token(Comma) |,|
//@[051:0096) |   |   |   ├─FunctionArgumentSyntax
//@[051:0096) |   |   |   | └─PropertyAccessSyntax
//@[051:0091) |   |   |   |   ├─VariableAccessSyntax
//@[051:0091) |   |   |   |   | └─IdentifierSyntax
//@[051:0091) |   |   |   |   |   └─Token(Identifier) |optionalWithAllParamsAndManualDependency|
//@[091:0092) |   |   |   |   ├─Token(Dot) |.|
//@[092:0096) |   |   |   |   └─IdentifierSyntax
//@[092:0096) |   |   |   |     └─Token(Identifier) |name|
//@[096:0097) |   |   |   └─Token(RightParen) |)|
//@[097:0099) |   |   ├─Token(NewLine) |\r\n|
    optionalInt: 42
//@[004:0019) |   |   ├─ObjectPropertySyntax
//@[004:0015) |   |   | ├─IdentifierSyntax
//@[004:0015) |   |   | | └─Token(Identifier) |optionalInt|
//@[015:0016) |   |   | ├─Token(Colon) |:|
//@[017:0019) |   |   | └─IntegerLiteralSyntax
//@[017:0019) |   |   |   └─Token(Integer) |42|
//@[019:0021) |   |   ├─Token(NewLine) |\r\n|
    optionalObj: { }
//@[004:0020) |   |   ├─ObjectPropertySyntax
//@[004:0015) |   |   | ├─IdentifierSyntax
//@[004:0015) |   |   | | └─Token(Identifier) |optionalObj|
//@[015:0016) |   |   | ├─Token(Colon) |:|
//@[017:0020) |   |   | └─ObjectSyntax
//@[017:0018) |   |   |   ├─Token(LeftBrace) |{|
//@[019:0020) |   |   |   └─Token(RightBrace) |}|
//@[020:0022) |   |   ├─Token(NewLine) |\r\n|
    optionalArray: [ ]
//@[004:0022) |   |   ├─ObjectPropertySyntax
//@[004:0017) |   |   | ├─IdentifierSyntax
//@[004:0017) |   |   | | └─Token(Identifier) |optionalArray|
//@[017:0018) |   |   | ├─Token(Colon) |:|
//@[019:0022) |   |   | └─ArraySyntax
//@[019:0020) |   |   |   ├─Token(LeftSquare) |[|
//@[021:0022) |   |   |   └─Token(RightSquare) |]|
//@[022:0024) |   |   ├─Token(NewLine) |\r\n|
  }
//@[002:0003) |   |   └─Token(RightBrace) |}|
//@[003:0005) |   ├─Token(NewLine) |\r\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0005) ├─Token(NewLine) |\r\n\r\n|

resource resWithCalculatedNameDependencies 'Mock.Rp/mockResource@2020-01-01' = {
//@[000:0241) ├─ResourceDeclarationSyntax
//@[000:0008) | ├─Token(Identifier) |resource|
//@[009:0042) | ├─IdentifierSyntax
//@[009:0042) | | └─Token(Identifier) |resWithCalculatedNameDependencies|
//@[043:0076) | ├─StringSyntax
//@[043:0076) | | └─Token(StringComplete) |'Mock.Rp/mockResource@2020-01-01'|
//@[077:0078) | ├─Token(Assignment) |=|
//@[079:0241) | └─ObjectSyntax
//@[079:0080) |   ├─Token(LeftBrace) |{|
//@[080:0082) |   ├─Token(NewLine) |\r\n|
  name: '${optionalWithAllParamsAndManualDependency.name}${deployTimeSuffix}'
//@[002:0077) |   ├─ObjectPropertySyntax
//@[002:0006) |   | ├─IdentifierSyntax
//@[002:0006) |   | | └─Token(Identifier) |name|
//@[006:0007) |   | ├─Token(Colon) |:|
//@[008:0077) |   | └─StringSyntax
//@[008:0011) |   |   ├─Token(StringLeftPiece) |'${|
//@[011:0056) |   |   ├─PropertyAccessSyntax
//@[011:0051) |   |   | ├─VariableAccessSyntax
//@[011:0051) |   |   | | └─IdentifierSyntax
//@[011:0051) |   |   | |   └─Token(Identifier) |optionalWithAllParamsAndManualDependency|
//@[051:0052) |   |   | ├─Token(Dot) |.|
//@[052:0056) |   |   | └─IdentifierSyntax
//@[052:0056) |   |   |   └─Token(Identifier) |name|
//@[056:0059) |   |   ├─Token(StringMiddlePiece) |}${|
//@[059:0075) |   |   ├─VariableAccessSyntax
//@[059:0075) |   |   | └─IdentifierSyntax
//@[059:0075) |   |   |   └─Token(Identifier) |deployTimeSuffix|
//@[075:0077) |   |   └─Token(StringRightPiece) |}'|
//@[077:0079) |   ├─Token(NewLine) |\r\n|
  properties: {
//@[002:0077) |   ├─ObjectPropertySyntax
//@[002:0012) |   | ├─IdentifierSyntax
//@[002:0012) |   | | └─Token(Identifier) |properties|
//@[012:0013) |   | ├─Token(Colon) |:|
//@[014:0077) |   | └─ObjectSyntax
//@[014:0015) |   |   ├─Token(LeftBrace) |{|
//@[015:0017) |   |   ├─Token(NewLine) |\r\n|
    modADep: moduleWithCalculatedName.outputs.outputObj
//@[004:0055) |   |   ├─ObjectPropertySyntax
//@[004:0011) |   |   | ├─IdentifierSyntax
//@[004:0011) |   |   | | └─Token(Identifier) |modADep|
//@[011:0012) |   |   | ├─Token(Colon) |:|
//@[013:0055) |   |   | └─PropertyAccessSyntax
//@[013:0045) |   |   |   ├─PropertyAccessSyntax
//@[013:0037) |   |   |   | ├─VariableAccessSyntax
//@[013:0037) |   |   |   | | └─IdentifierSyntax
//@[013:0037) |   |   |   | |   └─Token(Identifier) |moduleWithCalculatedName|
//@[037:0038) |   |   |   | ├─Token(Dot) |.|
//@[038:0045) |   |   |   | └─IdentifierSyntax
//@[038:0045) |   |   |   |   └─Token(Identifier) |outputs|
//@[045:0046) |   |   |   ├─Token(Dot) |.|
//@[046:0055) |   |   |   └─IdentifierSyntax
//@[046:0055) |   |   |     └─Token(Identifier) |outputObj|
//@[055:0057) |   |   ├─Token(NewLine) |\r\n|
  }
//@[002:0003) |   |   └─Token(RightBrace) |}|
//@[003:0005) |   ├─Token(NewLine) |\r\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0005) ├─Token(NewLine) |\r\n\r\n|

output stringOutputA string = modATest.outputs.stringOutputA
//@[000:0060) ├─OutputDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |output|
//@[007:0020) | ├─IdentifierSyntax
//@[007:0020) | | └─Token(Identifier) |stringOutputA|
//@[021:0027) | ├─SimpleTypeSyntax
//@[021:0027) | | └─Token(Identifier) |string|
//@[028:0029) | ├─Token(Assignment) |=|
//@[030:0060) | └─PropertyAccessSyntax
//@[030:0046) |   ├─PropertyAccessSyntax
//@[030:0038) |   | ├─VariableAccessSyntax
//@[030:0038) |   | | └─IdentifierSyntax
//@[030:0038) |   | |   └─Token(Identifier) |modATest|
//@[038:0039) |   | ├─Token(Dot) |.|
//@[039:0046) |   | └─IdentifierSyntax
//@[039:0046) |   |   └─Token(Identifier) |outputs|
//@[046:0047) |   ├─Token(Dot) |.|
//@[047:0060) |   └─IdentifierSyntax
//@[047:0060) |     └─Token(Identifier) |stringOutputA|
//@[060:0062) ├─Token(NewLine) |\r\n|
output stringOutputB string = modATest.outputs.stringOutputB
//@[000:0060) ├─OutputDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |output|
//@[007:0020) | ├─IdentifierSyntax
//@[007:0020) | | └─Token(Identifier) |stringOutputB|
//@[021:0027) | ├─SimpleTypeSyntax
//@[021:0027) | | └─Token(Identifier) |string|
//@[028:0029) | ├─Token(Assignment) |=|
//@[030:0060) | └─PropertyAccessSyntax
//@[030:0046) |   ├─PropertyAccessSyntax
//@[030:0038) |   | ├─VariableAccessSyntax
//@[030:0038) |   | | └─IdentifierSyntax
//@[030:0038) |   | |   └─Token(Identifier) |modATest|
//@[038:0039) |   | ├─Token(Dot) |.|
//@[039:0046) |   | └─IdentifierSyntax
//@[039:0046) |   |   └─Token(Identifier) |outputs|
//@[046:0047) |   ├─Token(Dot) |.|
//@[047:0060) |   └─IdentifierSyntax
//@[047:0060) |     └─Token(Identifier) |stringOutputB|
//@[060:0062) ├─Token(NewLine) |\r\n|
output objOutput object = modATest.outputs.objOutput
//@[000:0052) ├─OutputDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |output|
//@[007:0016) | ├─IdentifierSyntax
//@[007:0016) | | └─Token(Identifier) |objOutput|
//@[017:0023) | ├─SimpleTypeSyntax
//@[017:0023) | | └─Token(Identifier) |object|
//@[024:0025) | ├─Token(Assignment) |=|
//@[026:0052) | └─PropertyAccessSyntax
//@[026:0042) |   ├─PropertyAccessSyntax
//@[026:0034) |   | ├─VariableAccessSyntax
//@[026:0034) |   | | └─IdentifierSyntax
//@[026:0034) |   | |   └─Token(Identifier) |modATest|
//@[034:0035) |   | ├─Token(Dot) |.|
//@[035:0042) |   | └─IdentifierSyntax
//@[035:0042) |   |   └─Token(Identifier) |outputs|
//@[042:0043) |   ├─Token(Dot) |.|
//@[043:0052) |   └─IdentifierSyntax
//@[043:0052) |     └─Token(Identifier) |objOutput|
//@[052:0054) ├─Token(NewLine) |\r\n|
output arrayOutput array = modATest.outputs.arrayOutput
//@[000:0055) ├─OutputDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |output|
//@[007:0018) | ├─IdentifierSyntax
//@[007:0018) | | └─Token(Identifier) |arrayOutput|
//@[019:0024) | ├─SimpleTypeSyntax
//@[019:0024) | | └─Token(Identifier) |array|
//@[025:0026) | ├─Token(Assignment) |=|
//@[027:0055) | └─PropertyAccessSyntax
//@[027:0043) |   ├─PropertyAccessSyntax
//@[027:0035) |   | ├─VariableAccessSyntax
//@[027:0035) |   | | └─IdentifierSyntax
//@[027:0035) |   | |   └─Token(Identifier) |modATest|
//@[035:0036) |   | ├─Token(Dot) |.|
//@[036:0043) |   | └─IdentifierSyntax
//@[036:0043) |   |   └─Token(Identifier) |outputs|
//@[043:0044) |   ├─Token(Dot) |.|
//@[044:0055) |   └─IdentifierSyntax
//@[044:0055) |     └─Token(Identifier) |arrayOutput|
//@[055:0057) ├─Token(NewLine) |\r\n|
output modCalculatedNameOutput object = moduleWithCalculatedName.outputs.outputObj
//@[000:0082) ├─OutputDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |output|
//@[007:0030) | ├─IdentifierSyntax
//@[007:0030) | | └─Token(Identifier) |modCalculatedNameOutput|
//@[031:0037) | ├─SimpleTypeSyntax
//@[031:0037) | | └─Token(Identifier) |object|
//@[038:0039) | ├─Token(Assignment) |=|
//@[040:0082) | └─PropertyAccessSyntax
//@[040:0072) |   ├─PropertyAccessSyntax
//@[040:0064) |   | ├─VariableAccessSyntax
//@[040:0064) |   | | └─IdentifierSyntax
//@[040:0064) |   | |   └─Token(Identifier) |moduleWithCalculatedName|
//@[064:0065) |   | ├─Token(Dot) |.|
//@[065:0072) |   | └─IdentifierSyntax
//@[065:0072) |   |   └─Token(Identifier) |outputs|
//@[072:0073) |   ├─Token(Dot) |.|
//@[073:0082) |   └─IdentifierSyntax
//@[073:0082) |     └─Token(Identifier) |outputObj|
//@[082:0086) ├─Token(NewLine) |\r\n\r\n|

/*
  valid loop cases
*/ 
//@[003:0007) ├─Token(NewLine) |\r\n\r\n|

@sys.description('this is myModules')
//@[000:0162) ├─VariableDeclarationSyntax
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
//@[017:0036) | |   |   └─Token(StringComplete) |'this is myModules'|
//@[036:0037) | |   └─Token(RightParen) |)|
//@[037:0039) | ├─Token(NewLine) |\r\n|
var myModules = [
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0013) | ├─IdentifierSyntax
//@[004:0013) | | └─Token(Identifier) |myModules|
//@[014:0015) | ├─Token(Assignment) |=|
//@[016:0123) | └─ArraySyntax
//@[016:0017) |   ├─Token(LeftSquare) |[|
//@[017:0019) |   ├─Token(NewLine) |\r\n|
  {
//@[002:0050) |   ├─ArrayItemSyntax
//@[002:0050) |   | └─ObjectSyntax
//@[002:0003) |   |   ├─Token(LeftBrace) |{|
//@[003:0005) |   |   ├─Token(NewLine) |\r\n|
    name: 'one'
//@[004:0015) |   |   ├─ObjectPropertySyntax
//@[004:0008) |   |   | ├─IdentifierSyntax
//@[004:0008) |   |   | | └─Token(Identifier) |name|
//@[008:0009) |   |   | ├─Token(Colon) |:|
//@[010:0015) |   |   | └─StringSyntax
//@[010:0015) |   |   |   └─Token(StringComplete) |'one'|
//@[015:0017) |   |   ├─Token(NewLine) |\r\n|
    location: 'eastus2'
//@[004:0023) |   |   ├─ObjectPropertySyntax
//@[004:0012) |   |   | ├─IdentifierSyntax
//@[004:0012) |   |   | | └─Token(Identifier) |location|
//@[012:0013) |   |   | ├─Token(Colon) |:|
//@[014:0023) |   |   | └─StringSyntax
//@[014:0023) |   |   |   └─Token(StringComplete) |'eastus2'|
//@[023:0025) |   |   ├─Token(NewLine) |\r\n|
  }
//@[002:0003) |   |   └─Token(RightBrace) |}|
//@[003:0005) |   ├─Token(NewLine) |\r\n|
  {
//@[002:0049) |   ├─ArrayItemSyntax
//@[002:0049) |   | └─ObjectSyntax
//@[002:0003) |   |   ├─Token(LeftBrace) |{|
//@[003:0005) |   |   ├─Token(NewLine) |\r\n|
    name: 'two'
//@[004:0015) |   |   ├─ObjectPropertySyntax
//@[004:0008) |   |   | ├─IdentifierSyntax
//@[004:0008) |   |   | | └─Token(Identifier) |name|
//@[008:0009) |   |   | ├─Token(Colon) |:|
//@[010:0015) |   |   | └─StringSyntax
//@[010:0015) |   |   |   └─Token(StringComplete) |'two'|
//@[015:0017) |   |   ├─Token(NewLine) |\r\n|
    location: 'westus'
//@[004:0022) |   |   ├─ObjectPropertySyntax
//@[004:0012) |   |   | ├─IdentifierSyntax
//@[004:0012) |   |   | | └─Token(Identifier) |location|
//@[012:0013) |   |   | ├─Token(Colon) |:|
//@[014:0022) |   |   | └─StringSyntax
//@[014:0022) |   |   |   └─Token(StringComplete) |'westus'|
//@[022:0024) |   |   ├─Token(NewLine) |\r\n|
  }
//@[002:0003) |   |   └─Token(RightBrace) |}|
//@[003:0005) |   ├─Token(NewLine) |\r\n|
]
//@[000:0001) |   └─Token(RightSquare) |]|
//@[001:0005) ├─Token(NewLine) |\r\n\r\n|

var emptyArray = []
//@[000:0019) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0014) | ├─IdentifierSyntax
//@[004:0014) | | └─Token(Identifier) |emptyArray|
//@[015:0016) | ├─Token(Assignment) |=|
//@[017:0019) | └─ArraySyntax
//@[017:0018) |   ├─Token(LeftSquare) |[|
//@[018:0019) |   └─Token(RightSquare) |]|
//@[019:0023) ├─Token(NewLine) |\r\n\r\n|

// simple module loop
//@[021:0023) ├─Token(NewLine) |\r\n|
module storageResources 'modulea.bicep' = [for module in myModules: {
//@[000:0189) ├─ModuleDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |module|
//@[007:0023) | ├─IdentifierSyntax
//@[007:0023) | | └─Token(Identifier) |storageResources|
//@[024:0039) | ├─StringSyntax
//@[024:0039) | | └─Token(StringComplete) |'modulea.bicep'|
//@[040:0041) | ├─Token(Assignment) |=|
//@[042:0189) | └─ForSyntax
//@[042:0043) |   ├─Token(LeftSquare) |[|
//@[043:0046) |   ├─Token(Identifier) |for|
//@[047:0053) |   ├─LocalVariableSyntax
//@[047:0053) |   | └─IdentifierSyntax
//@[047:0053) |   |   └─Token(Identifier) |module|
//@[054:0056) |   ├─Token(Identifier) |in|
//@[057:0066) |   ├─VariableAccessSyntax
//@[057:0066) |   | └─IdentifierSyntax
//@[057:0066) |   |   └─Token(Identifier) |myModules|
//@[066:0067) |   ├─Token(Colon) |:|
//@[068:0188) |   ├─ObjectSyntax
//@[068:0069) |   | ├─Token(LeftBrace) |{|
//@[069:0071) |   | ├─Token(NewLine) |\r\n|
  name: module.name
//@[002:0019) |   | ├─ObjectPropertySyntax
//@[002:0006) |   | | ├─IdentifierSyntax
//@[002:0006) |   | | | └─Token(Identifier) |name|
//@[006:0007) |   | | ├─Token(Colon) |:|
//@[008:0019) |   | | └─PropertyAccessSyntax
//@[008:0014) |   | |   ├─VariableAccessSyntax
//@[008:0014) |   | |   | └─IdentifierSyntax
//@[008:0014) |   | |   |   └─Token(Identifier) |module|
//@[014:0015) |   | |   ├─Token(Dot) |.|
//@[015:0019) |   | |   └─IdentifierSyntax
//@[015:0019) |   | |     └─Token(Identifier) |name|
//@[019:0021) |   | ├─Token(NewLine) |\r\n|
  params: {
//@[002:0093) |   | ├─ObjectPropertySyntax
//@[002:0008) |   | | ├─IdentifierSyntax
//@[002:0008) |   | | | └─Token(Identifier) |params|
//@[008:0009) |   | | ├─Token(Colon) |:|
//@[010:0093) |   | | └─ObjectSyntax
//@[010:0011) |   | |   ├─Token(LeftBrace) |{|
//@[011:0013) |   | |   ├─Token(NewLine) |\r\n|
    arrayParam: []
//@[004:0018) |   | |   ├─ObjectPropertySyntax
//@[004:0014) |   | |   | ├─IdentifierSyntax
//@[004:0014) |   | |   | | └─Token(Identifier) |arrayParam|
//@[014:0015) |   | |   | ├─Token(Colon) |:|
//@[016:0018) |   | |   | └─ArraySyntax
//@[016:0017) |   | |   |   ├─Token(LeftSquare) |[|
//@[017:0018) |   | |   |   └─Token(RightSquare) |]|
//@[018:0020) |   | |   ├─Token(NewLine) |\r\n|
    objParam: module
//@[004:0020) |   | |   ├─ObjectPropertySyntax
//@[004:0012) |   | |   | ├─IdentifierSyntax
//@[004:0012) |   | |   | | └─Token(Identifier) |objParam|
//@[012:0013) |   | |   | ├─Token(Colon) |:|
//@[014:0020) |   | |   | └─VariableAccessSyntax
//@[014:0020) |   | |   |   └─IdentifierSyntax
//@[014:0020) |   | |   |     └─Token(Identifier) |module|
//@[020:0022) |   | |   ├─Token(NewLine) |\r\n|
    stringParamB: module.location
//@[004:0033) |   | |   ├─ObjectPropertySyntax
//@[004:0016) |   | |   | ├─IdentifierSyntax
//@[004:0016) |   | |   | | └─Token(Identifier) |stringParamB|
//@[016:0017) |   | |   | ├─Token(Colon) |:|
//@[018:0033) |   | |   | └─PropertyAccessSyntax
//@[018:0024) |   | |   |   ├─VariableAccessSyntax
//@[018:0024) |   | |   |   | └─IdentifierSyntax
//@[018:0024) |   | |   |   |   └─Token(Identifier) |module|
//@[024:0025) |   | |   |   ├─Token(Dot) |.|
//@[025:0033) |   | |   |   └─IdentifierSyntax
//@[025:0033) |   | |   |     └─Token(Identifier) |location|
//@[033:0035) |   | |   ├─Token(NewLine) |\r\n|
  }
//@[002:0003) |   | |   └─Token(RightBrace) |}|
//@[003:0005) |   | ├─Token(NewLine) |\r\n|
}]
//@[000:0001) |   | └─Token(RightBrace) |}|
//@[001:0002) |   └─Token(RightSquare) |]|
//@[002:0006) ├─Token(NewLine) |\r\n\r\n|

// simple indexed module loop
//@[029:0031) ├─Token(NewLine) |\r\n|
module storageResourcesWithIndex 'modulea.bicep' = [for (module, i) in myModules: {
//@[000:0256) ├─ModuleDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |module|
//@[007:0032) | ├─IdentifierSyntax
//@[007:0032) | | └─Token(Identifier) |storageResourcesWithIndex|
//@[033:0048) | ├─StringSyntax
//@[033:0048) | | └─Token(StringComplete) |'modulea.bicep'|
//@[049:0050) | ├─Token(Assignment) |=|
//@[051:0256) | └─ForSyntax
//@[051:0052) |   ├─Token(LeftSquare) |[|
//@[052:0055) |   ├─Token(Identifier) |for|
//@[056:0067) |   ├─VariableBlockSyntax
//@[056:0057) |   | ├─Token(LeftParen) |(|
//@[057:0063) |   | ├─LocalVariableSyntax
//@[057:0063) |   | | └─IdentifierSyntax
//@[057:0063) |   | |   └─Token(Identifier) |module|
//@[063:0064) |   | ├─Token(Comma) |,|
//@[065:0066) |   | ├─LocalVariableSyntax
//@[065:0066) |   | | └─IdentifierSyntax
//@[065:0066) |   | |   └─Token(Identifier) |i|
//@[066:0067) |   | └─Token(RightParen) |)|
//@[068:0070) |   ├─Token(Identifier) |in|
//@[071:0080) |   ├─VariableAccessSyntax
//@[071:0080) |   | └─IdentifierSyntax
//@[071:0080) |   |   └─Token(Identifier) |myModules|
//@[080:0081) |   ├─Token(Colon) |:|
//@[082:0255) |   ├─ObjectSyntax
//@[082:0083) |   | ├─Token(LeftBrace) |{|
//@[083:0085) |   | ├─Token(NewLine) |\r\n|
  name: module.name
//@[002:0019) |   | ├─ObjectPropertySyntax
//@[002:0006) |   | | ├─IdentifierSyntax
//@[002:0006) |   | | | └─Token(Identifier) |name|
//@[006:0007) |   | | ├─Token(Colon) |:|
//@[008:0019) |   | | └─PropertyAccessSyntax
//@[008:0014) |   | |   ├─VariableAccessSyntax
//@[008:0014) |   | |   | └─IdentifierSyntax
//@[008:0014) |   | |   |   └─Token(Identifier) |module|
//@[014:0015) |   | |   ├─Token(Dot) |.|
//@[015:0019) |   | |   └─IdentifierSyntax
//@[015:0019) |   | |     └─Token(Identifier) |name|
//@[019:0021) |   | ├─Token(NewLine) |\r\n|
  params: {
//@[002:0146) |   | ├─ObjectPropertySyntax
//@[002:0008) |   | | ├─IdentifierSyntax
//@[002:0008) |   | | | └─Token(Identifier) |params|
//@[008:0009) |   | | ├─Token(Colon) |:|
//@[010:0146) |   | | └─ObjectSyntax
//@[010:0011) |   | |   ├─Token(LeftBrace) |{|
//@[011:0013) |   | |   ├─Token(NewLine) |\r\n|
    arrayParam: [
//@[004:0037) |   | |   ├─ObjectPropertySyntax
//@[004:0014) |   | |   | ├─IdentifierSyntax
//@[004:0014) |   | |   | | └─Token(Identifier) |arrayParam|
//@[014:0015) |   | |   | ├─Token(Colon) |:|
//@[016:0037) |   | |   | └─ArraySyntax
//@[016:0017) |   | |   |   ├─Token(LeftSquare) |[|
//@[017:0019) |   | |   |   ├─Token(NewLine) |\r\n|
      i + 1
//@[006:0011) |   | |   |   ├─ArrayItemSyntax
//@[006:0011) |   | |   |   | └─BinaryOperationSyntax
//@[006:0007) |   | |   |   |   ├─VariableAccessSyntax
//@[006:0007) |   | |   |   |   | └─IdentifierSyntax
//@[006:0007) |   | |   |   |   |   └─Token(Identifier) |i|
//@[008:0009) |   | |   |   |   ├─Token(Plus) |+|
//@[010:0011) |   | |   |   |   └─IntegerLiteralSyntax
//@[010:0011) |   | |   |   |     └─Token(Integer) |1|
//@[011:0013) |   | |   |   ├─Token(NewLine) |\r\n|
    ]
//@[004:0005) |   | |   |   └─Token(RightSquare) |]|
//@[005:0007) |   | |   ├─Token(NewLine) |\r\n|
    objParam: module
//@[004:0020) |   | |   ├─ObjectPropertySyntax
//@[004:0012) |   | |   | ├─IdentifierSyntax
//@[004:0012) |   | |   | | └─Token(Identifier) |objParam|
//@[012:0013) |   | |   | ├─Token(Colon) |:|
//@[014:0020) |   | |   | └─VariableAccessSyntax
//@[014:0020) |   | |   |   └─IdentifierSyntax
//@[014:0020) |   | |   |     └─Token(Identifier) |module|
//@[020:0022) |   | |   ├─Token(NewLine) |\r\n|
    stringParamB: module.location
//@[004:0033) |   | |   ├─ObjectPropertySyntax
//@[004:0016) |   | |   | ├─IdentifierSyntax
//@[004:0016) |   | |   | | └─Token(Identifier) |stringParamB|
//@[016:0017) |   | |   | ├─Token(Colon) |:|
//@[018:0033) |   | |   | └─PropertyAccessSyntax
//@[018:0024) |   | |   |   ├─VariableAccessSyntax
//@[018:0024) |   | |   |   | └─IdentifierSyntax
//@[018:0024) |   | |   |   |   └─Token(Identifier) |module|
//@[024:0025) |   | |   |   ├─Token(Dot) |.|
//@[025:0033) |   | |   |   └─IdentifierSyntax
//@[025:0033) |   | |   |     └─Token(Identifier) |location|
//@[033:0035) |   | |   ├─Token(NewLine) |\r\n|
    stringParamA: concat('a', i)
//@[004:0032) |   | |   ├─ObjectPropertySyntax
//@[004:0016) |   | |   | ├─IdentifierSyntax
//@[004:0016) |   | |   | | └─Token(Identifier) |stringParamA|
//@[016:0017) |   | |   | ├─Token(Colon) |:|
//@[018:0032) |   | |   | └─FunctionCallSyntax
//@[018:0024) |   | |   |   ├─IdentifierSyntax
//@[018:0024) |   | |   |   | └─Token(Identifier) |concat|
//@[024:0025) |   | |   |   ├─Token(LeftParen) |(|
//@[025:0028) |   | |   |   ├─FunctionArgumentSyntax
//@[025:0028) |   | |   |   | └─StringSyntax
//@[025:0028) |   | |   |   |   └─Token(StringComplete) |'a'|
//@[028:0029) |   | |   |   ├─Token(Comma) |,|
//@[030:0031) |   | |   |   ├─FunctionArgumentSyntax
//@[030:0031) |   | |   |   | └─VariableAccessSyntax
//@[030:0031) |   | |   |   |   └─IdentifierSyntax
//@[030:0031) |   | |   |   |     └─Token(Identifier) |i|
//@[031:0032) |   | |   |   └─Token(RightParen) |)|
//@[032:0034) |   | |   ├─Token(NewLine) |\r\n|
  }
//@[002:0003) |   | |   └─Token(RightBrace) |}|
//@[003:0005) |   | ├─Token(NewLine) |\r\n|
}]
//@[000:0001) |   | └─Token(RightBrace) |}|
//@[001:0002) |   └─Token(RightSquare) |]|
//@[002:0006) ├─Token(NewLine) |\r\n\r\n|

// nested module loop
//@[021:0023) ├─Token(NewLine) |\r\n|
module nestedModuleLoop 'modulea.bicep' = [for module in myModules: {
//@[000:0246) ├─ModuleDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |module|
//@[007:0023) | ├─IdentifierSyntax
//@[007:0023) | | └─Token(Identifier) |nestedModuleLoop|
//@[024:0039) | ├─StringSyntax
//@[024:0039) | | └─Token(StringComplete) |'modulea.bicep'|
//@[040:0041) | ├─Token(Assignment) |=|
//@[042:0246) | └─ForSyntax
//@[042:0043) |   ├─Token(LeftSquare) |[|
//@[043:0046) |   ├─Token(Identifier) |for|
//@[047:0053) |   ├─LocalVariableSyntax
//@[047:0053) |   | └─IdentifierSyntax
//@[047:0053) |   |   └─Token(Identifier) |module|
//@[054:0056) |   ├─Token(Identifier) |in|
//@[057:0066) |   ├─VariableAccessSyntax
//@[057:0066) |   | └─IdentifierSyntax
//@[057:0066) |   |   └─Token(Identifier) |myModules|
//@[066:0067) |   ├─Token(Colon) |:|
//@[068:0245) |   ├─ObjectSyntax
//@[068:0069) |   | ├─Token(LeftBrace) |{|
//@[069:0071) |   | ├─Token(NewLine) |\r\n|
  name: module.name
//@[002:0019) |   | ├─ObjectPropertySyntax
//@[002:0006) |   | | ├─IdentifierSyntax
//@[002:0006) |   | | | └─Token(Identifier) |name|
//@[006:0007) |   | | ├─Token(Colon) |:|
//@[008:0019) |   | | └─PropertyAccessSyntax
//@[008:0014) |   | |   ├─VariableAccessSyntax
//@[008:0014) |   | |   | └─IdentifierSyntax
//@[008:0014) |   | |   |   └─Token(Identifier) |module|
//@[014:0015) |   | |   ├─Token(Dot) |.|
//@[015:0019) |   | |   └─IdentifierSyntax
//@[015:0019) |   | |     └─Token(Identifier) |name|
//@[019:0021) |   | ├─Token(NewLine) |\r\n|
  params: {
//@[002:0150) |   | ├─ObjectPropertySyntax
//@[002:0008) |   | | ├─IdentifierSyntax
//@[002:0008) |   | | | └─Token(Identifier) |params|
//@[008:0009) |   | | ├─Token(Colon) |:|
//@[010:0150) |   | | └─ObjectSyntax
//@[010:0011) |   | |   ├─Token(LeftBrace) |{|
//@[011:0013) |   | |   ├─Token(NewLine) |\r\n|
    arrayParam: [for i in range(0,3): concat('test-', i, '-', module.name)]
//@[004:0075) |   | |   ├─ObjectPropertySyntax
//@[004:0014) |   | |   | ├─IdentifierSyntax
//@[004:0014) |   | |   | | └─Token(Identifier) |arrayParam|
//@[014:0015) |   | |   | ├─Token(Colon) |:|
//@[016:0075) |   | |   | └─ForSyntax
//@[016:0017) |   | |   |   ├─Token(LeftSquare) |[|
//@[017:0020) |   | |   |   ├─Token(Identifier) |for|
//@[021:0022) |   | |   |   ├─LocalVariableSyntax
//@[021:0022) |   | |   |   | └─IdentifierSyntax
//@[021:0022) |   | |   |   |   └─Token(Identifier) |i|
//@[023:0025) |   | |   |   ├─Token(Identifier) |in|
//@[026:0036) |   | |   |   ├─FunctionCallSyntax
//@[026:0031) |   | |   |   | ├─IdentifierSyntax
//@[026:0031) |   | |   |   | | └─Token(Identifier) |range|
//@[031:0032) |   | |   |   | ├─Token(LeftParen) |(|
//@[032:0033) |   | |   |   | ├─FunctionArgumentSyntax
//@[032:0033) |   | |   |   | | └─IntegerLiteralSyntax
//@[032:0033) |   | |   |   | |   └─Token(Integer) |0|
//@[033:0034) |   | |   |   | ├─Token(Comma) |,|
//@[034:0035) |   | |   |   | ├─FunctionArgumentSyntax
//@[034:0035) |   | |   |   | | └─IntegerLiteralSyntax
//@[034:0035) |   | |   |   | |   └─Token(Integer) |3|
//@[035:0036) |   | |   |   | └─Token(RightParen) |)|
//@[036:0037) |   | |   |   ├─Token(Colon) |:|
//@[038:0074) |   | |   |   ├─FunctionCallSyntax
//@[038:0044) |   | |   |   | ├─IdentifierSyntax
//@[038:0044) |   | |   |   | | └─Token(Identifier) |concat|
//@[044:0045) |   | |   |   | ├─Token(LeftParen) |(|
//@[045:0052) |   | |   |   | ├─FunctionArgumentSyntax
//@[045:0052) |   | |   |   | | └─StringSyntax
//@[045:0052) |   | |   |   | |   └─Token(StringComplete) |'test-'|
//@[052:0053) |   | |   |   | ├─Token(Comma) |,|
//@[054:0055) |   | |   |   | ├─FunctionArgumentSyntax
//@[054:0055) |   | |   |   | | └─VariableAccessSyntax
//@[054:0055) |   | |   |   | |   └─IdentifierSyntax
//@[054:0055) |   | |   |   | |     └─Token(Identifier) |i|
//@[055:0056) |   | |   |   | ├─Token(Comma) |,|
//@[057:0060) |   | |   |   | ├─FunctionArgumentSyntax
//@[057:0060) |   | |   |   | | └─StringSyntax
//@[057:0060) |   | |   |   | |   └─Token(StringComplete) |'-'|
//@[060:0061) |   | |   |   | ├─Token(Comma) |,|
//@[062:0073) |   | |   |   | ├─FunctionArgumentSyntax
//@[062:0073) |   | |   |   | | └─PropertyAccessSyntax
//@[062:0068) |   | |   |   | |   ├─VariableAccessSyntax
//@[062:0068) |   | |   |   | |   | └─IdentifierSyntax
//@[062:0068) |   | |   |   | |   |   └─Token(Identifier) |module|
//@[068:0069) |   | |   |   | |   ├─Token(Dot) |.|
//@[069:0073) |   | |   |   | |   └─IdentifierSyntax
//@[069:0073) |   | |   |   | |     └─Token(Identifier) |name|
//@[073:0074) |   | |   |   | └─Token(RightParen) |)|
//@[074:0075) |   | |   |   └─Token(RightSquare) |]|
//@[075:0077) |   | |   ├─Token(NewLine) |\r\n|
    objParam: module
//@[004:0020) |   | |   ├─ObjectPropertySyntax
//@[004:0012) |   | |   | ├─IdentifierSyntax
//@[004:0012) |   | |   | | └─Token(Identifier) |objParam|
//@[012:0013) |   | |   | ├─Token(Colon) |:|
//@[014:0020) |   | |   | └─VariableAccessSyntax
//@[014:0020) |   | |   |   └─IdentifierSyntax
//@[014:0020) |   | |   |     └─Token(Identifier) |module|
//@[020:0022) |   | |   ├─Token(NewLine) |\r\n|
    stringParamB: module.location
//@[004:0033) |   | |   ├─ObjectPropertySyntax
//@[004:0016) |   | |   | ├─IdentifierSyntax
//@[004:0016) |   | |   | | └─Token(Identifier) |stringParamB|
//@[016:0017) |   | |   | ├─Token(Colon) |:|
//@[018:0033) |   | |   | └─PropertyAccessSyntax
//@[018:0024) |   | |   |   ├─VariableAccessSyntax
//@[018:0024) |   | |   |   | └─IdentifierSyntax
//@[018:0024) |   | |   |   |   └─Token(Identifier) |module|
//@[024:0025) |   | |   |   ├─Token(Dot) |.|
//@[025:0033) |   | |   |   └─IdentifierSyntax
//@[025:0033) |   | |   |     └─Token(Identifier) |location|
//@[033:0035) |   | |   ├─Token(NewLine) |\r\n|
  }
//@[002:0003) |   | |   └─Token(RightBrace) |}|
//@[003:0005) |   | ├─Token(NewLine) |\r\n|
}]
//@[000:0001) |   | └─Token(RightBrace) |}|
//@[001:0002) |   └─Token(RightSquare) |]|
//@[002:0006) ├─Token(NewLine) |\r\n\r\n|

// duplicate identifiers across scopes are allowed (inner hides the outer)
//@[074:0076) ├─Token(NewLine) |\r\n|
module duplicateIdentifiersWithinLoop 'modulea.bicep' = [for x in emptyArray:{
//@[000:0234) ├─ModuleDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |module|
//@[007:0037) | ├─IdentifierSyntax
//@[007:0037) | | └─Token(Identifier) |duplicateIdentifiersWithinLoop|
//@[038:0053) | ├─StringSyntax
//@[038:0053) | | └─Token(StringComplete) |'modulea.bicep'|
//@[054:0055) | ├─Token(Assignment) |=|
//@[056:0234) | └─ForSyntax
//@[056:0057) |   ├─Token(LeftSquare) |[|
//@[057:0060) |   ├─Token(Identifier) |for|
//@[061:0062) |   ├─LocalVariableSyntax
//@[061:0062) |   | └─IdentifierSyntax
//@[061:0062) |   |   └─Token(Identifier) |x|
//@[063:0065) |   ├─Token(Identifier) |in|
//@[066:0076) |   ├─VariableAccessSyntax
//@[066:0076) |   | └─IdentifierSyntax
//@[066:0076) |   |   └─Token(Identifier) |emptyArray|
//@[076:0077) |   ├─Token(Colon) |:|
//@[077:0233) |   ├─ObjectSyntax
//@[077:0078) |   | ├─Token(LeftBrace) |{|
//@[078:0080) |   | ├─Token(NewLine) |\r\n|
  name: 'hello-${x}'
//@[002:0020) |   | ├─ObjectPropertySyntax
//@[002:0006) |   | | ├─IdentifierSyntax
//@[002:0006) |   | | | └─Token(Identifier) |name|
//@[006:0007) |   | | ├─Token(Colon) |:|
//@[008:0020) |   | | └─StringSyntax
//@[008:0017) |   | |   ├─Token(StringLeftPiece) |'hello-${|
//@[017:0018) |   | |   ├─VariableAccessSyntax
//@[017:0018) |   | |   | └─IdentifierSyntax
//@[017:0018) |   | |   |   └─Token(Identifier) |x|
//@[018:0020) |   | |   └─Token(StringRightPiece) |}'|
//@[020:0022) |   | ├─Token(NewLine) |\r\n|
  params: {
//@[002:0128) |   | ├─ObjectPropertySyntax
//@[002:0008) |   | | ├─IdentifierSyntax
//@[002:0008) |   | | | └─Token(Identifier) |params|
//@[008:0009) |   | | ├─Token(Colon) |:|
//@[010:0128) |   | | └─ObjectSyntax
//@[010:0011) |   | |   ├─Token(LeftBrace) |{|
//@[011:0013) |   | |   ├─Token(NewLine) |\r\n|
    objParam: {}
//@[004:0016) |   | |   ├─ObjectPropertySyntax
//@[004:0012) |   | |   | ├─IdentifierSyntax
//@[004:0012) |   | |   | | └─Token(Identifier) |objParam|
//@[012:0013) |   | |   | ├─Token(Colon) |:|
//@[014:0016) |   | |   | └─ObjectSyntax
//@[014:0015) |   | |   |   ├─Token(LeftBrace) |{|
//@[015:0016) |   | |   |   └─Token(RightBrace) |}|
//@[016:0018) |   | |   ├─Token(NewLine) |\r\n|
    stringParamA: 'test'
//@[004:0024) |   | |   ├─ObjectPropertySyntax
//@[004:0016) |   | |   | ├─IdentifierSyntax
//@[004:0016) |   | |   | | └─Token(Identifier) |stringParamA|
//@[016:0017) |   | |   | ├─Token(Colon) |:|
//@[018:0024) |   | |   | └─StringSyntax
//@[018:0024) |   | |   |   └─Token(StringComplete) |'test'|
//@[024:0026) |   | |   ├─Token(NewLine) |\r\n|
    stringParamB: 'test'
//@[004:0024) |   | |   ├─ObjectPropertySyntax
//@[004:0016) |   | |   | ├─IdentifierSyntax
//@[004:0016) |   | |   | | └─Token(Identifier) |stringParamB|
//@[016:0017) |   | |   | ├─Token(Colon) |:|
//@[018:0024) |   | |   | └─StringSyntax
//@[018:0024) |   | |   |   └─Token(StringComplete) |'test'|
//@[024:0026) |   | |   ├─Token(NewLine) |\r\n|
    arrayParam: [for x in emptyArray: x]
//@[004:0040) |   | |   ├─ObjectPropertySyntax
//@[004:0014) |   | |   | ├─IdentifierSyntax
//@[004:0014) |   | |   | | └─Token(Identifier) |arrayParam|
//@[014:0015) |   | |   | ├─Token(Colon) |:|
//@[016:0040) |   | |   | └─ForSyntax
//@[016:0017) |   | |   |   ├─Token(LeftSquare) |[|
//@[017:0020) |   | |   |   ├─Token(Identifier) |for|
//@[021:0022) |   | |   |   ├─LocalVariableSyntax
//@[021:0022) |   | |   |   | └─IdentifierSyntax
//@[021:0022) |   | |   |   |   └─Token(Identifier) |x|
//@[023:0025) |   | |   |   ├─Token(Identifier) |in|
//@[026:0036) |   | |   |   ├─VariableAccessSyntax
//@[026:0036) |   | |   |   | └─IdentifierSyntax
//@[026:0036) |   | |   |   |   └─Token(Identifier) |emptyArray|
//@[036:0037) |   | |   |   ├─Token(Colon) |:|
//@[038:0039) |   | |   |   ├─VariableAccessSyntax
//@[038:0039) |   | |   |   | └─IdentifierSyntax
//@[038:0039) |   | |   |   |   └─Token(Identifier) |x|
//@[039:0040) |   | |   |   └─Token(RightSquare) |]|
//@[040:0042) |   | |   ├─Token(NewLine) |\r\n|
  }
//@[002:0003) |   | |   └─Token(RightBrace) |}|
//@[003:0005) |   | ├─Token(NewLine) |\r\n|
}]
//@[000:0001) |   | └─Token(RightBrace) |}|
//@[001:0002) |   └─Token(RightSquare) |]|
//@[002:0006) ├─Token(NewLine) |\r\n\r\n|

// duplicate identifiers across scopes are allowed (inner hides the outer)
//@[074:0076) ├─Token(NewLine) |\r\n|
var duplicateAcrossScopes = 'hello'
//@[000:0035) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0025) | ├─IdentifierSyntax
//@[004:0025) | | └─Token(Identifier) |duplicateAcrossScopes|
//@[026:0027) | ├─Token(Assignment) |=|
//@[028:0035) | └─StringSyntax
//@[028:0035) |   └─Token(StringComplete) |'hello'|
//@[035:0037) ├─Token(NewLine) |\r\n|
module duplicateInGlobalAndOneLoop 'modulea.bicep' = [for duplicateAcrossScopes in []: {
//@[000:0264) ├─ModuleDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |module|
//@[007:0034) | ├─IdentifierSyntax
//@[007:0034) | | └─Token(Identifier) |duplicateInGlobalAndOneLoop|
//@[035:0050) | ├─StringSyntax
//@[035:0050) | | └─Token(StringComplete) |'modulea.bicep'|
//@[051:0052) | ├─Token(Assignment) |=|
//@[053:0264) | └─ForSyntax
//@[053:0054) |   ├─Token(LeftSquare) |[|
//@[054:0057) |   ├─Token(Identifier) |for|
//@[058:0079) |   ├─LocalVariableSyntax
//@[058:0079) |   | └─IdentifierSyntax
//@[058:0079) |   |   └─Token(Identifier) |duplicateAcrossScopes|
//@[080:0082) |   ├─Token(Identifier) |in|
//@[083:0085) |   ├─ArraySyntax
//@[083:0084) |   | ├─Token(LeftSquare) |[|
//@[084:0085) |   | └─Token(RightSquare) |]|
//@[085:0086) |   ├─Token(Colon) |:|
//@[087:0263) |   ├─ObjectSyntax
//@[087:0088) |   | ├─Token(LeftBrace) |{|
//@[088:0090) |   | ├─Token(NewLine) |\r\n|
  name: 'hello-${duplicateAcrossScopes}'
//@[002:0040) |   | ├─ObjectPropertySyntax
//@[002:0006) |   | | ├─IdentifierSyntax
//@[002:0006) |   | | | └─Token(Identifier) |name|
//@[006:0007) |   | | ├─Token(Colon) |:|
//@[008:0040) |   | | └─StringSyntax
//@[008:0017) |   | |   ├─Token(StringLeftPiece) |'hello-${|
//@[017:0038) |   | |   ├─VariableAccessSyntax
//@[017:0038) |   | |   | └─IdentifierSyntax
//@[017:0038) |   | |   |   └─Token(Identifier) |duplicateAcrossScopes|
//@[038:0040) |   | |   └─Token(StringRightPiece) |}'|
//@[040:0042) |   | ├─Token(NewLine) |\r\n|
  params: {
//@[002:0128) |   | ├─ObjectPropertySyntax
//@[002:0008) |   | | ├─IdentifierSyntax
//@[002:0008) |   | | | └─Token(Identifier) |params|
//@[008:0009) |   | | ├─Token(Colon) |:|
//@[010:0128) |   | | └─ObjectSyntax
//@[010:0011) |   | |   ├─Token(LeftBrace) |{|
//@[011:0013) |   | |   ├─Token(NewLine) |\r\n|
    objParam: {}
//@[004:0016) |   | |   ├─ObjectPropertySyntax
//@[004:0012) |   | |   | ├─IdentifierSyntax
//@[004:0012) |   | |   | | └─Token(Identifier) |objParam|
//@[012:0013) |   | |   | ├─Token(Colon) |:|
//@[014:0016) |   | |   | └─ObjectSyntax
//@[014:0015) |   | |   |   ├─Token(LeftBrace) |{|
//@[015:0016) |   | |   |   └─Token(RightBrace) |}|
//@[016:0018) |   | |   ├─Token(NewLine) |\r\n|
    stringParamA: 'test'
//@[004:0024) |   | |   ├─ObjectPropertySyntax
//@[004:0016) |   | |   | ├─IdentifierSyntax
//@[004:0016) |   | |   | | └─Token(Identifier) |stringParamA|
//@[016:0017) |   | |   | ├─Token(Colon) |:|
//@[018:0024) |   | |   | └─StringSyntax
//@[018:0024) |   | |   |   └─Token(StringComplete) |'test'|
//@[024:0026) |   | |   ├─Token(NewLine) |\r\n|
    stringParamB: 'test'
//@[004:0024) |   | |   ├─ObjectPropertySyntax
//@[004:0016) |   | |   | ├─IdentifierSyntax
//@[004:0016) |   | |   | | └─Token(Identifier) |stringParamB|
//@[016:0017) |   | |   | ├─Token(Colon) |:|
//@[018:0024) |   | |   | └─StringSyntax
//@[018:0024) |   | |   |   └─Token(StringComplete) |'test'|
//@[024:0026) |   | |   ├─Token(NewLine) |\r\n|
    arrayParam: [for x in emptyArray: x]
//@[004:0040) |   | |   ├─ObjectPropertySyntax
//@[004:0014) |   | |   | ├─IdentifierSyntax
//@[004:0014) |   | |   | | └─Token(Identifier) |arrayParam|
//@[014:0015) |   | |   | ├─Token(Colon) |:|
//@[016:0040) |   | |   | └─ForSyntax
//@[016:0017) |   | |   |   ├─Token(LeftSquare) |[|
//@[017:0020) |   | |   |   ├─Token(Identifier) |for|
//@[021:0022) |   | |   |   ├─LocalVariableSyntax
//@[021:0022) |   | |   |   | └─IdentifierSyntax
//@[021:0022) |   | |   |   |   └─Token(Identifier) |x|
//@[023:0025) |   | |   |   ├─Token(Identifier) |in|
//@[026:0036) |   | |   |   ├─VariableAccessSyntax
//@[026:0036) |   | |   |   | └─IdentifierSyntax
//@[026:0036) |   | |   |   |   └─Token(Identifier) |emptyArray|
//@[036:0037) |   | |   |   ├─Token(Colon) |:|
//@[038:0039) |   | |   |   ├─VariableAccessSyntax
//@[038:0039) |   | |   |   | └─IdentifierSyntax
//@[038:0039) |   | |   |   |   └─Token(Identifier) |x|
//@[039:0040) |   | |   |   └─Token(RightSquare) |]|
//@[040:0042) |   | |   ├─Token(NewLine) |\r\n|
  }
//@[002:0003) |   | |   └─Token(RightBrace) |}|
//@[003:0005) |   | ├─Token(NewLine) |\r\n|
}]
//@[000:0001) |   | └─Token(RightBrace) |}|
//@[001:0002) |   └─Token(RightSquare) |]|
//@[002:0006) ├─Token(NewLine) |\r\n\r\n|

var someDuplicate = true
//@[000:0024) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0017) | ├─IdentifierSyntax
//@[004:0017) | | └─Token(Identifier) |someDuplicate|
//@[018:0019) | ├─Token(Assignment) |=|
//@[020:0024) | └─BooleanLiteralSyntax
//@[020:0024) |   └─Token(TrueKeyword) |true|
//@[024:0026) ├─Token(NewLine) |\r\n|
var otherDuplicate = false
//@[000:0026) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0018) | ├─IdentifierSyntax
//@[004:0018) | | └─Token(Identifier) |otherDuplicate|
//@[019:0020) | ├─Token(Assignment) |=|
//@[021:0026) | └─BooleanLiteralSyntax
//@[021:0026) |   └─Token(FalseKeyword) |false|
//@[026:0028) ├─Token(NewLine) |\r\n|
module duplicatesEverywhere 'modulea.bicep' = [for someDuplicate in []: {
//@[000:0263) ├─ModuleDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |module|
//@[007:0027) | ├─IdentifierSyntax
//@[007:0027) | | └─Token(Identifier) |duplicatesEverywhere|
//@[028:0043) | ├─StringSyntax
//@[028:0043) | | └─Token(StringComplete) |'modulea.bicep'|
//@[044:0045) | ├─Token(Assignment) |=|
//@[046:0263) | └─ForSyntax
//@[046:0047) |   ├─Token(LeftSquare) |[|
//@[047:0050) |   ├─Token(Identifier) |for|
//@[051:0064) |   ├─LocalVariableSyntax
//@[051:0064) |   | └─IdentifierSyntax
//@[051:0064) |   |   └─Token(Identifier) |someDuplicate|
//@[065:0067) |   ├─Token(Identifier) |in|
//@[068:0070) |   ├─ArraySyntax
//@[068:0069) |   | ├─Token(LeftSquare) |[|
//@[069:0070) |   | └─Token(RightSquare) |]|
//@[070:0071) |   ├─Token(Colon) |:|
//@[072:0262) |   ├─ObjectSyntax
//@[072:0073) |   | ├─Token(LeftBrace) |{|
//@[073:0075) |   | ├─Token(NewLine) |\r\n|
  name: 'hello-${someDuplicate}'
//@[002:0032) |   | ├─ObjectPropertySyntax
//@[002:0006) |   | | ├─IdentifierSyntax
//@[002:0006) |   | | | └─Token(Identifier) |name|
//@[006:0007) |   | | ├─Token(Colon) |:|
//@[008:0032) |   | | └─StringSyntax
//@[008:0017) |   | |   ├─Token(StringLeftPiece) |'hello-${|
//@[017:0030) |   | |   ├─VariableAccessSyntax
//@[017:0030) |   | |   | └─IdentifierSyntax
//@[017:0030) |   | |   |   └─Token(Identifier) |someDuplicate|
//@[030:0032) |   | |   └─Token(StringRightPiece) |}'|
//@[032:0034) |   | ├─Token(NewLine) |\r\n|
  params: {
//@[002:0150) |   | ├─ObjectPropertySyntax
//@[002:0008) |   | | ├─IdentifierSyntax
//@[002:0008) |   | | | └─Token(Identifier) |params|
//@[008:0009) |   | | ├─Token(Colon) |:|
//@[010:0150) |   | | └─ObjectSyntax
//@[010:0011) |   | |   ├─Token(LeftBrace) |{|
//@[011:0013) |   | |   ├─Token(NewLine) |\r\n|
    objParam: {}
//@[004:0016) |   | |   ├─ObjectPropertySyntax
//@[004:0012) |   | |   | ├─IdentifierSyntax
//@[004:0012) |   | |   | | └─Token(Identifier) |objParam|
//@[012:0013) |   | |   | ├─Token(Colon) |:|
//@[014:0016) |   | |   | └─ObjectSyntax
//@[014:0015) |   | |   |   ├─Token(LeftBrace) |{|
//@[015:0016) |   | |   |   └─Token(RightBrace) |}|
//@[016:0018) |   | |   ├─Token(NewLine) |\r\n|
    stringParamB: 'test'
//@[004:0024) |   | |   ├─ObjectPropertySyntax
//@[004:0016) |   | |   | ├─IdentifierSyntax
//@[004:0016) |   | |   | | └─Token(Identifier) |stringParamB|
//@[016:0017) |   | |   | ├─Token(Colon) |:|
//@[018:0024) |   | |   | └─StringSyntax
//@[018:0024) |   | |   |   └─Token(StringComplete) |'test'|
//@[024:0026) |   | |   ├─Token(NewLine) |\r\n|
    arrayParam: [for otherDuplicate in emptyArray: '${someDuplicate}-${otherDuplicate}']
//@[004:0088) |   | |   ├─ObjectPropertySyntax
//@[004:0014) |   | |   | ├─IdentifierSyntax
//@[004:0014) |   | |   | | └─Token(Identifier) |arrayParam|
//@[014:0015) |   | |   | ├─Token(Colon) |:|
//@[016:0088) |   | |   | └─ForSyntax
//@[016:0017) |   | |   |   ├─Token(LeftSquare) |[|
//@[017:0020) |   | |   |   ├─Token(Identifier) |for|
//@[021:0035) |   | |   |   ├─LocalVariableSyntax
//@[021:0035) |   | |   |   | └─IdentifierSyntax
//@[021:0035) |   | |   |   |   └─Token(Identifier) |otherDuplicate|
//@[036:0038) |   | |   |   ├─Token(Identifier) |in|
//@[039:0049) |   | |   |   ├─VariableAccessSyntax
//@[039:0049) |   | |   |   | └─IdentifierSyntax
//@[039:0049) |   | |   |   |   └─Token(Identifier) |emptyArray|
//@[049:0050) |   | |   |   ├─Token(Colon) |:|
//@[051:0087) |   | |   |   ├─StringSyntax
//@[051:0054) |   | |   |   | ├─Token(StringLeftPiece) |'${|
//@[054:0067) |   | |   |   | ├─VariableAccessSyntax
//@[054:0067) |   | |   |   | | └─IdentifierSyntax
//@[054:0067) |   | |   |   | |   └─Token(Identifier) |someDuplicate|
//@[067:0071) |   | |   |   | ├─Token(StringMiddlePiece) |}-${|
//@[071:0085) |   | |   |   | ├─VariableAccessSyntax
//@[071:0085) |   | |   |   | | └─IdentifierSyntax
//@[071:0085) |   | |   |   | |   └─Token(Identifier) |otherDuplicate|
//@[085:0087) |   | |   |   | └─Token(StringRightPiece) |}'|
//@[087:0088) |   | |   |   └─Token(RightSquare) |]|
//@[088:0090) |   | |   ├─Token(NewLine) |\r\n|
  }
//@[002:0003) |   | |   └─Token(RightBrace) |}|
//@[003:0005) |   | ├─Token(NewLine) |\r\n|
}]
//@[000:0001) |   | └─Token(RightBrace) |}|
//@[001:0002) |   └─Token(RightSquare) |]|
//@[002:0006) ├─Token(NewLine) |\r\n\r\n|

module propertyLoopInsideParameterValue 'modulea.bicep' = {
//@[000:0438) ├─ModuleDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |module|
//@[007:0039) | ├─IdentifierSyntax
//@[007:0039) | | └─Token(Identifier) |propertyLoopInsideParameterValue|
//@[040:0055) | ├─StringSyntax
//@[040:0055) | | └─Token(StringComplete) |'modulea.bicep'|
//@[056:0057) | ├─Token(Assignment) |=|
//@[058:0438) | └─ObjectSyntax
//@[058:0059) |   ├─Token(LeftBrace) |{|
//@[059:0061) |   ├─Token(NewLine) |\r\n|
  name: 'propertyLoopInsideParameterValue'
//@[002:0042) |   ├─ObjectPropertySyntax
//@[002:0006) |   | ├─IdentifierSyntax
//@[002:0006) |   | | └─Token(Identifier) |name|
//@[006:0007) |   | ├─Token(Colon) |:|
//@[008:0042) |   | └─StringSyntax
//@[008:0042) |   |   └─Token(StringComplete) |'propertyLoopInsideParameterValue'|
//@[042:0044) |   ├─Token(NewLine) |\r\n|
  params: {
//@[002:0330) |   ├─ObjectPropertySyntax
//@[002:0008) |   | ├─IdentifierSyntax
//@[002:0008) |   | | └─Token(Identifier) |params|
//@[008:0009) |   | ├─Token(Colon) |:|
//@[010:0330) |   | └─ObjectSyntax
//@[010:0011) |   |   ├─Token(LeftBrace) |{|
//@[011:0013) |   |   ├─Token(NewLine) |\r\n|
    objParam: {
//@[004:0209) |   |   ├─ObjectPropertySyntax
//@[004:0012) |   |   | ├─IdentifierSyntax
//@[004:0012) |   |   | | └─Token(Identifier) |objParam|
//@[012:0013) |   |   | ├─Token(Colon) |:|
//@[014:0209) |   |   | └─ObjectSyntax
//@[014:0015) |   |   |   ├─Token(LeftBrace) |{|
//@[015:0017) |   |   |   ├─Token(NewLine) |\r\n|
      a: [for i in range(0,10): i]
//@[006:0034) |   |   |   ├─ObjectPropertySyntax
//@[006:0007) |   |   |   | ├─IdentifierSyntax
//@[006:0007) |   |   |   | | └─Token(Identifier) |a|
//@[007:0008) |   |   |   | ├─Token(Colon) |:|
//@[009:0034) |   |   |   | └─ForSyntax
//@[009:0010) |   |   |   |   ├─Token(LeftSquare) |[|
//@[010:0013) |   |   |   |   ├─Token(Identifier) |for|
//@[014:0015) |   |   |   |   ├─LocalVariableSyntax
//@[014:0015) |   |   |   |   | └─IdentifierSyntax
//@[014:0015) |   |   |   |   |   └─Token(Identifier) |i|
//@[016:0018) |   |   |   |   ├─Token(Identifier) |in|
//@[019:0030) |   |   |   |   ├─FunctionCallSyntax
//@[019:0024) |   |   |   |   | ├─IdentifierSyntax
//@[019:0024) |   |   |   |   | | └─Token(Identifier) |range|
//@[024:0025) |   |   |   |   | ├─Token(LeftParen) |(|
//@[025:0026) |   |   |   |   | ├─FunctionArgumentSyntax
//@[025:0026) |   |   |   |   | | └─IntegerLiteralSyntax
//@[025:0026) |   |   |   |   | |   └─Token(Integer) |0|
//@[026:0027) |   |   |   |   | ├─Token(Comma) |,|
//@[027:0029) |   |   |   |   | ├─FunctionArgumentSyntax
//@[027:0029) |   |   |   |   | | └─IntegerLiteralSyntax
//@[027:0029) |   |   |   |   | |   └─Token(Integer) |10|
//@[029:0030) |   |   |   |   | └─Token(RightParen) |)|
//@[030:0031) |   |   |   |   ├─Token(Colon) |:|
//@[032:0033) |   |   |   |   ├─VariableAccessSyntax
//@[032:0033) |   |   |   |   | └─IdentifierSyntax
//@[032:0033) |   |   |   |   |   └─Token(Identifier) |i|
//@[033:0034) |   |   |   |   └─Token(RightSquare) |]|
//@[034:0036) |   |   |   ├─Token(NewLine) |\r\n|
      b: [for i in range(1,2): i]
//@[006:0033) |   |   |   ├─ObjectPropertySyntax
//@[006:0007) |   |   |   | ├─IdentifierSyntax
//@[006:0007) |   |   |   | | └─Token(Identifier) |b|
//@[007:0008) |   |   |   | ├─Token(Colon) |:|
//@[009:0033) |   |   |   | └─ForSyntax
//@[009:0010) |   |   |   |   ├─Token(LeftSquare) |[|
//@[010:0013) |   |   |   |   ├─Token(Identifier) |for|
//@[014:0015) |   |   |   |   ├─LocalVariableSyntax
//@[014:0015) |   |   |   |   | └─IdentifierSyntax
//@[014:0015) |   |   |   |   |   └─Token(Identifier) |i|
//@[016:0018) |   |   |   |   ├─Token(Identifier) |in|
//@[019:0029) |   |   |   |   ├─FunctionCallSyntax
//@[019:0024) |   |   |   |   | ├─IdentifierSyntax
//@[019:0024) |   |   |   |   | | └─Token(Identifier) |range|
//@[024:0025) |   |   |   |   | ├─Token(LeftParen) |(|
//@[025:0026) |   |   |   |   | ├─FunctionArgumentSyntax
//@[025:0026) |   |   |   |   | | └─IntegerLiteralSyntax
//@[025:0026) |   |   |   |   | |   └─Token(Integer) |1|
//@[026:0027) |   |   |   |   | ├─Token(Comma) |,|
//@[027:0028) |   |   |   |   | ├─FunctionArgumentSyntax
//@[027:0028) |   |   |   |   | | └─IntegerLiteralSyntax
//@[027:0028) |   |   |   |   | |   └─Token(Integer) |2|
//@[028:0029) |   |   |   |   | └─Token(RightParen) |)|
//@[029:0030) |   |   |   |   ├─Token(Colon) |:|
//@[031:0032) |   |   |   |   ├─VariableAccessSyntax
//@[031:0032) |   |   |   |   | └─IdentifierSyntax
//@[031:0032) |   |   |   |   |   └─Token(Identifier) |i|
//@[032:0033) |   |   |   |   └─Token(RightSquare) |]|
//@[033:0035) |   |   |   ├─Token(NewLine) |\r\n|
      c: {
//@[006:0056) |   |   |   ├─ObjectPropertySyntax
//@[006:0007) |   |   |   | ├─IdentifierSyntax
//@[006:0007) |   |   |   | | └─Token(Identifier) |c|
//@[007:0008) |   |   |   | ├─Token(Colon) |:|
//@[009:0056) |   |   |   | └─ObjectSyntax
//@[009:0010) |   |   |   |   ├─Token(LeftBrace) |{|
//@[010:0012) |   |   |   |   ├─Token(NewLine) |\r\n|
        d: [for j in range(2,3): j]
//@[008:0035) |   |   |   |   ├─ObjectPropertySyntax
//@[008:0009) |   |   |   |   | ├─IdentifierSyntax
//@[008:0009) |   |   |   |   | | └─Token(Identifier) |d|
//@[009:0010) |   |   |   |   | ├─Token(Colon) |:|
//@[011:0035) |   |   |   |   | └─ForSyntax
//@[011:0012) |   |   |   |   |   ├─Token(LeftSquare) |[|
//@[012:0015) |   |   |   |   |   ├─Token(Identifier) |for|
//@[016:0017) |   |   |   |   |   ├─LocalVariableSyntax
//@[016:0017) |   |   |   |   |   | └─IdentifierSyntax
//@[016:0017) |   |   |   |   |   |   └─Token(Identifier) |j|
//@[018:0020) |   |   |   |   |   ├─Token(Identifier) |in|
//@[021:0031) |   |   |   |   |   ├─FunctionCallSyntax
//@[021:0026) |   |   |   |   |   | ├─IdentifierSyntax
//@[021:0026) |   |   |   |   |   | | └─Token(Identifier) |range|
//@[026:0027) |   |   |   |   |   | ├─Token(LeftParen) |(|
//@[027:0028) |   |   |   |   |   | ├─FunctionArgumentSyntax
//@[027:0028) |   |   |   |   |   | | └─IntegerLiteralSyntax
//@[027:0028) |   |   |   |   |   | |   └─Token(Integer) |2|
//@[028:0029) |   |   |   |   |   | ├─Token(Comma) |,|
//@[029:0030) |   |   |   |   |   | ├─FunctionArgumentSyntax
//@[029:0030) |   |   |   |   |   | | └─IntegerLiteralSyntax
//@[029:0030) |   |   |   |   |   | |   └─Token(Integer) |3|
//@[030:0031) |   |   |   |   |   | └─Token(RightParen) |)|
//@[031:0032) |   |   |   |   |   ├─Token(Colon) |:|
//@[033:0034) |   |   |   |   |   ├─VariableAccessSyntax
//@[033:0034) |   |   |   |   |   | └─IdentifierSyntax
//@[033:0034) |   |   |   |   |   |   └─Token(Identifier) |j|
//@[034:0035) |   |   |   |   |   └─Token(RightSquare) |]|
//@[035:0037) |   |   |   |   ├─Token(NewLine) |\r\n|
      }
//@[006:0007) |   |   |   |   └─Token(RightBrace) |}|
//@[007:0009) |   |   |   ├─Token(NewLine) |\r\n|
      e: [for k in range(4,4): {
//@[006:0056) |   |   |   ├─ObjectPropertySyntax
//@[006:0007) |   |   |   | ├─IdentifierSyntax
//@[006:0007) |   |   |   | | └─Token(Identifier) |e|
//@[007:0008) |   |   |   | ├─Token(Colon) |:|
//@[009:0056) |   |   |   | └─ForSyntax
//@[009:0010) |   |   |   |   ├─Token(LeftSquare) |[|
//@[010:0013) |   |   |   |   ├─Token(Identifier) |for|
//@[014:0015) |   |   |   |   ├─LocalVariableSyntax
//@[014:0015) |   |   |   |   | └─IdentifierSyntax
//@[014:0015) |   |   |   |   |   └─Token(Identifier) |k|
//@[016:0018) |   |   |   |   ├─Token(Identifier) |in|
//@[019:0029) |   |   |   |   ├─FunctionCallSyntax
//@[019:0024) |   |   |   |   | ├─IdentifierSyntax
//@[019:0024) |   |   |   |   | | └─Token(Identifier) |range|
//@[024:0025) |   |   |   |   | ├─Token(LeftParen) |(|
//@[025:0026) |   |   |   |   | ├─FunctionArgumentSyntax
//@[025:0026) |   |   |   |   | | └─IntegerLiteralSyntax
//@[025:0026) |   |   |   |   | |   └─Token(Integer) |4|
//@[026:0027) |   |   |   |   | ├─Token(Comma) |,|
//@[027:0028) |   |   |   |   | ├─FunctionArgumentSyntax
//@[027:0028) |   |   |   |   | | └─IntegerLiteralSyntax
//@[027:0028) |   |   |   |   | |   └─Token(Integer) |4|
//@[028:0029) |   |   |   |   | └─Token(RightParen) |)|
//@[029:0030) |   |   |   |   ├─Token(Colon) |:|
//@[031:0055) |   |   |   |   ├─ObjectSyntax
//@[031:0032) |   |   |   |   | ├─Token(LeftBrace) |{|
//@[032:0034) |   |   |   |   | ├─Token(NewLine) |\r\n|
        f: k
//@[008:0012) |   |   |   |   | ├─ObjectPropertySyntax
//@[008:0009) |   |   |   |   | | ├─IdentifierSyntax
//@[008:0009) |   |   |   |   | | | └─Token(Identifier) |f|
//@[009:0010) |   |   |   |   | | ├─Token(Colon) |:|
//@[011:0012) |   |   |   |   | | └─VariableAccessSyntax
//@[011:0012) |   |   |   |   | |   └─IdentifierSyntax
//@[011:0012) |   |   |   |   | |     └─Token(Identifier) |k|
//@[012:0014) |   |   |   |   | ├─Token(NewLine) |\r\n|
      }]
//@[006:0007) |   |   |   |   | └─Token(RightBrace) |}|
//@[007:0008) |   |   |   |   └─Token(RightSquare) |]|
//@[008:0010) |   |   |   ├─Token(NewLine) |\r\n|
    }
//@[004:0005) |   |   |   └─Token(RightBrace) |}|
//@[005:0007) |   |   ├─Token(NewLine) |\r\n|
    stringParamB: ''
//@[004:0020) |   |   ├─ObjectPropertySyntax
//@[004:0016) |   |   | ├─IdentifierSyntax
//@[004:0016) |   |   | | └─Token(Identifier) |stringParamB|
//@[016:0017) |   |   | ├─Token(Colon) |:|
//@[018:0020) |   |   | └─StringSyntax
//@[018:0020) |   |   |   └─Token(StringComplete) |''|
//@[020:0022) |   |   ├─Token(NewLine) |\r\n|
    arrayParam: [
//@[004:0079) |   |   ├─ObjectPropertySyntax
//@[004:0014) |   |   | ├─IdentifierSyntax
//@[004:0014) |   |   | | └─Token(Identifier) |arrayParam|
//@[014:0015) |   |   | ├─Token(Colon) |:|
//@[016:0079) |   |   | └─ArraySyntax
//@[016:0017) |   |   |   ├─Token(LeftSquare) |[|
//@[017:0019) |   |   |   ├─Token(NewLine) |\r\n|
      {
//@[006:0053) |   |   |   ├─ArrayItemSyntax
//@[006:0053) |   |   |   | └─ObjectSyntax
//@[006:0007) |   |   |   |   ├─Token(LeftBrace) |{|
//@[007:0009) |   |   |   |   ├─Token(NewLine) |\r\n|
        e: [for j in range(7,7): j]
//@[008:0035) |   |   |   |   ├─ObjectPropertySyntax
//@[008:0009) |   |   |   |   | ├─IdentifierSyntax
//@[008:0009) |   |   |   |   | | └─Token(Identifier) |e|
//@[009:0010) |   |   |   |   | ├─Token(Colon) |:|
//@[011:0035) |   |   |   |   | └─ForSyntax
//@[011:0012) |   |   |   |   |   ├─Token(LeftSquare) |[|
//@[012:0015) |   |   |   |   |   ├─Token(Identifier) |for|
//@[016:0017) |   |   |   |   |   ├─LocalVariableSyntax
//@[016:0017) |   |   |   |   |   | └─IdentifierSyntax
//@[016:0017) |   |   |   |   |   |   └─Token(Identifier) |j|
//@[018:0020) |   |   |   |   |   ├─Token(Identifier) |in|
//@[021:0031) |   |   |   |   |   ├─FunctionCallSyntax
//@[021:0026) |   |   |   |   |   | ├─IdentifierSyntax
//@[021:0026) |   |   |   |   |   | | └─Token(Identifier) |range|
//@[026:0027) |   |   |   |   |   | ├─Token(LeftParen) |(|
//@[027:0028) |   |   |   |   |   | ├─FunctionArgumentSyntax
//@[027:0028) |   |   |   |   |   | | └─IntegerLiteralSyntax
//@[027:0028) |   |   |   |   |   | |   └─Token(Integer) |7|
//@[028:0029) |   |   |   |   |   | ├─Token(Comma) |,|
//@[029:0030) |   |   |   |   |   | ├─FunctionArgumentSyntax
//@[029:0030) |   |   |   |   |   | | └─IntegerLiteralSyntax
//@[029:0030) |   |   |   |   |   | |   └─Token(Integer) |7|
//@[030:0031) |   |   |   |   |   | └─Token(RightParen) |)|
//@[031:0032) |   |   |   |   |   ├─Token(Colon) |:|
//@[033:0034) |   |   |   |   |   ├─VariableAccessSyntax
//@[033:0034) |   |   |   |   |   | └─IdentifierSyntax
//@[033:0034) |   |   |   |   |   |   └─Token(Identifier) |j|
//@[034:0035) |   |   |   |   |   └─Token(RightSquare) |]|
//@[035:0037) |   |   |   |   ├─Token(NewLine) |\r\n|
      }
//@[006:0007) |   |   |   |   └─Token(RightBrace) |}|
//@[007:0009) |   |   |   ├─Token(NewLine) |\r\n|
    ]
//@[004:0005) |   |   |   └─Token(RightSquare) |]|
//@[005:0007) |   |   ├─Token(NewLine) |\r\n|
  }
//@[002:0003) |   |   └─Token(RightBrace) |}|
//@[003:0005) |   ├─Token(NewLine) |\r\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0005) ├─Token(NewLine) |\r\n\r\n|

module propertyLoopInsideParameterValueWithIndexes 'modulea.bicep' = {
//@[000:0514) ├─ModuleDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |module|
//@[007:0050) | ├─IdentifierSyntax
//@[007:0050) | | └─Token(Identifier) |propertyLoopInsideParameterValueWithIndexes|
//@[051:0066) | ├─StringSyntax
//@[051:0066) | | └─Token(StringComplete) |'modulea.bicep'|
//@[067:0068) | ├─Token(Assignment) |=|
//@[069:0514) | └─ObjectSyntax
//@[069:0070) |   ├─Token(LeftBrace) |{|
//@[070:0072) |   ├─Token(NewLine) |\r\n|
  name: 'propertyLoopInsideParameterValueWithIndexes'
//@[002:0053) |   ├─ObjectPropertySyntax
//@[002:0006) |   | ├─IdentifierSyntax
//@[002:0006) |   | | └─Token(Identifier) |name|
//@[006:0007) |   | ├─Token(Colon) |:|
//@[008:0053) |   | └─StringSyntax
//@[008:0053) |   |   └─Token(StringComplete) |'propertyLoopInsideParameterValueWithIndexes'|
//@[053:0055) |   ├─Token(NewLine) |\r\n|
  params: {
//@[002:0384) |   ├─ObjectPropertySyntax
//@[002:0008) |   | ├─IdentifierSyntax
//@[002:0008) |   | | └─Token(Identifier) |params|
//@[008:0009) |   | ├─Token(Colon) |:|
//@[010:0384) |   | └─ObjectSyntax
//@[010:0011) |   |   ├─Token(LeftBrace) |{|
//@[011:0013) |   |   ├─Token(NewLine) |\r\n|
    objParam: {
//@[004:0263) |   |   ├─ObjectPropertySyntax
//@[004:0012) |   |   | ├─IdentifierSyntax
//@[004:0012) |   |   | | └─Token(Identifier) |objParam|
//@[012:0013) |   |   | ├─Token(Colon) |:|
//@[014:0263) |   |   | └─ObjectSyntax
//@[014:0015) |   |   |   ├─Token(LeftBrace) |{|
//@[015:0017) |   |   |   ├─Token(NewLine) |\r\n|
      a: [for (i, i2) in range(0,10): i + i2]
//@[006:0045) |   |   |   ├─ObjectPropertySyntax
//@[006:0007) |   |   |   | ├─IdentifierSyntax
//@[006:0007) |   |   |   | | └─Token(Identifier) |a|
//@[007:0008) |   |   |   | ├─Token(Colon) |:|
//@[009:0045) |   |   |   | └─ForSyntax
//@[009:0010) |   |   |   |   ├─Token(LeftSquare) |[|
//@[010:0013) |   |   |   |   ├─Token(Identifier) |for|
//@[014:0021) |   |   |   |   ├─VariableBlockSyntax
//@[014:0015) |   |   |   |   | ├─Token(LeftParen) |(|
//@[015:0016) |   |   |   |   | ├─LocalVariableSyntax
//@[015:0016) |   |   |   |   | | └─IdentifierSyntax
//@[015:0016) |   |   |   |   | |   └─Token(Identifier) |i|
//@[016:0017) |   |   |   |   | ├─Token(Comma) |,|
//@[018:0020) |   |   |   |   | ├─LocalVariableSyntax
//@[018:0020) |   |   |   |   | | └─IdentifierSyntax
//@[018:0020) |   |   |   |   | |   └─Token(Identifier) |i2|
//@[020:0021) |   |   |   |   | └─Token(RightParen) |)|
//@[022:0024) |   |   |   |   ├─Token(Identifier) |in|
//@[025:0036) |   |   |   |   ├─FunctionCallSyntax
//@[025:0030) |   |   |   |   | ├─IdentifierSyntax
//@[025:0030) |   |   |   |   | | └─Token(Identifier) |range|
//@[030:0031) |   |   |   |   | ├─Token(LeftParen) |(|
//@[031:0032) |   |   |   |   | ├─FunctionArgumentSyntax
//@[031:0032) |   |   |   |   | | └─IntegerLiteralSyntax
//@[031:0032) |   |   |   |   | |   └─Token(Integer) |0|
//@[032:0033) |   |   |   |   | ├─Token(Comma) |,|
//@[033:0035) |   |   |   |   | ├─FunctionArgumentSyntax
//@[033:0035) |   |   |   |   | | └─IntegerLiteralSyntax
//@[033:0035) |   |   |   |   | |   └─Token(Integer) |10|
//@[035:0036) |   |   |   |   | └─Token(RightParen) |)|
//@[036:0037) |   |   |   |   ├─Token(Colon) |:|
//@[038:0044) |   |   |   |   ├─BinaryOperationSyntax
//@[038:0039) |   |   |   |   | ├─VariableAccessSyntax
//@[038:0039) |   |   |   |   | | └─IdentifierSyntax
//@[038:0039) |   |   |   |   | |   └─Token(Identifier) |i|
//@[040:0041) |   |   |   |   | ├─Token(Plus) |+|
//@[042:0044) |   |   |   |   | └─VariableAccessSyntax
//@[042:0044) |   |   |   |   |   └─IdentifierSyntax
//@[042:0044) |   |   |   |   |     └─Token(Identifier) |i2|
//@[044:0045) |   |   |   |   └─Token(RightSquare) |]|
//@[045:0047) |   |   |   ├─Token(NewLine) |\r\n|
      b: [for (i, i2) in range(1,2): i / i2]
//@[006:0044) |   |   |   ├─ObjectPropertySyntax
//@[006:0007) |   |   |   | ├─IdentifierSyntax
//@[006:0007) |   |   |   | | └─Token(Identifier) |b|
//@[007:0008) |   |   |   | ├─Token(Colon) |:|
//@[009:0044) |   |   |   | └─ForSyntax
//@[009:0010) |   |   |   |   ├─Token(LeftSquare) |[|
//@[010:0013) |   |   |   |   ├─Token(Identifier) |for|
//@[014:0021) |   |   |   |   ├─VariableBlockSyntax
//@[014:0015) |   |   |   |   | ├─Token(LeftParen) |(|
//@[015:0016) |   |   |   |   | ├─LocalVariableSyntax
//@[015:0016) |   |   |   |   | | └─IdentifierSyntax
//@[015:0016) |   |   |   |   | |   └─Token(Identifier) |i|
//@[016:0017) |   |   |   |   | ├─Token(Comma) |,|
//@[018:0020) |   |   |   |   | ├─LocalVariableSyntax
//@[018:0020) |   |   |   |   | | └─IdentifierSyntax
//@[018:0020) |   |   |   |   | |   └─Token(Identifier) |i2|
//@[020:0021) |   |   |   |   | └─Token(RightParen) |)|
//@[022:0024) |   |   |   |   ├─Token(Identifier) |in|
//@[025:0035) |   |   |   |   ├─FunctionCallSyntax
//@[025:0030) |   |   |   |   | ├─IdentifierSyntax
//@[025:0030) |   |   |   |   | | └─Token(Identifier) |range|
//@[030:0031) |   |   |   |   | ├─Token(LeftParen) |(|
//@[031:0032) |   |   |   |   | ├─FunctionArgumentSyntax
//@[031:0032) |   |   |   |   | | └─IntegerLiteralSyntax
//@[031:0032) |   |   |   |   | |   └─Token(Integer) |1|
//@[032:0033) |   |   |   |   | ├─Token(Comma) |,|
//@[033:0034) |   |   |   |   | ├─FunctionArgumentSyntax
//@[033:0034) |   |   |   |   | | └─IntegerLiteralSyntax
//@[033:0034) |   |   |   |   | |   └─Token(Integer) |2|
//@[034:0035) |   |   |   |   | └─Token(RightParen) |)|
//@[035:0036) |   |   |   |   ├─Token(Colon) |:|
//@[037:0043) |   |   |   |   ├─BinaryOperationSyntax
//@[037:0038) |   |   |   |   | ├─VariableAccessSyntax
//@[037:0038) |   |   |   |   | | └─IdentifierSyntax
//@[037:0038) |   |   |   |   | |   └─Token(Identifier) |i|
//@[039:0040) |   |   |   |   | ├─Token(Slash) |/|
//@[041:0043) |   |   |   |   | └─VariableAccessSyntax
//@[041:0043) |   |   |   |   |   └─IdentifierSyntax
//@[041:0043) |   |   |   |   |     └─Token(Identifier) |i2|
//@[043:0044) |   |   |   |   └─Token(RightSquare) |]|
//@[044:0046) |   |   |   ├─Token(NewLine) |\r\n|
      c: {
//@[006:0067) |   |   |   ├─ObjectPropertySyntax
//@[006:0007) |   |   |   | ├─IdentifierSyntax
//@[006:0007) |   |   |   | | └─Token(Identifier) |c|
//@[007:0008) |   |   |   | ├─Token(Colon) |:|
//@[009:0067) |   |   |   | └─ObjectSyntax
//@[009:0010) |   |   |   |   ├─Token(LeftBrace) |{|
//@[010:0012) |   |   |   |   ├─Token(NewLine) |\r\n|
        d: [for (j, j2) in range(2,3): j * j2]
//@[008:0046) |   |   |   |   ├─ObjectPropertySyntax
//@[008:0009) |   |   |   |   | ├─IdentifierSyntax
//@[008:0009) |   |   |   |   | | └─Token(Identifier) |d|
//@[009:0010) |   |   |   |   | ├─Token(Colon) |:|
//@[011:0046) |   |   |   |   | └─ForSyntax
//@[011:0012) |   |   |   |   |   ├─Token(LeftSquare) |[|
//@[012:0015) |   |   |   |   |   ├─Token(Identifier) |for|
//@[016:0023) |   |   |   |   |   ├─VariableBlockSyntax
//@[016:0017) |   |   |   |   |   | ├─Token(LeftParen) |(|
//@[017:0018) |   |   |   |   |   | ├─LocalVariableSyntax
//@[017:0018) |   |   |   |   |   | | └─IdentifierSyntax
//@[017:0018) |   |   |   |   |   | |   └─Token(Identifier) |j|
//@[018:0019) |   |   |   |   |   | ├─Token(Comma) |,|
//@[020:0022) |   |   |   |   |   | ├─LocalVariableSyntax
//@[020:0022) |   |   |   |   |   | | └─IdentifierSyntax
//@[020:0022) |   |   |   |   |   | |   └─Token(Identifier) |j2|
//@[022:0023) |   |   |   |   |   | └─Token(RightParen) |)|
//@[024:0026) |   |   |   |   |   ├─Token(Identifier) |in|
//@[027:0037) |   |   |   |   |   ├─FunctionCallSyntax
//@[027:0032) |   |   |   |   |   | ├─IdentifierSyntax
//@[027:0032) |   |   |   |   |   | | └─Token(Identifier) |range|
//@[032:0033) |   |   |   |   |   | ├─Token(LeftParen) |(|
//@[033:0034) |   |   |   |   |   | ├─FunctionArgumentSyntax
//@[033:0034) |   |   |   |   |   | | └─IntegerLiteralSyntax
//@[033:0034) |   |   |   |   |   | |   └─Token(Integer) |2|
//@[034:0035) |   |   |   |   |   | ├─Token(Comma) |,|
//@[035:0036) |   |   |   |   |   | ├─FunctionArgumentSyntax
//@[035:0036) |   |   |   |   |   | | └─IntegerLiteralSyntax
//@[035:0036) |   |   |   |   |   | |   └─Token(Integer) |3|
//@[036:0037) |   |   |   |   |   | └─Token(RightParen) |)|
//@[037:0038) |   |   |   |   |   ├─Token(Colon) |:|
//@[039:0045) |   |   |   |   |   ├─BinaryOperationSyntax
//@[039:0040) |   |   |   |   |   | ├─VariableAccessSyntax
//@[039:0040) |   |   |   |   |   | | └─IdentifierSyntax
//@[039:0040) |   |   |   |   |   | |   └─Token(Identifier) |j|
//@[041:0042) |   |   |   |   |   | ├─Token(Asterisk) |*|
//@[043:0045) |   |   |   |   |   | └─VariableAccessSyntax
//@[043:0045) |   |   |   |   |   |   └─IdentifierSyntax
//@[043:0045) |   |   |   |   |   |     └─Token(Identifier) |j2|
//@[045:0046) |   |   |   |   |   └─Token(RightSquare) |]|
//@[046:0048) |   |   |   |   ├─Token(NewLine) |\r\n|
      }
//@[006:0007) |   |   |   |   └─Token(RightBrace) |}|
//@[007:0009) |   |   |   ├─Token(NewLine) |\r\n|
      e: [for (k, k2) in range(4,4): {
//@[006:0077) |   |   |   ├─ObjectPropertySyntax
//@[006:0007) |   |   |   | ├─IdentifierSyntax
//@[006:0007) |   |   |   | | └─Token(Identifier) |e|
//@[007:0008) |   |   |   | ├─Token(Colon) |:|
//@[009:0077) |   |   |   | └─ForSyntax
//@[009:0010) |   |   |   |   ├─Token(LeftSquare) |[|
//@[010:0013) |   |   |   |   ├─Token(Identifier) |for|
//@[014:0021) |   |   |   |   ├─VariableBlockSyntax
//@[014:0015) |   |   |   |   | ├─Token(LeftParen) |(|
//@[015:0016) |   |   |   |   | ├─LocalVariableSyntax
//@[015:0016) |   |   |   |   | | └─IdentifierSyntax
//@[015:0016) |   |   |   |   | |   └─Token(Identifier) |k|
//@[016:0017) |   |   |   |   | ├─Token(Comma) |,|
//@[018:0020) |   |   |   |   | ├─LocalVariableSyntax
//@[018:0020) |   |   |   |   | | └─IdentifierSyntax
//@[018:0020) |   |   |   |   | |   └─Token(Identifier) |k2|
//@[020:0021) |   |   |   |   | └─Token(RightParen) |)|
//@[022:0024) |   |   |   |   ├─Token(Identifier) |in|
//@[025:0035) |   |   |   |   ├─FunctionCallSyntax
//@[025:0030) |   |   |   |   | ├─IdentifierSyntax
//@[025:0030) |   |   |   |   | | └─Token(Identifier) |range|
//@[030:0031) |   |   |   |   | ├─Token(LeftParen) |(|
//@[031:0032) |   |   |   |   | ├─FunctionArgumentSyntax
//@[031:0032) |   |   |   |   | | └─IntegerLiteralSyntax
//@[031:0032) |   |   |   |   | |   └─Token(Integer) |4|
//@[032:0033) |   |   |   |   | ├─Token(Comma) |,|
//@[033:0034) |   |   |   |   | ├─FunctionArgumentSyntax
//@[033:0034) |   |   |   |   | | └─IntegerLiteralSyntax
//@[033:0034) |   |   |   |   | |   └─Token(Integer) |4|
//@[034:0035) |   |   |   |   | └─Token(RightParen) |)|
//@[035:0036) |   |   |   |   ├─Token(Colon) |:|
//@[037:0076) |   |   |   |   ├─ObjectSyntax
//@[037:0038) |   |   |   |   | ├─Token(LeftBrace) |{|
//@[038:0040) |   |   |   |   | ├─Token(NewLine) |\r\n|
        f: k
//@[008:0012) |   |   |   |   | ├─ObjectPropertySyntax
//@[008:0009) |   |   |   |   | | ├─IdentifierSyntax
//@[008:0009) |   |   |   |   | | | └─Token(Identifier) |f|
//@[009:0010) |   |   |   |   | | ├─Token(Colon) |:|
//@[011:0012) |   |   |   |   | | └─VariableAccessSyntax
//@[011:0012) |   |   |   |   | |   └─IdentifierSyntax
//@[011:0012) |   |   |   |   | |     └─Token(Identifier) |k|
//@[012:0014) |   |   |   |   | ├─Token(NewLine) |\r\n|
        g: k2
//@[008:0013) |   |   |   |   | ├─ObjectPropertySyntax
//@[008:0009) |   |   |   |   | | ├─IdentifierSyntax
//@[008:0009) |   |   |   |   | | | └─Token(Identifier) |g|
//@[009:0010) |   |   |   |   | | ├─Token(Colon) |:|
//@[011:0013) |   |   |   |   | | └─VariableAccessSyntax
//@[011:0013) |   |   |   |   | |   └─IdentifierSyntax
//@[011:0013) |   |   |   |   | |     └─Token(Identifier) |k2|
//@[013:0015) |   |   |   |   | ├─Token(NewLine) |\r\n|
      }]
//@[006:0007) |   |   |   |   | └─Token(RightBrace) |}|
//@[007:0008) |   |   |   |   └─Token(RightSquare) |]|
//@[008:0010) |   |   |   ├─Token(NewLine) |\r\n|
    }
//@[004:0005) |   |   |   └─Token(RightBrace) |}|
//@[005:0007) |   |   ├─Token(NewLine) |\r\n|
    stringParamB: ''
//@[004:0020) |   |   ├─ObjectPropertySyntax
//@[004:0016) |   |   | ├─IdentifierSyntax
//@[004:0016) |   |   | | └─Token(Identifier) |stringParamB|
//@[016:0017) |   |   | ├─Token(Colon) |:|
//@[018:0020) |   |   | └─StringSyntax
//@[018:0020) |   |   |   └─Token(StringComplete) |''|
//@[020:0022) |   |   ├─Token(NewLine) |\r\n|
    arrayParam: [
//@[004:0079) |   |   ├─ObjectPropertySyntax
//@[004:0014) |   |   | ├─IdentifierSyntax
//@[004:0014) |   |   | | └─Token(Identifier) |arrayParam|
//@[014:0015) |   |   | ├─Token(Colon) |:|
//@[016:0079) |   |   | └─ArraySyntax
//@[016:0017) |   |   |   ├─Token(LeftSquare) |[|
//@[017:0019) |   |   |   ├─Token(NewLine) |\r\n|
      {
//@[006:0053) |   |   |   ├─ArrayItemSyntax
//@[006:0053) |   |   |   | └─ObjectSyntax
//@[006:0007) |   |   |   |   ├─Token(LeftBrace) |{|
//@[007:0009) |   |   |   |   ├─Token(NewLine) |\r\n|
        e: [for j in range(7,7): j]
//@[008:0035) |   |   |   |   ├─ObjectPropertySyntax
//@[008:0009) |   |   |   |   | ├─IdentifierSyntax
//@[008:0009) |   |   |   |   | | └─Token(Identifier) |e|
//@[009:0010) |   |   |   |   | ├─Token(Colon) |:|
//@[011:0035) |   |   |   |   | └─ForSyntax
//@[011:0012) |   |   |   |   |   ├─Token(LeftSquare) |[|
//@[012:0015) |   |   |   |   |   ├─Token(Identifier) |for|
//@[016:0017) |   |   |   |   |   ├─LocalVariableSyntax
//@[016:0017) |   |   |   |   |   | └─IdentifierSyntax
//@[016:0017) |   |   |   |   |   |   └─Token(Identifier) |j|
//@[018:0020) |   |   |   |   |   ├─Token(Identifier) |in|
//@[021:0031) |   |   |   |   |   ├─FunctionCallSyntax
//@[021:0026) |   |   |   |   |   | ├─IdentifierSyntax
//@[021:0026) |   |   |   |   |   | | └─Token(Identifier) |range|
//@[026:0027) |   |   |   |   |   | ├─Token(LeftParen) |(|
//@[027:0028) |   |   |   |   |   | ├─FunctionArgumentSyntax
//@[027:0028) |   |   |   |   |   | | └─IntegerLiteralSyntax
//@[027:0028) |   |   |   |   |   | |   └─Token(Integer) |7|
//@[028:0029) |   |   |   |   |   | ├─Token(Comma) |,|
//@[029:0030) |   |   |   |   |   | ├─FunctionArgumentSyntax
//@[029:0030) |   |   |   |   |   | | └─IntegerLiteralSyntax
//@[029:0030) |   |   |   |   |   | |   └─Token(Integer) |7|
//@[030:0031) |   |   |   |   |   | └─Token(RightParen) |)|
//@[031:0032) |   |   |   |   |   ├─Token(Colon) |:|
//@[033:0034) |   |   |   |   |   ├─VariableAccessSyntax
//@[033:0034) |   |   |   |   |   | └─IdentifierSyntax
//@[033:0034) |   |   |   |   |   |   └─Token(Identifier) |j|
//@[034:0035) |   |   |   |   |   └─Token(RightSquare) |]|
//@[035:0037) |   |   |   |   ├─Token(NewLine) |\r\n|
      }
//@[006:0007) |   |   |   |   └─Token(RightBrace) |}|
//@[007:0009) |   |   |   ├─Token(NewLine) |\r\n|
    ]
//@[004:0005) |   |   |   └─Token(RightSquare) |]|
//@[005:0007) |   |   ├─Token(NewLine) |\r\n|
  }
//@[002:0003) |   |   └─Token(RightBrace) |}|
//@[003:0005) |   ├─Token(NewLine) |\r\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0005) ├─Token(NewLine) |\r\n\r\n|

module propertyLoopInsideParameterValueInsideModuleLoop 'modulea.bicep' = [for thing in range(0,1): {
//@[000:0529) ├─ModuleDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |module|
//@[007:0055) | ├─IdentifierSyntax
//@[007:0055) | | └─Token(Identifier) |propertyLoopInsideParameterValueInsideModuleLoop|
//@[056:0071) | ├─StringSyntax
//@[056:0071) | | └─Token(StringComplete) |'modulea.bicep'|
//@[072:0073) | ├─Token(Assignment) |=|
//@[074:0529) | └─ForSyntax
//@[074:0075) |   ├─Token(LeftSquare) |[|
//@[075:0078) |   ├─Token(Identifier) |for|
//@[079:0084) |   ├─LocalVariableSyntax
//@[079:0084) |   | └─IdentifierSyntax
//@[079:0084) |   |   └─Token(Identifier) |thing|
//@[085:0087) |   ├─Token(Identifier) |in|
//@[088:0098) |   ├─FunctionCallSyntax
//@[088:0093) |   | ├─IdentifierSyntax
//@[088:0093) |   | | └─Token(Identifier) |range|
//@[093:0094) |   | ├─Token(LeftParen) |(|
//@[094:0095) |   | ├─FunctionArgumentSyntax
//@[094:0095) |   | | └─IntegerLiteralSyntax
//@[094:0095) |   | |   └─Token(Integer) |0|
//@[095:0096) |   | ├─Token(Comma) |,|
//@[096:0097) |   | ├─FunctionArgumentSyntax
//@[096:0097) |   | | └─IntegerLiteralSyntax
//@[096:0097) |   | |   └─Token(Integer) |1|
//@[097:0098) |   | └─Token(RightParen) |)|
//@[098:0099) |   ├─Token(Colon) |:|
//@[100:0528) |   ├─ObjectSyntax
//@[100:0101) |   | ├─Token(LeftBrace) |{|
//@[101:0103) |   | ├─Token(NewLine) |\r\n|
  name: 'propertyLoopInsideParameterValueInsideModuleLoop'
//@[002:0058) |   | ├─ObjectPropertySyntax
//@[002:0006) |   | | ├─IdentifierSyntax
//@[002:0006) |   | | | └─Token(Identifier) |name|
//@[006:0007) |   | | ├─Token(Colon) |:|
//@[008:0058) |   | | └─StringSyntax
//@[008:0058) |   | |   └─Token(StringComplete) |'propertyLoopInsideParameterValueInsideModuleLoop'|
//@[058:0060) |   | ├─Token(NewLine) |\r\n|
  params: {
//@[002:0362) |   | ├─ObjectPropertySyntax
//@[002:0008) |   | | ├─IdentifierSyntax
//@[002:0008) |   | | | └─Token(Identifier) |params|
//@[008:0009) |   | | ├─Token(Colon) |:|
//@[010:0362) |   | | └─ObjectSyntax
//@[010:0011) |   | |   ├─Token(LeftBrace) |{|
//@[011:0013) |   | |   ├─Token(NewLine) |\r\n|
    objParam: {
//@[004:0233) |   | |   ├─ObjectPropertySyntax
//@[004:0012) |   | |   | ├─IdentifierSyntax
//@[004:0012) |   | |   | | └─Token(Identifier) |objParam|
//@[012:0013) |   | |   | ├─Token(Colon) |:|
//@[014:0233) |   | |   | └─ObjectSyntax
//@[014:0015) |   | |   |   ├─Token(LeftBrace) |{|
//@[015:0017) |   | |   |   ├─Token(NewLine) |\r\n|
      a: [for i in range(0,10): i + thing]
//@[006:0042) |   | |   |   ├─ObjectPropertySyntax
//@[006:0007) |   | |   |   | ├─IdentifierSyntax
//@[006:0007) |   | |   |   | | └─Token(Identifier) |a|
//@[007:0008) |   | |   |   | ├─Token(Colon) |:|
//@[009:0042) |   | |   |   | └─ForSyntax
//@[009:0010) |   | |   |   |   ├─Token(LeftSquare) |[|
//@[010:0013) |   | |   |   |   ├─Token(Identifier) |for|
//@[014:0015) |   | |   |   |   ├─LocalVariableSyntax
//@[014:0015) |   | |   |   |   | └─IdentifierSyntax
//@[014:0015) |   | |   |   |   |   └─Token(Identifier) |i|
//@[016:0018) |   | |   |   |   ├─Token(Identifier) |in|
//@[019:0030) |   | |   |   |   ├─FunctionCallSyntax
//@[019:0024) |   | |   |   |   | ├─IdentifierSyntax
//@[019:0024) |   | |   |   |   | | └─Token(Identifier) |range|
//@[024:0025) |   | |   |   |   | ├─Token(LeftParen) |(|
//@[025:0026) |   | |   |   |   | ├─FunctionArgumentSyntax
//@[025:0026) |   | |   |   |   | | └─IntegerLiteralSyntax
//@[025:0026) |   | |   |   |   | |   └─Token(Integer) |0|
//@[026:0027) |   | |   |   |   | ├─Token(Comma) |,|
//@[027:0029) |   | |   |   |   | ├─FunctionArgumentSyntax
//@[027:0029) |   | |   |   |   | | └─IntegerLiteralSyntax
//@[027:0029) |   | |   |   |   | |   └─Token(Integer) |10|
//@[029:0030) |   | |   |   |   | └─Token(RightParen) |)|
//@[030:0031) |   | |   |   |   ├─Token(Colon) |:|
//@[032:0041) |   | |   |   |   ├─BinaryOperationSyntax
//@[032:0033) |   | |   |   |   | ├─VariableAccessSyntax
//@[032:0033) |   | |   |   |   | | └─IdentifierSyntax
//@[032:0033) |   | |   |   |   | |   └─Token(Identifier) |i|
//@[034:0035) |   | |   |   |   | ├─Token(Plus) |+|
//@[036:0041) |   | |   |   |   | └─VariableAccessSyntax
//@[036:0041) |   | |   |   |   |   └─IdentifierSyntax
//@[036:0041) |   | |   |   |   |     └─Token(Identifier) |thing|
//@[041:0042) |   | |   |   |   └─Token(RightSquare) |]|
//@[042:0044) |   | |   |   ├─Token(NewLine) |\r\n|
      b: [for i in range(1,2): i * thing]
//@[006:0041) |   | |   |   ├─ObjectPropertySyntax
//@[006:0007) |   | |   |   | ├─IdentifierSyntax
//@[006:0007) |   | |   |   | | └─Token(Identifier) |b|
//@[007:0008) |   | |   |   | ├─Token(Colon) |:|
//@[009:0041) |   | |   |   | └─ForSyntax
//@[009:0010) |   | |   |   |   ├─Token(LeftSquare) |[|
//@[010:0013) |   | |   |   |   ├─Token(Identifier) |for|
//@[014:0015) |   | |   |   |   ├─LocalVariableSyntax
//@[014:0015) |   | |   |   |   | └─IdentifierSyntax
//@[014:0015) |   | |   |   |   |   └─Token(Identifier) |i|
//@[016:0018) |   | |   |   |   ├─Token(Identifier) |in|
//@[019:0029) |   | |   |   |   ├─FunctionCallSyntax
//@[019:0024) |   | |   |   |   | ├─IdentifierSyntax
//@[019:0024) |   | |   |   |   | | └─Token(Identifier) |range|
//@[024:0025) |   | |   |   |   | ├─Token(LeftParen) |(|
//@[025:0026) |   | |   |   |   | ├─FunctionArgumentSyntax
//@[025:0026) |   | |   |   |   | | └─IntegerLiteralSyntax
//@[025:0026) |   | |   |   |   | |   └─Token(Integer) |1|
//@[026:0027) |   | |   |   |   | ├─Token(Comma) |,|
//@[027:0028) |   | |   |   |   | ├─FunctionArgumentSyntax
//@[027:0028) |   | |   |   |   | | └─IntegerLiteralSyntax
//@[027:0028) |   | |   |   |   | |   └─Token(Integer) |2|
//@[028:0029) |   | |   |   |   | └─Token(RightParen) |)|
//@[029:0030) |   | |   |   |   ├─Token(Colon) |:|
//@[031:0040) |   | |   |   |   ├─BinaryOperationSyntax
//@[031:0032) |   | |   |   |   | ├─VariableAccessSyntax
//@[031:0032) |   | |   |   |   | | └─IdentifierSyntax
//@[031:0032) |   | |   |   |   | |   └─Token(Identifier) |i|
//@[033:0034) |   | |   |   |   | ├─Token(Asterisk) |*|
//@[035:0040) |   | |   |   |   | └─VariableAccessSyntax
//@[035:0040) |   | |   |   |   |   └─IdentifierSyntax
//@[035:0040) |   | |   |   |   |     └─Token(Identifier) |thing|
//@[040:0041) |   | |   |   |   └─Token(RightSquare) |]|
//@[041:0043) |   | |   |   ├─Token(NewLine) |\r\n|
      c: {
//@[006:0056) |   | |   |   ├─ObjectPropertySyntax
//@[006:0007) |   | |   |   | ├─IdentifierSyntax
//@[006:0007) |   | |   |   | | └─Token(Identifier) |c|
//@[007:0008) |   | |   |   | ├─Token(Colon) |:|
//@[009:0056) |   | |   |   | └─ObjectSyntax
//@[009:0010) |   | |   |   |   ├─Token(LeftBrace) |{|
//@[010:0012) |   | |   |   |   ├─Token(NewLine) |\r\n|
        d: [for j in range(2,3): j]
//@[008:0035) |   | |   |   |   ├─ObjectPropertySyntax
//@[008:0009) |   | |   |   |   | ├─IdentifierSyntax
//@[008:0009) |   | |   |   |   | | └─Token(Identifier) |d|
//@[009:0010) |   | |   |   |   | ├─Token(Colon) |:|
//@[011:0035) |   | |   |   |   | └─ForSyntax
//@[011:0012) |   | |   |   |   |   ├─Token(LeftSquare) |[|
//@[012:0015) |   | |   |   |   |   ├─Token(Identifier) |for|
//@[016:0017) |   | |   |   |   |   ├─LocalVariableSyntax
//@[016:0017) |   | |   |   |   |   | └─IdentifierSyntax
//@[016:0017) |   | |   |   |   |   |   └─Token(Identifier) |j|
//@[018:0020) |   | |   |   |   |   ├─Token(Identifier) |in|
//@[021:0031) |   | |   |   |   |   ├─FunctionCallSyntax
//@[021:0026) |   | |   |   |   |   | ├─IdentifierSyntax
//@[021:0026) |   | |   |   |   |   | | └─Token(Identifier) |range|
//@[026:0027) |   | |   |   |   |   | ├─Token(LeftParen) |(|
//@[027:0028) |   | |   |   |   |   | ├─FunctionArgumentSyntax
//@[027:0028) |   | |   |   |   |   | | └─IntegerLiteralSyntax
//@[027:0028) |   | |   |   |   |   | |   └─Token(Integer) |2|
//@[028:0029) |   | |   |   |   |   | ├─Token(Comma) |,|
//@[029:0030) |   | |   |   |   |   | ├─FunctionArgumentSyntax
//@[029:0030) |   | |   |   |   |   | | └─IntegerLiteralSyntax
//@[029:0030) |   | |   |   |   |   | |   └─Token(Integer) |3|
//@[030:0031) |   | |   |   |   |   | └─Token(RightParen) |)|
//@[031:0032) |   | |   |   |   |   ├─Token(Colon) |:|
//@[033:0034) |   | |   |   |   |   ├─VariableAccessSyntax
//@[033:0034) |   | |   |   |   |   | └─IdentifierSyntax
//@[033:0034) |   | |   |   |   |   |   └─Token(Identifier) |j|
//@[034:0035) |   | |   |   |   |   └─Token(RightSquare) |]|
//@[035:0037) |   | |   |   |   ├─Token(NewLine) |\r\n|
      }
//@[006:0007) |   | |   |   |   └─Token(RightBrace) |}|
//@[007:0009) |   | |   |   ├─Token(NewLine) |\r\n|
      e: [for k in range(4,4): {
//@[006:0064) |   | |   |   ├─ObjectPropertySyntax
//@[006:0007) |   | |   |   | ├─IdentifierSyntax
//@[006:0007) |   | |   |   | | └─Token(Identifier) |e|
//@[007:0008) |   | |   |   | ├─Token(Colon) |:|
//@[009:0064) |   | |   |   | └─ForSyntax
//@[009:0010) |   | |   |   |   ├─Token(LeftSquare) |[|
//@[010:0013) |   | |   |   |   ├─Token(Identifier) |for|
//@[014:0015) |   | |   |   |   ├─LocalVariableSyntax
//@[014:0015) |   | |   |   |   | └─IdentifierSyntax
//@[014:0015) |   | |   |   |   |   └─Token(Identifier) |k|
//@[016:0018) |   | |   |   |   ├─Token(Identifier) |in|
//@[019:0029) |   | |   |   |   ├─FunctionCallSyntax
//@[019:0024) |   | |   |   |   | ├─IdentifierSyntax
//@[019:0024) |   | |   |   |   | | └─Token(Identifier) |range|
//@[024:0025) |   | |   |   |   | ├─Token(LeftParen) |(|
//@[025:0026) |   | |   |   |   | ├─FunctionArgumentSyntax
//@[025:0026) |   | |   |   |   | | └─IntegerLiteralSyntax
//@[025:0026) |   | |   |   |   | |   └─Token(Integer) |4|
//@[026:0027) |   | |   |   |   | ├─Token(Comma) |,|
//@[027:0028) |   | |   |   |   | ├─FunctionArgumentSyntax
//@[027:0028) |   | |   |   |   | | └─IntegerLiteralSyntax
//@[027:0028) |   | |   |   |   | |   └─Token(Integer) |4|
//@[028:0029) |   | |   |   |   | └─Token(RightParen) |)|
//@[029:0030) |   | |   |   |   ├─Token(Colon) |:|
//@[031:0063) |   | |   |   |   ├─ObjectSyntax
//@[031:0032) |   | |   |   |   | ├─Token(LeftBrace) |{|
//@[032:0034) |   | |   |   |   | ├─Token(NewLine) |\r\n|
        f: k - thing
//@[008:0020) |   | |   |   |   | ├─ObjectPropertySyntax
//@[008:0009) |   | |   |   |   | | ├─IdentifierSyntax
//@[008:0009) |   | |   |   |   | | | └─Token(Identifier) |f|
//@[009:0010) |   | |   |   |   | | ├─Token(Colon) |:|
//@[011:0020) |   | |   |   |   | | └─BinaryOperationSyntax
//@[011:0012) |   | |   |   |   | |   ├─VariableAccessSyntax
//@[011:0012) |   | |   |   |   | |   | └─IdentifierSyntax
//@[011:0012) |   | |   |   |   | |   |   └─Token(Identifier) |k|
//@[013:0014) |   | |   |   |   | |   ├─Token(Minus) |-|
//@[015:0020) |   | |   |   |   | |   └─VariableAccessSyntax
//@[015:0020) |   | |   |   |   | |     └─IdentifierSyntax
//@[015:0020) |   | |   |   |   | |       └─Token(Identifier) |thing|
//@[020:0022) |   | |   |   |   | ├─Token(NewLine) |\r\n|
      }]
//@[006:0007) |   | |   |   |   | └─Token(RightBrace) |}|
//@[007:0008) |   | |   |   |   └─Token(RightSquare) |]|
//@[008:0010) |   | |   |   ├─Token(NewLine) |\r\n|
    }
//@[004:0005) |   | |   |   └─Token(RightBrace) |}|
//@[005:0007) |   | |   ├─Token(NewLine) |\r\n|
    stringParamB: ''
//@[004:0020) |   | |   ├─ObjectPropertySyntax
//@[004:0016) |   | |   | ├─IdentifierSyntax
//@[004:0016) |   | |   | | └─Token(Identifier) |stringParamB|
//@[016:0017) |   | |   | ├─Token(Colon) |:|
//@[018:0020) |   | |   | └─StringSyntax
//@[018:0020) |   | |   |   └─Token(StringComplete) |''|
//@[020:0022) |   | |   ├─Token(NewLine) |\r\n|
    arrayParam: [
//@[004:0087) |   | |   ├─ObjectPropertySyntax
//@[004:0014) |   | |   | ├─IdentifierSyntax
//@[004:0014) |   | |   | | └─Token(Identifier) |arrayParam|
//@[014:0015) |   | |   | ├─Token(Colon) |:|
//@[016:0087) |   | |   | └─ArraySyntax
//@[016:0017) |   | |   |   ├─Token(LeftSquare) |[|
//@[017:0019) |   | |   |   ├─Token(NewLine) |\r\n|
      {
//@[006:0061) |   | |   |   ├─ArrayItemSyntax
//@[006:0061) |   | |   |   | └─ObjectSyntax
//@[006:0007) |   | |   |   |   ├─Token(LeftBrace) |{|
//@[007:0009) |   | |   |   |   ├─Token(NewLine) |\r\n|
        e: [for j in range(7,7): j % thing]
//@[008:0043) |   | |   |   |   ├─ObjectPropertySyntax
//@[008:0009) |   | |   |   |   | ├─IdentifierSyntax
//@[008:0009) |   | |   |   |   | | └─Token(Identifier) |e|
//@[009:0010) |   | |   |   |   | ├─Token(Colon) |:|
//@[011:0043) |   | |   |   |   | └─ForSyntax
//@[011:0012) |   | |   |   |   |   ├─Token(LeftSquare) |[|
//@[012:0015) |   | |   |   |   |   ├─Token(Identifier) |for|
//@[016:0017) |   | |   |   |   |   ├─LocalVariableSyntax
//@[016:0017) |   | |   |   |   |   | └─IdentifierSyntax
//@[016:0017) |   | |   |   |   |   |   └─Token(Identifier) |j|
//@[018:0020) |   | |   |   |   |   ├─Token(Identifier) |in|
//@[021:0031) |   | |   |   |   |   ├─FunctionCallSyntax
//@[021:0026) |   | |   |   |   |   | ├─IdentifierSyntax
//@[021:0026) |   | |   |   |   |   | | └─Token(Identifier) |range|
//@[026:0027) |   | |   |   |   |   | ├─Token(LeftParen) |(|
//@[027:0028) |   | |   |   |   |   | ├─FunctionArgumentSyntax
//@[027:0028) |   | |   |   |   |   | | └─IntegerLiteralSyntax
//@[027:0028) |   | |   |   |   |   | |   └─Token(Integer) |7|
//@[028:0029) |   | |   |   |   |   | ├─Token(Comma) |,|
//@[029:0030) |   | |   |   |   |   | ├─FunctionArgumentSyntax
//@[029:0030) |   | |   |   |   |   | | └─IntegerLiteralSyntax
//@[029:0030) |   | |   |   |   |   | |   └─Token(Integer) |7|
//@[030:0031) |   | |   |   |   |   | └─Token(RightParen) |)|
//@[031:0032) |   | |   |   |   |   ├─Token(Colon) |:|
//@[033:0042) |   | |   |   |   |   ├─BinaryOperationSyntax
//@[033:0034) |   | |   |   |   |   | ├─VariableAccessSyntax
//@[033:0034) |   | |   |   |   |   | | └─IdentifierSyntax
//@[033:0034) |   | |   |   |   |   | |   └─Token(Identifier) |j|
//@[035:0036) |   | |   |   |   |   | ├─Token(Modulo) |%|
//@[037:0042) |   | |   |   |   |   | └─VariableAccessSyntax
//@[037:0042) |   | |   |   |   |   |   └─IdentifierSyntax
//@[037:0042) |   | |   |   |   |   |     └─Token(Identifier) |thing|
//@[042:0043) |   | |   |   |   |   └─Token(RightSquare) |]|
//@[043:0045) |   | |   |   |   ├─Token(NewLine) |\r\n|
      }
//@[006:0007) |   | |   |   |   └─Token(RightBrace) |}|
//@[007:0009) |   | |   |   ├─Token(NewLine) |\r\n|
    ]
//@[004:0005) |   | |   |   └─Token(RightSquare) |]|
//@[005:0007) |   | |   ├─Token(NewLine) |\r\n|
  }
//@[002:0003) |   | |   └─Token(RightBrace) |}|
//@[003:0005) |   | ├─Token(NewLine) |\r\n|
}]
//@[000:0001) |   | └─Token(RightBrace) |}|
//@[001:0002) |   └─Token(RightSquare) |]|
//@[002:0008) ├─Token(NewLine) |\r\n\r\n\r\n|


// BEGIN: Key Vault Secret Reference
//@[036:0040) ├─Token(NewLine) |\r\n\r\n|

resource kv 'Microsoft.KeyVault/vaults@2019-09-01' existing = {
//@[000:0090) ├─ResourceDeclarationSyntax
//@[000:0008) | ├─Token(Identifier) |resource|
//@[009:0011) | ├─IdentifierSyntax
//@[009:0011) | | └─Token(Identifier) |kv|
//@[012:0050) | ├─StringSyntax
//@[012:0050) | | └─Token(StringComplete) |'Microsoft.KeyVault/vaults@2019-09-01'|
//@[051:0059) | ├─Token(Identifier) |existing|
//@[060:0061) | ├─Token(Assignment) |=|
//@[062:0090) | └─ObjectSyntax
//@[062:0063) |   ├─Token(LeftBrace) |{|
//@[063:0065) |   ├─Token(NewLine) |\r\n|
  name: 'testkeyvault'
//@[002:0022) |   ├─ObjectPropertySyntax
//@[002:0006) |   | ├─IdentifierSyntax
//@[002:0006) |   | | └─Token(Identifier) |name|
//@[006:0007) |   | ├─Token(Colon) |:|
//@[008:0022) |   | └─StringSyntax
//@[008:0022) |   |   └─Token(StringComplete) |'testkeyvault'|
//@[022:0024) |   ├─Token(NewLine) |\r\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0005) ├─Token(NewLine) |\r\n\r\n|

module secureModule1 'child/secureParams.bicep' = {
//@[000:0213) ├─ModuleDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |module|
//@[007:0020) | ├─IdentifierSyntax
//@[007:0020) | | └─Token(Identifier) |secureModule1|
//@[021:0047) | ├─StringSyntax
//@[021:0047) | | └─Token(StringComplete) |'child/secureParams.bicep'|
//@[048:0049) | ├─Token(Assignment) |=|
//@[050:0213) | └─ObjectSyntax
//@[050:0051) |   ├─Token(LeftBrace) |{|
//@[051:0053) |   ├─Token(NewLine) |\r\n|
  name: 'secureModule1'
//@[002:0023) |   ├─ObjectPropertySyntax
//@[002:0006) |   | ├─IdentifierSyntax
//@[002:0006) |   | | └─Token(Identifier) |name|
//@[006:0007) |   | ├─Token(Colon) |:|
//@[008:0023) |   | └─StringSyntax
//@[008:0023) |   |   └─Token(StringComplete) |'secureModule1'|
//@[023:0025) |   ├─Token(NewLine) |\r\n|
  params: {
//@[002:0132) |   ├─ObjectPropertySyntax
//@[002:0008) |   | ├─IdentifierSyntax
//@[002:0008) |   | | └─Token(Identifier) |params|
//@[008:0009) |   | ├─Token(Colon) |:|
//@[010:0132) |   | └─ObjectSyntax
//@[010:0011) |   |   ├─Token(LeftBrace) |{|
//@[011:0013) |   |   ├─Token(NewLine) |\r\n|
    secureStringParam1: kv.getSecret('mySecret')
//@[004:0048) |   |   ├─ObjectPropertySyntax
//@[004:0022) |   |   | ├─IdentifierSyntax
//@[004:0022) |   |   | | └─Token(Identifier) |secureStringParam1|
//@[022:0023) |   |   | ├─Token(Colon) |:|
//@[024:0048) |   |   | └─InstanceFunctionCallSyntax
//@[024:0026) |   |   |   ├─VariableAccessSyntax
//@[024:0026) |   |   |   | └─IdentifierSyntax
//@[024:0026) |   |   |   |   └─Token(Identifier) |kv|
//@[026:0027) |   |   |   ├─Token(Dot) |.|
//@[027:0036) |   |   |   ├─IdentifierSyntax
//@[027:0036) |   |   |   | └─Token(Identifier) |getSecret|
//@[036:0037) |   |   |   ├─Token(LeftParen) |(|
//@[037:0047) |   |   |   ├─FunctionArgumentSyntax
//@[037:0047) |   |   |   | └─StringSyntax
//@[037:0047) |   |   |   |   └─Token(StringComplete) |'mySecret'|
//@[047:0048) |   |   |   └─Token(RightParen) |)|
//@[048:0050) |   |   ├─Token(NewLine) |\r\n|
    secureStringParam2: kv.getSecret('mySecret','secretVersion')
//@[004:0064) |   |   ├─ObjectPropertySyntax
//@[004:0022) |   |   | ├─IdentifierSyntax
//@[004:0022) |   |   | | └─Token(Identifier) |secureStringParam2|
//@[022:0023) |   |   | ├─Token(Colon) |:|
//@[024:0064) |   |   | └─InstanceFunctionCallSyntax
//@[024:0026) |   |   |   ├─VariableAccessSyntax
//@[024:0026) |   |   |   | └─IdentifierSyntax
//@[024:0026) |   |   |   |   └─Token(Identifier) |kv|
//@[026:0027) |   |   |   ├─Token(Dot) |.|
//@[027:0036) |   |   |   ├─IdentifierSyntax
//@[027:0036) |   |   |   | └─Token(Identifier) |getSecret|
//@[036:0037) |   |   |   ├─Token(LeftParen) |(|
//@[037:0047) |   |   |   ├─FunctionArgumentSyntax
//@[037:0047) |   |   |   | └─StringSyntax
//@[037:0047) |   |   |   |   └─Token(StringComplete) |'mySecret'|
//@[047:0048) |   |   |   ├─Token(Comma) |,|
//@[048:0063) |   |   |   ├─FunctionArgumentSyntax
//@[048:0063) |   |   |   | └─StringSyntax
//@[048:0063) |   |   |   |   └─Token(StringComplete) |'secretVersion'|
//@[063:0064) |   |   |   └─Token(RightParen) |)|
//@[064:0066) |   |   ├─Token(NewLine) |\r\n|
  }
//@[002:0003) |   |   └─Token(RightBrace) |}|
//@[003:0005) |   ├─Token(NewLine) |\r\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0005) ├─Token(NewLine) |\r\n\r\n|

resource scopedKv 'Microsoft.KeyVault/vaults@2019-09-01' existing = {
//@[000:0134) ├─ResourceDeclarationSyntax
//@[000:0008) | ├─Token(Identifier) |resource|
//@[009:0017) | ├─IdentifierSyntax
//@[009:0017) | | └─Token(Identifier) |scopedKv|
//@[018:0056) | ├─StringSyntax
//@[018:0056) | | └─Token(StringComplete) |'Microsoft.KeyVault/vaults@2019-09-01'|
//@[057:0065) | ├─Token(Identifier) |existing|
//@[066:0067) | ├─Token(Assignment) |=|
//@[068:0134) | └─ObjectSyntax
//@[068:0069) |   ├─Token(LeftBrace) |{|
//@[069:0071) |   ├─Token(NewLine) |\r\n|
  name: 'testkeyvault'
//@[002:0022) |   ├─ObjectPropertySyntax
//@[002:0006) |   | ├─IdentifierSyntax
//@[002:0006) |   | | └─Token(Identifier) |name|
//@[006:0007) |   | ├─Token(Colon) |:|
//@[008:0022) |   | └─StringSyntax
//@[008:0022) |   |   └─Token(StringComplete) |'testkeyvault'|
//@[022:0024) |   ├─Token(NewLine) |\r\n|
  scope: resourceGroup('otherGroup')
//@[002:0036) |   ├─ObjectPropertySyntax
//@[002:0007) |   | ├─IdentifierSyntax
//@[002:0007) |   | | └─Token(Identifier) |scope|
//@[007:0008) |   | ├─Token(Colon) |:|
//@[009:0036) |   | └─FunctionCallSyntax
//@[009:0022) |   |   ├─IdentifierSyntax
//@[009:0022) |   |   | └─Token(Identifier) |resourceGroup|
//@[022:0023) |   |   ├─Token(LeftParen) |(|
//@[023:0035) |   |   ├─FunctionArgumentSyntax
//@[023:0035) |   |   | └─StringSyntax
//@[023:0035) |   |   |   └─Token(StringComplete) |'otherGroup'|
//@[035:0036) |   |   └─Token(RightParen) |)|
//@[036:0038) |   ├─Token(NewLine) |\r\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0005) ├─Token(NewLine) |\r\n\r\n|

module secureModule2 'child/secureParams.bicep' = {
//@[000:0225) ├─ModuleDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |module|
//@[007:0020) | ├─IdentifierSyntax
//@[007:0020) | | └─Token(Identifier) |secureModule2|
//@[021:0047) | ├─StringSyntax
//@[021:0047) | | └─Token(StringComplete) |'child/secureParams.bicep'|
//@[048:0049) | ├─Token(Assignment) |=|
//@[050:0225) | └─ObjectSyntax
//@[050:0051) |   ├─Token(LeftBrace) |{|
//@[051:0053) |   ├─Token(NewLine) |\r\n|
  name: 'secureModule2'
//@[002:0023) |   ├─ObjectPropertySyntax
//@[002:0006) |   | ├─IdentifierSyntax
//@[002:0006) |   | | └─Token(Identifier) |name|
//@[006:0007) |   | ├─Token(Colon) |:|
//@[008:0023) |   | └─StringSyntax
//@[008:0023) |   |   └─Token(StringComplete) |'secureModule2'|
//@[023:0025) |   ├─Token(NewLine) |\r\n|
  params: {
//@[002:0144) |   ├─ObjectPropertySyntax
//@[002:0008) |   | ├─IdentifierSyntax
//@[002:0008) |   | | └─Token(Identifier) |params|
//@[008:0009) |   | ├─Token(Colon) |:|
//@[010:0144) |   | └─ObjectSyntax
//@[010:0011) |   |   ├─Token(LeftBrace) |{|
//@[011:0013) |   |   ├─Token(NewLine) |\r\n|
    secureStringParam1: scopedKv.getSecret('mySecret')
//@[004:0054) |   |   ├─ObjectPropertySyntax
//@[004:0022) |   |   | ├─IdentifierSyntax
//@[004:0022) |   |   | | └─Token(Identifier) |secureStringParam1|
//@[022:0023) |   |   | ├─Token(Colon) |:|
//@[024:0054) |   |   | └─InstanceFunctionCallSyntax
//@[024:0032) |   |   |   ├─VariableAccessSyntax
//@[024:0032) |   |   |   | └─IdentifierSyntax
//@[024:0032) |   |   |   |   └─Token(Identifier) |scopedKv|
//@[032:0033) |   |   |   ├─Token(Dot) |.|
//@[033:0042) |   |   |   ├─IdentifierSyntax
//@[033:0042) |   |   |   | └─Token(Identifier) |getSecret|
//@[042:0043) |   |   |   ├─Token(LeftParen) |(|
//@[043:0053) |   |   |   ├─FunctionArgumentSyntax
//@[043:0053) |   |   |   | └─StringSyntax
//@[043:0053) |   |   |   |   └─Token(StringComplete) |'mySecret'|
//@[053:0054) |   |   |   └─Token(RightParen) |)|
//@[054:0056) |   |   ├─Token(NewLine) |\r\n|
    secureStringParam2: scopedKv.getSecret('mySecret','secretVersion')
//@[004:0070) |   |   ├─ObjectPropertySyntax
//@[004:0022) |   |   | ├─IdentifierSyntax
//@[004:0022) |   |   | | └─Token(Identifier) |secureStringParam2|
//@[022:0023) |   |   | ├─Token(Colon) |:|
//@[024:0070) |   |   | └─InstanceFunctionCallSyntax
//@[024:0032) |   |   |   ├─VariableAccessSyntax
//@[024:0032) |   |   |   | └─IdentifierSyntax
//@[024:0032) |   |   |   |   └─Token(Identifier) |scopedKv|
//@[032:0033) |   |   |   ├─Token(Dot) |.|
//@[033:0042) |   |   |   ├─IdentifierSyntax
//@[033:0042) |   |   |   | └─Token(Identifier) |getSecret|
//@[042:0043) |   |   |   ├─Token(LeftParen) |(|
//@[043:0053) |   |   |   ├─FunctionArgumentSyntax
//@[043:0053) |   |   |   | └─StringSyntax
//@[043:0053) |   |   |   |   └─Token(StringComplete) |'mySecret'|
//@[053:0054) |   |   |   ├─Token(Comma) |,|
//@[054:0069) |   |   |   ├─FunctionArgumentSyntax
//@[054:0069) |   |   |   | └─StringSyntax
//@[054:0069) |   |   |   |   └─Token(StringComplete) |'secretVersion'|
//@[069:0070) |   |   |   └─Token(RightParen) |)|
//@[070:0072) |   |   ├─Token(NewLine) |\r\n|
  }
//@[002:0003) |   |   └─Token(RightBrace) |}|
//@[003:0005) |   ├─Token(NewLine) |\r\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0005) ├─Token(NewLine) |\r\n\r\n|

//looped module with looped existing resource (Issue #2862)
//@[059:0061) ├─Token(NewLine) |\r\n|
var vaults = [
//@[000:0200) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0010) | ├─IdentifierSyntax
//@[004:0010) | | └─Token(Identifier) |vaults|
//@[011:0012) | ├─Token(Assignment) |=|
//@[013:0200) | └─ArraySyntax
//@[013:0014) |   ├─Token(LeftSquare) |[|
//@[014:0016) |   ├─Token(NewLine) |\r\n|
  {
//@[002:0089) |   ├─ArrayItemSyntax
//@[002:0089) |   | └─ObjectSyntax
//@[002:0003) |   |   ├─Token(LeftBrace) |{|
//@[003:0005) |   |   ├─Token(NewLine) |\r\n|
    vaultName: 'test-1-kv'
//@[004:0026) |   |   ├─ObjectPropertySyntax
//@[004:0013) |   |   | ├─IdentifierSyntax
//@[004:0013) |   |   | | └─Token(Identifier) |vaultName|
//@[013:0014) |   |   | ├─Token(Colon) |:|
//@[015:0026) |   |   | └─StringSyntax
//@[015:0026) |   |   |   └─Token(StringComplete) |'test-1-kv'|
//@[026:0028) |   |   ├─Token(NewLine) |\r\n|
    vaultRG: 'test-1-rg'
//@[004:0024) |   |   ├─ObjectPropertySyntax
//@[004:0011) |   |   | ├─IdentifierSyntax
//@[004:0011) |   |   | | └─Token(Identifier) |vaultRG|
//@[011:0012) |   |   | ├─Token(Colon) |:|
//@[013:0024) |   |   | └─StringSyntax
//@[013:0024) |   |   |   └─Token(StringComplete) |'test-1-rg'|
//@[024:0026) |   |   ├─Token(NewLine) |\r\n|
    vaultSub: 'abcd-efgh'
//@[004:0025) |   |   ├─ObjectPropertySyntax
//@[004:0012) |   |   | ├─IdentifierSyntax
//@[004:0012) |   |   | | └─Token(Identifier) |vaultSub|
//@[012:0013) |   |   | ├─Token(Colon) |:|
//@[014:0025) |   |   | └─StringSyntax
//@[014:0025) |   |   |   └─Token(StringComplete) |'abcd-efgh'|
//@[025:0027) |   |   ├─Token(NewLine) |\r\n|
  }
//@[002:0003) |   |   └─Token(RightBrace) |}|
//@[003:0005) |   ├─Token(NewLine) |\r\n|
  {
//@[002:0090) |   ├─ArrayItemSyntax
//@[002:0090) |   | └─ObjectSyntax
//@[002:0003) |   |   ├─Token(LeftBrace) |{|
//@[003:0005) |   |   ├─Token(NewLine) |\r\n|
    vaultName: 'test-2-kv'
//@[004:0026) |   |   ├─ObjectPropertySyntax
//@[004:0013) |   |   | ├─IdentifierSyntax
//@[004:0013) |   |   | | └─Token(Identifier) |vaultName|
//@[013:0014) |   |   | ├─Token(Colon) |:|
//@[015:0026) |   |   | └─StringSyntax
//@[015:0026) |   |   |   └─Token(StringComplete) |'test-2-kv'|
//@[026:0028) |   |   ├─Token(NewLine) |\r\n|
    vaultRG: 'test-2-rg'
//@[004:0024) |   |   ├─ObjectPropertySyntax
//@[004:0011) |   |   | ├─IdentifierSyntax
//@[004:0011) |   |   | | └─Token(Identifier) |vaultRG|
//@[011:0012) |   |   | ├─Token(Colon) |:|
//@[013:0024) |   |   | └─StringSyntax
//@[013:0024) |   |   |   └─Token(StringComplete) |'test-2-rg'|
//@[024:0026) |   |   ├─Token(NewLine) |\r\n|
    vaultSub: 'ijkl-1adg1'
//@[004:0026) |   |   ├─ObjectPropertySyntax
//@[004:0012) |   |   | ├─IdentifierSyntax
//@[004:0012) |   |   | | └─Token(Identifier) |vaultSub|
//@[012:0013) |   |   | ├─Token(Colon) |:|
//@[014:0026) |   |   | └─StringSyntax
//@[014:0026) |   |   |   └─Token(StringComplete) |'ijkl-1adg1'|
//@[026:0028) |   |   ├─Token(NewLine) |\r\n|
  }
//@[002:0003) |   |   └─Token(RightBrace) |}|
//@[003:0005) |   ├─Token(NewLine) |\r\n|
]
//@[000:0001) |   └─Token(RightSquare) |]|
//@[001:0003) ├─Token(NewLine) |\r\n|
var secrets = [
//@[000:0132) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0011) | ├─IdentifierSyntax
//@[004:0011) | | └─Token(Identifier) |secrets|
//@[012:0013) | ├─Token(Assignment) |=|
//@[014:0132) | └─ArraySyntax
//@[014:0015) |   ├─Token(LeftSquare) |[|
//@[015:0017) |   ├─Token(NewLine) |\r\n|
  {
//@[002:0055) |   ├─ArrayItemSyntax
//@[002:0055) |   | └─ObjectSyntax
//@[002:0003) |   |   ├─Token(LeftBrace) |{|
//@[003:0005) |   |   ├─Token(NewLine) |\r\n|
    name: 'secret01'
//@[004:0020) |   |   ├─ObjectPropertySyntax
//@[004:0008) |   |   | ├─IdentifierSyntax
//@[004:0008) |   |   | | └─Token(Identifier) |name|
//@[008:0009) |   |   | ├─Token(Colon) |:|
//@[010:0020) |   |   | └─StringSyntax
//@[010:0020) |   |   |   └─Token(StringComplete) |'secret01'|
//@[020:0022) |   |   ├─Token(NewLine) |\r\n|
    version: 'versionA'
//@[004:0023) |   |   ├─ObjectPropertySyntax
//@[004:0011) |   |   | ├─IdentifierSyntax
//@[004:0011) |   |   | | └─Token(Identifier) |version|
//@[011:0012) |   |   | ├─Token(Colon) |:|
//@[013:0023) |   |   | └─StringSyntax
//@[013:0023) |   |   |   └─Token(StringComplete) |'versionA'|
//@[023:0025) |   |   ├─Token(NewLine) |\r\n|
  }
//@[002:0003) |   |   └─Token(RightBrace) |}|
//@[003:0005) |   ├─Token(NewLine) |\r\n|
  {
//@[002:0055) |   ├─ArrayItemSyntax
//@[002:0055) |   | └─ObjectSyntax
//@[002:0003) |   |   ├─Token(LeftBrace) |{|
//@[003:0005) |   |   ├─Token(NewLine) |\r\n|
    name: 'secret02'
//@[004:0020) |   |   ├─ObjectPropertySyntax
//@[004:0008) |   |   | ├─IdentifierSyntax
//@[004:0008) |   |   | | └─Token(Identifier) |name|
//@[008:0009) |   |   | ├─Token(Colon) |:|
//@[010:0020) |   |   | └─StringSyntax
//@[010:0020) |   |   |   └─Token(StringComplete) |'secret02'|
//@[020:0022) |   |   ├─Token(NewLine) |\r\n|
    version: 'versionB'
//@[004:0023) |   |   ├─ObjectPropertySyntax
//@[004:0011) |   |   | ├─IdentifierSyntax
//@[004:0011) |   |   | | └─Token(Identifier) |version|
//@[011:0012) |   |   | ├─Token(Colon) |:|
//@[013:0023) |   |   | └─StringSyntax
//@[013:0023) |   |   |   └─Token(StringComplete) |'versionB'|
//@[023:0025) |   |   ├─Token(NewLine) |\r\n|
  }
//@[002:0003) |   |   └─Token(RightBrace) |}|
//@[003:0005) |   ├─Token(NewLine) |\r\n|
]
//@[000:0001) |   └─Token(RightSquare) |]|
//@[001:0005) ├─Token(NewLine) |\r\n\r\n|

resource loopedKv 'Microsoft.KeyVault/vaults@2019-09-01' existing = [for vault in vaults: {
//@[000:0175) ├─ResourceDeclarationSyntax
//@[000:0008) | ├─Token(Identifier) |resource|
//@[009:0017) | ├─IdentifierSyntax
//@[009:0017) | | └─Token(Identifier) |loopedKv|
//@[018:0056) | ├─StringSyntax
//@[018:0056) | | └─Token(StringComplete) |'Microsoft.KeyVault/vaults@2019-09-01'|
//@[057:0065) | ├─Token(Identifier) |existing|
//@[066:0067) | ├─Token(Assignment) |=|
//@[068:0175) | └─ForSyntax
//@[068:0069) |   ├─Token(LeftSquare) |[|
//@[069:0072) |   ├─Token(Identifier) |for|
//@[073:0078) |   ├─LocalVariableSyntax
//@[073:0078) |   | └─IdentifierSyntax
//@[073:0078) |   |   └─Token(Identifier) |vault|
//@[079:0081) |   ├─Token(Identifier) |in|
//@[082:0088) |   ├─VariableAccessSyntax
//@[082:0088) |   | └─IdentifierSyntax
//@[082:0088) |   |   └─Token(Identifier) |vaults|
//@[088:0089) |   ├─Token(Colon) |:|
//@[090:0174) |   ├─ObjectSyntax
//@[090:0091) |   | ├─Token(LeftBrace) |{|
//@[091:0093) |   | ├─Token(NewLine) |\r\n|
  name: vault.vaultName
//@[002:0023) |   | ├─ObjectPropertySyntax
//@[002:0006) |   | | ├─IdentifierSyntax
//@[002:0006) |   | | | └─Token(Identifier) |name|
//@[006:0007) |   | | ├─Token(Colon) |:|
//@[008:0023) |   | | └─PropertyAccessSyntax
//@[008:0013) |   | |   ├─VariableAccessSyntax
//@[008:0013) |   | |   | └─IdentifierSyntax
//@[008:0013) |   | |   |   └─Token(Identifier) |vault|
//@[013:0014) |   | |   ├─Token(Dot) |.|
//@[014:0023) |   | |   └─IdentifierSyntax
//@[014:0023) |   | |     └─Token(Identifier) |vaultName|
//@[023:0025) |   | ├─Token(NewLine) |\r\n|
  scope: resourceGroup(vault.vaultSub, vault.vaultRG)
//@[002:0053) |   | ├─ObjectPropertySyntax
//@[002:0007) |   | | ├─IdentifierSyntax
//@[002:0007) |   | | | └─Token(Identifier) |scope|
//@[007:0008) |   | | ├─Token(Colon) |:|
//@[009:0053) |   | | └─FunctionCallSyntax
//@[009:0022) |   | |   ├─IdentifierSyntax
//@[009:0022) |   | |   | └─Token(Identifier) |resourceGroup|
//@[022:0023) |   | |   ├─Token(LeftParen) |(|
//@[023:0037) |   | |   ├─FunctionArgumentSyntax
//@[023:0037) |   | |   | └─PropertyAccessSyntax
//@[023:0028) |   | |   |   ├─VariableAccessSyntax
//@[023:0028) |   | |   |   | └─IdentifierSyntax
//@[023:0028) |   | |   |   |   └─Token(Identifier) |vault|
//@[028:0029) |   | |   |   ├─Token(Dot) |.|
//@[029:0037) |   | |   |   └─IdentifierSyntax
//@[029:0037) |   | |   |     └─Token(Identifier) |vaultSub|
//@[037:0038) |   | |   ├─Token(Comma) |,|
//@[039:0052) |   | |   ├─FunctionArgumentSyntax
//@[039:0052) |   | |   | └─PropertyAccessSyntax
//@[039:0044) |   | |   |   ├─VariableAccessSyntax
//@[039:0044) |   | |   |   | └─IdentifierSyntax
//@[039:0044) |   | |   |   |   └─Token(Identifier) |vault|
//@[044:0045) |   | |   |   ├─Token(Dot) |.|
//@[045:0052) |   | |   |   └─IdentifierSyntax
//@[045:0052) |   | |   |     └─Token(Identifier) |vaultRG|
//@[052:0053) |   | |   └─Token(RightParen) |)|
//@[053:0055) |   | ├─Token(NewLine) |\r\n|
}]
//@[000:0001) |   | └─Token(RightBrace) |}|
//@[001:0002) |   └─Token(RightSquare) |]|
//@[002:0006) ├─Token(NewLine) |\r\n\r\n|

module secureModuleLooped 'child/secureParams.bicep' = [for (secret, i) in secrets: {
//@[000:0278) ├─ModuleDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |module|
//@[007:0025) | ├─IdentifierSyntax
//@[007:0025) | | └─Token(Identifier) |secureModuleLooped|
//@[026:0052) | ├─StringSyntax
//@[026:0052) | | └─Token(StringComplete) |'child/secureParams.bicep'|
//@[053:0054) | ├─Token(Assignment) |=|
//@[055:0278) | └─ForSyntax
//@[055:0056) |   ├─Token(LeftSquare) |[|
//@[056:0059) |   ├─Token(Identifier) |for|
//@[060:0071) |   ├─VariableBlockSyntax
//@[060:0061) |   | ├─Token(LeftParen) |(|
//@[061:0067) |   | ├─LocalVariableSyntax
//@[061:0067) |   | | └─IdentifierSyntax
//@[061:0067) |   | |   └─Token(Identifier) |secret|
//@[067:0068) |   | ├─Token(Comma) |,|
//@[069:0070) |   | ├─LocalVariableSyntax
//@[069:0070) |   | | └─IdentifierSyntax
//@[069:0070) |   | |   └─Token(Identifier) |i|
//@[070:0071) |   | └─Token(RightParen) |)|
//@[072:0074) |   ├─Token(Identifier) |in|
//@[075:0082) |   ├─VariableAccessSyntax
//@[075:0082) |   | └─IdentifierSyntax
//@[075:0082) |   |   └─Token(Identifier) |secrets|
//@[082:0083) |   ├─Token(Colon) |:|
//@[084:0277) |   ├─ObjectSyntax
//@[084:0085) |   | ├─Token(LeftBrace) |{|
//@[085:0087) |   | ├─Token(NewLine) |\r\n|
  name: 'secureModuleLooped-${i}'
//@[002:0033) |   | ├─ObjectPropertySyntax
//@[002:0006) |   | | ├─IdentifierSyntax
//@[002:0006) |   | | | └─Token(Identifier) |name|
//@[006:0007) |   | | ├─Token(Colon) |:|
//@[008:0033) |   | | └─StringSyntax
//@[008:0030) |   | |   ├─Token(StringLeftPiece) |'secureModuleLooped-${|
//@[030:0031) |   | |   ├─VariableAccessSyntax
//@[030:0031) |   | |   | └─IdentifierSyntax
//@[030:0031) |   | |   |   └─Token(Identifier) |i|
//@[031:0033) |   | |   └─Token(StringRightPiece) |}'|
//@[033:0035) |   | ├─Token(NewLine) |\r\n|
  params: {
//@[002:0152) |   | ├─ObjectPropertySyntax
//@[002:0008) |   | | ├─IdentifierSyntax
//@[002:0008) |   | | | └─Token(Identifier) |params|
//@[008:0009) |   | | ├─Token(Colon) |:|
//@[010:0152) |   | | └─ObjectSyntax
//@[010:0011) |   | |   ├─Token(LeftBrace) |{|
//@[011:0013) |   | |   ├─Token(NewLine) |\r\n|
    secureStringParam1: loopedKv[i].getSecret(secret.name)
//@[004:0058) |   | |   ├─ObjectPropertySyntax
//@[004:0022) |   | |   | ├─IdentifierSyntax
//@[004:0022) |   | |   | | └─Token(Identifier) |secureStringParam1|
//@[022:0023) |   | |   | ├─Token(Colon) |:|
//@[024:0058) |   | |   | └─InstanceFunctionCallSyntax
//@[024:0035) |   | |   |   ├─ArrayAccessSyntax
//@[024:0032) |   | |   |   | ├─VariableAccessSyntax
//@[024:0032) |   | |   |   | | └─IdentifierSyntax
//@[024:0032) |   | |   |   | |   └─Token(Identifier) |loopedKv|
//@[032:0033) |   | |   |   | ├─Token(LeftSquare) |[|
//@[033:0034) |   | |   |   | ├─VariableAccessSyntax
//@[033:0034) |   | |   |   | | └─IdentifierSyntax
//@[033:0034) |   | |   |   | |   └─Token(Identifier) |i|
//@[034:0035) |   | |   |   | └─Token(RightSquare) |]|
//@[035:0036) |   | |   |   ├─Token(Dot) |.|
//@[036:0045) |   | |   |   ├─IdentifierSyntax
//@[036:0045) |   | |   |   | └─Token(Identifier) |getSecret|
//@[045:0046) |   | |   |   ├─Token(LeftParen) |(|
//@[046:0057) |   | |   |   ├─FunctionArgumentSyntax
//@[046:0057) |   | |   |   | └─PropertyAccessSyntax
//@[046:0052) |   | |   |   |   ├─VariableAccessSyntax
//@[046:0052) |   | |   |   |   | └─IdentifierSyntax
//@[046:0052) |   | |   |   |   |   └─Token(Identifier) |secret|
//@[052:0053) |   | |   |   |   ├─Token(Dot) |.|
//@[053:0057) |   | |   |   |   └─IdentifierSyntax
//@[053:0057) |   | |   |   |     └─Token(Identifier) |name|
//@[057:0058) |   | |   |   └─Token(RightParen) |)|
//@[058:0060) |   | |   ├─Token(NewLine) |\r\n|
    secureStringParam2: loopedKv[i].getSecret(secret.name, secret.version)
//@[004:0074) |   | |   ├─ObjectPropertySyntax
//@[004:0022) |   | |   | ├─IdentifierSyntax
//@[004:0022) |   | |   | | └─Token(Identifier) |secureStringParam2|
//@[022:0023) |   | |   | ├─Token(Colon) |:|
//@[024:0074) |   | |   | └─InstanceFunctionCallSyntax
//@[024:0035) |   | |   |   ├─ArrayAccessSyntax
//@[024:0032) |   | |   |   | ├─VariableAccessSyntax
//@[024:0032) |   | |   |   | | └─IdentifierSyntax
//@[024:0032) |   | |   |   | |   └─Token(Identifier) |loopedKv|
//@[032:0033) |   | |   |   | ├─Token(LeftSquare) |[|
//@[033:0034) |   | |   |   | ├─VariableAccessSyntax
//@[033:0034) |   | |   |   | | └─IdentifierSyntax
//@[033:0034) |   | |   |   | |   └─Token(Identifier) |i|
//@[034:0035) |   | |   |   | └─Token(RightSquare) |]|
//@[035:0036) |   | |   |   ├─Token(Dot) |.|
//@[036:0045) |   | |   |   ├─IdentifierSyntax
//@[036:0045) |   | |   |   | └─Token(Identifier) |getSecret|
//@[045:0046) |   | |   |   ├─Token(LeftParen) |(|
//@[046:0057) |   | |   |   ├─FunctionArgumentSyntax
//@[046:0057) |   | |   |   | └─PropertyAccessSyntax
//@[046:0052) |   | |   |   |   ├─VariableAccessSyntax
//@[046:0052) |   | |   |   |   | └─IdentifierSyntax
//@[046:0052) |   | |   |   |   |   └─Token(Identifier) |secret|
//@[052:0053) |   | |   |   |   ├─Token(Dot) |.|
//@[053:0057) |   | |   |   |   └─IdentifierSyntax
//@[053:0057) |   | |   |   |     └─Token(Identifier) |name|
//@[057:0058) |   | |   |   ├─Token(Comma) |,|
//@[059:0073) |   | |   |   ├─FunctionArgumentSyntax
//@[059:0073) |   | |   |   | └─PropertyAccessSyntax
//@[059:0065) |   | |   |   |   ├─VariableAccessSyntax
//@[059:0065) |   | |   |   |   | └─IdentifierSyntax
//@[059:0065) |   | |   |   |   |   └─Token(Identifier) |secret|
//@[065:0066) |   | |   |   |   ├─Token(Dot) |.|
//@[066:0073) |   | |   |   |   └─IdentifierSyntax
//@[066:0073) |   | |   |   |     └─Token(Identifier) |version|
//@[073:0074) |   | |   |   └─Token(RightParen) |)|
//@[074:0076) |   | |   ├─Token(NewLine) |\r\n|
  }
//@[002:0003) |   | |   └─Token(RightBrace) |}|
//@[003:0005) |   | ├─Token(NewLine) |\r\n|
}]
//@[000:0001) |   | └─Token(RightBrace) |}|
//@[001:0002) |   └─Token(RightSquare) |]|
//@[002:0008) ├─Token(NewLine) |\r\n\r\n\r\n|


// END: Key Vault Secret Reference
//@[034:0038) ├─Token(NewLine) |\r\n\r\n|

module withSpace 'module with space.bicep' = {
//@[000:0070) ├─ModuleDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |module|
//@[007:0016) | ├─IdentifierSyntax
//@[007:0016) | | └─Token(Identifier) |withSpace|
//@[017:0042) | ├─StringSyntax
//@[017:0042) | | └─Token(StringComplete) |'module with space.bicep'|
//@[043:0044) | ├─Token(Assignment) |=|
//@[045:0070) | └─ObjectSyntax
//@[045:0046) |   ├─Token(LeftBrace) |{|
//@[046:0048) |   ├─Token(NewLine) |\r\n|
  name: 'withSpace'
//@[002:0019) |   ├─ObjectPropertySyntax
//@[002:0006) |   | ├─IdentifierSyntax
//@[002:0006) |   | | └─Token(Identifier) |name|
//@[006:0007) |   | ├─Token(Colon) |:|
//@[008:0019) |   | └─StringSyntax
//@[008:0019) |   |   └─Token(StringComplete) |'withSpace'|
//@[019:0021) |   ├─Token(NewLine) |\r\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0005) ├─Token(NewLine) |\r\n\r\n|

module folderWithSpace 'child/folder with space/child with space.bicep' = {
//@[000:0104) ├─ModuleDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |module|
//@[007:0022) | ├─IdentifierSyntax
//@[007:0022) | | └─Token(Identifier) |folderWithSpace|
//@[023:0071) | ├─StringSyntax
//@[023:0071) | | └─Token(StringComplete) |'child/folder with space/child with space.bicep'|
//@[072:0073) | ├─Token(Assignment) |=|
//@[074:0104) | └─ObjectSyntax
//@[074:0075) |   ├─Token(LeftBrace) |{|
//@[075:0077) |   ├─Token(NewLine) |\r\n|
  name: 'childWithSpace'
//@[002:0024) |   ├─ObjectPropertySyntax
//@[002:0006) |   | ├─IdentifierSyntax
//@[002:0006) |   | | └─Token(Identifier) |name|
//@[006:0007) |   | ├─Token(Colon) |:|
//@[008:0024) |   | └─StringSyntax
//@[008:0024) |   |   └─Token(StringComplete) |'childWithSpace'|
//@[024:0026) |   ├─Token(NewLine) |\r\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0003) ├─Token(NewLine) |\r\n|

//@[000:0000) └─Token(EndOfFile) ||
