using none
//@[00:2123) ProgramSyntax
//@[00:0010) ├─UsingDeclarationSyntax
//@[00:0005) | ├─Token(Identifier) |using|
//@[06:0010) | ├─NoneLiteralSyntax
//@[06:0010) | | └─Token(Identifier) |none|
//@[10:0010) | └─SkippedTriviaSyntax
//@[10:0014) ├─Token(NewLine) |\r\n\r\n|

// single parameter
//@[19:0021) ├─Token(NewLine) |\r\n|
param singleParam = externalInput('sys.cli', 'foo')
//@[00:0051) ├─ParameterAssignmentSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0017) | ├─IdentifierSyntax
//@[06:0017) | | └─Token(Identifier) |singleParam|
//@[18:0019) | ├─Token(Assignment) |=|
//@[20:0051) | └─FunctionCallSyntax
//@[20:0033) |   ├─IdentifierSyntax
//@[20:0033) |   | └─Token(Identifier) |externalInput|
//@[33:0034) |   ├─Token(LeftParen) |(|
//@[34:0043) |   ├─FunctionArgumentSyntax
//@[34:0043) |   | └─StringSyntax
//@[34:0043) |   |   └─Token(StringComplete) |'sys.cli'|
//@[43:0044) |   ├─Token(Comma) |,|
//@[45:0050) |   ├─FunctionArgumentSyntax
//@[45:0050) |   | └─StringSyntax
//@[45:0050) |   |   └─Token(StringComplete) |'foo'|
//@[50:0051) |   └─Token(RightParen) |)|
//@[51:0055) ├─Token(NewLine) |\r\n\r\n|

// single parameter with casting expression
//@[43:0045) ├─Token(NewLine) |\r\n|
param singleParamCast = bool(externalInput('sys.cli', 'foo'))
//@[00:0061) ├─ParameterAssignmentSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0021) | ├─IdentifierSyntax
//@[06:0021) | | └─Token(Identifier) |singleParamCast|
//@[22:0023) | ├─Token(Assignment) |=|
//@[24:0061) | └─FunctionCallSyntax
//@[24:0028) |   ├─IdentifierSyntax
//@[24:0028) |   | └─Token(Identifier) |bool|
//@[28:0029) |   ├─Token(LeftParen) |(|
//@[29:0060) |   ├─FunctionArgumentSyntax
//@[29:0060) |   | └─FunctionCallSyntax
//@[29:0042) |   |   ├─IdentifierSyntax
//@[29:0042) |   |   | └─Token(Identifier) |externalInput|
//@[42:0043) |   |   ├─Token(LeftParen) |(|
//@[43:0052) |   |   ├─FunctionArgumentSyntax
//@[43:0052) |   |   | └─StringSyntax
//@[43:0052) |   |   |   └─Token(StringComplete) |'sys.cli'|
//@[52:0053) |   |   ├─Token(Comma) |,|
//@[54:0059) |   |   ├─FunctionArgumentSyntax
//@[54:0059) |   |   | └─StringSyntax
//@[54:0059) |   |   |   └─Token(StringComplete) |'foo'|
//@[59:0060) |   |   └─Token(RightParen) |)|
//@[60:0061) |   └─Token(RightParen) |)|
//@[61:0065) ├─Token(NewLine) |\r\n\r\n|

// multiple parameters with different syntax
//@[44:0046) ├─Token(NewLine) |\r\n|
param foo = externalInput('sys.cli', 'foo')
//@[00:0043) ├─ParameterAssignmentSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0009) | ├─IdentifierSyntax
//@[06:0009) | | └─Token(Identifier) |foo|
//@[10:0011) | ├─Token(Assignment) |=|
//@[12:0043) | └─FunctionCallSyntax
//@[12:0025) |   ├─IdentifierSyntax
//@[12:0025) |   | └─Token(Identifier) |externalInput|
//@[25:0026) |   ├─Token(LeftParen) |(|
//@[26:0035) |   ├─FunctionArgumentSyntax
//@[26:0035) |   | └─StringSyntax
//@[26:0035) |   |   └─Token(StringComplete) |'sys.cli'|
//@[35:0036) |   ├─Token(Comma) |,|
//@[37:0042) |   ├─FunctionArgumentSyntax
//@[37:0042) |   | └─StringSyntax
//@[37:0042) |   |   └─Token(StringComplete) |'foo'|
//@[42:0043) |   └─Token(RightParen) |)|
//@[43:0045) ├─Token(NewLine) |\r\n|
param bar = externalInput('sys.envVar', 'bar')
//@[00:0046) ├─ParameterAssignmentSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0009) | ├─IdentifierSyntax
//@[06:0009) | | └─Token(Identifier) |bar|
//@[10:0011) | ├─Token(Assignment) |=|
//@[12:0046) | └─FunctionCallSyntax
//@[12:0025) |   ├─IdentifierSyntax
//@[12:0025) |   | └─Token(Identifier) |externalInput|
//@[25:0026) |   ├─Token(LeftParen) |(|
//@[26:0038) |   ├─FunctionArgumentSyntax
//@[26:0038) |   | └─StringSyntax
//@[26:0038) |   |   └─Token(StringComplete) |'sys.envVar'|
//@[38:0039) |   ├─Token(Comma) |,|
//@[40:0045) |   ├─FunctionArgumentSyntax
//@[40:0045) |   | └─StringSyntax
//@[40:0045) |   |   └─Token(StringComplete) |'bar'|
//@[45:0046) |   └─Token(RightParen) |)|
//@[46:0048) ├─Token(NewLine) |\r\n|
param baz = externalInput('custom.binding', '__BINDING__')
//@[00:0058) ├─ParameterAssignmentSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0009) | ├─IdentifierSyntax
//@[06:0009) | | └─Token(Identifier) |baz|
//@[10:0011) | ├─Token(Assignment) |=|
//@[12:0058) | └─FunctionCallSyntax
//@[12:0025) |   ├─IdentifierSyntax
//@[12:0025) |   | └─Token(Identifier) |externalInput|
//@[25:0026) |   ├─Token(LeftParen) |(|
//@[26:0042) |   ├─FunctionArgumentSyntax
//@[26:0042) |   | └─StringSyntax
//@[26:0042) |   |   └─Token(StringComplete) |'custom.binding'|
//@[42:0043) |   ├─Token(Comma) |,|
//@[44:0057) |   ├─FunctionArgumentSyntax
//@[44:0057) |   | └─StringSyntax
//@[44:0057) |   |   └─Token(StringComplete) |'__BINDING__'|
//@[57:0058) |   └─Token(RightParen) |)|
//@[58:0062) ├─Token(NewLine) |\r\n\r\n|

