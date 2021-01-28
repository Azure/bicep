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

var α = 32
//@[0:3) Identifier |var|
//@[4:5) Identifier |α|
//@[6:7) Assignment |=|
//@[8:10) Integer |32|
//@[10:11) NewLine |\n|
var Θμ = '💪'
//@[0:3) Identifier |var|
//@[4:6) Identifier |Θμ|
//@[7:8) Assignment |=|
//@[9:13) StringComplete |'💪'|
//@[13:15) NewLine |\n\n|

var 二头肌 = true
//@[0:3) Identifier |var|
//@[4:7) Identifier |二头肌|
//@[8:9) Assignment |=|
//@[10:14) TrueKeyword |true|
//@[14:16) NewLine |\n\n|

// greek letters in comment: Π π Φ φ plus emoji 😎
//@[50:51) NewLine |\n|
var variousAlphabets = {
//@[0:3) Identifier |var|
//@[4:20) Identifier |variousAlphabets|
//@[21:22) Assignment |=|
//@[23:24) LeftBrace |{|
//@[24:25) NewLine |\n|
  α: α
//@[2:3) Identifier |α|
//@[3:4) Colon |:|
//@[5:6) Identifier |α|
//@[6:7) NewLine |\n|
  Ωω: [
//@[2:4) Identifier |Ωω|
//@[4:5) Colon |:|
//@[6:7) LeftSquare |[|
//@[7:8) NewLine |\n|
    Θμ
//@[4:6) Identifier |Θμ|
//@[6:7) NewLine |\n|
  ]
//@[2:3) RightSquare |]|
//@[3:4) NewLine |\n|
  ążźćłóę: 'Cześć!'
//@[2:9) Identifier |ążźćłóę|
//@[9:10) Colon |:|
//@[11:19) StringComplete |'Cześć!'|
//@[19:20) NewLine |\n|
  áéóúñü: '¡Hola!'
//@[2:8) Identifier |áéóúñü|
//@[8:9) Colon |:|
//@[10:18) StringComplete |'¡Hola!'|
//@[18:20) NewLine |\n\n|

  二头肌: 二头肌
//@[2:5) Identifier |二头肌|
//@[5:6) Colon |:|
//@[7:10) Identifier |二头肌|
//@[10:11) NewLine |\n|
}
//@[0:1) RightBrace |}|
//@[1:3) NewLine |\n\n|

output Ñ string = concat(Θμ, 二头肌, α)
//@[0:6) Identifier |output|
//@[7:8) Identifier |Ñ|
//@[9:15) Identifier |string|
//@[16:17) Assignment |=|
//@[18:24) Identifier |concat|
//@[24:25) LeftParen |(|
//@[25:27) Identifier |Θμ|
//@[27:28) Comma |,|
//@[29:32) Identifier |二头肌|
//@[32:33) Comma |,|
//@[34:35) Identifier |α|
//@[35:36) RightParen |)|
//@[36:38) NewLine |\n\n|

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
//@[40:42) NewLine |\n\n|

// identifier start characters
//@[30:31) NewLine |\n|
// Lu
//@[5:6) NewLine |\n|
var Öa = 1
//@[0:3) Identifier |var|
//@[4:6) Identifier |Öa|
//@[7:8) Assignment |=|
//@[9:10) Integer |1|
//@[10:11) NewLine |\n|
var Ϸb = 1
//@[0:3) Identifier |var|
//@[4:6) Identifier |Ϸb|
//@[7:8) Assignment |=|
//@[9:10) Integer |1|
//@[10:11) NewLine |\n|
// Ll
//@[5:6) NewLine |\n|
var ɇ3 = true
//@[0:3) Identifier |var|
//@[4:6) Identifier |ɇ3|
//@[7:8) Assignment |=|
//@[9:13) TrueKeyword |true|
//@[13:14) NewLine |\n|
var ɱɱg = true
//@[0:3) Identifier |var|
//@[4:7) Identifier |ɱɱg|
//@[8:9) Assignment |=|
//@[10:14) TrueKeyword |true|
//@[14:15) NewLine |\n|
// Lt
//@[5:6) NewLine |\n|
var ῼ = 1
//@[0:3) Identifier |var|
//@[4:5) Identifier |ῼ|
//@[6:7) Assignment |=|
//@[8:9) Integer |1|
//@[9:10) NewLine |\n|
var ǈ = 2
//@[0:3) Identifier |var|
//@[4:5) Identifier |ǈ|
//@[6:7) Assignment |=|
//@[8:9) Integer |2|
//@[9:10) NewLine |\n|
// Lm
//@[5:6) NewLine |\n|
var ᱽ = 1
//@[0:3) Identifier |var|
//@[4:5) Identifier |ᱽ|
//@[6:7) Assignment |=|
//@[8:9) Integer |1|
//@[9:10) NewLine |\n|
var ᴲ = 1
//@[0:3) Identifier |var|
//@[4:5) Identifier |ᴲ|
//@[6:7) Assignment |=|
//@[8:9) Integer |1|
//@[9:10) NewLine |\n|
// Lo
//@[5:6) NewLine |\n|
var ƻ = 2
//@[0:3) Identifier |var|
//@[4:5) Identifier |ƻ|
//@[6:7) Assignment |=|
//@[8:9) Integer |2|
//@[9:10) NewLine |\n|
var ঽ = 1
//@[0:3) Identifier |var|
//@[4:5) Identifier |ঽ|
//@[6:7) Assignment |=|
//@[8:9) Integer |1|
//@[9:10) NewLine |\n|
// Nl
//@[5:6) NewLine |\n|
var Ⅷa = 1
//@[0:3) Identifier |var|
//@[4:6) Identifier |Ⅷa|
//@[7:8) Assignment |=|
//@[9:10) Integer |1|
//@[10:11) NewLine |\n|
var ↂa = 1
//@[0:3) Identifier |var|
//@[4:6) Identifier |ↂa|
//@[7:8) Assignment |=|
//@[9:10) Integer |1|
//@[10:12) NewLine |\n\n|

// id start chars are id continue chars as well
//@[47:48) NewLine |\n|
var ÖaঽϷƻↂabɇ3ɱɱgῼǈⅧaᴲᱽ = 100
//@[0:3) Identifier |var|
//@[4:23) Identifier |ÖaঽϷƻↂabɇ3ɱɱgῼǈⅧaᴲᱽ|
//@[24:25) Assignment |=|
//@[26:29) Integer |100|
//@[29:31) NewLine |\n\n|

// additional id continue classes
//@[33:34) NewLine |\n|
var a೦4﹏﹎    = 1000
//@[0:3) Identifier |var|
//@[4:9) Identifier |a೦4﹏﹎|
//@[13:14) Assignment |=|
//@[15:19) Integer |1000|
//@[19:19) EndOfFile ||
