////////////////////////////////////////////////////////////////////////////////
//@[80:81) NewLine |\n|
//////////////////////////// Baselines for width 40 ////////////////////////////
//@[80:81) NewLine |\n|
////////////////////////////////////////////////////////////////////////////////
//@[80:81) NewLine |\n|
var w38 = [true, /* xxxxx */ true, 12]      // suffix
//@[00:03) Identifier |var|
//@[04:07) Identifier |w38|
//@[08:09) Assignment |=|
//@[10:11) LeftSquare |[|
//@[11:15) TrueKeyword |true|
//@[15:16) Comma |,|
//@[29:33) TrueKeyword |true|
//@[33:34) Comma |,|
//@[35:37) Integer |12|
//@[37:38) RightSquare |]|
//@[53:54) NewLine |\n|
var w39 = [true, true
//@[00:03) Identifier |var|
//@[04:07) Identifier |w39|
//@[08:09) Assignment |=|
//@[10:11) LeftSquare |[|
//@[11:15) TrueKeyword |true|
//@[15:16) Comma |,|
//@[17:21) TrueKeyword |true|
//@[21:22) NewLine |\n|
    true, true, 123]
//@[04:08) TrueKeyword |true|
//@[08:09) Comma |,|
//@[10:14) TrueKeyword |true|
//@[14:15) Comma |,|
//@[16:19) Integer |123|
//@[19:20) RightSquare |]|
//@[20:21) NewLine |\n|
var w40 =[true
//@[00:03) Identifier |var|
//@[04:07) Identifier |w40|
//@[08:09) Assignment |=|
//@[09:10) LeftSquare |[|
//@[10:14) TrueKeyword |true|
//@[14:15) NewLine |\n|
    true, 1234/* xxxxx */]  // suffix
//@[04:08) TrueKeyword |true|
//@[08:09) Comma |,|
//@[10:14) Integer |1234|
//@[25:26) RightSquare |]|
//@[37:38) NewLine |\n|
var w41 =[ true, true, true, true, 12345 ]
//@[00:03) Identifier |var|
//@[04:07) Identifier |w41|
//@[08:09) Assignment |=|
//@[09:10) LeftSquare |[|
//@[11:15) TrueKeyword |true|
//@[15:16) Comma |,|
//@[17:21) TrueKeyword |true|
//@[21:22) Comma |,|
//@[23:27) TrueKeyword |true|
//@[27:28) Comma |,|
//@[29:33) TrueKeyword |true|
//@[33:34) Comma |,|
//@[35:40) Integer |12345|
//@[41:42) RightSquare |]|
//@[42:43) NewLine |\n|
var w42 =[true, /* xxx */ 12 /* xx */, 1]
//@[00:03) Identifier |var|
//@[04:07) Identifier |w42|
//@[08:09) Assignment |=|
//@[09:10) LeftSquare |[|
//@[10:14) TrueKeyword |true|
//@[14:15) Comma |,|
//@[26:28) Integer |12|
//@[37:38) Comma |,|
//@[39:40) Integer |1|
//@[40:41) RightSquare |]|
//@[41:43) NewLine |\n\n|

var w38_= { foo: true, bar: 1234567
//@[00:03) Identifier |var|
//@[04:08) Identifier |w38_|
//@[08:09) Assignment |=|
//@[10:11) LeftBrace |{|
//@[12:15) Identifier |foo|
//@[15:16) Colon |:|
//@[17:21) TrueKeyword |true|
//@[21:22) Comma |,|
//@[23:26) Identifier |bar|
//@[26:27) Colon |:|
//@[28:35) Integer |1234567|
//@[35:36) NewLine |\n|
} // suffix
//@[00:01) RightBrace |}|
//@[11:12) NewLine |\n|
var        w39_= { foo: true
//@[00:03) Identifier |var|
//@[11:15) Identifier |w39_|
//@[15:16) Assignment |=|
//@[17:18) LeftBrace |{|
//@[19:22) Identifier |foo|
//@[22:23) Colon |:|
//@[24:28) TrueKeyword |true|
//@[28:29) NewLine |\n|
  bar: 12345678 } // suffix
//@[02:05) Identifier |bar|
//@[05:06) Colon |:|
//@[07:15) Integer |12345678|
//@[16:17) RightBrace |}|
//@[27:28) NewLine |\n|
    var w40_= { foo: 1, bar:    1 /* xxxx */ }
//@[04:07) Identifier |var|
//@[08:12) Identifier |w40_|
//@[12:13) Assignment |=|
//@[14:15) LeftBrace |{|
//@[16:19) Identifier |foo|
//@[19:20) Colon |:|
//@[21:22) Integer |1|
//@[22:23) Comma |,|
//@[24:27) Identifier |bar|
//@[27:28) Colon |:|
//@[32:33) Integer |1|
//@[45:46) RightBrace |}|
//@[46:47) NewLine |\n|
var w41_={ foo: true, bar    : 1234567890 }
//@[00:03) Identifier |var|
//@[04:08) Identifier |w41_|
//@[08:09) Assignment |=|
//@[09:10) LeftBrace |{|
//@[11:14) Identifier |foo|
//@[14:15) Colon |:|
//@[16:20) TrueKeyword |true|
//@[20:21) Comma |,|
//@[22:25) Identifier |bar|
//@[29:30) Colon |:|
//@[31:41) Integer |1234567890|
//@[42:43) RightBrace |}|
//@[43:44) NewLine |\n|
var w42_= { foo: true
//@[00:03) Identifier |var|
//@[04:08) Identifier |w42_|
//@[08:09) Assignment |=|
//@[10:11) LeftBrace |{|
//@[12:15) Identifier |foo|
//@[15:16) Colon |:|
//@[17:21) TrueKeyword |true|
//@[21:22) NewLine |\n|
    bar: 12345678901 } // suffix
