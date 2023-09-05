////////////////////////////////////////////////////////////////////////////////
//@[00:4175) ProgramExpression
//////////////////////////// Baselines for width 40 ////////////////////////////
////////////////////////////////////////////////////////////////////////////////
var w38 = [true, /* xxxxx */ true, 12]      // suffix
//@[00:0038) ├─DeclaredVariableExpression { Name = w38 }
//@[10:0038) | └─ArrayExpression
//@[11:0015) |   ├─BooleanLiteralExpression { Value = True }
//@[29:0033) |   ├─BooleanLiteralExpression { Value = True }
//@[35:0037) |   └─IntegerLiteralExpression { Value = 12 }
var w39 = [true, true
//@[00:0042) ├─DeclaredVariableExpression { Name = w39 }
//@[10:0042) | └─ArrayExpression
//@[11:0015) |   ├─BooleanLiteralExpression { Value = True }
//@[17:0021) |   ├─BooleanLiteralExpression { Value = True }
    true, true, 123]
//@[04:0008) |   ├─BooleanLiteralExpression { Value = True }
//@[10:0014) |   ├─BooleanLiteralExpression { Value = True }
//@[16:0019) |   └─IntegerLiteralExpression { Value = 123 }
var w40 =[
//@[00:0043) ├─DeclaredVariableExpression { Name = w40 }
//@[09:0043) | └─ArrayExpression
    true, true, 1234/* xxxxx */]  // suffix
//@[04:0008) |   ├─BooleanLiteralExpression { Value = True }
//@[10:0014) |   ├─BooleanLiteralExpression { Value = True }
//@[16:0020) |   └─IntegerLiteralExpression { Value = 1234 }
var w41 =[ true, true, true, true, 12345 ]
//@[00:0042) ├─DeclaredVariableExpression { Name = w41 }
//@[09:0042) | └─ArrayExpression
//@[11:0015) |   ├─BooleanLiteralExpression { Value = True }
//@[17:0021) |   ├─BooleanLiteralExpression { Value = True }
//@[23:0027) |   ├─BooleanLiteralExpression { Value = True }
//@[29:0033) |   ├─BooleanLiteralExpression { Value = True }
//@[35:0040) |   └─IntegerLiteralExpression { Value = 12345 }
var w42 =[true, /* xxx */ 12 /* xx */, 1]
//@[00:0041) ├─DeclaredVariableExpression { Name = w42 }
//@[09:0041) | └─ArrayExpression
//@[10:0014) |   ├─BooleanLiteralExpression { Value = True }
//@[26:0028) |   ├─IntegerLiteralExpression { Value = 12 }
//@[39:0040) |   └─IntegerLiteralExpression { Value = 1 }

var w38_= { foo: true, bar: 1234567
//@[00:0037) ├─DeclaredVariableExpression { Name = w38_ }
//@[10:0037) | └─ObjectExpression
//@[12:0021) |   ├─ObjectPropertyExpression
//@[12:0015) |   | ├─StringLiteralExpression { Value = foo }
//@[17:0021) |   | └─BooleanLiteralExpression { Value = True }
//@[23:0035) |   └─ObjectPropertyExpression
//@[23:0026) |     ├─StringLiteralExpression { Value = bar }
//@[28:0035) |     └─IntegerLiteralExpression { Value = 1234567 }
} // suffix
var        w39_= { foo: true
//@[00:0046) ├─DeclaredVariableExpression { Name = w39_ }
//@[17:0046) | └─ObjectExpression
//@[19:0028) |   ├─ObjectPropertyExpression
//@[19:0022) |   | ├─StringLiteralExpression { Value = foo }
//@[24:0028) |   | └─BooleanLiteralExpression { Value = True }
  bar: 12345678 } // suffix
//@[02:0015) |   └─ObjectPropertyExpression
//@[02:0005) |     ├─StringLiteralExpression { Value = bar }
//@[07:0015) |     └─IntegerLiteralExpression { Value = 12345678 }
    var w40_= { foo: 1, bar:    1 /* xxxx */ }
