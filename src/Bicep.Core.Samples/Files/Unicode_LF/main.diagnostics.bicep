var emojis = '💪😊😈🍕☕'
var ninjaCat = '🐱‍👤'

/*
朝辞白帝彩云间
千里江陵一日还
两岸猿声啼不住
轻舟已过万重山
*/

// greek letters in comment: Π π Φ φ plus emoji 😎
var variousAlphabets = {
//@[4:20) [no-unused-vars (Warning)] Variable "variousAlphabets" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |variousAlphabets|
  'α': 'α'
  'Ωω': [
    'Θμ'
  ]
  'ążźćłóę': 'Cześć!'
  'áéóúñü': '¡Hola!'

  '二头肌': '二头肌'
}

output concatUnicodeStrings string = concat('Θμ', '二头肌', 'α')
//@[37:61) [prefer-interpolation (Warning)] Use string interpolation instead of the concat function. (CodeDescription: bicep core(https://aka.ms/bicep/linter/prefer-interpolation)) |concat('Θμ', '二头肌', 'α')|
output interpolateUnicodeStrings string = 'Θμ二${emojis}头肌${ninjaCat}α'

// all of these should produce the same string
var surrogate_char      = '𐐷'
//@[4:18) [no-unused-vars (Warning)] Variable "surrogate_char" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |surrogate_char|
var surrogate_codepoint = '\u{10437}'
//@[4:23) [no-unused-vars (Warning)] Variable "surrogate_codepoint" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |surrogate_codepoint|
var surrogate_pairs     = '\u{D801}\u{DC37}'
//@[4:19) [no-unused-vars (Warning)] Variable "surrogate_pairs" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |surrogate_pairs|

// ascii escapes
var hello = '❆ Hello\u{20}World\u{21} ❁'
//@[4:9) [no-unused-vars (Warning)] Variable "hello" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |hello|
