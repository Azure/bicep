////////////////////////////////////////////////////////////////////////////////
//////////////////////////// Baselines for width 40 ////////////////////////////
////////////////////////////////////////////////////////////////////////////////
var w38 = [true, /* xxxxx */ true, 12]      // suffix
//@[04:07) Variable w38. Type: [true, true, 12]. Declaration start char: 0, length: 38
var w39 = [true, true
//@[04:07) Variable w39. Type: [true, true, true, true, 123]. Declaration start char: 0, length: 42
    true, true, 123]
var w40 =[true
//@[04:07) Variable w40. Type: [true, true, 1234]. Declaration start char: 0, length: 41
    true, 1234/* xxxxx */]  // suffix
var w41 =[ true, true, true, true, 12345 ]
//@[04:07) Variable w41. Type: [true, true, true, true, 12345]. Declaration start char: 0, length: 42
var w42 =[true, /* xxx */ 12 /* xx */, 1]
//@[04:07) Variable w42. Type: [true, 12, 1]. Declaration start char: 0, length: 41

var w38_= { foo: true, bar: 1234567
//@[04:08) Variable w38_. Type: object. Declaration start char: 0, length: 37
} // suffix
var        w39_= { foo: true
//@[11:15) Variable w39_. Type: object. Declaration start char: 0, length: 46
  bar: 12345678 } // suffix
    var w40_= { foo: 1, bar:    1 /* xxxx */ }
//@[08:12) Variable w40_. Type: object. Declaration start char: 4, length: 42
var w41_={ foo: true, bar    : 1234567890 }
//@[04:08) Variable w41_. Type: object. Declaration start char: 0, length: 43
var w42_= { foo: true
//@[04:08) Variable w42_. Type: object. Declaration start char: 0, length: 44
    bar: 12345678901 } // suffix

   var w38__ =    concat('xxxxxx', 'xxxxxx')
//@[07:12) Variable w38__. Type: string. Declaration start char: 3, length: 41
var w39__ = concat('xxxxxx', 'xxxxxxx'
//@[04:09) Variable w39__. Type: string. Declaration start char: 0, length: 40
) // suffix
var w40__ = concat('xxxxxx',
//@[04:09) Variable w40__. Type: string. Declaration start char: 0, length: 40
'xxxxxxxx') // suffix

var        w41__= concat('xxxxx', 'xxxxxxxxxx')
//@[11:16) Variable w41__. Type: string. Declaration start char: 0, length: 47
var w42__ = concat('xxxxx', 'xxxxxxxxxxx')
//@[04:09) Variable w42__. Type: string. Declaration start char: 0, length: 42

var w38___ = true? 'xxxxx' : 'xxxxxx'
//@[04:10) Variable w38___. Type: 'xxxxx'. Declaration start char: 0, length: 37
var w39___ = true
//@[04:10) Variable w39___. Type: 'xxxxxx'. Declaration start char: 0, length: 39
? 'xxxxxx' : 'xxxxxx' // suffix
var w40___ = true ?'xxxxxx' : 'xxxxxxx'
//@[04:10) Variable w40___. Type: 'xxxxxx'. Declaration start char: 0, length: 39
var w41___ = true ? 'xxxxxxx' :         'xxxxxxx'
//@[04:10) Variable w41___. Type: 'xxxxxxx'. Declaration start char: 0, length: 49
var w42___ = true ? 'xxxxxxx':'xxxxxxxx'
//@[04:10) Variable w42___. Type: 'xxxxxxx'. Declaration start char: 0, length: 40

////////////////////////////////////////////////////////////////////////////////
//////////////////////////// Baselines for width 80 ////////////////////////////
////////////////////////////////////////////////////////////////////////////////
var w78 = [true, { foo: 'object width: 37' /* xxx */ }, 'xxxxxxxxxxxxxxxxxxx' ]
//@[04:07) Variable w78. Type: [true, object, 'xxxxxxxxxxxxxxxxxxx']. Declaration start char: 0, length: 79
var w79 = [true
//@[04:07) Variable w79. Type: [true, object, 'xxxxxxxxxxxxxxxxxx']. Declaration start char: 0, length: 85
    { /* xxxx */ foo: 'object width: 38' }
    'xxxxxxxxxxxxxxxxxx' ]