//@[04:07) Identifier |bar|
//@[07:08) Colon |:|
//@[09:20) Integer |12345678901|
//@[21:22) RightBrace |}|
//@[32:34) NewLine |\n\n|

   var w38__ =    concat('xxxxxx', 'xxxxxx')
//@[03:06) Identifier |var|
//@[07:12) Identifier |w38__|
//@[13:14) Assignment |=|
//@[18:24) Identifier |concat|
//@[24:25) LeftParen |(|
//@[25:33) StringComplete |'xxxxxx'|
//@[33:34) Comma |,|
//@[35:43) StringComplete |'xxxxxx'|
//@[43:44) RightParen |)|
//@[44:45) NewLine |\n|
var w39__ = concat('xxxxxx', 'xxxxxxx'
//@[00:03) Identifier |var|
//@[04:09) Identifier |w39__|
//@[10:11) Assignment |=|
//@[12:18) Identifier |concat|
//@[18:19) LeftParen |(|
//@[19:27) StringComplete |'xxxxxx'|
//@[27:28) Comma |,|
//@[29:38) StringComplete |'xxxxxxx'|
//@[38:39) NewLine |\n|
) // suffix
//@[00:01) RightParen |)|
//@[11:12) NewLine |\n|
var w40__ = concat('xxxxxx',
//@[00:03) Identifier |var|
//@[04:09) Identifier |w40__|
//@[10:11) Assignment |=|
//@[12:18) Identifier |concat|
//@[18:19) LeftParen |(|
//@[19:27) StringComplete |'xxxxxx'|
//@[27:28) Comma |,|
//@[28:29) NewLine |\n|
'xxxxxxxx') // suffix
//@[00:10) StringComplete |'xxxxxxxx'|
//@[10:11) RightParen |)|
//@[21:23) NewLine |\n\n|

var        w41__= concat('xxxxx', 'xxxxxxxxxx')
//@[00:03) Identifier |var|
//@[11:16) Identifier |w41__|
//@[16:17) Assignment |=|
//@[18:24) Identifier |concat|
//@[24:25) LeftParen |(|
//@[25:32) StringComplete |'xxxxx'|
//@[32:33) Comma |,|
//@[34:46) StringComplete |'xxxxxxxxxx'|
//@[46:47) RightParen |)|
//@[47:48) NewLine |\n|
var w42__ = concat('xxxxx', 'xxxxxxxxxxx')
//@[00:03) Identifier |var|
//@[04:09) Identifier |w42__|
//@[10:11) Assignment |=|
//@[12:18) Identifier |concat|
//@[18:19) LeftParen |(|
//@[19:26) StringComplete |'xxxxx'|
//@[26:27) Comma |,|
//@[28:41) StringComplete |'xxxxxxxxxxx'|
//@[41:42) RightParen |)|
//@[42:44) NewLine |\n\n|

var w38___ = true? 'xxxxx' : 'xxxxxx'
//@[00:03) Identifier |var|
//@[04:10) Identifier |w38___|
//@[11:12) Assignment |=|
//@[13:17) TrueKeyword |true|
//@[17:18) Question |?|
//@[19:26) StringComplete |'xxxxx'|
//@[27:28) Colon |:|
//@[29:37) StringComplete |'xxxxxx'|
//@[37:38) NewLine |\n|
var w39___ = true
//@[00:03) Identifier |var|
//@[04:10) Identifier |w39___|
//@[11:12) Assignment |=|
//@[13:17) TrueKeyword |true|
//@[17:18) NewLine |\n|
? 'xxxxxx' : 'xxxxxx' // suffix
//@[00:01) Question |?|
//@[02:10) StringComplete |'xxxxxx'|
//@[11:12) Colon |:|
//@[13:21) StringComplete |'xxxxxx'|
//@[31:32) NewLine |\n|
var w40___ = true ?'xxxxxx' : 'xxxxxxx'
//@[00:03) Identifier |var|
//@[04:10) Identifier |w40___|
//@[11:12) Assignment |=|
//@[13:17) TrueKeyword |true|
//@[18:19) Question |?|
//@[19:27) StringComplete |'xxxxxx'|
//@[28:29) Colon |:|
//@[30:39) StringComplete |'xxxxxxx'|
//@[39:40) NewLine |\n|
var w41___ = true ? 'xxxxxxx' :         'xxxxxxx'
//@[00:03) Identifier |var|
//@[04:10) Identifier |w41___|
//@[11:12) Assignment |=|
//@[13:17) TrueKeyword |true|
//@[18:19) Question |?|
//@[20:29) StringComplete |'xxxxxxx'|
//@[30:31) Colon |:|
//@[40:49) StringComplete |'xxxxxxx'|
//@[49:50) NewLine |\n|
var w42___ = true ? 'xxxxxxx':'xxxxxxxx'
//@[00:03) Identifier |var|
//@[04:10) Identifier |w42___|
//@[11:12) Assignment |=|
//@[13:17) TrueKeyword |true|
//@[18:19) Question |?|
//@[20:29) StringComplete |'xxxxxxx'|
//@[29:30) Colon |:|
//@[30:40) StringComplete |'xxxxxxxx'|
//@[40:42) NewLine |\n\n|

