using 'main.bicep'
//@[000:851) ProgramSyntax
//@[000:018) ├─UsingDeclarationSyntax
//@[000:005) | ├─Token(Identifier) |using|
//@[006:018) | └─StringSyntax
//@[006:018) |   └─Token(StringComplete) |'main.bicep'|
//@[018:020) ├─Token(NewLine) |\n\n|

var emptyObjVar = {}
//@[000:020) ├─VariableDeclarationSyntax
//@[000:003) | ├─Token(Identifier) |var|
//@[004:015) | ├─IdentifierSyntax
//@[004:015) | | └─Token(Identifier) |emptyObjVar|
//@[016:017) | ├─Token(Assignment) |=|
//@[018:020) | └─ObjectSyntax
//@[018:019) |   ├─Token(LeftBrace) |{|
//@[019:020) |   └─Token(RightBrace) |}|
//@[020:022) ├─Token(NewLine) |\n\n|

param strParam1 = 'strParam1Value'
//@[000:034) ├─ParameterAssignmentSyntax
//@[000:005) | ├─Token(Identifier) |param|
//@[006:015) | ├─IdentifierSyntax
//@[006:015) | | └─Token(Identifier) |strParam1|
//@[016:017) | ├─Token(Assignment) |=|
//@[018:034) | └─StringSyntax
//@[018:034) |   └─Token(StringComplete) |'strParam1Value'|
//@[034:036) ├─Token(NewLine) |\n\n|

extensionConfig validAssignment1 with {
//@[000:067) ├─ExtensionConfigAssignmentSyntax
//@[000:015) | ├─Token(Identifier) |extensionConfig|
//@[016:032) | ├─IdentifierSyntax
//@[016:032) | | └─Token(Identifier) |validAssignment1|
//@[033:067) | └─ExtensionWithClauseSyntax
//@[033:037) |   ├─Token(Identifier) |with|
//@[038:067) |   └─ObjectSyntax
//@[038:039) |     ├─Token(LeftBrace) |{|
//@[039:040) |     ├─Token(NewLine) |\n|
  requiredString: 'value'
//@[002:025) |     ├─ObjectPropertySyntax
//@[002:016) |     | ├─IdentifierSyntax
//@[002:016) |     | | └─Token(Identifier) |requiredString|
//@[016:017) |     | ├─Token(Colon) |:|
//@[018:025) |     | └─StringSyntax
//@[018:025) |     |   └─Token(StringComplete) |'value'|
//@[025:026) |     ├─Token(NewLine) |\n|
}
//@[000:001) |     └─Token(RightBrace) |}|
//@[001:003) ├─Token(NewLine) |\n\n|

extensionConfig
//@[000:015) ├─ExtensionConfigAssignmentSyntax
//@[000:015) | ├─Token(Identifier) |extensionConfig|
//@[015:015) | ├─IdentifierSyntax
//@[015:015) | | └─SkippedTriviaSyntax
//@[015:015) | └─SkippedTriviaSyntax
//@[015:017) ├─Token(NewLine) |\n\n|

extensionConfig incompleteAssignment1
//@[000:037) ├─ExtensionConfigAssignmentSyntax
//@[000:015) | ├─Token(Identifier) |extensionConfig|
//@[016:037) | ├─IdentifierSyntax
//@[016:037) | | └─Token(Identifier) |incompleteAssignment1|
//@[037:037) | └─SkippedTriviaSyntax
//@[037:038) ├─Token(NewLine) |\n|
extensionConfig incompleteAssignment2 with
//@[000:042) ├─ExtensionConfigAssignmentSyntax
//@[000:015) | ├─Token(Identifier) |extensionConfig|
//@[016:037) | ├─IdentifierSyntax
//@[016:037) | | └─Token(Identifier) |incompleteAssignment2|
//@[038:042) | └─ExtensionWithClauseSyntax
//@[038:042) |   ├─Token(Identifier) |with|
//@[042:042) |   └─SkippedTriviaSyntax
//@[042:044) ├─Token(NewLine) |\n\n|

