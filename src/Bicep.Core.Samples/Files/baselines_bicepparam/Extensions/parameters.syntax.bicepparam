using 'main.bicep'
//@[000:773) ProgramSyntax
//@[000:018) ├─UsingDeclarationSyntax
//@[000:005) | ├─Token(Identifier) |using|
//@[006:018) | └─StringSyntax
//@[006:018) |   └─Token(StringComplete) |'main.bicep'|
//@[018:020) ├─Token(NewLine) |\n\n|

extensionConfig hasObjConfig1 with {
//@[000:074) ├─ExtensionConfigAssignmentSyntax
//@[000:015) | ├─Token(Identifier) |extensionConfig|
//@[016:029) | ├─IdentifierSyntax
//@[016:029) | | └─Token(Identifier) |hasObjConfig1|
//@[030:074) | └─ExtensionWithClauseSyntax
//@[030:034) |   ├─Token(Identifier) |with|
//@[035:074) |   └─ObjectSyntax
//@[035:036) |     ├─Token(LeftBrace) |{|
//@[036:037) |     ├─Token(NewLine) |\n|
  requiredString: 'valueFromParams'
//@[002:035) |     ├─ObjectPropertySyntax
//@[002:016) |     | ├─IdentifierSyntax
//@[002:016) |     | | └─Token(Identifier) |requiredString|
//@[016:017) |     | ├─Token(Colon) |:|
//@[018:035) |     | └─StringSyntax
//@[018:035) |     |   └─Token(StringComplete) |'valueFromParams'|
//@[035:036) |     ├─Token(NewLine) |\n|
}
//@[000:001) |     └─Token(RightBrace) |}|
//@[001:003) ├─Token(NewLine) |\n\n|

extensionConfig hasObjConfig2 with {
//@[000:067) ├─ExtensionConfigAssignmentSyntax
//@[000:015) | ├─Token(Identifier) |extensionConfig|
//@[016:029) | ├─IdentifierSyntax
//@[016:029) | | └─Token(Identifier) |hasObjConfig2|
//@[030:067) | └─ExtensionWithClauseSyntax
//@[030:034) |   ├─Token(Identifier) |with|
//@[035:067) |   └─ObjectSyntax
//@[035:036) |     ├─Token(LeftBrace) |{|
//@[036:037) |     ├─Token(NewLine) |\n|
  optionalString: 'optional'
//@[002:028) |     ├─ObjectPropertySyntax
//@[002:016) |     | ├─IdentifierSyntax
//@[002:016) |     | | └─Token(Identifier) |optionalString|
//@[016:017) |     | ├─Token(Colon) |:|
//@[018:028) |     | └─StringSyntax
//@[018:028) |     |   └─Token(StringComplete) |'optional'|
//@[028:029) |     ├─Token(NewLine) |\n|
}
//@[000:001) |     └─Token(RightBrace) |}|
//@[001:003) ├─Token(NewLine) |\n\n|

extensionConfig hasObjConfig3 with {}
//@[000:037) ├─ExtensionConfigAssignmentSyntax
//@[000:015) | ├─Token(Identifier) |extensionConfig|
//@[016:029) | ├─IdentifierSyntax
//@[016:029) | | └─Token(Identifier) |hasObjConfig3|
//@[030:037) | └─ExtensionWithClauseSyntax
//@[030:034) |   ├─Token(Identifier) |with|
//@[035:037) |   └─ObjectSyntax
//@[035:036) |     ├─Token(LeftBrace) |{|
//@[036:037) |     └─Token(RightBrace) |}|
//@[037:039) ├─Token(NewLine) |\n\n|

// hasObjConfig4 not here to test assignment is not required because required field is defaulted
//@[096:098) ├─Token(NewLine) |\n\n|

