////////////////////////////////////////////////////////////////////////////////
//////////////////////////// Baselines for width 40 ////////////////////////////
////////////////////////////////////////////////////////////////////////////////
var w38 = [true, /* xxxxx */ true, 12]      // suffix
//@    "w38": [
//@      true,
//@      true,
//@      12
//@    ],
var w39 = [true, true
//@    "w39": [
//@      true,
//@      true,
//@    ],
    true, true, 123]
//@      true,
//@      true,
//@      123
var w40 =[true
//@    "w40": [
//@      true,
//@    ],
    true, 1234/* xxxxx */]  // suffix
//@      true,
//@      1234
var w41 =[ true, true, true, true, 12345 ]
//@    "w41": [
//@      true,
//@      true,
//@      true,
//@      true,
//@      12345
//@    ],
var w42 =[true, /* xxx */ 12 /* xx */, 1]
//@    "w42": [
//@      true,
//@      12,
//@      1
//@    ],

var w38_= { foo: true, bar: 1234567
//@    "w38_": {
//@      "foo": true,
//@      "bar": 1234567
//@    },
} // suffix
var        w39_= { foo: true
//@    "w39_": {
//@      "foo": true,
//@    },
  bar: 12345678 } // suffix
//@      "bar": 12345678
    var w40_= { foo: 1, bar:    1 /* xxxx */ }
//@    "w40_": {
//@      "foo": 1,
//@      "bar": 1
//@    },
var w41_={ foo: true, bar    : 1234567890 }
//@    "w41_": {
//@      "foo": true,
//@      "bar": 1234567890
//@    },
var w42_= { foo: true
//@    "w42_": {
//@      "foo": true,
//@    },
    bar: 12345678901 } // suffix
//@      "bar": 12345678901

   var w38__ =    concat('xxxxxx', 'xxxxxx')
//@    "w38__": "[concat('xxxxxx', 'xxxxxx')]",
var w39__ = concat('xxxxxx', 'xxxxxxx'
//@    "w39__": "[concat('xxxxxx', 'xxxxxxx')]",
) // suffix
var w40__ = concat('xxxxxx',
//@    "w40__": "[concat('xxxxxx', 'xxxxxxxx')]",
'xxxxxxxx') // suffix

var        w41__= concat('xxxxx', 'xxxxxxxxxx')
//@    "w41__": "[concat('xxxxx', 'xxxxxxxxxx')]",
var w42__ = concat('xxxxx', 'xxxxxxxxxxx')
//@    "w42__": "[concat('xxxxx', 'xxxxxxxxxxx')]",

var w38___ = true? 'xxxxx' : 'xxxxxx'
//@    "w38___": "[if(true(), 'xxxxx', 'xxxxxx')]",
var w39___ = true
//@    "w39___": "[if(true(), 'xxxxxx', 'xxxxxx')]",
? 'xxxxxx' : 'xxxxxx' // suffix
var w40___ = true ?'xxxxxx' : 'xxxxxxx'
//@    "w40___": "[if(true(), 'xxxxxx', 'xxxxxxx')]",
var w41___ = true ? 'xxxxxxx' :         'xxxxxxx'
//@    "w41___": "[if(true(), 'xxxxxxx', 'xxxxxxx')]",
var w42___ = true ? 'xxxxxxx':'xxxxxxxx'
//@    "w42___": "[if(true(), 'xxxxxxx', 'xxxxxxxx')]",

////////////////////////////////////////////////////////////////////////////////
//////////////////////////// Baselines for width 80 ////////////////////////////
////////////////////////////////////////////////////////////////////////////////
var w78 = [true, { foo: 'object width: 37' /* xxx */ }, 'xxxxxxxxxxxxxxxxxxx' ]
//@    "w78": [
//@      true,
//@      {
//@        "foo": "object width: 37"
//@      },
//@      "xxxxxxxxxxxxxxxxxxx"
//@    ],
var w79 = [true
//@    "w79": [
//@      true,
//@    ],
    { /* xxxx */ foo: 'object width: 38' }
//@      {
//@        "foo": "object width: 38"
//@      },
    'xxxxxxxxxxxxxxxxxx' ]
//@      "xxxxxxxxxxxxxxxxxx"
var w80 = [true, { foo: 'object width: 39 xxxxxxxxxxx' }
//@    "w80": [
//@      true,
//@      {
//@        "foo": "object width: 39 xxxxxxxxxxx"
//@      },
//@    ],
    'xxxxxxxxxxxxxxxxxxx']