//@[04:0046) ├─DeclaredVariableExpression { Name = w40_ }
//@[14:0046) | └─ObjectExpression
//@[16:0022) |   ├─ObjectPropertyExpression
//@[16:0019) |   | ├─StringLiteralExpression { Value = foo }
//@[21:0022) |   | └─IntegerLiteralExpression { Value = 1 }
//@[24:0033) |   └─ObjectPropertyExpression
//@[24:0027) |     ├─StringLiteralExpression { Value = bar }
//@[32:0033) |     └─IntegerLiteralExpression { Value = 1 }
var w41_={ foo: true, bar    : 1234567890 }
//@[00:0043) ├─DeclaredVariableExpression { Name = w41_ }
//@[09:0043) | └─ObjectExpression
//@[11:0020) |   ├─ObjectPropertyExpression
//@[11:0014) |   | ├─StringLiteralExpression { Value = foo }
//@[16:0020) |   | └─BooleanLiteralExpression { Value = True }
//@[22:0041) |   └─ObjectPropertyExpression
//@[22:0025) |     ├─StringLiteralExpression { Value = bar }
//@[31:0041) |     └─IntegerLiteralExpression { Value = 1234567890 }
var w42_= { foo: true
//@[00:0044) ├─DeclaredVariableExpression { Name = w42_ }
//@[10:0044) | └─ObjectExpression
//@[12:0021) |   ├─ObjectPropertyExpression
//@[12:0015) |   | ├─StringLiteralExpression { Value = foo }
//@[17:0021) |   | └─BooleanLiteralExpression { Value = True }
    bar: 12345678901 } // suffix
//@[04:0020) |   └─ObjectPropertyExpression
//@[04:0007) |     ├─StringLiteralExpression { Value = bar }
//@[09:0020) |     └─IntegerLiteralExpression { Value = 12345678901 }

   var w38__ =    concat('xxxxxx', 'xxxxxx')
//@[03:0044) ├─DeclaredVariableExpression { Name = w38__ }
//@[18:0044) | └─FunctionCallExpression { Name = concat }
//@[25:0033) |   ├─StringLiteralExpression { Value = xxxxxx }
//@[35:0043) |   └─StringLiteralExpression { Value = xxxxxx }
var w39__ = concat('xxxxxx', 'xxxxxxx'
//@[00:0040) ├─DeclaredVariableExpression { Name = w39__ }
//@[12:0040) | └─FunctionCallExpression { Name = concat }
//@[19:0027) |   ├─StringLiteralExpression { Value = xxxxxx }
//@[29:0038) |   └─StringLiteralExpression { Value = xxxxxxx }
) // suffix
var w40__ = concat('xxxxxx',
//@[00:0040) ├─DeclaredVariableExpression { Name = w40__ }
//@[12:0040) | └─FunctionCallExpression { Name = concat }
//@[19:0027) |   ├─StringLiteralExpression { Value = xxxxxx }
'xxxxxxxx') // suffix
//@[00:0010) |   └─StringLiteralExpression { Value = xxxxxxxx }

var        w41__= concat('xxxxx'/* xxxxxxx */)
//@[00:0046) ├─DeclaredVariableExpression { Name = w41__ }
//@[18:0046) | └─FunctionCallExpression { Name = concat }
//@[25:0032) |   └─StringLiteralExpression { Value = xxxxx }
var w42__ = concat('xxxxx', 'xxxxxxxxxxx')
//@[00:0042) ├─DeclaredVariableExpression { Name = w42__ }
//@[12:0042) | └─FunctionCallExpression { Name = concat }
//@[19:0026) |   ├─StringLiteralExpression { Value = xxxxx }
//@[28:0041) |   └─StringLiteralExpression { Value = xxxxxxxxxxx }

var w38___ = true? 'xxxxx' : 'xxxxxx'
//@[00:0037) ├─DeclaredVariableExpression { Name = w38___ }
//@[13:0037) | └─TernaryExpression
//@[13:0017) |   ├─BooleanLiteralExpression { Value = True }
//@[19:0026) |   ├─StringLiteralExpression { Value = xxxxx }
//@[29:0037) |   └─StringLiteralExpression { Value = xxxxxx }
var w39___ = true
//@[00:0039) ├─DeclaredVariableExpression { Name = w39___ }
//@[13:0039) | └─TernaryExpression
//@[13:0017) |   ├─BooleanLiteralExpression { Value = True }
? 'xxxxxx' : 'xxxxxx' // suffix
//@[02:0010) |   ├─StringLiteralExpression { Value = xxxxxx }
//@[13:0021) |   └─StringLiteralExpression { Value = xxxxxx }
var w40___ = true ?'xxxxxx' : 'xxxxxxx'
//@[00:0039) ├─DeclaredVariableExpression { Name = w40___ }
//@[13:0039) | └─TernaryExpression
//@[13:0017) |   ├─BooleanLiteralExpression { Value = True }
//@[19:0027) |   ├─StringLiteralExpression { Value = xxxxxx }
//@[30:0039) |   └─StringLiteralExpression { Value = xxxxxxx }
var w41___ = true ? 'xxxxxxx' :         'xxxxxxx'
//@[00:0049) ├─DeclaredVariableExpression { Name = w41___ }
//@[13:0049) | └─TernaryExpression
//@[13:0017) |   ├─BooleanLiteralExpression { Value = True }
//@[20:0029) |   ├─StringLiteralExpression { Value = xxxxxxx }
//@[40:0049) |   └─StringLiteralExpression { Value = xxxxxxx }
var w42___ = true ? 'xxxxxxx':'xxxxxxxx'
//@[00:0040) ├─DeclaredVariableExpression { Name = w42___ }
//@[13:0040) | └─TernaryExpression
//@[13:0017) |   ├─BooleanLiteralExpression { Value = True }
//@[20:0029) |   ├─StringLiteralExpression { Value = xxxxxxx }
//@[30:0040) |   └─StringLiteralExpression { Value = xxxxxxxx }

