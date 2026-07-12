
//@[000:10080) ProgramSyntax
//@[000:00002) ├─Token(NewLine) |\r\n|
@sys.description('this is deployTimeSuffix param')
//@[000:00093) ├─ParameterDeclarationSyntax
//@[000:00050) | ├─DecoratorSyntax
//@[000:00001) | | ├─Token(At) |@|
//@[001:00050) | | └─InstanceFunctionCallSyntax
//@[001:00004) | |   ├─VariableAccessSyntax
//@[001:00004) | |   | └─IdentifierSyntax
//@[001:00004) | |   |   └─Token(Identifier) |sys|
//@[004:00005) | |   ├─Token(Dot) |.|
//@[005:00016) | |   ├─IdentifierSyntax
//@[005:00016) | |   | └─Token(Identifier) |description|
//@[016:00017) | |   ├─Token(LeftParen) |(|
//@[017:00049) | |   ├─FunctionArgumentSyntax
//@[017:00049) | |   | └─StringSyntax
//@[017:00049) | |   |   └─Token(StringComplete) |'this is deployTimeSuffix param'|
//@[049:00050) | |   └─Token(RightParen) |)|
//@[050:00052) | ├─Token(NewLine) |\r\n|
param deployTimeSuffix string = newGuid()
//@[000:00005) | ├─Token(Identifier) |param|
//@[006:00022) | ├─IdentifierSyntax
//@[006:00022) | | └─Token(Identifier) |deployTimeSuffix|
//@[023:00029) | ├─TypeVariableAccessSyntax
//@[023:00029) | | └─IdentifierSyntax
//@[023:00029) | |   └─Token(Identifier) |string|
//@[030:00041) | └─ParameterDefaultValueSyntax
//@[030:00031) |   ├─Token(Assignment) |=|
//@[032:00041) |   └─FunctionCallSyntax
//@[032:00039) |     ├─IdentifierSyntax
//@[032:00039) |     | └─Token(Identifier) |newGuid|
//@[039:00040) |     ├─Token(LeftParen) |(|
//@[040:00041) |     └─Token(RightParen) |)|
//@[041:00045) ├─Token(NewLine) |\r\n\r\n|

@sys.description('this module a')
//@[000:00252) ├─ModuleDeclarationSyntax
//@[000:00033) | ├─DecoratorSyntax
//@[000:00001) | | ├─Token(At) |@|
//@[001:00033) | | └─InstanceFunctionCallSyntax
//@[001:00004) | |   ├─VariableAccessSyntax
//@[001:00004) | |   | └─IdentifierSyntax
//@[001:00004) | |   |   └─Token(Identifier) |sys|
//@[004:00005) | |   ├─Token(Dot) |.|
//@[005:00016) | |   ├─IdentifierSyntax
//@[005:00016) | |   | └─Token(Identifier) |description|
//@[016:00017) | |   ├─Token(LeftParen) |(|
//@[017:00032) | |   ├─FunctionArgumentSyntax
//@[017:00032) | |   | └─StringSyntax
//@[017:00032) | |   |   └─Token(StringComplete) |'this module a'|
//@[032:00033) | |   └─Token(RightParen) |)|
//@[033:00035) | ├─Token(NewLine) |\r\n|
module modATest './modulea.bicep' = {
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00015) | ├─IdentifierSyntax
//@[007:00015) | | └─Token(Identifier) |modATest|
//@[016:00033) | ├─StringSyntax
//@[016:00033) | | └─Token(StringComplete) |'./modulea.bicep'|
//@[034:00035) | ├─Token(Assignment) |=|
//@[036:00217) | └─ObjectSyntax
//@[036:00037) |   ├─Token(LeftBrace) |{|
//@[037:00039) |   ├─Token(NewLine) |\r\n|
  name: 'modATest'
//@[002:00018) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00018) |   | └─StringSyntax
//@[008:00018) |   |   └─Token(StringComplete) |'modATest'|
//@[018:00020) |   ├─Token(NewLine) |\r\n|
  params: {
//@[002:00155) |   ├─ObjectPropertySyntax
//@[002:00008) |   | ├─IdentifierSyntax
//@[002:00008) |   | | └─Token(Identifier) |params|
//@[008:00009) |   | ├─Token(Colon) |:|
//@[010:00155) |   | └─ObjectSyntax
//@[010:00011) |   |   ├─Token(LeftBrace) |{|
//@[011:00013) |   |   ├─Token(NewLine) |\r\n|
    stringParamB: 'hello!'
//@[004:00026) |   |   ├─ObjectPropertySyntax
//@[004:00016) |   |   | ├─IdentifierSyntax
//@[004:00016) |   |   | | └─Token(Identifier) |stringParamB|
//@[016:00017) |   |   | ├─Token(Colon) |:|
//@[018:00026) |   |   | └─StringSyntax
//@[018:00026) |   |   |   └─Token(StringComplete) |'hello!'|
//@[026:00028) |   |   ├─Token(NewLine) |\r\n|
    objParam: {
//@[004:00036) |   |   ├─ObjectPropertySyntax
//@[004:00012) |   |   | ├─IdentifierSyntax
//@[004:00012) |   |   | | └─Token(Identifier) |objParam|
//@[012:00013) |   |   | ├─Token(Colon) |:|
//@[014:00036) |   |   | └─ObjectSyntax
//@[014:00015) |   |   |   ├─Token(LeftBrace) |{|
//@[015:00017) |   |   |   ├─Token(NewLine) |\r\n|
      a: 'b'
//@[006:00012) |   |   |   ├─ObjectPropertySyntax
//@[006:00007) |   |   |   | ├─IdentifierSyntax
//@[006:00007) |   |   |   | | └─Token(Identifier) |a|
//@[007:00008) |   |   |   | ├─Token(Colon) |:|
//@[009:00012) |   |   |   | └─StringSyntax
//@[009:00012) |   |   |   |   └─Token(StringComplete) |'b'|
//@[012:00014) |   |   |   ├─Token(NewLine) |\r\n|
    }
//@[004:00005) |   |   |   └─Token(RightBrace) |}|
//@[005:00007) |   |   ├─Token(NewLine) |\r\n|
    arrayParam: [
//@[004:00071) |   |   ├─ObjectPropertySyntax
//@[004:00014) |   |   | ├─IdentifierSyntax
//@[004:00014) |   |   | | └─Token(Identifier) |arrayParam|
//@[014:00015) |   |   | ├─Token(Colon) |:|
//@[016:00071) |   |   | └─ArraySyntax
//@[016:00017) |   |   |   ├─Token(LeftSquare) |[|
//@[017:00019) |   |   |   ├─Token(NewLine) |\r\n|
      {
//@[006:00032) |   |   |   ├─ArrayItemSyntax
//@[006:00032) |   |   |   | └─ObjectSyntax
//@[006:00007) |   |   |   |   ├─Token(LeftBrace) |{|
//@[007:00009) |   |   |   |   ├─Token(NewLine) |\r\n|
        a: 'b'
//@[008:00014) |   |   |   |   ├─ObjectPropertySyntax
//@[008:00009) |   |   |   |   | ├─IdentifierSyntax
//@[008:00009) |   |   |   |   | | └─Token(Identifier) |a|
//@[009:00010) |   |   |   |   | ├─Token(Colon) |:|
//@[011:00014) |   |   |   |   | └─StringSyntax
//@[011:00014) |   |   |   |   |   └─Token(StringComplete) |'b'|
//@[014:00016) |   |   |   |   ├─Token(NewLine) |\r\n|
      }
//@[006:00007) |   |   |   |   └─Token(RightBrace) |}|
//@[007:00009) |   |   |   ├─Token(NewLine) |\r\n|
      'abc'
//@[006:00011) |   |   |   ├─ArrayItemSyntax
//@[006:00011) |   |   |   | └─StringSyntax
//@[006:00011) |   |   |   |   └─Token(StringComplete) |'abc'|
//@[011:00013) |   |   |   ├─Token(NewLine) |\r\n|
    ]
//@[004:00005) |   |   |   └─Token(RightSquare) |]|
//@[005:00007) |   |   ├─Token(NewLine) |\r\n|
  }
//@[002:00003) |   |   └─Token(RightBrace) |}|
//@[003:00005) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00007) ├─Token(NewLine) |\r\n\r\n\r\n|


@sys.description('this module b')
//@[000:00136) ├─ModuleDeclarationSyntax
//@[000:00033) | ├─DecoratorSyntax
//@[000:00001) | | ├─Token(At) |@|
//@[001:00033) | | └─InstanceFunctionCallSyntax
//@[001:00004) | |   ├─VariableAccessSyntax
//@[001:00004) | |   | └─IdentifierSyntax
//@[001:00004) | |   |   └─Token(Identifier) |sys|
//@[004:00005) | |   ├─Token(Dot) |.|
//@[005:00016) | |   ├─IdentifierSyntax
//@[005:00016) | |   | └─Token(Identifier) |description|
//@[016:00017) | |   ├─Token(LeftParen) |(|
//@[017:00032) | |   ├─FunctionArgumentSyntax
//@[017:00032) | |   | └─StringSyntax
//@[017:00032) | |   |   └─Token(StringComplete) |'this module b'|
//@[032:00033) | |   └─Token(RightParen) |)|
//@[033:00035) | ├─Token(NewLine) |\r\n|
module modB './child/moduleb.bicep' = {
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00011) | ├─IdentifierSyntax
//@[007:00011) | | └─Token(Identifier) |modB|
//@[012:00035) | ├─StringSyntax
//@[012:00035) | | └─Token(StringComplete) |'./child/moduleb.bicep'|
//@[036:00037) | ├─Token(Assignment) |=|
//@[038:00101) | └─ObjectSyntax
//@[038:00039) |   ├─Token(LeftBrace) |{|
//@[039:00041) |   ├─Token(NewLine) |\r\n|
  name: 'modB'
//@[002:00014) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00014) |   | └─StringSyntax
//@[008:00014) |   |   └─Token(StringComplete) |'modB'|
//@[014:00016) |   ├─Token(NewLine) |\r\n|
  params: {
//@[002:00041) |   ├─ObjectPropertySyntax
//@[002:00008) |   | ├─IdentifierSyntax
//@[002:00008) |   | | └─Token(Identifier) |params|
//@[008:00009) |   | ├─Token(Colon) |:|
//@[010:00041) |   | └─ObjectSyntax
//@[010:00011) |   |   ├─Token(LeftBrace) |{|
//@[011:00013) |   |   ├─Token(NewLine) |\r\n|
    location: 'West US'
//@[004:00023) |   |   ├─ObjectPropertySyntax
//@[004:00012) |   |   | ├─IdentifierSyntax
//@[004:00012) |   |   | | └─Token(Identifier) |location|
//@[012:00013) |   |   | ├─Token(Colon) |:|
//@[014:00023) |   |   | └─StringSyntax
//@[014:00023) |   |   |   └─Token(StringComplete) |'West US'|
//@[023:00025) |   |   ├─Token(NewLine) |\r\n|
  }
//@[002:00003) |   |   └─Token(RightBrace) |}|
//@[003:00005) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

@sys.description('this is just module b with a condition')
//@[000:00203) ├─ModuleDeclarationSyntax
//@[000:00058) | ├─DecoratorSyntax
//@[000:00001) | | ├─Token(At) |@|
//@[001:00058) | | └─InstanceFunctionCallSyntax
//@[001:00004) | |   ├─VariableAccessSyntax
//@[001:00004) | |   | └─IdentifierSyntax
//@[001:00004) | |   |   └─Token(Identifier) |sys|
//@[004:00005) | |   ├─Token(Dot) |.|
//@[005:00016) | |   ├─IdentifierSyntax
//@[005:00016) | |   | └─Token(Identifier) |description|
//@[016:00017) | |   ├─Token(LeftParen) |(|
//@[017:00057) | |   ├─FunctionArgumentSyntax
//@[017:00057) | |   | └─StringSyntax
//@[017:00057) | |   |   └─Token(StringComplete) |'this is just module b with a condition'|
//@[057:00058) | |   └─Token(RightParen) |)|
//@[058:00060) | ├─Token(NewLine) |\r\n|
module modBWithCondition './child/moduleb.bicep' = if (1 + 1 == 2) {
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00024) | ├─IdentifierSyntax
//@[007:00024) | | └─Token(Identifier) |modBWithCondition|
//@[025:00048) | ├─StringSyntax
//@[025:00048) | | └─Token(StringComplete) |'./child/moduleb.bicep'|
//@[049:00050) | ├─Token(Assignment) |=|
//@[051:00143) | └─IfConditionSyntax
//@[051:00053) |   ├─Token(Identifier) |if|
//@[054:00066) |   ├─ParenthesizedExpressionSyntax
//@[054:00055) |   | ├─Token(LeftParen) |(|
//@[055:00065) |   | ├─BinaryOperationSyntax
//@[055:00060) |   | | ├─BinaryOperationSyntax
//@[055:00056) |   | | | ├─IntegerLiteralSyntax
//@[055:00056) |   | | | | └─Token(Integer) |1|
//@[057:00058) |   | | | ├─Token(Plus) |+|
//@[059:00060) |   | | | └─IntegerLiteralSyntax
//@[059:00060) |   | | |   └─Token(Integer) |1|
//@[061:00063) |   | | ├─Token(Equals) |==|
//@[064:00065) |   | | └─IntegerLiteralSyntax
//@[064:00065) |   | |   └─Token(Integer) |2|
//@[065:00066) |   | └─Token(RightParen) |)|
//@[067:00143) |   └─ObjectSyntax
//@[067:00068) |     ├─Token(LeftBrace) |{|
//@[068:00070) |     ├─Token(NewLine) |\r\n|
  name: 'modBWithCondition'
//@[002:00027) |     ├─ObjectPropertySyntax
//@[002:00006) |     | ├─IdentifierSyntax
//@[002:00006) |     | | └─Token(Identifier) |name|
//@[006:00007) |     | ├─Token(Colon) |:|
//@[008:00027) |     | └─StringSyntax
//@[008:00027) |     |   └─Token(StringComplete) |'modBWithCondition'|
//@[027:00029) |     ├─Token(NewLine) |\r\n|
  params: {
//@[002:00041) |     ├─ObjectPropertySyntax
//@[002:00008) |     | ├─IdentifierSyntax
//@[002:00008) |     | | └─Token(Identifier) |params|
//@[008:00009) |     | ├─Token(Colon) |:|
//@[010:00041) |     | └─ObjectSyntax
//@[010:00011) |     |   ├─Token(LeftBrace) |{|
//@[011:00013) |     |   ├─Token(NewLine) |\r\n|
    location: 'East US'
//@[004:00023) |     |   ├─ObjectPropertySyntax
//@[004:00012) |     |   | ├─IdentifierSyntax
//@[004:00012) |     |   | | └─Token(Identifier) |location|
//@[012:00013) |     |   | ├─Token(Colon) |:|
//@[014:00023) |     |   | └─StringSyntax
//@[014:00023) |     |   |   └─Token(StringComplete) |'East US'|
//@[023:00025) |     |   ├─Token(NewLine) |\r\n|
  }
//@[002:00003) |     |   └─Token(RightBrace) |}|
//@[003:00005) |     ├─Token(NewLine) |\r\n|
}
//@[000:00001) |     └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

module modBWithCondition2 './child/moduleb.bicep' =
//@[000:00166) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00025) | ├─IdentifierSyntax
//@[007:00025) | | └─Token(Identifier) |modBWithCondition2|
//@[026:00049) | ├─StringSyntax
//@[026:00049) | | └─Token(StringComplete) |'./child/moduleb.bicep'|
//@[050:00051) | ├─Token(Assignment) |=|
//@[051:00053) | ├─Token(NewLine) |\r\n|
// awkward comment
//@[018:00020) | ├─Token(NewLine) |\r\n|
if (1 + 1 == 2) {
//@[000:00093) | └─IfConditionSyntax
//@[000:00002) |   ├─Token(Identifier) |if|
//@[003:00015) |   ├─ParenthesizedExpressionSyntax
//@[003:00004) |   | ├─Token(LeftParen) |(|
//@[004:00014) |   | ├─BinaryOperationSyntax
//@[004:00009) |   | | ├─BinaryOperationSyntax
//@[004:00005) |   | | | ├─IntegerLiteralSyntax
//@[004:00005) |   | | | | └─Token(Integer) |1|
//@[006:00007) |   | | | ├─Token(Plus) |+|
//@[008:00009) |   | | | └─IntegerLiteralSyntax
//@[008:00009) |   | | |   └─Token(Integer) |1|
//@[010:00012) |   | | ├─Token(Equals) |==|
//@[013:00014) |   | | └─IntegerLiteralSyntax
//@[013:00014) |   | |   └─Token(Integer) |2|
//@[014:00015) |   | └─Token(RightParen) |)|
//@[016:00093) |   └─ObjectSyntax
//@[016:00017) |     ├─Token(LeftBrace) |{|
//@[017:00019) |     ├─Token(NewLine) |\r\n|
  name: 'modBWithCondition2'
//@[002:00028) |     ├─ObjectPropertySyntax
//@[002:00006) |     | ├─IdentifierSyntax
//@[002:00006) |     | | └─Token(Identifier) |name|
//@[006:00007) |     | ├─Token(Colon) |:|
//@[008:00028) |     | └─StringSyntax
//@[008:00028) |     |   └─Token(StringComplete) |'modBWithCondition2'|
//@[028:00030) |     ├─Token(NewLine) |\r\n|
  params: {
//@[002:00041) |     ├─ObjectPropertySyntax
//@[002:00008) |     | ├─IdentifierSyntax
//@[002:00008) |     | | └─Token(Identifier) |params|
//@[008:00009) |     | ├─Token(Colon) |:|
//@[010:00041) |     | └─ObjectSyntax
//@[010:00011) |     |   ├─Token(LeftBrace) |{|
//@[011:00013) |     |   ├─Token(NewLine) |\r\n|
    location: 'East US'
//@[004:00023) |     |   ├─ObjectPropertySyntax
//@[004:00012) |     |   | ├─IdentifierSyntax
//@[004:00012) |     |   | | └─Token(Identifier) |location|
//@[012:00013) |     |   | ├─Token(Colon) |:|
//@[014:00023) |     |   | └─StringSyntax
//@[014:00023) |     |   |   └─Token(StringComplete) |'East US'|
//@[023:00025) |     |   ├─Token(NewLine) |\r\n|
  }
//@[002:00003) |     |   └─Token(RightBrace) |}|
//@[003:00005) |     ├─Token(NewLine) |\r\n|
}
//@[000:00001) |     └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

