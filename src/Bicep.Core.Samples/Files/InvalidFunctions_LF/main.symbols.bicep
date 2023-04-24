func useRuntimeFunction = () => string reference('foo').bar
//@[05:23) Function useRuntimeFunction. Type: () => any. Declaration start char: 0, length: 59

func constFunc = () => string 'A'
//@[05:14) Function constFunc. Type: () => 'A'. Declaration start char: 0, length: 33
func funcWithOtherFuncRef = () => string constFunc()
//@[05:25) Function funcWithOtherFuncRef. Type: () => string. Declaration start char: 0, length: 52

func invalidType = (string input) => string input
//@[27:32) Local input. Type: string. Declaration start char: 20, length: 12
//@[05:16) Function invalidType. Type: string => string. Declaration start char: 0, length: 49

output invalidType string = invalidType(true)
//@[07:18) Output invalidType. Type: string. Declaration start char: 0, length: 45

func madeUpTypeArgs = (notAType a, alsoNotAType b) => string '${a}-${b}'
//@[32:33) Local a. Type: error. Declaration start char: 23, length: 10
//@[48:49) Local b. Type: error. Declaration start char: 35, length: 14
//@[05:19) Function madeUpTypeArgs. Type: error. Declaration start char: 0, length: 72

func noLambda = ('foo') => string ''
//@[22:22) Local <missing>. Type: error. Declaration start char: 17, length: 5
//@[05:13) Function noLambda. Type: error. Declaration start char: 0, length: 36

func noLambda2 = ('foo' sdf) => string ''
//@[24:27) Local sdf. Type: error. Declaration start char: 18, length: 9
//@[05:14) Function noLambda2. Type: error. Declaration start char: 0, length: 41

func noLambda3 = string 'asdf'
//@[05:14) Function noLambda3. Type: error. Declaration start char: 0, length: 30

func argLengthMismatch = (string a, string b, string c) => array ([a, b, c])
//@[33:34) Local a. Type: string. Declaration start char: 26, length: 8
//@[43:44) Local b. Type: string. Declaration start char: 36, length: 8
//@[53:54) Local c. Type: string. Declaration start char: 46, length: 8
//@[05:22) Function argLengthMismatch. Type: (string, string, string) => string[]. Declaration start char: 0, length: 76
var sdf = argLengthMismatch('asdf')
//@[04:07) Variable sdf. Type: error. Declaration start char: 0, length: 35

var asdfwdf = noLambda('asd')
//@[04:11) Variable asdfwdf. Type: error. Declaration start char: 0, length: 29

func sayHello = (string name) => string 'Hi ${name}!'
//@[24:28) Local name. Type: string. Declaration start char: 17, length: 11
//@[05:13) Function sayHello. Type: string => string. Declaration start char: 0, length: 53
output hellos array = map(['Evie', 'Casper'], sayHello) // this syntax not supported currently, but should it be?
//@[07:13) Output hellos. Type: array. Declaration start char: 0, length: 55

func sayHelloBadNewlines = (
//@[05:24) Function sayHelloBadNewlines. Type: error. Declaration start char: 0, length: 28
  string name) => string 'Hi ${name}!'

