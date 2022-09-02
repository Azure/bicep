var emojis = '💪😊😈🍕☕'
//@[00:613) ProgramSyntax
//@[00:024) ├─VariableDeclarationSyntax
//@[00:003) | ├─Token(Identifier) |var|
//@[04:010) | ├─IdentifierSyntax
//@[04:010) | | └─Token(Identifier) |emojis|
//@[11:012) | ├─Token(Assignment) |=|
//@[13:024) | └─StringSyntax
//@[13:024) |   └─Token(StringComplete) |'💪😊😈🍕☕'|
//@[24:025) ├─Token(NewLine) |\n|
var ninjaCat = '🐱‍👤'
//@[00:022) ├─VariableDeclarationSyntax
//@[00:003) | ├─Token(Identifier) |var|
//@[04:012) | ├─IdentifierSyntax
//@[04:012) | | └─Token(Identifier) |ninjaCat|
//@[13:014) | ├─Token(Assignment) |=|
//@[15:022) | └─StringSyntax
//@[15:022) |   └─Token(StringComplete) |'🐱‍👤'|
//@[22:024) ├─Token(NewLine) |\n\n|

/*
朝辞白帝彩云间
千里江陵一日还
两岸猿声啼不住
轻舟已过万重山
*/
//@[02:004) ├─Token(NewLine) |\n\n|

// greek letters in comment: Π π Φ φ plus emoji 😎
//@[50:051) ├─Token(NewLine) |\n|
var variousAlphabets = {
//@[00:119) ├─VariableDeclarationSyntax
//@[00:003) | ├─Token(Identifier) |var|
//@[04:020) | ├─IdentifierSyntax
//@[04:020) | | └─Token(Identifier) |variousAlphabets|
//@[21:022) | ├─Token(Assignment) |=|
//@[23:119) | └─ObjectSyntax
//@[23:024) |   ├─Token(LeftBrace) |{|
//@[24:025) |   ├─Token(NewLine) |\n|
  'α': 'α'
//@[02:010) |   ├─ObjectPropertySyntax
//@[02:005) |   | ├─StringSyntax
//@[02:005) |   | | └─Token(StringComplete) |'α'|
//@[05:006) |   | ├─Token(Colon) |:|
//@[07:010) |   | └─StringSyntax
//@[07:010) |   |   └─Token(StringComplete) |'α'|
//@[10:011) |   ├─Token(NewLine) |\n|
  'Ωω': [
//@[02:022) |   ├─ObjectPropertySyntax
//@[02:006) |   | ├─StringSyntax
//@[02:006) |   | | └─Token(StringComplete) |'Ωω'|
//@[06:007) |   | ├─Token(Colon) |:|
//@[08:022) |   | └─ArraySyntax
//@[08:009) |   |   ├─Token(LeftSquare) |[|
//@[09:010) |   |   ├─Token(NewLine) |\n|
    'Θμ'
//@[04:008) |   |   ├─ArrayItemSyntax
//@[04:008) |   |   | └─StringSyntax
//@[04:008) |   |   |   └─Token(StringComplete) |'Θμ'|
//@[08:009) |   |   ├─Token(NewLine) |\n|
  ]
//@[02:003) |   |   └─Token(RightSquare) |]|
//@[03:004) |   ├─Token(NewLine) |\n|
  'ążźćłóę': 'Cześć!'
//@[02:021) |   ├─ObjectPropertySyntax
//@[02:011) |   | ├─StringSyntax
//@[02:011) |   | | └─Token(StringComplete) |'ążźćłóę'|
//@[11:012) |   | ├─Token(Colon) |:|
//@[13:021) |   | └─StringSyntax
//@[13:021) |   |   └─Token(StringComplete) |'Cześć!'|
//@[21:022) |   ├─Token(NewLine) |\n|
  'áéóúñü': '¡Hola!'
//@[02:020) |   ├─ObjectPropertySyntax
//@[02:010) |   | ├─StringSyntax
//@[02:010) |   | | └─Token(StringComplete) |'áéóúñü'|
//@[10:011) |   | ├─Token(Colon) |:|
//@[12:020) |   | └─StringSyntax
//@[12:020) |   |   └─Token(StringComplete) |'¡Hola!'|
//@[20:022) |   ├─Token(NewLine) |\n\n|

  '二头肌': '二头肌'
//@[02:014) |   ├─ObjectPropertySyntax
//@[02:007) |   | ├─StringSyntax
//@[02:007) |   | | └─Token(StringComplete) |'二头肌'|
//@[07:008) |   | ├─Token(Colon) |:|
//@[09:014) |   | └─StringSyntax
//@[09:014) |   |   └─Token(StringComplete) |'二头肌'|
//@[14:015) |   ├─Token(NewLine) |\n|
}
//@[00:001) |   └─Token(RightBrace) |}|
//@[01:003) ├─Token(NewLine) |\n\n|

