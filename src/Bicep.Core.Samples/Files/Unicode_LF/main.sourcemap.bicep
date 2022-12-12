var emojis = '💪😊😈🍕☕'
//@[line00->line11]     "emojis": "💪😊😈🍕☕",
var ninjaCat = '🐱‍👤'
//@[line01->line12]     "ninjaCat": "🐱‍👤",

/*
朝辞白帝彩云间
千里江陵一日还
两岸猿声啼不住
轻舟已过万重山
*/

// greek letters in comment: Π π Φ φ plus emoji 😎
var variousAlphabets = {
//@[line11->line13]     "variousAlphabets": {
//@[line11->line21]     },
  'α': 'α'
//@[line12->line14]       "α": "α",
  'Ωω': [
//@[line13->line15]       "Ωω": [
//@[line13->line17]       ],
    'Θμ'
//@[line14->line16]         "Θμ"
  ]
  'ążźćłóę': 'Cześć!'
//@[line16->line18]       "ążźćłóę": "Cześć!",
  'áéóúñü': '¡Hola!'
//@[line17->line19]       "áéóúñü": "¡Hola!",

  '二头肌': '二头肌'
//@[line19->line20]       "二头肌": "二头肌"
}

output concatUnicodeStrings string = concat('Θμ', '二头肌', 'α')
//@[line22->line29]     "concatUnicodeStrings": {
//@[line22->line30]       "type": "string",
//@[line22->line31]       "value": "[concat('Θμ', '二头肌', 'α')]"
//@[line22->line32]     },
output interpolateUnicodeStrings string = 'Θμ二${emojis}头肌${ninjaCat}α'
//@[line23->line33]     "interpolateUnicodeStrings": {
//@[line23->line34]       "type": "string",
//@[line23->line35]       "value": "[format('Θμ二{0}头肌{1}α', variables('emojis'), variables('ninjaCat'))]"
//@[line23->line36]     }

// all of these should produce the same string
var surrogate_char      = '𐐷'
//@[line26->line22]     "surrogate_char": "𐐷",
var surrogate_codepoint = '\u{10437}'
//@[line27->line23]     "surrogate_codepoint": "𐐷",
var surrogate_pairs     = '\u{D801}\u{DC37}'
//@[line28->line24]     "surrogate_pairs": "𐐷",

// ascii escapes
var hello = '❆ Hello\u{20}World\u{21} ❁'
//@[line31->line25]     "hello": "❆ Hello World! ❁"