// single param with variable reference
//@[39:0041) ├─Token(NewLine) |\r\n|
var myVar = bool(externalInput('sys.cli', 'myVar'))
//@[00:0051) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0009) | ├─IdentifierSyntax
//@[04:0009) | | └─Token(Identifier) |myVar|
//@[10:0011) | ├─Token(Assignment) |=|
//@[12:0051) | └─FunctionCallSyntax
//@[12:0016) |   ├─IdentifierSyntax
//@[12:0016) |   | └─Token(Identifier) |bool|
//@[16:0017) |   ├─Token(LeftParen) |(|
//@[17:0050) |   ├─FunctionArgumentSyntax
//@[17:0050) |   | └─FunctionCallSyntax
//@[17:0030) |   |   ├─IdentifierSyntax
//@[17:0030) |   |   | └─Token(Identifier) |externalInput|
//@[30:0031) |   |   ├─Token(LeftParen) |(|
//@[31:0040) |   |   ├─FunctionArgumentSyntax
//@[31:0040) |   |   | └─StringSyntax
//@[31:0040) |   |   |   └─Token(StringComplete) |'sys.cli'|
//@[40:0041) |   |   ├─Token(Comma) |,|
//@[42:0049) |   |   ├─FunctionArgumentSyntax
//@[42:0049) |   |   | └─StringSyntax
//@[42:0049) |   |   |   └─Token(StringComplete) |'myVar'|
//@[49:0050) |   |   └─Token(RightParen) |)|
//@[50:0051) |   └─Token(RightParen) |)|
//@[51:0053) ├─Token(NewLine) |\r\n|
param varRef = myVar
//@[00:0020) ├─ParameterAssignmentSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0012) | ├─IdentifierSyntax
//@[06:0012) | | └─Token(Identifier) |varRef|
//@[13:0014) | ├─Token(Assignment) |=|
//@[15:0020) | └─VariableAccessSyntax
//@[15:0020) |   └─IdentifierSyntax
//@[15:0020) |     └─Token(Identifier) |myVar|
//@[20:0024) ├─Token(NewLine) |\r\n\r\n|

// object config
//@[16:0018) ├─Token(NewLine) |\r\n|
param objectConfig = externalInput('custom.tool', {
//@[00:0098) ├─ParameterAssignmentSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0018) | ├─IdentifierSyntax
//@[06:0018) | | └─Token(Identifier) |objectConfig|
//@[19:0020) | ├─Token(Assignment) |=|
//@[21:0098) | └─FunctionCallSyntax
//@[21:0034) |   ├─IdentifierSyntax
//@[21:0034) |   | └─Token(Identifier) |externalInput|
//@[34:0035) |   ├─Token(LeftParen) |(|
//@[35:0048) |   ├─FunctionArgumentSyntax
//@[35:0048) |   | └─StringSyntax
//@[35:0048) |   |   └─Token(StringComplete) |'custom.tool'|
//@[48:0049) |   ├─Token(Comma) |,|
//@[50:0097) |   ├─FunctionArgumentSyntax
//@[50:0097) |   | └─ObjectSyntax
//@[50:0051) |   |   ├─Token(LeftBrace) |{|
//@[51:0053) |   |   ├─Token(NewLine) |\r\n|
  path: '/path/to/file'
//@[02:0023) |   |   ├─ObjectPropertySyntax
//@[02:0006) |   |   | ├─IdentifierSyntax
//@[02:0006) |   |   | | └─Token(Identifier) |path|
//@[06:0007) |   |   | ├─Token(Colon) |:|
//@[08:0023) |   |   | └─StringSyntax
//@[08:0023) |   |   |   └─Token(StringComplete) |'/path/to/file'|
//@[23:0025) |   |   ├─Token(NewLine) |\r\n|
  isSecure: true
//@[02:0016) |   |   ├─ObjectPropertySyntax
//@[02:0010) |   |   | ├─IdentifierSyntax
//@[02:0010) |   |   | | └─Token(Identifier) |isSecure|
//@[10:0011) |   |   | ├─Token(Colon) |:|
//@[12:0016) |   |   | └─BooleanLiteralSyntax
//@[12:0016) |   |   |   └─Token(TrueKeyword) |true|
//@[16:0018) |   |   ├─Token(NewLine) |\r\n|
})
//@[00:0001) |   |   └─Token(RightBrace) |}|
//@[01:0002) |   └─Token(RightParen) |)|
//@[02:0006) ├─Token(NewLine) |\r\n\r\n|