var w80 = [true, { foo: 'object width: 39 xxxxxxxxxxx' }
//@[04:07) Variable w80. Type: [true, object, 'xxxxxxxxxxxxxxxxxxx']. Declaration start char: 0, length: 83
    'xxxxxxxxxxxxxxxxxxx']
var w81 = [true, { foo: 'object width: 40 xxxxxxxxxxxx' }, 'xxxxxxxxxxxxxxxxxxx' ]
//@[04:07) Variable w81. Type: [true, object, 'xxxxxxxxxxxxxxxxxxx']. Declaration start char: 0, length: 82
var w82 = [  true, concat(/* function width: 41 */123, 456) /* xxxxxxxxxxxxxxxx */ ]
//@[04:07) Variable w82. Type: [true, string]. Declaration start char: 0, length: 84

var w78_ ={ foo: 123, /* xxxx */ baz: ['xxxxxxxxxxx', 'xxxxxxxxxxxxxxxxxx'] }
//@[04:08) Variable w78_. Type: object. Declaration start char: 0, length: 77
var w79_ = { foo: 123, bar: true, baz: ['xxxxxxxxxxx', 'xxxxxxxx'] }
//@[04:08) Variable w79_. Type: object. Declaration start char: 0, length: 68
var w80_ = { foo: 123, bar: true, baz: ['xxxxxxxxxxx'
//@[04:08) Variable w80_. Type: object. Declaration start char: 0, length: 79
'xxxxxxxxxxxxxxxxxxxx'] } // suffix
var w81_ = { foo: 123, bar: true, baz: ['xxxxxxxxxxx', 'xxxxxxxxxxxxxxxxxxxxx'] }
//@[04:08) Variable w81_. Type: object. Declaration start char: 0, length: 81
var w82_ = { foo: 123, bar: true, baz: ['array length: 41', 'xxxxxxxxxxxxxxxxx'] }
//@[04:08) Variable w82_. Type: object. Declaration start char: 0, length: 82

var w78__ = union({ foo: 'xxxxx' }, { bar: 'xxxxxxxxx' }, { baz: 'xxxxxxxxx'})
//@[04:09) Variable w78__. Type: { foo: 'xxxxx', bar: 'xxxxxxxxx', baz: 'xxxxxxxxx' }. Declaration start char: 0, length: 78
var w79__ = union({ foo: 'xxxxx' }, { bar: 'xxxxxxxxx' },
//@[04:09) Variable w79__. Type: { foo: 'xxxxx', bar: 'xxxxxxxxx', baz: 'xxxxxxxxxx' }. Declaration start char: 0, length: 83
    { baz: 'xxxxxxxxxx'}) // suffix
var w80__ = union(
//@[04:09) Variable w80__. Type: { foo: 'xxxxxx', bar: 'xxxxxx', baz: 'xxxxxxxxxxxxx' }. Declaration start char: 0, length: 93
    { foo: 'xxxxxx' },
    { bar: 'xxxxxx' },
    { baz: 'xxxxxxxxxxxxx'})
var w81__ = union({ foo: 'x' } /* xxx */, any({ baz: 'func call length: 38  ' }))
//@[04:09) Variable w81__. Type: object. Declaration start char: 0, length: 81
var w82__ = union({ foo: 'x', bar: 'x' }, any({ baz: 'func call length: 39   ' }))
//@[04:09) Variable w82__. Type: object. Declaration start char: 0, length: 82

var w78___ = /* xxxxxxxxxxxxxxxxxxxxxxxxxxxx */ true
//@[04:10) Variable w78___. Type: 1234567890. Declaration start char: 0, length: 78
? 1234567890
: 1234567890
var w79___ = /* xxxxxxxxxxxxxxxxxxxxxxxxxxxxx */ true ? { foo: 1 } : [12345678]
//@[04:10) Variable w79___. Type: object. Declaration start char: 0, length: 79
var w80___ = true ? { foo: true, bar: false } : [123, 234, 456, { xyz: 'xxxx' }]
//@[04:10) Variable w80___. Type: object. Declaration start char: 0, length: 80
var w81___ = /* xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx */ true ? 1234567890 : 1234567890
//@[04:10) Variable w81___. Type: 1234567890. Declaration start char: 0, length: 81
var w82___ = /* xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx */ true ? 1234567890 : 1234567890
//@[04:10) Variable w82___. Type: 1234567890. Declaration start char: 0, length: 82


