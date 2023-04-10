func useRuntimeFunction = () => reference('foo').bar
//@[05:23) Variable useRuntimeFunction. Type: () => any. Declaration start char: 0, length: 52

func funcA = () => 'A'
//@[05:10) Variable funcA. Type: () => 'A'. Declaration start char: 0, length: 22
func funcB = () => funcA()
//@[05:10) Variable funcB. Type: () => any. Declaration start char: 0, length: 26

func invalidType = (string input) => input
//@[27:32) Local input. Type: string. Declaration start char: 20, length: 12
//@[05:16) Variable invalidType. Type: string => string. Declaration start char: 0, length: 42

output invalidType string = invalidType(true)
//@[07:18) Output invalidType. Type: string. Declaration start char: 0, length: 45

