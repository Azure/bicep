using 'main.bicep'
//@[000:1092) ProgramSyntax
//@[000:0018) ├─UsingDeclarationSyntax
//@[000:0005) | ├─Token(Identifier) |using|
//@[006:0018) | └─StringSyntax
//@[006:0018) |   └─Token(StringComplete) |'main.bicep'|
//@[018:0020) ├─Token(NewLine) |\n\n|

param strParam1 = 'strParam1Value'
//@[000:0034) ├─ParameterAssignmentSyntax
//@[000:0005) | ├─Token(Identifier) |param|
//@[006:0015) | ├─IdentifierSyntax
//@[006:0015) | | └─Token(Identifier) |strParam1|
//@[016:0017) | ├─Token(Assignment) |=|
//@[018:0034) | └─StringSyntax
//@[018:0034) |   └─Token(StringComplete) |'strParam1Value'|
//@[034:0035) ├─Token(NewLine) |\n|
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
//@[060:0062) ├─Token(NewLine) |\n\n|

extensionConfig hasObjConfig1 with {
//@[000:0074) ├─ExtensionConfigAssignmentSyntax
//@[000:0015) | ├─Token(Identifier) |extensionConfig|
//@[016:0029) | ├─IdentifierSyntax
//@[016:0029) | | └─Token(Identifier) |hasObjConfig1|
//@[030:0074) | └─ExtensionWithClauseSyntax
//@[030:0034) |   ├─Token(Identifier) |with|
//@[035:0074) |   └─ObjectSyntax
//@[035:0036) |     ├─Token(LeftBrace) |{|
//@[036:0037) |     ├─Token(NewLine) |\n|
  requiredString: 'valueFromParams'
//@[002:0035) |     ├─ObjectPropertySyntax
//@[002:0016) |     | ├─IdentifierSyntax
//@[002:0016) |     | | └─Token(Identifier) |requiredString|
//@[016:0017) |     | ├─Token(Colon) |:|
//@[018:0035) |     | └─StringSyntax
//@[018:0035) |     |   └─Token(StringComplete) |'valueFromParams'|
//@[035:0036) |     ├─Token(NewLine) |\n|
}
//@[000:0001) |     └─Token(RightBrace) |}|
//@[001:0003) ├─Token(NewLine) |\n\n|

extensionConfig hasObjConfig2 with {
//@[000:0067) ├─ExtensionConfigAssignmentSyntax
//@[000:0015) | ├─Token(Identifier) |extensionConfig|
//@[016:0029) | ├─IdentifierSyntax
//@[016:0029) | | └─Token(Identifier) |hasObjConfig2|
//@[030:0067) | └─ExtensionWithClauseSyntax
//@[030:0034) |   ├─Token(Identifier) |with|
//@[035:0067) |   └─ObjectSyntax
//@[035:0036) |     ├─Token(LeftBrace) |{|
//@[036:0037) |     ├─Token(NewLine) |\n|
  optionalString: 'optional'
//@[002:0028) |     ├─ObjectPropertySyntax
//@[002:0016) |     | ├─IdentifierSyntax
//@[002:0016) |     | | └─Token(Identifier) |optionalString|
//@[016:0017) |     | ├─Token(Colon) |:|
//@[018:0028) |     | └─StringSyntax
//@[018:0028) |     |   └─Token(StringComplete) |'optional'|
//@[028:0029) |     ├─Token(NewLine) |\n|
}
//@[000:0001) |     └─Token(RightBrace) |}|
//@[001:0003) ├─Token(NewLine) |\n\n|

extensionConfig hasObjConfig3 with {}
//@[000:0037) ├─ExtensionConfigAssignmentSyntax
//@[000:0015) | ├─Token(Identifier) |extensionConfig|
//@[016:0029) | ├─IdentifierSyntax
//@[016:0029) | | └─Token(Identifier) |hasObjConfig3|
//@[030:0037) | └─ExtensionWithClauseSyntax
//@[030:0034) |   ├─Token(Identifier) |with|
//@[035:0037) |   └─ObjectSyntax
//@[035:0036) |     ├─Token(LeftBrace) |{|
//@[036:0037) |     └─Token(RightBrace) |}|
//@[037:0039) ├─Token(NewLine) |\n\n|