var w80_________ = union(/*******************************************/ {}, {}, {
//@[04:16) Variable w80_________. Type: { bar: false, foo: true }. Declaration start char: 0, length: 112
    foo: true
    bar: false
})
var w81_________ = union(/********************************************/ {}, {}, {
//@[04:16) Variable w81_________. Type: { bar: false, foo: true }. Declaration start char: 0, length: 113
    foo: true
    bar: false
})

var w80__________ = union(/******************************************/ {}, {}, {
//@[04:17) Variable w80__________. Type: { foo: true, w80: { baz: 123 } }. Declaration start char: 0, length: 204
    foo: true
    w80: union(/***********************************************************/ {}, {
        baz: 123
    })
})

var w81__________ = union(/*******************************************/ {}, {}, {
//@[04:17) Variable w81__________. Type: { foo: true, w81: { baz: 123 } }. Declaration start char: 0, length: 204
    foo: true
    w81: union(/**********************************************************/ {}, {
        baz: 123
    })
})

////////////////////////////////////////////////////////////////////////////////
////////////////////////// Baselines for line breakers /////////////////////////
////////////////////////////////////////////////////////////////////////////////
var forceBreak0 = [
//@[04:15) Variable forceBreak0. Type: [1]. Declaration start char: 0, length: 25
  1 ]

var forceBreak1 = {
//@[04:15) Variable forceBreak1. Type: object. Declaration start char: 0, length: 35
    foo: true
}
var forceBreak2 = {
//@[04:15) Variable forceBreak2. Type: object. Declaration start char: 0, length: 47
    foo: true, bar: false
}
var forceBreak3 = [1, 2, {
//@[04:15) Variable forceBreak3. Type: [1, 2, object, 3, 4]. Declaration start char: 0, length: 49
    foo: true }, 3, 4]
var forceBreak4 = { foo: true, bar: false // force break
//@[04:15) Variable forceBreak4. Type: object. Declaration start char: 0, length: 58
}
var forceBreak5 = { foo: true
//@[04:15) Variable forceBreak5. Type: object. Declaration start char: 0, length: 48
/* force break */}
var forceBreak6 = { foo: true
//@[04:15) Variable forceBreak6. Type: object. Declaration start char: 0, length: 76
    bar: false
    baz: 123
/* force break */}
var forceBreak7 = [1, 2 // force break
//@[04:15) Variable forceBreak7. Type: [1, 2]. Declaration start char: 0, length: 40
]
var forceBreak8 = [1, 2
//@[04:15) Variable forceBreak8. Type: [1, 2]. Declaration start char: 0, length: 47
    /* force break */ ]
var forceBreak9 = [1, 2, {
//@[04:15) Variable forceBreak9. Type: [1, 2, object]. Declaration start char: 0, length: 58
    foo: true
    bar: false
}]
// Does not break immediate parent group, but breaks grandparent.
var forceBreak10 = [1, 2, intersection({ foo: true, bar: false }, {
//@[04:16) Variable forceBreak10. Type: [1, 2, { foo: true }]. Declaration start char: 0, length: 82
  foo: true})]

var forceBreak11 = true // comment
//@[04:16) Variable forceBreak11. Type: true. Declaration start char: 0, length: 57
    ? true
    : false
var forceBreak12 = true ? true // comment
//@[04:16) Variable forceBreak12. Type: true. Declaration start char: 0, length: 53
    : false
var forceBreak13 = true
//@[04:16) Variable forceBreak13. Type: true. Declaration start char: 0, length: 57
    ? true // comment
    : false
var forceBreak14 = true ? {
//@[04:16) Variable forceBreak14. Type: object. Declaration start char: 0, length: 49
    foo: 42
} : false
var forceBreak15 = true ? { foo: 0 } : {
//@[04:16) Variable forceBreak15. Type: object. Declaration start char: 0, length: 52
    bar: 1}

var forceBreak16 = union({ foo: 0 }, {
//@[04:16) Variable forceBreak16. Type: { foo: 123, bar: 456 }. Declaration start char: 0, length: 79
    foo: 123
    bar: 456
} // comment
)