////////////////////////////////////////////////////////////////////////////////
//@[80:81) NewLine |\n|
//////////////////////////// Baselines for width 80 ////////////////////////////
//@[80:81) NewLine |\n|
////////////////////////////////////////////////////////////////////////////////
//@[80:81) NewLine |\n|
var w78 = [true, { foo: 'object width: 37' /* xxx */ }, 'xxxxxxxxxxxxxxxxxxx' ]
//@[00:03) Identifier |var|
//@[04:07) Identifier |w78|
//@[08:09) Assignment |=|
//@[10:11) LeftSquare |[|
//@[11:15) TrueKeyword |true|
//@[15:16) Comma |,|
//@[17:18) LeftBrace |{|
//@[19:22) Identifier |foo|
//@[22:23) Colon |:|
//@[24:42) StringComplete |'object width: 37'|
//@[53:54) RightBrace |}|
//@[54:55) Comma |,|
//@[56:77) StringComplete |'xxxxxxxxxxxxxxxxxxx'|
//@[78:79) RightSquare |]|
//@[79:80) NewLine |\n|
var w79 = [true
//@[00:03) Identifier |var|
//@[04:07) Identifier |w79|
//@[08:09) Assignment |=|
//@[10:11) LeftSquare |[|
//@[11:15) TrueKeyword |true|
//@[15:16) NewLine |\n|
    { /* xxxx */ foo: 'object width: 38' }
//@[04:05) LeftBrace |{|
//@[17:20) Identifier |foo|
//@[20:21) Colon |:|
//@[22:40) StringComplete |'object width: 38'|
//@[41:42) RightBrace |}|
//@[42:43) NewLine |\n|
    'xxxxxxxxxxxxxxxxxx' ]
//@[04:24) StringComplete |'xxxxxxxxxxxxxxxxxx'|
//@[25:26) RightSquare |]|
//@[26:27) NewLine |\n|
var w80 = [true, { foo: 'object width: 39 xxxxxxxxxxx' }
//@[00:03) Identifier |var|
//@[04:07) Identifier |w80|
//@[08:09) Assignment |=|
//@[10:11) LeftSquare |[|
//@[11:15) TrueKeyword |true|
//@[15:16) Comma |,|
//@[17:18) LeftBrace |{|
//@[19:22) Identifier |foo|
//@[22:23) Colon |:|
//@[24:54) StringComplete |'object width: 39 xxxxxxxxxxx'|
//@[55:56) RightBrace |}|
//@[56:57) NewLine |\n|
    'xxxxxxxxxxxxxxxxxxx']
//@[04:25) StringComplete |'xxxxxxxxxxxxxxxxxxx'|
//@[25:26) RightSquare |]|
//@[26:27) NewLine |\n|
var w81 = [true, { foo: 'object width: 40 xxxxxxxxxxxx' }, 'xxxxxxxxxxxxxxxxxxx' ]
//@[00:03) Identifier |var|
//@[04:07) Identifier |w81|
//@[08:09) Assignment |=|
//@[10:11) LeftSquare |[|
//@[11:15) TrueKeyword |true|
//@[15:16) Comma |,|
//@[17:18) LeftBrace |{|
//@[19:22) Identifier |foo|
//@[22:23) Colon |:|
//@[24:55) StringComplete |'object width: 40 xxxxxxxxxxxx'|
//@[56:57) RightBrace |}|
//@[57:58) Comma |,|
//@[59:80) StringComplete |'xxxxxxxxxxxxxxxxxxx'|
//@[81:82) RightSquare |]|
//@[82:83) NewLine |\n|
var w82 = [  true, concat(/* function width: 41 */123, 456) /* xxxxxxxxxxxxxxxx */ ]
//@[00:03) Identifier |var|
//@[04:07) Identifier |w82|
//@[08:09) Assignment |=|
//@[10:11) LeftSquare |[|
//@[13:17) TrueKeyword |true|
//@[17:18) Comma |,|
//@[19:25) Identifier |concat|
//@[25:26) LeftParen |(|
//@[50:53) Integer |123|
//@[53:54) Comma |,|
//@[55:58) Integer |456|
//@[58:59) RightParen |)|
//@[83:84) RightSquare |]|
//@[84:86) NewLine |\n\n|

