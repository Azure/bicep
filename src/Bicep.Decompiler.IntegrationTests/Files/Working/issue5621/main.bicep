var emptyObject = {
//@[4:15) [no-unused-vars (Warning)] Variable "emptyObject" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |emptyObject|
}
var simpleObject = {
//@[4:16) [no-unused-vars (Warning)] Variable "simpleObject" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |simpleObject|
  foo: 'bar'
}
var emptyArray = []
//@[4:14) [no-unused-vars (Warning)] Variable "emptyArray" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |emptyArray|
var singletonList = [
//@[4:17) [no-unused-vars (Warning)] Variable "singletonList" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |singletonList|
  'boo!'
]
var krispies = [
//@[4:12) [no-unused-vars (Warning)] Variable "krispies" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |krispies|
  'snap'
  'crackle'
  'pop'
]
var nestedArrays = [
//@[4:16) [no-unused-vars (Warning)] Variable "nestedArrays" is declared but never used. (CodeDescription: bicep core(https://aka.ms/bicep/linter/no-unused-vars)) |nestedArrays|
  [
    1
    2
  ]
  {
    key1: [
      3
      4
    ]
    key2: {
      nestedKey1: [
        5
        6
      ]
      nestedKey2: {
      }
    }
  }
]
