using none
//@[00:1035) ProgramSyntax
//@[00:0010) ├─UsingDeclarationSyntax
//@[00:0005) | ├─Token(Identifier) |using|
//@[06:0010) | ├─NoneLiteralSyntax
//@[06:0010) | | └─Token(Identifier) |none|
//@[10:0010) | └─SkippedTriviaSyntax
//@[10:0014) ├─Token(NewLine) |\r\n\r\n|

// single parameter
//@[19:0021) ├─Token(NewLine) |\r\n|
param singleParam = readCliArg('foo')
//@[00:0037) ├─ParameterAssignmentSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0017) | ├─IdentifierSyntax
//@[06:0017) | | └─Token(Identifier) |singleParam|
//@[18:0019) | ├─Token(Assignment) |=|
//@[20:0037) | └─FunctionCallSyntax
//@[20:0030) |   ├─IdentifierSyntax
//@[20:0030) |   | └─Token(Identifier) |readCliArg|
//@[30:0031) |   ├─Token(LeftParen) |(|
//@[31:0036) |   ├─FunctionArgumentSyntax
//@[31:0036) |   | └─StringSyntax
//@[31:0036) |   |   └─Token(StringComplete) |'foo'|
//@[36:0037) |   └─Token(RightParen) |)|
//@[37:0041) ├─Token(NewLine) |\r\n\r\n|

// single parameter with casting expression
//@[43:0045) ├─Token(NewLine) |\r\n|
param singleParamCast = bool(readCliArg('foo'))
//@[00:0047) ├─ParameterAssignmentSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0021) | ├─IdentifierSyntax
//@[06:0021) | | └─Token(Identifier) |singleParamCast|
//@[22:0023) | ├─Token(Assignment) |=|
//@[24:0047) | └─FunctionCallSyntax
//@[24:0028) |   ├─IdentifierSyntax
//@[24:0028) |   | └─Token(Identifier) |bool|
//@[28:0029) |   ├─Token(LeftParen) |(|
//@[29:0046) |   ├─FunctionArgumentSyntax
//@[29:0046) |   | └─FunctionCallSyntax
//@[29:0039) |   |   ├─IdentifierSyntax
//@[29:0039) |   |   | └─Token(Identifier) |readCliArg|
//@[39:0040) |   |   ├─Token(LeftParen) |(|
//@[40:0045) |   |   ├─FunctionArgumentSyntax
//@[40:0045) |   |   | └─StringSyntax
//@[40:0045) |   |   |   └─Token(StringComplete) |'foo'|
//@[45:0046) |   |   └─Token(RightParen) |)|
//@[46:0047) |   └─Token(RightParen) |)|
//@[47:0051) ├─Token(NewLine) |\r\n\r\n|

// multiple parameters with different syntax
//@[44:0046) ├─Token(NewLine) |\r\n|
param foo = readCliArg('foo')
//@[00:0029) ├─ParameterAssignmentSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0009) | ├─IdentifierSyntax
//@[06:0009) | | └─Token(Identifier) |foo|
//@[10:0011) | ├─Token(Assignment) |=|
//@[12:0029) | └─FunctionCallSyntax
//@[12:0022) |   ├─IdentifierSyntax
//@[12:0022) |   | └─Token(Identifier) |readCliArg|
//@[22:0023) |   ├─Token(LeftParen) |(|
//@[23:0028) |   ├─FunctionArgumentSyntax
//@[23:0028) |   | └─StringSyntax
//@[23:0028) |   |   └─Token(StringComplete) |'foo'|
//@[28:0029) |   └─Token(RightParen) |)|
//@[29:0031) ├─Token(NewLine) |\r\n|
param bar = readEnvVar('bar')
//@[00:0029) ├─ParameterAssignmentSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0009) | ├─IdentifierSyntax
//@[06:0009) | | └─Token(Identifier) |bar|
//@[10:0011) | ├─Token(Assignment) |=|
//@[12:0029) | └─FunctionCallSyntax
//@[12:0022) |   ├─IdentifierSyntax
//@[12:0022) |   | └─Token(Identifier) |readEnvVar|
//@[22:0023) |   ├─Token(LeftParen) |(|
//@[23:0028) |   ├─FunctionArgumentSyntax
//@[23:0028) |   | └─StringSyntax
//@[23:0028) |   |   └─Token(StringComplete) |'bar'|
//@[28:0029) |   └─Token(RightParen) |)|
//@[29:0031) ├─Token(NewLine) |\r\n|
param baz = readEnvVar('FIRST_ENV_VAR')
//@[00:0039) ├─ParameterAssignmentSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0009) | ├─IdentifierSyntax
//@[06:0009) | | └─Token(Identifier) |baz|
//@[10:0011) | ├─Token(Assignment) |=|
//@[12:0039) | └─FunctionCallSyntax
//@[12:0022) |   ├─IdentifierSyntax
//@[12:0022) |   | └─Token(Identifier) |readEnvVar|
//@[22:0023) |   ├─Token(LeftParen) |(|
//@[23:0038) |   ├─FunctionArgumentSyntax
//@[23:0038) |   | └─StringSyntax
//@[23:0038) |   |   └─Token(StringComplete) |'FIRST_ENV_VAR'|
//@[38:0039) |   └─Token(RightParen) |)|
//@[39:0043) ├─Token(NewLine) |\r\n\r\n|