// variable reference chain
//@[27:0029) ├─Token(NewLine) |\r\n|
var a = externalInput('sys.cli', 'a')
//@[00:0037) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0005) | ├─IdentifierSyntax
//@[04:0005) | | └─Token(Identifier) |a|
//@[06:0007) | ├─Token(Assignment) |=|
//@[08:0037) | └─FunctionCallSyntax
//@[08:0021) |   ├─IdentifierSyntax
//@[08:0021) |   | └─Token(Identifier) |externalInput|
//@[21:0022) |   ├─Token(LeftParen) |(|
//@[22:0031) |   ├─FunctionArgumentSyntax
//@[22:0031) |   | └─StringSyntax
//@[22:0031) |   |   └─Token(StringComplete) |'sys.cli'|
//@[31:0032) |   ├─Token(Comma) |,|
//@[33:0036) |   ├─FunctionArgumentSyntax
//@[33:0036) |   | └─StringSyntax
//@[33:0036) |   |   └─Token(StringComplete) |'a'|
//@[36:0037) |   └─Token(RightParen) |)|
//@[37:0039) ├─Token(NewLine) |\r\n|
var b = a
//@[00:0009) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0005) | ├─IdentifierSyntax
//@[04:0005) | | └─Token(Identifier) |b|
//@[06:0007) | ├─Token(Assignment) |=|
//@[08:0009) | └─VariableAccessSyntax
//@[08:0009) |   └─IdentifierSyntax
//@[08:0009) |     └─Token(Identifier) |a|
//@[09:0011) ├─Token(NewLine) |\r\n|
param c = b
//@[00:0011) ├─ParameterAssignmentSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0007) | ├─IdentifierSyntax
//@[06:0007) | | └─Token(Identifier) |c|
//@[08:0009) | ├─Token(Assignment) |=|
//@[10:0011) | └─VariableAccessSyntax
//@[10:0011) |   └─IdentifierSyntax
//@[10:0011) |     └─Token(Identifier) |b|
//@[11:0015) ├─Token(NewLine) |\r\n\r\n|

// param reference chain
//@[24:0026) ├─Token(NewLine) |\r\n|
param a1 = externalInput('sys.cli', 'a')
//@[00:0040) ├─ParameterAssignmentSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0008) | ├─IdentifierSyntax
//@[06:0008) | | └─Token(Identifier) |a1|
//@[09:0010) | ├─Token(Assignment) |=|
//@[11:0040) | └─FunctionCallSyntax
//@[11:0024) |   ├─IdentifierSyntax
//@[11:0024) |   | └─Token(Identifier) |externalInput|
//@[24:0025) |   ├─Token(LeftParen) |(|
//@[25:0034) |   ├─FunctionArgumentSyntax
//@[25:0034) |   | └─StringSyntax
//@[25:0034) |   |   └─Token(StringComplete) |'sys.cli'|
//@[34:0035) |   ├─Token(Comma) |,|
//@[36:0039) |   ├─FunctionArgumentSyntax
//@[36:0039) |   | └─StringSyntax
//@[36:0039) |   |   └─Token(StringComplete) |'a'|
//@[39:0040) |   └─Token(RightParen) |)|
//@[40:0042) ├─Token(NewLine) |\r\n|
param b1 = a1
//@[00:0013) ├─ParameterAssignmentSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0008) | ├─IdentifierSyntax
//@[06:0008) | | └─Token(Identifier) |b1|
//@[09:0010) | ├─Token(Assignment) |=|
//@[11:0013) | └─VariableAccessSyntax
//@[11:0013) |   └─IdentifierSyntax
//@[11:0013) |     └─Token(Identifier) |a1|
//@[13:0015) ├─Token(NewLine) |\r\n|
param c1 = b1
//@[00:0013) ├─ParameterAssignmentSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0008) | ├─IdentifierSyntax
//@[06:0008) | | └─Token(Identifier) |c1|
//@[09:0010) | ├─Token(Assignment) |=|
//@[11:0013) | └─VariableAccessSyntax
//@[11:0013) |   └─IdentifierSyntax
//@[11:0013) |     └─Token(Identifier) |b1|
//@[13:0017) ├─Token(NewLine) |\r\n\r\n|

