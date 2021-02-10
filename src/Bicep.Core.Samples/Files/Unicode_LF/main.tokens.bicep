var emojis = '💪😊😈🍕☕'
//@[0:3) Identifier |var|
//@[4:10) Identifier |emojis|
//@[11:12) Assignment |=|
//@[13:24) StringComplete |'💪😊😈🍕☕'|
//@[24:25) NewLine |\n|
var ninjaCat = '🐱‍👤'
//@[0:3) Identifier |var|
//@[4:12) Identifier |ninjaCat|
//@[13:14) Assignment |=|
//@[15:22) StringComplete |'🐱‍👤'|
//@[22:24) NewLine |\n\n|

/*
朝辞白帝彩云间
千里江陵一日还
两岸猿声啼不住
轻舟已过万重山
*/
//@[2:4) NewLine |\n\n|

// greek letters in comment: Π π Φ φ plus emoji 😎
//@[50:51) NewLine |\n|
var variousAlphabets = {
//@[0:3) Identifier |var|
//@[4:20) Identifier |variousAlphabets|
//@[21:22) Assignment |=|
//@[23:24) LeftBrace |{|
//@[24:25) NewLine |\n|
  'α': 'α'
//@[2:5) StringComplete |'α'|
//@[5:6) Colon |:|
//@[7:10) StringComplete |'α'|
//@[10:11) NewLine |\n|
  'Ωω': [
//@[2:6) StringComplete |'Ωω'|
//@[6:7) Colon |:|
//@[8:9) LeftSquare |[|
//@[9:10) NewLine |\n|
    'Θμ'
//@[4:8) StringComplete |'Θμ'|
//@[8:9) NewLine |\n|
  ]
//@[2:3) RightSquare |]|
//@[3:4) NewLine |\n|
  'ążźćłóę': 'Cześć!'
//@[2:11) StringComplete |'ążźćłóę'|
//@[11:12) Colon |:|
//@[13:21) StringComplete |'Cześć!'|
//@[21:22) NewLine |\n|
  'áéóúñü': '¡Hola!'
//@[2:10) StringComplete |'áéóúñü'|
//@[10:11) Colon |:|
//@[12:20) StringComplete |'¡Hola!'|
//@[20:22) NewLine |\n\n|

  '二头肌': '二头肌'
//@[2:7) StringComplete |'二头肌'|
//@[7:8) Colon |:|
//@[9:14) StringComplete |'二头肌'|
//@[14:15) NewLine |\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

output concatUnicodeStrings string = concat('Θμ', '二头肌', 'α')
//@[0:6) Identifier |output|
//@[7:27) Identifier |concatUnicodeStrings|
//@[28:34) Identifier |string|
//@[35:36) Assignment |=|
//@[37:43) Identifier |concat|
//@[43:44) LeftParen |(|
//@[44:48) StringComplete |'Θμ'|
//@[48:49) Comma |,|
//@[50:55) StringComplete |'二头肌'|
//@[55:56) Comma |,|
//@[57:60) StringComplete |'α'|
//@[60:61) RightParen |)|
//@[61:62) NewLine |\n|
output interpolateUnicodeStrings string = 'Θμ二${emojis}头肌${ninjaCat}α'
//@[0:6) Identifier |output|
//@[7:32) Identifier |interpolateUnicodeStrings|
//@[33:39) Identifier |string|
//@[40:41) Assignment |=|
//@[42:48) StringLeftPiece |'Θμ二${|
//@[48:54) Identifier |emojis|
//@[54:59) StringMiddlePiece |}头肌${|
//@[59:67) Identifier |ninjaCat|
//@[67:70) StringRightPiece |}α'|
//@[70:72) NewLine |\n\n|

// all of these should produce the same string
//@[46:47) NewLine |\n|
var surrogate_char      = '𐐷'
//@[0:3) Identifier |var|
//@[4:18) Identifier |surrogate_char|
//@[24:25) Assignment |=|
//@[26:30) StringComplete |'𐐷'|
//@[30:31) NewLine |\n|
var surrogate_codepoint = '\u{10437}'
//@[0:3) Identifier |var|
//@[4:23) Identifier |surrogate_codepoint|
//@[24:25) Assignment |=|
//@[26:37) StringComplete |'\u{10437}'|
//@[37:38) NewLine |\n|
var surrogate_pairs     = '\u{D801}\u{DC37}'
//@[0:3) Identifier |var|
//@[4:19) Identifier |surrogate_pairs|
//@[24:25) Assignment |=|
//@[26:44) StringComplete |'\u{D801}\u{DC37}'|
//@[44:46) NewLine |\n\n|

// ascii escapes
//@[16:17) NewLine |\n|
var hello = '❆ Hello\u{20}World\u{21} ❁'
//@[0:3) Identifier |var|
//@[4:9) Identifier |hello|
//@[10:11) Assignment |=|
//@[12:40) StringComplete |'❆ Hello\u{20}World\u{21} ❁'|
//@[40:40) EndOfFile ||