var w78_ ={ foo: 123, /* xxxx */ baz: ['xxxxxxxxxxx', 'xxxxxxxxxxxxxxxxxx'] }
//@[00:03) Identifier |var|
//@[04:08) Identifier |w78_|
//@[09:10) Assignment |=|
//@[10:11) LeftBrace |{|
//@[12:15) Identifier |foo|
//@[15:16) Colon |:|
//@[17:20) Integer |123|
//@[20:21) Comma |,|
//@[33:36) Identifier |baz|
//@[36:37) Colon |:|
//@[38:39) LeftSquare |[|
//@[39:52) StringComplete |'xxxxxxxxxxx'|
//@[52:53) Comma |,|
//@[54:74) StringComplete |'xxxxxxxxxxxxxxxxxx'|
//@[74:75) RightSquare |]|
//@[76:77) RightBrace |}|
//@[77:78) NewLine |\n|
var w79_ = { foo: 123, bar: true, baz: ['xxxxxxxxxxx', 'xxxxxxxx'] }
//@[00:03) Identifier |var|
//@[04:08) Identifier |w79_|
//@[09:10) Assignment |=|
//@[11:12) LeftBrace |{|
//@[13:16) Identifier |foo|
//@[16:17) Colon |:|
//@[18:21) Integer |123|
//@[21:22) Comma |,|
//@[23:26) Identifier |bar|
//@[26:27) Colon |:|
//@[28:32) TrueKeyword |true|
//@[32:33) Comma |,|
//@[34:37) Identifier |baz|
//@[37:38) Colon |:|
//@[39:40) LeftSquare |[|
//@[40:53) StringComplete |'xxxxxxxxxxx'|
//@[53:54) Comma |,|
//@[55:65) StringComplete |'xxxxxxxx'|
//@[65:66) RightSquare |]|
//@[67:68) RightBrace |}|
//@[68:69) NewLine |\n|
var w80_ = { foo: 123, bar: true, baz: ['xxxxxxxxxxx'
//@[00:03) Identifier |var|
//@[04:08) Identifier |w80_|
//@[09:10) Assignment |=|
//@[11:12) LeftBrace |{|
//@[13:16) Identifier |foo|
//@[16:17) Colon |:|
//@[18:21) Integer |123|
//@[21:22) Comma |,|
//@[23:26) Identifier |bar|
//@[26:27) Colon |:|
//@[28:32) TrueKeyword |true|
//@[32:33) Comma |,|
//@[34:37) Identifier |baz|
//@[37:38) Colon |:|
//@[39:40) LeftSquare |[|
//@[40:53) StringComplete |'xxxxxxxxxxx'|
//@[53:54) NewLine |\n|
'xxxxxxxxxxxxxxxxxxxx'] } // suffix
//@[00:22) StringComplete |'xxxxxxxxxxxxxxxxxxxx'|
//@[22:23) RightSquare |]|
//@[24:25) RightBrace |}|
//@[35:36) NewLine |\n|
var w81_ = { foo: 123, bar: true, baz: ['xxxxxxxxxxx', 'xxxxxxxxxxxxxxxxxxxxx'] }
//@[00:03) Identifier |var|
//@[04:08) Identifier |w81_|
//@[09:10) Assignment |=|
//@[11:12) LeftBrace |{|
//@[13:16) Identifier |foo|
//@[16:17) Colon |:|
//@[18:21) Integer |123|
//@[21:22) Comma |,|
//@[23:26) Identifier |bar|
//@[26:27) Colon |:|
//@[28:32) TrueKeyword |true|
//@[32:33) Comma |,|
//@[34:37) Identifier |baz|
//@[37:38) Colon |:|
//@[39:40) LeftSquare |[|
//@[40:53) StringComplete |'xxxxxxxxxxx'|
//@[53:54) Comma |,|
//@[55:78) StringComplete |'xxxxxxxxxxxxxxxxxxxxx'|
//@[78:79) RightSquare |]|
//@[80:81) RightBrace |}|
//@[81:82) NewLine |\n|
var w82_ = { foo: 123, bar: true, baz: ['array length: 41', 'xxxxxxxxxxxxxxxxx'] }
//@[00:03) Identifier |var|
//@[04:08) Identifier |w82_|
//@[09:10) Assignment |=|
//@[11:12) LeftBrace |{|
//@[13:16) Identifier |foo|
//@[16:17) Colon |:|
//@[18:21) Integer |123|
//@[21:22) Comma |,|
//@[23:26) Identifier |bar|
//@[26:27) Colon |:|
//@[28:32) TrueKeyword |true|
//@[32:33) Comma |,|
//@[34:37) Identifier |baz|
//@[37:38) Colon |:|
//@[39:40) LeftSquare |[|
//@[40:58) StringComplete |'array length: 41'|
//@[58:59) Comma |,|
//@[60:79) StringComplete |'xxxxxxxxxxxxxxxxx'|
//@[79:80) RightSquare |]|
//@[81:82) RightBrace |}|
//@[82:84) NewLine |\n\n|

var w78__ = union({ foo: 'xxxxx' }, { bar: 'xxxxxxxxx' }, { baz: 'xxxxxxxxx'})
//@[00:03) Identifier |var|
//@[04:09) Identifier |w78__|
//@[10:11) Assignment |=|
//@[12:17) Identifier |union|
//@[17:18) LeftParen |(|
//@[18:19) LeftBrace |{|
//@[20:23) Identifier |foo|
//@[23:24) Colon |:|
//@[25:32) StringComplete |'xxxxx'|
//@[33:34) RightBrace |}|
//@[34:35) Comma |,|
//@[36:37) LeftBrace |{|
//@[38:41) Identifier |bar|
//@[41:42) Colon |:|
//@[43:54) StringComplete |'xxxxxxxxx'|
//@[55:56) RightBrace |}|
//@[56:57) Comma |,|
//@[58:59) LeftBrace |{|
//@[60:63) Identifier |baz|
//@[63:64) Colon |:|
//@[65:76) StringComplete |'xxxxxxxxx'|
//@[76:77) RightBrace |}|
//@[77:78) RightParen |)|
//@[78:79) NewLine |\n|
var w79__ = union({ foo: 'xxxxx' }, { bar: 'xxxxxxxxx' },
//@[00:03) Identifier |var|
//@[04:09) Identifier |w79__|
//@[10:11) Assignment |=|
//@[12:17) Identifier |union|
//@[17:18) LeftParen |(|
//@[18:19) LeftBrace |{|
//@[20:23) Identifier |foo|
//@[23:24) Colon |:|
//@[25:32) StringComplete |'xxxxx'|
//@[33:34) RightBrace |}|
//@[34:35) Comma |,|
//@[36:37) LeftBrace |{|
//@[38:41) Identifier |bar|
//@[41:42) Colon |:|
//@[43:54) StringComplete |'xxxxxxxxx'|
//@[55:56) RightBrace |}|
//@[56:57) Comma |,|
//@[57:58) NewLine |\n|
    { baz: 'xxxxxxxxxx'}) // suffix
//@[04:05) LeftBrace |{|
//@[06:09) Identifier |baz|
//@[09:10) Colon |:|
//@[11:23) StringComplete |'xxxxxxxxxx'|
//@[23:24) RightBrace |}|
//@[24:25) RightParen |)|
//@[35:36) NewLine |\n|
var w80__ = union(
//@[00:03) Identifier |var|
//@[04:09) Identifier |w80__|
//@[10:11) Assignment |=|
//@[12:17) Identifier |union|
//@[17:18) LeftParen |(|
//@[18:19) NewLine |\n|
    { foo: 'xxxxxx' },
//@[04:05) LeftBrace |{|
//@[06:09) Identifier |foo|
//@[09:10) Colon |:|
//@[11:19) StringComplete |'xxxxxx'|
//@[20:21) RightBrace |}|
//@[21:22) Comma |,|
//@[22:23) NewLine |\n|
    { bar: 'xxxxxx' },
//@[04:05) LeftBrace |{|
//@[06:09) Identifier |bar|
//@[09:10) Colon |:|
//@[11:19) StringComplete |'xxxxxx'|
//@[20:21) RightBrace |}|
//@[21:22) Comma |,|
//@[22:23) NewLine |\n|
    { baz: 'xxxxxxxxxxxxx'})
