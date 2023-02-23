var unquoted = {
//@[4:12) [no-unused-vars (Warning)] Variable "unquoted" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |unquoted|
  _artifactsLocation: 123
  _artifactsLocationSasToken: 456
}
var stillQuoted = {
//@[4:15) [no-unused-vars (Warning)] Variable "stillQuoted" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |stillQuoted|
  '123': 123
  '+abc': 456
  '': 789
}
