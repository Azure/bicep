func useRuntimeFunction = () => reference('foo').bar

func funcA = () => 'A'
func funcB = () => funcA()
//@[19:26) [BCP340 (Error)] Symbol "funcA" cannot be used here. Function bodies must only refer to symbols defined as function arguments. (CodeDescription: none) |funcA()|

func invalidType = (string input) => input

output invalidType string = invalidType(true)
//@[40:44) [BCP033 (Error)] Expected a value of type "string" but the provided value is of type "true". (CodeDescription: none) |true|