//@[04:05) LeftBrace |{|
//@[06:09) Identifier |baz|
//@[09:10) Colon |:|
//@[11:26) StringComplete |'xxxxxxxxxxxxx'|
//@[26:27) RightBrace |}|
//@[27:28) RightParen |)|
//@[28:29) NewLine |\n|
var w81__ = union({ foo: 'x' } /* xxx */, any({ baz: 'func call length: 38  ' }))
//@[00:03) Identifier |var|
//@[04:09) Identifier |w81__|
//@[10:11) Assignment |=|
//@[12:17) Identifier |union|
//@[17:18) LeftParen |(|
//@[18:19) LeftBrace |{|
//@[20:23) Identifier |foo|
//@[23:24) Colon |:|
//@[25:28) StringComplete |'x'|
//@[29:30) RightBrace |}|
//@[40:41) Comma |,|
//@[42:45) Identifier |any|
//@[45:46) LeftParen |(|
//@[46:47) LeftBrace |{|
//@[48:51) Identifier |baz|
//@[51:52) Colon |:|
//@[53:77) StringComplete |'func call length: 38  '|
//@[78:79) RightBrace |}|
//@[79:80) RightParen |)|
//@[80:81) RightParen |)|
//@[81:82) NewLine |\n|
var w82__ = union({ foo: 'x', bar: 'x' }, any({ baz: 'func call length: 39   ' }))
//@[00:03) Identifier |var|
//@[04:09) Identifier |w82__|
//@[10:11) Assignment |=|
//@[12:17) Identifier |union|
//@[17:18) LeftParen |(|
//@[18:19) LeftBrace |{|
//@[20:23) Identifier |foo|
//@[23:24) Colon |:|
//@[25:28) StringComplete |'x'|
//@[28:29) Comma |,|
//@[30:33) Identifier |bar|
//@[33:34) Colon |:|
//@[35:38) StringComplete |'x'|
//@[39:40) RightBrace |}|
//@[40:41) Comma |,|
//@[42:45) Identifier |any|
//@[45:46) LeftParen |(|
//@[46:47) LeftBrace |{|
//@[48:51) Identifier |baz|
//@[51:52) Colon |:|
//@[53:78) StringComplete |'func call length: 39   '|
//@[79:80) RightBrace |}|
//@[80:81) RightParen |)|
//@[81:82) RightParen |)|
//@[82:84) NewLine |\n\n|

var w78___ = /* xxxxxxxxxxxxxxxxxxxxxxxxxxxx */ true
//@[00:03) Identifier |var|
//@[04:10) Identifier |w78___|
//@[11:12) Assignment |=|
//@[48:52) TrueKeyword |true|
//@[52:53) NewLine |\n|
? 1234567890
//@[00:01) Question |?|
//@[02:12) Integer |1234567890|
//@[12:13) NewLine |\n|
: 1234567890
//@[00:01) Colon |:|
//@[02:12) Integer |1234567890|
//@[12:13) NewLine |\n|
var w79___ = /* xxxxxxxxxxxxxxxxxxxxxxxxxxxxx */ true ? { foo: 1 } : [12345678]
//@[00:03) Identifier |var|
//@[04:10) Identifier |w79___|
//@[11:12) Assignment |=|
//@[49:53) TrueKeyword |true|
//@[54:55) Question |?|
//@[56:57) LeftBrace |{|
//@[58:61) Identifier |foo|
//@[61:62) Colon |:|
//@[63:64) Integer |1|
//@[65:66) RightBrace |}|
//@[67:68) Colon |:|
//@[69:70) LeftSquare |[|
//@[70:78) Integer |12345678|
//@[78:79) RightSquare |]|
//@[79:80) NewLine |\n|
var w80___ = true ? { foo: true, bar: false } : [123, 234, 456, { xyz: 'xxxx' }]
//@[00:03) Identifier |var|
//@[04:10) Identifier |w80___|
//@[11:12) Assignment |=|
//@[13:17) TrueKeyword |true|
//@[18:19) Question |?|
//@[20:21) LeftBrace |{|
//@[22:25) Identifier |foo|
//@[25:26) Colon |:|
//@[27:31) TrueKeyword |true|
//@[31:32) Comma |,|
//@[33:36) Identifier |bar|
//@[36:37) Colon |:|
//@[38:43) FalseKeyword |false|
//@[44:45) RightBrace |}|
//@[46:47) Colon |:|
//@[48:49) LeftSquare |[|
//@[49:52) Integer |123|
//@[52:53) Comma |,|
//@[54:57) Integer |234|
//@[57:58) Comma |,|
//@[59:62) Integer |456|
//@[62:63) Comma |,|
//@[64:65) LeftBrace |{|
//@[66:69) Identifier |xyz|
//@[69:70) Colon |:|
//@[71:77) StringComplete |'xxxx'|
//@[78:79) RightBrace |}|
//@[79:80) RightSquare |]|
//@[80:81) NewLine |\n|
var w81___ = /* xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx */ true ? 1234567890 : 1234567890
//@[00:03) Identifier |var|
//@[04:10) Identifier |w81___|
//@[11:12) Assignment |=|
//@[51:55) TrueKeyword |true|
//@[56:57) Question |?|
//@[58:68) Integer |1234567890|
//@[69:70) Colon |:|
//@[71:81) Integer |1234567890|
//@[81:82) NewLine |\n|
var w82___ = /* xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx */ true ? 1234567890 : 1234567890
//@[00:03) Identifier |var|
//@[04:10) Identifier |w82___|
//@[11:12) Assignment |=|
//@[52:56) TrueKeyword |true|
//@[57:58) Question |?|
//@[59:69) Integer |1234567890|
//@[70:71) Colon |:|
//@[72:82) Integer |1234567890|
//@[82:85) NewLine |\n\n\n|


