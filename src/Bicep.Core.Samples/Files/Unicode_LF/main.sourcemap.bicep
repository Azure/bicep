var emojis = '💪😊😈🍕☕'
//@[11:11]     "emojis": "💪😊😈🍕☕",
var ninjaCat = '🐱‍👤'
//@[12:12]     "ninjaCat": "🐱‍👤",

/*
朝辞白帝彩云间
千里江陵一日还
两岸猿声啼不住
轻舟已过万重山
*/

// greek letters in comment: Π π Φ φ plus emoji 😎
var variousAlphabets = {
//@[13:21]     "variousAlphabets": {
  'α': 'α'
//@[14:14]       "α": "α",
  'Ωω': [
//@[15:17]       "Ωω": [
    'Θμ'
//@[16:16]         "Θμ"
  ]
  'ążźćłóę': 'Cześć!'
//@[18:18]       "ążźćłóę": "Cześć!",
  'áéóúñü': '¡Hola!'
//@[19:19]       "áéóúñü": "¡Hola!",

  '二头肌': '二头肌'
//@[20:20]       "二头肌": "二头肌"
}

output concatUnicodeStrings string = concat('Θμ', '二头肌', 'α')
//@[29:32]     "concatUnicodeStrings": {
output interpolateUnicodeStrings string = 'Θμ二${emojis}头肌${ninjaCat}α'
//@[33:36]     "interpolateUnicodeStrings": {

// all of these should produce the same string
var surrogate_char      = '𐐷'
//@[22:22]     "surrogate_char": "𐐷",
var surrogate_codepoint = '\u{10437}'
//@[23:23]     "surrogate_codepoint": "𐐷",
var surrogate_pairs     = '\u{D801}\u{DC37}'
//@[24:24]     "surrogate_pairs": "𐐷",

// ascii escapes
var hello = '❆ Hello\u{20}World\u{21} ❁'
//@[25:25]     "hello": "❆ Hello World! ❁"
