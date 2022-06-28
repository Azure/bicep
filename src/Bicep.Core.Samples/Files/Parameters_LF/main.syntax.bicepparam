/*
//@[00:835) ProgramSyntax
This is a
multiline comment!
*/
//@[02:006) ├─Token(NewLine) |\r\n\r\n|

// This is a single line comment
//@[32:036) ├─Token(NewLine) |\r\n\r\n|

// using keyword for specifying a Bicep file
//@[44:046) ├─Token(NewLine) |\r\n|
using './main.bicep/'
//@[00:021) ├─UsingDeclarationSyntax
//@[00:005) | ├─Token(Identifier) |using|
//@[06:021) | └─StringSyntax
//@[06:021) | | └─Token(StringComplete) |'./main.bicep/'|
//@[21:025) ├─Token(NewLine) |\r\n\r\n|

// parameter assignment to literals
//@[35:037) ├─Token(NewLine) |\r\n|
param myInt = 42
//@[00:016) ├─ParameterAssignmentSyntax
//@[00:005) | ├─Token(Identifier) |param|
//@[06:011) | ├─IdentifierSyntax
//@[06:011) | | └─Token(Identifier) |myInt|
//@[12:013) | ├─Token(Assignment) |=|
//@[14:016) | └─IntegerLiteralSyntax
//@[14:016) | | └─Token(Integer) |42|
//@[16:018) ├─Token(NewLine) |\r\n|
param myStr = "hello world!"
//@[00:028) ├─ParameterAssignmentSyntax
//@[00:005) | ├─Token(Identifier) |param|
//@[06:011) | ├─IdentifierSyntax
//@[06:011) | | └─Token(Identifier) |myStr|
//@[12:013) | ├─Token(Assignment) |=|
//@[14:028) | └─SkippedTriviaSyntax
//@[14:015) | | ├─Token(Unrecognized) |"|
//@[15:020) | | ├─Token(Identifier) |hello|
//@[21:026) | | ├─Token(Identifier) |world|
//@[26:027) | | ├─Token(Exclamation) |!|
//@[27:028) | | └─Token(Unrecognized) |"|
//@[28:030) ├─Token(NewLine) |\r\n|
param myBool = true
//@[00:019) ├─ParameterAssignmentSyntax
//@[00:005) | ├─Token(Identifier) |param|
//@[06:012) | ├─IdentifierSyntax
//@[06:012) | | └─Token(Identifier) |myBool|
//@[13:014) | ├─Token(Assignment) |=|
//@[15:019) | └─BooleanLiteralSyntax
//@[15:019) | | └─Token(TrueKeyword) |true|
//@[19:023) ├─Token(NewLine) |\r\n\r\n|

