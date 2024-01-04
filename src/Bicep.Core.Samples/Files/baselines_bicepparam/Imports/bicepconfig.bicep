var json = loadJsonContent('bicepconfig.json')
func testFunction(b bool) bool => b
@export()
var directExport = json.experimentalFeaturesEnabled.userDefinedFunctions
@export()
var functionExport = testFunction(json.experimentalFeaturesEnabled.userDefinedFunctions)
