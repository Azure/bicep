targetScope = 'tenant'
//@[000:1047) ProgramSyntax
//@[000:0022) ├─TargetScopeSyntax
//@[000:0011) | ├─Token(Identifier) |targetScope|
//@[012:0013) | ├─Token(Assignment) |=|
//@[014:0022) | └─StringSyntax
//@[014:0022) |   └─Token(StringComplete) |'tenant'|
//@[022:0024) ├─Token(NewLine) |\n\n|

module myManagementGroupMod 'modules/managementgroup.bicep' = {
//@[000:0142) ├─ModuleDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |module|
//@[007:0027) | ├─IdentifierSyntax
//@[007:0027) | | └─Token(Identifier) |myManagementGroupMod|
//@[028:0059) | ├─StringSyntax
//@[028:0059) | | └─Token(StringComplete) |'modules/managementgroup.bicep'|
//@[060:0061) | ├─Token(Assignment) |=|
//@[062:0142) | └─ObjectSyntax
//@[062:0063) |   ├─Token(LeftBrace) |{|
//@[063:0064) |   ├─Token(NewLine) |\n|
  name: 'myManagementGroupMod'
//@[002:0030) |   ├─ObjectPropertySyntax
//@[002:0006) |   | ├─IdentifierSyntax
//@[002:0006) |   | | └─Token(Identifier) |name|
//@[006:0007) |   | ├─Token(Colon) |:|
//@[008:0030) |   | └─StringSyntax
//@[008:0030) |   |   └─Token(StringComplete) |'myManagementGroupMod'|
//@[030:0031) |   ├─Token(NewLine) |\n|
  scope: managementGroup('myManagementGroup')
//@[002:0045) |   ├─ObjectPropertySyntax
//@[002:0007) |   | ├─IdentifierSyntax
//@[002:0007) |   | | └─Token(Identifier) |scope|
//@[007:0008) |   | ├─Token(Colon) |:|
//@[009:0045) |   | └─FunctionCallSyntax
//@[009:0024) |   |   ├─IdentifierSyntax
//@[009:0024) |   |   | └─Token(Identifier) |managementGroup|
//@[024:0025) |   |   ├─Token(LeftParen) |(|
//@[025:0044) |   |   ├─FunctionArgumentSyntax
//@[025:0044) |   |   | └─StringSyntax
//@[025:0044) |   |   |   └─Token(StringComplete) |'myManagementGroup'|
//@[044:0045) |   |   └─Token(RightParen) |)|
//@[045:0046) |   ├─Token(NewLine) |\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0002) ├─Token(NewLine) |\n|
module myManagementGroupModWithDuplicatedNameButDifferentScope 'modules/managementgroup_empty.bicep' = {
//@[000:0184) ├─ModuleDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |module|
//@[007:0062) | ├─IdentifierSyntax
//@[007:0062) | | └─Token(Identifier) |myManagementGroupModWithDuplicatedNameButDifferentScope|
//@[063:0100) | ├─StringSyntax
//@[063:0100) | | └─Token(StringComplete) |'modules/managementgroup_empty.bicep'|
//@[101:0102) | ├─Token(Assignment) |=|
//@[103:0184) | └─ObjectSyntax
//@[103:0104) |   ├─Token(LeftBrace) |{|
//@[104:0105) |   ├─Token(NewLine) |\n|
  name: 'myManagementGroupMod'
//@[002:0030) |   ├─ObjectPropertySyntax
//@[002:0006) |   | ├─IdentifierSyntax
//@[002:0006) |   | | └─Token(Identifier) |name|
//@[006:0007) |   | ├─Token(Colon) |:|
//@[008:0030) |   | └─StringSyntax
//@[008:0030) |   |   └─Token(StringComplete) |'myManagementGroupMod'|
//@[030:0031) |   ├─Token(NewLine) |\n|
  scope: managementGroup('myManagementGroup2')
//@[002:0046) |   ├─ObjectPropertySyntax
//@[002:0007) |   | ├─IdentifierSyntax
//@[002:0007) |   | | └─Token(Identifier) |scope|
//@[007:0008) |   | ├─Token(Colon) |:|
//@[009:0046) |   | └─FunctionCallSyntax
//@[009:0024) |   |   ├─IdentifierSyntax
//@[009:0024) |   |   | └─Token(Identifier) |managementGroup|
//@[024:0025) |   |   ├─Token(LeftParen) |(|
//@[025:0045) |   |   ├─FunctionArgumentSyntax
//@[025:0045) |   |   | └─StringSyntax
//@[025:0045) |   |   |   └─Token(StringComplete) |'myManagementGroup2'|
//@[045:0046) |   |   └─Token(RightParen) |)|
//@[046:0047) |   ├─Token(NewLine) |\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0002) ├─Token(NewLine) |\n|
module mySubscriptionMod 'modules/subscription.bicep' = {
//@[000:0149) ├─ModuleDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |module|
//@[007:0024) | ├─IdentifierSyntax
//@[007:0024) | | └─Token(Identifier) |mySubscriptionMod|
//@[025:0053) | ├─StringSyntax
//@[025:0053) | | └─Token(StringComplete) |'modules/subscription.bicep'|
//@[054:0055) | ├─Token(Assignment) |=|
//@[056:0149) | └─ObjectSyntax
//@[056:0057) |   ├─Token(LeftBrace) |{|
//@[057:0058) |   ├─Token(NewLine) |\n|
  name: 'mySubscriptionMod'
//@[002:0027) |   ├─ObjectPropertySyntax
//@[002:0006) |   | ├─IdentifierSyntax
//@[002:0006) |   | | └─Token(Identifier) |name|
//@[006:0007) |   | ├─Token(Colon) |:|
//@[008:0027) |   | └─StringSyntax
//@[008:0027) |   |   └─Token(StringComplete) |'mySubscriptionMod'|
//@[027:0028) |   ├─Token(NewLine) |\n|
  scope: subscription('ee44cd78-68c6-43d9-874e-e684ec8d1191')
//@[002:0061) |   ├─ObjectPropertySyntax
//@[002:0007) |   | ├─IdentifierSyntax
//@[002:0007) |   | | └─Token(Identifier) |scope|
//@[007:0008) |   | ├─Token(Colon) |:|
//@[009:0061) |   | └─FunctionCallSyntax
//@[009:0021) |   |   ├─IdentifierSyntax
//@[009:0021) |   |   | └─Token(Identifier) |subscription|
//@[021:0022) |   |   ├─Token(LeftParen) |(|
//@[022:0060) |   |   ├─FunctionArgumentSyntax
//@[022:0060) |   |   | └─StringSyntax
//@[022:0060) |   |   |   └─Token(StringComplete) |'ee44cd78-68c6-43d9-874e-e684ec8d1191'|
//@[060:0061) |   |   └─Token(RightParen) |)|
//@[061:0062) |   ├─Token(NewLine) |\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0003) ├─Token(NewLine) |\n\n|

module mySubscriptionModWithCondition 'modules/subscription.bicep' = if (length('foo') == 3) {
//@[000:0199) ├─ModuleDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |module|
//@[007:0037) | ├─IdentifierSyntax
//@[007:0037) | | └─Token(Identifier) |mySubscriptionModWithCondition|
//@[038:0066) | ├─StringSyntax
//@[038:0066) | | └─Token(StringComplete) |'modules/subscription.bicep'|
//@[067:0068) | ├─Token(Assignment) |=|
//@[069:0199) | └─IfConditionSyntax
//@[069:0071) |   ├─Token(Identifier) |if|
//@[072:0092) |   ├─ParenthesizedExpressionSyntax
//@[072:0073) |   | ├─Token(LeftParen) |(|
//@[073:0091) |   | ├─BinaryOperationSyntax
//@[073:0086) |   | | ├─FunctionCallSyntax
//@[073:0079) |   | | | ├─IdentifierSyntax
//@[073:0079) |   | | | | └─Token(Identifier) |length|
//@[079:0080) |   | | | ├─Token(LeftParen) |(|
//@[080:0085) |   | | | ├─FunctionArgumentSyntax
//@[080:0085) |   | | | | └─StringSyntax
//@[080:0085) |   | | | |   └─Token(StringComplete) |'foo'|
//@[085:0086) |   | | | └─Token(RightParen) |)|
//@[087:0089) |   | | ├─Token(Equals) |==|
//@[090:0091) |   | | └─IntegerLiteralSyntax
//@[090:0091) |   | |   └─Token(Integer) |3|
//@[091:0092) |   | └─Token(RightParen) |)|
//@[093:0199) |   └─ObjectSyntax
//@[093:0094) |     ├─Token(LeftBrace) |{|
//@[094:0095) |     ├─Token(NewLine) |\n|
  name: 'mySubscriptionModWithCondition'
//@[002:0040) |     ├─ObjectPropertySyntax
//@[002:0006) |     | ├─IdentifierSyntax
//@[002:0006) |     | | └─Token(Identifier) |name|
//@[006:0007) |     | ├─Token(Colon) |:|
//@[008:0040) |     | └─StringSyntax
//@[008:0040) |     |   └─Token(StringComplete) |'mySubscriptionModWithCondition'|
//@[040:0041) |     ├─Token(NewLine) |\n|
  scope: subscription('ee44cd78-68c6-43d9-874e-e684ec8d1191')
//@[002:0061) |     ├─ObjectPropertySyntax
//@[002:0007) |     | ├─IdentifierSyntax
//@[002:0007) |     | | └─Token(Identifier) |scope|
//@[007:0008) |     | ├─Token(Colon) |:|
//@[009:0061) |     | └─FunctionCallSyntax
//@[009:0021) |     |   ├─IdentifierSyntax
//@[009:0021) |     |   | └─Token(Identifier) |subscription|
//@[021:0022) |     |   ├─Token(LeftParen) |(|
//@[022:0060) |     |   ├─FunctionArgumentSyntax
//@[022:0060) |     |   | └─StringSyntax
//@[022:0060) |     |   |   └─Token(StringComplete) |'ee44cd78-68c6-43d9-874e-e684ec8d1191'|
//@[060:0061) |     |   └─Token(RightParen) |)|
//@[061:0062) |     ├─Token(NewLine) |\n|
}
//@[000:0001) |     └─Token(RightBrace) |}|
//@[001:0003) ├─Token(NewLine) |\n\n|

module mySubscriptionModWithDuplicatedNameButDifferentScope 'modules/subscription_empty.bicep' = {
//@[000:0190) ├─ModuleDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |module|
//@[007:0059) | ├─IdentifierSyntax
//@[007:0059) | | └─Token(Identifier) |mySubscriptionModWithDuplicatedNameButDifferentScope|
//@[060:0094) | ├─StringSyntax
//@[060:0094) | | └─Token(StringComplete) |'modules/subscription_empty.bicep'|
//@[095:0096) | ├─Token(Assignment) |=|
//@[097:0190) | └─ObjectSyntax
//@[097:0098) |   ├─Token(LeftBrace) |{|
//@[098:0099) |   ├─Token(NewLine) |\n|
  name: 'mySubscriptionMod'
//@[002:0027) |   ├─ObjectPropertySyntax
//@[002:0006) |   | ├─IdentifierSyntax
//@[002:0006) |   | | └─Token(Identifier) |name|
//@[006:0007) |   | ├─Token(Colon) |:|
//@[008:0027) |   | └─StringSyntax
//@[008:0027) |   |   └─Token(StringComplete) |'mySubscriptionMod'|
//@[027:0028) |   ├─Token(NewLine) |\n|
  scope: subscription('1ad827ac-2669-4c2f-9970-282b93c3c550')
//@[002:0061) |   ├─ObjectPropertySyntax
//@[002:0007) |   | ├─IdentifierSyntax
//@[002:0007) |   | | └─Token(Identifier) |scope|
//@[007:0008) |   | ├─Token(Colon) |:|
//@[009:0061) |   | └─FunctionCallSyntax
//@[009:0021) |   |   ├─IdentifierSyntax
//@[009:0021) |   |   | └─Token(Identifier) |subscription|
//@[021:0022) |   |   ├─Token(LeftParen) |(|
//@[022:0060) |   |   ├─FunctionArgumentSyntax
//@[022:0060) |   |   | └─StringSyntax
//@[022:0060) |   |   |   └─Token(StringComplete) |'1ad827ac-2669-4c2f-9970-282b93c3c550'|
//@[060:0061) |   |   └─Token(RightParen) |)|
//@[061:0062) |   ├─Token(NewLine) |\n|
}
//@[000:0001) |   └─Token(RightBrace) |}|
//@[001:0004) ├─Token(NewLine) |\n\n\n|


output myManagementGroupOutput string = myManagementGroupMod.outputs.myOutput
//@[000:0077) ├─OutputDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |output|
//@[007:0030) | ├─IdentifierSyntax
//@[007:0030) | | └─Token(Identifier) |myManagementGroupOutput|
//@[031:0037) | ├─SimpleTypeSyntax
//@[031:0037) | | └─Token(Identifier) |string|
//@[038:0039) | ├─Token(Assignment) |=|
//@[040:0077) | └─PropertyAccessSyntax
//@[040:0068) |   ├─PropertyAccessSyntax
//@[040:0060) |   | ├─VariableAccessSyntax
//@[040:0060) |   | | └─IdentifierSyntax
//@[040:0060) |   | |   └─Token(Identifier) |myManagementGroupMod|
//@[060:0061) |   | ├─Token(Dot) |.|
//@[061:0068) |   | └─IdentifierSyntax
//@[061:0068) |   |   └─Token(Identifier) |outputs|
//@[068:0069) |   ├─Token(Dot) |.|
//@[069:0077) |   └─IdentifierSyntax
//@[069:0077) |     └─Token(Identifier) |myOutput|
//@[077:0078) ├─Token(NewLine) |\n|
output mySubscriptionOutput string = mySubscriptionMod.outputs.myOutput
//@[000:0071) ├─OutputDeclarationSyntax
//@[000:0006) | ├─Token(Identifier) |output|
//@[007:0027) | ├─IdentifierSyntax
//@[007:0027) | | └─Token(Identifier) |mySubscriptionOutput|
//@[028:0034) | ├─SimpleTypeSyntax
//@[028:0034) | | └─Token(Identifier) |string|
//@[035:0036) | ├─Token(Assignment) |=|
//@[037:0071) | └─PropertyAccessSyntax
//@[037:0062) |   ├─PropertyAccessSyntax
//@[037:0054) |   | ├─VariableAccessSyntax
//@[037:0054) |   | | └─IdentifierSyntax
//@[037:0054) |   | |   └─Token(Identifier) |mySubscriptionMod|
//@[054:0055) |   | ├─Token(Dot) |.|
//@[055:0062) |   | └─IdentifierSyntax
//@[055:0062) |   |   └─Token(Identifier) |outputs|
//@[062:0063) |   ├─Token(Dot) |.|
//@[063:0071) |   └─IdentifierSyntax
//@[063:0071) |     └─Token(Identifier) |myOutput|
//@[071:0072) ├─Token(NewLine) |\n|

//@[000:0000) └─Token(EndOfFile) ||
