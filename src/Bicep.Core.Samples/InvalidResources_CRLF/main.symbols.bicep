
// wrong declaration
bad

// incomplete
resource 
resource foo
resource fo/o
resource foo 'ddd'
resource foo 'ddd'=

// wrong resource type
resource foo 'ddd'={
//@[9:12) Resource foo. Declaration start char: 0, length: 27
}

// using string interpolation for the resource type
resource foo 'Microsoft.${provider}/foos@2020-02-02-alpha'= {
//@[9:12) Resource foo. Declaration start char: 0, length: 68
}

// missing required property
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'={
//@[9:12) Resource foo. Declaration start char: 0, length: 59
}

// duplicate property at the top level
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= {
//@[9:12) Resource foo. Declaration start char: 0, length: 89
  name: 'foo'
  name: true
}

// duplicate property at the top level with string literal syntax
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= {
//@[9:12) Resource foo. Declaration start char: 0, length: 91
  name: 'foo'
  'name': true
}

// duplicate property inside
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= {
//@[9:12) Resource foo. Declaration start char: 0, length: 125
  name: 'foo'
  properties: {
    foo: 'a'
    foo: 'a'
  }
}

// duplicate property inside with string literal syntax
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= {
//@[9:12) Resource foo. Declaration start char: 0, length: 127
  name: 'foo'
  properties: {
    foo: 'a'
    'foo': 'a'
  }
}

// wrong property types
resource foo 'Microsoft.Foo/foos@2020-02-02-alpha'= {
//@[9:12) Resource foo. Declaration start char: 0, length: 128
  name: 'foo'
  location: [
  ]
  tags: 'tag are not a string?'
}

resource bar 'Microsoft.Foo/foos@2020-02-02-alpha' = {
//@[9:12) Resource bar. Declaration start char: 0, length: 235
  name: true ? 's' : 'a' + 1
  properties: {
    x: foo()
    y: true && (null || !4)
    a: [
      a
      !null
      true && true || true + -true * 4
    ]
  }
}

// unsupported resource ref
var resrefvar = bar.name
//@[4:13) Variable resrefvar. Declaration start char: 0, length: 28

param resrefpar string = foo.id
//@[6:15) Parameter resrefpar. Declaration start char: 0, length: 35

output resrefout bool = bar.id
//@[7:16) Output resrefout. Declaration start char: 0, length: 30