// string interpolation
//@[23:0025) ├─Token(NewLine) |\r\n|
param first = int(externalInput('custom.binding', '__BINDING__'))
//@[00:0065) ├─ParameterAssignmentSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0011) | ├─IdentifierSyntax
//@[06:0011) | | └─Token(Identifier) |first|
//@[12:0013) | ├─Token(Assignment) |=|
//@[14:0065) | └─FunctionCallSyntax
//@[14:0017) |   ├─IdentifierSyntax
//@[14:0017) |   | └─Token(Identifier) |int|
//@[17:0018) |   ├─Token(LeftParen) |(|
//@[18:0064) |   ├─FunctionArgumentSyntax
//@[18:0064) |   | └─FunctionCallSyntax
//@[18:0031) |   |   ├─IdentifierSyntax
//@[18:0031) |   |   | └─Token(Identifier) |externalInput|
//@[31:0032) |   |   ├─Token(LeftParen) |(|
//@[32:0048) |   |   ├─FunctionArgumentSyntax
//@[32:0048) |   |   | └─StringSyntax
//@[32:0048) |   |   |   └─Token(StringComplete) |'custom.binding'|
//@[48:0049) |   |   ├─Token(Comma) |,|
//@[50:0063) |   |   ├─FunctionArgumentSyntax
//@[50:0063) |   |   | └─StringSyntax
//@[50:0063) |   |   |   └─Token(StringComplete) |'__BINDING__'|
//@[63:0064) |   |   └─Token(RightParen) |)|
//@[64:0065) |   └─Token(RightParen) |)|
//@[65:0067) ├─Token(NewLine) |\r\n|
param second = externalInput('custom.binding', {
//@[00:0099) ├─ParameterAssignmentSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0012) | ├─IdentifierSyntax
//@[06:0012) | | └─Token(Identifier) |second|
//@[13:0014) | ├─Token(Assignment) |=|
//@[15:0099) | └─FunctionCallSyntax
//@[15:0028) |   ├─IdentifierSyntax
//@[15:0028) |   | └─Token(Identifier) |externalInput|
//@[28:0029) |   ├─Token(LeftParen) |(|
//@[29:0045) |   ├─FunctionArgumentSyntax
//@[29:0045) |   | └─StringSyntax
//@[29:0045) |   |   └─Token(StringComplete) |'custom.binding'|
//@[45:0046) |   ├─Token(Comma) |,|
//@[47:0098) |   ├─FunctionArgumentSyntax
//@[47:0098) |   | └─ObjectSyntax
//@[47:0048) |   |   ├─Token(LeftBrace) |{|
//@[48:0050) |   |   ├─Token(NewLine) |\r\n|
    path: '/path/to/file'
//@[04:0025) |   |   ├─ObjectPropertySyntax
//@[04:0008) |   |   | ├─IdentifierSyntax
//@[04:0008) |   |   | | └─Token(Identifier) |path|
//@[08:0009) |   |   | ├─Token(Colon) |:|
//@[10:0025) |   |   | └─StringSyntax
//@[10:0025) |   |   |   └─Token(StringComplete) |'/path/to/file'|
//@[25:0027) |   |   ├─Token(NewLine) |\r\n|
    isSecure: true
//@[04:0018) |   |   ├─ObjectPropertySyntax
//@[04:0012) |   |   | ├─IdentifierSyntax
//@[04:0012) |   |   | | └─Token(Identifier) |isSecure|
//@[12:0013) |   |   | ├─Token(Colon) |:|
//@[14:0018) |   |   | └─BooleanLiteralSyntax
//@[14:0018) |   |   |   └─Token(TrueKeyword) |true|
//@[18:0020) |   |   ├─Token(NewLine) |\r\n|
})
//@[00:0001) |   |   └─Token(RightBrace) |}|
//@[01:0002) |   └─Token(RightParen) |)|
//@[02:0004) ├─Token(NewLine) |\r\n|
param result = '${first} combined with ${second}'
//@[00:0049) ├─ParameterAssignmentSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0012) | ├─IdentifierSyntax
//@[06:0012) | | └─Token(Identifier) |result|
//@[13:0014) | ├─Token(Assignment) |=|
//@[15:0049) | └─StringSyntax
//@[15:0018) |   ├─Token(StringLeftPiece) |'${|
//@[18:0023) |   ├─VariableAccessSyntax
//@[18:0023) |   | └─IdentifierSyntax
//@[18:0023) |   |   └─Token(Identifier) |first|
//@[23:0041) |   ├─Token(StringMiddlePiece) |} combined with ${|
//@[41:0047) |   ├─VariableAccessSyntax
//@[41:0047) |   | └─IdentifierSyntax
//@[41:0047) |   |   └─Token(Identifier) |second|
//@[47:0049) |   └─Token(StringRightPiece) |}'|
//@[49:0053) ├─Token(NewLine) |\r\n\r\n|

// instance function call
//@[25:0027) ├─Token(NewLine) |\r\n|
param myParam = sys.externalInput('sys.cli', 'myParam')
//@[00:0055) ├─ParameterAssignmentSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0013) | ├─IdentifierSyntax
//@[06:0013) | | └─Token(Identifier) |myParam|
//@[14:0015) | ├─Token(Assignment) |=|
//@[16:0055) | └─InstanceFunctionCallSyntax
//@[16:0019) |   ├─VariableAccessSyntax
//@[16:0019) |   | └─IdentifierSyntax
//@[16:0019) |   |   └─Token(Identifier) |sys|
//@[19:0020) |   ├─Token(Dot) |.|
//@[20:0033) |   ├─IdentifierSyntax
//@[20:0033) |   | └─Token(Identifier) |externalInput|
//@[33:0034) |   ├─Token(LeftParen) |(|
//@[34:0043) |   ├─FunctionArgumentSyntax
//@[34:0043) |   | └─StringSyntax
//@[34:0043) |   |   └─Token(StringComplete) |'sys.cli'|
//@[43:0044) |   ├─Token(Comma) |,|
//@[45:0054) |   ├─FunctionArgumentSyntax
//@[45:0054) |   | └─StringSyntax
//@[45:0054) |   |   └─Token(StringComplete) |'myParam'|
//@[54:0055) |   └─Token(RightParen) |)|
//@[55:0059) ├─Token(NewLine) |\r\n\r\n|