output concatUnicodeStrings string = concat('Θμ', '二头肌', 'α')
//@[00:061) ├─OutputDeclarationSyntax
//@[00:006) | ├─Token(Identifier) |output|
//@[07:027) | ├─IdentifierSyntax
//@[07:027) | | └─Token(Identifier) |concatUnicodeStrings|
//@[28:034) | ├─SimpleTypeSyntax
//@[28:034) | | └─Token(Identifier) |string|
//@[35:036) | ├─Token(Assignment) |=|
//@[37:061) | └─FunctionCallSyntax
//@[37:043) |   ├─IdentifierSyntax
//@[37:043) |   | └─Token(Identifier) |concat|
//@[43:044) |   ├─Token(LeftParen) |(|
//@[44:048) |   ├─FunctionArgumentSyntax
//@[44:048) |   | └─StringSyntax
//@[44:048) |   |   └─Token(StringComplete) |'Θμ'|
//@[48:049) |   ├─Token(Comma) |,|
//@[50:055) |   ├─FunctionArgumentSyntax
//@[50:055) |   | └─StringSyntax
//@[50:055) |   |   └─Token(StringComplete) |'二头肌'|
//@[55:056) |   ├─Token(Comma) |,|
//@[57:060) |   ├─FunctionArgumentSyntax
//@[57:060) |   | └─StringSyntax
//@[57:060) |   |   └─Token(StringComplete) |'α'|
//@[60:061) |   └─Token(RightParen) |)|
//@[61:062) ├─Token(NewLine) |\n|
output interpolateUnicodeStrings string = 'Θμ二${emojis}头肌${ninjaCat}α'
//@[00:070) ├─OutputDeclarationSyntax
//@[00:006) | ├─Token(Identifier) |output|
//@[07:032) | ├─IdentifierSyntax
//@[07:032) | | └─Token(Identifier) |interpolateUnicodeStrings|
//@[33:039) | ├─SimpleTypeSyntax
//@[33:039) | | └─Token(Identifier) |string|
//@[40:041) | ├─Token(Assignment) |=|
//@[42:070) | └─StringSyntax
//@[42:048) |   ├─Token(StringLeftPiece) |'Θμ二${|
//@[48:054) |   ├─VariableAccessSyntax
//@[48:054) |   | └─IdentifierSyntax
//@[48:054) |   |   └─Token(Identifier) |emojis|
//@[54:059) |   ├─Token(StringMiddlePiece) |}头肌${|
//@[59:067) |   ├─VariableAccessSyntax
//@[59:067) |   | └─IdentifierSyntax
//@[59:067) |   |   └─Token(Identifier) |ninjaCat|
//@[67:070) |   └─Token(StringRightPiece) |}α'|
//@[70:072) ├─Token(NewLine) |\n\n|

// all of these should produce the same string
//@[46:047) ├─Token(NewLine) |\n|
var surrogate_char      = '𐐷'
//@[00:030) ├─VariableDeclarationSyntax
//@[00:003) | ├─Token(Identifier) |var|
//@[04:018) | ├─IdentifierSyntax
//@[04:018) | | └─Token(Identifier) |surrogate_char|
//@[24:025) | ├─Token(Assignment) |=|
//@[26:030) | └─StringSyntax
//@[26:030) |   └─Token(StringComplete) |'𐐷'|
//@[30:031) ├─Token(NewLine) |\n|
var surrogate_codepoint = '\u{10437}'
//@[00:037) ├─VariableDeclarationSyntax
//@[00:003) | ├─Token(Identifier) |var|
//@[04:023) | ├─IdentifierSyntax
//@[04:023) | | └─Token(Identifier) |surrogate_codepoint|
//@[24:025) | ├─Token(Assignment) |=|
//@[26:037) | └─StringSyntax
//@[26:037) |   └─Token(StringComplete) |'\u{10437}'|
//@[37:038) ├─Token(NewLine) |\n|
var surrogate_pairs     = '\u{D801}\u{DC37}'
//@[00:044) ├─VariableDeclarationSyntax
//@[00:003) | ├─Token(Identifier) |var|
//@[04:019) | ├─IdentifierSyntax
//@[04:019) | | └─Token(Identifier) |surrogate_pairs|
//@[24:025) | ├─Token(Assignment) |=|
//@[26:044) | └─StringSyntax
//@[26:044) |   └─Token(StringComplete) |'\u{D801}\u{DC37}'|
//@[44:046) ├─Token(NewLine) |\n\n|

// ascii escapes
//@[16:017) ├─Token(NewLine) |\n|
var hello = '❆ Hello\u{20}World\u{21} ❁'
//@[00:040) ├─VariableDeclarationSyntax
//@[00:003) | ├─Token(Identifier) |var|
//@[04:009) | ├─IdentifierSyntax
//@[04:009) | | └─Token(Identifier) |hello|
//@[10:011) | ├─Token(Assignment) |=|
//@[12:040) | └─StringSyntax
//@[12:040) |   └─Token(StringComplete) |'❆ Hello\u{20}World\u{21} ❁'|
//@[40:040) └─Token(EndOfFile) ||
