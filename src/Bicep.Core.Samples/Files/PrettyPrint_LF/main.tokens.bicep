////////////////////////////////////////////////////////////////////////////////
//@[080:081) NewLine |\n|
//////////////////////////// Baselines for width 40 ////////////////////////////
//@[080:081) NewLine |\n|
////////////////////////////////////////////////////////////////////////////////
//@[080:081) NewLine |\n|
var w38 = [true, /* xxxxx */ true, 12]      // suffix
//@[000:003) Identifier |var|
//@[004:007) Identifier |w38|
//@[008:009) Assignment |=|
//@[010:011) LeftSquare |[|
//@[011:015) TrueKeyword |true|
//@[015:016) Comma |,|
//@[029:033) TrueKeyword |true|
//@[033:034) Comma |,|
//@[035:037) Integer |12|
//@[037:038) RightSquare |]|
//@[053:054) NewLine |\n|
var w39 = [true, true
//@[000:003) Identifier |var|
//@[004:007) Identifier |w39|
//@[008:009) Assignment |=|
//@[010:011) LeftSquare |[|
//@[011:015) TrueKeyword |true|
//@[015:016) Comma |,|
//@[017:021) TrueKeyword |true|
//@[021:022) NewLine |\n|
    true, true, 123]
//@[004:008) TrueKeyword |true|
//@[008:009) Comma |,|
//@[010:014) TrueKeyword |true|
//@[014:015) Comma |,|
//@[016:019) Integer |123|
//@[019:020) RightSquare |]|
//@[020:021) NewLine |\n|
var w40 =[
//@[000:003) Identifier |var|
//@[004:007) Identifier |w40|
//@[008:009) Assignment |=|
//@[009:010) LeftSquare |[|
//@[010:011) NewLine |\n|
    true, true, 1234/* xxxxx */]  // suffix
//@[004:008) TrueKeyword |true|
//@[008:009) Comma |,|
//@[010:014) TrueKeyword |true|
//@[014:015) Comma |,|
//@[016:020) Integer |1234|
//@[031:032) RightSquare |]|
//@[043:044) NewLine |\n|
var w41 =[ true, true, true, true, 12345 ]
//@[000:003) Identifier |var|
//@[004:007) Identifier |w41|
//@[008:009) Assignment |=|
//@[009:010) LeftSquare |[|
//@[011:015) TrueKeyword |true|
//@[015:016) Comma |,|
//@[017:021) TrueKeyword |true|
//@[021:022) Comma |,|
//@[023:027) TrueKeyword |true|
//@[027:028) Comma |,|
//@[029:033) TrueKeyword |true|
//@[033:034) Comma |,|
//@[035:040) Integer |12345|
//@[041:042) RightSquare |]|
//@[042:043) NewLine |\n|
var w42 =[true, /* xxx */ 12 /* xx */, 1]
//@[000:003) Identifier |var|
//@[004:007) Identifier |w42|
//@[008:009) Assignment |=|
//@[009:010) LeftSquare |[|
//@[010:014) TrueKeyword |true|
//@[014:015) Comma |,|
//@[026:028) Integer |12|
//@[037:038) Comma |,|
//@[039:040) Integer |1|
//@[040:041) RightSquare |]|
//@[041:043) NewLine |\n\n|

var w38_= { foo: true, bar: 1234567
//@[000:003) Identifier |var|
//@[004:008) Identifier |w38_|
//@[008:009) Assignment |=|
//@[010:011) LeftBrace |{|
//@[012:015) Identifier |foo|
//@[015:016) Colon |:|
//@[017:021) TrueKeyword |true|
//@[021:022) Comma |,|
//@[023:026) Identifier |bar|
//@[026:027) Colon |:|
//@[028:035) Integer |1234567|
//@[035:036) NewLine |\n|
} // suffix
//@[000:001) RightBrace |}|
//@[011:012) NewLine |\n|
var        w39_= { foo: true
//@[000:003) Identifier |var|
//@[011:015) Identifier |w39_|
//@[015:016) Assignment |=|
//@[017:018) LeftBrace |{|
//@[019:022) Identifier |foo|
//@[022:023) Colon |:|
//@[024:028) TrueKeyword |true|
//@[028:029) NewLine |\n|
  bar: 12345678 } // suffix