// hasObjConfig4 not here to test assignment is not required because required field is defaulted
//@[096:0098) ├─Token(NewLine) |\n\n|

extensionConfig hasObjConfig5 with {
//@[000:0157) ├─ExtensionConfigAssignmentSyntax
//@[000:0015) | ├─Token(Identifier) |extensionConfig|
//@[016:0029) | ├─IdentifierSyntax
//@[016:0029) | | └─Token(Identifier) |hasObjConfig5|
//@[030:0157) | └─ExtensionWithClauseSyntax
//@[030:0034) |   ├─Token(Identifier) |with|
//@[035:0157) |   └─ObjectSyntax
//@[035:0036) |     ├─Token(LeftBrace) |{|
//@[036:0037) |     ├─Token(NewLine) |\n|
  requiredString: 'asdf'
//@[002:0024) |     ├─ObjectPropertySyntax
//@[002:0016) |     | ├─IdentifierSyntax
//@[002:0016) |     | | └─Token(Identifier) |requiredString|
//@[016:0017) |     | ├─Token(Colon) |:|
//@[018:0024) |     | └─StringSyntax
//@[018:0024) |     |   └─Token(StringComplete) |'asdf'|
//@[024:0025) |     ├─Token(NewLine) |\n|
  optionalString: bool(readEnvironmentVariable('xyz', 'false')) ? 'inlineVal1' : 'inlineVal2'
//@[002:0093) |     ├─ObjectPropertySyntax
//@[002:0016) |     | ├─IdentifierSyntax
//@[002:0016) |     | | └─Token(Identifier) |optionalString|
//@[016:0017) |     | ├─Token(Colon) |:|
//@[018:0093) |     | └─TernaryOperationSyntax
//@[018:0063) |     |   ├─FunctionCallSyntax
//@[018:0022) |     |   | ├─IdentifierSyntax
//@[018:0022) |     |   | | └─Token(Identifier) |bool|
//@[022:0023) |     |   | ├─Token(LeftParen) |(|
//@[023:0062) |     |   | ├─FunctionArgumentSyntax
//@[023:0062) |     |   | | └─FunctionCallSyntax
//@[023:0046) |     |   | |   ├─IdentifierSyntax
//@[023:0046) |     |   | |   | └─Token(Identifier) |readEnvironmentVariable|
//@[046:0047) |     |   | |   ├─Token(LeftParen) |(|
//@[047:0052) |     |   | |   ├─FunctionArgumentSyntax
//@[047:0052) |     |   | |   | └─StringSyntax
//@[047:0052) |     |   | |   |   └─Token(StringComplete) |'xyz'|
//@[052:0053) |     |   | |   ├─Token(Comma) |,|
//@[054:0061) |     |   | |   ├─FunctionArgumentSyntax
//@[054:0061) |     |   | |   | └─StringSyntax
//@[054:0061) |     |   | |   |   └─Token(StringComplete) |'false'|
//@[061:0062) |     |   | |   └─Token(RightParen) |)|
//@[062:0063) |     |   | └─Token(RightParen) |)|
//@[064:0065) |     |   ├─Token(Question) |?|
//@[066:0078) |     |   ├─StringSyntax
//@[066:0078) |     |   | └─Token(StringComplete) |'inlineVal1'|
//@[079:0080) |     |   ├─Token(Colon) |:|
//@[081:0093) |     |   └─StringSyntax
//@[081:0093) |     |     └─Token(StringComplete) |'inlineVal2'|
//@[093:0094) |     ├─Token(NewLine) |\n|
}
//@[000:0001) |     └─Token(RightBrace) |}|
//@[001:0003) ├─Token(NewLine) |\n\n|