////////////////////////////////////////////////////////////////////////////////
//////////////////////////// Baselines for width 80 ////////////////////////////
////////////////////////////////////////////////////////////////////////////////
var w78 = [
//@[00:0084) ├─DeclaredVariableExpression { Name = w78 }
//@[10:0084) | └─ArrayExpression
    true, { foo: 'object width: 37' /* xxx */ }, 'xxxxxxxxxxxxxxxxxxx' ]
//@[04:0008) |   ├─BooleanLiteralExpression { Value = True }
//@[10:0047) |   ├─ObjectExpression
//@[12:0035) |   | └─ObjectPropertyExpression
//@[12:0015) |   |   ├─StringLiteralExpression { Value = foo }
//@[17:0035) |   |   └─StringLiteralExpression { Value = object width: 37 }
//@[49:0070) |   └─StringLiteralExpression { Value = xxxxxxxxxxxxxxxxxxx }
var w79 = [true
//@[00:0085) ├─DeclaredVariableExpression { Name = w79 }
//@[10:0085) | └─ArrayExpression
//@[11:0015) |   ├─BooleanLiteralExpression { Value = True }
    { /* xxxx */ foo: 'object width: 38' }
//@[04:0042) |   ├─ObjectExpression
//@[17:0040) |   | └─ObjectPropertyExpression
//@[17:0020) |   |   ├─StringLiteralExpression { Value = foo }
//@[22:0040) |   |   └─StringLiteralExpression { Value = object width: 38 }
    'xxxxxxxxxxxxxxxxxx' ]
//@[04:0024) |   └─StringLiteralExpression { Value = xxxxxxxxxxxxxxxxxx }
var w80 = [true, { foo: 'object width: 39 xxxxxxxxxxx' }
//@[00:0083) ├─DeclaredVariableExpression { Name = w80 }
//@[10:0083) | └─ArrayExpression
//@[11:0015) |   ├─BooleanLiteralExpression { Value = True }
//@[17:0056) |   ├─ObjectExpression
//@[19:0054) |   | └─ObjectPropertyExpression
//@[19:0022) |   |   ├─StringLiteralExpression { Value = foo }
//@[24:0054) |   |   └─StringLiteralExpression { Value = object width: 39 xxxxxxxxxxx }
    'xxxxxxxxxxxxxxxxxxx']
//@[04:0025) |   └─StringLiteralExpression { Value = xxxxxxxxxxxxxxxxxxx }
var w81 = [true, { foo: 'object width: 40 xxxxxxxxxxxx' }, 'xxxxxxxxxxxxxxxxxxx' ]
//@[00:0082) ├─DeclaredVariableExpression { Name = w81 }
//@[10:0082) | └─ArrayExpression
//@[11:0015) |   ├─BooleanLiteralExpression { Value = True }
//@[17:0057) |   ├─ObjectExpression
//@[19:0055) |   | └─ObjectPropertyExpression
//@[19:0022) |   |   ├─StringLiteralExpression { Value = foo }
//@[24:0055) |   |   └─StringLiteralExpression { Value = object width: 40 xxxxxxxxxxxx }
//@[59:0080) |   └─StringLiteralExpression { Value = xxxxxxxxxxxxxxxxxxx }
var w82 = [  true, concat(/* function width: 41 */123, 456) /* xxxxxxxxxxxxxxxx */ ]
//@[00:0084) ├─DeclaredVariableExpression { Name = w82 }
//@[10:0084) | └─ArrayExpression
//@[13:0017) |   ├─BooleanLiteralExpression { Value = True }
//@[19:0059) |   └─FunctionCallExpression { Name = concat }
//@[50:0053) |     ├─IntegerLiteralExpression { Value = 123 }
//@[55:0058) |     └─IntegerLiteralExpression { Value = 456 }

