var emojis = '💪😊😈🍕☕'
//@[00:613) ProgramExpression
//@[00:024) ├─DeclaredVariableExpression { Name = emojis }
//@[13:024) | └─StringLiteralExpression { Value = 💪😊😈🍕☕ }
var ninjaCat = '🐱‍👤'
//@[00:022) ├─DeclaredVariableExpression { Name = ninjaCat }
//@[15:022) | └─StringLiteralExpression { Value = 🐱‍👤 }

/*
朝辞白帝彩云间
千里江陵一日还
两岸猿声啼不住
轻舟已过万重山
*/

// greek letters in comment: Π π Φ φ plus emoji 😎
var variousAlphabets = {
//@[00:119) ├─DeclaredVariableExpression { Name = variousAlphabets }
//@[23:119) | └─ObjectExpression
  'α': 'α'
//@[02:010) |   ├─ObjectPropertyExpression
//@[02:005) |   | ├─StringLiteralExpression { Value = α }
//@[07:010) |   | └─StringLiteralExpression { Value = α }
  'Ωω': [
//@[02:022) |   ├─ObjectPropertyExpression
//@[02:006) |   | ├─StringLiteralExpression { Value = Ωω }
//@[08:022) |   | └─ArrayExpression
    'Θμ'
//@[04:008) |   |   └─StringLiteralExpression { Value = Θμ }
  ]
  'ążźćłóę': 'Cześć!'
//@[02:021) |   ├─ObjectPropertyExpression
//@[02:011) |   | ├─StringLiteralExpression { Value = ążźćłóę }
//@[13:021) |   | └─StringLiteralExpression { Value = Cześć! }
  'áéóúñü': '¡Hola!'
//@[02:020) |   ├─ObjectPropertyExpression
//@[02:010) |   | ├─StringLiteralExpression { Value = áéóúñü }
//@[12:020) |   | └─StringLiteralExpression { Value = ¡Hola! }

  '二头肌': '二头肌'
//@[02:014) |   └─ObjectPropertyExpression
//@[02:007) |     ├─StringLiteralExpression { Value = 二头肌 }
//@[09:014) |     └─StringLiteralExpression { Value = 二头肌 }
}

output concatUnicodeStrings string = concat('Θμ', '二头肌', 'α')
//@[00:061) ├─DeclaredOutputExpression { Name = concatUnicodeStrings }
//@[37:061) | └─FunctionCallExpression { Name = concat }
//@[44:048) |   ├─StringLiteralExpression { Value = Θμ }
//@[50:055) |   ├─StringLiteralExpression { Value = 二头肌 }
//@[57:060) |   └─StringLiteralExpression { Value = α }
output interpolateUnicodeStrings string = 'Θμ二${emojis}头肌${ninjaCat}α'
//@[00:070) └─DeclaredOutputExpression { Name = interpolateUnicodeStrings }
//@[42:070)   └─InterpolatedStringExpression
//@[48:054)     ├─VariableReferenceExpression { Variable = emojis }
//@[59:067)     └─VariableReferenceExpression { Variable = ninjaCat }

// all of these should produce the same string
var surrogate_char      = '𐐷'
//@[00:030) ├─DeclaredVariableExpression { Name = surrogate_char }
//@[26:030) | └─StringLiteralExpression { Value = 𐐷 }
var surrogate_codepoint = '\u{10437}'
//@[00:037) ├─DeclaredVariableExpression { Name = surrogate_codepoint }
//@[26:037) | └─StringLiteralExpression { Value = 𐐷 }
var surrogate_pairs     = '\u{D801}\u{DC37}'
//@[00:044) ├─DeclaredVariableExpression { Name = surrogate_pairs }
//@[26:044) | └─StringLiteralExpression { Value = 𐐷 }

// ascii escapes
var hello = '❆ Hello\u{20}World\u{21} ❁'
//@[00:040) ├─DeclaredVariableExpression { Name = hello }
//@[12:040) | └─StringLiteralExpression { Value = ❆ Hello World! ❁ }
