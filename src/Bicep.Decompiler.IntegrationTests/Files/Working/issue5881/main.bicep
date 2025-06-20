var foo = 'abc'
var bar = guid(subscription().id, 'xxxx', foo)
//@[4:7) [no-unused-vars (Warning)] Variable "bar" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |bar|
var abc = guid('blah')
//@[4:7) [no-unused-vars (Warning)] Variable "abc" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |abc|
var def = {
//@[4:7) [no-unused-vars (Warning)] Variable "def" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |def|
  '1234': '1234'
  '${guid('blah')}': guid('blah')
}