//@[002:005) Identifier |bar|
//@[005:006) Colon |:|
//@[007:015) Integer |12345678|
//@[016:017) RightBrace |}|
//@[027:028) NewLine |\n|
    var w40_= { foo: 1, bar:    1 /* xxxx */ }
//@[004:007) Identifier |var|
//@[008:012) Identifier |w40_|
//@[012:013) Assignment |=|
//@[014:015) LeftBrace |{|
//@[016:019) Identifier |foo|
//@[019:020) Colon |:|
//@[021:022) Integer |1|
//@[022:023) Comma |,|
//@[024:027) Identifier |bar|
//@[027:028) Colon |:|
//@[032:033) Integer |1|
//@[045:046) RightBrace |}|
//@[046:047) NewLine |\n|
var w41_={ foo: true, bar    : 1234567890 }
//@[000:003) Identifier |var|
//@[004:008) Identifier |w41_|
//@[008:009) Assignment |=|
//@[009:010) LeftBrace |{|
//@[011:014) Identifier |foo|
//@[014:015) Colon |:|
//@[016:020) TrueKeyword |true|
//@[020:021) Comma |,|
//@[022:025) Identifier |bar|
//@[029:030) Colon |:|
//@[031:041) Integer |1234567890|
//@[042:043) RightBrace |}|
//@[043:044) NewLine |\n|
var w42_= { foo: true
//@[000:003) Identifier |var|
//@[004:008) Identifier |w42_|
//@[008:009) Assignment |=|
//@[010:011) LeftBrace |{|
//@[012:015) Identifier |foo|
//@[015:016) Colon |:|
//@[017:021) TrueKeyword |true|
//@[021:022) NewLine |\n|
    bar: 12345678901 } // suffix
//@[004:007) Identifier |bar|
//@[007:008) Colon |:|
//@[009:020) Integer |12345678901|
//@[021:022) RightBrace |}|
//@[032:034) NewLine |\n\n|

   var w38__ =    concat('xxxxxx', 'xxxxxx')
//@[003:006) Identifier |var|
//@[007:012) Identifier |w38__|
//@[013:014) Assignment |=|
//@[018:024) Identifier |concat|
//@[024:025) LeftParen |(|
//@[025:033) StringComplete |'xxxxxx'|
//@[033:034) Comma |,|
//@[035:043) StringComplete |'xxxxxx'|
//@[043:044) RightParen |)|
//@[044:045) NewLine |\n|
var w39__ = concat('xxxxxx', 'xxxxxxx'
//@[000:003) Identifier |var|
//@[004:009) Identifier |w39__|
//@[010:011) Assignment |=|
//@[012:018) Identifier |concat|
//@[018:019) LeftParen |(|
//@[019:027) StringComplete |'xxxxxx'|
//@[027:028) Comma |,|
//@[029:038) StringComplete |'xxxxxxx'|
//@[038:039) NewLine |\n|
) // suffix
//@[000:001) RightParen |)|
//@[011:012) NewLine |\n|
var w40__ = concat('xxxxxx',
//@[000:003) Identifier |var|
//@[004:009) Identifier |w40__|
//@[010:011) Assignment |=|
//@[012:018) Identifier |concat|
//@[018:019) LeftParen |(|
//@[019:027) StringComplete |'xxxxxx'|
//@[027:028) Comma |,|
//@[028:029) NewLine |\n|
'xxxxxxxx') // suffix
//@[000:010) StringComplete |'xxxxxxxx'|
//@[010:011) RightParen |)|
//@[021:023) NewLine |\n\n|

