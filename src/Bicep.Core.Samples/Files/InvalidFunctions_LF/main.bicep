func useRuntimeFunction = () => reference('foo').bar

func funcA = () => 'A'
func funcB = () => funcA()

func invalidType = (string input) => input

output invalidType string = invalidType(true)
