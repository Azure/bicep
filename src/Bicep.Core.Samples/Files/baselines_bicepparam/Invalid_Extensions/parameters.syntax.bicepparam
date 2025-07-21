using 'main.bicep'
//@[000:1417) ProgramSyntax
//@[000:0018) ├─UsingDeclarationSyntax
//@[000:0005) | ├─Token(Identifier) |using|
//@[006:0018) | └─StringSyntax
//@[006:0018) |   └─Token(StringComplete) |'main.bicep'|
//@[018:0020) ├─Token(NewLine) |\n\n|

var emptyObjVar = {}
//@[000:0020) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0015) | ├─IdentifierSyntax
//@[004:0015) | | └─Token(Identifier) |emptyObjVar|
//@[016:0017) | ├─Token(Assignment) |=|
//@[018:0020) | └─ObjectSyntax
//@[018:0019) |   ├─Token(LeftBrace) |{|
//@[019:0020) |   └─Token(RightBrace) |}|
//@[020:0021) ├─Token(NewLine) |\n|
param strParam1 = 'strParam1Value'
//@[000:0034) ├─ParameterAssignmentSyntax
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0015) | ├─IdentifierSyntax
//@[006:0015) | | └─Token(Identifier) |strParam1|
//@[016:0017) | ├─Token(Assignment) |=|
//@[018:0034) | └─StringSyntax
//@[018:0034) |   └─Token(StringComplete) |'strParam1Value'|
//@[034:0035) ├─Token(NewLine) |\n|
var strVar1 = 'strVar1Value'
//@[000:0028) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0011) | ├─IdentifierSyntax
//@[004:0011) | | └─Token(Identifier) |strVar1|
//@[012:0013) | ├─Token(Assignment) |=|
//@[014:0028) | └─StringSyntax
//@[014:0028) |   └─Token(StringComplete) |'strVar1Value'|
//@[028:0029) ├─Token(NewLine) |\n|
param secureStrParam1 = az.getSecret('a', 'b', 'c', 'param')
//@[000:0060) ├─ParameterAssignmentSyntax
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0021) | ├─IdentifierSyntax
//@[006:0021) | | └─Token(Identifier) |secureStrParam1|
//@[022:0023) | ├─Token(Assignment) |=|
//@[024:0060) | └─InstanceFunctionCallSyntax
//@[024:0026) |   ├─VariableAccessSyntax
//@[024:0026) |   | └─IdentifierSyntax
//@[024:0026) |   |   └─Token(Identifier) |az|
//@[026:0027) |   ├─Token(Dot) |.|
//@[027:0036) |   ├─IdentifierSyntax
//@[027:0036) |   | └─Token(Identifier) |getSecret|
//@[036:0037) |   ├─Token(LeftParen) |(|
//@[037:0040) |   ├─FunctionArgumentSyntax
//@[037:0040) |   | └─StringSyntax
//@[037:0040) |   |   └─Token(StringComplete) |'a'|
//@[040:0041) |   ├─Token(Comma) |,|
//@[042:0045) |   ├─FunctionArgumentSyntax
//@[042:0045) |   | └─StringSyntax
//@[042:0045) |   |   └─Token(StringComplete) |'b'|
//@[045:0046) |   ├─Token(Comma) |,|
//@[047:0050) |   ├─FunctionArgumentSyntax
//@[047:0050) |   | └─StringSyntax
//@[047:0050) |   |   └─Token(StringComplete) |'c'|
//@[050:0051) |   ├─Token(Comma) |,|
//@[052:0059) |   ├─FunctionArgumentSyntax
//@[052:0059) |   | └─StringSyntax
//@[052:0059) |   |   └─Token(StringComplete) |'param'|
//@[059:0060) |   └─Token(RightParen) |)|
//@[060:0061) ├─Token(NewLine) |\n|
var secureStrVar1 = az.getSecret('a', 'b', 'c', 'var')
//@[000:0054) ├─VariableDeclarationSyntax
//@[000:0003) | ├─Token(Identifier) |var|
//@[004:0017) | ├─IdentifierSyntax
//@[004:0017) | | └─Token(Identifier) |secureStrVar1|
//@[018:0019) | ├─Token(Assignment) |=|
//@[020:0054) | └─InstanceFunctionCallSyntax
//@[020:0022) |   ├─VariableAccessSyntax
//@[020:0022) |   | └─IdentifierSyntax
//@[020:0022) |   |   └─Token(Identifier) |az|
//@[022:0023) |   ├─Token(Dot) |.|
//@[023:0032) |   ├─IdentifierSyntax
//@[023:0032) |   | └─Token(Identifier) |getSecret|
//@[032:0033) |   ├─Token(LeftParen) |(|
//@[033:0036) |   ├─FunctionArgumentSyntax
//@[033:0036) |   | └─StringSyntax
//@[033:0036) |   |   └─Token(StringComplete) |'a'|
//@[036:0037) |   ├─Token(Comma) |,|
//@[038:0041) |   ├─FunctionArgumentSyntax
//@[038:0041) |   | └─StringSyntax
//@[038:0041) |   |   └─Token(StringComplete) |'b'|
//@[041:0042) |   ├─Token(Comma) |,|
//@[043:0046) |   ├─FunctionArgumentSyntax
//@[043:0046) |   | └─StringSyntax
//@[043:0046) |   |   └─Token(StringComplete) |'c'|
//@[046:0047) |   ├─Token(Comma) |,|
//@[048:0053) |   ├─FunctionArgumentSyntax
//@[048:0053) |   | └─StringSyntax
//@[048:0053) |   |   └─Token(StringComplete) |'var'|
//@[053:0054) |   └─Token(RightParen) |)|
//@[054:0056) ├─Token(NewLine) |\n\n|

