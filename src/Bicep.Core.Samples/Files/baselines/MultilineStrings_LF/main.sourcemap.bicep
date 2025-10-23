var name = 'Anthony'
//@    "name": "Anthony",
var multilineInterpolation = $'''
//@    "multilineInterpolation": "[format('Hello,\nmy\nname is\n{0}\n', variables('name'))]",
Hello,
my
name is
${name}
'''

var complexMultilineInterpolation = $$$'''
//@    "complexMultilineInterpolation": "[format('${{name}}\n$${{name}}\n{0}\n${1}\n', variables('name'), variables('name'))]",
${name}
$${name}
$$${name}
$$$${name}
'''

var interpMultiEmpty = $''''''
//@    "interpMultiEmpty": "",
var interp1Multi = $'''
//@    "interp1Multi": "[format('abc{0}def\n', 123)]",
abc${123}def
'''
var interp2Multi = $'''${123}def'''
//@    "interp2Multi": "[format('{0}def', 123)]",
var interp3Multi = $$'''abc$${123}'''
//@    "interp3Multi": "[format('abc{0}', 123)]",
var interp4Multi = $'''abc${123}${456}jk$l${789}p$'''
//@    "interp4Multi": "[format('abc{0}{1}jk$l{2}p$', 123, 456, 789)]",
var doubleInterpMulti = $'''abc${'def${123}'}_${'${456}${789}'}'''
//@    "doubleInterpMulti": "[format('abc{0}_{1}', format('def{0}', 123), format('{0}{1}', 456, 789))]",
var curliesInInterpMulti = $'''{${123}{0}${true}}'''
//@    "curliesInInterpMulti": "[format('{{{0}{{0}}{1}}}', 123, true())]"