// parameter assignment to objects
//@[34:036) ├─Token(NewLine) |\r\n|
param myObj = {
//@[00:053) ├─ParameterAssignmentSyntax
//@[00:005) | ├─Token(Identifier) |param|
//@[06:011) | ├─IdentifierSyntax
//@[06:011) | | └─Token(Identifier) |myObj|
//@[12:013) | ├─Token(Assignment) |=|
//@[14:053) | └─ObjectSyntax
//@[14:015) | | ├─Token(LeftBrace) |{|
//@[15:017) | | ├─Token(NewLine) |\r\n|
	name: 'vm1'
//@[01:012) | | ├─ObjectPropertySyntax
//@[01:005) | | | ├─IdentifierSyntax
//@[01:005) | | | | └─Token(Identifier) |name|
//@[05:006) | | | ├─Token(Colon) |:|
//@[07:012) | | | └─StringSyntax
//@[07:012) | | | | └─Token(StringComplete) |'vm1'|
//@[12:014) | | ├─Token(NewLine) |\r\n|
	location: 'westus'
//@[01:019) | | ├─ObjectPropertySyntax
//@[01:009) | | | ├─IdentifierSyntax
//@[01:009) | | | | └─Token(Identifier) |location|
//@[09:010) | | | ├─Token(Colon) |:|
//@[11:019) | | | └─StringSyntax
//@[11:019) | | | | └─Token(StringComplete) |'westus'|
//@[19:021) | | ├─Token(NewLine) |\r\n|
}
//@[00:001) | | └─Token(RightBrace) |}|
//@[01:003) ├─Token(NewLine) |\r\n|
param myComplexObj = {
//@[00:134) ├─ParameterAssignmentSyntax
//@[00:005) | ├─Token(Identifier) |param|
//@[06:018) | ├─IdentifierSyntax
//@[06:018) | | └─Token(Identifier) |myComplexObj|
//@[19:020) | ├─Token(Assignment) |=|
//@[21:134) | └─ObjectSyntax
//@[21:022) | | ├─Token(LeftBrace) |{|
//@[22:024) | | ├─Token(NewLine) |\r\n|
	enabled: true
//@[01:014) | | ├─ObjectPropertySyntax
//@[01:008) | | | ├─IdentifierSyntax
//@[01:008) | | | | └─Token(Identifier) |enabled|
//@[08:009) | | | ├─Token(Colon) |:|
//@[10:014) | | | └─BooleanLiteralSyntax
//@[10:014) | | | | └─Token(TrueKeyword) |true|
//@[14:016) | | ├─Token(NewLine) |\r\n|
	name: 'complex object!'
//@[01:024) | | ├─ObjectPropertySyntax
//@[01:005) | | | ├─IdentifierSyntax
//@[01:005) | | | | └─Token(Identifier) |name|
//@[05:006) | | | ├─Token(Colon) |:|
//@[07:024) | | | └─StringSyntax
//@[07:024) | | | | └─Token(StringComplete) |'complex object!'|
//@[24:026) | | ├─Token(NewLine) |\r\n|
	priority: 3
//@[01:012) | | ├─ObjectPropertySyntax
//@[01:009) | | | ├─IdentifierSyntax
//@[01:009) | | | | └─Token(Identifier) |priority|
//@[09:010) | | | ├─Token(Colon) |:|
//@[11:012) | | | └─IntegerLiteralSyntax
//@[11:012) | | | | └─Token(Integer) |3|
//@[12:014) | | ├─Token(NewLine) |\r\n|
	data: {
//@[01:051) | | ├─ObjectPropertySyntax
//@[01:005) | | | ├─IdentifierSyntax
//@[01:005) | | | | └─Token(Identifier) |data|
//@[05:006) | | | ├─Token(Colon) |:|
//@[07:051) | | | └─ObjectSyntax
//@[07:008) | | | | ├─Token(LeftBrace) |{|
//@[08:010) | | | | ├─Token(NewLine) |\r\n|
		a: 'b'
//@[02:008) | | | | ├─ObjectPropertySyntax
//@[02:003) | | | | | ├─IdentifierSyntax
//@[02:003) | | | | | | └─Token(Identifier) |a|
//@[03:004) | | | | | ├─Token(Colon) |:|
//@[05:008) | | | | | └─StringSyntax
//@[05:008) | | | | | | └─Token(StringComplete) |'b'|
//@[08:010) | | | | ├─Token(NewLine) |\r\n|
		c: [
//@[02:027) | | | | ├─ObjectPropertySyntax
//@[02:003) | | | | | ├─IdentifierSyntax
//@[02:003) | | | | | | └─Token(Identifier) |c|
//@[03:004) | | | | | ├─Token(Colon) |:|
//@[05:027) | | | | | └─ArraySyntax
//@[05:006) | | | | | | ├─Token(LeftSquare) |[|
//@[06:008) | | | | | | ├─Token(NewLine) |\r\n|
			'd'
//@[03:006) | | | | | | ├─ArrayItemSyntax
//@[03:006) | | | | | | | └─StringSyntax
//@[03:006) | | | | | | | | └─Token(StringComplete) |'d'|
//@[06:008) | | | | | | ├─Token(NewLine) |\r\n|
			'e'
//@[03:006) | | | | | | ├─ArrayItemSyntax
//@[03:006) | | | | | | | └─StringSyntax
//@[03:006) | | | | | | | | └─Token(StringComplete) |'e'|
//@[06:008) | | | | | | ├─Token(NewLine) |\r\n|
		]
//@[02:003) | | | | | | └─Token(RightSquare) |]|
//@[03:005) | | | | ├─Token(NewLine) |\r\n|
	}
//@[01:002) | | | | └─Token(RightBrace) |}|
//@[02:004) | | ├─Token(NewLine) |\r\n|
}
//@[00:001) | | └─Token(RightBrace) |}|
//@[01:005) ├─Token(NewLine) |\r\n\r\n|

