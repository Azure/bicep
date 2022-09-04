param _ string = '_'
param _1 string = '_1'
param _123 string = '123'
param my_bad string = 'my bad'
param _doNotStripUnderscoresFromUserNames string = '_doNotStripUnderscoresFromUserNames'
param doNotStripUnderscoresFromUserNames_ string = 'doNotStripUnderscoresFromUserNames_'
param _doNotStripUnderscoresFromUserNames_ string = '_doNotStripUnderscoresFromUserNames_'

var _doNotStripUnderscoresFromUserNames__var = '_doNotStripUnderscoresFromUserNames_'

module nestedInnerDeployment './nested_nestedInnerDeployment.bicep' = {
  name: 'nestedInnerDeployment'
  params: {
    '_doNotStripUnderscoresFromUserNames_': _doNotStripUnderscoresFromUserNames_
//@[4:42) [prefer-unquoted-property-names (Warning)] Property names that are valid identifiers should be declared without quotation marks and accessed using dot notation. (CodeDescription: bicep core(https://aka.ms/bicep/linter/prefer-unquoted-property-names)) |'_doNotStripUnderscoresFromUserNames_'|
  }
}

output output1 string = _
output output2 string = _1
output output3 string = _123
output output4 string = my_bad
output output5 string = _doNotStripUnderscoresFromUserNames
output output6 string = doNotStripUnderscoresFromUserNames_
output output7 string = _doNotStripUnderscoresFromUserNames_
output output8 string = concat(_doNotStripUnderscoresFromUserNames__var)
output _doNotStripUnderscoresFromUserNames_ string = '_doNotStripUnderscoresFromUserNames_'
