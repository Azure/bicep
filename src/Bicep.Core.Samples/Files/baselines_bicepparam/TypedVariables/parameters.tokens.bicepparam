using 'main.bicep'
//@[00:05) Identifier |using|
//@[06:18) StringComplete |'main.bicep'|
//@[18:20) NewLine |\n\n|

import { FooType } from './types.bicep'
//@[00:06) Identifier |import|
//@[07:08) LeftBrace |{|
//@[09:16) Identifier |FooType|
//@[17:18) RightBrace |}|
//@[19:23) Identifier |from|
//@[24:39) StringComplete |'./types.bicep'|
//@[39:41) NewLine |\n\n|

var imported FooType = {
//@[00:03) Identifier |var|
//@[04:12) Identifier |imported|
//@[13:20) Identifier |FooType|
//@[21:22) Assignment |=|
//@[23:24) LeftBrace |{|
//@[24:25) NewLine |\n|
  stringProp: 'adfadf'
//@[02:12) Identifier |stringProp|
//@[12:13) Colon |:|
//@[14:22) StringComplete |'adfadf'|
//@[22:23) NewLine |\n|
  intProp: 123
//@[02:09) Identifier |intProp|
//@[09:10) Colon |:|
//@[11:14) Integer |123|
//@[14:15) NewLine |\n|
}
//@[00:01) RightBrace |}|
//@[01:03) NewLine |\n\n|

var inline {
//@[00:03) Identifier |var|
//@[04:10) Identifier |inline|
//@[11:12) LeftBrace |{|
//@[12:13) NewLine |\n|
  stringProp: string
//@[02:12) Identifier |stringProp|
//@[12:13) Colon |:|
//@[14:20) Identifier |string|
//@[20:21) NewLine |\n|
  intProp: int
//@[02:09) Identifier |intProp|
//@[09:10) Colon |:|
//@[11:14) Identifier |int|
//@[14:15) NewLine |\n|
} = {
//@[00:01) RightBrace |}|
//@[02:03) Assignment |=|
//@[04:05) LeftBrace |{|
//@[05:06) NewLine |\n|
  stringProp: 'asdaosd'
//@[02:12) Identifier |stringProp|
//@[12:13) Colon |:|
//@[14:23) StringComplete |'asdaosd'|
//@[23:24) NewLine |\n|
  intProp: 123
//@[02:09) Identifier |intProp|
//@[09:10) Colon |:|
//@[11:14) Integer |123|
//@[14:15) NewLine |\n|
}
//@[00:01) RightBrace |}|
//@[01:03) NewLine |\n\n|

type InFileType = {
//@[00:04) Identifier |type|
//@[05:15) Identifier |InFileType|
//@[16:17) Assignment |=|
//@[18:19) LeftBrace |{|
//@[19:20) NewLine |\n|
  stringProp: string
//@[02:12) Identifier |stringProp|
//@[12:13) Colon |:|
//@[14:20) Identifier |string|
//@[20:21) NewLine |\n|
  intProp: int
//@[02:09) Identifier |intProp|
//@[09:10) Colon |:|
//@[11:14) Identifier |int|
//@[14:15) NewLine |\n|
}
//@[00:01) RightBrace |}|
//@[01:03) NewLine |\n\n|

var inFile InFileType = {
//@[00:03) Identifier |var|
//@[04:10) Identifier |inFile|
//@[11:21) Identifier |InFileType|
//@[22:23) Assignment |=|
//@[24:25) LeftBrace |{|
//@[25:26) NewLine |\n|
  stringProp: 'asdaosd'
//@[02:12) Identifier |stringProp|
//@[12:13) Colon |:|
//@[14:23) StringComplete |'asdaosd'|
//@[23:24) NewLine |\n|
  intProp: 123
//@[02:09) Identifier |intProp|
//@[09:10) Colon |:|
//@[11:14) Integer |123|
//@[14:15) NewLine |\n|
}
//@[00:01) RightBrace |}|
//@[01:02) NewLine |\n|

//@[00:00) EndOfFile ||