extensionConfig hasSecureConfig1 with {
//@[000:0147) ├─ExtensionConfigAssignmentSyntax
//@[000:0015) | ├─Token(Identifier) |extensionConfig|
//@[016:0032) | ├─IdentifierSyntax
//@[016:0032) | | └─Token(Identifier) |hasSecureConfig1|
//@[033:0147) | └─ExtensionWithClauseSyntax
//@[033:0037) |   ├─Token(Identifier) |with|
//@[038:0147) |   └─ObjectSyntax
//@[038:0039) |     ├─Token(LeftBrace) |{|
//@[039:0040) |     ├─Token(NewLine) |\n|
  requiredSecureString: az.getSecret('00000000-0000-0000-0000-000000000001', 'mock-rg', 'kv', 'mySecret')
//@[002:0105) |     ├─ObjectPropertySyntax
//@[002:0022) |     | ├─IdentifierSyntax
//@[002:0022) |     | | └─Token(Identifier) |requiredSecureString|
//@[022:0023) |     | ├─Token(Colon) |:|
//@[024:0105) |     | └─InstanceFunctionCallSyntax
//@[024:0026) |     |   ├─VariableAccessSyntax
//@[024:0026) |     |   | └─IdentifierSyntax
//@[024:0026) |     |   |   └─Token(Identifier) |az|
//@[026:0027) |     |   ├─Token(Dot) |.|
//@[027:0036) |     |   ├─IdentifierSyntax
//@[027:0036) |     |   | └─Token(Identifier) |getSecret|
//@[036:0037) |     |   ├─Token(LeftParen) |(|
//@[037:0075) |     |   ├─FunctionArgumentSyntax
//@[037:0075) |     |   | └─StringSyntax
//@[037:0075) |     |   |   └─Token(StringComplete) |'00000000-0000-0000-0000-000000000001'|
//@[075:0076) |     |   ├─Token(Comma) |,|
//@[077:0086) |     |   ├─FunctionArgumentSyntax
//@[077:0086) |     |   | └─StringSyntax
//@[077:0086) |     |   |   └─Token(StringComplete) |'mock-rg'|
//@[086:0087) |     |   ├─Token(Comma) |,|
//@[088:0092) |     |   ├─FunctionArgumentSyntax
//@[088:0092) |     |   | └─StringSyntax
//@[088:0092) |     |   |   └─Token(StringComplete) |'kv'|
//@[092:0093) |     |   ├─Token(Comma) |,|
//@[094:0104) |     |   ├─FunctionArgumentSyntax
//@[094:0104) |     |   | └─StringSyntax
//@[094:0104) |     |   |   └─Token(StringComplete) |'mySecret'|
//@[104:0105) |     |   └─Token(RightParen) |)|
//@[105:0106) |     ├─Token(NewLine) |\n|
}
//@[000:0001) |     └─Token(RightBrace) |}|
//@[001:0003) ├─Token(NewLine) |\n\n|

extensionConfig hasSecureConfig2 with {
//@[000:0111) ├─ExtensionConfigAssignmentSyntax
//@[000:0015) | ├─Token(Identifier) |extensionConfig|
//@[016:0032) | ├─IdentifierSyntax
//@[016:0032) | | └─Token(Identifier) |hasSecureConfig2|
//@[033:0111) | └─ExtensionWithClauseSyntax
//@[033:0037) |   ├─Token(Identifier) |with|
//@[038:0111) |   └─ObjectSyntax
//@[038:0039) |     ├─Token(LeftBrace) |{|
//@[039:0040) |     ├─Token(NewLine) |\n|
  requiredSecureString: 'Inlined'
//@[002:0033) |     ├─ObjectPropertySyntax
//@[002:0022) |     | ├─IdentifierSyntax
//@[002:0022) |     | | └─Token(Identifier) |requiredSecureString|
//@[022:0023) |     | ├─Token(Colon) |:|
//@[024:0033) |     | └─StringSyntax
//@[024:0033) |     |   └─Token(StringComplete) |'Inlined'|
//@[033:0034) |     ├─Token(NewLine) |\n|
  optionalString: 'valueFromParams'
//@[002:0035) |     ├─ObjectPropertySyntax
//@[002:0016) |     | ├─IdentifierSyntax
//@[002:0016) |     | | └─Token(Identifier) |optionalString|
//@[016:0017) |     | ├─Token(Colon) |:|
//@[018:0035) |     | └─StringSyntax
//@[018:0035) |     |   └─Token(StringComplete) |'valueFromParams'|
//@[035:0036) |     ├─Token(NewLine) |\n|
}
//@[000:0001) |     └─Token(RightBrace) |}|
//@[001:0003) ├─Token(NewLine) |\n\n|