var w78_ ={ foo: 123, /* xxxx */ baz: ['xxxxxxxxxxx', 'xxxxxxxxxxxxxxxxxx'] }
//@[00:0077) ├─DeclaredVariableExpression { Name = w78_ }
//@[10:0077) | └─ObjectExpression
//@[12:0020) |   ├─ObjectPropertyExpression
//@[12:0015) |   | ├─StringLiteralExpression { Value = foo }
//@[17:0020) |   | └─IntegerLiteralExpression { Value = 123 }
//@[33:0075) |   └─ObjectPropertyExpression
//@[33:0036) |     ├─StringLiteralExpression { Value = baz }
//@[38:0075) |     └─ArrayExpression
//@[39:0052) |       ├─StringLiteralExpression { Value = xxxxxxxxxxx }
//@[54:0074) |       └─StringLiteralExpression { Value = xxxxxxxxxxxxxxxxxx }
var w79_ = { foo: 123, bar: true, baz: ['xxxxxxxxxxx', 'xxxxxxxx'] }
//@[00:0068) ├─DeclaredVariableExpression { Name = w79_ }
//@[11:0068) | └─ObjectExpression
//@[13:0021) |   ├─ObjectPropertyExpression
//@[13:0016) |   | ├─StringLiteralExpression { Value = foo }
//@[18:0021) |   | └─IntegerLiteralExpression { Value = 123 }
//@[23:0032) |   ├─ObjectPropertyExpression
//@[23:0026) |   | ├─StringLiteralExpression { Value = bar }
//@[28:0032) |   | └─BooleanLiteralExpression { Value = True }
//@[34:0066) |   └─ObjectPropertyExpression
//@[34:0037) |     ├─StringLiteralExpression { Value = baz }
//@[39:0066) |     └─ArrayExpression
//@[40:0053) |       ├─StringLiteralExpression { Value = xxxxxxxxxxx }
//@[55:0065) |       └─StringLiteralExpression { Value = xxxxxxxx }
var w80_ = { foo: 123, bar: true, baz: [
//@[00:0085) ├─DeclaredVariableExpression { Name = w80_ }
//@[11:0085) | └─ObjectExpression
//@[13:0021) |   ├─ObjectPropertyExpression
//@[13:0016) |   | ├─StringLiteralExpression { Value = foo }
//@[18:0021) |   | └─IntegerLiteralExpression { Value = 123 }
//@[23:0032) |   ├─ObjectPropertyExpression
//@[23:0026) |   | ├─StringLiteralExpression { Value = bar }
//@[28:0032) |   | └─BooleanLiteralExpression { Value = True }
//@[34:0083) |   └─ObjectPropertyExpression
//@[34:0037) |     ├─StringLiteralExpression { Value = baz }
//@[39:0083) |     └─ArrayExpression
    'xxxxxxxxxxx', 'xxxxxxxxxxxxxxxxxxxx'] } // suffix
//@[04:0017) |       ├─StringLiteralExpression { Value = xxxxxxxxxxx }
//@[19:0041) |       └─StringLiteralExpression { Value = xxxxxxxxxxxxxxxxxxxx }
var w81_ = { foo: 123, bar: true, baz: ['xxxxxxxxxxx', 'xxxxxxxxxxxxxxxxxxxxx'] }
//@[00:0081) ├─DeclaredVariableExpression { Name = w81_ }
//@[11:0081) | └─ObjectExpression
//@[13:0021) |   ├─ObjectPropertyExpression
//@[13:0016) |   | ├─StringLiteralExpression { Value = foo }
//@[18:0021) |   | └─IntegerLiteralExpression { Value = 123 }
//@[23:0032) |   ├─ObjectPropertyExpression
//@[23:0026) |   | ├─StringLiteralExpression { Value = bar }
//@[28:0032) |   | └─BooleanLiteralExpression { Value = True }
//@[34:0079) |   └─ObjectPropertyExpression
//@[34:0037) |     ├─StringLiteralExpression { Value = baz }
//@[39:0079) |     └─ArrayExpression
//@[40:0053) |       ├─StringLiteralExpression { Value = xxxxxxxxxxx }
//@[55:0078) |       └─StringLiteralExpression { Value = xxxxxxxxxxxxxxxxxxxxx }
var w82_ = { foo: 123, bar: true, baz: ['array length: 41', 'xxxxxxxxxxxxxxxxx'] }
//@[00:0082) ├─DeclaredVariableExpression { Name = w82_ }
//@[11:0082) | └─ObjectExpression
//@[13:0021) |   ├─ObjectPropertyExpression
//@[13:0016) |   | ├─StringLiteralExpression { Value = foo }
//@[18:0021) |   | └─IntegerLiteralExpression { Value = 123 }
//@[23:0032) |   ├─ObjectPropertyExpression
//@[23:0026) |   | ├─StringLiteralExpression { Value = bar }
//@[28:0032) |   | └─BooleanLiteralExpression { Value = True }
//@[34:0080) |   └─ObjectPropertyExpression
//@[34:0037) |     ├─StringLiteralExpression { Value = baz }
//@[39:0080) |     └─ArrayExpression
//@[40:0058) |       ├─StringLiteralExpression { Value = array length: 41 }
//@[60:0079) |       └─StringLiteralExpression { Value = xxxxxxxxxxxxxxxxx }