// check sanitized externaInputDefinition
//@[41:0043) ├─Token(NewLine) |\r\n|
param coolParam = externalInput('sys&sons.cool#param provider')
//@[00:0063) ├─ParameterAssignmentSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0015) | ├─IdentifierSyntax
//@[06:0015) | | └─Token(Identifier) |coolParam|
//@[16:0017) | ├─Token(Assignment) |=|
//@[18:0063) | └─FunctionCallSyntax
//@[18:0031) |   ├─IdentifierSyntax
//@[18:0031) |   | └─Token(Identifier) |externalInput|
//@[31:0032) |   ├─Token(LeftParen) |(|
//@[32:0062) |   ├─FunctionArgumentSyntax
//@[32:0062) |   | └─StringSyntax
//@[32:0062) |   |   └─Token(StringComplete) |'sys&sons.cool#param provider'|
//@[62:0063) |   └─Token(RightParen) |)|
//@[63:0067) ├─Token(NewLine) |\r\n\r\n|

param objectBody = {
//@[00:0132) ├─ParameterAssignmentSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0016) | ├─IdentifierSyntax
//@[06:0016) | | └─Token(Identifier) |objectBody|
//@[17:0018) | ├─Token(Assignment) |=|
//@[19:0132) | └─ObjectSyntax
//@[19:0020) |   ├─Token(LeftBrace) |{|
//@[20:0022) |   ├─Token(NewLine) |\r\n|
  foo: externalInput('custom.binding', 'foo')
//@[02:0045) |   ├─ObjectPropertySyntax
//@[02:0005) |   | ├─IdentifierSyntax
//@[02:0005) |   | | └─Token(Identifier) |foo|
//@[05:0006) |   | ├─Token(Colon) |:|
//@[07:0045) |   | └─FunctionCallSyntax
//@[07:0020) |   |   ├─IdentifierSyntax
//@[07:0020) |   |   | └─Token(Identifier) |externalInput|
//@[20:0021) |   |   ├─Token(LeftParen) |(|
//@[21:0037) |   |   ├─FunctionArgumentSyntax
//@[21:0037) |   |   | └─StringSyntax
//@[21:0037) |   |   |   └─Token(StringComplete) |'custom.binding'|
//@[37:0038) |   |   ├─Token(Comma) |,|
//@[39:0044) |   |   ├─FunctionArgumentSyntax
//@[39:0044) |   |   | └─StringSyntax
//@[39:0044) |   |   |   └─Token(StringComplete) |'foo'|
//@[44:0045) |   |   └─Token(RightParen) |)|
//@[45:0047) |   ├─Token(NewLine) |\r\n|
  bar: externalInput('custom.binding', 'bar')
//@[02:0045) |   ├─ObjectPropertySyntax
//@[02:0005) |   | ├─IdentifierSyntax
//@[02:0005) |   | | └─Token(Identifier) |bar|
//@[05:0006) |   | ├─Token(Colon) |:|
//@[07:0045) |   | └─FunctionCallSyntax
//@[07:0020) |   |   ├─IdentifierSyntax
//@[07:0020) |   |   | └─Token(Identifier) |externalInput|
//@[20:0021) |   |   ├─Token(LeftParen) |(|
//@[21:0037) |   |   ├─FunctionArgumentSyntax
//@[21:0037) |   |   | └─StringSyntax
//@[21:0037) |   |   |   └─Token(StringComplete) |'custom.binding'|
//@[37:0038) |   |   ├─Token(Comma) |,|
//@[39:0044) |   |   ├─FunctionArgumentSyntax
//@[39:0044) |   |   | └─StringSyntax
//@[39:0044) |   |   |   └─Token(StringComplete) |'bar'|
//@[44:0045) |   |   └─Token(RightParen) |)|
//@[45:0047) |   ├─Token(NewLine) |\r\n|
  baz: 'blah'
//@[02:0013) |   ├─ObjectPropertySyntax
//@[02:0005) |   | ├─IdentifierSyntax
//@[02:0005) |   | | └─Token(Identifier) |baz|
//@[05:0006) |   | ├─Token(Colon) |:|
//@[07:0013) |   | └─StringSyntax
//@[07:0013) |   |   └─Token(StringComplete) |'blah'|
//@[13:0015) |   ├─Token(NewLine) |\r\n|
}
//@[00:0001) |   └─Token(RightBrace) |}|
//@[01:0005) ├─Token(NewLine) |\r\n\r\n|

var poodle = 'toy'
//@[00:0018) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0010) | ├─IdentifierSyntax
//@[04:0010) | | └─Token(Identifier) |poodle|
//@[11:0012) | ├─Token(Assignment) |=|
//@[13:0018) | └─StringSyntax
//@[13:0018) |   └─Token(StringComplete) |'toy'|
//@[18:0020) ├─Token(NewLine) |\r\n|
param retriever = 'golden'
//@[00:0026) ├─ParameterAssignmentSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0015) | ├─IdentifierSyntax
//@[06:0015) | | └─Token(Identifier) |retriever|
//@[16:0017) | ├─Token(Assignment) |=|
//@[18:0026) | └─StringSyntax
//@[18:0026) |   └─Token(StringComplete) |'golden'|
//@[26:0028) ├─Token(NewLine) |\r\n|
param concat = '${poodle}-${retriever}-${externalInput('sys.cli', 'foo')}'
//@[00:0074) ├─ParameterAssignmentSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0012) | ├─IdentifierSyntax
//@[06:0012) | | └─Token(Identifier) |concat|
//@[13:0014) | ├─Token(Assignment) |=|
//@[15:0074) | └─StringSyntax
//@[15:0018) |   ├─Token(StringLeftPiece) |'${|
//@[18:0024) |   ├─VariableAccessSyntax
//@[18:0024) |   | └─IdentifierSyntax
//@[18:0024) |   |   └─Token(Identifier) |poodle|
//@[24:0028) |   ├─Token(StringMiddlePiece) |}-${|
//@[28:0037) |   ├─VariableAccessSyntax
//@[28:0037) |   | └─IdentifierSyntax
//@[28:0037) |   |   └─Token(Identifier) |retriever|
//@[37:0041) |   ├─Token(StringMiddlePiece) |}-${|
//@[41:0072) |   ├─FunctionCallSyntax
//@[41:0054) |   | ├─IdentifierSyntax
//@[41:0054) |   | | └─Token(Identifier) |externalInput|
//@[54:0055) |   | ├─Token(LeftParen) |(|
//@[55:0064) |   | ├─FunctionArgumentSyntax
//@[55:0064) |   | | └─StringSyntax
//@[55:0064) |   | |   └─Token(StringComplete) |'sys.cli'|
//@[64:0065) |   | ├─Token(Comma) |,|
//@[66:0071) |   | ├─FunctionArgumentSyntax
//@[66:0071) |   | | └─StringSyntax
//@[66:0071) |   | |   └─Token(StringComplete) |'foo'|
//@[71:0072) |   | └─Token(RightParen) |)|
//@[72:0074) |   └─Token(StringRightPiece) |}'|
//@[74:0078) ├─Token(NewLine) |\r\n\r\n|