var        w41__= concat('xxxxx'/* xxxxxxx */)
//@[000:003) Identifier |var|
//@[011:016) Identifier |w41__|
//@[016:017) Assignment |=|
//@[018:024) Identifier |concat|
//@[024:025) LeftParen |(|
//@[025:032) StringComplete |'xxxxx'|
//@[045:046) RightParen |)|
//@[046:047) NewLine |\n|
var w42__ = concat('xxxxx', 'xxxxxxxxxxx')
//@[000:003) Identifier |var|
//@[004:009) Identifier |w42__|
//@[010:011) Assignment |=|
//@[012:018) Identifier |concat|
//@[018:019) LeftParen |(|
//@[019:026) StringComplete |'xxxxx'|
//@[026:027) Comma |,|
//@[028:041) StringComplete |'xxxxxxxxxxx'|
//@[041:042) RightParen |)|
//@[042:044) NewLine |\n\n|

////////////////////////////////////////////////////////////////////////////////
//@[080:081) NewLine |\n|
//////////////////////////// Baselines for width 80 ////////////////////////////
//@[080:081) NewLine |\n|
////////////////////////////////////////////////////////////////////////////////
//@[080:081) NewLine |\n|
var w78 = [
//@[000:003) Identifier |var|
//@[004:007) Identifier |w78|
//@[008:009) Assignment |=|
//@[010:011) LeftSquare |[|
//@[011:012) NewLine |\n|
    true, { foo: 'object width: 37' /* xxx */ }, 'xxxxxxxxxxxxxxxxxx' ]
//@[004:008) TrueKeyword |true|
//@[008:009) Comma |,|
//@[010:011) LeftBrace |{|
//@[012:015) Identifier |foo|
//@[015:016) Colon |:|
//@[017:035) StringComplete |'object width: 37'|
//@[046:047) RightBrace |}|
//@[047:048) Comma |,|
//@[049:069) StringComplete |'xxxxxxxxxxxxxxxxxx'|
//@[070:071) RightSquare |]|
//@[071:072) NewLine |\n|
var w79 = [true
//@[000:003) Identifier |var|
//@[004:007) Identifier |w79|
//@[008:009) Assignment |=|
//@[010:011) LeftSquare |[|
//@[011:015) TrueKeyword |true|
//@[015:016) NewLine |\n|
    { /* xxxx */ foo: 'object width: 38' }
//@[004:005) LeftBrace |{|
//@[017:020) Identifier |foo|
//@[020:021) Colon |:|
//@[022:040) StringComplete |'object width: 38'|
//@[041:042) RightBrace |}|
//@[042:043) NewLine |\n|
    'xxxxxxxxxxxxxxxxxx' ]
//@[004:024) StringComplete |'xxxxxxxxxxxxxxxxxx'|
//@[025:026) RightSquare |]|
//@[026:027) NewLine |\n|
var w80 = [true, { foo: 'object width: 39 xxxxxxxxxxx' }
//@[000:003) Identifier |var|
//@[004:007) Identifier |w80|
//@[008:009) Assignment |=|
//@[010:011) LeftSquare |[|
//@[011:015) TrueKeyword |true|
//@[015:016) Comma |,|
//@[017:018) LeftBrace |{|
//@[019:022) Identifier |foo|
//@[022:023) Colon |:|
//@[024:054) StringComplete |'object width: 39 xxxxxxxxxxx'|
//@[055:056) RightBrace |}|
//@[056:057) NewLine |\n|
    'xxxxxxxxxxxxxxxxxxx']
