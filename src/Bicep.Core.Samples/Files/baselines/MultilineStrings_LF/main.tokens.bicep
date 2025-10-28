var name = 'Anthony'
//@[00:03) Identifier |var|
//@[04:08) Identifier |name|
//@[09:10) Assignment |=|
//@[11:20) StringComplete |'Anthony'|
//@[20:21) NewLine |\n|
var multilineInterpolation = $'''
//@[00:03) Identifier |var|
//@[04:26) Identifier |multilineInterpolation|
//@[27:28) Assignment |=|
//@[29:54) StringLeftPiece |$'''\nHello,\nmy\nname is\n${|
Hello,
my
name is
${name}
//@[02:06) Identifier |name|
//@[06:11) StringRightPiece |}\n'''|
'''
//@[03:05) NewLine |\n\n|

var complexMultilineInterpolation = $$$'''
//@[00:03) Identifier |var|
//@[04:33) Identifier |complexMultilineInterpolation|
//@[34:35) Assignment |=|
//@[36:64) StringLeftPiece |$$$'''\n${name}\n$${name}\n$$${|
${name}
$${name}
$$${name}
//@[04:08) Identifier |name|
//@[08:15) StringMiddlePiece |}\n$$$${|
$$$${name}
//@[05:09) Identifier |name|
//@[09:14) StringRightPiece |}\n'''|
'''
//@[03:05) NewLine |\n\n|

var interpMultiEmpty = $''''''
//@[00:03) Identifier |var|
//@[04:20) Identifier |interpMultiEmpty|
//@[21:22) Assignment |=|
//@[23:30) StringComplete |$''''''|
//@[30:31) NewLine |\n|
var interp1Multi = $'''
//@[00:03) Identifier |var|
//@[04:16) Identifier |interp1Multi|
//@[17:18) Assignment |=|
//@[19:29) StringLeftPiece |$'''\nabc${|
abc${123}def
//@[05:08) Integer |123|
//@[08:16) StringRightPiece |}def\n'''|
'''
//@[03:04) NewLine |\n|
var interp2Multi = $'''${123}def'''
//@[00:03) Identifier |var|
//@[04:16) Identifier |interp2Multi|
//@[17:18) Assignment |=|
//@[19:25) StringLeftPiece |$'''${|
//@[25:28) Integer |123|
//@[28:35) StringRightPiece |}def'''|
//@[35:36) NewLine |\n|
var interp3Multi = $$'''abc$${123}'''
//@[00:03) Identifier |var|
//@[04:16) Identifier |interp3Multi|
//@[17:18) Assignment |=|
//@[19:30) StringLeftPiece |$$'''abc$${|
//@[30:33) Integer |123|
//@[33:37) StringRightPiece |}'''|
//@[37:38) NewLine |\n|
var interp4Multi = $'''abc${123}${456}jk$l${789}p$'''
//@[00:03) Identifier |var|
//@[04:16) Identifier |interp4Multi|
//@[17:18) Assignment |=|
//@[19:28) StringLeftPiece |$'''abc${|
//@[28:31) Integer |123|
//@[31:34) StringMiddlePiece |}${|
//@[34:37) Integer |456|
//@[37:44) StringMiddlePiece |}jk$l${|
//@[44:47) Integer |789|
//@[47:53) StringRightPiece |}p$'''|
//@[53:54) NewLine |\n|
var doubleInterpMulti = $'''abc${'def${123}'}_${'${456}${789}'}'''
//@[00:03) Identifier |var|
//@[04:21) Identifier |doubleInterpMulti|
//@[22:23) Assignment |=|
//@[24:33) StringLeftPiece |$'''abc${|
//@[33:39) StringLeftPiece |'def${|
//@[39:42) Integer |123|
//@[42:44) StringRightPiece |}'|
//@[44:48) StringMiddlePiece |}_${|
//@[48:51) StringLeftPiece |'${|
//@[51:54) Integer |456|
//@[54:57) StringMiddlePiece |}${|
//@[57:60) Integer |789|
//@[60:62) StringRightPiece |}'|
//@[62:66) StringRightPiece |}'''|
//@[66:67) NewLine |\n|
var curliesInInterpMulti = $'''{${123}{0}${true}}'''
//@[00:03) Identifier |var|
//@[04:24) Identifier |curliesInInterpMulti|
//@[25:26) Assignment |=|
//@[27:34) StringLeftPiece |$'''{${|
//@[34:37) Integer |123|
//@[37:43) StringMiddlePiece |}{0}${|
//@[43:47) TrueKeyword |true|
//@[47:52) StringRightPiece |}}'''|
//@[52:53) NewLine |\n|

//@[00:00) EndOfFile ||
