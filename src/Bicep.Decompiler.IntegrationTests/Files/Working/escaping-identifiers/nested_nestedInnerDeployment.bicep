param _doNotStripUnderscoresFromUserNames_ string

var _doNotStripUnderscoresFromUserNames__var = '_doNotStripUnderscoresFromUserNames_'

output _doNotStripUnderscoresFromUserNames_ string = concat(
//@[53:145) [prefer-interpolation (Warning)] Use string interpolation instead of the concat function. (CodeDescription: bicep core(https://aka.ms/bicep/linter/prefer-interpolation)) |concat(\n  _doNotStripUnderscoresFromUserNames_,\n  _doNotStripUnderscoresFromUserNames__var\n)|
  _doNotStripUnderscoresFromUserNames_,
  _doNotStripUnderscoresFromUserNames__var
)

