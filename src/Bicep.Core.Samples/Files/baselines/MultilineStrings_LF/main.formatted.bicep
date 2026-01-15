var name = 'Anthony'
var multilineInterpolation = $'''
Hello,
my
name is
${name}
'''

var complexMultilineInterpolation = $$$'''
${name}
$${name}
$$${name}
$$$${name}
'''

var interpMultiEmpty = $''''''
var interp1Multi = $'''
abc${123}def
'''
var interp2Multi = $'''${123}def'''
var interp3Multi = $$'''abc$${123}'''
var interp4Multi = $'''abc${123}${456}jk$l${789}p$'''
var doubleInterpMulti = $'''abc${'def${123}'}_${'${456}${789}'}'''
var curliesInInterpMulti = $'''{${123}{0}${true}}'''