extensionConfig hasDiscrimConfig1 with {
//@[000:0081) ├─ExtensionConfigAssignmentSyntax
//@[000:0015) | ├─Token(Identifier) |extensionConfig|
//@[016:0033) | ├─IdentifierSyntax
//@[016:0033) | | └─Token(Identifier) |hasDiscrimConfig1|
//@[034:0081) | └─ExtensionWithClauseSyntax
//@[034:0038) |   ├─Token(Identifier) |with|
//@[039:0081) |   └─ObjectSyntax
//@[039:0040) |     ├─Token(LeftBrace) |{|
//@[040:0041) |     ├─Token(NewLine) |\n|
  discrim: 'a'
//@[002:0014) |     ├─ObjectPropertySyntax
//@[002:0009) |     | ├─IdentifierSyntax
//@[002:0009) |     | | └─Token(Identifier) |discrim|
//@[009:0010) |     | ├─Token(Colon) |:|
//@[011:0014) |     | └─StringSyntax
//@[011:0014) |     |   └─Token(StringComplete) |'a'|
//@[014:0015) |     ├─Token(NewLine) |\n|
  z1: 'z1v'
//@[002:0011) |     ├─ObjectPropertySyntax
//@[002:0004) |     | ├─IdentifierSyntax
//@[002:0004) |     | | └─Token(Identifier) |z1|
//@[004:0005) |     | ├─Token(Colon) |:|
//@[006:0011) |     | └─StringSyntax
//@[006:0011) |     |   └─Token(StringComplete) |'z1v'|
//@[011:0012) |     ├─Token(NewLine) |\n|
  a1: 'a1v'
//@[002:0011) |     ├─ObjectPropertySyntax
//@[002:0004) |     | ├─IdentifierSyntax
//@[002:0004) |     | | └─Token(Identifier) |a1|
//@[004:0005) |     | ├─Token(Colon) |:|
//@[006:0011) |     | └─StringSyntax
//@[006:0011) |     |   └─Token(StringComplete) |'a1v'|
//@[011:0012) |     ├─Token(NewLine) |\n|
}
//@[000:0001) |     └─Token(RightBrace) |}|
//@[001:0003) ├─Token(NewLine) |\n\n|

extensionConfig hasDiscrimConfig2 with {
//@[000:0054) ├─ExtensionConfigAssignmentSyntax
//@[000:0015) | ├─Token(Identifier) |extensionConfig|
//@[016:0033) | ├─IdentifierSyntax
//@[016:0033) | | └─Token(Identifier) |hasDiscrimConfig2|
//@[034:0054) | └─ExtensionWithClauseSyntax
//@[034:0038) |   ├─Token(Identifier) |with|
//@[039:0054) |   └─ObjectSyntax
//@[039:0040) |     ├─Token(LeftBrace) |{|
//@[040:0041) |     ├─Token(NewLine) |\n|
  a1: 'a1v'
//@[002:0011) |     ├─ObjectPropertySyntax
//@[002:0004) |     | ├─IdentifierSyntax
//@[002:0004) |     | | └─Token(Identifier) |a1|
//@[004:0005) |     | ├─Token(Colon) |:|
//@[006:0011) |     | └─StringSyntax
//@[006:0011) |     |   └─Token(StringComplete) |'a1v'|
//@[011:0012) |     ├─Token(NewLine) |\n|
}
//@[000:0001) |     └─Token(RightBrace) |}|
//@[001:0003) ├─Token(NewLine) |\n\n|