//@      "xxxxxxxxxxxxxxxxxxx"
var w81 = [true, { foo: 'object width: 40 xxxxxxxxxxxx' }, 'xxxxxxxxxxxxxxxxxxx' ]
//@    "w81": [
//@      true,
//@      {
//@        "foo": "object width: 40 xxxxxxxxxxxx"
//@      },
//@      "xxxxxxxxxxxxxxxxxxx"
//@    ],
var w82 = [  true, concat(/* function width: 41 */123, 456) /* xxxxxxxxxxxxxxxx */ ]
//@    "w82": [
//@      true,
//@      "[concat(123, 456)]"
//@    ],

var w78_ ={ foo: 123, /* xxxx */ baz: ['xxxxxxxxxxx', 'xxxxxxxxxxxxxxxxxx'] }
//@    "w78_": {
//@      "foo": 123,
//@      "baz": [
//@        "xxxxxxxxxxx",
//@        "xxxxxxxxxxxxxxxxxx"
//@      ]
//@    },
var w79_ = { foo: 123, bar: true, baz: ['xxxxxxxxxxx', 'xxxxxxxx'] }
//@    "w79_": {
//@      "foo": 123,
//@      "bar": true,
//@      "baz": [
//@        "xxxxxxxxxxx",
//@        "xxxxxxxx"
//@      ]
//@    },
var w80_ = { foo: 123, bar: true, baz: ['xxxxxxxxxxx'
//@    "w80_": {
//@      "foo": 123,
//@      "bar": true,
//@      "baz": [
//@        "xxxxxxxxxxx",
//@      ]
//@    },
'xxxxxxxxxxxxxxxxxxxx'] } // suffix
//@        "xxxxxxxxxxxxxxxxxxxx"
var w81_ = { foo: 123, bar: true, baz: ['xxxxxxxxxxx', 'xxxxxxxxxxxxxxxxxxxxx'] }
//@    "w81_": {
//@      "foo": 123,
//@      "bar": true,
//@      "baz": [
//@        "xxxxxxxxxxx",
//@        "xxxxxxxxxxxxxxxxxxxxx"
//@      ]
//@    },
var w82_ = { foo: 123, bar: true, baz: ['array length: 41', 'xxxxxxxxxxxxxxxxx'] }
//@    "w82_": {
//@      "foo": 123,
//@      "bar": true,
//@      "baz": [
//@        "array length: 41",
//@        "xxxxxxxxxxxxxxxxx"
//@      ]
//@    },

var w78__ = union({ foo: 'xxxxx' }, { bar: 'xxxxxxxxx' }, { baz: 'xxxxxxxxx'})
//@    "w78__": "[union(createObject('foo', 'xxxxx'), createObject('bar', 'xxxxxxxxx'), createObject('baz', 'xxxxxxxxx'))]",
var w79__ = union({ foo: 'xxxxx' }, { bar: 'xxxxxxxxx' },
//@    "w79__": "[union(createObject('foo', 'xxxxx'), createObject('bar', 'xxxxxxxxx'), createObject('baz', 'xxxxxxxxxx'))]",
    { baz: 'xxxxxxxxxx'}) // suffix
var w80__ = union(
//@    "w80__": "[union(createObject('foo', 'xxxxxx'), createObject('bar', 'xxxxxx'), createObject('baz', 'xxxxxxxxxxxxx'))]",
    { foo: 'xxxxxx' },
    { bar: 'xxxxxx' },
    { baz: 'xxxxxxxxxxxxx'})
var w81__ = union({ foo: 'x' } /* xxx */, any({ baz: 'func call length: 38  ' }))
//@    "w81__": "[union(createObject('foo', 'x'), createObject('baz', 'func call length: 38  '))]",
var w82__ = union({ foo: 'x', bar: 'x' }, any({ baz: 'func call length: 39   ' }))
//@    "w82__": "[union(createObject('foo', 'x', 'bar', 'x'), createObject('baz', 'func call length: 39   '))]",

var w78___ = /* xxxxxxxxxxxxxxxxxxxxxxxxxxxx */ true
//@    "w78___": "[if(true(), 1234567890, 1234567890)]",
? 1234567890
: 1234567890
var w79___ = /* xxxxxxxxxxxxxxxxxxxxxxxxxxxxx */ true ? { foo: 1 } : [12345678]
//@    "w79___": "[if(true(), createObject('foo', 1), createArray(12345678))]",
var w80___ = true ? { foo: true, bar: false } : [123, 234, 456, { xyz: 'xxxx' }]
//@    "w80___": "[if(true(), createObject('foo', true(), 'bar', false()), createArray(123, 234, 456, createObject('xyz', 'xxxx')))]",
var w81___ = /* xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx */ true ? 1234567890 : 1234567890
//@    "w81___": "[if(true(), 1234567890, 1234567890)]",
var w82___ = /* xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx */ true ? 1234567890 : 1234567890
//@    "w82___": "[if(true(), 1234567890, 1234567890)]",


