////////////////////////////////////////////////////////////////////////////////
//////////////////////////// Baselines for width 40 ////////////////////////////
////////////////////////////////////////////////////////////////////////////////
var w38 = [true, /* xxxxx */ true, 12]      // suffix
//@[04:07) [no-unused-vars (Warning)] Variable "w38" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |w38|
var w39 = [true, true
//@[04:07) [no-unused-vars (Warning)] Variable "w39" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |w39|
    true, true, 123]
var w40 =[true
//@[04:07) [no-unused-vars (Warning)] Variable "w40" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |w40|
    true, 1234/* xxxxx */]  // suffix
var w41 =[ true, true, true, true, 12345 ]
//@[04:07) [no-unused-vars (Warning)] Variable "w41" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |w41|
var w42 =[true, /* xxx */ 12 /* xx */, 1]
//@[04:07) [no-unused-vars (Warning)] Variable "w42" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |w42|

var w38_= { foo: true, bar: 1234567
//@[04:08) [no-unused-vars (Warning)] Variable "w38_" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |w38_|
} // suffix
var        w39_= { foo: true
//@[11:15) [no-unused-vars (Warning)] Variable "w39_" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |w39_|
  bar: 12345678 } // suffix
    var w40_= { foo: 1, bar:    1 /* xxxx */ }
//@[08:12) [no-unused-vars (Warning)] Variable "w40_" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |w40_|
var w41_={ foo: true, bar    : 1234567890 }
//@[04:08) [no-unused-vars (Warning)] Variable "w41_" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |w41_|
var w42_= { foo: true
//@[04:08) [no-unused-vars (Warning)] Variable "w42_" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |w42_|
    bar: 12345678901 } // suffix

   var w38__ =    concat('xxxxxx', 'xxxxxx')
//@[07:12) [no-unused-vars (Warning)] Variable "w38__" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |w38__|
//@[18:44) [prefer-interpolation (Warning)] Use string interpolation instead of the concat function. (bicep core linter https://aka.ms/bicep/linter-diagnostics#prefer-interpolation) |concat('xxxxxx', 'xxxxxx')|
var w39__ = concat('xxxxxx', 'xxxxxxx'
//@[04:09) [no-unused-vars (Warning)] Variable "w39__" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |w39__|
//@[12:40) [prefer-interpolation (Warning)] Use string interpolation instead of the concat function. (bicep core linter https://aka.ms/bicep/linter-diagnostics#prefer-interpolation) |concat('xxxxxx', 'xxxxxxx'\n)|
) // suffix
var w40__ = concat('xxxxxx',
//@[04:09) [no-unused-vars (Warning)] Variable "w40__" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |w40__|
//@[12:40) [prefer-interpolation (Warning)] Use string interpolation instead of the concat function. (bicep core linter https://aka.ms/bicep/linter-diagnostics#prefer-interpolation) |concat('xxxxxx',\n'xxxxxxxx')|
'xxxxxxxx') // suffix

var        w41__= concat('xxxxx', 'xxxxxxxxxx')
//@[11:16) [no-unused-vars (Warning)] Variable "w41__" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |w41__|
//@[18:47) [prefer-interpolation (Warning)] Use string interpolation instead of the concat function. (bicep core linter https://aka.ms/bicep/linter-diagnostics#prefer-interpolation) |concat('xxxxx', 'xxxxxxxxxx')|
var w42__ = concat('xxxxx', 'xxxxxxxxxxx')
//@[04:09) [no-unused-vars (Warning)] Variable "w42__" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |w42__|
//@[12:42) [prefer-interpolation (Warning)] Use string interpolation instead of the concat function. (bicep core linter https://aka.ms/bicep/linter-diagnostics#prefer-interpolation) |concat('xxxxx', 'xxxxxxxxxxx')|

var w38___ = true? 'xxxxx' : 'xxxxxx'
//@[04:10) [no-unused-vars (Warning)] Variable "w38___" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |w38___|
var w39___ = true
//@[04:10) [no-unused-vars (Warning)] Variable "w39___" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |w39___|
? 'xxxxxx' : 'xxxxxx' // suffix
var w40___ = true ?'xxxxxx' : 'xxxxxxx'
//@[04:10) [no-unused-vars (Warning)] Variable "w40___" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |w40___|
var w41___ = true ? 'xxxxxxx' :         'xxxxxxx'
//@[04:10) [no-unused-vars (Warning)] Variable "w41___" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |w41___|
var w42___ = true ? 'xxxxxxx':'xxxxxxxx'
//@[04:10) [no-unused-vars (Warning)] Variable "w42___" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |w42___|