extensionConfig validAssignment1 with {
//@[000:0067) ├─ExtensionConfigAssignmentSyntax
//@[000:0015) | ├─Token(Identifier) |extensionConfig|
//@[016:0032) | ├─IdentifierSyntax
//@[016:0032) | | └─Token(Identifier) |validAssignment1|
//@[033:0067) | └─ExtensionWithClauseSyntax
//@[033:0037) |   ├─Token(Identifier) |with|
//@[038:0067) |   └─ObjectSyntax
//@[038:0039) |     ├─Token(LeftBrace) |{|
//@[039:0040) |     ├─Token(NewLine) |\n|
  requiredString: 'value'
//@[002:0025) |     ├─ObjectPropertySyntax
//@[002:0016) |     | ├─IdentifierSyntax
//@[002:0016) |     | | └─Token(Identifier) |requiredString|
//@[016:0017) |     | ├─Token(Colon) |:|
//@[018:0025) |     | └─StringSyntax
//@[018:0025) |     |   └─Token(StringComplete) |'value'|
//@[025:0026) |     ├─Token(NewLine) |\n|
}
//@[000:0001) |     └─Token(RightBrace) |}|
//@[001:0003) ├─Token(NewLine) |\n\n|

extensionConfig
//@[000:0015) ├─ExtensionConfigAssignmentSyntax
//@[000:0015) | ├─Token(Identifier) |extensionConfig|
//@[015:0015) | ├─IdentifierSyntax
//@[015:0015) | | └─SkippedTriviaSyntax
//@[015:0015) | └─SkippedTriviaSyntax
//@[015:0017) ├─Token(NewLine) |\n\n|

extensionConfig incompleteAssignment1
//@[000:0037) ├─ExtensionConfigAssignmentSyntax
//@[000:0015) | ├─Token(Identifier) |extensionConfig|
//@[016:0037) | ├─IdentifierSyntax
//@[016:0037) | | └─Token(Identifier) |incompleteAssignment1|
//@[037:0037) | └─SkippedTriviaSyntax
//@[037:0038) ├─Token(NewLine) |\n|
extensionConfig incompleteAssignment2 with
//@[000:0042) ├─ExtensionConfigAssignmentSyntax
//@[000:0015) | ├─Token(Identifier) |extensionConfig|
//@[016:0037) | ├─IdentifierSyntax
//@[016:0037) | | └─Token(Identifier) |incompleteAssignment2|
//@[038:0042) | └─ExtensionWithClauseSyntax
//@[038:0042) |   ├─Token(Identifier) |with|
//@[042:0042) |   └─SkippedTriviaSyntax
//@[042:0044) ├─Token(NewLine) |\n\n|