import * as main2 from 'main2.bicep'
//@[00:0036) ├─CompileTimeImportDeclarationSyntax
//@[00:0006) | ├─Token(Identifier) |import|
//@[07:0017) | ├─WildcardImportSyntax
//@[07:0008) | | ├─Token(Asterisk) |*|
//@[09:0017) | | └─AliasAsClauseSyntax
//@[09:0011) | |   ├─Token(Identifier) |as|
//@[12:0017) | |   └─IdentifierSyntax
//@[12:0017) | |     └─Token(Identifier) |main2|
//@[18:0036) | └─CompileTimeImportFromClauseSyntax
//@[18:0022) |   ├─Token(Identifier) |from|
//@[23:0036) |   └─StringSyntax
//@[23:0036) |     └─Token(StringComplete) |'main2.bicep'|
//@[36:0038) ├─Token(NewLine) |\r\n|
import { person, getPerson, getDefaultPerson } from 'main.bicep'
//@[00:0064) ├─CompileTimeImportDeclarationSyntax
//@[00:0006) | ├─Token(Identifier) |import|
//@[07:0046) | ├─ImportedSymbolsListSyntax
//@[07:0008) | | ├─Token(LeftBrace) |{|
//@[09:0015) | | ├─ImportedSymbolsListItemSyntax
//@[09:0015) | | | └─IdentifierSyntax
//@[09:0015) | | |   └─Token(Identifier) |person|
//@[15:0016) | | ├─Token(Comma) |,|
//@[17:0026) | | ├─ImportedSymbolsListItemSyntax
//@[17:0026) | | | └─IdentifierSyntax
//@[17:0026) | | |   └─Token(Identifier) |getPerson|
//@[26:0027) | | ├─Token(Comma) |,|
//@[28:0044) | | ├─ImportedSymbolsListItemSyntax
//@[28:0044) | | | └─IdentifierSyntax
//@[28:0044) | | |   └─Token(Identifier) |getDefaultPerson|
//@[45:0046) | | └─Token(RightBrace) |}|
//@[47:0064) | └─CompileTimeImportFromClauseSyntax
//@[47:0051) |   ├─Token(Identifier) |from|
//@[52:0064) |   └─StringSyntax
//@[52:0064) |     └─Token(StringComplete) |'main.bicep'|
//@[64:0068) ├─Token(NewLine) |\r\n\r\n|

param principalIds = externalInput('sys.cli', 'principalIds')
//@[00:0061) ├─ParameterAssignmentSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0018) | ├─IdentifierSyntax
//@[06:0018) | | └─Token(Identifier) |principalIds|
//@[19:0020) | ├─Token(Assignment) |=|
//@[21:0061) | └─FunctionCallSyntax
//@[21:0034) |   ├─IdentifierSyntax
//@[21:0034) |   | └─Token(Identifier) |externalInput|
//@[34:0035) |   ├─Token(LeftParen) |(|
//@[35:0044) |   ├─FunctionArgumentSyntax
//@[35:0044) |   | └─StringSyntax
//@[35:0044) |   |   └─Token(StringComplete) |'sys.cli'|
//@[44:0045) |   ├─Token(Comma) |,|
//@[46:0060) |   ├─FunctionArgumentSyntax
//@[46:0060) |   | └─StringSyntax
//@[46:0060) |   |   └─Token(StringComplete) |'principalIds'|
//@[60:0061) |   └─Token(RightParen) |)|
//@[61:0065) ├─Token(NewLine) |\r\n\r\n|

