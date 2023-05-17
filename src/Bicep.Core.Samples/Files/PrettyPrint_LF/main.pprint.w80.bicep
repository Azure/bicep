////////////////////////////////////////////////////////////////////////////////
//////////////////////////// Baselines for width 40 ////////////////////////////
////////////////////////////////////////////////////////////////////////////////
var w38 = [true, /* xxxxx */ true, 12] // suffix
var w39 = [true, true, true, true, 123]
var w40 = [true, true, 1234 /* xxxxx */] // suffix
var w41 = [true, true, true, true, 12345]
var w42 = [true, /* xxx */ 12 /* xx */, 1]

var w38_ = { foo: true, bar: 1234567 } // suffix
var w39_ = { foo: true, bar: 12345678 } // suffix
var w40_ = { foo: 1, bar: 1 /* xxxx */ }
var w41_ = { foo: true, bar: 1234567890 }
var w42_ = { foo: true, bar: 12345678901 } // suffix

var w38__ = concat('xxxxxx', 'xxxxxx')
var w39__ = concat('xxxxxx', 'xxxxxxx') // suffix
var w40__ = concat('xxxxxx', 'xxxxxxxx') // suffix

var w41__ = concat('xxxxx' /* xxxxxxx */)
var w42__ = concat('xxxxx', 'xxxxxxxxxxx')

////////////////////////////////////////////////////////////////////////////////
//////////////////////////// Baselines for width 80 ////////////////////////////
////////////////////////////////////////////////////////////////////////////////
var w78 = [true, { foo: 'object width: 37' /* xxx */ }, 'xxxxxxxxxxxxxxxxxxx']
var w79 = [true, { /* xxxx */ foo: 'object width: 38' }, 'xxxxxxxxxxxxxxxxxx']
var w80 = [true, { foo: 'object width: 39 xxxxxxxxxxx' }, 'xxxxxxxxxxxxxxxxxxx']
var w81 = [
  true
  { foo: 'object width: 40 xxxxxxxxxxxx' }
  'xxxxxxxxxxxxxxxxxxx'
]
var w82 = [
  true
  concat(/* function width: 41 */ 123, 456) /* xxxxxxxxxxxxxxxx */
]

var w78_ = { foo: 123, /* xxxx */ baz: ['xxxxxxxxxxx', 'xxxxxxxxxxxxxxxxxx'] }
var w79_ = { foo: 123, bar: true, baz: ['xxxxxxxxxxx', 'xxxxxxxx'] }
var w80_ = { foo: 123, bar: true, baz: ['xxxxxxxxxxx', 'xxxxxxxxxxxxxxxxxxxx'] } // suffix
var w81_ = {
  foo: 123
  bar: true
  baz: ['xxxxxxxxxxx', 'xxxxxxxxxxxxxxxxxxxxx']
}
var w82_ = {
  foo: 123
  bar: true
  baz: ['array length: 41', 'xxxxxxxxxxxxxxxxx']
}

var w78__ = union({ foo: 'xxxxx' }, { bar: 'xxxxxxxxx' }, { baz: 'xxxxxxxxx' })
var w79__ = union({ foo: 'xxxxx' }, { bar: 'xxxxxxxxx' }, { baz: 'xxxxxxxxxx' }) // suffix
var w80__ = union(
  { foo: 'xxxxxx' },
  { bar: 'xxxxxx' },
  { baz: 'xxxxxxxxxxxxx' }
)
var w81__ = union(
  { foo: 'x' } /* xxx */,
  any({ baz: 'func call length: 38  ' })
)
var w82__ = union(
  { foo: 'x', bar: 'x' },
  any({ baz: 'func call length: 39   ' })
)

////////////////////////////////////////////////////////////////////////////////
////////////////////////// Baselines for line breakers /////////////////////////
////////////////////////////////////////////////////////////////////////////////
var forceBreak1 = {
  foo: true
}
var forceBreak2 = {
  foo: true
  bar: false
}
var forceBreak3 = [
  1
  2
  {
    foo: true
  }
  3
  4
]
var forceBreak4 = {
  foo: true
  bar: false // force break
}
var forceBreak5 = {
  foo: true
  /* force break */
}
var forceBreak6 = {
  foo: true
  bar: false
  baz: 123
  /* force break */
}
var forceBreak7 = [
  1
  2 // force break
]
var forceBreak8 = [
  1
  2
  /* force break */
]
var forceBreak9 = [
  1
  2
  {
    foo: true
    bar: false
  }
]
var forceBreak10 = [
  1
  2
  intersection(
    { foo: true, bar: false },
    {
      foo: true
    }
  )
]
