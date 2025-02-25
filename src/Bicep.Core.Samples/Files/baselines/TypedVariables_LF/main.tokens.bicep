@export()
//@[00:01) At |@|
//@[01:07) Identifier |export|
//@[07:08) LeftParen |(|
//@[08:09) RightParen |)|
//@[09:10) NewLine |\n|
var exportedString string = 'foo'
//@[00:03) Identifier |var|
//@[04:18) Identifier |exportedString|
//@[19:25) Identifier |string|
//@[26:27) Assignment |=|
//@[28:33) StringComplete |'foo'|
//@[33:35) NewLine |\n\n|

@export()
//@[00:01) At |@|
//@[01:07) Identifier |export|
//@[07:08) LeftParen |(|
//@[08:09) RightParen |)|
//@[09:10) NewLine |\n|
var exporteInlineType {
//@[00:03) Identifier |var|
//@[04:21) Identifier |exporteInlineType|
//@[22:23) LeftBrace |{|
//@[23:24) NewLine |\n|
  foo: string
//@[02:05) Identifier |foo|
//@[05:06) Colon |:|
//@[07:13) Identifier |string|
//@[13:14) NewLine |\n|
  bar: int
//@[02:05) Identifier |bar|
//@[05:06) Colon |:|
//@[07:10) Identifier |int|
//@[10:11) NewLine |\n|
} = {
//@[00:01) RightBrace |}|
//@[02:03) Assignment |=|
//@[04:05) LeftBrace |{|
//@[05:06) NewLine |\n|
  foo: 'abc'
//@[02:05) Identifier |foo|
//@[05:06) Colon |:|
//@[07:12) StringComplete |'abc'|
//@[12:13) NewLine |\n|
  bar: 123
//@[02:05) Identifier |bar|
//@[05:06) Colon |:|
//@[07:10) Integer |123|
//@[10:11) NewLine |\n|
}
//@[00:01) RightBrace |}|
//@[01:03) NewLine |\n\n|

type FooType = {
//@[00:04) Identifier |type|
//@[05:12) Identifier |FooType|
//@[13:14) Assignment |=|
//@[15:16) LeftBrace |{|
//@[16:17) NewLine |\n|
  foo: string
//@[02:05) Identifier |foo|
//@[05:06) Colon |:|
//@[07:13) Identifier |string|
//@[13:14) NewLine |\n|
  bar: int
//@[02:05) Identifier |bar|
//@[05:06) Colon |:|
//@[07:10) Identifier |int|
//@[10:11) NewLine |\n|
}
//@[00:01) RightBrace |}|
//@[01:03) NewLine |\n\n|

@export()
//@[00:01) At |@|
//@[01:07) Identifier |export|
//@[07:08) LeftParen |(|
//@[08:09) RightParen |)|
//@[09:10) NewLine |\n|
var exportedTypeRef FooType = {
//@[00:03) Identifier |var|
//@[04:19) Identifier |exportedTypeRef|
//@[20:27) Identifier |FooType|
//@[28:29) Assignment |=|
//@[30:31) LeftBrace |{|
//@[31:32) NewLine |\n|
  foo: 'abc'
//@[02:05) Identifier |foo|
//@[05:06) Colon |:|
//@[07:12) StringComplete |'abc'|
//@[12:13) NewLine |\n|
  bar: 123
//@[02:05) Identifier |bar|
//@[05:06) Colon |:|
//@[07:10) Integer |123|
//@[10:11) NewLine |\n|
}
//@[00:01) RightBrace |}|
//@[01:03) NewLine |\n\n|

var unExported FooType = {
//@[00:03) Identifier |var|
//@[04:14) Identifier |unExported|
//@[15:22) Identifier |FooType|
//@[23:24) Assignment |=|
//@[25:26) LeftBrace |{|
//@[26:27) NewLine |\n|
  foo: 'abc'
//@[02:05) Identifier |foo|
//@[05:06) Colon |:|
//@[07:12) StringComplete |'abc'|
//@[12:13) NewLine |\n|
  bar: 123
//@[02:05) Identifier |bar|
//@[05:06) Colon |:|
//@[07:10) Integer |123|
//@[10:11) NewLine |\n|
}
//@[00:01) RightBrace |}|
//@[01:02) NewLine |\n|

//@[00:00) EndOfFile ||