module modC './child/modulec.json' = {
//@[000:00100) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00011) | ├─IdentifierSyntax
//@[007:00011) | | └─Token(Identifier) |modC|
//@[012:00034) | ├─StringSyntax
//@[012:00034) | | └─Token(StringComplete) |'./child/modulec.json'|
//@[035:00036) | ├─Token(Assignment) |=|
//@[037:00100) | └─ObjectSyntax
//@[037:00038) |   ├─Token(LeftBrace) |{|
//@[038:00040) |   ├─Token(NewLine) |\r\n|
  name: 'modC'
//@[002:00014) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00014) |   | └─StringSyntax
//@[008:00014) |   |   └─Token(StringComplete) |'modC'|
//@[014:00016) |   ├─Token(NewLine) |\r\n|
  params: {
//@[002:00041) |   ├─ObjectPropertySyntax
//@[002:00008) |   | ├─IdentifierSyntax
//@[002:00008) |   | | └─Token(Identifier) |params|
//@[008:00009) |   | ├─Token(Colon) |:|
//@[010:00041) |   | └─ObjectSyntax
//@[010:00011) |   |   ├─Token(LeftBrace) |{|
//@[011:00013) |   |   ├─Token(NewLine) |\r\n|
    location: 'West US'
//@[004:00023) |   |   ├─ObjectPropertySyntax
//@[004:00012) |   |   | ├─IdentifierSyntax
//@[004:00012) |   |   | | └─Token(Identifier) |location|
//@[012:00013) |   |   | ├─Token(Colon) |:|
//@[014:00023) |   |   | └─StringSyntax
//@[014:00023) |   |   |   └─Token(StringComplete) |'West US'|
//@[023:00025) |   |   ├─Token(NewLine) |\r\n|
  }
//@[002:00003) |   |   └─Token(RightBrace) |}|
//@[003:00005) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

module modCWithCondition './child/modulec.json' = if (2 - 1 == 1) {
//@[000:00142) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00024) | ├─IdentifierSyntax
//@[007:00024) | | └─Token(Identifier) |modCWithCondition|
//@[025:00047) | ├─StringSyntax
//@[025:00047) | | └─Token(StringComplete) |'./child/modulec.json'|
//@[048:00049) | ├─Token(Assignment) |=|
//@[050:00142) | └─IfConditionSyntax
//@[050:00052) |   ├─Token(Identifier) |if|
//@[053:00065) |   ├─ParenthesizedExpressionSyntax
//@[053:00054) |   | ├─Token(LeftParen) |(|
//@[054:00064) |   | ├─BinaryOperationSyntax
//@[054:00059) |   | | ├─BinaryOperationSyntax
//@[054:00055) |   | | | ├─IntegerLiteralSyntax
//@[054:00055) |   | | | | └─Token(Integer) |2|
//@[056:00057) |   | | | ├─Token(Minus) |-|
//@[058:00059) |   | | | └─IntegerLiteralSyntax
//@[058:00059) |   | | |   └─Token(Integer) |1|
//@[060:00062) |   | | ├─Token(Equals) |==|
//@[063:00064) |   | | └─IntegerLiteralSyntax
//@[063:00064) |   | |   └─Token(Integer) |1|
//@[064:00065) |   | └─Token(RightParen) |)|
//@[066:00142) |   └─ObjectSyntax
//@[066:00067) |     ├─Token(LeftBrace) |{|
//@[067:00069) |     ├─Token(NewLine) |\r\n|
  name: 'modCWithCondition'
//@[002:00027) |     ├─ObjectPropertySyntax
//@[002:00006) |     | ├─IdentifierSyntax
//@[002:00006) |     | | └─Token(Identifier) |name|
//@[006:00007) |     | ├─Token(Colon) |:|
//@[008:00027) |     | └─StringSyntax
//@[008:00027) |     |   └─Token(StringComplete) |'modCWithCondition'|
//@[027:00029) |     ├─Token(NewLine) |\r\n|
  params: {
//@[002:00041) |     ├─ObjectPropertySyntax
//@[002:00008) |     | ├─IdentifierSyntax
//@[002:00008) |     | | └─Token(Identifier) |params|
//@[008:00009) |     | ├─Token(Colon) |:|
//@[010:00041) |     | └─ObjectSyntax
//@[010:00011) |     |   ├─Token(LeftBrace) |{|
//@[011:00013) |     |   ├─Token(NewLine) |\r\n|
    location: 'East US'
//@[004:00023) |     |   ├─ObjectPropertySyntax
//@[004:00012) |     |   | ├─IdentifierSyntax
//@[004:00012) |     |   | | └─Token(Identifier) |location|
//@[012:00013) |     |   | ├─Token(Colon) |:|
//@[014:00023) |     |   | └─StringSyntax
//@[014:00023) |     |   |   └─Token(StringComplete) |'East US'|
//@[023:00025) |     |   ├─Token(NewLine) |\r\n|
  }
//@[002:00003) |     |   └─Token(RightBrace) |}|
//@[003:00005) |     ├─Token(NewLine) |\r\n|
}
//@[000:00001) |     └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

module optionalWithNoParams1 './child/optionalParams.bicep'= {
//@[000:00098) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00028) | ├─IdentifierSyntax
//@[007:00028) | | └─Token(Identifier) |optionalWithNoParams1|
//@[029:00059) | ├─StringSyntax
//@[029:00059) | | └─Token(StringComplete) |'./child/optionalParams.bicep'|
//@[059:00060) | ├─Token(Assignment) |=|
//@[061:00098) | └─ObjectSyntax
//@[061:00062) |   ├─Token(LeftBrace) |{|
//@[062:00064) |   ├─Token(NewLine) |\r\n|
  name: 'optionalWithNoParams1'
//@[002:00031) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00031) |   | └─StringSyntax
//@[008:00031) |   |   └─Token(StringComplete) |'optionalWithNoParams1'|
//@[031:00033) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

module optionalWithNoParams2 './child/optionalParams.bicep'= {
//@[000:00116) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00028) | ├─IdentifierSyntax
//@[007:00028) | | └─Token(Identifier) |optionalWithNoParams2|
//@[029:00059) | ├─StringSyntax
//@[029:00059) | | └─Token(StringComplete) |'./child/optionalParams.bicep'|
//@[059:00060) | ├─Token(Assignment) |=|
//@[061:00116) | └─ObjectSyntax
//@[061:00062) |   ├─Token(LeftBrace) |{|
//@[062:00064) |   ├─Token(NewLine) |\r\n|
  name: 'optionalWithNoParams2'
//@[002:00031) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00031) |   | └─StringSyntax
//@[008:00031) |   |   └─Token(StringComplete) |'optionalWithNoParams2'|
//@[031:00033) |   ├─Token(NewLine) |\r\n|
  params: {
//@[002:00016) |   ├─ObjectPropertySyntax
//@[002:00008) |   | ├─IdentifierSyntax
//@[002:00008) |   | | └─Token(Identifier) |params|
//@[008:00009) |   | ├─Token(Colon) |:|
//@[010:00016) |   | └─ObjectSyntax
//@[010:00011) |   |   ├─Token(LeftBrace) |{|
//@[011:00013) |   |   ├─Token(NewLine) |\r\n|
  }
//@[002:00003) |   |   └─Token(RightBrace) |}|
//@[003:00005) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

module optionalWithAllParams './child/optionalParams.bicep'= {
//@[000:00210) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00028) | ├─IdentifierSyntax
//@[007:00028) | | └─Token(Identifier) |optionalWithAllParams|
//@[029:00059) | ├─StringSyntax
//@[029:00059) | | └─Token(StringComplete) |'./child/optionalParams.bicep'|
//@[059:00060) | ├─Token(Assignment) |=|
//@[061:00210) | └─ObjectSyntax
//@[061:00062) |   ├─Token(LeftBrace) |{|
//@[062:00064) |   ├─Token(NewLine) |\r\n|
  name: 'optionalWithNoParams3'
//@[002:00031) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00031) |   | └─StringSyntax
//@[008:00031) |   |   └─Token(StringComplete) |'optionalWithNoParams3'|
//@[031:00033) |   ├─Token(NewLine) |\r\n|
  params: {
//@[002:00110) |   ├─ObjectPropertySyntax
//@[002:00008) |   | ├─IdentifierSyntax
//@[002:00008) |   | | └─Token(Identifier) |params|
//@[008:00009) |   | ├─Token(Colon) |:|
//@[010:00110) |   | └─ObjectSyntax
//@[010:00011) |   |   ├─Token(LeftBrace) |{|
//@[011:00013) |   |   ├─Token(NewLine) |\r\n|
    optionalString: 'abc'
//@[004:00025) |   |   ├─ObjectPropertySyntax
//@[004:00018) |   |   | ├─IdentifierSyntax
//@[004:00018) |   |   | | └─Token(Identifier) |optionalString|
//@[018:00019) |   |   | ├─Token(Colon) |:|
//@[020:00025) |   |   | └─StringSyntax
//@[020:00025) |   |   |   └─Token(StringComplete) |'abc'|
//@[025:00027) |   |   ├─Token(NewLine) |\r\n|
    optionalInt: 42
//@[004:00019) |   |   ├─ObjectPropertySyntax
//@[004:00015) |   |   | ├─IdentifierSyntax
//@[004:00015) |   |   | | └─Token(Identifier) |optionalInt|
//@[015:00016) |   |   | ├─Token(Colon) |:|
//@[017:00019) |   |   | └─IntegerLiteralSyntax
//@[017:00019) |   |   |   └─Token(Integer) |42|
//@[019:00021) |   |   ├─Token(NewLine) |\r\n|
    optionalObj: { }
//@[004:00020) |   |   ├─ObjectPropertySyntax
//@[004:00015) |   |   | ├─IdentifierSyntax
//@[004:00015) |   |   | | └─Token(Identifier) |optionalObj|
//@[015:00016) |   |   | ├─Token(Colon) |:|
//@[017:00020) |   |   | └─ObjectSyntax
//@[017:00018) |   |   |   ├─Token(LeftBrace) |{|
//@[019:00020) |   |   |   └─Token(RightBrace) |}|
//@[020:00022) |   |   ├─Token(NewLine) |\r\n|
    optionalArray: [ ]
//@[004:00022) |   |   ├─ObjectPropertySyntax
//@[004:00017) |   |   | ├─IdentifierSyntax
//@[004:00017) |   |   | | └─Token(Identifier) |optionalArray|
//@[017:00018) |   |   | ├─Token(Colon) |:|
//@[019:00022) |   |   | └─ArraySyntax
//@[019:00020) |   |   |   ├─Token(LeftSquare) |[|
//@[021:00022) |   |   |   └─Token(RightSquare) |]|
//@[022:00024) |   |   ├─Token(NewLine) |\r\n|
  }
//@[002:00003) |   |   └─Token(RightBrace) |}|
//@[003:00005) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

resource resWithDependencies 'Mock.Rp/mockResource@2020-01-01' = {
//@[000:00233) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00028) | ├─IdentifierSyntax
//@[009:00028) | | └─Token(Identifier) |resWithDependencies|
//@[029:00062) | ├─StringSyntax
//@[029:00062) | | └─Token(StringComplete) |'Mock.Rp/mockResource@2020-01-01'|
//@[063:00064) | ├─Token(Assignment) |=|
//@[065:00233) | └─ObjectSyntax
//@[065:00066) |   ├─Token(LeftBrace) |{|
//@[066:00068) |   ├─Token(NewLine) |\r\n|
  name: 'harry'
//@[002:00015) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00015) |   | └─StringSyntax
//@[008:00015) |   |   └─Token(StringComplete) |'harry'|
//@[015:00017) |   ├─Token(NewLine) |\r\n|
  properties: {
//@[002:00145) |   ├─ObjectPropertySyntax
//@[002:00012) |   | ├─IdentifierSyntax
//@[002:00012) |   | | └─Token(Identifier) |properties|
//@[012:00013) |   | ├─Token(Colon) |:|
//@[014:00145) |   | └─ObjectSyntax
//@[014:00015) |   |   ├─Token(LeftBrace) |{|
//@[015:00017) |   |   ├─Token(NewLine) |\r\n|
    modADep: modATest.outputs.stringOutputA
//@[004:00043) |   |   ├─ObjectPropertySyntax
//@[004:00011) |   |   | ├─IdentifierSyntax
//@[004:00011) |   |   | | └─Token(Identifier) |modADep|
//@[011:00012) |   |   | ├─Token(Colon) |:|
//@[013:00043) |   |   | └─PropertyAccessSyntax
//@[013:00029) |   |   |   ├─PropertyAccessSyntax
//@[013:00021) |   |   |   | ├─VariableAccessSyntax
//@[013:00021) |   |   |   | | └─IdentifierSyntax
//@[013:00021) |   |   |   | |   └─Token(Identifier) |modATest|
//@[021:00022) |   |   |   | ├─Token(Dot) |.|
//@[022:00029) |   |   |   | └─IdentifierSyntax
//@[022:00029) |   |   |   |   └─Token(Identifier) |outputs|
//@[029:00030) |   |   |   ├─Token(Dot) |.|
//@[030:00043) |   |   |   └─IdentifierSyntax
//@[030:00043) |   |   |     └─Token(Identifier) |stringOutputA|
//@[043:00045) |   |   ├─Token(NewLine) |\r\n|
    modBDep: modB.outputs.myResourceId
//@[004:00038) |   |   ├─ObjectPropertySyntax
//@[004:00011) |   |   | ├─IdentifierSyntax
//@[004:00011) |   |   | | └─Token(Identifier) |modBDep|
//@[011:00012) |   |   | ├─Token(Colon) |:|
//@[013:00038) |   |   | └─PropertyAccessSyntax
//@[013:00025) |   |   |   ├─PropertyAccessSyntax
//@[013:00017) |   |   |   | ├─VariableAccessSyntax
//@[013:00017) |   |   |   | | └─IdentifierSyntax
//@[013:00017) |   |   |   | |   └─Token(Identifier) |modB|
//@[017:00018) |   |   |   | ├─Token(Dot) |.|
//@[018:00025) |   |   |   | └─IdentifierSyntax
//@[018:00025) |   |   |   |   └─Token(Identifier) |outputs|
//@[025:00026) |   |   |   ├─Token(Dot) |.|
//@[026:00038) |   |   |   └─IdentifierSyntax
//@[026:00038) |   |   |     └─Token(Identifier) |myResourceId|
//@[038:00040) |   |   ├─Token(NewLine) |\r\n|
    modCDep: modC.outputs.myResourceId
//@[004:00038) |   |   ├─ObjectPropertySyntax
//@[004:00011) |   |   | ├─IdentifierSyntax
//@[004:00011) |   |   | | └─Token(Identifier) |modCDep|
//@[011:00012) |   |   | ├─Token(Colon) |:|
//@[013:00038) |   |   | └─PropertyAccessSyntax
//@[013:00025) |   |   |   ├─PropertyAccessSyntax
//@[013:00017) |   |   |   | ├─VariableAccessSyntax
//@[013:00017) |   |   |   | | └─IdentifierSyntax
//@[013:00017) |   |   |   | |   └─Token(Identifier) |modC|
//@[017:00018) |   |   |   | ├─Token(Dot) |.|
//@[018:00025) |   |   |   | └─IdentifierSyntax
//@[018:00025) |   |   |   |   └─Token(Identifier) |outputs|
//@[025:00026) |   |   |   ├─Token(Dot) |.|
//@[026:00038) |   |   |   └─IdentifierSyntax
//@[026:00038) |   |   |     └─Token(Identifier) |myResourceId|
//@[038:00040) |   |   ├─Token(NewLine) |\r\n|
  }
//@[002:00003) |   |   └─Token(RightBrace) |}|
//@[003:00005) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

module optionalWithAllParamsAndManualDependency './child/optionalParams.bicep'= {
//@[000:00321) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00047) | ├─IdentifierSyntax
//@[007:00047) | | └─Token(Identifier) |optionalWithAllParamsAndManualDependency|
//@[048:00078) | ├─StringSyntax
//@[048:00078) | | └─Token(StringComplete) |'./child/optionalParams.bicep'|
//@[078:00079) | ├─Token(Assignment) |=|
//@[080:00321) | └─ObjectSyntax
//@[080:00081) |   ├─Token(LeftBrace) |{|
//@[081:00083) |   ├─Token(NewLine) |\r\n|
  name: 'optionalWithAllParamsAndManualDependency'
//@[002:00050) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00050) |   | └─StringSyntax
//@[008:00050) |   |   └─Token(StringComplete) |'optionalWithAllParamsAndManualDependency'|
//@[050:00052) |   ├─Token(NewLine) |\r\n|
  params: {
//@[002:00110) |   ├─ObjectPropertySyntax
//@[002:00008) |   | ├─IdentifierSyntax
//@[002:00008) |   | | └─Token(Identifier) |params|
//@[008:00009) |   | ├─Token(Colon) |:|
//@[010:00110) |   | └─ObjectSyntax
//@[010:00011) |   |   ├─Token(LeftBrace) |{|
//@[011:00013) |   |   ├─Token(NewLine) |\r\n|
    optionalString: 'abc'
//@[004:00025) |   |   ├─ObjectPropertySyntax
//@[004:00018) |   |   | ├─IdentifierSyntax
//@[004:00018) |   |   | | └─Token(Identifier) |optionalString|
//@[018:00019) |   |   | ├─Token(Colon) |:|
//@[020:00025) |   |   | └─StringSyntax
//@[020:00025) |   |   |   └─Token(StringComplete) |'abc'|
//@[025:00027) |   |   ├─Token(NewLine) |\r\n|
    optionalInt: 42
//@[004:00019) |   |   ├─ObjectPropertySyntax
//@[004:00015) |   |   | ├─IdentifierSyntax
//@[004:00015) |   |   | | └─Token(Identifier) |optionalInt|
//@[015:00016) |   |   | ├─Token(Colon) |:|
//@[017:00019) |   |   | └─IntegerLiteralSyntax
//@[017:00019) |   |   |   └─Token(Integer) |42|
//@[019:00021) |   |   ├─Token(NewLine) |\r\n|
    optionalObj: { }
//@[004:00020) |   |   ├─ObjectPropertySyntax
//@[004:00015) |   |   | ├─IdentifierSyntax
//@[004:00015) |   |   | | └─Token(Identifier) |optionalObj|
//@[015:00016) |   |   | ├─Token(Colon) |:|
//@[017:00020) |   |   | └─ObjectSyntax
//@[017:00018) |   |   |   ├─Token(LeftBrace) |{|
//@[019:00020) |   |   |   └─Token(RightBrace) |}|
//@[020:00022) |   |   ├─Token(NewLine) |\r\n|
    optionalArray: [ ]
//@[004:00022) |   |   ├─ObjectPropertySyntax
//@[004:00017) |   |   | ├─IdentifierSyntax
//@[004:00017) |   |   | | └─Token(Identifier) |optionalArray|
//@[017:00018) |   |   | ├─Token(Colon) |:|
//@[019:00022) |   |   | └─ArraySyntax
//@[019:00020) |   |   |   ├─Token(LeftSquare) |[|
//@[021:00022) |   |   |   └─Token(RightSquare) |]|
//@[022:00024) |   |   ├─Token(NewLine) |\r\n|
  }
//@[002:00003) |   |   └─Token(RightBrace) |}|
//@[003:00005) |   ├─Token(NewLine) |\r\n|
  dependsOn: [
//@[002:00071) |   ├─ObjectPropertySyntax
//@[002:00011) |   | ├─IdentifierSyntax
//@[002:00011) |   | | └─Token(Identifier) |dependsOn|
//@[011:00012) |   | ├─Token(Colon) |:|
//@[013:00071) |   | └─ArraySyntax
//@[013:00014) |   |   ├─Token(LeftSquare) |[|
//@[014:00016) |   |   ├─Token(NewLine) |\r\n|
    resWithDependencies
//@[004:00023) |   |   ├─ArrayItemSyntax
//@[004:00023) |   |   | └─VariableAccessSyntax
//@[004:00023) |   |   |   └─IdentifierSyntax
//@[004:00023) |   |   |     └─Token(Identifier) |resWithDependencies|
//@[023:00025) |   |   ├─Token(NewLine) |\r\n|
    optionalWithAllParams
//@[004:00025) |   |   ├─ArrayItemSyntax
//@[004:00025) |   |   | └─VariableAccessSyntax
//@[004:00025) |   |   |   └─IdentifierSyntax
//@[004:00025) |   |   |     └─Token(Identifier) |optionalWithAllParams|
//@[025:00027) |   |   ├─Token(NewLine) |\r\n|
  ]
//@[002:00003) |   |   └─Token(RightSquare) |]|
//@[003:00005) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

module optionalWithImplicitDependency './child/optionalParams.bicep'= {
//@[000:00300) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00037) | ├─IdentifierSyntax
//@[007:00037) | | └─Token(Identifier) |optionalWithImplicitDependency|
//@[038:00068) | ├─StringSyntax
//@[038:00068) | | └─Token(StringComplete) |'./child/optionalParams.bicep'|
//@[068:00069) | ├─Token(Assignment) |=|
//@[070:00300) | └─ObjectSyntax
//@[070:00071) |   ├─Token(LeftBrace) |{|
//@[071:00073) |   ├─Token(NewLine) |\r\n|
  name: 'optionalWithImplicitDependency'
//@[002:00040) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00040) |   | └─StringSyntax
//@[008:00040) |   |   └─Token(StringComplete) |'optionalWithImplicitDependency'|
//@[040:00042) |   ├─Token(NewLine) |\r\n|
  params: {
//@[002:00182) |   ├─ObjectPropertySyntax
//@[002:00008) |   | ├─IdentifierSyntax
//@[002:00008) |   | | └─Token(Identifier) |params|
//@[008:00009) |   | ├─Token(Colon) |:|
//@[010:00182) |   | └─ObjectSyntax
//@[010:00011) |   |   ├─Token(LeftBrace) |{|
//@[011:00013) |   |   ├─Token(NewLine) |\r\n|
    optionalString: concat(resWithDependencies.id, optionalWithAllParamsAndManualDependency.name)
//@[004:00097) |   |   ├─ObjectPropertySyntax
//@[004:00018) |   |   | ├─IdentifierSyntax
//@[004:00018) |   |   | | └─Token(Identifier) |optionalString|
//@[018:00019) |   |   | ├─Token(Colon) |:|
//@[020:00097) |   |   | └─FunctionCallSyntax
//@[020:00026) |   |   |   ├─IdentifierSyntax
//@[020:00026) |   |   |   | └─Token(Identifier) |concat|
//@[026:00027) |   |   |   ├─Token(LeftParen) |(|
//@[027:00049) |   |   |   ├─FunctionArgumentSyntax
//@[027:00049) |   |   |   | └─PropertyAccessSyntax
//@[027:00046) |   |   |   |   ├─VariableAccessSyntax
//@[027:00046) |   |   |   |   | └─IdentifierSyntax
//@[027:00046) |   |   |   |   |   └─Token(Identifier) |resWithDependencies|
//@[046:00047) |   |   |   |   ├─Token(Dot) |.|
//@[047:00049) |   |   |   |   └─IdentifierSyntax
//@[047:00049) |   |   |   |     └─Token(Identifier) |id|
//@[049:00050) |   |   |   ├─Token(Comma) |,|
//@[051:00096) |   |   |   ├─FunctionArgumentSyntax
//@[051:00096) |   |   |   | └─PropertyAccessSyntax
//@[051:00091) |   |   |   |   ├─VariableAccessSyntax
//@[051:00091) |   |   |   |   | └─IdentifierSyntax
//@[051:00091) |   |   |   |   |   └─Token(Identifier) |optionalWithAllParamsAndManualDependency|
//@[091:00092) |   |   |   |   ├─Token(Dot) |.|
//@[092:00096) |   |   |   |   └─IdentifierSyntax
//@[092:00096) |   |   |   |     └─Token(Identifier) |name|
//@[096:00097) |   |   |   └─Token(RightParen) |)|
//@[097:00099) |   |   ├─Token(NewLine) |\r\n|
    optionalInt: 42
//@[004:00019) |   |   ├─ObjectPropertySyntax
//@[004:00015) |   |   | ├─IdentifierSyntax
//@[004:00015) |   |   | | └─Token(Identifier) |optionalInt|
//@[015:00016) |   |   | ├─Token(Colon) |:|
//@[017:00019) |   |   | └─IntegerLiteralSyntax
//@[017:00019) |   |   |   └─Token(Integer) |42|
//@[019:00021) |   |   ├─Token(NewLine) |\r\n|
    optionalObj: { }
//@[004:00020) |   |   ├─ObjectPropertySyntax
//@[004:00015) |   |   | ├─IdentifierSyntax
//@[004:00015) |   |   | | └─Token(Identifier) |optionalObj|
//@[015:00016) |   |   | ├─Token(Colon) |:|
//@[017:00020) |   |   | └─ObjectSyntax
//@[017:00018) |   |   |   ├─Token(LeftBrace) |{|
//@[019:00020) |   |   |   └─Token(RightBrace) |}|
//@[020:00022) |   |   ├─Token(NewLine) |\r\n|
    optionalArray: [ ]
//@[004:00022) |   |   ├─ObjectPropertySyntax
//@[004:00017) |   |   | ├─IdentifierSyntax
//@[004:00017) |   |   | | └─Token(Identifier) |optionalArray|
//@[017:00018) |   |   | ├─Token(Colon) |:|
//@[019:00022) |   |   | └─ArraySyntax
//@[019:00020) |   |   |   ├─Token(LeftSquare) |[|
//@[021:00022) |   |   |   └─Token(RightSquare) |]|
//@[022:00024) |   |   ├─Token(NewLine) |\r\n|
  }
//@[002:00003) |   |   └─Token(RightBrace) |}|
//@[003:00005) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

module moduleWithCalculatedName './child/optionalParams.bicep'= {
//@[000:00331) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00031) | ├─IdentifierSyntax
//@[007:00031) | | └─Token(Identifier) |moduleWithCalculatedName|
//@[032:00062) | ├─StringSyntax
//@[032:00062) | | └─Token(StringComplete) |'./child/optionalParams.bicep'|
//@[062:00063) | ├─Token(Assignment) |=|
//@[064:00331) | └─ObjectSyntax
//@[064:00065) |   ├─Token(LeftBrace) |{|
//@[065:00067) |   ├─Token(NewLine) |\r\n|
  name: '${optionalWithAllParamsAndManualDependency.name}${deployTimeSuffix}'
//@[002:00077) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00077) |   | └─StringSyntax
//@[008:00011) |   |   ├─Token(StringLeftPiece) |'${|
//@[011:00056) |   |   ├─PropertyAccessSyntax
//@[011:00051) |   |   | ├─VariableAccessSyntax
//@[011:00051) |   |   | | └─IdentifierSyntax
//@[011:00051) |   |   | |   └─Token(Identifier) |optionalWithAllParamsAndManualDependency|
//@[051:00052) |   |   | ├─Token(Dot) |.|
//@[052:00056) |   |   | └─IdentifierSyntax
//@[052:00056) |   |   |   └─Token(Identifier) |name|
//@[056:00059) |   |   ├─Token(StringMiddlePiece) |}${|
//@[059:00075) |   |   ├─VariableAccessSyntax
//@[059:00075) |   |   | └─IdentifierSyntax
//@[059:00075) |   |   |   └─Token(Identifier) |deployTimeSuffix|
//@[075:00077) |   |   └─Token(StringRightPiece) |}'|
//@[077:00079) |   ├─Token(NewLine) |\r\n|
  params: {
//@[002:00182) |   ├─ObjectPropertySyntax
//@[002:00008) |   | ├─IdentifierSyntax
//@[002:00008) |   | | └─Token(Identifier) |params|
//@[008:00009) |   | ├─Token(Colon) |:|
//@[010:00182) |   | └─ObjectSyntax
//@[010:00011) |   |   ├─Token(LeftBrace) |{|
//@[011:00013) |   |   ├─Token(NewLine) |\r\n|
    optionalString: concat(resWithDependencies.id, optionalWithAllParamsAndManualDependency.name)
//@[004:00097) |   |   ├─ObjectPropertySyntax
//@[004:00018) |   |   | ├─IdentifierSyntax
//@[004:00018) |   |   | | └─Token(Identifier) |optionalString|
//@[018:00019) |   |   | ├─Token(Colon) |:|
//@[020:00097) |   |   | └─FunctionCallSyntax
//@[020:00026) |   |   |   ├─IdentifierSyntax
//@[020:00026) |   |   |   | └─Token(Identifier) |concat|
//@[026:00027) |   |   |   ├─Token(LeftParen) |(|
//@[027:00049) |   |   |   ├─FunctionArgumentSyntax
//@[027:00049) |   |   |   | └─PropertyAccessSyntax
//@[027:00046) |   |   |   |   ├─VariableAccessSyntax
//@[027:00046) |   |   |   |   | └─IdentifierSyntax
//@[027:00046) |   |   |   |   |   └─Token(Identifier) |resWithDependencies|
//@[046:00047) |   |   |   |   ├─Token(Dot) |.|
//@[047:00049) |   |   |   |   └─IdentifierSyntax
//@[047:00049) |   |   |   |     └─Token(Identifier) |id|
//@[049:00050) |   |   |   ├─Token(Comma) |,|
//@[051:00096) |   |   |   ├─FunctionArgumentSyntax
//@[051:00096) |   |   |   | └─PropertyAccessSyntax
//@[051:00091) |   |   |   |   ├─VariableAccessSyntax
//@[051:00091) |   |   |   |   | └─IdentifierSyntax
//@[051:00091) |   |   |   |   |   └─Token(Identifier) |optionalWithAllParamsAndManualDependency|
//@[091:00092) |   |   |   |   ├─Token(Dot) |.|
//@[092:00096) |   |   |   |   └─IdentifierSyntax
//@[092:00096) |   |   |   |     └─Token(Identifier) |name|
//@[096:00097) |   |   |   └─Token(RightParen) |)|
//@[097:00099) |   |   ├─Token(NewLine) |\r\n|
    optionalInt: 42
//@[004:00019) |   |   ├─ObjectPropertySyntax
//@[004:00015) |   |   | ├─IdentifierSyntax
//@[004:00015) |   |   | | └─Token(Identifier) |optionalInt|
//@[015:00016) |   |   | ├─Token(Colon) |:|
//@[017:00019) |   |   | └─IntegerLiteralSyntax
//@[017:00019) |   |   |   └─Token(Integer) |42|
//@[019:00021) |   |   ├─Token(NewLine) |\r\n|
    optionalObj: { }
//@[004:00020) |   |   ├─ObjectPropertySyntax
//@[004:00015) |   |   | ├─IdentifierSyntax
//@[004:00015) |   |   | | └─Token(Identifier) |optionalObj|
//@[015:00016) |   |   | ├─Token(Colon) |:|
//@[017:00020) |   |   | └─ObjectSyntax
//@[017:00018) |   |   |   ├─Token(LeftBrace) |{|
//@[019:00020) |   |   |   └─Token(RightBrace) |}|
//@[020:00022) |   |   ├─Token(NewLine) |\r\n|
    optionalArray: [ ]
//@[004:00022) |   |   ├─ObjectPropertySyntax
//@[004:00017) |   |   | ├─IdentifierSyntax
//@[004:00017) |   |   | | └─Token(Identifier) |optionalArray|
//@[017:00018) |   |   | ├─Token(Colon) |:|
//@[019:00022) |   |   | └─ArraySyntax
//@[019:00020) |   |   |   ├─Token(LeftSquare) |[|
//@[021:00022) |   |   |   └─Token(RightSquare) |]|
//@[022:00024) |   |   ├─Token(NewLine) |\r\n|
  }
//@[002:00003) |   |   └─Token(RightBrace) |}|
//@[003:00005) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

resource resWithCalculatedNameDependencies 'Mock.Rp/mockResource@2020-01-01' = {
//@[000:00241) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00042) | ├─IdentifierSyntax
//@[009:00042) | | └─Token(Identifier) |resWithCalculatedNameDependencies|
//@[043:00076) | ├─StringSyntax
//@[043:00076) | | └─Token(StringComplete) |'Mock.Rp/mockResource@2020-01-01'|
//@[077:00078) | ├─Token(Assignment) |=|
//@[079:00241) | └─ObjectSyntax
//@[079:00080) |   ├─Token(LeftBrace) |{|
//@[080:00082) |   ├─Token(NewLine) |\r\n|
  name: '${optionalWithAllParamsAndManualDependency.name}${deployTimeSuffix}'
//@[002:00077) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00077) |   | └─StringSyntax
//@[008:00011) |   |   ├─Token(StringLeftPiece) |'${|
//@[011:00056) |   |   ├─PropertyAccessSyntax
//@[011:00051) |   |   | ├─VariableAccessSyntax
//@[011:00051) |   |   | | └─IdentifierSyntax
//@[011:00051) |   |   | |   └─Token(Identifier) |optionalWithAllParamsAndManualDependency|
//@[051:00052) |   |   | ├─Token(Dot) |.|
//@[052:00056) |   |   | └─IdentifierSyntax
//@[052:00056) |   |   |   └─Token(Identifier) |name|
//@[056:00059) |   |   ├─Token(StringMiddlePiece) |}${|
//@[059:00075) |   |   ├─VariableAccessSyntax
//@[059:00075) |   |   | └─IdentifierSyntax
//@[059:00075) |   |   |   └─Token(Identifier) |deployTimeSuffix|
//@[075:00077) |   |   └─Token(StringRightPiece) |}'|
//@[077:00079) |   ├─Token(NewLine) |\r\n|
  properties: {
//@[002:00077) |   ├─ObjectPropertySyntax
//@[002:00012) |   | ├─IdentifierSyntax
//@[002:00012) |   | | └─Token(Identifier) |properties|
//@[012:00013) |   | ├─Token(Colon) |:|
//@[014:00077) |   | └─ObjectSyntax
//@[014:00015) |   |   ├─Token(LeftBrace) |{|
//@[015:00017) |   |   ├─Token(NewLine) |\r\n|
    modADep: moduleWithCalculatedName.outputs.outputObj
//@[004:00055) |   |   ├─ObjectPropertySyntax
//@[004:00011) |   |   | ├─IdentifierSyntax
//@[004:00011) |   |   | | └─Token(Identifier) |modADep|
//@[011:00012) |   |   | ├─Token(Colon) |:|
//@[013:00055) |   |   | └─PropertyAccessSyntax
//@[013:00045) |   |   |   ├─PropertyAccessSyntax
//@[013:00037) |   |   |   | ├─VariableAccessSyntax
//@[013:00037) |   |   |   | | └─IdentifierSyntax
//@[013:00037) |   |   |   | |   └─Token(Identifier) |moduleWithCalculatedName|
//@[037:00038) |   |   |   | ├─Token(Dot) |.|
//@[038:00045) |   |   |   | └─IdentifierSyntax
//@[038:00045) |   |   |   |   └─Token(Identifier) |outputs|
//@[045:00046) |   |   |   ├─Token(Dot) |.|
//@[046:00055) |   |   |   └─IdentifierSyntax
//@[046:00055) |   |   |     └─Token(Identifier) |outputObj|
//@[055:00057) |   |   ├─Token(NewLine) |\r\n|
  }
//@[002:00003) |   |   └─Token(RightBrace) |}|
//@[003:00005) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

output stringOutputA string = modATest.outputs.stringOutputA
//@[000:00060) ├─OutputDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |output|
//@[007:00020) | ├─IdentifierSyntax
//@[007:00020) | | └─Token(Identifier) |stringOutputA|
//@[021:00027) | ├─TypeVariableAccessSyntax
//@[021:00027) | | └─IdentifierSyntax
//@[021:00027) | |   └─Token(Identifier) |string|
//@[028:00029) | ├─Token(Assignment) |=|
//@[030:00060) | └─PropertyAccessSyntax
//@[030:00046) |   ├─PropertyAccessSyntax
//@[030:00038) |   | ├─VariableAccessSyntax
//@[030:00038) |   | | └─IdentifierSyntax
//@[030:00038) |   | |   └─Token(Identifier) |modATest|
//@[038:00039) |   | ├─Token(Dot) |.|
//@[039:00046) |   | └─IdentifierSyntax
//@[039:00046) |   |   └─Token(Identifier) |outputs|
//@[046:00047) |   ├─Token(Dot) |.|
//@[047:00060) |   └─IdentifierSyntax
//@[047:00060) |     └─Token(Identifier) |stringOutputA|
//@[060:00062) ├─Token(NewLine) |\r\n|
output stringOutputB string = modATest.outputs.stringOutputB
//@[000:00060) ├─OutputDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |output|
//@[007:00020) | ├─IdentifierSyntax
//@[007:00020) | | └─Token(Identifier) |stringOutputB|
//@[021:00027) | ├─TypeVariableAccessSyntax
//@[021:00027) | | └─IdentifierSyntax
//@[021:00027) | |   └─Token(Identifier) |string|
//@[028:00029) | ├─Token(Assignment) |=|
//@[030:00060) | └─PropertyAccessSyntax
//@[030:00046) |   ├─PropertyAccessSyntax
//@[030:00038) |   | ├─VariableAccessSyntax
//@[030:00038) |   | | └─IdentifierSyntax
//@[030:00038) |   | |   └─Token(Identifier) |modATest|
//@[038:00039) |   | ├─Token(Dot) |.|
//@[039:00046) |   | └─IdentifierSyntax
//@[039:00046) |   |   └─Token(Identifier) |outputs|
//@[046:00047) |   ├─Token(Dot) |.|
//@[047:00060) |   └─IdentifierSyntax
//@[047:00060) |     └─Token(Identifier) |stringOutputB|
//@[060:00062) ├─Token(NewLine) |\r\n|
output objOutput object = modATest.outputs.objOutput
//@[000:00052) ├─OutputDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |output|
//@[007:00016) | ├─IdentifierSyntax
//@[007:00016) | | └─Token(Identifier) |objOutput|
//@[017:00023) | ├─TypeVariableAccessSyntax
//@[017:00023) | | └─IdentifierSyntax
//@[017:00023) | |   └─Token(Identifier) |object|
//@[024:00025) | ├─Token(Assignment) |=|
//@[026:00052) | └─PropertyAccessSyntax
//@[026:00042) |   ├─PropertyAccessSyntax
//@[026:00034) |   | ├─VariableAccessSyntax
//@[026:00034) |   | | └─IdentifierSyntax
//@[026:00034) |   | |   └─Token(Identifier) |modATest|
//@[034:00035) |   | ├─Token(Dot) |.|
//@[035:00042) |   | └─IdentifierSyntax
//@[035:00042) |   |   └─Token(Identifier) |outputs|
//@[042:00043) |   ├─Token(Dot) |.|
//@[043:00052) |   └─IdentifierSyntax
//@[043:00052) |     └─Token(Identifier) |objOutput|
//@[052:00054) ├─Token(NewLine) |\r\n|
output arrayOutput array = modATest.outputs.arrayOutput
//@[000:00055) ├─OutputDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |output|
//@[007:00018) | ├─IdentifierSyntax
//@[007:00018) | | └─Token(Identifier) |arrayOutput|
//@[019:00024) | ├─TypeVariableAccessSyntax
//@[019:00024) | | └─IdentifierSyntax
//@[019:00024) | |   └─Token(Identifier) |array|
//@[025:00026) | ├─Token(Assignment) |=|
//@[027:00055) | └─PropertyAccessSyntax
//@[027:00043) |   ├─PropertyAccessSyntax
//@[027:00035) |   | ├─VariableAccessSyntax
//@[027:00035) |   | | └─IdentifierSyntax
//@[027:00035) |   | |   └─Token(Identifier) |modATest|
//@[035:00036) |   | ├─Token(Dot) |.|
//@[036:00043) |   | └─IdentifierSyntax
//@[036:00043) |   |   └─Token(Identifier) |outputs|
//@[043:00044) |   ├─Token(Dot) |.|
//@[044:00055) |   └─IdentifierSyntax
//@[044:00055) |     └─Token(Identifier) |arrayOutput|
//@[055:00057) ├─Token(NewLine) |\r\n|
output modCalculatedNameOutput object = moduleWithCalculatedName.outputs.outputObj
//@[000:00082) ├─OutputDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |output|
//@[007:00030) | ├─IdentifierSyntax
//@[007:00030) | | └─Token(Identifier) |modCalculatedNameOutput|
//@[031:00037) | ├─TypeVariableAccessSyntax
//@[031:00037) | | └─IdentifierSyntax
//@[031:00037) | |   └─Token(Identifier) |object|
//@[038:00039) | ├─Token(Assignment) |=|
//@[040:00082) | └─PropertyAccessSyntax
//@[040:00072) |   ├─PropertyAccessSyntax
//@[040:00064) |   | ├─VariableAccessSyntax
//@[040:00064) |   | | └─IdentifierSyntax
//@[040:00064) |   | |   └─Token(Identifier) |moduleWithCalculatedName|
//@[064:00065) |   | ├─Token(Dot) |.|
//@[065:00072) |   | └─IdentifierSyntax
//@[065:00072) |   |   └─Token(Identifier) |outputs|
//@[072:00073) |   ├─Token(Dot) |.|
//@[073:00082) |   └─IdentifierSyntax
//@[073:00082) |     └─Token(Identifier) |outputObj|
//@[082:00086) ├─Token(NewLine) |\r\n\r\n|

/*
  valid loop cases
*/
//@[002:00006) ├─Token(NewLine) |\r\n\r\n|

@sys.description('this is myModules')
//@[000:00162) ├─VariableDeclarationSyntax
//@[000:00037) | ├─DecoratorSyntax
//@[000:00001) | | ├─Token(At) |@|
//@[001:00037) | | └─InstanceFunctionCallSyntax
//@[001:00004) | |   ├─VariableAccessSyntax
//@[001:00004) | |   | └─IdentifierSyntax
//@[001:00004) | |   |   └─Token(Identifier) |sys|
//@[004:00005) | |   ├─Token(Dot) |.|
//@[005:00016) | |   ├─IdentifierSyntax
//@[005:00016) | |   | └─Token(Identifier) |description|
//@[016:00017) | |   ├─Token(LeftParen) |(|
//@[017:00036) | |   ├─FunctionArgumentSyntax
//@[017:00036) | |   | └─StringSyntax
//@[017:00036) | |   |   └─Token(StringComplete) |'this is myModules'|
//@[036:00037) | |   └─Token(RightParen) |)|
//@[037:00039) | ├─Token(NewLine) |\r\n|
var myModules = [
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00013) | ├─IdentifierSyntax
//@[004:00013) | | └─Token(Identifier) |myModules|
//@[014:00015) | ├─Token(Assignment) |=|
//@[016:00123) | └─ArraySyntax
//@[016:00017) |   ├─Token(LeftSquare) |[|
//@[017:00019) |   ├─Token(NewLine) |\r\n|
  {
//@[002:00050) |   ├─ArrayItemSyntax
//@[002:00050) |   | └─ObjectSyntax
//@[002:00003) |   |   ├─Token(LeftBrace) |{|
//@[003:00005) |   |   ├─Token(NewLine) |\r\n|
    name: 'one'
//@[004:00015) |   |   ├─ObjectPropertySyntax
//@[004:00008) |   |   | ├─IdentifierSyntax
//@[004:00008) |   |   | | └─Token(Identifier) |name|
//@[008:00009) |   |   | ├─Token(Colon) |:|
//@[010:00015) |   |   | └─StringSyntax
//@[010:00015) |   |   |   └─Token(StringComplete) |'one'|
//@[015:00017) |   |   ├─Token(NewLine) |\r\n|
    location: 'eastus2'
//@[004:00023) |   |   ├─ObjectPropertySyntax
//@[004:00012) |   |   | ├─IdentifierSyntax
//@[004:00012) |   |   | | └─Token(Identifier) |location|
//@[012:00013) |   |   | ├─Token(Colon) |:|
//@[014:00023) |   |   | └─StringSyntax
//@[014:00023) |   |   |   └─Token(StringComplete) |'eastus2'|
//@[023:00025) |   |   ├─Token(NewLine) |\r\n|
  }
//@[002:00003) |   |   └─Token(RightBrace) |}|
//@[003:00005) |   ├─Token(NewLine) |\r\n|
  {
//@[002:00049) |   ├─ArrayItemSyntax
//@[002:00049) |   | └─ObjectSyntax
//@[002:00003) |   |   ├─Token(LeftBrace) |{|
//@[003:00005) |   |   ├─Token(NewLine) |\r\n|
    name: 'two'
//@[004:00015) |   |   ├─ObjectPropertySyntax
//@[004:00008) |   |   | ├─IdentifierSyntax
//@[004:00008) |   |   | | └─Token(Identifier) |name|
//@[008:00009) |   |   | ├─Token(Colon) |:|
//@[010:00015) |   |   | └─StringSyntax
//@[010:00015) |   |   |   └─Token(StringComplete) |'two'|
//@[015:00017) |   |   ├─Token(NewLine) |\r\n|
    location: 'westus'
//@[004:00022) |   |   ├─ObjectPropertySyntax
//@[004:00012) |   |   | ├─IdentifierSyntax
//@[004:00012) |   |   | | └─Token(Identifier) |location|
//@[012:00013) |   |   | ├─Token(Colon) |:|
//@[014:00022) |   |   | └─StringSyntax
//@[014:00022) |   |   |   └─Token(StringComplete) |'westus'|
//@[022:00024) |   |   ├─Token(NewLine) |\r\n|
  }
//@[002:00003) |   |   └─Token(RightBrace) |}|
//@[003:00005) |   ├─Token(NewLine) |\r\n|
]
//@[000:00001) |   └─Token(RightSquare) |]|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

var emptyArray = []
//@[000:00019) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00014) | ├─IdentifierSyntax
//@[004:00014) | | └─Token(Identifier) |emptyArray|
//@[015:00016) | ├─Token(Assignment) |=|
//@[017:00019) | └─ArraySyntax
//@[017:00018) |   ├─Token(LeftSquare) |[|
//@[018:00019) |   └─Token(RightSquare) |]|
//@[019:00023) ├─Token(NewLine) |\r\n\r\n|

// simple module loop
//@[021:00023) ├─Token(NewLine) |\r\n|
module storageResources 'modulea.bicep' = [for module in myModules: {
//@[000:00189) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00023) | ├─IdentifierSyntax
//@[007:00023) | | └─Token(Identifier) |storageResources|
//@[024:00039) | ├─StringSyntax
//@[024:00039) | | └─Token(StringComplete) |'modulea.bicep'|
//@[040:00041) | ├─Token(Assignment) |=|
//@[042:00189) | └─ForSyntax
//@[042:00043) |   ├─Token(LeftSquare) |[|
//@[043:00046) |   ├─Token(Identifier) |for|
//@[047:00053) |   ├─LocalVariableSyntax
//@[047:00053) |   | └─IdentifierSyntax
//@[047:00053) |   |   └─Token(Identifier) |module|
//@[054:00056) |   ├─Token(Identifier) |in|
//@[057:00066) |   ├─VariableAccessSyntax
//@[057:00066) |   | └─IdentifierSyntax
//@[057:00066) |   |   └─Token(Identifier) |myModules|
//@[066:00067) |   ├─Token(Colon) |:|
//@[068:00188) |   ├─ObjectSyntax
//@[068:00069) |   | ├─Token(LeftBrace) |{|
//@[069:00071) |   | ├─Token(NewLine) |\r\n|
  name: module.name
//@[002:00019) |   | ├─ObjectPropertySyntax
//@[002:00006) |   | | ├─IdentifierSyntax
//@[002:00006) |   | | | └─Token(Identifier) |name|
//@[006:00007) |   | | ├─Token(Colon) |:|
//@[008:00019) |   | | └─PropertyAccessSyntax
//@[008:00014) |   | |   ├─VariableAccessSyntax
//@[008:00014) |   | |   | └─IdentifierSyntax
//@[008:00014) |   | |   |   └─Token(Identifier) |module|
//@[014:00015) |   | |   ├─Token(Dot) |.|
//@[015:00019) |   | |   └─IdentifierSyntax
//@[015:00019) |   | |     └─Token(Identifier) |name|
//@[019:00021) |   | ├─Token(NewLine) |\r\n|
  params: {
//@[002:00093) |   | ├─ObjectPropertySyntax
//@[002:00008) |   | | ├─IdentifierSyntax
//@[002:00008) |   | | | └─Token(Identifier) |params|
//@[008:00009) |   | | ├─Token(Colon) |:|
//@[010:00093) |   | | └─ObjectSyntax
//@[010:00011) |   | |   ├─Token(LeftBrace) |{|
//@[011:00013) |   | |   ├─Token(NewLine) |\r\n|
    arrayParam: []
//@[004:00018) |   | |   ├─ObjectPropertySyntax
//@[004:00014) |   | |   | ├─IdentifierSyntax
//@[004:00014) |   | |   | | └─Token(Identifier) |arrayParam|
//@[014:00015) |   | |   | ├─Token(Colon) |:|
//@[016:00018) |   | |   | └─ArraySyntax
//@[016:00017) |   | |   |   ├─Token(LeftSquare) |[|
//@[017:00018) |   | |   |   └─Token(RightSquare) |]|
//@[018:00020) |   | |   ├─Token(NewLine) |\r\n|
    objParam: module
//@[004:00020) |   | |   ├─ObjectPropertySyntax
//@[004:00012) |   | |   | ├─IdentifierSyntax
//@[004:00012) |   | |   | | └─Token(Identifier) |objParam|
//@[012:00013) |   | |   | ├─Token(Colon) |:|
//@[014:00020) |   | |   | └─VariableAccessSyntax
//@[014:00020) |   | |   |   └─IdentifierSyntax
//@[014:00020) |   | |   |     └─Token(Identifier) |module|
//@[020:00022) |   | |   ├─Token(NewLine) |\r\n|
    stringParamB: module.location
//@[004:00033) |   | |   ├─ObjectPropertySyntax
//@[004:00016) |   | |   | ├─IdentifierSyntax
//@[004:00016) |   | |   | | └─Token(Identifier) |stringParamB|
//@[016:00017) |   | |   | ├─Token(Colon) |:|
//@[018:00033) |   | |   | └─PropertyAccessSyntax
//@[018:00024) |   | |   |   ├─VariableAccessSyntax
//@[018:00024) |   | |   |   | └─IdentifierSyntax
//@[018:00024) |   | |   |   |   └─Token(Identifier) |module|
//@[024:00025) |   | |   |   ├─Token(Dot) |.|
//@[025:00033) |   | |   |   └─IdentifierSyntax
//@[025:00033) |   | |   |     └─Token(Identifier) |location|
//@[033:00035) |   | |   ├─Token(NewLine) |\r\n|
  }
//@[002:00003) |   | |   └─Token(RightBrace) |}|
//@[003:00005) |   | ├─Token(NewLine) |\r\n|
}]
//@[000:00001) |   | └─Token(RightBrace) |}|
//@[001:00002) |   └─Token(RightSquare) |]|
//@[002:00006) ├─Token(NewLine) |\r\n\r\n|

// simple indexed module loop
//@[029:00031) ├─Token(NewLine) |\r\n|
module storageResourcesWithIndex 'modulea.bicep' = [for (module, i) in myModules: {
//@[000:00256) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00032) | ├─IdentifierSyntax
//@[007:00032) | | └─Token(Identifier) |storageResourcesWithIndex|
//@[033:00048) | ├─StringSyntax
//@[033:00048) | | └─Token(StringComplete) |'modulea.bicep'|
//@[049:00050) | ├─Token(Assignment) |=|
//@[051:00256) | └─ForSyntax
//@[051:00052) |   ├─Token(LeftSquare) |[|
//@[052:00055) |   ├─Token(Identifier) |for|
//@[056:00067) |   ├─VariableBlockSyntax
//@[056:00057) |   | ├─Token(LeftParen) |(|
//@[057:00063) |   | ├─LocalVariableSyntax
//@[057:00063) |   | | └─IdentifierSyntax
//@[057:00063) |   | |   └─Token(Identifier) |module|
//@[063:00064) |   | ├─Token(Comma) |,|
//@[065:00066) |   | ├─LocalVariableSyntax
//@[065:00066) |   | | └─IdentifierSyntax
//@[065:00066) |   | |   └─Token(Identifier) |i|
//@[066:00067) |   | └─Token(RightParen) |)|
//@[068:00070) |   ├─Token(Identifier) |in|
//@[071:00080) |   ├─VariableAccessSyntax
//@[071:00080) |   | └─IdentifierSyntax
//@[071:00080) |   |   └─Token(Identifier) |myModules|
//@[080:00081) |   ├─Token(Colon) |:|
//@[082:00255) |   ├─ObjectSyntax
//@[082:00083) |   | ├─Token(LeftBrace) |{|
//@[083:00085) |   | ├─Token(NewLine) |\r\n|
  name: module.name
//@[002:00019) |   | ├─ObjectPropertySyntax
//@[002:00006) |   | | ├─IdentifierSyntax
//@[002:00006) |   | | | └─Token(Identifier) |name|
//@[006:00007) |   | | ├─Token(Colon) |:|
//@[008:00019) |   | | └─PropertyAccessSyntax
//@[008:00014) |   | |   ├─VariableAccessSyntax
//@[008:00014) |   | |   | └─IdentifierSyntax
//@[008:00014) |   | |   |   └─Token(Identifier) |module|
//@[014:00015) |   | |   ├─Token(Dot) |.|
//@[015:00019) |   | |   └─IdentifierSyntax
//@[015:00019) |   | |     └─Token(Identifier) |name|
//@[019:00021) |   | ├─Token(NewLine) |\r\n|
  params: {
//@[002:00146) |   | ├─ObjectPropertySyntax
//@[002:00008) |   | | ├─IdentifierSyntax
//@[002:00008) |   | | | └─Token(Identifier) |params|
//@[008:00009) |   | | ├─Token(Colon) |:|
//@[010:00146) |   | | └─ObjectSyntax
//@[010:00011) |   | |   ├─Token(LeftBrace) |{|
//@[011:00013) |   | |   ├─Token(NewLine) |\r\n|
    arrayParam: [
//@[004:00037) |   | |   ├─ObjectPropertySyntax
//@[004:00014) |   | |   | ├─IdentifierSyntax
//@[004:00014) |   | |   | | └─Token(Identifier) |arrayParam|
//@[014:00015) |   | |   | ├─Token(Colon) |:|
//@[016:00037) |   | |   | └─ArraySyntax
//@[016:00017) |   | |   |   ├─Token(LeftSquare) |[|
//@[017:00019) |   | |   |   ├─Token(NewLine) |\r\n|
      i + 1
//@[006:00011) |   | |   |   ├─ArrayItemSyntax
//@[006:00011) |   | |   |   | └─BinaryOperationSyntax
//@[006:00007) |   | |   |   |   ├─VariableAccessSyntax
//@[006:00007) |   | |   |   |   | └─IdentifierSyntax
//@[006:00007) |   | |   |   |   |   └─Token(Identifier) |i|
//@[008:00009) |   | |   |   |   ├─Token(Plus) |+|
//@[010:00011) |   | |   |   |   └─IntegerLiteralSyntax
//@[010:00011) |   | |   |   |     └─Token(Integer) |1|
//@[011:00013) |   | |   |   ├─Token(NewLine) |\r\n|
    ]
//@[004:00005) |   | |   |   └─Token(RightSquare) |]|
//@[005:00007) |   | |   ├─Token(NewLine) |\r\n|
    objParam: module
//@[004:00020) |   | |   ├─ObjectPropertySyntax
//@[004:00012) |   | |   | ├─IdentifierSyntax
//@[004:00012) |   | |   | | └─Token(Identifier) |objParam|
//@[012:00013) |   | |   | ├─Token(Colon) |:|
//@[014:00020) |   | |   | └─VariableAccessSyntax
//@[014:00020) |   | |   |   └─IdentifierSyntax
//@[014:00020) |   | |   |     └─Token(Identifier) |module|
//@[020:00022) |   | |   ├─Token(NewLine) |\r\n|
    stringParamB: module.location
//@[004:00033) |   | |   ├─ObjectPropertySyntax
//@[004:00016) |   | |   | ├─IdentifierSyntax
//@[004:00016) |   | |   | | └─Token(Identifier) |stringParamB|
//@[016:00017) |   | |   | ├─Token(Colon) |:|
//@[018:00033) |   | |   | └─PropertyAccessSyntax
//@[018:00024) |   | |   |   ├─VariableAccessSyntax
//@[018:00024) |   | |   |   | └─IdentifierSyntax
//@[018:00024) |   | |   |   |   └─Token(Identifier) |module|
//@[024:00025) |   | |   |   ├─Token(Dot) |.|
//@[025:00033) |   | |   |   └─IdentifierSyntax
//@[025:00033) |   | |   |     └─Token(Identifier) |location|
//@[033:00035) |   | |   ├─Token(NewLine) |\r\n|
    stringParamA: concat('a', i)
//@[004:00032) |   | |   ├─ObjectPropertySyntax
//@[004:00016) |   | |   | ├─IdentifierSyntax
//@[004:00016) |   | |   | | └─Token(Identifier) |stringParamA|
//@[016:00017) |   | |   | ├─Token(Colon) |:|
//@[018:00032) |   | |   | └─FunctionCallSyntax
//@[018:00024) |   | |   |   ├─IdentifierSyntax
//@[018:00024) |   | |   |   | └─Token(Identifier) |concat|
//@[024:00025) |   | |   |   ├─Token(LeftParen) |(|
//@[025:00028) |   | |   |   ├─FunctionArgumentSyntax
//@[025:00028) |   | |   |   | └─StringSyntax
//@[025:00028) |   | |   |   |   └─Token(StringComplete) |'a'|
//@[028:00029) |   | |   |   ├─Token(Comma) |,|
//@[030:00031) |   | |   |   ├─FunctionArgumentSyntax
//@[030:00031) |   | |   |   | └─VariableAccessSyntax
//@[030:00031) |   | |   |   |   └─IdentifierSyntax
//@[030:00031) |   | |   |   |     └─Token(Identifier) |i|
//@[031:00032) |   | |   |   └─Token(RightParen) |)|
//@[032:00034) |   | |   ├─Token(NewLine) |\r\n|
  }
//@[002:00003) |   | |   └─Token(RightBrace) |}|
//@[003:00005) |   | ├─Token(NewLine) |\r\n|
}]
//@[000:00001) |   | └─Token(RightBrace) |}|
//@[001:00002) |   └─Token(RightSquare) |]|
//@[002:00006) ├─Token(NewLine) |\r\n\r\n|

// nested module loop
//@[021:00023) ├─Token(NewLine) |\r\n|
module nestedModuleLoop 'modulea.bicep' = [for module in myModules: {
//@[000:00246) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00023) | ├─IdentifierSyntax
//@[007:00023) | | └─Token(Identifier) |nestedModuleLoop|
//@[024:00039) | ├─StringSyntax
//@[024:00039) | | └─Token(StringComplete) |'modulea.bicep'|
//@[040:00041) | ├─Token(Assignment) |=|
//@[042:00246) | └─ForSyntax
//@[042:00043) |   ├─Token(LeftSquare) |[|
//@[043:00046) |   ├─Token(Identifier) |for|
//@[047:00053) |   ├─LocalVariableSyntax
//@[047:00053) |   | └─IdentifierSyntax
//@[047:00053) |   |   └─Token(Identifier) |module|
//@[054:00056) |   ├─Token(Identifier) |in|
//@[057:00066) |   ├─VariableAccessSyntax
//@[057:00066) |   | └─IdentifierSyntax
//@[057:00066) |   |   └─Token(Identifier) |myModules|
//@[066:00067) |   ├─Token(Colon) |:|
//@[068:00245) |   ├─ObjectSyntax
//@[068:00069) |   | ├─Token(LeftBrace) |{|
//@[069:00071) |   | ├─Token(NewLine) |\r\n|
  name: module.name
//@[002:00019) |   | ├─ObjectPropertySyntax
//@[002:00006) |   | | ├─IdentifierSyntax
//@[002:00006) |   | | | └─Token(Identifier) |name|
//@[006:00007) |   | | ├─Token(Colon) |:|
//@[008:00019) |   | | └─PropertyAccessSyntax
//@[008:00014) |   | |   ├─VariableAccessSyntax
//@[008:00014) |   | |   | └─IdentifierSyntax
//@[008:00014) |   | |   |   └─Token(Identifier) |module|
//@[014:00015) |   | |   ├─Token(Dot) |.|
//@[015:00019) |   | |   └─IdentifierSyntax
//@[015:00019) |   | |     └─Token(Identifier) |name|
//@[019:00021) |   | ├─Token(NewLine) |\r\n|
  params: {
//@[002:00150) |   | ├─ObjectPropertySyntax
//@[002:00008) |   | | ├─IdentifierSyntax
//@[002:00008) |   | | | └─Token(Identifier) |params|
//@[008:00009) |   | | ├─Token(Colon) |:|
//@[010:00150) |   | | └─ObjectSyntax
//@[010:00011) |   | |   ├─Token(LeftBrace) |{|
//@[011:00013) |   | |   ├─Token(NewLine) |\r\n|
    arrayParam: [for i in range(0,3): concat('test-', i, '-', module.name)]
//@[004:00075) |   | |   ├─ObjectPropertySyntax
//@[004:00014) |   | |   | ├─IdentifierSyntax
//@[004:00014) |   | |   | | └─Token(Identifier) |arrayParam|
//@[014:00015) |   | |   | ├─Token(Colon) |:|
//@[016:00075) |   | |   | └─ForSyntax
//@[016:00017) |   | |   |   ├─Token(LeftSquare) |[|
//@[017:00020) |   | |   |   ├─Token(Identifier) |for|
//@[021:00022) |   | |   |   ├─LocalVariableSyntax
//@[021:00022) |   | |   |   | └─IdentifierSyntax
//@[021:00022) |   | |   |   |   └─Token(Identifier) |i|
//@[023:00025) |   | |   |   ├─Token(Identifier) |in|
//@[026:00036) |   | |   |   ├─FunctionCallSyntax
//@[026:00031) |   | |   |   | ├─IdentifierSyntax
//@[026:00031) |   | |   |   | | └─Token(Identifier) |range|
//@[031:00032) |   | |   |   | ├─Token(LeftParen) |(|
//@[032:00033) |   | |   |   | ├─FunctionArgumentSyntax
//@[032:00033) |   | |   |   | | └─IntegerLiteralSyntax
//@[032:00033) |   | |   |   | |   └─Token(Integer) |0|
//@[033:00034) |   | |   |   | ├─Token(Comma) |,|
//@[034:00035) |   | |   |   | ├─FunctionArgumentSyntax
//@[034:00035) |   | |   |   | | └─IntegerLiteralSyntax
//@[034:00035) |   | |   |   | |   └─Token(Integer) |3|
//@[035:00036) |   | |   |   | └─Token(RightParen) |)|
//@[036:00037) |   | |   |   ├─Token(Colon) |:|
//@[038:00074) |   | |   |   ├─FunctionCallSyntax
//@[038:00044) |   | |   |   | ├─IdentifierSyntax
//@[038:00044) |   | |   |   | | └─Token(Identifier) |concat|
//@[044:00045) |   | |   |   | ├─Token(LeftParen) |(|
//@[045:00052) |   | |   |   | ├─FunctionArgumentSyntax
//@[045:00052) |   | |   |   | | └─StringSyntax
//@[045:00052) |   | |   |   | |   └─Token(StringComplete) |'test-'|
//@[052:00053) |   | |   |   | ├─Token(Comma) |,|
//@[054:00055) |   | |   |   | ├─FunctionArgumentSyntax
//@[054:00055) |   | |   |   | | └─VariableAccessSyntax
//@[054:00055) |   | |   |   | |   └─IdentifierSyntax
//@[054:00055) |   | |   |   | |     └─Token(Identifier) |i|
//@[055:00056) |   | |   |   | ├─Token(Comma) |,|
//@[057:00060) |   | |   |   | ├─FunctionArgumentSyntax
//@[057:00060) |   | |   |   | | └─StringSyntax
//@[057:00060) |   | |   |   | |   └─Token(StringComplete) |'-'|
//@[060:00061) |   | |   |   | ├─Token(Comma) |,|
//@[062:00073) |   | |   |   | ├─FunctionArgumentSyntax
//@[062:00073) |   | |   |   | | └─PropertyAccessSyntax
//@[062:00068) |   | |   |   | |   ├─VariableAccessSyntax
//@[062:00068) |   | |   |   | |   | └─IdentifierSyntax
//@[062:00068) |   | |   |   | |   |   └─Token(Identifier) |module|
//@[068:00069) |   | |   |   | |   ├─Token(Dot) |.|
//@[069:00073) |   | |   |   | |   └─IdentifierSyntax
//@[069:00073) |   | |   |   | |     └─Token(Identifier) |name|
//@[073:00074) |   | |   |   | └─Token(RightParen) |)|
//@[074:00075) |   | |   |   └─Token(RightSquare) |]|
//@[075:00077) |   | |   ├─Token(NewLine) |\r\n|
    objParam: module
//@[004:00020) |   | |   ├─ObjectPropertySyntax
//@[004:00012) |   | |   | ├─IdentifierSyntax
//@[004:00012) |   | |   | | └─Token(Identifier) |objParam|
//@[012:00013) |   | |   | ├─Token(Colon) |:|
//@[014:00020) |   | |   | └─VariableAccessSyntax
//@[014:00020) |   | |   |   └─IdentifierSyntax
//@[014:00020) |   | |   |     └─Token(Identifier) |module|
//@[020:00022) |   | |   ├─Token(NewLine) |\r\n|
    stringParamB: module.location
//@[004:00033) |   | |   ├─ObjectPropertySyntax
//@[004:00016) |   | |   | ├─IdentifierSyntax
//@[004:00016) |   | |   | | └─Token(Identifier) |stringParamB|
//@[016:00017) |   | |   | ├─Token(Colon) |:|
//@[018:00033) |   | |   | └─PropertyAccessSyntax
//@[018:00024) |   | |   |   ├─VariableAccessSyntax
//@[018:00024) |   | |   |   | └─IdentifierSyntax
//@[018:00024) |   | |   |   |   └─Token(Identifier) |module|
//@[024:00025) |   | |   |   ├─Token(Dot) |.|
//@[025:00033) |   | |   |   └─IdentifierSyntax
//@[025:00033) |   | |   |     └─Token(Identifier) |location|
//@[033:00035) |   | |   ├─Token(NewLine) |\r\n|
  }
//@[002:00003) |   | |   └─Token(RightBrace) |}|
//@[003:00005) |   | ├─Token(NewLine) |\r\n|
}]
//@[000:00001) |   | └─Token(RightBrace) |}|
//@[001:00002) |   └─Token(RightSquare) |]|
//@[002:00006) ├─Token(NewLine) |\r\n\r\n|

// duplicate identifiers across scopes are allowed (inner hides the outer)
//@[074:00076) ├─Token(NewLine) |\r\n|
module duplicateIdentifiersWithinLoop 'modulea.bicep' = [for x in emptyArray:{
//@[000:00234) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00037) | ├─IdentifierSyntax
//@[007:00037) | | └─Token(Identifier) |duplicateIdentifiersWithinLoop|
//@[038:00053) | ├─StringSyntax
//@[038:00053) | | └─Token(StringComplete) |'modulea.bicep'|
//@[054:00055) | ├─Token(Assignment) |=|
//@[056:00234) | └─ForSyntax
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
//@[077:00233) |   ├─ObjectSyntax
//@[077:00078) |   | ├─Token(LeftBrace) |{|
//@[078:00080) |   | ├─Token(NewLine) |\r\n|
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
//@[020:00022) |   | ├─Token(NewLine) |\r\n|
  params: {
//@[002:00128) |   | ├─ObjectPropertySyntax
//@[002:00008) |   | | ├─IdentifierSyntax
//@[002:00008) |   | | | └─Token(Identifier) |params|
//@[008:00009) |   | | ├─Token(Colon) |:|
//@[010:00128) |   | | └─ObjectSyntax
//@[010:00011) |   | |   ├─Token(LeftBrace) |{|
//@[011:00013) |   | |   ├─Token(NewLine) |\r\n|
    objParam: {}
//@[004:00016) |   | |   ├─ObjectPropertySyntax
//@[004:00012) |   | |   | ├─IdentifierSyntax
//@[004:00012) |   | |   | | └─Token(Identifier) |objParam|
//@[012:00013) |   | |   | ├─Token(Colon) |:|
//@[014:00016) |   | |   | └─ObjectSyntax
//@[014:00015) |   | |   |   ├─Token(LeftBrace) |{|
//@[015:00016) |   | |   |   └─Token(RightBrace) |}|
//@[016:00018) |   | |   ├─Token(NewLine) |\r\n|
    stringParamA: 'test'
//@[004:00024) |   | |   ├─ObjectPropertySyntax
//@[004:00016) |   | |   | ├─IdentifierSyntax
//@[004:00016) |   | |   | | └─Token(Identifier) |stringParamA|
//@[016:00017) |   | |   | ├─Token(Colon) |:|
//@[018:00024) |   | |   | └─StringSyntax
//@[018:00024) |   | |   |   └─Token(StringComplete) |'test'|
//@[024:00026) |   | |   ├─Token(NewLine) |\r\n|
    stringParamB: 'test'
//@[004:00024) |   | |   ├─ObjectPropertySyntax
//@[004:00016) |   | |   | ├─IdentifierSyntax
//@[004:00016) |   | |   | | └─Token(Identifier) |stringParamB|
//@[016:00017) |   | |   | ├─Token(Colon) |:|
//@[018:00024) |   | |   | └─StringSyntax
//@[018:00024) |   | |   |   └─Token(StringComplete) |'test'|
//@[024:00026) |   | |   ├─Token(NewLine) |\r\n|
    arrayParam: [for x in emptyArray: x]
//@[004:00040) |   | |   ├─ObjectPropertySyntax
//@[004:00014) |   | |   | ├─IdentifierSyntax
//@[004:00014) |   | |   | | └─Token(Identifier) |arrayParam|
//@[014:00015) |   | |   | ├─Token(Colon) |:|
//@[016:00040) |   | |   | └─ForSyntax
//@[016:00017) |   | |   |   ├─Token(LeftSquare) |[|
//@[017:00020) |   | |   |   ├─Token(Identifier) |for|
//@[021:00022) |   | |   |   ├─LocalVariableSyntax
//@[021:00022) |   | |   |   | └─IdentifierSyntax
//@[021:00022) |   | |   |   |   └─Token(Identifier) |x|
//@[023:00025) |   | |   |   ├─Token(Identifier) |in|
//@[026:00036) |   | |   |   ├─VariableAccessSyntax
//@[026:00036) |   | |   |   | └─IdentifierSyntax
//@[026:00036) |   | |   |   |   └─Token(Identifier) |emptyArray|
//@[036:00037) |   | |   |   ├─Token(Colon) |:|
//@[038:00039) |   | |   |   ├─VariableAccessSyntax
//@[038:00039) |   | |   |   | └─IdentifierSyntax
//@[038:00039) |   | |   |   |   └─Token(Identifier) |x|
//@[039:00040) |   | |   |   └─Token(RightSquare) |]|
//@[040:00042) |   | |   ├─Token(NewLine) |\r\n|
  }
//@[002:00003) |   | |   └─Token(RightBrace) |}|
//@[003:00005) |   | ├─Token(NewLine) |\r\n|
}]
//@[000:00001) |   | └─Token(RightBrace) |}|
//@[001:00002) |   └─Token(RightSquare) |]|
//@[002:00006) ├─Token(NewLine) |\r\n\r\n|

// duplicate identifiers across scopes are allowed (inner hides the outer)
//@[074:00076) ├─Token(NewLine) |\r\n|
var duplicateAcrossScopes = 'hello'
//@[000:00035) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00025) | ├─IdentifierSyntax
//@[004:00025) | | └─Token(Identifier) |duplicateAcrossScopes|
//@[026:00027) | ├─Token(Assignment) |=|
//@[028:00035) | └─StringSyntax
//@[028:00035) |   └─Token(StringComplete) |'hello'|
//@[035:00037) ├─Token(NewLine) |\r\n|
module duplicateInGlobalAndOneLoop 'modulea.bicep' = [for duplicateAcrossScopes in []: {
//@[000:00264) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00034) | ├─IdentifierSyntax
//@[007:00034) | | └─Token(Identifier) |duplicateInGlobalAndOneLoop|
//@[035:00050) | ├─StringSyntax
//@[035:00050) | | └─Token(StringComplete) |'modulea.bicep'|
//@[051:00052) | ├─Token(Assignment) |=|
//@[053:00264) | └─ForSyntax
//@[053:00054) |   ├─Token(LeftSquare) |[|
//@[054:00057) |   ├─Token(Identifier) |for|
//@[058:00079) |   ├─LocalVariableSyntax
//@[058:00079) |   | └─IdentifierSyntax
//@[058:00079) |   |   └─Token(Identifier) |duplicateAcrossScopes|
//@[080:00082) |   ├─Token(Identifier) |in|
//@[083:00085) |   ├─ArraySyntax
//@[083:00084) |   | ├─Token(LeftSquare) |[|
//@[084:00085) |   | └─Token(RightSquare) |]|
//@[085:00086) |   ├─Token(Colon) |:|
//@[087:00263) |   ├─ObjectSyntax
//@[087:00088) |   | ├─Token(LeftBrace) |{|
//@[088:00090) |   | ├─Token(NewLine) |\r\n|
  name: 'hello-${duplicateAcrossScopes}'
//@[002:00040) |   | ├─ObjectPropertySyntax
//@[002:00006) |   | | ├─IdentifierSyntax
//@[002:00006) |   | | | └─Token(Identifier) |name|
//@[006:00007) |   | | ├─Token(Colon) |:|
//@[008:00040) |   | | └─StringSyntax
//@[008:00017) |   | |   ├─Token(StringLeftPiece) |'hello-${|
//@[017:00038) |   | |   ├─VariableAccessSyntax
//@[017:00038) |   | |   | └─IdentifierSyntax
//@[017:00038) |   | |   |   └─Token(Identifier) |duplicateAcrossScopes|
//@[038:00040) |   | |   └─Token(StringRightPiece) |}'|
//@[040:00042) |   | ├─Token(NewLine) |\r\n|
  params: {
//@[002:00128) |   | ├─ObjectPropertySyntax
//@[002:00008) |   | | ├─IdentifierSyntax
//@[002:00008) |   | | | └─Token(Identifier) |params|
//@[008:00009) |   | | ├─Token(Colon) |:|
//@[010:00128) |   | | └─ObjectSyntax
//@[010:00011) |   | |   ├─Token(LeftBrace) |{|
//@[011:00013) |   | |   ├─Token(NewLine) |\r\n|
    objParam: {}
//@[004:00016) |   | |   ├─ObjectPropertySyntax
//@[004:00012) |   | |   | ├─IdentifierSyntax
//@[004:00012) |   | |   | | └─Token(Identifier) |objParam|
//@[012:00013) |   | |   | ├─Token(Colon) |:|
//@[014:00016) |   | |   | └─ObjectSyntax
//@[014:00015) |   | |   |   ├─Token(LeftBrace) |{|
//@[015:00016) |   | |   |   └─Token(RightBrace) |}|
//@[016:00018) |   | |   ├─Token(NewLine) |\r\n|
    stringParamA: 'test'
//@[004:00024) |   | |   ├─ObjectPropertySyntax
//@[004:00016) |   | |   | ├─IdentifierSyntax
//@[004:00016) |   | |   | | └─Token(Identifier) |stringParamA|
//@[016:00017) |   | |   | ├─Token(Colon) |:|
//@[018:00024) |   | |   | └─StringSyntax
//@[018:00024) |   | |   |   └─Token(StringComplete) |'test'|
//@[024:00026) |   | |   ├─Token(NewLine) |\r\n|
    stringParamB: 'test'
//@[004:00024) |   | |   ├─ObjectPropertySyntax
//@[004:00016) |   | |   | ├─IdentifierSyntax
//@[004:00016) |   | |   | | └─Token(Identifier) |stringParamB|
//@[016:00017) |   | |   | ├─Token(Colon) |:|
//@[018:00024) |   | |   | └─StringSyntax
//@[018:00024) |   | |   |   └─Token(StringComplete) |'test'|
//@[024:00026) |   | |   ├─Token(NewLine) |\r\n|
    arrayParam: [for x in emptyArray: x]
//@[004:00040) |   | |   ├─ObjectPropertySyntax
//@[004:00014) |   | |   | ├─IdentifierSyntax
//@[004:00014) |   | |   | | └─Token(Identifier) |arrayParam|
//@[014:00015) |   | |   | ├─Token(Colon) |:|
//@[016:00040) |   | |   | └─ForSyntax
//@[016:00017) |   | |   |   ├─Token(LeftSquare) |[|
//@[017:00020) |   | |   |   ├─Token(Identifier) |for|
//@[021:00022) |   | |   |   ├─LocalVariableSyntax
//@[021:00022) |   | |   |   | └─IdentifierSyntax
//@[021:00022) |   | |   |   |   └─Token(Identifier) |x|
//@[023:00025) |   | |   |   ├─Token(Identifier) |in|
//@[026:00036) |   | |   |   ├─VariableAccessSyntax
//@[026:00036) |   | |   |   | └─IdentifierSyntax
//@[026:00036) |   | |   |   |   └─Token(Identifier) |emptyArray|
//@[036:00037) |   | |   |   ├─Token(Colon) |:|
//@[038:00039) |   | |   |   ├─VariableAccessSyntax
//@[038:00039) |   | |   |   | └─IdentifierSyntax
//@[038:00039) |   | |   |   |   └─Token(Identifier) |x|
//@[039:00040) |   | |   |   └─Token(RightSquare) |]|
//@[040:00042) |   | |   ├─Token(NewLine) |\r\n|
  }
//@[002:00003) |   | |   └─Token(RightBrace) |}|
//@[003:00005) |   | ├─Token(NewLine) |\r\n|
}]
//@[000:00001) |   | └─Token(RightBrace) |}|
//@[001:00002) |   └─Token(RightSquare) |]|
//@[002:00006) ├─Token(NewLine) |\r\n\r\n|

var someDuplicate = true
//@[000:00024) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00017) | ├─IdentifierSyntax
//@[004:00017) | | └─Token(Identifier) |someDuplicate|
//@[018:00019) | ├─Token(Assignment) |=|
//@[020:00024) | └─BooleanLiteralSyntax
//@[020:00024) |   └─Token(TrueKeyword) |true|
//@[024:00026) ├─Token(NewLine) |\r\n|
var otherDuplicate = false
//@[000:00026) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00018) | ├─IdentifierSyntax
//@[004:00018) | | └─Token(Identifier) |otherDuplicate|
//@[019:00020) | ├─Token(Assignment) |=|
//@[021:00026) | └─BooleanLiteralSyntax
//@[021:00026) |   └─Token(FalseKeyword) |false|
//@[026:00028) ├─Token(NewLine) |\r\n|
module duplicatesEverywhere 'modulea.bicep' = [for someDuplicate in []: {
//@[000:00263) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00027) | ├─IdentifierSyntax
//@[007:00027) | | └─Token(Identifier) |duplicatesEverywhere|
//@[028:00043) | ├─StringSyntax
//@[028:00043) | | └─Token(StringComplete) |'modulea.bicep'|
//@[044:00045) | ├─Token(Assignment) |=|
//@[046:00263) | └─ForSyntax
//@[046:00047) |   ├─Token(LeftSquare) |[|
//@[047:00050) |   ├─Token(Identifier) |for|
//@[051:00064) |   ├─LocalVariableSyntax
//@[051:00064) |   | └─IdentifierSyntax
//@[051:00064) |   |   └─Token(Identifier) |someDuplicate|
//@[065:00067) |   ├─Token(Identifier) |in|
//@[068:00070) |   ├─ArraySyntax
//@[068:00069) |   | ├─Token(LeftSquare) |[|
//@[069:00070) |   | └─Token(RightSquare) |]|
//@[070:00071) |   ├─Token(Colon) |:|
//@[072:00262) |   ├─ObjectSyntax
//@[072:00073) |   | ├─Token(LeftBrace) |{|
//@[073:00075) |   | ├─Token(NewLine) |\r\n|
  name: 'hello-${someDuplicate}'
//@[002:00032) |   | ├─ObjectPropertySyntax
//@[002:00006) |   | | ├─IdentifierSyntax
//@[002:00006) |   | | | └─Token(Identifier) |name|
//@[006:00007) |   | | ├─Token(Colon) |:|
//@[008:00032) |   | | └─StringSyntax
//@[008:00017) |   | |   ├─Token(StringLeftPiece) |'hello-${|
//@[017:00030) |   | |   ├─VariableAccessSyntax
//@[017:00030) |   | |   | └─IdentifierSyntax
//@[017:00030) |   | |   |   └─Token(Identifier) |someDuplicate|
//@[030:00032) |   | |   └─Token(StringRightPiece) |}'|
//@[032:00034) |   | ├─Token(NewLine) |\r\n|
  params: {
//@[002:00150) |   | ├─ObjectPropertySyntax
//@[002:00008) |   | | ├─IdentifierSyntax
//@[002:00008) |   | | | └─Token(Identifier) |params|
//@[008:00009) |   | | ├─Token(Colon) |:|
//@[010:00150) |   | | └─ObjectSyntax
//@[010:00011) |   | |   ├─Token(LeftBrace) |{|
//@[011:00013) |   | |   ├─Token(NewLine) |\r\n|
    objParam: {}
//@[004:00016) |   | |   ├─ObjectPropertySyntax
//@[004:00012) |   | |   | ├─IdentifierSyntax
//@[004:00012) |   | |   | | └─Token(Identifier) |objParam|
//@[012:00013) |   | |   | ├─Token(Colon) |:|
//@[014:00016) |   | |   | └─ObjectSyntax
//@[014:00015) |   | |   |   ├─Token(LeftBrace) |{|
//@[015:00016) |   | |   |   └─Token(RightBrace) |}|
//@[016:00018) |   | |   ├─Token(NewLine) |\r\n|
    stringParamB: 'test'
//@[004:00024) |   | |   ├─ObjectPropertySyntax
//@[004:00016) |   | |   | ├─IdentifierSyntax
//@[004:00016) |   | |   | | └─Token(Identifier) |stringParamB|
//@[016:00017) |   | |   | ├─Token(Colon) |:|
//@[018:00024) |   | |   | └─StringSyntax
//@[018:00024) |   | |   |   └─Token(StringComplete) |'test'|
//@[024:00026) |   | |   ├─Token(NewLine) |\r\n|
    arrayParam: [for otherDuplicate in emptyArray: '${someDuplicate}-${otherDuplicate}']
//@[004:00088) |   | |   ├─ObjectPropertySyntax
//@[004:00014) |   | |   | ├─IdentifierSyntax
//@[004:00014) |   | |   | | └─Token(Identifier) |arrayParam|
//@[014:00015) |   | |   | ├─Token(Colon) |:|
//@[016:00088) |   | |   | └─ForSyntax
//@[016:00017) |   | |   |   ├─Token(LeftSquare) |[|
//@[017:00020) |   | |   |   ├─Token(Identifier) |for|
//@[021:00035) |   | |   |   ├─LocalVariableSyntax
//@[021:00035) |   | |   |   | └─IdentifierSyntax
//@[021:00035) |   | |   |   |   └─Token(Identifier) |otherDuplicate|
//@[036:00038) |   | |   |   ├─Token(Identifier) |in|
//@[039:00049) |   | |   |   ├─VariableAccessSyntax
//@[039:00049) |   | |   |   | └─IdentifierSyntax
//@[039:00049) |   | |   |   |   └─Token(Identifier) |emptyArray|
//@[049:00050) |   | |   |   ├─Token(Colon) |:|
//@[051:00087) |   | |   |   ├─StringSyntax
//@[051:00054) |   | |   |   | ├─Token(StringLeftPiece) |'${|
//@[054:00067) |   | |   |   | ├─VariableAccessSyntax
//@[054:00067) |   | |   |   | | └─IdentifierSyntax
//@[054:00067) |   | |   |   | |   └─Token(Identifier) |someDuplicate|
//@[067:00071) |   | |   |   | ├─Token(StringMiddlePiece) |}-${|
//@[071:00085) |   | |   |   | ├─VariableAccessSyntax
//@[071:00085) |   | |   |   | | └─IdentifierSyntax
//@[071:00085) |   | |   |   | |   └─Token(Identifier) |otherDuplicate|
//@[085:00087) |   | |   |   | └─Token(StringRightPiece) |}'|
//@[087:00088) |   | |   |   └─Token(RightSquare) |]|
//@[088:00090) |   | |   ├─Token(NewLine) |\r\n|
  }
//@[002:00003) |   | |   └─Token(RightBrace) |}|
//@[003:00005) |   | ├─Token(NewLine) |\r\n|
}]
//@[000:00001) |   | └─Token(RightBrace) |}|
//@[001:00002) |   └─Token(RightSquare) |]|
//@[002:00006) ├─Token(NewLine) |\r\n\r\n|

module propertyLoopInsideParameterValue 'modulea.bicep' = {
//@[000:00438) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00039) | ├─IdentifierSyntax
//@[007:00039) | | └─Token(Identifier) |propertyLoopInsideParameterValue|
//@[040:00055) | ├─StringSyntax
//@[040:00055) | | └─Token(StringComplete) |'modulea.bicep'|
//@[056:00057) | ├─Token(Assignment) |=|
//@[058:00438) | └─ObjectSyntax
//@[058:00059) |   ├─Token(LeftBrace) |{|
//@[059:00061) |   ├─Token(NewLine) |\r\n|
  name: 'propertyLoopInsideParameterValue'
//@[002:00042) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00042) |   | └─StringSyntax
//@[008:00042) |   |   └─Token(StringComplete) |'propertyLoopInsideParameterValue'|
//@[042:00044) |   ├─Token(NewLine) |\r\n|
  params: {
//@[002:00330) |   ├─ObjectPropertySyntax
//@[002:00008) |   | ├─IdentifierSyntax
//@[002:00008) |   | | └─Token(Identifier) |params|
//@[008:00009) |   | ├─Token(Colon) |:|
//@[010:00330) |   | └─ObjectSyntax
//@[010:00011) |   |   ├─Token(LeftBrace) |{|
//@[011:00013) |   |   ├─Token(NewLine) |\r\n|
    objParam: {
//@[004:00209) |   |   ├─ObjectPropertySyntax
//@[004:00012) |   |   | ├─IdentifierSyntax
//@[004:00012) |   |   | | └─Token(Identifier) |objParam|
//@[012:00013) |   |   | ├─Token(Colon) |:|
//@[014:00209) |   |   | └─ObjectSyntax
//@[014:00015) |   |   |   ├─Token(LeftBrace) |{|
//@[015:00017) |   |   |   ├─Token(NewLine) |\r\n|
      a: [for i in range(0,10): i]
//@[006:00034) |   |   |   ├─ObjectPropertySyntax
//@[006:00007) |   |   |   | ├─IdentifierSyntax
//@[006:00007) |   |   |   | | └─Token(Identifier) |a|
//@[007:00008) |   |   |   | ├─Token(Colon) |:|
//@[009:00034) |   |   |   | └─ForSyntax
//@[009:00010) |   |   |   |   ├─Token(LeftSquare) |[|
//@[010:00013) |   |   |   |   ├─Token(Identifier) |for|
//@[014:00015) |   |   |   |   ├─LocalVariableSyntax
//@[014:00015) |   |   |   |   | └─IdentifierSyntax
//@[014:00015) |   |   |   |   |   └─Token(Identifier) |i|
//@[016:00018) |   |   |   |   ├─Token(Identifier) |in|
//@[019:00030) |   |   |   |   ├─FunctionCallSyntax
//@[019:00024) |   |   |   |   | ├─IdentifierSyntax
//@[019:00024) |   |   |   |   | | └─Token(Identifier) |range|
//@[024:00025) |   |   |   |   | ├─Token(LeftParen) |(|
//@[025:00026) |   |   |   |   | ├─FunctionArgumentSyntax
//@[025:00026) |   |   |   |   | | └─IntegerLiteralSyntax
//@[025:00026) |   |   |   |   | |   └─Token(Integer) |0|
//@[026:00027) |   |   |   |   | ├─Token(Comma) |,|
//@[027:00029) |   |   |   |   | ├─FunctionArgumentSyntax
//@[027:00029) |   |   |   |   | | └─IntegerLiteralSyntax
//@[027:00029) |   |   |   |   | |   └─Token(Integer) |10|
//@[029:00030) |   |   |   |   | └─Token(RightParen) |)|
//@[030:00031) |   |   |   |   ├─Token(Colon) |:|
//@[032:00033) |   |   |   |   ├─VariableAccessSyntax
//@[032:00033) |   |   |   |   | └─IdentifierSyntax
//@[032:00033) |   |   |   |   |   └─Token(Identifier) |i|
//@[033:00034) |   |   |   |   └─Token(RightSquare) |]|
//@[034:00036) |   |   |   ├─Token(NewLine) |\r\n|
      b: [for i in range(1,2): i]
//@[006:00033) |   |   |   ├─ObjectPropertySyntax
//@[006:00007) |   |   |   | ├─IdentifierSyntax
//@[006:00007) |   |   |   | | └─Token(Identifier) |b|
//@[007:00008) |   |   |   | ├─Token(Colon) |:|
//@[009:00033) |   |   |   | └─ForSyntax
//@[009:00010) |   |   |   |   ├─Token(LeftSquare) |[|
//@[010:00013) |   |   |   |   ├─Token(Identifier) |for|
//@[014:00015) |   |   |   |   ├─LocalVariableSyntax
//@[014:00015) |   |   |   |   | └─IdentifierSyntax
//@[014:00015) |   |   |   |   |   └─Token(Identifier) |i|
//@[016:00018) |   |   |   |   ├─Token(Identifier) |in|
//@[019:00029) |   |   |   |   ├─FunctionCallSyntax
//@[019:00024) |   |   |   |   | ├─IdentifierSyntax
//@[019:00024) |   |   |   |   | | └─Token(Identifier) |range|
//@[024:00025) |   |   |   |   | ├─Token(LeftParen) |(|
//@[025:00026) |   |   |   |   | ├─FunctionArgumentSyntax
//@[025:00026) |   |   |   |   | | └─IntegerLiteralSyntax
//@[025:00026) |   |   |   |   | |   └─Token(Integer) |1|
//@[026:00027) |   |   |   |   | ├─Token(Comma) |,|
//@[027:00028) |   |   |   |   | ├─FunctionArgumentSyntax
//@[027:00028) |   |   |   |   | | └─IntegerLiteralSyntax
//@[027:00028) |   |   |   |   | |   └─Token(Integer) |2|
//@[028:00029) |   |   |   |   | └─Token(RightParen) |)|
//@[029:00030) |   |   |   |   ├─Token(Colon) |:|
//@[031:00032) |   |   |   |   ├─VariableAccessSyntax
//@[031:00032) |   |   |   |   | └─IdentifierSyntax
//@[031:00032) |   |   |   |   |   └─Token(Identifier) |i|
//@[032:00033) |   |   |   |   └─Token(RightSquare) |]|
//@[033:00035) |   |   |   ├─Token(NewLine) |\r\n|
      c: {
//@[006:00056) |   |   |   ├─ObjectPropertySyntax
//@[006:00007) |   |   |   | ├─IdentifierSyntax
//@[006:00007) |   |   |   | | └─Token(Identifier) |c|
//@[007:00008) |   |   |   | ├─Token(Colon) |:|
//@[009:00056) |   |   |   | └─ObjectSyntax
//@[009:00010) |   |   |   |   ├─Token(LeftBrace) |{|
//@[010:00012) |   |   |   |   ├─Token(NewLine) |\r\n|
        d: [for j in range(2,3): j]
//@[008:00035) |   |   |   |   ├─ObjectPropertySyntax
//@[008:00009) |   |   |   |   | ├─IdentifierSyntax
//@[008:00009) |   |   |   |   | | └─Token(Identifier) |d|
//@[009:00010) |   |   |   |   | ├─Token(Colon) |:|
//@[011:00035) |   |   |   |   | └─ForSyntax
//@[011:00012) |   |   |   |   |   ├─Token(LeftSquare) |[|
//@[012:00015) |   |   |   |   |   ├─Token(Identifier) |for|
//@[016:00017) |   |   |   |   |   ├─LocalVariableSyntax
//@[016:00017) |   |   |   |   |   | └─IdentifierSyntax
//@[016:00017) |   |   |   |   |   |   └─Token(Identifier) |j|
//@[018:00020) |   |   |   |   |   ├─Token(Identifier) |in|
//@[021:00031) |   |   |   |   |   ├─FunctionCallSyntax
//@[021:00026) |   |   |   |   |   | ├─IdentifierSyntax
//@[021:00026) |   |   |   |   |   | | └─Token(Identifier) |range|
//@[026:00027) |   |   |   |   |   | ├─Token(LeftParen) |(|
//@[027:00028) |   |   |   |   |   | ├─FunctionArgumentSyntax
//@[027:00028) |   |   |   |   |   | | └─IntegerLiteralSyntax
//@[027:00028) |   |   |   |   |   | |   └─Token(Integer) |2|
//@[028:00029) |   |   |   |   |   | ├─Token(Comma) |,|
//@[029:00030) |   |   |   |   |   | ├─FunctionArgumentSyntax
//@[029:00030) |   |   |   |   |   | | └─IntegerLiteralSyntax
//@[029:00030) |   |   |   |   |   | |   └─Token(Integer) |3|
//@[030:00031) |   |   |   |   |   | └─Token(RightParen) |)|
//@[031:00032) |   |   |   |   |   ├─Token(Colon) |:|
//@[033:00034) |   |   |   |   |   ├─VariableAccessSyntax
//@[033:00034) |   |   |   |   |   | └─IdentifierSyntax
//@[033:00034) |   |   |   |   |   |   └─Token(Identifier) |j|
//@[034:00035) |   |   |   |   |   └─Token(RightSquare) |]|
//@[035:00037) |   |   |   |   ├─Token(NewLine) |\r\n|
      }
//@[006:00007) |   |   |   |   └─Token(RightBrace) |}|
//@[007:00009) |   |   |   ├─Token(NewLine) |\r\n|
      e: [for k in range(4,4): {
//@[006:00056) |   |   |   ├─ObjectPropertySyntax
//@[006:00007) |   |   |   | ├─IdentifierSyntax
//@[006:00007) |   |   |   | | └─Token(Identifier) |e|
//@[007:00008) |   |   |   | ├─Token(Colon) |:|
//@[009:00056) |   |   |   | └─ForSyntax
//@[009:00010) |   |   |   |   ├─Token(LeftSquare) |[|
//@[010:00013) |   |   |   |   ├─Token(Identifier) |for|
//@[014:00015) |   |   |   |   ├─LocalVariableSyntax
//@[014:00015) |   |   |   |   | └─IdentifierSyntax
//@[014:00015) |   |   |   |   |   └─Token(Identifier) |k|
//@[016:00018) |   |   |   |   ├─Token(Identifier) |in|
//@[019:00029) |   |   |   |   ├─FunctionCallSyntax
//@[019:00024) |   |   |   |   | ├─IdentifierSyntax
//@[019:00024) |   |   |   |   | | └─Token(Identifier) |range|
//@[024:00025) |   |   |   |   | ├─Token(LeftParen) |(|
//@[025:00026) |   |   |   |   | ├─FunctionArgumentSyntax
//@[025:00026) |   |   |   |   | | └─IntegerLiteralSyntax
//@[025:00026) |   |   |   |   | |   └─Token(Integer) |4|
//@[026:00027) |   |   |   |   | ├─Token(Comma) |,|
//@[027:00028) |   |   |   |   | ├─FunctionArgumentSyntax
//@[027:00028) |   |   |   |   | | └─IntegerLiteralSyntax
//@[027:00028) |   |   |   |   | |   └─Token(Integer) |4|
//@[028:00029) |   |   |   |   | └─Token(RightParen) |)|
//@[029:00030) |   |   |   |   ├─Token(Colon) |:|
//@[031:00055) |   |   |   |   ├─ObjectSyntax
//@[031:00032) |   |   |   |   | ├─Token(LeftBrace) |{|
//@[032:00034) |   |   |   |   | ├─Token(NewLine) |\r\n|
        f: k
//@[008:00012) |   |   |   |   | ├─ObjectPropertySyntax
//@[008:00009) |   |   |   |   | | ├─IdentifierSyntax
//@[008:00009) |   |   |   |   | | | └─Token(Identifier) |f|
//@[009:00010) |   |   |   |   | | ├─Token(Colon) |:|
//@[011:00012) |   |   |   |   | | └─VariableAccessSyntax
//@[011:00012) |   |   |   |   | |   └─IdentifierSyntax
//@[011:00012) |   |   |   |   | |     └─Token(Identifier) |k|
//@[012:00014) |   |   |   |   | ├─Token(NewLine) |\r\n|
      }]
//@[006:00007) |   |   |   |   | └─Token(RightBrace) |}|
//@[007:00008) |   |   |   |   └─Token(RightSquare) |]|
//@[008:00010) |   |   |   ├─Token(NewLine) |\r\n|
    }
//@[004:00005) |   |   |   └─Token(RightBrace) |}|
//@[005:00007) |   |   ├─Token(NewLine) |\r\n|
    stringParamB: ''
//@[004:00020) |   |   ├─ObjectPropertySyntax
//@[004:00016) |   |   | ├─IdentifierSyntax
//@[004:00016) |   |   | | └─Token(Identifier) |stringParamB|
//@[016:00017) |   |   | ├─Token(Colon) |:|
//@[018:00020) |   |   | └─StringSyntax
//@[018:00020) |   |   |   └─Token(StringComplete) |''|
//@[020:00022) |   |   ├─Token(NewLine) |\r\n|
    arrayParam: [
//@[004:00079) |   |   ├─ObjectPropertySyntax
//@[004:00014) |   |   | ├─IdentifierSyntax
//@[004:00014) |   |   | | └─Token(Identifier) |arrayParam|
//@[014:00015) |   |   | ├─Token(Colon) |:|
//@[016:00079) |   |   | └─ArraySyntax
//@[016:00017) |   |   |   ├─Token(LeftSquare) |[|
//@[017:00019) |   |   |   ├─Token(NewLine) |\r\n|
      {
//@[006:00053) |   |   |   ├─ArrayItemSyntax
//@[006:00053) |   |   |   | └─ObjectSyntax
//@[006:00007) |   |   |   |   ├─Token(LeftBrace) |{|
//@[007:00009) |   |   |   |   ├─Token(NewLine) |\r\n|
        e: [for j in range(7,7): j]
//@[008:00035) |   |   |   |   ├─ObjectPropertySyntax
//@[008:00009) |   |   |   |   | ├─IdentifierSyntax
//@[008:00009) |   |   |   |   | | └─Token(Identifier) |e|
//@[009:00010) |   |   |   |   | ├─Token(Colon) |:|
//@[011:00035) |   |   |   |   | └─ForSyntax
//@[011:00012) |   |   |   |   |   ├─Token(LeftSquare) |[|
//@[012:00015) |   |   |   |   |   ├─Token(Identifier) |for|
//@[016:00017) |   |   |   |   |   ├─LocalVariableSyntax
//@[016:00017) |   |   |   |   |   | └─IdentifierSyntax
//@[016:00017) |   |   |   |   |   |   └─Token(Identifier) |j|
//@[018:00020) |   |   |   |   |   ├─Token(Identifier) |in|
//@[021:00031) |   |   |   |   |   ├─FunctionCallSyntax
//@[021:00026) |   |   |   |   |   | ├─IdentifierSyntax
//@[021:00026) |   |   |   |   |   | | └─Token(Identifier) |range|
//@[026:00027) |   |   |   |   |   | ├─Token(LeftParen) |(|
//@[027:00028) |   |   |   |   |   | ├─FunctionArgumentSyntax
//@[027:00028) |   |   |   |   |   | | └─IntegerLiteralSyntax
//@[027:00028) |   |   |   |   |   | |   └─Token(Integer) |7|
//@[028:00029) |   |   |   |   |   | ├─Token(Comma) |,|
//@[029:00030) |   |   |   |   |   | ├─FunctionArgumentSyntax
//@[029:00030) |   |   |   |   |   | | └─IntegerLiteralSyntax
//@[029:00030) |   |   |   |   |   | |   └─Token(Integer) |7|
//@[030:00031) |   |   |   |   |   | └─Token(RightParen) |)|
//@[031:00032) |   |   |   |   |   ├─Token(Colon) |:|
//@[033:00034) |   |   |   |   |   ├─VariableAccessSyntax
//@[033:00034) |   |   |   |   |   | └─IdentifierSyntax
//@[033:00034) |   |   |   |   |   |   └─Token(Identifier) |j|
//@[034:00035) |   |   |   |   |   └─Token(RightSquare) |]|
//@[035:00037) |   |   |   |   ├─Token(NewLine) |\r\n|
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

module propertyLoopInsideParameterValueWithIndexes 'modulea.bicep' = {
//@[000:00514) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00050) | ├─IdentifierSyntax
//@[007:00050) | | └─Token(Identifier) |propertyLoopInsideParameterValueWithIndexes|
//@[051:00066) | ├─StringSyntax
//@[051:00066) | | └─Token(StringComplete) |'modulea.bicep'|
//@[067:00068) | ├─Token(Assignment) |=|
//@[069:00514) | └─ObjectSyntax
//@[069:00070) |   ├─Token(LeftBrace) |{|
//@[070:00072) |   ├─Token(NewLine) |\r\n|
  name: 'propertyLoopInsideParameterValueWithIndexes'
//@[002:00053) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00053) |   | └─StringSyntax
//@[008:00053) |   |   └─Token(StringComplete) |'propertyLoopInsideParameterValueWithIndexes'|
//@[053:00055) |   ├─Token(NewLine) |\r\n|
  params: {
//@[002:00384) |   ├─ObjectPropertySyntax
//@[002:00008) |   | ├─IdentifierSyntax
//@[002:00008) |   | | └─Token(Identifier) |params|
//@[008:00009) |   | ├─Token(Colon) |:|
//@[010:00384) |   | └─ObjectSyntax
//@[010:00011) |   |   ├─Token(LeftBrace) |{|
//@[011:00013) |   |   ├─Token(NewLine) |\r\n|
    objParam: {
//@[004:00263) |   |   ├─ObjectPropertySyntax
//@[004:00012) |   |   | ├─IdentifierSyntax
//@[004:00012) |   |   | | └─Token(Identifier) |objParam|
//@[012:00013) |   |   | ├─Token(Colon) |:|
//@[014:00263) |   |   | └─ObjectSyntax
//@[014:00015) |   |   |   ├─Token(LeftBrace) |{|
//@[015:00017) |   |   |   ├─Token(NewLine) |\r\n|
      a: [for (i, i2) in range(0,10): i + i2]
//@[006:00045) |   |   |   ├─ObjectPropertySyntax
//@[006:00007) |   |   |   | ├─IdentifierSyntax
//@[006:00007) |   |   |   | | └─Token(Identifier) |a|
//@[007:00008) |   |   |   | ├─Token(Colon) |:|
//@[009:00045) |   |   |   | └─ForSyntax
//@[009:00010) |   |   |   |   ├─Token(LeftSquare) |[|
//@[010:00013) |   |   |   |   ├─Token(Identifier) |for|
//@[014:00021) |   |   |   |   ├─VariableBlockSyntax
//@[014:00015) |   |   |   |   | ├─Token(LeftParen) |(|
//@[015:00016) |   |   |   |   | ├─LocalVariableSyntax
//@[015:00016) |   |   |   |   | | └─IdentifierSyntax
//@[015:00016) |   |   |   |   | |   └─Token(Identifier) |i|
//@[016:00017) |   |   |   |   | ├─Token(Comma) |,|
//@[018:00020) |   |   |   |   | ├─LocalVariableSyntax
//@[018:00020) |   |   |   |   | | └─IdentifierSyntax
//@[018:00020) |   |   |   |   | |   └─Token(Identifier) |i2|
//@[020:00021) |   |   |   |   | └─Token(RightParen) |)|
//@[022:00024) |   |   |   |   ├─Token(Identifier) |in|
//@[025:00036) |   |   |   |   ├─FunctionCallSyntax
//@[025:00030) |   |   |   |   | ├─IdentifierSyntax
//@[025:00030) |   |   |   |   | | └─Token(Identifier) |range|
//@[030:00031) |   |   |   |   | ├─Token(LeftParen) |(|
//@[031:00032) |   |   |   |   | ├─FunctionArgumentSyntax
//@[031:00032) |   |   |   |   | | └─IntegerLiteralSyntax
//@[031:00032) |   |   |   |   | |   └─Token(Integer) |0|
//@[032:00033) |   |   |   |   | ├─Token(Comma) |,|
//@[033:00035) |   |   |   |   | ├─FunctionArgumentSyntax
//@[033:00035) |   |   |   |   | | └─IntegerLiteralSyntax
//@[033:00035) |   |   |   |   | |   └─Token(Integer) |10|
//@[035:00036) |   |   |   |   | └─Token(RightParen) |)|
//@[036:00037) |   |   |   |   ├─Token(Colon) |:|
//@[038:00044) |   |   |   |   ├─BinaryOperationSyntax
//@[038:00039) |   |   |   |   | ├─VariableAccessSyntax
//@[038:00039) |   |   |   |   | | └─IdentifierSyntax
//@[038:00039) |   |   |   |   | |   └─Token(Identifier) |i|
//@[040:00041) |   |   |   |   | ├─Token(Plus) |+|
//@[042:00044) |   |   |   |   | └─VariableAccessSyntax
//@[042:00044) |   |   |   |   |   └─IdentifierSyntax
//@[042:00044) |   |   |   |   |     └─Token(Identifier) |i2|
//@[044:00045) |   |   |   |   └─Token(RightSquare) |]|
//@[045:00047) |   |   |   ├─Token(NewLine) |\r\n|
      b: [for (i, i2) in range(1,2): i / i2]
//@[006:00044) |   |   |   ├─ObjectPropertySyntax
//@[006:00007) |   |   |   | ├─IdentifierSyntax
//@[006:00007) |   |   |   | | └─Token(Identifier) |b|
//@[007:00008) |   |   |   | ├─Token(Colon) |:|
//@[009:00044) |   |   |   | └─ForSyntax
//@[009:00010) |   |   |   |   ├─Token(LeftSquare) |[|
//@[010:00013) |   |   |   |   ├─Token(Identifier) |for|
//@[014:00021) |   |   |   |   ├─VariableBlockSyntax
//@[014:00015) |   |   |   |   | ├─Token(LeftParen) |(|
//@[015:00016) |   |   |   |   | ├─LocalVariableSyntax
//@[015:00016) |   |   |   |   | | └─IdentifierSyntax
//@[015:00016) |   |   |   |   | |   └─Token(Identifier) |i|
//@[016:00017) |   |   |   |   | ├─Token(Comma) |,|
//@[018:00020) |   |   |   |   | ├─LocalVariableSyntax
//@[018:00020) |   |   |   |   | | └─IdentifierSyntax
//@[018:00020) |   |   |   |   | |   └─Token(Identifier) |i2|
//@[020:00021) |   |   |   |   | └─Token(RightParen) |)|
//@[022:00024) |   |   |   |   ├─Token(Identifier) |in|
//@[025:00035) |   |   |   |   ├─FunctionCallSyntax
//@[025:00030) |   |   |   |   | ├─IdentifierSyntax
//@[025:00030) |   |   |   |   | | └─Token(Identifier) |range|
//@[030:00031) |   |   |   |   | ├─Token(LeftParen) |(|
//@[031:00032) |   |   |   |   | ├─FunctionArgumentSyntax
//@[031:00032) |   |   |   |   | | └─IntegerLiteralSyntax
//@[031:00032) |   |   |   |   | |   └─Token(Integer) |1|
//@[032:00033) |   |   |   |   | ├─Token(Comma) |,|
//@[033:00034) |   |   |   |   | ├─FunctionArgumentSyntax
//@[033:00034) |   |   |   |   | | └─IntegerLiteralSyntax
//@[033:00034) |   |   |   |   | |   └─Token(Integer) |2|
//@[034:00035) |   |   |   |   | └─Token(RightParen) |)|
//@[035:00036) |   |   |   |   ├─Token(Colon) |:|
//@[037:00043) |   |   |   |   ├─BinaryOperationSyntax
//@[037:00038) |   |   |   |   | ├─VariableAccessSyntax
//@[037:00038) |   |   |   |   | | └─IdentifierSyntax
//@[037:00038) |   |   |   |   | |   └─Token(Identifier) |i|
//@[039:00040) |   |   |   |   | ├─Token(Slash) |/|
//@[041:00043) |   |   |   |   | └─VariableAccessSyntax
//@[041:00043) |   |   |   |   |   └─IdentifierSyntax
//@[041:00043) |   |   |   |   |     └─Token(Identifier) |i2|
//@[043:00044) |   |   |   |   └─Token(RightSquare) |]|
//@[044:00046) |   |   |   ├─Token(NewLine) |\r\n|
      c: {
//@[006:00067) |   |   |   ├─ObjectPropertySyntax
//@[006:00007) |   |   |   | ├─IdentifierSyntax
//@[006:00007) |   |   |   | | └─Token(Identifier) |c|
//@[007:00008) |   |   |   | ├─Token(Colon) |:|
//@[009:00067) |   |   |   | └─ObjectSyntax
//@[009:00010) |   |   |   |   ├─Token(LeftBrace) |{|
//@[010:00012) |   |   |   |   ├─Token(NewLine) |\r\n|
        d: [for (j, j2) in range(2,3): j * j2]
//@[008:00046) |   |   |   |   ├─ObjectPropertySyntax
//@[008:00009) |   |   |   |   | ├─IdentifierSyntax
//@[008:00009) |   |   |   |   | | └─Token(Identifier) |d|
//@[009:00010) |   |   |   |   | ├─Token(Colon) |:|
//@[011:00046) |   |   |   |   | └─ForSyntax
//@[011:00012) |   |   |   |   |   ├─Token(LeftSquare) |[|
//@[012:00015) |   |   |   |   |   ├─Token(Identifier) |for|
//@[016:00023) |   |   |   |   |   ├─VariableBlockSyntax
//@[016:00017) |   |   |   |   |   | ├─Token(LeftParen) |(|
//@[017:00018) |   |   |   |   |   | ├─LocalVariableSyntax
//@[017:00018) |   |   |   |   |   | | └─IdentifierSyntax
//@[017:00018) |   |   |   |   |   | |   └─Token(Identifier) |j|
//@[018:00019) |   |   |   |   |   | ├─Token(Comma) |,|
//@[020:00022) |   |   |   |   |   | ├─LocalVariableSyntax
//@[020:00022) |   |   |   |   |   | | └─IdentifierSyntax
//@[020:00022) |   |   |   |   |   | |   └─Token(Identifier) |j2|
//@[022:00023) |   |   |   |   |   | └─Token(RightParen) |)|
//@[024:00026) |   |   |   |   |   ├─Token(Identifier) |in|
//@[027:00037) |   |   |   |   |   ├─FunctionCallSyntax
//@[027:00032) |   |   |   |   |   | ├─IdentifierSyntax
//@[027:00032) |   |   |   |   |   | | └─Token(Identifier) |range|
//@[032:00033) |   |   |   |   |   | ├─Token(LeftParen) |(|
//@[033:00034) |   |   |   |   |   | ├─FunctionArgumentSyntax
//@[033:00034) |   |   |   |   |   | | └─IntegerLiteralSyntax
//@[033:00034) |   |   |   |   |   | |   └─Token(Integer) |2|
//@[034:00035) |   |   |   |   |   | ├─Token(Comma) |,|
//@[035:00036) |   |   |   |   |   | ├─FunctionArgumentSyntax
//@[035:00036) |   |   |   |   |   | | └─IntegerLiteralSyntax
//@[035:00036) |   |   |   |   |   | |   └─Token(Integer) |3|
//@[036:00037) |   |   |   |   |   | └─Token(RightParen) |)|
//@[037:00038) |   |   |   |   |   ├─Token(Colon) |:|
//@[039:00045) |   |   |   |   |   ├─BinaryOperationSyntax
//@[039:00040) |   |   |   |   |   | ├─VariableAccessSyntax
//@[039:00040) |   |   |   |   |   | | └─IdentifierSyntax
//@[039:00040) |   |   |   |   |   | |   └─Token(Identifier) |j|
//@[041:00042) |   |   |   |   |   | ├─Token(Asterisk) |*|
//@[043:00045) |   |   |   |   |   | └─VariableAccessSyntax
//@[043:00045) |   |   |   |   |   |   └─IdentifierSyntax
//@[043:00045) |   |   |   |   |   |     └─Token(Identifier) |j2|
//@[045:00046) |   |   |   |   |   └─Token(RightSquare) |]|
//@[046:00048) |   |   |   |   ├─Token(NewLine) |\r\n|
      }
//@[006:00007) |   |   |   |   └─Token(RightBrace) |}|
//@[007:00009) |   |   |   ├─Token(NewLine) |\r\n|
      e: [for (k, k2) in range(4,4): {
//@[006:00077) |   |   |   ├─ObjectPropertySyntax
//@[006:00007) |   |   |   | ├─IdentifierSyntax
//@[006:00007) |   |   |   | | └─Token(Identifier) |e|
//@[007:00008) |   |   |   | ├─Token(Colon) |:|
//@[009:00077) |   |   |   | └─ForSyntax
//@[009:00010) |   |   |   |   ├─Token(LeftSquare) |[|
//@[010:00013) |   |   |   |   ├─Token(Identifier) |for|
//@[014:00021) |   |   |   |   ├─VariableBlockSyntax
//@[014:00015) |   |   |   |   | ├─Token(LeftParen) |(|
//@[015:00016) |   |   |   |   | ├─LocalVariableSyntax
//@[015:00016) |   |   |   |   | | └─IdentifierSyntax
//@[015:00016) |   |   |   |   | |   └─Token(Identifier) |k|
//@[016:00017) |   |   |   |   | ├─Token(Comma) |,|
//@[018:00020) |   |   |   |   | ├─LocalVariableSyntax
//@[018:00020) |   |   |   |   | | └─IdentifierSyntax
//@[018:00020) |   |   |   |   | |   └─Token(Identifier) |k2|
//@[020:00021) |   |   |   |   | └─Token(RightParen) |)|
//@[022:00024) |   |   |   |   ├─Token(Identifier) |in|
//@[025:00035) |   |   |   |   ├─FunctionCallSyntax
//@[025:00030) |   |   |   |   | ├─IdentifierSyntax
//@[025:00030) |   |   |   |   | | └─Token(Identifier) |range|
//@[030:00031) |   |   |   |   | ├─Token(LeftParen) |(|
//@[031:00032) |   |   |   |   | ├─FunctionArgumentSyntax
//@[031:00032) |   |   |   |   | | └─IntegerLiteralSyntax
//@[031:00032) |   |   |   |   | |   └─Token(Integer) |4|
//@[032:00033) |   |   |   |   | ├─Token(Comma) |,|
//@[033:00034) |   |   |   |   | ├─FunctionArgumentSyntax
//@[033:00034) |   |   |   |   | | └─IntegerLiteralSyntax
//@[033:00034) |   |   |   |   | |   └─Token(Integer) |4|
//@[034:00035) |   |   |   |   | └─Token(RightParen) |)|
//@[035:00036) |   |   |   |   ├─Token(Colon) |:|
//@[037:00076) |   |   |   |   ├─ObjectSyntax
//@[037:00038) |   |   |   |   | ├─Token(LeftBrace) |{|
//@[038:00040) |   |   |   |   | ├─Token(NewLine) |\r\n|
        f: k
//@[008:00012) |   |   |   |   | ├─ObjectPropertySyntax
//@[008:00009) |   |   |   |   | | ├─IdentifierSyntax
//@[008:00009) |   |   |   |   | | | └─Token(Identifier) |f|
//@[009:00010) |   |   |   |   | | ├─Token(Colon) |:|
//@[011:00012) |   |   |   |   | | └─VariableAccessSyntax
//@[011:00012) |   |   |   |   | |   └─IdentifierSyntax
//@[011:00012) |   |   |   |   | |     └─Token(Identifier) |k|
//@[012:00014) |   |   |   |   | ├─Token(NewLine) |\r\n|
        g: k2
//@[008:00013) |   |   |   |   | ├─ObjectPropertySyntax
//@[008:00009) |   |   |   |   | | ├─IdentifierSyntax
//@[008:00009) |   |   |   |   | | | └─Token(Identifier) |g|
//@[009:00010) |   |   |   |   | | ├─Token(Colon) |:|
//@[011:00013) |   |   |   |   | | └─VariableAccessSyntax
//@[011:00013) |   |   |   |   | |   └─IdentifierSyntax
//@[011:00013) |   |   |   |   | |     └─Token(Identifier) |k2|
//@[013:00015) |   |   |   |   | ├─Token(NewLine) |\r\n|
      }]
//@[006:00007) |   |   |   |   | └─Token(RightBrace) |}|
//@[007:00008) |   |   |   |   └─Token(RightSquare) |]|
//@[008:00010) |   |   |   ├─Token(NewLine) |\r\n|
    }
//@[004:00005) |   |   |   └─Token(RightBrace) |}|
//@[005:00007) |   |   ├─Token(NewLine) |\r\n|
    stringParamB: ''
//@[004:00020) |   |   ├─ObjectPropertySyntax
//@[004:00016) |   |   | ├─IdentifierSyntax
//@[004:00016) |   |   | | └─Token(Identifier) |stringParamB|
//@[016:00017) |   |   | ├─Token(Colon) |:|
//@[018:00020) |   |   | └─StringSyntax
//@[018:00020) |   |   |   └─Token(StringComplete) |''|
//@[020:00022) |   |   ├─Token(NewLine) |\r\n|
    arrayParam: [
//@[004:00079) |   |   ├─ObjectPropertySyntax
//@[004:00014) |   |   | ├─IdentifierSyntax
//@[004:00014) |   |   | | └─Token(Identifier) |arrayParam|
//@[014:00015) |   |   | ├─Token(Colon) |:|
//@[016:00079) |   |   | └─ArraySyntax
//@[016:00017) |   |   |   ├─Token(LeftSquare) |[|
//@[017:00019) |   |   |   ├─Token(NewLine) |\r\n|
      {
//@[006:00053) |   |   |   ├─ArrayItemSyntax
//@[006:00053) |   |   |   | └─ObjectSyntax
//@[006:00007) |   |   |   |   ├─Token(LeftBrace) |{|
//@[007:00009) |   |   |   |   ├─Token(NewLine) |\r\n|
        e: [for j in range(7,7): j]
//@[008:00035) |   |   |   |   ├─ObjectPropertySyntax
//@[008:00009) |   |   |   |   | ├─IdentifierSyntax
//@[008:00009) |   |   |   |   | | └─Token(Identifier) |e|
//@[009:00010) |   |   |   |   | ├─Token(Colon) |:|
//@[011:00035) |   |   |   |   | └─ForSyntax
//@[011:00012) |   |   |   |   |   ├─Token(LeftSquare) |[|
//@[012:00015) |   |   |   |   |   ├─Token(Identifier) |for|
//@[016:00017) |   |   |   |   |   ├─LocalVariableSyntax
//@[016:00017) |   |   |   |   |   | └─IdentifierSyntax
//@[016:00017) |   |   |   |   |   |   └─Token(Identifier) |j|
//@[018:00020) |   |   |   |   |   ├─Token(Identifier) |in|
//@[021:00031) |   |   |   |   |   ├─FunctionCallSyntax
//@[021:00026) |   |   |   |   |   | ├─IdentifierSyntax
//@[021:00026) |   |   |   |   |   | | └─Token(Identifier) |range|
//@[026:00027) |   |   |   |   |   | ├─Token(LeftParen) |(|
//@[027:00028) |   |   |   |   |   | ├─FunctionArgumentSyntax
//@[027:00028) |   |   |   |   |   | | └─IntegerLiteralSyntax
//@[027:00028) |   |   |   |   |   | |   └─Token(Integer) |7|
//@[028:00029) |   |   |   |   |   | ├─Token(Comma) |,|
//@[029:00030) |   |   |   |   |   | ├─FunctionArgumentSyntax
//@[029:00030) |   |   |   |   |   | | └─IntegerLiteralSyntax
//@[029:00030) |   |   |   |   |   | |   └─Token(Integer) |7|
//@[030:00031) |   |   |   |   |   | └─Token(RightParen) |)|
//@[031:00032) |   |   |   |   |   ├─Token(Colon) |:|
//@[033:00034) |   |   |   |   |   ├─VariableAccessSyntax
//@[033:00034) |   |   |   |   |   | └─IdentifierSyntax
//@[033:00034) |   |   |   |   |   |   └─Token(Identifier) |j|
//@[034:00035) |   |   |   |   |   └─Token(RightSquare) |]|
//@[035:00037) |   |   |   |   ├─Token(NewLine) |\r\n|
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

module propertyLoopInsideParameterValueInsideModuleLoop 'modulea.bicep' = [for thing in range(0,1): {
//@[000:00535) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00055) | ├─IdentifierSyntax
//@[007:00055) | | └─Token(Identifier) |propertyLoopInsideParameterValueInsideModuleLoop|
//@[056:00071) | ├─StringSyntax
//@[056:00071) | | └─Token(StringComplete) |'modulea.bicep'|
//@[072:00073) | ├─Token(Assignment) |=|
//@[074:00535) | └─ForSyntax
//@[074:00075) |   ├─Token(LeftSquare) |[|
//@[075:00078) |   ├─Token(Identifier) |for|
//@[079:00084) |   ├─LocalVariableSyntax
//@[079:00084) |   | └─IdentifierSyntax
//@[079:00084) |   |   └─Token(Identifier) |thing|
//@[085:00087) |   ├─Token(Identifier) |in|
//@[088:00098) |   ├─FunctionCallSyntax
//@[088:00093) |   | ├─IdentifierSyntax
//@[088:00093) |   | | └─Token(Identifier) |range|
//@[093:00094) |   | ├─Token(LeftParen) |(|
//@[094:00095) |   | ├─FunctionArgumentSyntax
//@[094:00095) |   | | └─IntegerLiteralSyntax
//@[094:00095) |   | |   └─Token(Integer) |0|
//@[095:00096) |   | ├─Token(Comma) |,|
//@[096:00097) |   | ├─FunctionArgumentSyntax
//@[096:00097) |   | | └─IntegerLiteralSyntax
//@[096:00097) |   | |   └─Token(Integer) |1|
//@[097:00098) |   | └─Token(RightParen) |)|
//@[098:00099) |   ├─Token(Colon) |:|
//@[100:00534) |   ├─ObjectSyntax
//@[100:00101) |   | ├─Token(LeftBrace) |{|
//@[101:00103) |   | ├─Token(NewLine) |\r\n|
  name: 'propertyLoopInsideParameterValueInsideModuleLoop'
//@[002:00058) |   | ├─ObjectPropertySyntax
//@[002:00006) |   | | ├─IdentifierSyntax
//@[002:00006) |   | | | └─Token(Identifier) |name|
//@[006:00007) |   | | ├─Token(Colon) |:|
//@[008:00058) |   | | └─StringSyntax
//@[008:00058) |   | |   └─Token(StringComplete) |'propertyLoopInsideParameterValueInsideModuleLoop'|
//@[058:00060) |   | ├─Token(NewLine) |\r\n|
  params: {
//@[002:00368) |   | ├─ObjectPropertySyntax
//@[002:00008) |   | | ├─IdentifierSyntax
//@[002:00008) |   | | | └─Token(Identifier) |params|
//@[008:00009) |   | | ├─Token(Colon) |:|
//@[010:00368) |   | | └─ObjectSyntax
//@[010:00011) |   | |   ├─Token(LeftBrace) |{|
//@[011:00013) |   | |   ├─Token(NewLine) |\r\n|
    objParam: {
//@[004:00233) |   | |   ├─ObjectPropertySyntax
//@[004:00012) |   | |   | ├─IdentifierSyntax
//@[004:00012) |   | |   | | └─Token(Identifier) |objParam|
//@[012:00013) |   | |   | ├─Token(Colon) |:|
//@[014:00233) |   | |   | └─ObjectSyntax
//@[014:00015) |   | |   |   ├─Token(LeftBrace) |{|
//@[015:00017) |   | |   |   ├─Token(NewLine) |\r\n|
      a: [for i in range(0,10): i + thing]
//@[006:00042) |   | |   |   ├─ObjectPropertySyntax
//@[006:00007) |   | |   |   | ├─IdentifierSyntax
//@[006:00007) |   | |   |   | | └─Token(Identifier) |a|
//@[007:00008) |   | |   |   | ├─Token(Colon) |:|
//@[009:00042) |   | |   |   | └─ForSyntax
//@[009:00010) |   | |   |   |   ├─Token(LeftSquare) |[|
//@[010:00013) |   | |   |   |   ├─Token(Identifier) |for|
//@[014:00015) |   | |   |   |   ├─LocalVariableSyntax
//@[014:00015) |   | |   |   |   | └─IdentifierSyntax
//@[014:00015) |   | |   |   |   |   └─Token(Identifier) |i|
//@[016:00018) |   | |   |   |   ├─Token(Identifier) |in|
//@[019:00030) |   | |   |   |   ├─FunctionCallSyntax
//@[019:00024) |   | |   |   |   | ├─IdentifierSyntax
//@[019:00024) |   | |   |   |   | | └─Token(Identifier) |range|
//@[024:00025) |   | |   |   |   | ├─Token(LeftParen) |(|
//@[025:00026) |   | |   |   |   | ├─FunctionArgumentSyntax
//@[025:00026) |   | |   |   |   | | └─IntegerLiteralSyntax
//@[025:00026) |   | |   |   |   | |   └─Token(Integer) |0|
//@[026:00027) |   | |   |   |   | ├─Token(Comma) |,|
//@[027:00029) |   | |   |   |   | ├─FunctionArgumentSyntax
//@[027:00029) |   | |   |   |   | | └─IntegerLiteralSyntax
//@[027:00029) |   | |   |   |   | |   └─Token(Integer) |10|
//@[029:00030) |   | |   |   |   | └─Token(RightParen) |)|
//@[030:00031) |   | |   |   |   ├─Token(Colon) |:|
//@[032:00041) |   | |   |   |   ├─BinaryOperationSyntax
//@[032:00033) |   | |   |   |   | ├─VariableAccessSyntax
//@[032:00033) |   | |   |   |   | | └─IdentifierSyntax
//@[032:00033) |   | |   |   |   | |   └─Token(Identifier) |i|
//@[034:00035) |   | |   |   |   | ├─Token(Plus) |+|
//@[036:00041) |   | |   |   |   | └─VariableAccessSyntax
//@[036:00041) |   | |   |   |   |   └─IdentifierSyntax
//@[036:00041) |   | |   |   |   |     └─Token(Identifier) |thing|
//@[041:00042) |   | |   |   |   └─Token(RightSquare) |]|
//@[042:00044) |   | |   |   ├─Token(NewLine) |\r\n|
      b: [for i in range(1,2): i * thing]
//@[006:00041) |   | |   |   ├─ObjectPropertySyntax
//@[006:00007) |   | |   |   | ├─IdentifierSyntax
//@[006:00007) |   | |   |   | | └─Token(Identifier) |b|
//@[007:00008) |   | |   |   | ├─Token(Colon) |:|
//@[009:00041) |   | |   |   | └─ForSyntax
//@[009:00010) |   | |   |   |   ├─Token(LeftSquare) |[|
//@[010:00013) |   | |   |   |   ├─Token(Identifier) |for|
//@[014:00015) |   | |   |   |   ├─LocalVariableSyntax
//@[014:00015) |   | |   |   |   | └─IdentifierSyntax
//@[014:00015) |   | |   |   |   |   └─Token(Identifier) |i|
//@[016:00018) |   | |   |   |   ├─Token(Identifier) |in|
//@[019:00029) |   | |   |   |   ├─FunctionCallSyntax
//@[019:00024) |   | |   |   |   | ├─IdentifierSyntax
//@[019:00024) |   | |   |   |   | | └─Token(Identifier) |range|
//@[024:00025) |   | |   |   |   | ├─Token(LeftParen) |(|
//@[025:00026) |   | |   |   |   | ├─FunctionArgumentSyntax
//@[025:00026) |   | |   |   |   | | └─IntegerLiteralSyntax
//@[025:00026) |   | |   |   |   | |   └─Token(Integer) |1|
//@[026:00027) |   | |   |   |   | ├─Token(Comma) |,|
//@[027:00028) |   | |   |   |   | ├─FunctionArgumentSyntax
//@[027:00028) |   | |   |   |   | | └─IntegerLiteralSyntax
//@[027:00028) |   | |   |   |   | |   └─Token(Integer) |2|
//@[028:00029) |   | |   |   |   | └─Token(RightParen) |)|
//@[029:00030) |   | |   |   |   ├─Token(Colon) |:|
//@[031:00040) |   | |   |   |   ├─BinaryOperationSyntax
//@[031:00032) |   | |   |   |   | ├─VariableAccessSyntax
//@[031:00032) |   | |   |   |   | | └─IdentifierSyntax
//@[031:00032) |   | |   |   |   | |   └─Token(Identifier) |i|
//@[033:00034) |   | |   |   |   | ├─Token(Asterisk) |*|
//@[035:00040) |   | |   |   |   | └─VariableAccessSyntax
//@[035:00040) |   | |   |   |   |   └─IdentifierSyntax
//@[035:00040) |   | |   |   |   |     └─Token(Identifier) |thing|
//@[040:00041) |   | |   |   |   └─Token(RightSquare) |]|
//@[041:00043) |   | |   |   ├─Token(NewLine) |\r\n|
      c: {
//@[006:00056) |   | |   |   ├─ObjectPropertySyntax
//@[006:00007) |   | |   |   | ├─IdentifierSyntax
//@[006:00007) |   | |   |   | | └─Token(Identifier) |c|
//@[007:00008) |   | |   |   | ├─Token(Colon) |:|
//@[009:00056) |   | |   |   | └─ObjectSyntax
//@[009:00010) |   | |   |   |   ├─Token(LeftBrace) |{|
//@[010:00012) |   | |   |   |   ├─Token(NewLine) |\r\n|
        d: [for j in range(2,3): j]
//@[008:00035) |   | |   |   |   ├─ObjectPropertySyntax
//@[008:00009) |   | |   |   |   | ├─IdentifierSyntax
//@[008:00009) |   | |   |   |   | | └─Token(Identifier) |d|
//@[009:00010) |   | |   |   |   | ├─Token(Colon) |:|
//@[011:00035) |   | |   |   |   | └─ForSyntax
//@[011:00012) |   | |   |   |   |   ├─Token(LeftSquare) |[|
//@[012:00015) |   | |   |   |   |   ├─Token(Identifier) |for|
//@[016:00017) |   | |   |   |   |   ├─LocalVariableSyntax
//@[016:00017) |   | |   |   |   |   | └─IdentifierSyntax
//@[016:00017) |   | |   |   |   |   |   └─Token(Identifier) |j|
//@[018:00020) |   | |   |   |   |   ├─Token(Identifier) |in|
//@[021:00031) |   | |   |   |   |   ├─FunctionCallSyntax
//@[021:00026) |   | |   |   |   |   | ├─IdentifierSyntax
//@[021:00026) |   | |   |   |   |   | | └─Token(Identifier) |range|
//@[026:00027) |   | |   |   |   |   | ├─Token(LeftParen) |(|
//@[027:00028) |   | |   |   |   |   | ├─FunctionArgumentSyntax
//@[027:00028) |   | |   |   |   |   | | └─IntegerLiteralSyntax
//@[027:00028) |   | |   |   |   |   | |   └─Token(Integer) |2|
//@[028:00029) |   | |   |   |   |   | ├─Token(Comma) |,|
//@[029:00030) |   | |   |   |   |   | ├─FunctionArgumentSyntax
//@[029:00030) |   | |   |   |   |   | | └─IntegerLiteralSyntax
//@[029:00030) |   | |   |   |   |   | |   └─Token(Integer) |3|
//@[030:00031) |   | |   |   |   |   | └─Token(RightParen) |)|
//@[031:00032) |   | |   |   |   |   ├─Token(Colon) |:|
//@[033:00034) |   | |   |   |   |   ├─VariableAccessSyntax
//@[033:00034) |   | |   |   |   |   | └─IdentifierSyntax
//@[033:00034) |   | |   |   |   |   |   └─Token(Identifier) |j|
//@[034:00035) |   | |   |   |   |   └─Token(RightSquare) |]|
//@[035:00037) |   | |   |   |   ├─Token(NewLine) |\r\n|
      }
//@[006:00007) |   | |   |   |   └─Token(RightBrace) |}|
//@[007:00009) |   | |   |   ├─Token(NewLine) |\r\n|
      e: [for k in range(4,4): {
//@[006:00064) |   | |   |   ├─ObjectPropertySyntax
//@[006:00007) |   | |   |   | ├─IdentifierSyntax
//@[006:00007) |   | |   |   | | └─Token(Identifier) |e|
//@[007:00008) |   | |   |   | ├─Token(Colon) |:|
//@[009:00064) |   | |   |   | └─ForSyntax
//@[009:00010) |   | |   |   |   ├─Token(LeftSquare) |[|
//@[010:00013) |   | |   |   |   ├─Token(Identifier) |for|
//@[014:00015) |   | |   |   |   ├─LocalVariableSyntax
//@[014:00015) |   | |   |   |   | └─IdentifierSyntax
//@[014:00015) |   | |   |   |   |   └─Token(Identifier) |k|
//@[016:00018) |   | |   |   |   ├─Token(Identifier) |in|
//@[019:00029) |   | |   |   |   ├─FunctionCallSyntax
//@[019:00024) |   | |   |   |   | ├─IdentifierSyntax
//@[019:00024) |   | |   |   |   | | └─Token(Identifier) |range|
//@[024:00025) |   | |   |   |   | ├─Token(LeftParen) |(|
//@[025:00026) |   | |   |   |   | ├─FunctionArgumentSyntax
//@[025:00026) |   | |   |   |   | | └─IntegerLiteralSyntax
//@[025:00026) |   | |   |   |   | |   └─Token(Integer) |4|
//@[026:00027) |   | |   |   |   | ├─Token(Comma) |,|
//@[027:00028) |   | |   |   |   | ├─FunctionArgumentSyntax
//@[027:00028) |   | |   |   |   | | └─IntegerLiteralSyntax
//@[027:00028) |   | |   |   |   | |   └─Token(Integer) |4|
//@[028:00029) |   | |   |   |   | └─Token(RightParen) |)|
//@[029:00030) |   | |   |   |   ├─Token(Colon) |:|
//@[031:00063) |   | |   |   |   ├─ObjectSyntax
//@[031:00032) |   | |   |   |   | ├─Token(LeftBrace) |{|
//@[032:00034) |   | |   |   |   | ├─Token(NewLine) |\r\n|
        f: k - thing
//@[008:00020) |   | |   |   |   | ├─ObjectPropertySyntax
//@[008:00009) |   | |   |   |   | | ├─IdentifierSyntax
//@[008:00009) |   | |   |   |   | | | └─Token(Identifier) |f|
//@[009:00010) |   | |   |   |   | | ├─Token(Colon) |:|
//@[011:00020) |   | |   |   |   | | └─BinaryOperationSyntax
//@[011:00012) |   | |   |   |   | |   ├─VariableAccessSyntax
//@[011:00012) |   | |   |   |   | |   | └─IdentifierSyntax
//@[011:00012) |   | |   |   |   | |   |   └─Token(Identifier) |k|
//@[013:00014) |   | |   |   |   | |   ├─Token(Minus) |-|
//@[015:00020) |   | |   |   |   | |   └─VariableAccessSyntax
//@[015:00020) |   | |   |   |   | |     └─IdentifierSyntax
//@[015:00020) |   | |   |   |   | |       └─Token(Identifier) |thing|
//@[020:00022) |   | |   |   |   | ├─Token(NewLine) |\r\n|
      }]
//@[006:00007) |   | |   |   |   | └─Token(RightBrace) |}|
//@[007:00008) |   | |   |   |   └─Token(RightSquare) |]|
//@[008:00010) |   | |   |   ├─Token(NewLine) |\r\n|
    }
//@[004:00005) |   | |   |   └─Token(RightBrace) |}|
//@[005:00007) |   | |   ├─Token(NewLine) |\r\n|
    stringParamB: ''
//@[004:00020) |   | |   ├─ObjectPropertySyntax
//@[004:00016) |   | |   | ├─IdentifierSyntax
//@[004:00016) |   | |   | | └─Token(Identifier) |stringParamB|
//@[016:00017) |   | |   | ├─Token(Colon) |:|
//@[018:00020) |   | |   | └─StringSyntax
//@[018:00020) |   | |   |   └─Token(StringComplete) |''|
//@[020:00022) |   | |   ├─Token(NewLine) |\r\n|
    arrayParam: [
//@[004:00093) |   | |   ├─ObjectPropertySyntax
//@[004:00014) |   | |   | ├─IdentifierSyntax
//@[004:00014) |   | |   | | └─Token(Identifier) |arrayParam|
//@[014:00015) |   | |   | ├─Token(Colon) |:|
//@[016:00093) |   | |   | └─ArraySyntax
//@[016:00017) |   | |   |   ├─Token(LeftSquare) |[|
//@[017:00019) |   | |   |   ├─Token(NewLine) |\r\n|
      {
//@[006:00067) |   | |   |   ├─ArrayItemSyntax
//@[006:00067) |   | |   |   | └─ObjectSyntax
//@[006:00007) |   | |   |   |   ├─Token(LeftBrace) |{|
//@[007:00009) |   | |   |   |   ├─Token(NewLine) |\r\n|
        e: [for j in range(7,7): j % (thing + 1)]
//@[008:00049) |   | |   |   |   ├─ObjectPropertySyntax
//@[008:00009) |   | |   |   |   | ├─IdentifierSyntax
//@[008:00009) |   | |   |   |   | | └─Token(Identifier) |e|
//@[009:00010) |   | |   |   |   | ├─Token(Colon) |:|
//@[011:00049) |   | |   |   |   | └─ForSyntax
//@[011:00012) |   | |   |   |   |   ├─Token(LeftSquare) |[|
//@[012:00015) |   | |   |   |   |   ├─Token(Identifier) |for|
//@[016:00017) |   | |   |   |   |   ├─LocalVariableSyntax
//@[016:00017) |   | |   |   |   |   | └─IdentifierSyntax
//@[016:00017) |   | |   |   |   |   |   └─Token(Identifier) |j|
//@[018:00020) |   | |   |   |   |   ├─Token(Identifier) |in|
//@[021:00031) |   | |   |   |   |   ├─FunctionCallSyntax
//@[021:00026) |   | |   |   |   |   | ├─IdentifierSyntax
//@[021:00026) |   | |   |   |   |   | | └─Token(Identifier) |range|
//@[026:00027) |   | |   |   |   |   | ├─Token(LeftParen) |(|
//@[027:00028) |   | |   |   |   |   | ├─FunctionArgumentSyntax
//@[027:00028) |   | |   |   |   |   | | └─IntegerLiteralSyntax
//@[027:00028) |   | |   |   |   |   | |   └─Token(Integer) |7|
//@[028:00029) |   | |   |   |   |   | ├─Token(Comma) |,|
//@[029:00030) |   | |   |   |   |   | ├─FunctionArgumentSyntax
//@[029:00030) |   | |   |   |   |   | | └─IntegerLiteralSyntax
//@[029:00030) |   | |   |   |   |   | |   └─Token(Integer) |7|
//@[030:00031) |   | |   |   |   |   | └─Token(RightParen) |)|
//@[031:00032) |   | |   |   |   |   ├─Token(Colon) |:|
//@[033:00048) |   | |   |   |   |   ├─BinaryOperationSyntax
//@[033:00034) |   | |   |   |   |   | ├─VariableAccessSyntax
//@[033:00034) |   | |   |   |   |   | | └─IdentifierSyntax
//@[033:00034) |   | |   |   |   |   | |   └─Token(Identifier) |j|
//@[035:00036) |   | |   |   |   |   | ├─Token(Modulo) |%|
//@[037:00048) |   | |   |   |   |   | └─ParenthesizedExpressionSyntax
//@[037:00038) |   | |   |   |   |   |   ├─Token(LeftParen) |(|
//@[038:00047) |   | |   |   |   |   |   ├─BinaryOperationSyntax
//@[038:00043) |   | |   |   |   |   |   | ├─VariableAccessSyntax
//@[038:00043) |   | |   |   |   |   |   | | └─IdentifierSyntax
//@[038:00043) |   | |   |   |   |   |   | |   └─Token(Identifier) |thing|
//@[044:00045) |   | |   |   |   |   |   | ├─Token(Plus) |+|
//@[046:00047) |   | |   |   |   |   |   | └─IntegerLiteralSyntax
//@[046:00047) |   | |   |   |   |   |   |   └─Token(Integer) |1|
//@[047:00048) |   | |   |   |   |   |   └─Token(RightParen) |)|
//@[048:00049) |   | |   |   |   |   └─Token(RightSquare) |]|
//@[049:00051) |   | |   |   |   ├─Token(NewLine) |\r\n|
      }
//@[006:00007) |   | |   |   |   └─Token(RightBrace) |}|
//@[007:00009) |   | |   |   ├─Token(NewLine) |\r\n|
    ]
//@[004:00005) |   | |   |   └─Token(RightSquare) |]|
//@[005:00007) |   | |   ├─Token(NewLine) |\r\n|
  }
//@[002:00003) |   | |   └─Token(RightBrace) |}|
//@[003:00005) |   | ├─Token(NewLine) |\r\n|
}]
//@[000:00001) |   | └─Token(RightBrace) |}|
//@[001:00002) |   └─Token(RightSquare) |]|
//@[002:00008) ├─Token(NewLine) |\r\n\r\n\r\n|


// BEGIN: Key Vault Secret Reference
//@[036:00040) ├─Token(NewLine) |\r\n\r\n|

resource kv 'Microsoft.KeyVault/vaults@2019-09-01' existing = {
//@[000:00090) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00011) | ├─IdentifierSyntax
//@[009:00011) | | └─Token(Identifier) |kv|
//@[012:00050) | ├─StringSyntax
//@[012:00050) | | └─Token(StringComplete) |'Microsoft.KeyVault/vaults@2019-09-01'|
//@[051:00059) | ├─Token(Identifier) |existing|
//@[060:00061) | ├─Token(Assignment) |=|
//@[062:00090) | └─ObjectSyntax
//@[062:00063) |   ├─Token(LeftBrace) |{|
//@[063:00065) |   ├─Token(NewLine) |\r\n|
  name: 'testkeyvault'
//@[002:00022) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00022) |   | └─StringSyntax
//@[008:00022) |   |   └─Token(StringComplete) |'testkeyvault'|
//@[022:00024) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

module secureModule1 'child/secureParams.bicep' = {
//@[000:00213) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00020) | ├─IdentifierSyntax
//@[007:00020) | | └─Token(Identifier) |secureModule1|
//@[021:00047) | ├─StringSyntax
//@[021:00047) | | └─Token(StringComplete) |'child/secureParams.bicep'|
//@[048:00049) | ├─Token(Assignment) |=|
//@[050:00213) | └─ObjectSyntax
//@[050:00051) |   ├─Token(LeftBrace) |{|
//@[051:00053) |   ├─Token(NewLine) |\r\n|
  name: 'secureModule1'
//@[002:00023) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00023) |   | └─StringSyntax
//@[008:00023) |   |   └─Token(StringComplete) |'secureModule1'|
//@[023:00025) |   ├─Token(NewLine) |\r\n|
  params: {
//@[002:00132) |   ├─ObjectPropertySyntax
//@[002:00008) |   | ├─IdentifierSyntax
//@[002:00008) |   | | └─Token(Identifier) |params|
//@[008:00009) |   | ├─Token(Colon) |:|
//@[010:00132) |   | └─ObjectSyntax
//@[010:00011) |   |   ├─Token(LeftBrace) |{|
//@[011:00013) |   |   ├─Token(NewLine) |\r\n|
    secureStringParam1: kv.getSecret('mySecret')
//@[004:00048) |   |   ├─ObjectPropertySyntax
//@[004:00022) |   |   | ├─IdentifierSyntax
//@[004:00022) |   |   | | └─Token(Identifier) |secureStringParam1|
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
//@[048:00050) |   |   ├─Token(NewLine) |\r\n|
    secureStringParam2: kv.getSecret('mySecret','secretVersion')
//@[004:00064) |   |   ├─ObjectPropertySyntax
//@[004:00022) |   |   | ├─IdentifierSyntax
//@[004:00022) |   |   | | └─Token(Identifier) |secureStringParam2|
//@[022:00023) |   |   | ├─Token(Colon) |:|
//@[024:00064) |   |   | └─InstanceFunctionCallSyntax
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
//@[047:00048) |   |   |   ├─Token(Comma) |,|
//@[048:00063) |   |   |   ├─FunctionArgumentSyntax
//@[048:00063) |   |   |   | └─StringSyntax
//@[048:00063) |   |   |   |   └─Token(StringComplete) |'secretVersion'|
//@[063:00064) |   |   |   └─Token(RightParen) |)|
//@[064:00066) |   |   ├─Token(NewLine) |\r\n|
  }
//@[002:00003) |   |   └─Token(RightBrace) |}|
//@[003:00005) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

resource scopedKv 'Microsoft.KeyVault/vaults@2019-09-01' existing = {
//@[000:00134) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00017) | ├─IdentifierSyntax
//@[009:00017) | | └─Token(Identifier) |scopedKv|
//@[018:00056) | ├─StringSyntax
//@[018:00056) | | └─Token(StringComplete) |'Microsoft.KeyVault/vaults@2019-09-01'|
//@[057:00065) | ├─Token(Identifier) |existing|
//@[066:00067) | ├─Token(Assignment) |=|
//@[068:00134) | └─ObjectSyntax
//@[068:00069) |   ├─Token(LeftBrace) |{|
//@[069:00071) |   ├─Token(NewLine) |\r\n|
  name: 'testkeyvault'
//@[002:00022) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00022) |   | └─StringSyntax
//@[008:00022) |   |   └─Token(StringComplete) |'testkeyvault'|
//@[022:00024) |   ├─Token(NewLine) |\r\n|
  scope: resourceGroup('otherGroup')
//@[002:00036) |   ├─ObjectPropertySyntax
//@[002:00007) |   | ├─IdentifierSyntax
//@[002:00007) |   | | └─Token(Identifier) |scope|
//@[007:00008) |   | ├─Token(Colon) |:|
//@[009:00036) |   | └─FunctionCallSyntax
//@[009:00022) |   |   ├─IdentifierSyntax
//@[009:00022) |   |   | └─Token(Identifier) |resourceGroup|
//@[022:00023) |   |   ├─Token(LeftParen) |(|
//@[023:00035) |   |   ├─FunctionArgumentSyntax
//@[023:00035) |   |   | └─StringSyntax
//@[023:00035) |   |   |   └─Token(StringComplete) |'otherGroup'|
//@[035:00036) |   |   └─Token(RightParen) |)|
//@[036:00038) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

module secureModule2 'child/secureParams.bicep' = {
//@[000:00225) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00020) | ├─IdentifierSyntax
//@[007:00020) | | └─Token(Identifier) |secureModule2|
//@[021:00047) | ├─StringSyntax
//@[021:00047) | | └─Token(StringComplete) |'child/secureParams.bicep'|
//@[048:00049) | ├─Token(Assignment) |=|
//@[050:00225) | └─ObjectSyntax
//@[050:00051) |   ├─Token(LeftBrace) |{|
//@[051:00053) |   ├─Token(NewLine) |\r\n|
  name: 'secureModule2'
//@[002:00023) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00023) |   | └─StringSyntax
//@[008:00023) |   |   └─Token(StringComplete) |'secureModule2'|
//@[023:00025) |   ├─Token(NewLine) |\r\n|
  params: {
//@[002:00144) |   ├─ObjectPropertySyntax
//@[002:00008) |   | ├─IdentifierSyntax
//@[002:00008) |   | | └─Token(Identifier) |params|
//@[008:00009) |   | ├─Token(Colon) |:|
//@[010:00144) |   | └─ObjectSyntax
//@[010:00011) |   |   ├─Token(LeftBrace) |{|
//@[011:00013) |   |   ├─Token(NewLine) |\r\n|
    secureStringParam1: scopedKv.getSecret('mySecret')
//@[004:00054) |   |   ├─ObjectPropertySyntax
//@[004:00022) |   |   | ├─IdentifierSyntax
//@[004:00022) |   |   | | └─Token(Identifier) |secureStringParam1|
//@[022:00023) |   |   | ├─Token(Colon) |:|
//@[024:00054) |   |   | └─InstanceFunctionCallSyntax
//@[024:00032) |   |   |   ├─VariableAccessSyntax
//@[024:00032) |   |   |   | └─IdentifierSyntax
//@[024:00032) |   |   |   |   └─Token(Identifier) |scopedKv|
//@[032:00033) |   |   |   ├─Token(Dot) |.|
//@[033:00042) |   |   |   ├─IdentifierSyntax
//@[033:00042) |   |   |   | └─Token(Identifier) |getSecret|
//@[042:00043) |   |   |   ├─Token(LeftParen) |(|
//@[043:00053) |   |   |   ├─FunctionArgumentSyntax
//@[043:00053) |   |   |   | └─StringSyntax
//@[043:00053) |   |   |   |   └─Token(StringComplete) |'mySecret'|
//@[053:00054) |   |   |   └─Token(RightParen) |)|
//@[054:00056) |   |   ├─Token(NewLine) |\r\n|
    secureStringParam2: scopedKv.getSecret('mySecret','secretVersion')
//@[004:00070) |   |   ├─ObjectPropertySyntax
//@[004:00022) |   |   | ├─IdentifierSyntax
//@[004:00022) |   |   | | └─Token(Identifier) |secureStringParam2|
//@[022:00023) |   |   | ├─Token(Colon) |:|
//@[024:00070) |   |   | └─InstanceFunctionCallSyntax
//@[024:00032) |   |   |   ├─VariableAccessSyntax
//@[024:00032) |   |   |   | └─IdentifierSyntax
//@[024:00032) |   |   |   |   └─Token(Identifier) |scopedKv|
//@[032:00033) |   |   |   ├─Token(Dot) |.|
//@[033:00042) |   |   |   ├─IdentifierSyntax
//@[033:00042) |   |   |   | └─Token(Identifier) |getSecret|
//@[042:00043) |   |   |   ├─Token(LeftParen) |(|
//@[043:00053) |   |   |   ├─FunctionArgumentSyntax
//@[043:00053) |   |   |   | └─StringSyntax
//@[043:00053) |   |   |   |   └─Token(StringComplete) |'mySecret'|
//@[053:00054) |   |   |   ├─Token(Comma) |,|
//@[054:00069) |   |   |   ├─FunctionArgumentSyntax
//@[054:00069) |   |   |   | └─StringSyntax
//@[054:00069) |   |   |   |   └─Token(StringComplete) |'secretVersion'|
//@[069:00070) |   |   |   └─Token(RightParen) |)|
//@[070:00072) |   |   ├─Token(NewLine) |\r\n|
  }
//@[002:00003) |   |   └─Token(RightBrace) |}|
//@[003:00005) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

//looped module with looped existing resource (Issue #2862)
//@[059:00061) ├─Token(NewLine) |\r\n|
var vaults = [
//@[000:00200) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00010) | ├─IdentifierSyntax
//@[004:00010) | | └─Token(Identifier) |vaults|
//@[011:00012) | ├─Token(Assignment) |=|
//@[013:00200) | └─ArraySyntax
//@[013:00014) |   ├─Token(LeftSquare) |[|
//@[014:00016) |   ├─Token(NewLine) |\r\n|
  {
//@[002:00089) |   ├─ArrayItemSyntax
//@[002:00089) |   | └─ObjectSyntax
//@[002:00003) |   |   ├─Token(LeftBrace) |{|
//@[003:00005) |   |   ├─Token(NewLine) |\r\n|
    vaultName: 'test-1-kv'
//@[004:00026) |   |   ├─ObjectPropertySyntax
//@[004:00013) |   |   | ├─IdentifierSyntax
//@[004:00013) |   |   | | └─Token(Identifier) |vaultName|
//@[013:00014) |   |   | ├─Token(Colon) |:|
//@[015:00026) |   |   | └─StringSyntax
//@[015:00026) |   |   |   └─Token(StringComplete) |'test-1-kv'|
//@[026:00028) |   |   ├─Token(NewLine) |\r\n|
    vaultRG: 'test-1-rg'
//@[004:00024) |   |   ├─ObjectPropertySyntax
//@[004:00011) |   |   | ├─IdentifierSyntax
//@[004:00011) |   |   | | └─Token(Identifier) |vaultRG|
//@[011:00012) |   |   | ├─Token(Colon) |:|
//@[013:00024) |   |   | └─StringSyntax
//@[013:00024) |   |   |   └─Token(StringComplete) |'test-1-rg'|
//@[024:00026) |   |   ├─Token(NewLine) |\r\n|
    vaultSub: 'abcd-efgh'
//@[004:00025) |   |   ├─ObjectPropertySyntax
//@[004:00012) |   |   | ├─IdentifierSyntax
//@[004:00012) |   |   | | └─Token(Identifier) |vaultSub|
//@[012:00013) |   |   | ├─Token(Colon) |:|
//@[014:00025) |   |   | └─StringSyntax
//@[014:00025) |   |   |   └─Token(StringComplete) |'abcd-efgh'|
//@[025:00027) |   |   ├─Token(NewLine) |\r\n|
  }
//@[002:00003) |   |   └─Token(RightBrace) |}|
//@[003:00005) |   ├─Token(NewLine) |\r\n|
  {
//@[002:00090) |   ├─ArrayItemSyntax
//@[002:00090) |   | └─ObjectSyntax
//@[002:00003) |   |   ├─Token(LeftBrace) |{|
//@[003:00005) |   |   ├─Token(NewLine) |\r\n|
    vaultName: 'test-2-kv'
//@[004:00026) |   |   ├─ObjectPropertySyntax
//@[004:00013) |   |   | ├─IdentifierSyntax
//@[004:00013) |   |   | | └─Token(Identifier) |vaultName|
//@[013:00014) |   |   | ├─Token(Colon) |:|
//@[015:00026) |   |   | └─StringSyntax
//@[015:00026) |   |   |   └─Token(StringComplete) |'test-2-kv'|
//@[026:00028) |   |   ├─Token(NewLine) |\r\n|
    vaultRG: 'test-2-rg'
//@[004:00024) |   |   ├─ObjectPropertySyntax
//@[004:00011) |   |   | ├─IdentifierSyntax
//@[004:00011) |   |   | | └─Token(Identifier) |vaultRG|
//@[011:00012) |   |   | ├─Token(Colon) |:|
//@[013:00024) |   |   | └─StringSyntax
//@[013:00024) |   |   |   └─Token(StringComplete) |'test-2-rg'|
//@[024:00026) |   |   ├─Token(NewLine) |\r\n|
    vaultSub: 'ijkl-1adg1'
//@[004:00026) |   |   ├─ObjectPropertySyntax
//@[004:00012) |   |   | ├─IdentifierSyntax
//@[004:00012) |   |   | | └─Token(Identifier) |vaultSub|
//@[012:00013) |   |   | ├─Token(Colon) |:|
//@[014:00026) |   |   | └─StringSyntax
//@[014:00026) |   |   |   └─Token(StringComplete) |'ijkl-1adg1'|
//@[026:00028) |   |   ├─Token(NewLine) |\r\n|
  }
//@[002:00003) |   |   └─Token(RightBrace) |}|
//@[003:00005) |   ├─Token(NewLine) |\r\n|
]
//@[000:00001) |   └─Token(RightSquare) |]|
//@[001:00003) ├─Token(NewLine) |\r\n|
var secrets = [
//@[000:00132) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00011) | ├─IdentifierSyntax
//@[004:00011) | | └─Token(Identifier) |secrets|
//@[012:00013) | ├─Token(Assignment) |=|
//@[014:00132) | └─ArraySyntax
//@[014:00015) |   ├─Token(LeftSquare) |[|
//@[015:00017) |   ├─Token(NewLine) |\r\n|
  {
//@[002:00055) |   ├─ArrayItemSyntax
//@[002:00055) |   | └─ObjectSyntax
//@[002:00003) |   |   ├─Token(LeftBrace) |{|
//@[003:00005) |   |   ├─Token(NewLine) |\r\n|
    name: 'secret01'
//@[004:00020) |   |   ├─ObjectPropertySyntax
//@[004:00008) |   |   | ├─IdentifierSyntax
//@[004:00008) |   |   | | └─Token(Identifier) |name|
//@[008:00009) |   |   | ├─Token(Colon) |:|
//@[010:00020) |   |   | └─StringSyntax
//@[010:00020) |   |   |   └─Token(StringComplete) |'secret01'|
//@[020:00022) |   |   ├─Token(NewLine) |\r\n|
    version: 'versionA'
//@[004:00023) |   |   ├─ObjectPropertySyntax
//@[004:00011) |   |   | ├─IdentifierSyntax
//@[004:00011) |   |   | | └─Token(Identifier) |version|
//@[011:00012) |   |   | ├─Token(Colon) |:|
//@[013:00023) |   |   | └─StringSyntax
//@[013:00023) |   |   |   └─Token(StringComplete) |'versionA'|
//@[023:00025) |   |   ├─Token(NewLine) |\r\n|
  }
//@[002:00003) |   |   └─Token(RightBrace) |}|
//@[003:00005) |   ├─Token(NewLine) |\r\n|
  {
//@[002:00055) |   ├─ArrayItemSyntax
//@[002:00055) |   | └─ObjectSyntax
//@[002:00003) |   |   ├─Token(LeftBrace) |{|
//@[003:00005) |   |   ├─Token(NewLine) |\r\n|
    name: 'secret02'
//@[004:00020) |   |   ├─ObjectPropertySyntax
//@[004:00008) |   |   | ├─IdentifierSyntax
//@[004:00008) |   |   | | └─Token(Identifier) |name|
//@[008:00009) |   |   | ├─Token(Colon) |:|
//@[010:00020) |   |   | └─StringSyntax
//@[010:00020) |   |   |   └─Token(StringComplete) |'secret02'|
//@[020:00022) |   |   ├─Token(NewLine) |\r\n|
    version: 'versionB'
//@[004:00023) |   |   ├─ObjectPropertySyntax
//@[004:00011) |   |   | ├─IdentifierSyntax
//@[004:00011) |   |   | | └─Token(Identifier) |version|
//@[011:00012) |   |   | ├─Token(Colon) |:|
//@[013:00023) |   |   | └─StringSyntax
//@[013:00023) |   |   |   └─Token(StringComplete) |'versionB'|
//@[023:00025) |   |   ├─Token(NewLine) |\r\n|
  }
//@[002:00003) |   |   └─Token(RightBrace) |}|
//@[003:00005) |   ├─Token(NewLine) |\r\n|
]
//@[000:00001) |   └─Token(RightSquare) |]|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

resource loopedKv 'Microsoft.KeyVault/vaults@2019-09-01' existing = [for vault in vaults: {
//@[000:00175) ├─ResourceDeclarationSyntax
//@[000:00008) | ├─Token(Identifier) |resource|
//@[009:00017) | ├─IdentifierSyntax
//@[009:00017) | | └─Token(Identifier) |loopedKv|
//@[018:00056) | ├─StringSyntax
//@[018:00056) | | └─Token(StringComplete) |'Microsoft.KeyVault/vaults@2019-09-01'|
//@[057:00065) | ├─Token(Identifier) |existing|
//@[066:00067) | ├─Token(Assignment) |=|
//@[068:00175) | └─ForSyntax
//@[068:00069) |   ├─Token(LeftSquare) |[|
//@[069:00072) |   ├─Token(Identifier) |for|
//@[073:00078) |   ├─LocalVariableSyntax
//@[073:00078) |   | └─IdentifierSyntax
//@[073:00078) |   |   └─Token(Identifier) |vault|
//@[079:00081) |   ├─Token(Identifier) |in|
//@[082:00088) |   ├─VariableAccessSyntax
//@[082:00088) |   | └─IdentifierSyntax
//@[082:00088) |   |   └─Token(Identifier) |vaults|
//@[088:00089) |   ├─Token(Colon) |:|
//@[090:00174) |   ├─ObjectSyntax
//@[090:00091) |   | ├─Token(LeftBrace) |{|
//@[091:00093) |   | ├─Token(NewLine) |\r\n|
  name: vault.vaultName
//@[002:00023) |   | ├─ObjectPropertySyntax
//@[002:00006) |   | | ├─IdentifierSyntax
//@[002:00006) |   | | | └─Token(Identifier) |name|
//@[006:00007) |   | | ├─Token(Colon) |:|
//@[008:00023) |   | | └─PropertyAccessSyntax
//@[008:00013) |   | |   ├─VariableAccessSyntax
//@[008:00013) |   | |   | └─IdentifierSyntax
//@[008:00013) |   | |   |   └─Token(Identifier) |vault|
//@[013:00014) |   | |   ├─Token(Dot) |.|
//@[014:00023) |   | |   └─IdentifierSyntax
//@[014:00023) |   | |     └─Token(Identifier) |vaultName|
//@[023:00025) |   | ├─Token(NewLine) |\r\n|
  scope: resourceGroup(vault.vaultSub, vault.vaultRG)
//@[002:00053) |   | ├─ObjectPropertySyntax
//@[002:00007) |   | | ├─IdentifierSyntax
//@[002:00007) |   | | | └─Token(Identifier) |scope|
//@[007:00008) |   | | ├─Token(Colon) |:|
//@[009:00053) |   | | └─FunctionCallSyntax
//@[009:00022) |   | |   ├─IdentifierSyntax
//@[009:00022) |   | |   | └─Token(Identifier) |resourceGroup|
//@[022:00023) |   | |   ├─Token(LeftParen) |(|
//@[023:00037) |   | |   ├─FunctionArgumentSyntax
//@[023:00037) |   | |   | └─PropertyAccessSyntax
//@[023:00028) |   | |   |   ├─VariableAccessSyntax
//@[023:00028) |   | |   |   | └─IdentifierSyntax
//@[023:00028) |   | |   |   |   └─Token(Identifier) |vault|
//@[028:00029) |   | |   |   ├─Token(Dot) |.|
//@[029:00037) |   | |   |   └─IdentifierSyntax
//@[029:00037) |   | |   |     └─Token(Identifier) |vaultSub|
//@[037:00038) |   | |   ├─Token(Comma) |,|
//@[039:00052) |   | |   ├─FunctionArgumentSyntax
//@[039:00052) |   | |   | └─PropertyAccessSyntax
//@[039:00044) |   | |   |   ├─VariableAccessSyntax
//@[039:00044) |   | |   |   | └─IdentifierSyntax
//@[039:00044) |   | |   |   |   └─Token(Identifier) |vault|
//@[044:00045) |   | |   |   ├─Token(Dot) |.|
//@[045:00052) |   | |   |   └─IdentifierSyntax
//@[045:00052) |   | |   |     └─Token(Identifier) |vaultRG|
//@[052:00053) |   | |   └─Token(RightParen) |)|
//@[053:00055) |   | ├─Token(NewLine) |\r\n|
}]
//@[000:00001) |   | └─Token(RightBrace) |}|
//@[001:00002) |   └─Token(RightSquare) |]|
//@[002:00006) ├─Token(NewLine) |\r\n\r\n|

module secureModuleLooped 'child/secureParams.bicep' = [for (secret, i) in secrets: {
//@[000:00278) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00025) | ├─IdentifierSyntax
//@[007:00025) | | └─Token(Identifier) |secureModuleLooped|
//@[026:00052) | ├─StringSyntax
//@[026:00052) | | └─Token(StringComplete) |'child/secureParams.bicep'|
//@[053:00054) | ├─Token(Assignment) |=|
//@[055:00278) | └─ForSyntax
//@[055:00056) |   ├─Token(LeftSquare) |[|
//@[056:00059) |   ├─Token(Identifier) |for|
//@[060:00071) |   ├─VariableBlockSyntax
//@[060:00061) |   | ├─Token(LeftParen) |(|
//@[061:00067) |   | ├─LocalVariableSyntax
//@[061:00067) |   | | └─IdentifierSyntax
//@[061:00067) |   | |   └─Token(Identifier) |secret|
//@[067:00068) |   | ├─Token(Comma) |,|
//@[069:00070) |   | ├─LocalVariableSyntax
//@[069:00070) |   | | └─IdentifierSyntax
//@[069:00070) |   | |   └─Token(Identifier) |i|
//@[070:00071) |   | └─Token(RightParen) |)|
//@[072:00074) |   ├─Token(Identifier) |in|
//@[075:00082) |   ├─VariableAccessSyntax
//@[075:00082) |   | └─IdentifierSyntax
//@[075:00082) |   |   └─Token(Identifier) |secrets|
//@[082:00083) |   ├─Token(Colon) |:|
//@[084:00277) |   ├─ObjectSyntax
//@[084:00085) |   | ├─Token(LeftBrace) |{|
//@[085:00087) |   | ├─Token(NewLine) |\r\n|
  name: 'secureModuleLooped-${i}'
//@[002:00033) |   | ├─ObjectPropertySyntax
//@[002:00006) |   | | ├─IdentifierSyntax
//@[002:00006) |   | | | └─Token(Identifier) |name|
//@[006:00007) |   | | ├─Token(Colon) |:|
//@[008:00033) |   | | └─StringSyntax
//@[008:00030) |   | |   ├─Token(StringLeftPiece) |'secureModuleLooped-${|
//@[030:00031) |   | |   ├─VariableAccessSyntax
//@[030:00031) |   | |   | └─IdentifierSyntax
//@[030:00031) |   | |   |   └─Token(Identifier) |i|
//@[031:00033) |   | |   └─Token(StringRightPiece) |}'|
//@[033:00035) |   | ├─Token(NewLine) |\r\n|
  params: {
//@[002:00152) |   | ├─ObjectPropertySyntax
//@[002:00008) |   | | ├─IdentifierSyntax
//@[002:00008) |   | | | └─Token(Identifier) |params|
//@[008:00009) |   | | ├─Token(Colon) |:|
//@[010:00152) |   | | └─ObjectSyntax
//@[010:00011) |   | |   ├─Token(LeftBrace) |{|
//@[011:00013) |   | |   ├─Token(NewLine) |\r\n|
    secureStringParam1: loopedKv[i].getSecret(secret.name)
//@[004:00058) |   | |   ├─ObjectPropertySyntax
//@[004:00022) |   | |   | ├─IdentifierSyntax
//@[004:00022) |   | |   | | └─Token(Identifier) |secureStringParam1|
//@[022:00023) |   | |   | ├─Token(Colon) |:|
//@[024:00058) |   | |   | └─InstanceFunctionCallSyntax
//@[024:00035) |   | |   |   ├─ArrayAccessSyntax
//@[024:00032) |   | |   |   | ├─VariableAccessSyntax
//@[024:00032) |   | |   |   | | └─IdentifierSyntax
//@[024:00032) |   | |   |   | |   └─Token(Identifier) |loopedKv|
//@[032:00033) |   | |   |   | ├─Token(LeftSquare) |[|
//@[033:00034) |   | |   |   | ├─VariableAccessSyntax
//@[033:00034) |   | |   |   | | └─IdentifierSyntax
//@[033:00034) |   | |   |   | |   └─Token(Identifier) |i|
//@[034:00035) |   | |   |   | └─Token(RightSquare) |]|
//@[035:00036) |   | |   |   ├─Token(Dot) |.|
//@[036:00045) |   | |   |   ├─IdentifierSyntax
//@[036:00045) |   | |   |   | └─Token(Identifier) |getSecret|
//@[045:00046) |   | |   |   ├─Token(LeftParen) |(|
//@[046:00057) |   | |   |   ├─FunctionArgumentSyntax
//@[046:00057) |   | |   |   | └─PropertyAccessSyntax
//@[046:00052) |   | |   |   |   ├─VariableAccessSyntax
//@[046:00052) |   | |   |   |   | └─IdentifierSyntax
//@[046:00052) |   | |   |   |   |   └─Token(Identifier) |secret|
//@[052:00053) |   | |   |   |   ├─Token(Dot) |.|
//@[053:00057) |   | |   |   |   └─IdentifierSyntax
//@[053:00057) |   | |   |   |     └─Token(Identifier) |name|
//@[057:00058) |   | |   |   └─Token(RightParen) |)|
//@[058:00060) |   | |   ├─Token(NewLine) |\r\n|
    secureStringParam2: loopedKv[i].getSecret(secret.name, secret.version)
//@[004:00074) |   | |   ├─ObjectPropertySyntax
//@[004:00022) |   | |   | ├─IdentifierSyntax
//@[004:00022) |   | |   | | └─Token(Identifier) |secureStringParam2|
//@[022:00023) |   | |   | ├─Token(Colon) |:|
//@[024:00074) |   | |   | └─InstanceFunctionCallSyntax
//@[024:00035) |   | |   |   ├─ArrayAccessSyntax
//@[024:00032) |   | |   |   | ├─VariableAccessSyntax
//@[024:00032) |   | |   |   | | └─IdentifierSyntax
//@[024:00032) |   | |   |   | |   └─Token(Identifier) |loopedKv|
//@[032:00033) |   | |   |   | ├─Token(LeftSquare) |[|
//@[033:00034) |   | |   |   | ├─VariableAccessSyntax
//@[033:00034) |   | |   |   | | └─IdentifierSyntax
//@[033:00034) |   | |   |   | |   └─Token(Identifier) |i|
//@[034:00035) |   | |   |   | └─Token(RightSquare) |]|
//@[035:00036) |   | |   |   ├─Token(Dot) |.|
//@[036:00045) |   | |   |   ├─IdentifierSyntax
//@[036:00045) |   | |   |   | └─Token(Identifier) |getSecret|
//@[045:00046) |   | |   |   ├─Token(LeftParen) |(|
//@[046:00057) |   | |   |   ├─FunctionArgumentSyntax
//@[046:00057) |   | |   |   | └─PropertyAccessSyntax
//@[046:00052) |   | |   |   |   ├─VariableAccessSyntax
//@[046:00052) |   | |   |   |   | └─IdentifierSyntax
//@[046:00052) |   | |   |   |   |   └─Token(Identifier) |secret|
//@[052:00053) |   | |   |   |   ├─Token(Dot) |.|
//@[053:00057) |   | |   |   |   └─IdentifierSyntax
//@[053:00057) |   | |   |   |     └─Token(Identifier) |name|
//@[057:00058) |   | |   |   ├─Token(Comma) |,|
//@[059:00073) |   | |   |   ├─FunctionArgumentSyntax
//@[059:00073) |   | |   |   | └─PropertyAccessSyntax
//@[059:00065) |   | |   |   |   ├─VariableAccessSyntax
//@[059:00065) |   | |   |   |   | └─IdentifierSyntax
//@[059:00065) |   | |   |   |   |   └─Token(Identifier) |secret|
//@[065:00066) |   | |   |   |   ├─Token(Dot) |.|
//@[066:00073) |   | |   |   |   └─IdentifierSyntax
//@[066:00073) |   | |   |   |     └─Token(Identifier) |version|
//@[073:00074) |   | |   |   └─Token(RightParen) |)|
//@[074:00076) |   | |   ├─Token(NewLine) |\r\n|
  }
//@[002:00003) |   | |   └─Token(RightBrace) |}|
//@[003:00005) |   | ├─Token(NewLine) |\r\n|
}]
//@[000:00001) |   | └─Token(RightBrace) |}|
//@[001:00002) |   └─Token(RightSquare) |]|
//@[002:00006) ├─Token(NewLine) |\r\n\r\n|

module secureModuleCondition 'child/secureParams.bicep' = {
//@[000:00285) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00028) | ├─IdentifierSyntax
//@[007:00028) | | └─Token(Identifier) |secureModuleCondition|
//@[029:00055) | ├─StringSyntax
//@[029:00055) | | └─Token(StringComplete) |'child/secureParams.bicep'|
//@[056:00057) | ├─Token(Assignment) |=|
//@[058:00285) | └─ObjectSyntax
//@[058:00059) |   ├─Token(LeftBrace) |{|
//@[059:00061) |   ├─Token(NewLine) |\r\n|
  name: 'secureModuleCondition'
//@[002:00031) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00031) |   | └─StringSyntax
//@[008:00031) |   |   └─Token(StringComplete) |'secureModuleCondition'|
//@[031:00033) |   ├─Token(NewLine) |\r\n|
  params: {
//@[002:00188) |   ├─ObjectPropertySyntax
//@[002:00008) |   | ├─IdentifierSyntax
//@[002:00008) |   | | └─Token(Identifier) |params|
//@[008:00009) |   | ├─Token(Colon) |:|
//@[010:00188) |   | └─ObjectSyntax
//@[010:00011) |   |   ├─Token(LeftBrace) |{|
//@[011:00013) |   |   ├─Token(NewLine) |\r\n|
    secureStringParam1: true ? kv.getSecret('mySecret') : 'notTrue'
//@[004:00067) |   |   ├─ObjectPropertySyntax
//@[004:00022) |   |   | ├─IdentifierSyntax
//@[004:00022) |   |   | | └─Token(Identifier) |secureStringParam1|
//@[022:00023) |   |   | ├─Token(Colon) |:|
//@[024:00067) |   |   | └─TernaryOperationSyntax
//@[024:00028) |   |   |   ├─BooleanLiteralSyntax
//@[024:00028) |   |   |   | └─Token(TrueKeyword) |true|
//@[029:00030) |   |   |   ├─Token(Question) |?|
//@[031:00055) |   |   |   ├─InstanceFunctionCallSyntax
//@[031:00033) |   |   |   | ├─VariableAccessSyntax
//@[031:00033) |   |   |   | | └─IdentifierSyntax
//@[031:00033) |   |   |   | |   └─Token(Identifier) |kv|
//@[033:00034) |   |   |   | ├─Token(Dot) |.|
//@[034:00043) |   |   |   | ├─IdentifierSyntax
//@[034:00043) |   |   |   | | └─Token(Identifier) |getSecret|
//@[043:00044) |   |   |   | ├─Token(LeftParen) |(|
//@[044:00054) |   |   |   | ├─FunctionArgumentSyntax
//@[044:00054) |   |   |   | | └─StringSyntax
//@[044:00054) |   |   |   | |   └─Token(StringComplete) |'mySecret'|
//@[054:00055) |   |   |   | └─Token(RightParen) |)|
//@[056:00057) |   |   |   ├─Token(Colon) |:|
//@[058:00067) |   |   |   └─StringSyntax
//@[058:00067) |   |   |     └─Token(StringComplete) |'notTrue'|
//@[067:00069) |   |   ├─Token(NewLine) |\r\n|
    secureStringParam2: true ? false ? 'false' : kv.getSecret('mySecret','secretVersion') : 'notTrue'
//@[004:00101) |   |   ├─ObjectPropertySyntax
//@[004:00022) |   |   | ├─IdentifierSyntax
//@[004:00022) |   |   | | └─Token(Identifier) |secureStringParam2|
//@[022:00023) |   |   | ├─Token(Colon) |:|
//@[024:00101) |   |   | └─TernaryOperationSyntax
//@[024:00028) |   |   |   ├─BooleanLiteralSyntax
//@[024:00028) |   |   |   | └─Token(TrueKeyword) |true|
//@[029:00030) |   |   |   ├─Token(Question) |?|
//@[031:00089) |   |   |   ├─TernaryOperationSyntax
//@[031:00036) |   |   |   | ├─BooleanLiteralSyntax
//@[031:00036) |   |   |   | | └─Token(FalseKeyword) |false|
//@[037:00038) |   |   |   | ├─Token(Question) |?|
//@[039:00046) |   |   |   | ├─StringSyntax
//@[039:00046) |   |   |   | | └─Token(StringComplete) |'false'|
//@[047:00048) |   |   |   | ├─Token(Colon) |:|
//@[049:00089) |   |   |   | └─InstanceFunctionCallSyntax
//@[049:00051) |   |   |   |   ├─VariableAccessSyntax
//@[049:00051) |   |   |   |   | └─IdentifierSyntax
//@[049:00051) |   |   |   |   |   └─Token(Identifier) |kv|
//@[051:00052) |   |   |   |   ├─Token(Dot) |.|
//@[052:00061) |   |   |   |   ├─IdentifierSyntax
//@[052:00061) |   |   |   |   | └─Token(Identifier) |getSecret|
//@[061:00062) |   |   |   |   ├─Token(LeftParen) |(|
//@[062:00072) |   |   |   |   ├─FunctionArgumentSyntax
//@[062:00072) |   |   |   |   | └─StringSyntax
//@[062:00072) |   |   |   |   |   └─Token(StringComplete) |'mySecret'|
//@[072:00073) |   |   |   |   ├─Token(Comma) |,|
//@[073:00088) |   |   |   |   ├─FunctionArgumentSyntax
//@[073:00088) |   |   |   |   | └─StringSyntax
//@[073:00088) |   |   |   |   |   └─Token(StringComplete) |'secretVersion'|
//@[088:00089) |   |   |   |   └─Token(RightParen) |)|
//@[090:00091) |   |   |   ├─Token(Colon) |:|
//@[092:00101) |   |   |   └─StringSyntax
//@[092:00101) |   |   |     └─Token(StringComplete) |'notTrue'|
//@[101:00103) |   |   ├─Token(NewLine) |\r\n|
  }
//@[002:00003) |   |   └─Token(RightBrace) |}|
//@[003:00005) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

// END: Key Vault Secret Reference
//@[034:00038) ├─Token(NewLine) |\r\n\r\n|

module withSpace 'module with space.bicep' = {
//@[000:00070) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00016) | ├─IdentifierSyntax
//@[007:00016) | | └─Token(Identifier) |withSpace|
//@[017:00042) | ├─StringSyntax
//@[017:00042) | | └─Token(StringComplete) |'module with space.bicep'|
//@[043:00044) | ├─Token(Assignment) |=|
//@[045:00070) | └─ObjectSyntax
//@[045:00046) |   ├─Token(LeftBrace) |{|
//@[046:00048) |   ├─Token(NewLine) |\r\n|
  name: 'withSpace'
//@[002:00019) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00019) |   | └─StringSyntax
//@[008:00019) |   |   └─Token(StringComplete) |'withSpace'|
//@[019:00021) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

module folderWithSpace 'child/folder with space/child with space.bicep' = {
//@[000:00104) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00022) | ├─IdentifierSyntax
//@[007:00022) | | └─Token(Identifier) |folderWithSpace|
//@[023:00071) | ├─StringSyntax
//@[023:00071) | | └─Token(StringComplete) |'child/folder with space/child with space.bicep'|
//@[072:00073) | ├─Token(Assignment) |=|
//@[074:00104) | └─ObjectSyntax
//@[074:00075) |   ├─Token(LeftBrace) |{|
//@[075:00077) |   ├─Token(NewLine) |\r\n|
  name: 'childWithSpace'
//@[002:00024) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00024) |   | └─StringSyntax
//@[008:00024) |   |   └─Token(StringComplete) |'childWithSpace'|
//@[024:00026) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

// nameof
//@[009:00013) ├─Token(NewLine) |\r\n\r\n|

var nameofModule = nameof(folderWithSpace)
//@[000:00042) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00016) | ├─IdentifierSyntax
//@[004:00016) | | └─Token(Identifier) |nameofModule|
//@[017:00018) | ├─Token(Assignment) |=|
//@[019:00042) | └─FunctionCallSyntax
//@[019:00025) |   ├─IdentifierSyntax
//@[019:00025) |   | └─Token(Identifier) |nameof|
//@[025:00026) |   ├─Token(LeftParen) |(|
//@[026:00041) |   ├─FunctionArgumentSyntax
//@[026:00041) |   | └─VariableAccessSyntax
//@[026:00041) |   |   └─IdentifierSyntax
//@[026:00041) |   |     └─Token(Identifier) |folderWithSpace|
//@[041:00042) |   └─Token(RightParen) |)|
//@[042:00044) ├─Token(NewLine) |\r\n|
var nameofModuleParam = nameof(secureModuleCondition.outputs.exposedSecureString)
//@[000:00081) ├─VariableDeclarationSyntax
//@[000:00003) | ├─Token(Identifier) |var|
//@[004:00021) | ├─IdentifierSyntax
//@[004:00021) | | └─Token(Identifier) |nameofModuleParam|
//@[022:00023) | ├─Token(Assignment) |=|
//@[024:00081) | └─FunctionCallSyntax
//@[024:00030) |   ├─IdentifierSyntax
//@[024:00030) |   | └─Token(Identifier) |nameof|
//@[030:00031) |   ├─Token(LeftParen) |(|
//@[031:00080) |   ├─FunctionArgumentSyntax
//@[031:00080) |   | └─PropertyAccessSyntax
//@[031:00060) |   |   ├─PropertyAccessSyntax
//@[031:00052) |   |   | ├─VariableAccessSyntax
//@[031:00052) |   |   | | └─IdentifierSyntax
//@[031:00052) |   |   | |   └─Token(Identifier) |secureModuleCondition|
//@[052:00053) |   |   | ├─Token(Dot) |.|
//@[053:00060) |   |   | └─IdentifierSyntax
//@[053:00060) |   |   |   └─Token(Identifier) |outputs|
//@[060:00061) |   |   ├─Token(Dot) |.|
//@[061:00080) |   |   └─IdentifierSyntax
//@[061:00080) |   |     └─Token(Identifier) |exposedSecureString|
//@[080:00081) |   └─Token(RightParen) |)|
//@[081:00085) ├─Token(NewLine) |\r\n\r\n|

module moduleWithNameof 'modulea.bicep' = {
//@[000:00358) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00023) | ├─IdentifierSyntax
//@[007:00023) | | └─Token(Identifier) |moduleWithNameof|
//@[024:00039) | ├─StringSyntax
//@[024:00039) | | └─Token(StringComplete) |'modulea.bicep'|
//@[040:00041) | ├─Token(Assignment) |=|
//@[042:00358) | └─ObjectSyntax
//@[042:00043) |   ├─Token(LeftBrace) |{|
//@[043:00045) |   ├─Token(NewLine) |\r\n|
  name: 'nameofModule'
//@[002:00022) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00022) |   | └─StringSyntax
//@[008:00022) |   |   └─Token(StringComplete) |'nameofModule'|
//@[022:00024) |   ├─Token(NewLine) |\r\n|
  scope: resourceGroup(nameof(nameofModuleParam))
//@[002:00049) |   ├─ObjectPropertySyntax
//@[002:00007) |   | ├─IdentifierSyntax
//@[002:00007) |   | | └─Token(Identifier) |scope|
//@[007:00008) |   | ├─Token(Colon) |:|
//@[009:00049) |   | └─FunctionCallSyntax
//@[009:00022) |   |   ├─IdentifierSyntax
//@[009:00022) |   |   | └─Token(Identifier) |resourceGroup|
//@[022:00023) |   |   ├─Token(LeftParen) |(|
//@[023:00048) |   |   ├─FunctionArgumentSyntax
//@[023:00048) |   |   | └─FunctionCallSyntax
//@[023:00029) |   |   |   ├─IdentifierSyntax
//@[023:00029) |   |   |   | └─Token(Identifier) |nameof|
//@[029:00030) |   |   |   ├─Token(LeftParen) |(|
//@[030:00047) |   |   |   ├─FunctionArgumentSyntax
//@[030:00047) |   |   |   | └─VariableAccessSyntax
//@[030:00047) |   |   |   |   └─IdentifierSyntax
//@[030:00047) |   |   |   |     └─Token(Identifier) |nameofModuleParam|
//@[047:00048) |   |   |   └─Token(RightParen) |)|
//@[048:00049) |   |   └─Token(RightParen) |)|
//@[049:00051) |   ├─Token(NewLine) |\r\n|
  params:{
//@[002:00235) |   ├─ObjectPropertySyntax
//@[002:00008) |   | ├─IdentifierSyntax
//@[002:00008) |   | | └─Token(Identifier) |params|
//@[008:00009) |   | ├─Token(Colon) |:|
//@[009:00235) |   | └─ObjectSyntax
//@[009:00010) |   |   ├─Token(LeftBrace) |{|
//@[010:00012) |   |   ├─Token(NewLine) |\r\n|
    stringParamA: nameof(withSpace)
//@[004:00035) |   |   ├─ObjectPropertySyntax
//@[004:00016) |   |   | ├─IdentifierSyntax
//@[004:00016) |   |   | | └─Token(Identifier) |stringParamA|
//@[016:00017) |   |   | ├─Token(Colon) |:|
//@[018:00035) |   |   | └─FunctionCallSyntax
//@[018:00024) |   |   |   ├─IdentifierSyntax
//@[018:00024) |   |   |   | └─Token(Identifier) |nameof|
//@[024:00025) |   |   |   ├─Token(LeftParen) |(|
//@[025:00034) |   |   |   ├─FunctionArgumentSyntax
//@[025:00034) |   |   |   | └─VariableAccessSyntax
//@[025:00034) |   |   |   |   └─IdentifierSyntax
//@[025:00034) |   |   |   |     └─Token(Identifier) |withSpace|
//@[034:00035) |   |   |   └─Token(RightParen) |)|
//@[035:00037) |   |   ├─Token(NewLine) |\r\n|
    stringParamB: nameof(folderWithSpace)
//@[004:00041) |   |   ├─ObjectPropertySyntax
//@[004:00016) |   |   | ├─IdentifierSyntax
//@[004:00016) |   |   | | └─Token(Identifier) |stringParamB|
//@[016:00017) |   |   | ├─Token(Colon) |:|
//@[018:00041) |   |   | └─FunctionCallSyntax
//@[018:00024) |   |   |   ├─IdentifierSyntax
//@[018:00024) |   |   |   | └─Token(Identifier) |nameof|
//@[024:00025) |   |   |   ├─Token(LeftParen) |(|
//@[025:00040) |   |   |   ├─FunctionArgumentSyntax
//@[025:00040) |   |   |   | └─VariableAccessSyntax
//@[025:00040) |   |   |   |   └─IdentifierSyntax
//@[025:00040) |   |   |   |     └─Token(Identifier) |folderWithSpace|
//@[040:00041) |   |   |   └─Token(RightParen) |)|
//@[041:00043) |   |   ├─Token(NewLine) |\r\n|
    objParam: {
//@[004:00090) |   |   ├─ObjectPropertySyntax
//@[004:00012) |   |   | ├─IdentifierSyntax
//@[004:00012) |   |   | | └─Token(Identifier) |objParam|
//@[012:00013) |   |   | ├─Token(Colon) |:|
//@[014:00090) |   |   | └─ObjectSyntax
//@[014:00015) |   |   |   ├─Token(LeftBrace) |{|
//@[015:00017) |   |   |   ├─Token(NewLine) |\r\n|
      a: nameof(secureModuleCondition.outputs.exposedSecureString)
//@[006:00066) |   |   |   ├─ObjectPropertySyntax
//@[006:00007) |   |   |   | ├─IdentifierSyntax
//@[006:00007) |   |   |   | | └─Token(Identifier) |a|
//@[007:00008) |   |   |   | ├─Token(Colon) |:|
//@[009:00066) |   |   |   | └─FunctionCallSyntax
//@[009:00015) |   |   |   |   ├─IdentifierSyntax
//@[009:00015) |   |   |   |   | └─Token(Identifier) |nameof|
//@[015:00016) |   |   |   |   ├─Token(LeftParen) |(|
//@[016:00065) |   |   |   |   ├─FunctionArgumentSyntax
//@[016:00065) |   |   |   |   | └─PropertyAccessSyntax
//@[016:00045) |   |   |   |   |   ├─PropertyAccessSyntax
//@[016:00037) |   |   |   |   |   | ├─VariableAccessSyntax
//@[016:00037) |   |   |   |   |   | | └─IdentifierSyntax
//@[016:00037) |   |   |   |   |   | |   └─Token(Identifier) |secureModuleCondition|
//@[037:00038) |   |   |   |   |   | ├─Token(Dot) |.|
//@[038:00045) |   |   |   |   |   | └─IdentifierSyntax
//@[038:00045) |   |   |   |   |   |   └─Token(Identifier) |outputs|
//@[045:00046) |   |   |   |   |   ├─Token(Dot) |.|
//@[046:00065) |   |   |   |   |   └─IdentifierSyntax
//@[046:00065) |   |   |   |   |     └─Token(Identifier) |exposedSecureString|
//@[065:00066) |   |   |   |   └─Token(RightParen) |)|
//@[066:00068) |   |   |   ├─Token(NewLine) |\r\n|
    }
//@[004:00005) |   |   |   └─Token(RightBrace) |}|
//@[005:00007) |   |   ├─Token(NewLine) |\r\n|
    arrayParam: [
//@[004:00046) |   |   ├─ObjectPropertySyntax
//@[004:00014) |   |   | ├─IdentifierSyntax
//@[004:00014) |   |   | | └─Token(Identifier) |arrayParam|
//@[014:00015) |   |   | ├─Token(Colon) |:|
//@[016:00046) |   |   | └─ArraySyntax
//@[016:00017) |   |   |   ├─Token(LeftSquare) |[|
//@[017:00019) |   |   |   ├─Token(NewLine) |\r\n|
      nameof(vaults)
//@[006:00020) |   |   |   ├─ArrayItemSyntax
//@[006:00020) |   |   |   | └─FunctionCallSyntax
//@[006:00012) |   |   |   |   ├─IdentifierSyntax
//@[006:00012) |   |   |   |   | └─Token(Identifier) |nameof|
//@[012:00013) |   |   |   |   ├─Token(LeftParen) |(|
//@[013:00019) |   |   |   |   ├─FunctionArgumentSyntax
//@[013:00019) |   |   |   |   | └─VariableAccessSyntax
//@[013:00019) |   |   |   |   |   └─IdentifierSyntax
//@[013:00019) |   |   |   |   |     └─Token(Identifier) |vaults|
//@[019:00020) |   |   |   |   └─Token(RightParen) |)|
//@[020:00022) |   |   |   ├─Token(NewLine) |\r\n|
    ]
//@[004:00005) |   |   |   └─Token(RightSquare) |]|
//@[005:00007) |   |   ├─Token(NewLine) |\r\n|
  }
//@[002:00003) |   |   └─Token(RightBrace) |}|
//@[003:00005) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

module moduleWithNullableOutputs 'child/nullableOutputs.bicep' = {
//@[000:00096) ├─ModuleDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |module|
//@[007:00032) | ├─IdentifierSyntax
//@[007:00032) | | └─Token(Identifier) |moduleWithNullableOutputs|
//@[033:00062) | ├─StringSyntax
//@[033:00062) | | └─Token(StringComplete) |'child/nullableOutputs.bicep'|
//@[063:00064) | ├─Token(Assignment) |=|
//@[065:00096) | └─ObjectSyntax
//@[065:00066) |   ├─Token(LeftBrace) |{|
//@[066:00068) |   ├─Token(NewLine) |\r\n|
  name: 'nullableOutputs'
//@[002:00025) |   ├─ObjectPropertySyntax
//@[002:00006) |   | ├─IdentifierSyntax
//@[002:00006) |   | | └─Token(Identifier) |name|
//@[006:00007) |   | ├─Token(Colon) |:|
//@[008:00025) |   | └─StringSyntax
//@[008:00025) |   |   └─Token(StringComplete) |'nullableOutputs'|
//@[025:00027) |   ├─Token(NewLine) |\r\n|
}
//@[000:00001) |   └─Token(RightBrace) |}|
//@[001:00005) ├─Token(NewLine) |\r\n\r\n|

output nullableString string? = moduleWithNullableOutputs.outputs.?nullableString
//@[000:00081) ├─OutputDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |output|
//@[007:00021) | ├─IdentifierSyntax
//@[007:00021) | | └─Token(Identifier) |nullableString|
//@[022:00029) | ├─NullableTypeSyntax
//@[022:00028) | | ├─TypeVariableAccessSyntax
//@[022:00028) | | | └─IdentifierSyntax
//@[022:00028) | | |   └─Token(Identifier) |string|
//@[028:00029) | | └─Token(Question) |?|
//@[030:00031) | ├─Token(Assignment) |=|
//@[032:00081) | └─PropertyAccessSyntax
//@[032:00065) |   ├─PropertyAccessSyntax
//@[032:00057) |   | ├─VariableAccessSyntax
//@[032:00057) |   | | └─IdentifierSyntax
//@[032:00057) |   | |   └─Token(Identifier) |moduleWithNullableOutputs|
//@[057:00058) |   | ├─Token(Dot) |.|
//@[058:00065) |   | └─IdentifierSyntax
//@[058:00065) |   |   └─Token(Identifier) |outputs|
//@[065:00066) |   ├─Token(Dot) |.|
//@[066:00067) |   ├─Token(Question) |?|
//@[067:00081) |   └─IdentifierSyntax
//@[067:00081) |     └─Token(Identifier) |nullableString|
//@[081:00083) ├─Token(NewLine) |\r\n|
output deeplyNestedProperty string? = moduleWithNullableOutputs.outputs.?nullableObj.deeply.nested.property
//@[000:00107) ├─OutputDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |output|
//@[007:00027) | ├─IdentifierSyntax
//@[007:00027) | | └─Token(Identifier) |deeplyNestedProperty|
//@[028:00035) | ├─NullableTypeSyntax
//@[028:00034) | | ├─TypeVariableAccessSyntax
//@[028:00034) | | | └─IdentifierSyntax
//@[028:00034) | | |   └─Token(Identifier) |string|
//@[034:00035) | | └─Token(Question) |?|
//@[036:00037) | ├─Token(Assignment) |=|
//@[038:00107) | └─PropertyAccessSyntax
//@[038:00098) |   ├─PropertyAccessSyntax
//@[038:00091) |   | ├─PropertyAccessSyntax
//@[038:00084) |   | | ├─PropertyAccessSyntax
//@[038:00071) |   | | | ├─PropertyAccessSyntax
//@[038:00063) |   | | | | ├─VariableAccessSyntax
//@[038:00063) |   | | | | | └─IdentifierSyntax
//@[038:00063) |   | | | | |   └─Token(Identifier) |moduleWithNullableOutputs|
//@[063:00064) |   | | | | ├─Token(Dot) |.|
//@[064:00071) |   | | | | └─IdentifierSyntax
//@[064:00071) |   | | | |   └─Token(Identifier) |outputs|
//@[071:00072) |   | | | ├─Token(Dot) |.|
//@[072:00073) |   | | | ├─Token(Question) |?|
//@[073:00084) |   | | | └─IdentifierSyntax
//@[073:00084) |   | | |   └─Token(Identifier) |nullableObj|
//@[084:00085) |   | | ├─Token(Dot) |.|
//@[085:00091) |   | | └─IdentifierSyntax
//@[085:00091) |   | |   └─Token(Identifier) |deeply|
//@[091:00092) |   | ├─Token(Dot) |.|
//@[092:00098) |   | └─IdentifierSyntax
//@[092:00098) |   |   └─Token(Identifier) |nested|
//@[098:00099) |   ├─Token(Dot) |.|
//@[099:00107) |   └─IdentifierSyntax
//@[099:00107) |     └─Token(Identifier) |property|
//@[107:00109) ├─Token(NewLine) |\r\n|
output deeplyNestedArrayItem string? = moduleWithNullableOutputs.outputs.?nullableObj.deeply.nested.array[0]
//@[000:00108) ├─OutputDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |output|
//@[007:00028) | ├─IdentifierSyntax
//@[007:00028) | | └─Token(Identifier) |deeplyNestedArrayItem|
//@[029:00036) | ├─NullableTypeSyntax
//@[029:00035) | | ├─TypeVariableAccessSyntax
//@[029:00035) | | | └─IdentifierSyntax
//@[029:00035) | | |   └─Token(Identifier) |string|
//@[035:00036) | | └─Token(Question) |?|
//@[037:00038) | ├─Token(Assignment) |=|
//@[039:00108) | └─ArrayAccessSyntax
//@[039:00105) |   ├─PropertyAccessSyntax
//@[039:00099) |   | ├─PropertyAccessSyntax
//@[039:00092) |   | | ├─PropertyAccessSyntax
//@[039:00085) |   | | | ├─PropertyAccessSyntax
//@[039:00072) |   | | | | ├─PropertyAccessSyntax
//@[039:00064) |   | | | | | ├─VariableAccessSyntax
//@[039:00064) |   | | | | | | └─IdentifierSyntax
//@[039:00064) |   | | | | | |   └─Token(Identifier) |moduleWithNullableOutputs|
//@[064:00065) |   | | | | | ├─Token(Dot) |.|
//@[065:00072) |   | | | | | └─IdentifierSyntax
//@[065:00072) |   | | | | |   └─Token(Identifier) |outputs|
//@[072:00073) |   | | | | ├─Token(Dot) |.|
//@[073:00074) |   | | | | ├─Token(Question) |?|
//@[074:00085) |   | | | | └─IdentifierSyntax
//@[074:00085) |   | | | |   └─Token(Identifier) |nullableObj|
//@[085:00086) |   | | | ├─Token(Dot) |.|
//@[086:00092) |   | | | └─IdentifierSyntax
//@[086:00092) |   | | |   └─Token(Identifier) |deeply|
//@[092:00093) |   | | ├─Token(Dot) |.|
//@[093:00099) |   | | └─IdentifierSyntax
//@[093:00099) |   | |   └─Token(Identifier) |nested|
//@[099:00100) |   | ├─Token(Dot) |.|
//@[100:00105) |   | └─IdentifierSyntax
//@[100:00105) |   |   └─Token(Identifier) |array|
//@[105:00106) |   ├─Token(LeftSquare) |[|
//@[106:00107) |   ├─IntegerLiteralSyntax
//@[106:00107) |   | └─Token(Integer) |0|
//@[107:00108) |   └─Token(RightSquare) |]|
//@[108:00110) ├─Token(NewLine) |\r\n|
output deeplyNestedArrayItemFromEnd string? = moduleWithNullableOutputs.outputs.?nullableObj.deeply.nested.array[^1]
//@[000:00116) ├─OutputDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |output|
//@[007:00035) | ├─IdentifierSyntax
//@[007:00035) | | └─Token(Identifier) |deeplyNestedArrayItemFromEnd|
//@[036:00043) | ├─NullableTypeSyntax
//@[036:00042) | | ├─TypeVariableAccessSyntax
//@[036:00042) | | | └─IdentifierSyntax
//@[036:00042) | | |   └─Token(Identifier) |string|
//@[042:00043) | | └─Token(Question) |?|
//@[044:00045) | ├─Token(Assignment) |=|
//@[046:00116) | └─ArrayAccessSyntax
//@[046:00112) |   ├─PropertyAccessSyntax
//@[046:00106) |   | ├─PropertyAccessSyntax
//@[046:00099) |   | | ├─PropertyAccessSyntax
//@[046:00092) |   | | | ├─PropertyAccessSyntax
//@[046:00079) |   | | | | ├─PropertyAccessSyntax
//@[046:00071) |   | | | | | ├─VariableAccessSyntax
//@[046:00071) |   | | | | | | └─IdentifierSyntax
//@[046:00071) |   | | | | | |   └─Token(Identifier) |moduleWithNullableOutputs|
//@[071:00072) |   | | | | | ├─Token(Dot) |.|
//@[072:00079) |   | | | | | └─IdentifierSyntax
//@[072:00079) |   | | | | |   └─Token(Identifier) |outputs|
//@[079:00080) |   | | | | ├─Token(Dot) |.|
//@[080:00081) |   | | | | ├─Token(Question) |?|
//@[081:00092) |   | | | | └─IdentifierSyntax
//@[081:00092) |   | | | |   └─Token(Identifier) |nullableObj|
//@[092:00093) |   | | | ├─Token(Dot) |.|
//@[093:00099) |   | | | └─IdentifierSyntax
//@[093:00099) |   | | |   └─Token(Identifier) |deeply|
//@[099:00100) |   | | ├─Token(Dot) |.|
//@[100:00106) |   | | └─IdentifierSyntax
//@[100:00106) |   | |   └─Token(Identifier) |nested|
//@[106:00107) |   | ├─Token(Dot) |.|
//@[107:00112) |   | └─IdentifierSyntax
//@[107:00112) |   |   └─Token(Identifier) |array|
//@[112:00113) |   ├─Token(LeftSquare) |[|
//@[113:00114) |   ├─Token(Hat) |^|
//@[114:00115) |   ├─IntegerLiteralSyntax
//@[114:00115) |   | └─Token(Integer) |1|
//@[115:00116) |   └─Token(RightSquare) |]|
//@[116:00118) ├─Token(NewLine) |\r\n|
output deeplyNestedArrayItemFromEndAttempt string? = moduleWithNullableOutputs.outputs.?nullableObj.deeply.nested.array[?^1]
//@[000:00124) ├─OutputDeclarationSyntax
//@[000:00006) | ├─Token(Identifier) |output|
//@[007:00042) | ├─IdentifierSyntax
//@[007:00042) | | └─Token(Identifier) |deeplyNestedArrayItemFromEndAttempt|
//@[043:00050) | ├─NullableTypeSyntax
//@[043:00049) | | ├─TypeVariableAccessSyntax
//@[043:00049) | | | └─IdentifierSyntax
//@[043:00049) | | |   └─Token(Identifier) |string|
//@[049:00050) | | └─Token(Question) |?|
//@[051:00052) | ├─Token(Assignment) |=|
//@[053:00124) | └─ArrayAccessSyntax
//@[053:00119) |   ├─PropertyAccessSyntax
//@[053:00113) |   | ├─PropertyAccessSyntax
//@[053:00106) |   | | ├─PropertyAccessSyntax
//@[053:00099) |   | | | ├─PropertyAccessSyntax
//@[053:00086) |   | | | | ├─PropertyAccessSyntax
//@[053:00078) |   | | | | | ├─VariableAccessSyntax
//@[053:00078) |   | | | | | | └─IdentifierSyntax
//@[053:00078) |   | | | | | |   └─Token(Identifier) |moduleWithNullableOutputs|
//@[078:00079) |   | | | | | ├─Token(Dot) |.|
//@[079:00086) |   | | | | | └─IdentifierSyntax
//@[079:00086) |   | | | | |   └─Token(Identifier) |outputs|
//@[086:00087) |   | | | | ├─Token(Dot) |.|
//@[087:00088) |   | | | | ├─Token(Question) |?|
//@[088:00099) |   | | | | └─IdentifierSyntax
//@[088:00099) |   | | | |   └─Token(Identifier) |nullableObj|
//@[099:00100) |   | | | ├─Token(Dot) |.|
//@[100:00106) |   | | | └─IdentifierSyntax
//@[100:00106) |   | | |   └─Token(Identifier) |deeply|
//@[106:00107) |   | | ├─Token(Dot) |.|
//@[107:00113) |   | | └─IdentifierSyntax
//@[107:00113) |   | |   └─Token(Identifier) |nested|
//@[113:00114) |   | ├─Token(Dot) |.|
//@[114:00119) |   | └─IdentifierSyntax
//@[114:00119) |   |   └─Token(Identifier) |array|
//@[119:00120) |   ├─Token(LeftSquare) |[|
//@[120:00121) |   ├─Token(Question) |?|
//@[121:00122) |   ├─Token(Hat) |^|
//@[122:00123) |   ├─IntegerLiteralSyntax
//@[122:00123) |   | └─Token(Integer) |1|
//@[123:00124) |   └─Token(RightSquare) |]|
//@[124:00126) ├─Token(NewLine) |\r\n|

//@[000:00000) └─Token(EndOfFile) ||
