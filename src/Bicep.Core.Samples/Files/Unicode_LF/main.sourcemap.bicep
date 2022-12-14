var emojis = '💪😊😈🍕☕'
//@    "emojis": "💪😊😈🍕☕",
var ninjaCat = '🐱‍👤'
//@    "ninjaCat": "🐱‍👤",

/*
朝辞白帝彩云间
千里江陵一日还
两岸猿声啼不住
轻舟已过万重山
*/

// greek letters in comment: Π π Φ φ plus emoji 😎
var variousAlphabets = {
//@    "variousAlphabets": {
//@    },
  'α': 'α'
//@      "α": "α",
  'Ωω': [
//@      "Ωω": [
//@      ],
    'Θμ'
//@        "Θμ"
  ]
  'ążźćłóę': 'Cześć!'
//@      "ążźćłóę": "Cześć!",
  'áéóúñü': '¡Hola!'
//@      "áéóúñü": "¡Hola!",

  '二头肌': '二头肌'
//@      "二头肌": "二头肌"
}

output concatUnicodeStrings string = concat('Θμ', '二头肌', 'α')
//@    "concatUnicodeStrings": {
//@      "type": "string",
//@      "value": "[concat('Θμ', '二头肌', 'α')]"
//@    },
output interpolateUnicodeStrings string = 'Θμ二${emojis}头肌${ninjaCat}α'
//@    "interpolateUnicodeStrings": {
//@      "type": "string",
//@      "value": "[format('Θμ二{0}头肌{1}α', variables('emojis'), variables('ninjaCat'))]"
//@    }

// all of these should produce the same string
var surrogate_char      = '𐐷'
//@    "surrogate_char": "𐐷",
var surrogate_codepoint = '\u{10437}'
//@    "surrogate_codepoint": "𐐷",
var surrogate_pairs     = '\u{D801}\u{DC37}'
//@    "surrogate_pairs": "𐐷",

// ascii escapes
var hello = '❆ Hello\u{20}World\u{21} ❁'
//@    "hello": "❆ Hello World! ❁"
