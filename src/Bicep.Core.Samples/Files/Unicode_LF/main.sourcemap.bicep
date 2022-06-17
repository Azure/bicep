var emojis = '💪😊😈🍕☕'
//@[12:12]     "emojis": "💪😊😈🍕☕",
var ninjaCat = '🐱‍👤'
//@[13:13]     "ninjaCat": "🐱‍👤",

/*
朝辞白帝彩云间
千里江陵一日还
两岸猿声啼不住
轻舟已过万重山
*/

// greek letters in comment: Π π Φ φ plus emoji 😎
var variousAlphabets = {
//@[14:22]     "variousAlphabets": {
  'α': 'α'
//@[15:15]       "α": "α",
  'Ωω': [
//@[16:18]       "Ωω": [
    'Θμ'
//@[17:17]         "Θμ"
  ]
  'ążźćłóę': 'Cześć!'
//@[19:19]       "ążźćłóę": "Cześć!",
  'áéóúñü': '¡Hola!'
//@[20:20]       "áéóúñü": "¡Hola!",

  '二头肌': '二头肌'
//@[21:21]       "二头肌": "二头肌"
}

output concatUnicodeStrings string = concat('Θμ', '二头肌', 'α')
//@[30:33]     "concatUnicodeStrings": {
output interpolateUnicodeStrings string = 'Θμ二${emojis}头肌${ninjaCat}α'
//@[34:37]     "interpolateUnicodeStrings": {

// all of these should produce the same string
var surrogate_char      = '𐐷'
//@[23:23]     "surrogate_char": "𐐷",
var surrogate_codepoint = '\u{10437}'
//@[24:24]     "surrogate_codepoint": "𐐷",
var surrogate_pairs     = '\u{D801}\u{DC37}'
//@[25:25]     "surrogate_pairs": "𐐷",

// ascii escapes
var hello = '❆ Hello\u{20}World\u{21} ❁'
//@[26:26]     "hello": "❆ Hello World! ❁"