var w78__ = union({ foo: 'xxxxx' }, { bar: 'xxxxxxxxx' }, { baz: 'xxxxxxxxx'})
//@[00:0078) ├─DeclaredVariableExpression { Name = w78__ }
//@[12:0078) | └─FunctionCallExpression { Name = union }
//@[18:0034) |   ├─ObjectExpression
//@[20:0032) |   | └─ObjectPropertyExpression
//@[20:0023) |   |   ├─StringLiteralExpression { Value = foo }
//@[25:0032) |   |   └─StringLiteralExpression { Value = xxxxx }
//@[36:0056) |   ├─ObjectExpression
//@[38:0054) |   | └─ObjectPropertyExpression
//@[38:0041) |   |   ├─StringLiteralExpression { Value = bar }
//@[43:0054) |   |   └─StringLiteralExpression { Value = xxxxxxxxx }
//@[58:0077) |   └─ObjectExpression
//@[60:0076) |     └─ObjectPropertyExpression
//@[60:0063) |       ├─StringLiteralExpression { Value = baz }
//@[65:0076) |       └─StringLiteralExpression { Value = xxxxxxxxx }
var w79__ = union({ foo: 'xxxxx' }, { bar: 'xxxxxxxxx' },
//@[00:0083) ├─DeclaredVariableExpression { Name = w79__ }
//@[12:0083) | └─FunctionCallExpression { Name = union }
//@[18:0034) |   ├─ObjectExpression
//@[20:0032) |   | └─ObjectPropertyExpression
//@[20:0023) |   |   ├─StringLiteralExpression { Value = foo }
//@[25:0032) |   |   └─StringLiteralExpression { Value = xxxxx }
//@[36:0056) |   ├─ObjectExpression
//@[38:0054) |   | └─ObjectPropertyExpression
//@[38:0041) |   |   ├─StringLiteralExpression { Value = bar }
//@[43:0054) |   |   └─StringLiteralExpression { Value = xxxxxxxxx }
    { baz: 'xxxxxxxxxx'}) // suffix
//@[04:0024) |   └─ObjectExpression
//@[06:0023) |     └─ObjectPropertyExpression
//@[06:0009) |       ├─StringLiteralExpression { Value = baz }
//@[11:0023) |       └─StringLiteralExpression { Value = xxxxxxxxxx }
var w80__ = union(
//@[00:0093) ├─DeclaredVariableExpression { Name = w80__ }
//@[12:0093) | └─FunctionCallExpression { Name = union }
    { foo: 'xxxxxx' },
//@[04:0021) |   ├─ObjectExpression
//@[06:0019) |   | └─ObjectPropertyExpression
//@[06:0009) |   |   ├─StringLiteralExpression { Value = foo }
//@[11:0019) |   |   └─StringLiteralExpression { Value = xxxxxx }
    { bar: 'xxxxxx' },
//@[04:0021) |   ├─ObjectExpression
//@[06:0019) |   | └─ObjectPropertyExpression
//@[06:0009) |   |   ├─StringLiteralExpression { Value = bar }
//@[11:0019) |   |   └─StringLiteralExpression { Value = xxxxxx }
    { baz: 'xxxxxxxxxxxxx'})
//@[04:0027) |   └─ObjectExpression
//@[06:0026) |     └─ObjectPropertyExpression
//@[06:0009) |       ├─StringLiteralExpression { Value = baz }
//@[11:0026) |       └─StringLiteralExpression { Value = xxxxxxxxxxxxx }
var w81__ = union({ foo: 'x' } /* xxx */, any({ baz: 'func call length: 38  ' }))
//@[00:0081) ├─DeclaredVariableExpression { Name = w81__ }
//@[12:0081) | └─FunctionCallExpression { Name = union }
//@[18:0030) |   ├─ObjectExpression
//@[20:0028) |   | └─ObjectPropertyExpression
//@[20:0023) |   |   ├─StringLiteralExpression { Value = foo }
//@[25:0028) |   |   └─StringLiteralExpression { Value = x }
//@[46:0079) |   └─ObjectExpression
//@[48:0077) |     └─ObjectPropertyExpression
//@[48:0051) |       ├─StringLiteralExpression { Value = baz }
//@[53:0077) |       └─StringLiteralExpression { Value = func call length: 38   }
var w82__ = union({ foo: 'x', bar: 'x' }, any({ baz: 'func call length: 39   ' }))
//@[00:0082) ├─DeclaredVariableExpression { Name = w82__ }
//@[12:0082) | └─FunctionCallExpression { Name = union }
//@[18:0040) |   ├─ObjectExpression
//@[20:0028) |   | ├─ObjectPropertyExpression
//@[20:0023) |   | | ├─StringLiteralExpression { Value = foo }
//@[25:0028) |   | | └─StringLiteralExpression { Value = x }
//@[30:0038) |   | └─ObjectPropertyExpression
//@[30:0033) |   |   ├─StringLiteralExpression { Value = bar }
//@[35:0038) |   |   └─StringLiteralExpression { Value = x }
//@[46:0080) |   └─ObjectExpression
//@[48:0078) |     └─ObjectPropertyExpression
//@[48:0051) |       ├─StringLiteralExpression { Value = baz }
//@[53:0078) |       └─StringLiteralExpression { Value = func call length: 39    }