extensionConfig hasNoConfig with {}
//@[000:035) ├─ExtensionConfigAssignmentSyntax
//@[000:015) | ├─Token(Identifier) |extensionConfig|
//@[016:027) | ├─IdentifierSyntax
//@[016:027) | | └─Token(Identifier) |hasNoConfig|
//@[028:035) | └─ExtensionWithClauseSyntax
//@[028:032) |   ├─Token(Identifier) |with|
//@[033:035) |   └─ObjectSyntax
//@[033:034) |     ├─Token(LeftBrace) |{|
//@[034:035) |     └─Token(RightBrace) |}|
//@[035:037) ├─Token(NewLine) |\n\n|

extensionConfig invalidSyntax1 = emptyObjVar
//@[000:044) ├─ExtensionConfigAssignmentSyntax
//@[000:015) | ├─Token(Identifier) |extensionConfig|
//@[016:030) | ├─IdentifierSyntax
//@[016:030) | | └─Token(Identifier) |invalidSyntax1|
//@[031:044) | └─SkippedTriviaSyntax
//@[031:032) |   ├─Token(Assignment) |=|
//@[033:044) |   └─Token(Identifier) |emptyObjVar|
//@[044:045) ├─Token(NewLine) |\n|
extensionConfig invalidSyntax2 with emptyObjVar
//@[000:047) ├─ExtensionConfigAssignmentSyntax
//@[000:015) | ├─Token(Identifier) |extensionConfig|
//@[016:030) | ├─IdentifierSyntax
//@[016:030) | | └─Token(Identifier) |invalidSyntax2|
//@[031:047) | └─ExtensionWithClauseSyntax
//@[031:035) |   ├─Token(Identifier) |with|
//@[036:047) |   └─SkippedTriviaSyntax
//@[036:047) |     └─Token(Identifier) |emptyObjVar|
//@[047:048) ├─Token(NewLine) |\n|
extensionConfig invalidSyntax3 with {
//@[000:056) ├─ExtensionConfigAssignmentSyntax
//@[000:015) | ├─Token(Identifier) |extensionConfig|
//@[016:030) | ├─IdentifierSyntax
//@[016:030) | | └─Token(Identifier) |invalidSyntax3|
//@[031:056) | └─ExtensionWithClauseSyntax
//@[031:035) |   ├─Token(Identifier) |with|
//@[036:056) |   └─ObjectSyntax
//@[036:037) |     ├─Token(LeftBrace) |{|
//@[037:038) |     ├─Token(NewLine) |\n|
  ...emptyObjVar
//@[002:016) |     ├─SpreadExpressionSyntax
//@[002:005) |     | ├─Token(Ellipsis) |...|
//@[005:016) |     | └─VariableAccessSyntax
//@[005:016) |     |   └─IdentifierSyntax
//@[005:016) |     |     └─Token(Identifier) |emptyObjVar|
//@[016:017) |     ├─Token(NewLine) |\n|
}
//@[000:001) |     └─Token(RightBrace) |}|
//@[001:003) ├─Token(NewLine) |\n\n|

extensionConfig invalidSyntax4 with {
//@[000:089) ├─ExtensionConfigAssignmentSyntax
//@[000:015) | ├─Token(Identifier) |extensionConfig|
//@[016:030) | ├─IdentifierSyntax
//@[016:030) | | └─Token(Identifier) |invalidSyntax4|
//@[031:089) | └─ExtensionWithClauseSyntax
//@[031:035) |   ├─Token(Identifier) |with|
//@[036:089) |   └─ObjectSyntax
//@[036:037) |     ├─Token(LeftBrace) |{|
//@[037:038) |     ├─Token(NewLine) |\n|
  requiredString: validAssignment1.requiredString
//@[002:049) |     ├─ObjectPropertySyntax
//@[002:016) |     | ├─IdentifierSyntax
//@[002:016) |     | | └─Token(Identifier) |requiredString|
//@[016:017) |     | ├─Token(Colon) |:|
//@[018:049) |     | └─PropertyAccessSyntax
//@[018:034) |     |   ├─VariableAccessSyntax
//@[018:034) |     |   | └─IdentifierSyntax
//@[018:034) |     |   |   └─Token(Identifier) |validAssignment1|
//@[034:035) |     |   ├─Token(Dot) |.|
//@[035:049) |     |   └─IdentifierSyntax
//@[035:049) |     |     └─Token(Identifier) |requiredString|
//@[049:050) |     ├─Token(NewLine) |\n|
}
//@[000:001) |     └─Token(RightBrace) |}|
//@[001:003) ├─Token(NewLine) |\n\n|