//@[004:025) StringComplete |'xxxxxxxxxxxxxxxxxxx'|
//@[025:026) RightSquare |]|
//@[026:027) NewLine |\n|
var w81 = [true, { foo: 'object width: 40 xxxxxxxxxxxx' }, 'xxxxxxxxxxxxxxxxxx' ]
//@[000:003) Identifier |var|
//@[004:007) Identifier |w81|
//@[008:009) Assignment |=|
//@[010:011) LeftSquare |[|
//@[011:015) TrueKeyword |true|
//@[015:016) Comma |,|
//@[017:018) LeftBrace |{|
//@[019:022) Identifier |foo|
//@[022:023) Colon |:|
//@[024:055) StringComplete |'object width: 40 xxxxxxxxxxxx'|
//@[056:057) RightBrace |}|
//@[057:058) Comma |,|
//@[059:079) StringComplete |'xxxxxxxxxxxxxxxxxx'|
//@[080:081) RightSquare |]|
//@[081:082) NewLine |\n|
var w82 = [  true, concat(/* function width: 41 */123, 456) /* xxxxxxxxxxxxxxxx */ ]
//@[000:003) Identifier |var|
//@[004:007) Identifier |w82|
//@[008:009) Assignment |=|
//@[010:011) LeftSquare |[|
//@[013:017) TrueKeyword |true|
//@[017:018) Comma |,|
//@[019:025) Identifier |concat|
//@[025:026) LeftParen |(|
//@[050:053) Integer |123|
//@[053:054) Comma |,|
//@[055:058) Integer |456|
//@[058:059) RightParen |)|
//@[083:084) RightSquare |]|
//@[084:086) NewLine |\n\n|

var w78_ ={ foo: 123, /* xxxx */ baz: ['xxxxxxxxxxx', 'xxxxxxxxxxxxxxxxxx'] }
//@[000:003) Identifier |var|
//@[004:008) Identifier |w78_|
//@[009:010) Assignment |=|
//@[010:011) LeftBrace |{|
//@[012:015) Identifier |foo|
//@[015:016) Colon |:|
//@[017:020) Integer |123|
//@[020:021) Comma |,|
//@[033:036) Identifier |baz|
//@[036:037) Colon |:|
//@[038:039) LeftSquare |[|
//@[039:052) StringComplete |'xxxxxxxxxxx'|
//@[052:053) Comma |,|
//@[054:074) StringComplete |'xxxxxxxxxxxxxxxxxx'|
//@[074:075) RightSquare |]|
//@[076:077) RightBrace |}|
//@[077:078) NewLine |\n|
/* should print a newline after this */ var w79_ = { foo: 123, bar: true, baz: ['xxxxxxxxxxx', 'xxxxxxxx'] }
//@[040:043) Identifier |var|
//@[044:048) Identifier |w79_|
//@[049:050) Assignment |=|
//@[051:052) LeftBrace |{|
//@[053:056) Identifier |foo|
//@[056:057) Colon |:|
//@[058:061) Integer |123|
//@[061:062) Comma |,|
//@[063:066) Identifier |bar|
//@[066:067) Colon |:|
//@[068:072) TrueKeyword |true|
//@[072:073) Comma |,|
//@[074:077) Identifier |baz|
//@[077:078) Colon |:|
//@[079:080) LeftSquare |[|
//@[080:093) StringComplete |'xxxxxxxxxxx'|
//@[093:094) Comma |,|
//@[095:105) StringComplete |'xxxxxxxx'|
//@[105:106) RightSquare |]|
//@[107:108) RightBrace |}|
//@[108:109) NewLine |\n|
var w80_ = { foo: 123, bar: true, baz: [
//@[000:003) Identifier |var|
//@[004:008) Identifier |w80_|
//@[009:010) Assignment |=|
//@[011:012) LeftBrace |{|
//@[013:016) Identifier |foo|
//@[016:017) Colon |:|
//@[018:021) Integer |123|
//@[021:022) Comma |,|
//@[023:026) Identifier |bar|
//@[026:027) Colon |:|
//@[028:032) TrueKeyword |true|
//@[032:033) Comma |,|
//@[034:037) Identifier |baz|
//@[037:038) Colon |:|
//@[039:040) LeftSquare |[|
//@[040:041) NewLine |\n|
    'xxxxxxxxxxx', 'xxxxxxxxxxxxxxxxxxxx'] } // suffix