// single param with variable reference
//@[39:0041) ├─Token(NewLine) |\r\n|
var myVar = bool(readCliArg('myVar'))
//@[00:0037) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0009) | ├─IdentifierSyntax
//@[04:0009) | | └─Token(Identifier) |myVar|
//@[10:0011) | ├─Token(Assignment) |=|
//@[12:0037) | └─FunctionCallSyntax
//@[12:0016) |   ├─IdentifierSyntax
//@[12:0016) |   | └─Token(Identifier) |bool|
//@[16:0017) |   ├─Token(LeftParen) |(|
//@[17:0036) |   ├─FunctionArgumentSyntax
//@[17:0036) |   | └─FunctionCallSyntax
//@[17:0027) |   |   ├─IdentifierSyntax
//@[17:0027) |   |   | └─Token(Identifier) |readCliArg|
//@[27:0028) |   |   ├─Token(LeftParen) |(|
//@[28:0035) |   |   ├─FunctionArgumentSyntax
//@[28:0035) |   |   | └─StringSyntax
//@[28:0035) |   |   |   └─Token(StringComplete) |'myVar'|
//@[35:0036) |   |   └─Token(RightParen) |)|
//@[36:0037) |   └─Token(RightParen) |)|
//@[37:0039) ├─Token(NewLine) |\r\n|
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

// variable reference chain
//@[27:0029) ├─Token(NewLine) |\r\n|
var a = readCliArg('a')
//@[00:0023) ├─VariableDeclarationSyntax
//@[00:0003) | ├─Token(Identifier) |var|
//@[04:0005) | ├─IdentifierSyntax
//@[04:0005) | | └─Token(Identifier) |a|
//@[06:0007) | ├─Token(Assignment) |=|
//@[08:0023) | └─FunctionCallSyntax
//@[08:0018) |   ├─IdentifierSyntax
//@[08:0018) |   | └─Token(Identifier) |readCliArg|
//@[18:0019) |   ├─Token(LeftParen) |(|
//@[19:0022) |   ├─FunctionArgumentSyntax
//@[19:0022) |   | └─StringSyntax
//@[19:0022) |   |   └─Token(StringComplete) |'a'|
//@[22:0023) |   └─Token(RightParen) |)|
//@[23:0025) ├─Token(NewLine) |\r\n|
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
param a1 = readCliArg('a')
//@[00:0026) ├─ParameterAssignmentSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0008) | ├─IdentifierSyntax
//@[06:0008) | | └─Token(Identifier) |a1|
//@[09:0010) | ├─Token(Assignment) |=|
//@[11:0026) | └─FunctionCallSyntax
//@[11:0021) |   ├─IdentifierSyntax
//@[11:0021) |   | └─Token(Identifier) |readCliArg|
//@[21:0022) |   ├─Token(LeftParen) |(|
//@[22:0025) |   ├─FunctionArgumentSyntax
//@[22:0025) |   | └─StringSyntax
//@[22:0025) |   |   └─Token(StringComplete) |'a'|
//@[25:0026) |   └─Token(RightParen) |)|
//@[26:0028) ├─Token(NewLine) |\r\n|
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
param first = int(readEnvVar('FIRST_ENV_VAR'))
//@[00:0046) ├─ParameterAssignmentSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0011) | ├─IdentifierSyntax
//@[06:0011) | | └─Token(Identifier) |first|
//@[12:0013) | ├─Token(Assignment) |=|
//@[14:0046) | └─FunctionCallSyntax
//@[14:0017) |   ├─IdentifierSyntax
//@[14:0017) |   | └─Token(Identifier) |int|
//@[17:0018) |   ├─Token(LeftParen) |(|
//@[18:0045) |   ├─FunctionArgumentSyntax
//@[18:0045) |   | └─FunctionCallSyntax
//@[18:0028) |   |   ├─IdentifierSyntax
//@[18:0028) |   |   | └─Token(Identifier) |readEnvVar|
//@[28:0029) |   |   ├─Token(LeftParen) |(|
//@[29:0044) |   |   ├─FunctionArgumentSyntax
//@[29:0044) |   |   | └─StringSyntax
//@[29:0044) |   |   |   └─Token(StringComplete) |'FIRST_ENV_VAR'|
//@[44:0045) |   |   └─Token(RightParen) |)|
//@[45:0046) |   └─Token(RightParen) |)|
//@[46:0048) ├─Token(NewLine) |\r\n|
param second = readEnvVar('SECOND_ENV_VAR')
//@[00:0043) ├─ParameterAssignmentSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0012) | ├─IdentifierSyntax
//@[06:0012) | | └─Token(Identifier) |second|
//@[13:0014) | ├─Token(Assignment) |=|
//@[15:0043) | └─FunctionCallSyntax
//@[15:0025) |   ├─IdentifierSyntax
//@[15:0025) |   | └─Token(Identifier) |readEnvVar|
//@[25:0026) |   ├─Token(LeftParen) |(|
//@[26:0042) |   ├─FunctionArgumentSyntax
//@[26:0042) |   | └─StringSyntax
//@[26:0042) |   |   └─Token(StringComplete) |'SECOND_ENV_VAR'|
//@[42:0043) |   └─Token(RightParen) |)|
//@[43:0045) ├─Token(NewLine) |\r\n|
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
param myParam = sys.readCliArg('myParam')
//@[00:0041) ├─ParameterAssignmentSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0013) | ├─IdentifierSyntax
//@[06:0013) | | └─Token(Identifier) |myParam|
//@[14:0015) | ├─Token(Assignment) |=|
//@[16:0041) | └─InstanceFunctionCallSyntax
//@[16:0019) |   ├─VariableAccessSyntax
//@[16:0019) |   | └─IdentifierSyntax
//@[16:0019) |   |   └─Token(Identifier) |sys|
//@[19:0020) |   ├─Token(Dot) |.|
//@[20:0030) |   ├─IdentifierSyntax
//@[20:0030) |   | └─Token(Identifier) |readCliArg|
//@[30:0031) |   ├─Token(LeftParen) |(|
//@[31:0040) |   ├─FunctionArgumentSyntax
//@[31:0040) |   | └─StringSyntax
//@[31:0040) |   |   └─Token(StringComplete) |'myParam'|
//@[40:0041) |   └─Token(RightParen) |)|
//@[41:0045) ├─Token(NewLine) |\r\n\r\n|