var w80_________ = union(/*******************************************/ {}, {}, {
//@[00:03) Identifier |var|
//@[04:16) Identifier |w80_________|
//@[17:18) Assignment |=|
//@[19:24) Identifier |union|
//@[24:25) LeftParen |(|
//@[71:72) LeftBrace |{|
//@[72:73) RightBrace |}|
//@[73:74) Comma |,|
//@[75:76) LeftBrace |{|
//@[76:77) RightBrace |}|
//@[77:78) Comma |,|
//@[79:80) LeftBrace |{|
//@[80:81) NewLine |\n|
    foo: true
//@[04:07) Identifier |foo|
//@[07:08) Colon |:|
//@[09:13) TrueKeyword |true|
//@[13:14) NewLine |\n|
    bar: false
//@[04:07) Identifier |bar|
//@[07:08) Colon |:|
//@[09:14) FalseKeyword |false|
//@[14:15) NewLine |\n|
})
//@[00:01) RightBrace |}|
//@[01:02) RightParen |)|
//@[02:03) NewLine |\n|
var w81_________ = union(/********************************************/ {}, {}, {
//@[00:03) Identifier |var|
//@[04:16) Identifier |w81_________|
//@[17:18) Assignment |=|
//@[19:24) Identifier |union|
//@[24:25) LeftParen |(|
//@[72:73) LeftBrace |{|
//@[73:74) RightBrace |}|
//@[74:75) Comma |,|
//@[76:77) LeftBrace |{|
//@[77:78) RightBrace |}|
//@[78:79) Comma |,|
//@[80:81) LeftBrace |{|
//@[81:82) NewLine |\n|
    foo: true
//@[04:07) Identifier |foo|
//@[07:08) Colon |:|
//@[09:13) TrueKeyword |true|
//@[13:14) NewLine |\n|
    bar: false
//@[04:07) Identifier |bar|
//@[07:08) Colon |:|
//@[09:14) FalseKeyword |false|
//@[14:15) NewLine |\n|
})
//@[00:01) RightBrace |}|
//@[01:02) RightParen |)|
//@[02:04) NewLine |\n\n|

var w80__________ = union(/******************************************/ {}, {}, {
//@[00:03) Identifier |var|
//@[04:17) Identifier |w80__________|
//@[18:19) Assignment |=|
//@[20:25) Identifier |union|
//@[25:26) LeftParen |(|
//@[71:72) LeftBrace |{|
//@[72:73) RightBrace |}|
//@[73:74) Comma |,|
//@[75:76) LeftBrace |{|
//@[76:77) RightBrace |}|
//@[77:78) Comma |,|
//@[79:80) LeftBrace |{|
//@[80:81) NewLine |\n|
    foo: true
//@[04:07) Identifier |foo|
//@[07:08) Colon |:|
//@[09:13) TrueKeyword |true|
//@[13:14) NewLine |\n|
    w80: union(/***********************************************************/ {}, {
//@[04:07) Identifier |w80|
//@[07:08) Colon |:|
//@[09:14) Identifier |union|
//@[14:15) LeftParen |(|
//@[77:78) LeftBrace |{|
//@[78:79) RightBrace |}|
//@[79:80) Comma |,|
//@[81:82) LeftBrace |{|
//@[82:83) NewLine |\n|
        baz: 123
//@[08:11) Identifier |baz|
//@[11:12) Colon |:|
//@[13:16) Integer |123|
//@[16:17) NewLine |\n|
    })
//@[04:05) RightBrace |}|
//@[05:06) RightParen |)|
//@[06:07) NewLine |\n|
})
//@[00:01) RightBrace |}|
//@[01:02) RightParen |)|
//@[02:04) NewLine |\n\n|

var w81__________ = union(/*******************************************/ {}, {}, {
//@[00:03) Identifier |var|
//@[04:17) Identifier |w81__________|
//@[18:19) Assignment |=|
//@[20:25) Identifier |union|
//@[25:26) LeftParen |(|
//@[72:73) LeftBrace |{|
//@[73:74) RightBrace |}|
//@[74:75) Comma |,|
//@[76:77) LeftBrace |{|
//@[77:78) RightBrace |}|
//@[78:79) Comma |,|
//@[80:81) LeftBrace |{|
//@[81:82) NewLine |\n|
    foo: true
//@[04:07) Identifier |foo|
//@[07:08) Colon |:|
//@[09:13) TrueKeyword |true|
//@[13:14) NewLine |\n|
    w81: union(/**********************************************************/ {}, {
//@[04:07) Identifier |w81|
//@[07:08) Colon |:|
//@[09:14) Identifier |union|
//@[14:15) LeftParen |(|
//@[76:77) LeftBrace |{|
//@[77:78) RightBrace |}|
//@[78:79) Comma |,|
//@[80:81) LeftBrace |{|
//@[81:82) NewLine |\n|
        baz: 123
//@[08:11) Identifier |baz|
//@[11:12) Colon |:|
//@[13:16) Integer |123|
//@[16:17) NewLine |\n|
    })
//@[04:05) RightBrace |}|
//@[05:06) RightParen |)|
//@[06:07) NewLine |\n|
})
//@[00:01) RightBrace |}|
//@[01:02) RightParen |)|
//@[02:04) NewLine |\n\n|