//@[004:017) StringComplete |'xxxxxxxxxxx'|
//@[017:018) Comma |,|
//@[019:041) StringComplete |'xxxxxxxxxxxxxxxxxxxx'|
//@[041:042) RightSquare |]|
//@[043:044) RightBrace |}|
//@[054:055) NewLine |\n|
var w81_ = { foo: 123, bar: true, baz: ['xxxxxxxxxxx', 'xxxxxxxxxxxxxxxxxxxxx'] }
//@[000:003) Identifier |var|
//@[004:008) Identifier |w81_|
//@[009:010) Assignment |=|
//@[011:012) LeftBrace |{|
//@[013:016) Identifier |foo|
//@[016:017) Colon |:|
//@[018:021) Integer |123|
//@[021:022) Comma |,|
//@[023:026) Identifier |bar|
//@[026:027) Colon |:|
//@[028:032) TrueKeyword |true|
//@[032:033) Comma |,|
//@[034:037) Identifier |baz|
//@[037:038) Colon |:|
//@[039:040) LeftSquare |[|
//@[040:053) StringComplete |'xxxxxxxxxxx'|
//@[053:054) Comma |,|
//@[055:078) StringComplete |'xxxxxxxxxxxxxxxxxxxxx'|
//@[078:079) RightSquare |]|
//@[080:081) RightBrace |}|
//@[081:082) NewLine |\n|
var w82_ = { foo: 123, bar: true, baz: ['array length: 41', 'xxxxxxxxxxxxxxxxx'] }
//@[000:003) Identifier |var|
//@[004:008) Identifier |w82_|
//@[009:010) Assignment |=|
//@[011:012) LeftBrace |{|
//@[013:016) Identifier |foo|
//@[016:017) Colon |:|
//@[018:021) Integer |123|
//@[021:022) Comma |,|
//@[023:026) Identifier |bar|
//@[026:027) Colon |:|
//@[028:032) TrueKeyword |true|
//@[032:033) Comma |,|
//@[034:037) Identifier |baz|
//@[037:038) Colon |:|
//@[039:040) LeftSquare |[|
//@[040:058) StringComplete |'array length: 41'|
//@[058:059) Comma |,|
//@[060:079) StringComplete |'xxxxxxxxxxxxxxxxx'|
//@[079:080) RightSquare |]|
//@[081:082) RightBrace |}|
//@[082:084) NewLine |\n\n|

var w78__ = union({ foo: 'xxxxx' }, { bar: 'xxxxxxxxx' }, { baz: 'xxxxxxxxx'})
//@[000:003) Identifier |var|
//@[004:009) Identifier |w78__|
//@[010:011) Assignment |=|
//@[012:017) Identifier |union|
//@[017:018) LeftParen |(|
//@[018:019) LeftBrace |{|
//@[020:023) Identifier |foo|
//@[023:024) Colon |:|
//@[025:032) StringComplete |'xxxxx'|
//@[033:034) RightBrace |}|
//@[034:035) Comma |,|
//@[036:037) LeftBrace |{|
//@[038:041) Identifier |bar|
//@[041:042) Colon |:|
//@[043:054) StringComplete |'xxxxxxxxx'|
//@[055:056) RightBrace |}|
//@[056:057) Comma |,|
//@[058:059) LeftBrace |{|
//@[060:063) Identifier |baz|
//@[063:064) Colon |:|
//@[065:076) StringComplete |'xxxxxxxxx'|
//@[076:077) RightBrace |}|
//@[077:078) RightParen |)|
//@[078:079) NewLine |\n|
var w79__ = union({ foo: 'xxxxx' }, { bar: 'xxxxxxxxx' },
//@[000:003) Identifier |var|
//@[004:009) Identifier |w79__|
//@[010:011) Assignment |=|
//@[012:017) Identifier |union|
//@[017:018) LeftParen |(|
//@[018:019) LeftBrace |{|
//@[020:023) Identifier |foo|
//@[023:024) Colon |:|
//@[025:032) StringComplete |'xxxxx'|
//@[033:034) RightBrace |}|
//@[034:035) Comma |,|
//@[036:037) LeftBrace |{|
//@[038:041) Identifier |bar|
//@[041:042) Colon |:|
//@[043:054) StringComplete |'xxxxxxxxx'|
//@[055:056) RightBrace |}|
//@[056:057) Comma |,|
//@[057:058) NewLine |\n|
    { baz: 'xxxxxxxxxx'}) // suffix