extensionConfig invalidSyntax5 with {
//@[000:061) ├─ExtensionConfigAssignmentSyntax
//@[000:015) | ├─Token(Identifier) |extensionConfig|
//@[016:030) | ├─IdentifierSyntax
//@[016:030) | | └─Token(Identifier) |invalidSyntax5|
//@[031:061) | └─ExtensionWithClauseSyntax
//@[031:035) |   ├─Token(Identifier) |with|
//@[036:061) |   └─ObjectSyntax
//@[036:037) |     ├─Token(LeftBrace) |{|
//@[037:038) |     ├─Token(NewLine) |\n|
  ...validAssignment1
//@[002:021) |     ├─SpreadExpressionSyntax
//@[002:005) |     | ├─Token(Ellipsis) |...|
//@[005:021) |     | └─VariableAccessSyntax
//@[005:021) |     |   └─IdentifierSyntax
//@[005:021) |     |     └─Token(Identifier) |validAssignment1|
//@[021:022) |     ├─Token(NewLine) |\n|
}
//@[000:001) |     └─Token(RightBrace) |}|
//@[001:003) ├─Token(NewLine) |\n\n|

extensionConfig invalidAssignment1 with {
//@[000:071) ├─ExtensionConfigAssignmentSyntax
//@[000:015) | ├─Token(Identifier) |extensionConfig|
//@[016:034) | ├─IdentifierSyntax
//@[016:034) | | └─Token(Identifier) |invalidAssignment1|
//@[035:071) | └─ExtensionWithClauseSyntax
//@[035:039) |   ├─Token(Identifier) |with|
//@[040:071) |   └─ObjectSyntax
//@[040:041) |     ├─Token(LeftBrace) |{|
//@[041:042) |     ├─Token(NewLine) |\n|
  requiredString: strParam1
//@[002:027) |     ├─ObjectPropertySyntax
//@[002:016) |     | ├─IdentifierSyntax
//@[002:016) |     | | └─Token(Identifier) |requiredString|
//@[016:017) |     | ├─Token(Colon) |:|
//@[018:027) |     | └─VariableAccessSyntax
//@[018:027) |     |   └─IdentifierSyntax
//@[018:027) |     |     └─Token(Identifier) |strParam1|
//@[027:028) |     ├─Token(NewLine) |\n|
}
//@[000:001) |     └─Token(RightBrace) |}|
//@[001:003) ├─Token(NewLine) |\n\n|