extensionConfig hasNoConfig with {}
//@[000:0035) ├─ExtensionConfigAssignmentSyntax
//@[000:0015) | ├─Token(Identifier) |extensionConfig|
//@[016:0027) | ├─IdentifierSyntax
//@[016:0027) | | └─Token(Identifier) |hasNoConfig|
//@[028:0035) | └─ExtensionWithClauseSyntax
//@[028:0032) |   ├─Token(Identifier) |with|
//@[033:0035) |   └─ObjectSyntax
//@[033:0034) |     ├─Token(LeftBrace) |{|
//@[034:0035) |     └─Token(RightBrace) |}|
//@[035:0037) ├─Token(NewLine) |\n\n|

extensionConfig invalidSyntax1 = emptyObjVar
//@[000:0044) ├─ExtensionConfigAssignmentSyntax
//@[000:0015) | ├─Token(Identifier) |extensionConfig|
//@[016:0030) | ├─IdentifierSyntax
//@[016:0030) | | └─Token(Identifier) |invalidSyntax1|
//@[031:0044) | └─SkippedTriviaSyntax
//@[031:0032) |   ├─Token(Assignment) |=|
//@[033:0044) |   └─Token(Identifier) |emptyObjVar|
//@[044:0045) ├─Token(NewLine) |\n|
extensionConfig invalidSyntax2 with emptyObjVar
//@[000:0047) ├─ExtensionConfigAssignmentSyntax
//@[000:0015) | ├─Token(Identifier) |extensionConfig|
//@[016:0030) | ├─IdentifierSyntax
//@[016:0030) | | └─Token(Identifier) |invalidSyntax2|
//@[031:0047) | └─ExtensionWithClauseSyntax
//@[031:0035) |   ├─Token(Identifier) |with|
//@[036:0047) |   └─SkippedTriviaSyntax
//@[036:0047) |     └─Token(Identifier) |emptyObjVar|
//@[047:0048) ├─Token(NewLine) |\n|
extensionConfig invalidSyntax3 with {
//@[000:0056) ├─ExtensionConfigAssignmentSyntax
//@[000:0015) | ├─Token(Identifier) |extensionConfig|
//@[016:0030) | ├─IdentifierSyntax
//@[016:0030) | | └─Token(Identifier) |invalidSyntax3|
//@[031:0056) | └─ExtensionWithClauseSyntax
//@[031:0035) |   ├─Token(Identifier) |with|
//@[036:0056) |   └─ObjectSyntax
//@[036:0037) |     ├─Token(LeftBrace) |{|
//@[037:0038) |     ├─Token(NewLine) |\n|
  ...emptyObjVar
//@[002:0016) |     ├─SpreadExpressionSyntax
//@[002:0005) |     | ├─Token(Ellipsis) |...|
//@[005:0016) |     | └─VariableAccessSyntax
//@[005:0016) |     |   └─IdentifierSyntax
//@[005:0016) |     |     └─Token(Identifier) |emptyObjVar|
//@[016:0017) |     ├─Token(NewLine) |\n|
}
//@[000:0001) |     └─Token(RightBrace) |}|
//@[001:0003) ├─Token(NewLine) |\n\n|