////////////////////////////////////////////////////////////////////////////////
//////////////////////////// Baselines for width 80 ////////////////////////////
////////////////////////////////////////////////////////////////////////////////
var w78 = [true, { foo: 'object width: 37' /* xxx */ }, 'xxxxxxxxxxxxxxxxxxx' ]
//@[04:07) [no-unused-vars (Warning)] Variable "w78" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |w78|
var w79 = [true
//@[04:07) [no-unused-vars (Warning)] Variable "w79" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |w79|
    { /* xxxx */ foo: 'object width: 38' }
    'xxxxxxxxxxxxxxxxxx' ]
var w80 = [true, { foo: 'object width: 39 xxxxxxxxxxx' }
//@[04:07) [no-unused-vars (Warning)] Variable "w80" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |w80|
    'xxxxxxxxxxxxxxxxxxx']
var w81 = [true, { foo: 'object width: 40 xxxxxxxxxxxx' }, 'xxxxxxxxxxxxxxxxxxx' ]
//@[04:07) [no-unused-vars (Warning)] Variable "w81" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |w81|
var w82 = [  true, concat(/* function width: 41 */123, 456) /* xxxxxxxxxxxxxxxx */ ]
//@[04:07) [no-unused-vars (Warning)] Variable "w82" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |w82|
//@[19:59) [prefer-interpolation (Warning)] Use string interpolation instead of the concat function. (bicep core linter https://aka.ms/bicep/linter-diagnostics#prefer-interpolation) |concat(/* function width: 41 */123, 456)|

var w78_ ={ foo: 123, /* xxxx */ baz: ['xxxxxxxxxxx', 'xxxxxxxxxxxxxxxxxx'] }
//@[04:08) [no-unused-vars (Warning)] Variable "w78_" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |w78_|
var w79_ = { foo: 123, bar: true, baz: ['xxxxxxxxxxx', 'xxxxxxxx'] }
//@[04:08) [no-unused-vars (Warning)] Variable "w79_" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |w79_|
var w80_ = { foo: 123, bar: true, baz: ['xxxxxxxxxxx'
//@[04:08) [no-unused-vars (Warning)] Variable "w80_" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |w80_|
'xxxxxxxxxxxxxxxxxxxx'] } // suffix
var w81_ = { foo: 123, bar: true, baz: ['xxxxxxxxxxx', 'xxxxxxxxxxxxxxxxxxxxx'] }
//@[04:08) [no-unused-vars (Warning)] Variable "w81_" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |w81_|
var w82_ = { foo: 123, bar: true, baz: ['array length: 41', 'xxxxxxxxxxxxxxxxx'] }
//@[04:08) [no-unused-vars (Warning)] Variable "w82_" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |w82_|

var w78__ = union({ foo: 'xxxxx' }, { bar: 'xxxxxxxxx' }, { baz: 'xxxxxxxxx'})
//@[04:09) [no-unused-vars (Warning)] Variable "w78__" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |w78__|
var w79__ = union({ foo: 'xxxxx' }, { bar: 'xxxxxxxxx' },
//@[04:09) [no-unused-vars (Warning)] Variable "w79__" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |w79__|
    { baz: 'xxxxxxxxxx'}) // suffix
var w80__ = union(
//@[04:09) [no-unused-vars (Warning)] Variable "w80__" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |w80__|
    { foo: 'xxxxxx' },
    { bar: 'xxxxxx' },
    { baz: 'xxxxxxxxxxxxx'})
var w81__ = union({ foo: 'x' } /* xxx */, any({ baz: 'func call length: 38  ' }))
//@[04:09) [no-unused-vars (Warning)] Variable "w81__" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |w81__|
var w82__ = union({ foo: 'x', bar: 'x' }, any({ baz: 'func call length: 39   ' }))
//@[04:09) [no-unused-vars (Warning)] Variable "w82__" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |w82__|

var w78___ = /* xxxxxxxxxxxxxxxxxxxxxxxxxxxx */ true
//@[04:10) [no-unused-vars (Warning)] Variable "w78___" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |w78___|
? 1234567890
: 1234567890
var w79___ = /* xxxxxxxxxxxxxxxxxxxxxxxxxxxxx */ true ? { foo: 1 } : [12345678]
//@[04:10) [no-unused-vars (Warning)] Variable "w79___" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |w79___|
var w80___ = true ? { foo: true, bar: false } : [123, 234, 456, { xyz: 'xxxx' }]
//@[04:10) [no-unused-vars (Warning)] Variable "w80___" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |w80___|
var w81___ = /* xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx */ true ? 1234567890 : 1234567890
//@[04:10) [no-unused-vars (Warning)] Variable "w81___" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |w81___|
var w82___ = /* xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx */ true ? 1234567890 : 1234567890
//@[04:10) [no-unused-vars (Warning)] Variable "w82___" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |w82___|