var w78___ = /* xxxxxxxxxxxxxxxxxxxxxxxxxxxx */ true
//@[00:0078) ├─DeclaredVariableExpression { Name = w78___ }
//@[48:0078) | └─TernaryExpression
//@[48:0052) |   ├─BooleanLiteralExpression { Value = True }
? 1234567890
//@[02:0012) |   ├─IntegerLiteralExpression { Value = 1234567890 }
: 1234567890
//@[02:0012) |   └─IntegerLiteralExpression { Value = 1234567890 }
var w79___ = /* xxxxxxxxxxxxxxxxxxxxxxxxxxxxx */ true ? { foo: 1 } : [12345678]
//@[00:0079) ├─DeclaredVariableExpression { Name = w79___ }
//@[49:0079) | └─TernaryExpression
//@[49:0053) |   ├─BooleanLiteralExpression { Value = True }
//@[56:0066) |   ├─ObjectExpression
//@[58:0064) |   | └─ObjectPropertyExpression
//@[58:0061) |   |   ├─StringLiteralExpression { Value = foo }
//@[63:0064) |   |   └─IntegerLiteralExpression { Value = 1 }
//@[69:0079) |   └─ArrayExpression
//@[70:0078) |     └─IntegerLiteralExpression { Value = 12345678 }
var w80___ = true ? { foo: true, bar: false } : [123, 234, 456, { xyz: 'xxxx' }]
//@[00:0080) ├─DeclaredVariableExpression { Name = w80___ }
//@[13:0080) | └─TernaryExpression
//@[13:0017) |   ├─BooleanLiteralExpression { Value = True }
//@[20:0045) |   ├─ObjectExpression
//@[22:0031) |   | ├─ObjectPropertyExpression
//@[22:0025) |   | | ├─StringLiteralExpression { Value = foo }
//@[27:0031) |   | | └─BooleanLiteralExpression { Value = True }
//@[33:0043) |   | └─ObjectPropertyExpression
//@[33:0036) |   |   ├─StringLiteralExpression { Value = bar }
//@[38:0043) |   |   └─BooleanLiteralExpression { Value = False }
//@[48:0080) |   └─ArrayExpression
//@[49:0052) |     ├─IntegerLiteralExpression { Value = 123 }
//@[54:0057) |     ├─IntegerLiteralExpression { Value = 234 }
//@[59:0062) |     ├─IntegerLiteralExpression { Value = 456 }
//@[64:0079) |     └─ObjectExpression
//@[66:0077) |       └─ObjectPropertyExpression
//@[66:0069) |         ├─StringLiteralExpression { Value = xyz }
//@[71:0077) |         └─StringLiteralExpression { Value = xxxx }
var w81___ = /* xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx */ true ? 1234567890 : 1234567890
//@[00:0081) ├─DeclaredVariableExpression { Name = w81___ }
//@[51:0081) | └─TernaryExpression
//@[51:0055) |   ├─BooleanLiteralExpression { Value = True }
//@[58:0068) |   ├─IntegerLiteralExpression { Value = 1234567890 }
//@[71:0081) |   └─IntegerLiteralExpression { Value = 1234567890 }
var w82___ = /* xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx */ true ? 1234567890 : 1234567890
//@[00:0082) ├─DeclaredVariableExpression { Name = w82___ }
//@[52:0082) | └─TernaryExpression
//@[52:0056) |   ├─BooleanLiteralExpression { Value = True }
//@[59:0069) |   ├─IntegerLiteralExpression { Value = 1234567890 }
//@[72:0082) |   └─IntegerLiteralExpression { Value = 1234567890 }

