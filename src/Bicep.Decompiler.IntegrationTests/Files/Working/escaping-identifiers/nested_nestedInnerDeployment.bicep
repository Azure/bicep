param _doNotStripUnderscoresFromUserNames_ string

var _doNotStripUnderscoresFromUserNames__var = '_doNotStripUnderscoresFromUserNames_'

output _doNotStripUnderscoresFromUserNames_ string = concat(_doNotStripUnderscoresFromUserNames_, _doNotStripUnderscoresFromUserNames__var)
//@[53:139) [prefer-interpolation (Warning)] Use string interpolation instead of the concat function. (CodeDescription: bicep core(https://aka.ms/bicep/linter/prefer-interpolation)) |concat(_doNotStripUnderscoresFromUserNames_, _doNotStripUnderscoresFromUserNames__var)|
