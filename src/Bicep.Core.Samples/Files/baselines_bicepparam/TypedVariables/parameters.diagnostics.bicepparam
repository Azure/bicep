using 'main.bicep'

import { FooType } from './types.bicep'

var imported FooType = {
//@[4:12) [no-unused-vars (Warning)] Variable "imported" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |imported|
  stringProp: 'adfadf'
  intProp: 123
}

var inline {
//@[4:10) [no-unused-vars (Warning)] Variable "inline" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |inline|
  stringProp: string
  intProp: int
} = {
  stringProp: 'asdaosd'
  intProp: 123
}

type InFileType = {
  stringProp: string
  intProp: int
}

var inFile InFileType = {
//@[4:10) [no-unused-vars (Warning)] Variable "inFile" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |inFile|
  stringProp: 'asdaosd'
  intProp: 123
}

