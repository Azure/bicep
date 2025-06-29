using 'main.bicep'

import { FooType } from './types.bicep'

var imported FooType = {
  stringProp: 'adfadf'
  intProp: 123
}

var inline {
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
  stringProp: 'asdaosd'
  intProp: 123
}