var w80_________ = union(/*******************************************/ {}, {}, {
//@[04:16) [no-unused-vars (Warning)] Variable "w80_________" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |w80_________|
    foo: true
    bar: false
})
var w81_________ = union(/********************************************/ {}, {}, {
//@[04:16) [no-unused-vars (Warning)] Variable "w81_________" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |w81_________|
    foo: true
    bar: false
})

var w80__________ = union(/******************************************/ {}, {}, {
//@[04:17) [no-unused-vars (Warning)] Variable "w80__________" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |w80__________|
    foo: true
    w80: union(/***********************************************************/ {}, {
        baz: 123
    })
})

var w81__________ = union(/*******************************************/ {}, {}, {
//@[04:17) [no-unused-vars (Warning)] Variable "w81__________" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |w81__________|
    foo: true
    w81: union(/**********************************************************/ {}, {
        baz: 123
    })
})

////////////////////////////////////////////////////////////////////////////////
////////////////////////// Baselines for line breakers /////////////////////////
////////////////////////////////////////////////////////////////////////////////
var forceBreak0 = [
//@[04:15) [no-unused-vars (Warning)] Variable "forceBreak0" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |forceBreak0|
  1 ]

var forceBreak1 = {
//@[04:15) [no-unused-vars (Warning)] Variable "forceBreak1" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |forceBreak1|
    foo: true
}
var forceBreak2 = {
//@[04:15) [no-unused-vars (Warning)] Variable "forceBreak2" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |forceBreak2|
    foo: true, bar: false
}
var forceBreak3 = [1, 2, {
//@[04:15) [no-unused-vars (Warning)] Variable "forceBreak3" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |forceBreak3|
    foo: true }, 3, 4]
var forceBreak4 = { foo: true, bar: false // force break
//@[04:15) [no-unused-vars (Warning)] Variable "forceBreak4" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |forceBreak4|
}
var forceBreak5 = { foo: true
//@[04:15) [no-unused-vars (Warning)] Variable "forceBreak5" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |forceBreak5|
/* force break */}
var forceBreak6 = { foo: true
//@[04:15) [no-unused-vars (Warning)] Variable "forceBreak6" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |forceBreak6|
    bar: false
    baz: 123
/* force break */}
var forceBreak7 = [1, 2 // force break
//@[04:15) [no-unused-vars (Warning)] Variable "forceBreak7" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |forceBreak7|
]
var forceBreak8 = [1, 2
//@[04:15) [no-unused-vars (Warning)] Variable "forceBreak8" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |forceBreak8|
    /* force break */ ]
var forceBreak9 = [1, 2, {
//@[04:15) [no-unused-vars (Warning)] Variable "forceBreak9" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |forceBreak9|
    foo: true
    bar: false
}]
// Does not break immediate parent group, but breaks grandparent.
var forceBreak10 = [1, 2, intersection({ foo: true, bar: false }, {
//@[04:16) [no-unused-vars (Warning)] Variable "forceBreak10" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |forceBreak10|
  foo: true})]

var forceBreak11 = true // comment
//@[04:16) [no-unused-vars (Warning)] Variable "forceBreak11" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |forceBreak11|
    ? true
    : false
var forceBreak12 = true ? true // comment
//@[04:16) [no-unused-vars (Warning)] Variable "forceBreak12" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |forceBreak12|
    : false
var forceBreak13 = true
//@[04:16) [no-unused-vars (Warning)] Variable "forceBreak13" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |forceBreak13|
    ? true // comment
    : false
var forceBreak14 = true ? {
//@[04:16) [no-unused-vars (Warning)] Variable "forceBreak14" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |forceBreak14|
    foo: 42
} : false
var forceBreak15 = true ? { foo: 0 } : {
//@[04:16) [no-unused-vars (Warning)] Variable "forceBreak15" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |forceBreak15|
    bar: 1}

var forceBreak16 = union({ foo: 0 }, {
//@[04:16) [no-unused-vars (Warning)] Variable "forceBreak16" is declared but never used. (bicep core linter https://aka.ms/bicep/linter-diagnostics#no-unused-vars) |forceBreak16|
    foo: 123
    bar: 456
} // comment
)

