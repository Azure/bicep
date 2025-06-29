using 'main.bicep'

import { FooType } from './types.bicep'
//@[9:16) TypeAlias FooType. Type: Type<{ stringProp: string, intProp: int }>. Declaration start char: 9, length: 7

var imported FooType = {
//@[4:12) Variable imported. Type: { stringProp: string, intProp: int }. Declaration start char: 0, length: 64
  stringProp: 'adfadf'
  intProp: 123
}

var inline {
//@[4:10) Variable inline. Type: { stringProp: string, intProp: int }. Declaration start char: 0, length: 95
  stringProp: string
  intProp: int
} = {
  stringProp: 'asdaosd'
  intProp: 123
}

type InFileType = {
//@[5:15) TypeAlias InFileType. Type: Type<{ stringProp: string, intProp: int }>. Declaration start char: 0, length: 57
  stringProp: string
  intProp: int
}

var inFile InFileType = {
//@[4:10) Variable inFile. Type: { stringProp: string, intProp: int }. Declaration start char: 0, length: 66
  stringProp: 'asdaosd'
  intProp: 123
}

