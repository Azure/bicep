var unquoted = {
//@[4:12) [no-unused-vars (Warning)] Variable "unquoted" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |unquoted|
  _artifactsLocation: 123
  _artifactsLocationSasToken: 456
}
var stillQuoted = {
//@[4:15) [no-unused-vars (Warning)] Variable "stillQuoted" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |stillQuoted|
  '123': 123
  '+abc': 456
  '': 789
}