// parameter assignment to arrays
//@[33:035) ├─Token(NewLine) |\r\n|
param myIntArr = [
//@[00:041) ├─ParameterAssignmentSyntax
//@[00:005) | ├─Token(Identifier) |param|
//@[06:014) | ├─IdentifierSyntax
//@[06:014) | | └─Token(Identifier) |myIntArr|
//@[15:016) | ├─Token(Assignment) |=|
//@[17:041) | └─ArraySyntax
//@[17:018) | | ├─Token(LeftSquare) |[|
//@[18:020) | | ├─Token(NewLine) |\r\n|
	1
//@[01:002) | | ├─ArrayItemSyntax
//@[01:002) | | | └─IntegerLiteralSyntax
//@[01:002) | | | | └─Token(Integer) |1|
//@[02:004) | | ├─Token(NewLine) |\r\n|
	2
//@[01:002) | | ├─ArrayItemSyntax
//@[01:002) | | | └─IntegerLiteralSyntax
//@[01:002) | | | | └─Token(Integer) |2|
//@[02:004) | | ├─Token(NewLine) |\r\n|
	3
//@[01:002) | | ├─ArrayItemSyntax
//@[01:002) | | | └─IntegerLiteralSyntax
//@[01:002) | | | | └─Token(Integer) |3|
//@[02:004) | | ├─Token(NewLine) |\r\n|
	4
//@[01:002) | | ├─ArrayItemSyntax
//@[01:002) | | | └─IntegerLiteralSyntax
//@[01:002) | | | | └─Token(Integer) |4|
//@[02:004) | | ├─Token(NewLine) |\r\n|
	5
//@[01:002) | | ├─ArrayItemSyntax
//@[01:002) | | | └─IntegerLiteralSyntax
//@[01:002) | | | | └─Token(Integer) |5|
//@[02:004) | | ├─Token(NewLine) |\r\n|
]
//@[00:001) | | └─Token(RightSquare) |]|
//@[01:003) ├─Token(NewLine) |\r\n|
param myStrArr = [
//@[00:054) ├─ParameterAssignmentSyntax
//@[00:005) | ├─Token(Identifier) |param|
//@[06:014) | ├─IdentifierSyntax
//@[06:014) | | └─Token(Identifier) |myStrArr|
//@[15:016) | ├─Token(Assignment) |=|
//@[17:054) | └─ArraySyntax
//@[17:018) | | ├─Token(LeftSquare) |[|
//@[18:020) | | ├─Token(NewLine) |\r\n|
	'ant'
//@[01:006) | | ├─ArrayItemSyntax
//@[01:006) | | | └─StringSyntax
//@[01:006) | | | | └─Token(StringComplete) |'ant'|
//@[06:008) | | ├─Token(NewLine) |\r\n|
	'bear'
//@[01:007) | | ├─ArrayItemSyntax
//@[01:007) | | | └─StringSyntax
//@[01:007) | | | | └─Token(StringComplete) |'bear'|
//@[07:009) | | ├─Token(NewLine) |\r\n|
	'cat'
//@[01:006) | | ├─ArrayItemSyntax
//@[01:006) | | | └─StringSyntax
//@[01:006) | | | | └─Token(StringComplete) |'cat'|
//@[06:008) | | ├─Token(NewLine) |\r\n|
	'dog'
//@[01:006) | | ├─ArrayItemSyntax
//@[01:006) | | | └─StringSyntax
//@[01:006) | | | | └─Token(StringComplete) |'dog'|
//@[06:008) | | ├─Token(NewLine) |\r\n|
]
//@[00:001) | | └─Token(RightSquare) |]|
//@[01:003) ├─Token(NewLine) |\r\n|
param myComplexArr = [
//@[00:085) ├─ParameterAssignmentSyntax
//@[00:005) | ├─Token(Identifier) |param|
//@[06:018) | ├─IdentifierSyntax
//@[06:018) | | └─Token(Identifier) |myComplexArr|
//@[19:020) | ├─Token(Assignment) |=|
//@[21:085) | └─ArraySyntax
//@[21:022) | | ├─Token(LeftSquare) |[|
//@[22:024) | | ├─Token(NewLine) |\r\n|
	'eagle'
//@[01:008) | | ├─ArrayItemSyntax
//@[01:008) | | | └─StringSyntax
//@[01:008) | | | | └─Token(StringComplete) |'eagle'|
//@[08:010) | | ├─Token(NewLine) |\r\n|
	21
//@[01:003) | | ├─ArrayItemSyntax
//@[01:003) | | | └─IntegerLiteralSyntax
//@[01:003) | | | | └─Token(Integer) |21|
//@[03:005) | | ├─Token(NewLine) |\r\n|
	false
//@[01:006) | | ├─ArrayItemSyntax
//@[01:006) | | | └─BooleanLiteralSyntax
//@[01:006) | | | | └─Token(FalseKeyword) |false|
//@[06:008) | | ├─Token(NewLine) |\r\n|
	{
//@[01:035) | | ├─ArrayItemSyntax
//@[01:035) | | | └─ObjectSyntax
//@[01:002) | | | | ├─Token(LeftBrace) |{|
//@[02:004) | | | | ├─Token(NewLine) |\r\n|
		f: [
//@[02:027) | | | | ├─ObjectPropertySyntax
//@[02:003) | | | | | ├─IdentifierSyntax
//@[02:003) | | | | | | └─Token(Identifier) |f|
//@[03:004) | | | | | ├─Token(Colon) |:|
//@[05:027) | | | | | └─ArraySyntax
//@[05:006) | | | | | | ├─Token(LeftSquare) |[|
//@[06:008) | | | | | | ├─Token(NewLine) |\r\n|
			'g'
//@[03:006) | | | | | | ├─ArrayItemSyntax
//@[03:006) | | | | | | | └─StringSyntax
//@[03:006) | | | | | | | | └─Token(StringComplete) |'g'|
//@[06:008) | | | | | | ├─Token(NewLine) |\r\n|
			'h'
//@[03:006) | | | | | | ├─ArrayItemSyntax
//@[03:006) | | | | | | | └─StringSyntax
//@[03:006) | | | | | | | | └─Token(StringComplete) |'h'|
//@[06:008) | | | | | | ├─Token(NewLine) |\r\n|
		]
//@[02:003) | | | | | | └─Token(RightSquare) |]|
//@[03:005) | | | | ├─Token(NewLine) |\r\n|
	}
//@[01:002) | | | | └─Token(RightBrace) |}|
//@[02:004) | | ├─Token(NewLine) |\r\n|
]
//@[00:001) | | └─Token(RightSquare) |]|
//@[01:003) ├─Token(NewLine) |\r\n|
param myFunction = union({}, {})
//@[00:032) ├─ParameterAssignmentSyntax
//@[00:005) | ├─Token(Identifier) |param|
//@[06:016) | ├─IdentifierSyntax
//@[06:016) | | └─Token(Identifier) |myFunction|
//@[17:018) | ├─Token(Assignment) |=|
//@[19:032) | └─FunctionCallSyntax
//@[19:024) | | ├─IdentifierSyntax
//@[19:024) | | | └─Token(Identifier) |union|
//@[24:025) | | ├─Token(LeftParen) |(|
//@[25:027) | | ├─FunctionArgumentSyntax
//@[25:027) | | | └─ObjectSyntax
//@[25:026) | | | | ├─Token(LeftBrace) |{|
//@[26:027) | | | | └─Token(RightBrace) |}|
//@[27:028) | | ├─Token(Comma) |,|
//@[29:031) | | ├─FunctionArgumentSyntax
//@[29:031) | | | └─ObjectSyntax
//@[29:030) | | | | ├─Token(LeftBrace) |{|
//@[30:031) | | | | └─Token(RightBrace) |}|
//@[31:032) | | └─Token(RightParen) |)|
//@[32:034) ├─Token(NewLine) |\r\n|
param myComplexArrWithFunction = [
//@[00:095) ├─ParameterAssignmentSyntax
//@[00:005) | ├─Token(Identifier) |param|
//@[06:030) | ├─IdentifierSyntax
//@[06:030) | | └─Token(Identifier) |myComplexArrWithFunction|
//@[31:032) | ├─Token(Assignment) |=|
//@[33:095) | └─ArraySyntax
//@[33:034) | | ├─Token(LeftSquare) |[|
//@[34:036) | | ├─Token(NewLine) |\r\n|
	{
//@[01:030) | | ├─ArrayItemSyntax
//@[01:030) | | | └─ObjectSyntax
//@[01:002) | | | | ├─Token(LeftBrace) |{|
//@[02:004) | | | | ├─Token(NewLine) |\r\n|
		foo: resourceGroup()
//@[02:022) | | | | ├─ObjectPropertySyntax
//@[02:005) | | | | | ├─IdentifierSyntax
//@[02:005) | | | | | | └─Token(Identifier) |foo|
//@[05:006) | | | | | ├─Token(Colon) |:|
//@[07:022) | | | | | └─FunctionCallSyntax
//@[07:020) | | | | | | ├─IdentifierSyntax
//@[07:020) | | | | | | | └─Token(Identifier) |resourceGroup|
//@[20:021) | | | | | | ├─Token(LeftParen) |(|
//@[21:022) | | | | | | └─Token(RightParen) |)|
//@[22:024) | | | | ├─Token(NewLine) |\r\n|
	}
//@[01:002) | | | | └─Token(RightBrace) |}|
//@[02:004) | | ├─Token(NewLine) |\r\n|
	true
//@[01:005) | | ├─ArrayItemSyntax
//@[01:005) | | | └─BooleanLiteralSyntax
//@[01:005) | | | | └─Token(TrueKeyword) |true|
//@[05:007) | | ├─Token(NewLine) |\r\n|
	[
//@[01:017) | | ├─ArrayItemSyntax
//@[01:017) | | | └─ArraySyntax
//@[01:002) | | | | ├─Token(LeftSquare) |[|
//@[02:004) | | | | ├─Token(NewLine) |\r\n|
    	42
//@[05:007) | | | | ├─ArrayItemSyntax
//@[05:007) | | | | | └─IntegerLiteralSyntax
//@[05:007) | | | | | | └─Token(Integer) |42|
//@[07:009) | | | | ├─Token(NewLine) |\r\n|
  	]
//@[03:004) | | | | └─Token(RightSquare) |]|
//@[04:006) | | ├─Token(NewLine) |\r\n|
]
//@[00:001) | | └─Token(RightSquare) |]|
//@[01:001) └─Token(EndOfFile) ||
