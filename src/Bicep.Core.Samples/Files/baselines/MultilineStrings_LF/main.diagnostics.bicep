var name = 'Anthony'
var multilineInterpolation = $'''
//@[4:26) [no-unused-vars (Warning)] Variable "multilineInterpolation" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |multilineInterpolation|
Hello,
my
name is
${name}
'''

var complexMultilineInterpolation = $$$'''
//@[4:33) [no-unused-vars (Warning)] Variable "complexMultilineInterpolation" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |complexMultilineInterpolation|
${name}
$${name}
$$${name}
$$$${name}
'''

var interpMultiEmpty = $''''''
//@[4:20) [no-unused-vars (Warning)] Variable "interpMultiEmpty" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |interpMultiEmpty|
var interp1Multi = $'''
//@[4:16) [no-unused-vars (Warning)] Variable "interp1Multi" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |interp1Multi|
abc${123}def
'''
var interp2Multi = $'''${123}def'''
//@[4:16) [no-unused-vars (Warning)] Variable "interp2Multi" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |interp2Multi|
var interp3Multi = $$'''abc$${123}'''
//@[4:16) [no-unused-vars (Warning)] Variable "interp3Multi" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |interp3Multi|
var interp4Multi = $'''abc${123}${456}jk$l${789}p$'''
//@[4:16) [no-unused-vars (Warning)] Variable "interp4Multi" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |interp4Multi|
var doubleInterpMulti = $'''abc${'def${123}'}_${'${456}${789}'}'''
//@[4:21) [no-unused-vars (Warning)] Variable "doubleInterpMulti" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |doubleInterpMulti|
var curliesInInterpMulti = $'''{${123}{0}${true}}'''
//@[4:24) [no-unused-vars (Warning)] Variable "curliesInInterpMulti" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |curliesInInterpMulti|

