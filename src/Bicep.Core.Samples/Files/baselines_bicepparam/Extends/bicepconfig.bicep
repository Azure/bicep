var json = loadJsonContent('foo.json')
func testFunction(b bool) bool => b
@export()
var directExport = json.bar.baz
@export()
var functionExport = testFunction(json.bar.baz)
