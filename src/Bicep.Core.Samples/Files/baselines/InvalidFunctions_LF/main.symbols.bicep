func useRuntimeFunction() string => reference('foo').bar
//@[05:23) Function useRuntimeFunction. Type: () => any. Declaration start char: 0, length: 56

func missingArgType(input) string => input
//@[20:25) Local input. Type: any. Declaration start char: 20, length: 5
//@[05:19) Function missingArgType. Type: any => any. Declaration start char: 0, length: 42

func missingOutputType(input string) => input
//@[23:28) Local input. Type: string. Declaration start char: 23, length: 12
//@[05:22) Function missingOutputType. Type: string => any. Declaration start char: 0, length: 45

func invalidType(input string) string => input
//@[17:22) Local input. Type: string. Declaration start char: 17, length: 12
//@[05:16) Function invalidType. Type: string => string. Declaration start char: 0, length: 46

output invalidType string = invalidType(true)
//@[07:18) Output invalidType. Type: string. Declaration start char: 0, length: 45

func madeUpTypeArgs(a notAType, b alsoNotAType) string => '${a}-${b}'
//@[20:21) Local a. Type: error. Declaration start char: 20, length: 10
//@[32:33) Local b. Type: error. Declaration start char: 32, length: 14
//@[05:19) Function madeUpTypeArgs. Type: (error, error) => string. Declaration start char: 0, length: 69

func noLambda('foo') string => ''
//@[14:14) Local <missing>. Type: 'foo'. Declaration start char: 14, length: 5
//@[05:13) Function noLambda. Type: 'foo' => ''. Declaration start char: 0, length: 33

func noLambda2 = (sdf 'foo') string => ''
//@[05:14) Function noLambda2. Type: error. Declaration start char: 0, length: 41

func noLambda3 = string 'asdf'
//@[05:14) Function noLambda3. Type: error. Declaration start char: 0, length: 30

func argLengthMismatch(a string, b string, c string) array => ([a, b, c])
//@[23:24) Local a. Type: string. Declaration start char: 23, length: 8
//@[33:34) Local b. Type: string. Declaration start char: 33, length: 8
//@[43:44) Local c. Type: string. Declaration start char: 43, length: 8
//@[05:22) Function argLengthMismatch. Type: (string, string, string) => string[]. Declaration start char: 0, length: 73
var sdf = argLengthMismatch('asdf')
//@[04:07) Variable sdf. Type: error. Declaration start char: 0, length: 35

var asdfwdf = noLambda('asd')
//@[04:11) Variable asdfwdf. Type: error. Declaration start char: 0, length: 29

func sayHello(name string) string => 'Hi ${name}!'
//@[14:18) Local name. Type: string. Declaration start char: 14, length: 11
//@[05:13) Function sayHello. Type: string => string. Declaration start char: 0, length: 50
output hellos array = map(['Evie', 'Casper'], sayHello) // this syntax not supported currently, but should it be?
//@[07:13) Output hellos. Type: array. Declaration start char: 0, length: 55

func sayHelloBadNewlines(
//@[05:24) Function sayHelloBadNewlines. Type: string => string. Declaration start char: 0, length: 64
  name string) string => 'Hi ${name}!'
//@[02:06) Local name. Type: string. Declaration start char: 2, length: 11

type validStringLiteralUnion = 'foo'|'bar'|'baz'
//@[05:28) TypeAlias validStringLiteralUnion. Type: Type<'bar' | 'baz' | 'foo'>. Declaration start char: 0, length: 48
func invalidArgs(a validStringLiteralUnion, b string) string => a
//@[17:18) Local a. Type: 'bar' | 'baz' | 'foo'. Declaration start char: 17, length: 25
//@[44:45) Local b. Type: string. Declaration start char: 44, length: 8
//@[05:16) Function invalidArgs. Type: (('bar' | 'baz' | 'foo'), string) => ('bar' | 'baz' | 'foo'). Declaration start char: 0, length: 65
func invalidOutput() validStringLiteralUnion => 'foo'
//@[05:18) Function invalidOutput. Type: () => 'foo'. Declaration start char: 0, length: 53

func recursive() string => recursive()
//@[05:14) Function recursive. Type: () => string. Declaration start char: 0, length: 38

func recursiveA() string => recursiveB()
//@[05:15) Function recursiveA. Type: () => string. Declaration start char: 0, length: 40
func recursiveB() string => recursiveA()
//@[05:15) Function recursiveB. Type: () => string. Declaration start char: 0, length: 40

func onlyComma(,) string => 'foo'
//@[15:15) Local <missing>. Type: any. Declaration start char: 15, length: 0
//@[05:14) Function onlyComma. Type: any => 'foo'. Declaration start char: 0, length: 33
func trailingCommas(a string,,) string => 'foo'
//@[20:21) Local a. Type: string. Declaration start char: 20, length: 8
//@[29:29) Local <missing>. Type: any. Declaration start char: 29, length: 0
//@[05:19) Function trailingCommas. Type: (string, any) => 'foo'. Declaration start char: 0, length: 47
func multiLineOnly(
//@[05:18) Function multiLineOnly. Type: string => any. Declaration start char: 0, length: 58
  a string
//@[02:03) Local a. Type: string. Declaration start char: 2, length: 8
  b string) string => 'foo'

func multiLineTrailingCommas(
//@[05:28) Function multiLineTrailingCommas. Type: (string, any) => 'foo'. Declaration start char: 0, length: 62
  a string,
//@[02:03) Local a. Type: string. Declaration start char: 2, length: 8
  ,) string => 'foo'
//@[02:02) Local <missing>. Type: any. Declaration start char: 2, length: 0

func lineBeforeComma(
//@[05:20) Function lineBeforeComma. Type: (string, string) => 'foo'. Declaration start char: 0, length: 61
  a string
//@[02:03) Local a. Type: string. Declaration start char: 2, length: 8
  ,b string) string => 'foo'
//@[03:04) Local b. Type: string. Declaration start char: 3, length: 8