extensionConfig invalidSyntax4 with {
//@[000:0089) ├─ExtensionConfigAssignmentSyntax
//@[000:0015) | ├─Token(Identifier) |extensionConfig|
//@[016:0030) | ├─IdentifierSyntax
//@[016:0030) | | └─Token(Identifier) |invalidSyntax4|
//@[031:0089) | └─ExtensionWithClauseSyntax
//@[031:0035) |   ├─Token(Identifier) |with|
//@[036:0089) |   └─ObjectSyntax
//@[036:0037) |     ├─Token(LeftBrace) |{|
//@[037:0038) |     ├─Token(NewLine) |\n|
  requiredString: validAssignment1.requiredString
//@[002:0049) |     ├─ObjectPropertySyntax
//@[002:0016) |     | ├─IdentifierSyntax
//@[002:0016) |     | | └─Token(Identifier) |requiredString|
//@[016:0017) |     | ├─Token(Colon) |:|
//@[018:0049) |     | └─PropertyAccessSyntax
//@[018:0034) |     |   ├─VariableAccessSyntax
//@[018:0034) |     |   | └─IdentifierSyntax
//@[018:0034) |     |   |   └─Token(Identifier) |validAssignment1|
//@[034:0035) |     |   ├─Token(Dot) |.|
//@[035:0049) |     |   └─IdentifierSyntax
//@[035:0049) |     |     └─Token(Identifier) |requiredString|
//@[049:0050) |     ├─Token(NewLine) |\n|
}
//@[000:0001) |     └─Token(RightBrace) |}|
//@[001:0003) ├─Token(NewLine) |\n\n|

extensionConfig invalidSyntax5 with {
//@[000:0061) ├─ExtensionConfigAssignmentSyntax
//@[000:0015) | ├─Token(Identifier) |extensionConfig|
//@[016:0030) | ├─IdentifierSyntax
//@[016:0030) | | └─Token(Identifier) |invalidSyntax5|
//@[031:0061) | └─ExtensionWithClauseSyntax
//@[031:0035) |   ├─Token(Identifier) |with|
//@[036:0061) |   └─ObjectSyntax
//@[036:0037) |     ├─Token(LeftBrace) |{|
//@[037:0038) |     ├─Token(NewLine) |\n|
  ...validAssignment1
//@[002:0021) |     ├─SpreadExpressionSyntax
//@[002:0005) |     | ├─Token(Ellipsis) |...|
//@[005:0021) |     | └─VariableAccessSyntax
//@[005:0021) |     |   └─IdentifierSyntax
//@[005:0021) |     |     └─Token(Identifier) |validAssignment1|
//@[021:0022) |     ├─Token(NewLine) |\n|
}
//@[000:0001) |     └─Token(RightBrace) |}|
//@[001:0003) ├─Token(NewLine) |\n\n|

extensionConfig invalidAssignment1 with {
//@[000:0071) ├─ExtensionConfigAssignmentSyntax
//@[000:0015) | ├─Token(Identifier) |extensionConfig|
//@[016:0034) | ├─IdentifierSyntax
//@[016:0034) | | └─Token(Identifier) |invalidAssignment1|
//@[035:0071) | └─ExtensionWithClauseSyntax
//@[035:0039) |   ├─Token(Identifier) |with|
//@[040:0071) |   └─ObjectSyntax
//@[040:0041) |     ├─Token(LeftBrace) |{|
//@[041:0042) |     ├─Token(NewLine) |\n|
  requiredString: strParam1
//@[002:0027) |     ├─ObjectPropertySyntax
//@[002:0016) |     | ├─IdentifierSyntax
//@[002:0016) |     | | └─Token(Identifier) |requiredString|
//@[016:0017) |     | ├─Token(Colon) |:|
//@[018:0027) |     | └─VariableAccessSyntax
//@[018:0027) |     |   └─IdentifierSyntax
//@[018:0027) |     |     └─Token(Identifier) |strParam1|
//@[027:0028) |     ├─Token(NewLine) |\n|
}
//@[000:0001) |     └─Token(RightBrace) |}|
//@[001:0003) ├─Token(NewLine) |\n\n|