extensionConfig hasDiscrimConfig3 with {
//@[000:0132) ├─ExtensionConfigAssignmentSyntax
//@[000:0015) | ├─Token(Identifier) |extensionConfig|
//@[016:0033) | ├─IdentifierSyntax
//@[016:0033) | | └─Token(Identifier) |hasDiscrimConfig3|
//@[034:0132) | └─ExtensionWithClauseSyntax
//@[034:0038) |   ├─Token(Identifier) |with|
//@[039:0132) |   └─ObjectSyntax
//@[039:0040) |     ├─Token(LeftBrace) |{|
//@[040:0041) |     ├─Token(NewLine) |\n|
  discrim: 'b'
//@[002:0014) |     ├─ObjectPropertySyntax
//@[002:0009) |     | ├─IdentifierSyntax
//@[002:0009) |     | | └─Token(Identifier) |discrim|
//@[009:0010) |     | ├─Token(Colon) |:|
//@[011:0014) |     | └─StringSyntax
//@[011:0014) |     |   └─Token(StringComplete) |'b'|
//@[014:0015) |     ├─Token(NewLine) |\n|
  b1: bool(readEnvironmentVariable('xyz', 'false')) ? 'b1True' : 'b1False'
//@[002:0074) |     ├─ObjectPropertySyntax
//@[002:0004) |     | ├─IdentifierSyntax
//@[002:0004) |     | | └─Token(Identifier) |b1|
//@[004:0005) |     | ├─Token(Colon) |:|
//@[006:0074) |     | └─TernaryOperationSyntax
//@[006:0051) |     |   ├─FunctionCallSyntax
//@[006:0010) |     |   | ├─IdentifierSyntax
//@[006:0010) |     |   | | └─Token(Identifier) |bool|
//@[010:0011) |     |   | ├─Token(LeftParen) |(|
//@[011:0050) |     |   | ├─FunctionArgumentSyntax
//@[011:0050) |     |   | | └─FunctionCallSyntax
//@[011:0034) |     |   | |   ├─IdentifierSyntax
//@[011:0034) |     |   | |   | └─Token(Identifier) |readEnvironmentVariable|
//@[034:0035) |     |   | |   ├─Token(LeftParen) |(|
//@[035:0040) |     |   | |   ├─FunctionArgumentSyntax
//@[035:0040) |     |   | |   | └─StringSyntax
//@[035:0040) |     |   | |   |   └─Token(StringComplete) |'xyz'|
//@[040:0041) |     |   | |   ├─Token(Comma) |,|
//@[042:0049) |     |   | |   ├─FunctionArgumentSyntax
//@[042:0049) |     |   | |   | └─StringSyntax
//@[042:0049) |     |   | |   |   └─Token(StringComplete) |'false'|
//@[049:0050) |     |   | |   └─Token(RightParen) |)|
//@[050:0051) |     |   | └─Token(RightParen) |)|
//@[052:0053) |     |   ├─Token(Question) |?|
//@[054:0062) |     |   ├─StringSyntax
//@[054:0062) |     |   | └─Token(StringComplete) |'b1True'|
//@[063:0064) |     |   ├─Token(Colon) |:|
//@[065:0074) |     |   └─StringSyntax
//@[065:0074) |     |     └─Token(StringComplete) |'b1False'|
//@[074:0075) |     ├─Token(NewLine) |\n|
}
//@[000:0001) |     └─Token(RightBrace) |}|
//@[001:0002) ├─Token(NewLine) |\n|

//@[000:0000) └─Token(EndOfFile) ||