////////////////////////////////////////////////////////////////////////////////
////////////////////////// Baselines for line breakers /////////////////////////
////////////////////////////////////////////////////////////////////////////////
var forceBreak1 = {
//@[00:0035) ├─DeclaredVariableExpression { Name = forceBreak1 }
//@[18:0035) | └─ObjectExpression
    foo: true
//@[04:0013) |   └─ObjectPropertyExpression
//@[04:0007) |     ├─StringLiteralExpression { Value = foo }
//@[09:0013) |     └─BooleanLiteralExpression { Value = True }
}
var forceBreak2 = {
//@[00:0047) ├─DeclaredVariableExpression { Name = forceBreak2 }
//@[18:0047) | └─ObjectExpression
    foo: true, bar: false
//@[04:0013) |   ├─ObjectPropertyExpression
//@[04:0007) |   | ├─StringLiteralExpression { Value = foo }
//@[09:0013) |   | └─BooleanLiteralExpression { Value = True }
//@[15:0025) |   └─ObjectPropertyExpression
//@[15:0018) |     ├─StringLiteralExpression { Value = bar }
//@[20:0025) |     └─BooleanLiteralExpression { Value = False }
}
var forceBreak3 = [1, 2, {
//@[00:0049) ├─DeclaredVariableExpression { Name = forceBreak3 }
//@[18:0049) | └─ArrayExpression
//@[19:0020) |   ├─IntegerLiteralExpression { Value = 1 }
//@[22:0023) |   ├─IntegerLiteralExpression { Value = 2 }
//@[25:0042) |   ├─ObjectExpression
    foo: true }, 3, 4]
//@[04:0013) |   | └─ObjectPropertyExpression
//@[04:0007) |   |   ├─StringLiteralExpression { Value = foo }
//@[09:0013) |   |   └─BooleanLiteralExpression { Value = True }
//@[17:0018) |   ├─IntegerLiteralExpression { Value = 3 }
//@[20:0021) |   └─IntegerLiteralExpression { Value = 4 }
var forceBreak4 = { foo: true, bar: false // force break
//@[00:0058) ├─DeclaredVariableExpression { Name = forceBreak4 }
//@[18:0058) | └─ObjectExpression
//@[20:0029) |   ├─ObjectPropertyExpression
//@[20:0023) |   | ├─StringLiteralExpression { Value = foo }
//@[25:0029) |   | └─BooleanLiteralExpression { Value = True }
//@[31:0041) |   └─ObjectPropertyExpression
//@[31:0034) |     ├─StringLiteralExpression { Value = bar }
//@[36:0041) |     └─BooleanLiteralExpression { Value = False }
}
var forceBreak5 = { foo: true
//@[00:0048) ├─DeclaredVariableExpression { Name = forceBreak5 }
//@[18:0048) | └─ObjectExpression
//@[20:0029) |   └─ObjectPropertyExpression
//@[20:0023) |     ├─StringLiteralExpression { Value = foo }
//@[25:0029) |     └─BooleanLiteralExpression { Value = True }
/* force break */}
var forceBreak6 = { foo: true
//@[00:0076) ├─DeclaredVariableExpression { Name = forceBreak6 }
//@[18:0076) | └─ObjectExpression
//@[20:0029) |   ├─ObjectPropertyExpression
//@[20:0023) |   | ├─StringLiteralExpression { Value = foo }
//@[25:0029) |   | └─BooleanLiteralExpression { Value = True }
    bar: false
//@[04:0014) |   ├─ObjectPropertyExpression
//@[04:0007) |   | ├─StringLiteralExpression { Value = bar }
//@[09:0014) |   | └─BooleanLiteralExpression { Value = False }
    baz: 123
//@[04:0012) |   └─ObjectPropertyExpression
//@[04:0007) |     ├─StringLiteralExpression { Value = baz }
//@[09:0012) |     └─IntegerLiteralExpression { Value = 123 }
/* force break */}
var forceBreak7 = [1, 2 // force break
//@[00:0040) ├─DeclaredVariableExpression { Name = forceBreak7 }
//@[18:0040) | └─ArrayExpression
//@[19:0020) |   ├─IntegerLiteralExpression { Value = 1 }
//@[22:0023) |   └─IntegerLiteralExpression { Value = 2 }
]
var forceBreak8 = [1, 2
//@[00:0047) ├─DeclaredVariableExpression { Name = forceBreak8 }
//@[18:0047) | └─ArrayExpression
//@[19:0020) |   ├─IntegerLiteralExpression { Value = 1 }
//@[22:0023) |   └─IntegerLiteralExpression { Value = 2 }
    /* force break */ ]