extensionConfig invalidAssignment2 with {
//@[000:0069) ├─ExtensionConfigAssignmentSyntax
//@[000:0015) | ├─Token(Identifier) |extensionConfig|
//@[016:0034) | ├─IdentifierSyntax
//@[016:0034) | | └─Token(Identifier) |invalidAssignment2|
//@[035:0069) | └─ExtensionWithClauseSyntax
//@[035:0039) |   ├─Token(Identifier) |with|
//@[040:0069) |   └─ObjectSyntax
//@[040:0041) |     ├─Token(LeftBrace) |{|
//@[041:0042) |     ├─Token(NewLine) |\n|
  requiredString: strVar1
//@[002:0025) |     ├─ObjectPropertySyntax
//@[002:0016) |     | ├─IdentifierSyntax
//@[002:0016) |     | | └─Token(Identifier) |requiredString|
//@[016:0017) |     | ├─Token(Colon) |:|
//@[018:0025) |     | └─VariableAccessSyntax
//@[018:0025) |     |   └─IdentifierSyntax
//@[018:0025) |     |     └─Token(Identifier) |strVar1|
//@[025:0026) |     ├─Token(NewLine) |\n|
}
//@[000:0001) |     └─Token(RightBrace) |}|
//@[001:0003) ├─Token(NewLine) |\n\n|

extensionConfig invalidSecretAssignment1 with {
//@[000:0189) ├─ExtensionConfigAssignmentSyntax
//@[000:0015) | ├─Token(Identifier) |extensionConfig|
//@[016:0040) | ├─IdentifierSyntax
//@[016:0040) | | └─Token(Identifier) |invalidSecretAssignment1|
//@[041:0189) | └─ExtensionWithClauseSyntax
//@[041:0045) |   ├─Token(Identifier) |with|
//@[046:0189) |   └─ObjectSyntax
//@[046:0047) |     ├─Token(LeftBrace) |{|
//@[047:0048) |     ├─Token(NewLine) |\n|
  requiredSecureString: bool(readEnvironmentVariable('xyz', 'false')) ? az.getSecret('a', 'b', 'c', 'd') : az.getSecret('w', 'x', 'y', 'z')
//@[002:0139) |     ├─ObjectPropertySyntax
//@[002:0022) |     | ├─IdentifierSyntax
//@[002:0022) |     | | └─Token(Identifier) |requiredSecureString|
//@[022:0023) |     | ├─Token(Colon) |:|
//@[024:0139) |     | └─TernaryOperationSyntax
//@[024:0069) |     |   ├─FunctionCallSyntax
//@[024:0028) |     |   | ├─IdentifierSyntax
//@[024:0028) |     |   | | └─Token(Identifier) |bool|
//@[028:0029) |     |   | ├─Token(LeftParen) |(|
//@[029:0068) |     |   | ├─FunctionArgumentSyntax
//@[029:0068) |     |   | | └─FunctionCallSyntax
//@[029:0052) |     |   | |   ├─IdentifierSyntax
//@[029:0052) |     |   | |   | └─Token(Identifier) |readEnvironmentVariable|
//@[052:0053) |     |   | |   ├─Token(LeftParen) |(|
//@[053:0058) |     |   | |   ├─FunctionArgumentSyntax
//@[053:0058) |     |   | |   | └─StringSyntax
//@[053:0058) |     |   | |   |   └─Token(StringComplete) |'xyz'|
//@[058:0059) |     |   | |   ├─Token(Comma) |,|
//@[060:0067) |     |   | |   ├─FunctionArgumentSyntax
//@[060:0067) |     |   | |   | └─StringSyntax
//@[060:0067) |     |   | |   |   └─Token(StringComplete) |'false'|
//@[067:0068) |     |   | |   └─Token(RightParen) |)|
//@[068:0069) |     |   | └─Token(RightParen) |)|
//@[070:0071) |     |   ├─Token(Question) |?|
//@[072:0104) |     |   ├─InstanceFunctionCallSyntax
//@[072:0074) |     |   | ├─VariableAccessSyntax
//@[072:0074) |     |   | | └─IdentifierSyntax
//@[072:0074) |     |   | |   └─Token(Identifier) |az|
//@[074:0075) |     |   | ├─Token(Dot) |.|
//@[075:0084) |     |   | ├─IdentifierSyntax
//@[075:0084) |     |   | | └─Token(Identifier) |getSecret|
//@[084:0085) |     |   | ├─Token(LeftParen) |(|
//@[085:0088) |     |   | ├─FunctionArgumentSyntax
//@[085:0088) |     |   | | └─StringSyntax
//@[085:0088) |     |   | |   └─Token(StringComplete) |'a'|
//@[088:0089) |     |   | ├─Token(Comma) |,|
//@[090:0093) |     |   | ├─FunctionArgumentSyntax
//@[090:0093) |     |   | | └─StringSyntax
//@[090:0093) |     |   | |   └─Token(StringComplete) |'b'|
//@[093:0094) |     |   | ├─Token(Comma) |,|
//@[095:0098) |     |   | ├─FunctionArgumentSyntax
//@[095:0098) |     |   | | └─StringSyntax
//@[095:0098) |     |   | |   └─Token(StringComplete) |'c'|
//@[098:0099) |     |   | ├─Token(Comma) |,|
//@[100:0103) |     |   | ├─FunctionArgumentSyntax
//@[100:0103) |     |   | | └─StringSyntax
//@[100:0103) |     |   | |   └─Token(StringComplete) |'d'|
//@[103:0104) |     |   | └─Token(RightParen) |)|
//@[105:0106) |     |   ├─Token(Colon) |:|
//@[107:0139) |     |   └─InstanceFunctionCallSyntax
//@[107:0109) |     |     ├─VariableAccessSyntax
//@[107:0109) |     |     | └─IdentifierSyntax
//@[107:0109) |     |     |   └─Token(Identifier) |az|
//@[109:0110) |     |     ├─Token(Dot) |.|
//@[110:0119) |     |     ├─IdentifierSyntax
//@[110:0119) |     |     | └─Token(Identifier) |getSecret|
//@[119:0120) |     |     ├─Token(LeftParen) |(|
//@[120:0123) |     |     ├─FunctionArgumentSyntax
//@[120:0123) |     |     | └─StringSyntax
//@[120:0123) |     |     |   └─Token(StringComplete) |'w'|
//@[123:0124) |     |     ├─Token(Comma) |,|
//@[125:0128) |     |     ├─FunctionArgumentSyntax
//@[125:0128) |     |     | └─StringSyntax
//@[125:0128) |     |     |   └─Token(StringComplete) |'x'|
//@[128:0129) |     |     ├─Token(Comma) |,|
//@[130:0133) |     |     ├─FunctionArgumentSyntax
//@[130:0133) |     |     | └─StringSyntax
//@[130:0133) |     |     |   └─Token(StringComplete) |'y'|
//@[133:0134) |     |     ├─Token(Comma) |,|
//@[135:0138) |     |     ├─FunctionArgumentSyntax
//@[135:0138) |     |     | └─StringSyntax
//@[135:0138) |     |     |   └─Token(StringComplete) |'z'|
//@[138:0139) |     |     └─Token(RightParen) |)|
//@[139:0140) |     ├─Token(NewLine) |\n|
}
//@[000:0001) |     └─Token(RightBrace) |}|
//@[001:0003) ├─Token(NewLine) |\n\n|

