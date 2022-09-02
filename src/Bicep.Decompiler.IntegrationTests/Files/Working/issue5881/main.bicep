var foo = 'abc'
var bar = guid(subscription().id, 'xxxx', foo)
//@[4:7) [no-unused-vars (Warning)] Variable "bar" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |bar|
var abc = guid('blah')
//@[4:7) [no-unused-vars (Warning)] Variable "abc" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |abc|
var def = {
//@[4:7) [no-unused-vars (Warning)] Variable "def" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |def|
  '1234': '1234'
  '${guid('blah')}': guid('blah')
}