var forceBreak9 = [1, 2, {
//@[00:0058) ├─DeclaredVariableExpression { Name = forceBreak9 }
//@[18:0058) | └─ArrayExpression
//@[19:0020) |   ├─IntegerLiteralExpression { Value = 1 }
//@[22:0023) |   ├─IntegerLiteralExpression { Value = 2 }
//@[25:0057) |   └─ObjectExpression
    foo: true
//@[04:0013) |     ├─ObjectPropertyExpression
//@[04:0007) |     | ├─StringLiteralExpression { Value = foo }
//@[09:0013) |     | └─BooleanLiteralExpression { Value = True }
    bar: false
//@[04:0014) |     └─ObjectPropertyExpression
//@[04:0007) |       ├─StringLiteralExpression { Value = bar }
//@[09:0014) |       └─BooleanLiteralExpression { Value = False }
}]
var forceBreak10 = [1, 2, intersection({ foo: true, bar: false }, {
//@[00:0082) ├─DeclaredVariableExpression { Name = forceBreak10 }
//@[19:0082) | └─ArrayExpression
//@[20:0021) |   ├─IntegerLiteralExpression { Value = 1 }
//@[23:0024) |   ├─IntegerLiteralExpression { Value = 2 }
//@[26:0081) |   └─FunctionCallExpression { Name = intersection }
//@[39:0064) |     ├─ObjectExpression
//@[41:0050) |     | ├─ObjectPropertyExpression
//@[41:0044) |     | | ├─StringLiteralExpression { Value = foo }
//@[46:0050) |     | | └─BooleanLiteralExpression { Value = True }
//@[52:0062) |     | └─ObjectPropertyExpression
//@[52:0055) |     |   ├─StringLiteralExpression { Value = bar }
//@[57:0062) |     |   └─BooleanLiteralExpression { Value = False }
//@[66:0080) |     └─ObjectExpression
  foo: true})]
//@[02:0011) |       └─ObjectPropertyExpression
//@[02:0005) |         ├─StringLiteralExpression { Value = foo }
//@[07:0011) |         └─BooleanLiteralExpression { Value = True }

var forceBreak11 = true // comment
//@[00:0057) ├─DeclaredVariableExpression { Name = forceBreak11 }
//@[19:0057) | └─TernaryExpression
//@[19:0023) |   ├─BooleanLiteralExpression { Value = True }
    ? true
//@[06:0010) |   ├─BooleanLiteralExpression { Value = True }
    : false
//@[06:0011) |   └─BooleanLiteralExpression { Value = False }
var forceBreak12 = true ? true // comment
//@[00:0053) ├─DeclaredVariableExpression { Name = forceBreak12 }
//@[19:0053) | └─TernaryExpression
//@[19:0023) |   ├─BooleanLiteralExpression { Value = True }
//@[26:0030) |   ├─BooleanLiteralExpression { Value = True }
    : false
//@[06:0011) |   └─BooleanLiteralExpression { Value = False }
var forceBreak13 = true
//@[00:0057) ├─DeclaredVariableExpression { Name = forceBreak13 }
//@[19:0057) | └─TernaryExpression
//@[19:0023) |   ├─BooleanLiteralExpression { Value = True }
    ? true // comment
//@[06:0010) |   ├─BooleanLiteralExpression { Value = True }
    : false
//@[06:0011) |   └─BooleanLiteralExpression { Value = False }
var forceBreak14 = true ? {
//@[00:0049) ├─DeclaredVariableExpression { Name = forceBreak14 }
//@[19:0049) | └─TernaryExpression
//@[19:0023) |   ├─BooleanLiteralExpression { Value = True }
//@[26:0041) |   ├─ObjectExpression
    foo: 42
//@[04:0011) |   | └─ObjectPropertyExpression
//@[04:0007) |   |   ├─StringLiteralExpression { Value = foo }
//@[09:0011) |   |   └─IntegerLiteralExpression { Value = 42 }
} : false
//@[04:0009) |   └─BooleanLiteralExpression { Value = False }
var forceBreak15 = true ? { foo: 0 } : {
//@[00:0052) └─DeclaredVariableExpression { Name = forceBreak15 }
//@[19:0052)   └─TernaryExpression
//@[19:0023)     ├─BooleanLiteralExpression { Value = True }
//@[26:0036)     ├─ObjectExpression
//@[28:0034)     | └─ObjectPropertyExpression
//@[28:0031)     |   ├─StringLiteralExpression { Value = foo }
//@[33:0034)     |   └─IntegerLiteralExpression { Value = 0 }
//@[39:0052)     └─ObjectExpression
    bar: 1}
//@[04:0010)       └─ObjectPropertyExpression
//@[04:0007)         ├─StringLiteralExpression { Value = bar }
//@[09:0010)         └─IntegerLiteralExpression { Value = 1 }