extensionConfig hasSecureConfig1 with {
//@[000:147) ├─ExtensionConfigAssignmentSyntax
//@[000:015) | ├─Token(Identifier) |extensionConfig|
//@[016:032) | ├─IdentifierSyntax
//@[016:032) | | └─Token(Identifier) |hasSecureConfig1|
//@[033:147) | └─ExtensionWithClauseSyntax
//@[033:037) |   ├─Token(Identifier) |with|
//@[038:147) |   └─ObjectSyntax
//@[038:039) |     ├─Token(LeftBrace) |{|
//@[039:040) |     ├─Token(NewLine) |\n|
  requiredSecureString: az.getSecret('00000000-0000-0000-0000-000000000001', 'mock-rg', 'kv', 'mySecret')
//@[002:105) |     ├─ObjectPropertySyntax
//@[002:022) |     | ├─IdentifierSyntax
//@[002:022) |     | | └─Token(Identifier) |requiredSecureString|
//@[022:023) |     | ├─Token(Colon) |:|
//@[024:105) |     | └─InstanceFunctionCallSyntax
//@[024:026) |     |   ├─VariableAccessSyntax
//@[024:026) |     |   | └─IdentifierSyntax
//@[024:026) |     |   |   └─Token(Identifier) |az|
//@[026:027) |     |   ├─Token(Dot) |.|
//@[027:036) |     |   ├─IdentifierSyntax
//@[027:036) |     |   | └─Token(Identifier) |getSecret|
//@[036:037) |     |   ├─Token(LeftParen) |(|
//@[037:075) |     |   ├─FunctionArgumentSyntax
//@[037:075) |     |   | └─StringSyntax
//@[037:075) |     |   |   └─Token(StringComplete) |'00000000-0000-0000-0000-000000000001'|
//@[075:076) |     |   ├─Token(Comma) |,|
//@[077:086) |     |   ├─FunctionArgumentSyntax
//@[077:086) |     |   | └─StringSyntax
//@[077:086) |     |   |   └─Token(StringComplete) |'mock-rg'|
//@[086:087) |     |   ├─Token(Comma) |,|
//@[088:092) |     |   ├─FunctionArgumentSyntax
//@[088:092) |     |   | └─StringSyntax
//@[088:092) |     |   |   └─Token(StringComplete) |'kv'|
//@[092:093) |     |   ├─Token(Comma) |,|
//@[094:104) |     |   ├─FunctionArgumentSyntax
//@[094:104) |     |   | └─StringSyntax
//@[094:104) |     |   |   └─Token(StringComplete) |'mySecret'|
//@[104:105) |     |   └─Token(RightParen) |)|
//@[105:106) |     ├─Token(NewLine) |\n|
}
//@[000:001) |     └─Token(RightBrace) |}|
//@[001:003) ├─Token(NewLine) |\n\n|

extensionConfig hasSecureConfig2 with {
//@[000:111) ├─ExtensionConfigAssignmentSyntax
//@[000:015) | ├─Token(Identifier) |extensionConfig|
//@[016:032) | ├─IdentifierSyntax
//@[016:032) | | └─Token(Identifier) |hasSecureConfig2|
//@[033:111) | └─ExtensionWithClauseSyntax
//@[033:037) |   ├─Token(Identifier) |with|
//@[038:111) |   └─ObjectSyntax
//@[038:039) |     ├─Token(LeftBrace) |{|
//@[039:040) |     ├─Token(NewLine) |\n|
  requiredSecureString: 'Inlined'
//@[002:033) |     ├─ObjectPropertySyntax
//@[002:022) |     | ├─IdentifierSyntax
//@[002:022) |     | | └─Token(Identifier) |requiredSecureString|
//@[022:023) |     | ├─Token(Colon) |:|
//@[024:033) |     | └─StringSyntax
//@[024:033) |     |   └─Token(StringComplete) |'Inlined'|
//@[033:034) |     ├─Token(NewLine) |\n|
  optionalString: 'valueFromParams'
//@[002:035) |     ├─ObjectPropertySyntax
//@[002:016) |     | ├─IdentifierSyntax
//@[002:016) |     | | └─Token(Identifier) |optionalString|
//@[016:017) |     | ├─Token(Colon) |:|
//@[018:035) |     | └─StringSyntax
//@[018:035) |     |   └─Token(StringComplete) |'valueFromParams'|
//@[035:036) |     ├─Token(NewLine) |\n|
}
//@[000:001) |     └─Token(RightBrace) |}|
//@[001:003) ├─Token(NewLine) |\n\n|

