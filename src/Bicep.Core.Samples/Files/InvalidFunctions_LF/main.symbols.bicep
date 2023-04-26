func useRuntimeFunction = () string => reference('foo').bar
//@[05:23) Function useRuntimeFunction. Type: () => any. Declaration start char: 0, length: 59

func constFunc = () string => 'A'
//@[05:14) Function constFunc. Type: () => 'A'. Declaration start char: 0, length: 33
func funcWithOtherFuncRef = () string => constFunc()
//@[05:25) Function funcWithOtherFuncRef. Type: () => string. Declaration start char: 0, length: 52

func missingArgType = (input) string => input
//@[23:28) Local input. Type: any. Declaration start char: 23, length: 5
//@[05:19) Function missingArgType. Type: any => any. Declaration start char: 0, length: 45

func missingOutputType = (input string) => input
//@[26:31) Local input. Type: string. Declaration start char: 26, length: 12
//@[05:22) Function missingOutputType. Type: string => any. Declaration start char: 0, length: 48

func invalidType = (input string) string => input
//@[20:25) Local input. Type: string. Declaration start char: 20, length: 12
//@[05:16) Function invalidType. Type: string => string. Declaration start char: 0, length: 49

output invalidType string = invalidType(true)
//@[07:18) Output invalidType. Type: string. Declaration start char: 0, length: 45

func madeUpTypeArgs = (a notAType, b alsoNotAType) string => '${a}-${b}'
//@[23:24) Local a. Type: error. Declaration start char: 23, length: 10
//@[35:36) Local b. Type: error. Declaration start char: 35, length: 14
//@[05:19) Function madeUpTypeArgs. Type: error. Declaration start char: 0, length: 72

func noLambda = ('foo') string => ''
//@[17:17) Local <missing>. Type: error. Declaration start char: 17, length: 5
//@[05:13) Function noLambda. Type: error. Declaration start char: 0, length: 36

func noLambda2 = (sdf 'foo') string => ''
//@[18:21) Local sdf. Type: error. Declaration start char: 18, length: 9
//@[05:14) Function noLambda2. Type: error. Declaration start char: 0, length: 41

func noLambda3 = string 'asdf'
//@[05:14) Function noLambda3. Type: error. Declaration start char: 0, length: 30

func argLengthMismatch = (a string, b string, c string) array => ([a, b, c])
//@[26:27) Local a. Type: string. Declaration start char: 26, length: 8
//@[36:37) Local b. Type: string. Declaration start char: 36, length: 8
//@[46:47) Local c. Type: string. Declaration start char: 46, length: 8
//@[05:22) Function argLengthMismatch. Type: (string, string, string) => string[]. Declaration start char: 0, length: 76
var sdf = argLengthMismatch('asdf')
//@[04:07) Variable sdf. Type: error. Declaration start char: 0, length: 35

var asdfwdf = noLambda('asd')
//@[04:11) Variable asdfwdf. Type: error. Declaration start char: 0, length: 29

func sayHello = (name string) string => 'Hi ${name}!'
//@[17:21) Local name. Type: string. Declaration start char: 17, length: 11
//@[05:13) Function sayHello. Type: string => string. Declaration start char: 0, length: 53
output hellos array = map(['Evie', 'Casper'], sayHello) // this syntax not supported currently, but should it be?
//@[07:13) Output hellos. Type: array. Declaration start char: 0, length: 55

func sayHelloBadNewlines = (
//@[28:28) Local <missing>. Type: any. Declaration start char: 28, length: 0
//@[05:24) Function sayHelloBadNewlines. Type: any => any. Declaration start char: 0, length: 28
  name string) string => 'Hi ${name}!'