extensionConfig invalidSecretAssignment1 with {
//@[000:189) ├─ExtensionConfigAssignmentSyntax
//@[000:015) | ├─Token(Identifier) |extensionConfig|
//@[016:040) | ├─IdentifierSyntax
//@[016:040) | | └─Token(Identifier) |invalidSecretAssignment1|
//@[041:189) | └─ExtensionWithClauseSyntax
//@[041:045) |   ├─Token(Identifier) |with|
//@[046:189) |   └─ObjectSyntax
//@[046:047) |     ├─Token(LeftBrace) |{|
//@[047:048) |     ├─Token(NewLine) |\n|
  requiredSecureString: bool(readEnvironmentVariable('xyz', 'false')) ? az.getSecret('a', 'b', 'c', 'd') : az.getSecret('w', 'x', 'y', 'z')
//@[002:139) |     ├─ObjectPropertySyntax
//@[002:022) |     | ├─IdentifierSyntax
//@[002:022) |     | | └─Token(Identifier) |requiredSecureString|
//@[022:023) |     | ├─Token(Colon) |:|
//@[024:139) |     | └─TernaryOperationSyntax
//@[024:069) |     |   ├─FunctionCallSyntax
//@[024:028) |     |   | ├─IdentifierSyntax
//@[024:028) |     |   | | └─Token(Identifier) |bool|
//@[028:029) |     |   | ├─Token(LeftParen) |(|
//@[029:068) |     |   | ├─FunctionArgumentSyntax
//@[029:068) |     |   | | └─FunctionCallSyntax
//@[029:052) |     |   | |   ├─IdentifierSyntax
//@[029:052) |     |   | |   | └─Token(Identifier) |readEnvironmentVariable|
//@[052:053) |     |   | |   ├─Token(LeftParen) |(|
//@[053:058) |     |   | |   ├─FunctionArgumentSyntax
//@[053:058) |     |   | |   | └─StringSyntax
//@[053:058) |     |   | |   |   └─Token(StringComplete) |'xyz'|
//@[058:059) |     |   | |   ├─Token(Comma) |,|
//@[060:067) |     |   | |   ├─FunctionArgumentSyntax
//@[060:067) |     |   | |   | └─StringSyntax
//@[060:067) |     |   | |   |   └─Token(StringComplete) |'false'|
//@[067:068) |     |   | |   └─Token(RightParen) |)|
//@[068:069) |     |   | └─Token(RightParen) |)|
//@[070:071) |     |   ├─Token(Question) |?|
//@[072:104) |     |   ├─InstanceFunctionCallSyntax
//@[072:074) |     |   | ├─VariableAccessSyntax
//@[072:074) |     |   | | └─IdentifierSyntax
//@[072:074) |     |   | |   └─Token(Identifier) |az|
//@[074:075) |     |   | ├─Token(Dot) |.|
//@[075:084) |     |   | ├─IdentifierSyntax
//@[075:084) |     |   | | └─Token(Identifier) |getSecret|
//@[084:085) |     |   | ├─Token(LeftParen) |(|
//@[085:088) |     |   | ├─FunctionArgumentSyntax
//@[085:088) |     |   | | └─StringSyntax
//@[085:088) |     |   | |   └─Token(StringComplete) |'a'|
//@[088:089) |     |   | ├─Token(Comma) |,|
//@[090:093) |     |   | ├─FunctionArgumentSyntax
//@[090:093) |     |   | | └─StringSyntax
//@[090:093) |     |   | |   └─Token(StringComplete) |'b'|
//@[093:094) |     |   | ├─Token(Comma) |,|
//@[095:098) |     |   | ├─FunctionArgumentSyntax
//@[095:098) |     |   | | └─StringSyntax
//@[095:098) |     |   | |   └─Token(StringComplete) |'c'|
//@[098:099) |     |   | ├─Token(Comma) |,|
//@[100:103) |     |   | ├─FunctionArgumentSyntax
//@[100:103) |     |   | | └─StringSyntax
//@[100:103) |     |   | |   └─Token(StringComplete) |'d'|
//@[103:104) |     |   | └─Token(RightParen) |)|
//@[105:106) |     |   ├─Token(Colon) |:|
//@[107:139) |     |   └─InstanceFunctionCallSyntax
//@[107:109) |     |     ├─VariableAccessSyntax
//@[107:109) |     |     | └─IdentifierSyntax
//@[107:109) |     |     |   └─Token(Identifier) |az|
//@[109:110) |     |     ├─Token(Dot) |.|
//@[110:119) |     |     ├─IdentifierSyntax
//@[110:119) |     |     | └─Token(Identifier) |getSecret|
//@[119:120) |     |     ├─Token(LeftParen) |(|
//@[120:123) |     |     ├─FunctionArgumentSyntax
//@[120:123) |     |     | └─StringSyntax
//@[120:123) |     |     |   └─Token(StringComplete) |'w'|
//@[123:124) |     |     ├─Token(Comma) |,|
//@[125:128) |     |     ├─FunctionArgumentSyntax
//@[125:128) |     |     | └─StringSyntax
//@[125:128) |     |     |   └─Token(StringComplete) |'x'|
//@[128:129) |     |     ├─Token(Comma) |,|
//@[130:133) |     |     ├─FunctionArgumentSyntax
//@[130:133) |     |     | └─StringSyntax
//@[130:133) |     |     |   └─Token(StringComplete) |'y'|
//@[133:134) |     |     ├─Token(Comma) |,|
//@[135:138) |     |     ├─FunctionArgumentSyntax
//@[135:138) |     |     | └─StringSyntax
//@[135:138) |     |     |   └─Token(StringComplete) |'z'|
//@[138:139) |     |     └─Token(RightParen) |)|
//@[139:140) |     ├─Token(NewLine) |\n|
}
//@[000:001) |     └─Token(RightBrace) |}|
//@[001:002) ├─Token(NewLine) |\n|

//@[000:000) └─Token(EndOfFile) ||