// check sanitized externaInputDefinition
//@[41:0043) ├─Token(NewLine) |\r\n|
param coolParam = readCliArg('sys&sons.cool#param provider')
//@[00:0060) ├─ParameterAssignmentSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0015) | ├─IdentifierSyntax
//@[06:0015) | | └─Token(Identifier) |coolParam|
//@[16:0017) | ├─Token(Assignment) |=|
//@[18:0060) | └─FunctionCallSyntax
//@[18:0028) |   ├─IdentifierSyntax
//@[18:0028) |   | └─Token(Identifier) |readCliArg|
//@[28:0029) |   ├─Token(LeftParen) |(|
//@[29:0059) |   ├─FunctionArgumentSyntax
//@[29:0059) |   | └─StringSyntax
//@[29:0059) |   |   └─Token(StringComplete) |'sys&sons.cool#param provider'|
//@[59:0060) |   └─Token(RightParen) |)|
//@[60:0064) ├─Token(NewLine) |\r\n\r\n|

param objectBody = {
//@[00:0090) ├─ParameterAssignmentSyntax
//@[00:0005) | ├─Token(Identifier) |param|
//@[06:0016) | ├─IdentifierSyntax
//@[06:0016) | | └─Token(Identifier) |objectBody|
//@[17:0018) | ├─Token(Assignment) |=|
//@[19:0090) | └─ObjectSyntax
//@[19:0020) |   ├─Token(LeftBrace) |{|
//@[20:0022) |   ├─Token(NewLine) |\r\n|
  foo: readEnvVar('foo')
//@[02:0024) |   ├─ObjectPropertySyntax
//@[02:0005) |   | ├─IdentifierSyntax
//@[02:0005) |   | | └─Token(Identifier) |foo|
//@[05:0006) |   | ├─Token(Colon) |:|
//@[07:0024) |   | └─FunctionCallSyntax
//@[07:0017) |   |   ├─IdentifierSyntax
//@[07:0017) |   |   | └─Token(Identifier) |readEnvVar|
//@[17:0018) |   |   ├─Token(LeftParen) |(|
//@[18:0023) |   |   ├─FunctionArgumentSyntax
//@[18:0023) |   |   | └─StringSyntax
//@[18:0023) |   |   |   └─Token(StringComplete) |'foo'|
//@[23:0024) |   |   └─Token(RightParen) |)|
//@[24:0026) |   ├─Token(NewLine) |\r\n|
  bar: readEnvVar('bar')
//@[02:0024) |   ├─ObjectPropertySyntax
//@[02:0005) |   | ├─IdentifierSyntax
//@[02:0005) |   | | └─Token(Identifier) |bar|
//@[05:0006) |   | ├─Token(Colon) |:|
//@[07:0024) |   | └─FunctionCallSyntax
//@[07:0017) |   |   ├─IdentifierSyntax
//@[07:0017) |   |   | └─Token(Identifier) |readEnvVar|
//@[17:0018) |   |   ├─Token(LeftParen) |(|
//@[18:0023) |   |   ├─FunctionArgumentSyntax
//@[18:0023) |   |   | └─StringSyntax
//@[18:0023) |   |   |   └─Token(StringComplete) |'bar'|
//@[23:0024) |   |   └─Token(RightParen) |)|
//@[24:0026) |   ├─Token(NewLine) |\r\n|
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
//@[01:0003) ├─Token(NewLine) |\r\n|

//@[00:0000) └─Token(EndOfFile) ||
