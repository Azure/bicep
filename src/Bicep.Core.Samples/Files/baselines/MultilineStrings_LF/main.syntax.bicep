var name = 'Anthony'
//@[00:492) ProgramSyntax
//@[00:020) ├─VariableDeclarationSyntax
//@[00:003) | ├─Token(Identifier) |var|
//@[04:008) | ├─IdentifierSyntax
//@[04:008) | | └─Token(Identifier) |name|
//@[09:010) | ├─Token(Assignment) |=|
//@[11:020) | └─StringSyntax
//@[11:020) |   └─Token(StringComplete) |'Anthony'|
//@[20:021) ├─Token(NewLine) |\n|
var multilineInterpolation = $'''
//@[00:063) ├─VariableDeclarationSyntax
//@[00:003) | ├─Token(Identifier) |var|
//@[04:026) | ├─IdentifierSyntax
//@[04:026) | | └─Token(Identifier) |multilineInterpolation|
//@[27:028) | ├─Token(Assignment) |=|
//@[29:063) | └─StringSyntax
//@[29:054) |   ├─Token(StringLeftPiece) |$'''\nHello,\nmy\nname is\n${|
Hello,
my
name is
${name}
//@[02:006) |   ├─VariableAccessSyntax
//@[02:006) |   | └─IdentifierSyntax
//@[02:006) |   |   └─Token(Identifier) |name|
//@[06:011) |   └─Token(StringRightPiece) |}\n'''|
'''
//@[03:005) ├─Token(NewLine) |\n\n|

var complexMultilineInterpolation = $$$'''
//@[00:084) ├─VariableDeclarationSyntax
//@[00:003) | ├─Token(Identifier) |var|
//@[04:033) | ├─IdentifierSyntax
//@[04:033) | | └─Token(Identifier) |complexMultilineInterpolation|
//@[34:035) | ├─Token(Assignment) |=|
//@[36:084) | └─StringSyntax
//@[36:064) |   ├─Token(StringLeftPiece) |$$$'''\n${name}\n$${name}\n$$${|
${name}
$${name}
$$${name}
//@[04:008) |   ├─VariableAccessSyntax
//@[04:008) |   | └─IdentifierSyntax
//@[04:008) |   |   └─Token(Identifier) |name|
//@[08:015) |   ├─Token(StringMiddlePiece) |}\n$$$${|
$$$${name}
//@[05:009) |   ├─VariableAccessSyntax
//@[05:009) |   | └─IdentifierSyntax
//@[05:009) |   |   └─Token(Identifier) |name|
//@[09:014) |   └─Token(StringRightPiece) |}\n'''|
'''
//@[03:005) ├─Token(NewLine) |\n\n|

var interpMultiEmpty = $''''''
//@[00:030) ├─VariableDeclarationSyntax
//@[00:003) | ├─Token(Identifier) |var|
//@[04:020) | ├─IdentifierSyntax
//@[04:020) | | └─Token(Identifier) |interpMultiEmpty|
//@[21:022) | ├─Token(Assignment) |=|
//@[23:030) | └─StringSyntax
//@[23:030) |   └─Token(StringComplete) |$''''''|
//@[30:031) ├─Token(NewLine) |\n|
var interp1Multi = $'''
//@[00:040) ├─VariableDeclarationSyntax
//@[00:003) | ├─Token(Identifier) |var|
//@[04:016) | ├─IdentifierSyntax
//@[04:016) | | └─Token(Identifier) |interp1Multi|
//@[17:018) | ├─Token(Assignment) |=|
//@[19:040) | └─StringSyntax
//@[19:029) |   ├─Token(StringLeftPiece) |$'''\nabc${|
abc${123}def
//@[05:008) |   ├─IntegerLiteralSyntax
//@[05:008) |   | └─Token(Integer) |123|
//@[08:016) |   └─Token(StringRightPiece) |}def\n'''|
'''
//@[03:004) ├─Token(NewLine) |\n|
var interp2Multi = $'''${123}def'''
//@[00:035) ├─VariableDeclarationSyntax
//@[00:003) | ├─Token(Identifier) |var|
//@[04:016) | ├─IdentifierSyntax
//@[04:016) | | └─Token(Identifier) |interp2Multi|
//@[17:018) | ├─Token(Assignment) |=|
//@[19:035) | └─StringSyntax
//@[19:025) |   ├─Token(StringLeftPiece) |$'''${|
//@[25:028) |   ├─IntegerLiteralSyntax
//@[25:028) |   | └─Token(Integer) |123|
//@[28:035) |   └─Token(StringRightPiece) |}def'''|
//@[35:036) ├─Token(NewLine) |\n|
var interp3Multi = $$'''abc$${123}'''
//@[00:037) ├─VariableDeclarationSyntax
//@[00:003) | ├─Token(Identifier) |var|
//@[04:016) | ├─IdentifierSyntax
//@[04:016) | | └─Token(Identifier) |interp3Multi|
//@[17:018) | ├─Token(Assignment) |=|
//@[19:037) | └─StringSyntax
//@[19:030) |   ├─Token(StringLeftPiece) |$$'''abc$${|
//@[30:033) |   ├─IntegerLiteralSyntax
//@[30:033) |   | └─Token(Integer) |123|
//@[33:037) |   └─Token(StringRightPiece) |}'''|
//@[37:038) ├─Token(NewLine) |\n|
var interp4Multi = $'''abc${123}${456}jk$l${789}p$'''
//@[00:053) ├─VariableDeclarationSyntax
//@[00:003) | ├─Token(Identifier) |var|
//@[04:016) | ├─IdentifierSyntax
//@[04:016) | | └─Token(Identifier) |interp4Multi|
//@[17:018) | ├─Token(Assignment) |=|
//@[19:053) | └─StringSyntax
//@[19:028) |   ├─Token(StringLeftPiece) |$'''abc${|
//@[28:031) |   ├─IntegerLiteralSyntax
//@[28:031) |   | └─Token(Integer) |123|
//@[31:034) |   ├─Token(StringMiddlePiece) |}${|
//@[34:037) |   ├─IntegerLiteralSyntax
//@[34:037) |   | └─Token(Integer) |456|
//@[37:044) |   ├─Token(StringMiddlePiece) |}jk$l${|
//@[44:047) |   ├─IntegerLiteralSyntax
//@[44:047) |   | └─Token(Integer) |789|
//@[47:053) |   └─Token(StringRightPiece) |}p$'''|
//@[53:054) ├─Token(NewLine) |\n|
var doubleInterpMulti = $'''abc${'def${123}'}_${'${456}${789}'}'''
//@[00:066) ├─VariableDeclarationSyntax
//@[00:003) | ├─Token(Identifier) |var|
//@[04:021) | ├─IdentifierSyntax
//@[04:021) | | └─Token(Identifier) |doubleInterpMulti|
//@[22:023) | ├─Token(Assignment) |=|
//@[24:066) | └─StringSyntax
//@[24:033) |   ├─Token(StringLeftPiece) |$'''abc${|
//@[33:044) |   ├─StringSyntax
//@[33:039) |   | ├─Token(StringLeftPiece) |'def${|
//@[39:042) |   | ├─IntegerLiteralSyntax
//@[39:042) |   | | └─Token(Integer) |123|
//@[42:044) |   | └─Token(StringRightPiece) |}'|
//@[44:048) |   ├─Token(StringMiddlePiece) |}_${|
//@[48:062) |   ├─StringSyntax
//@[48:051) |   | ├─Token(StringLeftPiece) |'${|
//@[51:054) |   | ├─IntegerLiteralSyntax
//@[51:054) |   | | └─Token(Integer) |456|
//@[54:057) |   | ├─Token(StringMiddlePiece) |}${|
//@[57:060) |   | ├─IntegerLiteralSyntax
//@[57:060) |   | | └─Token(Integer) |789|
//@[60:062) |   | └─Token(StringRightPiece) |}'|
//@[62:066) |   └─Token(StringRightPiece) |}'''|
//@[66:067) ├─Token(NewLine) |\n|
var curliesInInterpMulti = $'''{${123}{0}${true}}'''
//@[00:052) ├─VariableDeclarationSyntax
//@[00:003) | ├─Token(Identifier) |var|
//@[04:024) | ├─IdentifierSyntax
//@[04:024) | | └─Token(Identifier) |curliesInInterpMulti|
//@[25:026) | ├─Token(Assignment) |=|
//@[27:052) | └─StringSyntax
//@[27:034) |   ├─Token(StringLeftPiece) |$'''{${|
//@[34:037) |   ├─IntegerLiteralSyntax
//@[34:037) |   | └─Token(Integer) |123|
//@[37:043) |   ├─Token(StringMiddlePiece) |}{0}${|
//@[43:047) |   ├─BooleanLiteralSyntax
//@[43:047) |   | └─Token(TrueKeyword) |true|
//@[47:052) |   └─Token(StringRightPiece) |}}'''|
//@[52:053) ├─Token(NewLine) |\n|

//@[00:000) └─Token(EndOfFile) ||
