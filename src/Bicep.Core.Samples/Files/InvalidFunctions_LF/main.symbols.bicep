func useRuntimeFunction = () => reference('foo').bar
//@[05:23) Function useRuntimeFunction. Type: () => any. Declaration start char: 0, length: 52

func constFunc = () => 'A'
//@[05:14) Function constFunc. Type: () => 'A'. Declaration start char: 0, length: 26
func funcWithOtherFuncRef = () => constFunc()
//@[05:25) Function funcWithOtherFuncRef. Type: () => string. Declaration start char: 0, length: 45

func invalidType = (string input) => input
//@[27:32) Local input. Type: string. Declaration start char: 20, length: 12
//@[05:16) Function invalidType. Type: string => string. Declaration start char: 0, length: 42

output invalidType string = invalidType(true)
//@[07:18) Output invalidType. Type: string. Declaration start char: 0, length: 45

func madeUpTypeArgs = (notAType a, alsoNotAType b) => '${a}-${b}'
//@[32:33) Local a. Type: error. Declaration start char: 23, length: 10
//@[48:49) Local b. Type: error. Declaration start char: 35, length: 14
//@[05:19) Function madeUpTypeArgs. Type: error. Declaration start char: 0, length: 65

func noLambda = ('foo') => ''
//@[22:22) Local <missing>. Type: error. Declaration start char: 17, length: 5
//@[05:13) Function noLambda. Type: error. Declaration start char: 0, length: 29

func noLambda2 = ('foo' sdf) => ''
//@[24:27) Local sdf. Type: error. Declaration start char: 18, length: 9
//@[05:14) Function noLambda2. Type: error. Declaration start char: 0, length: 34

func noLambda3 = 'asdf'
//@[05:14) Function noLambda3. Type: error. Declaration start char: 0, length: 23

func argLengthMismatch = (string a, string b, string c) => [a, b, c]
//@[33:34) Local a. Type: string. Declaration start char: 26, length: 8
//@[43:44) Local b. Type: string. Declaration start char: 36, length: 8
//@[53:54) Local c. Type: string. Declaration start char: 46, length: 8
//@[05:22) Function argLengthMismatch. Type: (string, string, string) => string. Declaration start char: 0, length: 68
var sdf = argLengthMismatch('asdf')
//@[04:07) Variable sdf. Type: error. Declaration start char: 0, length: 35

var asdfwdf = noLambda('asd')
//@[04:11) Variable asdfwdf. Type: error. Declaration start char: 0, length: 29

func buildUrl = (bool https, string hostname, string path) => '${https ? 'https' : 'http'}://${hostname}${empty(path) ? '' : '/${path}'}'
//@[22:27) Local https. Type: bool. Declaration start char: 17, length: 10
//@[36:44) Local hostname. Type: string. Declaration start char: 29, length: 15
//@[53:57) Local path. Type: string. Declaration start char: 46, length: 11
//@[05:13) Function buildUrl. Type: (bool, string, string) => string. Declaration start char: 0, length: 137

output foo array = buildUrl(true, 'google.com', 'search')
//@[07:10) Output foo. Type: array. Declaration start char: 0, length: 57

func sayHello = (string name) => 'Hi ${name}!'
//@[24:28) Local name. Type: string. Declaration start char: 17, length: 11
//@[05:13) Function sayHello. Type: string => string. Declaration start char: 0, length: 46
output hellos array = map(['Evie', 'Casper'], sayHello) // this syntax not supported currently, but should it be?
//@[07:13) Output hellos. Type: array. Declaration start char: 0, length: 55

func sayHelloBadNewlines = (
//@[05:24) Function sayHelloBadNewlines. Type: error. Declaration start char: 0, length: 28
  string name) => 'Hi ${name}!'

