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

// greek letters in comment: Π π Φ φ plus emoji 😎
var variousAlphabets = {
//@[4:20) Variable variousAlphabets. Type: object. Declaration start char: 0, length: 119
  'α': 'α'
  'Ωω': [
    'Θμ'
  ]
  'ążźćłóę': 'Cześć!'
  'áéóúñü': '¡Hola!'

  '二头肌': '二头肌'
}

output concatUnicodeStrings string = concat('Θμ', '二头肌', 'α')
//@[7:27) Output concatUnicodeStrings. Type: string. Declaration start char: 0, length: 61
output interpolateUnicodeStrings string = 'Θμ二${emojis}头肌${ninjaCat}α'
//@[7:32) Output interpolateUnicodeStrings. Type: string. Declaration start char: 0, length: 70

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