var w80_________ = union(/*******************************************/ {}, {}, {
//@    "w80_________": "[union(createObject(), createObject(), createObject('foo', true(), 'bar', false()))]",
    foo: true
    bar: false
})
var w81_________ = union(/********************************************/ {}, {}, {
//@    "w81_________": "[union(createObject(), createObject(), createObject('foo', true(), 'bar', false()))]",
    foo: true
    bar: false
})

var w80__________ = union(/******************************************/ {}, {}, {
//@    "w80__________": "[union(createObject(), createObject(), createObject('foo', true(), 'w80', union(createObject(), createObject('baz', 123))))]",
    foo: true
    w80: union(/***********************************************************/ {}, {
        baz: 123
    })
})

var w81__________ = union(/*******************************************/ {}, {}, {
//@    "w81__________": "[union(createObject(), createObject(), createObject('foo', true(), 'w81', union(createObject(), createObject('baz', 123))))]",
    foo: true
    w81: union(/**********************************************************/ {}, {
        baz: 123
    })
})

////////////////////////////////////////////////////////////////////////////////
////////////////////////// Baselines for line breakers /////////////////////////
////////////////////////////////////////////////////////////////////////////////
var forceBreak0 = [
//@    "forceBreak0": [
//@    ],
  1 ]
//@      1

var forceBreak1 = {
//@    "forceBreak1": {
//@    },
    foo: true
//@      "foo": true
}
var forceBreak2 = {
//@    "forceBreak2": {
//@    },
    foo: true, bar: false
//@      "foo": true,
//@      "bar": false
}
var forceBreak3 = [1, 2, {
//@    "forceBreak3": [
//@      1,
//@      2,
//@      {
//@      },
//@    ],
    foo: true }, 3, 4]
//@        "foo": true
//@      3,
//@      4
var forceBreak4 = { foo: true, bar: false // force break
//@    "forceBreak4": {
//@      "foo": true,
//@      "bar": false
//@    },
}
var forceBreak5 = { foo: true
//@    "forceBreak5": {
//@      "foo": true
//@    },
/* force break */}
var forceBreak6 = { foo: true
//@    "forceBreak6": {
//@      "foo": true,
//@    },
    bar: false
//@      "bar": false,
    baz: 123
//@      "baz": 123
/* force break */}
var forceBreak7 = [1, 2 // force break
//@    "forceBreak7": [
//@      1,
//@      2
//@    ],
]
var forceBreak8 = [1, 2
//@    "forceBreak8": [
//@      1,
//@      2
//@    ],
    /* force break */ ]
var forceBreak9 = [1, 2, {
//@    "forceBreak9": [
//@      1,
//@      2,
//@      {
//@      }
//@    ],
    foo: true
//@        "foo": true,
    bar: false
//@        "bar": false
}]
// Does not break immediate parent group, but breaks grandparent.
var forceBreak10 = [1, 2, intersection({ foo: true, bar: false }, {
//@    "forceBreak10": [
//@      1,
//@      2,
//@      "[intersection(createObject('foo', true(), 'bar', false()), createObject('foo', true()))]"
//@    ],
  foo: true})]

var forceBreak11 = true // comment
//@    "forceBreak11": "[if(true(), true(), false())]",
    ? true
    : false
var forceBreak12 = true ? true // comment
//@    "forceBreak12": "[if(true(), true(), false())]",
    : false
var forceBreak13 = true
//@    "forceBreak13": "[if(true(), true(), false())]",
    ? true // comment
    : false
var forceBreak14 = true ? {
//@    "forceBreak14": "[if(true(), createObject('foo', 42), false())]",
    foo: 42
} : false
var forceBreak15 = true ? { foo: 0 } : {
//@    "forceBreak15": "[if(true(), createObject('foo', 0), createObject('bar', 1))]",
    bar: 1}

var forceBreak16 = union({ foo: 0 }, {
//@    "forceBreak16": "[union(createObject('foo', 0), createObject('foo', 123, 'bar', 456))]"
    foo: 123
    bar: 456
} // comment
)