////////////////////////////////////////////////////////////////////////////////
//@[80:81) NewLine |\n|
////////////////////////// Baselines for line breakers /////////////////////////
//@[80:81) NewLine |\n|
////////////////////////////////////////////////////////////////////////////////
//@[80:81) NewLine |\n|
var forceBreak0 = [
//@[00:03) Identifier |var|
//@[04:15) Identifier |forceBreak0|
//@[16:17) Assignment |=|
//@[18:19) LeftSquare |[|
//@[19:20) NewLine |\n|
  1 ]
//@[02:03) Integer |1|
//@[04:05) RightSquare |]|
//@[05:07) NewLine |\n\n|

var forceBreak1 = {
//@[00:03) Identifier |var|
//@[04:15) Identifier |forceBreak1|
//@[16:17) Assignment |=|
//@[18:19) LeftBrace |{|
//@[19:20) NewLine |\n|
    foo: true
//@[04:07) Identifier |foo|
//@[07:08) Colon |:|
//@[09:13) TrueKeyword |true|
//@[13:14) NewLine |\n|
}
//@[00:01) RightBrace |}|
//@[01:02) NewLine |\n|
var forceBreak2 = {
//@[00:03) Identifier |var|
//@[04:15) Identifier |forceBreak2|
//@[16:17) Assignment |=|
//@[18:19) LeftBrace |{|
//@[19:20) NewLine |\n|
    foo: true, bar: false
//@[04:07) Identifier |foo|
//@[07:08) Colon |:|
//@[09:13) TrueKeyword |true|
//@[13:14) Comma |,|
//@[15:18) Identifier |bar|
//@[18:19) Colon |:|
//@[20:25) FalseKeyword |false|
//@[25:26) NewLine |\n|
}
//@[00:01) RightBrace |}|
//@[01:02) NewLine |\n|
var forceBreak3 = [1, 2, {
//@[00:03) Identifier |var|
//@[04:15) Identifier |forceBreak3|
//@[16:17) Assignment |=|
//@[18:19) LeftSquare |[|
//@[19:20) Integer |1|
//@[20:21) Comma |,|
//@[22:23) Integer |2|
//@[23:24) Comma |,|
//@[25:26) LeftBrace |{|
//@[26:27) NewLine |\n|
    foo: true }, 3, 4]
//@[04:07) Identifier |foo|
//@[07:08) Colon |:|
//@[09:13) TrueKeyword |true|
//@[14:15) RightBrace |}|
//@[15:16) Comma |,|
//@[17:18) Integer |3|
//@[18:19) Comma |,|
//@[20:21) Integer |4|
//@[21:22) RightSquare |]|
//@[22:23) NewLine |\n|
var forceBreak4 = { foo: true, bar: false // force break
//@[00:03) Identifier |var|
//@[04:15) Identifier |forceBreak4|
//@[16:17) Assignment |=|
//@[18:19) LeftBrace |{|
//@[20:23) Identifier |foo|
//@[23:24) Colon |:|
//@[25:29) TrueKeyword |true|
//@[29:30) Comma |,|
//@[31:34) Identifier |bar|
//@[34:35) Colon |:|
//@[36:41) FalseKeyword |false|
//@[56:57) NewLine |\n|
}
//@[00:01) RightBrace |}|
//@[01:02) NewLine |\n|
var forceBreak5 = { foo: true
//@[00:03) Identifier |var|
//@[04:15) Identifier |forceBreak5|
//@[16:17) Assignment |=|
//@[18:19) LeftBrace |{|
//@[20:23) Identifier |foo|
//@[23:24) Colon |:|
//@[25:29) TrueKeyword |true|
//@[29:30) NewLine |\n|
/* force break */}
//@[17:18) RightBrace |}|
//@[18:19) NewLine |\n|
var forceBreak6 = { foo: true
//@[00:03) Identifier |var|
//@[04:15) Identifier |forceBreak6|
//@[16:17) Assignment |=|
//@[18:19) LeftBrace |{|
//@[20:23) Identifier |foo|
//@[23:24) Colon |:|
//@[25:29) TrueKeyword |true|
//@[29:30) NewLine |\n|
    bar: false
//@[04:07) Identifier |bar|
//@[07:08) Colon |:|
//@[09:14) FalseKeyword |false|
//@[14:15) NewLine |\n|
    baz: 123
//@[04:07) Identifier |baz|
//@[07:08) Colon |:|
//@[09:12) Integer |123|
//@[12:13) NewLine |\n|
/* force break */}
//@[17:18) RightBrace |}|
//@[18:19) NewLine |\n|
var forceBreak7 = [1, 2 // force break
//@[00:03) Identifier |var|
//@[04:15) Identifier |forceBreak7|
//@[16:17) Assignment |=|
//@[18:19) LeftSquare |[|
//@[19:20) Integer |1|
//@[20:21) Comma |,|
//@[22:23) Integer |2|
//@[38:39) NewLine |\n|
]
//@[00:01) RightSquare |]|
//@[01:02) NewLine |\n|
var forceBreak8 = [1, 2
//@[00:03) Identifier |var|
//@[04:15) Identifier |forceBreak8|
//@[16:17) Assignment |=|
//@[18:19) LeftSquare |[|
//@[19:20) Integer |1|
//@[20:21) Comma |,|
//@[22:23) Integer |2|
//@[23:24) NewLine |\n|
    /* force break */ ]
