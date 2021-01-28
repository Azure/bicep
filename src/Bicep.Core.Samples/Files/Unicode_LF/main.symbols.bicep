var emojis = '💪😊😈🍕☕'
//@[4:10) Variable emojis. Type: '💪😊😈🍕☕'. Declaration start char: 0, length: 24
var ninjaCat = '🐱‍👤'
//@[4:12) Variable ninjaCat. Type: '🐱‍👤'. Declaration start char: 0, length: 22

/*
朝辞白帝彩云间
千里江陵一日还
两岸猿声啼不住
轻舟已过万重山
*/

var α = 32
//@[4:5) Variable α. Type: int. Declaration start char: 0, length: 10
var Θμ = '💪'
//@[4:6) Variable Θμ. Type: '💪'. Declaration start char: 0, length: 13

var 二头肌 = true
//@[4:7) Variable 二头肌. Type: bool. Declaration start char: 0, length: 14

// greek letters in comment: Π π Φ φ plus emoji 😎
var variousAlphabets = {
//@[4:20) Variable variousAlphabets. Type: object. Declaration start char: 0, length: 103
  α: α
  Ωω: [
    Θμ
  ]
  ążźćłóę: 'Cześć!'
  áéóúñü: '¡Hola!'

  二头肌: 二头肌
}

output Ñ string = concat(Θμ, 二头肌, α)
//@[7:8) Output Ñ. Type: string. Declaration start char: 0, length: 36

// all of these should produce the same string
var surrogate_char      = '𐐷'
//@[4:18) Variable surrogate_char. Type: '𐐷'. Declaration start char: 0, length: 30
var surrogate_codepoint = '\u{10437}'
//@[4:23) Variable surrogate_codepoint. Type: '𐐷'. Declaration start char: 0, length: 37
var surrogate_pairs     = '\u{D801}\u{DC37}'
//@[4:19) Variable surrogate_pairs. Type: '𐐷'. Declaration start char: 0, length: 44

// ascii escapes
var hello = '❆ Hello\u{20}World\u{21} ❁'
//@[4:9) Variable hello. Type: '❆ Hello World! ❁'. Declaration start char: 0, length: 40

// identifier start characters
// Lu
var Öa = 1
//@[4:6) Variable Öa. Type: int. Declaration start char: 0, length: 10
var Ϸb = 1
//@[4:6) Variable Ϸb. Type: int. Declaration start char: 0, length: 10
// Ll
var ɇ3 = true
//@[4:6) Variable ɇ3. Type: bool. Declaration start char: 0, length: 13
var ɱɱg = true
//@[4:7) Variable ɱɱg. Type: bool. Declaration start char: 0, length: 14
// Lt
var ῼ = 1
//@[4:5) Variable ῼ. Type: int. Declaration start char: 0, length: 9
var ǈ = 2
//@[4:5) Variable ǈ. Type: int. Declaration start char: 0, length: 9
// Lm
var ᱽ = 1
//@[4:5) Variable ᱽ. Type: int. Declaration start char: 0, length: 9
var ᴲ = 1
//@[4:5) Variable ᴲ. Type: int. Declaration start char: 0, length: 9
// Lo
var ƻ = 2
//@[4:5) Variable ƻ. Type: int. Declaration start char: 0, length: 9
var ঽ = 1
//@[4:5) Variable ঽ. Type: int. Declaration start char: 0, length: 9
// Nl
var Ⅷa = 1
//@[4:6) Variable Ⅷa. Type: int. Declaration start char: 0, length: 10
var ↂa = 1
//@[4:6) Variable ↂa. Type: int. Declaration start char: 0, length: 10

// id start chars are id continue chars as well
var ÖaঽϷƻↂabɇ3ɱɱgῼǈⅧaᴲᱽ = 100
//@[4:23) Variable ÖaঽϷƻↂabɇ3ɱɱgῼǈⅧaᴲᱽ. Type: int. Declaration start char: 0, length: 29

// additional id continue classes
var a೦4﹏﹎    = 1000
//@[4:9) Variable a೦4﹏﹎. Type: int. Declaration start char: 0, length: 19