extensionConfig hasDiscrimConfig1 with {
//@[000:081) ├─ExtensionConfigAssignmentSyntax
//@[000:015) | ├─Token(Identifier) |extensionConfig|
//@[016:033) | ├─IdentifierSyntax
//@[016:033) | | └─Token(Identifier) |hasDiscrimConfig1|
//@[034:081) | └─ExtensionWithClauseSyntax
//@[034:038) |   ├─Token(Identifier) |with|
//@[039:081) |   └─ObjectSyntax
//@[039:040) |     ├─Token(LeftBrace) |{|
//@[040:041) |     ├─Token(NewLine) |\n|
  discrim: 'a'
//@[002:014) |     ├─ObjectPropertySyntax
//@[002:009) |     | ├─IdentifierSyntax
//@[002:009) |     | | └─Token(Identifier) |discrim|
//@[009:010) |     | ├─Token(Colon) |:|
//@[011:014) |     | └─StringSyntax
//@[011:014) |     |   └─Token(StringComplete) |'a'|
//@[014:015) |     ├─Token(NewLine) |\n|
  z1: 'z1v'
//@[002:011) |     ├─ObjectPropertySyntax
//@[002:004) |     | ├─IdentifierSyntax
//@[002:004) |     | | └─Token(Identifier) |z1|
//@[004:005) |     | ├─Token(Colon) |:|
//@[006:011) |     | └─StringSyntax
//@[006:011) |     |   └─Token(StringComplete) |'z1v'|
//@[011:012) |     ├─Token(NewLine) |\n|
  a1: 'a1v'
//@[002:011) |     ├─ObjectPropertySyntax
//@[002:004) |     | ├─IdentifierSyntax
//@[002:004) |     | | └─Token(Identifier) |a1|
//@[004:005) |     | ├─Token(Colon) |:|
//@[006:011) |     | └─StringSyntax
//@[006:011) |     |   └─Token(StringComplete) |'a1v'|
//@[011:012) |     ├─Token(NewLine) |\n|
}
//@[000:001) |     └─Token(RightBrace) |}|
//@[001:003) ├─Token(NewLine) |\n\n|

extensionConfig hasDiscrimConfig2 with {
//@[000:054) ├─ExtensionConfigAssignmentSyntax
//@[000:015) | ├─Token(Identifier) |extensionConfig|
//@[016:033) | ├─IdentifierSyntax
//@[016:033) | | └─Token(Identifier) |hasDiscrimConfig2|
//@[034:054) | └─ExtensionWithClauseSyntax
//@[034:038) |   ├─Token(Identifier) |with|
//@[039:054) |   └─ObjectSyntax
//@[039:040) |     ├─Token(LeftBrace) |{|
//@[040:041) |     ├─Token(NewLine) |\n|
  a1: 'a1v'
//@[002:011) |     ├─ObjectPropertySyntax
//@[002:004) |     | ├─IdentifierSyntax
//@[002:004) |     | | └─Token(Identifier) |a1|
//@[004:005) |     | ├─Token(Colon) |:|
//@[006:011) |     | └─StringSyntax
//@[006:011) |     |   └─Token(StringComplete) |'a1v'|
//@[011:012) |     ├─Token(NewLine) |\n|
}
//@[000:001) |     └─Token(RightBrace) |}|
//@[001:003) ├─Token(NewLine) |\n\n|

extensionConfig hasDiscrimConfig3 with {
//@[000:069) ├─ExtensionConfigAssignmentSyntax
//@[000:015) | ├─Token(Identifier) |extensionConfig|
//@[016:033) | ├─IdentifierSyntax
//@[016:033) | | └─Token(Identifier) |hasDiscrimConfig3|
//@[034:069) | └─ExtensionWithClauseSyntax
//@[034:038) |   ├─Token(Identifier) |with|
//@[039:069) |   └─ObjectSyntax
//@[039:040) |     ├─Token(LeftBrace) |{|
//@[040:041) |     ├─Token(NewLine) |\n|
  discrim: 'b'
//@[002:014) |     ├─ObjectPropertySyntax
//@[002:009) |     | ├─IdentifierSyntax
//@[002:009) |     | | └─Token(Identifier) |discrim|
//@[009:010) |     | ├─Token(Colon) |:|
//@[011:014) |     | └─StringSyntax
//@[011:014) |     |   └─Token(StringComplete) |'b'|
//@[014:015) |     ├─Token(NewLine) |\n|
  b1: 'b1v'
//@[002:011) |     ├─ObjectPropertySyntax
//@[002:004) |     | ├─IdentifierSyntax
//@[002:004) |     | | └─Token(Identifier) |b1|
//@[004:005) |     | ├─Token(Colon) |:|
//@[006:011) |     | └─StringSyntax
//@[006:011) |     |   └─Token(StringComplete) |'b1v'|
//@[011:012) |     ├─Token(NewLine) |\n|
}
//@[000:001) |     └─Token(RightBrace) |}|
//@[001:002) ├─Token(NewLine) |\n|

//@[000:000) └─Token(EndOfFile) ||