var anotherPerson = {
//@[00:0051) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0017) | ├─IdentifierSyntax
//@[04:0017) | | └─Token(Identifier) |anotherPerson|
//@[18:0019) | ├─Token(Assignment) |=|
//@[20:0051) | └─ObjectSyntax
//@[20:0021) |   ├─Token(LeftBrace) |{|
//@[21:0023) |   ├─Token(NewLine) |\r\n|
  name: 'John'
//@[02:0014) |   ├─ObjectPropertySyntax
//@[02:0006) |   | ├─IdentifierSyntax
//@[02:0006) |   | | └─Token(Identifier) |name|
//@[06:0007) |   | ├─Token(Colon) |:|
//@[08:0014) |   | └─StringSyntax
//@[08:0014) |   |   └─Token(StringComplete) |'John'|
//@[14:0016) |   ├─Token(NewLine) |\r\n|
  age: 21
//@[02:0009) |   ├─ObjectPropertySyntax
//@[02:0005) |   | ├─IdentifierSyntax
//@[02:0005) |   | | └─Token(Identifier) |age|
//@[05:0006) |   | ├─Token(Colon) |:|
//@[07:0009) |   | └─IntegerLiteralSyntax
//@[07:0009) |   |   └─Token(Integer) |21|
//@[09:0011) |   ├─Token(NewLine) |\r\n|
}
//@[00:0001) |   └─Token(RightBrace) |}|
//@[01:0003) ├─Token(NewLine) |\r\n|
param varPeople = [
//@[00:0193) ├─ParameterAssignmentSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0015) | ├─IdentifierSyntax
//@[06:0015) | | └─Token(Identifier) |varPeople|
//@[16:0017) | ├─Token(Assignment) |=|
//@[18:0193) | └─ArraySyntax
//@[18:0019) |   ├─Token(LeftSquare) |[|
//@[19:0021) |   ├─Token(NewLine) |\r\n|
  ...map(principalIds, id => {
//@[02:0054) |   ├─SpreadExpressionSyntax
//@[02:0005) |   | ├─Token(Ellipsis) |...|
//@[05:0054) |   | └─FunctionCallSyntax
//@[05:0008) |   |   ├─IdentifierSyntax
//@[05:0008) |   |   | └─Token(Identifier) |map|
//@[08:0009) |   |   ├─Token(LeftParen) |(|
//@[09:0021) |   |   ├─FunctionArgumentSyntax
//@[09:0021) |   |   | └─VariableAccessSyntax
//@[09:0021) |   |   |   └─IdentifierSyntax
//@[09:0021) |   |   |     └─Token(Identifier) |principalIds|
//@[21:0022) |   |   ├─Token(Comma) |,|
//@[23:0053) |   |   ├─FunctionArgumentSyntax
//@[23:0053) |   |   | └─LambdaSyntax
//@[23:0025) |   |   |   ├─LocalVariableSyntax
//@[23:0025) |   |   |   | └─IdentifierSyntax
//@[23:0025) |   |   |   |   └─Token(Identifier) |id|
//@[26:0028) |   |   |   ├─Token(Arrow) |=>|
//@[29:0053) |   |   |   └─ObjectSyntax
//@[29:0030) |   |   |     ├─Token(LeftBrace) |{|
//@[30:0032) |   |   |     ├─Token(NewLine) |\r\n|
    objectId: id
//@[04:0016) |   |   |     ├─ObjectPropertySyntax
//@[04:0012) |   |   |     | ├─IdentifierSyntax
//@[04:0012) |   |   |     | | └─Token(Identifier) |objectId|
//@[12:0013) |   |   |     | ├─Token(Colon) |:|
//@[14:0016) |   |   |     | └─VariableAccessSyntax
//@[14:0016) |   |   |     |   └─IdentifierSyntax
//@[14:0016) |   |   |     |     └─Token(Identifier) |id|
//@[16:0018) |   |   |     ├─Token(NewLine) |\r\n|
  })
//@[02:0003) |   |   |     └─Token(RightBrace) |}|
//@[03:0004) |   |   └─Token(RightParen) |)|
//@[04:0006) |   ├─Token(NewLine) |\r\n|
  person
//@[02:0008) |   ├─ArrayItemSyntax
//@[02:0008) |   | └─VariableAccessSyntax
//@[02:0008) |   |   └─IdentifierSyntax
//@[02:0008) |   |     └─Token(Identifier) |person|
//@[08:0010) |   ├─Token(NewLine) |\r\n|
  anotherPerson
//@[02:0015) |   ├─ArrayItemSyntax
//@[02:0015) |   | └─VariableAccessSyntax
//@[02:0015) |   |   └─IdentifierSyntax
//@[02:0015) |   |     └─Token(Identifier) |anotherPerson|
//@[15:0017) |   ├─Token(NewLine) |\r\n|
  getPerson('Bob', 30)
//@[02:0022) |   ├─ArrayItemSyntax
//@[02:0022) |   | └─FunctionCallSyntax
//@[02:0011) |   |   ├─IdentifierSyntax
//@[02:0011) |   |   | └─Token(Identifier) |getPerson|
//@[11:0012) |   |   ├─Token(LeftParen) |(|
//@[12:0017) |   |   ├─FunctionArgumentSyntax
//@[12:0017) |   |   | └─StringSyntax
//@[12:0017) |   |   |   └─Token(StringComplete) |'Bob'|
//@[17:0018) |   |   ├─Token(Comma) |,|
//@[19:0021) |   |   ├─FunctionArgumentSyntax
//@[19:0021) |   |   | └─IntegerLiteralSyntax
//@[19:0021) |   |   |   └─Token(Integer) |30|
//@[21:0022) |   |   └─Token(RightParen) |)|
//@[22:0024) |   ├─Token(NewLine) |\r\n|
  getDefaultPerson()
//@[02:0020) |   ├─ArrayItemSyntax
//@[02:0020) |   | └─FunctionCallSyntax
//@[02:0018) |   |   ├─IdentifierSyntax
//@[02:0018) |   |   | └─Token(Identifier) |getDefaultPerson|
//@[18:0019) |   |   ├─Token(LeftParen) |(|
//@[19:0020) |   |   └─Token(RightParen) |)|
//@[20:0022) |   ├─Token(NewLine) |\r\n|
  externalInput('custom.binding', 'foo')
//@[02:0040) |   ├─ArrayItemSyntax
//@[02:0040) |   | └─FunctionCallSyntax
//@[02:0015) |   |   ├─IdentifierSyntax
//@[02:0015) |   |   | └─Token(Identifier) |externalInput|
//@[15:0016) |   |   ├─Token(LeftParen) |(|
//@[16:0032) |   |   ├─FunctionArgumentSyntax
//@[16:0032) |   |   | └─StringSyntax
//@[16:0032) |   |   |   └─Token(StringComplete) |'custom.binding'|
//@[32:0033) |   |   ├─Token(Comma) |,|
//@[34:0039) |   |   ├─FunctionArgumentSyntax
//@[34:0039) |   |   | └─StringSyntax
//@[34:0039) |   |   |   └─Token(StringComplete) |'foo'|
//@[39:0040) |   |   └─Token(RightParen) |)|
//@[40:0042) |   ├─Token(NewLine) |\r\n|
]
//@[00:0001) |   └─Token(RightSquare) |]|
//@[01:0005) ├─Token(NewLine) |\r\n\r\n|