//@[004:005) LeftBrace |{|
//@[006:009) Identifier |baz|
//@[009:010) Colon |:|
//@[011:023) StringComplete |'xxxxxxxxxx'|
//@[023:024) RightBrace |}|
//@[024:025) RightParen |)|
//@[035:036) NewLine |\n|
var w80__ = union(
//@[000:003) Identifier |var|
//@[004:009) Identifier |w80__|
//@[010:011) Assignment |=|
//@[012:017) Identifier |union|
//@[017:018) LeftParen |(|
//@[018:019) NewLine |\n|
    { foo: 'xxxxxx' },
//@[004:005) LeftBrace |{|
//@[006:009) Identifier |foo|
//@[009:010) Colon |:|
//@[011:019) StringComplete |'xxxxxx'|
//@[020:021) RightBrace |}|
//@[021:022) Comma |,|
//@[022:023) NewLine |\n|
    { bar: 'xxxxxx' },
//@[004:005) LeftBrace |{|
//@[006:009) Identifier |bar|
//@[009:010) Colon |:|
//@[011:019) StringComplete |'xxxxxx'|
//@[020:021) RightBrace |}|
//@[021:022) Comma |,|
//@[022:023) NewLine |\n|
    { baz: 'xxxxxxxxxxxxx'})
//@[004:005) LeftBrace |{|
//@[006:009) Identifier |baz|
//@[009:010) Colon |:|
//@[011:026) StringComplete |'xxxxxxxxxxxxx'|
//@[026:027) RightBrace |}|
//@[027:028) RightParen |)|
//@[028:029) NewLine |\n|
var w81__ = union({ foo: 'x' } /* xxx */, any({ baz: 'func call length: 38  ' }))
//@[000:003) Identifier |var|
//@[004:009) Identifier |w81__|
//@[010:011) Assignment |=|
//@[012:017) Identifier |union|
//@[017:018) LeftParen |(|
//@[018:019) LeftBrace |{|
//@[020:023) Identifier |foo|
//@[023:024) Colon |:|
//@[025:028) StringComplete |'x'|
//@[029:030) RightBrace |}|
//@[040:041) Comma |,|
//@[042:045) Identifier |any|
//@[045:046) LeftParen |(|
//@[046:047) LeftBrace |{|
//@[048:051) Identifier |baz|
//@[051:052) Colon |:|
//@[053:077) StringComplete |'func call length: 38  '|
//@[078:079) RightBrace |}|
//@[079:080) RightParen |)|
//@[080:081) RightParen |)|
//@[081:082) NewLine |\n|
var w82__ = union({ foo: 'x', bar: 'x' }, any({ baz: 'func call length: 39   ' }))
//@[000:003) Identifier |var|
//@[004:009) Identifier |w82__|
//@[010:011) Assignment |=|
//@[012:017) Identifier |union|
//@[017:018) LeftParen |(|
//@[018:019) LeftBrace |{|
//@[020:023) Identifier |foo|
//@[023:024) Colon |:|
//@[025:028) StringComplete |'x'|
//@[028:029) Comma |,|
//@[030:033) Identifier |bar|
//@[033:034) Colon |:|
//@[035:038) StringComplete |'x'|
//@[039:040) RightBrace |}|
//@[040:041) Comma |,|
//@[042:045) Identifier |any|
//@[045:046) LeftParen |(|
//@[046:047) LeftBrace |{|
//@[048:051) Identifier |baz|
//@[051:052) Colon |:|
//@[053:078) StringComplete |'func call length: 39   '|
//@[079:080) RightBrace |}|
//@[080:081) RightParen |)|
//@[081:082) RightParen |)|
//@[082:084) NewLine |\n\n|