extensionConfig invalidSecretAssignment2 with {
//@[000:0123) ├─ExtensionConfigAssignmentSyntax
//@[000:0015) | ├─Token(Identifier) |extensionConfig|
//@[016:0040) | ├─IdentifierSyntax
//@[016:0040) | | └─Token(Identifier) |invalidSecretAssignment2|
//@[041:0123) | └─ExtensionWithClauseSyntax
//@[041:0045) |   ├─Token(Identifier) |with|
//@[046:0123) |   └─ObjectSyntax
//@[046:0047) |     ├─Token(LeftBrace) |{|
//@[047:0048) |     ├─Token(NewLine) |\n|
  requiredSecureString: secureStrParam1
//@[002:0039) |     ├─ObjectPropertySyntax
//@[002:0022) |     | ├─IdentifierSyntax
//@[002:0022) |     | | └─Token(Identifier) |requiredSecureString|
//@[022:0023) |     | ├─Token(Colon) |:|
//@[024:0039) |     | └─VariableAccessSyntax
//@[024:0039) |     |   └─IdentifierSyntax
//@[024:0039) |     |     └─Token(Identifier) |secureStrParam1|
//@[039:0040) |     ├─Token(NewLine) |\n|
  optionalString: secureStrParam1
//@[002:0033) |     ├─ObjectPropertySyntax
//@[002:0016) |     | ├─IdentifierSyntax
//@[002:0016) |     | | └─Token(Identifier) |optionalString|
//@[016:0017) |     | ├─Token(Colon) |:|
//@[018:0033) |     | └─VariableAccessSyntax
//@[018:0033) |     |   └─IdentifierSyntax
//@[018:0033) |     |     └─Token(Identifier) |secureStrParam1|
//@[033:0034) |     ├─Token(NewLine) |\n|
}
//@[000:0001) |     └─Token(RightBrace) |}|
//@[001:0003) ├─Token(NewLine) |\n\n|