var infra main2.InfraConfig = {
//@[00:0135) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0009) | ├─IdentifierSyntax
//@[04:0009) | | └─Token(Identifier) |infra|
//@[10:0027) | ├─TypePropertyAccessSyntax
//@[10:0015) | | ├─TypeVariableAccessSyntax
//@[10:0015) | | | └─IdentifierSyntax
//@[10:0015) | | |   └─Token(Identifier) |main2|
//@[15:0016) | | ├─Token(Dot) |.|
//@[16:0027) | | └─IdentifierSyntax
//@[16:0027) | |   └─Token(Identifier) |InfraConfig|
//@[28:0029) | ├─Token(Assignment) |=|
//@[30:0135) | └─ObjectSyntax
//@[30:0031) |   ├─Token(LeftBrace) |{|
//@[31:0033) |   ├─Token(NewLine) |\r\n|
  storage: main2.storageConfig
//@[02:0030) |   ├─ObjectPropertySyntax
//@[02:0009) |   | ├─IdentifierSyntax
//@[02:0009) |   | | └─Token(Identifier) |storage|
//@[09:0010) |   | ├─Token(Colon) |:|
//@[11:0030) |   | └─PropertyAccessSyntax
//@[11:0016) |   |   ├─VariableAccessSyntax
//@[11:0016) |   |   | └─IdentifierSyntax
//@[11:0016) |   |   |   └─Token(Identifier) |main2|
//@[16:0017) |   |   ├─Token(Dot) |.|
//@[17:0030) |   |   └─IdentifierSyntax
//@[17:0030) |   |     └─Token(Identifier) |storageConfig|
//@[30:0032) |   ├─Token(NewLine) |\r\n|
  vm: main2.vmConfig
//@[02:0020) |   ├─ObjectPropertySyntax
//@[02:0004) |   | ├─IdentifierSyntax
//@[02:0004) |   | | └─Token(Identifier) |vm|
//@[04:0005) |   | ├─Token(Colon) |:|
//@[06:0020) |   | └─PropertyAccessSyntax
//@[06:0011) |   |   ├─VariableAccessSyntax
//@[06:0011) |   |   | └─IdentifierSyntax
//@[06:0011) |   |   |   └─Token(Identifier) |main2|
//@[11:0012) |   |   ├─Token(Dot) |.|
//@[12:0020) |   |   └─IdentifierSyntax
//@[12:0020) |   |     └─Token(Identifier) |vmConfig|
//@[20:0022) |   ├─Token(NewLine) |\r\n|
  tag: externalInput('custom.binding', 'bar')
//@[02:0045) |   ├─ObjectPropertySyntax
//@[02:0005) |   | ├─IdentifierSyntax
//@[02:0005) |   | | └─Token(Identifier) |tag|
//@[05:0006) |   | ├─Token(Colon) |:|
//@[07:0045) |   | └─FunctionCallSyntax
//@[07:0020) |   |   ├─IdentifierSyntax
//@[07:0020) |   |   | └─Token(Identifier) |externalInput|
//@[20:0021) |   |   ├─Token(LeftParen) |(|
//@[21:0037) |   |   ├─FunctionArgumentSyntax
//@[21:0037) |   |   | └─StringSyntax
//@[21:0037) |   |   |   └─Token(StringComplete) |'custom.binding'|
//@[37:0038) |   |   ├─Token(Comma) |,|
//@[39:0044) |   |   ├─FunctionArgumentSyntax
//@[39:0044) |   |   | └─StringSyntax
//@[39:0044) |   |   |   └─Token(StringComplete) |'bar'|
//@[44:0045) |   |   └─Token(RightParen) |)|
//@[45:0047) |   ├─Token(NewLine) |\r\n|
}
//@[00:0001) |   └─Token(RightBrace) |}|
//@[01:0005) ├─Token(NewLine) |\r\n\r\n|

param infraParam = infra
//@[00:0024) ├─ParameterAssignmentSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0016) | ├─IdentifierSyntax
//@[06:0016) | | └─Token(Identifier) |infraParam|
//@[17:0018) | ├─Token(Assignment) |=|
//@[19:0024) | └─VariableAccessSyntax
//@[19:0024) |   └─IdentifierSyntax
//@[19:0024) |     └─Token(Identifier) |infra|
//@[24:0026) ├─Token(NewLine) |\r\n|

//@[00:0000) └─Token(EndOfFile) ||