////////////////////////////////////////////////////////////////////////////////
//@[080:081) NewLine |\n|
////////////////////////// Baselines for line breakers /////////////////////////
//@[080:081) NewLine |\n|
////////////////////////////////////////////////////////////////////////////////
//@[080:081) NewLine |\n|
var forceBreak1 = {
//@[000:003) Identifier |var|
//@[004:015) Identifier |forceBreak1|
//@[016:017) Assignment |=|
//@[018:019) LeftBrace |{|
//@[019:020) NewLine |\n|
    foo: true
//@[004:007) Identifier |foo|
//@[007:008) Colon |:|
//@[009:013) TrueKeyword |true|
//@[013:014) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:002) NewLine |\n|
/* should print a newline after this */var forceBreak2 = {
//@[039:042) Identifier |var|
//@[043:054) Identifier |forceBreak2|
//@[055:056) Assignment |=|
//@[057:058) LeftBrace |{|
//@[058:059) NewLine |\n|
    foo: true, bar: false
//@[004:007) Identifier |foo|
//@[007:008) Colon |:|
//@[009:013) TrueKeyword |true|
//@[013:014) Comma |,|
//@[015:018) Identifier |bar|
//@[018:019) Colon |:|
//@[020:025) FalseKeyword |false|
//@[025:026) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:002) NewLine |\n|
var forceBreak3 = [1, 2, {
//@[000:003) Identifier |var|
//@[004:015) Identifier |forceBreak3|
//@[016:017) Assignment |=|
//@[018:019) LeftSquare |[|
//@[019:020) Integer |1|
//@[020:021) Comma |,|
//@[022:023) Integer |2|
//@[023:024) Comma |,|
//@[025:026) LeftBrace |{|
//@[026:027) NewLine |\n|
    foo: true }, 3, 4]
//@[004:007) Identifier |foo|
//@[007:008) Colon |:|
//@[009:013) TrueKeyword |true|
//@[014:015) RightBrace |}|
//@[015:016) Comma |,|
//@[017:018) Integer |3|
//@[018:019) Comma |,|
//@[020:021) Integer |4|
//@[021:022) RightSquare |]|
//@[022:023) NewLine |\n|
var forceBreak4 = { foo: true, bar: false // force break
//@[000:003) Identifier |var|
//@[004:015) Identifier |forceBreak4|
//@[016:017) Assignment |=|
//@[018:019) LeftBrace |{|
//@[020:023) Identifier |foo|
//@[023:024) Colon |:|
//@[025:029) TrueKeyword |true|
//@[029:030) Comma |,|
//@[031:034) Identifier |bar|
//@[034:035) Colon |:|
//@[036:041) FalseKeyword |false|
//@[056:057) NewLine |\n|
}
//@[000:001) RightBrace |}|
//@[001:002) NewLine |\n|
var forceBreak5 = { foo: true
//@[000:003) Identifier |var|
//@[004:015) Identifier |forceBreak5|
//@[016:017) Assignment |=|
//@[018:019) LeftBrace |{|
//@[020:023) Identifier |foo|
//@[023:024) Colon |:|
//@[025:029) TrueKeyword |true|
//@[029:030) NewLine |\n|
/* force break */}
//@[017:018) RightBrace |}|
//@[018:019) NewLine |\n|
var forceBreak6 = { foo: true
//@[000:003) Identifier |var|
//@[004:015) Identifier |forceBreak6|
//@[016:017) Assignment |=|
//@[018:019) LeftBrace |{|
//@[020:023) Identifier |foo|
//@[023:024) Colon |:|
//@[025:029) TrueKeyword |true|
//@[029:030) NewLine |\n|
    bar: false
//@[004:007) Identifier |bar|
//@[007:008) Colon |:|
//@[009:014) FalseKeyword |false|
//@[014:015) NewLine |\n|
    baz: 123
//@[004:007) Identifier |baz|
//@[007:008) Colon |:|
//@[009:012) Integer |123|
//@[012:013) NewLine |\n|
/* force break */}
//@[017:018) RightBrace |}|
//@[018:019) NewLine |\n|
var forceBreak7 = [1, 2 // force break
//@[000:003) Identifier |var|
//@[004:015) Identifier |forceBreak7|
//@[016:017) Assignment |=|
//@[018:019) LeftSquare |[|
//@[019:020) Integer |1|
//@[020:021) Comma |,|
//@[022:023) Integer |2|
//@[038:039) NewLine |\n|
]
//@[000:001) RightSquare |]|
//@[001:002) NewLine |\n|
var forceBreak8 = [1, 2
//@[000:003) Identifier |var|
//@[004:015) Identifier |forceBreak8|
//@[016:017) Assignment |=|
//@[018:019) LeftSquare |[|
//@[019:020) Integer |1|
//@[020:021) Comma |,|
//@[022:023) Integer |2|
//@[023:024) NewLine |\n|
    /* force break */ ]
//@[022:023) RightSquare |]|
//@[023:024) NewLine |\n|
var forceBreak9 = [1, 2, {
//@[000:003) Identifier |var|
//@[004:015) Identifier |forceBreak9|
//@[016:017) Assignment |=|
//@[018:019) LeftSquare |[|
//@[019:020) Integer |1|
//@[020:021) Comma |,|
//@[022:023) Integer |2|
//@[023:024) Comma |,|
//@[025:026) LeftBrace |{|
//@[026:027) NewLine |\n|
    foo: true
//@[004:007) Identifier |foo|
//@[007:008) Colon |:|
//@[009:013) TrueKeyword |true|
//@[013:014) NewLine |\n|
    bar: false
//@[004:007) Identifier |bar|
//@[007:008) Colon |:|
//@[009:014) FalseKeyword |false|
//@[014:015) NewLine |\n|
}]
//@[000:001) RightBrace |}|
//@[001:002) RightSquare |]|
//@[002:003) NewLine |\n|
var forceBreak10 = [1, 2, intersection({ foo: true, bar: false }, {
//@[000:003) Identifier |var|
//@[004:016) Identifier |forceBreak10|
//@[017:018) Assignment |=|
//@[019:020) LeftSquare |[|
//@[020:021) Integer |1|
//@[021:022) Comma |,|
//@[023:024) Integer |2|
//@[024:025) Comma |,|
//@[026:038) Identifier |intersection|
//@[038:039) LeftParen |(|
//@[039:040) LeftBrace |{|
//@[041:044) Identifier |foo|
//@[044:045) Colon |:|
//@[046:050) TrueKeyword |true|
//@[050:051) Comma |,|
//@[052:055) Identifier |bar|
//@[055:056) Colon |:|
//@[057:062) FalseKeyword |false|
//@[063:064) RightBrace |}|
//@[064:065) Comma |,|
//@[066:067) LeftBrace |{|
//@[067:068) NewLine |\n|
  foo: true})]
//@[002:005) Identifier |foo|
//@[005:006) Colon |:|
//@[007:011) TrueKeyword |true|
//@[011:012) RightBrace |}|
//@[012:013) RightParen |)|
//@[013:014) RightSquare |]|
//@[014:015) NewLine |\n|

//@[000:000) EndOfFile ||