//@[22:23) RightSquare |]|
//@[23:24) NewLine |\n|
var forceBreak9 = [1, 2, {
//@[00:03) Identifier |var|
//@[04:15) Identifier |forceBreak9|
//@[16:17) Assignment |=|
//@[18:19) LeftSquare |[|
//@[19:20) Integer |1|
//@[20:21) Comma |,|
//@[22:23) Integer |2|
//@[23:24) Comma |,|
//@[25:26) LeftBrace |{|
//@[26:27) NewLine |\n|
    foo: true
//@[04:07) Identifier |foo|
//@[07:08) Colon |:|
//@[09:13) TrueKeyword |true|
//@[13:14) NewLine |\n|
    bar: false
//@[04:07) Identifier |bar|
//@[07:08) Colon |:|
//@[09:14) FalseKeyword |false|
//@[14:15) NewLine |\n|
}]
//@[00:01) RightBrace |}|
//@[01:02) RightSquare |]|
//@[02:03) NewLine |\n|
// Does not break immediate parent group, but breaks grandparent.
//@[65:66) NewLine |\n|
var forceBreak10 = [1, 2, intersection({ foo: true, bar: false }, {
//@[00:03) Identifier |var|
//@[04:16) Identifier |forceBreak10|
//@[17:18) Assignment |=|
//@[19:20) LeftSquare |[|
//@[20:21) Integer |1|
//@[21:22) Comma |,|
//@[23:24) Integer |2|
//@[24:25) Comma |,|
//@[26:38) Identifier |intersection|
//@[38:39) LeftParen |(|
//@[39:40) LeftBrace |{|
//@[41:44) Identifier |foo|
//@[44:45) Colon |:|
//@[46:50) TrueKeyword |true|
//@[50:51) Comma |,|
//@[52:55) Identifier |bar|
//@[55:56) Colon |:|
//@[57:62) FalseKeyword |false|
//@[63:64) RightBrace |}|
//@[64:65) Comma |,|
//@[66:67) LeftBrace |{|
//@[67:68) NewLine |\n|
  foo: true})]
//@[02:05) Identifier |foo|
//@[05:06) Colon |:|
//@[07:11) TrueKeyword |true|
//@[11:12) RightBrace |}|
//@[12:13) RightParen |)|
//@[13:14) RightSquare |]|
//@[14:16) NewLine |\n\n|

var forceBreak11 = true // comment
//@[00:03) Identifier |var|
//@[04:16) Identifier |forceBreak11|
//@[17:18) Assignment |=|
//@[19:23) TrueKeyword |true|
//@[34:35) NewLine |\n|
    ? true
//@[04:05) Question |?|
//@[06:10) TrueKeyword |true|
//@[10:11) NewLine |\n|
    : false
//@[04:05) Colon |:|
//@[06:11) FalseKeyword |false|
//@[11:12) NewLine |\n|
var forceBreak12 = true ? true // comment
//@[00:03) Identifier |var|
//@[04:16) Identifier |forceBreak12|
//@[17:18) Assignment |=|
//@[19:23) TrueKeyword |true|
//@[24:25) Question |?|
//@[26:30) TrueKeyword |true|
//@[41:42) NewLine |\n|
    : false
//@[04:05) Colon |:|
//@[06:11) FalseKeyword |false|
//@[11:12) NewLine |\n|
var forceBreak13 = true
//@[00:03) Identifier |var|
//@[04:16) Identifier |forceBreak13|
//@[17:18) Assignment |=|
//@[19:23) TrueKeyword |true|
//@[23:24) NewLine |\n|
    ? true // comment
//@[04:05) Question |?|
//@[06:10) TrueKeyword |true|
//@[21:22) NewLine |\n|
    : false
//@[04:05) Colon |:|
//@[06:11) FalseKeyword |false|
//@[11:12) NewLine |\n|
var forceBreak14 = true ? {
//@[00:03) Identifier |var|
//@[04:16) Identifier |forceBreak14|
//@[17:18) Assignment |=|
//@[19:23) TrueKeyword |true|
//@[24:25) Question |?|
//@[26:27) LeftBrace |{|
//@[27:28) NewLine |\n|
    foo: 42
//@[04:07) Identifier |foo|
//@[07:08) Colon |:|
//@[09:11) Integer |42|
//@[11:12) NewLine |\n|
} : false
//@[00:01) RightBrace |}|
//@[02:03) Colon |:|
//@[04:09) FalseKeyword |false|
//@[09:10) NewLine |\n|
var forceBreak15 = true ? { foo: 0 } : {
//@[00:03) Identifier |var|
//@[04:16) Identifier |forceBreak15|
//@[17:18) Assignment |=|
//@[19:23) TrueKeyword |true|
//@[24:25) Question |?|
//@[26:27) LeftBrace |{|
//@[28:31) Identifier |foo|
//@[31:32) Colon |:|
//@[33:34) Integer |0|
//@[35:36) RightBrace |}|
//@[37:38) Colon |:|
//@[39:40) LeftBrace |{|
//@[40:41) NewLine |\n|
    bar: 1}
//@[04:07) Identifier |bar|
//@[07:08) Colon |:|
//@[09:10) Integer |1|
//@[10:11) RightBrace |}|
//@[11:13) NewLine |\n\n|

var forceBreak16 = union({ foo: 0 }, {
//@[00:03) Identifier |var|
//@[04:16) Identifier |forceBreak16|
//@[17:18) Assignment |=|
//@[19:24) Identifier |union|
//@[24:25) LeftParen |(|
//@[25:26) LeftBrace |{|
//@[27:30) Identifier |foo|
//@[30:31) Colon |:|
//@[32:33) Integer |0|
//@[34:35) RightBrace |}|
//@[35:36) Comma |,|
//@[37:38) LeftBrace |{|
//@[38:39) NewLine |\n|
    foo: 123
//@[04:07) Identifier |foo|
//@[07:08) Colon |:|
//@[09:12) Integer |123|
//@[12:13) NewLine |\n|
    bar: 456
//@[04:07) Identifier |bar|
//@[07:08) Colon |:|
//@[09:12) Integer |456|
//@[12:13) NewLine |\n|
} // comment
//@[00:01) RightBrace |}|
//@[12:13) NewLine |\n|
)
//@[00:01) RightParen |)|
//@[01:02) NewLine |\n|

//@[00:00) EndOfFile ||