extensionConfig invalidSecretAssignment3 with {
//@[000:0119) ├─ExtensionConfigAssignmentSyntax
//@[000:0015) | ├─Token(Identifier) |extensionConfig|
//@[016:0040) | ├─IdentifierSyntax
//@[016:0040) | | └─Token(Identifier) |invalidSecretAssignment3|
//@[041:0119) | └─ExtensionWithClauseSyntax
//@[041:0045) |   ├─Token(Identifier) |with|
//@[046:0119) |   └─ObjectSyntax
//@[046:0047) |     ├─Token(LeftBrace) |{|
//@[047:0048) |     ├─Token(NewLine) |\n|
  requiredSecureString: secureStrVar1
//@[002:0037) |     ├─ObjectPropertySyntax
//@[002:0022) |     | ├─IdentifierSyntax
//@[002:0022) |     | | └─Token(Identifier) |requiredSecureString|
//@[022:0023) |     | ├─Token(Colon) |:|
//@[024:0037) |     | └─VariableAccessSyntax
//@[024:0037) |     |   └─IdentifierSyntax
//@[024:0037) |     |     └─Token(Identifier) |secureStrVar1|
//@[037:0038) |     ├─Token(NewLine) |\n|
  optionalString: secureStrVar1
//@[002:0031) |     ├─ObjectPropertySyntax
//@[002:0016) |     | ├─IdentifierSyntax
//@[002:0016) |     | | └─Token(Identifier) |optionalString|
//@[016:0017) |     | ├─Token(Colon) |:|
//@[018:0031) |     | └─VariableAccessSyntax
//@[018:0031) |     |   └─IdentifierSyntax
//@[018:0031) |     |     └─Token(Identifier) |secureStrVar1|
//@[031:0032) |     ├─Token(NewLine) |\n|
}
//@[000:0001) |     └─Token(RightBrace) |}|
//@[001:0003) ├─Token(NewLine) |\n\n|

extensionConfig invalidDiscrimAssignment1 with {
//@[000:0103) ├─ExtensionConfigAssignmentSyntax
//@[000:0015) | ├─Token(Identifier) |extensionConfig|
//@[016:0041) | ├─IdentifierSyntax
//@[016:0041) | | └─Token(Identifier) |invalidDiscrimAssignment1|
//@[042:0103) | └─ExtensionWithClauseSyntax
//@[042:0046) |   ├─Token(Identifier) |with|
//@[047:0103) |   └─ObjectSyntax
//@[047:0048) |     ├─Token(LeftBrace) |{|
//@[048:0049) |     ├─Token(NewLine) |\n|
  discrim: 'a' // this property cannot be reassigned
//@[002:0014) |     ├─ObjectPropertySyntax
//@[002:0009) |     | ├─IdentifierSyntax
//@[002:0009) |     | | └─Token(Identifier) |discrim|
//@[009:0010) |     | ├─Token(Colon) |:|
//@[011:0014) |     | └─StringSyntax
//@[011:0014) |     |   └─Token(StringComplete) |'a'|
//@[052:0053) |     ├─Token(NewLine) |\n|
}
//@[000:0001) |     └─Token(RightBrace) |}|
//@[001:0002) ├─Token(NewLine) |\n|

//@[000:0000) └─Token(EndOfFile) ||
