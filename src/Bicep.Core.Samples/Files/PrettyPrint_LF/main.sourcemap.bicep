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
var w40 =[
//@    "w40": [
//@    ],
    true, true, 1234/* xxxxx */]  // suffix
//@      true,
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

var        w41__= concat('xxxxx'/* xxxxxxx */)
//@    "w41__": "[concat('xxxxx')]",
var w42__ = concat('xxxxx', 'xxxxxxxxxxx')
//@    "w42__": "[concat('xxxxx', 'xxxxxxxxxxx')]",

////////////////////////////////////////////////////////////////////////////////
//////////////////////////// Baselines for width 80 ////////////////////////////
////////////////////////////////////////////////////////////////////////////////
var w78 = [
//@    "w78": [
//@    ],
    true, { foo: 'object width: 37' /* xxx */ }, 'xxxxxxxxxxxxxxxxxx' ]
//@      true,
//@      {
//@        "foo": "object width: 37"
//@      },
//@      "xxxxxxxxxxxxxxxxxx"
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
var w81 = [true, { foo: 'object width: 40 xxxxxxxxxxxx' }, 'xxxxxxxxxxxxxxxxxx' ]
//@    "w81": [
//@      true,
//@      {
//@        "foo": "object width: 40 xxxxxxxxxxxx"
//@      },
//@      "xxxxxxxxxxxxxxxxxx"
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
/* should print a newline after this */ var w79_ = { foo: 123, bar: true, baz: ['xxxxxxxxxxx', 'xxxxxxxx'] }
//@    "w79_": {
//@      "foo": 123,
//@      "bar": true,
//@      "baz": [
//@        "xxxxxxxxxxx",
//@        "xxxxxxxx"
//@      ]
//@    },
var w80_ = { foo: 123, bar: true, baz: [
//@    "w80_": {
//@      "foo": 123,
//@      "bar": true,
//@      "baz": [
//@      ]
//@    },
    'xxxxxxxxxxx', 'xxxxxxxxxxxxxxxxxxxx'] } // suffix
//@        "xxxxxxxxxxx",
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

////////////////////////////////////////////////////////////////////////////////
////////////////////////// Baselines for line breakers /////////////////////////
////////////////////////////////////////////////////////////////////////////////
var forceBreak1 = {
//@    "forceBreak1": {
//@    },
    foo: true
//@      "foo": true
}
/* should print a newline after this */var forceBreak2 = {
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
var forceBreak10 = [1, 2, intersection({ foo: true, bar: false }, {
//@    "forceBreak10": [
//@      1,
//@      2,
//@      "[intersection(createObject('foo', true(), 'bar', false()), createObject('foo', true()))]"
//@    ]
  foo: true})]

